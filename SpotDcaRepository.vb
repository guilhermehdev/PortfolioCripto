Imports System
Imports System.Collections.Generic
Imports System.Globalization
Imports System.Linq
Imports Microsoft.Data.Sqlite

Public NotInheritable Class SpotDcaRepository
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
                Using command = connection.CreateCommand()
                    command.CommandText =
                        "CREATE TABLE IF NOT EXISTS DcaPlans (" &
                        "Id INTEGER PRIMARY KEY AUTOINCREMENT, " &
                        "Symbol TEXT NOT NULL UNIQUE, " &
                        "Enabled INTEGER NOT NULL DEFAULT 1, " &
                        "BudgetQuote REAL NOT NULL DEFAULT 100, " &
                        "CooldownMinutes INTEGER NOT NULL DEFAULT 60, " &
                        "DropLevels TEXT NOT NULL, " &
                        "UseAutomaticSupports INTEGER NOT NULL DEFAULT 1, " &
                        "CandleInterval TEXT NOT NULL DEFAULT '4h', " &
                        "SupportLookbackCandles INTEGER NOT NULL DEFAULT 200, " &
                        "SupportTolerancePercent REAL NOT NULL DEFAULT 0.5, " &
                        "SupportMinimumTouches INTEGER NOT NULL DEFAULT 2, " &
                        "SupportMaxLevels INTEGER NOT NULL DEFAULT 4, " &
                        "RequireSupportRejection INTEGER NOT NULL DEFAULT 1, " &
                        "ManualSupportLevels TEXT NOT NULL DEFAULT '', " &
                        "UpdatedAt TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP);"
                    command.ExecuteNonQuery()
                End Using

                ' Migração compatível com planos DCA criados antes do modo
                ' automático por zonas de suporte.
                EnsureColumn(connection, "DcaPlans", "UseAutomaticSupports", "INTEGER NOT NULL DEFAULT 0")
                EnsureColumn(connection, "DcaPlans", "CandleInterval", "TEXT NOT NULL DEFAULT '4h'")
                EnsureColumn(connection, "DcaPlans", "SupportLookbackCandles", "INTEGER NOT NULL DEFAULT 200")
                EnsureColumn(connection, "DcaPlans", "SupportTolerancePercent", "REAL NOT NULL DEFAULT 0.5")
                EnsureColumn(connection, "DcaPlans", "SupportMinimumTouches", "INTEGER NOT NULL DEFAULT 2")
                EnsureColumn(connection, "DcaPlans", "SupportMaxLevels", "INTEGER NOT NULL DEFAULT 4")
                EnsureColumn(connection, "DcaPlans", "RequireSupportRejection", "INTEGER NOT NULL DEFAULT 1")
                EnsureColumn(connection, "DcaPlans", "ManualSupportLevels", "TEXT NOT NULL DEFAULT ''")

                Using command = connection.CreateCommand()
                    command.CommandText =
                        "CREATE TABLE IF NOT EXISTS DcaStates (" &
                        "Symbol TEXT PRIMARY KEY, " &
                        "AnchorPrice REAL NOT NULL DEFAULT 0, " &
                        "LastPrice REAL NOT NULL DEFAULT 0, " &
                        "Quantity REAL NOT NULL DEFAULT 0, " &
                        "InvestedQuote REAL NOT NULL DEFAULT 0, " &
                        "AveragePrice REAL NOT NULL DEFAULT 0, " &
                        "LastPurchaseAtUtc TEXT, " &
                        "TriggeredLevels TEXT NOT NULL DEFAULT '', " &
                        "UpdatedAt TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP);"
                    command.ExecuteNonQuery()
                End Using
            End Using

            _initialized = True
        End SyncLock
    End Sub

    Public Shared Sub SavePlan(plan As DcaPlan)
        If plan Is Nothing Then Throw New ArgumentNullException(NameOf(plan))
        Initialize()

        Using connection As New SqliteConnection(GetConnectionString())
            connection.Open()
            Using command = connection.CreateCommand()
                command.CommandText =
                    "INSERT INTO DcaPlans(Symbol, Enabled, BudgetQuote, CooldownMinutes, DropLevels, " &
                    "UseAutomaticSupports, CandleInterval, SupportLookbackCandles, SupportTolerancePercent, " &
                    "SupportMinimumTouches, SupportMaxLevels, RequireSupportRejection, ManualSupportLevels, UpdatedAt) " &
                    "VALUES($symbol, $enabled, $budget, $cooldown, $levels, $automatic, $interval, $lookback, " &
                    "$tolerance, $minTouches, $maxLevels, $rejection, $manualSupports, CURRENT_TIMESTAMP) " &
                    "ON CONFLICT(Symbol) DO UPDATE SET Enabled = excluded.Enabled, " &
                    "BudgetQuote = excluded.BudgetQuote, CooldownMinutes = excluded.CooldownMinutes, " &
                    "DropLevels = excluded.DropLevels, UseAutomaticSupports = excluded.UseAutomaticSupports, " &
                    "CandleInterval = excluded.CandleInterval, SupportLookbackCandles = excluded.SupportLookbackCandles, " &
                    "SupportTolerancePercent = excluded.SupportTolerancePercent, " &
                    "SupportMinimumTouches = excluded.SupportMinimumTouches, SupportMaxLevels = excluded.SupportMaxLevels, " &
                    "RequireSupportRejection = excluded.RequireSupportRejection, " &
                    "ManualSupportLevels = excluded.ManualSupportLevels, UpdatedAt = CURRENT_TIMESTAMP;"
                command.Parameters.AddWithValue("$symbol", NormalizeSymbol(plan.Symbol))
                command.Parameters.AddWithValue("$enabled", If(plan.Enabled, 1, 0))
                command.Parameters.AddWithValue("$budget", Convert.ToDouble(Math.Max(0D, plan.BudgetQuote), CultureInfo.InvariantCulture))
                command.Parameters.AddWithValue("$cooldown", Math.Max(0, plan.CooldownMinutes))
                command.Parameters.AddWithValue("$levels", SerializeLevels(plan.Levels))
                command.Parameters.AddWithValue("$automatic", If(plan.UseAutomaticSupports, 1, 0))
                command.Parameters.AddWithValue("$interval", NormalizeInterval(plan.CandleInterval))
                command.Parameters.AddWithValue("$lookback", Math.Max(30, plan.SupportLookbackCandles))
                AddRealParameter(command, "$tolerance", Math.Max(0.05D, plan.SupportTolerancePercent))
                command.Parameters.AddWithValue("$minTouches", Math.Max(1, plan.SupportMinimumTouches))
                command.Parameters.AddWithValue("$maxLevels", Math.Max(1, plan.SupportMaxLevels))
                command.Parameters.AddWithValue("$rejection", If(plan.RequireSupportRejection, 1, 0))
                command.Parameters.AddWithValue("$manualSupports", SerializeManualSupports(plan.ManualSupportOverrides))
                command.ExecuteNonQuery()
            End Using
        End Using
    End Sub

    Public Shared Function LoadPlans() As List(Of DcaPlan)
        Initialize()
        Dim result As New List(Of DcaPlan)()

        Using connection As New SqliteConnection(GetConnectionString())
            connection.Open()
            Using command = connection.CreateCommand()
                command.CommandText =
                    "SELECT Symbol, Enabled, BudgetQuote, CooldownMinutes, DropLevels, " &
                    "UseAutomaticSupports, CandleInterval, SupportLookbackCandles, SupportTolerancePercent, " &
                    "SupportMinimumTouches, SupportMaxLevels, RequireSupportRejection, ManualSupportLevels " &
                    "FROM DcaPlans ORDER BY Symbol;"
                Using reader = command.ExecuteReader()
                    While reader.Read()
                        result.Add(New DcaPlan With {
                            .Symbol = reader.GetString(0),
                            .Enabled = reader.GetInt32(1) <> 0,
                            .BudgetQuote = ReadDecimal(reader, 2),
                            .CooldownMinutes = Math.Max(0, reader.GetInt32(3)),
                            .Levels = DeserializeLevels(If(reader.IsDBNull(4), String.Empty, reader.GetString(4))),
                            .UseAutomaticSupports = reader.GetInt32(5) <> 0,
                            .CandleInterval = NormalizeInterval(If(reader.IsDBNull(6), "4h", reader.GetString(6))),
                            .SupportLookbackCandles = Math.Max(30, reader.GetInt32(7)),
                            .SupportTolerancePercent = Math.Max(0.05D, ReadDecimal(reader, 8)),
                            .SupportMinimumTouches = Math.Max(1, reader.GetInt32(9)),
                            .SupportMaxLevels = Math.Max(1, reader.GetInt32(10)),
                            .RequireSupportRejection = reader.GetInt32(11) <> 0,
                            .ManualSupportOverrides = DeserializeManualSupports(If(reader.IsDBNull(12), String.Empty, reader.GetString(12)))})
                    End While
                End Using
            End Using
        End Using

        Return result
    End Function

    Public Shared Function LoadStates() As List(Of DcaState)
        Initialize()
        Dim result As New List(Of DcaState)()

        Using connection As New SqliteConnection(GetConnectionString())
            connection.Open()
            Using command = connection.CreateCommand()
                command.CommandText = "SELECT Symbol, AnchorPrice, LastPrice, Quantity, InvestedQuote, AveragePrice, LastPurchaseAtUtc, TriggeredLevels FROM DcaStates;"
                Using reader = command.ExecuteReader()
                    While reader.Read()
                        Dim lastPurchase = DateTime.MinValue
                        If Not reader.IsDBNull(6) Then DateTime.TryParse(reader.GetString(6), Nothing, DateTimeStyles.RoundtripKind, lastPurchase)
                        result.Add(New DcaState With {
                            .Symbol = reader.GetString(0),
                            .AnchorPrice = ReadDecimal(reader, 1),
                            .LastPrice = ReadDecimal(reader, 2),
                            .Quantity = ReadDecimal(reader, 3),
                            .InvestedQuote = ReadDecimal(reader, 4),
                            .AveragePrice = ReadDecimal(reader, 5),
                            .LastPurchaseAtUtc = lastPurchase,
                            .TriggeredLevels = DeserializeTriggeredLevels(If(reader.IsDBNull(7), String.Empty, reader.GetString(7)))})
                    End While
                End Using
            End Using
        End Using

        Return result
    End Function

    Public Shared Sub SaveState(state As DcaState)
        If state Is Nothing Then Return
        Initialize()

        Using connection As New SqliteConnection(GetConnectionString())
            connection.Open()
            Using command = connection.CreateCommand()
                command.CommandText =
                    "INSERT INTO DcaStates(Symbol, AnchorPrice, LastPrice, Quantity, InvestedQuote, AveragePrice, LastPurchaseAtUtc, TriggeredLevels, UpdatedAt) " &
                    "VALUES($symbol, $anchor, $last, $quantity, $invested, $average, $purchaseAt, $triggered, CURRENT_TIMESTAMP) " &
                    "ON CONFLICT(Symbol) DO UPDATE SET AnchorPrice = excluded.AnchorPrice, LastPrice = excluded.LastPrice, " &
                    "Quantity = excluded.Quantity, InvestedQuote = excluded.InvestedQuote, AveragePrice = excluded.AveragePrice, " &
                    "LastPurchaseAtUtc = excluded.LastPurchaseAtUtc, TriggeredLevels = excluded.TriggeredLevels, UpdatedAt = CURRENT_TIMESTAMP;"
                command.Parameters.AddWithValue("$symbol", NormalizeSymbol(state.Symbol))
                AddRealParameter(command, "$anchor", state.AnchorPrice)
                AddRealParameter(command, "$last", state.LastPrice)
                AddRealParameter(command, "$quantity", state.Quantity)
                AddRealParameter(command, "$invested", state.InvestedQuote)
                AddRealParameter(command, "$average", state.AveragePrice)
                command.Parameters.AddWithValue("$purchaseAt", If(state.LastPurchaseAtUtc = DateTime.MinValue, DBNull.Value, state.LastPurchaseAtUtc.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture)))
                command.Parameters.AddWithValue("$triggered", String.Join("|", state.TriggeredLevels.OrderBy(Function(item) item, StringComparer.Ordinal)))
                command.ExecuteNonQuery()
            End Using
        End Using
    End Sub

    Public Shared Sub ResetState(symbol As String)
        Initialize()
        Using connection As New SqliteConnection(GetConnectionString())
            connection.Open()
            Using command = connection.CreateCommand()
                command.CommandText = "DELETE FROM DcaStates WHERE Symbol = $symbol;"
                command.Parameters.AddWithValue("$symbol", NormalizeSymbol(symbol))
                command.ExecuteNonQuery()
            End Using
        End Using
    End Sub

    Public Shared Sub DeletePlan(symbol As String)
        Initialize()
        Using connection As New SqliteConnection(GetConnectionString())
            connection.Open()
            Using transaction = connection.BeginTransaction()
                Using command = connection.CreateCommand()
                    command.Transaction = transaction
                    command.CommandText =
                        "DELETE FROM DcaPlans WHERE Symbol = $symbol; " &
                        "DELETE FROM DcaStates WHERE Symbol = $symbol;"
                    command.Parameters.AddWithValue("$symbol", NormalizeSymbol(symbol))
                    command.ExecuteNonQuery()
                End Using
                transaction.Commit()
            End Using
        End Using
    End Sub

    Private Shared Function GetConnectionString() As String
        Return $"Data Source={PortfolioRepository.GetDatabasePath()}"
    End Function

    Private Shared Function NormalizeSymbol(symbol As String) As String
        Return If(symbol, String.Empty).Trim().ToUpperInvariant().Replace("USDT", String.Empty)
    End Function

    Private Shared Function SerializeLevels(levels As IEnumerable(Of DcaLevel)) As String
        Return String.Join(";", If(levels, Enumerable.Empty(Of DcaLevel)()).
            Where(Function(item) item IsNot Nothing AndAlso item.DropPercent > 0D AndAlso item.AllocationPercent > 0D).
            OrderBy(Function(item) item.DropPercent).
            Select(Function(item) item.DropPercent.ToString("0.####", CultureInfo.InvariantCulture) & ":" & item.AllocationPercent.ToString("0.####", CultureInfo.InvariantCulture)))
    End Function

    Private Shared Function DeserializeLevels(value As String) As List(Of DcaLevel)
        Dim result As New List(Of DcaLevel)()
        For Each part In If(value, String.Empty).Split(";"c, StringSplitOptions.RemoveEmptyEntries)
            Dim pieces = part.Split(":"c)
            If pieces.Length <> 2 Then Continue For
            Dim drop As Decimal
            Dim allocation As Decimal
            If Decimal.TryParse(pieces(0), NumberStyles.Float, CultureInfo.InvariantCulture, drop) AndAlso
               Decimal.TryParse(pieces(1), NumberStyles.Float, CultureInfo.InvariantCulture, allocation) Then
                result.Add(New DcaLevel With {.DropPercent = drop, .AllocationPercent = allocation})
            End If
        Next
        Return result.OrderBy(Function(item) item.DropPercent).ToList()
    End Function

    Private Shared Function DeserializeTriggeredLevels(value As String) As HashSet(Of String)
        Return New HashSet(Of String)(If(value, String.Empty).Split("|"c, StringSplitOptions.RemoveEmptyEntries), StringComparer.Ordinal)
    End Function

    Private Shared Function SerializeManualSupports(levels As IEnumerable(Of DcaSupportOverride)) As String
        Return String.Join(";", If(levels, Enumerable.Empty(Of DcaSupportOverride)()).
            Where(Function(item) item IsNot Nothing AndAlso item.Enabled AndAlso item.Price > 0D AndAlso item.AllocationPercent > 0D).
            Select(Function(item)
                       Return String.Join(":", {
                           item.Price.ToString("0.################", CultureInfo.InvariantCulture),
                           item.AllocationPercent.ToString("0.####", CultureInfo.InvariantCulture),
                           Math.Max(0.05D, item.TolerancePercent).ToString("0.####", CultureInfo.InvariantCulture)})
                   End Function))
    End Function

    Private Shared Function DeserializeManualSupports(value As String) As List(Of DcaSupportOverride)
        Dim result As New List(Of DcaSupportOverride)()
        For Each part In If(value, String.Empty).Split(";"c, StringSplitOptions.RemoveEmptyEntries)
            Dim pieces = part.Split(":"c)
            If pieces.Length < 2 Then Continue For

            Dim price As Decimal
            Dim allocation As Decimal
            Dim tolerance As Decimal = 0.5D
            If Not Decimal.TryParse(pieces(0), NumberStyles.Float, CultureInfo.InvariantCulture, price) OrElse
               Not Decimal.TryParse(pieces(1), NumberStyles.Float, CultureInfo.InvariantCulture, allocation) Then
                Continue For
            End If

            If pieces.Length >= 3 Then
                Decimal.TryParse(pieces(2), NumberStyles.Float, CultureInfo.InvariantCulture, tolerance)
            End If

            If price > 0D AndAlso allocation > 0D Then
                result.Add(New DcaSupportOverride With {
                    .Price = price,
                    .AllocationPercent = allocation,
                    .TolerancePercent = Math.Max(0.05D, tolerance),
                    .Enabled = True})
            End If
        Next
        Return result
    End Function

    Private Shared Function NormalizeInterval(value As String) As String
        If String.Equals(value?.Trim(), "1d", StringComparison.OrdinalIgnoreCase) Then
            Return "1d"
        End If
        Return "4h"
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

    Private Shared Function ReadDecimal(reader As SqliteDataReader, ordinal As Integer) As Decimal
        If reader.IsDBNull(ordinal) Then Return 0D
        Return Convert.ToDecimal(reader.GetDouble(ordinal), CultureInfo.InvariantCulture)
    End Function

    Private Shared Sub AddRealParameter(command As SqliteCommand, name As String, value As Decimal)
        Dim parameter = command.Parameters.Add(name, SqliteType.Real)
        parameter.Value = Convert.ToDouble(value, CultureInfo.InvariantCulture)
    End Sub
End Class
