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

''' <summary>
''' Adaptação experimental da abordagem contrária à média atribuída ao BNF.
''' Usa a distância do fechamento em relação à SMA de 25 candles. Um candle
''' de pânico abaixo de 20% da média cria o setup; a entrada só ocorre quando
''' o preço rompe a máxima desse candle. O alvo estrutural é a SMA e o stop é
''' o mínimo do candle de pânico. Não representa uma fórmula autenticada pelo
''' próprio BNF.
''' </summary>
Public NotInheritable Class SpotBotBnfContrarianStrategy
    Implements ISpotBotStrategy

    Public ReadOnly Property Kind As SpotBotStrategyKind Implements ISpotBotStrategy.Kind
        Get
            Return SpotBotStrategyKind.BnfContrarian
        End Get
    End Property

    Public Function Evaluate(
        symbol As String,
        prices As IReadOnlyList(Of SpotBotPricePoint),
        configuration As SpotBotConfiguration) As SpotBotStrategySignal Implements ISpotBotStrategy.Evaluate

        Dim period = Math.Max(2, configuration.BnfMovingAveragePeriod)
        Dim minimumDeviation = Math.Max(0.1D, configuration.BnfMinimumDeviationPercent)
        If prices Is Nothing OrElse prices.Count < period + 2 Then
            Return HoldSignal($"BNF: histórico insuficiente para SMA {period}")
        End If

        ' O penúltimo ponto é o último candle fechado; o último representa o
        ' preço atual usado apenas para confirmar o rompimento intrabar.
        Dim candidateIndex = prices.Count - 2
        Dim confirmation = prices(prices.Count - 1)
        Dim candidate = prices(candidateIndex)
        Dim movingAverage = CalculateMovingAverage(prices, candidateIndex, period)
        If movingAverage <= 0D Then
            Return HoldSignal("BNF: média móvel inválida")
        End If

        Dim deviationPercent = (candidate.Close - movingAverage) / movingAverage * 100D

        If deviationPercent <= -minimumDeviation Then
            If confirmation.High > candidate.High AndAlso confirmation.Close > candidate.Close Then
                Dim stopPrice = candidate.Low
                Dim entryPrice = confirmation.Close
                If stopPrice > 0D AndAlso entryPrice > stopPrice AndAlso movingAverage > entryPrice Then
                    Return New SpotBotStrategySignal With {
                        .Strategy = Kind,
                        .Direction = SpotBotSignalDirection.Buy,
                        .Confidence = Math.Min(1D, Math.Abs(deviationPercent) / 35D),
                        .ObservedValue = deviationPercent,
                        .EntryPrice = entryPrice,
                        .StopPrice = stopPrice,
                        .TakeProfitPrice = movingAverage,
                        .Reason = $"BNF: fechamento {Math.Abs(deviationPercent):0.##}% abaixo da SMA {period} + rompimento da máxima; alvo SMA"
                    }
                End If
            End If

            Return HoldSignal($"BNF: desvio {Math.Abs(deviationPercent):0.##}% abaixo da SMA {period}; aguardando rompimento")
        End If

        Return HoldSignal($"BNF: desvio {deviationPercent:0.##}% (mínimo {minimumDeviation:0.##}%)")
    End Function

    Private Shared Function CalculateMovingAverage(
        prices As IReadOnlyList(Of SpotBotPricePoint),
        endIndex As Integer,
        period As Integer) As Decimal

        Dim firstIndex = endIndex - period + 1
        Dim sum = 0D
        For index = firstIndex To endIndex
            sum += prices(index).Close
        Next

        Return sum / period
    End Function

    Private Shared Function HoldSignal(reason As String) As SpotBotStrategySignal
        Return New SpotBotStrategySignal With {
            .Strategy = SpotBotStrategyKind.BnfContrarian,
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

''' <summary>
''' Pullback de tendência na zona de valor, inspirado nos quadros do vídeo:
''' M5 define o contexto, M2 confirma o retorno à EMA 9/20 e M1 confirma a
''' retomada. O VWAP diário funciona como filtro de viés. No Spot a versão é
''' deliberadamente long-only; a versão Short deve ser implementada no módulo
''' de Futures, onde há suporte a posições vendidas.
''' </summary>
Public NotInheritable Class SpotBotValueZonePullbackStrategy
    Implements ISpotBotMultiTimeframeStrategy

    Public ReadOnly Property Kind As SpotBotStrategyKind Implements ISpotBotStrategy.Kind
        Get
            Return SpotBotStrategyKind.ValueZonePullback
        End Get
    End Property

    Public Function Evaluate(
        symbol As String,
        prices As IReadOnlyList(Of SpotBotPricePoint),
        configuration As SpotBotConfiguration) As SpotBotStrategySignal Implements ISpotBotStrategy.Evaluate

        Return HoldSignal("pullback zona de valor aguardando candles M5/M2/M1")
    End Function

    Public Function EvaluateMultiTimeframe(
        symbol As String,
        m1Candles As IReadOnlyList(Of SpotBotPricePoint),
        m2Candles As IReadOnlyList(Of SpotBotPricePoint),
        m5Candles As IReadOnlyList(Of SpotBotPricePoint),
        configuration As SpotBotConfiguration) As SpotBotStrategySignal Implements ISpotBotMultiTimeframeStrategy.EvaluateMultiTimeframe

        Dim fastPeriod = Math.Max(2, configuration.ValueZoneFastEmaPeriod)
        Dim slowPeriod = Math.Max(fastPeriod + 1, configuration.ValueZoneSlowEmaPeriod)
        Dim atrPeriod = Math.Max(3, configuration.ValueZoneAtrPeriod)
        Dim structureLookback = Math.Max(5, configuration.ValueZoneStructureLookback)

        If m1Candles Is Nothing OrElse m2Candles Is Nothing OrElse m5Candles Is Nothing OrElse
           m1Candles.Count < 3 OrElse
           m2Candles.Count < Math.Max(slowPeriod + 2, atrPeriod + 2) OrElse
           m5Candles.Count < slowPeriod + 3 Then
            Return HoldSignal("pullback zona de valor: histórico M5/M2/M1 insuficiente")
        End If

        Dim m5Last = m5Candles.Count - 1
        Dim m5Previous = Math.Max(0, m5Last - 2)
        Dim m5Fast = CalculateEma(m5Candles, m5Last, fastPeriod)
        Dim m5Slow = CalculateEma(m5Candles, m5Last, slowPeriod)
        Dim m5FastPrevious = CalculateEma(m5Candles, m5Previous, fastPeriod)
        Dim m5TrendUp = m5Fast > m5Slow AndAlso m5Fast > m5FastPrevious AndAlso
                        m5Candles(m5Last).Close > m5Slow
        If Not m5TrendUp Then
            Return HoldSignal($"pullback zona de valor: M5 sem tendência de alta (EMA {fastPeriod}/{slowPeriod})")
        End If

        Dim dailyVwap = CalculateDailyValue(m5Candles, m5Last)
        If configuration.ValueZoneRequireVwap AndAlso dailyVwap > 0D AndAlso
           m5Candles(m5Last).Close < dailyVwap Then
            Return HoldSignal($"pullback zona de valor: preço abaixo da VWAP diária ({dailyVwap:F8})")
        End If

        Dim m2PullbackIndex = m2Candles.Count - 2
        Dim m2ConfirmationIndex = m2Candles.Count - 1
        Dim m2Pullback = m2Candles(m2PullbackIndex)
        Dim m2Confirmation = m2Candles(m2ConfirmationIndex)
        Dim m2Fast = CalculateEma(m2Candles, m2PullbackIndex, fastPeriod)
        Dim m2Slow = CalculateEma(m2Candles, m2PullbackIndex, slowPeriod)
        Dim zoneLow = Math.Min(m2Fast, m2Slow)
        Dim zoneHigh = Math.Max(m2Fast, m2Slow)
        Dim atr = CalculateAverageTrueRange(m2Candles, m2ConfirmationIndex, atrPeriod)
        If atr <= 0D Then Return HoldSignal("pullback zona de valor: ATR inválido")

        Dim tolerance = atr * Math.Max(0D, configuration.ValueZoneToleranceAtr)
        Dim touchedZone = m2Pullback.Low <= zoneHigh + tolerance AndAlso
                          m2Pullback.High >= zoneLow - tolerance
        If Not touchedZone Then
            Return HoldSignal("pullback zona de valor: M2 ainda não retornou à zona EMA 9/20")
        End If

        Dim m2Confirmed = m2Confirmation.IsBullish AndAlso
                         m2Confirmation.Close > m2Pullback.High AndAlso
                         m2Confirmation.Close > m2Fast
        If Not m2Confirmed Then
            Return HoldSignal("pullback zona de valor: aguardando confirmação de retomada no M2")
        End If

        Dim m1Previous = m1Candles(m1Candles.Count - 2)
        Dim m1Confirmation = m1Candles(m1Candles.Count - 1)
        If Not m1Confirmation.IsBullish OrElse m1Confirmation.Close <= m1Previous.High Then
            Return HoldSignal("pullback zona de valor: M1 sem rompimento de confirmação")
        End If

        Dim entryPrice = m1Confirmation.Close
        Dim supportFirst = Math.Max(0, m2Candles.Count - structureLookback - 2)
        Dim supportLast = m2Candles.Count - 2
        Dim support = Decimal.MaxValue
        For index = supportFirst To supportLast
            support = Math.Min(support, m2Candles(index).Low)
        Next
        If support = Decimal.MaxValue Then support = m2Pullback.Low

        Dim stopPrice = Math.Min(m2Pullback.Low, support) - (atr * 0.25D)
        If stopPrice <= 0D OrElse stopPrice >= entryPrice Then
            Return HoldSignal("pullback zona de valor: suporte sem distância válida para o stop")
        End If

        Dim risk = entryPrice - stopPrice
        Dim resistance = Decimal.MinValue
        Dim resistanceFirst = Math.Max(0, m5Candles.Count - structureLookback - 1)
        Dim resistanceLast = m5Candles.Count - 2
        For index = resistanceFirst To resistanceLast
            If m5Candles(index).High > entryPrice Then
                resistance = Math.Max(resistance, m5Candles(index).High)
            End If
        Next

        Dim minimumRiskReward = Math.Max(1D, configuration.ValueZoneMinimumRiskReward)
        Dim targetPrice = entryPrice + (risk * minimumRiskReward)
        If resistance > entryPrice AndAlso resistance - entryPrice >= risk * minimumRiskReward Then
            targetPrice = resistance
        End If

        Dim vwapText = If(dailyVwap > 0D, $"; VWAP {dailyVwap:F8}", String.Empty)
        Return New SpotBotStrategySignal With {
            .Strategy = Kind,
            .Direction = SpotBotSignalDirection.Buy,
            .Confidence = If(configuration.ValueZoneRequireVwap, 1D, 0.85D),
            .ObservedValue = (entryPrice - m2Fast) / entryPrice * 100D,
            .EntryPrice = entryPrice,
            .StopPrice = stopPrice,
            .TakeProfitPrice = targetPrice,
            .Reason = $"pullback zona de valor: M5 tendência, retorno à EMA {fastPeriod}/{slowPeriod}, confirmação M2/M1{vwapText}; RR {(targetPrice - entryPrice) / risk:F2}:1"
        }
    End Function

    Private Shared Function CalculateEma(
        candles As IReadOnlyList(Of SpotBotPricePoint),
        lastIndex As Integer,
        period As Integer) As Decimal

        If candles Is Nothing OrElse candles.Count = 0 Then Return 0D
        Dim boundedLast = Math.Min(lastIndex, candles.Count - 1)
        Dim smoothing = 2D / (period + 1D)
        Dim ema = candles(0).Close
        For index = 1 To boundedLast
            ema = (candles(index).Close - ema) * smoothing + ema
        Next
        Return ema
    End Function

    Private Shared Function CalculateAverageTrueRange(
        candles As IReadOnlyList(Of SpotBotPricePoint),
        lastIndex As Integer,
        period As Integer) As Decimal

        Dim first = Math.Max(1, lastIndex - period + 1)
        Dim total = 0D
        Dim count = 0
        For index = first To lastIndex
            Dim previousClose = candles(index - 1).Close
            Dim trueRange = Math.Max(
                candles(index).High - candles(index).Low,
                Math.Max(Math.Abs(candles(index).High - previousClose),
                         Math.Abs(candles(index).Low - previousClose)))
            total += Math.Max(0D, trueRange)
            count += 1
        Next
        Return If(count = 0, 0D, total / count)
    End Function

    Private Shared Function CalculateDailyValue(
        candles As IReadOnlyList(Of SpotBotPricePoint),
        lastIndex As Integer) As Decimal

        If candles Is Nothing OrElse candles.Count = 0 Then Return 0D
        Dim lastDate = candles(lastIndex).TimestampUtc.Date
        Dim weightedTotal = 0D
        Dim volumeTotal = 0D
        Dim simpleTotal = 0D
        Dim simpleCount = 0

        For index = 0 To lastIndex
            If candles(index).TimestampUtc.Date <> lastDate Then Continue For
            Dim typical = (candles(index).High + candles(index).Low + candles(index).Close) / 3D
            If candles(index).Volume > 0D Then
                weightedTotal += typical * candles(index).Volume
                volumeTotal += candles(index).Volume
            Else
                simpleTotal += typical
                simpleCount += 1
            End If
        Next

        If volumeTotal > 0D Then Return weightedTotal / volumeTotal
        If simpleCount > 0 Then Return simpleTotal / simpleCount

        Return 0D
    End Function

    Private Shared Function HoldSignal(reason As String) As SpotBotStrategySignal
        Return New SpotBotStrategySignal With {
            .Strategy = SpotBotStrategyKind.ValueZonePullback,
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
''' Rompimento de uma faixa seguido de reteste do nível rompido.
''' O sinal só é gerado em candles fechados e permanece long/short simétrico;
''' no Spot o lado Sell serve para reduzir ou encerrar uma posição existente.
''' </summary>
Public NotInheritable Class SpotBotBreakoutRetestStrategy
    Implements ISpotBotCandleStrategy

    Public ReadOnly Property Kind As SpotBotStrategyKind Implements ISpotBotStrategy.Kind
        Get
            Return SpotBotStrategyKind.BreakoutRetest
        End Get
    End Property

    Public Function Evaluate(
        symbol As String,
        prices As IReadOnlyList(Of SpotBotPricePoint),
        configuration As SpotBotConfiguration) As SpotBotStrategySignal Implements ISpotBotStrategy.Evaluate

        Return HoldSignal("aguardando candle fechado do rompimento e reteste")
    End Function

    Public Function EvaluateCandles(
        symbol As String,
        candles As IReadOnlyList(Of SpotBotPricePoint),
        configuration As SpotBotConfiguration) As SpotBotStrategySignal Implements ISpotBotCandleStrategy.EvaluateCandles

        Dim period = Math.Max(5, configuration.BreakoutPeriod)
        If candles Is Nothing OrElse candles.Count < period + 3 Then
            Return HoldSignal("histórico insuficiente para o rompimento e reteste")
        End If

        Dim breakoutIndex = candles.Count - 2
        Dim retestIndex = candles.Count - 1
        Dim rangeFirst = breakoutIndex - period
        Dim rangeLast = breakoutIndex - 1
        Dim resistance = Decimal.MinValue
        Dim support = Decimal.MaxValue

        For index = rangeFirst To rangeLast
            resistance = Math.Max(resistance, candles(index).High)
            support = Math.Min(support, candles(index).Low)
        Next

        Dim breakout = candles(breakoutIndex)
        Dim retest = candles(retestIndex)
        Dim riskReward = 2D

        If breakout.Close > resistance AndAlso
           retest.Low <= resistance AndAlso
           retest.Close > resistance AndAlso
           retest.IsBullish Then
            Dim stopPrice = Math.Min(retest.Low, resistance)
            Dim risk = retest.Close - stopPrice
            If risk > 0D Then
                Return New SpotBotStrategySignal With {
                    .Strategy = Kind,
                    .Direction = SpotBotSignalDirection.Buy,
                    .Confidence = 1D,
                    .ObservedValue = retest.Close - resistance,
                    .EntryPrice = retest.Close,
                    .StopPrice = stopPrice,
                    .TakeProfitPrice = retest.Close + risk * riskReward,
                    .Reason = $"rompimento da resistência {resistance:F8} + reteste confirmado; RR {riskReward:0.##}:1"
                }
            End If
        End If

        If breakout.Close < support AndAlso
           retest.High >= support AndAlso
           retest.Close < support AndAlso
           retest.IsBearish Then
            Dim stopPrice = Math.Max(retest.High, support)
            Dim risk = stopPrice - retest.Close
            If risk > 0D Then
                Return New SpotBotStrategySignal With {
                    .Strategy = Kind,
                    .Direction = SpotBotSignalDirection.Sell,
                    .Confidence = 1D,
                    .ObservedValue = support - retest.Close,
                    .EntryPrice = retest.Close,
                    .StopPrice = stopPrice,
                    .TakeProfitPrice = retest.Close - risk * riskReward,
                    .Reason = $"rompimento do suporte {support:F8} + reteste confirmado; RR {riskReward:0.##}:1"
                }
            End If
        End If

        Return HoldSignal("sem rompimento com reteste confirmado")
    End Function

    Private Shared Function HoldSignal(reason As String) As SpotBotStrategySignal
        Return New SpotBotStrategySignal With {
            .Strategy = SpotBotStrategyKind.BreakoutRetest,
            .Direction = SpotBotSignalDirection.Hold,
            .Reason = reason
        }
    End Function
End Class

''' <summary>
''' Momentum confirmado por expansão de volume e fechamento acima/abaixo da
''' EMA. O volume recebido pelo ticker é tratado como uma confirmação
''' experimental; a execução continua protegida pelo modo de simulação.
''' </summary>
Public NotInheritable Class SpotBotMomentumVolumeStrategy
    Implements ISpotBotStrategy

    Public ReadOnly Property Kind As SpotBotStrategyKind Implements ISpotBotStrategy.Kind
        Get
            Return SpotBotStrategyKind.MomentumVolume
        End Get
    End Property

    Public Function Evaluate(
        symbol As String,
        prices As IReadOnlyList(Of SpotBotPricePoint),
        configuration As SpotBotConfiguration) As SpotBotStrategySignal Implements ISpotBotStrategy.Evaluate

        Dim lookback = Math.Max(5, configuration.MomentumVolumeLookback)
        Dim emaPeriod = Math.Max(2, configuration.MomentumEmaPeriod)
        Dim multiplier = Math.Max(1D, configuration.MomentumVolumeMultiplier)
        If prices Is Nothing OrElse prices.Count < Math.Max(lookback + 2, emaPeriod + 2) Then
            Return HoldSignal("histórico insuficiente para momentum por volume")
        End If

        Dim latest = prices(prices.Count - 1)
        If latest.Volume <= 0D Then
            Return HoldSignal("ticker sem volume disponível")
        End If

        Dim averageVolume = prices.Skip(Math.Max(0, prices.Count - lookback - 1)).Take(lookback).
            Where(Function(item) item.Volume > 0D).
            Select(Function(item) item.Volume).
            DefaultIfEmpty(0D).
            Average()
        If averageVolume <= 0D OrElse latest.Volume < averageVolume * multiplier Then
            Return HoldSignal($"volume {latest.Volume:0.##} abaixo do gatilho {averageVolume * multiplier:0.##}")
        End If

        Dim ema = CalculateEma(prices, emaPeriod)
        Dim previous = prices(prices.Count - 2)

        If latest.Close > ema AndAlso latest.Close > previous.Close Then
            Dim stopPrice = Math.Min(previous.Close, ema)
            Dim risk = latest.Close - stopPrice
            If risk > 0D Then
                Return New SpotBotStrategySignal With {
                    .Strategy = Kind,
                    .Direction = SpotBotSignalDirection.Buy,
                    .Confidence = Math.Min(1D, latest.Volume / (averageVolume * multiplier)),
                    .ObservedValue = latest.Volume / averageVolume,
                    .EntryPrice = latest.Close,
                    .StopPrice = stopPrice,
                    .TakeProfitPrice = latest.Close + risk * Math.Max(1D, configuration.MomentumRiskReward),
                    .Reason = $"momentum comprador: volume {latest.Volume / averageVolume:0.##}x da média + fechamento acima da EMA {emaPeriod}"
                }
            End If
        End If

        If latest.Close < ema AndAlso latest.Close < previous.Close Then
            Dim stopPrice = Math.Max(previous.Close, ema)
            Dim risk = stopPrice - latest.Close
            If risk > 0D Then
                Return New SpotBotStrategySignal With {
                    .Strategy = Kind,
                    .Direction = SpotBotSignalDirection.Sell,
                    .Confidence = Math.Min(1D, latest.Volume / (averageVolume * multiplier)),
                    .ObservedValue = latest.Volume / averageVolume,
                    .EntryPrice = latest.Close,
                    .StopPrice = stopPrice,
                    .TakeProfitPrice = latest.Close - risk * Math.Max(1D, configuration.MomentumRiskReward),
                    .Reason = $"momentum vendedor: volume {latest.Volume / averageVolume:0.##}x da média + fechamento abaixo da EMA {emaPeriod}"
                }
            End If
        End If

        Return HoldSignal("volume expandiu, mas o preço não confirmou o momentum")
    End Function

    Private Shared Function CalculateEma(prices As IReadOnlyList(Of SpotBotPricePoint), period As Integer) As Decimal
        Dim firstIndex = Math.Max(0, prices.Count - period)
        Dim smoothing = 2D / (period + 1D)
        Dim ema = prices(firstIndex).Close
        For index = firstIndex + 1 To prices.Count - 1
            ema = (prices(index).Close - ema) * smoothing + ema
        Next
        Return ema
    End Function

    Private Shared Function HoldSignal(reason As String) As SpotBotStrategySignal
        Return New SpotBotStrategySignal With {
            .Strategy = SpotBotStrategyKind.MomentumVolume,
            .Direction = SpotBotSignalDirection.Hold,
            .Reason = reason
        }
    End Function
End Class

''' <summary>
''' Grade que só opera em regime lateral: o preço precisa permanecer dentro
''' de uma faixa limitada e a inclinação da EMA curta não pode indicar uma
''' tendência forte. Não cria ordens reais; fornece sinais para o simulador.
''' </summary>
Public NotInheritable Class SpotBotGridRegimeStrategy
    Implements ISpotBotStrategy

    Public ReadOnly Property Kind As SpotBotStrategyKind Implements ISpotBotStrategy.Kind
        Get
            Return SpotBotStrategyKind.GridRegime
        End Get
    End Property

    Public Function Evaluate(
        symbol As String,
        prices As IReadOnlyList(Of SpotBotPricePoint),
        configuration As SpotBotConfiguration) As SpotBotStrategySignal Implements ISpotBotStrategy.Evaluate

        Dim lookback = Math.Max(10, configuration.GridRegimeLookback)
        If prices Is Nothing OrElse prices.Count < lookback + 1 Then
            Return HoldSignal("histórico insuficiente para grade com filtro de regime")
        End If

        Dim window = prices.Skip(prices.Count - lookback).ToList()
        Dim highest = window.Max(Function(item) item.High)
        Dim lowest = window.Min(Function(item) item.Low)
        Dim midpoint = (highest + lowest) / 2D
        If midpoint <= 0D Then Return HoldSignal("faixa de regime inválida")

        Dim rangePercent = (highest - lowest) / midpoint * 100D
        If rangePercent > Math.Max(0.1D, configuration.GridRegimeMaxRangePercent) Then
            Return HoldSignal($"regime tendencial: faixa de {rangePercent:0.##}%")
        End If

        Dim emaShort = CalculateEma(window, Math.Min(10, lookback))
        Dim emaPrevious = CalculateEma(window.Take(window.Count - 1).ToList(), Math.Min(10, lookback))
        Dim slopePercent = If(emaPrevious = 0D, 0D, Math.Abs(emaShort - emaPrevious) / emaPrevious * 100D)
        If slopePercent > Math.Max(0.1D, configuration.GridRegimeMaxSlopePercent) Then
            Return HoldSignal($"regime tendencial: inclinação de {slopePercent:0.##}%")
        End If

        Dim reference = If(configuration.GridReferencePrice > 0D, configuration.GridReferencePrice, midpoint)
        Dim spacing = Math.Max(0.01D, configuration.GridSpacingPercent) / 100D
        Dim latest = prices(prices.Count - 1).Close
        Dim buyLevel = reference * (1D - spacing)
        Dim sellLevel = reference * (1D + spacing)

        If latest <= buyLevel Then
            Return New SpotBotStrategySignal With {
                .Strategy = Kind,
                .Direction = SpotBotSignalDirection.Buy,
                .Confidence = 1D,
                .ObservedValue = rangePercent,
                .EntryPrice = latest,
                .Reason = $"grade ativa em regime lateral ({rangePercent:0.##}% de faixa); nível de compra {buyLevel:F8}"
            }
        End If

        If latest >= sellLevel Then
            Return New SpotBotStrategySignal With {
                .Strategy = Kind,
                .Direction = SpotBotSignalDirection.Sell,
                .Confidence = 1D,
                .ObservedValue = rangePercent,
                .EntryPrice = latest,
                .Reason = $"grade ativa em regime lateral ({rangePercent:0.##}% de faixa); nível de venda {sellLevel:F8}"
            }
        End If

        Return HoldSignal("regime lateral confirmado, preço dentro da faixa da grade")
    End Function

    Private Shared Function CalculateEma(prices As IReadOnlyList(Of SpotBotPricePoint), period As Integer) As Decimal
        If prices.Count = 0 Then Return 0D
        Dim smoothing = 2D / (period + 1D)
        Dim ema = prices(0).Close
        For index = 1 To prices.Count - 1
            ema = (prices(index).Close - ema) * smoothing + ema
        Next
        Return ema
    End Function

    Private Shared Function HoldSignal(reason As String) As SpotBotStrategySignal
        Return New SpotBotStrategySignal With {
            .Strategy = SpotBotStrategyKind.GridRegime,
            .Direction = SpotBotSignalDirection.Hold,
            .Reason = reason
        }
    End Function
End Class

''' <summary>
''' Grade adaptativa v2. Mantém a lógica de reversão somente em regime lateral,
''' calcula o espaçamento pela volatilidade recente e pausa quando há expansão
''' de faixa compatível com rompimento. O motor de simulação pode acumular até
''' os níveis configurados para o mesmo ativo.
''' </summary>
Public NotInheritable Class SpotBotGridAdaptiveStrategy
    Implements ISpotBotTrendFilterStrategy

    Public ReadOnly Property Kind As SpotBotStrategyKind Implements ISpotBotStrategy.Kind
        Get
            Return SpotBotStrategyKind.GridAdaptive
        End Get
    End Property

    Public Function Evaluate(
        symbol As String,
        prices As IReadOnlyList(Of SpotBotPricePoint),
        configuration As SpotBotConfiguration) As SpotBotStrategySignal Implements ISpotBotStrategy.Evaluate

        Return EvaluateInternal(symbol, prices, Nothing, configuration)
    End Function

    Public Function EvaluateWithTrend(
        symbol As String,
        prices As IReadOnlyList(Of SpotBotPricePoint),
        trendCandles As IReadOnlyList(Of SpotBotPricePoint),
        configuration As SpotBotConfiguration) As SpotBotStrategySignal Implements ISpotBotTrendFilterStrategy.EvaluateWithTrend

        Return EvaluateInternal(symbol, prices, trendCandles, configuration)
    End Function

    Private Function EvaluateInternal(
        symbol As String,
        prices As IReadOnlyList(Of SpotBotPricePoint),
        trendCandles As IReadOnlyList(Of SpotBotPricePoint),
        configuration As SpotBotConfiguration) As SpotBotStrategySignal

        If configuration.GridV2EmaFilterEnabled AndAlso trendCandles IsNot Nothing Then
            Dim regimeReason As String = String.Empty
            If Not IsLateralByEma(trendCandles, configuration, regimeReason) Then
                Return HoldSignal(regimeReason)
            End If
        End If

        Dim lookback = Math.Max(20, configuration.GridV2Lookback)
        Dim volatilityPeriod = Math.Max(5, configuration.GridV2VolatilityPeriod)
        If prices Is Nothing OrElse prices.Count < Math.Max(lookback + 1, volatilityPeriod + 2) Then
            Return HoldSignal("histórico insuficiente para a grade adaptativa")
        End If

        Dim window = prices.Skip(prices.Count - lookback).ToList()
        Dim latest = window(window.Count - 1).Close
        Dim regimeWindow = window.Take(window.Count - 1).ToList()
        Dim highest = regimeWindow.Max(Function(item) item.High)
        Dim lowest = regimeWindow.Min(Function(item) item.Low)
        Dim midpoint = (highest + lowest) / 2D
        If latest <= 0D OrElse midpoint <= 0D Then
            Return HoldSignal("faixa de regime inválida")
        End If

        Dim rangePercent = (highest - lowest) / midpoint * 100D
        If rangePercent > Math.Max(0.1D, configuration.GridV2MaxRangePercent) Then
            Return HoldSignal($"grade pausada: faixa de {rangePercent:0.##}% indica tendência")
        End If

        Dim emaPeriod = Math.Min(10, Math.Max(3, lookback \ 3))
        Dim emaCurrent = CalculateEma(window, emaPeriod)
        Dim previousWindow = window.Take(window.Count - 1).ToList()
        Dim emaPrevious = CalculateEma(previousWindow, emaPeriod)
        Dim slopePercent = If(emaPrevious = 0D, 0D, Math.Abs(emaCurrent - emaPrevious) / emaPrevious * 100D)
        If slopePercent > Math.Max(0.1D, configuration.GridV2MaxSlopePercent) Then
            Return HoldSignal($"grade pausada: inclinação de {slopePercent:0.##}% indica tendência")
        End If

        Dim volatilityPercent = CalculateReturnVolatility(window, volatilityPeriod)
        Dim spacingPercent = Math.Max(
            configuration.GridV2SpacingMinimumPercent,
            Math.Min(configuration.GridV2SpacingMaximumPercent, volatilityPercent * 2.5D))
        spacingPercent = Math.Max(0.01D, spacingPercent)

        Dim minimumStopPercent = Math.Max(0D, configuration.GridV2MinimumStopPercent)
        Dim stopVolatilityMultiplier = Math.Max(0D, configuration.GridV2StopVolatilityMultiplier)
        Dim dynamicStopPercent = Math.Max(minimumStopPercent, volatilityPercent * stopVolatilityMultiplier)
        Dim maximumStopPercent = Math.Max(minimumStopPercent, configuration.GridV2MaximumStopPercent)
        If dynamicStopPercent > maximumStopPercent Then
            Return HoldSignal($"grade pausada: stop dinâmico de {dynamicStopPercent:0.##}% acima do limite de {maximumStopPercent:0.##}%")
        End If

        Dim averageRange = CalculateAverageRange(regimeWindow, volatilityPeriod)
        Dim breakoutBuffer = averageRange * Math.Max(0D, configuration.GridV2BreakoutAtrMultiplier)
        If breakoutBuffer > 0D AndAlso (latest > highest + breakoutBuffer OrElse latest < lowest - breakoutBuffer) Then
            Return HoldSignal("grade pausada: rompimento da faixa detectado")
        End If

        Dim reference = If(configuration.GridReferencePrice > 0D, configuration.GridReferencePrice, midpoint)
        Dim buyLevel = reference * (1D - spacingPercent / 100D)
        Dim sellLevel = reference * (1D + spacingPercent / 100D)

        If latest <= buyLevel Then
            Dim supportPrice = lowest
            If configuration.GridV2UseNearestSupportStop Then
                Dim nearestSupport = FindNearestSupportBelow(regimeWindow, latest)
                If nearestSupport > 0D Then supportPrice = nearestSupport
            End If

            Dim supportBuffer = averageRange * Math.Max(0D, configuration.GridV2SupportBufferAtr)
            Dim structuralStopPrice = Math.Max(0D, supportPrice - supportBuffer)
            Dim minimumStopPrice = latest * (1D - dynamicStopPercent / 100D)
            Dim stopPrice = Math.Max(0D, Math.Min(structuralStopPrice, minimumStopPrice))
            Dim maximumStopPrice = latest * (1D - maximumStopPercent / 100D)
            stopPrice = Math.Max(stopPrice, maximumStopPrice)
            Dim stopDistancePercent = If(latest <= 0D, 0D, (latest - stopPrice) / latest * 100D)
            Return New SpotBotStrategySignal With {
                .Strategy = Kind,
                .Direction = SpotBotSignalDirection.Buy,
                .Confidence = 1D,
                .ObservedValue = spacingPercent,
                .EntryPrice = latest,
                .StopPrice = stopPrice,
                .GridManaged = True,
                .Reason = $"grade v2 em regime lateral: compra {buyLevel:F8}, espaçamento {spacingPercent:0.##}%, suporte {supportPrice:F8}, SL {stopDistancePercent:0.##}%"
            }
        End If

        If latest >= sellLevel Then
            Return New SpotBotStrategySignal With {
                .Strategy = Kind,
                .Direction = SpotBotSignalDirection.Sell,
                .Confidence = 1D,
                .ObservedValue = spacingPercent,
                .EntryPrice = latest,
                .GridManaged = True,
                .Reason = $"grade v2 em regime lateral: venda {sellLevel:F8}, espaçamento {spacingPercent:0.##}%"
            }
        End If

        Return HoldSignal($"grade v2 ativa; faixa {rangePercent:0.##}%, espaçamento {spacingPercent:0.##}%")
    End Function

    Private Shared Function FindNearestSupportBelow(
        candles As IReadOnlyList(Of SpotBotPricePoint),
        entryPrice As Decimal) As Decimal

        If candles Is Nothing OrElse candles.Count = 0 OrElse entryPrice <= 0D Then Return 0D

        Dim nearest = 0D
        Dim lowest = Decimal.MaxValue
        For index = 0 To candles.Count - 1
            Dim low = candles(index).Low
            If low > 0D AndAlso low < entryPrice Then
                lowest = Math.Min(lowest, low)
            End If

            If index <= 0 OrElse index >= candles.Count - 1 Then Continue For
            If low < entryPrice AndAlso
               low <= candles(index - 1).Low AndAlso
               low <= candles(index + 1).Low Then
                nearest = Math.Max(nearest, low)
            End If
        Next

        Return If(nearest > 0D, nearest, If(lowest = Decimal.MaxValue, 0D, lowest))
    End Function

    Private Shared Function IsLateralByEma(
        candles As IReadOnlyList(Of SpotBotPricePoint),
        configuration As SpotBotConfiguration,
        ByRef reason As String) As Boolean

        Dim fastPeriod = Math.Max(2, configuration.GridV2FastEmaPeriod)
        Dim slowPeriod = Math.Max(fastPeriod + 1, configuration.GridV2SlowEmaPeriod)
        If candles Is Nothing OrElse candles.Count < slowPeriod + 3 Then
            ' O WebSocket pode iniciar sem histórico M5. Nesse período os
            ' filtros estruturais originais da grade continuam valendo; assim
            ' o Grid não fica travado por horas apenas por causa do warm-up.
            reason = $"filtro EMA {fastPeriod}/{slowPeriod} em aquecimento; filtros originais da grade ativos"
            Return True
        End If

        Dim latest = candles(candles.Count - 1).Close
        If latest <= 0D Then
            reason = "grade pausada: preço M5 inválido para filtro EMA"
            Return False
        End If

        Dim fast = CalculateEma(candles, fastPeriod)
        Dim slow = CalculateEma(candles, slowPeriod)
        Dim previousSlow = CalculateEma(candles.Take(candles.Count - 2).ToList(), slowPeriod)
        If slow <= 0D OrElse previousSlow <= 0D Then
            reason = "grade pausada: EMA M5 inválida"
            Return False
        End If

        Dim spreadPercent = Math.Abs(fast - slow) / latest * 100D
        Dim slopePercent = Math.Abs(slow - previousSlow) / previousSlow * 100D
        Dim maxSpread = Math.Max(0.01D, configuration.GridV2EmaMaxSpreadPercent)
        Dim maxSlope = Math.Max(0.01D, configuration.GridV2EmaMaxSlopePercent)
        If spreadPercent > maxSpread OrElse slopePercent > maxSlope Then
            reason = $"grade pausada: EMA {fastPeriod}/{slowPeriod} M5 indica tendência (distância {spreadPercent:0.##}%, inclinação {slopePercent:0.##}%)"
            Return False
        End If

        Return True
    End Function

    Private Shared Function CalculateEma(prices As IReadOnlyList(Of SpotBotPricePoint), period As Integer) As Decimal
        If prices Is Nothing OrElse prices.Count = 0 Then Return 0D
        Dim smoothing = 2D / (period + 1D)
        Dim ema = prices(0).Close
        For index = 1 To prices.Count - 1
            ema = (prices(index).Close - ema) * smoothing + ema
        Next
        Return ema
    End Function

    Private Shared Function CalculateAverageRange(prices As IReadOnlyList(Of SpotBotPricePoint), period As Integer) As Decimal
        Dim first = Math.Max(1, prices.Count - period)
        Dim total = 0D
        Dim count = 0
        For index = first To prices.Count - 1
            total += Math.Max(prices(index).Range, Math.Abs(prices(index).Close - prices(index - 1).Close))
            count += 1
        Next
        Return If(count = 0, 0D, total / count)
    End Function

    Private Shared Function CalculateReturnVolatility(prices As IReadOnlyList(Of SpotBotPricePoint), period As Integer) As Decimal
        Dim first = Math.Max(1, prices.Count - period)
        Dim returns As New List(Of Decimal)()
        For index = first To prices.Count - 1
            Dim previous = prices(index - 1).Close
            If previous > 0D Then returns.Add((prices(index).Close - previous) / previous * 100D)
        Next
        If returns.Count = 0 Then Return 0D
        Dim average = returns.Average()
        Dim variance = returns.Sum(Function(item) (item - average) * (item - average)) / returns.Count
        Return CDec(Math.Sqrt(CDbl(Math.Max(0D, variance))))
    End Function

    Private Shared Function HoldSignal(reason As String) As SpotBotStrategySignal
        Return New SpotBotStrategySignal With {
            .Strategy = SpotBotStrategyKind.GridAdaptive,
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

    Private ReadOnly _kind As SpotBotStrategyKind

    Public Sub New(Optional kind As SpotBotStrategyKind = SpotBotStrategyKind.BollingerMeanReversion)
        _kind = kind
    End Sub

    Public ReadOnly Property Kind As SpotBotStrategyKind Implements ISpotBotStrategy.Kind
        Get
            Return _kind
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

    Private Function HoldSignal(reason As String) As SpotBotStrategySignal
        Return New SpotBotStrategySignal With {
            .Strategy = _kind,
            .Direction = SpotBotSignalDirection.Hold,
            .Reason = reason
        }
    End Function
End Class
