Imports System.Net.Http
Imports Newtonsoft.Json.Linq

Public Class Coingecko

    Public Async Function CGECKO_GetBTCDominance() As Task(Of Decimal)
        Try

            Using client As New HttpClient()
                Dim url As String = "https://api.coingecko.com/api/v3/global"
                Dim response = Await client.GetAsync(url)

                If response.IsSuccessStatusCode Then
                    Dim json As String = Await response.Content.ReadAsStringAsync()
                    Dim jObj As JObject = JObject.Parse(json)
                    Dim dominance As Decimal = jObj("data")("market_cap_percentage")("btc").Value(Of Decimal)()
                    Return dominance
                Else
                    Throw New Exception("Erro ao consultar dominância BTC: " & response.StatusCode)
                End If
            End Using
        Catch ex As Exception
            Debug.Write($"Erro ao consultar dominância BTC: {ex.Message}")
            Return False
        End Try
    End Function

End Class
