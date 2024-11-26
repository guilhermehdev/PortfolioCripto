<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FormIntervalo
    Inherits System.Windows.Forms.Form

    'Descartar substituições de formulário para limpar a lista de componentes.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Exigido pelo Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'OBSERVAÇÃO: o procedimento a seguir é exigido pelo Windows Form Designer
    'Pode ser modificado usando o Windows Form Designer.  
    'Não o modifique usando o editor de códigos.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        cbIntervalo = New ComboBox()
        btOkIntervalo = New Button()
        btCancelTimer = New Button()
        SuspendLayout()
        ' 
        ' cbIntervalo
        ' 
        cbIntervalo.FormattingEnabled = True
        cbIntervalo.Items.AddRange(New Object() {"1 minuto", "5 minutos", "10 minutos", "15 minutos", "20 minutos", "25 minutos", "30 minutos", "45 minutos", "60 minutos"})
        cbIntervalo.Location = New Point(12, 33)
        cbIntervalo.Name = "cbIntervalo"
        cbIntervalo.Size = New Size(121, 23)
        cbIntervalo.TabIndex = 0
        ' 
        ' btOkIntervalo
        ' 
        btOkIntervalo.BackColor = SystemColors.ButtonFace
        btOkIntervalo.Cursor = Cursors.Hand
        btOkIntervalo.FlatAppearance.BorderSize = 0
        btOkIntervalo.FlatStyle = FlatStyle.Flat
        btOkIntervalo.Font = New Font("Segoe UI", 9F, FontStyle.Italic, GraphicsUnit.Point, CByte(0))
        btOkIntervalo.ForeColor = SystemColors.ActiveCaptionText
        btOkIntervalo.Location = New Point(139, 33)
        btOkIntervalo.Margin = New Padding(0)
        btOkIntervalo.Name = "btOkIntervalo"
        btOkIntervalo.Size = New Size(43, 23)
        btOkIntervalo.TabIndex = 1
        btOkIntervalo.Text = "OK"
        btOkIntervalo.UseVisualStyleBackColor = False
        ' 
        ' btCancelTimer
        ' 
        btCancelTimer.BackColor = Color.IndianRed
        btCancelTimer.Cursor = Cursors.Hand
        btCancelTimer.FlatAppearance.BorderSize = 0
        btCancelTimer.FlatStyle = FlatStyle.Flat
        btCancelTimer.ForeColor = SystemColors.ButtonHighlight
        btCancelTimer.Location = New Point(185, 33)
        btCancelTimer.Name = "btCancelTimer"
        btCancelTimer.Size = New Size(43, 23)
        btCancelTimer.TabIndex = 2
        btCancelTimer.Text = "X"
        btCancelTimer.UseVisualStyleBackColor = False
        ' 
        ' FormIntervalo
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        BackColor = Color.FromArgb(CByte(31), CByte(33), CByte(32))
        ClientSize = New Size(240, 89)
        Controls.Add(btCancelTimer)
        Controls.Add(btOkIntervalo)
        Controls.Add(cbIntervalo)
        FormBorderStyle = FormBorderStyle.FixedToolWindow
        Name = "FormIntervalo"
        StartPosition = FormStartPosition.CenterScreen
        Text = "Configurar intervalo"
        ResumeLayout(False)
    End Sub

    Friend WithEvents cbIntervalo As ComboBox
    Friend WithEvents btOkIntervalo As Button
    Friend WithEvents btCancelTimer As Button
End Class
