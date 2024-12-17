Public Class FormSymbols
    Private Sub FormSymbols_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim json As New JSON
        json.loadFromJSON2ComboGrid(Application.StartupPath & "\JSON\criptos.json", Nothing, dgSymbols)
        Me.Icon = FormMain.Icon

        dgSymbols.Columns(0).HeaderText = "Simbolo"
        dgSymbols.ColumnHeadersHeight = 30
        dgSymbols.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.None
        dgSymbols.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft

        With dgSymbols.ColumnHeadersDefaultCellStyle
            .BackColor = Color.FromArgb(30, 30, 30)
            .ForeColor = Color.Aqua
            .Font = New Font("Calibri", 12, FontStyle.Regular)
        End With

        For Each row As DataGridViewRow In dgSymbols.Rows
            With row.Cells(0)
                .Style.ForeColor = Color.Orange
                .Style.BackColor = Color.FromArgb(20, 20, 20)
            End With
        Next

        dgSymbols.ClearSelection()
        dgSymbols.CurrentCell = Nothing
        ToolStripStatusLabel1.Text = dgSymbols.RowCount & " Registros"
        tbSymbol.Clear()
        tbSymbol.Focus()

    End Sub

    Private Sub btSalvarEntrada_Click(sender As Object, e As EventArgs) Handles btSalvarEntrada.Click
        Dim json As New JSON
        json.AddWalletExchangeSymbolToJson(Application.StartupPath & "\JSON\criptos.json", tbSymbol.Text)
        FormSymbols_Load(sender, e)
        json.loadFromJSON2ComboGrid(Application.StartupPath & "\JSON\criptos.json", FormEntradas.cbCripto, Nothing)
    End Sub
    Private Sub ExcluirToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExcluirToolStripMenuItem.Click
        Dim json As New JSON
        json.RemoveWalletExchangeSymbolFromJson(Application.StartupPath & "\JSON\criptos.json", dgSymbols.SelectedRows(0).Cells(0).Value.ToString)
        FormSymbols_Load(sender, e)
        json.loadFromJSON2ComboGrid(Application.StartupPath & "\JSON\criptos.json", FormEntradas.cbCripto, Nothing)
    End Sub

    Private Sub dgSymbols_MouseDown(sender As Object, e As MouseEventArgs) Handles dgSymbols.MouseDown
        Dim json As New JSON
        json.captureRightClick(dgSymbols, e)
    End Sub

End Class