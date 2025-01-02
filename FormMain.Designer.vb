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
        dgPortfolio = New DataGridView()
        PanelProfits = New Panel()
        Label15 = New Label()
        lbTotalBRL = New Label()
        Label5 = New Label()
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
        Label2 = New Label()
        PictureBox2 = New PictureBox()
        PictureBox1 = New PictureBox()
        PanelPerformance = New Panel()
        Label16 = New Label()
        lbPerformWallet = New Label()
        Label6 = New Label()
        lbDataTotalToday = New Label()
        Label7 = New Label()
        Label12 = New Label()
        lbValoresHojeBRL = New Label()
        lbValoresHojeUSD = New Label()
        lbRoiUSD = New Label()
        lbTotalEntradaBRL = New Label()
        lbTotalEntradaUSD = New Label()
        PanelGraphs = New Panel()
        Panel2 = New Panel()
        CadastroToolStripMenuItem = New ToolStripMenuItem()
        CriptoToolStripMenuItem = New ToolStripMenuItem()
        OpçõesToolStripMenuItem = New ToolStripMenuItem()
        IntervaloToolStripMenuItem = New ToolStripMenuItem()
        JSONToolStripMenuItem = New ToolStripMenuItem()
        ImportarToolStripMenuItem = New ToolStripMenuItem()
        PortfolioToolStripMenuItem = New ToolStripMenuItem()
        WalletsExchangeToolStripMenuItem = New ToolStripMenuItem()
        ExportarToolStripMenuItem = New ToolStripMenuItem()
        ImportarToolStripMenuItem1 = New ToolStripMenuItem()
        ExportarToolStripMenuItem1 = New ToolStripMenuItem()
        CriptoToolStripMenuItem2 = New ToolStripMenuItem()
        ImportarToolStripMenuItem2 = New ToolStripMenuItem()
        ExportarToolStripMenuItem2 = New ToolStripMenuItem()
        APIToolStripMenuItem = New ToolStripMenuItem()
        FecharToolStripMenuItem = New ToolStripMenuItem()
        MenuStrip1 = New MenuStrip()
        pbBRL = New PictureBox()
        pbUSD = New PictureBox()
        OpenFileDialog1 = New OpenFileDialog()
        SaveFileDialog1 = New SaveFileDialog()
        Label8 = New Label()
        CType(dgPortfolio, ComponentModel.ISupportInitialize).BeginInit()
        PanelProfits.SuspendLayout()
        Panel1.SuspendLayout()
        CType(PictureBox2, ComponentModel.ISupportInitialize).BeginInit()
        CType(PictureBox1, ComponentModel.ISupportInitialize).BeginInit()
        PanelPerformance.SuspendLayout()
        PanelGraphs.SuspendLayout()
        Panel2.SuspendLayout()
        MenuStrip1.SuspendLayout()
        CType(pbBRL, ComponentModel.ISupportInitialize).BeginInit()
        CType(pbUSD, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
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
        dgPortfolio.CellBorderStyle = DataGridViewCellBorderStyle.None
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
        DataGridViewCellStyle2.Font = New Font("Segoe UI", 9F)
        DataGridViewCellStyle2.ForeColor = SystemColors.ControlText
        DataGridViewCellStyle2.SelectionBackColor = Color.Transparent
        DataGridViewCellStyle2.SelectionForeColor = SystemColors.ControlText
        DataGridViewCellStyle2.WrapMode = DataGridViewTriState.True
        dgPortfolio.DefaultCellStyle = DataGridViewCellStyle2
        dgPortfolio.EnableHeadersVisualStyles = False
        dgPortfolio.Location = New Point(0, 27)
        dgPortfolio.MinimumSize = New Size(0, 350)
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
        dgPortfolio.Size = New Size(1154, 350)
        dgPortfolio.TabIndex = 11
        ' 
        ' PanelProfits
        ' 
        PanelProfits.BackColor = Color.FromArgb(CByte(56), CByte(86), CByte(35))
        PanelProfits.Controls.Add(Label15)
        PanelProfits.Controls.Add(lbTotalBRL)
        PanelProfits.Controls.Add(Label5)
        PanelProfits.Dock = DockStyle.Bottom
        PanelProfits.Location = New Point(0, 577)
        PanelProfits.Name = "PanelProfits"
        PanelProfits.Size = New Size(1154, 40)
        PanelProfits.TabIndex = 15
        ' 
        ' Label15
        ' 
        Label15.Anchor = AnchorStyles.Bottom Or AnchorStyles.Left
        Label15.AutoSize = True
        Label15.BackColor = Color.Transparent
        Label15.Font = New Font("Candara", 14F, FontStyle.Bold)
        Label15.ForeColor = Color.Gold
        Label15.Location = New Point(5, 9)
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
        ' lbLoadFromMarket
        ' 
        lbLoadFromMarket.Anchor = AnchorStyles.Top
        lbLoadFromMarket.AutoSize = True
        lbLoadFromMarket.BackColor = Color.Transparent
        lbLoadFromMarket.Font = New Font("Segoe UI", 9F, FontStyle.Italic)
        lbLoadFromMarket.ForeColor = Color.OrangeRed
        lbLoadFromMarket.Location = New Point(488, 2)
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
        Panel1.Location = New Point(477, -1)
        Panel1.Name = "Panel1"
        Panel1.Size = New Size(673, 29)
        Panel1.TabIndex = 22
        ' 
        ' lbAtualizaEm
        ' 
        lbAtualizaEm.AutoSize = True
        lbAtualizaEm.ForeColor = Color.White
        lbAtualizaEm.Location = New Point(6, 7)
        lbAtualizaEm.Name = "lbAtualizaEm"
        lbAtualizaEm.Size = New Size(72, 15)
        lbAtualizaEm.TabIndex = 28
        lbAtualizaEm.Text = "Atualiza em:"
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
        ' 
        ' btRefresh
        ' 
        btRefresh.BackColor = Color.IndianRed
        btRefresh.Cursor = Cursors.Hand
        btRefresh.FlatAppearance.BorderSize = 0
        btRefresh.FlatStyle = FlatStyle.Popup
        btRefresh.Font = New Font("Calibri", 10F)
        btRefresh.ForeColor = Color.Transparent
        btRefresh.Location = New Point(215, 3)
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
        Label4.Location = New Point(539, 4)
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
        lbDom.Location = New Point(610, 4)
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
        Label3.Location = New Point(411, 4)
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
        lbBTC.Location = New Point(445, 4)
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
        Label1.Location = New Point(306, 4)
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
        lbDolar.Location = New Point(352, 4)
        lbDolar.Name = "lbDolar"
        lbDolar.Size = New Size(45, 19)
        lbDolar.TabIndex = 21
        lbDolar.Text = "$0.00"
        ' 
        ' NotifyIcon1
        ' 
        NotifyIcon1.BalloonTipIcon = ToolTipIcon.Info
        NotifyIcon1.Icon = CType(resources.GetObject("NotifyIcon1.Icon"), Icon)
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
        ' Label2
        ' 
        Label2.AutoSize = True
        Label2.Font = New Font("Calibri", 14F, FontStyle.Bold Or FontStyle.Italic)
        Label2.ForeColor = Color.Aqua
        Label2.Location = New Point(7, 1)
        Label2.Name = "Label2"
        Label2.Size = New Size(100, 23)
        Label2.TabIndex = 0
        Label2.Text = "Visão Geral"
        ' 
        ' PictureBox2
        ' 
        PictureBox2.Image = CType(resources.GetObject("PictureBox2.Image"), Image)
        PictureBox2.Location = New Point(325, 78)
        PictureBox2.Name = "PictureBox2"
        PictureBox2.Size = New Size(35, 25)
        PictureBox2.SizeMode = PictureBoxSizeMode.StretchImage
        PictureBox2.TabIndex = 31
        PictureBox2.TabStop = False
        ' 
        ' PictureBox1
        ' 
        PictureBox1.Image = CType(resources.GetObject("PictureBox1.Image"), Image)
        PictureBox1.Location = New Point(219, 78)
        PictureBox1.Name = "PictureBox1"
        PictureBox1.Size = New Size(35, 25)
        PictureBox1.SizeMode = PictureBoxSizeMode.StretchImage
        PictureBox1.TabIndex = 30
        PictureBox1.TabStop = False
        ' 
        ' PanelPerformance
        ' 
        PanelPerformance.Controls.Add(Label16)
        PanelPerformance.Controls.Add(lbPerformWallet)
        PanelPerformance.Controls.Add(Label6)
        PanelPerformance.Controls.Add(lbDataTotalToday)
        PanelPerformance.Controls.Add(Label7)
        PanelPerformance.Controls.Add(Label12)
        PanelPerformance.Location = New Point(18, 29)
        PanelPerformance.Name = "PanelPerformance"
        PanelPerformance.Size = New Size(174, 153)
        PanelPerformance.TabIndex = 29
        ' 
        ' Label16
        ' 
        Label16.AutoSize = True
        Label16.Font = New Font("Calibri", 13F, FontStyle.Italic)
        Label16.ForeColor = Color.White
        Label16.Location = New Point(34, 11)
        Label16.Name = "Label16"
        Label16.Size = New Size(105, 22)
        Label16.TabIndex = 12
        Label16.Text = "Desempenho"
        ' 
        ' lbPerformWallet
        ' 
        lbPerformWallet.AutoSize = True
        lbPerformWallet.Font = New Font("Calibri", 20F, FontStyle.Italic)
        lbPerformWallet.ForeColor = Color.GreenYellow
        lbPerformWallet.Location = New Point(34, 33)
        lbPerformWallet.Name = "lbPerformWallet"
        lbPerformWallet.Size = New Size(48, 33)
        lbPerformWallet.TabIndex = 13
        lbPerformWallet.Text = "0%"
        ' 
        ' Label6
        ' 
        Label6.AutoSize = True
        Label6.Font = New Font("Calibri", 12F, FontStyle.Italic)
        Label6.ForeColor = Color.White
        Label6.Location = New Point(69, 83)
        Label6.Name = "Label6"
        Label6.Size = New Size(97, 19)
        Label6.TabIndex = 0
        Label6.Text = "Investimento:"
        ' 
        ' lbDataTotalToday
        ' 
        lbDataTotalToday.AutoSize = True
        lbDataTotalToday.Font = New Font("Calibri", 12F, FontStyle.Italic)
        lbDataTotalToday.ForeColor = Color.DarkOrange
        lbDataTotalToday.Location = New Point(81, 102)
        lbDataTotalToday.Name = "lbDataTotalToday"
        lbDataTotalToday.Size = New Size(85, 19)
        lbDataTotalToday.TabIndex = 9
        lbDataTotalToday.Text = "00/00/0000"
        ' 
        ' Label7
        ' 
        Label7.AutoSize = True
        Label7.Font = New Font("Calibri", 12F, FontStyle.Italic)
        Label7.ForeColor = Color.White
        Label7.Location = New Point(134, 125)
        Label7.Name = "Label7"
        Label7.Size = New Size(36, 19)
        Label7.TabIndex = 3
        Label7.Text = "ROI:"
        ' 
        ' Label12
        ' 
        Label12.AutoSize = True
        Label12.Font = New Font("Calibri", 12F, FontStyle.Italic)
        Label12.ForeColor = Color.White
        Label12.Location = New Point(4, 102)
        Label12.Name = "Label12"
        Label12.Size = New Size(80, 19)
        Label12.TabIndex = 6
        Label12.Text = "Patrimônio"
        ' 
        ' lbValoresHojeBRL
        ' 
        lbValoresHojeBRL.AutoSize = True
        lbValoresHojeBRL.Font = New Font("Calibri", 12F, FontStyle.Bold Or FontStyle.Italic)
        lbValoresHojeBRL.ForeColor = Color.DeepSkyBlue
        lbValoresHojeBRL.Location = New Point(322, 131)
        lbValoresHojeBRL.Name = "lbValoresHojeBRL"
        lbValoresHojeBRL.Size = New Size(37, 19)
        lbValoresHojeBRL.TabIndex = 28
        lbValoresHojeBRL.Text = "0,00"
        ' 
        ' lbValoresHojeUSD
        ' 
        lbValoresHojeUSD.AutoSize = True
        lbValoresHojeUSD.Font = New Font("Calibri", 12F, FontStyle.Bold Or FontStyle.Italic)
        lbValoresHojeUSD.ForeColor = Color.LimeGreen
        lbValoresHojeUSD.Location = New Point(216, 131)
        lbValoresHojeUSD.Name = "lbValoresHojeUSD"
        lbValoresHojeUSD.Size = New Size(37, 19)
        lbValoresHojeUSD.TabIndex = 27
        lbValoresHojeUSD.Text = "0.00"
        ' 
        ' lbRoiUSD
        ' 
        lbRoiUSD.AutoSize = True
        lbRoiUSD.Font = New Font("Calibri", 12F, FontStyle.Bold Or FontStyle.Italic)
        lbRoiUSD.ForeColor = Color.Lime
        lbRoiUSD.Location = New Point(216, 154)
        lbRoiUSD.Name = "lbRoiUSD"
        lbRoiUSD.Size = New Size(37, 19)
        lbRoiUSD.TabIndex = 26
        lbRoiUSD.Text = "0.00"
        ' 
        ' lbTotalEntradaBRL
        ' 
        lbTotalEntradaBRL.AutoSize = True
        lbTotalEntradaBRL.Font = New Font("Calibri", 12F, FontStyle.Bold Or FontStyle.Italic)
        lbTotalEntradaBRL.ForeColor = Color.DeepSkyBlue
        lbTotalEntradaBRL.Location = New Point(322, 112)
        lbTotalEntradaBRL.Name = "lbTotalEntradaBRL"
        lbTotalEntradaBRL.Size = New Size(37, 19)
        lbTotalEntradaBRL.TabIndex = 25
        lbTotalEntradaBRL.Text = "0,00"
        ' 
        ' lbTotalEntradaUSD
        ' 
        lbTotalEntradaUSD.AutoSize = True
        lbTotalEntradaUSD.Font = New Font("Calibri", 12F, FontStyle.Bold Or FontStyle.Italic)
        lbTotalEntradaUSD.ForeColor = Color.LimeGreen
        lbTotalEntradaUSD.Location = New Point(216, 112)
        lbTotalEntradaUSD.Name = "lbTotalEntradaUSD"
        lbTotalEntradaUSD.Size = New Size(37, 19)
        lbTotalEntradaUSD.TabIndex = 24
        lbTotalEntradaUSD.Text = "0.00"
        ' 
        ' PanelGraphs
        ' 
        PanelGraphs.BackColor = Color.FromArgb(CByte(30), CByte(30), CByte(30))
        PanelGraphs.Controls.Add(PictureBox2)
        PanelGraphs.Controls.Add(Label2)
        PanelGraphs.Controls.Add(PictureBox1)
        PanelGraphs.Controls.Add(lbTotalEntradaUSD)
        PanelGraphs.Controls.Add(PanelPerformance)
        PanelGraphs.Controls.Add(lbTotalEntradaBRL)
        PanelGraphs.Controls.Add(lbValoresHojeBRL)
        PanelGraphs.Controls.Add(lbRoiUSD)
        PanelGraphs.Controls.Add(lbValoresHojeUSD)
        PanelGraphs.Dock = DockStyle.Bottom
        PanelGraphs.Location = New Point(0, 395)
        PanelGraphs.Name = "PanelGraphs"
        PanelGraphs.Size = New Size(1154, 182)
        PanelGraphs.TabIndex = 32
        ' 
        ' Panel2
        ' 
        Panel2.Controls.Add(lbLoadFromMarket)
        Panel2.Dock = DockStyle.Bottom
        Panel2.Location = New Point(0, 375)
        Panel2.Name = "Panel2"
        Panel2.Size = New Size(1154, 20)
        Panel2.TabIndex = 33
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
        CriptoToolStripMenuItem.Size = New Size(115, 22)
        CriptoToolStripMenuItem.Text = "Aportes"
        ' 
        ' OpçõesToolStripMenuItem
        ' 
        OpçõesToolStripMenuItem.DropDownItems.AddRange(New ToolStripItem() {IntervaloToolStripMenuItem, JSONToolStripMenuItem, APIToolStripMenuItem})
        OpçõesToolStripMenuItem.ForeColor = SystemColors.ButtonHighlight
        OpçõesToolStripMenuItem.Name = "OpçõesToolStripMenuItem"
        OpçõesToolStripMenuItem.Size = New Size(96, 20)
        OpçõesToolStripMenuItem.Text = "Configurações"
        ' 
        ' IntervaloToolStripMenuItem
        ' 
        IntervaloToolStripMenuItem.BackColor = Color.FromArgb(CByte(31), CByte(33), CByte(32))
        IntervaloToolStripMenuItem.ForeColor = SystemColors.ButtonHighlight
        IntervaloToolStripMenuItem.Name = "IntervaloToolStripMenuItem"
        IntervaloToolStripMenuItem.Size = New Size(120, 22)
        IntervaloToolStripMenuItem.Text = "Intervalo"
        ' 
        ' JSONToolStripMenuItem
        ' 
        JSONToolStripMenuItem.BackColor = Color.FromArgb(CByte(31), CByte(33), CByte(32))
        JSONToolStripMenuItem.DropDownItems.AddRange(New ToolStripItem() {ImportarToolStripMenuItem, ExportarToolStripMenuItem, CriptoToolStripMenuItem2})
        JSONToolStripMenuItem.ForeColor = SystemColors.ButtonHighlight
        JSONToolStripMenuItem.Name = "JSONToolStripMenuItem"
        JSONToolStripMenuItem.Size = New Size(120, 22)
        JSONToolStripMenuItem.Text = "JSON"
        ' 
        ' ImportarToolStripMenuItem
        ' 
        ImportarToolStripMenuItem.BackColor = Color.FromArgb(CByte(31), CByte(33), CByte(32))
        ImportarToolStripMenuItem.DropDownItems.AddRange(New ToolStripItem() {PortfolioToolStripMenuItem, WalletsExchangeToolStripMenuItem})
        ImportarToolStripMenuItem.ForeColor = SystemColors.ButtonHighlight
        ImportarToolStripMenuItem.Name = "ImportarToolStripMenuItem"
        ImportarToolStripMenuItem.Size = New Size(168, 22)
        ImportarToolStripMenuItem.Text = "Portfolio"
        ' 
        ' PortfolioToolStripMenuItem
        ' 
        PortfolioToolStripMenuItem.BackColor = Color.FromArgb(CByte(31), CByte(33), CByte(32))
        PortfolioToolStripMenuItem.ForeColor = SystemColors.ControlLightLight
        PortfolioToolStripMenuItem.Name = "PortfolioToolStripMenuItem"
        PortfolioToolStripMenuItem.Size = New Size(120, 22)
        PortfolioToolStripMenuItem.Text = "Importar"
        ' 
        ' WalletsExchangeToolStripMenuItem
        ' 
        WalletsExchangeToolStripMenuItem.BackColor = Color.FromArgb(CByte(31), CByte(33), CByte(32))
        WalletsExchangeToolStripMenuItem.ForeColor = SystemColors.ControlLightLight
        WalletsExchangeToolStripMenuItem.Name = "WalletsExchangeToolStripMenuItem"
        WalletsExchangeToolStripMenuItem.Size = New Size(120, 22)
        WalletsExchangeToolStripMenuItem.Text = "Exportar"
        ' 
        ' ExportarToolStripMenuItem
        ' 
        ExportarToolStripMenuItem.BackColor = Color.FromArgb(CByte(31), CByte(33), CByte(32))
        ExportarToolStripMenuItem.DropDownItems.AddRange(New ToolStripItem() {ImportarToolStripMenuItem1, ExportarToolStripMenuItem1})
        ExportarToolStripMenuItem.ForeColor = SystemColors.ButtonHighlight
        ExportarToolStripMenuItem.Name = "ExportarToolStripMenuItem"
        ExportarToolStripMenuItem.Size = New Size(168, 22)
        ExportarToolStripMenuItem.Text = "Wallets/Exchange"
        ' 
        ' ImportarToolStripMenuItem1
        ' 
        ImportarToolStripMenuItem1.BackColor = Color.FromArgb(CByte(31), CByte(33), CByte(32))
        ImportarToolStripMenuItem1.ForeColor = SystemColors.ControlLightLight
        ImportarToolStripMenuItem1.Name = "ImportarToolStripMenuItem1"
        ImportarToolStripMenuItem1.Size = New Size(120, 22)
        ImportarToolStripMenuItem1.Text = "Importar"
        ' 
        ' ExportarToolStripMenuItem1
        ' 
        ExportarToolStripMenuItem1.BackColor = Color.FromArgb(CByte(31), CByte(33), CByte(32))
        ExportarToolStripMenuItem1.ForeColor = SystemColors.ControlLightLight
        ExportarToolStripMenuItem1.Name = "ExportarToolStripMenuItem1"
        ExportarToolStripMenuItem1.Size = New Size(120, 22)
        ExportarToolStripMenuItem1.Text = "Exportar"
        ' 
        ' CriptoToolStripMenuItem2
        ' 
        CriptoToolStripMenuItem2.BackColor = Color.FromArgb(CByte(31), CByte(33), CByte(32))
        CriptoToolStripMenuItem2.DropDownItems.AddRange(New ToolStripItem() {ImportarToolStripMenuItem2, ExportarToolStripMenuItem2})
        CriptoToolStripMenuItem2.ForeColor = SystemColors.ControlLightLight
        CriptoToolStripMenuItem2.Name = "CriptoToolStripMenuItem2"
        CriptoToolStripMenuItem2.Size = New Size(168, 22)
        CriptoToolStripMenuItem2.Text = "Cripto"
        ' 
        ' ImportarToolStripMenuItem2
        ' 
        ImportarToolStripMenuItem2.BackColor = Color.FromArgb(CByte(31), CByte(33), CByte(32))
        ImportarToolStripMenuItem2.ForeColor = SystemColors.ControlLightLight
        ImportarToolStripMenuItem2.Name = "ImportarToolStripMenuItem2"
        ImportarToolStripMenuItem2.Size = New Size(120, 22)
        ImportarToolStripMenuItem2.Text = "Importar"
        ' 
        ' ExportarToolStripMenuItem2
        ' 
        ExportarToolStripMenuItem2.BackColor = Color.FromArgb(CByte(31), CByte(33), CByte(32))
        ExportarToolStripMenuItem2.ForeColor = SystemColors.ControlLightLight
        ExportarToolStripMenuItem2.Name = "ExportarToolStripMenuItem2"
        ExportarToolStripMenuItem2.Size = New Size(120, 22)
        ExportarToolStripMenuItem2.Text = "Exportar"
        ' 
        ' APIToolStripMenuItem
        ' 
        APIToolStripMenuItem.BackColor = Color.FromArgb(CByte(31), CByte(33), CByte(32))
        APIToolStripMenuItem.ForeColor = SystemColors.ButtonHighlight
        APIToolStripMenuItem.Name = "APIToolStripMenuItem"
        APIToolStripMenuItem.Size = New Size(120, 22)
        APIToolStripMenuItem.Text = "API"
        ' 
        ' FecharToolStripMenuItem
        ' 
        FecharToolStripMenuItem.ForeColor = SystemColors.ButtonHighlight
        FecharToolStripMenuItem.Name = "FecharToolStripMenuItem"
        FecharToolStripMenuItem.Size = New Size(54, 20)
        FecharToolStripMenuItem.Text = "Fechar"
        ' 
        ' MenuStrip1
        ' 
        MenuStrip1.BackColor = Color.FromArgb(CByte(31), CByte(33), CByte(32))
        MenuStrip1.Items.AddRange(New ToolStripItem() {CadastroToolStripMenuItem, OpçõesToolStripMenuItem, FecharToolStripMenuItem})
        MenuStrip1.Location = New Point(0, 0)
        MenuStrip1.Name = "MenuStrip1"
        MenuStrip1.Size = New Size(1154, 24)
        MenuStrip1.TabIndex = 10
        MenuStrip1.Text = "MenuStrip1"
        ' 
        ' pbBRL
        ' 
        pbBRL.Cursor = Cursors.Hand
        pbBRL.Image = CType(resources.GetObject("pbBRL.Image"), Image)
        pbBRL.Location = New Point(446, 7)
        pbBRL.Name = "pbBRL"
        pbBRL.Size = New Size(25, 15)
        pbBRL.SizeMode = PictureBoxSizeMode.StretchImage
        pbBRL.TabIndex = 34
        pbBRL.TabStop = False
        ' 
        ' pbUSD
        ' 
        pbUSD.Cursor = Cursors.Hand
        pbUSD.Image = CType(resources.GetObject("pbUSD.Image"), Image)
        pbUSD.Location = New Point(417, 7)
        pbUSD.Name = "pbUSD"
        pbUSD.Size = New Size(25, 15)
        pbUSD.SizeMode = PictureBoxSizeMode.StretchImage
        pbUSD.TabIndex = 35
        pbUSD.TabStop = False
        ' 
        ' SaveFileDialog1
        ' 
        SaveFileDialog1.DefaultExt = "json"
        SaveFileDialog1.Filter = "json files (*.json)|*.json"
        ' 
        ' Label8
        ' 
        Label8.AutoSize = True
        Label8.ForeColor = SystemColors.ControlLightLight
        Label8.Location = New Point(370, 7)
        Label8.Name = "Label8"
        Label8.Size = New Size(47, 15)
        Label8.TabIndex = 36
        Label8.Text = "Moeda:"
        ' 
        ' FormMain
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        BackColor = Color.FromArgb(CByte(40), CByte(40), CByte(40))
        ClientSize = New Size(1154, 617)
        Controls.Add(Label8)
        Controls.Add(pbUSD)
        Controls.Add(pbBRL)
        Controls.Add(Panel2)
        Controls.Add(PanelGraphs)
        Controls.Add(Panel1)
        Controls.Add(PanelProfits)
        Controls.Add(dgPortfolio)
        Controls.Add(MenuStrip1)
        Font = New Font("Segoe UI", 9F)
        Icon = CType(resources.GetObject("$this.Icon"), Icon)
        MainMenuStrip = MenuStrip1
        Name = "FormMain"
        StartPosition = FormStartPosition.CenterScreen
        Text = "Portfolio Cripto"
        CType(dgPortfolio, ComponentModel.ISupportInitialize).EndInit()
        PanelProfits.ResumeLayout(False)
        PanelProfits.PerformLayout()
        Panel1.ResumeLayout(False)
        Panel1.PerformLayout()
        CType(PictureBox2, ComponentModel.ISupportInitialize).EndInit()
        CType(PictureBox1, ComponentModel.ISupportInitialize).EndInit()
        PanelPerformance.ResumeLayout(False)
        PanelPerformance.PerformLayout()
        PanelGraphs.ResumeLayout(False)
        PanelGraphs.PerformLayout()
        Panel2.ResumeLayout(False)
        Panel2.PerformLayout()
        MenuStrip1.ResumeLayout(False)
        MenuStrip1.PerformLayout()
        CType(pbBRL, ComponentModel.ISupportInitialize).EndInit()
        CType(pbUSD, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)
        PerformLayout()
    End Sub
    Friend WithEvents dgPortfolio As DataGridView
    Friend WithEvents PanelProfits As Panel
    Friend WithEvents Label5 As Label
    Friend WithEvents lbTotalBRL As Label
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
    Friend WithEvents lbRefresh As Label
    Friend WithEvents TimerCountdown As Timer
    Friend WithEvents lbLoadFromMarket As Label
    Friend WithEvents Label15 As Label
    Friend WithEvents lbAtualizaEm As Label
    Friend WithEvents TimerBlink As Timer
    Friend WithEvents Label2 As Label
    Friend WithEvents PictureBox2 As PictureBox
    Friend WithEvents PictureBox1 As PictureBox
    Friend WithEvents PanelPerformance As Panel
    Friend WithEvents Label16 As Label
    Friend WithEvents lbPerformWallet As Label
    Friend WithEvents Label6 As Label
    Friend WithEvents lbDataTotalToday As Label
    Friend WithEvents Label7 As Label
    Friend WithEvents Label12 As Label
    Friend WithEvents lbValoresHojeBRL As Label
    Friend WithEvents lbValoresHojeUSD As Label
    Friend WithEvents lbRoiUSD As Label
    Friend WithEvents lbTotalEntradaBRL As Label
    Friend WithEvents lbTotalEntradaUSD As Label
    Friend WithEvents PanelGraphs As Panel
    Friend WithEvents Panel2 As Panel
    Friend WithEvents CadastroToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents CriptoToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents OpçõesToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents IntervaloToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents JSONToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents FecharToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents MenuStrip1 As MenuStrip
    Friend WithEvents pbBRL As PictureBox
    Friend WithEvents pbUSD As PictureBox
    Friend WithEvents ImportarToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents OpenFileDialog1 As OpenFileDialog
    Friend WithEvents SaveFileDialog1 As SaveFileDialog
    Friend WithEvents Label8 As Label
    Friend WithEvents PortfolioToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents WalletsExchangeToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents CriptoToolStripMenuItem2 As ToolStripMenuItem
    Friend WithEvents ImportarToolStripMenuItem2 As ToolStripMenuItem
    Friend WithEvents ExportarToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ImportarToolStripMenuItem1 As ToolStripMenuItem
    Friend WithEvents ExportarToolStripMenuItem1 As ToolStripMenuItem
    Friend WithEvents ExportarToolStripMenuItem2 As ToolStripMenuItem
    Friend WithEvents APIToolStripMenuItem As ToolStripMenuItem

End Class
