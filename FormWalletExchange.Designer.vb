<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FormWalletExchange
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
        GroupBox1 = New GroupBox()
        dgWalletExchange = New DataGridView()
        ContextMenuStrip1 = New ContextMenuStrip(components)
        ExcluirToolStripMenuItem = New ToolStripMenuItem()
        tbQtd = New MaskedTextBox()
        Label2 = New Label()
        btSalvarEntrada = New Button()
        StatusStrip1 = New StatusStrip()
        ToolStripStatusLabel1 = New ToolStripStatusLabel()
        ToolStripStatusLabel2 = New ToolStripStatusLabel()
        GroupBox1.SuspendLayout()
        CType(dgWalletExchange, ComponentModel.ISupportInitialize).BeginInit()
        ContextMenuStrip1.SuspendLayout()
        StatusStrip1.SuspendLayout()
        SuspendLayout()
        ' 
        ' GroupBox1
        ' 
        GroupBox1.Controls.Add(dgWalletExchange)
        GroupBox1.Font = New Font("Calibri", 14F, FontStyle.Italic)
        GroupBox1.ForeColor = Color.Aqua
        GroupBox1.Location = New Point(12, 66)
        GroupBox1.Name = "GroupBox1"
        GroupBox1.Size = New Size(222, 313)
        GroupBox1.TabIndex = 22
        GroupBox1.TabStop = False
        GroupBox1.Text = "Registros"
        ' 
        ' dgWalletExchange
        ' 
        dgWalletExchange.AllowUserToAddRows = False
        dgWalletExchange.AllowUserToDeleteRows = False
        dgWalletExchange.AllowUserToOrderColumns = True
        dgWalletExchange.AllowUserToResizeRows = False
        dgWalletExchange.Anchor = AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
        dgWalletExchange.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        dgWalletExchange.BackgroundColor = Color.FromArgb(CByte(64), CByte(64), CByte(64))
        dgWalletExchange.CellBorderStyle = DataGridViewCellBorderStyle.None
        dgWalletExchange.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None
        DataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle1.BackColor = Color.Black
        DataGridViewCellStyle1.Font = New Font("Calibri", 14F, FontStyle.Italic)
        DataGridViewCellStyle1.ForeColor = Color.DodgerBlue
        DataGridViewCellStyle1.SelectionBackColor = Color.Black
        DataGridViewCellStyle1.SelectionForeColor = SystemColors.ButtonHighlight
        DataGridViewCellStyle1.WrapMode = DataGridViewTriState.True
        dgWalletExchange.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle1
        dgWalletExchange.ContextMenuStrip = ContextMenuStrip1
        DataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle2.BackColor = Color.FromArgb(CByte(64), CByte(64), CByte(64))
        DataGridViewCellStyle2.Font = New Font("Calibri", 14F, FontStyle.Italic)
        DataGridViewCellStyle2.ForeColor = Color.Aqua
        DataGridViewCellStyle2.SelectionBackColor = Color.Transparent
        DataGridViewCellStyle2.SelectionForeColor = SystemColors.ControlText
        DataGridViewCellStyle2.WrapMode = DataGridViewTriState.True
        dgWalletExchange.DefaultCellStyle = DataGridViewCellStyle2
        dgWalletExchange.EnableHeadersVisualStyles = False
        dgWalletExchange.Location = New Point(6, 29)
        dgWalletExchange.MultiSelect = False
        dgWalletExchange.Name = "dgWalletExchange"
        dgWalletExchange.ReadOnly = True
        DataGridViewCellStyle3.BackColor = Color.FromArgb(CByte(64), CByte(64), CByte(64))
        DataGridViewCellStyle3.Font = New Font("Calibri", 14F, FontStyle.Italic)
        DataGridViewCellStyle3.ForeColor = SystemColors.ButtonHighlight
        DataGridViewCellStyle3.Padding = New Padding(2)
        DataGridViewCellStyle3.SelectionBackColor = Color.DarkOrange
        DataGridViewCellStyle3.SelectionForeColor = SystemColors.ControlText
        DataGridViewCellStyle3.WrapMode = DataGridViewTriState.True
        dgWalletExchange.RowHeadersDefaultCellStyle = DataGridViewCellStyle3
        dgWalletExchange.RowHeadersWidth = 4
        dgWalletExchange.RowTemplate.DefaultCellStyle.SelectionBackColor = Color.DarkOrange
        dgWalletExchange.RowTemplate.DefaultCellStyle.SelectionForeColor = Color.FromArgb(CByte(64), CByte(64), CByte(64))
        dgWalletExchange.RowTemplate.DefaultCellStyle.WrapMode = DataGridViewTriState.True
        dgWalletExchange.ScrollBars = ScrollBars.Vertical
        dgWalletExchange.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgWalletExchange.Size = New Size(210, 278)
        dgWalletExchange.TabIndex = 12
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
        ' tbQtd
        ' 
        tbQtd.BackColor = Color.FromArgb(CByte(32), CByte(32), CByte(32))
        tbQtd.ForeColor = Color.Gold
        tbQtd.Location = New Point(12, 37)
        tbQtd.Name = "tbQtd"
        tbQtd.Size = New Size(144, 23)
        tbQtd.TabIndex = 23
        ' 
        ' Label2
        ' 
        Label2.AutoSize = True
        Label2.ForeColor = SystemColors.ButtonHighlight
        Label2.Location = New Point(12, 19)
        Label2.Name = "Label2"
        Label2.Size = New Size(40, 15)
        Label2.TabIndex = 24
        Label2.Text = "Nome"
        ' 
        ' btSalvarEntrada
        ' 
        btSalvarEntrada.BackColor = Color.ForestGreen
        btSalvarEntrada.Cursor = Cursors.Hand
        btSalvarEntrada.FlatAppearance.BorderSize = 0
        btSalvarEntrada.FlatStyle = FlatStyle.Flat
        btSalvarEntrada.ForeColor = SystemColors.ButtonHighlight
        btSalvarEntrada.Location = New Point(162, 37)
        btSalvarEntrada.Name = "btSalvarEntrada"
        btSalvarEntrada.Size = New Size(72, 23)
        btSalvarEntrada.TabIndex = 25
        btSalvarEntrada.Text = "Salvar"
        btSalvarEntrada.UseVisualStyleBackColor = False
        ' 
        ' StatusStrip1
        ' 
        StatusStrip1.BackColor = Color.FromArgb(CByte(31), CByte(33), CByte(32))
        StatusStrip1.Items.AddRange(New ToolStripItem() {ToolStripStatusLabel1, ToolStripStatusLabel2})
        StatusStrip1.Location = New Point(0, 385)
        StatusStrip1.Name = "StatusStrip1"
        StatusStrip1.Size = New Size(252, 22)
        StatusStrip1.TabIndex = 26
        ' 
        ' ToolStripStatusLabel1
        ' 
        ToolStripStatusLabel1.ForeColor = SystemColors.ButtonHighlight
        ToolStripStatusLabel1.Name = "ToolStripStatusLabel1"
        ToolStripStatusLabel1.Size = New Size(55, 17)
        ToolStripStatusLabel1.Text = "Registros"
        ' 
        ' ToolStripStatusLabel2
        ' 
        ToolStripStatusLabel2.ForeColor = SystemColors.ButtonHighlight
        ToolStripStatusLabel2.Name = "ToolStripStatusLabel2"
        ToolStripStatusLabel2.Size = New Size(258, 17)
        ToolStripStatusLabel2.Text = "Botão direito para excluir o registro selecionado"
        ' 
        ' FormWalletExchange
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        BackColor = Color.FromArgb(CByte(31), CByte(33), CByte(32))
        ClientSize = New Size(252, 407)
        Controls.Add(StatusStrip1)
        Controls.Add(btSalvarEntrada)
        Controls.Add(tbQtd)
        Controls.Add(Label2)
        Controls.Add(GroupBox1)
        MaximizeBox = False
        MinimizeBox = False
        Name = "FormWalletExchange"
        StartPosition = FormStartPosition.CenterScreen
        Text = "Cadastrar Wallet / Exchange"
        GroupBox1.ResumeLayout(False)
        CType(dgWalletExchange, ComponentModel.ISupportInitialize).EndInit()
        ContextMenuStrip1.ResumeLayout(False)
        StatusStrip1.ResumeLayout(False)
        StatusStrip1.PerformLayout()
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents GroupBox1 As GroupBox
    Friend WithEvents dgWalletExchange As DataGridView
    Friend WithEvents tbQtd As MaskedTextBox
    Friend WithEvents Label2 As Label
    Friend WithEvents btSalvarEntrada As Button
    Friend WithEvents StatusStrip1 As StatusStrip
    Friend WithEvents ToolStripStatusLabel1 As ToolStripStatusLabel
    Friend WithEvents ContextMenuStrip1 As ContextMenuStrip
    Friend WithEvents ExcluirToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolStripStatusLabel2 As ToolStripStatusLabel
End Class
