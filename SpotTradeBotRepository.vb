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
                        "UpdatedAt TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP" &
                        ");"
                    command.ExecuteNonQuery()
                End Using

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
                        "OpenedAtUtc TEXT NOT NULL, " &
                        "ClosedAtUtc TEXT NOT NULL, " &
                        "CreatedAt TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP, " &
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
                    "StartingQuote, AllocationPercent, FeeRate, Equity, QuoteBalance) " &
                    "VALUES ($startedAt, $status, $enabledStrategies, $combinationMode, $requiredConfirmations, " &
                    "$startingQuote, $allocationPercent, $feeRate, $equity, $quoteBalance);"

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
                    "RunId, Symbol, Quantity, EntryPrice, ExitPrice, CostBasis, GrossProceeds, Fees, Pnl, OpenedAtUtc, ClosedAtUtc) " &
                    "VALUES($runId, $symbol, $quantity, $entryPrice, $exitPrice, $costBasis, $grossProceeds, $fees, $pnl, $openedAtUtc, $closedAtUtc);"
                command.Parameters.AddWithValue("$runId", runId)
                command.Parameters.AddWithValue("$symbol", If(trade.Symbol, String.Empty).Trim().ToUpperInvariant())
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
                    "QuoteBalance, OpenValue, OpenPositions, LastAction, UpdatedAt " &
                    "FROM SpotBotRuns ORDER BY Id DESC;"
                Using reader = command.ExecuteReader()
                    table.Load(reader)
                End Using
            End Using
        End Using

        Return table
    End Function

    Private Shared Function GetConnectionString() As String
        Return $"Data Source={PortfolioRepository.GetDatabasePath()}"
    End Function

    Private Shared Function UtcText(value As DateTime) As String
        Return value.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture)
    End Function

    Private Shared Sub AddRealParameter(command As SqliteCommand, name As String, value As Decimal)
        Dim parameter = command.Parameters.Add(name, SqliteType.Real)
        parameter.Value = Convert.ToDouble(value, CultureInfo.InvariantCulture)
    End Sub

End Class
