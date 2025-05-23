Public Class FormWalletExchange
    Private Sub FormWalletExchange_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim json As New JSON
        json.loadFromJSON2ComboGrid(Application.StartupPath & "\JSON\wallets.json", Nothing, dgWalletExchange)
        Me.Icon = FormMain.Icon

        dgWalletExchange.Columns(0).Visible = False
        dgWalletExchange.Columns(1).HeaderText = "Wallet/Exchange"
        dgWalletExchange.ColumnHeadersHeight = 30
        dgWalletExchange.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.None
        dgWalletExchange.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft

        With dgWalletExchange.ColumnHeadersDefaultCellStyle
            .BackColor = Color.FromArgb(30, 30, 30)
            .ForeColor = Color.Aqua
            .Font = New Font("Calibri", 12, FontStyle.Regular)
        End With

        For Each row As DataGridViewRow In dgWalletExchange.Rows
            With row.Cells(1)
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
        json.AddWalletExchangeSymbolToJson(Application.StartupPath & "\JSON\wallets.json", tbWalletExchange.Text)
        FormWalletExchange_Load(sender, e)
        json.loadFromJSON2ComboGrid(Application.StartupPath & "\JSON\wallets.json", FormEntradas.cbWallet, Nothing)
    End Sub
    Private Sub ExcluirToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExcluirToolStripMenuItem.Click
        Dim json As New JSON
        json.RemoveWalletExchangeSymbolFromJson(Application.StartupPath & "\JSON\wallets.json", dgWalletExchange.SelectedRows(0).Cells(0).Value.ToString)
        FormWalletExchange_Load(sender, e)
        json.loadFromJSON2ComboGrid(Application.StartupPath & "\JSON\wallets.json", FormEntradas.cbWallet, Nothing)
    End Sub
    Private Sub dgWalletExchange_MouseDown(sender As Object, e As MouseEventArgs) Handles dgWalletExchange.MouseDown
        Dim json As New JSON
        json.captureRightClick(dgWalletExchange, e)
    End Sub

End Class