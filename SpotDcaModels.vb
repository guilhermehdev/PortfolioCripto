Imports System
Imports System.Collections.Generic

Public NotInheritable Class DcaCandle
    Public Property StartUtc As DateTime
    Public Property CloseUtc As DateTime
    Public Property Open As Decimal
    Public Property High As Decimal
    Public Property Low As Decimal
    Public Property Close As Decimal
    Public Property Volume As Decimal
    Public Property IsClosed As Boolean

    Public ReadOnly Property Range As Decimal
        Get
            Return Math.Max(0D, High - Low)
        End Get
    End Property

    Public ReadOnly Property IsBullish As Boolean
        Get
            Return Close > Open
        End Get
    End Property
End Class

Public NotInheritable Class DcaLevel
    Public Property DropPercent As Decimal
    Public Property AllocationPercent As Decimal
End Class

Public NotInheritable Class DcaSupportOverride
    Public Property Price As Decimal
    Public Property AllocationPercent As Decimal
    Public Property TolerancePercent As Decimal = 0.5D
    Public Property Enabled As Boolean = True
End Class

Public NotInheritable Class DcaSupportZone
    Public Property Price As Decimal
    Public Property LowerPrice As Decimal
    Public Property UpperPrice As Decimal
    Public Property AllocationPercent As Decimal
    Public Property Touches As Integer
    Public Property Strength As Decimal
    Public Property LastTouchUtc As DateTime
    Public Property IsManual As Boolean
    Public Property IsConfirmed As Boolean
    Public Property Enabled As Boolean = True
End Class

Public NotInheritable Class DcaPlan
    Public Property Symbol As String = String.Empty
    Public Property Enabled As Boolean = True
    Public Property BudgetQuote As Decimal = 100D
    Public Property CooldownMinutes As Integer = 60
    Public Property Levels As New List(Of DcaLevel)()

    ' O modo automático usa candles fechados para detectar zonas de suporte.
    ' O modo percentual permanece disponível para compatibilidade e fallback.
    Public Property UseAutomaticSupports As Boolean = True
    Public Property CandleInterval As String = "4h"
    Public Property SupportLookbackCandles As Integer = 200
    Public Property SupportTolerancePercent As Decimal = 0.5D
    Public Property SupportMinimumTouches As Integer = 2
    Public Property SupportMaxLevels As Integer = 4
    Public Property RequireSupportRejection As Boolean = True
    Public Property ManualSupportOverrides As New List(Of DcaSupportOverride)()
End Class

Public NotInheritable Class DcaState
    Public Property Symbol As String = String.Empty
    Public Property AnchorPrice As Decimal
    Public Property LastPrice As Decimal
    Public Property Quantity As Decimal
    Public Property InvestedQuote As Decimal
    Public Property AveragePrice As Decimal
    Public Property LastPurchaseAtUtc As DateTime
    Public Property TriggeredLevels As New HashSet(Of String)(StringComparer.Ordinal)
    Public Property Supports As New List(Of DcaSupportZone)()
    Public Property LastSupportRefreshUtc As DateTime

    Public ReadOnly Property CurrentValue As Decimal
        Get
            Return Quantity * LastPrice
        End Get
    End Property

    Public ReadOnly Property UnrealizedPnl As Decimal
        Get
            Return CurrentValue - InvestedQuote
        End Get
    End Property

    Public ReadOnly Property RoiPercent As Decimal
        Get
            If InvestedQuote <= 0D Then Return 0D
            Return UnrealizedPnl / InvestedQuote * 100D
        End Get
    End Property
End Class

Public NotInheritable Class DcaPurchase
    Public Property Symbol As String = String.Empty
    Public Property DropPercent As Decimal
    Public Property Price As Decimal
    Public Property Quantity As Decimal
    Public Property QuoteAmount As Decimal
    Public Property AveragePrice As Decimal
    Public Property TimestampUtc As DateTime
End Class

Public NotInheritable Class DcaPurchaseEventArgs
    Inherits EventArgs

    Public Sub New(purchase As DcaPurchase)
        Me.Purchase = purchase
    End Sub

    Public ReadOnly Property Purchase As DcaPurchase
End Class

Public NotInheritable Class DcaSimulationEventArgs
    Inherits EventArgs

    Public Sub New(states As IReadOnlyList(Of DcaState))
        Me.States = states
    End Sub

    Public ReadOnly Property States As IReadOnlyList(Of DcaState)
End Class
