<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FormEntradas
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
        components = New ComponentModel.Container()
        Label3 = New Label()
        cbWallet = New ComboBox()
        Label2 = New Label()
        tbQtd = New TextBox()
        Label1 = New Label()
        Label4 = New Label()
        dtpDataEntrada = New DateTimePicker()
        btSalvarEntrada = New Button()
        Label5 = New Label()
        cbCripto = New ComboBox()
        TbPrecoEntrada = New TextBox()
        GroupBox1 = New GroupBox()
        dgCriptos = New DataGridView()
        ContextMenuStrip1 = New ContextMenuStrip(components)
        ExcluirToolStripMenuItem = New ToolStripMenuItem()
        StatusStrip1 = New StatusStrip()
        ToolStripStatusLabel1 = New ToolStripStatusLabel()
        GroupBox1.SuspendLayout()
        CType(dgCriptos, ComponentModel.ISupportInitialize).BeginInit()
        ContextMenuStrip1.SuspendLayout()
        StatusStrip1.SuspendLayout()
        SuspendLayout()
        ' 
        ' Label3
        ' 
        Label3.AutoSize = True
        Label3.Location = New Point(184, 53)
        Label3.Name = "Label3"
        Label3.Size = New Size(40, 15)
        Label3.TabIndex = 13
        Label3.Text = "Wallet"
        ' 
        ' cbWallet
        ' 
        cbWallet.FormattingEnabled = True
        cbWallet.Items.AddRange(New Object() {"Metamask", "TrustWallet", "Phantom", "Binance", "Bybit", "Gate.io"})
        cbWallet.Location = New Point(184, 71)
        cbWallet.Name = "cbWallet"
        cbWallet.Size = New Size(140, 23)
        cbWallet.TabIndex = 12
        ' 
        ' Label2
        ' 
        Label2.AutoSize = True
        Label2.Location = New Point(12, 53)
        Label2.Name = "Label2"
        Label2.Size = New Size(27, 15)
        Label2.TabIndex = 11
        Label2.Text = "Qtd"
        ' 
        ' tbQtd
        ' 
        tbQtd.BorderStyle = BorderStyle.FixedSingle
        tbQtd.Location = New Point(12, 71)
        tbQtd.Name = "tbQtd"
        tbQtd.Size = New Size(166, 23)
        tbQtd.TabIndex = 10
        tbQtd.Text = "0"
        ' 
        ' Label1
        ' 
        Label1.AutoSize = True
        Label1.Location = New Point(108, 9)
        Label1.Name = "Label1"
        Label1.Size = New Size(119, 15)
        Label1.TabIndex = 9
        Label1.Text = "Preço entrada/médio"
        ' 
        ' Label4
        ' 
        Label4.AutoSize = True
        Label4.Location = New Point(229, 9)
        Label4.Name = "Label4"
        Label4.Size = New Size(31, 15)
        Label4.TabIndex = 15
        Label4.Text = "Data"
        ' 
        ' dtpDataEntrada
        ' 
        dtpDataEntrada.Format = DateTimePickerFormat.Short
        dtpDataEntrada.Location = New Point(229, 27)
        dtpDataEntrada.Name = "dtpDataEntrada"
        dtpDataEntrada.Size = New Size(95, 23)
        dtpDataEntrada.TabIndex = 16
        ' 
        ' btSalvarEntrada
        ' 
        btSalvarEntrada.Location = New Point(330, 27)
        btSalvarEntrada.Name = "btSalvarEntrada"
        btSalvarEntrada.Size = New Size(77, 67)
        btSalvarEntrada.TabIndex = 17
        btSalvarEntrada.Text = "Salvar"
        btSalvarEntrada.UseVisualStyleBackColor = True
        ' 
        ' Label5
        ' 
        Label5.AutoSize = True
        Label5.Location = New Point(12, 9)
        Label5.Name = "Label5"
        Label5.Size = New Size(40, 15)
        Label5.TabIndex = 19
        Label5.Text = "Cripto"
        ' 
        ' cbCripto
        ' 
        cbCripto.FormattingEnabled = True
        cbCripto.Items.AddRange(New Object() {"BTC", "ETH", "SOL", "NEAR", "PENDLE", "KSM", "ENA", "DOG", "ATH", "ZRO", "PEPE", "VISTA"})
        cbCripto.Location = New Point(12, 27)
        cbCripto.Name = "cbCripto"
        cbCripto.Size = New Size(90, 23)
        cbCripto.TabIndex = 18
        ' 
        ' TbPrecoEntrada
        ' 
        TbPrecoEntrada.BorderStyle = BorderStyle.FixedSingle
        TbPrecoEntrada.Location = New Point(108, 27)
        TbPrecoEntrada.Name = "TbPrecoEntrada"
        TbPrecoEntrada.Size = New Size(115, 23)
        TbPrecoEntrada.TabIndex = 20
        TbPrecoEntrada.Text = "0,00"
        ' 
        ' GroupBox1
        ' 
        GroupBox1.Controls.Add(dgCriptos)
        GroupBox1.Location = New Point(12, 100)
        GroupBox1.Name = "GroupBox1"
        GroupBox1.Size = New Size(395, 261)
        GroupBox1.TabIndex = 21
        GroupBox1.TabStop = False
        GroupBox1.Text = "Registros"
        ' 
        ' dgCriptos
        ' 
        dgCriptos.AllowUserToAddRows = False
        dgCriptos.AllowUserToOrderColumns = True
        dgCriptos.AllowUserToResizeColumns = False
        dgCriptos.AllowUserToResizeRows = False
        dgCriptos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        dgCriptos.BackgroundColor = SystemColors.ButtonHighlight
        dgCriptos.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize
        dgCriptos.ContextMenuStrip = ContextMenuStrip1
        dgCriptos.Cursor = Cursors.Hand
        dgCriptos.Location = New Point(6, 22)
        dgCriptos.MultiSelect = False
        dgCriptos.Name = "dgCriptos"
        dgCriptos.ReadOnly = True
        dgCriptos.RowHeadersWidth = 4
        dgCriptos.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgCriptos.Size = New Size(383, 233)
        dgCriptos.TabIndex = 0
        ' 
        ' ContextMenuStrip1
        ' 
        ContextMenuStrip1.Items.AddRange(New ToolStripItem() {ExcluirToolStripMenuItem})
        ContextMenuStrip1.Name = "ContextMenuStrip1"
        ContextMenuStrip1.Size = New Size(110, 26)
        ' 
        ' ExcluirToolStripMenuItem
        ' 
        ExcluirToolStripMenuItem.Name = "ExcluirToolStripMenuItem"
        ExcluirToolStripMenuItem.Size = New Size(109, 22)
        ExcluirToolStripMenuItem.Text = "Excluir"
        ' 
        ' StatusStrip1
        ' 
        StatusStrip1.Items.AddRange(New ToolStripItem() {ToolStripStatusLabel1})
        StatusStrip1.Location = New Point(0, 370)
        StatusStrip1.Name = "StatusStrip1"
        StatusStrip1.Size = New Size(419, 22)
        StatusStrip1.TabIndex = 22
        StatusStrip1.Text = "StatusStrip1"
        ' 
        ' ToolStripStatusLabel1
        ' 
        ToolStripStatusLabel1.Name = "ToolStripStatusLabel1"
        ToolStripStatusLabel1.Size = New Size(258, 17)
        ToolStripStatusLabel1.Text = "Botão direito para excluir o registro selecionado"
        ' 
        ' FormEntradas
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(419, 392)
        Controls.Add(StatusStrip1)
        Controls.Add(GroupBox1)
        Controls.Add(TbPrecoEntrada)
        Controls.Add(Label5)
        Controls.Add(cbCripto)
        Controls.Add(btSalvarEntrada)
        Controls.Add(dtpDataEntrada)
        Controls.Add(Label4)
        Controls.Add(Label3)
        Controls.Add(cbWallet)
        Controls.Add(Label2)
        Controls.Add(tbQtd)
        Controls.Add(Label1)
        FormBorderStyle = FormBorderStyle.FixedSingle
        MaximizeBox = False
        Name = "FormEntradas"
        StartPosition = FormStartPosition.CenterParent
        Text = "Lançar entradas"
        GroupBox1.ResumeLayout(False)
        CType(dgCriptos, ComponentModel.ISupportInitialize).EndInit()
        ContextMenuStrip1.ResumeLayout(False)
        StatusStrip1.ResumeLayout(False)
        StatusStrip1.PerformLayout()
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents Label3 As Label
    Friend WithEvents cbWallet As ComboBox
    Friend WithEvents Label2 As Label
    Friend WithEvents tbQtd As TextBox
    Friend WithEvents Label1 As Label
    Friend WithEvents Label4 As Label
    Friend WithEvents dtpDataEntrada As DateTimePicker
    Friend WithEvents btSalvarEntrada As Button
    Friend WithEvents Label5 As Label
    Friend WithEvents cbCripto As ComboBox
    Friend WithEvents TbPrecoEntrada As TextBox
    Friend WithEvents GroupBox1 As GroupBox
    Friend WithEvents dgCriptos As DataGridView
    Friend WithEvents ContextMenuStrip1 As ContextMenuStrip
    Friend WithEvents ExcluirToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents StatusStrip1 As StatusStrip
    Friend WithEvents ToolStripStatusLabel1 As ToolStripStatusLabel
End Class
