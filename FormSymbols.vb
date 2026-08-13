Public Class FormSymbols

    Private Sub FormSymbols_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        LoadSymbols()
        Me.Icon = FormMain.Icon
        tbSymbol.Clear()
        tbSymbol.Focus()
    End Sub

    Private Sub LoadSymbols()
        Dim table As DataTable = PortfolioRepository.GetCryptoSymbols()
        dgSymbols.DataSource = table

        If dgSymbols.Columns.Contains("Id") Then
            dgSymbols.Columns("Id").HeaderText = "ID"
            dgSymbols.Columns("Id").Width = 75
        End If

        If dgSymbols.Columns.Contains("Symbol") Then
            dgSymbols.Columns("Symbol").HeaderText = "Simbolo"
        End If

        dgSymbols.ColumnHeadersHeight = 30
        dgSymbols.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.None
        dgSymbols.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft

        With dgSymbols.ColumnHeadersDefaultCellStyle
            .BackColor = Color.FromArgb(30, 30, 30)
            .ForeColor = Color.Aqua
            .Font = New Font("Calibri", 12, FontStyle.Regular)
        End With

        For Each row As DataGridViewRow In dgSymbols.Rows
            If row.IsNewRow Then Continue For
            If dgSymbols.Columns.Contains("Id") Then
                row.Cells("Id").Style.ForeColor = Color.Lime
                row.Cells("Id").Style.BackColor = Color.FromArgb(20, 20, 30)
            End If
            If dgSymbols.Columns.Contains("Symbol") Then
                row.Cells("Symbol").Style.ForeColor = Color.Orange
                row.Cells("Symbol").Style.BackColor = Color.FromArgb(20, 20, 20)
            End If
        Next

        dgSymbols.ClearSelection()
        dgSymbols.CurrentCell = Nothing
        ToolStripStatusLabel1.Text = dgSymbols.Rows.Cast(Of DataGridViewRow)().Count(Function(r) Not r.IsNewRow) & " Registros"
    End Sub

    Private Sub btSalvarEntrada_Click(sender As Object, e As EventArgs) Handles btSalvarEntrada.Click
        If String.IsNullOrWhiteSpace(tbSymbol.Text) OrElse tbSymbol.Text.Trim().Length < 3 Then
            MessageBox.Show("Preencha o simbolo!", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            tbSymbol.Focus()
            Return
        End If

        Try
            Dim id As Integer?
            Dim parsedId As Integer

            If Integer.TryParse(tbID.Text.Trim(), parsedId) Then
                id = parsedId
            End If

            PortfolioRepository.AddCryptoSymbol(tbSymbol.Text, id)
            LoadSymbols()

            If FormEntradas IsNot Nothing Then
                FormEntradas.ReloadCryptoCombo()
            End If

        Catch ex As Exception
            MessageBox.Show("Erro ao salvar cripto: " & ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub ExcluirToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExcluirToolStripMenuItem.Click
        Try
            If dgSymbols.SelectedRows.Count = 0 Then Return

            Dim symbol As String =
                dgSymbols.SelectedRows(0).Cells("Symbol").Value?.ToString()

            If String.IsNullOrWhiteSpace(symbol) Then Return

            PortfolioRepository.DeleteCryptoSymbol(symbol)
            LoadSymbols()

            If FormEntradas IsNot Nothing Then
                FormEntradas.ReloadCryptoCombo()
            End If

        Catch ex As Exception
            MessageBox.Show("Erro ao excluir cripto: " & ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub dgSymbols_MouseDown(sender As Object, e As MouseEventArgs) Handles dgSymbols.MouseDown
        Dim json As New JSON
        json.captureRightClick(dgSymbols, e)
    End Sub

    Private Sub dgSymbols_CellEnter(sender As Object, e As DataGridViewCellEventArgs) Handles dgSymbols.CellEnter
        Try
            If dgSymbols.CurrentRow Is Nothing Then Return
            tbID.Text = dgSymbols.CurrentRow.Cells("Id").Value?.ToString()
            tbSymbol.Text = dgSymbols.CurrentRow.Cells("Symbol").Value?.ToString()
        Catch
        End Try
    End Sub

End Class