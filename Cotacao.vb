
Imports System.Globalization
Imports System.Net.Http
Imports System.Net.Http.Headers
Imports System.Text.Json

Public Class Cotacao
    Private Shared ReadOnly apiKey As String = My.Settings.apiCMCKey
    Private Shared ReadOnly apiUrlHistorical As String = My.Settings.apiUrlHistorical
    Private Shared ReadOnly apiUrl As String = My.Settings.activeAPI

    Public Async Function CM_GetCriptoPrices(symbolORid As String) As Task(Of String)

        Try
            Dim requestUrl As String

            Using client As New HttpClient()
                client.DefaultRequestHeaders.Add("X-CMC_PRO_API_KEY", apiKey)
                client.DefaultRequestHeaders.Accept.Add(New MediaTypeWithQualityHeaderValue("application/json"))

                If IsNumeric(symbolORid) Then
                    requestUrl = $"{apiUrl}?id={symbolORid}"
                Else
                    requestUrl = $"{apiUrl}?symbol={symbolORid.ToUpper()}"
                End If

                Dim response As HttpResponseMessage = Await client.GetAsync(requestUrl)
                response.EnsureSuccessStatusCode()

                Dim responseBody As String = Await response.Content.ReadAsStringAsync()
                Dim json = JsonDocument.Parse(responseBody)
                Dim data = json.RootElement.GetProperty("data").GetProperty(symbolORid.ToUpper()).GetProperty("quote").GetProperty("USD")

                Dim preco As Decimal = data.GetProperty("price").GetDecimal()
                Dim marketcap As Decimal = data.GetProperty("market_cap").GetDecimal()

                Return $"{preco}|{marketcap.ToString("F2")}"

            End Using
        Catch e As HttpRequestException
            Debug.WriteLine($"[CMC] Erro ao chamar API para {symbolORid}: {e.Message}")
            FormMain.lbLoadFromMarket.Visible = False
            FormMain.TimerBlink.Stop()
            FormMain.Cursor = Cursors.Default
            FormMain.dgPortfolio.Cursor = Cursors.Default
            Return False
        Catch ex As Exception
            Debug.WriteLine($"[CMC] Erro ao processar {symbolORid}: {ex.Message}")
            FormMain.lbLoadFromMarket.Visible = False
            FormMain.TimerBlink.Stop()
            FormMain.Cursor = Cursors.Default
            FormMain.dgPortfolio.Cursor = Cursors.Default
            Return False
        End Try

    End Function

    Public Async Function CM_GetUSDBRL() As Task(Of Decimal)

        Try
            Dim requestUrl As String

            Using client As New HttpClient()
                client.DefaultRequestHeaders.Add("X-CMC_PRO_API_KEY", apiKey)
                client.DefaultRequestHeaders.Accept.Add(New MediaTypeWithQualityHeaderValue("application/json"))

                requestUrl = $"{apiUrl}?symbol=USDT&convert=BRL"

                Dim response As HttpResponseMessage = Await client.GetAsync(requestUrl)
                response.EnsureSuccessStatusCode()

                Dim responseBody As String = Await response.Content.ReadAsStringAsync()

                Dim json = JsonDocument.Parse(responseBody)
                Dim preco As Decimal = json.RootElement _
                        .GetProperty("data") _
                        .GetProperty("USDT") _
                        .GetProperty("quote") _
                        .GetProperty("BRL") _
                        .GetProperty("price") _
                        .GetDecimal()

                Return preco

            End Using
        Catch e As HttpRequestException
            Debug.WriteLine($"[CMC] Erro ao buscar USDT/BRL: {e.Message}")
            Return 0D
        Catch ex As Exception
            Debug.WriteLine($"[CMC] Erro ao processar USDT/BRL: {ex.Message}")
            Return 0D
        End Try

    End Function

    Public Async Function CM_GetBTCDOM() As Task(Of Decimal?)
        Try
            Dim requestUrl As String

            Using client As New HttpClient()
                client.DefaultRequestHeaders.Add("X-CMC_PRO_API_KEY", apiKey)
                client.DefaultRequestHeaders.Accept.Add(New MediaTypeWithQualityHeaderValue("application/json"))

                requestUrl = apiUrlHistorical

                Dim response As HttpResponseMessage = Await client.GetAsync(requestUrl)
                response.EnsureSuccessStatusCode()

                Dim responseBody As String = Await response.Content.ReadAsStringAsync()
                Dim json = JsonDocument.Parse(responseBody)

                Dim dataArray As JsonElement
                Dim firstElement As JsonElement
                Dim quoteElement As JsonElement
                Dim usdElement As JsonElement
                Dim dominanceElement As JsonElement

                If json.RootElement.TryGetProperty("data", dataArray) AndAlso dataArray.ValueKind = JsonValueKind.Array Then
                    If dataArray.GetArrayLength() > 0 Then
                        firstElement = dataArray(0)

                        If firstElement.TryGetProperty("quote", quoteElement) AndAlso
                           quoteElement.TryGetProperty("USD", usdElement) AndAlso
                           usdElement.TryGetProperty("market_cap_dominance", dominanceElement) Then

                            If dominanceElement.ValueKind = JsonValueKind.Number Then
                                Return dominanceElement.GetDecimal()
                            End If

                            Debug.WriteLine("[CMC] market_cap_dominance não é numérico.")
                            Return Nothing
                        End If

                        Debug.WriteLine("[CMC] Estrutura de dominância inesperada: quote/USD ausentes.")
                        Return Nothing
                    End If

                    Debug.WriteLine("[CMC] Array data vazio ao consultar dominância BTC.")
                    Return Nothing
                End If

                Debug.WriteLine("[CMC] Estrutura inesperada ao consultar dominância BTC.")
                Return Nothing
            End Using

        Catch e As HttpRequestException
            Debug.WriteLine($"[CMC] Erro HTTP ao consultar dominância BTC: {e.Message}")
            Return Nothing
        Catch ex As Exception
            Debug.WriteLine($"[CMC] Erro ao processar dominância BTC: {ex.Message}")
            Return Nothing
        End Try

    End Function

End Class
