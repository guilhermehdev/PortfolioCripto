Imports System.Collections.Generic
Imports System.Drawing
Imports System.Linq
Imports System.Windows.Forms

Friend NotInheritable Class SpotTradeBotUniverseForm
    Inherits Form

    Private ReadOnly _symbolsList As CheckedListBox

    Public Sub New(candidates As IEnumerable(Of String), selectedSymbols As IEnumerable(Of String))
        Text = "Universo manual do Tradebot"
        StartPosition = FormStartPosition.CenterParent
        FormBorderStyle = FormBorderStyle.FixedDialog
        MaximizeBox = False
        MinimizeBox = False
        ShowInTaskbar = False
        ClientSize = New Size(330, 470)
        BackColor = Color.FromArgb(35, 35, 35)
        ForeColor = Color.White

        Controls.Add(New Label With {
            .AutoSize = False,
            .Location = New Point(14, 12),
            .Size = New Size(300, 34),
            .ForeColor = Color.Gainsboro,
            .Text = "Marque as moedas que o bot poderá acompanhar."
        })

        _symbolsList = New CheckedListBox With {
            .Location = New Point(14, 52),
            .Size = New Size(302, 350),
            .BackColor = Color.FromArgb(55, 55, 55),
            .ForeColor = Color.White,
            .BorderStyle = BorderStyle.FixedSingle,
            .CheckOnClick = True,
            .IntegralHeight = False,
            .Font = New Font("Calibri", 11.0F)
        }
        Controls.Add(_symbolsList)

        Dim selected = New HashSet(Of String)(
            If(selectedSymbols, Enumerable.Empty(Of String)()).
            Where(Function(item) Not String.IsNullOrWhiteSpace(item)).
            Select(Function(item) item.Trim().ToUpperInvariant()),
            StringComparer.OrdinalIgnoreCase)

        Dim normalizedCandidates = New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
        For Each candidate In If(candidates, Enumerable.Empty(Of String)()).OrderBy(Function(item) item, StringComparer.OrdinalIgnoreCase)
            Dim symbol = If(candidate, String.Empty).Trim().ToUpperInvariant()
            If String.IsNullOrWhiteSpace(symbol) OrElse Not normalizedCandidates.Add(symbol) Then Continue For
            Dim index = _symbolsList.Items.Add(symbol)
            _symbolsList.SetItemChecked(index, selected.Contains(symbol))
        Next

        For Each symbol In selected
            If normalizedCandidates.Contains(symbol) Then Continue For
            Dim index = _symbolsList.Items.Add(symbol)
            _symbolsList.SetItemChecked(index, True)
        Next

        Dim saveButton As New Button With {
            .Text = "Salvar",
            .Location = New Point(146, 419),
            .Size = New Size(80, 28),
            .BackColor = Color.DarkGreen,
            .ForeColor = Color.White,
            .FlatStyle = FlatStyle.Flat,
            .DialogResult = DialogResult.OK
        }
        saveButton.FlatAppearance.BorderSize = 0
        AddHandler saveButton.Click, AddressOf SaveButton_Click
        Controls.Add(saveButton)

        Dim cancelButton As New Button With {
            .Text = "Cancelar",
            .Location = New Point(236, 419),
            .Size = New Size(80, 28),
            .BackColor = Color.FromArgb(75, 75, 75),
            .ForeColor = Color.White,
            .FlatStyle = FlatStyle.Flat,
            .DialogResult = DialogResult.Cancel
        }
        cancelButton.FlatAppearance.BorderSize = 0
        Controls.Add(cancelButton)

        AcceptButton = saveButton
        CancelButton = cancelButton
    End Sub

    Public Property SelectedSymbols As List(Of String) = New List(Of String)()

    Private Sub SaveButton_Click(sender As Object, e As EventArgs)
        SelectedSymbols = _symbolsList.CheckedItems.Cast(Of Object)().
            Select(Function(item) item.ToString().Trim().ToUpperInvariant()).
            Where(Function(item) Not String.IsNullOrWhiteSpace(item)).
            Distinct(StringComparer.OrdinalIgnoreCase).
            OrderBy(Function(item) item, StringComparer.OrdinalIgnoreCase).
            ToList()

        If SelectedSymbols.Count = 0 Then
            MessageBox.Show(
                "Selecione pelo menos uma moeda para o universo manual.",
                "Universo do Tradebot",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning)
            DialogResult = DialogResult.None
        End If
    End Sub
End Class
