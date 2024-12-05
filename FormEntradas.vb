Imports System.Globalization

Public Class FormEntradas

    Private Sub BtSalvarEntrada_Click(sender As Object, e As EventArgs) Handles btSalvarEntrada.Click
        Dim json As New JSON
        Dim key = cbCripto.SelectedItem.ToString
        If (IsNothing(tbQtd.Text) Or tbQtd.Text = 0) Or (Not IsNumeric(TbPrecoEntrada.Text) Or TbPrecoEntrada.Text = 0 Or IsNothing(TbPrecoEntrada)) Then
            MsgBox("Preencha todos os campos!")
        Else
            If json.AppendJSON(key, TbPrecoEntrada.Text, tbQtd.Text, dtpDataEntrada.Text, cbWallet.SelectedItem.ToString) Then
                MsgBox("Salvo!")
                FormEntradas_Load(sender, e)
                FormMain.GroupOverview.Controls.RemoveByKey("CriptoChart")
                FormMain.Setup()
            End If
        End If

    End Sub


    Private Sub FormEntradas_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim json As New JSON
        cbCripto.SelectedIndex = 0
        cbWallet.SelectedIndex = 0
        TbPrecoEntrada.Text = 0.00
        tbQtd.Text = 0
        json.LoadJSONtoDataGrid(dgCriptos)
        dgCriptos.Columns(1).HeaderText = "Preço médio/entrada"
        dgCriptos.Columns(0).Width = 60
        dgCriptos.Columns(1).Width = 90
        dgCriptos.Columns(2).Width = 80
        dgCriptos.Columns(3).Width = 80
        dgCriptos.Columns(4).Width = 104
    End Sub


    Private Sub ExcluirToolStripMenuItem_Click_1(sender As Object, e As EventArgs) Handles ExcluirToolStripMenuItem.Click
        Dim json As New JSON
        Dim row As DataGridViewRow = dgCriptos.CurrentRow

        Dim key As String = dgCriptos.CurrentRow.Cells(0).Value.ToString()
        If json.DeleteJSON(key) Then
            dgCriptos.Rows.Remove(row)
            FormMain.GroupOverview.Controls.RemoveByKey("CriptoChart")
            FormMain.Setup()
        End If

    End Sub

    Private Sub dgCriptos_CellEnter(sender As Object, e As DataGridViewCellEventArgs) Handles dgCriptos.CellEnter
        Try
            cbCripto.Text = dgCriptos.SelectedRows.Item(0).Cells(0).Value.ToString
            TbPrecoEntrada.Text = dgCriptos.SelectedRows.Item(0).Cells(1).Value.ToString.Replace(".", ",")
            tbQtd.Text = dgCriptos.SelectedRows.Item(0).Cells(2).Value.ToString.Replace(".", ",")
            dtpDataEntrada.Value = dgCriptos.SelectedRows.Item(0).Cells(3).Value.ToString
            cbWallet.Text = dgCriptos.SelectedRows.Item(0).Cells(4).Value.ToString
        Catch ex As Exception
            ' MsgBox(ex.Message)
        End Try

    End Sub


    Private Sub ButtonCancel_Click(sender As Object, e As EventArgs) Handles ButtonCancel.Click
        Me.Close()
    End Sub

End Class