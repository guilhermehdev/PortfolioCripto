<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FormPools
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
        btnCalcular = New Button()
        txtPrecoMin = New TextBox()
        txtPrecoMax = New TextBox()
        txtTotalUSD = New TextBox()
        txtResultado = New TextBox()
        trackActivePrice = New TrackBar()
        txtToken0 = New TextBox()
        txtToken1 = New TextBox()
        txtPrecoFuturo = New TextBox()
        Label1 = New Label()
        Label2 = New Label()
        Label3 = New Label()
        Label5 = New Label()
        lblPrecoAtivo = New Label()
        Label4 = New Label()
        txtPrecoEntrada = New TextBox()
        CType(trackActivePrice, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        ' 
        ' btnCalcular
        ' 
        btnCalcular.Location = New Point(208, 164)
        btnCalcular.Name = "btnCalcular"
        btnCalcular.Size = New Size(75, 23)
        btnCalcular.TabIndex = 6
        btnCalcular.Text = "Calcular"
        btnCalcular.UseVisualStyleBackColor = True
        ' 
        ' txtPrecoMin
        ' 
        txtPrecoMin.Location = New Point(77, 28)
        txtPrecoMin.Name = "txtPrecoMin"
        txtPrecoMin.Size = New Size(100, 23)
        txtPrecoMin.TabIndex = 7
        txtPrecoMin.Text = "0"
        ' 
        ' txtPrecoMax
        ' 
        txtPrecoMax.Location = New Point(183, 28)
        txtPrecoMax.Name = "txtPrecoMax"
        txtPrecoMax.Size = New Size(100, 23)
        txtPrecoMax.TabIndex = 8
        txtPrecoMax.Text = "0"
        ' 
        ' txtTotalUSD
        ' 
        txtTotalUSD.Location = New Point(12, 28)
        txtTotalUSD.Name = "txtTotalUSD"
        txtTotalUSD.Size = New Size(59, 23)
        txtTotalUSD.TabIndex = 9
        txtTotalUSD.Text = "100"
        ' 
        ' txtResultado
        ' 
        txtResultado.Location = New Point(289, 10)
        txtResultado.Multiline = True
        txtResultado.Name = "txtResultado"
        txtResultado.Size = New Size(365, 333)
        txtResultado.TabIndex = 10
        ' 
        ' trackActivePrice
        ' 
        trackActivePrice.Location = New Point(12, 113)
        trackActivePrice.Maximum = 1000
        trackActivePrice.Name = "trackActivePrice"
        trackActivePrice.Size = New Size(271, 45)
        trackActivePrice.TabIndex = 15
        trackActivePrice.TickFrequency = 100
        trackActivePrice.Value = 500
        ' 
        ' txtToken0
        ' 
        txtToken0.Location = New Point(12, 55)
        txtToken0.Name = "txtToken0"
        txtToken0.Size = New Size(59, 23)
        txtToken0.TabIndex = 16
        txtToken0.Text = "SOL"
        ' 
        ' txtToken1
        ' 
        txtToken1.Location = New Point(12, 84)
        txtToken1.Name = "txtToken1"
        txtToken1.Size = New Size(59, 23)
        txtToken1.TabIndex = 17
        txtToken1.Text = "USDC"
        ' 
        ' txtPrecoFuturo
        ' 
        txtPrecoFuturo.AcceptsReturn = True
        txtPrecoFuturo.Location = New Point(183, 73)
        txtPrecoFuturo.Name = "txtPrecoFuturo"
        txtPrecoFuturo.Size = New Size(100, 23)
        txtPrecoFuturo.TabIndex = 19
        txtPrecoFuturo.Text = "0"
        ' 
        ' Label1
        ' 
        Label1.AutoSize = True
        Label1.Location = New Point(12, 10)
        Label1.Name = "Label1"
        Label1.Size = New Size(41, 15)
        Label1.TabIndex = 20
        Label1.Text = "Total $"
        ' 
        ' Label2
        ' 
        Label2.AutoSize = True
        Label2.Location = New Point(77, 10)
        Label2.Name = "Label2"
        Label2.Size = New Size(82, 15)
        Label2.TabIndex = 21
        Label2.Text = "Preço minimo"
        ' 
        ' Label3
        ' 
        Label3.AutoSize = True
        Label3.Location = New Point(183, 10)
        Label3.Name = "Label3"
        Label3.Size = New Size(84, 15)
        Label3.TabIndex = 22
        Label3.Text = "Preço maximo"
        ' 
        ' Label5
        ' 
        Label5.AutoSize = True
        Label5.Location = New Point(183, 55)
        Label5.Name = "Label5"
        Label5.Size = New Size(73, 15)
        Label5.TabIndex = 24
        Label5.Text = "Preço futuro"
        ' 
        ' lblPrecoAtivo
        ' 
        lblPrecoAtivo.AutoSize = True
        lblPrecoAtivo.Location = New Point(12, 161)
        lblPrecoAtivo.Name = "lblPrecoAtivo"
        lblPrecoAtivo.Size = New Size(41, 15)
        lblPrecoAtivo.TabIndex = 11
        lblPrecoAtivo.Text = "Label1"
        ' 
        ' Label4
        ' 
        Label4.AutoSize = True
        Label4.Location = New Point(77, 55)
        Label4.Name = "Label4"
        Label4.Size = New Size(80, 15)
        Label4.TabIndex = 26
        Label4.Text = "Preço entrada"
        ' 
        ' txtPrecoEntrada
        ' 
        txtPrecoEntrada.AcceptsReturn = True
        txtPrecoEntrada.Location = New Point(77, 73)
        txtPrecoEntrada.Name = "txtPrecoEntrada"
        txtPrecoEntrada.Size = New Size(100, 23)
        txtPrecoEntrada.TabIndex = 25
        txtPrecoEntrada.Text = "0"
        ' 
        ' FormPools
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(666, 355)
        Controls.Add(Label4)
        Controls.Add(txtPrecoEntrada)
        Controls.Add(Label5)
        Controls.Add(Label3)
        Controls.Add(Label2)
        Controls.Add(Label1)
        Controls.Add(txtPrecoFuturo)
        Controls.Add(txtToken1)
        Controls.Add(txtToken0)
        Controls.Add(trackActivePrice)
        Controls.Add(lblPrecoAtivo)
        Controls.Add(txtResultado)
        Controls.Add(txtTotalUSD)
        Controls.Add(txtPrecoMax)
        Controls.Add(txtPrecoMin)
        Controls.Add(btnCalcular)
        Name = "FormPools"
        StartPosition = FormStartPosition.CenterScreen
        Text = "FormPools"
        CType(trackActivePrice, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)
        PerformLayout()
    End Sub
    Friend WithEvents btnCalcular As Button
    Friend WithEvents txtPrecoMin As TextBox
    Friend WithEvents txtPrecoMax As TextBox
    Friend WithEvents txtTotalUSD As TextBox
    Friend WithEvents txtResultado As TextBox
    Friend WithEvents trackActivePrice As TrackBar
    Friend WithEvents txtToken0 As TextBox
    Friend WithEvents txtToken1 As TextBox
    Friend WithEvents txtPrecoFuturo As TextBox
    Friend WithEvents Label1 As Label
    Friend WithEvents Label2 As Label
    Friend WithEvents Label3 As Label
    Friend WithEvents Label5 As Label
    Friend WithEvents lblPrecoAtivo As Label
    Friend WithEvents Label4 As Label
    Friend WithEvents txtPrecoEntrada As TextBox
End Class
