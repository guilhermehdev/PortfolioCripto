Imports System.Collections.Concurrent
Imports System.Globalization
Imports System.Net.WebSockets
Imports System.Text
Imports System.Text.Json
Imports System.Threading

''' <summary>
''' Stream público de ticker/mark price dos futuros USDT da Gate.
''' </summary>
Public Class GateFuturesMarketWebSocket
    Private Const WebSocketUrl As String = "wss://fx-ws.gateio.ws/v4/ws/usdt"
    Private ReadOnly _prices As New ConcurrentDictionary(Of String, Decimal)(StringComparer.OrdinalIgnoreCase)
    Private _socket As ClientWebSocket
    Private _cts As CancellationTokenSource
    Private _supervisorTask As Task
    Private _symbols As List(Of String) = New List(Of String)()

    Public Event MarkPriceUpdated(symbol As String, price As Decimal)
    Public Event ConnectionStateChanged(connected As Boolean, message As String)

    Public Function TryGetPrice(symbol As String, ByRef price As Decimal) As Boolean
        Return _prices.TryGetValue(Normalize(symbol), price)
    End Function

    Public Async Function StartAsync(symbols As IEnumerable(Of String)) As Task
        Dim normalized = symbols.
            Where(Function(symbol) Not String.IsNullOrWhiteSpace(symbol)).
            Select(Function(symbol) Normalize(symbol)).
            Where(Function(symbol) symbol.EndsWith("_USDT", StringComparison.OrdinalIgnoreCase)).
            Distinct(StringComparer.OrdinalIgnoreCase).
            ToList()
        If normalized.Count = 0 Then Return

        Await StopAsync()
        _symbols = normalized
        _cts = New CancellationTokenSource()
        _supervisorTask = SupervisorAsync(_cts.Token)
    End Function

    Private Async Function SupervisorAsync(token As CancellationToken) As Task
        Dim delay = 2000
        While Not token.IsCancellationRequested
            Try
                Await ConnectAndReceiveAsync(token)
                delay = 2000
            Catch ex As OperationCanceledException
                Exit While
            Catch ex As Exception
                If Not token.IsCancellationRequested Then RaiseEvent ConnectionStateChanged(False, "Gate Futures WebSocket: " & ex.Message)
            Finally
                CloseSocket()
            End Try
            If Not token.IsCancellationRequested Then
                Try
                    Await Task.Delay(delay, token)
                Catch
                    Exit While
                End Try
                delay = Math.Min(delay * 2, 15000)
            End If
        End While
    End Function

    Private Async Function ConnectAndReceiveAsync(token As CancellationToken) As Task
        CloseSocket()
        _socket = New ClientWebSocket()
        _socket.Options.KeepAliveInterval = TimeSpan.FromSeconds(20)
        Await _socket.ConnectAsync(New Uri(WebSocketUrl), token)

        Dim request = New With {
            .time = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            .channel = "futures.tickers",
            .event = "subscribe",
            .payload = _symbols.ToArray()
        }
        Dim bytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(request))
        Await _socket.SendAsync(New ArraySegment(Of Byte)(bytes), WebSocketMessageType.Text, True, token)
        RaiseEvent ConnectionStateChanged(True, $"Gate Futures WebSocket conectado: {_symbols.Count} símbolos.")

        Dim buffer(8191) As Byte
        While Not token.IsCancellationRequested AndAlso _socket.State = WebSocketState.Open
            Using ms As New IO.MemoryStream()
                Dim result As WebSocketReceiveResult
                Do
                    result = Await _socket.ReceiveAsync(New ArraySegment(Of Byte)(buffer), token)
                    If result.MessageType = WebSocketMessageType.Close Then Throw New WebSocketException("Gate fechou o stream de Futures.")
                    If result.Count > 0 Then ms.Write(buffer, 0, result.Count)
                Loop Until result.EndOfMessage
                ProcessMessage(Encoding.UTF8.GetString(ms.ToArray()))
            End Using
        End While
    End Function

    Private Sub ProcessMessage(json As String)
        Try
            Using document = JsonDocument.Parse(json)
                Dim root = document.RootElement
                Dim result As JsonElement
                If Not root.TryGetProperty("result", result) OrElse result.ValueKind <> JsonValueKind.Array Then Return
                For Each item In result.EnumerateArray()
                    Dim contract As String = Nothing
                    Dim contractElement As JsonElement
                    If item.TryGetProperty("contract", contractElement) Then contract = contractElement.GetString()
                    If String.IsNullOrWhiteSpace(contract) Then Continue For
                    Dim priceText As String = Nothing
                    Dim priceElement As JsonElement
                    If item.TryGetProperty("mark_price", priceElement) Then priceText = priceElement.GetString()
                    If String.IsNullOrWhiteSpace(priceText) AndAlso item.TryGetProperty("last", priceElement) Then priceText = priceElement.GetString()
                    Dim price As Decimal
                    If Not Decimal.TryParse(priceText, NumberStyles.Float, CultureInfo.InvariantCulture, price) Then Continue For
                    Dim symbol = contract.Replace("_", String.Empty, StringComparison.OrdinalIgnoreCase).ToUpperInvariant()
                    _prices(symbol) = price
                    RaiseEvent MarkPriceUpdated(symbol, price)
                Next
            End Using
        Catch ex As Exception
            Debug.WriteLine("Erro processando Gate Futures WebSocket: " & ex.Message)
        End Try
    End Sub

    Private Shared Function Normalize(symbol As String) As String
        Dim value = If(symbol, String.Empty).Trim().ToUpperInvariant().Replace("-", "_")
        If value.EndsWith("USDT", StringComparison.OrdinalIgnoreCase) AndAlso Not value.EndsWith("_USDT", StringComparison.OrdinalIgnoreCase) Then value = value.Substring(0, value.Length - 4) & "_USDT"
        If Not value.EndsWith("_USDT", StringComparison.OrdinalIgnoreCase) Then value &= "_USDT"
        Return value
    End Function

    Private Sub CloseSocket()
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
        CloseSocket()
        If supervisor IsNot Nothing Then
            Try
                Await Task.WhenAny(supervisor, Task.Delay(TimeSpan.FromSeconds(2)))
            Catch
            End Try
        End If
        If cts IsNot Nothing Then cts.Dispose()
    End Function
End Class
