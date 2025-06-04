<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FormCaixa
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
        Dim DataGridViewCellStyle1 As DataGridViewCellStyle = New DataGridViewCellStyle()
        Dim DataGridViewCellStyle2 As DataGridViewCellStyle = New DataGridViewCellStyle()
        Dim DataGridViewCellStyle3 As DataGridViewCellStyle = New DataGridViewCellStyle()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FormCaixa))
        btCancelTimer = New Button()
        Label2 = New Label()
        dgCaixa = New DataGridView()
        CType(dgCaixa, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        ' 
        ' btCancelTimer
        ' 
        btCancelTimer.BackColor = Color.IndianRed
        btCancelTimer.Cursor = Cursors.Hand
        btCancelTimer.FlatAppearance.BorderSize = 0
        btCancelTimer.FlatStyle = FlatStyle.Flat
        btCancelTimer.ForeColor = SystemColors.ButtonHighlight
        btCancelTimer.Location = New Point(255, 6)
        btCancelTimer.Name = "btCancelTimer"
        btCancelTimer.Size = New Size(29, 23)
        btCancelTimer.TabIndex = 3
        btCancelTimer.Text = "X"
        btCancelTimer.UseVisualStyleBackColor = False
        ' 
        ' Label2
        ' 
        Label2.AutoSize = True
        Label2.Font = New Font("Calibri", 14F, FontStyle.Bold Or FontStyle.Italic)
        Label2.ForeColor = Color.Aqua
        Label2.Location = New Point(12, 9)
        Label2.Name = "Label2"
        Label2.Size = New Size(54, 23)
        Label2.TabIndex = 4
        Label2.Text = "Caixa"
        ' 
        ' dgCaixa
        ' 
        dgCaixa.AllowUserToAddRows = False
        dgCaixa.AllowUserToDeleteRows = False
        dgCaixa.AllowUserToOrderColumns = True
        dgCaixa.AllowUserToResizeRows = False
        dgCaixa.Anchor = AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
        dgCaixa.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        dgCaixa.BackgroundColor = Color.FromArgb(CByte(64), CByte(64), CByte(64))
        dgCaixa.CellBorderStyle = DataGridViewCellBorderStyle.None
        dgCaixa.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None
        DataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle1.BackColor = Color.FromArgb(CByte(64), CByte(64), CByte(64))
        DataGridViewCellStyle1.Font = New Font("Segoe UI", 9F)
        DataGridViewCellStyle1.ForeColor = Color.Turquoise
        DataGridViewCellStyle1.SelectionBackColor = Color.Transparent
        DataGridViewCellStyle1.SelectionForeColor = Color.Transparent
        DataGridViewCellStyle1.WrapMode = DataGridViewTriState.True
        dgCaixa.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle1
        DataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle2.BackColor = Color.FromArgb(CByte(64), CByte(64), CByte(64))
        DataGridViewCellStyle2.Font = New Font("Segoe UI", 9F)
        DataGridViewCellStyle2.ForeColor = Color.Bisque
        DataGridViewCellStyle2.SelectionBackColor = Color.Transparent
        DataGridViewCellStyle2.SelectionForeColor = Color.Transparent
        DataGridViewCellStyle2.WrapMode = DataGridViewTriState.True
        dgCaixa.DefaultCellStyle = DataGridViewCellStyle2
        dgCaixa.EnableHeadersVisualStyles = False
        dgCaixa.Location = New Point(12, 35)
        dgCaixa.MultiSelect = False
        dgCaixa.Name = "dgCaixa"
        dgCaixa.ReadOnly = True
        DataGridViewCellStyle3.BackColor = Color.FromArgb(CByte(64), CByte(64), CByte(64))
        DataGridViewCellStyle3.Font = New Font("Segoe UI", 9F)
        DataGridViewCellStyle3.ForeColor = SystemColors.ButtonHighlight
        DataGridViewCellStyle3.Padding = New Padding(2)
        DataGridViewCellStyle3.SelectionBackColor = Color.Transparent
        DataGridViewCellStyle3.SelectionForeColor = Color.Transparent
        DataGridViewCellStyle3.WrapMode = DataGridViewTriState.True
        dgCaixa.RowHeadersDefaultCellStyle = DataGridViewCellStyle3
        dgCaixa.RowHeadersWidth = 4
        dgCaixa.RowTemplate.DefaultCellStyle.SelectionBackColor = Color.DarkOrange
        dgCaixa.RowTemplate.DefaultCellStyle.SelectionForeColor = Color.FromArgb(CByte(64), CByte(64), CByte(64))
        dgCaixa.RowTemplate.DefaultCellStyle.WrapMode = DataGridViewTriState.True
        dgCaixa.ScrollBars = ScrollBars.Vertical
        dgCaixa.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgCaixa.Size = New Size(272, 118)
        dgCaixa.TabIndex = 12
        ' 
        ' FormCaixa
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        BackColor = Color.FromArgb(CByte(40), CByte(40), CByte(40))
        ClientSize = New Size(296, 165)
        Controls.Add(dgCaixa)
        Controls.Add(Label2)
        Controls.Add(btCancelTimer)
        FormBorderStyle = FormBorderStyle.None
        Icon = CType(resources.GetObject("$this.Icon"), Icon)
        MaximizeBox = False
        MinimizeBox = False
        Name = "FormCaixa"
        StartPosition = FormStartPosition.Manual
        Text = "PortfolioCripto - Caixa"
        CType(dgCaixa, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents btCancelTimer As Button
    Friend WithEvents Label2 As Label
    Friend WithEvents dgCaixa As DataGridView
End Class
