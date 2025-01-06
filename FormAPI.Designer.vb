<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FormAPI
    Inherits System.Windows.Forms.Form

    'Descartar substituições de formulário para limpar a lista de componentes.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Exigido pelo Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'OBSERVAÇÃO: o procedimento a seguir é exigido pelo Windows Form Designer
    'Pode ser modificado usando o Windows Form Designer.  
    'Não o modifique usando o editor de códigos.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FormAPI))
        Label1 = New Label()
        tbAPIKey = New TextBox()
        btSalvarAPIKey = New Button()
        btSalvarURLPro = New Button()
        tbAPIURLPro = New TextBox()
        Label2 = New Label()
        btSalvarURLSandbox = New Button()
        tbURLSandbox = New TextBox()
        Label3 = New Label()
        cbAPIActive = New ComboBox()
        Label4 = New Label()
        btSaveActiveAPI = New Button()
        btCancelar = New Button()
        SuspendLayout()
        ' 
        ' Label1
        ' 
        Label1.AutoSize = True
        Label1.ForeColor = SystemColors.ButtonHighlight
        Label1.Location = New Point(35, 27)
        Label1.Name = "Label1"
        Label1.Size = New Size(47, 15)
        Label1.TabIndex = 0
        Label1.Text = "API Key"
        ' 
        ' tbAPIKey
        ' 
        tbAPIKey.Location = New Point(35, 45)
        tbAPIKey.Name = "tbAPIKey"
        tbAPIKey.Size = New Size(435, 23)
        tbAPIKey.TabIndex = 1
        ' 
        ' btSalvarAPIKey
        ' 
        btSalvarAPIKey.BackColor = Color.SteelBlue
        btSalvarAPIKey.Cursor = Cursors.Hand
        btSalvarAPIKey.FlatAppearance.BorderSize = 0
        btSalvarAPIKey.FlatStyle = FlatStyle.Flat
        btSalvarAPIKey.ForeColor = SystemColors.ButtonHighlight
        btSalvarAPIKey.Location = New Point(476, 45)
        btSalvarAPIKey.Name = "btSalvarAPIKey"
        btSalvarAPIKey.Size = New Size(58, 23)
        btSalvarAPIKey.TabIndex = 18
        btSalvarAPIKey.Text = "Gravar"
        btSalvarAPIKey.UseVisualStyleBackColor = False
        ' 
        ' btSalvarURLPro
        ' 
        btSalvarURLPro.BackColor = Color.SteelBlue
        btSalvarURLPro.Cursor = Cursors.Hand
        btSalvarURLPro.FlatAppearance.BorderSize = 0
        btSalvarURLPro.FlatStyle = FlatStyle.Flat
        btSalvarURLPro.ForeColor = SystemColors.ButtonHighlight
        btSalvarURLPro.Location = New Point(476, 89)
        btSalvarURLPro.Name = "btSalvarURLPro"
        btSalvarURLPro.Size = New Size(58, 23)
        btSalvarURLPro.TabIndex = 21
        btSalvarURLPro.Text = "Gravar"
        btSalvarURLPro.UseVisualStyleBackColor = False
        ' 
        ' tbAPIURLPro
        ' 
        tbAPIURLPro.Location = New Point(35, 89)
        tbAPIURLPro.Name = "tbAPIURLPro"
        tbAPIURLPro.Size = New Size(435, 23)
        tbAPIURLPro.TabIndex = 20
        ' 
        ' Label2
        ' 
        Label2.AutoSize = True
        Label2.ForeColor = SystemColors.ButtonHighlight
        Label2.Location = New Point(35, 71)
        Label2.Name = "Label2"
        Label2.Size = New Size(70, 15)
        Label2.TabIndex = 19
        Label2.Text = "API URL Pro"
        ' 
        ' btSalvarURLSandbox
        ' 
        btSalvarURLSandbox.BackColor = Color.SteelBlue
        btSalvarURLSandbox.Cursor = Cursors.Hand
        btSalvarURLSandbox.FlatAppearance.BorderSize = 0
        btSalvarURLSandbox.FlatStyle = FlatStyle.Flat
        btSalvarURLSandbox.ForeColor = SystemColors.ButtonHighlight
        btSalvarURLSandbox.Location = New Point(476, 133)
        btSalvarURLSandbox.Name = "btSalvarURLSandbox"
        btSalvarURLSandbox.Size = New Size(58, 23)
        btSalvarURLSandbox.TabIndex = 24
        btSalvarURLSandbox.Text = "Gravar"
        btSalvarURLSandbox.UseVisualStyleBackColor = False
        ' 
        ' tbURLSandbox
        ' 
        tbURLSandbox.Location = New Point(35, 133)
        tbURLSandbox.Name = "tbURLSandbox"
        tbURLSandbox.Size = New Size(435, 23)
        tbURLSandbox.TabIndex = 23
        ' 
        ' Label3
        ' 
        Label3.AutoSize = True
        Label3.ForeColor = SystemColors.ButtonHighlight
        Label3.Location = New Point(35, 115)
        Label3.Name = "Label3"
        Label3.Size = New Size(98, 15)
        Label3.TabIndex = 22
        Label3.Text = "API URL Sandbox"
        ' 
        ' cbAPIActive
        ' 
        cbAPIActive.FormattingEnabled = True
        cbAPIActive.Location = New Point(35, 177)
        cbAPIActive.Name = "cbAPIActive"
        cbAPIActive.Size = New Size(499, 23)
        cbAPIActive.TabIndex = 25
        ' 
        ' Label4
        ' 
        Label4.AutoSize = True
        Label4.ForeColor = SystemColors.ButtonHighlight
        Label4.Location = New Point(35, 159)
        Label4.Name = "Label4"
        Label4.Size = New Size(53, 15)
        Label4.TabIndex = 26
        Label4.Text = "API ativa"
        ' 
        ' btSaveActiveAPI
        ' 
        btSaveActiveAPI.BackColor = Color.ForestGreen
        btSaveActiveAPI.Cursor = Cursors.Hand
        btSaveActiveAPI.FlatAppearance.BorderSize = 0
        btSaveActiveAPI.FlatStyle = FlatStyle.Flat
        btSaveActiveAPI.ForeColor = SystemColors.ButtonHighlight
        btSaveActiveAPI.Location = New Point(35, 206)
        btSaveActiveAPI.Name = "btSaveActiveAPI"
        btSaveActiveAPI.Size = New Size(107, 31)
        btSaveActiveAPI.TabIndex = 27
        btSaveActiveAPI.Text = "Salvar e reiniciar"
        btSaveActiveAPI.UseVisualStyleBackColor = False
        ' 
        ' btCancelar
        ' 
        btCancelar.BackColor = Color.IndianRed
        btCancelar.Cursor = Cursors.Hand
        btCancelar.FlatAppearance.BorderSize = 0
        btCancelar.FlatStyle = FlatStyle.Flat
        btCancelar.ForeColor = SystemColors.ButtonHighlight
        btCancelar.Location = New Point(148, 206)
        btCancelar.Name = "btCancelar"
        btCancelar.Size = New Size(66, 31)
        btCancelar.TabIndex = 28
        btCancelar.Text = "Cancelar"
        btCancelar.UseVisualStyleBackColor = False
        ' 
        ' FormAPI
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        BackColor = Color.FromArgb(CByte(31), CByte(33), CByte(32))
        ClientSize = New Size(569, 248)
        Controls.Add(btCancelar)
        Controls.Add(btSaveActiveAPI)
        Controls.Add(Label4)
        Controls.Add(cbAPIActive)
        Controls.Add(btSalvarURLSandbox)
        Controls.Add(tbURLSandbox)
        Controls.Add(Label3)
        Controls.Add(btSalvarURLPro)
        Controls.Add(tbAPIURLPro)
        Controls.Add(Label2)
        Controls.Add(btSalvarAPIKey)
        Controls.Add(tbAPIKey)
        Controls.Add(Label1)
        FormBorderStyle = FormBorderStyle.FixedSingle
        Icon = CType(resources.GetObject("$this.Icon"), Icon)
        MaximizeBox = False
        MinimizeBox = False
        Name = "FormAPI"
        StartPosition = FormStartPosition.CenterScreen
        Text = "Configurar dados da API"
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents Label1 As Label
    Friend WithEvents tbAPIKey As TextBox
    Friend WithEvents btSalvarAPIKey As Button
    Friend WithEvents btSalvarURLPro As Button
    Friend WithEvents tbAPIURLPro As TextBox
    Friend WithEvents Label2 As Label
    Friend WithEvents btSalvarURLSandbox As Button
    Friend WithEvents tbURLSandbox As TextBox
    Friend WithEvents Label3 As Label
    Friend WithEvents cbAPIActive As ComboBox
    Friend WithEvents Label4 As Label
    Friend WithEvents btSaveActiveAPI As Button
    Friend WithEvents btCancelar As Button
End Class
