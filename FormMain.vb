Imports System.IO
Imports System.Windows.Forms.DataVisualization.Charting
Imports Newtonsoft.Json.Linq

Public Class FormMain
    Public remainingtimeInSeconds As Integer
    Dim Cjson As New JSON
    Dim chart As New Charts

    Private Sub CriptoToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CriptoToolStripMenuItem.Click
        FormEntradas.Show()
    End Sub

    Private Sub FecharToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles FecharToolStripMenuItem.Click
        Application.Exit()
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Setup()
        lbDataTotalToday.Text = Date.Today & ":"
    End Sub

    Public Async Sub Setup()

        Try
            If Await Cjson.checkLastUpdateOnJSONBin() Then

                chart.removeCharts()
                lbLoadFromMarket.Visible = True
                TimerBlink.Start()

                Cursor = Cursors.WaitCursor
                dgPortfolio.Cursor = Cursors.WaitCursor
                Await Cjson.LoadCriptos(dgPortfolio)
                dgPortfolio.Sort(dgPortfolio.Columns("ROIusd"), System.ComponentModel.ListSortDirection.Descending)
                Adjust()

                lbAtualizaEm.Text = "Atualizado em:"
                lbRefresh.Location = New Point(125, 7)
                lbRefresh.Text = My.Settings.lastView
            End If

        Catch ex As Exception

        End Try


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
                NotifyIcon1.ShowBalloonTip(3000, "Porfólio Cripto", lbBTC.Text, ToolTipIcon.Info)
            End If

            Adjust()

            Me.CenterToScreen()

        Catch ex As Exception

        End Try

    End Sub

    Private Async Sub btRefresh_Click_1Async(sender As Object, e As EventArgs) Handles btRefresh.Click
        Try
            chart.removeCharts()
            lbLoadFromMarket.Visible = True
            TimerBlink.Start()

            Cursor = Cursors.WaitCursor
            dgPortfolio.Cursor = Cursors.WaitCursor
            Await Cjson.LoadCriptos(dgPortfolio)
            dgPortfolio.Sort(dgPortfolio.Columns("ROIusd"), System.ComponentModel.ListSortDirection.Descending)
            Adjust()

            lbAtualizaEm.Text = "Atualizado em:"
            lbRefresh.Location = New Point(125, 7)
            lbRefresh.Text = My.Settings.lastView

            TimerCountdown.Stop()
            TimerRefresh.Stop()
        Catch ex As Exception

        End Try
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

    Private Async Function Timer1_TickAsync(sender As Object, e As EventArgs) As Task Handles TimerRefresh.Tick
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

    End Function
    Private Sub TimerCountdown_Tick(sender As Object, e As EventArgs) Handles TimerCountdown.Tick
        remainingtimeInSeconds -= 1
        lbAtualizaEm.Text = "Atualiza em:"
        lbRefresh.Location = New Point(112, 7)
        lbRefresh.Text = $"{(remainingtimeInSeconds \ 60).ToString("D2")}:{(remainingtimeInSeconds Mod 60).ToString("D2")}"
    End Sub

    Private Sub NotifyIcon1_MouseMove(sender As Object, e As MouseEventArgs) Handles NotifyIcon1.MouseMove
        NotifyIcon1.Text = "BTC: " & lbBTC.Text
    End Sub

    Private Sub Adjust()
        lbTotalBRL.Location = New Point((PanelProfits.Width / 2) - (lbTotalBRL.Width / 2), 3)
        PanelGraphs.Width = Me.Width
        'If dgPortfolio.RowCount < 10 Then
        dgPortfolio.Height = (dgPortfolio.RowCount * 35) + 43
        'Else
        'dgPortfolio.Height = 350
        'End If
        Me.Height = MenuStrip1.Height + dgPortfolio.Height + PanelGraphs.Height + PanelProfits.Height + 60

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

End Class
