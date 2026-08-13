Imports System.Collections.Concurrent
Imports System.Globalization
Imports System.Net.WebSockets
Imports System.Text
Imports System.Text.Json
Imports System.Threading

Public Class BinanceWebSocket

    Private Const WebSocketBaseUrl As String = "wss://stream.binance.com:9443/stream?streams="

    Private ReadOnly _prices As New ConcurrentDictionary(Of String, Decimal)(StringComparer.OrdinalIgnoreCase)
    Private _socket As ClientWebSocket
    Private _cts As CancellationTokenSource
    Private _receiveTask As Task
    Private _symbols As List(Of String)

    Public Event PriceUpdated(symbol As String, price As Decimal)
    Public Event ConnectionStateChanged(connected As Boolean, message As String)

    Public ReadOnly Property IsConnected As Boolean
        Get
            Return _socket IsNot Nothing AndAlso _socket.State = WebSocketState.Open
        End Get
    End Property

    Public Function TryGetPrice(symbol As String, ByRef price As Decimal) As Boolean
        Return _prices.TryGetValue(symbol.Trim().ToUpperInvariant(), price)
    End Function

    Public Async Function StartAsync(symbols As IEnumerable(Of String)) As Task

        Dim normalized As List(Of String) = symbols.
            Where(Function(s) Not String.IsNullOrWhiteSpace(s)).
            Select(Function(s) s.Trim().ToUpperInvariant()).
            Distinct().
            ToList()

        If normalized.Count = 0 Then
            RaiseEvent ConnectionStateChanged(False, "Nenhum símbolo Binance para assinar.")
            Return
        End If

        Await StopAsync()

        _symbols = normalized
        _cts = New CancellationTokenSource()

        Try
            Await ConnectAsync(_cts.Token)
        Catch ex As OperationCanceledException
            RaiseEvent ConnectionStateChanged(False, "Binance WebSocket cancelado.")
        Catch ex As Exception
            RaiseEvent ConnectionStateChanged(False, "Erro Binance WebSocket: " & ex.Message)
        End Try

    End Function

    Private Async Function ConnectAsync(token As CancellationToken) As Task

        Dim delay As Integer = 1000

        For attempt As Integer = 1 To 5

            token.ThrowIfCancellationRequested()

            Try
                _socket = New ClientWebSocket()
                _socket.Options.KeepAliveInterval = TimeSpan.FromSeconds(20)

                Dim streamUrl As String = BuildCombinedStreamUrl(_symbols)

                Await _socket.ConnectAsync(New Uri(streamUrl), token)

                RaiseEvent ConnectionStateChanged(
                    True,
                    $"Binance WebSocket conectado: {_symbols.Count} símbolos.")

                _receiveTask = ReceiveLoopAsync(token)
                Return

            Catch ex As OperationCanceledException
                If _socket IsNot Nothing Then
                    _socket.Dispose()
                    _socket = Nothing
                End If
                Throw

            Catch ex As Exception
                If _socket IsNot Nothing Then
                    _socket.Dispose()
                    _socket = Nothing
                End If

                If attempt >= 5 Then
                    Throw New WebSocketException(
                        "Não foi possível conectar ao Binance WebSocket após 5 tentativas.", ex)
                End If

                Await Task.Delay(delay, token)
                delay = Math.Min(delay * 2, 8000)
            End Try

        Next

    End Function

    Private Function BuildCombinedStreamUrl(symbols As IEnumerable(Of String)) As String
        Dim streams = symbols.
            Select(Function(s) s.ToLowerInvariant() & "@ticker")

        Return WebSocketBaseUrl & String.Join("/", streams)
    End Function

    Private Async Function ReceiveLoopAsync(token As CancellationToken) As Task

        Dim buffer(8191) As Byte

        Try
            While Not token.IsCancellationRequested AndAlso
                  _socket IsNot Nothing AndAlso
                  _socket.State = WebSocketState.Open

                Using ms As New IO.MemoryStream()

                    Dim result As WebSocketReceiveResult

                    Do
                        result = Await _socket.ReceiveAsync(
                            New ArraySegment(Of Byte)(buffer), token)

                        If result.MessageType = WebSocketMessageType.Close Then
                            Throw New WebSocketException("Binance fechou a conexão.")
                        End If

                        If result.Count > 0 Then
                            ms.Write(buffer, 0, result.Count)
                        End If

                    Loop Until result.EndOfMessage

                    Dim json As String = Encoding.UTF8.GetString(ms.ToArray())
                    ProcessMessage(json)

                End Using

            End While

        Catch ex As OperationCanceledException
            ' Encerramento normal.

        Catch ex As Exception
            RaiseEvent ConnectionStateChanged(
                False,
                "Binance WebSocket desconectado: " & ex.Message)

            If Not token.IsCancellationRequested Then
                _ = ReconnectAsync(token)
            End If
        End Try

    End Function

    Private Sub ProcessMessage(json As String)

        Try
            Using document As JsonDocument = JsonDocument.Parse(json)

                Dim root As JsonElement = document.RootElement
                Dim data As JsonElement

                If Not root.TryGetProperty("data", data) Then
                    Return
                End If

                Dim symbolElement As JsonElement
                Dim priceElement As JsonElement

                If Not data.TryGetProperty("s", symbolElement) Then
                    Return
                End If

                If Not data.TryGetProperty("c", priceElement) Then
                    Return
                End If

                Dim symbol As String = symbolElement.GetString()
                Dim priceText As String = priceElement.GetString()

                If String.IsNullOrWhiteSpace(symbol) OrElse
                   String.IsNullOrWhiteSpace(priceText) Then
                    Return
                End If

                Dim price As Decimal

                If Decimal.TryParse(
                    priceText,
                    NumberStyles.Float,
                    CultureInfo.InvariantCulture,
                    price) Then

                    _prices(symbol) = price
                    RaiseEvent PriceUpdated(symbol, price)

                End If

            End Using

        Catch ex As JsonException
            Debug.WriteLine("Binance WebSocket JSON inválido: " & ex.Message)
        Catch ex As Exception
            Debug.WriteLine("Erro processando Binance WebSocket: " & ex.Message)
        End Try

    End Sub

    Private Async Function ReconnectAsync(token As CancellationToken) As Task

        If token.IsCancellationRequested Then
            Return
        End If

        RaiseEvent ConnectionStateChanged(
            False,
            "Reconectando Binance WebSocket...")

        Try
            Await Task.Delay(2000, token)

            If token.IsCancellationRequested Then
                Return
            End If

            Await ConnectAsync(token)

        Catch ex As OperationCanceledException
            ' Encerramento normal.
        Catch ex As Exception
            RaiseEvent ConnectionStateChanged(
                False,
                "Falha na reconexão Binance: " & ex.Message)
        End Try

    End Function

    Public Async Function StopAsync() As Task

        If _cts IsNot Nothing Then
            _cts.Cancel()
        End If

        Dim receiveTaskLocal As Task = _receiveTask
        _receiveTask = Nothing

        If receiveTaskLocal IsNot Nothing Then
            Try
                Await receiveTaskLocal
            Catch
                ' Ignora erro durante encerramento.
            End Try
        End If

        Dim socketLocal As ClientWebSocket = _socket
        _socket = Nothing

        If socketLocal IsNot Nothing Then
            Try
                If socketLocal.State = WebSocketState.Open OrElse
                   socketLocal.State = WebSocketState.CloseReceived Then

                    Using localCts As New CancellationTokenSource(TimeSpan.FromSeconds(2))
                        Await socketLocal.CloseAsync(
                            WebSocketCloseStatus.NormalClosure,
                            "Encerrando",
                            localCts.Token)
                    End Using
                End If
            Catch
                ' Ignora erros de fechamento.
            Finally
                socketLocal.Dispose()
            End Try
        End If

        If _cts IsNot Nothing Then
            _cts.Dispose()
            _cts = Nothing
        End If

        RaiseEvent ConnectionStateChanged(False, "Binance WebSocket parado.")

    End Function

End Class
