Namespace Concept

    Public Class roAccrualsRule

        Dim intIdAccrualsRule As Integer
        Dim strName As String
        Dim strDescription As String
        Dim strDefinition As String
        Dim strSchedule As String
        Dim datBeginDate As DateTime
        Dim datEndDate As DateTime
        Dim intPriority As Integer
        Dim bolExecuteFromLastExecDay As Boolean

        Public Property IdAccrualsRule() As Integer
            Get
                Return intIdAccrualsRule
            End Get
            Set(ByVal value As Integer)
                intIdAccrualsRule = value
            End Set
        End Property

        Public Property Name() As String
            Get
                Return strName
            End Get
            Set(ByVal value As String)
                strName = value
            End Set
        End Property

        Public Property Description() As String
            Get
                Return strDescription
            End Get
            Set(ByVal value As String)
                strDescription = value
            End Set
        End Property

        Public Property Definition() As String
            Get
                Return strDefinition
            End Get
            Set(ByVal value As String)
                strDefinition = value
            End Set
        End Property

        Public Property Schedule() As String
            Get
                Return strSchedule
            End Get
            Set(ByVal value As String)
                strSchedule = value
            End Set
        End Property

        Public Property BeginDate() As DateTime
            Get
                Return datBeginDate
            End Get
            Set(ByVal value As DateTime)
                datBeginDate = value
            End Set
        End Property

        Public Property EndDate() As DateTime
            Get
                Return datEndDate
            End Get
            Set(ByVal value As DateTime)
                datEndDate = value
            End Set
        End Property

        Public Property Priority() As Integer
            Get
                Return intPriority
            End Get
            Set(ByVal value As Integer)
                intPriority = value
            End Set
        End Property

        Public Property ExecuteFromLastExecDay() As Boolean
            Get
                Return bolExecuteFromLastExecDay
            End Get
            Set(ByVal value As Boolean)
                bolExecuteFromLastExecDay = value
            End Set
        End Property
    End Class

End Namespace