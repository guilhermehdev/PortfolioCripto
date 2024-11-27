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
        components = New ComponentModel.Container()
        Dim DataGridViewCellStyle1 As DataGridViewCellStyle = New DataGridViewCellStyle()
        Dim DataGridViewCellStyle2 As DataGridViewCellStyle = New DataGridViewCellStyle()
        Dim DataGridViewCellStyle3 As DataGridViewCellStyle = New DataGridViewCellStyle()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FormMain))
        MenuStrip1 = New MenuStrip()
        CadastroToolStripMenuItem = New ToolStripMenuItem()
        CriptoToolStripMenuItem = New ToolStripMenuItem()
        OpçõesToolStripMenuItem = New ToolStripMenuItem()
        IntervaloToolStripMenuItem = New ToolStripMenuItem()
        JSONToolStripMenuItem = New ToolStripMenuItem()
        FecharToolStripMenuItem = New ToolStripMenuItem()
        dgPortfolio = New DataGridView()
        PanelProfits = New Panel()
        Label15 = New Label()
        lbTotalBRL = New Label()
        Label5 = New Label()
        GroupOverview = New GroupBox()
        lbPerformWallet = New Label()
        Label16 = New Label()
        Label14 = New Label()
        Label13 = New Label()
        lbDataTotalToday = New Label()
        Label10 = New Label()
        Label11 = New Label()
        Label12 = New Label()
        Label8 = New Label()
        Label9 = New Label()
        Label7 = New Label()
        lbTotalEntradaBRL = New Label()
        lbTotalEntrada = New Label()
        Label6 = New Label()
        lbLoadFromMarket = New Label()
        Panel1 = New Panel()
        lbAtualizaEm = New Label()
        lbRefresh = New Label()
        btRefresh = New Button()
        Label4 = New Label()
        lbDom = New Label()
        Label3 = New Label()
        lbBTC = New Label()
        Label1 = New Label()
        lbDolar = New Label()
        NotifyIcon1 = New NotifyIcon(components)
        TimerRefresh = New Timer(components)
        TimerCountdown = New Timer(components)
        TimerBlink = New Timer(components)
        MenuStrip1.SuspendLayout()
        CType(dgPortfolio, ComponentModel.ISupportInitialize).BeginInit()
        PanelProfits.SuspendLayout()
        GroupOverview.SuspendLayout()
        Panel1.SuspendLayout()
        SuspendLayout()
        ' 
        ' MenuStrip1
        ' 
        MenuStrip1.BackColor = Color.FromArgb(CByte(31), CByte(33), CByte(32))
        MenuStrip1.Items.AddRange(New ToolStripItem() {CadastroToolStripMenuItem, OpçõesToolStripMenuItem, FecharToolStripMenuItem})
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
        CriptoToolStripMenuItem.BackColor = Color.FromArgb(CByte(31), CByte(33), CByte(32))
        CriptoToolStripMenuItem.ForeColor = SystemColors.ButtonHighlight
        CriptoToolStripMenuItem.Name = "CriptoToolStripMenuItem"
        CriptoToolStripMenuItem.Size = New Size(119, 22)
        CriptoToolStripMenuItem.Text = "Entradas"
        ' 
        ' OpçõesToolStripMenuItem
        ' 
        OpçõesToolStripMenuItem.DropDownItems.AddRange(New ToolStripItem() {IntervaloToolStripMenuItem, JSONToolStripMenuItem})
        OpçõesToolStripMenuItem.ForeColor = SystemColors.ButtonHighlight
        OpçõesToolStripMenuItem.Name = "OpçõesToolStripMenuItem"
        OpçõesToolStripMenuItem.Size = New Size(96, 20)
        OpçõesToolStripMenuItem.Text = "Configurações"
        ' 
        ' IntervaloToolStripMenuItem
        ' 
        IntervaloToolStripMenuItem.BackColor = SystemColors.ActiveCaptionText
        IntervaloToolStripMenuItem.ForeColor = SystemColors.ButtonHighlight
        IntervaloToolStripMenuItem.Name = "IntervaloToolStripMenuItem"
        IntervaloToolStripMenuItem.Size = New Size(120, 22)
        IntervaloToolStripMenuItem.Text = "Intervalo"
        ' 
        ' JSONToolStripMenuItem
        ' 
        JSONToolStripMenuItem.BackColor = Color.FromArgb(CByte(31), CByte(33), CByte(32))
        JSONToolStripMenuItem.ForeColor = SystemColors.ButtonHighlight
        JSONToolStripMenuItem.Name = "JSONToolStripMenuItem"
        JSONToolStripMenuItem.Size = New Size(120, 22)
        JSONToolStripMenuItem.Text = "JSON"
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
        dgPortfolio.AllowUserToResizeRows = False
        dgPortfolio.Anchor = AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
        dgPortfolio.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        dgPortfolio.BackgroundColor = Color.FromArgb(CByte(64), CByte(64), CByte(64))
        dgPortfolio.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None
        DataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle1.BackColor = Color.FromArgb(CByte(64), CByte(64), CByte(64))
        DataGridViewCellStyle1.Font = New Font("Segoe UI", 9F)
        DataGridViewCellStyle1.ForeColor = Color.Turquoise
        DataGridViewCellStyle1.SelectionBackColor = Color.Transparent
        DataGridViewCellStyle1.SelectionForeColor = SystemColors.ControlText
        DataGridViewCellStyle1.WrapMode = DataGridViewTriState.True
        dgPortfolio.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle1
        DataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle2.BackColor = Color.FromArgb(CByte(64), CByte(64), CByte(64))
        DataGridViewCellStyle2.Font = New Font("Calibri", 12F)
        DataGridViewCellStyle2.ForeColor = SystemColors.ButtonHighlight
        DataGridViewCellStyle2.SelectionBackColor = Color.Transparent
        DataGridViewCellStyle2.SelectionForeColor = SystemColors.ControlText
        DataGridViewCellStyle2.WrapMode = DataGridViewTriState.True
        dgPortfolio.DefaultCellStyle = DataGridViewCellStyle2
        dgPortfolio.EnableHeadersVisualStyles = False
        dgPortfolio.Location = New Point(0, 27)
        dgPortfolio.MultiSelect = False
        dgPortfolio.Name = "dgPortfolio"
        dgPortfolio.ReadOnly = True
        DataGridViewCellStyle3.BackColor = Color.FromArgb(CByte(64), CByte(64), CByte(64))
        DataGridViewCellStyle3.Font = New Font("Segoe UI", 9F)
        DataGridViewCellStyle3.ForeColor = SystemColors.ButtonHighlight
        DataGridViewCellStyle3.Padding = New Padding(2)
        DataGridViewCellStyle3.SelectionBackColor = Color.DarkOrange
        DataGridViewCellStyle3.SelectionForeColor = SystemColors.ControlText
        DataGridViewCellStyle3.WrapMode = DataGridViewTriState.True
        dgPortfolio.RowHeadersDefaultCellStyle = DataGridViewCellStyle3
        dgPortfolio.RowHeadersWidth = 4
        dgPortfolio.RowTemplate.DefaultCellStyle.SelectionBackColor = Color.DarkOrange
        dgPortfolio.RowTemplate.DefaultCellStyle.SelectionForeColor = Color.FromArgb(CByte(64), CByte(64), CByte(64))
        dgPortfolio.RowTemplate.DefaultCellStyle.WrapMode = DataGridViewTriState.True
        dgPortfolio.ScrollBars = ScrollBars.Vertical
        dgPortfolio.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgPortfolio.Size = New Size(1084, 350)
        dgPortfolio.TabIndex = 11
        ' 
        ' PanelProfits
        ' 
        PanelProfits.BackColor = Color.FromArgb(CByte(56), CByte(86), CByte(35))
        PanelProfits.Controls.Add(Label15)
        PanelProfits.Controls.Add(lbTotalBRL)
        PanelProfits.Controls.Add(Label5)
        PanelProfits.Dock = DockStyle.Bottom
        PanelProfits.Location = New Point(0, 503)
        PanelProfits.Name = "PanelProfits"
        PanelProfits.Size = New Size(1084, 37)
        PanelProfits.TabIndex = 15
        ' 
        ' Label15
        ' 
        Label15.Anchor = AnchorStyles.Bottom Or AnchorStyles.Left
        Label15.AutoSize = True
        Label15.BackColor = Color.Transparent
        Label15.Font = New Font("Candara", 14F, FontStyle.Bold)
        Label15.ForeColor = Color.DarkTurquoise
        Label15.Location = New Point(5, 6)
        Label15.Name = "Label15"
        Label15.Size = New Size(60, 23)
        Label15.TabIndex = 17
        Label15.Text = "Lucro:"
        ' 
        ' lbTotalBRL
        ' 
        lbTotalBRL.AutoSize = True
        lbTotalBRL.Font = New Font("Segoe UI", 16F, FontStyle.Bold)
        lbTotalBRL.ForeColor = Color.Lime
        lbTotalBRL.Location = New Point(496, 3)
        lbTotalBRL.Name = "lbTotalBRL"
        lbTotalBRL.Size = New Size(91, 30)
        lbTotalBRL.TabIndex = 16
        lbTotalBRL.Text = "R$ 0,00"
        lbTotalBRL.TextAlign = ContentAlignment.MiddleCenter
        lbTotalBRL.Visible = False
        ' 
        ' Label5
        ' 
        Label5.AutoSize = True
        Label5.Font = New Font("Segoe UI", 16F, FontStyle.Bold)
        Label5.ForeColor = Color.White
        Label5.Location = New Point(7, 3)
        Label5.Name = "Label5"
        Label5.Size = New Size(0, 30)
        Label5.TabIndex = 15
        ' 
        ' GroupOverview
        ' 
        GroupOverview.Controls.Add(lbPerformWallet)
        GroupOverview.Controls.Add(Label16)
        GroupOverview.Controls.Add(Label14)
        GroupOverview.Controls.Add(Label13)
        GroupOverview.Controls.Add(lbDataTotalToday)
        GroupOverview.Controls.Add(Label10)
        GroupOverview.Controls.Add(Label11)
        GroupOverview.Controls.Add(Label12)
        GroupOverview.Controls.Add(Label8)
        GroupOverview.Controls.Add(Label9)
        GroupOverview.Controls.Add(Label7)
        GroupOverview.Controls.Add(lbTotalEntradaBRL)
        GroupOverview.Controls.Add(lbTotalEntrada)
        GroupOverview.Controls.Add(Label6)
        GroupOverview.Dock = DockStyle.Bottom
        GroupOverview.Font = New Font("Calibri", 14F, FontStyle.Italic)
        GroupOverview.ForeColor = Color.Bisque
        GroupOverview.Location = New Point(0, 393)
        GroupOverview.Name = "GroupOverview"
        GroupOverview.Size = New Size(1084, 110)
        GroupOverview.TabIndex = 21
        GroupOverview.TabStop = False
        GroupOverview.Text = "Visão geral"
        ' 
        ' lbPerformWallet
        ' 
        lbPerformWallet.AutoSize = True
        lbPerformWallet.Font = New Font("Calibri", 14F, FontStyle.Italic)
        lbPerformWallet.ForeColor = Color.GreenYellow
        lbPerformWallet.Location = New Point(81, 65)
        lbPerformWallet.Name = "lbPerformWallet"
        lbPerformWallet.Size = New Size(34, 23)
        lbPerformWallet.TabIndex = 13
        lbPerformWallet.Text = "0%"
        ' 
        ' Label16
        ' 
        Label16.AutoSize = True
        Label16.Font = New Font("Calibri", 12F, FontStyle.Italic)
        Label16.ForeColor = Color.White
        Label16.Location = New Point(12, 41)
        Label16.Name = "Label16"
        Label16.Size = New Size(172, 19)
        Label16.TabIndex = 12
        Label16.Text = "Desempenho da carteira:"
        ' 
        ' Label14
        ' 
        Label14.AutoSize = True
        Label14.Location = New Point(464, 21)
        Label14.Name = "Label14"
        Label14.Size = New Size(38, 23)
        Label14.TabIndex = 11
        Label14.Text = "BRL"
        ' 
        ' Label13
        ' 
        Label13.AutoSize = True
        Label13.Location = New Point(395, 21)
        Label13.Name = "Label13"
        Label13.Size = New Size(43, 23)
        Label13.TabIndex = 10
        Label13.Text = "USD"
        ' 
        ' lbDataTotalToday
        ' 
        lbDataTotalToday.AutoSize = True
        lbDataTotalToday.Font = New Font("Calibri", 12F, FontStyle.Italic)
        lbDataTotalToday.ForeColor = Color.DeepSkyBlue
        lbDataTotalToday.Location = New Point(301, 63)
        lbDataTotalToday.Name = "lbDataTotalToday"
        lbDataTotalToday.Size = New Size(85, 19)
        lbDataTotalToday.TabIndex = 9
        lbDataTotalToday.Text = "00/00/0000"
        ' 
        ' Label10
        ' 
        Label10.AutoSize = True
        Label10.Font = New Font("Calibri", 12F, FontStyle.Italic)
        Label10.Location = New Point(464, 63)
        Label10.Name = "Label10"
        Label10.Size = New Size(37, 19)
        Label10.TabIndex = 8
        Label10.Text = "0,00"
        ' 
        ' Label11
        ' 
        Label11.AutoSize = True
        Label11.Font = New Font("Calibri", 12F, FontStyle.Italic)
        Label11.Location = New Point(395, 63)
        Label11.Name = "Label11"
        Label11.Size = New Size(37, 19)
        Label11.TabIndex = 7
        Label11.Text = "0.00"
        ' 
        ' Label12
        ' 
        Label12.AutoSize = True
        Label12.Font = New Font("Calibri", 12F, FontStyle.Italic)
        Label12.ForeColor = Color.White
        Label12.Location = New Point(222, 63)
        Label12.Name = "Label12"
        Label12.Size = New Size(81, 19)
        Label12.TabIndex = 6
        Label12.Text = "Valores em"
        ' 
        ' Label8
        ' 
        Label8.AutoSize = True
        Label8.Font = New Font("Calibri", 12F, FontStyle.Italic)
        Label8.Location = New Point(464, 84)
        Label8.Name = "Label8"
        Label8.Size = New Size(37, 19)
        Label8.TabIndex = 5
        Label8.Text = "0,00"
        ' 
        ' Label9
        ' 
        Label9.AutoSize = True
        Label9.Font = New Font("Calibri", 12F, FontStyle.Italic)
        Label9.Location = New Point(395, 84)
        Label9.Name = "Label9"
        Label9.Size = New Size(37, 19)
        Label9.TabIndex = 4
        Label9.Text = "0.00"
        ' 
        ' Label7
        ' 
        Label7.AutoSize = True
        Label7.Font = New Font("Calibri", 12F, FontStyle.Italic)
        Label7.ForeColor = Color.White
        Label7.Location = New Point(353, 84)
        Label7.Name = "Label7"
        Label7.Size = New Size(36, 19)
        Label7.TabIndex = 3
        Label7.Text = "ROI:"
        ' 
        ' lbTotalEntradaBRL
        ' 
        lbTotalEntradaBRL.AutoSize = True
        lbTotalEntradaBRL.Font = New Font("Calibri", 12F, FontStyle.Italic)
        lbTotalEntradaBRL.Location = New Point(464, 44)
        lbTotalEntradaBRL.Name = "lbTotalEntradaBRL"
        lbTotalEntradaBRL.Size = New Size(37, 19)
        lbTotalEntradaBRL.TabIndex = 2
        lbTotalEntradaBRL.Text = "0,00"
        ' 
        ' lbTotalEntrada
        ' 
        lbTotalEntrada.AutoSize = True
        lbTotalEntrada.Font = New Font("Calibri", 12F, FontStyle.Italic)
        lbTotalEntrada.Location = New Point(395, 44)
        lbTotalEntrada.Name = "lbTotalEntrada"
        lbTotalEntrada.Size = New Size(37, 19)
        lbTotalEntrada.TabIndex = 1
        lbTotalEntrada.Text = "0.00"
        ' 
        ' Label6
        ' 
        Label6.AutoSize = True
        Label6.Font = New Font("Calibri", 12F, FontStyle.Italic)
        Label6.ForeColor = Color.White
        Label6.Location = New Point(255, 44)
        Label6.Name = "Label6"
        Label6.Size = New Size(134, 19)
        Label6.TabIndex = 0
        Label6.Text = "Valores de entrada:"
        ' 
        ' lbLoadFromMarket
        ' 
        lbLoadFromMarket.Anchor = AnchorStyles.Right
        lbLoadFromMarket.AutoSize = True
        lbLoadFromMarket.BackColor = Color.Transparent
        lbLoadFromMarket.Font = New Font("Segoe UI", 9F, FontStyle.Italic)
        lbLoadFromMarket.ForeColor = Color.OrangeRed
        lbLoadFromMarket.Location = New Point(906, 380)
        lbLoadFromMarket.Name = "lbLoadFromMarket"
        lbLoadFromMarket.Size = New Size(178, 15)
        lbLoadFromMarket.TabIndex = 0
        lbLoadFromMarket.Text = "Carregando dados do mercado..."
        lbLoadFromMarket.Visible = False
        ' 
        ' Panel1
        ' 
        Panel1.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        Panel1.Controls.Add(lbAtualizaEm)
        Panel1.Controls.Add(lbRefresh)
        Panel1.Controls.Add(btRefresh)
        Panel1.Controls.Add(Label4)
        Panel1.Controls.Add(lbDom)
        Panel1.Controls.Add(Label3)
        Panel1.Controls.Add(lbBTC)
        Panel1.Controls.Add(Label1)
        Panel1.Controls.Add(lbDolar)
        Panel1.Location = New Point(490, -1)
        Panel1.Name = "Panel1"
        Panel1.Size = New Size(594, 29)
        Panel1.TabIndex = 22
        ' 
        ' lbAtualizaEm
        ' 
        lbAtualizaEm.AutoSize = True
        lbAtualizaEm.ForeColor = Color.White
        lbAtualizaEm.Location = New Point(17, 6)
        lbAtualizaEm.Name = "lbAtualizaEm"
        lbAtualizaEm.Size = New Size(72, 15)
        lbAtualizaEm.TabIndex = 28
        lbAtualizaEm.Text = "Atualiza em:"
        lbAtualizaEm.Visible = False
        ' 
        ' lbRefresh
        ' 
        lbRefresh.AutoSize = True
        lbRefresh.Font = New Font("Segoe UI", 9F, FontStyle.Bold)
        lbRefresh.ForeColor = Color.Gold
        lbRefresh.Location = New Point(89, 7)
        lbRefresh.Name = "lbRefresh"
        lbRefresh.Size = New Size(38, 15)
        lbRefresh.TabIndex = 0
        lbRefresh.Text = "00:00"
        lbRefresh.Visible = False
        ' 
        ' btRefresh
        ' 
        btRefresh.BackColor = Color.IndianRed
        btRefresh.Cursor = Cursors.Hand
        btRefresh.FlatAppearance.BorderSize = 0
        btRefresh.FlatStyle = FlatStyle.Popup
        btRefresh.Font = New Font("Calibri", 10F)
        btRefresh.ForeColor = Color.Transparent
        btRefresh.Location = New Point(133, 3)
        btRefresh.Name = "btRefresh"
        btRefresh.Size = New Size(75, 23)
        btRefresh.TabIndex = 27
        btRefresh.Text = "Atualizar"
        btRefresh.UseVisualStyleBackColor = False
        ' 
        ' Label4
        ' 
        Label4.AutoSize = True
        Label4.BackColor = Color.Transparent
        Label4.FlatStyle = FlatStyle.Flat
        Label4.Font = New Font("Segoe UI", 10F, FontStyle.Bold)
        Label4.ForeColor = Color.LightGray
        Label4.Location = New Point(466, 4)
        Label4.Name = "Label4"
        Label4.Size = New Size(75, 19)
        Label4.TabIndex = 26
        Label4.Text = "Dom BTC:"
        ' 
        ' lbDom
        ' 
        lbDom.AutoSize = True
        lbDom.BackColor = Color.Transparent
        lbDom.FlatStyle = FlatStyle.Flat
        lbDom.Font = New Font("Segoe UI", 10F, FontStyle.Bold)
        lbDom.ForeColor = Color.DeepSkyBlue
        lbDom.Location = New Point(537, 4)
        lbDom.Name = "lbDom"
        lbDom.Size = New Size(29, 19)
        lbDom.TabIndex = 25
        lbDom.Text = "0%"
        ' 
        ' Label3
        ' 
        Label3.AutoSize = True
        Label3.BackColor = Color.Transparent
        Label3.FlatStyle = FlatStyle.Flat
        Label3.Font = New Font("Segoe UI", 10F, FontStyle.Bold)
        Label3.ForeColor = Color.LightGray
        Label3.Location = New Point(338, 4)
        Label3.Name = "Label3"
        Label3.Size = New Size(39, 19)
        Label3.TabIndex = 24
        Label3.Text = "BTC:"
        ' 
        ' lbBTC
        ' 
        lbBTC.AutoSize = True
        lbBTC.BackColor = Color.Transparent
        lbBTC.FlatStyle = FlatStyle.Flat
        lbBTC.Font = New Font("Segoe UI", 10F, FontStyle.Bold)
        lbBTC.ForeColor = Color.DarkOrange
        lbBTC.Location = New Point(372, 4)
        lbBTC.Name = "lbBTC"
        lbBTC.Size = New Size(45, 19)
        lbBTC.TabIndex = 23
        lbBTC.Text = "$0.00"
        ' 
        ' Label1
        ' 
        Label1.AutoSize = True
        Label1.BackColor = Color.Transparent
        Label1.FlatStyle = FlatStyle.Flat
        Label1.Font = New Font("Segoe UI", 10F, FontStyle.Bold)
        Label1.ForeColor = Color.LightGray
        Label1.Location = New Point(222, 4)
        Label1.Name = "Label1"
        Label1.Size = New Size(51, 19)
        Label1.TabIndex = 22
        Label1.Text = "Dolar:"
        ' 
        ' lbDolar
        ' 
        lbDolar.AutoSize = True
        lbDolar.BackColor = Color.Transparent
        lbDolar.FlatStyle = FlatStyle.Flat
        lbDolar.Font = New Font("Segoe UI", 10F, FontStyle.Bold)
        lbDolar.ForeColor = Color.GreenYellow
        lbDolar.Location = New Point(268, 4)
        lbDolar.Name = "lbDolar"
        lbDolar.Size = New Size(45, 19)
        lbDolar.TabIndex = 21
        lbDolar.Text = "$0.00"
        ' 
        ' NotifyIcon1
        ' 
        NotifyIcon1.BalloonTipIcon = ToolTipIcon.Info
        NotifyIcon1.Icon = CType(resources.GetObject("NotifyIcon1.Icon"), Icon)
        NotifyIcon1.Text = "klkllkl"
        NotifyIcon1.Visible = True
        ' 
        ' TimerRefresh
        ' 
        TimerRefresh.Interval = 1000
        ' 
        ' TimerCountdown
        ' 
        TimerCountdown.Interval = 1000
        ' 
        ' TimerBlink
        ' 
        ' 
        ' FormMain
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        BackColor = Color.FromArgb(CByte(31), CByte(33), CByte(32))
        ClientSize = New Size(1084, 540)
        Controls.Add(Panel1)
        Controls.Add(GroupOverview)
        Controls.Add(PanelProfits)
        Controls.Add(dgPortfolio)
        Controls.Add(lbLoadFromMarket)
        Controls.Add(MenuStrip1)
        Font = New Font("Segoe UI", 9F)
        Icon = CType(resources.GetObject("$this.Icon"), Icon)
        MainMenuStrip = MenuStrip1
        Name = "FormMain"
        StartPosition = FormStartPosition.CenterScreen
        Text = "Portfolio Cripto"
        MenuStrip1.ResumeLayout(False)
        MenuStrip1.PerformLayout()
        CType(dgPortfolio, ComponentModel.ISupportInitialize).EndInit()
        PanelProfits.ResumeLayout(False)
        PanelProfits.PerformLayout()
        GroupOverview.ResumeLayout(False)
        GroupOverview.PerformLayout()
        Panel1.ResumeLayout(False)
        Panel1.PerformLayout()
        ResumeLayout(False)
        PerformLayout()
    End Sub
    Friend WithEvents MenuStrip1 As MenuStrip
    Friend WithEvents CadastroToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents CriptoToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents FecharToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents dgPortfolio As DataGridView
    Friend WithEvents PanelProfits As Panel
    Friend WithEvents Label5 As Label
    Friend WithEvents lbTotalBRL As Label
    Friend WithEvents GroupOverview As GroupBox
    Friend WithEvents Panel1 As Panel
    Friend WithEvents btRefresh As Button
    Friend WithEvents Label4 As Label
    Friend WithEvents lbDom As Label
    Friend WithEvents Label3 As Label
    Friend WithEvents lbBTC As Label
    Friend WithEvents Label1 As Label
    Friend WithEvents lbDolar As Label
    Friend WithEvents NotifyIcon1 As NotifyIcon
    Friend WithEvents TimerRefresh As Timer
    Friend WithEvents OpçõesToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents IntervaloToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents JSONToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents lbRefresh As Label
    Friend WithEvents TimerCountdown As Timer
    Friend WithEvents lbLoadFromMarket As Label
    Friend WithEvents Label6 As Label
    Friend WithEvents lbTotalEntrada As Label
    Friend WithEvents lbTotalEntradaBRL As Label
    Friend WithEvents Label7 As Label
    Friend WithEvents Label8 As Label
    Friend WithEvents Label9 As Label
    Friend WithEvents Label10 As Label
    Friend WithEvents Label11 As Label
    Friend WithEvents Label12 As Label
    Friend WithEvents lbDataTotalToday As Label
    Friend WithEvents Label14 As Label
    Friend WithEvents Label13 As Label
    Friend WithEvents Label15 As Label
    Friend WithEvents Label16 As Label
    Friend WithEvents lbPerformWallet As Label
    Friend WithEvents lbAtualizaEm As Label
    Friend WithEvents TimerBlink As Timer

End Class
