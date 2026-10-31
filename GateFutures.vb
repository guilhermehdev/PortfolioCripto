Imports System.Globalization
Imports System.Net.Http
Imports System.Net.Http.Headers
Imports System.Security.Authentication
Imports System.Security.Cryptography
Imports System.Text
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

''' <summary>
''' Cliente REST para os futuros perpétuos USDT da Gate API v4.
''' Os métodos de leitura reutilizam os modelos de Futures já usados pela tela.
''' </summary>
Public Class GateFutures
    Private Const BaseUrl As String = "https://fx-api.gateio.ws/api/v4"
    Private Const ApiPrefix As String = "/api/v4"
    Private Const Settle As String = "usdt"
    Private Shared ReadOnly Http As HttpClient = CreateHttpClient()
    Private ReadOnly _contractMultipliers As New Dictionary(Of String, Decimal)(StringComparer.OrdinalIgnoreCase)

    Private Shared Function CreateHttpClient() As HttpClient
        Dim handler As New HttpClientHandler With {
            .SslProtocols = SslProtocols.Tls12 Or SslProtocols.Tls13
        }
        Dim client As New HttpClient(handler)
        client.Timeout = TimeSpan.FromSeconds(20)
        client.DefaultRequestHeaders.Accept.Add(New MediaTypeWithQualityHeaderValue("application/json"))
        client.DefaultRequestHeaders.UserAgent.ParseAdd("PortfolioCripto/1.0")
        Return client
    End Function

    Public Async Function GATE_GetFuturesAccountSummaryAsync() As Task(Of BinanceFuturesAccountSummary)
        Dim summary As New BinanceFuturesAccountSummary()
        Dim raw = Await SendSignedAsync(HttpMethod.Get, "/futures/usdt/accounts", String.Empty, String.Empty)
        Dim account = JObject.Parse(raw)

        summary.WalletBalance = ReadDecimal(account, "total", "total_balance", "wallet_balance")
        summary.AvailableBalance = ReadDecimal(account, "available", "available_balance", "cross_available")
        summary.UnrealizedProfit = ReadDecimal(account, "unrealised_pnl", "unrealized_pnl", "unrealizedProfit")
        summary.InitialMargin = ReadDecimal(account, "position_margin", "initial_margin") +
                                ReadDecimal(account, "order_margin", "order_initial_margin")
        summary.Equity = summary.WalletBalance + summary.UnrealizedProfit
        Return summary
    End Function

    Public Async Function GATE_GetFuturesPositionsAsync(
        Optional includeEmptyPositions As Boolean = False) As Task(Of List(Of BinanceFuturesPosition))
        Dim result As New List(Of BinanceFuturesPosition)()
        Dim raw = Await SendSignedAsync(HttpMethod.Get, "/futures/usdt/positions", String.Empty, String.Empty)
        Dim positions = TryCast(JToken.Parse(raw), JArray)
        If positions Is Nothing Then Return result

        For Each item In positions.OfType(Of JObject)()
            Dim contract = ReadString(item, "contract")
            If String.IsNullOrWhiteSpace(contract) Then Continue For

            Dim sizeContracts = ReadDecimal(item, "size", "amount")
            Dim unrealized = ReadDecimal(item, "unrealised_pnl", "unrealized_pnl", "unrealizedProfit")
            If Not includeEmptyPositions AndAlso sizeContracts = 0D AndAlso unrealized = 0D Then Continue For

            Dim multiplier = Await GetContractMultiplierAsync(contract)
            Dim quantity = sizeContracts * multiplier
            Dim markPrice = ReadDecimal(item, "mark_price", "markPrice")
            Dim entryPrice = ReadDecimal(item, "entry_price", "entryPrice")
            Dim notional = Math.Abs(ReadDecimal(item, "value", "notional"))
            If notional = 0D Then notional = Math.Abs(quantity * markPrice)

            Dim leverage As Integer
            Integer.TryParse(ReadString(item, "leverage", "lever"), leverage)

            result.Add(New BinanceFuturesPosition With {
                .Exchange = "GATE",
                .Symbol = contract.Replace("_", String.Empty, StringComparison.OrdinalIgnoreCase),
                .PositionAmount = quantity,
                .EntryPrice = entryPrice,
                .MarkPrice = markPrice,
                .UnrealizedProfit = unrealized,
                .LiquidationPrice = ReadDecimal(item, "liquidation_price", "liquidationPrice"),
                .Leverage = leverage,
                .Notional = notional,
                .MarginType = ReadString(item, "pos_margin_mode", "margin_mode", "mode"),
                .IsolatedMargin = ReadDecimal(item, "isolated_margin"),
                .InitialMargin = ReadDecimal(item, "initial_margin", "position_margin", "margin"),
                .PositionSide = If(sizeContracts < 0D, "SHORT", "LONG")
            })
        Next

        Return result
    End Function

    Public Async Function GATE_GetFuturesOpenOrdersAsync(
        Optional symbol As String = "") As Task(Of List(Of BinanceFuturesOrder))
        Dim query = "status=open&limit=100"
        If Not String.IsNullOrWhiteSpace(symbol) Then
            query &= "&contract=" & Uri.EscapeDataString(ToContract(symbol))
        End If

        Dim raw = Await SendSignedAsync(HttpMethod.Get, "/futures/usdt/orders", query, String.Empty)
        Dim orders = TryCast(JToken.Parse(raw), JArray)
        Dim result As New List(Of BinanceFuturesOrder)()
        If orders Is Nothing Then Return result

        For Each item In orders.OfType(Of JObject)()
            Dim contract = ReadString(item, "contract")
            If String.IsNullOrWhiteSpace(contract) Then Continue For
            Dim multiplier = Await GetContractMultiplierAsync(contract)
            Dim sizeContracts = ReadDecimal(item, "size", "amount")
            Dim leftContracts = ReadDecimal(item, "left")
            Dim quantity = Math.Abs(sizeContracts * multiplier)
            Dim executed = Math.Max(0D, Math.Abs(sizeContracts - leftContracts) * multiplier)
            Dim price = ReadDecimal(item, "price")
            Dim orderId = ReadLong(item, "id", "order_id", "id_string")

            result.Add(New BinanceFuturesOrder With {
                .Exchange = "GATE",
                .Symbol = contract.Replace("_", String.Empty, StringComparison.OrdinalIgnoreCase),
                .OrderId = orderId,
                .PositionSide = If(sizeContracts < 0D, "SHORT", "LONG"),
                .Side = If(sizeContracts < 0D, "SELL", "BUY"),
                .OrderType = If(String.Equals(ReadString(item, "tif"), "ioc", StringComparison.OrdinalIgnoreCase), "MARKET", "LIMIT"),
                .Status = ReadString(item, "status", "open_status"),
                .Quantity = quantity,
                .ExecutedQuantity = executed,
                .Price = price,
                .StopPrice = 0D,
                .OrderTime = ReadDateTime(item, "create_time", "update_time")
            })
        Next

        Return result
    End Function

    Public Async Function GATE_GetFuturesProtectionOrdersAsync(
        Optional positions As List(Of BinanceFuturesPosition) = Nothing) As Task(Of List(Of BinanceFuturesProtectionOrder))
        Dim raw = Await SendSignedAsync(HttpMethod.Get, "/futures/usdt/price_orders", "status=open&limit=100", String.Empty)
        Dim orders = TryCast(JToken.Parse(raw), JArray)
        Dim result As New List(Of BinanceFuturesProtectionOrder)()
        If orders Is Nothing Then Return result
        If positions Is Nothing Then positions = Await GATE_GetFuturesPositionsAsync()

        For Each item In orders.OfType(Of JObject)()
            Dim initial = TryCast(item("initial"), JObject)
            Dim trigger = TryCast(item("trigger"), JObject)
            If initial Is Nothing OrElse trigger Is Nothing Then Continue For

            Dim contract = ReadString(initial, "contract")
            Dim sizeContracts = ReadDecimal(initial, "size", "amount")
            Dim multiplier = Await GetContractMultiplierAsync(contract)
            Dim triggerPrice = ReadDecimal(trigger, "price", "trigger_price")
            Dim markPrice = positions.
                Where(Function(position) String.Equals(position.Symbol, contract.Replace("_", String.Empty, StringComparison.OrdinalIgnoreCase), StringComparison.OrdinalIgnoreCase)).
                Select(Function(position) position.MarkPrice).
                FirstOrDefault()
            Dim side = If(sizeContracts < 0D, "SHORT", "LONG")
            Dim autoSize = ReadString(item, "auto_size", "order_type")
            If autoSize.Contains("short", StringComparison.OrdinalIgnoreCase) Then side = "SHORT"
            If autoSize.Contains("long", StringComparison.OrdinalIgnoreCase) Then side = "LONG"

            Dim isTakeProfit = If(
                markPrice > 0D,
                If(String.Equals(side, "SHORT", StringComparison.OrdinalIgnoreCase), triggerPrice < markPrice, triggerPrice > markPrice),
                ReadLong(trigger, "rule") = 1L)

            result.Add(New BinanceFuturesProtectionOrder With {
                .Exchange = "GATE",
                .AlgoId = ReadLong(item, "id", "id_string"),
                .Symbol = contract.Replace("_", String.Empty, StringComparison.OrdinalIgnoreCase),
                .PositionSide = side,
                .Side = If(String.Equals(side, "SHORT", StringComparison.OrdinalIgnoreCase), "BUY", "SELL"),
                .OrderType = If(isTakeProfit, "TAKE_PROFIT", "STOP_LOSS"),
                .Quantity = Math.Abs(sizeContracts * multiplier),
                .StopPrice = triggerPrice,
                .Price = ReadDecimal(initial, "price"),
                .Status = ReadString(item, "status"),
                .OrderTime = ReadDateTime(item, "create_time")
            })
        Next

        Return result
    End Function

    Public Async Function GATE_SetFuturesLeverageAsync(symbol As String, leverage As Integer) As Task
        If leverage <= 0 Then Return
        Dim contract = ToContract(symbol)
        Dim query = "leverage=" & leverage.ToString(CultureInfo.InvariantCulture)
        Await SendSignedAsync(HttpMethod.Post, $"/futures/usdt/positions/{Uri.EscapeDataString(contract)}/leverage", query, String.Empty)
    End Function

    Public Async Function GATE_CreateFuturesOrderAsync(
        symbol As String,
        positionSide As String,
        orderType As String,
        quantity As Decimal,
        price As Decimal) As Task(Of BinanceFuturesOrder)
        Dim contract = ToContract(symbol)
        Dim multiplier = Await GetContractMultiplierAsync(contract)
        Dim sizeContracts = Math.Round(quantity / Math.Max(multiplier, 0.00000001D), 8, MidpointRounding.AwayFromZero)
        If String.Equals(positionSide, "SHORT", StringComparison.OrdinalIgnoreCase) Then sizeContracts = -Math.Abs(sizeContracts) Else sizeContracts = Math.Abs(sizeContracts)

        Dim isMarket = String.Equals(orderType, "MARKET", StringComparison.OrdinalIgnoreCase)
        Dim body As New JObject From {
            {"contract", contract},
            {"size", sizeContracts.ToString("0.########", CultureInfo.InvariantCulture)},
            {"price", If(isMarket, "0", price.ToString("0.########", CultureInfo.InvariantCulture))},
            {"tif", If(isMarket, "ioc", "gtc")}
        }
        Dim raw = Await SendSignedAsync(HttpMethod.Post, "/futures/usdt/orders", String.Empty, body.ToString(Newtonsoft.Json.Formatting.None))
        Return ParseCreatedOrder(JObject.Parse(raw), contract, sizeContracts, price, orderType)
    End Function

    Public Async Function GATE_CancelFuturesOrderAsync(symbol As String, orderId As Long) As Task(Of Boolean)
        If orderId <= 0 Then Return False
        Await SendSignedAsync(HttpMethod.Delete, $"/futures/usdt/orders/{orderId.ToString(CultureInfo.InvariantCulture)}", String.Empty, String.Empty)
        Return True
    End Function

    Public Async Function GATE_CloseFuturesPositionAsync(
        symbol As String,
        positionSide As String,
        quantity As Decimal,
        limitPrice As Decimal) As Task(Of BinanceFuturesOrder)
        Dim contract = ToContract(symbol)
        Dim multiplier = Await GetContractMultiplierAsync(contract)
        Dim sizeContracts = Math.Round(quantity / Math.Max(multiplier, 0.00000001D), 8, MidpointRounding.AwayFromZero)
        If String.Equals(positionSide, "SHORT", StringComparison.OrdinalIgnoreCase) Then sizeContracts = Math.Abs(sizeContracts) Else sizeContracts = -Math.Abs(sizeContracts)

        Dim body As New JObject From {
            {"contract", contract},
            {"size", sizeContracts.ToString("0.########", CultureInfo.InvariantCulture)},
            {"price", limitPrice.ToString("0.########", CultureInfo.InvariantCulture)},
            {"tif", "gtc"},
            {"reduce_only", True}
        }
        Dim raw = Await SendSignedAsync(HttpMethod.Post, "/futures/usdt/orders", String.Empty, body.ToString(Newtonsoft.Json.Formatting.None))
        Return ParseCreatedOrder(JObject.Parse(raw), contract, sizeContracts, limitPrice, "LIMIT")
    End Function

    Public Async Function GATE_CreateFuturesProtectionOrderAsync(
        symbol As String,
        positionSide As String,
        protectionType As String,
        quantity As Decimal,
        triggerPrice As Decimal) As Task(Of Long)
        Dim contract = ToContract(symbol)
        Dim multiplier = Await GetContractMultiplierAsync(contract)
        Dim sizeContracts = Math.Round(quantity / Math.Max(multiplier, 0.00000001D), 8, MidpointRounding.AwayFromZero)
        Dim isShort = String.Equals(positionSide, "SHORT", StringComparison.OrdinalIgnoreCase)
        If isShort Then sizeContracts = -Math.Abs(sizeContracts) Else sizeContracts = Math.Abs(sizeContracts)
        Dim isTakeProfit = protectionType.Contains("TAKE", StringComparison.OrdinalIgnoreCase) OrElse protectionType.Contains("TP", StringComparison.OrdinalIgnoreCase)
        Dim rule = If(isShort,
                      If(isTakeProfit, 2, 1),
                      If(isTakeProfit, 1, 2))
        Dim body As New JObject From {
            {"initial", New JObject From {
                {"contract", contract},
                {"size", sizeContracts.ToString("0.########", CultureInfo.InvariantCulture)},
                {"price", "0"},
                {"tif", "ioc"}
            }},
            {"trigger", New JObject From {
                {"strategy_type", 0},
                {"price_type", 1},
                {"price", triggerPrice.ToString("0.########", CultureInfo.InvariantCulture)},
                {"rule", rule},
                {"expiration", 0}
            }},
            {"order_type", If(isShort, "close-short-order", "close-long-order")}
        }
        Dim raw = Await SendSignedAsync(HttpMethod.Post, "/futures/usdt/price_orders", String.Empty, body.ToString(Newtonsoft.Json.Formatting.None))
        Return ReadLong(JObject.Parse(raw), "id", "id_string")
    End Function

    Public Async Function GATE_CancelFuturesProtectionOrderAsync(symbol As String, algoId As Long) As Task(Of Boolean)
        If algoId <= 0 Then Return False
        Await SendSignedAsync(HttpMethod.Delete, $"/futures/usdt/price_orders/{algoId.ToString(CultureInfo.InvariantCulture)}", String.Empty, String.Empty)
        Return True
    End Function

    Private Async Function GetContractMultiplierAsync(contract As String) As Task(Of Decimal)
        contract = ToContract(contract)
        Dim cached As Decimal
        If _contractMultipliers.TryGetValue(contract, cached) Then Return cached

        Try
            Dim raw = Await SendPublicAsync($"/futures/usdt/contracts/{Uri.EscapeDataString(contract)}")
            Dim item = JObject.Parse(raw)
            Dim multiplier = ReadDecimal(item, "quanto_multiplier", "contract_size", "multiplier")
            If multiplier <= 0D Then multiplier = 1D
            _contractMultipliers(contract) = multiplier
            Return multiplier
        Catch ex As Exception
            Debug.WriteLine($"[GATE FUTURES] Multiplicador indisponível para {contract}: {ex.Message}")
            Return 1D
        End Try
    End Function

    Private Shared Function ParseCreatedOrder(item As JObject, contract As String, size As Decimal, price As Decimal, orderType As String) As BinanceFuturesOrder
        Return New BinanceFuturesOrder With {
            .Exchange = "GATE",
            .Symbol = contract.Replace("_", String.Empty, StringComparison.OrdinalIgnoreCase),
            .OrderId = ReadLong(item, "id", "order_id", "id_string"),
            .PositionSide = If(size < 0D, "SHORT", "LONG"),
            .Side = If(size < 0D, "SELL", "BUY"),
            .OrderType = orderType,
            .Status = ReadString(item, "status", "open_status", "finish_as"),
            .Quantity = Math.Abs(size),
            .ExecutedQuantity = 0D,
            .Price = price,
            .StopPrice = 0D,
            .OrderTime = ReadDateTime(item, "create_time")
        }
    End Function

    Private Shared Function ToContract(symbol As String) As String
        Dim normalized = If(symbol, String.Empty).Trim().ToUpperInvariant().Replace("-", "_")
        If normalized.EndsWith("USDT", StringComparison.OrdinalIgnoreCase) AndAlso Not normalized.EndsWith("_USDT", StringComparison.OrdinalIgnoreCase) Then
            normalized = normalized.Substring(0, normalized.Length - 4) & "_USDT"
        End If
        If Not normalized.EndsWith("_USDT", StringComparison.OrdinalIgnoreCase) Then normalized &= "_USDT"
        Return normalized
    End Function

    Private Shared Async Function SendPublicAsync(path As String) As Task(Of String)
        Using response = Await Http.GetAsync(BaseUrl & path)
            Dim body = Await response.Content.ReadAsStringAsync()
            If Not response.IsSuccessStatusCode Then Throw New InvalidOperationException($"Gate Futures HTTP {CInt(response.StatusCode)}: {body}")
            Return body
        End Using
    End Function

    Private Shared Async Function SendSignedAsync(method As HttpMethod, path As String, query As String, body As String) As Task(Of String)
        Dim apiKey = ExchangeCredentials.GateAPIKey.Trim()
        Dim secret = ExchangeCredentials.GateSecretKey.Trim()
        If String.IsNullOrWhiteSpace(apiKey) OrElse String.IsNullOrWhiteSpace(secret) Then
            Throw New InvalidOperationException("As credenciais da Gate não estão configuradas no Windows.")
        End If

        Dim timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(CultureInfo.InvariantCulture)
        Dim bodyHash As String
        Using sha As SHA512 = SHA512.Create()
            bodyHash = Convert.ToHexString(sha.ComputeHash(Encoding.UTF8.GetBytes(If(body, String.Empty)))).ToLowerInvariant()
        End Using
        Dim signatureText = method.Method.ToUpperInvariant() & vbLf & ApiPrefix & path & vbLf & If(query, String.Empty) & vbLf & bodyHash & vbLf & timestamp
        Dim signature As String
        Using hmac As New HMACSHA512(Encoding.UTF8.GetBytes(secret))
            signature = Convert.ToHexString(hmac.ComputeHash(Encoding.UTF8.GetBytes(signatureText))).ToLowerInvariant()
        End Using

        Dim url = BaseUrl & path & If(String.IsNullOrWhiteSpace(query), String.Empty, "?" & query)
        Using request As New HttpRequestMessage(method, url)
            request.Headers.TryAddWithoutValidation("KEY", apiKey)
            request.Headers.TryAddWithoutValidation("Timestamp", timestamp)
            request.Headers.TryAddWithoutValidation("SIGN", signature)
            request.Headers.TryAddWithoutValidation("X-Gate-Size-Decimal", "1")
            If Not String.IsNullOrWhiteSpace(body) Then
                request.Content = New StringContent(body, Encoding.UTF8, "application/json")
            End If
            Using response = Await Http.SendAsync(request)
                Dim responseBody = Await response.Content.ReadAsStringAsync()
                If Not response.IsSuccessStatusCode Then
                    Try
                        Dim errorObject = JObject.Parse(responseBody)
                        Throw New InvalidOperationException($"Gate Futures HTTP {CInt(response.StatusCode)} ({ReadString(errorObject, "label")}): {ReadString(errorObject, "message")}")
                    Catch ex As JsonException
                        Throw New InvalidOperationException($"Gate Futures HTTP {CInt(response.StatusCode)}: {responseBody}")
                    End Try
                End If
                Return responseBody
            End Using
        End Using
    End Function

    Private Shared Function ReadString(item As JObject, ParamArray names() As String) As String
        For Each name In names
            Dim token = item(name)
            If token IsNot Nothing AndAlso token.Type <> JTokenType.Null Then Return token.ToString()
        Next
        Return String.Empty
    End Function

    Private Shared Function ReadDecimal(item As JObject, ParamArray names() As String) As Decimal
        Dim value As Decimal
        Decimal.TryParse(ReadString(item, names), NumberStyles.Any, CultureInfo.InvariantCulture, value)
        Return value
    End Function

    Private Shared Function ReadLong(item As JObject, ParamArray names() As String) As Long
        Dim value As Long
        Long.TryParse(ReadString(item, names), NumberStyles.Any, CultureInfo.InvariantCulture, value)
        Return value
    End Function

    Private Shared Function ReadDateTime(item As JObject, ParamArray names() As String) As DateTime
        Dim seconds As Decimal = ReadDecimal(item, names)
        If seconds <= 0D Then Return DateTime.MinValue
        Try
            Return DateTimeOffset.FromUnixTimeMilliseconds(CLng(seconds * 1000D)).LocalDateTime
        Catch
            Return DateTime.MinValue
        End Try
    End Function
End Class
