Imports System.Collections.Concurrent
Imports System.Globalization
Imports System.Net.WebSockets
Imports System.Text
Imports System.Text.Json
Imports System.Threading

Public Class BinanceWebSocket

    Private Const WebSocketBaseUrl As String = "wss://stream.binance.com:9443/stream?streams="

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

        Await StopAsync()

        _symbols = normalized
        _cts = New CancellationTokenSource()

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

            Try

                Await ConnectOnceAsync(token)

                RaiseEvent ConnectionStateChanged(
                    True,
                    $"Binance WebSocket conectado: {_symbols.Count} símbolos.")

                delay = 2000

                _receiveTask = ReceiveLoopAsync(token)
                Await _receiveTask

                If Not token.IsCancellationRequested Then
                    RaiseEvent ConnectionStateChanged(
                        False,
                        "Binance WebSocket desconectado. Reconectando...")
                End If

            Catch ex As OperationCanceledException

                Exit While

            Catch ex As Exception

                If Not token.IsCancellationRequested Then
                    RaiseEvent ConnectionStateChanged(
                        False,
                        "Binance WebSocket: " & ex.Message)
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
                        Throw New WebSocketException("Binance fechou a conexão.")
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

                Dim root As JsonElement = document.RootElement
                Dim data As JsonElement

                If Not root.TryGetProperty("data", data) Then
                    Return
                End If

                Dim symbolElement As JsonElement
                Dim priceElement As JsonElement

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

            End Using

        Catch ex As JsonException

            Debug.WriteLine(
                "Binance WebSocket JSON inválido: " & ex.Message)

        Catch ex As Exception

            Debug.WriteLine(
                "Erro processando Binance WebSocket: " & ex.Message)

        End Try

    End Sub

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
