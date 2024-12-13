Public Class FormWalletExchange
    Private Sub FormWalletExchange_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim json As New JSON
        json.loadFromJSON2ComboGrid(Nothing, dgWalletExchange)
        Me.Icon = FormMain.Icon

        dgWalletExchange.Columns(0).HeaderText = "Wallet/Exchange"
        dgWalletExchange.ColumnHeadersHeight = 30
        dgWalletExchange.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.None
        dgWalletExchange.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft

        With dgWalletExchange.ColumnHeadersDefaultCellStyle
            .BackColor = Color.FromArgb(30, 30, 30)
            .ForeColor = Color.Aqua
            .Font = New Font("Calibri", 14, FontStyle.Regular)
        End With

        For Each row As DataGridViewRow In dgWalletExchange.Rows
            With row.Cells(0)
                .Style.ForeColor = Color.Orange
                .Style.BackColor = Color.FromArgb(20, 20, 20)
            End With
        Next


        dgWalletExchange.ClearSelection()
        dgWalletExchange.CurrentCell = Nothing
        ToolStripStatusLabel1.Text = dgWalletExchange.RowCount & " Registros"
        tbWalletExchange.Clear()
        tbWalletExchange.Focus()

    End Sub
    Private Sub btSalvarEntrada_Click(sender As Object, e As EventArgs) Handles btSalvarEntrada.Click
        Dim json As New JSON
        json.AddWalletExchangeToJson(tbWalletExchange.Text)
        FormWalletExchange_Load(sender, e)
    End Sub
    Private Sub ExcluirToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExcluirToolStripMenuItem.Click
        Dim json As New JSON
        json.RemoveWalletExchangeFromJson(dgWalletExchange.CurrentRow.Cells(0).Value.ToString)
        FormWalletExchange_Load(sender, e)
    End Sub
End Class