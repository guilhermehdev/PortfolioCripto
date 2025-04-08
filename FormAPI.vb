Public Class FormAPI
    Dim json As New JSON
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

        If Not My.Settings.JSONBinID = Nothing Then
            tbJSONBinID.Text = My.Settings.JSONBinID
        End If
        If Not My.Settings.JSONBinMasterKey = Nothing Then
            tbJSONBinMasterKey.Text = My.Settings.JSONBinMasterKey
        End If
        If Not My.Settings.JSONBinURL = Nothing Then
            tbJSONBinGet.Text = My.Settings.JSONBinURL
        End If

        loadApi()

    End Sub

    Private Sub loadApi()
        cbAPIActive.Items.Clear()
        cbAPIActive.Items.Add(My.Settings.apiUrl)
        cbAPIActive.Items.Add(My.Settings.apiUrlSandbox)
    End Sub
    Private Sub btSaveActiveAPI_Click(sender As Object, e As EventArgs) Handles btSaveActiveAPI.Click
        If Not cbAPIActive.Text = "" Then
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

        If Not tbJSONBinID.Text = "" Then
            My.Settings.JSONBinID = tbJSONBinID.Text
            My.Settings.Save()
        Else
            MessageBox.Show("Não pode ser vazio!")
            tbJSONBinID.Focus()
            Return
        End If

        If Not tbJSONBinMasterKey.Text = "" Then
            My.Settings.JSONBinMasterKey = tbJSONBinMasterKey.Text
            My.Settings.Save()
        Else
            MessageBox.Show("Não pode ser vazio!")
            tbJSONBinMasterKey.Focus()
            Return
        End If

        If Not tbJSONBinGet.Text = "" Then
            My.Settings.JSONBinURL = tbJSONBinGet.Text
            My.Settings.Save()
        Else
            MessageBox.Show("Não pode ser vazio!")
            tbJSONBinGet.Focus()
            Return
        End If

        My.Settings.activeAPI = cbAPIActive.Text
        MessageBox.Show("Salvo com sucesso! Reiniciando...")
        My.Settings.Save()
        Application.Restart()

    End Sub

    Private Sub btCancelar_Click(sender As Object, e As EventArgs) Handles btCancelar.Click
        If Not json.checkMySettings Then
            MessageBox.Show("Configurações faltando!")
        Else
            Me.Close()
        End If

    End Sub

End Class
