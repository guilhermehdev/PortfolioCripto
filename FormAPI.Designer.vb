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
        tbAPIURLPro = New TextBox()
        Label2 = New Label()
        tbURLSandbox = New TextBox()
        Label3 = New Label()
        cbAPIActive = New ComboBox()
        Label4 = New Label()
        btSaveActiveAPI = New Button()
        btCancelar = New Button()
        GroupBox1 = New GroupBox()
        GroupBox2 = New GroupBox()
        Label5 = New Label()
        tbJSONBinID = New TextBox()
        Label6 = New Label()
        tbJSONBinMasterKey = New TextBox()
        Label8 = New Label()
        tbJSONBinGet = New TextBox()
        GroupBox1.SuspendLayout()
        GroupBox2.SuspendLayout()
        SuspendLayout()
        ' 
        ' Label1
        ' 
        Label1.AutoSize = True
        Label1.ForeColor = SystemColors.ButtonHighlight
        Label1.Location = New Point(34, 35)
        Label1.Name = "Label1"
        Label1.Size = New Size(47, 15)
        Label1.TabIndex = 0
        Label1.Text = "API Key"
        ' 
        ' tbAPIKey
        ' 
        tbAPIKey.Location = New Point(34, 53)
        tbAPIKey.Name = "tbAPIKey"
        tbAPIKey.Size = New Size(383, 23)
        tbAPIKey.TabIndex = 1
        ' 
        ' tbAPIURLPro
        ' 
        tbAPIURLPro.Location = New Point(34, 97)
        tbAPIURLPro.Name = "tbAPIURLPro"
        tbAPIURLPro.Size = New Size(383, 23)
        tbAPIURLPro.TabIndex = 20
        ' 
        ' Label2
        ' 
        Label2.AutoSize = True
        Label2.ForeColor = SystemColors.ButtonHighlight
        Label2.Location = New Point(34, 79)
        Label2.Name = "Label2"
        Label2.Size = New Size(70, 15)
        Label2.TabIndex = 19
        Label2.Text = "API URL Pro"
        ' 
        ' tbURLSandbox
        ' 
        tbURLSandbox.Location = New Point(34, 141)
        tbURLSandbox.Name = "tbURLSandbox"
        tbURLSandbox.Size = New Size(383, 23)
        tbURLSandbox.TabIndex = 23
        ' 
        ' Label3
        ' 
        Label3.AutoSize = True
        Label3.ForeColor = SystemColors.ButtonHighlight
        Label3.Location = New Point(34, 123)
        Label3.Name = "Label3"
        Label3.Size = New Size(98, 15)
        Label3.TabIndex = 22
        Label3.Text = "API URL Sandbox"
        ' 
        ' cbAPIActive
        ' 
        cbAPIActive.FormattingEnabled = True
        cbAPIActive.Location = New Point(34, 185)
        cbAPIActive.Name = "cbAPIActive"
        cbAPIActive.Size = New Size(383, 23)
        cbAPIActive.TabIndex = 25
        ' 
        ' Label4
        ' 
        Label4.AutoSize = True
        Label4.ForeColor = SystemColors.ButtonHighlight
        Label4.Location = New Point(34, 167)
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
        btSaveActiveAPI.Location = New Point(283, 441)
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
        btCancelar.Location = New Point(396, 441)
        btCancelar.Name = "btCancelar"
        btCancelar.Size = New Size(66, 31)
        btCancelar.TabIndex = 28
        btCancelar.Text = "Cancelar"
        btCancelar.UseVisualStyleBackColor = False
        ' 
        ' GroupBox1
        ' 
        GroupBox1.Controls.Add(Label1)
        GroupBox1.Controls.Add(tbAPIKey)
        GroupBox1.Controls.Add(Label2)
        GroupBox1.Controls.Add(Label4)
        GroupBox1.Controls.Add(tbAPIURLPro)
        GroupBox1.Controls.Add(cbAPIActive)
        GroupBox1.Controls.Add(Label3)
        GroupBox1.Controls.Add(tbURLSandbox)
        GroupBox1.Font = New Font("Segoe UI", 9F)
        GroupBox1.ForeColor = Color.DeepSkyBlue
        GroupBox1.Location = New Point(12, 12)
        GroupBox1.Name = "GroupBox1"
        GroupBox1.Size = New Size(450, 229)
        GroupBox1.TabIndex = 29
        GroupBox1.TabStop = False
        GroupBox1.Text = "CoinMarketCap"
        ' 
        ' GroupBox2
        ' 
        GroupBox2.Controls.Add(Label5)
        GroupBox2.Controls.Add(tbJSONBinID)
        GroupBox2.Controls.Add(Label6)
        GroupBox2.Controls.Add(tbJSONBinMasterKey)
        GroupBox2.Controls.Add(Label8)
        GroupBox2.Controls.Add(tbJSONBinGet)
        GroupBox2.Font = New Font("Segoe UI", 9F)
        GroupBox2.ForeColor = Color.DeepSkyBlue
        GroupBox2.Location = New Point(12, 247)
        GroupBox2.Name = "GroupBox2"
        GroupBox2.Size = New Size(450, 188)
        GroupBox2.TabIndex = 30
        GroupBox2.TabStop = False
        GroupBox2.Text = "JSONBin"
        ' 
        ' Label5
        ' 
        Label5.AutoSize = True
        Label5.ForeColor = SystemColors.ButtonHighlight
        Label5.Location = New Point(34, 35)
        Label5.Name = "Label5"
        Label5.Size = New Size(66, 15)
        Label5.TabIndex = 0
        Label5.Text = "JSONBin ID"
        ' 
        ' tbJSONBinID
        ' 
        tbJSONBinID.Location = New Point(34, 53)
        tbJSONBinID.Name = "tbJSONBinID"
        tbJSONBinID.Size = New Size(383, 23)
        tbJSONBinID.TabIndex = 1
        ' 
        ' Label6
        ' 
        Label6.AutoSize = True
        Label6.ForeColor = SystemColors.ButtonHighlight
        Label6.Location = New Point(34, 79)
        Label6.Name = "Label6"
        Label6.Size = New Size(127, 15)
        Label6.TabIndex = 19
        Label6.Text = "JSONBin X-Master-Key"
        ' 
        ' tbJSONBinMasterKey
        ' 
        tbJSONBinMasterKey.Location = New Point(34, 97)
        tbJSONBinMasterKey.Name = "tbJSONBinMasterKey"
        tbJSONBinMasterKey.Size = New Size(383, 23)
        tbJSONBinMasterKey.TabIndex = 20
        ' 
        ' Label8
        ' 
        Label8.AutoSize = True
        Label8.ForeColor = SystemColors.ButtonHighlight
        Label8.Location = New Point(34, 123)
        Label8.Name = "Label8"
        Label8.Size = New Size(76, 15)
        Label8.TabIndex = 22
        Label8.Text = "JSONBin URL"
        ' 
        ' tbJSONBinGet
        ' 
        tbJSONBinGet.Location = New Point(34, 141)
        tbJSONBinGet.Name = "tbJSONBinGet"
        tbJSONBinGet.Size = New Size(383, 23)
        tbJSONBinGet.TabIndex = 23
        ' 
        ' FormAPI
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        BackColor = Color.FromArgb(CByte(31), CByte(33), CByte(32))
        ClientSize = New Size(476, 484)
        Controls.Add(GroupBox2)
        Controls.Add(GroupBox1)
        Controls.Add(btCancelar)
        Controls.Add(btSaveActiveAPI)
        FormBorderStyle = FormBorderStyle.None
        Icon = CType(resources.GetObject("$this.Icon"), Icon)
        MaximizeBox = False
        MinimizeBox = False
        Name = "FormAPI"
        StartPosition = FormStartPosition.CenterScreen
        Text = "Configurar dados das APIs"
        GroupBox1.ResumeLayout(False)
        GroupBox1.PerformLayout()
        GroupBox2.ResumeLayout(False)
        GroupBox2.PerformLayout()
        ResumeLayout(False)
    End Sub

    Friend WithEvents Label1 As Label
    Friend WithEvents tbAPIKey As TextBox
    Friend WithEvents tbAPIURLPro As TextBox
    Friend WithEvents Label2 As Label
    Friend WithEvents tbURLSandbox As TextBox
    Friend WithEvents Label3 As Label
    Friend WithEvents cbAPIActive As ComboBox
    Friend WithEvents Label4 As Label
    Friend WithEvents btSaveActiveAPI As Button
    Friend WithEvents btCancelar As Button
    Friend WithEvents GroupBox1 As GroupBox
    Friend WithEvents GroupBox2 As GroupBox
    Friend WithEvents Label5 As Label
    Friend WithEvents tbJSONBinID As TextBox
    Friend WithEvents Label6 As Label
    Friend WithEvents tbJSONBinMasterKey As TextBox
    Friend WithEvents Label8 As Label
    Friend WithEvents tbJSONBinGet As TextBox
End Class
