Public Class BinanceFuturesPosition
    Public Property Symbol As String = String.Empty
    Public Property PositionAmount As Decimal
    Public Property EntryPrice As Decimal
    Public Property MarkPrice As Decimal
    Public Property UnrealizedProfit As Decimal
    Public Property LiquidationPrice As Decimal
    Public Property Leverage As Integer
    Public Property Notional As Decimal
    Public Property MarginType As String = String.Empty
    Public Property IsolatedMargin As Decimal
    Public Property InitialMargin As Decimal
    Public Property PositionSide As String = String.Empty
End Class
