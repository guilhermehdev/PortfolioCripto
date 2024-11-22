Imports System.Globalization

Public Class FormMain

    Private Sub CriptoToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CriptoToolStripMenuItem.Click
        FormEntradas.Show()
    End Sub

    Private Sub FecharToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles FecharToolStripMenuItem.Click
        Application.Exit()
    End Sub

    Private Async Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim json As New JSON
        Dim usdTask As New Cotacao
        Dim usdValue = Await usdTask.GetUSDBRL
        Dim dom As Decimal? = Await usdTask.GetBTCDOM()
        Dim total = Await json.LoadCriptos(dgPortfolio)

        lbDolar.Text = $"R$ {Format(usdValue, "#,##0.00")}"
        lbBTC.Text = json.USDformat(Await usdTask.GetCriptoPrices("BTC"))
        lbDom.Text = $"{dom.Value:F2}%"

        Label2.Text = json.USDformat(total)
        Label2.Location = New Point((PanelProfits.Width / 2) - (Label2.Width) - (lbDiv.Width / 2), 3)
        lbTotalBRL.Text = json.BRLformat(total)
        lbDiv.Location = New Point(Label2.Location.X + Label2.Width, 3)
        lbTotalBRL.Location = New Point(lbDiv.Location.X + lbDiv.Width, 3)

    End Sub
    Private Sub btRefresh_Click(sender As Object, e As EventArgs) Handles btRefresh.Click
        Try
            Me.Form1_Load(sender, e)
        Catch ex As Exception

        End Try
    End Sub

End Class
