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

    Public Async Function CGECKO_MCaps(symbols As IEnumerable(Of String)) _
        As Task(Of Dictionary(Of String, Decimal))

        Dim symList = symbols.Select(Function(s) s.Trim().ToUpper()).Distinct().ToList()

        ' 1. Pega lista global de moedas da CG (símbolo → id)
        Using cli As New HttpClient()
            cli.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (VB.NET App)")
            Dim listJson = Await cli.GetStringAsync("https://api.coingecko.com/api/v3/coins/list?include_platform=false")
            Dim coinList = JArray.Parse(listJson)

            Dim symToId = coinList.
            GroupBy(Function(c) c("symbol").ToString().ToUpper()).
            ToDictionary(
                Function(g) g.Key,
                Function(g) g.First()("id").ToString()
            )

            ' 2. Converte símbolos em IDs válidos
            Dim ids = symList.
          Where(Function(s) symToId.ContainsKey(s)).
          Select(Function(s) symToId(s)).
          ToList()

            If ids.Count = 0 Then Return New Dictionary(Of String, Decimal)

            ' 3. Chama /coins/markets em lote
            Dim idsParam = String.Join(",", ids)
            Dim url = $"https://api.coingecko.com/api/v3/coins/markets?vs_currency=usd&ids={idsParam}"

            Dim marketJson = JArray.Parse(Await cli.GetStringAsync(url))

            ' 4. Monta dicionário symbol → marketcap
            Dim mcapDic As New Dictionary(Of String, Decimal)

            For Each coin In marketJson
                Dim id = coin("id").ToString()
                Dim symbol = coin("symbol").ToString().ToUpper()
                Dim mcap = coin("market_cap").Value(Of Decimal)()
                mcapDic(symbol) = mcap
            Next

            Return mcapDic
        End Using

    End Function

End Class
