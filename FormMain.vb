Imports System.Globalization
Imports System.IO
Imports System.Text
Imports System.Windows.Forms.DataVisualization.Charting
Imports Newtonsoft.Json.Linq
Imports System.Diagnostics
Imports System.Diagnostics.Tracing
Public Class FormMain
    Public remainingtimeInSeconds As Integer
    Dim Cjson As New JSON
    Dim chart As New Charts
    Dim B As New Binance
    Private ReadOnly _gateSpot As New Gateio
    Private ReadOnly _gateFutures As New GateFutures
    Dim gec As New Coingecko
    Private ReadOnly _binanceWs As New BinanceWebSocket
    Private ReadOnly _gateWs As New GateWebSocket
    Private ReadOnly _binanceFuturesMarketWs As New BinanceFuturesMarketWebSocket
    Private ReadOnly _gateFuturesMarketWs As New GateFuturesMarketWebSocket
    Private ReadOnly _binanceFuturesUserWs As New BinanceFuturesUserDataWebSocket
    Private ReadOnly _binanceSpotUserWs As New BinanceSpotUserDataWebSocket
    Private _marketRefreshRunning As Boolean = False
    Private _futuresLoadRunning As Boolean = False
    Private _futuresMarketSymbolsKey As String = String.Empty
    Private _futuresMarketProviderKey As String = String.Empty
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
    Private _spotOrderCashUsd As Decimal = 0D
    Private _spotOrderCashExchange As String = String.Empty
    Private _spotCoinEntryUsd As Decimal = 0D
    Private _grossInvestmentUsd As Decimal = 0D
    Private _grossInvestmentBrl As Decimal = 0D
    Private _spotOverviewInitialized As Boolean = False
    Private _dgOrdensSpot As DataGridView
    Private _btAtualizarOrdensSpot As Button
    Private _spotOrdersLoadRunning As Boolean = False
    Private _cbSpotOrderSymbol As ComboBox
    Private _cbSpotOrderExchange As ComboBox
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
    Private _cbFuturesOrderExchange As ComboBox
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
    Private _futuresOrderPriceKey As String = String.Empty
    Private _spotOrderInputUpdating As Boolean = False
    Private _spotOrderInputMode As String = "Quantity"
    Private ReadOnly _spotOrderAppliedQuantities As New Dictionary(Of String, Decimal)(StringComparer.OrdinalIgnoreCase)
    Private ReadOnly _futuresLeverageBySymbol As New Dictionary(Of String, Integer)(StringComparer.OrdinalIgnoreCase)
    Private _portfolioGridDefaultHeight As Integer
    Private _futuresGridDefaultHeight As Integer
    Private _binanceWsStartRunning As Boolean
    Private _catalogWarningShown As Boolean
    Private _binanceCatalogPromptRunning As Boolean
    Private _credentialsMenuItem As ToolStripMenuItem
    Private ReadOnly _spotTradeBot As New SpotTradeBotEngine()
    Private ReadOnly _spotDca As New SpotDcaEngine()
    Private _tradeBotButton As Button
    Private _panelSpotBot As Panel
    Private _panelSpotBotEditor As UserControl
    Private _panelDca As Panel
    Private _botLog As RichTextBox
    Private _dgSpotBotPositions As DataGridView
    Private _botCombination As ComboBox
    Private _botBollingerMode As ComboBox
    Private _botBollingerTimeframe As ComboBox
    Private _botConfirmations As NumericUpDown
    Private _botStartingQuote As NumericUpDown
    Private _botAllocation As NumericUpDown
    Private _botFee As NumericUpDown
    Private _botTakeProfit As NumericUpDown
    Private _botStopLoss As NumericUpDown
    Private _botBreakEvenEnabled As CheckBox
    Private _botBreakEvenTrigger As NumericUpDown
    Private _botTrailingEnabled As CheckBox
    Private _botTrailingDistance As NumericUpDown
    Private _botRiskBrakeEnabled As CheckBox
    Private _botRiskSessionLoss As NumericUpDown
    Private _botRiskProfitLockEnabled As CheckBox
    Private _botRiskProfitTrigger As NumericUpDown
    Private _botRiskGiveback As NumericUpDown
    Private _botRiskMaxLosses As NumericUpDown
    Private _botRiskPauseMinutes As NumericUpDown
    Private _botRiskClosePositions As CheckBox
    Private _botRiskUseUnrealized As CheckBox
    Private _botPostExitCooldown As NumericUpDown
    Private _botProfitGateEnabled As CheckBox
    Private _botMinProfitBeforeEntry As NumericUpDown
    Private _botUniverseMode As ComboBox
    Private _botUniverseMaxAssets As NumericUpDown
    Private _botUniverseMinVolume As NumericUpDown
    Private _botUniverseMinTrades As NumericUpDown
    Private _botGridRiskPercent As NumericUpDown
    Private _botGridReservePercent As NumericUpDown
    Private _botGridMaxExposurePercent As NumericUpDown
    Private _botGridMaxLevels As NumericUpDown
    Private _botGridRequireRejection As CheckBox
    Private _botBtcFlushEnabled As CheckBox
    Private _botBtcFlushDropPercent As NumericUpDown
    Private _botBtcFlushPauseMinutes As NumericUpDown
    Private _botBtcFlushVolumeMultiplier As NumericUpDown
    Private _botBtcDailyEnabled As CheckBox
    Private _botBtcDailyDropPercent As NumericUpDown
    Private _botBtcDailyDrawdownPercent As NumericUpDown
    Private _botUniverseButton As Button
    Private _botTrades As Button
    Private ReadOnly _spotBotUniverseSymbols As New List(Of String)()
    Private _botSimulationSummary As Label
    Private _botStart As Button
    Private _botStop As Button
    Private _botExport As Button
    Private _btDca As Button
    Private ReadOnly _botStrategyChecks As New Dictionary(Of SpotBotStrategyKind, CheckBox)()
    Private _lastBotSummaryUpdateUtc As DateTime
    Private _lastBotSimulationAction As String = String.Empty
    Private _cbDcaSymbol As ComboBox
    Private _cbDcaExchange As ComboBox
    Private _dcaCashRefreshRunning As Boolean
    Private _txtDcaLevels As TextBox
    Private _nudDcaBudget As NumericUpDown
    Private _nudDcaCooldown As NumericUpDown
    Private _chkDcaAutomaticSupports As CheckBox
    Private _cbDcaTimeframe As ComboBox
    Private _nudDcaLookback As NumericUpDown
    Private _nudDcaTolerance As NumericUpDown
    Private _nudDcaMinimumTouches As NumericUpDown
    Private _nudDcaMaximumLevels As NumericUpDown
    Private _chkDcaRequireRejection As CheckBox
    Private _chkDcaEnabled As CheckBox
    Private _btDcaAdd As Button
    Private _btDcaStart As Button
    Private _btDcaStop As Button
    Private _btDcaReset As Button
    Private _dgDca As DataGridView
    Private _lbDcaStatus As Label
    Private ReadOnly _dcaOrderInFlight As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)

    Private ReadOnly Property UseGateFutures As Boolean
        Get
            Return Not String.IsNullOrWhiteSpace(ExchangeCredentials.GateAPIKey) AndAlso
                   Not String.IsNullOrWhiteSpace(ExchangeCredentials.GateSecretKey)
        End Get
    End Property

    Private ReadOnly Property UseBinanceFutures As Boolean
        Get
            Return Not String.IsNullOrWhiteSpace(ExchangeCredentials.BinanceAPIKey) AndAlso
                   Not String.IsNullOrWhiteSpace(ExchangeCredentials.BinanceSecretKey)
        End Get
    End Property

    Private Shared Function NormalizeFuturesExchange(exchange As String) As String
        If String.Equals(exchange, "GATE", StringComparison.OrdinalIgnoreCase) Then
            Return "GATE"
        End If

        Return "BINANCE"
    End Function

    Private Shared Function FuturesProviderKey(exchange As String, symbol As String) As String
        Return NormalizeFuturesExchange(exchange) & "|" & ToFuturesUsdtSymbol(symbol)
    End Function

    Private Function GetDefaultFuturesExchange() As String
        If UseGateFutures Then Return "GATE"
        Return "BINANCE"
    End Function

    Private Function GetSelectedFuturesOrderExchange() As String
        If _cbFuturesOrderExchange Is Nothing OrElse
           String.IsNullOrWhiteSpace(_cbFuturesOrderExchange.Text) Then
            Return GetDefaultFuturesExchange()
        End If

        Return NormalizeFuturesExchange(_cbFuturesOrderExchange.Text)
    End Function

    Private Function IsFuturesExchangeConfigured(exchange As String) As Boolean
        If NormalizeFuturesExchange(exchange) = "GATE" Then
            Return UseGateFutures
        End If

        Return UseBinanceFutures
    End Function

    Private Sub RefreshFuturesOrderExchanges()
        If _cbFuturesOrderExchange Is Nothing Then
            Return
        End If

        Dim previousExchange = GetSelectedFuturesOrderExchange()
        _cbFuturesOrderExchange.BeginUpdate()
        Try
            _cbFuturesOrderExchange.Items.Clear()
            If UseBinanceFutures Then
                _cbFuturesOrderExchange.Items.Add("BINANCE")
            End If
            If UseGateFutures Then
                _cbFuturesOrderExchange.Items.Add("GATE")
            End If

            ' Mantém a seleção disponível mesmo antes de as credenciais serem
            ' configuradas, para que o painel continue orientando o usuário.
            If _cbFuturesOrderExchange.Items.Count = 0 Then
                _cbFuturesOrderExchange.Items.AddRange(New Object() {"BINANCE", "GATE"})
            End If

            Dim selectedIndex = _cbFuturesOrderExchange.FindStringExact(previousExchange)
            If selectedIndex < 0 Then
                selectedIndex = _cbFuturesOrderExchange.FindStringExact(GetDefaultFuturesExchange())
            End If
            _cbFuturesOrderExchange.SelectedIndex = If(selectedIndex >= 0, selectedIndex, 0)
        Finally
            _cbFuturesOrderExchange.EndUpdate()
        End Try
    End Sub

    Private Function GetRowFuturesExchange(row As DataGridViewRow) As String
        If row IsNot Nothing AndAlso
           row.DataGridView IsNot Nothing AndAlso
           row.DataGridView.Columns.Contains("Exchange") Then
            Dim value = row.Cells("Exchange").Value?.ToString()
            If Not String.IsNullOrWhiteSpace(value) Then
                Return NormalizeFuturesExchange(value)
            End If
        End If

        Return GetDefaultFuturesExchange()
    End Function

    Private Sub AppendFuturesProviderError(exchange As String, ex As Exception)
        Dim message = $"Erro ao carregar futuros {exchange}: {ex.Message}"
        Debug.WriteLine(message)
        If lbDebug IsNot Nothing Then
            lbDebug.AppendText(Environment.NewLine & message)
        End If
    End Sub

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
        If System.ComponentModel.LicenseManager.UsageMode = System.ComponentModel.LicenseUsageMode.Designtime Then
            Return
        End If

        ConfigureCredentialsMenu()
        ConfigureSpotOrdersPanel()
        ConfigureFuturesOrdersPanel()
        ConfigureSpotBotPanel()
        ConfigureDcaPanel()
        RemoveSelectionHighlight(dgPortfolio)
        RemoveSelectionHighlight(dgFuturos)
        RemoveSelectionHighlight(_dgOrdensSpot)
        RemoveSelectionHighlight(_dgOrdensFuturos)
        RemoveSelectionHighlight(_dgSpotBotPositions)
        RemoveSelectionHighlight(_dgDca)
        _portfolioGridDefaultHeight = dgPortfolio.Height
        _futuresGridDefaultHeight = dgFuturos.Height
        ArrangeOrdersPanel(False)
        Setup()
        lbDataTotalToday.Text = Date.Today & ":"
        Adjust()
        ArrangeProfitPairs()
    End Sub

    ''' <summary>
    ''' Mantém uma seleção funcional para teclado/edição, mas sem aplicar a
    ''' inversão de cores padrão do DataGridView nos grids operacionais.
    ''' Os grids de cadastro (símbolos e wallets) não passam por este método.
    ''' </summary>
    Private Shared Sub RemoveSelectionHighlight(grid As DataGridView)
        If grid Is Nothing Then Return

        Dim normalBackColor = If(grid.DefaultCellStyle.BackColor.IsEmpty,
                                 grid.BackgroundColor,
                                 grid.DefaultCellStyle.BackColor)
        Dim normalForeColor = If(grid.DefaultCellStyle.ForeColor.IsEmpty,
                                 Color.White,
                                 grid.DefaultCellStyle.ForeColor)

        grid.DefaultCellStyle.SelectionBackColor = normalBackColor
        grid.DefaultCellStyle.SelectionForeColor = normalForeColor
        grid.RowsDefaultCellStyle.SelectionBackColor = If(grid.RowsDefaultCellStyle.BackColor.IsEmpty,
                                                          normalBackColor,
                                                          grid.RowsDefaultCellStyle.BackColor)
        grid.RowsDefaultCellStyle.SelectionForeColor = If(grid.RowsDefaultCellStyle.ForeColor.IsEmpty,
                                                          normalForeColor,
                                                          grid.RowsDefaultCellStyle.ForeColor)
        grid.RowTemplate.DefaultCellStyle.SelectionBackColor = normalBackColor
        grid.RowTemplate.DefaultCellStyle.SelectionForeColor = normalForeColor
        grid.ColumnHeadersDefaultCellStyle.SelectionBackColor = grid.ColumnHeadersDefaultCellStyle.BackColor
        grid.ColumnHeadersDefaultCellStyle.SelectionForeColor = grid.ColumnHeadersDefaultCellStyle.ForeColor
        grid.RowHeadersDefaultCellStyle.SelectionBackColor = grid.RowHeadersDefaultCellStyle.BackColor
        grid.RowHeadersDefaultCellStyle.SelectionForeColor = grid.RowHeadersDefaultCellStyle.ForeColor

        For Each column As DataGridViewColumn In grid.Columns
            column.DefaultCellStyle.SelectionBackColor = If(column.DefaultCellStyle.BackColor.IsEmpty,
                                                             normalBackColor,
                                                             column.DefaultCellStyle.BackColor)
            column.DefaultCellStyle.SelectionForeColor = If(column.DefaultCellStyle.ForeColor.IsEmpty,
                                                             normalForeColor,
                                                             column.DefaultCellStyle.ForeColor)
        Next

        For Each row As DataGridViewRow In grid.Rows
            If row.IsNewRow Then Continue For
            If Not row.DefaultCellStyle.BackColor.IsEmpty Then
                row.DefaultCellStyle.SelectionBackColor = row.DefaultCellStyle.BackColor
            Else
                row.DefaultCellStyle.SelectionBackColor = normalBackColor
            End If
            If Not row.DefaultCellStyle.ForeColor.IsEmpty Then
                row.DefaultCellStyle.SelectionForeColor = row.DefaultCellStyle.ForeColor
            Else
                row.DefaultCellStyle.SelectionForeColor = normalForeColor
            End If

            For Each cell As DataGridViewCell In row.Cells
                Dim inherited = cell.InheritedStyle
                cell.Style.SelectionBackColor = inherited.BackColor
                cell.Style.SelectionForeColor = inherited.ForeColor
            Next
        Next
    End Sub

    Private Shared Sub RemoveCellSelectionHighlight(row As DataGridViewRow)
        If row Is Nothing OrElse row.IsNewRow Then Return
        For Each cell As DataGridViewCell In row.Cells
            Dim inherited = cell.InheritedStyle
            cell.Style.SelectionBackColor = inherited.BackColor
            cell.Style.SelectionForeColor = inherited.ForeColor
        Next
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

    Private Async Sub CredentialsMenuItem_Click(sender As Object, e As EventArgs)
        Using credentialsForm As New FormExchangeCredentials()
            credentialsForm.ShowDialog(Me)
        End Using

        RefreshFuturesOrderExchanges()
        If PanelFuturos IsNot Nothing AndAlso PanelFuturos.Visible Then
            Await LoadFuturesOrdersAsync()
        End If
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

        AddSpotOrderEditorLabel(editor, "Exchange", 314, 1, 68)
        _cbSpotOrderExchange = New ComboBox With {
            .BackColor = Color.FromArgb(45, 45, 45),
            .DropDownStyle = ComboBoxStyle.DropDownList,
            .FlatStyle = FlatStyle.Flat,
            .ForeColor = Color.White,
            .Location = New Point(314, 17),
            .Name = "cbSpotOrderExchange",
            .Size = New Size(68, 23)
        }
        _cbSpotOrderExchange.Items.AddRange(New Object() {"BINANCE", "GATE.IO"})
        _cbSpotOrderExchange.SelectedIndex = 0
        AddHandler _cbSpotOrderExchange.SelectedIndexChanged, AddressOf SpotOrderExchangeChanged
        editor.Controls.Add(_cbSpotOrderExchange)

        AddSpotOrderEditorLabel(editor, "Lado", 390, 1, 62)
        _cbSpotOrderSide = New ComboBox With {
            .BackColor = Color.FromArgb(45, 45, 45),
            .DropDownStyle = ComboBoxStyle.DropDownList,
            .FlatStyle = FlatStyle.Flat,
            .ForeColor = Color.White,
            .Location = New Point(390, 17),
            .Name = "cbSpotOrderSide",
            .Size = New Size(62, 23)
        }
        _cbSpotOrderSide.Items.AddRange(New Object() {"BUY", "SELL"})
        _cbSpotOrderSide.SelectedIndex = 0
        editor.Controls.Add(_cbSpotOrderSide)

        AddSpotOrderEditorLabel(editor, "Tipo", 460, 1, 68)
        _cbSpotOrderType = New ComboBox With {
            .BackColor = Color.FromArgb(45, 45, 45),
            .DropDownStyle = ComboBoxStyle.DropDownList,
            .FlatStyle = FlatStyle.Flat,
            .ForeColor = Color.White,
            .Location = New Point(460, 17),
            .Name = "cbSpotOrderType",
            .Size = New Size(68, 23)
        }
        _cbSpotOrderType.Items.Add("LIMIT")
        _cbSpotOrderType.SelectedIndex = 0
        AddHandler _cbSpotOrderType.SelectedIndexChanged, AddressOf SpotOrderTypeChanged
        editor.Controls.Add(_cbSpotOrderType)

        AddSpotOrderEditorLabel(editor, "Caixa Spot", 536, 1, 88)
        _lbSpotOrderCash = New Label With {
            .AutoSize = False,
            .BackColor = Color.FromArgb(45, 45, 45),
            .BorderStyle = BorderStyle.FixedSingle,
            .ForeColor = Color.Gold,
            .Location = New Point(536, 17),
            .Name = "lbSpotOrderCash",
            .Size = New Size(88, 23),
            .Text = "$0.00",
            .TextAlign = ContentAlignment.MiddleRight
        }
        editor.Controls.Add(_lbSpotOrderCash)

        AddSpotOrderEditorLabel(editor, "Quantidade", 632, 1, 82)
        _txtSpotOrderQuantity = CreateSpotOrderEditorTextBox("txtSpotOrderQuantity", 632, 17, 82)
        AddHandler _txtSpotOrderQuantity.TextChanged, AddressOf SpotOrderQuantityChanged
        editor.Controls.Add(_txtSpotOrderQuantity)

        AddSpotOrderEditorLabel(editor, "Preço limite", 722, 1, 90)
        _txtSpotOrderPrice = CreateSpotOrderEditorTextBox("txtSpotOrderPrice", 722, 17, 90)
        AddHandler _txtSpotOrderPrice.TextChanged, AddressOf SpotOrderPriceChanged
        editor.Controls.Add(_txtSpotOrderPrice)

        AddSpotOrderEditorLabel(editor, "Valor USDT", 820, 1, 88)
        _txtSpotOrderNotional = CreateSpotOrderEditorTextBox("txtSpotOrderNotional", 820, 17, 88)
        _txtSpotOrderNotional.TextAlign = HorizontalAlignment.Right
        _txtSpotOrderNotional.ForeColor = Color.Gold
        AddHandler _txtSpotOrderNotional.TextChanged, AddressOf SpotOrderNotionalChanged
        editor.Controls.Add(_txtSpotOrderNotional)

        _btAbrirSpotOrder = CreateSpotOrderEditorButton("Enviar ordem", 916, 15, 100)
        _btAbrirSpotOrder.BackColor = Color.DarkGreen
        AddHandler _btAbrirSpotOrder.Click, AddressOf btAbrirSpotOrder_Click
        editor.Controls.Add(_btAbrirSpotOrder)

        _btAtualizarOrdensSpot = CreateSpotOrderEditorButton("Atualizar", 1024, 15, 90)
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
            .SelectionBackColor = Color.FromArgb(64, 64, 64),
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

        AddSpotOrderEditorLabel(editor, "Exchange", 6, 1, 82)
        _cbFuturesOrderExchange = New ComboBox With {
            .BackColor = Color.FromArgb(45, 45, 45),
            .DropDownStyle = ComboBoxStyle.DropDownList,
            .FlatStyle = FlatStyle.Flat,
            .ForeColor = Color.White,
            .Location = New Point(6, 17),
            .Name = "cbFuturesOrderExchange",
            .Size = New Size(82, 23)
        }
        RefreshFuturesOrderExchanges()
        AddHandler _cbFuturesOrderExchange.SelectedIndexChanged, AddressOf FuturesOrderExchangeChanged
        editor.Controls.Add(_cbFuturesOrderExchange)
        ToolTip1.SetToolTip(_cbFuturesOrderExchange, "Exchange usada para carregar saldo, ordens e enviar a ordem")

        AddSpotOrderEditorLabel(editor, "Cripto", 94, 1, 125)
        _cbFuturesOrderSymbol = New ComboBox With {
            .BackColor = Color.FromArgb(45, 45, 45),
            .DropDownStyle = ComboBoxStyle.DropDown,
            .FlatStyle = FlatStyle.Flat,
            .ForeColor = Color.White,
            .Location = New Point(94, 17),
            .Name = "cbFuturesOrderSymbol",
            .Size = New Size(125, 23)
        }
        AddHandler _cbFuturesOrderSymbol.SelectedIndexChanged, AddressOf FuturesOrderSymbolChanged
        AddHandler _cbFuturesOrderSymbol.TextChanged, AddressOf FuturesOrderSymbolTextChanged
        editor.Controls.Add(_cbFuturesOrderSymbol)

        _btGraficoFuturesOrder = CreateSpotOrderEditorButton("↗", 223, 15, 28)
        _btGraficoFuturesOrder.Name = "btGraficoFuturesOrder"
        _btGraficoFuturesOrder.Font = New Font("Calibri", 10.0F, FontStyle.Bold)
        AddHandler _btGraficoFuturesOrder.Click, AddressOf btGraficoFuturesOrder_Click
        editor.Controls.Add(_btGraficoFuturesOrder)
        ToolTip1.SetToolTip(_btGraficoFuturesOrder, "Abrir gráfico da moeda selecionada")

        AddSpotOrderEditorLabel(editor, "Preço atual", 257, 1, 100)
        _txtFuturesOrderCurrentPrice = CreateSpotOrderEditorTextBox("txtFuturesOrderCurrentPrice", 257, 17, 100)
        _txtFuturesOrderCurrentPrice.ReadOnly = True
        _txtFuturesOrderCurrentPrice.TabStop = False
        _txtFuturesOrderCurrentPrice.TextAlign = HorizontalAlignment.Right
        _txtFuturesOrderCurrentPrice.ForeColor = Color.Yellow
        _txtFuturesOrderCurrentPrice.Text = "--"
        editor.Controls.Add(_txtFuturesOrderCurrentPrice)

        AddSpotOrderEditorLabel(editor, "Lado", 365, 1, 70)
        _cbFuturesOrderSide = New ComboBox With {
            .BackColor = Color.FromArgb(45, 45, 45),
            .DropDownStyle = ComboBoxStyle.DropDownList,
            .FlatStyle = FlatStyle.Flat,
            .ForeColor = Color.White,
            .Location = New Point(365, 17),
            .Name = "cbFuturesOrderSide",
            .Size = New Size(70, 23)
        }
        _cbFuturesOrderSide.Items.AddRange(New Object() {"LONG", "SHORT"})
        _cbFuturesOrderSide.SelectedIndex = 0
        editor.Controls.Add(_cbFuturesOrderSide)

        AddSpotOrderEditorLabel(editor, "Tipo", 443, 1, 70)
        _cbFuturesOrderType = New ComboBox With {
            .BackColor = Color.FromArgb(45, 45, 45),
            .DropDownStyle = ComboBoxStyle.DropDownList,
            .FlatStyle = FlatStyle.Flat,
            .ForeColor = Color.White,
            .Location = New Point(443, 17),
            .Name = "cbFuturesOrderType",
            .Size = New Size(70, 23)
        }
        _cbFuturesOrderType.Items.Add("LIMIT")
        _cbFuturesOrderType.SelectedIndex = 0
        AddHandler _cbFuturesOrderType.SelectedIndexChanged, AddressOf FuturesOrderTypeChanged
        editor.Controls.Add(_cbFuturesOrderType)

        AddSpotOrderEditorLabel(editor, "Disponível", 521, 1, 84)
        _lbFuturesOrderAvailable = New Label With {
            .AutoSize = False,
            .BackColor = Color.FromArgb(45, 45, 45),
            .BorderStyle = BorderStyle.FixedSingle,
            .ForeColor = Color.Gold,
            .Location = New Point(521, 17),
            .Name = "lbFuturesOrderAvailable",
            .Size = New Size(84, 23),
            .Text = "$0.00",
            .TextAlign = ContentAlignment.MiddleRight
        }
        editor.Controls.Add(_lbFuturesOrderAvailable)

        AddSpotOrderEditorLabel(editor, "Alavancagem", 613, 1, 70)
        _cbFuturesOrderLeverage = New ComboBox With {
            .BackColor = Color.FromArgb(45, 45, 45),
            .DropDownStyle = ComboBoxStyle.DropDownList,
            .FlatStyle = FlatStyle.Flat,
            .ForeColor = Color.White,
            .Location = New Point(613, 17),
            .Name = "cbFuturesOrderLeverage",
            .Size = New Size(70, 23)
        }
        For leverage As Integer = 0 To 10
            _cbFuturesOrderLeverage.Items.Add($"{leverage}x")
        Next
        _cbFuturesOrderLeverage.SelectedIndex = 0
        editor.Controls.Add(_cbFuturesOrderLeverage)

        AddSpotOrderEditorLabel(editor, "Quantidade", 689, 1, 80)
        _txtFuturesOrderQuantity = CreateSpotOrderEditorTextBox("txtFuturesOrderQuantity", 689, 17, 80)
        AddHandler _txtFuturesOrderQuantity.TextChanged, AddressOf FuturesOrderQuantityChanged
        editor.Controls.Add(_txtFuturesOrderQuantity)

        AddSpotOrderEditorLabel(editor, "Preço limite", 775, 1, 80)
        _txtFuturesOrderPrice = CreateSpotOrderEditorTextBox("txtFuturesOrderPrice", 775, 17, 80)
        AddHandler _txtFuturesOrderPrice.TextChanged, AddressOf FuturesOrderPriceChanged
        editor.Controls.Add(_txtFuturesOrderPrice)

        AddSpotOrderEditorLabel(editor, "Valor USDT", 861, 1, 80)
        _txtFuturesOrderNotional = CreateSpotOrderEditorTextBox("txtFuturesOrderNotional", 861, 17, 80)
        _txtFuturesOrderNotional.TextAlign = HorizontalAlignment.Right
        _txtFuturesOrderNotional.ForeColor = Color.Gold
        AddHandler _txtFuturesOrderNotional.TextChanged, AddressOf FuturesOrderNotionalChanged
        editor.Controls.Add(_txtFuturesOrderNotional)

        _btAbrirFuturesOrder = CreateSpotOrderEditorButton("Enviar ordem", 947, 15, 94)
        _btAbrirFuturesOrder.BackColor = Color.DarkGreen
        _btAbrirFuturesOrder.Name = "btAbrirFuturesOrder"
        AddHandler _btAbrirFuturesOrder.Click, AddressOf btAbrirFuturesOrder_Click
        editor.Controls.Add(_btAbrirFuturesOrder)

        _btAtualizarOrdensFuturos = CreateSpotOrderEditorButton("Atualizar", 1047, 15, 89)
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
            .SelectionBackColor = Color.FromArgb(64, 64, 64),
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

        ' O painel e os controles agora são mantidos pelo Designer do FormMain.
        ' O editor dinâmico antigo permanece apenas como código legado abaixo
        ' deste método, mas não é mais instanciado nem adicionado à tela.
        _panelSpotBot = PanelTradeBot
        _panelSpotBot.Name = "PanelTradeBot"
        _panelSpotBot.BackColor = Color.FromArgb(30, 30, 30)
        _panelSpotBot.Visible = False
        _panelSpotBotEditor = Nothing

        Dim universeSettings As SpotBotUniverseSettings
        Try
            universeSettings = PortfolioRepository.GetSpotBotUniverseSettings()
        Catch ex As Exception
            universeSettings = New SpotBotUniverseSettings()
            Debug.WriteLine("Erro carregando configurações do universo do Tradebot: " & ex.Message)
        End Try

        ConfigureSpotBotManualReferences(universeSettings)
        ConfigureSpotBotManualPositionsGrid()
        ConfigureSpotBotManualActions()
        AddHandler _spotTradeBot.SignalGenerated, AddressOf SpotTradeBot_SignalGenerated
        AddHandler _spotTradeBot.SimulationUpdated, AddressOf SpotTradeBot_SimulationUpdated
        UpdateSpotBotUniverseButtonText()
        Return

        ' Código legado da montagem dinâmica. Mantido fora da compilação para
        ' preservar o histórico sem voltar a criar controles em tempo de execução.
#If False Then
        GoTo BuildSpotBotGrid

        AddBotLabel(editor, "Estratégia base", 6, 3, 150)
        _botBollingerMode = New ComboBox With {
            .Name = "cbSpotBotBollingerMode",
            .DropDownStyle = ComboBoxStyle.DropDownList,
            .BackColor = Color.FromArgb(64, 64, 64),
            .ForeColor = Color.White,
            .FlatStyle = FlatStyle.Flat,
            .Location = New Point(6, 24),
            .Width = 150
        }
        _botBollingerMode.Items.AddRange(New Object() {
            "Base Original Protegida",
            "Desativada"
        })
        _botBollingerMode.SelectedIndex = GetBollingerModeIndex()
        AddHandler _botBollingerMode.SelectedIndexChanged, AddressOf SpotBotBollingerModeChanged
        editor.Controls.Add(_botBollingerMode)

        AddBotLabel(editor, "Estratégias adicionais", 164, 3, 180)
        AddBotStrategyCheckBox(editor, SpotBotStrategyKind.WavePullback, "Pullback EMA34", 164, 24, 105)
        AddBotStrategyCheckBox(editor, SpotBotStrategyKind.BreakoutRetest, "Breakout + Reteste", 274, 24, 125)
        AddBotStrategyCheckBox(editor, SpotBotStrategyKind.MomentumVolume, "Momentum volume", 404, 24, 110)
        AddBotStrategyCheckBox(editor, SpotBotStrategyKind.GridAdaptive, "Grid adaptativo v2", 520, 24, 125)
        AddBotStrategyCheckBox(editor, SpotBotStrategyKind.ValueZonePullback, "Pullback zona de valor", 240, 58, 165)

        If _spotTradeBot.HasRestoredState Then
            For Each strategyCheck In _botStrategyChecks
                strategyCheck.Value.Checked =
                    _spotTradeBot.Configuration.EnabledStrategies.Contains(strategyCheck.Key) OrElse
                    (strategyCheck.Key = SpotBotStrategyKind.GridAdaptive AndAlso
                     _spotTradeBot.Configuration.EnabledStrategies.Contains(SpotBotStrategyKind.GridRegime))
            Next
        End If

        AddBotLabel(editor, "Combinação", 650, 3, 100)
        _botCombination = New ComboBox With {
            .Name = "cbSpotBotCombination",
            .DropDownStyle = ComboBoxStyle.DropDownList,
            .BackColor = Color.FromArgb(64, 64, 64),
            .ForeColor = Color.White,
            .FlatStyle = FlatStyle.Flat,
            .Location = New Point(650, 24),
            .Width = 150
        }
        _botCombination.Items.AddRange(New Object() {
            "Qualquer estratégia",
            "Todas as estratégias",
            "Mínimo de confirmações",
            "Pontuação ponderada"
        })
        _botCombination.SelectedIndex = If(_spotTradeBot.HasRestoredState,
                                          CInt(_spotTradeBot.Configuration.CombinationMode),
                                          CInt(SpotBotCombinationMode.MinimumConfirmations))
        editor.Controls.Add(_botCombination)

        AddBotLabel(editor, "Confirmações", 808, 3, 78)
        _botConfirmations = New NumericUpDown With {
            .Name = "nudSpotBotConfirmations",
            .Minimum = 1,
            .Maximum = 5,
            .Value = 2,
            .BackColor = Color.FromArgb(64, 64, 64),
            .ForeColor = Color.White,
            .Location = New Point(808, 24),
            .Width = 64
        }
        If _spotTradeBot.HasRestoredState Then
            _botConfirmations.Value = Math.Min(
                _botConfirmations.Maximum,
                Math.Max(_botConfirmations.Minimum, _spotTradeBot.Configuration.RequiredConfirmations))
        End If
        editor.Controls.Add(_botConfirmations)

        AddBotLabel(editor, "Candle", 886, 3, 100)
        _botBollingerTimeframe = New ComboBox With {
            .Name = "cbSpotBotBollingerTimeframe",
            .DropDownStyle = ComboBoxStyle.DropDownList,
            .BackColor = Color.FromArgb(64, 64, 64),
            .ForeColor = Color.White,
            .FlatStyle = FlatStyle.Flat,
            .Location = New Point(886, 24),
            .Width = 100
        }
        _botBollingerTimeframe.Items.AddRange(New Object() {"4 horas", "Diário"})
        _botBollingerTimeframe.SelectedIndex = GetBollingerTimeframeIndex()
        editor.Controls.Add(_botBollingerTimeframe)
        UpdateSpotBotBollingerTimeframeAvailability()

        AddBotLabel(editor, "Universo", 6, 58, 100)
        _botUniverseMode = New ComboBox With {
            .Name = "cbSpotBotUniverseMode",
            .DropDownStyle = ComboBoxStyle.DropDownList,
            .BackColor = Color.FromArgb(64, 64, 64),
            .ForeColor = Color.White,
            .FlatStyle = FlatStyle.Flat,
            .Location = New Point(6, 79),
            .Width = 120
        }
        _botUniverseMode.Items.AddRange(New Object() {"Manual", "Top volume"})
        _botUniverseMode.SelectedIndex = If(universeSettings.Mode = SpotBotUniverseMode.TopVolume, 1, 0)
        AddHandler _botUniverseMode.SelectedIndexChanged, AddressOf SpotBotUniverseModeChanged
        editor.Controls.Add(_botUniverseMode)

        _botUniverseButton = CreateBotButton("Selecionar...", 132, 79, 102, Color.FromArgb(55, 75, 105))
        AddHandler _botUniverseButton.Click, AddressOf SpotBotUniverseButton_Click
        editor.Controls.Add(_botUniverseButton)

        Dim simulationLabel As New Label With {
            .AutoSize = False,
            .Width = 205,
            .Height = 20,
            .ForeColor = Color.Gold,
            .Font = New Font("Calibri", 9.0F, FontStyle.Bold),
            .Location = New Point(940, 83),
            .Text = "SIMULAÇÃO — sem ordens reais"
        }
        editor.Controls.Add(simulationLabel)

        _botStart = CreateBotButton("Iniciar bot", 6, 111, 110, Color.DarkGreen)
        AddHandler _botStart.Click, AddressOf SpotBotStart_Click
        editor.Controls.Add(_botStart)

        _botStop = CreateBotButton("Parar bot", 122, 111, 100, Color.IndianRed)
        _botStop.Enabled = False
        AddHandler _botStop.Click, AddressOf SpotBotStop_Click
        editor.Controls.Add(_botStop)

        _botExport = CreateBotButton("Exportar CSV", 1000, 24, 100, Color.FromArgb(55, 75, 105))
        AddHandler _botExport.Click, AddressOf SpotBotExport_Click
        editor.Controls.Add(_botExport)

        _botTrades = CreateBotButton("Trades da run", 884, 24, 110, Color.FromArgb(55, 75, 105))
        AddHandler _botTrades.Click, AddressOf SpotBotTrades_Click
        editor.Controls.Add(_botTrades)

        AddBotLabel(editor, "Saldo virtual (USDT)", 240, 88, 110)
        _botStartingQuote = CreateBotNumeric(1000D, 0D, 100000000D, 2, 240, 107, 98)
        editor.Controls.Add(_botStartingQuote)

        If _spotTradeBot.HasRestoredState Then
            Dim restoredSnapshot = _spotTradeBot.GetSimulationSnapshot()
            _botStartingQuote.Value = Math.Min(
                _botStartingQuote.Maximum,
                Math.Max(_botStartingQuote.Minimum, restoredSnapshot.StartingQuote))
        End If

        AddBotLabel(editor, "Alocação (%)", 348, 88, 86)
        _botAllocation = CreateBotNumeric(10D, 0D, 100D, 2, 348, 107, 82)
        If _spotTradeBot.HasRestoredState Then
            _botAllocation.Value = Math.Min(
                _botAllocation.Maximum,
                Math.Max(_botAllocation.Minimum, _spotTradeBot.Configuration.SimulationAllocationPercent))
        End If
        editor.Controls.Add(_botAllocation)

        AddBotLabel(editor, "Taxa (%)", 440, 88, 70)
        _botFee = CreateBotNumeric(0.1D, 0D, 5D, 3, 440, 107, 70)
        If _spotTradeBot.HasRestoredState Then
            _botFee.Value = Math.Min(
                _botFee.Maximum,
                Math.Max(_botFee.Minimum, _spotTradeBot.Configuration.SimulationFeeRate * 100D))
        End If
        editor.Controls.Add(_botFee)

        AddBotLabel(editor, "TP (%)", 520, 88, 75)
        _botTakeProfit = CreateBotNumeric(2D, 0D, 100D, 2, 520, 107, 75)
        editor.Controls.Add(_botTakeProfit)

        AddBotLabel(editor, "SL (%)", 604, 88, 75)
        _botStopLoss = CreateBotNumeric(1D, 0D, 100D, 2, 604, 107, 75)
        editor.Controls.Add(_botStopLoss)

        AddBotLabel(editor, "Máx. ativos", 704, 88, 90)
        _botUniverseMaxAssets = CreateBotNumeric(universeSettings.MaxAssets, 1D, 500D, 0, 704, 107, 75)
        editor.Controls.Add(_botUniverseMaxAssets)

        AddBotLabel(editor, "Volume mínimo", 790, 88, 105)
        _botUniverseMinVolume = CreateBotNumeric(universeSettings.MinimumQuoteVolume, 0D, 1000000000000D, 0, 790, 107, 112)
        editor.Controls.Add(_botUniverseMinVolume)

        AddBotLabel(editor, "Proteções dinâmicas", 940, 83, 210)
        _botBreakEvenEnabled = New CheckBox With {
            .Text = "BE",
            .AutoSize = False,
            .BackColor = Color.FromArgb(30, 30, 30),
            .ForeColor = Color.White,
            .Location = New Point(940, 107),
            .Size = New Size(40, 23),
            .Checked = True,
            .Cursor = Cursors.Hand
        }
        editor.Controls.Add(_botBreakEvenEnabled)
        _botBreakEvenTrigger = CreateBotNumeric(1.5D, 0D, 100D, 2, 980, 107, 62)
        editor.Controls.Add(_botBreakEvenTrigger)

        _botTrailingEnabled = New CheckBox With {
            .Text = "Trail",
            .AutoSize = False,
            .BackColor = Color.FromArgb(30, 30, 30),
            .ForeColor = Color.White,
            .Location = New Point(1046, 107),
            .Size = New Size(50, 23),
            .Checked = False,
            .Cursor = Cursors.Hand
        }
        editor.Controls.Add(_botTrailingEnabled)
        _botTrailingDistance = CreateBotNumeric(1D, 0.01D, 100D, 2, 1096, 107, 62)
        editor.Controls.Add(_botTrailingDistance)
        ToolTip1.SetToolTip(_botBreakEvenEnabled, "Move o stop para a entrada após o gatilho (%)")
        ToolTip1.SetToolTip(_botBreakEvenTrigger, "Gatilho do break-even em percentual")
        ToolTip1.SetToolTip(_botTrailingEnabled, "Depois do break-even, acompanha o maior preço")
        ToolTip1.SetToolTip(_botTrailingDistance, "Distância do trailing em percentual")
        If _spotTradeBot.HasRestoredState Then
            _botBreakEvenEnabled.Checked = _spotTradeBot.Configuration.BreakEvenEnabled
            _botBreakEvenTrigger.Value = Math.Min(
                _botBreakEvenTrigger.Maximum,
                Math.Max(_botBreakEvenTrigger.Minimum, _spotTradeBot.Configuration.BreakEvenTriggerPercent))
            _botTrailingEnabled.Checked = _spotTradeBot.Configuration.TrailingStopEnabled
            _botTrailingDistance.Value = Math.Min(
                _botTrailingDistance.Maximum,
                Math.Max(_botTrailingDistance.Minimum, _spotTradeBot.Configuration.TrailingStopDistancePercent))
        End If

        AddBotLabel(editor, "Cooldown pós-saída (min)", 6, 136, 160)
        _botPostExitCooldown = CreateBotNumeric(15D, 0D, 1440D, 0, 6, 155, 82)
        If _spotTradeBot.HasRestoredState Then
            _botPostExitCooldown.Value = Math.Min(
                _botPostExitCooldown.Maximum,
                Math.Max(_botPostExitCooldown.Minimum, _spotTradeBot.Configuration.PostExitCooldownMinutes))
        End If
        editor.Controls.Add(_botPostExitCooldown)
        ToolTip1.SetToolTip(
            _botPostExitCooldown,
            "Tempo mínimo antes de permitir uma nova entrada no mesmo ativo após uma saída. Stops, alvos e fechamento manual não são atrasados.")

        AddBotLabel(editor, "Negócios 24h min.", 175, 136, 130)
        _botUniverseMinTrades = CreateBotNumeric(universeSettings.MinimumTrades24h, 0D, 10000000D, 0, 175, 155, 90)
        editor.Controls.Add(_botUniverseMinTrades)
        ToolTip1.SetToolTip(
            _botUniverseMinTrades,
            "Filtro de liquidez: quantidade mínima de negócios do par nas últimas 24 horas.")

        AddBotLabel(editor, "Grid risco/nível (%)", 280, 136, 125)
        _botGridRiskPercent = CreateBotNumeric(
            _spotTradeBot.Configuration.GridV2RiskPerTradePercent,
            0D,
            100D,
            2,
            280,
            155,
            72)
        editor.Controls.Add(_botGridRiskPercent)

        AddBotLabel(editor, "Reserva (%)", 360, 136, 70)
        _botGridReservePercent = CreateBotNumeric(
            _spotTradeBot.Configuration.GridV2ReserveQuotePercent,
            0D,
            95D,
            2,
            360,
            155,
            65)
        editor.Controls.Add(_botGridReservePercent)

        AddBotLabel(editor, "Exposição máx. (%)", 433, 136, 112)
        _botGridMaxExposurePercent = CreateBotNumeric(
            _spotTradeBot.Configuration.GridV2MaxExposurePercent,
            0D,
            100D,
            2,
            433,
            155,
            78)
        editor.Controls.Add(_botGridMaxExposurePercent)

        AddBotLabel(editor, "Níveis", 519, 136, 52)
        _botGridMaxLevels = CreateBotNumeric(
            _spotTradeBot.Configuration.GridV2MaxLevels,
            1D,
            20D,
            0,
            519,
            155,
            58)
        editor.Controls.Add(_botGridMaxLevels)

        _botGridRequireRejection = New CheckBox With {
            .Text = "Confirmar rejeição",
            .AutoSize = False,
            .BackColor = Color.FromArgb(30, 30, 30),
            .ForeColor = Color.White,
            .Location = New Point(585, 155),
            .Size = New Size(135, 23),
            .Checked = _spotTradeBot.Configuration.GridV2RequireRejection,
            .Cursor = Cursors.Hand
        }
        editor.Controls.Add(_botGridRequireRejection)
        ToolTip1.SetToolTip(
            _botGridRequireRejection,
            "Exige toque do suporte, fechamento acima do nível e confirmação antes da compra.")
        ToolTip1.SetToolTip(
            _botGridRiskPercent,
            "Risco máximo estimado de cada nível, calculado pela distância até o stop.")
        ToolTip1.SetToolTip(
            _botGridReservePercent,
            "Percentual do saldo inicial reservado e nunca usado pela grade.")
        ToolTip1.SetToolTip(
            _botGridMaxExposurePercent,
            "Exposição máxima acumulada por ativo, em relação ao saldo inicial.")
        ToolTip1.SetToolTip(
            _botGridMaxLevels,
            "Quantidade máxima de entradas acumuladas no mesmo ativo.")

        AddBotLabel(editor, "Flush BTC (%)", 730, 136, 92)
        _botBtcFlushDropPercent = CreateBotNumeric(
            _spotTradeBot.Configuration.BitcoinFlushDropPercent,
            0.1D,
            20D,
            2,
            730,
            155,
            72)
        editor.Controls.Add(_botBtcFlushDropPercent)

        AddBotLabel(editor, "Pausa (min)", 810, 136, 78)
        _botBtcFlushPauseMinutes = CreateBotNumeric(
            _spotTradeBot.Configuration.BitcoinFlushPauseMinutes,
            1D,
            1440D,
            0,
            810,
            155,
            72)
        editor.Controls.Add(_botBtcFlushPauseMinutes)

        _botBtcFlushEnabled = New CheckBox With {
            .Text = "Proteger no flush BTC",
            .AutoSize = False,
            .BackColor = Color.FromArgb(30, 30, 30),
            .ForeColor = Color.White,
            .Location = New Point(890, 155),
            .Size = New Size(155, 23),
            .Checked = _spotTradeBot.Configuration.BitcoinFlushProtectionEnabled,
            .Cursor = Cursors.Hand
        }
        editor.Controls.Add(_botBtcFlushEnabled)

        AddBotLabel(editor, "Vol. x", 1050, 136, 55)
        _botBtcFlushVolumeMultiplier = CreateBotNumeric(
            _spotTradeBot.Configuration.BitcoinFlushVolumeMultiplier,
            1.1D,
            20D,
            2,
            1050,
            155,
            65)
        editor.Controls.Add(_botBtcFlushVolumeMultiplier)
        ToolTip1.SetToolTip(
            _botBtcFlushEnabled,
            "Pausa novas entradas quando a queda do BTC é confirmada por volume anormal.")
        ToolTip1.SetToolTip(
            _botBtcFlushDropPercent,
            "Queda acumulada do BTC no período de 5 candles de 1 minuto.")
        ToolTip1.SetToolTip(
            _botBtcFlushPauseMinutes,
            "Tempo da pausa de novas entradas após um flush confirmado.")
        ToolTip1.SetToolTip(
            _botBtcFlushVolumeMultiplier,
            "Volume do candle atual em relação à média dos candles anteriores.")

        AddBotLabel(editor, "Queda diária (%)", 730, 180, 78)
        _botBtcDailyDropPercent = CreateBotNumeric(
            _spotTradeBot.Configuration.BitcoinDailyDropPercent,
            0.1D,
            50D,
            2,
            730,
            199,
            72)
        editor.Controls.Add(_botBtcDailyDropPercent)

        AddBotLabel(editor, "Oscilação do dia (%)", 810, 180, 105)
        _botBtcDailyDrawdownPercent = CreateBotNumeric(
            _spotTradeBot.Configuration.BitcoinDailyDrawdownPercent,
            0.1D,
            50D,
            2,
            810,
            199,
            90)
        editor.Controls.Add(_botBtcDailyDrawdownPercent)

        _botBtcDailyEnabled = New CheckBox With {
            .Text = "Proteger no diário",
            .AutoSize = False,
            .BackColor = Color.FromArgb(30, 30, 30),
            .ForeColor = Color.White,
            .Location = New Point(910, 199),
            .Size = New Size(145, 23),
            .Checked = _spotTradeBot.Configuration.BitcoinDailyProtectionEnabled,
            .Cursor = Cursors.Hand
        }
        editor.Controls.Add(_botBtcDailyEnabled)
        ToolTip1.SetToolTip(
            _botBtcDailyDropPercent,
            "Pausa novas entradas quando o BTC estiver abaixo do fechamento diário anterior por este percentual.")
        ToolTip1.SetToolTip(
            _botBtcDailyDrawdownPercent,
            "Pausa quando a queda da máxima do candle diário em formação atingir este percentual, desde que o candle esteja negativo.")
        ToolTip1.SetToolTip(
            _botBtcDailyEnabled,
            "Usa o candle diário em formação para detectar queda e oscilação fortes do BTC.")

        ' O editor antigo era uma lista única de controles criados em tempo de
        ' execução. Reorganizamos essa lista em páginas nomeadas para que o
        ' usuário consiga ajustar cada grupo sem sobrepor o grid/log.
        Dim existingEditorControls = editor.Controls.Cast(Of Control)().ToList()
        Dim settingsTabs As New TabControl With {
            .Name = "tcSpotBotSettings",
            .Dock = DockStyle.Fill,
            .BackColor = Color.FromArgb(30, 30, 30),
            .ForeColor = Color.White,
            .Alignment = TabAlignment.Top,
            .HotTrack = True}
        Dim strategyPage As New TabPage("Estratégias e execução") With {
            .Name = "tpSpotBotStrategies",
            .BackColor = Color.FromArgb(30, 30, 30),
            .ForeColor = Color.White,
            .AutoScroll = True}
        Dim riskPage As New TabPage("Freio de perdas") With {
            .Name = "tpSpotBotRiskBrake",
            .BackColor = Color.FromArgb(30, 30, 30),
            .ForeColor = Color.White,
            .AutoScroll = True}
        For Each child In existingEditorControls
            editor.Controls.Remove(child)
            child.Top += 22
            strategyPage.Controls.Add(child)
        Next

        AddBotLabel(riskPage, "Ativar freio", 10, 14, 100)
        _botRiskBrakeEnabled = New CheckBox With {
            .Name = "chkSpotBotRiskBrakeEnabled",
            .Text = "Proteção geral",
            .AutoSize = True,
            .BackColor = Color.FromArgb(30, 30, 30),
            .ForeColor = Color.White,
            .Location = New Point(10, 35),
            .Checked = _spotTradeBot.Configuration.RiskBrakeEnabled,
            .Cursor = Cursors.Hand}
        riskPage.Controls.Add(_botRiskBrakeEnabled)

        AddBotLabel(riskPage, "Perda máx. da sessão (%)", 155, 14, 150)
        _botRiskSessionLoss = CreateBotNumeric(_spotTradeBot.Configuration.RiskBrakeSessionLossPercent, 0D, 100D, 2, 155, 35, 90)
        riskPage.Controls.Add(_botRiskSessionLoss)

        AddBotLabel(riskPage, "Trava de lucro", 265, 14, 120)
        _botRiskProfitLockEnabled = New CheckBox With {
            .Name = "chkSpotBotRiskProfitLock",
            .Text = "Ativa",
            .AutoSize = True,
            .BackColor = Color.FromArgb(30, 30, 30),
            .ForeColor = Color.White,
            .Location = New Point(265, 35),
            .Checked = _spotTradeBot.Configuration.RiskBrakeProfitLockEnabled,
            .Cursor = Cursors.Hand}
        riskPage.Controls.Add(_botRiskProfitLockEnabled)

        AddBotLabel(riskPage, "Armar em ganho (%)", 345, 14, 130)
        _botRiskProfitTrigger = CreateBotNumeric(_spotTradeBot.Configuration.RiskBrakeProfitLockTriggerPercent, 0D, 100D, 2, 345, 35, 90)
        riskPage.Controls.Add(_botRiskProfitTrigger)

        AddBotLabel(riskPage, "Devolver do pico (%)", 455, 14, 135)
        _botRiskGiveback = CreateBotNumeric(_spotTradeBot.Configuration.RiskBrakeProfitGivebackPercent, 0D, 100D, 0, 455, 35, 90)
        riskPage.Controls.Add(_botRiskGiveback)

        AddBotLabel(riskPage, "Máx. perdas seguidas", 10, 80, 140)
        _botRiskMaxLosses = CreateBotNumeric(_spotTradeBot.Configuration.RiskBrakeMaxConsecutiveLosses, 0D, 100D, 0, 10, 101, 90)
        riskPage.Controls.Add(_botRiskMaxLosses)

        AddBotLabel(riskPage, "Pausa após sequência (min)", 120, 80, 180)
        _botRiskPauseMinutes = CreateBotNumeric(_spotTradeBot.Configuration.RiskBrakePauseMinutes, 1D, 10080D, 0, 120, 101, 100)
        riskPage.Controls.Add(_botRiskPauseMinutes)

        _botRiskClosePositions = New CheckBox With {
            .Name = "chkSpotBotRiskClosePositions",
            .Text = "Fechar posições no Handbrake",
            .AutoSize = True,
            .BackColor = Color.FromArgb(30, 30, 30),
            .ForeColor = Color.White,
            .Location = New Point(245, 104),
            .Checked = _spotTradeBot.Configuration.RiskBrakeClosePositionsOnHardStop,
            .Cursor = Cursors.Hand}
        riskPage.Controls.Add(_botRiskClosePositions)

        _botRiskUseUnrealized = New CheckBox With {
            .Name = "chkSpotBotRiskUseUnrealized",
            .Text = "Considerar P/L aberto",
            .AutoSize = True,
            .BackColor = Color.FromArgb(30, 30, 30),
            .ForeColor = Color.White,
            .Location = New Point(470, 104),
            .Checked = _spotTradeBot.Configuration.RiskBrakeUseUnrealizedPnl,
            .Cursor = Cursors.Hand}
        riskPage.Controls.Add(_botRiskUseUnrealized)

        ToolTip1.SetToolTip(_botRiskSessionLoss, "Limite de perda em relação ao patrimônio no início da simulação.")
        ToolTip1.SetToolTip(_botRiskProfitTrigger, "A trava arma quando o patrimônio atingir este ganho.")
        ToolTip1.SetToolTip(_botRiskGiveback, "Percentual do ganho máximo que pode ser devolvido antes do freio.")
        ToolTip1.SetToolTip(_botRiskMaxLosses, "Zero desativa o freio por sequência de perdas.")
        ToolTip1.SetToolTip(_botRiskClosePositions, "No Handbrake, encerra as posições simuladas no preço atual.")
        ToolTip1.SetToolTip(_botRiskUseUnrealized, "Usa o patrimônio marcado a mercado, incluindo P/L aberto.")

        settingsTabs.TabPages.Add(strategyPage)
        settingsTabs.TabPages.Add(riskPage)
        editor.Controls.Add(settingsTabs)

BuildSpotBotGrid:
        _botLog = New RichTextBox With {
            .Name = "rtbSpotBotLog",
            .Dock = DockStyle.Bottom,
            .BackColor = Color.Black,
            .ForeColor = Color.White,
            .BorderStyle = BorderStyle.FixedSingle,
            .ReadOnly = True,
            .DetectUrls = False,
            .Font = New Font("Consolas", 10.0F),
            .ScrollBars = RichTextBoxScrollBars.Vertical,
            .Height = 40
        }

        _dgSpotBotPositions = New DataGridView With {
            .Name = "dgSpotBotPositions",
            .Dock = DockStyle.Top,
            .Height = 150,
            .AllowUserToAddRows = False,
            .AllowUserToDeleteRows = False,
            .AllowUserToResizeRows = False,
            .AllowUserToOrderColumns = False,
            .AutoGenerateColumns = False,
            .AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            .BackgroundColor = Color.FromArgb(64, 64, 64),
            .BorderStyle = BorderStyle.FixedSingle,
            .CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
            .ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None,
            .EnableHeadersVisualStyles = False,
            .ReadOnly = True,
            .MultiSelect = False,
            .SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            .RowHeadersVisible = False,
            .ScrollBars = ScrollBars.Vertical
        }
        _dgSpotBotPositions.ColumnHeadersDefaultCellStyle = New DataGridViewCellStyle With {
            .Alignment = DataGridViewContentAlignment.MiddleLeft,
            .BackColor = Color.FromArgb(40, 40, 40),
            .Font = New Font("Calibri", 9.0F, FontStyle.Italic),
            .ForeColor = Color.Turquoise,
            .SelectionBackColor = Color.FromArgb(40, 40, 40),
            .SelectionForeColor = Color.Turquoise,
            .WrapMode = DataGridViewTriState.False
        }
        _dgSpotBotPositions.DefaultCellStyle = New DataGridViewCellStyle With {
            .Alignment = DataGridViewContentAlignment.MiddleLeft,
            .BackColor = Color.FromArgb(64, 64, 64),
            .Font = New Font("Calibri", 10.0F),
            .ForeColor = Color.White,
            .SelectionBackColor = Color.FromArgb(64, 64, 64),
            .SelectionForeColor = Color.White,
            .WrapMode = DataGridViewTriState.False
        }
        _dgSpotBotPositions.RowTemplate.Height = 28
        _dgSpotBotPositions.ColumnHeadersHeight = 25
        AddSpotBotPositionColumn("Symbol", "Cripto")
        AddSpotBotPositionColumn("Strategy", "Estratégia")
        AddSpotBotPositionColumn("Side", "Lado")
        AddSpotBotPositionColumn("Quantity", "Qtd")
        AddSpotBotPositionColumn("Notional", "Notional")
        AddSpotBotPositionColumn("EntryPrice", "Entrada")
        AddSpotBotPositionColumn("CurrentPrice", "Preço atual")
        AddSpotBotPositionColumn("StopPrice", "SL")
        AddSpotBotPositionColumn("UnrealizedPnl", "PnL")
        AddSpotBotPositionColumn("RoiPercent", "ROI %")
        AddSpotBotPositionColumn("GridLevelCount", "Níveis")

        Dim closeBotPositionColumn As New DataGridViewButtonColumn With {
            .Name = "ClosePosition",
            .HeaderText = "Ação",
            .Text = "Fechar",
            .UseColumnTextForButtonValue = True,
            .SortMode = DataGridViewColumnSortMode.NotSortable,
            .AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
            .Width = 72,
            .FlatStyle = FlatStyle.Flat
        }
        closeBotPositionColumn.DefaultCellStyle = New DataGridViewCellStyle With {
            .Alignment = DataGridViewContentAlignment.MiddleCenter,
            .BackColor = Color.IndianRed,
            .ForeColor = Color.White,
            .SelectionBackColor = Color.IndianRed,
            .SelectionForeColor = Color.White
        }
        _dgSpotBotPositions.Columns.Add(closeBotPositionColumn)
        AddHandler _dgSpotBotPositions.CellClick, AddressOf SpotBotPositions_CellClick
        AddHandler _dgSpotBotPositions.CellContentClick, AddressOf SpotBotPositions_CellContentClick
        AddHandler _dgSpotBotPositions.CellMouseEnter, AddressOf SpotBotPositions_CellMouseEnter
        AddHandler _dgSpotBotPositions.CellMouseLeave, AddressOf SpotBotPositions_CellMouseLeave

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
        _panelSpotBot.Controls.Add(_dgSpotBotPositions)
        _panelSpotBot.Controls.Add(_botSimulationSummary)
        _panelSpotBot.Controls.Add(editor)
        Controls.Add(_panelSpotBot)

        _tradeBotButton = New Button With {
            .Name = "btTradeBot",
            .Text = "Tradebot",
            .BackColor = Color.FromArgb(20, 20, 20),
            .ForeColor = Color.White,
            .FlatStyle = FlatStyle.Flat,
            .Cursor = Cursors.Hand,
            .Location = New Point(280, 432),
            .Size = New Size(85, 23)
        }
        _tradeBotButton.FlatAppearance.BorderSize = 0
        _tradeBotButton.FlatAppearance.MouseOverBackColor = Color.RoyalBlue
        _tradeBotButton.FlatAppearance.MouseDownBackColor = Color.DodgerBlue

        Controls.Add(_tradeBotButton)
        ArrangeSpotBotButton()

        AddHandler _spotTradeBot.SignalGenerated, AddressOf SpotTradeBot_SignalGenerated
        AddHandler _spotTradeBot.SimulationUpdated, AddressOf SpotTradeBot_SimulationUpdated
        UpdateSpotBotUniverseButtonText()
#End If
    End Sub

    Private Sub ConfigureDcaPanel()
        If _panelDca IsNot Nothing Then Return

        _panelDca = New Panel With {
            .Name = "PanelDca",
            .BackColor = Color.FromArgb(30, 30, 30),
            .Padding = New Padding(14, 0, 14, 0),
            .Visible = False}

        Dim editor As New Panel With {
            .Dock = DockStyle.Top,
            .Height = 126,
            .BackColor = Color.FromArgb(30, 30, 30)}

        AddBotLabel(editor, "Exchange", 6, 3, 80)
        _cbDcaExchange = New ComboBox With {
            .Name = "cbDcaExchange",
            .DropDownStyle = ComboBoxStyle.DropDownList,
            .BackColor = Color.FromArgb(64, 64, 64),
            .ForeColor = Color.White,
            .FlatStyle = FlatStyle.Flat,
            .Location = New Point(6, 24),
            .Width = 88}
        _cbDcaExchange.Items.AddRange(New Object() {"BINANCE", "GATE.IO"})
        _cbDcaExchange.SelectedIndex = 0
        AddHandler _cbDcaExchange.SelectedIndexChanged, AddressOf DcaExchange_SelectedIndexChanged
        editor.Controls.Add(_cbDcaExchange)

        AddBotLabel(editor, "Moeda", 102, 3, 110)
        _cbDcaSymbol = New ComboBox With {
            .Name = "cbDcaSymbol",
            .DropDownStyle = ComboBoxStyle.DropDownList,
            .BackColor = Color.FromArgb(64, 64, 64),
            .ForeColor = Color.White,
            .FlatStyle = FlatStyle.Flat,
            .Location = New Point(102, 24),
            .Width = 125}
        For Each symbol In GetManualSpotBotUniverseCandidates()
            _cbDcaSymbol.Items.Add(symbol)
        Next
        If _cbDcaSymbol.Items.Count > 0 Then _cbDcaSymbol.SelectedIndex = 0
        editor.Controls.Add(_cbDcaSymbol)

        AddBotLabel(editor, "Caixa", 240, 3, 110)
        _nudDcaBudget = CreateBotNumeric(0D, 0D, 100000000D, 2, 240, 24, 125)
        editor.Controls.Add(_nudDcaBudget)

        AddBotLabel(editor, "Níveis/ajustes valor:alocação", 378, 3, 250)
        _txtDcaLevels = New TextBox With {
            .Name = "txtDcaLevels",
            .BackColor = Color.FromArgb(64, 64, 64),
            .ForeColor = Color.White,
            .BorderStyle = BorderStyle.FixedSingle,
            .Location = New Point(378, 24),
            .Width = 250,
            .Text = ""}
        editor.Controls.Add(_txtDcaLevels)

        AddBotLabel(editor, "Intervalo (min)", 640, 3, 110)
        _nudDcaCooldown = CreateBotNumeric(60D, 0D, 10080D, 0, 640, 24, 90)
        editor.Controls.Add(_nudDcaCooldown)

        _chkDcaEnabled = New CheckBox With {
            .Name = "chkDcaEnabled",
            .Text = "Ativo",
            .Checked = True,
            .AutoSize = True,
            .ForeColor = Color.White,
            .BackColor = Color.FromArgb(30, 30, 30),
            .Location = New Point(738, 27),
            .Cursor = Cursors.Hand}
        editor.Controls.Add(_chkDcaEnabled)

        _chkDcaAutomaticSupports = New CheckBox With {
            .Name = "chkDcaAutomaticSupports",
            .Text = "Suportes automáticos",
            .Checked = True,
            .AutoSize = True,
            .ForeColor = Color.White,
            .BackColor = Color.FromArgb(30, 30, 30),
            .Location = New Point(6, 78),
            .Cursor = Cursors.Hand}
        AddHandler _chkDcaAutomaticSupports.CheckedChanged, AddressOf DcaAutomaticSupports_CheckedChanged
        editor.Controls.Add(_chkDcaAutomaticSupports)

        AddBotLabel(editor, "Período", 150, 55, 70)
        _cbDcaTimeframe = New ComboBox With {
            .Name = "cbDcaTimeframe",
            .DropDownStyle = ComboBoxStyle.DropDownList,
            .BackColor = Color.FromArgb(64, 64, 64),
            .ForeColor = Color.White,
            .FlatStyle = FlatStyle.Flat,
            .Location = New Point(150, 76),
            .Width = 72}
        _cbDcaTimeframe.Items.AddRange(New Object() {"4 horas", "Diário"})
        _cbDcaTimeframe.SelectedIndex = 0
        editor.Controls.Add(_cbDcaTimeframe)

        AddBotLabel(editor, "Lookback", 230, 55, 70)
        _nudDcaLookback = CreateBotNumeric(200D, 30D, 1000D, 0, 230, 76, 65)
        editor.Controls.Add(_nudDcaLookback)

        AddBotLabel(editor, "Tolerância %", 305, 55, 80)
        _nudDcaTolerance = CreateBotNumeric(0.5D, 0.05D, 10D, 2, 305, 76, 65)
        editor.Controls.Add(_nudDcaTolerance)

        AddBotLabel(editor, "Mín. toques", 380, 55, 80)
        _nudDcaMinimumTouches = CreateBotNumeric(2D, 1D, 10D, 0, 380, 76, 65)
        editor.Controls.Add(_nudDcaMinimumTouches)

        AddBotLabel(editor, "Máx. níveis", 455, 55, 80)
        _nudDcaMaximumLevels = CreateBotNumeric(4D, 1D, 10D, 0, 455, 76, 65)
        editor.Controls.Add(_nudDcaMaximumLevels)

        _chkDcaRequireRejection = New CheckBox With {
            .Name = "chkDcaRequireRejection",
            .Text = "Exigir rejeição",
            .Checked = True,
            .AutoSize = True,
            .ForeColor = Color.White,
            .BackColor = Color.FromArgb(30, 30, 30),
            .Location = New Point(530, 78),
            .Cursor = Cursors.Hand}
        editor.Controls.Add(_chkDcaRequireRejection)

        _btDcaAdd = CreateBotButton("Salvar plano", 800, 24, 110, Color.FromArgb(55, 75, 105))
        AddHandler _btDcaAdd.Click, AddressOf DcaSavePlan_Click
        editor.Controls.Add(_btDcaAdd)
        _btDcaStart = CreateBotButton("Iniciar DCA", 920, 24, 100, Color.DarkGreen)
        AddHandler _btDcaStart.Click, AddressOf DcaStart_Click
        editor.Controls.Add(_btDcaStart)
        _btDcaStop = CreateBotButton("Parar", 1030, 24, 75, Color.IndianRed)
        _btDcaStop.Enabled = False
        AddHandler _btDcaStop.Click, AddressOf DcaStop_Click
        editor.Controls.Add(_btDcaStop)
        _btDcaReset = CreateBotButton("Zerar DCA", 1115, 24, 105, Color.FromArgb(80, 60, 35))
        AddHandler _btDcaReset.Click, AddressOf DcaReset_Click
        editor.Controls.Add(_btDcaReset)

        _lbDcaStatus = New Label With {
            .Name = "lbDcaStatus",
            .Dock = DockStyle.Bottom,
            .Height = 24,
            .BackColor = Color.FromArgb(20, 20, 20),
            .ForeColor = Color.Gold,
            .Padding = New Padding(4, 0, 4, 0),
            .TextAlign = ContentAlignment.MiddleLeft,
            .Text = "DCA: aguardando início"}

        _dgDca = New DataGridView With {
            .Name = "dgDca",
            .Dock = DockStyle.Fill,
            .AllowUserToAddRows = False,
            .AllowUserToDeleteRows = False,
            .AllowUserToResizeRows = False,
            .AutoGenerateColumns = False,
            .AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            .BackgroundColor = Color.FromArgb(64, 64, 64),
            .BorderStyle = BorderStyle.FixedSingle,
            .CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
            .EnableHeadersVisualStyles = False,
            .ReadOnly = True,
            .RowHeadersVisible = False,
            .SelectionMode = DataGridViewSelectionMode.FullRowSelect}
        _dgDca.ColumnHeadersDefaultCellStyle = New DataGridViewCellStyle With {
            .Alignment = DataGridViewContentAlignment.MiddleLeft,
            .BackColor = Color.FromArgb(40, 40, 40),
            .ForeColor = Color.Turquoise,
            .Font = New Font("Calibri", 9.0F, FontStyle.Italic)}
        _dgDca.DefaultCellStyle = New DataGridViewCellStyle With {
            .Alignment = DataGridViewContentAlignment.MiddleLeft,
            .BackColor = Color.FromArgb(64, 64, 64),
            .ForeColor = Color.White,
            .SelectionBackColor = Color.FromArgb(64, 64, 64),
            .SelectionForeColor = Color.White,
            .Font = New Font("Calibri", 10.0F)}
        AddDcaGridColumn("Symbol", "Cripto")
        AddDcaGridColumn("Exchange", "Exchange")
        AddDcaGridColumn("Enabled", "Ativo")
        AddDcaGridColumn("Anchor", "Topo")
        AddDcaGridColumn("LastPrice", "Preço")
        AddDcaGridColumn("Drop", "Queda")
        AddDcaGridColumn("Quantity", "Qtd")
        AddDcaGridColumn("Invested", "Investido")
        AddDcaGridColumn("Average", "Preço médio")
        AddDcaGridColumn("Supports", "Suportes")
        AddDcaGridColumn("Pnl", "PnL")
        AddDcaGridColumn("Roi", "ROI %")

        Dim removeColumn As New DataGridViewButtonColumn With {
            .Name = "DcaRemove",
            .HeaderText = "Ação",
            .Text = "Excluir",
            .UseColumnTextForButtonValue = True,
            .AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
            .Width = 62,
            .FlatStyle = FlatStyle.Flat,
            .ReadOnly = True}
        removeColumn.DefaultCellStyle = New DataGridViewCellStyle With {
            .Alignment = DataGridViewContentAlignment.MiddleCenter,
            .BackColor = Color.IndianRed,
            .ForeColor = Color.White,
            .SelectionBackColor = Color.IndianRed,
            .SelectionForeColor = Color.White}
        _dgDca.Columns.Add(removeColumn)
        AddHandler _dgDca.CellContentClick, AddressOf DcaGrid_CellContentClick

        _panelDca.Controls.Add(_dgDca)
        _panelDca.Controls.Add(_lbDcaStatus)
        _panelDca.Controls.Add(editor)
        Controls.Add(_panelDca)

        AddHandler _spotDca.PurchaseSimulated, AddressOf Dca_PurchaseSimulated
        AddHandler _spotDca.SimulationUpdated, AddressOf Dca_SimulationUpdated
        DcaExchange_SelectedIndexChanged(Nothing, EventArgs.Empty)
        UpdateDcaGrid()

        _btDca = New Button With {
            .Name = "btDca",
            .Text = "DCA",
            .BackColor = Color.FromArgb(20, 20, 20),
            .ForeColor = Color.White,
            .FlatStyle = FlatStyle.Flat,
            .Cursor = Cursors.Hand,
            .Size = New Size(55, 23)}
        _btDca.FlatAppearance.BorderSize = 0
        _btDca.FlatAppearance.MouseOverBackColor = Color.RoyalBlue
        _btDca.FlatAppearance.MouseDownBackColor = Color.DodgerBlue
        AddHandler _btDca.Click, AddressOf btDca_Click
        Controls.Add(_btDca)
        ArrangeDcaButton()
    End Sub

    Private Sub AddDcaGridColumn(name As String, headerText As String)
        _dgDca.Columns.Add(New DataGridViewTextBoxColumn With {
            .Name = name,
            .HeaderText = headerText,
            .SortMode = DataGridViewColumnSortMode.NotSortable,
            .AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill})
    End Sub

    Private Async Function LoadDcaHistoricalCandlesAsync(
        plan As DcaPlan,
        interval As String,
        limit As Integer) As Task(Of List(Of DcaCandle))

        If plan IsNot Nothing AndAlso String.Equals(plan.Exchange, "GATE.IO", StringComparison.OrdinalIgnoreCase) Then
            Return Await _gateSpot.GATE_GetSpotKlinesAsync(plan.Symbol, interval, limit)
        End If

        Return Await B.BINANCE_GetSpotKlinesAsync(plan?.Symbol, interval, limit)
    End Function

    Private Async Function LoadSpotBotHistoricalCandlesAsync(
        symbol As String,
        interval As String,
        limit As Integer) As Task(Of List(Of DcaCandle))

        Return Await B.BINANCE_GetSpotKlinesAsync(symbol, interval, limit)
    End Function

    Private Async Sub DcaExchange_SelectedIndexChanged(sender As Object, e As EventArgs)
        Await RefreshDcaExchangeCashAsync()
    End Sub

    Private Async Function RefreshDcaExchangeCashAsync() As Task
        If _cbDcaExchange Is Nothing OrElse _nudDcaBudget Is Nothing OrElse _dcaCashRefreshRunning Then Return

        _dcaCashRefreshRunning = True
        Dim selectedExchange = If(_cbDcaExchange.Text.Trim().ToUpperInvariant(), "BINANCE")
        Try
            Dim assets As Dictionary(Of String, Decimal)
            If String.Equals(selectedExchange, "GATE.IO", StringComparison.OrdinalIgnoreCase) Then
                assets = Await _gateSpot.GATE_GetAllSpotAssets()
            Else
                assets = Await B.BINANCE_GetSpotAssetsFull()
            End If

            Dim cash = 0D
            If assets IsNot Nothing Then
                For Each asset In assets
                    If Cjson.stablecoins.Contains(asset.Key) Then cash += Math.Max(0D, asset.Value)
                Next
            End If

            ' Não sobrescreve o campo se o usuário trocou de exchange durante a consulta.
            If String.Equals(selectedExchange, _cbDcaExchange.Text.Trim().ToUpperInvariant(), StringComparison.OrdinalIgnoreCase) Then
                _nudDcaBudget.Value = Math.Max(_nudDcaBudget.Minimum, Math.Min(_nudDcaBudget.Maximum, cash))
                SetDcaStatus($"Caixa {selectedExchange}: {cash:0.00} USDT.", Color.Gold)
            End If
        Catch ex As Exception
            Debug.WriteLine($"Erro carregando caixa DCA {selectedExchange}: {ex.Message}")
            If String.Equals(selectedExchange, _cbDcaExchange.Text.Trim().ToUpperInvariant(), StringComparison.OrdinalIgnoreCase) Then
                SetDcaStatus($"Caixa {selectedExchange}: não foi possível consultar o saldo.", Color.IndianRed)
            End If
        Finally
            _dcaCashRefreshRunning = False
        End Try
    End Function

    Private Async Sub DcaSavePlan_Click(sender As Object, e As EventArgs)
        Try
            Dim automaticSupports = _chkDcaAutomaticSupports IsNot Nothing AndAlso _chkDcaAutomaticSupports.Checked
            Dim plan = New DcaPlan With {
                .Symbol = If(_cbDcaSymbol.SelectedItem, String.Empty).ToString(),
                .Exchange = If(_cbDcaExchange?.Text.Trim().ToUpperInvariant(), "BINANCE"),
                .BudgetQuote = _nudDcaBudget.Value,
                .CooldownMinutes = CInt(_nudDcaCooldown.Value),
                .Enabled = _chkDcaEnabled.Checked,
                .UseAutomaticSupports = automaticSupports,
                .CandleInterval = If(_cbDcaTimeframe.SelectedIndex = 1, "1d", "4h"),
                .SupportLookbackCandles = CInt(_nudDcaLookback.Value),
                .SupportTolerancePercent = _nudDcaTolerance.Value,
                .SupportMinimumTouches = CInt(_nudDcaMinimumTouches.Value),
                .SupportMaxLevels = CInt(_nudDcaMaximumLevels.Value),
                .RequireSupportRejection = _chkDcaRequireRejection.Checked,
                .Levels = If(automaticSupports, New List(Of DcaLevel)(), ParseDcaLevels(_txtDcaLevels.Text)),
                .ManualSupportOverrides = If(automaticSupports, ParseDcaSupportOverrides(_txtDcaLevels.Text), New List(Of DcaSupportOverride)())}
            _spotDca.SavePlan(plan)
            UpdateDcaGrid()
            Dim configuredLevels = If(automaticSupports,
                                     plan.ManualSupportOverrides.Count,
                                     plan.Levels.Count)
            SetDcaStatus($"Plano {plan.Symbol} salvo com {configuredLevels} ajuste(s)/nível(is).", Color.LimeGreen)
            If _spotDca.IsRunning Then
                Await _spotDca.LoadHistoricalCandlesAsync(AddressOf LoadDcaHistoricalCandlesAsync)
                Await StartBinanceWebSocket()
                Await StartGateWebSocket()
            End If
        Catch ex As Exception
            SetDcaStatus("Erro no plano DCA: " & ex.Message, Color.IndianRed)
        End Try
    End Sub

    Private Async Sub DcaStart_Click(sender As Object, e As EventArgs)
        Dim plans = _spotDca.GetPlans().Where(Function(item) item.Enabled).ToList()
        If plans.Count = 0 Then
            SetDcaStatus("Salve pelo menos um plano antes de iniciar.", Color.Gold)
            Return
        End If

        If plans.Any(Function(item) String.Equals(item.Exchange, "GATE.IO", StringComparison.OrdinalIgnoreCase)) AndAlso
           (String.IsNullOrWhiteSpace(ExchangeCredentials.GateAPIKey) OrElse
            String.IsNullOrWhiteSpace(ExchangeCredentials.GateSecretKey)) Then
            SetDcaStatus("Configure as credenciais da Gate.io antes de iniciar o DCA.", Color.IndianRed)
            Return
        End If

        If plans.Any(Function(item) Not String.Equals(item.Exchange, "GATE.IO", StringComparison.OrdinalIgnoreCase)) AndAlso
           (String.IsNullOrWhiteSpace(ExchangeCredentials.BinanceAPIKey) OrElse
            String.IsNullOrWhiteSpace(ExchangeCredentials.BinanceSecretKey)) Then
            SetDcaStatus("Configure as credenciais da Binance antes de iniciar o DCA.", Color.IndianRed)
            Return
        End If

        Try
            If MessageBox.Show(
                   "O DCA enviará ordens MARKET reais na exchange definida em cada plano." & Environment.NewLine &
                   "Deseja continuar?",
                   "Confirmar DCA real",
                   MessageBoxButtons.YesNo,
                   MessageBoxIcon.Warning) <> DialogResult.Yes Then
                Return
            End If

            SetDcaStatus("Carregando candles e recalculando suportes...", Color.Gold)
            Await _spotDca.LoadHistoricalCandlesAsync(AddressOf LoadDcaHistoricalCandlesAsync)
            _spotDca.Start()
            _btDcaStart.Enabled = False
            _btDcaStop.Enabled = True
            SetDcaStatus("DCA ativo; ordens MARKET reais e suportes automáticos ativos.", Color.LimeGreen)
            Await StartBinanceWebSocket()
            Await StartGateWebSocket()
        Catch ex As Exception
            _spotDca.Stop()
            SetDcaStatus("Erro iniciando DCA: " & ex.Message, Color.IndianRed)
        End Try
    End Sub

    Private Sub DcaStop_Click(sender As Object, e As EventArgs)
        _spotDca.Stop()
        _btDcaStart.Enabled = True
        _btDcaStop.Enabled = False
        SetDcaStatus("DCA parado; os estados foram preservados.", Color.Gold)
    End Sub

    Private Sub DcaReset_Click(sender As Object, e As EventArgs)
        If MessageBox.Show("Zerar quantidades, níveis acionados e capital registrado do DCA?", "DCA", MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then Return
        _spotDca.ResetSimulation()
        UpdateDcaGrid()
        SetDcaStatus("Estado do DCA zerado.", Color.Gold)
    End Sub

    Private Async Sub DcaGrid_CellContentClick(sender As Object, e As DataGridViewCellEventArgs)
        If e.RowIndex < 0 OrElse e.ColumnIndex < 0 OrElse
           Not String.Equals(_dgDca.Columns(e.ColumnIndex).Name, "DcaRemove", StringComparison.OrdinalIgnoreCase) Then
            Return
        End If

        Dim symbol = _dgDca.Rows(e.RowIndex).Cells("Symbol").Value?.ToString().Trim()
        If String.IsNullOrWhiteSpace(symbol) Then Return

        If MessageBox.Show(
                $"Excluir o plano DCA de {symbol}? Isso também removerá o estado salvo e as compras simuladas dessa moeda.",
                "DCA",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning) <> DialogResult.Yes Then
            Return
        End If

        Try
            _spotDca.RemovePlan(symbol)
            If _spotDca.GetPlans().Count = 0 Then
                _btDcaStart.Enabled = True
                _btDcaStop.Enabled = False
            End If
            UpdateDcaGrid()
            SetDcaStatus($"Plano DCA de {symbol} excluído.", Color.Gold)
            Await StartBinanceWebSocket()
        Catch ex As Exception
            SetDcaStatus("Erro excluindo plano DCA: " & ex.Message, Color.IndianRed)
        End Try
    End Sub

    Private Sub btDca_Click(sender As Object, e As EventArgs)
        PanelGraphs.Visible = False
        PanelOrdens.Visible = False
        PanelFuturos.Visible = False
        ArrangeOrdersPanel(False)
        ArrangeFuturesOrdersPanel(False)
        ArrangeSpotBotPanel(False)
        ArrangeDcaPanel(True)
        btVisaoGeral.BackColor = Color.FromArgb(20, 20, 20)
        btOrdensSpot.BackColor = Color.FromArgb(20, 20, 20)
        btOrdensFuturo.BackColor = Color.FromArgb(20, 20, 20)
        _tradeBotButton.BackColor = Color.FromArgb(20, 20, 20)
        _btDca.BackColor = Color.SteelBlue
        DcaExchange_SelectedIndexChanged(Nothing, EventArgs.Empty)
        UpdateDcaGrid()
    End Sub

    Private Function ParseDcaLevels(text As String) As List(Of DcaLevel)
        Dim result As New List(Of DcaLevel)()
        For Each part In If(text, String.Empty).Split(";"c, StringSplitOptions.RemoveEmptyEntries)
            Dim pieces = part.Trim().Split(":"c)
            If pieces.Length <> 2 Then Throw New FormatException("Use o formato queda:alocação; exemplo 5:10;10:20.")
            Dim drop As Decimal
            Dim allocation As Decimal
            If Not TryParseDcaDecimal(pieces(0), drop) OrElse
               Not TryParseDcaDecimal(pieces(1), allocation) OrElse
               drop <= 0D OrElse allocation <= 0D Then
                Throw New FormatException("Cada nível precisa ter percentuais positivos.")
            End If
            result.Add(New DcaLevel With {.DropPercent = drop, .AllocationPercent = allocation})
        Next
        If result.Count = 0 Then Throw New FormatException("Informe pelo menos um nível DCA.")
        If result.Sum(Function(item) item.AllocationPercent) > 100.01D Then Throw New FormatException("A soma das alocações não pode ultrapassar 100%.")
        Return result.OrderBy(Function(item) item.DropPercent).ToList()
    End Function

    Private Function ParseDcaSupportOverrides(text As String) As List(Of DcaSupportOverride)
        Dim result As New List(Of DcaSupportOverride)()
        For Each part In If(text, String.Empty).Split(";"c, StringSplitOptions.RemoveEmptyEntries)
            Dim pieces = part.Trim().Split(":"c)
            If pieces.Length < 2 OrElse pieces.Length > 3 Then
                Throw New FormatException("Use o formato preço:alocação; exemplo 0,9500:20;0,8800:30.")
            End If

            Dim price As Decimal
            Dim allocation As Decimal
            Dim tolerance = _nudDcaTolerance.Value
            If Not TryParseDcaDecimal(pieces(0), price) OrElse
               Not TryParseDcaDecimal(pieces(1), allocation) OrElse
               price <= 0D OrElse allocation <= 0D Then
                Throw New FormatException("Cada ajuste manual precisa ter preço e alocação positivos.")
            End If

            If pieces.Length = 3 AndAlso Not TryParseDcaDecimal(pieces(2), tolerance) Then
                Throw New FormatException("A tolerância manual deve ser um percentual positivo.")
            End If

            If tolerance <= 0D Then
                Throw New FormatException("A tolerância manual deve ser maior que zero.")
            End If

            result.Add(New DcaSupportOverride With {
                .Price = price,
                .AllocationPercent = allocation,
                .TolerancePercent = tolerance,
                .Enabled = True})
        Next

        If result.Sum(Function(item) item.AllocationPercent) > 100.01D Then
            Throw New FormatException("A soma dos ajustes manuais não pode ultrapassar 100%.")
        End If
        Return result
    End Function

    Private Shared Function TryParseDcaDecimal(value As String, ByRef result As Decimal) As Boolean
        If Decimal.TryParse(value?.Trim(), NumberStyles.Float, CultureInfo.CurrentCulture, result) Then Return True
        Return Decimal.TryParse(value?.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, result)
    End Function

    Private Sub DcaAutomaticSupports_CheckedChanged(sender As Object, e As EventArgs)
        If _chkDcaAutomaticSupports Is Nothing Then Return
        If _chkDcaAutomaticSupports.Checked Then
            SetDcaStatus("Modo automático: informe apenas ajustes manuais opcionais.", Color.Gold)
        Else
            SetDcaStatus("Modo percentual: os níveis serão interpretados como queda:alocação.", Color.Gold)
        End If
    End Sub

    Private Async Sub Dca_PurchaseSimulated(sender As Object, e As DcaPurchaseEventArgs)
        If e Is Nothing OrElse e.Purchase Is Nothing Then Return

        Dim purchase = e.Purchase
        Dim exchange = If(String.Equals(purchase.Exchange, "GATE.IO", StringComparison.OrdinalIgnoreCase), "GATE.IO", "BINANCE")
        Dim symbol = ToSpotUsdtSymbol(purchase.Symbol)
        Dim key = exchange & "|" & symbol

        SyncLock _dcaOrderInFlight
            If Not _dcaOrderInFlight.Add(key) Then Return
        End SyncLock

        Try
            SetDcaStatus($"Enviando DCA {exchange}: {purchase.Symbol} {purchase.QuoteAmount:0.00} USDT...", Color.Gold)
            Dim orderResponse As String
            If exchange = "GATE.IO" Then
                orderResponse = Await _gateSpot.GATE_CreateSpotOrderAsync(
                    symbol,
                    "BUY",
                    "MARKET",
                    purchase.QuoteAmount)
            Else
                orderResponse = Await B.BINANCE_CreateSpotMarketBuyByQuoteAsync(
                    symbol,
                    purchase.QuoteAmount)
            End If

            If String.IsNullOrWhiteSpace(orderResponse) Then
                Throw New InvalidOperationException("A exchange não retornou confirmação da ordem DCA.")
            End If

            SetDcaStatus(
                $"DCA executado {exchange}: {purchase.Symbol} {purchase.QuoteAmount:0.00} USDT em -{purchase.DropPercent:0.##}%.",
                Color.LimeGreen)

            Try
                Await refreshMarket()
            Catch ex As Exception
                Debug.WriteLine("DCA: ordem enviada, mas a atualização do portfólio falhou: " & ex.Message)
            End Try
        Catch ex As Exception
            _spotDca.RollbackPurchase(purchase)
            SetDcaStatus($"Erro na ordem DCA de {purchase.Symbol}: {ex.Message}", Color.IndianRed)
        Finally
            SyncLock _dcaOrderInFlight
                _dcaOrderInFlight.Remove(key)
            End SyncLock
        End Try
    End Sub

    Private Sub Dca_SimulationUpdated(sender As Object, e As DcaSimulationEventArgs)
        If IsDisposed OrElse Disposing Then Return
        If InvokeRequired Then
            BeginInvoke(New Action(AddressOf UpdateDcaGrid))
            Return
        End If
        UpdateDcaGrid()
    End Sub

    Private Sub SetDcaStatus(text As String, color As Color)
        If _lbDcaStatus Is Nothing Then Return
        If InvokeRequired Then
            BeginInvoke(New Action(Of String, Color)(AddressOf SetDcaStatus), text, color)
            Return
        End If
        _lbDcaStatus.Text = text
        _lbDcaStatus.ForeColor = color
    End Sub

    Private Sub UpdateDcaGrid()
        If _dgDca Is Nothing Then Return
        _dgDca.Rows.Clear()
        Dim states = _spotDca.GetStates().ToDictionary(Function(item) item.Symbol, StringComparer.OrdinalIgnoreCase)
        For Each plan In _spotDca.GetPlans()
            Dim state As DcaState = Nothing
            If Not states.TryGetValue(plan.Symbol, state) Then state = New DcaState With {.Symbol = plan.Symbol}
            Dim drop = If(state.AnchorPrice <= 0D, 0D, (state.AnchorPrice - state.LastPrice) / state.AnchorPrice * 100D)
            Dim index = _dgDca.Rows.Add()
            Dim row = _dgDca.Rows(index)
            row.Cells("Symbol").Value = plan.Symbol
            row.Cells("Exchange").Value = plan.Exchange
            row.Cells("Enabled").Value = If(plan.Enabled, "SIM", "NÃO")
            row.Cells("Anchor").Value = If(state.AnchorPrice <= 0D, "-", state.AnchorPrice.ToString("0.########", CultureInfo.InvariantCulture))
            row.Cells("LastPrice").Value = If(state.LastPrice <= 0D, "-", state.LastPrice.ToString("0.########", CultureInfo.InvariantCulture))
            row.Cells("Drop").Value = drop.ToString("0.00", CultureInfo.InvariantCulture) & "%"
            row.Cells("Quantity").Value = state.Quantity.ToString("0.########", CultureInfo.InvariantCulture)
            row.Cells("Invested").Value = FormatSpotBotUsd(state.InvestedQuote, 2)
            row.Cells("Average").Value = If(state.AveragePrice <= 0D, "-", state.AveragePrice.ToString("0.########", CultureInfo.InvariantCulture))
            row.Cells("Supports").Value = If(
                state.Supports Is Nothing OrElse state.Supports.Count = 0,
                "-",
                String.Join(" | ", state.Supports.
                    OrderByDescending(Function(item) item.Price).
                    Select(Function(item) item.Price.ToString("0.########", CultureInfo.InvariantCulture) &
                           " (" & item.AllocationPercent.ToString("0.#", CultureInfo.InvariantCulture) & "%)")))
            row.Cells("Pnl").Value = FormatSpotBotUsd(state.UnrealizedPnl, 2)
            row.Cells("Roi").Value = state.RoiPercent.ToString("0.00;-0.00;0.00", CultureInfo.InvariantCulture) & "%"
            Dim positive = state.UnrealizedPnl >= 0D
            Dim pnlColor = If(positive, Color.LimeGreen, Color.IndianRed)
            row.DefaultCellStyle.BackColor = If(positive, Color.FromArgb(0, 25, 0), Color.FromArgb(25, 0, 0))
            row.DefaultCellStyle.SelectionBackColor = row.DefaultCellStyle.BackColor
            row.DefaultCellStyle.SelectionForeColor = row.DefaultCellStyle.ForeColor
            row.Cells("Pnl").Style.ForeColor = pnlColor
            row.Cells("Roi").Style.ForeColor = pnlColor
            RemoveCellSelectionHighlight(row)
        Next
    End Sub

    Private Sub ArrangeDcaPanel(showDca As Boolean)
        If _panelDca Is Nothing OrElse PanelGraphs Is Nothing Then Return
        If Not showDca Then
            _panelDca.Visible = False
            Return
        End If

        Dim panelTop = Math.Max(PanelGraphs.Top, _btDca.Bottom + 2)
        Dim panelBottom = PanelProfits.Top
        _panelDca.Padding = New Padding(14, 0, 14, 0)
        _panelDca.Location = New Point(0, panelTop)
        _panelDca.Size = New Size(Me.ClientSize.Width, Math.Max(150, panelBottom - panelTop))
        _panelDca.Visible = True
        _panelDca.BringToFront()
        btVisaoGeral.BringToFront()
        btOrdensSpot.BringToFront()
        btOrdensFuturo.BringToFront()
        _tradeBotButton.BringToFront()
        _btDca.BringToFront()
    End Sub

    Private Sub ArrangeDcaButton()
        If _btDca Is Nothing OrElse _tradeBotButton Is Nothing Then Return
        _btDca.Location = New Point(_tradeBotButton.Right, _tradeBotButton.Top)
    End Sub

    Private Sub SpotBotUniverseModeChanged(sender As Object, e As EventArgs)
        UpdateSpotBotUniverseButtonText()
    End Sub

    Private Sub UpdateSpotBotUniverseButtonText()
        If _botUniverseButton Is Nothing OrElse _botUniverseMode Is Nothing Then Return
        _botUniverseButton.Text = If(_botUniverseMode.SelectedIndex = 0, "Selecionar...", "Atualizar")
    End Sub

    Private Async Sub SpotBotUniverseButton_Click(sender As Object, e As EventArgs)
        If _botUniverseMode Is Nothing Then Return

        If _botUniverseMode.SelectedIndex = 0 Then
            Dim candidates = GetManualSpotBotUniverseCandidates()
            If candidates.Count = 0 Then
                MessageBox.Show(
                    "Cadastre moedas em Cadastro > Símbolos antes de montar o universo manual.",
                    "Universo do Tradebot",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information)
                Return
            End If

            Dim selected = PortfolioRepository.GetSpotBotManualUniverseSymbols()
            If selected.Count = 0 Then selected = candidates.ToList()

            Using dialog As New SpotTradeBotUniverseForm(candidates, selected)
                If dialog.ShowDialog(Me) <> DialogResult.OK Then Return
                PortfolioRepository.SaveSpotBotManualUniverseSymbols(dialog.SelectedSymbols)
                _spotBotUniverseSymbols.Clear()
                _spotBotUniverseSymbols.AddRange(dialog.SelectedSymbols)
                _spotTradeBot.Configuration.ManualUniverseSymbols.Clear()
                For Each symbol In dialog.SelectedSymbols
                    _spotTradeBot.Configuration.ManualUniverseSymbols.Add(symbol)
                Next
                AppendSpotBotLog($"Universo manual salvo: {dialog.SelectedSymbols.Count} ativo(s).", Color.LightGray)
            End Using
            Return
        End If

        Try
            Dim symbols = Await B.BINANCE_GetTopSpotSymbolsByVolumeAsync(
                CInt(_botUniverseMaxAssets.Value),
                _botUniverseMinVolume.Value,
                CInt(_botUniverseMinTrades.Value))
            _spotBotUniverseSymbols.Clear()
            _spotBotUniverseSymbols.AddRange(symbols)
            AppendSpotBotLog(
                $"Universo Top volume/liquidez atualizado: {symbols.Count} ativo(s).",
                Color.LightGray)
            If _spotTradeBot.IsRunning Then Await StartBinanceWebSocket()
        Catch ex As Exception
            AppendSpotBotLog("Erro carregando o universo Top volume: " & ex.Message, Color.IndianRed)
        End Try
    End Sub

    Private Function GetManualSpotBotUniverseCandidates() As List(Of String)
        Dim symbols As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)

        Try
            Dim table = PortfolioRepository.GetCryptoSymbols()
            For Each databaseRow As DataRow In table.Rows
                AddSpotBotUniverseSymbol(symbols, databaseRow("Symbol")?.ToString())
            Next
        Catch ex As Exception
            Debug.WriteLine("Erro carregando moedas manuais do Tradebot: " & ex.Message)
        End Try

        If dgPortfolio IsNot Nothing AndAlso dgPortfolio.Columns.Contains("Cripto") Then
            For Each row As DataGridViewRow In dgPortfolio.Rows
                If row.IsNewRow Then Continue For
                AddSpotBotUniverseSymbol(symbols, row.Cells("Cripto").Value?.ToString())
            Next
        End If

        Return symbols.OrderBy(Function(item) item, StringComparer.OrdinalIgnoreCase).ToList()
    End Function

    Private Sub AddSpotBotUniverseSymbol(symbols As HashSet(Of String), rawSymbol As String)
        Dim symbol = GetSpotBaseAsset(rawSymbol)
        If String.IsNullOrWhiteSpace(symbol) OrElse Cjson.stablecoins.Contains(symbol) Then Return
        symbols.Add(symbol)
    End Sub

    Private Async Function ResolveSpotBotUniverseAsync() As Task(Of List(Of String))
        Dim mode = If(_botUniverseMode IsNot Nothing AndAlso _botUniverseMode.SelectedIndex = 1,
                      SpotBotUniverseMode.TopVolume,
                      SpotBotUniverseMode.Manual)
        Dim maxAssets = If(_botUniverseMaxAssets Is Nothing, 20, CInt(_botUniverseMaxAssets.Value))
        Dim minimumVolume = If(_botUniverseMinVolume Is Nothing, 1000000D, _botUniverseMinVolume.Value)
        Dim minimumTrades = If(_botUniverseMinTrades Is Nothing, 1000, CInt(_botUniverseMinTrades.Value))
        Dim symbols As New List(Of String)()

        If mode = SpotBotUniverseMode.TopVolume Then
            symbols = Await B.BINANCE_GetTopSpotSymbolsByVolumeAsync(maxAssets, minimumVolume, minimumTrades)
        Else
            symbols = PortfolioRepository.GetSpotBotManualUniverseSymbols()
            If symbols.Count = 0 Then symbols = GetManualSpotBotUniverseCandidates()
            If symbols.Count > 0 Then
                symbols = Await B.BINANCE_GetTopSpotSymbolsByVolumeAsync(
                    Math.Max(maxAssets, symbols.Count),
                    minimumVolume,
                    minimumTrades,
                    symbols)
            End If
        End If

        symbols = symbols.
            Select(Function(item) GetSpotBaseAsset(item)).
            Where(Function(item) Not String.IsNullOrWhiteSpace(item) AndAlso Not Cjson.stablecoins.Contains(item)).
            Distinct(StringComparer.OrdinalIgnoreCase).
            Take(Math.Max(1, maxAssets)).
            ToList()

        PortfolioRepository.SaveSpotBotUniverseSettings(New SpotBotUniverseSettings With {
            .Mode = mode,
            .MaxAssets = maxAssets,
            .MinimumQuoteVolume = minimumVolume,
            .MinimumTrades24h = minimumTrades
        })

        _spotTradeBot.Configuration.UniverseMode = mode
        _spotTradeBot.Configuration.UniverseMaxAssets = maxAssets
        _spotTradeBot.Configuration.UniverseMinimumQuoteVolume = minimumVolume
        _spotTradeBot.Configuration.UniverseMinimumTrades24h = minimumTrades
        _spotTradeBot.Configuration.ManualUniverseSymbols.Clear()
        For Each symbol In symbols
            _spotTradeBot.Configuration.ManualUniverseSymbols.Add(symbol)
        Next

        _spotBotUniverseSymbols.Clear()
        _spotBotUniverseSymbols.AddRange(symbols)
        Return symbols
    End Function

    Private Sub AddSpotBotPositionColumn(name As String, headerText As String)
        _dgSpotBotPositions.Columns.Add(New DataGridViewTextBoxColumn With {
            .Name = name,
            .HeaderText = headerText,
            .SortMode = DataGridViewColumnSortMode.NotSortable,
            .AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
        })
    End Sub

    Private Sub SpotBotPositions_CellClick(sender As Object, e As DataGridViewCellEventArgs)
        If _dgSpotBotPositions Is Nothing OrElse
           e.RowIndex < 0 OrElse
           e.ColumnIndex < 0 OrElse
           Not String.Equals(_dgSpotBotPositions.Columns(e.ColumnIndex).Name, "Symbol", StringComparison.OrdinalIgnoreCase) Then
            Return
        End If

        Dim symbol = _dgSpotBotPositions.Rows(e.RowIndex).Cells("Symbol").Value?.ToString().Trim().ToUpperInvariant()
        If String.IsNullOrWhiteSpace(symbol) Then Return

        Try
            Dim chartForm As New FormBrowser(symbol)
            chartForm.Show(Me)
        Catch ex As Exception
            MessageBox.Show(
                "Não foi possível abrir o gráfico de " & symbol & ": " & ex.Message,
                "Gráfico do TradeBot",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub SpotBotPositions_CellContentClick(sender As Object, e As DataGridViewCellEventArgs)
        If _dgSpotBotPositions Is Nothing OrElse
           e.RowIndex < 0 OrElse
           e.ColumnIndex < 0 OrElse
           Not String.Equals(_dgSpotBotPositions.Columns(e.ColumnIndex).Name, "ClosePosition", StringComparison.OrdinalIgnoreCase) Then
            Return
        End If

        Dim symbol = _dgSpotBotPositions.Rows(e.RowIndex).Cells("Symbol").Value?.ToString().Trim().ToUpperInvariant()
        If String.IsNullOrWhiteSpace(symbol) Then Return

        Dim position = _spotTradeBot.GetOpenPositions().FirstOrDefault(
            Function(item) String.Equals(item.Symbol, symbol, StringComparison.OrdinalIgnoreCase))
        If position Is Nothing Then Return

        Try
            If _spotTradeBot.ClosePositionManually(symbol) Then
                AppendSpotBotLog(
                    $"FECHAMENTO MANUAL {symbol}: posição encerrada a {FormatSpotBotUsd(position.CurrentPrice, 4)}.",
                    Color.Gold)
            End If
        Catch ex As Exception
            MessageBox.Show(
                "Erro ao fechar o trade: " & ex.Message,
                "Fechar trade",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub SpotBotPositions_CellMouseEnter(sender As Object, e As DataGridViewCellEventArgs)
        If _dgSpotBotPositions Is Nothing OrElse e.ColumnIndex < 0 Then Return
        _dgSpotBotPositions.Cursor = If(
            String.Equals(_dgSpotBotPositions.Columns(e.ColumnIndex).Name, "ClosePosition", StringComparison.OrdinalIgnoreCase) OrElse
            String.Equals(_dgSpotBotPositions.Columns(e.ColumnIndex).Name, "Symbol", StringComparison.OrdinalIgnoreCase),
            Cursors.Hand,
            Cursors.Default)
    End Sub

    Private Sub SpotBotPositions_CellMouseLeave(sender As Object, e As DataGridViewCellEventArgs)
        If _dgSpotBotPositions IsNot Nothing Then
            _dgSpotBotPositions.Cursor = Cursors.Default
        End If
    End Sub

    Private Sub AddBotStrategyCheckBox(
        editor As Control,
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

    Private Function GetBollingerModeIndex() As Integer
        If Not _spotTradeBot.HasRestoredConfiguration OrElse
           Not _spotTradeBot.Configuration.EnabledStrategies.Contains(SpotBotStrategyKind.BaseOriginalProtected) Then
            Return 1
        End If

        Return 0
    End Function

    Private Function GetBollingerTimeframeIndex() As Integer
        Return If(_spotTradeBot.HasRestoredConfiguration AndAlso _spotTradeBot.Configuration.BollingerCandleMinutes = 240, 0, 1)
    End Function

    Private Sub SpotBotBollingerModeChanged(sender As Object, e As EventArgs)
        UpdateSpotBotBollingerTimeframeAvailability()
    End Sub

    Private Sub UpdateSpotBotBollingerTimeframeAvailability()
        If _botBollingerMode Is Nothing OrElse _botBollingerTimeframe Is Nothing Then
            Return
        End If

        _botBollingerTimeframe.Enabled = _botBollingerMode.SelectedIndex = 0
    End Sub

    Private Sub ApplyBollingerTimeframe()
        If _botBollingerTimeframe Is Nothing OrElse _botBollingerTimeframe.SelectedIndex = 0 Then
            _spotTradeBot.Configuration.BollingerCandleMinutes = 240
        Else
            _spotTradeBot.Configuration.BollingerCandleMinutes = 1440
        End If
    End Sub

    Private Sub ApplyBollingerMode()
        ' A Base Original Protegida é o perfil principal, mas pode ser
        ' desativada pelo combo quando o usuário quiser testar apenas as
        ' estratégias adicionais.
        _spotTradeBot.SetStrategyEnabled(SpotBotStrategyKind.BnfContrarian, False)
        _spotTradeBot.SetStrategyEnabled(SpotBotStrategyKind.BollingerMeanReversion, False)
        _spotTradeBot.SetStrategyEnabled(
            SpotBotStrategyKind.BaseOriginalProtected,
            _botBollingerMode Is Nothing OrElse _botBollingerMode.SelectedIndex = 0)
        _spotTradeBot.Configuration.BollingerPeriod = 25
        _spotTradeBot.Configuration.BollingerStandardDeviations = 3D
    End Sub

    Private Sub ConfigureSpotBotManualReferences(universeSettings As SpotBotUniverseSettings)
        _botBollingerMode = cbSpotBotBollingerMode
        _botCombination = cbSpotBotCombination
        _botBollingerTimeframe = cbSpotBotBollingerTimeframe
        _botConfirmations = nudSpotBotConfirmations
        _botUniverseMode = cbSpotBotUniverseMode
        _botUniverseButton = btSpotBotUniverse
        _botStartingQuote = nudSpotBotStartingQuote
        _botAllocation = nudSpotBotAllocation
        _botFee = nudSpotBotFee
        _botTakeProfit = nudSpotBotTakeProfit
        _botStopLoss = nudSpotBotStopLoss
        _botUniverseMaxAssets = nudSpotBotMaxAssets
        _botUniverseMinVolume = nudSpotBotMinVolume
        _botBreakEvenEnabled = chkSpotBotBreakEven
        _botBreakEvenTrigger = NumericUpDown2
        _botTrailingEnabled = chkSpotBotTrailing
        _botTrailingDistance = NumericUpDown1
        _botPostExitCooldown = nudSpotBotPostExitCooldown
        _botUniverseMinTrades = nudSpotBotMinTrades
        _botGridRiskPercent = nudSpotBotGridRisk
        _botGridReservePercent = nudSpotBotGridReserve
        _botGridMaxExposurePercent = nudSpotBotGridExposure
        _botGridMaxLevels = nudSpotBotGridLevels
        _botGridRequireRejection = chkSpotBotGridRejection
        _botBtcFlushDropPercent = nudSpotBotBtcFlushDrop
        _botBtcFlushPauseMinutes = nudSpotBotBtcFlushPause
        _botBtcFlushEnabled = chkSpotBotBtcFlush
        _botBtcFlushVolumeMultiplier = nudSpotBotBtcFlushVolume
        _botBtcDailyDropPercent = nudSpotBotBtcDailyDrop
        _botBtcDailyDrawdownPercent = nudSpotBotBtcDailyDrawdown
        _botBtcDailyEnabled = chkSpotBotBtcDaily
        _botRiskBrakeEnabled = chkSpotBotRiskBrakeEnabled
        _botRiskSessionLoss = nudSpotBotRiskSessionLoss
        _botRiskProfitLockEnabled = chkSpotBotRiskProfitLock
        _botRiskProfitTrigger = nudSpotBotRiskProfitTrigger
        _botRiskGiveback = nudSpotBotRiskGiveback
        _botRiskMaxLosses = nudSpotBotRiskMaxLosses
        _botRiskPauseMinutes = nudSpotBotRiskPause
        _botRiskClosePositions = chkSpotBotRiskClosePositions
        _botRiskUseUnrealized = chkSpotBotRiskUseUnrealized
        _botProfitGateEnabled = chkSpotBotProfitGateEnabled
        _botMinProfitBeforeEntry = nudSpotBotMinProfitBeforeEntry
        ToolTip1.SetToolTip(
            _botProfitGateEnabled,
            "Quando ativo, uma nova posição em outro ativo só é aceita se as posições abertas estiverem acima do lucro líquido mínimo.")
        ToolTip1.SetToolTip(
            _botMinProfitBeforeEntry,
            "Lucro líquido estimado mínimo de cada posição aberta, já descontando taxa de saída e slippage.")
        _botStart = btSpotBotStart
        _botStop = btSpotBotStop
        _botExport = btSpotBotExport
        _botTrades = btSpotBotTrades
        _botSimulationSummary = lbSpotBotSimulationSummary
        _botLog = rtbSpotBotLog
        _dgSpotBotPositions = dgSpotBotPositionsControl

        _botStrategyChecks.Clear()
        _botStrategyChecks(SpotBotStrategyKind.WavePullback) = chkSpotBotWavePullback
        _botStrategyChecks(SpotBotStrategyKind.BreakoutRetest) = chkSpotBotBreakoutRetest
        _botStrategyChecks(SpotBotStrategyKind.MomentumVolume) = chkSpotBotMomentumVolume
        _botStrategyChecks(SpotBotStrategyKind.GridAdaptive) = chkSpotBotGridAdaptive
        _botStrategyChecks(SpotBotStrategyKind.ValueZonePullback) = chkSpotBotValueZonePullback

        If _botBollingerMode.Items.Count = 0 Then
            _botBollingerMode.Items.AddRange(New Object() {"Base Original Protegida", "Desativada"})
        End If
        If _botCombination.Items.Count = 0 Then
            _botCombination.Items.AddRange(New Object() {"Qualquer estratégia", "Todas as estratégias", "Mínimo de confirmações", "Pontuação ponderada"})
        End If
        If _botBollingerTimeframe.Items.Count = 0 Then
            _botBollingerTimeframe.Items.AddRange(New Object() {"4 horas", "Diário"})
        End If
        If _botUniverseMode.Items.Count = 0 Then
            _botUniverseMode.Items.AddRange(New Object() {"Manual", "Top volume"})
        End If

        _botBollingerMode.DropDownStyle = ComboBoxStyle.DropDownList
        _botCombination.DropDownStyle = ComboBoxStyle.DropDownList
        _botBollingerTimeframe.DropDownStyle = ComboBoxStyle.DropDownList
        _botUniverseMode.DropDownStyle = ComboBoxStyle.DropDownList
        _botBollingerMode.SelectedIndex = Math.Max(0, Math.Min(_botBollingerMode.Items.Count - 1, GetBollingerModeIndex()))
        _botCombination.SelectedIndex = Math.Max(0, Math.Min(_botCombination.Items.Count - 1,
            If(_spotTradeBot.HasRestoredConfiguration,
               CInt(_spotTradeBot.Configuration.CombinationMode),
               CInt(SpotBotCombinationMode.MinimumConfirmations))))
        _botConfirmations.Value = Math.Min(
            _botConfirmations.Maximum,
            Math.Max(_botConfirmations.Minimum,
                     If(_spotTradeBot.HasRestoredConfiguration, _spotTradeBot.Configuration.RequiredConfirmations, 2D)))
        _botBollingerTimeframe.SelectedIndex = Math.Max(0, Math.Min(_botBollingerTimeframe.Items.Count - 1, GetBollingerTimeframeIndex()))
        _botUniverseMode.SelectedIndex = If(universeSettings.Mode = SpotBotUniverseMode.TopVolume, 1, 0)
        _botUniverseMaxAssets.Value = ClampNumeric(_botUniverseMaxAssets, universeSettings.MaxAssets)
        _botUniverseMinVolume.Value = ClampNumeric(_botUniverseMinVolume, universeSettings.MinimumQuoteVolume)
        _botUniverseMinTrades.Value = ClampNumeric(_botUniverseMinTrades, universeSettings.MinimumTrades24h)

        If _spotTradeBot.HasRestoredConfiguration Then
            Dim restoredSnapshot = _spotTradeBot.GetSimulationSnapshot()
            _botStartingQuote.Value = ClampNumeric(_botStartingQuote, restoredSnapshot.StartingQuote)
            _botAllocation.Value = ClampNumeric(_botAllocation, _spotTradeBot.Configuration.SimulationAllocationPercent)
            _botFee.Value = ClampNumeric(_botFee, _spotTradeBot.Configuration.SimulationFeeRate * 100D)
            _botTakeProfit.Value = ClampNumeric(_botTakeProfit, _spotTradeBot.Configuration.TakeProfitPercent)
            _botStopLoss.Value = ClampNumeric(_botStopLoss, _spotTradeBot.Configuration.StopLossPercent)
            _botBreakEvenEnabled.Checked = _spotTradeBot.Configuration.BreakEvenEnabled
            _botBreakEvenTrigger.Value = ClampNumeric(_botBreakEvenTrigger, _spotTradeBot.Configuration.BreakEvenTriggerPercent)
            _botTrailingEnabled.Checked = _spotTradeBot.Configuration.TrailingStopEnabled
            _botTrailingDistance.Value = ClampNumeric(_botTrailingDistance, _spotTradeBot.Configuration.TrailingStopDistancePercent)
            _botPostExitCooldown.Value = ClampNumeric(_botPostExitCooldown, _spotTradeBot.Configuration.PostExitCooldownMinutes)
            _botGridRiskPercent.Value = ClampNumeric(_botGridRiskPercent, _spotTradeBot.Configuration.GridV2RiskPerTradePercent)
            _botGridReservePercent.Value = ClampNumeric(_botGridReservePercent, _spotTradeBot.Configuration.GridV2ReserveQuotePercent)
            _botGridMaxExposurePercent.Value = ClampNumeric(_botGridMaxExposurePercent, _spotTradeBot.Configuration.GridV2MaxExposurePercent)
            _botGridMaxLevels.Value = ClampNumeric(_botGridMaxLevels, _spotTradeBot.Configuration.GridV2MaxLevels)
            _botGridRequireRejection.Checked = _spotTradeBot.Configuration.GridV2RequireRejection
            _botBtcFlushDropPercent.Value = ClampNumeric(_botBtcFlushDropPercent, _spotTradeBot.Configuration.BitcoinFlushDropPercent)
            _botBtcFlushPauseMinutes.Value = ClampNumeric(_botBtcFlushPauseMinutes, _spotTradeBot.Configuration.BitcoinFlushPauseMinutes)
            _botBtcFlushEnabled.Checked = _spotTradeBot.Configuration.BitcoinFlushProtectionEnabled
            _botBtcFlushVolumeMultiplier.Value = ClampNumeric(_botBtcFlushVolumeMultiplier, _spotTradeBot.Configuration.BitcoinFlushVolumeMultiplier)
            _botBtcDailyDropPercent.Value = ClampNumeric(_botBtcDailyDropPercent, _spotTradeBot.Configuration.BitcoinDailyDropPercent)
            _botBtcDailyDrawdownPercent.Value = ClampNumeric(_botBtcDailyDrawdownPercent, _spotTradeBot.Configuration.BitcoinDailyDrawdownPercent)
            _botBtcDailyEnabled.Checked = _spotTradeBot.Configuration.BitcoinDailyProtectionEnabled
            _botRiskBrakeEnabled.Checked = _spotTradeBot.Configuration.RiskBrakeEnabled
            _botRiskSessionLoss.Value = ClampNumeric(_botRiskSessionLoss, _spotTradeBot.Configuration.RiskBrakeSessionLossPercent)
            _botRiskProfitLockEnabled.Checked = _spotTradeBot.Configuration.RiskBrakeProfitLockEnabled
            _botRiskProfitTrigger.Value = ClampNumeric(_botRiskProfitTrigger, _spotTradeBot.Configuration.RiskBrakeProfitLockTriggerPercent)
            _botRiskGiveback.Value = ClampNumeric(_botRiskGiveback, _spotTradeBot.Configuration.RiskBrakeProfitGivebackPercent)
            _botRiskMaxLosses.Value = ClampNumeric(_botRiskMaxLosses, _spotTradeBot.Configuration.RiskBrakeMaxConsecutiveLosses)
            _botRiskPauseMinutes.Value = ClampNumeric(_botRiskPauseMinutes, _spotTradeBot.Configuration.RiskBrakePauseMinutes)
            _botRiskClosePositions.Checked = _spotTradeBot.Configuration.RiskBrakeClosePositionsOnHardStop
            _botRiskUseUnrealized.Checked = _spotTradeBot.Configuration.RiskBrakeUseUnrealizedPnl
            _botProfitGateEnabled.Checked = _spotTradeBot.Configuration.NewPositionProfitGateEnabled
            _botMinProfitBeforeEntry.Value = ClampNumeric(_botMinProfitBeforeEntry, _spotTradeBot.Configuration.MinimumNetProfitBeforeNewPositionPercent)
            For Each strategyCheck In _botStrategyChecks
                strategyCheck.Value.Checked =
                    _spotTradeBot.Configuration.EnabledStrategies.Contains(strategyCheck.Key) OrElse
                    (strategyCheck.Key = SpotBotStrategyKind.GridAdaptive AndAlso
                     _spotTradeBot.Configuration.EnabledStrategies.Contains(SpotBotStrategyKind.GridRegime))
            Next
        End If

        AddHandler _botBollingerMode.SelectedIndexChanged, AddressOf SpotBotBollingerModeChanged
        AddHandler _botUniverseMode.SelectedIndexChanged, AddressOf SpotBotUniverseModeChanged
        AddHandler _botUniverseButton.Click, AddressOf SpotBotUniverseButton_Click
        AddHandler _botStart.Click, AddressOf SpotBotStart_Click
        AddHandler _botStop.Click, AddressOf SpotBotStop_Click
        AddHandler _botExport.Click, AddressOf SpotBotExport_Click
        AddHandler _botTrades.Click, AddressOf SpotBotTrades_Click
        UpdateSpotBotBollingerTimeframeAvailability()
    End Sub

    Private Shared Function ClampNumeric(control As NumericUpDown, value As Decimal) As Decimal
        If control Is Nothing Then Return value
        Return Math.Min(control.Maximum, Math.Max(control.Minimum, value))
    End Function

    Private Sub ConfigureSpotBotManualActions()
        _tradeBotButton = btTradebot
        _tradeBotButton.Cursor = Cursors.Hand
        _botStart.Enabled = Not _spotTradeBot.IsRunning
        _botStop.Enabled = _spotTradeBot.IsRunning
        _botExport.Enabled = True
        _botTrades.Enabled = True
        _botUniverseButton.Text = If(_botUniverseMode.SelectedIndex = 0, "Selecionar...", "Atualizar")
        _botSimulationSummary.Text = "Simulação: aguardando início"
        _botLog.ReadOnly = True
        _botLog.DetectUrls = False
        _botLog.ScrollBars = RichTextBoxScrollBars.Vertical
        _botLog.Visible = True
        _dgSpotBotPositions.Visible = True
    End Sub

    Private Sub UpdateSpotBotRunButtons(snapshot As SpotBotSimulationSnapshot)
        If _botStart Is Nothing OrElse _botStop Is Nothing Then Return

        Dim handbrakePaused = snapshot IsNot Nothing AndAlso
                              snapshot.RiskBrakeState = SpotBotRiskBrakeState.HardStop AndAlso
                              Not _spotTradeBot.IsRunning
        If handbrakePaused Then
            _botStart.Enabled = True
            _botStop.Enabled = False
        Else
            _botStart.Enabled = Not _spotTradeBot.IsRunning
            _botStop.Enabled = _spotTradeBot.IsRunning
        End If
    End Sub

    Private Sub ConfigureSpotBotManualPositionsGrid()
        If _dgSpotBotPositions Is Nothing Then Return

        RemoveHandler _dgSpotBotPositions.CellClick, AddressOf SpotBotPositions_CellClick
        RemoveHandler _dgSpotBotPositions.CellContentClick, AddressOf SpotBotPositions_CellContentClick
        RemoveHandler _dgSpotBotPositions.CellMouseEnter, AddressOf SpotBotPositions_CellMouseEnter
        RemoveHandler _dgSpotBotPositions.CellMouseLeave, AddressOf SpotBotPositions_CellMouseLeave
        _dgSpotBotPositions.Columns.Clear()
        _dgSpotBotPositions.AutoGenerateColumns = False
        _dgSpotBotPositions.Visible = True

        AddSpotBotPositionColumn("Symbol", "Cripto")
        AddSpotBotPositionColumn("Strategy", "Estratégia")
        AddSpotBotPositionColumn("Side", "Lado")
        AddSpotBotPositionColumn("Quantity", "Qtd")
        AddSpotBotPositionColumn("Notional", "Notional")
        AddSpotBotPositionColumn("EntryPrice", "Entrada")
        AddSpotBotPositionColumn("CurrentPrice", "Preço atual")
        AddSpotBotPositionColumn("StopPrice", "SL")
        AddSpotBotPositionColumn("UnrealizedPnl", "PnL")
        AddSpotBotPositionColumn("RoiPercent", "ROI %")
        AddSpotBotPositionColumn("GridLevelCount", "Níveis")

        Dim closeColumn As New DataGridViewButtonColumn With {
            .Name = "ClosePosition",
            .HeaderText = "Ação",
            .Text = "Fechar",
            .UseColumnTextForButtonValue = True,
            .SortMode = DataGridViewColumnSortMode.NotSortable,
            .AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
            .Width = 72,
            .FlatStyle = FlatStyle.Flat
        }
        closeColumn.DefaultCellStyle = New DataGridViewCellStyle With {
            .Alignment = DataGridViewContentAlignment.MiddleCenter,
            .BackColor = Color.IndianRed,
            .ForeColor = Color.White,
            .SelectionBackColor = Color.IndianRed,
            .SelectionForeColor = Color.White
        }
        _dgSpotBotPositions.Columns.Add(closeColumn)
        AddHandler _dgSpotBotPositions.CellClick, AddressOf SpotBotPositions_CellClick
        AddHandler _dgSpotBotPositions.CellContentClick, AddressOf SpotBotPositions_CellContentClick
        AddHandler _dgSpotBotPositions.CellMouseEnter, AddressOf SpotBotPositions_CellMouseEnter
        AddHandler _dgSpotBotPositions.CellMouseLeave, AddressOf SpotBotPositions_CellMouseLeave
        ConfigureSpotBotToolTips()
    End Sub

    Private Sub ConfigureSpotBotToolTips()
        If ToolTip1 Is Nothing Then Return

        ToolTip1.AutoPopDelay = 12000
        ToolTip1.InitialDelay = 350
        ToolTip1.ReshowDelay = 150
        ToolTip1.ShowAlways = True

        SetSpotBotToolTip(Label13, "Define como os sinais das estratégias ativas serão combinados para gerar uma entrada.")
        SetSpotBotToolTip(Label14, "Quantidade mínima de estratégias que precisam confirmar o mesmo sinal.")
        SetSpotBotToolTip(Label24, "Timeframe usado para formar os candles da estratégia Bollinger.")
        SetSpotBotToolTip(Label21, "Fonte do universo de ativos: seleção manual ou ativos com maior volume.")
        SetSpotBotToolTip(cbSpotBotBollingerMode, "Ativa ou desativa a Base Original Protegida, baseada na Bollinger 25/3.")
        SetSpotBotToolTip(cbSpotBotCombination, "Regra usada para combinar os sinais das estratégias habilitadas.")
        SetSpotBotToolTip(cbSpotBotBollingerTimeframe, "Período dos candles usados pela estratégia Bollinger.")
        SetSpotBotToolTip(cbSpotBotUniverseMode, "Escolhe entre ativos definidos manualmente ou os ativos de maior volume.")
        SetSpotBotToolTip(btSpotBotUniverse, "Abre a seleção de ativos do universo manual.")
        SetSpotBotToolTip(chkSpotBotWavePullback, "Pullback EMA34: procura retorno à EMA34 e confirmação da retomada antes da entrada.")
        SetSpotBotToolTip(chkSpotBotBreakoutRetest, "Breakout + Reteste: aguarda rompimento e reteste confirmado da região rompida.")
        SetSpotBotToolTip(chkSpotBotMomentumVolume, "Momentum por volume: procura expansão de volume acompanhando um movimento direcional.")
        SetSpotBotToolTip(chkSpotBotGridAdaptive, "Grid adaptativo V2: monta níveis individualmente em lateralização e usa proteção automática de regime.")
        SetSpotBotToolTip(chkSpotBotValueZonePullback, "Pullback zona de valor: combina tendência M5, retorno M2 e confirmação de retomada no M1.")

        SetSpotBotToolTip(Label17, "Saldo virtual inicial usado na simulação do tradebot.")
        SetSpotBotToolTip(Label18, "Percentual do saldo destinado a cada entrada normal.")
        SetSpotBotToolTip(Label33, "Taxa aplicada em cada execução simulada. O custo de ida e volta é calculado internamente.")
        SetSpotBotToolTip(Label34, "Alvo percentual padrão para estratégias que não definem um alvo estrutural.")
        SetSpotBotToolTip(Label35, "Perda percentual padrão para estratégias que não definem um stop estrutural.")
        SetSpotBotToolTip(Label23, "Número máximo de ativos que podem participar do universo.")
        SetSpotBotToolTip(Label22, "Volume mínimo negociado em 24 horas para aceitar um ativo.")
        SetSpotBotToolTip(Label19, "Tempo de espera após uma saída antes de permitir nova entrada no mesmo ativo.")
        SetSpotBotToolTip(Label20, "Quantidade mínima de negócios em 24 horas para filtrar ativos pouco líquidos.")
        SetSpotBotToolTip(Label36, "Risco máximo estimado para cada nível do Grid V2 em relação ao saldo inicial.")
        SetSpotBotToolTip(Label37, "Percentual do saldo reservado e que não pode ser consumido pelo Grid V2.")
        SetSpotBotToolTip(Label38, "Exposição máxima acumulada por ativo dentro do Grid V2.")
        SetSpotBotToolTip(Label39, "Quantidade máxima de níveis/lotes que o Grid V2 pode acumular por ativo.")
        SetSpotBotToolTip(chkSpotBotGridRejection, "Exige toque no nível e confirmação de rejeição antes de abrir um novo lote do Grid V2.")

        SetSpotBotToolTip(chkSpotBotBreakEven, "Move o stop para entrada mais custos quando o lucro mínimo for atingido.")
        SetSpotBotToolTip(NumericUpDown2, "Gatilho mínimo do Break Even. O motor também respeita taxas, slippage e pequeno ganho.")
        SetSpotBotToolTip(chkSpotBotTrailing, "Depois da proteção inicial, acompanha o maior preço alcançado e sobe o stop.")
        SetSpotBotToolTip(NumericUpDown1, "Distância percentual entre o maior preço alcançado e o stop móvel.")
        SetSpotBotToolTip(Label12, "Percentual relacionado ao gatilho do Break Even.")
        SetSpotBotToolTip(Label25, "Percentual de distância usado pelo trailing stop.")

        SetSpotBotToolTip(chkSpotBotBtcFlush, "Pausa ou protege novas entradas quando o BTC sofre uma queda rápida com volume anormal.")
        SetSpotBotToolTip(Label26, "Queda acumulada do BTC usada para detectar um flush de curto prazo.")
        SetSpotBotToolTip(Label27, "Tempo de pausa das novas entradas após a confirmação do flush do BTC.")
        SetSpotBotToolTip(Label32, "Multiplicador de volume exigido para confirmar que a queda do BTC é anormal.")
        SetSpotBotToolTip(Label31, "Unidade do multiplicador de volume usado na proteção de flush.")
        SetSpotBotToolTip(chkSpotBotBtcDaily, "Usa a queda e a oscilação diária do BTC para bloquear entradas em uma deterioração forte.")
        SetSpotBotToolTip(Label28, "Queda diária do BTC que ativa a proteção diária.")
        SetSpotBotToolTip(Label29, "Oscilação máxima diária usada como limite de proteção do BTC.")
        SetSpotBotToolTip(Label30, "Unidade percentual do limite de oscilação diária.")

        SetSpotBotToolTip(GroupBox4, "Freio de perdas/circuit breaker: limita perdas da sessão, sequência de stops e devolução de lucro.")
        SetSpotBotToolTip(Label40, "Percentual máximo de perda do patrimônio desde o início da sessão.")
        SetSpotBotToolTip(Label41, "Ganho necessário para armar a trava de lucro.")
        SetSpotBotToolTip(Label42, "Percentual do pico de lucro que pode ser devolvido antes de acionar o freio.")
        SetSpotBotToolTip(Label43, "Quantidade de perdas consecutivas que inicia uma pausa.")
        SetSpotBotToolTip(Label44, "Duração da pausa após atingir a sequência máxima de perdas.")
        SetSpotBotToolTip(Label45, "Lucro líquido mínimo exigido antes de permitir novas entradas, quando o filtro está ativo.")
        SetSpotBotToolTip(chkSpotBotRiskBrakeEnabled, "Liga ou desliga o freio de perdas da sessão.")
        SetSpotBotToolTip(chkSpotBotRiskProfitLock, "Ativa a trava de lucro e a proteção contra devolução do pico.")
        SetSpotBotToolTip(chkSpotBotRiskClosePositions, "Encerra posições simuladas quando o Handbrake é acionado.")
        SetSpotBotToolTip(chkSpotBotRiskUseUnrealized, "Inclui o P/L aberto no cálculo do patrimônio usado pelo freio.")
        SetSpotBotToolTip(chkSpotBotProfitGateEnabled, "Impede novas entradas quando o resultado líquido mínimo configurado ainda não foi alcançado.")
    End Sub

    Private Sub SetSpotBotToolTip(control As Control, text As String)
        If control IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(text) Then
            ToolTip1.SetToolTip(control, text)
        End If
    End Sub

    Private Sub ConfigureSpotBotEditorReferences(
        editor As SpotTradeBotEditor,
        universeSettings As SpotBotUniverseSettings)

        If editor Is Nothing Then Return

        _botBollingerMode = editor.cbSpotBotBollingerMode
        _botCombination = editor.cbSpotBotCombination
        _botBollingerTimeframe = editor.cbSpotBotBollingerTimeframe
        _botConfirmations = editor.nudSpotBotConfirmations
        _botUniverseMode = editor.cbSpotBotUniverseMode
        _botUniverseButton = editor.btSpotBotUniverse
        _botStartingQuote = editor.nudSpotBotStartingQuote
        _botAllocation = editor.nudSpotBotAllocation
        _botFee = editor.nudSpotBotFee
        _botTakeProfit = editor.nudSpotBotTakeProfit
        _botStopLoss = editor.nudSpotBotStopLoss
        _botUniverseMaxAssets = editor.nudSpotBotMaxAssets
        _botUniverseMinVolume = editor.nudSpotBotMinVolume
        _botBreakEvenEnabled = editor.chkSpotBotBreakEven
        _botBreakEvenTrigger = editor.nudSpotBotBreakEvenTrigger
        _botTrailingEnabled = editor.chkSpotBotTrailing
        _botTrailingDistance = editor.nudSpotBotTrailingDistance
        _botPostExitCooldown = editor.nudSpotBotPostExitCooldown
        _botUniverseMinTrades = editor.nudSpotBotMinTrades
        _botGridRiskPercent = editor.nudSpotBotGridRisk
        _botGridReservePercent = editor.nudSpotBotGridReserve
        _botGridMaxExposurePercent = editor.nudSpotBotGridExposure
        _botGridMaxLevels = editor.nudSpotBotGridLevels
        _botGridRequireRejection = editor.chkSpotBotGridRejection
        _botBtcFlushDropPercent = editor.nudSpotBotBtcFlushDrop
        _botBtcFlushPauseMinutes = editor.nudSpotBotBtcFlushPause
        _botBtcFlushEnabled = editor.chkSpotBotBtcFlush
        _botBtcFlushVolumeMultiplier = editor.nudSpotBotBtcFlushVolume
        _botBtcDailyDropPercent = editor.nudSpotBotBtcDailyDrop
        _botBtcDailyDrawdownPercent = editor.nudSpotBotBtcDailyDrawdown
        _botBtcDailyEnabled = editor.chkSpotBotBtcDaily
        _botRiskBrakeEnabled = editor.chkSpotBotRiskBrakeEnabled
        _botRiskSessionLoss = editor.nudSpotBotRiskSessionLoss
        _botRiskProfitLockEnabled = editor.chkSpotBotRiskProfitLock
        _botRiskProfitTrigger = editor.nudSpotBotRiskProfitTrigger
        _botRiskGiveback = editor.nudSpotBotRiskGiveback
        _botRiskMaxLosses = editor.nudSpotBotRiskMaxLosses
        _botRiskPauseMinutes = editor.nudSpotBotRiskPause
        _botRiskClosePositions = editor.chkSpotBotRiskClosePositions
        _botRiskUseUnrealized = editor.chkSpotBotRiskUseUnrealized
        _botStart = editor.btSpotBotStart
        _botStop = editor.btSpotBotStop
        _botExport = editor.btSpotBotExport
        _botTrades = editor.btSpotBotTrades

        _botStrategyChecks.Clear()
        _botStrategyChecks(SpotBotStrategyKind.WavePullback) = editor.chkSpotBotWavePullback
        _botStrategyChecks(SpotBotStrategyKind.BreakoutRetest) = editor.chkSpotBotBreakoutRetest
        _botStrategyChecks(SpotBotStrategyKind.MomentumVolume) = editor.chkSpotBotMomentumVolume
        _botStrategyChecks(SpotBotStrategyKind.GridAdaptive) = editor.chkSpotBotGridAdaptive
        _botStrategyChecks(SpotBotStrategyKind.ValueZonePullback) = editor.chkSpotBotValueZonePullback

        _botBollingerMode.SelectedIndex = GetBollingerModeIndex()
        _botCombination.SelectedIndex = If(
            _spotTradeBot.HasRestoredState,
            CInt(_spotTradeBot.Configuration.CombinationMode),
            CInt(SpotBotCombinationMode.MinimumConfirmations))
        _botConfirmations.Value = If(
            _spotTradeBot.HasRestoredState,
            Math.Min(_botConfirmations.Maximum, Math.Max(_botConfirmations.Minimum, _spotTradeBot.Configuration.RequiredConfirmations)),
            2D)
        _botBollingerTimeframe.SelectedIndex = GetBollingerTimeframeIndex()
        _botUniverseMode.SelectedIndex = If(universeSettings.Mode = SpotBotUniverseMode.TopVolume, 1, 0)
        _botUniverseMaxAssets.Value = Math.Min(_botUniverseMaxAssets.Maximum, Math.Max(_botUniverseMaxAssets.Minimum, universeSettings.MaxAssets))
        _botUniverseMinVolume.Value = Math.Min(_botUniverseMinVolume.Maximum, Math.Max(_botUniverseMinVolume.Minimum, universeSettings.MinimumQuoteVolume))
        _botUniverseMinTrades.Value = Math.Min(_botUniverseMinTrades.Maximum, Math.Max(_botUniverseMinTrades.Minimum, universeSettings.MinimumTrades24h))

        If _spotTradeBot.HasRestoredState Then
            Dim restoredSnapshot = _spotTradeBot.GetSimulationSnapshot()
            _botStartingQuote.Value = Math.Min(_botStartingQuote.Maximum, Math.Max(_botStartingQuote.Minimum, restoredSnapshot.StartingQuote))
            _botAllocation.Value = Math.Min(_botAllocation.Maximum, Math.Max(_botAllocation.Minimum, _spotTradeBot.Configuration.SimulationAllocationPercent))
            _botFee.Value = Math.Min(_botFee.Maximum, Math.Max(_botFee.Minimum, _spotTradeBot.Configuration.SimulationFeeRate * 100D))
            _botBreakEvenEnabled.Checked = _spotTradeBot.Configuration.BreakEvenEnabled
            _botBreakEvenTrigger.Value = Math.Min(_botBreakEvenTrigger.Maximum, Math.Max(_botBreakEvenTrigger.Minimum, _spotTradeBot.Configuration.BreakEvenTriggerPercent))
            _botTrailingEnabled.Checked = _spotTradeBot.Configuration.TrailingStopEnabled
            _botTrailingDistance.Value = Math.Min(_botTrailingDistance.Maximum, Math.Max(_botTrailingDistance.Minimum, _spotTradeBot.Configuration.TrailingStopDistancePercent))
            _botPostExitCooldown.Value = Math.Min(_botPostExitCooldown.Maximum, Math.Max(_botPostExitCooldown.Minimum, _spotTradeBot.Configuration.PostExitCooldownMinutes))
            _botGridRiskPercent.Value = Math.Min(_botGridRiskPercent.Maximum, Math.Max(_botGridRiskPercent.Minimum, _spotTradeBot.Configuration.GridV2RiskPerTradePercent))
            _botGridReservePercent.Value = Math.Min(_botGridReservePercent.Maximum, Math.Max(_botGridReservePercent.Minimum, _spotTradeBot.Configuration.GridV2ReserveQuotePercent))
            _botGridMaxExposurePercent.Value = Math.Min(_botGridMaxExposurePercent.Maximum, Math.Max(_botGridMaxExposurePercent.Minimum, _spotTradeBot.Configuration.GridV2MaxExposurePercent))
            _botGridMaxLevels.Value = Math.Min(_botGridMaxLevels.Maximum, Math.Max(_botGridMaxLevels.Minimum, _spotTradeBot.Configuration.GridV2MaxLevels))
            _botGridRequireRejection.Checked = _spotTradeBot.Configuration.GridV2RequireRejection
            _botBtcFlushDropPercent.Value = Math.Min(_botBtcFlushDropPercent.Maximum, Math.Max(_botBtcFlushDropPercent.Minimum, _spotTradeBot.Configuration.BitcoinFlushDropPercent))
            _botBtcFlushPauseMinutes.Value = Math.Min(_botBtcFlushPauseMinutes.Maximum, Math.Max(_botBtcFlushPauseMinutes.Minimum, _spotTradeBot.Configuration.BitcoinFlushPauseMinutes))
            _botBtcFlushEnabled.Checked = _spotTradeBot.Configuration.BitcoinFlushProtectionEnabled
            _botBtcFlushVolumeMultiplier.Value = Math.Min(_botBtcFlushVolumeMultiplier.Maximum, Math.Max(_botBtcFlushVolumeMultiplier.Minimum, _spotTradeBot.Configuration.BitcoinFlushVolumeMultiplier))
            _botBtcDailyDropPercent.Value = Math.Min(_botBtcDailyDropPercent.Maximum, Math.Max(_botBtcDailyDropPercent.Minimum, _spotTradeBot.Configuration.BitcoinDailyDropPercent))
            _botBtcDailyDrawdownPercent.Value = Math.Min(_botBtcDailyDrawdownPercent.Maximum, Math.Max(_botBtcDailyDrawdownPercent.Minimum, _spotTradeBot.Configuration.BitcoinDailyDrawdownPercent))
            _botBtcDailyEnabled.Checked = _spotTradeBot.Configuration.BitcoinDailyProtectionEnabled
            _botRiskBrakeEnabled.Checked = _spotTradeBot.Configuration.RiskBrakeEnabled
            _botRiskSessionLoss.Value = Math.Min(_botRiskSessionLoss.Maximum, Math.Max(_botRiskSessionLoss.Minimum, _spotTradeBot.Configuration.RiskBrakeSessionLossPercent))
            _botRiskProfitLockEnabled.Checked = _spotTradeBot.Configuration.RiskBrakeProfitLockEnabled
            _botRiskProfitTrigger.Value = Math.Min(_botRiskProfitTrigger.Maximum, Math.Max(_botRiskProfitTrigger.Minimum, _spotTradeBot.Configuration.RiskBrakeProfitLockTriggerPercent))
            _botRiskGiveback.Value = Math.Min(_botRiskGiveback.Maximum, Math.Max(_botRiskGiveback.Minimum, _spotTradeBot.Configuration.RiskBrakeProfitGivebackPercent))
            _botRiskMaxLosses.Value = Math.Min(_botRiskMaxLosses.Maximum, Math.Max(_botRiskMaxLosses.Minimum, _spotTradeBot.Configuration.RiskBrakeMaxConsecutiveLosses))
            _botRiskPauseMinutes.Value = Math.Min(_botRiskPauseMinutes.Maximum, Math.Max(_botRiskPauseMinutes.Minimum, _spotTradeBot.Configuration.RiskBrakePauseMinutes))
            _botRiskClosePositions.Checked = _spotTradeBot.Configuration.RiskBrakeClosePositionsOnHardStop
            _botRiskUseUnrealized.Checked = _spotTradeBot.Configuration.RiskBrakeUseUnrealizedPnl
            For Each strategyCheck In _botStrategyChecks
                strategyCheck.Value.Checked =
                    _spotTradeBot.Configuration.EnabledStrategies.Contains(strategyCheck.Key) OrElse
                    (strategyCheck.Key = SpotBotStrategyKind.GridAdaptive AndAlso
                     _spotTradeBot.Configuration.EnabledStrategies.Contains(SpotBotStrategyKind.GridRegime))
            Next
        End If

        AddHandler _botBollingerMode.SelectedIndexChanged, AddressOf SpotBotBollingerModeChanged
        AddHandler _botUniverseMode.SelectedIndexChanged, AddressOf SpotBotUniverseModeChanged
        AddHandler _botUniverseButton.Click, AddressOf SpotBotUniverseButton_Click
        AddHandler _botStart.Click, AddressOf SpotBotStart_Click
        AddHandler _botStop.Click, AddressOf SpotBotStop_Click
        AddHandler _botExport.Click, AddressOf SpotBotExport_Click
        AddHandler _botTrades.Click, AddressOf SpotBotTrades_Click
        UpdateSpotBotBollingerTimeframeAvailability()
    End Sub

    Private Shared Sub AddBotLabel(editor As Control, text As String, left As Integer, top As Integer, width As Integer)
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

        Dim panelTop = Math.Max(PanelGraphs.Top, _tradeBotButton.Bottom + 2)
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
        _tradeBotButton.BringToFront()
        If _btDca IsNot Nothing Then _btDca.BringToFront()
    End Sub

    Private Sub ArrangeSpotBotButton()
        If _tradeBotButton Is Nothing OrElse btOrdensFuturo Is Nothing Then
            Return
        End If

        _tradeBotButton.Location = New Point(btOrdensFuturo.Right, btOrdensFuturo.Top)
    End Sub

    Private Sub ApplySpotBotControls(Optional preserveRestoredStartingQuote As Boolean = False)
        _spotTradeBot.DisableAllStrategies()
        For Each item In _botStrategyChecks
            _spotTradeBot.SetStrategyEnabled(item.Key, item.Value.Checked)
        Next

        ApplyBollingerMode()
        ApplyBollingerTimeframe()

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
        If Not preserveRestoredStartingQuote Then
            _spotTradeBot.Configuration.SimulationStartingQuote = _botStartingQuote.Value
        End If
        _spotTradeBot.Configuration.SimulationAllocationPercent = _botAllocation.Value
        _spotTradeBot.Configuration.GridV2LevelAllocationPercent = _botAllocation.Value
        _spotTradeBot.Configuration.GridV2RiskPerTradePercent = _botGridRiskPercent.Value
        _spotTradeBot.Configuration.GridV2ReserveQuotePercent = _botGridReservePercent.Value
        _spotTradeBot.Configuration.GridV2MaxExposurePercent = _botGridMaxExposurePercent.Value
        _spotTradeBot.Configuration.GridV2MaxLevels = CInt(_botGridMaxLevels.Value)
        _spotTradeBot.Configuration.GridV2RequireRejection = _botGridRequireRejection.Checked
        Dim gridEnabled = _botStrategyChecks.ContainsKey(SpotBotStrategyKind.GridAdaptive) AndAlso
                          _botStrategyChecks(SpotBotStrategyKind.GridAdaptive).Checked
        ' O regime de queda leve do BTC faz parte do Grid V2 e é ativado
        ' automaticamente enquanto a estratégia Grid V2 estiver habilitada.
        _spotTradeBot.Configuration.GridV2BtcMildDeclineEnabled = gridEnabled
        _spotTradeBot.Configuration.BitcoinFlushProtectionEnabled = _botBtcFlushEnabled.Checked
        _spotTradeBot.Configuration.BitcoinFlushDropPercent = _botBtcFlushDropPercent.Value
        _spotTradeBot.Configuration.BitcoinFlushPauseMinutes = _botBtcFlushPauseMinutes.Value
        _spotTradeBot.Configuration.BitcoinFlushVolumeMultiplier = _botBtcFlushVolumeMultiplier.Value
        _spotTradeBot.Configuration.BitcoinDailyProtectionEnabled = _botBtcDailyEnabled.Checked
        _spotTradeBot.Configuration.BitcoinDailyDropPercent = _botBtcDailyDropPercent.Value
        _spotTradeBot.Configuration.BitcoinDailyDrawdownPercent = _botBtcDailyDrawdownPercent.Value
        _spotTradeBot.Configuration.SimulationFeeRate = _botFee.Value / 100D
        _spotTradeBot.Configuration.TakeProfitPercent = _botTakeProfit.Value
        _spotTradeBot.Configuration.StopLossPercent = _botStopLoss.Value
        _spotTradeBot.Configuration.BreakEvenEnabled = _botBreakEvenEnabled IsNot Nothing AndAlso _botBreakEvenEnabled.Checked
        _spotTradeBot.Configuration.BreakEvenTriggerPercent = If(_botBreakEvenTrigger Is Nothing, 1.5D, _botBreakEvenTrigger.Value)
        _spotTradeBot.Configuration.BreakEvenOffsetPercent = 0.1D
        _spotTradeBot.Configuration.TrailingStopEnabled = _botTrailingEnabled IsNot Nothing AndAlso _botTrailingEnabled.Checked
        _spotTradeBot.Configuration.TrailingStopDistancePercent = If(_botTrailingDistance Is Nothing, 1D, _botTrailingDistance.Value)
        _spotTradeBot.Configuration.RiskBrakeEnabled = _botRiskBrakeEnabled Is Nothing OrElse _botRiskBrakeEnabled.Checked
        _spotTradeBot.Configuration.RiskBrakeSessionLossPercent = If(_botRiskSessionLoss Is Nothing, 2D, _botRiskSessionLoss.Value)
        _spotTradeBot.Configuration.RiskBrakeProfitLockEnabled = _botRiskProfitLockEnabled Is Nothing OrElse _botRiskProfitLockEnabled.Checked
        _spotTradeBot.Configuration.RiskBrakeProfitLockTriggerPercent = If(_botRiskProfitTrigger Is Nothing, 1D, _botRiskProfitTrigger.Value)
        _spotTradeBot.Configuration.RiskBrakeProfitGivebackPercent = If(_botRiskGiveback Is Nothing, 50D, _botRiskGiveback.Value)
        _spotTradeBot.Configuration.RiskBrakeMaxConsecutiveLosses = If(_botRiskMaxLosses Is Nothing, 3, CInt(_botRiskMaxLosses.Value))
        _spotTradeBot.Configuration.RiskBrakePauseMinutes = If(_botRiskPauseMinutes Is Nothing, 60D, _botRiskPauseMinutes.Value)
        _spotTradeBot.Configuration.RiskBrakeClosePositionsOnHardStop = _botRiskClosePositions Is Nothing OrElse _botRiskClosePositions.Checked
        _spotTradeBot.Configuration.RiskBrakeUseUnrealizedPnl = _botRiskUseUnrealized Is Nothing OrElse _botRiskUseUnrealized.Checked
        _spotTradeBot.Configuration.PostExitCooldownMinutes = If(_botPostExitCooldown Is Nothing, 15D, _botPostExitCooldown.Value)
        _spotTradeBot.Configuration.NewPositionProfitGateEnabled = _botProfitGateEnabled Is Nothing OrElse _botProfitGateEnabled.Checked
        _spotTradeBot.Configuration.MinimumNetProfitBeforeNewPositionPercent = If(_botMinProfitBeforeEntry Is Nothing, 0.5D, _botMinProfitBeforeEntry.Value)
        _spotTradeBot.Configuration.SimulationOnly = True

        _spotTradeBot.Configuration.UniverseMode = If(_botUniverseMode.SelectedIndex = 1,
                                                      SpotBotUniverseMode.TopVolume,
                                                      SpotBotUniverseMode.Manual)
        _spotTradeBot.Configuration.UniverseMaxAssets = CInt(_botUniverseMaxAssets.Value)
        _spotTradeBot.Configuration.UniverseMinimumQuoteVolume = _botUniverseMinVolume.Value
        _spotTradeBot.Configuration.UniverseMinimumTrades24h = If(_botUniverseMinTrades Is Nothing, 1000, CInt(_botUniverseMinTrades.Value))
    End Sub

    Private Async Sub SpotBotStart_Click(sender As Object, e As EventArgs)
        Dim resumePreviousSimulation = False
        If _spotTradeBot.HasRestoredState Then
            Dim answer = MessageBox.Show(
                "Foi encontrada uma simulação anterior pausada ou com posições abertas." & Environment.NewLine &
                "Sim = continuar essa simulação; Não = iniciar uma nova; Cancelar = voltar.",
                "Tradebot",
                MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button1)

            If answer = DialogResult.Cancel Then Return
            resumePreviousSimulation = answer = DialogResult.Yes
        End If

        ApplySpotBotControls(resumePreviousSimulation)
        If _spotTradeBot.Configuration.EnabledStrategies.Count = 0 Then
            AppendSpotBotLog("Selecione pelo menos uma estratégia antes de iniciar.", Color.Gold)
            Return
        End If

        Dim universe As List(Of String)
        Try
            universe = Await ResolveSpotBotUniverseAsync()
        Catch ex As Exception
            AppendSpotBotLog("Erro carregando o universo do Tradebot: " & ex.Message, Color.IndianRed)
            Return
        End Try

        If universe.Count = 0 Then
            AppendSpotBotLog("Nenhum ativo foi encontrado para o universo selecionado.", Color.Gold)
            Return
        End If

        If Not resumePreviousSimulation Then
            _spotTradeBot.ResetHistory()
        End If

        Try
            AppendSpotBotLog(
                "Carregando candles históricos antes de conectar o WebSocket...",
                Color.Gold)
            Dim warmup = Await _spotTradeBot.WarmupHistoricalDataAsync(
                universe,
                AddressOf LoadSpotBotHistoricalCandlesAsync)
            AppendSpotBotLog(
                $"Histórico carregado: {warmup.LoadedSymbols}/{warmup.RequestedSymbols} ativo(s), " &
                $"{warmup.TotalCandles} candle(s), intervalos {String.Join(", ", warmup.Intervals)}.",
                Color.LightGreen)
            If warmup.FailedSymbols.Count > 0 Then
                AppendSpotBotLog(
                    "Ativos sem histórico: " & String.Join(", ", warmup.FailedSymbols.Take(10)) &
                    If(warmup.FailedSymbols.Count > 10, "...", String.Empty),
                    Color.Gold)
            End If
        Catch ex As Exception
            AppendSpotBotLog("Erro carregando histórico do TradeBot: " & ex.Message, Color.IndianRed)
            Return
        End Try

        If resumePreviousSimulation Then
            _spotTradeBot.ContinueAfterHandbrake()
        End If

        UpdateSpotBotPositionsGrid(_spotTradeBot.GetOpenPositions())
        _spotTradeBot.Start()
        _botUniverseMode.Enabled = False
        _botUniverseMaxAssets.Enabled = False
        _botUniverseMinVolume.Enabled = False
        _botUniverseMinTrades.Enabled = False
        _botUniverseButton.Enabled = False
        _botTakeProfit.Enabled = False
        _botStopLoss.Enabled = False
        _botBreakEvenEnabled.Enabled = False
        _botBreakEvenTrigger.Enabled = False
        _botTrailingEnabled.Enabled = False
        _botTrailingDistance.Enabled = False
        _botRiskBrakeEnabled.Enabled = False
        _botRiskSessionLoss.Enabled = False
        _botRiskProfitLockEnabled.Enabled = False
        _botRiskProfitTrigger.Enabled = False
        _botRiskGiveback.Enabled = False
        _botRiskMaxLosses.Enabled = False
        _botRiskPauseMinutes.Enabled = False
        _botRiskClosePositions.Enabled = False
        _botRiskUseUnrealized.Enabled = False
        _botPostExitCooldown.Enabled = False
        _botProfitGateEnabled.Enabled = False
        _botMinProfitBeforeEntry.Enabled = False
        _botGridRiskPercent.Enabled = False
        _botGridReservePercent.Enabled = False
        _botGridMaxExposurePercent.Enabled = False
        _botGridMaxLevels.Enabled = False
        _botGridRequireRejection.Enabled = False
        _botBtcFlushDropPercent.Enabled = False
        _botBtcFlushPauseMinutes.Enabled = False
        _botBtcFlushEnabled.Enabled = False
        _botBtcFlushVolumeMultiplier.Enabled = False
        _botBtcDailyDropPercent.Enabled = False
        _botBtcDailyDrawdownPercent.Enabled = False
        _botBtcDailyEnabled.Enabled = False
        Await StartBinanceWebSocket()
        _botStart.Enabled = False
        _botStop.Enabled = True
        _lastBotSimulationAction = String.Empty
        RenderSpotBotSimulationSummary(_spotTradeBot.GetSimulationSnapshot())
        AppendSpotBotLog(
            If(resumePreviousSimulation,
               "Bot Spot retomado em modo SIMULAÇÃO.",
               "Bot Spot iniciado em modo SIMULAÇÃO."),
            Color.LimeGreen)
        AppendSpotBotLog(
            If(_botBollingerMode.SelectedIndex = 0,
               "Perfil ativo: Base Original Protegida (Bollinger 25/3); alvo na média central e stop no extremo do candle.",
               "Perfil base: Base Original Protegida desativada."),
            Color.LightGray)
        AppendSpotBotLog(
            $"Timeframe: {If(_spotTradeBot.Configuration.BollingerCandleMinutes = 240, "4 horas", "diário")}.",
            Color.LightGray)
        AppendSpotBotLog($"Universo: {universe.Count} ativo(s) ({If(_spotTradeBot.Configuration.UniverseMode = SpotBotUniverseMode.TopVolume, "Top volume", "Manual")}).", Color.LightGray)
        AppendSpotBotLog(
            $"Liquidez mínima: {_spotTradeBot.Configuration.UniverseMinimumQuoteVolume:0} USDT de volume e " &
            $"{_spotTradeBot.Configuration.UniverseMinimumTrades24h:0} negócios em 24h.",
            Color.LightGray)
        AppendSpotBotLog("Estratégias: " & String.Join(", ", _spotTradeBot.Configuration.EnabledStrategies), Color.LightGray)
        AppendSpotBotLog(
            $"Alocação: {_spotTradeBot.Configuration.SimulationAllocationPercent:0.##}% " &
            "(Grid: saldo inicial por nível; demais: saldo disponível).",
            Color.LightGray)
        AppendSpotBotLog(
            $"Proteções: BE {If(_spotTradeBot.Configuration.BreakEvenEnabled, "ativo", "desligado")} após " &
            $"{_spotTradeBot.Configuration.BreakEvenTriggerPercent:0.##}% | " &
            $"Trailing {If(_spotTradeBot.Configuration.TrailingStopEnabled, "ativo", "desligado")} " &
            $"({_spotTradeBot.Configuration.TrailingStopDistancePercent:0.##}%).",
            Color.LightGray)
        AppendSpotBotLog(
            $"Cooldown pós-saída: {_spotTradeBot.Configuration.PostExitCooldownMinutes:0.##} minuto(s) por ativo.",
            Color.LightGray)
        AppendSpotBotLog(
            $"Filtro de novas posições: {If(_spotTradeBot.Configuration.NewPositionProfitGateEnabled, "ativo", "desligado")}; " &
            $"exige lucro líquido mínimo de +{_spotTradeBot.Configuration.MinimumNetProfitBeforeNewPositionPercent:0.##}% " &
            "nas posições abertas; não bloqueia Grid/DCA do mesmo ativo nem saídas.",
            Color.LightGray)
        AppendSpotBotLog(
            $"Proteção flush BTC: {If(_spotTradeBot.Configuration.BitcoinFlushProtectionEnabled, "ativa", "desligada")}; " &
            $"queda {_spotTradeBot.Configuration.BitcoinFlushDropPercent:0.##}% em " &
            $"{_spotTradeBot.Configuration.BitcoinFlushLookbackMinutes}m, volume mínimo " &
            $"{_spotTradeBot.Configuration.BitcoinFlushVolumeMultiplier:0.##}x, pausa " &
            $"{_spotTradeBot.Configuration.BitcoinFlushPauseMinutes:0} min. Stops continuam ativos.",
            Color.LightGray)
        AppendSpotBotLog(
            $"Proteção diária BTC: {If(_spotTradeBot.Configuration.BitcoinDailyProtectionEnabled, "ativa", "desligada")}; " &
            $"queda {_spotTradeBot.Configuration.BitcoinDailyDropPercent:0.##}% ou drawdown da máxima " &
            $"{_spotTradeBot.Configuration.BitcoinDailyDrawdownPercent:0.##}%. " &
            "A pausa bloqueia novas entradas e mantém stops ativos.",
            Color.LightGray)
        AppendSpotBotLog(
            $"Freio de perdas: {If(_spotTradeBot.Configuration.RiskBrakeEnabled, "ativo", "desligado")}; " &
            $"limite {_spotTradeBot.Configuration.RiskBrakeSessionLossPercent:0.##}%; " &
            $"trava +{_spotTradeBot.Configuration.RiskBrakeProfitLockTriggerPercent:0.##}% / " &
            $"devolução {_spotTradeBot.Configuration.RiskBrakeProfitGivebackPercent:0.##}%; " &
            $"até {_spotTradeBot.Configuration.RiskBrakeMaxConsecutiveLosses} perdas seguidas.",
            Color.Gold)
        If _spotTradeBot.Configuration.EnabledStrategies.Contains(SpotBotStrategyKind.WavePullback) Then
            AppendSpotBotLog(
                "Pullback EMA34: SL e TP estruturais do sinal; os percentuais globais não bloqueiam a inicialização.",
                Color.LightGray)
        End If
        If _spotTradeBot.Configuration.EnabledStrategies.Contains(SpotBotStrategyKind.ValueZonePullback) Then
            AppendSpotBotLog(
                "Pullback zona de valor: M5 filtra tendência, M2 espera retorno à EMA 9/20 e M1 confirma a retomada; Spot opera somente comprado.",
                Color.LightGray)
        End If
        If _spotTradeBot.Configuration.EnabledStrategies.Contains(SpotBotStrategyKind.GridAdaptive) Then
            AppendSpotBotLog(
                $"Grid V2: EMA 9/21 em candles M5 libera a grade apenas em lateralização; " &
                $"custos simulados: taxa {_spotTradeBot.Configuration.SimulationFeeRate * 100D:0.###}% + slippage {_spotTradeBot.Configuration.SimulationSlippagePercent:0.###}%; " &
                $"confirma rejeição: {If(_spotTradeBot.Configuration.GridV2RequireRejection, "sim", "não")}; " &
                $"risco/nível {_spotTradeBot.Configuration.GridV2RiskPerTradePercent:0.##}%, " &
                $"reserva {_spotTradeBot.Configuration.GridV2ReserveQuotePercent:0.##}%, " &
                $"exposição máxima {_spotTradeBot.Configuration.GridV2MaxExposurePercent:0.##}% e " &
                $"até {_spotTradeBot.Configuration.GridV2MaxLevels} níveis; " &
                $"tempo máximo {_spotTradeBot.Configuration.GridV2MaximumHoldingMinutes} min, " &
                $"saída por deterioração de regime: {If(_spotTradeBot.Configuration.GridV2RegimeExitEnabled, "sim", "não")}, " &
                $"redução progressiva por nível: {If(_spotTradeBot.Configuration.GridV2ProgressiveExposureEnabled, _spotTradeBot.Configuration.GridV2ProgressiveExposureReductionPercent.ToString("0.##") & "%", "não")}; " &
                $"ignora suporte a menos de {_spotTradeBot.Configuration.GridV2MinimumSupportDistancePercent:0.##}%, confirma o SL em {Math.Max(1, _spotTradeBot.Configuration.GridV2StopConfirmationCandles)} candles de 1 min " &
                $"(emergência em {_spotTradeBot.Configuration.GridV2StopEmergencyBufferPercent:0.##}%), usa SL estrutural com mínimo de {_spotTradeBot.Configuration.GridV2MinimumStopPercent:0.##}% e limite de 3%, " &
                "e bloqueia o ativo após 2 stops até a recuperação confirmada." &
                If(_spotTradeBot.Configuration.GridV2BtcMildDeclineEnabled,
                   $" Regime BTC Leve Queda ativo: queda diária entre {_spotTradeBot.Configuration.GridV2BtcMildDeclineMinimumPercent:0.##}% e {_spotTradeBot.Configuration.GridV2BtcMildDeclineMaximumPercent:0.##}%.",
                   String.Empty),
                Color.LightGray)
        End If
    End Sub

    Private Sub SpotBotStop_Click(sender As Object, e As EventArgs)
        _spotTradeBot.Stop()
        _botStart.Enabled = True
        _botStop.Enabled = False
        _botUniverseMode.Enabled = True
        _botUniverseMaxAssets.Enabled = True
        _botUniverseMinVolume.Enabled = True
        _botUniverseMinTrades.Enabled = True
        _botUniverseButton.Enabled = True
        _botTakeProfit.Enabled = True
        _botStopLoss.Enabled = True
        _botBreakEvenEnabled.Enabled = True
        _botBreakEvenTrigger.Enabled = True
        _botTrailingEnabled.Enabled = True
        _botTrailingDistance.Enabled = True
        _botRiskBrakeEnabled.Enabled = True
        _botRiskSessionLoss.Enabled = True
        _botRiskProfitLockEnabled.Enabled = True
        _botRiskProfitTrigger.Enabled = True
        _botRiskGiveback.Enabled = True
        _botRiskMaxLosses.Enabled = True
        _botRiskPauseMinutes.Enabled = True
        _botRiskClosePositions.Enabled = True
        _botRiskUseUnrealized.Enabled = True
        _botPostExitCooldown.Enabled = True
        _botProfitGateEnabled.Enabled = True
        _botMinProfitBeforeEntry.Enabled = True
        _botGridRiskPercent.Enabled = True
        _botGridReservePercent.Enabled = True
        _botGridMaxExposurePercent.Enabled = True
        _botGridMaxLevels.Enabled = True
        _botGridRequireRejection.Enabled = True
        _botBtcFlushDropPercent.Enabled = True
        _botBtcFlushPauseMinutes.Enabled = True
        _botBtcFlushEnabled.Enabled = True
        _botBtcFlushVolumeMultiplier.Enabled = True
        _botBtcDailyDropPercent.Enabled = True
        _botBtcDailyDrawdownPercent.Enabled = True
        _botBtcDailyEnabled.Enabled = True
        AppendSpotBotLog("Bot Spot parado.", Color.IndianRed)
    End Sub

    Private Sub SpotBotExport_Click(sender As Object, e As EventArgs)
        Try
            Dim state = SpotTradeBotRepository.LoadLatestRestorableState()
            If state Is Nothing OrElse state.Snapshot Is Nothing Then
                MessageBox.Show(
                    "Ainda não há uma simulação salva para exportar.",
                    "Exportar Tradebot",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information)
                Return
            End If

            Using dialog As New SaveFileDialog With {
                .Title = "Exportar resultados do Tradebot",
                .Filter = "Arquivo CSV (*.csv)|*.csv",
                .DefaultExt = "csv",
                .AddExtension = True,
                .InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                .FileName = $"tradebot_run_{state.RunId}.csv"}

                If dialog.ShowDialog(Me) <> DialogResult.OK Then Return

                Dim basePath = Path.Combine(
                    Path.GetDirectoryName(dialog.FileName),
                    Path.GetFileNameWithoutExtension(dialog.FileName))

                WriteSpotBotSummaryCsv(basePath & "_resumo.csv", state)
                WriteSpotBotDataTableCsv(basePath & "_trades.csv", SpotTradeBotRepository.GetTrades(state.RunId))
                WriteSpotBotDataTableCsv(basePath & "_posicoes.csv", SpotTradeBotRepository.GetOpenPositions(state.RunId))
                WriteSpotBotComparisonCsv(basePath & "_comparacao.csv", state)

                MessageBox.Show(
                    "Resultados exportados em quatro arquivos CSV:" & Environment.NewLine &
                    Path.GetFileName(basePath & "_resumo.csv") & Environment.NewLine &
                    Path.GetFileName(basePath & "_trades.csv") & Environment.NewLine &
                    Path.GetFileName(basePath & "_posicoes.csv") & Environment.NewLine &
                    Path.GetFileName(basePath & "_comparacao.csv"),
                    "Exportar Tradebot",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information)
            End Using
        Catch ex As Exception
            MessageBox.Show(
                "Erro exportando os resultados: " & ex.Message,
                "Exportar Tradebot",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub SpotBotTrades_Click(sender As Object, e As EventArgs)
        Try
            Dim runId = _spotTradeBot.ActiveRunId
            If runId <= 0 Then
                Dim state = SpotTradeBotRepository.LoadLatestRestorableState()
                runId = If(state Is Nothing, 0L, state.RunId)
            End If

            If runId <= 0 Then
                MessageBox.Show(
                    "Ainda não há uma run do Tradebot com trades registrados.",
                    "Trades da run",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information)
                Return
            End If

            Using tradesForm As New SpotTradeBotTradesForm(runId)
                tradesForm.ShowDialog(Me)
            End Using
        Catch ex As Exception
            MessageBox.Show(
                "Erro carregando os trades da run: " & ex.Message,
                "Trades da run",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error)
        End Try
    End Sub

    Private Shared Sub WriteSpotBotSummaryCsv(filePath As String, state As SpotBotPersistedSimulationState)
        Dim snapshot = state.Snapshot
        Dim builder As New StringBuilder()
        builder.AppendLine("Campo;Valor")
        AppendSpotBotCsvPair(builder, "RunId", state.RunId)
        AppendSpotBotCsvPair(builder, "Status", state.Status)
        AppendSpotBotCsvPair(builder, "Estrategias", String.Join(", ", state.EnabledStrategies.OrderBy(Function(item) item.ToString())))
        AppendSpotBotCsvPair(builder, "Combinacao", state.CombinationMode)
        AppendSpotBotCsvPair(builder, "Confirmacoes", state.RequiredConfirmations)
        AppendSpotBotCsvPair(builder, "SaldoInicialUSDT", snapshot.StartingQuote)
        AppendSpotBotCsvPair(builder, "SaldoDisponivelUSDT", snapshot.QuoteBalance)
        AppendSpotBotCsvPair(builder, "ValorAbertoUSDT", snapshot.OpenValue)
        AppendSpotBotCsvPair(builder, "PnlRealizadoUSDT", snapshot.RealizedPnl)
        AppendSpotBotCsvPair(builder, "PnlAbertoUSDT", snapshot.UnrealizedPnl)
        AppendSpotBotCsvPair(builder, "PatrimonioUSDT", snapshot.Equity)
        AppendSpotBotCsvPair(builder, "PnlTotalUSDT", snapshot.TotalPnl)
        AppendSpotBotCsvPair(builder, "Trades", snapshot.CompletedTrades)
        AppendSpotBotCsvPair(builder, "TradesVencedores", snapshot.WinningTrades)
        AppendSpotBotCsvPair(builder, "TradesPerdedores", snapshot.LosingTrades)
        AppendSpotBotCsvPair(builder, "WinRatePercentual", snapshot.WinRatePercent)
        AppendSpotBotCsvPair(builder, "BollingerPeriodo", state.BollingerPeriod)
        AppendSpotBotCsvPair(builder, "BollingerDesvios", state.BollingerStandardDeviations)
        AppendSpotBotCsvPair(builder, "BollingerCandleMinutos", state.BollingerCandleMinutes)
        AppendSpotBotCsvPair(builder, "BreakEvenAtivo", state.BreakEvenEnabled)
        AppendSpotBotCsvPair(builder, "BreakEvenGatilhoPercentual", state.BreakEvenTriggerPercent)
        AppendSpotBotCsvPair(builder, "BreakEvenOffsetPercentual", state.BreakEvenOffsetPercent)
        AppendSpotBotCsvPair(builder, "TrailingAtivo", state.TrailingStopEnabled)
        AppendSpotBotCsvPair(builder, "TrailingDistanciaPercentual", state.TrailingStopDistancePercent)
        AppendSpotBotCsvPair(builder, "FreioPerdasAtivo", state.RiskBrakeEnabled)
        AppendSpotBotCsvPair(builder, "FreioPerdaSessaoPercentual", state.RiskBrakeSessionLossPercent)
        AppendSpotBotCsvPair(builder, "FreioTravaLucroAtiva", state.RiskBrakeProfitLockEnabled)
        AppendSpotBotCsvPair(builder, "FreioGatilhoLucroPercentual", state.RiskBrakeProfitLockTriggerPercent)
        AppendSpotBotCsvPair(builder, "FreioDevolucaoPicoPercentual", state.RiskBrakeProfitGivebackPercent)
        AppendSpotBotCsvPair(builder, "FreioMaxPerdasSeguidas", state.RiskBrakeMaxConsecutiveLosses)
        AppendSpotBotCsvPair(builder, "FreioPausaMinutos", state.RiskBrakePauseMinutes)
        AppendSpotBotCsvPair(builder, "FreioFecharPosicoes", state.RiskBrakeClosePositionsOnHardStop)
        AppendSpotBotCsvPair(builder, "FreioConsideraPnlAberto", state.RiskBrakeUseUnrealizedPnl)
        AppendSpotBotCsvPair(builder, "FreioEstadoAtual", snapshot.RiskBrakeState)
        AppendSpotBotCsvPair(builder, "FreioPicoPatrimonio", snapshot.RiskBrakePeakEquity)
        AppendSpotBotCsvPair(builder, "FreioPerdasSeguidasAtual", snapshot.RiskBrakeConsecutiveLosses)
        AppendSpotBotCsvPair(builder, "FreioMotivo", snapshot.RiskBrakeReason)
        AppendSpotBotCsvPair(builder, "FreioEntradasBloqueadas", snapshot.RiskBrakeBlockedEntries)
        AppendSpotBotCsvPair(builder, "FreioPnLEvitadoEstimadoUSDT", snapshot.RiskBrakeEstimatedAvoidedPnl)
        AppendSpotBotCsvPair(builder, "FreioPausas", snapshot.RiskBrakePauseCount)
        AppendSpotBotCsvPair(builder, "FreioUltimoInicioPausaUTC", snapshot.RiskBrakeLastPauseStartedUtc)
        AppendSpotBotCsvPair(builder, "FreioUltimoFimPausaUTC", snapshot.RiskBrakeLastPauseEndedUtc)
        AppendSpotBotCsvPair(builder, "CooldownPosSaidaMinutos", state.PostExitCooldownMinutes)
        AppendSpotBotCsvPair(builder, "FiltroLucroNovaPosicaoAtivo", state.NewPositionProfitGateEnabled)
        AppendSpotBotCsvPair(builder, "LucroLiquidoMinimoNovaPosicaoPercentual", state.MinimumNetProfitBeforeNewPositionPercent)
        AppendSpotBotCsvPair(builder, "GridV2SuporteMaisProximo", state.GridV2UseNearestSupportStop)
        AppendSpotBotCsvPair(builder, "GridV2BufferSuporteATR", state.GridV2SupportBufferAtr)
        AppendSpotBotCsvPair(builder, "GridV2StopMinimoPercentual", state.GridV2MinimumStopPercent)
        AppendSpotBotCsvPair(builder, "GridV2StopMaximoPercentual", state.GridV2MaximumStopPercent)
        AppendSpotBotCsvPair(builder, "GridV2DistanciaMinimaSuportePercentual", state.GridV2MinimumSupportDistancePercent)
        AppendSpotBotCsvPair(builder, "GridV2ConfirmacaoSLAtiva", state.GridV2StopConfirmationEnabled)
        AppendSpotBotCsvPair(builder, "GridV2ConfirmacaoSLCandles", state.GridV2StopConfirmationCandles)
        AppendSpotBotCsvPair(builder, "GridV2BufferEmergenciaSLPercentual", state.GridV2StopEmergencyBufferPercent)
        AppendSpotBotCsvPair(builder, "GridV2ExposicaoMaximaPercentual", state.GridV2MaxExposurePercent)
        AppendSpotBotCsvPair(builder, "GridV2ConfirmarRejeicao", state.GridV2RequireRejection)
        AppendSpotBotCsvPair(builder, "GridV2RiscoPorNivelPercentual", state.GridV2RiskPerTradePercent)
        AppendSpotBotCsvPair(builder, "GridV2ReservaPercentual", state.GridV2ReserveQuotePercent)
        AppendSpotBotCsvPair(builder, "GridV2NiveisMaximos", state.GridV2MaxLevels)
        AppendSpotBotCsvPair(builder, "GridV2TempoMaximoPermanenciaMinutos", state.GridV2MaximumHoldingMinutes)
        AppendSpotBotCsvPair(builder, "GridV2SaidaPorDeterioracaoRegime", state.GridV2RegimeExitEnabled)
        AppendSpotBotCsvPair(builder, "GridV2ExposicaoProgressiva", state.GridV2ProgressiveExposureEnabled)
        AppendSpotBotCsvPair(builder, "GridV2ReducaoExposicaoPorNivelPercentual", state.GridV2ProgressiveExposureReductionPercent)
        AppendSpotBotCsvPair(builder, "GridV2RegimeBTCLeveQueda", state.GridV2BtcMildDeclineEnabled)
        AppendSpotBotCsvPair(builder, "GridV2BTCLeveQuedaMinimaPercentual", state.GridV2BtcMildDeclineMinimumPercent)
        AppendSpotBotCsvPair(builder, "GridV2BTCLeveQuedaMaximaPercentual", state.GridV2BtcMildDeclineMaximumPercent)
        AppendSpotBotCsvPair(builder, "ProtecaoFlushBTC", state.BitcoinFlushProtectionEnabled)
        AppendSpotBotCsvPair(builder, "FlushBTCQuedaPercentual", state.BitcoinFlushDropPercent)
        AppendSpotBotCsvPair(builder, "FlushBTCPausaMinutos", state.BitcoinFlushPauseMinutes)
        AppendSpotBotCsvPair(builder, "FlushBTCVolumeMultiplicador", state.BitcoinFlushVolumeMultiplier)
        AppendSpotBotCsvPair(builder, "ProtecaoDiariaBTC", state.BitcoinDailyProtectionEnabled)
        AppendSpotBotCsvPair(builder, "QuedaDiariaBTCPercentual", state.BitcoinDailyDropPercent)
        AppendSpotBotCsvPair(builder, "DrawdownMaximaBTCPercentual", state.BitcoinDailyDrawdownPercent)
        AppendSpotBotCsvPair(builder, "Negocios24hMinimos", state.UniverseMinimumTrades24h)
        AppendSpotBotCsvPair(builder, "AlocacaoPercentual", state.AllocationPercent)
        AppendSpotBotCsvPair(builder, "Taxa", state.FeeRate)
        AppendSpotBotCsvPair(builder, "SlippagePercentual", state.SlippagePercent)
        AppendSpotBotCsvPair(builder, "UltimaAcao", snapshot.LastAction)
        File.WriteAllText(filePath, builder.ToString(), New UTF8Encoding(True))
    End Sub

    Private Shared Sub WriteSpotBotComparisonCsv(filePath As String, state As SpotBotPersistedSimulationState)
        Dim builder As New StringBuilder()
        builder.AppendLine("Campo;Atual;Run anterior;Delta")

        If state Is Nothing OrElse state.Snapshot Is Nothing Then
            File.WriteAllText(filePath, builder.ToString(), New UTF8Encoding(True))
            Return
        End If

        Dim runs = SpotTradeBotRepository.GetRuns()
        Dim previousRun As DataRow = Nothing
        If runs IsNot Nothing Then
            previousRun = runs.AsEnumerable().
                Where(Function(row) Not row.IsNull("Id") AndAlso Convert.ToInt64(row("Id"), CultureInfo.InvariantCulture) < state.RunId).
                OrderByDescending(Function(row) Convert.ToInt64(row("Id"), CultureInfo.InvariantCulture)).
                FirstOrDefault()
        End If

        Dim currentTrades = SpotTradeBotRepository.GetTrades(state.RunId)
        Dim currentStops = If(currentTrades Is Nothing,
                              0D,
                              currentTrades.AsEnumerable().Count(Function(row) IsSpotBotStopExitReason(row("ExitReason"))))
        Dim currentFees = If(currentTrades Is Nothing,
                             0D,
                             currentTrades.AsEnumerable().Sum(Function(row) DecimalFromDataRow(row, "Fees")))

        Dim previousRunId As Object = Nothing
        Dim previousTrades As DataTable = Nothing
        Dim previousPnl = 0D
        Dim previousTradesCount = 0D
        Dim previousStops = 0D
        Dim previousWinRate = 0D
        Dim previousFees = 0D

        If previousRun IsNot Nothing Then
            previousRunId = previousRun("Id")
            Dim previousId = Convert.ToInt64(previousRun("Id"), CultureInfo.InvariantCulture)
            previousTrades = SpotTradeBotRepository.GetTrades(previousId)
            previousTradesCount = DecimalFromDataRow(previousRun, "CompletedTrades")
            previousPnl = DecimalFromDataRow(previousRun, "TotalPnl")
            Dim previousWins = DecimalFromDataRow(previousRun, "WinningTrades")
            previousWinRate = If(previousTradesCount = 0D, 0D, previousWins * 100D / previousTradesCount)
            previousStops = If(previousTrades Is Nothing,
                               0D,
                               previousTrades.AsEnumerable().Count(Function(row) IsSpotBotStopExitReason(row("ExitReason"))))
            previousFees = If(previousTrades Is Nothing,
                              0D,
                              previousTrades.AsEnumerable().Sum(Function(row) DecimalFromDataRow(row, "Fees")))
        End If

        AppendSpotBotComparisonRow(builder, "RunId", state.RunId, previousRunId)
        AppendSpotBotComparisonRow(builder, "PnlTotalUSDT", state.Snapshot.TotalPnl, If(previousRun Is Nothing, Nothing, previousPnl))
        AppendSpotBotComparisonRow(builder, "Trades", state.Snapshot.CompletedTrades, If(previousRun Is Nothing, Nothing, previousTradesCount))
        AppendSpotBotComparisonRow(builder, "StopsSL", currentStops, If(previousRun Is Nothing, Nothing, previousStops))
        AppendSpotBotComparisonRow(builder, "WinRatePercentual", state.Snapshot.WinRatePercent, If(previousRun Is Nothing, Nothing, previousWinRate))
        AppendSpotBotComparisonRow(builder, "TaxasUSDT", currentFees, If(previousRun Is Nothing, Nothing, previousFees))

        File.WriteAllText(filePath, builder.ToString(), New UTF8Encoding(True))
    End Sub

    Private Shared Function IsSpotBotStopExitReason(value As Object) As Boolean
        Dim reason = Convert.ToString(value, CultureInfo.InvariantCulture)
        Return String.Equals(reason, "SL", StringComparison.OrdinalIgnoreCase) OrElse
               reason.StartsWith("SL_", StringComparison.OrdinalIgnoreCase)
    End Function

    Private Shared Sub AppendSpotBotComparisonRow(builder As StringBuilder, name As String, currentValue As Object, previousValue As Object)
        Dim currentText = FormatSpotBotCsvValue(currentValue)
        Dim previousText = If(previousValue Is Nothing, String.Empty, FormatSpotBotCsvValue(previousValue))
        Dim deltaText = String.Empty

        If previousValue IsNot Nothing AndAlso TypeOf currentValue Is IConvertible Then
            Dim currentNumber As Decimal
            Dim previousNumber As Decimal
            If Decimal.TryParse(currentText, NumberStyles.Any, CultureInfo.InvariantCulture, currentNumber) AndAlso
               Decimal.TryParse(previousText, NumberStyles.Any, CultureInfo.InvariantCulture, previousNumber) Then
                deltaText = FormatSpotBotCsvValue(currentNumber - previousNumber)
            End If
        End If

        builder.Append(CsvEscape(name)).Append(";").
            Append(CsvEscape(currentText)).Append(";").
            Append(CsvEscape(previousText)).Append(";").
            AppendLine(CsvEscape(deltaText))
    End Sub

    Private Shared Function DecimalFromDataRow(row As DataRow, columnName As String) As Decimal
        If row Is Nothing OrElse Not row.Table.Columns.Contains(columnName) OrElse row.IsNull(columnName) Then Return 0D
        Return Convert.ToDecimal(row(columnName), CultureInfo.InvariantCulture)
    End Function

    Private Shared Sub AppendSpotBotCsvPair(builder As StringBuilder, name As String, value As Object)
        builder.Append(CsvEscape(name)).Append(";").AppendLine(CsvEscape(FormatSpotBotCsvValue(value)))
    End Sub

    Private Shared Sub WriteSpotBotDataTableCsv(filePath As String, table As DataTable)
        Dim builder As New StringBuilder()
        If table Is Nothing Then
            File.WriteAllText(filePath, String.Empty, New UTF8Encoding(True))
            Return
        End If

        builder.AppendLine(String.Join(";", table.Columns.Cast(Of DataColumn)().Select(Function(column) CsvEscape(column.ColumnName))))
        For Each row As DataRow In table.Rows
            builder.AppendLine(String.Join(";", table.Columns.Cast(Of DataColumn)().Select(Function(column) CsvEscape(FormatSpotBotCsvValue(row(column))))))
        Next

        File.WriteAllText(filePath, builder.ToString(), New UTF8Encoding(True))
    End Sub

    Private Shared Function FormatSpotBotCsvValue(value As Object) As String
        If value Is Nothing OrElse Convert.IsDBNull(value) Then Return String.Empty
        If TypeOf value Is DateTime Then Return DirectCast(value, DateTime).ToString("O", CultureInfo.InvariantCulture)
        If TypeOf value Is Decimal Then Return DirectCast(value, Decimal).ToString("0.########", CultureInfo.InvariantCulture)
        If TypeOf value Is Double Then Return DirectCast(value, Double).ToString("0.########", CultureInfo.InvariantCulture)
        If TypeOf value Is Single Then Return DirectCast(value, Single).ToString("0.########", CultureInfo.InvariantCulture)
        Return Convert.ToString(value, CultureInfo.InvariantCulture)
    End Function

    Private Shared Function CsvEscape(value As String) As String
        Dim quote = ChrW(34)
        Dim text = If(value, String.Empty).Replace(quote.ToString(), New String(quote, 2))
        If text.Contains(";"c) OrElse text.Contains(quote) OrElse text.Contains(ControlChars.Cr) OrElse text.Contains(ControlChars.Lf) Then
            Return quote & text & quote
        End If
        Return text
    End Function

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

        If signal.StopPrice > 0D OrElse signal.TakeProfitPrice > 0D Then
            AppendSpotBotLog(
                $"   Proteção estrutural: SL {If(signal.StopPrice > 0D, signal.StopPrice.ToString("0.########"), "-")} | " &
                $"TP {If(signal.TakeProfitPrice > 0D, signal.TakeProfitPrice.ToString("0.########"), "-")}",
                Color.LightGray)
        End If
    End Sub

    Private Sub SpotTradeBot_SimulationUpdated(sender As Object, e As SpotBotSimulationEventArgs)
        If e Is Nothing OrElse e.Snapshot Is Nothing Then Return

        If InvokeRequired Then
            BeginInvoke(New MethodInvoker(Sub() SpotTradeBot_SimulationUpdated(Me, e)))
            Return
        End If

        If _dgSpotBotPositions IsNot Nothing Then
            UpdateSpotBotPositionsGrid(_spotTradeBot.GetOpenPositions())
        End If

        UpdateSpotBotRunButtons(e.Snapshot)

        If _botSimulationSummary Is Nothing Then Return

        Dim nowUtc = DateTime.UtcNow
        If nowUtc - _lastBotSummaryUpdateUtc < TimeSpan.FromMilliseconds(250) AndAlso
           String.Equals(_lastBotSimulationAction, e.Snapshot.LastAction, StringComparison.Ordinal) Then
            Return
        End If

        Dim previousAction = _lastBotSimulationAction
        _lastBotSummaryUpdateUtc = nowUtc
        _lastBotSimulationAction = e.Snapshot.LastAction
        If Not String.Equals(previousAction, e.Snapshot.LastAction, StringComparison.Ordinal) AndAlso
           (e.Snapshot.LastAction.StartsWith("TP ", StringComparison.OrdinalIgnoreCase) OrElse
            e.Snapshot.LastAction.StartsWith("SL ", StringComparison.OrdinalIgnoreCase) OrElse
            e.Snapshot.LastAction.StartsWith("BE ", StringComparison.OrdinalIgnoreCase) OrElse
            e.Snapshot.LastAction.StartsWith("TRAIL ", StringComparison.OrdinalIgnoreCase) OrElse
            e.Snapshot.LastAction.StartsWith("FREIO", StringComparison.OrdinalIgnoreCase) OrElse
            e.Snapshot.LastAction.StartsWith("HANDBRAKE", StringComparison.OrdinalIgnoreCase) OrElse
            e.Snapshot.LastAction.StartsWith("FILTRO", StringComparison.OrdinalIgnoreCase)) Then
            AppendSpotBotLog(
                e.Snapshot.LastAction,
                If(e.Snapshot.LastAction.StartsWith("SL ", StringComparison.OrdinalIgnoreCase) OrElse
                   e.Snapshot.LastAction.StartsWith("FREIO", StringComparison.OrdinalIgnoreCase) OrElse
                   e.Snapshot.LastAction.StartsWith("HANDBRAKE", StringComparison.OrdinalIgnoreCase),
                   Color.IndianRed,
                   Color.Gold))
        End If
        RenderSpotBotSimulationSummary(e.Snapshot)
    End Sub

    Private Sub RenderSpotBotSimulationSummary(snapshot As SpotBotSimulationSnapshot)
        If snapshot Is Nothing OrElse _botSimulationSummary Is Nothing Then Return

        Dim pnlColor = If(snapshot.TotalPnl >= 0D, Color.LimeGreen, Color.IndianRed)
        _botSimulationSummary.ForeColor = pnlColor
        _botSimulationSummary.Text =
            $"Simulação | Patrimônio: {snapshot.Equity:0.00} USDT | " &
            $"P/L: {snapshot.TotalPnl:+0.00;-0.00;0.00} " &
            $"(realizado {snapshot.RealizedPnl:+0.00;-0.00;0.00} / " &
            $"aberto {snapshot.UnrealizedPnl:+0.00;-0.00;0.00}) | " &
            $"Trades: {snapshot.CompletedTrades} | " &
            $"Acerto: {snapshot.WinRatePercent:0.0}% | " &
            $"Abertas: {snapshot.OpenPositions} | " &
            $"Freio: {GetRiskBrakeDisplayText(snapshot)} | " &
            $"Bloq.: {snapshot.RiskBrakeBlockedEntries} | " &
            $"Evitado est.: {snapshot.RiskBrakeEstimatedAvoidedPnl:+0.00;-0.00;0.00} USDT | " &
            $"Pausas: {snapshot.RiskBrakePauseCount}"
    End Sub

    Private Shared Function GetRiskBrakeDisplayText(snapshot As SpotBotSimulationSnapshot) As String
        If snapshot Is Nothing Then Return "-"
        Select Case snapshot.RiskBrakeState
            Case SpotBotRiskBrakeState.HardStop
                Return "HANDBRAKE"
            Case SpotBotRiskBrakeState.Paused
                Return $"PAUSA ({snapshot.RiskBrakeConsecutiveLosses})"
            Case SpotBotRiskBrakeState.ProfitLockArmed
                Return "TRAVA ARMADA"
            Case Else
                Return "normal"
        End Select
    End Function

    Private Sub UpdateSpotBotPositionsGrid(positions As IReadOnlyList(Of SpotBotOpenPosition))
        If _dgSpotBotPositions Is Nothing Then Return

        Dim safePositions = If(positions, Array.Empty(Of SpotBotOpenPosition)())
        Dim rebuild = _dgSpotBotPositions.Rows.Count <> safePositions.Count

        If Not rebuild Then
            For index = 0 To safePositions.Count - 1
                Dim existingSymbol = _dgSpotBotPositions.Rows(index).Cells("Symbol").Value?.ToString()
                If Not String.Equals(existingSymbol, safePositions(index).Symbol, StringComparison.OrdinalIgnoreCase) Then
                    rebuild = True
                    Exit For
                End If
            Next
        End If

        If rebuild Then
            _dgSpotBotPositions.SuspendLayout()
            Try
                _dgSpotBotPositions.Rows.Clear()
                For Each position In safePositions
                    _dgSpotBotPositions.Rows.Add()
                Next
            Finally
                _dgSpotBotPositions.ResumeLayout()
            End Try
        End If

        For index = 0 To safePositions.Count - 1
            Dim position = safePositions(index)
            Dim row = _dgSpotBotPositions.Rows(index)
            row.Cells("Symbol").Value = position.Symbol
            row.Cells("Strategy").Value = If(String.IsNullOrWhiteSpace(position.Strategy), "-", position.Strategy)
            row.Cells("Side").Value = position.Side
            row.Cells("Quantity").Value = position.Quantity.ToString("0.########", CultureInfo.InvariantCulture)
            row.Cells("Notional").Value = FormatSpotBotUsd(position.Notional, 2)
            row.Cells("EntryPrice").Value = FormatSpotBotUsd(position.EntryPrice, 4)
            row.Cells("CurrentPrice").Value = FormatSpotBotUsd(position.CurrentPrice, 4)
            row.Cells("StopPrice").Value = If(position.StopPrice > 0D, FormatSpotBotUsd(position.StopPrice, 6), "-")
            row.Cells("UnrealizedPnl").Value = FormatSpotBotUsd(position.UnrealizedPnl, 2)
            row.Cells("RoiPercent").Value = position.RoiPercent.ToString("0.00;-0.00;0.00", CultureInfo.InvariantCulture) & "%"
            row.Cells("GridLevelCount").Value = If(position.GridLevelCount > 0, position.GridLevelCount.ToString(CultureInfo.InvariantCulture), "-")
            ApplySpotBotPositionRowStyle(row, position.UnrealizedPnl)
        Next
    End Sub

    Private Shared Function FormatSpotBotUsd(value As Decimal, decimals As Integer) As String
        Dim format = "0." & New String("0"c, Math.Max(0, decimals))
        If value < 0D Then
            Return "-$" & Math.Abs(value).ToString(format, CultureInfo.InvariantCulture)
        End If
        Return "$" & value.ToString(format, CultureInfo.InvariantCulture)
    End Function

    Private Shared Sub ApplySpotBotPositionRowStyle(row As DataGridViewRow, unrealizedPnl As Decimal)
        Dim positive = unrealizedPnl >= 0D
        Dim rowBackColor = If(positive, Color.FromArgb(0, 25, 0), Color.FromArgb(25, 0, 0))
        Dim fontColor = Color.White
        Dim pnlColor = If(positive, Color.LimeGreen, Color.IndianRed)

        row.DefaultCellStyle.BackColor = rowBackColor
        row.DefaultCellStyle.ForeColor = fontColor
        row.DefaultCellStyle.SelectionBackColor = rowBackColor
        row.DefaultCellStyle.SelectionForeColor = fontColor
        row.Cells("Symbol").Style.ForeColor = pnlColor
        If row.DataGridView.Columns.Contains("Strategy") Then
            row.Cells("Strategy").Style.ForeColor = Color.Gold
        End If
        row.Cells("Side").Style.ForeColor = Color.LimeGreen
        row.Cells("EntryPrice").Style.ForeColor = Color.Cyan
        row.Cells("CurrentPrice").Style.ForeColor = pnlColor
        If row.DataGridView.Columns.Contains("StopPrice") Then
            row.Cells("StopPrice").Style.ForeColor = Color.DarkOrange
        End If
        row.Cells("UnrealizedPnl").Style.ForeColor = pnlColor
        row.Cells("RoiPercent").Style.ForeColor = pnlColor
        If row.DataGridView.Columns.Contains("ClosePosition") Then
            row.Cells("ClosePosition").Style.BackColor = Color.IndianRed
            row.Cells("ClosePosition").Style.ForeColor = Color.White
            row.Cells("ClosePosition").Style.SelectionBackColor = Color.IndianRed
            row.Cells("ClosePosition").Style.SelectionForeColor = Color.White
        End If
        RemoveCellSelectionHighlight(row)
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

            EnsureBinanceCredentials()

            CheckCatalogsOnStartup()

            AddHandler _binanceWs.PriceUpdated, AddressOf BinanceWs_PriceUpdated
            AddHandler _binanceWs.VolumeUpdated, AddressOf BinanceWs_VolumeUpdated
            AddHandler _binanceWs.CandleUpdated, AddressOf BinanceWs_CandleUpdated

            AddHandler _binanceWs.ConnectionStateChanged, AddressOf BinanceWs_ConnectionStateChanged
            AddHandler _gateWs.PriceUpdated, AddressOf GateWs_PriceUpdated
            AddHandler _gateWs.ConnectionStateChanged, AddressOf GateWs_ConnectionStateChanged
            AddHandler _binanceFuturesMarketWs.MarkPriceUpdated, AddressOf BinanceFuturesMarkPriceUpdated
            AddHandler _binanceFuturesMarketWs.ConnectionStateChanged, AddressOf BinanceFuturesWs_ConnectionStateChanged
            AddHandler _gateFuturesMarketWs.MarkPriceUpdated, AddressOf GateFuturesMarkPriceUpdated
            AddHandler _gateFuturesMarketWs.ConnectionStateChanged, AddressOf GateFuturesWs_ConnectionStateChanged
            AddHandler _binanceFuturesUserWs.DataUpdated, AddressOf BinanceFuturesUserDataUpdated
            AddHandler _binanceFuturesUserWs.ConnectionStateChanged, AddressOf BinanceFuturesWs_ConnectionStateChanged
            AddHandler _binanceSpotUserWs.DataUpdated, AddressOf BinanceSpotUserDataUpdated
            AddHandler _binanceSpotUserWs.ExecutionReportReceived, AddressOf BinanceSpotExecutionReportReceived
            AddHandler _binanceSpotUserWs.ConnectionStateChanged, AddressOf BinanceSpotWs_ConnectionStateChanged

            Await B.SyncBinanceTime()

            Await OfferBinanceCatalogRegistrationAsync()

            chart.removeCharts()

            lbDebug.Clear()
            lbDebug.AppendText("Status: Pronto")

            changeOnOffColor("Pronto")

        Catch ex As Exception
            lbDebug.Clear()
            lbDebug.AppendText("Erro ao carregar o portfólio: " & ex.Message)
        End Try

    End Sub

    Private Sub EnsureBinanceCredentials()
        If Not String.IsNullOrWhiteSpace(ExchangeCredentials.BinanceAPIKey) AndAlso
           Not String.IsNullOrWhiteSpace(ExchangeCredentials.BinanceSecretKey) Then
            Return
        End If

        Using credentialsForm As New FormExchangeCredentials()
            credentialsForm.ShowDialog(Me)
        End Using
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

        If _cbSpotOrderExchange IsNot Nothing AndAlso
           String.Equals(_cbSpotOrderExchange.Text.Trim(), "GATE.IO", StringComparison.OrdinalIgnoreCase) Then
            Dim selectedSymbol = GetSpotBaseAsset(_cbSpotOrderSymbol?.Text)
            If Not String.IsNullOrWhiteSpace(selectedSymbol) AndAlso Not Cjson.stablecoins.Contains(selectedSymbol) Then
                symbols.Add(selectedSymbol)
            End If
        End If

        If _dgOrdensSpot IsNot Nothing AndAlso _dgOrdensSpot.Columns.Contains("Symbol") AndAlso
           _dgOrdensSpot.Columns.Contains("Exchange") Then
            For Each orderRow As DataGridViewRow In _dgOrdensSpot.Rows
                If orderRow.IsNewRow Then Continue For
                Dim exchange = orderRow.Cells("Exchange").Value?.ToString().Trim().ToUpperInvariant()
                If exchange <> "GATE.IO" Then Continue For
                Dim orderSymbol = GetSpotBaseAsset(orderRow.Cells("Symbol").Value?.ToString())
                If Not String.IsNullOrWhiteSpace(orderSymbol) AndAlso Not Cjson.stablecoins.Contains(orderSymbol) Then
                    symbols.Add(orderSymbol)
                End If
            Next
        End If

        For Each dcaPlan In _spotDca.GetPlans()
            If dcaPlan.Enabled AndAlso
               String.Equals(dcaPlan.Exchange, "GATE.IO", StringComparison.OrdinalIgnoreCase) AndAlso
               Not String.IsNullOrWhiteSpace(dcaPlan.Symbol) Then
                symbols.Add(dcaPlan.Symbol.Trim().ToUpperInvariant())
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
                    UpdateSpotOrdersCurrentPrice(symbol, price, "GATE.IO")
                    UpdateSpotOrderCurrentPrice()
                    _spotDca.ProcessPrice(symbol, price, exchange:="GATE.IO")
                End Sub))

            Return

        End If

        UpdateGateRow(symbol, price)
        UpdateSpotOrdersCurrentPrice(symbol, price, "GATE.IO")
        UpdateSpotOrderCurrentPrice()
        _spotDca.ProcessPrice(symbol, price, exchange:="GATE.IO")

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

            For Each botSymbol In _spotBotUniverseSymbols
                If Not String.IsNullOrWhiteSpace(botSymbol) Then
                    symbols.Add(botSymbol.Trim().ToUpperInvariant())
                End If
            Next

            For Each dcaPlan In _spotDca.GetPlans()
                If dcaPlan.Enabled AndAlso
                   Not String.Equals(dcaPlan.Exchange, "GATE.IO", StringComparison.OrdinalIgnoreCase) AndAlso
                   Not String.IsNullOrWhiteSpace(dcaPlan.Symbol) Then
                    symbols.Add(dcaPlan.Symbol.Trim().ToUpperInvariant())
                End If
            Next

            If _spotTradeBot.IsRunning AndAlso
               (_spotTradeBot.Configuration.BitcoinFlushProtectionEnabled OrElse
                _spotTradeBot.Configuration.BitcoinDailyProtectionEnabled OrElse
                _spotTradeBot.Configuration.GridV2BtcMildDeclineEnabled) Then
                symbols.Add("BTC")
            End If

            symbols =
            symbols.Distinct().ToList()

            If symbols.Count = 0 Then

                Debug.WriteLine(
                "Nenhum ativo Binance encontrado no DataGrid.")

                Return

            End If

            Dim candleIntervals = _spotDca.GetRequiredCandleIntervals("BINANCE").ToList()
            If _spotTradeBot.IsRunning AndAlso
               _spotTradeBot.Configuration.BitcoinFlushProtectionEnabled AndAlso
               Not candleIntervals.Contains("1m", StringComparer.OrdinalIgnoreCase) Then
                candleIntervals.Add("1m")
            End If
            If _spotTradeBot.IsRunning AndAlso
               _spotTradeBot.Configuration.BitcoinDailyProtectionEnabled AndAlso
               Not candleIntervals.Contains("1d", StringComparer.OrdinalIgnoreCase) Then
                candleIntervals.Add("1d")
            End If
            If _spotTradeBot.IsRunning AndAlso
               _spotTradeBot.Configuration.GridV2BtcMildDeclineEnabled AndAlso
               Not candleIntervals.Contains("1d", StringComparer.OrdinalIgnoreCase) Then
                candleIntervals.Add("1d")
            End If

            Await _binanceWs.StartAsync(symbols, candleIntervals.Distinct(StringComparer.OrdinalIgnoreCase))

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

        RemoveCellSelectionHighlight(row)
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

                If Not dgPortfolio.Columns.Contains("Cripto") OrElse
                   Not dgPortfolio.Columns.Contains("Wallet") OrElse
                   Not dgPortfolio.Columns.Contains("vlEntradaUSD") OrElse
                   Not dgPortfolio.Columns.Contains("vlAtualUSD") Then
                    Continue For
                End If

                Dim wallet As String =
                row.Cells("Wallet").Value?.
                ToString().
                Trim().
                ToUpperInvariant()

                Dim entrada As Decimal
                If Not TryReadFuturesDecimal(row.Cells("vlEntradaUSD").Value, entrada) Then
                    Continue For
                End If

                Dim atual As Decimal
                If Not TryReadFuturesDecimal(row.Cells("vlAtualUSD").Value, atual) Then
                    Continue For
                End If

                Dim symbol As String =
                row.Cells("Cripto").Value?.
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
            $"{percentualInvestido:F0}%"

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
                End Sub))

            Return

        End If

        UpdateBinanceRow(symbol, price)
        UpdateSpotOrdersCurrentPrice(symbol, price)
        UpdateSpotOrderCurrentPrice()

    End Sub

    Private Sub BinanceWs_VolumeUpdated(symbol As String, price As Decimal, quoteVolume24h As Decimal)
        If Me.InvokeRequired Then
            Me.BeginInvoke(New Action(Of String, Decimal, Decimal)(AddressOf BinanceWs_VolumeUpdated), symbol, price, quoteVolume24h)
            Return
        End If

        ProcessSpotBotPrice(symbol, price, quoteVolume24h)
        _spotDca.ProcessPrice(symbol, price)
    End Sub

    Private Sub BinanceWs_CandleUpdated(
        symbol As String,
        interval As String,
        candleStartUtc As DateTime,
        openPrice As Decimal,
        highPrice As Decimal,
        lowPrice As Decimal,
        closePrice As Decimal,
        volume As Decimal,
        isClosed As Boolean)

        _spotDca.ProcessCandle(
            symbol,
            interval,
            candleStartUtc,
            openPrice,
            highPrice,
            lowPrice,
            closePrice,
            volume,
            isClosed)

        If String.Equals(symbol, "BTC", StringComparison.OrdinalIgnoreCase) Then
            Dim protection = _spotTradeBot.ProcessBitcoinCandle(
                interval,
                candleStartUtc,
                openPrice,
                highPrice,
                lowPrice,
                closePrice,
                volume,
                isClosed)
            If protection IsNot Nothing AndAlso protection.NewFlushDetected AndAlso
               Me.IsHandleCreated Then
                Dim protectionMessage = protection.Reason
                Me.BeginInvoke(
                    New Action(
                        Sub()
                            AppendSpotBotLog(protectionMessage, Color.OrangeRed)
                        End Sub))
            End If
        End If
    End Sub

    Private Sub ProcessSpotBotPrice(symbol As String, price As Decimal, Optional quoteVolume24h As Decimal = 0D)
        If Not _spotTradeBot.IsRunning Then Return
        If Not IsSpotBotUniverseSymbol(symbol) Then Return
        _spotTradeBot.ProcessPrice(symbol, price, quoteVolume24h:=quoteVolume24h)
    End Sub

    Private Function IsSpotBotUniverseSymbol(symbol As String) As Boolean
        Dim baseSymbol = GetSpotBaseAsset(symbol)
        If String.IsNullOrWhiteSpace(baseSymbol) Then Return False
        Return _spotBotUniverseSymbols.Any(
            Function(item) String.Equals(
                GetSpotBaseAsset(item),
                baseSymbol,
                StringComparison.OrdinalIgnoreCase))
    End Function

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
            ArrangeDcaButton()
            If _panelSpotBot IsNot Nothing AndAlso _panelSpotBot.Visible Then
                ArrangeSpotBotPanel(True)
            End If
            If _panelDca IsNot Nothing AndAlso _panelDca.Visible Then
                ArrangeDcaPanel(True)
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
                .SelectionBackColor = Color.FromArgb(64, 64, 64),
                .SelectionForeColor = Color.Gold
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
                row.Cells("SpotExitPrice").Style.SelectionBackColor = Color.FromArgb(64, 64, 64)
                row.Cells("SpotExitPrice").Style.SelectionForeColor = Color.Gold
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

        If PanelProfits IsNot Nothing AndAlso
           PanelGraphs IsNot Nothing AndAlso
           panelDebug IsNot Nothing Then
            Dim contentBottom = Me.ClientSize.Height - panelDebug.Height
            Dim minimumProfitTop = PanelGraphs.Top + 265
            Dim profitTop = Math.Max(minimumProfitTop, contentBottom - PanelProfits.Height)
            PanelProfits.Location = New Point(0, profitTop)
            PanelGraphs.Height = Math.Max(265, profitTop - PanelGraphs.Top)
            CenterPanelGraphsItems()
        End If

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

    Private Sub CenterPanelGraphsItems()
        If PanelGraphs Is Nothing OrElse PanelGraphs.Controls.Count = 0 Then
            Return
        End If

        ' Os gráficos e os indicadores são adicionados em momentos diferentes.
        ' Calculamos o retângulo de todos os controles visíveis e deslocamos o
        ' conjunto, preservando o espaçamento horizontal e vertical entre eles.
        Dim items As New List(Of Control)()
        Dim top As Integer = Integer.MaxValue
        Dim bottom As Integer = Integer.MinValue

        For Each item As Control In PanelGraphs.Controls
            If Not item.Visible OrElse item.Dock <> DockStyle.None Then
                Continue For
            End If

            items.Add(item)
            top = Math.Min(top, item.Top)
            bottom = Math.Max(bottom, item.Bottom)
        Next

        If items.Count = 0 Then
            Return
        End If

        Dim contentHeight = bottom - top
        Dim offset = ((PanelGraphs.ClientSize.Height - contentHeight) \ 2) - top
        If offset = 0 Then
            Return
        End If

        For Each item As Control In items
            item.Top += offset
        Next
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

        gCriptos.collumGraph(400, 230, 100, 400, "Ativos", "Portfólio", 10, Color.Aqua, Color.FromArgb(30, 30, 30), SeriesChartType.Column, criptoDic, PanelGraphs)
    End Sub

    Public Sub addressGraph(criptoDic As Dictionary(Of String, Decimal))
        Dim gCriptos As New Charts

        gCriptos.pieGraph(330, 230, 100, 800, "Custódia", 10, Color.Aqua, Color.FromArgb(30, 30, 30), criptoDic, 7.5, Color.White, PanelGraphs)
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

    Private Sub PortfolioToolStripMenuItem_Click(sender As Object, e As EventArgs)
        Dim filePath = Application.StartupPath & "\JSON\portfolio.json"
        OpenFileDialog1.Filter = "json Files (*.json)|*.json"
        OpenFileDialog1.FileName = "portfolio.json"

        If OpenFileDialog1.ShowDialog = System.Windows.Forms.DialogResult.OK Then
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

    Private Sub ImportarToolStripMenuItem1_Click(sender As Object, e As EventArgs)
        Dim filePath = Application.StartupPath & "\JSON\wallets.json"
        OpenFileDialog1.Filter = "json Files (*.json)|*.json"
        OpenFileDialog1.FileName = "wallets.json"

        If OpenFileDialog1.ShowDialog = System.Windows.Forms.DialogResult.OK Then
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
    Private Sub ImportarToolStripMenuItem2_Click(sender As Object, e As EventArgs)
        Dim filePath = Application.StartupPath & "\JSON\criptos.json"
        OpenFileDialog1.Filter = "json Files (*.json)|*.json"
        OpenFileDialog1.FileName = "criptos.json"

        If OpenFileDialog1.ShowDialog = System.Windows.Forms.DialogResult.OK Then
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

    Private Sub WalletsExchangeToolStripMenuItem_Click(sender As Object, e As EventArgs)
        Dim filePath = Application.StartupPath & "\JSON\portfolio.json"

        SaveFileDialog1.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
        SaveFileDialog1.FileName = "portfolio.json"

        If SaveFileDialog1.ShowDialog = System.Windows.Forms.DialogResult.OK Then
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

    Private Sub ExportarToolStripMenuItem1_Click(sender As Object, e As EventArgs)
        Dim filePath = Application.StartupPath & "\JSON\wallets.json"

        SaveFileDialog1.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
        SaveFileDialog1.FileName = "wallets.json"

        If SaveFileDialog1.ShowDialog = System.Windows.Forms.DialogResult.OK Then
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

    Private Sub ExportarToolStripMenuItem2_Click(sender As Object, e As EventArgs)
        Dim filePath = Application.StartupPath & "\JSON\criptos.json"

        SaveFileDialog1.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
        SaveFileDialog1.FileName = "criptos.json"

        If SaveFileDialog1.ShowDialog = System.Windows.Forms.DialogResult.OK Then
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
                e.Value = valor.ToString("C2", CultureInfo.GetCultureInfo("en-US"))
                e.CellStyle.ForeColor = Color.WhiteSmoke
                e.CellStyle.SelectionForeColor = Color.WhiteSmoke
                e.FormattingApplied = True
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
                e.FormattingApplied = True
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

        If wallet <> "BINANCE" AndAlso wallet <> "GATE.IO" Then
            MessageBox.Show("O fechamento Spot está disponível somente para posições da Binance ou Gate.", "Fechar posição", MessageBoxButtons.OK, MessageBoxIcon.Information)
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
            Dim orderResponse = If(wallet = "GATE.IO",
                                   Await _gateSpot.GATE_CreateSpotOrderAsync(tradingSymbol, "SELL", "LIMIT", quantity, limitPrice),
                                   Await B.BINANCE_CreateSpotOrderAsync(tradingSymbol, "SELL", "LIMIT", quantity, limitPrice))
            row.Cells("SpotExitPrice").Value = String.Empty
            Await RefreshSpotPortfolioAfterFilledOrderAsync(orderResponse, wallet, limitPrice)
            Await LoadSpotOrdersAsync()
            MessageBox.Show($"Ordem de fechamento enviada para {wallet}.", "Fechar posição", MessageBoxButtons.OK, MessageBoxIcon.Information)
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
            Dim positions As New List(Of BinanceFuturesPosition)()
            Dim protectionOrders As New List(Of BinanceFuturesProtectionOrder)()
            Dim futuresAccount As New BinanceFuturesAccountSummary()
            Dim accountLoaded = False

            If UseBinanceFutures Then
                Try
                    Dim binancePositions = Await B.BINANCE_GetFuturesPositionsAsync()
                    For Each position In binancePositions
                        position.Exchange = "BINANCE"
                        positions.Add(position)
                    Next

                    Dim binanceAccount = Await B.BINANCE_GetFuturesAccountSummaryAsync()
                    futuresAccount.AvailableBalance += binanceAccount.AvailableBalance
                    futuresAccount.InitialMargin += binanceAccount.InitialMargin
                    futuresAccount.WalletBalance += binanceAccount.WalletBalance
                    futuresAccount.Equity += binanceAccount.Equity
                    futuresAccount.UnrealizedProfit += binanceAccount.UnrealizedProfit
                    accountLoaded = True

                    Dim binanceProtectionOrders = Await B.BINANCE_GetFuturesProtectionOrdersAsync()
                    For Each protection In binanceProtectionOrders
                        protection.Exchange = "BINANCE"
                        protectionOrders.Add(protection)
                    Next
                Catch ex As Exception
                    AppendFuturesProviderError("Binance", ex)
                End Try
            End If

            If UseGateFutures Then
                Try
                    Dim gatePositions = Await _gateFutures.GATE_GetFuturesPositionsAsync()
                    For Each position In gatePositions
                        position.Exchange = "GATE"
                        positions.Add(position)
                    Next

                    Dim gateAccount = Await _gateFutures.GATE_GetFuturesAccountSummaryAsync()
                    futuresAccount.AvailableBalance += gateAccount.AvailableBalance
                    futuresAccount.InitialMargin += gateAccount.InitialMargin
                    futuresAccount.WalletBalance += gateAccount.WalletBalance
                    futuresAccount.Equity += gateAccount.Equity
                    futuresAccount.UnrealizedProfit += gateAccount.UnrealizedProfit
                    accountLoaded = True

                    Dim gateProtectionOrders = Await _gateFutures.GATE_GetFuturesProtectionOrdersAsync(gatePositions)
                    For Each protection In gateProtectionOrders
                        protection.Exchange = "GATE"
                        protectionOrders.Add(protection)
                    Next
                Catch ex As Exception
                    AppendFuturesProviderError("Gate", ex)
                End Try
            End If

            If Not accountLoaded AndAlso positions.Count = 0 Then
                Throw New InvalidOperationException("Nenhuma conta de futuros está disponível. Configure as credenciais da Binance ou da Gate.")
            End If

            Await EnsureFuturesUsdBrlRateAsync()
            Dim table As New DataTable()

            table.Columns.Add("Symbol", GetType(String))
            table.Columns.Add("Exchange", GetType(String))
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
                    NormalizeFuturesExchange(position.Exchange),
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

            _futuresEquityBaseUsd = futuresAccount.WalletBalance
            _futuresEquityLoaded = accountLoaded
            _futuresAvailableUsd = futuresAccount.AvailableBalance
            UpdateFuturesOrderAvailableLabel()

            UpdateFuturesPnlSummary()

            If Not UseBinanceFutures AndAlso _futuresUserStreamStarted Then
                Await _binanceFuturesUserWs.StopAsync()
                _futuresUserStreamStarted = False
            ElseIf UseBinanceFutures AndAlso Not _futuresUserStreamStarted Then
                Await _binanceFuturesUserWs.StartAsync()
                _futuresUserStreamStarted = True
            End If

            If UseGateFutures Then
                Dim activeProviders = New List(Of String)()
                If UseBinanceFutures Then activeProviders.Add("Binance")
                If UseGateFutures Then activeProviders.Add("Gate")
                Debug.WriteLine($"Futures ativos: {String.Join(" + ", activeProviders)}; posições via API e preços via WebSocket.")
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
            Dim orders As New List(Of BinanceSpotOrder)()
            Dim accountLoaded = False
            If Not String.IsNullOrWhiteSpace(ExchangeCredentials.BinanceAPIKey) AndAlso
               Not String.IsNullOrWhiteSpace(ExchangeCredentials.BinanceSecretKey) Then
                Try
                    Dim binanceOrders = Await B.BINANCE_GetSpotOpenOrdersAsync()
                    For Each order In binanceOrders
                        order.Exchange = "BINANCE"
                        orders.Add(order)
                    Next
                    accountLoaded = True
                Catch ex As Exception
                    Debug.WriteLine("Erro ao carregar ordens Spot Binance: " & ex.Message)
                End Try
            End If
            If Not String.IsNullOrWhiteSpace(ExchangeCredentials.GateAPIKey) AndAlso
               Not String.IsNullOrWhiteSpace(ExchangeCredentials.GateSecretKey) Then
                Try
                    Dim gateOrders = Await _gateSpot.GATE_GetSpotOpenOrdersAsync()
                    For Each order In gateOrders
                        order.Exchange = "GATE.IO"
                        orders.Add(order)
                    Next
                    accountLoaded = True
                Catch ex As Exception
                    Debug.WriteLine("Erro ao carregar ordens Spot Gate: " & ex.Message)
                End Try
            End If
            If Not accountLoaded Then
                Throw New InvalidOperationException("Nenhuma conta Spot está disponível. Configure as credenciais da Binance ou da Gate.")
            End If
            Dim table As New DataTable()

            table.Columns.Add("Symbol", GetType(String))
            table.Columns.Add("Exchange", GetType(String))
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
                    If(String.IsNullOrWhiteSpace(order.Exchange), "BINANCE", order.Exchange),
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
            Await StartGateWebSocket()
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
        Dim binanceSymbols As New List(Of String)()
        Dim gateSymbols As New List(Of String)()

        Dim addSymbol = Sub(target As List(Of String), rawSymbol As String)
                            Dim normalized = ToFuturesUsdtSymbol(rawSymbol)
                            If Not String.IsNullOrWhiteSpace(normalized) AndAlso
                               Not target.Contains(normalized, StringComparer.OrdinalIgnoreCase) Then
                                target.Add(normalized)
                            End If
                        End Sub

        Dim addRowSymbol = Sub(row As DataGridViewRow)
                               If row Is Nothing OrElse row.IsNewRow Then Return
                               Dim exchange = GetRowFuturesExchange(row)

                               If exchange = "GATE" Then
                                   addSymbol(gateSymbols, row.Cells("Symbol").Value?.ToString())
                               Else
                                   addSymbol(binanceSymbols, row.Cells("Symbol").Value?.ToString())
                               End If
                           End Sub

        For Each row As DataGridViewRow In dgFuturos.Rows
            addRowSymbol(row)
        Next

        If _dgOrdensFuturos IsNot Nothing AndAlso
           _dgOrdensFuturos.Columns.Contains("Symbol") Then
            For Each orderRow As DataGridViewRow In _dgOrdensFuturos.Rows
                addRowSymbol(orderRow)
            Next
        End If

        If _cbFuturesOrderSymbol IsNot Nothing Then
            Dim selectedSymbol = ToFuturesUsdtSymbol(_cbFuturesOrderSymbol.Text)
            If Not String.IsNullOrWhiteSpace(selectedSymbol) Then
                If UseBinanceFutures Then addSymbol(binanceSymbols, selectedSymbol)
                If UseGateFutures Then addSymbol(gateSymbols, selectedSymbol)
            End If
        End If

        If binanceSymbols.Count = 0 AndAlso gateSymbols.Count = 0 Then
            Await _binanceFuturesMarketWs.StopAsync()
            Await _gateFuturesMarketWs.StopAsync()
            _futuresMarketSymbolsKey = String.Empty
            _futuresMarketProviderKey = String.Empty
            Return
        End If

        Dim symbolsKey = "BINANCE:" & String.Join(
            "|",
            binanceSymbols.OrderBy(Function(symbol) symbol, StringComparer.OrdinalIgnoreCase)) &
            "|GATE:" & String.Join(
                "|",
                gateSymbols.OrderBy(Function(symbol) symbol, StringComparer.OrdinalIgnoreCase))
        Dim providerKey = String.Join(
            "+",
            New String() {
                If(UseBinanceFutures, "Binance", String.Empty),
                If(UseGateFutures, "Gate", String.Empty)
            }.Where(Function(item) Not String.IsNullOrWhiteSpace(item)))
        If String.Equals(symbolsKey, _futuresMarketSymbolsKey, StringComparison.OrdinalIgnoreCase) AndAlso
           String.Equals(providerKey, _futuresMarketProviderKey, StringComparison.OrdinalIgnoreCase) Then
            Return
        End If

        If binanceSymbols.Count = 0 OrElse Not UseBinanceFutures Then
            Await _binanceFuturesMarketWs.StopAsync()
        Else
            Await _binanceFuturesMarketWs.StartAsync(binanceSymbols)
        End If

        If gateSymbols.Count = 0 OrElse Not UseGateFutures Then
            Await _gateFuturesMarketWs.StopAsync()
        Else
            Await _gateFuturesMarketWs.StartAsync(gateSymbols)
        End If

        _futuresMarketSymbolsKey = symbolsKey
        _futuresMarketProviderKey = providerKey
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

        Dim selectedExchange = GetSelectedFuturesOrderExchange()
        For Each row As DataGridViewRow In dgFuturos.Rows
            If row.IsNewRow Then Continue For
            If dgFuturos.Columns.Contains("Exchange") AndAlso
               Not String.Equals(
                   NormalizeFuturesExchange(row.Cells("Exchange").Value?.ToString()),
                   selectedExchange,
                   StringComparison.OrdinalIgnoreCase) Then
                Continue For
            End If
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
        Return TryGetFuturesCurrentPrice(symbol, String.Empty, price)
    End Function

    Private Function TryGetFuturesCurrentPrice(symbol As String, exchange As String, ByRef price As Decimal) As Boolean
        Dim normalizedSymbol = ToFuturesUsdtSymbol(symbol)
        If String.IsNullOrWhiteSpace(normalizedSymbol) Then
            Return False
        End If

        Dim normalizedExchange = If(String.IsNullOrWhiteSpace(exchange), String.Empty, NormalizeFuturesExchange(exchange))
        If normalizedExchange = "GATE" Then
            Return _gateFuturesMarketWs.TryGetPrice(normalizedSymbol, price) OrElse
                   TryGetFuturesGridPrice(normalizedSymbol, price, normalizedExchange)
        ElseIf normalizedExchange = "BINANCE" Then
            Return _binanceFuturesMarketWs.TryGetPrice(normalizedSymbol, price) OrElse
                   TryGetFuturesGridPrice(normalizedSymbol, price, normalizedExchange)
        End If

        Return (_gateFuturesMarketWs.TryGetPrice(normalizedSymbol, price) OrElse
                _binanceFuturesMarketWs.TryGetPrice(normalizedSymbol, price) OrElse
                TryGetFuturesGridPrice(normalizedSymbol, price, String.Empty))
    End Function

    Private Function TryGetFuturesGridPrice(symbol As String, ByRef price As Decimal) As Boolean
        Return TryGetFuturesGridPrice(symbol, price, String.Empty)
    End Function

    Private Function TryGetFuturesGridPrice(symbol As String, ByRef price As Decimal, exchange As String) As Boolean
        If Not dgFuturos.Columns.Contains("Symbol") OrElse
           Not dgFuturos.Columns.Contains("MarkPrice") Then
            Return False
        End If

        For Each row As DataGridViewRow In dgFuturos.Rows
            If row.IsNewRow Then Continue For
            If (String.IsNullOrWhiteSpace(exchange) OrElse
                Not dgFuturos.Columns.Contains("Exchange") OrElse
                String.Equals(
                    NormalizeFuturesExchange(row.Cells("Exchange").Value?.ToString()),
                    NormalizeFuturesExchange(exchange),
                    StringComparison.OrdinalIgnoreCase)) AndAlso
               String.Equals(
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

        Dim symbol = ToFuturesUsdtSymbol(_cbFuturesOrderSymbol.Text)
        Dim exchange = GetSelectedFuturesOrderExchange()
        Dim priceKey = If(
            String.IsNullOrWhiteSpace(symbol),
            String.Empty,
            exchange & "|" & symbol)
        Dim price As Decimal
        If TryGetFuturesCurrentPrice(
               symbol,
               exchange,
               price) Then
            _txtFuturesOrderCurrentPrice.Text = "$" & price.ToString("0.0000", CultureInfo.GetCultureInfo("en-US"))
            _futuresOrderPriceKey = priceKey
        ElseIf Not String.Equals(_futuresOrderPriceKey, priceKey, StringComparison.OrdinalIgnoreCase) Then
            _txtFuturesOrderCurrentPrice.Text = "--"
            _futuresOrderPriceKey = priceKey
        End If

        UpdateFuturesOrderLinkedValue()
    End Sub

    Private Async Function LoadFuturesOrderCurrentPriceAsync() As Task
        If _txtFuturesOrderCurrentPrice Is Nothing OrElse _cbFuturesOrderSymbol Is Nothing Then
            Return
        End If

        Dim symbol = ToFuturesUsdtSymbol(_cbFuturesOrderSymbol.Text)
        If String.IsNullOrWhiteSpace(symbol) Then
            Return
        End If

        Dim exchange = GetSelectedFuturesOrderExchange()
        Dim priceKey = exchange & "|" & symbol
        Dim currentPrice As Decimal
        If TryGetFuturesCurrentPrice(symbol, exchange, currentPrice) Then
            Return
        End If

        Try
            Dim loadedPrice = If(
                exchange = "GATE",
                Await _gateFutures.GATE_GetFuturesMarkPriceAsync(symbol),
                Await B.BINANCE_GetFuturesMarkPriceAsync(symbol))

            If loadedPrice <= 0D OrElse
               Not String.Equals(
                   ToFuturesUsdtSymbol(_cbFuturesOrderSymbol.Text),
                   symbol,
                   StringComparison.OrdinalIgnoreCase) OrElse
               Not String.Equals(
                   GetSelectedFuturesOrderExchange(),
                   exchange,
                   StringComparison.OrdinalIgnoreCase) Then
                Return
            End If

            _txtFuturesOrderCurrentPrice.Text = "$" & loadedPrice.ToString("0.0000", CultureInfo.GetCultureInfo("en-US"))
            _futuresOrderPriceKey = priceKey
            UpdateFuturesOrderLinkedValue()
        Catch ex As Exception
            Debug.WriteLine($"Erro ao carregar preço inicial de Futures ({exchange}/{symbol}): {ex.Message}")
        End Try
    End Function

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

    Private Async Sub FuturesOrderExchangeChanged(sender As Object, e As EventArgs)
        If _cbFuturesOrderExchange Is Nothing OrElse _dgOrdensFuturos Is Nothing Then
            Return
        End If

        PopulateFuturesOrderSymbols()
        UpdateFuturesOrderCurrentPrice()
        Await EnsureFuturesMarketWebSocketAsync()
        UpdateFuturesOrderCurrentPrice()
        Await LoadFuturesOrderCurrentPriceAsync()
        Await LoadFuturesOrdersAsync()
    End Sub

    Private Async Sub FuturesOrderSymbolChanged(sender As Object, e As EventArgs)
        UpdateFuturesOrderCurrentPrice()
        If Not String.IsNullOrWhiteSpace(ToFuturesUsdtSymbol(_cbFuturesOrderSymbol?.Text)) Then
            Await EnsureFuturesMarketWebSocketAsync()
            UpdateFuturesOrderCurrentPrice()
            Await LoadFuturesOrderCurrentPriceAsync()
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
            Dim selectedExchange = GetSelectedFuturesOrderExchange()
            Dim availableBalance As Decimal = 0D
            Dim accountLoaded = False
            If selectedExchange = "BINANCE" AndAlso UseBinanceFutures Then
                Try
                    Dim binanceAccount = Await B.BINANCE_GetFuturesAccountSummaryAsync()
                    availableBalance = binanceAccount.AvailableBalance
                    accountLoaded = True
                Catch ex As Exception
                    AppendFuturesProviderError("Binance", ex)
                End Try
            End If
            If selectedExchange = "GATE" AndAlso UseGateFutures Then
                Try
                    Dim gateAccount = Await _gateFutures.GATE_GetFuturesAccountSummaryAsync()
                    availableBalance = gateAccount.AvailableBalance
                    accountLoaded = True
                Catch ex As Exception
                    AppendFuturesProviderError("Gate", ex)
                End Try
            End If
            If Not accountLoaded Then
                Throw New InvalidOperationException(
                    $"A conta de futuros da exchange {selectedExchange} não está disponível. Configure as credenciais e as permissões dessa exchange.")
            End If
            _futuresAvailableUsd = availableBalance
            UpdateFuturesOrderAvailableLabel()

            Try
                If selectedExchange = "BINANCE" AndAlso UseBinanceFutures Then
                    Dim binancePositions = Await B.BINANCE_GetFuturesPositionsAsync(includeEmptyPositions:=True)
                    For Each position In binancePositions
                        If position IsNot Nothing AndAlso position.Leverage > 0 Then
                            position.Exchange = "BINANCE"
                            _futuresLeverageBySymbol(FuturesProviderKey("BINANCE", position.Symbol)) = position.Leverage
                        End If
                    Next
                End If
                If selectedExchange = "GATE" AndAlso UseGateFutures Then
                    Dim gatePositions = Await _gateFutures.GATE_GetFuturesPositionsAsync(includeEmptyPositions:=True)
                    For Each position In gatePositions
                        If position IsNot Nothing AndAlso position.Leverage > 0 Then
                            position.Exchange = "GATE"
                            _futuresLeverageBySymbol(FuturesProviderKey("GATE", position.Symbol)) = position.Leverage
                        End If
                    Next
                End If
            Catch ex As Exception
                Debug.WriteLine("Erro ao carregar alavancagem das posições Futures: " & ex.Message)
            End Try

            Dim orders As New List(Of BinanceFuturesOrder)()
            If selectedExchange = "BINANCE" AndAlso UseBinanceFutures Then
                Try
                    Dim binanceOrders = Await B.BINANCE_GetFuturesOpenOrdersAsync()
                    For Each order In binanceOrders
                        order.Exchange = "BINANCE"
                        orders.Add(order)
                    Next
                Catch ex As Exception
                    AppendFuturesProviderError("Binance", ex)
                End Try
            End If
            If selectedExchange = "GATE" AndAlso UseGateFutures Then
                Try
                    Dim gateOrders = Await _gateFutures.GATE_GetFuturesOpenOrdersAsync()
                    For Each order In gateOrders
                        order.Exchange = "GATE"
                        orders.Add(order)
                    Next
                Catch ex As Exception
                    AppendFuturesProviderError("Gate", ex)
                End Try
            End If
            Dim table As New DataTable()
            table.Columns.Add("Symbol", GetType(String))
            table.Columns.Add("Exchange", GetType(String))
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
                TryGetFuturesCurrentPrice(order.Symbol, order.Exchange, currentPrice)
                Dim orderValue = order.Quantity * order.Price
                Dim orderLeverage As Integer = 0
                Dim normalizedOrderSymbol = ToFuturesUsdtSymbol(order.Symbol)
                If Not _futuresLeverageBySymbol.TryGetValue(FuturesProviderKey(order.Exchange, normalizedOrderSymbol), orderLeverage) AndAlso
                   _cbFuturesOrderSymbol IsNot Nothing AndAlso
                   String.Equals(normalizedOrderSymbol, ToFuturesUsdtSymbol(_cbFuturesOrderSymbol.Text), StringComparison.OrdinalIgnoreCase) Then
                    Integer.TryParse(_cbFuturesOrderLeverage?.Text.Replace("x", String.Empty).Trim(), orderLeverage)
                End If

                table.Rows.Add(
                    order.Symbol,
                    NormalizeFuturesExchange(order.Exchange),
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

            Dim protectionOrders As New List(Of BinanceFuturesProtectionOrder)()
            If selectedExchange = "BINANCE" AndAlso UseBinanceFutures Then
                Try
                    Dim binanceProtectionOrders = Await B.BINANCE_GetFuturesProtectionOrdersAsync()
                    For Each protection In binanceProtectionOrders
                        protection.Exchange = "BINANCE"
                        protectionOrders.Add(protection)
                    Next
                Catch ex As Exception
                    AppendFuturesProviderError("Binance", ex)
                End Try
            End If
            If selectedExchange = "GATE" AndAlso UseGateFutures Then
                Try
                    Dim gateProtectionOrders = Await _gateFutures.GATE_GetFuturesProtectionOrdersAsync()
                    For Each protection In gateProtectionOrders
                        protection.Exchange = "GATE"
                        protectionOrders.Add(protection)
                    Next
                Catch ex As Exception
                    AppendFuturesProviderError("Gate", ex)
                End Try
            End If
            For Each protection In protectionOrders
                Dim currentPrice As Decimal = 0D
                TryGetFuturesCurrentPrice(protection.Symbol, protection.Exchange, currentPrice)

                Dim normalizedProtectionSymbol = ToFuturesUsdtSymbol(protection.Symbol)
                Dim protectionLeverage As Integer = 0
                _futuresLeverageBySymbol.TryGetValue(FuturesProviderKey(protection.Exchange, normalizedProtectionSymbol), protectionLeverage)

                Dim protectionPrice = If(
                    protection.Price > 0D,
                    protection.Price,
                    protection.StopPrice)
                Dim protectionValue = protection.Quantity * protectionPrice

                table.Rows.Add(
                    protection.Symbol,
                    NormalizeFuturesExchange(protection.Exchange),
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
            Await LoadFuturesOrderCurrentPriceAsync()
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
            {"Exchange", "Exchange"},
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
                .SelectionBackColor = Color.IndianRed,
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
            Case "Exchange"
                Dim exchange = NormalizeFuturesExchange(e.Value?.ToString())
                Dim exchangeColor = If(exchange = "GATE", Color.DeepSkyBlue, Color.Gold)
                e.CellStyle.ForeColor = exchangeColor
                e.CellStyle.SelectionForeColor = exchangeColor
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
        Dim exchange = GetRowFuturesExchange(row)

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
                Dim cancelled = If(exchange = "GATE",
                                   Await _gateFutures.GATE_CancelFuturesProtectionOrderAsync(symbol, algoId),
                                   Await B.BINANCE_CancelFuturesProtectionOrderAsync(symbol, algoId))
                If cancelled Then
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
            Dim cancelled = If(exchange = "GATE",
                               Await _gateFutures.GATE_CancelFuturesOrderAsync(symbol, orderId),
                               Await B.BINANCE_CancelFuturesOrderAsync(symbol, orderId))
            If cancelled Then
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
        Dim exchange = GetSelectedFuturesOrderExchange()
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

        If Not IsFuturesExchangeConfigured(exchange) Then
            MessageBox.Show(
                $"Configure as credenciais e as permissões de Futures da exchange {exchange} antes de enviar a ordem.",
                "Ordem Futures",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning)
            Return
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
        Dim details = $"{exchange} - {positionSide} {orderType} {quantity:0.########} {symbol} a ${price:0.########}{leverageDetails}"
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
                If exchange = "GATE" Then
                    Await _gateFutures.GATE_SetFuturesLeverageAsync(symbol, leverage)
                Else
                    Await B.BINANCE_SetFuturesLeverageAsync(symbol, leverage)
                End If
                _futuresLeverageBySymbol(FuturesProviderKey(exchange, symbol)) = leverage
            End If
            If exchange = "GATE" Then
                Await _gateFutures.GATE_CreateFuturesOrderAsync(symbol, positionSide, orderType, quantity, price)
            Else
                Await B.BINANCE_CreateFuturesOrderAsync(symbol, positionSide, orderType, quantity, price)
            End If
            Await LoadFuturesOrdersAsync()
            MessageBox.Show($"Ordem enviada para {exchange}.", "Ordem Futures", MessageBoxButtons.OK, MessageBoxIcon.Information)
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

        Return _binanceWs.TryGetPrice(symbol, price) OrElse
               _gateWs.TryGetPrice(symbol, price) OrElse
               TryGetSpotGridPrice(symbol, price)
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

        _lbSpotOrderCash.Text = Cjson.USDformat(_spotOrderCashUsd)
        _lbSpotOrderCash.ForeColor = Color.Gold
    End Sub

    Public Sub ApplySpotCashFromPopup(cashUsd As Decimal)
        _spotCashUsd = Math.Max(0D, cashUsd)
        _spotOrderCashUsd = _spotCashUsd
        _spotOrderCashExchange = If(_cbSpotOrderExchange?.Text.Trim().ToUpperInvariant(), String.Empty)
        _spotCashLoadedFromAccount = True
        UpdateSpotOrderCashLabel()

        If _spotOverviewInitialized AndAlso Not _marketRefreshRunning Then
            UpdateRealtimeOverview(force:=True)
        End If
    End Sub

    Private Async Function RefreshSpotOrderCashAsync() As Task
        Dim cashUsd As Decimal = 0D
        Dim selectedCashUsd As Decimal = 0D
        Dim selectedExchange = If(_cbSpotOrderExchange?.Text.Trim().ToUpperInvariant(), "BINANCE")
        Dim accountLoaded = False

        If Not String.IsNullOrWhiteSpace(ExchangeCredentials.BinanceAPIKey) AndAlso
           Not String.IsNullOrWhiteSpace(ExchangeCredentials.BinanceSecretKey) Then
            Try
                Dim assets = Await B.BINANCE_GetSpotAssetsFull()
                Dim binanceCashUsd As Decimal = 0D
                For Each asset In assets
                    If Cjson.stablecoins.Contains(asset.Key) Then binanceCashUsd += asset.Value
                Next
                cashUsd += binanceCashUsd
                If String.Equals(selectedExchange, "BINANCE", StringComparison.OrdinalIgnoreCase) Then
                    selectedCashUsd = binanceCashUsd
                End If
                accountLoaded = True
            Catch ex As Exception
                Debug.WriteLine("Erro ao carregar Caixa Spot Binance: " & ex.Message)
            End Try
        End If

        If Not String.IsNullOrWhiteSpace(ExchangeCredentials.GateAPIKey) AndAlso
           Not String.IsNullOrWhiteSpace(ExchangeCredentials.GateSecretKey) Then
            Try
                Dim gateAssets = Await _gateSpot.GATE_GetAllSpotAssets()
                Dim gateCashUsd As Decimal = 0D
                For Each asset In gateAssets
                    If Cjson.stablecoins.Contains(asset.Key) Then gateCashUsd += asset.Value
                Next
                cashUsd += gateCashUsd
                If String.Equals(selectedExchange, "GATE.IO", StringComparison.OrdinalIgnoreCase) Then
                    selectedCashUsd = gateCashUsd
                End If
                accountLoaded = True
            Catch ex As Exception
                Debug.WriteLine("Erro ao carregar Caixa Spot Gate: " & ex.Message)
            End Try
        End If

        If Not accountLoaded Then
            _spotOrderCashUsd = 0D
            _spotOrderCashExchange = selectedExchange
            UpdateSpotOrderCashLabel()
            Return
        End If

        _spotCashUsd = cashUsd
        _spotOrderCashUsd = selectedCashUsd
        _spotOrderCashExchange = selectedExchange
        _spotCashLoadedFromAccount = True
        UpdateSpotOrderCashLabel()

        ' A abertura/atualização do painel de ordens também consulta o
        ' saldo Spot. Quando uma ordem acabou de preencher, aproveite a
        ' mesma leitura para manter a Visão Geral sincronizada com o grid.
        If _spotOverviewInitialized AndAlso Not _marketRefreshRunning Then
            UpdateRealtimeOverview(force:=True)
        End If
    End Function

    Private Async Sub SpotOrderExchangeChanged(sender As Object, e As EventArgs)
        UpdateSpotOrderCurrentPrice()
        Await EnsureSpotOrderSymbolWebSocketAsync()
        UpdateSpotOrderCurrentPrice()
        Await RefreshSpotOrderCashAsync()
    End Sub

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
           String.IsNullOrWhiteSpace(_cbSpotOrderSymbol.Text) Then
            Return
        End If

        Dim exchange = If(_cbSpotOrderExchange?.Text.Trim().ToUpperInvariant(), "BINANCE")
        If exchange = "GATE.IO" Then
            Await StartGateWebSocket()
        ElseIf Not _binanceWsStartRunning Then
            Await StartBinanceWebSocket()
        End If
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

        Dim exchange = If(_cbSpotOrderExchange?.Text.Trim().ToUpperInvariant(), "BINANCE")
        Dim hasPrice = If(exchange = "GATE.IO",
                          _gateWs.TryGetPrice(symbol, price) OrElse _binanceWs.TryGetPrice(symbol, price),
                          _binanceWs.TryGetPrice(symbol, price) OrElse _gateWs.TryGetPrice(symbol, price))

        If hasPrice OrElse TryGetSpotGridPrice(symbol, price) Then
            _txtSpotOrderCurrentPrice.Text = "$" & price.ToString("0.########", CultureInfo.GetCultureInfo("en-US"))
        Else
            _txtSpotOrderCurrentPrice.Text = "--"
        End If

        UpdateSpotOrderLinkedValue()
    End Sub

    Private Sub UpdateSpotOrdersCurrentPrice(symbol As String, price As Decimal, Optional exchange As String = "")
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
            Dim rowExchange = If(_dgOrdensSpot.Columns.Contains("Exchange"), row.Cells("Exchange").Value?.ToString().Trim(), String.Empty)
            If String.Equals(rowSymbol, baseSymbol, StringComparison.OrdinalIgnoreCase) AndAlso
               (String.IsNullOrWhiteSpace(exchange) OrElse String.Equals(rowExchange, exchange, StringComparison.OrdinalIgnoreCase)) Then
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
        Dim exchange = If(_cbSpotOrderExchange?.Text.Trim().ToUpperInvariant(), "BINANCE")
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

        Dim details = $"{exchange} - {side} {orderType} {quantity:0.########} {symbol}"
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
            Dim orderResponse As String
            If exchange = "GATE.IO" Then
                orderResponse = Await _gateSpot.GATE_CreateSpotOrderAsync(symbol, side, orderType, quantity, price)
            Else
                orderResponse = Await B.BINANCE_CreateSpotOrderAsync(symbol, side, orderType, quantity, price)
            End If
            Dim filledPriceFallback = price
            If filledPriceFallback <= 0D Then
                TryReadFuturesDecimal(_txtSpotOrderCurrentPrice.Text, filledPriceFallback)
            End If
            Await RefreshSpotPortfolioAfterFilledOrderAsync(orderResponse, exchange, filledPriceFallback)
            Await LoadSpotOrdersAsync()
            MessageBox.Show($"Ordem enviada para {exchange}.", "Ordem Spot", MessageBoxButtons.OK, MessageBoxIcon.Information)
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

    Private Shared Function IsSpotOrderFilled(orderResponse As String) As Boolean
        If String.IsNullOrWhiteSpace(orderResponse) Then
            Return False
        End If

        Try
            Dim response = JObject.Parse(orderResponse)
            Dim status = response("status")?.ToString().Trim().ToUpperInvariant()
            Return status = "FILLED" OrElse
                   status = "PARTIALLY_FILLED" OrElse
                   status = "CLOSED"
        Catch
            Return False
        End Try
    End Function

    Private Async Function RefreshSpotPortfolioAfterFilledOrderAsync(
        orderResponse As String,
        exchange As String,
        Optional fallbackPrice As Decimal = 0D) As Task
        If IsDisposed OrElse Disposing Then
            Return
        End If

        Dim filledResponse = Await WaitForSpotOrderFillAsync(orderResponse, exchange)

        Dim symbol As String = String.Empty
        Dim side As String = String.Empty
        Dim filledQuantity As Decimal = 0D
        Dim executionPrice As Decimal = 0D
        If TryReadSpotOrderFill(
                filledResponse,
                exchange,
                symbol,
                side,
                filledQuantity,
                executionPrice,
                fallbackPrice) Then
            Dim quantityToApply = ConsumeSpotOrderQuantity(filledResponse, exchange, filledQuantity)
            If quantityToApply > 0D Then
                PortfolioRepository.ApplySpotOrderFill(
                    symbol,
                    exchange,
                    side,
                    quantityToApply,
                    executionPrice,
                    executionPrice)
            End If
        End If

        ' Mesmo quando a ordem continua pendente, atualize o grid para refletir
        ' imediatamente qualquer fill confirmado durante a consulta.
        For attempt As Integer = 1 To 3
            While _marketRefreshRunning AndAlso attempt < 3
                Await Task.Delay(250)
            End While

            If Await refreshMarket() Then
                Exit For
            End If

            If attempt < 3 Then
                Await Task.Delay(250)
            End If
        Next
    End Function

    Private Async Function WaitForSpotOrderFillAsync(
        orderResponse As String,
        exchange As String) As Task(Of String)

        If IsSpotOrderFilled(orderResponse) Then Return orderResponse
        If String.IsNullOrWhiteSpace(orderResponse) Then Return orderResponse

        Try
            Dim initial = JObject.Parse(orderResponse)
            Dim orderIdText = ReadSpotOrderText(initial, "orderId", "order_id", "i", "id")
            Dim orderId As Long
            If Not Long.TryParse(orderIdText, NumberStyles.Integer, CultureInfo.InvariantCulture, orderId) OrElse orderId <= 0 Then
                Return orderResponse
            End If

            Dim symbol = GetSpotBaseAsset(ReadSpotOrderText(initial, "symbol", "s", "currency_pair").Replace("_", String.Empty))
            If String.IsNullOrWhiteSpace(symbol) Then Return orderResponse

            For attempt As Integer = 1 To 4
                Await Task.Delay(250 * attempt)
                Dim latest As String
                If String.Equals(exchange?.Trim(), "GATE.IO", StringComparison.OrdinalIgnoreCase) Then
                    latest = Await _gateSpot.GATE_GetSpotOrderAsync(symbol, orderId)
                Else
                    latest = Await B.BINANCE_GetSpotOrderAsync(ToSpotUsdtSymbol(symbol), orderId)
                End If

                If IsSpotOrderFilled(latest) Then Return latest
            Next
        Catch ex As Exception
            Debug.WriteLine("Erro consultando preenchimento da ordem Spot: " & ex.Message)
        End Try

        Return orderResponse
    End Function

    Private Shared Function TryReadSpotOrderFill(
        orderResponse As String,
        exchange As String,
        ByRef symbol As String,
        ByRef side As String,
        ByRef filledQuantity As Decimal,
        ByRef executionPrice As Decimal,
        Optional fallbackPrice As Decimal = 0D) As Boolean

        symbol = String.Empty
        side = String.Empty
        filledQuantity = 0D
        executionPrice = 0D

        If String.IsNullOrWhiteSpace(orderResponse) Then Return False

        Try
            Dim response = JObject.Parse(orderResponse)
            Dim isExecutionReport =
                String.Equals(ReadSpotOrderText(response, "e"), "executionReport", StringComparison.OrdinalIgnoreCase)
            Dim status = ReadSpotOrderText(response, "status", "X").ToUpperInvariant()
            If status <> "FILLED" AndAlso status <> "PARTIALLY_FILLED" AndAlso status <> "CLOSED" Then
                Return False
            End If

            Dim rawSymbol = ReadSpotOrderText(response, "symbol", "s", "currency_pair").Replace("_", String.Empty)
            symbol = GetSpotBaseAsset(rawSymbol)
            side = ReadSpotOrderText(response, "side", "S").ToUpperInvariant()
            If String.IsNullOrWhiteSpace(symbol) OrElse
               (side <> "BUY" AndAlso side <> "SELL") Then
                Return False
            End If

            If isExecutionReport Then
                ' O User Data Stream envia a quantidade do último fill em l.
                ' Usamos esse delta para não contabilizar duas vezes o acumulado z.
                filledQuantity = ReadSpotOrderDecimal(response, "l", "lastExecutedQty")
                executionPrice = ReadSpotOrderDecimal(response, "L", "lastExecutedPrice")
            Else
                filledQuantity = ReadSpotOrderDecimal(response, "executedQty", "filledQty", "filled_amount")
                If filledQuantity <= 0D Then
                    Dim amount = ReadSpotOrderDecimal(response, "amount", "origQty")
                    Dim remaining = ReadSpotOrderDecimal(response, "left", "remaining")
                    If amount > 0D Then filledQuantity = Math.Max(0D, amount - remaining)
                End If
            End If

            If filledQuantity <= 0D Then Return False

            Dim quoteAmount = ReadSpotOrderDecimal(
                response,
                "cummulativeQuoteQty",
                "cumulativeQuoteQty",
                "Z",
                "filled_total")
            If Not isExecutionReport AndAlso quoteAmount > 0D Then
                executionPrice = quoteAmount / filledQuantity
            ElseIf executionPrice <= 0D Then
                executionPrice = ReadSpotOrderDecimal(
                    response,
                    "price",
                    "avg_deal_price",
                    "avgPrice",
                    "average_price",
                    "filled_average_price")
            End If

            If executionPrice <= 0D Then executionPrice = Math.Max(0D, fallbackPrice)
            Return executionPrice > 0D
        Catch ex As Exception
            Debug.WriteLine("Erro lendo execução da ordem Spot: " & ex.Message)
            Return False
        End Try
    End Function

    Private Shared Function ReadSpotOrderText(response As JObject, ParamArray names() As String) As String
        If response Is Nothing OrElse names Is Nothing Then Return String.Empty
        For Each fieldName As String In names
            Dim token As JToken = response.Item(fieldName)
            If token IsNot Nothing AndAlso token.Type <> JTokenType.Null Then
                Return token.ToString().Trim()
            End If
        Next
        Return String.Empty
    End Function

    Private Shared Function ReadSpotOrderDecimal(response As JObject, ParamArray names() As String) As Decimal
        Dim value As Decimal
        Dim textValue = ReadSpotOrderText(response, names)
        If Decimal.TryParse(textValue, NumberStyles.Float, CultureInfo.InvariantCulture, value) Then
            Return value
        End If
        Return 0D
    End Function

    Private Function ConsumeSpotOrderQuantity(
        orderResponse As String,
        exchange As String,
        cumulativeQuantity As Decimal) As Decimal

        If cumulativeQuantity <= 0D Then Return 0D

        Try
            Dim response = JObject.Parse(orderResponse)
            Dim orderId = ReadSpotOrderText(response, "orderId", "order_id", "i", "id")
            If String.IsNullOrWhiteSpace(orderId) Then Return cumulativeQuantity

            Dim key = exchange.Trim().ToUpperInvariant() & "|" & orderId
            SyncLock _spotOrderAppliedQuantities
                Dim previousQuantity = 0D
                _spotOrderAppliedQuantities.TryGetValue(key, previousQuantity)
                Dim delta = Math.Max(0D, cumulativeQuantity - previousQuantity)
                _spotOrderAppliedQuantities(key) = Math.Max(previousQuantity, cumulativeQuantity)
                Return delta
            End SyncLock
        Catch ex As Exception
            Debug.WriteLine("Erro controlando execução duplicada Spot: " & ex.Message)
            Return cumulativeQuantity
        End Try
    End Function

    Private Sub BinanceSpotExecutionReportReceived(json As String)
        If String.IsNullOrWhiteSpace(json) Then Return

        Try
            Dim response = JObject.Parse(json)
            Dim executionType = ReadSpotOrderText(response, "x").ToUpperInvariant()
            If executionType <> "TRADE" Then Return

            Dim status = ReadSpotOrderText(response, "X", "status").ToUpperInvariant()
            If status <> "FILLED" AndAlso status <> "PARTIALLY_FILLED" Then Return

            Dim symbol = GetSpotBaseAsset(ReadSpotOrderText(response, "s", "symbol"))
            Dim side = ReadSpotOrderText(response, "S", "side").ToUpperInvariant()
            Dim lastQuantity = ReadSpotOrderDecimal(response, "l", "lastExecutedQty")
            Dim lastPrice = ReadSpotOrderDecimal(response, "L", "lastExecutedPrice")
            Dim cumulativeQuantity = ReadSpotOrderDecimal(response, "z", "executedQty")
            If String.IsNullOrWhiteSpace(symbol) OrElse
               (side <> "BUY" AndAlso side <> "SELL") OrElse
               lastQuantity <= 0D OrElse lastPrice <= 0D Then
                Return
            End If

            Dim orderId = ReadSpotOrderText(response, "i", "orderId")
            Dim quantityToApply = lastQuantity
            If Not String.IsNullOrWhiteSpace(orderId) Then
                Dim key = "BINANCE|" & orderId
                SyncLock _spotOrderAppliedQuantities
                    Dim previousQuantity = 0D
                    _spotOrderAppliedQuantities.TryGetValue(key, previousQuantity)
                    If cumulativeQuantity > 0D Then
                        quantityToApply = Math.Max(0D, cumulativeQuantity - previousQuantity)
                        _spotOrderAppliedQuantities(key) = Math.Max(previousQuantity, cumulativeQuantity)
                    Else
                        _spotOrderAppliedQuantities(key) = previousQuantity + lastQuantity
                    End If
                End SyncLock
            End If

            If quantityToApply > 0D Then
                PortfolioRepository.ApplySpotOrderFill(
                    symbol,
                    "BINANCE",
                    side,
                    quantityToApply,
                    lastPrice,
                    lastPrice)
            End If
        Catch ex As Exception
            Debug.WriteLine("Erro aplicando execução Spot da Binance: " & ex.Message)
        End Try
    End Sub

    Private Sub ConfigureSpotOrdersGrid()
        If _dgOrdensSpot Is Nothing Then
            Return
        End If

        Dim headers As New Dictionary(Of String, String) From {
            {"Symbol", "Cripto"},
            {"Exchange", "Exchange"},
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
                .SelectionBackColor = Color.IndianRed,
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
            Case "Exchange"
                Dim exchange = e.Value?.ToString().Trim().ToUpperInvariant()
                Dim exchangeColor = If(exchange = "GATE.IO", Color.DeepSkyBlue, Color.Gold)
                e.CellStyle.ForeColor = exchangeColor
                e.CellStyle.SelectionForeColor = exchangeColor
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
        Dim exchange = If(_dgOrdensSpot.Columns.Contains("Exchange"), row.Cells("Exchange").Value?.ToString().Trim().ToUpperInvariant(), "BINANCE")
        Dim orderId As Long
        If String.IsNullOrWhiteSpace(symbol) OrElse
           Not Long.TryParse(row.Cells("OrderId").Value?.ToString(), orderId) OrElse
           orderId <= 0 Then
            Return
        End If

        If MessageBox.Show(
               $"Cancelar a ordem {symbol} #{orderId} ({exchange})?",
               "Cancelar ordem Spot",
               MessageBoxButtons.YesNo,
               MessageBoxIcon.Warning) <> DialogResult.Yes Then
            Return
        End If

        Try
            _dgOrdensSpot.Cursor = Cursors.WaitCursor
            Dim cancelled = If(exchange = "GATE.IO",
                               Await _gateSpot.GATE_CancelSpotOrderAsync(symbol, orderId),
                               Await B.BINANCE_CancelSpotOrderAsync(symbol, orderId))
            If cancelled Then
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
        RemoveCellSelectionHighlight(row)
    End Sub
    Private Sub BinanceFuturesWs_ConnectionStateChanged(connected As Boolean, message As String)
        If IsDisposed OrElse Disposing Then Return

        If InvokeRequired Then
            BeginInvoke(New Action(Of Boolean, String)(AddressOf BinanceFuturesWs_ConnectionStateChanged), connected, message)
            Return
        End If

        Debug.WriteLine(message)
        If connected AndAlso lbDebug IsNot Nothing Then
            lbDebug.Clear()
            lbDebug.AppendText("Status: Online")
            changeOnOffColor("Online")
        End If
    End Sub

    Private Sub GateFuturesWs_ConnectionStateChanged(connected As Boolean, message As String)
        If IsDisposed OrElse Disposing Then Return
        If InvokeRequired Then
            BeginInvoke(New Action(Of Boolean, String)(AddressOf GateFuturesWs_ConnectionStateChanged), connected, message)
            Return
        End If
        Debug.WriteLine(message)
        If connected AndAlso lbDebug IsNot Nothing Then
            lbDebug.Clear()
            lbDebug.AppendText("Status: Online")
            changeOnOffColor("Online")
        End If
    End Sub

    Private Sub GateFuturesMarkPriceUpdated(symbol As String, markPrice As Decimal)
        UpdateFuturesMarkPrice(symbol, markPrice, "GATE")
    End Sub

    Private Sub BinanceFuturesMarkPriceUpdated(symbol As String, markPrice As Decimal)
        UpdateFuturesMarkPrice(symbol, markPrice, "BINANCE")
    End Sub

    Private Sub UpdateFuturesMarkPrice(symbol As String, markPrice As Decimal, exchange As String)
        If IsDisposed OrElse Disposing Then Return

        If InvokeRequired Then
            BeginInvoke(New Action(Of String, Decimal, String)(AddressOf UpdateFuturesMarkPrice), symbol, markPrice, exchange)
            Return
        End If

        UpdateFuturesOrdersCurrentPrice(symbol, markPrice, exchange)
        UpdateFuturesOrderCurrentPrice()

        For Each row As DataGridViewRow In dgFuturos.Rows
            If row.IsNewRow OrElse Not String.Equals(row.Cells("Symbol").Value?.ToString(), symbol, StringComparison.OrdinalIgnoreCase) Then
                Continue For
            End If
            If dgFuturos.Columns.Contains("Exchange") AndAlso
               Not String.Equals(
                   NormalizeFuturesExchange(row.Cells("Exchange").Value?.ToString()),
                   NormalizeFuturesExchange(exchange),
                   StringComparison.OrdinalIgnoreCase) Then
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
        UpdateFuturesOrdersCurrentPrice(symbol, price, String.Empty)
    End Sub

    Private Sub UpdateFuturesOrdersCurrentPrice(symbol As String, price As Decimal, exchange As String)
        If _dgOrdensFuturos Is Nothing OrElse
           Not _dgOrdensFuturos.Columns.Contains("Symbol") OrElse
           Not _dgOrdensFuturos.Columns.Contains("CurrentPrice") Then
            Return
        End If

        Dim normalizedSymbol = ToFuturesUsdtSymbol(symbol)
        For Each row As DataGridViewRow In _dgOrdensFuturos.Rows
            If row.IsNewRow Then Continue For

            If (String.IsNullOrWhiteSpace(exchange) OrElse
                Not _dgOrdensFuturos.Columns.Contains("Exchange") OrElse
                String.Equals(
                    NormalizeFuturesExchange(row.Cells("Exchange").Value?.ToString()),
                    NormalizeFuturesExchange(exchange),
                    StringComparison.OrdinalIgnoreCase)) AndAlso
               String.Equals(
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

                      If Not String.Equals(
                             NormalizeFuturesExchange(order.Exchange),
                             NormalizeFuturesExchange(position.Exchange),
                             StringComparison.OrdinalIgnoreCase) Then
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
            {"Exchange", "Exchange"},
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
                .SelectionBackColor = Color.FromArgb(64, 64, 64),
                .SelectionForeColor = Color.Gold
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
            "Symbol", "Exchange", "PositionSide", "InitialMargin", "Leverage", "Notional",
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
        Dim symbol = row.Cells("Symbol").Value?.ToString.Trim.ToUpperInvariant
        Dim exchange = GetRowFuturesExchange(row)
        Dim positionSide = row.Cells("PositionSide").Value?.ToString.Trim.ToUpperInvariant
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
            If exchange = "GATE" Then
                Await _gateFutures.GATE_CloseFuturesPositionAsync(symbol, positionSide, absoluteQuantity, limitPrice)
            Else
                Await B.BINANCE_CloseFuturesPositionAsync(symbol, positionSide, absoluteQuantity, limitPrice)
            End If
            row.Cells("FuturesExitPrice").Value = String.Empty
            Await LoadFuturesPositionsAsync()

            If PanelFuturos.Visible Then
                Await LoadFuturesOrdersAsync()
            End If
            MessageBox.Show($"Ordem de fechamento enviada para {exchange}.", "Fechar posição", MessageBoxButtons.OK, MessageBoxIcon.Information)
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
            Case "Exchange"
                Dim exchange = NormalizeFuturesExchange(e.Value?.ToString)
                Dim exchangeColor = If(exchange = "GATE", Color.DeepSkyBlue, Color.Gold)
                e.CellStyle.ForeColor = exchangeColor
                e.CellStyle.SelectionForeColor = exchangeColor
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
           Not String.Equals(dgFuturos.Columns(e.ColumnIndex).Name, "Symbol", StringComparison.OrdinalIgnoreCase) AndAlso
            Not String.Equals(dgFuturos.Columns(e.ColumnIndex).Name, "TakeProfit", StringComparison.OrdinalIgnoreCase) AndAlso
            Not String.Equals(dgFuturos.Columns(e.ColumnIndex).Name, "StopLoss", StringComparison.OrdinalIgnoreCase) Then
            Return
        End If

        Dim row = dgFuturos.Rows(e.RowIndex)
        Dim clickedColumn = dgFuturos.Columns(e.ColumnIndex).Name
        If String.Equals(clickedColumn, "TakeProfit", StringComparison.OrdinalIgnoreCase) OrElse
           String.Equals(clickedColumn, "StopLoss", StringComparison.OrdinalIgnoreCase) Then
            Await OpenFuturesProtectionFormAsync(row, clickedColumn)
            Return
        End If

        Dim symbol = dgFuturos.Rows(e.RowIndex).Cells("Symbol").Value?.ToString.Trim
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
        Dim exchange = GetRowFuturesExchange(row)
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
                Dim openProtections As List(Of BinanceFuturesProtectionOrder)
                If exchange = "GATE" Then
                    openProtections = Await _gateFutures.GATE_GetFuturesProtectionOrdersAsync()
                Else
                    openProtections = Await B.BINANCE_GetFuturesProtectionOrdersAsync()
                End If
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
                $"Enviar {protectionName} para {exchange}?" & Environment.NewLine & Environment.NewLine &
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
                If exchange = "GATE" Then
                    Await _gateFutures.GATE_CreateFuturesProtectionOrderAsync(symbol, positionSide, protectionType, protectionQuantity, triggerPrice)
                Else
                    Await B.BINANCE_CreateFuturesProtectionOrderAsync(symbol, positionSide, protectionType, protectionQuantity, triggerPrice)
                End If

                For Each existingStop In existingStopOrders
                    Dim cancelled = If(exchange = "GATE",
                                       Await _gateFutures.GATE_CancelFuturesProtectionOrderAsync(symbol, existingStop.AlgoId),
                                       Await B.BINANCE_CancelFuturesProtectionOrderAsync(symbol, existingStop.AlgoId))
                    If Not cancelled Then
                        Throw New Exception($"Não foi possível cancelar a SL anterior #{existingStop.AlgoId}.")
                    End If
                Next

                Await LoadFuturesPositionsAsync()
                If PanelFuturos.Visible Then Await LoadFuturesOrdersAsync()
                MessageBox.Show($"{protectionName} enviado para {exchange}.", "Proteção Futures", MessageBoxButtons.OK, MessageBoxIcon.Information)
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
            Await _gateFuturesMarketWs.StopAsync()
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
        ArrangeDcaPanel(False)
        ArrangeOrdersPanel(False)
        ArrangeFuturesOrdersPanel(False)
        btOrdensSpot.BackColor = Color.FromArgb(20, 20, 20)
        btOrdensFuturo.BackColor = Color.FromArgb(20, 20, 20)
        btTradebot.BackColor = Color.FromArgb(20, 20, 20)
        If _btDca IsNot Nothing Then _btDca.BackColor = Color.FromArgb(20, 20, 20)
        btVisaoGeral.BackColor = Color.SteelBlue
    End Sub
    Private Async Sub btOrdensSpot_Click(sender As Object, e As EventArgs) Handles btOrdensSpot.Click
        PanelGraphs.Visible = False
        PanelOrdens.Visible = True
        PanelFuturos.Visible = False
        ArrangeSpotBotPanel(False)
        ArrangeDcaPanel(False)
        ArrangeOrdersPanel(True)
        ArrangeFuturesOrdersPanel(False)
        btOrdensSpot.BackColor = Color.SteelBlue
        btVisaoGeral.BackColor = Color.FromArgb(20, 20, 20)
        btOrdensFuturo.BackColor = Color.FromArgb(20, 20, 20)
        btTradebot.BackColor = Color.FromArgb(20, 20, 20)
        If _btDca IsNot Nothing Then _btDca.BackColor = Color.FromArgb(20, 20, 20)
        Await LoadSpotOrdersAsync()
    End Sub
    Private Async Sub btOrdensFuturo_Click(sender As Object, e As EventArgs) Handles btOrdensFuturo.Click
        PanelGraphs.Visible = False
        PanelOrdens.Visible = False
        ArrangeSpotBotPanel(False)
        ArrangeDcaPanel(False)
        ArrangeFuturesOrdersPanel(True)
        ArrangeOrdersPanel(False)
        btOrdensSpot.BackColor = Color.FromArgb(20, 20, 20)
        btVisaoGeral.BackColor = Color.FromArgb(20, 20, 20)
        btOrdensFuturo.BackColor = Color.SteelBlue
        btTradebot.BackColor = Color.FromArgb(20, 20, 20)
        If _btDca IsNot Nothing Then _btDca.BackColor = Color.FromArgb(20, 20, 20)
        Await LoadFuturesOrdersAsync()
    End Sub

    Private Sub SimbolosToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SimbolosToolStripMenuItem.Click
        FormSymbols.Show()
    End Sub

    Private Sub CustodiaToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CustodiaToolStripMenuItem.Click
        FormWalletExchange.Show()
    End Sub

    Private Sub btTradebot_Click_1(sender As Object, e As EventArgs) Handles btTradebot.Click
        PanelGraphs.Visible = False
        PanelOrdens.Visible = False
        PanelFuturos.Visible = False
        PanelTradeBot.Visible = True
        ArrangeOrdersPanel(False)
        ArrangeFuturesOrdersPanel(False)
        ArrangeDcaPanel(False)
        ArrangeSpotBotPanel(True)
        UpdateSpotBotPositionsGrid(_spotTradeBot.GetOpenPositions())
        If _spotTradeBot.IsRunning OrElse _spotTradeBot.HasRestoredState Then
            RenderSpotBotSimulationSummary(_spotTradeBot.GetSimulationSnapshot())
        End If
        btVisaoGeral.BackColor = Color.FromArgb(20, 20, 20)
        btOrdensSpot.BackColor = Color.FromArgb(20, 20, 20)
        btOrdensFuturo.BackColor = Color.FromArgb(20, 20, 20)
        btTradebot.BackColor = Color.SteelBlue
        If _btDca IsNot Nothing Then _btDca.BackColor = Color.FromArgb(20, 20, 20)
    End Sub

    Private Sub chkSpotBotBtcFlush_CheckedChanged(sender As Object, e As EventArgs) Handles chkSpotBotBtcFlush.CheckedChanged
        If chkSpotBotBtcFlush.Checked Then
            nudSpotBotBtcFlushPause.Enabled = True
        Else
            nudSpotBotBtcFlushPause.Enabled = False
        End If
    End Sub

End Class
