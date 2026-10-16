Imports System.Collections.Concurrent
Imports System.Globalization
Imports System.Net.WebSockets
Imports System.Text
Imports System.Text.Json
Imports System.Threading

Public Class GateWebSocket

    Private Const WebSocketUrl As String = "wss://api.gateio.ws/ws/v4/"

    Private ReadOnly _prices As New ConcurrentDictionary(Of String, Decimal)(StringComparer.OrdinalIgnoreCase)
    Private _socket As ClientWebSocket
    Private _cts As CancellationTokenSource
    Private _receiveTask As Task
    Private _supervisorTask As Task
    Private _symbols As List(Of String)

    Public Event PriceUpdated(symbol As String, price As Decimal)
    Public Event ConnectionStateChanged(connected As Boolean, message As String)

    Public ReadOnly Property IsConnected As Boolean
        Get
            Return _socket IsNot Nothing AndAlso _socket.State = WebSocketState.Open
        End Get
    End Property

    Public Function TryGetPrice(symbol As String, ByRef price As Decimal) As Boolean
        Return _prices.TryGetValue(symbol.Trim().ToUpperInvariant(), price)
    End Function

    Public Async Function StartAsync(symbols As IEnumerable(Of String)) As Task

        Dim normalized As List(Of String) = symbols.
            Where(Function(s) Not String.IsNullOrWhiteSpace(s)).
            Select(Function(s) s.Trim().ToUpperInvariant()).
            Distinct().
            ToList()

        If normalized.Count = 0 Then
            RaiseEvent ConnectionStateChanged(False, "Nenhum símbolo Gate.io para assinar.")
            Return
        End If

        Await StopAsync()

        _symbols = normalized
        _cts = New CancellationTokenSource()

        _supervisorTask = ConnectionSupervisorAsync(_cts.Token)

    End Function

    Private Async Function ConnectionSupervisorAsync(token As CancellationToken) As Task

        Dim delay As Integer = 2000

        While Not token.IsCancellationRequested

            Try

                Await ConnectOnceAsync(token)

                RaiseEvent ConnectionStateChanged(
                    True,
                    $"Gate.io WebSocket conectado: {_symbols.Count} símbolos.")

                delay = 2000

                _receiveTask = ReceiveLoopAsync(token)
                Await _receiveTask

                If Not token.IsCancellationRequested Then
                    RaiseEvent ConnectionStateChanged(
                        False,
                        "Gate.io WebSocket desconectado. Reconectando...")
                End If

            Catch ex As OperationCanceledException

                Exit While

            Catch ex As Exception

                If Not token.IsCancellationRequested Then
                    RaiseEvent ConnectionStateChanged(
                        False,
                        "Gate.io WebSocket: " & ex.Message)
                End If

            Finally

                CloseCurrentSocket()
                _receiveTask = Nothing

            End Try

            If Not token.IsCancellationRequested Then

                Try
                    Await Task.Delay(delay, token)
                Catch ex As OperationCanceledException
                    Exit While
                End Try

                delay = Math.Min(delay * 2, 15000)

            End If

        End While

    End Function

    Private Async Function ConnectOnceAsync(token As CancellationToken) As Task

        CloseCurrentSocket()

        _socket = New ClientWebSocket()
        _socket.Options.KeepAliveInterval = TimeSpan.FromSeconds(20)

        Await _socket.ConnectAsync(
            New Uri(WebSocketUrl),
            token)

        Dim payload As New List(Of String)

        For Each symbol In _symbols
            payload.Add(symbol & "_USDT")
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

    End Function

    Private Async Function ReceiveLoopAsync(token As CancellationToken) As Task

        Dim buffer(8191) As Byte

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

                    If result.MessageType = WebSocketMessageType.Close Then
                        Throw New WebSocketException("Gate.io fechou a conexão.")
                    End If

                    If result.Count > 0 Then
                        ms.Write(buffer, 0, result.Count)
                    End If

                Loop Until result.EndOfMessage

                ProcessMessage(
                    Encoding.UTF8.GetString(ms.ToArray()))

            End Using

        End While

    End Function

    Private Sub ProcessMessage(json As String)

        Try

            Using document As JsonDocument =
                JsonDocument.Parse(json)

                Dim root As JsonElement = document.RootElement
                Dim result As JsonElement

                If Not root.TryGetProperty("result", result) Then
                    Return
                End If

                Dim pairElement As JsonElement
                Dim lastElement As JsonElement

                If Not result.TryGetProperty("currency_pair", pairElement) OrElse
                   Not result.TryGetProperty("last", lastElement) Then
                    Return
                End If

                Dim pair As String = pairElement.GetString()
                Dim priceText As String = lastElement.GetString()

                If String.IsNullOrWhiteSpace(pair) OrElse
                   String.IsNullOrWhiteSpace(priceText) Then
                    Return
                End If

                Dim symbol As String =
                    pair.Replace("_USDT", String.Empty, StringComparison.OrdinalIgnoreCase)

                Dim price As Decimal

                If Not Decimal.TryParse(
                    priceText,
                    NumberStyles.Float,
                    CultureInfo.InvariantCulture,
                    price) Then
                    Return
                End If

                _prices(symbol) = price

                Debug.WriteLine(
                    $"[GATE WS] {symbol} = {price.ToString(CultureInfo.InvariantCulture)}")

                RaiseEvent PriceUpdated(symbol, price)

            End Using

        Catch ex As JsonException

            Debug.WriteLine(
                "Gate.io WebSocket JSON inválido: " & ex.Message)

        Catch ex As Exception

            Debug.WriteLine(
                "Erro processando Gate.io WebSocket: " & ex.Message)

        End Try

    End Sub

    Private Sub CloseCurrentSocket()

        Dim socket As ClientWebSocket = _socket
        _socket = Nothing

        If socket IsNot Nothing Then

            Try
                socket.Abort()
            Catch
            End Try

            Try
                socket.Dispose()
            Catch
            End Try

        End If

    End Sub

    Public Async Function StopAsync() As Task

        Dim cts As CancellationTokenSource = _cts
        Dim supervisor As Task = _supervisorTask

        _cts = Nothing
        _supervisorTask = Nothing

        If cts IsNot Nothing Then
            cts.Cancel()
        End If

        CloseCurrentSocket()

        If supervisor IsNot Nothing Then

            Try
                Await Task.WhenAny(
                    supervisor,
                    Task.Delay(TimeSpan.FromSeconds(2)))
            Catch
            End Try

        End If

        If _receiveTask IsNot Nothing Then
            _receiveTask = Nothing
        End If

        If cts IsNot Nothing Then
            cts.Dispose()
        End If

        RaiseEvent ConnectionStateChanged(
            False,
            "Gate.io WebSocket parado.")

    End Function

End Class
