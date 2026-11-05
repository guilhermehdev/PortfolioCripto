Imports System.Data
Imports System.Drawing
Imports System.Globalization
Imports System.Windows.Forms

Public Class SpotTradeBotTradesForm
    Inherits Form

    Private ReadOnly _runId As Long
    Private ReadOnly _grid As DataGridView
    Private ReadOnly _summary As Label
    Private ReadOnly _refresh As Button

    Public Sub New(runId As Long)
        _runId = runId

        Text = $"Trades da run #{runId}"
        StartPosition = FormStartPosition.CenterParent
        MinimumSize = New Size(900, 420)
        Size = New Size(1180, 620)
        BackColor = Color.FromArgb(30, 30, 30)
        ForeColor = Color.White
        Try
            Icon = FormMain.Icon
        Catch
        End Try

        Dim header As New Panel With {
            .Dock = DockStyle.Top,
            .Height = 42,
            .BackColor = Color.FromArgb(30, 30, 30),
            .Padding = New Padding(8, 8, 8, 6)}

        _summary = New Label With {
            .AutoSize = False,
            .Dock = DockStyle.Fill,
            .ForeColor = Color.Gold,
            .TextAlign = ContentAlignment.MiddleLeft,
            .Text = "Carregando trades..."}
        header.Controls.Add(_summary)

        _refresh = New Button With {
            .Dock = DockStyle.Right,
            .FlatStyle = FlatStyle.Flat,
            .BackColor = Color.FromArgb(55, 75, 105),
            .ForeColor = Color.White,
            .Text = "Atualizar",
            .Width = 90,
            .Cursor = Cursors.Hand}
        _refresh.FlatAppearance.BorderColor = Color.Gray
        AddHandler _refresh.Click, AddressOf Refresh_Click
        header.Controls.Add(_refresh)

        _grid = New DataGridView With {
            .AllowUserToAddRows = False,
            .AllowUserToDeleteRows = False,
            .AllowUserToOrderColumns = True,
            .AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            .BackgroundColor = Color.FromArgb(64, 64, 64),
            .BorderStyle = BorderStyle.FixedSingle,
            .CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
            .ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None,
            .Dock = DockStyle.Fill,
            .EnableHeadersVisualStyles = False,
            .MultiSelect = False,
            .ReadOnly = True,
            .RowHeadersVisible = False,
            .SelectionMode = DataGridViewSelectionMode.FullRowSelect}
        _grid.ColumnHeadersDefaultCellStyle = New DataGridViewCellStyle With {
            .BackColor = Color.FromArgb(40, 40, 40),
            .ForeColor = Color.Turquoise,
            .Font = New Font("Calibri", 10.0F, FontStyle.Italic),
            .SelectionBackColor = Color.FromArgb(40, 40, 40),
            .SelectionForeColor = Color.Turquoise,
            .WrapMode = DataGridViewTriState.True}
        _grid.DefaultCellStyle = New DataGridViewCellStyle With {
            .BackColor = Color.FromArgb(64, 64, 64),
            .ForeColor = Color.White,
            .Font = New Font("Calibri", 10.0F),
            .SelectionBackColor = Color.FromArgb(64, 64, 64),
            .SelectionForeColor = Color.White}
        _grid.RowTemplate.Height = 28
        AddHandler _grid.CellFormatting, AddressOf Grid_CellFormatting

        Controls.Add(_grid)
        Controls.Add(header)
        AddHandler Load, AddressOf Form_Load
    End Sub

    Private Sub Form_Load(sender As Object, e As EventArgs)
        LoadTrades()
    End Sub

    Private Sub Refresh_Click(sender As Object, e As EventArgs)
        LoadTrades()
    End Sub

    Private Sub LoadTrades()
        Try
            Dim table = SpotTradeBotRepository.GetTrades(_runId)
            _grid.DataSource = Nothing
            _grid.DataSource = table
            ConfigureColumns()

            Dim tradeCount = If(table Is Nothing, 0, table.Rows.Count)
            Dim totalPnl = If(
                table Is Nothing,
                0D,
                table.AsEnumerable().Sum(Function(row) DecimalValue(row, "Pnl")))
            Dim wins = If(
                table Is Nothing,
                0,
                table.AsEnumerable().Count(Function(row) DecimalValue(row, "Pnl") > 0D))
            Dim winRate = If(tradeCount = 0, 0D, wins * 100D / tradeCount)
            _summary.Text =
                $"Run #{_runId} | Trades fechados: {tradeCount} | Acertos: {wins} ({winRate:0.##}%) | P/L: {totalPnl:+0.00;-0.00;0.00} USDT"
        Catch ex As Exception
            _summary.Text = "Erro ao carregar os trades."
            MessageBox.Show(
                "Erro ao carregar os trades da run: " & ex.Message,
                "Trades da run",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub ConfigureColumns()
        Dim headers As New Dictionary(Of String, String) From {
            {"Id", "ID"},
            {"RunId", "Run"},
            {"Symbol", "Cripto"},
            {"Strategy", "Estratégia"},
            {"ExitReason", "Saída"},
            {"Quantity", "Quantidade"},
            {"EntryPrice", "Entrada"},
            {"ExitPrice", "Saída preço"},
            {"CostBasis", "Custo"},
            {"GrossProceeds", "Bruto"},
            {"Fees", "Taxas"},
            {"Pnl", "P/L"},
            {"SupportPrice", "Suporte"},
            {"InitialStopPrice", "Stop inicial"},
            {"ExitStopPrice", "Stop saída"},
            {"OpenedAtUtc", "Abertura"},
            {"ClosedAtUtc", "Fechamento"}}

        For Each item In headers
            If _grid.Columns.Contains(item.Key) Then
                _grid.Columns(item.Key).HeaderText = item.Value
            End If
        Next

        For Each hiddenColumn In New String() {"Id", "RunId"}
            If _grid.Columns.Contains(hiddenColumn) Then
                _grid.Columns(hiddenColumn).Visible = False
            End If
        Next

        For Each decimalColumn In New String() {
            "Quantity", "EntryPrice", "ExitPrice", "CostBasis", "GrossProceeds", "Fees", "Pnl", "SupportPrice", "InitialStopPrice", "ExitStopPrice"}
            If _grid.Columns.Contains(decimalColumn) Then
                _grid.Columns(decimalColumn).DefaultCellStyle.Format = "0.########"
                _grid.Columns(decimalColumn).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            End If
        Next
    End Sub

    Private Sub Grid_CellFormatting(sender As Object, e As DataGridViewCellFormattingEventArgs)
        If e.RowIndex < 0 OrElse Not _grid.Columns.Contains("Pnl") Then Return
        Dim pnl As Decimal
        If Decimal.TryParse(
               Convert.ToString(_grid.Rows(e.RowIndex).Cells("Pnl").Value, CultureInfo.InvariantCulture),
               NumberStyles.Any,
               CultureInfo.InvariantCulture,
               pnl) Then
            Dim pnlColor = If(pnl > 0D, Color.LimeGreen, If(pnl < 0D, Color.Red, Color.White))
            _grid.Rows(e.RowIndex).Cells("Pnl").Style.ForeColor = pnlColor
        End If
    End Sub

    Private Shared Function DecimalValue(row As DataRow, columnName As String) As Decimal
        If row Is Nothing OrElse row.IsNull(columnName) Then Return 0D
        Return Convert.ToDecimal(row(columnName), CultureInfo.InvariantCulture)
    End Function
End Class
