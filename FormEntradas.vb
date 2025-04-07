Imports System.Globalization
Imports Newtonsoft.Json.Linq

Public Class FormEntradas
    Dim charts As New Charts
    Dim json As New JSON

    'Private Sub BtSalvarEntrada_Click(sender As Object, e As EventArgs) Handles btSalvarEntrada.Click
    '    Dim json As New JSON
    '    Dim key = cbCripto.Text
    '    If (IsNothing(tbQtd.Text) Or tbQtd.Text = 0) Or (Not IsNumeric(TbPrecoEntrada.Text) Or TbPrecoEntrada.Text = 0 Or IsNothing(TbPrecoEntrada)) Then
    '        MsgBox("Preencha todos os campos!")
    '    Else
    '        'If json.AppendJSONLocal(key, TbPrecoEntrada.Text, tbQtd.Text, dtpDataEntrada.Text, cbWallet.Text, 1) Then
    '        If json.AppendJSONToBin(key, TbPrecoEntrada.Text, tbQtd.Text, dtpDataEntrada.Text, cbWallet.Text, 1) Then
    '            MsgBox("Salvo!")
    '            FormEntradas_Load(sender, e)
    '            'FormMain.Setup()
    '        End If
    '    End If

    'End Sub

    Private Async Sub BtSalvarEntrada_Click(sender As Object, e As EventArgs) Handles btSalvarEntrada.Click
        Dim key = cbCripto.Text

        If (String.IsNullOrWhiteSpace(tbQtd.Text) OrElse tbQtd.Text = "0") OrElse
       (Not IsNumeric(TbPrecoEntrada.Text) OrElse TbPrecoEntrada.Text = "0") Then
            MsgBox("Preencha todos os campos!")
        Else
            Dim sucesso As Boolean = Await json.AppendJSONToBin(key, TbPrecoEntrada.Text, tbQtd.Text, dtpDataEntrada.Text, cbWallet.Text, 1)

            If sucesso Then
                MsgBox("Salvo!")
                FormEntradas_Load(sender, e)
            End If
        End If
    End Sub

    Private Sub FormEntradas_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        cbCripto.SelectedItem = 0
        cbWallet.SelectedItem = 0
        TbPrecoEntrada.Text = 0.00
        tbQtd.Text = 0
        loadJSONtoDatagridLocal(dgCriptos)
        json.loadFromJSON2ComboGrid(Application.StartupPath & "\JSON\wallets.json", cbWallet, Nothing)
        json.loadFromJSON2ComboGrid(Application.StartupPath & "\JSON\criptos.json", cbCripto, Nothing)
        FormatGrid(dgCriptos)
    End Sub

    Private Function loadJSONtoDatagridLocal(Optional ByVal datagrid As DataGridView = Nothing)
        Dim jsonObject As JObject = JObject.Parse(json.loadJSONfile)
        Dim allItems As New List(Of ItemKey)()

        For Each propertyPair As KeyValuePair(Of String, JToken) In jsonObject
            If propertyPair.Value.Type = JTokenType.Array Then
                Dim items As List(Of Item) = propertyPair.Value.ToObject(Of List(Of Item))()

                For Each item As Item In items
                    Dim itemkey As New ItemKey() With {
                    .Cripto = propertyPair.Key,
                    .InitialPrice = item.InitialPrice,
                    .Qtd = item.Qtd,
                    .Data = item.Data,
                    .Wallet = item.Wallet,
                    .LastPrice = item.LastPrice
                }
                    allItems.Add(itemkey)
                Next
            End If
        Next

        If datagrid IsNot Nothing Then
            datagrid.DataSource = allItems
        End If

        Return allItems

    End Function


    Private Sub FormatGrid(datagrid As DataGridView)
        Dim fontsize As Int16 = 10
        Dim fontname As String = "Calibri"

        datagrid.ColumnHeadersHeight = 40
        datagrid.CellBorderStyle = DataGridViewCellBorderStyle.None
        datagrid.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.None
        datagrid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter

        With datagrid.ColumnHeadersDefaultCellStyle
            .BackColor = Color.FromArgb(40, 40, 40)
            .ForeColor = Color.Aqua
            .Font = New Font("Calibri", 10, FontStyle.Italic)
        End With

        datagrid.Columns(0).Width = 60
        With datagrid.Columns(0).DefaultCellStyle
            .BackColor = Color.Black
            .ForeColor = Color.White
            .Font = New Font(fontname, fontsize, FontStyle.Bold)
            .Alignment = DataGridViewContentAlignment.MiddleLeft
        End With

        datagrid.Columns(1).HeaderText = "Preço médio/entrada"
        datagrid.Columns(1).Width = 90
        With datagrid.Columns(1).DefaultCellStyle
            .BackColor = Color.Black
            .ForeColor = Color.LimeGreen
            .Font = New Font(fontname, fontsize, FontStyle.Bold)
            .Alignment = DataGridViewContentAlignment.MiddleLeft
        End With

        datagrid.Columns(2).Width = 80
        With datagrid.Columns(2).DefaultCellStyle
            .BackColor = Color.Black
            .ForeColor = Color.Gold
            .Font = New Font(fontname, fontsize, FontStyle.Bold)
            .Alignment = DataGridViewContentAlignment.MiddleLeft
        End With

        datagrid.Columns(3).Width = 80
        With datagrid.Columns(3).DefaultCellStyle
            .BackColor = Color.Black
            .ForeColor = Color.White
            .Font = New Font(fontname, fontsize, FontStyle.Bold)
            .Alignment = DataGridViewContentAlignment.MiddleLeft
        End With

        datagrid.Columns(4).Width = 104
        With datagrid.Columns(4).DefaultCellStyle
            .BackColor = Color.Black
            .Font = New Font(fontname, fontsize, FontStyle.Bold)
            .Alignment = DataGridViewContentAlignment.MiddleCenter
        End With

        For Each row As DataGridViewRow In datagrid.Rows

            Select Case row.Cells(4).Value
                Case "Binance"
                    row.Cells(4).Style.ForeColor = Color.Goldenrod
                Case "Metamask"
                    row.Cells(4).Style.ForeColor = Color.DarkOrange
                Case "TrustWallet"
                    row.Cells(4).Style.ForeColor = Color.LawnGreen
                Case "Phantom"
                    row.Cells(4).Style.ForeColor = Color.MediumPurple
                Case "Bybit"
                    row.Cells(4).Style.ForeColor = Color.Gainsboro
                Case "Gate.io"
                    row.Cells(4).Style.ForeColor = Color.DodgerBlue
            End Select

            row.Height = 35
            datagrid.ClearSelection()
            datagrid.CurrentCell = Nothing

            With row.Cells(1)
                .Style.Format = "C2"
                .Style.FormatProvider = New CultureInfo("en-US")
            End With
        Next

        datagrid.ClearSelection()
        datagrid.CurrentCell = Nothing

    End Sub

    Private Sub ExcluirToolStripMenuItem_Click_1(sender As Object, e As EventArgs) Handles ExcluirToolStripMenuItem.Click
        Dim row As DataGridViewRow = dgCriptos.SelectedRows.Item(0)

        Dim key As String = dgCriptos.SelectedRows.Item(0).Cells(0).Value.ToString()
        If json.DeleteJSONLocal(key) Then
            dgCriptos.Rows.Remove(row)
            FormEntradas_Load(sender, e)
            'FormMain.Setup()
        End If

    End Sub
    Private Sub ButtonCancel_Click(sender As Object, e As EventArgs) Handles ButtonCancel.Click
        Me.Close()
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

    Private Sub btAddWallet_Click(sender As Object, e As EventArgs) Handles btAddWallet.Click
        FormWalletExchange.Show()
    End Sub
    Private Sub btAddSymbol_Click(sender As Object, e As EventArgs) Handles btAddSymbol.Click
        FormSymbols.Show()
    End Sub

    Private Sub dgCriptos_MouseDown(sender As Object, e As MouseEventArgs) Handles dgCriptos.MouseDown
        json.captureRightClick(dgCriptos, e)
    End Sub
    Private Sub FormEntradas_FormClosed(sender As Object, e As FormClosedEventArgs) Handles MyBase.FormClosed
        FormMain.Setup()
    End Sub

End Class