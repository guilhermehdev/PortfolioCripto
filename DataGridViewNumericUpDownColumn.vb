Imports System.ComponentModel

Public Class DataGridViewNumericUpDownColumn
    Inherits DataGridViewColumn

    Public Sub New()
        MyBase.New(New DataGridViewNumericUpDownCell())
    End Sub

    <DefaultValue(0D)>
    Public Property Minimum As Decimal
        Get
            Return DirectCast(CellTemplate, DataGridViewNumericUpDownCell).Minimum
        End Get
        Set(value As Decimal)
            DirectCast(CellTemplate, DataGridViewNumericUpDownCell).Minimum = value
        End Set
    End Property

    <DefaultValue(100D)>
    Public Property Maximum As Decimal
        Get
            Return DirectCast(CellTemplate, DataGridViewNumericUpDownCell).Maximum
        End Get
        Set(value As Decimal)
            DirectCast(CellTemplate, DataGridViewNumericUpDownCell).Maximum = value
        End Set
    End Property

    <DefaultValue(1D)>
    Public Property Increment As Decimal = 1D

    Public Overrides Function Clone() As Object
        Dim clonedColumn = DirectCast(MyBase.Clone(), DataGridViewNumericUpDownColumn)
        clonedColumn.Minimum = Minimum
        clonedColumn.Maximum = Maximum
        clonedColumn.Increment = Increment
        Return clonedColumn
    End Function
End Class

Public Class DataGridViewNumericUpDownCell
    Inherits DataGridViewTextBoxCell

    Public Property Minimum As Decimal
    Public Property Maximum As Decimal = 100D
    Public Property Increment As Decimal = 1D

    Public Overrides ReadOnly Property EditType As Type
        Get
            Return GetType(DataGridViewNumericUpDownEditingControl)
        End Get
    End Property

    Public Overrides Sub InitializeEditingControl(rowIndex As Integer, initialFormattedValue As Object, dataGridViewCellStyle As DataGridViewCellStyle)
        MyBase.InitializeEditingControl(rowIndex, initialFormattedValue, dataGridViewCellStyle)
        Dim control = DirectCast(DataGridView.EditingControl, DataGridViewNumericUpDownEditingControl)
        control.Minimum = Minimum
        control.Maximum = Maximum
        control.Increment = Increment
        Dim value As Decimal
        If Decimal.TryParse(Convert.ToString(Value, Globalization.CultureInfo.InvariantCulture), value) Then control.Value = Math.Min(Maximum, Math.Max(Minimum, value))
    End Sub
End Class

Public Class DataGridViewNumericUpDownEditingControl
    Inherits NumericUpDown
    Implements IDataGridViewEditingControl

    Private _grid As DataGridView
    Private _rowIndex As Integer
    Private _valueChanged As Boolean

    Public Function GetEditingControlFormattedValue(context As DataGridViewDataErrorContexts) As Object Implements IDataGridViewEditingControl.GetEditingControlFormattedValue
        Return Value
    End Function
    Public Property EditingControlFormattedValue As Object Implements IDataGridViewEditingControl.EditingControlFormattedValue
        Get
            Return Value
        End Get
        Set(value As Object)
            Dim parsed As Decimal
            If Decimal.TryParse(Convert.ToString(value), parsed) Then Me.Value = parsed
        End Set
    End Property
    Public ReadOnly Property EditingPanelCursor As Cursor Implements IDataGridViewEditingControl.EditingPanelCursor
        Get
            Return Cursors.Default
        End Get
    End Property
    Public Sub ApplyCellStyleToEditingControl(style As DataGridViewCellStyle) Implements IDataGridViewEditingControl.ApplyCellStyleToEditingControl
        Font = style.Font
    End Sub
    Public Property EditingControlRowIndex As Integer Implements IDataGridViewEditingControl.EditingControlRowIndex
        Get
            Return _rowIndex
        End Get
        Set(value As Integer)
            _rowIndex = value
        End Set
    End Property
    Public Function EditingControlWantsInputKey(key As Keys, dataGridViewWantsInputKey As Boolean) As Boolean Implements IDataGridViewEditingControl.EditingControlWantsInputKey
        Return key = Keys.Left OrElse key = Keys.Right OrElse key = Keys.Up OrElse key = Keys.Down OrElse key = Keys.Home OrElse key = Keys.End
    End Function
    Public Sub PrepareEditingControlForEdit(selectAll As Boolean) Implements IDataGridViewEditingControl.PrepareEditingControlForEdit
        If selectAll Then Me.Select(0, Text.Length)
    End Sub
    Public ReadOnly Property RepositionEditingControlOnValueChange As Boolean Implements IDataGridViewEditingControl.RepositionEditingControlOnValueChange
        Get
            Return False
        End Get
    End Property
    Public Property EditingControlDataGridView As DataGridView Implements IDataGridViewEditingControl.EditingControlDataGridView
        Get
            Return _grid
        End Get
        Set(value As DataGridView)
            _grid = value
        End Set
    End Property
    Public Property EditingControlValueChanged As Boolean Implements IDataGridViewEditingControl.EditingControlValueChanged
        Get
            Return _valueChanged
        End Get
        Set(value As Boolean)
            _valueChanged = value
        End Set
    End Property
    Protected Overrides Sub OnValueChanged(e As EventArgs)
        _valueChanged = True
        _grid?.NotifyCurrentCellDirty(True)
        MyBase.OnValueChanged(e)
    End Sub
End Class
