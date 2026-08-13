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

        Dim apiKey As String = My.Settings.GateAPIKey.Trim()
        Dim secret As String = My.Settings.GateSecretKey.Trim()

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

End Class
