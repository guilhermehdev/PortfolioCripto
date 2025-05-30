Imports System.Net.Http
Imports System.Security.Cryptography
Imports System.Text
Imports Newtonsoft.Json.Linq
Imports System.Globalization
Public Class Gateio
    Private Async Function GATE_GetAssetQty(symbol As String) As Task(Of Decimal)
        Dim apiKey As String = My.Settings.GateAPIKey
        Dim secret As String = My.Settings.GateSecretKey
        Dim endpoint = "/api/v4/spot/accounts"
        Dim baseUrl = "https://api.gate.io"
        Dim url = baseUrl & endpoint

        Dim timestamp = CInt(DateTimeOffset.UtcNow.ToUnixTimeSeconds()).ToString()
        Dim method = "GET"
        Dim query = "" ' vazio para este endpoint
        Dim body = ""  ' corpo vazio para GET

        ' Formato de assinatura Gate.io v4: 
        ' StringToSign = "{timestamp}\n{method}\n{request_path}\n{query_string}\n{body}"
        Dim stringToSign = $"{timestamp}" & vbLf &
                           $"{method}" & vbLf &
                           $"{endpoint}" & vbLf &
                           $"{query}" & vbLf &
                           $"{body}"

        Dim encoding = New UTF8Encoding()
        Dim secretBytes = encoding.GetBytes(secret)
        Dim signBytes = encoding.GetBytes(stringToSign)
        Dim hmac = New HMACSHA512(secretBytes)
        Dim hash = hmac.ComputeHash(signBytes)
        Dim signature = BitConverter.ToString(hash).Replace("-", "").ToLower()

        Using client As New HttpClient()
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12
            client.DefaultRequestHeaders.Add("KEY", apiKey)
            client.DefaultRequestHeaders.Add("Timestamp", timestamp)
            client.DefaultRequestHeaders.Add("SIGN", signature)
            client.DefaultRequestHeaders.Add("Content-Type", "application/json")

            Dim response = Await client.GetAsync(url)
            response.EnsureSuccessStatusCode()

            Dim content = Await response.Content.ReadAsStringAsync()
            Dim jsonArray = JArray.Parse(content)

            For Each item In jsonArray
                If item("currency").ToString().ToUpper() = symbol.ToUpper() Then
                    Dim free = Decimal.Parse(item("available").ToString(), CultureInfo.InvariantCulture)
                    Dim locked = Decimal.Parse(item("locked").ToString(), CultureInfo.InvariantCulture)
                    Return free + locked
                End If
            Next
        End Using

        Return 0D ' Não encontrado
    End Function

    Private Async Function GATE_GetCoinsPrice(symbol As String) As Task(Of Decimal)
        Dim pair = $"{symbol.Trim().ToUpper()}_USDT"
        Dim url = $"https://api.gate.io/api/v4/spot/tickers?currency_pair={pair}"

        Using client As New HttpClient()
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12
            client.DefaultRequestHeaders.Add("User-Agent", "VBApp/1.0")
            Dim response = Await client.GetAsync(url)

            If Not response.IsSuccessStatusCode Then
                Throw New Exception($"Erro HTTP {CInt(response.StatusCode)} – {response.ReasonPhrase}")
            End If

            Dim json = JArray.Parse(Await response.Content.ReadAsStringAsync())
            Dim lastPrice = json(0)("last").ToString()
            Return Decimal.Parse(lastPrice, CultureInfo.InvariantCulture)
        End Using
    End Function

    Public Async Function GATE_GetCoinsInfo(symbol As String) As Task(Of String)
        Try
            Dim preco As Decimal = Await GATE_GetCoinsPrice(symbol)
            Dim qtd As Decimal = Await GATE_GetAssetQty(symbol)

            Return $"{preco.ToString(CultureInfo.InvariantCulture)}|0|{qtd.ToString(CultureInfo.InvariantCulture)}"

        Catch ex As Exception
            Debug.WriteLine($"[GATE.IO] Erro ao obter info de {symbol}: {ex.Message}")
            Return $"0|0|0"
        End Try
    End Function


End Class
