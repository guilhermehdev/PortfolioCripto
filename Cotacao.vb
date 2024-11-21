
Imports System.Net.Http
Imports System.Net.Http.Headers
Imports System.Text.Json

Public Class Cotacao
    Private Shared ReadOnly apiKey As String = "803eb0bd-7743-468d-869a-f5b4914e4f29"
    Private Shared ReadOnly apiUrl As String = "https://pro-api.coinmarketcap.com/v1/cryptocurrency/quotes/latest"

    Public Async Function GetCriptoPrices(simbolosCripto As String, Optional convertUSDBRL As Boolean = False) As Task(Of String)
        Dim brl As String

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

End Class
