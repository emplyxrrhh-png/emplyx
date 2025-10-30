Public Class roTaskAccrualItem
    Private mPeriod As New roTimePeriod
    Private mTimeValue As New roTime
    Private mIDJob As Double
    Private mField1 As String
    Private mField2 As String
    Private mField3 As String
    Private mField4 As Double
    Private mField5 As Double
    Private mField6 As Double

    Public Property Period() As roTimePeriod
        Get
            Return mPeriod
        End Get
        Set(ByVal value As roTimePeriod)
            mPeriod = value
        End Set
    End Property

    Public Property TimeValue() As roTime
        Get
            Return mTimeValue
        End Get
        Set(ByVal value As roTime)
            mTimeValue = value
        End Set
    End Property

    Public Property IDJob() As Double
        Get
            Return mIDJob
        End Get
        Set(ByVal value As Double)
            mIDJob = value
        End Set
    End Property

    Public Property Field1() As String
        Get
            Return mField1
        End Get
        Set(ByVal value As String)
            mField1 = value
        End Set
    End Property

    Public Property Field2() As String
        Get
            Return mField2
        End Get
        Set(ByVal value As String)
            mField2 = value
        End Set
    End Property

    Public Property Field3() As String
        Get
            Return mField3
        End Get
        Set(ByVal value As String)
            mField3 = value
        End Set
    End Property

    Public Property Field4() As Double
        Get
            Return mField4
        End Get
        Set(ByVal value As Double)
            mField4 = value
        End Set
    End Property

    Public Property Field5() As Double
        Get
            Return mField5
        End Get
        Set(ByVal value As Double)
            mField5 = value
        End Set
    End Property

    Public Property Field6() As Double
        Get
            Return mField6
        End Get
        Set(ByVal value As Double)
            mField6 = value
        End Set
    End Property

End Class