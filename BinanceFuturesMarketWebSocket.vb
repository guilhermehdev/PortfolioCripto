Imports System.Threading
Imports System.Globalization
Imports System.Net.WebSockets
Imports System.Text
Imports System.Text.Json

Public Class BinanceFuturesMarketWebSocket
    Private Const WebSocketBaseUrl As String = "wss://fstream.binance.com/market/stream?streams="

    Private _socket As ClientWebSocket
    Private _cts As CancellationTokenSource
    Private _supervisorTask As Task
    Private _receiveTask As Task
    Private _symbols As List(Of String) = New List(Of String)()

    Public Event MarkPriceUpdated(symbol As String, price As Decimal)
    Public Event ConnectionStateChanged(connected As Boolean, message As String)

    Public Async Function StartAsync(symbols As IEnumerable(Of String)) As Task
        Dim normalized = symbols.
            Where(Function(symbol) Not String.IsNullOrWhiteSpace(symbol)).
            Select(Function(symbol) symbol.Trim().ToUpperInvariant()).
            Where(Function(symbol) symbol.EndsWith("USDT", StringComparison.OrdinalIgnoreCase)).
            Distinct().
            ToList()

        If normalized.Count = 0 Then
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
                Await ConnectAndReceiveAsync(token)
                delay = 2000
            Catch ex As OperationCanceledException
                Exit While
            Catch ex As Exception
                If Not token.IsCancellationRequested Then
                    RaiseEvent ConnectionStateChanged(False, "Binance Futures WebSocket: " & ex.Message)
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

    Private Async Function ConnectAndReceiveAsync(token As CancellationToken) As Task
        CloseCurrentSocket()
        _socket = New ClientWebSocket()
        _socket.Options.KeepAliveInterval = TimeSpan.FromSeconds(20)

        Await _socket.ConnectAsync(New Uri(BuildStreamUrl()), token)
        RaiseEvent ConnectionStateChanged(True, $"Binance Futures WebSocket conectado: {_symbols.Count} símbolos.")

        _receiveTask = ReceiveLoopAsync(token)
        Await _receiveTask
    End Function

    Private Function BuildStreamUrl() As String
        Dim streams = _symbols.Select(Function(symbol) symbol.ToLowerInvariant() & "@markPrice@1s")
        Return WebSocketBaseUrl & String.Join("/", streams)
    End Function

    Private Async Function ReceiveLoopAsync(token As CancellationToken) As Task
        Dim buffer(8191) As Byte

        While Not token.IsCancellationRequested AndAlso
              _socket IsNot Nothing AndAlso
              _socket.State = WebSocketState.Open

            Using ms As New IO.MemoryStream()
                Dim result As WebSocketReceiveResult = Nothing
                Do
                    result = Await _socket.ReceiveAsync(New ArraySegment(Of Byte)(buffer), token)
                    If result.MessageType = WebSocketMessageType.Close Then
                        Throw New WebSocketException("Binance fechou o stream de Futures.")
                    End If
                    If result.Count > 0 Then
                        ms.Write(buffer, 0, result.Count)
                    End If
                Loop Until result.EndOfMessage

                ProcessMessage(Encoding.UTF8.GetString(ms.ToArray()))
            End Using
        End While
    End Function

    Private Sub ProcessMessage(json As String)
        Try
            Using document = JsonDocument.Parse(json)
                Dim root = document.RootElement
                Dim data As JsonElement
                If Not root.TryGetProperty("data", data) Then Return

                Dim symbolElement As JsonElement
                Dim priceElement As JsonElement
                If Not data.TryGetProperty("s", symbolElement) OrElse
                   Not data.TryGetProperty("p", priceElement) Then Return

                Dim symbol = symbolElement.GetString()
                Dim priceText = priceElement.GetString()
                Dim price As Decimal
                If String.IsNullOrWhiteSpace(symbol) OrElse
                   Not Decimal.TryParse(priceText, NumberStyles.Float, CultureInfo.InvariantCulture, price) Then
                    Return
                End If

                RaiseEvent MarkPriceUpdated(symbol.Trim().ToUpperInvariant(), price)
            End Using
        Catch ex As Exception
            Debug.WriteLine("Erro processando Binance Futures WebSocket: " & ex.Message)
        End Try
    End Sub

    Private Sub CloseCurrentSocket()
        Dim socket = _socket
        _socket = Nothing
        If socket Is Nothing Then Return
        Try
            socket.Abort()
        Catch
        End Try
        Try
            socket.Dispose()
        Catch
        End Try
    End Sub

    Public Async Function StopAsync() As Task
        Dim cts = _cts
        Dim supervisor = _supervisorTask
        _cts = Nothing
        _supervisorTask = Nothing

        If cts IsNot Nothing Then cts.Cancel()
        CloseCurrentSocket()

        If supervisor IsNot Nothing Then
            Try
                Await Task.WhenAny(supervisor, Task.Delay(TimeSpan.FromSeconds(2)))
            Catch
            End Try
        End If

        _receiveTask = Nothing
        If cts IsNot Nothing Then cts.Dispose()
    End Function
End Class
