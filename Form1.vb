
Imports System.Net.Http
Imports System.Security.Cryptography
Imports System.Text
Imports Newtonsoft.Json.Linq
Imports System.Globalization


Public Class Form1

    Private Const GATE_API_KEY As String = "98db4f63584bb557c0e5d162324edf43"
    Private Const GATE_API_SECRET As String = "6c7a7c9aa890301a5078d09adfeb2d5e6eef4c81a7da556232d5b6e8d81685ec"
    Private Const GATE_HOST As String = "https://api.gateio.ws"


    Public Async Function TestarGateAPI(symbol As String) As Task(Of String)
        Try
            Dim endpoint = "/api/v4/spot/accounts"
            Dim url = GATE_HOST & endpoint
            Dim timestamp = CInt(DateTimeOffset.UtcNow.ToUnixTimeSeconds()).ToString()
            Dim method = "GET"
            Dim query = "" ' Para /api/v4/spot/accounts, a query string é vazia
            Dim body = ""  ' Para requisições GET, o corpo é vazio

            Dim debugLog As New StringBuilder()
            debugLog.AppendLine("🕒 TIMESTAMP Gerado = " & timestamp)
            debugLog.AppendLine("🔐 GATE_API_KEY = " & GATE_API_KEY)
            ' Por segurança, evite logar o API Secret em produção.
            ' debugLog.AppendLine("🔐 GATE_API_SECRET = " & GATE_API_SECRET)


            ' 1. Gerar o hash SHA512 do corpo da requisição (mesmo que vazio)
            Dim bodyHash As String
            Using sha512 As New SHA512Managed()
                Dim bodyBytes = Encoding.UTF8.GetBytes(body)
                Dim hashBytesValue = sha512.ComputeHash(bodyBytes)
                bodyHash = BitConverter.ToString(hashBytesValue).Replace("-", "").ToLower()
            End Using
            debugLog.AppendLine("🔑 HASHED_BODY = " & bodyHash)


            ' 2. Construir a stringToSign no formato correto:
            ' METHOD\nENDPOINT\nQUERY_STRING\nHASHED_BODY\nTIMESTAMP
            Dim stringToSign = $"{method}" & vbLf &
                               $"{endpoint}" & vbLf &
                               $"{query}" & vbLf &
                               $"{bodyHash}" & vbLf &
                               $"{timestamp}"

            debugLog.AppendLine("🧾 stringToSign (Formato Corrigido):")
            ' Apresenta a stringToSign com \n visíveis para facilitar a depuração
            debugLog.AppendLine(stringToSign.Replace(vbLf, "\n"))


            ' NÃO faça hex-decode do secret – use o ASCII puro
            Dim secretClean = GATE_API_SECRET.Trim()
            Dim signBytes = New HMACSHA512(Encoding.UTF8.GetBytes(secretClean)).
                              ComputeHash(Encoding.UTF8.GetBytes(stringToSign))
            Dim signature = BitConverter.ToString(signBytes).Replace("-", "").ToLower()

            debugLog.AppendLine("🔏 SIGN Gerado:")
            debugLog.AppendLine(signature)
            debugLog.AppendLine("🌍 URL Chamada: " & url)

            Dim handler As New HttpClientHandler()

            ' Definir os protocolos SSL/TLS. Tls12 é amplamente suportado.
            ' Tls13 é mais novo e mais seguro, mas pode não ser suportado em todos os ambientes .NET Framework mais antigos.
            ' Se System.Security.Authentication.SslProtocols.Tls13 não for reconhecido, use apenas Tls12.
            Try
                ' Tenta usar Tls12 e Tls13 se disponível
                handler.SslProtocols = System.Security.Authentication.SslProtocols.Tls12 Or System.Security.Authentication.SslProtocols.Tls13
            Catch exNoSuchMember As MissingMemberException ' Ou um erro mais específico se Tls13 não existir
                ' Fallback para Tls12 se Tls13 não for suportado pelo framework/OS
                handler.SslProtocols = System.Security.Authentication.SslProtocols.Tls12
            Catch ex As Exception ' Captura genérica para outros possíveis problemas com SslProtocols
                ' Fallback para Tls12 como um padrão seguro
                handler.SslProtocols = System.Security.Authentication.SslProtocols.Tls12
                debugLog.AppendLine("⚠️ Aviso: Problema ao definir SslProtocols para Tls13, usando Tls12. Erro: " & ex.Message)
            End Try

            Using client As New HttpClient(handler)
                client.DefaultRequestHeaders.Clear()
                client.DefaultRequestHeaders.Add("KEY", GATE_API_KEY)
                client.DefaultRequestHeaders.Add("Timestamp", timestamp)
                client.DefaultRequestHeaders.Add("SIGN", signature)
                client.DefaultRequestHeaders.Accept.Add(New System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"))
                ' client.DefaultRequestHeaders.Add("Content-Type", "application/json") ' Geralmente não necessário para GET sem corpo, mas algumas APIs podem exigir.
                client.DefaultRequestHeaders.Add("User-Agent", "VBTest/1.0")

                debugLog.AppendLine("📤 HEADERS ENVIADOS:")
                debugLog.AppendLine("KEY: " & GATE_API_KEY)
                debugLog.AppendLine("Timestamp: " & timestamp)
                debugLog.AppendLine("SIGN: " & signature)

                Dim response = Await client.GetAsync(url)
                Dim responseBody = Await response.Content.ReadAsStringAsync()

                debugLog.AppendLine("📡 Código HTTP: " & CInt(response.StatusCode))
                debugLog.AppendLine("📥 Corpo da Resposta:")
                debugLog.AppendLine(responseBody)

                If response.IsSuccessStatusCode Then
                    If Not String.IsNullOrEmpty(responseBody) Then
                        Try
                            Dim json = JArray.Parse(responseBody)
                            For Each item In json
                                If item("currency") IsNot Nothing AndAlso item("currency").ToString().ToUpper() = symbol.ToUpper() Then
                                    Dim free = If(item("available") IsNot Nothing, item("available").ToString(), "N/A")
                                    Dim locked = If(item("locked") IsNot Nothing, item("locked").ToString(), "N/A")
                                    debugLog.AppendLine($"✅ {symbol} = {free} disponível + {locked} bloqueado")
                                End If
                            Next
                        Catch jsonEx As Exception
                            debugLog.AppendLine("⚠️ ERRO ao processar JSON: " & jsonEx.Message)
                        End Try
                    Else
                        debugLog.AppendLine("✅ Resposta com sucesso, mas corpo vazio.")
                    End If
                ElseIf CInt(response.StatusCode) = 401 Then
                    debugLog.AppendLine("❌ ERRO DE AUTENTICAÇÃO (401): Verifique novamente a chave da API, o segredo, a construção da string de assinatura e a sincronia do timestamp.")
                    debugLog.AppendLine("   A stringToSign esperada para esta requisição (query e body vazios) seria algo como:")
                    debugLog.AppendLine($"   GET\n{endpoint}\n\n{bodyHash}\n{timestamp}")
                End If

                Return debugLog.ToString()
            End Using

        Catch ex As Exception
            Dim errorMsg As String = "❌ ERRO GERAL: " & ex.Message
            If ex.InnerException IsNot Nothing Then
                errorMsg &= vbCrLf & "   Inner Exception: " & ex.InnerException.Message
            End If
            Return errorMsg
        End Try
    End Function


End Class
