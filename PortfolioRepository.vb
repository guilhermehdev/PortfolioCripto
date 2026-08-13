Imports Microsoft.Data.Sqlite
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
                command.CommandText = """
                    PRAGMA journal_mode = WAL;
                    PRAGMA foreign_keys = ON;

                    CREATE TABLE IF NOT EXISTS PortfolioItems (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Cripto TEXT NOT NULL,
                        Symbol TEXT NOT NULL,
                        InitialPrice NUMERIC NOT NULL DEFAULT 0,
                        Quantity NUMERIC NOT NULL DEFAULT 0,
                        Data TEXT,
                        Wallet TEXT NOT NULL,
                        LastPrice NUMERIC NOT NULL DEFAULT 0,
                        CreatedAt TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
                        UpdatedAt TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP
                    );

                    CREATE INDEX IF NOT EXISTS IX_PortfolioItems_Symbol
                        ON PortfolioItems(Symbol);

                    CREATE INDEX IF NOT EXISTS IX_PortfolioItems_Wallet
                        ON PortfolioItems(Wallet);
                """
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
                command.CommandText = """
                    SELECT
                        Id,
                        Cripto,
                        Symbol,
                        InitialPrice,
                        Quantity,
                        Data,
                        Wallet,
                        LastPrice,
                        CreatedAt,
                        UpdatedAt
                    FROM PortfolioItems
                    ORDER BY Id;
                """

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
                        findCommand.CommandText = """
                            SELECT Id
                            FROM PortfolioItems
                            WHERE Cripto = $cripto
                              AND Symbol = $symbol
                              AND Wallet = $wallet
                              AND COALESCE(Data, '') = $data
                            LIMIT 1;
                        """
                        findCommand.Parameters.AddWithValue("$cripto", cripto)
                        findCommand.Parameters.AddWithValue("$symbol", symbol)
                        findCommand.Parameters.AddWithValue("$wallet", wallet)
                        findCommand.Parameters.AddWithValue("$data", data)

                        Dim result = findCommand.ExecuteScalar()
                        If result IsNot Nothing AndAlso result IsNot DBNull.Value Then
                            existingId = Convert.ToInt64(result, CultureInfo.InvariantCulture)
                        End If
                    End Using

                    If existingId > 0 Then
                        Using updateCommand As SqliteCommand = connection.CreateCommand()
                            updateCommand.Transaction = transaction
                            updateCommand.CommandText = """
                                UPDATE PortfolioItems
                                SET InitialPrice = $initialPrice,
                                    Quantity = $quantity,
                                    LastPrice = $lastPrice,
                                    UpdatedAt = CURRENT_TIMESTAMP
                                WHERE Id = $id;
                            """
                            updateCommand.Parameters.AddWithValue("$initialPrice", initialPrice)
                            updateCommand.Parameters.AddWithValue("$quantity", quantity)
                            updateCommand.Parameters.AddWithValue("$lastPrice", lastPrice)
                            updateCommand.Parameters.AddWithValue("$id", existingId)
                            updateCommand.ExecuteNonQuery()
                        End Using
                    Else
                        Using insertCommand As SqliteCommand = connection.CreateCommand()
                            insertCommand.Transaction = transaction
                            insertCommand.CommandText = """
                                INSERT INTO PortfolioItems
                                    (Cripto, Symbol, InitialPrice, Quantity, Data, Wallet, LastPrice)
                                VALUES
                                    ($cripto, $symbol, $initialPrice, $quantity, $data, $wallet, $lastPrice);
                                SELECT last_insert_rowid();
                            """
                            insertCommand.Parameters.AddWithValue("$cripto", cripto)
                            insertCommand.Parameters.AddWithValue("$symbol", symbol)
                            insertCommand.Parameters.AddWithValue("$initialPrice", initialPrice)
                            insertCommand.Parameters.AddWithValue("$quantity", quantity)
                            insertCommand.Parameters.AddWithValue("$data", data)
                            insertCommand.Parameters.AddWithValue("$wallet", wallet)
                            insertCommand.Parameters.AddWithValue("$lastPrice", lastPrice)

                            existingId = Convert.ToInt64(
                                insertCommand.ExecuteScalar(),
                                CultureInfo.InvariantCulture)
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

        Using connection As New SqliteConnection(ConnectionString)
            connection.Open()

            Using command As SqliteCommand = connection.CreateCommand()
                command.CommandText = """
                    UPDATE PortfolioItems
                    SET LastPrice = $lastPrice,
                        UpdatedAt = CURRENT_TIMESTAMP
                    WHERE Id = $id;
                """
                command.Parameters.AddWithValue("$lastPrice", lastPrice)
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

End Class
