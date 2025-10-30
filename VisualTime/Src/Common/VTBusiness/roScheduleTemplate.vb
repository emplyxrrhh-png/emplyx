Imports System.Data.Common
Imports System.Runtime.Serialization
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTBusiness.Support
Imports Robotics.Base.VTEmployees
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.Security
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions
Imports Robotics.VTBase.Extensions.VTLiveTasks
Imports Robotics.VTBase.roTypes
Imports Robotics.Security.Base

Namespace Scheduler

    <DataContract()>
    Public Class roScheduleTemplate

#Region "Declarations - Constructor"

        Private oState As roSchedulerState

        Private intID As Integer
        Private strName As String
        Private strDescription As String

        Private bolFeastTemplate As Boolean

        Private intMode As Integer

        Private lstScheduleDates As Generic.List(Of roScheduleTemplateDate)

        Public Sub New()
            Me.oState = New roSchedulerState
            Me.intID = -1
            Me.strName = ""
            Me.strDescription = ""
            Me.bolFeastTemplate = False
            Me.intMode = 0
            Me.lstScheduleDates = New Generic.List(Of roScheduleTemplateDate)
        End Sub

        Public Sub New(ByVal _ID As Integer, ByVal _State As roSchedulerState,
                       Optional ByVal bAudit As Boolean = False)
            Me.oState = _State
            Me.intID = _ID
            Me.intMode = 0
            Me.lstScheduleDates = New Generic.List(Of roScheduleTemplateDate)
            Me.Load(bAudit)
        End Sub

        <OnDeserializing>
        Private Sub OnDeserialize(pp As StreamingContext)
            If Me.oState Is Nothing Then
                Me.oState = New roSchedulerState(roTypes.Any2Integer(roConstants.GetGlobalEnvironmentParameter(GlobalAsaxParameter.CurrentIdPassport)))
            End If
        End Sub

#End Region

#Region "Properties"

        Public Property State() As roSchedulerState
            Get
                Return Me.oState
            End Get
            Set(ByVal value As roSchedulerState)
                Me.oState = value
            End Set
        End Property
        <DataMember()>
        Public Property ID() As Integer
            Get
                Return Me.intID
            End Get
            Set(ByVal value As Integer)
                Me.intID = value
            End Set
        End Property
        <DataMember()>
        Public Property Name() As String
            Get
                Return Me.strName
            End Get
            Set(ByVal value As String)
                Me.strName = value
            End Set
        End Property
        <DataMember()>
        Public Property Description() As String
            Get
                Return Me.strDescription
            End Get
            Set(ByVal value As String)
                Me.strDescription = value
            End Set
        End Property
        <DataMember()>
        Public Property ScheduleDates() As Generic.List(Of roScheduleTemplateDate)
            Get
                Return Me.lstScheduleDates
            End Get
            Set(ByVal value As Generic.List(Of roScheduleTemplateDate))
                Me.lstScheduleDates = value
            End Set
        End Property
        <DataMember()>
        Public Property FeastTemplate() As Boolean
            Get
                Return Me.bolFeastTemplate
            End Get
            Set(ByVal value As Boolean)
                Me.bolFeastTemplate = value
            End Set
        End Property

        <DataMember()>
        Public Property Mode() As Integer
            Get
                Return Me.intMode
            End Get
            Set(ByVal value As Integer)
                Me.intMode = value
            End Set
        End Property

#End Region

#Region "Methods"

        Public Function Load(Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Try

                Me.strName = ""
                Me.strDescription = ""
                Me.bolFeastTemplate = False
                Me.lstScheduleDates = New Generic.List(Of roScheduleTemplateDate)
                Me.intMode = 0

                Dim strSQL As String

                strSQL = "@SELECT# * FROM sysroScheduleTemplates " &
                         "WHERE ID = " & Me.intID.ToString
                Dim tbTemplate As DataTable = CreateDataTable(strSQL, )
                If tbTemplate IsNot Nothing AndAlso tbTemplate.Rows.Count = 1 Then

                    Me.strName = tbTemplate.Rows(0).Item("Name")
                    Me.strDescription = Any2String(tbTemplate.Rows(0).Item("Description"))
                    Me.bolFeastTemplate = Any2Boolean(tbTemplate.Rows(0).Item("FeastTemplate"))
                    Me.intMode = Any2Integer(tbTemplate.Rows(0).Item("Mode"))

                    strSQL = "@SELECT# * FROM sysroScheduleTemplates_Detail " &
                             "WHERE IDTemplate = " & Me.intID.ToString & " " &
                             "ORDER BY ScheduleDate"
                    Dim tbDetail As DataTable = CreateDataTable(strSQL, )
                    If tbDetail IsNot Nothing Then
                        For Each oRow As DataRow In tbDetail.Rows
                            Dim oScheduleTemplateDate As New roScheduleTemplateDate(CDate(oRow("ScheduleDate")), Any2String(oRow("Description")))
                            Me.lstScheduleDates.Add(oScheduleTemplateDate)
                        Next
                    End If

                End If

                bolRet = True

                ' Auditar lectura
                If bAudit Then
                    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    oState.AddAuditParameter(tbParameters, "{Name}", Me.strName, "", 1)
                    bolRet = Me.oState.Audit(Audit.Action.aSelect, Audit.ObjectType.tSchedulerTemplate, Me.strName, tbParameters, -1)
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roScheduleTemplate::Load")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roScheduleTemplate::Load")
            Finally

            End Try

            Return bolRet

        End Function

        Public Function Validate() As Boolean
            Dim bolRet As Boolean = True

            Try

                If Me.strName = "" Then
                    bolRet = False
                    Me.oState.Result = SchedulerResultEnum.InvalidScheduleTemplateName
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roScheduleTemplate::Validate")
                bolRet = False
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roScheduleTemplate::Validate")
                bolRet = False
            End Try

            Return bolRet

        End Function

        Public Function Save(Optional ByVal bAudit As Boolean = False, Optional ByVal idPassport As Integer = -1) As Boolean
            Dim bolRet As Boolean = False

            Dim bHaveToClose As Boolean = False
            Try
                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                If Me.Validate() Then
                    Dim oAuditDataOld As DataRow = Nothing
                    Dim oAuditDataNew As DataRow = Nothing

                    Dim tb As New DataTable("sysroScheduleTemplates")
                    Dim strSQL As String = "@SELECT# * FROM sysroScheduleTemplates " &
                                           "WHERE ID = " & Me.intID.ToString
                    Dim cmd As DbCommand = CreateCommand(strSQL)
                    Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)
                    da.Fill(tb)

                    Dim oRow As DataRow
                    If tb.Rows.Count = 0 Then
                        oRow = tb.NewRow
                        Me.intID = Me.GetNextID()
                        oRow("ID") = Me.intID
                    Else
                        oRow = tb.Rows(0)
                        oAuditDataOld = Extensions.roAudit.CloneRow(oRow)
                    End If

                    oRow("Name") = Me.strName
                    oRow("Description") = Me.strDescription
                    oRow("FeastTemplate") = Me.bolFeastTemplate
                    oRow("Mode") = Me.intMode

                    If tb.Rows.Count = 0 Then
                        tb.Rows.Add(oRow)
                    End If
                    da.Update(tb)

                    oAuditDataNew = oRow

                    ' Actualizamos la lista de fechas de la plantilla
                    strSQL = "@DELETE# FROM sysroScheduleTemplates_Detail " &
                             "WHERE IDTemplate = " & Me.intID.ToString
                    ExecuteSql(strSQL)

                    If Me.lstScheduleDates IsNot Nothing AndAlso Me.lstScheduleDates.Count > 0 Then

                        tb = New DataTable("sysroScheduleTemplates_Detail")
                        strSQL = "@SELECT# * FROM sysroScheduleTemplates_Detail " &
                                 "WHERE IDTemplate = " & Me.intID.ToString
                        cmd = CreateCommand(strSQL)
                        da = CreateDataAdapter(cmd, True)
                        da.Fill(tb)

                        For Each oScheduleTemplateDate As roScheduleTemplateDate In Me.lstScheduleDates
                            oRow = tb.NewRow
                            oRow("IDTemplate") = Me.intID
                            oRow("ScheduleDate") = oScheduleTemplateDate.ScheduleDate
                            oRow("Description") = ""
                            If Me.bolFeastTemplate Then oRow("Description") = oScheduleTemplateDate.Description

                            tb.Rows.Add(oRow)
                        Next

                        da.Update(tb)

                    End If

                    bolRet = True

                    If bolRet And bAudit Then
                        bolRet = False
                        ' Auditamos
                        Dim tbAuditParameters As DataTable = Extensions.roAudit.CreateParametersTable()
                        Extensions.roAudit.AddFieldsValues(tbAuditParameters, oAuditDataNew, oAuditDataOld)
                        Dim oAuditAction As Audit.Action = IIf(oAuditDataOld Is Nothing, Audit.Action.aInsert, Audit.Action.aUpdate)
                        Dim strObjectName As String
                        If oAuditAction = Audit.Action.aInsert Then
                            strObjectName = oAuditDataNew("Name")
                        Else
                            strObjectName = oAuditDataOld("Name") & " -> " & oAuditDataNew("Name")
                        End If
                        Me.oState.IDPassport = idPassport
                        bolRet = Me.oState.Audit(oAuditAction, Audit.ObjectType.tSchedulerTemplate, strObjectName, tbAuditParameters, -1)
                    End If

                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roScheduleTemplate::Save")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roScheduleTemplate::Save")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet

        End Function

        Public Function Delete(Optional ByVal bAudit As Boolean = False) As Boolean
            Dim bolRet As Boolean = False

            Dim bHaveToClose As Boolean = False
            Try
                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                Dim strSQL As String

                strSQL = "@DELETE# FROM sysroScheduleTemplates_Detail " &
                         "WHERE IDTemplate = " & Me.intID.ToString
                bolRet = ExecuteSql(strSQL)

                If bolRet Then

                    strSQL = "@DELETE# FROM sysroScheduleTemplates " &
                             "WHERE ID = " & Me.intID.ToString
                    bolRet = ExecuteSql(strSQL)

                End If

                If bolRet And bAudit Then
                    ' Auditamos
                    bolRet = Me.oState.Audit(Audit.Action.aDelete, Audit.ObjectType.tSchedulerTemplate, Me.strName, Nothing, -1)
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roScheduleTemplate::Delete")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roScheduleTemplate::Delete")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet

        End Function

        Private Function GetNextID() As Integer
            Dim intRet As Integer = -1

            Try
                Dim strSQL As String = "@SELECT# MAX(ID) as MaxID FROM sysroScheduleTemplates"
                Dim oRet As Object = ExecuteScalar(strSQL)

                If IsDBNull(oRet) Then
                    intRet = 1
                Else
                    intRet = CInt(oRet) + 1
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roScheduleTemplate::GetNextID")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roScheduleTemplate::GetNextID")
            End Try

            Return intRet

        End Function

#Region "Helper methods"

        Public Shared Function GetScheduleTemplates(ByVal _State As roSchedulerState, Optional ByVal _Mode As Integer = 0) As Generic.List(Of roScheduleTemplate)

            Dim oRet As New Generic.List(Of roScheduleTemplate)

            Try

                Dim strSQL As String = "@SELECT# ID " &
                                       "FROM sysroScheduleTemplates " &
                                       " WHERE Mode=" & _Mode.ToString &
                                       "ORDER BY Name"
                Dim tb As DataTable = CreateDataTable(strSQL, )

                If tb IsNot Nothing Then

                    For Each oRow As DataRow In tb.Rows

                        oRet.Add(New roScheduleTemplate(oRow("ID"), _State))

                    Next

                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roScheduleTemplate::GetScheduleTemplates")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roScheduleTemplate::GetScheduleTemplates")
            Finally

            End Try

            Return oRet

        End Function

        Public Shared Function AssignTemplate(ByVal _IDTemplate As Integer, ByVal _IDGroup As Integer, ByVal _UserFieldConditions As Generic.List(Of VTUserFields.UserFields.roUserFieldCondition), ByVal _Year As Integer, ByVal _IDShift As Integer,
                                              ByVal _StartShift As DateTime, ByVal _LockDays As Boolean, ByVal _IncludeChildGroups As Boolean, ByVal _LockedDayAction As LockedDayAction,
                                              ByVal _CoverageDayAction As LockedDayAction, ByRef xDateLocked As Date, ByRef intIDEmployeeLocked As Integer, ByRef _State As roSchedulerState,
                                              Optional ByVal keepHolidays As Boolean = True, Optional ByVal bolAudit As Boolean = True, Optional ByVal oTask As roLiveTask = Nothing) As Boolean

            Dim bolRet As Boolean = False

            Try

                Dim oEmployeeState As New Employee.roEmployeeState(_State.IDPassport)

                ' Obtenemos las definición de la plantilla
                Dim oTemplate As New roScheduleTemplate(_IDTemplate, _State)

                Dim oLicense As New roServerLicense
                Dim obolHRSchedulingInstalled As Boolean = oLicense.FeatureIsInstalled("Feature\HRScheduling")

                Dim xFreezingDate As Date = roParameters.GetFirstDate()

                ' Obtenemos las lista de empleados a los que hay que asignar la plantilla
                Dim strSQL As String

                strSQL = "@SELECT# sysrovwAllEmployeeGroups.IDEmployee " &
                         "FROM sysrovwAllEmployeeGroups INNER JOIN Employees " &
                                "ON sysrovwAllEmployeeGroups.IDEmployee = Employees.ID " &
                         "WHERE "
                If Not _IncludeChildGroups Then
                    strSQL &= "sysrovwAllEmployeeGroups.IDGroup = " & _IDGroup.ToString & " "
                Else
                    strSQL &= "(sysrovwAllEmployeeGroups.IDGroup = " & _IDGroup.ToString & " OR sysrovwAllEmployeeGroups.Path LIKE '%\" & _IDGroup & "\%' OR sysrovwAllEmployeeGroups.Path LIKE '" & _IDGroup.ToString & "\%') "
                End If
                If _UserFieldConditions IsNot Nothing Then
                    For Each oCondition As VTUserFields.UserFields.roUserFieldCondition In _UserFieldConditions
                        strSQL &= " AND " & oCondition.GetFilter(-1)
                    Next
                End If
                strSQL &= " ORDER BY sysrovwAllEmployeeGroups.IDEmployee"

                Dim tbEmployees As DataTable = CreateDataTable(strSQL, )

                If tbEmployees IsNot Nothing Then

                    Dim bolNotify As Boolean = False
                    Dim xDate As Date
                    Dim intIDEmployee As Integer

                    Dim bolExistBusinessGroup As Boolean = (Any2Double(ExecuteScalar("@SELECT# count(*) as Total from ShiftGroups WHERE isnull(BusinessGroup, '') <> ''")) > 0)

                    If obolHRSchedulingInstalled Then
                        obolHRSchedulingInstalled = (Any2Double(ExecuteScalar("@SELECT# count(*) as Total from Assignments ")) > 0)
                    End If

                    Dim totalActions As Integer = oTemplate.ScheduleDates.Count * tbEmployees.Rows.Count
                    If totalActions = 0 Then totalActions = 1
                    Dim stepProgress As Integer = 100 / totalActions

                    For Each oScheduleTemplateDate As roScheduleTemplateDate In oTemplate.ScheduleDates

                        Try
                            xDate = New Date(_Year, oScheduleTemplateDate.ScheduleDate.Month, oScheduleTemplateDate.ScheduleDate.Day)
                        Catch
                            xDate = Nothing
                        End Try

                        If xDate <> Nothing Then

                            If xDateLocked = Nothing OrElse xDateLocked = xDate Then

                                xDateLocked = Nothing

                                For Each oRow As DataRow In tbEmployees.Rows

                                    intIDEmployee = oRow("IDEmployee")

                                    If intIDEmployeeLocked <= 0 OrElse (intIDEmployeeLocked > 0 AndAlso intIDEmployeeLocked = intIDEmployee) Then

                                        intIDEmployeeLocked = 0

                                        'Asignamos la planificación
                                        'Si no tenemos que mantener las vacaciones, primero las quitamos y despues asignamos el horario deseado
                                        If keepHolidays = False Then
                                            Scheduler.roScheduler.AssignShiftEx(intIDEmployee, xDate, -1, 0, 0, 0, _StartShift, Nothing, Nothing, Nothing, 0,
                                                                     _LockedDayAction, _CoverageDayAction, oEmployeeState, obolHRSchedulingInstalled, xFreezingDate, bolExistBusinessGroup, True, , bolAudit)
                                        End If

                                        If Scheduler.roScheduler.AssignShiftEx(intIDEmployee, xDate, _IDShift, -1, -1, -1, _StartShift, Nothing, Nothing, Nothing, 0,
                                                                    _LockedDayAction, _CoverageDayAction, oEmployeeState, obolHRSchedulingInstalled, xFreezingDate, bolExistBusinessGroup, True, , bolAudit) Then

                                            bolNotify = True
                                            If _LockDays Then
                                                ' Bloquear el día
                                                strSQL = "@UPDATE# DailySchedule WITH (ROWLOCK) SET LockedDay = 1 " &
                                                         "WHERE IDEmployee = " & intIDEmployee.ToString & " AND " &
                                                               "Date = " & Any2Time(xDate).SQLSmallDateTime
                                                ExecuteSql(strSQL)
                                            End If
                                        End If

                                        If oEmployeeState.Result = EmployeeResultEnum.DailyScheduleLockedDay Or
                                           oEmployeeState.Result = EmployeeResultEnum.DailyScheduleCoverageDay Then
                                            ' El día está bloqueado o hay covertura. Obtenemos el código del empleado y fecha bloqueada, y finalizamos proceso.
                                            intIDEmployeeLocked = intIDEmployee
                                            xDateLocked = xDate
                                            Exit For
                                        ElseIf oEmployeeState.Result = EmployeeResultEnum.Exception Then
                                            Exit For
                                        End If

                                    End If

                                    If oTask IsNot Nothing Then
                                        oTask.Progress = oTask.Progress + stepProgress
                                        oTask.Save()
                                    End If

                                Next

                                If oEmployeeState.Result = EmployeeResultEnum.Exception Or
                                   oEmployeeState.Result = EmployeeResultEnum.DailyScheduleLockedDay Or
                                   oEmployeeState.Result = EmployeeResultEnum.DailyScheduleCoverageDay Then
                                    Exit For
                                End If

                            End If

                        End If

                    Next

                    bolRet = (oEmployeeState.Result = EmployeeResultEnum.NoError Or
                              oEmployeeState.Result = EmployeeResultEnum.InPeriodOfFreezing Or
                              oEmployeeState.Result = EmployeeResultEnum.EmployeeNoActiveContract Or
                              oEmployeeState.Result = EmployeeResultEnum.AccessDenied Or
                              oEmployeeState.Result = EmployeeResultEnum.NoWorkingDay Or
                              oEmployeeState.Result = EmployeeResultEnum.NoBaseShiftAssigned)

                    If oEmployeeState.Result = EmployeeResultEnum.DailyScheduleLockedDay Then
                        _State.Result = SchedulerResultEnum.DailyScheduleLockedDay
                    ElseIf oEmployeeState.Result = EmployeeResultEnum.DailyScheduleCoverageDay Then
                        _State.Result = SchedulerResultEnum.DailyScheduleCoverageDay
                    Else
                        If bolRet Then
                            _State.Result = SchedulerResultEnum.NoError
                        Else
                            _State.Result = SchedulerResultEnum.Exception
                            _State.IDPassport = oEmployeeState.IDPassport
                            _State.Context = oEmployeeState.Context
                            _State.ClientAddress = oEmployeeState.ClientAddress
                            _State.SessionID = oEmployeeState.SessionID
                            _State.ErrorText = oEmployeeState.ErrorText
                            _State.ErrorDetail = oEmployeeState.ErrorDetail
                            _State.ErrorNumber = oEmployeeState.ErrorNumber
                        End If
                        'roBusinessState.CopyTo(oEmployeeState, _State)
                    End If

                    If bolNotify And bolRet And _State.Result = SchedulerResultEnum.NoError Then
                        ' Notificamos el cambio
                        roConnector.InitTask(TasksType.DAILYSCHEDULE)
                    End If
                Else
                    ' No se han podido recuperar los empleados

                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roScheduleTemplate::AssignTemplate")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roScheduleTemplate::AssignTemplate")
            End Try

            Return bolRet

        End Function

        Public Shared Function AssignTemplatev2(ByVal _IDTemplate As Integer, ByVal _lstEmployees As Generic.List(Of Integer), ByVal _IDShift As Integer,
                                              ByVal _StartShift As DateTime, ByVal _LockDays As Boolean, ByVal _LockedDayAction As LockedDayAction,
                                              ByVal _CoverageDayAction As LockedDayAction, ByRef xDateLocked As Date, ByRef intIDEmployeeLocked As Integer, ByRef _State As roSchedulerState, ByVal _FeastDays As Boolean,
                                              Optional ByVal keepHolidays As Boolean = True, Optional ByVal bolAudit As Boolean = True, Optional ByVal oTask As roLiveTask = Nothing, Optional strEmployeeFilter As String = "", Optional strUserFieldToMark As String = "", Optional strUserFieldValueToSet As String = "") As Boolean

            Dim bolRet As Boolean = False

            Dim strSQL As String = ""
            Dim bUseEmployeeUserFieldFilter As Boolean = False
            Dim iTotalDaysPlanified As Integer = 0
            Dim lEmployeesPlanified As New List(Of Integer)

            Try

                Dim oEmployeeState As New Employee.roEmployeeState(_State.IDPassport)

                ' Decidimos si aplicamos a la lista de empleados directa, o al resultado de aplicar los filtros recibidos
                If strEmployeeFilter.Trim.Length > 0 Then bUseEmployeeUserFieldFilter = True

                ' Obtenemos las definición de la plantilla
                Dim oTemplate As New roScheduleTemplate(_IDTemplate, _State)

                Dim oLicense As New roServerLicense
                Dim obolHRSchedulingInstalled As Boolean = oLicense.FeatureIsInstalled("Feature\HRScheduling")

                Dim xFreezingDate As Date = roParameters.GetFirstDate()

                ' Obtenemos las lista de empleados a los que hay que asignar la plantilla

                If (Not bUseEmployeeUserFieldFilter AndAlso _lstEmployees IsNot Nothing AndAlso _lstEmployees.Count > 0) OrElse (bUseEmployeeUserFieldFilter) Then

                    Dim bolNotify As Boolean = False

                    roBusinessState.CopyTo(_State, oEmployeeState)

                    Dim xDate As Date
                    Dim intIDEmployee As Integer

                    Dim bolExistBusinessGroup As Boolean = (Any2Double(ExecuteScalar("@SELECT# count(*) as Total from ShiftGroups WHERE isnull(BusinessGroup, '') <> ''")) > 0)

                    If obolHRSchedulingInstalled Then
                        obolHRSchedulingInstalled = (Any2Double(ExecuteScalar("@SELECT# count(*) as Total from Assignments ")) > 0)
                    End If

                    Dim totalActions As Integer = oTemplate.ScheduleDates.Count * _lstEmployees.Count
                    If totalActions = 0 Then totalActions = 1
                    Dim stepProgress As Integer = 100 / totalActions

                    For Each oScheduleTemplateDate As roScheduleTemplateDate In oTemplate.ScheduleDates

                        Try
                            xDate = oScheduleTemplateDate.ScheduleDate
                        Catch
                            xDate = Nothing
                        End Try

                        If bUseEmployeeUserFieldFilter Then
                            ' Filtro lista de empleados por valores de la ficha en la fecha concreta
                            Dim conf As String() = roTypes.Any2String(strEmployeeFilter).Split("@")
                            Dim strSelectorEmployees As String = conf(0)
                            Dim strFeature As String = conf(1)
                            Dim strFilter As String = conf(2)
                            Dim strFilterUser As String = conf(3)

                            Dim lstEmployees As Generic.List(Of Integer) = roSelector.GetEmployeeList(_State.IDPassport, strFeature, "U", Permission.Read,
                                                                                                        strSelectorEmployees, strFilter, strFilterUser, False, Nothing, Nothing)

                            _lstEmployees = lstEmployees
                        End If

                        If xDate <> Nothing AndAlso (xDateLocked = Nothing OrElse xDateLocked = xDate) Then

                            xDateLocked = Nothing

                            For Each _IDEmployee As Integer In _lstEmployees

                                intIDEmployee = _IDEmployee

                                If intIDEmployeeLocked <= 0 OrElse (intIDEmployeeLocked > 0 AndAlso intIDEmployeeLocked = intIDEmployee) Then

                                    intIDEmployeeLocked = 0

                                    'Si no tenemos que mantener las vacaciones, primero las quitamos y despues asignamos el horario deseado
                                    If Not keepHolidays Then
                                        Scheduler.roScheduler.AssignShiftEx(intIDEmployee, xDate, -1, 0, 0, 0, _StartShift, Nothing, Nothing, Nothing, 0,
                                                                 _LockedDayAction, _CoverageDayAction, oEmployeeState, obolHRSchedulingInstalled, xFreezingDate, bolExistBusinessGroup, True, , bolAudit)
                                    End If

                                    Dim bUpdate As Boolean = False
                                    'Si es un 0 es que no tenemos que modificar la planificación, mantenemos el horario actual este como este.
                                    If _IDShift = 0 Then
                                        ' Obtenemos la fecha/empleado a actualizar
                                        Dim tb As New DataTable("DailySchedule")
                                        strSQL = "@SELECT# * FROM DailySchedule with (nolock) WHERE IDEmployee= " & intIDEmployee.ToString & " AND Date=" & roTypes.Any2Time(xDate).SQLSmallDateTime
                                        Dim cmd As DbCommand = CreateCommand(strSQL)
                                        Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)

                                        da.Fill(tb)

                                        If tb.Rows.Count = 0 Then
                                            bUpdate = Scheduler.roScheduler.AssignShiftEx(intIDEmployee, xDate, Nothing, -1, -1, -1, _StartShift, Nothing, Nothing, Nothing, 0,
                                                                _LockedDayAction, _CoverageDayAction, oEmployeeState, obolHRSchedulingInstalled, xFreezingDate, bolExistBusinessGroup, True, , bolAudit)
                                        Else
                                            bUpdate = True
                                        End If
                                    Else
                                        bUpdate = Scheduler.roScheduler.AssignShiftEx(intIDEmployee, xDate, _IDShift, -1, -1, -1, _StartShift, Nothing, Nothing, Nothing, 0,
                                                                _LockedDayAction, _CoverageDayAction, oEmployeeState, obolHRSchedulingInstalled, xFreezingDate, bolExistBusinessGroup, True, , bolAudit)
                                    End If

                                    If bUpdate Then

                                        bolNotify = True
                                        If _LockDays Then
                                            ' Bloquear el día
                                            strSQL = "@UPDATE# DailySchedule WITH (ROWLOCK) SET LockedDay = 1 " &
                                                     "WHERE IDEmployee = " & intIDEmployee.ToString & " AND " &
                                                           "Date = " & Any2Time(xDate).SQLSmallDateTime
                                            ExecuteSql(strSQL)
                                        End If

                                        If _FeastDays Then
                                            ' Marcamos como festivo
                                            strSQL = "@UPDATE# DailySchedule WITH (ROWLOCK) SET FeastDay = 1 " &
                                                     "WHERE IDEmployee = " & intIDEmployee.ToString & " AND " &
                                                           "Date = " & Any2Time(xDate).SQLSmallDateTime
                                            ExecuteSql(strSQL)

                                        End If
                                        ' Recuento para detalle
                                        iTotalDaysPlanified += 1
                                        If Not lEmployeesPlanified.Contains(intIDEmployee) Then lEmployeesPlanified.Add(intIDEmployee)
                                    End If

                                    If oEmployeeState.Result = EmployeeResultEnum.DailyScheduleLockedDay Or
                                       oEmployeeState.Result = EmployeeResultEnum.DailyScheduleCoverageDay Then
                                        ' El día está bloqueado o hay covertura. Obtenemos el código del empleado y fecha bloqueada, y finalizamos proceso.
                                        intIDEmployeeLocked = intIDEmployee
                                        xDateLocked = xDate
                                        Exit For
                                    ElseIf oEmployeeState.Result = EmployeeResultEnum.Exception Then
                                        Exit For
                                    End If

                                End If

                                If oTask IsNot Nothing Then
                                    oTask.Progress = oTask.Progress + stepProgress
                                    oTask.Save()
                                End If

                            Next

                            If oEmployeeState.Result = EmployeeResultEnum.Exception Or
                               oEmployeeState.Result = EmployeeResultEnum.DailyScheduleLockedDay Or
                               oEmployeeState.Result = EmployeeResultEnum.DailyScheduleCoverageDay Then
                                Exit For
                            End If

                        End If
                    Next

                    ' Si se informó, marco campo de la ficha de todo empleado planificado
                    Try
                        If strUserFieldToMark IsNot Nothing AndAlso strUserFieldValueToSet IsNot Nothing AndAlso strUserFieldToMark.Trim.Length > 0 Then
                            For Each iIDEmployeePlanified In lEmployeesPlanified
                                Dim oEmployeeUserField As New VTUserFields.UserFields.roEmployeeUserField(iIDEmployeePlanified, strUserFieldToMark.Trim, New Date(1900, 1, 1), New VTUserFields.UserFields.roUserFieldState(_State.IDPassport))
                                oEmployeeUserField.FieldValue = strUserFieldValueToSet.Trim
                                oEmployeeUserField.Save(,, False)
                            Next

                        End If
                    Catch ex As Exception
                        roLog.GetInstance().logMessage(roLog.EventType.roError, "roScheduleTemplate::AssignTemplatev2::Unknown error", ex)
                    End Try

                    bolRet = (oEmployeeState.Result = EmployeeResultEnum.NoError OrElse
                                      oEmployeeState.Result = EmployeeResultEnum.InPeriodOfFreezing OrElse
                                      oEmployeeState.Result = EmployeeResultEnum.EmployeeNoActiveContract OrElse
                                      oEmployeeState.Result = EmployeeResultEnum.AccessDenied OrElse
                                      oEmployeeState.Result = EmployeeResultEnum.NoWorkingDay OrElse
                                      oEmployeeState.Result = EmployeeResultEnum.NoBaseShiftAssigned)

                    If oEmployeeState.Result = EmployeeResultEnum.DailyScheduleLockedDay Then
                        _State.Result = SchedulerResultEnum.DailyScheduleLockedDay
                    ElseIf oEmployeeState.Result = EmployeeResultEnum.DailyScheduleCoverageDay Then
                        _State.Result = SchedulerResultEnum.DailyScheduleCoverageDay
                    Else
                        If bolRet Then
                            _State.Result = SchedulerResultEnum.NoError
                            _State.Language.ClearUserTokens()
                            _State.Language.AddUserToken(iTotalDaysPlanified.ToString)
                            _State.Language.AddUserToken(lEmployeesPlanified.Count.ToString)
                            _State.ResultDetail = _State.Language.Translate("SchedulingActionStats", "")
                        Else
                            _State.Result = SchedulerResultEnum.Exception
                            _State.IDPassport = oEmployeeState.IDPassport
                            _State.Context = oEmployeeState.Context
                            _State.ClientAddress = oEmployeeState.ClientAddress
                            _State.SessionID = oEmployeeState.SessionID
                            _State.ErrorText = oEmployeeState.ErrorText
                            _State.ErrorDetail = oEmployeeState.ErrorDetail
                            _State.ErrorNumber = oEmployeeState.ErrorNumber
                        End If
                    End If

                    If bolNotify And bolRet And _State.Result = SchedulerResultEnum.NoError Then
                        ' Notificamos el cambio
                        roConnector.InitTask(TasksType.DAILYSCHEDULE)
                    End If
                Else
                    ' No se han podido recuperar los empleados
                    _State.Result = SchedulerResultEnum.NoEmployeesAffected
                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roScheduleTemplate::AssignTemplatev2")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roScheduleTemplate::AssignTemplatev2")
            Finally

            End Try

            Return bolRet

        End Function

#End Region

#End Region

    End Class

    <DataContract()>
    Public Class roScheduleTemplateDate

#Region "Declarations - Constructor"

        Private xScheduleDate As Date
        Private strDescription As String

        Public Sub New()
            Me.strDescription = ""
        End Sub

        Public Sub New(ByVal _ScheduleDate As Date, ByVal _Description As String)
            Me.xScheduleDate = _ScheduleDate
            Me.strDescription = _Description
        End Sub

#End Region

#Region "Properties"

        <DataMember()>
        Public Property ScheduleDate() As Date
            Get
                Return Me.xScheduleDate
            End Get
            Set(ByVal value As Date)
                Me.xScheduleDate = value
            End Set
        End Property
        <DataMember()>
        Public Property Description() As String
            Get
                Return Me.strDescription
            End Get
            Set(ByVal value As String)
                Me.strDescription = value
            End Set
        End Property

#End Region

    End Class

End Namespace