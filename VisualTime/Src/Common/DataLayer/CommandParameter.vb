''' <summary>
''' Class for specify parameters on sql commands
''' </summary>
Public NotInheritable Class CommandParameter

    Public Enum ParameterType
        tInt
        tBoolean
        tDouble
        tDateTime
        tString
        tVarBinary
    End Enum

    Public Sub New()

    End Sub

    Public Sub New(pName As String, pType As ParameterType, pValue As Object)
        _Name = pName
        _Type = pType
        _Value = pValue
    End Sub

    Public Const prefixParameter = "@"

    Dim _Name As String

    Public Property Name As String
        Get

            Return _Name
        End Get
        Set(value As String)
            _Name = IIf(value.StartsWith(prefixParameter), value, String.Format("{0}{1}", prefixParameter, value))
        End Set
    End Property

    Dim _Value As Object

    Public Property Value As Object
        Get

            Return _Value
        End Get
        Set(value As Object)
            _Value = value
        End Set
    End Property

    Dim _Type As ParameterType

    Public Property Type As ParameterType
        Get

            Return _Type
        End Get
        Set(value As ParameterType)
            _Type = value
        End Set
    End Property

End Class