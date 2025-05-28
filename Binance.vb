Imports System.Globalization
Imports System.Net.Http
Imports System.Security.Cryptography
Imports System.Text
Imports System.Text.Json
Imports Newtonsoft.Json.Linq
Public Class Binance
    Private apiKey As String = My.Settings.BinanceAPIKey
    Private secret As String = My.Settings.BinanceSecretKey
    Private Function QuerySigned(ByVal path As String, ByVal query As String) As String

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

    Public Function BINANCE_getMyAccount()
        Dim json = QuerySigned("/api/v3/account", "recvWindow=5000")
        Dim account = JObject.Parse(json)
        Dim ativos = account("balances")

        For Each asset In ativos
            Dim moeda = asset("asset").ToString()
            Dim free = Decimal.Parse(asset("free").ToString(), CultureInfo.InvariantCulture)
            Dim locked = Decimal.Parse(asset("locked").ToString(), CultureInfo.InvariantCulture)

            If free > 0 Or locked > 0 Then
                Return $"{moeda}|{free}"
            End If
        Next

        Return False

    End Function
    Public Async Function BINANCE_GetCoinsPrice(symbol As String) As Task(Of String)
        Using client As New HttpClient()
            Dim usdt As String = "USDT"

            If symbol.Trim.ToUpper = "USDT" Then
                symbol = "USDC"
            End If

            Dim url = $"https://api.binance.com/api/v3/ticker/price?symbol={symbol.Trim.ToUpper}USDT"
            Dim response = Await client.GetAsync(url)

            If response.IsSuccessStatusCode Then
                Dim content = Await response.Content.ReadAsStringAsync()
                Dim json = JsonDocument.Parse(content)
                Dim priceStr = json.RootElement.GetProperty("price").GetString()
                Dim price As Decimal = Decimal.Parse(priceStr, CultureInfo.InvariantCulture)

                Return $"{price.ToString(CultureInfo.InvariantCulture)}|0"
            Else
                Throw New Exception($"HTTP {CInt(response.StatusCode)} – {response.ReasonPhrase}")
            End If
        End Using
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
                Throw New Exception($"HTTP {CInt(response.StatusCode)} – {response.ReasonPhrase}")
            End If
        End Using
    End Function

End Class
