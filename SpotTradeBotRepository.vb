Imports Microsoft.Data.Sqlite
Imports System.Data
Imports System.Globalization
Imports System.Linq
Imports System.Text.Json

Public NotInheritable Class SpotTradeBotRepository

    Private Shared ReadOnly SyncRoot As New Object()
    Private Shared _initialized As Boolean

    Private Sub New()
    End Sub

    Public Shared Sub Initialize()
        If _initialized Then Return

        SyncLock SyncRoot
            If _initialized Then Return

            PortfolioRepository.Initialize()

            Using connection As New SqliteConnection(GetConnectionString())
                connection.Open()

                Using command As SqliteCommand = connection.CreateCommand()
                    command.CommandText =
                        "CREATE TABLE IF NOT EXISTS SpotBotRuns (" &
                        "Id INTEGER PRIMARY KEY AUTOINCREMENT, " &
                        "StartedAt TEXT NOT NULL, " &
                        "EndedAt TEXT, " &
                        "Status TEXT NOT NULL, " &
                        "EnabledStrategies TEXT NOT NULL, " &
                        "CombinationMode INTEGER NOT NULL, " &
                        "RequiredConfirmations INTEGER NOT NULL, " &
                        "StartingQuote REAL NOT NULL, " &
                        "AllocationPercent REAL NOT NULL, " &
                        "FeeRate REAL NOT NULL, " &
                        "CompletedTrades INTEGER NOT NULL DEFAULT 0, " &
                        "WinningTrades INTEGER NOT NULL DEFAULT 0, " &
                        "LosingTrades INTEGER NOT NULL DEFAULT 0, " &
                        "RealizedPnl REAL NOT NULL DEFAULT 0, " &
                        "UnrealizedPnl REAL NOT NULL DEFAULT 0, " &
                        "Equity REAL NOT NULL DEFAULT 0, " &
                        "TotalPnl REAL NOT NULL DEFAULT 0, " &
                        "QuoteBalance REAL NOT NULL DEFAULT 0, " &
                        "OpenValue REAL NOT NULL DEFAULT 0, " &
                        "OpenPositions INTEGER NOT NULL DEFAULT 0, " &
                        "LastAction TEXT, " &
                        "BollingerPeriod INTEGER NOT NULL DEFAULT 20, " &
                        "BollingerStandardDeviations REAL NOT NULL DEFAULT 3, " &
                        "BollingerCandleMinutes INTEGER NOT NULL DEFAULT 1440, " &
                         "BreakEvenEnabled INTEGER NOT NULL DEFAULT 1, " &
                         "BreakEvenTriggerPercent REAL NOT NULL DEFAULT 1.5, " &
                         "BreakEvenOffsetPercent REAL NOT NULL DEFAULT 0.1, " &
                         "TrailingStopEnabled INTEGER NOT NULL DEFAULT 0, " &
                         "TrailingStopDistancePercent REAL NOT NULL DEFAULT 1, " &
                        "PostExitCooldownMinutes REAL NOT NULL DEFAULT 5, " &
                        "NewPositionProfitGateEnabled INTEGER NOT NULL DEFAULT 1, " &
                        "MinimumNetProfitBeforeNewPositionPercent REAL NOT NULL DEFAULT 0.5, " &
                         "GridV2UseNearestSupportStop INTEGER NOT NULL DEFAULT 1, " &
                         "GridV2SupportBufferAtr REAL NOT NULL DEFAULT 1, " &
                         "GridV2MinimumStopPercent REAL NOT NULL DEFAULT 1.5, " &
                         "GridV2MaximumStopPercent REAL NOT NULL DEFAULT 3, " &
                        "UniverseMinimumTrades24h INTEGER NOT NULL DEFAULT 1000, " &
                         "GridV2MinimumSupportDistancePercent REAL NOT NULL DEFAULT 0.8, " &
                         "GridV2MaxExposurePercent REAL NOT NULL DEFAULT 30, " &
                         "GridV2RequireRejection INTEGER NOT NULL DEFAULT 1, " &
                         "GridV2RiskPerTradePercent REAL NOT NULL DEFAULT 0.5, " &
                         "GridV2ReserveQuotePercent REAL NOT NULL DEFAULT 20, " &
                         "GridV2MaxLevels INTEGER NOT NULL DEFAULT 5, " &
                         "BitcoinFlushProtectionEnabled INTEGER NOT NULL DEFAULT 1, " &
                         "BitcoinFlushLookbackMinutes INTEGER NOT NULL DEFAULT 5, " &
                         "BitcoinFlushDropPercent REAL NOT NULL DEFAULT 2, " &
                         "BitcoinFlushCandleDropPercent REAL NOT NULL DEFAULT 1.5, " &
                         "BitcoinFlushVolumeMultiplier REAL NOT NULL DEFAULT 1.8, " &
                         "BitcoinFlushVolumeLookback INTEGER NOT NULL DEFAULT 20, " &
                         "BitcoinFlushPauseMinutes REAL NOT NULL DEFAULT 30, " &
                         "BitcoinDailyProtectionEnabled INTEGER NOT NULL DEFAULT 1, " &
                         "BitcoinDailyDropPercent REAL NOT NULL DEFAULT 4, " &
                         "BitcoinDailyDrawdownPercent REAL NOT NULL DEFAULT 5, " &
                        "RiskBrakeEnabled INTEGER NOT NULL DEFAULT 1, " &
                        "RiskBrakeSessionLossPercent REAL NOT NULL DEFAULT 2, " &
                        "RiskBrakeProfitLockEnabled INTEGER NOT NULL DEFAULT 1, " &
                        "RiskBrakeProfitLockTriggerPercent REAL NOT NULL DEFAULT 1, " &
                        "RiskBrakeProfitGivebackPercent REAL NOT NULL DEFAULT 50, " &
                        "RiskBrakeMaxConsecutiveLosses INTEGER NOT NULL DEFAULT 3, " &
                        "RiskBrakePauseMinutes REAL NOT NULL DEFAULT 60, " &
                        "RiskBrakeClosePositionsOnHardStop INTEGER NOT NULL DEFAULT 1, " &
                        "RiskBrakeUseUnrealizedPnl INTEGER NOT NULL DEFAULT 1, " &
                        "RiskBrakeState INTEGER NOT NULL DEFAULT 0, " &
                        "RiskBrakePeakEquity REAL NOT NULL DEFAULT 0, " &
                        "RiskBrakeConsecutiveLosses INTEGER NOT NULL DEFAULT 0, " &
                         "RiskBrakePauseUntilUtc TEXT, " &
                         "RiskBrakeReason TEXT, " &
                          "RiskBrakeBlockedEntries INTEGER NOT NULL DEFAULT 0, " &
                          "RiskBrakeEstimatedAvoidedPnl REAL NOT NULL DEFAULT 0, " &
                          "RiskBrakePauseCount INTEGER NOT NULL DEFAULT 0, " &
                          "RiskBrakeLastPauseStartedUtc TEXT, " &
                          "RiskBrakeLastPauseEndedUtc TEXT, " &
                         "TakeProfitPercent REAL NOT NULL DEFAULT 2, " &
                         "StopLossPercent REAL NOT NULL DEFAULT 1, " &
                        "GridV2BtcMildDeclineEnabled INTEGER NOT NULL DEFAULT 0, " &
                        "GridV2BtcMildDeclineMinimumPercent REAL NOT NULL DEFAULT 0.5, " &
                        "GridV2BtcMildDeclineMaximumPercent REAL NOT NULL DEFAULT 2, " &
                        "RiskBrakeManualRealizedPnl REAL NOT NULL DEFAULT 0, " &
                        "GridV2MaximumHoldingMinutes INTEGER NOT NULL DEFAULT 240, " &
                        "GridV2RegimeExitEnabled INTEGER NOT NULL DEFAULT 1, " &
                        "GridV2ProgressiveExposureEnabled INTEGER NOT NULL DEFAULT 1, " &
                        "GridV2ProgressiveExposureReductionPercent REAL NOT NULL DEFAULT 25, " &
                        "SlippagePercent REAL NOT NULL DEFAULT 0.05, " &
                        "UpdatedAt TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP" &
                        ");"
                    command.ExecuteNonQuery()
                End Using

                EnsureColumn(connection, "SpotBotRuns", "BollingerPeriod", "INTEGER NOT NULL DEFAULT 20")
                EnsureColumn(connection, "SpotBotRuns", "BollingerStandardDeviations", "REAL NOT NULL DEFAULT 3")
                EnsureColumn(connection, "SpotBotRuns", "BollingerCandleMinutes", "INTEGER NOT NULL DEFAULT 1440")
                EnsureColumn(connection, "SpotBotRuns", "BreakEvenEnabled", "INTEGER NOT NULL DEFAULT 1")
                EnsureColumn(connection, "SpotBotRuns", "BreakEvenTriggerPercent", "REAL NOT NULL DEFAULT 1.5")
                EnsureColumn(connection, "SpotBotRuns", "BreakEvenOffsetPercent", "REAL NOT NULL DEFAULT 0.1")
                EnsureColumn(connection, "SpotBotRuns", "TrailingStopEnabled", "INTEGER NOT NULL DEFAULT 0")
                EnsureColumn(connection, "SpotBotRuns", "TrailingStopDistancePercent", "REAL NOT NULL DEFAULT 1")
                EnsureColumn(connection, "SpotBotRuns", "PostExitCooldownMinutes", "REAL NOT NULL DEFAULT 5")
                EnsureColumn(connection, "SpotBotRuns", "NewPositionProfitGateEnabled", "INTEGER NOT NULL DEFAULT 1")
                EnsureColumn(connection, "SpotBotRuns", "MinimumNetProfitBeforeNewPositionPercent", "REAL NOT NULL DEFAULT 0.5")
                EnsureColumn(connection, "SpotBotRuns", "GridV2UseNearestSupportStop", "INTEGER NOT NULL DEFAULT 1")
                EnsureColumn(connection, "SpotBotRuns", "GridV2SupportBufferAtr", "REAL NOT NULL DEFAULT 1")
                EnsureColumn(connection, "SpotBotRuns", "GridV2MinimumStopPercent", "REAL NOT NULL DEFAULT 1.5")
                EnsureColumn(connection, "SpotBotRuns", "GridV2MaximumStopPercent", "REAL NOT NULL DEFAULT 3")
                EnsureColumn(connection, "SpotBotRuns", "UniverseMinimumTrades24h", "INTEGER NOT NULL DEFAULT 1000")
                 EnsureColumn(connection, "SpotBotRuns", "GridV2MinimumSupportDistancePercent", "REAL NOT NULL DEFAULT 0.8")
                EnsureColumn(connection, "SpotBotRuns", "GridV2MaxExposurePercent", "REAL NOT NULL DEFAULT 30")
                EnsureColumn(connection, "SpotBotRuns", "GridV2RequireRejection", "INTEGER NOT NULL DEFAULT 1")
                EnsureColumn(connection, "SpotBotRuns", "GridV2RiskPerTradePercent", "REAL NOT NULL DEFAULT 0.5")
                EnsureColumn(connection, "SpotBotRuns", "GridV2ReserveQuotePercent", "REAL NOT NULL DEFAULT 20")
                EnsureColumn(connection, "SpotBotRuns", "GridV2MaxLevels", "INTEGER NOT NULL DEFAULT 5")
                EnsureColumn(connection, "SpotBotRuns", "BitcoinFlushProtectionEnabled", "INTEGER NOT NULL DEFAULT 1")
                EnsureColumn(connection, "SpotBotRuns", "BitcoinFlushLookbackMinutes", "INTEGER NOT NULL DEFAULT 5")
                EnsureColumn(connection, "SpotBotRuns", "BitcoinFlushDropPercent", "REAL NOT NULL DEFAULT 2")
                EnsureColumn(connection, "SpotBotRuns", "BitcoinFlushCandleDropPercent", "REAL NOT NULL DEFAULT 1.5")
                EnsureColumn(connection, "SpotBotRuns", "BitcoinFlushVolumeMultiplier", "REAL NOT NULL DEFAULT 1.8")
                EnsureColumn(connection, "SpotBotRuns", "BitcoinFlushVolumeLookback", "INTEGER NOT NULL DEFAULT 20")
                EnsureColumn(connection, "SpotBotRuns", "BitcoinFlushPauseMinutes", "REAL NOT NULL DEFAULT 30")
                EnsureColumn(connection, "SpotBotRuns", "BitcoinDailyProtectionEnabled", "INTEGER NOT NULL DEFAULT 1")
                EnsureColumn(connection, "SpotBotRuns", "BitcoinDailyDropPercent", "REAL NOT NULL DEFAULT 4")
                EnsureColumn(connection, "SpotBotRuns", "BitcoinDailyDrawdownPercent", "REAL NOT NULL DEFAULT 5")
                EnsureColumn(connection, "SpotBotRuns", "RiskBrakeEnabled", "INTEGER NOT NULL DEFAULT 1")
                EnsureColumn(connection, "SpotBotRuns", "RiskBrakeSessionLossPercent", "REAL NOT NULL DEFAULT 2")
                EnsureColumn(connection, "SpotBotRuns", "RiskBrakeProfitLockEnabled", "INTEGER NOT NULL DEFAULT 1")
                EnsureColumn(connection, "SpotBotRuns", "RiskBrakeProfitLockTriggerPercent", "REAL NOT NULL DEFAULT 1")
                EnsureColumn(connection, "SpotBotRuns", "RiskBrakeProfitGivebackPercent", "REAL NOT NULL DEFAULT 50")
                EnsureColumn(connection, "SpotBotRuns", "RiskBrakeMaxConsecutiveLosses", "INTEGER NOT NULL DEFAULT 3")
                EnsureColumn(connection, "SpotBotRuns", "RiskBrakePauseMinutes", "REAL NOT NULL DEFAULT 60")
                EnsureColumn(connection, "SpotBotRuns", "RiskBrakeClosePositionsOnHardStop", "INTEGER NOT NULL DEFAULT 1")
                EnsureColumn(connection, "SpotBotRuns", "RiskBrakeUseUnrealizedPnl", "INTEGER NOT NULL DEFAULT 1")
                EnsureColumn(connection, "SpotBotRuns", "TakeProfitPercent", "REAL NOT NULL DEFAULT 2")
                EnsureColumn(connection, "SpotBotRuns", "StopLossPercent", "REAL NOT NULL DEFAULT 1")
                EnsureColumn(connection, "SpotBotRuns", "GridV2BtcMildDeclineEnabled", "INTEGER NOT NULL DEFAULT 0")
                EnsureColumn(connection, "SpotBotRuns", "GridV2BtcMildDeclineMinimumPercent", "REAL NOT NULL DEFAULT 0.5")
                EnsureColumn(connection, "SpotBotRuns", "GridV2BtcMildDeclineMaximumPercent", "REAL NOT NULL DEFAULT 2")
                EnsureColumn(connection, "SpotBotRuns", "RiskBrakeManualRealizedPnl", "REAL NOT NULL DEFAULT 0")
                EnsureColumn(connection, "SpotBotRuns", "GridV2MaximumHoldingMinutes", "INTEGER NOT NULL DEFAULT 240")
                EnsureColumn(connection, "SpotBotRuns", "GridV2RegimeExitEnabled", "INTEGER NOT NULL DEFAULT 1")
                EnsureColumn(connection, "SpotBotRuns", "GridV2ProgressiveExposureEnabled", "INTEGER NOT NULL DEFAULT 1")
                EnsureColumn(connection, "SpotBotRuns", "GridV2ProgressiveExposureReductionPercent", "REAL NOT NULL DEFAULT 25")
                EnsureColumn(connection, "SpotBotRuns", "SlippagePercent", "REAL NOT NULL DEFAULT 0.05")
                EnsureColumn(connection, "SpotBotRuns", "RiskBrakeState", "INTEGER NOT NULL DEFAULT 0")
                EnsureColumn(connection, "SpotBotRuns", "RiskBrakePeakEquity", "REAL NOT NULL DEFAULT 0")
                EnsureColumn(connection, "SpotBotRuns", "RiskBrakeConsecutiveLosses", "INTEGER NOT NULL DEFAULT 0")
                EnsureColumn(connection, "SpotBotRuns", "RiskBrakePauseUntilUtc", "TEXT")
                EnsureColumn(connection, "SpotBotRuns", "RiskBrakeReason", "TEXT")
                EnsureColumn(connection, "SpotBotRuns", "RiskBrakeBlockedEntries", "INTEGER NOT NULL DEFAULT 0")
                EnsureColumn(connection, "SpotBotRuns", "RiskBrakeEstimatedAvoidedPnl", "REAL NOT NULL DEFAULT 0")
                EnsureColumn(connection, "SpotBotRuns", "RiskBrakePauseCount", "INTEGER NOT NULL DEFAULT 0")
                EnsureColumn(connection, "SpotBotRuns", "RiskBrakeLastPauseStartedUtc", "TEXT")
                EnsureColumn(connection, "SpotBotRuns", "RiskBrakeLastPauseEndedUtc", "TEXT")

                Using command As SqliteCommand = connection.CreateCommand()
                    command.CommandText =
                        "CREATE TABLE IF NOT EXISTS SpotBotCandles (" &
                        "Id INTEGER PRIMARY KEY AUTOINCREMENT, " &
                        "RunId INTEGER NOT NULL, " &
                        "Symbol TEXT NOT NULL, " &
                        "CandleStartUtc TEXT NOT NULL, " &
                        "ClosePrice REAL NOT NULL, " &
                        "CreatedAt TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP, " &
                        "UNIQUE(RunId, Symbol, CandleStartUtc), " &
                        "FOREIGN KEY(RunId) REFERENCES SpotBotRuns(Id) ON DELETE CASCADE" &
                        ");"
                    command.ExecuteNonQuery()
                End Using

                Using command As SqliteCommand = connection.CreateCommand()
                    command.CommandText =
                        "CREATE TABLE IF NOT EXISTS SpotBotTrades (" &
                        "Id INTEGER PRIMARY KEY AUTOINCREMENT, " &
                        "RunId INTEGER NOT NULL, " &
                        "Symbol TEXT NOT NULL, " &
                        "Quantity REAL NOT NULL, " &
                        "EntryPrice REAL NOT NULL, " &
                        "ExitPrice REAL NOT NULL, " &
                        "CostBasis REAL NOT NULL, " &
                        "GrossProceeds REAL NOT NULL, " &
                        "Fees REAL NOT NULL, " &
                        "Pnl REAL NOT NULL, " &
                        "SupportPrice REAL NOT NULL DEFAULT 0, " &
                        "InitialStopPrice REAL NOT NULL DEFAULT 0, " &
                        "ExitStopPrice REAL NOT NULL DEFAULT 0, " &
                        "Strategy TEXT NOT NULL DEFAULT '', " &
                        "ExitReason TEXT NOT NULL DEFAULT '', " &
                        "OpenedAtUtc TEXT NOT NULL, " &
                        "ClosedAtUtc TEXT NOT NULL, " &
                        "CreatedAt TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP, " &
                        "FOREIGN KEY(RunId) REFERENCES SpotBotRuns(Id) ON DELETE CASCADE" &
                        ");"
                    command.ExecuteNonQuery()
                End Using

                EnsureColumn(connection, "SpotBotTrades", "Strategy", "TEXT NOT NULL DEFAULT ''")
                EnsureColumn(connection, "SpotBotTrades", "ExitReason", "TEXT NOT NULL DEFAULT ''")
                EnsureColumn(connection, "SpotBotTrades", "SupportPrice", "REAL NOT NULL DEFAULT 0")
                EnsureColumn(connection, "SpotBotTrades", "InitialStopPrice", "REAL NOT NULL DEFAULT 0")
                EnsureColumn(connection, "SpotBotTrades", "ExitStopPrice", "REAL NOT NULL DEFAULT 0")

                Using command As SqliteCommand = connection.CreateCommand()
                    command.CommandText =
                        "CREATE TABLE IF NOT EXISTS SpotBotOpenPositions (" &
                        "Id INTEGER PRIMARY KEY AUTOINCREMENT, " &
                        "RunId INTEGER NOT NULL, " &
                        "Symbol TEXT NOT NULL, " &
                        "Side TEXT NOT NULL DEFAULT 'LONG', " &
                        "Quantity REAL NOT NULL, " &
                        "EntryPrice REAL NOT NULL, " &
                        "CurrentPrice REAL NOT NULL, " &
                        "CostBasis REAL NOT NULL, " &
                        "SupportPrice REAL NOT NULL DEFAULT 0, " &
                        "InitialStopPrice REAL NOT NULL DEFAULT 0, " &
                        "OpenedAtUtc TEXT NOT NULL, " &
                        "UpdatedAt TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP, " &
                        "UNIQUE(RunId, Symbol), " &
                        "FOREIGN KEY(RunId) REFERENCES SpotBotRuns(Id) ON DELETE CASCADE" &
                        ");"
                    command.ExecuteNonQuery()
                End Using

                ' Migração compatível com bases criadas antes do stop/alvo
                ' estruturais da estratégia de Bollinger.
                EnsureColumn(connection, "SpotBotOpenPositions", "StopPrice", "REAL NOT NULL DEFAULT 0")
                EnsureColumn(connection, "SpotBotOpenPositions", "TakeProfitPrice", "REAL NOT NULL DEFAULT 0")
                EnsureColumn(connection, "SpotBotOpenPositions", "PeakPrice", "REAL NOT NULL DEFAULT 0")
                EnsureColumn(connection, "SpotBotOpenPositions", "Strategy", "TEXT NOT NULL DEFAULT ''")
                EnsureColumn(connection, "SpotBotOpenPositions", "SupportPrice", "REAL NOT NULL DEFAULT 0")
                EnsureColumn(connection, "SpotBotOpenPositions", "InitialStopPrice", "REAL NOT NULL DEFAULT 0")
                EnsureColumn(connection, "SpotBotOpenPositions", "GridLevelCount", "INTEGER NOT NULL DEFAULT 0")
                EnsureColumn(connection, "SpotBotOpenPositions", "GridAnchorPrice", "REAL NOT NULL DEFAULT 0")
                EnsureColumn(connection, "SpotBotOpenPositions", "LastGridEntryPrice", "REAL NOT NULL DEFAULT 0")
                EnsureColumn(connection, "SpotBotOpenPositions", "GridSpacingPercent", "REAL NOT NULL DEFAULT 0")
                EnsureColumn(connection, "SpotBotOpenPositions", "GridManaged", "INTEGER NOT NULL DEFAULT 0")
                EnsureColumn(connection, "SpotBotOpenPositions", "GridLevelsJson", "TEXT NOT NULL DEFAULT '[]'")

                Using command As SqliteCommand = connection.CreateCommand()
                    command.CommandText =
                        "CREATE TABLE IF NOT EXISTS SpotBotGridGuards (" &
                        "RunId INTEGER NOT NULL, " &
                        "Symbol TEXT NOT NULL, " &
                        "ConsecutiveStops INTEGER NOT NULL DEFAULT 0, " &
                        "RecoveryEntryPrice REAL NOT NULL DEFAULT 0, " &
                        "BlockedUntilUtc TEXT, " &
                        "UpdatedAt TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP, " &
                        "PRIMARY KEY(RunId, Symbol), " &
                        "FOREIGN KEY(RunId) REFERENCES SpotBotRuns(Id) ON DELETE CASCADE" &
                        ");"
                    command.ExecuteNonQuery()
                End Using

                Using command As SqliteCommand = connection.CreateCommand()
                    command.CommandText =
                        "CREATE INDEX IF NOT EXISTS IX_SpotBotCandles_RunSymbol " &
                        "ON SpotBotCandles(RunId, Symbol, CandleStartUtc);"
                    command.ExecuteNonQuery()
                End Using

                Using command As SqliteCommand = connection.CreateCommand()
                    command.CommandText =
                        "CREATE INDEX IF NOT EXISTS IX_SpotBotTrades_Run " &
                        "ON SpotBotTrades(RunId, ClosedAtUtc);"
                    command.ExecuteNonQuery()
                End Using
            End Using

            _initialized = True
        End SyncLock
    End Sub

    Public Shared Function StartRun(configuration As SpotBotConfiguration) As Long
        Initialize()

        If configuration Is Nothing Then
            Throw New ArgumentNullException(NameOf(configuration))
        End If

        Using connection As New SqliteConnection(GetConnectionString())
            connection.Open()

            Using command As SqliteCommand = connection.CreateCommand()
                command.CommandText =
                    "INSERT INTO SpotBotRuns (" &
                    "StartedAt, Status, EnabledStrategies, CombinationMode, RequiredConfirmations, " &
                        "StartingQuote, AllocationPercent, FeeRate, Equity, QuoteBalance, " &
                        "BollingerPeriod, BollingerStandardDeviations, BollingerCandleMinutes, " &
                        "BreakEvenEnabled, BreakEvenTriggerPercent, BreakEvenOffsetPercent, " &
                        "TrailingStopEnabled, TrailingStopDistancePercent, PostExitCooldownMinutes, " &
                        "NewPositionProfitGateEnabled, MinimumNetProfitBeforeNewPositionPercent, " &
                        "GridV2UseNearestSupportStop, GridV2SupportBufferAtr, GridV2MinimumStopPercent, GridV2MaximumStopPercent, " &
                        "UniverseMinimumTrades24h, GridV2MinimumSupportDistancePercent, GridV2MaxExposurePercent, " &
                        "GridV2RequireRejection, GridV2RiskPerTradePercent, GridV2ReserveQuotePercent, GridV2MaxLevels, " &
                        "BitcoinFlushProtectionEnabled, BitcoinFlushLookbackMinutes, BitcoinFlushDropPercent, " &
                        "BitcoinFlushCandleDropPercent, BitcoinFlushVolumeMultiplier, BitcoinFlushVolumeLookback, " &
                        "BitcoinFlushPauseMinutes, BitcoinDailyProtectionEnabled, BitcoinDailyDropPercent, " &
                        "BitcoinDailyDrawdownPercent, RiskBrakeEnabled, RiskBrakeSessionLossPercent, " &
                        "RiskBrakeProfitLockEnabled, RiskBrakeProfitLockTriggerPercent, RiskBrakeProfitGivebackPercent, " &
                        "RiskBrakeMaxConsecutiveLosses, RiskBrakePauseMinutes, RiskBrakeClosePositionsOnHardStop, " &
                         "RiskBrakeUseUnrealizedPnl, RiskBrakeState, RiskBrakePeakEquity, " &
                          "RiskBrakeConsecutiveLosses, RiskBrakePauseUntilUtc, RiskBrakeReason, " &
                          "RiskBrakeBlockedEntries, RiskBrakeEstimatedAvoidedPnl, RiskBrakePauseCount, " &
                          "RiskBrakeLastPauseStartedUtc, RiskBrakeLastPauseEndedUtc, " &
                         "TakeProfitPercent, StopLossPercent, GridV2BtcMildDeclineEnabled, " &
                         "GridV2BtcMildDeclineMinimumPercent, GridV2BtcMildDeclineMaximumPercent, RiskBrakeManualRealizedPnl, " &
                         "GridV2MaximumHoldingMinutes, GridV2RegimeExitEnabled, GridV2ProgressiveExposureEnabled, GridV2ProgressiveExposureReductionPercent, SlippagePercent) " &
                    "VALUES ($startedAt, $status, $enabledStrategies, $combinationMode, $requiredConfirmations, " &
                        "$startingQuote, $allocationPercent, $feeRate, $equity, $quoteBalance, " &
                        "$bollingerPeriod, $bollingerStandardDeviations, $bollingerCandleMinutes, " &
                        "$breakEvenEnabled, $breakEvenTriggerPercent, $breakEvenOffsetPercent, " &
                        "$trailingStopEnabled, $trailingStopDistancePercent, $postExitCooldownMinutes, " &
                        "$newPositionProfitGateEnabled, $minimumNetProfitBeforeNewPositionPercent, " &
                        "$gridV2UseNearestSupportStop, $gridV2SupportBufferAtr, $gridV2MinimumStopPercent, $gridV2MaximumStopPercent, " &
                        "$universeMinimumTrades24h, $gridV2MinimumSupportDistancePercent, $gridV2MaxExposurePercent, " &
                        "$gridV2RequireRejection, $gridV2RiskPerTradePercent, $gridV2ReserveQuotePercent, $gridV2MaxLevels, " &
                        "$bitcoinFlushProtectionEnabled, $bitcoinFlushLookbackMinutes, $bitcoinFlushDropPercent, " &
                        "$bitcoinFlushCandleDropPercent, $bitcoinFlushVolumeMultiplier, $bitcoinFlushVolumeLookback, " &
                        "$bitcoinFlushPauseMinutes, $bitcoinDailyProtectionEnabled, $bitcoinDailyDropPercent, " &
                        "$bitcoinDailyDrawdownPercent, $riskBrakeEnabled, $riskBrakeSessionLossPercent, " &
                        "$riskBrakeProfitLockEnabled, $riskBrakeProfitLockTriggerPercent, $riskBrakeProfitGivebackPercent, " &
                        "$riskBrakeMaxConsecutiveLosses, $riskBrakePauseMinutes, $riskBrakeClosePositionsOnHardStop, " &
                         "$riskBrakeUseUnrealizedPnl, $riskBrakeState, $riskBrakePeakEquity, " &
                          "$riskBrakeConsecutiveLosses, $riskBrakePauseUntilUtc, $riskBrakeReason, " &
                          "$riskBrakeBlockedEntries, $riskBrakeEstimatedAvoidedPnl, $riskBrakePauseCount, " &
                          "$riskBrakeLastPauseStartedUtc, $riskBrakeLastPauseEndedUtc, " &
                         "$takeProfitPercent, $stopLossPercent, $gridV2BtcMildDeclineEnabled, " &
                         "$gridV2BtcMildDeclineMinimumPercent, $gridV2BtcMildDeclineMaximumPercent, $riskBrakeManualRealizedPnl, " &
                         "$gridV2MaximumHoldingMinutes, $gridV2RegimeExitEnabled, $gridV2ProgressiveExposureEnabled, $gridV2ProgressiveExposureReductionPercent, $slippagePercent);"

                command.Parameters.AddWithValue("$startedAt", UtcText(DateTime.UtcNow))
                command.Parameters.AddWithValue("$status", "RUNNING")
                command.Parameters.AddWithValue("$enabledStrategies", String.Join(",", configuration.EnabledStrategies.OrderBy(Function(item) item.ToString())))
                command.Parameters.AddWithValue("$combinationMode", CInt(configuration.CombinationMode))
                command.Parameters.AddWithValue("$requiredConfirmations", configuration.RequiredConfirmations)
                AddRealParameter(command, "$startingQuote", configuration.SimulationStartingQuote)
                AddRealParameter(command, "$allocationPercent", configuration.SimulationAllocationPercent)
                AddRealParameter(command, "$feeRate", configuration.SimulationFeeRate)
                AddRealParameter(command, "$equity", configuration.SimulationStartingQuote)
                AddRealParameter(command, "$quoteBalance", configuration.SimulationStartingQuote)
                command.Parameters.AddWithValue("$bollingerPeriod", Math.Max(2, configuration.BollingerPeriod))
                AddRealParameter(command, "$bollingerStandardDeviations", Math.Max(0.1D, configuration.BollingerStandardDeviations))
                command.Parameters.AddWithValue("$bollingerCandleMinutes", If(configuration.BollingerCandleMinutes = 240, 240, 1440))
                command.Parameters.AddWithValue("$breakEvenEnabled", If(configuration.BreakEvenEnabled, 1, 0))
                AddRealParameter(command, "$breakEvenTriggerPercent", Math.Max(0D, configuration.BreakEvenTriggerPercent))
                AddRealParameter(command, "$breakEvenOffsetPercent", Math.Max(0D, configuration.BreakEvenOffsetPercent))
                command.Parameters.AddWithValue("$trailingStopEnabled", If(configuration.TrailingStopEnabled, 1, 0))
                AddRealParameter(command, "$trailingStopDistancePercent", Math.Max(0.01D, configuration.TrailingStopDistancePercent))
                AddRealParameter(command, "$postExitCooldownMinutes", Math.Max(0D, configuration.PostExitCooldownMinutes))
                command.Parameters.AddWithValue("$newPositionProfitGateEnabled", If(configuration.NewPositionProfitGateEnabled, 1, 0))
                AddRealParameter(command, "$minimumNetProfitBeforeNewPositionPercent", Math.Max(0D, configuration.MinimumNetProfitBeforeNewPositionPercent))
                command.Parameters.AddWithValue("$gridV2UseNearestSupportStop", If(configuration.GridV2UseNearestSupportStop, 1, 0))
                AddRealParameter(command, "$gridV2SupportBufferAtr", Math.Max(0D, configuration.GridV2SupportBufferAtr))
                AddRealParameter(command, "$gridV2MinimumStopPercent", Math.Max(1.5D, configuration.GridV2MinimumStopPercent))
                AddRealParameter(command, "$gridV2MaximumStopPercent", Math.Max(1.5D, Math.Max(configuration.GridV2MinimumStopPercent, configuration.GridV2MaximumStopPercent)))
                command.Parameters.AddWithValue("$universeMinimumTrades24h", Math.Max(0, configuration.UniverseMinimumTrades24h))
                AddRealParameter(command, "$gridV2MinimumSupportDistancePercent", Math.Max(0.8D, configuration.GridV2MinimumSupportDistancePercent))
                AddRealParameter(command, "$gridV2MaxExposurePercent", Math.Min(100D, Math.Max(0D, configuration.GridV2MaxExposurePercent)))
                command.Parameters.AddWithValue("$gridV2RequireRejection", If(configuration.GridV2RequireRejection, 1, 0))
                AddRealParameter(command, "$gridV2RiskPerTradePercent", Math.Min(100D, Math.Max(0D, configuration.GridV2RiskPerTradePercent)))
                AddRealParameter(command, "$gridV2ReserveQuotePercent", Math.Min(95D, Math.Max(0D, configuration.GridV2ReserveQuotePercent)))
                command.Parameters.AddWithValue("$gridV2MaxLevels", Math.Max(1, configuration.GridV2MaxLevels))
                command.Parameters.AddWithValue("$bitcoinFlushProtectionEnabled", If(configuration.BitcoinFlushProtectionEnabled, 1, 0))
                command.Parameters.AddWithValue("$bitcoinFlushLookbackMinutes", Math.Max(2, configuration.BitcoinFlushLookbackMinutes))
                AddRealParameter(command, "$bitcoinFlushDropPercent", Math.Max(0.1D, configuration.BitcoinFlushDropPercent))
                AddRealParameter(command, "$bitcoinFlushCandleDropPercent", Math.Max(0.1D, configuration.BitcoinFlushCandleDropPercent))
                AddRealParameter(command, "$bitcoinFlushVolumeMultiplier", Math.Max(1.1D, configuration.BitcoinFlushVolumeMultiplier))
                command.Parameters.AddWithValue("$bitcoinFlushVolumeLookback", Math.Max(5, configuration.BitcoinFlushVolumeLookback))
                AddRealParameter(command, "$bitcoinFlushPauseMinutes", Math.Max(1D, configuration.BitcoinFlushPauseMinutes))
                command.Parameters.AddWithValue("$bitcoinDailyProtectionEnabled", If(configuration.BitcoinDailyProtectionEnabled, 1, 0))
                AddRealParameter(command, "$bitcoinDailyDropPercent", Math.Max(0.1D, configuration.BitcoinDailyDropPercent))
                AddRealParameter(command, "$bitcoinDailyDrawdownPercent", Math.Max(0.1D, configuration.BitcoinDailyDrawdownPercent))
                command.Parameters.AddWithValue("$riskBrakeEnabled", If(configuration.RiskBrakeEnabled, 1, 0))
                AddRealParameter(command, "$riskBrakeSessionLossPercent", Math.Max(0D, configuration.RiskBrakeSessionLossPercent))
                command.Parameters.AddWithValue("$riskBrakeProfitLockEnabled", If(configuration.RiskBrakeProfitLockEnabled, 1, 0))
                AddRealParameter(command, "$riskBrakeProfitLockTriggerPercent", Math.Max(0D, configuration.RiskBrakeProfitLockTriggerPercent))
                AddRealParameter(command, "$riskBrakeProfitGivebackPercent", Math.Min(100D, Math.Max(0D, configuration.RiskBrakeProfitGivebackPercent)))
                command.Parameters.AddWithValue("$riskBrakeMaxConsecutiveLosses", Math.Max(0, configuration.RiskBrakeMaxConsecutiveLosses))
                AddRealParameter(command, "$riskBrakePauseMinutes", Math.Max(1D, configuration.RiskBrakePauseMinutes))
                command.Parameters.AddWithValue("$riskBrakeClosePositionsOnHardStop", If(configuration.RiskBrakeClosePositionsOnHardStop, 1, 0))
                command.Parameters.AddWithValue("$riskBrakeUseUnrealizedPnl", If(configuration.RiskBrakeUseUnrealizedPnl, 1, 0))
                command.Parameters.AddWithValue("$riskBrakeState", CInt(SpotBotRiskBrakeState.Normal))
                AddRealParameter(command, "$riskBrakePeakEquity", configuration.SimulationStartingQuote)
                command.Parameters.AddWithValue("$riskBrakeConsecutiveLosses", 0)
                command.Parameters.AddWithValue("$riskBrakePauseUntilUtc", DBNull.Value)
                command.Parameters.AddWithValue("$riskBrakeReason", String.Empty)
                command.Parameters.AddWithValue("$riskBrakeBlockedEntries", 0)
                AddRealParameter(command, "$riskBrakeEstimatedAvoidedPnl", 0D)
                command.Parameters.AddWithValue("$riskBrakePauseCount", 0)
                command.Parameters.AddWithValue("$riskBrakeLastPauseStartedUtc", DBNull.Value)
                command.Parameters.AddWithValue("$riskBrakeLastPauseEndedUtc", DBNull.Value)
                AddRealParameter(command, "$takeProfitPercent", Math.Max(0D, configuration.TakeProfitPercent))
                AddRealParameter(command, "$stopLossPercent", Math.Max(0D, configuration.StopLossPercent))
                command.Parameters.AddWithValue("$gridV2BtcMildDeclineEnabled", If(configuration.GridV2BtcMildDeclineEnabled, 1, 0))
                AddRealParameter(command, "$gridV2BtcMildDeclineMinimumPercent", Math.Max(0.1D, configuration.GridV2BtcMildDeclineMinimumPercent))
                AddRealParameter(command, "$gridV2BtcMildDeclineMaximumPercent", Math.Max(Math.Max(0.1D, configuration.GridV2BtcMildDeclineMinimumPercent), configuration.GridV2BtcMildDeclineMaximumPercent))
                AddRealParameter(command, "$riskBrakeManualRealizedPnl", 0D)
                command.Parameters.AddWithValue("$gridV2MaximumHoldingMinutes", Math.Max(0, configuration.GridV2MaximumHoldingMinutes))
                command.Parameters.AddWithValue("$gridV2RegimeExitEnabled", If(configuration.GridV2RegimeExitEnabled, 1, 0))
                command.Parameters.AddWithValue("$gridV2ProgressiveExposureEnabled", If(configuration.GridV2ProgressiveExposureEnabled, 1, 0))
                AddRealParameter(command, "$gridV2ProgressiveExposureReductionPercent", Math.Min(100D, Math.Max(0D, configuration.GridV2ProgressiveExposureReductionPercent)))
                AddRealParameter(command, "$slippagePercent", Math.Min(5D, Math.Max(0D, configuration.SimulationSlippagePercent)))
                command.ExecuteNonQuery()
            End Using

            Using command As SqliteCommand = connection.CreateCommand()
                command.CommandText = "SELECT last_insert_rowid();"
                Return Convert.ToInt64(command.ExecuteScalar(), CultureInfo.InvariantCulture)
            End Using
        End Using
    End Function

    Public Shared Sub UpdateGridConfiguration(runId As Long, configuration As SpotBotConfiguration)
        If runId <= 0 OrElse configuration Is Nothing Then Return
        Initialize()

        Using connection As New SqliteConnection(GetConnectionString())
            connection.Open()
            Using command As SqliteCommand = connection.CreateCommand()
                command.CommandText =
                    "UPDATE SpotBotRuns SET AllocationPercent = $allocationPercent, " &
                    "GridV2MaxExposurePercent = $gridV2MaxExposurePercent, " &
                    "GridV2RequireRejection = $gridV2RequireRejection, " &
                    "GridV2RiskPerTradePercent = $gridV2RiskPerTradePercent, " &
                    "GridV2ReserveQuotePercent = $gridV2ReserveQuotePercent, " &
                    "GridV2MaxLevels = $gridV2MaxLevels, " &
                    "BitcoinFlushProtectionEnabled = $bitcoinFlushProtectionEnabled, " &
                    "BitcoinFlushLookbackMinutes = $bitcoinFlushLookbackMinutes, " &
                    "BitcoinFlushDropPercent = $bitcoinFlushDropPercent, " &
                    "BitcoinFlushCandleDropPercent = $bitcoinFlushCandleDropPercent, " &
                    "BitcoinFlushVolumeMultiplier = $bitcoinFlushVolumeMultiplier, " &
                    "BitcoinFlushVolumeLookback = $bitcoinFlushVolumeLookback, " &
                    "BitcoinFlushPauseMinutes = $bitcoinFlushPauseMinutes, " &
                    "BitcoinDailyProtectionEnabled = $bitcoinDailyProtectionEnabled, " &
                    "BitcoinDailyDropPercent = $bitcoinDailyDropPercent, " &
                    "BitcoinDailyDrawdownPercent = $bitcoinDailyDrawdownPercent, " &
                    "RiskBrakeEnabled = $riskBrakeEnabled, " &
                    "RiskBrakeSessionLossPercent = $riskBrakeSessionLossPercent, " &
                    "RiskBrakeProfitLockEnabled = $riskBrakeProfitLockEnabled, " &
                    "RiskBrakeProfitLockTriggerPercent = $riskBrakeProfitLockTriggerPercent, " &
                    "RiskBrakeProfitGivebackPercent = $riskBrakeProfitGivebackPercent, " &
                    "RiskBrakeMaxConsecutiveLosses = $riskBrakeMaxConsecutiveLosses, " &
                    "RiskBrakePauseMinutes = $riskBrakePauseMinutes, " &
                    "RiskBrakeClosePositionsOnHardStop = $riskBrakeClosePositionsOnHardStop, " &
                    "RiskBrakeUseUnrealizedPnl = $riskBrakeUseUnrealizedPnl, " &
                    "NewPositionProfitGateEnabled = $newPositionProfitGateEnabled, " &
                    "MinimumNetProfitBeforeNewPositionPercent = $minimumNetProfitBeforeNewPositionPercent, " &
                     "TakeProfitPercent = $takeProfitPercent, " &
                     "StopLossPercent = $stopLossPercent, " &
                     "GridV2BtcMildDeclineEnabled = $gridV2BtcMildDeclineEnabled, " &
                     "GridV2BtcMildDeclineMinimumPercent = $gridV2BtcMildDeclineMinimumPercent, " &
                     "GridV2BtcMildDeclineMaximumPercent = $gridV2BtcMildDeclineMaximumPercent, " &
                     "GridV2MaximumHoldingMinutes = $gridV2MaximumHoldingMinutes, " &
                     "GridV2RegimeExitEnabled = $gridV2RegimeExitEnabled, " &
                     "GridV2ProgressiveExposureEnabled = $gridV2ProgressiveExposureEnabled, " &
                     "GridV2ProgressiveExposureReductionPercent = $gridV2ProgressiveExposureReductionPercent, " &
                     "SlippagePercent = $slippagePercent, " &
                    "UpdatedAt = CURRENT_TIMESTAMP " &
                    "WHERE Id = $runId;"
                AddRealParameter(command, "$allocationPercent", Math.Min(100D, Math.Max(0D, configuration.SimulationAllocationPercent)))
                AddRealParameter(command, "$gridV2MaxExposurePercent", Math.Min(100D, Math.Max(0D, configuration.GridV2MaxExposurePercent)))
                command.Parameters.AddWithValue("$gridV2RequireRejection", If(configuration.GridV2RequireRejection, 1, 0))
                AddRealParameter(command, "$gridV2RiskPerTradePercent", Math.Min(100D, Math.Max(0D, configuration.GridV2RiskPerTradePercent)))
                AddRealParameter(command, "$gridV2ReserveQuotePercent", Math.Min(95D, Math.Max(0D, configuration.GridV2ReserveQuotePercent)))
                command.Parameters.AddWithValue("$gridV2MaxLevels", Math.Max(1, configuration.GridV2MaxLevels))
                command.Parameters.AddWithValue("$bitcoinFlushProtectionEnabled", If(configuration.BitcoinFlushProtectionEnabled, 1, 0))
                command.Parameters.AddWithValue("$bitcoinFlushLookbackMinutes", Math.Max(2, configuration.BitcoinFlushLookbackMinutes))
                AddRealParameter(command, "$bitcoinFlushDropPercent", Math.Max(0.1D, configuration.BitcoinFlushDropPercent))
                AddRealParameter(command, "$bitcoinFlushCandleDropPercent", Math.Max(0.1D, configuration.BitcoinFlushCandleDropPercent))
                AddRealParameter(command, "$bitcoinFlushVolumeMultiplier", Math.Max(1.1D, configuration.BitcoinFlushVolumeMultiplier))
                command.Parameters.AddWithValue("$bitcoinFlushVolumeLookback", Math.Max(5, configuration.BitcoinFlushVolumeLookback))
                AddRealParameter(command, "$bitcoinFlushPauseMinutes", Math.Max(1D, configuration.BitcoinFlushPauseMinutes))
                command.Parameters.AddWithValue("$bitcoinDailyProtectionEnabled", If(configuration.BitcoinDailyProtectionEnabled, 1, 0))
                AddRealParameter(command, "$bitcoinDailyDropPercent", Math.Max(0.1D, configuration.BitcoinDailyDropPercent))
                AddRealParameter(command, "$bitcoinDailyDrawdownPercent", Math.Max(0.1D, configuration.BitcoinDailyDrawdownPercent))
                command.Parameters.AddWithValue("$riskBrakeEnabled", If(configuration.RiskBrakeEnabled, 1, 0))
                AddRealParameter(command, "$riskBrakeSessionLossPercent", Math.Max(0D, configuration.RiskBrakeSessionLossPercent))
                command.Parameters.AddWithValue("$riskBrakeProfitLockEnabled", If(configuration.RiskBrakeProfitLockEnabled, 1, 0))
                AddRealParameter(command, "$riskBrakeProfitLockTriggerPercent", Math.Max(0D, configuration.RiskBrakeProfitLockTriggerPercent))
                AddRealParameter(command, "$riskBrakeProfitGivebackPercent", Math.Min(100D, Math.Max(0D, configuration.RiskBrakeProfitGivebackPercent)))
                command.Parameters.AddWithValue("$riskBrakeMaxConsecutiveLosses", Math.Max(0, configuration.RiskBrakeMaxConsecutiveLosses))
                AddRealParameter(command, "$riskBrakePauseMinutes", Math.Max(1D, configuration.RiskBrakePauseMinutes))
                command.Parameters.AddWithValue("$riskBrakeClosePositionsOnHardStop", If(configuration.RiskBrakeClosePositionsOnHardStop, 1, 0))
                command.Parameters.AddWithValue("$riskBrakeUseUnrealizedPnl", If(configuration.RiskBrakeUseUnrealizedPnl, 1, 0))
                command.Parameters.AddWithValue("$newPositionProfitGateEnabled", If(configuration.NewPositionProfitGateEnabled, 1, 0))
                AddRealParameter(command, "$minimumNetProfitBeforeNewPositionPercent", Math.Max(0D, configuration.MinimumNetProfitBeforeNewPositionPercent))
                AddRealParameter(command, "$takeProfitPercent", Math.Max(0D, configuration.TakeProfitPercent))
                AddRealParameter(command, "$stopLossPercent", Math.Max(0D, configuration.StopLossPercent))
                command.Parameters.AddWithValue("$gridV2BtcMildDeclineEnabled", If(configuration.GridV2BtcMildDeclineEnabled, 1, 0))
                AddRealParameter(command, "$gridV2BtcMildDeclineMinimumPercent", Math.Max(0.1D, configuration.GridV2BtcMildDeclineMinimumPercent))
                AddRealParameter(command, "$gridV2BtcMildDeclineMaximumPercent", Math.Max(Math.Max(0.1D, configuration.GridV2BtcMildDeclineMinimumPercent), configuration.GridV2BtcMildDeclineMaximumPercent))
                command.Parameters.AddWithValue("$gridV2MaximumHoldingMinutes", Math.Max(0, configuration.GridV2MaximumHoldingMinutes))
                command.Parameters.AddWithValue("$gridV2RegimeExitEnabled", If(configuration.GridV2RegimeExitEnabled, 1, 0))
                command.Parameters.AddWithValue("$gridV2ProgressiveExposureEnabled", If(configuration.GridV2ProgressiveExposureEnabled, 1, 0))
                AddRealParameter(command, "$gridV2ProgressiveExposureReductionPercent", Math.Min(100D, Math.Max(0D, configuration.GridV2ProgressiveExposureReductionPercent)))
                AddRealParameter(command, "$slippagePercent", Math.Min(5D, Math.Max(0D, configuration.SimulationSlippagePercent)))
                command.Parameters.AddWithValue("$runId", runId)
                command.ExecuteNonQuery()
            End Using
        End Using
    End Sub

    Public Shared Sub SaveCandle(
        runId As Long,
        symbol As String,
        candleStartUtc As DateTime,
        closePrice As Decimal)

        If runId <= 0 OrElse String.IsNullOrWhiteSpace(symbol) OrElse closePrice <= 0D Then Return
        Initialize()

        Using connection As New SqliteConnection(GetConnectionString())
            connection.Open()
            Using command As SqliteCommand = connection.CreateCommand()
                command.CommandText =
                    "INSERT INTO SpotBotCandles(RunId, Symbol, CandleStartUtc, ClosePrice) " &
                    "VALUES($runId, $symbol, $candleStartUtc, $closePrice) " &
                    "ON CONFLICT(RunId, Symbol, CandleStartUtc) DO UPDATE SET ClosePrice = excluded.ClosePrice;"
                command.Parameters.AddWithValue("$runId", runId)
                command.Parameters.AddWithValue("$symbol", symbol.Trim().ToUpperInvariant())
                command.Parameters.AddWithValue("$candleStartUtc", UtcText(candleStartUtc))
                AddRealParameter(command, "$closePrice", closePrice)
                command.ExecuteNonQuery()
            End Using
        End Using
    End Sub

    Public Shared Sub SaveTrade(runId As Long, trade As SpotBotSimulationTrade)
        If runId <= 0 OrElse trade Is Nothing Then Return
        Initialize()

        Using connection As New SqliteConnection(GetConnectionString())
            connection.Open()
            Using command As SqliteCommand = connection.CreateCommand()
                command.CommandText =
                    "INSERT INTO SpotBotTrades(" &
                    "RunId, Symbol, Strategy, ExitReason, Quantity, EntryPrice, ExitPrice, CostBasis, GrossProceeds, Fees, Pnl, SupportPrice, InitialStopPrice, ExitStopPrice, OpenedAtUtc, ClosedAtUtc) " &
                    "VALUES($runId, $symbol, $strategy, $exitReason, $quantity, $entryPrice, $exitPrice, $costBasis, $grossProceeds, $fees, $pnl, $supportPrice, $initialStopPrice, $exitStopPrice, $openedAtUtc, $closedAtUtc);"
                command.Parameters.AddWithValue("$runId", runId)
                command.Parameters.AddWithValue("$symbol", If(trade.Symbol, String.Empty).Trim().ToUpperInvariant())
                command.Parameters.AddWithValue("$strategy", If(trade.Strategy, String.Empty))
                command.Parameters.AddWithValue("$exitReason", If(trade.ExitReason, String.Empty))
                AddRealParameter(command, "$quantity", trade.Quantity)
                AddRealParameter(command, "$entryPrice", trade.EntryPrice)
                AddRealParameter(command, "$exitPrice", trade.ExitPrice)
                AddRealParameter(command, "$costBasis", trade.CostBasis)
                AddRealParameter(command, "$grossProceeds", trade.GrossProceeds)
                AddRealParameter(command, "$fees", trade.Fees)
                AddRealParameter(command, "$pnl", trade.Pnl)
                AddRealParameter(command, "$supportPrice", trade.SupportPrice)
                AddRealParameter(command, "$initialStopPrice", trade.InitialStopPrice)
                AddRealParameter(command, "$exitStopPrice", trade.ExitStopPrice)
                command.Parameters.AddWithValue("$openedAtUtc", UtcText(trade.OpenedAtUtc))
                command.Parameters.AddWithValue("$closedAtUtc", UtcText(trade.ClosedAtUtc))
                command.ExecuteNonQuery()
            End Using
        End Using
    End Sub

    Public Shared Sub SaveOpenPositions(runId As Long, positions As IReadOnlyList(Of SpotBotOpenPosition))
        If runId <= 0 Then Return
        Initialize()

        Using connection As New SqliteConnection(GetConnectionString())
            connection.Open()
            Using transaction = connection.BeginTransaction()
                Using deleteCommand As SqliteCommand = connection.CreateCommand()
                    deleteCommand.Transaction = transaction
                    deleteCommand.CommandText = "DELETE FROM SpotBotOpenPositions WHERE RunId = $runId;"
                    deleteCommand.Parameters.AddWithValue("$runId", runId)
                    deleteCommand.ExecuteNonQuery()
                End Using

                If positions IsNot Nothing Then
                    For Each position In positions
                        If position Is Nothing OrElse String.IsNullOrWhiteSpace(position.Symbol) OrElse
                           position.Quantity <= 0D OrElse position.EntryPrice <= 0D Then
                            Continue For
                        End If

                        Using insertCommand As SqliteCommand = connection.CreateCommand()
                            insertCommand.Transaction = transaction
                            insertCommand.CommandText =
                                "INSERT INTO SpotBotOpenPositions(" &
                                "RunId, Symbol, Side, Strategy, Quantity, EntryPrice, CurrentPrice, CostBasis, SupportPrice, InitialStopPrice, StopPrice, TakeProfitPrice, PeakPrice, OpenedAtUtc, GridLevelCount, GridAnchorPrice, LastGridEntryPrice, GridSpacingPercent, GridManaged, GridLevelsJson) " &
                                "VALUES($runId, $symbol, $side, $strategy, $quantity, $entryPrice, $currentPrice, $costBasis, $supportPrice, $initialStopPrice, $stopPrice, $takeProfitPrice, $peakPrice, $openedAtUtc, $gridLevelCount, $gridAnchorPrice, $lastGridEntryPrice, $gridSpacingPercent, $gridManaged, $gridLevelsJson);"
                            insertCommand.Parameters.AddWithValue("$runId", runId)
                            insertCommand.Parameters.AddWithValue("$symbol", position.Symbol.Trim().ToUpperInvariant())
                            insertCommand.Parameters.AddWithValue("$side", If(String.IsNullOrWhiteSpace(position.Side), "LONG", position.Side.Trim().ToUpperInvariant()))
                            insertCommand.Parameters.AddWithValue("$strategy", If(position.Strategy, String.Empty))
                            AddRealParameter(insertCommand, "$quantity", position.Quantity)
                            AddRealParameter(insertCommand, "$entryPrice", position.EntryPrice)
                            AddRealParameter(insertCommand, "$currentPrice", If(position.CurrentPrice > 0D, position.CurrentPrice, position.EntryPrice))
                            AddRealParameter(insertCommand, "$costBasis", position.CostBasis)
                            AddRealParameter(insertCommand, "$supportPrice", position.SupportPrice)
                            AddRealParameter(insertCommand, "$initialStopPrice", position.InitialStopPrice)
                            AddRealParameter(insertCommand, "$stopPrice", position.StopPrice)
                            AddRealParameter(insertCommand, "$takeProfitPrice", position.TakeProfitPrice)
                            AddRealParameter(insertCommand, "$peakPrice", Math.Max(position.HighestPrice, position.CurrentPrice))
                            insertCommand.Parameters.AddWithValue("$openedAtUtc", UtcText(position.OpenedAtUtc))
                            insertCommand.Parameters.AddWithValue("$gridLevelCount", Math.Max(0, position.GridLevelCount))
                            AddRealParameter(insertCommand, "$gridAnchorPrice", Math.Max(0D, position.GridAnchorPrice))
                            AddRealParameter(insertCommand, "$lastGridEntryPrice", Math.Max(0D, position.LastGridEntryPrice))
                            AddRealParameter(insertCommand, "$gridSpacingPercent", Math.Max(0D, position.GridSpacingPercent))
                            insertCommand.Parameters.AddWithValue("$gridManaged", If(position.GridManaged, 1, 0))
                            insertCommand.Parameters.AddWithValue("$gridLevelsJson", SerializeGridLevels(position.GridLevels))
                            insertCommand.ExecuteNonQuery()
                        End Using
                    Next
                End If

                transaction.Commit()
            End Using
        End Using
    End Sub

    Public Shared Sub SaveGridGuards(runId As Long, guards As IReadOnlyList(Of SpotBotGridGuardState))
        If runId <= 0 Then Return
        Initialize()

        Using connection As New SqliteConnection(GetConnectionString())
            connection.Open()
            Using transaction = connection.BeginTransaction()
                Using deleteCommand As SqliteCommand = connection.CreateCommand()
                    deleteCommand.Transaction = transaction
                    deleteCommand.CommandText = "DELETE FROM SpotBotGridGuards WHERE RunId = $runId;"
                    deleteCommand.Parameters.AddWithValue("$runId", runId)
                    deleteCommand.ExecuteNonQuery()
                End Using

                If guards IsNot Nothing Then
                    For Each guard In guards
                        If guard Is Nothing OrElse String.IsNullOrWhiteSpace(guard.Symbol) Then Continue For
                        If guard.ConsecutiveStops <= 0 AndAlso
                           guard.RecoveryEntryPrice <= 0D AndAlso
                           guard.BlockedUntilUtc = DateTime.MinValue Then Continue For

                        Using insertCommand As SqliteCommand = connection.CreateCommand()
                            insertCommand.Transaction = transaction
                            insertCommand.CommandText =
                                "INSERT INTO SpotBotGridGuards(RunId, Symbol, ConsecutiveStops, RecoveryEntryPrice, BlockedUntilUtc) " &
                                "VALUES($runId, $symbol, $consecutiveStops, $recoveryEntryPrice, $blockedUntilUtc);"
                            insertCommand.Parameters.AddWithValue("$runId", runId)
                            insertCommand.Parameters.AddWithValue("$symbol", guard.Symbol.Trim().ToUpperInvariant())
                            insertCommand.Parameters.AddWithValue("$consecutiveStops", Math.Max(0, guard.ConsecutiveStops))
                            AddRealParameter(insertCommand, "$recoveryEntryPrice", Math.Max(0D, guard.RecoveryEntryPrice))
                            insertCommand.Parameters.AddWithValue(
                                "$blockedUntilUtc",
                                If(guard.BlockedUntilUtc = DateTime.MinValue,
                                   CType(DBNull.Value, Object),
                                   UtcText(guard.BlockedUntilUtc)))
                            insertCommand.ExecuteNonQuery()
                        End Using
                    Next
                End If

                transaction.Commit()
            End Using
        End Using
    End Sub

    Public Shared Function LoadGridGuards(runId As Long) As List(Of SpotBotGridGuardState)
        Dim guards As New List(Of SpotBotGridGuardState)()
        If runId <= 0 Then Return guards
        Initialize()

        Using connection As New SqliteConnection(GetConnectionString())
            connection.Open()
            Using command As SqliteCommand = connection.CreateCommand()
                command.CommandText =
                    "SELECT Symbol, ConsecutiveStops, RecoveryEntryPrice, BlockedUntilUtc " &
                    "FROM SpotBotGridGuards WHERE RunId = $runId ORDER BY Symbol;"
                command.Parameters.AddWithValue("$runId", runId)

                Using reader = command.ExecuteReader()
                    While reader.Read()
                        guards.Add(New SpotBotGridGuardState With {
                            .Symbol = If(reader.IsDBNull(0), String.Empty, reader.GetString(0)),
                            .ConsecutiveStops = If(reader.IsDBNull(1), 0, Math.Max(0, reader.GetInt32(1))),
                            .RecoveryEntryPrice = If(reader.IsDBNull(2), 0D, Math.Max(0D, ReadDecimal(reader, 2))),
                            .BlockedUntilUtc = If(reader.IsDBNull(3), DateTime.MinValue, ParseUtc(reader.GetString(3)))
                        })
                    End While
                End Using
            End Using
        End Using

        Return guards
    End Function

    Public Shared Sub UpdateRun(
        runId As Long,
        snapshot As SpotBotSimulationSnapshot,
        status As String,
        Optional endedAtUtc As DateTime = Nothing)

        If runId <= 0 OrElse snapshot Is Nothing Then Return
        Initialize()

        Using connection As New SqliteConnection(GetConnectionString())
            connection.Open()
            Using command As SqliteCommand = connection.CreateCommand()
                command.CommandText =
                    "UPDATE SpotBotRuns SET " &
                    "EndedAt = CASE WHEN $endedAt IS NULL THEN EndedAt ELSE $endedAt END, " &
                    "Status = $status, CompletedTrades = $completedTrades, WinningTrades = $winningTrades, " &
                    "LosingTrades = $losingTrades, RealizedPnl = $realizedPnl, UnrealizedPnl = $unrealizedPnl, " &
                    "Equity = $equity, TotalPnl = $totalPnl, QuoteBalance = $quoteBalance, " &
                    "OpenValue = $openValue, OpenPositions = $openPositions, LastAction = $lastAction, " &
                    "RiskBrakeState = $riskBrakeState, RiskBrakePeakEquity = $riskBrakePeakEquity, " &
                    "RiskBrakeConsecutiveLosses = $riskBrakeConsecutiveLosses, RiskBrakePauseUntilUtc = $riskBrakePauseUntilUtc, " &
                     "RiskBrakeReason = $riskBrakeReason, RiskBrakeBlockedEntries = $riskBrakeBlockedEntries, " &
                     "RiskBrakeEstimatedAvoidedPnl = $riskBrakeEstimatedAvoidedPnl, RiskBrakePauseCount = $riskBrakePauseCount, " &
                     "RiskBrakeLastPauseStartedUtc = $riskBrakeLastPauseStartedUtc, RiskBrakeLastPauseEndedUtc = $riskBrakeLastPauseEndedUtc, " &
                    "RiskBrakeManualRealizedPnl = $riskBrakeManualRealizedPnl, " &
                    "UpdatedAt = CURRENT_TIMESTAMP WHERE Id = $runId;"
                command.Parameters.AddWithValue("$endedAt", If(endedAtUtc = DateTime.MinValue, CType(DBNull.Value, Object), UtcText(endedAtUtc)))
                command.Parameters.AddWithValue("$status", If(status, String.Empty))
                command.Parameters.AddWithValue("$completedTrades", snapshot.CompletedTrades)
                command.Parameters.AddWithValue("$winningTrades", snapshot.WinningTrades)
                command.Parameters.AddWithValue("$losingTrades", snapshot.LosingTrades)
                AddRealParameter(command, "$realizedPnl", snapshot.RealizedPnl)
                AddRealParameter(command, "$unrealizedPnl", snapshot.UnrealizedPnl)
                AddRealParameter(command, "$equity", snapshot.Equity)
                AddRealParameter(command, "$totalPnl", snapshot.TotalPnl)
                AddRealParameter(command, "$quoteBalance", snapshot.QuoteBalance)
                AddRealParameter(command, "$openValue", snapshot.OpenValue)
                command.Parameters.AddWithValue("$openPositions", snapshot.OpenPositions)
                command.Parameters.AddWithValue("$lastAction", If(snapshot.LastAction, String.Empty))
                command.Parameters.AddWithValue("$riskBrakeState", CInt(snapshot.RiskBrakeState))
                AddRealParameter(command, "$riskBrakePeakEquity", Math.Max(0D, snapshot.RiskBrakePeakEquity))
                command.Parameters.AddWithValue("$riskBrakeConsecutiveLosses", Math.Max(0, snapshot.RiskBrakeConsecutiveLosses))
                command.Parameters.AddWithValue("$riskBrakePauseUntilUtc", If(snapshot.RiskBrakePauseUntilUtc = DateTime.MinValue, CType(DBNull.Value, Object), UtcText(snapshot.RiskBrakePauseUntilUtc)))
                command.Parameters.AddWithValue("$riskBrakeReason", If(snapshot.RiskBrakeReason, String.Empty))
                command.Parameters.AddWithValue("$riskBrakeBlockedEntries", Math.Max(0, snapshot.RiskBrakeBlockedEntries))
                AddRealParameter(command, "$riskBrakeEstimatedAvoidedPnl", Math.Max(0D, snapshot.RiskBrakeEstimatedAvoidedPnl))
                command.Parameters.AddWithValue("$riskBrakePauseCount", Math.Max(0, snapshot.RiskBrakePauseCount))
                command.Parameters.AddWithValue("$riskBrakeLastPauseStartedUtc", If(snapshot.RiskBrakeLastPauseStartedUtc = DateTime.MinValue, CType(DBNull.Value, Object), UtcText(snapshot.RiskBrakeLastPauseStartedUtc)))
                command.Parameters.AddWithValue("$riskBrakeLastPauseEndedUtc", If(snapshot.RiskBrakeLastPauseEndedUtc = DateTime.MinValue, CType(DBNull.Value, Object), UtcText(snapshot.RiskBrakeLastPauseEndedUtc)))
                AddRealParameter(command, "$riskBrakeManualRealizedPnl", snapshot.RiskBrakeManualRealizedPnl)
                command.Parameters.AddWithValue("$runId", runId)
                command.ExecuteNonQuery()
            End Using
        End Using
    End Sub

    Public Shared Function GetRuns() As DataTable
        Initialize()
        Dim table As New DataTable()

        Using connection As New SqliteConnection(GetConnectionString())
            connection.Open()
            Using command As SqliteCommand = connection.CreateCommand()
                command.CommandText =
                    "SELECT Id, StartedAt, EndedAt, Status, EnabledStrategies, CombinationMode, " &
                    "RequiredConfirmations, StartingQuote, AllocationPercent, FeeRate, CompletedTrades, " &
                    "WinningTrades, LosingTrades, RealizedPnl, UnrealizedPnl, Equity, TotalPnl, " &
                        "QuoteBalance, OpenValue, OpenPositions, LastAction, " &
                        "BollingerPeriod, BollingerStandardDeviations, BollingerCandleMinutes, " &
                        "PostExitCooldownMinutes, UpdatedAt " &
                    "FROM SpotBotRuns ORDER BY Id DESC;"
                Using reader = command.ExecuteReader()
                    table.Load(reader)
                End Using
            End Using
        End Using

        Return table
    End Function

    Public Shared Function GetTrades(runId As Long) As DataTable
        Initialize()
        Dim table As New DataTable()

        Using connection As New SqliteConnection(GetConnectionString())
            connection.Open()
            Using command As SqliteCommand = connection.CreateCommand()
                command.CommandText =
                    "SELECT Id, RunId, Symbol, Strategy, ExitReason, Quantity, EntryPrice, ExitPrice, CostBasis, " &
                    "GrossProceeds, Fees, Pnl, SupportPrice, InitialStopPrice, ExitStopPrice, OpenedAtUtc, ClosedAtUtc " &
                    "FROM SpotBotTrades WHERE RunId = $runId ORDER BY ClosedAtUtc, Id;"
                command.Parameters.AddWithValue("$runId", runId)
                Using reader = command.ExecuteReader()
                    table.Load(reader)
                End Using
            End Using
        End Using

        Return table
    End Function

    Public Shared Function GetOpenPositions(runId As Long) As DataTable
        Initialize()
        Dim table As New DataTable()

        Using connection As New SqliteConnection(GetConnectionString())
            connection.Open()
            Using command As SqliteCommand = connection.CreateCommand()
                command.CommandText =
                    "SELECT Symbol, Side, Strategy, Quantity, EntryPrice, CurrentPrice, CostBasis, SupportPrice, InitialStopPrice, " &
                    "StopPrice, TakeProfitPrice, PeakPrice, OpenedAtUtc, GridLevelCount, GridAnchorPrice, " &
                    "LastGridEntryPrice, GridSpacingPercent, GridManaged, GridLevelsJson " &
                    "FROM SpotBotOpenPositions WHERE RunId = $runId ORDER BY Symbol;"
                command.Parameters.AddWithValue("$runId", runId)
                Using reader = command.ExecuteReader()
                    table.Load(reader)
                End Using
            End Using
        End Using

        Return table
    End Function

    Public Shared Function LoadLatestRestorableState() As SpotBotPersistedSimulationState
        Initialize()

        Using connection As New SqliteConnection(GetConnectionString())
            connection.Open()

            Dim state As SpotBotPersistedSimulationState = Nothing
            Using command As SqliteCommand = connection.CreateCommand()
                command.CommandText =
                    "SELECT Id, Status, EnabledStrategies, CombinationMode, RequiredConfirmations, " &
                    "StartingQuote, AllocationPercent, FeeRate, QuoteBalance, OpenValue, RealizedPnl, " &
                    "UnrealizedPnl, Equity, TotalPnl, CompletedTrades, WinningTrades, LosingTrades, " &
                        "OpenPositions, LastAction, BollingerPeriod, BollingerStandardDeviations, BollingerCandleMinutes, " &
                        "BreakEvenEnabled, BreakEvenTriggerPercent, BreakEvenOffsetPercent, TrailingStopEnabled, " &
                        "TrailingStopDistancePercent, PostExitCooldownMinutes, GridV2UseNearestSupportStop, " &
                        "GridV2SupportBufferAtr, GridV2MinimumStopPercent, GridV2MaximumStopPercent, " &
                        "UniverseMinimumTrades24h, GridV2MinimumSupportDistancePercent, GridV2MaxExposurePercent, " &
                        "GridV2RequireRejection, GridV2RiskPerTradePercent, GridV2ReserveQuotePercent, GridV2MaxLevels, " &
                        "BitcoinFlushProtectionEnabled, BitcoinFlushLookbackMinutes, BitcoinFlushDropPercent, " &
                        "BitcoinFlushCandleDropPercent, BitcoinFlushVolumeMultiplier, BitcoinFlushVolumeLookback, " &
                        "BitcoinFlushPauseMinutes, BitcoinDailyProtectionEnabled, BitcoinDailyDropPercent, " &
                        "BitcoinDailyDrawdownPercent, RiskBrakeEnabled, RiskBrakeSessionLossPercent, " &
                        "RiskBrakeProfitLockEnabled, RiskBrakeProfitLockTriggerPercent, RiskBrakeProfitGivebackPercent, " &
                        "RiskBrakeMaxConsecutiveLosses, RiskBrakePauseMinutes, RiskBrakeClosePositionsOnHardStop, " &
                         "RiskBrakeUseUnrealizedPnl, RiskBrakeState, RiskBrakePeakEquity, " &
                          "RiskBrakeConsecutiveLosses, RiskBrakePauseUntilUtc, RiskBrakeReason, " &
                          "RiskBrakeBlockedEntries, RiskBrakeEstimatedAvoidedPnl, RiskBrakePauseCount, " &
                          "RiskBrakeLastPauseStartedUtc, RiskBrakeLastPauseEndedUtc, " &
                         "TakeProfitPercent, StopLossPercent, GridV2BtcMildDeclineEnabled, " &
                         "GridV2BtcMildDeclineMinimumPercent, GridV2BtcMildDeclineMaximumPercent, RiskBrakeManualRealizedPnl, " &
                         "GridV2MaximumHoldingMinutes, GridV2RegimeExitEnabled, GridV2ProgressiveExposureEnabled, GridV2ProgressiveExposureReductionPercent, SlippagePercent, " &
                         "NewPositionProfitGateEnabled, MinimumNetProfitBeforeNewPositionPercent " &
                    "FROM SpotBotRuns ORDER BY Id DESC LIMIT 1;"

                Using reader = command.ExecuteReader()
                    If Not reader.Read() Then Return Nothing

                    state = New SpotBotPersistedSimulationState With {
                        .RunId = reader.GetInt64(0),
                        .Status = If(reader.IsDBNull(1), String.Empty, reader.GetString(1)),
                        .CombinationMode = CType(reader.GetInt32(3), SpotBotCombinationMode),
                        .RequiredConfirmations = reader.GetInt32(4),
                        .AllocationPercent = ReadDecimal(reader, 6),
                        .FeeRate = ReadDecimal(reader, 7),
                        .Snapshot = New SpotBotSimulationSnapshot With {
                            .StartingQuote = ReadDecimal(reader, 5),
                            .QuoteBalance = ReadDecimal(reader, 8),
                            .OpenValue = ReadDecimal(reader, 9),
                            .RealizedPnl = ReadDecimal(reader, 10),
                            .UnrealizedPnl = ReadDecimal(reader, 11),
                            .Equity = ReadDecimal(reader, 12),
                            .TotalPnl = ReadDecimal(reader, 13),
                            .CompletedTrades = reader.GetInt32(14),
                            .WinningTrades = reader.GetInt32(15),
                            .LosingTrades = reader.GetInt32(16),
                            .OpenPositions = reader.GetInt32(17),
                            .LastAction = If(reader.IsDBNull(18), String.Empty, reader.GetString(18))
                        },
                        .BollingerPeriod = If(reader.IsDBNull(19), 20, Math.Max(2, reader.GetInt32(19))),
                        .BollingerStandardDeviations = If(reader.IsDBNull(20), 3D, Math.Max(0.1D, ReadDecimal(reader, 20))),
                         .BollingerCandleMinutes = If(reader.IsDBNull(21), 1440, If(reader.GetInt32(21) = 240, 240, 1440)),
                         .BreakEvenEnabled = If(reader.IsDBNull(22), True, reader.GetInt32(22) <> 0),
                         .BreakEvenTriggerPercent = If(reader.IsDBNull(23), 1.5D, Math.Max(0D, ReadDecimal(reader, 23))),
                         .BreakEvenOffsetPercent = If(reader.IsDBNull(24), 0.1D, Math.Max(0D, ReadDecimal(reader, 24))),
                         .TrailingStopEnabled = If(reader.IsDBNull(25), False, reader.GetInt32(25) <> 0),
                         .TrailingStopDistancePercent = If(reader.IsDBNull(26), 1D, Math.Max(0.01D, ReadDecimal(reader, 26))),
                          .PostExitCooldownMinutes = If(reader.IsDBNull(27), 5D, Math.Max(0D, ReadDecimal(reader, 27))),
                          .GridV2UseNearestSupportStop = If(reader.IsDBNull(28), True, reader.GetInt32(28) <> 0),
                          .GridV2SupportBufferAtr = If(reader.IsDBNull(29), 1D, Math.Max(0D, ReadDecimal(reader, 29))),
                          .GridV2MinimumStopPercent = If(reader.IsDBNull(30), 1.5D, Math.Max(1.5D, ReadDecimal(reader, 30))),
                          .GridV2MaximumStopPercent = If(reader.IsDBNull(31), 3D, Math.Max(0D, ReadDecimal(reader, 31))),
                          .UniverseMinimumTrades24h = If(reader.IsDBNull(32), 1000, Math.Max(0, reader.GetInt32(32))),
                           .GridV2MinimumSupportDistancePercent = If(reader.IsDBNull(33), 0.8D, Math.Max(0.8D, ReadDecimal(reader, 33))),
                          .GridV2MaxExposurePercent = If(reader.IsDBNull(34), 30D, Math.Min(100D, Math.Max(0D, ReadDecimal(reader, 34)))),
                          .GridV2RequireRejection = If(reader.IsDBNull(35), True, reader.GetInt32(35) <> 0),
                          .GridV2RiskPerTradePercent = If(reader.IsDBNull(36), 0.5D, Math.Min(100D, Math.Max(0D, ReadDecimal(reader, 36)))),
                          .GridV2ReserveQuotePercent = If(reader.IsDBNull(37), 20D, Math.Min(95D, Math.Max(0D, ReadDecimal(reader, 37)))),
                          .GridV2MaxLevels = If(reader.IsDBNull(38), 5, Math.Max(1, reader.GetInt32(38))),
                          .BitcoinFlushProtectionEnabled = If(reader.IsDBNull(39), True, reader.GetInt32(39) <> 0),
                          .BitcoinFlushLookbackMinutes = If(reader.IsDBNull(40), 5, Math.Max(2, reader.GetInt32(40))),
                          .BitcoinFlushDropPercent = If(reader.IsDBNull(41), 2D, Math.Max(0.1D, ReadDecimal(reader, 41))),
                          .BitcoinFlushCandleDropPercent = If(reader.IsDBNull(42), 1.5D, Math.Max(0.1D, ReadDecimal(reader, 42))),
                          .BitcoinFlushVolumeMultiplier = If(reader.IsDBNull(43), 1.8D, Math.Max(1.1D, ReadDecimal(reader, 43))),
                          .BitcoinFlushVolumeLookback = If(reader.IsDBNull(44), 20, Math.Max(5, reader.GetInt32(44))),
                          .BitcoinFlushPauseMinutes = If(reader.IsDBNull(45), 30D, Math.Max(1D, ReadDecimal(reader, 45))),
                          .BitcoinDailyProtectionEnabled = If(reader.IsDBNull(46), True, reader.GetInt32(46) <> 0),
                          .BitcoinDailyDropPercent = If(reader.IsDBNull(47), 4D, Math.Max(0.1D, ReadDecimal(reader, 47))),
                          .BitcoinDailyDrawdownPercent = If(reader.IsDBNull(48), 5D, Math.Max(0.1D, ReadDecimal(reader, 48))),
                          .RiskBrakeEnabled = If(reader.IsDBNull(49), True, reader.GetInt32(49) <> 0),
                          .RiskBrakeSessionLossPercent = If(reader.IsDBNull(50), 2D, Math.Max(0D, ReadDecimal(reader, 50))),
                          .RiskBrakeProfitLockEnabled = If(reader.IsDBNull(51), True, reader.GetInt32(51) <> 0),
                          .RiskBrakeProfitLockTriggerPercent = If(reader.IsDBNull(52), 1D, Math.Max(0D, ReadDecimal(reader, 52))),
                          .RiskBrakeProfitGivebackPercent = If(reader.IsDBNull(53), 50D, Math.Min(100D, Math.Max(0D, ReadDecimal(reader, 53)))),
                          .RiskBrakeMaxConsecutiveLosses = If(reader.IsDBNull(54), 3, Math.Max(0, reader.GetInt32(54))),
                          .RiskBrakePauseMinutes = If(reader.IsDBNull(55), 60D, Math.Max(1D, ReadDecimal(reader, 55))),
                          .RiskBrakeClosePositionsOnHardStop = If(reader.IsDBNull(56), True, reader.GetInt32(56) <> 0),
                          .RiskBrakeUseUnrealizedPnl = If(reader.IsDBNull(57), True, reader.GetInt32(57) <> 0)
                    }

                    state.Snapshot.RiskBrakeState = If(reader.IsDBNull(58), SpotBotRiskBrakeState.Normal, CType(reader.GetInt32(58), SpotBotRiskBrakeState))
                    state.Snapshot.RiskBrakePeakEquity = If(reader.IsDBNull(59), 0D, ReadDecimal(reader, 59))
                    state.Snapshot.RiskBrakeConsecutiveLosses = If(reader.IsDBNull(60), 0, Math.Max(0, reader.GetInt32(60)))
                    state.Snapshot.RiskBrakePauseUntilUtc = If(reader.IsDBNull(61), DateTime.MinValue, ParseUtc(reader.GetString(61)))
                    state.Snapshot.RiskBrakeReason = If(reader.IsDBNull(62), String.Empty, reader.GetString(62))
                    state.Snapshot.RiskBrakeBlockedEntries = If(reader.IsDBNull(63), 0, Math.Max(0, reader.GetInt32(63)))
                    state.Snapshot.RiskBrakeEstimatedAvoidedPnl = If(reader.IsDBNull(64), 0D, Math.Max(0D, ReadDecimal(reader, 64)))
                    state.Snapshot.RiskBrakePauseCount = If(reader.IsDBNull(65), 0, Math.Max(0, reader.GetInt32(65)))
                    state.Snapshot.RiskBrakeLastPauseStartedUtc = If(reader.IsDBNull(66), DateTime.MinValue, ParseUtc(reader.GetString(66)))
                    state.Snapshot.RiskBrakeLastPauseEndedUtc = If(reader.IsDBNull(67), DateTime.MinValue, ParseUtc(reader.GetString(67)))
                    state.TakeProfitPercent = If(reader.IsDBNull(68), 2D, Math.Max(0D, ReadDecimal(reader, 68)))
                    state.StopLossPercent = If(reader.IsDBNull(69), 1D, Math.Max(0D, ReadDecimal(reader, 69)))
                    state.GridV2BtcMildDeclineEnabled = If(reader.IsDBNull(70), False, reader.GetInt32(70) <> 0)
                    state.GridV2BtcMildDeclineMinimumPercent = If(reader.IsDBNull(71), 0.5D, Math.Max(0.1D, ReadDecimal(reader, 71)))
                    state.GridV2BtcMildDeclineMaximumPercent = If(reader.IsDBNull(72), 2D, Math.Max(state.GridV2BtcMildDeclineMinimumPercent, ReadDecimal(reader, 72)))
                    state.Snapshot.RiskBrakeManualRealizedPnl = If(reader.IsDBNull(73), 0D, ReadDecimal(reader, 73))
                    state.GridV2MaximumHoldingMinutes = If(reader.IsDBNull(74), 240, Math.Max(0, reader.GetInt32(74)))
                    state.GridV2RegimeExitEnabled = If(reader.IsDBNull(75), True, reader.GetInt32(75) <> 0)
                    state.GridV2ProgressiveExposureEnabled = If(reader.IsDBNull(76), True, reader.GetInt32(76) <> 0)
                    state.GridV2ProgressiveExposureReductionPercent = If(reader.IsDBNull(77), 25D, Math.Min(100D, Math.Max(0D, ReadDecimal(reader, 77))))
                    state.SlippagePercent = If(reader.IsDBNull(78), 0.05D, Math.Min(5D, Math.Max(0D, ReadDecimal(reader, 78))))
                    state.NewPositionProfitGateEnabled = If(reader.IsDBNull(79), True, reader.GetInt32(79) <> 0)
                    state.MinimumNetProfitBeforeNewPositionPercent = If(reader.IsDBNull(80), 0.5D, Math.Max(0D, ReadDecimal(reader, 80)))

                    If Not reader.IsDBNull(2) Then
                        For Each strategyText In reader.GetString(2).Split(","c, StringSplitOptions.RemoveEmptyEntries)
                            Dim strategy As SpotBotStrategyKind
                            If [Enum].TryParse(strategyText.Trim(), True, strategy) Then
                                state.EnabledStrategies.Add(strategy)
                            End If
                        Next
                    End If
                End Using
            End Using

            Using command As SqliteCommand = connection.CreateCommand()
                command.CommandText =
                        "SELECT Symbol, Side, Strategy, Quantity, EntryPrice, CurrentPrice, CostBasis, SupportPrice, InitialStopPrice, StopPrice, TakeProfitPrice, PeakPrice, OpenedAtUtc, " &
                        "GridLevelCount, GridAnchorPrice, LastGridEntryPrice, GridSpacingPercent, GridManaged, GridLevelsJson " &
                    "FROM SpotBotOpenPositions WHERE RunId = $runId ORDER BY Symbol;"
                command.Parameters.AddWithValue("$runId", state.RunId)

                Using reader = command.ExecuteReader()
                    While reader.Read()
                        Dim openedAt = ParseUtc(reader.GetString(12))
                        Dim entryPrice = ReadDecimal(reader, 4)
                        Dim currentPrice = ReadDecimal(reader, 5)
                        Dim quantity = ReadDecimal(reader, 3)
                        Dim costBasis = ReadDecimal(reader, 6)
                        Dim supportPrice = ReadDecimal(reader, 7)
                        Dim initialStopPrice = ReadDecimal(reader, 8)
                        Dim stopPrice = ReadDecimal(reader, 9)
                        Dim takeProfitPrice = ReadDecimal(reader, 10)
                        Dim peakPrice = ReadDecimal(reader, 11)
                        Dim gridLevelCount = If(reader.IsDBNull(13), 0, Math.Max(0, reader.GetInt32(13)))
                        Dim gridAnchorPrice = ReadDecimal(reader, 14)
                        Dim lastGridEntryPrice = ReadDecimal(reader, 15)
                        Dim gridSpacingPercent = ReadDecimal(reader, 16)
                        Dim gridManaged = If(reader.IsDBNull(17), False, reader.GetInt32(17) <> 0)
                        Dim gridLevelsJson = If(reader.IsDBNull(18), "[]", reader.GetString(18))
                        Dim strategyText = If(reader.IsDBNull(2), String.Empty, reader.GetString(2))
                        If Not gridManaged AndAlso strategyText.IndexOf("Grid", StringComparison.OrdinalIgnoreCase) >= 0 Then
                            ' Compatibilidade com posições gravadas antes da
                            ' coluna explícita GridManaged.
                            gridManaged = True
                        End If
                        Dim notional = quantity * currentPrice
                        Dim unrealizedPnl = notional - costBasis

                        state.Positions.Add(New SpotBotOpenPosition With {
                            .Symbol = reader.GetString(0),
                            .Side = If(reader.IsDBNull(1), "LONG", reader.GetString(1)),
                            .Strategy = strategyText,
                            .Quantity = quantity,
                            .EntryPrice = entryPrice,
                            .CurrentPrice = currentPrice,
                            .CostBasis = costBasis,
                            .SupportPrice = supportPrice,
                            .InitialStopPrice = initialStopPrice,
                            .StopPrice = stopPrice,
                            .TakeProfitPrice = takeProfitPrice,
                            .HighestPrice = peakPrice,
                            .Notional = notional,
                            .UnrealizedPnl = unrealizedPnl,
                            .RoiPercent = If(costBasis = 0D, 0D, unrealizedPnl / costBasis * 100D),
                            .GridManaged = gridManaged,
                            .GridLevelCount = gridLevelCount,
                            .GridAnchorPrice = gridAnchorPrice,
                            .LastGridEntryPrice = lastGridEntryPrice,
                            .GridSpacingPercent = gridSpacingPercent,
                            .GridLevels = DeserializeGridLevels(gridLevelsJson),
                            .OpenedAtUtc = openedAt
                        })
                    End While
                End Using
            End Using

            state.GridGuards = LoadGridGuards(state.RunId)

            state.Snapshot.OpenPositions = state.Positions.Count
            Return state
        End Using
    End Function

    Private Shared Function GetConnectionString() As String
        Return $"Data Source={PortfolioRepository.GetDatabasePath()}"
    End Function

    Private Shared Sub EnsureColumn(
        connection As SqliteConnection,
        tableName As String,
        columnName As String,
        columnDefinition As String)

        Using checkCommand As SqliteCommand = connection.CreateCommand()
            checkCommand.CommandText = $"PRAGMA table_info({tableName});"
            Using reader = checkCommand.ExecuteReader()
                While reader.Read()
                    If String.Equals(reader.GetString(1), columnName, StringComparison.OrdinalIgnoreCase) Then
                        Return
                    End If
                End While
            End Using
        End Using

        Using alterCommand As SqliteCommand = connection.CreateCommand()
            alterCommand.CommandText = $"ALTER TABLE {tableName} ADD COLUMN {columnName} {columnDefinition};"
            alterCommand.ExecuteNonQuery()
        End Using
    End Sub

    Private Shared Function UtcText(value As DateTime) As String
        Return value.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture)
    End Function

    Private Shared Sub AddRealParameter(command As SqliteCommand, name As String, value As Decimal)
        Dim parameter = command.Parameters.Add(name, SqliteType.Real)
        parameter.Value = Convert.ToDouble(value, CultureInfo.InvariantCulture)
    End Sub

    Private Shared Function ReadDecimal(reader As SqliteDataReader, ordinal As Integer) As Decimal
        If reader.IsDBNull(ordinal) Then Return 0D
        Return Convert.ToDecimal(reader.GetValue(ordinal), CultureInfo.InvariantCulture)
    End Function

    Private Shared Function SerializeGridLevels(levels As IReadOnlyList(Of SpotBotGridLevel)) As String
        Try
            Return JsonSerializer.Serialize(If(levels, Array.Empty(Of SpotBotGridLevel)()))
        Catch
            Return "[]"
        End Try
    End Function

    Private Shared Function DeserializeGridLevels(json As String) As List(Of SpotBotGridLevel)
        If String.IsNullOrWhiteSpace(json) Then Return New List(Of SpotBotGridLevel)()
        Try
            Return JsonSerializer.Deserialize(Of List(Of SpotBotGridLevel))(json)
        Catch
            Return New List(Of SpotBotGridLevel)()
        End Try
    End Function

    Private Shared Function ParseUtc(value As String) As DateTime
        Dim parsed As DateTime
        If DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, parsed) Then
            Return If(parsed.Kind = DateTimeKind.Unspecified,
                      DateTime.SpecifyKind(parsed, DateTimeKind.Utc),
                      parsed.ToUniversalTime())
        End If

        Return DateTime.UtcNow
    End Function

End Class
