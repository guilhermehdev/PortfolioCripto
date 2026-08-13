Imports Microsoft.VisualBasic.ApplicationServices
Imports System.IO

Namespace My
    Partial Friend Class MyApplication

        Private Sub MyApplication_Startup(sender As Object, e As StartupEventArgs) Handles Me.Startup
            Try
                PortfolioDb.Initialize()

                Dim jsonPath As String = Path.Combine(
                    My.Application.Info.DirectoryPath,
                    "JSON",
                    "portfolio.json")

                If PortfolioDb.GetDatabaseRowCount() = 0 AndAlso File.Exists(jsonPath) Then
                    Dim imported As Integer = PortfolioDb.MigrateFromJson(jsonPath)
                    Debug.WriteLine($"SQLite: {imported} registros migrados do portfolio.json.")
                End If

            Catch ex As Exception
                Debug.WriteLine("Erro ao inicializar SQLite: " & ex.Message)
            End Try
        End Sub

    End Class
End Namespace
