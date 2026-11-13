Imports System
Imports System.Collections.Generic

Public Enum SpotBotSignalDirection
    Hold = 0
    Buy = 1
    Sell = -1
End Enum

Public Enum SpotBotCombinationMode
    Any = 0
    All = 1
    MinimumConfirmations = 2
    WeightedScore = 3
End Enum

Public Enum SpotBotStrategyKind
    EmaCrossover = 0
    Rsi = 1
    Breakout = 2
    Grid = 3
    BollingerMeanReversion = 4
    WavePullback = 5
    BnfContrarian = 6
    ' Perfis nomeados do Tradebot. Os valores novos preservam as simulações
    ' antigas que ainda possam estar persistidas no SQLite.
    BaseOriginalProtected = 7
    BreakoutRetest = 8
    MomentumVolume = 9
    GridRegime = 10
    GridAdaptive = 11
    ' Pullback de tendência na zona EMA/VWAP, adaptado do vídeo analisado.
    ValueZonePullback = 12
    ' Valor legado de versões que exibiam o regime BTC como estratégia separada.
    ' O regime agora é uma opção interna do Grid V2.
    BtcMildDeclinePullback = 13
End Enum

Public Enum SpotBotUniverseMode
    Manual = 0
    TopVolume = 1
End Enum

Public Enum SpotBotRiskBrakeState
    Normal = 0
    ProfitLockArmed = 1
    Paused = 2
    HardStop = 3
End Enum

Public NotInheritable Class SpotBotPricePoint
    Public Sub New(
        timestampUtc As DateTime,
        price As Decimal,
        Optional openPrice As Decimal = 0D,
        Optional highPrice As Decimal = 0D,
        Optional lowPrice As Decimal = 0D,
        Optional volume As Decimal = 0D)

        Me.TimestampUtc = timestampUtc
        Me.Price = price
        Me.Open = If(openPrice > 0D, openPrice, price)
        Me.High = If(highPrice > 0D, highPrice, Math.Max(Me.Open, price))
        Me.Low = If(lowPrice > 0D, lowPrice, Math.Min(Me.Open, price))
        Me.Volume = Math.Max(0D, volume)
    End Sub

    Public ReadOnly Property TimestampUtc As DateTime
    Public ReadOnly Property Price As Decimal
    Public ReadOnly Property Open As Decimal
    Public ReadOnly Property High As Decimal
    Public ReadOnly Property Low As Decimal
    Public ReadOnly Property Volume As Decimal
    Public ReadOnly Property Close As Decimal
        Get
            Return Price
        End Get
    End Property

    Public ReadOnly Property Range As Decimal
        Get
            Return Math.Max(0D, High - Low)
        End Get
    End Property

    Public ReadOnly Property Body As Decimal
        Get
            Return Math.Abs(Close - Open)
        End Get
    End Property

    Public ReadOnly Property IsBullish As Boolean
        Get
            Return Close > Open
        End Get
    End Property

    Public ReadOnly Property IsBearish As Boolean
        Get
            Return Close < Open
        End Get
    End Property
End Class

Public NotInheritable Class SpotBotStrategySignal
    Public Property Strategy As SpotBotStrategyKind
    Public Property Direction As SpotBotSignalDirection
    Public Property Confidence As Decimal
    Public Property Reason As String = String.Empty
    Public Property ObservedValue As Decimal
    Public Property EntryPrice As Decimal
    Public Property StopPrice As Decimal
    Public Property TakeProfitPrice As Decimal
    Public Property SupportPrice As Decimal
    Public Property GridManaged As Boolean
    Public Property GridSpacingPercent As Decimal
End Class

Public NotInheritable Class SpotBotSignal
    Public Property Symbol As String = String.Empty
    Public Property TimestampUtc As DateTime
    Public Property Price As Decimal
    Public Property EntryPrice As Decimal
    Public Property StopPrice As Decimal
    Public Property TakeProfitPrice As Decimal
    Public Property SupportPrice As Decimal
    Public Property Direction As SpotBotSignalDirection
    Public Property Confirmations As Integer
    Public Property ActiveStrategies As Integer
    Public Property Score As Decimal
    Public Property Reasons As New List(Of String)
    Public Property StrategySignals As New List(Of SpotBotStrategySignal)
    Public Property SimulationOnly As Boolean = True
    Public Property GridManaged As Boolean
    Public Property GridSpacingPercent As Decimal
End Class

Public NotInheritable Class SpotBotConfiguration
    Public Sub New()
        EnabledStrategies = New HashSet(Of SpotBotStrategyKind)()
    End Sub

    Public Property EnabledStrategies As HashSet(Of SpotBotStrategyKind)
    Public Property CombinationMode As SpotBotCombinationMode = SpotBotCombinationMode.MinimumConfirmations
    Public Property RequiredConfirmations As Integer = 2
    Public Property MinimumWeightedScore As Decimal = 0.6D
    Public Property Cooldown As TimeSpan = TimeSpan.FromMinutes(5)
    ' Impede uma nova entrada no mesmo ativo imediatamente após o fechamento.
    ' O bloqueio não atrasa stop, alvo ou fechamento manual; ele só filtra reentradas.
    Public Property PostExitCooldownMinutes As Decimal = 5D
    ' Permite uma nova posição em outro ativo somente quando as posições
    ' existentes estiverem com lucro líquido estimado acima do limite.
    ' Grid/DCA no mesmo ativo e saídas não passam por este filtro.
    Public Property NewPositionProfitGateEnabled As Boolean = True
    Public Property MinimumNetProfitBeforeNewPositionPercent As Decimal = 0.5D
    Public Property MaximumPriceHistory As Integer = 250

    Public Property EmaFastPeriod As Integer = 9
    Public Property EmaSlowPeriod As Integer = 21
    Public Property RsiPeriod As Integer = 14
    Public Property RsiOversold As Decimal = 30D
    Public Property RsiOverbought As Decimal = 70D
    Public Property BreakoutPeriod As Integer = 20
    Public Property GridSpacingPercent As Decimal = 1D
    Public Property GridReferencePrice As Decimal
    Public Property GridRegimeLookback As Integer = 30
    Public Property GridRegimeMaxRangePercent As Decimal = 12D
    Public Property GridRegimeMaxSlopePercent As Decimal = 4D

    ' Parâmetros do momentum confirmado por volume de 24h do ticker.
    Public Property MomentumVolumeLookback As Integer = 20
    Public Property MomentumVolumeMultiplier As Decimal = 1.5D
    Public Property MomentumEmaPeriod As Integer = 21
    Public Property MomentumRiskReward As Decimal = 2D

    ' Parâmetros da variante Bollinger selecionada no painel.
    Public Property BollingerPeriod As Integer = 20
    Public Property BollingerStandardDeviations As Decimal = 3D
    Public Property BollingerCandleMinutes As Integer = 1440

    ' Parâmetros da adaptação experimental da abordagem contrária à média
    ' atribuída ao BNF. O padrão usa SMA de 25 candles e desvio mínimo de 20%.
    Public Property BnfMovingAveragePeriod As Integer = 25
    Public Property BnfMinimumDeviationPercent As Decimal = 20D

    ' Parâmetros da estratégia da onda/relógio.
    Public Property WaveEmaPeriod As Integer = 34
    Public Property WaveSlopeLookback As Integer = 5
    Public Property WaveTouchTolerancePercent As Decimal = 0.35D
    Public Property WaveMinimumSlopeDegrees As Decimal = 15D
    Public Property WaveStrongSlopeDegrees As Decimal = 45D
    Public Property WaveMinimumRiskReward As Decimal = 2D
    Public Property WaveStructureLookback As Integer = 20

    ' Parâmetros da carteira virtual usada pelo modo de simulação.
    Public Property SimulationStartingQuote As Decimal = 1000D
    Public Property SimulationAllocationPercent As Decimal = 10D
    Public Property SimulationFeeRate As Decimal = 0.001D
    ' Slippage adverso aplicado às execuções simuladas: compra paga acima da
    ' cotação observada e venda recebe abaixo dela.
    Public Property SimulationSlippagePercent As Decimal = 0.05D
    Public Property TakeProfitPercent As Decimal = 2D
    Public Property StopLossPercent As Decimal = 1D
    Public Property BreakEvenEnabled As Boolean = True
    Public Property BreakEvenTriggerPercent As Decimal = 1.5D
    Public Property BreakEvenOffsetPercent As Decimal = 0.1D
    Public Property TrailingStopEnabled As Boolean = False
    Public Property TrailingStopDistancePercent As Decimal = 1D

    ' Freio de perdas da carteira virtual. O freio é uma camada externa às
    ' estratégias: ele bloqueia novas entradas sem alterar os sinais. Fechamentos
    ' manuais não entram na sequência de perdas que dispara o freio.
    Public Property RiskBrakeEnabled As Boolean = True
    Public Property RiskBrakeSessionLossPercent As Decimal = 2D
    Public Property RiskBrakeProfitLockEnabled As Boolean = True
    Public Property RiskBrakeProfitLockTriggerPercent As Decimal = 1D
    Public Property RiskBrakeProfitGivebackPercent As Decimal = 50D
    Public Property RiskBrakeMaxConsecutiveLosses As Integer = 3
    Public Property RiskBrakePauseMinutes As Decimal = 60D
    Public Property RiskBrakeCancelPendingEntries As Boolean = True
    Public Property RiskBrakeClosePositionsOnHardStop As Boolean = True
    Public Property RiskBrakeUseUnrealizedPnl As Boolean = True

    ' Parâmetros da grade adaptativa v2. O espaçamento é calculado pela
    ' volatilidade recente e limitado para evitar uma grade apertada demais
    ' ou distante demais.
    Public Property GridV2Lookback As Integer = 48
    Public Property GridV2VolatilityPeriod As Integer = 14
    Public Property GridV2SpacingMinimumPercent As Decimal = 0.8D
    Public Property GridV2SpacingMaximumPercent As Decimal = 2.5D
    Public Property GridV2MaxRangePercent As Decimal = 12D
    Public Property GridV2MaxSlopePercent As Decimal = 2D
    Public Property GridV2MaxLevels As Integer = 5
    Public Property GridV2LevelAllocationPercent As Decimal = 10D
    Public Property GridV2MaxExposurePercent As Decimal = 30D
    ' Limite de inventário acumulado por ativo, em relação ao saldo inicial.
    ' O cálculo usa GridV2MaxExposurePercent como teto por posição.
    Public Property GridV2MaximumHoldingMinutes As Integer = 240
    Public Property GridV2RegimeExitEnabled As Boolean = True
    Public Property GridV2ProgressiveExposureEnabled As Boolean = True
    Public Property GridV2ProgressiveExposureReductionPercent As Decimal = 25D
    ' Saída híbrida: realiza parte no primeiro alvo e protege o restante.
    Public Property GridV2PartialExitPercent As Decimal = 50D
    Public Property GridV2RegimeReductionPercent As Decimal = 50D
    Public Property GridV2TimeReductionPercent As Decimal = 50D
    ' Quando ativo, o Grid V2 mantém a operação normal fora da queda leve do BTC
    ' e exige o pullback defensivo adicional somente enquanto o regime estiver ativo.
    Public Property GridV2BtcMildDeclineEnabled As Boolean = False
    Public Property GridV2BtcMildDeclineMinimumPercent As Decimal = 0.5D
    Public Property GridV2BtcMildDeclineMaximumPercent As Decimal = 2D
    Public Property GridV2BreakoutAtrMultiplier As Decimal = 0.5D
    Public Property GridV2MinimumStopPercent As Decimal = 1.5D
    Public Property GridV2StopVolatilityMultiplier As Decimal = 1.5D
    Public Property GridV2MaximumStopPercent As Decimal = 3D
    Public Property GridV2EmaFilterEnabled As Boolean = True
    Public Property GridV2FastEmaPeriod As Integer = 9
    Public Property GridV2SlowEmaPeriod As Integer = 21
    Public Property GridV2EmaMaxSpreadPercent As Decimal = 1.5D
    Public Property GridV2EmaMaxSlopePercent As Decimal = 0.5D
    Public Property GridV2UseNearestSupportStop As Boolean = True
    Public Property GridV2SupportBufferAtr As Decimal = 1D
    ' Ignora entradas quando o suporte estrutural está praticamente no preço,
    ' pois nesse caso o stop precisaria ser artificialmente apertado.
    Public Property GridV2MinimumSupportDistancePercent As Decimal = 0.8D
    ' Evita encerrar a posição no primeiro tick abaixo do SL. O stop é
    ' confirmado por candles fechados de 1 minuto, mantendo uma saída de
    ' emergência para rompimentos rápidos.
    Public Property GridV2StopConfirmationEnabled As Boolean = True
    Public Property GridV2StopConfirmationCandles As Integer = 2
    Public Property GridV2StopEmergencyBufferPercent As Decimal = 0.5D
    ' A grade só entra depois que o candle toca o nível e confirma rejeição.
    Public Property GridV2RequireRejection As Boolean = True
    ' Risco máximo estimado por nível, calculado contra o stop estrutural.
    Public Property GridV2RiskPerTradePercent As Decimal = 0.5D
    ' Reserva de caixa que não pode ser consumida pela grade.
    Public Property GridV2ReserveQuotePercent As Decimal = 20D

    ' Proteção macro contra flush do BTC. A detecção usa candles de 1 minuto,
    ' queda acumulada, candle vermelho e expansão de volume.
    Public Property BitcoinFlushProtectionEnabled As Boolean = True
    Public Property BitcoinFlushLookbackMinutes As Integer = 5
    Public Property BitcoinFlushDropPercent As Decimal = 2D
    Public Property BitcoinFlushCandleDropPercent As Decimal = 1.5D
    Public Property BitcoinFlushVolumeMultiplier As Decimal = 1.8D
    Public Property BitcoinFlushVolumeLookback As Integer = 20
    Public Property BitcoinFlushPauseMinutes As Decimal = 30D
    ' Proteção diária do BTC. A queda é comparada ao fechamento diário anterior
    ' e o drawdown mede a distância da máxima do dia até o preço atual.
    Public Property BitcoinDailyProtectionEnabled As Boolean = True
    Public Property BitcoinDailyDropPercent As Decimal = 4D
    Public Property BitcoinDailyDrawdownPercent As Decimal = 5D

    ' Parâmetros do pullback em zona de valor. A estratégia usa M5 para
    ' contexto, M2 para o retorno à zona e M1 para a confirmação da entrada.
    Public Property ValueZoneFastEmaPeriod As Integer = 9
    Public Property ValueZoneSlowEmaPeriod As Integer = 20
    Public Property ValueZoneAtrPeriod As Integer = 14
    Public Property ValueZoneToleranceAtr As Decimal = 0.35D
    Public Property ValueZoneMinimumRiskReward As Decimal = 1.5D
    Public Property ValueZoneStructureLookback As Integer = 20
    Public Property ValueZoneRequireVwap As Boolean = True

    ' O motor começa sempre em simulação. A execução real será conectada
    ' depois de validarmos os sinais e os limites de risco.
    Public Property SimulationOnly As Boolean = True

    Public Property UniverseMode As SpotBotUniverseMode = SpotBotUniverseMode.Manual
    Public Property UniverseMaxAssets As Integer = 20
    Public Property UniverseMinimumQuoteVolume As Decimal = 1000000D
    Public Property UniverseMinimumTrades24h As Integer = 1000
    Public Property ManualUniverseSymbols As HashSet(Of String) = New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
End Class

Public NotInheritable Class SpotBotUniverseSettings
    Public Property Mode As SpotBotUniverseMode = SpotBotUniverseMode.Manual
    Public Property MaxAssets As Integer = 20
    Public Property MinimumQuoteVolume As Decimal = 1000000D
    Public Property MinimumTrades24h As Integer = 1000
End Class

Public NotInheritable Class SpotBotSignalEventArgs
    Inherits EventArgs

    Public Sub New(signal As SpotBotSignal)
        Me.Signal = signal
    End Sub

    Public ReadOnly Property Signal As SpotBotSignal
End Class

Public NotInheritable Class SpotBotWarmupSummary
    Public Property RequestedSymbols As Integer
    Public Property LoadedSymbols As Integer
    Public Property TotalCandles As Integer
    Public Property Intervals As New List(Of String)()
    Public Property FailedSymbols As New List(Of String)()
End Class

Public NotInheritable Class SpotBotBitcoinFlushStatus
    Public Property IsPaused As Boolean
    Public Property NewFlushDetected As Boolean
    Public Property CurrentPrice As Decimal
    Public Property DropPercent As Decimal
    Public Property CandleDropPercent As Decimal
    Public Property VolumeRatio As Decimal
    Public Property DailyDropPercent As Decimal
    Public Property DailyDrawdownPercent As Decimal
    Public Property DailyRangePercent As Decimal
    Public Property PauseUntilUtc As DateTime
    Public Property Reason As String = String.Empty
End Class

Public NotInheritable Class SpotBotSimulationSnapshot
    Public Property StartingQuote As Decimal
    Public Property QuoteBalance As Decimal
    Public Property OpenValue As Decimal
    Public Property RealizedPnl As Decimal
    Public Property UnrealizedPnl As Decimal
    Public Property Equity As Decimal
    Public Property TotalPnl As Decimal
    Public Property CompletedTrades As Integer
    Public Property WinningTrades As Integer
    Public Property LosingTrades As Integer
    Public Property OpenPositions As Integer
    Public Property LastAction As String = String.Empty
    Public Property RiskBrakeState As SpotBotRiskBrakeState = SpotBotRiskBrakeState.Normal
    ' P/L realizado manualmente é preservado fora dos gatilhos do freio.
    Public Property RiskBrakeManualRealizedPnl As Decimal
    Public Property RiskBrakePeakEquity As Decimal
    Public Property RiskBrakeConsecutiveLosses As Integer
    Public Property RiskBrakePauseUntilUtc As DateTime
    Public Property RiskBrakeReason As String = String.Empty
    Public Property RiskBrakeBlockedEntries As Integer
    Public Property RiskBrakeEstimatedAvoidedPnl As Decimal
    Public Property RiskBrakePauseCount As Integer
    Public Property RiskBrakeLastPauseStartedUtc As DateTime
    Public Property RiskBrakeLastPauseEndedUtc As DateTime

    Public ReadOnly Property WinRatePercent As Decimal
        Get
            If CompletedTrades <= 0 Then Return 0D
            Return WinningTrades * 100D / CompletedTrades
        End Get
    End Property
End Class

Public NotInheritable Class SpotBotSimulationTrade
    Public Property Symbol As String = String.Empty
    Public Property Strategy As String = String.Empty
    Public Property ExitReason As String = String.Empty
    Public Property Quantity As Decimal
    Public Property EntryPrice As Decimal
    Public Property ExitPrice As Decimal
    Public Property CostBasis As Decimal
    Public Property GrossProceeds As Decimal
    Public Property Fees As Decimal
    Public Property Pnl As Decimal
    Public Property SupportPrice As Decimal
    Public Property InitialStopPrice As Decimal
    Public Property ExitStopPrice As Decimal
    Public Property OpenedAtUtc As DateTime
    Public Property ClosedAtUtc As DateTime
End Class

Public NotInheritable Class SpotBotGridLevel
    Public Property EntryPrice As Decimal
    Public Property Quantity As Decimal
    Public Property CostBasis As Decimal
    Public Property TakeProfitPrice As Decimal
    Public Property StopPrice As Decimal
    Public Property PartialTargetTaken As Boolean
    Public Property RegimeReductionTaken As Boolean
    Public Property TimeReductionTaken As Boolean
    Public Property OpenedAtUtc As DateTime
End Class

Public NotInheritable Class SpotBotOpenPosition
    Public Property Symbol As String = String.Empty
    Public Property Strategy As String = String.Empty
    Public Property Side As String = "LONG"
    Public Property Quantity As Decimal
    Public Property EntryPrice As Decimal
    Public Property CurrentPrice As Decimal
    Public Property CostBasis As Decimal
    Public Property Notional As Decimal
    Public Property UnrealizedPnl As Decimal
    Public Property RoiPercent As Decimal
    Public Property SupportPrice As Decimal
    Public Property InitialStopPrice As Decimal
    Public Property StopPrice As Decimal
    Public Property TakeProfitPrice As Decimal
    Public Property HighestPrice As Decimal
    Public Property GridManaged As Boolean
    Public Property GridLevelCount As Integer
    Public Property GridAnchorPrice As Decimal
    Public Property LastGridEntryPrice As Decimal
    Public Property GridSpacingPercent As Decimal
    Public Property GridLevels As New List(Of SpotBotGridLevel)()
    Public Property OpenedAtUtc As DateTime
End Class

Public NotInheritable Class SpotBotGridGuardState
    Public Property Symbol As String = String.Empty
    Public Property ConsecutiveStops As Integer
    Public Property RecoveryEntryPrice As Decimal
    Public Property BlockedUntilUtc As DateTime
End Class

Public NotInheritable Class SpotBotPersistedSimulationState
    Public Property RunId As Long
    Public Property Status As String = String.Empty
    Public Property Snapshot As SpotBotSimulationSnapshot
    Public Property Positions As New List(Of SpotBotOpenPosition)
    Public Property GridGuards As New List(Of SpotBotGridGuardState)
    Public Property EnabledStrategies As New HashSet(Of SpotBotStrategyKind)()
    Public Property CombinationMode As SpotBotCombinationMode = SpotBotCombinationMode.MinimumConfirmations
    Public Property RequiredConfirmations As Integer = 2
    Public Property BollingerPeriod As Integer = 20
    Public Property BollingerStandardDeviations As Decimal = 3D
    Public Property BollingerCandleMinutes As Integer = 1440
    Public Property AllocationPercent As Decimal = 10D
    Public Property FeeRate As Decimal = 0.001D
    Public Property SlippagePercent As Decimal = 0.05D
    Public Property TakeProfitPercent As Decimal = 2D
    Public Property StopLossPercent As Decimal = 1D
    Public Property BreakEvenEnabled As Boolean = True
    Public Property BreakEvenTriggerPercent As Decimal = 1.5D
    Public Property BreakEvenOffsetPercent As Decimal = 0.1D
    Public Property TrailingStopEnabled As Boolean = False
    Public Property TrailingStopDistancePercent As Decimal = 1D
    Public Property RiskBrakeEnabled As Boolean = True
    Public Property RiskBrakeSessionLossPercent As Decimal = 2D
    Public Property RiskBrakeProfitLockEnabled As Boolean = True
    Public Property RiskBrakeProfitLockTriggerPercent As Decimal = 1D
    Public Property RiskBrakeProfitGivebackPercent As Decimal = 50D
    Public Property RiskBrakeMaxConsecutiveLosses As Integer = 3
    Public Property RiskBrakePauseMinutes As Decimal = 60D
    Public Property RiskBrakeClosePositionsOnHardStop As Boolean = True
    Public Property RiskBrakeUseUnrealizedPnl As Boolean = True
    Public Property PostExitCooldownMinutes As Decimal = 5D
    Public Property NewPositionProfitGateEnabled As Boolean = True
    Public Property MinimumNetProfitBeforeNewPositionPercent As Decimal = 0.5D
    Public Property GridV2UseNearestSupportStop As Boolean = True
    Public Property GridV2SupportBufferAtr As Decimal = 1D
    Public Property GridV2MinimumStopPercent As Decimal = 1.5D
    Public Property GridV2MaximumStopPercent As Decimal = 3D
    Public Property GridV2MinimumSupportDistancePercent As Decimal = 0.8D
    Public Property GridV2StopConfirmationEnabled As Boolean = True
    Public Property GridV2StopConfirmationCandles As Integer = 2
    Public Property GridV2StopEmergencyBufferPercent As Decimal = 0.5D
    Public Property GridV2MaxExposurePercent As Decimal = 30D
    Public Property GridV2RequireRejection As Boolean = True
    Public Property GridV2RiskPerTradePercent As Decimal = 0.5D
    Public Property GridV2ReserveQuotePercent As Decimal = 20D
    Public Property GridV2MaxLevels As Integer = 5
    Public Property GridV2MaximumHoldingMinutes As Integer = 240
    Public Property GridV2RegimeExitEnabled As Boolean = True
    Public Property GridV2ProgressiveExposureEnabled As Boolean = True
    Public Property GridV2ProgressiveExposureReductionPercent As Decimal = 25D
    Public Property GridV2PartialExitPercent As Decimal = 50D
    Public Property GridV2RegimeReductionPercent As Decimal = 50D
    Public Property GridV2TimeReductionPercent As Decimal = 50D
    Public Property GridV2BtcMildDeclineEnabled As Boolean = False
    Public Property GridV2BtcMildDeclineMinimumPercent As Decimal = 0.5D
    Public Property GridV2BtcMildDeclineMaximumPercent As Decimal = 2D
    Public Property BitcoinFlushProtectionEnabled As Boolean = True
    Public Property BitcoinFlushLookbackMinutes As Integer = 5
    Public Property BitcoinFlushDropPercent As Decimal = 2D
    Public Property BitcoinFlushCandleDropPercent As Decimal = 1.5D
    Public Property BitcoinFlushVolumeMultiplier As Decimal = 1.8D
    Public Property BitcoinFlushVolumeLookback As Integer = 20
    Public Property BitcoinFlushPauseMinutes As Decimal = 30D
    Public Property BitcoinDailyProtectionEnabled As Boolean = True
    Public Property BitcoinDailyDropPercent As Decimal = 4D
    Public Property BitcoinDailyDrawdownPercent As Decimal = 5D
    Public Property UniverseMinimumTrades24h As Integer = 1000
End Class

Public NotInheritable Class SpotBotSimulationEventArgs
    Inherits EventArgs

    Public Sub New(snapshot As SpotBotSimulationSnapshot)
        Me.Snapshot = snapshot
    End Sub

    Public ReadOnly Property Snapshot As SpotBotSimulationSnapshot
End Class

Public Interface ISpotBotStrategy
    ReadOnly Property Kind As SpotBotStrategyKind

    Function Evaluate(
        symbol As String,
        prices As IReadOnlyList(Of SpotBotPricePoint),
        configuration As SpotBotConfiguration) As SpotBotStrategySignal
End Interface

Public Interface ISpotBotCandleStrategy
    Inherits ISpotBotStrategy

    Function EvaluateCandles(
        symbol As String,
        candles As IReadOnlyList(Of SpotBotPricePoint),
        configuration As SpotBotConfiguration) As SpotBotStrategySignal
End Interface

''' <summary>
''' Estratégia que precisa de candles fechados em mais de um timeframe.
''' O motor fornece as séries já agregadas para evitar misturar ticks com
''' candles de contexto e para manter a confirmação sem look-ahead.
''' </summary>
Public Interface ISpotBotMultiTimeframeStrategy
    Inherits ISpotBotStrategy

    Function EvaluateMultiTimeframe(
        symbol As String,
        m1Candles As IReadOnlyList(Of SpotBotPricePoint),
        m2Candles As IReadOnlyList(Of SpotBotPricePoint),
        m5Candles As IReadOnlyList(Of SpotBotPricePoint),
        configuration As SpotBotConfiguration) As SpotBotStrategySignal
End Interface

''' <summary>
''' Permite que estratégias que continuam avaliando ticks usem uma série de
''' candles fechados apenas como filtro de regime.
''' </summary>
Public Interface ISpotBotTrendFilterStrategy
    Inherits ISpotBotStrategy

    Function EvaluateWithTrend(
        symbol As String,
        prices As IReadOnlyList(Of SpotBotPricePoint),
        trendCandles As IReadOnlyList(Of SpotBotPricePoint),
        configuration As SpotBotConfiguration) As SpotBotStrategySignal
End Interface

''' <summary>
''' Extensão do filtro de tendência que também recebe o contexto diário do BTC.
''' Usada pelo Grid V2 para alternar o filtro defensivo durante a queda leve
''' do BTC sem criar uma estratégia concorrente na combinação de sinais.
''' </summary>
Public Interface ISpotBotBitcoinTrendFilterStrategy
    Inherits ISpotBotTrendFilterStrategy

    Function EvaluateWithTrendAndBitcoinContext(
        symbol As String,
        prices As IReadOnlyList(Of SpotBotPricePoint),
        trendCandles As IReadOnlyList(Of SpotBotPricePoint),
        pullbackM1Candles As IReadOnlyList(Of SpotBotPricePoint),
        pullbackM2Candles As IReadOnlyList(Of SpotBotPricePoint),
        bitcoinDailyCandles As IReadOnlyList(Of SpotBotPricePoint),
        configuration As SpotBotConfiguration) As SpotBotStrategySignal
End Interface
