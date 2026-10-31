Imports System.Globalization
Imports System.IO
Imports System.Net.Http
Imports System.Net.Http.Headers
Imports System.Security.Cryptography
Imports System.Security.Principal
Imports System.Text
Imports System.Text.Json
Imports Newtonsoft.Json.Linq

Public Class BinanceFuturesAccountSummary
    Public Property AvailableBalance As Decimal
    Public Property InitialMargin As Decimal
    Public Property WalletBalance As Decimal
    Public Property Equity As Decimal
    Public Property UnrealizedProfit As Decimal
End Class

Public Class BinanceSpotOrder
    Public Property Exchange As String = String.Empty
    Public Property Symbol As String
    Public Property OrderId As Long
    Public Property Side As String
    Public Property OrderType As String
    Public Property Status As String
    Public Property Quantity As Decimal
    Public Property ExecutedQuantity As Decimal
    Public Property Price As Decimal
    Public Property StopPrice As Decimal
    Public Property OrderTime As DateTime
End Class

Public Class BinanceFuturesOrder
    Public Property Exchange As String = String.Empty
    Public Property Symbol As String
    Public Property OrderId As Long
    Public Property PositionSide As String
    Public Property Side As String
    Public Property OrderType As String
    Public Property Status As String
    Public Property Quantity As Decimal
    Public Property ExecutedQuantity As Decimal
    Public Property Price As Decimal
    Public Property StopPrice As Decimal
    Public Property OrderTime As DateTime
End Class

Public Class BinanceSpotSymbolFilters
    Public Property MinQuantity As Decimal
    Public Property StepSize As Decimal
    Public Property MinNotional As Decimal
    Public Property TickSize As Decimal
End Class

Public Class BinanceSpotUniverseCandidate
    Public Property Symbol As String = String.Empty
    Public Property BaseAsset As String = String.Empty
    Public Property QuoteVolume As Decimal
End Class

Public Class Binance
    Private BinanceTimeOffset As Long = 0

    Public Async Function SyncBinanceTime() As Task

        Using cli As New HttpClient()

            Dim json As String = Await cli.GetStringAsync("https://api.binance.com/api/v3/time")
            Dim obj As JObject = JObject.Parse(json)
            Dim serverTime As Long = CLng(obj("serverTime"))
            Dim localTime As Long =
            DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
            BinanceTimeOffset = serverTime - localTime

        End Using

    End Function

    Private Async Function BINANCE_GetSpotAccountAsync() As Task(Of JObject)

        Dim json = Await QuerySignedAsync("/api/v3/account", "recvWindow=60000")

        If String.IsNullOrWhiteSpace(json) Then
            Return Nothing
        End If

        Dim account As JObject = JObject.Parse(json)

        If account("code") IsNot Nothing Then

            Throw New Exception(
            $"Binance ({account("code")}): {account("msg")}"
        )

        End If

        Return account

    End Function

    Private Async Function BINANCE_GetFuturesAccountAsync() As Task(Of JObject)

        Dim json =
        Await QuerySignedAsync(
            "/fapi/v2/account",
            "recvWindow=60000",
            isFutures:=True)

        If String.IsNullOrWhiteSpace(json) Then
            Return Nothing
        End If

        Dim account As JObject = JObject.Parse(json)

        If account("code") IsNot Nothing Then

            Throw New Exception(
            $"Binance ({account("code")}): {account("msg")}"
        )

        End If

        Return account

    End Function

    Private Function GetFuturesAssets(account As JObject) As Dictionary(Of String, (Saldo As Decimal, Ordem As Decimal, Lucro As Decimal))

        Dim ativos As New Dictionary(Of String, (Saldo As Decimal, Ordem As Decimal, Lucro As Decimal))(StringComparer.OrdinalIgnoreCase)

        For Each asset In account("assets")

            Dim symbol = asset("asset").ToString()

            Dim saldo =
            Decimal.Parse(
                asset("walletBalance").ToString(),
                CultureInfo.InvariantCulture)

            Dim ordem =
            Decimal.Parse(
                asset("openOrderInitialMargin").ToString(),
                CultureInfo.InvariantCulture)

            If saldo > 0D OrElse ordem > 0D Then

                ativos(symbol) = (
                Saldo:=saldo,
                Ordem:=ordem,
                Lucro:=0D)

            End If

        Next

        For Each pos In account("positions")

            Dim contrato =
            pos("symbol").ToString()

            Dim lucro =
            Decimal.Parse(
                pos("unrealizedProfit").ToString(),
                CultureInfo.InvariantCulture)

            If lucro <> 0D Then

                If ativos.ContainsKey(contrato) Then

                    Dim atual = ativos(contrato)

                    ativos(contrato) = (
                    atual.Saldo,
                    atual.Ordem,
                    atual.Lucro + lucro)

                Else

                    ativos(contrato) = (
                    Saldo:=0D,
                    Ordem:=0D,
                    Lucro:=lucro)

                End If

            End If

        Next

        Return ativos

    End Function
    'Private Function QuerySigned(ByVal path As String, ByVal query As String, Optional isFutures As Boolean = False) As String
    '    Dim apiKey As String = ExchangeCredentials.BinanceAPIKey
    '    Dim secret As String = ExchangeCredentials.BinanceSecretKey

    '    Try

    '        Dim baseUrl = If(isFutures, "https://fapi.binance.com", "https://api.binance.com")
    '        ' Dim qs = query & "&timestamp=" & (DateTimeOffset.UtcNow.ToUnixTimeMilliseconds())
    '        Dim timestamp As Long = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + BinanceTimeOffset
    '        Dim qs = query & "&timestamp=" & timestamp

    '        Using hmac As New HMACSHA256(Encoding.UTF8.GetBytes(secret))
    '            Dim sigBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(qs))
    '            Dim signature = BitConverter.ToString(sigBytes).Replace("-", "").ToLower()
    '            qs &= "&signature=" & signature
    '        End Using

    '        Using cli As New HttpClient()
    '            cli.BaseAddress = New Uri(baseUrl)
    '            cli.DefaultRequestHeaders.Add("X-MBX-APIKEY", apiKey)
    '            Dim resp = cli.GetAsync(path & "?" & qs).Result
    '            Return resp.Content.ReadAsStringAsync().Result
    '        End Using

    '    Catch ex As Exception
    '        FormMain.lbDebug.Clear()
    '        FormMain.lbDebug.AppendText("Erro na assinatura da API Binance: " & ex.Message)
    '        Debug.WriteLine("Erro na assinatura da API Binance: " & ex.Message)
    '        Return False
    '    End Try

    'End Function

    Private Async Function QuerySignedAsync(
        ByVal path As String,
        ByVal query As String,
        Optional isFutures As Boolean = False,
        Optional retry As Integer = 0,
        Optional httpMethod As String = "GET") As Task(Of String)

        Dim apiKey As String = ExchangeCredentials.BinanceAPIKey
        Dim secret As String = ExchangeCredentials.BinanceSecretKey

        Try

            Dim baseUrl As String = If(isFutures, "https://fapi.binance.com", "https://api.binance.com")
            Dim timestamp As Long = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + BinanceTimeOffset
            Dim qs As String = $"{query}&timestamp={timestamp}"

            Using hmac As New HMACSHA256(Encoding.UTF8.GetBytes(secret))
                Dim sigBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(qs))
                Dim signature = BitConverter.ToString(sigBytes).Replace("-", "").ToLowerInvariant()

                qs &= "&signature=" & signature

            End Using

            Using cli As New HttpClient()

                cli.BaseAddress = New Uri(baseUrl)
                cli.DefaultRequestHeaders.Add("X-MBX-APIKEY", apiKey)

                Using request As New HttpRequestMessage(
                    New HttpMethod(httpMethod),
                    path & "?" & qs)
                    Dim resp = Await cli.SendAsync(request)
                Dim json = Await resp.Content.ReadAsStringAsync()
                Dim obj As JObject = Nothing

                Try
                    obj = JObject.Parse(json)
                Catch
                End Try

                If obj IsNot Nothing AndAlso
               obj("code") IsNot Nothing AndAlso
               CLng(obj("code")) = -1021 AndAlso
               retry < 1 Then

                    Await SyncBinanceTime()
                    Return Await QuerySignedAsync(path, query, isFutures, retry + 1, httpMethod)
                End If

                Return json

                End Using

            End Using

        Catch ex As Exception

            FormMain.lbDebug.Clear()
            FormMain.lbDebug.AppendText("Erro na assinatura da API Binance: " & ex.Message)
            Debug.WriteLine(ex.ToString())
            Return String.Empty

        End Try

    End Function

    Public Async Function BINANCE_GetSpotOpenOrdersAsync(
        Optional symbol As String = "") As Task(Of List(Of BinanceSpotOrder))

        Dim result As New List(Of BinanceSpotOrder)()
        Dim query = "recvWindow=60000"

        If Not String.IsNullOrWhiteSpace(symbol) Then
            query &= "&symbol=" & Uri.EscapeDataString(symbol.Trim().ToUpperInvariant())
        End If

        Dim json = Await QuerySignedAsync("/api/v3/openOrders", query)
        If String.IsNullOrWhiteSpace(json) Then
            Return result
        End If

        Dim token As JToken = JToken.Parse(json)
        If token.Type = JTokenType.Object AndAlso token("code") IsNot Nothing Then
            Throw New Exception($"Binance ({token("code")}): {token("msg")}")
        End If

        Dim rawOrders = TryCast(token, JArray)
        If rawOrders Is Nothing Then
            Return result
        End If

        For Each raw In rawOrders.OfType(Of JObject)()
            Dim orderTimeMs As Long = 0
            Long.TryParse(GetTokenValue(raw, "time", "transactTime")?.ToString(), orderTimeMs)

            Dim orderTime = DateTime.MinValue
            If orderTimeMs > 0 Then
                orderTime = DateTimeOffset.FromUnixTimeMilliseconds(orderTimeMs).LocalDateTime
            End If

            Dim orderId As Long = 0
            Long.TryParse(GetTokenValue(raw, "orderId")?.ToString(), orderId)

            result.Add(New BinanceSpotOrder With {
                .Symbol = If(GetTokenValue(raw, "symbol")?.ToString(), String.Empty),
                .OrderId = orderId,
                .Side = If(GetTokenValue(raw, "side")?.ToString(), String.Empty),
                .OrderType = If(GetTokenValue(raw, "type")?.ToString(), String.Empty),
                .Status = If(GetTokenValue(raw, "status")?.ToString(), String.Empty),
                .Quantity = ParseDecimalInvariant(GetTokenValue(raw, "origQty")),
                .ExecutedQuantity = ParseDecimalInvariant(GetTokenValue(raw, "executedQty")),
                .Price = ParseDecimalInvariant(GetTokenValue(raw, "price")),
                .StopPrice = ParseDecimalInvariant(GetTokenValue(raw, "stopPrice")),
                .OrderTime = orderTime
            })
        Next

        Return result
    End Function

    Public Async Function BINANCE_CancelSpotOrderAsync(
        symbol As String,
        orderId As Long) As Task(Of Boolean)

        If String.IsNullOrWhiteSpace(symbol) Then
            Throw New ArgumentException("O símbolo da ordem é obrigatório.", NameOf(symbol))
        End If

        If orderId <= 0 Then
            Throw New ArgumentException("O identificador da ordem é inválido.", NameOf(orderId))
        End If

        Dim query = String.Join("&", {
            "symbol=" & Uri.EscapeDataString(symbol.Trim().ToUpperInvariant()),
            "orderId=" & orderId.ToString(CultureInfo.InvariantCulture),
            "recvWindow=60000"
        })

        Dim json = Await QuerySignedAsync(
            "/api/v3/order",
            query,
            httpMethod:="DELETE")

        If String.IsNullOrWhiteSpace(json) Then
            Return False
        End If

        Dim response As JObject = JObject.Parse(json)
        If response("code") IsNot Nothing Then
            Throw New Exception($"Binance ({response("code")}): {response("msg")}")
        End If

        Return True
    End Function

    Public Async Function BINANCE_CreateSpotOrderAsync(
        symbol As String,
        side As String,
        orderType As String,
        quantity As Decimal,
        Optional limitPrice As Decimal = 0D) As Task(Of String)

        Dim normalizedSymbol = symbol?.Trim().ToUpperInvariant()
        Dim normalizedSide = side?.Trim().ToUpperInvariant()
        Dim normalizedType = orderType?.Trim().ToUpperInvariant()

        If String.IsNullOrWhiteSpace(normalizedSymbol) Then
            Throw New ArgumentException("O símbolo da ordem é obrigatório.", NameOf(symbol))
        End If

        If normalizedSide <> "BUY" AndAlso normalizedSide <> "SELL" Then
            Throw New ArgumentException("O lado deve ser BUY ou SELL.", NameOf(side))
        End If

        If normalizedType <> "MARKET" AndAlso normalizedType <> "LIMIT" Then
            Throw New ArgumentException("O tipo deve ser MARKET ou LIMIT.", NameOf(orderType))
        End If

        If quantity <= 0D Then
            Throw New ArgumentException("A quantidade deve ser maior que zero.", NameOf(quantity))
        End If

        If normalizedType = "LIMIT" AndAlso limitPrice <= 0D Then
            Throw New ArgumentException("O preço limite deve ser maior que zero.", NameOf(limitPrice))
        End If

        Dim filters = Await BINANCE_GetSpotSymbolFiltersAsync(normalizedSymbol)
        If filters IsNot Nothing Then
            If filters.MinQuantity > 0D AndAlso quantity < filters.MinQuantity Then
                Throw New ArgumentException(
                    $"A quantidade mínima para {normalizedSymbol} é {filters.MinQuantity.ToString("0.########", CultureInfo.InvariantCulture)}.")
            End If

            If filters.StepSize > 0D Then
                Dim quantitySteps = quantity / filters.StepSize
                Dim alignedQuantity = Decimal.Truncate(quantitySteps) * filters.StepSize
                If Math.Abs(quantity - alignedQuantity) > filters.StepSize / 1000000D Then
                    Throw New ArgumentException(
                        $"A quantidade deve ser múltipla de {filters.StepSize.ToString("0.########", CultureInfo.InvariantCulture)} para {normalizedSymbol}.")
                End If
            End If

            If normalizedType = "LIMIT" AndAlso
               filters.MinNotional > 0D AndAlso
               quantity * limitPrice < filters.MinNotional Then
                Dim minimumQuantity = Decimal.Ceiling(filters.MinNotional / limitPrice)
                Throw New ArgumentException(
                    $"O valor mínimo da ordem para {normalizedSymbol} é ${filters.MinNotional.ToString("0.##", CultureInfo.InvariantCulture)}. " &
                    $"Com o preço informado, use pelo menos {minimumQuantity.ToString("0.########", CultureInfo.InvariantCulture)} unidades.")
            End If
        End If

        Dim parameters As New List(Of String) From {
            "symbol=" & Uri.EscapeDataString(normalizedSymbol),
            "side=" & normalizedSide,
            "type=" & normalizedType,
            "quantity=" & quantity.ToString("0.################", CultureInfo.InvariantCulture),
            "recvWindow=60000"
        }

        If normalizedType = "LIMIT" Then
            parameters.Add("timeInForce=GTC")
            parameters.Add("price=" & limitPrice.ToString("0.################", CultureInfo.InvariantCulture))
        End If

        Dim json = Await QuerySignedAsync(
            "/api/v3/order",
            String.Join("&", parameters),
            httpMethod:="POST")

        If String.IsNullOrWhiteSpace(json) Then
            Throw New Exception("A Binance não retornou o resultado da ordem.")
        End If

        Dim response As JObject = JObject.Parse(json)
        If response("code") IsNot Nothing Then
            Throw New Exception($"Binance ({response("code")}): {response("msg")}")
        End If

        Return json
    End Function

    Public Async Function BINANCE_GetSpotSymbolFiltersAsync(symbol As String) As Task(Of BinanceSpotSymbolFilters)
        Dim normalizedSymbol = symbol?.Trim().ToUpperInvariant()
        If String.IsNullOrWhiteSpace(normalizedSymbol) Then
            Return Nothing
        End If

        Try
            Using client As New HttpClient()
                Dim url = "https://api.binance.com/api/v3/exchangeInfo?symbol=" & Uri.EscapeDataString(normalizedSymbol)
                Dim response = Await client.GetAsync(url)
                If Not response.IsSuccessStatusCode Then
                    Return Nothing
                End If

                Dim content = Await response.Content.ReadAsStringAsync()
                Dim exchangeInfo = JObject.Parse(content)
                Dim symbols = TryCast(exchangeInfo("symbols"), JArray)
                If symbols Is Nothing OrElse symbols.Count = 0 Then
                    Return Nothing
                End If

                Dim filters = New BinanceSpotSymbolFilters()
                For Each symbolFilter In symbols(0)("filters")
                    Dim filterType = symbolFilter("filterType")?.ToString()
                    Select Case filterType
                        Case "LOT_SIZE"
                            filters.MinQuantity = ParseDecimalInvariant(symbolFilter("minQty"))
                            filters.StepSize = ParseDecimalInvariant(symbolFilter("stepSize"))
                        Case "PRICE_FILTER"
                            filters.TickSize = ParseDecimalInvariant(symbolFilter("tickSize"))
                        Case "MIN_NOTIONAL", "NOTIONAL"
                            Dim minNotional = ParseDecimalInvariant(symbolFilter("minNotional"))
                            If minNotional > filters.MinNotional Then
                                filters.MinNotional = minNotional
                            End If
                    End Select
                Next

                Return filters
            End Using
        Catch ex As Exception
            Debug.WriteLine("Erro lendo filtros da ordem Spot: " & ex.Message)
            Return Nothing
        End Try
    End Function

    Public Async Function BINANCE_GetSpotKlinesAsync(
        symbol As String,
        interval As String,
        limit As Integer) As Task(Of List(Of DcaCandle))

        Dim result As New List(Of DcaCandle)()
        Dim normalizedSymbol = symbol?.Trim().ToUpperInvariant()
        If String.IsNullOrWhiteSpace(normalizedSymbol) Then Return result
        If Not normalizedSymbol.EndsWith("USDT", StringComparison.OrdinalIgnoreCase) Then
            normalizedSymbol &= "USDT"
        End If

        Dim normalizedInterval = If(String.Equals(interval?.Trim(), "1d", StringComparison.OrdinalIgnoreCase), "1d", "4h")
        Dim requestedLimit = Math.Max(30, Math.Min(1000, limit))

        Using client As New HttpClient()
            client.DefaultRequestHeaders.Add("User-Agent", "PortfolioCripto/1.0")
            Dim url = $"https://api.binance.com/api/v3/klines?symbol={Uri.EscapeDataString(normalizedSymbol)}" &
                      $"&interval={normalizedInterval}&limit={requestedLimit}"
            Dim response = Await client.GetAsync(url)
            Dim content = Await response.Content.ReadAsStringAsync()
            If Not response.IsSuccessStatusCode Then
                Throw New InvalidOperationException(
                    $"Binance não carregou candles de {normalizedSymbol}: HTTP {CInt(response.StatusCode)}.")
            End If

            Dim raw = TryCast(JToken.Parse(content), JArray)
            If raw Is Nothing Then Return result

            For Each item In raw.OfType(Of JArray)()
                If item.Count < 7 Then Continue For

                Dim startMs As Long
                Dim closeMs As Long
                If Not Long.TryParse(item(0)?.ToString(), startMs) OrElse
                   Not Long.TryParse(item(6)?.ToString(), closeMs) Then Continue For

                result.Add(New DcaCandle With {
                    .StartUtc = DateTimeOffset.FromUnixTimeMilliseconds(startMs).UtcDateTime,
                    .CloseUtc = DateTimeOffset.FromUnixTimeMilliseconds(closeMs).UtcDateTime,
                    .Open = ParseDecimalInvariant(item(1)),
                    .High = ParseDecimalInvariant(item(2)),
                    .Low = ParseDecimalInvariant(item(3)),
                    .Close = ParseDecimalInvariant(item(4)),
                    .Volume = ParseDecimalInvariant(item(5)),
                    .IsClosed = DateTimeOffset.FromUnixTimeMilliseconds(closeMs) <= DateTimeOffset.UtcNow})
            Next
        End Using

        Return result.
            Where(Function(item) item.IsClosed AndAlso item.Low > 0D AndAlso item.Close > 0D).
            OrderBy(Function(item) item.StartUtc).
            ToList()
    End Function

    Public Async Function BINANCE_GetTopSpotSymbolsByVolumeAsync(
        maxAssets As Integer,
        minimumQuoteVolume As Decimal) As Task(Of List(Of String))

        Dim result As New List(Of String)()
        Dim requestedCount = Math.Max(1, Math.Min(500, maxAssets))
        Dim minimumVolume = Math.Max(0D, minimumQuoteVolume)

        Using client As New HttpClient()
            client.DefaultRequestHeaders.Add("User-Agent", "PortfolioCripto/1.0")

            Dim exchangeInfoText = Await client.GetStringAsync("https://api.binance.com/api/v3/exchangeInfo")
            Dim tickerText = Await client.GetStringAsync("https://api.binance.com/api/v3/ticker/24hr")
            Dim exchangeInfo = JObject.Parse(exchangeInfoText)
            Dim tickerArray = TryCast(JToken.Parse(tickerText), JArray)
            If tickerArray Is Nothing Then Return result

            Dim volumes As New Dictionary(Of String, Decimal)(StringComparer.OrdinalIgnoreCase)
            For Each ticker In tickerArray
                Dim symbol = ticker("symbol")?.ToString()?.Trim().ToUpperInvariant()
                If String.IsNullOrWhiteSpace(symbol) Then Continue For
                volumes(symbol) = ParseDecimalInvariant(ticker("quoteVolume"))
            Next

            Dim candidates As New List(Of BinanceSpotUniverseCandidate)()
            For Each symbolInfo In TryCast(exchangeInfo("symbols"), JArray)
                Dim symbol = symbolInfo("symbol")?.ToString()?.Trim().ToUpperInvariant()
                Dim baseAsset = symbolInfo("baseAsset")?.ToString()?.Trim().ToUpperInvariant()
                Dim quoteAsset = symbolInfo("quoteAsset")?.ToString()?.Trim().ToUpperInvariant()
                Dim status = symbolInfo("status")?.ToString()?.Trim().ToUpperInvariant()
                Dim spotAllowed = symbolInfo("isSpotTradingAllowed")

                If String.IsNullOrWhiteSpace(symbol) OrElse
                   String.IsNullOrWhiteSpace(baseAsset) OrElse
                   quoteAsset <> "USDT" OrElse
                   status <> "TRADING" OrElse
                   (spotAllowed IsNot Nothing AndAlso Not spotAllowed.Value(Of Boolean)()) OrElse
                   IsStablecoinAsset(baseAsset) OrElse
                   IsLeveragedToken(baseAsset) Then
                    Continue For
                End If

                Dim quoteVolume As Decimal = 0D
                If Not volumes.TryGetValue(symbol, quoteVolume) OrElse quoteVolume < minimumVolume Then
                    Continue For
                End If

                candidates.Add(New BinanceSpotUniverseCandidate With {
                    .Symbol = symbol,
                    .BaseAsset = baseAsset,
                    .QuoteVolume = quoteVolume
                })
            Next

            result.AddRange(
                candidates.OrderByDescending(Function(item) item.QuoteVolume).
                Take(requestedCount).
                Select(Function(item) item.BaseAsset))
        End Using

        Return result
    End Function

    Private Shared Function IsLeveragedToken(baseAsset As String) As Boolean
        Return baseAsset.EndsWith("UP", StringComparison.OrdinalIgnoreCase) OrElse
               baseAsset.EndsWith("DOWN", StringComparison.OrdinalIgnoreCase) OrElse
               baseAsset.EndsWith("BULL", StringComparison.OrdinalIgnoreCase) OrElse
               baseAsset.EndsWith("BEAR", StringComparison.OrdinalIgnoreCase)
    End Function

    Private Shared Function IsStablecoinAsset(baseAsset As String) As Boolean
        Select Case baseAsset.Trim().ToUpperInvariant()
            Case "USDT", "USDC", "BUSD", "DAI", "FDUSD", "TUSD", "USD", "USDP", "GUSD"
                Return True
            Case Else
                Return False
        End Select
    End Function

    Public Async Function BINANCE_GetFuturesOpenOrdersAsync(
        Optional symbol As String = "") As Task(Of List(Of BinanceFuturesOrder))

        Dim result As New List(Of BinanceFuturesOrder)()
        Dim query = "recvWindow=60000"
        If Not String.IsNullOrWhiteSpace(symbol) Then
            query &= "&symbol=" & Uri.EscapeDataString(symbol.Trim().ToUpperInvariant())
        End If

        Dim json = Await QuerySignedAsync(
            "/fapi/v1/openOrders",
            query,
            isFutures:=True)
        If String.IsNullOrWhiteSpace(json) Then
            Return result
        End If

        Dim token As JToken = JToken.Parse(json)
        If token.Type = JTokenType.Object AndAlso token("code") IsNot Nothing Then
            Throw New Exception($"Binance ({token("code")}): {token("msg")}")
        End If

        Dim rawOrders = TryCast(token, JArray)
        If rawOrders Is Nothing Then
            Return result
        End If

        For Each raw In rawOrders.OfType(Of JObject)()
            Dim orderTimeMs As Long = 0
            Long.TryParse(GetTokenValue(raw, "time", "updateTime", "transactTime")?.ToString(), orderTimeMs)

            Dim orderTime = DateTime.MinValue
            If orderTimeMs > 0 Then
                orderTime = DateTimeOffset.FromUnixTimeMilliseconds(orderTimeMs).LocalDateTime
            End If

            Dim orderId As Long = 0
            Long.TryParse(GetTokenValue(raw, "orderId")?.ToString(), orderId)

            result.Add(New BinanceFuturesOrder With {
                .Symbol = If(GetTokenValue(raw, "symbol")?.ToString(), String.Empty),
                .OrderId = orderId,
                .PositionSide = If(GetTokenValue(raw, "positionSide")?.ToString(), String.Empty),
                .Side = If(GetTokenValue(raw, "side")?.ToString(), String.Empty),
                .OrderType = If(GetTokenValue(raw, "type")?.ToString(), String.Empty),
                .Status = If(GetTokenValue(raw, "status")?.ToString(), String.Empty),
                .Quantity = ParseDecimalInvariant(GetTokenValue(raw, "origQty")),
                .ExecutedQuantity = ParseDecimalInvariant(GetTokenValue(raw, "executedQty")),
                .Price = ParseDecimalInvariant(GetTokenValue(raw, "price")),
                .StopPrice = ParseDecimalInvariant(GetTokenValue(raw, "stopPrice")),
                .OrderTime = orderTime
            })
        Next

        Return result
    End Function

    Public Async Function BINANCE_SetFuturesLeverageAsync(
        symbol As String,
        leverage As Integer) As Task(Of Boolean)

        Dim normalizedSymbol = symbol?.Trim().ToUpperInvariant()
        If String.IsNullOrWhiteSpace(normalizedSymbol) OrElse leverage < 1 OrElse leverage > 125 Then
            Throw New ArgumentException("Símbolo e alavancagem válidos são obrigatórios.")
        End If

        Dim query = String.Join("&", {
            "symbol=" & Uri.EscapeDataString(normalizedSymbol),
            "leverage=" & leverage.ToString(CultureInfo.InvariantCulture),
            "recvWindow=60000"
        })

        Dim json = Await QuerySignedAsync(
            "/fapi/v1/leverage",
            query,
            isFutures:=True,
            httpMethod:="POST")
        If String.IsNullOrWhiteSpace(json) Then
            Throw New Exception("A Binance não retornou resposta ao definir a alavancagem de Futuros.")
        End If

        Dim response As JObject = JObject.Parse(json)
        If response("code") IsNot Nothing Then
            Throw New Exception($"Binance ({response("code")}): {response("msg")}")
        End If

        Return True
    End Function

    Public Async Function BINANCE_CreateFuturesOrderAsync(
        symbol As String,
        positionSide As String,
        orderType As String,
        quantity As Decimal,
        limitPrice As Decimal) As Task(Of String)

        Dim normalizedSymbol = symbol?.Trim().ToUpperInvariant()
        Dim normalizedPositionSide = positionSide?.Trim().ToUpperInvariant()
        Dim normalizedType = orderType?.Trim().ToUpperInvariant()

        If String.IsNullOrWhiteSpace(normalizedSymbol) OrElse
           quantity <= 0D OrElse limitPrice <= 0D Then
            Throw New ArgumentException("Símbolo, quantidade e preço limite devem ser maiores que zero.")
        End If

        If normalizedPositionSide <> "LONG" AndAlso normalizedPositionSide <> "SHORT" Then
            Throw New ArgumentException("O lado deve ser LONG ou SHORT.", NameOf(positionSide))
        End If

        If normalizedType <> "LIMIT" Then
            Throw New ArgumentException("Somente ordens LIMIT estão disponíveis para Futuros.", NameOf(orderType))
        End If

        Dim side = If(normalizedPositionSide = "LONG", "BUY", "SELL")
        Dim query = String.Join("&", {
            "symbol=" & Uri.EscapeDataString(normalizedSymbol),
            "side=" & side,
            "positionSide=" & normalizedPositionSide,
            "type=LIMIT",
            "timeInForce=GTC",
            "quantity=" & quantity.ToString("0.################", CultureInfo.InvariantCulture),
            "price=" & limitPrice.ToString("0.################", CultureInfo.InvariantCulture),
            "recvWindow=60000"
        })

        Dim json = Await QuerySignedAsync(
            "/fapi/v1/order",
            query,
            isFutures:=True,
            httpMethod:="POST")
        If String.IsNullOrWhiteSpace(json) Then
            Throw New Exception("A Binance não retornou resposta ao enviar a ordem de Futuros.")
        End If

        Dim response As JObject = JObject.Parse(json)
        If response("code") IsNot Nothing Then
            Throw New Exception($"Binance ({response("code")}): {response("msg")}")
        End If

        Return json
    End Function

    Public Async Function BINANCE_CreateFuturesProtectionOrderAsync(
        symbol As String,
        positionSide As String,
        protectionType As String,
        quantity As Decimal,
        triggerPrice As Decimal) As Task(Of String)

        Dim normalizedSymbol = symbol?.Trim().ToUpperInvariant()
        Dim normalizedSide = positionSide?.Trim().ToUpperInvariant()
        Dim normalizedProtection = protectionType?.Trim().ToUpperInvariant()

        If String.IsNullOrWhiteSpace(normalizedSymbol) OrElse
           quantity <= 0D OrElse triggerPrice <= 0D Then
            Throw New ArgumentException("Símbolo, quantidade e preço de disparo devem ser maiores que zero.")
        End If

        If normalizedSide <> "LONG" AndAlso normalizedSide <> "SHORT" AndAlso normalizedSide <> "BOTH" Then
            Throw New ArgumentException("A posição precisa ser LONG, SHORT ou BOTH.", NameOf(positionSide))
        End If

        If normalizedProtection <> "TAKE_PROFIT" AndAlso normalizedProtection <> "STOP_LOSS" Then
            Throw New ArgumentException("O tipo de proteção deve ser TAKE_PROFIT ou STOP_LOSS.", NameOf(protectionType))
        End If

        Dim orderType = If(normalizedProtection = "TAKE_PROFIT", "TAKE_PROFIT_MARKET", "STOP_MARKET")
        Dim side = If(normalizedSide = "LONG", "SELL", "BUY")
        Dim queryParts As New List(Of String) From {
            "algoType=CONDITIONAL",
            "symbol=" & Uri.EscapeDataString(normalizedSymbol),
            "side=" & side,
            "type=" & orderType,
            "quantity=" & quantity.ToString("0.################", CultureInfo.InvariantCulture),
            "triggerPrice=" & triggerPrice.ToString("0.################", CultureInfo.InvariantCulture),
            "workingType=MARK_PRICE",
            "recvWindow=60000"
        }

        If normalizedSide = "LONG" OrElse normalizedSide = "SHORT" Then
            queryParts.Add("positionSide=" & normalizedSide)
        Else
            queryParts.Add("reduceOnly=true")
        End If

        Dim json = Await QuerySignedAsync(
            "/fapi/v1/algoOrder",
            String.Join("&", queryParts),
            isFutures:=True,
            httpMethod:="POST")

        If String.IsNullOrWhiteSpace(json) Then
            Throw New Exception("A Binance não retornou resposta ao criar a proteção de Futuros.")
        End If

        Dim response As JObject = JObject.Parse(json)
        If response("code") IsNot Nothing Then
            Throw New Exception($"Binance ({response("code")}): {response("msg")}")
        End If

        Return json
    End Function

    Public Async Function BINANCE_CancelFuturesOrderAsync(
        symbol As String,
        orderId As Long) As Task(Of Boolean)

        If String.IsNullOrWhiteSpace(symbol) OrElse orderId <= 0 Then
            Throw New ArgumentException("Símbolo e identificador da ordem são obrigatórios.")
        End If

        Dim query = String.Join("&", {
            "symbol=" & Uri.EscapeDataString(symbol.Trim().ToUpperInvariant()),
            "orderId=" & orderId.ToString(CultureInfo.InvariantCulture),
            "recvWindow=60000"
        })

        Dim json = Await QuerySignedAsync(
            "/fapi/v1/order",
            query,
            isFutures:=True,
            httpMethod:="DELETE")
        If String.IsNullOrWhiteSpace(json) Then
            Return False
        End If

        Dim response As JObject = JObject.Parse(json)
        If response("code") IsNot Nothing Then
            Throw New Exception($"Binance ({response("code")}): {response("msg")}")
        End If

        Return True
    End Function

    Public Async Function BINANCE_CloseFuturesPositionAsync(
        symbol As String,
        positionSide As String,
        quantity As Decimal,
        limitPrice As Decimal) As Task(Of String)

        If String.IsNullOrWhiteSpace(symbol) OrElse quantity <= 0D OrElse limitPrice <= 0D Then
            Throw New ArgumentException("Símbolo, quantidade e preço limite devem ser maiores que zero.")
        End If

        Dim normalizedSide = positionSide.Trim().ToUpperInvariant()
        If normalizedSide <> "LONG" AndAlso normalizedSide <> "SHORT" Then
            Throw New ArgumentException("A posição precisa ser LONG ou SHORT para usar o botão de saída.")
        End If

        Dim side = If(normalizedSide = "LONG", "SELL", "BUY")
        Dim query = String.Join("&", {
            "symbol=" & Uri.EscapeDataString(symbol.Trim().ToUpperInvariant()),
            "side=" & side,
            "positionSide=" & normalizedSide,
            "type=LIMIT",
            "timeInForce=GTC",
            "quantity=" & quantity.ToString("0.################", CultureInfo.InvariantCulture),
            "price=" & limitPrice.ToString("0.################", CultureInfo.InvariantCulture),
            "reduceOnly=true",
            "recvWindow=60000"
        })

        Dim apiKey = ExchangeCredentials.BinanceAPIKey
        Dim secret = ExchangeCredentials.BinanceSecretKey
        Dim timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + BinanceTimeOffset
        Dim signedQuery = query & "&timestamp=" & timestamp
        Using hmac As New HMACSHA256(Encoding.UTF8.GetBytes(secret))
            signedQuery &= "&signature=" & BitConverter.ToString(hmac.ComputeHash(Encoding.UTF8.GetBytes(signedQuery))).Replace("-", "").ToLowerInvariant()
        End Using

        Using cli As New HttpClient()
            cli.BaseAddress = New Uri("https://fapi.binance.com")
            cli.DefaultRequestHeaders.Add("X-MBX-APIKEY", apiKey)
            Using content As New StringContent(String.Empty, Encoding.UTF8, "application/x-www-form-urlencoded")
                Dim response = Await cli.PostAsync("/fapi/v1/order?" & signedQuery, content)
                Dim json = Await response.Content.ReadAsStringAsync()
                Dim obj = JObject.Parse(json)
                If obj("code") IsNot Nothing Then
                    Throw New Exception($"Binance ({obj("code")}): {obj("msg")}")
                End If
                Return json
            End Using
        End Using
    End Function

    Private Shared Function GetTokenValue(position As JObject, ParamArray names() As String) As JToken
        For Each name In names
            Dim value = position.GetValue(name, StringComparison.OrdinalIgnoreCase)
            If value IsNot Nothing Then
                Return value
            End If
        Next

        Return Nothing
    End Function

    Private Shared Function ParseDecimalInvariant(value As JToken) As Decimal
        If value Is Nothing OrElse value.Type = JTokenType.Null Then
            Return 0D
        End If

        Dim parsed As Decimal
        If Decimal.TryParse(value.ToString(), NumberStyles.Float, CultureInfo.InvariantCulture, parsed) Then
            Return parsed
        End If

        Return 0D
    End Function

    Private Shared Function ParseBinanceOrderTime(value As JToken) As DateTime
        Dim timestamp As Long = 0
        If value Is Nothing OrElse
           Not Long.TryParse(value.ToString(), NumberStyles.Integer, CultureInfo.InvariantCulture, timestamp) OrElse
           timestamp <= 0 Then
            Return DateTime.MinValue
        End If

        Return DateTimeOffset.FromUnixTimeMilliseconds(timestamp).LocalDateTime
    End Function

    ''' <summary>
    ''' Lê as posições abertas da conta USD-M da Binance.
    ''' A chamada é somente leitura e não altera ordens, margem ou posição.
    ''' </summary>
    Public Async Function BINANCE_GetFuturesPositionsAsync(
        Optional includeEmptyPositions As Boolean = False) As Task(Of List(Of BinanceFuturesPosition))
        Dim result As New List(Of BinanceFuturesPosition)()

        Try
            ' positionRisk fornece preço de marcação, liquidação e notional.
            Dim riskJson = Await QuerySignedAsync(
                "/fapi/v2/positionRisk",
                "recvWindow=60000",
                isFutures:=True)

            If String.IsNullOrWhiteSpace(riskJson) Then
                Return result
            End If

            Dim riskToken As JToken = JToken.Parse(riskJson)
            If riskToken.Type = JTokenType.Object AndAlso riskToken("code") IsNot Nothing Then
                Throw New Exception($"Binance ({riskToken("code")}): {riskToken("msg")}")
            End If

            Dim rawPositions = TryCast(riskToken, JArray)
            If rawPositions Is Nothing Then
                Return result
            End If

            For Each raw In rawPositions.OfType(Of JObject)()
                Dim amount = ParseDecimalInvariant(GetTokenValue(raw, "positionAmt"))
                Dim unrealizedProfit = ParseDecimalInvariant(
                    GetTokenValue(raw, "unRealizedProfit", "unrealizedProfit"))

                ' A API devolve uma linha por contrato mesmo quando não há posição.
                If Not includeEmptyPositions AndAlso amount = 0D AndAlso unrealizedProfit = 0D Then
                    Continue For
                End If

                Dim leverage As Integer
                Integer.TryParse(GetTokenValue(raw, "leverage")?.ToString(), leverage)

                result.Add(New BinanceFuturesPosition With {
                    .Symbol = If(GetTokenValue(raw, "symbol")?.ToString(), String.Empty),
                    .PositionAmount = amount,
                    .EntryPrice = ParseDecimalInvariant(GetTokenValue(raw, "entryPrice")),
                    .MarkPrice = ParseDecimalInvariant(GetTokenValue(raw, "markPrice")),
                    .UnrealizedProfit = unrealizedProfit,
                    .LiquidationPrice = ParseDecimalInvariant(GetTokenValue(raw, "liquidationPrice")),
                    .Leverage = leverage,
                    .Notional = ParseDecimalInvariant(GetTokenValue(raw, "notional")),
                    .MarginType = If(GetTokenValue(raw, "marginType")?.ToString(), String.Empty),
                    .IsolatedMargin = ParseDecimalInvariant(GetTokenValue(raw, "isolatedMargin")),
                    .PositionSide = If(GetTokenValue(raw, "positionSide")?.ToString(), String.Empty)
                })
            Next

            ' initialMargin vem na resposta da conta e completa a informação da margem.
            If result.Count > 0 Then
                Dim accountJson = Await QuerySignedAsync(
                    "/fapi/v2/account",
                    "recvWindow=60000",
                    isFutures:=True)

                If Not String.IsNullOrWhiteSpace(accountJson) Then
                    Dim account As JObject = JObject.Parse(accountJson)
                    If account("code") Is Nothing Then
                        For Each raw In TryCast(account("positions"), JArray)
                            Dim symbol = GetTokenValue(DirectCast(raw, JObject), "symbol")?.ToString()
                            Dim side = GetTokenValue(DirectCast(raw, JObject), "positionSide")?.ToString()

                            For Each position In result
                                If position.Symbol.Equals(symbol, StringComparison.OrdinalIgnoreCase) AndAlso
                                   position.PositionSide.Equals(side, StringComparison.OrdinalIgnoreCase) Then
                                    position.InitialMargin = ParseDecimalInvariant(
                                        GetTokenValue(DirectCast(raw, JObject), "positionInitialMargin", "initialMargin"))
                                    Exit For
                                End If
                            Next
                        Next
                    End If
                End If
            End If

            Return result
        Catch ex As Exception
            Debug.WriteLine("Erro ao trazer posições de Futuros da Binance: " & ex.Message)
            Return result
        End Try
    End Function
    ''' <summary>
    ''' Lê as ordens abertas de proteção da conta USD-M (take profit e stop loss).
    ''' </summary>
    Public Async Function BINANCE_GetFuturesProtectionOrdersAsync() As Task(Of List(Of BinanceFuturesProtectionOrder))
        Dim result As New List(Of BinanceFuturesProtectionOrder)()

        Try
            ' TP/SL criados pela interface da Binance são conditional/algo orders.
            Dim json = Await QuerySignedAsync(
                "/fapi/v1/openAlgoOrders",
                "recvWindow=60000",
                isFutures:=True)

            If String.IsNullOrWhiteSpace(json) Then
                Return result
            End If

            Dim token As JToken = JToken.Parse(json)
            If token.Type = JTokenType.Object AndAlso token("code") IsNot Nothing Then
                Throw New Exception($"Binance ({token("code")}): {token("msg")}")
            End If

            Dim rawOrders = TryCast(token, JArray)
            If rawOrders Is Nothing Then
                Return result
            End If

            For Each raw In rawOrders.OfType(Of JObject)()
                Dim orderType = GetTokenValue(raw, "orderType", "type")?.ToString()?.ToUpperInvariant()
                If String.IsNullOrWhiteSpace(orderType) OrElse
                   (Not orderType.Contains("TAKE_PROFIT") AndAlso
                    Not orderType.Contains("STOP")) Then
                    Continue For
                End If

                Dim algoId As Long = 0
                Long.TryParse(GetTokenValue(raw, "algoId", "orderId")?.ToString(), algoId)

                result.Add(New BinanceFuturesProtectionOrder With {
                    .AlgoId = algoId,
                    .Symbol = If(GetTokenValue(raw, "symbol")?.ToString(), String.Empty),
                    .PositionSide = If(GetTokenValue(raw, "positionSide")?.ToString(), String.Empty),
                    .Side = If(GetTokenValue(raw, "side")?.ToString(), String.Empty),
                    .OrderType = orderType,
                    .Quantity = ParseDecimalInvariant(GetTokenValue(raw, "quantity", "origQty")),
                    .StopPrice = ParseDecimalInvariant(GetTokenValue(raw, "triggerPrice", "stopPrice")),
                    .Price = ParseDecimalInvariant(GetTokenValue(raw, "price")),
                    .Status = If(GetTokenValue(raw, "algoStatus", "status")?.ToString(), String.Empty),
                    .OrderTime = ParseBinanceOrderTime(GetTokenValue(raw, "createTime", "updateTime"))
                })
            Next

            Return result
        Catch ex As Exception
            Debug.WriteLine("Erro ao trazer ordens TP/SL da Binance: " & ex.Message)
            Return result
        End Try
    End Function

    Public Async Function BINANCE_CancelFuturesProtectionOrderAsync(
        symbol As String,
        algoId As Long) As Task(Of Boolean)

        If String.IsNullOrWhiteSpace(symbol) OrElse algoId <= 0 Then
            Throw New ArgumentException("Símbolo e identificador da proteção são obrigatórios.")
        End If

        Dim query = String.Join("&", {
            "algoId=" & algoId.ToString(CultureInfo.InvariantCulture),
            "recvWindow=60000"
        })

        Dim json = Await QuerySignedAsync(
            "/fapi/v1/algoOrder",
            query,
            isFutures:=True,
            httpMethod:="DELETE")
        If String.IsNullOrWhiteSpace(json) Then
            Return False
        End If

        Dim response As JObject = JObject.Parse(json)
        If response("code") IsNot Nothing Then
            Throw New Exception($"Binance ({response("code")}): {response("msg")}")
        End If

        Return True
    End Function
    'Private Function BINANCE_GetSpotAssets() As Task(Of Dictionary(Of String, Decimal))
    '    Dim json = QuerySigned("/api/v3/account", "recvWindow=5000")
    '    Dim account = JObject.Parse(json)

    '    Dim ativos As New Dictionary(Of String, Decimal)(StringComparer.OrdinalIgnoreCase)

    '    If account("code") IsNot Nothing Then
    '        Throw New Exception($"Binance ({account("code")}): {account("msg")}")
    '        Exit Function
    '    End If

    '    Try


    '        For Each bal In account("balances")
    '            Dim asset As String = bal("asset").ToString()
    '            Dim free = Decimal.Parse(bal("free").ToString(), CultureInfo.InvariantCulture)
    '            Dim locked = Decimal.Parse(bal("locked").ToString(), CultureInfo.InvariantCulture)
    '            Dim qtd As Decimal = free + locked

    '            If qtd > 0D Then
    '                ativos(asset) = qtd
    '            End If
    '        Next

    '        Return Task.FromResult(ativos)

    '    Catch ex As Exception
    '        FormMain.lbDebug.Clear()
    '        FormMain.lbDebug.AppendText("Erro ao trazer dados da conta Spot: " & ex.Message)
    '        Debug.WriteLine("Erro ao trazer dados da conta Spot: " & ex.Message)
    '        Return Task.FromResult(New Dictionary(Of String, Decimal)) ' vazio
    '    End Try
    'End Function

    Private Async Function BINANCE_GetSpotAssets() As Task(Of Dictionary(Of String, Decimal))

        Dim json = Await QuerySignedAsync("/api/v3/account", "recvWindow=60000")

        If String.IsNullOrWhiteSpace(json) Then
            Return New Dictionary(Of String, Decimal)
        End If

        Dim account As JObject = JObject.Parse(json)

        If account("code") IsNot Nothing Then

            Throw New Exception(
            $"Binance ({account("code")}): {account("msg")}"
        )

        End If

        Dim ativos As New Dictionary(Of String, Decimal)(StringComparer.OrdinalIgnoreCase)

        For Each bal In account("balances")

            Dim asset As String =
            bal("asset").ToString()

            Dim free As Decimal = Decimal.Parse(bal("free").ToString(), CultureInfo.InvariantCulture)
            Dim locked As Decimal = Decimal.Parse(bal("locked").ToString(), CultureInfo.InvariantCulture)
            Dim qtd As Decimal = free + locked

            If qtd > 0D Then
                ativos(asset) = qtd
            End If

        Next

        Return ativos

    End Function

    Private Function GetSpotAssets(account As JObject) As Dictionary(Of String, Decimal)

        Dim ativos As New Dictionary(Of String, Decimal)(
        StringComparer.OrdinalIgnoreCase)

        For Each bal In account("balances")

            Dim asset As String = bal("asset").ToString()

            Dim free As Decimal =
            Decimal.Parse(
                bal("free").ToString(),
                CultureInfo.InvariantCulture)

            Dim locked As Decimal =
            Decimal.Parse(
                bal("locked").ToString(),
                CultureInfo.InvariantCulture)

            Dim qtd As Decimal = free + locked

            If qtd > 0D Then
                ativos(asset) = qtd
            End If

        Next

        Return ativos

    End Function

    Private Function GetAssetQty(account As JObject, asset As String) As Decimal

        For Each bal In account("balances")

            If bal("asset").ToString().
            Equals(asset, StringComparison.OrdinalIgnoreCase) Then

                Dim free As Decimal =
                Decimal.Parse(
                    bal("free").ToString(),
                    CultureInfo.InvariantCulture)

                Dim locked As Decimal =
                Decimal.Parse(
                    bal("locked").ToString(),
                    CultureInfo.InvariantCulture)

                Return free + locked

            End If

        Next

        Return 0D

    End Function

    'Private Function BINANCE_GetFuturesAssets() As Task(Of Dictionary(Of String, (Saldo As Decimal, Ordem As Decimal, Lucro As Decimal)))
    '    Dim json = QuerySigned("/fapi/v2/account", "recvWindow=5000", isFutures:=True)
    '    Dim account = JObject.Parse(json)

    '    ' Usaremos dicionário por símbolo de contrato (ex: BTCUSDT)
    '    Dim ativos As New Dictionary(Of String, (Saldo As Decimal, Ordem As Decimal, Lucro As Decimal))(StringComparer.OrdinalIgnoreCase)

    '    If account("code") IsNot Nothing Then
    '        Throw New Exception($"Binance ({account("code")}): {account("msg")}")
    '        Exit Function
    '    End If

    '    Try

    '        For Each asset In account("assets")
    '            Dim symbol = asset("asset").ToString()
    '            Dim saldo = Decimal.Parse(asset("walletBalance").ToString(), CultureInfo.InvariantCulture)
    '            Dim ordem = Decimal.Parse(asset("openOrderInitialMargin").ToString(), CultureInfo.InvariantCulture)

    '            ' Inicializa com lucro 0, será somado depois com base nas posições
    '            If saldo > 0 Or ordem > 0 Then
    '                ativos(symbol) = (Saldo:=saldo, Ordem:=ordem, Lucro:=0)
    '            End If
    '        Next

    '        ' 2. Pega lucro/prejuízo não realizado por contrato (BTCUSDT, ETHUSDT, etc.)
    '        For Each pos In account("positions")
    '            Dim contrato = pos("symbol").ToString()
    '            Dim lucro = Decimal.Parse(pos("unrealizedProfit").ToString(), CultureInfo.InvariantCulture)

    '            If lucro <> 0 Then
    '                If ativos.ContainsKey(contrato) Then
    '                    Dim atual = ativos(contrato)
    '                    ativos(contrato) = (atual.Saldo, atual.Ordem, atual.Lucro + lucro)
    '                Else
    '                    ativos(contrato) = (Saldo:=0, Ordem:=0, Lucro:=lucro)
    '                End If
    '            End If
    '        Next

    '        Return Task.FromResult(ativos)

    '    Catch ex As Exception
    '        FormMain.lbDebug.Clear()
    '        FormMain.lbDebug.AppendText("Erro ao trazer dados da conta de Futuros: " & ex.Message)
    '        Return Task.FromResult(New Dictionary(Of String, (Decimal, Decimal, Decimal)))
    '    End Try
    'End Function

    Private Async Function BINANCE_GetFuturesAssets() As Task(Of Dictionary(Of String, (Saldo As Decimal, Ordem As Decimal, Lucro As Decimal)))

        Dim json = Await QuerySignedAsync("/fapi/v2/account", "recvWindow=60000", isFutures:=True)

        If String.IsNullOrWhiteSpace(json) Then
            Return New Dictionary(Of String, (Decimal, Decimal, Decimal))
        End If

        Dim account As JObject = JObject.Parse(json)

        If account("code") IsNot Nothing Then
            Throw New Exception(
            $"Binance ({account("code")}): {account("msg")}"
        )
        End If

        Dim ativos As New Dictionary(Of String, (Saldo As Decimal, Ordem As Decimal, Lucro As Decimal))(
        StringComparer.OrdinalIgnoreCase)

        Try

            For Each asset In account("assets")

                Dim symbol As String =
                asset("asset").ToString()

                Dim saldo As Decimal =
                Decimal.Parse(
                    asset("walletBalance").ToString(),
                    CultureInfo.InvariantCulture)

                Dim ordem As Decimal =
                Decimal.Parse(
                    asset("openOrderInitialMargin").ToString(),
                    CultureInfo.InvariantCulture)

                If saldo > 0D OrElse ordem > 0D Then
                    ativos(symbol) = (
                    Saldo:=saldo,
                    Ordem:=ordem,
                    Lucro:=0D)
                End If

            Next

            For Each pos In account("positions")

                Dim contrato As String =
                pos("symbol").ToString()

                Dim lucro As Decimal =
                Decimal.Parse(
                    pos("unrealizedProfit").ToString(),
                    CultureInfo.InvariantCulture)

                If lucro <> 0D Then

                    If ativos.ContainsKey(contrato) Then

                        Dim atual = ativos(contrato)

                        ativos(contrato) = (
                        atual.Saldo,
                        atual.Ordem,
                        atual.Lucro + lucro)

                    Else

                        ativos(contrato) = (
                        Saldo:=0D,
                        Ordem:=0D,
                        Lucro:=lucro)

                    End If

                End If

            Next

            Return ativos

        Catch ex As Exception

            FormMain.lbDebug.Clear()
            FormMain.lbDebug.AppendText(
            "Erro ao trazer dados da conta de Futuros: " &
            ex.Message)

            Return New Dictionary(Of String, (Decimal, Decimal, Decimal))

        End Try

    End Function


    'Private Function BINANCE_GetAssetQty(asset As String) As Decimal
    '    Dim json = QuerySigned("/api/v3/account", "recvWindow=5000")
    '    Dim account = JObject.Parse(json)

    '    Try

    '        For Each bal In account("balances")
    '            If bal("asset").ToString().Equals(asset, StringComparison.OrdinalIgnoreCase) Then
    '                Dim free = Decimal.Parse(bal("free").ToString(), CultureInfo.InvariantCulture)
    '                Dim locked = Decimal.Parse(bal("locked").ToString(), CultureInfo.InvariantCulture)
    '                Return free + locked
    '            End If
    '        Next

    '        Return 0D

    '    Catch ex As Exception
    '        FormMain.lbDebug.AppendText(ex.Message)
    '        Return False
    '    End Try

    'End Function

    Private Async Function BINANCE_GetAssetQty(asset As String) As Task(Of Decimal)

        Dim json = Await QuerySignedAsync("/api/v3/account", "recvWindow=60000")

        If String.IsNullOrWhiteSpace(json) Then
            Return 0D
        End If

        Dim account As JObject = JObject.Parse(json)

        If account("code") IsNot Nothing Then

            Throw New Exception(
            $"Binance ({account("code")}): {account("msg")}"
        )

        End If

        Try

            For Each bal In account("balances")

                If bal("asset").ToString().
                Equals(asset, StringComparison.OrdinalIgnoreCase) Then

                    Dim free As Decimal =
                    Decimal.Parse(
                        bal("free").ToString(),
                        CultureInfo.InvariantCulture)

                    Dim locked As Decimal =
                    Decimal.Parse(
                        bal("locked").ToString(),
                        CultureInfo.InvariantCulture)

                    Return free + locked

                End If

            Next

            Return 0D

        Catch ex As Exception

            FormMain.lbDebug.AppendText(ex.Message)

            Return 0D

        End Try

    End Function
    'Public Async Function BINANCE_GetAllAssetsFull() As Task(Of Dictionary(Of String, Decimal))
    '    Dim spotAssets = Await BINANCE_GetSpotAssets()
    '    Dim futuresAssets = Await BINANCE_GetFuturesAssets()

    '    ' Junta os dois dicionários
    '    Dim allAccounts As New Dictionary(Of String, Decimal)(StringComparer.OrdinalIgnoreCase)

    '    For Each kv In spotAssets
    '        allAccounts(kv.Key) = kv.Value
    '    Next

    '    For Each kv In futuresAssets
    '        If allAccounts.ContainsKey(kv.Key) Then
    '            allAccounts(kv.Key) += kv.Value.Saldo
    '        Else
    '            allAccounts(kv.Key) = kv.Value.Saldo
    '        End If
    '    Next

    '    Return allAccounts

    'End Function

    Public Async Function BINANCE_GetAllAssetsFull() As Task(Of Dictionary(Of String, Decimal))

        Dim spotTask = BINANCE_GetSpotAccountAsync()
        Dim futuresTask = BINANCE_GetFuturesAccountAsync()

        Await Task.WhenAll(spotTask, futuresTask)

        Dim spotAccount = Await spotTask
        Dim futuresAccount = Await futuresTask

        Dim spotAssets = GetSpotAssets(spotAccount)
        Dim futuresAssets = GetFuturesAssets(futuresAccount)

        Dim allAccounts As New Dictionary(Of String, Decimal)(
        StringComparer.OrdinalIgnoreCase)

        For Each kv In spotAssets
            allAccounts(kv.Key) = kv.Value
        Next

        For Each kv In futuresAssets

            If allAccounts.ContainsKey(kv.Key) Then
                allAccounts(kv.Key) += kv.Value.Saldo
            Else
                allAccounts(kv.Key) = kv.Value.Saldo
            End If

        Next

        Return allAccounts

    End Function

    ''' <summary>
    ''' Retorna o saldo da carteira Futures e o patrimônio incluindo o PnL aberto.
    ''' </summary>
    Public Async Function BINANCE_GetFuturesAccountSummaryAsync() As Task(Of BinanceFuturesAccountSummary)
        Dim summary As New BinanceFuturesAccountSummary()

        Try
            Dim account = Await BINANCE_GetFuturesAccountAsync()
            If account Is Nothing Then Return summary

            summary.AvailableBalance = ParseDecimalInvariant(GetTokenValue(account, "availableBalance"))
            summary.InitialMargin = ParseDecimalInvariant(GetTokenValue(account, "totalInitialMargin"))
            summary.UnrealizedProfit = ParseDecimalInvariant(GetTokenValue(account, "totalUnrealizedProfit"))
            Dim walletBalanceToken = account("totalWalletBalance")
            If walletBalanceToken IsNot Nothing AndAlso walletBalanceToken.Type <> JTokenType.Null Then
                summary.WalletBalance = ParseDecimalInvariant(walletBalanceToken)
            Else
                summary.WalletBalance =
                    ParseDecimalInvariant(GetTokenValue(account, "totalMarginBalance")) - summary.UnrealizedProfit
            End If
            summary.Equity = summary.WalletBalance + summary.UnrealizedProfit
        Catch ex As Exception
            Debug.WriteLine("Erro ao trazer o resumo da conta Futures: " & ex.Message)
        End Try

        Return summary
    End Function

    ''' <summary>
    ''' Retorna somente os saldos da conta Spot.
    ''' O saldo da carteira Futures não deve entrar no grid Spot nem no Caixa.
    ''' </summary>
    Public Async Function BINANCE_GetSpotAssetsFull() As Task(Of Dictionary(Of String, Decimal))
        Dim spotAccount = Await BINANCE_GetSpotAccountAsync()
        If spotAccount Is Nothing Then
            Return New Dictionary(Of String, Decimal)(StringComparer.OrdinalIgnoreCase)
        End If

        Return GetSpotAssets(spotAccount)
    End Function

    Public Async Function BINANCE_GetCoinsInfo(Optional symbol As String = "") As Task(Of Object)
        Dim account = Await BINANCE_GetSpotAssetsFull()
        Dim urlBase As String = "https://api.binance.com/api/v3/ticker/price?symbol="

        Using client As New HttpClient()
            client.DefaultRequestHeaders.Add("User-Agent", "VBApp/1.0")

            If String.IsNullOrWhiteSpace(symbol) Then
                Dim result As New List(Of String)

                For Each cripto In account

                    Dim originalSymbol = cripto.Key.Trim().ToUpper()
                    Dim qtd = cripto.Value
                    Dim pairSymbol

                    If originalSymbol = "USDT" Then
                        pairSymbol = "USDC"
                    Else
                        pairSymbol = originalSymbol
                    End If

                    Dim pair = $"{pairSymbol}USDT"
                    Dim url = $"{urlBase}{pair}"

                    Try
                        Dim resp = Await client.GetAsync(url)
                        If Not resp.IsSuccessStatusCode Then Continue For

                        Dim content = Await resp.Content.ReadAsStringAsync()
                        Dim j = JsonDocument.Parse(content)
                        Dim priceStr = j.RootElement.GetProperty("price").GetString()
                        Dim price = Decimal.Parse(priceStr, CultureInfo.InvariantCulture)

                        If originalSymbol = "USDT" Then
                            price = 1 / price ' Inverter USDC→USDT para ter o preço do USDT em USD
                        End If

                        Dim totalUSD = price * qtd
                        If totalUSD >= 1 Then ' <<<<< FILTRO AQUI
                            result.Add($"{originalSymbol}|{price.ToString(CultureInfo.InvariantCulture)}|{qtd.ToString(CultureInfo.InvariantCulture)}")

                            'MsgBox($"simbolo:{originalSymbol} Preço:{price.ToString(CultureInfo.InvariantCulture)} Qtd:{qtd.ToString(CultureInfo.InvariantCulture)}")
                        End If

                    Catch ex As Exception
                        FormMain.lbDebug.Clear()
                        Debug.WriteLine($"Erro ao buscar {pair}: {ex.Message}")
                        FormMain.lbDebug.AppendText($"Erro ao buscar {pair}: {ex.Message}")
                        Continue For
                    End Try
                Next

                Return result

            Else
                ' Caso symbol específico
                Dim originalSymbol = symbol.Trim().ToUpper()
                Dim qtd As Decimal = Await BINANCE_GetAssetQty(originalSymbol)
                Dim pairSymbol = If(originalSymbol = "USDT", "USDC", originalSymbol)
                Dim pair = $"{pairSymbol}USDT"
                Dim url = $"{urlBase}{pair}"

                Dim resp = Await client.GetAsync(url)
                If Not resp.IsSuccessStatusCode Then Throw New Exception($"HTTP {CInt(resp.StatusCode)} – {resp.ReasonPhrase}")

                Dim content = Await resp.Content.ReadAsStringAsync()
                Dim j = JsonDocument.Parse(content)
                Dim priceStr = j.RootElement.GetProperty("price").GetString()
                Dim price = Decimal.Parse(priceStr, CultureInfo.InvariantCulture)

                If originalSymbol = "USDT" Then
                    price = 1 / price
                End If

                Return $"{price.ToString(CultureInfo.InvariantCulture)}|0|{qtd.ToString(CultureInfo.InvariantCulture)}"

            End If
        End Using

        Return Nothing

    End Function
    Public Async Function BINANCE_GetUSDTBRL() As Task(Of Decimal)
        Using client As New HttpClient()

            Dim url = $"https://api.binance.com/api/v3/ticker/price?symbol=USDTBRL"
            Dim response = Await client.GetAsync(url)

            If response.IsSuccessStatusCode Then
                Dim content = Await response.Content.ReadAsStringAsync()
                Dim json = JsonDocument.Parse(content)
                Dim priceStr = json.RootElement.GetProperty("price").GetString()
                Dim price As Decimal = Decimal.Parse(priceStr, CultureInfo.InvariantCulture)

                Return $"{price}"
            Else
                FormMain.lbDebug.AppendText($"HTTP {CInt(response.StatusCode)} – {response.ReasonPhrase}")
                Return False
            End If
        End Using

    End Function

    Public Async Function ParExisteNaBinance(symbol As String) As Task(Of Boolean)
        Try
            Dim url As String = $"https://api.binance.com/api/v3/exchangeInfo?symbol={symbol}USDT"
            Using client As New Net.Http.HttpClient()
                Dim response As Net.Http.HttpResponseMessage = Await client.GetAsync(url)
                Return response.IsSuccessStatusCode
            End Using
        Catch
            Return False
        End Try
    End Function
End Class
