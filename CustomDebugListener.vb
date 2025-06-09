Public Class CustomDebugListener
    Inherits DefaultTraceListener

    Public Event MessageCaptured(msg As String)

    Public Overrides Sub Write(message As String)
        RaiseEvent MessageCaptured(message)
    End Sub

    Public Overrides Sub WriteLine(message As String)
        RaiseEvent MessageCaptured(message & Environment.NewLine)
    End Sub

End Class
