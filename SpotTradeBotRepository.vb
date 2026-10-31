Imports Microsoft.Data.Sqlite
Imports System.Data
Imports System.Globalization
Imports System.Linq

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
                        "TrailingStopEnabled, TrailingStopDistancePercent, PostExitCooldownMinutes) " &
                    "VALUES ($startedAt, $status, $enabledStrategies, $combinationMode, $requiredConfirmations, " &
                        "$startingQuote, $allocationPercent, $feeRate, $equity, $quoteBalance, " &
                        "$bollingerPeriod, $bollingerStandardDeviations, $bollingerCandleMinutes, " &
                        "$breakEvenEnabled, $breakEvenTriggerPercent, $breakEvenOffsetPercent, " &
                        "$trailingStopEnabled, $trailingStopDistancePercent, $postExitCooldownMinutes);"

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
                command.ExecuteNonQuery()
            End Using

            Using command As SqliteCommand = connection.CreateCommand()
                command.CommandText = "SELECT last_insert_rowid();"
                Return Convert.ToInt64(command.ExecuteScalar(), CultureInfo.InvariantCulture)
            End Using
        End Using
    End Function

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
                    "RunId, Symbol, Strategy, ExitReason, Quantity, EntryPrice, ExitPrice, CostBasis, GrossProceeds, Fees, Pnl, OpenedAtUtc, ClosedAtUtc) " &
                    "VALUES($runId, $symbol, $strategy, $exitReason, $quantity, $entryPrice, $exitPrice, $costBasis, $grossProceeds, $fees, $pnl, $openedAtUtc, $closedAtUtc);"
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
                                "RunId, Symbol, Side, Strategy, Quantity, EntryPrice, CurrentPrice, CostBasis, StopPrice, TakeProfitPrice, PeakPrice, OpenedAtUtc) " &
                                "VALUES($runId, $symbol, $side, $strategy, $quantity, $entryPrice, $currentPrice, $costBasis, $stopPrice, $takeProfitPrice, $peakPrice, $openedAtUtc);"
                            insertCommand.Parameters.AddWithValue("$runId", runId)
                            insertCommand.Parameters.AddWithValue("$symbol", position.Symbol.Trim().ToUpperInvariant())
                            insertCommand.Parameters.AddWithValue("$side", If(String.IsNullOrWhiteSpace(position.Side), "LONG", position.Side.Trim().ToUpperInvariant()))
                            insertCommand.Parameters.AddWithValue("$strategy", If(position.Strategy, String.Empty))
                            AddRealParameter(insertCommand, "$quantity", position.Quantity)
                            AddRealParameter(insertCommand, "$entryPrice", position.EntryPrice)
                            AddRealParameter(insertCommand, "$currentPrice", If(position.CurrentPrice > 0D, position.CurrentPrice, position.EntryPrice))
                            AddRealParameter(insertCommand, "$costBasis", position.CostBasis)
                            AddRealParameter(insertCommand, "$stopPrice", position.StopPrice)
                            AddRealParameter(insertCommand, "$takeProfitPrice", position.TakeProfitPrice)
                            AddRealParameter(insertCommand, "$peakPrice", Math.Max(position.HighestPrice, position.CurrentPrice))
                            insertCommand.Parameters.AddWithValue("$openedAtUtc", UtcText(position.OpenedAtUtc))
                            insertCommand.ExecuteNonQuery()
                        End Using
                    Next
                End If

                transaction.Commit()
            End Using
        End Using
    End Sub

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
                    "GrossProceeds, Fees, Pnl, OpenedAtUtc, ClosedAtUtc " &
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
                    "SELECT Symbol, Side, Strategy, Quantity, EntryPrice, CurrentPrice, CostBasis, " &
                    "StopPrice, TakeProfitPrice, PeakPrice, OpenedAtUtc " &
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
                        "TrailingStopDistancePercent, PostExitCooldownMinutes " &
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
                         .PostExitCooldownMinutes = If(reader.IsDBNull(27), 5D, Math.Max(0D, ReadDecimal(reader, 27)))
                    }

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
                        "SELECT Symbol, Side, Strategy, Quantity, EntryPrice, CurrentPrice, CostBasis, StopPrice, TakeProfitPrice, PeakPrice, OpenedAtUtc " &
                    "FROM SpotBotOpenPositions WHERE RunId = $runId ORDER BY Symbol;"
                command.Parameters.AddWithValue("$runId", state.RunId)

                Using reader = command.ExecuteReader()
                    While reader.Read()
                        Dim openedAt = ParseUtc(reader.GetString(10))
                        Dim entryPrice = ReadDecimal(reader, 4)
                        Dim currentPrice = ReadDecimal(reader, 5)
                        Dim quantity = ReadDecimal(reader, 3)
                        Dim costBasis = ReadDecimal(reader, 6)
                        Dim stopPrice = ReadDecimal(reader, 7)
                        Dim takeProfitPrice = ReadDecimal(reader, 8)
                        Dim peakPrice = ReadDecimal(reader, 9)
                        Dim notional = quantity * currentPrice
                        Dim unrealizedPnl = notional - costBasis

                        state.Positions.Add(New SpotBotOpenPosition With {
                            .Symbol = reader.GetString(0),
                            .Side = If(reader.IsDBNull(1), "LONG", reader.GetString(1)),
                            .Strategy = If(reader.IsDBNull(2), String.Empty, reader.GetString(2)),
                            .Quantity = quantity,
                            .EntryPrice = entryPrice,
                            .CurrentPrice = currentPrice,
                            .CostBasis = costBasis,
                            .StopPrice = stopPrice,
                            .TakeProfitPrice = takeProfitPrice,
                            .HighestPrice = peakPrice,
                            .Notional = notional,
                            .UnrealizedPnl = unrealizedPnl,
                            .RoiPercent = If(costBasis = 0D, 0D, unrealizedPnl / costBasis * 100D),
                            .OpenedAtUtc = openedAt
                        })
                    End While
                End Using
            End Using

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
