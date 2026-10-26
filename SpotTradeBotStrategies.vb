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

        ' O motor fornece fechamentos de candles de um minuto para o RSI.
        ' A suavização de Wilder evita que um único tick altere o sinal inteiro.
        Dim averageGain = 0D
        Dim averageLoss = 0D
        Dim firstChange = 1

        For index = firstChange To firstChange + period - 1
            Dim change = prices(index).Price - prices(index - 1).Price
            If change >= 0D Then
                averageGain += change
            Else
                averageLoss -= change
            End If
        Next

        averageGain /= period
        averageLoss /= period

        For index = firstChange + period To prices.Count - 1
            Dim change = prices(index).Price - prices(index - 1).Price
            Dim gain = If(change > 0D, change, 0D)
            Dim loss = If(change < 0D, -change, 0D)
            averageGain = ((averageGain * (period - 1)) + gain) / period
            averageLoss = ((averageLoss * (period - 1)) + loss) / period
        Next

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

Public NotInheritable Class SpotBotWavePullbackStrategy
    Implements ISpotBotCandleStrategy

    Public ReadOnly Property Kind As SpotBotStrategyKind Implements ISpotBotCandleStrategy.Kind
        Get
            Return SpotBotStrategyKind.WavePullback
        End Get
    End Property

    Public Function Evaluate(
        symbol As String,
        prices As IReadOnlyList(Of SpotBotPricePoint),
        configuration As SpotBotConfiguration) As SpotBotStrategySignal Implements ISpotBotStrategy.Evaluate

        Return HoldSignal("aguardando candle fechado da onda")
    End Function

    Public Function EvaluateCandles(
        symbol As String,
        candles As IReadOnlyList(Of SpotBotPricePoint),
        configuration As SpotBotConfiguration) As SpotBotStrategySignal Implements ISpotBotCandleStrategy.EvaluateCandles

        Dim period = Math.Max(5, configuration.WaveEmaPeriod)
        Dim lookback = Math.Max(2, configuration.WaveSlopeLookback)
        If candles Is Nothing OrElse candles.Count < period + lookback + 2 Then
            Return HoldSignal("histórico insuficiente para a onda")
        End If

        Dim previous = candles(candles.Count - 2)
        Dim confirmation = candles(candles.Count - 1)
        Dim emaNow = CalculateEma(candles, candles.Count - 1, period)
        Dim emaPast = CalculateEma(candles, candles.Count - 1 - lookback, period)
        Dim averageRange = CalculateAverageRange(candles, candles.Count - 1, lookback)

        If averageRange <= 0D Then
            Return HoldSignal("range médio inválido para a onda")
        End If

        Dim priceDelta = emaNow - emaPast
        Dim slopeDegrees = CDec(Math.Atan2(CDbl(Math.Abs(priceDelta)), CDbl(averageRange * lookback)) * 180D / Math.PI)
        Dim tolerance = Math.Max(0D, configuration.WaveTouchTolerancePercent) / 100D
        Dim bandHalfWidth = averageRange
        Dim bandUpper = emaPast + bandHalfWidth
        Dim bandLower = emaPast - bandHalfWidth
        Dim touchedBand = previous.Low <= bandUpper * (1D + tolerance) AndAlso
                          previous.High >= bandLower * (1D - tolerance)

        If Not touchedBand Then
            Return HoldSignal("preço não tocou a banda da onda")
        End If

        Dim range = Math.Max(previous.Range, confirmation.Range)
        If range <= 0D Then
            Return HoldSignal("candle de confirmação sem range")
        End If

        Dim bodyRatio = confirmation.Body / range
        Dim bullishConfirmation = confirmation.IsBullish AndAlso
                                   bodyRatio >= 0.5D AndAlso
                                   confirmation.Close > previous.High
        Dim bearishConfirmation = confirmation.IsBearish AndAlso
                                  bodyRatio >= 0.5D AndAlso
                                  confirmation.Close < previous.Low

        Dim rsi = CalculateRsi(candles, configuration.RsiPeriod)
        Dim highSlope = priceDelta > 0D AndAlso slopeDegrees >= configuration.WaveMinimumSlopeDegrees
        Dim lowSlope = priceDelta < 0D AndAlso slopeDegrees >= configuration.WaveMinimumSlopeDegrees
        Dim structureLookback = Math.Max(5, configuration.WaveStructureLookback)
        Dim structureFirst = Math.Max(0, candles.Count - structureLookback - 2)
        Dim structureLast = candles.Count - 3
        Dim priorResistance = Decimal.MinValue
        Dim priorSupport = Decimal.MaxValue

        For index = structureFirst To structureLast
            priorResistance = Math.Max(priorResistance, candles(index).High)
            priorSupport = Math.Min(priorSupport, candles(index).Low)
        Next

        If highSlope AndAlso bullishConfirmation Then
            If rsi >= configuration.RsiOverbought Then
                Return HoldSignal($"alta confirmada, mas RSI {rsi:F2} sobrecomprado")
            End If

            Dim stopPrice = Math.Min(previous.Low, bandLower)
            Dim risk = confirmation.Close - stopPrice
            Dim room = If(priorResistance > confirmation.Close,
                          priorResistance - confirmation.Close,
                          risk * configuration.WaveMinimumRiskReward)
            If risk <= 0D OrElse room < risk * configuration.WaveMinimumRiskReward Then
                Return HoldSignal("compra bloqueada: resistência próxima ou risco/retorno menor que 2:1")
            End If

            Dim strength = If(slopeDegrees >= configuration.WaveStrongSlopeDegrees, "alta forte", "alta")
            Return New SpotBotStrategySignal With {
                .Strategy = Kind,
                .Direction = SpotBotSignalDirection.Buy,
                .Confidence = If(slopeDegrees >= configuration.WaveStrongSlopeDegrees, 1D, 0.8D),
                .ObservedValue = slopeDegrees,
                .Reason = $"onda {strength} ({slopeDegrees:F1} graus), toque e rompimento da máxima; RSI {rsi:F2}; RR {(room / risk):F2}:1",
                .StopPrice = stopPrice,
                .TakeProfitPrice = confirmation.Close + (risk * configuration.WaveMinimumRiskReward)
            }
        End If

        If lowSlope AndAlso bearishConfirmation Then
            If rsi <= configuration.RsiOversold Then
                Return HoldSignal($"baixa confirmada, mas RSI {rsi:F2} sobrevendido")
            End If

            Dim stopPrice = Math.Max(previous.High, bandUpper)
            Dim risk = stopPrice - confirmation.Close
            Dim room = If(priorSupport < confirmation.Close,
                          confirmation.Close - priorSupport,
                          risk * configuration.WaveMinimumRiskReward)
            If risk <= 0D OrElse room < risk * configuration.WaveMinimumRiskReward Then
                Return HoldSignal("venda bloqueada: suporte próximo ou risco/retorno menor que 2:1")
            End If

            Dim strength = If(slopeDegrees >= configuration.WaveStrongSlopeDegrees, "baixa forte", "baixa")
            Return New SpotBotStrategySignal With {
                .Strategy = Kind,
                .Direction = SpotBotSignalDirection.Sell,
                .Confidence = If(slopeDegrees >= configuration.WaveStrongSlopeDegrees, 1D, 0.8D),
                .ObservedValue = slopeDegrees,
                .Reason = $"onda {strength} ({slopeDegrees:F1} graus), toque e rompimento da mínima; RSI {rsi:F2}; RR {(room / risk):F2}:1",
                .StopPrice = stopPrice,
                .TakeProfitPrice = confirmation.Close - (risk * configuration.WaveMinimumRiskReward)
            }
        End If

        Return HoldSignal("sem confirmação da onda")
    End Function

    Private Shared Function CalculateEma(
        candles As IReadOnlyList(Of SpotBotPricePoint),
        lastIndex As Integer,
        period As Integer) As Decimal

        Dim firstIndex = Math.Max(0, lastIndex - period + 1)
        Dim smoothing = 2D / (period + 1D)
        Dim ema = candles(firstIndex).Close

        For index = firstIndex + 1 To lastIndex
            ema = (candles(index).Close - ema) * smoothing + ema
        Next

        Return ema
    End Function

    Private Shared Function CalculateAverageRange(
        candles As IReadOnlyList(Of SpotBotPricePoint),
        lastIndex As Integer,
        count As Integer) As Decimal

        Dim firstIndex = Math.Max(0, lastIndex - count + 1)
        Dim total = 0D
        Dim samples = 0
        For index = firstIndex To lastIndex
            total += candles(index).Range
            samples += 1
        Next

        Return If(samples = 0, 0D, total / samples)
    End Function

    Private Shared Function CalculateRsi(
        candles As IReadOnlyList(Of SpotBotPricePoint),
        period As Integer) As Decimal

        period = Math.Max(2, period)
        If candles.Count < period + 1 Then Return 50D

        Dim gain = 0D
        Dim loss = 0D
        Dim first = candles.Count - period - 1
        For index = first + 1 To first + period
            Dim change = candles(index).Close - candles(index - 1).Close
            If change >= 0D Then gain += change Else loss -= change
        Next

        gain /= period
        loss /= period
        For index = first + period + 1 To candles.Count - 1
            Dim change = candles(index).Close - candles(index - 1).Close
            gain = ((gain * (period - 1)) + Math.Max(0D, change)) / period
            loss = ((loss * (period - 1)) + Math.Max(0D, -change)) / period
        Next

        If loss = 0D Then Return If(gain = 0D, 50D, 100D)
        Return 100D - (100D / (1D + (gain / loss)))
    End Function

    Private Shared Function HoldSignal(reason As String) As SpotBotStrategySignal
        Return New SpotBotStrategySignal With {
            .Strategy = SpotBotStrategyKind.WavePullback,
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

''' <summary>
''' Reversão à média com Bandas de Bollinger conforme a estratégia do vídeo:
''' período 20, três desvios-padrão, fechamento fora da banda, confirmação por
''' rompimento do extremo do candle e saída na média central. O stop fica no
''' extremo do candle que fechou fora da banda.
''' </summary>
Public NotInheritable Class SpotBotBollingerMeanReversionStrategy
    Implements ISpotBotStrategy

    Public ReadOnly Property Kind As SpotBotStrategyKind Implements ISpotBotStrategy.Kind
        Get
            Return SpotBotStrategyKind.BollingerMeanReversion
        End Get
    End Property

    Public Function Evaluate(
        symbol As String,
        prices As IReadOnlyList(Of SpotBotPricePoint),
        configuration As SpotBotConfiguration) As SpotBotStrategySignal Implements ISpotBotStrategy.Evaluate

        Dim period = Math.Max(2, configuration.BollingerPeriod)
        Dim deviations = Math.Max(0.1D, configuration.BollingerStandardDeviations)
        If prices Is Nothing OrElse prices.Count < period + 2 Then
            Return HoldSignal($"Bollinger {period}/{deviations:0.##}: histórico insuficiente")
        End If

        ' O penúltimo ponto é o último candle fechado e o último ponto é o tick
        ' atual. Assim a entrada acontece na superação real da máxima/mínima
        ' marcada, sem esperar outro candle fechar.
        Dim candidateIndex = prices.Count - 2
        Dim confirmationIndex = prices.Count - 1
        Dim candidate = prices(candidateIndex)
        Dim confirmation = prices(confirmationIndex)

        Dim candidateMiddle As Decimal
        Dim candidateUpper As Decimal
        Dim candidateLower As Decimal
        CalculateBands(prices, candidateIndex, period, deviations, candidateMiddle, candidateUpper, candidateLower)

        Dim confirmationMiddle As Decimal
        Dim ignoredUpper As Decimal
        Dim ignoredLower As Decimal
        CalculateBands(prices, confirmationIndex, period, deviations, confirmationMiddle, ignoredUpper, ignoredLower)

        If candidate.Close < candidateLower Then
            If confirmation.High > candidate.High AndAlso confirmation.Close > candidate.Close Then
                Dim stopPrice = candidate.Low
                Dim entryPrice = confirmation.Close
                If stopPrice > 0D AndAlso entryPrice > stopPrice AndAlso confirmationMiddle > entryPrice Then
                    Return New SpotBotStrategySignal With {
                        .Strategy = Kind,
                        .Direction = SpotBotSignalDirection.Buy,
                        .Confidence = 1D,
                        .ObservedValue = candidate.Close - candidateLower,
                        .EntryPrice = entryPrice,
                        .StopPrice = stopPrice,
                        .TakeProfitPrice = confirmationMiddle,
                        .Reason = $"fechamento abaixo da banda inferior {deviations:0.##}σ + superação da máxima marcada; alvo média {period}"
                    }
                End If
            End If

            Return HoldSignal("fechamento abaixo da banda: aguardando confirmação de reversão")
        End If

        If candidate.Close > candidateUpper Then
            If confirmation.Low < candidate.Low AndAlso confirmation.Close < candidate.Close Then
                Dim stopPrice = candidate.High
                Dim entryPrice = confirmation.Close
                If stopPrice > entryPrice AndAlso confirmationMiddle < entryPrice Then
                    Return New SpotBotStrategySignal With {
                        .Strategy = Kind,
                        .Direction = SpotBotSignalDirection.Sell,
                        .Confidence = 1D,
                        .ObservedValue = candidate.Close - candidateUpper,
                        .EntryPrice = entryPrice,
                        .StopPrice = stopPrice,
                        .TakeProfitPrice = confirmationMiddle,
                        .Reason = $"fechamento acima da banda superior {deviations:0.##}σ + perda da mínima marcada; alvo média {period}"
                    }
                End If
            End If

            Return HoldSignal("fechamento acima da banda: aguardando confirmação de reversão")
        End If

        Return HoldSignal($"sem fechamento fora das bandas {period}/{deviations:0.##}")
    End Function

    Private Shared Sub CalculateBands(
        prices As IReadOnlyList(Of SpotBotPricePoint),
        endIndex As Integer,
        period As Integer,
        deviations As Decimal,
        ByRef middle As Decimal,
        ByRef upper As Decimal,
        ByRef lower As Decimal)

        Dim firstIndex = endIndex - period + 1
        Dim sum = 0D
        For index = firstIndex To endIndex
            sum += prices(index).Close
        Next

        middle = sum / period
        Dim variance = 0D
        For index = firstIndex To endIndex
            Dim difference = prices(index).Close - middle
            variance += difference * difference
        Next

        Dim standardDeviation = Convert.ToDecimal(Math.Sqrt(Convert.ToDouble(variance / period)))
        upper = middle + deviations * standardDeviation
        lower = middle - deviations * standardDeviation
    End Sub

    Private Shared Function HoldSignal(reason As String) As SpotBotStrategySignal
        Return New SpotBotStrategySignal With {
            .Strategy = SpotBotStrategyKind.BollingerMeanReversion,
            .Direction = SpotBotSignalDirection.Hold,
            .Reason = reason
        }
    End Function
End Class
