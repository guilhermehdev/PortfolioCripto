Imports System
Imports System.Collections.Generic

Public NotInheritable Class DcaLevel
    Public Property DropPercent As Decimal
    Public Property AllocationPercent As Decimal
End Class

Public NotInheritable Class DcaPlan
    Public Property Symbol As String = String.Empty
    Public Property Enabled As Boolean = True
    Public Property BudgetQuote As Decimal = 100D
    Public Property CooldownMinutes As Integer = 60
    Public Property Levels As New List(Of DcaLevel)()
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
