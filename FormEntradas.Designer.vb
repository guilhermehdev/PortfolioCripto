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
        Dim DataGridViewCellStyle1 As DataGridViewCellStyle = New DataGridViewCellStyle()
        Dim DataGridViewCellStyle2 As DataGridViewCellStyle = New DataGridViewCellStyle()
        Dim DataGridViewCellStyle3 As DataGridViewCellStyle = New DataGridViewCellStyle()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FormEntradas))
        cbWallet = New ComboBox()
        Label2 = New Label()
        Label1 = New Label()
        Label4 = New Label()
        dtpDataEntrada = New DateTimePicker()
        btSalvarEntrada = New Button()
        Label5 = New Label()
        cbCripto = New ComboBox()
        GroupBox1 = New GroupBox()
        dgCriptos = New DataGridView()
        ContextMenuStrip1 = New ContextMenuStrip(components)
        ExcluirToolStripMenuItem = New ToolStripMenuItem()
        StatusStrip1 = New StatusStrip()
        ToolStripStatusLabel1 = New ToolStripStatusLabel()
        ButtonCancel = New Button()
        Label3 = New Label()
        tbQtd = New MaskedTextBox()
        TbPrecoEntrada = New MaskedTextBox()
        GroupBox1.SuspendLayout()
        CType(dgCriptos, ComponentModel.ISupportInitialize).BeginInit()
        ContextMenuStrip1.SuspendLayout()
        StatusStrip1.SuspendLayout()
        SuspendLayout()
        ' 
        ' cbWallet
        ' 
        cbWallet.BackColor = Color.FromArgb(CByte(32), CByte(32), CByte(32))
        cbWallet.FlatStyle = FlatStyle.Flat
        cbWallet.ForeColor = Color.White
        cbWallet.FormattingEnabled = True
        cbWallet.Location = New Point(184, 71)
        cbWallet.Name = "cbWallet"
        cbWallet.Size = New Size(182, 23)
        cbWallet.TabIndex = 12
        ' 
        ' Label2
        ' 
        Label2.AutoSize = True
        Label2.ForeColor = SystemColors.ButtonHighlight
        Label2.Location = New Point(12, 53)
        Label2.Name = "Label2"
        Label2.Size = New Size(27, 15)
        Label2.TabIndex = 11
        Label2.Text = "Qtd"
        ' 
        ' Label1
        ' 
        Label1.AutoSize = True
        Label1.ForeColor = SystemColors.ButtonHighlight
        Label1.Location = New Point(135, 9)
        Label1.Name = "Label1"
        Label1.Size = New Size(119, 15)
        Label1.TabIndex = 9
        Label1.Text = "Preço entrada/médio"
        ' 
        ' Label4
        ' 
        Label4.AutoSize = True
        Label4.ForeColor = SystemColors.ButtonHighlight
        Label4.Location = New Point(255, 9)
        Label4.Name = "Label4"
        Label4.Size = New Size(31, 15)
        Label4.TabIndex = 15
        Label4.Text = "Data"
        ' 
        ' dtpDataEntrada
        ' 
        dtpDataEntrada.CalendarMonthBackground = SystemColors.ControlLightLight
        dtpDataEntrada.Format = DateTimePickerFormat.Short
        dtpDataEntrada.Location = New Point(255, 27)
        dtpDataEntrada.Name = "dtpDataEntrada"
        dtpDataEntrada.Size = New Size(111, 23)
        dtpDataEntrada.TabIndex = 16
        ' 
        ' btSalvarEntrada
        ' 
        btSalvarEntrada.BackColor = Color.ForestGreen
        btSalvarEntrada.Cursor = Cursors.Hand
        btSalvarEntrada.FlatAppearance.BorderSize = 0
        btSalvarEntrada.FlatStyle = FlatStyle.Flat
        btSalvarEntrada.ForeColor = SystemColors.ButtonHighlight
        btSalvarEntrada.Location = New Point(372, 27)
        btSalvarEntrada.Name = "btSalvarEntrada"
        btSalvarEntrada.Size = New Size(72, 36)
        btSalvarEntrada.TabIndex = 17
        btSalvarEntrada.Text = "Salvar"
        btSalvarEntrada.UseVisualStyleBackColor = False
        ' 
        ' Label5
        ' 
        Label5.AutoSize = True
        Label5.ForeColor = SystemColors.ButtonHighlight
        Label5.Location = New Point(12, 9)
        Label5.Name = "Label5"
        Label5.Size = New Size(40, 15)
        Label5.TabIndex = 19
        Label5.Text = "Cripto"
        ' 
        ' cbCripto
        ' 
        cbCripto.BackColor = Color.FromArgb(CByte(32), CByte(32), CByte(32))
        cbCripto.FlatStyle = FlatStyle.Flat
        cbCripto.ForeColor = Color.White
        cbCripto.FormattingEnabled = True
        cbCripto.Items.AddRange(New Object() {"BTC", "ETH", "SOL", "NEAR", "PENDLE", "KSM", "ENA", "DOG", "ATH", "ZRO", "PEPE", "VISTA", "MLC"})
        cbCripto.Location = New Point(12, 27)
        cbCripto.Name = "cbCripto"
        cbCripto.Size = New Size(116, 23)
        cbCripto.TabIndex = 18
        ' 
        ' GroupBox1
        ' 
        GroupBox1.Controls.Add(dgCriptos)
        GroupBox1.Font = New Font("Calibri", 14F, FontStyle.Italic)
        GroupBox1.ForeColor = Color.Aqua
        GroupBox1.Location = New Point(12, 105)
        GroupBox1.Name = "GroupBox1"
        GroupBox1.Size = New Size(432, 262)
        GroupBox1.TabIndex = 21
        GroupBox1.TabStop = False
        GroupBox1.Text = "Registros"
        ' 
        ' dgCriptos
        ' 
        dgCriptos.AllowUserToAddRows = False
        dgCriptos.AllowUserToDeleteRows = False
        dgCriptos.AllowUserToOrderColumns = True
        dgCriptos.AllowUserToResizeRows = False
        dgCriptos.Anchor = AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
        dgCriptos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        dgCriptos.BackgroundColor = Color.FromArgb(CByte(64), CByte(64), CByte(64))
        dgCriptos.CellBorderStyle = DataGridViewCellBorderStyle.None
        dgCriptos.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None
        DataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle1.BackColor = Color.FromArgb(CByte(64), CByte(64), CByte(64))
        DataGridViewCellStyle1.Font = New Font("Calibri", 14F, FontStyle.Italic)
        DataGridViewCellStyle1.ForeColor = Color.Turquoise
        DataGridViewCellStyle1.SelectionBackColor = Color.Transparent
        DataGridViewCellStyle1.SelectionForeColor = SystemColors.ControlText
        DataGridViewCellStyle1.WrapMode = DataGridViewTriState.True
        dgCriptos.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle1
        dgCriptos.ContextMenuStrip = ContextMenuStrip1
        DataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle2.BackColor = Color.FromArgb(CByte(64), CByte(64), CByte(64))
        DataGridViewCellStyle2.Font = New Font("Calibri", 14F, FontStyle.Italic)
        DataGridViewCellStyle2.ForeColor = Color.Aqua
        DataGridViewCellStyle2.SelectionBackColor = Color.Transparent
        DataGridViewCellStyle2.SelectionForeColor = SystemColors.ControlText
        DataGridViewCellStyle2.WrapMode = DataGridViewTriState.True
        dgCriptos.DefaultCellStyle = DataGridViewCellStyle2
        dgCriptos.EnableHeadersVisualStyles = False
        dgCriptos.Location = New Point(6, 29)
        dgCriptos.MultiSelect = False
        dgCriptos.Name = "dgCriptos"
        dgCriptos.ReadOnly = True
        DataGridViewCellStyle3.BackColor = Color.FromArgb(CByte(64), CByte(64), CByte(64))
        DataGridViewCellStyle3.Font = New Font("Calibri", 14F, FontStyle.Italic)
        DataGridViewCellStyle3.ForeColor = SystemColors.ButtonHighlight
        DataGridViewCellStyle3.Padding = New Padding(2)
        DataGridViewCellStyle3.SelectionBackColor = Color.DarkOrange
        DataGridViewCellStyle3.SelectionForeColor = SystemColors.ControlText
        DataGridViewCellStyle3.WrapMode = DataGridViewTriState.True
        dgCriptos.RowHeadersDefaultCellStyle = DataGridViewCellStyle3
        dgCriptos.RowHeadersWidth = 4
        dgCriptos.RowTemplate.DefaultCellStyle.SelectionBackColor = Color.DarkOrange
        dgCriptos.RowTemplate.DefaultCellStyle.SelectionForeColor = Color.FromArgb(CByte(64), CByte(64), CByte(64))
        dgCriptos.RowTemplate.DefaultCellStyle.WrapMode = DataGridViewTriState.True
        dgCriptos.ScrollBars = ScrollBars.Vertical
        dgCriptos.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgCriptos.Size = New Size(420, 227)
        dgCriptos.TabIndex = 12
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
        StatusStrip1.BackColor = Color.Black
        StatusStrip1.Items.AddRange(New ToolStripItem() {ToolStripStatusLabel1})
        StatusStrip1.Location = New Point(0, 370)
        StatusStrip1.Name = "StatusStrip1"
        StatusStrip1.Size = New Size(456, 22)
        StatusStrip1.TabIndex = 22
        StatusStrip1.Text = "StatusStrip1"
        ' 
        ' ToolStripStatusLabel1
        ' 
        ToolStripStatusLabel1.ForeColor = SystemColors.ButtonHighlight
        ToolStripStatusLabel1.Name = "ToolStripStatusLabel1"
        ToolStripStatusLabel1.Size = New Size(258, 17)
        ToolStripStatusLabel1.Text = "Botão direito para excluir o registro selecionado"
        ' 
        ' ButtonCancel
        ' 
        ButtonCancel.BackColor = Color.IndianRed
        ButtonCancel.Cursor = Cursors.Hand
        ButtonCancel.FlatAppearance.BorderSize = 0
        ButtonCancel.FlatStyle = FlatStyle.Flat
        ButtonCancel.ForeColor = SystemColors.ButtonHighlight
        ButtonCancel.Location = New Point(372, 71)
        ButtonCancel.Name = "ButtonCancel"
        ButtonCancel.Size = New Size(72, 23)
        ButtonCancel.TabIndex = 23
        ButtonCancel.Text = "Cancelar"
        ButtonCancel.UseVisualStyleBackColor = False
        ' 
        ' Label3
        ' 
        Label3.AutoSize = True
        Label3.ForeColor = SystemColors.ButtonHighlight
        Label3.Location = New Point(184, 53)
        Label3.Name = "Label3"
        Label3.Size = New Size(56, 15)
        Label3.TabIndex = 24
        Label3.Text = "Endereço"
        ' 
        ' tbQtd
        ' 
        tbQtd.BackColor = Color.FromArgb(CByte(32), CByte(32), CByte(32))
        tbQtd.ForeColor = Color.Gold
        tbQtd.Location = New Point(12, 71)
        tbQtd.Name = "tbQtd"
        tbQtd.PromptChar = "0"c
        tbQtd.Size = New Size(166, 23)
        tbQtd.TabIndex = 1
        tbQtd.Text = "0"
        ' 
        ' TbPrecoEntrada
        ' 
        TbPrecoEntrada.BackColor = Color.FromArgb(CByte(32), CByte(32), CByte(32))
        TbPrecoEntrada.ForeColor = SystemColors.MenuBar
        TbPrecoEntrada.Location = New Point(135, 27)
        TbPrecoEntrada.Name = "TbPrecoEntrada"
        TbPrecoEntrada.Size = New Size(115, 23)
        TbPrecoEntrada.TabIndex = 25
        TbPrecoEntrada.Text = "$0,00"
        ' 
        ' FormEntradas
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        BackColor = Color.FromArgb(CByte(31), CByte(33), CByte(32))
        ClientSize = New Size(456, 392)
        Controls.Add(TbPrecoEntrada)
        Controls.Add(tbQtd)
        Controls.Add(Label3)
        Controls.Add(ButtonCancel)
        Controls.Add(StatusStrip1)
        Controls.Add(GroupBox1)
        Controls.Add(Label5)
        Controls.Add(cbCripto)
        Controls.Add(btSalvarEntrada)
        Controls.Add(dtpDataEntrada)
        Controls.Add(Label4)
        Controls.Add(cbWallet)
        Controls.Add(Label2)
        Controls.Add(Label1)
        FormBorderStyle = FormBorderStyle.FixedSingle
        Icon = CType(resources.GetObject("$this.Icon"), Icon)
        MaximizeBox = False
        MinimizeBox = False
        Name = "FormEntradas"
        StartPosition = FormStartPosition.CenterScreen
        Text = "Lançar entradas"
        GroupBox1.ResumeLayout(False)
        CType(dgCriptos, ComponentModel.ISupportInitialize).EndInit()
        ContextMenuStrip1.ResumeLayout(False)
        StatusStrip1.ResumeLayout(False)
        StatusStrip1.PerformLayout()
        ResumeLayout(False)
        PerformLayout()
    End Sub
    Friend WithEvents cbWallet As ComboBox
    Friend WithEvents Label2 As Label
    Friend WithEvents Label1 As Label
    Friend WithEvents Label4 As Label
    Friend WithEvents dtpDataEntrada As DateTimePicker
    Friend WithEvents btSalvarEntrada As Button
    Friend WithEvents Label5 As Label
    Friend WithEvents cbCripto As ComboBox
    Friend WithEvents GroupBox1 As GroupBox
    Friend WithEvents ContextMenuStrip1 As ContextMenuStrip
    Friend WithEvents ExcluirToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents StatusStrip1 As StatusStrip
    Friend WithEvents ToolStripStatusLabel1 As ToolStripStatusLabel
    Friend WithEvents ButtonCancel As Button
    Friend WithEvents Label3 As Label
    Friend WithEvents tbQtd As MaskedTextBox
    Friend WithEvents TbPrecoEntrada As MaskedTextBox
    Friend WithEvents dgCriptos As DataGridView
End Class
