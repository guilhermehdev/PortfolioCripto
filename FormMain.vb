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

    Private Async Sub SetupLabels()
        Dim json As New JSON
        Dim usdTask As New Cotacao
        Dim usdValue = Await usdTask.GetUSDBRL
        Dim dom As Decimal? = Await usdTask.GetBTCDOM()
        Dim total = Await json.LoadCriptos(dgPortfolio)

        lbDolar.Text = $"R$ {Format(usdValue, "#,##0.00")}"
        lbBTC.Text = json.USDformat(Await usdTask.GetCriptoPrices("BTC"))
        lbDom.Text = $"{dom.Value:F2}%"
        lbTotalBRL.Text = json.BRLformat(total * usdValue)

    End Sub

    Private Sub btRefresh_Click(sender As Object, e As EventArgs)
        Try
            Form1_Load(sender, e)
        Catch ex As Exception

        End Try
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
        Catch ex As Exception

        End Try

    End Sub
End Class
