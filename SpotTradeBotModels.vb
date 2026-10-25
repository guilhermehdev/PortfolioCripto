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
    BollingerMeanReversion = 4
End Enum

Public Enum SpotBotUniverseMode
    Manual = 0
    TopVolume = 1
End Enum

Public NotInheritable Class SpotBotPricePoint
    Public Sub New(
        timestampUtc As DateTime,
        price As Decimal,
        Optional openPrice As Decimal = 0D,
        Optional highPrice As Decimal = 0D,
        Optional lowPrice As Decimal = 0D)

        Me.TimestampUtc = timestampUtc
        Me.Price = price
        Me.Open = If(openPrice > 0D, openPrice, price)
        Me.High = If(highPrice > 0D, highPrice, Math.Max(Me.Open, price))
        Me.Low = If(lowPrice > 0D, lowPrice, Math.Min(Me.Open, price))
    End Sub

    Public ReadOnly Property TimestampUtc As DateTime
    Public ReadOnly Property Price As Decimal
    Public ReadOnly Property Open As Decimal
    Public ReadOnly Property High As Decimal
    Public ReadOnly Property Low As Decimal
    Public ReadOnly Property Close As Decimal
        Get
            Return Price
        End Get
    End Property
End Class

Public NotInheritable Class SpotBotStrategySignal
    Public Property Strategy As SpotBotStrategyKind
    Public Property Direction As SpotBotSignalDirection
    Public Property Confidence As Decimal
    Public Property Reason As String = String.Empty
    Public Property ObservedValue As Decimal
    Public Property EntryPrice As Decimal
    Public Property StopPrice As Decimal
    Public Property TakeProfitPrice As Decimal
End Class

Public NotInheritable Class SpotBotSignal
    Public Property Symbol As String = String.Empty
    Public Property TimestampUtc As DateTime
    Public Property Price As Decimal
    Public Property EntryPrice As Decimal
    Public Property StopPrice As Decimal
    Public Property TakeProfitPrice As Decimal
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

    ' Parâmetros da variante Bollinger selecionada no painel.
    Public Property BollingerPeriod As Integer = 20
    Public Property BollingerStandardDeviations As Decimal = 3D
    Public Property BollingerCandleMinutes As Integer = 1440

    ' Parâmetros da carteira virtual usada pelo modo de simulação.
    Public Property SimulationStartingQuote As Decimal = 1000D
    Public Property SimulationAllocationPercent As Decimal = 10D
    Public Property SimulationFeeRate As Decimal = 0.001D
    Public Property TakeProfitPercent As Decimal = 2D
    Public Property StopLossPercent As Decimal = 1D

    ' O motor começa sempre em simulação. A execução real será conectada
    ' depois de validarmos os sinais e os limites de risco.
    Public Property SimulationOnly As Boolean = True

    Public Property UniverseMode As SpotBotUniverseMode = SpotBotUniverseMode.Manual
    Public Property UniverseMaxAssets As Integer = 20
    Public Property UniverseMinimumQuoteVolume As Decimal = 1000000D
    Public Property ManualUniverseSymbols As HashSet(Of String) = New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
End Class

Public NotInheritable Class SpotBotUniverseSettings
    Public Property Mode As SpotBotUniverseMode = SpotBotUniverseMode.Manual
    Public Property MaxAssets As Integer = 20
    Public Property MinimumQuoteVolume As Decimal = 1000000D
End Class

Public NotInheritable Class SpotBotSignalEventArgs
    Inherits EventArgs

    Public Sub New(signal As SpotBotSignal)
        Me.Signal = signal
    End Sub

    Public ReadOnly Property Signal As SpotBotSignal
End Class

Public NotInheritable Class SpotBotSimulationSnapshot
    Public Property StartingQuote As Decimal
    Public Property QuoteBalance As Decimal
    Public Property OpenValue As Decimal
    Public Property RealizedPnl As Decimal
    Public Property UnrealizedPnl As Decimal
    Public Property Equity As Decimal
    Public Property TotalPnl As Decimal
    Public Property CompletedTrades As Integer
    Public Property WinningTrades As Integer
    Public Property LosingTrades As Integer
    Public Property OpenPositions As Integer
    Public Property LastAction As String = String.Empty

    Public ReadOnly Property WinRatePercent As Decimal
        Get
            If CompletedTrades <= 0 Then Return 0D
            Return WinningTrades * 100D / CompletedTrades
        End Get
    End Property
End Class

Public NotInheritable Class SpotBotSimulationTrade
    Public Property Symbol As String = String.Empty
    Public Property Quantity As Decimal
    Public Property EntryPrice As Decimal
    Public Property ExitPrice As Decimal
    Public Property CostBasis As Decimal
    Public Property GrossProceeds As Decimal
    Public Property Fees As Decimal
    Public Property Pnl As Decimal
    Public Property OpenedAtUtc As DateTime
    Public Property ClosedAtUtc As DateTime
End Class

Public NotInheritable Class SpotBotOpenPosition
    Public Property Symbol As String = String.Empty
    Public Property Side As String = "LONG"
    Public Property Quantity As Decimal
    Public Property EntryPrice As Decimal
    Public Property CurrentPrice As Decimal
    Public Property CostBasis As Decimal
    Public Property Notional As Decimal
    Public Property UnrealizedPnl As Decimal
    Public Property RoiPercent As Decimal
    Public Property StopPrice As Decimal
    Public Property TakeProfitPrice As Decimal
    Public Property OpenedAtUtc As DateTime
End Class

Public NotInheritable Class SpotBotPersistedSimulationState
    Public Property RunId As Long
    Public Property Status As String = String.Empty
    Public Property Snapshot As SpotBotSimulationSnapshot
    Public Property Positions As New List(Of SpotBotOpenPosition)
    Public Property EnabledStrategies As New HashSet(Of SpotBotStrategyKind)()
    Public Property CombinationMode As SpotBotCombinationMode = SpotBotCombinationMode.MinimumConfirmations
    Public Property RequiredConfirmations As Integer = 2
    Public Property BollingerPeriod As Integer = 20
    Public Property BollingerStandardDeviations As Decimal = 3D
    Public Property BollingerCandleMinutes As Integer = 1440
    Public Property AllocationPercent As Decimal = 10D
    Public Property FeeRate As Decimal = 0.001D
End Class

Public NotInheritable Class SpotBotSimulationEventArgs
    Inherits EventArgs

    Public Sub New(snapshot As SpotBotSimulationSnapshot)
        Me.Snapshot = snapshot
    End Sub

    Public ReadOnly Property Snapshot As SpotBotSimulationSnapshot
End Class

Public Interface ISpotBotStrategy
    ReadOnly Property Kind As SpotBotStrategyKind

    Function Evaluate(
        symbol As String,
        prices As IReadOnlyList(Of SpotBotPricePoint),
        configuration As SpotBotConfiguration) As SpotBotStrategySignal
End Interface
