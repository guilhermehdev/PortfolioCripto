Imports System.Windows.Forms.DataVisualization.Charting

Public Class Charts

    Public Sub Graph(ByVal width As Integer, ByVal height As Integer, ByVal top As Integer, ByVal left As Integer, serieName As String, title As String, titlefontSize As Integer, titleColor As Color, backgroundColor As Color, chartType As SeriesChartType, seriesItens As Dictionary(Of String, Integer), container As Control)
        Dim chartArea As New ChartArea("MainArea")
        Dim chartTitle As New Title(title, Docking.Top, New Font("Arial", titlefontSize), titleColor)
        Dim series As New Series(serieName)
        Dim myChart As New Chart With {
           .Width = width,
           .Height = height,
           .Top = top,
           .Left = left,
           .BackColor = Color.FromArgb(31, 33, 32)
       }
        myChart.ChartAreas.Add(chartArea)
        myChart.ChartAreas("MainArea").BackColor = backgroundColor
        series.ChartType = chartType
        series.Color = Color.White
        'series.IsValueShownAsLabel = True
        'series.LabelForeColor = Color.Blue

        series.Font = New Font("Arial", 12, FontStyle.Bold)

        For Each item As KeyValuePair(Of String, Integer) In seriesItens
            series.Points.AddXY(item.Key, item.Value)
        Next

        myChart.Titles.Add(chartTitle)
        myChart.Series.Add(series)
        container.Controls.Add(myChart)

    End Sub



End Class
