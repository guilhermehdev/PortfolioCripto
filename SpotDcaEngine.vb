Imports System
Imports System.Collections.Generic
Imports System.Linq

Public NotInheritable Class SpotDcaEngine
    Private ReadOnly _sync As New Object()
    Private ReadOnly _plans As New Dictionary(Of String, DcaPlan)(StringComparer.OrdinalIgnoreCase)
    Private ReadOnly _states As New Dictionary(Of String, DcaState)(StringComparer.OrdinalIgnoreCase)
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

    Public Sub SavePlan(plan As DcaPlan)
        If plan Is Nothing Then Throw New ArgumentNullException(NameOf(plan))
        plan.Symbol = NormalizeSymbol(plan.Symbol)
        If String.IsNullOrWhiteSpace(plan.Symbol) Then Throw New ArgumentException("Informe a moeda do plano DCA.", NameOf(plan))
        plan.Levels = plan.Levels.Where(Function(item) item IsNot Nothing AndAlso item.DropPercent > 0D AndAlso item.AllocationPercent > 0D).
            OrderBy(Function(item) item.DropPercent).ToList()
        If plan.Levels.Count = 0 Then Throw New ArgumentException("Informe pelo menos um nível de queda.", NameOf(plan))

        SyncLock _sync
            _plans(plan.Symbol) = plan
            If Not _states.ContainsKey(plan.Symbol) Then
                _states(plan.Symbol) = New DcaState With {.Symbol = plan.Symbol}
            End If
        End SyncLock
        SpotDcaRepository.SavePlan(plan)
        SpotDcaRepository.SaveState(_states(plan.Symbol))
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
                SpotDcaRepository.ResetState(state.Symbol)
            Next
        End SyncLock
        RaiseUpdated()
    End Sub

    Public Function ProcessPrice(symbol As String, price As Decimal, Optional timestampUtc As DateTime = Nothing) As DcaPurchase
        Dim normalized = NormalizeSymbol(symbol)
        If String.IsNullOrWhiteSpace(normalized) OrElse price <= 0D Then Return Nothing

        Dim observedAt = If(timestampUtc = DateTime.MinValue, DateTime.UtcNow, timestampUtc.ToUniversalTime())
        Dim purchase As DcaPurchase = Nothing
        Dim persistState As DcaState = Nothing
        SyncLock _sync
            If Not _running Then Return Nothing

            Dim plan As DcaPlan = Nothing
            If Not _plans.TryGetValue(normalized, plan) OrElse Not plan.Enabled Then Return Nothing

            Dim state As DcaState = Nothing
            If Not _states.TryGetValue(normalized, state) Then
                state = New DcaState With {.Symbol = normalized}
                _states(normalized) = state
            End If

            If state.AnchorPrice <= 0D Then
                state.AnchorPrice = price
                persistState = state
            ElseIf price > state.AnchorPrice Then
                state.AnchorPrice = price
                persistState = state
            End If
            state.LastPrice = price

            Dim dropPercent = If(state.AnchorPrice <= 0D, 0D, (state.AnchorPrice - price) / state.AnchorPrice * 100D)
            For Each level In plan.Levels.OrderBy(Function(item) item.DropPercent)
                Dim key = LevelKey(level.DropPercent)
                If dropPercent < level.DropPercent OrElse state.TriggeredLevels.Contains(key) Then Continue For
                If plan.CooldownMinutes > 0 AndAlso state.LastPurchaseAtUtc <> DateTime.MinValue AndAlso
                   observedAt - state.LastPurchaseAtUtc < TimeSpan.FromMinutes(plan.CooldownMinutes) Then Continue For

                Dim remainingBudget = Math.Max(0D, plan.BudgetQuote - state.InvestedQuote)
                Dim quoteAmount = Math.Min(remainingBudget, plan.BudgetQuote * Math.Max(0D, level.AllocationPercent) / 100D)
                If quoteAmount <= 0D Then
                    state.TriggeredLevels.Add(key)
                    Continue For
                End If

                Dim quantity = quoteAmount / price
                state.Quantity += quantity
                state.InvestedQuote += quoteAmount
                state.AveragePrice = If(state.Quantity <= 0D, 0D, state.InvestedQuote / state.Quantity)
                state.LastPurchaseAtUtc = observedAt
                state.TriggeredLevels.Add(key)
                purchase = New DcaPurchase With {
                    .Symbol = normalized,
                    .DropPercent = dropPercent,
                    .Price = price,
                    .Quantity = quantity,
                    .QuoteAmount = quoteAmount,
                    .AveragePrice = state.AveragePrice,
                    .TimestampUtc = observedAt}
                SpotDcaRepository.SaveState(state)
                persistState = Nothing
                Exit For
            Next
        End SyncLock

        If persistState IsNot Nothing Then
            SpotDcaRepository.SaveState(persistState)
        End If

        If purchase IsNot Nothing Then RaiseEvent PurchaseSimulated(Me, New DcaPurchaseEventArgs(purchase))
        RaiseUpdated()
        Return purchase
    End Function

    Private Sub RaiseUpdated()
        RaiseEvent SimulationUpdated(Me, New DcaSimulationEventArgs(GetStates()))
    End Sub

    Private Shared Function NormalizeSymbol(symbol As String) As String
        Return If(symbol, String.Empty).Trim().ToUpperInvariant().Replace("USDT", String.Empty)
    End Function

    Private Shared Function LevelKey(dropPercent As Decimal) As String
        Return dropPercent.ToString("0.####", Globalization.CultureInfo.InvariantCulture)
    End Function
End Class
