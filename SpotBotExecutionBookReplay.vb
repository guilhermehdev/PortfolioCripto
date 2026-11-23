Imports System.Globalization
Imports System.IO
Imports System.Text
Imports Microsoft.VisualBasic.FileIO

Friend NotInheritable Class SpotBotExecutionBookReplay
    Friend Shared ReadOnly MaximumBookAge As TimeSpan = TimeSpan.FromSeconds(5)
    Private ReadOnly _books As New Dictionary(Of String, List(Of SpotBotExecutionSnapshot))(StringComparer.OrdinalIgnoreCase)

    Friend Shared Function Load(filePath As String) As SpotBotExecutionBookReplay
        Dim replay As New SpotBotExecutionBookReplay()
        Using parser As New TextFieldParser(filePath, Encoding.UTF8)
            parser.SetDelimiters(";")
            Dim headers = parser.ReadFields()
            Dim required = {"Symbol", "CapturedAtUtc", "BidPrice", "AskPrice", "BidDepthQuote", "AskDepthQuote"}
            If headers Is Nothing OrElse Not headers.SequenceEqual(required, StringComparer.OrdinalIgnoreCase) Then
                Throw New InvalidDataException("Selecione um CSV de livro exportado pelo TradeBot.")
            End If

            While Not parser.EndOfData
                Dim line = parser.LineNumber
                Dim row = parser.ReadFields()
                Dim capturedAt As DateTime
                Dim bid, ask, bidDepth, askDepth As Decimal
                If row Is Nothing OrElse row.Length <> required.Length OrElse String.IsNullOrWhiteSpace(row(0)) OrElse
                   Not DateTime.TryParse(row(1), CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal Or DateTimeStyles.AdjustToUniversal, capturedAt) OrElse
                   Not Decimal.TryParse(row(2), NumberStyles.Float, CultureInfo.InvariantCulture, bid) OrElse
                   Not Decimal.TryParse(row(3), NumberStyles.Float, CultureInfo.InvariantCulture, ask) OrElse
                   Not Decimal.TryParse(row(4), NumberStyles.Float, CultureInfo.InvariantCulture, bidDepth) OrElse
                   Not Decimal.TryParse(row(5), NumberStyles.Float, CultureInfo.InvariantCulture, askDepth) OrElse
                   bid <= 0D OrElse ask < bid OrElse bidDepth <= 0D OrElse askDepth <= 0D Then
                    Throw New InvalidDataException($"Snapshot de livro inválido na linha {line}.")
                End If

                Dim symbol = NormalizeSymbol(row(0))
                Dim snapshots As List(Of SpotBotExecutionSnapshot) = Nothing
                If Not replay._books.TryGetValue(symbol, snapshots) Then
                    snapshots = New List(Of SpotBotExecutionSnapshot)()
                    replay._books.Add(symbol, snapshots)
                End If
                snapshots.Add(New SpotBotExecutionSnapshot With {
                    .CapturedAtUtc = capturedAt, .BidPrice = bid, .AskPrice = ask,
                    .BidDepthQuote = bidDepth, .AskDepthQuote = askDepth})
            End While
        End Using
        For Each snapshots In replay._books.Values
            snapshots.Sort(Function(left, right) left.CapturedAtUtc.CompareTo(right.CapturedAtUtc))
        Next
        Return replay
    End Function

    Friend Function FindSnapshot(symbol As String, timestamp As DateTime) As SpotBotExecutionSnapshot
        Dim snapshots As List(Of SpotBotExecutionSnapshot) = Nothing
        If Not _books.TryGetValue(NormalizeSymbol(symbol), snapshots) Then Return Nothing

        Dim low = 0
        Dim high = snapshots.Count
        While low < high
            Dim middle = low + (high - low) \ 2
            If snapshots(middle).CapturedAtUtc <= timestamp Then
                low = middle + 1
            Else
                high = middle
            End If
        End While
        If low = 0 Then Return Nothing
        Dim snapshot = snapshots(low - 1)
        Return If(timestamp - snapshot.CapturedAtUtc <= MaximumBookAge, snapshot, Nothing)
    End Function

    Private Shared Function NormalizeSymbol(symbol As String) As String
        Dim normalized = symbol.Trim()
        If normalized.EndsWith("USDT", StringComparison.OrdinalIgnoreCase) AndAlso normalized.Length > 4 Then
            Return normalized.Substring(0, normalized.Length - 4)
        End If
        Return normalized
    End Function
End Class
