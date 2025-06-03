Imports System.Net.Http
Imports System.Security.Cryptography
Imports System.Text
Imports Newtonsoft.Json.Linq
Imports System.Globalization
Public Class Gateio
    Dim gec As New Coingecko

    Public Async Function GATE_GetAssetQty(symbol As String) As Task(Of Decimal)
        Dim endpoint = "/api/v4/spot/accounts"
        Dim url = "https://api.gateio.ws" & endpoint
        Dim method = "GET"
        Dim query = ""
        Dim body = ""
        Dim timestamp = CInt(DateTimeOffset.UtcNow.ToUnixTimeSeconds()).ToString()

        ' hash do corpo (mesmo vazio)
        Dim bodyHash As String
        Using sha512 As New SHA512Managed()
            bodyHash = BitConverter.ToString(
                       sha512.ComputeHash(Encoding.UTF8.GetBytes(body))
                   ).Replace("-", "").ToLower()
        End Using

        ' stringToSign 100 % doc-Gate
        Dim stringToSign = $"{method}" & vbLf &
                       $"{endpoint}" & vbLf &
                       $"{query}" & vbLf &
                       $"{bodyHash}" & vbLf &
                       $"{timestamp}"

        Dim secret = My.Settings.GateSecretKey.Trim()
        Dim signBytes = New HMACSHA512(Encoding.UTF8.GetBytes(secret)).
                    ComputeHash(Encoding.UTF8.GetBytes(stringToSign))
        Dim signature = BitConverter.ToString(signBytes).Replace("-", "").ToLower()

        Dim handler As New HttpClientHandler() With {
        .SslProtocols = Security.Authentication.SslProtocols.Tls12
    }

        Using client As New HttpClient(handler)
            client.DefaultRequestHeaders.Add("KEY", My.Settings.GateAPIKey)
            client.DefaultRequestHeaders.Add("Timestamp", timestamp)
            client.DefaultRequestHeaders.Add("SIGN", signature)
            client.DefaultRequestHeaders.Add("User-Agent", "VBApp/1.0")
            client.DefaultRequestHeaders.Accept.Add(
            New Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"))

            Dim resp = Await client.GetAsync(url)
            Dim raw = Await resp.Content.ReadAsStringAsync()

            '– se a chamada falhou, mostre o erro detalhado
            If Not resp.IsSuccessStatusCode Then
                Dim err = JObject.Parse(raw)
                Throw New Exception($"Gate.io {CInt(resp.StatusCode)} – {err("label")}: {err("message")}")
            End If

            ' resposta OK – agora sim é um array
            Dim balances = JArray.Parse(raw)
            For Each bal In balances
                If bal("currency").ToString().Equals(symbol, StringComparison.OrdinalIgnoreCase) Then
                    Dim free = Decimal.Parse(bal("available").ToString(), CultureInfo.InvariantCulture)
                    Dim locked = Decimal.Parse(bal("locked").ToString(), CultureInfo.InvariantCulture)
                    Return free + locked
                End If
            Next
        End Using

        Return 0D ' moeda não encontrada
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

            Dim priceDecimal As Decimal = Await GATE_GetCoinsPrice(symbol)
            Dim qtd As Decimal = Await GATE_GetAssetQty(symbol)

            MsgBox(qtd)

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
