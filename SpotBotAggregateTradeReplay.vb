Imports System.Globalization
Imports System.IO
Imports System.Net.Http
Imports Newtonsoft.Json.Linq

' Uma instância por comparação, compartilhada pelos cenários e janelas.
Friend NotInheritable Class SpotBotAggregateTradeReplay
    Private Shared ReadOnly Client As New HttpClient With {.Timeout = TimeSpan.FromSeconds(30)}
    Private ReadOnly _minutes As New Dictionary(Of (String, DateTime), IReadOnlyList(Of SpotBotPricePoint))

    Friend Function ReadMinute(symbol As String, startUtc As DateTime) As IReadOnlyList(Of SpotBotPricePoint)
        Dim pair = symbol.ToUpperInvariant()
        If Not pair.EndsWith("USDT", StringComparison.Ordinal) Then pair &= "USDT"
        Dim key = (pair, startUtc)
        Dim cached As IReadOnlyList(Of SpotBotPricePoint) = Nothing
        If _minutes.TryGetValue(key, cached) Then Return cached
        Dim startMs = New DateTimeOffset(startUtc).ToUnixTimeMilliseconds()
        Dim endMs = startMs + 59999
        Dim query = $"startTime={startMs}&endTime={endMs}"
        Dim trades As New List(Of SpotBotPricePoint)()
        Dim lastId As Long = -1
        Do
            Dim url = $"https://data-api.binance.vision/api/v3/aggTrades?symbol={Uri.EscapeDataString(pair)}&limit=1000&{query}"
            Dim page = JArray.Parse(Client.GetStringAsync(url).GetAwaiter().GetResult())
            For Each item In page
                Dim ms = CLng(item("T"))
                If ms > endMs Then Exit For
                Dim id = CLng(item("a"))
                If ms < startMs OrElse id <= lastId Then Throw New InvalidDataException($"Sequência aggTrades inválida em {pair}.")
                lastId = id
                trades.Add(New SpotBotPricePoint(DateTimeOffset.FromUnixTimeMilliseconds(ms).UtcDateTime,
                    Decimal.Parse(CStr(item("p")), CultureInfo.InvariantCulture),
                    volume:=Decimal.Parse(CStr(item("q")), CultureInfo.InvariantCulture)))
            Next
            If page.Count < 1000 OrElse CLng(page.Last("T")) > endMs Then Exit Do
            query = $"fromId={lastId + 1}"
        Loop
        _minutes.Add(key, trades)
        Return trades
    End Function
End Class
