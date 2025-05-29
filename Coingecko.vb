Imports System.Net.Http
Imports Newtonsoft.Json.Linq

Public Class Coingecko
    Private Shared symToIdCache As Dictionary(Of String, String) = Nothing

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

            If Not mcapDic.ContainsKey("USDT") AndAlso marketJson.Any(Function(j) j("id").ToString() = "tether") Then
                mcapDic("USDT") = marketJson.First(Function(j) j("id").ToString() = "tether")("market_cap").Value(Of Decimal)()
            End If

            If Not mcapDic.ContainsKey("VELO") AndAlso marketJson.Any(Function(j) j("id").ToString() = "velodrome-finance") Then
                mcapDic("VELO") = marketJson.First(Function(j) j("id").ToString() = "velodrome-finance")("market_cap").Value(Of Decimal)()
            End If

            Return mcapDic
        End Using

    End Function

    Public Async Function GetMarketCapsFromCoinGecko(symbols As IEnumerable(Of String)) _
        As Task(Of Dictionary(Of String, Decimal))

        Dim symList = symbols.Select(Function(s) s.Trim().ToUpper()).Distinct().ToList()

        Using cli As New HttpClient()
            cli.DefaultRequestHeaders.Add("User-Agent", "VBApp/1.0")

            ' Cache para evitar repetição de chamada


            If symToIdCache Is Nothing Then
                Dim listJson = Await cli.GetStringAsync("https://api.coingecko.com/api/v3/coins/list?include_platform=false")
                Dim coinList = JArray.Parse(listJson)

                symToIdCache = coinList.
                GroupBy(Function(c) c("symbol").ToString().ToUpper()).
                ToDictionary(
                    Function(g) g.Key,
                    Function(g) g.First()("id").ToString()
                )
            End If

            ' Mapeia os símbolos para os ids válidos
            Dim ids = symList.
                  Where(Function(s) symToIdCache.ContainsKey(s)).
                  Select(Function(s) symToIdCache(s)).
                  ToList()

            ' Inclui manualmente os IDs de moedas problemáticas
            If Not ids.Contains("tether") AndAlso symList.Contains("USDT") Then
                ids.Add("tether")
            End If

            If Not ids.Contains("velodrome-finance") AndAlso symList.Contains("VELO") Then
                ids.Add("velodrome-finance")
            End If

            ' Se não há ids válidos, retorna vazio
            If ids.Count = 0 Then Return New Dictionary(Of String, Decimal)

            ' Monta a requisição
            Dim idsParam = String.Join(",", ids)
            Dim url = $"https://api.coingecko.com/api/v3/coins/markets?vs_currency=usd&ids={idsParam}"
            Dim marketJson = JArray.Parse(Await cli.GetStringAsync(url))

            Dim mcapDic As New Dictionary(Of String, Decimal)

            For Each coin In marketJson
                Dim id = coin("id").ToString()
                Dim symbol = coin("symbol").ToString().ToUpper()
                Dim marketcap = coin("market_cap").Value(Of Decimal)()
                mcapDic(symbol) = marketcap
            Next

            ' Substituições manuais
            If Not mcapDic.ContainsKey("USDT") AndAlso marketJson.Any(Function(j) j("id").ToString() = "tether") Then
                mcapDic("USDT") = marketJson.First(Function(j) j("id").ToString() = "tether")("market_cap").Value(Of Decimal)()
            End If

            If Not mcapDic.ContainsKey("VELO") AndAlso marketJson.Any(Function(j) j("id").ToString() = "velodrome-finance") Then
                mcapDic("VELO") = marketJson.First(Function(j) j("id").ToString() = "velodrome-finance")("market_cap").Value(Of Decimal)()
            End If

            Return mcapDic
        End Using
    End Function


    Public Async Function CGECKO_GetMarketCapById(id As String) As Task(Of Decimal)
        Using cli As New HttpClient()
            cli.DefaultRequestHeaders.Add("User-Agent", "VBApp/1.0")
            Dim url = $"https://api.coingecko.com/api/v3/coins/markets?vs_currency=usd&ids={id}"
            Dim json = JArray.Parse(Await cli.GetStringAsync(url))
            If json.Count = 0 Then Return 0D
            Return json(0)("market_cap").Value(Of Decimal)()
        End Using
    End Function


End Class
