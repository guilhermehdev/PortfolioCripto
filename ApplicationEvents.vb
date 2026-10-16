Imports Microsoft.VisualBasic.ApplicationServices
Imports System.IO

Namespace My
    Partial Friend Class MyApplication

        Private Sub MyApplication_Startup(sender As Object, e As StartupEventArgs) Handles Me.Startup
            Try
                PortfolioRepository.Initialize()

                Dim cryptoJsonPath As String = Path.Combine(
                    My.Application.Info.DirectoryPath,
                    "JSON",
                    "criptos.json")

                Dim walletJsonPath As String = Path.Combine(
                    My.Application.Info.DirectoryPath,
                    "JSON",
                    "wallets.json")

                Dim cryptoTable = PortfolioRepository.GetCryptoSymbols()
                Dim walletTable = PortfolioRepository.GetWallets()

                If (cryptoTable.Rows.Count = 0 OrElse walletTable.Rows.Count = 0) AndAlso
                   (File.Exists(cryptoJsonPath) OrElse File.Exists(walletJsonPath)) Then

                    PortfolioRepository.MigrateCatalogsFromJson(
                        cryptoJsonPath,
                        walletJsonPath)
                End If

                Debug.WriteLine(
                    "SQLite inicializado: " &
                    PortfolioRepository.GetDatabasePath())

            Catch ex As Exception
                Debug.WriteLine(
                    "Erro ao inicializar SQLite: " &
                    ex.Message)
            End Try
        End Sub

    End Class
End Namespace