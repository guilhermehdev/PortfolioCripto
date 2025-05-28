Imports System.Globalization
Imports System.Net.Http
Imports System.Security.Cryptography
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
        ' Usa sua QuerySigned já pronta (timestamp e assinatura dentro dela)
        Dim json = QuerySigned("/api/v3/account", "recvWindow=5000")
        Dim account = JObject.Parse(json)

        For Each bal In account("balances")
            If bal("asset").ToString().Equals(asset, StringComparison.OrdinalIgnoreCase) Then
                Dim free = Decimal.Parse(bal("free").ToString(), CultureInfo.InvariantCulture)
                Dim locked = Decimal.Parse(bal("locked").ToString(), CultureInfo.InvariantCulture)
                Return free + locked
            End If
        Next

        Return 0D     'se não achar, volta zero
    End Function

    Public Async Function BINANCE_GetCoinsPrice(symbol As String) As Task(Of String)
        Using client As New HttpClient()
            Dim originalSymbol As String = symbol.Trim().ToUpper() ' <-- mantemos o símbolo original
            Dim pairSymbol As String = originalSymbol

            If originalSymbol = "USDT" Then
                pairSymbol = "USDC"
            End If

            Dim pair As String = $"{pairSymbol}USDT"
            Dim url As String = $"https://api.binance.com/api/v3/ticker/price?symbol={pair}"

            Dim resp = Await client.GetAsync(url)
            If Not resp.IsSuccessStatusCode Then
                Throw New Exception($"HTTP {CInt(resp.StatusCode)} – {resp.ReasonPhrase}")
            End If

            Dim content = Await resp.Content.ReadAsStringAsync()
            Dim j = JsonDocument.Parse(content)
            Dim priceStr = j.RootElement.GetProperty("price").GetString()
            Dim price = Decimal.Parse(priceStr, CultureInfo.InvariantCulture)

            ' Se estamos usando USDCUSDT, inverter o valor (USDT → USD)
            If originalSymbol = "USDT" Then
                price = 1 / price
            End If

            Dim qtd As Decimal = BINANCE_GetAssetQty(originalSymbol)  ' <-- usa o símbolo correto para saldo

            Return $"{price.ToString(CultureInfo.InvariantCulture)}|0|{qtd.ToString(CultureInfo.InvariantCulture)}"
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
                Debug.Write($"HTTP {CInt(response.StatusCode)} – {response.ReasonPhrase}")
                Return False
            End If
        End Using

    End Function

    Public Function BINANCE_GetPreciseAverageBuyPrice(symbol As String) As Decimal

        Try
            ' Ex: BTCUSDT → asset = BTC
            Dim asset As String = symbol.Replace("USDT", "").ToUpper()

            ' Pega o saldo atual da moeda
            Dim saldoAtual As Decimal = BINANCE_GetAssetQty(asset)
            If saldoAtual = 0D Then Return 0D


            ' Busca os trades da Binance
            Dim tradesJson As String = QuerySigned("/api/v3/myTrades", $"symbol={symbol.ToUpper()}&limit=1000")
            If tradesJson.Trim().StartsWith("{") Then
                Dim errObj = JObject.Parse(tradesJson)
                Throw New Exception("Erro ao buscar trades: " & errObj.ToString())
            End If

            Dim trades As JArray = JArray.Parse(tradesJson)

            ' Filtra apenas compras e ordena por data (FIFO)
            Dim compras = trades.Where(Function(t) t("isBuyer").Value(Of Boolean)) _
                                .OrderBy(Function(t) CLng(t("time"))).ToList()

            Dim restante As Decimal = saldoAtual
            Dim totalGasto As Decimal = 0D

            For Each c In compras
                Dim qty = Decimal.Parse(c("qty").ToString(), CultureInfo.InvariantCulture)
                Dim price = Decimal.Parse(c("price").ToString(), CultureInfo.InvariantCulture)

                If restante <= 0 Then Exit For

                Dim usada = Math.Min(qty, restante)
                totalGasto += usada * price
                restante -= usada
            Next

            If saldoAtual = 0D Then Return 0D

            MsgBox(totalGasto & " " & saldoAtual)

            Return totalGasto / saldoAtual

        Catch ex As Exception
            Debug.Write($"Erro ao calcular o preço médio de compra: {ex.Message}")
            Return False
        End Try
    End Function


End Class
