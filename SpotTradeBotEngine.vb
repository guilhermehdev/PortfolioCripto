Imports System
Imports System.Collections.Generic
Imports System.Linq

Public NotInheritable Class SpotTradeBotEngine
    Private ReadOnly _sync As New Object()
    Private ReadOnly _strategies As New Dictionary(Of SpotBotStrategyKind, ISpotBotStrategy)()
    Private ReadOnly _priceHistory As New Dictionary(Of String, List(Of SpotBotPricePoint))(StringComparer.OrdinalIgnoreCase)
    Private ReadOnly _rsiCandleHistory As New Dictionary(Of String, List(Of SpotBotPricePoint))(StringComparer.OrdinalIgnoreCase)
    Private ReadOnly _rsiCurrentCandles As New Dictionary(Of String, RsiCandleState)(StringComparer.OrdinalIgnoreCase)
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

        SyncLock _sync
            ResetSimulationLocked()
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

        SyncLock _sync
            If _running Then Return
            _running = True
            configurationSnapshot = CopyConfiguration(_configuration)
        End SyncLock

        Try
            Dim runId = SpotTradeBotRepository.StartRun(configurationSnapshot)
            SyncLock _sync
                _activeRunId = runId
            End SyncLock
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
        End SyncLock

        If runId <= 0 Then Return

        Try
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
                _lastSignals.Clear()
                ResetSimulationLocked()
            Else
                Dim normalized = NormalizeSymbol(symbol)
                _priceHistory.Remove(normalized)
                _rsiCandleHistory.Remove(normalized)
                _rsiCurrentCandles.Remove(normalized)
                For Each key In _lastSignals.Keys.
                    Where(Function(item) item.StartsWith(normalized & "|", StringComparison.OrdinalIgnoreCase)).
                    ToList()
                    _lastSignals.Remove(key)
                Next
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

                Dim strategySignal = strategy.Evaluate(normalizedSymbol, strategyPrices, configurationSnapshot)
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
        Dim simulationSnapshot = ApplySimulation(normalizedSymbol, price, acceptedSignal, configurationSnapshot, completedTrade)
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
                End If
            Catch ex As Exception
                Debug.WriteLine("Erro salvando dados da simulação: " & ex.Message)
            End Try
        End If

        If acceptedSignal Is Nothing Then Return Nothing

        RaiseEvent SignalGenerated(Me, New SpotBotSignalEventArgs(acceptedSignal))
        Return acceptedSignal
    End Function

    Private Function ApplySimulation(
        symbol As String,
        price As Decimal,
        signal As SpotBotSignal,
        configuration As SpotBotConfiguration,
        ByRef completedTrade As SpotBotSimulationTrade) As SpotBotSimulationSnapshot

        SyncLock _sync
            Dim position As SimulationPosition = Nothing
            If _simulationPositions.TryGetValue(symbol, position) Then
                position.LastPrice = price
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
                                    .Quantity = notional / price,
                                    .EntryPrice = price,
                                    .CostBasis = notional + fee,
                                    .LastPrice = price,
                                    .EntryTimestampUtc = signal.TimestampUtc
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
                                .ClosedAtUtc = signal.TimestampUtc
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
                            _simulationLastAction = $"VENDA simulada {symbol}: P/L {tradePnl:+0.00;-0.00;0.00} USDT"
                        Else
                            _simulationLastAction = $"VENDA ignorada {symbol}: sem posição aberta"
                        End If
                End Select
            End If

            Return BuildSimulationSnapshotLocked(configuration)
        End SyncLock
    End Function

    Private Sub ResetSimulationLocked()
        _simulationPositions.Clear()
        _simulationQuoteBalance = Math.Max(0D, _configuration.SimulationStartingQuote)
        _simulationRealizedPnl = 0D
        _simulationCompletedTrades = 0
        _simulationWinningTrades = 0
        _simulationLosingTrades = 0
        _simulationLastAction = "Carteira virtual reiniciada"
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
            .SimulationStartingQuote = source.SimulationStartingQuote,
            .SimulationAllocationPercent = source.SimulationAllocationPercent,
            .SimulationFeeRate = source.SimulationFeeRate,
            .SimulationOnly = source.SimulationOnly
        }

        For Each kind In source.EnabledStrategies
            copy.EnabledStrategies.Add(kind)
        Next

        Return copy
    End Function

    Private Shared Function NormalizeSymbol(symbol As String) As String
        Return If(symbol, String.Empty).Trim().ToUpperInvariant()
    End Function

    Private NotInheritable Class SimulationPosition
        Public Property Quantity As Decimal
        Public Property EntryPrice As Decimal
        Public Property CostBasis As Decimal
        Public Property LastPrice As Decimal
        Public Property EntryTimestampUtc As DateTime
    End Class

    Private NotInheritable Class RsiCandleState
        Public Property StartUtc As DateTime
        Public Property Close As Decimal
    End Class
End Class
