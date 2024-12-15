Public Class FormIntervalo
    Public timerRefresh As Timer = FormMain.TimerRefresh
    Public timerCount As Timer = FormMain.TimerCountdown
    Private Sub btOkIntervalo_Click(sender As Object, e As EventArgs) Handles btOkIntervalo.Click
        Select Case cbIntervalo.SelectedIndex
            Case 0
                timerRefresh.Interval = 60000
            Case 1
                timerRefresh.Interval = 300000
            Case 2
                timerRefresh.Interval = 600000
            Case 3
                timerRefresh.Interval = 900000
            Case 4
                timerRefresh.Interval = 1200000.0
            Case 5
                timerRefresh.Interval = 1500000.0
            Case 6
                timerRefresh.Interval = 1800000.0
            Case 7
                timerRefresh.Interval = 2700000.0
            Case 8
                timerRefresh.Interval = 3600000.0
        End Select

        timerRefresh.Start()
        timerCount.Interval = 1000
        timerCount.Start()
        FormMain.lbAtualizaEm.Text = "Atualizado em:"
        FormMain.lbRefresh.Text = My.Settings.lastView

        FormMain.remainingtimeInSeconds = timerRefresh.Interval / 1000

        Me.Close()

    End Sub
    Private Sub FormIntervalo_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        timerRefresh.Stop()
        timerCount.Stop()
        cbIntervalo.SelectedIndex = 0
    End Sub
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles btCancelTimer.Click
        timerRefresh.Stop()
        timerCount.Stop()
        FormMain.lbAtualizaEm.Text = "Atualizado em:"
        FormMain.lbRefresh.Text = My.Settings.lastView
        Me.Close()
    End Sub
End Class