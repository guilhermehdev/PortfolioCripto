Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports System.Text.Json
Imports System.Globalization
Imports System.IO
Imports System.Windows.Forms.VisualStyles.VisualStyleElement
Imports System.Net.Http
Imports System.Text
Imports System.Reflection
Imports System.Diagnostics.Eventing.Reader


Public Class JSON
    Private ReadOnly portfolioPathFile As String = Application.StartupPath & "\JSON\portfolio.json"
    Private ReadOnly bindingSource As New BindingSource()
    Private ReadOnly jsonbin = My.Settings.JSONBinID
    Private ReadOnly JSONBinMasterKey As String = My.Settings.JSONBinMasterKey
    Private ReadOnly saoPauloTimeZone As TimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time")
    Private ReadOnly saoPauloTime As DateTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, saoPauloTimeZone)
    Private ReadOnly JSONBinGet As String = $"{My.Settings.JSONBinURL}/b/{jsonbin}/latest"
    Private ReadOnly JSONBinPut As String = $"{My.Settings.JSONBinURL}/b/{jsonbin}"
    Dim b As New Binance

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

                    ' Fazer o download do JSON caso seja necessário
                    Dim response As HttpResponseMessage = Await client.GetAsync(url)

                    If response.IsSuccessStatusCode Then
                        Dim json As String = Await response.Content.ReadAsStringAsync()
                        Dim jObj As JObject = JObject.Parse(json)
                        Dim conteudoLimpo As JObject = CType(jObj("record"), JObject)

                        ' Atualizar ultimaAtualizacao com a data/hora atual
                        conteudoLimpo("ultimaAtualizacao") = saoPauloTime.ToString("yyyy-MM-dd HH:mm:ss")

                        ' Salvar o conteúdo atualizado no arquivo local
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

                ' Solicitação GET para obter apenas os metadados
                Dim response As HttpResponseMessage = Await client.GetAsync(url)

                If response.IsSuccessStatusCode Then
                    Dim json As String = Await response.Content.ReadAsStringAsync()
                    Dim metaObj As JObject = JObject.Parse(json)

                    ' Verificar se o campo 'ultimaAtualizacao' existe nos metadados
                    If metaObj("record") IsNot Nothing AndAlso metaObj("record")("ultimaAtualizacao") IsNot Nothing Then
                        Dim ultimaAtualizacaoStr As String = metaObj("record")("ultimaAtualizacao").ToString()
                        Dim ultimaAtualizacao As DateTime = DateTime.Parse(ultimaAtualizacaoStr)

                        ' MsgBox(My.Settings.lastUpdate & " " & ultimaAtualizacao.ToString)

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
            Debug.WriteLine("Erro: " & ex.Message)
            Return False
        End Try
    End Function

    Public Async Function AppendJSONToBin(chave As String, InitialPrice As Decimal, Qtd As Decimal, Data As String, Wallet As String, lastPrice As Decimal, symbol As String) As Task(Of Boolean)
        Dim url As String = JSONBinPut
        Dim jsonAtual As JObject = Nothing

        Using client As New HttpClient()
            client.DefaultRequestHeaders.Add("X-Master-Key", JSONBinMasterKey)

            Try
                jsonAtual = JObject.Parse(loadJSONfile)

                ' 2. Atualizar ou adicionar item na chave correspondente
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

                ' 3. Serializar o novo JSON e enviar via PUT
                jsonAtual("ultimaAtualizacao") = saoPauloTime.ToString("yyyy-MM-ddTHH:mm:ss")

                Dim body As String = JsonConvert.SerializeObject(jsonAtual, Formatting.Indented)
                Dim stringContent As New StringContent(body, Encoding.UTF8, "application/json")
                Dim putResponse = Await client.PutAsync(url, stringContent)

                If putResponse.IsSuccessStatusCode Then
                    File.WriteAllText(portfolioPathFile, jsonAtual.ToString())
                    My.Settings.lastUpdate = saoPauloTime.ToString("yyyy-MM-ddTHH:mm:ss")
                    My.Settings.Save()
                    Return True
                Else
                    MsgBox("Erro ao salvar: " & putResponse.StatusCode)
                    Return False
                End If

            Catch ex As Exception
                MsgBox("Erro: " & ex.Message)
                Return False
            End Try
        End Using
    End Function
    Public Async Function DeleteJSONFromBin(ByVal key As String) As Task(Of Boolean)
        Try
            Dim url As String = JSONBinPut

            Using client As New HttpClient()
                Dim record As JObject = JObject.Parse(loadJSONfile)

                If record.ContainsKey(key) Then
                    record.Remove(key)
                Else
                    MessageBox.Show("Chave não encontrada no arquivo.")
                    Return False
                End If

                ' Atualizar o campo ultimaAtualizacao
                record("ultimaAtualizacao") = saoPauloTime.ToString("yyyy-MM-ddTHH:mm:ss")

                ' Atualizando o JSONBin com o novo conteúdo (sem o Content-Type aqui)
                ' Dim putUrl As String = $"https://api.jsonbin.io/v3/b/{jsonbin}"
                'client.DefaultRequestHeaders.Clear()
                client.DefaultRequestHeaders.Add("X-Master-Key", JSONBinMasterKey)

                'Dim content As New StringContent(record.ToString(), Encoding.UTF8, "application/json")
                'Dim putResponse As HttpResponseMessage = Await client.PutAsync(url, content)


                Dim body As String = JsonConvert.SerializeObject(record, Formatting.Indented)
                Dim stringContent As New StringContent(body, Encoding.UTF8, "application/json")
                Dim putResponse = Await client.PutAsync(url, stringContent)

                If putResponse.IsSuccessStatusCode Then
                    MessageBox.Show("Removido com sucesso.")
                    File.WriteAllText(portfolioPathFile, record.ToString())
                    My.Settings.lastUpdate = saoPauloTime.ToString("yyyy-MM-ddTHH:mm:ss")
                    My.Settings.Save()
                    Return True
                Else
                    MessageBox.Show("Erro ao atualizar: " & putResponse.StatusCode.ToString())
                    Return False
                End If

                'Else
                'MessageBox.Show("Erro ao carregar: " & response.StatusCode.ToString())
                'Return False
                'End If
            End Using

        Catch ex As Exception
            MessageBox.Show("Erro: " & ex.Message)
            Return False
        End Try
    End Function


    Public Sub loadFromJSON2ComboGrid(filePath As String, Optional combobox As System.Windows.Forms.ComboBox = Nothing, Optional grid As DataGridView = Nothing)

        'Dim filePath As String = Application.StartupPath & "\JSON\wallets.json"

        ' Tente ler o arquivo e pegar os dados JSON
        Dim jsonData As String = String.Empty
        Try
            jsonData = File.ReadAllText(filePath)
        Catch ex As Exception
            MessageBox.Show("Erro ao ler o arquivo: " & ex.Message)
            Exit Sub
        End Try

        ' Verifique se o conteúdo do arquivo JSON está vazio
        If String.IsNullOrEmpty(jsonData) Then
            MessageBox.Show("O arquivo JSON está vazio.")
            Exit Sub
        End If

        ' Desserializar os dados JSON para uma lista de objetos usando o Newtonsoft.Json
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
        ' Caminho do arquivo JSON
        ' Dim filePath As String = Application.StartupPath & "\JSON\wallets.json"

        ' Verificar se o arquivo existe
        If Not File.Exists(filePath) Then
            MessageBox.Show("O arquivo JSON não foi encontrado.")
            Exit Sub
        End If

        ' Tente ler o arquivo e pegar os dados JSON
        Dim jsonData As String = String.Empty
        Try
            jsonData = File.ReadAllText(filePath)
        Catch ex As Exception
            MessageBox.Show("Erro ao ler o arquivo: " & ex.Message)
            Exit Sub
        End Try

        ' Desserializar o JSON para uma lista de objetos
        Dim exchanges As List(Of Exchange)
        Try
            exchanges = JsonConvert.DeserializeObject(Of List(Of Exchange))(jsonData)
        Catch ex As Newtonsoft.Json.JsonException
            MessageBox.Show("Erro ao desserializar o JSON: " & ex.Message)
            Exit Sub
        End Try

        Dim newID As String
        If id = "" Or Not IsNumeric(id) Then
            newID = simbol_wallet.ToUpper
        Else
            newID = id
        End If
        ' Criar um novo objeto Exchange e adicionar à lista
        Dim newExchange As New Exchange With {
            .Name = simbol_wallet.ToUpper,
            .id = newID
        }
        exchanges.Add(newExchange)

        ' Serializar a lista de volta para JSON
        Try
            Dim updatedJson As String = JsonConvert.SerializeObject(exchanges, Formatting.Indented)

            ' Reescrever o arquivo com os dados atualizados
            File.WriteAllText(filePath, updatedJson)

            MessageBox.Show("Salvo com sucesso!")
        Catch ex As Exception
            MessageBox.Show("Erro ao salvar o arquivo JSON: " & ex.Message)
        End Try

    End Sub

    Public Sub RemoveWalletExchangeSymbolFromJson(filePath As String, valueToRemove As String)
        ' Caminho do arquivo JSON
        'Dim filePath As String = Application.StartupPath & "\JSON\wallets.json"

        ' Verificar se o arquivo existe
        If Not File.Exists(filePath) Then
            MessageBox.Show("O arquivo JSON não foi encontrado.")
            Exit Sub
        End If

        ' Tente ler o arquivo e pegar os dados JSON
        Dim jsonData As String = String.Empty
        Try
            jsonData = File.ReadAllText(filePath)
        Catch ex As Exception
            MessageBox.Show("Erro ao ler o arquivo: " & ex.Message)
            Exit Sub
        End Try

        ' Desserializar o JSON para uma lista de objetos
        Dim exchanges As List(Of Exchange)
        Try
            exchanges = JsonConvert.DeserializeObject(Of List(Of Exchange))(jsonData)
        Catch ex As Newtonsoft.Json.JsonException
            MessageBox.Show("Erro ao desserializar o JSON: " & ex.Message)
            Exit Sub
        End Try

        ' Encontrar e remover o item com o nome correspondente
        Dim exchangeToRemove As Exchange = exchanges.FirstOrDefault(Function(e) e.Name = valueToRemove)
        If exchangeToRemove IsNot Nothing Then
            exchanges.Remove(exchangeToRemove)

            ' Serializar a lista de volta para JSON
            Try
                Dim updatedJson As String = JsonConvert.SerializeObject(exchanges, Formatting.Indented)

                ' Reescrever o arquivo com os dados atualizados
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
    Public Async Function LoadCriptos(datagrid As DataGridView, Optional currencyCollum As String = "USD") As Task

        Dim b As New Binance
        Dim cot As New Cotacao
        Dim gec As New Coingecko
        Dim result = LoadJSONtoDataGrid()
        Dim originalDT = ConvertListToDataTable(Of ItemKey)(DirectCast(result, List(Of ItemKey)))
        Dim allSymbols = originalDT.AsEnumerable().
                 Select(Function(r) r.Field(Of String)("Symbol").ToUpper()).
                 ToList()

        Dim mcapDict = Await gec.CGECKO_MarketData(allSymbols)
        Dim USDBRLprice = Await b.BINANCE_GetUSDTBRL()
        Dim BTCprice As String = Await b.BINANCE_GetCoinsPrice("BTC")
        Dim json As New JSON
        Dim dom As Decimal? = Await Task.Run(Async Function() Await gec.CGECKO_GetBTCDominance())
        Dim profit As Decimal
        Dim initialValue As Decimal
        Dim currValueTotal As Decimal
        Dim cashflow As Decimal
        Dim wallet As String = ""
        Dim currValueUSD As Decimal
        Dim currValueBRL As Decimal
        Dim roi As Decimal
        Dim x As String
        Dim perform As Decimal?
        Dim performWallet As Decimal?
        Dim criptoDic As New Dictionary(Of String, Decimal)
        Dim listAddress As New List(Of String)
        Dim listCriptos As New List(Of String)
        Dim listInitValue As New List(Of Decimal)
        Dim listCurrValue As New List(Of Decimal)
        Dim addressDic As New Dictionary(Of String, Decimal)
        Dim difPrice As Decimal = 0
        Dim total As Decimal
        Dim critoPriceTask As String

        Dim newDT As New DataTable()
        newDT.Columns.Add("Cripto", GetType(String))
        newDT.Columns.Add("Perf", GetType(String))
        newDT.Columns.Add("Wallet", GetType(String))
        newDT.Columns.Add("Qtd", GetType(Decimal))
        newDT.Columns.Add("vlEntradaUSD", GetType(Decimal))
        newDT.Columns.Add("vlEntradaBRL", GetType(Decimal))
        newDT.Columns.Add("precoMedio", GetType(Decimal))
        newDT.Columns.Add("precoAtual", GetType(Decimal))
        newDT.Columns.Add("lastPrice", GetType(String))
        newDT.Columns.Add("marketcap", GetType(Decimal))
        newDT.Columns.Add("vlAtualUSD", GetType(Decimal))
        newDT.Columns.Add("vlAtualBRL", GetType(Decimal))
        newDT.Columns.Add("ROIusd", GetType(Decimal))
        newDT.Columns.Add("ROIbrl", GetType(Decimal))
        newDT.Columns.Add("X", GetType(String))

        Try
            ' Adicionar dados do DataTable original ao novo
            For Each row As DataRow In originalDT.Rows
                Dim newRow As DataRow = newDT.NewRow()
                Dim qtd As Decimal
                Dim initialPrice As Decimal
                initialPrice = cot.decimalBR(row("InitialPrice"))

                Dim symbolUpper = row.Item(6).ToString().ToUpper()
                Dim mData As CoinMarketData = Nothing

                If mcapDict.ContainsKey(symbolUpper) Then
                    mData = mcapDict(symbolUpper)
                Else
                    mData = New CoinMarketData() ' fallback vazio
                End If

                Dim marketcap = mData.MarketCap
                Dim price = mData.Price
                Dim volume = mData.Volume24h
                Dim change = mData.Change24h

                If row("Wallet") = "BINANCE" Then
                    critoPriceTask = Await b.BINANCE_GetCoinsPrice(row.Item(6).ToString.ToUpper)
                Else
                    critoPriceTask = $"{price}|{marketcap}|0"
                End If

                Dim valores() As String = critoPriceTask.Split("|"c)
                Dim preco As String = valores(0)

                If valores(2) > 0 Then
                    qtd = cot.decimalBR(valores(2))
                Else
                    qtd = cot.decimalBR(row("Qtd"))
                End If

                If qtd >= 1 Then
                    qtd = qtd.ToString("N2")
                Else
                    qtd = qtd
                End If

                ' Dim currPrice As Decimal = cot.decimalBR(preco)
                Dim currPrice As Decimal = price
                Dim initialValueUSD As Decimal = (qtd) * cot.decimalBR(row("InitialPrice"))
                Dim initialValueBRL As Decimal = initialValueUSD * USDBRLprice
                wallet = row("Wallet")
                currValueUSD = qtd * currPrice
                currValueBRL = currValueUSD * USDBRLprice
                roi = currValueUSD - initialValueUSD
                perform = (roi / initialValueUSD) * 100
                initialValue += initialValueUSD

                If row("Cripto") = "USDT" Or row("Cripto") = "USDC" Then
                    cashflow += currValueUSD
                Else
                    currValueTotal += currValueUSD
                    profit += roi
                End If

                total = cashflow + currValueTotal

                x = CDec((currValueUSD - initialValueUSD) / initialValueUSD).ToString("N2")

                newRow("Cripto") = row.Item(6).ToString
                newRow("Qtd") = qtd
                newRow("Perf") = $"{perform.Value:F2}%"
                newRow("Wallet") = wallet

                Dim lastPrice As Decimal = cot.decimalBR(row("LastPrice"))
                difPrice = CDec(currPrice.ToString("N2")) - CDec(lastPrice.ToString("N2"))
                Dim priceAction As String
                Dim op As String

                If difPrice > 0 Then
                    priceAction = $"{((difPrice / lastPrice) * 100):F2}%"
                    op = "+"
                ElseIf difPrice = 0 Then
                    priceAction = "--"
                    op = ""
                Else
                    priceAction = $"{((difPrice / lastPrice) * 100):F2}%"
                    op = "-"
                End If

                newRow("vlEntradaUSD") = initialValueUSD
                newRow("vlEntradaBRL") = initialValueBRL
                newRow("precoMedio") = initialPrice
                newRow("precoAtual") = currPrice
                newRow("lastPrice") = op & priceAction.ToString().Replace("-", "")
                newRow("marketcap") = marketcap
                newRow("vlAtualUSD") = (currValueUSD)
                newRow("vlAtualBRL") = (currValueBRL)
                newRow("ROIusd") = (roi)
                newRow("ROIbrl") = (roi * USDBRLprice)

                If x < 1 Then
                    newRow("X") = "0 X"
                Else
                    newRow("X") = $"{x} X"
                End If

                newDT.Rows.Add(newRow)
                'criptoDic.Add(row("Cripto"), (currValueUSD / total) * 100)
                listCriptos.Add(row.Item(6).ToString)
                listAddress.Add(wallet)
                listInitValue.Add(initialValueUSD)
                listCurrValue.Add(currValueUSD)

                AppendJSONLocal(row("Cripto"), initialPrice, qtd, row("Data"), row("Wallet"), currPrice.ToString("C8"), row.Item(6).ToString)

            Next

            Dim arrayInitValue() As Decimal = listInitValue.ToArray
            Dim arrayAddress() As String = listAddress.ToArray
            Dim arraySum() = Array.Empty(Of Object)()
            Dim listSum As New List(Of KeyValuePair(Of String, Decimal))
            'Dim task As String = Await getCriptoData.GetCriptoPrices("BTC")

            Dim res() As String = BTCprice.Split("|"c)
            Dim btc As String = res(0)
            Dim percentCashFlow As Decimal? = (cashflow / total) * 100
            Dim percentInvest As Decimal? = (currValueTotal / total) * 100

            For i = 0 To listCriptos.Count - 1
                criptoDic.Add(listCriptos(i), (listCurrValue(i) / total) * 100)
            Next

            For i = 0 To listAddress.Count - 1
                listSum.Add(New KeyValuePair(Of String, Decimal)(listAddress(i), SomaSe(arrayInitValue, arrayAddress, listAddress(i))))
            Next

            Dim filteredSum = listSum.GroupBy(Function(item) item.Key).Select(Function(grupo) grupo.First()).ToList()

            For Each item In filteredSum
                addressDic.Add(item.Key, item.Value)
            Next

            performWallet = (profit / initialValue) * 100

            FormMain.lbTotalBRL.Visible = True
            FormMain.lbTotalBRL.Text = BRLformat(profit * USDBRLprice)
            If profit > 0 Then
                FormMain.lbTotalBRL.ForeColor = Color.FromArgb(0, 255, 0)
            Else
                FormMain.lbTotalBRL.ForeColor = Color.FromArgb(255, 73, 73)
            End If

            If total < initialValue Then
                FormMain.lbValoresHojeUSD.ForeColor = Color.IndianRed
                FormMain.lbValoresHojeUSD.Text = USDformat(currValueTotal * -1)
            End If

            If (total * USDBRLprice) < (initialValue * USDBRLprice) Then
                FormMain.lbValoresHojeBRL.ForeColor = Color.IndianRed
                FormMain.lbValoresHojeBRL.Text = BRLformat((currValueTotal * USDBRLprice) * -1)
            End If

            If profit < 0 Then
                FormMain.lbRoiUSD.ForeColor = Color.Red
                FormMain.lbRoiUSD.Text = USDformat(profit * -1)
            End If

            FormMain.lbDolar.Text = BRLformat(USDBRLprice)
            FormMain.lbBTC.Text = USDformat(cot.decimalBR(btc))
            FormMain.lbDom.Text = $"{dom.Value:F2}%"
            If performWallet < 0 Then
                FormMain.lbPerformWallet.ForeColor = Color.Red
                FormMain.lbPerformWallet.Text = performWallet * -1
            End If
            FormMain.lbPerformWallet.Text = $"{performWallet.Value:F2}%"
            FormMain.lbTotalEntradaUSD.Text = USDformat(initialValue)
            FormMain.lbTotalEntradaBRL.Text = BRLformat(initialValue * USDBRLprice)
            FormMain.lbValoresHojeUSD.Text = USDformat(total)
            FormMain.lbValoresHojeBRL.Text = BRLformat(total * USDBRLprice)
            FormMain.lbRoiUSD.Text = USDformat(profit)
            FormMain.lbCaixa.Text = USDformat(cashflow)
            FormMain.lbCaixaBRL.Text = BRLformat(cashflow * USDBRLprice)
            FormMain.lbPercentCaixa.Text = $"{percentCashFlow.Value:F2}%"
            FormMain.lbPercentInvestido.Text = $"{percentInvest.Value:F2}%"

            datagrid.DataSource = newDT
            FormatGrid(datagrid)

            FormMain.lbLoadFromMarket.Visible = False
            FormMain.TimerBlink.Stop()
            FormMain.Cursor = Cursors.Default
            FormMain.dgPortfolio.Cursor = Cursors.Default

            FormMain.criptoGraph(criptoDic)
            FormMain.addressGraph(addressDic)

            My.Settings.lastView = Date.Now

            If currencyCollum = "USD" Then
                FormMain.showUSDCollumns()
            ElseIf currencyCollum = "BRL" Then
                FormMain.showBRLCollumns()
            End If

        Catch ex As Exception
            Debug.WriteLine("Erro ao carregar os dados: LoadCripto(): " & ex.Message)

        End Try

    End Function

    Public Function SomaSe(ByVal valores() As Decimal, ByVal criterios() As String, ByVal criterio As String) As Double
        Dim soma As Decimal = 0

        ' Verificar se os arrays têm o mesmo tamanho
        If valores.Length <> criterios.Length Then
            Throw New ArgumentException("Os arrays de valores e critérios devem ter o mesmo tamanho.")
        End If

        ' Iterar pelos arrays e somar os valores que atendem ao critério
        For i As Integer = 0 To valores.Length - 1
            If criterios(i) = criterio Then
                soma += valores(i)
            End If
        Next

        Return soma

    End Function

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
            .BackColor = Color.FromArgb(20, 20, 20)
            .ForeColor = Color.Red
            .Font = New Font(fontname, fontsize, FontStyle.Regular)
            .Alignment = DataGridViewContentAlignment.MiddleCenter
        End With

        datagrid.Columns(2).HeaderText = "Wallet/Cex"
        datagrid.Columns(2).Width = 110
        With datagrid.Columns(2).DefaultCellStyle
            .BackColor = Color.FromArgb(20, 20, 20)
            .Font = New Font(fontname, fontsize, FontStyle.Regular)
            .Alignment = DataGridViewContentAlignment.MiddleLeft
        End With

        datagrid.Columns(3).Width = 90
        With datagrid.Columns(3).DefaultCellStyle
            .BackColor = Color.MidnightBlue
            .ForeColor = Color.Gold
            .Font = New Font(fontname, fontsize, FontStyle.Italic)
            .Alignment = DataGridViewContentAlignment.MiddleLeft
        End With

        datagrid.Columns(4).HeaderText = "Valor entrada/médio"
        datagrid.Columns(4).Width = 95
        With datagrid.Columns(4).DefaultCellStyle
            .BackColor = Color.FromArgb(20, 20, 20)
            .ForeColor = Color.LimeGreen
            .Format = "C"
            .FormatProvider = New CultureInfo("en-US")
            .Font = New Font(fontname, fontsize, FontStyle.Regular)
            .Alignment = DataGridViewContentAlignment.MiddleCenter
        End With

        datagrid.Columns(5).HeaderText = "Valor entrada/médio"
        datagrid.Columns(5).Width = 95
        With datagrid.Columns(5).DefaultCellStyle
            .BackColor = Color.FromArgb(20, 20, 20)
            .ForeColor = Color.DeepSkyBlue
            .Format = "C"
            .FormatProvider = New CultureInfo("pt-BR")
            .Font = New Font(fontname, fontsize, FontStyle.Regular)
            .Alignment = DataGridViewContentAlignment.MiddleCenter
        End With

        datagrid.Columns(6).HeaderText = "Preço médio"
        datagrid.Columns(6).Width = 95
        With datagrid.Columns(6).DefaultCellStyle
            .BackColor = Color.FromArgb(50, 50, 50)
            .ForeColor = Color.LightYellow
            .Font = New Font(fontname, fontsize, FontStyle.Bold)
            .Alignment = DataGridViewContentAlignment.MiddleCenter
        End With

        datagrid.Columns(7).HeaderText = "Preço atual"
        datagrid.Columns(7).Width = 95
        With datagrid.Columns(7).DefaultCellStyle
            .BackColor = Color.FromArgb(50, 50, 50)
            .ForeColor = Color.Yellow
            .Font = New Font(fontname, fontsize, FontStyle.Bold)
            .Alignment = DataGridViewContentAlignment.MiddleCenter
        End With

        datagrid.Columns(8).HeaderText = "Última atualização"
        datagrid.Columns(8).Width = 70
        With datagrid.Columns(8).DefaultCellStyle
            .BackColor = Color.FromArgb(20, 20, 20)
            .Font = New Font(fontname, fontsize, FontStyle.Bold)
            .Alignment = DataGridViewContentAlignment.MiddleCenter
        End With

        datagrid.Columns(9).HeaderText = "Capitalização de mercado"
        datagrid.Columns(9).Width = 150
        With datagrid.Columns(9).DefaultCellStyle
            .BackColor = Color.FromArgb(20, 20, 20)
            .ForeColor = Color.Orange
            .Font = New Font(fontname, fontsize, FontStyle.Bold)
            .Alignment = DataGridViewContentAlignment.MiddleLeft
            .Format = "C2"
            .FormatProvider = New CultureInfo("en-US")
        End With

        datagrid.Columns(10).HeaderText = "Valor atual"
        datagrid.Columns(10).Width = 95
        With datagrid.Columns(10).DefaultCellStyle
            .BackColor = Color.FromArgb(20, 20, 20)
            .ForeColor = Color.Lime
            .Font = New Font(fontname, fontsize, FontStyle.Regular)
            .Alignment = DataGridViewContentAlignment.MiddleCenter
        End With

        datagrid.Columns(11).HeaderText = "Valor atual"
        datagrid.Columns(11).Width = 95
        With datagrid.Columns(11).DefaultCellStyle
            .BackColor = Color.FromArgb(20, 20, 20)
            .ForeColor = Color.DeepSkyBlue
            .Font = New Font(fontname, fontsize, FontStyle.Regular)
            .Alignment = DataGridViewContentAlignment.MiddleCenter
        End With

        datagrid.Columns(12).HeaderText = "ROI"
        datagrid.Columns(12).Width = 130
        With datagrid.Columns(12).DefaultCellStyle
            .BackColor = Color.FromArgb(20, 20, 20)
            .Format = "C2"
            .FormatProvider = New CultureInfo("en-US")
            .Font = New Font(fontname, fontsize, FontStyle.Regular)
            .Alignment = DataGridViewContentAlignment.MiddleCenter
        End With

        datagrid.Columns(13).HeaderText = "ROI"
        datagrid.Columns(13).Width = 130
        With datagrid.Columns(13).DefaultCellStyle
            .BackColor = Color.FromArgb(20, 20, 20)
            .ForeColor = Color.IndianRed
            .Format = "C2"
            .FormatProvider = New CultureInfo("pt-BR")
            .Font = New Font(fontname, fontsize, FontStyle.Regular)
            .Alignment = DataGridViewContentAlignment.MiddleCenter
        End With

        datagrid.Columns(14).HeaderText = "X"
        datagrid.Columns(14).Width = 50
        With datagrid.Columns(14).DefaultCellStyle
            .BackColor = Color.FromArgb(20, 20, 20)
            .ForeColor = Color.Red
            .Font = New Font(fontname, fontsize, FontStyle.Regular)
            .Alignment = DataGridViewContentAlignment.MiddleCenter
        End With

        For Each row As DataGridViewRow In datagrid.Rows

            Select Case CDec(row.Cells(1).Value.ToString.Replace("%", ""))
                Case > 0
                    row.Cells(1).Style.ForeColor = Color.LawnGreen
            End Select

            Select Case row.Cells(2).Value
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

            Select Case CDec(row.Cells(12).Value)
                Case > 0
                    row.Cells(12).Style.ForeColor = Color.Aquamarine
                Case < 0
                    row.Cells(12).Style.ForeColor = Color.LightCoral
            End Select

            Select Case CDec(row.Cells(13).Value)
                Case > 0
                    row.Cells(13).Style.ForeColor = Color.Aqua
                Case < 0
                    row.Cells(13).Style.ForeColor = Color.LightCoral
            End Select


            If row.Cells(6).Value >= 1 Then
                With row.Cells(6)
                    .Style.Format = "C2"
                    .Style.FormatProvider = New CultureInfo("en-US")
                End With
            Else
                With row.Cells(6)
                    .Style.Format = "C6"
                    .Style.FormatProvider = New CultureInfo("en-US")
                End With
            End If

            If row.Cells(7).Value > 1 Then
                With row.Cells(7)
                    .Style.Format = "C2"
                    .Style.FormatProvider = New CultureInfo("en-US")
                End With
            Else
                With row.Cells(7)
                    .Style.Format = "C6"
                    .Style.FormatProvider = New CultureInfo("en-US")
                End With
            End If

            Dim cellValue = row.Cells(8).Value

            If cellValue.ToString.Contains("+") Then
                With row.Cells(8)
                    .Style.ForeColor = Color.LimeGreen
                End With
            ElseIf cellValue.ToString.Contains("-") Then
                With row.Cells(8)
                    .Style.ForeColor = Color.Red
                End With
            Else
                With row.Cells(8)
                    .Style.ForeColor = Color.FromArgb(20, 20, 20)
                End With
            End If

            If row.Cells(10).Value > 1 Then
                With row.Cells(10)
                    .Style.Format = "C2"
                    .Style.FormatProvider = New CultureInfo("en-US")
                End With
            Else
                With row.Cells(10)
                    .Style.Format = "C8"
                    .Style.FormatProvider = New CultureInfo("en-US")
                End With
            End If

            If row.Cells(11).Value > 1 Then
                With row.Cells(11)
                    .Style.Format = "C2"
                    .Style.FormatProvider = New CultureInfo("pt-BR")
                End With
            Else
                With row.Cells(11)
                    .Style.Format = "C8"
                    .Style.FormatProvider = New CultureInfo("pt-BR")
                End With
            End If

            Dim rowBackColor As Color = Color.FromArgb(88, 0, 0)

            If row.Cells(7).Value < row.Cells(6).Value Then
                With row.Cells(7)
                    .Style.ForeColor = Color.IndianRed
                    .Style.BackColor = rowBackColor
                End With
                With row.Cells(8)
                    .Style.BackColor = rowBackColor
                End With
                With row.Cells(9)
                    .Style.ForeColor = Color.Orange
                    .Style.BackColor = rowBackColor
                End With
                With row.Cells(10)
                    .Style.ForeColor = Color.Orange
                    .Style.BackColor = rowBackColor
                End With
                With row.Cells(11)
                    .Style.ForeColor = Color.DarkOrange
                    .Style.BackColor = rowBackColor
                End With
                With row.Cells(0)
                    .Style.BackColor = rowBackColor
                End With
                With row.Cells(1)
                    .Style.ForeColor = Color.IndianRed
                    .Style.BackColor = rowBackColor
                End With
                With row.Cells(2)
                    .Style.BackColor = rowBackColor
                End With
                With row.Cells(3)
                    .Style.BackColor = rowBackColor
                End With
                With row.Cells(4)
                    .Style.BackColor = rowBackColor
                End With
                With row.Cells(5)
                    .Style.BackColor = rowBackColor
                End With
                With row.Cells(6)
                    .Style.BackColor = rowBackColor
                End With
                With row.Cells(12)
                    .Style.BackColor = rowBackColor
                End With
                With row.Cells(13)
                    .Style.BackColor = rowBackColor
                End With
                With row.Cells(14)
                    .Style.BackColor = rowBackColor
                    .Style.ForeColor = rowBackColor
                End With

            End If

            Dim mcap = row.Cells(9).Value
            If mcap <= 100000000 Then
                row.Cells(9).Style.ForeColor = Color.DeepSkyBlue
            ElseIf mcap > 100000000 And mcap <= 300000000 Then
                row.Cells(9).Style.ForeColor = Color.LimeGreen
            ElseIf mcap > 300000000 And mcap <= 600000000 Then
                row.Cells(9).Style.ForeColor = Color.YellowGreen
            ElseIf mcap > 600000000 And mcap <= 1000000000 Then
                row.Cells(9).Style.ForeColor = Color.Yellow
            ElseIf mcap > 1000000000 And mcap <= 10000000000 Then
                row.Cells(9).Style.ForeColor = Color.DarkOrange
            ElseIf mcap > 10000000000 Then
                row.Cells(9).Style.ForeColor = Color.Red
            End If

            row.Height = 35
            datagrid.ClearSelection()
            datagrid.CurrentCell = Nothing
        Next

    End Sub

    Public Function USDformat(ByVal value As Decimal)
        Return value.ToString("C", New CultureInfo("en-US"))
    End Function

    Public Function BRLformat(ByVal value As Decimal)
        Return value.ToString("C", New CultureInfo("pt-BR"))
    End Function

    Public Sub captureRightClick(datagrid As DataGridView, e As MouseEventArgs)
        If e.Button = MouseButtons.Right Then

            Dim hitTest As DataGridView.HitTestInfo = datagrid.HitTest(e.X, e.Y)

            If hitTest.Type = DataGridViewHitTestType.Cell Then
                datagrid.ClearSelection()
                datagrid.Rows(hitTest.RowIndex).Selected = True
            End If
        End If
    End Sub

End Class

Public Class Item
    Public Property InitialPrice As String
    Public Property Qtd As String
    Public Property Data As String
    Public Property Wallet As String
    Public Property LastPrice As String
    Public Property Symbol As Object
End Class

Public Class ItemKey
    Public Property Cripto As String
    Public Property InitialPrice As String
    Public Property Qtd As String
    Public Property Data As String
    Public Property Wallet As String
    Public Property LastPrice As String
    Public Property Symbol As Object
End Class
