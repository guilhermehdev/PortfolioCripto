Imports System
Imports System.Collections.Generic
Imports System.Linq

Public NotInheritable Class SpotTradeBotEngine
    Private ReadOnly _sync As New Object()
    Private ReadOnly _strategies As New Dictionary(Of SpotBotStrategyKind, ISpotBotStrategy)()
    Private ReadOnly _priceHistory As New Dictionary(Of String, List(Of SpotBotPricePoint))(StringComparer.OrdinalIgnoreCase)
    Private ReadOnly _rsiCandleHistory As New Dictionary(Of String, List(Of SpotBotPricePoint))(StringComparer.OrdinalIgnoreCase)
    Private ReadOnly _rsiCurrentCandles As New Dictionary(Of String, RsiCandleState)(StringComparer.OrdinalIgnoreCase)
    Private ReadOnly _bollingerCandleHistory As New Dictionary(Of String, List(Of SpotBotPricePoint))(StringComparer.OrdinalIgnoreCase)
    Private ReadOnly _bollingerCurrentCandles As New Dictionary(Of String, CandleState)(StringComparer.OrdinalIgnoreCase)
    Private ReadOnly _waveCandleHistory As New Dictionary(Of String, List(Of SpotBotPricePoint))(StringComparer.OrdinalIgnoreCase)
    Private ReadOnly _waveCurrentCandles As New Dictionary(Of String, CandleState)(StringComparer.OrdinalIgnoreCase)
    Private ReadOnly _valueZoneM2CandleHistory As New Dictionary(Of String, List(Of SpotBotPricePoint))(StringComparer.OrdinalIgnoreCase)
    Private ReadOnly _valueZoneM5CandleHistory As New Dictionary(Of String, List(Of SpotBotPricePoint))(StringComparer.OrdinalIgnoreCase)
    Private ReadOnly _valueZoneM2CurrentCandles As New Dictionary(Of String, CandleState)(StringComparer.OrdinalIgnoreCase)
    Private ReadOnly _valueZoneM5CurrentCandles As New Dictionary(Of String, CandleState)(StringComparer.OrdinalIgnoreCase)
    Private ReadOnly _lastSignals As New Dictionary(Of String, DateTime)(StringComparer.OrdinalIgnoreCase)
    Private ReadOnly _lastExits As New Dictionary(Of String, DateTime)(StringComparer.OrdinalIgnoreCase)
    Private ReadOnly _gridRecoveryEntryPrices As New Dictionary(Of String, Decimal)(StringComparer.OrdinalIgnoreCase)
    Private ReadOnly _simulationPositions As New Dictionary(Of String, SimulationPosition)(StringComparer.OrdinalIgnoreCase)
    Private ReadOnly _configuration As New SpotBotConfiguration()
    Private _simulationQuoteBalance As Decimal
    Private _simulationRealizedPnl As Decimal
    Private _simulationCompletedTrades As Integer
    Private _simulationWinningTrades As Integer
    Private _simulationLosingTrades As Integer
    Private _simulationLastAction As String = String.Empty
    Private _running As Boolean
    Private _activeRunId As Long
    Private _restoredStatePending As Boolean

    Public Event SignalGenerated As EventHandler(Of SpotBotSignalEventArgs)
    Public Event SimulationUpdated As EventHandler(Of SpotBotSimulationEventArgs)

    Public Sub New()
        Try
            SpotTradeBotRepository.Initialize()
        Catch ex As Exception
            Debug.WriteLine("Erro inicializando persistência do Tradebot: " & ex.Message)
        End Try

        RegisterStrategy(New SpotBotEmaCrossoverStrategy())
        RegisterStrategy(New SpotBotRsiStrategy())
        RegisterStrategy(New SpotBotBreakoutStrategy())
        RegisterStrategy(New SpotBotGridStrategy())
        RegisterStrategy(New SpotBotBollingerMeanReversionStrategy())
        RegisterStrategy(New SpotBotBollingerMeanReversionStrategy(SpotBotStrategyKind.BaseOriginalProtected))
        RegisterStrategy(New SpotBotBnfContrarianStrategy())
        RegisterStrategy(New SpotBotWavePullbackStrategy())
        RegisterStrategy(New SpotBotBreakoutRetestStrategy())
        RegisterStrategy(New SpotBotMomentumVolumeStrategy())
        RegisterStrategy(New SpotBotGridRegimeStrategy())
        RegisterStrategy(New SpotBotGridAdaptiveStrategy())
        RegisterStrategy(New SpotBotValueZonePullbackStrategy())

        Dim persistedState As SpotBotPersistedSimulationState = Nothing
        Try
            persistedState = SpotTradeBotRepository.LoadLatestRestorableState()
        Catch ex As Exception
            Debug.WriteLine("Erro carregando a última simulação do Tradebot: " & ex.Message)
        End Try

        SyncLock _sync
            ResetSimulationLocked()
            If persistedState IsNot Nothing Then
                RestoreSimulationLocked(persistedState)
            End If
        End SyncLock
    End Sub

    Public ReadOnly Property Configuration As SpotBotConfiguration
        Get
            Return _configuration
        End Get
    End Property

    Public ReadOnly Property IsRunning As Boolean
        Get
            SyncLock _sync
                Return _running
            End SyncLock
        End Get
    End Property

    Public ReadOnly Property HasRestoredState As Boolean
        Get
            SyncLock _sync
                Return _restoredStatePending AndAlso _simulationPositions.Count > 0
            End SyncLock
        End Get
    End Property

    Public Sub RegisterStrategy(strategy As ISpotBotStrategy)
        If strategy Is Nothing Then Throw New ArgumentNullException(NameOf(strategy))

        SyncLock _sync
            _strategies(strategy.Kind) = strategy
        End SyncLock
    End Sub

    Public Sub SetStrategyEnabled(kind As SpotBotStrategyKind, enabled As Boolean)
        SyncLock _sync
            If enabled Then
                _configuration.EnabledStrategies.Add(kind)
            Else
                _configuration.EnabledStrategies.Remove(kind)
            End If
        End SyncLock
    End Sub

    Public Sub EnableAllStrategies()
        SyncLock _sync
            For Each kind In _strategies.Keys
                _configuration.EnabledStrategies.Add(kind)
            Next
        End SyncLock
    End Sub

    Public Sub DisableAllStrategies()
        SyncLock _sync
            _configuration.EnabledStrategies.Clear()
        End SyncLock
    End Sub

    Public Sub Start()
        Dim configurationSnapshot As SpotBotConfiguration
        Dim runId As Long
        Dim resumeExisting As Boolean

        SyncLock _sync
            If _running Then Return
            _running = True
            configurationSnapshot = CopyConfiguration(_configuration)
            resumeExisting = _restoredStatePending AndAlso _activeRunId > 0
            runId = _activeRunId
        End SyncLock

        Try
            If Not resumeExisting Then
                runId = SpotTradeBotRepository.StartRun(configurationSnapshot)
            End If

            SyncLock _sync
                _activeRunId = runId
                _restoredStatePending = False
            End SyncLock

            If resumeExisting Then
                SpotTradeBotRepository.UpdateRun(
                    runId,
                    GetSimulationSnapshot(),
                    "RUNNING")
                SpotTradeBotRepository.SaveOpenPositions(runId, GetOpenPositions())
            End If
        Catch ex As Exception
            Debug.WriteLine("Erro iniciando persistência da simulação: " & ex.Message)
        End Try
    End Sub

    Public Sub [Stop]()
        Dim runId As Long
        Dim snapshot As SpotBotSimulationSnapshot

        SyncLock _sync
            _running = False
            runId = _activeRunId
            snapshot = BuildSimulationSnapshotLocked(_configuration)
            _activeRunId = 0
            _restoredStatePending = False
        End SyncLock

        If runId <= 0 Then Return

        Try
            SpotTradeBotRepository.SaveOpenPositions(runId, GetOpenPositions())
            SpotTradeBotRepository.UpdateRun(runId, snapshot, "STOPPED", DateTime.UtcNow)
        Catch ex As Exception
            Debug.WriteLine("Erro salvando resultado da simulação: " & ex.Message)
        End Try
    End Sub

    Public Sub ResetHistory(Optional symbol As String = Nothing)
        SyncLock _sync
            If String.IsNullOrWhiteSpace(symbol) Then
                _priceHistory.Clear()
                _rsiCandleHistory.Clear()
                _rsiCurrentCandles.Clear()
                _bollingerCandleHistory.Clear()
                _bollingerCurrentCandles.Clear()
                _waveCandleHistory.Clear()
                _waveCurrentCandles.Clear()
                _valueZoneM2CandleHistory.Clear()
                _valueZoneM5CandleHistory.Clear()
                _valueZoneM2CurrentCandles.Clear()
                _valueZoneM5CurrentCandles.Clear()
                _lastSignals.Clear()
                _lastExits.Clear()
                _gridRecoveryEntryPrices.Clear()
                ResetSimulationLocked()
                If Not _running Then
                    _activeRunId = 0
                    _restoredStatePending = False
                End If
            Else
                Dim normalized = NormalizeSymbol(symbol)
                _priceHistory.Remove(normalized)
                _rsiCandleHistory.Remove(normalized)
                _rsiCurrentCandles.Remove(normalized)
                _bollingerCandleHistory.Remove(normalized)
                _bollingerCurrentCandles.Remove(normalized)
                _waveCandleHistory.Remove(normalized)
                _waveCurrentCandles.Remove(normalized)
                _valueZoneM2CandleHistory.Remove(normalized)
                _valueZoneM5CandleHistory.Remove(normalized)
                _valueZoneM2CurrentCandles.Remove(normalized)
                _valueZoneM5CurrentCandles.Remove(normalized)
                For Each key In _lastSignals.Keys.
                    Where(Function(item) item.StartsWith(normalized & "|", StringComparison.OrdinalIgnoreCase)).
                    ToList()
                    _lastSignals.Remove(key)
                Next
                _lastExits.Remove(normalized)
                _gridRecoveryEntryPrices.Remove(normalized)

                If Not _running Then
                    _activeRunId = 0
                    _restoredStatePending = False
                End If
            End If
        End SyncLock
    End Sub

    Public Function ProcessPrice(
        symbol As String,
        price As Decimal,
        Optional timestampUtc As DateTime = Nothing,
        Optional quoteVolume24h As Decimal = 0D) As SpotBotSignal

        Dim normalizedSymbol = NormalizeSymbol(symbol)
        If String.IsNullOrWhiteSpace(normalizedSymbol) OrElse price <= 0D Then
            Return Nothing
        End If

        Dim observedAt = timestampUtc
        If observedAt = DateTime.MinValue Then
            observedAt = DateTime.UtcNow
        ElseIf observedAt.Kind = DateTimeKind.Unspecified Then
            observedAt = DateTime.SpecifyKind(observedAt, DateTimeKind.Utc)
        End If

        Dim history As List(Of SpotBotPricePoint) = Nothing
        Dim rsiHistory As List(Of SpotBotPricePoint) = Nothing
        Dim bollingerHistory As List(Of SpotBotPricePoint) = Nothing
        Dim valueZoneM2History As List(Of SpotBotPricePoint) = Nothing
        Dim valueZoneM5History As List(Of SpotBotPricePoint) = Nothing
        Dim rsiPriceSnapshot As IReadOnlyList(Of SpotBotPricePoint) = Array.Empty(Of SpotBotPricePoint)()
        Dim bollingerPriceSnapshot As IReadOnlyList(Of SpotBotPricePoint) = Array.Empty(Of SpotBotPricePoint)()
        Dim wavePriceSnapshot As IReadOnlyList(Of SpotBotPricePoint) = Array.Empty(Of SpotBotPricePoint)()
        Dim valueZoneM2PriceSnapshot As IReadOnlyList(Of SpotBotPricePoint) = Array.Empty(Of SpotBotPricePoint)()
        Dim valueZoneM5PriceSnapshot As IReadOnlyList(Of SpotBotPricePoint) = Array.Empty(Of SpotBotPricePoint)()
        Dim rsiClosedCandle As SpotBotPricePoint = Nothing
        Dim bollingerClosedCandle As SpotBotPricePoint = Nothing
        Dim waveClosedCandle As SpotBotPricePoint = Nothing
        Dim closedCandle As SpotBotPricePoint = Nothing
        Dim candleClosed As Boolean
        Dim bollingerCandleClosed As Boolean
        Dim waveCandleClosed As Boolean
        Dim valueZoneM1CandleClosed As Boolean
        Dim valueZoneM2CandleClosed As Boolean
        Dim valueZoneM5CandleClosed As Boolean
        Dim valueZoneM2ClosedCandle As SpotBotPricePoint = Nothing
        Dim valueZoneM5ClosedCandle As SpotBotPricePoint = Nothing
        Dim enabledStrategies As List(Of SpotBotStrategyKind)
        Dim configurationSnapshot As SpotBotConfiguration
        Dim activeRunId As Long

        SyncLock _sync
            If Not _running Then Return Nothing

            If Not _priceHistory.TryGetValue(normalizedSymbol, history) Then
                history = New List(Of SpotBotPricePoint)()
                _priceHistory(normalizedSymbol) = history
            End If

            history.Add(New SpotBotPricePoint(observedAt, price, volume:=quoteVolume24h))
            Dim maximum = Math.Max(30, _configuration.MaximumPriceHistory)
            If history.Count > maximum Then
                history.RemoveRange(0, history.Count - maximum)
            End If

            enabledStrategies = _configuration.EnabledStrategies.ToList()
            If enabledStrategies.Contains(SpotBotStrategyKind.Rsi) OrElse
               enabledStrategies.Contains(SpotBotStrategyKind.ValueZonePullback) OrElse
               enabledStrategies.Contains(SpotBotStrategyKind.GridAdaptive) Then
                rsiHistory = GetOrCreateRsiCandleHistory(normalizedSymbol)
                valueZoneM1CandleClosed = UpdateRsiCandle(normalizedSymbol, observedAt, price, rsiHistory, rsiClosedCandle)
                candleClosed = valueZoneM1CandleClosed
                rsiPriceSnapshot = rsiHistory.ToArray()
            End If

            If enabledStrategies.Contains(SpotBotStrategyKind.BollingerMeanReversion) OrElse
               enabledStrategies.Contains(SpotBotStrategyKind.BaseOriginalProtected) OrElse
               enabledStrategies.Contains(SpotBotStrategyKind.BnfContrarian) Then
                bollingerHistory = GetOrCreateBollingerCandleHistory(normalizedSymbol)
                bollingerCandleClosed = UpdateBollingerCandle(
                    normalizedSymbol,
                    observedAt,
                    price,
                    bollingerHistory,
                    bollingerClosedCandle)
                bollingerPriceSnapshot = bollingerHistory.ToArray()
                candleClosed = candleClosed OrElse bollingerCandleClosed
            End If

            If enabledStrategies.Contains(SpotBotStrategyKind.WavePullback) OrElse
               enabledStrategies.Contains(SpotBotStrategyKind.BreakoutRetest) Then
                Dim waveHistory = GetOrCreateWaveCandleHistory(normalizedSymbol)
                waveCandleClosed = UpdateWaveCandle(
                    normalizedSymbol,
                    observedAt,
                    price,
                    waveHistory,
                    waveClosedCandle,
                    _configuration.BollingerCandleMinutes)
                wavePriceSnapshot = waveHistory.ToArray()
                candleClosed = candleClosed OrElse waveCandleClosed
            End If

            If enabledStrategies.Contains(SpotBotStrategyKind.ValueZonePullback) OrElse
               enabledStrategies.Contains(SpotBotStrategyKind.GridAdaptive) Then
                valueZoneM2History = GetOrCreateValueZoneCandleHistory(normalizedSymbol, 2)
                valueZoneM5History = GetOrCreateValueZoneCandleHistory(normalizedSymbol, 5)
                valueZoneM2CandleClosed = UpdateValueZoneCandle(
                    normalizedSymbol,
                    observedAt,
                    price,
                    valueZoneM2History,
                    valueZoneM2ClosedCandle,
                    2)
                valueZoneM5CandleClosed = UpdateValueZoneCandle(
                    normalizedSymbol,
                    observedAt,
                    price,
                    valueZoneM5History,
                    valueZoneM5ClosedCandle,
                    5)
                valueZoneM2PriceSnapshot = valueZoneM2History.ToArray()
                valueZoneM5PriceSnapshot = valueZoneM5History.ToArray()
                candleClosed = candleClosed OrElse valueZoneM2CandleClosed OrElse valueZoneM5CandleClosed
            End If

            closedCandle = If(rsiClosedCandle, If(bollingerClosedCandle, waveClosedCandle))

            configurationSnapshot = CopyConfiguration(_configuration)
            activeRunId = _activeRunId
        End SyncLock

        If enabledStrategies.Count = 0 Then Return Nothing

        Dim priceSnapshot As IReadOnlyList(Of SpotBotPricePoint) = history.ToArray()
        Dim strategySignals As New List(Of SpotBotStrategySignal)()

        For Each kind In enabledStrategies
            Dim strategy As ISpotBotStrategy = Nothing
            SyncLock _sync
                _strategies.TryGetValue(kind, strategy)
            End SyncLock

            If strategy IsNot Nothing Then
                Dim strategyPrices As IReadOnlyList(Of SpotBotPricePoint) = priceSnapshot
                If kind = SpotBotStrategyKind.Rsi Then
                    strategyPrices = rsiPriceSnapshot
                ElseIf kind = SpotBotStrategyKind.BollingerMeanReversion OrElse
                       kind = SpotBotStrategyKind.BaseOriginalProtected OrElse
                       kind = SpotBotStrategyKind.BnfContrarian Then
                    Dim bollingerEvaluation = bollingerPriceSnapshot.ToList()
                    If bollingerEvaluation.Count > 0 Then
                        ' O candle em formação representa o preço atual. A
                        ' estratégia só usa candles fechados para a banda, mas
                        ' aceita o rompimento intrabar do extremo marcado.
                        bollingerEvaluation.Add(New SpotBotPricePoint(observedAt, price))
                    End If
                    strategyPrices = bollingerEvaluation
                End If

                Dim strategySignal As SpotBotStrategySignal
                If kind = SpotBotStrategyKind.GridAdaptive Then
                    Dim trendFilterStrategy = TryCast(strategy, ISpotBotTrendFilterStrategy)
                    If trendFilterStrategy IsNot Nothing Then
                        strategySignal = trendFilterStrategy.EvaluateWithTrend(
                            normalizedSymbol,
                            strategyPrices,
                            valueZoneM5PriceSnapshot,
                            configurationSnapshot)
                    Else
                        strategySignal = strategy.Evaluate(normalizedSymbol, strategyPrices, configurationSnapshot)
                    End If
                ElseIf kind = SpotBotStrategyKind.ValueZonePullback Then
                    If Not valueZoneM1CandleClosed Then Continue For
                    Dim multiTimeframeStrategy = TryCast(strategy, ISpotBotMultiTimeframeStrategy)
                    If multiTimeframeStrategy Is Nothing Then Continue For
                    strategySignal = multiTimeframeStrategy.EvaluateMultiTimeframe(
                        normalizedSymbol,
                        rsiPriceSnapshot,
                        valueZoneM2PriceSnapshot,
                        valueZoneM5PriceSnapshot,
                        configurationSnapshot)
                ElseIf kind = SpotBotStrategyKind.WavePullback OrElse
                   kind = SpotBotStrategyKind.BreakoutRetest Then
                    If Not waveCandleClosed Then Continue For
                    Dim candleStrategy = TryCast(strategy, ISpotBotCandleStrategy)
                    If candleStrategy Is Nothing Then Continue For
                    strategySignal = candleStrategy.EvaluateCandles(
                        normalizedSymbol,
                        wavePriceSnapshot,
                        configurationSnapshot)
                Else
                    strategySignal = strategy.Evaluate(normalizedSymbol, strategyPrices, configurationSnapshot)
                End If
                If strategySignal IsNot Nothing Then
                    strategySignals.Add(strategySignal)
                End If
            End If
        Next

        Dim signal = CombineSignals(normalizedSymbol, price, observedAt, strategySignals, configurationSnapshot)
        Dim acceptedSignal As SpotBotSignal = Nothing

        If signal IsNot Nothing AndAlso signal.Direction <> SpotBotSignalDirection.Hold Then
            Dim cooldownKey = $"{normalizedSymbol}|{signal.Direction}"
            Dim cooldownBlocked = False

            SyncLock _sync
                Dim previousSignalAt As DateTime
                cooldownBlocked = _lastSignals.TryGetValue(cooldownKey, previousSignalAt) AndAlso
                                  observedAt - previousSignalAt < configurationSnapshot.Cooldown

                If Not cooldownBlocked AndAlso signal.Direction = SpotBotSignalDirection.Buy Then
                    Dim lastExitAt As DateTime
                    Dim postExitCooldown = TimeSpan.FromMinutes(
                        CDbl(Math.Max(0D, configurationSnapshot.PostExitCooldownMinutes)))
                    cooldownBlocked = postExitCooldown > TimeSpan.Zero AndAlso
                                      _lastExits.TryGetValue(normalizedSymbol, lastExitAt) AndAlso
                                      observedAt >= lastExitAt AndAlso
                                      observedAt - lastExitAt < postExitCooldown
                End If

                If Not cooldownBlocked AndAlso signal.Direction = SpotBotSignalDirection.Buy AndAlso signal.GridManaged Then
                    Dim recoveryEntryPrice As Decimal
                    If _gridRecoveryEntryPrices.TryGetValue(normalizedSymbol, recoveryEntryPrice) Then
                        If IsGridRecoveryConfirmed(price, rsiPriceSnapshot, recoveryEntryPrice) Then
                            _gridRecoveryEntryPrices.Remove(normalizedSymbol)
                        Else
                            cooldownBlocked = True
                            Debug.WriteLine($"[GRID V2] Reentrada bloqueada em {normalizedSymbol}: aguardando recuperação confirmada após stop.")
                        End If
                    End If
                End If

                If Not cooldownBlocked Then
                    _lastSignals(cooldownKey) = observedAt
                    acceptedSignal = signal
                End If
            End SyncLock
        End If

        Dim completedTrade As SpotBotSimulationTrade = Nothing
        Dim automaticExit As Boolean
        Dim stopAdjusted As Boolean
        Dim simulationSnapshot = ApplySimulation(
            normalizedSymbol,
            price,
            observedAt,
            acceptedSignal,
            configurationSnapshot,
            completedTrade,
            automaticExit,
            stopAdjusted)
        RaiseEvent SimulationUpdated(Me, New SpotBotSimulationEventArgs(simulationSnapshot))

        If activeRunId > 0 Then
            Try
                If candleClosed AndAlso closedCandle IsNot Nothing Then
                    SpotTradeBotRepository.SaveCandle(
                        activeRunId,
                        normalizedSymbol,
                        closedCandle.TimestampUtc,
                        closedCandle.Price)
                End If

                If completedTrade IsNot Nothing Then
                    SpotTradeBotRepository.SaveTrade(activeRunId, completedTrade)
                End If

                If candleClosed OrElse completedTrade IsNot Nothing OrElse acceptedSignal IsNot Nothing OrElse stopAdjusted Then
                    SpotTradeBotRepository.UpdateRun(activeRunId, simulationSnapshot, "RUNNING")
                    SpotTradeBotRepository.SaveOpenPositions(activeRunId, GetOpenPositions())
                End If
            Catch ex As Exception
                Debug.WriteLine("Erro salvando dados da simulação: " & ex.Message)
            End Try
        End If

        If automaticExit Then
            acceptedSignal = Nothing
        End If

        If acceptedSignal Is Nothing Then Return Nothing

        RaiseEvent SignalGenerated(Me, New SpotBotSignalEventArgs(acceptedSignal))
        Return acceptedSignal
    End Function

    Private Shared Function IsGridRecoveryConfirmed(
        currentPrice As Decimal,
        m1Candles As IReadOnlyList(Of SpotBotPricePoint),
        previousEntryPrice As Decimal) As Boolean

        If currentPrice <= 0D OrElse previousEntryPrice <= 0D OrElse
           m1Candles Is Nothing OrElse m1Candles.Count < 3 Then
            Return False
        End If

        Dim latest = m1Candles(m1Candles.Count - 1)
        Dim previous = m1Candles(m1Candles.Count - 2)
        Dim beforePrevious = m1Candles(m1Candles.Count - 3)
        If latest Is Nothing OrElse previous Is Nothing OrElse beforePrevious Is Nothing Then Return False

        ' A recuperação exige três fechamentos ascendentes, candle atual de
        ' alta, preço acima da entrada que sofreu stop e cotação atual sem
        ' perder o último fechamento confirmado.
        If Not latest.IsBullish OrElse
           latest.Close <= previous.Close OrElse
           previous.Close <= beforePrevious.Close OrElse
           latest.Close <= previousEntryPrice OrElse
           currentPrice < latest.Close Then
            Return False
        End If

        If m1Candles.Count < 9 Then Return True

        Dim ema9 = CalculateRecoveryEma(m1Candles, 9)
        Return ema9 > 0D AndAlso latest.Close > ema9
    End Function

    Private Shared Function CalculateRecoveryEma(
        candles As IReadOnlyList(Of SpotBotPricePoint),
        period As Integer) As Decimal

        If candles Is Nothing OrElse candles.Count = 0 Then Return 0D
        Dim smoothing = 2D / (period + 1D)
        Dim ema = candles(0).Close
        For index = 1 To candles.Count - 1
            ema = (candles(index).Close - ema) * smoothing + ema
        Next
        Return ema
    End Function

    Private Function ApplySimulation(
        symbol As String,
        price As Decimal,
        timestampUtc As DateTime,
        signal As SpotBotSignal,
        configuration As SpotBotConfiguration,
        ByRef completedTrade As SpotBotSimulationTrade,
        ByRef automaticExit As Boolean,
        ByRef stopAdjusted As Boolean) As SpotBotSimulationSnapshot

        SyncLock _sync
            automaticExit = False
            stopAdjusted = False
            Dim position As SimulationPosition = Nothing
            If _simulationPositions.TryGetValue(symbol, position) Then
                position.LastPrice = price
                position.HighestPrice = Math.Max(position.HighestPrice, price)
            End If

            If position IsNot Nothing AndAlso position.EntryPrice > 0D Then
                stopAdjusted = UpdateDynamicStop(position, price, configuration)
                Dim variationPercent = (price - position.EntryPrice) / position.EntryPrice * 100D
                Dim takeProfitReached = position.TakeProfitPrice > 0D AndAlso price >= position.TakeProfitPrice
                Dim stopLossReached = position.StopPrice > 0D AndAlso price <= position.StopPrice
                Dim takeProfitPercent = Math.Max(0D, configuration.TakeProfitPercent)
                Dim stopLossPercent = Math.Max(0D, configuration.StopLossPercent)

                If takeProfitReached OrElse
                   (Not position.GridManaged AndAlso position.TakeProfitPrice <= 0D AndAlso takeProfitPercent > 0D AndAlso variationPercent >= takeProfitPercent) Then
                    CloseSimulationPosition(
                        symbol,
                        price,
                        timestampUtc,
                        position,
                        configuration,
                        "TP",
                        completedTrade)
                    automaticExit = True
                    Return BuildSimulationSnapshotLocked(configuration)
                End If

                If stopLossReached OrElse
                   (Not position.GridManaged AndAlso position.StopPrice <= 0D AndAlso stopLossPercent > 0D AndAlso variationPercent <= -stopLossPercent) Then
                    CloseSimulationPosition(
                        symbol,
                        price,
                        timestampUtc,
                        position,
                        configuration,
                        "SL",
                        completedTrade)
                    automaticExit = True
                    Return BuildSimulationSnapshotLocked(configuration)
                End If
            End If

            If signal IsNot Nothing AndAlso signal.SimulationOnly Then
                Select Case signal.Direction
                    Case SpotBotSignalDirection.Buy
                        If position Is Nothing Then
                            Dim feeRate = Math.Max(0D, configuration.SimulationFeeRate)
                            Dim isGridSignal = signal.GridManaged
                            Dim allocationPercent = Math.Min(100D, Math.Max(0D, configuration.SimulationAllocationPercent))
                            Dim requestedNotional = If(
                                isGridSignal,
                                configuration.SimulationStartingQuote * allocationPercent / 100D,
                                _simulationQuoteBalance * allocationPercent / 100D)
                            Dim maxExposure = If(
                                isGridSignal,
                                configuration.SimulationStartingQuote * Math.Min(100D, Math.Max(0D, configuration.GridV2MaxExposurePercent)) / 100D,
                                Decimal.MaxValue)
                            Dim notional = Math.Min(requestedNotional, Math.Min(_simulationQuoteBalance / (1D + feeRate), maxExposure))

                            If notional > 0D Then
                                Dim fee = notional * feeRate
                                Dim executionPrice = If(signal.EntryPrice > 0D, signal.EntryPrice, price)
                                position = New SimulationPosition With {
                                    .Symbol = symbol,
                                    .Strategy = ResolveSignalStrategy(signal),
                                    .Quantity = notional / executionPrice,
                                    .EntryPrice = executionPrice,
                                    .CostBasis = notional + fee,
                                    .LastPrice = executionPrice,
                                     .HighestPrice = executionPrice,
                                    .StopPrice = signal.StopPrice,
                                    .TakeProfitPrice = signal.TakeProfitPrice,
                                    .GridManaged = isGridSignal,
                                    .GridLevelCount = If(isGridSignal, 1, 0),
                                    .EntryTimestampUtc = signal.TimestampUtc
                                }
                                _simulationPositions(symbol) = position
                                _simulationQuoteBalance -= notional + fee
                                _simulationLastAction = $"COMPRA simulada {symbol}: {position.Quantity:0.########} @ {price:0.########}"
                            End If
                        ElseIf signal.GridManaged AndAlso position.GridManaged AndAlso
                               position.GridLevelCount < Math.Max(1, configuration.GridV2MaxLevels) Then
                            Dim feeRate = Math.Max(0D, configuration.SimulationFeeRate)
                            Dim maxExposure = configuration.SimulationStartingQuote * Math.Min(100D, Math.Max(0D, configuration.GridV2MaxExposurePercent)) / 100D
                            Dim allocationPercent = Math.Min(100D, Math.Max(0D, configuration.SimulationAllocationPercent))
                            Dim requestedNotional = configuration.SimulationStartingQuote * allocationPercent / 100D
                            Dim availableExposure = Math.Max(0D, maxExposure - position.CostBasis)
                            Dim notional = Math.Min(requestedNotional, Math.Min(_simulationQuoteBalance / (1D + feeRate), availableExposure))
                            If notional > 0D Then
                                Dim fee = notional * feeRate
                                Dim executionPrice = If(signal.EntryPrice > 0D, signal.EntryPrice, price)
                                Dim addedQuantity = notional / executionPrice
                                Dim previousQuantity = position.Quantity
                                position.Quantity += addedQuantity
                                position.EntryPrice = If(position.Quantity <= 0D,
                                                         executionPrice,
                                                         ((previousQuantity * position.EntryPrice) + (addedQuantity * executionPrice)) / position.Quantity)
                                position.CostBasis += notional + fee
                                position.LastPrice = executionPrice
                                 position.HighestPrice = Math.Max(position.HighestPrice, executionPrice)
                                If signal.StopPrice > 0D Then
                                    position.StopPrice = If(position.StopPrice <= 0D, signal.StopPrice, Math.Min(position.StopPrice, signal.StopPrice))
                                End If
                                position.GridLevelCount += 1
                                _simulationQuoteBalance -= notional + fee
                                _simulationLastAction = $"COMPRA grid nível {position.GridLevelCount} {symbol}: {addedQuantity:0.########} @ {price:0.########}"
                            End If
                        Else
                            _simulationLastAction = $"COMPRA ignorada {symbol}: posição já aberta"
                        End If

                    Case SpotBotSignalDirection.Sell
                        If position IsNot Nothing Then
                            If signal.GridManaged AndAlso position.GridManaged AndAlso position.GridLevelCount > 1 Then
                                CloseSimulationPositionPartially(symbol, price, timestampUtc, position, configuration, completedTrade)
                            Else
                                CloseSimulationPosition(symbol, price, timestampUtc, position, configuration, "VENDA", completedTrade)
                            End If
                        Else
                            _simulationLastAction = $"VENDA ignorada {symbol}: sem posição aberta"
                        End If
                End Select
            End If

            Return BuildSimulationSnapshotLocked(configuration)
        End SyncLock
    End Function

    Private Function UpdateDynamicStop(
        position As SimulationPosition,
        price As Decimal,
        configuration As SpotBotConfiguration) As Boolean

        If position Is Nothing OrElse position.EntryPrice <= 0D OrElse price <= 0D Then Return False

        Dim triggerPercent = Math.Max(0D, configuration.BreakEvenTriggerPercent)
        Dim triggerPrice = position.EntryPrice * (1D + triggerPercent / 100D)
        If price < triggerPrice Then Return False

        Dim changed = False
        If configuration.BreakEvenEnabled Then
            Dim offsetPercent = Math.Max(0D, configuration.BreakEvenOffsetPercent)
            Dim breakEvenPrice = position.EntryPrice * (1D + offsetPercent / 100D)
            If breakEvenPrice > position.StopPrice Then
                position.StopPrice = breakEvenPrice
                _simulationLastAction = $"BE {position.Symbol}: stop ajustado para {breakEvenPrice:0.########}"
                changed = True
            End If
        End If

        If configuration.TrailingStopEnabled AndAlso position.HighestPrice > 0D Then
            Dim distancePercent = Math.Max(0.01D, configuration.TrailingStopDistancePercent)
            Dim trailingPrice = position.HighestPrice * (1D - distancePercent / 100D)
            If trailingPrice > position.StopPrice Then
                position.StopPrice = trailingPrice
                _simulationLastAction = $"TRAIL {position.Symbol}: stop ajustado para {trailingPrice:0.########}"
                changed = True
            End If
        End If

        Return changed
    End Function

    Private Sub CloseSimulationPosition(
        symbol As String,
        price As Decimal,
        timestampUtc As DateTime,
        position As SimulationPosition,
        configuration As SpotBotConfiguration,
        exitReason As String,
        ByRef completedTrade As SpotBotSimulationTrade)

        Dim feeRate = Math.Max(0D, configuration.SimulationFeeRate)
        Dim gross = position.Quantity * price
        Dim fee = gross * feeRate
        Dim net = gross - fee
        Dim tradePnl = net - position.CostBasis

        completedTrade = New SpotBotSimulationTrade With {
            .Symbol = symbol,
            .Strategy = If(position.Strategy, String.Empty),
            .ExitReason = If(exitReason, String.Empty),
            .Quantity = position.Quantity,
            .EntryPrice = position.EntryPrice,
            .ExitPrice = price,
            .CostBasis = position.CostBasis,
            .GrossProceeds = gross,
            .Fees = fee,
            .Pnl = tradePnl,
            .OpenedAtUtc = position.EntryTimestampUtc,
            .ClosedAtUtc = timestampUtc
        }

        _simulationQuoteBalance += net
        _simulationRealizedPnl += tradePnl
        _simulationCompletedTrades += 1
        If tradePnl > 0D Then
            _simulationWinningTrades += 1
        ElseIf tradePnl < 0D Then
            _simulationLosingTrades += 1
        End If

        _simulationPositions.Remove(symbol)
        _lastExits(symbol) = timestampUtc
        Dim actionText = If(exitReason.Equals("TP", StringComparison.OrdinalIgnoreCase),
                            "TP",
                            If(exitReason.Equals("SL", StringComparison.OrdinalIgnoreCase),
                               "SL",
                               If(exitReason.Equals("MANUAL", StringComparison.OrdinalIgnoreCase), "MANUAL", "VENDA")))
        _simulationLastAction = $"{actionText} simulado {symbol}: P/L {tradePnl:+0.00;-0.00;0.00} USDT"

        If position.GridManaged AndAlso
           exitReason.Equals("SL", StringComparison.OrdinalIgnoreCase) Then
            _gridRecoveryEntryPrices(symbol) = position.EntryPrice
        Else
            _gridRecoveryEntryPrices.Remove(symbol)
        End If
    End Sub

    Private Sub CloseSimulationPositionPartially(
        symbol As String,
        price As Decimal,
        timestampUtc As DateTime,
        position As SimulationPosition,
        configuration As SpotBotConfiguration,
        ByRef completedTrade As SpotBotSimulationTrade)

        If position Is Nothing OrElse position.Quantity <= 0D Then Return

        Dim levelCount = Math.Max(1, position.GridLevelCount)
        Dim quantity = If(levelCount <= 1, position.Quantity, position.Quantity / levelCount)
        quantity = Math.Min(position.Quantity, quantity)
        Dim proportion = If(position.Quantity = 0D, 1D, quantity / position.Quantity)
        Dim costBasis = position.CostBasis * proportion
        Dim fee = quantity * price * Math.Max(0D, configuration.SimulationFeeRate)
        Dim gross = quantity * price
        Dim net = gross - fee
        Dim tradePnl = net - costBasis

        completedTrade = New SpotBotSimulationTrade With {
            .Symbol = symbol,
            .Strategy = If(position.Strategy, String.Empty),
            .ExitReason = "VENDA_PARCIAL",
            .Quantity = quantity,
            .EntryPrice = position.EntryPrice,
            .ExitPrice = price,
            .CostBasis = costBasis,
            .GrossProceeds = gross,
            .Fees = fee,
            .Pnl = tradePnl,
            .OpenedAtUtc = position.EntryTimestampUtc,
            .ClosedAtUtc = timestampUtc
        }

        _simulationQuoteBalance += net
        _simulationRealizedPnl += tradePnl
        _simulationCompletedTrades += 1
        If tradePnl > 0D Then
            _simulationWinningTrades += 1
        ElseIf tradePnl < 0D Then
            _simulationLosingTrades += 1
        End If

        position.Quantity -= quantity
        position.CostBasis -= costBasis
        position.GridLevelCount = Math.Max(0, levelCount - 1)
        If position.Quantity <= 0.0000000001D OrElse position.GridLevelCount <= 0 Then
            _simulationPositions.Remove(symbol)
        End If

        _simulationLastAction = $"VENDA parcial grid {symbol}: P/L {tradePnl:+0.00;-0.00;0.00} USDT"
    End Sub

    Private Sub ResetSimulationLocked()
        _simulationPositions.Clear()
        _lastExits.Clear()
        _gridRecoveryEntryPrices.Clear()
        _simulationQuoteBalance = Math.Max(0D, _configuration.SimulationStartingQuote)
        _simulationRealizedPnl = 0D
        _simulationCompletedTrades = 0
        _simulationWinningTrades = 0
        _simulationLosingTrades = 0
        _simulationLastAction = "Carteira virtual reiniciada"
    End Sub

    Private Sub RestoreSimulationLocked(state As SpotBotPersistedSimulationState)
        If state Is Nothing OrElse state.Snapshot Is Nothing OrElse state.Positions Is Nothing OrElse
           state.Positions.Count = 0 Then
            Return
        End If

        _configuration.SimulationStartingQuote = Math.Max(0D, state.Snapshot.StartingQuote)
        _configuration.EnabledStrategies.Clear()
        For Each strategy In state.EnabledStrategies
            _configuration.EnabledStrategies.Add(strategy)
        Next
        _configuration.CombinationMode = state.CombinationMode
        _configuration.RequiredConfirmations = Math.Max(1, state.RequiredConfirmations)
        _configuration.BollingerPeriod = Math.Max(2, state.BollingerPeriod)
        _configuration.BollingerStandardDeviations = Math.Max(0.1D, state.BollingerStandardDeviations)
        _configuration.BollingerCandleMinutes = If(state.BollingerCandleMinutes = 240, 240, 1440)
        _configuration.SimulationAllocationPercent = Math.Max(0D, Math.Min(100D, state.AllocationPercent))
        _configuration.SimulationFeeRate = Math.Max(0D, state.FeeRate)
        _configuration.BreakEvenEnabled = state.BreakEvenEnabled
        _configuration.BreakEvenTriggerPercent = Math.Max(0D, state.BreakEvenTriggerPercent)
        _configuration.BreakEvenOffsetPercent = Math.Max(0D, state.BreakEvenOffsetPercent)
        _configuration.TrailingStopEnabled = state.TrailingStopEnabled
        _configuration.TrailingStopDistancePercent = Math.Max(0.01D, state.TrailingStopDistancePercent)
        _configuration.PostExitCooldownMinutes = Math.Max(0D, state.PostExitCooldownMinutes)
        _simulationQuoteBalance = Math.Max(0D, state.Snapshot.QuoteBalance)
        _simulationRealizedPnl = state.Snapshot.RealizedPnl
        _simulationCompletedTrades = state.Snapshot.CompletedTrades
        _simulationWinningTrades = state.Snapshot.WinningTrades
        _simulationLosingTrades = state.Snapshot.LosingTrades
        _simulationLastAction = If(String.IsNullOrWhiteSpace(state.Snapshot.LastAction),
                                    "Simulação restaurada",
                                    state.Snapshot.LastAction)
        _simulationPositions.Clear()

        For Each persistedPosition In state.Positions
            If persistedPosition Is Nothing OrElse String.IsNullOrWhiteSpace(persistedPosition.Symbol) OrElse
               persistedPosition.Quantity <= 0D OrElse persistedPosition.EntryPrice <= 0D Then
                Continue For
            End If

            _simulationPositions(NormalizeSymbol(persistedPosition.Symbol)) = New SimulationPosition With {
                .Symbol = NormalizeSymbol(persistedPosition.Symbol),
                .Strategy = If(String.IsNullOrWhiteSpace(persistedPosition.Strategy),
                               If(state.EnabledStrategies.Contains(SpotBotStrategyKind.GridAdaptive), "Grid V2", "Desconhecida"),
                               persistedPosition.Strategy),
                .Quantity = persistedPosition.Quantity,
                .EntryPrice = persistedPosition.EntryPrice,
                .CostBasis = persistedPosition.CostBasis,
                .LastPrice = If(persistedPosition.CurrentPrice > 0D, persistedPosition.CurrentPrice, persistedPosition.EntryPrice),
                 .HighestPrice = Math.Max(persistedPosition.HighestPrice,
                                         Math.Max(persistedPosition.CurrentPrice, persistedPosition.EntryPrice)),
                .StopPrice = persistedPosition.StopPrice,
                .TakeProfitPrice = persistedPosition.TakeProfitPrice,
                .GridManaged = state.EnabledStrategies.Contains(SpotBotStrategyKind.GridAdaptive) OrElse
                               state.EnabledStrategies.Contains(SpotBotStrategyKind.GridRegime),
                .GridLevelCount = 1,
                .EntryTimestampUtc = persistedPosition.OpenedAtUtc
            }
        Next

        If _simulationPositions.Count = 0 Then
            ResetSimulationLocked()
            Return
        End If

        _activeRunId = state.RunId
        _restoredStatePending = True
    End Sub

    Private Function BuildSimulationSnapshotLocked(configuration As SpotBotConfiguration) As SpotBotSimulationSnapshot
        Dim openValue = 0D
        Dim unrealizedPnl = 0D

        For Each position In _simulationPositions.Values
            openValue += position.Quantity * position.LastPrice
            unrealizedPnl += position.Quantity * position.LastPrice - position.CostBasis
        Next

        Dim equity = _simulationQuoteBalance + openValue
        Return New SpotBotSimulationSnapshot With {
            .StartingQuote = configuration.SimulationStartingQuote,
            .QuoteBalance = _simulationQuoteBalance,
            .OpenValue = openValue,
            .RealizedPnl = _simulationRealizedPnl,
            .UnrealizedPnl = unrealizedPnl,
            .Equity = equity,
            .TotalPnl = equity - configuration.SimulationStartingQuote,
            .CompletedTrades = _simulationCompletedTrades,
            .WinningTrades = _simulationWinningTrades,
            .LosingTrades = _simulationLosingTrades,
            .OpenPositions = _simulationPositions.Count,
            .LastAction = _simulationLastAction
        }
    End Function

    Public Function GetPriceHistory(symbol As String) As IReadOnlyList(Of SpotBotPricePoint)
        Dim normalizedSymbol = NormalizeSymbol(symbol)
        SyncLock _sync
            Dim history As List(Of SpotBotPricePoint) = Nothing
            If Not _priceHistory.TryGetValue(normalizedSymbol, history) Then
                Return Array.Empty(Of SpotBotPricePoint)()
            End If

            Return history.ToArray()
        End SyncLock
    End Function

    Private Function GetOrCreateRsiCandleHistory(symbol As String) As List(Of SpotBotPricePoint)
        Dim history As List(Of SpotBotPricePoint) = Nothing
        If Not _rsiCandleHistory.TryGetValue(symbol, history) Then
            history = New List(Of SpotBotPricePoint)()
            _rsiCandleHistory(symbol) = history
        End If

        Return history
    End Function

    Private Function GetOrCreateBollingerCandleHistory(symbol As String) As List(Of SpotBotPricePoint)
        Dim history As List(Of SpotBotPricePoint) = Nothing
        If Not _bollingerCandleHistory.TryGetValue(symbol, history) Then
            history = New List(Of SpotBotPricePoint)()
            _bollingerCandleHistory(symbol) = history
        End If

        Return history
    End Function

    Private Function GetOrCreateWaveCandleHistory(symbol As String) As List(Of SpotBotPricePoint)
        Dim history As List(Of SpotBotPricePoint) = Nothing
        If Not _waveCandleHistory.TryGetValue(symbol, history) Then
            history = New List(Of SpotBotPricePoint)()
            _waveCandleHistory(symbol) = history
        End If

        Return history
    End Function

    Private Function GetOrCreateValueZoneCandleHistory(symbol As String, intervalMinutes As Integer) As List(Of SpotBotPricePoint)
        Dim history As List(Of SpotBotPricePoint) = Nothing
        Dim target = If(intervalMinutes = 2, _valueZoneM2CandleHistory, _valueZoneM5CandleHistory)
        If Not target.TryGetValue(symbol, history) Then
            history = New List(Of SpotBotPricePoint)()
            target(symbol) = history
        End If

        Return history
    End Function

    Private Function UpdateValueZoneCandle(
        symbol As String,
        timestampUtc As DateTime,
        price As Decimal,
        history As List(Of SpotBotPricePoint),
        ByRef closedCandle As SpotBotPricePoint,
        intervalMinutes As Integer) As Boolean

        Dim candleStart = TruncateToFixedInterval(timestampUtc, intervalMinutes)
        Dim currentCandle As CandleState = Nothing
        Dim currentTarget = If(intervalMinutes = 2, _valueZoneM2CurrentCandles, _valueZoneM5CurrentCandles)

        If Not currentTarget.TryGetValue(symbol, currentCandle) Then
            currentTarget(symbol) = New CandleState With {
                .StartUtc = candleStart,
                .Open = price,
                .High = price,
                .Low = price,
                .Close = price}
            Return False
        End If

        If candleStart < currentCandle.StartUtc Then Return False

        If candleStart = currentCandle.StartUtc Then
            currentCandle.High = Math.Max(currentCandle.High, price)
            currentCandle.Low = Math.Min(currentCandle.Low, price)
            currentCandle.Close = price
            Return False
        End If

        closedCandle = New SpotBotPricePoint(
            currentCandle.StartUtc,
            currentCandle.Close,
            currentCandle.Open,
            currentCandle.High,
            currentCandle.Low)
        history.Add(closedCandle)
        Dim maximumCandles = Math.Max(60, _configuration.MaximumPriceHistory)
        If history.Count > maximumCandles Then
            history.RemoveRange(0, history.Count - maximumCandles)
        End If

        currentTarget(symbol) = New CandleState With {
            .StartUtc = candleStart,
            .Open = price,
            .High = price,
            .Low = price,
            .Close = price}
        Return True
    End Function

    Private Function UpdateRsiCandle(
        symbol As String,
        timestampUtc As DateTime,
        price As Decimal,
        history As List(Of SpotBotPricePoint),
        ByRef closedCandle As SpotBotPricePoint) As Boolean

        Dim candleStart = TruncateToMinute(timestampUtc)
        Dim currentCandle As RsiCandleState = Nothing

        If Not _rsiCurrentCandles.TryGetValue(symbol, currentCandle) Then
            currentCandle = New RsiCandleState With {
                .StartUtc = candleStart,
                .Open = price,
                .High = price,
                .Low = price,
                .Close = price
            }
            _rsiCurrentCandles(symbol) = currentCandle
            Return False
        End If

        If candleStart < currentCandle.StartUtc Then
            Return False
        End If

        If candleStart = currentCandle.StartUtc Then
            currentCandle.High = Math.Max(currentCandle.High, price)
            currentCandle.Low = Math.Min(currentCandle.Low, price)
            currentCandle.Close = price
            Return False
        End If

        closedCandle = New SpotBotPricePoint(
            currentCandle.StartUtc,
            currentCandle.Close,
            currentCandle.Open,
            currentCandle.High,
            currentCandle.Low)
        history.Add(closedCandle)
        Dim maximumCandles = Math.Max(30, _configuration.MaximumPriceHistory)
        If history.Count > maximumCandles Then
            history.RemoveRange(0, history.Count - maximumCandles)
        End If

        _rsiCurrentCandles(symbol) = New RsiCandleState With {
            .StartUtc = candleStart,
            .Open = price,
            .High = price,
            .Low = price,
            .Close = price
        }
        Return True
    End Function

    Private Function UpdateBollingerCandle(
        symbol As String,
        timestampUtc As DateTime,
        price As Decimal,
        history As List(Of SpotBotPricePoint),
        ByRef closedCandle As SpotBotPricePoint) As Boolean

        Dim candleStart = TruncateToBollingerInterval(timestampUtc, _configuration.BollingerCandleMinutes)
        Dim currentCandle As CandleState = Nothing

        If Not _bollingerCurrentCandles.TryGetValue(symbol, currentCandle) Then
            _bollingerCurrentCandles(symbol) = New CandleState With {
                .StartUtc = candleStart,
                .Open = price,
                .High = price,
                .Low = price,
                .Close = price
            }
            Return False
        End If

        If candleStart < currentCandle.StartUtc Then Return False

        If candleStart = currentCandle.StartUtc Then
            currentCandle.High = Math.Max(currentCandle.High, price)
            currentCandle.Low = Math.Min(currentCandle.Low, price)
            currentCandle.Close = price
            Return False
        End If

        closedCandle = New SpotBotPricePoint(
            currentCandle.StartUtc,
            currentCandle.Close,
            currentCandle.Open,
            currentCandle.High,
            currentCandle.Low)
        history.Add(closedCandle)

        Dim maximumCandles = Math.Max(30, _configuration.MaximumPriceHistory)
        If history.Count > maximumCandles Then
            history.RemoveRange(0, history.Count - maximumCandles)
        End If

        _bollingerCurrentCandles(symbol) = New CandleState With {
            .StartUtc = candleStart,
            .Open = price,
            .High = price,
            .Low = price,
            .Close = price
        }
        Return True
    End Function

    Private Function UpdateWaveCandle(
        symbol As String,
        timestampUtc As DateTime,
        price As Decimal,
        history As List(Of SpotBotPricePoint),
        ByRef closedCandle As SpotBotPricePoint,
        candleMinutes As Integer) As Boolean

        Dim candleStart = TruncateToBollingerInterval(timestampUtc, candleMinutes)
        Dim currentCandle As CandleState = Nothing

        If Not _waveCurrentCandles.TryGetValue(symbol, currentCandle) Then
            _waveCurrentCandles(symbol) = New CandleState With {
                .StartUtc = candleStart,
                .Open = price,
                .High = price,
                .Low = price,
                .Close = price}
            Return False
        End If

        If candleStart < currentCandle.StartUtc Then Return False

        If candleStart = currentCandle.StartUtc Then
            currentCandle.High = Math.Max(currentCandle.High, price)
            currentCandle.Low = Math.Min(currentCandle.Low, price)
            currentCandle.Close = price
            Return False
        End If

        closedCandle = New SpotBotPricePoint(
            currentCandle.StartUtc,
            currentCandle.Close,
            currentCandle.Open,
            currentCandle.High,
            currentCandle.Low)
        history.Add(closedCandle)

        Dim maximumCandles = Math.Max(60, _configuration.MaximumPriceHistory)
        If history.Count > maximumCandles Then
            history.RemoveRange(0, history.Count - maximumCandles)
        End If

        _waveCurrentCandles(symbol) = New CandleState With {
            .StartUtc = candleStart,
            .Open = price,
            .High = price,
            .Low = price,
            .Close = price}
        Return True
    End Function

    Private Shared Function TruncateToMinute(timestampUtc As DateTime) As DateTime
        Return New DateTime(
            timestampUtc.Year,
            timestampUtc.Month,
            timestampUtc.Day,
            timestampUtc.Hour,
            timestampUtc.Minute,
            0,
            DateTimeKind.Utc)
    End Function

    Private Shared Function TruncateToFixedInterval(timestampUtc As DateTime, intervalMinutes As Integer) As DateTime
        Dim interval = Math.Max(1, intervalMinutes)
        Dim dayStart = New DateTime(
            timestampUtc.Year,
            timestampUtc.Month,
            timestampUtc.Day,
            0,
            0,
            0,
            DateTimeKind.Utc)
        Dim minutesSinceMidnight = timestampUtc.Hour * 60 + timestampUtc.Minute
        Dim bucketMinutes = (minutesSinceMidnight \ interval) * interval
        Return dayStart.AddMinutes(bucketMinutes)
    End Function

    Private Shared Function TruncateToBollingerInterval(timestampUtc As DateTime, intervalMinutes As Integer) As DateTime
        Dim interval = If(intervalMinutes = 240, 240, 1440)
        Dim dayStart = New DateTime(
            timestampUtc.Year,
            timestampUtc.Month,
            timestampUtc.Day,
            0,
            0,
            0,
            DateTimeKind.Utc)

        If interval >= 1440 Then Return dayStart

        Dim minutesSinceMidnight = timestampUtc.Hour * 60 + timestampUtc.Minute
        Dim bucketMinutes = (minutesSinceMidnight \ interval) * interval
        Return dayStart.AddMinutes(bucketMinutes)
    End Function

    Public Function GetSimulationSnapshot() As SpotBotSimulationSnapshot
        SyncLock _sync
            Return BuildSimulationSnapshotLocked(_configuration)
        End SyncLock
    End Function

    Public Function ClosePositionManually(
        symbol As String,
        Optional timestampUtc As DateTime = Nothing) As Boolean

        Dim normalizedSymbol = NormalizeSymbol(symbol)
        If String.IsNullOrWhiteSpace(normalizedSymbol) Then Return False

        Dim completedTrade As SpotBotSimulationTrade = Nothing
        Dim snapshot As SpotBotSimulationSnapshot = Nothing
        Dim runId As Long
        Dim closeAt = If(timestampUtc = DateTime.MinValue, DateTime.UtcNow, timestampUtc.ToUniversalTime())

        SyncLock _sync
            Dim position As SimulationPosition = Nothing
            If Not _simulationPositions.TryGetValue(normalizedSymbol, position) OrElse
               position Is Nothing OrElse position.Quantity <= 0D Then
                Return False
            End If

            Dim closePrice = If(position.LastPrice > 0D, position.LastPrice, position.EntryPrice)
            CloseSimulationPosition(
                normalizedSymbol,
                closePrice,
                closeAt,
                position,
                _configuration,
                "MANUAL",
                completedTrade)
            snapshot = BuildSimulationSnapshotLocked(_configuration)
            runId = _activeRunId
        End SyncLock

        If completedTrade Is Nothing Then Return False

        If runId > 0 Then
            Try
                SpotTradeBotRepository.SaveTrade(runId, completedTrade)
                SpotTradeBotRepository.UpdateRun(
                    runId,
                    snapshot,
                    If(IsRunning, "RUNNING", "STOPPED"))
                SpotTradeBotRepository.SaveOpenPositions(runId, GetOpenPositions())
            Catch ex As Exception
                Debug.WriteLine("Erro salvando fechamento manual da simulação: " & ex.Message)
            End Try
        End If

        RaiseEvent SimulationUpdated(Me, New SpotBotSimulationEventArgs(snapshot))
        Return True
    End Function

    Public Function GetOpenPositions() As IReadOnlyList(Of SpotBotOpenPosition)
        SyncLock _sync
            Return _simulationPositions.Values.
                Select(Function(position)
                           Dim notional = position.Quantity * position.LastPrice
                           Dim unrealizedPnl = notional - position.CostBasis
                           Return New SpotBotOpenPosition With {
                               .Symbol = position.Symbol,
                               .Strategy = position.Strategy,
                               .Side = "LONG",
                               .Quantity = position.Quantity,
                               .EntryPrice = position.EntryPrice,
                               .CurrentPrice = position.LastPrice,
                               .CostBasis = position.CostBasis,
                               .Notional = notional,
                               .UnrealizedPnl = unrealizedPnl,
                               .RoiPercent = If(position.CostBasis = 0D, 0D, unrealizedPnl / position.CostBasis * 100D),
                                .StopPrice = position.StopPrice,
                                .TakeProfitPrice = position.TakeProfitPrice,
                                .HighestPrice = position.HighestPrice,
                               .GridLevelCount = Math.Max(0, position.GridLevelCount),
                               .OpenedAtUtc = position.EntryTimestampUtc
                           }
                       End Function).
                OrderBy(Function(position) position.Symbol, StringComparer.OrdinalIgnoreCase).
                ToList()
        End SyncLock
    End Function

    Private Function CombineSignals(
        symbol As String,
        price As Decimal,
        timestampUtc As DateTime,
        strategySignals As List(Of SpotBotStrategySignal),
        configuration As SpotBotConfiguration) As SpotBotSignal

        Dim buySignals = strategySignals.Where(Function(item) item.Direction = SpotBotSignalDirection.Buy).ToList()
        Dim sellSignals = strategySignals.Where(Function(item) item.Direction = SpotBotSignalDirection.Sell).ToList()
        Dim activeCount = strategySignals.Where(Function(item) item.Direction <> SpotBotSignalDirection.Hold).Count()
        Dim direction As SpotBotSignalDirection = SpotBotSignalDirection.Hold
        Dim selected As List(Of SpotBotStrategySignal) = Nothing
        Dim score As Decimal = 0D

        Select Case configuration.CombinationMode
            Case SpotBotCombinationMode.Any
                If buySignals.Sum(Function(item) item.Confidence) >= sellSignals.Sum(Function(item) item.Confidence) Then
                    direction = If(buySignals.Count > 0, SpotBotSignalDirection.Buy, SpotBotSignalDirection.Hold)
                    selected = buySignals
                Else
                    direction = SpotBotSignalDirection.Sell
                    selected = sellSignals
                End If

            Case SpotBotCombinationMode.All
                If strategySignals.Count > 0 AndAlso
                   buySignals.Count = strategySignals.Count Then
                    direction = SpotBotSignalDirection.Buy
                    selected = buySignals
                ElseIf strategySignals.Count > 0 AndAlso
                       sellSignals.Count = strategySignals.Count Then
                    direction = SpotBotSignalDirection.Sell
                    selected = sellSignals
                End If

            Case SpotBotCombinationMode.MinimumConfirmations
                Dim required = Math.Max(1, configuration.RequiredConfirmations)
                If buySignals.Count >= required AndAlso buySignals.Count > sellSignals.Count Then
                    direction = SpotBotSignalDirection.Buy
                    selected = buySignals
                ElseIf sellSignals.Count >= required AndAlso sellSignals.Count > buySignals.Count Then
                    direction = SpotBotSignalDirection.Sell
                    selected = sellSignals
                End If

            Case SpotBotCombinationMode.WeightedScore
                Dim buyScore = buySignals.Sum(Function(item) item.Confidence)
                Dim sellScore = sellSignals.Sum(Function(item) item.Confidence)
                If buyScore >= configuration.MinimumWeightedScore AndAlso buyScore > sellScore Then
                    direction = SpotBotSignalDirection.Buy
                    selected = buySignals
                    score = buyScore
                ElseIf sellScore >= configuration.MinimumWeightedScore AndAlso sellScore > buyScore Then
                    direction = SpotBotSignalDirection.Sell
                    selected = sellSignals
                    score = sellScore
                End If
        End Select

        If direction = SpotBotSignalDirection.Hold OrElse selected Is Nothing Then
            Return Nothing
        End If

        If score = 0D Then
            score = selected.Sum(Function(item) item.Confidence)
        End If

        Dim entryPrice = selected.Where(Function(item) item.EntryPrice > 0D).
            Select(Function(item) item.EntryPrice).
            DefaultIfEmpty(price).
            First()
        Dim stopPrice = selected.Where(Function(item) item.StopPrice > 0D).
            Select(Function(item) item.StopPrice).
            DefaultIfEmpty(0D).
            First()
        Dim takeProfitPrice = selected.Where(Function(item) item.TakeProfitPrice > 0D).
            Select(Function(item) item.TakeProfitPrice).
            DefaultIfEmpty(0D).
            First()
        Dim gridManaged = selected.Any(Function(item) item.GridManaged)

        Return New SpotBotSignal With {
            .Symbol = symbol,
            .TimestampUtc = timestampUtc,
            .Price = entryPrice,
            .EntryPrice = entryPrice,
            .StopPrice = stopPrice,
            .TakeProfitPrice = takeProfitPrice,
            .Direction = direction,
            .Confirmations = selected.Count,
            .ActiveStrategies = activeCount,
            .Score = score,
            .Reasons = selected.Where(Function(item) Not String.IsNullOrWhiteSpace(item.Reason)).Select(Function(item) item.Reason).ToList(),
            .StrategySignals = strategySignals,
            .SimulationOnly = configuration.SimulationOnly,
            .GridManaged = gridManaged
        }
    End Function

    Private Shared Function CopyConfiguration(source As SpotBotConfiguration) As SpotBotConfiguration
        Dim copy As New SpotBotConfiguration With {
            .CombinationMode = source.CombinationMode,
            .RequiredConfirmations = source.RequiredConfirmations,
            .BollingerPeriod = source.BollingerPeriod,
            .BollingerStandardDeviations = source.BollingerStandardDeviations,
            .BollingerCandleMinutes = source.BollingerCandleMinutes,
            .BnfMovingAveragePeriod = source.BnfMovingAveragePeriod,
            .BnfMinimumDeviationPercent = source.BnfMinimumDeviationPercent,
            .WaveEmaPeriod = source.WaveEmaPeriod,
            .WaveSlopeLookback = source.WaveSlopeLookback,
            .WaveTouchTolerancePercent = source.WaveTouchTolerancePercent,
            .WaveMinimumSlopeDegrees = source.WaveMinimumSlopeDegrees,
            .WaveStrongSlopeDegrees = source.WaveStrongSlopeDegrees,
            .WaveMinimumRiskReward = source.WaveMinimumRiskReward,
            .WaveStructureLookback = source.WaveStructureLookback,
            .MinimumWeightedScore = source.MinimumWeightedScore,
            .Cooldown = source.Cooldown,
            .MaximumPriceHistory = source.MaximumPriceHistory,
            .EmaFastPeriod = source.EmaFastPeriod,
            .EmaSlowPeriod = source.EmaSlowPeriod,
            .RsiPeriod = source.RsiPeriod,
            .RsiOversold = source.RsiOversold,
            .RsiOverbought = source.RsiOverbought,
            .BreakoutPeriod = source.BreakoutPeriod,
            .GridSpacingPercent = source.GridSpacingPercent,
            .GridReferencePrice = source.GridReferencePrice,
            .GridRegimeLookback = source.GridRegimeLookback,
            .GridRegimeMaxRangePercent = source.GridRegimeMaxRangePercent,
            .GridRegimeMaxSlopePercent = source.GridRegimeMaxSlopePercent,
            .GridV2Lookback = source.GridV2Lookback,
            .GridV2VolatilityPeriod = source.GridV2VolatilityPeriod,
            .GridV2SpacingMinimumPercent = source.GridV2SpacingMinimumPercent,
            .GridV2SpacingMaximumPercent = source.GridV2SpacingMaximumPercent,
            .GridV2MaxRangePercent = source.GridV2MaxRangePercent,
            .GridV2MaxSlopePercent = source.GridV2MaxSlopePercent,
            .GridV2MaxLevels = source.GridV2MaxLevels,
            .GridV2LevelAllocationPercent = source.GridV2LevelAllocationPercent,
            .GridV2MaxExposurePercent = source.GridV2MaxExposurePercent,
            .GridV2BreakoutAtrMultiplier = source.GridV2BreakoutAtrMultiplier,
            .GridV2MinimumStopPercent = source.GridV2MinimumStopPercent,
            .GridV2StopVolatilityMultiplier = source.GridV2StopVolatilityMultiplier,
            .GridV2MaximumStopPercent = source.GridV2MaximumStopPercent,
            .GridV2EmaFilterEnabled = source.GridV2EmaFilterEnabled,
            .GridV2FastEmaPeriod = source.GridV2FastEmaPeriod,
            .GridV2SlowEmaPeriod = source.GridV2SlowEmaPeriod,
            .GridV2EmaMaxSpreadPercent = source.GridV2EmaMaxSpreadPercent,
            .GridV2EmaMaxSlopePercent = source.GridV2EmaMaxSlopePercent,
            .GridV2UseNearestSupportStop = source.GridV2UseNearestSupportStop,
            .GridV2SupportBufferAtr = source.GridV2SupportBufferAtr,
            .ValueZoneFastEmaPeriod = source.ValueZoneFastEmaPeriod,
            .ValueZoneSlowEmaPeriod = source.ValueZoneSlowEmaPeriod,
            .ValueZoneAtrPeriod = source.ValueZoneAtrPeriod,
            .ValueZoneToleranceAtr = source.ValueZoneToleranceAtr,
            .ValueZoneMinimumRiskReward = source.ValueZoneMinimumRiskReward,
            .ValueZoneStructureLookback = source.ValueZoneStructureLookback,
            .ValueZoneRequireVwap = source.ValueZoneRequireVwap,
            .MomentumVolumeLookback = source.MomentumVolumeLookback,
            .MomentumVolumeMultiplier = source.MomentumVolumeMultiplier,
            .MomentumEmaPeriod = source.MomentumEmaPeriod,
            .MomentumRiskReward = source.MomentumRiskReward,
            .SimulationStartingQuote = source.SimulationStartingQuote,
            .SimulationAllocationPercent = source.SimulationAllocationPercent,
            .SimulationFeeRate = source.SimulationFeeRate,
            .TakeProfitPercent = source.TakeProfitPercent,
            .StopLossPercent = source.StopLossPercent,
            .BreakEvenEnabled = source.BreakEvenEnabled,
            .BreakEvenTriggerPercent = source.BreakEvenTriggerPercent,
            .BreakEvenOffsetPercent = source.BreakEvenOffsetPercent,
            .TrailingStopEnabled = source.TrailingStopEnabled,
            .TrailingStopDistancePercent = source.TrailingStopDistancePercent,
            .PostExitCooldownMinutes = source.PostExitCooldownMinutes,
            .SimulationOnly = source.SimulationOnly,
            .UniverseMode = source.UniverseMode,
            .UniverseMaxAssets = source.UniverseMaxAssets,
            .UniverseMinimumQuoteVolume = source.UniverseMinimumQuoteVolume
        }

        For Each kind In source.EnabledStrategies
            copy.EnabledStrategies.Add(kind)
        Next

        For Each symbol In source.ManualUniverseSymbols
            copy.ManualUniverseSymbols.Add(symbol)
        Next

        Return copy
    End Function

    Private Shared Function NormalizeSymbol(symbol As String) As String
        Return If(symbol, String.Empty).Trim().ToUpperInvariant()
    End Function

    Private Shared Function ResolveSignalStrategy(signal As SpotBotSignal) As String
        If signal Is Nothing Then Return "Desconhecida"

        Dim direction = signal.Direction
        Dim names = signal.StrategySignals.
            Where(Function(item) item IsNot Nothing AndAlso item.Direction = direction).
            Select(Function(item) StrategyDisplayName(item.Strategy)).
            Where(Function(item) Not String.IsNullOrWhiteSpace(item)).
            Distinct(StringComparer.OrdinalIgnoreCase).
            ToList()

        If names.Count = 0 Then
            Return If(signal.GridManaged, "Grid V2", "Desconhecida")
        End If

        Return String.Join(" + ", names)
    End Function

    Private Shared Function StrategyDisplayName(strategy As SpotBotStrategyKind) As String
        Select Case strategy
            Case SpotBotStrategyKind.BaseOriginalProtected
                Return "Bollinger Base"
            Case SpotBotStrategyKind.BollingerMeanReversion
                Return "Bollinger"
            Case SpotBotStrategyKind.GridAdaptive
                Return "Grid V2"
            Case SpotBotStrategyKind.GridRegime
                Return "Grid regime"
            Case SpotBotStrategyKind.WavePullback
                Return "Pullback EMA34"
            Case SpotBotStrategyKind.BreakoutRetest
                Return "Breakout + Reteste"
            Case SpotBotStrategyKind.MomentumVolume
                Return "Momentum volume"
            Case SpotBotStrategyKind.EmaCrossover
                Return "EMA"
            Case SpotBotStrategyKind.Rsi
                Return "RSI"
            Case SpotBotStrategyKind.Breakout
                Return "Breakout"
            Case SpotBotStrategyKind.Grid
                Return "Grid"
            Case SpotBotStrategyKind.BnfContrarian
                Return "BNF"
            Case SpotBotStrategyKind.ValueZonePullback
                Return "Pullback zona de valor"
            Case Else
                Return strategy.ToString()
        End Select
    End Function

    Private NotInheritable Class SimulationPosition
        Public Property Symbol As String
        Public Property Strategy As String
        Public Property Quantity As Decimal
        Public Property EntryPrice As Decimal
        Public Property CostBasis As Decimal
        Public Property LastPrice As Decimal
        Public Property HighestPrice As Decimal
        Public Property StopPrice As Decimal
        Public Property TakeProfitPrice As Decimal
        Public Property GridManaged As Boolean
        Public Property GridLevelCount As Integer
        Public Property EntryTimestampUtc As DateTime
    End Class

    Private NotInheritable Class RsiCandleState
        Public Property StartUtc As DateTime
        Public Property Open As Decimal
        Public Property High As Decimal
        Public Property Low As Decimal
        Public Property Close As Decimal
    End Class

    Private NotInheritable Class CandleState
        Public Property StartUtc As DateTime
        Public Property Open As Decimal
        Public Property High As Decimal
        Public Property Low As Decimal
        Public Property Close As Decimal
    End Class
End Class
