Imports System.Data
Imports System.Globalization

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
            Dim cot As New Cotacao
            Dim gate As New Gateio
            Dim gec As New Coingecko
            Dim formatter As New JSON

            Await b.SyncBinanceTime()

            Dim binanceAssets = Await b.BINANCE_GetAllAssetsFull()
            Dim gateAssets = Await gate.GATE_GetAllSpotAssets()

            Dim mcapDict = Await gec.CGECKO_MarketData(allSymbols)

            Dim usdBrl As Decimal =
                Await gec.CGECKO_GetPrice("USDT", "brl")

            If usdBrl <= 0D Then
                Throw New Exception("Cotação USDT/BRL retornou zero. Não foi possível atualizar os valores em BRL.")
            End If

            JSON.USDBRLprice = usdBrl
            formatter.USDBRLprice = usdBrl

            Dim dom As Decimal? = Await cot.CM_GetBTCDOM()

            Dim btcPriceString As String = Await b.BINANCE_GetCoinsInfo("BTC")
            Dim btcPrice As Decimal = 0D
            Dim btcParts() As String = btcPriceString.Split("|"c)

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

                Dim id As Long = Convert.ToInt64(row("Id"), CultureInfo.InvariantCulture)
                Dim symbol As String = row("Symbol").ToString().Trim().ToUpperInvariant()
                Dim wallet As String = row("Wallet").ToString().Trim()

                Dim market As CoinMarketData =
                    mcapDict.GetValueOrDefault(symbol, New CoinMarketData())

                Dim currPrice As Decimal = market.Price
                Dim quantity As Decimal = 0D

                Select Case wallet.ToUpperInvariant()

                    Case "BINANCE"

                        quantity =
                            binanceAssets.GetValueOrDefault(symbol, 0D)

                        Dim priceString As String =
                            Await b.BINANCE_GetCoinsInfo(symbol)

                        If Not String.IsNullOrWhiteSpace(priceString) Then
                            Dim parts() As String = priceString.Split("|"c)
                            If parts.Length > 0 Then
                                currPrice = formatter.decimalBR(parts(0))
                            End If
                        End If

                    Case "GATE.IO"

                        quantity =
                            gateAssets.GetValueOrDefault(symbol, 0D)

                        Dim gatePrice As Decimal =
                            Await gate.GATE_GetCoinsPrice(symbol)

                        If gatePrice > 0D Then
                            currPrice = gatePrice
                        End If

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
                newRow("X") = If(multiplier > 0D, $"{multiplier:N2} X", "0 X")

                If initialValueUSD > 1D Then
                    newDT.Rows.Add(newRow)
                End If

                listCriptos.Add(symbol)
                listAddress.Add(wallet)
                listCurrValue.Add(currentValueUSD)

                PortfolioRepository.UpdateLastPrice(id, currPrice)

            Next

            Dim total As Decimal = cashflow + currValueTotal
            Dim percentCash As Decimal = If(total > 0D, (cashflow / total) * 100D, 0D)
            Dim percentInvested As Decimal = If(total > 0D, (currValueTotal / total) * 100D, 0D)
            Dim walletPerformance As Decimal = If(initialValue > 0D, (profit / initialValue) * 100D, 0D)

            If total > 0D Then
                For i As Integer = 0 To listCriptos.Count - 1
                    criptoDic(listCriptos(i)) = (listCurrValue(i) / total) * 100D
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
            FormMain.lbTotalBRL.Text = formatter.BRLformat(profit * usdBrl)
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
            FormMain.lbDom.Text = $"{dom.GetValueOrDefault():F2}%"
            FormMain.lbPerformWallet.Text = $"{walletPerformance:F2}%"
            FormMain.lbTotalEntradaUSD.Text = formatter.USDformat(initialValue)
            FormMain.lbTotalEntradaBRL.Text = formatter.BRLformat(initialValue * usdBrl)
            FormMain.lbValoresHojeUSD.Text = formatter.USDformat(total)
            FormMain.lbValoresHojeBRL.Text = formatter.BRLformat(total * usdBrl)
            FormMain.lbRoiUSD.Text = formatter.USDformat(profit)
            FormMain.lbCaixa.Text = formatter.USDformat(cashflow)
            FormMain.lbCaixaBRL.Text = formatter.BRLformat(cashflow * usdBrl)
            FormMain.lbPercentCaixa.Text = $"{percentCash:F2}%"
            FormMain.lbPercentInvestido.Text = $"{percentInvested:F2}%"

            datagrid.DataSource = newDT
            formatter.FormatGrid(datagrid)

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

            Return True

        Catch ex As Exception

            FormMain.lbDebug.AppendText(
                "Erro ao carregar os dados: " & ex.ToString())

            Debug.WriteLine(
                "Ocorreu um erro ao carregar os dados: " & ex.Message)

            Return False

        End Try

    End Function

End Class
