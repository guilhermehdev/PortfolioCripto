<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class SpotTradeBotEditor
    Private tcSettings As TabControl
    Private tpStrategies As TabPage
    Private tpRiskBrake As TabPage

    Public cbSpotBotBollingerMode As ComboBox
    Public chkSpotBotWavePullback As CheckBox
    Public chkSpotBotBreakoutRetest As CheckBox
    Public chkSpotBotMomentumVolume As CheckBox
    Public chkSpotBotGridAdaptive As CheckBox
    Public chkSpotBotValueZonePullback As CheckBox
    Public chkSpotBotBtcMildDecline As CheckBox
    Public cbSpotBotCombination As ComboBox
    Public nudSpotBotConfirmations As NumericUpDown
    Public cbSpotBotBollingerTimeframe As ComboBox
    Public cbSpotBotUniverseMode As ComboBox
    Public btSpotBotUniverse As Button
    Public lbSpotBotSimulationMode As Label
    Public btSpotBotStart As Button
    Public btSpotBotStop As Button
    Public btSpotBotExport As Button
    Public btSpotBotTrades As Button
    Public nudSpotBotStartingQuote As NumericUpDown
    Public nudSpotBotAllocation As NumericUpDown
    Public nudSpotBotFee As NumericUpDown
    Public nudSpotBotTakeProfit As NumericUpDown
    Public nudSpotBotStopLoss As NumericUpDown
    Public nudSpotBotMaxAssets As NumericUpDown
    Public nudSpotBotMinVolume As NumericUpDown
    Public chkSpotBotBreakEven As CheckBox
    Public nudSpotBotBreakEvenTrigger As NumericUpDown
    Public chkSpotBotTrailing As CheckBox
    Public nudSpotBotTrailingDistance As NumericUpDown
    Public nudSpotBotPostExitCooldown As NumericUpDown
    Public nudSpotBotMinTrades As NumericUpDown
    Public nudSpotBotGridRisk As NumericUpDown
    Public nudSpotBotGridReserve As NumericUpDown
    Public nudSpotBotGridExposure As NumericUpDown
    Public nudSpotBotGridLevels As NumericUpDown
    Public chkSpotBotGridRejection As CheckBox
    Public nudSpotBotBtcFlushDrop As NumericUpDown
    Public nudSpotBotBtcFlushPause As NumericUpDown
    Public chkSpotBotBtcFlush As CheckBox
    Public nudSpotBotBtcFlushVolume As NumericUpDown
    Public nudSpotBotBtcDailyDrop As NumericUpDown
    Public nudSpotBotBtcDailyDrawdown As NumericUpDown
    Public chkSpotBotBtcDaily As CheckBox

    Public chkSpotBotRiskBrakeEnabled As CheckBox
    Public nudSpotBotRiskSessionLoss As NumericUpDown
    Public chkSpotBotRiskProfitLock As CheckBox
    Public nudSpotBotRiskProfitTrigger As NumericUpDown
    Public nudSpotBotRiskGiveback As NumericUpDown
    Public nudSpotBotRiskMaxLosses As NumericUpDown
    Public nudSpotBotRiskPause As NumericUpDown
    Public chkSpotBotRiskClosePositions As CheckBox
    Public chkSpotBotRiskUseUnrealized As CheckBox

    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        tcSettings = New TabControl()
        tpStrategies = New TabPage()
        tpRiskBrake = New TabPage()
        lbSpotBotSimulationMode = New Label()
        tcSettings.SuspendLayout()
        SuspendLayout()
        ' 
        ' tcSettings
        ' 
        tcSettings.Controls.Add(tpStrategies)
        tcSettings.Controls.Add(tpRiskBrake)
        tcSettings.Location = New Point(0, 0)
        tcSettings.Name = "tcSettings"
        tcSettings.SelectedIndex = 0
        tcSettings.Size = New Size(1147, 287)
        tcSettings.TabIndex = 0
        ' 
        ' tpStrategies
        ' 
        tpStrategies.BackColor = SystemColors.ActiveCaptionText
        tpStrategies.Location = New Point(4, 24)
        tpStrategies.Name = "tpStrategies"
        tpStrategies.Size = New Size(1139, 259)
        tpStrategies.TabIndex = 0
        tpStrategies.Text = "Estratégias e execução"
        ' 
        ' tpRiskBrake
        ' 
        tpRiskBrake.BackColor = SystemColors.ActiveCaptionText
        tpRiskBrake.Location = New Point(4, 24)
        tpRiskBrake.Name = "tpRiskBrake"
        tpRiskBrake.Size = New Size(1139, 259)
        tpRiskBrake.TabIndex = 1
        tpRiskBrake.Text = "Freio de perdas"
        ' 
        ' lbSpotBotSimulationMode
        ' 
        lbSpotBotSimulationMode.Location = New Point(0, 0)
        lbSpotBotSimulationMode.Name = "lbSpotBotSimulationMode"
        lbSpotBotSimulationMode.Size = New Size(100, 23)
        lbSpotBotSimulationMode.TabIndex = 0
        ' 
        ' SpotTradeBotEditor
        ' 
        BackColor = Color.FromArgb(CByte(30), CByte(30), CByte(30))
        Controls.Add(tcSettings)
        Name = "SpotTradeBotEditor"
        Size = New Size(1150, 290)
        tcSettings.ResumeLayout(False)
        ResumeLayout(False)
    End Sub

    Private Sub InitializeEditorControls()
        cbSpotBotBollingerMode = AddEditorCombo(tpStrategies, "cbSpotBotBollingerMode", 6, 25, 150, New Object() {"Base Original Protegida", "Desativada"})
        chkSpotBotWavePullback = AddEditorCheck(tpStrategies, "chkSpotBotWavePullback", "Pullback EMA34", 164, 25, 105)
        chkSpotBotBreakoutRetest = AddEditorCheck(tpStrategies, "chkSpotBotBreakoutRetest", "Breakout + Reteste", 274, 25, 125)
        chkSpotBotMomentumVolume = AddEditorCheck(tpStrategies, "chkSpotBotMomentumVolume", "Momentum volume", 404, 25, 110)
        chkSpotBotGridAdaptive = AddEditorCheck(tpStrategies, "chkSpotBotGridAdaptive", "Grid adaptativo v2", 520, 25, 125)
        chkSpotBotValueZonePullback = AddEditorCheck(tpStrategies, "chkSpotBotValueZonePullback", "Pullback zona de valor", 240, 59, 165)
        chkSpotBotBtcMildDecline = AddEditorCheck(tpStrategies, "chkSpotBotBtcMildDecline", "BTC queda leve", 410, 59, 130)
        cbSpotBotCombination = AddEditorCombo(tpStrategies, "cbSpotBotCombination", 650, 25, 150, New Object() {"Qualquer estratégia", "Todas as estratégias", "Mínimo de confirmações", "Pontuação ponderada"})
        nudSpotBotConfirmations = AddEditorNumeric(tpStrategies, "nudSpotBotConfirmations", 2D, 1D, 5D, 0, 808, 25, 64)
        cbSpotBotBollingerTimeframe = AddEditorCombo(tpStrategies, "cbSpotBotBollingerTimeframe", 886, 25, 100, New Object() {"4 horas", "Diário"})
        cbSpotBotUniverseMode = AddEditorCombo(tpStrategies, "cbSpotBotUniverseMode", 6, 80, 120, New Object() {"Manual", "Top volume"})
        btSpotBotUniverse = AddEditorButton(tpStrategies, "btSpotBotUniverse", "Selecionar...", 132, 80, 102, Color.FromArgb(55, 75, 105))
        btSpotBotTrades = AddEditorButton(tpStrategies, "btSpotBotTrades", "Trades da run", 994, 25, 110, Color.FromArgb(55, 75, 105))
        btSpotBotExport = AddEditorButton(tpStrategies, "btSpotBotExport", "Exportar CSV", 1110, 25, 100, Color.FromArgb(55, 75, 105))
        btSpotBotStart = AddEditorButton(tpStrategies, "btSpotBotStart", "Iniciar bot", 6, 112, 110, Color.DarkGreen)
        btSpotBotStop = AddEditorButton(tpStrategies, "btSpotBotStop", "Parar bot", 122, 112, 100, Color.IndianRed)
        btSpotBotStop.Enabled = False
        nudSpotBotStartingQuote = AddEditorNumeric(tpStrategies, "nudSpotBotStartingQuote", 1000D, 0D, 100000000D, 2, 240, 108, 98)
        nudSpotBotAllocation = AddEditorNumeric(tpStrategies, "nudSpotBotAllocation", 10D, 0D, 100D, 2, 348, 108, 82)
        nudSpotBotFee = AddEditorNumeric(tpStrategies, "nudSpotBotFee", 0.1D, 0D, 5D, 3, 440, 108, 70)
        nudSpotBotTakeProfit = AddEditorNumeric(tpStrategies, "nudSpotBotTakeProfit", 2D, 0D, 100D, 2, 520, 108, 75)
        nudSpotBotStopLoss = AddEditorNumeric(tpStrategies, "nudSpotBotStopLoss", 1D, 0D, 100D, 2, 604, 108, 75)
        nudSpotBotMaxAssets = AddEditorNumeric(tpStrategies, "nudSpotBotMaxAssets", 20D, 1D, 500D, 0, 704, 108, 75)
        nudSpotBotMinVolume = AddEditorNumeric(tpStrategies, "nudSpotBotMinVolume", 1000000D, 0D, 1000000000000D, 0, 790, 108, 112)
        chkSpotBotBreakEven = AddEditorCheck(tpStrategies, "chkSpotBotBreakEven", "BE", 940, 108, 40)
        nudSpotBotBreakEvenTrigger = AddEditorNumeric(tpStrategies, "nudSpotBotBreakEvenTrigger", 1.5D, 0D, 100D, 2, 980, 108, 62)
        chkSpotBotTrailing = AddEditorCheck(tpStrategies, "chkSpotBotTrailing", "Trail", 1046, 108, 50)
        nudSpotBotTrailingDistance = AddEditorNumeric(tpStrategies, "nudSpotBotTrailingDistance", 1D, 0.01D, 100D, 2, 1096, 108, 62)
        nudSpotBotPostExitCooldown = AddEditorNumeric(tpStrategies, "nudSpotBotPostExitCooldown", 15D, 0D, 1440D, 0, 6, 156, 82)
        nudSpotBotMinTrades = AddEditorNumeric(tpStrategies, "nudSpotBotMinTrades", 1000D, 0D, 10000000D, 0, 175, 156, 90)
        nudSpotBotGridRisk = AddEditorNumeric(tpStrategies, "nudSpotBotGridRisk", 0.5D, 0D, 100D, 2, 280, 156, 72)
        nudSpotBotGridReserve = AddEditorNumeric(tpStrategies, "nudSpotBotGridReserve", 20D, 0D, 95D, 2, 360, 156, 65)
        nudSpotBotGridExposure = AddEditorNumeric(tpStrategies, "nudSpotBotGridExposure", 30D, 0D, 100D, 2, 433, 156, 78)
        nudSpotBotGridLevels = AddEditorNumeric(tpStrategies, "nudSpotBotGridLevels", 5D, 1D, 20D, 0, 519, 156, 58)
        chkSpotBotGridRejection = AddEditorCheck(tpStrategies, "chkSpotBotGridRejection", "Confirmar rejeição", 585, 156, 135)
        nudSpotBotBtcFlushDrop = AddEditorNumeric(tpStrategies, "nudSpotBotBtcFlushDrop", 2D, 0.1D, 20D, 2, 730, 156, 72)
        nudSpotBotBtcFlushPause = AddEditorNumeric(tpStrategies, "nudSpotBotBtcFlushPause", 30D, 1D, 1440D, 0, 810, 156, 72)
        chkSpotBotBtcFlush = AddEditorCheck(tpStrategies, "chkSpotBotBtcFlush", "Proteger no flush BTC", 890, 156, 155)
        nudSpotBotBtcFlushVolume = AddEditorNumeric(tpStrategies, "nudSpotBotBtcFlushVolume", 1.8D, 1.1D, 20D, 2, 1050, 156, 65)
        nudSpotBotBtcDailyDrop = AddEditorNumeric(tpStrategies, "nudSpotBotBtcDailyDrop", 4D, 0.1D, 50D, 2, 730, 200, 72)
        nudSpotBotBtcDailyDrawdown = AddEditorNumeric(tpStrategies, "nudSpotBotBtcDailyDrawdown", 5D, 0.1D, 50D, 2, 810, 200, 90)
        chkSpotBotBtcDaily = AddEditorCheck(tpStrategies, "chkSpotBotBtcDaily", "Proteger no diário", 910, 200, 145)

        chkSpotBotRiskBrakeEnabled = AddEditorCheck(tpRiskBrake, "chkSpotBotRiskBrakeEnabled", "Proteção geral", 10, 35, 130)
        nudSpotBotRiskSessionLoss = AddEditorNumeric(tpRiskBrake, "nudSpotBotRiskSessionLoss", 2D, 0D, 100D, 2, 155, 35, 90)
        chkSpotBotRiskProfitLock = AddEditorCheck(tpRiskBrake, "chkSpotBotRiskProfitLock", "Ativa", 265, 35, 70)
        nudSpotBotRiskProfitTrigger = AddEditorNumeric(tpRiskBrake, "nudSpotBotRiskProfitTrigger", 1D, 0D, 100D, 2, 345, 35, 90)
        nudSpotBotRiskGiveback = AddEditorNumeric(tpRiskBrake, "nudSpotBotRiskGiveback", 50D, 0D, 100D, 0, 455, 35, 90)
        nudSpotBotRiskMaxLosses = AddEditorNumeric(tpRiskBrake, "nudSpotBotRiskMaxLosses", 3D, 0D, 100D, 0, 10, 101, 90)
        nudSpotBotRiskPause = AddEditorNumeric(tpRiskBrake, "nudSpotBotRiskPause", 60D, 1D, 10080D, 0, 120, 101, 100)
        chkSpotBotRiskClosePositions = AddEditorCheck(tpRiskBrake, "chkSpotBotRiskClosePositions", "Fechar posições no limite", 245, 104, 190)
        chkSpotBotRiskUseUnrealized = AddEditorCheck(tpRiskBrake, "chkSpotBotRiskUseUnrealized", "Considerar P/L aberto", 470, 104, 180)

        lbSpotBotSimulationMode.Location = New Point(940, 84)
        lbSpotBotSimulationMode.Size = New Size(205, 20)
        lbSpotBotSimulationMode.ForeColor = Color.Gold
        lbSpotBotSimulationMode.Font = New Font("Calibri", 9.0F, FontStyle.Bold)
        lbSpotBotSimulationMode.Text = "SIMULAÇÃO — sem ordens reais"
        tpStrategies.Controls.Add(lbSpotBotSimulationMode)

        cbSpotBotBollingerMode.SelectedIndex = 0
        cbSpotBotCombination.SelectedIndex = 2
        cbSpotBotBollingerTimeframe.SelectedIndex = 1
        cbSpotBotUniverseMode.SelectedIndex = 0
        chkSpotBotBreakEven.Checked = True
        chkSpotBotGridRejection.Checked = True
        chkSpotBotBtcFlush.Checked = True
        chkSpotBotBtcDaily.Checked = True
        chkSpotBotRiskBrakeEnabled.Checked = True
        chkSpotBotRiskProfitLock.Checked = True
        chkSpotBotRiskClosePositions.Checked = True
        chkSpotBotRiskUseUnrealized.Checked = True
    End Sub

    Private Shared Sub AddEditorLabel(parent As Control, text As String, left As Integer, top As Integer, width As Integer)
        parent.Controls.Add(New Label With {
            .AutoSize = False,
            .Location = New Point(left, top),
            .Size = New Size(width, 20),
            .ForeColor = Color.Aqua,
            .Font = New Font("Calibri", 10.0F, FontStyle.Italic),
            .Text = text})
    End Sub

    Private Shared Function AddEditorCombo(parent As Control, name As String, left As Integer, top As Integer, width As Integer, items As Object()) As ComboBox
        Dim control = New ComboBox With {
            .Name = name,
            .DropDownStyle = ComboBoxStyle.DropDownList,
            .BackColor = Color.FromArgb(64, 64, 64),
            .ForeColor = Color.White,
            .FlatStyle = FlatStyle.Flat,
            .Location = New Point(left, top),
            .Width = width}
        control.Items.AddRange(items)
        parent.Controls.Add(control)
        Return control
    End Function

    Private Shared Function AddEditorNumeric(parent As Control, name As String, value As Decimal, minimum As Decimal, maximum As Decimal, decimalPlaces As Integer, left As Integer, top As Integer, width As Integer) As NumericUpDown
        Dim control = New NumericUpDown With {
            .Name = name,
            .DecimalPlaces = decimalPlaces,
            .Increment = If(decimalPlaces = 0, 1D, 0.1D),
            .Minimum = minimum,
            .Maximum = maximum,
            .Value = value,
            .BackColor = Color.FromArgb(64, 64, 64),
            .ForeColor = Color.White,
            .Location = New Point(left, top),
            .Width = width}
        parent.Controls.Add(control)
        Return control
    End Function

    Private Shared Function AddEditorCheck(parent As Control, name As String, text As String, left As Integer, top As Integer, width As Integer) As CheckBox
        Dim control = New CheckBox With {
            .Name = name,
            .Text = text,
            .AutoSize = False,
            .BackColor = Color.FromArgb(30, 30, 30),
            .ForeColor = Color.White,
            .Location = New Point(left, top),
            .Size = New Size(width, 23),
            .Cursor = Cursors.Hand}
        parent.Controls.Add(control)
        Return control
    End Function

    Private Shared Function AddEditorButton(parent As Control, name As String, text As String, left As Integer, top As Integer, width As Integer, backColor As Color) As Button
        Dim control = New Button With {
            .Name = name,
            .Text = text,
            .BackColor = backColor,
            .ForeColor = Color.White,
            .FlatStyle = FlatStyle.Flat,
            .Location = New Point(left, top),
            .Size = New Size(width, 26),
            .Cursor = Cursors.Hand}
        control.FlatAppearance.BorderSize = 0
        control.FlatAppearance.MouseOverBackColor = Color.RoyalBlue
        control.FlatAppearance.MouseDownBackColor = Color.DodgerBlue
        parent.Controls.Add(control)
        Return control
    End Function
End Class
