Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports System.Globalization
Imports System.IO


Public Class JSON
    Private path As String = Application.StartupPath & "\JSON\criptos.json"
    Private jsonString As String = File.ReadAllText(path)
    Private bindingSource As New BindingSource()

    Public Function CheckJSONKey(ByVal jsonKey As String)
        Try
            Dim dados As JObject = JObject.Parse(jsonString)

            If dados.ContainsKey(jsonKey) Then
                Return True
            Else
                Return False
            End If
        Catch ex As Exception
            MsgBox("Erro ao carregar o arquivo JSON: " & ex.Message)
        End Try

        Return False

    End Function


    Public Function FindByJSONkey(ByVal jsonKey As String) As JObject
        Try
            Dim dados As JObject = JObject.Parse(jsonString)

            If CheckJSONKey(jsonKey) Then
                Return dados(jsonKey)
            End If
        Catch ex As Exception
            MsgBox("Erro ao carregar o arquivo JSON: " & ex.Message)
        End Try

        Return Nothing
    End Function

    Function AppendJSON(ByVal chave As String, ByVal InitialPrice As Decimal, ByVal Qtd As Decimal, ByVal Data As String, ByVal Wallet As String)
        Try
            Dim jsonObject As JObject = JObject.Parse(jsonString)
            Dim newObject As New JObject()

            newObject("InitialPrice") = InitialPrice
            newObject("Qtd") = Qtd
            newObject("Data") = Data
            newObject("Wallet") = Wallet
            jsonObject(chave) = New JArray()

            Dim itemsArray As JArray = CType(jsonObject(chave), JArray)

            itemsArray.Add(newObject)

            bindingSource.DataSource = itemsArray

            File.WriteAllText(path, jsonObject.ToString())

            Return True

        Catch ex As Exception
            MsgBox("Erro ao salvar o arquivo JSON: " & ex.Message)
            Return False
        End Try

    End Function

    Public Sub UpdateJSON(ByVal param As String, ByVal newValue As String)
        Try
            Dim jObject As JObject = TryCast(Newtonsoft.Json.JsonConvert.DeserializeObject(jsonString), JObject)
            Dim jToken As JToken = jObject.SelectToken(param)
            jToken.Replace(newValue)
            Dim updatedJsonString As String = jObject.ToString()
            File.WriteAllText(path, jObject.ToString(Formatting.Indented))
        Catch ex As Exception

        End Try

    End Sub

    Public Function DeleteJSON(ByVal key As String)
        If File.Exists(path) Then

            Dim jsonObject As JObject = JObject.Parse(jsonString)
            Dim chaveParaExcluir As String = key

            If jsonObject.ContainsKey(chaveParaExcluir) Then
                jsonObject.Remove(chaveParaExcluir)
                MessageBox.Show(chaveParaExcluir & " removido com sucesso.")
            Else
                MessageBox.Show("Chave não encontrada.")
                Return False
            End If
            ' Salva o JSON modificado de volta no arquivo
            File.WriteAllText(path, jsonObject.ToString())
            Return True
        Else
            MessageBox.Show("O arquivo JSON não foi encontrado.")
            Return False
        End If

    End Function

    Public Function LoadJSONtoDataGrid(Optional ByVal datagrid As DataGridView = Nothing)

        If File.Exists(path) Then
            Dim jsonObject As JObject = JObject.Parse(jsonString)
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
                            .Wallet = item.Wallet
                        }
                        allItems.Add(itemkey)
                    Next

                End If
            Next

            bindingSource.DataSource = allItems

            If datagrid IsNot Nothing Then
                datagrid.DataSource = bindingSource
            End If

            Return allItems
        Else
            MessageBox.Show("O arquivo JSON não foi encontrado.")
            Return False
        End If

    End Function

    Public Function ConvertListToDataTable(Of T)(list As List(Of T)) As DataTable
        Dim table As New DataTable()
        Dim properties = GetType(T).GetProperties()

        For Each prop In properties
            table.Columns.Add(prop.Name, If(Nullable.GetUnderlyingType(prop.PropertyType), prop.PropertyType))
        Next

        For Each item In list
            Dim row = table.NewRow()
            For Each prop In properties
                row(prop.Name) = If(prop.GetValue(item), DBNull.Value)
            Next
            table.Rows.Add(row)
        Next

        Return table

    End Function

    Public Async Function LoadCriptos(datagrid As DataGridView) As Task(Of Decimal)
        Dim originalDT = ConvertListToDataTable(LoadJSONtoDataGrid())
        Dim getCriptoData As New Cotacao
        Dim USDBRLprice = Await getCriptoData.GetUSDBRL
        Dim total As Decimal
        Dim wallet As String = ""
        Dim currValueUSD As Decimal
        Dim currValueBRL As Decimal
        Dim roi As Decimal
        Dim perform As Decimal?

        Dim newDT As New DataTable()
        newDT.Columns.Add("Cripto", GetType(String))
        newDT.Columns.Add("Perf", GetType(String))
        newDT.Columns.Add("Wallet", GetType(String))
        newDT.Columns.Add("Qtd", GetType(String))
        newDT.Columns.Add("vlEntradaUSD", GetType(String))
        newDT.Columns.Add("vlEntradaBRL", GetType(String))
        newDT.Columns.Add("precoMedio", GetType(String))
        newDT.Columns.Add("precoAtual", GetType(String))
        newDT.Columns.Add("vlAtualUSD", GetType(String))
        newDT.Columns.Add("vlAtualBRL", GetType(String))
        newDT.Columns.Add("ROIusd", GetType(String))
        newDT.Columns.Add("ROIbrl", GetType(String))

        ' Adicionar dados do DataTable original ao novo
        For Each row As DataRow In originalDT.Rows
            Dim newRow As DataRow = newDT.NewRow()

            Dim critoPriceTask As Task(Of String) = getCriptoData.GetCriptoPrices(row("Cripto"))
            Dim currPrice As Decimal = Await critoPriceTask
            Dim initialPrice As Decimal = row("InitialPrice").ToString.Replace(".", ",")
            Dim initialValueUSD As Decimal = row("Qtd").ToString.Replace(".", ",") * row("InitialPrice").ToString.Replace(".", ",")
            Dim initialValueBRL As Decimal = initialValueUSD * USDBRLprice
            wallet = row("Wallet")
            currValueUSD = row("Qtd").ToString.Replace(".", ",") * currPrice
            currValueBRL = currValueUSD * USDBRLprice
            roi = currValueUSD - initialValueUSD
            perform = (roi / initialValueUSD) * 100
            total += roi

            newRow("Cripto") = row("Cripto")
            '$"{perform.Value:F2}%"
            newRow("Perf") = $"{perform.Value:F2}%"

            newRow("Wallet") = wallet

            If row("Qtd").ToString.Contains(".") Then
                newRow("Qtd") = row("Qtd").ToString.Replace(".", ",")
            Else
                newRow("Qtd") = CDec(row("Qtd")).ToString("N2", New CultureInfo("pt-BR"))
            End If

            newRow("vlEntradaUSD") = USDformat(initialValueUSD)

            newRow("vlEntradaBRL") = BRLformat(initialValueBRL)

            If initialPrice > 1 Then
                newRow("precoMedio") = initialPrice.ToString("C", New CultureInfo("en-US"))
            Else
                newRow("precoMedio") = initialPrice.ToString("C8", New CultureInfo("en-US"))
            End If

            If currPrice > 1 Then
                newRow("precoAtual") = currPrice.ToString("C", New CultureInfo("en-US"))
            Else
                newRow("precoAtual") = currPrice.ToString("C8", New CultureInfo("en-US"))
            End If

            newRow("vlAtualUSD") = USDformat(currValueUSD)

            newRow("vlAtualBRL") = BRLformat(currValueBRL)

            newRow("ROIusd") = USDformat(roi)

            newRow("ROIbrl") = BRLformat(roi * USDBRLprice)

            newDT.Rows.Add(newRow)

        Next
        Dim fontsize As Int16 = 9.7
        Dim fontname As String = "Calibri"

        datagrid.DataSource = newDT

        datagrid.Columns(0).Width = 75
        With datagrid.Columns(0).DefaultCellStyle
            .BackColor = Color.Black
            .ForeColor = Color.Cyan
            .Font = New Font(fontname, fontsize, FontStyle.Bold)
        End With

        datagrid.Columns(1).Width = 80
        With datagrid.Columns(1).DefaultCellStyle
            .BackColor = Color.FromArgb(30, 30, 30)
            .ForeColor = Color.Orange
            .Font = New Font(fontname, fontsize, FontStyle.Bold)
        End With

        datagrid.Columns(2).Width = 80
        With datagrid.Columns(2).DefaultCellStyle
            .BackColor = Color.FromArgb(50, 50, 50)
            .Font = New Font(fontname, fontsize, FontStyle.Bold)
        End With

        datagrid.Columns(3).Width = 95
        With datagrid.Columns(3).DefaultCellStyle
            .BackColor = Color.MidnightBlue
            .ForeColor = Color.Red
            .Font = New Font(fontname, fontsize, FontStyle.Bold)
        End With

        datagrid.Columns(4).HeaderText = "Valor entrada/médio USD"
        datagrid.Columns(4).Width = 95
        With datagrid.Columns(4).DefaultCellStyle
            .BackColor = Color.Black
            .ForeColor = Color.Lime
            .Font = New Font(fontname, fontsize, FontStyle.Bold)
        End With

        datagrid.Columns(5).HeaderText = "Valor entrada/médio BRL"
        datagrid.Columns(5).Width = 95
        With datagrid.Columns(5).DefaultCellStyle
            .BackColor = Color.Black
            .ForeColor = Color.Yellow
            .Font = New Font(fontname, fontsize, FontStyle.Bold)
        End With


        datagrid.Columns(6).HeaderText = "Preço entrada USD"
        datagrid.Columns(6).Width = 95
        With datagrid.Columns(6).DefaultCellStyle
            .BackColor = Color.Indigo
            .ForeColor = Color.Lime
            .Font = New Font(fontname, fontsize, FontStyle.Bold)
        End With

        datagrid.Columns(7).HeaderText = "Preço atual"
        datagrid.Columns(7).Width = 95
        With datagrid.Columns(7).DefaultCellStyle
            .BackColor = Color.Indigo
            .ForeColor = Color.Yellow
            .Font = New Font(fontname, fontsize, FontStyle.Bold)
        End With

        datagrid.Columns(8).HeaderText = "Valor atual USD"
        datagrid.Columns(8).Width = 95
        With datagrid.Columns(8).DefaultCellStyle
            .BackColor = Color.FromArgb(30, 30, 30)
            .ForeColor = Color.Lime
            .Font = New Font(fontname, fontsize, FontStyle.Bold)
        End With

        datagrid.Columns(9).HeaderText = "Valor atual BRL"
        datagrid.Columns(9).Width = 95
        With datagrid.Columns(9).DefaultCellStyle
            .BackColor = Color.FromArgb(30, 30, 30)
            .ForeColor = Color.Yellow
            .Font = New Font(fontname, fontsize, FontStyle.Bold)
        End With


        datagrid.Columns(10).HeaderText = "ROI"
        datagrid.Columns(10).Width = 95
        With datagrid.Columns(10).DefaultCellStyle
            .BackColor = Color.FromArgb(20, 20, 20)
            .Font = New Font(fontname, fontsize, FontStyle.Bold)
        End With

        datagrid.Columns(11).HeaderText = ""
        datagrid.Columns(11).Width = 95
        With datagrid.Columns(11).DefaultCellStyle
            .BackColor = Color.FromArgb(20, 20, 20)
            .ForeColor = Color.IndianRed
            .Font = New Font(fontname, fontsize, FontStyle.Bold)
        End With

        For Each row As DataGridViewRow In datagrid.Rows

            Select Case row.Cells(2).Value
                Case "Binance"
                    row.Cells(2).Style.ForeColor = Color.Goldenrod
                Case "Metamask"
                    row.Cells(2).Style.ForeColor = Color.DarkOrange
                Case "TrustWallet"
                    row.Cells(2).Style.ForeColor = Color.LawnGreen
                Case "Phantom"
                    row.Cells(2).Style.ForeColor = Color.MediumPurple
                Case "Bybit"
                    row.Cells(2).Style.ForeColor = Color.Gainsboro
                Case "Gate.io"
                    row.Cells(2).Style.ForeColor = Color.DodgerBlue
            End Select

            Select Case CInt(row.Cells(1).Value.ToString.Replace("%", ""))
                Case > 0
                    row.Cells(1).Style.ForeColor = Color.LawnGreen
            End Select

            Select Case CDec(row.Cells(11).Value)
                Case > 0
                    row.Cells(11).Style.ForeColor = Color.Aquamarine
            End Select
        Next

        Return total

    End Function

    Public Function USDformat(ByVal value As Decimal)
        Return value.ToString("C", New CultureInfo("en-US"))
    End Function

    Public Function BRLformat(ByVal value As Decimal)
        Return value.ToString("C", New CultureInfo("pt-BR"))
    End Function

End Class

Public Class Item
    Public Property InitialPrice As String
    Public Property Qtd As String
    Public Property Data As String
    Public Property Wallet As String
End Class

Public Class ItemKey
    Public Property Cripto As String
    Public Property InitialPrice As String
    Public Property Qtd As String
    Public Property Data As String
    Public Property Wallet As String
End Class
