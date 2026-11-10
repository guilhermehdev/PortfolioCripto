Imports System.Drawing
Imports System.ComponentModel
Imports System.Windows.Forms

<System.ComponentModel.DesignerCategory("UserControl")>
Public Partial Class SpotTradeBotEditor
    Inherits UserControl

    Public Sub New()
        InitializeComponent()
        ' O designer do WinForms cria o UserControl em um contexto parcial.
        ' Os campos já são criados pelo InitializeComponent; não execute a
        ' montagem auxiliar de labels durante o carregamento do designer.
        If LicenseManager.UsageMode <> LicenseUsageMode.Designtime Then
            BuildEditorControls()
        End If
    End Sub

    Private Sub BuildEditorControls()
        If cbSpotBotBollingerMode IsNot Nothing Then
            EnsureEditorLabels()
            Return
        End If

        AddEditorLabel(tpStrategies, "Estratégia base", 6, 4, 150)
        cbSpotBotBollingerMode = AddEditorCombo(tpStrategies, "cbSpotBotBollingerMode", 6, 25, 150, New Object() {"Base Original Protegida", "Desativada"})
        AddEditorLabel(tpStrategies, "Estratégias adicionais", 164, 4, 180)
        chkSpotBotWavePullback = AddEditorCheck(tpStrategies, "chkSpotBotWavePullback", "Pullback EMA34", 164, 25, 105)
        chkSpotBotBreakoutRetest = AddEditorCheck(tpStrategies, "chkSpotBotBreakoutRetest", "Breakout + Reteste", 274, 25, 125)
        chkSpotBotMomentumVolume = AddEditorCheck(tpStrategies, "chkSpotBotMomentumVolume", "Momentum volume", 404, 25, 110)
        chkSpotBotGridAdaptive = AddEditorCheck(tpStrategies, "chkSpotBotGridAdaptive", "Grid adaptativo v2", 520, 25, 125)
        chkSpotBotValueZonePullback = AddEditorCheck(tpStrategies, "chkSpotBotValueZonePullback", "Pullback zona de valor", 240, 59, 165)

        AddEditorLabel(tpStrategies, "Combinação", 650, 4, 100)
        cbSpotBotCombination = AddEditorCombo(tpStrategies, "cbSpotBotCombination", 650, 25, 150, New Object() {"Qualquer estratégia", "Todas as estratégias", "Mínimo de confirmações", "Pontuação ponderada"})
        AddEditorLabel(tpStrategies, "Confirmações", 808, 4, 78)
        nudSpotBotConfirmations = AddEditorNumeric(tpStrategies, "nudSpotBotConfirmations", 2D, 1D, 5D, 0, 808, 25, 64)
        AddEditorLabel(tpStrategies, "Candle", 886, 4, 100)
        cbSpotBotBollingerTimeframe = AddEditorCombo(tpStrategies, "cbSpotBotBollingerTimeframe", 886, 25, 100, New Object() {"4 horas", "Diário"})
        btSpotBotTrades = AddEditorButton(tpStrategies, "btSpotBotTrades", "Trades da run", 994, 25, 110, Color.FromArgb(55, 75, 105))
        btSpotBotExport = AddEditorButton(tpStrategies, "btSpotBotExport", "Exportar CSV", 1110, 25, 100, Color.FromArgb(55, 75, 105))

        AddEditorLabel(tpStrategies, "Universo", 6, 59, 100)
        cbSpotBotUniverseMode = AddEditorCombo(tpStrategies, "cbSpotBotUniverseMode", 6, 80, 120, New Object() {"Manual", "Top volume"})
        btSpotBotUniverse = AddEditorButton(tpStrategies, "btSpotBotUniverse", "Selecionar...", 132, 80, 102, Color.FromArgb(55, 75, 105))
        lbSpotBotSimulationMode.Location = New Point(940, 84)
        lbSpotBotSimulationMode.Size = New Size(205, 20)
        lbSpotBotSimulationMode.ForeColor = Color.Gold
        lbSpotBotSimulationMode.Font = New Font("Calibri", 9.0F, FontStyle.Bold)
        lbSpotBotSimulationMode.Text = "SIMULAÇÃO — sem ordens reais"

        btSpotBotStart = AddEditorButton(tpStrategies, "btSpotBotStart", "Iniciar bot", 6, 112, 110, Color.DarkGreen)
        btSpotBotStop = AddEditorButton(tpStrategies, "btSpotBotStop", "Parar bot", 122, 112, 100, Color.IndianRed)
        btSpotBotStop.Enabled = False
        AddEditorLabel(tpStrategies, "Saldo virtual (USDT)", 240, 89, 110)
        nudSpotBotStartingQuote = AddEditorNumeric(tpStrategies, "nudSpotBotStartingQuote", 1000D, 0D, 100000000D, 2, 240, 108, 98)
        AddEditorLabel(tpStrategies, "Alocação (%)", 348, 89, 86)
        nudSpotBotAllocation = AddEditorNumeric(tpStrategies, "nudSpotBotAllocation", 10D, 0D, 100D, 2, 348, 108, 82)
        AddEditorLabel(tpStrategies, "Taxa (%)", 440, 89, 70)
        nudSpotBotFee = AddEditorNumeric(tpStrategies, "nudSpotBotFee", 0.1D, 0D, 5D, 3, 440, 108, 70)
        AddEditorLabel(tpStrategies, "TP (%)", 520, 89, 75)
        nudSpotBotTakeProfit = AddEditorNumeric(tpStrategies, "nudSpotBotTakeProfit", 2D, 0D, 100D, 2, 520, 108, 75)
        AddEditorLabel(tpStrategies, "SL (%)", 604, 89, 75)
        nudSpotBotStopLoss = AddEditorNumeric(tpStrategies, "nudSpotBotStopLoss", 1D, 0D, 100D, 2, 604, 108, 75)
        AddEditorLabel(tpStrategies, "Máx. ativos", 704, 89, 90)
        nudSpotBotMaxAssets = AddEditorNumeric(tpStrategies, "nudSpotBotMaxAssets", 20D, 1D, 500D, 0, 704, 108, 75)
        AddEditorLabel(tpStrategies, "Volume mínimo", 790, 89, 105)
        nudSpotBotMinVolume = AddEditorNumeric(tpStrategies, "nudSpotBotMinVolume", 1000000D, 0D, 1000000000000D, 0, 790, 108, 112)

        AddEditorLabel(tpStrategies, "Proteções dinâmicas", 940, 89, 210)
        chkSpotBotBreakEven = AddEditorCheck(tpStrategies, "chkSpotBotBreakEven", "BE", 940, 108, 40)
        nudSpotBotBreakEvenTrigger = AddEditorNumeric(tpStrategies, "nudSpotBotBreakEvenTrigger", 1.5D, 0D, 100D, 2, 980, 108, 62)
        chkSpotBotTrailing = AddEditorCheck(tpStrategies, "chkSpotBotTrailing", "Trail", 1046, 108, 50)
        nudSpotBotTrailingDistance = AddEditorNumeric(tpStrategies, "nudSpotBotTrailingDistance", 1D, 0.01D, 100D, 2, 1096, 108, 62)

        AddEditorLabel(tpStrategies, "Cooldown pós-saída (min)", 6, 137, 160)
        nudSpotBotPostExitCooldown = AddEditorNumeric(tpStrategies, "nudSpotBotPostExitCooldown", 15D, 0D, 1440D, 0, 6, 156, 82)
        AddEditorLabel(tpStrategies, "Negócios 24h min.", 175, 137, 130)
        nudSpotBotMinTrades = AddEditorNumeric(tpStrategies, "nudSpotBotMinTrades", 1000D, 0D, 10000000D, 0, 175, 156, 90)
        AddEditorLabel(tpStrategies, "Grid risco/nível (%)", 280, 137, 125)
        nudSpotBotGridRisk = AddEditorNumeric(tpStrategies, "nudSpotBotGridRisk", 0.5D, 0D, 100D, 2, 280, 156, 72)
        AddEditorLabel(tpStrategies, "Reserva (%)", 360, 137, 70)
        nudSpotBotGridReserve = AddEditorNumeric(tpStrategies, "nudSpotBotGridReserve", 20D, 0D, 95D, 2, 360, 156, 65)
        AddEditorLabel(tpStrategies, "Exposição máx. (%)", 433, 137, 112)
        nudSpotBotGridExposure = AddEditorNumeric(tpStrategies, "nudSpotBotGridExposure", 30D, 0D, 100D, 2, 433, 156, 78)
        AddEditorLabel(tpStrategies, "Níveis", 519, 137, 52)
        nudSpotBotGridLevels = AddEditorNumeric(tpStrategies, "nudSpotBotGridLevels", 5D, 1D, 20D, 0, 519, 156, 58)
        chkSpotBotGridRejection = AddEditorCheck(tpStrategies, "chkSpotBotGridRejection", "Confirmar rejeição", 585, 156, 135)

        AddEditorLabel(tpStrategies, "Flush BTC (%)", 730, 137, 92)
        nudSpotBotBtcFlushDrop = AddEditorNumeric(tpStrategies, "nudSpotBotBtcFlushDrop", 2D, 0.1D, 20D, 2, 730, 156, 72)
        AddEditorLabel(tpStrategies, "Pausa (min)", 810, 137, 78)
        nudSpotBotBtcFlushPause = AddEditorNumeric(tpStrategies, "nudSpotBotBtcFlushPause", 30D, 1D, 1440D, 0, 810, 156, 72)
        chkSpotBotBtcFlush = AddEditorCheck(tpStrategies, "chkSpotBotBtcFlush", "Proteger no flush BTC", 890, 156, 155)
        AddEditorLabel(tpStrategies, "Vol. x", 1050, 137, 55)
        nudSpotBotBtcFlushVolume = AddEditorNumeric(tpStrategies, "nudSpotBotBtcFlushVolume", 1.8D, 1.1D, 20D, 2, 1050, 156, 65)
        AddEditorLabel(tpStrategies, "Queda diária (%)", 730, 181, 78)
        nudSpotBotBtcDailyDrop = AddEditorNumeric(tpStrategies, "nudSpotBotBtcDailyDrop", 4D, 0.1D, 50D, 2, 730, 200, 72)
        AddEditorLabel(tpStrategies, "Oscilação do dia (%)", 810, 181, 105)
        nudSpotBotBtcDailyDrawdown = AddEditorNumeric(tpStrategies, "nudSpotBotBtcDailyDrawdown", 5D, 0.1D, 50D, 2, 810, 200, 90)
        chkSpotBotBtcDaily = AddEditorCheck(tpStrategies, "chkSpotBotBtcDaily", "Proteger no diário", 910, 200, 145)

        AddEditorLabel(tpRiskBrake, "Ativar freio", 10, 14, 100)
        chkSpotBotRiskBrakeEnabled = AddEditorCheck(tpRiskBrake, "chkSpotBotRiskBrakeEnabled", "Proteção geral", 10, 35, 130)
        AddEditorLabel(tpRiskBrake, "Perda máx. da sessão (%)", 155, 14, 150)
        nudSpotBotRiskSessionLoss = AddEditorNumeric(tpRiskBrake, "nudSpotBotRiskSessionLoss", 2D, 0D, 100D, 2, 155, 35, 90)
        AddEditorLabel(tpRiskBrake, "Trava de lucro", 265, 14, 120)
        chkSpotBotRiskProfitLock = AddEditorCheck(tpRiskBrake, "chkSpotBotRiskProfitLock", "Ativa", 265, 35, 70)
        AddEditorLabel(tpRiskBrake, "Armar em ganho (%)", 345, 14, 130)
        nudSpotBotRiskProfitTrigger = AddEditorNumeric(tpRiskBrake, "nudSpotBotRiskProfitTrigger", 1D, 0D, 100D, 2, 345, 35, 90)
        AddEditorLabel(tpRiskBrake, "Devolver do pico (%)", 455, 14, 135)
        nudSpotBotRiskGiveback = AddEditorNumeric(tpRiskBrake, "nudSpotBotRiskGiveback", 50D, 0D, 100D, 0, 455, 35, 90)
        AddEditorLabel(tpRiskBrake, "Máx. perdas seguidas", 10, 80, 140)
        nudSpotBotRiskMaxLosses = AddEditorNumeric(tpRiskBrake, "nudSpotBotRiskMaxLosses", 3D, 0D, 100D, 0, 10, 101, 90)
        AddEditorLabel(tpRiskBrake, "Pausa após sequência (min)", 120, 80, 180)
        nudSpotBotRiskPause = AddEditorNumeric(tpRiskBrake, "nudSpotBotRiskPause", 60D, 1D, 10080D, 0, 120, 101, 100)
        chkSpotBotRiskClosePositions = AddEditorCheck(tpRiskBrake, "chkSpotBotRiskClosePositions", "Fechar posições no limite", 245, 104, 190)
        chkSpotBotRiskUseUnrealized = AddEditorCheck(tpRiskBrake, "chkSpotBotRiskUseUnrealized", "Considerar P/L aberto", 470, 104, 180)

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

    Private Sub EnsureEditorLabels()
        Dim strategyLabelExists = False
        For Each control As Control In tpStrategies.Controls
            If TypeOf control Is Label AndAlso String.Equals(control.Text, "Estratégia base", StringComparison.Ordinal) Then
                strategyLabelExists = True
                Exit For
            End If
        Next

        If strategyLabelExists Then Return

        AddEditorLabel(tpStrategies, "Estratégia base", 6, 4, 150)
        AddEditorLabel(tpStrategies, "Estratégias adicionais", 164, 4, 180)
        AddEditorLabel(tpStrategies, "Combinação", 650, 4, 100)
        AddEditorLabel(tpStrategies, "Confirmações", 808, 4, 78)
        AddEditorLabel(tpStrategies, "Candle", 886, 4, 100)
        AddEditorLabel(tpStrategies, "Universo", 6, 59, 100)
        AddEditorLabel(tpStrategies, "Saldo virtual (USDT)", 240, 89, 110)
        AddEditorLabel(tpStrategies, "Alocação (%)", 348, 89, 86)
        AddEditorLabel(tpStrategies, "Taxa (%)", 440, 89, 70)
        AddEditorLabel(tpStrategies, "TP (%)", 520, 89, 75)
        AddEditorLabel(tpStrategies, "SL (%)", 604, 89, 75)
        AddEditorLabel(tpStrategies, "Máx. ativos", 704, 89, 90)
        AddEditorLabel(tpStrategies, "Volume mínimo", 790, 89, 105)
        AddEditorLabel(tpStrategies, "Proteções dinâmicas", 940, 89, 210)
        AddEditorLabel(tpStrategies, "Cooldown pós-saída (min)", 6, 137, 160)
        AddEditorLabel(tpStrategies, "Negócios 24h min.", 175, 137, 130)
        AddEditorLabel(tpStrategies, "Grid risco/nível (%)", 280, 137, 125)
        AddEditorLabel(tpStrategies, "Reserva (%)", 360, 137, 70)
        AddEditorLabel(tpStrategies, "Exposição máx. (%)", 433, 137, 112)
        AddEditorLabel(tpStrategies, "Níveis", 519, 137, 52)
        AddEditorLabel(tpStrategies, "Flush BTC (%)", 730, 137, 92)
        AddEditorLabel(tpStrategies, "Pausa (min)", 810, 137, 78)
        AddEditorLabel(tpStrategies, "Vol. x", 1050, 137, 55)
        AddEditorLabel(tpStrategies, "Queda diária (%)", 730, 181, 78)
        AddEditorLabel(tpStrategies, "Oscilação do dia (%)", 810, 181, 105)

        AddEditorLabel(tpRiskBrake, "Ativar freio", 10, 14, 100)
        AddEditorLabel(tpRiskBrake, "Perda máx. da sessão (%)", 155, 14, 150)
        AddEditorLabel(tpRiskBrake, "Trava de lucro", 265, 14, 120)
        AddEditorLabel(tpRiskBrake, "Armar em ganho (%)", 345, 14, 130)
        AddEditorLabel(tpRiskBrake, "Devolver do pico (%)", 455, 14, 135)
        AddEditorLabel(tpRiskBrake, "Máx. perdas seguidas", 10, 80, 140)
        AddEditorLabel(tpRiskBrake, "Pausa após sequência (min)", 120, 80, 180)
    End Sub

    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(disposing As Boolean)
        If disposing AndAlso components IsNot Nothing Then components.Dispose()
        MyBase.Dispose(disposing)
    End Sub

    Private components As System.ComponentModel.IContainer
End Class
