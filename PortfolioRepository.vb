Imports Microsoft.Data.Sqlite
Imports Newtonsoft.Json.Linq
Imports System.Data
Imports System.Globalization
Imports System.IO

Public NotInheritable Class PortfolioRepository

    Private Sub New()
    End Sub

    Private Shared ReadOnly DatabasePath As String =
        Path.Combine(Application.StartupPath, "PortfolioCripto.db")

    Private Shared ReadOnly ConnectionString As String =
        $"Data Source={DatabasePath}"

    Public Shared Sub Initialize()
        Using connection As New SqliteConnection(ConnectionString)
            connection.Open()

            Using command As SqliteCommand = connection.CreateCommand()
                command.CommandText = "PRAGMA journal_mode = WAL;"
                command.ExecuteNonQuery()
                command.CommandText = "PRAGMA foreign_keys = ON;"
                command.ExecuteNonQuery()
            End Using

            Using command As SqliteCommand = connection.CreateCommand()
                command.CommandText =
                    "CREATE TABLE IF NOT EXISTS PortfolioItems (" &
                    "Id INTEGER PRIMARY KEY AUTOINCREMENT, " &
                    "Cripto TEXT NOT NULL, " &
                    "Symbol TEXT NOT NULL, " &
                    "InitialPrice NUMERIC NOT NULL DEFAULT 0, " &
                    "Quantity NUMERIC NOT NULL DEFAULT 0, " &
                    "Data TEXT, " &
                    "Wallet TEXT NOT NULL, " &
                    "LastPrice NUMERIC NOT NULL DEFAULT 0, " &
                    "CreatedAt TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP, " &
                    "UpdatedAt TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP" &
                    ");"
                command.ExecuteNonQuery()
            End Using

            Using command As SqliteCommand = connection.CreateCommand()
                command.CommandText =
                    "CREATE TABLE IF NOT EXISTS CryptoSymbols (" &
                    "Id INTEGER PRIMARY KEY, " &
                    "Symbol TEXT NOT NULL UNIQUE COLLATE NOCASE" &
                    ");"
                command.ExecuteNonQuery()
            End Using

            Using command As SqliteCommand = connection.CreateCommand()
                command.CommandText =
                    "CREATE TABLE IF NOT EXISTS Wallets (" &
                    "Id INTEGER PRIMARY KEY AUTOINCREMENT, " &
                    "Name TEXT NOT NULL UNIQUE COLLATE NOCASE" &
                    ");"
                command.ExecuteNonQuery()
            End Using

            Using command As SqliteCommand = connection.CreateCommand()
                command.CommandText =
                    "CREATE INDEX IF NOT EXISTS IX_PortfolioItems_Symbol " &
                    "ON PortfolioItems(Symbol);"
                command.ExecuteNonQuery()
            End Using

            Using command As SqliteCommand = connection.CreateCommand()
                command.CommandText =
                    "CREATE INDEX IF NOT EXISTS IX_PortfolioItems_Wallet " &
                    "ON PortfolioItems(Wallet);"
                command.ExecuteNonQuery()
            End Using
        End Using
    End Sub

    Public Shared Function DatabaseExists() As Boolean
        Return File.Exists(DatabasePath)
    End Function

    Public Shared Function GetDatabasePath() As String
        Return DatabasePath
    End Function

    Public Shared Function GetAll() As DataTable
        Initialize()
        Dim table As New DataTable()

        Using connection As New SqliteConnection(ConnectionString)
            connection.Open()
            Using command As SqliteCommand = connection.CreateCommand()
                command.CommandText =
                    "SELECT Id, Cripto, Symbol, InitialPrice, Quantity, Data, Wallet, LastPrice, CreatedAt, UpdatedAt " &
                    "FROM PortfolioItems ORDER BY Id;"

                Using reader = command.ExecuteReader()
                    table.Load(reader)
                End Using
            End Using
        End Using

        Return table
    End Function

    Public Shared Function Count() As Long
        Initialize()

        Using connection As New SqliteConnection(ConnectionString)
            connection.Open()
            Using command As SqliteCommand = connection.CreateCommand()
                command.CommandText = "SELECT COUNT(1) FROM PortfolioItems;"
                Return Convert.ToInt64(command.ExecuteScalar(), CultureInfo.InvariantCulture)
            End Using
        End Using
    End Function

    Public Shared Function GetCryptoSymbols() As DataTable
        Initialize()
        Dim table As New DataTable()

        Using connection As New SqliteConnection(ConnectionString)
            connection.Open()
            Using command As SqliteCommand = connection.CreateCommand()
                command.CommandText =
                    "SELECT Id, Symbol FROM CryptoSymbols ORDER BY Symbol;"

                Using reader = command.ExecuteReader()
                    table.Load(reader)
                End Using
            End Using
        End Using

        Return table
    End Function

    Public Shared Function GetWallets() As DataTable
        Initialize()
        Dim table As New DataTable()

        Using connection As New SqliteConnection(ConnectionString)
            connection.Open()
            Using command As SqliteCommand = connection.CreateCommand()
                command.CommandText =
                    "SELECT Id, Name FROM Wallets ORDER BY Name;"

                Using reader = command.ExecuteReader()
                    table.Load(reader)
                End Using
            End Using
        End Using

        Return table
    End Function

    Public Shared Sub AddCryptoSymbol(symbol As String, Optional id As Integer? = Nothing)
        Initialize()

        symbol = If(symbol, String.Empty).Trim().ToUpperInvariant()
        If String.IsNullOrWhiteSpace(symbol) Then
            Throw New ArgumentException("Símbolo não pode ser vazio.", NameOf(symbol))
        End If

        Using connection As New SqliteConnection(ConnectionString)
            connection.Open()

            Using command As SqliteCommand = connection.CreateCommand()
                If id.HasValue Then
                    command.CommandText =
                        "INSERT INTO CryptoSymbols(Id, Symbol) VALUES($id, $symbol) " &
                        "ON CONFLICT(Id) DO UPDATE SET Symbol = excluded.Symbol;"
                    command.Parameters.AddWithValue("$id", id.Value)
                Else
                    command.CommandText =
                        "INSERT INTO CryptoSymbols(Symbol) VALUES($symbol) " &
                        "ON CONFLICT(Symbol) DO NOTHING;"
                End If

                command.Parameters.AddWithValue("$symbol", symbol)
                command.ExecuteNonQuery()
            End Using
        End Using
    End Sub

    Public Shared Sub DeleteCryptoSymbol(symbol As String)
        Initialize()

        Using connection As New SqliteConnection(ConnectionString)
            connection.Open()
            Using command As SqliteCommand = connection.CreateCommand()
                command.CommandText =
                    "DELETE FROM CryptoSymbols WHERE Symbol = $symbol;"
                command.Parameters.AddWithValue(
                    "$symbol",
                    If(symbol, String.Empty).Trim().ToUpperInvariant())
                command.ExecuteNonQuery()
            End Using
        End Using
    End Sub

    Public Shared Sub AddWallet(name As String)
        Initialize()

        name = If(name, String.Empty).Trim()
        If String.IsNullOrWhiteSpace(name) Then
            Throw New ArgumentException("Wallet não pode ser vazia.", NameOf(name))
        End If

        Using connection As New SqliteConnection(ConnectionString)
            connection.Open()

            Using command As SqliteCommand = connection.CreateCommand()
                command.CommandText =
                    "INSERT INTO Wallets(Name) VALUES($name) " &
                    "ON CONFLICT(Name) DO NOTHING;"
                command.Parameters.AddWithValue("$name", name)
                command.ExecuteNonQuery()
            End Using
        End Using
    End Sub

    Public Shared Sub DeleteWallet(id As Long)
        Initialize()

        Using connection As New SqliteConnection(ConnectionString)
            connection.Open()
            Using command As SqliteCommand = connection.CreateCommand()
                command.CommandText = "DELETE FROM Wallets WHERE Id = $id;"
                command.Parameters.AddWithValue("$id", id)
                command.ExecuteNonQuery()
            End Using
        End Using
    End Sub

    Public Shared Sub MigrateCatalogsFromJson(cryptoJsonPath As String, walletJsonPath As String)
        Initialize()

        If File.Exists(cryptoJsonPath) Then
            Dim jsonObject As JObject = JObject.Parse(File.ReadAllText(cryptoJsonPath))
            For Each propertyPair As KeyValuePair(Of String, JToken) In jsonObject
                If propertyPair.Value.Type <> JTokenType.Array Then Continue For

                For Each item As JToken In propertyPair.Value
                    Dim symbol As String = item("Symbol")?.ToString()
                    Dim idToken As JToken = item("Id")
                    Dim idValue As Integer

                    If String.IsNullOrWhiteSpace(symbol) Then Continue For

                    If idToken IsNot Nothing AndAlso Integer.TryParse(idToken.ToString(), idValue) Then
                        AddCryptoSymbol(symbol, idValue)
                    Else
                        AddCryptoSymbol(symbol)
                    End If
                Next
            Next
        End If

        If File.Exists(walletJsonPath) Then
            Dim jsonObject As JObject = JObject.Parse(File.ReadAllText(walletJsonPath))
            For Each propertyPair As KeyValuePair(Of String, JToken) In jsonObject
                If propertyPair.Value.Type <> JTokenType.Array Then Continue For

                For Each item As JToken In propertyPair.Value
                    Dim name As String = item("Wallet")?.ToString()
                    If String.IsNullOrWhiteSpace(name) Then
                        name = item("Name")?.ToString()
                    End If
                    If String.IsNullOrWhiteSpace(name) Then
                        name = item.ToString()
                    End If
                    If Not String.IsNullOrWhiteSpace(name) Then
                        AddWallet(name)
                    End If
                Next
            Next
        End If
    End Sub

    Public Shared Function AddOrUpdate(
        cripto As String,
        symbol As String,
        initialPrice As Decimal,
        quantity As Decimal,
        data As String,
        wallet As String,
        lastPrice As Decimal) As Long

        Initialize()

        cripto = If(cripto, String.Empty).Trim()
        symbol = If(symbol, String.Empty).Trim().ToUpperInvariant()
        wallet = If(wallet, String.Empty).Trim()
        data = If(data, String.Empty)

        If String.IsNullOrWhiteSpace(symbol) Then
            Throw New ArgumentException("Symbol não pode ser vazio.", NameOf(symbol))
        End If

        If String.IsNullOrWhiteSpace(wallet) Then
            Throw New ArgumentException("Wallet não pode ser vazia.", NameOf(wallet))
        End If

        Using connection As New SqliteConnection(ConnectionString)
            connection.Open()

            Using transaction = connection.BeginTransaction()
                Try
                    Dim existingId As Long = 0

                    Using findCommand As SqliteCommand = connection.CreateCommand()
                        findCommand.Transaction = transaction
                        findCommand.CommandText =
                            "SELECT Id FROM PortfolioItems " &
                            "WHERE Cripto = $cripto " &
                            "AND Symbol = $symbol " &
                            "AND Wallet = $wallet " &
                            "AND COALESCE(Data, '') = $data " &
                            "LIMIT 1;"

                        findCommand.Parameters.AddWithValue("$cripto", cripto)
                        findCommand.Parameters.AddWithValue("$symbol", symbol)
                        findCommand.Parameters.AddWithValue("$wallet", wallet)
                        findCommand.Parameters.AddWithValue("$data", data)

                        Dim result = findCommand.ExecuteScalar()
                        If result IsNot Nothing AndAlso result IsNot DBNull.Value Then
                            existingId = Convert.ToInt64(result, CultureInfo.InvariantCulture)
                        End If
                    End Using

                    Dim initialPriceValue As Double = Convert.ToDouble(initialPrice)
                    Dim quantityValue As Double = Convert.ToDouble(quantity)
                    Dim lastPriceValue As Double = Convert.ToDouble(lastPrice)

                    If existingId > 0 Then
                        Using updateCommand As SqliteCommand = connection.CreateCommand()
                            updateCommand.Transaction = transaction
                            updateCommand.CommandText =
                                "UPDATE PortfolioItems SET " &
                                "InitialPrice = $initialPrice, " &
                                "Quantity = $quantity, " &
                                "LastPrice = $lastPrice, " &
                                "UpdatedAt = CURRENT_TIMESTAMP " &
                                "WHERE Id = $id;"

                            AddRealParameter(updateCommand, "$initialPrice", initialPriceValue)
                            AddRealParameter(updateCommand, "$quantity", quantityValue)
                            AddRealParameter(updateCommand, "$lastPrice", lastPriceValue)
                            updateCommand.Parameters.AddWithValue("$id", existingId)
                            updateCommand.ExecuteNonQuery()
                        End Using
                    Else
                        Using insertCommand As SqliteCommand = connection.CreateCommand()
                            insertCommand.Transaction = transaction
                            insertCommand.CommandText =
                                "INSERT INTO PortfolioItems " &
                                "(Cripto, Symbol, InitialPrice, Quantity, Data, Wallet, LastPrice) " &
                                "VALUES ($cripto, $symbol, $initialPrice, $quantity, $data, $wallet, $lastPrice);"

                            insertCommand.Parameters.AddWithValue("$cripto", cripto)
                            insertCommand.Parameters.AddWithValue("$symbol", symbol)
                            AddRealParameter(insertCommand, "$initialPrice", initialPriceValue)
                            AddRealParameter(insertCommand, "$quantity", quantityValue)
                            insertCommand.Parameters.AddWithValue("$data", data)
                            insertCommand.Parameters.AddWithValue("$wallet", wallet)
                            AddRealParameter(insertCommand, "$lastPrice", lastPriceValue)
                            insertCommand.ExecuteNonQuery()
                        End Using

                        Using idCommand As SqliteCommand = connection.CreateCommand()
                            idCommand.Transaction = transaction
                            idCommand.CommandText = "SELECT last_insert_rowid();"
                            existingId = Convert.ToInt64(idCommand.ExecuteScalar(), CultureInfo.InvariantCulture)
                        End Using
                    End If

                    transaction.Commit()
                    Return existingId

                Catch
                    transaction.Rollback()
                    Throw
                End Try
            End Using
        End Using
    End Function

    Public Shared Sub UpdateLastPrice(id As Long, lastPrice As Decimal)
        Initialize()

        Dim lastPriceValue As Double = Convert.ToDouble(lastPrice)

        Using connection As New SqliteConnection(ConnectionString)
            connection.Open()

            Using command As SqliteCommand = connection.CreateCommand()
                command.CommandText =
                    "UPDATE PortfolioItems SET " &
                    "LastPrice = $lastPrice, " &
                    "UpdatedAt = CURRENT_TIMESTAMP " &
                    "WHERE Id = $id;"

                AddRealParameter(command, "$lastPrice", lastPriceValue)
                command.Parameters.AddWithValue("$id", id)
                command.ExecuteNonQuery()
            End Using
        End Using
    End Sub

    Public Shared Sub Delete(id As Long)
        Initialize()

        Using connection As New SqliteConnection(ConnectionString)
            connection.Open()

            Using command As SqliteCommand = connection.CreateCommand()
                command.CommandText = "DELETE FROM PortfolioItems WHERE Id = $id;"
                command.Parameters.AddWithValue("$id", id)
                command.ExecuteNonQuery()
            End Using
        End Using
    End Sub

    Private Shared Sub AddRealParameter(
        command As SqliteCommand,
        name As String,
        value As Double)

        Dim parameter As SqliteParameter =
            command.Parameters.Add(name, SqliteType.Real)

        parameter.Value = value
    End Sub

End Class