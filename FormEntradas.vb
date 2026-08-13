Imports System.Data
Imports System.Globalization

Public Class FormEntradas

    Dim charts As New Charts
    Dim json As New JSON
    Dim bs As New BindingSource()

    Private Sub BtSalvarEntrada_Click(sender As Object, e As EventArgs) Handles btSalvarEntrada.Click

        Try

            Dim key As String = cbCripto.Text.Trim()
            Dim symbol As String = cbCripto.Text.Trim().ToUpperInvariant()
            Dim wallet As String = cbWallet.Text.Trim()

            If String.IsNullOrWhiteSpace(key) OrElse
               String.IsNullOrWhiteSpace(symbol) OrElse
               String.IsNullOrWhiteSpace(wallet) Then

                MsgBox("Preencha todos os campos!")
                Return

            End If

            Dim precoEntrada As Decimal
            Dim qtd As Decimal

            If Not TryParseDecimalBR(TbPrecoEntrada.Text, precoEntrada) OrElse
               precoEntrada <= 0D Then

                MsgBox("Informe um preço de entrada válido.")
                Return

            End If

            If Not TryParseDecimalBR(tbQtd.Text, qtd) OrElse
               qtd <= 0D Then

                MsgBox("Informe uma quantidade válida.")
                Return

            End If

            Dim dataEntrada As String =
                dtpDataEntrada.Value.ToString("yyyy-MM-dd HH:mm:ss")

            Dim id As Long =
                PortfolioRepository.AddOrUpdate(
                    key,
                    symbol,
                    precoEntrada,
                    qtd,
                    dataEntrada,
                    wallet,
                    0D)

            If id > 0 Then

                MsgBox("Salvo!")
                LoadPortfolioGrid(dgCriptos)

            End If

        Catch ex As Exception

            MsgBox("Erro ao salvar no SQLite: " & ex.Message)

        End Try

    End Sub

    Private Sub FormEntradas_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        ' Estes dois JSONs são apenas catálogos auxiliares.
        ' Eles continuam independentes do banco do portfólio.
        json.loadFromJSON2ComboGrid(
            Application.StartupPath & "\JSON\wallets.json",
            cbWallet,
            Nothing)

        json.loadFromJSON2ComboGrid(
            Application.StartupPath & "\JSON\criptos.json",
            cbCripto,
            Nothing)

        ' Agora que os itens existem, define a seleção inicial.
        If cbCripto.Items.Count > 0 Then
            cbCripto.SelectedIndex = 0
        Else
            cbCripto.Text = ""
        End If

        If cbWallet.Items.Count > 0 Then
            cbWallet.SelectedIndex = 0
        Else
            cbWallet.Text = ""
        End If

        TbPrecoEntrada.Text = "0,00"
        tbQtd.Text = "0"

        LoadPortfolioGrid(dgCriptos)

    End Sub

    Private Sub LoadPortfolioGrid(Optional datagrid As DataGridView = Nothing)

        Dim table As DataTable =
            PortfolioRepository.GetAll()

        bs.DataSource = table

        If datagrid Is Nothing Then
            Return
        End If

        datagrid.DataSource = Nothing
        datagrid.AutoGenerateColumns = True
        datagrid.DataSource = bs

        FormatPortfolioGrid(datagrid)

    End Sub

    Private Sub FormatPortfolioGrid(datagrid As DataGridView)

        If datagrid.Columns.Contains("Id") Then
            datagrid.Columns("Id").Visible = False
        End If

        If datagrid.Columns.Contains("Cripto") Then
            datagrid.Columns("Cripto").Visible = False
        End If

        If datagrid.Columns.Contains("LastPrice") Then
            datagrid.Columns("LastPrice").Visible = False
        End If

        If datagrid.Columns.Contains("CreatedAt") Then
            datagrid.Columns("CreatedAt").Visible = False
        End If

        If datagrid.Columns.Contains("UpdatedAt") Then
            datagrid.Columns("UpdatedAt").Visible = False
        End If

        If datagrid.Columns.Contains("Symbol") Then
            datagrid.Columns("Symbol").DisplayIndex = 0
            datagrid.Columns("Symbol").HeaderText = "Cripto"
            datagrid.Columns("Symbol").Width = 100
        End If

        If datagrid.Columns.Contains("Quantity") Then
            datagrid.Columns("Quantity").DisplayIndex = 1
            datagrid.Columns("Quantity").HeaderText = "Qtd"
            datagrid.Columns("Quantity").Width = 90
        End If

        If datagrid.Columns.Contains("InitialPrice") Then
            datagrid.Columns("InitialPrice").DisplayIndex = 2
            datagrid.Columns("InitialPrice").HeaderText = "Preço médio"
            datagrid.Columns("InitialPrice").Width = 100
            datagrid.Columns("InitialPrice").DefaultCellStyle.Format = "N8"
        End If

        If datagrid.Columns.Contains("Data") Then
            datagrid.Columns("Data").DisplayIndex = 3
            datagrid.Columns("Data").HeaderText = "Data"
            datagrid.Columns("Data").Width = 150
        End If

        If datagrid.Columns.Contains("Wallet") Then
            datagrid.Columns("Wallet").DisplayIndex = 4
            datagrid.Columns("Wallet").HeaderText = "Wallet"
            datagrid.Columns("Wallet").Width = 100
        End If

        datagrid.ColumnHeadersHeight = 40
        datagrid.CellBorderStyle = DataGridViewCellBorderStyle.None
        datagrid.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.None
        datagrid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter

        With datagrid.ColumnHeadersDefaultCellStyle
            .BackColor = Color.FromArgb(40, 40, 40)
            .ForeColor = Color.Aqua
            .Font = New Font("Calibri", 10, FontStyle.Italic)
        End With

        For Each column As DataGridViewColumn In datagrid.Columns
            column.DefaultCellStyle.BackColor = Color.Black
            column.DefaultCellStyle.Font = New Font("Calibri", 10, FontStyle.Bold)
            column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
        Next

        If datagrid.Columns.Contains("Symbol") Then
            datagrid.Columns("Symbol").DefaultCellStyle.ForeColor = Color.White
        End If

        If datagrid.Columns.Contains("InitialPrice") Then
            datagrid.Columns("InitialPrice").DefaultCellStyle.ForeColor = Color.LimeGreen
        End If

        If datagrid.Columns.Contains("Quantity") Then
            datagrid.Columns("Quantity").DefaultCellStyle.ForeColor = Color.Gold
        End If

        If datagrid.Columns.Contains("Wallet") Then
            datagrid.Columns("Wallet").DefaultCellStyle.ForeColor = Color.White
        End If

        For Each row As DataGridViewRow In datagrid.Rows

            If row.IsNewRow Then Continue For

            If datagrid.Columns.Contains("Wallet") Then

                Select Case row.Cells("Wallet").Value?.ToString().ToUpperInvariant()
                    Case "BINANCE"
                        row.Cells("Wallet").Style.ForeColor = Color.Goldenrod
                    Case "METAMASK"
                        row.Cells("Wallet").Style.ForeColor = Color.DarkOrange
                    Case "TRUSTWALLET"
                        row.Cells("Wallet").Style.ForeColor = Color.LawnGreen
                    Case "PHANTOM"
                        row.Cells("Wallet").Style.ForeColor = Color.MediumPurple
                    Case "BYBIT"
                        row.Cells("Wallet").Style.ForeColor = Color.Gainsboro
                    Case "GATE.IO"
                        row.Cells("Wallet").Style.ForeColor = Color.DodgerBlue
                    Case "MEXC"
                        row.Cells("Wallet").Style.ForeColor = Color.White
                End Select

            End If

            row.Height = 35

        Next

        datagrid.ClearSelection()
        datagrid.CurrentCell = Nothing

    End Sub

    Private Sub ExcluirToolStripMenuItem_Click_1(sender As Object, e As EventArgs) Handles ExcluirToolStripMenuItem.Click

        Try

            If dgCriptos.SelectedRows.Count = 0 Then
                MsgBox("Selecione um registro para excluir.")
                Return
            End If

            Dim selectedRow As DataGridViewRow = dgCriptos.SelectedRows(0)

            Dim id As Long =
                Convert.ToInt64(selectedRow.Cells("Id").Value)

            Dim symbol As String =
                selectedRow.Cells("Symbol").Value?.ToString()

            If MessageBox.Show(
                $"Excluir {symbol}?",
                "Confirmar exclusão",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning) <> DialogResult.Yes Then
                Return
            End If

            PortfolioRepository.Delete(id)
            LoadPortfolioGrid(dgCriptos)

        Catch ex As Exception

            MsgBox("Erro ao excluir do SQLite: " & ex.Message)

        End Try

    End Sub

    Private Sub ButtonCancel_Click(sender As Object, e As EventArgs) Handles ButtonCancel.Click
        Me.Close()
    End Sub

    Private Sub dgCriptos_CellEnter(sender As Object, e As DataGridViewCellEventArgs) Handles dgCriptos.CellEnter

        Try

            If dgCriptos.SelectedRows.Count = 0 Then Return

            Dim row As DataGridViewRow = dgCriptos.SelectedRows(0)

            cbCripto.Text = row.Cells("Symbol").Value?.ToString()

            TbPrecoEntrada.Text =
                Convert.ToDecimal(row.Cells("InitialPrice").Value).
                ToString("N8", CultureInfo.GetCultureInfo("pt-BR"))

            tbQtd.Text =
                Convert.ToDecimal(row.Cells("Quantity").Value).
                ToString("G29", CultureInfo.GetCultureInfo("pt-BR"))

            Dim dataValue As DateTime

            If DateTime.TryParse(
                row.Cells("Data").Value?.ToString(),
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                dataValue) Then

                dtpDataEntrada.Value = dataValue

            End If

            cbWallet.Text = row.Cells("Wallet").Value?.ToString()

        Catch
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

    Private Async Sub FormEntradas_FormClosed(sender As Object, e As FormClosedEventArgs) Handles MyBase.FormClosed

        If FormMain.msgQuestion(
            "Deseja atualizar os gráficos?",
            "Aviso") Then

            Await FormMain.refreshMarket()

        End If

    End Sub

    Private Function TryParseDecimalBR(
        text As String,
        ByRef value As Decimal) As Boolean

        If String.IsNullOrWhiteSpace(text) Then
            value = 0D
            Return False
        End If

        Dim normalized As String = text.Trim()

        If normalized.Contains(",") AndAlso normalized.Contains(".") Then

            If normalized.LastIndexOf(","c) > normalized.LastIndexOf("."c) Then
                normalized = normalized.Replace(".", String.Empty).Replace(",", ".")
            Else
                normalized = normalized.Replace(",", String.Empty)
            End If

            Return Decimal.TryParse(
                normalized,
                NumberStyles.Any,
                CultureInfo.InvariantCulture,
                value)

        End If

        If normalized.Contains(",") Then

            Return Decimal.TryParse(
                normalized,
                NumberStyles.Any,
                CultureInfo.GetCultureInfo("pt-BR"),
                value)

        End If

        Return Decimal.TryParse(
            normalized,
            NumberStyles.Any,
            CultureInfo.InvariantCulture,
            value)

    End Function

End Class