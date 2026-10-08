Imports System.Globalization
Imports System.IO
Imports System.Windows.Forms.DataVisualization.Charting
Imports Newtonsoft.Json.Linq
Imports System.Diagnostics
Imports System.Diagnostics.Tracing
Public Class FormMain
    Public remainingtimeInSeconds As Integer
    Dim Cjson As New JSON
    Dim chart As New Charts
    Dim B As New Binance
    Dim gec As New Coingecko
    Private ReadOnly _binanceWs As New BinanceWebSocket
    Private ReadOnly _gateWs As New GateWebSocket
    Private ReadOnly _binanceFuturesMarketWs As New BinanceFuturesMarketWebSocket
    Private ReadOnly _binanceFuturesUserWs As New BinanceFuturesUserDataWebSocket
    Private _marketRefreshRunning As Boolean = False
    Private _futuresLoadRunning As Boolean = False
    Private _futuresMarketSymbolsKey As String = String.Empty
    Private _futuresUserStreamStarted As Boolean = False
    Private _spotPnlUsd As Decimal = 0D
    Private _futuresPnlUsd As Decimal = 0D

    Private Sub CriptoToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CriptoToolStripMenuItem.Click
        FormEntradas.Show()
    End Sub
    Private Sub FecharToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles FecharToolStripMenuItem.Click
        Application.Exit()
    End Sub
    Public Shared Function msgQuestion(ByVal msgText As String, ByVal Title As String) As String
        If MessageBox.Show(msgText, Title, MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button3) = DialogResult.Yes Then
            Return True
        Else
            Return False
        End If
    End Function
    Private Sub Form1_LoadAsync(sender As Object, e As EventArgs) Handles MyBase.Load
        Setup()
        lbDataTotalToday.Text = Date.Today & ":"
    End Sub

    Public Sub changeOnOffColor(text As String)
        Dim palavra As String = text
        Dim pos As Integer = lbDebug.Text.IndexOf(palavra)

        If pos >= 0 Then

            lbDebug.SelectionStart = pos
            lbDebug.SelectionLength = palavra.Length

            If palavra = "Online" Then
                lbDebug.SelectionColor = Color.LimeGreen
            ElseIf palavra = "Offline" Then
                lbDebug.SelectionColor = Color.Red
            ElseIf palavra = "Pronto" Then
                lbDebug.SelectionColor = Color.Aqua
            End If

        End If

    End Sub

    Public Async Sub Setup()

        Try

            AddHandler _binanceWs.PriceUpdated, AddressOf BinanceWs_PriceUpdated

            AddHandler _binanceWs.ConnectionStateChanged, AddressOf BinanceWs_ConnectionStateChanged
            AddHandler _gateWs.PriceUpdated, AddressOf GateWs_PriceUpdated
            AddHandler _gateWs.ConnectionStateChanged, AddressOf GateWs_ConnectionStateChanged
            AddHandler _binanceFuturesMarketWs.MarkPriceUpdated, AddressOf BinanceFuturesMarkPriceUpdated
            AddHandler _binanceFuturesMarketWs.ConnectionStateChanged, AddressOf BinanceFuturesWs_ConnectionStateChanged
            AddHandler _binanceFuturesUserWs.DataUpdated, AddressOf BinanceFuturesUserDataUpdated
            AddHandler _binanceFuturesUserWs.ConnectionStateChanged, AddressOf BinanceFuturesWs_ConnectionStateChanged

            Await B.SyncBinanceTime()

            chart.removeCharts()

            lbDebug.Clear()
            lbDebug.AppendText("Status: Pronto")

            changeOnOffColor("Pronto")

        Catch ex As Exception

            lbDebug.Clear()
            lbDebug.AppendText(
            "Erro ao carregar o portfólio: " &
            ex.Message)

        End Try

    End Sub

    Private Async Function StartGateWebSocket() As Task

        Dim symbols As New List(Of String)

        For Each row As DataGridViewRow In dgPortfolio.Rows

            If row.IsNewRow Then Continue For

            Dim wallet As String =
            row.Cells(2).Value?.
            ToString().
            Trim().
            ToUpperInvariant()

            If wallet <> "GATE.IO" Then Continue For

            Dim symbol As String =
            row.Cells(0).Value?.
            ToString().
            Trim().
            ToUpperInvariant()

            If Not String.IsNullOrWhiteSpace(symbol) Then
                symbols.Add(symbol)
            End If

        Next

        If symbols.Count > 0 Then
            Await _gateWs.StartAsync(symbols.Distinct())
        End If

    End Function

    Private Sub UpdateGateRow(
    symbol As String,
    price As Decimal)

        Try

            For Each row As DataGridViewRow In dgPortfolio.Rows

                If row.IsNewRow Then
                    Continue For
                End If

                Dim rowSymbol As String =
                row.Cells(0).Value?.
                ToString().
                Trim().
                ToUpperInvariant()

                Dim wallet As String =
                row.Cells(2).Value?.
                ToString().
                Trim().
                ToUpperInvariant()

                If rowSymbol <> symbol.ToUpperInvariant() Then
                    Continue For
                End If

                If wallet <> "GATE.IO" Then
                    Continue For
                End If

                Dim qtd As Decimal =
                Convert.ToDecimal(row.Cells(3).Value)

                Dim precoMedio As Decimal =
                Convert.ToDecimal(row.Cells(6).Value)

                Dim valorEntradaUSD As Decimal =
                qtd * precoMedio

                Dim valorAtualUSD As Decimal =
                qtd * price

                Dim roiUSD As Decimal =
                valorAtualUSD - valorEntradaUSD

                Dim performance As Decimal = 0D

                If valorEntradaUSD > 0D Then

                    performance =
                    (roiUSD / valorEntradaUSD) * 100D

                End If

                Dim usdBrl As Decimal =
                Cjson.USDBRLprice

                Dim valorAtualBRL As Decimal =
                valorAtualUSD * usdBrl

                Dim roiBRL As Decimal =
                roiUSD * usdBrl

                Dim x As Decimal = 0D

                If valorEntradaUSD > 0D Then
                    x = valorAtualUSD / valorEntradaUSD
                End If

                ' Preço
                row.Cells(7).Value = price

                ' Performance
                row.Cells(1).Value =
                $"{performance:F2}%"

                ' Valor atual USD
                row.Cells(10).Value =
                valorAtualUSD

                ' Valor atual BRL
                row.Cells(11).Value =
                valorAtualBRL

                ' ROI USD
                row.Cells(12).Value =
                roiUSD

                ' ROI BRL
                row.Cells(13).Value =
                roiBRL

                ' Multiplicador
                row.Cells(14).Value =
                $"{x:N2} X"

                Exit For

            Next

            ' O mesmo recálculo usado pela Binance
            UpdateRealtimeOverview()

        Catch ex As Exception

            Debug.WriteLine(
            "[GATE WS] Erro atualizando " &
            symbol &
            ": " &
            ex.Message)

        End Try

    End Sub

    Private Sub GateWs_ConnectionStateChanged(
    connected As Boolean,
    message As String)

        Debug.WriteLine(
        "[GATE WS] " &
        message)

    End Sub

    Private Sub GateWs_PriceUpdated(
    symbol As String,
    price As Decimal)

        If Me.InvokeRequired Then

            Me.BeginInvoke(
            New Action(
                Sub()
                    UpdateGateRow(symbol, price)
                End Sub))

            Return

        End If

        UpdateGateRow(symbol, price)

    End Sub

    Private Sub BinanceWs_ConnectionStateChanged(
    connected As Boolean,
    message As String)

        If Me.InvokeRequired Then

            Me.BeginInvoke(
            New Action(
                Sub()
                    BinanceWs_ConnectionStateChanged(
                        connected,
                        message)
                End Sub))

            Return

        End If

        Debug.WriteLine(message)

        If connected Then

            lbDebug.Clear()
            lbDebug.AppendText("Status: Online")
            changeOnOffColor("Online")

        Else

            lbDebug.AppendText(
            Environment.NewLine & message)

        End If

    End Sub

    Private Async Function StartBinanceWebSocket() As Task

        Try

            Dim symbols As New List(Of String)

            Debug.WriteLine(
    "[WS] Símbolos Binance: " &
    String.Join(", ", symbols))

            For Each row As DataGridViewRow In dgPortfolio.Rows

                If row.IsNewRow Then
                    Continue For
                End If

                Dim wallet As String =
                row.Cells(2).Value?.ToString().Trim().ToUpperInvariant()

                If wallet <> "BINANCE" Then
                    Continue For
                End If

                Dim symbol As String =
                row.Cells(0).Value?.ToString().Trim().ToUpperInvariant()

                If String.IsNullOrWhiteSpace(symbol) Then
                    Continue For
                End If

                symbols.Add(symbol)

            Next

            symbols =
            symbols.Distinct().ToList()

            If symbols.Count = 0 Then

                Debug.WriteLine(
                "Nenhum ativo Binance encontrado no DataGrid.")

                Return

            End If

            Await _binanceWs.StartAsync(symbols)

        Catch ex As Exception

            Debug.WriteLine(
            "Erro iniciando Binance WebSocket: " &
            ex.Message)

        End Try

    End Function

    Private Sub UpdateRealtimeOverview(Optional force As Boolean = False)

        Try
            If _marketRefreshRunning AndAlso Not force Then
                Return
            End If

            Dim totalEntradaUSD As Decimal = 0D
            Dim totalAtualUSD As Decimal = 0D

            Dim cashflowUSD As Decimal = 0D
            Dim investidoUSD As Decimal = 0D

            Dim lucroUSD As Decimal = 0D

            For Each row As DataGridViewRow In dgPortfolio.Rows

                If row.IsNewRow Then
                    Continue For
                End If

                Dim wallet As String =
                row.Cells(2).Value?.
                ToString().
                Trim().
                ToUpperInvariant()

                Dim entrada As Decimal =
                Convert.ToDecimal(
                    row.Cells(4).Value)

                Dim atual As Decimal =
                Convert.ToDecimal(
                    row.Cells(10).Value)

                Dim symbol As String =
                row.Cells(0).Value?.
                ToString().
                Trim().
                ToUpperInvariant()

                totalEntradaUSD += entrada
                totalAtualUSD += atual

                If Cjson.stablecoins.Contains(symbol) Then

                    cashflowUSD += atual

                Else

                    investidoUSD += atual
                    lucroUSD +=
                    atual - entrada

                End If

            Next

            ' =============================================
            ' TOTAL
            ' =============================================
            Dim usdBrl As Decimal =
            Cjson.USDBRLprice

            Dim totalBRL As Decimal =
            totalAtualUSD * usdBrl

            Dim percentualCaixa As Decimal = 0D
            Dim percentualInvestido As Decimal = 0D
            Dim performanceWallet As Decimal = 0D

            If totalAtualUSD > 0D Then

                percentualCaixa =
                (cashflowUSD / totalAtualUSD) * 100D

                percentualInvestido =
                (investidoUSD / totalAtualUSD) * 100D

            End If

            If totalEntradaUSD > 0D Then

                performanceWallet =
                (lucroUSD / totalEntradaUSD) * 100D

            End If

            ' =============================================
            ' UI
            ' =============================================
            _spotPnlUsd = lucroUSD

            Me.lbTotalEntradaUSD.Text =
            Cjson.USDformat(totalEntradaUSD)

            Me.lbTotalEntradaBRL.Text =
            Cjson.BRLformat(
                totalEntradaUSD * usdBrl)

            Me.lbValoresHojeUSD.Text =
            Cjson.USDformat(totalAtualUSD)

            Me.lbValoresHojeBRL.Text =
            Cjson.BRLformat(totalBRL)

            Me.lbRoiUSD.Text =
            Cjson.USDformat(lucroUSD)

            ' Mantém as regras visuais sincronizadas com os valores recalculados pelo WebSocket.
            Me.lbTotalEntradaUSD.ForeColor = Color.LimeGreen
            Me.lbTotalEntradaBRL.ForeColor = Color.DeepSkyBlue

            Me.lbValoresHojeUSD.ForeColor =
            If(totalAtualUSD < totalEntradaUSD,
               Color.IndianRed,
               Color.GreenYellow)

            Me.lbValoresHojeBRL.ForeColor =
            If(totalAtualUSD < totalEntradaUSD,
               Color.IndianRed,
               Color.Cyan)

            Me.lbRoiUSD.ForeColor =
            If(lucroUSD < 0D,
               Color.Red,
               Color.Gold)

            Me.lbCaixa.Text =
            Cjson.USDformat(cashflowUSD)

            Me.lbCaixaBRL.Text =
            Cjson.BRLformat(
                cashflowUSD * usdBrl)

            Me.lbPercentCaixa.Text =
            $"{percentualCaixa:F2}%"

            Me.lbPercentInvestido.Text =
            $"{percentualInvestido:F2}%"

            Me.lbPerformWallet.Text =
            $"{performanceWallet:F2}%"

            Me.lbRoiUSD.ForeColor =
            If(
                lucroUSD < 0D,
                Color.Red,
                Color.Gold)

            Me.lbPerformWallet.ForeColor =
            If(
                performanceWallet < 0D,
                Color.Red,
                Color.Lime)

            UpdateFuturesPnlSummary()

        Catch ex As Exception

            Debug.WriteLine(
            "Erro atualizando visão geral realtime: " &
            ex.Message)

        End Try

    End Sub

    Private Sub UpdateBinanceRow(
    symbol As String,
    price As Decimal)

        Try

            For Each row As DataGridViewRow In dgPortfolio.Rows

                If row.IsNewRow Then
                    Continue For
                End If

                Dim rowSymbol As String =
                row.Cells(0).Value?.
                ToString().
                Trim().
                ToUpperInvariant()

                Dim wallet As String =
                row.Cells(2).Value?.
                ToString().
                Trim().
                ToUpperInvariant()

                If rowSymbol <> symbol.ToUpperInvariant() Then
                    Continue For
                End If

                If wallet <> "BINANCE" Then
                    Continue For
                End If

                ' =============================================
                ' QUANTIDADE
                ' =============================================
                Dim qtd As Decimal =
                Convert.ToDecimal(
                    row.Cells(3).Value)

                ' =============================================
                ' PREÇO MÉDIO
                ' =============================================
                Dim precoMedio As Decimal =
                Convert.ToDecimal(
                    row.Cells(6).Value)

                ' =============================================
                ' VALOR ATUAL USD
                ' =============================================
                Dim valorAtualUSD As Decimal =
                qtd * price

                ' =============================================
                ' ROI USD
                ' =============================================
                Dim valorEntradaUSD As Decimal =
                qtd * precoMedio

                Dim roiUSD As Decimal =
                valorAtualUSD - valorEntradaUSD

                Dim performance As Decimal = 0D

                If valorEntradaUSD > 0D Then

                    performance =
                    (roiUSD / valorEntradaUSD) * 100D

                End If

                ' =============================================
                ' USD → BRL
                ' =============================================
                Dim usdBrl As Decimal =
                Cjson.USDBRLprice

                Dim valorAtualBRL As Decimal =
                valorAtualUSD * usdBrl

                Dim roiBRL As Decimal =
                roiUSD * usdBrl

                ' =============================================
                ' MULTIPLICADOR
                ' =============================================
                Dim x As Decimal = 0D

                If valorEntradaUSD > 0D Then

                    x =
                    valorAtualUSD / valorEntradaUSD

                End If

                ' =============================================
                ' ATUALIZA GRID
                ' =============================================
                row.Cells(7).Value =
                price

                row.Cells(10).Value =
                valorAtualUSD

                row.Cells(11).Value =
                valorAtualBRL

                row.Cells(12).Value =
                roiUSD

                row.Cells(13).Value =
                roiBRL

                row.Cells(1).Value =
                $"{performance:F2}%"

                If x > 0D Then
                    row.Cells(14).Value =
                    $"{x:N2} X"
                Else
                    row.Cells(14).Value =
                    "0 X"
                End If

                Exit For

            Next

            ' Recalcula visão geral
            UpdateRealtimeOverview()

            ' A formatação do grid ocorre no carregamento/ordenação; não a repetimos a cada tick.
            ' Isso evita alterar visibilidade e binding durante o update do WebSocket.

        Catch ex As Exception

            Debug.WriteLine(
            "Erro atualizando preço realtime [" &
            symbol &
            "]: " &
            ex.Message)

        End Try

    End Sub

    Public Sub BinanceWs_PriceUpdated(
    symbol As String,
    price As Decimal)

        Debug.WriteLine(
        $"[WS RECEBIDO] {symbol} = {price.ToString(CultureInfo.InvariantCulture)}")

        If Me.InvokeRequired Then

            Me.BeginInvoke(
            New Action(
                Sub()
                    UpdateBinanceRow(symbol, price)
                End Sub))

            Return

        End If

        UpdateBinanceRow(symbol, price)

    End Sub

    Private Sub dgPortfolio_MouseLeave(sender As Object, e As EventArgs) Handles dgPortfolio.MouseLeave
        dgPortfolio.ClearSelection()
        dgPortfolio.CurrentCell = Nothing
        dgPortfolio.Cursor = Cursors.Default
    End Sub

    Private Shared Sub UpdateCurrencyColumnCursor(grid As DataGridView, rowIndex As Integer, columnIndex As Integer, columnName As String)
        If rowIndex >= 0 AndAlso columnIndex >= 0 AndAlso
           String.Equals(grid.Columns(columnIndex).Name, columnName, StringComparison.OrdinalIgnoreCase) Then
            grid.Cursor = Cursors.Hand
        Else
            grid.Cursor = Cursors.Default
        End If
    End Sub

    Private Sub dgPortfolio_CellMouseMove(sender As Object, e As DataGridViewCellMouseEventArgs) Handles dgPortfolio.CellMouseMove
        UpdateCurrencyColumnCursor(dgPortfolio, e.RowIndex, e.ColumnIndex, "Cripto")
    End Sub

    Private Sub dgFuturos_CellMouseMove(sender As Object, e As DataGridViewCellMouseEventArgs) Handles dgFuturos.CellMouseMove
        UpdateCurrencyColumnCursor(dgFuturos, e.RowIndex, e.ColumnIndex, "Symbol")
    End Sub

    Private Sub dgFuturos_MouseLeave(sender As Object, e As EventArgs) Handles dgFuturos.MouseLeave
        dgFuturos.Cursor = Cursors.Default
    End Sub

    Private Sub FormMain_Resize(sender As Object, e As EventArgs) Handles MyBase.Resize
        Dim json As New JSON
        Try
            json.FormatGrid(dgPortfolio)

            If Me.WindowState = FormWindowState.Minimized Then
                Me.Hide()
                NotifyIcon1.Visible = True
                ' NotifyIcon1.ShowBalloonTip(3000, "Porfólio Cripto", lbBTC.Text, ToolTipIcon.Info)
            End If

            Adjust()

            Me.CenterToScreen()

        Catch ex As Exception

        End Try

    End Sub

    Public Async Function refreshMarket() As Task(Of Boolean)
        Try
            If _marketRefreshRunning Then
                Return False
            End If

            _marketRefreshRunning = True

            chart.removeCharts()
            lbLoadFromMarket.Visible = True
            TimerBlink.Start()
            Cursor = Cursors.WaitCursor
            dgPortfolio.Cursor = Cursors.WaitCursor

            If Await PortfolioMarketService.LoadAsync(dgPortfolio) Then
                UpdateRealtimeOverview(force:=True)
                Await StartBinanceWebSocket()
                Await StartGateWebSocket()
                dgPortfolio.Sort(dgPortfolio.Columns("ROIusd"), System.ComponentModel.ListSortDirection.Descending)
                Adjust()
            Else
                lbDebug.AppendText("Status: Erro ao carregar o portfólio.")
            End If

            If TimerRefresh.Enabled = False Then
                lbAtualizaEm.Text = "Atualizado em:"
                lbRefresh.Location = New Point(125, 7)
                lbRefresh.Text = My.Settings.lastView
            End If

            'TimerCountdown.Stop()
            'TimerRefresh.Stop()

            Return True

        Catch ex As Exception
            lbDebug.Clear()
            lbDebug.AppendText("Offline - Erro ao atualizar o mercado: " & ex.Message)
            changeOnOffColor("Offline")
            JSON.hideMarketDataLabel()
            Return False

        Finally
            _marketRefreshRunning = False
            Cursor = Cursors.Default
            dgPortfolio.Cursor = Cursors.Default
        End Try

    End Function

    Private Async Sub btRefresh_Click_1Async(sender As Object, e As EventArgs) Handles btRefresh.Click
        If dgPortfolio.Visible Then
            Await refreshMarket()
        ElseIf dgFuturos.Visible Then
            BinanceFuturesUserDataUpdated()
        End If
    End Sub

    Private Sub dgPortfolio_Sorted(sender As Object, e As EventArgs) Handles dgPortfolio.Sorted
        Dim json As New JSON
        Try
            json.FormatGrid(dgPortfolio)
        Catch ex As Exception

        End Try
    End Sub

    Private Sub NotifyIcon1_MouseClick(sender As Object, e As MouseEventArgs) Handles NotifyIcon1.MouseClick
        Me.Show()
        Me.WindowState = FormWindowState.Normal
        NotifyIcon1.Visible = False
        Me.CenterToScreen()
    End Sub

    Private Sub IntervaloToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles IntervaloToolStripMenuItem.Click
        FormIntervalo.ShowDialog()
    End Sub

    Private Async Sub Timer1_TickAsync(sender As Object, e As EventArgs) Handles TimerRefresh.Tick
        Dim json As New JSON
        Try

            Me.remainingtimeInSeconds = TimerRefresh.Interval / 1000
            chart.removeCharts()
            lbLoadFromMarket.Visible = True
            TimerBlink.Start()

            Cursor = Cursors.WaitCursor
            dgPortfolio.Cursor = Cursors.WaitCursor
            Await PortfolioMarketService.LoadAsync(dgPortfolio)
            UpdateRealtimeOverview(force:=True)
            dgPortfolio.Sort(dgPortfolio.Columns("ROIusd"), System.ComponentModel.ListSortDirection.Descending)
            Adjust()

            lbAtualizaEm.Text = "Atualizado em:"
            lbRefresh.Text = My.Settings.lastView
            lbRefresh.Location = New Point(125, 7)
            json.FormatGrid(dgPortfolio)
        Catch ex As Exception
            Debug.WriteLine(
            "[TIMER] Erro ao atualizar mercado: " &
            ex.ToString())
        Finally
            Cursor = Cursors.Default
            dgPortfolio.Cursor = Cursors.Default
        End Try

    End Sub
    Private Sub TimerCountdown_Tick(sender As Object, e As EventArgs) Handles TimerCountdown.Tick
        remainingtimeInSeconds -= 1
        lbAtualizaEm.Text = "Atualiza em:"
        lbRefresh.Location = New Point(112, 7)
        'lbRefresh.Text = $"{(remainingtimeInSeconds \ 60).ToString("D2")}:{(remainingtimeInSeconds Mod 60).ToString("D2")}"
        Dim ts As TimeSpan = TimeSpan.FromSeconds(remainingtimeInSeconds)
        lbRefresh.Text = $"{Math.Floor(ts.TotalHours):00}:{ts.Minutes:00}:{ts.Seconds:00}"
    End Sub

    Private Sub NotifyIcon1_MouseMove(sender As Object, e As MouseEventArgs) Handles NotifyIcon1.MouseMove
        NotifyIcon1.Text = "BTC: " & lbBTC.Text
    End Sub

    Private Sub Adjust()
        lbTotalBRL.Location = New Point((PanelProfits.Width / 2) - (lbTotalBRL.Width / 2), 3)
        PanelGraphs.Width = Me.Width
        'dgPortfolio.Height = (dgPortfolio.RowCount * 35)
        ' Me.Height = MenuStrip1.Height + dgPortfolio.Height + PanelGraphs.Height + PanelProfits.Height + panelDebug.Height + 65
    End Sub

    Private Sub CadastroToolStripMenuItem_MouseEnter(sender As Object, e As EventArgs) Handles CadastroToolStripMenuItem.MouseEnter
        CadastroToolStripMenuItem.ForeColor = Color.Black
    End Sub

    Private Sub OpçõesToolStripMenuItem_MouseEnter(sender As Object, e As EventArgs) Handles OpçõesToolStripMenuItem.MouseEnter
        OpçõesToolStripMenuItem.ForeColor = Color.Black
    End Sub

    Private Sub CadastroToolStripMenuItem_MouseLeave(sender As Object, e As EventArgs) Handles CadastroToolStripMenuItem.MouseLeave
        CadastroToolStripMenuItem.ForeColor = Color.White
    End Sub

    Private Sub OpçõesToolStripMenuItem_MouseLeave(sender As Object, e As EventArgs) Handles OpçõesToolStripMenuItem.MouseLeave
        OpçõesToolStripMenuItem.ForeColor = Color.White
    End Sub

    Private Sub TimerBlink_Tick(sender As Object, e As EventArgs) Handles TimerBlink.Tick
        If lbLoadFromMarket.Visible = True Then
            If lbLoadFromMarket.ForeColor = Color.OrangeRed Then
                lbLoadFromMarket.ForeColor = Color.Gold
            ElseIf lbLoadFromMarket.ForeColor = Color.Gold Then
                lbLoadFromMarket.ForeColor = Color.White
            ElseIf lbLoadFromMarket.ForeColor = Color.White Then
                lbLoadFromMarket.ForeColor = Color.Yellow
            ElseIf lbLoadFromMarket.ForeColor = Color.Yellow Then
                lbLoadFromMarket.ForeColor = Color.OrangeRed
            End If
        End If
    End Sub

    Public Sub criptoGraph(criptoDic As Dictionary(Of String, Decimal))
        Dim gCriptos As New Charts

        gCriptos.collumGraph(500, 185, -2, 360, "Criptos", "% Criptos", 10, Color.Aqua, Color.FromArgb(30, 30, 30), SeriesChartType.Column, criptoDic, PanelGraphs)
    End Sub

    Public Sub addressGraph(criptoDic As Dictionary(Of String, Decimal))
        Dim gCriptos As New Charts

        gCriptos.pieGraph(330, 190, -2, 850, "Custódia", 10, Color.Aqua, Color.FromArgb(30, 30, 30), criptoDic, 7.5, Color.White, PanelGraphs)
    End Sub

    Private Sub dgPortfolio_CellPainting(sender As Object, e As DataGridViewCellPaintingEventArgs) Handles dgPortfolio.CellPainting
        If e.RowIndex >= 0 Then

            ' Pinta o fundo e o texto padrão
            e.PaintBackground(e.CellBounds, True)
            e.PaintContent(e.CellBounds)

            Using pen As New Pen(Color.FromArgb(70, 70, 70), 1)
                Dim rect = e.CellBounds
                Dim y = rect.Bottom - 1 ' Posição da linha inferior da célula
                e.Graphics.DrawLine(pen, rect.Left, y, rect.Right, y)
            End Using

            Dim colunasComLinhasVerticais As Integer() = {6} ' Índices das colunas que terão linhas verticais
            If colunasComLinhasVerticais.Contains(e.ColumnIndex) Then
                Using pen As New Pen(Color.FromArgb(3, 3, 3), 1)
                    Dim rect = e.CellBounds
                    Dim x = rect.Right - 1 ' Posição da borda direita da célula
                    e.Graphics.DrawLine(pen, x, rect.Top, x, rect.Bottom)
                End Using
            End If

            e.Handled = True ' Impede o desenho padrão

        End If

    End Sub

    Public Sub showUSDCollumns()
        Dim json As New JSON
        Try

            dgPortfolio.Columns(4).Visible = True
            dgPortfolio.Columns(10).Visible = True
            dgPortfolio.Columns(12).Visible = True

            dgPortfolio.Columns(5).Visible = False
            dgPortfolio.Columns(11).Visible = False
            dgPortfolio.Columns(13).Visible = False

            json.FormatGrid(dgPortfolio)

        Catch ex As Exception

        End Try
    End Sub

    Public Sub showBRLCollumns()
        Dim json As New JSON
        Try
            dgPortfolio.Columns(4).Visible = False
            dgPortfolio.Columns(10).Visible = False
            dgPortfolio.Columns(12).Visible = False

            dgPortfolio.Columns(5).Visible = True
            dgPortfolio.Columns(11).Visible = True
            dgPortfolio.Columns(13).Visible = True

            json.FormatGrid(dgPortfolio)

        Catch ex As Exception

        End Try

    End Sub

    Private Sub pbUSD_Click(sender As Object, e As EventArgs) Handles pbUSD.Click
        showUSDCollumns()
    End Sub

    Private Sub pbBRL_Click(sender As Object, e As EventArgs) Handles pbBRL.Click
        showBRLCollumns()
    End Sub

    Private Sub PortfolioToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles PortfolioToolStripMenuItem.Click
        Dim filePath = Application.StartupPath & "\JSON\portfolio.json"
        OpenFileDialog1.Filter = "json Files (*.json)|*.json"
        OpenFileDialog1.FileName = "portfolio.json"

        If OpenFileDialog1.ShowDialog() = System.Windows.Forms.DialogResult.OK Then
            Dim jsonFile = OpenFileDialog1.FileName
            If File.Exists(filePath) Then
                If MessageBox.Show("Substituir arquivo existente?", "Atenção", MessageBoxButtons.YesNoCancel) = DialogResult.Yes Then
                    File.Copy(jsonFile, filePath, True)
                Else
                    Exit Sub
                End If
            Else
                File.Copy(jsonFile, filePath, False)
            End If
            MessageBox.Show("Importado com sucesso!", "Importar arquivo json", MessageBoxButtons.OK)
        End If
    End Sub

    Private Sub ImportarToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles ImportarToolStripMenuItem1.Click
        Dim filePath = Application.StartupPath & "\JSON\wallets.json"
        OpenFileDialog1.Filter = "json Files (*.json)|*.json"
        OpenFileDialog1.FileName = "wallets.json"

        If OpenFileDialog1.ShowDialog() = System.Windows.Forms.DialogResult.OK Then
            Dim jsonFile = OpenFileDialog1.FileName
            If File.Exists(filePath) Then
                If MessageBox.Show("Substituir arquivo existente?", "Atenção", MessageBoxButtons.YesNoCancel) = DialogResult.Yes Then
                    File.Copy(jsonFile, filePath, True)
                Else
                    Exit Sub
                End If
            Else
                File.Copy(jsonFile, filePath, False)
            End If
            MessageBox.Show("Importado com sucesso!", "Importar arquivo json", MessageBoxButtons.OK)
        End If
    End Sub
    Private Sub ImportarToolStripMenuItem2_Click(sender As Object, e As EventArgs) Handles ImportarToolStripMenuItem2.Click
        Dim filePath = Application.StartupPath & "\JSON\criptos.json"
        OpenFileDialog1.Filter = "json Files (*.json)|*.json"
        OpenFileDialog1.FileName = "criptos.json"

        If OpenFileDialog1.ShowDialog() = System.Windows.Forms.DialogResult.OK Then
            Dim jsonFile = OpenFileDialog1.FileName
            If File.Exists(filePath) Then
                If MessageBox.Show("Substituir arquivo existente?", "Atenção", MessageBoxButtons.YesNoCancel) = DialogResult.Yes Then
                    File.Copy(jsonFile, filePath, True)
                Else
                    Exit Sub
                End If
            Else
                File.Copy(jsonFile, filePath, False)
            End If
            MessageBox.Show("Importado com sucesso!", "Importar arquivo json", MessageBoxButtons.OK)
        End If
    End Sub

    Private Sub WalletsExchangeToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles WalletsExchangeToolStripMenuItem.Click
        Dim filePath = Application.StartupPath & "\JSON\portfolio.json"

        SaveFileDialog1.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
        SaveFileDialog1.FileName = "portfolio.json"

        If SaveFileDialog1.ShowDialog() = System.Windows.Forms.DialogResult.OK Then
            Dim jsonDestination = SaveFileDialog1.FileName
            If File.Exists(jsonDestination) Then
                If MessageBox.Show("Substituir arquivo existente?", "Atenção", MessageBoxButtons.YesNoCancel) = DialogResult.Yes Then
                    File.Copy(filePath, jsonDestination, True)
                Else
                    Exit Sub
                End If
            Else
                File.Copy(filePath, jsonDestination, False)
            End If
            MessageBox.Show("Exportado com sucesso!", "Exportar arquivo json", MessageBoxButtons.OK)
        End If
    End Sub

    Private Sub ExportarToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles ExportarToolStripMenuItem1.Click
        Dim filePath = Application.StartupPath & "\JSON\wallets.json"

        SaveFileDialog1.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
        SaveFileDialog1.FileName = "wallets.json"

        If SaveFileDialog1.ShowDialog() = System.Windows.Forms.DialogResult.OK Then
            Dim jsonDestination = SaveFileDialog1.FileName
            If File.Exists(jsonDestination) Then
                If MessageBox.Show("Substituir arquivo existente?", "Atenção", MessageBoxButtons.YesNoCancel) = DialogResult.Yes Then
                    File.Copy(filePath, jsonDestination, True)
                Else
                    Exit Sub
                End If
            Else
                File.Copy(filePath, jsonDestination, False)
            End If
            MessageBox.Show("Exportado com sucesso!", "Exportar arquivo json", MessageBoxButtons.OK)
        End If
    End Sub

    Private Sub ExportarToolStripMenuItem2_Click(sender As Object, e As EventArgs) Handles ExportarToolStripMenuItem2.Click
        Dim filePath = Application.StartupPath & "\JSON\criptos.json"

        SaveFileDialog1.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
        SaveFileDialog1.FileName = "criptos.json"

        If SaveFileDialog1.ShowDialog() = System.Windows.Forms.DialogResult.OK Then
            Dim jsonDestination = SaveFileDialog1.FileName
            If File.Exists(jsonDestination) Then
                If MessageBox.Show("Substituir arquivo existente?", "Atenção", MessageBoxButtons.YesNoCancel) = DialogResult.Yes Then
                    File.Copy(filePath, jsonDestination, True)
                Else
                    Exit Sub
                End If
            Else
                File.Copy(filePath, jsonDestination, False)
            End If
            MessageBox.Show("Exportado com sucesso!", "Exportar arquivo json", MessageBoxButtons.OK)
        End If
    End Sub

    Private Sub APIToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles APIToolStripMenuItem.Click
        FormAPI.ShowDialog()
    End Sub

    Private Sub dgPortfolio_SelectionChanged(sender As Object, e As EventArgs) Handles dgPortfolio.SelectionChanged
        dgPortfolio.ClearSelection()
    End Sub
    Private Sub lbCaixa_Click(sender As Object, e As EventArgs) Handles lbCaixa.Click
        Dim posLabelNaTela As Point = lbCaixa.PointToScreen(Point.Empty)
        FormCaixa.Location = New Point(
            posLabelNaTela.X + (lbCaixa.Width - FormCaixa.Width) \ 2,
            posLabelNaTela.Y - FormCaixa.Height - 5 ' 5px de margem acima
        )
        FormCaixa.ShowDialog()
    End Sub

    Private Sub dgPortfolio_CellFormatting(sender As Object, e As DataGridViewCellFormattingEventArgs) Handles dgPortfolio.CellFormatting
        If e.RowIndex < 0 OrElse e.ColumnIndex < 0 Then
            Return
        End If

        Dim dgv = DirectCast(sender, DataGridView)
        Dim row = dgv.Rows(e.RowIndex)

        If e.Value Is Nothing OrElse e.Value Is DBNull.Value Then
            Return
        End If

        Dim valor As Decimal

        If TypeOf e.Value Is Decimal Then
            valor = DirectCast(e.Value, Decimal)
        ElseIf Not Decimal.TryParse(e.Value.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, valor) Then
            Dim texto As String = e.Value.ToString().Trim()
            If Not Decimal.TryParse(texto, NumberStyles.Number, CultureInfo.GetCultureInfo("pt-BR"), valor) Then
                Return
            End If
        End If

        Select Case dgv.Columns(e.ColumnIndex).Name

        ' ==========================================
        ' QTD
        ' ==========================================
            Case "Qtd"
                e.Value = valor.ToString("N2")
        ' ==========================================
        ' PREÇO MÉDIO
        ' ==========================================
            Case "precoMedio"
                e.Value = valor.ToString("C2", CultureInfo.GetCultureInfo("en-US"))
        ' ==========================================
        ' PREÇO ATUAL
        ' ==========================================
            Case "precoAtual"

                e.Value = valor.ToString("C4", CultureInfo.GetCultureInfo("en-US"))

                Dim precoMedio As Decimal = 0D
                If row.Cells("precoMedio").Value IsNot Nothing Then
                    precoMedio = Convert.ToDecimal(row.Cells("precoMedio").Value)
                End If

                If valor > precoMedio Then
                    e.CellStyle.ForeColor = Color.LightGreen
                ElseIf valor < precoMedio Then
                    e.CellStyle.ForeColor = Color.LightCoral
                Else
                    e.CellStyle.ForeColor = Color.WhiteSmoke
                End If

                e.FormattingApplied = True

        ' ==========================================
        ' 24 HORAS
        ' ==========================================
            Case "24horas"

                Dim texto As String =
                    e.Value.ToString().
                    Replace("%", "").
                    Replace(",", ".")

                If Decimal.TryParse(
                    texto,
                    NumberStyles.Any,
                    CultureInfo.InvariantCulture,
                    valor) Then

                    e.Value =
                        $"{valor:F2}%"

                    If valor > 0D Then

                        e.CellStyle.ForeColor =
                            Color.LimeGreen

                    ElseIf valor < 0D Then

                        e.CellStyle.ForeColor =
                            Color.Red
                    Else

                        e.CellStyle.ForeColor =
                            Color.FromArgb(20, 20, 20)

                    End If

                    e.FormattingApplied = True

                End If

        ' ==========================================
        ' MARKET CAP
        ' ==========================================
            Case "marketcap"
                valor.ToString("C2", CultureInfo.GetCultureInfo("en-US"))
        ' ==========================================
        ' QUANTIA ATUAL USD
        ' ==========================================
            Case "vlAtualUSD"
                e.Value = valor.ToString("C2", CultureInfo.GetCultureInfo("en-US"))
                e.FormattingApplied = True
        ' ==========================================
        ' QUANTIA ATUAL BRL
        ' ==========================================
            Case "vlAtualBRL"
                e.Value = valor.ToString("C2", CultureInfo.GetCultureInfo("pt-BR"))
        End Select
    End Sub
    Private Sub ImpermanetLossToolStripMenuItem_Click(sender As Object, e As EventArgs)
        FormPools.Show()
    End Sub

    Private Sub dgPortfolio_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgPortfolio.CellClick
        If dgPortfolio.Columns(e.ColumnIndex).Name = "Cripto" Then

            Dim valor As String = dgPortfolio.Rows(e.RowIndex).Cells(e.ColumnIndex).Value?.ToString()
            If Not String.IsNullOrEmpty(valor) Then
                Dim f As New FormBrowser(valor)
                f.Show()
            End If
        End If
    End Sub
    Private Sub ExportarPortfolioToolStripMenuItem_Click(sender As Object, e As EventArgs)

    End Sub

    Private Async Function LoadFuturesPositionsAsync() As Task
        If _futuresLoadRunning Then
            Return
        End If

        _futuresLoadRunning = True
        dgFuturos.Cursor = Cursors.WaitCursor

        Try
            Dim positions = Await B.BINANCE_GetFuturesPositionsAsync()
            Dim protectionOrders = Await B.BINANCE_GetFuturesProtectionOrdersAsync()
            Await EnsureFuturesUsdBrlRateAsync()
            Dim table As New DataTable()

            table.Columns.Add("Symbol", GetType(String))
            table.Columns.Add("PositionSide", GetType(String))
            table.Columns.Add("PositionAmount", GetType(Decimal))
            table.Columns.Add("Notional", GetType(Decimal))
            table.Columns.Add("InitialMargin", GetType(Decimal))
            table.Columns.Add("EntryPrice", GetType(Decimal))
            table.Columns.Add("MarkPrice", GetType(Decimal))
            table.Columns.Add("UnrealizedProfit", GetType(Decimal))
            table.Columns.Add("ROI", GetType(Decimal))
            table.Columns.Add("TakeProfit", GetType(String))
            table.Columns.Add("StopLoss", GetType(String))
            table.Columns.Add("LiquidationPrice", GetType(Decimal))
            table.Columns.Add("Leverage", GetType(Integer))

            For Each position In positions
                Dim roiMargin = GetFuturesEntryMargin(position.PositionAmount, position.EntryPrice, position.Leverage, position.InitialMargin)
                Dim roiPercent = CalculateFuturesRoi(position.UnrealizedProfit, roiMargin)

                Dim takeProfitText = GetProtectionText(protectionOrders, position, takeProfit:=True)
                Dim stopLossText = GetProtectionText(protectionOrders, position, takeProfit:=False)

                table.Rows.Add(
                    position.Symbol,
                    If(String.IsNullOrWhiteSpace(position.PositionSide), "-", position.PositionSide),
                    position.PositionAmount,
                    position.Notional,
                    roiMargin,
                    position.EntryPrice,
                    position.MarkPrice,
                    position.UnrealizedProfit,
                    roiPercent,
                    takeProfitText,
                    stopLossText,
                    position.LiquidationPrice,
                    position.Leverage)
            Next
            dgFuturos.DataSource = Nothing
            dgFuturos.DataSource = table
            ConfigureFuturesGrid()
            ApplyFuturesGridColors()
            UpdateFuturesPnlSummary()

            If Not _futuresUserStreamStarted Then
                Await _binanceFuturesUserWs.StartAsync()
                _futuresUserStreamStarted = True
            End If
            If positions.Count > 0 Then
                Dim symbolsKey = String.Join("|", positions.Select(Function(position) position.Symbol).Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(Function(symbol) symbol, StringComparer.OrdinalIgnoreCase))
                If Not String.Equals(symbolsKey, _futuresMarketSymbolsKey, StringComparison.OrdinalIgnoreCase) Then
                    Await _binanceFuturesMarketWs.StartAsync(positions.Select(Function(position) position.Symbol))
                    _futuresMarketSymbolsKey = symbolsKey
                End If
            ElseIf Not String.IsNullOrWhiteSpace(_futuresMarketSymbolsKey) Then
                Await _binanceFuturesMarketWs.StopAsync()
                _futuresMarketSymbolsKey = String.Empty
            End If

        Catch ex As Exception
            lbDebug.AppendText(
                Environment.NewLine &
                "Erro ao carregar posições de futuros: " & ex.Message)
        Finally
            dgFuturos.Cursor = Cursors.Default
            _futuresLoadRunning = False
        End Try
    End Function

    Private Async Function EnsureFuturesUsdBrlRateAsync() As Task
        If JSON.USDBRLprice > 0D Then Return

        Dim usdBrl As Decimal = 0D
        Try
            usdBrl = Await B.BINANCE_GetUSDTBRL()
        Catch ex As Exception
            Debug.WriteLine("[FUTURES] Binance USDT/BRL indisponível: " & ex.Message)
        End Try

        If usdBrl <= 0D AndAlso
           Not String.IsNullOrWhiteSpace(My.Settings.apiCMCKey) AndAlso
           Not String.IsNullOrWhiteSpace(My.Settings.activeAPI) Then
            Try
                Dim cmc As New Cotacao()
                usdBrl = Await cmc.CM_GetUSDBRL()
            Catch ex As Exception
                Debug.WriteLine("[FUTURES] CMC USDT/BRL indisponível: " & ex.Message)
            End Try
        End If

        If usdBrl > 0D Then
            JSON.USDBRLprice = usdBrl
        End If
    End Function

    Private Shared Function GetFuturesEntryMargin(positionAmount As Decimal, entryPrice As Decimal, leverage As Integer, fallbackMargin As Decimal) As Decimal
        If leverage > 0 AndAlso positionAmount <> 0D AndAlso entryPrice <> 0D Then
            Return Math.Abs(positionAmount * entryPrice) / leverage
        End If

        Return Math.Abs(fallbackMargin)
    End Function

    Private Shared Function CalculateFuturesRoi(unrealizedProfit As Decimal, entryMargin As Decimal) As Decimal
        If entryMargin = 0D Then Return 0D
        Return (unrealizedProfit / entryMargin) * 100D
    End Function
    Private Shared Function GetFuturesPositionSideColor(rawSide As Object) As Color
        Dim side = rawSide?.ToString().Trim()
        If String.Equals(side, "LONG", StringComparison.OrdinalIgnoreCase) Then
            Return Color.LimeGreen
        End If
        If String.Equals(side, "SHORT", StringComparison.OrdinalIgnoreCase) Then
            Return Color.Red
        End If
        Return Color.White
    End Function
    Private Shared Function GetFuturesProfitColor(rawValue As Object) As Color
        Dim value As Decimal
        If Not TryReadFuturesDecimal(rawValue, value) Then
            Return Color.WhiteSmoke
        End If

        If value > 0D Then
            Return Color.Lime
        End If
        If value < 0D Then
            Return Color.LightCoral
        End If

        Return Color.WhiteSmoke
    End Function
    Private Sub ApplyFuturesGridColors()
        For Each row As DataGridViewRow In dgFuturos.Rows
            If Not row.IsNewRow Then
                ApplyFuturesRowColors(row)
            End If
        Next
    End Sub

    Private Sub UpdateFuturesPnlSummary()
        Dim totalPnl As Decimal = 0D

        For Each row As DataGridViewRow In dgFuturos.Rows
            If row.IsNewRow Then Continue For

            Dim pnl As Decimal
            If TryReadFuturesDecimal(row.Cells("UnrealizedProfit").Value, pnl) Then
                totalPnl += pnl
            End If
        Next

        _futuresPnlUsd = totalPnl

        Dim pnlColor =
            If(totalPnl > 0D,
               Color.Lime,
               If(totalPnl < 0D, Color.LightCoral, Color.WhiteSmoke))

        lbFuturosUSD.Text = Cjson.USDformat(totalPnl)
        lbFuturosBRL.Text = Cjson.BRLformat(totalPnl * JSON.USDBRLprice)
        lbFuturosUSD.ForeColor = pnlColor
        lbFuturosBRL.ForeColor =
            If(totalPnl < 0D,
               Color.LightCoral,
               Color.CornflowerBlue)

        UpdateConsolidatedPnl()
    End Sub

    Private Sub UpdateConsolidatedPnl()
        Dim totalPnlUsd = _spotPnlUsd + _futuresPnlUsd
        Dim totalPnlBrl = totalPnlUsd * JSON.USDBRLprice

        lbTotalBRL.Visible = True
        lbTotalBRL.Text = Cjson.BRLformat(totalPnlBrl)
        lbTotalBRL.ForeColor =
            If(totalPnlBrl > 0D,
               Color.FromArgb(0, 255, 0),
               Color.FromArgb(255, 73, 73))
    End Sub

    Private Sub ApplyFuturesRowColors(row As DataGridViewRow)
        If row Is Nothing OrElse row.IsNewRow Then Return

        row.Cells("EntryPrice").Style.ForeColor = Color.Cyan
        row.Cells("EntryPrice").Style.SelectionForeColor = Color.Cyan

        Dim symbolColor = GetFuturesProfitColor(row.Cells("UnrealizedProfit").Value)
        row.Cells("Symbol").Style.ForeColor = symbolColor
        row.Cells("Symbol").Style.SelectionForeColor = symbolColor

        Dim sideColor = GetFuturesPositionSideColor(row.Cells("PositionSide").Value)
        row.Cells("PositionSide").Style.ForeColor = sideColor
        row.Cells("PositionSide").Style.SelectionForeColor = sideColor

        row.Cells("TakeProfit").Style.ForeColor = Color.DeepSkyBlue
        row.Cells("TakeProfit").Style.SelectionForeColor = Color.DeepSkyBlue
        For Each protectionColumn In New String() {"StopLoss", "LiquidationPrice"}
            row.Cells(protectionColumn).Style.ForeColor = Color.DarkOrange
            row.Cells(protectionColumn).Style.SelectionForeColor = Color.DarkOrange
        Next

        Dim entryPrice As Decimal
        Dim markPrice As Decimal
        If TryReadFuturesDecimal(row.Cells("MarkPrice").Value, markPrice) AndAlso
           TryReadFuturesDecimal(row.Cells("EntryPrice").Value, entryPrice) Then
            Dim side = row.Cells("PositionSide").Value?.ToString().Trim()
            Dim isShort = String.Equals(side, "SHORT", StringComparison.OrdinalIgnoreCase)
            Dim isLoss = If(isShort, markPrice > entryPrice, markPrice < entryPrice)
            Dim rowBackColor = If(isLoss, Color.FromArgb(25, 0, 0), Color.FromArgb(0, 25, 0))
            Dim priceColor = If(isLoss, Color.IndianRed, Color.LimeGreen)

            For Each cell As DataGridViewCell In row.Cells
                cell.Style.BackColor = rowBackColor
                cell.Style.SelectionBackColor = rowBackColor
            Next

            row.Cells("MarkPrice").Style.ForeColor = priceColor
            row.Cells("MarkPrice").Style.SelectionForeColor = priceColor
        End If

        For Each columnName In New String() {"UnrealizedProfit", "ROI"}
            Dim value As Decimal
            If TryReadFuturesDecimal(row.Cells(columnName).Value, value) Then
                Dim pnlColor = If(value > 0D, Color.LimeGreen, Color.Red)
                row.Cells(columnName).Style.ForeColor = pnlColor
                row.Cells(columnName).Style.SelectionForeColor = pnlColor
            End If
        Next
    End Sub
    Private Sub BinanceFuturesWs_ConnectionStateChanged(connected As Boolean, message As String)
        If IsDisposed OrElse Disposing Then Return

        If InvokeRequired Then
            BeginInvoke(New Action(Of Boolean, String)(AddressOf BinanceFuturesWs_ConnectionStateChanged), connected, message)
            Return
        End If

        If Not connected AndAlso lbDebug IsNot Nothing Then
            lbDebug.AppendText(Environment.NewLine & message)
        End If
    End Sub
    Private Sub BinanceFuturesMarkPriceUpdated(symbol As String, markPrice As Decimal)
        If IsDisposed OrElse Disposing Then Return

        If InvokeRequired Then
            BeginInvoke(New Action(Of String, Decimal)(AddressOf BinanceFuturesMarkPriceUpdated), symbol, markPrice)
            Return
        End If

        For Each row As DataGridViewRow In dgFuturos.Rows
            If row.IsNewRow OrElse Not String.Equals(row.Cells("Symbol").Value?.ToString(), symbol, StringComparison.OrdinalIgnoreCase) Then
                Continue For
            End If

            Dim quantity As Decimal
            Dim entryPrice As Decimal
            Dim initialMargin As Decimal
            Dim leverage As Integer
            If Not TryReadFuturesDecimal(row.Cells("PositionAmount").Value, quantity) OrElse
               Not TryReadFuturesDecimal(row.Cells("EntryPrice").Value, entryPrice) Then
                Continue For
            End If

            TryReadFuturesDecimal(row.Cells("InitialMargin").Value, initialMargin)
            Integer.TryParse(row.Cells("Leverage").Value?.ToString().Replace("x", String.Empty), leverage)
            Dim quantityAbs = Math.Abs(quantity)
            Dim positionSide = row.Cells("PositionSide").Value?.ToString()
            Dim isShort = String.Equals(positionSide, "SHORT", StringComparison.OrdinalIgnoreCase) OrElse quantity < 0D
            Dim pnl = If(isShort, (entryPrice - markPrice) * quantityAbs, (markPrice - entryPrice) * quantityAbs)
            Dim notional = quantityAbs * markPrice
            Dim entryMargin = GetFuturesEntryMargin(quantityAbs, entryPrice, leverage, initialMargin)
            Dim roi = CalculateFuturesRoi(pnl, entryMargin)

            row.Cells("MarkPrice").Value = markPrice
            row.Cells("Notional").Value = notional
            row.Cells("InitialMargin").Value = entryMargin
            row.Cells("UnrealizedProfit").Value = pnl
            row.Cells("ROI").Value = roi
            ApplyFuturesRowColors(row)
            UpdateFuturesPnlSummary()
            dgFuturos.InvalidateRow(row.Index)
            Exit For
        Next
    End Sub

    Private Async Sub BinanceFuturesUserDataUpdated()
        If dgFuturos.Visible = False Then Return

        If InvokeRequired Then
            BeginInvoke(New MethodInvoker(Async Sub() Await LoadFuturesPositionsAsync()))
            Return
        End If

        Await LoadFuturesPositionsAsync()
    End Sub
    Private Shared Function GetProtectionText(
        orders As List(Of BinanceFuturesProtectionOrder),
        position As BinanceFuturesPosition,
        takeProfit As Boolean) As String

        Dim prices = orders.
            Where(Function(order)
                      If Not order.Symbol.Equals(position.Symbol, StringComparison.OrdinalIgnoreCase) Then
                          Return False
                      End If

                      If Not String.IsNullOrWhiteSpace(order.PositionSide) AndAlso
                         Not order.PositionSide.Equals("BOTH", StringComparison.OrdinalIgnoreCase) AndAlso
                         Not position.PositionSide.Equals("BOTH", StringComparison.OrdinalIgnoreCase) AndAlso
                         Not order.PositionSide.Equals(position.PositionSide, StringComparison.OrdinalIgnoreCase) Then
                          Return False
                      End If

                      Dim isTakeProfitOrder = order.OrderType.Contains("TAKE_PROFIT", StringComparison.OrdinalIgnoreCase)
                      Return If(takeProfit, isTakeProfitOrder, Not isTakeProfitOrder)
                  End Function).
            Select(Function(order) If(order.StopPrice <> 0D, order.StopPrice, order.Price)).
            Where(Function(price) price > 0D).
            Distinct().
            OrderBy(Function(price) price).
            Select(Function(price) "$" & price.ToString("N2", CultureInfo.GetCultureInfo("en-US"))).
            ToList()

        If prices.Count = 0 Then
            Return "-"
        End If

        Return String.Join(" / ", prices)
    End Function
    Private Sub ConfigureFuturesGrid()
        Dim headers As New Dictionary(Of String, String) From {
            {"Symbol", "Cripto"},
            {"PositionSide", "Lado"},
            {"PositionAmount", "Quantidade"},
            {"Notional", "Notional"},
            {"InitialMargin", "Margem (USDT)"},
            {"EntryPrice", "Entrada"},
            {"MarkPrice", "Preço atual"},
            {"TakeProfit", "TP"},
            {"StopLoss", "SL"},
            {"UnrealizedProfit", "PnL não realizado"},
            {"ROI", "ROI %"},
            {"LiquidationPrice", "Liquidação"},
            {"Leverage", "Alavancagem"}
        }
        For Each item In headers
            If dgFuturos.Columns.Contains(item.Key) Then
                dgFuturos.Columns(item.Key).HeaderText = item.Value
            End If
        Next

        If dgFuturos.Columns.Contains("Symbol") Then
            dgFuturos.Columns("Symbol").MinimumWidth = 120
        End If

        If dgFuturos.Columns.Contains("UnrealizedProfit") Then
            dgFuturos.Columns("UnrealizedProfit").MinimumWidth = 135
        End If
        If dgFuturos.Columns.Contains("InitialMargin") Then
            dgFuturos.Columns("InitialMargin").Visible = True
        End If

        dgFuturos.Font = New Font("Calibri", 12.0F, FontStyle.Regular)
        dgFuturos.DefaultCellStyle.Font = dgFuturos.Font
        dgFuturos.RowHeadersDefaultCellStyle.Font = dgFuturos.Font
        dgFuturos.RowTemplate.Height = 35
        dgFuturos.ColumnHeadersHeight = 40
        dgFuturos.ColumnHeadersDefaultCellStyle.Font = New Font("Calibri", 10.0F, FontStyle.Italic)
        For Each row As DataGridViewRow In dgFuturos.Rows
            If Not row.IsNewRow Then
                row.Height = 35
            End If
        Next
    End Sub

    Private Shared Function TryReadFuturesDecimal(rawValue As Object, ByRef result As Decimal) As Boolean
        If rawValue Is Nothing OrElse rawValue Is DBNull.Value Then
            Return False
        End If

        If TypeOf rawValue Is Decimal Then
            result = DirectCast(rawValue, Decimal)
            Return True
        End If

        Dim textValue = rawValue.ToString().Replace("%", String.Empty).Replace("$", String.Empty).Trim()

        If Decimal.TryParse(
            textValue,
            NumberStyles.Float,
            CultureInfo.InvariantCulture,
            result) Then
            Return True
        End If

        Return Decimal.TryParse(
            textValue,
            NumberStyles.Number,
            CultureInfo.GetCultureInfo("pt-BR"),
            result)
    End Function
    Private Sub dgFuturos_CellFormatting(
        sender As Object,
        e As DataGridViewCellFormattingEventArgs) Handles dgFuturos.CellFormatting

        If e.RowIndex < 0 OrElse e.ColumnIndex < 0 OrElse e.Value Is Nothing Then
            Return
        End If

        Dim columnName = dgFuturos.Columns(e.ColumnIndex).Name
        Dim value As Decimal
        Dim leverageValue As Integer

        Select Case columnName
            Case "Symbol"
                Dim symbolColor = GetFuturesProfitColor(dgFuturos.Rows(e.RowIndex).Cells("UnrealizedProfit").Value)
                e.CellStyle.ForeColor = symbolColor
                e.CellStyle.SelectionForeColor = symbolColor
                e.FormattingApplied = True
            Case "PositionSide"
                Dim sideColor = GetFuturesPositionSideColor(e.Value)
                e.CellStyle.ForeColor = sideColor
                e.CellStyle.SelectionForeColor = sideColor
                e.FormattingApplied = True
            Case "PositionAmount"
                If TryReadFuturesDecimal(e.Value, value) Then
                    e.Value = value.ToString("N2", CultureInfo.GetCultureInfo("en-US"))
                    e.FormattingApplied = True
                End If
            Case "EntryPrice"
                If TryReadFuturesDecimal(e.Value, value) Then
                    e.Value = "$" & value.ToString("N2", CultureInfo.GetCultureInfo("en-US"))
                    e.CellStyle.ForeColor = Color.Cyan
                    e.CellStyle.SelectionForeColor = Color.Cyan
                    e.FormattingApplied = True
                End If
            Case "MarkPrice"
                If TryReadFuturesDecimal(e.Value, value) Then
                    e.Value = "$" & value.ToString("N3", CultureInfo.GetCultureInfo("en-US"))
                    Dim entryPrice As Decimal
                    If TryReadFuturesDecimal(dgFuturos.Rows(e.RowIndex).Cells("EntryPrice").Value, entryPrice) AndAlso value > entryPrice Then
                        e.CellStyle.ForeColor = Color.LimeGreen
                        e.CellStyle.SelectionForeColor = Color.LimeGreen
                    Else
                        e.CellStyle.ForeColor = Color.Red
                        e.CellStyle.SelectionForeColor = Color.Red
                    End If
                    e.FormattingApplied = True
                End If
            Case "LiquidationPrice"
                If TryReadFuturesDecimal(e.Value, value) Then
                    e.Value = "$" & value.ToString("N2", CultureInfo.GetCultureInfo("en-US"))
                    e.CellStyle.ForeColor = Color.DarkOrange
                    e.CellStyle.SelectionForeColor = Color.DarkOrange
                    e.FormattingApplied = True
                End If
            Case "TakeProfit"
                e.CellStyle.ForeColor = Color.DeepSkyBlue
                e.CellStyle.SelectionForeColor = Color.DeepSkyBlue
            Case "StopLoss"
                e.CellStyle.ForeColor = Color.DarkOrange
                e.CellStyle.SelectionForeColor = Color.DarkOrange
            Case "UnrealizedProfit", "ROI", "Notional", "InitialMargin"
                If TryReadFuturesDecimal(e.Value, value) Then
                    If columnName = "ROI" Then
                        e.Value = value.ToString("N2", CultureInfo.GetCultureInfo("en-US")) & "%"
                    Else
                        e.Value = "$" & value.ToString("N2", CultureInfo.GetCultureInfo("en-US"))
                    End If
                    If columnName = "UnrealizedProfit" OrElse columnName = "ROI" Then
                        Dim pnlColor = If(value > 0D, Color.LimeGreen, Color.Red)
                        e.CellStyle.ForeColor = pnlColor
                        e.CellStyle.SelectionForeColor = pnlColor
                    End If
                    e.FormattingApplied = True
                End If
            Case "Leverage"
                If Integer.TryParse(e.Value.ToString, leverageValue) Then
                    e.Value = $"{leverageValue:0}x"
                    e.FormattingApplied = True
                End If
        End Select
    End Sub
    Private Sub dgFuturos_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgFuturos.CellClick
        If e.RowIndex < 0 OrElse e.ColumnIndex < 0 OrElse
           Not String.Equals(dgFuturos.Columns(e.ColumnIndex).Name, "Symbol", StringComparison.OrdinalIgnoreCase) Then
            Return
        End If

        Dim symbol = dgFuturos.Rows(e.RowIndex).Cells("Symbol").Value?.ToString().Trim()
        If String.IsNullOrWhiteSpace(symbol) Then Return

        If symbol.EndsWith("USDT", StringComparison.OrdinalIgnoreCase) Then
            symbol = symbol.Substring(0, symbol.Length - 4)
        End If

        If Not String.IsNullOrWhiteSpace(symbol) Then
            Dim f As New FormBrowser(symbol)
            f.Show()
        End If
    End Sub
    Private Async Sub FormMain_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing

        Try
            Await _binanceWs.StopAsync()
            Await _gateWs.StopAsync()
            Await _binanceFuturesMarketWs.StopAsync()
            Await _binanceFuturesUserWs.StopAsync()
        Catch
        End Try
    End Sub
    Private Sub btSpot_Click(sender As Object, e As EventArgs) Handles btSpot.Click
        dgPortfolio.Visible = True
        dgFuturos.Visible = False
        btFuturos.BackColor = Color.FromArgb(20, 20, 20)
        btSpot.BackColor = Color.SteelBlue
    End Sub
    Private Sub btFuturos_Click(sender As Object, e As EventArgs) Handles btFuturos.Click
        dgPortfolio.Visible = False
        dgFuturos.Visible = True
        btFuturos.BackColor = Color.SteelBlue
        btSpot.BackColor = Color.FromArgb(20, 20, 20)
    End Sub

End Class
