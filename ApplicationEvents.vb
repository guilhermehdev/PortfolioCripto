Imports Microsoft.VisualBasic.ApplicationServices
Imports System.IO

Namespace My
    ' The following events are available for MyApplication:
    ' Startup: Raised when the application starts, before the startup form is created.
    ' Shutdown: Raised after all application forms are closed.  This event is not raised if the application terminates abnormally.
    ' UnhandledException: Raised if the application encounters an unhandled exception.
    ' StartupNextInstance: Raised when launching a single-instance application and the application is already active.
    ' NetworkAvailabilityChanged: Raised when the network connection is connected or disconnected.

    Partial Friend Class MyApplication

        Private Sub MyApplication_Startup(sender As Object, e As StartupEventArgs) Handles Me.Startup
            Try
                PortfolioDb.Initialize()

                Dim jsonPath As String = Path.Combine(
                    Application.StartupPath,
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
