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

        Dim myChart As New Chart With {
            .Width = 200,
            .Height = 100,
            .Top = 5,
            .Left = 700
        }

        ' Adicionar uma área de gráfico
        Dim chartArea As New ChartArea("MainArea")
        myChart.ChartAreas.Add(chartArea)

        ' Criar uma série para os dados
        Dim series As New Series("Vendas")
        series.ChartType = SeriesChartType.Column ' Definir o tipo de gráfico (barra neste caso)
        series.Points.AddXY("Janeiro", 100)
        series.Points.AddXY("Fevereiro", 150)
        series.Points.AddXY("Março", 200)
        series.Points.AddXY("Abril", 250)

        ' Adicionar a série ao gráfico
        myChart.Series.Add(series)

        ' Adicionar título ao gráfico (opcional)
        Dim chartTitle As New Title("Gráfico de Vendas", Docking.Top, New Font("Arial", 10), Color.Black)
        myChart.Titles.Add(chartTitle)

        Me.GroupOverview.Controls.Add(myChart)

    End Sub

    Public Async Sub Setup()
        Dim Cjson As New JSON

        lbLoadFromMarket.Visible = True
        TimerBlink.Start()
        Cursor = Cursors.WaitCursor
        dgPortfolio.Cursor = Cursors.WaitCursor

        Await Cjson.LoadCriptos(dgPortfolio)

        TotalAdjust()

    End Sub

    Private Sub dgPortfolio_MouseLeave(sender As Object, e As EventArgs) Handles dgPortfolio.MouseLeave
        dgPortfolio.ClearSelection()
        dgPortfolio.CurrentCell = Nothing
    End Sub

    Private Sub GroupBox1_Paint(sender As Object, e As PaintEventArgs) Handles GroupOverview.Paint
        e.Graphics.Clear(Me.BackColor)
        Dim text As String = GroupOverview.Text
        If Not String.IsNullOrEmpty(text) Then
            e.Graphics.DrawString(text, GroupOverview.Font, New SolidBrush(GroupOverview.ForeColor), 10, 0)
        End If
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

            TotalAdjust()

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

            Setup()
            json.FormatGrid(dgPortfolio)
            Me.remainingtimeInSeconds = TimerRefresh.Interval / 1000
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

    Private Sub TotalAdjust()
        lbTotalBRL.Location = New Point((PanelProfits.Width / 2) - (lbTotalBRL.Width / 2), 3)
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

End Class
