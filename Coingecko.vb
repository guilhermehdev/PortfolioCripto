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

    Private Shared PreferredIds As New Dictionary(Of String, String) From {
        {"XRP", "ripple"},
        {"SOL", "solana"},
        {"USDT", "tether"},
        {"VELODROME", "velodrome-finance"},
        {"BTC", "bitcoin"}
    }

    Public Async Function CGECKO_MarketData(symbols As IEnumerable(Of String)) _
        As Task(Of Dictionary(Of String, CoinMarketData))

        Dim symList = symbols.Select(Function(s) s.Trim().ToUpper()).Distinct().ToList()

        Using cli As New HttpClient()
            cli.DefaultRequestHeaders.Add("User-Agent", "VBApp/1.0")

            ' Cache para ID lookup
            If symToIdCache Is Nothing Then
                Dim listJson = Await cli.GetStringAsync("https://api.coingecko.com/api/v3/coins/list?include_platform=false")
                Dim coinList = JArray.Parse(listJson)

                symToIdCache = coinList.
                    GroupBy(Function(c) c("symbol").ToString().ToUpper()).
                    ToDictionary(
                        Function(g) g.Key,
                        Function(g) g.First()("id").ToString(),
                        StringComparer.OrdinalIgnoreCase
                    )
            End If

            ' Gera lista de CoinGecko IDs válidos
            Dim ids = symList.
                Where(Function(s) PreferredIds.ContainsKey(s) OrElse symToIdCache.ContainsKey(s)).
                Select(Function(s) If(PreferredIds.ContainsKey(s), PreferredIds(s), symToIdCache(s))).
                Distinct().
                ToList()

            If ids.Count = 0 Then Return New Dictionary(Of String, CoinMarketData)

            ' Faz chamada em lote
            Dim idsParam = String.Join(",", ids)
            Dim url = $"https://api.coingecko.com/api/v3/coins/markets?vs_currency=usd&ids={idsParam}&price_change_percentage=24h"
            Dim marketJson = JArray.Parse(Await cli.GetStringAsync(url))

            ' Monta dicionário symbol → CoinMarketData
            Dim result As New Dictionary(Of String, CoinMarketData)(StringComparer.OrdinalIgnoreCase)

            For Each coin In marketJson
                Dim symbol = coin("symbol").ToString().ToUpper()
                Dim data As New CoinMarketData With {
                    .Price = coin("current_price").Value(Of Decimal)(),
                    .MarketCap = coin("market_cap").Value(Of Decimal)(),
                    .Volume24h = coin("total_volume").Value(Of Decimal)(),
                    .Change24h = coin("price_change_percentage_24h").Value(Of Decimal)()
                }
                result(symbol) = data
            Next

            ' Corrige símbolos que vieram via PreferredIds (como USDT, VELO)
            For Each kvp In PreferredIds
                Dim symbol = kvp.Key.ToUpper()
                Dim id = kvp.Value
                If Not result.ContainsKey(symbol) Then
                    Dim coin = marketJson.FirstOrDefault(Function(j) j("id").ToString().Equals(id, StringComparison.OrdinalIgnoreCase))
                    If coin IsNot Nothing Then
                        result(symbol) = New CoinMarketData With {
                            .Price = coin("current_price").Value(Of Decimal)(),
                            .MarketCap = coin("market_cap").Value(Of Decimal)(),
                            .Volume24h = coin("total_volume").Value(Of Decimal)(),
                            .Change24h = coin("price_change_percentage_24h").Value(Of Decimal)()
                        }
                    End If
                End If
            Next

            Return result
        End Using

    End Function

    Public Async Function CGECKO_GetPrice(symbol As String, Optional currency As String = "usd") As Task(Of Decimal)
        Dim idMap As New Dictionary(Of String, String)(StringComparer.OrdinalIgnoreCase) From {
            {"TOMI", "tominet"},
            {"BTC", "bitcoin"},
            {"ETH", "ethereum"},
            {"USDT", "tether"},
            {"VELO", "velodrome-finance"},
            {"SOL", "solana"},
            {"USD", "usd"}
        }

        If Not idMap.ContainsKey(symbol) Then Return 0D

        Dim id = idMap(symbol)
        Dim url = $"https://api.coingecko.com/api/v3/simple/price?ids={id}&vs_currencies={currency}"

        Using client As New HttpClient()
            client.DefaultRequestHeaders.Add("User-Agent", "VBApp/1.0")
            Dim json = JObject.Parse(Await client.GetStringAsync(url))
            Return json(id)(currency).Value(Of Decimal)()
        End Using
    End Function


End Class

Public Class CoinMarketData
    Public Property Price As Decimal
    Public Property MarketCap As Decimal
    Public Property Volume24h As Decimal
    Public Property Change24h As Decimal
End Class

