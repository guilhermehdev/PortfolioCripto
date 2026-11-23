Imports System
Imports System.Collections.Generic
Imports System.Collections
Imports System.IO
Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports System.Text.Json
Imports Microsoft.Data.Sqlite
Imports PortfolioCripto

    Module Program
        Private _failures As New List(Of String)()

        Sub Main()
            Run("ExecutionSnapshot.HasBook", AddressOf TestExecutionSnapshot)
            Run("MarketDataAvailabilityGate", AddressOf TestMarketDataAvailabilityGate)
            Run("BacktestAcceptsExecutionConstraints", AddressOf TestBacktestAcceptsExecutionConstraints)
            Run("RestartExecutionSettingsRoundTrip", AddressOf TestRestartExecutionSettingsRoundTrip)
            Run("DuplicateOrderIntentGuard", AddressOf TestDuplicateOrderIntentGuard)
            Run("GridEntryRiskAtFill", AddressOf TestGridEntryRiskAtFill)
            Run("EmergencyRecoveryRequiresClosedCandle", AddressOf TestEmergencyRecovery)
            Run("DelayedExitUsesOwnSymbol", AddressOf TestDelayedExitUsesOwnSymbol)
            Run("ReplayOrdersMultiAssetTicks", AddressOf TestReplayOrdersMultiAssetTicks)
            Run("BinancePartialBook", AddressOf TestBinancePartialBook)
            Run("ExecutionBookReplayIsCausal", AddressOf TestExecutionBookReplayIsCausal)
            Run("WalkForwardSeparatesCarriedTrades", AddressOf TestWalkForwardSeparatesCarriedTrades)
            Run("GridPartialBrakeKeepsLevelsAndCashConsistent", AddressOf TestGridPartialBrakeKeepsLevelsAndCashConsistent)
            Run("GridStopUsesRecordedTradesAndFillBook", AddressOf TestRecordedGridStop)
            Run("GridDelayedTargetClosesOnlyOnce", AddressOf TestGridDelayedTargetClosesOnlyOnce)
            Run("HistoricalBitcoinFlushUsesReplayClock", AddressOf TestHistoricalBitcoinFlushUsesReplayClock)
            Run("BacktestKeepsPaperState", AddressOf TestBacktestKeepsPaperState)
            Run("RestoreSkipsHistoricalRuns", AddressOf TestRestoreSkipsHistoricalRuns)

            If _failures.Count > 0 Then
                Console.Error.WriteLine("FALHAS:")
                For Each failure In _failures
                    Console.Error.WriteLine(" - " & failure)
                Next
                Environment.ExitCode = 1
                Return
            End If

            Console.WriteLine("TradeBot self-tests: OK")
        End Sub

        Private Sub Run(name As String, test As Action)
            Try
                test()
                Console.WriteLine("PASS " & name)
            Catch ex As Exception
                _failures.Add(name & ": " & ex.Message)
            End Try
        End Sub

        Private Sub Assert(condition As Boolean, message As String)
            If Not condition Then Throw New InvalidOperationException(message)
        End Sub

        Private Sub TestExecutionSnapshot()
            Dim snapshot = New SpotBotExecutionSnapshot With {
                .BidPrice = 99D,
                .AskPrice = 101D,
                .BidDepthQuote = 500D,
                .AskDepthQuote = 400D}
            Assert(snapshot.HasBook, "snapshot com bid/ask não foi reconhecido como livro")
        End Sub

        Private Sub TestMarketDataAvailabilityGate()
            Dim engine = New SpotTradeBotEngine(persistRuns:=False)
            engine.Start()
            engine.SetMarketDataAvailability(False)
            Assert(Not engine.MarketDataAvailable, "a trava offline não foi ativada")
            engine.SetMarketDataAvailability(True)
            Assert(engine.MarketDataAvailable, "a trava online não foi liberada")
            engine.Stop()
        End Sub

        Private Sub TestBacktestAcceptsExecutionConstraints()
            Dim engine = New SpotTradeBotEngine(persistRuns:=False)
            engine.DisableAllStrategies()
            engine.SetStrategyEnabled(SpotBotStrategyKind.BaseOriginalProtected, True)
            engine.Configuration.BollingerCandleMinutes = 1
            engine.Configuration.SimulationUseExecutionBook = True
            Dim candles As New List(Of SpotBotBacktestCandle)()
            Dim startUtc = DateTime.SpecifyKind(DateTime.UtcNow.Date.AddDays(-2), DateTimeKind.Utc)
            For index = 0 To 39
                Dim closePrice = 100D + (index Mod 5)
                candles.Add(New SpotBotBacktestCandle With {
                    .Symbol = "BTCUSDT",
                    .TimestampUtc = startUtc.AddMinutes(index),
                    .IntervalMinutes = 1,
                    .Open = closePrice,
                    .High = closePrice + 1D,
                    .Low = closePrice - 1D,
                    .Close = closePrice,
                    .Volume = 10000D,
                    .BidPrice = closePrice - 0.01D,
                    .AskPrice = closePrice + 0.01D,
                    .BidDepthQuote = 500D,
                    .AskDepthQuote = 500D,
                    .PriceTickSize = 0.01D,
                    .QuantityStep = 0.001D,
                    .MinimumNotional = 10D,
                    .IsClosed = True})
            Next

            Dim result = engine.RunDeterministicBacktest(
                candles,
                New SpotBotBacktestOptions With {
                    .ScenarioName = "SelfTest",
                    .ExecutionDelaySeconds = 1,
                    .ExitExecutionDelaySeconds = 1,
                    .ApplyLatencyToExits = True,
                    .UseExecutionBook = True,
                    .RequireOneMinuteBaseForGrid = False})
            Assert(result IsNot Nothing, "backtest não retornou resultado")
            Assert(result.CandlesProcessed = candles.Count, "quantidade de candles processados divergente")
            Assert(result.ExecutionBookCandles = candles.Count, "cobertura de livro dos candles divergente")
            engine.Stop()
        End Sub

        Private Sub TestRestartExecutionSettingsRoundTrip()
            Dim original = New SpotBotSimulationExecutionSettings With {
                .SpreadPercent = 0.12D,
                .ExecutionDelaySeconds = 7,
                .ExitExecutionDelaySeconds = 11,
                .UseExecutionBook = True,
                .PriceTickSize = 0.01D,
                .QuantityStep = 0.001D,
                .MinimumNotional = 10D,
                .TakerFeeRate = 0.001D,
                .ProtectiveOrdersEnabled = False,
                .PaperTradingCheckpointSeconds = 60}
            Dim json = JsonSerializer.Serialize(original)
            Dim restored = JsonSerializer.Deserialize(Of SpotBotSimulationExecutionSettings)(json)
            Assert(restored IsNot Nothing, "configuração não foi desserializada")
            Assert(restored.ExecutionDelaySeconds = original.ExecutionDelaySeconds, "latência não sobreviveu ao round-trip")
            Assert(restored.MinimumNotional = original.MinimumNotional, "minimum notional não sobreviveu ao round-trip")
            Assert(restored.PaperTradingCheckpointSeconds = original.PaperTradingCheckpointSeconds, "checkpoint não sobreviveu ao round-trip")
        End Sub

        Private Sub TestDuplicateOrderIntentGuard()
            Dim guard = New SpotBotOrderIntentDeduplicator()
            Assert(guard.TryBegin("BINANCE|PENDLE|SELL|SL|1"), "primeira intenção deveria ser aceita")
            Assert(Not guard.TryBegin("BINANCE|PENDLE|SELL|SL|1"), "intenção duplicada foi aceita")
            guard.Complete("BINANCE|PENDLE|SELL|SL|1")
            Assert(guard.TryBegin("BINANCE|PENDLE|SELL|SL|1"), "intenção não foi liberada após conclusão")
            guard.Clear()
        End Sub

        Private Sub TestGridEntryRiskAtFill()
            Dim config As New SpotBotConfiguration With {.GridV2MaximumStopPercent = 2D}
            Dim signal As New SpotBotSignal With {.GridBuyLevel = 99D, .StopPrice = 98D, .TakeProfitPrice = 103D}
            Assert(SpotTradeBotEngine.GridEntryRejection(signal, 100D, 0.001D, config) Is Nothing, "fill no limite deveria passar")
            Assert(SpotTradeBotEngine.GridEntryRejection(signal, 100.1D, 0.001D, config) IsNot Nothing, "fill acima do risco aceito")
            signal.TakeProfitPrice = 102D
            Assert(SpotTradeBotEngine.GridEntryRejection(signal, 100D, 0.001D, config) IsNot Nothing, "taxas ignoradas no retorno")
        End Sub

        Private Sub TestEmergencyRecovery()
            Dim start = New DateTime(2026, 11, 1, 0, 0, 0, DateTimeKind.Utc)
            Dim candle As New SpotBotPricePoint(start, 101D, 99D, 102D, 98D)
            Assert(Not SpotTradeBotEngine.HasEmergencyRecovery(candle, start.AddSeconds(1), start.AddMinutes(5), 100D, 103D), "candle anterior ao stop")
            Assert(Not SpotTradeBotEngine.HasEmergencyRecovery(candle, start, start.AddMinutes(4), 100D, 103D), "candle ainda aberto")
            Assert(Not SpotTradeBotEngine.HasEmergencyRecovery(candle, start, start.AddMinutes(5), 100D, 102D), "faltou romper máxima")
            Assert(SpotTradeBotEngine.HasEmergencyRecovery(candle, start, start.AddMinutes(5), 100D, 103D), "recuperação válida bloqueada")
        End Sub

        Private Sub TestDelayedExitUsesOwnSymbol()
            Const members As BindingFlags = BindingFlags.Instance Or BindingFlags.NonPublic
            Dim engine = New SpotTradeBotEngine(persistRuns:=False)
            engine.SetStrategyEnabled(SpotBotStrategyKind.BaseOriginalProtected, True)
            engine.Configuration.SimulationExitExecutionDelaySeconds = 1
            engine.Configuration.SimulationApplyLatencyToExits = True
            engine.Configuration.SimulationSlippagePercent = 0D
            engine.Configuration.SimulationDynamicSlippageEnabled = False
            engine.Configuration.SimulationSpreadPercent = 0D
            engine.Configuration.RiskBrakeEnabled = False
            engine.Start()
            Dim engineType = GetType(SpotTradeBotEngine)
            Dim opened = DateTime.UtcNow.AddMinutes(-2)
            Dim positionType = engineType.GetNestedType("SimulationPosition", BindingFlags.NonPublic)
            Dim position = Activator.CreateInstance(positionType, nonPublic:=True)
            positionType.GetProperty("Symbol").SetValue(position, "PENDLE")
            positionType.GetProperty("EntryPrice").SetValue(position, 2D)
            positionType.GetProperty("LastPrice").SetValue(position, 2D)
            positionType.GetProperty("Quantity").SetValue(position, 10D)
            positionType.GetProperty("CostBasis").SetValue(position, 20D)
            positionType.GetProperty("EntryTimestampUtc").SetValue(position, opened)
            Dim positions = DirectCast(engineType.GetField("_simulationPositions", members).GetValue(engine), IDictionary)
            positions.Add("PENDLE", position)

            Dim queuedAt = opened.AddSeconds(30)
            Dim observation = SpotTradeBotEngine.ObserveStop(queuedAt, 2D, New SpotBotExecutionSnapshot With {
                .BidPrice = 1.99D, .CapturedAtUtc = queuedAt.AddMilliseconds(-250)})
            Dim queued = CBool(engineType.GetMethod("QueuePendingBacktestExitLocked", members).
                Invoke(engine, {"PENDLE", queuedAt, "SL", 10D, 0D, engine.Configuration, observation}))
            Assert(queued, "a saída não entrou na fila")
            engine.ProcessPrice("BTC", 80000D, queuedAt.AddSeconds(2))
            Assert(positions.Contains("PENDLE"), "a cotação de BTC encerrou PENDLE")
            engine.ProcessPrice("PENDLE", 2.1D, queuedAt.AddSeconds(3),
                                executionSnapshot:=New SpotBotExecutionSnapshot With {.BidPrice = 2.09D, .AskPrice = 2.11D})
            Dim trades = engine.GetLastCompletedTrades()
            Assert(trades.Count = 1 AndAlso trades(0).Symbol = "PENDLE" AndAlso trades(0).ExitPrice = 2.09D,
                   $"a saída não usou o livro de PENDLE: trades={trades.Count}, preço={trades.FirstOrDefault()?.ExitPrice}")
            Dim trigger = trades(0).StopObservation
            Assert(trigger.TriggerUtc = queuedAt AndAlso trigger.BestBid = 1.99D AndAlso trigger.BookAgeMs = 250 AndAlso
                   trigger.TriggerPrice = 2D AndAlso trades(0).ClosedAtUtc > trigger.TriggerUtc, "disparo substituído pelos dados do fill")
        End Sub

        Private Sub TestReplayOrdersMultiAssetTicks()
            Dim start = DateTime.UtcNow.Date
            Dim candles = New List(Of SpotBotBacktestCandle) From {
                New SpotBotBacktestCandle With {.Symbol = "BTC", .TimestampUtc = start, .IntervalMinutes = 5, .Open = 100D, .Low = 99D, .High = 101D, .Close = 100D},
                New SpotBotBacktestCandle With {.Symbol = "PENDLE", .TimestampUtc = start.AddMinutes(1), .IntervalMinutes = 1, .Open = 2D, .Low = 1D, .High = 3D, .Close = 2D}}
            Dim method = GetType(SpotTradeBotEngine).GetMethod("EnumerateBacktestPricePoints", BindingFlags.Static Or BindingFlags.NonPublic)
            Dim points = DirectCast(method.Invoke(Nothing, {candles, SpotBotBacktestPathMode.ConservativeLong}), IEnumerable)
            Dim previous = DateTime.MinValue
            Dim total = 0
            For Each item In points
                Dim point = DirectCast(item, ITuple)(1)
                Dim timestamp = CDate(point.GetType().GetProperty("TimestampUtc").GetValue(point))
                Assert(timestamp >= previous, "replay voltou no tempo entre símbolos")
                previous = timestamp
                total += 1
            Next
            Assert(total = 8, "pontos intrabar ausentes")
        End Sub

        Private Sub TestBinancePartialBook()
            Dim socket As New BinanceWebSocket()
            Dim readMessage = GetType(BinanceWebSocket).GetMethod("ProcessMessage", BindingFlags.Instance Or BindingFlags.NonPublic)
            readMessage.Invoke(socket, {"{""stream"":""pendleusdt@depth5@100ms"",""data"":{""lastUpdateId"":123,""bids"":[[""2.00"",""12""],[""1.99"",""10""]],""asks"":[[""2.01"",""20""]]}}"})
            Dim snapshot As SpotBotExecutionSnapshot = Nothing
            Assert(socket.TryGetExecutionSnapshot("PENDLE", snapshot), "snapshot parcial da Binance foi descartado")
            Assert(snapshot.BidPrice = 2D AndAlso snapshot.AskPrice = 2.01D AndAlso
                   snapshot.BidDepthQuote = 43.9D AndAlso snapshot.AskDepthQuote = 40.2D, "livro parcial incorreto")
            snapshot.CapturedAtUtc = DateTime.UtcNow.AddSeconds(-6)
            Assert(Not socket.TryGetExecutionSnapshot("PENDLE", snapshot), "livro vencido foi aceito")
        End Sub

        Private Sub TestExecutionBookReplayIsCausal()
            Dim filePath = Path.GetTempFileName()
            Try
                File.WriteAllLines(filePath, {
                    "Symbol;CapturedAtUtc;BidPrice;AskPrice;BidDepthQuote;AskDepthQuote",
                    "PENDLE;2026-01-01T10:00:10Z;2;2.01;100;100",
                    "PENDLE;2026-01-01T10:00:12Z;3;3.01;100;100"})
                Dim replay = SpotBotExecutionBookReplay.Load(filePath)
                Dim first = New DateTime(2026, 1, 1, 10, 0, 10, DateTimeKind.Utc)
                Assert(replay.FindSnapshot("PENDLE", first.AddSeconds(-1)) Is Nothing, "livro futuro foi aceito")
                Assert(replay.FindSnapshot("PENDLEUSDT", first.AddSeconds(1))?.BidPrice = 2D, "livro válido não encontrado")
                Assert(replay.FindSnapshot("BTC", first) Is Nothing, "livro de outro ativo foi aceito")
                Assert(replay.FindSnapshot("PENDLE", first.AddSeconds(8)) Is Nothing, "livro vencido foi aceito")

                Dim engine = New SpotTradeBotEngine(persistRuns:=False)
                engine.DisableAllStrategies()
                engine.SetStrategyEnabled(SpotBotStrategyKind.BaseOriginalProtected, True)
                Dim candles = {
                    New SpotBotBacktestCandle With {.Symbol = "PENDLE", .TimestampUtc = first, .IntervalMinutes = 1, .Open = 2D, .High = 2D, .Low = 2D, .Close = 2D, .IsClosed = True},
                    New SpotBotBacktestCandle With {.Symbol = "BTC", .TimestampUtc = first, .IntervalMinutes = 1, .Open = 100D, .High = 100D, .Low = 100D, .Close = 100D, .IsClosed = True}}
                Dim result = engine.RunDeterministicBacktest(candles, New SpotBotBacktestOptions With {.ExecutionBookReplay = replay})
                Assert(result.ExecutionBookCandles = 1, "o replay não associou o CSV à execução pelo símbolo")
            Finally
                File.Delete(filePath)
            End Try
        End Sub

        Private Sub TestWalkForwardSeparatesCarriedTrades()
            Dim start = DateTime.UtcNow.Date
            Dim finish = start.AddDays(1)
            Dim summary As New SpotBotWalkForwardSummary With {.TestStartUtc = start, .TestEndUtc = finish}
            summary.RecordTestTrades({
                New SpotBotSimulationTrade With {.OpenedAtUtc = start.AddSeconds(-1), .ClosedAtUtc = start, .Pnl = 12D},
                New SpotBotSimulationTrade With {.OpenedAtUtc = start, .ClosedAtUtc = start.AddHours(1), .Pnl = 3D, .Fees = 0.2D},
                New SpotBotSimulationTrade With {.OpenedAtUtc = start, .ClosedAtUtc = finish, .Pnl = 99D}})
            Assert(summary.TestTrades = 1 AndAlso summary.TestPnl = 3D AndAlso summary.TestFees = 0.2D, "métricas fora da amostra contaminadas")
            Assert(summary.CarriedTrades = 1 AndAlso summary.CarriedPnl = 12D, "posição do treino não foi separada")
        End Sub
        Private Const PrivateInstance As BindingFlags = BindingFlags.Instance Or BindingFlags.NonPublic

        Private Sub RestoreSimulation(engine As SpotTradeBotEngine, snapshot As SpotBotSimulationSnapshot,
                                      positions As IEnumerable(Of SpotBotOpenPosition))
            Dim state As New SpotBotPersistedSimulationState With {
                .Snapshot = snapshot,
                .Positions = positions.ToList()}
            state.EnabledStrategies.Add(SpotBotStrategyKind.GridAdaptive)
            GetType(SpotTradeBotEngine).GetMethod("RestoreSimulationLocked", PrivateInstance).Invoke(engine, {state})
        End Sub

        Private Function CreateGridWithOpenLevel(opened As DateTime) As SpotTradeBotEngine
            Dim engine As New SpotTradeBotEngine(persistRuns:=False)
            Dim level As New SpotBotGridLevel With {
                .Quantity = 100D, .CostBasis = 200D, .EntryPrice = 2D,
                .TakeProfitPrice = 2.1D, .StopPrice = 1.9D, .OpenedAtUtc = opened}
            Dim position As New SpotBotOpenPosition With {
                .Symbol = "PENDLE", .Strategy = "Grid V2", .GridManaged = True,
                .Quantity = level.Quantity, .CostBasis = level.CostBasis, .EntryPrice = level.EntryPrice,
                .CurrentPrice = level.EntryPrice, .OpenedAtUtc = opened,
                .GridLevels = New List(Of SpotBotGridLevel) From {level}}
            RestoreSimulation(engine, New SpotBotSimulationSnapshot With {.StartingQuote = 1000D, .QuoteBalance = 800D}, {position})
            engine.Configuration.RiskBrakeEnabled = False
            engine.Configuration.SimulationFeeRate = 0D
            engine.Configuration.SimulationSlippagePercent = 0D
            engine.Configuration.SimulationDynamicSlippageEnabled = False
            engine.Configuration.SimulationUseExecutionBook = True
            engine.Configuration.GridV2PartialExitPercent = 50D
            engine.Configuration.BreakEvenOffsetPercent = 0.1D
            engine.Start()
            Return engine
        End Function

        Private Sub TestGridPartialBrakeKeepsLevelsAndCashConsistent()
            Dim opened = New DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            Dim engine = CreateGridWithOpenLevel(opened)
            Dim engineType = GetType(SpotTradeBotEngine)
            engineType.GetField("_activeExecutionSnapshot", PrivateInstance).SetValue(engine,
                New SpotBotExecutionSnapshot With {.BidPrice = 2D, .AskPrice = 2.01D, .BidDepthQuote = 80D})
            Dim trades As New List(Of SpotBotSimulationTrade)()
            engineType.GetMethod("CloseSimulationPositionsOnHardStopLocked", PrivateInstance).
                Invoke(engine, {"PENDLE", opened.AddMinutes(1), engine.Configuration, trades})
            Dim position = engine.GetOpenPositions().Single()
            Dim snapshot = engine.GetSimulationSnapshot()
            Assert(trades.Single().Quantity = 40D AndAlso snapshot.QuoteBalance = 880D, "venda de 40 e crédito de 80 esperados")
            Assert(position.Quantity = 60D AndAlso position.GridLevels.Sum(Function(level) level.Quantity) = 60D,
                   "quantidade do agregado e dos níveis deveria cair para 60")
            Assert(position.CostBasis = 120D AndAlso position.GridLevels.Sum(Function(level) level.CostBasis) = 120D,
                   "o custo restante deveria cair para 120")
            Dim restarted As New SpotTradeBotEngine(persistRuns:=False)
            RestoreSimulation(restarted, snapshot, {position})
            Assert(restarted.GetOpenPositions().Single().GridLevels.Single().Quantity = 60D AndAlso
                   restarted.GetSimulationSnapshot().QuoteBalance = 880D, "a restauração divergiu do saldo e dos níveis")
        End Sub

        Private Sub TestRecordedGridStop()
            Dim start As New DateTime(2026, 11, 23, 1, 0, 0, DateTimeKind.Utc)
            Dim engine = CreateGridWithOpenLevel(start.AddMinutes(-1))
            Dim csv = Path.GetTempFileName()
            Try
                File.WriteAllLines(csv, {"Symbol;CapturedAtUtc;BidPrice;AskPrice;BidDepthQuote;AskDepthQuote",
                    "PENDLE;2026-11-23T01:00:23.100Z;1.87;1.88;10000;10000",
                    "PENDLE;2026-11-23T01:00:53.900Z;1.85;1.86;10000;10000"})
                Dim ticks As SpotBotPricePoint() = {
                    New SpotBotPricePoint(start.AddMilliseconds(14231), 1.91D, volume:=100D),
                    New SpotBotPricePoint(start.AddMilliseconds(23768), 1.88D, volume:=100D),
                    New SpotBotPricePoint(start.AddMilliseconds(42371), 1.84D, volume:=100D),
                    New SpotBotPricePoint(start.AddMilliseconds(53963), 1.86D, volume:=100D)}
                Dim candle As New SpotBotBacktestCandle With {.Symbol = "PENDLE", .TimestampUtc = start,
                    .IntervalMinutes = 1, .Open = 2D, .High = 2D, .Low = 1.84D, .Close = 1.86D, .Volume = 400D}
                Dim result = engine.RunDeterministicBacktest({candle}, New SpotBotBacktestOptions With {
                    .ResetBeforeRun = False, .ExitExecutionDelaySeconds = 30, .DynamicSlippageEnabled = False,
                    .ExecutionBookReplay = SpotBotExecutionBookReplay.Load(csv),
                    .ReadGridStopTrades = Function(symbol, minute) ticks})
                Dim trade = result.Trades.Single()
                Assert(trade.ExitReason = "SL_EMERGENCY" AndAlso trade.StopObservation.TriggerUtc = ticks(1).TimestampUtc AndAlso
                       trade.StopObservation.TriggerPrice = 1.88D AndAlso trade.StopObservation.BestBid = 1.87D,
                       "o OHLC disparou o stop antes do negócio real")
                Assert(trade.ClosedAtUtc = ticks(3).TimestampUtc AndAlso trade.ExitPrice = 1.85D AndAlso
                       result.AggregateStopTicks = 4, "o fill deve usar o primeiro negócio após a latência e seu livro")
            Finally
                File.Delete(csv)
            End Try
        End Sub

        Private Sub TestGridDelayedTargetClosesOnlyOnce()
            Dim opened = New DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            Dim engine = CreateGridWithOpenLevel(opened)
            engine.Configuration.SimulationApplyLatencyToExits = True
            engine.Configuration.SimulationExitExecutionDelaySeconds = 1

            Dim reachedAt = opened.AddSeconds(10)
            engine.ProcessPrice("PENDLE", 2.11D, reachedAt)
            Assert(engine.GetLastCompletedTrades().Count = 0, "alvo executado antes da latência")
            engine.ProcessPrice("PENDLE", 2.11D, reachedAt.AddSeconds(1))
            Assert(engine.GetLastCompletedTrades().Single().Quantity = 50D, "a primeira execução deveria vender 50")
            For seconds = 2 To 5
                engine.ProcessPrice("PENDLE", 2.11D, reachedAt.AddSeconds(seconds))
                Assert(engine.GetLastCompletedTrades().Count = 0, "alvo parcial executado novamente")
            Next
            Dim level = engine.GetOpenPositions().Single().GridLevels.Single()
            Assert(level.Quantity = 50D AndAlso level.PartialTargetTaken AndAlso
                   level.TakeProfitPrice = 0D AndAlso level.StopPrice >= 2.002D,
                   "a parcial deveria proteger as 50 unidades restantes")
        End Sub

        Private Sub TestHistoricalBitcoinFlushUsesReplayClock()
            Dim engine As New SpotTradeBotEngine(persistRuns:=False)
            engine.SetStrategyEnabled(SpotBotStrategyKind.BaseOriginalProtected, True)
            engine.Configuration.BitcoinFlushProtectionEnabled = True
            engine.Configuration.BitcoinFlushPauseMinutes = 30D
            engine.Configuration.BitcoinDailyProtectionEnabled = False
            Dim start = New DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            Dim candles = Enumerable.Range(0, 31).Select(Function(minute) New SpotBotBacktestCandle With {
                .Symbol = "BTC", .TimestampUtc = start.AddMinutes(minute), .IntervalMinutes = 1,
                .Open = 100D, .High = 100D, .Low = If(minute = 30, 95D, 100D),
                .Close = If(minute = 30, 95D, 100D), .Volume = If(minute = 30, 1000D, 100D),
                .IsClosed = True}).ToList()
            engine.RunDeterministicBacktest(candles)
            Dim pauseUntil = CDate(GetType(SpotTradeBotEngine).GetField("_bitcoinFlushPausedUntilUtc", PrivateInstance).GetValue(engine))
            Assert(pauseUntil = start.AddMinutes(61).AddTicks(-1), $"vencimento histórico incorreto: {pauseUntil:O}")
        End Sub
        Private Sub TestBacktestKeepsPaperState()
            Dim opened = New DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            Dim paper = CreateGridWithOpenLevel(opened)
            paper.Stop()
            RestoreSimulation(paper, paper.GetSimulationSnapshot(), paper.GetOpenPositions())
            Dim backtest = paper.CreateBacktestEngine(persistRuns:=False)
            Assert(backtest.GetOpenPositions().Count = 0 AndAlso Not backtest.HasRestoredState,
                   "o backtest herdou a carteira do paper")
            Assert(backtest.Configuration.EnabledStrategies.Contains(SpotBotStrategyKind.GridAdaptive),
                   "o backtest não copiou a estratégia")
            backtest.Configuration.SimulationStartingQuote = 500D
            Dim candle As New SpotBotBacktestCandle With {
                .Symbol = "PENDLE", .TimestampUtc = opened.AddDays(-2), .IntervalMinutes = 1,
                .Open = 2D, .High = 2D, .Low = 2D, .Close = 2D, .IsClosed = True}
            backtest.RunDeterministicBacktest({candle}, New SpotBotBacktestOptions With {.GapToleranceIntervals = 1})
            Assert(paper.GetOpenPositions().Single().Quantity = 100D AndAlso
                   paper.GetSimulationSnapshot().QuoteBalance = 800D AndAlso
                   paper.Configuration.SimulationStartingQuote = 1000D AndAlso paper.HasRestoredState,
                   "rodar backtest afetou o estado retomável do paper")
        End Sub

        Private Sub TestRestoreSkipsHistoricalRuns()
            Using connection As New SqliteConnection("Data Source=:memory:")
                connection.Open()
                Using command = connection.CreateCommand()
                    command.CommandText =
                        "CREATE TABLE SpotBotRuns (Id INTEGER, RunOrigin TEXT, StartedAt TEXT);" &
                        "CREATE TABLE SpotBotTrades (RunId INTEGER, ClosedAtUtc TEXT);" &
                        "CREATE TABLE SpotBotOpenPositions (RunId INTEGER, OpenedAtUtc TEXT);" &
                        "CREATE TABLE SpotBotExecutionSnapshots (RunId INTEGER, CapturedAtUtc TEXT);" &
                        "INSERT INTO SpotBotRuns VALUES (1,'LEGACY','2026-01-01T12:00:00Z')," &
                        "(2,'LEGACY','2026-01-01T12:00:00Z'),(3,'LEGACY','2026-01-01T12:00:00Z')," &
                        "(4,'LEGACY','2026-01-01T12:00:00Z'),(5,'BACKTEST','2026-01-01T12:00:00Z');" &
                        "INSERT INTO SpotBotTrades VALUES (1,'2026-01-01T13:00:00Z'),(2,'2025-01-01T13:00:00Z');" &
                        "INSERT INTO SpotBotOpenPositions VALUES (3,'2025-01-01T13:00:00Z');" &
                        "INSERT INTO SpotBotExecutionSnapshots VALUES (5,'2026-01-01T13:00:00Z');"
                    command.ExecuteNonQuery()
                    Dim selectRun = "SELECT Id FROM SpotBotRuns WHERE " &
                                    SpotTradeBotRepository.RestorablePaperRunFilter & " ORDER BY Id DESC LIMIT 1;"
                    command.CommandText = selectRun
                    Assert(CLng(command.ExecuteScalar()) = 1, "a restauração escolheu backtest ou run antiga ambígua")
                    command.CommandText = "INSERT INTO SpotBotRuns VALUES (6,'PAPER','2026-01-01T12:00:00Z')," &
                                          "(7,'BACKTEST','2026-01-01T12:00:00Z');"
                    command.ExecuteNonQuery()
                    command.CommandText = selectRun
                    Assert(CLng(command.ExecuteScalar()) = 6, "a run de paper mais recente deveria ser retomável mesmo sem trades")
                End Using
            End Using
        End Sub
    End Module
