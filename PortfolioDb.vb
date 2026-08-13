Imports Microsoft.Data.Sqlite
Imports Newtonsoft.Json.Linq
Imports System.Globalization
Imports System.IO

Public Class PortfolioDb

    Private Shared ReadOnly DbPath As String = Path.Combine(Application.StartupPath, "PortfolioCripto.db")
    Private Shared ReadOnly ConnectionString As String = $"Data Source={DbPath}"

    Public Shared Sub Initialize()

        Using cn As New SqliteConnection(ConnectionString)

            cn.Open()

            ' --------------------------------------------------------
            ' Configurações do SQLite
            ' --------------------------------------------------------
            Using cmd As SqliteCommand = cn.CreateCommand()

                cmd.CommandText = "PRAGMA journal_mode = WAL;"
                cmd.ExecuteNonQuery()

                cmd.CommandText = "PRAGMA foreign_keys = ON;"
                cmd.ExecuteNonQuery()

            End Using

            ' --------------------------------------------------------
            ' Tabela principal
            ' --------------------------------------------------------
            Using cmd As SqliteCommand = cn.CreateCommand()

                cmd.CommandText =
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

                cmd.ExecuteNonQuery()

            End Using

            ' --------------------------------------------------------
            ' Índice Symbol
            ' --------------------------------------------------------
            Using cmd As SqliteCommand = cn.CreateCommand()

                cmd.CommandText =
                "CREATE INDEX IF NOT EXISTS IX_PortfolioItems_Symbol " &
                "ON PortfolioItems(Symbol);"

                cmd.ExecuteNonQuery()

            End Using

            ' --------------------------------------------------------
            ' Índice Wallet
            ' --------------------------------------------------------
            Using cmd As SqliteCommand = cn.CreateCommand()

                cmd.CommandText =
                "CREATE INDEX IF NOT EXISTS IX_PortfolioItems_Wallet " &
                "ON PortfolioItems(Wallet);"

                cmd.ExecuteNonQuery()

            End Using

        End Using

    End Sub
    Public Shared Function Exists() As Boolean
        Return File.Exists(DbPath)
    End Function

    Public Shared Function GetDatabasePath() As String
        Return DbPath
    End Function

    Public Shared Function GetDatabaseRowCount() As Long
        Initialize()

        Using cn As New SqliteConnection(ConnectionString)
            cn.Open()

            Using cmd As SqliteCommand = cn.CreateCommand()
                cmd.CommandText = "SELECT COUNT(1) FROM PortfolioItems;"
                Return Convert.ToInt64(cmd.ExecuteScalar(), CultureInfo.InvariantCulture)
            End Using
        End Using
    End Function

    ''' <summary>
    ''' Migra o portfolio.json atual para SQLite sem alterar o JSON original.
    ''' Pode ser executado mais de uma vez sem duplicar registros equivalentes.
    ''' </summary>
    Public Shared Function MigrateFromJson(jsonPath As String) As Integer
        If Not File.Exists(jsonPath) Then
            Throw New FileNotFoundException("Arquivo portfolio.json não encontrado.", jsonPath)
        End If

        Initialize()

        Dim jsonObject As JObject = JObject.Parse(File.ReadAllText(jsonPath))
        Dim imported As Integer = 0

        Using cn As New SqliteConnection(ConnectionString)
            cn.Open()

            Using transaction = cn.BeginTransaction()
                For Each propertyPair As KeyValuePair(Of String, JToken) In jsonObject
                    If propertyPair.Value.Type <> JTokenType.Array Then
                        Continue For
                    End If

                    For Each item As JToken In propertyPair.Value
                        Dim cripto As String = propertyPair.Key
                        Dim symbol As String = item("Symbol")?.ToString()
                        Dim wallet As String = item("Wallet")?.ToString()

                        If String.IsNullOrWhiteSpace(symbol) Then
                            symbol = cripto
                        End If

                        If String.IsNullOrWhiteSpace(wallet) Then
                            wallet = "UNKNOWN"
                        End If

                        Dim initialPrice As Decimal = ReadDecimal(item("InitialPrice"))
                        Dim quantity As Decimal = ReadDecimal(item("Qtd"))
                        Dim data As String = item("Data")?.ToString()
                        Dim lastPrice As Decimal = ReadDecimal(item("LastPrice"))

                        Using checkCmd As SqliteCommand = cn.CreateCommand()
                            checkCmd.Transaction = transaction
                            checkCmd.CommandText =
                                "SELECT COUNT(1) FROM PortfolioItems " &
                                "WHERE Cripto = $cripto " &
                                "AND Symbol = $symbol " &
                                "AND Wallet = $wallet " &
                                "AND Quantity = $quantity " &
                                "AND InitialPrice = $initialPrice " &
                                "AND COALESCE(Data, '') = COALESCE($data, '');"

                            checkCmd.Parameters.AddWithValue("$cripto", cripto)
                            checkCmd.Parameters.AddWithValue("$symbol", symbol)
                            checkCmd.Parameters.AddWithValue("$wallet", wallet)
                            checkCmd.Parameters.AddWithValue("$quantity", quantity)
                            checkCmd.Parameters.AddWithValue("$initialPrice", initialPrice)
                            checkCmd.Parameters.AddWithValue("$data", If(data, String.Empty))

                            If Convert.ToInt32(checkCmd.ExecuteScalar(), CultureInfo.InvariantCulture) > 0 Then
                                Continue For
                            End If
                        End Using

                        Using insertCmd As SqliteCommand = cn.CreateCommand()
                            insertCmd.Transaction = transaction
                            insertCmd.CommandText =
                                "INSERT INTO PortfolioItems " &
                                "(Cripto, Symbol, InitialPrice, Quantity, Data, Wallet, LastPrice) " &
                                "VALUES ($cripto, $symbol, $initialPrice, $quantity, $data, $wallet, $lastPrice);"

                            insertCmd.Parameters.AddWithValue("$cripto", cripto)
                            insertCmd.Parameters.AddWithValue("$symbol", symbol)
                            insertCmd.Parameters.AddWithValue("$initialPrice", initialPrice)
                            insertCmd.Parameters.AddWithValue("$quantity", quantity)
                            insertCmd.Parameters.AddWithValue("$data", If(data, String.Empty))
                            insertCmd.Parameters.AddWithValue("$wallet", wallet)
                            insertCmd.Parameters.AddWithValue("$lastPrice", lastPrice)
                            insertCmd.ExecuteNonQuery()
                        End Using

                        imported += 1
                    Next
                Next

                transaction.Commit()
            End Using
        End Using

        Return imported
    End Function

    Public Shared Function GetPortfolioItems() As DataTable
        Initialize()

        Dim table As New DataTable()

        Using cn As New SqliteConnection(ConnectionString)
            cn.Open()

            Using cmd As SqliteCommand = cn.CreateCommand()
                cmd.CommandText =
                    "SELECT Id, Cripto, Symbol, InitialPrice, Quantity, Data, Wallet, LastPrice, CreatedAt, UpdatedAt " &
                    "FROM PortfolioItems ORDER BY Id;"

                Using reader = cmd.ExecuteReader()
                    table.Load(reader)
                End Using
            End Using
        End Using

        Return table
    End Function

    Public Shared Sub UpdateLastPrice(id As Long, lastPrice As Decimal)
        Initialize()

        Using cn As New SqliteConnection(ConnectionString)
            cn.Open()

            Using cmd As SqliteCommand = cn.CreateCommand()
                cmd.CommandText =
                    "UPDATE PortfolioItems SET LastPrice = $lastPrice, UpdatedAt = CURRENT_TIMESTAMP " &
                    "WHERE Id = $id;"

                cmd.Parameters.AddWithValue("$lastPrice", lastPrice)
                cmd.Parameters.AddWithValue("$id", id)
                cmd.ExecuteNonQuery()
            End Using
        End Using
    End Sub

    Private Shared Function ReadDecimal(token As JToken) As Decimal
        If token Is Nothing OrElse token.Type = JTokenType.Null Then
            Return 0D
        End If

        Dim value As Decimal

        If Decimal.TryParse(token.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, value) Then
            Return value
        End If

        If Decimal.TryParse(token.ToString(), NumberStyles.Any, CultureInfo.GetCultureInfo("pt-BR"), value) Then
            Return value
        End If

        Return 0D
    End Function

End Class
