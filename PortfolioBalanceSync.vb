Imports System.Data
Imports System.Globalization

Public NotInheritable Class PortfolioBalanceSync

    Private Sub New()
    End Sub

    Public Shared Async Function SyncAsync(
        binance As Binance,
        gate As Gateio,
        binanceAssets As Dictionary(Of String, Decimal),
        gateAssets As Dictionary(Of String, Decimal)) As Task(Of Integer)

        PortfolioRepository.Initialize()

        Dim portfolio As DataTable = PortfolioRepository.GetAll()
        Dim known As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)

        For Each row As DataRow In portfolio.Rows
            Dim wallet As String = row("Wallet").ToString().Trim()
            Dim symbol As String = row("Symbol").ToString().Trim().ToUpperInvariant()
            known.Add(wallet & "|" & symbol)
        Next

        Dim added As Integer = 0

        ' ============================================================
        ' BINANCE
        ' Usa o método já existente que retorna os ativos da conta
        ' com valor de pelo menos US$ 1 e o respectivo preço.
        ' ============================================================
        Try
            Dim binanceInfo = Await binance.BINANCE_GetCoinsInfo()

            If TypeOf binanceInfo Is List(Of String) Then

                For Each line As String In DirectCast(binanceInfo, List(Of String))

                    Dim parts() As String = line.Split("|"c)
                    If parts.Length < 3 Then Continue For

                    Dim symbol As String = parts(0).Trim().ToUpperInvariant()
                    Dim price As Decimal
                    Dim quantity As Decimal

                    If Not Decimal.TryParse(
                        parts(1),
                        NumberStyles.Float,
                        CultureInfo.InvariantCulture,
                        price) Then
                        Continue For
                    End If

                    If Not Decimal.TryParse(
                        parts(2),
                        NumberStyles.Float,
                        CultureInfo.InvariantCulture,
                        quantity) Then
                        Continue For
                    End If

                    If quantity <= 0D OrElse quantity * price < 1D Then
                        Continue For
                    End If

                    Dim key As String = "BINANCE|" & symbol

                    If known.Contains(key) Then
                        Continue For
                    End If

                    Dim resposta As DialogResult =
                        MessageBox.Show(
                            $"A moeda {symbol} foi encontrada na sua conta Binance, mas não está cadastrada no portfólio." &
                            Environment.NewLine & Environment.NewLine &
                            $"Quantidade: {quantity.ToString("N8", CultureInfo.GetCultureInfo("pt-BR"))}" &
                            Environment.NewLine &
                            $"Preço atual: ${price.ToString("N4", CultureInfo.InvariantCulture)}" &
                            Environment.NewLine & Environment.NewLine &
                            "Deseja adicionar ao portfólio?",
                            "Nova moeda encontrada - Binance",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question)

                    If resposta <> DialogResult.Yes Then
                        Continue For
                    End If

                    Dim initialPrice As Decimal = price

                    If symbol = "USDT" Then
                        initialPrice = 1D
                    End If

                    PortfolioRepository.AddOrUpdate(
                        symbol,
                        symbol,
                        initialPrice,
                        quantity,
                        Date.Today.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                        "BINANCE",
                        price)

                    known.Add(key)
                    added += 1

                Next

            End If

        Catch ex As Exception
            Debug.WriteLine(
                "[SYNC BINANCE] Erro ao comparar saldos: " & ex.ToString())
        End Try

        ' ============================================================
        ' GATE.IO
        ' Usa os saldos já obtidos e consulta preço apenas dos ativos
        ' que ainda não existem no SQLite.
        ' ============================================================
        Try

            For Each item In gateAssets

                Dim symbol As String =
                    item.Key.Trim().ToUpperInvariant()

                Dim quantity As Decimal = item.Value

                If quantity <= 0D Then
                    Continue For
                End If

                Dim key As String = "GATE.IO|" & symbol

                If known.Contains(key) Then
                    Continue For
                End If

                Dim price As Decimal = 0D

                Try
                    price = Await gate.GATE_GetCoinsPrice(symbol)
                Catch ex As Exception
                    Debug.WriteLine(
                        $"[SYNC GATE] Erro buscando preço de {symbol}: {ex.Message}")
                    Continue For
                End Try

                If price <= 0D OrElse quantity * price < 1D Then
                    Continue For
                End If

                Dim resposta As DialogResult =
                    MessageBox.Show(
                        $"A moeda {symbol} foi encontrada na sua conta Gate.io, mas não está cadastrada no portfólio." &
                        Environment.NewLine & Environment.NewLine &
                        $"Quantidade: {quantity.ToString("N8", CultureInfo.GetCultureInfo("pt-BR"))}" &
                        Environment.NewLine &
                        $"Preço atual: ${price.ToString("N4", CultureInfo.InvariantCulture)}" &
                        Environment.NewLine & Environment.NewLine &
                        "Deseja adicionar ao portfólio?",
                        "Nova moeda encontrada - Gate.io",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question)

                If resposta <> DialogResult.Yes Then
                    Continue For
                End If

                Dim initialPrice As Decimal = price

                If symbol = "USDT" Then
                    initialPrice = 1D
                End If

                PortfolioRepository.AddOrUpdate(
                    symbol,
                    symbol,
                    initialPrice,
                    quantity,
                    Date.Today.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                    "GATE.IO",
                    price)

                known.Add(key)
                added += 1

            Next

        Catch ex As Exception
            Debug.WriteLine(
                "[SYNC GATE] Erro ao comparar saldos: " & ex.ToString())
        End Try

        If added > 0 Then
            Debug.WriteLine(
                $"[PORTFOLIO SYNC] {added} ativo(s) novo(s) adicionado(s) ao SQLite.")
        End If

        Return added

    End Function

End Class
