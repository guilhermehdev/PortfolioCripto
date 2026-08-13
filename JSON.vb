Imports System.Diagnostics.Eventing.Reader
Imports System.Globalization
Imports System.IO
Imports System.Net.Http
Imports System.Reflection
Imports System.Runtime.InteropServices.JavaScript.JSType
Imports System.Text
Imports System.Text.Json
Imports System.Windows.Forms.VisualStyles.VisualStyleElement
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports Windows.Win32.System.Diagnostics


Public Class JSON
    Public ReadOnly portfolioPathFile As String = Application.StartupPath & "\JSON\portfolio.json"
    Private ReadOnly bindingSource As New BindingSource()
    Private ReadOnly jsonbin = My.Settings.JSONBinID
    Private ReadOnly JSONBinMasterKey As String = My.Settings.JSONBinMasterKey
    Private ReadOnly saoPauloTimeZone As TimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time")
    Private ReadOnly saoPauloTime As DateTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, saoPauloTimeZone)
    Private ReadOnly JSONBinGet As String = $"{My.Settings.JSONBinURL}/b/{jsonbin}/latest"
    Private ReadOnly JSONBinPut As String = $"{My.Settings.JSONBinURL}/b/{jsonbin}"
    Dim b As New Binance
    Dim gec As New Coingecko
    Public Shared USDBRLprice As Decimal = 0D
    Public stablecoins As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase) From {
             "USDT", "USDC", "BUSD", "DAI", "TUSD", "USD", "USDP", "GUSD"
            }

    Public Function loadJSONfile()
        Dim jsonString As String = File.ReadAllText(portfolioPathFile)
        Return jsonString
    End Function
    Public Function checkMySettings()
        For Each settings In My.Settings.Properties
            Dim propertyName As String = settings.Name
            Dim propertyValue As Object = My.Settings(propertyName)

            Select Case propertyName
                Case "JSONBinID"
                    If propertyValue Is Nothing OrElse String.IsNullOrWhiteSpace(propertyValue.ToString()) Then
                        Return False
                    End If
                Case "JSONBinMasterKey"
                    If propertyValue Is Nothing OrElse String.IsNullOrWhiteSpace(propertyValue.ToString()) Then
                        Return False
                    End If
                Case "JSONBinURL"
                    If propertyValue Is Nothing OrElse String.IsNullOrWhiteSpace(propertyValue.ToString()) Then
                        Return False
                    End If
                Case "apiCMCKey"
                    If propertyValue Is Nothing OrElse String.IsNullOrWhiteSpace(propertyValue.ToString()) Then
                        Return False
                    End If

            End Select

        Next

        Return True

    End Function

    Async Function loadJSONfromJSONBIN() As Task(Of Boolean)
        If checkMySettings() Then
            Dim url As String = JSONBinGet

            Using client As New HttpClient()
                Try
                    client.DefaultRequestHeaders.Add("X-Master-Key", JSONBinMasterKey)

                    Dim response As HttpResponseMessage = Await client.GetAsync(url)

                    If response.IsSuccessStatusCode Then
                        Dim json As String = Await response.Content.ReadAsStringAsync()
                        Dim jObj As JObject = JObject.Parse(json)
                        Dim conteudoLimpo As JObject = CType(jObj("record"), JObject)

                        conteudoLimpo("ultimaAtualizacao") = saoPauloTime.ToString("yyyy-MM-dd HH:mm:ss")

                        File.WriteAllText(portfolioPathFile, conteudoLimpo.ToString())
                        Debug.WriteLine("JSON atualizado com sucesso!")
                        Return True
                    Else
                        Debug.WriteLine("Erro: " & response.StatusCode)
                        Return False
                    End If

                Catch ex As Exception
                    Debug.WriteLine("Exceção: " & ex.Message)
                    Return False
                End Try
            End Using
        Else
            FormAPI.ShowDialog()
            Return False
        End If

    End Function

    Public Async Function checkLastUpdateOnJSONBin() As Task(Of Boolean)
        Dim cjson As New JSON

        Try

            If Not Directory.Exists(Application.StartupPath & "\JSON") Or Not File.Exists(portfolioPathFile) Then

                Directory.CreateDirectory(Application.StartupPath & "\JSON")
                Await cjson.AppendJSONToBin("BTC", 10000, 5, Date.Today, "Wallet", 3000, "BTC")

            End If

            Dim url As String = $"{JSONBinPut}?meta=true"
            Using client As New HttpClient()
                client.DefaultRequestHeaders.Add("X-Master-Key", JSONBinMasterKey)

                Dim response As HttpResponseMessage = Await client.GetAsync(url)

                If response.IsSuccessStatusCode Then
                    Dim json As String = Await response.Content.ReadAsStringAsync()
                    Dim metaObj As JObject = JObject.Parse(json)

                    If metaObj("record") IsNot Nothing AndAlso metaObj("record")("ultimaAtualizacao") IsNot Nothing Then
                        Dim ultimaAtualizacaoStr As String = metaObj("record")("ultimaAtualizacao").ToString()
                        Dim ultimaAtualizacao As DateTime = DateTime.Parse(ultimaAtualizacaoStr)

                        If My.Settings.lastUpdate <> ultimaAtualizacao Then

                            My.Settings.lastUpdate = ultimaAtualizacao
                            My.Settings.Save()
                            Return Await loadJSONfromJSONBIN()
                        Else

                            Return True
                        End If
                    Else
                        Return False
                    End If
                Else
                    Return False
                End If
            End Using

        Catch ex As Exception
            FormMain.lbDebug.Clear()
            FormMain.lbDebug.AppendText("Status: JSONBin não respondeu! Carregando arquivo local...")
            Return False
        End Try
    End Function

    Public Async Function saveAportToJSONBin(key As String, precoMedio As Decimal, qtd As Decimal, data As Date, wallet As String, symbol As String) As Task(Of Boolean)

        Dim sucesso As Boolean = Await AppendJSONToBin(key, precoMedio, qtd, data, wallet, 1, symbol)
        Return sucesso

    End Function

    Public Async Function AppendJSONToBin(chave As String, InitialPrice As Decimal, Qtd As Decimal, Data As String, Wallet As String, lastPrice As Decimal, symbol As String) As Task(Of Boolean)
        Dim url As String = JSONBinPut
        Dim jsonAtual As JObject = Nothing

        Using client As New HttpClient()
            client.DefaultRequestHeaders.Add("X-Master-Key", JSONBinMasterKey)

            Try
                jsonAtual = JObject.Parse(loadJSONfile)

                If jsonAtual(chave) Is Nothing Then
                    jsonAtual(chave) = New JArray()
                End If

                Dim itemsArray As JArray = CType(jsonAtual(chave), JArray)
                Dim atualizado As Boolean = False

                For Each item As JObject In itemsArray
                    If item("Data") = Data AndAlso item("Wallet") = Wallet Then
                        item("InitialPrice") = InitialPrice
                        item("Qtd") = Qtd
                        item("LastPrice") = lastPrice
                        atualizado = True
                        Exit For
                    End If
                Next

                If Not atualizado Then
                    Dim novoItem As New JObject()
                    novoItem("InitialPrice") = InitialPrice
                    novoItem("Qtd") = Qtd
                    novoItem("Data") = Data
                    novoItem("Wallet") = Wallet
                    novoItem("LastPrice") = lastPrice
                    novoItem("Symbol") = symbol
                    itemsArray.Add(novoItem)
                End If

                jsonAtual("ultimaAtualizacao") = saoPauloTime.ToString("yyyy-MM-ddTHH:mm:ss")

                Dim body As String = JsonConvert.SerializeObject(jsonAtual, Formatting.Indented)
                Dim stringContent As New StringContent(body, Encoding.UTF8, "application/json")
                Dim putResponse = Await client.PutAsync(url, stringContent)

                File.WriteAllText(portfolioPathFile, jsonAtual.ToString())
                Return True

            Catch ex As Exception
                Debug.Write("Erro em AppendJSONToBin: " & ex.Message)
                FormMain.lbDebug.Text = "Erro ao salvar em JSONBin: " & ex.Message
                Return False
            End Try
        End Using
    End Function

    Public Function DeleteJSONFromBin(ByVal key As String) As Boolean
        Try
            Dim url As String = JSONBinPut

            Using client As New HttpClient()
                Dim jsonLocal As JObject = JObject.Parse(loadJSONfile)

                If jsonLocal.ContainsKey(key) Then
                    jsonLocal.Remove(key)
                Else
                    MessageBox.Show("Chave não encontrada no arquivo.")
                    Return False
                End If

                jsonLocal("ultimaAtualizacao") = saoPauloTime.ToString("yyyy-MM-ddTHH:mm:ss")

                client.DefaultRequestHeaders.Add("X-Master-Key", JSONBinMasterKey)

                File.WriteAllText(portfolioPathFile, jsonLocal.ToString())
                My.Settings.lastUpdate = saoPauloTime.ToString("yyyy-MM-ddTHH:mm:ss")
                My.Settings.Save()
                MessageBox.Show("Removido com sucesso.")
                Return True

            End Using

        Catch ex As Exception
            Debug.WriteLine("Erro em DeleteJSONFromBin: " & ex.Message)
            FormMain.lbDebug.Text = "Erro ao deletar de JSONBin: " & ex.Message
            Return False
        End Try
    End Function

    Public Sub loadFromJSON2ComboGrid(filePath As String, Optional combobox As System.Windows.Forms.ComboBox = Nothing, Optional grid As DataGridView = Nothing)
        Dim jsonData As String = String.Empty
        Try
            jsonData = File.ReadAllText(filePath)
        Catch ex As Exception
            MessageBox.Show("Erro ao ler o arquivo: " & ex.Message)
            Exit Sub
        End Try

        If String.IsNullOrEmpty(jsonData) Then
            MessageBox.Show("O arquivo JSON está vazio.")
            Exit Sub
        End If

        Dim exchanges As List(Of Exchange)
        Try
            exchanges = JsonConvert.DeserializeObject(Of List(Of Exchange))(jsonData)
        Catch ex As Exception
            MessageBox.Show("Erro ao desserializar o JSON: " & ex.Message)
            Exit Sub
        End Try

        If Not IsNothing(combobox) Then
            combobox.DataSource = exchanges
            combobox.ValueMember = "id"
            combobox.DisplayMember = "Name"
        End If

        If Not IsNothing(grid) Then
            grid.DataSource = exchanges
        End If

    End Sub

    Public Sub AddWalletExchangeSymbolToJson(filePath As String, simbol_wallet As String, Optional id As String = "")
        If Not File.Exists(filePath) Then
            MessageBox.Show("O arquivo JSON não foi encontrado.")
            Exit Sub
        End If

        Dim jsonData As String = String.Empty
        Try
            jsonData = File.ReadAllText(filePath)
        Catch ex As Exception
            MessageBox.Show("Erro ao ler o arquivo: " & ex.Message)
            Exit Sub
        End Try

        Dim exchanges As List(Of Exchange)
        Try
            exchanges = JsonConvert.DeserializeObject(Of List(Of Exchange))(jsonData)
        Catch ex As Newtonsoft.Json.JsonException
            Debug.WriteLine("Erro ao desserializar o JSON: " & ex.Message)
            Exit Sub
        End Try

        Dim newID As String
        If id = "" Or Not IsNumeric(id) Then
            newID = simbol_wallet.ToUpper
        Else
            newID = id
        End If

        Dim newExchange As New Exchange With {
            .Name = simbol_wallet.ToUpper,
            .id = newID
        }
        exchanges.Add(newExchange)

        Try
            Dim updatedJson As String = JsonConvert.SerializeObject(exchanges, Formatting.Indented)
            File.WriteAllText(filePath, updatedJson)
            MessageBox.Show("Salvo com sucesso!")
        Catch ex As Exception
            Debug.WriteLine("Erro ao salvar o arquivo JSON: " & ex.Message)
        End Try

    End Sub

    Public Sub RemoveWalletExchangeSymbolFromJson(filePath As String, valueToRemove As String)
        If Not File.Exists(filePath) Then
            MessageBox.Show("O arquivo JSON não foi encontrado.")
            Exit Sub
        End If

        Dim jsonData As String = String.Empty
        Try
            jsonData = File.ReadAllText(filePath)
        Catch ex As Exception
            MessageBox.Show("Erro ao ler o arquivo: " & ex.Message)
            Exit Sub
        End Try

        Dim exchanges As List(Of Exchange)
        Try
            exchanges = JsonConvert.DeserializeObject(Of List(Of Exchange))(jsonData)
        Catch ex As Newtonsoft.Json.JsonException
            MessageBox.Show("Erro ao desserializar o JSON: " & ex.Message)
            Exit Sub
        End Try

        Dim exchangeToRemove As Exchange = exchanges.FirstOrDefault(Function(e) e.Name = valueToRemove)
        If exchangeToRemove IsNot Nothing Then
            exchanges.Remove(exchangeToRemove)

            Try
                Dim updatedJson As String = JsonConvert.SerializeObject(exchanges, Formatting.Indented)
                File.WriteAllText(filePath, updatedJson)
                MessageBox.Show("Removido com sucesso!")
            Catch ex As Exception
                MessageBox.Show("Erro ao salvar o arquivo JSON: " & ex.Message)
            End Try
        Else
            MessageBox.Show("Exchange não encontrado.")
        End If

    End Sub

    Public Class Exchange
        Public Property id As String
        Public Property Name As String
    End Class

    Public Function CheckJSONKey(ByVal jsonKey As String)
        Try
            Dim dados As JObject = JObject.Parse(loadJSONfile)

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
            Dim dados As JObject = JObject.Parse(loadJSONfile)

            If CheckJSONKey(jsonKey) Then
                Return dados(jsonKey)
            End If
        Catch ex As Exception
            MsgBox("Erro ao carregar o arquivo JSON: " & ex.Message)
        End Try

        Return Nothing
    End Function

    Function AppendJSONLocal(ByVal chave As String, ByVal InitialPrice As Decimal, ByVal Qtd As Decimal, ByVal Data As String, ByVal Wallet As String, ByVal lastPrice As Decimal, symbol As String)
        Try
            Dim jsonObject As JObject = JObject.Parse(loadJSONfile)
            Dim newObject As New JObject()

            newObject("InitialPrice") = InitialPrice
            newObject("Qtd") = Qtd
            newObject("Data") = Data
            newObject("Wallet") = Wallet
            newObject("LastPrice") = lastPrice
            newObject("Symbol") = symbol
            jsonObject(chave) = New JArray()

            Dim itemsArray As JArray = CType(jsonObject(chave), JArray)

            itemsArray.Add(newObject)

            bindingSource.DataSource = itemsArray

            File.WriteAllText(portfolioPathFile, jsonObject.ToString())

            Return True

        Catch ex As Exception
            MsgBox("Erro ao salvar o arquivo JSON: " & ex.Message)
            Return False
        End Try

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

    Public Function LoadJSONtoDataGrid(Optional ByVal datagrid As DataGridView = Nothing) As Object
        Try

            Dim jsonObject As JObject = JObject.Parse(loadJSONfile)
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
                        .LastPrice = item.LastPrice,
                        .Symbol = item.Symbol
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
        Catch ex As Exception
            Return False
        End Try

    End Function

    Public Function SomaSe(ByVal valores() As Decimal, ByVal criterios() As String, ByVal criterio As String) As Double
        Dim soma As Decimal = 0

        If valores.Length <> criterios.Length Then
            Throw New ArgumentException("Os arrays de valores e critérios devem ter o mesmo tamanho.")
        End If

        For i As Integer = 0 To valores.Length - 1
            If criterios(i) = criterio Then
                soma += valores(i)
            End If
        Next

        Return soma

    End Function

    Public Sub loadCaixa(datagrid As DataGridView)
        Dim caminhoArquivo As String = portfolioPathFile
        Dim jsonTexto As String = File.ReadAllText(caminhoArquivo)
        Dim jsonObj As JObject = JObject.Parse(jsonTexto)

        datagrid.Rows.Clear()
        datagrid.Columns.Clear()

        datagrid.Columns.Add("Symbol", "Cripto")
        datagrid.Columns.Add("Qtd", "Quantidade")
        datagrid.Columns.Add("Wallet", "Carteira")

        Dim totalUsd As Decimal = 0D

        For Each prop In jsonObj.Properties()
            Dim chave As String = prop.Name

            If chave.ToUpper().Contains("USD") Then
                Dim ativos = prop.Value

                For Each item In ativos
                    Dim qtd As Decimal = item("Qtd")
                    Dim wallet As String = item("Wallet").ToString()
                    Dim symbol As String = item("Symbol").ToString()

                    datagrid.Rows.Add(symbol, qtd, wallet)
                    totalUsd += qtd
                Next
            End If
        Next

        datagrid.Columns(0).HeaderText = "Cripto"
        datagrid.Columns(0).Width = 40
        datagrid.Columns(1).HeaderText = "Qtd"
        datagrid.Columns(1).Width = 80
        datagrid.Columns(2).HeaderText = "Wallet/Cex"
        datagrid.Columns(2).Width = 100

        datagrid.ClearSelection()

    End Sub

    Public Async Function LoadCriptos(datagrid As DataGridView, Optional currencyCollum As String = "USD") As Task(Of Boolean)
        Return Await PortfolioMarketService.LoadAsync(datagrid, currencyCollum)
    End Function

    Public Shared Sub hideMarketDataLabel()
        FormMain.lbLoadFromMarket.Visible = False
        FormMain.TimerBlink.Stop()
        FormMain.Cursor = Cursors.Default
        FormMain.dgPortfolio.Cursor = Cursors.Default
    End Sub

    Public Sub FormatGrid(ByVal datagrid As DataGridView)
        Dim fontsize As Int16 = 12
        Dim fontname As String = "Calibri"

        datagrid.ColumnHeadersHeight = 40
        datagrid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal
        datagrid.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.None
        datagrid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter

        With datagrid.ColumnHeadersDefaultCellStyle
            .BackColor = Color.FromArgb(40, 40, 40)
            .ForeColor = Color.Aqua
            .Font = New Font("Calibri", 10, FontStyle.Italic)
        End With

        Try
            Dim cm As CurrencyManager = CType(FormMain.BindingContext(datagrid.DataSource), CurrencyManager)
            cm.SuspendBinding()
            datagrid.ClearSelection()

            For Each row As DataGridViewRow In datagrid.Rows
                row.Height = 35
                row.Selected = False

                If row.IsNewRow Then Continue For

                Dim symbol = row.Cells(0).Value.ToString().Trim().ToUpper()

                If stablecoins.Contains(symbol) Then
                    row.Visible = False
                End If

                datagrid.Columns(0).Width = 100
                With datagrid.Columns(0).DefaultCellStyle
                    .BackColor = Color.Black
                    .ForeColor = Color.White
                    .Font = New Font(fontname, fontsize, FontStyle.Bold)
                    .Alignment = DataGridViewContentAlignment.MiddleLeft
                End With

                datagrid.Columns(1).HeaderText = "Desempenho"
                datagrid.Columns(1).Width = 80
                With datagrid.Columns(1).DefaultCellStyle
                    .BackColor = Color.Black
                    .ForeColor = Color.WhiteSmoke
                    .Font = New Font(fontname, fontsize, FontStyle.Regular)
                    .Alignment = DataGridViewContentAlignment.MiddleCenter
                End With

                datagrid.Columns(2).HeaderText = "Wallet/Cex"
                datagrid.Columns(2).Width = 110
                With datagrid.Columns(2).DefaultCellStyle
                    .BackColor = Color.Black
                    .ForeColor = Color.WhiteSmoke
                    .Font = New Font(fontname, fontsize, FontStyle.Regular)
                    .Alignment = DataGridViewContentAlignment.MiddleLeft
                End With

                datagrid.Columns(3).Width = 90
                With datagrid.Columns(3).DefaultCellStyle
                    .BackColor = Color.Black
                    .ForeColor = Color.WhiteSmoke
                    .Font = New Font(fontname, fontsize, FontStyle.Italic)
                    .Alignment = DataGridViewContentAlignment.MiddleLeft
                End With

                datagrid.Columns(4).HeaderText = "Quantia entrada/médio"
                datagrid.Columns(4).Width = 95
                With datagrid.Columns(4).DefaultCellStyle
                    .BackColor = Color.Black
                    .ForeColor = Color.WhiteSmoke
                    .Format = "C"
                    .FormatProvider = New CultureInfo("en-US")
                    .Font = New Font(fontname, fontsize, FontStyle.Regular)
                    .Alignment = DataGridViewContentAlignment.MiddleCenter
                End With

                datagrid.Columns(5).HeaderText = "Quantia entrada/médio"
                datagrid.Columns(5).Width = 95
                With datagrid.Columns(5).DefaultCellStyle
                    .BackColor = Color.Black
                    .ForeColor = Color.WhiteSmoke
                    .Format = "C"
                    .FormatProvider = New CultureInfo("pt-BR")
                    .Font = New Font(fontname, fontsize, FontStyle.Regular)
                    .Alignment = DataGridViewContentAlignment.MiddleCenter
                End With

                datagrid.Columns(6).HeaderText = "Preço médio"
                datagrid.Columns(6).Width = 95
                With datagrid.Columns(6).DefaultCellStyle
                    .BackColor = Color.Black
                    .ForeColor = Color.WhiteSmoke
                    .Font = New Font(fontname, fontsize, FontStyle.Bold)
                    .Alignment = DataGridViewContentAlignment.MiddleCenter
                End With

                datagrid.Columns(7).HeaderText = "Preço atual"
                datagrid.Columns(7).Width = 95
                With datagrid.Columns(7).DefaultCellStyle
                    .BackColor = Color.Black
                    .ForeColor = Color.WhiteSmoke
                    .Font = New Font(fontname, fontsize, FontStyle.Bold)
                    .Alignment = DataGridViewContentAlignment.MiddleCenter
                End With

                datagrid.Columns(8).HeaderText = "24 horas"
                datagrid.Columns(8).Width = 70
                With datagrid.Columns(8).DefaultCellStyle
                    .BackColor = Color.Black
                    .ForeColor = Color.WhiteSmoke
                    .Font = New Font(fontname, fontsize, FontStyle.Bold)
                    .Alignment = DataGridViewContentAlignment.MiddleCenter
                End With

                datagrid.Columns(9).HeaderText = "Capitalização de mercado"
                datagrid.Columns(9).Width = 150
                With datagrid.Columns(9).DefaultCellStyle
                    .BackColor = Color.Black
                    .Font = New Font(fontname, fontsize, FontStyle.Bold)
                    .Alignment = DataGridViewContentAlignment.MiddleLeft
                    .Format = "C2"
                    .FormatProvider = New CultureInfo("en-US")
                End With

                datagrid.Columns(10).HeaderText = "Quantia atual"
                datagrid.Columns(10).Width = 95
                With datagrid.Columns(10).DefaultCellStyle
                    .BackColor = Color.Black
                    .ForeColor = Color.WhiteSmoke
                    .Font = New Font(fontname, fontsize, FontStyle.Regular)
                    .Alignment = DataGridViewContentAlignment.MiddleCenter
                End With

                datagrid.Columns(11).HeaderText = "Quantia atual"
                datagrid.Columns(11).Width = 95
                With datagrid.Columns(11).DefaultCellStyle
                    .BackColor = Color.Black
                    .ForeColor = Color.WhiteSmoke
                    .Font = New Font(fontname, fontsize, FontStyle.Regular)
                    .Alignment = DataGridViewContentAlignment.MiddleCenter
                End With

                datagrid.Columns(12).HeaderText = "ROI"
                datagrid.Columns(12).Width = 130
                With datagrid.Columns(12).DefaultCellStyle
                    .BackColor = Color.Black
                    .Format = "C2"
                    .FormatProvider = New CultureInfo("en-US")
                    .Font = New Font(fontname, fontsize, FontStyle.Regular)
                    .Alignment = DataGridViewContentAlignment.MiddleCenter
                End With

                datagrid.Columns(13).HeaderText = "ROI"
                datagrid.Columns(13).Width = 130
                With datagrid.Columns(13).DefaultCellStyle
                    .BackColor = Color.Black
                    .ForeColor = Color.IndianRed
                    .Format = "C2"
                    .FormatProvider = New CultureInfo("pt-BR")
                    .Font = New Font(fontname, fontsize, FontStyle.Regular)
                    .Alignment = DataGridViewContentAlignment.MiddleCenter
                End With

                datagrid.Columns(14).HeaderText = "X"
                datagrid.Columns(14).Width = 50
                With datagrid.Columns(14).DefaultCellStyle
                    .BackColor = Color.Black
                    .ForeColor = Color.Red
                    .Font = New Font(fontname, fontsize, FontStyle.Regular)
                    .Alignment = DataGridViewContentAlignment.MiddleCenter
                End With

                If row.Cells(13).Value > 0 Then
                    row.Cells(0).Style.ForeColor = Color.Lime
                ElseIf row.Cells(13).Value < 0 Then
                    row.Cells(0).Style.ForeColor = Color.LightCoral
                End If

                Select Case CDec(row.Cells(1).Value.ToString.Replace("%", ""))
                    Case > 0
                        row.Cells(1).Style.ForeColor = Color.LightGreen
                    Case < 0
                        row.Cells(1).Style.ForeColor = Color.LightCoral
                    Case Else
                        row.Cells(1).Style.ForeColor = Color.WhiteSmoke
                End Select

                Select Case row.Cells(2).Value.ToString.ToUpper()
                    Case "BINANCE"
                        row.Cells(2).Style.ForeColor = Color.Goldenrod
                    Case "METAMASK"
                        row.Cells(2).Style.ForeColor = Color.DarkOrange
                    Case "TRUSTWALLET"
                        row.Cells(2).Style.ForeColor = Color.LawnGreen
                    Case "PHANTOM"
                        row.Cells(2).Style.ForeColor = Color.MediumPurple
                    Case "BYBIT"
                        row.Cells(2).Style.ForeColor = Color.Gainsboro
                    Case "GATE.IO"
                        row.Cells(2).Style.ForeColor = Color.DodgerBlue
                    Case "MEXC"
                        row.Cells(2).Style.ForeColor = Color.White
                End Select

            Next

            datagrid.ClearSelection()
            cm.ResumeBinding()
        Catch ex As Exception
            Debug.WriteLine(ex.Message)
            cm.ResumeBinding()
        End Try

    End Sub

    Public Function decimalBR(valor As String) As Decimal
        If String.IsNullOrWhiteSpace(valor) Then Return 0D
        Dim texto = valor.Trim()
        Dim resultado As Decimal

        If texto.Contains(",") AndAlso Not texto.Contains(".") Then
            If Decimal.TryParse(texto, NumberStyles.Any, CultureInfo.GetCultureInfo("pt-BR"), resultado) Then Return resultado
        End If

        If texto.Contains(".") AndAlso Not texto.Contains(",") Then
            If Decimal.TryParse(texto, NumberStyles.Any, CultureInfo.InvariantCulture, resultado) Then Return resultado
        End If

        If Not texto.Contains(",") AndAlso Not texto.Contains(".") Then
            If Decimal.TryParse(texto, NumberStyles.Any, CultureInfo.InvariantCulture, resultado) Then Return resultado
        End If

        If texto.Contains(",") AndAlso texto.Contains(".") Then
            If texto.LastIndexOf(","c) > texto.LastIndexOf("."c) Then
                texto = texto.Replace(".", "")
                texto = texto.Replace(",", ".")
            Else
                texto = texto.Replace(",", "")
            End If

            If Decimal.TryParse(texto, NumberStyles.Any, CultureInfo.InvariantCulture, resultado) Then Return resultado
        End If

        Return 0D
    End Function

    Public Function USDformat(valor As Decimal) As String
        Return valor.ToString("C", CultureInfo.GetCultureInfo("en-US"))
    End Function

    Public Function BRLformat(valor As Decimal) As String
        Return valor.ToString("C", CultureInfo.GetCultureInfo("pt-BR"))
    End Function

End Class
