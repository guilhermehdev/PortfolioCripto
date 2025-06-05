Imports System.Globalization
Imports System.Net.Http
Imports System.Security.Cryptography
Imports System.Security.Principal
Imports System.Text
Imports System.Text.Json
Imports Newtonsoft.Json.Linq
Public Class Binance

    Private Function QuerySigned(ByVal path As String, ByVal query As String) As String
        Dim apiKey As String = My.Settings.BinanceAPIKey
        Dim secret As String = My.Settings.BinanceSecretKey

        Dim qs = query & "&timestamp=" & (DateTimeOffset.UtcNow.ToUnixTimeMilliseconds())
        Using hmac As New HMACSHA256(Encoding.UTF8.GetBytes(secret))
            Dim sigBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(qs))
            Dim signature = BitConverter.ToString(sigBytes).Replace("-", "").ToLower()
            qs &= "&signature=" & signature
        End Using

        Using cli As New HttpClient()
            cli.BaseAddress = New Uri("https://api.binance.com")
            cli.DefaultRequestHeaders.Add("X-MBX-APIKEY", apiKey)
            Dim resp = cli.GetAsync(path & "?" & qs).Result
            Return resp.Content.ReadAsStringAsync().Result
        End Using

    End Function

    Private Function BINANCE_GetAssetQty(asset As String) As Decimal
        Dim json = QuerySigned("/api/v3/account", "recvWindow=5000")
        Dim account = JObject.Parse(json)

        Try

            For Each bal In account("balances")
                If bal("asset").ToString().Equals(asset, StringComparison.OrdinalIgnoreCase) Then
                    Dim free = Decimal.Parse(bal("free").ToString(), CultureInfo.InvariantCulture)
                    Dim locked = Decimal.Parse(bal("locked").ToString(), CultureInfo.InvariantCulture)
                    Return free + locked
                End If
            Next

            Return 0D

        Catch ex As Exception
            Debug.Write(ex.Message)
            Return False
        End Try

    End Function

    Private Function BINANCE_GetAllAssets() As Task(Of Dictionary(Of String, Decimal))
        Dim json = QuerySigned("/api/v3/account", "recvWindow=5000")
        Dim account = JObject.Parse(json)

        Dim ativos As New Dictionary(Of String, Decimal)(StringComparer.OrdinalIgnoreCase)

        Try
            For Each bal In account("balances")
                Dim asset As String = bal("asset").ToString()
                Dim free = Decimal.Parse(bal("free").ToString(), CultureInfo.InvariantCulture)
                Dim locked = Decimal.Parse(bal("locked").ToString(), CultureInfo.InvariantCulture)
                Dim qtd As Decimal = free + locked

                If qtd > 0D Then
                    ativos(asset) = qtd
                End If
            Next

            Return Task.FromResult(ativos)

        Catch ex As Exception
            Debug.WriteLine("Erro em BINANCE_GetAllAssets: " & ex.Message)
            Return Task.FromResult(New Dictionary(Of String, Decimal)) ' vazio
        End Try
    End Function

    'Public Async Function BINANCE_GetCoinsInfo(Optional symbol As String = "") As Task(Of String)
    '    Dim account = Await BINANCE_GetAllAssets()
    '    Dim originalSymbol As String = ""
    '    Dim pairSymbol As String = ""
    '    Dim pair As String = ""
    '    Dim url As String = "https://api.binance.com/api/v3/ticker/price?symbol="

    '    Using client As New HttpClient()

    '        If symbol Is Nothing OrElse symbol.Trim() = "" Then

    '            For Each cripto In account

    '                originalSymbol = cripto.Key.Trim().ToUpper()
    '                pairSymbol = originalSymbol

    '                If originalSymbol = "USDT" Then
    '                    pairSymbol = "USDC"
    '                End If

    '                pair = $"{pairSymbol}USDT"
    '                url = $"{url}{pair}"

    '                Dim resp = Await client.GetAsync(url)
    '                If Not resp.IsSuccessStatusCode Then
    '                    Throw New Exception($"HTTP {CInt(resp.StatusCode)} – {resp.ReasonPhrase}")
    '                End If

    '                Dim content = Await resp.Content.ReadAsStringAsync()
    '                Dim j = JsonDocument.Parse(content)
    '                Dim priceStr = j.RootElement.GetProperty("price").GetString()
    '                Dim price = Decimal.Parse(priceStr, CultureInfo.InvariantCulture)

    '                ' Se estamos usando USDCUSDT, inverter o valor (USDT → USD)
    '                If originalSymbol = "USDT" Then
    '                    price = 1 / price
    '                End If

    '            Next

    '        Else

    '            originalSymbol = symbol.Trim().ToUpper()
    '            pairSymbol = originalSymbol

    '            If originalSymbol = "USDT" Then
    '                pairSymbol = "USDC"
    '            End If

    '            pair = $"{pairSymbol}USDT"
    '            url = $"{url}{pair}"

    '            Dim resp = Await client.GetAsync(url)
    '            If Not resp.IsSuccessStatusCode Then
    '                Throw New Exception($"HTTP {CInt(resp.StatusCode)} – {resp.ReasonPhrase}")
    '            End If

    '            Dim content = Await resp.Content.ReadAsStringAsync()
    '            Dim j = JsonDocument.Parse(content)
    '            Dim priceStr = j.RootElement.GetProperty("price").GetString()
    '            Dim price = Decimal.Parse(priceStr, CultureInfo.InvariantCulture)

    '            ' Se estamos usando USDCUSDT, inverter o valor (USDT → USD)
    '            If originalSymbol = "USDT" Then
    '                price = 1 / price
    '            End If

    '            Dim qtd As Decimal = BINANCE_GetAssetQty(originalSymbol)

    '            Return $"{price.ToString(CultureInfo.InvariantCulture)}|0|{qtd.ToString(CultureInfo.InvariantCulture)}"

    '        End If

    '    End Using

    'End Function

    Public Async Function BINANCE_GetCoinsInfo(Optional symbol As String = "") As Task(Of Object)
        Dim account = Await Task.Run(Function() BINANCE_GetAllAssets()) ' Executa em thread separada se for necessário
        Dim urlBase As String = "https://api.binance.com/api/v3/ticker/price?symbol="

        Using client As New HttpClient()
            client.DefaultRequestHeaders.Add("User-Agent", "VBApp/1.0")

            If String.IsNullOrWhiteSpace(symbol) Then
                Dim result As New List(Of String)
                For Each cripto In account
                    Dim originalSymbol = cripto.Key.Trim().ToUpper()
                    Dim qtd = cripto.Value
                    Dim pairSymbol = If(originalSymbol = "USDT", "USDC", originalSymbol)
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

                        result.Add($"{originalSymbol}|{price.ToString(CultureInfo.InvariantCulture)}|{qtd.ToString(CultureInfo.InvariantCulture)}")

                    Catch ex As Exception
                        Debug.WriteLine($"Erro ao buscar {pair}: {ex.Message}")
                        Continue For
                    End Try
                Next

                Return result

            Else
                ' Caso symbol específico
                Dim originalSymbol = symbol.Trim().ToUpper()
                Dim qtd As Decimal = BINANCE_GetAssetQty(originalSymbol)
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
                Debug.Write($"HTTP {CInt(response.StatusCode)} – {response.ReasonPhrase}")
                Return False
            End If
        End Using

    End Function

End Class
