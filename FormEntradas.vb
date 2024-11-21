Imports System.Globalization

Public Class FormEntradas

    Private Sub BtSalvarEntrada_Click(sender As Object, e As EventArgs) Handles btSalvarEntrada.Click
        Dim json As New JSON
        Dim key = cbCripto.SelectedItem.ToString

        If json.AppendJSON(key, TbPrecoEntrada.Text, tbQtd.Text, dtpDataEntrada.Text, cbWallet.SelectedItem.ToString) Then
            MsgBox("Salvo!")
            FormEntradas_Load(sender, e)
        End If

    End Sub


    Private Sub FormEntradas_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim json As New JSON
        cbCripto.SelectedIndex = 0
        cbWallet.SelectedIndex = 0
        TbPrecoEntrada.Text = 0.00
        tbQtd.Text = 0
        json.LoadJSONtoDataGrid(dgCriptos)

    End Sub


    Private Sub ExcluirToolStripMenuItem_Click_1(sender As Object, e As EventArgs) Handles ExcluirToolStripMenuItem.Click
        Dim json As New JSON
        Dim row As DataGridViewRow = dgCriptos.CurrentRow

        Dim key As String = dgCriptos.CurrentCell.Value.ToString()
        If json.DeleteJSON(key) Then
            dgCriptos.Rows.Remove(row)
        End If

    End Sub

    Private Sub dgCriptos_CellEnter(sender As Object, e As DataGridViewCellEventArgs) Handles dgCriptos.CellEnter
        Try
            cbCripto.Text = dgCriptos.SelectedRows.Item(0).Cells(0).Value.ToString
            TbPrecoEntrada.Text = dgCriptos.SelectedRows.Item(0).Cells(1).Value.ToString
            tbQtd.Text = dgCriptos.SelectedRows.Item(0).Cells(2).Value.ToString
            dtpDataEntrada.Value = dgCriptos.SelectedRows.Item(0).Cells(3).Value.ToString
            cbWallet.Text = dgCriptos.SelectedRows.Item(0).Cells(4).Value.ToString
        Catch ex As Exception
            ' MsgBox(ex.Message)
        End Try

    End Sub

End Class