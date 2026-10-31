Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Threading.Tasks

Public NotInheritable Class SpotDcaEngine
    Private ReadOnly _sync As New Object()
    Private ReadOnly _plans As New Dictionary(Of String, DcaPlan)(StringComparer.OrdinalIgnoreCase)
    Private ReadOnly _states As New Dictionary(Of String, DcaState)(StringComparer.OrdinalIgnoreCase)
    Private ReadOnly _candles As New Dictionary(Of String, List(Of DcaCandle))(StringComparer.OrdinalIgnoreCase)
    Private ReadOnly _currentCandles As New Dictionary(Of String, DcaCandle)(StringComparer.OrdinalIgnoreCase)
    Private _running As Boolean

    Public Event PurchaseSimulated As EventHandler(Of DcaPurchaseEventArgs)
    Public Event SimulationUpdated As EventHandler(Of DcaSimulationEventArgs)

    Public Sub New()
        Try
            SpotDcaRepository.Initialize()
            For Each plan In SpotDcaRepository.LoadPlans()
                _plans(plan.Symbol) = plan
            Next
            For Each state In SpotDcaRepository.LoadStates()
                _states(state.Symbol) = state
            Next
        Catch ex As Exception
            Debug.WriteLine("Erro carregando DCA: " & ex.Message)
        End Try
    End Sub

    Public ReadOnly Property IsRunning As Boolean
        Get
            SyncLock _sync
                Return _running
            End SyncLock
        End Get
    End Property

    Public Function GetPlans() As IReadOnlyList(Of DcaPlan)
        SyncLock _sync
            Return _plans.Values.OrderBy(Function(item) item.Symbol).ToList()
        End SyncLock
    End Function

    Public Function GetStates() As IReadOnlyList(Of DcaState)
        SyncLock _sync
            Return _states.Values.OrderBy(Function(item) item.Symbol).ToList()
        End SyncLock
    End Function

    Public Function GetRequiredCandleIntervals() As IReadOnlyList(Of String)
        SyncLock _sync
            Return _plans.Values.
                Where(Function(item) item.Enabled AndAlso item.UseAutomaticSupports).
                Select(Function(item) NormalizeInterval(item.CandleInterval)).
                Distinct(StringComparer.OrdinalIgnoreCase).
                ToList()
        End SyncLock
    End Function

    Public Async Function LoadHistoricalCandlesAsync(
        loader As Func(Of String, String, Integer, Task(Of List(Of DcaCandle)))) As Task

        If loader Is Nothing Then Throw New ArgumentNullException(NameOf(loader))

        Dim plans = GetPlans().
            Where(Function(item) item.Enabled AndAlso item.UseAutomaticSupports).
            ToList()

        For Each plan In plans
            Dim interval = NormalizeInterval(plan.CandleInterval)
            Dim limit = Math.Max(30, Math.Min(1000, plan.SupportLookbackCandles))
            Dim loaded = Await loader(plan.Symbol, interval, limit)
            If loaded Is Nothing OrElse loaded.Count = 0 Then
                Throw New InvalidOperationException(
                    $"Não foi possível carregar candles de {plan.Symbol} ({interval}) para calcular os suportes.")
            End If

            SyncLock _sync
                Dim history = loaded.
                    Where(Function(item) item IsNot Nothing AndAlso item.Low > 0D AndAlso item.Close > 0D).
                    OrderBy(Function(item) item.StartUtc).
                    ToList()
                _candles(plan.Symbol) = history

                Dim state = GetOrCreateStateLocked(plan.Symbol)
                Dim currentPrice = If(state.LastPrice > 0D, state.LastPrice, history.Last().Close)
                RefreshSupportsLocked(plan, state, currentPrice)
                If state.LastPrice <= 0D Then state.LastPrice = currentPrice
                If state.AnchorPrice <= 0D Then state.AnchorPrice = currentPrice
            End SyncLock
        Next

        RaiseUpdated()
    End Function

    Public Sub SavePlan(plan As DcaPlan)
        If plan Is Nothing Then Throw New ArgumentNullException(NameOf(plan))

        plan.Symbol = NormalizeSymbol(plan.Symbol)
        If String.IsNullOrWhiteSpace(plan.Symbol) Then
            Throw New ArgumentException("Informe a moeda do plano DCA.", NameOf(plan))
        End If

        plan.CandleInterval = NormalizeInterval(plan.CandleInterval)
        plan.SupportLookbackCandles = Math.Max(30, Math.Min(1000, plan.SupportLookbackCandles))
        plan.SupportTolerancePercent = Math.Max(0.05D, plan.SupportTolerancePercent)
        plan.SupportMinimumTouches = Math.Max(1, plan.SupportMinimumTouches)
        plan.SupportMaxLevels = Math.Max(1, plan.SupportMaxLevels)
        plan.Levels = If(plan.Levels, New List(Of DcaLevel)()).
            Where(Function(item) item IsNot Nothing AndAlso item.DropPercent > 0D AndAlso item.AllocationPercent > 0D).
            OrderBy(Function(item) item.DropPercent).
            ToList()
        plan.ManualSupportOverrides = If(plan.ManualSupportOverrides, New List(Of DcaSupportOverride)()).
            Where(Function(item) item IsNot Nothing AndAlso item.Enabled AndAlso item.Price > 0D AndAlso item.AllocationPercent > 0D).
            ToList()

        If Not plan.UseAutomaticSupports AndAlso plan.Levels.Count = 0 Then
            Throw New ArgumentException("Informe pelo menos um nível de queda.", NameOf(plan))
        End If

        If plan.UseAutomaticSupports AndAlso
           plan.ManualSupportOverrides.Sum(Function(item) item.AllocationPercent) > 100.01D Then
            Throw New ArgumentException("A soma dos ajustes manuais de suporte não pode ultrapassar 100%.", NameOf(plan))
        End If

        Dim state As DcaState
        SyncLock _sync
            _plans(plan.Symbol) = plan
            state = GetOrCreateStateLocked(plan.Symbol)
        End SyncLock

        SpotDcaRepository.SavePlan(plan)
        SpotDcaRepository.SaveState(state)
        RaiseUpdated()
    End Sub

    Public Sub Start()
        SyncLock _sync
            _running = True
        End SyncLock
        RaiseUpdated()
    End Sub

    Public Sub [Stop]()
        SyncLock _sync
            _running = False
        End SyncLock
        RaiseUpdated()
    End Sub

    Public Sub RemovePlan(symbol As String)
        Dim normalized = NormalizeSymbol(symbol)
        If String.IsNullOrWhiteSpace(normalized) Then Return

        SyncLock _sync
            _plans.Remove(normalized)
            _states.Remove(normalized)
            _candles.Remove(normalized)
            _currentCandles.Remove(normalized)
            If _plans.Count = 0 Then _running = False
        End SyncLock

        SpotDcaRepository.DeletePlan(normalized)
        RaiseUpdated()
    End Sub

    Public Sub ResetSimulation()
        SyncLock _sync
            For Each state In _states.Values
                state.AnchorPrice = 0D
                state.LastPrice = 0D
                state.Quantity = 0D
                state.InvestedQuote = 0D
                state.AveragePrice = 0D
                state.LastPurchaseAtUtc = DateTime.MinValue
                state.TriggeredLevels.Clear()
                state.Supports.Clear()
                state.LastSupportRefreshUtc = DateTime.MinValue
                SpotDcaRepository.ResetState(state.Symbol)
            Next
        End SyncLock
        RaiseUpdated()
    End Sub

    Public Sub ProcessCandle(
        symbol As String,
        interval As String,
        candleStartUtc As DateTime,
        openPrice As Decimal,
        highPrice As Decimal,
        lowPrice As Decimal,
        closePrice As Decimal,
        volume As Decimal,
        isClosed As Boolean)

        Dim normalized = NormalizeSymbol(symbol)
        If String.IsNullOrWhiteSpace(normalized) OrElse closePrice <= 0D Then Return

        Dim purchase As DcaPurchase = Nothing
        SyncLock _sync
            Dim plan As DcaPlan = Nothing
            If Not _plans.TryGetValue(normalized, plan) OrElse
               Not plan.Enabled OrElse
               Not plan.UseAutomaticSupports OrElse
               Not String.Equals(NormalizeInterval(interval), NormalizeInterval(plan.CandleInterval), StringComparison.OrdinalIgnoreCase) Then
                Return
            End If

            Dim candle = New DcaCandle With {
                .StartUtc = candleStartUtc.ToUniversalTime(),
                .CloseUtc = candleStartUtc.ToUniversalTime(),
                .Open = Math.Max(0D, openPrice),
                .High = Math.Max(Math.Max(0D, highPrice), Math.Max(0D, closePrice)),
                .Low = If(lowPrice > 0D, lowPrice, Math.Min(Math.Max(0D, openPrice), closePrice)),
                .Close = closePrice,
                .Volume = Math.Max(0D, volume),
                .IsClosed = isClosed}

            If Not isClosed Then
                _currentCandles(normalized) = candle
                Return
            End If

            Dim history = GetOrCreateCandleHistoryLocked(normalized)
            Dim existingIndex = history.FindIndex(Function(item) item.StartUtc = candle.StartUtc)
            If existingIndex >= 0 Then
                history(existingIndex) = candle
            Else
                history.Add(candle)
            End If
            history.Sort(Function(left, right) left.StartUtc.CompareTo(right.StartUtc))
            TrimCandleHistory(history, plan.SupportLookbackCandles)
            _currentCandles.Remove(normalized)

            Dim state = GetOrCreateStateLocked(normalized)
            state.LastPrice = closePrice
            If state.AnchorPrice <= 0D OrElse closePrice > state.AnchorPrice Then
                state.AnchorPrice = closePrice
            End If

            Dim previousSupports = state.Supports
            RefreshSupportsLocked(plan, state, closePrice)
            PreserveSupportStateLocked(previousSupports, state)
            SpotDcaSupportDetector.ApplyRejectionConfirmation(state.Supports, candle)

            If _running AndAlso plan.RequireSupportRejection Then
                For Each zone In state.Supports.OrderByDescending(Function(item) item.Price)
                    If Not zone.IsConfirmed Then Continue For
                    purchase = TryCreateSupportPurchaseLocked(
                        plan,
                        state,
                        zone,
                        closePrice,
                        DateTime.UtcNow,
                        allowConfirmedClose:=True)
                    If purchase IsNot Nothing Then Exit For
                Next
            End If

            SpotDcaRepository.SaveState(state)
        End SyncLock

        If purchase IsNot Nothing Then RaiseEvent PurchaseSimulated(Me, New DcaPurchaseEventArgs(purchase))
        RaiseUpdated()
    End Sub

    Public Function ProcessPrice(
        symbol As String,
        price As Decimal,
        Optional timestampUtc As DateTime = Nothing) As DcaPurchase

        Dim normalized = NormalizeSymbol(symbol)
        If String.IsNullOrWhiteSpace(normalized) OrElse price <= 0D Then Return Nothing

        Dim observedAt = If(timestampUtc = DateTime.MinValue, DateTime.UtcNow, timestampUtc.ToUniversalTime())
        Dim purchase As DcaPurchase = Nothing
        SyncLock _sync
            If Not _running Then Return Nothing

            Dim plan As DcaPlan = Nothing
            If Not _plans.TryGetValue(normalized, plan) OrElse Not plan.Enabled Then Return Nothing

            Dim state = GetOrCreateStateLocked(normalized)
            If state.AnchorPrice <= 0D OrElse price > state.AnchorPrice Then
                state.AnchorPrice = price
            End If
            state.LastPrice = price

            If plan.UseAutomaticSupports Then
                If Not plan.RequireSupportRejection Then
                    For Each zone In state.Supports.OrderByDescending(Function(item) item.Price)
                        If price < zone.LowerPrice OrElse price > zone.UpperPrice Then Continue For
                        purchase = TryCreateSupportPurchaseLocked(plan, state, zone, price, observedAt)
                        If purchase IsNot Nothing Then Exit For
                    Next
                End If
            Else
                Dim dropPercent = If(state.AnchorPrice <= 0D, 0D, (state.AnchorPrice - price) / state.AnchorPrice * 100D)
                For Each level In plan.Levels.OrderBy(Function(item) item.DropPercent)
                    If dropPercent < level.DropPercent OrElse state.TriggeredLevels.Contains(LevelKey(level.DropPercent)) Then Continue For

                    purchase = TryCreatePercentPurchaseLocked(plan, state, level, dropPercent, price, observedAt)
                    If purchase IsNot Nothing Then Exit For
                Next
            End If

            SpotDcaRepository.SaveState(state)
        End SyncLock

        If purchase IsNot Nothing Then RaiseEvent PurchaseSimulated(Me, New DcaPurchaseEventArgs(purchase))
        RaiseUpdated()
        Return purchase
    End Function

    Private Function TryCreateSupportPurchaseLocked(
        plan As DcaPlan,
        state As DcaState,
        zone As DcaSupportZone,
        price As Decimal,
        observedAt As DateTime,
        Optional allowConfirmedClose As Boolean = False) As DcaPurchase

        If zone Is Nothing OrElse Not zone.Enabled OrElse zone.AllocationPercent <= 0D Then Return Nothing
        Dim key = SupportKey(zone.Price)
        If state.TriggeredLevels.Contains(key) Then Return Nothing
        If plan.RequireSupportRejection AndAlso Not zone.IsConfirmed Then Return Nothing
        If plan.RequireSupportRejection AndAlso price > zone.UpperPrice AndAlso Not allowConfirmedClose Then Return Nothing

        Return CreatePurchaseLocked(
            plan,
            state,
            key,
            zone.AllocationPercent,
            price,
            If(state.AnchorPrice <= 0D, 0D, (state.AnchorPrice - price) / state.AnchorPrice * 100D),
            observedAt)
    End Function

    Private Function TryCreatePercentPurchaseLocked(
        plan As DcaPlan,
        state As DcaState,
        level As DcaLevel,
        dropPercent As Decimal,
        price As Decimal,
        observedAt As DateTime) As DcaPurchase

        Return CreatePurchaseLocked(
            plan,
            state,
            LevelKey(level.DropPercent),
            level.AllocationPercent,
            price,
            dropPercent,
            observedAt)
    End Function

    Private Function CreatePurchaseLocked(
        plan As DcaPlan,
        state As DcaState,
        key As String,
        allocationPercent As Decimal,
        price As Decimal,
        dropPercent As Decimal,
        observedAt As DateTime) As DcaPurchase

        If state.TriggeredLevels.Contains(key) Then Return Nothing
        If plan.CooldownMinutes > 0 AndAlso
           state.LastPurchaseAtUtc <> DateTime.MinValue AndAlso
           observedAt - state.LastPurchaseAtUtc < TimeSpan.FromMinutes(plan.CooldownMinutes) Then
            Return Nothing
        End If

        Dim remainingBudget = Math.Max(0D, plan.BudgetQuote - state.InvestedQuote)
        Dim quoteAmount = Math.Min(
            remainingBudget,
            plan.BudgetQuote * Math.Max(0D, allocationPercent) / 100D)
        If quoteAmount <= 0D Then
            state.TriggeredLevels.Add(key)
            Return Nothing
        End If

        Dim quantity = quoteAmount / price
        state.Quantity += quantity
        state.InvestedQuote += quoteAmount
        state.AveragePrice = If(state.Quantity <= 0D, 0D, state.InvestedQuote / state.Quantity)
        state.LastPurchaseAtUtc = observedAt
        state.TriggeredLevels.Add(key)

        Return New DcaPurchase With {
            .Symbol = state.Symbol,
            .DropPercent = dropPercent,
            .Price = price,
            .Quantity = quantity,
            .QuoteAmount = quoteAmount,
            .AveragePrice = state.AveragePrice,
            .TimestampUtc = observedAt}
    End Function

    Private Sub RefreshSupportsLocked(plan As DcaPlan, state As DcaState, currentPrice As Decimal)
        Dim history = GetOrCreateCandleHistoryLocked(plan.Symbol)
        state.Supports = SpotDcaSupportDetector.Detect(history, plan, currentPrice)
        state.LastSupportRefreshUtc = DateTime.UtcNow
    End Sub

    Private Sub PreserveSupportStateLocked(
        previous As IReadOnlyList(Of DcaSupportZone),
        state As DcaState)

        If previous Is Nothing OrElse state.Supports Is Nothing Then Return
        For Each current In state.Supports
            Dim old = previous.FirstOrDefault(
                Function(item) Math.Abs(item.Price - current.Price) / Math.Max(0.00000001D, item.Price) * 100D <= 1D)
            If old IsNot Nothing Then
                current.IsConfirmed = old.IsConfirmed
                If state.TriggeredLevels.Contains(SupportKey(old.Price)) Then
                    state.TriggeredLevels.Add(SupportKey(current.Price))
                End If
            End If
        Next
    End Sub

    Private Function GetOrCreateStateLocked(symbol As String) As DcaState
        Dim state As DcaState = Nothing
        If Not _states.TryGetValue(symbol, state) Then
            state = New DcaState With {.Symbol = symbol}
            _states(symbol) = state
        End If
        Return state
    End Function

    Private Function GetOrCreateCandleHistoryLocked(symbol As String) As List(Of DcaCandle)
        Dim history As List(Of DcaCandle) = Nothing
        If Not _candles.TryGetValue(symbol, history) Then
            history = New List(Of DcaCandle)()
            _candles(symbol) = history
        End If
        Return history
    End Function

    Private Shared Sub TrimCandleHistory(history As List(Of DcaCandle), lookback As Integer)
        Dim maximum = Math.Max(30, Math.Min(1000, lookback))
        If history.Count > maximum Then
            history.RemoveRange(0, history.Count - maximum)
        End If
    End Sub

    Private Sub RaiseUpdated()
        RaiseEvent SimulationUpdated(Me, New DcaSimulationEventArgs(GetStates()))
    End Sub

    Private Shared Function NormalizeSymbol(symbol As String) As String
        Return If(symbol, String.Empty).Trim().ToUpperInvariant().Replace("USDT", String.Empty)
    End Function

    Private Shared Function NormalizeInterval(value As String) As String
        If String.Equals(value?.Trim(), "1d", StringComparison.OrdinalIgnoreCase) Then Return "1d"
        Return "4h"
    End Function

    Private Shared Function LevelKey(dropPercent As Decimal) As String
        ' Mantém compatibilidade com estados salvos antes do modo automático.
        Return dropPercent.ToString("0.####", Globalization.CultureInfo.InvariantCulture)
    End Function

    Private Shared Function SupportKey(price As Decimal) As String
        Return "S:" & price.ToString("0.########", Globalization.CultureInfo.InvariantCulture)
    End Function
End Class
