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
        Dim DataGridViewCellStyle4 As DataGridViewCellStyle = New DataGridViewCellStyle()
        Dim DataGridViewCellStyle5 As DataGridViewCellStyle = New DataGridViewCellStyle()
        Dim DataGridViewCellStyle6 As DataGridViewCellStyle = New DataGridViewCellStyle()
        Dim DataGridViewCellStyle7 As DataGridViewCellStyle = New DataGridViewCellStyle()
        Dim DataGridViewCellStyle8 As DataGridViewCellStyle = New DataGridViewCellStyle()
        Dim DataGridViewCellStyle9 As DataGridViewCellStyle = New DataGridViewCellStyle()
        Dim DataGridViewCellStyle10 As DataGridViewCellStyle = New DataGridViewCellStyle()
        Dim DataGridViewCellStyle11 As DataGridViewCellStyle = New DataGridViewCellStyle()
        dgPortfolio = New DataGridView()
        PanelProfits = New Panel()
        Label15 = New Label()
        Label5 = New Label()
        lbRoiSeparator = New Label()
        lbValoresSeparator = New Label()
        lbValoresHojeBRL = New Label()
        lbValoresHojeUSD = New Label()
        Label7 = New Label()
        lbRoiUSD = New Label()
        lbTotalBRL = New Label()
        lbLoadFromMarket = New Label()
        Panel1 = New Panel()
        lbAtualizaEm = New Label()
        lbRefresh = New Label()
        Label4 = New Label()
        lbDom = New Label()
        Label3 = New Label()
        lbBTC = New Label()
        Label1 = New Label()
        lbDolar = New Label()
        btRefresh = New Button()
        NotifyIcon1 = New NotifyIcon(components)
        TimerRefresh = New Timer(components)
        TimerCountdown = New Timer(components)
        TimerBlink = New Timer(components)
        PictureBox2 = New PictureBox()
        PictureBox1 = New PictureBox()
        Label10 = New Label()
        lbPercentInvestido = New Label()
        lbPercentCaixa = New Label()
        Label9 = New Label()
        Label16 = New Label()
        lbPerformWallet = New Label()
        Label6 = New Label()
        LabelFuturos = New Label()
        lbDataTotalToday = New Label()
        lbInvestimentoBRL = New Label()
        lbInvestimentoUSD = New Label()
        lbTotalEntradaBRL = New Label()
        lbTotalEntradaUSD = New Label()
        lbFuturosBRL = New Label()
        lbFuturosUSD = New Label()
        PanelGraphs = New Panel()
        Label11 = New Label()
        Label2 = New Label()
        lbCaixaBRL = New Label()
        lbCaixa = New Label()
        PanelOrdens = New Panel()
        dgFuturos = New DataGridView()
        Panel2 = New Panel()
        CadastroToolStripMenuItem = New ToolStripMenuItem()
        CriptoToolStripMenuItem = New ToolStripMenuItem()
        SimbolosToolStripMenuItem = New ToolStripMenuItem()
        CustodiaToolStripMenuItem = New ToolStripMenuItem()
        OpçõesToolStripMenuItem = New ToolStripMenuItem()
        APIToolStripMenuItem = New ToolStripMenuItem()
        IntervaloToolStripMenuItem = New ToolStripMenuItem()
        FecharToolStripMenuItem = New ToolStripMenuItem()
        MenuStrip1 = New MenuStrip()
        pbBRL = New PictureBox()
        pbUSD = New PictureBox()
        OpenFileDialog1 = New OpenFileDialog()
        SaveFileDialog1 = New SaveFileDialog()
        Label8 = New Label()
        ToolTip1 = New ToolTip(components)
        panelDebug = New Panel()
        lbDebug = New RichTextBox()
        btSpot = New Button()
        btFuturos = New Button()
        btVisaoGeral = New Button()
        btOrdensSpot = New Button()
        PanelFuturos = New Panel()
        btOrdensFuturo = New Button()
        btTradebot = New Button()
        PanelTradeBot = New Panel()
        btSpotBotTrades = New Button()
        btSpotBotExport = New Button()
        lbSpotBotSimulationSummary = New Label()
        rtbSpotBotLog = New RichTextBox()
        GroupBox4 = New GroupBox()
        chkSpotBotRiskUseUnrealized = New CheckBox()
        chkSpotBotRiskClosePositions = New CheckBox()
        nudSpotBotRiskPause = New NumericUpDown()
        Label44 = New Label()
        nudSpotBotRiskMaxLosses = New NumericUpDown()
        Label43 = New Label()
        nudSpotBotRiskGiveback = New NumericUpDown()
        Label42 = New Label()
        nudSpotBotRiskProfitTrigger = New NumericUpDown()
        Label41 = New Label()
        chkSpotBotRiskProfitLock = New CheckBox()
        nudSpotBotRiskSessionLoss = New NumericUpDown()
        Label40 = New Label()
        chkSpotBotRiskBrakeEnabled = New CheckBox()
        dgSpotBotPositionsControl = New DataGridView()
        GroupBox1 = New GroupBox()
        chkSpotBotGridRejection = New CheckBox()
        nudSpotBotGridLevels = New NumericUpDown()
        btSpotBotStart = New Button()
        btSpotBotStop = New Button()
        Label39 = New Label()
        nudSpotBotGridExposure = New NumericUpDown()
        btSpotBotUniverse = New Button()
        Label38 = New Label()
        nudSpotBotGridReserve = New NumericUpDown()
        Label37 = New Label()
        nudSpotBotGridRisk = New NumericUpDown()
        Label36 = New Label()
        nudSpotBotMinTrades = New NumericUpDown()
        Label20 = New Label()
        nudSpotBotPostExitCooldown = New NumericUpDown()
        Label19 = New Label()
        nudSpotBotMinVolume = New NumericUpDown()
        Label22 = New Label()
        nudSpotBotMaxAssets = New NumericUpDown()
        Label23 = New Label()
        nudSpotBotStopLoss = New NumericUpDown()
        Label35 = New Label()
        nudSpotBotTakeProfit = New NumericUpDown()
        Label34 = New Label()
        nudSpotBotFee = New NumericUpDown()
        Label33 = New Label()
        nudSpotBotAllocation = New NumericUpDown()
        Label18 = New Label()
        nudSpotBotStartingQuote = New NumericUpDown()
        Label17 = New Label()
        GroupBox3 = New GroupBox()
        Label31 = New Label()
        Label32 = New Label()
        nudSpotBotBtcFlushVolume = New NumericUpDown()
        Label30 = New Label()
        Label29 = New Label()
        nudSpotBotBtcDailyDrawdown = New NumericUpDown()
        Label28 = New Label()
        nudSpotBotBtcDailyDrop = New NumericUpDown()
        chkSpotBotBtcDaily = New CheckBox()
        Label27 = New Label()
        nudSpotBotBtcFlushPause = New NumericUpDown()
        Label26 = New Label()
        nudSpotBotBtcFlushDrop = New NumericUpDown()
        chkSpotBotBtcFlush = New CheckBox()
        Label25 = New Label()
        Label12 = New Label()
        NumericUpDown2 = New NumericUpDown()
        NumericUpDown1 = New NumericUpDown()
        chkSpotBotBreakEven = New CheckBox()
        chkSpotBotTrailing = New CheckBox()
        GroupBox2 = New GroupBox()
        chkSpotBotValueZonePullback = New CheckBox()
        chkSpotBotBtcMildDecline = New CheckBox()
        chkSpotBotGridAdaptive = New CheckBox()
        chkSpotBotMomentumVolume = New CheckBox()
        chkSpotBotBreakoutRetest = New CheckBox()
        chkSpotBotWavePullback = New CheckBox()
        cbSpotBotBollingerMode = New ComboBox()
        Label24 = New Label()
        Label21 = New Label()
        Label14 = New Label()
        nudSpotBotConfirmations = New NumericUpDown()
        Label13 = New Label()
        cbSpotBotCombination = New ComboBox()
        cbSpotBotBollingerTimeframe = New ComboBox()
        cbSpotBotUniverseMode = New ComboBox()
        CType(dgPortfolio, ComponentModel.ISupportInitialize).BeginInit()
        PanelProfits.SuspendLayout()
        Panel1.SuspendLayout()
        CType(PictureBox2, ComponentModel.ISupportInitialize).BeginInit()
        CType(PictureBox1, ComponentModel.ISupportInitialize).BeginInit()
        PanelGraphs.SuspendLayout()
        CType(dgFuturos, ComponentModel.ISupportInitialize).BeginInit()
        MenuStrip1.SuspendLayout()
        CType(pbBRL, ComponentModel.ISupportInitialize).BeginInit()
        CType(pbUSD, ComponentModel.ISupportInitialize).BeginInit()
        panelDebug.SuspendLayout()
        PanelTradeBot.SuspendLayout()
        GroupBox4.SuspendLayout()
        CType(nudSpotBotRiskPause, ComponentModel.ISupportInitialize).BeginInit()
        CType(nudSpotBotRiskMaxLosses, ComponentModel.ISupportInitialize).BeginInit()
        CType(nudSpotBotRiskGiveback, ComponentModel.ISupportInitialize).BeginInit()
        CType(nudSpotBotRiskProfitTrigger, ComponentModel.ISupportInitialize).BeginInit()
        CType(nudSpotBotRiskSessionLoss, ComponentModel.ISupportInitialize).BeginInit()
        CType(dgSpotBotPositionsControl, ComponentModel.ISupportInitialize).BeginInit()
        GroupBox1.SuspendLayout()
        CType(nudSpotBotGridLevels, ComponentModel.ISupportInitialize).BeginInit()
        CType(nudSpotBotGridExposure, ComponentModel.ISupportInitialize).BeginInit()
        CType(nudSpotBotGridReserve, ComponentModel.ISupportInitialize).BeginInit()
        CType(nudSpotBotGridRisk, ComponentModel.ISupportInitialize).BeginInit()
        CType(nudSpotBotMinTrades, ComponentModel.ISupportInitialize).BeginInit()
        CType(nudSpotBotPostExitCooldown, ComponentModel.ISupportInitialize).BeginInit()
        CType(nudSpotBotMinVolume, ComponentModel.ISupportInitialize).BeginInit()
        CType(nudSpotBotMaxAssets, ComponentModel.ISupportInitialize).BeginInit()
        CType(nudSpotBotStopLoss, ComponentModel.ISupportInitialize).BeginInit()
        CType(nudSpotBotTakeProfit, ComponentModel.ISupportInitialize).BeginInit()
        CType(nudSpotBotFee, ComponentModel.ISupportInitialize).BeginInit()
        CType(nudSpotBotAllocation, ComponentModel.ISupportInitialize).BeginInit()
        CType(nudSpotBotStartingQuote, ComponentModel.ISupportInitialize).BeginInit()
        GroupBox3.SuspendLayout()
        CType(nudSpotBotBtcFlushVolume, ComponentModel.ISupportInitialize).BeginInit()
        CType(nudSpotBotBtcDailyDrawdown, ComponentModel.ISupportInitialize).BeginInit()
        CType(nudSpotBotBtcDailyDrop, ComponentModel.ISupportInitialize).BeginInit()
        CType(nudSpotBotBtcFlushPause, ComponentModel.ISupportInitialize).BeginInit()
        CType(nudSpotBotBtcFlushDrop, ComponentModel.ISupportInitialize).BeginInit()
        CType(NumericUpDown2, ComponentModel.ISupportInitialize).BeginInit()
        CType(NumericUpDown1, ComponentModel.ISupportInitialize).BeginInit()
        GroupBox2.SuspendLayout()
        CType(nudSpotBotConfirmations, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        ' 
        ' dgPortfolio
        ' 
        dgPortfolio.AllowUserToAddRows = False
        dgPortfolio.AllowUserToDeleteRows = False
        dgPortfolio.AllowUserToOrderColumns = True
        dgPortfolio.AllowUserToResizeRows = False
        dgPortfolio.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        dgPortfolio.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        dgPortfolio.BackgroundColor = Color.FromArgb(CByte(64), CByte(64), CByte(64))
        dgPortfolio.BorderStyle = BorderStyle.None
        dgPortfolio.CellBorderStyle = DataGridViewCellBorderStyle.None
        dgPortfolio.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None
        DataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle1.BackColor = Color.FromArgb(CByte(64), CByte(64), CByte(64))
        DataGridViewCellStyle1.Font = New Font("Segoe UI", 9F)
        DataGridViewCellStyle1.ForeColor = Color.Turquoise
        DataGridViewCellStyle1.SelectionBackColor = Color.Transparent
        DataGridViewCellStyle1.SelectionForeColor = Color.Transparent
        DataGridViewCellStyle1.WrapMode = DataGridViewTriState.True
        dgPortfolio.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle1
        DataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle2.BackColor = Color.FromArgb(CByte(64), CByte(64), CByte(64))
        DataGridViewCellStyle2.Font = New Font("Segoe UI", 9F)
        DataGridViewCellStyle2.ForeColor = SystemColors.ControlText
        DataGridViewCellStyle2.SelectionBackColor = Color.Transparent
        DataGridViewCellStyle2.SelectionForeColor = Color.Transparent
        DataGridViewCellStyle2.WrapMode = DataGridViewTriState.True
        dgPortfolio.DefaultCellStyle = DataGridViewCellStyle2
        dgPortfolio.EnableHeadersVisualStyles = False
        dgPortfolio.Location = New Point(9, 51)
        dgPortfolio.MultiSelect = False
        dgPortfolio.Name = "dgPortfolio"
        dgPortfolio.ReadOnly = True
        DataGridViewCellStyle3.BackColor = Color.FromArgb(CByte(64), CByte(64), CByte(64))
        DataGridViewCellStyle3.Font = New Font("Segoe UI", 9F)
        DataGridViewCellStyle3.ForeColor = SystemColors.ButtonHighlight
        DataGridViewCellStyle3.Padding = New Padding(2)
        DataGridViewCellStyle3.SelectionBackColor = Color.Transparent
        DataGridViewCellStyle3.SelectionForeColor = Color.Transparent
        DataGridViewCellStyle3.WrapMode = DataGridViewTriState.True
        dgPortfolio.RowHeadersDefaultCellStyle = DataGridViewCellStyle3
        dgPortfolio.RowHeadersWidth = 4
        dgPortfolio.RowTemplate.DefaultCellStyle.SelectionBackColor = Color.DarkOrange
        dgPortfolio.RowTemplate.DefaultCellStyle.SelectionForeColor = Color.FromArgb(CByte(64), CByte(64), CByte(64))
        dgPortfolio.RowTemplate.DefaultCellStyle.WrapMode = DataGridViewTriState.True
        dgPortfolio.ScrollBars = ScrollBars.Vertical
        dgPortfolio.SelectionMode = DataGridViewSelectionMode.CellSelect
        dgPortfolio.Size = New Size(1132, 262)
        dgPortfolio.TabIndex = 11
        ' 
        ' PanelProfits
        ' 
        PanelProfits.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        PanelProfits.BackColor = Color.FromArgb(CByte(56), CByte(86), CByte(35))
        PanelProfits.Controls.Add(Label15)
        PanelProfits.Controls.Add(Label5)
        PanelProfits.Controls.Add(lbRoiSeparator)
        PanelProfits.Controls.Add(lbValoresSeparator)
        PanelProfits.Controls.Add(lbValoresHojeBRL)
        PanelProfits.Controls.Add(lbValoresHojeUSD)
        PanelProfits.Location = New Point(0, 695)
        PanelProfits.Name = "PanelProfits"
        PanelProfits.Size = New Size(1151, 40)
        PanelProfits.TabIndex = 15
        ' 
        ' Label15
        ' 
        Label15.AutoSize = True
        Label15.BackColor = Color.Transparent
        Label15.Font = New Font("Calibri", 15.75F, FontStyle.Bold, GraphicsUnit.Point, CByte(0))
        Label15.ForeColor = Color.GreenYellow
        Label15.Location = New Point(9, 7)
        Label15.Name = "Label15"
        Label15.Size = New Size(93, 26)
        Label15.TabIndex = 17
        Label15.Text = "L/P Total:"
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
        ' lbRoiSeparator
        ' 
        lbRoiSeparator.AutoSize = True
        lbRoiSeparator.Font = New Font("Segoe UI", 16F, FontStyle.Bold)
        lbRoiSeparator.ForeColor = Color.White
        lbRoiSeparator.Location = New Point(498, 3)
        lbRoiSeparator.Name = "lbRoiSeparator"
        lbRoiSeparator.Size = New Size(23, 30)
        lbRoiSeparator.TabIndex = 37
        lbRoiSeparator.Text = "/"
        lbRoiSeparator.Visible = False
        ' 
        ' lbValoresSeparator
        ' 
        lbValoresSeparator.AutoSize = True
        lbValoresSeparator.Font = New Font("Segoe UI", 16F, FontStyle.Bold)
        lbValoresSeparator.ForeColor = Color.White
        lbValoresSeparator.Location = New Point(516, 5)
        lbValoresSeparator.Name = "lbValoresSeparator"
        lbValoresSeparator.Size = New Size(23, 30)
        lbValoresSeparator.TabIndex = 38
        lbValoresSeparator.Text = "/"
        ' 
        ' lbValoresHojeBRL
        ' 
        lbValoresHojeBRL.AutoSize = True
        lbValoresHojeBRL.Font = New Font("Segoe UI", 16F, FontStyle.Bold)
        lbValoresHojeBRL.ForeColor = Color.DeepSkyBlue
        lbValoresHojeBRL.Location = New Point(826, 5)
        lbValoresHojeBRL.Name = "lbValoresHojeBRL"
        lbValoresHojeBRL.Size = New Size(58, 30)
        lbValoresHojeBRL.TabIndex = 28
        lbValoresHojeBRL.Text = "0,00"
        ' 
        ' lbValoresHojeUSD
        ' 
        lbValoresHojeUSD.AutoSize = True
        lbValoresHojeUSD.Font = New Font("Segoe UI", 16F, FontStyle.Bold)
        lbValoresHojeUSD.ForeColor = Color.LimeGreen
        lbValoresHojeUSD.Location = New Point(679, 5)
        lbValoresHojeUSD.Name = "lbValoresHojeUSD"
        lbValoresHojeUSD.Size = New Size(58, 30)
        lbValoresHojeUSD.TabIndex = 27
        lbValoresHojeUSD.Text = "0.00"
        ' 
        ' Label7
        ' 
        Label7.AutoSize = True
        Label7.Font = New Font("Calibri", 15.75F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        Label7.ForeColor = Color.White
        Label7.Location = New Point(90, 280)
        Label7.Name = "Label7"
        Label7.Size = New Size(48, 26)
        Label7.TabIndex = 3
        Label7.Text = "ROI:"
        ' 
        ' lbRoiUSD
        ' 
        lbRoiUSD.AutoSize = True
        lbRoiUSD.Font = New Font("Calibri", 15.75F)
        lbRoiUSD.ForeColor = Color.Lime
        lbRoiUSD.Location = New Point(134, 281)
        lbRoiUSD.Name = "lbRoiUSD"
        lbRoiUSD.Size = New Size(50, 26)
        lbRoiUSD.TabIndex = 26
        lbRoiUSD.Text = "0.00"
        ' 
        ' lbTotalBRL
        ' 
        lbTotalBRL.AutoSize = True
        lbTotalBRL.Font = New Font("Calibri", 15.75F)
        lbTotalBRL.ForeColor = Color.Lime
        lbTotalBRL.Location = New Point(260, 281)
        lbTotalBRL.Name = "lbTotalBRL"
        lbTotalBRL.Size = New Size(50, 26)
        lbTotalBRL.TabIndex = 16
        lbTotalBRL.Text = "0,00"
        lbTotalBRL.TextAlign = ContentAlignment.MiddleCenter
        ' 
        ' lbLoadFromMarket
        ' 
        lbLoadFromMarket.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        lbLoadFromMarket.BackColor = Color.Transparent
        lbLoadFromMarket.Font = New Font("Segoe UI", 9F, FontStyle.Italic)
        lbLoadFromMarket.ForeColor = Color.OrangeRed
        lbLoadFromMarket.Location = New Point(4, 30)
        lbLoadFromMarket.Name = "lbLoadFromMarket"
        lbLoadFromMarket.Size = New Size(1143, 19)
        lbLoadFromMarket.TabIndex = 0
        lbLoadFromMarket.Text = "Buscando dados"
        lbLoadFromMarket.TextAlign = ContentAlignment.MiddleCenter
        lbLoadFromMarket.Visible = False
        ' 
        ' Panel1
        ' 
        Panel1.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        Panel1.Controls.Add(lbAtualizaEm)
        Panel1.Controls.Add(lbRefresh)
        Panel1.Controls.Add(Label4)
        Panel1.Controls.Add(lbDom)
        Panel1.Controls.Add(Label3)
        Panel1.Controls.Add(lbBTC)
        Panel1.Controls.Add(Label1)
        Panel1.Controls.Add(lbDolar)
        Panel1.Location = New Point(474, -1)
        Panel1.Name = "Panel1"
        Panel1.Size = New Size(673, 29)
        Panel1.TabIndex = 22
        ' 
        ' lbAtualizaEm
        ' 
        lbAtualizaEm.AutoSize = True
        lbAtualizaEm.ForeColor = Color.White
        lbAtualizaEm.Location = New Point(77, 8)
        lbAtualizaEm.Name = "lbAtualizaEm"
        lbAtualizaEm.Size = New Size(27, 15)
        lbAtualizaEm.TabIndex = 28
        lbAtualizaEm.Text = "em:"
        ' 
        ' lbRefresh
        ' 
        lbRefresh.AutoSize = True
        lbRefresh.Font = New Font("Segoe UI", 9F, FontStyle.Bold)
        lbRefresh.ForeColor = Color.Gold
        lbRefresh.Location = New Point(110, 9)
        lbRefresh.Name = "lbRefresh"
        lbRefresh.Size = New Size(38, 15)
        lbRefresh.TabIndex = 0
        lbRefresh.Text = "00:00"
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
        ' btRefresh
        ' 
        btRefresh.BackColor = Color.DeepSkyBlue
        btRefresh.BackgroundImageLayout = ImageLayout.Stretch
        btRefresh.Cursor = Cursors.Hand
        btRefresh.FlatAppearance.BorderSize = 0
        btRefresh.FlatStyle = FlatStyle.Flat
        btRefresh.Font = New Font("Calibri", 10F)
        btRefresh.ForeColor = Color.White
        btRefresh.ImageAlign = ContentAlignment.MiddleRight
        btRefresh.Location = New Point(426, 1)
        btRefresh.Name = "btRefresh"
        btRefresh.Size = New Size(117, 27)
        btRefresh.TabIndex = 27
        btRefresh.Text = "Atualizar posições"
        ToolTip1.SetToolTip(btRefresh, "Atualizar Mercado")
        btRefresh.UseVisualStyleBackColor = False
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
        ' PictureBox2
        ' 
        PictureBox2.Image = CType(resources.GetObject("PictureBox2.Image"), Image)
        PictureBox2.Location = New Point(263, 114)
        PictureBox2.Name = "PictureBox2"
        PictureBox2.Size = New Size(35, 25)
        PictureBox2.SizeMode = PictureBoxSizeMode.StretchImage
        PictureBox2.TabIndex = 31
        PictureBox2.TabStop = False
        ' 
        ' PictureBox1
        ' 
        PictureBox1.Image = CType(resources.GetObject("PictureBox1.Image"), Image)
        PictureBox1.Location = New Point(136, 114)
        PictureBox1.Name = "PictureBox1"
        PictureBox1.Size = New Size(35, 25)
        PictureBox1.SizeMode = PictureBoxSizeMode.StretchImage
        PictureBox1.TabIndex = 30
        PictureBox1.TabStop = False
        ' 
        ' Label10
        ' 
        Label10.AutoSize = True
        Label10.Font = New Font("Calibri", 15.75F, FontStyle.Italic)
        Label10.ForeColor = Color.White
        Label10.Location = New Point(8, 155)
        Label10.Name = "Label10"
        Label10.Size = New Size(129, 26)
        Label10.TabIndex = 35
        Label10.Text = "Investimento:"
        ToolTip1.SetToolTip(Label10, "Caixa Spot + caixa Futuros + valor de compra das moedas")
        ' 
        ' lbPercentInvestido
        ' 
        lbPercentInvestido.AutoSize = True
        lbPercentInvestido.Font = New Font("Calibri", 9.75F, FontStyle.Italic)
        lbPercentInvestido.ForeColor = Color.LimeGreen
        lbPercentInvestido.Location = New Point(633, 337)
        lbPercentInvestido.Name = "lbPercentInvestido"
        lbPercentInvestido.Size = New Size(23, 15)
        lbPercentInvestido.TabIndex = 16
        lbPercentInvestido.Text = "0%"
        ' 
        ' lbPercentCaixa
        ' 
        lbPercentCaixa.AutoSize = True
        lbPercentCaixa.Font = New Font("Calibri", 9.75F, FontStyle.Italic)
        lbPercentCaixa.ForeColor = Color.White
        lbPercentCaixa.Location = New Point(530, 337)
        lbPercentCaixa.Name = "lbPercentCaixa"
        lbPercentCaixa.Size = New Size(23, 15)
        lbPercentCaixa.TabIndex = 15
        lbPercentCaixa.Text = "0%"
        ' 
        ' Label9
        ' 
        Label9.AutoSize = True
        Label9.Font = New Font("Calibri", 15.75F, FontStyle.Italic)
        Label9.ForeColor = Color.White
        Label9.Location = New Point(72, 185)
        Label9.Name = "Label9"
        Label9.Size = New Size(65, 26)
        Label9.TabIndex = 14
        Label9.Text = "Caixa:"
        ToolTip1.SetToolTip(Label9, "Valor em SPOT + FUTUROS")
        ' 
        ' Label16
        ' 
        Label16.AutoSize = True
        Label16.Font = New Font("Calibri", 20.25F, FontStyle.Italic, GraphicsUnit.Point, CByte(0))
        Label16.ForeColor = Color.White
        Label16.Location = New Point(92, 53)
        Label16.Name = "Label16"
        Label16.Size = New Size(159, 33)
        Label16.TabIndex = 12
        Label16.Text = "Desempenho"
        ' 
        ' lbPerformWallet
        ' 
        lbPerformWallet.AutoSize = True
        lbPerformWallet.Font = New Font("Calibri", 20F, FontStyle.Italic)
        lbPerformWallet.ForeColor = Color.GreenYellow
        lbPerformWallet.Location = New Point(253, 53)
        lbPerformWallet.Name = "lbPerformWallet"
        lbPerformWallet.Size = New Size(48, 33)
        lbPerformWallet.TabIndex = 13
        lbPerformWallet.Text = "0%"
        ' 
        ' Label6
        ' 
        Label6.AutoSize = True
        Label6.Font = New Font("Calibri", 15.75F, FontStyle.Italic)
        Label6.ForeColor = Color.White
        Label6.Location = New Point(80, 215)
        Label6.Name = "Label6"
        Label6.Size = New Size(57, 26)
        Label6.TabIndex = 0
        Label6.Text = "Spot:"
        ' 
        ' LabelFuturos
        ' 
        LabelFuturos.AutoSize = True
        LabelFuturos.Font = New Font("Calibri", 15.75F, FontStyle.Italic)
        LabelFuturos.ForeColor = Color.White
        LabelFuturos.Location = New Point(54, 245)
        LabelFuturos.Name = "LabelFuturos"
        LabelFuturos.Size = New Size(83, 26)
        LabelFuturos.TabIndex = 34
        LabelFuturos.Text = "Futuros:"
        ' 
        ' lbDataTotalToday
        ' 
        lbDataTotalToday.AutoSize = True
        lbDataTotalToday.Font = New Font("Calibri", 12F, FontStyle.Italic)
        lbDataTotalToday.ForeColor = Color.DarkOrange
        lbDataTotalToday.Location = New Point(99, 34)
        lbDataTotalToday.Name = "lbDataTotalToday"
        lbDataTotalToday.Size = New Size(85, 19)
        lbDataTotalToday.TabIndex = 9
        lbDataTotalToday.Text = "00/00/0000"
        ' 
        ' lbInvestimentoBRL
        ' 
        lbInvestimentoBRL.AutoSize = True
        lbInvestimentoBRL.Font = New Font("Calibri", 15.75F, FontStyle.Bold Or FontStyle.Italic)
        lbInvestimentoBRL.ForeColor = Color.Gold
        lbInvestimentoBRL.Location = New Point(260, 155)
        lbInvestimentoBRL.Name = "lbInvestimentoBRL"
        lbInvestimentoBRL.Size = New Size(50, 26)
        lbInvestimentoBRL.TabIndex = 41
        lbInvestimentoBRL.Text = "0,00"
        ' 
        ' lbInvestimentoUSD
        ' 
        lbInvestimentoUSD.AutoSize = True
        lbInvestimentoUSD.Font = New Font("Calibri", 15.75F, FontStyle.Bold Or FontStyle.Italic)
        lbInvestimentoUSD.ForeColor = Color.Yellow
        lbInvestimentoUSD.Location = New Point(133, 155)
        lbInvestimentoUSD.Name = "lbInvestimentoUSD"
        lbInvestimentoUSD.Size = New Size(51, 26)
        lbInvestimentoUSD.TabIndex = 40
        lbInvestimentoUSD.Text = "0.00"
        ' 
        ' lbTotalEntradaBRL
        ' 
        lbTotalEntradaBRL.AutoSize = True
        lbTotalEntradaBRL.Font = New Font("Calibri", 15.75F, FontStyle.Bold Or FontStyle.Italic)
        lbTotalEntradaBRL.ForeColor = Color.DeepSkyBlue
        lbTotalEntradaBRL.Location = New Point(260, 215)
        lbTotalEntradaBRL.Name = "lbTotalEntradaBRL"
        lbTotalEntradaBRL.Size = New Size(50, 26)
        lbTotalEntradaBRL.TabIndex = 25
        lbTotalEntradaBRL.Text = "0,00"
        ' 
        ' lbTotalEntradaUSD
        ' 
        lbTotalEntradaUSD.AutoSize = True
        lbTotalEntradaUSD.Font = New Font("Calibri", 15.75F, FontStyle.Bold Or FontStyle.Italic)
        lbTotalEntradaUSD.ForeColor = Color.LimeGreen
        lbTotalEntradaUSD.Location = New Point(133, 215)
        lbTotalEntradaUSD.Name = "lbTotalEntradaUSD"
        lbTotalEntradaUSD.Size = New Size(51, 26)
        lbTotalEntradaUSD.TabIndex = 24
        lbTotalEntradaUSD.Text = "0.00"
        ' 
        ' lbFuturosBRL
        ' 
        lbFuturosBRL.AutoSize = True
        lbFuturosBRL.Font = New Font("Calibri", 15.75F, FontStyle.Bold Or FontStyle.Italic)
        lbFuturosBRL.ForeColor = Color.DeepSkyBlue
        lbFuturosBRL.Location = New Point(260, 245)
        lbFuturosBRL.Name = "lbFuturosBRL"
        lbFuturosBRL.Size = New Size(50, 26)
        lbFuturosBRL.TabIndex = 35
        lbFuturosBRL.Text = "0,00"
        ' 
        ' lbFuturosUSD
        ' 
        lbFuturosUSD.AutoSize = True
        lbFuturosUSD.Font = New Font("Calibri", 15.75F, FontStyle.Bold Or FontStyle.Italic)
        lbFuturosUSD.ForeColor = Color.LimeGreen
        lbFuturosUSD.Location = New Point(133, 245)
        lbFuturosUSD.Name = "lbFuturosUSD"
        lbFuturosUSD.Size = New Size(51, 26)
        lbFuturosUSD.TabIndex = 36
        lbFuturosUSD.Text = "0.00"
        ' 
        ' PanelGraphs
        ' 
        PanelGraphs.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        PanelGraphs.BackColor = Color.FromArgb(CByte(30), CByte(30), CByte(30))
        PanelGraphs.Controls.Add(Label11)
        PanelGraphs.Controls.Add(Label2)
        PanelGraphs.Controls.Add(Label7)
        PanelGraphs.Controls.Add(lbCaixaBRL)
        PanelGraphs.Controls.Add(Label10)
        PanelGraphs.Controls.Add(lbPercentInvestido)
        PanelGraphs.Controls.Add(lbRoiUSD)
        PanelGraphs.Controls.Add(lbPercentCaixa)
        PanelGraphs.Controls.Add(lbTotalBRL)
        PanelGraphs.Controls.Add(Label9)
        PanelGraphs.Controls.Add(lbCaixa)
        PanelGraphs.Controls.Add(Label6)
        PanelGraphs.Controls.Add(PictureBox2)
        PanelGraphs.Controls.Add(LabelFuturos)
        PanelGraphs.Controls.Add(Label16)
        PanelGraphs.Controls.Add(lbPerformWallet)
        PanelGraphs.Controls.Add(PictureBox1)
        PanelGraphs.Controls.Add(lbDataTotalToday)
        PanelGraphs.Controls.Add(lbInvestimentoUSD)
        PanelGraphs.Controls.Add(lbTotalEntradaUSD)
        PanelGraphs.Controls.Add(lbInvestimentoBRL)
        PanelGraphs.Controls.Add(lbTotalEntradaBRL)
        PanelGraphs.Controls.Add(lbFuturosBRL)
        PanelGraphs.Controls.Add(lbFuturosUSD)
        PanelGraphs.Location = New Point(0, 340)
        PanelGraphs.Name = "PanelGraphs"
        PanelGraphs.Size = New Size(1151, 355)
        PanelGraphs.TabIndex = 32
        ' 
        ' Label11
        ' 
        Label11.AutoSize = True
        Label11.Font = New Font("Calibri", 9.75F, FontStyle.Italic)
        Label11.ForeColor = Color.OrangeRed
        Label11.Location = New Point(577, 336)
        Label11.Name = "Label11"
        Label11.Size = New Size(58, 15)
        Label11.TabIndex = 43
        Label11.Text = "Investido:"
        ToolTip1.SetToolTip(Label11, "Valor em SPOT + FUTUROS")
        ' 
        ' Label2
        ' 
        Label2.AutoSize = True
        Label2.Font = New Font("Calibri", 9.75F, FontStyle.Italic)
        Label2.ForeColor = Color.OrangeRed
        Label2.Location = New Point(492, 336)
        Label2.Name = "Label2"
        Label2.Size = New Size(40, 15)
        Label2.TabIndex = 42
        Label2.Text = "Caixa:"
        ToolTip1.SetToolTip(Label2, "Valor em SPOT + FUTUROS")
        ' 
        ' lbCaixaBRL
        ' 
        lbCaixaBRL.AutoSize = True
        lbCaixaBRL.Font = New Font("Calibri", 15.75F, FontStyle.Bold Or FontStyle.Italic)
        lbCaixaBRL.ForeColor = Color.Ivory
        lbCaixaBRL.Location = New Point(260, 185)
        lbCaixaBRL.Name = "lbCaixaBRL"
        lbCaixaBRL.Size = New Size(51, 26)
        lbCaixaBRL.TabIndex = 33
        lbCaixaBRL.Text = "0.00"
        ' 
        ' lbCaixa
        ' 
        lbCaixa.AutoSize = True
        lbCaixa.Cursor = Cursors.Hand
        lbCaixa.Font = New Font("Calibri", 15.75F, FontStyle.Bold Or FontStyle.Italic)
        lbCaixa.ForeColor = Color.Ivory
        lbCaixa.Location = New Point(133, 185)
        lbCaixa.Name = "lbCaixa"
        lbCaixa.Size = New Size(51, 26)
        lbCaixa.TabIndex = 32
        lbCaixa.Text = "0.00"
        ' 
        ' PanelOrdens
        ' 
        PanelOrdens.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        PanelOrdens.BackColor = Color.FromArgb(CByte(30), CByte(30), CByte(30))
        PanelOrdens.Location = New Point(0, 339)
        PanelOrdens.Name = "PanelOrdens"
        PanelOrdens.Size = New Size(1151, 263)
        PanelOrdens.TabIndex = 42
        PanelOrdens.Visible = False
        ' 
        ' dgFuturos
        ' 
        dgFuturos.AllowUserToAddRows = False
        dgFuturos.AllowUserToDeleteRows = False
        dgFuturos.AllowUserToOrderColumns = True
        dgFuturos.AllowUserToResizeRows = False
        dgFuturos.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        dgFuturos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        dgFuturos.BackgroundColor = Color.FromArgb(CByte(64), CByte(64), CByte(64))
        dgFuturos.BorderStyle = BorderStyle.None
        dgFuturos.CellBorderStyle = DataGridViewCellBorderStyle.None
        dgFuturos.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None
        DataGridViewCellStyle4.Alignment = DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle4.BackColor = Color.FromArgb(CByte(40), CByte(40), CByte(40))
        DataGridViewCellStyle4.Font = New Font("Calibri", 10F, FontStyle.Italic)
        DataGridViewCellStyle4.ForeColor = Color.Turquoise
        DataGridViewCellStyle4.SelectionBackColor = Color.FromArgb(CByte(40), CByte(40), CByte(40))
        DataGridViewCellStyle4.SelectionForeColor = Color.Turquoise
        DataGridViewCellStyle4.WrapMode = DataGridViewTriState.True
        dgFuturos.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle4
        DataGridViewCellStyle5.Alignment = DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle5.BackColor = Color.FromArgb(CByte(64), CByte(64), CByte(64))
        DataGridViewCellStyle5.Font = New Font("Calibri", 12F)
        DataGridViewCellStyle5.ForeColor = Color.White
        DataGridViewCellStyle5.SelectionBackColor = Color.Black
        DataGridViewCellStyle5.SelectionForeColor = Color.White
        DataGridViewCellStyle5.WrapMode = DataGridViewTriState.True
        dgFuturos.DefaultCellStyle = DataGridViewCellStyle5
        dgFuturos.EnableHeadersVisualStyles = False
        dgFuturos.Location = New Point(9, 51)
        dgFuturos.MultiSelect = False
        dgFuturos.Name = "dgFuturos"
        dgFuturos.ReadOnly = True
        DataGridViewCellStyle6.BackColor = Color.Black
        DataGridViewCellStyle6.Font = New Font("Calibri", 12F)
        DataGridViewCellStyle6.ForeColor = Color.White
        DataGridViewCellStyle6.Padding = New Padding(2)
        DataGridViewCellStyle6.SelectionBackColor = Color.Black
        DataGridViewCellStyle6.SelectionForeColor = Color.White
        DataGridViewCellStyle6.WrapMode = DataGridViewTriState.True
        dgFuturos.RowHeadersDefaultCellStyle = DataGridViewCellStyle6
        dgFuturos.RowHeadersWidth = 4
        DataGridViewCellStyle7.BackColor = Color.Black
        DataGridViewCellStyle7.ForeColor = Color.White
        DataGridViewCellStyle7.SelectionBackColor = Color.Black
        DataGridViewCellStyle7.SelectionForeColor = Color.White
        dgFuturos.RowsDefaultCellStyle = DataGridViewCellStyle7
        dgFuturos.RowTemplate.DefaultCellStyle.SelectionBackColor = Color.Black
        dgFuturos.RowTemplate.DefaultCellStyle.SelectionForeColor = Color.White
        dgFuturos.RowTemplate.DefaultCellStyle.WrapMode = DataGridViewTriState.True
        dgFuturos.ScrollBars = ScrollBars.Vertical
        dgFuturos.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgFuturos.Size = New Size(1132, 262)
        dgFuturos.TabIndex = 37
        dgFuturos.Visible = False
        ' 
        ' Panel2
        ' 
        Panel2.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        Panel2.Location = New Point(0, 289)
        Panel2.Name = "Panel2"
        Panel2.Size = New Size(1151, 25)
        Panel2.TabIndex = 33
        ' 
        ' CadastroToolStripMenuItem
        ' 
        CadastroToolStripMenuItem.DropDownItems.AddRange(New ToolStripItem() {CriptoToolStripMenuItem, SimbolosToolStripMenuItem, CustodiaToolStripMenuItem})
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
        CriptoToolStripMenuItem.Size = New Size(123, 22)
        CriptoToolStripMenuItem.Text = "Aportes "
        ' 
        ' SimbolosToolStripMenuItem
        ' 
        SimbolosToolStripMenuItem.BackColor = Color.FromArgb(CByte(31), CByte(33), CByte(32))
        SimbolosToolStripMenuItem.ForeColor = SystemColors.ButtonHighlight
        SimbolosToolStripMenuItem.Name = "SimbolosToolStripMenuItem"
        SimbolosToolStripMenuItem.Size = New Size(123, 22)
        SimbolosToolStripMenuItem.Text = "Simbolos"
        ' 
        ' CustodiaToolStripMenuItem
        ' 
        CustodiaToolStripMenuItem.BackColor = Color.FromArgb(CByte(31), CByte(33), CByte(32))
        CustodiaToolStripMenuItem.ForeColor = SystemColors.ButtonHighlight
        CustodiaToolStripMenuItem.Name = "CustodiaToolStripMenuItem"
        CustodiaToolStripMenuItem.Size = New Size(123, 22)
        CustodiaToolStripMenuItem.Text = "Custodia"
        ' 
        ' OpçõesToolStripMenuItem
        ' 
        OpçõesToolStripMenuItem.DropDownItems.AddRange(New ToolStripItem() {APIToolStripMenuItem, IntervaloToolStripMenuItem})
        OpçõesToolStripMenuItem.ForeColor = SystemColors.ButtonHighlight
        OpçõesToolStripMenuItem.Name = "OpçõesToolStripMenuItem"
        OpçõesToolStripMenuItem.Size = New Size(96, 20)
        OpçõesToolStripMenuItem.Text = "Configurações"
        ' 
        ' APIToolStripMenuItem
        ' 
        APIToolStripMenuItem.BackColor = Color.FromArgb(CByte(31), CByte(33), CByte(32))
        APIToolStripMenuItem.ForeColor = SystemColors.ButtonHighlight
        APIToolStripMenuItem.Name = "APIToolStripMenuItem"
        APIToolStripMenuItem.Size = New Size(120, 22)
        APIToolStripMenuItem.Text = "API"
        ' 
        ' IntervaloToolStripMenuItem
        ' 
        IntervaloToolStripMenuItem.BackColor = Color.FromArgb(CByte(31), CByte(33), CByte(32))
        IntervaloToolStripMenuItem.ForeColor = SystemColors.ButtonHighlight
        IntervaloToolStripMenuItem.Name = "IntervaloToolStripMenuItem"
        IntervaloToolStripMenuItem.Size = New Size(120, 22)
        IntervaloToolStripMenuItem.Text = "Intervalo"
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
        MenuStrip1.Size = New Size(1151, 24)
        MenuStrip1.TabIndex = 10
        MenuStrip1.Text = "MenuStrip1"
        ' 
        ' pbBRL
        ' 
        pbBRL.Cursor = Cursors.Hand
        pbBRL.Image = CType(resources.GetObject("pbBRL.Image"), Image)
        pbBRL.Location = New Point(1117, 33)
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
        pbUSD.Location = New Point(1088, 33)
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
        Label8.Location = New Point(1041, 33)
        Label8.Name = "Label8"
        Label8.Size = New Size(47, 15)
        Label8.TabIndex = 36
        Label8.Text = "Moeda:"
        ' 
        ' panelDebug
        ' 
        panelDebug.BackColor = Color.Black
        panelDebug.Controls.Add(lbDebug)
        panelDebug.Dock = DockStyle.Bottom
        panelDebug.Location = New Point(0, 735)
        panelDebug.Name = "panelDebug"
        panelDebug.Size = New Size(1151, 75)
        panelDebug.TabIndex = 18
        ' 
        ' lbDebug
        ' 
        lbDebug.Anchor = AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
        lbDebug.BackColor = SystemColors.MenuText
        lbDebug.BorderStyle = BorderStyle.None
        lbDebug.Font = New Font("Verdana", 11F, FontStyle.Italic, GraphicsUnit.Point, CByte(0))
        lbDebug.ForeColor = Color.White
        lbDebug.ImeMode = ImeMode.Disable
        lbDebug.Location = New Point(11, 3)
        lbDebug.Margin = New Padding(2, 3, 3, 3)
        lbDebug.Name = "lbDebug"
        lbDebug.ReadOnly = True
        lbDebug.ScrollBars = RichTextBoxScrollBars.Vertical
        lbDebug.Size = New Size(1140, 72)
        lbDebug.TabIndex = 0
        lbDebug.Text = ""
        ' 
        ' btSpot
        ' 
        btSpot.BackColor = Color.SteelBlue
        btSpot.Cursor = Cursors.Hand
        btSpot.FlatAppearance.BorderColor = Color.Gray
        btSpot.FlatAppearance.BorderSize = 0
        btSpot.FlatAppearance.MouseDownBackColor = Color.DodgerBlue
        btSpot.FlatAppearance.MouseOverBackColor = Color.RoyalBlue
        btSpot.FlatStyle = FlatStyle.Flat
        btSpot.ForeColor = Color.White
        btSpot.Location = New Point(9, 28)
        btSpot.Name = "btSpot"
        btSpot.Size = New Size(75, 23)
        btSpot.TabIndex = 38
        btSpot.Text = "Spot"
        btSpot.UseVisualStyleBackColor = False
        ' 
        ' btFuturos
        ' 
        btFuturos.BackColor = Color.FromArgb(CByte(20), CByte(20), CByte(20))
        btFuturos.Cursor = Cursors.Hand
        btFuturos.FlatAppearance.BorderColor = Color.Gray
        btFuturos.FlatAppearance.BorderSize = 0
        btFuturos.FlatAppearance.MouseDownBackColor = Color.DodgerBlue
        btFuturos.FlatAppearance.MouseOverBackColor = Color.RoyalBlue
        btFuturos.FlatStyle = FlatStyle.Flat
        btFuturos.ForeColor = Color.White
        btFuturos.Location = New Point(84, 28)
        btFuturos.Name = "btFuturos"
        btFuturos.Size = New Size(75, 23)
        btFuturos.TabIndex = 39
        btFuturos.Text = "Futuros"
        btFuturos.UseVisualStyleBackColor = False
        ' 
        ' btVisaoGeral
        ' 
        btVisaoGeral.BackColor = Color.SteelBlue
        btVisaoGeral.Cursor = Cursors.Hand
        btVisaoGeral.FlatAppearance.BorderColor = Color.Gray
        btVisaoGeral.FlatAppearance.BorderSize = 0
        btVisaoGeral.FlatAppearance.MouseDownBackColor = Color.DodgerBlue
        btVisaoGeral.FlatAppearance.MouseOverBackColor = Color.RoyalBlue
        btVisaoGeral.FlatStyle = FlatStyle.Flat
        btVisaoGeral.ForeColor = Color.White
        btVisaoGeral.Location = New Point(5, 316)
        btVisaoGeral.Name = "btVisaoGeral"
        btVisaoGeral.Size = New Size(75, 23)
        btVisaoGeral.TabIndex = 40
        btVisaoGeral.Text = "Visão geral"
        btVisaoGeral.UseVisualStyleBackColor = False
        ' 
        ' btOrdensSpot
        ' 
        btOrdensSpot.BackColor = Color.FromArgb(CByte(20), CByte(20), CByte(20))
        btOrdensSpot.Cursor = Cursors.Hand
        btOrdensSpot.FlatAppearance.BorderColor = Color.Gray
        btOrdensSpot.FlatAppearance.BorderSize = 0
        btOrdensSpot.FlatAppearance.MouseDownBackColor = Color.DodgerBlue
        btOrdensSpot.FlatAppearance.MouseOverBackColor = Color.RoyalBlue
        btOrdensSpot.FlatStyle = FlatStyle.Flat
        btOrdensSpot.ForeColor = Color.White
        btOrdensSpot.Location = New Point(80, 316)
        btOrdensSpot.Name = "btOrdensSpot"
        btOrdensSpot.Size = New Size(100, 23)
        btOrdensSpot.TabIndex = 41
        btOrdensSpot.Text = "Ordens Spot"
        btOrdensSpot.UseVisualStyleBackColor = False
        ' 
        ' PanelFuturos
        ' 
        PanelFuturos.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        PanelFuturos.BackColor = Color.FromArgb(CByte(30), CByte(30), CByte(30))
        PanelFuturos.Location = New Point(0, 340)
        PanelFuturos.Name = "PanelFuturos"
        PanelFuturos.Size = New Size(1151, 265)
        PanelFuturos.TabIndex = 43
        PanelFuturos.Visible = False
        ' 
        ' btOrdensFuturo
        ' 
        btOrdensFuturo.BackColor = Color.FromArgb(CByte(20), CByte(20), CByte(20))
        btOrdensFuturo.Cursor = Cursors.Hand
        btOrdensFuturo.FlatAppearance.BorderColor = Color.Gray
        btOrdensFuturo.FlatAppearance.BorderSize = 0
        btOrdensFuturo.FlatAppearance.MouseDownBackColor = Color.DodgerBlue
        btOrdensFuturo.FlatAppearance.MouseOverBackColor = Color.RoyalBlue
        btOrdensFuturo.FlatStyle = FlatStyle.Flat
        btOrdensFuturo.ForeColor = Color.White
        btOrdensFuturo.Location = New Point(180, 316)
        btOrdensFuturo.Name = "btOrdensFuturo"
        btOrdensFuturo.Size = New Size(100, 23)
        btOrdensFuturo.TabIndex = 44
        btOrdensFuturo.Text = "Ordens Futuros"
        btOrdensFuturo.UseVisualStyleBackColor = False
        ' 
        ' btTradebot
        ' 
        btTradebot.BackColor = Color.FromArgb(CByte(20), CByte(20), CByte(20))
        btTradebot.Cursor = Cursors.Hand
        btTradebot.FlatAppearance.BorderColor = Color.Gray
        btTradebot.FlatAppearance.BorderSize = 0
        btTradebot.FlatAppearance.MouseDownBackColor = Color.DodgerBlue
        btTradebot.FlatAppearance.MouseOverBackColor = Color.RoyalBlue
        btTradebot.FlatStyle = FlatStyle.Flat
        btTradebot.ForeColor = Color.White
        btTradebot.Location = New Point(280, 316)
        btTradebot.Name = "btTradebot"
        btTradebot.Size = New Size(100, 23)
        btTradebot.TabIndex = 45
        btTradebot.Text = "TradeBot"
        btTradebot.UseVisualStyleBackColor = False
        ' 
        ' PanelTradeBot
        ' 
        PanelTradeBot.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        PanelTradeBot.BackColor = Color.FromArgb(CByte(30), CByte(30), CByte(30))
        PanelTradeBot.Controls.Add(btSpotBotTrades)
        PanelTradeBot.Controls.Add(btSpotBotExport)
        PanelTradeBot.Controls.Add(lbSpotBotSimulationSummary)
        PanelTradeBot.Controls.Add(rtbSpotBotLog)
        PanelTradeBot.Controls.Add(GroupBox4)
        PanelTradeBot.Controls.Add(dgSpotBotPositionsControl)
        PanelTradeBot.Controls.Add(GroupBox1)
        PanelTradeBot.Location = New Point(0, 340)
        PanelTradeBot.Name = "PanelTradeBot"
        PanelTradeBot.Size = New Size(1151, 355)
        PanelTradeBot.TabIndex = 46
        ' 
        ' btSpotBotTrades
        ' 
        btSpotBotTrades.BackColor = Color.FromArgb(CByte(55), CByte(75), CByte(105))
        btSpotBotTrades.Cursor = Cursors.Hand
        btSpotBotTrades.FlatAppearance.BorderSize = 0
        btSpotBotTrades.FlatAppearance.MouseDownBackColor = Color.DodgerBlue
        btSpotBotTrades.FlatAppearance.MouseOverBackColor = Color.RoyalBlue
        btSpotBotTrades.FlatStyle = FlatStyle.Flat
        btSpotBotTrades.ForeColor = Color.White
        btSpotBotTrades.Location = New Point(1083, 204)
        btSpotBotTrades.Name = "btSpotBotTrades"
        btSpotBotTrades.Size = New Size(60, 26)
        btSpotBotTrades.TabIndex = 68
        btSpotBotTrades.Text = "Trades"
        btSpotBotTrades.UseVisualStyleBackColor = False
        ' 
        ' btSpotBotExport
        ' 
        btSpotBotExport.BackColor = Color.FromArgb(CByte(55), CByte(75), CByte(105))
        btSpotBotExport.Cursor = Cursors.Hand
        btSpotBotExport.FlatAppearance.BorderSize = 0
        btSpotBotExport.FlatAppearance.MouseDownBackColor = Color.DodgerBlue
        btSpotBotExport.FlatAppearance.MouseOverBackColor = Color.RoyalBlue
        btSpotBotExport.FlatStyle = FlatStyle.Flat
        btSpotBotExport.ForeColor = Color.White
        btSpotBotExport.Location = New Point(1008, 204)
        btSpotBotExport.Name = "btSpotBotExport"
        btSpotBotExport.Size = New Size(73, 26)
        btSpotBotExport.TabIndex = 67
        btSpotBotExport.Text = "Telemetria"
        btSpotBotExport.UseVisualStyleBackColor = False
        ' 
        ' lbSpotBotSimulationSummary
        ' 
        lbSpotBotSimulationSummary.Anchor = AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
        lbSpotBotSimulationSummary.BackColor = Color.FromArgb(CByte(20), CByte(20), CByte(20))
        lbSpotBotSimulationSummary.Font = New Font("Calibri", 9F, FontStyle.Bold, GraphicsUnit.Point, CByte(0))
        lbSpotBotSimulationSummary.ForeColor = Color.Gold
        lbSpotBotSimulationSummary.Location = New Point(0, 202)
        lbSpotBotSimulationSummary.Name = "lbSpotBotSimulationSummary"
        lbSpotBotSimulationSummary.Padding = New Padding(4, 0, 4, 0)
        lbSpotBotSimulationSummary.Size = New Size(1002, 26)
        lbSpotBotSimulationSummary.TabIndex = 69
        lbSpotBotSimulationSummary.Text = "Simulação: aguardando início"
        lbSpotBotSimulationSummary.TextAlign = ContentAlignment.MiddleLeft
        ' 
        ' rtbSpotBotLog
        ' 
        rtbSpotBotLog.BackColor = Color.FromArgb(CByte(40), CByte(40), CByte(40))
        rtbSpotBotLog.BorderStyle = BorderStyle.None
        rtbSpotBotLog.ForeColor = Color.White
        rtbSpotBotLog.Location = New Point(738, 230)
        rtbSpotBotLog.Name = "rtbSpotBotLog"
        rtbSpotBotLog.Size = New Size(413, 125)
        rtbSpotBotLog.TabIndex = 40
        rtbSpotBotLog.Text = ""
        ' 
        ' GroupBox4
        ' 
        GroupBox4.Controls.Add(chkSpotBotRiskUseUnrealized)
        GroupBox4.Controls.Add(chkSpotBotRiskClosePositions)
        GroupBox4.Controls.Add(nudSpotBotRiskPause)
        GroupBox4.Controls.Add(Label44)
        GroupBox4.Controls.Add(nudSpotBotRiskMaxLosses)
        GroupBox4.Controls.Add(Label43)
        GroupBox4.Controls.Add(nudSpotBotRiskGiveback)
        GroupBox4.Controls.Add(Label42)
        GroupBox4.Controls.Add(nudSpotBotRiskProfitTrigger)
        GroupBox4.Controls.Add(Label41)
        GroupBox4.Controls.Add(chkSpotBotRiskProfitLock)
        GroupBox4.Controls.Add(nudSpotBotRiskSessionLoss)
        GroupBox4.Controls.Add(Label40)
        GroupBox4.Controls.Add(chkSpotBotRiskBrakeEnabled)
        GroupBox4.ForeColor = Color.White
        GroupBox4.Location = New Point(770, 5)
        GroupBox4.Name = "GroupBox4"
        GroupBox4.Size = New Size(372, 193)
        GroupBox4.TabIndex = 39
        GroupBox4.TabStop = False
        GroupBox4.Text = "Circuit breaker"
        ' 
        ' chkSpotBotRiskUseUnrealized
        ' 
        chkSpotBotRiskUseUnrealized.AutoSize = True
        chkSpotBotRiskUseUnrealized.Checked = True
        chkSpotBotRiskUseUnrealized.CheckState = CheckState.Checked
        chkSpotBotRiskUseUnrealized.ForeColor = Color.Gold
        chkSpotBotRiskUseUnrealized.Location = New Point(241, 38)
        chkSpotBotRiskUseUnrealized.Name = "chkSpotBotRiskUseUnrealized"
        chkSpotBotRiskUseUnrealized.Size = New Size(104, 19)
        chkSpotBotRiskUseUnrealized.TabIndex = 63
        chkSpotBotRiskUseUnrealized.Text = "Considerar P/L"
        chkSpotBotRiskUseUnrealized.UseVisualStyleBackColor = True
        ' 
        ' chkSpotBotRiskClosePositions
        ' 
        chkSpotBotRiskClosePositions.AutoSize = True
        chkSpotBotRiskClosePositions.Checked = True
        chkSpotBotRiskClosePositions.CheckState = CheckState.Checked
        chkSpotBotRiskClosePositions.ForeColor = Color.Gold
        chkSpotBotRiskClosePositions.Location = New Point(127, 38)
        chkSpotBotRiskClosePositions.Name = "chkSpotBotRiskClosePositions"
        chkSpotBotRiskClosePositions.Size = New Size(111, 19)
        chkSpotBotRiskClosePositions.TabIndex = 62
        chkSpotBotRiskClosePositions.Text = "Fechar no limite"
        chkSpotBotRiskClosePositions.UseVisualStyleBackColor = True
        ' 
        ' nudSpotBotRiskPause
        ' 
        nudSpotBotRiskPause.BackColor = Color.FromArgb(CByte(40), CByte(40), CByte(40))
        nudSpotBotRiskPause.ForeColor = Color.Gold
        nudSpotBotRiskPause.Location = New Point(25, 132)
        nudSpotBotRiskPause.Name = "nudSpotBotRiskPause"
        nudSpotBotRiskPause.Size = New Size(114, 23)
        nudSpotBotRiskPause.TabIndex = 61
        nudSpotBotRiskPause.Value = New Decimal(New Integer() {50, 0, 0, 0})
        ' 
        ' Label44
        ' 
        Label44.AutoSize = True
        Label44.ForeColor = Color.Cyan
        Label44.Location = New Point(22, 113)
        Label44.Name = "Label44"
        Label44.Size = New Size(154, 15)
        Label44.TabIndex = 60
        Label44.Text = "Pausa após sequência (min)"
        ' 
        ' nudSpotBotRiskMaxLosses
        ' 
        nudSpotBotRiskMaxLosses.BackColor = Color.FromArgb(CByte(40), CByte(40), CByte(40))
        nudSpotBotRiskMaxLosses.ForeColor = Color.Gold
        nudSpotBotRiskMaxLosses.Location = New Point(225, 85)
        nudSpotBotRiskMaxLosses.Name = "nudSpotBotRiskMaxLosses"
        nudSpotBotRiskMaxLosses.Size = New Size(114, 23)
        nudSpotBotRiskMaxLosses.TabIndex = 59
        nudSpotBotRiskMaxLosses.Value = New Decimal(New Integer() {3, 0, 0, 0})
        ' 
        ' Label43
        ' 
        Label43.AutoSize = True
        Label43.ForeColor = Color.Cyan
        Label43.Location = New Point(222, 66)
        Label43.Name = "Label43"
        Label43.Size = New Size(119, 15)
        Label43.TabIndex = 58
        Label43.Text = "Máx. perdas seguidas"
        ' 
        ' nudSpotBotRiskGiveback
        ' 
        nudSpotBotRiskGiveback.BackColor = Color.FromArgb(CByte(40), CByte(40), CByte(40))
        nudSpotBotRiskGiveback.ForeColor = Color.Gold
        nudSpotBotRiskGiveback.Location = New Point(105, 85)
        nudSpotBotRiskGiveback.Name = "nudSpotBotRiskGiveback"
        nudSpotBotRiskGiveback.Size = New Size(114, 23)
        nudSpotBotRiskGiveback.TabIndex = 57
        nudSpotBotRiskGiveback.Value = New Decimal(New Integer() {50, 0, 0, 0})
        ' 
        ' Label42
        ' 
        Label42.AutoSize = True
        Label42.ForeColor = Color.Cyan
        Label42.Location = New Point(102, 66)
        Label42.Name = "Label42"
        Label42.Size = New Size(117, 15)
        Label42.TabIndex = 56
        Label42.Text = "Devolver do pico (%)"
        ' 
        ' nudSpotBotRiskProfitTrigger
        ' 
        nudSpotBotRiskProfitTrigger.BackColor = Color.FromArgb(CByte(40), CByte(40), CByte(40))
        nudSpotBotRiskProfitTrigger.DecimalPlaces = 2
        nudSpotBotRiskProfitTrigger.ForeColor = Color.Gold
        nudSpotBotRiskProfitTrigger.Increment = New Decimal(New Integer() {1, 0, 0, 65536})
        nudSpotBotRiskProfitTrigger.Location = New Point(241, 132)
        nudSpotBotRiskProfitTrigger.Name = "nudSpotBotRiskProfitTrigger"
        nudSpotBotRiskProfitTrigger.Size = New Size(77, 23)
        nudSpotBotRiskProfitTrigger.TabIndex = 55
        nudSpotBotRiskProfitTrigger.Value = New Decimal(New Integer() {1, 0, 0, 0})
        ' 
        ' Label41
        ' 
        Label41.AutoSize = True
        Label41.ForeColor = Color.Cyan
        Label41.Location = New Point(238, 113)
        Label41.Name = "Label41"
        Label41.Size = New Size(116, 15)
        Label41.TabIndex = 54
        Label41.Text = "Ativar em ganho.(%)"
        ' 
        ' chkSpotBotRiskProfitLock
        ' 
        chkSpotBotRiskProfitLock.AutoSize = True
        chkSpotBotRiskProfitLock.Checked = True
        chkSpotBotRiskProfitLock.CheckState = CheckState.Checked
        chkSpotBotRiskProfitLock.ForeColor = Color.Gold
        chkSpotBotRiskProfitLock.Location = New Point(145, 133)
        chkSpotBotRiskProfitLock.Name = "chkSpotBotRiskProfitLock"
        chkSpotBotRiskProfitLock.Size = New Size(100, 19)
        chkSpotBotRiskProfitLock.TabIndex = 53
        chkSpotBotRiskProfitLock.Text = "Trava de lucro"
        chkSpotBotRiskProfitLock.UseVisualStyleBackColor = True
        ' 
        ' nudSpotBotRiskSessionLoss
        ' 
        nudSpotBotRiskSessionLoss.BackColor = Color.FromArgb(CByte(40), CByte(40), CByte(40))
        nudSpotBotRiskSessionLoss.DecimalPlaces = 2
        nudSpotBotRiskSessionLoss.ForeColor = Color.Gold
        nudSpotBotRiskSessionLoss.Increment = New Decimal(New Integer() {1, 0, 0, 65536})
        nudSpotBotRiskSessionLoss.Location = New Point(22, 85)
        nudSpotBotRiskSessionLoss.Name = "nudSpotBotRiskSessionLoss"
        nudSpotBotRiskSessionLoss.Size = New Size(77, 23)
        nudSpotBotRiskSessionLoss.TabIndex = 52
        nudSpotBotRiskSessionLoss.Value = New Decimal(New Integer() {2, 0, 0, 0})
        ' 
        ' Label40
        ' 
        Label40.AutoSize = True
        Label40.ForeColor = Color.Cyan
        Label40.Location = New Point(19, 66)
        Label40.Name = "Label40"
        Label40.Size = New Size(83, 15)
        Label40.TabIndex = 51
        Label40.Text = "Perda máx.(%)"
        ' 
        ' chkSpotBotRiskBrakeEnabled
        ' 
        chkSpotBotRiskBrakeEnabled.AutoSize = True
        chkSpotBotRiskBrakeEnabled.Checked = True
        chkSpotBotRiskBrakeEnabled.CheckState = CheckState.Checked
        chkSpotBotRiskBrakeEnabled.ForeColor = Color.Gold
        chkSpotBotRiskBrakeEnabled.Location = New Point(19, 38)
        chkSpotBotRiskBrakeEnabled.Name = "chkSpotBotRiskBrakeEnabled"
        chkSpotBotRiskBrakeEnabled.Size = New Size(102, 19)
        chkSpotBotRiskBrakeEnabled.TabIndex = 44
        chkSpotBotRiskBrakeEnabled.Text = "Proteção geral"
        chkSpotBotRiskBrakeEnabled.UseVisualStyleBackColor = True
        ' 
        ' dgSpotBotPositionsControl
        ' 
        dgSpotBotPositionsControl.AllowUserToAddRows = False
        dgSpotBotPositionsControl.AllowUserToDeleteRows = False
        dgSpotBotPositionsControl.AllowUserToOrderColumns = True
        dgSpotBotPositionsControl.AllowUserToResizeRows = False
        dgSpotBotPositionsControl.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        dgSpotBotPositionsControl.BackgroundColor = Color.FromArgb(CByte(64), CByte(64), CByte(64))
        dgSpotBotPositionsControl.BorderStyle = BorderStyle.None
        dgSpotBotPositionsControl.CellBorderStyle = DataGridViewCellBorderStyle.None
        dgSpotBotPositionsControl.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None
        DataGridViewCellStyle8.Alignment = DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle8.BackColor = Color.FromArgb(CByte(40), CByte(40), CByte(40))
        DataGridViewCellStyle8.Font = New Font("Calibri", 9F, FontStyle.Italic, GraphicsUnit.Point, CByte(0))
        DataGridViewCellStyle8.ForeColor = Color.Turquoise
        DataGridViewCellStyle8.SelectionBackColor = Color.FromArgb(CByte(40), CByte(40), CByte(40))
        DataGridViewCellStyle8.SelectionForeColor = Color.Turquoise
        DataGridViewCellStyle8.WrapMode = DataGridViewTriState.True
        dgSpotBotPositionsControl.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle8
        DataGridViewCellStyle9.Alignment = DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle9.BackColor = Color.FromArgb(CByte(64), CByte(64), CByte(64))
        DataGridViewCellStyle9.Font = New Font("Calibri", 8.25F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        DataGridViewCellStyle9.ForeColor = Color.White
        DataGridViewCellStyle9.SelectionBackColor = Color.Black
        DataGridViewCellStyle9.SelectionForeColor = Color.White
        DataGridViewCellStyle9.WrapMode = DataGridViewTriState.True
        dgSpotBotPositionsControl.DefaultCellStyle = DataGridViewCellStyle9
        dgSpotBotPositionsControl.EnableHeadersVisualStyles = False
        dgSpotBotPositionsControl.Location = New Point(0, 230)
        dgSpotBotPositionsControl.MultiSelect = False
        dgSpotBotPositionsControl.Name = "dgSpotBotPositionsControl"
        dgSpotBotPositionsControl.ReadOnly = True
        DataGridViewCellStyle10.BackColor = Color.Black
        DataGridViewCellStyle10.Font = New Font("Calibri", 12F)
        DataGridViewCellStyle10.ForeColor = Color.White
        DataGridViewCellStyle10.Padding = New Padding(2)
        DataGridViewCellStyle10.SelectionBackColor = Color.Black
        DataGridViewCellStyle10.SelectionForeColor = Color.White
        DataGridViewCellStyle10.WrapMode = DataGridViewTriState.True
        dgSpotBotPositionsControl.RowHeadersDefaultCellStyle = DataGridViewCellStyle10
        dgSpotBotPositionsControl.RowHeadersWidth = 4
        DataGridViewCellStyle11.BackColor = Color.Black
        DataGridViewCellStyle11.ForeColor = Color.White
        DataGridViewCellStyle11.SelectionBackColor = Color.Black
        DataGridViewCellStyle11.SelectionForeColor = Color.White
        dgSpotBotPositionsControl.RowsDefaultCellStyle = DataGridViewCellStyle11
        dgSpotBotPositionsControl.RowTemplate.DefaultCellStyle.SelectionBackColor = Color.Black
        dgSpotBotPositionsControl.RowTemplate.DefaultCellStyle.SelectionForeColor = Color.White
        dgSpotBotPositionsControl.RowTemplate.DefaultCellStyle.WrapMode = DataGridViewTriState.True
        dgSpotBotPositionsControl.ScrollBars = ScrollBars.Vertical
        dgSpotBotPositionsControl.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgSpotBotPositionsControl.Size = New Size(736, 125)
        dgSpotBotPositionsControl.TabIndex = 38
        dgSpotBotPositionsControl.Visible = False
        ' 
        ' GroupBox1
        ' 
        GroupBox1.Controls.Add(chkSpotBotGridRejection)
        GroupBox1.Controls.Add(nudSpotBotGridLevels)
        GroupBox1.Controls.Add(btSpotBotStart)
        GroupBox1.Controls.Add(btSpotBotStop)
        GroupBox1.Controls.Add(Label39)
        GroupBox1.Controls.Add(nudSpotBotGridExposure)
        GroupBox1.Controls.Add(btSpotBotUniverse)
        GroupBox1.Controls.Add(Label38)
        GroupBox1.Controls.Add(nudSpotBotGridReserve)
        GroupBox1.Controls.Add(Label37)
        GroupBox1.Controls.Add(nudSpotBotGridRisk)
        GroupBox1.Controls.Add(Label36)
        GroupBox1.Controls.Add(nudSpotBotMinTrades)
        GroupBox1.Controls.Add(Label20)
        GroupBox1.Controls.Add(nudSpotBotPostExitCooldown)
        GroupBox1.Controls.Add(Label19)
        GroupBox1.Controls.Add(nudSpotBotMinVolume)
        GroupBox1.Controls.Add(Label22)
        GroupBox1.Controls.Add(nudSpotBotMaxAssets)
        GroupBox1.Controls.Add(Label23)
        GroupBox1.Controls.Add(nudSpotBotStopLoss)
        GroupBox1.Controls.Add(Label35)
        GroupBox1.Controls.Add(nudSpotBotTakeProfit)
        GroupBox1.Controls.Add(Label34)
        GroupBox1.Controls.Add(nudSpotBotFee)
        GroupBox1.Controls.Add(Label33)
        GroupBox1.Controls.Add(nudSpotBotAllocation)
        GroupBox1.Controls.Add(Label18)
        GroupBox1.Controls.Add(nudSpotBotStartingQuote)
        GroupBox1.Controls.Add(Label17)
        GroupBox1.Controls.Add(GroupBox3)
        GroupBox1.Controls.Add(GroupBox2)
        GroupBox1.Controls.Add(Label24)
        GroupBox1.Controls.Add(Label21)
        GroupBox1.Controls.Add(Label14)
        GroupBox1.Controls.Add(nudSpotBotConfirmations)
        GroupBox1.Controls.Add(Label13)
        GroupBox1.Controls.Add(cbSpotBotCombination)
        GroupBox1.Controls.Add(cbSpotBotBollingerTimeframe)
        GroupBox1.Controls.Add(cbSpotBotUniverseMode)
        GroupBox1.ForeColor = Color.White
        GroupBox1.Location = New Point(9, 5)
        GroupBox1.Name = "GroupBox1"
        GroupBox1.Size = New Size(755, 193)
        GroupBox1.TabIndex = 0
        GroupBox1.TabStop = False
        GroupBox1.Text = "Configurações"
        ' 
        ' chkSpotBotGridRejection
        ' 
        chkSpotBotGridRejection.AutoSize = True
        chkSpotBotGridRejection.Checked = True
        chkSpotBotGridRejection.CheckState = CheckState.Checked
        chkSpotBotGridRejection.ForeColor = Color.Gold
        chkSpotBotGridRejection.Location = New Point(272, 166)
        chkSpotBotGridRejection.Name = "chkSpotBotGridRejection"
        chkSpotBotGridRejection.Size = New Size(149, 19)
        chkSpotBotGridRejection.TabIndex = 43
        chkSpotBotGridRejection.Text = "Confirmar rejeição Grid"
        chkSpotBotGridRejection.UseVisualStyleBackColor = True
        ' 
        ' nudSpotBotGridLevels
        ' 
        nudSpotBotGridLevels.BackColor = Color.FromArgb(CByte(40), CByte(40), CByte(40))
        nudSpotBotGridLevels.ForeColor = Color.Gold
        nudSpotBotGridLevels.Location = New Point(528, 119)
        nudSpotBotGridLevels.Maximum = New Decimal(New Integer() {500, 0, 0, 0})
        nudSpotBotGridLevels.Name = "nudSpotBotGridLevels"
        nudSpotBotGridLevels.Size = New Size(48, 23)
        nudSpotBotGridLevels.TabIndex = 52
        nudSpotBotGridLevels.Value = New Decimal(New Integer() {5, 0, 0, 0})
        ' 
        ' btSpotBotStart
        ' 
        btSpotBotStart.BackColor = Color.DarkGreen
        btSpotBotStart.Cursor = Cursors.Hand
        btSpotBotStart.FlatAppearance.BorderSize = 0
        btSpotBotStart.FlatAppearance.MouseDownBackColor = Color.DodgerBlue
        btSpotBotStart.FlatAppearance.MouseOverBackColor = Color.RoyalBlue
        btSpotBotStart.FlatStyle = FlatStyle.Flat
        btSpotBotStart.ForeColor = Color.White
        btSpotBotStart.Location = New Point(486, 161)
        btSpotBotStart.Name = "btSpotBotStart"
        btSpotBotStart.Size = New Size(45, 26)
        btSpotBotStart.TabIndex = 65
        btSpotBotStart.Text = "Start"
        btSpotBotStart.UseVisualStyleBackColor = False
        ' 
        ' btSpotBotStop
        ' 
        btSpotBotStop.BackColor = Color.IndianRed
        btSpotBotStop.Cursor = Cursors.Hand
        btSpotBotStop.Enabled = False
        btSpotBotStop.FlatAppearance.BorderSize = 0
        btSpotBotStop.FlatAppearance.MouseDownBackColor = Color.Red
        btSpotBotStop.FlatAppearance.MouseOverBackColor = Color.Red
        btSpotBotStop.FlatStyle = FlatStyle.Flat
        btSpotBotStop.ForeColor = Color.White
        btSpotBotStop.Location = New Point(531, 161)
        btSpotBotStop.Name = "btSpotBotStop"
        btSpotBotStop.Size = New Size(45, 26)
        btSpotBotStop.TabIndex = 66
        btSpotBotStop.Text = "Stop"
        btSpotBotStop.UseVisualStyleBackColor = False
        ' 
        ' Label39
        ' 
        Label39.AutoSize = True
        Label39.ForeColor = Color.Cyan
        Label39.Location = New Point(525, 101)
        Label39.Name = "Label39"
        Label39.Size = New Size(39, 15)
        Label39.TabIndex = 51
        Label39.Text = "Niveis"
        ' 
        ' nudSpotBotGridExposure
        ' 
        nudSpotBotGridExposure.BackColor = Color.FromArgb(CByte(40), CByte(40), CByte(40))
        nudSpotBotGridExposure.DecimalPlaces = 2
        nudSpotBotGridExposure.ForeColor = Color.Gold
        nudSpotBotGridExposure.Increment = New Decimal(New Integer() {1, 0, 0, 65536})
        nudSpotBotGridExposure.Location = New Point(450, 119)
        nudSpotBotGridExposure.Name = "nudSpotBotGridExposure"
        nudSpotBotGridExposure.Size = New Size(77, 23)
        nudSpotBotGridExposure.TabIndex = 50
        nudSpotBotGridExposure.Value = New Decimal(New Integer() {30, 0, 0, 0})
        ' 
        ' btSpotBotUniverse
        ' 
        btSpotBotUniverse.BackColor = Color.DarkOrange
        btSpotBotUniverse.Cursor = Cursors.Hand
        btSpotBotUniverse.FlatAppearance.BorderSize = 0
        btSpotBotUniverse.FlatAppearance.MouseDownBackColor = Color.DodgerBlue
        btSpotBotUniverse.FlatAppearance.MouseOverBackColor = Color.RoyalBlue
        btSpotBotUniverse.FlatStyle = FlatStyle.Flat
        btSpotBotUniverse.ForeColor = Color.White
        btSpotBotUniverse.Location = New Point(421, 161)
        btSpotBotUniverse.Name = "btSpotBotUniverse"
        btSpotBotUniverse.Size = New Size(65, 26)
        btSpotBotUniverse.TabIndex = 64
        btSpotBotUniverse.Text = "Atualizar"
        btSpotBotUniverse.UseVisualStyleBackColor = False
        ' 
        ' Label38
        ' 
        Label38.AutoSize = True
        Label38.ForeColor = Color.Cyan
        Label38.Location = New Point(447, 100)
        Label38.Name = "Label38"
        Label38.Size = New Size(80, 15)
        Label38.TabIndex = 49
        Label38.Text = "Exposição (%)"
        ' 
        ' nudSpotBotGridReserve
        ' 
        nudSpotBotGridReserve.BackColor = Color.FromArgb(CByte(40), CByte(40), CByte(40))
        nudSpotBotGridReserve.DecimalPlaces = 2
        nudSpotBotGridReserve.ForeColor = Color.Gold
        nudSpotBotGridReserve.Increment = New Decimal(New Integer() {1, 0, 0, 65536})
        nudSpotBotGridReserve.Location = New Point(382, 119)
        nudSpotBotGridReserve.Maximum = New Decimal(New Integer() {9999999, 0, 0, 0})
        nudSpotBotGridReserve.Name = "nudSpotBotGridReserve"
        nudSpotBotGridReserve.Size = New Size(67, 23)
        nudSpotBotGridReserve.TabIndex = 48
        nudSpotBotGridReserve.Value = New Decimal(New Integer() {20, 0, 0, 0})
        ' 
        ' Label37
        ' 
        Label37.AutoSize = True
        Label37.ForeColor = Color.Cyan
        Label37.Location = New Point(381, 101)
        Label37.Name = "Label37"
        Label37.Size = New Size(68, 15)
        Label37.TabIndex = 47
        Label37.Text = "Reserva (%)"
        ' 
        ' nudSpotBotGridRisk
        ' 
        nudSpotBotGridRisk.BackColor = Color.FromArgb(CByte(40), CByte(40), CByte(40))
        nudSpotBotGridRisk.DecimalPlaces = 2
        nudSpotBotGridRisk.ForeColor = Color.Gold
        nudSpotBotGridRisk.Increment = New Decimal(New Integer() {1, 0, 0, 65536})
        nudSpotBotGridRisk.Location = New Point(187, 164)
        nudSpotBotGridRisk.Maximum = New Decimal(New Integer() {9999999, 0, 0, 0})
        nudSpotBotGridRisk.Name = "nudSpotBotGridRisk"
        nudSpotBotGridRisk.Size = New Size(80, 23)
        nudSpotBotGridRisk.TabIndex = 46
        nudSpotBotGridRisk.Value = New Decimal(New Integer() {5, 0, 0, 65536})
        ' 
        ' Label36
        ' 
        Label36.AutoSize = True
        Label36.ForeColor = Color.Cyan
        Label36.Location = New Point(184, 146)
        Label36.Name = "Label36"
        Label36.Size = New Size(81, 15)
        Label36.TabIndex = 45
        Label36.Text = "Risco Grid (%)"
        ' 
        ' nudSpotBotMinTrades
        ' 
        nudSpotBotMinTrades.BackColor = Color.FromArgb(CByte(40), CByte(40), CByte(40))
        nudSpotBotMinTrades.ForeColor = Color.Gold
        nudSpotBotMinTrades.Increment = New Decimal(New Integer() {5, 0, 0, 0})
        nudSpotBotMinTrades.Location = New Point(329, 119)
        nudSpotBotMinTrades.Maximum = New Decimal(New Integer() {9999999, 0, 0, 0})
        nudSpotBotMinTrades.Name = "nudSpotBotMinTrades"
        nudSpotBotMinTrades.Size = New Size(52, 23)
        nudSpotBotMinTrades.TabIndex = 44
        nudSpotBotMinTrades.Value = New Decimal(New Integer() {1000, 0, 0, 0})
        ' 
        ' Label20
        ' 
        Label20.AutoSize = True
        Label20.ForeColor = Color.Cyan
        Label20.Location = New Point(329, 101)
        Label20.Name = "Label20"
        Label20.Size = New Size(53, 15)
        Label20.TabIndex = 11
        Label20.Text = "24 Horas"
        ' 
        ' nudSpotBotPostExitCooldown
        ' 
        nudSpotBotPostExitCooldown.BackColor = Color.FromArgb(CByte(40), CByte(40), CByte(40))
        nudSpotBotPostExitCooldown.ForeColor = Color.Gold
        nudSpotBotPostExitCooldown.Increment = New Decimal(New Integer() {5, 0, 0, 0})
        nudSpotBotPostExitCooldown.Location = New Point(273, 119)
        nudSpotBotPostExitCooldown.Maximum = New Decimal(New Integer() {500, 0, 0, 0})
        nudSpotBotPostExitCooldown.Name = "nudSpotBotPostExitCooldown"
        nudSpotBotPostExitCooldown.Size = New Size(55, 23)
        nudSpotBotPostExitCooldown.TabIndex = 43
        nudSpotBotPostExitCooldown.Value = New Decimal(New Integer() {15, 0, 0, 0})
        ' 
        ' Label19
        ' 
        Label19.AutoSize = True
        Label19.ForeColor = Color.Cyan
        Label19.Location = New Point(270, 101)
        Label19.Name = "Label19"
        Label19.Size = New Size(62, 15)
        Label19.TabIndex = 10
        Label19.Text = "Cooldown"
        ' 
        ' nudSpotBotMinVolume
        ' 
        nudSpotBotMinVolume.BackColor = Color.FromArgb(CByte(40), CByte(40), CByte(40))
        nudSpotBotMinVolume.ForeColor = Color.Gold
        nudSpotBotMinVolume.Increment = New Decimal(New Integer() {5, 0, 0, 0})
        nudSpotBotMinVolume.Location = New Point(187, 119)
        nudSpotBotMinVolume.Maximum = New Decimal(New Integer() {999999999, 0, 0, 0})
        nudSpotBotMinVolume.Name = "nudSpotBotMinVolume"
        nudSpotBotMinVolume.Size = New Size(85, 23)
        nudSpotBotMinVolume.TabIndex = 42
        nudSpotBotMinVolume.Value = New Decimal(New Integer() {1000000, 0, 0, 0})
        ' 
        ' Label22
        ' 
        Label22.AutoSize = True
        Label22.ForeColor = Color.Cyan
        Label22.Location = New Point(184, 101)
        Label22.Name = "Label22"
        Label22.Size = New Size(71, 15)
        Label22.TabIndex = 13
        Label22.Text = "Volume min"
        ' 
        ' nudSpotBotMaxAssets
        ' 
        nudSpotBotMaxAssets.BackColor = Color.FromArgb(CByte(40), CByte(40), CByte(40))
        nudSpotBotMaxAssets.ForeColor = Color.Gold
        nudSpotBotMaxAssets.Increment = New Decimal(New Integer() {5, 0, 0, 0})
        nudSpotBotMaxAssets.Location = New Point(187, 75)
        nudSpotBotMaxAssets.Maximum = New Decimal(New Integer() {500, 0, 0, 0})
        nudSpotBotMaxAssets.Name = "nudSpotBotMaxAssets"
        nudSpotBotMaxAssets.Size = New Size(68, 23)
        nudSpotBotMaxAssets.TabIndex = 41
        nudSpotBotMaxAssets.Value = New Decimal(New Integer() {200, 0, 0, 0})
        ' 
        ' Label23
        ' 
        Label23.AutoSize = True
        Label23.ForeColor = Color.Cyan
        Label23.Location = New Point(184, 57)
        Label23.Name = "Label23"
        Label23.Size = New Size(63, 15)
        Label23.TabIndex = 14
        Label23.Text = "Máx ativos"
        ' 
        ' nudSpotBotStopLoss
        ' 
        nudSpotBotStopLoss.BackColor = Color.FromArgb(CByte(40), CByte(40), CByte(40))
        nudSpotBotStopLoss.DecimalPlaces = 2
        nudSpotBotStopLoss.ForeColor = Color.Gold
        nudSpotBotStopLoss.Increment = New Decimal(New Integer() {1, 0, 0, 65536})
        nudSpotBotStopLoss.Location = New Point(522, 75)
        nudSpotBotStopLoss.Maximum = New Decimal(New Integer() {9999999, 0, 0, 0})
        nudSpotBotStopLoss.Name = "nudSpotBotStopLoss"
        nudSpotBotStopLoss.Size = New Size(54, 23)
        nudSpotBotStopLoss.TabIndex = 40
        nudSpotBotStopLoss.Value = New Decimal(New Integer() {1, 0, 0, 0})
        ' 
        ' Label35
        ' 
        Label35.AutoSize = True
        Label35.ForeColor = Color.Cyan
        Label35.Location = New Point(519, 57)
        Label35.Name = "Label35"
        Label35.Size = New Size(40, 15)
        Label35.TabIndex = 39
        Label35.Text = "SL (%)"
        ' 
        ' nudSpotBotTakeProfit
        ' 
        nudSpotBotTakeProfit.BackColor = Color.FromArgb(CByte(40), CByte(40), CByte(40))
        nudSpotBotTakeProfit.DecimalPlaces = 2
        nudSpotBotTakeProfit.ForeColor = Color.Gold
        nudSpotBotTakeProfit.Increment = New Decimal(New Integer() {1, 0, 0, 65536})
        nudSpotBotTakeProfit.Location = New Point(471, 75)
        nudSpotBotTakeProfit.Maximum = New Decimal(New Integer() {9999999, 0, 0, 0})
        nudSpotBotTakeProfit.Name = "nudSpotBotTakeProfit"
        nudSpotBotTakeProfit.Size = New Size(50, 23)
        nudSpotBotTakeProfit.TabIndex = 38
        nudSpotBotTakeProfit.Value = New Decimal(New Integer() {2, 0, 0, 0})
        ' 
        ' Label34
        ' 
        Label34.AutoSize = True
        Label34.ForeColor = Color.Cyan
        Label34.Location = New Point(468, 57)
        Label34.Name = "Label34"
        Label34.Size = New Size(42, 15)
        Label34.TabIndex = 37
        Label34.Text = "TP (%)"
        ' 
        ' nudSpotBotFee
        ' 
        nudSpotBotFee.BackColor = Color.FromArgb(CByte(40), CByte(40), CByte(40))
        nudSpotBotFee.DecimalPlaces = 2
        nudSpotBotFee.ForeColor = Color.Gold
        nudSpotBotFee.Increment = New Decimal(New Integer() {1, 0, 0, 65536})
        nudSpotBotFee.Location = New Point(409, 75)
        nudSpotBotFee.Maximum = New Decimal(New Integer() {9999999, 0, 0, 0})
        nudSpotBotFee.Name = "nudSpotBotFee"
        nudSpotBotFee.Size = New Size(61, 23)
        nudSpotBotFee.TabIndex = 36
        nudSpotBotFee.Value = New Decimal(New Integer() {1, 0, 0, 65536})
        ' 
        ' Label33
        ' 
        Label33.AutoSize = True
        Label33.ForeColor = Color.Cyan
        Label33.Location = New Point(406, 57)
        Label33.Name = "Label33"
        Label33.Size = New Size(51, 15)
        Label33.TabIndex = 35
        Label33.Text = "Taxa (%)"
        ' 
        ' nudSpotBotAllocation
        ' 
        nudSpotBotAllocation.BackColor = Color.FromArgb(CByte(40), CByte(40), CByte(40))
        nudSpotBotAllocation.DecimalPlaces = 2
        nudSpotBotAllocation.ForeColor = Color.Gold
        nudSpotBotAllocation.Increment = New Decimal(New Integer() {1, 0, 0, 65536})
        nudSpotBotAllocation.Location = New Point(341, 75)
        nudSpotBotAllocation.Maximum = New Decimal(New Integer() {9999999, 0, 0, 0})
        nudSpotBotAllocation.Name = "nudSpotBotAllocation"
        nudSpotBotAllocation.Size = New Size(67, 23)
        nudSpotBotAllocation.TabIndex = 34
        nudSpotBotAllocation.Value = New Decimal(New Integer() {10, 0, 0, 0})
        ' 
        ' Label18
        ' 
        Label18.AutoSize = True
        Label18.ForeColor = Color.Cyan
        Label18.Location = New Point(338, 57)
        Label18.Name = "Label18"
        Label18.Size = New Size(56, 15)
        Label18.TabIndex = 9
        Label18.Text = "Ativo (%)"
        ' 
        ' nudSpotBotStartingQuote
        ' 
        nudSpotBotStartingQuote.BackColor = Color.FromArgb(CByte(40), CByte(40), CByte(40))
        nudSpotBotStartingQuote.DecimalPlaces = 2
        nudSpotBotStartingQuote.ForeColor = Color.Gold
        nudSpotBotStartingQuote.Increment = New Decimal(New Integer() {1, 0, 0, 65536})
        nudSpotBotStartingQuote.Location = New Point(256, 75)
        nudSpotBotStartingQuote.Maximum = New Decimal(New Integer() {9999999, 0, 0, 0})
        nudSpotBotStartingQuote.Name = "nudSpotBotStartingQuote"
        nudSpotBotStartingQuote.Size = New Size(84, 23)
        nudSpotBotStartingQuote.TabIndex = 33
        nudSpotBotStartingQuote.Value = New Decimal(New Integer() {18, 0, 0, 65536})
        ' 
        ' Label17
        ' 
        Label17.AutoSize = True
        Label17.ForeColor = Color.Cyan
        Label17.Location = New Point(255, 57)
        Label17.Name = "Label17"
        Label17.Size = New Size(36, 15)
        Label17.TabIndex = 8
        Label17.Text = "Saldo"
        ' 
        ' GroupBox3
        ' 
        GroupBox3.Controls.Add(Label31)
        GroupBox3.Controls.Add(Label32)
        GroupBox3.Controls.Add(nudSpotBotBtcFlushVolume)
        GroupBox3.Controls.Add(Label30)
        GroupBox3.Controls.Add(Label29)
        GroupBox3.Controls.Add(nudSpotBotBtcDailyDrawdown)
        GroupBox3.Controls.Add(Label28)
        GroupBox3.Controls.Add(nudSpotBotBtcDailyDrop)
        GroupBox3.Controls.Add(chkSpotBotBtcDaily)
        GroupBox3.Controls.Add(Label27)
        GroupBox3.Controls.Add(nudSpotBotBtcFlushPause)
        GroupBox3.Controls.Add(Label26)
        GroupBox3.Controls.Add(nudSpotBotBtcFlushDrop)
        GroupBox3.Controls.Add(chkSpotBotBtcFlush)
        GroupBox3.Controls.Add(Label25)
        GroupBox3.Controls.Add(Label12)
        GroupBox3.Controls.Add(NumericUpDown2)
        GroupBox3.Controls.Add(NumericUpDown1)
        GroupBox3.Controls.Add(chkSpotBotBreakEven)
        GroupBox3.Controls.Add(chkSpotBotTrailing)
        GroupBox3.ForeColor = Color.Cyan
        GroupBox3.Location = New Point(586, 12)
        GroupBox3.Name = "GroupBox3"
        GroupBox3.Size = New Size(169, 181)
        GroupBox3.TabIndex = 17
        GroupBox3.TabStop = False
        GroupBox3.Text = "Proteções"
        ' 
        ' Label31
        ' 
        Label31.AutoSize = True
        Label31.ForeColor = Color.Gold
        Label31.Location = New Point(143, 114)
        Label31.Name = "Label31"
        Label31.Size = New Size(17, 15)
        Label31.TabIndex = 33
        Label31.Text = "%"
        ' 
        ' Label32
        ' 
        Label32.AutoSize = True
        Label32.ForeColor = Color.Gold
        Label32.Location = New Point(26, 113)
        Label32.Name = "Label32"
        Label32.Size = New Size(57, 15)
        Label32.TabIndex = 32
        Label32.Text = "Volume X"
        ' 
        ' nudSpotBotBtcFlushVolume
        ' 
        nudSpotBotBtcFlushVolume.BackColor = Color.FromArgb(CByte(40), CByte(40), CByte(40))
        nudSpotBotBtcFlushVolume.DecimalPlaces = 2
        nudSpotBotBtcFlushVolume.ForeColor = Color.Gold
        nudSpotBotBtcFlushVolume.Increment = New Decimal(New Integer() {1, 0, 0, 65536})
        nudSpotBotBtcFlushVolume.Location = New Point(86, 110)
        nudSpotBotBtcFlushVolume.Name = "nudSpotBotBtcFlushVolume"
        nudSpotBotBtcFlushVolume.Size = New Size(55, 23)
        nudSpotBotBtcFlushVolume.TabIndex = 31
        nudSpotBotBtcFlushVolume.Value = New Decimal(New Integer() {18, 0, 0, 65536})
        ' 
        ' Label30
        ' 
        Label30.AutoSize = True
        Label30.ForeColor = Color.Gold
        Label30.Location = New Point(143, 162)
        Label30.Name = "Label30"
        Label30.Size = New Size(17, 15)
        Label30.TabIndex = 30
        Label30.Text = "%"
        ' 
        ' Label29
        ' 
        Label29.AutoSize = True
        Label29.ForeColor = Color.Gold
        Label29.Location = New Point(3, 161)
        Label29.Name = "Label29"
        Label29.Size = New Size(90, 15)
        Label29.TabIndex = 29
        Label29.Text = "Oscilação diária"
        ' 
        ' nudSpotBotBtcDailyDrawdown
        ' 
        nudSpotBotBtcDailyDrawdown.BackColor = Color.FromArgb(CByte(40), CByte(40), CByte(40))
        nudSpotBotBtcDailyDrawdown.DecimalPlaces = 2
        nudSpotBotBtcDailyDrawdown.ForeColor = Color.Gold
        nudSpotBotBtcDailyDrawdown.Increment = New Decimal(New Integer() {1, 0, 0, 65536})
        nudSpotBotBtcDailyDrawdown.Location = New Point(96, 158)
        nudSpotBotBtcDailyDrawdown.Name = "nudSpotBotBtcDailyDrawdown"
        nudSpotBotBtcDailyDrawdown.Size = New Size(45, 23)
        nudSpotBotBtcDailyDrawdown.TabIndex = 28
        nudSpotBotBtcDailyDrawdown.Value = New Decimal(New Integer() {5, 0, 0, 0})
        ' 
        ' Label28
        ' 
        Label28.AutoSize = True
        Label28.ForeColor = Color.Gold
        Label28.Location = New Point(144, 137)
        Label28.Name = "Label28"
        Label28.Size = New Size(17, 15)
        Label28.TabIndex = 27
        Label28.Text = "%"
        ' 
        ' nudSpotBotBtcDailyDrop
        ' 
        nudSpotBotBtcDailyDrop.BackColor = Color.FromArgb(CByte(40), CByte(40), CByte(40))
        nudSpotBotBtcDailyDrop.DecimalPlaces = 2
        nudSpotBotBtcDailyDrop.ForeColor = Color.Gold
        nudSpotBotBtcDailyDrop.Increment = New Decimal(New Integer() {1, 0, 0, 65536})
        nudSpotBotBtcDailyDrop.Location = New Point(96, 134)
        nudSpotBotBtcDailyDrop.Name = "nudSpotBotBtcDailyDrop"
        nudSpotBotBtcDailyDrop.Size = New Size(45, 23)
        nudSpotBotBtcDailyDrop.TabIndex = 26
        nudSpotBotBtcDailyDrop.Value = New Decimal(New Integer() {4, 0, 0, 0})
        ' 
        ' chkSpotBotBtcDaily
        ' 
        chkSpotBotBtcDaily.AutoSize = True
        chkSpotBotBtcDaily.Checked = True
        chkSpotBotBtcDaily.CheckState = CheckState.Checked
        chkSpotBotBtcDaily.ForeColor = Color.Gold
        chkSpotBotBtcDaily.Location = New Point(5, 135)
        chkSpotBotBtcDaily.Name = "chkSpotBotBtcDaily"
        chkSpotBotBtcDaily.Size = New Size(94, 19)
        chkSpotBotBtcDaily.TabIndex = 25
        chkSpotBotBtcDaily.Text = "Queda diário"
        chkSpotBotBtcDaily.UseVisualStyleBackColor = True
        ' 
        ' Label27
        ' 
        Label27.AutoSize = True
        Label27.ForeColor = Color.Gold
        Label27.Location = New Point(13, 90)
        Label27.Name = "Label27"
        Label27.Size = New Size(70, 15)
        Label27.TabIndex = 24
        Label27.Text = "Pausa (min)"
        ' 
        ' nudSpotBotBtcFlushPause
        ' 
        nudSpotBotBtcFlushPause.BackColor = Color.FromArgb(CByte(40), CByte(40), CByte(40))
        nudSpotBotBtcFlushPause.Enabled = False
        nudSpotBotBtcFlushPause.ForeColor = Color.Gold
        nudSpotBotBtcFlushPause.Increment = New Decimal(New Integer() {10, 0, 0, 0})
        nudSpotBotBtcFlushPause.Location = New Point(86, 87)
        nudSpotBotBtcFlushPause.Name = "nudSpotBotBtcFlushPause"
        nudSpotBotBtcFlushPause.Size = New Size(55, 23)
        nudSpotBotBtcFlushPause.TabIndex = 23
        nudSpotBotBtcFlushPause.Value = New Decimal(New Integer() {30, 0, 0, 0})
        ' 
        ' Label26
        ' 
        Label26.AutoSize = True
        Label26.ForeColor = Color.Gold
        Label26.Location = New Point(144, 66)
        Label26.Name = "Label26"
        Label26.Size = New Size(17, 15)
        Label26.TabIndex = 22
        Label26.Text = "%"
        ' 
        ' nudSpotBotBtcFlushDrop
        ' 
        nudSpotBotBtcFlushDrop.BackColor = Color.FromArgb(CByte(40), CByte(40), CByte(40))
        nudSpotBotBtcFlushDrop.DecimalPlaces = 2
        nudSpotBotBtcFlushDrop.ForeColor = Color.Gold
        nudSpotBotBtcFlushDrop.Increment = New Decimal(New Integer() {1, 0, 0, 65536})
        nudSpotBotBtcFlushDrop.Location = New Point(86, 63)
        nudSpotBotBtcFlushDrop.Name = "nudSpotBotBtcFlushDrop"
        nudSpotBotBtcFlushDrop.Size = New Size(55, 23)
        nudSpotBotBtcFlushDrop.TabIndex = 21
        nudSpotBotBtcFlushDrop.Value = New Decimal(New Integer() {2, 0, 0, 0})
        ' 
        ' chkSpotBotBtcFlush
        ' 
        chkSpotBotBtcFlush.AutoSize = True
        chkSpotBotBtcFlush.Checked = True
        chkSpotBotBtcFlush.CheckState = CheckState.Checked
        chkSpotBotBtcFlush.ForeColor = Color.Gold
        chkSpotBotBtcFlush.Location = New Point(6, 65)
        chkSpotBotBtcFlush.Name = "chkSpotBotBtcFlush"
        chkSpotBotBtcFlush.Size = New Size(77, 19)
        chkSpotBotBtcFlush.TabIndex = 20
        chkSpotBotBtcFlush.Text = "Flush BTC"
        chkSpotBotBtcFlush.UseVisualStyleBackColor = True
        ' 
        ' Label25
        ' 
        Label25.AutoSize = True
        Label25.ForeColor = Color.Gold
        Label25.Location = New Point(144, 42)
        Label25.Name = "Label25"
        Label25.Size = New Size(17, 15)
        Label25.TabIndex = 19
        Label25.Text = "%"
        ' 
        ' Label12
        ' 
        Label12.AutoSize = True
        Label12.ForeColor = Color.Gold
        Label12.Location = New Point(144, 18)
        Label12.Name = "Label12"
        Label12.Size = New Size(17, 15)
        Label12.TabIndex = 18
        Label12.Text = "%"
        ' 
        ' NumericUpDown2
        ' 
        NumericUpDown2.BackColor = Color.FromArgb(CByte(40), CByte(40), CByte(40))
        NumericUpDown2.DecimalPlaces = 2
        NumericUpDown2.ForeColor = Color.Gold
        NumericUpDown2.Increment = New Decimal(New Integer() {1, 0, 0, 65536})
        NumericUpDown2.Location = New Point(86, 15)
        NumericUpDown2.Name = "NumericUpDown2"
        NumericUpDown2.Size = New Size(55, 23)
        NumericUpDown2.TabIndex = 8
        NumericUpDown2.Value = New Decimal(New Integer() {15, 0, 0, 65536})
        ' 
        ' NumericUpDown1
        ' 
        NumericUpDown1.BackColor = Color.FromArgb(CByte(40), CByte(40), CByte(40))
        NumericUpDown1.DecimalPlaces = 2
        NumericUpDown1.ForeColor = Color.Gold
        NumericUpDown1.Increment = New Decimal(New Integer() {1, 0, 0, 65536})
        NumericUpDown1.Location = New Point(86, 39)
        NumericUpDown1.Name = "NumericUpDown1"
        NumericUpDown1.Size = New Size(55, 23)
        NumericUpDown1.TabIndex = 7
        NumericUpDown1.Value = New Decimal(New Integer() {1, 0, 0, 0})
        ' 
        ' chkSpotBotBreakEven
        ' 
        chkSpotBotBreakEven.AutoSize = True
        chkSpotBotBreakEven.Checked = True
        chkSpotBotBreakEven.CheckState = CheckState.Checked
        chkSpotBotBreakEven.ForeColor = Color.Gold
        chkSpotBotBreakEven.Location = New Point(6, 18)
        chkSpotBotBreakEven.Name = "chkSpotBotBreakEven"
        chkSpotBotBreakEven.Size = New Size(83, 19)
        chkSpotBotBreakEven.TabIndex = 4
        chkSpotBotBreakEven.Text = "Break Even"
        chkSpotBotBreakEven.UseVisualStyleBackColor = True
        ' 
        ' chkSpotBotTrailing
        ' 
        chkSpotBotTrailing.AutoSize = True
        chkSpotBotTrailing.Checked = True
        chkSpotBotTrailing.CheckState = CheckState.Checked
        chkSpotBotTrailing.ForeColor = Color.Gold
        chkSpotBotTrailing.Location = New Point(6, 41)
        chkSpotBotTrailing.Name = "chkSpotBotTrailing"
        chkSpotBotTrailing.Size = New Size(48, 19)
        chkSpotBotTrailing.TabIndex = 3
        chkSpotBotTrailing.Text = "Trail"
        chkSpotBotTrailing.UseVisualStyleBackColor = True
        ' 
        ' GroupBox2
        ' 
        GroupBox2.Controls.Add(chkSpotBotValueZonePullback)
        GroupBox2.Controls.Add(chkSpotBotBtcMildDecline)
        GroupBox2.Controls.Add(chkSpotBotGridAdaptive)
        GroupBox2.Controls.Add(chkSpotBotMomentumVolume)
        GroupBox2.Controls.Add(chkSpotBotBreakoutRetest)
        GroupBox2.Controls.Add(chkSpotBotWavePullback)
        GroupBox2.Controls.Add(cbSpotBotBollingerMode)
        GroupBox2.ForeColor = Color.Aqua
        GroupBox2.Location = New Point(0, 22)
        GroupBox2.Name = "GroupBox2"
        GroupBox2.Size = New Size(178, 171)
        GroupBox2.TabIndex = 16
        GroupBox2.TabStop = False
        GroupBox2.Text = "Estratégias"
        ' 
        ' chkSpotBotValueZonePullback
        ' 
        chkSpotBotValueZonePullback.AutoSize = True
        chkSpotBotValueZonePullback.ForeColor = Color.Gold
        chkSpotBotValueZonePullback.Location = New Point(8, 132)
        chkSpotBotValueZonePullback.Name = "chkSpotBotValueZonePullback"
        chkSpotBotValueZonePullback.Size = New Size(144, 19)
        chkSpotBotValueZonePullback.TabIndex = 6
        chkSpotBotValueZonePullback.Text = "Pullback zona de valor"
        chkSpotBotValueZonePullback.UseVisualStyleBackColor = True
        ' 
        ' chkSpotBotBtcMildDecline
        ' 
        chkSpotBotBtcMildDecline.AutoSize = True
        chkSpotBotBtcMildDecline.ForeColor = Color.Gold
        chkSpotBotBtcMildDecline.Location = New Point(8, 150)
        chkSpotBotBtcMildDecline.Name = "chkSpotBotBtcMildDecline"
        chkSpotBotBtcMildDecline.Size = New Size(144, 19)
        chkSpotBotBtcMildDecline.TabIndex = 7
        chkSpotBotBtcMildDecline.Text = "BTC queda leve"
        chkSpotBotBtcMildDecline.UseVisualStyleBackColor = True
        ' 
        ' chkSpotBotGridAdaptive
        ' 
        chkSpotBotGridAdaptive.AutoSize = True
        chkSpotBotGridAdaptive.ForeColor = Color.Gold
        chkSpotBotGridAdaptive.Location = New Point(8, 114)
        chkSpotBotGridAdaptive.Name = "chkSpotBotGridAdaptive"
        chkSpotBotGridAdaptive.Size = New Size(48, 19)
        chkSpotBotGridAdaptive.TabIndex = 5
        chkSpotBotGridAdaptive.Text = "Grid"
        chkSpotBotGridAdaptive.UseVisualStyleBackColor = True
        ' 
        ' chkSpotBotMomentumVolume
        ' 
        chkSpotBotMomentumVolume.AutoSize = True
        chkSpotBotMomentumVolume.ForeColor = Color.Gold
        chkSpotBotMomentumVolume.Location = New Point(8, 95)
        chkSpotBotMomentumVolume.Name = "chkSpotBotMomentumVolume"
        chkSpotBotMomentumVolume.Size = New Size(90, 19)
        chkSpotBotMomentumVolume.TabIndex = 4
        chkSpotBotMomentumVolume.Text = "Momentum"
        chkSpotBotMomentumVolume.UseVisualStyleBackColor = True
        ' 
        ' chkSpotBotBreakoutRetest
        ' 
        chkSpotBotBreakoutRetest.AutoSize = True
        chkSpotBotBreakoutRetest.ForeColor = Color.Gold
        chkSpotBotBreakoutRetest.Location = New Point(8, 77)
        chkSpotBotBreakoutRetest.Name = "chkSpotBotBreakoutRetest"
        chkSpotBotBreakoutRetest.Size = New Size(125, 19)
        chkSpotBotBreakoutRetest.TabIndex = 3
        chkSpotBotBreakoutRetest.Text = "Breakout + Reteste"
        chkSpotBotBreakoutRetest.UseVisualStyleBackColor = True
        ' 
        ' chkSpotBotWavePullback
        ' 
        chkSpotBotWavePullback.AutoSize = True
        chkSpotBotWavePullback.ForeColor = Color.Gold
        chkSpotBotWavePullback.Location = New Point(8, 59)
        chkSpotBotWavePullback.Name = "chkSpotBotWavePullback"
        chkSpotBotWavePullback.Size = New Size(71, 19)
        chkSpotBotWavePullback.TabIndex = 2
        chkSpotBotWavePullback.Text = "Pullback"
        chkSpotBotWavePullback.UseVisualStyleBackColor = True
        ' 
        ' cbSpotBotBollingerMode
        ' 
        cbSpotBotBollingerMode.BackColor = Color.FromArgb(CByte(40), CByte(40), CByte(40))
        cbSpotBotBollingerMode.ForeColor = Color.Gold
        cbSpotBotBollingerMode.FormattingEnabled = True
        cbSpotBotBollingerMode.Location = New Point(8, 25)
        cbSpotBotBollingerMode.Name = "cbSpotBotBollingerMode"
        cbSpotBotBollingerMode.Size = New Size(161, 23)
        cbSpotBotBollingerMode.TabIndex = 1
        ' 
        ' Label24
        ' 
        Label24.AutoSize = True
        Label24.ForeColor = Color.Cyan
        Label24.Location = New Point(385, 13)
        Label24.Name = "Label24"
        Label24.Size = New Size(44, 15)
        Label24.TabIndex = 15
        Label24.Text = "Candle"
        ' 
        ' Label21
        ' 
        Label21.AutoSize = True
        Label21.ForeColor = Color.Cyan
        Label21.Location = New Point(469, 13)
        Label21.Name = "Label21"
        Label21.Size = New Size(47, 15)
        Label21.TabIndex = 12
        Label21.Text = "Origem"
        ' 
        ' Label14
        ' 
        Label14.AutoSize = True
        Label14.ForeColor = Color.Cyan
        Label14.Location = New Point(307, 13)
        Label14.Name = "Label14"
        Label14.Size = New Size(81, 15)
        Label14.TabIndex = 7
        Label14.Text = "Confirmações"
        ' 
        ' nudSpotBotConfirmations
        ' 
        nudSpotBotConfirmations.BackColor = Color.FromArgb(CByte(40), CByte(40), CByte(40))
        nudSpotBotConfirmations.ForeColor = Color.Gold
        nudSpotBotConfirmations.Location = New Point(346, 32)
        nudSpotBotConfirmations.Name = "nudSpotBotConfirmations"
        nudSpotBotConfirmations.Size = New Size(39, 23)
        nudSpotBotConfirmations.TabIndex = 6
        nudSpotBotConfirmations.Value = New Decimal(New Integer() {1, 0, 0, 0})
        ' 
        ' Label13
        ' 
        Label13.AutoSize = True
        Label13.ForeColor = Color.Cyan
        Label13.Location = New Point(186, 13)
        Label13.Name = "Label13"
        Label13.Size = New Size(53, 15)
        Label13.TabIndex = 5
        Label13.Text = "Ativação"
        ' 
        ' cbSpotBotCombination
        ' 
        cbSpotBotCombination.BackColor = Color.FromArgb(CByte(40), CByte(40), CByte(40))
        cbSpotBotCombination.ForeColor = Color.Gold
        cbSpotBotCombination.FormattingEnabled = True
        cbSpotBotCombination.Location = New Point(186, 31)
        cbSpotBotCombination.Name = "cbSpotBotCombination"
        cbSpotBotCombination.Size = New Size(159, 23)
        cbSpotBotCombination.TabIndex = 4
        ' 
        ' cbSpotBotBollingerTimeframe
        ' 
        cbSpotBotBollingerTimeframe.BackColor = Color.FromArgb(CByte(40), CByte(40), CByte(40))
        cbSpotBotBollingerTimeframe.ForeColor = Color.Gold
        cbSpotBotBollingerTimeframe.FormattingEnabled = True
        cbSpotBotBollingerTimeframe.Location = New Point(386, 32)
        cbSpotBotBollingerTimeframe.Name = "cbSpotBotBollingerTimeframe"
        cbSpotBotBollingerTimeframe.Size = New Size(84, 23)
        cbSpotBotBollingerTimeframe.TabIndex = 3
        ' 
        ' cbSpotBotUniverseMode
        ' 
        cbSpotBotUniverseMode.BackColor = Color.FromArgb(CByte(40), CByte(40), CByte(40))
        cbSpotBotUniverseMode.ForeColor = Color.Gold
        cbSpotBotUniverseMode.FormattingEnabled = True
        cbSpotBotUniverseMode.Location = New Point(471, 32)
        cbSpotBotUniverseMode.Name = "cbSpotBotUniverseMode"
        cbSpotBotUniverseMode.Size = New Size(105, 23)
        cbSpotBotUniverseMode.TabIndex = 2
        ' 
        ' FormMain
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        BackColor = Color.FromArgb(CByte(40), CByte(40), CByte(40))
        ClientSize = New Size(1151, 810)
        Controls.Add(PanelTradeBot)
        Controls.Add(btTradebot)
        Controls.Add(pbBRL)
        Controls.Add(pbUSD)
        Controls.Add(Label8)
        Controls.Add(btSpot)
        Controls.Add(PanelGraphs)
        Controls.Add(PanelFuturos)
        Controls.Add(btOrdensFuturo)
        Controls.Add(btRefresh)
        Controls.Add(PanelOrdens)
        Controls.Add(btOrdensSpot)
        Controls.Add(btVisaoGeral)
        Controls.Add(btFuturos)
        Controls.Add(dgFuturos)
        Controls.Add(dgPortfolio)
        Controls.Add(panelDebug)
        Controls.Add(PanelProfits)
        Controls.Add(Panel1)
        Controls.Add(MenuStrip1)
        Controls.Add(Panel2)
        Controls.Add(lbLoadFromMarket)
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
        PanelGraphs.ResumeLayout(False)
        PanelGraphs.PerformLayout()
        CType(dgFuturos, ComponentModel.ISupportInitialize).EndInit()
        MenuStrip1.ResumeLayout(False)
        MenuStrip1.PerformLayout()
        CType(pbBRL, ComponentModel.ISupportInitialize).EndInit()
        CType(pbUSD, ComponentModel.ISupportInitialize).EndInit()
        panelDebug.ResumeLayout(False)
        PanelTradeBot.ResumeLayout(False)
        GroupBox4.ResumeLayout(False)
        GroupBox4.PerformLayout()
        CType(nudSpotBotRiskPause, ComponentModel.ISupportInitialize).EndInit()
        CType(nudSpotBotRiskMaxLosses, ComponentModel.ISupportInitialize).EndInit()
        CType(nudSpotBotRiskGiveback, ComponentModel.ISupportInitialize).EndInit()
        CType(nudSpotBotRiskProfitTrigger, ComponentModel.ISupportInitialize).EndInit()
        CType(nudSpotBotRiskSessionLoss, ComponentModel.ISupportInitialize).EndInit()
        CType(dgSpotBotPositionsControl, ComponentModel.ISupportInitialize).EndInit()
        GroupBox1.ResumeLayout(False)
        GroupBox1.PerformLayout()
        CType(nudSpotBotGridLevels, ComponentModel.ISupportInitialize).EndInit()
        CType(nudSpotBotGridExposure, ComponentModel.ISupportInitialize).EndInit()
        CType(nudSpotBotGridReserve, ComponentModel.ISupportInitialize).EndInit()
        CType(nudSpotBotGridRisk, ComponentModel.ISupportInitialize).EndInit()
        CType(nudSpotBotMinTrades, ComponentModel.ISupportInitialize).EndInit()
        CType(nudSpotBotPostExitCooldown, ComponentModel.ISupportInitialize).EndInit()
        CType(nudSpotBotMinVolume, ComponentModel.ISupportInitialize).EndInit()
        CType(nudSpotBotMaxAssets, ComponentModel.ISupportInitialize).EndInit()
        CType(nudSpotBotStopLoss, ComponentModel.ISupportInitialize).EndInit()
        CType(nudSpotBotTakeProfit, ComponentModel.ISupportInitialize).EndInit()
        CType(nudSpotBotFee, ComponentModel.ISupportInitialize).EndInit()
        CType(nudSpotBotAllocation, ComponentModel.ISupportInitialize).EndInit()
        CType(nudSpotBotStartingQuote, ComponentModel.ISupportInitialize).EndInit()
        GroupBox3.ResumeLayout(False)
        GroupBox3.PerformLayout()
        CType(nudSpotBotBtcFlushVolume, ComponentModel.ISupportInitialize).EndInit()
        CType(nudSpotBotBtcDailyDrawdown, ComponentModel.ISupportInitialize).EndInit()
        CType(nudSpotBotBtcDailyDrop, ComponentModel.ISupportInitialize).EndInit()
        CType(nudSpotBotBtcFlushPause, ComponentModel.ISupportInitialize).EndInit()
        CType(nudSpotBotBtcFlushDrop, ComponentModel.ISupportInitialize).EndInit()
        CType(NumericUpDown2, ComponentModel.ISupportInitialize).EndInit()
        CType(NumericUpDown1, ComponentModel.ISupportInitialize).EndInit()
        GroupBox2.ResumeLayout(False)
        GroupBox2.PerformLayout()
        CType(nudSpotBotConfirmations, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)
        PerformLayout()
    End Sub
    Friend WithEvents dgPortfolio As DataGridView
    Friend WithEvents PanelProfits As Panel
    Friend WithEvents Label5 As Label
    Friend WithEvents lbRoiSeparator As Label
    Friend WithEvents lbValoresSeparator As Label
    Friend WithEvents lbTotalBRL As Label
    Friend WithEvents LabelFuturos As Label
    Friend WithEvents lbFuturosUSD As Label
    Friend WithEvents lbFuturosBRL As Label
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
    Friend WithEvents PictureBox2 As PictureBox
    Friend WithEvents PictureBox1 As PictureBox
    Friend WithEvents Label16 As Label
    Friend WithEvents lbPerformWallet As Label
    Friend WithEvents Label6 As Label
    Friend WithEvents lbDataTotalToday As Label
    Friend WithEvents Label7 As Label
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
    Friend WithEvents FecharToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents MenuStrip1 As MenuStrip
    Friend WithEvents pbBRL As PictureBox
    Friend WithEvents pbUSD As PictureBox
    Friend WithEvents OpenFileDialog1 As OpenFileDialog
    Friend WithEvents SaveFileDialog1 As SaveFileDialog
    Friend WithEvents Label8 As Label
    Friend WithEvents APIToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents lbCaixa As Label
    Friend WithEvents Label9 As Label
    Friend WithEvents lbCaixaBRL As Label
    Friend WithEvents lbPercentInvestido As Label
    Friend WithEvents lbPercentCaixa As Label
    Friend WithEvents ToolTip1 As ToolTip
    Friend WithEvents panelDebug As Panel
    Friend WithEvents lbDebug As RichTextBox
    Friend WithEvents dgFuturos As DataGridView
    Friend WithEvents btSpot As Button
    Friend WithEvents btFuturos As Button
    Friend WithEvents Label10 As Label
    Friend WithEvents lbInvestimentoUSD As Label
    Friend WithEvents lbInvestimentoBRL As Label
    Friend WithEvents btVisaoGeral As Button
    Friend WithEvents btOrdensSpot As Button
    Friend WithEvents PanelOrdens As Panel
    Friend WithEvents SimbolosToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents CustodiaToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents PanelFuturos As Panel
    Friend WithEvents btOrdensFuturo As Button
    Friend WithEvents Label11 As Label
    Friend WithEvents Label2 As Label
    Friend WithEvents btTradebot As Button
    Friend WithEvents PanelTradeBot As Panel
    Friend WithEvents btSpotBotUniverse As Button
    Friend WithEvents btSpotBotStart As Button
    Friend WithEvents btSpotBotStop As Button
    Friend WithEvents btSpotBotExport As Button
    Friend WithEvents btSpotBotTrades As Button
    Friend WithEvents lbSpotBotSimulationSummary As Label
    Friend WithEvents GroupBox1 As GroupBox
    Friend WithEvents cbSpotBotBollingerMode As ComboBox
    Friend WithEvents nudSpotBotConfirmations As NumericUpDown
    Friend WithEvents Label13 As Label
    Friend WithEvents cbSpotBotCombination As ComboBox
    Friend WithEvents cbSpotBotBollingerTimeframe As ComboBox
    Friend WithEvents cbSpotBotUniverseMode As ComboBox
    Friend WithEvents Label14 As Label
    Friend WithEvents Label24 As Label
    Friend WithEvents Label23 As Label
    Friend WithEvents Label22 As Label
    Friend WithEvents Label21 As Label
    Friend WithEvents Label20 As Label
    Friend WithEvents Label19 As Label
    Friend WithEvents Label18 As Label
    Friend WithEvents Label17 As Label
    Friend WithEvents GroupBox2 As GroupBox
    Friend WithEvents chkSpotBotGridAdaptive As CheckBox
    Friend WithEvents CheckBox3 As CheckBox
    Friend WithEvents chkSpotBotBreakEven As CheckBox
    Friend WithEvents chkSpotBotWavePullback As CheckBox
    Friend WithEvents chkSpotBotBreakoutRetest As CheckBox
    Friend WithEvents chkSpotBotMomentumVolume As CheckBox
    Friend WithEvents chkSpotBotValueZonePullback As CheckBox
    Friend WithEvents chkSpotBotBtcMildDecline As CheckBox
    Friend WithEvents GroupBox3 As GroupBox
    Friend WithEvents chkSpotBotTrailing As CheckBox
    Friend WithEvents NumericUpDown2 As NumericUpDown
    Friend WithEvents NumericUpDown1 As NumericUpDown
    Friend WithEvents Label25 As Label
    Friend WithEvents Label12 As Label
    Friend WithEvents chkSpotBotBtcFlush As CheckBox
    Friend WithEvents Label26 As Label
    Friend WithEvents nudSpotBotBtcFlushVolume As NumericUpDown
    Friend WithEvents Label27 As Label
    Friend WithEvents nudSpotBotBtcFlushPause As NumericUpDown
    Friend WithEvents Label28 As Label
    Friend WithEvents nudSpotBotBtcFlushDrop As NumericUpDown
    Friend WithEvents chkSpotBotBtcDaily As CheckBox
    Friend WithEvents nudSpotBotBtcDailyDrop As NumericUpDown
    Friend WithEvents Label30 As Label
    Friend WithEvents Label29 As Label
    Friend WithEvents nudSpotBotBtcDailyDrawdown As NumericUpDown
    Friend WithEvents Label31 As Label
    Friend WithEvents Label32 As Label
    Friend WithEvents nudSpotBotStartingQuote As NumericUpDown
    Friend WithEvents nudSpotBotAllocation As NumericUpDown
    Friend WithEvents nudSpotBotFee As NumericUpDown
    Friend WithEvents Label33 As Label
    Friend WithEvents nudSpotBotStopLoss As NumericUpDown
    Friend WithEvents Label35 As Label
    Friend WithEvents nudSpotBotTakeProfit As NumericUpDown
    Friend WithEvents Label34 As Label
    Friend WithEvents nudSpotBotMaxAssets As NumericUpDown
    Friend WithEvents nudSpotBotMinVolume As NumericUpDown
    Friend WithEvents chkSpotBotGridRejection As CheckBox
    Friend WithEvents nudSpotBotPostExitCooldown As NumericUpDown
    Friend WithEvents nudSpotBotMinTrades As NumericUpDown
    Friend WithEvents nudSpotBotGridRisk As NumericUpDown
    Friend WithEvents Label36 As Label
    Friend WithEvents nudSpotBotGridReserve As NumericUpDown
    Friend WithEvents Label37 As Label
    Friend WithEvents nudSpotBotGridExposure As NumericUpDown
    Friend WithEvents Label38 As Label
    Friend WithEvents nudSpotBotGridLevels As NumericUpDown
    Friend WithEvents Label39 As Label
    Friend WithEvents DataGridView1 As DataGridView
    Friend WithEvents dgSpotBotPositionsControl As DataGridView
    Friend WithEvents GroupBox4 As GroupBox
    Friend WithEvents rtbSpotBotLog As RichTextBox
    Friend WithEvents chkSpotBotRiskBrakeEnabled As CheckBox
    Friend WithEvents nudSpotBotRiskSessionLoss As NumericUpDown
    Friend WithEvents Label40 As Label
    Friend WithEvents nudSpotBotRiskProfitTrigger As NumericUpDown
    Friend WithEvents Label41 As Label
    Friend WithEvents chkSpotBotRiskProfitLock As CheckBox
    Friend WithEvents nudSpotBotRiskGiveback As NumericUpDown
    Friend WithEvents Label42 As Label
    Friend WithEvents nudSpotBotRiskMaxLosses As NumericUpDown
    Friend WithEvents Label43 As Label
    Friend WithEvents nudSpotBotRiskPause As NumericUpDown
    Friend WithEvents Label44 As Label
    Friend WithEvents chkSpotBotRiskClosePositions As CheckBox
    Friend WithEvents chkSpotBotRiskUseUnrealized As CheckBox

End Class
