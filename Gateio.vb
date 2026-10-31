Imports System.Net.Http
Imports System.Security.Cryptography
Imports System.Text
Imports Newtonsoft.Json.Linq
Imports System.Globalization
Public Class Gateio
    Dim gec As New Coingecko
    Private Shared ReadOnly StableCoins As HashSet(Of String) =
    New HashSet(Of String)(StringComparer.OrdinalIgnoreCase) From {
        "USDT",
        "USDC",
        "FDUSD",
        "TUSD",
        "DAI"
    }

    Public Async Function GATE_GetAssetQty(symbol As String) As Task(Of Decimal)
        Dim assets = Await GATE_GetAllSpotAssets()
        Return assets.GetValueOrDefault(symbol.Trim().ToUpperInvariant(), 0D)
    End Function

    Public Async Function GATE_GetAllSpotAssets() As Task(Of Dictionary(Of String, Decimal))

        Dim endpoint = "/api/v4/spot/accounts"
        Dim url = "https://api.gateio.ws" & endpoint
        Dim method = "GET"
        Dim query = ""
        Dim body = ""
        Dim timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(CultureInfo.InvariantCulture)

        Dim bodyHash As String
        Using sha512 As New SHA512Managed()
            bodyHash = BitConverter.ToString(
                       sha512.ComputeHash(Encoding.UTF8.GetBytes(body))
                   ).Replace("-", "").ToLowerInvariant()
        End Using

        Dim stringToSign = $"{method}" & vbLf &
                       $"{endpoint}" & vbLf &
                       $"{query}" & vbLf &
                       $"{bodyHash}" & vbLf &
                       $"{timestamp}"

        Dim apiKey As String = ExchangeCredentials.GateAPIKey.Trim()
        Dim secret As String = ExchangeCredentials.GateSecretKey.Trim()

        If String.IsNullOrWhiteSpace(apiKey) OrElse String.IsNullOrWhiteSpace(secret) Then
            Throw New Exception("Chave API/Secret da Gate.io não configurada.")
        End If

        Dim signBytes = New HMACSHA512(Encoding.UTF8.GetBytes(secret)).
                    ComputeHash(Encoding.UTF8.GetBytes(stringToSign))
        Dim signature = BitConverter.ToString(signBytes).Replace("-", "").ToLowerInvariant()

        Dim handler As New HttpClientHandler() With {
            .SslProtocols = Security.Authentication.SslProtocols.Tls12
        }

        Using client As New HttpClient(handler)

            client.DefaultRequestHeaders.Add("KEY", apiKey)
            client.DefaultRequestHeaders.Add("Timestamp", timestamp)
            client.DefaultRequestHeaders.Add("SIGN", signature)
            client.DefaultRequestHeaders.Add("User-Agent", "VBApp/1.0")
            client.DefaultRequestHeaders.Accept.Add(
                New Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"))

            Dim resp = Await client.GetAsync(url)
            Dim raw = Await resp.Content.ReadAsStringAsync()

            If Not resp.IsSuccessStatusCode Then

                Debug.WriteLine(
                    $"[GATE.IO] Saldo HTTP {CInt(resp.StatusCode)}: {raw}")

                Try
                    Dim err = JObject.Parse(raw)
                    Throw New Exception(
                        $"Gate.io {CInt(resp.StatusCode)} – {err("label")}: {err("message")}")
                Catch ex As Exception When TypeOf ex Is Newtonsoft.Json.JsonException
                    Throw New Exception(
                        $"Gate.io {CInt(resp.StatusCode)} – {raw}")
                End Try

            End If

            Dim balances = JArray.Parse(raw)
            Dim result As New Dictionary(Of String, Decimal)(StringComparer.OrdinalIgnoreCase)

            For Each bal In balances

                Dim currency As String =
                    bal("currency")?.ToString().Trim().ToUpperInvariant()

                If String.IsNullOrWhiteSpace(currency) Then
                    Continue For
                End If

                Dim free As Decimal =
                    Decimal.Parse(
                        bal("available")?.ToString(),
                        NumberStyles.Any,
                        CultureInfo.InvariantCulture)

                Dim locked As Decimal =
                    Decimal.Parse(
                        bal("locked")?.ToString(),
                        NumberStyles.Any,
                        CultureInfo.InvariantCulture)

                Dim total As Decimal = free + locked

                If total <> 0D Then
                    result(currency) = total
                End If

            Next

            Debug.WriteLine(
                $"[GATE.IO] Saldos carregados: {result.Count}")

            Return result

        End Using

    End Function

    Public Async Function GATE_GetCoinsPrice(symbol As String) As Task(Of Decimal)
        symbol = symbol.Trim().ToUpper()

        If StableCoins.Contains(symbol) Then
            Return 1D
        End If

        Dim pair = $"{symbol}_USDT"
        Dim url = $"https://api.gateio.ws/api/v4/spot/tickers?currency_pair={pair}"

        Dim handler As New HttpClientHandler()
        handler.SslProtocols = Security.Authentication.SslProtocols.Tls12

        Using client As New HttpClient(handler)
            Try
                client.DefaultRequestHeaders.Add("User-Agent", "VBApp/1.0")
                client.DefaultRequestHeaders.Add("Accept", "application/json")

                Dim response = Await client.GetAsync(url)

                If Not response.IsSuccessStatusCode Then
                    Debug.WriteLine($"[GATE.IO] Par inválido ou erro: {pair} – HTTP {CInt(response.StatusCode)}")
                    Return 0D
                End If

                Dim json = JArray.Parse(Await response.Content.ReadAsStringAsync())
                If json.Count = 0 Then
                    Return 0D
                End If

                Dim lastPriceStr = json(0)("last").ToString()
                Return Decimal.Parse(lastPriceStr, CultureInfo.InvariantCulture)

            Catch ex As Exception
                Debug.WriteLine($"[GATE.IO] Erro ao buscar preço de {symbol}: {ex.Message}")
                Return 0D
            End Try
        End Using
    End Function

    Public Async Function GATE_GetCoinsInfo(symbol As String) As Task(Of String)
        Try
            Dim priceDecimal As Decimal = Await GATE_GetCoinsPrice(symbol)
            Dim qtd As Decimal = Await GATE_GetAssetQty(symbol)
            Return $"{priceDecimal.ToString(CultureInfo.InvariantCulture)}|0|{qtd.ToString(CultureInfo.InvariantCulture)}"
        Catch ex As Exception
            Debug.WriteLine($"[GATE.IO] Erro geral em GetCoinsInfo({symbol}): {ex.Message}")
            Return "0|0|0"
        End Try
    End Function

    Public Async Function GATE_GetSpotOpenOrdersAsync(
        Optional symbol As String = "") As Task(Of List(Of BinanceSpotOrder))

        If String.IsNullOrWhiteSpace(symbol) Then
            ' Esta rota retorna todos os pares com ordens pendentes e não
            ' exige que o par seja conhecido previamente pelo SQLite.
            Dim rawAll = Await SendSignedSpotAsync("GET", "/api/v4/spot/open_orders", String.Empty, String.Empty)
            Dim groups = TryCast(JToken.Parse(rawAll), JArray)
            Dim consolidated As New List(Of BinanceSpotOrder)()
            If groups Is Nothing Then Return consolidated

            For Each group In groups.OfType(Of JObject)()
                Dim groupOrders = TryCast(group("orders"), JArray)
                If groupOrders Is Nothing Then Continue For
                For Each item In groupOrders.OfType(Of JObject)()
                    Dim mapped = MapGateSpotOrder(item)
                    If mapped IsNot Nothing Then consolidated.Add(mapped)
                Next
            Next
            Return consolidated
        End If

        Dim query = "currency_pair=" & Uri.EscapeDataString(ToGatePair(symbol)) & "&status=open&limit=100"
        Dim raw = Await SendSignedSpotAsync("GET", "/api/v4/spot/orders", query, String.Empty)
        Dim orders = TryCast(JToken.Parse(raw), JArray)
        Dim result As New List(Of BinanceSpotOrder)()
        If orders Is Nothing Then Return result

        For Each item In orders.OfType(Of JObject)()
            Dim mapped = MapGateSpotOrder(item)
            If mapped IsNot Nothing Then result.Add(mapped)
        Next

        Return result
    End Function

    Private Shared Function MapGateSpotOrder(item As JObject) As BinanceSpotOrder
        Dim pair = ReadString(item, "currency_pair", "currencyPair")
        If String.IsNullOrWhiteSpace(pair) Then Return Nothing

        Dim orderId As Long
        Long.TryParse(ReadString(item, "id", "order_id"), NumberStyles.Integer, CultureInfo.InvariantCulture, orderId)
        Dim amount = ReadDecimal(item, "amount", "quantity")
        Dim left = ReadDecimal(item, "left", "remaining")

        Return New BinanceSpotOrder With {
            .Exchange = "GATE.IO",
            .Symbol = pair.Replace("_", String.Empty, StringComparison.OrdinalIgnoreCase),
            .OrderId = orderId,
            .Side = ReadString(item, "side").ToUpperInvariant(),
            .OrderType = ReadString(item, "type").ToUpperInvariant(),
            .Status = ReadString(item, "status", "account"),
            .Quantity = Math.Abs(amount),
            .ExecutedQuantity = Math.Max(0D, Math.Abs(amount - left)),
            .Price = ReadDecimal(item, "price"),
            .StopPrice = ReadDecimal(item, "stop_price", "trigger_price"),
            .OrderTime = ReadGateDateTime(item, "create_time_ms", "create_time", "update_time")
        }
    End Function

    Public Async Function GATE_CancelSpotOrderAsync(symbol As String, orderId As Long) As Task(Of Boolean)
        If String.IsNullOrWhiteSpace(symbol) Then Throw New ArgumentException("O símbolo da ordem é obrigatório.", NameOf(symbol))
        If orderId <= 0 Then Throw New ArgumentException("O identificador da ordem é inválido.", NameOf(orderId))

        Dim query = "currency_pair=" & Uri.EscapeDataString(ToGatePair(symbol))
        Await SendSignedSpotAsync("DELETE", "/api/v4/spot/orders/" & orderId.ToString(CultureInfo.InvariantCulture), query, String.Empty)
        Return True
    End Function

    Public Async Function GATE_CreateSpotOrderAsync(
        symbol As String,
        side As String,
        orderType As String,
        quantity As Decimal,
        Optional limitPrice As Decimal = 0D) As Task(Of String)

        Dim normalizedSide = side?.Trim().ToLowerInvariant()
        Dim normalizedType = orderType?.Trim().ToLowerInvariant()
        If String.IsNullOrWhiteSpace(symbol) OrElse quantity <= 0D Then
            Throw New ArgumentException("Símbolo e quantidade válidos são obrigatórios.")
        End If
        If normalizedSide <> "buy" AndAlso normalizedSide <> "sell" Then
            Throw New ArgumentException("O lado deve ser BUY ou SELL.", NameOf(side))
        End If
        If normalizedType <> "limit" AndAlso normalizedType <> "market" Then
            Throw New ArgumentException("O tipo deve ser LIMIT ou MARKET.", NameOf(orderType))
        End If
        If normalizedType = "limit" AndAlso limitPrice <= 0D Then
            Throw New ArgumentException("O preço limite deve ser maior que zero.", NameOf(limitPrice))
        End If

        Dim body As New JObject From {
            {"currency_pair", ToGatePair(symbol)},
            {"type", normalizedType},
            {"account", "spot"},
            {"side", normalizedSide},
            {"amount", quantity.ToString("0.########", CultureInfo.InvariantCulture)},
            {"price", If(normalizedType = "limit", limitPrice.ToString("0.########", CultureInfo.InvariantCulture), "0")},
            {"time_in_force", If(normalizedType = "limit", "gtc", "ioc")}
        }

        Return Await SendSignedSpotAsync("POST", "/api/v4/spot/orders", String.Empty, body.ToString(Newtonsoft.Json.Formatting.None))
    End Function

    Public Async Function ParExisteNaGateIo(symbol As String) As Task(Of Boolean)
        Try
            Dim parComUnderline As String = $"{symbol.Trim().ToUpper()}_USDT"
            Dim url As String = $"https://api.gateio.ws/api/v4/spot/tickers?currency_pair={parComUnderline}"
            Using client As New Net.Http.HttpClient()
                Dim response = Await client.GetAsync(url)
                Return response.IsSuccessStatusCode
            End Using
        Catch
            Return False
        End Try
    End Function

    Private Shared Function ToGatePair(symbol As String) As String
        Dim normalized = If(symbol, String.Empty).Trim().ToUpperInvariant().Replace("-", "_")
        If normalized.EndsWith("USDT", StringComparison.OrdinalIgnoreCase) AndAlso Not normalized.EndsWith("_USDT", StringComparison.OrdinalIgnoreCase) Then
            normalized = normalized.Substring(0, normalized.Length - 4) & "_USDT"
        End If
        If Not normalized.EndsWith("_USDT", StringComparison.OrdinalIgnoreCase) Then normalized &= "_USDT"
        Return normalized
    End Function

    Private Shared Async Function SendSignedSpotAsync(
        method As String,
        endpoint As String,
        query As String,
        body As String) As Task(Of String)

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
        Dim signatureText = method.ToUpperInvariant() & vbLf & endpoint & vbLf & If(query, String.Empty) & vbLf & bodyHash & vbLf & timestamp
        Dim signature As String
        Using hmac As New HMACSHA512(Encoding.UTF8.GetBytes(secret))
            signature = Convert.ToHexString(hmac.ComputeHash(Encoding.UTF8.GetBytes(signatureText))).ToLowerInvariant()
        End Using

        Dim url = "https://api.gateio.ws" & endpoint & If(String.IsNullOrWhiteSpace(query), String.Empty, "?" & query)
        Using client As New HttpClient()
            client.DefaultRequestHeaders.TryAddWithoutValidation("KEY", apiKey)
            client.DefaultRequestHeaders.TryAddWithoutValidation("Timestamp", timestamp)
            client.DefaultRequestHeaders.TryAddWithoutValidation("SIGN", signature)
            client.DefaultRequestHeaders.UserAgent.ParseAdd("PortfolioCripto/1.0")

            Using request As New HttpRequestMessage(New HttpMethod(method), url)
                If Not String.IsNullOrWhiteSpace(body) Then
                    request.Content = New StringContent(body, Encoding.UTF8, "application/json")
                End If
                Using response = Await client.SendAsync(request)
                    Dim responseBody = Await response.Content.ReadAsStringAsync()
                    If Not response.IsSuccessStatusCode Then
                        Try
                            Dim errorObject = JObject.Parse(responseBody)
                            Throw New InvalidOperationException($"Gate.io HTTP {CInt(response.StatusCode)}: {ReadString(errorObject, "label", "message")}")
                        Catch ex As Newtonsoft.Json.JsonException
                            Throw New InvalidOperationException($"Gate.io HTTP {CInt(response.StatusCode)}: {responseBody}")
                        End Try
                    End If
                    Return responseBody
                End Using
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

    Private Shared Function ReadGateDateTime(item As JObject, ParamArray names() As String) As DateTime
        Dim raw = ReadString(item, names)
        Dim number As Decimal
        If Decimal.TryParse(raw, NumberStyles.Any, CultureInfo.InvariantCulture, number) Then
            If raw.Length >= 13 Then number /= 1000D
            Return DateTimeOffset.FromUnixTimeSeconds(CLng(number)).LocalDateTime
        End If

        Dim parsed As DateTime
        If DateTime.TryParse(raw, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, parsed) Then Return parsed.ToLocalTime()
        Return DateTime.MinValue
    End Function

End Class
