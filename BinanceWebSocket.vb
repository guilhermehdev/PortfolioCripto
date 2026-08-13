Imports System.Collections.Concurrent
Imports System.Globalization
Imports System.Net.WebSockets
Imports System.Text
Imports System.Text.Json

Public Class BinanceWebSocket

    Private Const WebSocketUrl As String = "wss://stream.binance.com:9443/ws"

    Private ReadOnly _prices As New ConcurrentDictionary(Of String, Decimal)(StringComparer.OrdinalIgnoreCase)
    Private ReadOnly _http As New Net.Http.HttpClient()
    Private _socket As ClientWebSocket
    Private _cts As CancellationTokenSource
    Private _receiveTask As Task
    Private _symbols As List(Of String)

    Public Event PriceUpdated(symbol As String, price As Decimal)
    Public Event ConnectionStateChanged(connected As Boolean, message As String)

    Public ReadOnly Property IsConnected As Boolean
        Get
            Return _socket IsNot Nothing AndAlso _socket.State = WebSocketState.Open
        End Get
    End Property

    Public Function TryGetPrice(symbol As String, ByRef price As Decimal) As Boolean
        Return _prices.TryGetValue(symbol.ToUpperInvariant(), price)
    End Function

    Public Async Function StartAsync(symbols As IEnumerable(Of String)) As Task

        Dim normalized = symbols.
            Where(Function(s) Not String.IsNullOrWhiteSpace(s)).
            Select(Function(s) s.Trim().ToUpperInvariant()).
            Distinct().
            ToList()

        If normalized.Count = 0 Then
            RaiseEvent ConnectionStateChanged(False, "Nenhum símbolo Binance para assinar.")
            Return
        End If

        _symbols = normalized

        Await StopAsync()

        _cts = New CancellationTokenSource()
        _socket = New ClientWebSocket()
        _socket.Options.KeepAliveInterval = TimeSpan.FromSeconds(20)

        Try
            Await ConnectWithRetryAsync(_cts.Token)

            Dim streamUrl As String = BuildCombinedStreamUrl(_symbols)

            ' Recria o socket para usar combined streams.
            Await StopSocketOnlyAsync()

            _socket = New ClientWebSocket()
            _socket.Options.KeepAliveInterval = TimeSpan.FromSeconds(20)

            Await _socket.ConnectAsync(New Uri(streamUrl), _cts.Token)

            RaiseEvent ConnectionStateChanged(True, $"Binance WebSocket conectado: {_symbols.Count} símbolos.")

            _receiveTask = ReceiveLoopAsync(_cts.Token)

        Catch ex As Exception
            RaiseEvent ConnectionStateChanged(False, "Erro Binance WebSocket: " & ex.Message)
        End Try

    End Function

    Private Async Function ConnectWithRetryAsync(token As CancellationToken) As Task
        Dim delay As Integer = 1000

        For attempt As Integer = 1 To 5

            token.ThrowIfCancellationRequested()

            Try
                Await _socket.ConnectAsync(New Uri(WebSocketUrl), token)
                Return

            Catch ex As Exception When attempt < 5
                Await Task.Delay(delay, token)
                delay = Math.Min(delay * 2, 8000)
            End Try

        Next

        Throw New WebSocketException("Não foi possível conectar ao Binance WebSocket.")
    End Function

    Private Function BuildCombinedStreamUrl(symbols As IEnumerable(Of String)) As String

        Dim streams = symbols.
            Select(Function(s) s.ToLowerInvariant() & "@ticker")

        Return "wss://stream.binance.com:9443/stream?streams=" &
               String.Join("/", streams)

    End Function

    Private Async Function ReceiveLoopAsync(token As CancellationToken) As Task

        Dim buffer(8191) As Byte

        Try

            While Not token.IsCancellationRequested AndAlso
                  _socket IsNot Nothing AndAlso
                  _socket.State = WebSocketState.Open

                Using ms As New IO.MemoryStream()

                    Dim result As WebSocketReceiveResult

                    Do
                        result = Await _socket.ReceiveAsync(
                            New ArraySegment(Of Byte)(buffer),
                            token)

                        If result.MessageType = WebSocketMessageType.Close Then
                            Throw New WebSocketException("Binance fechou a conexão.")
                        End If

                        If result.Count > 0 Then
                            ms.Write(buffer, 0, result.Count)
                        End If

                    Loop Until result.EndOfMessage

                    Dim json As String =
                        Encoding.UTF8.GetString(ms.ToArray())

                    ProcessMessage(json)

                End Using

            End While

        Catch ex As OperationCanceledException
            ' Encerramento normal.

        Catch ex As Exception
            RaiseEvent ConnectionStateChanged(False, "Binance WebSocket desconectado: " & ex.Message)

            If Not token.IsCancellationRequested Then
                _ = ReconnectAsync(token)
            End If

        End Try

    End Function

    Private Sub ProcessMessage(json As String)

        Try
            Using document As JsonDocument = JsonDocument.Parse(json)

                Dim root As JsonElement = document.RootElement

                If Not root.TryGetProperty("data", root) Then
                    Return
                End If

                If Not root.TryGetProperty("s", root) Then
                    Return
                End If

                If Not root.TryGetProperty("c", root) Then
                    Return
                End If

                Dim symbol As String =
                    root.GetProperty("s").GetString()

                Dim priceText As String =
                    root.GetProperty("c").GetString()

                If String.IsNullOrWhiteSpace(symbol) OrElse
                   String.IsNullOrWhiteSpace(priceText) Then
                    Return
                End If

                Dim price As Decimal

                If Decimal.TryParse(
                    priceText,
                    NumberStyles.Float,
                    CultureInfo.InvariantCulture,
                    price) Then

                    _prices(symbol) = price
                    RaiseEvent PriceUpdated(symbol, price)

                End If

            End Using

        Catch ex As JsonException
            Debug.WriteLine("Binance WebSocket JSON inválido: " & ex.Message)
        Catch ex As Exception
            Debug.WriteLine("Erro processando Binance WebSocket: " & ex.Message)
        End Try

    End Sub

    Private Async Function ReconnectAsync(token As CancellationToken) As Task

        If token.IsCancellationRequested Then
            Return
        End If

        RaiseEvent ConnectionStateChanged(False, "Reconectando Binance WebSocket...")

        Try
            Await Task.Delay(2000, token)

            If token.IsCancellationRequested Then
                Return
            End If

            Await StartInternalAsync(token)

        Catch ex As OperationCanceledException
        Catch ex As Exception
            RaiseEvent ConnectionStateChanged(False, "Falha na reconexão Binance: " & ex.Message)
        End Try

    End Function

    Private Async Function StartInternalAsync(token As CancellationToken) As Task

        Await StopSocketOnlyAsync()

        _socket = New ClientWebSocket()
        _socket.Options.KeepAliveInterval = TimeSpan.FromSeconds(20)

        Dim streamUrl As String = BuildCombinedStreamUrl(_symbols)

        Await _socket.ConnectAsync(New Uri(streamUrl), token)

        RaiseEvent ConnectionStateChanged(True, "Binance WebSocket reconectado.")

        _receiveTask = ReceiveLoopAsync(token)

    End Function

    Public Async Function StopAsync() As Task

        If _cts IsNot Nothing Then
            _cts.Cancel()
        End If

        Await StopSocketOnlyAsync()

        If _cts IsNot Nothing Then
            _cts.Dispose()
            _cts = Nothing
        End If

        _receiveTask = Nothing

        RaiseEvent ConnectionStateChanged(False, "Binance WebSocket parado.")

    End Function

    Private Async Function StopSocketOnlyAsync() As Task

        If _socket Is Nothing Then
            Return
        End If

        Try
            If _socket.State = WebSocketState.Open OrElse
               _socket.State = WebSocketState.CloseReceived Then

                Using localCts As New CancellationTokenSource(TimeSpan.FromSeconds(2))
                    Await _socket.CloseAsync(
                        WebSocketCloseStatus.NormalClosure,
                        "Encerrando",
                        localCts.Token)
                End Using

            End If

        Catch
            ' Ignora erros de fechamento.
        Finally
            _socket.Dispose()
            _socket = Nothing
        End Try

    End Function

End Class
