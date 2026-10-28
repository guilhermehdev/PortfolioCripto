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
                        "UpdatedAt TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP);"
                    command.ExecuteNonQuery()
                End Using

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
                    "INSERT INTO DcaPlans(Symbol, Enabled, BudgetQuote, CooldownMinutes, DropLevels, UpdatedAt) " &
                    "VALUES($symbol, $enabled, $budget, $cooldown, $levels, CURRENT_TIMESTAMP) " &
                    "ON CONFLICT(Symbol) DO UPDATE SET Enabled = excluded.Enabled, " &
                    "BudgetQuote = excluded.BudgetQuote, CooldownMinutes = excluded.CooldownMinutes, " &
                    "DropLevels = excluded.DropLevels, UpdatedAt = CURRENT_TIMESTAMP;"
                command.Parameters.AddWithValue("$symbol", NormalizeSymbol(plan.Symbol))
                command.Parameters.AddWithValue("$enabled", If(plan.Enabled, 1, 0))
                command.Parameters.AddWithValue("$budget", Convert.ToDouble(Math.Max(0D, plan.BudgetQuote), CultureInfo.InvariantCulture))
                command.Parameters.AddWithValue("$cooldown", Math.Max(0, plan.CooldownMinutes))
                command.Parameters.AddWithValue("$levels", SerializeLevels(plan.Levels))
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
                command.CommandText = "SELECT Symbol, Enabled, BudgetQuote, CooldownMinutes, DropLevels FROM DcaPlans ORDER BY Symbol;"
                Using reader = command.ExecuteReader()
                    While reader.Read()
                        result.Add(New DcaPlan With {
                            .Symbol = reader.GetString(0),
                            .Enabled = reader.GetInt32(1) <> 0,
                            .BudgetQuote = ReadDecimal(reader, 2),
                            .CooldownMinutes = Math.Max(0, reader.GetInt32(3)),
                            .Levels = DeserializeLevels(If(reader.IsDBNull(4), String.Empty, reader.GetString(4)))})
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

    Private Shared Function ReadDecimal(reader As SqliteDataReader, ordinal As Integer) As Decimal
        If reader.IsDBNull(ordinal) Then Return 0D
        Return Convert.ToDecimal(reader.GetDouble(ordinal), CultureInfo.InvariantCulture)
    End Function

    Private Shared Sub AddRealParameter(command As SqliteCommand, name As String, value As Decimal)
        Dim parameter = command.Parameters.Add(name, SqliteType.Real)
        parameter.Value = Convert.ToDouble(value, CultureInfo.InvariantCulture)
    End Sub
End Class
