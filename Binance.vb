Imports System.Net.Http
Imports System.Security.Cryptography
Imports System.Text
Imports Newtonsoft.Json.Linq
Imports System.Globalization
Public Class Binance
    Private apiKey As String = My.Settings.BinanceAPIKey
    Private secret As String = My.Settings.BinanceSecretKey
    Function QuerySigned(ByVal path As String, ByVal query As String) As String

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

    Public Function getMyAccount()
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
    Public Function getCoinsPrice(ByVal pair As String) As String
        Try
            Using client As New HttpClient()
                Dim url = $"https://api.binance.com/api/v3/ticker/price?symbol={pair}"
                Dim response = client.GetAsync(url).Result

                ' Verifica se a resposta HTTP foi bem-sucedida (status 200)
                If response.IsSuccessStatusCode Then
                    Dim price = response.Content.ReadAsStringAsync().Result
                    Return price
                Else
                    Debug.WriteLine($"Erro HTTP: {CInt(response.StatusCode)} - {response.ReasonPhrase}")
                    Return False
                End If
            End Using
        Catch ex As Exception
            Debug.WriteLine($"Erro de exceção: {ex.Message}")
            Return False
        End Try
    End Function


End Class
