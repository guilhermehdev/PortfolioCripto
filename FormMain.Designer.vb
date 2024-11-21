<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class FormMain
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Dim DataGridViewCellStyle1 As DataGridViewCellStyle = New DataGridViewCellStyle()
        Dim DataGridViewCellStyle2 As DataGridViewCellStyle = New DataGridViewCellStyle()
        MenuStrip1 = New MenuStrip()
        CadastroToolStripMenuItem = New ToolStripMenuItem()
        CriptoToolStripMenuItem = New ToolStripMenuItem()
        FecharToolStripMenuItem = New ToolStripMenuItem()
        dgPortfolio = New DataGridView()
        lbDolar = New Label()
        Label1 = New Label()
        MenuStrip1.SuspendLayout()
        CType(dgPortfolio, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        ' 
        ' MenuStrip1
        ' 
        MenuStrip1.Items.AddRange(New ToolStripItem() {CadastroToolStripMenuItem, FecharToolStripMenuItem})
        MenuStrip1.Location = New Point(0, 0)
        MenuStrip1.Name = "MenuStrip1"
        MenuStrip1.Size = New Size(921, 24)
        MenuStrip1.TabIndex = 10
        MenuStrip1.Text = "MenuStrip1"
        ' 
        ' CadastroToolStripMenuItem
        ' 
        CadastroToolStripMenuItem.DropDownItems.AddRange(New ToolStripItem() {CriptoToolStripMenuItem})
        CadastroToolStripMenuItem.Name = "CadastroToolStripMenuItem"
        CadastroToolStripMenuItem.Size = New Size(66, 20)
        CadastroToolStripMenuItem.Text = "Cadastro"
        ' 
        ' CriptoToolStripMenuItem
        ' 
        CriptoToolStripMenuItem.Name = "CriptoToolStripMenuItem"
        CriptoToolStripMenuItem.Size = New Size(119, 22)
        CriptoToolStripMenuItem.Text = "Entradas"
        ' 
        ' FecharToolStripMenuItem
        ' 
        FecharToolStripMenuItem.Name = "FecharToolStripMenuItem"
        FecharToolStripMenuItem.Size = New Size(54, 20)
        FecharToolStripMenuItem.Text = "Fechar"
        ' 
        ' dgPortfolio
        ' 
        dgPortfolio.AllowUserToAddRows = False
        dgPortfolio.AllowUserToDeleteRows = False
        dgPortfolio.AllowUserToOrderColumns = True
        dgPortfolio.Anchor = AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
        dgPortfolio.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        dgPortfolio.BackgroundColor = Color.White
        DataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle1.BackColor = SystemColors.Control
        DataGridViewCellStyle1.Font = New Font("Segoe UI", 9F)
        DataGridViewCellStyle1.ForeColor = SystemColors.WindowText
        DataGridViewCellStyle1.SelectionBackColor = SystemColors.Highlight
        DataGridViewCellStyle1.SelectionForeColor = SystemColors.HighlightText
        DataGridViewCellStyle1.WrapMode = DataGridViewTriState.True
        dgPortfolio.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle1
        dgPortfolio.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize
        DataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle2.BackColor = SystemColors.Window
        DataGridViewCellStyle2.Font = New Font("Calibri", 12F)
        DataGridViewCellStyle2.ForeColor = SystemColors.ControlText
        DataGridViewCellStyle2.SelectionBackColor = SystemColors.Highlight
        DataGridViewCellStyle2.SelectionForeColor = SystemColors.HighlightText
        DataGridViewCellStyle2.WrapMode = DataGridViewTriState.False
        dgPortfolio.DefaultCellStyle = DataGridViewCellStyle2
        dgPortfolio.Location = New Point(12, 27)
        dgPortfolio.MultiSelect = False
        dgPortfolio.Name = "dgPortfolio"
        dgPortfolio.ReadOnly = True
        dgPortfolio.RowHeadersWidth = 4
        dgPortfolio.ScrollBars = ScrollBars.Vertical
        dgPortfolio.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgPortfolio.Size = New Size(897, 372)
        dgPortfolio.TabIndex = 11
        ' 
        ' lbDolar
        ' 
        lbDolar.AutoSize = True
        lbDolar.FlatStyle = FlatStyle.Flat
        lbDolar.Font = New Font("Segoe UI", 10F, FontStyle.Bold)
        lbDolar.ForeColor = Color.ForestGreen
        lbDolar.Location = New Point(186, 2)
        lbDolar.Name = "lbDolar"
        lbDolar.Size = New Size(45, 19)
        lbDolar.TabIndex = 12
        lbDolar.Text = "$0.00"
        ' 
        ' Label1
        ' 
        Label1.AutoSize = True
        Label1.FlatStyle = FlatStyle.Flat
        Label1.Font = New Font("Segoe UI", 10F, FontStyle.Bold)
        Label1.ForeColor = Color.Black
        Label1.Location = New Point(140, 2)
        Label1.Name = "Label1"
        Label1.Size = New Size(51, 19)
        Label1.TabIndex = 13
        Label1.Text = "Dolar:"
        ' 
        ' FormMain
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(921, 477)
        Controls.Add(Label1)
        Controls.Add(lbDolar)
        Controls.Add(dgPortfolio)
        Controls.Add(MenuStrip1)
        Font = New Font("Segoe UI", 9F)
        MainMenuStrip = MenuStrip1
        MaximizeBox = False
        Name = "FormMain"
        StartPosition = FormStartPosition.CenterScreen
        Text = "Portfolio Cripto"
        MenuStrip1.ResumeLayout(False)
        MenuStrip1.PerformLayout()
        CType(dgPortfolio, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)
        PerformLayout()
    End Sub
    Friend WithEvents MenuStrip1 As MenuStrip
    Friend WithEvents CadastroToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents CriptoToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents FecharToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents dgPortfolio As DataGridView
    Friend WithEvents lbDolar As Label
    Friend WithEvents Label1 As Label

End Class
