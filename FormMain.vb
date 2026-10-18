Imports System.Globalization
Imports System.IO
Imports System.Windows.Forms.DataVisualization.Charting
Imports Newtonsoft.Json.Linq
Imports System.Diagnostics
Imports System.Diagnostics.Tracing
Public Class FormMain
    Public remainingtimeInSeconds As Integer
    Dim Cjson As New JSON
    Dim chart As New Charts
    Dim B As New Binance
    Dim gec As New Coingecko
    Private ReadOnly _binanceWs As New BinanceWebSocket
    Private ReadOnly _gateWs As New GateWebSocket
    Private ReadOnly _binanceFuturesMarketWs As New BinanceFuturesMarketWebSocket
    Private ReadOnly _binanceFuturesUserWs As New BinanceFuturesUserDataWebSocket
    Private ReadOnly _binanceSpotUserWs As New BinanceSpotUserDataWebSocket
    Private _marketRefreshRunning As Boolean = False
    Private _futuresLoadRunning As Boolean = False
    Private _futuresMarketSymbolsKey As String = String.Empty
    Private _futuresUserStreamStarted As Boolean = False
    Private _spotUserStreamStarted As Boolean = False
    Private _spotAccountRefreshQueued As Boolean = False
    Private _spotPnlUsd As Decimal = 0D
    Private _futuresPnlUsd As Decimal = 0D
    Private _futuresEquityUsd As Decimal = 0D
    Private _futuresEquityBaseUsd As Decimal = 0D
    Private _futuresEquityLoaded As Boolean = False
    Private _spotCurrentUsd As Decimal = 0D
    Private _spotEntryUsd As Decimal = 0D
    Private _spotCashUsd As Decimal = 0D
    Private _spotCashLoadedFromAccount As Boolean = False
    Private _spotCoinEntryUsd As Decimal = 0D
    Private _grossInvestmentUsd As Decimal = 0D
    Private _grossInvestmentBrl As Decimal = 0D
    Private _spotOverviewInitialized As Boolean = False
    Private _dgOrdensSpot As DataGridView
    Private _btAtualizarOrdensSpot As Button
    Private _spotOrdersLoadRunning As Boolean = False
    Private _cbSpotOrderSymbol As ComboBox
    Private _cbSpotOrderSide As ComboBox
    Private _cbSpotOrderType As ComboBox
    Private _txtSpotOrderQuantity As TextBox
    Private _txtSpotOrderPrice As TextBox
    Private _txtSpotOrderCurrentPrice As TextBox
    Private _lbSpotOrderCash As Label
    Private _txtSpotOrderNotional As TextBox
    Private _btGraficoSpotOrder As Button
    Private _btAbrirSpotOrder As Button
    Private _dgOrdensFuturos As DataGridView
    Private _btAtualizarOrdensFuturos As Button
    Private _futuresOrdersLoadRunning As Boolean = False
    Private _cbFuturesOrderSymbol As ComboBox
    Private _cbFuturesOrderSide As ComboBox
    Private _cbFuturesOrderType As ComboBox
    Private _cbFuturesOrderLeverage As ComboBox
    Private _txtFuturesOrderQuantity As TextBox
    Private _txtFuturesOrderPrice As TextBox
    Private _txtFuturesOrderCurrentPrice As TextBox
    Private _txtFuturesOrderNotional As TextBox
    Private _lbFuturesOrderAvailable As Label
    Private _btGraficoFuturesOrder As Button
    Private _btAbrirFuturesOrder As Button
    Private _futuresAvailableUsd As Decimal = 0D
    Private _futuresOrderInputUpdating As Boolean = False
    Private _futuresOrderInputMode As String = "Quantity"
    Private _spotOrderInputUpdating As Boolean = False
    Private _spotOrderInputMode As String = "Quantity"
    Private ReadOnly _futuresLeverageBySymbol As New Dictionary(Of String, Integer)(StringComparer.OrdinalIgnoreCase)
    Private _portfolioGridDefaultHeight As Integer
    Private _futuresGridDefaultHeight As Integer
    Private _binanceWsStartRunning As Boolean
    Private _catalogWarningShown As Boolean
    Private _binanceCatalogPromptRunning As Boolean
    Private _credentialsMenuItem As ToolStripMenuItem
    Private ReadOnly _spotTradeBot As New SpotTradeBotEngine()
    Private _panelSpotBot As Panel
    Private _botLog As RichTextBox
    Private _botCombination As ComboBox
    Private _botConfirmations As NumericUpDown
    Private _botStartingQuote As NumericUpDown
    Private _botAllocation As NumericUpDown
    Private _botFee As NumericUpDown
    Private _botSimulationSummary As Label
    Private _botStart As Button
    Private _botStop As Button
    Private _btTradeBot As Button
    Private ReadOnly _botStrategyChecks As New Dictionary(Of SpotBotStrategyKind, CheckBox)()
    Private _lastBotSummaryUpdateUtc As DateTime
    Private _lastBotSimulationAction As String = String.Empty

    Private Sub CriptoToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CriptoToolStripMenuItem.Click
        FormEntradas.Show()
    End Sub
    Private Sub FecharToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles FecharToolStripMenuItem.Click
        Application.Exit()
    End Sub
    Public Shared Function msgQuestion(ByVal msgText As String, ByVal Title As String) As String
        If MessageBox.Show(msgText, Title, MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button3) = DialogResult.Yes Then
            Return True
        Else
            Return False
        End If
    End Function
    Private Sub Form1_LoadAsync(sender As Object, e As EventArgs) Handles MyBase.Load
        ConfigureCredentialsMenu()
        ConfigureSpotOrdersPanel()
        ConfigureFuturesOrdersPanel()
        ConfigureSpotBotPanel()
        _portfolioGridDefaultHeight = dgPortfolio.Height
        _futuresGridDefaultHeight = dgFuturos.Height
        ArrangeOrdersPanel(False)
        Setup()
        lbDataTotalToday.Text = Date.Today & ":"
        ArrangeProfitPairs()
    End Sub

    Private Sub ConfigureCredentialsMenu()
        If _credentialsMenuItem IsNot Nothing Then Return

        _credentialsMenuItem = New ToolStripMenuItem With {
            .Name = "CredenciaisExchangesToolStripMenuItem",
            .Text = "Credenciais exchanges",
            .BackColor = Color.FromArgb(31, 33, 32),
            .ForeColor = SystemColors.ButtonHighlight
        }
        AddHandler _credentialsMenuItem.Click, AddressOf CredentialsMenuItem_Click
        OpçõesToolStripMenuItem.DropDownItems.Insert(0, _credentialsMenuItem)
    End Sub

    Private Sub CredentialsMenuItem_Click(sender As Object, e As EventArgs)
        Using credentialsForm As New FormExchangeCredentials()
            credentialsForm.ShowDialog(Me)
        End Using
    End Sub

    Private Sub ConfigureSpotOrdersPanel()
        If _dgOrdensSpot IsNot Nothing Then
            Return
        End If

        Dim editor As New Panel With {
            .Dock = DockStyle.Top,
            .Height = 40,
            .BackColor = Color.FromArgb(30, 30, 30)
        }

        AddSpotOrderEditorLabel(editor, "Cripto", 6, 1, 150)
        _cbSpotOrderSymbol = New ComboBox With {
            .BackColor = Color.FromArgb(45, 45, 45),
            .DropDownStyle = ComboBoxStyle.DropDown,
            .FlatStyle = FlatStyle.Flat,
            .ForeColor = Color.White,
            .Location = New Point(6, 17),
            .Name = "cbSpotOrderSymbol",
            .Size = New Size(150, 23)
        }
        AddHandler _cbSpotOrderSymbol.SelectedIndexChanged, AddressOf SpotOrderSymbolChanged
        AddHandler _cbSpotOrderSymbol.TextChanged, AddressOf SpotOrderSymbolTextChanged
        editor.Controls.Add(_cbSpotOrderSymbol)

        _btGraficoSpotOrder = CreateSpotOrderEditorButton("↗", 160, 15, 28)
        _btGraficoSpotOrder.Name = "btGraficoSpotOrder"
        _btGraficoSpotOrder.Font = New Font("Calibri", 10.0F, FontStyle.Bold)
        AddHandler _btGraficoSpotOrder.Click, AddressOf btGraficoSpotOrder_Click
        editor.Controls.Add(_btGraficoSpotOrder)
        ToolTip1.SetToolTip(_btGraficoSpotOrder, "Abrir gráfico da moeda selecionada")

        AddSpotOrderEditorLabel(editor, "Preço atual", 194, 1, 110)
        _txtSpotOrderCurrentPrice = CreateSpotOrderEditorTextBox("txtSpotOrderCurrentPrice", 194, 17, 110)
        _txtSpotOrderCurrentPrice.ReadOnly = True
        _txtSpotOrderCurrentPrice.TabStop = False
        _txtSpotOrderCurrentPrice.TextAlign = HorizontalAlignment.Right
        _txtSpotOrderCurrentPrice.ForeColor = Color.Gold
        _txtSpotOrderCurrentPrice.Text = "--"
        editor.Controls.Add(_txtSpotOrderCurrentPrice)

        AddSpotOrderEditorLabel(editor, "Lado", 314, 1, 68)
        _cbSpotOrderSide = New ComboBox With {
            .BackColor = Color.FromArgb(45, 45, 45),
            .DropDownStyle = ComboBoxStyle.DropDownList,
            .FlatStyle = FlatStyle.Flat,
            .ForeColor = Color.White,
            .Location = New Point(314, 17),
            .Name = "cbSpotOrderSide",
            .Size = New Size(68, 23)
        }
        _cbSpotOrderSide.Items.AddRange(New Object() {"BUY", "SELL"})
        _cbSpotOrderSide.SelectedIndex = 0
        editor.Controls.Add(_cbSpotOrderSide)

        AddSpotOrderEditorLabel(editor, "Tipo", 390, 1, 76)
        _cbSpotOrderType = New ComboBox With {
            .BackColor = Color.FromArgb(45, 45, 45),
            .DropDownStyle = ComboBoxStyle.DropDownList,
            .FlatStyle = FlatStyle.Flat,
            .ForeColor = Color.White,
            .Location = New Point(390, 17),
            .Name = "cbSpotOrderType",
            .Size = New Size(76, 23)
        }
        _cbSpotOrderType.Items.Add("LIMIT")
        _cbSpotOrderType.SelectedIndex = 0
        AddHandler _cbSpotOrderType.SelectedIndexChanged, AddressOf SpotOrderTypeChanged
        editor.Controls.Add(_cbSpotOrderType)

        AddSpotOrderEditorLabel(editor, "Caixa Spot", 474, 1, 96)
        _lbSpotOrderCash = New Label With {
            .AutoSize = False,
            .BackColor = Color.FromArgb(45, 45, 45),
            .BorderStyle = BorderStyle.FixedSingle,
            .ForeColor = Color.Gold,
            .Location = New Point(474, 17),
            .Name = "lbSpotOrderCash",
            .Size = New Size(96, 23),
            .Text = "$0.00",
            .TextAlign = ContentAlignment.MiddleRight
        }
        editor.Controls.Add(_lbSpotOrderCash)

        AddSpotOrderEditorLabel(editor, "Quantidade", 584, 1, 96)
        _txtSpotOrderQuantity = CreateSpotOrderEditorTextBox("txtSpotOrderQuantity", 584, 17, 96)
        AddHandler _txtSpotOrderQuantity.TextChanged, AddressOf SpotOrderQuantityChanged
        editor.Controls.Add(_txtSpotOrderQuantity)

        AddSpotOrderEditorLabel(editor, "Preço limite", 688, 1, 102)
        _txtSpotOrderPrice = CreateSpotOrderEditorTextBox("txtSpotOrderPrice", 688, 17, 102)
        AddHandler _txtSpotOrderPrice.TextChanged, AddressOf SpotOrderPriceChanged
        editor.Controls.Add(_txtSpotOrderPrice)

        AddSpotOrderEditorLabel(editor, "Valor USDT", 798, 1, 102)
        _txtSpotOrderNotional = CreateSpotOrderEditorTextBox("txtSpotOrderNotional", 798, 17, 102)
        _txtSpotOrderNotional.TextAlign = HorizontalAlignment.Right
        _txtSpotOrderNotional.ForeColor = Color.Gold
        AddHandler _txtSpotOrderNotional.TextChanged, AddressOf SpotOrderNotionalChanged
        editor.Controls.Add(_txtSpotOrderNotional)

        _btAbrirSpotOrder = CreateSpotOrderEditorButton("Enviar ordem", 908, 15, 102)
        _btAbrirSpotOrder.BackColor = Color.DarkGreen
        AddHandler _btAbrirSpotOrder.Click, AddressOf btAbrirSpotOrder_Click
        editor.Controls.Add(_btAbrirSpotOrder)

        _btAtualizarOrdensSpot = CreateSpotOrderEditorButton("Atualizar", 1018, 15, 90)
        _btAtualizarOrdensSpot.Name = "btAtualizarOrdensSpot"
        AddHandler _btAtualizarOrdensSpot.Click, AddressOf btAtualizarOrdensSpot_Click
        editor.Controls.Add(_btAtualizarOrdensSpot)

        SpotOrderTypeChanged(Nothing, EventArgs.Empty)

        _dgOrdensSpot = New DataGridView With {
            .AllowUserToAddRows = False,
            .AllowUserToDeleteRows = False,
            .AllowUserToOrderColumns = True,
            .AllowUserToResizeRows = False,
            .AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            .BackgroundColor = Color.FromArgb(64, 64, 64),
            .BorderStyle = BorderStyle.FixedSingle,
            .CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
            .ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None,
            .Dock = DockStyle.Fill,
            .EnableHeadersVisualStyles = False,
            .MultiSelect = False,
            .Name = "dgOrdensSpot",
            .ReadOnly = True,
            .RowHeadersVisible = False,
            .SelectionMode = DataGridViewSelectionMode.FullRowSelect
        }

        _dgOrdensSpot.ColumnHeadersDefaultCellStyle = New DataGridViewCellStyle With {
            .Alignment = DataGridViewContentAlignment.MiddleLeft,
            .BackColor = Color.FromArgb(40, 40, 40),
            .Font = New Font("Calibri", 10.0F, FontStyle.Italic),
            .ForeColor = Color.Turquoise,
            .SelectionBackColor = Color.FromArgb(40, 40, 40),
            .SelectionForeColor = Color.Turquoise,
            .WrapMode = DataGridViewTriState.True
        }
        _dgOrdensSpot.DefaultCellStyle = New DataGridViewCellStyle With {
            .Alignment = DataGridViewContentAlignment.MiddleLeft,
            .BackColor = Color.FromArgb(64, 64, 64),
            .Font = New Font("Calibri", 11.0F),
            .ForeColor = Color.White,
            .SelectionBackColor = Color.Black,
            .SelectionForeColor = Color.White
        }
        _dgOrdensSpot.RowTemplate.Height = 30
        _dgOrdensSpot.ColumnHeadersHeight = 32

        AddHandler _dgOrdensSpot.CellContentClick, AddressOf dgOrdensSpot_CellContentClick
        AddHandler _dgOrdensSpot.CellFormatting, AddressOf dgOrdensSpot_CellFormatting

        PanelOrdens.Controls.Add(_dgOrdensSpot)
        PanelOrdens.Controls.Add(editor)
    End Sub

    Private Shared Sub AddSpotOrderEditorLabel(
        parent As Control,
        text As String,
        left As Integer,
        top As Integer,
        width As Integer)

        parent.Controls.Add(New Label With {
            .AutoSize = False,
            .ForeColor = Color.Turquoise,
            .Location = New Point(left, top),
            .Size = New Size(width, 16),
            .Text = text
        })
    End Sub

    Private Shared Function CreateSpotOrderEditorTextBox(
        name As String,
        left As Integer,
        top As Integer,
        width As Integer) As TextBox

        Return New TextBox With {
            .BackColor = Color.FromArgb(45, 45, 45),
            .BorderStyle = BorderStyle.FixedSingle,
            .ForeColor = Color.White,
            .Location = New Point(left, top),
            .Name = name,
            .Size = New Size(width, 23)
        }
    End Function

    Private Shared Function CreateSpotOrderEditorButton(
        text As String,
        left As Integer,
        top As Integer,
        width As Integer) As Button

        Dim button As New Button With {
            .BackColor = Color.FromArgb(20, 20, 20),
            .Cursor = Cursors.Hand,
            .FlatStyle = FlatStyle.Flat,
            .ForeColor = Color.White,
            .Location = New Point(left, top),
            .Size = New Size(width, 24),
            .Text = text,
            .UseVisualStyleBackColor = False
        }
        button.FlatAppearance.BorderColor = Color.Gray
        button.FlatAppearance.BorderSize = 1
        Return button
    End Function

    Private Sub ConfigureFuturesOrdersPanel()
        If PanelFuturos Is Nothing OrElse _dgOrdensFuturos IsNot Nothing Then
            Return
        End If

        Dim editor As New Panel With {
            .Dock = DockStyle.Top,
            .Height = 40,
            .BackColor = Color.FromArgb(30, 30, 30)
        }

        AddSpotOrderEditorLabel(editor, "Cripto", 6, 1, 150)
        _cbFuturesOrderSymbol = New ComboBox With {
            .BackColor = Color.FromArgb(45, 45, 45),
            .DropDownStyle = ComboBoxStyle.DropDown,
            .FlatStyle = FlatStyle.Flat,
            .ForeColor = Color.White,
            .Location = New Point(6, 17),
            .Name = "cbFuturesOrderSymbol",
            .Size = New Size(150, 23)
        }
        AddHandler _cbFuturesOrderSymbol.SelectedIndexChanged, AddressOf FuturesOrderSymbolChanged
        AddHandler _cbFuturesOrderSymbol.TextChanged, AddressOf FuturesOrderSymbolTextChanged
        editor.Controls.Add(_cbFuturesOrderSymbol)

        _btGraficoFuturesOrder = CreateSpotOrderEditorButton("↗", 160, 15, 28)
        _btGraficoFuturesOrder.Name = "btGraficoFuturesOrder"
        _btGraficoFuturesOrder.Font = New Font("Calibri", 10.0F, FontStyle.Bold)
        AddHandler _btGraficoFuturesOrder.Click, AddressOf btGraficoFuturesOrder_Click
        editor.Controls.Add(_btGraficoFuturesOrder)
        ToolTip1.SetToolTip(_btGraficoFuturesOrder, "Abrir gráfico da moeda selecionada")

        AddSpotOrderEditorLabel(editor, "Preço atual", 194, 1, 110)
        _txtFuturesOrderCurrentPrice = CreateSpotOrderEditorTextBox("txtFuturesOrderCurrentPrice", 194, 17, 110)
        _txtFuturesOrderCurrentPrice.ReadOnly = True
        _txtFuturesOrderCurrentPrice.TabStop = False
        _txtFuturesOrderCurrentPrice.TextAlign = HorizontalAlignment.Right
        _txtFuturesOrderCurrentPrice.ForeColor = Color.Yellow
        _txtFuturesOrderCurrentPrice.Text = "--"
        editor.Controls.Add(_txtFuturesOrderCurrentPrice)

        AddSpotOrderEditorLabel(editor, "Lado", 314, 1, 76)
        _cbFuturesOrderSide = New ComboBox With {
            .BackColor = Color.FromArgb(45, 45, 45),
            .DropDownStyle = ComboBoxStyle.DropDownList,
            .FlatStyle = FlatStyle.Flat,
            .ForeColor = Color.White,
            .Location = New Point(314, 17),
            .Name = "cbFuturesOrderSide",
            .Size = New Size(76, 23)
        }
        _cbFuturesOrderSide.Items.AddRange(New Object() {"LONG", "SHORT"})
        _cbFuturesOrderSide.SelectedIndex = 0
        editor.Controls.Add(_cbFuturesOrderSide)

        AddSpotOrderEditorLabel(editor, "Tipo", 398, 1, 76)
        _cbFuturesOrderType = New ComboBox With {
            .BackColor = Color.FromArgb(45, 45, 45),
            .DropDownStyle = ComboBoxStyle.DropDownList,
            .FlatStyle = FlatStyle.Flat,
            .ForeColor = Color.White,
            .Location = New Point(398, 17),
            .Name = "cbFuturesOrderType",
            .Size = New Size(76, 23)
        }
        _cbFuturesOrderType.Items.Add("LIMIT")
        _cbFuturesOrderType.SelectedIndex = 0
        AddHandler _cbFuturesOrderType.SelectedIndexChanged, AddressOf FuturesOrderTypeChanged
        editor.Controls.Add(_cbFuturesOrderType)

        AddSpotOrderEditorLabel(editor, "Disponível", 482, 1, 90)
        _lbFuturesOrderAvailable = New Label With {
            .AutoSize = False,
            .BackColor = Color.FromArgb(45, 45, 45),
            .BorderStyle = BorderStyle.FixedSingle,
            .ForeColor = Color.Gold,
            .Location = New Point(482, 17),
            .Name = "lbFuturesOrderAvailable",
            .Size = New Size(90, 23),
            .Text = "$0.00",
            .TextAlign = ContentAlignment.MiddleRight
        }
        editor.Controls.Add(_lbFuturesOrderAvailable)

        AddSpotOrderEditorLabel(editor, "Alavancagem", 576, 1, 76)
        _cbFuturesOrderLeverage = New ComboBox With {
            .BackColor = Color.FromArgb(45, 45, 45),
            .DropDownStyle = ComboBoxStyle.DropDownList,
            .FlatStyle = FlatStyle.Flat,
            .ForeColor = Color.White,
            .Location = New Point(576, 17),
            .Name = "cbFuturesOrderLeverage",
            .Size = New Size(76, 23)
        }
        For leverage As Integer = 0 To 10
            _cbFuturesOrderLeverage.Items.Add($"{leverage}x")
        Next
        _cbFuturesOrderLeverage.SelectedIndex = 0
        editor.Controls.Add(_cbFuturesOrderLeverage)

        AddSpotOrderEditorLabel(editor, "Quantidade", 658, 1, 84)
        _txtFuturesOrderQuantity = CreateSpotOrderEditorTextBox("txtFuturesOrderQuantity", 658, 17, 84)
        AddHandler _txtFuturesOrderQuantity.TextChanged, AddressOf FuturesOrderQuantityChanged
        editor.Controls.Add(_txtFuturesOrderQuantity)

        AddSpotOrderEditorLabel(editor, "Preço limite", 748, 1, 90)
        _txtFuturesOrderPrice = CreateSpotOrderEditorTextBox("txtFuturesOrderPrice", 748, 17, 90)
        AddHandler _txtFuturesOrderPrice.TextChanged, AddressOf FuturesOrderPriceChanged
        editor.Controls.Add(_txtFuturesOrderPrice)

        AddSpotOrderEditorLabel(editor, "Valor USDT", 844, 1, 90)
        _txtFuturesOrderNotional = CreateSpotOrderEditorTextBox("txtFuturesOrderNotional", 844, 17, 90)
        _txtFuturesOrderNotional.TextAlign = HorizontalAlignment.Right
        _txtFuturesOrderNotional.ForeColor = Color.Gold
        AddHandler _txtFuturesOrderNotional.TextChanged, AddressOf FuturesOrderNotionalChanged
        editor.Controls.Add(_txtFuturesOrderNotional)

        _btAbrirFuturesOrder = CreateSpotOrderEditorButton("Enviar ordem", 940, 15, 100)
        _btAbrirFuturesOrder.BackColor = Color.DarkGreen
        _btAbrirFuturesOrder.Name = "btAbrirFuturesOrder"
        AddHandler _btAbrirFuturesOrder.Click, AddressOf btAbrirFuturesOrder_Click
        editor.Controls.Add(_btAbrirFuturesOrder)

        _btAtualizarOrdensFuturos = CreateSpotOrderEditorButton("Atualizar", 1046, 15, 90)
        _btAtualizarOrdensFuturos.Name = "btAtualizarOrdensFuturos"
        AddHandler _btAtualizarOrdensFuturos.Click, AddressOf btAtualizarOrdensFuturos_Click
        editor.Controls.Add(_btAtualizarOrdensFuturos)

        FuturesOrderTypeChanged(Nothing, EventArgs.Empty)

        _dgOrdensFuturos = New DataGridView With {
            .AllowUserToAddRows = False,
            .AllowUserToDeleteRows = False,
            .AllowUserToOrderColumns = True,
            .AllowUserToResizeRows = False,
            .AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            .BackgroundColor = Color.FromArgb(64, 64, 64),
            .BorderStyle = BorderStyle.FixedSingle,
            .CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
            .ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None,
            .Dock = DockStyle.Fill,
            .EnableHeadersVisualStyles = False,
            .MultiSelect = False,
            .Name = "dgOrdensFuturos",
            .ReadOnly = True,
            .RowHeadersVisible = False,
            .SelectionMode = DataGridViewSelectionMode.FullRowSelect
        }
        _dgOrdensFuturos.ColumnHeadersDefaultCellStyle = New DataGridViewCellStyle With {
            .Alignment = DataGridViewContentAlignment.MiddleLeft,
            .BackColor = Color.FromArgb(40, 40, 40),
            .Font = New Font("Calibri", 10.0F, FontStyle.Italic),
            .ForeColor = Color.Turquoise,
            .SelectionBackColor = Color.FromArgb(40, 40, 40),
            .SelectionForeColor = Color.Turquoise,
            .WrapMode = DataGridViewTriState.True
        }
        _dgOrdensFuturos.DefaultCellStyle = New DataGridViewCellStyle With {
            .Alignment = DataGridViewContentAlignment.MiddleLeft,
            .BackColor = Color.FromArgb(64, 64, 64),
            .Font = New Font("Calibri", 11.0F),
            .ForeColor = Color.White,
            .SelectionBackColor = Color.Black,
            .SelectionForeColor = Color.White
        }
        _dgOrdensFuturos.RowTemplate.Height = 30
        _dgOrdensFuturos.ColumnHeadersHeight = 32
        AddHandler _dgOrdensFuturos.CellContentClick, AddressOf dgOrdensFuturos_CellContentClick
        AddHandler _dgOrdensFuturos.CellFormatting, AddressOf dgOrdensFuturos_CellFormatting

        PanelFuturos.Controls.Add(_dgOrdensFuturos)
        PanelFuturos.Controls.Add(editor)
    End Sub

    Private Sub ConfigureSpotBotPanel()
        If _panelSpotBot IsNot Nothing Then
            Return
        End If

        _panelSpotBot = New Panel With {
            .Name = "PanelSpotBot",
            .BackColor = Color.FromArgb(30, 30, 30),
            .Padding = New Padding(14, 0, 14, 0),
            .Visible = False
        }

        Dim editor As New Panel With {
            .Dock = DockStyle.Top,
            .Height = 94,
            .BackColor = Color.FromArgb(30, 30, 30)
        }

        AddBotLabel(editor, "Estratégias ativas", 6, 3, 150)
        AddBotStrategyCheckBox(editor, SpotBotStrategyKind.EmaCrossover, "EMA", 6, 24, 100)
        AddBotStrategyCheckBox(editor, SpotBotStrategyKind.Rsi, "RSI", 112, 24, 80)
        AddBotStrategyCheckBox(editor, SpotBotStrategyKind.Breakout, "Rompimento", 198, 24, 118)
        AddBotStrategyCheckBox(editor, SpotBotStrategyKind.Grid, "Grade", 322, 24, 80)

        AddBotLabel(editor, "Combinação", 420, 3, 100)
        _botCombination = New ComboBox With {
            .Name = "cbSpotBotCombination",
            .DropDownStyle = ComboBoxStyle.DropDownList,
            .BackColor = Color.FromArgb(64, 64, 64),
            .ForeColor = Color.White,
            .FlatStyle = FlatStyle.Flat,
            .Location = New Point(420, 24),
            .Width = 190
        }
        _botCombination.Items.AddRange(New Object() {
            "Qualquer estratégia",
            "Todas as estratégias",
            "Mínimo de confirmações",
            "Pontuação ponderada"
        })
        _botCombination.SelectedIndex = 2
        editor.Controls.Add(_botCombination)

        AddBotLabel(editor, "Confirmações", 620, 3, 100)
        _botConfirmations = New NumericUpDown With {
            .Name = "nudSpotBotConfirmations",
            .Minimum = 1,
            .Maximum = 4,
            .Value = 2,
            .BackColor = Color.FromArgb(64, 64, 64),
            .ForeColor = Color.White,
            .Location = New Point(620, 24),
            .Width = 64
        }
        editor.Controls.Add(_botConfirmations)

        Dim simulationLabel As New Label With {
            .AutoSize = True,
            .ForeColor = Color.Gold,
            .Font = New Font("Calibri", 10.0F, FontStyle.Bold),
            .Location = New Point(704, 28),
            .Text = "SIMULAÇÃO — nenhuma ordem será enviada"
        }
        editor.Controls.Add(simulationLabel)

        _botStart = CreateBotButton("Iniciar bot", 6, 58, 110, Color.DarkGreen)
        AddHandler _botStart.Click, AddressOf SpotBotStart_Click
        editor.Controls.Add(_botStart)

        _botStop = CreateBotButton("Parar bot", 122, 58, 100, Color.IndianRed)
        _botStop.Enabled = False
        AddHandler _botStop.Click, AddressOf SpotBotStop_Click
        editor.Controls.Add(_botStop)

        AddBotLabel(editor, "Saldo virtual (USDT)", 240, 55, 110)
        _botStartingQuote = CreateBotNumeric(1000D, 0D, 100000000D, 2, 240, 74, 98)
        editor.Controls.Add(_botStartingQuote)

        AddBotLabel(editor, "Alocação (%)", 348, 55, 86)
        _botAllocation = CreateBotNumeric(10D, 0D, 100D, 2, 348, 74, 82)
        editor.Controls.Add(_botAllocation)

        AddBotLabel(editor, "Taxa (%)", 440, 55, 70)
        _botFee = CreateBotNumeric(0.1D, 0D, 5D, 3, 440, 74, 70)
        editor.Controls.Add(_botFee)

        Dim hint As New Label With {
            .AutoSize = True,
            .ForeColor = Color.LightGray,
            .Location = New Point(520, 78),
            .Text = "Os períodos usam as amostras recebidas pelo WebSocket."
        }
        editor.Controls.Add(hint)

        _botLog = New RichTextBox With {
            .Name = "rtbSpotBotLog",
            .Dock = DockStyle.Fill,
            .BackColor = Color.Black,
            .ForeColor = Color.White,
            .BorderStyle = BorderStyle.FixedSingle,
            .ReadOnly = True,
            .DetectUrls = False,
            .Font = New Font("Consolas", 10.0F),
            .ScrollBars = RichTextBoxScrollBars.Vertical
        }

        _botSimulationSummary = New Label With {
            .Name = "lbSpotBotSimulationSummary",
            .Dock = DockStyle.Top,
            .Height = 26,
            .BackColor = Color.FromArgb(20, 20, 20),
            .ForeColor = Color.Gold,
            .Font = New Font("Calibri", 10.0F, FontStyle.Bold),
            .TextAlign = ContentAlignment.MiddleLeft,
            .Padding = New Padding(4, 0, 4, 0),
            .Text = "Simulação: aguardando início"
        }

        _panelSpotBot.Controls.Add(_botLog)
        _panelSpotBot.Controls.Add(_botSimulationSummary)
        _panelSpotBot.Controls.Add(editor)
        Controls.Add(_panelSpotBot)

        _btTradeBot = New Button With {
            .Name = "btTradeBot",
            .Text = "Tradebot",
            .BackColor = Color.FromArgb(20, 20, 20),
            .ForeColor = Color.White,
            .FlatStyle = FlatStyle.Flat,
            .Cursor = Cursors.Hand,
            .Location = New Point(280, 432),
            .Size = New Size(85, 23)
        }
        _btTradeBot.FlatAppearance.BorderSize = 0
        _btTradeBot.FlatAppearance.MouseOverBackColor = Color.RoyalBlue
        _btTradeBot.FlatAppearance.MouseDownBackColor = Color.DodgerBlue
        AddHandler _btTradeBot.Click, AddressOf btTradeBot_Click
        Controls.Add(_btTradeBot)
        ArrangeSpotBotButton()

        AddHandler _spotTradeBot.SignalGenerated, AddressOf SpotTradeBot_SignalGenerated
        AddHandler _spotTradeBot.SimulationUpdated, AddressOf SpotTradeBot_SimulationUpdated
    End Sub

    Private Sub AddBotStrategyCheckBox(
        editor As Panel,
        kind As SpotBotStrategyKind,
        text As String,
        left As Integer,
        top As Integer,
        width As Integer)

        Dim check As New CheckBox With {
            .Name = $"chkSpotBot{kind}",
            .Text = text,
            .AutoSize = False,
            .Width = width,
            .Height = 24,
            .Location = New Point(left, top),
            .ForeColor = Color.White,
            .BackColor = Color.FromArgb(30, 30, 30),
            .FlatStyle = FlatStyle.Flat,
            .Cursor = Cursors.Hand
        }
        editor.Controls.Add(check)
        _botStrategyChecks(kind) = check
    End Sub

    Private Shared Sub AddBotLabel(editor As Panel, text As String, left As Integer, top As Integer, width As Integer)
        editor.Controls.Add(New Label With {
            .AutoSize = False,
            .Width = width,
            .Height = 20,
            .Location = New Point(left, top),
            .ForeColor = Color.Aqua,
            .Font = New Font("Calibri", 10.0F, FontStyle.Italic),
            .Text = text
        })
    End Sub

    Private Shared Function CreateBotButton(
        text As String,
        left As Integer,
        top As Integer,
        width As Integer,
        backColor As Color) As Button

        Dim button As New Button With {
            .Text = text,
            .BackColor = backColor,
            .ForeColor = Color.White,
            .FlatStyle = FlatStyle.Flat,
            .Cursor = Cursors.Hand,
            .Location = New Point(left, top),
            .Size = New Size(width, 26)
        }
        button.FlatAppearance.BorderSize = 0
        button.FlatAppearance.MouseOverBackColor = Color.RoyalBlue
        button.FlatAppearance.MouseDownBackColor = Color.DodgerBlue
        Return button
    End Function

    Private Shared Function CreateBotNumeric(
        value As Decimal,
        minimum As Decimal,
        maximum As Decimal,
        decimalPlaces As Integer,
        left As Integer,
        top As Integer,
        width As Integer) As NumericUpDown

        Return New NumericUpDown With {
            .DecimalPlaces = decimalPlaces,
            .Increment = If(decimalPlaces = 0, 1D, 0.1D),
            .Minimum = minimum,
            .Maximum = maximum,
            .Value = value,
            .BackColor = Color.FromArgb(64, 64, 64),
            .ForeColor = Color.White,
            .Location = New Point(left, top),
            .Width = width
        }
    End Function

    Private Sub ArrangeSpotBotPanel(showBot As Boolean)
        If _panelSpotBot Is Nothing OrElse PanelGraphs Is Nothing Then
            Return
        End If

        If Not showBot Then
            _panelSpotBot.Visible = False
            Return
        End If

        Dim panelTop = Math.Max(PanelGraphs.Top, _btTradeBot.Bottom + 2)
        Dim panelBottom = PanelProfits.Top
        Dim panelHeight = Math.Max(150, panelBottom - panelTop)
        _panelSpotBot.Padding = New Padding(14, 0, 14, 0)
        _panelSpotBot.Location = New Point(0, panelTop)
        _panelSpotBot.Size = New Size(Me.ClientSize.Width, panelHeight)
        _panelSpotBot.Visible = True
        _panelSpotBot.BringToFront()

        btVisaoGeral.BringToFront()
        btOrdensSpot.BringToFront()
        btOrdensFuturo.BringToFront()
        _btTradeBot.BringToFront()
    End Sub

    Private Sub ArrangeSpotBotButton()
        If _btTradeBot Is Nothing OrElse btOrdensFuturo Is Nothing Then
            Return
        End If

        _btTradeBot.Location = New Point(btOrdensFuturo.Right, btOrdensFuturo.Top)
    End Sub

    Private Sub ApplySpotBotControls()
        _spotTradeBot.DisableAllStrategies()
        For Each item In _botStrategyChecks
            _spotTradeBot.SetStrategyEnabled(item.Key, item.Value.Checked)
        Next

        Select Case _botCombination.SelectedIndex
            Case 0
                _spotTradeBot.Configuration.CombinationMode = SpotBotCombinationMode.Any
            Case 1
                _spotTradeBot.Configuration.CombinationMode = SpotBotCombinationMode.All
            Case 3
                _spotTradeBot.Configuration.CombinationMode = SpotBotCombinationMode.WeightedScore
            Case Else
                _spotTradeBot.Configuration.CombinationMode = SpotBotCombinationMode.MinimumConfirmations
        End Select

        _spotTradeBot.Configuration.RequiredConfirmations = CInt(_botConfirmations.Value)
        _spotTradeBot.Configuration.SimulationStartingQuote = _botStartingQuote.Value
        _spotTradeBot.Configuration.SimulationAllocationPercent = _botAllocation.Value
        _spotTradeBot.Configuration.SimulationFeeRate = _botFee.Value / 100D
        _spotTradeBot.Configuration.SimulationOnly = True
    End Sub

    Private Sub SpotBotStart_Click(sender As Object, e As EventArgs)
        ApplySpotBotControls()
        If _spotTradeBot.Configuration.EnabledStrategies.Count = 0 Then
            AppendSpotBotLog("Selecione pelo menos uma estratégia antes de iniciar.", Color.Gold)
            Return
        End If

        _spotTradeBot.ResetHistory()
        _spotTradeBot.Start()
        _botStart.Enabled = False
        _botStop.Enabled = True
        _lastBotSimulationAction = String.Empty
        _botSimulationSummary.ForeColor = Color.Gold
        _botSimulationSummary.Text =
            $"Simulação | Patrimônio: {_spotTradeBot.Configuration.SimulationStartingQuote:0.00} USDT | " &
            "P/L: 0.00 (realizado 0.00 / aberto 0.00) | Trades: 0 | Acerto: 0.0% | Abertas: 0"
        AppendSpotBotLog("Bot Spot iniciado em modo SIMULAÇÃO.", Color.LimeGreen)
        AppendSpotBotLog("Estratégias: " & String.Join(", ", _spotTradeBot.Configuration.EnabledStrategies), Color.LightGray)
    End Sub

    Private Sub SpotBotStop_Click(sender As Object, e As EventArgs)
        _spotTradeBot.Stop()
        _botStart.Enabled = True
        _botStop.Enabled = False
        AppendSpotBotLog("Bot Spot parado.", Color.IndianRed)
    End Sub

    Private Sub SpotTradeBot_SignalGenerated(sender As Object, e As SpotBotSignalEventArgs)
        If e Is Nothing OrElse e.Signal Is Nothing Then Return

        If InvokeRequired Then
            BeginInvoke(New MethodInvoker(Sub() SpotTradeBot_SignalGenerated(Me, e)))
            Return
        End If

        Dim signal = e.Signal
        Dim directionText = If(signal.Direction = SpotBotSignalDirection.Buy, "COMPRA", "VENDA")
        AppendSpotBotLog(
            $"{signal.TimestampUtc.ToLocalTime():HH:mm:ss} {directionText} {signal.Symbol} @ {signal.Price:0.########} " &
            $"({signal.Confirmations}/{signal.ActiveStrategies}) - {String.Join(" | ", signal.Reasons)}",
            If(signal.Direction = SpotBotSignalDirection.Buy, Color.LimeGreen, Color.IndianRed))
    End Sub

    Private Sub SpotTradeBot_SimulationUpdated(sender As Object, e As SpotBotSimulationEventArgs)
        If e Is Nothing OrElse e.Snapshot Is Nothing OrElse _botSimulationSummary Is Nothing Then Return

        If InvokeRequired Then
            BeginInvoke(New MethodInvoker(Sub() SpotTradeBot_SimulationUpdated(Me, e)))
            Return
        End If

        Dim nowUtc = DateTime.UtcNow
        If nowUtc - _lastBotSummaryUpdateUtc < TimeSpan.FromMilliseconds(250) AndAlso
           String.Equals(_lastBotSimulationAction, e.Snapshot.LastAction, StringComparison.Ordinal) Then
            Return
        End If

        _lastBotSummaryUpdateUtc = nowUtc
        _lastBotSimulationAction = e.Snapshot.LastAction
        Dim pnlColor = If(e.Snapshot.TotalPnl >= 0D, Color.LimeGreen, Color.IndianRed)
        _botSimulationSummary.ForeColor = pnlColor
        _botSimulationSummary.Text =
            $"Simulação | Patrimônio: {e.Snapshot.Equity:0.00} USDT | " &
            $"P/L: {e.Snapshot.TotalPnl:+0.00;-0.00;0.00} " &
            $"(realizado {e.Snapshot.RealizedPnl:+0.00;-0.00;0.00} / " &
            $"aberto {e.Snapshot.UnrealizedPnl:+0.00;-0.00;0.00}) | " &
            $"Trades: {e.Snapshot.CompletedTrades} | " &
            $"Acerto: {e.Snapshot.WinRatePercent:0.0}% | " &
            $"Abertas: {e.Snapshot.OpenPositions}"
    End Sub

    Private Sub AppendSpotBotLog(message As String, color As Color)
        If _botLog Is Nothing Then Return
        _botLog.SelectionStart = _botLog.TextLength
        _botLog.SelectionLength = 0
        _botLog.SelectionColor = color
        _botLog.AppendText(message & Environment.NewLine)
        _botLog.ScrollToCaret()
    End Sub

    Private Sub ArrangeOrdersPanel(showOrders As Boolean)
        If PanelOrdens Is Nothing OrElse PanelGraphs Is Nothing Then
            Return
        End If

        If Not showOrders Then
            PanelOrdens.Padding = New Padding(0)
            PanelOrdens.Location = PanelGraphs.Location
            PanelOrdens.Size = PanelGraphs.Size
            If _portfolioGridDefaultHeight > 0 Then
                dgPortfolio.Height = _portfolioGridDefaultHeight
            End If
            If _futuresGridDefaultHeight > 0 Then
                dgFuturos.Height = _futuresGridDefaultHeight
            End If
            Return
        End If

        Dim panelTop = Math.Max(PanelGraphs.Top, btOrdensSpot.Bottom + 2)
        Dim panelBottom = PanelProfits.Top
        Dim panelHeight = Math.Max(100, panelBottom - panelTop)
        PanelOrdens.Padding = New Padding(14, 0, 14, 0)
        PanelOrdens.Location = New Point(0, panelTop)
        PanelOrdens.Size = New Size(Me.ClientSize.Width, panelHeight)

        If _portfolioGridDefaultHeight > 0 Then
            dgPortfolio.Height = _portfolioGridDefaultHeight
        End If
        If _futuresGridDefaultHeight > 0 Then
            dgFuturos.Height = _futuresGridDefaultHeight
        End If

        btVisaoGeral.BringToFront()
        btOrdensSpot.BringToFront()
    End Sub

    Private Sub ArrangeFuturesOrdersPanel(showOrders As Boolean)
        If PanelFuturos Is Nothing OrElse PanelGraphs Is Nothing Then
            Return
        End If

        If Not showOrders Then
            PanelFuturos.Visible = False
            PanelFuturos.Padding = New Padding(0)
            Return
        End If

        Dim panelTop = Math.Max(PanelGraphs.Top, btOrdensFuturo.Bottom + 2)
        Dim panelBottom = PanelProfits.Top
        Dim panelHeight = Math.Max(100, panelBottom - panelTop)
        PanelFuturos.Padding = New Padding(14, 0, 14, 0)
        PanelFuturos.Location = New Point(0, panelTop)
        PanelFuturos.Size = New Size(Me.ClientSize.Width, panelHeight)
        PanelFuturos.Visible = True

        btVisaoGeral.BringToFront()
        btOrdensSpot.BringToFront()
        btOrdensFuturo.BringToFront()
    End Sub

    Public Sub changeOnOffColor(text As String)
        Dim palavra As String = text
        Dim pos As Integer = lbDebug.Text.IndexOf(palavra)

        If pos >= 0 Then

            lbDebug.SelectionStart = pos
            lbDebug.SelectionLength = palavra.Length

            If palavra = "Online" Then
                lbDebug.SelectionColor = Color.LimeGreen
            ElseIf palavra = "Offline" Then
                lbDebug.SelectionColor = Color.Red
            ElseIf palavra = "Pronto" Then
                lbDebug.SelectionColor = Color.LimeGreen
            End If

        End If

    End Sub

    Public Async Sub Setup()

        Try

            CheckCatalogsOnStartup()

            AddHandler _binanceWs.PriceUpdated, AddressOf BinanceWs_PriceUpdated

            AddHandler _binanceWs.ConnectionStateChanged, AddressOf BinanceWs_ConnectionStateChanged
            AddHandler _gateWs.PriceUpdated, AddressOf GateWs_PriceUpdated
            AddHandler _gateWs.ConnectionStateChanged, AddressOf GateWs_ConnectionStateChanged
            AddHandler _binanceFuturesMarketWs.MarkPriceUpdated, AddressOf BinanceFuturesMarkPriceUpdated
            AddHandler _binanceFuturesMarketWs.ConnectionStateChanged, AddressOf BinanceFuturesWs_ConnectionStateChanged
            AddHandler _binanceFuturesUserWs.DataUpdated, AddressOf BinanceFuturesUserDataUpdated
            AddHandler _binanceFuturesUserWs.ConnectionStateChanged, AddressOf BinanceFuturesWs_ConnectionStateChanged
            AddHandler _binanceSpotUserWs.DataUpdated, AddressOf BinanceSpotUserDataUpdated
            AddHandler _binanceSpotUserWs.ConnectionStateChanged, AddressOf BinanceSpotWs_ConnectionStateChanged

            Await B.SyncBinanceTime()

            Await OfferBinanceCatalogRegistrationAsync()

            chart.removeCharts()

            lbDebug.Clear()
            lbDebug.AppendText("Status: Pronto")

            changeOnOffColor("Pronto")

        Catch ex As Exception

            lbDebug.Clear()
            lbDebug.AppendText(
            "Erro ao carregar o portfólio: " &
            ex.Message)

        End Try

    End Sub

    Private Sub CheckCatalogsOnStartup()
        If _catalogWarningShown Then
            Return
        End If

        _catalogWarningShown = True

        Try
            Dim cryptoTable = PortfolioRepository.GetCryptoSymbols()
            Dim walletTable = PortfolioRepository.GetWallets()
            Dim missing As New List(Of String)()

            If cryptoTable.Rows.Count = 0 Then
                missing.Add("ativos/moedas")
            End If

            If walletTable.Rows.Count = 0 Then
                missing.Add("wallets/exchanges")
            End If

            If missing.Count > 0 Then
                MessageBox.Show(
                    "O banco SQLite ainda não possui cadastro de " &
                    String.Join(" e ", missing) & "." & Environment.NewLine &
                    "Cadastre os dados pelo menu Cadastro ou use os ativos encontrados na Binance quando forem carregados.",
                    "Cadastro necessário",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information)
            End If
        Catch ex As Exception
            Debug.WriteLine("Erro ao verificar catálogos do SQLite: " & ex.Message)
        End Try
    End Sub

    Private Async Function OfferBinanceCatalogRegistrationAsync() As Task
        If _binanceCatalogPromptRunning OrElse
           String.IsNullOrWhiteSpace(ExchangeCredentials.BinanceAPIKey) OrElse
           String.IsNullOrWhiteSpace(ExchangeCredentials.BinanceSecretKey) Then
            Return
        End If

        _binanceCatalogPromptRunning = True

        Try
            Dim assets = Await B.BINANCE_GetSpotAssetsFull()
            If assets Is Nothing OrElse assets.Count = 0 Then
                Return
            End If

            Dim walletTable = PortfolioRepository.GetWallets()
            Dim hasBinance As Boolean = walletTable.AsEnumerable().Any(
                Function(row) String.Equals(
                    row("Name")?.ToString().Trim(),
                    "BINANCE",
                    StringComparison.OrdinalIgnoreCase))

            Dim cryptoTable = PortfolioRepository.GetCryptoSymbols()
            Dim knownSymbols As New HashSet(Of String)(
                cryptoTable.AsEnumerable().Select(
                    Function(row) row("Symbol")?.ToString().Trim().ToUpperInvariant()).
                    Where(Function(symbol) Not String.IsNullOrWhiteSpace(symbol)),
                StringComparer.OrdinalIgnoreCase)

            Dim portfolioTable = PortfolioRepository.GetAll()
            Dim knownBinancePositions As New HashSet(Of String)(
                portfolioTable.AsEnumerable().Where(
                    Function(row) String.Equals(
                        row("Wallet")?.ToString().Trim(),
                        "BINANCE",
                        StringComparison.OrdinalIgnoreCase)).Select(
                    Function(row) row("Symbol")?.ToString().Trim().ToUpperInvariant()).
                    Where(Function(symbol) Not String.IsNullOrWhiteSpace(symbol)),
                StringComparer.OrdinalIgnoreCase)

            Dim ignoredBinanceAssets = PortfolioRepository.GetIgnoredBinanceAssets()

            Dim foundSymbols = assets.Where(
                Function(asset) asset.Value > 0D AndAlso
                    Not String.IsNullOrWhiteSpace(asset.Key)).
                Select(Function(asset) asset.Key.Trim().ToUpperInvariant()).
                Distinct(StringComparer.OrdinalIgnoreCase).
                OrderBy(Function(symbol) symbol, StringComparer.OrdinalIgnoreCase).
                ToList()

            Dim newSymbols = foundSymbols.Where(
                Function(symbol) Not knownSymbols.Contains(symbol) AndAlso
                    (Not ignoredBinanceAssets.Contains(symbol) OrElse
                     knownBinancePositions.Contains(symbol))).ToList()
            Dim newPositions = foundSymbols.Where(
                Function(symbol) Not knownBinancePositions.Contains(symbol) AndAlso
                    Not ignoredBinanceAssets.Contains(symbol)).ToList()

            If hasBinance AndAlso newSymbols.Count = 0 AndAlso newPositions.Count = 0 Then
                Return
            End If

            If Not hasBinance Then
                If MessageBox.Show(
                    "A conta Binance foi lida com sucesso, mas a exchange ainda não está cadastrada." &
                    Environment.NewLine & Environment.NewLine &
                    "Deseja cadastrar a wallet BINANCE no SQLite?",
                    "Cadastro Binance",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question) <> DialogResult.Yes Then
                    Return
                End If

                PortfolioRepository.AddWallet("BINANCE")
                hasBinance = True
            End If

            Dim catalogAdded As Integer = 0
            For Each symbol In newSymbols
                If knownBinancePositions.Contains(symbol) Then
                    PortfolioRepository.AddCryptoSymbol(symbol)
                    knownSymbols.Add(symbol)
                    catalogAdded += 1
                End If
            Next

            Dim currentPrices As New Dictionary(Of String, Decimal)(StringComparer.OrdinalIgnoreCase)
            Try
                Dim coinsInfo = Await B.BINANCE_GetCoinsInfo()
                If TypeOf coinsInfo Is List(Of String) Then
                    For Each line As String In DirectCast(coinsInfo, List(Of String))
                        Dim parts = line.Split("|"c)
                        If parts.Length < 2 Then Continue For

                        Dim price As Decimal
                        If Decimal.TryParse(
                            parts(1),
                            NumberStyles.Float,
                            CultureInfo.InvariantCulture,
                            price) AndAlso price > 0D Then
                            currentPrices(parts(0).Trim().ToUpperInvariant()) = price
                        End If
                    Next
                End If
            Catch ex As Exception
                Debug.WriteLine("Não foi possível obter os preços atuais dos ativos Binance: " & ex.Message)
            End Try

            Dim positionsAdded As Integer = 0
            Dim ignoredNow As Integer = 0
            Dim ptBr = CultureInfo.GetCultureInfo("pt-BR")
            For Each symbol In newPositions
                Dim quantity As Decimal = assets(symbol)
                Dim currentPrice As Decimal = 0D
                currentPrices.TryGetValue(symbol, currentPrice)

                Dim assetDetails =
                    $"O ativo {symbol} foi encontrado na conta Binance." & Environment.NewLine &
                    $"Quantidade: {quantity.ToString("N8", ptBr)}" & Environment.NewLine &
                    If(currentPrice > 0D,
                       $"Preço atual de referência: ${currentPrice.ToString("N8", CultureInfo.InvariantCulture)}",
                       "Preço atual de referência indisponível") & Environment.NewLine & Environment.NewLine &
                    "Deseja adicionar este ativo ao portfólio?" & Environment.NewLine &
                    "Escolha Não para não perguntar novamente sobre ele."

                Dim assetAnswer = MessageBox.Show(
                    assetDetails,
                    "Novo ativo Binance",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Question)

                If assetAnswer = DialogResult.Cancel Then
                    Exit For
                End If

                If assetAnswer = DialogResult.No Then
                    PortfolioRepository.IgnoreBinanceAsset(symbol)
                    ignoredNow += 1
                    Continue For
                End If

                If Not knownSymbols.Contains(symbol) Then
                    PortfolioRepository.AddCryptoSymbol(symbol)
                    knownSymbols.Add(symbol)
                    catalogAdded += 1
                End If

                Dim defaultPrice = If(
                    currentPrice > 0D,
                    currentPrice.ToString("N8", ptBr),
                    String.Empty)
                Dim averagePriceText = InputBox(
                    $"Informe o preço médio de entrada para {symbol}:" & Environment.NewLine &
                    $"Quantidade encontrada na Binance: {quantity.ToString("N8", ptBr)}" & Environment.NewLine &
                    If(currentPrice > 0D,
                       $"Preço atual de referência: ${currentPrice.ToString("N8", CultureInfo.InvariantCulture)}",
                       "Preço atual de referência indisponível"),
                    "Preço médio - Binance",
                    defaultPrice)

                Dim averagePrice As Decimal
                If Not Decimal.TryParse(averagePriceText, NumberStyles.Any, ptBr, averagePrice) AndAlso
                   Not Decimal.TryParse(averagePriceText, NumberStyles.Any, CultureInfo.InvariantCulture, averagePrice) Then
                    MessageBox.Show(
                        $"Preço médio inválido. O ativo {symbol} foi mantido no catálogo, mas não foi lançado no portfólio.",
                        "Preço médio inválido",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning)
                    Continue For
                End If

                If averagePrice <= 0D Then
                    MessageBox.Show(
                        $"O preço médio de {symbol} precisa ser maior que zero. O ativo foi mantido no catálogo, mas não foi lançado no portfólio.",
                        "Preço médio inválido",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning)
                    Continue For
                End If

                PortfolioRepository.AddOrUpdate(
                    symbol,
                    symbol,
                    averagePrice,
                    quantity,
                    Date.Today.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                    "BINANCE",
                    currentPrice)
                positionsAdded += 1
            Next

            PopulateSpotOrderSymbols()
            PopulateFuturesOrderSymbols()

            MessageBox.Show(
                "Cadastro da Binance concluído." & Environment.NewLine &
                $"Ativos adicionados ao catálogo: {catalogAdded}." & Environment.NewLine &
                $"Posições lançadas: {positionsAdded}." & Environment.NewLine &
                $"Ativos ignorados nesta sessão: {ignoredNow}.",
                "Cadastro Binance",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information)
        Catch ex As Exception
            Debug.WriteLine("Erro ao oferecer cadastro dos ativos Binance: " & ex.Message)
        Finally
            _binanceCatalogPromptRunning = False
        End Try
    End Function

    Private Async Function StartGateWebSocket() As Task

        Dim symbols As New List(Of String)

        For Each row As DataGridViewRow In dgPortfolio.Rows

            If row.IsNewRow Then Continue For

            Dim wallet As String =
            row.Cells(2).Value?.
            ToString().
            Trim().
            ToUpperInvariant()

            If wallet <> "GATE.IO" Then Continue For

            Dim symbol As String =
            row.Cells(0).Value?.
            ToString().
            Trim().
            ToUpperInvariant()

            If Not String.IsNullOrWhiteSpace(symbol) Then
                symbols.Add(symbol)
            End If

        Next

        If symbols.Count > 0 Then
            Await _gateWs.StartAsync(symbols.Distinct())
        End If

    End Function

    Private Sub UpdateGateRow(
    symbol As String,
    price As Decimal)

        Try

            For Each row As DataGridViewRow In dgPortfolio.Rows

                If row.IsNewRow Then
                    Continue For
                End If

                Dim rowSymbol As String =
                row.Cells(0).Value?.
                ToString().
                Trim().
                ToUpperInvariant()

                Dim wallet As String =
                row.Cells(2).Value?.
                ToString().
                Trim().
                ToUpperInvariant()

                If rowSymbol <> symbol.ToUpperInvariant() Then
                    Continue For
                End If

                If wallet <> "GATE.IO" Then
                    Continue For
                End If

                Dim qtd As Decimal =
                Convert.ToDecimal(row.Cells(3).Value)

                Dim precoMedio As Decimal =
                Convert.ToDecimal(row.Cells(6).Value)

                Dim valorEntradaUSD As Decimal =
                qtd * precoMedio

                Dim valorAtualUSD As Decimal =
                qtd * price

                Dim roiUSD As Decimal =
                valorAtualUSD - valorEntradaUSD

                Dim performance As Decimal = 0D

                If valorEntradaUSD > 0D Then

                    performance =
                    (roiUSD / valorEntradaUSD) * 100D

                End If

                Dim usdBrl As Decimal =
                Cjson.USDBRLprice

                Dim valorAtualBRL As Decimal =
                valorAtualUSD * usdBrl

                Dim roiBRL As Decimal =
                roiUSD * usdBrl

                Dim x As Decimal = 0D

                If valorEntradaUSD > 0D Then
                    x = valorAtualUSD / valorEntradaUSD
                End If

                ' Preço
                row.Cells(7).Value = price

                ' Performance
                row.Cells(1).Value =
                $"{performance:F2}%"

                ' Valor atual USD
                row.Cells(10).Value =
                valorAtualUSD

                ' Valor atual BRL
                row.Cells(11).Value =
                valorAtualBRL

                ' ROI USD
                row.Cells(12).Value =
                roiUSD

                ' ROI BRL
                row.Cells(13).Value =
                roiBRL

                ' Multiplicador: a coluna X original permanece oculta.
                If row.DataGridView IsNot Nothing AndAlso row.DataGridView.Columns.Contains("X") Then
                    row.Cells("X").Value = $"{x:N2} X"
                End If

                ApplyPortfolioRealtimeRowStyle(row)

                Exit For

            Next

            ' O mesmo recálculo usado pela Binance
            UpdateRealtimeOverview()

        Catch ex As Exception

            Debug.WriteLine(
            "[GATE WS] Erro atualizando " &
            symbol &
            ": " &
            ex.Message)

        End Try

    End Sub

    Private Sub GateWs_ConnectionStateChanged(
    connected As Boolean,
    message As String)

        Debug.WriteLine(
        "[GATE WS] " &
        message)

    End Sub

    Private Sub GateWs_PriceUpdated(
    symbol As String,
    price As Decimal)

        If Me.InvokeRequired Then

            Me.BeginInvoke(
            New Action(
                Sub()
                    UpdateGateRow(symbol, price)
                End Sub))

            Return

        End If

        UpdateGateRow(symbol, price)

    End Sub

    Private Sub BinanceWs_ConnectionStateChanged(
    connected As Boolean,
    message As String)

        If Me.InvokeRequired Then

            Me.BeginInvoke(
            New Action(
                Sub()
                    BinanceWs_ConnectionStateChanged(
                        connected,
                        message)
                End Sub))

            Return

        End If

        Debug.WriteLine(message)

        If connected Then

            lbDebug.Clear()
            lbDebug.AppendText("Status: Online")
            changeOnOffColor("Online")

        Else

            lbDebug.AppendText(
            Environment.NewLine & message)

        End If

    End Sub

    Private Async Function StartBinanceWebSocket() As Task
        If _binanceWsStartRunning Then
            Return
        End If

        _binanceWsStartRunning = True

        Try

            Await StartBinanceSpotUserWebSocket()

            Dim symbols As New List(Of String)

            Debug.WriteLine(
    "[WS] Símbolos Binance: " &
    String.Join(", ", symbols))

            For Each row As DataGridViewRow In dgPortfolio.Rows

                If row.IsNewRow Then
                    Continue For
                End If

                Dim wallet As String =
                row.Cells(2).Value?.ToString().Trim().ToUpperInvariant()

                If wallet <> "BINANCE" Then
                    Continue For
                End If

                Dim symbol As String =
                row.Cells(0).Value?.ToString().Trim().ToUpperInvariant()

                If String.IsNullOrWhiteSpace(symbol) Then
                    Continue For
                End If

                symbols.Add(symbol)

            Next

            If _cbSpotOrderSymbol IsNot Nothing Then
                Dim selectedSymbol = GetSpotBaseAsset(_cbSpotOrderSymbol.Text)
                If Not String.IsNullOrWhiteSpace(selectedSymbol) AndAlso
                   Not Cjson.stablecoins.Contains(selectedSymbol) Then
                    symbols.Add(selectedSymbol)
                End If
            End If

            If _dgOrdensSpot IsNot Nothing AndAlso
               _dgOrdensSpot.Columns.Contains("Symbol") Then
                For Each orderRow As DataGridViewRow In _dgOrdensSpot.Rows
                    If orderRow.IsNewRow Then
                        Continue For
                    End If

                    Dim orderSymbol = GetSpotBaseAsset(orderRow.Cells("Symbol").Value?.ToString())
                    If Not String.IsNullOrWhiteSpace(orderSymbol) AndAlso
                       Not Cjson.stablecoins.Contains(orderSymbol) Then
                        symbols.Add(orderSymbol)
                    End If
                Next
            End If

            symbols =
            symbols.Distinct().ToList()

            If symbols.Count = 0 Then

                Debug.WriteLine(
                "Nenhum ativo Binance encontrado no DataGrid.")

                Return

            End If

            Await _binanceWs.StartAsync(symbols)

        Catch ex As Exception

            Debug.WriteLine(
            "Erro iniciando Binance WebSocket: " &
            ex.Message)

        Finally
            _binanceWsStartRunning = False
        End Try

    End Function

    Private Async Function StartBinanceSpotUserWebSocket() As Task
        If _spotUserStreamStarted OrElse
           String.IsNullOrWhiteSpace(ExchangeCredentials.BinanceAPIKey) Then
            Return
        End If

        Try
            Await _binanceSpotUserWs.StartAsync()
            _spotUserStreamStarted = True
        Catch ex As Exception
            Debug.WriteLine("Erro iniciando Binance Spot User Data: " & ex.Message)
        End Try
    End Function

    Private Sub ApplyPortfolioRealtimeRowStyle(row As DataGridViewRow)
        If row Is Nothing OrElse row.IsNewRow OrElse
           Not dgPortfolio.Columns.Contains("precoMedio") OrElse
           Not dgPortfolio.Columns.Contains("precoAtual") Then
            Return
        End If

        Dim averagePrice As Decimal
        Dim currentPrice As Decimal
        If Not TryReadFuturesDecimal(row.Cells("precoMedio").Value, averagePrice) OrElse
           Not TryReadFuturesDecimal(row.Cells("precoAtual").Value, currentPrice) Then
            Return
        End If

        Dim isLoss = currentPrice < averagePrice
        Dim rowBackColor = If(isLoss, Color.FromArgb(25, 0, 0), Color.FromArgb(0, 25, 0))
        Dim currentColor = If(isLoss, Color.IndianRed, Color.LimeGreen)

        Dim portfolioColumns As String() = {
            "Cripto", "Perf", "Wallet", "Qtd", "vlEntradaUSD", "vlEntradaBRL",
            "precoMedio", "precoAtual", "24horas", "marketcap", "vlAtualUSD",
            "vlAtualBRL", "ROIusd", "ROIbrl"
        }

        For Each columnName In portfolioColumns
            If dgPortfolio.Columns.Contains(columnName) Then
                row.Cells(columnName).Style.BackColor = rowBackColor
            End If
        Next

        For Each columnName In {"precoAtual", "vlAtualUSD", "vlAtualBRL"}
            If dgPortfolio.Columns.Contains(columnName) Then
                row.Cells(columnName).Style.ForeColor = currentColor
            End If
        Next

        Dim roiUsd As Decimal
        If dgPortfolio.Columns.Contains("ROIusd") AndAlso
           TryReadFuturesDecimal(row.Cells("ROIusd").Value, roiUsd) Then
            row.Cells("ROIusd").Style.ForeColor = If(roiUsd < 0D, Color.LightCoral, Color.Aquamarine)
        End If

        Dim roiBrl As Decimal
        If dgPortfolio.Columns.Contains("ROIbrl") AndAlso
           TryReadFuturesDecimal(row.Cells("ROIbrl").Value, roiBrl) Then
            row.Cells("ROIbrl").Style.ForeColor = If(roiBrl < 0D, Color.LightCoral, Color.Aqua)
            If dgPortfolio.Columns.Contains("Cripto") Then
                row.Cells("Cripto").Style.ForeColor = If(roiBrl < 0D, Color.LightCoral, Color.Lime)
            End If
        End If

        If dgPortfolio.Columns.Contains("Perf") Then
            Dim performanceText = row.Cells("Perf").Value?.ToString().Replace("%", "").Trim()
            Dim performance As Decimal
            If Decimal.TryParse(performanceText, NumberStyles.Any, CultureInfo.InvariantCulture, performance) OrElse
               Decimal.TryParse(performanceText, NumberStyles.Any, CultureInfo.GetCultureInfo("pt-BR"), performance) Then
                row.Cells("Perf").Style.ForeColor = If(performance < 0D, Color.LightCoral, Color.LightGreen)
            End If
        End If

        If dgPortfolio.Columns.Contains("Wallet") Then
            Select Case row.Cells("Wallet").Value?.ToString().Trim().ToUpperInvariant()
                Case "BINANCE"
                    row.Cells("Wallet").Style.ForeColor = Color.Goldenrod
                Case "METAMASK"
                    row.Cells("Wallet").Style.ForeColor = Color.DarkOrange
                Case "TRUSTWALLET"
                    row.Cells("Wallet").Style.ForeColor = Color.LawnGreen
                Case "PHANTOM"
                    row.Cells("Wallet").Style.ForeColor = Color.MediumPurple
                Case "BYBIT"
                    row.Cells("Wallet").Style.ForeColor = Color.Gainsboro
                Case "GATE.IO"
                    row.Cells("Wallet").Style.ForeColor = Color.DodgerBlue
                Case "MEXC"
                    row.Cells("Wallet").Style.ForeColor = Color.White
            End Select
        End If

        If dgPortfolio.Columns.Contains("SpotExitPrice") Then
            row.Cells("SpotExitPrice").Style.BackColor = rowBackColor
            row.Cells("SpotExitPrice").Style.ForeColor = Color.Gold
            row.Cells("SpotExitPrice").Style.SelectionBackColor = rowBackColor
            row.Cells("SpotExitPrice").Style.SelectionForeColor = Color.Gold
        End If

        If dgPortfolio.Columns.Contains("SpotClose") Then
            row.Cells("SpotClose").Style.BackColor = Color.IndianRed
            row.Cells("SpotClose").Style.ForeColor = Color.White
            row.Cells("SpotClose").Style.SelectionBackColor = Color.IndianRed
            row.Cells("SpotClose").Style.SelectionForeColor = Color.White
        End If
    End Sub

    Private Sub UpdateRealtimeOverview(Optional force As Boolean = False)

        Try
            If _marketRefreshRunning AndAlso Not force Then
                Return
            End If

            Dim totalEntradaUSD As Decimal = 0D
            Dim totalAtualUSD As Decimal = 0D

            Dim cashflowUSD As Decimal = 0D
            Dim investidoUSD As Decimal = 0D
            Dim coinEntryUSD As Decimal = 0D

            Dim lucroUSD As Decimal = 0D

            For Each row As DataGridViewRow In dgPortfolio.Rows

                If row.IsNewRow Then
                    Continue For
                End If

                Dim wallet As String =
                row.Cells(2).Value?.
                ToString().
                Trim().
                ToUpperInvariant()

                Dim entrada As Decimal =
                Convert.ToDecimal(
                    row.Cells(4).Value)

                Dim atual As Decimal =
                Convert.ToDecimal(
                    row.Cells(10).Value)

                Dim symbol As String =
                row.Cells(0).Value?.
                ToString().
                Trim().
                ToUpperInvariant()

                totalEntradaUSD += entrada
                totalAtualUSD += atual

                If Cjson.stablecoins.Contains(symbol) Then

                    cashflowUSD += atual

                Else

                    investidoUSD += atual
                    coinEntryUSD += entrada
                    lucroUSD +=
                    atual - entrada

                End If

                ApplyPortfolioRealtimeRowStyle(row)

            Next

            If _spotCashLoadedFromAccount Then
                cashflowUSD = _spotCashUsd
            Else
                _spotCashUsd = cashflowUSD
            End If

            ' =============================================
            ' TOTAL
            ' =============================================
            Dim usdBrl As Decimal =
            Cjson.USDBRLprice

            Dim totalBRL As Decimal =
            totalAtualUSD * usdBrl

            Dim percentualCaixa As Decimal = 0D
            Dim percentualInvestido As Decimal = 0D
            Dim performanceWallet As Decimal = 0D

            If totalAtualUSD > 0D Then

                percentualCaixa =
                (cashflowUSD / totalAtualUSD) * 100D

                percentualInvestido =
                (investidoUSD / totalAtualUSD) * 100D

            End If

            If totalEntradaUSD > 0D Then

                performanceWallet =
                (lucroUSD / totalEntradaUSD) * 100D

            End If

            ' =============================================
            ' UI
            ' =============================================
            _spotPnlUsd = lucroUSD
            _spotCurrentUsd = cashflowUSD + investidoUSD
            _spotEntryUsd = totalEntradaUSD
            _spotCoinEntryUsd = coinEntryUSD
            _spotOverviewInitialized = True
            UpdateSpotOrderCashLabel()

            UpdateGrossInvestmentSummary()

            ' O rótulo Spot representa somente os ativos investidos atuais.
            ' Caixa fica separado no lbCaixa; o valor de entrada permanece
            ' em _spotEntryUsd para calcular o desempenho.
            Me.lbTotalEntradaUSD.Text =
            Cjson.USDformat(investidoUSD)

            Me.lbTotalEntradaBRL.Text =
            Cjson.BRLformat(
                investidoUSD * usdBrl)

            Me.lbValoresHojeUSD.Text =
            Cjson.USDformat(totalAtualUSD)

            Me.lbValoresHojeBRL.Text =
            Cjson.BRLformat(totalBRL)

            Me.lbRoiUSD.Text =
            Cjson.USDformat(lucroUSD)

            ' Mantém as regras visuais sincronizadas com os valores recalculados pelo WebSocket.
            Me.lbTotalEntradaUSD.ForeColor = Color.LimeGreen
            Me.lbTotalEntradaBRL.ForeColor = Color.DeepSkyBlue

            Me.lbValoresHojeUSD.ForeColor =
            If(totalAtualUSD < _grossInvestmentUsd,
               Color.IndianRed,
               Color.GreenYellow)

            Me.lbValoresHojeBRL.ForeColor =
            If(totalBRL < _grossInvestmentBrl,
               Color.IndianRed,
               Color.Cyan)

            Me.lbRoiUSD.ForeColor =
            If(lucroUSD < 0D,
               Color.Red,
               Color.Gold)

            Me.lbCaixa.Text =
            Cjson.USDformat(cashflowUSD)

            Me.lbCaixaBRL.Text =
            Cjson.BRLformat(
                cashflowUSD * usdBrl)

            Me.lbPercentCaixa.Text =
            $"{percentualCaixa:F2}%"

            Me.lbPercentInvestido.Text =
            $"{percentualInvestido:F2}%"

            Me.lbPerformWallet.Text =
            $"{performanceWallet:F2}%"

            Me.lbRoiUSD.ForeColor =
            If(
                lucroUSD < 0D,
                Color.Red,
                Color.Gold)

            Me.lbPerformWallet.ForeColor =
            If(
                performanceWallet < 0D,
                Color.Red,
                Color.Lime)

            UpdateFuturesPnlSummary()
            ArrangeProfitPairs()

        Catch ex As Exception

            Debug.WriteLine(
            "Erro atualizando visão geral realtime: " &
            ex.Message)

        End Try

    End Sub

    Private Sub UpdateBinanceRow(
    symbol As String,
    price As Decimal)

        Try

            For Each row As DataGridViewRow In dgPortfolio.Rows

                If row.IsNewRow Then
                    Continue For
                End If

                Dim rowSymbol As String =
                row.Cells(0).Value?.
                ToString().
                Trim().
                ToUpperInvariant()

                Dim wallet As String =
                row.Cells(2).Value?.
                ToString().
                Trim().
                ToUpperInvariant()

                If rowSymbol <> symbol.ToUpperInvariant() Then
                    Continue For
                End If

                If wallet <> "BINANCE" Then
                    Continue For
                End If

                ' =============================================
                ' QUANTIDADE
                ' =============================================
                Dim qtd As Decimal =
                Convert.ToDecimal(
                    row.Cells(3).Value)

                ' =============================================
                ' PREÇO MÉDIO
                ' =============================================
                Dim precoMedio As Decimal =
                Convert.ToDecimal(
                    row.Cells(6).Value)

                ' =============================================
                ' VALOR ATUAL USD
                ' =============================================
                Dim valorAtualUSD As Decimal =
                qtd * price

                ' =============================================
                ' ROI USD
                ' =============================================
                Dim valorEntradaUSD As Decimal =
                qtd * precoMedio

                Dim roiUSD As Decimal =
                valorAtualUSD - valorEntradaUSD

                Dim performance As Decimal = 0D

                If valorEntradaUSD > 0D Then

                    performance =
                    (roiUSD / valorEntradaUSD) * 100D

                End If

                ' =============================================
                ' USD → BRL
                ' =============================================
                Dim usdBrl As Decimal =
                Cjson.USDBRLprice

                Dim valorAtualBRL As Decimal =
                valorAtualUSD * usdBrl

                Dim roiBRL As Decimal =
                roiUSD * usdBrl

                ' =============================================
                ' MULTIPLICADOR
                ' =============================================
                Dim x As Decimal = 0D

                If valorEntradaUSD > 0D Then

                    x =
                    valorAtualUSD / valorEntradaUSD

                End If

                ' =============================================
                ' ATUALIZA GRID
                ' =============================================
                row.Cells(7).Value =
                price

                row.Cells(10).Value =
                valorAtualUSD

                row.Cells(11).Value =
                valorAtualBRL

                row.Cells(12).Value =
                roiUSD

                row.Cells(13).Value =
                roiBRL

                row.Cells(1).Value =
                $"{performance:F2}%"

                If row.DataGridView IsNot Nothing AndAlso row.DataGridView.Columns.Contains("X") Then
                    If x > 0D Then
                        row.Cells("X").Value = $"{x:N2} X"
                    Else
                        row.Cells("X").Value = "0 X"
                    End If
                End If

                ApplyPortfolioRealtimeRowStyle(row)

                Exit For

            Next

            ' Recalcula visão geral
            UpdateRealtimeOverview()

            ' A formatação do grid ocorre no carregamento/ordenação; não a repetimos a cada tick.
            ' Isso evita alterar visibilidade e binding durante o update do WebSocket.

        Catch ex As Exception

            Debug.WriteLine(
            "Erro atualizando preço realtime [" &
            symbol &
            "]: " &
            ex.Message)

        End Try

    End Sub

    Public Sub BinanceWs_PriceUpdated(
    symbol As String,
    price As Decimal)

        Debug.WriteLine(
        $"[WS RECEBIDO] {symbol} = {price.ToString(CultureInfo.InvariantCulture)}")

        If Me.InvokeRequired Then

            Me.BeginInvoke(
            New Action(
                Sub()
                    UpdateBinanceRow(symbol, price)
                    UpdateSpotOrdersCurrentPrice(symbol, price)
                    UpdateSpotOrderCurrentPrice()
                    ProcessSpotBotPrice(symbol, price)
                End Sub))

            Return

        End If

        UpdateBinanceRow(symbol, price)
        UpdateSpotOrdersCurrentPrice(symbol, price)
        UpdateSpotOrderCurrentPrice()
        ProcessSpotBotPrice(symbol, price)

    End Sub

    Private Sub ProcessSpotBotPrice(symbol As String, price As Decimal)
        If Not _spotTradeBot.IsRunning Then Return
        _spotTradeBot.ProcessPrice(symbol, price)
    End Sub

    Private Sub dgPortfolio_MouseLeave(sender As Object, e As EventArgs) Handles dgPortfolio.MouseLeave
        dgPortfolio.ClearSelection()
        dgPortfolio.CurrentCell = Nothing
        dgPortfolio.Cursor = Cursors.Default
    End Sub

    Private Shared Sub UpdateCurrencyColumnCursor(
        grid As DataGridView,
        rowIndex As Integer,
        columnIndex As Integer,
        columnName As String,
        Optional secondaryColumnName As String = Nothing)

        If rowIndex >= 0 AndAlso columnIndex >= 0 AndAlso
           (String.Equals(grid.Columns(columnIndex).Name, columnName, StringComparison.OrdinalIgnoreCase) OrElse
            (Not String.IsNullOrWhiteSpace(secondaryColumnName) AndAlso
             String.Equals(grid.Columns(columnIndex).Name, secondaryColumnName, StringComparison.OrdinalIgnoreCase))) Then
            grid.Cursor = Cursors.Hand
        Else
            grid.Cursor = Cursors.Default
        End If
    End Sub

    Private Sub dgPortfolio_CellMouseMove(sender As Object, e As DataGridViewCellMouseEventArgs) Handles dgPortfolio.CellMouseMove
        UpdateCurrencyColumnCursor(dgPortfolio, e.RowIndex, e.ColumnIndex, "Cripto", "SpotClose")
    End Sub

    Private Sub dgPortfolio_CellMouseEnter(sender As Object, e As DataGridViewCellEventArgs) Handles dgPortfolio.CellMouseEnter
        SetCloseButtonHoverStyle(dgPortfolio, e.RowIndex, e.ColumnIndex, "SpotClose", isHover:=True)
    End Sub

    Private Sub dgPortfolio_CellMouseLeave(sender As Object, e As DataGridViewCellEventArgs) Handles dgPortfolio.CellMouseLeave
        SetCloseButtonHoverStyle(dgPortfolio, e.RowIndex, e.ColumnIndex, "SpotClose", isHover:=False)
    End Sub

    Private Sub dgFuturos_CellMouseMove(sender As Object, e As DataGridViewCellMouseEventArgs) Handles dgFuturos.CellMouseMove
        UpdateCurrencyColumnCursor(dgFuturos, e.RowIndex, e.ColumnIndex, "Symbol", "FuturesClose")
    End Sub

    Private Sub dgFuturos_CellPainting(sender As Object, e As DataGridViewCellPaintingEventArgs) Handles dgFuturos.CellPainting
        PaintCloseButtonCell(dgFuturos, e, "FuturesClose")
    End Sub

    Private Sub dgFuturos_CellMouseEnter(sender As Object, e As DataGridViewCellEventArgs) Handles dgFuturos.CellMouseEnter
        SetCloseButtonHoverStyle(dgFuturos, e.RowIndex, e.ColumnIndex, "FuturesClose", isHover:=True)
    End Sub

    Private Sub dgFuturos_CellMouseLeave(sender As Object, e As DataGridViewCellEventArgs) Handles dgFuturos.CellMouseLeave
        SetCloseButtonHoverStyle(dgFuturos, e.RowIndex, e.ColumnIndex, "FuturesClose", isHover:=False)
    End Sub

    Private Shared Sub SetCloseButtonHoverStyle(
        grid As DataGridView,
        rowIndex As Integer,
        columnIndex As Integer,
        columnName As String,
        isHover As Boolean)

        If rowIndex < 0 OrElse columnIndex < 0 OrElse
           rowIndex >= grid.Rows.Count OrElse
           columnIndex >= grid.Columns.Count OrElse
           Not String.Equals(grid.Columns(columnIndex).Name, columnName, StringComparison.OrdinalIgnoreCase) Then
            Return
        End If

        Dim cell = grid.Rows(rowIndex).Cells(columnIndex)
        Dim hoverColor = If(isHover, Color.Red, Color.IndianRed)
        cell.Style.BackColor = hoverColor
        cell.Style.SelectionBackColor = hoverColor
        grid.InvalidateCell(columnIndex, rowIndex)
    End Sub

    Private Shared Sub PaintCloseButtonCell(
        grid As DataGridView,
        e As DataGridViewCellPaintingEventArgs,
        columnName As String)

        If e.RowIndex < 0 OrElse e.ColumnIndex < 0 OrElse
           e.ColumnIndex >= grid.Columns.Count OrElse
           Not String.Equals(grid.Columns(e.ColumnIndex).Name, columnName, StringComparison.OrdinalIgnoreCase) Then
            Return
        End If

        Dim clientPoint = grid.PointToClient(System.Windows.Forms.Cursor.Position)
        Dim hit = grid.HitTest(clientPoint.X, clientPoint.Y)
        Dim isHover = hit.RowIndex = e.RowIndex AndAlso hit.ColumnIndex = e.ColumnIndex
        Dim buttonColor = If(isHover, Color.Red, Color.IndianRed)
        Dim buttonBounds = New Rectangle(
            e.CellBounds.X + 1,
            e.CellBounds.Y + 1,
            Math.Max(0, e.CellBounds.Width - 2),
            Math.Max(0, e.CellBounds.Height - 2))

        e.PaintBackground(e.CellBounds, True)
        Using brush As New SolidBrush(buttonColor)
            e.Graphics.FillRectangle(brush, buttonBounds)
        End Using
        TextRenderer.DrawText(
            e.Graphics,
            "X",
            grid.Font,
            buttonBounds,
            Color.White,
            TextFormatFlags.HorizontalCenter Or TextFormatFlags.VerticalCenter Or TextFormatFlags.NoPadding)
        e.Paint(e.CellBounds, DataGridViewPaintParts.Border)
        e.Handled = True
    End Sub

    Private Sub dgFuturos_MouseLeave(sender As Object, e As EventArgs) Handles dgFuturos.MouseLeave
        dgFuturos.Cursor = Cursors.Default
    End Sub

    Private Sub FormMain_Resize(sender As Object, e As EventArgs) Handles MyBase.Resize
        Dim json As New JSON
        Try
            json.FormatGrid(dgPortfolio)
            ConfigurePortfolioCloseColumns()

            If Me.WindowState = FormWindowState.Minimized Then
                Me.Hide()
                NotifyIcon1.Visible = True
                ' NotifyIcon1.ShowBalloonTip(3000, "Porfólio Cripto", lbBTC.Text, ToolTipIcon.Info)
            End If

            Adjust()

            If PanelFuturos.Visible Then
                ArrangeFuturesOrdersPanel(True)
            End If

            ArrangeSpotBotButton()
            If _panelSpotBot IsNot Nothing AndAlso _panelSpotBot.Visible Then
                ArrangeSpotBotPanel(True)
            End If

            Me.CenterToScreen()

        Catch ex As Exception

        End Try

    End Sub

    Public Async Function refreshMarket() As Task(Of Boolean)
        Try
            If _marketRefreshRunning Then
                Return False
            End If

            _marketRefreshRunning = True
            _spotCashLoadedFromAccount = False

            chart.removeCharts()
            lbLoadFromMarket.Visible = True
            TimerBlink.Start()
            Cursor = Cursors.WaitCursor
            dgPortfolio.Cursor = Cursors.WaitCursor

            If Await PortfolioMarketService.LoadAsync(dgPortfolio) Then
                ConfigurePortfolioCloseColumns()
                UpdateRealtimeOverview(force:=True)
                Await StartBinanceWebSocket()
                Await StartGateWebSocket()
                dgPortfolio.Sort(dgPortfolio.Columns("ROIusd"), System.ComponentModel.ListSortDirection.Descending)
                HideStablecoinPortfolioRows()
                Adjust()
            Else
                lbDebug.AppendText("Status: Erro ao carregar o portfólio.")
            End If

            ' O refresh do Spot também precisa atualizar a conta Futures,
            ' pois o resumo usa saldo disponível + margem reservada.
            Await LoadFuturesPositionsAsync()

            If TimerRefresh.Enabled = False Then
                lbAtualizaEm.Text = "Atualizado em:"
                lbRefresh.Location = New Point(165, 9)
                lbRefresh.Text = My.Settings.lastView
            End If

            'TimerCountdown.Stop()
            'TimerRefresh.Stop()

            Return True

        Catch ex As Exception
            lbDebug.Clear()
            lbDebug.AppendText("Offline - Erro ao atualizar o mercado: " & ex.Message)
            changeOnOffColor("Offline")
            JSON.hideMarketDataLabel()
            Return False

        Finally
            _marketRefreshRunning = False
            Cursor = Cursors.Default
            dgPortfolio.Cursor = Cursors.Default
        End Try

    End Function

    Private Async Sub btRefresh_Click_1Async(sender As Object, e As EventArgs) Handles btRefresh.Click
        If PanelOrdens.Visible Then
            Await LoadSpotOrdersAsync()
        End If
        If dgFuturos.Visible Then
            'BinanceFuturesUserDataUpdated()
            Await LoadFuturesPositionsAsync()
        End If
        If dgPortfolio.Visible Then
            Await refreshMarket()
        End If
    End Sub

    Private Sub dgPortfolio_Sorted(sender As Object, e As EventArgs) Handles dgPortfolio.Sorted
        Dim json As New JSON
        Try
            json.FormatGrid(dgPortfolio)
            ConfigurePortfolioCloseColumns()
            EnsurePortfolioQuantityColumnVisible()
            HideStablecoinPortfolioRows()
        Catch ex As Exception

        End Try
    End Sub

    Private Sub dgPortfolio_DataBindingComplete(
        sender As Object,
        e As DataGridViewBindingCompleteEventArgs) Handles dgPortfolio.DataBindingComplete
        EnsurePortfolioQuantityColumnVisible()
        HideStablecoinPortfolioRows()
    End Sub

    Private Sub EnsurePortfolioQuantityColumnVisible()
        If dgPortfolio Is Nothing OrElse
           Not dgPortfolio.Columns.Contains("Qtd") Then
            Return
        End If

        ' O DataGridView mantém as colunas auxiliares de fechamento entre as
        ' trocas de DataSource. Usar índices fixos nessas trocas pode acabar
        ' ocultando a coluna Qtd quando a ordem interna muda. Reforce o estado
        ' pelo nome da coluna, que permanece estável.
        Dim quantityColumn = dgPortfolio.Columns("Qtd")
        quantityColumn.Visible = True
        quantityColumn.HeaderText = "Qtd"
        quantityColumn.Width = 95
    End Sub

    Public Sub ConfigurePortfolioCloseColumns()
        If Not dgPortfolio.Columns.Contains("X") AndAlso
           Not dgPortfolio.Columns.Contains("SpotExitPrice") Then
            Return
        End If

        ' A coluna antiga guardava apenas o multiplicador visual. Mantemos o
        ' dado no DataTable, mas retiramos sua exibição para usar esta área
        ' com o preço limite e o fechamento da posição.
        Dim oldXIndex As Integer = dgPortfolio.Columns.Count
        If dgPortfolio.Columns.Contains("X") Then
            oldXIndex = dgPortfolio.Columns("X").Index
            dgPortfolio.Columns("X").Visible = False
        End If

        If Not dgPortfolio.Columns.Contains("SpotExitPrice") Then
            Dim priceColumn As New DataGridViewTextBoxColumn With {
                .Name = "SpotExitPrice",
                .HeaderText = "Preço limite",
                .AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                .Width = 92,
                .SortMode = DataGridViewColumnSortMode.NotSortable,
                .ReadOnly = False
            }
            priceColumn.DefaultCellStyle = New DataGridViewCellStyle With {
                .Alignment = DataGridViewContentAlignment.MiddleRight,
                .BackColor = Color.FromArgb(64, 64, 64),
                .ForeColor = Color.Gold,
                .SelectionBackColor = Color.White,
                .SelectionForeColor = Color.Black
            }
            dgPortfolio.Columns.Insert(Math.Min(oldXIndex, dgPortfolio.Columns.Count), priceColumn)
        End If

        If Not dgPortfolio.Columns.Contains("SpotClose") Then
            Dim closeColumn As New DataGridViewButtonColumn With {
                .Name = "SpotClose",
                .HeaderText = "Fechar",
                .Text = "X",
                .UseColumnTextForButtonValue = True,
                .AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                .Width = 38,
                .FlatStyle = FlatStyle.Flat,
                .ReadOnly = True
            }
            closeColumn.DefaultCellStyle = New DataGridViewCellStyle With {
                .Alignment = DataGridViewContentAlignment.MiddleCenter,
                .BackColor = Color.IndianRed,
                .ForeColor = Color.White,
                .SelectionBackColor = Color.IndianRed,
                .SelectionForeColor = Color.White
            }
            Dim closeIndex As Integer = If(dgPortfolio.Columns.Contains("X"), dgPortfolio.Columns("X").Index, dgPortfolio.Columns.Count)
            dgPortfolio.Columns.Insert(Math.Min(closeIndex, dgPortfolio.Columns.Count), closeColumn)
        End If

        Dim priceColumnReady = TryCast(dgPortfolio.Columns("SpotExitPrice"), DataGridViewTextBoxColumn)
        If priceColumnReady IsNot Nothing Then
            priceColumnReady.HeaderText = "Preço limite"
            priceColumnReady.Width = 92
            priceColumnReady.Visible = True
            priceColumnReady.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
        End If

        Dim closeColumnReady = TryCast(dgPortfolio.Columns("SpotClose"), DataGridViewButtonColumn)
        If closeColumnReady IsNot Nothing Then
            closeColumnReady.HeaderText = "Fechar"
            closeColumnReady.Text = "X"
            closeColumnReady.UseColumnTextForButtonValue = True
            closeColumnReady.Width = 58
            closeColumnReady.Visible = True
        End If

        dgPortfolio.ReadOnly = False
        dgPortfolio.EditMode = DataGridViewEditMode.EditOnEnter
        For Each column As DataGridViewColumn In dgPortfolio.Columns
            column.ReadOnly = Not String.Equals(column.Name, "SpotExitPrice", StringComparison.OrdinalIgnoreCase)
        Next

        For Each row As DataGridViewRow In dgPortfolio.Rows
            If row.IsNewRow Then Continue For

            If dgPortfolio.Columns.Contains("SpotExitPrice") Then
                row.Cells("SpotExitPrice").Style.Alignment = DataGridViewContentAlignment.MiddleRight
                row.Cells("SpotExitPrice").Style.BackColor = Color.FromArgb(64, 64, 64)
                row.Cells("SpotExitPrice").Style.ForeColor = Color.Gold
                row.Cells("SpotExitPrice").Style.SelectionBackColor = Color.White
                row.Cells("SpotExitPrice").Style.SelectionForeColor = Color.Black
            End If

            If dgPortfolio.Columns.Contains("SpotClose") Then
                row.Cells("SpotClose").Style.BackColor = Color.IndianRed
                row.Cells("SpotClose").Style.ForeColor = Color.White
                row.Cells("SpotClose").Style.SelectionBackColor = Color.IndianRed
                row.Cells("SpotClose").Style.SelectionForeColor = Color.White
            End If

            ApplyPortfolioRealtimeRowStyle(row)
        Next
    End Sub

    Private Sub NotifyIcon1_MouseClick(sender As Object, e As MouseEventArgs) Handles NotifyIcon1.MouseClick
        Me.Show()
        Me.WindowState = FormWindowState.Normal
        NotifyIcon1.Visible = False
        Me.CenterToScreen()
    End Sub

    Private Sub IntervaloToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles IntervaloToolStripMenuItem.Click
        FormIntervalo.ShowDialog()
    End Sub

    Private Async Sub Timer1_TickAsync(sender As Object, e As EventArgs) Handles TimerRefresh.Tick
        Dim json As New JSON
        Try

            Me.remainingtimeInSeconds = TimerRefresh.Interval / 1000
            chart.removeCharts()
            lbLoadFromMarket.Visible = True
            TimerBlink.Start()

            Cursor = Cursors.WaitCursor
            dgPortfolio.Cursor = Cursors.WaitCursor
            Await PortfolioMarketService.LoadAsync(dgPortfolio)
            ConfigurePortfolioCloseColumns()
            UpdateRealtimeOverview(force:=True)
            Await LoadFuturesPositionsAsync()
            Await StartBinanceSpotUserWebSocket()
            dgPortfolio.Sort(dgPortfolio.Columns("ROIusd"), System.ComponentModel.ListSortDirection.Descending)
            HideStablecoinPortfolioRows()
            Adjust()

            lbAtualizaEm.Text = "Atualizado em:"
            lbRefresh.Text = My.Settings.lastView
            lbRefresh.Location = New Point(125, 7)
            json.FormatGrid(dgPortfolio)
            ConfigurePortfolioCloseColumns()
            HideStablecoinPortfolioRows()
        Catch ex As Exception
            Debug.WriteLine(
            "[TIMER] Erro ao atualizar mercado: " &
            ex.ToString())
        Finally
            Cursor = Cursors.Default
            dgPortfolio.Cursor = Cursors.Default
        End Try

    End Sub
    Private Sub TimerCountdown_Tick(sender As Object, e As EventArgs) Handles TimerCountdown.Tick
        remainingtimeInSeconds -= 1
        lbAtualizaEm.Text = "Atualiza em:"
        lbRefresh.Location = New Point(150, 9)
        'lbRefresh.Text = $"{(remainingtimeInSeconds \ 60).ToString("D2")}:{(remainingtimeInSeconds Mod 60).ToString("D2")}"
        Dim ts As TimeSpan = TimeSpan.FromSeconds(remainingtimeInSeconds)
        lbRefresh.Text = $"{Math.Floor(ts.TotalHours):00}:{ts.Minutes:00}:{ts.Seconds:00}"
    End Sub

    Private Sub NotifyIcon1_MouseMove(sender As Object, e As MouseEventArgs) Handles NotifyIcon1.MouseMove
        NotifyIcon1.Text = "BTC: " & lbBTC.Text
    End Sub

    Private Sub Adjust()
        PanelGraphs.Width = Me.Width
        Const gridRightMargin As Integer = 8

        If Me.ClientSize.Width > dgPortfolio.Left + gridRightMargin Then
            dgPortfolio.Width = Me.ClientSize.Width - dgPortfolio.Left - gridRightMargin
        End If

        If Me.ClientSize.Width > dgFuturos.Left + gridRightMargin Then
            dgFuturos.Width = Me.ClientSize.Width - dgFuturos.Left - gridRightMargin
        End If

        ArrangeOrdersPanel(PanelOrdens.Visible)
        ArrangeProfitPairs()
        'dgPortfolio.Height = (dgPortfolio.RowCount * 35)
        ' Me.Height = MenuStrip1.Height + dgPortfolio.Height + PanelGraphs.Height + PanelProfits.Height + panelDebug.Height + 65
    End Sub

    Private Sub ArrangeProfitPairs()
        If PanelProfits Is Nothing OrElse
           lbValoresHojeUSD Is Nothing OrElse
           lbValoresHojeBRL Is Nothing Then
            Return
        End If

        Const gap As Integer = 4

        lbValoresSeparator.Visible = lbValoresHojeUSD.Visible AndAlso lbValoresHojeBRL.Visible
        If lbValoresSeparator.Visible Then
            Dim totalWidth = lbValoresHojeUSD.Width +
                             gap +
                             lbValoresSeparator.Width +
                             gap +
                             lbValoresHojeBRL.Width
            Dim left = Math.Max(0, (PanelProfits.ClientSize.Width - totalWidth) \ 2)

            lbValoresHojeUSD.Location = New Point(left, lbValoresHojeUSD.Top)
            lbValoresSeparator.Location = New Point(
                lbValoresHojeUSD.Right + gap,
                lbValoresHojeUSD.Top)

            lbValoresHojeBRL.Location = New Point(
                lbValoresSeparator.Right + gap,
                lbValoresHojeUSD.Top)
        End If
    End Sub

    Private Sub HideStablecoinPortfolioRows()
        If Not dgPortfolio.Columns.Contains("Cripto") Then Return

        Dim currencyManager = TryCast(Me.BindingContext(dgPortfolio.DataSource), CurrencyManager)
        Dim safePosition As Integer = -1

        For Each row As DataGridViewRow In dgPortfolio.Rows
            If row.IsNewRow Then Continue For

            Dim symbol = row.Cells("Cripto").Value?.ToString().Trim().ToUpperInvariant()
            If Not Cjson.stablecoins.Contains(symbol) Then
                safePosition = row.Index
                Exit For
            End If
        Next

        ' Uma linha vinculada ao CurrencyManager não pode ser ocultada enquanto
        ' estiver selecionada. Move a posição atual para uma moeda normal antes
        ' de esconder USDT e os demais stablecoins.
        If safePosition >= 0 AndAlso currencyManager IsNot Nothing Then
            If currencyManager.Position < 0 OrElse
               currencyManager.Position >= dgPortfolio.Rows.Count OrElse
               Cjson.stablecoins.Contains(
                   dgPortfolio.Rows(currencyManager.Position).Cells("Cripto").Value?.ToString().Trim().ToUpperInvariant()) Then
                dgPortfolio.Rows(safePosition).Visible = True
                currencyManager.Position = safePosition
                dgPortfolio.CurrentCell = dgPortfolio.Rows(safePosition).Cells("Cripto")
            End If
        End If

        For Each row As DataGridViewRow In dgPortfolio.Rows
            If row.IsNewRow Then Continue For

            Dim symbol = row.Cells("Cripto").Value?.ToString().Trim().ToUpperInvariant()
            If Not Cjson.stablecoins.Contains(symbol) Then
                row.Visible = True
            ElseIf row.Index <> safePosition Then
                row.Visible = False
            End If
        Next
    End Sub

    Private Sub CadastroToolStripMenuItem_MouseEnter(sender As Object, e As EventArgs) Handles CadastroToolStripMenuItem.MouseEnter
        CadastroToolStripMenuItem.ForeColor = Color.Black
    End Sub

    Private Sub OpçõesToolStripMenuItem_MouseEnter(sender As Object, e As EventArgs) Handles OpçõesToolStripMenuItem.MouseEnter
        OpçõesToolStripMenuItem.ForeColor = Color.Black
    End Sub

    Private Sub CadastroToolStripMenuItem_MouseLeave(sender As Object, e As EventArgs) Handles CadastroToolStripMenuItem.MouseLeave
        CadastroToolStripMenuItem.ForeColor = Color.White
    End Sub

    Private Sub OpçõesToolStripMenuItem_MouseLeave(sender As Object, e As EventArgs) Handles OpçõesToolStripMenuItem.MouseLeave
        OpçõesToolStripMenuItem.ForeColor = Color.White
    End Sub

    Private Sub TimerBlink_Tick(sender As Object, e As EventArgs) Handles TimerBlink.Tick
        If lbLoadFromMarket.Visible = True Then
            If lbLoadFromMarket.ForeColor = Color.OrangeRed Then
                lbLoadFromMarket.ForeColor = Color.Gold
            ElseIf lbLoadFromMarket.ForeColor = Color.Gold Then
                lbLoadFromMarket.ForeColor = Color.White
            ElseIf lbLoadFromMarket.ForeColor = Color.White Then
                lbLoadFromMarket.ForeColor = Color.Yellow
            ElseIf lbLoadFromMarket.ForeColor = Color.Yellow Then
                lbLoadFromMarket.ForeColor = Color.OrangeRed
            End If
        End If
    End Sub

    Public Sub criptoGraph(criptoDic As Dictionary(Of String, Decimal))
        Dim gCriptos As New Charts

        gCriptos.collumGraph(500, 185, -2, 360, "Criptos", "% Criptos", 10, Color.Aqua, Color.FromArgb(30, 30, 30), SeriesChartType.Column, criptoDic, PanelGraphs)
    End Sub

    Public Sub addressGraph(criptoDic As Dictionary(Of String, Decimal))
        Dim gCriptos As New Charts

        gCriptos.pieGraph(330, 190, -2, 850, "Custódia", 10, Color.Aqua, Color.FromArgb(30, 30, 30), criptoDic, 7.5, Color.White, PanelGraphs)
    End Sub

    Private Sub dgPortfolio_CellPainting(sender As Object, e As DataGridViewCellPaintingEventArgs) Handles dgPortfolio.CellPainting
        If e.RowIndex >= 0 AndAlso e.ColumnIndex >= 0 AndAlso
           dgPortfolio.Columns.Contains("SpotClose") AndAlso
           String.Equals(dgPortfolio.Columns(e.ColumnIndex).Name, "SpotClose", StringComparison.OrdinalIgnoreCase) Then
            PaintCloseButtonCell(dgPortfolio, e, "SpotClose")
            Return
        End If

        If e.RowIndex >= 0 Then

            ' Pinta o fundo e o texto padrão
            e.PaintBackground(e.CellBounds, True)
            e.PaintContent(e.CellBounds)

            Using pen As New Pen(Color.FromArgb(70, 70, 70), 1)
                Dim rect = e.CellBounds
                Dim y = rect.Bottom - 1 ' Posição da linha inferior da célula
                e.Graphics.DrawLine(pen, rect.Left, y, rect.Right, y)
            End Using

            Dim colunasComLinhasVerticais As Integer() = {6} ' Índices das colunas que terão linhas verticais
            If colunasComLinhasVerticais.Contains(e.ColumnIndex) Then
                Using pen As New Pen(Color.FromArgb(3, 3, 3), 1)
                    Dim rect = e.CellBounds
                    Dim x = rect.Right - 1 ' Posição da borda direita da célula
                    e.Graphics.DrawLine(pen, x, rect.Top, x, rect.Bottom)
                End Using
            End If

            e.Handled = True ' Impede o desenho padrão

        End If

    End Sub

    Public Sub showUSDCollumns()
        Dim json As New JSON
        Try
            SetPortfolioColumnVisible("vlEntradaUSD", True)
            SetPortfolioColumnVisible("vlAtualUSD", True)
            SetPortfolioColumnVisible("ROIusd", True)

            SetPortfolioColumnVisible("vlEntradaBRL", False)
            SetPortfolioColumnVisible("vlAtualBRL", False)
            SetPortfolioColumnVisible("ROIbrl", False)
            EnsurePortfolioQuantityColumnVisible()

            json.FormatGrid(dgPortfolio)
            ConfigurePortfolioCloseColumns()

        Catch ex As Exception

        End Try
    End Sub

    Public Sub showBRLCollumns()
        Dim json As New JSON
        Try
            SetPortfolioColumnVisible("vlEntradaUSD", False)
            SetPortfolioColumnVisible("vlAtualUSD", False)
            SetPortfolioColumnVisible("ROIusd", False)

            SetPortfolioColumnVisible("vlEntradaBRL", True)
            SetPortfolioColumnVisible("vlAtualBRL", True)
            SetPortfolioColumnVisible("ROIbrl", True)
            EnsurePortfolioQuantityColumnVisible()

            json.FormatGrid(dgPortfolio)
            ConfigurePortfolioCloseColumns()

        Catch ex As Exception

        End Try

    End Sub

    Private Sub SetPortfolioColumnVisible(columnName As String, visible As Boolean)
        If dgPortfolio Is Nothing OrElse
           Not dgPortfolio.Columns.Contains(columnName) Then
            Return
        End If

        dgPortfolio.Columns(columnName).Visible = visible
    End Sub

    Private Sub pbUSD_Click(sender As Object, e As EventArgs) Handles pbUSD.Click
        showUSDCollumns()
    End Sub

    Private Sub pbBRL_Click(sender As Object, e As EventArgs) Handles pbBRL.Click
        showBRLCollumns()
    End Sub

    Private Sub PortfolioToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles PortfolioToolStripMenuItem.Click
        Dim filePath = Application.StartupPath & "\JSON\portfolio.json"
        OpenFileDialog1.Filter = "json Files (*.json)|*.json"
        OpenFileDialog1.FileName = "portfolio.json"

        If OpenFileDialog1.ShowDialog() = System.Windows.Forms.DialogResult.OK Then
            Dim jsonFile = OpenFileDialog1.FileName
            If File.Exists(filePath) Then
                If MessageBox.Show("Substituir arquivo existente?", "Atenção", MessageBoxButtons.YesNoCancel) = DialogResult.Yes Then
                    File.Copy(jsonFile, filePath, True)
                Else
                    Exit Sub
                End If
            Else
                File.Copy(jsonFile, filePath, False)
            End If
            MessageBox.Show("Importado com sucesso!", "Importar arquivo json", MessageBoxButtons.OK)
        End If
    End Sub

    Private Sub ImportarToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles ImportarToolStripMenuItem1.Click
        Dim filePath = Application.StartupPath & "\JSON\wallets.json"
        OpenFileDialog1.Filter = "json Files (*.json)|*.json"
        OpenFileDialog1.FileName = "wallets.json"

        If OpenFileDialog1.ShowDialog() = System.Windows.Forms.DialogResult.OK Then
            Dim jsonFile = OpenFileDialog1.FileName
            If File.Exists(filePath) Then
                If MessageBox.Show("Substituir arquivo existente?", "Atenção", MessageBoxButtons.YesNoCancel) = DialogResult.Yes Then
                    File.Copy(jsonFile, filePath, True)
                Else
                    Exit Sub
                End If
            Else
                File.Copy(jsonFile, filePath, False)
            End If
            MessageBox.Show("Importado com sucesso!", "Importar arquivo json", MessageBoxButtons.OK)
        End If
    End Sub
    Private Sub ImportarToolStripMenuItem2_Click(sender As Object, e As EventArgs) Handles ImportarToolStripMenuItem2.Click
        Dim filePath = Application.StartupPath & "\JSON\criptos.json"
        OpenFileDialog1.Filter = "json Files (*.json)|*.json"
        OpenFileDialog1.FileName = "criptos.json"

        If OpenFileDialog1.ShowDialog() = System.Windows.Forms.DialogResult.OK Then
            Dim jsonFile = OpenFileDialog1.FileName
            If File.Exists(filePath) Then
                If MessageBox.Show("Substituir arquivo existente?", "Atenção", MessageBoxButtons.YesNoCancel) = DialogResult.Yes Then
                    File.Copy(jsonFile, filePath, True)
                Else
                    Exit Sub
                End If
            Else
                File.Copy(jsonFile, filePath, False)
            End If
            MessageBox.Show("Importado com sucesso!", "Importar arquivo json", MessageBoxButtons.OK)
        End If
    End Sub

    Private Sub WalletsExchangeToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles WalletsExchangeToolStripMenuItem.Click
        Dim filePath = Application.StartupPath & "\JSON\portfolio.json"

        SaveFileDialog1.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
        SaveFileDialog1.FileName = "portfolio.json"

        If SaveFileDialog1.ShowDialog() = System.Windows.Forms.DialogResult.OK Then
            Dim jsonDestination = SaveFileDialog1.FileName
            If File.Exists(jsonDestination) Then
                If MessageBox.Show("Substituir arquivo existente?", "Atenção", MessageBoxButtons.YesNoCancel) = DialogResult.Yes Then
                    File.Copy(filePath, jsonDestination, True)
                Else
                    Exit Sub
                End If
            Else
                File.Copy(filePath, jsonDestination, False)
            End If
            MessageBox.Show("Exportado com sucesso!", "Exportar arquivo json", MessageBoxButtons.OK)
        End If
    End Sub

    Private Sub ExportarToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles ExportarToolStripMenuItem1.Click
        Dim filePath = Application.StartupPath & "\JSON\wallets.json"

        SaveFileDialog1.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
        SaveFileDialog1.FileName = "wallets.json"

        If SaveFileDialog1.ShowDialog() = System.Windows.Forms.DialogResult.OK Then
            Dim jsonDestination = SaveFileDialog1.FileName
            If File.Exists(jsonDestination) Then
                If MessageBox.Show("Substituir arquivo existente?", "Atenção", MessageBoxButtons.YesNoCancel) = DialogResult.Yes Then
                    File.Copy(filePath, jsonDestination, True)
                Else
                    Exit Sub
                End If
            Else
                File.Copy(filePath, jsonDestination, False)
            End If
            MessageBox.Show("Exportado com sucesso!", "Exportar arquivo json", MessageBoxButtons.OK)
        End If
    End Sub

    Private Sub ExportarToolStripMenuItem2_Click(sender As Object, e As EventArgs) Handles ExportarToolStripMenuItem2.Click
        Dim filePath = Application.StartupPath & "\JSON\criptos.json"

        SaveFileDialog1.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
        SaveFileDialog1.FileName = "criptos.json"

        If SaveFileDialog1.ShowDialog() = System.Windows.Forms.DialogResult.OK Then
            Dim jsonDestination = SaveFileDialog1.FileName
            If File.Exists(jsonDestination) Then
                If MessageBox.Show("Substituir arquivo existente?", "Atenção", MessageBoxButtons.YesNoCancel) = DialogResult.Yes Then
                    File.Copy(filePath, jsonDestination, True)
                Else
                    Exit Sub
                End If
            Else
                File.Copy(filePath, jsonDestination, False)
            End If
            MessageBox.Show("Exportado com sucesso!", "Exportar arquivo json", MessageBoxButtons.OK)
        End If
    End Sub

    Private Sub APIToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles APIToolStripMenuItem.Click
        FormAPI.ShowDialog()
    End Sub

    Private Sub dgPortfolio_SelectionChanged(sender As Object, e As EventArgs) Handles dgPortfolio.SelectionChanged
        dgPortfolio.ClearSelection()
    End Sub
    Private Sub lbCaixa_Click(sender As Object, e As EventArgs) Handles lbCaixa.Click
        Dim posLabelNaTela = lbCaixa.PointToScreen(Point.Empty)
        FormCaixa.Location = New Point(
            posLabelNaTela.X + (lbCaixa.Width - FormCaixa.Width) \ 2,
            posLabelNaTela.Y - FormCaixa.Height - 5 ' 5px de margem acima
        )
        FormCaixa.ShowDialog()
    End Sub

    Private Sub dgPortfolio_CellFormatting(sender As Object, e As DataGridViewCellFormattingEventArgs) Handles dgPortfolio.CellFormatting
        If e.RowIndex < 0 OrElse e.ColumnIndex < 0 Then
            Return
        End If

        Dim dgv = DirectCast(sender, DataGridView)
        Dim row = dgv.Rows(e.RowIndex)

        If e.Value Is Nothing OrElse e.Value Is DBNull.Value Then
            Return
        End If

        Dim valor As Decimal

        If TypeOf e.Value Is Decimal Then
            valor = DirectCast(e.Value, Decimal)
        ElseIf Not Decimal.TryParse(e.Value.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, valor) Then
            Dim texto As String = e.Value.ToString().Trim()
            If Not Decimal.TryParse(texto, NumberStyles.Number, CultureInfo.GetCultureInfo("pt-BR"), valor) Then
                Return
            End If
        End If

        Select Case dgv.Columns(e.ColumnIndex).Name

        ' ==========================================
        ' QTD
        ' ==========================================
            Case "Qtd"
                e.Value = valor.ToString("N2")
        ' ==========================================
        ' PREÇO MÉDIO
        ' ==========================================
            Case "precoMedio"
                e.Value = valor.ToString("C2", CultureInfo.GetCultureInfo("en-US"))
        ' ==========================================
        ' PREÇO ATUAL
        ' ==========================================
            Case "precoAtual"

                e.Value = valor.ToString("C4", CultureInfo.GetCultureInfo("en-US"))

                Dim precoMedio As Decimal = 0D
                If row.Cells("precoMedio").Value IsNot Nothing Then
                    precoMedio = Convert.ToDecimal(row.Cells("precoMedio").Value)
                End If

                If valor > precoMedio Then
                    e.CellStyle.ForeColor = Color.LightGreen
                ElseIf valor < precoMedio Then
                    e.CellStyle.ForeColor = Color.LightCoral
                Else
                    e.CellStyle.ForeColor = Color.WhiteSmoke
                End If

                e.FormattingApplied = True

        ' ==========================================
        ' 24 HORAS
        ' ==========================================
            Case "24horas"

                Dim texto As String =
                    e.Value.ToString().
                    Replace("%", "").
                    Replace(",", ".")

                If Decimal.TryParse(
                    texto,
                    NumberStyles.Any,
                    CultureInfo.InvariantCulture,
                    valor) Then

                    e.Value =
                        $"{valor:F2}%"

                    If valor > 0D Then

                        e.CellStyle.ForeColor =
                            Color.LimeGreen

                    ElseIf valor < 0D Then

                        e.CellStyle.ForeColor =
                            Color.Red
                    Else

                        e.CellStyle.ForeColor =
                            Color.FromArgb(20, 20, 20)

                    End If

                    e.FormattingApplied = True

                End If

        ' ==========================================
        ' MARKET CAP
        ' ==========================================
            Case "marketcap"
                valor.ToString("C2", CultureInfo.GetCultureInfo("en-US"))
        ' ==========================================
        ' QUANTIA ATUAL USD
        ' ==========================================
            Case "vlAtualUSD"
                e.Value = valor.ToString("C2", CultureInfo.GetCultureInfo("en-US"))
                e.FormattingApplied = True
        ' ==========================================
        ' QUANTIA ATUAL BRL
        ' ==========================================
            Case "vlAtualBRL"
                e.Value = valor.ToString("C2", CultureInfo.GetCultureInfo("pt-BR"))
        End Select
    End Sub
    Private Sub ImpermanetLossToolStripMenuItem_Click(sender As Object, e As EventArgs)
        FormPools.Show()
    End Sub

    Private Sub dgPortfolio_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgPortfolio.CellClick
        If e.RowIndex < 0 OrElse e.ColumnIndex < 0 Then
            Return
        End If

        If dgPortfolio.Columns(e.ColumnIndex).Name = "Cripto" Then

            Dim valor As String = dgPortfolio.Rows(e.RowIndex).Cells(e.ColumnIndex).Value?.ToString()
            If Not String.IsNullOrEmpty(valor) Then
                Dim f As New FormBrowser(valor)
                f.Show()
            End If
        End If
    End Sub

    Private Async Sub dgPortfolio_CellContentClick(
        sender As Object,
        e As DataGridViewCellEventArgs) Handles dgPortfolio.CellContentClick

        If e.RowIndex < 0 OrElse e.ColumnIndex < 0 OrElse
           Not String.Equals(dgPortfolio.Columns(e.ColumnIndex).Name, "SpotClose", StringComparison.OrdinalIgnoreCase) Then
            Return
        End If

        Dim row = dgPortfolio.Rows(e.RowIndex)
        Dim symbol = row.Cells("Cripto").Value?.ToString().Trim().ToUpperInvariant()
        Dim wallet = row.Cells("Wallet").Value?.ToString().Trim().ToUpperInvariant()
        Dim quantity As Decimal
        Dim limitPrice As Decimal

        If wallet <> "BINANCE" Then
            MessageBox.Show("O fechamento Spot está disponível somente para posições da Binance.", "Fechar posição", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        If String.IsNullOrWhiteSpace(symbol) OrElse
           Not TryReadFuturesDecimal(row.Cells("Qtd").Value, quantity) OrElse quantity <= 0D Then
            MessageBox.Show("A posição não possui uma quantidade válida.", "Fechar posição", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        If Not TryReadFuturesDecimal(row.Cells("SpotExitPrice").Value, limitPrice) OrElse limitPrice <= 0D Then
            MessageBox.Show("Informe o preço limite na linha da moeda antes de fechar a posição.", "Fechar posição", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim tradingSymbol = ToSpotUsdtSymbol(symbol)
        If MessageBox.Show(
               $"Enviar SELL LIMIT de {quantity:0.########} {tradingSymbol} a ${limitPrice:0.########}?",
               "Confirmar fechamento",
               MessageBoxButtons.YesNo,
               MessageBoxIcon.Warning) <> DialogResult.Yes Then
            Return
        End If

        Try
            dgPortfolio.Cursor = Cursors.WaitCursor
            Await B.BINANCE_CreateSpotOrderAsync(tradingSymbol, "SELL", "LIMIT", quantity, limitPrice)
            row.Cells("SpotExitPrice").Value = String.Empty
            Await LoadSpotOrdersAsync()
            MessageBox.Show("Ordem de fechamento enviada para a Binance.", "Fechar posição", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Catch ex As Exception
            MessageBox.Show(
                "Erro ao fechar a posição: " & ex.Message,
                "Fechar posição",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error)
        Finally
            dgPortfolio.Cursor = Cursors.Default
        End Try
    End Sub
    Private Sub ExportarPortfolioToolStripMenuItem_Click(sender As Object, e As EventArgs)

    End Sub

    Private Async Function LoadFuturesPositionsAsync() As Task
        If _futuresLoadRunning Then
            Return
        End If

        ' FuturesExitPrice is an unbound editing column. Preserve the text
        ' while the position table is rebuilt by a refresh or user-stream event.
        Dim savedExitPrices As System.Collections.Generic.Dictionary(Of String, String) = Nothing
        _futuresLoadRunning = True
        dgFuturos.Cursor = Cursors.WaitCursor

        Try
            Dim positions = Await B.BINANCE_GetFuturesPositionsAsync()
            Dim futuresAccount = Await B.BINANCE_GetFuturesAccountSummaryAsync()
            Dim protectionOrders = Await B.BINANCE_GetFuturesProtectionOrdersAsync()
            Await EnsureFuturesUsdBrlRateAsync()
            Dim table As New DataTable()

            table.Columns.Add("Symbol", GetType(String))
            table.Columns.Add("PositionSide", GetType(String))
            table.Columns.Add("InitialMargin", GetType(Decimal))
            table.Columns.Add("Leverage", GetType(Integer))
            table.Columns.Add("Notional", GetType(Decimal))
            table.Columns.Add("EntryPrice", GetType(Decimal))
            table.Columns.Add("MarkPrice", GetType(Decimal))
            table.Columns.Add("UnrealizedProfit", GetType(Decimal))
            table.Columns.Add("ROI", GetType(Decimal))
            table.Columns.Add("TakeProfit", GetType(String))
            table.Columns.Add("StopLoss", GetType(String))
            table.Columns.Add("LiquidationPrice", GetType(Decimal))
            table.Columns.Add("PositionAmount", GetType(Decimal))

            For Each position In positions
                Dim roiMargin = GetFuturesEntryMargin(position.PositionAmount, position.EntryPrice, position.Leverage, position.InitialMargin)
                Dim roiPercent = CalculateFuturesRoi(position.UnrealizedProfit, roiMargin)

                Dim takeProfitText = GetProtectionText(protectionOrders, position, takeProfit:=True)
                Dim stopLossText = GetProtectionText(protectionOrders, position, takeProfit:=False)

                table.Rows.Add(
                    position.Symbol,
                    If(String.IsNullOrWhiteSpace(position.PositionSide), "-", position.PositionSide),
                    roiMargin,
                    position.Leverage,
                    position.Notional,
                    position.EntryPrice,
                    position.MarkPrice,
                    position.UnrealizedProfit,
                    roiPercent,
                    takeProfitText,
                    stopLossText,
                    position.LiquidationPrice,
                    position.PositionAmount)
            Next
            ' Capture immediately before replacing the rows so text entered
            ' while the Binance requests were running is not lost.
            savedExitPrices = CaptureFuturesExitPrices()
            dgFuturos.DataSource = Nothing
            dgFuturos.Columns.Clear()
            dgFuturos.DataSource = table
            ConfigureFuturesGrid()
            RestoreFuturesExitPrices(savedExitPrices)
            ApplyFuturesGridColors()

            If futuresAccount.Equity > 0D Then
                _futuresEquityBaseUsd = futuresAccount.WalletBalance
                _futuresEquityLoaded = True
            End If
            _futuresAvailableUsd = futuresAccount.AvailableBalance
            UpdateFuturesOrderAvailableLabel()

            UpdateFuturesPnlSummary()

            If Not _futuresUserStreamStarted Then
                Await _binanceFuturesUserWs.StartAsync()
                _futuresUserStreamStarted = True
            End If
            Await EnsureFuturesMarketWebSocketAsync()

        Catch ex As Exception
            lbDebug.AppendText(
                Environment.NewLine &
                "Erro ao carregar posições de futuros: " & ex.Message)
        Finally
            dgFuturos.Cursor = Cursors.Default
            _futuresLoadRunning = False
        End Try
    End Function

    Private Function CaptureFuturesExitPrices() As System.Collections.Generic.Dictionary(Of String, String)
        Dim savedValues As New System.Collections.Generic.Dictionary(Of String, String)(StringComparer.OrdinalIgnoreCase)

        If dgFuturos Is Nothing OrElse
           Not dgFuturos.Columns.Contains("FuturesExitPrice") Then
            Return savedValues
        End If

        Dim editingRowIndex = If(dgFuturos.CurrentCell Is Nothing, -1, dgFuturos.CurrentCell.RowIndex)
        Dim editingColumnName = If(dgFuturos.CurrentCell Is Nothing, String.Empty, dgFuturos.Columns(dgFuturos.CurrentCell.ColumnIndex).Name)
        Dim editingText As String = Nothing
        If dgFuturos.IsCurrentCellInEditMode AndAlso
           String.Equals(editingColumnName, "FuturesExitPrice", StringComparison.OrdinalIgnoreCase) AndAlso
           dgFuturos.EditingControl IsNot Nothing Then
            editingText = dgFuturos.EditingControl.Text
        End If

        For Each row As DataGridViewRow In dgFuturos.Rows
            If row.IsNewRow Then Continue For

            Dim symbol = row.Cells("Symbol").Value?.ToString().Trim().ToUpperInvariant()
            Dim side = row.Cells("PositionSide").Value?.ToString().Trim().ToUpperInvariant()
            If String.IsNullOrWhiteSpace(symbol) Then Continue For

            Dim value = row.Cells("FuturesExitPrice").Value?.ToString()
            If row.Index = editingRowIndex AndAlso
               String.Equals(editingColumnName, "FuturesExitPrice", StringComparison.OrdinalIgnoreCase) AndAlso
               editingText IsNot Nothing Then
                value = editingText
            End If

            savedValues(BuildFuturesExitPriceKey(symbol, side)) = If(value, String.Empty)
        Next

        Return savedValues
    End Function

    Private Sub RestoreFuturesExitPrices(
        savedValues As System.Collections.Generic.Dictionary(Of String, String))

        If savedValues Is Nothing OrElse
           savedValues.Count = 0 OrElse
           Not dgFuturos.Columns.Contains("FuturesExitPrice") Then
            Return
        End If

        For Each row As DataGridViewRow In dgFuturos.Rows
            If row.IsNewRow Then Continue For

            Dim symbol = row.Cells("Symbol").Value?.ToString().Trim().ToUpperInvariant()
            Dim side = row.Cells("PositionSide").Value?.ToString().Trim().ToUpperInvariant()
            Dim key = BuildFuturesExitPriceKey(symbol, side)
            Dim savedValue As String = Nothing
            If savedValues.TryGetValue(key, savedValue) Then
                row.Cells("FuturesExitPrice").Value = savedValue
            End If
        Next
    End Sub

    Private Shared Function BuildFuturesExitPriceKey(symbol As String, side As String) As String
        Return If(symbol, String.Empty) & "|" & If(side, String.Empty)
    End Function

    Private Async Function LoadSpotOrdersAsync() As Task
        If _spotOrdersLoadRunning OrElse _dgOrdensSpot Is Nothing Then
            Return
        End If

        _spotOrdersLoadRunning = True
        _dgOrdensSpot.Cursor = Cursors.WaitCursor

        Try
            PopulateSpotOrderSymbols()
            Await RefreshSpotOrderCashAsync()
            Dim orders = Await B.BINANCE_GetSpotOpenOrdersAsync()
            Dim table As New DataTable()

            table.Columns.Add("Symbol", GetType(String))
            table.Columns.Add("Side", GetType(String))
            table.Columns.Add("OrderType", GetType(String))
            table.Columns.Add("Quantity", GetType(Decimal))
            table.Columns.Add("OrderValue", GetType(Decimal))
            table.Columns.Add("ExecutedQuantity", GetType(Decimal))
            table.Columns.Add("Price", GetType(Decimal))
            table.Columns.Add("CurrentPrice", GetType(Decimal))
            table.Columns.Add("StopPrice", GetType(Decimal))
            table.Columns.Add("Status", GetType(String))
            table.Columns.Add("OrderTime", GetType(DateTime))
            table.Columns.Add("OrderId", GetType(Long))

            For Each order In orders
                Dim currentPrice As Decimal = 0D
                TryGetSpotCurrentPrice(order.Symbol, currentPrice)
                Dim orderValue = order.Quantity * order.Price

                table.Rows.Add(
                    order.Symbol,
                    order.Side,
                    order.OrderType,
                    order.Quantity,
                    orderValue,
                    order.ExecutedQuantity,
                    order.Price,
                    currentPrice,
                    order.StopPrice,
                    order.Status,
                    order.OrderTime,
                    order.OrderId)
            Next

            _dgOrdensSpot.DataSource = Nothing
            _dgOrdensSpot.DataSource = table
            ConfigureSpotOrdersGrid()
            Await EnsureSpotOrderSymbolsWebSocketAsync()
        Catch ex As Exception
            Debug.WriteLine("Erro ao carregar ordens Spot: " & ex.Message)
            MessageBox.Show(
                "Erro ao carregar as ordens Spot: " & ex.Message,
                "Ordens Spot",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error)
        Finally
            _dgOrdensSpot.Cursor = Cursors.Default
            _spotOrdersLoadRunning = False
        End Try
    End Function

    Private Async Function EnsureFuturesMarketWebSocketAsync() As Task
        Dim symbols As New List(Of String)()

        For Each row As DataGridViewRow In dgFuturos.Rows
            If row.IsNewRow Then
                Continue For
            End If

            Dim symbol = row.Cells("Symbol").Value?.ToString().Trim().ToUpperInvariant()
            If Not String.IsNullOrWhiteSpace(symbol) Then
                symbols.Add(symbol)
            End If
        Next

        If _dgOrdensFuturos IsNot Nothing AndAlso
           _dgOrdensFuturos.Columns.Contains("Symbol") Then
            For Each orderRow As DataGridViewRow In _dgOrdensFuturos.Rows
                If orderRow.IsNewRow Then
                    Continue For
                End If

                Dim orderSymbol = ToFuturesUsdtSymbol(orderRow.Cells("Symbol").Value?.ToString())
                If Not String.IsNullOrWhiteSpace(orderSymbol) Then
                    symbols.Add(orderSymbol)
                End If
            Next
        End If

        If _cbFuturesOrderSymbol IsNot Nothing Then
            Dim selectedSymbol = ToFuturesUsdtSymbol(_cbFuturesOrderSymbol.Text)
            If Not String.IsNullOrWhiteSpace(selectedSymbol) Then
                symbols.Add(selectedSymbol)
            End If
        End If

        symbols = symbols.
            Where(Function(symbol) Not String.IsNullOrWhiteSpace(symbol)).
            Select(Function(symbol) ToFuturesUsdtSymbol(symbol)).
            Distinct(StringComparer.OrdinalIgnoreCase).
            ToList()

        If symbols.Count = 0 Then
            If Not String.IsNullOrWhiteSpace(_futuresMarketSymbolsKey) Then
                Await _binanceFuturesMarketWs.StopAsync()
                _futuresMarketSymbolsKey = String.Empty
            End If
            Return
        End If

        Dim symbolsKey = String.Join(
            "|",
            symbols.OrderBy(Function(symbol) symbol, StringComparer.OrdinalIgnoreCase))
        If String.Equals(symbolsKey, _futuresMarketSymbolsKey, StringComparison.OrdinalIgnoreCase) Then
            Return
        End If

        Await _binanceFuturesMarketWs.StartAsync(symbols)
        _futuresMarketSymbolsKey = symbolsKey
    End Function

    Private Shared Function ToFuturesUsdtSymbol(symbol As String) As String
        If String.IsNullOrWhiteSpace(symbol) Then
            Return String.Empty
        End If

        Dim normalized = symbol.Trim().ToUpperInvariant()
        If normalized.EndsWith("USDT", StringComparison.OrdinalIgnoreCase) Then
            Return normalized
        End If

        Return normalized & "USDT"
    End Function

    Private Shared Function GetFuturesBaseAsset(symbol As String) As String
        Dim normalized = ToFuturesUsdtSymbol(symbol)
        If normalized.EndsWith("USDT", StringComparison.OrdinalIgnoreCase) Then
            Return normalized.Substring(0, normalized.Length - 4)
        End If

        Return normalized
    End Function

    Private Sub PopulateFuturesOrderSymbols()
        If _cbFuturesOrderSymbol Is Nothing Then
            Return
        End If

        Dim previousSymbol = _cbFuturesOrderSymbol.Text
        Dim symbols As New List(Of String)()

        Try
            Dim cryptoTable As DataTable = PortfolioRepository.GetCryptoSymbols()
            For Each databaseRow As DataRow In cryptoTable.Rows
                Dim baseSymbol = databaseRow("Symbol")?.ToString().Trim().ToUpperInvariant()
                If String.IsNullOrWhiteSpace(baseSymbol) OrElse Cjson.stablecoins.Contains(baseSymbol) Then
                    Continue For
                End If

                Dim tradingSymbol = ToFuturesUsdtSymbol(baseSymbol)
                If Not symbols.Contains(tradingSymbol, StringComparer.OrdinalIgnoreCase) Then
                    symbols.Add(tradingSymbol)
                End If
            Next
        Catch ex As Exception
            Debug.WriteLine("Erro ao carregar moedas do SQLite para ordens Futures: " & ex.Message)
        End Try

        For Each row As DataGridViewRow In dgFuturos.Rows
            If row.IsNewRow Then Continue For
            Dim positionSymbol = ToFuturesUsdtSymbol(row.Cells("Symbol").Value?.ToString())
            If Not String.IsNullOrWhiteSpace(positionSymbol) AndAlso
               Not symbols.Contains(positionSymbol, StringComparer.OrdinalIgnoreCase) Then
                symbols.Add(positionSymbol)
            End If
        Next

        _cbFuturesOrderSymbol.BeginUpdate()
        Try
            _cbFuturesOrderSymbol.Items.Clear()
            For Each symbol In symbols.OrderBy(Function(item) item, StringComparer.OrdinalIgnoreCase)
                _cbFuturesOrderSymbol.Items.Add(symbol)
            Next

            Dim previousIndex = _cbFuturesOrderSymbol.FindStringExact(previousSymbol)
            If previousIndex >= 0 Then
                _cbFuturesOrderSymbol.SelectedIndex = previousIndex
            Else
                _cbFuturesOrderSymbol.SelectedIndex = -1
                _cbFuturesOrderSymbol.Text = String.Empty
            End If
        Finally
            _cbFuturesOrderSymbol.EndUpdate()
        End Try
    End Sub

    Private Sub UpdateFuturesOrderAvailableLabel()
        If _lbFuturesOrderAvailable IsNot Nothing Then
            _lbFuturesOrderAvailable.Text = Cjson.USDformat(_futuresAvailableUsd)
            _lbFuturesOrderAvailable.ForeColor = Color.Gold
        End If
    End Sub

    Private Function TryGetFuturesCurrentPrice(symbol As String, ByRef price As Decimal) As Boolean
        Dim normalizedSymbol = ToFuturesUsdtSymbol(symbol)
        If String.IsNullOrWhiteSpace(normalizedSymbol) Then
            Return False
        End If

        Return _binanceFuturesMarketWs.TryGetPrice(normalizedSymbol, price) OrElse
               TryGetFuturesGridPrice(normalizedSymbol, price)
    End Function

    Private Function TryGetFuturesGridPrice(symbol As String, ByRef price As Decimal) As Boolean
        If Not dgFuturos.Columns.Contains("Symbol") OrElse
           Not dgFuturos.Columns.Contains("MarkPrice") Then
            Return False
        End If

        For Each row As DataGridViewRow In dgFuturos.Rows
            If row.IsNewRow Then Continue For
            If String.Equals(
                   ToFuturesUsdtSymbol(row.Cells("Symbol").Value?.ToString()),
                   ToFuturesUsdtSymbol(symbol),
                   StringComparison.OrdinalIgnoreCase) Then
                Return TryReadFuturesDecimal(row.Cells("MarkPrice").Value, price)
            End If
        Next

        Return False
    End Function

    Private Sub UpdateFuturesOrderCurrentPrice()
        If _txtFuturesOrderCurrentPrice Is Nothing OrElse _cbFuturesOrderSymbol Is Nothing Then
            Return
        End If

        Dim price As Decimal
        If TryGetFuturesCurrentPrice(_cbFuturesOrderSymbol.Text, price) Then
            _txtFuturesOrderCurrentPrice.Text = "$" & price.ToString("0.0000", CultureInfo.GetCultureInfo("en-US"))
        Else
            _txtFuturesOrderCurrentPrice.Text = "--"
        End If

        UpdateFuturesOrderLinkedValue()
    End Sub

    Private Function TryGetFuturesOrderPrice(ByRef price As Decimal) As Boolean
        If _txtFuturesOrderPrice IsNot Nothing AndAlso
           TryReadFuturesDecimal(_txtFuturesOrderPrice.Text, price) AndAlso
           price > 0D Then
            Return True
        End If

        If _txtFuturesOrderCurrentPrice IsNot Nothing AndAlso
           TryReadFuturesDecimal(_txtFuturesOrderCurrentPrice.Text, price) AndAlso
           price > 0D Then
            Return True
        End If

        Return False
    End Function

    Private Sub SetFuturesOrderInputText(textBox As TextBox, value As String)
        If textBox Is Nothing OrElse textBox.Text = value Then
            Return
        End If

        _futuresOrderInputUpdating = True
        Try
            textBox.Text = value
        Finally
            _futuresOrderInputUpdating = False
        End Try
    End Sub

    Private Sub UpdateFuturesOrderNotional()
        If _txtFuturesOrderNotional Is Nothing OrElse _txtFuturesOrderQuantity Is Nothing OrElse _futuresOrderInputUpdating Then
            Return
        End If

        Dim quantity As Decimal
        Dim price As Decimal
        If Not TryReadFuturesDecimal(_txtFuturesOrderQuantity.Text, quantity) OrElse quantity <= 0D Then
            SetFuturesOrderInputText(_txtFuturesOrderNotional, String.Empty)
            Return
        End If

        If Not TryGetFuturesOrderPrice(price) Then
            SetFuturesOrderInputText(_txtFuturesOrderNotional, String.Empty)
            Return
        End If

        SetFuturesOrderInputText(
            _txtFuturesOrderNotional,
            (quantity * price).ToString("0.00", CultureInfo.GetCultureInfo("en-US")))
    End Sub

    Private Sub UpdateFuturesOrderQuantity()
        If _txtFuturesOrderNotional Is Nothing OrElse _txtFuturesOrderQuantity Is Nothing OrElse _futuresOrderInputUpdating Then
            Return
        End If

        Dim notional As Decimal
        Dim price As Decimal
        If Not TryReadFuturesDecimal(_txtFuturesOrderNotional.Text, notional) OrElse notional <= 0D OrElse
           Not TryGetFuturesOrderPrice(price) Then
            SetFuturesOrderInputText(_txtFuturesOrderQuantity, String.Empty)
            Return
        End If

        SetFuturesOrderInputText(
            _txtFuturesOrderQuantity,
            (notional / price).ToString("0.########", CultureInfo.GetCultureInfo("en-US")))
    End Sub

    Private Sub UpdateFuturesOrderLinkedValue()
        If _futuresOrderInputMode = "Notional" Then
            UpdateFuturesOrderQuantity()
        Else
            UpdateFuturesOrderNotional()
        End If
    End Sub

    Private Sub FuturesOrderTypeChanged(sender As Object, e As EventArgs)
        If _cbFuturesOrderType Is Nothing OrElse _txtFuturesOrderPrice Is Nothing Then
            Return
        End If

        Dim isLimit = String.Equals(_cbFuturesOrderType.Text, "LIMIT", StringComparison.OrdinalIgnoreCase)
        _txtFuturesOrderPrice.Enabled = isLimit
        _txtFuturesOrderPrice.BackColor = If(isLimit, Color.FromArgb(45, 45, 45), Color.FromArgb(35, 35, 35))
        UpdateFuturesOrderLinkedValue()
    End Sub

    Private Sub FuturesOrderQuantityChanged(sender As Object, e As EventArgs)
        If _futuresOrderInputUpdating Then Return
        _futuresOrderInputMode = "Quantity"
        UpdateFuturesOrderNotional()
    End Sub

    Private Sub FuturesOrderNotionalChanged(sender As Object, e As EventArgs)
        If _futuresOrderInputUpdating Then Return
        _futuresOrderInputMode = "Notional"
        UpdateFuturesOrderQuantity()
    End Sub

    Private Sub FuturesOrderPriceChanged(sender As Object, e As EventArgs)
        If _futuresOrderInputUpdating Then Return
        UpdateFuturesOrderLinkedValue()
    End Sub

    Private Sub FuturesOrderSymbolTextChanged(sender As Object, e As EventArgs)
        UpdateFuturesOrderCurrentPrice()
        UpdateFuturesOrderLinkedValue()
    End Sub

    Private Async Sub FuturesOrderSymbolChanged(sender As Object, e As EventArgs)
        UpdateFuturesOrderCurrentPrice()
        If Not String.IsNullOrWhiteSpace(ToFuturesUsdtSymbol(_cbFuturesOrderSymbol?.Text)) Then
            Await EnsureFuturesMarketWebSocketAsync()
            UpdateFuturesOrderCurrentPrice()
        End If
    End Sub

    Private Async Sub btGraficoFuturesOrder_Click(sender As Object, e As EventArgs)
        Dim symbol = GetFuturesBaseAsset(_cbFuturesOrderSymbol?.Text)
        If String.IsNullOrWhiteSpace(symbol) Then
            MessageBox.Show("Selecione uma moeda válida para abrir o gráfico.", "Gráfico", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        Await EnsureFuturesMarketWebSocketAsync()
        Dim f As New FormBrowser(symbol)
        f.Show()
    End Sub

    Private Async Function LoadFuturesOrdersAsync() As Task
        If _futuresOrdersLoadRunning OrElse _dgOrdensFuturos Is Nothing Then
            Return
        End If

        _futuresOrdersLoadRunning = True
        _dgOrdensFuturos.Cursor = Cursors.WaitCursor

        Try
            PopulateFuturesOrderSymbols()
            Dim account = Await B.BINANCE_GetFuturesAccountSummaryAsync()
            _futuresAvailableUsd = account.AvailableBalance
            UpdateFuturesOrderAvailableLabel()

            Try
                Dim positions = Await B.BINANCE_GetFuturesPositionsAsync(includeEmptyPositions:=True)
                For Each position In positions
                    If position IsNot Nothing AndAlso position.Leverage > 0 Then
                        _futuresLeverageBySymbol(ToFuturesUsdtSymbol(position.Symbol)) = position.Leverage
                    End If
                Next
            Catch ex As Exception
                Debug.WriteLine("Erro ao carregar alavancagem das posições Futures: " & ex.Message)
            End Try

            Dim orders = Await B.BINANCE_GetFuturesOpenOrdersAsync()
            Dim table As New DataTable()
            table.Columns.Add("Symbol", GetType(String))
            table.Columns.Add("PositionSide", GetType(String))
            table.Columns.Add("Leverage", GetType(Integer))
            table.Columns.Add("Side", GetType(String))
            table.Columns.Add("OrderType", GetType(String))
            table.Columns.Add("Quantity", GetType(Decimal))
            table.Columns.Add("OrderValue", GetType(Decimal))
            table.Columns.Add("ExecutedQuantity", GetType(Decimal))
            table.Columns.Add("Price", GetType(Decimal))
            table.Columns.Add("CurrentPrice", GetType(Decimal))
            table.Columns.Add("StopPrice", GetType(Decimal))
            table.Columns.Add("Status", GetType(String))
            table.Columns.Add("OrderTime", GetType(DateTime))
            table.Columns.Add("OrderId", GetType(Long))
            table.Columns.Add("AlgoId", GetType(Long))
            table.Columns.Add("IsProtection", GetType(Boolean))

            For Each order In orders
                Dim currentPrice As Decimal = 0D
                TryGetFuturesCurrentPrice(order.Symbol, currentPrice)
                Dim orderValue = order.Quantity * order.Price
                Dim orderLeverage As Integer = 0
                Dim normalizedOrderSymbol = ToFuturesUsdtSymbol(order.Symbol)
                If Not _futuresLeverageBySymbol.TryGetValue(normalizedOrderSymbol, orderLeverage) AndAlso
                   _cbFuturesOrderSymbol IsNot Nothing AndAlso
                   String.Equals(normalizedOrderSymbol, ToFuturesUsdtSymbol(_cbFuturesOrderSymbol.Text), StringComparison.OrdinalIgnoreCase) Then
                    Integer.TryParse(_cbFuturesOrderLeverage?.Text.Replace("x", String.Empty).Trim(), orderLeverage)
                End If

                table.Rows.Add(
                    order.Symbol,
                    If(String.IsNullOrWhiteSpace(order.PositionSide), "-", order.PositionSide),
                    orderLeverage,
                    order.Side,
                    order.OrderType,
                    order.Quantity,
                    orderValue,
                    order.ExecutedQuantity,
                    order.Price,
                    currentPrice,
                    order.StopPrice,
                    order.Status,
                    order.OrderTime,
                    order.OrderId,
                    0L,
                    False)
            Next

            Dim protectionOrders = Await B.BINANCE_GetFuturesProtectionOrdersAsync()
            For Each protection In protectionOrders
                Dim currentPrice As Decimal = 0D
                TryGetFuturesCurrentPrice(protection.Symbol, currentPrice)

                Dim normalizedProtectionSymbol = ToFuturesUsdtSymbol(protection.Symbol)
                Dim protectionLeverage As Integer = 0
                _futuresLeverageBySymbol.TryGetValue(normalizedProtectionSymbol, protectionLeverage)

                Dim protectionPrice = If(
                    protection.Price > 0D,
                    protection.Price,
                    protection.StopPrice)
                Dim protectionValue = protection.Quantity * protectionPrice

                table.Rows.Add(
                    protection.Symbol,
                    If(String.IsNullOrWhiteSpace(protection.PositionSide), "-", protection.PositionSide),
                    protectionLeverage,
                    protection.Side,
                    protection.OrderType,
                    protection.Quantity,
                    protectionValue,
                    0D,
                    protection.Price,
                    currentPrice,
                    protection.StopPrice,
                    If(String.IsNullOrWhiteSpace(protection.Status), "NEW", protection.Status),
                    protection.OrderTime,
                    0L,
                    protection.AlgoId,
                    True)
            Next

            _dgOrdensFuturos.DataSource = Nothing
            _dgOrdensFuturos.DataSource = table
            ConfigureFuturesOrdersGrid()
            Await EnsureFuturesMarketWebSocketAsync()
        Catch ex As Exception
            Debug.WriteLine("Erro ao carregar ordens Futures: " & ex.Message)
            MessageBox.Show(
                "Erro ao carregar as ordens Futures: " & ex.Message,
                "Ordens Futures",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error)
        Finally
            _dgOrdensFuturos.Cursor = Cursors.Default
            _futuresOrdersLoadRunning = False
        End Try
    End Function

    Private Sub ConfigureFuturesOrdersGrid()
        If _dgOrdensFuturos Is Nothing Then
            Return
        End If

        Dim headers As New Dictionary(Of String, String) From {
            {"Symbol", "Cripto"},
            {"PositionSide", "Lado"},
            {"Leverage", "Alavancagem"},
            {"Side", "Ordem"},
            {"OrderType", "Tipo"},
            {"Quantity", "Quantidade"},
            {"OrderValue", "Total USDT"},
            {"ExecutedQuantity", "Executada"},
            {"Price", "Preço"},
            {"CurrentPrice", "Preço atual"},
            {"StopPrice", "Stop"},
            {"Status", "Status"},
            {"OrderTime", "Data"},
            {"OrderId", "ID"}
        }

        For Each item In headers
            If _dgOrdensFuturos.Columns.Contains(item.Key) Then
                _dgOrdensFuturos.Columns(item.Key).HeaderText = item.Value
            End If
        Next

        If _dgOrdensFuturos.Columns.Contains("OrderId") Then
            _dgOrdensFuturos.Columns("OrderId").Visible = False
        End If
        If _dgOrdensFuturos.Columns.Contains("AlgoId") Then
            _dgOrdensFuturos.Columns("AlgoId").Visible = False
        End If
        If _dgOrdensFuturos.Columns.Contains("IsProtection") Then
            _dgOrdensFuturos.Columns("IsProtection").Visible = False
        End If

        If Not _dgOrdensFuturos.Columns.Contains("CancelOrder") Then
            Dim cancelColumn As New DataGridViewButtonColumn With {
                .Name = "CancelOrder",
                .HeaderText = "Ação",
                .Text = "Cancelar",
                .UseColumnTextForButtonValue = True,
                .AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                .Width = 78,
                .FlatStyle = FlatStyle.Flat
            }
            _dgOrdensFuturos.Columns.Add(cancelColumn)
        End If

        Dim cancelColumnReady = TryCast(_dgOrdensFuturos.Columns("CancelOrder"), DataGridViewButtonColumn)
        If cancelColumnReady IsNot Nothing Then
            cancelColumnReady.DisplayIndex = _dgOrdensFuturos.Columns.Count - 1
            cancelColumnReady.DefaultCellStyle = New DataGridViewCellStyle With {
                .Alignment = DataGridViewContentAlignment.MiddleCenter,
                .BackColor = Color.IndianRed,
                .ForeColor = Color.White,
                .SelectionBackColor = Color.DarkRed,
                .SelectionForeColor = Color.White
            }
        End If

        If _dgOrdensFuturos.Columns.Contains("Symbol") Then
            _dgOrdensFuturos.Columns("Symbol").MinimumWidth = 100
        End If
        If _dgOrdensFuturos.Columns.Contains("Status") Then
            _dgOrdensFuturos.Columns("Status").MinimumWidth = 90
        End If

        _dgOrdensFuturos.Font = New Font("Calibri", 11.0F, FontStyle.Regular)
        _dgOrdensFuturos.DefaultCellStyle.Font = _dgOrdensFuturos.Font
        _dgOrdensFuturos.RowTemplate.Height = 30
        _dgOrdensFuturos.ColumnHeadersHeight = 32
    End Sub

    Private Sub dgOrdensFuturos_CellFormatting(sender As Object, e As DataGridViewCellFormattingEventArgs)
        If _dgOrdensFuturos Is Nothing OrElse
           e.RowIndex < 0 OrElse
           e.ColumnIndex < 0 OrElse
           e.Value Is Nothing Then
            Return
        End If

        Dim columnName = _dgOrdensFuturos.Columns(e.ColumnIndex).Name
        Dim value As Decimal
        Dim leverageValue As Integer

        Select Case columnName
            Case "Symbol"
                e.CellStyle.ForeColor = Color.White
                e.CellStyle.SelectionForeColor = Color.White
            Case "PositionSide"
                Dim side = e.Value.ToString().Trim().ToUpperInvariant()
                Dim sideColor = If(side = "LONG", Color.LimeGreen, If(side = "SHORT", Color.IndianRed, Color.White))
                e.CellStyle.ForeColor = sideColor
                e.CellStyle.SelectionForeColor = sideColor
            Case "Leverage"
                If Integer.TryParse(e.Value.ToString().Replace("x", String.Empty).Trim(), leverageValue) Then
                    e.Value = If(leverageValue > 0, $"{leverageValue}x", "-")
                    e.CellStyle.ForeColor = Color.White
                    e.CellStyle.SelectionForeColor = Color.White
                    e.FormattingApplied = True
                End If
            Case "Side"
                Dim side = e.Value.ToString().Trim().ToUpperInvariant()
                Dim sideColor = If(side = "BUY", Color.LimeGreen, If(side = "SELL", Color.IndianRed, Color.White))
                e.CellStyle.ForeColor = sideColor
                e.CellStyle.SelectionForeColor = sideColor
            Case "Quantity", "ExecutedQuantity"
                If TryReadFuturesDecimal(e.Value, value) Then
                    e.Value = value.ToString("0.########", CultureInfo.GetCultureInfo("en-US"))
                    e.FormattingApplied = True
                End If
            Case "OrderValue"
                If TryReadFuturesDecimal(e.Value, value) Then
                    e.Value = If(value = 0D, "-", "$" & value.ToString("N2", CultureInfo.GetCultureInfo("en-US")))
                    e.FormattingApplied = True
                End If
            Case "Price"
                If TryReadFuturesDecimal(e.Value, value) Then
                    e.Value = If(value = 0D, "-", "$" & value.ToString("0.########", CultureInfo.GetCultureInfo("en-US")))
                    e.CellStyle.ForeColor = Color.LimeGreen
                    e.CellStyle.SelectionForeColor = Color.LimeGreen
                    e.FormattingApplied = True
                End If
            Case "CurrentPrice"
                If TryReadFuturesDecimal(e.Value, value) Then
                    e.Value = If(value = 0D, "-", "$" & value.ToString("0.0000", CultureInfo.GetCultureInfo("en-US")))
                    e.CellStyle.ForeColor = Color.Yellow
                    e.CellStyle.SelectionForeColor = Color.Yellow
                    e.FormattingApplied = True
                End If
            Case "StopPrice"
                If TryReadFuturesDecimal(e.Value, value) Then
                    e.Value = If(value = 0D, "-", "$" & value.ToString("0.########", CultureInfo.GetCultureInfo("en-US")))
                    e.CellStyle.ForeColor = Color.DarkOrange
                    e.CellStyle.SelectionForeColor = Color.DarkOrange
                    e.FormattingApplied = True
                End If
            Case "Status"
                e.CellStyle.ForeColor = Color.Gold
                e.CellStyle.SelectionForeColor = Color.Gold
            Case "OrderTime"
                If TypeOf e.Value Is DateTime Then
                    e.Value = DirectCast(e.Value, DateTime).ToString("dd/MM HH:mm:ss")
                    e.FormattingApplied = True
                End If
        End Select
    End Sub

    Private Async Sub dgOrdensFuturos_CellContentClick(sender As Object, e As DataGridViewCellEventArgs)
        If _dgOrdensFuturos Is Nothing OrElse
           e.RowIndex < 0 OrElse
           e.ColumnIndex < 0 OrElse
           _dgOrdensFuturos.Columns(e.ColumnIndex).Name <> "CancelOrder" Then
            Return
        End If

        Dim row = _dgOrdensFuturos.Rows(e.RowIndex)
        Dim symbol = row.Cells("Symbol").Value?.ToString().Trim().ToUpperInvariant()

        Dim isProtection As Boolean = False
        If _dgOrdensFuturos.Columns.Contains("IsProtection") Then
            Boolean.TryParse(row.Cells("IsProtection").Value?.ToString(), isProtection)
        End If

        If isProtection Then
            Dim algoId As Long
            If String.IsNullOrWhiteSpace(symbol) OrElse
               Not Long.TryParse(row.Cells("AlgoId").Value?.ToString(), algoId) OrElse
               algoId <= 0 Then
                Return
            End If

            If MessageBox.Show(
                   $"Cancelar a proteção {row.Cells("OrderType").Value} de {symbol} (#{algoId})?",
                   "Cancelar proteção Futures",
                   MessageBoxButtons.YesNo,
                   MessageBoxIcon.Warning) <> DialogResult.Yes Then
                Return
            End If

            Try
                _dgOrdensFuturos.Cursor = Cursors.WaitCursor
                If Await B.BINANCE_CancelFuturesProtectionOrderAsync(symbol, algoId) Then
                    Await LoadFuturesOrdersAsync()
                End If
            Catch ex As Exception
                MessageBox.Show(
                    "Erro ao cancelar a proteção Futures: " & ex.Message,
                    "Ordens Futures",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error)
            Finally
                _dgOrdensFuturos.Cursor = Cursors.Default
            End Try
            Return
        End If

        Dim orderId As Long
        If String.IsNullOrWhiteSpace(symbol) OrElse
           Not Long.TryParse(row.Cells("OrderId").Value?.ToString(), orderId) OrElse
           orderId <= 0 Then
            Return
        End If

        If MessageBox.Show(
               $"Cancelar a ordem Futures {symbol} #{orderId}?",
               "Cancelar ordem Futures",
               MessageBoxButtons.YesNo,
               MessageBoxIcon.Warning) <> DialogResult.Yes Then
            Return
        End If

        Try
            _dgOrdensFuturos.Cursor = Cursors.WaitCursor
            If Await B.BINANCE_CancelFuturesOrderAsync(symbol, orderId) Then
                Await LoadFuturesOrdersAsync()
            End If
        Catch ex As Exception
            MessageBox.Show(
                "Erro ao cancelar a ordem Futures: " & ex.Message,
                "Ordens Futures",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error)
        Finally
            _dgOrdensFuturos.Cursor = Cursors.Default
        End Try
    End Sub

    Private Async Sub btAtualizarOrdensFuturos_Click(sender As Object, e As EventArgs)
        Await LoadFuturesOrdersAsync()
    End Sub

    Private Async Sub btAbrirFuturesOrder_Click(sender As Object, e As EventArgs)
        If _cbFuturesOrderSymbol Is Nothing OrElse
           _cbFuturesOrderSide Is Nothing OrElse
           _cbFuturesOrderType Is Nothing Then
            Return
        End If

        Dim symbol = ToFuturesUsdtSymbol(_cbFuturesOrderSymbol.Text)
        Dim positionSide = _cbFuturesOrderSide.Text.Trim().ToUpperInvariant()
        Dim orderType = _cbFuturesOrderType.Text.Trim().ToUpperInvariant()
        Dim leverage As Integer = 0
        Dim quantity As Decimal
        Dim price As Decimal

        If _cbFuturesOrderLeverage IsNot Nothing Then
            Integer.TryParse(
                _cbFuturesOrderLeverage.Text.Replace("x", String.Empty).Trim(),
                leverage)
        End If

        If String.IsNullOrWhiteSpace(symbol) OrElse
           Not TryReadFuturesDecimal(_txtFuturesOrderQuantity.Text, quantity) OrElse
           quantity <= 0D Then
            MessageBox.Show("Informe uma quantidade válida.", "Ordem Futures", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        If Not TryReadFuturesDecimal(_txtFuturesOrderPrice.Text, price) OrElse price <= 0D Then
            MessageBox.Show("Informe um preço limite válido.", "Ordem Futures", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim leverageDetails = If(leverage > 0, $" com {leverage}x", String.Empty)
        Dim details = $"{positionSide} {orderType} {quantity:0.########} {symbol} a ${price:0.########}{leverageDetails}"
        If MessageBox.Show(
               "Enviar esta ordem Futures?" & Environment.NewLine & details,
               "Confirmar ordem",
               MessageBoxButtons.YesNo,
               MessageBoxIcon.Warning) <> DialogResult.Yes Then
            Return
        End If

        Try
            _btAbrirFuturesOrder.Enabled = False
            If leverage > 0 Then
                Await B.BINANCE_SetFuturesLeverageAsync(symbol, leverage)
                _futuresLeverageBySymbol(symbol) = leverage
            End If
            Await B.BINANCE_CreateFuturesOrderAsync(symbol, positionSide, orderType, quantity, price)
            Await LoadFuturesOrdersAsync()
            MessageBox.Show("Ordem enviada para a Binance.", "Ordem Futures", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Catch ex As Exception
            MessageBox.Show(
                "Erro ao enviar a ordem Futures: " & ex.Message,
                "Ordem Futures",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error)
        Finally
            _btAbrirFuturesOrder.Enabled = True
        End Try
    End Sub

    Private Sub PopulateSpotOrderSymbols()
        If _cbSpotOrderSymbol Is Nothing Then
            Return
        End If

        Dim previousSymbol = _cbSpotOrderSymbol.Text
        Dim symbols As New List(Of String)()

        Try
            Dim cryptoTable As DataTable = PortfolioRepository.GetCryptoSymbols()
            For Each databaseRow As DataRow In cryptoTable.Rows
                Dim baseSymbol = databaseRow("Symbol")?.ToString().Trim().ToUpperInvariant()
                If String.IsNullOrWhiteSpace(baseSymbol) OrElse Cjson.stablecoins.Contains(baseSymbol) Then
                    Continue For
                End If

                Dim tradingSymbol = ToSpotUsdtSymbol(baseSymbol)
                If Not symbols.Contains(tradingSymbol, StringComparer.OrdinalIgnoreCase) Then
                    symbols.Add(tradingSymbol)
                End If
            Next
        Catch ex As Exception
            Debug.WriteLine("Erro ao carregar moedas do SQLite para ordens Spot: " & ex.Message)
        End Try

        _cbSpotOrderSymbol.BeginUpdate()
        Try
            _cbSpotOrderSymbol.Items.Clear()
            For Each symbol In symbols.OrderBy(Function(item) item, StringComparer.OrdinalIgnoreCase)
                _cbSpotOrderSymbol.Items.Add(symbol)
            Next

            Dim previousIndex = _cbSpotOrderSymbol.FindStringExact(previousSymbol)
            If previousIndex >= 0 Then
                _cbSpotOrderSymbol.SelectedIndex = previousIndex
            Else
                _cbSpotOrderSymbol.SelectedIndex = -1
                _cbSpotOrderSymbol.Text = String.Empty
            End If
        Finally
            _cbSpotOrderSymbol.EndUpdate()
        End Try
    End Sub

    Private Shared Function ToSpotUsdtSymbol(baseSymbol As String) As String
        If String.IsNullOrWhiteSpace(baseSymbol) Then
            Return String.Empty
        End If

        Dim symbol = baseSymbol.Trim().ToUpperInvariant()
        For Each quote In New String() {"USDT", "USDC", "BUSD", "FDUSD", "BTC", "ETH", "BNB", "BRL"}
            If symbol.Length > quote.Length AndAlso
               symbol.EndsWith(quote, StringComparison.OrdinalIgnoreCase) Then
                Return symbol
            End If
        Next

        Return symbol & "USDT"
    End Function

    Private Shared Function GetSpotBaseAsset(tradingSymbol As String) As String
        If String.IsNullOrWhiteSpace(tradingSymbol) Then
            Return String.Empty
        End If

        Dim symbol = tradingSymbol.Trim().ToUpperInvariant()
        For Each quote In New String() {"USDT", "USDC", "BUSD", "FDUSD", "BTC", "ETH", "BNB", "BRL"}
            If symbol.EndsWith(quote, StringComparison.OrdinalIgnoreCase) AndAlso symbol.Length > quote.Length Then
                Return symbol.Substring(0, symbol.Length - quote.Length)
            End If
        Next

        Return symbol
    End Function

    Private Function TryGetSpotCurrentPrice(tradingSymbol As String, ByRef price As Decimal) As Boolean
        Dim symbol = GetSpotBaseAsset(tradingSymbol)
        If String.IsNullOrWhiteSpace(symbol) OrElse Cjson.stablecoins.Contains(symbol) Then
            Return False
        End If

        Return _binanceWs.TryGetPrice(symbol, price) OrElse TryGetSpotGridPrice(symbol, price)
    End Function

    Private Async Function EnsureSpotOrderSymbolsWebSocketAsync() As Task
        If _dgOrdensSpot Is Nothing OrElse
           Not _dgOrdensSpot.Columns.Contains("Symbol") Then
            Return
        End If

        For Each row As DataGridViewRow In _dgOrdensSpot.Rows
            If row.IsNewRow Then
                Continue For
            End If

            Dim symbol = row.Cells("Symbol").Value?.ToString()
            Dim price As Decimal
            If Not TryGetSpotCurrentPrice(symbol, price) Then
                Await StartBinanceWebSocket()
                Exit Function
            End If
        Next
    End Function

    Private Sub SpotOrderTypeChanged(sender As Object, e As EventArgs)
        If _cbSpotOrderType Is Nothing OrElse _txtSpotOrderPrice Is Nothing Then
            Return
        End If

        Dim isLimit = String.Equals(_cbSpotOrderType.Text, "LIMIT", StringComparison.OrdinalIgnoreCase)
        _txtSpotOrderPrice.Enabled = isLimit
        _txtSpotOrderPrice.BackColor = If(isLimit, Color.FromArgb(45, 45, 45), Color.FromArgb(35, 35, 35))
        If Not isLimit Then
            _txtSpotOrderPrice.Clear()
        End If

        UpdateSpotOrderLinkedValue()
    End Sub

    Private Sub SetSpotOrderInputText(textBox As TextBox, value As String)
        If textBox Is Nothing OrElse textBox.Text = value Then
            Return
        End If

        _spotOrderInputUpdating = True
        Try
            textBox.Text = value
        Finally
            _spotOrderInputUpdating = False
        End Try
    End Sub

    Private Function TryGetSpotOrderPrice(ByRef price As Decimal) As Boolean
        If _txtSpotOrderPrice IsNot Nothing AndAlso
           TryReadFuturesDecimal(_txtSpotOrderPrice.Text, price) AndAlso
           price > 0D Then
            Return True
        End If

        If _txtSpotOrderCurrentPrice IsNot Nothing AndAlso
           TryReadFuturesDecimal(_txtSpotOrderCurrentPrice.Text, price) AndAlso
           price > 0D Then
            Return True
        End If

        Return False
    End Function

    Private Sub SpotOrderQuantityChanged(sender As Object, e As EventArgs)
        If _spotOrderInputUpdating Then Return
        _spotOrderInputMode = "Quantity"
        UpdateSpotOrderLinkedValue()
    End Sub

    Private Sub SpotOrderNotionalChanged(sender As Object, e As EventArgs)
        If _spotOrderInputUpdating Then Return
        _spotOrderInputMode = "Notional"
        UpdateSpotOrderLinkedValue()
    End Sub

    Private Sub SpotOrderPriceChanged(sender As Object, e As EventArgs)
        If _spotOrderInputUpdating Then Return
        UpdateSpotOrderLinkedValue()
    End Sub

    Private Sub UpdateSpotOrderCashLabel()
        If _lbSpotOrderCash Is Nothing Then
            Return
        End If

        _lbSpotOrderCash.Text = Cjson.USDformat(_spotCashUsd)
        _lbSpotOrderCash.ForeColor = Color.Gold
    End Sub

    Private Async Function RefreshSpotOrderCashAsync() As Task
        Try
            Dim assets = Await B.BINANCE_GetSpotAssetsFull()
            Dim cashUsd As Decimal = 0D

            For Each asset In assets
                If Cjson.stablecoins.Contains(asset.Key) Then
                    cashUsd += asset.Value
                End If
            Next

            _spotCashUsd = cashUsd
            _spotCashLoadedFromAccount = True
            UpdateSpotOrderCashLabel()
        Catch ex As Exception
            Debug.WriteLine("Erro ao carregar Caixa Spot: " & ex.Message)
        End Try
    End Function

    Private Sub SpotOrderSymbolTextChanged(sender As Object, e As EventArgs)
        UpdateSpotOrderCurrentPrice()
        UpdateSpotOrderLinkedValue()
    End Sub

    Private Async Sub SpotOrderSymbolChanged(sender As Object, e As EventArgs)
        UpdateSpotOrderCurrentPrice()

        Dim symbol = GetSpotBaseAsset(_cbSpotOrderSymbol?.Text)
        If String.IsNullOrWhiteSpace(symbol) OrElse Cjson.stablecoins.Contains(symbol) Then
            Return
        End If

        Await EnsureSpotOrderSymbolWebSocketAsync()
        UpdateSpotOrderCurrentPrice()
    End Sub

    Private Async Function EnsureSpotOrderSymbolWebSocketAsync() As Task
        If _cbSpotOrderSymbol Is Nothing OrElse
           String.IsNullOrWhiteSpace(_cbSpotOrderSymbol.Text) OrElse
           _binanceWsStartRunning Then
            Return
        End If

        Await StartBinanceWebSocket()
    End Function

    Private Sub UpdateSpotOrderCurrentPrice()
        If _txtSpotOrderCurrentPrice Is Nothing OrElse _cbSpotOrderSymbol Is Nothing Then
            Return
        End If

        Dim symbol = GetSpotBaseAsset(_cbSpotOrderSymbol.Text)
        Dim price As Decimal
        If String.IsNullOrWhiteSpace(symbol) OrElse Cjson.stablecoins.Contains(symbol) Then
            _txtSpotOrderCurrentPrice.Text = "--"
            UpdateSpotOrderLinkedValue()
            Return
        End If

        If _binanceWs.TryGetPrice(symbol, price) OrElse TryGetSpotGridPrice(symbol, price) Then
            _txtSpotOrderCurrentPrice.Text = "$" & price.ToString("0.########", CultureInfo.GetCultureInfo("en-US"))
        Else
            _txtSpotOrderCurrentPrice.Text = "--"
        End If

        UpdateSpotOrderLinkedValue()
    End Sub

    Private Sub UpdateSpotOrdersCurrentPrice(symbol As String, price As Decimal)
        If _dgOrdensSpot Is Nothing OrElse
           Not _dgOrdensSpot.Columns.Contains("Symbol") OrElse
           Not _dgOrdensSpot.Columns.Contains("CurrentPrice") Then
            Return
        End If

        Dim baseSymbol = GetSpotBaseAsset(symbol)
        If String.IsNullOrWhiteSpace(baseSymbol) Then
            Return
        End If

        For Each row As DataGridViewRow In _dgOrdensSpot.Rows
            If row.IsNewRow Then
                Continue For
            End If

            Dim rowSymbol = GetSpotBaseAsset(row.Cells("Symbol").Value?.ToString())
            If String.Equals(rowSymbol, baseSymbol, StringComparison.OrdinalIgnoreCase) Then
                row.Cells("CurrentPrice").Value = price
            End If
        Next
    End Sub

    Private Sub UpdateSpotOrderNotional()
        If _txtSpotOrderNotional Is Nothing OrElse
           _txtSpotOrderQuantity Is Nothing OrElse
           _spotOrderInputUpdating Then
            Return
        End If

        Dim quantity As Decimal
        Dim price As Decimal
        If Not TryReadFuturesDecimal(_txtSpotOrderQuantity.Text, quantity) OrElse quantity <= 0D OrElse
           Not TryGetSpotOrderPrice(price) Then
            SetSpotOrderInputText(_txtSpotOrderNotional, String.Empty)
            Return
        End If

        SetSpotOrderInputText(
            _txtSpotOrderNotional,
            (quantity * price).ToString("0.00", CultureInfo.GetCultureInfo("en-US")))
    End Sub

    Private Sub UpdateSpotOrderQuantity()
        If _txtSpotOrderNotional Is Nothing OrElse
           _txtSpotOrderQuantity Is Nothing OrElse
           _spotOrderInputUpdating Then
            Return
        End If

        Dim notional As Decimal
        Dim price As Decimal
        If Not TryReadFuturesDecimal(_txtSpotOrderNotional.Text, notional) OrElse notional <= 0D OrElse
           Not TryGetSpotOrderPrice(price) Then
            SetSpotOrderInputText(_txtSpotOrderQuantity, String.Empty)
            Return
        End If

        SetSpotOrderInputText(
            _txtSpotOrderQuantity,
            (notional / price).ToString("0.########", CultureInfo.GetCultureInfo("en-US")))
    End Sub

    Private Sub UpdateSpotOrderLinkedValue()
        If _spotOrderInputMode = "Notional" Then
            UpdateSpotOrderQuantity()
        Else
            UpdateSpotOrderNotional()
        End If
    End Sub

    Private Function TryGetSpotGridPrice(symbol As String, ByRef price As Decimal) As Boolean
        If Not dgPortfolio.Columns.Contains("Cripto") OrElse
           Not dgPortfolio.Columns.Contains("precoAtual") Then
            Return False
        End If

        For Each row As DataGridViewRow In dgPortfolio.Rows
            If row.IsNewRow Then Continue For

            Dim rowSymbol = row.Cells("Cripto").Value?.ToString().Trim().ToUpperInvariant()
            If String.Equals(rowSymbol, symbol, StringComparison.OrdinalIgnoreCase) Then
                Return TryReadFuturesDecimal(row.Cells("precoAtual").Value, price)
            End If
        Next

        Return False
    End Function

    Private Async Sub btGraficoSpotOrder_Click(sender As Object, e As EventArgs)
        If _cbSpotOrderSymbol Is Nothing Then Return

        Dim symbol = GetSpotBaseAsset(_cbSpotOrderSymbol.Text)
        If String.IsNullOrWhiteSpace(symbol) OrElse Cjson.stablecoins.Contains(symbol) Then
            MessageBox.Show("Selecione uma moeda válida para abrir o gráfico.", "Gráfico", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        Await EnsureSpotOrderSymbolWebSocketAsync()
        Dim f As New FormBrowser(symbol)
        f.Show()
    End Sub

    Private Async Sub btAbrirSpotOrder_Click(sender As Object, e As EventArgs)
        If _cbSpotOrderSymbol Is Nothing OrElse _cbSpotOrderSide Is Nothing OrElse _cbSpotOrderType Is Nothing Then
            Return
        End If

        Dim symbol = ToSpotUsdtSymbol(_cbSpotOrderSymbol.Text.Trim())
        Dim side = _cbSpotOrderSide.Text.Trim().ToUpperInvariant()
        Dim orderType = _cbSpotOrderType.Text.Trim().ToUpperInvariant()
        Dim quantity As Decimal
        Dim price As Decimal = 0D

        If String.IsNullOrWhiteSpace(symbol) OrElse
           Not TryReadFuturesDecimal(_txtSpotOrderQuantity.Text, quantity) OrElse
           quantity <= 0D Then
            MessageBox.Show("Informe uma quantidade válida.", "Ordem Spot", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        If orderType = "LIMIT" AndAlso
           (Not TryReadFuturesDecimal(_txtSpotOrderPrice.Text, price) OrElse price <= 0D) Then
            MessageBox.Show("Informe um preço limite válido.", "Ordem Spot", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim details = $"{side} {orderType} {quantity:0.########} {symbol}"
        If orderType = "LIMIT" Then
            details &= $" a ${price:0.########}"
        End If

        If MessageBox.Show(
               "Enviar esta ordem Spot?" & Environment.NewLine & details,
               "Confirmar ordem",
               MessageBoxButtons.YesNo,
               MessageBoxIcon.Warning) <> DialogResult.Yes Then
            Return
        End If

        Try
            _btAbrirSpotOrder.Enabled = False
            Await B.BINANCE_CreateSpotOrderAsync(symbol, side, orderType, quantity, price)
            Await LoadSpotOrdersAsync()
            MessageBox.Show("Ordem enviada para a Binance.", "Ordem Spot", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Catch ex As Exception
            MessageBox.Show(
                "Erro ao enviar a ordem: " & ex.Message,
                "Ordem Spot",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error)
        Finally
            _btAbrirSpotOrder.Enabled = True
        End Try
    End Sub

    Private Sub ConfigureSpotOrdersGrid()
        If _dgOrdensSpot Is Nothing Then
            Return
        End If

        Dim headers As New Dictionary(Of String, String) From {
            {"Symbol", "Cripto"},
            {"Side", "Lado"},
            {"OrderType", "Tipo"},
            {"Quantity", "Quantidade"},
            {"OrderValue", "Total USDT"},
            {"ExecutedQuantity", "Executada"},
            {"Price", "Preço"},
            {"CurrentPrice", "Preço atual"},
            {"StopPrice", "Stop"},
            {"Status", "Status"},
            {"OrderTime", "Data"},
            {"OrderId", "ID"}
        }

        For Each item In headers
            If _dgOrdensSpot.Columns.Contains(item.Key) Then
                _dgOrdensSpot.Columns(item.Key).HeaderText = item.Value
            End If
        Next

        If _dgOrdensSpot.Columns.Contains("OrderId") Then
            _dgOrdensSpot.Columns("OrderId").Visible = False
        End If

        If Not _dgOrdensSpot.Columns.Contains("CancelOrder") Then
            Dim cancelColumn As New DataGridViewButtonColumn With {
                .Name = "CancelOrder",
                .HeaderText = "Ação",
                .Text = "Cancelar",
                .UseColumnTextForButtonValue = True,
                .AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                .Width = 78,
                .FlatStyle = FlatStyle.Flat
            }
            _dgOrdensSpot.Columns.Add(cancelColumn)
        End If

        Dim cancelColumnReady = TryCast(_dgOrdensSpot.Columns("CancelOrder"), DataGridViewButtonColumn)
        If cancelColumnReady IsNot Nothing Then
            cancelColumnReady.DisplayIndex = _dgOrdensSpot.Columns.Count - 1
            cancelColumnReady.DefaultCellStyle = New DataGridViewCellStyle With {
                .Alignment = DataGridViewContentAlignment.MiddleCenter,
                .BackColor = Color.IndianRed,
                .ForeColor = Color.White,
                .SelectionBackColor = Color.DarkRed,
                .SelectionForeColor = Color.White
            }
        End If

        If _dgOrdensSpot.Columns.Contains("Symbol") Then
            _dgOrdensSpot.Columns("Symbol").MinimumWidth = 100
        End If
        If _dgOrdensSpot.Columns.Contains("Status") Then
            _dgOrdensSpot.Columns("Status").MinimumWidth = 90
        End If

        _dgOrdensSpot.Font = New Font("Calibri", 11.0F, FontStyle.Regular)
        _dgOrdensSpot.DefaultCellStyle.Font = _dgOrdensSpot.Font
        _dgOrdensSpot.RowTemplate.Height = 30
        _dgOrdensSpot.ColumnHeadersHeight = 32
    End Sub

    Private Sub dgOrdensSpot_CellFormatting(
        sender As Object,
        e As DataGridViewCellFormattingEventArgs)

        If _dgOrdensSpot Is Nothing OrElse
           e.RowIndex < 0 OrElse
           e.ColumnIndex < 0 OrElse
           e.Value Is Nothing Then
            Return
        End If

        Dim columnName = _dgOrdensSpot.Columns(e.ColumnIndex).Name
        Dim value As Decimal

        Select Case columnName
            Case "Symbol"
                e.CellStyle.ForeColor = Color.White
                e.CellStyle.SelectionForeColor = Color.White
            Case "Side"
                Dim side = e.Value.ToString().Trim().ToUpperInvariant()
                Dim sideColor = If(side = "BUY", Color.LimeGreen, If(side = "SELL", Color.IndianRed, Color.White))
                e.CellStyle.ForeColor = sideColor
                e.CellStyle.SelectionForeColor = sideColor
            Case "Quantity", "ExecutedQuantity"
                If TryReadFuturesDecimal(e.Value, value) Then
                    e.Value = value.ToString("0.########", CultureInfo.GetCultureInfo("en-US"))
                    e.FormattingApplied = True
                End If
            Case "OrderValue"
                If TryReadFuturesDecimal(e.Value, value) Then
                    e.Value = If(value = 0D, "-", "$" & value.ToString("N2", CultureInfo.GetCultureInfo("en-US")))
                    e.FormattingApplied = True
                End If
            Case "Price"
                If TryReadFuturesDecimal(e.Value, value) Then
                    e.Value = If(value = 0D, "-", "$" & value.ToString("0.########", CultureInfo.GetCultureInfo("en-US")))
                    e.CellStyle.ForeColor = Color.LimeGreen
                    e.CellStyle.SelectionForeColor = Color.LimeGreen
                    e.FormattingApplied = True
                End If
            Case "StopPrice"
                If TryReadFuturesDecimal(e.Value, value) Then
                    e.Value = If(value = 0D, "-", "$" & value.ToString("0.########", CultureInfo.GetCultureInfo("en-US")))
                    e.FormattingApplied = True
                End If
            Case "CurrentPrice"
                If TryReadFuturesDecimal(e.Value, value) Then
                    e.Value = If(value = 0D, "-", "$" & value.ToString("0.########", CultureInfo.GetCultureInfo("en-US")))
                    e.CellStyle.ForeColor = Color.DeepSkyBlue
                    e.CellStyle.SelectionForeColor = Color.DeepSkyBlue
                    e.FormattingApplied = True
                End If
            Case "Status"
                e.CellStyle.ForeColor = Color.Gold
                e.CellStyle.SelectionForeColor = Color.Gold
            Case "OrderTime"
                If TypeOf e.Value Is DateTime Then
                    e.Value = DirectCast(e.Value, DateTime).ToString("dd/MM HH:mm:ss")
                    e.FormattingApplied = True
                End If
        End Select
    End Sub

    Private Async Sub dgOrdensSpot_CellContentClick(
        sender As Object,
        e As DataGridViewCellEventArgs)

        If _dgOrdensSpot Is Nothing OrElse
           e.RowIndex < 0 OrElse
           e.ColumnIndex < 0 OrElse
           _dgOrdensSpot.Columns(e.ColumnIndex).Name <> "CancelOrder" Then
            Return
        End If

        Dim row = _dgOrdensSpot.Rows(e.RowIndex)
        Dim symbol = row.Cells("Symbol").Value?.ToString().Trim().ToUpperInvariant()
        Dim orderId As Long
        If String.IsNullOrWhiteSpace(symbol) OrElse
           Not Long.TryParse(row.Cells("OrderId").Value?.ToString(), orderId) OrElse
           orderId <= 0 Then
            Return
        End If

        If MessageBox.Show(
               $"Cancelar a ordem {symbol} #{orderId}?",
               "Cancelar ordem Spot",
               MessageBoxButtons.YesNo,
               MessageBoxIcon.Warning) <> DialogResult.Yes Then
            Return
        End If

        Try
            _dgOrdensSpot.Cursor = Cursors.WaitCursor
            If Await B.BINANCE_CancelSpotOrderAsync(symbol, orderId) Then
                Await LoadSpotOrdersAsync()
            End If
        Catch ex As Exception
            MessageBox.Show(
                "Erro ao cancelar a ordem: " & ex.Message,
                "Ordens Spot",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error)
        Finally
            _dgOrdensSpot.Cursor = Cursors.Default
        End Try
    End Sub

    Private Async Sub btAtualizarOrdensSpot_Click(sender As Object, e As EventArgs)
        Await LoadSpotOrdersAsync()
    End Sub

    Private Async Function EnsureFuturesUsdBrlRateAsync() As Task
        If JSON.USDBRLprice > 0D Then Return

        Dim usdBrl As Decimal = 0D
        Try
            usdBrl = Await B.BINANCE_GetUSDTBRL()
        Catch ex As Exception
            Debug.WriteLine("[FUTURES] Binance USDT/BRL indisponível: " & ex.Message)
        End Try

        If usdBrl <= 0D AndAlso
           Not String.IsNullOrWhiteSpace(My.Settings.apiCMCKey) AndAlso
           Not String.IsNullOrWhiteSpace(My.Settings.activeAPI) Then
            Try
                Dim cmc As New Cotacao()
                usdBrl = Await cmc.CM_GetUSDBRL()
            Catch ex As Exception
                Debug.WriteLine("[FUTURES] CMC USDT/BRL indisponível: " & ex.Message)
            End Try
        End If

        If usdBrl > 0D Then
            JSON.USDBRLprice = usdBrl
        End If
    End Function

    Private Shared Function GetFuturesEntryMargin(positionAmount As Decimal, entryPrice As Decimal, leverage As Integer, fallbackMargin As Decimal) As Decimal
        If leverage > 0 AndAlso positionAmount <> 0D AndAlso entryPrice <> 0D Then
            Return Math.Abs(positionAmount * entryPrice) / leverage
        End If

        Return Math.Abs(fallbackMargin)
    End Function

    Private Shared Function CalculateFuturesRoi(unrealizedProfit As Decimal, entryMargin As Decimal) As Decimal
        If entryMargin = 0D Then Return 0D
        Return (unrealizedProfit / entryMargin) * 100D
    End Function
    Private Shared Function GetFuturesPositionSideColor(rawSide As Object) As Color
        Dim side = rawSide?.ToString().Trim()
        If String.Equals(side, "LONG", StringComparison.OrdinalIgnoreCase) Then
            Return Color.LimeGreen
        End If
        If String.Equals(side, "SHORT", StringComparison.OrdinalIgnoreCase) Then
            Return Color.Red
        End If
        Return Color.White
    End Function
    Private Shared Function GetFuturesProfitColor(rawValue As Object) As Color
        Dim value As Decimal
        If Not TryReadFuturesDecimal(rawValue, value) Then
            Return Color.WhiteSmoke
        End If

        If value > 0D Then
            Return Color.Lime
        End If
        If value < 0D Then
            Return Color.LightCoral
        End If

        Return Color.WhiteSmoke
    End Function
    Private Sub ApplyFuturesGridColors()
        For Each row As DataGridViewRow In dgFuturos.Rows
            If Not row.IsNewRow Then
                ApplyFuturesRowColors(row)
            End If
        Next
    End Sub

    Private Sub UpdateFuturesPnlSummary()
        Dim totalPnl As Decimal = 0D

        For Each row As DataGridViewRow In dgFuturos.Rows
            If row.IsNewRow Then Continue For

            Dim pnl As Decimal
            If TryReadFuturesDecimal(row.Cells("UnrealizedProfit").Value, pnl) Then
                totalPnl += pnl
            End If
        Next

        _futuresPnlUsd = totalPnl

        If _futuresEquityLoaded Then
            _futuresEquityUsd = _futuresEquityBaseUsd + totalPnl
        Else
            _futuresEquityUsd = totalPnl
        End If

        lbFuturosUSD.Text = Cjson.USDformat(_futuresEquityUsd)
        lbFuturosBRL.Text = Cjson.BRLformat(_futuresEquityUsd * JSON.USDBRLprice)
        lbFuturosUSD.ForeColor = Color.LimeGreen
        lbFuturosBRL.ForeColor = Color.DeepSkyBlue

        UpdateGrossInvestmentSummary()
        UpdateConsolidatedPnl()
        UpdateCombinedOverviewValues()
    End Sub

    Private Sub UpdateConsolidatedPnl()
        Dim totalPnlUsd = _spotPnlUsd + _futuresPnlUsd
        Dim totalPnlBrl = totalPnlUsd * JSON.USDBRLprice

        lbTotalBRL.Visible = True
        lbTotalBRL.Text = Cjson.BRLformat(totalPnlBrl)
        lbTotalBRL.ForeColor =
            If(totalPnlBrl > 0D,
               Color.FromArgb(0, 255, 0),
               Color.FromArgb(255, 73, 73))
        ArrangeProfitPairs()
    End Sub

    Private Sub UpdateCombinedOverviewValues()
        If Not _spotOverviewInitialized Then Return

        Dim totalAtualUsd = _spotCurrentUsd + _futuresEquityUsd
        Dim totalAtualBrl = totalAtualUsd * JSON.USDBRLprice
        Dim lucroUsd = _spotPnlUsd + _futuresPnlUsd
        Dim performanceWallet =
            If(_grossInvestmentUsd > 0D,
               (lucroUsd / _grossInvestmentUsd) * 100D,
               0D)

        lbValoresHojeUSD.Text = Cjson.USDformat(totalAtualUsd)
        lbValoresHojeBRL.Text = Cjson.BRLformat(totalAtualBrl)
        lbRoiUSD.Text = Cjson.USDformat(lucroUsd)
        lbPerformWallet.Text = $"{performanceWallet:F2}%"

        lbValoresHojeUSD.ForeColor =
            If(totalAtualUsd < _grossInvestmentUsd,
               Color.IndianRed,
               Color.GreenYellow)

        lbValoresHojeBRL.ForeColor =
            If(totalAtualBrl < _grossInvestmentBrl,
               Color.IndianRed,
               Color.Cyan)

        lbRoiUSD.ForeColor =
            If(lucroUsd < 0D,
               Color.Red,
               Color.Gold)

        lbPerformWallet.ForeColor =
            If(performanceWallet < 0D,
               Color.Red,
               Color.Lime)

        ArrangeProfitPairs()
    End Sub

    Private Sub UpdateGrossInvestmentSummary()
        Dim futuresCashUsd = If(_futuresEquityLoaded,
                                _futuresEquityBaseUsd,
                                0D)
        Dim grossInvestmentUsd =
            _spotCashUsd + futuresCashUsd + _spotCoinEntryUsd
        _grossInvestmentUsd = grossInvestmentUsd
        _grossInvestmentBrl = grossInvestmentUsd * JSON.USDBRLprice

        lbInvestimentoUSD.Text = Cjson.USDformat(grossInvestmentUsd)
        lbInvestimentoBRL.Text = Cjson.BRLformat(_grossInvestmentBrl)

        Dim investmentDetails =
            $"Caixa Spot: {Cjson.USDformat(_spotCashUsd)}" & Environment.NewLine &
            $"Custo das moedas Spot: {Cjson.USDformat(_spotCoinEntryUsd)}" & Environment.NewLine &
            $"Carteira Futures sem PnL aberto: {Cjson.USDformat(futuresCashUsd)}"
        ToolTip1.SetToolTip(lbInvestimentoUSD, investmentDetails)
        ToolTip1.SetToolTip(lbInvestimentoBRL, investmentDetails)
        ToolTip1.SetToolTip(lbRoiUSD,
            $"PnL Spot: {Cjson.USDformat(_spotPnlUsd)}" & Environment.NewLine &
            $"PnL Futures: {Cjson.USDformat(_futuresPnlUsd)}")
    End Sub

    Private Sub ApplyFuturesRowColors(row As DataGridViewRow)
        If row Is Nothing OrElse row.IsNewRow Then Return

        row.Cells("EntryPrice").Style.ForeColor = Color.Cyan
        row.Cells("EntryPrice").Style.SelectionForeColor = Color.Cyan

        Dim symbolColor = GetFuturesProfitColor(row.Cells("UnrealizedProfit").Value)
        row.Cells("Symbol").Style.ForeColor = symbolColor
        row.Cells("Symbol").Style.SelectionForeColor = symbolColor

        Dim sideColor = GetFuturesPositionSideColor(row.Cells("PositionSide").Value)
        row.Cells("PositionSide").Style.ForeColor = sideColor
        row.Cells("PositionSide").Style.SelectionForeColor = sideColor

        row.Cells("TakeProfit").Style.ForeColor = Color.DeepSkyBlue
        row.Cells("TakeProfit").Style.SelectionForeColor = Color.DeepSkyBlue
        For Each protectionColumn In New String() {"StopLoss", "LiquidationPrice"}
            row.Cells(protectionColumn).Style.ForeColor = Color.DarkOrange
            row.Cells(protectionColumn).Style.SelectionForeColor = Color.DarkOrange
        Next

        Dim entryPrice As Decimal
        Dim markPrice As Decimal
        Dim rowBackColor As Color = Color.Empty
        If TryReadFuturesDecimal(row.Cells("MarkPrice").Value, markPrice) AndAlso
           TryReadFuturesDecimal(row.Cells("EntryPrice").Value, entryPrice) Then
            Dim side = row.Cells("PositionSide").Value?.ToString().Trim()
            Dim isShort = String.Equals(side, "SHORT", StringComparison.OrdinalIgnoreCase)
            Dim isLoss = If(isShort, markPrice > entryPrice, markPrice < entryPrice)
            rowBackColor = If(isLoss, Color.FromArgb(25, 0, 0), Color.FromArgb(0, 25, 0))

            For Each cell As DataGridViewCell In row.Cells
                cell.Style.BackColor = rowBackColor
                cell.Style.SelectionBackColor = rowBackColor
            Next

            row.Cells("MarkPrice").Style.ForeColor = Color.Yellow
            row.Cells("MarkPrice").Style.SelectionForeColor = Color.Yellow
        End If

        For Each columnName In New String() {"UnrealizedProfit", "ROI"}
            Dim value As Decimal
            If TryReadFuturesDecimal(row.Cells(columnName).Value, value) Then
                Dim pnlColor = If(value > 0D, Color.LimeGreen, Color.Red)
                row.Cells(columnName).Style.ForeColor = pnlColor
                row.Cells(columnName).Style.SelectionForeColor = pnlColor
            End If
        Next

        ApplyFuturesCloseCellStyle(row, rowBackColor)
    End Sub
    Private Sub BinanceFuturesWs_ConnectionStateChanged(connected As Boolean, message As String)
        If IsDisposed OrElse Disposing Then Return

        If InvokeRequired Then
            BeginInvoke(New Action(Of Boolean, String)(AddressOf BinanceFuturesWs_ConnectionStateChanged), connected, message)
            Return
        End If

        If Not connected AndAlso lbDebug IsNot Nothing Then
            lbDebug.AppendText(Environment.NewLine & message)
        End If
    End Sub
    Private Sub BinanceFuturesMarkPriceUpdated(symbol As String, markPrice As Decimal)
        If IsDisposed OrElse Disposing Then Return

        If InvokeRequired Then
            BeginInvoke(New Action(Of String, Decimal)(AddressOf BinanceFuturesMarkPriceUpdated), symbol, markPrice)
            Return
        End If

        UpdateFuturesOrdersCurrentPrice(symbol, markPrice)
        UpdateFuturesOrderCurrentPrice()

        For Each row As DataGridViewRow In dgFuturos.Rows
            If row.IsNewRow OrElse Not String.Equals(row.Cells("Symbol").Value?.ToString(), symbol, StringComparison.OrdinalIgnoreCase) Then
                Continue For
            End If

            Dim quantity As Decimal
            Dim entryPrice As Decimal
            Dim initialMargin As Decimal
            Dim leverage As Integer
            If Not TryReadFuturesDecimal(row.Cells("PositionAmount").Value, quantity) OrElse
               Not TryReadFuturesDecimal(row.Cells("EntryPrice").Value, entryPrice) Then
                Continue For
            End If

            TryReadFuturesDecimal(row.Cells("InitialMargin").Value, initialMargin)
            Integer.TryParse(row.Cells("Leverage").Value?.ToString().Replace("x", String.Empty), leverage)
            Dim quantityAbs = Math.Abs(quantity)
            Dim positionSide = row.Cells("PositionSide").Value?.ToString()
            Dim isShort = String.Equals(positionSide, "SHORT", StringComparison.OrdinalIgnoreCase) OrElse quantity < 0D
            Dim pnl = If(isShort, (entryPrice - markPrice) * quantityAbs, (markPrice - entryPrice) * quantityAbs)
            Dim notional = quantityAbs * markPrice
            Dim entryMargin = GetFuturesEntryMargin(quantityAbs, entryPrice, leverage, initialMargin)
            Dim roi = CalculateFuturesRoi(pnl, entryMargin)

            row.Cells("MarkPrice").Value = markPrice
            row.Cells("Notional").Value = notional
            row.Cells("InitialMargin").Value = entryMargin
            row.Cells("UnrealizedProfit").Value = pnl
            row.Cells("ROI").Value = roi
            ApplyFuturesRowColors(row)
            UpdateFuturesPnlSummary()
            dgFuturos.InvalidateRow(row.Index)
            Exit For
        Next
    End Sub

    Private Sub UpdateFuturesOrdersCurrentPrice(symbol As String, price As Decimal)
        If _dgOrdensFuturos Is Nothing OrElse
           Not _dgOrdensFuturos.Columns.Contains("Symbol") OrElse
           Not _dgOrdensFuturos.Columns.Contains("CurrentPrice") Then
            Return
        End If

        Dim normalizedSymbol = ToFuturesUsdtSymbol(symbol)
        For Each row As DataGridViewRow In _dgOrdensFuturos.Rows
            If row.IsNewRow Then Continue For

            If String.Equals(
                   ToFuturesUsdtSymbol(row.Cells("Symbol").Value?.ToString()),
                   normalizedSymbol,
                   StringComparison.OrdinalIgnoreCase) Then
                row.Cells("CurrentPrice").Value = price
            End If
        Next
    End Sub

    Private Async Sub BinanceFuturesUserDataUpdated()
        If Not dgFuturos.Visible AndAlso Not PanelFuturos.Visible Then Return

        If InvokeRequired Then
            BeginInvoke(New MethodInvoker(
                Async Sub()
                    If dgFuturos.Visible Then
                        Await LoadFuturesPositionsAsync()
                    End If
                    If PanelFuturos.Visible Then
                        Await LoadFuturesOrdersAsync()
                    End If
                End Sub))
            Return
        End If

        If dgFuturos.Visible Then
            Await LoadFuturesPositionsAsync()
        End If
        If PanelFuturos.Visible Then
            Await LoadFuturesOrdersAsync()
        End If
    End Sub

    Private Sub BinanceSpotWs_ConnectionStateChanged(connected As Boolean, message As String)
        If Not connected Then
            Debug.WriteLine(message)
        End If
    End Sub

    Private Sub BinanceSpotUserDataUpdated()
        If IsDisposed OrElse Disposing Then Return

        If InvokeRequired Then
            BeginInvoke(New MethodInvoker(AddressOf BinanceSpotUserDataUpdated))
            Return
        End If

        If _spotAccountRefreshQueued Then Return
        _spotAccountRefreshQueued = True
        RefreshSpotAfterAccountEventAsync()
    End Sub

    Private Async Sub RefreshSpotAfterAccountEventAsync()
        Try
            ' Agrupa fills consecutivos da mesma ordem antes de consultar a conta.
            Await Task.Delay(350)

            Dim attempts As Integer = 0
            While _marketRefreshRunning AndAlso attempts < 10
                Await Task.Delay(250)
                attempts += 1
            End While

            If Not IsDisposed AndAlso Not Disposing Then
                Await refreshMarket()
                If PanelOrdens.Visible Then
                    Await LoadSpotOrdersAsync()
                End If
            End If
        Catch ex As Exception
            Debug.WriteLine("Erro atualizando Spot após evento de conta: " & ex.Message)
        Finally
            _spotAccountRefreshQueued = False
        End Try
    End Sub

    Private Shared Function GetProtectionText(
        orders As List(Of BinanceFuturesProtectionOrder),
        position As BinanceFuturesPosition,
        takeProfit As Boolean) As String

        Dim prices = orders.
            Where(Function(order)
                      If Not order.Symbol.Equals(position.Symbol, StringComparison.OrdinalIgnoreCase) Then
                          Return False
                      End If

                      If Not String.IsNullOrWhiteSpace(order.PositionSide) AndAlso
                         Not order.PositionSide.Equals("BOTH", StringComparison.OrdinalIgnoreCase) AndAlso
                         Not position.PositionSide.Equals("BOTH", StringComparison.OrdinalIgnoreCase) AndAlso
                         Not order.PositionSide.Equals(position.PositionSide, StringComparison.OrdinalIgnoreCase) Then
                          Return False
                      End If

                      Dim isTakeProfitOrder = order.OrderType.Contains("TAKE_PROFIT", StringComparison.OrdinalIgnoreCase)
                      Return If(takeProfit, isTakeProfitOrder, Not isTakeProfitOrder)
                  End Function).
            Select(Function(order) If(order.StopPrice <> 0D, order.StopPrice, order.Price)).
            Where(Function(price) price > 0D).
            Distinct().
            OrderBy(Function(price) price).
            Select(Function(price) "$" & price.ToString("N4", CultureInfo.GetCultureInfo("en-US"))).
            ToList()

        If prices.Count = 0 Then
            Return "-"
        End If

        Return String.Join(" / ", prices)
    End Function
    Private Sub ConfigureFuturesGrid()
        Dim headers As New Dictionary(Of String, String) From {
            {"Symbol", "Cripto"},
            {"PositionSide", "Lado"},
            {"Notional", "Notional"},
            {"InitialMargin", "Margem (USDT)"},
            {"EntryPrice", "Entrada"},
            {"MarkPrice", "Preço atual"},
            {"TakeProfit", "TP"},
            {"StopLoss", "SL"},
            {"UnrealizedProfit", "Pnl"},
            {"ROI", "ROI %"},
            {"LiquidationPrice", "Liquidação"},
            {"Leverage", "Alavancagem"}
        }
        For Each item In headers
            If dgFuturos.Columns.Contains(item.Key) Then
                dgFuturos.Columns(item.Key).HeaderText = item.Value
            End If
        Next

        If dgFuturos.Columns.Contains("PositionAmount") Then
            dgFuturos.Columns("PositionAmount").Visible = False
        End If

        If dgFuturos.Columns.Contains("Symbol") Then
            dgFuturos.Columns("Symbol").MinimumWidth = 120
        End If

        If dgFuturos.Columns.Contains("UnrealizedProfit") Then
            dgFuturos.Columns("UnrealizedProfit").AutoSizeMode = DataGridViewAutoSizeColumnMode.None
            dgFuturos.Columns("UnrealizedProfit").MinimumWidth = 78
            dgFuturos.Columns("UnrealizedProfit").Width = 78
        End If
        If dgFuturos.Columns.Contains("InitialMargin") Then
            dgFuturos.Columns("InitialMargin").Visible = True
        End If

        ConfigureFuturesCloseColumns()

        dgFuturos.Font = New Font("Calibri", 12.0F, FontStyle.Regular)
        dgFuturos.DefaultCellStyle.Font = dgFuturos.Font
        dgFuturos.RowHeadersDefaultCellStyle.Font = dgFuturos.Font
        dgFuturos.RowTemplate.Height = 35
        dgFuturos.ColumnHeadersHeight = 40
        dgFuturos.ColumnHeadersDefaultCellStyle.Font = New Font("Calibri", 10.0F, FontStyle.Italic)
        For Each row As DataGridViewRow In dgFuturos.Rows
            If Not row.IsNewRow Then
                row.Height = 35
            End If
        Next
    End Sub

    Private Sub ConfigureFuturesCloseColumns()
        If Not dgFuturos.Columns.Contains("FuturesExitPrice") Then
            Dim priceColumn As New DataGridViewTextBoxColumn With {
                .Name = "FuturesExitPrice",
                .HeaderText = "Preço limite",
                .AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                .Width = 92,
                .SortMode = DataGridViewColumnSortMode.NotSortable,
                .ReadOnly = False
            }
            priceColumn.DefaultCellStyle = New DataGridViewCellStyle With {
                .Alignment = DataGridViewContentAlignment.MiddleRight,
                .BackColor = Color.FromArgb(64, 64, 64),
                .ForeColor = Color.Gold,
                .SelectionBackColor = Color.White,
                .SelectionForeColor = Color.Black
            }
            dgFuturos.Columns.Add(priceColumn)
        End If

        If Not dgFuturos.Columns.Contains("FuturesClose") Then
            Dim closeColumn As New DataGridViewButtonColumn With {
                .Name = "FuturesClose",
                .HeaderText = "Fechar",
                .Text = "X",
                .UseColumnTextForButtonValue = True,
                .AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                .Width = 38,
                .FlatStyle = FlatStyle.Flat,
                .ReadOnly = True
            }
            closeColumn.DefaultCellStyle = New DataGridViewCellStyle With {
                .Alignment = DataGridViewContentAlignment.MiddleCenter,
                .BackColor = Color.IndianRed,
                .ForeColor = Color.White,
                .SelectionBackColor = Color.IndianRed,
                .SelectionForeColor = Color.White
            }
            dgFuturos.Columns.Add(closeColumn)
        End If

        Dim priceColumnReady = TryCast(dgFuturos.Columns("FuturesExitPrice"), DataGridViewTextBoxColumn)
        If priceColumnReady IsNot Nothing Then
            priceColumnReady.HeaderText = "Preço limite"
            priceColumnReady.Width = 92
            priceColumnReady.Visible = True
        End If

        Dim closeColumnReady = TryCast(dgFuturos.Columns("FuturesClose"), DataGridViewButtonColumn)
        If closeColumnReady IsNot Nothing Then
            closeColumnReady.HeaderText = "Fechar"
            closeColumnReady.Text = "X"
            closeColumnReady.UseColumnTextForButtonValue = True
            closeColumnReady.Width = 58
            closeColumnReady.Visible = True
        End If

        dgFuturos.ReadOnly = False
        dgFuturos.EditMode = DataGridViewEditMode.EditOnEnter
        For Each column As DataGridViewColumn In dgFuturos.Columns
            column.ReadOnly = Not String.Equals(column.Name, "FuturesExitPrice", StringComparison.OrdinalIgnoreCase)
        Next

        For Each row As DataGridViewRow In dgFuturos.Rows
            ApplyFuturesCloseCellStyle(row)
        Next

        Dim futuresColumnOrder As String() = {
            "Symbol", "PositionSide", "InitialMargin", "Leverage", "Notional",
            "EntryPrice", "MarkPrice", "UnrealizedProfit", "ROI", "TakeProfit",
            "StopLoss", "LiquidationPrice", "FuturesExitPrice", "FuturesClose"
        }

        If dgFuturos.Columns.Contains("PositionAmount") Then
            dgFuturos.Columns("PositionAmount").DisplayIndex = dgFuturos.Columns.Count - 1
        End If

        For displayIndex As Integer = futuresColumnOrder.Length - 1 To 0 Step -1
            Dim columnName = futuresColumnOrder(displayIndex)
            If dgFuturos.Columns.Contains(columnName) Then
                dgFuturos.Columns(columnName).DisplayIndex = displayIndex
            End If
        Next
    End Sub

    Private Sub ApplyFuturesCloseCellStyle(row As DataGridViewRow, Optional rowBackColor As Color = Nothing)
        If row Is Nothing OrElse row.IsNewRow Then Return

        Dim exitPriceBackColor = rowBackColor
        If exitPriceBackColor.IsEmpty Then
            exitPriceBackColor = Color.FromArgb(64, 64, 64)
        End If

        If dgFuturos.Columns.Contains("FuturesExitPrice") Then
            row.Cells("FuturesExitPrice").Style.BackColor = exitPriceBackColor
            row.Cells("FuturesExitPrice").Style.ForeColor = Color.Gold
            row.Cells("FuturesExitPrice").Style.SelectionBackColor = exitPriceBackColor
            row.Cells("FuturesExitPrice").Style.SelectionForeColor = Color.Gold
        End If

        If dgFuturos.Columns.Contains("FuturesClose") Then
            row.Cells("FuturesClose").Style.BackColor = Color.IndianRed
            row.Cells("FuturesClose").Style.ForeColor = Color.White
            row.Cells("FuturesClose").Style.SelectionBackColor = Color.IndianRed
            row.Cells("FuturesClose").Style.SelectionForeColor = Color.White
        End If
    End Sub

    Private Async Sub dgFuturos_CellContentClick(
        sender As Object,
        e As DataGridViewCellEventArgs) Handles dgFuturos.CellContentClick

        If e.RowIndex < 0 OrElse e.ColumnIndex < 0 OrElse
           Not String.Equals(dgFuturos.Columns(e.ColumnIndex).Name, "FuturesClose", StringComparison.OrdinalIgnoreCase) Then
            Return
        End If

        Dim row = dgFuturos.Rows(e.RowIndex)
        Dim symbol = row.Cells("Symbol").Value?.ToString().Trim().ToUpperInvariant()
        Dim positionSide = row.Cells("PositionSide").Value?.ToString().Trim().ToUpperInvariant()
        Dim quantity As Decimal
        Dim limitPrice As Decimal

        If String.IsNullOrWhiteSpace(symbol) OrElse
           Not TryReadFuturesDecimal(row.Cells("PositionAmount").Value, quantity) OrElse
           quantity = 0D Then
            MessageBox.Show("A posição não possui uma quantidade válida.", "Fechar posição", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        If positionSide <> "LONG" AndAlso positionSide <> "SHORT" Then
            MessageBox.Show("A posição precisa ser LONG ou SHORT para ser fechada.", "Fechar posição", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        If Not TryReadFuturesDecimal(row.Cells("FuturesExitPrice").Value, limitPrice) OrElse limitPrice <= 0D Then
            MessageBox.Show("Informe o preço limite na linha da posição antes de fechar.", "Fechar posição", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim absoluteQuantity = Math.Abs(quantity)
        If MessageBox.Show(
               $"Enviar ordem LIMIT para fechar {absoluteQuantity:0.########} {symbol} ({positionSide}) a ${limitPrice:0.########}?",
               "Confirmar fechamento",
               MessageBoxButtons.YesNo,
               MessageBoxIcon.Warning) <> DialogResult.Yes Then
            Return
        End If

        Try
            dgFuturos.Cursor = Cursors.WaitCursor
            Await B.BINANCE_CloseFuturesPositionAsync(symbol, positionSide, absoluteQuantity, limitPrice)
            row.Cells("FuturesExitPrice").Value = String.Empty
            Await LoadFuturesPositionsAsync()
            If PanelFuturos.Visible Then
                Await LoadFuturesOrdersAsync()
            End If
            MessageBox.Show("Ordem de fechamento enviada para a Binance.", "Fechar posição", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Catch ex As Exception
            MessageBox.Show("Erro ao fechar a posição: " & ex.Message, "Fechar posição", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            dgFuturos.Cursor = Cursors.Default
        End Try
    End Sub

    Private Shared Function TryReadFuturesDecimal(rawValue As Object, ByRef result As Decimal) As Boolean
        If rawValue Is Nothing OrElse rawValue Is DBNull.Value Then
            Return False
        End If

        If TypeOf rawValue Is Decimal Then
            result = DirectCast(rawValue, Decimal)
            Return True
        End If

        Dim textValue = rawValue.ToString().Replace("%", String.Empty).Replace("$", String.Empty).Trim()

        If Decimal.TryParse(
            textValue,
            NumberStyles.Float,
            CultureInfo.InvariantCulture,
            result) Then
            Return True
        End If

        Return Decimal.TryParse(
            textValue,
            NumberStyles.Number,
            CultureInfo.GetCultureInfo("pt-BR"),
            result)
    End Function
    Private Sub dgFuturos_CellFormatting(
        sender As Object,
        e As DataGridViewCellFormattingEventArgs) Handles dgFuturos.CellFormatting

        If e.RowIndex < 0 OrElse e.ColumnIndex < 0 OrElse e.Value Is Nothing Then
            Return
        End If

        Dim columnName = dgFuturos.Columns(e.ColumnIndex).Name
        Dim value As Decimal
        Dim leverageValue As Integer

        Select Case columnName
            Case "Symbol"
                Dim symbolColor = GetFuturesProfitColor(dgFuturos.Rows(e.RowIndex).Cells("UnrealizedProfit").Value)
                e.CellStyle.ForeColor = symbolColor
                e.CellStyle.SelectionForeColor = symbolColor
                e.FormattingApplied = True
            Case "PositionSide"
                Dim sideColor = GetFuturesPositionSideColor(e.Value)
                e.CellStyle.ForeColor = sideColor
                e.CellStyle.SelectionForeColor = sideColor
                e.FormattingApplied = True
            Case "PositionAmount"
                If TryReadFuturesDecimal(e.Value, value) Then
                    e.Value = value.ToString("N2", CultureInfo.GetCultureInfo("en-US"))
                    e.FormattingApplied = True
                End If
            Case "EntryPrice"
                If TryReadFuturesDecimal(e.Value, value) Then
                    e.Value = "$" & value.ToString("N4", CultureInfo.GetCultureInfo("en-US"))
                    e.CellStyle.ForeColor = Color.Cyan
                    e.CellStyle.SelectionForeColor = Color.Cyan
                    e.FormattingApplied = True
                End If
            Case "MarkPrice"
                If TryReadFuturesDecimal(e.Value, value) Then
                    e.Value = "$" & value.ToString("N4", CultureInfo.GetCultureInfo("en-US"))
                    e.CellStyle.ForeColor = Color.Yellow
                    e.CellStyle.SelectionForeColor = Color.Yellow
                    e.FormattingApplied = True
                End If
            Case "LiquidationPrice"
                If TryReadFuturesDecimal(e.Value, value) Then
                    e.Value = "$" & value.ToString("N4", CultureInfo.GetCultureInfo("en-US"))
                    e.CellStyle.ForeColor = Color.DarkOrange
                    e.CellStyle.SelectionForeColor = Color.DarkOrange
                    e.FormattingApplied = True
                End If
            Case "TakeProfit"
                e.CellStyle.ForeColor = Color.DeepSkyBlue
                e.CellStyle.SelectionForeColor = Color.DeepSkyBlue
            Case "StopLoss"
                e.CellStyle.ForeColor = Color.DarkOrange
                e.CellStyle.SelectionForeColor = Color.DarkOrange
            Case "UnrealizedProfit", "ROI", "Notional", "InitialMargin"
                If TryReadFuturesDecimal(e.Value, value) Then
                    If columnName = "ROI" Then
                        e.Value = value.ToString("N2", CultureInfo.GetCultureInfo("en-US")) & "%"
                    Else
                        e.Value = "$" & value.ToString("N2", CultureInfo.GetCultureInfo("en-US"))
                    End If
                    If columnName = "UnrealizedProfit" OrElse columnName = "ROI" Then
                        Dim pnlColor = If(value > 0D, Color.LimeGreen, Color.Red)
                        e.CellStyle.ForeColor = pnlColor
                        e.CellStyle.SelectionForeColor = pnlColor
                    End If
                    e.FormattingApplied = True
                End If
            Case "Leverage"
                If Integer.TryParse(e.Value.ToString, leverageValue) Then
                    e.Value = $"{leverageValue:0}x"
                    e.FormattingApplied = True
                End If
        End Select
    End Sub
    Private Async Sub dgFuturos_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgFuturos.CellClick
        If e.RowIndex < 0 OrElse e.ColumnIndex < 0 OrElse
           (Not String.Equals(dgFuturos.Columns(e.ColumnIndex).Name, "Symbol", StringComparison.OrdinalIgnoreCase) AndAlso
            Not String.Equals(dgFuturos.Columns(e.ColumnIndex).Name, "TakeProfit", StringComparison.OrdinalIgnoreCase) AndAlso
            Not String.Equals(dgFuturos.Columns(e.ColumnIndex).Name, "StopLoss", StringComparison.OrdinalIgnoreCase)) Then
            Return
        End If

        Dim row = dgFuturos.Rows(e.RowIndex)
        Dim clickedColumn = dgFuturos.Columns(e.ColumnIndex).Name
        If String.Equals(clickedColumn, "TakeProfit", StringComparison.OrdinalIgnoreCase) OrElse
           String.Equals(clickedColumn, "StopLoss", StringComparison.OrdinalIgnoreCase) Then
            Await OpenFuturesProtectionFormAsync(row, clickedColumn)
            Return
        End If

        Dim symbol = dgFuturos.Rows(e.RowIndex).Cells("Symbol").Value?.ToString().Trim()
        If String.IsNullOrWhiteSpace(symbol) Then Return

        If symbol.EndsWith("USDT", StringComparison.OrdinalIgnoreCase) Then
            symbol = symbol.Substring(0, symbol.Length - 4)
        End If

        If Not String.IsNullOrWhiteSpace(symbol) Then
            Dim f As New FormBrowser(symbol)
            f.Show()
        End If
    End Sub

    Private Async Function OpenFuturesProtectionFormAsync(row As DataGridViewRow, columnName As String) As Task
        If row Is Nothing OrElse row.IsNewRow Then Return

        Dim symbol = row.Cells("Symbol").Value?.ToString().Trim().ToUpperInvariant()
        Dim positionSide = row.Cells("PositionSide").Value?.ToString().Trim().ToUpperInvariant()
        Dim quantity As Decimal

        If String.IsNullOrWhiteSpace(symbol) OrElse
           Not TryReadFuturesDecimal(row.Cells("PositionAmount").Value, quantity) OrElse quantity = 0D Then
            MessageBox.Show("A linha selecionada não contém uma posição aberta válida.", "Proteção Futures", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        If positionSide <> "LONG" AndAlso positionSide <> "SHORT" AndAlso positionSide <> "BOTH" Then
            MessageBox.Show("A posição precisa ser LONG, SHORT ou BOTH para receber TP/SL.", "Proteção Futures", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim protectionName = If(String.Equals(columnName, "TakeProfit", StringComparison.OrdinalIgnoreCase), "Take Profit", "Stop Loss")
        Dim protectionType = If(String.Equals(columnName, "TakeProfit", StringComparison.OrdinalIgnoreCase), "TAKE_PROFIT", "STOP_LOSS")
        Dim currentValue = row.Cells(columnName).Value?.ToString()
        Dim entryPrice As Decimal
        If Not TryReadFuturesDecimal(row.Cells("EntryPrice").Value, entryPrice) OrElse entryPrice <= 0D Then
            MessageBox.Show("A posição não possui um preço de entrada válido.", "Proteção Futures", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Using protectionForm As New FormFuturesProtection(symbol, positionSide, protectionName, currentValue, entryPrice, quantity)
            If protectionForm.ShowDialog(Me) <> DialogResult.OK Then Return

            Dim triggerPrice = protectionForm.TriggerPrice
            Dim quantityPercent = protectionForm.QuantityPercent
            Dim protectionQuantity = Math.Abs(quantity) * quantityPercent / 100D
            Dim orderType = If(protectionType = "TAKE_PROFIT", "TAKE_PROFIT_MARKET", "STOP_MARKET")
            Dim side = If(positionSide = "LONG", "SELL", "BUY")
            Dim existingStopOrders As New List(Of BinanceFuturesProtectionOrder)()

            If protectionType = "STOP_LOSS" Then
                Dim openProtections = Await B.BINANCE_GetFuturesProtectionOrdersAsync()
                existingStopOrders = openProtections.
                    Where(Function(order)
                              If Not order.OrderType.Contains("STOP", StringComparison.OrdinalIgnoreCase) OrElse
                                 order.OrderType.Contains("TAKE_PROFIT", StringComparison.OrdinalIgnoreCase) OrElse
                                 order.OrderType.Contains("TRAILING", StringComparison.OrdinalIgnoreCase) OrElse
                                 Not order.Symbol.Equals(symbol, StringComparison.OrdinalIgnoreCase) Then
                                  Return False
                              End If

                              Return String.IsNullOrWhiteSpace(order.PositionSide) OrElse
                                     order.PositionSide.Equals("BOTH", StringComparison.OrdinalIgnoreCase) OrElse
                                     order.PositionSide.Equals(positionSide, StringComparison.OrdinalIgnoreCase)
                          End Function).
                    Where(Function(order) order.AlgoId > 0).
                    ToList()
            End If

            If protectionQuantity <= 0D Then
                MessageBox.Show("A quantidade calculada para a proteção deve ser maior que zero.", "Proteção Futures", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            Dim confirmation =
                $"Enviar {protectionName} para a Binance?" & Environment.NewLine & Environment.NewLine &
                $"Símbolo: {symbol}" & Environment.NewLine &
                $"Posição: {positionSide}" & Environment.NewLine &
                $"Ação: {side}" & Environment.NewLine &
                $"Tipo: {orderType}" & Environment.NewLine &
                $"Quantidade: {protectionQuantity:0.########} ({quantityPercent:0.##}%)" & Environment.NewLine &
                $"Preço de disparo: ${triggerPrice:0.########}" &
                If(existingStopOrders.Count > 0,
                   Environment.NewLine & Environment.NewLine & $"A SL existente será substituída ({existingStopOrders.Count} ordem(ns)).",
                   String.Empty)

            If MessageBox.Show(confirmation, "Confirmar proteção", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) <> DialogResult.Yes Then Return

            Try
                dgFuturos.Cursor = Cursors.WaitCursor
                Await B.BINANCE_CreateFuturesProtectionOrderAsync(symbol, positionSide, protectionType, protectionQuantity, triggerPrice)

                For Each existingStop In existingStopOrders
                    If Not Await B.BINANCE_CancelFuturesProtectionOrderAsync(symbol, existingStop.AlgoId) Then
                        Throw New Exception($"Não foi possível cancelar a SL anterior #{existingStop.AlgoId}.")
                    End If
                Next

                Await LoadFuturesPositionsAsync()
                If PanelFuturos.Visible Then Await LoadFuturesOrdersAsync()
                MessageBox.Show($"{protectionName} enviado para a Binance.", "Proteção Futures", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Catch ex As Exception
                MessageBox.Show("Erro ao enviar a proteção: " & ex.Message, "Proteção Futures", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Finally
                dgFuturos.Cursor = Cursors.Default
            End Try
        End Using
    End Function
    Private Async Sub FormMain_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing

        Try
            _spotTradeBot.Stop()
            Await _binanceWs.StopAsync()
            Await _gateWs.StopAsync()
            Await _binanceSpotUserWs.StopAsync()
            Await _binanceFuturesMarketWs.StopAsync()
            Await _binanceFuturesUserWs.StopAsync()
        Catch
        End Try
    End Sub
    Private Sub btSpot_Click(sender As Object, e As EventArgs) Handles btSpot.Click
        dgPortfolio.Visible = True
        dgFuturos.Visible = False
        btFuturos.BackColor = Color.FromArgb(20, 20, 20)
        btSpot.BackColor = Color.SteelBlue
    End Sub
    Private Sub btFuturos_Click(sender As Object, e As EventArgs) Handles btFuturos.Click
        dgPortfolio.Visible = False
        dgFuturos.Visible = True
        btFuturos.BackColor = Color.SteelBlue
        btSpot.BackColor = Color.FromArgb(20, 20, 20)

        'Await LoadFuturesPositionsAsync()
    End Sub

    Private Sub btVisaoGeral_Click(sender As Object, e As EventArgs) Handles btVisaoGeral.Click
        PanelGraphs.Visible = True
        PanelOrdens.Visible = False
        PanelFuturos.Visible = False
        ArrangeSpotBotPanel(False)
        ArrangeOrdersPanel(False)
        ArrangeFuturesOrdersPanel(False)
        btOrdensSpot.BackColor = Color.FromArgb(20, 20, 20)
        btOrdensFuturo.BackColor = Color.FromArgb(20, 20, 20)
        _btTradeBot.BackColor = Color.FromArgb(20, 20, 20)
        btVisaoGeral.BackColor = Color.SteelBlue
    End Sub
    Private Async Sub btOrdensSpot_Click(sender As Object, e As EventArgs) Handles btOrdensSpot.Click
        PanelGraphs.Visible = False
        PanelOrdens.Visible = True
        PanelFuturos.Visible = False
        ArrangeSpotBotPanel(False)
        ArrangeOrdersPanel(True)
        ArrangeFuturesOrdersPanel(False)
        btOrdensSpot.BackColor = Color.SteelBlue
        btVisaoGeral.BackColor = Color.FromArgb(20, 20, 20)
        btOrdensFuturo.BackColor = Color.FromArgb(20, 20, 20)
        _btTradeBot.BackColor = Color.FromArgb(20, 20, 20)
        Await LoadSpotOrdersAsync()
    End Sub
    Private Async Sub btOrdensFuturo_Click(sender As Object, e As EventArgs) Handles btOrdensFuturo.Click
        PanelGraphs.Visible = False
        PanelOrdens.Visible = False
        ArrangeSpotBotPanel(False)
        ArrangeFuturesOrdersPanel(True)
        ArrangeOrdersPanel(False)
        btOrdensSpot.BackColor = Color.FromArgb(20, 20, 20)
        btVisaoGeral.BackColor = Color.FromArgb(20, 20, 20)
        btOrdensFuturo.BackColor = Color.SteelBlue
        _btTradeBot.BackColor = Color.FromArgb(20, 20, 20)
        Await LoadFuturesOrdersAsync()
    End Sub

    Private Sub btTradeBot_Click(sender As Object, e As EventArgs)
        PanelGraphs.Visible = False
        PanelOrdens.Visible = False
        PanelFuturos.Visible = False
        ArrangeOrdersPanel(False)
        ArrangeFuturesOrdersPanel(False)
        ArrangeSpotBotPanel(True)
        btVisaoGeral.BackColor = Color.FromArgb(20, 20, 20)
        btOrdensSpot.BackColor = Color.FromArgb(20, 20, 20)
        btOrdensFuturo.BackColor = Color.FromArgb(20, 20, 20)
        _btTradeBot.BackColor = Color.SteelBlue
    End Sub

    Private Sub SimbolosToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SimbolosToolStripMenuItem.Click
        FormSymbols.Show()
    End Sub

    Private Sub CustodiaToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CustodiaToolStripMenuItem.Click
        FormWalletExchange.Show()
    End Sub


End Class
