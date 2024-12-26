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

        loadApi()

    End Sub

    Private Sub btSalvarAPIKey_Click(sender As Object, e As EventArgs) Handles btSalvarAPIKey.Click
        If Not tbAPIKey.Text = "" Then
            My.Settings.apiCMCKey = tbAPIKey.Text
            MessageBox.Show("Salvo com sucesso!")
            My.Settings.Save()
        Else
            MessageBox.Show("Não pode ser vazio!")
            tbAPIKey.Focus()
        End If
    End Sub

    Private Sub btSalvarURLPro_Click(sender As Object, e As EventArgs) Handles btSalvarURLPro.Click
        If Not tbAPIURLPro.Text = "" Then
            My.Settings.apiUrl = tbAPIURLPro.Text
            MessageBox.Show("Salvo com sucesso!")
            My.Settings.Save()
        Else
            MessageBox.Show("Não pode ser vazio!")
            tbAPIURLPro.Focus()
        End If
    End Sub

    Private Sub btSalvarURLSandbox_Click(sender As Object, e As EventArgs) Handles btSalvarURLSandbox.Click
        If Not tbURLSandbox.Text = "" Then
            My.Settings.apiUrlSandbox = tbURLSandbox.Text
            MessageBox.Show("Salvo com sucesso!")
            My.Settings.Save()
        Else
            MessageBox.Show("Não pode ser vazio!")
            tbURLSandbox.Focus()
        End If
    End Sub

    Private Sub loadApi()
        cbAPIActive.Items.Add(My.Settings.apiUrl)
        cbAPIActive.Items.Add(My.Settings.apiUrlSandbox)
    End Sub
    Private Sub btSaveActiveAPI_Click(sender As Object, e As EventArgs) Handles btSaveActiveAPI.Click
        If Not cbAPIActive.Text = "" Then
            My.Settings.activeAPI = cbAPIActive.Text
            MessageBox.Show("Salvo com sucesso!")
            My.Settings.Save()
        Else
            MessageBox.Show("Não pode ser vazio!")
            cbAPIActive.Focus()
        End If
    End Sub

End Class
