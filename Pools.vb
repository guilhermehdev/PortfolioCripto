Imports System.Globalization

Public Structure PoolPosition
    Public TokenA As Double
    Public TokenB As Double
End Structure

Public Class Pools

    Public Function Sqrt(x As Double) As Double
        Return Math.Sqrt(x)
    End Function

    Public Sub DistribuirUSD(totalUSD As Double, precoAtual As Double, precoMin As Double, precoMax As Double, ByRef token0USD As Double, ByRef token1USD As Double)
        Dim sqrtP = Sqrt(precoAtual)
        Dim sqrtMin = Sqrt(precoMin)
        Dim sqrtMax = Sqrt(precoMax)

        Dim t = (sqrtP - sqrtMin) / (sqrtMax - sqrtMin)
        If t < 0 Then t = 0
        If t > 1 Then t = 1

        token0USD = totalUSD * (1 - t)
        token1USD = totalUSD * t
    End Sub

    Public Function SimularHODL(token0USD As Double, token1USD As Double, precoEntrada As Double, precoFuturo As Double) As Double
        Dim qtdToken0 = token0USD / precoEntrada
        Return (qtdToken0 * precoFuturo) + token1USD
    End Function

    Public Function SimularUniswapV3(token0USD As Double, token1USD As Double, precoAtivo As Double, precoFuturo As Double, precoMin As Double, precoMax As Double) As Double
        ' Convertendo valores iniciais para quantidades
        Dim qtdToken0 = token0USD / precoAtivo
        Dim qtdToken1 = token1USD / 1

        ' Fora da faixa para baixo (só SOL)
        If precoFuturo < precoMin OrElse Math.Abs(precoFuturo - precoMin) < 0.00001 Then
            Return qtdToken0 * precoFuturo
            ' Fora da faixa para cima (só USDC)
        ElseIf precoFuturo >= precoMax Then
            Return qtdToken1 * 1

            ' Dentro da faixa → calcular proporção líquida
        Else
            ' Fórmula baseada em Uniswap V3: proporção entre token0/token1
            ' Ajuste de quantidade LP com base no preço futuro dentro do range
            Dim raizMin = Math.Sqrt(precoMin)
            Dim raizMax = Math.Sqrt(precoMax)
            Dim raizAtual = Math.Sqrt(precoFuturo)

            Dim L0 = qtdToken0 * (raizMax * raizAtual) / (raizMax - raizAtual)
            Dim L1 = qtdToken1 / (raizAtual - raizMin)

            ' Liquidez efetiva = menor dos dois lados (token0 e token1)
            Dim L = Math.Min(L0, L1)

            ' Recalcular quantidade final dos ativos
            Dim finalToken0 = L * (raizMax - raizAtual) / (raizAtual * raizMax)
            Dim finalToken1 = L * (raizAtual - raizMin)

            ' Retornar valor total em USD
            Return (finalToken0 * precoFuturo) + finalToken1
        End If
    End Function

    Public Function CalcularIL(valorHold As Double, valorLP As Double) As Double
        Return ((valorLP - valorHold) / valorHold) * 100
    End Function

End Class