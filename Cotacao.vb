
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

                'requestUrl = $"{apiUrl}?symbol={symbolORid.ToUpper()}"

                Dim response As HttpResponseMessage = Await client.GetAsync(requestUrl)
                response.EnsureSuccessStatusCode()

                Dim responseBody As String = Await response.Content.ReadAsStringAsync()

                ' Processa o JSON para extrair o preço da criptomoeda
                Dim json = JsonDocument.Parse(responseBody)
                Dim data = json.RootElement.GetProperty("data").GetProperty(symbolORid.ToUpper()).GetProperty("quote").GetProperty("USD")

                Dim preco As Decimal = data.GetProperty("price").GetDecimal()
                Dim marketcap As Decimal = data.GetProperty("market_cap").GetDecimal()

                Return $"{preco}|{marketcap.ToString("F2")}"

            End Using
        Catch e As HttpRequestException
            Debug.Write($"Erro ao chamar a API do CoinMarketCap.{vbCrLf & vbCrLf & e.Message}")
            FormMain.lbLoadFromMarket.Visible = False
            FormMain.TimerBlink.Stop()
            FormMain.Cursor = Cursors.Default
            FormMain.dgPortfolio.Cursor = Cursors.Default
            Return False
            Exit Function
        Catch ex As Exception
            Debug.Write($"Erro ao processar a resposta: Verifique o simbolo, talvez {symbolORid} não esteja correto. " & ex.Message)
            FormMain.lbLoadFromMarket.Visible = False
            FormMain.TimerBlink.Stop()
            FormMain.Cursor = Cursors.Default
            FormMain.dgPortfolio.Cursor = Cursors.Default
            Return False
            Exit Function
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

                ' Processa o JSON para extrair o preço da criptomoeda
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
            Debug.Write($"Erro ao chamar a API! Aguarde um momento e tente novamente.{vbCrLf & vbCrLf & e.Message}")
            Return 0
        Catch ex As Exception
            Debug.Write($"Erro ao chamar a API! Aguarde um momento e tente novamente.{vbCrLf & vbCrLf & ex.Message}")
            Return 0
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

                ' Exibe o JSON retornado para depuração
                ' MsgBox(responseBody)

                ' Processa o JSON
                Dim json = JsonDocument.Parse(responseBody)

                ' Declarar variáveis para armazenar partes do JSON
                Dim dataArray As JsonElement
                Dim firstElement As JsonElement
                Dim quoteElement As JsonElement
                Dim usdElement As JsonElement
                Dim dominanceElement As JsonElement

                ' Acesse o array "data"
                If json.RootElement.TryGetProperty("data", dataArray) AndAlso dataArray.ValueKind = JsonValueKind.Array Then
                    ' Verifique se o array tem pelo menos um elemento
                    If dataArray.GetArrayLength() > 0 Then
                        firstElement = dataArray(0)

                        ' Acesse as propriedades internas do primeiro elemento
                        If firstElement.TryGetProperty("quote", quoteElement) AndAlso
                           quoteElement.TryGetProperty("USD", usdElement) AndAlso
                           usdElement.TryGetProperty("market_cap_dominance", dominanceElement) Then

                            ' Certifique-se de que o valor é numérico antes de converter
                            If dominanceElement.ValueKind = JsonValueKind.Number Then
                                Return dominanceElement.GetDecimal()
                            Else
                                Throw New InvalidCastException("O valor de 'market_cap_dominance' não é numérico.")
                            End If
                        Else
                            Throw New KeyNotFoundException("Estrutura do JSON inesperada: propriedades 'quote' ou 'USD' ausentes.")
                        End If
                    Else
                        Throw New KeyNotFoundException("O array 'data' está vazio.")
                    End If
                Else
                    Throw New KeyNotFoundException("Estrutura do JSON inesperada: 'data' não é um array.")
                End If
            End Using

        Catch e As HttpRequestException
            MessageBox.Show($"Erro ao chamar a API! Aguarde um momento e tente novamente.{vbCrLf & vbCrLf & e.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error)
            'Application.Exit()
            Return False
        Catch ex As Exception
            MessageBox.Show($"Erro ao chamar a API! Aguarde um momento e tente novamente.{vbCrLf & vbCrLf & ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error)
            ' Application.Exit()
            Return False
        End Try

        ' Retorna Nothing em caso de falha
        Return Nothing
    End Function

    Public Function decimalBR(valor As String)
        Return Decimal.Parse(valor, CultureInfo.InvariantCulture)
    End Function

End Class

