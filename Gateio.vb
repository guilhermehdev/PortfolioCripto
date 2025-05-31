Imports System.Net.Http
Imports System.Security.Cryptography
Imports System.Text
Imports Newtonsoft.Json.Linq
Imports System.Globalization
Public Class Gateio
    Dim gec As New Coingecko
    'Private Async Function GATE_GetAssetQty(symbol As String) As Task(Of Decimal)
    '    Dim apiKey As String = My.Settings.GateAPIKey
    '    Dim secret As String = My.Settings.GateSecretKey
    '    Dim endpoint = "/api/v4/spot/accounts"
    '    Dim baseUrl = "https://api.gateio.ws"
    '    Dim url = baseUrl & endpoint

    '    Dim timestamp = CInt(DateTimeOffset.UtcNow.ToUnixTimeSeconds()).ToString()
    '    Dim method = "GET"
    '    Dim query = "" ' vazio para este endpoint
    '    Dim body = ""  ' corpo vazio para GET

    '    ' Formato de assinatura Gate.io v4: 
    '    ' StringToSign = "{timestamp}\n{method}\n{request_path}\n{query_string}\n{body}"
    '    Dim stringToSign = $"{timestamp}" & vbLf &
    '                       $"{method}" & vbLf &
    '                       $"{endpoint}" & vbLf &
    '                       $"{query}" & vbLf &
    '                       $"{body}"

    '    Dim encoding = New UTF8Encoding()
    '    Dim secretBytes = encoding.GetBytes(secret)
    '    Dim signBytes = encoding.GetBytes(stringToSign)
    '    Dim hmac = New HMACSHA512(secretBytes)
    '    Dim hash = hmac.ComputeHash(signBytes)
    '    Dim signature = BitConverter.ToString(hash).Replace("-", "").ToLower()

    '    Using client As New HttpClient()
    '        System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12
    '        client.DefaultRequestHeaders.Add("KEY", apiKey)
    '        client.DefaultRequestHeaders.Add("Timestamp", timestamp)
    '        client.DefaultRequestHeaders.Add("SIGN", signature)
    '        client.DefaultRequestHeaders.Add("Content-Type", "application/json")

    '        Dim response = Await client.GetAsync(url)
    '        response.EnsureSuccessStatusCode()

    '        Dim content = Await response.Content.ReadAsStringAsync()
    '        Dim jsonArray = JArray.Parse(content)

    '        For Each item In jsonArray
    '            If item("currency").ToString().ToUpper() = symbol.ToUpper() Then
    '                Dim free = Decimal.Parse(item("available").ToString(), CultureInfo.InvariantCulture)
    '                Dim locked = Decimal.Parse(item("locked").ToString(), CultureInfo.InvariantCulture)
    '                Return free + locked
    '            End If
    '        Next
    '    End Using

    '    Return 0D ' Não encontrado
    'End Function

    'Private Async Function GATE_GetCoinsPrice(symbol As String) As Task(Of Decimal)
    '    Dim pair = $"{symbol.Trim().ToUpper()}_USDT"
    '    Dim url = $"https://api.gate.io/api/v4/spot/tickers?currency_pair={pair}"

    '    Using client As New HttpClient()
    '        System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12
    '        client.DefaultRequestHeaders.Add("User-Agent", "VBApp/1.0")
    '        Dim response = Await client.GetAsync(url)

    '        If Not response.IsSuccessStatusCode Then
    '            Throw New Exception($"Erro HTTP {CInt(response.StatusCode)} – {response.ReasonPhrase}")
    '        End If

    '        Dim json = JArray.Parse(Await response.Content.ReadAsStringAsync())
    '        Dim lastPrice = json(0)("last").ToString()
    '        Return Decimal.Parse(lastPrice, CultureInfo.InvariantCulture)
    '    End Using
    'End Function

    'Public Async Function GATE_GetAssetQty(symbol As String) As Task(Of Decimal)
    '    Dim endpoint = "/api/v4/spot/accounts"
    '    Dim url = "https://api.gateio.ws" & endpoint
    '    Dim timestamp = CInt(DateTimeOffset.UtcNow.ToUnixTimeSeconds()).ToString()
    '    Dim method = "GET"
    '    Dim query = ""
    '    Dim body = ""

    '    Dim stringToSign = $"{timestamp}" & vbLf &
    '                   $"{method}" & vbLf &
    '                   $"{endpoint}" & vbLf &
    '                   $"{query}" & vbLf &
    '                   $"{body}"

    '    Debug.WriteLine($"[GATE.IO] StringToSign: {stringToSign}")

    '    Dim signBytes = New HMACSHA512(Encoding.UTF8.GetBytes(My.Settings.GateSecretKey)).
    '                ComputeHash(Encoding.UTF8.GetBytes(stringToSign))
    '    Dim signature = BitConverter.ToString(signBytes).Replace("-", "").ToLower()

    '    Dim handler As New HttpClientHandler() With {.SslProtocols = Security.Authentication.SslProtocols.Tls12}

    '    Using client As New HttpClient(handler)
    '        client.DefaultRequestHeaders.Add("KEY", My.Settings.GateAPIKey)
    '        client.DefaultRequestHeaders.Add("Timestamp", timestamp)
    '        client.DefaultRequestHeaders.Add("SIGN", signature)
    '        client.DefaultRequestHeaders.Add("User-Agent", "VBApp/1.0")
    '        'client.DefaultRequestHeaders.Add("Content-Type", "application/json")

    '        Dim resp = Await client.GetAsync(url)
    '        resp.EnsureSuccessStatusCode()

    '        Dim json = JArray.Parse(Await resp.Content.ReadAsStringAsync())

    '        For Each bal In json
    '            If bal("currency").ToString().Equals(symbol, StringComparison.OrdinalIgnoreCase) Then
    '                Dim free = Decimal.Parse(bal("available").ToString(), CultureInfo.InvariantCulture)
    '                Dim locked = Decimal.Parse(bal("locked").ToString(), CultureInfo.InvariantCulture)
    '                Return free + locked
    '            End If
    '        Next
    '    End Using

    '    Return 0D  ' não encontrou a moeda
    'End Function

    Public Async Function GATE_GetAssetQty(symbol As String) As Task(Of Decimal)
        Dim endpoint = "/api/v4/spot/accounts"
        Dim url = "https://api.gateio.ws" & endpoint
        Dim method = "GET"
        Dim query = ""
        Dim body = ""
        Dim timestamp = CInt(DateTimeOffset.UtcNow.ToUnixTimeSeconds()).ToString()

        ' 🔑 Gerar hash do corpo vazio
        Dim bodyHash As String
        Using sha512 As New SHA512Managed()
            Dim bodyBytes = Encoding.UTF8.GetBytes(body)
            Dim hashBytes = sha512.ComputeHash(bodyBytes)
            bodyHash = BitConverter.ToString(hashBytes).Replace("-", "").ToLower()
        End Using

        ' 🧾 stringToSign padrão atualizado
        Dim stringToSign = $"{method}" & vbLf &
                       $"{endpoint}" & vbLf &
                       $"{query}" & vbLf &
                       $"{bodyHash}" & vbLf &
                       $"{timestamp}"

        ' 🔏 Gerar assinatura com o segredo em UTF8 puro (sem hex-decode!)
        Dim secretClean = My.Settings.GateSecretKey.Trim()
        Dim signBytes = New HMACSHA512(Encoding.UTF8.GetBytes(secretClean)).
                    ComputeHash(Encoding.UTF8.GetBytes(stringToSign))
        Dim signature = BitConverter.ToString(signBytes).Replace("-", "").ToLower()

        ' 🌐 Enviar requisição
        Dim handler As New HttpClientHandler() With {
        .SslProtocols = Security.Authentication.SslProtocols.Tls12
    }

        Using client As New HttpClient(handler)
            client.DefaultRequestHeaders.Clear()
            client.DefaultRequestHeaders.Add("KEY", My.Settings.GateAPIKey)
            client.DefaultRequestHeaders.Add("Timestamp", timestamp)
            client.DefaultRequestHeaders.Add("SIGN", signature)
            client.DefaultRequestHeaders.Add("User-Agent", "VBApp/1.0")
            client.DefaultRequestHeaders.Accept.Add(New System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"))

            Dim resp = Await client.GetAsync(url)
            Dim json = JArray.Parse(Await resp.Content.ReadAsStringAsync())

            For Each bal In json
                If bal("currency").ToString().Equals(symbol, StringComparison.OrdinalIgnoreCase) Then
                    Dim free = Decimal.Parse(bal("available").ToString(), CultureInfo.InvariantCulture)
                    Dim locked = Decimal.Parse(bal("locked").ToString(), CultureInfo.InvariantCulture)
                    Return free + locked
                End If
            Next
        End Using

        Return 0D ' Se não encontrou
    End Function


    Public Async Function GATE_GetCoinsPrice(symbol As String) As Task(Of Decimal)
        Dim pair = $"{symbol.Trim().ToUpper()}_USDT"
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
            ' 1. Obter o preço atual da moeda
            Dim pair = $"{symbol.Trim().ToUpper()}_USDT"
            Dim url = $"https://api.gateio.ws/api/v4/spot/tickers?currency_pair={pair}"

            Dim priceDecimal As Decimal = 0D
            Using client As New HttpClient()
                Dim resp = Await client.GetAsync(url)
                Dim body = Await resp.Content.ReadAsStringAsync()

                If Not body.Trim().StartsWith("[") Then
                    Dim err = JObject.Parse(body)
                    Throw New Exception($"[GATE.IO] Erro no preço: {err}")
                End If

                Dim json = JArray.Parse(body)
                priceDecimal = Decimal.Parse(json(0)("last").ToString(), CultureInfo.InvariantCulture)
            End Using

            ' 2. Obter saldo da moeda (via API privada)
            Dim qtd As Decimal = Await GATE_GetAssetQty(symbol)

            ' 3. Montar retorno no formato esperado
            Return $"{priceDecimal.ToString(CultureInfo.InvariantCulture)}|0|{qtd.ToString(CultureInfo.InvariantCulture)}"

        Catch ex As Exception
            Debug.WriteLine($"[GATE.IO] Erro geral em GetCoinsInfo({symbol}): {ex.Message}")
            Return "0|0|0"
        End Try
    End Function


    'Public Async Function GATE_GetCoinsInfo(symbol As String) As Task(Of String)
    '    Try
    '        Dim preco As Decimal = Await GATE_GetCoinsPrice(symbol)

    '        Dim qtd As Decimal = Await GATE_GetAssetQty(symbol)
    '        MsgBox(qtd)
    '        Return $"{preco.ToString(CultureInfo.InvariantCulture)}|0|{qtd.ToString(CultureInfo.InvariantCulture)}"

    '    Catch ex As Exception
    '        Debug.WriteLine($"[GATE.IO] Erro ao obter info de {symbol}: {ex.Message}")
    '        Return $"0|0|0"
    '    End Try
    'End Function


End Class
