Imports System
Imports System.Collections.Generic

Public Enum SpotBotSignalDirection
    Hold = 0
    Buy = 1
    Sell = -1
End Enum

Public Enum SpotBotCombinationMode
    Any = 0
    All = 1
    MinimumConfirmations = 2
    WeightedScore = 3
End Enum

Public Enum SpotBotStrategyKind
    EmaCrossover = 0
    Rsi = 1
    Breakout = 2
    Grid = 3
End Enum

Public NotInheritable Class SpotBotPricePoint
    Public Sub New(timestampUtc As DateTime, price As Decimal)
        Me.TimestampUtc = timestampUtc
        Me.Price = price
    End Sub

    Public ReadOnly Property TimestampUtc As DateTime
    Public ReadOnly Property Price As Decimal
End Class

Public NotInheritable Class SpotBotStrategySignal
    Public Property Strategy As SpotBotStrategyKind
    Public Property Direction As SpotBotSignalDirection
    Public Property Confidence As Decimal
    Public Property Reason As String = String.Empty
    Public Property ObservedValue As Decimal
End Class

Public NotInheritable Class SpotBotSignal
    Public Property Symbol As String = String.Empty
    Public Property TimestampUtc As DateTime
    Public Property Price As Decimal
    Public Property Direction As SpotBotSignalDirection
    Public Property Confirmations As Integer
    Public Property ActiveStrategies As Integer
    Public Property Score As Decimal
    Public Property Reasons As New List(Of String)
    Public Property StrategySignals As New List(Of SpotBotStrategySignal)
    Public Property SimulationOnly As Boolean = True
End Class

Public NotInheritable Class SpotBotConfiguration
    Public Sub New()
        EnabledStrategies = New HashSet(Of SpotBotStrategyKind)()
    End Sub

    Public Property EnabledStrategies As HashSet(Of SpotBotStrategyKind)
    Public Property CombinationMode As SpotBotCombinationMode = SpotBotCombinationMode.MinimumConfirmations
    Public Property RequiredConfirmations As Integer = 2
    Public Property MinimumWeightedScore As Decimal = 0.6D
    Public Property Cooldown As TimeSpan = TimeSpan.FromMinutes(5)
    Public Property MaximumPriceHistory As Integer = 250

    Public Property EmaFastPeriod As Integer = 9
    Public Property EmaSlowPeriod As Integer = 21
    Public Property RsiPeriod As Integer = 14
    Public Property RsiOversold As Decimal = 30D
    Public Property RsiOverbought As Decimal = 70D
    Public Property BreakoutPeriod As Integer = 20
    Public Property GridSpacingPercent As Decimal = 1D
    Public Property GridReferencePrice As Decimal

    ' O motor começa sempre em simulação. A execução real será conectada
    ' depois de validarmos os sinais e os limites de risco.
    Public Property SimulationOnly As Boolean = True
End Class

Public NotInheritable Class SpotBotSignalEventArgs
    Inherits EventArgs

    Public Sub New(signal As SpotBotSignal)
        Me.Signal = signal
    End Sub

    Public ReadOnly Property Signal As SpotBotSignal
End Class

Public Interface ISpotBotStrategy
    ReadOnly Property Kind As SpotBotStrategyKind

    Function Evaluate(
        symbol As String,
        prices As IReadOnlyList(Of SpotBotPricePoint),
        configuration As SpotBotConfiguration) As SpotBotStrategySignal
End Interface
