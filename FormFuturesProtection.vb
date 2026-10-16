Imports System.Globalization

Public Class FormFuturesProtection
    Inherits Form

    Private ReadOnly _priceTextBox As New TextBox()
    Private ReadOnly _quantityPercentNumeric As New NumericUpDown()
    Private ReadOnly _resultLabel As New Label()
    Private ReadOnly _sendButton As New Button()
    Private ReadOnly _cancelButton As New Button()

    Public Property TriggerPrice As Decimal
    Public Property QuantityPercent As Decimal = 100D

    Public Sub New(
        symbol As String,
        positionSide As String,
        protectionName As String,
        currentValue As String,
        entryPrice As Decimal,
        positionQuantity As Decimal)
        Text = $"Futuros - {protectionName}"
        StartPosition = FormStartPosition.CenterParent
        FormBorderStyle = FormBorderStyle.FixedDialog
        MaximizeBox = False
        MinimizeBox = False
        ShowInTaskbar = False
        ClientSize = New Size(365, 225)
        BackColor = Color.FromArgb(35, 35, 35)
        ForeColor = Color.White

        Dim titleLabel As New Label With {
            .AutoSize = False,
            .Location = New Point(16, 14),
            .Size = New Size(330, 24),
            .ForeColor = Color.White,
            .Text = $"{protectionName} - {symbol} ({positionSide})",
            .Font = New Font("Segoe UI", 10.0F, FontStyle.Bold)
        }
        Controls.Add(titleLabel)

        Dim descriptionLabel As New Label With {
            .AutoSize = False,
            .Location = New Point(16, 45),
            .Size = New Size(330, 38),
            .ForeColor = Color.Gainsboro,
            .Text = "Informe o preço de disparo usando o preço de marcação (Mark Price)."
        }
        Controls.Add(descriptionLabel)

        Dim priceLabel As New Label With {
            .AutoSize = True,
            .Location = New Point(16, 96),
            .ForeColor = Color.White,
            .Text = "Preço de disparo:"
        }
        Controls.Add(priceLabel)

        _priceTextBox.Location = New Point(135, 92)
        _priceTextBox.Size = New Size(195, 23)
        _priceTextBox.BackColor = Color.FromArgb(55, 55, 55)
        _priceTextBox.ForeColor = Color.Gold
        _priceTextBox.BorderStyle = BorderStyle.FixedSingle
        _priceTextBox.TextAlign = HorizontalAlignment.Right
        _priceTextBox.Text = If(String.Equals(currentValue, "-", StringComparison.OrdinalIgnoreCase), String.Empty, currentValue?.Replace("$", String.Empty).Trim())
        AddHandler _priceTextBox.TextChanged,
            Sub() UpdateEstimatedResult(positionSide, entryPrice, positionQuantity)
        Controls.Add(_priceTextBox)

        Dim quantityPercentLabel As New Label With {
            .AutoSize = True,
            .Location = New Point(16, 125),
            .ForeColor = Color.White,
            .Text = "Quantidade (%):"
        }
        Controls.Add(quantityPercentLabel)

        _quantityPercentNumeric.Location = New Point(135, 121)
        _quantityPercentNumeric.Size = New Size(195, 23)
        _quantityPercentNumeric.Minimum = 0D
        _quantityPercentNumeric.Maximum = 100D
        _quantityPercentNumeric.DecimalPlaces = 2
        _quantityPercentNumeric.Increment = 1D
        _quantityPercentNumeric.Value = 100D
        _quantityPercentNumeric.ThousandsSeparator = False
        _quantityPercentNumeric.BackColor = Color.FromArgb(55, 55, 55)
        _quantityPercentNumeric.ForeColor = Color.Gold
        _quantityPercentNumeric.BorderStyle = BorderStyle.FixedSingle
        _quantityPercentNumeric.TextAlign = HorizontalAlignment.Right
        AddHandler _quantityPercentNumeric.ValueChanged,
            Sub() UpdateEstimatedResult(positionSide, entryPrice, positionQuantity)
        AddHandler _quantityPercentNumeric.TextChanged,
            Sub() UpdateEstimatedResult(positionSide, entryPrice, positionQuantity)
        Controls.Add(_quantityPercentNumeric)

        _resultLabel.AutoSize = False
        _resultLabel.Location = New Point(16, 151)
        _resultLabel.Size = New Size(314, 22)
        _resultLabel.ForeColor = Color.White
        _resultLabel.Text = "Resultado estimado: --"
        Controls.Add(_resultLabel)

        _sendButton.Text = "Enviar ordem"
        _sendButton.Location = New Point(135, 180)
        _sendButton.Size = New Size(125, 28)
        _sendButton.BackColor = Color.DarkGreen
        _sendButton.ForeColor = Color.White
        _sendButton.FlatStyle = FlatStyle.Flat
        _sendButton.FlatAppearance.BorderSize = 0
        AddHandler _sendButton.Click, AddressOf SendButton_Click
        Controls.Add(_sendButton)

        _cancelButton.Text = "Cancelar"
        _cancelButton.Location = New Point(270, 180)
        _cancelButton.Size = New Size(60, 28)
        _cancelButton.BackColor = Color.Maroon
        _cancelButton.ForeColor = Color.White
        _cancelButton.FlatStyle = FlatStyle.Flat
        _cancelButton.FlatAppearance.BorderSize = 0
        AddHandler _cancelButton.Click, Sub() DialogResult = DialogResult.Cancel
        Controls.Add(_cancelButton)

        AcceptButton = _sendButton
        CancelButton = _cancelButton
        UpdateEstimatedResult(positionSide, entryPrice, positionQuantity)
    End Sub

    Private Sub UpdateEstimatedResult(positionSide As String, entryPrice As Decimal, positionQuantity As Decimal)
        Dim triggerPrice As Decimal
        Dim quantityPercent As Decimal
        Dim normalized = _priceTextBox.Text.Replace("$", String.Empty).Trim()
        If Not Decimal.TryParse(normalized, NumberStyles.Float, CultureInfo.InvariantCulture, triggerPrice) AndAlso
           Not Decimal.TryParse(normalized, NumberStyles.Number, CultureInfo.GetCultureInfo("pt-BR"), triggerPrice) Then
            _resultLabel.Text = "Resultado estimado: --"
            _resultLabel.ForeColor = Color.White
            Return
        End If

        If Not TryParseDecimal(_quantityPercentNumeric.Text, quantityPercent) OrElse
           quantityPercent <= 0D OrElse quantityPercent > 100D Then
            _resultLabel.Text = "Resultado estimado: informe um percentual entre 0 e 100."
            _resultLabel.ForeColor = Color.OrangeRed
            Return
        End If

        If entryPrice <= 0D OrElse positionQuantity = 0D OrElse triggerPrice <= 0D Then
            _resultLabel.Text = "Resultado estimado: --"
            _resultLabel.ForeColor = Color.White
            Return
        End If

        Dim quantity = Math.Abs(positionQuantity) * quantityPercent / 100D
        Dim resultUsd = If(String.Equals(positionSide, "SHORT", StringComparison.OrdinalIgnoreCase),
                           (entryPrice - triggerPrice) * quantity,
                           (triggerPrice - entryPrice) * quantity)
        _resultLabel.Text = "Resultado estimado: " & If(resultUsd >= 0D, "+", "") &
                            "$" & resultUsd.ToString("N2", CultureInfo.GetCultureInfo("en-US"))
        _resultLabel.ForeColor = If(resultUsd >= 0D, Color.LimeGreen, Color.OrangeRed)
    End Sub

    Private Sub SendButton_Click(sender As Object, e As EventArgs)
        Dim value As Decimal
        Dim quantityPercent As Decimal
        Dim normalized = _priceTextBox.Text.Replace("$", String.Empty).Trim()
        If Not Decimal.TryParse(normalized, NumberStyles.Float, CultureInfo.InvariantCulture, value) AndAlso
           Not Decimal.TryParse(normalized, NumberStyles.Number, CultureInfo.GetCultureInfo("pt-BR"), value) Then
            MessageBox.Show("Informe um preço de disparo válido.", "Proteção Futures", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            _priceTextBox.Focus()
            Return
        End If

        If Not TryParseDecimal(_quantityPercentNumeric.Text, quantityPercent) OrElse
           quantityPercent <= 0D OrElse quantityPercent > 100D Then
            MessageBox.Show("Informe um percentual entre 0 e 100.", "Proteção Futures", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            _quantityPercentNumeric.Focus()
            Return
        End If

        If value <= 0D Then
            MessageBox.Show("O preço de disparo deve ser maior que zero.", "Proteção Futures", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            _priceTextBox.Focus()
            Return
        End If

        TriggerPrice = value
        QuantityPercent = quantityPercent
        DialogResult = DialogResult.OK
    End Sub

    Private Shared Function TryParseDecimal(text As String, ByRef value As Decimal) As Boolean
        Dim normalized = text?.Replace("$", String.Empty).Replace("%", String.Empty).Trim()
        Return Decimal.TryParse(normalized, NumberStyles.Float, CultureInfo.InvariantCulture, value) OrElse
               Decimal.TryParse(normalized, NumberStyles.Number, CultureInfo.GetCultureInfo("pt-BR"), value)
    End Function
End Class
