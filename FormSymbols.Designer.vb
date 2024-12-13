<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FormSymbols
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
        StatusStrip1 = New StatusStrip()
        ToolStripStatusLabel1 = New ToolStripStatusLabel()
        ToolStripStatusLabel2 = New ToolStripStatusLabel()
        btSalvarEntrada = New Button()
        tbSymbol = New MaskedTextBox()
        Label2 = New Label()
        GroupBox1 = New GroupBox()
        dgSymbols = New DataGridView()
        ContextMenuStrip1 = New ContextMenuStrip(components)
        ExcluirToolStripMenuItem = New ToolStripMenuItem()
        StatusStrip1.SuspendLayout()
        GroupBox1.SuspendLayout()
        CType(dgSymbols, ComponentModel.ISupportInitialize).BeginInit()
        ContextMenuStrip1.SuspendLayout()
        SuspendLayout()
        ' 
        ' StatusStrip1
        ' 
        StatusStrip1.BackColor = Color.FromArgb(CByte(31), CByte(33), CByte(32))
        StatusStrip1.Items.AddRange(New ToolStripItem() {ToolStripStatusLabel1, ToolStripStatusLabel2})
        StatusStrip1.Location = New Point(0, 385)
        StatusStrip1.Name = "StatusStrip1"
        StatusStrip1.Size = New Size(250, 22)
        StatusStrip1.TabIndex = 31
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
        ToolStripStatusLabel2.Size = New Size(258, 15)
        ToolStripStatusLabel2.Text = "Botão direito para excluir o registro selecionado"
        ' 
        ' btSalvarEntrada
        ' 
        btSalvarEntrada.BackColor = Color.ForestGreen
        btSalvarEntrada.Cursor = Cursors.Hand
        btSalvarEntrada.FlatAppearance.BorderSize = 0
        btSalvarEntrada.FlatStyle = FlatStyle.Flat
        btSalvarEntrada.ForeColor = SystemColors.ButtonHighlight
        btSalvarEntrada.Location = New Point(162, 28)
        btSalvarEntrada.Name = "btSalvarEntrada"
        btSalvarEntrada.Size = New Size(72, 23)
        btSalvarEntrada.TabIndex = 30
        btSalvarEntrada.Text = "Salvar"
        btSalvarEntrada.UseVisualStyleBackColor = False
        ' 
        ' tbSymbol
        ' 
        tbSymbol.BackColor = Color.FromArgb(CByte(32), CByte(32), CByte(32))
        tbSymbol.ForeColor = Color.Gold
        tbSymbol.Location = New Point(12, 28)
        tbSymbol.Name = "tbSymbol"
        tbSymbol.Size = New Size(144, 23)
        tbSymbol.TabIndex = 28
        ' 
        ' Label2
        ' 
        Label2.AutoSize = True
        Label2.ForeColor = SystemColors.ButtonHighlight
        Label2.Location = New Point(12, 10)
        Label2.Name = "Label2"
        Label2.Size = New Size(40, 15)
        Label2.TabIndex = 29
        Label2.Text = "Nome"
        ' 
        ' GroupBox1
        ' 
        GroupBox1.Controls.Add(dgSymbols)
        GroupBox1.Font = New Font("Calibri", 14F, FontStyle.Italic)
        GroupBox1.ForeColor = Color.Aqua
        GroupBox1.Location = New Point(12, 57)
        GroupBox1.Name = "GroupBox1"
        GroupBox1.Size = New Size(222, 325)
        GroupBox1.TabIndex = 27
        GroupBox1.TabStop = False
        GroupBox1.Text = "Registros"
        ' 
        ' dgSymbols
        ' 
        dgSymbols.AllowUserToAddRows = False
        dgSymbols.AllowUserToDeleteRows = False
        dgSymbols.AllowUserToOrderColumns = True
        dgSymbols.AllowUserToResizeRows = False
        dgSymbols.Anchor = AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
        dgSymbols.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        dgSymbols.BackgroundColor = Color.FromArgb(CByte(64), CByte(64), CByte(64))
        dgSymbols.CellBorderStyle = DataGridViewCellBorderStyle.None
        dgSymbols.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None
        DataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle1.BackColor = Color.Black
        DataGridViewCellStyle1.Font = New Font("Calibri", 14F, FontStyle.Italic)
        DataGridViewCellStyle1.ForeColor = Color.DodgerBlue
        DataGridViewCellStyle1.SelectionBackColor = Color.Black
        DataGridViewCellStyle1.SelectionForeColor = SystemColors.ButtonHighlight
        DataGridViewCellStyle1.WrapMode = DataGridViewTriState.True
        dgSymbols.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle1
        dgSymbols.ContextMenuStrip = ContextMenuStrip1
        DataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle2.BackColor = Color.FromArgb(CByte(64), CByte(64), CByte(64))
        DataGridViewCellStyle2.Font = New Font("Calibri", 14F, FontStyle.Italic)
        DataGridViewCellStyle2.ForeColor = Color.Aqua
        DataGridViewCellStyle2.SelectionBackColor = Color.Transparent
        DataGridViewCellStyle2.SelectionForeColor = SystemColors.ControlText
        DataGridViewCellStyle2.WrapMode = DataGridViewTriState.True
        dgSymbols.DefaultCellStyle = DataGridViewCellStyle2
        dgSymbols.EnableHeadersVisualStyles = False
        dgSymbols.Location = New Point(6, 25)
        dgSymbols.MultiSelect = False
        dgSymbols.Name = "dgSymbols"
        dgSymbols.ReadOnly = True
        DataGridViewCellStyle3.BackColor = Color.FromArgb(CByte(64), CByte(64), CByte(64))
        DataGridViewCellStyle3.Font = New Font("Calibri", 14F, FontStyle.Bold)
        DataGridViewCellStyle3.ForeColor = SystemColors.ButtonHighlight
        DataGridViewCellStyle3.Padding = New Padding(2)
        DataGridViewCellStyle3.SelectionBackColor = Color.DarkOrange
        DataGridViewCellStyle3.SelectionForeColor = SystemColors.ControlText
        DataGridViewCellStyle3.WrapMode = DataGridViewTriState.True
        dgSymbols.RowHeadersDefaultCellStyle = DataGridViewCellStyle3
        dgSymbols.RowHeadersWidth = 4
        dgSymbols.RowTemplate.DefaultCellStyle.SelectionBackColor = Color.DarkOrange
        dgSymbols.RowTemplate.DefaultCellStyle.SelectionForeColor = Color.FromArgb(CByte(64), CByte(64), CByte(64))
        dgSymbols.RowTemplate.DefaultCellStyle.WrapMode = DataGridViewTriState.True
        dgSymbols.ScrollBars = ScrollBars.Vertical
        dgSymbols.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgSymbols.Size = New Size(210, 294)
        dgSymbols.TabIndex = 12
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
        ' FormSymbols
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        BackColor = Color.FromArgb(CByte(31), CByte(33), CByte(32))
        ClientSize = New Size(250, 407)
        Controls.Add(StatusStrip1)
        Controls.Add(btSalvarEntrada)
        Controls.Add(tbSymbol)
        Controls.Add(Label2)
        Controls.Add(GroupBox1)
        FormBorderStyle = FormBorderStyle.FixedSingle
        MaximizeBox = False
        MinimizeBox = False
        Name = "FormSymbols"
        StartPosition = FormStartPosition.CenterScreen
        Text = "Cadastrar Simbolos"
        StatusStrip1.ResumeLayout(False)
        StatusStrip1.PerformLayout()
        GroupBox1.ResumeLayout(False)
        CType(dgSymbols, ComponentModel.ISupportInitialize).EndInit()
        ContextMenuStrip1.ResumeLayout(False)
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents StatusStrip1 As StatusStrip
    Friend WithEvents ToolStripStatusLabel1 As ToolStripStatusLabel
    Friend WithEvents ToolStripStatusLabel2 As ToolStripStatusLabel
    Friend WithEvents btSalvarEntrada As Button
    Friend WithEvents tbSymbol As MaskedTextBox
    Friend WithEvents Label2 As Label
    Friend WithEvents GroupBox1 As GroupBox
    Friend WithEvents dgSymbols As DataGridView
    Friend WithEvents ContextMenuStrip1 As ContextMenuStrip
    Friend WithEvents ExcluirToolStripMenuItem As ToolStripMenuItem
End Class
