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
        Dim DataGridViewCellStyle3 As DataGridViewCellStyle = New DataGridViewCellStyle()
        Dim DataGridViewCellStyle4 As DataGridViewCellStyle = New DataGridViewCellStyle()
        MenuStrip1 = New MenuStrip()
        CadastroToolStripMenuItem = New ToolStripMenuItem()
        CriptoToolStripMenuItem = New ToolStripMenuItem()
        FecharToolStripMenuItem = New ToolStripMenuItem()
        dgPortfolio = New DataGridView()
        lbDolar = New Label()
        Label1 = New Label()
        Label2 = New Label()
        GroupBox1 = New GroupBox()
        PanelProfits = New Panel()
        lbDiv = New Label()
        lbTotalBRL = New Label()
        Label5 = New Label()
        Label3 = New Label()
        lbBTC = New Label()
        Label4 = New Label()
        lbDom = New Label()
        btRefresh = New Button()
        MenuStrip1.SuspendLayout()
        CType(dgPortfolio, ComponentModel.ISupportInitialize).BeginInit()
        GroupBox1.SuspendLayout()
        PanelProfits.SuspendLayout()
        SuspendLayout()
        ' 
        ' MenuStrip1
        ' 
        MenuStrip1.BackColor = SystemColors.ButtonHighlight
        MenuStrip1.Items.AddRange(New ToolStripItem() {CadastroToolStripMenuItem, FecharToolStripMenuItem})
        MenuStrip1.Location = New Point(0, 0)
        MenuStrip1.Name = "MenuStrip1"
        MenuStrip1.Size = New Size(1001, 24)
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
        DataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle3.BackColor = SystemColors.Control
        DataGridViewCellStyle3.Font = New Font("Segoe UI", 9F)
        DataGridViewCellStyle3.ForeColor = SystemColors.WindowText
        DataGridViewCellStyle3.SelectionBackColor = SystemColors.Highlight
        DataGridViewCellStyle3.SelectionForeColor = SystemColors.HighlightText
        DataGridViewCellStyle3.WrapMode = DataGridViewTriState.True
        dgPortfolio.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle3
        dgPortfolio.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize
        DataGridViewCellStyle4.Alignment = DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle4.BackColor = SystemColors.Window
        DataGridViewCellStyle4.Font = New Font("Calibri", 12F)
        DataGridViewCellStyle4.ForeColor = SystemColors.ControlText
        DataGridViewCellStyle4.SelectionBackColor = SystemColors.Highlight
        DataGridViewCellStyle4.SelectionForeColor = SystemColors.HighlightText
        DataGridViewCellStyle4.WrapMode = DataGridViewTriState.False
        dgPortfolio.DefaultCellStyle = DataGridViewCellStyle4
        dgPortfolio.Location = New Point(12, 27)
        dgPortfolio.MultiSelect = False
        dgPortfolio.Name = "dgPortfolio"
        dgPortfolio.ReadOnly = True
        dgPortfolio.RowHeadersWidth = 4
        dgPortfolio.ScrollBars = ScrollBars.Vertical
        dgPortfolio.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgPortfolio.Size = New Size(977, 335)
        dgPortfolio.TabIndex = 11
        ' 
        ' lbDolar
        ' 
        lbDolar.AutoSize = True
        lbDolar.BackColor = SystemColors.ButtonHighlight
        lbDolar.FlatStyle = FlatStyle.Flat
        lbDolar.Font = New Font("Segoe UI", 10F, FontStyle.Bold)
        lbDolar.ForeColor = Color.ForestGreen
        lbDolar.Location = New Point(667, 3)
        lbDolar.Name = "lbDolar"
        lbDolar.Size = New Size(45, 19)
        lbDolar.TabIndex = 12
        lbDolar.Text = "$0.00"
        ' 
        ' Label1
        ' 
        Label1.AutoSize = True
        Label1.BackColor = SystemColors.ButtonHighlight
        Label1.FlatStyle = FlatStyle.Flat
        Label1.Font = New Font("Segoe UI", 10F, FontStyle.Bold)
        Label1.ForeColor = Color.Black
        Label1.Location = New Point(621, 3)
        Label1.Name = "Label1"
        Label1.Size = New Size(51, 19)
        Label1.TabIndex = 13
        Label1.Text = "Dolar:"
        ' 
        ' Label2
        ' 
        Label2.AutoSize = True
        Label2.Font = New Font("Segoe UI", 16F, FontStyle.Bold)
        Label2.ForeColor = Color.Chartreuse
        Label2.Location = New Point(353, 3)
        Label2.Name = "Label2"
        Label2.Size = New Size(71, 30)
        Label2.TabIndex = 14
        Label2.Text = "$0.00"
        Label2.TextAlign = ContentAlignment.MiddleCenter
        ' 
        ' GroupBox1
        ' 
        GroupBox1.Controls.Add(PanelProfits)
        GroupBox1.Dock = DockStyle.Bottom
        GroupBox1.Location = New Point(0, 368)
        GroupBox1.Name = "GroupBox1"
        GroupBox1.Size = New Size(1001, 111)
        GroupBox1.TabIndex = 15
        GroupBox1.TabStop = False
        GroupBox1.Text = "Estatisticas"
        ' 
        ' PanelProfits
        ' 
        PanelProfits.BackColor = Color.Indigo
        PanelProfits.Controls.Add(lbDiv)
        PanelProfits.Controls.Add(lbTotalBRL)
        PanelProfits.Controls.Add(Label5)
        PanelProfits.Controls.Add(Label2)
        PanelProfits.Dock = DockStyle.Bottom
        PanelProfits.Location = New Point(3, 71)
        PanelProfits.Name = "PanelProfits"
        PanelProfits.Size = New Size(995, 37)
        PanelProfits.TabIndex = 15
        ' 
        ' lbDiv
        ' 
        lbDiv.AutoSize = True
        lbDiv.Font = New Font("Segoe UI", 16F, FontStyle.Bold)
        lbDiv.ForeColor = Color.White
        lbDiv.Location = New Point(424, 3)
        lbDiv.Name = "lbDiv"
        lbDiv.Size = New Size(33, 30)
        lbDiv.TabIndex = 17
        lbDiv.Text = "//"
        lbDiv.TextAlign = ContentAlignment.MiddleCenter
        ' 
        ' lbTotalBRL
        ' 
        lbTotalBRL.AutoSize = True
        lbTotalBRL.Font = New Font("Segoe UI", 16F, FontStyle.Bold)
        lbTotalBRL.ForeColor = Color.Yellow
        lbTotalBRL.Location = New Point(456, 3)
        lbTotalBRL.Name = "lbTotalBRL"
        lbTotalBRL.Size = New Size(91, 30)
        lbTotalBRL.TabIndex = 16
        lbTotalBRL.Text = "R$ 0,00"
        lbTotalBRL.TextAlign = ContentAlignment.MiddleCenter
        ' 
        ' Label5
        ' 
        Label5.AutoSize = True
        Label5.Font = New Font("Segoe UI", 16F, FontStyle.Bold)
        Label5.ForeColor = Color.White
        Label5.Location = New Point(7, 3)
        Label5.Name = "Label5"
        Label5.Size = New Size(184, 30)
        Label5.TabIndex = 15
        Label5.Text = "Ganhos e Perdas"
        ' 
        ' Label3
        ' 
        Label3.AutoSize = True
        Label3.BackColor = SystemColors.ButtonHighlight
        Label3.FlatStyle = FlatStyle.Flat
        Label3.Font = New Font("Segoe UI", 10F, FontStyle.Bold)
        Label3.ForeColor = Color.Black
        Label3.Location = New Point(742, 3)
        Label3.Name = "Label3"
        Label3.Size = New Size(39, 19)
        Label3.TabIndex = 17
        Label3.Text = "BTC:"
        ' 
        ' lbBTC
        ' 
        lbBTC.AutoSize = True
        lbBTC.BackColor = SystemColors.ButtonHighlight
        lbBTC.FlatStyle = FlatStyle.Flat
        lbBTC.Font = New Font("Segoe UI", 10F, FontStyle.Bold)
        lbBTC.ForeColor = Color.DarkOrange
        lbBTC.Location = New Point(776, 3)
        lbBTC.Name = "lbBTC"
        lbBTC.Size = New Size(45, 19)
        lbBTC.TabIndex = 16
        lbBTC.Text = "$0.00"
        ' 
        ' Label4
        ' 
        Label4.AutoSize = True
        Label4.BackColor = SystemColors.ButtonHighlight
        Label4.FlatStyle = FlatStyle.Flat
        Label4.Font = New Font("Segoe UI", 10F, FontStyle.Bold)
        Label4.ForeColor = Color.Black
        Label4.Location = New Point(865, 3)
        Label4.Name = "Label4"
        Label4.Size = New Size(75, 19)
        Label4.TabIndex = 19
        Label4.Text = "Dom BTC:"
        ' 
        ' lbDom
        ' 
        lbDom.AutoSize = True
        lbDom.BackColor = SystemColors.ButtonHighlight
        lbDom.FlatStyle = FlatStyle.Flat
        lbDom.Font = New Font("Segoe UI", 10F, FontStyle.Bold)
        lbDom.ForeColor = Color.DeepSkyBlue
        lbDom.Location = New Point(936, 3)
        lbDom.Name = "lbDom"
        lbDom.Size = New Size(29, 19)
        lbDom.TabIndex = 18
        lbDom.Text = "0%"
        ' 
        ' btRefresh
        ' 
        btRefresh.BackColor = Color.IndianRed
        btRefresh.Cursor = Cursors.Hand
        btRefresh.FlatAppearance.BorderSize = 0
        btRefresh.FlatStyle = FlatStyle.Popup
        btRefresh.Font = New Font("Calibri", 10F)
        btRefresh.ForeColor = Color.Transparent
        btRefresh.Location = New Point(532, 2)
        btRefresh.Name = "btRefresh"
        btRefresh.Size = New Size(75, 23)
        btRefresh.TabIndex = 20
        btRefresh.Text = "Atualizar"
        btRefresh.UseVisualStyleBackColor = False
        ' 
        ' FormMain
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(1001, 479)
        Controls.Add(btRefresh)
        Controls.Add(Label4)
        Controls.Add(lbDom)
        Controls.Add(Label3)
        Controls.Add(lbBTC)
        Controls.Add(GroupBox1)
        Controls.Add(Label1)
        Controls.Add(lbDolar)
        Controls.Add(dgPortfolio)
        Controls.Add(MenuStrip1)
        Font = New Font("Segoe UI", 9F)
        MainMenuStrip = MenuStrip1
        Name = "FormMain"
        StartPosition = FormStartPosition.CenterScreen
        Text = "Portfolio Cripto"
        MenuStrip1.ResumeLayout(False)
        MenuStrip1.PerformLayout()
        CType(dgPortfolio, ComponentModel.ISupportInitialize).EndInit()
        GroupBox1.ResumeLayout(False)
        PanelProfits.ResumeLayout(False)
        PanelProfits.PerformLayout()
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
    Friend WithEvents Label2 As Label
    Friend WithEvents GroupBox1 As GroupBox
    Friend WithEvents Label3 As Label
    Friend WithEvents lbBTC As Label
    Friend WithEvents Label4 As Label
    Friend WithEvents lbDom As Label
    Friend WithEvents btRefresh As Button
    Friend WithEvents PanelProfits As Panel
    Friend WithEvents Label5 As Label
    Friend WithEvents lbTotalBRL As Label
    Friend WithEvents lbDiv As Label

End Class
