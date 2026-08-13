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

            AddHandler _binanceWs.PriceUpdated,
            AddressOf BinanceWs_PriceUpdated

            AddHandler _binanceWs.ConnectionStateChanged,
            AddressOf BinanceWs_ConnectionStateChanged

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

    Private Sub UpdateRealtimeOverview()

        Try

            Dim totalEntradaUSD As Decimal = 0D
            Dim totalAtualUSD As Decimal = 0D

            Dim cashflowUSD As Decimal = 0D
            Dim investidoUSD As Decimal = 0D

            Dim lucroUSD As Decimal = 0D

            For Each row As DataGridViewRow In dgPortfolio.Rows

                If row.IsNewRow OrElse Not row.Visible Then
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

            Dim lucroBRL As Decimal =
            lucroUSD * usdBrl

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
            Me.lbTotalBRL.Text =
            Cjson.BRLformat(lucroBRL)

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

            ' Mantém o estilo visual
            Cjson.FormatGrid(dgPortfolio)

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

        If Me.InvokeRequired Then

            Me.BeginInvoke(
            New Action(
                Sub()
                    UpdateBinanceRow(
                        symbol,
                        price)
                End Sub))

            Return

        End If

        UpdateBinanceRow(
        symbol,
        price)

    End Sub

    Private Sub dgPortfolio_MouseLeave(sender As Object, e As EventArgs) Handles dgPortfolio.MouseLeave
        dgPortfolio.ClearSelection()
        dgPortfolio.CurrentCell = Nothing
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
            chart.removeCharts()
            lbLoadFromMarket.Visible = True
            TimerBlink.Start()
            Cursor = Cursors.WaitCursor
            dgPortfolio.Cursor = Cursors.WaitCursor

            'Await Cjson.LoadCriptos(dgPortfolio)
            If Await Cjson.LoadCriptos(dgPortfolio) Then
                Await StartBinanceWebSocket()
                lbDebug.Clear()
                lbDebug.AppendText("Status: Online")
                changeOnOffColor("Online")
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
        End Try

    End Function

    Private Async Sub btRefresh_Click_1Async(sender As Object, e As EventArgs) Handles btRefresh.Click
        Await refreshMarket()
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
            json.FormatGrid(dgPortfolio)
            Me.remainingtimeInSeconds = TimerRefresh.Interval / 1000
            chart.removeCharts()
            lbLoadFromMarket.Visible = True
            TimerBlink.Start()

            Cursor = Cursors.WaitCursor
            dgPortfolio.Cursor = Cursors.WaitCursor
            Await Cjson.LoadCriptos(dgPortfolio)
            dgPortfolio.Sort(dgPortfolio.Columns("ROIusd"), System.ComponentModel.ListSortDirection.Descending)
            Adjust()

            lbAtualizaEm.Text = "Atualizado em:"
            lbRefresh.Text = My.Settings.lastView
            lbRefresh.Location = New Point(125, 7)
        Catch ex As Exception

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
        dgPortfolio.Height = (dgPortfolio.RowCount * 35)
        Me.Height = MenuStrip1.Height + dgPortfolio.Height + PanelGraphs.Height + PanelProfits.Height + panelDebug.Height + 65
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
        Dim dgv = DirectCast(sender, DataGridView)

        If dgv.Columns(e.ColumnIndex).Name = "24horas" AndAlso e.Value IsNot Nothing Then
            Dim valorDecimal As Decimal

            ' Garante conversão segura
            If Decimal.TryParse(e.Value.ToString().Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, valorDecimal) Then
                e.Value = $"{valorDecimal:F2}%"
            End If
        End If
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

    Private Async Sub FormMain_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing

        Try
            Await _binanceWs.StopAsync()
        Catch
        End Try
    End Sub
End Class
