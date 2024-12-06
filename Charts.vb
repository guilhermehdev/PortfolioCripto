Imports System.Globalization
Imports System.Windows.Forms.DataVisualization.Charting

Public Class Charts

    Public Sub collumGraph(ByVal width As Integer, ByVal height As Integer, ByVal top As Integer, ByVal left As Integer, serieName As String, title As String, titlefontSize As Integer, titleColor As Color, backgroundColor As Color, chartType As SeriesChartType, seriesItens As Dictionary(Of String, Decimal), container As Control)
        Dim chartArea As New ChartArea("MainArea")
        Dim chartTitle As New Title(title, Docking.Top, New Font("Arial", titlefontSize), titleColor)
        Dim series As New Series(serieName)
        Dim legend As New Legend("MainLegend")
        Dim myChart As New Chart With {
           .Width = width,
           .Height = height,
           .Top = top,
           .Left = left,
           .BackColor = Color.FromArgb(31, 33, 32),
           .Name = "CriptoChart"
       }

        myChart.Font = New Font("Calibri", 10, FontStyle.Bold)

        legend.Title = "Legenda do Gráfico"
        legend.BackColor = Color.LightGray ' Fundo da legenda
        legend.Font = New Font("Arial", 10, FontStyle.Bold) ' Fonte da legenda
        legend.ForeColor = Color.Black ' Cor do texto da legenda
        'myChart.Legends.Add(legend)

        myChart.ChartAreas.Add(chartArea)
        With myChart.ChartAreas("MainArea")
            .BackColor = backgroundColor
            .AxisX.LabelStyle.ForeColor = Color.White
            .AxisX.LabelStyle.Font = New Font("Calibri", 7, FontStyle.Italic)
        End With

        With chartArea.AxisY
            .Title = "(Dolar $)"
            .TitleFont = New Font("Arial", 8, FontStyle.Regular)
            .TitleForeColor = Color.Aquamarine
            .LabelStyle.ForeColor = Color.IndianRed
            .LabelStyle.Font = New Font("Calibri", 7, FontStyle.Regular)
        End With

        With chartArea.AxisX.MajorGrid
            .LineColor = Color.FromArgb(64, 64, 64) ' Cor da linha da grade principal no eixo X
            .LineDashStyle = ChartDashStyle.Dash ' Estilo da linha (tracejada)
        End With

        ' Configurar a grade no eixo Y
        With chartArea.AxisY.MajorGrid
            .LineColor = Color.FromArgb(64, 64, 64) ' Cor da linha da grade principal no eixo Y
            .LineDashStyle = ChartDashStyle.Dot ' Estilo da linha (sólida)
        End With

        'chartArea.AxisX.MajorGrid.Enabled = False
        'chartArea.AxisY.MajorGrid.Enabled = False

        series.ChartType = chartType
        series.Color = Color.Purple
        series.IsValueShownAsLabel = True
        series.LabelForeColor = Color.Gold
        series.Font = New Font("Arial", 8, FontStyle.Italic)

        For Each item As KeyValuePair(Of String, Decimal) In seriesItens
            series.Points.AddXY(item.Key, item.Value)
        Next

        For Each point In series.Points
            point.Label = "$" & point.YValues(0).ToString("N2")
        Next

        myChart.Series.Add(series)
        myChart.Titles.Add(chartTitle)
        myChart.ChartAreas("MainArea").RecalculateAxesScale()
        container.Controls.Add(myChart)

    End Sub

    Public Sub pieGraph(ByVal width As Integer, ByVal height As Integer, ByVal top As Integer, ByVal left As Integer, title As String, titlefontSize As Integer, titleColor As Color, backgroundColor As Color, seriesItens As Dictionary(Of String, Decimal), seriesFontSize As Integer, seriesFontColor As Color, container As Control)
        Dim chartArea As New ChartArea("MainArea")
        Dim chartTitle As New Title(title, Docking.Top, New Font("Arial", titlefontSize), titleColor)
        Dim series As New Series("MainSerie")
        Dim legend As New Legend("MainLegend")
        Dim myChart As New Chart With {
           .Width = width,
           .Height = height,
           .Top = top,
           .Left = left,
           .BackColor = Color.FromArgb(31, 33, 32),
           .Name = "CriptoChart"
       }

        myChart.ChartAreas.Add(chartArea)
        With myChart.ChartAreas("MainArea")
            .BackColor = backgroundColor
            .Area3DStyle.Enable3D = True
        End With

        series.ChartType = SeriesChartType.Pie
        series.LabelForeColor = seriesFontColor
        series.Font = New Font("Calibri", seriesFontSize, FontStyle.Bold)

        For Each item As KeyValuePair(Of String, Decimal) In seriesItens
            series.Points.AddXY(item.Key, item.Value)
        Next

        series.Label = "#AXISLABEL \n #PERCENT" ' Exibir percentual no rótulo
        series.CustomProperties = "PieLabelStyle=inside, PieLineColor=red"

        myChart.Series.Add(series)
        myChart.Titles.Add(chartTitle)
        container.Controls.Add(myChart)

    End Sub

End Class
