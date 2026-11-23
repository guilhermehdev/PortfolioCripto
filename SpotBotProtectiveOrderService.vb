Imports System.Globalization
Imports Newtonsoft.Json.Linq

''' <summary>
''' Camada única para colocar proteções spot na exchange. O serviço é
''' deliberadamente opt-in e usa uma chave idempotente para impedir duas
''' proteções iguais em paralelo.
''' </summary>
Public NotInheritable Class SpotBotProtectiveOrderRequest
    Public Property Exchange As String = "BINANCE"
    Public Property Symbol As String = String.Empty
    Public Property Quantity As Decimal
    Public Property StopPrice As Decimal
    Public Property TakeProfitPrice As Decimal
    Public Property StopLimitPrice As Decimal
    Public Property IntentKey As String = String.Empty
    Public Property RequireExplicitApproval As Boolean = True
End Class

Public NotInheritable Class SpotBotProtectiveOrderResult
    Public Property Exchange As String = String.Empty
    Public Property Symbol As String = String.Empty
    Public Property StopOrderResponse As String = String.Empty
    Public Property TakeProfitOrderResponse As String = String.Empty
    Public Property OcoResponse As String = String.Empty
    Public Property WasSkipped As Boolean
End Class

Public NotInheritable Class SpotBotProtectiveOrderService
    Private ReadOnly _binance As Binance
    Private ReadOnly _gate As Gateio
    Private ReadOnly _deduplicator As SpotBotOrderIntentDeduplicator

    Public Sub New(binance As Binance, gate As Gateio, deduplicator As SpotBotOrderIntentDeduplicator)
        _binance = binance
        _gate = gate
        _deduplicator = If(deduplicator, New SpotBotOrderIntentDeduplicator())
    End Sub

    Public Async Function PlaceAsync(
        request As SpotBotProtectiveOrderRequest,
        enabled As Boolean,
        explicitApproval As Boolean) As Task(Of SpotBotProtectiveOrderResult)

        If request Is Nothing Then Throw New ArgumentNullException(NameOf(request))
        If Not enabled Then
            Return New SpotBotProtectiveOrderResult With {.WasSkipped = True}
        End If
        If request.RequireExplicitApproval AndAlso Not explicitApproval Then
            Throw New InvalidOperationException("A proteção real exige aprovação explícita para esta ordem.")
        End If
        If request.Quantity <= 0D OrElse request.StopPrice <= 0D OrElse request.TakeProfitPrice <= 0D Then
            Throw New ArgumentException("Quantidade, stop e alvo devem ser maiores que zero.")
        End If

        Dim exchange = If(request.Exchange, String.Empty).Trim().ToUpperInvariant()
        Dim symbol = If(request.Symbol, String.Empty).Trim().ToUpperInvariant()
        Dim key = If(String.IsNullOrWhiteSpace(request.IntentKey),
                     $"{exchange}|{symbol}|{request.Quantity.ToString("0.########", CultureInfo.InvariantCulture)}|" &
                     $"{request.StopPrice.ToString("0.########", CultureInfo.InvariantCulture)}|{request.TakeProfitPrice.ToString("0.########", CultureInfo.InvariantCulture)}",
                     request.IntentKey.Trim())
        If Not _deduplicator.TryBegin(key) Then
            Throw New InvalidOperationException("A proteção foi ignorada porque a mesma intenção já está em andamento.")
        End If

        Try
            Dim result As New SpotBotProtectiveOrderResult With {.Exchange = exchange, .Symbol = symbol}
            Select Case exchange
                Case "BINANCE"
                    If _binance Is Nothing Then Throw New InvalidOperationException("Cliente Binance indisponível.")
                    result.OcoResponse = Await _binance.BINANCE_CreateSpotProtectionOcoAsync(
                        symbol,
                        request.Quantity,
                        request.TakeProfitPrice,
                        request.StopPrice,
                        request.StopLimitPrice)
                Case "GATE", "GATE.IO"
                    If _gate Is Nothing Then Throw New InvalidOperationException("Cliente Gate indisponível.")
                    Dim stopResponse As String = Nothing
                    Dim gateFailure As Exception = Nothing
                    Try
                        stopResponse = Await _gate.GATE_CreateSpotPriceTriggeredOrderAsync(
                            symbol,
                            "sell",
                            request.Quantity,
                            request.StopPrice,
                            "<=",
                            request.StopLimitPrice)
                        result.StopOrderResponse = stopResponse
                        Dim takeResponse = Await _gate.GATE_CreateSpotPriceTriggeredOrderAsync(
                            symbol,
                            "sell",
                            request.Quantity,
                            request.TakeProfitPrice,
                            ">=",
                            request.TakeProfitPrice)
                        result.TakeProfitOrderResponse = takeResponse
                    Catch ex As Exception
                        gateFailure = ex
                    End Try
                    If gateFailure IsNot Nothing Then
                        Dim stopId = TryReadGatePriceOrderId(stopResponse)
                        If Not String.IsNullOrWhiteSpace(stopId) Then
                            Try
                                Await _gate.GATE_CancelSpotPriceTriggeredOrderAsync(stopId)
                            Catch
                            End Try
                        End If
                        Throw gateFailure
                    End If
                Case Else
                    Throw New ArgumentException("Exchange de proteção não suportada: " & exchange)
            End Select
            Return result
        Finally
            _deduplicator.Complete(key)
        End Try
    End Function

    Private Shared Function TryReadGatePriceOrderId(response As String) As String
        If String.IsNullOrWhiteSpace(response) Then Return String.Empty
        Try
            Dim item = JObject.Parse(response)
            Return If(item("id")?.ToString(), String.Empty)
        Catch
            Return String.Empty
        End Try
    End Function
End Class
