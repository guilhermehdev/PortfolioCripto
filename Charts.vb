Imports System.Windows.Forms.DataVisualization.Charting

Public Class Charts

    Public Sub Graph(ByVal width As Integer, ByVal height As Integer, ByVal top As Integer, ByVal left As Integer, serieName As String, title As String, chartType As SeriesChartType, seriesItens As Dictionary(Of String, Integer), container As Control)
        Dim myChart As New Chart With {
           .Width = width,
           .Height = height,
           .Top = top,
           .Left = left
       }

        Dim chartArea As New ChartArea("MainArea")
        myChart.ChartAreas.Add(chartArea)

        ' Criar uma série para os dados
        Dim series As New Series(serieName)
        series.ChartType = chartType ' Definir o tipo de gráfico (barra neste caso)

        For Each item As KeyValuePair(Of String, Integer) In seriesItens
            series.Points.AddXY(item.Key, item.Value)
        Next

        myChart.Series.Add(series)

        ' Adicionar título ao gráfico (opcional)
        Dim chartTitle As New Title(title, Docking.Top, New Font("Arial", 10), Color.Black)
        myChart.Titles.Add(chartTitle)

        container.Controls.Add(myChart)

    End Sub



End Class
