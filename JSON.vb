Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports System.IO


Public Class JSON
    Private path As String = Application.StartupPath & "\JSON\criptos.json"
    Private jsonString As String = File.ReadAllText(path)
    Public bindingSource As New BindingSource()

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

    Public Sub LoadJSONtoDataGrid(ByVal datagrid As DataGridView)

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
            datagrid.DataSource = bindingSource

        Else
            MessageBox.Show("O arquivo JSON não foi encontrado.")
        End If
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
