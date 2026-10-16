Imports System.Data
Imports System.Globalization
Imports System.Net.Http

Public NotInheritable Class PortfolioMarketService

    Private Sub New()
    End Sub

    Public Shared Async Function LoadAsync(
        datagrid As DataGridView,
        Optional currencyColumn As String = "USD") As Task(Of Boolean)

        Try
            PortfolioRepository.Initialize()

            Dim originalDT As DataTable = PortfolioRepository.GetAll()

            If originalDT.Rows.Count = 0 Then
                Throw New Exception("Nenhum ativo encontrado no SQLite.")
            End If

            Dim allSymbols As List(Of String) =
                originalDT.AsEnumerable().
                Select(Function(r) r("Symbol").ToString().Trim().ToUpperInvariant()).
                Where(Function(s) Not String.IsNullOrWhiteSpace(s)).
                Distinct().
                ToList()

            Dim b As New Binance
            Dim gate As New Gateio
            Dim gec As New Coingecko
            Dim formatter As New JSON

            Await b.SyncBinanceTime()

            ' O portfólio Spot deve usar somente a conta Spot. O saldo da
            ' carteira Futures é tratado separadamente no dgFuturos.
            Dim binanceAssets = Await b.BINANCE_GetSpotAssetsFull()
            Dim gateAssets = Await gate.GATE_GetAllSpotAssets()

            ' Procura moedas existentes nas corretoras mas ainda não cadastradas
            ' no portfólio e permite adicioná-las ao SQLite.
            Await PortfolioBalanceSync.SyncAsync(
                b,
                gate,
                binanceAssets,
                gateAssets)

            ' Recarrega o SQLite para incluir imediatamente as moedas adicionadas.
            originalDT = PortfolioRepository.GetAll()

            If originalDT.Rows.Count = 0 Then
                Throw New Exception("Nenhum ativo encontrado no SQLite.")
            End If

            allSymbols =
                originalDT.AsEnumerable().
                Select(Function(r) r("Symbol").ToString().Trim().ToUpperInvariant()).
                Where(Function(s) Not String.IsNullOrWhiteSpace(s)).
                Distinct().
                ToList()

            Dim portfolioSymbols As New HashSet(Of String)(
                allSymbols,
                StringComparer.OrdinalIgnoreCase)

            ' Mantém somente os saldos da Gate que pertencem ao portfólio.
            gateAssets =
                gateAssets.
                Where(Function(kvp) portfolioSymbols.Contains(kvp.Key)).
                ToDictionary(
                    Function(kvp) kvp.Key,
                    Function(kvp) kvp.Value,
                    StringComparer.OrdinalIgnoreCase)

            Debug.WriteLine(
                $"[GATE.IO] Saldos considerados no portfólio: {gateAssets.Count}")
            Dim cmc As New Cotacao()
            Dim mcapDict As New Dictionary(Of String, CoinMarketData)(StringComparer.OrdinalIgnoreCase)

            Dim useMarketFallback = False

            Try
                mcapDict = Await gec.CGECKO_MarketData(allSymbols)
            Catch ex As Exception
                Debug.WriteLine("[MARKET] CoinGecko indisponível: " & ex.Message)
                useMarketFallback = True
            End Try

            If useMarketFallback Then
                mcapDict = Await LoadFallbackMarketDataAsync(b, allSymbols, cmc)
            End If

            Dim usdBrl As Decimal = 0D
            Try
                usdBrl = Await gec.CGECKO_GetPrice("USDT", "brl")
            Catch ex As Exception
                Debug.WriteLine("[MARKET] CoinGecko USDT/BRL indisponível: " & ex.Message)
            End Try

            If usdBrl <= 0D Then
                Try
                    usdBrl = Await b.BINANCE_GetUSDTBRL()
                Catch ex As Exception
                    Debug.WriteLine("[MARKET] Binance USDT/BRL indisponível: " & ex.Message)
                End Try
            End If

            If usdBrl <= 0D AndAlso
               Not String.IsNullOrWhiteSpace(My.Settings.apiCMCKey) AndAlso
               Not String.IsNullOrWhiteSpace(My.Settings.activeAPI) Then
                usdBrl = Await cmc.CM_GetUSDBRL()
            End If

            If usdBrl <= 0D Then
                Throw New Exception(
                    "Cotação USDT/BRL retornou zero. Não foi possível atualizar os valores em BRL.")
            End If

            JSON.USDBRLprice = usdBrl
            formatter.USDBRLprice = usdBrl

            Dim dom As Decimal = Await gec.CGECKO_GetBTCDominance()
            FormMain.lbDom.Text =
                If(dom > 0D, $"{dom:F2}%", "--")

            ' Valor inicial do BTC. Depois o WebSocket assume o realtime.
            Dim btcPriceString As String =
                Await b.BINANCE_GetCoinsInfo("BTC")

            Dim btcPrice As Decimal = 0D
            Dim btcParts() As String =
                btcPriceString.Split("|"c)

            If btcParts.Length > 0 Then
                btcPrice = formatter.decimalBR(btcParts(0))
            End If

            Dim profit As Decimal = 0D
            Dim initialValue As Decimal = 0D
            Dim currValueTotal As Decimal = 0D
            Dim cashflow As Decimal = 0D

            Dim criptoDic As New Dictionary(Of String, Decimal)
            Dim addressDic As New Dictionary(Of String, Decimal)
            Dim listAddress As New List(Of String)
            Dim listCriptos As New List(Of String)
            Dim listCurrValue As New List(Of Decimal)

            Dim newDT As New DataTable()
            newDT.Columns.Add("Cripto", GetType(String))
            newDT.Columns.Add("Perf", GetType(String))
            newDT.Columns.Add("Wallet", GetType(String))
            newDT.Columns.Add("Qtd", GetType(Decimal))
            newDT.Columns.Add("vlEntradaUSD", GetType(Decimal))
            newDT.Columns.Add("vlEntradaBRL", GetType(Decimal))
            newDT.Columns.Add("precoMedio", GetType(Decimal))
            newDT.Columns.Add("precoAtual", GetType(Decimal))
            newDT.Columns.Add("24horas", GetType(String))
            newDT.Columns.Add("marketcap", GetType(Decimal))
            newDT.Columns.Add("vlAtualUSD", GetType(Decimal))
            newDT.Columns.Add("vlAtualBRL", GetType(Decimal))
            newDT.Columns.Add("ROIusd", GetType(Decimal))
            newDT.Columns.Add("ROIbrl", GetType(Decimal))
            newDT.Columns.Add("X", GetType(String))

            For Each row As DataRow In originalDT.Rows

                Dim id As Long =
                    Convert.ToInt64(row("Id"), CultureInfo.InvariantCulture)

                Dim symbol As String =
                    row("Symbol").ToString().Trim().ToUpperInvariant()

                Dim wallet As String =
                    row("Wallet").ToString().Trim()

                Dim market As CoinMarketData =
                    mcapDict.GetValueOrDefault(
                        symbol,
                        New CoinMarketData())

                Dim currPrice As Decimal = market.Price
                Dim quantity As Decimal = 0D

                Select Case wallet.ToUpperInvariant()

                    Case "BINANCE"

                        quantity =
                            binanceAssets.GetValueOrDefault(symbol, 0D)

                        Dim priceString As String =
                            Await b.BINANCE_GetCoinsInfo(symbol)

                        If Not String.IsNullOrWhiteSpace(priceString) Then

                            Dim parts() As String =
                                priceString.Split("|"c)

                            If parts.Length > 0 Then
                                currPrice = formatter.decimalBR(parts(0))
                            End If

                        End If

                    Case "GATE.IO"

                        quantity =
                            gateAssets.GetValueOrDefault(symbol, 0D)

                        Debug.WriteLine(
                            $"[GATE LOAD] {symbol} -> saldo={quantity}")

                        Try

                            Dim gatePrice As Decimal =
                                Await gate.GATE_GetCoinsPrice(symbol)

                            Debug.WriteLine(
                                $"[GATE LOAD] {symbol} -> preço={gatePrice}")

                            If gatePrice > 0D Then
                                currPrice = gatePrice
                            End If

                        Catch ex As HttpRequestException

                            Debug.WriteLine(
                                $"[GATE LOAD] ERRO HTTP {symbol}: {ex.Message}")

                        Catch ex As Exception

                            Debug.WriteLine(
                                $"[GATE LOAD] ERRO {symbol}: {ex}")

                        End Try

                    Case Else

                        quantity =
                            Convert.ToDecimal(
                                row("Quantity"),
                                CultureInfo.InvariantCulture)

                End Select

                If quantity <= 0D Then

                    Debug.WriteLine(
                        $"[{wallet}] {symbol}: saldo zero ou não encontrado.")

                    Continue For

                End If

                Dim initialPrice As Decimal =
                    Convert.ToDecimal(
                        row("InitialPrice"),
                        CultureInfo.InvariantCulture)

                Dim initialValueUSD As Decimal = quantity * initialPrice
                Dim initialValueBRL As Decimal = initialValueUSD * usdBrl
                Dim currentValueUSD As Decimal = quantity * currPrice
                Dim currentValueBRL As Decimal = currentValueUSD * usdBrl
                Dim roi As Decimal = currentValueUSD - initialValueUSD

                Dim performance As Decimal = 0D

                If initialValueUSD > 0D Then
                    performance =
                        (roi / initialValueUSD) * 100D
                End If

                Dim multiplier As Decimal = 0D

                If initialValueUSD > 0D Then
                    multiplier =
                        currentValueUSD / initialValueUSD
                End If

                initialValue += initialValueUSD

                If formatter.stablecoins.Contains(symbol) Then
                    cashflow += currentValueUSD
                Else
                    currValueTotal += currentValueUSD
                    profit += roi
                End If

                Dim newRow As DataRow = newDT.NewRow()
                newRow("Cripto") = symbol
                newRow("Perf") = $"{performance:F2}%"
                newRow("Wallet") = wallet
                newRow("Qtd") = quantity
                newRow("vlEntradaUSD") = initialValueUSD
                newRow("vlEntradaBRL") = initialValueBRL
                newRow("precoMedio") = initialPrice
                newRow("precoAtual") = currPrice
                newRow("24horas") = market.Change24h.ToString("F2")
                newRow("marketcap") = market.MarketCap
                newRow("vlAtualUSD") = currentValueUSD
                newRow("vlAtualBRL") = currentValueBRL
                newRow("ROIusd") = roi
                newRow("ROIbrl") = roi * usdBrl
                newRow("X") =
                    If(multiplier > 0D,
                       $"{multiplier:N2} X",
                       "0 X")

                If initialValueUSD > 1D Then
                    newDT.Rows.Add(newRow)
                End If

                listCriptos.Add(symbol)
                listAddress.Add(wallet)
                listCurrValue.Add(currentValueUSD)

                PortfolioRepository.UpdateLastPrice(
                    id,
                    currPrice)

            Next

            Dim total As Decimal = cashflow + currValueTotal
            Dim percentCash As Decimal =
                If(total > 0D,
                   (cashflow / total) * 100D,
                   0D)

            Dim percentInvested As Decimal =
                If(total > 0D,
                   (currValueTotal / total) * 100D,
                   0D)

            Dim walletPerformance As Decimal =
                If(initialValue > 0D,
                   (profit / initialValue) * 100D,
                   0D)

            If total > 0D Then

                For i As Integer = 0 To listCriptos.Count - 1
                    Dim symbol = listCriptos(i)
                    If formatter.stablecoins.Contains(symbol) Then
                        Continue For
                    End If

                    criptoDic(symbol) =
                        If(currValueTotal > 0D,
                           (listCurrValue(i) / currValueTotal) * 100D,
                           0D)
                Next

            End If

            For Each walletName In listAddress.Distinct()

                Dim sum As Decimal = 0D

                For i As Integer = 0 To listAddress.Count - 1

                    If listAddress(i) = walletName Then
                        sum += listCurrValue(i)
                    End If

                Next

                addressDic(walletName) = sum

            Next

            FormMain.lbTotalBRL.Visible = True
            FormMain.lbTotalBRL.Text =
                formatter.BRLformat(profit * usdBrl)

            FormMain.lbTotalBRL.ForeColor =
                If(profit > 0D,
                   Color.FromArgb(0, 255, 0),
                   Color.FromArgb(255, 73, 73))

            FormMain.lbValoresHojeUSD.ForeColor =
                If(total < initialValue,
                   Color.IndianRed,
                   Color.GreenYellow)

            FormMain.lbValoresHojeBRL.ForeColor =
                If(total < initialValue,
                   Color.IndianRed,
                   Color.Cyan)

            FormMain.lbRoiUSD.ForeColor =
                If(profit < 0D,
                   Color.Red,
                   Color.Gold)

            FormMain.lbPerformWallet.ForeColor =
                If(walletPerformance < 0D,
                   Color.Red,
                   Color.Lime)

            FormMain.lbDolar.Text = formatter.BRLformat(usdBrl)
            FormMain.lbBTC.Text = formatter.USDformat(btcPrice)
            FormMain.lbDom.Text = If(dom > 0D, $"{dom:F2}%", "--")
            FormMain.lbPerformWallet.Text = $"{walletPerformance:F2}%"
            ' Spot exibe o valor atual dos ativos investidos; Caixa já é
            ' mostrado separadamente com os stablecoins.
            FormMain.lbTotalEntradaUSD.Text = formatter.USDformat(currValueTotal)
            FormMain.lbTotalEntradaBRL.Text = formatter.BRLformat(currValueTotal * usdBrl)
            FormMain.lbValoresHojeUSD.Text = formatter.USDformat(total)
            FormMain.lbValoresHojeBRL.Text = formatter.BRLformat(total * usdBrl)
            FormMain.lbRoiUSD.Text = formatter.USDformat(profit)
            FormMain.lbCaixa.Text = formatter.USDformat(cashflow)
            FormMain.lbCaixaBRL.Text = formatter.BRLformat(cashflow * usdBrl)
            FormMain.lbPercentCaixa.Text = $"{percentCash:F2}%"
            FormMain.lbPercentInvestido.Text = $"{percentInvested:F2}%"

            datagrid.DataSource = newDT

            If criptoDic.Count > 0 Then
                FormMain.criptoGraph(criptoDic)
            End If

            If addressDic.Count > 0 Then
                FormMain.addressGraph(addressDic)
            End If

            JSON.hideMarketDataLabel()
            My.Settings.lastView = Date.Now

            If currencyColumn = "USD" Then
                FormMain.showUSDCollumns()
            ElseIf currencyColumn = "BRL" Then
                FormMain.showBRLCollumns()
            End If

            formatter.FormatGrid(datagrid)

            Return True

        Catch ex As Exception

            FormMain.lbDebug.AppendText(
                "Erro ao carregar os dados: " & ex.ToString())

            Debug.WriteLine(
                "Ocorreu um erro ao carregar os dados: " & ex.Message)

            Return False

        End Try

    End Function

    Private Shared Async Function LoadFallbackMarketDataAsync(
        binance As Binance,
        symbols As IEnumerable(Of String),
        cmc As Cotacao) As Task(Of Dictionary(Of String, CoinMarketData))

        Dim result As New Dictionary(Of String, CoinMarketData)(StringComparer.OrdinalIgnoreCase)
        Dim useCmc = Not String.IsNullOrWhiteSpace(My.Settings.apiCMCKey) AndAlso
                     Not String.IsNullOrWhiteSpace(My.Settings.activeAPI)

        For Each symbol In symbols.Distinct(StringComparer.OrdinalIgnoreCase)
            Dim loaded = False

            If useCmc Then
                Try
                    Dim rawCmc = Await cmc.CM_GetCriptoPrices(symbol)
                    Dim parts = rawCmc?.ToString().Split("|"c)
                    Dim price As Decimal
                    Dim marketCap As Decimal = 0D

                    If parts IsNot Nothing AndAlso
                       parts.Length >= 1 AndAlso
                       TryParseMarketDecimal(parts(0), price) AndAlso
                       price > 0D Then
                        If parts.Length >= 2 Then
                            TryParseMarketDecimal(parts(1), marketCap)
                        End If

                        result(symbol) = New CoinMarketData With {
                            .Price = price,
                            .MarketCap = marketCap}
                        loaded = True
                    End If
                Catch ex As Exception
                    Debug.WriteLine($"[MARKET] CoinMarketCap indisponível para {symbol}: {ex.Message}")
                End Try
            End If

            If loaded Then Continue For

            Try
                Dim rawBinance = Await binance.BINANCE_GetCoinsInfo(symbol)
                Dim parts = rawBinance?.ToString().Split("|"c)
                Dim price As Decimal

                If parts IsNot Nothing AndAlso
                   parts.Length >= 1 AndAlso
                   TryParseMarketDecimal(parts(0), price) AndAlso
                   price > 0D Then
                    result(symbol) = New CoinMarketData With {.Price = price}
                End If
            Catch ex As Exception
                Debug.WriteLine($"[MARKET] Binance indisponível para {symbol}: {ex.Message}")
            End Try
        Next

        Return result
    End Function

    Private Shared Function TryParseMarketDecimal(value As String, ByRef result As Decimal) As Boolean
        If String.IsNullOrWhiteSpace(value) Then Return False

        Dim text = value.Trim()
        Dim hasComma = text.Contains(","c)
        Dim hasDot = text.Contains("."c)

        If hasComma AndAlso hasDot Then
            If text.LastIndexOf(","c) > text.LastIndexOf("."c) Then
                text = text.Replace(".", String.Empty).Replace(",", ".")
            Else
                text = text.Replace(",", String.Empty)
            End If

            Return Decimal.TryParse(
                text,
                NumberStyles.Float,
                CultureInfo.InvariantCulture,
                result)
        End If

        If hasComma Then
            Return Decimal.TryParse(
                text,
                NumberStyles.Number,
                CultureInfo.GetCultureInfo("pt-BR"),
                result)
        End If

        Return Decimal.TryParse(
            text,
            NumberStyles.Float Or NumberStyles.AllowThousands,
            CultureInfo.InvariantCulture,
            result)
    End Function

End Class
