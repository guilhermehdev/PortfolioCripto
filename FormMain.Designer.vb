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
        Dim DataGridViewCellStyle4 As DataGridViewCellStyle = New DataGridViewCellStyle()
        Dim DataGridViewCellStyle5 As DataGridViewCellStyle = New DataGridViewCellStyle()
        Dim DataGridViewCellStyle6 As DataGridViewCellStyle = New DataGridViewCellStyle()
        MenuStrip1 = New MenuStrip()
        CadastroToolStripMenuItem = New ToolStripMenuItem()
        CriptoToolStripMenuItem = New ToolStripMenuItem()
        FecharToolStripMenuItem = New ToolStripMenuItem()
        dgPortfolio = New DataGridView()
        lbDolar = New Label()
        Label1 = New Label()
        PanelProfits = New Panel()
        lbTotalBRL = New Label()
        Label5 = New Label()
        Label3 = New Label()
        lbBTC = New Label()
        Label4 = New Label()
        lbDom = New Label()
        btRefresh = New Button()
        Label2 = New Label()
        MenuStrip1.SuspendLayout()
        CType(dgPortfolio, ComponentModel.ISupportInitialize).BeginInit()
        PanelProfits.SuspendLayout()
        SuspendLayout()
        ' 
        ' MenuStrip1
        ' 
        MenuStrip1.BackColor = Color.FromArgb(CByte(31), CByte(33), CByte(32))
        MenuStrip1.Items.AddRange(New ToolStripItem() {CadastroToolStripMenuItem, FecharToolStripMenuItem})
        MenuStrip1.Location = New Point(0, 0)
        MenuStrip1.Name = "MenuStrip1"
        MenuStrip1.Size = New Size(1084, 24)
        MenuStrip1.TabIndex = 10
        MenuStrip1.Text = "MenuStrip1"
        ' 
        ' CadastroToolStripMenuItem
        ' 
        CadastroToolStripMenuItem.DropDownItems.AddRange(New ToolStripItem() {CriptoToolStripMenuItem})
        CadastroToolStripMenuItem.ForeColor = SystemColors.ButtonHighlight
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
        FecharToolStripMenuItem.ForeColor = SystemColors.ButtonHighlight
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
        dgPortfolio.BackgroundColor = Color.FromArgb(CByte(64), CByte(64), CByte(64))
        DataGridViewCellStyle4.Alignment = DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle4.BackColor = Color.FromArgb(CByte(64), CByte(64), CByte(64))
        DataGridViewCellStyle4.Font = New Font("Segoe UI", 9F)
        DataGridViewCellStyle4.ForeColor = Color.Turquoise
        DataGridViewCellStyle4.SelectionBackColor = Color.DarkOrange
        DataGridViewCellStyle4.SelectionForeColor = SystemColors.ControlText
        DataGridViewCellStyle4.WrapMode = DataGridViewTriState.True
        dgPortfolio.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle4
        dgPortfolio.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize
        DataGridViewCellStyle5.Alignment = DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle5.BackColor = Color.FromArgb(CByte(64), CByte(64), CByte(64))
        DataGridViewCellStyle5.Font = New Font("Calibri", 12F)
        DataGridViewCellStyle5.ForeColor = SystemColors.ButtonHighlight
        DataGridViewCellStyle5.SelectionBackColor = Color.DarkOrange
        DataGridViewCellStyle5.SelectionForeColor = SystemColors.ControlText
        DataGridViewCellStyle5.WrapMode = DataGridViewTriState.False
        dgPortfolio.DefaultCellStyle = DataGridViewCellStyle5
        dgPortfolio.Location = New Point(0, 27)
        dgPortfolio.MultiSelect = False
        dgPortfolio.Name = "dgPortfolio"
        dgPortfolio.ReadOnly = True
        DataGridViewCellStyle6.Alignment = DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle6.BackColor = Color.FromArgb(CByte(64), CByte(64), CByte(64))
        DataGridViewCellStyle6.Font = New Font("Segoe UI", 9F)
        DataGridViewCellStyle6.ForeColor = SystemColors.ButtonHighlight
        DataGridViewCellStyle6.Padding = New Padding(2)
        DataGridViewCellStyle6.SelectionBackColor = Color.Orange
        DataGridViewCellStyle6.SelectionForeColor = SystemColors.ControlText
        DataGridViewCellStyle6.WrapMode = DataGridViewTriState.True
        dgPortfolio.RowHeadersDefaultCellStyle = DataGridViewCellStyle6
        dgPortfolio.RowHeadersWidth = 4
        dgPortfolio.RowTemplate.DefaultCellStyle.SelectionBackColor = Color.DarkOrange
        dgPortfolio.RowTemplate.DefaultCellStyle.SelectionForeColor = SystemColors.ControlText
        dgPortfolio.ScrollBars = ScrollBars.Vertical
        dgPortfolio.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgPortfolio.Size = New Size(1084, 388)
        dgPortfolio.TabIndex = 11
        ' 
        ' lbDolar
        ' 
        lbDolar.AutoSize = True
        lbDolar.BackColor = Color.Transparent
        lbDolar.FlatStyle = FlatStyle.Flat
        lbDolar.Font = New Font("Segoe UI", 10F, FontStyle.Bold)
        lbDolar.ForeColor = Color.ForestGreen
        lbDolar.Location = New Point(654, 3)
        lbDolar.Name = "lbDolar"
        lbDolar.Size = New Size(45, 19)
        lbDolar.TabIndex = 12
        lbDolar.Text = "$0.00"
        ' 
        ' Label1
        ' 
        Label1.AutoSize = True
        Label1.BackColor = Color.Transparent
        Label1.FlatStyle = FlatStyle.Flat
        Label1.Font = New Font("Segoe UI", 10F, FontStyle.Bold)
        Label1.ForeColor = Color.LightGray
        Label1.Location = New Point(608, 3)
        Label1.Name = "Label1"
        Label1.Size = New Size(51, 19)
        Label1.TabIndex = 13
        Label1.Text = "Dolar:"
        ' 
        ' PanelProfits
        ' 
        PanelProfits.BackColor = Color.FromArgb(CByte(56), CByte(86), CByte(35))
        PanelProfits.Controls.Add(lbTotalBRL)
        PanelProfits.Controls.Add(Label5)
        PanelProfits.Dock = DockStyle.Bottom
        PanelProfits.Location = New Point(0, 503)
        PanelProfits.Name = "PanelProfits"
        PanelProfits.Size = New Size(1084, 37)
        PanelProfits.TabIndex = 15
        ' 
        ' lbTotalBRL
        ' 
        lbTotalBRL.AutoSize = True
        lbTotalBRL.Font = New Font("Segoe UI", 16F, FontStyle.Bold)
        lbTotalBRL.ForeColor = Color.LawnGreen
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
        Label5.Size = New Size(76, 30)
        Label5.TabIndex = 15
        Label5.Text = "Lucro:"
        ' 
        ' Label3
        ' 
        Label3.AutoSize = True
        Label3.BackColor = Color.Transparent
        Label3.FlatStyle = FlatStyle.Flat
        Label3.Font = New Font("Segoe UI", 10F, FontStyle.Bold)
        Label3.ForeColor = Color.LightGray
        Label3.Location = New Point(724, 3)
        Label3.Name = "Label3"
        Label3.Size = New Size(39, 19)
        Label3.TabIndex = 17
        Label3.Text = "BTC:"
        ' 
        ' lbBTC
        ' 
        lbBTC.AutoSize = True
        lbBTC.BackColor = Color.Transparent
        lbBTC.FlatStyle = FlatStyle.Flat
        lbBTC.Font = New Font("Segoe UI", 10F, FontStyle.Bold)
        lbBTC.ForeColor = Color.DarkOrange
        lbBTC.Location = New Point(758, 3)
        lbBTC.Name = "lbBTC"
        lbBTC.Size = New Size(45, 19)
        lbBTC.TabIndex = 16
        lbBTC.Text = "$0.00"
        ' 
        ' Label4
        ' 
        Label4.AutoSize = True
        Label4.BackColor = Color.Transparent
        Label4.FlatStyle = FlatStyle.Flat
        Label4.Font = New Font("Segoe UI", 10F, FontStyle.Bold)
        Label4.ForeColor = Color.LightGray
        Label4.Location = New Point(852, 3)
        Label4.Name = "Label4"
        Label4.Size = New Size(75, 19)
        Label4.TabIndex = 19
        Label4.Text = "Dom BTC:"
        ' 
        ' lbDom
        ' 
        lbDom.AutoSize = True
        lbDom.BackColor = Color.Transparent
        lbDom.FlatStyle = FlatStyle.Flat
        lbDom.Font = New Font("Segoe UI", 10F, FontStyle.Bold)
        lbDom.ForeColor = Color.DeepSkyBlue
        lbDom.Location = New Point(923, 3)
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
        btRefresh.Location = New Point(519, 2)
        btRefresh.Name = "btRefresh"
        btRefresh.Size = New Size(75, 23)
        btRefresh.TabIndex = 20
        btRefresh.Text = "Atualizar"
        btRefresh.UseVisualStyleBackColor = False
        ' 
        ' Label2
        ' 
        Label2.AutoSize = True
        Label2.Font = New Font("Calibri", 14F, FontStyle.Bold Or FontStyle.Italic)
        Label2.ForeColor = Color.LightGray
        Label2.Location = New Point(7, 418)
        Label2.Name = "Label2"
        Label2.Size = New Size(97, 23)
        Label2.TabIndex = 21
        Label2.Text = "Estatísticas"
        ' 
        ' FormMain
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        BackColor = Color.FromArgb(CByte(31), CByte(33), CByte(32))
        ClientSize = New Size(1084, 540)
        Controls.Add(Label2)
        Controls.Add(PanelProfits)
        Controls.Add(btRefresh)
        Controls.Add(Label4)
        Controls.Add(lbDom)
        Controls.Add(Label3)
        Controls.Add(lbBTC)
        Controls.Add(Label1)
        Controls.Add(lbDolar)
        Controls.Add(dgPortfolio)
        Controls.Add(MenuStrip1)
        Font = New Font("Segoe UI", 9F)
        FormBorderStyle = FormBorderStyle.FixedSingle
        MainMenuStrip = MenuStrip1
        Name = "FormMain"
        StartPosition = FormStartPosition.CenterScreen
        Text = "Portfolio Cripto"
        MenuStrip1.ResumeLayout(False)
        MenuStrip1.PerformLayout()
        CType(dgPortfolio, ComponentModel.ISupportInitialize).EndInit()
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
    Friend WithEvents Label3 As Label
    Friend WithEvents lbBTC As Label
    Friend WithEvents Label4 As Label
    Friend WithEvents lbDom As Label
    Friend WithEvents btRefresh As Button
    Friend WithEvents PanelProfits As Panel
    Friend WithEvents Label5 As Label
    Friend WithEvents lbTotalBRL As Label
    Friend WithEvents Label2 As Label

End Class
