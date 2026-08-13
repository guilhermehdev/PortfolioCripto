Public Class FormWalletExchange

    Private Sub FormWalletExchange_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        LoadWallets()
        Me.Icon = FormMain.Icon
        tbWalletExchange.Clear()
        tbWalletExchange.Focus()
    End Sub

    Private Sub LoadWallets()
        Dim table As DataTable = PortfolioRepository.GetWallets()
        dgWalletExchange.DataSource = table

        If dgWalletExchange.Columns.Contains("Id") Then
            dgWalletExchange.Columns("Id").Visible = False
        End If

        If dgWalletExchange.Columns.Contains("Name") Then
            dgWalletExchange.Columns("Name").HeaderText = "Wallet/Exchange"
        End If

        dgWalletExchange.ColumnHeadersHeight = 30
        dgWalletExchange.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.None
        dgWalletExchange.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft

        With dgWalletExchange.ColumnHeadersDefaultCellStyle
            .BackColor = Color.FromArgb(30, 30, 30)
            .ForeColor = Color.Aqua
            .Font = New Font("Calibri", 12, FontStyle.Regular)
        End With

        For Each row As DataGridViewRow In dgWalletExchange.Rows
            If row.IsNewRow Then Continue For
            If dgWalletExchange.Columns.Contains("Name") Then
                row.Cells("Name").Style.ForeColor = Color.Orange
                row.Cells("Name").Style.BackColor = Color.FromArgb(20, 20, 20)
            End If
        Next

        dgWalletExchange.ClearSelection()
        dgWalletExchange.CurrentCell = Nothing
        ToolStripStatusLabel1.Text = dgWalletExchange.Rows.Cast(Of DataGridViewRow)().Count(Function(r) Not r.IsNewRow) & " Registros"
    End Sub

    Private Sub btSalvarEntrada_Click(sender As Object, e As EventArgs) Handles btSalvarEntrada.Click
        If String.IsNullOrWhiteSpace(tbWalletExchange.Text) Then
            MessageBox.Show("Preencha a wallet/exchange!", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            tbWalletExchange.Focus()
            Return
        End If

        Try
            PortfolioRepository.AddWallet(tbWalletExchange.Text)
            LoadWallets()

            If FormEntradas IsNot Nothing Then
                FormEntradas.ReloadWalletCombo()
            End If

        Catch ex As Exception
            MessageBox.Show("Erro ao salvar wallet: " & ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub ExcluirToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExcluirToolStripMenuItem.Click
        Try
            If dgWalletExchange.SelectedRows.Count = 0 Then Return

            Dim id As Long =
                Convert.ToInt64(dgWalletExchange.SelectedRows(0).Cells("Id").Value)

            PortfolioRepository.DeleteWallet(id)
            LoadWallets()

            If FormEntradas IsNot Nothing Then
                FormEntradas.ReloadWalletCombo()
            End If

        Catch ex As Exception
            MessageBox.Show("Erro ao excluir wallet: " & ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub dgWalletExchange_MouseDown(sender As Object, e As MouseEventArgs) Handles dgWalletExchange.MouseDown
        Dim json As New JSON
        'json.captureRightClick(dgWalletExchange, e)
    End Sub

End Class