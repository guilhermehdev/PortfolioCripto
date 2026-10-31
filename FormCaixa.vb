Public Class FormCaixa
    Private ReadOnly _json As New JSON
    Private ReadOnly _binance As New Binance
    Private ReadOnly _gate As New Gateio
    Private _loading As Boolean

    Private NotInheritable Class CashRow
        Public Property Symbol As String
        Public Property Quantity As Decimal
        Public Property Exchange As String
    End Class
    Private Async Sub FormCaixa_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Await RefreshCashAsync()
    End Sub

    Private Async Sub FormCaixa_Shown(sender As Object, e As EventArgs) Handles MyBase.Shown
        ' Recarrega também quando a mesma janela é aberta novamente após uma
        ' atualização do WebSocket Spot.
        Await RefreshCashAsync()
    End Sub

    Private Async Function RefreshCashAsync() As Task
        If _loading Then Return
        _loading = True

        Try
            Dim rows As New List(Of CashRow)()

            If Not String.IsNullOrWhiteSpace(ExchangeCredentials.BinanceAPIKey) AndAlso
               Not String.IsNullOrWhiteSpace(ExchangeCredentials.BinanceSecretKey) Then
                Try
                    AppendStablecoinRows(rows, Await _binance.BINANCE_GetSpotAssetsFull(), "BINANCE")
                Catch ex As Exception
                    System.Diagnostics.Debug.WriteLine("Erro carregando Caixa Spot Binance: " & ex.Message)
                End Try
            End If

            If Not String.IsNullOrWhiteSpace(ExchangeCredentials.GateAPIKey) AndAlso
               Not String.IsNullOrWhiteSpace(ExchangeCredentials.GateSecretKey) Then
                Try
                    AppendStablecoinRows(rows, Await _gate.GATE_GetAllSpotAssets(), "GATE.IO")
                Catch ex As Exception
                    System.Diagnostics.Debug.WriteLine("Erro carregando Caixa Spot Gate: " & ex.Message)
                End Try
            End If

            If rows.Count > 0 Then
                RenderCashRows(rows)
                NotifyMainCashTotal(rows)
            Else
                ' Compatibilidade para abertura sem credenciais ou antes da
                ' primeira leitura das contas das exchanges.
                _json.loadCaixa(dgCaixa)
                NormalizeGridSelection()
            End If
        Finally
            _loading = False
        End Try
    End Function

    Private Sub NotifyMainCashTotal(rows As IReadOnlyList(Of CashRow))
        If rows Is Nothing Then Return

        Dim totalUsd = rows.Sum(Function(item) item.Quantity)
        If My.Forms.FormMain IsNot Nothing Then
            My.Forms.FormMain.ApplySpotCashFromPopup(totalUsd)
        End If
    End Sub

    Private Sub AppendStablecoinRows(
        rows As List(Of CashRow),
        assets As Dictionary(Of String, Decimal),
        exchange As String)

        If assets Is Nothing Then Return

        For Each asset In assets.OrderBy(Function(item) item.Key)
            Dim symbol = asset.Key.Trim().ToUpperInvariant()
            If _json.stablecoins.Contains(symbol) AndAlso asset.Value <> 0D Then
                rows.Add(New CashRow With {
                    .Symbol = symbol,
                    .Quantity = asset.Value,
                    .Exchange = exchange
                })
            End If
        Next
    End Sub

    Private Sub RenderCashRows(rows As IReadOnlyList(Of CashRow))
        dgCaixa.Rows.Clear()
        dgCaixa.Columns.Clear()

        dgCaixa.Columns.Add("Symbol", "Cripto")
        dgCaixa.Columns.Add("Qtd", "Qtd")
        dgCaixa.Columns.Add("Wallet", "Wallet/Cex")

        For Each item In rows
            dgCaixa.Rows.Add(
                item.Symbol,
                item.Quantity.ToString("0.########", Globalization.CultureInfo.CurrentCulture),
                item.Exchange)
        Next

        dgCaixa.Columns("Symbol").Width = 55
        dgCaixa.Columns("Qtd").Width = 105
        dgCaixa.Columns("Wallet").Width = 105
        dgCaixa.ClearSelection()
        NormalizeGridSelection()
    End Sub

    Private Sub NormalizeGridSelection()
        Dim backColor = If(dgCaixa.DefaultCellStyle.BackColor.IsEmpty, dgCaixa.BackgroundColor, dgCaixa.DefaultCellStyle.BackColor)
        Dim foreColor = If(dgCaixa.DefaultCellStyle.ForeColor.IsEmpty, Color.White, dgCaixa.DefaultCellStyle.ForeColor)
        dgCaixa.DefaultCellStyle.SelectionBackColor = backColor
        dgCaixa.DefaultCellStyle.SelectionForeColor = foreColor
        dgCaixa.RowTemplate.DefaultCellStyle.SelectionBackColor = backColor
        dgCaixa.RowTemplate.DefaultCellStyle.SelectionForeColor = foreColor

        For Each column As DataGridViewColumn In dgCaixa.Columns
            column.DefaultCellStyle.SelectionBackColor = If(column.DefaultCellStyle.BackColor.IsEmpty, backColor, column.DefaultCellStyle.BackColor)
            column.DefaultCellStyle.SelectionForeColor = If(column.DefaultCellStyle.ForeColor.IsEmpty, foreColor, column.DefaultCellStyle.ForeColor)
        Next

        For Each row As DataGridViewRow In dgCaixa.Rows
            If row.IsNewRow Then Continue For
            For Each cell As DataGridViewCell In row.Cells
                Dim inherited = cell.InheritedStyle
                cell.Style.SelectionBackColor = inherited.BackColor
                cell.Style.SelectionForeColor = inherited.ForeColor
            Next
        Next
    End Sub

    Private Sub btCancelTimer_Click(sender As Object, e As EventArgs) Handles btCancelTimer.Click
        Me.Close()
    End Sub

End Class
