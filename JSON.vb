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

    Function AppendJSON(ByVal chave As String, ByVal InitialPrice As String, ByVal Qtd As Decimal, ByVal Data As String, ByVal Wallet As String)
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
                datagrid.Columns(1).HeaderText = "Preço médio"
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

    Public Async Sub LoadCriptos(datagrid As DataGridView)
        Dim originalDT = ConvertListToDataTable(LoadJSONtoDataGrid())
        Dim getCriptoData As New Cotacao

        Dim newDT As New DataTable()
        newDT.Columns.Add("Cripto", GetType(String))
        newDT.Columns.Add("%", GetType(String))
        newDT.Columns.Add("Wallet", GetType(String))
        newDT.Columns.Add("Qtd", GetType(String))
        newDT.Columns.Add("valorEntrada$", GetType(String))
        newDT.Columns.Add("valorEntradaR$", GetType(String))
        newDT.Columns.Add("precoMedio", GetType(String))
        newDT.Columns.Add("precoAtual", GetType(String))
        newDT.Columns.Add("valorAtual$", GetType(String))
        newDT.Columns.Add("valorAtualR$", GetType(String))

        ' Adicionar dados do DataTable original ao novo
        For Each row As DataRow In originalDT.Rows
            Dim newRow As DataRow = newDT.NewRow()

            Dim critoPriceTask As Task(Of String) = getCriptoData.GetCriptoPrices(row("Cripto"))
            Dim currPrice As Decimal = Await critoPriceTask
            Dim initialPrice As Decimal = row("InitialPrice").ToString.Replace(".", ",")
            Dim initialValueUSD As Decimal = row("Qtd").ToString.Replace(".", ",") * row("InitialPrice").ToString.Replace(".", ",")
            Dim initialValueBRL As Decimal = initialValueUSD * 5.7
            Dim currValueUSD As Decimal = row("Qtd").ToString.Replace(".", ",") * currPrice
            Dim currValueBRL As Decimal = currValueUSD * 5.7

            newRow("Cripto") = row("Cripto")

            newRow("%") = "10%"

            newRow("Wallet") = row("Wallet")

            If row("Qtd").ToString.Contains(".") Then
                newRow("Qtd") = row("Qtd").ToString.Replace(".", ",")
            Else
                newRow("Qtd") = CDec(row("Qtd")).ToString("N2", New CultureInfo("pt-BR"))
            End If

            newRow("valorEntrada$") = initialValueUSD.ToString("C", New CultureInfo("en-US"))

            newRow("valorEntradaR$") = initialValueBRL.ToString("C", New CultureInfo("pt-BR"))

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

            newRow("valorAtual$") = currValueUSD.ToString("C", New CultureInfo("en-US")).Replace("US$", "")

            newRow("valorAtualR$") = currValueBRL.ToString("C", New CultureInfo("pt-BR")).Replace("US$", "")

            newDT.Rows.Add(newRow)
        Next

        datagrid.DataSource = newDT
        'For Each row As DataRow In dt.Rows
        'MsgBox(row("InitialPrice"))
        'Next


    End Sub

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
