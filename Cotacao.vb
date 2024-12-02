
Imports System.Net.Http
Imports System.Net.Http.Headers
Imports System.Text.Json

Public Class Cotacao
    Private Shared ReadOnly apiKey As String = "803eb0bd-7743-468d-869a-f5b4914e4f29"
    Private Shared ReadOnly apiUrlHistorical As String = "https://pro-api.coinmarketcap.com/v1/cryptocurrency/listings/latest"

    Private Shared ReadOnly apiUrl As String = "https://pro-api.coinmarketcap.com/v1/cryptocurrency/quotes/latest"
    'Private Shared ReadOnly apiUrl As String = "https://sandbox-api.coinmarketcap.com/v1/cryptocurrency/quotes/latest"

    Public Async Function GetCriptoPrices(simbolosCripto As String) As Task(Of String)

        Try
            Dim requestUrl As String

            Using client As New HttpClient()
                client.DefaultRequestHeaders.Add("X-CMC_PRO_API_KEY", apiKey)
                client.DefaultRequestHeaders.Accept.Add(New MediaTypeWithQualityHeaderValue("application/json"))

                requestUrl = $"{apiUrl}?symbol={simbolosCripto.ToUpper()}"

                Dim response As HttpResponseMessage = Await client.GetAsync(requestUrl)
                response.EnsureSuccessStatusCode()

                Dim responseBody As String = Await response.Content.ReadAsStringAsync()

                ' Processa o JSON para extrair o preço da criptomoeda
                Dim json = JsonDocument.Parse(responseBody)
                Dim preco As Decimal = json.RootElement _
                        .GetProperty("data") _
                        .GetProperty(simbolosCripto.ToUpper()) _
                        .GetProperty("quote") _
                        .GetProperty("USD") _
                        .GetProperty("price") _
                        .GetDecimal()

                Return preco

            End Using
        Catch e As HttpRequestException
            Return "Erro ao chamar a API: " & e.Message
        Catch ex As Exception
            Return "Erro ao processar a resposta: " & ex.Message
        End Try
    End Function

    Public Async Function GetUSDBRL() As Task(Of Decimal)

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
            Return "Erro ao chamar a API: " & e.Message
        Catch ex As Exception
            Return "Erro ao processar a resposta: " & ex.Message
        End Try
    End Function

    Public Async Function GetBTCDOM() As Task(Of Decimal?)
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
            WriteLine($"Erro de requisição: {e.Message}")
        Catch ex As Exception
            WriteLine($"Erro: {ex.Message}")
        End Try

        ' Retorna Nothing em caso de falha
        Return Nothing
    End Function


End Class
Public Class TransparentPictureBox
    Inherits PictureBox

    Protected Overrides Sub OnPaintBackground(pevent As PaintEventArgs)
        ' Não pinta o fundo, preservando a transparência
    End Sub
End Class
