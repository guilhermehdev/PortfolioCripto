Imports System.Globalization

''' <summary>
''' Valor decimal usado nas células do DataGridView.
''' Mantém o valor numérico para cálculos/Convert.ToDecimal,
''' mas usa ponto no ToString() para evitar que o CellFormatting
''' interprete 1,53 como 153.
''' </summary>
Public NotInheritable Class GridDecimal
    Implements IConvertible
    Implements IComparable
    Implements IComparable(Of GridDecimal)
    Implements IEquatable(Of GridDecimal)
    Implements IFormattable

    Public ReadOnly Property Value As Decimal

    Public Sub New(value As Decimal)
        Me.Value = value
    End Sub

    Public Overrides Function ToString() As String
        Return Value.ToString(CultureInfo.InvariantCulture)
    End Function

    Public Function ToString(format As String, formatProvider As IFormatProvider) As String Implements IFormattable.ToString
        Return Value.ToString(format, formatProvider)
    End Function

    Public Function CompareTo(other As GridDecimal) As Integer Implements IComparable(Of GridDecimal).CompareTo
        Return Value.CompareTo(other.Value)
    End Function

    Public Function CompareTo(obj As Object) As Integer Implements IComparable.CompareTo
        If obj Is Nothing Then Return 1

        If TypeOf obj Is GridDecimal Then
            Return Value.CompareTo(DirectCast(obj, GridDecimal).Value)
        End If

        Return Value.CompareTo(Convert.ToDecimal(obj, CultureInfo.InvariantCulture))
    End Function

    Public Function Equals(other As GridDecimal) As Boolean Implements IEquatable(Of GridDecimal).Equals
        Return Value = other.Value
    End Function

    Public Overrides Function Equals(obj As Object) As Boolean
        If TypeOf obj Is GridDecimal Then
            Return Equals(DirectCast(obj, GridDecimal))
        End If

        Return False
    End Function

    Public Overrides Function GetHashCode() As Integer
        Return Value.GetHashCode()
    End Function

    Public Shared Widening Operator CType(value As Decimal) As GridDecimal
        Return New GridDecimal(value)
    End Operator

    Public Shared Widening Operator CType(value As GridDecimal) As Decimal
        Return value.Value
    End Operator

    Public Shared Operator >(left As GridDecimal, right As GridDecimal) As Boolean
        Return left.Value > right.Value
    End Operator

    Public Shared Operator <(left As GridDecimal, right As GridDecimal) As Boolean
        Return left.Value < right.Value
    End Operator

    Public Shared Operator >=(left As GridDecimal, right As GridDecimal) As Boolean
        Return left.Value >= right.Value
    End Operator

    Public Shared Operator <=(left As GridDecimal, right As GridDecimal) As Boolean
        Return left.Value <= right.Value
    End Operator

    Public Shared Operator =(left As GridDecimal, right As GridDecimal) As Boolean
        Return left.Value = right.Value
    End Operator

    Public Shared Operator <>(left As GridDecimal, right As GridDecimal) As Boolean
        Return left.Value <> right.Value
    End Operator

    Public Function GetTypeCode() As TypeCode Implements IConvertible.GetTypeCode
        Return TypeCode.Decimal
    End Function

    Public Function ToBoolean(provider As IFormatProvider) As Boolean Implements IConvertible.ToBoolean
        Return Convert.ToBoolean(Value, provider)
    End Function

    Public Function ToByte(provider As IFormatProvider) As Byte Implements IConvertible.ToByte
        Return Convert.ToByte(Value, provider)
    End Function

    Public Function ToChar(provider As IFormatProvider) As Char Implements IConvertible.ToChar
        Return Convert.ToChar(Value, provider)
    End Function

    Public Function ToDateTime(provider As IFormatProvider) As DateTime Implements IConvertible.ToDateTime
        Return Convert.ToDateTime(Value, provider)
    End Function

    Public Function ToDecimal(provider As IFormatProvider) As Decimal Implements IConvertible.ToDecimal
        Return Value
    End Function

    Public Function ToDouble(provider As IFormatProvider) As Double Implements IConvertible.ToDouble
        Return Convert.ToDouble(Value, provider)
    End Function

    Public Function ToInt16(provider As IFormatProvider) As Short Implements IConvertible.ToInt16
        Return Convert.ToInt16(Value, provider)
    End Function

    Public Function ToInt32(provider As IFormatProvider) As Integer Implements IConvertible.ToInt32
        Return Convert.ToInt32(Value, provider)
    End Function

    Public Function ToInt64(provider As IFormatProvider) As Long Implements IConvertible.ToInt64
        Return Convert.ToInt64(Value, provider)
    End Function

    Public Function ToSByte(provider As IFormatProvider) As SByte Implements IConvertible.ToSByte
        Return Convert.ToSByte(Value, provider)
    End Function

    Public Function ToSingle(provider As IFormatProvider) As Single Implements IConvertible.ToSingle
        Return Convert.ToSingle(Value, provider)
    End Function

    Public Function ToString(provider As IFormatProvider) As String Implements IConvertible.ToString
        Return Value.ToString(provider)
    End Function

    Public Function ToType(conversionType As Type, provider As IFormatProvider) As Object Implements IConvertible.ToType
        Return Convert.ChangeType(Value, conversionType, provider)
    End Function

    Public Function ToUInt16(provider As IFormatProvider) As UShort Implements IConvertible.ToUInt16
        Return Convert.ToUInt16(Value, provider)
    End Function

    Public Function ToUInt32(provider As IFormatProvider) As UInteger Implements IConvertible.ToUInt32
        Return Convert.ToUInt32(Value, provider)
    End Function

    Public Function ToUInt64(provider As IFormatProvider) As ULong Implements IConvertible.ToUInt64
        Return Convert.ToUInt64(Value, provider)
    End Function

End Class
