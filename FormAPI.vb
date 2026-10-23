Public Class FormAPI
    Private Sub FormAPI_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        If Not My.Settings.apiCMCKey = Nothing Then
            tbAPIKey.Text = My.Settings.apiCMCKey
        End If
        If Not My.Settings.apiUrl = Nothing Then
            tbAPIURLPro.Text = My.Settings.apiUrl
        End If
        If Not My.Settings.apiUrlSandbox = Nothing Then
            tbURLSandbox.Text = My.Settings.apiUrlSandbox
        End If
        If Not My.Settings.activeAPI = Nothing Then
            cbAPIActive.Text = My.Settings.activeAPI
        End If

        cbAPIActive.Items.Clear()
        cbAPIActive.Items.Add(My.Settings.apiUrl)
        cbAPIActive.Items.Add(My.Settings.apiUrlSandbox)
        If Not String.IsNullOrWhiteSpace(My.Settings.activeAPI) Then
            cbAPIActive.SelectedItem = My.Settings.activeAPI
            If cbAPIActive.SelectedIndex < 0 Then cbAPIActive.Text = My.Settings.activeAPI
        ElseIf cbAPIActive.Items.Count > 0 Then
            cbAPIActive.SelectedIndex = 0
        End If

    End Sub

    Private Sub btSaveActiveAPI_Click(sender As Object, e As EventArgs) Handles btSaveActiveAPI.Click
        If Not String.IsNullOrWhiteSpace(cbAPIActive.Text) Then
            If Not tbAPIKey.Text = "" Then
                My.Settings.apiCMCKey = tbAPIKey.Text
                My.Settings.Save()
            Else
                MessageBox.Show("Não pode ser vazio!")
                tbAPIKey.Focus()
                Return
            End If

            If Not tbAPIURLPro.Text = "" Then
                My.Settings.apiUrl = tbAPIURLPro.Text
                My.Settings.Save()
            Else
                MessageBox.Show("Não pode ser vazio!")
                tbAPIURLPro.Focus()
                Return
            End If

            If Not tbURLSandbox.Text = "" Then
                My.Settings.apiUrlSandbox = tbURLSandbox.Text
                My.Settings.Save()
            Else
                MessageBox.Show("Não pode ser vazio!")
                tbURLSandbox.Focus()
                Return
            End If

        Else
            MessageBox.Show("Não pode ser vazio!")
            cbAPIActive.Focus()
            Return
        End If

        My.Settings.activeAPI = cbAPIActive.Text
        MessageBox.Show("Salvo com sucesso! Reiniciando...")
        My.Settings.Save()
        Application.Restart()

    End Sub

    Private Sub btCancelar_Click(sender As Object, e As EventArgs) Handles btCancelar.Click
        Me.Close()

    End Sub

End Class
