Public Class BinanceFuturesProtectionOrder
    Public Property Exchange As String = String.Empty
    Public Property AlgoId As Long
    Public Property Symbol As String = String.Empty
    Public Property PositionSide As String = String.Empty
    Public Property Side As String = String.Empty
    Public Property OrderType As String = String.Empty
    Public Property Quantity As Decimal
    Public Property StopPrice As Decimal
    Public Property Price As Decimal
    Public Property Status As String = String.Empty
    Public Property OrderTime As DateTime
End Class
