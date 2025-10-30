Public Class roAccrual

    Public Enum AccrualType
        Hours
        Ocurrences
    End Enum

    Public Enum AccrualQueryPeriod
        Month
        Year
        Week
        Contract
    End Enum

    Private intID As Integer
    Private strName As String
    Private oType As AccrualType
    Private oQueryPeriod As AccrualQueryPeriod
    Private dblValue As Double

    Public Sub New(ByVal _ID As Integer, ByVal _Name As String, ByVal _Type As AccrualType, ByVal _QueryPeriod As AccrualQueryPeriod, Optional ByVal _Value As Double = 0)

        Me.intID = _ID
        Me.strName = _Name
        Me.oType = _Type
        Me.oQueryPeriod = _QueryPeriod
        Me.dblValue = _Value

    End Sub

    Public Sub New(ByVal _ID As Integer, ByVal _Name As String, ByVal _strType As String, ByVal _strQueryPeriod As String, Optional ByVal _Value As Double = 0)

        Me.intID = _ID
        Me.strName = _Name
        Select Case _strType
            Case "H"
                Me.oType = AccrualType.Hours
            Case "O"
                Me.oType = AccrualType.Ocurrences
        End Select
        Select Case _strQueryPeriod
            Case "M"
                Me.oQueryPeriod = AccrualQueryPeriod.Month
            Case "Y"
                Me.oQueryPeriod = AccrualQueryPeriod.Year
        End Select
        Me.dblValue = _Value

    End Sub

    Public ReadOnly Property ID() As Integer
        Get
            Return Me.intID
        End Get
    End Property

    Public ReadOnly Property Name() As String
        Get
            Return Me.strName
        End Get
    End Property

    Public ReadOnly Property Type() As AccrualType
        Get
            Return Me.oType
        End Get
    End Property

    Public ReadOnly Property QueryPeriod() As AccrualQueryPeriod
        Get
            Return Me.oQueryPeriod
        End Get
    End Property

    Public Property Value() As Double
        Get
            Return Me.dblValue
        End Get
        Set(ByVal value As Double)
            Me.dblValue = value
        End Set
    End Property

End Class