Imports System.Globalization

Public Class FormPools
    Dim UniswapPoolFishCalc As New Pools
    Private Sub btnCalcular_Click(sender As Object, e As EventArgs) Handles btnCalcular.Click
        Try
            Dim token0Nome = txtToken0.Text.Trim()
            Dim token1Nome = txtToken1.Text.Trim()

            Dim totalUSD = Double.Parse(txtTotalUSD.Text, CultureInfo.InvariantCulture)
            Dim precoFuturo = Double.Parse(txtPrecoFuturo.Text, CultureInfo.InvariantCulture)
            Dim precoMin = Double.Parse(txtPrecoMin.Text, CultureInfo.InvariantCulture)
            Dim precoMax = Double.Parse(txtPrecoMax.Text, CultureInfo.InvariantCulture)

            ' Preço ativo baseado no slider (0.0 a 1.0)
            Dim tSlider = trackActivePrice.Value / 1000.0
            Dim precoAtivo = precoMin + (precoMax - precoMin) * tSlider
            lblPrecoAtivo.Text = $"Preço Ativo: ${precoAtivo:F2}"

            ' Distribuição de tokens baseada na faixa
            Dim token0USD As Double = 0
            Dim token1USD As Double = 0
            UniswapPoolFishCalc.DistribuirUSD(totalUSD, precoAtivo, precoMin, precoMax, token0USD, token1USD)

            ' Quantidades reais dos tokens
            Dim token0Qtd = token0USD / precoAtivo
            Dim token1Qtd = token1USD / 1
            Dim pct0 = token0USD / totalUSD * 100
            Dim pct1 = token1USD / totalUSD * 100

            ' Estratégia HODL
            Dim valorHODL = (token0Qtd * precoFuturo) + token1USD

            ' Estratégia LP (preço futuro)
            Dim valorLP = UniswapPoolFishCalc.SimularUniswapV3(token0USD, token1USD, precoAtivo, precoFuturo, precoMin, precoMax)
            Dim il = ((valorLP - valorHODL) / valorHODL) * 100

            ' IL máxima no range
            Dim valorLPmin = UniswapPoolFishCalc.SimularUniswapV3(token0USD, token1USD, precoAtivo, precoMin, precoMin, precoMax)
            Dim ilMin = ((valorLPmin - valorHODL) / valorHODL) * 100

            Dim valorLPmax = UniswapPoolFishCalc.SimularUniswapV3(token0USD, token1USD, precoAtivo, precoMax, precoMin, precoMax)
            Dim ilMax = ((valorLPmax - valorHODL) / valorHODL) * 100

            ' Valor fora da faixa, caso esteja
            Dim valorForaFaixa As Double = 0
            Dim foraFaixaTexto As String = ""
            If precoFuturo < precoMin Then
                valorForaFaixa = token0Qtd * precoFuturo
                foraFaixaTexto = $"(Fora da faixa: {token0Qtd:F4} {token0Nome} → ${valorForaFaixa:F2})"
            ElseIf precoFuturo > precoMax Then
                valorForaFaixa = token1Qtd * 1
                foraFaixaTexto = $"(Fora da faixa: {token1Qtd:F4} {token1Nome} → ${valorForaFaixa:F2})"
            End If

            ' Exibir resultado final formatado
            txtResultado.Text =
            $"--- Preço de Entrada (slider): ${precoAtivo:F2} ---{Environment.NewLine}" &
            $"{token0Nome}: ${token0USD:F2} (~{token0Qtd:F4} {token0Nome}) [{pct0:F2}%]{Environment.NewLine}" &
            $"{token1Nome}: ${token1USD:F2} (~{token1Qtd:F4} {token1Nome}) [{pct1:F2}%]{Environment.NewLine}{Environment.NewLine}" &
            $"--- Estratégia HODL ---{Environment.NewLine}" &
            $"Valor Final: ${valorHODL:F2}{Environment.NewLine}{Environment.NewLine}" &
            $"--- Estratégia LP Uniswap V3 ---{Environment.NewLine}" &
            $"Valor Final: ${valorLP:F2} {foraFaixaTexto}{Environment.NewLine}" &
            $"Impermanent Loss: {il:F2}% (Δ = {(valorLP - valorHODL):+$0.00;-$0.00}){Environment.NewLine}{Environment.NewLine}" &
            $"--- IL Máxima na Faixa ---{Environment.NewLine}" &
            $"Se preço cair até ${precoMin:F2}: Valor LP = ${valorLPmin:F2}, IL = {ilMin:F2}% (Δ = {(valorLPmin - valorHODL):+$0.00;-$0.00}){Environment.NewLine}" &
            $"Se preço subir até ${precoMax:F2}: Valor LP = ${valorLPmax:F2}, IL = {ilMax:F2}% (Δ = {(valorLPmax - valorHODL):+$0.00;-$0.00})" & Environment.NewLine &
            $"Valor Final: ${valorLP:F2} {foraFaixaTexto}"
        Catch ex As Exception
            txtResultado.Text = $"Erro: {ex.Message}"
        End Try
    End Sub


    Private Sub TrackBar1_Scroll(sender As Object, e As EventArgs) Handles trackActivePrice.Scroll
        Try
            Dim precoMin = Double.Parse(txtPrecoMin.Text, CultureInfo.InvariantCulture)
            Dim precoMax = Double.Parse(txtPrecoMax.Text, CultureInfo.InvariantCulture)
            Dim totalUSD = Double.Parse(txtTotalUSD.Text, CultureInfo.InvariantCulture)
            Dim token0Nome = txtToken0.Text.Trim()
            Dim token1Nome = txtToken1.Text.Trim()

            ' Mapear valor do slider (0 a 1000) para faixa de preço ativa
            Dim tSlider = trackActivePrice.Value / 1000.0
            Dim precoAtivo = precoMin + (precoMax - precoMin) * tSlider

            lblPrecoAtivo.Text = $"Preço Ativo: ${precoAtivo:F2}"

            ' Distribuir USD entre os tokens
            Dim token0USD As Double = 0
            Dim token1USD As Double = 0
            UniswapPoolFishCalc.DistribuirUSD(totalUSD, precoAtivo, precoMin, precoMax, token0USD, token1USD)

            ' Calcular quantidades
            Dim token0Qtd = token0USD / precoAtivo
            Dim token1Qtd = token1USD / 1 ' USDC = 1 USD

            ' Porcentagens
            Dim pct0 = token0USD / totalUSD * 100
            Dim pct1 = token1USD / totalUSD * 100

            txtResultado.Text =
            $"--- Distribuição com Preço Ativo = ${precoAtivo:F2} ---{Environment.NewLine}" &
            $"{token0Nome}: ${token0USD:F2} (~{token0Qtd:F4} {token0Nome}) [{pct0:F2}%]{Environment.NewLine}" &
            $"{token1Nome}: ${token1USD:F2} (~{token1Qtd:F4} {token1Nome}) [{pct1:F2}%]"
        Catch ex As Exception
            txtResultado.Text = $"Erro: {ex.Message}"
        End Try
    End Sub

End Class