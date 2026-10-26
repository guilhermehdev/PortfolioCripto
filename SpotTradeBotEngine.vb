Imports System
Imports System.Collections.Generic
Imports System.Linq

Public NotInheritable Class SpotTradeBotEngine
    Private ReadOnly _sync As New Object()
    Private ReadOnly _strategies As New Dictionary(Of SpotBotStrategyKind, ISpotBotStrategy)()
    Private ReadOnly _priceHistory As New Dictionary(Of String, List(Of SpotBotPricePoint))(StringComparer.OrdinalIgnoreCase)
    Private ReadOnly _rsiCandleHistory As New Dictionary(Of String, List(Of SpotBotPricePoint))(StringComparer.OrdinalIgnoreCase)
    Private ReadOnly _rsiCurrentCandles As New Dictionary(Of String, RsiCandleState)(StringComparer.OrdinalIgnoreCase)
    Private ReadOnly _waveCandleHistory As New Dictionary(Of String, List(Of SpotBotCandle))(StringComparer.OrdinalIgnoreCase)
    Private ReadOnly _waveCurrentCandles As New Dictionary(Of String, WaveCandleState)(StringComparer.OrdinalIgnoreCase)
    Private ReadOnly _lastSignals As New Dictionary(Of String, DateTime)(StringComparer.OrdinalIgnoreCase)
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
        RegisterStrategy(New SpotBotWavePullbackStrategy())

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
                _waveCandleHistory.Clear()
                _waveCurrentCandles.Clear()
                _lastSignals.Clear()
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
                _waveCandleHistory.Remove(normalized)
                _waveCurrentCandles.Remove(normalized)
                For Each key In _lastSignals.Keys.
                    Where(Function(item) item.StartsWith(normalized & "|", StringComparison.OrdinalIgnoreCase)).
                    ToList()
                    _lastSignals.Remove(key)
                Next

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
        Optional timestampUtc As DateTime = Nothing) As SpotBotSignal

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
        Dim rsiPriceSnapshot As IReadOnlyList(Of SpotBotPricePoint) = Array.Empty(Of SpotBotPricePoint)()
        Dim closedCandle As SpotBotPricePoint = Nothing
        Dim waveCandleSnapshot As IReadOnlyList(Of SpotBotCandle) = Array.Empty(Of SpotBotCandle)()
        Dim closedWaveCandle As SpotBotCandle = Nothing
        Dim candleClosed As Boolean
        Dim enabledStrategies As List(Of SpotBotStrategyKind)
        Dim configurationSnapshot As SpotBotConfiguration
        Dim activeRunId As Long

        SyncLock _sync
            If Not _running Then Return Nothing

            If Not _priceHistory.TryGetValue(normalizedSymbol, history) Then
                history = New List(Of SpotBotPricePoint)()
                _priceHistory(normalizedSymbol) = history
            End If

            history.Add(New SpotBotPricePoint(observedAt, price))
            Dim maximum = Math.Max(30, _configuration.MaximumPriceHistory)
            If history.Count > maximum Then
                history.RemoveRange(0, history.Count - maximum)
            End If

            enabledStrategies = _configuration.EnabledStrategies.ToList()
            If enabledStrategies.Contains(SpotBotStrategyKind.Rsi) Then
                rsiHistory = GetOrCreateRsiCandleHistory(normalizedSymbol)
                candleClosed = UpdateRsiCandle(normalizedSymbol, observedAt, price, rsiHistory, closedCandle)
                rsiPriceSnapshot = rsiHistory.ToArray()
            End If

            If enabledStrategies.Contains(SpotBotStrategyKind.WavePullback) Then
                Dim waveHistory = GetOrCreateWaveCandleHistory(normalizedSymbol)
                candleClosed = UpdateWaveCandle(
                    normalizedSymbol,
                    observedAt,
                    price,
                    waveHistory,
                    closedWaveCandle) OrElse candleClosed
                waveCandleSnapshot = waveHistory.ToArray()
                If closedCandle Is Nothing AndAlso closedWaveCandle IsNot Nothing Then
                    closedCandle = New SpotBotPricePoint(closedWaveCandle.TimestampUtc, closedWaveCandle.Close)
                End If
            End If

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
                End If

                Dim strategySignal As SpotBotStrategySignal = Nothing
                If kind = SpotBotStrategyKind.WavePullback Then
                    If candleClosed AndAlso waveCandleSnapshot.Count > 0 Then
                        Dim candleStrategy = TryCast(strategy, ISpotBotCandleStrategy)
                        If candleStrategy IsNot Nothing Then
                            strategySignal = candleStrategy.EvaluateCandles(
                                normalizedSymbol,
                                waveCandleSnapshot,
                                configurationSnapshot)
                        End If
                    End If
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
                If Not cooldownBlocked Then
                    _lastSignals(cooldownKey) = observedAt
                    acceptedSignal = signal
                End If
            End SyncLock
        End If

        Dim completedTrade As SpotBotSimulationTrade = Nothing
        Dim automaticExit As Boolean
        Dim simulationSnapshot = ApplySimulation(
            normalizedSymbol,
            price,
            observedAt,
            acceptedSignal,
            configurationSnapshot,
            completedTrade,
            automaticExit)
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

                If candleClosed OrElse completedTrade IsNot Nothing OrElse acceptedSignal IsNot Nothing Then
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

    Private Function ApplySimulation(
        symbol As String,
        price As Decimal,
        timestampUtc As DateTime,
        signal As SpotBotSignal,
        configuration As SpotBotConfiguration,
        ByRef completedTrade As SpotBotSimulationTrade,
        ByRef automaticExit As Boolean) As SpotBotSimulationSnapshot

        SyncLock _sync
            automaticExit = False
            Dim position As SimulationPosition = Nothing
            If _simulationPositions.TryGetValue(symbol, position) Then
                position.LastPrice = price
            End If

            If position IsNot Nothing AndAlso position.EntryPrice > 0D Then
                Dim variationPercent = (price - position.EntryPrice) / position.EntryPrice * 100D
                Dim takeProfitPercent = Math.Max(0D, configuration.TakeProfitPercent)
                Dim stopLossPercent = Math.Max(0D, configuration.StopLossPercent)

                Dim targetReached = If(position.TargetPrice > 0D,
                                       price >= position.TargetPrice,
                                       takeProfitPercent > 0D AndAlso variationPercent >= takeProfitPercent)
                Dim stopReached = If(position.StopPrice > 0D,
                                     price <= position.StopPrice,
                                     stopLossPercent > 0D AndAlso variationPercent <= -stopLossPercent)

                If targetReached Then
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

                If stopReached Then
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
                            Dim allocationPercent = Math.Min(100D, Math.Max(0D, configuration.SimulationAllocationPercent))
                            Dim requestedNotional = _simulationQuoteBalance * allocationPercent / 100D
                            Dim notional = Math.Min(requestedNotional, _simulationQuoteBalance / (1D + feeRate))

                            If notional > 0D Then
                                Dim fee = notional * feeRate
                                position = New SimulationPosition With {
                                    .Symbol = symbol,
                                    .Quantity = notional / price,
                                    .EntryPrice = price,
                                    .CostBasis = notional + fee,
                                    .LastPrice = price,
                                    .EntryTimestampUtc = signal.TimestampUtc,
                                    .StopPrice = signal.SuggestedStopPrice,
                                    .TargetPrice = signal.SuggestedTargetPrice
                                }
                                _simulationPositions(symbol) = position
                                _simulationQuoteBalance -= notional + fee
                                _simulationLastAction = $"COMPRA simulada {symbol}: {position.Quantity:0.########} @ {price:0.########}"
                            End If
                        Else
                            _simulationLastAction = $"COMPRA ignorada {symbol}: posição já aberta"
                        End If

                    Case SpotBotSignalDirection.Sell
                        If position IsNot Nothing Then
                            CloseSimulationPosition(
                                symbol,
                                price,
                                timestampUtc,
                                position,
                                configuration,
                                "VENDA",
                                completedTrade)
                        Else
                            _simulationLastAction = $"VENDA ignorada {symbol}: sem posição aberta"
                        End If
                End Select
            End If

            Return BuildSimulationSnapshotLocked(configuration)
        End SyncLock
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
        Dim actionText = If(exitReason.Equals("TP", StringComparison.OrdinalIgnoreCase),
                            "TP",
                            If(exitReason.Equals("SL", StringComparison.OrdinalIgnoreCase), "SL", "VENDA"))
        _simulationLastAction = $"{actionText} simulado {symbol}: P/L {tradePnl:+0.00;-0.00;0.00} USDT"
    End Sub

    Private Sub ResetSimulationLocked()
        _simulationPositions.Clear()
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
        _configuration.SimulationAllocationPercent = Math.Max(0D, Math.Min(100D, state.AllocationPercent))
        _configuration.SimulationFeeRate = Math.Max(0D, state.FeeRate)
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
                .Quantity = persistedPosition.Quantity,
                .EntryPrice = persistedPosition.EntryPrice,
                .CostBasis = persistedPosition.CostBasis,
                .LastPrice = If(persistedPosition.CurrentPrice > 0D, persistedPosition.CurrentPrice, persistedPosition.EntryPrice),
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

    Private Function GetOrCreateWaveCandleHistory(symbol As String) As List(Of SpotBotCandle)
        Dim history As List(Of SpotBotCandle) = Nothing
        If Not _waveCandleHistory.TryGetValue(symbol, history) Then
            history = New List(Of SpotBotCandle)()
            _waveCandleHistory(symbol) = history
        End If

        Return history
    End Function

    Private Function UpdateWaveCandle(
        symbol As String,
        timestampUtc As DateTime,
        price As Decimal,
        history As List(Of SpotBotCandle),
        ByRef closedCandle As SpotBotCandle) As Boolean

        Dim candleStart = TruncateToMinute(timestampUtc)
        Dim current As WaveCandleState = Nothing

        If Not _waveCurrentCandles.TryGetValue(symbol, current) Then
            _waveCurrentCandles(symbol) = New WaveCandleState With {
                .TimestampUtc = candleStart,
                .Open = price,
                .High = price,
                .Low = price,
                .Close = price}
            Return False
        End If

        If candleStart < current.TimestampUtc Then Return False

        If candleStart = current.TimestampUtc Then
            current.High = Math.Max(current.High, price)
            current.Low = Math.Min(current.Low, price)
            current.Close = price
            Return False
        End If

        closedCandle = New SpotBotCandle With {
            .TimestampUtc = current.TimestampUtc,
            .Open = current.Open,
            .High = current.High,
            .Low = current.Low,
            .Close = current.Close}
        history.Add(closedCandle)

        Dim maximumCandles = Math.Max(60, _configuration.MaximumPriceHistory)
        If history.Count > maximumCandles Then
            history.RemoveRange(0, history.Count - maximumCandles)
        End If

        _waveCurrentCandles(symbol) = New WaveCandleState With {
            .TimestampUtc = candleStart,
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
                .Close = price
            }
            _rsiCurrentCandles(symbol) = currentCandle
            Return False
        End If

        If candleStart < currentCandle.StartUtc Then
            Return False
        End If

        If candleStart = currentCandle.StartUtc Then
            currentCandle.Close = price
            Return False
        End If

        closedCandle = New SpotBotPricePoint(currentCandle.StartUtc, currentCandle.Close)
        history.Add(closedCandle)
        Dim maximumCandles = Math.Max(30, _configuration.MaximumPriceHistory)
        If history.Count > maximumCandles Then
            history.RemoveRange(0, history.Count - maximumCandles)
        End If

        _rsiCurrentCandles(symbol) = New RsiCandleState With {
            .StartUtc = candleStart,
            .Close = price
        }
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

    Public Function GetSimulationSnapshot() As SpotBotSimulationSnapshot
        SyncLock _sync
            Return BuildSimulationSnapshotLocked(_configuration)
        End SyncLock
    End Function

    Public Function GetOpenPositions() As IReadOnlyList(Of SpotBotOpenPosition)
        SyncLock _sync
            Return _simulationPositions.Values.
                Select(Function(position)
                           Dim notional = position.Quantity * position.LastPrice
                           Dim unrealizedPnl = notional - position.CostBasis
                           Return New SpotBotOpenPosition With {
                               .Symbol = position.Symbol,
                               .Side = "LONG",
                               .Quantity = position.Quantity,
                               .EntryPrice = position.EntryPrice,
                               .CurrentPrice = position.LastPrice,
                               .CostBasis = position.CostBasis,
                               .Notional = notional,
                               .UnrealizedPnl = unrealizedPnl,
                               .RoiPercent = If(position.CostBasis = 0D, 0D, unrealizedPnl / position.CostBasis * 100D),
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

        Return New SpotBotSignal With {
            .Symbol = symbol,
            .TimestampUtc = timestampUtc,
            .Price = price,
            .Direction = direction,
            .Confirmations = selected.Count,
            .ActiveStrategies = activeCount,
            .Score = score,
            .Reasons = selected.Where(Function(item) Not String.IsNullOrWhiteSpace(item.Reason)).Select(Function(item) item.Reason).ToList(),
            .StrategySignals = strategySignals,
            .SuggestedStopPrice = selected.Where(Function(item) item.SuggestedStopPrice > 0D).Select(Function(item) item.SuggestedStopPrice).FirstOrDefault(),
            .SuggestedTargetPrice = selected.Where(Function(item) item.SuggestedTargetPrice > 0D).Select(Function(item) item.SuggestedTargetPrice).FirstOrDefault(),
            .SimulationOnly = configuration.SimulationOnly
        }
    End Function

    Private Shared Function CopyConfiguration(source As SpotBotConfiguration) As SpotBotConfiguration
        Dim copy As New SpotBotConfiguration With {
            .CombinationMode = source.CombinationMode,
            .RequiredConfirmations = source.RequiredConfirmations,
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
            .WaveEmaPeriod = source.WaveEmaPeriod,
            .WaveSlopeLookback = source.WaveSlopeLookback,
            .WaveTouchTolerancePercent = source.WaveTouchTolerancePercent,
            .WaveMinimumSlopeDegrees = source.WaveMinimumSlopeDegrees,
            .WaveStrongSlopeDegrees = source.WaveStrongSlopeDegrees,
            .WaveMinimumRiskReward = source.WaveMinimumRiskReward,
            .WaveStructureLookback = source.WaveStructureLookback,
            .SimulationStartingQuote = source.SimulationStartingQuote,
            .SimulationAllocationPercent = source.SimulationAllocationPercent,
            .SimulationFeeRate = source.SimulationFeeRate,
            .TakeProfitPercent = source.TakeProfitPercent,
            .StopLossPercent = source.StopLossPercent,
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

    Private NotInheritable Class SimulationPosition
        Public Property Symbol As String
        Public Property Quantity As Decimal
        Public Property EntryPrice As Decimal
        Public Property CostBasis As Decimal
        Public Property LastPrice As Decimal
        Public Property EntryTimestampUtc As DateTime
        Public Property StopPrice As Decimal
        Public Property TargetPrice As Decimal
    End Class

    Private NotInheritable Class RsiCandleState
        Public Property StartUtc As DateTime
        Public Property Close As Decimal
    End Class

    Private NotInheritable Class WaveCandleState
        Public Property TimestampUtc As DateTime
        Public Property Open As Decimal
        Public Property High As Decimal
        Public Property Low As Decimal
        Public Property Close As Decimal
    End Class
End Class
