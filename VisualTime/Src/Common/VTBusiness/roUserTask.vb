Imports System.Data.Common
Imports System.Runtime.Serialization
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTEmployees
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.Security
Imports Robotics.Security.Base
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions

Namespace UserTask

    Public Enum TaskType
        <EnumMember> UserTaskRepair = 9
        <EnumMember> UserTaskRepairTransitory = 8
        <EnumMember> UserTaskReview = 1
    End Enum

    Public Enum TaskCompletedState
        <EnumMember> All
        <EnumMember> Completed
        <EnumMember> NoCompleted
    End Enum

    <DataContract>
    Public Class roUserTask

        Public Const roUserTaskObject As String = "USERTASK"
        Public Const NO_EMPLOYEE_TASK As String = "NO_EMPLOYEE"
        Public Const BAD_ENTRIES_TASK As String = "BAD_ENTRIES"

#Region "Declarations - Constructor"

        Private oState As roUserTaskState

        Private strID As String
        Private xDateCreated As Date
        Private xDateCompleted As Nullable(Of Date)
        Private strSource As String = ""
        Private oTaskType As TaskType
        Private strMessage As String
        Private strResolverURL As String
        Private strResolverVariable1 As String
        Private strResolverValue1 As String
        Private strResolverVariable2 As String
        Private strResolverValue2 As String
        Private strResolverVariable3 As String
        Private strResolverValue3 As String
        Private strSecurityFlags As String = ""

        Public Sub New()
            Me.oState = New roUserTaskState(-1)
            Me.strID = ""
        End Sub

        Public Sub New(ByVal _State As roUserTaskState)
            Me.oState = _State
            Me.strID = ""
        End Sub

        Public Sub New(ByVal _ID As String, ByVal _State As roUserTaskState, Optional ByVal bAudit As Boolean = False)
            Me.oState = _State
            Me.strID = _ID
            Me.Load(bAudit)
        End Sub

#End Region

#Region "Properties"

        <IgnoreDataMember>
        Public Property State() As roUserTaskState
            Get
                Return Me.oState
            End Get
            Set(ByVal value As roUserTaskState)
                Me.oState = value
            End Set
        End Property

        <DataMember>
        Public Property ID() As String
            Get
                Return Me.strID
            End Get
            Set(ByVal value As String)
                Me.strID = value
            End Set
        End Property

        <DataMember>
        Public Property DateCreated() As Date
            Get
                Return Me.xDateCreated
            End Get
            Set(ByVal value As Date)
                Me.xDateCreated = value
            End Set
        End Property

        <DataMember>
        Public Property DateCompleted() As Nullable(Of Date)
            Get
                Return Me.xDateCompleted
            End Get
            Set(ByVal value As Nullable(Of Date))
                Me.xDateCompleted = value
            End Set
        End Property

        <DataMember>
        Public Property Source() As String
            Get
                Return Me.strSource
            End Get
            Set(ByVal value As String)
                Me.strSource = value
            End Set
        End Property

        <DataMember>
        Public Property TaskType() As TaskType
            Get
                Return Me.oTaskType
            End Get
            Set(ByVal value As TaskType)
                Me.oTaskType = value
            End Set
        End Property

        <DataMember>
        Public Property Message() As String
            Get
                Return Me.strMessage
            End Get
            Set(ByVal value As String)
                Me.strMessage = value
            End Set
        End Property

        <DataMember>
        Public Property ResolverURL() As String
            Get
                Return Me.strResolverURL
            End Get
            Set(ByVal value As String)
                Me.strResolverURL = value
            End Set
        End Property

        <DataMember>
        Public Property ResolverVariable1() As String
            Get
                Return Me.strResolverVariable1
            End Get
            Set(ByVal value As String)
                Me.strResolverVariable1 = value
            End Set
        End Property

        <DataMember>
        Public Property ResolverValue1() As String
            Get
                Return Me.strResolverValue1
            End Get
            Set(ByVal value As String)
                Me.strResolverValue1 = value
            End Set
        End Property

        <DataMember>
        Public Property ResolverVariable2() As String
            Get
                Return Me.strResolverVariable2
            End Get
            Set(ByVal value As String)
                Me.strResolverVariable2 = value
            End Set
        End Property

        <DataMember>
        Public Property ResolverValue2() As String
            Get
                Return Me.strResolverValue2
            End Get
            Set(ByVal value As String)
                Me.strResolverValue2 = value
            End Set
        End Property

        <DataMember>
        Public Property ResolverVariable3() As String
            Get
                Return Me.strResolverVariable3
            End Get
            Set(ByVal value As String)
                Me.strResolverVariable3 = value
            End Set
        End Property

        <DataMember>
        Public Property ResolverValue3() As String
            Get
                Return Me.strResolverValue3
            End Get
            Set(ByVal value As String)
                Me.strResolverValue3 = value
            End Set
        End Property

#End Region

#Region "Methods"

        Public Sub Load(Optional ByVal bAudit As Boolean = False)

            Me.xDateCreated = Now.Date
            Me.xDateCompleted = Nothing
            Me.strSource = ""
            Me.oTaskType = UserTask.TaskType.UserTaskRepair
            Me.strMessage = ""
            Me.strResolverURL = ""
            Me.strResolverVariable1 = ""
            Me.strResolverValue1 = ""
            Me.strResolverVariable2 = ""
            Me.strResolverValue2 = ""
            Me.strResolverVariable3 = ""
            Me.strResolverValue3 = ""
            Me.strSecurityFlags = "1111111111111111111111111111111111111111"

            Try

                Dim tb As DataTable = CreateDataTable("@SELECT# * FROM sysroUserTasks WHERE [ID] = '" & Me.ID & "'")
                If tb.Rows.Count > 0 Then

                    Me.xDateCreated = tb.Rows(0).Item("DateCreated")
                    If Not IsDBNull(tb.Rows(0).Item("DateCompleted")) Then Me.xDateCompleted = tb.Rows(0).Item("DateCompleted")
                    If Not IsDBNull(tb.Rows(0).Item("Source")) Then Me.strSource = tb.Rows(0).Item("Source")
                    Me.oTaskType = tb.Rows(0).Item("TaskType")
                    Me.strMessage = tb.Rows(0).Item("Message")
                    If Not IsDBNull(tb.Rows(0).Item("ResolverURL")) Then Me.strResolverURL = tb.Rows(0).Item("ResolverURL")
                    If Not IsDBNull(tb.Rows(0).Item("ResolverVariable1")) Then Me.strResolverVariable1 = tb.Rows(0).Item("ResolverVariable1")
                    If Not IsDBNull(tb.Rows(0).Item("ResolverValue1")) Then Me.strResolverValue1 = tb.Rows(0).Item("ResolverValue1")
                    If Not IsDBNull(tb.Rows(0).Item("ResolverVariable2")) Then Me.strResolverVariable2 = tb.Rows(0).Item("ResolverVariable2")
                    If Not IsDBNull(tb.Rows(0).Item("ResolverValue2")) Then Me.strResolverValue2 = tb.Rows(0).Item("ResolverValue2")
                    If Not IsDBNull(tb.Rows(0).Item("ResolverVariable3")) Then Me.strResolverVariable3 = tb.Rows(0).Item("ResolverVariable3")
                    If Not IsDBNull(tb.Rows(0).Item("ResolverValue3")) Then Me.strResolverValue3 = tb.Rows(0).Item("ResolverValue3")
                    Me.strSecurityFlags = tb.Rows(0).Item("SecurityFlags")

                    ' Auditamos consulta tarea usuario
                    If bAudit Then
                        Dim tbParameters As DataTable = oState.CreateAuditParameters()
                        oState.AddAuditParameter(tbParameters, "{UserTaskMessage}", Me.Message, "", 1)
                        oState.Audit(Audit.Action.aSelect, Audit.ObjectType.tUserTask, Me.Message, tbParameters, -1)
                    End If

                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roUserTask::Load")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roUserTask::Load")
            End Try

        End Sub

        Public Function Save(Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Try
                Dim strQueryRow As String = ""
                Dim oUserTaskOld As DataRow = Nothing
                Dim oUserTaskNew As DataRow = Nothing

                strQueryRow = "@SELECT# * " &
                              "FROM sysroUserTasks WHERE [ID] = '" & Me.strID & "'"
                Dim tbAuditOld As DataTable = CreateDataTable(strQueryRow, "sysroUserTasks")
                If tbAuditOld.Rows.Count = 1 Then oUserTaskOld = tbAuditOld.Rows(0)

                Dim tbUserTask As New DataTable("sysroUserTasks")
                Dim strSQL As String = "@SELECT# * FROM sysroUserTasks WHERE [ID] = '" & Me.strID & "'"
                Dim cmd As DbCommand = CreateCommand(strSQL)
                Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)
                da.Fill(tbUserTask)

                Dim oRow As DataRow = Nothing
                If tbUserTask.Rows.Count = 0 Then
                    oRow = tbUserTask.NewRow
                    oRow("ID") = Me.strID ' "USERTASK:\\NO_EMPLOYEE"
                    tbUserTask.Rows.Add(oRow)
                ElseIf tbUserTask.Rows.Count = 1 Then
                    oRow = tbUserTask.Rows(0)
                End If

                oRow("DateCreated") = Me.xDateCreated
                If Me.xDateCompleted.HasValue Then
                    oRow("DateCompleted") = Me.xDateCompleted.Value
                Else
                    oRow("datecompleted") = DBNull.Value
                End If
                oRow("Source") = Me.strSource
                oRow("TaskType") = Me.oTaskType
                oRow("Message") = Me.strMessage
                oRow("ResolverURL") = Me.strResolverURL
                oRow("ResolverVariable1") = Me.strResolverVariable1
                oRow("ResolverValue1") = Me.strResolverValue1
                oRow("ResolverVariable2") = Me.strResolverVariable2
                oRow("ResolverValue2") = Me.strResolverValue2
                oRow("ResolverVariable3") = Me.strResolverVariable3
                oRow("ResolverValue3") = Me.strResolverValue3
                oRow("SecurityFlags") = Me.strSecurityFlags

                da.Update(tbUserTask)

                If bAudit Then
                    strQueryRow = "@SELECT# * " &
                                  "FROM sysroUserTasks WHERE [ID] = '" & Me.strID & "'"
                    Dim tbAuditNew As DataTable = CreateDataTable(strQueryRow, "sysroUserTasks")
                    If tbAuditNew.Rows.Count = 1 Then oUserTaskNew = tbAuditNew.Rows(0)

                    ' Insertar registro auditoria
                    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    Dim oAuditAction As Audit.Action = IIf(oUserTaskOld Is Nothing, Audit.Action.aInsert, Audit.Action.aUpdate)
                    oState.AddAuditFieldsValues(tbParameters, oUserTaskNew, oUserTaskOld)
                    Dim strObjectName As String
                    If oAuditAction = Audit.Action.aInsert Then
                        strObjectName = oUserTaskNew("Message")
                    ElseIf oUserTaskOld("Message") <> oUserTaskNew("Message") Then
                        strObjectName = oUserTaskOld("Message") & " -> " & oUserTaskNew("Message")
                    Else
                        strObjectName = oUserTaskNew("Message")
                    End If
                    oState.Audit(oAuditAction, Audit.ObjectType.tUserTask, strObjectName, tbParameters, -1)
                End If

                bolRet = True
            Catch Ex As DbException
                Me.oState.UpdateStateInfo(Ex, "roUserTask::Save")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roUserTask::Save")
            End Try

            Return bolRet

        End Function

        Public Function Delete(Optional ByVal bAudit As Boolean = False) As Boolean
            Dim bolRet As Boolean = False
            Me.oState.UpdateStateInfo()

            Try

                Dim strSQL As String

                ' Miramos si existe o no para auditar borrado
                Dim bolAudit As Boolean = False
                strSQL = "@SELECT# [ID] FROM sysroUserTasks " &
                         "WHERE [ID] = '" & Me.strID & "'"
                Dim tb As DataTable = CreateDataTable(strSQL)
                If tb IsNot Nothing Then bolAudit = (tb.Rows.Count > 0)

                strSQL = "@DELETE# FROM sysroUserTasks " &
                         "WHERE [ID] = '" & Me.strID & "'"
                If Not ExecuteSql(strSQL) Then
                    Me.oState.Result = UserTaskResultEnum.ConnectionError
                End If

                If bolAudit And bAudit And Me.oState.Result = UserTaskResultEnum.NoError Then
                    ' Auditar borrado
                    Me.oState.Audit(Audit.Action.aDelete, Audit.ObjectType.tUserTask, Me.Message, Nothing, -1)
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roUserTask::Delete")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roUserTask::Delete")
            End Try

            Return bolRet

        End Function

#Region "Helper methods"

        Public Shared Function GetUserTasks(ByVal _TaskType As TaskType, ByVal _TaskCompleted As TaskCompletedState, ByVal _State As roUserTaskState) As DataTable

            Dim oRet As DataTable = Nothing

            Try

                Dim strSQL As String = "@SELECT# * FROM sysroUserTasks " &
                                       "WHERE TaskType = " & _TaskType ' & " OR TaskType = " & TaskType.UserTaskReview

                Select Case _TaskCompleted
                    Case TaskCompletedState.Completed
                        strSQL &= " AND DateCompleted <= " & roTypes.Any2Time(Now.Date).SQLSmallDateTime
                    Case TaskCompletedState.NoCompleted
                        strSQL &= " AND (DateCompleted IS NULL OR DateCompleted > " & roTypes.Any2Time(Now.Date).SQLSmallDateTime & ")"
                End Select

                Dim oLicense As New roServerLicense
                Dim bolLicenseHRScheduling = oLicense.FeatureIsInstalled("Feature\HRScheduling")

                Dim oLanguage As roLanguage = GetLanguage(_State)

                If bolLicenseHRScheduling Then

                    Dim strEmployee As String = oLanguage.Translate("UserTasksCheck.EmployeeAbsenceCoverage", "").Replace("'", "''")
                    Dim strGroup As String = oLanguage.Translate("UserTasksCheck.GroupNegativeCoverage", "").Replace("'", "''")
                    Dim strAssignment As String = oLanguage.Translate("UserTasksCheck.AssignmentName", "").Replace("'", "''")
                    Dim strDayCoverage As String = oLanguage.Translate("UserTasksCheck.DayCoverage", "").Replace("'", "''")

                    strSQL &= " UNION "

                    strSQL &= " @SELECT#  Key3DateTime as DateCreated , null, ''," & _TaskType & ","
                    'strSQL &= " 'Message',"
                    strSQL &= " CASE IDType WHEN 14 THEN '" & strGroup & " ' + (@SELECT# dbo.GetFullGroupPathName(dbo.Groups.ID) as Name FROM Groups WHERE Groups.ID=Key1Numeric) + ' " & strDayCoverage & " ' + convert(nvarchar(10), datepart(day,Key3DateTime)) + '/' + convert(nvarchar(10), datepart(month,Key3DateTime)) + '/' + convert(nvarchar(10), datepart(year,Key3DateTime)) + ' " & strAssignment & " ' + (@SELECT# Name FROM Assignments WHERE Assignments.ID=Key2Numeric)   ELSE "
                    strSQL &= " '" & strEmployee & " ' + (@SELECT# Name FROM Employees WHERE Employees.ID=Key1Numeric)  + ' " & strDayCoverage & " ' + convert(nvarchar(10), datepart(day,Key3DateTime)) + '/' + convert(nvarchar(10), datepart(month,Key3DateTime)) + '/' + convert(nvarchar(10), datepart(year,Key3DateTime))  END , "
                    strSQL &= " CASE IDType WHEN 14 THEN 'FN:\\Resolver_Coverage' ELSE 'FN:\\Resolver_Absence' END , "
                    strSQL &= " 'Date',convert(nvarchar(500), Key3DateTime, 120), CASE IDType WHEN 13 THEN 'IDEmployee' ELSE 'IDGroup' END ,"
                    strSQL &= " convert(nvarchar(500),Key1Numeric), 'IDAssignment',convert(nvarchar(500),Key2Numeric), '', 'USERTASK:\\COVERAGE' + convert(nvarchar(500),sysroNotificationTasks.ID)"
                    strSQL &= " FROM sysroNotificationTasks, Notifications WHERE sysroNotificationTasks.IDNotification = Notifications.ID "
                    strSQL &= " AND IDType IN(13, 14) AND Activated=1 AND Key3DateTime >= " & roTypes.Any2Time(Now.Date).SQLDateTime & " AND Key3DateTime <= " & roTypes.Any2Time(Now.Date).Add(3, "d").SQLDateTime
                    strSQL &= " AND CASE IDType WHEN 14 THEN (@SELECT# dbo.GetFullGroupPathName(dbo.Groups.ID) as Name FROM Groups WHERE Groups.ID=Key1Numeric) + (@SELECT# Name FROM Assignments WHERE Assignments.ID=Key2Numeric) ELSE (@SELECT# Name FROM Employees WHERE Employees.ID=Key1Numeric)  END Is not null"

                End If

                strSQL &= " ORDER BY DateCreated"

                oRet = CreateDataTable(strSQL, )

                If oRet IsNot Nothing Then
                    oRet.Columns.Add(New DataColumn("MessageEx", GetType(String)))
                    For Each oDataRow As DataRow In oRet.Rows
                        Dim strMsg As String = String.Empty
                        Dim bolPermissions As Boolean = False
                        oLanguage.ClearUserTokens()
                        oDataRow("MessageEx") = roTypes.Any2String(oDataRow("Message"))

                        Select Case roTypes.Any2String(oDataRow("ResolverURL"))
                            Case "FN:\\Resolver_MovesInvalidCardID"
                                bolPermissions = WLHelper.HasPermissionOverFeature(_State.IDPassport, "Employees.IdentifyMethods", "U", Permission.Write)

                                If bolPermissions Then
                                    strMsg = oLanguage.Translate("NoEmployeeFound.Task", "")
                                Else
                                    oDataRow.Delete()
                                End If

                            Case "FN:\\Resolver_Over_MaxJobEmployees_Soon"
                                bolPermissions = WLHelper.HasPermissionOverFeature(_State.IDPassport, "Administration.Security", "U", Permission.Write)

                                If bolPermissions Then
                                    oLanguage.AddUserToken(roTypes.Any2String(oDataRow("ResolverValue1")))
                                    oLanguage.AddUserToken(roTypes.Any2String(oDataRow("ResolverValue2")))
                                    oLanguage.AddUserToken(roTypes.Any2String(oDataRow("ResolverValue3")))
                                    strMsg = oLanguage.Translate("OverMaxJobEmployeesSoon.Title", "")
                                Else
                                    oDataRow.Delete()
                                End If

                            Case "FN:\\Resolver_Over_MaxEmployees_Soon"
                                bolPermissions = WLHelper.HasPermissionOverFeature(_State.IDPassport, "Administration.Security", "U", Permission.Write)

                                If bolPermissions Then
                                    oLanguage.AddUserToken(roTypes.Any2String(oDataRow("ResolverValue1")))
                                    oLanguage.AddUserToken(roTypes.Any2String(oDataRow("ResolverValue2")))
                                    oLanguage.AddUserToken(roTypes.Any2String(oDataRow("ResolverValue3")))
                                    strMsg = oLanguage.Translate("OverMaxEmployeesSoon.Title", "")
                                Else
                                    oDataRow.Delete()
                                End If

                            Case "FN:\\Resolver_Terminal_Unrecognized"
                                bolPermissions = WLHelper.HasPermissionOverFeature(_State.IDPassport, "Terminals.Definition", "U", Permission.Write)

                                If bolPermissions Then
                                    oLanguage.AddUserToken(roTypes.Any2String(oDataRow("ResolverValue1")))
                                    oLanguage.AddUserToken(roTypes.Any2String(oDataRow("ResolverValue2")))
                                    oLanguage.AddUserToken(roTypes.Any2String(oDataRow("ResolverValue3")))

                                    strMsg = oLanguage.Translate("TerminalUnrecognized.Title", "")
                                Else
                                    oDataRow.Delete()
                                End If

                            Case "FN:\\mxC_NotRegistered"
                                bolPermissions = WLHelper.HasPermissionOverFeature(_State.IDPassport, "Terminals.Definition", "U", Permission.Write)

                                If bolPermissions Then
                                    oLanguage.AddUserToken(roTypes.Any2String(oDataRow("ResolverValue1")))
                                    oLanguage.AddUserToken(roTypes.Any2String(oDataRow("ResolverValue2")))
                                    oLanguage.AddUserToken(roTypes.Any2String(oDataRow("ResolverValue3")))

                                    strMsg = oLanguage.Translate("TerminalNotRegistered.Title", "")
                                Else
                                    oDataRow.Delete()
                                End If

                            Case "FN:\\Resolver_ParserInvalidEntries"
                                bolPermissions = WLHelper.HasPermissionOverFeature(_State.IDPassport, "Calendar.Punches", "U", Permission.Write)

                                If bolPermissions Then
                                    strMsg = oLanguage.Translate("Collector.Process.AddBadLine", "")
                                Else
                                    oDataRow.Delete()
                                End If

                            Case "FN:\\Resolver_CloseDateAlert"
                                bolPermissions = WLHelper.HasPermissionOverFeature(_State.IDPassport, "Administration.Security", "U", Permission.Write)

                                If bolPermissions Then
                                    strMsg = oLanguage.Translate("CloseDate.Expired", "")
                                Else
                                    oDataRow.Delete()
                                End If

                            Case "FN:\\Resolver_TerminalDisconnected"
                                bolPermissions = WLHelper.HasPermissionOverFeature(_State.IDPassport, "Terminals.Definition", "U", Permission.Read) AndAlso WLHelper.HasPermissionOverFeature(_State.IDPassport, "Terminals.StatusInfo", "U", Permission.Read)

                                If bolPermissions Then
                                    ' Terminal
                                    oLanguage.AddUserToken(roTypes.Any2String(oDataRow("ResolverValue1")))
                                    ' Fecha
                                    oLanguage.AddUserToken(roTypes.Any2String(oDataRow("ResolverValue2")))
                                    strMsg = oLanguage.Translate("Notification.TerminalDisconnected.Body.Text", "")
                                Else
                                    oDataRow.Delete()
                                End If

                            Case "FN:\\Resolver_invalidemailsettings"
                                bolPermissions = WLHelper.HasPermissionOverFeature(_State.IDPassport, "Administration.Options.General", "U", Permission.Write)
                                ' Codigo error
                                If bolPermissions Then
                                    oLanguage.AddUserToken(roTypes.Any2String(oDataRow("ResolverValue1")))
                                    strMsg = oLanguage.Translate("Configuration.InvalidEmailSettings", "")
                                Else
                                    oDataRow.Delete()
                                End If

                            Case "FN:\\TERMINAL_NotRegistered"
                                bolPermissions = WLHelper.HasPermissionOverFeature(_State.IDPassport, "Terminals.Definition", "U", Permission.Write)

                                If bolPermissions Then
                                    oLanguage.AddUserToken(roTypes.Any2String(oDataRow("ResolverValue1")))
                                    oLanguage.AddUserToken(roTypes.Any2String(oDataRow("ResolverValue2")))
                                    oLanguage.AddUserToken(roTypes.Any2String(oDataRow("ResolverValue3")))

                                    strMsg = oLanguage.Translate("TerminalNotRegisteredEx.Title", "")
                                Else
                                    oDataRow.Delete()
                                End If

                            Case Else
                                bolPermissions = WLHelper.HasPermissionOverFeature(_State.IDPassport, "Administration", "U", Permission.Write)
                                If bolPermissions Then
                                    ' Genérico. En Message encuentro el tag genérico de lenguaje, que incluye los parámetros si los hay
                                    Dim sLangTag As String = roTypes.Any2String(oDataRow("Message"))
                                    For i As Integer = 1 To sLangTag.Split("¬").Count
                                        If i > 1 Then oLanguage.AddUserToken(sLangTag.Split("¬")(i - 1))
                                    Next
                                    strMsg = oLanguage.Translate(sLangTag.Split("¬")(0), "")
                                Else
                                    oDataRow.Delete()
                                End If

                        End Select

                        If bolPermissions AndAlso strMsg.Length > 0 AndAlso strMsg <> "NotFound" Then
                            oDataRow("MessageEx") = strMsg
                        End If

                    Next
                End If
                oRet.AcceptChanges()

                If bolLicenseHRScheduling Then
                    ' Miramos si el passport actual es de tipo empleado o no, para establecer los permisos correspondientes ('U' o 'E').
                    Dim bolPassportEmployee As Boolean = (_State.ActivePassportType() = "E")

                    For Each oDataRow As DataRow In oRet.Rows
                        Dim bolPermission As Boolean = False
                        If roTypes.Any2String(oDataRow("ResolverVariable2")) = "IDEmployee" Then
                            Dim oStateEmp As New Employee.roEmployeeState(_State.IDPassport)
                            Dim tbEmployeeMobilities As DataTable

                            tbEmployeeMobilities = Employee.roMobility.GetMobilities(oDataRow("ResolverValue2"), oStateEmp)
                            If tbEmployeeMobilities IsNot Nothing Then
                                For Each oMobility As DataRow In tbEmployeeMobilities.Rows
                                    If roTypes.Any2Time(oMobility("BeginDate")).NumericValue <= roTypes.Any2Time(oDataRow("ResolverValue1")).NumericValue And roTypes.Any2Time(oMobility("EndDate")).NumericValue >= roTypes.Any2Time(oDataRow("ResolverValue1")).NumericValue Then
                                        ' verificamos los permisos sobre el grupo
                                        Dim oPermission As Permission = Permission.Write
                                        If WLHelper.HasFeaturePermissionByGroup(_State.IDPassport, "Calendar.Scheduler", oPermission, oMobility("IDGroup"), "U") Then
                                            bolPermission = True
                                        End If
                                        Exit For
                                    End If
                                Next
                            End If

                            If bolPermission Then
                                ' Verificamos los permisos del pasaporte actual sobre la planificación
                                bolPermission = WLHelper.HasFeaturePermissionByEmployee(_State.IDPassport, "Calendar.Scheduler", Permission.Write, oDataRow("ResolverValue2"), )
                            End If
                            If Not bolPermission Then
                                oDataRow.Delete()
                            End If
                        ElseIf roTypes.Any2String(oDataRow("ResolverVariable2")) = "IDGroup" Then
                            Dim oPermission As Permission = Permission.Write
                            'Employees.UserFields.Information.High
                            If Not WLHelper.HasFeaturePermissionByGroup(_State.IDPassport, "Calendar.Scheduler", oPermission, oDataRow("ResolverValue2"), "U") Then
                                oDataRow.Delete()
                            End If
                        End If

                    Next
                    oRet.AcceptChanges()
                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roUserTask::GetUserTasks")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roUserTask::GetUserTasks")
            Finally

            End Try

            Return oRet

        End Function

        Private Shared Function GetLanguage(ByVal oState As roUserTaskState) As roLanguage
            Dim oRet As New roLanguage
            Dim strLanguageKey As String = ""
            Dim oPassport As roPassportTicket = roPassportManager.GetPassportTicket(oState.IDPassport, LoadType.Passport)
            If oPassport IsNot Nothing Then strLanguageKey = oPassport.Language.Key
            If strLanguageKey = "" Then
                Dim oSettings As New roSettings()
                strLanguageKey = oSettings.GetVTSetting(eKeys.DefaultLanguage)
            End If
            oRet.SetLanguageReference("LivePortal", strLanguageKey)
            Return oRet
        End Function

#End Region

#End Region

    End Class

End Namespace