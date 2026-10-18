Imports System
Imports System.Collections.Generic
Imports System.Linq

Public NotInheritable Class SpotBotEmaCrossoverStrategy
    Implements ISpotBotStrategy

    Public ReadOnly Property Kind As SpotBotStrategyKind Implements ISpotBotStrategy.Kind
        Get
            Return SpotBotStrategyKind.EmaCrossover
        End Get
    End Property

    Public Function Evaluate(
        symbol As String,
        prices As IReadOnlyList(Of SpotBotPricePoint),
        configuration As SpotBotConfiguration) As SpotBotStrategySignal Implements ISpotBotStrategy.Evaluate

        Dim fastPeriod = Math.Max(2, configuration.EmaFastPeriod)
        Dim slowPeriod = Math.Max(fastPeriod + 1, configuration.EmaSlowPeriod)
        If prices Is Nothing OrElse prices.Count < slowPeriod + 1 Then
            Return HoldSignal("histórico insuficiente para o cruzamento de médias")
        End If

        Dim previousPrices = prices.Take(prices.Count - 1).ToList()
        Dim previousFast = CalculateEma(previousPrices, fastPeriod)
        Dim previousSlow = CalculateEma(previousPrices, slowPeriod)
        Dim currentFast = CalculateEma(prices, fastPeriod)
        Dim currentSlow = CalculateEma(prices, slowPeriod)

        If previousFast <= previousSlow AndAlso currentFast > currentSlow Then
            Return New SpotBotStrategySignal With {
                .Strategy = Kind,
                .Direction = SpotBotSignalDirection.Buy,
                .Confidence = 1D,
                .ObservedValue = currentFast - currentSlow,
                .Reason = $"EMA {fastPeriod} cruzou acima da EMA {slowPeriod}"
            }
        End If

        If previousFast >= previousSlow AndAlso currentFast < currentSlow Then
            Return New SpotBotStrategySignal With {
                .Strategy = Kind,
                .Direction = SpotBotSignalDirection.Sell,
                .Confidence = 1D,
                .ObservedValue = currentFast - currentSlow,
                .Reason = $"EMA {fastPeriod} cruzou abaixo da EMA {slowPeriod}"
            }
        End If

        Return HoldSignal($"EMA {fastPeriod}/{slowPeriod} sem cruzamento")
    End Function

    Private Shared Function CalculateEma(
        prices As IReadOnlyList(Of SpotBotPricePoint),
        period As Integer) As Decimal

        Dim smoothing = 2D / (period + 1D)
        Dim ema = prices(0).Price

        For index = 1 To prices.Count - 1
            ema = (prices(index).Price - ema) * smoothing + ema
        Next

        Return ema
    End Function

    Private Shared Function HoldSignal(reason As String) As SpotBotStrategySignal
        Return New SpotBotStrategySignal With {
            .Strategy = SpotBotStrategyKind.EmaCrossover,
            .Direction = SpotBotSignalDirection.Hold,
            .Reason = reason
        }
    End Function
End Class

Public NotInheritable Class SpotBotRsiStrategy
    Implements ISpotBotStrategy

    Public ReadOnly Property Kind As SpotBotStrategyKind Implements ISpotBotStrategy.Kind
        Get
            Return SpotBotStrategyKind.Rsi
        End Get
    End Property

    Public Function Evaluate(
        symbol As String,
        prices As IReadOnlyList(Of SpotBotPricePoint),
        configuration As SpotBotConfiguration) As SpotBotStrategySignal Implements ISpotBotStrategy.Evaluate

        Dim period = Math.Max(2, configuration.RsiPeriod)
        If prices Is Nothing OrElse prices.Count < period + 1 Then
            Return HoldSignal("histórico insuficiente para o RSI")
        End If

        Dim averageGain = 0D
        Dim averageLoss = 0D
        Dim firstChange = prices.Count - period

        For index = firstChange To prices.Count - 1
            Dim change = prices(index).Price - prices(index - 1).Price
            If change >= 0D Then
                averageGain += change
            Else
                averageLoss -= change
            End If
        Next

        averageGain /= period
        averageLoss /= period

        Dim rsi As Decimal
        If averageLoss = 0D Then
            rsi = If(averageGain = 0D, 50D, 100D)
        Else
            Dim relativeStrength = averageGain / averageLoss
            rsi = 100D - (100D / (1D + relativeStrength))
        End If

        If rsi <= configuration.RsiOversold Then
            Dim confidence = Math.Min(1D, (configuration.RsiOversold - rsi + 1D) / 20D)
            Return New SpotBotStrategySignal With {
                .Strategy = Kind,
                .Direction = SpotBotSignalDirection.Buy,
                .Confidence = confidence,
                .ObservedValue = rsi,
                .Reason = $"RSI {rsi:F2} abaixo de {configuration.RsiOversold:F2}"
            }
        End If

        If rsi >= configuration.RsiOverbought Then
            Dim confidence = Math.Min(1D, (rsi - configuration.RsiOverbought + 1D) / 20D)
            Return New SpotBotStrategySignal With {
                .Strategy = Kind,
                .Direction = SpotBotSignalDirection.Sell,
                .Confidence = confidence,
                .ObservedValue = rsi,
                .Reason = $"RSI {rsi:F2} acima de {configuration.RsiOverbought:F2}"
            }
        End If

        Return HoldSignal($"RSI {rsi:F2} dentro da faixa")
    End Function

    Private Shared Function HoldSignal(reason As String) As SpotBotStrategySignal
        Return New SpotBotStrategySignal With {
            .Strategy = SpotBotStrategyKind.Rsi,
            .Direction = SpotBotSignalDirection.Hold,
            .Reason = reason
        }
    End Function
End Class

Public NotInheritable Class SpotBotBreakoutStrategy
    Implements ISpotBotStrategy

    Public ReadOnly Property Kind As SpotBotStrategyKind Implements ISpotBotStrategy.Kind
        Get
            Return SpotBotStrategyKind.Breakout
        End Get
    End Property

    Public Function Evaluate(
        symbol As String,
        prices As IReadOnlyList(Of SpotBotPricePoint),
        configuration As SpotBotConfiguration) As SpotBotStrategySignal Implements ISpotBotStrategy.Evaluate

        Dim period = Math.Max(2, configuration.BreakoutPeriod)
        If prices Is Nothing OrElse prices.Count < period + 1 Then
            Return HoldSignal("histórico insuficiente para o rompimento")
        End If

        Dim latest = prices(prices.Count - 1).Price
        Dim firstPrevious = prices.Count - period - 1
        Dim lastPrevious = prices.Count - 2
        Dim highest = Decimal.MinValue
        Dim lowest = Decimal.MaxValue

        For index = firstPrevious To lastPrevious
            highest = Math.Max(highest, prices(index).Price)
            lowest = Math.Min(lowest, prices(index).Price)
        Next

        If latest > highest Then
            Return New SpotBotStrategySignal With {
                .Strategy = Kind,
                .Direction = SpotBotSignalDirection.Buy,
                .Confidence = 1D,
                .ObservedValue = latest - highest,
                .Reason = $"preço rompeu a máxima de {period} períodos"
            }
        End If

        If latest < lowest Then
            Return New SpotBotStrategySignal With {
                .Strategy = Kind,
                .Direction = SpotBotSignalDirection.Sell,
                .Confidence = 1D,
                .ObservedValue = latest - lowest,
                .Reason = $"preço rompeu a mínima de {period} períodos"
            }
        End If

        Return HoldSignal($"sem rompimento nos últimos {period} períodos")
    End Function

    Private Shared Function HoldSignal(reason As String) As SpotBotStrategySignal
        Return New SpotBotStrategySignal With {
            .Strategy = SpotBotStrategyKind.Breakout,
            .Direction = SpotBotSignalDirection.Hold,
            .Reason = reason
        }
    End Function
End Class

Public NotInheritable Class SpotBotGridStrategy
    Implements ISpotBotStrategy

    Public ReadOnly Property Kind As SpotBotStrategyKind Implements ISpotBotStrategy.Kind
        Get
            Return SpotBotStrategyKind.Grid
        End Get
    End Property

    Public Function Evaluate(
        symbol As String,
        prices As IReadOnlyList(Of SpotBotPricePoint),
        configuration As SpotBotConfiguration) As SpotBotStrategySignal Implements ISpotBotStrategy.Evaluate

        If prices Is Nothing OrElse prices.Count = 0 Then
            Return HoldSignal("sem preço para a grade")
        End If

        Dim reference = configuration.GridReferencePrice
        If reference <= 0D Then
            reference = prices(0).Price
        End If

        If reference <= 0D Then
            Return HoldSignal("preço de referência inválido")
        End If

        Dim spacing = Math.Max(0.01D, configuration.GridSpacingPercent) / 100D
        Dim latest = prices(prices.Count - 1).Price
        Dim buyLevel = reference * (1D - spacing)
        Dim sellLevel = reference * (1D + spacing)

        If latest <= buyLevel Then
            Return New SpotBotStrategySignal With {
                .Strategy = Kind,
                .Direction = SpotBotSignalDirection.Buy,
                .Confidence = 1D,
                .ObservedValue = latest - buyLevel,
                .Reason = $"preço atingiu a faixa de compra da grade ({buyLevel:F8})"
            }
        End If

        If latest >= sellLevel Then
            Return New SpotBotStrategySignal With {
                .Strategy = Kind,
                .Direction = SpotBotSignalDirection.Sell,
                .Confidence = 1D,
                .ObservedValue = latest - sellLevel,
                .Reason = $"preço atingiu a faixa de venda da grade ({sellLevel:F8})"
            }
        End If

        Return HoldSignal("preço dentro da faixa da grade")
    End Function

    Private Shared Function HoldSignal(reason As String) As SpotBotStrategySignal
        Return New SpotBotStrategySignal With {
            .Strategy = SpotBotStrategyKind.Grid,
            .Direction = SpotBotSignalDirection.Hold,
            .Reason = reason
        }
    End Function
End Class
