Imports System.Collections.Concurrent
Imports System.Globalization
Imports System.Linq
Imports System.Net
Imports System.Net.WebSockets
Imports System.Text
Imports System.Text.Json
Imports System.Threading

Public Class BinanceWebSocket

    Private Const WebSocketBaseUrl As String = "wss://stream.binance.com:9443/stream?streams="

    Private ReadOnly _prices As New ConcurrentDictionary(Of String, Decimal)(StringComparer.OrdinalIgnoreCase)
    Private ReadOnly _executionSnapshots As New ConcurrentDictionary(Of String, SpotBotExecutionSnapshot)(StringComparer.OrdinalIgnoreCase)
    Private _socket As ClientWebSocket
    Private _cts As CancellationTokenSource
    Private _receiveTask As Task
    Private _supervisorTask As Task
    Private _symbols As List(Of String)
    Private _klineIntervals As List(Of String)
    Private _lastMessageUtc As DateTime
    Private _lastConnectedUtc As DateTime
    Private _reconnectCount As Integer

    Public ReadOnly Property LastMessageUtc As DateTime
        Get
            Return _lastMessageUtc
        End Get
    End Property

    Public ReadOnly Property LastConnectedUtc As DateTime
        Get
            Return _lastConnectedUtc
        End Get
    End Property

    Public ReadOnly Property ReconnectCount As Integer
        Get
            Return _reconnectCount
        End Get
    End Property

    Public Event PriceUpdated(symbol As String, price As Decimal)
    Public Event VolumeUpdated(symbol As String, price As Decimal, quoteVolume24h As Decimal)
    Public Event CandleUpdated(
        symbol As String,
        interval As String,
        candleStartUtc As DateTime,
        openPrice As Decimal,
        highPrice As Decimal,
        lowPrice As Decimal,
        closePrice As Decimal,
        volume As Decimal,
        isClosed As Boolean)
    Public Event ExecutionSnapshotUpdated(symbol As String, snapshot As SpotBotExecutionSnapshot)
    Public Event ConnectionStateChanged(connected As Boolean, message As String)
    Public Event ConnectionAttentionRequired(message As String)

    Public ReadOnly Property IsConnected As Boolean
        Get
            Return _socket IsNot Nothing AndAlso _socket.State = WebSocketState.Open
        End Get
    End Property

    Public Function TryGetPrice(symbol As String, ByRef price As Decimal) As Boolean
        Return _prices.TryGetValue(symbol.Trim().ToUpperInvariant(), price)
    End Function

    Public Function TryGetExecutionSnapshot(symbol As String, ByRef snapshot As SpotBotExecutionSnapshot) As Boolean
        If String.IsNullOrWhiteSpace(symbol) Then
            snapshot = Nothing
            Return False
        End If
        If _executionSnapshots.TryGetValue(symbol.Trim().ToUpperInvariant(), snapshot) AndAlso
           DateTime.UtcNow - snapshot.CapturedAtUtc <= SpotBotExecutionBookReplay.MaximumBookAge Then Return True
        snapshot = Nothing
        Return False
    End Function

    Public Function GetExecutionSnapshots() As IReadOnlyDictionary(Of String, SpotBotExecutionSnapshot)
        Dim oldest = DateTime.UtcNow - SpotBotExecutionBookReplay.MaximumBookAge
        Return _executionSnapshots.Where(Function(item) item.Value.CapturedAtUtc >= oldest).
            ToDictionary(Function(item) item.Key, Function(item) item.Value, StringComparer.OrdinalIgnoreCase)
    End Function

    Public Async Function StartAsync(
        symbols As IEnumerable(Of String),
        Optional klineIntervals As IEnumerable(Of String) = Nothing) As Task

        Dim normalized As List(Of String) = symbols.
            Where(Function(s) Not String.IsNullOrWhiteSpace(s)).
            Select(Function(s) s.Trim().ToUpperInvariant()).
            Distinct().
            ToList()

        If Not normalized.Contains("BTC") Then
            normalized.Add("BTC")
        End If

        normalized = normalized.
            Where(Function(s) Not IsStablecoin(s) OrElse s = "BTC").
            Distinct().
            ToList()

        If normalized.Count = 0 Then
            RaiseEvent ConnectionStateChanged(False, "Nenhum ativo Binance com par USDT para assinar.")
            Return
        End If

        Dim intervals = If(klineIntervals, Enumerable.Empty(Of String)()).
            Select(Function(item) NormalizeKlineInterval(item)).
            Where(Function(item) Not String.IsNullOrWhiteSpace(item)).
            Distinct(StringComparer.OrdinalIgnoreCase).
            ToList()
        If _supervisorTask IsNot Nothing AndAlso Not _supervisorTask.IsCompleted AndAlso
           New HashSet(Of String)(_symbols, StringComparer.OrdinalIgnoreCase).SetEquals(normalized) AndAlso
           New HashSet(Of String)(_klineIntervals, StringComparer.OrdinalIgnoreCase).SetEquals(intervals) Then Return

        Await StopAsync()

        _symbols = normalized
        _klineIntervals = intervals
        _cts = New CancellationTokenSource()
        _lastMessageUtc = DateTime.MinValue
        _lastConnectedUtc = DateTime.MinValue
        _reconnectCount = 0
        _executionSnapshots.Clear()

        _supervisorTask = ConnectionSupervisorAsync(_cts.Token)

    End Function

    Private Shared Function IsStablecoin(symbol As String) As Boolean
        Select Case symbol.Trim().ToUpperInvariant()
            Case "USDT", "USDC", "BUSD", "DAI", "FDUSD", "TUSD", "USDP", "GUSD"
                Return True
            Case Else
                Return False
        End Select
    End Function

    Private Async Function ConnectionSupervisorAsync(token As CancellationToken) As Task

        Dim delay As Integer = 2000

        While Not token.IsCancellationRequested
            Dim connectedAt As DateTime = DateTime.MinValue

            Try

                Await ConnectOnceAsync(token)

                If _lastConnectedUtc <> DateTime.MinValue Then
                    _reconnectCount += 1
                End If
                connectedAt = DateTime.UtcNow
                _lastConnectedUtc = connectedAt
                _lastMessageUtc = DateTime.MinValue
                RaiseEvent ConnectionStateChanged(
                    True,
                    $"Binance WebSocket conectado: {_symbols.Count} símbolos" &
                    If(_reconnectCount > 0, $" (reconexão {_reconnectCount})", String.Empty) & ".")

                _receiveTask = ReceiveLoopAsync(token)
                Await _receiveTask

                If Not token.IsCancellationRequested Then
                    RaiseEvent ConnectionStateChanged(
                        False,
                        "Binance WebSocket desconectado. Reconectando...")
                End If

            Catch ex As OperationCanceledException When token.IsCancellationRequested

                Exit While

            Catch ex As Exception

                If Not token.IsCancellationRequested Then
                    RaiseEvent ConnectionStateChanged(
                        False,
                        "Binance WebSocket: " & ex.Message)
                    Select Case _socket?.HttpStatusCode
                        Case HttpStatusCode.Forbidden, HttpStatusCode.Unauthorized, CType(451, HttpStatusCode)
                            RaiseEvent ConnectionAttentionRequired(
                                $"Binance Spot WebSocket (HTTP {CInt(_socket.HttpStatusCode)}). Verifique se a rede ou a região permite acesso ao mercado público da Binance.")
                        Case HttpStatusCode.RequestUriTooLong
                            RaiseEvent ConnectionAttentionRequired(
                                "Binance Spot WebSocket (HTTP 414). Reduza o máximo de ativos no universo do TradeBot.")
                    End Select
                End If

            Finally

                CloseCurrentSocket()
                _receiveTask = Nothing

            End Try

            If Not token.IsCancellationRequested Then
                If connectedAt <> DateTime.MinValue AndAlso DateTime.UtcNow - connectedAt >= TimeSpan.FromMinutes(1) Then
                    delay = 2000
                End If

                Try
                    Await Task.Delay(delay, token)
                Catch ex As OperationCanceledException
                    Exit While
                End Try

                delay = Math.Min(delay * 2, 15000)

            End If

        End While

    End Function

    Public Sub ReconnectIfSilent(nowUtc As DateTime, timeout As TimeSpan)
        Dim socket = _socket
        If socket Is Nothing OrElse socket.State <> WebSocketState.Open Then Return
        Dim lastActivity = If(_lastMessageUtc = DateTime.MinValue, _lastConnectedUtc, _lastMessageUtc)
        If nowUtc - lastActivity < timeout Then Return
        socket.Abort()
    End Sub

    Private Async Function ConnectOnceAsync(token As CancellationToken) As Task

        CloseCurrentSocket()

        _executionSnapshots.Clear()
        _socket = New ClientWebSocket()
        _socket.Options.CollectHttpResponseDetails = True
        _socket.Options.KeepAliveInterval = TimeSpan.FromSeconds(20)

        Dim streamUrl As String = BuildCombinedStreamUrl(_symbols)

        Debug.WriteLine("[BINANCE WS] Conectando: " & streamUrl)

        Await _socket.ConnectAsync(New Uri(streamUrl), token)

    End Function

    Private Function BuildCombinedStreamUrl(symbols As IEnumerable(Of String)) As String

        Dim streams As New List(Of String)

        For Each symbol As String In symbols

            Dim pair As String

            If symbol.EndsWith("USDT", StringComparison.OrdinalIgnoreCase) Then
                pair = symbol.ToLowerInvariant()
            Else
                pair = (symbol & "USDT").ToLowerInvariant()
            End If

            streams.Add(pair & "@ticker")
            ' Topo de livro real: os cinco melhores níveis de bid/ask, usado
            ' pelo paper trading e pelo gravador de replay.
            streams.Add(pair & "@depth5@100ms")

            For Each interval In If(_klineIntervals, New List(Of String)())
                streams.Add(pair & "@kline_" & interval)
            Next

        Next

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

                    result = Await _socket.ReceiveAsync(
                        New ArraySegment(Of Byte)(buffer),
                        token)

                    If result.MessageType = WebSocketMessageType.Close Then
                        Dim closeReason = If(String.IsNullOrWhiteSpace(result.CloseStatusDescription),
                                             String.Empty, " " & result.CloseStatusDescription)
                        Throw New WebSocketException($"Binance fechou a conexão ({result.CloseStatus}).{closeReason}")
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

            Using document As JsonDocument = JsonDocument.Parse(json)

                _lastMessageUtc = DateTime.UtcNow

                Dim root As JsonElement = document.RootElement
                Dim data As JsonElement

                If Not root.TryGetProperty("data", data) Then
                    Return
                End If

                Dim stream As JsonElement
                If root.TryGetProperty("stream", stream) AndAlso stream.ValueKind = JsonValueKind.String Then
                    Dim streamName = stream.GetString()
                    Const partialBookSuffix As String = "@depth5@100ms"
                    If streamName.EndsWith(partialBookSuffix, StringComparison.OrdinalIgnoreCase) Then
                        ProcessDepthMessage(streamName.Substring(0, streamName.Length - partialBookSuffix.Length), data)
                        Return
                    End If
                End If

                Dim eventType As JsonElement
                If data.TryGetProperty("e", eventType) AndAlso
                   String.Equals(eventType.GetString(), "depthUpdate", StringComparison.OrdinalIgnoreCase) Then
                    ' Deltas não representam um snapshot completo dos primeiros níveis.
                    Return
                End If

                Dim kline As JsonElement
                If data.TryGetProperty("k", kline) Then
                    ProcessKlineMessage(kline)
                    Return
                End If

                Dim symbolElement As JsonElement
                Dim priceElement As JsonElement
                Dim volumeElement As JsonElement

                If Not data.TryGetProperty("s", symbolElement) OrElse
                   Not data.TryGetProperty("c", priceElement) Then
                    Return
                End If

                Dim pairSymbol As String = symbolElement.GetString()
                Dim priceText As String = priceElement.GetString()

                If String.IsNullOrWhiteSpace(pairSymbol) OrElse
                   String.IsNullOrWhiteSpace(priceText) Then
                    Return
                End If

                Dim price As Decimal

                If Not Decimal.TryParse(
                    priceText,
                    NumberStyles.Float,
                    CultureInfo.InvariantCulture,
                    price) Then
                    Return
                End If

                Dim quoteVolume24h = 0D
                If data.TryGetProperty("q", volumeElement) Then
                    Decimal.TryParse(
                        volumeElement.GetString(),
                        NumberStyles.Float,
                        CultureInfo.InvariantCulture,
                        quoteVolume24h)
                End If

                Dim assetSymbol As String = pairSymbol

                If assetSymbol.EndsWith("USDT", StringComparison.OrdinalIgnoreCase) Then
                    assetSymbol = assetSymbol.Substring(0, assetSymbol.Length - 4)
                End If

                _prices(assetSymbol) = price

                Debug.WriteLine(
                    $"[BINANCE WS] {assetSymbol} = {price.ToString(CultureInfo.InvariantCulture)}")

                If assetSymbol.Equals("BTC", StringComparison.OrdinalIgnoreCase) Then
                    UpdateBitcoinLabel(price)
                End If

                RaiseEvent PriceUpdated(assetSymbol, price)
                RaiseEvent VolumeUpdated(assetSymbol, price, Math.Max(0D, quoteVolume24h))

            End Using

        Catch ex As JsonException

            Debug.WriteLine(
                "Binance WebSocket JSON inválido: " & ex.Message)

        Catch ex As Exception

            Debug.WriteLine(
                "Erro processando Binance WebSocket: " & ex.Message)

        End Try

    End Sub

    Private Sub ProcessDepthMessage(pairSymbol As String, data As JsonElement)
        Try
            If String.IsNullOrWhiteSpace(pairSymbol) Then Return

            Dim assetSymbol = pairSymbol.Trim().ToUpperInvariant()
            If assetSymbol.EndsWith("USDT", StringComparison.OrdinalIgnoreCase) Then
                assetSymbol = assetSymbol.Substring(0, assetSymbol.Length - 4)
            End If

            Dim bidPrice = 0D
            Dim askPrice = 0D
            Dim bidDepthQuote = 0D
            Dim askDepthQuote = 0D
            Dim bids As JsonElement
            data.TryGetProperty("bids", bids)
            If bids.ValueKind = JsonValueKind.Array Then
                For Each level In bids.EnumerateArray().Take(5)
                    If level.ValueKind <> JsonValueKind.Array OrElse level.GetArrayLength() < 2 Then Continue For
                    Dim price As Decimal
                    Dim quantity As Decimal
                    Decimal.TryParse(level(0).GetString(), NumberStyles.Float, CultureInfo.InvariantCulture, price)
                    Decimal.TryParse(level(1).GetString(), NumberStyles.Float, CultureInfo.InvariantCulture, quantity)
                    If bidPrice <= 0D Then bidPrice = price
                    bidDepthQuote += Math.Max(0D, price * quantity)
                Next
            End If

            Dim asks As JsonElement
            data.TryGetProperty("asks", asks)
            If asks.ValueKind = JsonValueKind.Array Then
                For Each level In asks.EnumerateArray().Take(5)
                    If level.ValueKind <> JsonValueKind.Array OrElse level.GetArrayLength() < 2 Then Continue For
                    Dim price As Decimal
                    Dim quantity As Decimal
                    Decimal.TryParse(level(0).GetString(), NumberStyles.Float, CultureInfo.InvariantCulture, price)
                    Decimal.TryParse(level(1).GetString(), NumberStyles.Float, CultureInfo.InvariantCulture, quantity)
                    If askPrice <= 0D Then askPrice = price
                    askDepthQuote += Math.Max(0D, price * quantity)
                Next
            End If

            If bidPrice <= 0D OrElse askPrice < bidPrice OrElse bidDepthQuote <= 0D OrElse askDepthQuote <= 0D Then
                Dim discarded As SpotBotExecutionSnapshot = Nothing
                _executionSnapshots.TryRemove(assetSymbol, discarded)
                Return
            End If
            Dim snapshot = New SpotBotExecutionSnapshot With {
                .CapturedAtUtc = _lastMessageUtc,
                .BidPrice = bidPrice,
                .AskPrice = askPrice,
                .BidDepthQuote = bidDepthQuote,
                .AskDepthQuote = askDepthQuote}
            _executionSnapshots(assetSymbol) = snapshot
            RaiseEvent ExecutionSnapshotUpdated(assetSymbol, snapshot)
        Catch ex As Exception
            Debug.WriteLine("Erro processando livro Binance: " & ex.Message)
        End Try
    End Sub

    Private Sub ProcessKlineMessage(kline As JsonElement)
        Try
            Dim symbol As String = Nothing
            Dim interval As String = Nothing
            Dim startMs As Long
            Dim openPrice As Decimal
            Dim highPrice As Decimal
            Dim lowPrice As Decimal
            Dim closePrice As Decimal
            Dim volume As Decimal
            Dim closeFlag As Boolean

            Dim symbolElement As JsonElement
            If Not kline.TryGetProperty("s", symbolElement) Then Return
            symbol = symbolElement.GetString()
            interval = kline.GetProperty("i").GetString()
            If Not Long.TryParse(kline.GetProperty("t").ToString(), startMs) Then Return
            If Not Decimal.TryParse(kline.GetProperty("o").GetString(), NumberStyles.Float, CultureInfo.InvariantCulture, openPrice) Then Return
            If Not Decimal.TryParse(kline.GetProperty("h").GetString(), NumberStyles.Float, CultureInfo.InvariantCulture, highPrice) Then Return
            If Not Decimal.TryParse(kline.GetProperty("l").GetString(), NumberStyles.Float, CultureInfo.InvariantCulture, lowPrice) Then Return
            If Not Decimal.TryParse(kline.GetProperty("c").GetString(), NumberStyles.Float, CultureInfo.InvariantCulture, closePrice) Then Return
            Decimal.TryParse(kline.GetProperty("v").GetString(), NumberStyles.Float, CultureInfo.InvariantCulture, volume)
            Boolean.TryParse(kline.GetProperty("x").ToString(), closeFlag)

            If String.IsNullOrWhiteSpace(symbol) OrElse String.IsNullOrWhiteSpace(interval) Then Return

            Dim assetSymbol = symbol.Trim().ToUpperInvariant()
            If assetSymbol.EndsWith("USDT", StringComparison.OrdinalIgnoreCase) Then
                assetSymbol = assetSymbol.Substring(0, assetSymbol.Length - 4)
            End If

            RaiseEvent CandleUpdated(
                assetSymbol,
                NormalizeKlineInterval(interval),
                DateTimeOffset.FromUnixTimeMilliseconds(startMs).UtcDateTime,
                openPrice,
                highPrice,
                lowPrice,
                closePrice,
                Math.Max(0D, volume),
                closeFlag)
        Catch ex As Exception
            Debug.WriteLine("Erro processando candle Binance: " & ex.Message)
        End Try
    End Sub

    Private Shared Function NormalizeKlineInterval(value As String) As String
        If String.Equals(value?.Trim(), "1m", StringComparison.OrdinalIgnoreCase) Then Return "1m"
        If String.Equals(value?.Trim(), "5m", StringComparison.OrdinalIgnoreCase) Then Return "5m"
        If String.Equals(value?.Trim(), "1d", StringComparison.OrdinalIgnoreCase) Then Return "1d"
        If String.Equals(value?.Trim(), "4h", StringComparison.OrdinalIgnoreCase) Then Return "4h"
        Return String.Empty
    End Function

    Private Sub UpdateBitcoinLabel(price As Decimal)

        Try

            Dim priceText As String =
                price.ToString("C2", CultureInfo.GetCultureInfo("en-US"))

            If FormMain.IsHandleCreated Then

                If FormMain.InvokeRequired Then
                    FormMain.BeginInvoke(
                        New Action(
                            Sub()
                                FormMain.lbBTC.Text = priceText
                            End Sub))
                Else
                    FormMain.lbBTC.Text = priceText
                End If

            End If

        Catch ex As Exception

            Debug.WriteLine(
                "Erro atualizando lbBTC pelo WebSocket: " & ex.Message)

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
            "Binance WebSocket parado.")

    End Function

End Class
