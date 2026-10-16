Public Class FormCaixa
    Dim j As New JSON
    Private Sub FormCaixa_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        j.loadCaixa(dgCaixa)
    End Sub

    Private Sub FormCaixa_Shown(sender As Object, e As EventArgs) Handles MyBase.Shown
        ' Recarrega também quando a mesma janela é aberta novamente após uma
        ' atualização do WebSocket Spot.
        j.loadCaixa(dgCaixa)
    End Sub

    Private Sub btCancelTimer_Click(sender As Object, e As EventArgs) Handles btCancelTimer.Click
        Me.Close()
    End Sub

End Class
