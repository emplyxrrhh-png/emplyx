Public Class roTaskMoveItem
    Private mID As Double

    Private mPeriod As New roTimePeriod

    Private mIDEmployee As Integer

    Private mIDJob As Double
    Private mField1 As String
    Private mField2 As String
    Private mField3 As String
    Private mField4 As Double
    Private mField5 As Double
    Private mField6 As Double

    Public Function Clone() As roTaskMoveItem
        '
        ' Devuelve una copia de este objeto
        '
        Dim ClonedMove As New roTaskMoveItem

        ClonedMove.ID = ID
        ClonedMove.Period = Period.Clone
        ClonedMove.IDEmployee = IDEmployee
        ClonedMove.IDJob = IDJob
        ClonedMove.Field1 = Field1
        ClonedMove.Field2 = Field2
        ClonedMove.Field3 = Field3
        ClonedMove.Field4 = Field4
        ClonedMove.Field5 = Field5
        ClonedMove.Field6 = Field6

        Clone = ClonedMove

    End Function

    Public Property ID() As Double
        Get
            Return mID
        End Get
        Set(ByVal value As Double)
            mID = value
        End Set
    End Property

    Public Property Period() As roTimePeriod
        Get
            Return mPeriod
        End Get
        Set(ByVal value As roTimePeriod)
            mPeriod = value
        End Set
    End Property

    Public Property IDEmployee() As Double
        Get
            Return mIDEmployee
        End Get
        Set(ByVal value As Double)
            mIDEmployee = value
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