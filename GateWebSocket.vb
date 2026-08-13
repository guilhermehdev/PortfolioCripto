Imports System.Collections.Concurrent
Imports System.Globalization
Imports System.Net.WebSockets
Imports System.Text
Imports System.Text.Json
Imports System.Threading

Public Class GateWebSocket

    Private Const WebSocketUrl As String =
        "wss://api.gateio.ws/ws/v4/"

    Private ReadOnly _prices As New ConcurrentDictionary(Of String, Decimal)(
        StringComparer.OrdinalIgnoreCase)

    Private _socket As ClientWebSocket
    Private _cts As CancellationTokenSource
    Private _receiveTask As Task
    Private _symbols As List(Of String)
    Private _reconnecting As Boolean

    Public Event PriceUpdated(symbol As String, price As Decimal)
    Public Event ConnectionStateChanged(connected As Boolean, message As String)

    Public ReadOnly Property IsConnected As Boolean
        Get
            Return _socket IsNot Nothing AndAlso
                   _socket.State = WebSocketState.Open
        End Get
    End Property

    Public Function TryGetPrice(
        symbol As String,
        ByRef price As Decimal) As Boolean

        Return _prices.TryGetValue(
            symbol.Trim().ToUpperInvariant(),
            price)

    End Function

    Public Async Function StartAsync(
        symbols As IEnumerable(Of String)) As Task

        Dim normalized As List(Of String) =
            symbols.
            Where(Function(s) Not String.IsNullOrWhiteSpace(s)).
            Select(Function(s) s.Trim().ToUpperInvariant()).
            Distinct().
            ToList()

        If normalized.Count = 0 Then

            RaiseEvent ConnectionStateChanged(
                False,
                "Nenhum símbolo Gate.io para assinar.")

            Return

        End If

        Await StopAsync()

        _symbols = normalized
        _cts = New CancellationTokenSource()
        _reconnecting = False

        Try

            Await ConnectAndSubscribeAsync(
                _cts.Token)

        Catch ex As OperationCanceledException

            RaiseEvent ConnectionStateChanged(
                False,
                "Gate.io WebSocket cancelado.")

        Catch ex As Exception

            RaiseEvent ConnectionStateChanged(
                False,
                "Erro Gate.io WebSocket: " &
                ex.Message)

        End Try

    End Function

    Private Async Function ConnectAndSubscribeAsync(
        token As CancellationToken) As Task

        Dim delay As Integer = 1000

        For attempt As Integer = 1 To 5

            token.ThrowIfCancellationRequested()

            Dim connected As Boolean = False

            Try

                If _socket IsNot Nothing Then
                    _socket.Dispose()
                    _socket = Nothing
                End If

                _socket = New ClientWebSocket()

                _socket.Options.KeepAliveInterval =
                    TimeSpan.FromSeconds(20)

                Await _socket.ConnectAsync(
                    New Uri(WebSocketUrl),
                    token)

                Dim payload As New List(Of String)

                For Each symbol In _symbols

                    payload.Add(
                        symbol & "_USDT")

                Next

                Dim request = New With {
                    .time = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                    .channel = "spot.tickers",
                    .event = "subscribe",
                    .payload = payload.ToArray()
                }

                Dim json As String =
                    JsonSerializer.Serialize(request)

                Dim bytes As Byte() =
                    Encoding.UTF8.GetBytes(json)

                Await _socket.SendAsync(
                    New ArraySegment(Of Byte)(bytes),
                    WebSocketMessageType.Text,
                    True,
                    token)

                connected = True

            Catch ex As OperationCanceledException

                Throw

            Catch ex As Exception

                If _socket IsNot Nothing Then
                    _socket.Dispose()
                    _socket = Nothing
                End If

                If attempt >= 5 Then

                    Throw New WebSocketException(
                        "Não foi possível conectar à Gate.io WebSocket após 5 tentativas.")

                End If

            End Try

            If connected Then

                RaiseEvent ConnectionStateChanged(
                    True,
                    $"Gate.io WebSocket conectado: {_symbols.Count} símbolos.")

                _receiveTask =
                    ReceiveLoopAsync(token)

                Return

            End If

            Await Task.Delay(
                delay,
                token)

            delay =
                Math.Min(
                    delay * 2,
                    8000)

        Next

    End Function

    Private Async Function ReceiveLoopAsync(
        token As CancellationToken) As Task

        Dim buffer(8191) As Byte

        Try

            While Not token.IsCancellationRequested AndAlso
                  _socket IsNot Nothing AndAlso
                  _socket.State = WebSocketState.Open

                Using ms As New IO.MemoryStream()

                    Dim result As WebSocketReceiveResult = Nothing

                    Do

                        result =
                            Await _socket.ReceiveAsync(
                                New ArraySegment(Of Byte)(buffer),
                                token)

                        If result.MessageType =
                           WebSocketMessageType.Close Then

                            Throw New WebSocketException(
                                "Gate.io fechou a conexão.")

                        End If

                        If result.Count > 0 Then

                            ms.Write(
                                buffer,
                                0,
                                result.Count)

                        End If

                    Loop Until result.EndOfMessage

                    ProcessMessage(
                        Encoding.UTF8.GetString(
                            ms.ToArray()))

                End Using

            End While

        Catch ex As OperationCanceledException

            Return

        Catch ex As Exception

            RaiseEvent ConnectionStateChanged(
                False,
                "Gate.io WebSocket desconectado: " &
                ex.Message)

            StartReconnect(token)

        End Try

    End Function

    Private Sub StartReconnect(
        token As CancellationToken)

        If token.IsCancellationRequested OrElse
           _reconnecting Then

            Return

        End If

        _reconnecting = True

        Task.Run(
            Async Function()

                Try

                    Await Task.Delay(
                        2000,
                        token)

                    If token.IsCancellationRequested Then
                        Return
                    End If

                    RaiseEvent ConnectionStateChanged(
                        False,
                        "Reconectando Gate.io WebSocket...")

                    Await ConnectAndSubscribeAsync(
                        token)

                Catch ex As OperationCanceledException

                    Return

                Catch ex As Exception

                    RaiseEvent ConnectionStateChanged(
                        False,
                        "Falha na reconexão Gate.io: " &
                        ex.Message)

                End Try

                _reconnecting = False

            End Function)

    End Sub

    Private Sub ProcessMessage(
        json As String)

        Try

            Using document As JsonDocument =
                JsonDocument.Parse(json)

                Dim root As JsonElement =
                    document.RootElement

                Dim result As JsonElement

                If Not root.TryGetProperty(
                    "result",
                    result) Then

                    Return

                End If

                Dim pairElement As JsonElement
                Dim lastElement As JsonElement

                If Not result.TryGetProperty(
                    "currency_pair",
                    pairElement) Then

                    Return

                End If

                If Not result.TryGetProperty(
                    "last",
                    lastElement) Then

                    Return

                End If

                Dim pair As String =
                    pairElement.GetString()

                Dim priceText As String =
                    lastElement.GetString()

                If String.IsNullOrWhiteSpace(pair) OrElse
                   String.IsNullOrWhiteSpace(priceText) Then

                    Return

                End If

                Dim symbol As String =
                    pair.Replace(
                        "_USDT",
                        String.Empty,
                        StringComparison.OrdinalIgnoreCase)

                Dim price As Decimal

                If Decimal.TryParse(
                    priceText,
                    NumberStyles.Float,
                    CultureInfo.InvariantCulture,
                    price) Then

                    _prices(symbol) =
                        price

                    Debug.WriteLine(
                        $"[GATE WS] {symbol} = {priceText}")

                    RaiseEvent PriceUpdated(
                        symbol,
                        price)

                End If

            End Using

        Catch ex As JsonException

            Debug.WriteLine(
                "Gate.io WebSocket JSON inválido: " &
                ex.Message)

        Catch ex As Exception

            Debug.WriteLine(
                "Erro processando Gate.io WebSocket: " &
                ex.Message)

        End Try

    End Sub

    Public Async Function StopAsync() As Task

        Dim socketToClose As ClientWebSocket =
            _socket

        Dim receiveTaskToWait As Task =
            _receiveTask

        Dim ctsToDispose As CancellationTokenSource =
            _cts

        _socket = Nothing
        _receiveTask = Nothing
        _cts = Nothing
        _reconnecting = False

        If ctsToDispose IsNot Nothing Then
            ctsToDispose.Cancel()
        End If

        If receiveTaskToWait IsNot Nothing Then

            Try
                Await receiveTaskToWait
            Catch
            End Try

        End If

        If socketToClose IsNot Nothing Then

            If socketToClose.State =
               WebSocketState.Open OrElse
               socketToClose.State =
               WebSocketState.CloseReceived Then

                Dim closeTokenSource As New CancellationTokenSource(
                    TimeSpan.FromSeconds(2))

                Try

                    Await socketToClose.CloseAsync(
                        WebSocketCloseStatus.NormalClosure,
                        "Encerrando",
                        closeTokenSource.Token)

                Catch
                End Try

                closeTokenSource.Dispose()

            End If

            socketToClose.Dispose()

        End If

        If ctsToDispose IsNot Nothing Then
            ctsToDispose.Dispose()
        End If

        RaiseEvent ConnectionStateChanged(
            False,
            "Gate.io WebSocket parado.")

    End Function

End Class