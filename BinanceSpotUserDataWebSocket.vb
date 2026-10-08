Imports System.Net.Http
Imports System.Net.WebSockets
Imports System.Text
Imports System.Text.Json
Imports System.Threading

''' <summary>
''' User Data Stream privado da conta Spot da Binance.
''' Os eventos servem como sinal de alteração; os saldos continuam sendo
''' lidos pela chamada REST assinada, que é a fonte autoritativa da conta.
''' </summary>
Public Class BinanceSpotUserDataWebSocket
    Private Const ListenKeyUrl As String = "https://api.binance.com/api/v3/userDataStream"
    Private Const WebSocketBaseUrl As String = "wss://stream.binance.com:9443/ws/"
    Private Shared ReadOnly Http As New HttpClient()

    Private _socket As ClientWebSocket
    Private _cts As CancellationTokenSource
    Private _supervisorTask As Task
    Private _listenKey As String = String.Empty

    Public Event DataUpdated()
    Public Event ConnectionStateChanged(connected As Boolean, message As String)

    Public Async Function StartAsync() As Task
        Await StopAsync()
        _cts = New CancellationTokenSource()
        _supervisorTask = ConnectionSupervisorAsync(_cts.Token)
    End Function

    Private Async Function ConnectionSupervisorAsync(token As CancellationToken) As Task
        Dim delay As Integer = 2000

        While Not token.IsCancellationRequested
            Dim listenKeyToDelete As String = String.Empty
            Try
                _listenKey = Await CreateListenKeyAsync(token)
                Await ConnectAndReceiveAsync(_listenKey, token)
                delay = 2000
            Catch ex As OperationCanceledException
                Exit While
            Catch ex As Exception
                If Not token.IsCancellationRequested Then
                    RaiseEvent ConnectionStateChanged(False, "Binance Spot User Data: " & ex.Message)
                End If
            Finally
                CloseCurrentSocket()
                listenKeyToDelete = _listenKey
                _listenKey = String.Empty
            End Try

            If Not String.IsNullOrWhiteSpace(listenKeyToDelete) Then
                Await TryDeleteListenKeyAsync(listenKeyToDelete)
            End If

            If Not token.IsCancellationRequested Then
                Try
                    Await Task.Delay(delay, token)
                Catch ex As OperationCanceledException
                    Exit While
                End Try
                delay = Math.Min(delay * 2, 15000)
            End If
        End While
    End Function

    Private Async Function CreateListenKeyAsync(token As CancellationToken) As Task(Of String)
        Using request As New HttpRequestMessage(HttpMethod.Post, ListenKeyUrl)
            request.Headers.Add("X-MBX-APIKEY", My.Settings.BinanceAPIKey)
            Using response = Await Http.SendAsync(request, token)
                Dim body = Await response.Content.ReadAsStringAsync(token)
                response.EnsureSuccessStatusCode()
                Using document = JsonDocument.Parse(body)
                    Dim listenKeyElement As JsonElement
                    If document.RootElement.TryGetProperty("listenKey", listenKeyElement) Then
                        Dim listenKey = listenKeyElement.GetString()
                        If Not String.IsNullOrWhiteSpace(listenKey) Then
                            Return listenKey
                        End If
                    End If
                End Using
                Throw New InvalidOperationException("A Binance não retornou o listenKey do Spot.")
            End Using
        End Using
    End Function

    Private Async Function ConnectAndReceiveAsync(listenKey As String, token As CancellationToken) As Task
        CloseCurrentSocket()
        _socket = New ClientWebSocket()
        _socket.Options.KeepAliveInterval = TimeSpan.FromSeconds(20)
        Await _socket.ConnectAsync(New Uri(WebSocketBaseUrl & Uri.EscapeDataString(listenKey)), token)
        RaiseEvent ConnectionStateChanged(True, "Binance Spot User Data conectado.")

        Dim keepAliveTask As Task = Nothing
        Using keepAliveCts = CancellationTokenSource.CreateLinkedTokenSource(token)
            keepAliveTask = KeepAliveLoopAsync(listenKey, keepAliveCts.Token)
            Try
                Await ReceiveLoopAsync(token)
            Finally
                keepAliveCts.Cancel()
            End Try
        End Using

        If keepAliveTask IsNot Nothing Then
            Try
                Await Task.WhenAny(keepAliveTask, Task.Delay(TimeSpan.FromSeconds(2)))
            Catch
            End Try
        End If
    End Function

    Private Async Function KeepAliveLoopAsync(listenKey As String, token As CancellationToken) As Task
        While Not token.IsCancellationRequested
            Await Task.Delay(TimeSpan.FromMinutes(30), token)
            If token.IsCancellationRequested Then Return

            Using request As New HttpRequestMessage(HttpMethod.Put, ListenKeyUrl)
                request.Headers.Add("X-MBX-APIKEY", My.Settings.BinanceAPIKey)
                request.Content = New StringContent(
                    "listenKey=" & Uri.EscapeDataString(listenKey),
                    Encoding.UTF8,
                    "application/x-www-form-urlencoded")
                Using response = Await Http.SendAsync(request, token)
                    response.EnsureSuccessStatusCode()
                End Using
            End Using
        End While
    End Function

    Private Async Function ReceiveLoopAsync(token As CancellationToken) As Task
        Dim buffer(8191) As Byte

        While Not token.IsCancellationRequested AndAlso
              _socket IsNot Nothing AndAlso
              _socket.State = WebSocketState.Open

            Using ms As New IO.MemoryStream()
                Dim result As WebSocketReceiveResult = Nothing
                Do
                    result = Await _socket.ReceiveAsync(New ArraySegment(Of Byte)(buffer), token)
                    If result.MessageType = WebSocketMessageType.Close Then
                        Throw New WebSocketException("Binance fechou o User Data Stream do Spot.")
                    End If
                    If result.Count > 0 Then
                        ms.Write(buffer, 0, result.Count)
                    End If
                Loop Until result.EndOfMessage

                ProcessMessage(Encoding.UTF8.GetString(ms.ToArray()))
            End Using
        End While
    End Function

    Private Sub ProcessMessage(json As String)
        Try
            Using document = JsonDocument.Parse(json)
                Dim root = document.RootElement
                Dim eventElement As JsonElement
                If Not root.TryGetProperty("e", eventElement) Then Return

                Dim eventName = eventElement.GetString()
                If String.Equals(eventName, "listenKeyExpired", StringComparison.OrdinalIgnoreCase) Then
                    Throw New WebSocketException("O listenKey do Spot expirou.")
                End If

                Select Case eventName
                    Case "outboundAccountPosition", "balanceUpdate", "executionReport", "externalLockUpdate"
                        RaiseEvent DataUpdated()
                End Select
            End Using
        Catch ex As WebSocketException
            Throw
        Catch ex As Exception
            Debug.WriteLine("Erro processando User Data do Binance Spot: " & ex.Message)
        End Try
    End Sub

    Private Async Function TryDeleteListenKeyAsync(listenKey As String) As Task
        Try
            Using request As New HttpRequestMessage(HttpMethod.Delete, ListenKeyUrl)
                request.Headers.Add("X-MBX-APIKEY", My.Settings.BinanceAPIKey)
                request.Content = New StringContent(
                    "listenKey=" & Uri.EscapeDataString(listenKey),
                    Encoding.UTF8,
                    "application/x-www-form-urlencoded")
                Await Http.SendAsync(request)
            End Using
        Catch
        End Try
    End Function

    Private Sub CloseCurrentSocket()
        Dim socket = _socket
        _socket = Nothing
        If socket Is Nothing Then Return
        Try
            socket.Abort()
        Catch
        End Try
        Try
            socket.Dispose()
        Catch
        End Try
    End Sub

    Public Async Function StopAsync() As Task
        Dim cts = _cts
        Dim supervisor = _supervisorTask
        _cts = Nothing
        _supervisorTask = Nothing

        If cts IsNot Nothing Then cts.Cancel()
        CloseCurrentSocket()

        If supervisor IsNot Nothing Then
            Try
                Await Task.WhenAny(supervisor, Task.Delay(TimeSpan.FromSeconds(2)))
            Catch
            End Try
        End If

        If cts IsNot Nothing Then cts.Dispose()
    End Function
End Class
