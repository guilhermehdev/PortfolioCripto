Imports System.Globalization
Imports System.IO
Imports System.Net.Http
Imports System.Security.Cryptography
Imports System.Security.Principal
Imports System.Text
Imports System.Text.Json
Imports Newtonsoft.Json.Linq
Public Class Binance
    Private BinanceTimeOffset As Long = 0

    Public Async Function SyncBinanceTime() As Task

        Using cli As New HttpClient()

            Dim json As String =
            Await cli.GetStringAsync(
                "https://api.binance.com/api/v3/time"
            )

            Dim obj As JObject = JObject.Parse(json)

            Dim serverTime As Long = CLng(obj("serverTime"))
            Dim localTime As Long =
            DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()

            BinanceTimeOffset = serverTime - localTime

        End Using

    End Function

    Private Async Function BINANCE_GetSpotAccountAsync() As Task(Of JObject)

        Dim json = Await QuerySignedAsync("/api/v3/account", "recvWindow=5000")

        If String.IsNullOrWhiteSpace(json) Then
            Return Nothing
        End If

        Dim account As JObject = JObject.Parse(json)

        If account("code") IsNot Nothing Then

            Throw New Exception(
            $"Binance ({account("code")}): {account("msg")}"
        )

        End If

        Return account

    End Function

    Private Async Function BINANCE_GetFuturesAccountAsync() As Task(Of JObject)

        Dim json =
        Await QuerySignedAsync(
            "/fapi/v2/account",
            "recvWindow=5000",
            isFutures:=True)

        If String.IsNullOrWhiteSpace(json) Then
            Return Nothing
        End If

        Dim account As JObject = JObject.Parse(json)

        If account("code") IsNot Nothing Then

            Throw New Exception(
            $"Binance ({account("code")}): {account("msg")}"
        )

        End If

        Return account

    End Function

    Private Function GetFuturesAssets(account As JObject) As Dictionary(Of String, (Saldo As Decimal, Ordem As Decimal, Lucro As Decimal))

        Dim ativos As New Dictionary(Of String, (Saldo As Decimal, Ordem As Decimal, Lucro As Decimal))(StringComparer.OrdinalIgnoreCase)

        For Each asset In account("assets")

            Dim symbol = asset("asset").ToString()

            Dim saldo =
            Decimal.Parse(
                asset("walletBalance").ToString(),
                CultureInfo.InvariantCulture)

            Dim ordem =
            Decimal.Parse(
                asset("openOrderInitialMargin").ToString(),
                CultureInfo.InvariantCulture)

            If saldo > 0D OrElse ordem > 0D Then

                ativos(symbol) = (
                Saldo:=saldo,
                Ordem:=ordem,
                Lucro:=0D)

            End If

        Next

        For Each pos In account("positions")

            Dim contrato =
            pos("symbol").ToString()

            Dim lucro =
            Decimal.Parse(
                pos("unrealizedProfit").ToString(),
                CultureInfo.InvariantCulture)

            If lucro <> 0D Then

                If ativos.ContainsKey(contrato) Then

                    Dim atual = ativos(contrato)

                    ativos(contrato) = (
                    atual.Saldo,
                    atual.Ordem,
                    atual.Lucro + lucro)

                Else

                    ativos(contrato) = (
                    Saldo:=0D,
                    Ordem:=0D,
                    Lucro:=lucro)

                End If

            End If

        Next

        Return ativos

    End Function
    'Private Function QuerySigned(ByVal path As String, ByVal query As String, Optional isFutures As Boolean = False) As String
    '    Dim apiKey As String = My.Settings.BinanceAPIKey
    '    Dim secret As String = My.Settings.BinanceSecretKey

    '    Try

    '        Dim baseUrl = If(isFutures, "https://fapi.binance.com", "https://api.binance.com")
    '        ' Dim qs = query & "&timestamp=" & (DateTimeOffset.UtcNow.ToUnixTimeMilliseconds())
    '        Dim timestamp As Long = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + BinanceTimeOffset
    '        Dim qs = query & "&timestamp=" & timestamp

    '        Using hmac As New HMACSHA256(Encoding.UTF8.GetBytes(secret))
    '            Dim sigBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(qs))
    '            Dim signature = BitConverter.ToString(sigBytes).Replace("-", "").ToLower()
    '            qs &= "&signature=" & signature
    '        End Using

    '        Using cli As New HttpClient()
    '            cli.BaseAddress = New Uri(baseUrl)
    '            cli.DefaultRequestHeaders.Add("X-MBX-APIKEY", apiKey)
    '            Dim resp = cli.GetAsync(path & "?" & qs).Result
    '            Return resp.Content.ReadAsStringAsync().Result
    '        End Using

    '    Catch ex As Exception
    '        FormMain.lbDebug.Clear()
    '        FormMain.lbDebug.AppendText("Erro na assinatura da API Binance: " & ex.Message)
    '        Debug.WriteLine("Erro na assinatura da API Binance: " & ex.Message)
    '        Return False
    '    End Try

    'End Function

    Private Async Function QuerySignedAsync(ByVal path As String, ByVal query As String, Optional isFutures As Boolean = False, Optional retry As Integer = 0) As Task(Of String)

        Dim apiKey As String = My.Settings.BinanceAPIKey
        Dim secret As String = My.Settings.BinanceSecretKey

        Try

            Dim baseUrl As String = If(isFutures, "https://fapi.binance.com", "https://api.binance.com")
            Dim timestamp As Long = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + BinanceTimeOffset
            Dim qs As String = $"{query}&timestamp={timestamp}"

            Using hmac As New HMACSHA256(Encoding.UTF8.GetBytes(secret))
                Dim sigBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(qs))
                Dim signature = BitConverter.ToString(sigBytes).Replace("-", "").ToLowerInvariant()

                qs &= "&signature=" & signature

            End Using

            Using cli As New HttpClient()

                cli.BaseAddress = New Uri(baseUrl)
                cli.DefaultRequestHeaders.Add("X-MBX-APIKEY", apiKey)

                Dim resp = Await cli.GetAsync(path & "?" & qs)
                Dim json = Await resp.Content.ReadAsStringAsync()
                Dim obj As JObject = Nothing

                Try
                    obj = JObject.Parse(json)
                Catch
                End Try

                If obj IsNot Nothing AndAlso
               obj("code") IsNot Nothing AndAlso
               CLng(obj("code")) = -1021 AndAlso
               retry < 1 Then

                    Await SyncBinanceTime()
                    Return Await QuerySignedAsync(path, query, isFutures, retry + 1)
                End If

                Return json

            End Using

        Catch ex As Exception

            FormMain.lbDebug.Clear()
            FormMain.lbDebug.AppendText("Erro na assinatura da API Binance: " & ex.Message)
            Debug.WriteLine(ex.ToString())
            Return String.Empty

        End Try

    End Function

    'Private Function BINANCE_GetSpotAssets() As Task(Of Dictionary(Of String, Decimal))
    '    Dim json = QuerySigned("/api/v3/account", "recvWindow=5000")
    '    Dim account = JObject.Parse(json)

    '    Dim ativos As New Dictionary(Of String, Decimal)(StringComparer.OrdinalIgnoreCase)

    '    If account("code") IsNot Nothing Then
    '        Throw New Exception($"Binance ({account("code")}): {account("msg")}")
    '        Exit Function
    '    End If

    '    Try


    '        For Each bal In account("balances")
    '            Dim asset As String = bal("asset").ToString()
    '            Dim free = Decimal.Parse(bal("free").ToString(), CultureInfo.InvariantCulture)
    '            Dim locked = Decimal.Parse(bal("locked").ToString(), CultureInfo.InvariantCulture)
    '            Dim qtd As Decimal = free + locked

    '            If qtd > 0D Then
    '                ativos(asset) = qtd
    '            End If
    '        Next

    '        Return Task.FromResult(ativos)

    '    Catch ex As Exception
    '        FormMain.lbDebug.Clear()
    '        FormMain.lbDebug.AppendText("Erro ao trazer dados da conta Spot: " & ex.Message)
    '        Debug.WriteLine("Erro ao trazer dados da conta Spot: " & ex.Message)
    '        Return Task.FromResult(New Dictionary(Of String, Decimal)) ' vazio
    '    End Try
    'End Function

    Private Async Function BINANCE_GetSpotAssets() As Task(Of Dictionary(Of String, Decimal))

        Dim json = Await QuerySignedAsync("/api/v3/account", "recvWindow=5000")

        If String.IsNullOrWhiteSpace(json) Then
            Return New Dictionary(Of String, Decimal)
        End If

        Dim account As JObject = JObject.Parse(json)

        If account("code") IsNot Nothing Then

            Throw New Exception(
            $"Binance ({account("code")}): {account("msg")}"
        )

        End If

        Dim ativos As New Dictionary(Of String, Decimal)(StringComparer.OrdinalIgnoreCase)

        For Each bal In account("balances")

            Dim asset As String =
            bal("asset").ToString()

            Dim free As Decimal = Decimal.Parse(bal("free").ToString(), CultureInfo.InvariantCulture)
            Dim locked As Decimal = Decimal.Parse(bal("locked").ToString(), CultureInfo.InvariantCulture)
            Dim qtd As Decimal = free + locked

            If qtd > 0D Then
                ativos(asset) = qtd
            End If

        Next

        Return ativos

    End Function

    Private Function GetSpotAssets(account As JObject) As Dictionary(Of String, Decimal)

        Dim ativos As New Dictionary(Of String, Decimal)(
        StringComparer.OrdinalIgnoreCase)

        For Each bal In account("balances")

            Dim asset As String = bal("asset").ToString()

            Dim free As Decimal =
            Decimal.Parse(
                bal("free").ToString(),
                CultureInfo.InvariantCulture)

            Dim locked As Decimal =
            Decimal.Parse(
                bal("locked").ToString(),
                CultureInfo.InvariantCulture)

            Dim qtd As Decimal = free + locked

            If qtd > 0D Then
                ativos(asset) = qtd
            End If

        Next

        Return ativos

    End Function

    Private Function GetAssetQty(account As JObject, asset As String) As Decimal

        For Each bal In account("balances")

            If bal("asset").ToString().
            Equals(asset, StringComparison.OrdinalIgnoreCase) Then

                Dim free As Decimal =
                Decimal.Parse(
                    bal("free").ToString(),
                    CultureInfo.InvariantCulture)

                Dim locked As Decimal =
                Decimal.Parse(
                    bal("locked").ToString(),
                    CultureInfo.InvariantCulture)

                Return free + locked

            End If

        Next

        Return 0D

    End Function

    'Private Function BINANCE_GetFuturesAssets() As Task(Of Dictionary(Of String, (Saldo As Decimal, Ordem As Decimal, Lucro As Decimal)))
    '    Dim json = QuerySigned("/fapi/v2/account", "recvWindow=5000", isFutures:=True)
    '    Dim account = JObject.Parse(json)

    '    ' Usaremos dicionário por símbolo de contrato (ex: BTCUSDT)
    '    Dim ativos As New Dictionary(Of String, (Saldo As Decimal, Ordem As Decimal, Lucro As Decimal))(StringComparer.OrdinalIgnoreCase)

    '    If account("code") IsNot Nothing Then
    '        Throw New Exception($"Binance ({account("code")}): {account("msg")}")
    '        Exit Function
    '    End If

    '    Try

    '        For Each asset In account("assets")
    '            Dim symbol = asset("asset").ToString()
    '            Dim saldo = Decimal.Parse(asset("walletBalance").ToString(), CultureInfo.InvariantCulture)
    '            Dim ordem = Decimal.Parse(asset("openOrderInitialMargin").ToString(), CultureInfo.InvariantCulture)

    '            ' Inicializa com lucro 0, será somado depois com base nas posições
    '            If saldo > 0 Or ordem > 0 Then
    '                ativos(symbol) = (Saldo:=saldo, Ordem:=ordem, Lucro:=0)
    '            End If
    '        Next

    '        ' 2. Pega lucro/prejuízo não realizado por contrato (BTCUSDT, ETHUSDT, etc.)
    '        For Each pos In account("positions")
    '            Dim contrato = pos("symbol").ToString()
    '            Dim lucro = Decimal.Parse(pos("unrealizedProfit").ToString(), CultureInfo.InvariantCulture)

    '            If lucro <> 0 Then
    '                If ativos.ContainsKey(contrato) Then
    '                    Dim atual = ativos(contrato)
    '                    ativos(contrato) = (atual.Saldo, atual.Ordem, atual.Lucro + lucro)
    '                Else
    '                    ativos(contrato) = (Saldo:=0, Ordem:=0, Lucro:=lucro)
    '                End If
    '            End If
    '        Next

    '        Return Task.FromResult(ativos)

    '    Catch ex As Exception
    '        FormMain.lbDebug.Clear()
    '        FormMain.lbDebug.AppendText("Erro ao trazer dados da conta de Futuros: " & ex.Message)
    '        Return Task.FromResult(New Dictionary(Of String, (Decimal, Decimal, Decimal)))
    '    End Try
    'End Function

    Private Async Function BINANCE_GetFuturesAssets() As Task(Of Dictionary(Of String, (Saldo As Decimal, Ordem As Decimal, Lucro As Decimal)))

        Dim json = Await QuerySignedAsync("/fapi/v2/account", "recvWindow=5000", isFutures:=True)

        If String.IsNullOrWhiteSpace(json) Then
            Return New Dictionary(Of String, (Decimal, Decimal, Decimal))
        End If

        Dim account As JObject = JObject.Parse(json)

        If account("code") IsNot Nothing Then
            Throw New Exception(
            $"Binance ({account("code")}): {account("msg")}"
        )
        End If

        Dim ativos As New Dictionary(Of String, (Saldo As Decimal, Ordem As Decimal, Lucro As Decimal))(
        StringComparer.OrdinalIgnoreCase)

        Try

            For Each asset In account("assets")

                Dim symbol As String =
                asset("asset").ToString()

                Dim saldo As Decimal =
                Decimal.Parse(
                    asset("walletBalance").ToString(),
                    CultureInfo.InvariantCulture)

                Dim ordem As Decimal =
                Decimal.Parse(
                    asset("openOrderInitialMargin").ToString(),
                    CultureInfo.InvariantCulture)

                If saldo > 0D OrElse ordem > 0D Then
                    ativos(symbol) = (
                    Saldo:=saldo,
                    Ordem:=ordem,
                    Lucro:=0D)
                End If

            Next

            For Each pos In account("positions")

                Dim contrato As String =
                pos("symbol").ToString()

                Dim lucro As Decimal =
                Decimal.Parse(
                    pos("unrealizedProfit").ToString(),
                    CultureInfo.InvariantCulture)

                If lucro <> 0D Then

                    If ativos.ContainsKey(contrato) Then

                        Dim atual = ativos(contrato)

                        ativos(contrato) = (
                        atual.Saldo,
                        atual.Ordem,
                        atual.Lucro + lucro)

                    Else

                        ativos(contrato) = (
                        Saldo:=0D,
                        Ordem:=0D,
                        Lucro:=lucro)

                    End If

                End If

            Next

            Return ativos

        Catch ex As Exception

            FormMain.lbDebug.Clear()
            FormMain.lbDebug.AppendText(
            "Erro ao trazer dados da conta de Futuros: " &
            ex.Message)

            Return New Dictionary(Of String, (Decimal, Decimal, Decimal))

        End Try

    End Function


    'Private Function BINANCE_GetAssetQty(asset As String) As Decimal
    '    Dim json = QuerySigned("/api/v3/account", "recvWindow=5000")
    '    Dim account = JObject.Parse(json)

    '    Try

    '        For Each bal In account("balances")
    '            If bal("asset").ToString().Equals(asset, StringComparison.OrdinalIgnoreCase) Then
    '                Dim free = Decimal.Parse(bal("free").ToString(), CultureInfo.InvariantCulture)
    '                Dim locked = Decimal.Parse(bal("locked").ToString(), CultureInfo.InvariantCulture)
    '                Return free + locked
    '            End If
    '        Next

    '        Return 0D

    '    Catch ex As Exception
    '        FormMain.lbDebug.AppendText(ex.Message)
    '        Return False
    '    End Try

    'End Function

    Private Async Function BINANCE_GetAssetQty(asset As String) As Task(Of Decimal)

        Dim json = Await QuerySignedAsync("/api/v3/account", "recvWindow=5000")

        If String.IsNullOrWhiteSpace(json) Then
            Return 0D
        End If

        Dim account As JObject = JObject.Parse(json)

        If account("code") IsNot Nothing Then

            Throw New Exception(
            $"Binance ({account("code")}): {account("msg")}"
        )

        End If

        Try

            For Each bal In account("balances")

                If bal("asset").ToString().
                Equals(asset, StringComparison.OrdinalIgnoreCase) Then

                    Dim free As Decimal =
                    Decimal.Parse(
                        bal("free").ToString(),
                        CultureInfo.InvariantCulture)

                    Dim locked As Decimal =
                    Decimal.Parse(
                        bal("locked").ToString(),
                        CultureInfo.InvariantCulture)

                    Return free + locked

                End If

            Next

            Return 0D

        Catch ex As Exception

            FormMain.lbDebug.AppendText(ex.Message)

            Return 0D

        End Try

    End Function
    'Public Async Function BINANCE_GetAllAssetsFull() As Task(Of Dictionary(Of String, Decimal))
    '    Dim spotAssets = Await BINANCE_GetSpotAssets()
    '    Dim futuresAssets = Await BINANCE_GetFuturesAssets()

    '    ' Junta os dois dicionários
    '    Dim allAccounts As New Dictionary(Of String, Decimal)(StringComparer.OrdinalIgnoreCase)

    '    For Each kv In spotAssets
    '        allAccounts(kv.Key) = kv.Value
    '    Next

    '    For Each kv In futuresAssets
    '        If allAccounts.ContainsKey(kv.Key) Then
    '            allAccounts(kv.Key) += kv.Value.Saldo
    '        Else
    '            allAccounts(kv.Key) = kv.Value.Saldo
    '        End If
    '    Next

    '    Return allAccounts

    'End Function

    Public Async Function BINANCE_GetAllAssetsFull() As Task(Of Dictionary(Of String, Decimal))

        Dim spotTask = BINANCE_GetSpotAccountAsync()
        Dim futuresTask = BINANCE_GetFuturesAccountAsync()

        Await Task.WhenAll(spotTask, futuresTask)

        Dim spotAccount = Await spotTask
        Dim futuresAccount = Await futuresTask

        Dim spotAssets = GetSpotAssets(spotAccount)
        Dim futuresAssets = GetFuturesAssets(futuresAccount)

        Dim allAccounts As New Dictionary(Of String, Decimal)(
        StringComparer.OrdinalIgnoreCase)

        For Each kv In spotAssets
            allAccounts(kv.Key) = kv.Value
        Next

        For Each kv In futuresAssets

            If allAccounts.ContainsKey(kv.Key) Then
                allAccounts(kv.Key) += kv.Value.Saldo
            Else
                allAccounts(kv.Key) = kv.Value.Saldo
            End If

        Next

        Return allAccounts

    End Function

    Public Async Function BINANCE_GetCoinsInfo(Optional symbol As String = "") As Task(Of Object)
        Dim account = Await Task.Run(Function() BINANCE_GetAllAssetsFull())
        Dim urlBase As String = "https://api.binance.com/api/v3/ticker/price?symbol="

        Using client As New HttpClient()
            client.DefaultRequestHeaders.Add("User-Agent", "VBApp/1.0")

            If String.IsNullOrWhiteSpace(symbol) Then
                Dim result As New List(Of String)

                For Each cripto In account

                    Dim originalSymbol = cripto.Key.Trim().ToUpper()
                    Dim qtd = cripto.Value
                    Dim pairSymbol

                    If originalSymbol = "USDT" Then
                        pairSymbol = "USDC"
                    Else
                        pairSymbol = originalSymbol
                    End If

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

                        Dim totalUSD = price * qtd
                        If totalUSD >= 1 Then ' <<<<< FILTRO AQUI
                            result.Add($"{originalSymbol}|{price.ToString(CultureInfo.InvariantCulture)}|{qtd.ToString(CultureInfo.InvariantCulture)}")

                            'MsgBox($"simbolo:{originalSymbol} Preço:{price.ToString(CultureInfo.InvariantCulture)} Qtd:{qtd.ToString(CultureInfo.InvariantCulture)}")
                        End If

                    Catch ex As Exception
                        FormMain.lbDebug.Clear()
                        Debug.WriteLine($"Erro ao buscar {pair}: {ex.Message}")
                        FormMain.lbDebug.AppendText($"Erro ao buscar {pair}: {ex.Message}")
                        Continue For
                    End Try
                Next

                Return result

            Else
                ' Caso symbol específico
                Dim originalSymbol = symbol.Trim().ToUpper()
                Dim qtd As Decimal = Await BINANCE_GetAssetQty(originalSymbol)
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
                FormMain.lbDebug.AppendText($"HTTP {CInt(response.StatusCode)} – {response.ReasonPhrase}")
                Return False
            End If
        End Using

    End Function

    Public Async Function ParExisteNaBinance(symbol As String) As Task(Of Boolean)
        Try
            Dim url As String = $"https://api.binance.com/api/v3/exchangeInfo?symbol={symbol}USDT"
            Using client As New Net.Http.HttpClient()
                Dim response As Net.Http.HttpResponseMessage = Await client.GetAsync(url)
                Return response.IsSuccessStatusCode
            End Using
        Catch
            Return False
        End Try
    End Function
    Public Async Function compare() As Task
        Dim j As New JSON
        ' Obtém as informações da Binance
        Dim binanceResult = Await BINANCE_GetCoinsInfo()

        ' Lê e parseia o arquivo JSON
        Dim jsonTexto As String = File.ReadAllText(j.portfolioPathFile)
        Dim jsonObj = JObject.Parse(jsonTexto)

        ' Monta o conjunto de símbolos existentes no JSON
        Dim jsonSymbols As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)

        For Each prop In jsonObj.Properties()
            If prop.Value.Type = JTokenType.Array Then
                Dim arr = CType(prop.Value, JArray)
                For Each item In arr
                    Dim symbol = item("Symbol")?.ToString()?.Trim()?.ToUpper()
                    If Not String.IsNullOrEmpty(symbol) Then
                        jsonSymbols.Add(symbol)
                    End If
                Next
            End If
        Next

        ' Percorre os dados vindos da Binance
        For Each linha As String In DirectCast(binanceResult, List(Of String))
            Dim parts = linha.Split("|"c)
            If parts.Length < 3 Then Continue For ' Garante que há dados suficientes

            Dim symbol = parts(0).Trim().ToUpper()
            Dim qtd As Decimal = j.decimalBR(parts(2))

            ' MsgBox($"Cripto: {symbol} - Quantidade: {qtd}")

            If Not jsonSymbols.Contains(symbol) Then
                MsgBox($"Cripto {symbol} não encontrada no arquivo local. Adicionando...")
                Dim precoMedioStr = InputBox($"Digite o preço de entrada/médio para {symbol}", "Nova Cripto")

                Dim precoMedio As Decimal
                If Decimal.TryParse(precoMedioStr.Replace(",", "."), Globalization.NumberStyles.Any, Globalization.CultureInfo.InvariantCulture, precoMedio) Then
                    Await j.saveAportToJSONBin(symbol, precoMedio, qtd, Date.Today, "BINANCE", symbol)
                Else
                    FormMain.lbDebug.Clear()
                    FormMain.lbDebug.AppendText("Preço inválido. Pulando " & symbol)
                End If
            End If
        Next
    End Function

End Class
