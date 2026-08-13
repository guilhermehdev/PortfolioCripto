Imports Microsoft.VisualBasic.ApplicationServices

Namespace My
    Partial Friend Class MyApplication

        Private Sub MyApplication_Startup(sender As Object, e As StartupEventArgs) Handles Me.Startup
            Try
                PortfolioRepository.Initialize()
                Debug.WriteLine("SQLite inicializado: " & PortfolioRepository.GetDatabasePath())
            Catch ex As Exception
                Debug.WriteLine("Erro ao inicializar SQLite: " & ex.Message)
            End Try
        End Sub

    End Class
End Namespace
