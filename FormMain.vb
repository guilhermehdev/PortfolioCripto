Imports System.Globalization

Public Class FormMain

    Private Sub CriptoToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CriptoToolStripMenuItem.Click
        FormEntradas.Show()
    End Sub

    Private Sub FecharToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles FecharToolStripMenuItem.Click
        Application.Exit()
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        SetupLabels()
    End Sub

    Public Async Sub SetupLabels()
        Dim json As New JSON
        Dim usdTask As New Cotacao
        Dim usdValue = Await usdTask.GetUSDBRL
        Dim dom As Decimal? = Await usdTask.GetBTCDOM()
        Dim total = Await json.LoadCriptos(dgPortfolio)

        lbDolar.Text = $"R$ {Format(usdValue, "#,##0.00")}"
        lbBTC.Text = json.USDformat(Await usdTask.GetCriptoPrices("BTC"))
        lbDom.Text = $"{dom.Value:F2}%"
        lbTotalBRL.Text = json.BRLformat(total * usdValue)
        If total > 0 Then
            lbTotalBRL.ForeColor = Color.FromArgb(0, 255, 0)
        Else
            lbTotalBRL.ForeColor = Color.FromArgb(255, 73, 73)
        End If

    End Sub

    Private Sub dgPortfolio_MouseLeave(sender As Object, e As EventArgs) Handles dgPortfolio.MouseLeave
        dgPortfolio.ClearSelection()
        dgPortfolio.CurrentCell = Nothing
    End Sub

    Private Sub GroupBox1_Paint(sender As Object, e As PaintEventArgs) Handles GroupBox1.Paint
        e.Graphics.Clear(Me.BackColor)
        Dim text As String = GroupBox1.Text
        If Not String.IsNullOrEmpty(text) Then
            e.Graphics.DrawString(text, GroupBox1.Font, New SolidBrush(GroupBox1.ForeColor), 10, 0)
        End If
    End Sub
    Private Sub FormMain_Resize(sender As Object, e As EventArgs) Handles MyBase.Resize
        Dim json As New JSON
        Try
            json.FormatGrid(dgPortfolio)
            lbTotalBRL.Location = New Point((PanelProfits.Width / 2) - (lbTotalBRL.Width / 2), 3)

            If Me.WindowState = FormWindowState.Minimized Then
                Me.Hide()
                NotifyIcon1.Visible = True
                NotifyIcon1.ShowBalloonTip(1000, "Programa Minimizou", "O programa foi minimizado para a bandeja do sistema.", ToolTipIcon.Info)
            End If
        Catch ex As Exception

        End Try

    End Sub

    Private Sub btRefresh_Click_1(sender As Object, e As EventArgs) Handles btRefresh.Click
        Try
            Form1_Load(sender, e)
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
End Class
