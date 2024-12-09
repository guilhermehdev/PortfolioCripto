Imports System.Windows.Forms.DataVisualization.Charting

Public Class FormMain
    Public remainingtimeInSeconds As Integer

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
        Dim Cjson As New JSON

        lbLoadFromMarket.Visible = True
        TimerBlink.Start()
        Cursor = Cursors.WaitCursor
        dgPortfolio.Cursor = Cursors.WaitCursor
        Await Cjson.LoadCriptos(dgPortfolio)
        dgPortfolio.Sort(dgPortfolio.Columns("ROIusd"), System.ComponentModel.ListSortDirection.Descending)
        Adjust()
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

        Catch ex As Exception

        End Try

    End Sub

    Private Sub btRefresh_Click_1(sender As Object, e As EventArgs) Handles btRefresh.Click
        Try
            Setup()
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
    End Sub

    Private Sub IntervaloToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles IntervaloToolStripMenuItem.Click
        FormIntervalo.ShowDialog()
    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles TimerRefresh.Tick
        Dim json As New JSON
        Try
            json.FormatGrid(dgPortfolio)
            Me.remainingtimeInSeconds = TimerRefresh.Interval / 1000
            Setup()
        Catch ex As Exception

        End Try

    End Sub
    Private Sub TimerCountdown_Tick(sender As Object, e As EventArgs) Handles TimerCountdown.Tick
        remainingtimeInSeconds -= 1
        lbAtualizaEm.Visible = True
        lbRefresh.Visible = True
        lbRefresh.Text = $"{(remainingtimeInSeconds \ 60).ToString("D2")}:{(remainingtimeInSeconds Mod 60).ToString("D2")}"
    End Sub

    Private Sub NotifyIcon1_MouseMove(sender As Object, e As MouseEventArgs) Handles NotifyIcon1.MouseMove
        NotifyIcon1.Text = "BTC: " & lbBTC.Text
    End Sub

    Private Sub Adjust()
        lbTotalBRL.Location = New Point((PanelProfits.Width / 2) - (lbTotalBRL.Width / 2), 3)
        PanelGraphs.Width = Me.Width
        If dgPortfolio.RowCount < 10 Then
            dgPortfolio.Height = (dgPortfolio.RowCount * 35) + 43
        Else
            dgPortfolio.Height = 350
        End If
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

        gCriptos.collumGraph(440, 190, -2, 400, "Criptos", "Criptos", 10, Color.Aqua, Color.FromArgb(30, 30, 30), SeriesChartType.Column, criptoDic, PanelGraphs)
    End Sub

    Public Sub addressGraph(criptoDic As Dictionary(Of String, Decimal))
        Dim gCriptos As New Charts

        gCriptos.pieGraph(350, 190, -2, 830, "Wallets", 10, Color.Aqua, Color.FromArgb(30, 30, 30), criptoDic, 7.5, Color.White, PanelGraphs)
    End Sub

    Private Sub dgPortfolio_CellPainting(sender As Object, e As DataGridViewCellPaintingEventArgs) Handles dgPortfolio.CellPainting
        If e.RowIndex >= 0 Then

            ' Pinta o fundo e o texto padrão
            e.PaintBackground(e.CellBounds, True)
            e.PaintContent(e.CellBounds)

            Using pen As New Pen(Color.FromArgb(50, 50, 50), 0.5)
                Dim rect = e.CellBounds
                Dim y = rect.Bottom - 1 ' Posição da linha inferior da célula
                e.Graphics.DrawLine(pen, rect.Left, y, rect.Right, y)
            End Using

            Dim colunasComLinhasVerticais As Integer() = {0} ' Índices das colunas que terão linhas verticais
            If colunasComLinhasVerticais.Contains(e.ColumnIndex) Then
                Using pen As New Pen(Color.FromArgb(65, 65, 65), 1)
                    Dim rect = e.CellBounds
                    Dim x = rect.Right - 1 ' Posição da borda direita da célula
                    e.Graphics.DrawLine(pen, x, rect.Top, x, rect.Bottom)
                End Using
            End If

            e.Handled = True ' Impede o desenho padrão

        End If

    End Sub

    Private Sub pbUSD_Click(sender As Object, e As EventArgs) Handles pbUSD.Click
        dgPortfolio.Columns(4).Visible = True
        dgPortfolio.Columns(8).Visible = True
        dgPortfolio.Columns(10).Visible = True

        dgPortfolio.Columns(5).Visible = False
        dgPortfolio.Columns(9).Visible = False
        dgPortfolio.Columns(11).Visible = False

    End Sub

    Private Sub pbBRL_Click(sender As Object, e As EventArgs) Handles pbBRL.Click
        dgPortfolio.Columns(4).Visible = False
        dgPortfolio.Columns(8).Visible = False
        dgPortfolio.Columns(10).Visible = False

        dgPortfolio.Columns(5).Visible = True
        dgPortfolio.Columns(9).Visible = True
        dgPortfolio.Columns(11).Visible = True

    End Sub

End Class
