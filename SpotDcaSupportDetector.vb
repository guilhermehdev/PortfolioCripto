Imports System
Imports System.Collections.Generic
Imports System.Linq

Public NotInheritable Class SpotDcaSupportDetector
    Private Sub New()
    End Sub

    Public Shared Function Detect(
        candles As IReadOnlyList(Of DcaCandle),
        plan As DcaPlan,
        currentPrice As Decimal) As List(Of DcaSupportZone)

        Dim result As New List(Of DcaSupportZone)()
        If candles Is Nothing OrElse plan Is Nothing OrElse currentPrice <= 0D Then Return result

        Dim tolerance = Math.Max(0.05D, plan.SupportTolerancePercent)
        Dim lookback = Math.Max(30, plan.SupportLookbackCandles)
        Dim minimumTouches = Math.Max(1, plan.SupportMinimumTouches)
        Dim source = candles.
            Where(Function(item) item IsNot Nothing AndAlso item.IsClosed AndAlso item.Low > 0D).
            OrderBy(Function(item) item.StartUtc).
            TakeLast(lookback).
            ToList()

        If source.Count >= 5 Then
            Dim pivotWindow = 2
            For index = pivotWindow To source.Count - pivotWindow - 1
                Dim candidate = source(index)
                Dim isPivot = True

                For offset = -pivotWindow To pivotWindow
                    If offset = 0 Then Continue For
                    If source(index + offset).Low < candidate.Low Then
                        isPivot = False
                        Exit For
                    End If
                Next

                If Not isPivot OrElse candidate.Low >= currentPrice Then Continue For

                Dim existing = result.FirstOrDefault(
                    Function(item) Math.Abs(item.Price - candidate.Low) / item.Price * 100D <= tolerance)

                If existing Is Nothing Then
                    result.Add(New DcaSupportZone With {
                        .Price = candidate.Low,
                        .Touches = 1,
                        .Strength = 1D,
                        .LastTouchUtc = candidate.StartUtc,
                        .IsManual = False,
                        .Enabled = True})
                Else
                    Dim touchCount = Math.Max(1, existing.Touches)
                    existing.Price = (existing.Price * touchCount + candidate.Low) / (touchCount + 1)
                    existing.Touches += 1
                    existing.Strength = existing.Touches
                    If candidate.StartUtc > existing.LastTouchUtc Then
                        existing.LastTouchUtc = candidate.StartUtc
                    End If
                End If
            Next
        End If

        Dim automaticZones = result.
            Where(Function(item) item.Touches >= minimumTouches).
            Where(Function(item) item.Price < currentPrice).
            OrderByDescending(Function(item) item.Price).
            ToList()

        Dim selected As New List(Of DcaSupportZone)()
        For Each item In automaticZones.Take(Math.Max(1, plan.SupportMaxLevels))
            selected.Add(item)
        Next

        Dim manualLevels = If(plan.ManualSupportOverrides, New List(Of DcaSupportOverride)()).
            Where(Function(item) item IsNot Nothing AndAlso item.Enabled AndAlso item.Price > 0D AndAlso item.AllocationPercent > 0D).
            ToList()

        For Each manual In manualLevels
            If manual.Price >= currentPrice * (1D + tolerance / 100D) Then Continue For

            Dim manualTolerance = Math.Max(0.05D, manual.TolerancePercent)
            Dim existing = selected.FirstOrDefault(
                Function(item) Math.Abs(item.Price - manual.Price) / item.Price * 100D <= manualTolerance)

            If existing Is Nothing Then
                selected.Add(New DcaSupportZone With {
                    .Price = manual.Price,
                    .Touches = 0,
                    .Strength = 0D,
                    .LastTouchUtc = DateTime.MinValue,
                    .IsManual = True,
                    .AllocationPercent = manual.AllocationPercent,
                    .Enabled = True})
            Else
                existing.Price = manual.Price
                existing.IsManual = True
                existing.AllocationPercent = manual.AllocationPercent
            End If
        Next

        selected = selected.
            Where(Function(item) item.Enabled AndAlso item.Price > 0D AndAlso item.Price < currentPrice).
            OrderByDescending(Function(item) item.IsManual).
            ThenByDescending(Function(item) item.Price).
            Take(Math.Max(1, plan.SupportMaxLevels)).
            ToList()

        Dim manualAllocation = selected.
            Where(Function(item) item.IsManual).
            Sum(Function(item) Math.Max(0D, item.AllocationPercent))
        Dim automatic = selected.Where(Function(item) Not item.IsManual).ToList()
        Dim remainingAllocation = Math.Max(0D, 100D - manualAllocation)
        Dim totalStrength = automatic.Sum(Function(item) Math.Max(1D, item.Strength))

        For Each item In automatic
            item.AllocationPercent = If(totalStrength <= 0D,
                                        remainingAllocation / Math.Max(1, automatic.Count),
                                        remainingAllocation * Math.Max(1D, item.Strength) / totalStrength)
        Next

        For Each item In selected
            Dim itemTolerance = tolerance
            Dim manual = manualLevels.FirstOrDefault(
                Function(level) Math.Abs(level.Price - item.Price) / item.Price * 100D <= Math.Max(0.05D, level.TolerancePercent))
            If manual IsNot Nothing Then itemTolerance = Math.Max(0.05D, manual.TolerancePercent)

            item.LowerPrice = item.Price * (1D - itemTolerance / 100D)
            item.UpperPrice = item.Price * (1D + itemTolerance / 100D)
        Next

        Return selected.OrderByDescending(Function(item) item.Price).ToList()
    End Function

    Public Shared Sub ApplyRejectionConfirmation(
        zones As IEnumerable(Of DcaSupportZone),
        candle As DcaCandle)

        If zones Is Nothing OrElse candle Is Nothing OrElse Not candle.IsClosed Then Return

        For Each zone In zones
            zone.IsConfirmed = candle.Low <= zone.UpperPrice AndAlso
                               candle.Close >= zone.Price AndAlso
                               candle.IsBullish
        Next
    End Sub
End Class
