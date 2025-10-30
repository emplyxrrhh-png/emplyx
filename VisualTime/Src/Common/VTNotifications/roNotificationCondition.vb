Imports System.Data.Common
Imports System.Runtime.Serialization
Imports Robotics.Base.DTOs
Imports Robotics.VTBase

Namespace Notifications

    <DataContract()>
    Public Class roNotificationCondition

#Region "Declarations - Constructor"

        Private oState As roNotificationState

        Private intDaysBefore As Nullable(Of Integer)
        Private intIDCause As Nullable(Of Integer)
        Private intDaysNoWorking As Nullable(Of Integer)
        Private strMailListUserfield As String
        Private strNotificationPeriod As Integer
        Private strNotificationRepeat As Integer
        Private strDatePeriodUserfield As String
        Private lstIDShifts As Integer()
        Private intIDConcept As Nullable(Of Integer)
        Private iCompareConceptType As Nullable(Of ConditionCompareType)
        Private strTargetTypeConcept As String
        Private strTargetConcept As String
        Private strTypeToIgnore As String
        Private strConditionRole As String()
        Private bolOnlyFromCalendar As Boolean
        Private enumBreachCompareType As DTOs.eBreachControlType
        Private iBreachCourtesy As Integer
        Private firstPunchLimit As DateTime
        Private firstPunchToleranceTime As DateTime
        Private employeeFilterCompose As String

        Public Sub New()
            Me.oState = New roNotificationState
            lstIDShifts = {}
            strMailListUserfield = String.Empty
            strDatePeriodUserfield = String.Empty
            strTypeToIgnore = String.Empty
            strConditionRole = {}
            strNotificationPeriod = 0
            strNotificationRepeat = 0
            strTargetTypeConcept = String.Empty
            strTargetConcept = String.Empty
            bolOnlyFromCalendar = False
        End Sub

        Public Sub New(ByVal _State As roNotificationState, ByVal strXml As String)
            Me.oState = _State
            lstIDShifts = {}
            strConditionRole = {}
            strTypeToIgnore = String.Empty
            strMailListUserfield = String.Empty
            strDatePeriodUserfield = String.Empty
            strTargetTypeConcept = String.Empty
            strTargetConcept = String.Empty
            bolOnlyFromCalendar = False
            enumBreachCompareType = eBreachControlType.Both
            iBreachCourtesy = 0
            Me.LoadFromXml(strXml)
        End Sub

#End Region

#Region "Properties"

        <IgnoreDataMember()>
        Public Property State() As roNotificationState
            Get
                Return Me.oState
            End Get
            Set(ByVal value As roNotificationState)
                Me.oState = value
            End Set
        End Property

        <DataMember()>
        Public Property DaysBefore() As Nullable(Of Integer)
            Get
                Return Me.intDaysBefore
            End Get
            Set(ByVal value As Nullable(Of Integer))
                Me.intDaysBefore = value
            End Set
        End Property

        <DataMember()>
        Public Property IDShifts() As Integer()
            Get
                Return Me.lstIDShifts
            End Get
            Set(ByVal value As Integer())
                Me.lstIDShifts = value
            End Set
        End Property

        <DataMember()>
        Public Property IDCause() As Nullable(Of Integer)
            Get
                Return Me.intIDCause
            End Get
            Set(ByVal value As Nullable(Of Integer))
                Me.intIDCause = value
            End Set
        End Property

        <DataMember()>
        Public Property DaysNoWorking() As Nullable(Of Integer)
            Get
                Return Me.intDaysNoWorking
            End Get
            Set(ByVal value As Nullable(Of Integer))
                Me.intDaysNoWorking = value
            End Set
        End Property
        <DataMember()>
        Public Property MailListUserfield() As String
            Get
                Return Me.strMailListUserfield
            End Get
            Set(ByVal value As String)
                Me.strMailListUserfield = value
            End Set
        End Property
        <DataMember()>
        Public Property NotificationPeriod() As Integer
            Get
                Return Me.strNotificationPeriod
            End Get
            Set(ByVal value As Integer)
                Me.strNotificationPeriod = value
            End Set
        End Property
        <DataMember()>
        Public Property NotificationRepeat() As Integer
            Get
                Return Me.strNotificationRepeat
            End Get
            Set(ByVal value As Integer)
                Me.strNotificationRepeat = value
            End Set
        End Property
        <DataMember()>
        Public Property DatePeriodUserfield() As String
            Get
                Return Me.strDatePeriodUserfield
            End Get
            Set(ByVal value As String)
                Me.strDatePeriodUserfield = value
            End Set
        End Property
        <DataMember()>
        Public Property IDConcept() As Nullable(Of Integer)
            Get
                Return Me.intIDConcept
            End Get
            Set(ByVal value As Nullable(Of Integer))
                Me.intIDConcept = value
            End Set
        End Property
        <DataMember()>
        Public Property tCompareConceptType() As Nullable(Of ConditionCompareType)
            Get
                Return Me.iCompareConceptType
            End Get
            Set(ByVal value As Nullable(Of ConditionCompareType))
                Me.iCompareConceptType = value
            End Set
        End Property
        <DataMember()>
        Public Property TargetTypeConcept() As String
            Get
                Return Me.strTargetTypeConcept
            End Get
            Set(ByVal value As String)
                Me.strTargetTypeConcept = value
            End Set
        End Property
        <DataMember()>
        Public Property TargetConcept() As String
            Get
                Return Me.strTargetConcept
            End Get
            Set(ByVal value As String)
                Me.strTargetConcept = value
            End Set
        End Property
        <DataMember()>
        Public Property TypeToIgnore() As String
            Get
                Return Me.strTypeToIgnore
            End Get
            Set(ByVal value As String)
                Me.strTypeToIgnore = value
            End Set
        End Property
        <DataMember()>
        Public Property ConditionRole() As String()
            Get
                Return Me.strConditionRole
            End Get
            Set(ByVal value As String())
                Me.strConditionRole = value
            End Set
        End Property

        <DataMember()>
        Public Property OnlyFromCalendar As Boolean
            Get
                Return Me.bolOnlyFromCalendar
            End Get
            Set(ByVal value As Boolean)
                Me.bolOnlyFromCalendar = value
            End Set
        End Property

        <DataMember()>
        Public Property BreachCompareType As DTOs.eBreachControlType
            Get
                Return Me.enumBreachCompareType
            End Get
            Set(ByVal value As DTOs.eBreachControlType)
                Me.enumBreachCompareType = value
            End Set
        End Property

        <DataMember()>
        Public Property BreachCourtesy As Integer
            Get
                Return Me.iBreachCourtesy
            End Get
            Set(ByVal value As Integer)
                Me.iBreachCourtesy = value
            End Set
        End Property

        <DataMember()>
        Public Property PunchBeforeStartTimeLimit As DateTime
            Get
                Return Me.firstPunchLimit
            End Get
            Set(ByVal value As DateTime)
                Me.firstPunchLimit = value
            End Set
        End Property

        <DataMember()>
        Public Property PunchToleranceTime As DateTime
            Get
                Return Me.firstPunchToleranceTime
            End Get
            Set(ByVal value As DateTime)
                Me.firstPunchToleranceTime = value
            End Set
        End Property


        <DataMember()>
        Public Property EmployeeFilter As String
            Get
                Return Me.employeeFilterCompose
            End Get
            Set(ByVal value As String)
                Me.employeeFilterCompose = value
            End Set
        End Property

#End Region

#Region "Methods"

        Public Function GetXml() As String

            Dim oCondition As New roCollection

            If intDaysBefore.HasValue Then oCondition.Add("DaysBefore", intDaysBefore.Value)
            If intIDCause.HasValue Then oCondition.Add("IDCause", Me.intIDCause.Value)

            If (lstIDShifts.Length > 0) Then oCondition.Add("SelectedShifts", String.Join(",", lstIDShifts))

            If intDaysNoWorking.HasValue Then oCondition.Add("DaysNoWorking", Me.intDaysNoWorking.Value)
            oCondition.Add("MailListUserfield", Me.strMailListUserfield)
            oCondition.Add("DatePeriodUserfield", Me.strDatePeriodUserfield)

            If strNotificationPeriod <> 0 Then oCondition.Add("NotificationPeriod", Me.strNotificationPeriod)
            If strNotificationRepeat <> 0 Then oCondition.Add("NotificationRepeat", Me.strNotificationRepeat)

            If intIDConcept.HasValue Then oCondition.Add("IDConcept", intIDConcept.Value)
            If iCompareConceptType.HasValue Then oCondition.Add("CompareConceptType", iCompareConceptType.Value)

            If Me.bolOnlyFromCalendar Then oCondition.Add("OnlyFromCalendar", Me.bolOnlyFromCalendar)
            oCondition.Add("TargetTypeConcept", Me.strTargetTypeConcept)
            oCondition.Add("TargetConcept", Me.strTargetConcept)

            If Me.strTypeToIgnore IsNot Nothing Then
                oCondition.Add("TypeToIgnore", Me.strTypeToIgnore)
            Else
                oCondition.Add("TypeToIgnore", "")
            End If
            If Me.strConditionRole IsNot Nothing Then
                oCondition.Add("ConditionRole", String.Join(",", Me.strConditionRole))
            Else
                oCondition.Add("ConditionRole", "")
            End If

            oCondition.Add("BreachControlType", Me.BreachCompareType)
            oCondition.Add("BreachCourtesy", Me.BreachCourtesy)

            oCondition.Add("PunchBeforeStartTimeLimit", Me.PunchBeforeStartTimeLimit)
            oCondition.Add("PunchToleranceTime", Me.PunchToleranceTime)

            ' Me quedo con la parte significativa del filtro
            Dim selectorFilter As New roSelectorFilter
            If Not String.IsNullOrWhiteSpace(Me.EmployeeFilter) AndAlso Me.EmployeeFilter <> "None" Then
                Try
                    selectorFilter = roJSONHelper.Deserialize(Of roSelectorFilter)(Me.EmployeeFilter)
                Catch ex As Exception
                    selectorFilter = New roSelectorFilter()
                End Try

            End If
            oCondition.Add("EmployeeFilter", roJSONHelper.Serialize(selectorFilter))

            Return oCondition.XML

        End Function

        Private Sub LoadFromXml(ByVal strXml As String)

            If strXml <> "" Then
                ' Añadimos la composición a la colección
                Dim oCondition As New roCollection(strXml)
                If oCondition.Exists("DaysBefore") Then intDaysBefore = oCondition.Item("DaysBefore")
                If oCondition.Exists("IDCause") Then intIDCause = oCondition.Item("IDCause")
                If oCondition.Exists("DaysNoWorking") Then intDaysNoWorking = oCondition.Item("DaysNoWorking")
                If oCondition.Exists("MailListUserfield") Then strMailListUserfield = oCondition.Item("MailListUserfield")
                If oCondition.Exists("DatePeriodUserfield") Then strDatePeriodUserfield = oCondition.Item("DatePeriodUserfield")
                If oCondition.Exists("TypeToIgnore") Then strTypeToIgnore = oCondition.Item("TypeToIgnore")
                If oCondition.Exists("ConditionRole") Then strConditionRole = oCondition.Item("ConditionRole").ToString.Split(",")
                If oCondition.Exists("NotificationPeriod") Then strNotificationPeriod = oCondition.Item("NotificationPeriod")
                If oCondition.Exists("NotificationRepeat") Then strNotificationRepeat = oCondition.Item("NotificationRepeat")
                If oCondition.Exists("OnlyFromCalendar") Then bolOnlyFromCalendar = roTypes.Any2Boolean(oCondition.Item("OnlyFromCalendar"))

                If oCondition.Exists("SelectedShifts") Then
                    Dim tmpShifts As String() = roTypes.Any2String(oCondition.Item("SelectedShifts")).Split(",")

                    If (tmpShifts.Length > 0) Then
                        Dim oLst As New Generic.List(Of String)
                        oLst.AddRange(tmpShifts)
                        lstIDShifts = oLst.ConvertAll(Function(str) CInt(str)).ToArray()
                    End If
                End If

                If oCondition.Exists("IDConcept") Then intIDConcept = oCondition.Item("IDConcept")
                If oCondition.Exists("CompareConceptType") Then iCompareConceptType = CType(oCondition.Item("CompareConceptType"), ConditionCompareType)
                If oCondition.Exists("TargetTypeConcept") Then strTargetTypeConcept = oCondition.Item("TargetTypeConcept")
                If oCondition.Exists("TargetConcept") Then strTargetConcept = oCondition.Item("TargetConcept")

                If oCondition.Exists("BreachControlType") Then BreachCompareType = roTypes.Any2Integer(oCondition.Item("BreachControlType"))
                If oCondition.Exists("BreachCourtesy") Then iBreachCourtesy = roTypes.Any2Integer(oCondition.Item("BreachCourtesy"))
                If oCondition.Exists("PunchBeforeStartTimeLimit") Then PunchBeforeStartTimeLimit = roTypes.Any2DateTime(oCondition.Item("PunchBeforeStartTimeLimit"))
                If oCondition.Exists("PunchToleranceTime") Then PunchToleranceTime = roTypes.Any2DateTime(oCondition.Item("PunchToleranceTime"))
                If oCondition.Exists("EmployeeFilter") Then EmployeeFilter = roTypes.Any2String(oCondition.Item("EmployeeFilter"))

            End If

        End Sub

        Public Function Validate() As Boolean

            Dim bolRet As Boolean = True

            Try

                'TODO: Falta validacions
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roNotificationCondition::Validate")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roNotificationCondition::Validate")
            Finally

            End Try

            Return bolRet

        End Function

#End Region

    End Class

End Namespace