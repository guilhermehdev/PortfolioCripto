Imports System.Drawing
Imports System.Windows.Forms

Public Class BlackTabControl
    Inherits TabControl

    Private Const WmPaint As Integer = &HF

    Public Sub New()
        BackColor = Color.Black
        ForeColor = Color.White
        SetStyle(ControlStyles.OptimizedDoubleBuffer, True)
    End Sub

    Protected Overrides Sub WndProc(ByRef m As Message)
        MyBase.WndProc(m)

        If m.Msg <> WmPaint OrElse Not IsHandleCreated Then
            Return
        End If

        Dim graphics As Graphics = Graphics.FromHwnd(Handle)
        Using graphics
            Dim displayRectangle As Rectangle = Me.DisplayRectangle
            Dim tabBottom As Integer = 0

            If TabPages.Count > 0 Then
                tabBottom = GetTabRect(TabPages.Count - 1).Bottom
            End If

            If displayRectangle.Top > tabBottom Then
                Using backgroundBrush As New SolidBrush(Color.Black)
                    graphics.FillRectangle(
                        backgroundBrush,
                        New Rectangle(0, tabBottom, ClientSize.Width, displayRectangle.Top - tabBottom + 1))
                End Using
            End If

            Using borderPen As New Pen(Color.Black)
                Dim borderRectangle = displayRectangle
                borderRectangle.Inflate(1, 1)
                graphics.DrawRectangle(borderPen, borderRectangle)
            End Using
        End Using
    End Sub
End Class