Public Class FormExchangeCredentials
    Inherits Form

    Private ReadOnly _binanceApi As TextBox
    Private ReadOnly _binanceSecret As TextBox
    Private ReadOnly _gateApi As TextBox
    Private ReadOnly _gateSecret As TextBox
    Private ReadOnly _binanceStatus As Label
    Private ReadOnly _gateStatus As Label

    Public Sub New()
        Text = "Credenciais das exchanges"
        StartPosition = FormStartPosition.CenterParent
        FormBorderStyle = FormBorderStyle.FixedDialog
        MaximizeBox = False
        MinimizeBox = False
        ShowInTaskbar = False
        ClientSize = New Size(520, 350)
        BackColor = Color.FromArgb(35, 35, 35)
        ForeColor = Color.White

        Controls.Add(New Label With {
            .AutoSize = False,
            .Location = New Point(18, 15),
            .Size = New Size(480, 25),
            .Font = New Font("Segoe UI", 11.0F, FontStyle.Bold),
            .ForeColor = Color.Aqua,
            .Text = "Credenciais das exchanges"
        })
        Controls.Add(New Label With {
            .AutoSize = False,
            .Location = New Point(18, 42),
            .Size = New Size(480, 32),
            .ForeColor = Color.Gainsboro,
            .Text = "As chaves ficam no Gerenciador de Credenciais do Windows." & Environment.NewLine &
                    "Deixe os dois campos vazios para manter a credencial atual."
        })

        Controls.Add(CreateSectionLabel("Binance", 18, 85))
        Controls.Add(CreateFieldLabel("Chave API:", 18, 116))
        _binanceApi = CreateSecretTextBox(150, 112)
        Controls.Add(_binanceApi)
        Controls.Add(CreateFieldLabel("Chave secreta:", 18, 148))
        _binanceSecret = CreateSecretTextBox(150, 144)
        Controls.Add(_binanceSecret)
        _binanceStatus = CreateStatusLabel(150, 174)
        Controls.Add(_binanceStatus)

        Controls.Add(CreateSectionLabel("Gate.io", 18, 202))
        Controls.Add(CreateFieldLabel("Chave API:", 18, 233))
        _gateApi = CreateSecretTextBox(150, 229)
        Controls.Add(_gateApi)
        Controls.Add(CreateFieldLabel("Chave secreta:", 18, 265))
        _gateSecret = CreateSecretTextBox(150, 261)
        Controls.Add(_gateSecret)
        _gateStatus = CreateStatusLabel(150, 291)
        Controls.Add(_gateStatus)

        Dim exportButton As New Button With {
            .Text = "Exportar",
            .Location = New Point(165, 315),
            .Size = New Size(80, 28),
            .BackColor = Color.FromArgb(55, 75, 105),
            .ForeColor = Color.White,
            .FlatStyle = FlatStyle.Flat
        }
        exportButton.FlatAppearance.BorderSize = 0
        AddHandler exportButton.Click, AddressOf ExportButton_Click
        Controls.Add(exportButton)

        Dim saveButton As New Button With {
            .Text = "Salvar",
            .Location = New Point(345, 315),
            .Size = New Size(80, 28),
            .BackColor = Color.DarkGreen,
            .ForeColor = Color.White,
            .FlatStyle = FlatStyle.Flat
        }
        saveButton.FlatAppearance.BorderSize = 0
        AddHandler saveButton.Click, AddressOf SaveButton_Click
        Controls.Add(saveButton)

        Dim cancelButton As New Button With {
            .Text = "Cancelar",
            .Location = New Point(435, 315),
            .Size = New Size(80, 28),
            .BackColor = Color.FromArgb(75, 75, 75),
            .ForeColor = Color.White,
            .FlatStyle = FlatStyle.Flat
        }
        cancelButton.FlatAppearance.BorderSize = 0
        AddHandler cancelButton.Click, Sub() DialogResult = DialogResult.Cancel
        Controls.Add(cancelButton)

        AcceptButton = saveButton
        CancelButton = cancelButton
        UpdateStatusLabels()
    End Sub

    Private Sub SaveButton_Click(sender As Object, e As EventArgs)
        Try
            SaveExchange("Binance", _binanceApi.Text, _binanceSecret.Text, AddressOf ExchangeCredentials.SaveBinanceCredentials)
            SaveExchange("Gate.io", _gateApi.Text, _gateSecret.Text, AddressOf ExchangeCredentials.SaveGateCredentials)
            MessageBox.Show("Credenciais salvas no Gerenciador de Credenciais do Windows.", "Credenciais", MessageBoxButtons.OK, MessageBoxIcon.Information)
            DialogResult = DialogResult.OK
        Catch ex As Exception
            MessageBox.Show(ex.Message, "Credenciais", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End Try
    End Sub

    Private Sub ExportButton_Click(sender As Object, e As EventArgs)
        If String.IsNullOrWhiteSpace(ExchangeCredentials.BinanceAPIKey) AndAlso
           String.IsNullOrWhiteSpace(ExchangeCredentials.BinanceSecretKey) AndAlso
           String.IsNullOrWhiteSpace(ExchangeCredentials.GateAPIKey) AndAlso
           String.IsNullOrWhiteSpace(ExchangeCredentials.GateSecretKey) Then
            MessageBox.Show(
                "Não há credenciais configuradas para exportar.",
                "Exportar credenciais",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information)
            Return
        End If

        Using passwordForm As New CredentialExportPasswordForm()
            If passwordForm.ShowDialog(Me) <> DialogResult.OK Then Return

            Using dialog As New SaveFileDialog With {
                .Title = "Exportar credenciais criptografadas",
                .Filter = "Exportação de credenciais (*.pcexport)|*.pcexport|Todos os arquivos (*.*)|*.*",
                .DefaultExt = "pcexport",
                .AddExtension = True,
                .OverwritePrompt = True,
                .FileName = "portfolio-credenciais.pcexport",
                .InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
            }
                If dialog.ShowDialog(Me) <> DialogResult.OK Then Return

                Try
                    ExchangeCredentialsExportService.ExportEncrypted(dialog.FileName, passwordForm.Password)
                    MessageBox.Show(
                        "Credenciais exportadas em arquivo criptografado." & Environment.NewLine &
                        "Guarde a senha: ela não é salva no arquivo.",
                        "Exportar credenciais",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information)
                Catch ex As Exception
                    MessageBox.Show(
                        "Erro ao exportar as credenciais: " & ex.Message,
                        "Exportar credenciais",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error)
                End Try
            End Using
        End Using
    End Sub

    Private Shared Sub SaveExchange(exchangeName As String, apiKey As String, secretKey As String, saveAction As Action(Of String, String))
        Dim hasApi = Not String.IsNullOrWhiteSpace(apiKey)
        Dim hasSecret = Not String.IsNullOrWhiteSpace(secretKey)
        If Not hasApi AndAlso Not hasSecret Then Return
        If Not hasApi OrElse Not hasSecret Then
            Throw New InvalidOperationException($"Informe a chave API e a chave secreta da {exchangeName}, ou deixe os dois campos vazios.")
        End If
        saveAction(apiKey, secretKey)
    End Sub

    Private Sub UpdateStatusLabels()
        Dim binanceConfigured = Not String.IsNullOrWhiteSpace(ExchangeCredentials.BinanceAPIKey) AndAlso
                                Not String.IsNullOrWhiteSpace(ExchangeCredentials.BinanceSecretKey)
        Dim gateConfigured = Not String.IsNullOrWhiteSpace(ExchangeCredentials.GateAPIKey) AndAlso
                             Not String.IsNullOrWhiteSpace(ExchangeCredentials.GateSecretKey)
        SetStatus(_binanceStatus, binanceConfigured)
        SetStatus(_gateStatus, gateConfigured)
    End Sub

    Private Shared Sub SetStatus(label As Label, configured As Boolean)
        label.Text = If(configured, "Credencial configurada", "Credencial não configurada")
        label.ForeColor = If(configured, Color.LimeGreen, Color.IndianRed)
    End Sub

    Private Shared Function CreateSectionLabel(text As String, x As Integer, y As Integer) As Label
        Return New Label With {.AutoSize = True, .Location = New Point(x, y), .Font = New Font("Segoe UI", 9.5F, FontStyle.Bold), .ForeColor = Color.Gold, .Text = text}
    End Function

    Private Shared Function CreateFieldLabel(text As String, x As Integer, y As Integer) As Label
        Return New Label With {.AutoSize = True, .Location = New Point(x, y + 4), .ForeColor = Color.White, .Text = text}
    End Function

    Private Shared Function CreateSecretTextBox(x As Integer, y As Integer) As TextBox
        Return New TextBox With {.Location = New Point(x, y), .Size = New Size(350, 23), .BackColor = Color.FromArgb(55, 55, 55), .ForeColor = Color.White, .BorderStyle = BorderStyle.FixedSingle, .UseSystemPasswordChar = True}
    End Function

    Private Shared Function CreateStatusLabel(x As Integer, y As Integer) As Label
        Return New Label With {.AutoSize = True, .Location = New Point(x, y), .Font = New Font("Segoe UI", 8.0F, FontStyle.Italic), .Text = String.Empty}
    End Function
End Class

Friend NotInheritable Class CredentialExportPasswordForm
    Inherits Form

    Private ReadOnly _password As TextBox
    Private ReadOnly _confirmation As TextBox

    Public Sub New()
        Text = "Senha da exportação"
        StartPosition = FormStartPosition.CenterParent
        FormBorderStyle = FormBorderStyle.FixedDialog
        MaximizeBox = False
        MinimizeBox = False
        ShowInTaskbar = False
        ClientSize = New Size(390, 190)
        BackColor = Color.FromArgb(35, 35, 35)
        ForeColor = Color.White

        Controls.Add(New Label With {
            .AutoSize = False,
            .Location = New Point(16, 14),
            .Size = New Size(355, 36),
            .ForeColor = Color.Gainsboro,
            .Text = "Defina uma senha para proteger o arquivo exportado."
        })
        Controls.Add(New Label With {.AutoSize = True, .Location = New Point(16, 65), .Text = "Senha:"})
        _password = CreatePasswordBox(100, 61)
        Controls.Add(_password)
        Controls.Add(New Label With {.AutoSize = True, .Location = New Point(16, 99), .Text = "Confirmar:"})
        _confirmation = CreatePasswordBox(100, 95)
        Controls.Add(_confirmation)

        Dim okButton As New Button With {
            .Text = "Exportar",
            .Location = New Point(205, 143),
            .Size = New Size(80, 28),
            .BackColor = Color.DarkGreen,
            .ForeColor = Color.White,
            .FlatStyle = FlatStyle.Flat,
            .DialogResult = DialogResult.OK
        }
        okButton.FlatAppearance.BorderSize = 0
        AddHandler okButton.Click, AddressOf OkButton_Click
        Controls.Add(okButton)

        Dim cancelButton As New Button With {
            .Text = "Cancelar",
            .Location = New Point(295, 143),
            .Size = New Size(80, 28),
            .BackColor = Color.FromArgb(75, 75, 75),
            .ForeColor = Color.White,
            .FlatStyle = FlatStyle.Flat,
            .DialogResult = DialogResult.Cancel
        }
        cancelButton.FlatAppearance.BorderSize = 0
        Controls.Add(cancelButton)

        AcceptButton = okButton
        CancelButton = cancelButton
    End Sub

    Public ReadOnly Property Password As String
        Get
            Return _password.Text
        End Get
    End Property

    Private Sub OkButton_Click(sender As Object, e As EventArgs)
        If String.IsNullOrWhiteSpace(_password.Text) Then
            MessageBox.Show("Informe uma senha.", "Exportar credenciais", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            DialogResult = DialogResult.None
            _password.Focus()
            Return
        End If

        If Not String.Equals(_password.Text, _confirmation.Text, StringComparison.Ordinal) Then
            MessageBox.Show("As senhas não conferem.", "Exportar credenciais", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            DialogResult = DialogResult.None
            _confirmation.SelectAll()
            _confirmation.Focus()
        End If
    End Sub

    Private Shared Function CreatePasswordBox(x As Integer, y As Integer) As TextBox
        Return New TextBox With {
            .Location = New Point(x, y),
            .Size = New Size(260, 23),
            .BackColor = Color.FromArgb(55, 55, 55),
            .ForeColor = Color.White,
            .BorderStyle = BorderStyle.FixedSingle,
            .UseSystemPasswordChar = True
        }
    End Function
End Class
