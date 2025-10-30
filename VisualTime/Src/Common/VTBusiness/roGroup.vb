Imports System.Data.Common
Imports System.Drawing
Imports System.Runtime.Serialization
Imports System.Text
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTBusiness.Support
Imports Robotics.Base.VTEmployees
Imports Robotics.DataLayer
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.Security
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions
Imports Robotics.VTBase.Extensions.VTLiveTasks
Imports Robotics.VTBase.roTypes
Imports Robotics.Security.Base


Namespace Group

    <DataContract>
    Public Class roGroup

#Region "Declarations - Constructor"

        Private oState As roGroupState

        Private intID As Integer
        Private strName As String
        Private strPath As String
        Private strSecurityFlags As String
        Private strDescriptionGroup As String
        Private strExport As String

        Private intIDZoneWorkingTime As Nullable(Of Integer)
        Private intIDZoneNonWorkingTime As Nullable(Of Integer)

        Private strFullGroupName As String

        Private bolMultiCompanyLicense As Boolean = False

        Private intIDCenter As Nullable(Of Integer)

        Private xCloseDate As Nullable(Of DateTime)

        Private _bAudit As Boolean = False

        Public Sub New()
            Me.oState = New roGroupState
            Me.ID = 0
        End Sub

        Public Sub New(ByVal _State As roGroupState, Optional ByVal bAudit As Boolean = False)
            Me.oState = _State
            _bAudit = bAudit
            Me.ID = 0
            Me.Load(_bAudit)
        End Sub

        Public Sub New(ByVal _IDGroup As Integer, ByVal _State As roGroupState, Optional ByVal bAudit As Boolean = False)
            Me.oState = _State
            _bAudit = bAudit
            Me.ID = _IDGroup
            Me.Load(_bAudit)
        End Sub

#End Region

#Region "Properties"

        <DataMember>
        Public Property ID() As Integer
            Get
                Return intID
            End Get
            Set(ByVal value As Integer)
                intID = value
                'Me.Load(_bAudit)
            End Set
        End Property

        <DataMember>
        Public Property Name() As String
            Get
                Return strName
            End Get
            Set(ByVal value As String)
                strName = value
            End Set
        End Property

        <DataMember>
        Public Property Path() As String
            Get
                Return strPath
            End Get
            Set(ByVal value As String)
                strPath = value
            End Set
        End Property

        <DataMember>
        Public Property SecurityFlags() As String
            Get
                Return strSecurityFlags
            End Get
            Set(ByVal value As String)
                strSecurityFlags = value
            End Set
        End Property

        <DataMember>
        Public Property FullGroupName() As String
            Get
                Return Me.strFullGroupName
            End Get
            Set(ByVal value As String)
                Me.strFullGroupName = value
            End Set
        End Property

        <DataMember>
        Public Property DescriptionGroup() As String
            Get
                Return Me.strDescriptionGroup
            End Get
            Set(ByVal value As String)
                Me.strDescriptionGroup = value
            End Set
        End Property

        <DataMember>
        Public Property Export() As String
            Get
                Return Me.strExport
            End Get
            Set(ByVal value As String)
                Me.strExport = value
            End Set
        End Property

        <DataMember>
        Public Property IDZoneWorkingTime() As Nullable(Of Integer)
            Get
                Return intIDZoneWorkingTime
            End Get
            Set(ByVal value As Nullable(Of Integer))
                intIDZoneWorkingTime = value
            End Set
        End Property

        <DataMember>
        Public Property IDCenter() As Nullable(Of Integer)
            Get
                Return intIDCenter
            End Get
            Set(ByVal value As Nullable(Of Integer))
                intIDCenter = value
            End Set
        End Property

        <DataMember>
        Public Property CloseDate() As Nullable(Of DateTime)
            Get
                Return xCloseDate
            End Get
            Set(ByVal value As Nullable(Of DateTime))
                xCloseDate = value
            End Set
        End Property

        <DataMember>
        Public Property IDZoneNoneWorkingTime() As Nullable(Of Integer)
            Get
                Return intIDZoneNonWorkingTime
            End Get
            Set(ByVal value As Nullable(Of Integer))
                intIDZoneNonWorkingTime = value
            End Set
        End Property

#End Region

#Region "Methods"

        Public Sub Load(Optional ByVal bAudit As Boolean = False)

            Dim oLicense As New roServerLicense
            Me.bolMultiCompanyLicense = oLicense.FeatureIsInstalled("Feature\MultiCompany")

            If Me.intID <= 0 Then

                Me.strName = ""
                Me.strPath = ""
                Me.strSecurityFlags = ""
                Me.strDescriptionGroup = ""
                Me.strExport = ""
            Else

                Try

                    Dim tb As DataTable = CreateDataTable("@SELECT# *, dbo.GetFullGroupPathName(ID) AS FullGroupName FROM Groups WHERE [ID] = " & Me.ID.ToString)
                    If tb.Rows.Count > 0 Then

                        Me.strName = tb.Rows(0).Item("Name")
                        Me.strPath = tb.Rows(0).Item("Path")
                        Me.strSecurityFlags = Any2String(tb.Rows(0).Item("SecurityFlags"))
                        Me.strFullGroupName = Any2String(tb.Rows(0).Item("FullGroupName"))
                        Me.strDescriptionGroup = Any2String(tb.Rows(0).Item("DescriptionGroup"))
                        Me.strExport = Any2String(tb.Rows(0).Item("Export"))

                        If tb.Rows(0).Item("IDZoneWorkingTime") IsNot DBNull.Value Then
                            Me.intIDZoneWorkingTime = roTypes.Any2Integer(tb.Rows(0).Item("IDZoneWorkingTime"))
                        Else
                            Me.intIDZoneWorkingTime = Nothing
                        End If

                        If tb.Rows(0).Item("IDCenter") IsNot DBNull.Value Then
                            Me.intIDCenter = roTypes.Any2Integer(tb.Rows(0).Item("IDCenter"))
                        Else
                            Me.intIDCenter = Nothing
                        End If

                        If tb.Rows(0).Item("CloseDate") IsNot DBNull.Value Then
                            Me.xCloseDate = roTypes.Any2DateTime(tb.Rows(0).Item("CloseDate"))
                        Else
                            Me.xCloseDate = Nothing
                        End If

                        If tb.Rows(0).Item("IDZoneNonWorkingTime") IsNot DBNull.Value Then
                            Me.intIDZoneNonWorkingTime = roTypes.Any2Integer(tb.Rows(0).Item("IDZoneNonWorkingTime"))
                        Else
                            Me.intIDZoneNonWorkingTime = Nothing
                        End If

                        ' Auditamos consulta grupo
                        If bAudit Then
                            Dim tbParameters As DataTable = oState.CreateAuditParameters()
                            oState.AddAuditParameter(tbParameters, "{GroupName}", Me.Name, "", 1)
                            oState.Audit(Audit.Action.aSelect, Audit.ObjectType.tGroup, Me.Name, tbParameters, -1)
                        End If

                    End If
                Catch ex As DbException
                    Me.oState.UpdateStateInfo(ex, "roGroup:: Load")
                Catch ex As Exception
                    Me.oState.UpdateStateInfo(ex, "roGroup::Load")
                End Try

            End If

        End Sub

        Public Function Save(Optional ByVal bAudit As Boolean = False, Optional _State As roGroupState = Nothing) As Boolean
            If _State IsNot Nothing Then
                roBusinessState.CopyTo(_State, Me.oState)
            End If

            If Me.oState Is Nothing Then
                Me.oState = New roGroupState
            End If

            Dim bolRet As Boolean = False

            Try


                If Not DataLayer.roSupport.IsXSSSafe(Me) Then
                    Me.oState.Result = GroupResultEnum.XSSvalidationError
                    Return False
                End If


                ' Miro si debo permitir resticción de presencia antigua ...

                Dim bLegacyRestrictionModeAllowed As Boolean = False
                Try
                    Dim strSQLaux As String = String.Empty
                    strSQLaux = "@SELECT# SUM(total) total from (@SELECT# COUNT(*) total from TerminalReaderEmployees UNION @SELECT# COUNT(*) total from Groups where DescriptionGroup like '%TERMINAL=%') aux"
                    Dim tbControl As DataTable = Nothing
                    tbControl = CreateDataTable(strSQLaux, "Control")
                    If Not tbControl Is Nothing AndAlso tbControl.Rows.Count > 0 Then
                        bLegacyRestrictionModeAllowed = Any2Boolean(tbControl.Rows(0).Item("total"))
                    End If
                Catch ex As Exception
                End Try

                If Not bLegacyRestrictionModeAllowed Then
                    If Me.DescriptionGroup.Contains("TERMINAL=") Then
                        bolRet = False
                        _State.Result = GroupResultEnum.NoAttRestrictionAllowed
                        Exit Function
                    End If
                End If

                If Me.strExport IsNot Nothing AndAlso Me.strExport <> "" Then
                    Dim bUniqueExportName As Boolean = True
                    Try
                        Dim strSQLaux As String = String.Empty
                        strSQLaux = "@SELECT# * from Groups where Export = '" & Me.strExport.Replace("'", "''") & "' AND ID <> " & Me.intID
                        Dim tbControl As DataTable = Nothing
                        tbControl = CreateDataTable(strSQLaux, "Control")
                        If Not tbControl Is Nothing AndAlso tbControl.Rows.Count > 0 Then
                            bUniqueExportName = False
                        End If
                    Catch ex As Exception
                    End Try

                    If Not bUniqueExportName Then
                        bolRet = False
                        _State.Result = GroupResultEnum.ExportNameNotUnique
                        Exit Function
                    End If
                End If

                Dim strQueryRow As String = ""
                Dim oGroupOld As DataRow = Nothing
                Dim oGroupNew As DataRow = Nothing

                strQueryRow = "@SELECT# ID, Name, Path, SecurityFlags, DescriptionGroup, IDCenter FROM Groups WHERE [ID] = " & Me.intID
                Dim tbAuditOld As DataTable = CreateDataTable(strQueryRow, "Groups")
                If tbAuditOld.Rows.Count = 1 Then oGroupOld = tbAuditOld.Rows(0)

                Dim tbGroup As New DataTable("Groups")
                Dim strSQL As String = "@SELECT# * FROM Groups WHERE [ID] = " & Me.intID.ToString
                Dim cmd As DbCommand = CreateCommand(strSQL)
                Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)
                Dim OldIDCenter As String = ""

                da.Fill(tbGroup)

                Dim oRow As DataRow = Nothing
                If Me.intID <= 0 Then
                    oRow = tbGroup.NewRow
                    oRow("ID") = Me.GetNextIDGroup()
                    Me.strPath &= IIf(Me.strPath <> "", "\", "") & oRow("ID")
                ElseIf tbGroup.Rows.Count = 1 Then
                    oRow = tbGroup.Rows(0)
                    OldIDCenter = Any2String(oRow("IDCenter"))
                End If

                ' Path sanitize
                Dim strPathAux As String = String.Empty
                Try
                    strPathAux = String.Join("\", Me.strPath.Split("\").Distinct)
                    If strPathAux <> Me.strPath Then
                        Me.strPath = strPathAux
                        roLog.GetInstance().logMessage(roLog.EventType.roInfo, "roGroup::Save::Path sanitized from " & Me.strPath & " to " & strPathAux)
                    End If
                Finally
                End Try

                oRow("Name") = Me.strName.Replace(Chr(10), "").Replace(Chr(13), "")
                oRow("Path") = Me.strPath
                oRow("SecurityFlags") = Me.strSecurityFlags
                oRow("DescriptionGroup") = Me.strDescriptionGroup
                oRow("Export") = Me.strExport

                If Me.intIDZoneWorkingTime Is Nothing Then
                    oRow("IDZoneWorkingTime") = DBNull.Value
                Else
                    oRow("IDZoneWorkingTime") = Me.intIDZoneWorkingTime
                End If

                If Me.intIDZoneNonWorkingTime Is Nothing Then
                    oRow("IDZoneNonWorkingTime") = DBNull.Value
                Else
                    oRow("IDZoneNonWorkingTime") = Me.intIDZoneNonWorkingTime
                End If

                If Me.intIDCenter Is Nothing Then
                    oRow("IDCenter") = DBNull.Value
                Else
                    oRow("IDCenter") = Me.intIDCenter
                End If

                If Me.xCloseDate Is Nothing Then
                    oRow("CloseDate") = DBNull.Value
                Else
                    oRow("CloseDate") = Me.xCloseDate
                End If

                If Me.intID <= 0 Then
                    tbGroup.Rows.Add(oRow)
                End If

                da.Update(tbGroup)

                'Calcula el FullPathName de todos los grupos afectados
                Try
                    ExecuteSql("@UPDATE# groups set FullGroupName= dbo.GetFullGroupPathName(id) where ID = " & oRow("ID").ToString & " OR [Path] like '%\" & oRow("ID").ToString & "\%' OR [Path] like '" & oRow("ID").ToString & "\%'")
                Catch ex As Exception
                    Me.oState.UpdateStateInfo(ex, "roGroup:: Save()::FullPathNameChildren")
                    Try
                        ExecuteSql("@UPDATE# groups set FullGroupName='' where ID = " & oRow("ID").ToString & " OR [Path] like '%\" & oRow("ID").ToString & "\%'")
                    Catch ex2 As Exception
                        Me.oState.UpdateStateInfo(ex2, "roGroup::Save::FullPathNameChildren2")
                    End Try
                End Try

                If Me.intID <= 0 Then
                    Me.intID = oRow("ID")
                    Dim bolIsEnterprise As Boolean = False
                    Dim sCustomization As String = roBusinessSupport.GetCustomizationCode()

                    If Not Me.strPath.Contains("\") Then
                        ' Si creamos una empresa
                        ' hay que asignar la empresa al nodo raiz
                        ' y volver a mapear los permisos
                        bolIsEnterprise = True

                        Try
                            If sCustomization <> "CRUZROJA" Then
                                ' Asignamos la empresa a los supervisores del grupo administrador y a los de robotics
                                ExecuteSql("@INSERT# INTO sysroPassports_Groups(IDGroup, IDPassport) @SELECT# " & Me.intID.ToString & ",ID FROM sysroPassports WHERE IsSupervisor = 1 AND ( (IDGroupFeature =0) OR (IDGroupFeature IN(@SELECT# id from sysroGroupFeatures where Name like '%@@ROBOTICS@@%')) or (id=" & Me.oState.IDPassport.ToString & ") )")
                            Else
                                ' Asignamos la empresa al grupo de robotics
                                ExecuteSql("@INSERT# INTO sysroPassports_Groups(IDGroup, IDPassport) @SELECT# " & Me.intID.ToString & ",ID FROM sysroPassports WHERE IsSupervisor = 1 AND IDGroupFeature IN(@SELECT# id from sysroGroupFeatures where Name like '%@@ROBOTICS@@%')")
                            End If
                        Catch ex As Exception
                        End Try
                    End If

                    If bolIsEnterprise Then
                        ' En el caso que sea una empresa en modo V2 o v3
                        ' lanzamos la tarea de mapeo de permisos
                        If sCustomization <> "CRUZROJA" Then
                            Dim oStateTask As New roLiveTaskState(-1)

                            Dim oParameters As New roCollection
                            oParameters.Add("Context", 4)
                            oParameters.Add("IDContext", Me.intID)
                            oParameters.Add("Action", -1)

                            roLiveTask.CreateLiveTask(roLiveTaskTypes.SecurityPermissions, oParameters, oStateTask)
                            roLiveTask.CreateLiveTask(roLiveTaskTypes.GenerateRoboticsPermissions, oParameters, oStateTask)
                        End If
                    Else

                        ' Si se crea el grupo hay que crear los permisos sobre el
                        ExecuteSql("@INSERT# INTO RequestPassportPermissionsPending Values(3," & Me.intID & ")")

                        Dim oStateTask As New roLiveTaskState(-1)
                        Dim oParameters As New roCollection
                        oParameters.Add("Mode", "")
                        roLiveTask.CreateLiveTask(roLiveTaskTypes.ChangeRequestPermissions, oParameters, oStateTask)

                    End If
                Else
                    ' si se modifica un grupo revisamos si ha cambiado el centro de coste para recalcular
                    If OldIDCenter <> Any2String(Me.intIDCenter) Then
                        Me.Recalculate()
                    End If

                End If

                strQueryRow = "@SELECT# ID, Name, Path, SecurityFlags, DescriptionGroup, IDCenter " &
                                  "FROM Groups WHERE [ID] = " & Me.intID
                Dim tbAuditNew As DataTable = CreateDataTable(strQueryRow, "Groups")
                If tbAuditNew.Rows.Count = 1 Then oGroupNew = tbAuditNew.Rows(0)

                ' Insertar registro auditoria
                If bAudit Then
                    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    Dim oAuditAction As Audit.Action = IIf(oGroupOld Is Nothing, Audit.Action.aInsert, Audit.Action.aUpdate)
                    oState.AddAuditFieldsValues(tbParameters, oGroupNew, oGroupOld)
                    Dim strObjectName As String = ""
                    If oAuditAction = Audit.Action.aInsert Then
                        strObjectName = oGroupNew("Name")
                    ElseIf oGroupOld("Name") <> oGroupNew("Name") Then
                        strObjectName = oGroupOld("Name") & " -> " & oGroupNew("Name")
                    Else
                        strObjectName = oGroupNew("Name")
                    End If

                    If oAuditAction = Audit.Action.aUpdate Then
                        If Any2String(oGroupOld("IDCenter")) <> Any2String(oGroupNew("IDCenter")) Then
                            strObjectName &= Any2String(oGroupOld("IDCenter")) & " -> " & Any2String(oGroupNew("IDCenter"))
                        End If
                    End If
                    oState.Audit(oAuditAction, Audit.ObjectType.tGroup, strObjectName, tbParameters, -1)
                End If

                bolRet = True

                ' Miramos si hay la licencia de prevención de riesgos laborales
                ' Miramos si es un grupo de nivel zero (empresa)
                Dim oServerLicense As New roServerLicense
                If oServerLicense.FeatureIsInstalled("Feature\OHP") AndAlso Me.intID.ToString = Me.strPath Then

                    Dim bolBroadcaster As Boolean = False
                    ' Miramos si es necesario ejecutar el broadcaster
                    If oGroupOld Is Nothing OrElse oGroupOld("Name") <> oGroupNew("Name") Then
                        bolBroadcaster = True
                    End If

                    If bolBroadcaster Then
                        roConnector.InitTask(TasksType.BROADCASTER)
                    End If

                End If

                Try
                    ' Miramos si se ha cambiado la descripción del grupo, y esta contenía la palabra TERMINAL, dado que en
                    ' este caso se aplicará restricción de presencia y se debe lanzar el Broadcaster
                    If oGroupOld IsNot Nothing AndAlso oGroupOld("DescriptionGroup") <> oGroupNew("DescriptionGroup") Then
                        If oGroupOld("DescriptionGroup").ToString.ToUpper.Contains("TERMINAL=") Or oGroupNew("DescriptionGroup").ToString.ToUpper.Contains("TERMINAL=") Then
                            roConnector.InitTask(TasksType.BROADCASTER)
                        End If
                    End If
                Catch ex As Exception
                    Me.oState.UpdateStateInfo(ex, "roGroup::Save")
                End Try
            Catch Ex As DbException
                Me.oState.UpdateStateInfo(Ex, "roGroup::Save")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roGroup::Save")
            End Try

            Return bolRet

        End Function

        Public Function Recalculate() As Boolean

            Dim bolRet As Boolean = False

            Try

                If roBusinessSupport.GetCustomizationCode() = "taif" Then
                    bolRet = True
                    Return True
                End If

                Dim strSQL As String

                strSQL = "@UPDATE# DailySchedule WITH (ROWLOCK) " &
                                 "SET Status = 45, [GUID] = '' " &
                                 "WHERE STATUS>45 AND " &
                                       "Date <= " & Any2Time(Now.Date).SQLSmallDateTime & " AND " &
                                       "Date > (@SELECT# LockDate from sysrovwEmployeeLockDate where sysrovwEmployeeLockDate.IDEmployee = DailySchedule.IDEmployee)  "
                bolRet = ExecuteSqlWithoutTimeOut(strSQL)

                If bolRet Then
                    ' Notificamos al servidor
                    roConnector.InitTask(TasksType.MOVES)
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roConcept::Recalculate")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roConcept::Recalculate")
            End Try

            Return bolRet

        End Function

        Public Function Delete(Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False
            Me.oState.UpdateStateInfo()

            Dim bHaveToClose As Boolean = False
            Try
                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                If Me.Path.Contains("\") Or Me.bolMultiCompanyLicense Then
                    'If Not IsUsed(oTrans) Then

                    Dim strQuery As String

                    If Me.Path.EndsWith("'\'") Then
                        strQuery = "@SELECT# TOP 1 ID From Groups Where Path Like '" & Me.Path & "%'"
                    Else
                        strQuery = "@SELECT# TOP 1 ID From Groups Where Path Like '" & Me.Path & "\%'"
                    End If
                    Dim tbGroups As DataTable = CreateDataTable(strQuery)
                    If tbGroups.Rows.Count > 0 Then
                        Me.oState.Result = GroupResultEnum.GroupNotEmpty
                    End If

                    If Me.oState.Result = GroupResultEnum.NoError Then

                        'Revisamos si no deja borrar el grupo porque está vinculado a un comunicado
                        Dim strQueryCom = "@SELECT# distinct e.id, e.Name from employees e JOIN EmployeeGroups eg ON EG.IDEmployee = E.ID JOIN Communiques c ON e.ID = c.IdCreatedBy JOIN CommuniqueGroups cg ON cg.IdCommunique = c.Id WHERE GETDATE() BETWEEN EG.BeginDate AND EG.EndDate AND EG.IDGroup = " & Me.intID.ToString & "AND cg.IdGroup = " & Me.intID.ToString
                        Dim tbCom As DataTable = CreateDataTable(strQueryCom)
                        If tbCom.Rows.Count > 0 Then
                            Dim strEmpOwners = ""
                            Me.oState.Result = GroupResultEnum.CantDeleteGroupCommuniquee
                            For Each row In tbCom.Rows
                                Me.oState.ErrorText += " " + row.Item("Name") + ","
                            Next
                            Me.oState.ErrorText = Me.oState.ErrorText.Substring(0, Me.oState.ErrorText.Length - 1)
                        End If
                        'Revisamos si no deja borrar el grupo porque está vinculado a una enquesta
                        Dim strQuerySurv = "@SELECT# distinct e.id, e.Name from employees e JOIN Surveys s ON e.ID = s.IdCreatedBy JOIN SurveyGroups sg ON sg.IdSurvey = s.Id WHERE sg.IdGroup = " & Me.intID.ToString
                        Dim tbSurv As DataTable = CreateDataTable(strQuerySurv)
                        If tbSurv.Rows.Count > 0 Then
                            Dim strEmpOwners = ""
                            Me.oState.Result = GroupResultEnum.CantDeleteGroupSurvey
                            For Each row In tbSurv.Rows
                                Me.oState.ErrorText += " " + row.Item("Name") + ","
                            Next
                            Me.oState.ErrorText = Me.oState.ErrorText.Substring(0, Me.oState.ErrorText.Length - 1)
                        End If
                    End If

                    If Me.oState.Result = GroupResultEnum.NoError Then
                        strQuery = "@SELECT# TOP 1 IDEmployee FROM EmployeeGroups WHERE IDGroup = " & Me.intID.ToString
                        Dim tb As DataTable = CreateDataTable(strQuery)
                        If tb.Rows.Count > 0 Then
                            Me.oState.Result = GroupResultEnum.GroupIsNotEmpty
                        Else

                            Dim _Name As String = ""
                            strQuery = "@SELECT# Name FROM Groups WHERE ID = " & Me.intID
                            tb = CreateDataTable(strQuery)
                            If tb.Rows.Count = 1 Then _Name = tb.Rows(0).Item("Name")

                            Dim strSqls() As String = {"@DELETE# FROM DailyCoverage WHERE IDGroup = " & Me.intID.ToString,
                                                       "@DELETE# FROM EmployeeGroups WHERE IDGroup = " & Me.intID.ToString,
                                                       "@DELETE# FROM GroupIndicators WHERE IDGroup = " & Me.intID.ToString,
                                                       "@DELETE# FROM sysroSecurityNode_Groups WHERE IDGroup = " & Me.intID.ToString,
                                                       "@DELETE# FROM sysroPassports_Groups WHERE IDGroup = " & Me.intID.ToString,
                                                       "@DELETE# FROM GroupsAccessAuthorization WHERE IDGroup = " & Me.intID.ToString,
                                                       "@DELETE# FROM CommuniqueGroups WHERE IDGroup = " & Me.intID.ToString,
                                                       "@DELETE# FROM Groups WHERE ID = " & Me.intID.ToString}
                            For Each strQuery In strSqls
                                If Not ExecuteSql(strQuery) Then
                                    Me.oState.Result = GroupResultEnum.ConnectionError
                                    Exit For
                                End If
                            Next

                            If Me.oState.Result = GroupResultEnum.NoError Then
                                ' Auditar borrado
                                If bAudit Then
                                    Me.oState.Audit(Audit.Action.aDelete, Audit.ObjectType.tGroup, _Name, Nothing, -1)
                                End If
                            End If

                            bolRet = (Me.oState.Result = GroupResultEnum.NoError)

                        End If
                    End If
                    'End If
                Else

                    Me.oState.Result = GroupResultEnum.IsFirstGroupCannotRemove

                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roGroup::Delete")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roGroup::Delete")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet

        End Function

        'Public Function IsUsed() As Boolean

        '    Dim bolIsUsed As Boolean = False

        '
        '

        '    Try

        '        If oActiveTransaction IsNot Nothing Then
        '            oCn = oActiveTransaction.Connection
        '            bolCloseCn = False
        '        Else
        '
        '
        '        End If

        '        Dim strSQL As String
        '        Dim tb As DataTable
        '        Dim strUseConcept As String = ""

        '        Dim strUsedActivity As String = ""
        '        If Not bolIsUsed Then
        '            ' Actividades
        '            ' Verifica que la regla no se esté usando en ningúna actividad
        '            strSQL = "@SELECT# Activities.ID, Activities.Name, ActivityCompanies.* from activities inner join ActivityCompanies on Activities.ID = ActivityCompanies.IDActivity Where IDGroup = " & Me.intID
        '            tb = CreateDataTable(strSQL, )
        '            If tb IsNot Nothing Then
        '                strUsedActivity = ""
        '                For Each oRow As DataRow In tb.Rows
        '                    ' Guardo el nombre de todos los empleados que lo usan
        '                    strUsedActivity &= "," & oRow("Name")
        '                Next
        '                If strUsedActivity <> "" Then strUsedActivity = strUsedActivity.Substring(1)
        '                If strUsedActivity <> "" Then
        '                    oState.Result = groupresultenum.GroupUsedInActivity
        '                    bolIsUsed = True
        '                End If
        '            End If
        '        End If

        '    Catch ex As DbException
        '        oState.UpdateStateInfo(ex, "roGroup::IsUsed")
        '    Catch ex As Exception
        '        oState.UpdateStateInfo(ex, "roGroup::IsUsed")
        '    Finally
        '
        '    End Try

        '    Return bolIsUsed

        'End Function

        Private Function GetNextIDGroup() As Integer

            ' Busca el siguiente ID de grupo.
            Dim intRet As Integer = 1

            Dim strQuery As String = " @SELECT# Max(ID) as Contador From Groups "
            Dim tb As DataTable = CreateDataTable(strQuery, )
            If tb.Rows.Count > 0 Then
                If Not IsDBNull(tb.Rows(0).Item("Contador")) Then
                    intRet = tb.Rows(0).Item("Contador") + 1
                End If
            End If

            Return intRet

        End Function

#End Region

#Region "Helper methods"

        Public Shared Function GetGroupZones(ByVal IDGroup As Integer, ByRef IDZoneWorkingTime As Integer, ByRef IDZoneNonWorkingTime As Integer, ByVal oState As roGroupState, Optional ByVal bolRecursive As Boolean = True) As Boolean
            Dim oRet As Boolean = False

            Try

                IDZoneWorkingTime = -1
                IDZoneNonWorkingTime = -1

                Dim tb As DataTable = CreateDataTable("@SELECT# *, dbo.GetFullGroupPathName(ID) AS FullGroupName FROM Groups WHERE [ID] = " & IDGroup, )
                If tb.Rows.Count > 0 Then
                    If tb.Rows(0).Item("IDZoneWorkingTime") IsNot DBNull.Value Then
                        IDZoneWorkingTime = roTypes.Any2Integer(tb.Rows(0).Item("IDZoneWorkingTime"))
                    Else
                        If bolRecursive Then
                            Dim groupIdsPath() As String = roTypes.Any2String(tb.Rows(0).Item("Path")).Split("\")
                            If groupIdsPath.Length > 0 Then
                                Dim workingZone As Integer = -1
                                Dim tmpdeletezone As Integer = -1
                                For i As Integer = (groupIdsPath.Length - 1) To 0 Step -1
                                    If roTypes.Any2Integer(groupIdsPath(i)) <> IDGroup Then
                                        GetGroupZones(roTypes.Any2Integer(groupIdsPath(i)), workingZone, tmpdeletezone, oState, False)
                                        If workingZone > -1 Then
                                            IDZoneWorkingTime = workingZone
                                            Exit For
                                        End If
                                    End If
                                Next
                            End If
                        End If

                    End If

                    If tb.Rows(0).Item("IDZoneNonWorkingTime") IsNot DBNull.Value Then
                        IDZoneNonWorkingTime = roTypes.Any2Integer(tb.Rows(0).Item("IDZoneNonWorkingTime"))
                    Else
                        If bolRecursive Then
                            Dim groupIdsPath() As String = roTypes.Any2String(tb.Rows(0).Item("Path")).Split("\")
                            If groupIdsPath.Length > 0 Then
                                Dim nonworkingZone As Integer = -1
                                Dim tmpdeletezone As Integer = -1
                                For i As Integer = (groupIdsPath.Length - 1) To 0 Step -1
                                    If roTypes.Any2Integer(groupIdsPath(i)) <> IDGroup Then
                                        GetGroupZones(roTypes.Any2Integer(groupIdsPath(i)), tmpdeletezone, nonworkingZone, oState, False)
                                        If nonworkingZone > -1 Then
                                            IDZoneNonWorkingTime = nonworkingZone
                                            Exit For
                                        End If
                                    End If
                                Next
                            End If
                        End If

                    End If
                End If

                oRet = True
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roGroup::GetGroupByName")
            Finally

            End Try

            Return oRet

        End Function

        Public Shared Function GetGroupCenters(ByVal IDGroup As Integer, ByRef IDCenter As Integer, ByVal oState As roGroupState, Optional ByVal bolRecursive As Boolean = True) As Boolean
            Dim oRet As Boolean = False

            Try

                IDCenter = -1

                Dim tb As DataTable = CreateDataTable("@SELECT# *, dbo.GetFullGroupPathName(ID) AS FullGroupName FROM Groups WHERE [ID] = " & IDGroup, )
                If tb.Rows.Count > 0 Then
                    If tb.Rows(0).Item("IDCenter") IsNot DBNull.Value Then
                        IDCenter = roTypes.Any2Integer(tb.Rows(0).Item("IDCenter"))
                    Else
                        If bolRecursive Then
                            Dim groupIdsPath() As String = roTypes.Any2String(tb.Rows(0).Item("Path")).Split("\")
                            If groupIdsPath.Length > 0 Then
                                Dim Center As Integer = -1
                                For i As Integer = (groupIdsPath.Length - 1) To 0 Step -1
                                    If roTypes.Any2Integer(groupIdsPath(i)) <> IDGroup Then
                                        GetGroupCenters(roTypes.Any2Integer(groupIdsPath(i)), Center, oState, False)
                                        If Center > -1 Then
                                            IDCenter = Center
                                            Exit For
                                        End If
                                    End If
                                Next
                            End If
                        End If

                    End If
                End If

                oRet = True
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roGroup::GetGroupCenters")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roGroup::GetGroupCenters")
            End Try

            Return oRet

        End Function

        Public Shared Function GetSupervisedGroups(ByVal oState As roGroupState, Optional isConsultor As Boolean = False) As System.Data.DataSet
            ' Devuelve un dataset con los grupos disponibles
            Dim strQuery As String
            Dim oDataset As System.Data.DataSet

            If isConsultor Then
                strQuery = "@SELECT# * from groups"
            Else
                strQuery = "@SELECT# g.* from  (@SELECT# groups.* from sysroPassports_Groups spg left join groups on spg.IDGroup = groups.id where spg.IDPassport = " & oState.IDPassport & ") aux inner join groups g on g.Path like aux.Path + '\%' or g.Path = aux.Path"
            End If
            Try

                oDataset = CreateDataSet(strQuery, "Groups")
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roGroup::GetGroups")
                oDataset = Nothing
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roGroup::GetGroups")
                oDataset = Nothing
            End Try

            Return oDataset

        End Function

        Public Shared Function GetGroups(ByVal Feature As String, ByVal Type As String, ByVal oState As roGroupState, Optional sRoot As String = "", Optional ByVal GroupID As String = "") As System.Data.DataSet
            ' Devuelve un dataset con los grupos disponibles
            Dim strQuery As String
            Dim oDataset As System.Data.DataSet
            Dim bolFilter As Boolean = False

            strQuery = "@SELECT# Groups.ID, Groups.Name, Groups.Path, Groups.FullGroupName, LEN(Groups.Path) - LEN(REPLACE(Groups.Path, '\', '')) AS Level From Groups"

            Try

                If Feature <> "" Then
                    strQuery &= roSelector.GetGroupPermissonInnerJoin(oState.IDPassport, Permission.Read, Feature, Type, "", "Groups.ID")
                    bolFilter = True
                End If

                If sRoot IsNot Nothing AndAlso sRoot.Trim.Length > 0 Then
                    strQuery &= " WHERE (Path = '" & sRoot.Trim & "' OR Path Like '" & sRoot.Trim & "\%' OR Path Like '%\" & sRoot.Trim & "\%' OR Path Like '%\" & sRoot.Trim & "') "
                    bolFilter = True
                End If

                If GroupID IsNot Nothing AndAlso GroupID.Trim.Length > 0 Then
                    If bolFilter Then
                        strQuery &= " AND "
                    Else
                        strQuery &= " WHERE "
                    End If
                    If Not IsNumeric(GroupID) Then GroupID = "-1"
                    strQuery &= " Groups.ID= " & GroupID.ToString
                    bolFilter = True
                End If

                oDataset = CreateDataSet(strQuery, "Groups")
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roGroup::GetGroups")
                oDataset = Nothing
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roGroup::GetGroups")
                oDataset = Nothing
            End Try

            Return oDataset

        End Function

        ''' <summary>
        ''' Devuelve un DataTable con los sub-grupos de un grupo
        ''' </summary>
        ''' <param name="IDGroup">ID de Grupo a recuperar sub-grupos</param>
        ''' <param name="Feature">Permisos: ejemplo: 'Calendar', ...</param>
        ''' <param name="Type">Tipo 'U'</param>
        ''' <param name="oState">Objeto roState</param>
        ''' <returns>Devuelve un Dataset con un DataTable con ID, Name, Path</returns>
        ''' <remarks></remarks>
        Public Shared Function GetChildGroups(ByVal IDGroup As Integer, ByVal Feature As String, ByVal Type As String, ByVal oState As roGroupState) As System.Data.DataSet
            ' Devuelve un dataset con los grupos disponibles
            Dim strQuery As String
            Dim oDataset As System.Data.DataSet

            strQuery = "@SELECT# Groups.ID, Groups.Name, Groups.Path, dbo.GetFullGroupPathName(ID) AS FullGroupName From Groups"

            Try

                If Feature <> "" Then
                    strQuery &= roSelector.GetGroupPermissonInnerJoin(oState.IDPassport, Permission.Read, Feature, Type, "", "Groups.ID")
                End If

                strQuery &= " Where Path LIKE '%\" & IDGroup.ToString & "\' + CONVERT(NVARCHAR,[ID]) OR Path = '" & IDGroup.ToString & "\' + CONVERT(NVARCHAR,[ID])"

                oDataset = CreateDataSet(strQuery, "Groups")
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roGroup::GetChildGroups")
                oDataset = Nothing
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roGroup::GetChildGroups")
                oDataset = Nothing
            End Try

            Return oDataset

        End Function

        Public Shared Function GetGroupByName(ByVal strGroupName As String, ByVal oState As roGroupState, Optional ByVal _SecurityFlags As String = "", Optional ByVal _Path As String = "") As roGroup

            Dim oRet As roGroup = Nothing

            Try

                Dim strQuery As String
                strQuery = " @SELECT# [ID] From Groups Where [Name] LIKE '" & strGroupName.Replace("'", "''") & "'"

                If _SecurityFlags <> "" Then strQuery &= " AND SecurityFlags LIKE '" & _SecurityFlags & "'"
                If _Path <> "" Then strQuery &= " AND Path LIKE '" & _Path & "'"

                Dim tb As DataTable = CreateDataTable(strQuery, )
                If tb.Rows.Count > 0 Then
                    oRet = New roGroup(CInt(tb.Rows(0).Item("ID")), oState)
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roGroup::GetGroupByName")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roGroup::GetGroupByName")
            Finally

            End Try

            Return oRet

        End Function

        Public Shared Function GetGroupByKey(ByVal strKey As String, ByVal oState As roGroupState) As roGroup

            Dim oRet As roGroup = Nothing

            Try

                Dim strQuery As String
                strQuery = " @SELECT# [ID] as IDGroup From Groups with (NOLOCK) Where [Export] = '" & strKey.Replace("'", "''") & "'"

                Dim tb As DataTable = CreateDataTable(strQuery, )
                If tb.Rows.Count > 0 Then
                    oRet = New roGroup(CInt(tb.Rows(0).Item("IDGroup")), oState)
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roGroup::GetGroupByKey")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roGroup::GetGroupByKey")
            Finally

            End Try

            Return oRet

        End Function

        Public Shared Function GetCompanyByCIF(ByVal strGroupCIF As String, ByVal oState As roGroupState) As roGroup

            Dim oRet As roGroup = Nothing

            Try

                Dim strQuery As String
                strQuery = " @SELECT# [ID] From Groups Where [USR_CIF] LIKE '" & strGroupCIF.Replace("'", "''") & "' AND CONVERT(varchar, [ID]) = Path"

                Dim tb As DataTable = CreateDataTable(strQuery, )
                If tb.Rows.Count > 0 Then
                    oRet = New roGroup()
                    oRet.ID = CInt(tb.Rows(0).Item("ID"))
                    oRet.Load(False)
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roGroup::GetGroupByCIF")
                oRet = Nothing
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roGroup::GetGroupByCIF")
                oRet = Nothing
            Finally

            End Try

            Return oRet

        End Function

        Public Shared Function GetCompanyByName(ByVal strCompanyName As String, ByVal oState As roGroupState) As roGroup

            Dim oRet As roGroup = Nothing

            Try

                Dim strQuery As String
                strQuery = " @SELECT# [ID] From Groups Where [Name] LIKE '" & strCompanyName.Replace("'", "''") & "' AND CONVERT(varchar, [ID]) = Path"

                Dim tb As DataTable = CreateDataTable(strQuery, )
                If tb.Rows.Count > 0 Then
                    oRet = New roGroup(CInt(tb.Rows(0).Item("ID")), oState)
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roGroup::GetCompanyByName")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roGroup::GetCompanyByName")
            Finally

            End Try

            Return oRet

        End Function

        Public Shared Function GetCompanies(ByVal oState As roGroupState) As List(Of roGroup)

            Dim oRet As New List(Of roGroup)

            Try

                Dim strQuery As String
                strQuery = " @SELECT# * From Groups Where LEN(Path) = LEN(REPLACE(Path,'\','')) ORDER BY ID ASC"

                Dim tb As DataTable = CreateDataTable(strQuery, )
                If tb.Rows.Count > 0 Then
                    oRet.Add(New roGroup(CInt(tb.Rows(0).Item("ID")), oState))
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roGroup::GetCompanyByName")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roGroup::GetCompanyByName")
            End Try

            Return oRet

        End Function

        Public Shared Function GetCompaniesCount(ByVal oState As roGroupState) As Integer

            Dim ret As Integer = 0

            Try

                Dim strQuery As String
                strQuery = " @SELECT# * From Groups Where LEN(Path) = LEN(REPLACE(Path,'\','')) ORDER BY ID ASC"

                Dim tb As DataTable = CreateDataTable(strQuery, )
                If tb.Rows.Count > 0 Then
                    ret = tb.Rows.Count
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roGroup::GetCompaniesCount")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roGroup::GetCompaniesCount")
            End Try

            Return ret

        End Function

        Public Shared Function GetGroupByNameInLevel(ByVal strGroupName As String, ByVal strPath As String, ByVal oState As roGroupState, Optional ByVal _SecurityFlags As String = "") As roGroup

            Dim oRet As roGroup = Nothing

            Try

                Dim strQuery As String
                strQuery = " @SELECT# [ID] From Groups Where [Name] LIKE '" & strGroupName.Replace("'", "''") & "'"

                If _SecurityFlags <> "" Then strQuery &= " AND SecurityFlags LIKE '" & _SecurityFlags & "'"
                If strPath <> "" Then
                    strQuery &= " AND [Path] = '" & strPath & "\'+CONVERT(varchar,[ID])"
                Else
                    strQuery &= " AND [Path] = CONVERT(varchar,[ID])"
                End If

                Dim tb As DataTable = CreateDataTable(strQuery, )
                If tb.Rows.Count > 0 Then
                    oRet = New roGroup(CInt(tb.Rows(0).Item("ID")), oState)
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roGroup::GetGroupByNameInLevel")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roGroup::GetGroupByNameInLevel")
            End Try

            Return oRet

        End Function

        Public Shared Function GetEmployeesFromGroup(ByVal IDGroup As Integer, ByVal Feature As String, ByVal Type As String, ByRef oState As roGroupState, Optional ByVal excludeWithoutContract As Boolean = False) As System.Data.DataSet
            ' Devuelve un dataset con los empledos que pertenecen al grupo pasado por parámetro
            ' Se da por supuesto que nunca llegara un IDGroup sobre el que no se tengan permisos
            Dim oRet As System.Data.DataSet = Nothing

            Try

                Dim strQuery As String
                strQuery = "@SELECT# tmp.* from (@SELECT# * from sysrovwCurrentEmployeeGroups ) tmp"
                If Feature <> String.Empty Then
                    strQuery &= " " & roSelector.GetEmployeePermissonInnerJoin(oState.IDPassport, Permission.Read, Feature, Type)
                End If
                strQuery = strQuery & " Where tmp.IDGroup = " & IDGroup & IIf(excludeWithoutContract, " AND tmp.CurrentEmployee = 1 ", "")

                strQuery = strQuery & " Order By tmp.EmployeeName "

                If Feature <> String.Empty Then
                    strQuery += " " & SQLServerHint.GetSQLHint(SQLServerHint.SelectHinted.GetEmployeesFromGroupWithPermissions)
                End If

                oRet = CreateDataSet(strQuery)
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roGroup::GetEmployeesFromGroup")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roGroup::GetEmployeesFromGroup")
            End Try

            Return oRet

        End Function

        Public Shared Function GetEmployeesInTransitToTheGroup(ByVal IDGroup As Integer, ByVal Feature As String, ByVal Type As String, ByRef oState As roGroupState) As System.Data.DataSet
            ' Devuelve un dataset con los empledos en tránsito hacia el grupo pasado por parámetro.
            Dim oRet As System.Data.DataSet = Nothing

            Try

                Dim strQueryPermissions As String = String.Empty

                'If Feature <> String.Empty Then
                '    strQueryPermissions &= " AND" & WLHelper.GetEmployeePermissonWhereClause(oState.IDPassport, Permission.Read, Feature, Type, "", "Employees.ID")
                'End If

                Dim strQuery As String
                strQuery = "@SELECT# DISTINCT  Employees.ID, Employees.Name, Groups.Path as Path, Employees.Type, BeginDate, EndDate "
                strQuery = strQuery & " FROM Employees, EmployeeGroups, Groups"
                strQuery = strQuery & " WHERE Employees.ID = EmployeeGroups.IDEmployee AND "
                strQuery = strQuery & " Employees.ID NOT IN (@SELECT# IDEmployee FROM EmployeeContracts "
                strQuery = strQuery & " WHERE BeginDate<=" & Any2Time(Now.Date).SQLSmallDateTime & ") AND "
                strQuery = strQuery & " EmployeeGroups.IDGroup = Groups.ID And "
                strQuery = strQuery & " Groups.ID=" & IDGroup ' & strQueryPermissions
                strQuery = strQuery & " UNION "
                strQuery = strQuery & " @SELECT# Employees.ID, Employees.Name, Groups.Path as Path, Employees.Type, EmployeeGroups.BeginDate, EmployeeGroups.EndDate  "
                strQuery = strQuery & " FROM EmployeeGroups, employees, Groups "
                strQuery = strQuery & " WHERE(EmployeeGroups.IDEmployee = Employees.ID)"
                strQuery = strQuery & " AND EmployeeGroups.BeginDate>" & Any2Time(Now.Date).SQLSmallDateTime
                strQuery = strQuery & " AND  EmployeeGroups.IDGroup = Groups.ID "
                strQuery = strQuery & " AND EmployeeGroups.IDgroup =" & IDGroup '& strQueryPermissions

                If Feature <> String.Empty Then
                    strQuery = " @SELECT# tmp.* FROM (" & strQuery & ") tmp " & roSelector.GetEmployeePermissonInnerJoin(oState.IDPassport, Permission.Read, Feature, Type,, "ID")
                End If

                oRet = CreateDataSet(strQuery)
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roGroup::GetEmployeesInTransitToTheGroup")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roGroup::GetEmployeesInTransitToTheGroup")
            End Try

            Return oRet

        End Function

        Public Shared Function GetOldEmployeesFromGroup(ByVal IDGroup As Integer, ByVal Feature As String, ByVal Type As String, ByRef oState As roGroupState, Optional ByVal includeWithoutContract As Boolean = False) As System.Data.DataSet
            ' Devuelve un dataset con los empledos que estuvieron en el grupo pasado por parámetro.
            Dim oRet As System.Data.DataSet = Nothing

            Try

                'Dim strQueryPermissions As String = String.Empty

                'If Feature <> String.Empty Then
                '    strQueryPermissions &= " AND" & WLHelper.GetEmployeePermissonWhereClause(oState.IDPassport, Permission.Read, Feature, Type, "", "Employees.ID")
                'End If

                Dim strQuery As String
                strQuery = "@SELECT# Employees.ID, Employees.Name, Employees.Type, EmployeeGroups.BeginDate, EmployeeGroups.EndDate"
                strQuery = strQuery & " From Employees "
                strQuery = strQuery & " Inner Join EmployeeGroups ON "
                strQuery = strQuery & " EmployeeGroups.IDGroup = " & IDGroup
                strQuery = strQuery & " Where "
                strQuery = strQuery & " EndDate < " & Any2Time(Now.Date).SQLSmallDateTime
                strQuery = strQuery & " And Employees.ID = EmployeeGroups.IDEmployee " '& strQueryPermissions

                If includeWithoutContract Then
                    strQuery = strQuery & " UNION " &
                    " @SELECT# IDEmployee, EmployeeName, employees.Type, srvceg.BeginDate, (@SELECT# top 1 EndDate from EmployeeContracts	where IDEmployee = srvceg.IDEmployee order by EndDate desc) from sysrovwCurrentEmployeeGroups srvceg inner join Employees on Employees.ID = srvceg.IDEmployee " &
                    " where IDGroup = " & IDGroup & " and CurrentEmployee = 0 " '& strQueryPermissions
                End If

                If Feature <> String.Empty Then
                    strQuery = " @SELECT# tmp.* FROM (" & strQuery & ") tmp " & roSelector.GetEmployeePermissonInnerJoin(oState.IDPassport, Permission.Read, Feature, Type,, "ID")
                End If

                oRet = CreateDataSet(strQuery)
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roGroup::GetOldEmployeesFromGroup")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roGroup::GetOldEmployeesFromGroup")
            End Try

            Return oRet

        End Function

        Public Shared Function GetEmployeesFromGroupWithType(ByVal IDGroup As Integer, ByVal Feature As String, ByVal Type As String, ByVal FieldWhere As String, ByRef oState As roGroupState) As System.Data.DataSet
            ' Devuelve un dataset con los empledos que pertenecen al grupo pasado por parámetro
            ' Se da por supuesto que nunca llegara un IDGroup sobre el que no se tengan permisos

            ' El formato del parámetro UserFieldWhere tiene que se el de una condición/es SQL:
            ' (Name = 'PEPE' or USR_Direccion like 'c%') and [USR_Correo Electrónico] = 'pepe@robotics.es'
            ' NOTA: NO PONER AND/OR/WHERE DELANTE DE LA CONDICION, ESO LO PONE LA FUNCION SI LO NECESITA

            ' Combinaciones de parámetros:
            ' IDGroup = -1 & FieldWhere <> "" - Devuelve toda la lista y el where se aplica a la select en general
            ' IDGroup <> -1 & FieldWhere = "" - Devuelve el contenido de sysrovwAllEmployeeGroups para ese grupo
            ' IDGroup <> -1 & FieldWhere <> "" - Devuelve el contenido de sysrovwAllEmployeeGroups combinado
            '                                    con Employees al que se le aplica el where

            Dim strDeclare As String = String.Empty
            Dim strQuery As String = String.Empty
            Dim oDataset As System.Data.DataSet = Nothing
            Dim oDataRow As System.Data.DataRow
            Dim oDataTable As New System.Data.DataTable
            Dim oDataColumn As System.Data.DataColumn

            'IMPORTANTE:El parametro FieldWhere se debe procesar adecuadamente. Según la forma Expresion1 + "#@#" +  Expresion2
            If FieldWhere <> String.Empty AndAlso FieldWhere.IndexOf("#@#") > 0 Then
                Dim strSplit As String() = {"#@#"}
                Dim strFilter As String() = FieldWhere.Split(strSplit, StringSplitOptions.None)

                strDeclare = strFilter(0) 'Contiene la expresion del declare para el parametro de funciones de la BBDD
                FieldWhere = strFilter(1) 'Contiene el filtro real
            End If

            strQuery = strQuery & " @SELECT# sysrovwAllEmployeeGroups.* from sysrovwAllEmployeeGroups "
            If FieldWhere <> "" AndAlso IDGroup <> -1 Then
                strQuery = strQuery & " Inner Join Employees ON "
                strQuery = strQuery & " Employees.ID = sysrovwAllEmployeeGroups.IDEmployee "
            End If

            If IDGroup <= 0 Then
                If FieldWhere <> "" Then
                    strQuery = strQuery & " Where " & FieldWhere
                End If
            Else
                strQuery = strQuery & " Where IDGroup = " & IDGroup
                If FieldWhere <> "" Then
                    strQuery &= " AND " & FieldWhere
                End If
            End If

            If Feature <> String.Empty Then
                If IDGroup = 0 Then
                    strQuery = strDeclare & " @SELECT# tmp.* FROM (" & strQuery & ") tmp " & roSelector.GetEmployeeExceptionsInnerJoin(oState.IDPassport, Permission.Read, Feature, Type)
                Else
                    strQuery = strDeclare & " @SELECT# tmp.* FROM (" & strQuery & ") tmp " & roSelector.GetEmployeePermissonInnerJoin(oState.IDPassport, Permission.Read, Feature, Type)
                End If
                strQuery += " " & SQLServerHint.GetSQLHint(SQLServerHint.SelectHinted.GetEmployeesFromGroupWithTypeWithPermissions)

            Else
                strQuery = strDeclare & strQuery
            End If

            Try
                oDataset = CreateDataSet(strQuery)

                ' Configuro la tabla de resultado
                oDataTable.TableName = "Resultado"
                oDataColumn = New System.Data.DataColumn
                oDataColumn.ColumnName = "IDEmployee"
                oDataTable.Columns.Add(oDataColumn)

                oDataColumn = New System.Data.DataColumn
                oDataColumn.ColumnName = "EmployeeName"
                oDataTable.Columns.Add(oDataColumn)

                oDataColumn = New System.Data.DataColumn
                oDataColumn.ColumnName = "Type"
                oDataTable.Columns.Add(oDataColumn)

                oDataColumn = New System.Data.DataColumn
                oDataColumn.ColumnName = "AttControlled"
                oDataTable.Columns.Add(oDataColumn)

                oDataColumn = New System.Data.DataColumn
                oDataColumn.ColumnName = "AccControlled"
                oDataTable.Columns.Add(oDataColumn)

                oDataColumn = New System.Data.DataColumn
                oDataColumn.ColumnName = "JobControlled"
                oDataTable.Columns.Add(oDataColumn)

                oDataColumn = New System.Data.DataColumn
                oDataColumn.ColumnName = "ExtControlled"
                oDataTable.Columns.Add(oDataColumn)

                oDataColumn = New System.Data.DataColumn
                oDataColumn.ColumnName = "RiskControlled"
                oDataTable.Columns.Add(oDataColumn)

                oDataColumn = New System.Data.DataColumn
                oDataColumn.ColumnName = "IDGroup"
                oDataTable.Columns.Add(oDataColumn)

                oDataColumn = New System.Data.DataColumn
                oDataColumn.ColumnName = "GroupName"
                oDataTable.Columns.Add(oDataColumn)

                Dim oAuxDataTable As New DataTable

                oAuxDataTable = oDataset.Tables(0)

                For Each oDataRow In oAuxDataTable.Rows
                    oDataTable.Rows.Add()
                    oDataTable.Rows(oDataTable.Rows.Count - 1).Item("IDEmployee") = oDataRow("IDEmployee")
                    oDataTable.Rows(oDataTable.Rows.Count - 1).Item("EmployeeName") = oDataRow("EmployeeName")
                    oDataTable.Rows(oDataTable.Rows.Count - 1).Item("IDGroup") = oDataRow("IDGroup")
                    oDataTable.Rows(oDataTable.Rows.Count - 1).Item("GroupName") = oDataRow("GroupName")

                    If oDataRow("CurrentEmployee") = 0 And oDataRow("Begindate") >= Now Then
                        ' El empleado es una futura incorporación
                        oDataTable.Rows(oDataTable.Rows.Count - 1).Item("Type") = 4
                    Else
                        If oDataRow("CurrentEmployee") = 0 And oDataRow("Begindate") < Now Then
                            ' El empleado es una baja
                            oDataTable.Rows(oDataTable.Rows.Count - 1).Item("Type") = 3
                        Else
                            If oDataRow("CurrentEmployee") = 1 And oDataRow("Enddate") <> CDate("01/01/2079") Then
                                ' Empleado con movilidad
                                oDataTable.Rows(oDataTable.Rows.Count - 1).Item("Type") = 2
                            Else
                                ' Empleado normal
                                oDataTable.Rows(oDataTable.Rows.Count - 1).Item("Type") = 1
                            End If
                        End If
                    End If

                    oDataTable.Rows(oDataTable.Rows.Count - 1).Item("AttControlled") = oDataRow("AttControlled")
                    oDataTable.Rows(oDataTable.Rows.Count - 1).Item("AccControlled") = oDataRow("AccControlled")
                    oDataTable.Rows(oDataTable.Rows.Count - 1).Item("JobControlled") = oDataRow("JobControlled")
                    oDataTable.Rows(oDataTable.Rows.Count - 1).Item("ExtControlled") = oDataRow("ExtControlled")
                    oDataTable.Rows(oDataTable.Rows.Count - 1).Item("RiskControlled") = oDataRow("RiskControlled")

                Next
                oDataTable.AcceptChanges()

                oDataset = New System.Data.DataSet
                oDataset.Tables.Add(oDataTable)
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roGroup::GetEmployeesFromGroupWithType")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roGroup::GetEmployeesFromGroupWithType")
                oDataset = Nothing
            End Try

            Return oDataset

        End Function

        ''' <summary>
        ''' Devuelve un lista con los códigos de los grupos padre de la selección indicada.
        ''' </summary>
        ''' <param name="lstGroupsSelection"></param>
        ''' <param name="lstEmployeesSelection"></param>
        ''' <param name="_State"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetGroupSelectionPath(ByVal lstGroupsSelection As Generic.List(Of Integer), ByVal lstEmployeesSelection As Generic.List(Of Integer), ByRef _State As roGroupState) As Generic.List(Of Integer)

            Dim oRet As New Generic.List(Of Integer)

            Try

                Dim oGroup As roGroup
                Dim oEmployeeState As New Employee.roEmployeeState(_State.IDPassport, _State.Context)
                Dim oMobility As Employee.roMobility

                For Each ID As String In lstGroupsSelection
                    ' Obtenemos la definición del grupo
                    oGroup = New roGroup(ID, _State)
                    ' Recorremos los grupos del path y los añadimos a la lista de expansión (si no existen)
                    For Each IDParent As String In oGroup.Path.Split("\")
                        If lstGroupsSelection.IndexOf(IDParent) = -1 AndAlso oRet.IndexOf(IDParent) = -1 Then
                            oRet.Add(IDParent)
                        End If
                    Next
                Next
                For Each ID As String In lstEmployeesSelection
                    ' Obtenemos la mobilidad actual del empleado
                    oMobility = Employee.roMobility.GetCurrentMobility(ID, oEmployeeState)
                    If oMobility IsNot Nothing Then
                        ' Obtenemos el grupo
                        oGroup = New roGroup(oMobility.IdGroup, _State)
                        For Each IDParent As String In oGroup.Path.Split("\")
                            If lstGroupsSelection.IndexOf(IDParent) = -1 AndAlso oRet.IndexOf(IDParent) = -1 Then
                                oRet.Add(IDParent)
                            End If
                        Next
                    End If
                Next
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roGroup::GetGroupSelectionPath")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roGroup::GetGroupSelectionPath")
            End Try

            Return oRet

        End Function

        ''' <summary>
        ''' Devuelve el último grupo de un string de ruta dado.
        ''' </summary>
        ''' <param name="pathString">El string que contiene la ruta.</param>
        ''' <returns>El nombre del último grupo como un string sin espacios.</returns>
        ''' <remarks></remarks>
        Public Shared Function GetLastGroup(pathString As String, ByRef oState As roGroupState) As String
            Try
                Dim lastBackslashPos As Integer
                lastBackslashPos = InStrRev(pathString, "\")

                If lastBackslashPos > 0 Then
                    Return Trim(Right(pathString, Len(pathString) - lastBackslashPos))
                Else
                    Return Trim(pathString)
                End If
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roGroup::GetLastGroup")
                Return String.Empty
            End Try
        End Function

        ''' <summary>
        ''' Obtiene el ID de la compañía raíz a partir de un ID de grupo dado.
        ''' </summary>
        ''' <param name="groupId">El ID del grupo a procesar.</param>
        ''' <returns>El ID de la compañía raíz si se encuentra, o 0 si no se encuentra.</returns>
        ''' <remarks></remarks>
        Public Shared Function GetRootCompanyIdFromGroupId(ByVal groupId As Integer, ByRef oState As roGroupState) As Integer
            Try
                If groupId > 0 Then
                    Dim sql As String = $"@SELECT# Path FROM Groups WHERE ID = {groupId}"

                    Dim path As String = roTypes.Any2String(ExecuteScalar(sql))

                    If Not String.IsNullOrEmpty(path) Then
                        Dim pathParts As String() = path.Split("\"c)
                        If pathParts.Length > 0 Then
                            Dim rootId As Integer = 0
                            If Integer.TryParse(pathParts(0), rootId) Then
                                Return rootId
                            End If
                        End If
                    End If

                    Return groupId
                End If
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roGroup::GetRootCompanyIdFromGroupId")
            End Try

            Return 0
        End Function

        Public Shared Function RegeneratePasswordsToEmployees(ByVal strEmployeeFilter As String, ByVal Feature As String,
                                                              ByVal strFilters As String, ByVal strFilterUserFields As String, ByRef oState As roGroupState) As Boolean

            Dim bolret As Boolean = True

            Try

                Dim lstEmployees As Generic.List(Of Integer) = roSelector.GetEmployeeList(oState.IDPassport, Feature, "U", Permission.Read,
                                                                                                        strEmployeeFilter, strFilters, strFilterUserFields, False, Nothing, Nothing)

                If lstEmployees IsNot Nothing AndAlso lstEmployees.Count > 0 Then

                    lstEmployees = lstEmployees.Distinct().ToList()

                    Dim sActive As Boolean = roTypes.Any2Boolean(ExecuteScalar("@SELECT# Data FROM sysroParameters WHERE ID = 'ACTIVE'"))

                    Dim strEmployeeNamesForAudit As String = roBusinessSupport.GetAuditEmployeeNames(lstEmployees, oState)

                    Dim oManager As New roPassportManager(oState.IDPassport)
                    Dim oPassport As roPassport

                    Dim oPassportAuthenticationMethods As roPassportAuthenticationMethods
                    For Each IdEmployee As Integer In lstEmployees
                        Try
                            oPassport = oManager.LoadPassport(IdEmployee, LoadType.Employee)
                        Catch ex As Exception
                            oPassport = Nothing
                        End Try

                        If oPassport IsNot Nothing Then
                            oPassportAuthenticationMethods = oPassport.AuthenticationMethods
                            If oPassportAuthenticationMethods IsNot Nothing AndAlso oPassportAuthenticationMethods.PasswordRow IsNot Nothing AndAlso
                                Not oPassportAuthenticationMethods.PasswordRow.Credential.Contains("\") Then 'Usuario no se valida mediante Active Directory

                                Dim r As New Random()
                                Dim strCode As New StringBuilder
                                For i As Integer = 1 To 10
                                    strCode.Append(CStr(r.Next(0, 9)))
                                Next
                                oPassportAuthenticationMethods.PasswordRow.Password = CryptographyHelper.EncryptWithMD5(strCode.ToString)
                                oPassportAuthenticationMethods.PasswordRow.RowState = RowState.UpdateRow
                                oPassportAuthenticationMethods.PasswordRow.LastUpdatePassword = New Date(1900, 1, 1)
                                oPassportAuthenticationMethods.PasswordRow.InvalidAccessAttemps = 0
                                oPassportAuthenticationMethods.PasswordRow.LastDateInvalidAccessAttempted = Nothing
                                oManager.Save(oPassport)

                                'Solo enviamos el mail si el servicio esta activo
                                If roPassportManager.IsRoboticsUserOrConsultant(oPassport.ID) OrElse sActive Then
                                    Robotics.DataLayer.AccessHelper.ExecuteSql("@INSERT# INTO sysroNotificationTasks (IDNotification, Key1Numeric, Key2Numeric , Parameters) VALUES (1903, " & IdEmployee & ", " & oPassport.ID & ",'" & CryptographyHelper.Encrypt(strCode.ToString) & "')")
                                End If
                            End If
                        End If
                    Next

                    'Auditoria
                    If strEmployeeNamesForAudit.Length > 0 Then
                        Dim tbParameters As DataTable = oState.CreateAuditParameters()
                        oState.AddAuditParameter(tbParameters, "{EmployeesNames}", strEmployeeNamesForAudit, "", 1)
                        oState.Audit(Audit.Action.aUpdate, Audit.ObjectType.tPassportPassword, "", tbParameters, -1)
                    End If

                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roGroup::RegeneratePasswordsToEmployees")
                bolret = False
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roGroup::RegeneratePasswordsToEmployees")
                bolret = False
            End Try

            Return bolret

        End Function

        Public Shared Function SendUsernameToEmployees(ByVal strEmployeeFilter As String, ByVal Feature As String,
                                                              ByVal strFilters As String, ByVal strFilterUserFields As String, ByRef oState As roGroupState) As Boolean

            Dim bolret As Boolean = True

            Try


                Dim lstEmployees As Generic.List(Of Integer) = roSelector.GetEmployeeList(oState.IDPassport, Feature, "U", Permission.Read,
                                                                                                        strEmployeeFilter, strFilters, strFilterUserFields, False, Nothing, Nothing)

                If lstEmployees IsNot Nothing AndAlso lstEmployees.Count > 0 Then

                    lstEmployees = lstEmployees.Distinct().ToList()

                    Dim sActive As Integer = roTypes.Any2Integer(ExecuteScalar("@SELECT# Data FROM sysroParameters WHERE ID = 'ACTIVE'"))

                    Dim oManager As New roPassportManager(oState.IDPassport)
                    Dim oPassport As roPassport
                    Dim oPassportAuthenticationMethods As roPassportAuthenticationMethods = Nothing
                    For Each IdEmployee As Integer In lstEmployees
                        Try
                            oPassport = oManager.LoadPassport(IdEmployee, LoadType.Employee)
                            If oPassport IsNot Nothing Then oPassportAuthenticationMethods = oPassport.AuthenticationMethods
                        Catch ex As Exception
                            oPassport = Nothing
                        End Try

                        'Solo enviamos el mail si el servicio esta activo
                        If oPassport IsNot Nothing AndAlso oPassportAuthenticationMethods IsNot Nothing AndAlso oPassportAuthenticationMethods.PasswordRow IsNot Nothing AndAlso
                            (roPassportManager.IsRoboticsUserOrConsultant(oPassport.ID) OrElse sActive = 1) Then
                            Robotics.DataLayer.AccessHelper.ExecuteSql("@INSERT# INTO sysroNotificationTasks (IDNotification, Key1Numeric, Key2Numeric , Parameters) VALUES (1905, " & IdEmployee & ", " & oPassport.ID & ",'" & oPassportAuthenticationMethods.PasswordRow.Credential & "')")
                        End If
                    Next
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roGroup::SendUsernameToEmployees")
                bolret = False
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roGroup::SendUsernameToEmployees")
                bolret = False
            End Try

            Return bolret

        End Function

        Public Shared Function SetBloquedAccessAppToEmployees(ByVal strEmployeeFilter As String, ByVal Feature As String,
                                                              ByVal strFilters As String, ByVal strFilterUserFields As String, ByVal LockAccess As Boolean, ByRef oState As roGroupState) As Boolean

            Dim bolRet As Boolean

            Try

                Dim lstEmployees As Generic.List(Of Integer) = roSelector.GetEmployeeList(oState.IDPassport, Feature, "U", Permission.Read,
                                                                                                        strEmployeeFilter, strFilters, strFilterUserFields, False, Nothing, Nothing)

                Dim strEmployeeNamesForAudit As String = roBusinessSupport.GetAuditEmployeeNames(lstEmployees, oState)
                Dim oManager As New roPassportManager(oState.IDPassport)
                Dim oPassport As roPassport
                Dim oPassportAuthenticationMethods As roPassportAuthenticationMethods
                For Each IdEmployee As Integer In lstEmployees
                    Try
                        oPassport = oManager.LoadPassport(IdEmployee, LoadType.Employee)
                    Catch ex As Exception
                        oPassport = Nothing
                    End Try

                    If oPassport IsNot Nothing AndAlso Not roPassportManager.GetPassportBelongsToAdminGroup(oPassport.ID) Then
                        oPassportAuthenticationMethods = oPassport.AuthenticationMethods
                        If oPassportAuthenticationMethods IsNot Nothing AndAlso oPassportAuthenticationMethods.PasswordRow IsNot Nothing Then
                            oPassportAuthenticationMethods.PasswordRow.BloquedAccessApp = LockAccess
                            oPassportAuthenticationMethods.PasswordRow.RowState = RowState.UpdateRow
                            oManager.Save(oPassport)
                        End If
                    End If
                Next

                'Auditoría
                If strEmployeeNamesForAudit.Length > 0 Then
                    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    oState.AddAuditParameter(tbParameters, "{EmployeesNames}", strEmployeeNamesForAudit, "", 1)
                    If LockAccess Then
                        oState.Audit(Audit.Action.aBlock, Audit.ObjectType.tPassport, "", tbParameters, -1)
                    Else
                        oState.Audit(Audit.Action.aUnblock, Audit.ObjectType.tPassport, "", tbParameters, -1)
                    End If

                End If

                bolRet = True
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roGroup::BlockAccessAppToEmployees")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roGroup::BlockAccessAppToEmployees")
            End Try

            Return bolRet

        End Function

        Public Shared Function SetAppConfigurationToEmployees(ByVal strEmployeeFilter As String, ByVal Feature As String,
                                                      ByVal strFilters As String, ByVal strFilterUserFields As String, ByVal LockAccess As Boolean, ByRef oState As roGroupState, ByVal FromEmployee As Integer) As Boolean

            Dim bolRet As Boolean
            Dim strSQL As String

            Try

                Dim oFromPassport As roPassport = Nothing

                Try
                    oFromPassport = roPassportManager.GetPassport(FromEmployee, LoadType.Employee)
                Catch ex As Exception
                    oFromPassport = Nothing
                End Try

                If oFromPassport Is Nothing Then
                    bolRet = True
                    Return bolRet
                End If

                Dim loginWithoutContract = 0

                Try
                    loginWithoutContract = roTypes.Any2Integer(ExecuteScalar("@SELECT# LoginWithoutContract FROM sysroPassPorts WHERE IDEmployee = " & FromEmployee & ""))
                Catch ex As Exception
                    roLog.GetInstance().logMessage(roLog.EventType.roError, "roGroup::SetAppConfigurationToEmployees::Unknown error", ex)
                End Try

                Dim lstEmployees As Generic.List(Of Integer) = roSelector.GetEmployeeList(oState.IDPassport, Feature, "U", Permission.Write,
                                                                                                        strEmployeeFilter, strFilters, strFilterUserFields, False, Nothing, Nothing)

                Dim strEmployeeNamesForAudit As String = roBusinessSupport.GetAuditEmployeeNames(lstEmployees, oState)
                Dim oEmpState As New Employee.roEmployeeState
                roBusinessState.CopyTo(oState, oEmpState)

                Dim bHaveToClose As Boolean = Robotics.DataLayer.AccessHelper.StartTransaction()
                bolRet = True

                For Each IdEmployee As Integer In lstEmployees
                    If IdEmployee <> FromEmployee AndAlso bolRet Then
                        strSQL = "@UPDATE# sysroPassports SET EnabledVTDesktop = " & If(oFromPassport.EnabledVTDesktop, "1", "0") & ", " &
                                                       "EnabledVTPortal  = " & If(oFromPassport.EnabledVTPortal, "1", "0") & ", " &
                                                       "EnabledVTPortalApp = " & If(oFromPassport.EnabledVTPortalApp, "1", "0") & ", " &
                                                       "EnabledVTVisits  = " & If(oFromPassport.EnabledVTVisits, "1", "0") & ", " &
                                                       "EnabledVTVisitsApp  = " & If(oFromPassport.EnabledVTVisitsApp, "1", "0") & ", " &
                                                       "PhotoRequiered  = " & If(oFromPassport.PhotoRequiered, "1", "0") & ", " &
                                                       "LocationRequiered  = " & If(oFromPassport.LocationRequiered, "1", "0") & ", " &
                                                       "LoginWithoutContract  = " & loginWithoutContract &
                                    " WHERE IDEmployee= " & IdEmployee.ToString

                        bolRet = ExecuteSqlWithoutTimeOut(strSQL)
                    End If
                Next

                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)

                'Auditoría
                If bolRet AndAlso strEmployeeNamesForAudit.Length > 0 Then
                    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    oState.AddAuditParameter(tbParameters, "{EmployeesNames}", strEmployeeNamesForAudit, "", 1)
                    If LockAccess Then
                        oState.Audit(Audit.Action.aExecuted, Audit.ObjectType.tPassport, "", tbParameters, -1)
                    Else
                        oState.Audit(Audit.Action.aExecuted, Audit.ObjectType.tPassport, "", tbParameters, -1)
                    End If

                End If
            Catch ex As DbException
                bolRet = False
                oState.UpdateStateInfo(ex, "roGroup::SetPermissionsToEmployees")
            End Try

            Return bolRet

        End Function

        Public Shared Function SetPermissionsToEmployees(ByVal strEmployeeFilter As String, ByVal Feature As String,
                                                      ByVal strFilters As String, ByVal strFilterUserFields As String, ByVal LockAccess As Boolean, ByRef oState As roGroupState, ByVal FromEmployee As Integer) As Boolean

            Dim bolRet As Boolean = False
            Dim strSQL As String = ""

            Try

                Dim oFromPassport As roPassport = Nothing

                Try
                    oFromPassport = roPassportManager.GetPassport(FromEmployee, LoadType.Employee)
                Catch ex As Exception
                    oFromPassport = Nothing
                End Try

                If oFromPassport Is Nothing Then
                    bolRet = True
                    Return bolRet
                End If




                Dim lstEmployees As Generic.List(Of Integer) = roSelector.GetEmployeeList(oState.IDPassport, Feature, "U", Permission.Write,
                                                                                                        strEmployeeFilter, strFilters, strFilterUserFields, False, Nothing, Nothing)

                Dim strEmployeeNamesForAudit As String = roBusinessSupport.GetAuditEmployeeNames(lstEmployees, oState)

                Dim oPassport As roPassportTicket = Nothing
                bolRet = True

                Dim bHaveToClose As Boolean = Robotics.DataLayer.AccessHelper.StartTransaction()
                For Each IdEmployee As Integer In lstEmployees
                    If IdEmployee <> FromEmployee AndAlso bolRet Then
                        Try
                            oPassport = roPassportManager.GetPassportTicket(IdEmployee, LoadType.Employee)
                        Catch ex As Exception
                            oPassport = Nothing
                        End Try

                        If oPassport IsNot Nothing Then
                            ' Eliminamos todos los permisos del empleado
                            bolRet = ExecuteSqlWithoutTimeOut("@DELETE# FROM sysroPassports_PermissionsOverFeatures WHERE IDPassport=" & oPassport.ID.ToString & " AND IDFeature IN(@SELECT# ID FROM sysroFeatures WHERE Type ='E')")

                            ' Generamos los mismos que tiene el empleado origen
                            strSQL = "@INSERT# INTO sysroPassports_PermissionsOverFeatures(IDPassport, IDFeature, Permission) " &
                                        "  @SELECT# " & oPassport.ID.ToString & ", IDFeature, Permission " &
                                        " FROM sysroPassports_PermissionsOverFeatures WHERE IDPassport= " & oFromPassport.ID.ToString &
                                        " AND sysroPassports_PermissionsOverFeatures.IDFeature IN(@SELECT# ID FROM sysroFeatures WHERE Type = 'E') "
                            bolRet = ExecuteSqlWithoutTimeOut(strSQL)
                        End If
                    End If
                Next

                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)

                'Auditoría
                If bolRet AndAlso strEmployeeNamesForAudit.Length > 0 Then
                    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    oState.AddAuditParameter(tbParameters, "{EmployeesNames}", strEmployeeNamesForAudit, "", 1)
                    If LockAccess Then
                        oState.Audit(Audit.Action.aUpdate, Audit.ObjectType.tPassport, "", tbParameters, -1)
                    Else
                        oState.Audit(Audit.Action.aUpdate, Audit.ObjectType.tPassport, "", tbParameters, -1)
                    End If

                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roGroup::SetPermissionsToEmployees")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roGroup::SetPermissionsToEmployees")
            End Try

            Return bolRet

        End Function

        Public Shared Function GetSecurityNodeFromGroupOrEmployee(ByRef oState As roGroupState, ByVal _intID As Integer, ByVal _IsGroup As Boolean, Optional xDate As Date = Nothing) As Integer
            Dim bolRet As Integer = -1

            Dim strSQL As String = ""
            Dim intIDGroup As Integer = 0
            Try



                If xDate = Nothing Then
                    xDate = Date.Now.Date
                End If


                If _IsGroup Then
                    '  Verificamos si el grupo esta asignado directamente a un nodo
                    intIDGroup = _intID
                Else
                    ' Obtenemos el grupo actual del empleado
                    strSQL = "@SELECT# IDGroup FROM sysrovwCurrentEmployeeGroups WHERE " &
                         "sysrovwCurrentEmployeeGroups.Begindate <= " & roTypes.Any2Time(xDate).SQLSmallDateTime & " AND sysrovwCurrentEmployeeGroups.Enddate >= " & roTypes.Any2Time(xDate).SQLSmallDateTime & " AND " &
                         "sysrovwCurrentEmployeeGroups.IDEmployee = " & _intID.ToString

                    intIDGroup = roTypes.Any2Integer(ExecuteScalar(strSQL))

                End If
                strSQL = "@SELECT# *  FROM sysroSecurityNode_Groups WHERE IDGroup= " & intIDGroup.ToString

                Dim tb As DataTable = CreateDataTable(strSQL)
                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    bolRet = roTypes.Any2Integer(tb.Rows(0).Item("IDSecurityNode"))
                Else
                    ' Si no esta asignado directamente, buscamos si alguno de sus grupos padres está asignado
                    Dim strPath As String = roTypes.Any2String(ExecuteScalar("@SELECT# Path FROM Groups WHERE ID=" & intIDGroup.ToString))
                    ' Obtenemos la definición del grupo
                    If strPath.Length > 0 Then
                        ' Recorremos los grupos del path y buscamos si estan asignados a algun nodo
                        For Each IDParent As String In strPath.Split("\")
                            Dim intNode As Integer = roTypes.Any2Integer(ExecuteScalar("@SELECT# IDSecurityNode FROM sysroSecurityNode_Groups WHERE IDGroup= " & IDParent))
                            If intNode > 0 Then
                                bolRet = intNode
                                Exit For
                            End If
                        Next
                    End If
                End If


            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roGroup::GetSecurityNodeFromGroupOrEmployee")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roGroup::GetSecurityNodeFromGroupOrEmployee")
            Finally

            End Try


            Return bolRet
        End Function

        Public Shared Function SendPinToEmployees(ByVal strEmployeeFilter As String, ByVal Feature As String,
                                                              ByVal strFilters As String, ByVal strFilterUserFields As String, ByRef oState As roGroupState) As Boolean
            Dim bolret As Boolean = True
            Try

                Dim lstEmployees As Generic.List(Of Integer) = roSelector.GetEmployeeList(oState.IDPassport, Feature, "U", Permission.Read,
                                                                                                        strEmployeeFilter, strFilters, strFilterUserFields, False, Nothing, Nothing)

                If lstEmployees IsNot Nothing AndAlso lstEmployees.Count > 0 Then

                    lstEmployees = lstEmployees.Distinct().ToList()

                    Dim sActive As Integer = roTypes.Any2Integer(ExecuteScalar("@SELECT# Data FROM sysroParameters WHERE ID = 'ACTIVE'"))
                    Dim strEmployeeNamesForAudit As String = roBusinessSupport.GetAuditEmployeeNames(lstEmployees, oState)
                    Dim oManager As New roPassportManager(oState.IDPassport)
                    Dim oPassport As roPassport

                    For Each IdEmployee As Integer In lstEmployees
                        Try
                            oPassport = oManager.LoadPassport(IdEmployee, LoadType.Employee)
                        Catch ex As Exception
                            oPassport = Nothing
                        End Try

                        If oPassport IsNot Nothing AndAlso oPassport.AuthenticationMethods IsNot Nothing Then
                            Dim oMethod As roPassportAuthenticationMethodsRow = oPassport.AuthenticationMethods.PinRow
                            Dim r As New Random()
                            Dim strCode As New StringBuilder
                            For i As Integer = 1 To 6
                                strCode.Append(CStr(r.Next(0, 9)))
                            Next

                            Dim bUpdate = False
                            'Create
                            If oMethod Is Nothing Then
                                oMethod = New roPassportAuthenticationMethodsRow

                                oMethod.IDPassport = oPassport.ID
                                oMethod.Method = AuthenticationMethod.Pin
                                oMethod.Version = ""
                                oMethod.Credential = String.Empty
                                oMethod.Password = strCode.ToString
                                oMethod.BiometricID = 0
                                oMethod.Enabled = True

                                oMethod.TimeStamp = Now
                                oMethod.RowState = RowState.NewRow
                                oPassport.AuthenticationMethods.PinRow = oMethod
                            Else
                                'Update
                                bUpdate = True
                                oMethod.Password = strCode.ToString
                                oMethod.Enabled = True
                                oMethod.TimeStamp = Now
                                oMethod.RowState = RowState.UpdateRow
                            End If

                            oManager.Save(oPassport)

                            'Solo enviamos el mail si el servicio esta activo
                            If sActive = 1 OrElse roPassportManager.IsRoboticsUserOrConsultant(oPassport.ID) Then
                                Robotics.DataLayer.AccessHelper.ExecuteSql("@INSERT# INTO sysroNotificationTasks (IDNotification, Key1Numeric, Key2Numeric , Parameters, Key5Numeric) VALUES (1903, " & IdEmployee & ", " & oPassport.ID & ",'" & strCode.ToString & "'," & Convert.ToInt32(AuthenticationMethod.Pin) & ")")
                            End If

                            'Auditoria
                            If strEmployeeNamesForAudit.Length > 0 Then
                                Dim tbParameters As DataTable = oState.CreateAuditParameters()
                                oState.AddAuditParameter(tbParameters, "{EmployeesNames}", strEmployeeNamesForAudit, "", 1)
                                If bUpdate Then
                                    oState.Audit(Audit.Action.aUpdate, Audit.ObjectType.tPassportPin, "", tbParameters, -1)
                                Else
                                    oState.Audit(Audit.Action.aInsert, Audit.ObjectType.tPassportPin, "", tbParameters, -1)
                                End If

                            End If

                        End If
                    Next
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roGroup::SendPinToEmployees")
                bolret = False
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roGroup::SendPinToEmployees")
                bolret = False
            End Try
            Return bolret
        End Function
#End Region

#Region "SaaS Service Administration"

        Public Shared Function ActivateService(ByRef oState As roGroupState) As Boolean
            Dim bolRet As Boolean = True

            Dim bHaveToClose As Boolean = False
            Try
                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                Dim sSql As String = "@SELECT# * FROM sysroPassports_SaaS_Status"
                Dim dt As DataTable = CreateDataTable(sSql)

                If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                    For Each oRow As DataRow In dt.Rows

                        Dim bEnabledDesktop As Integer = 0
                        Dim bEnabledSupervisor As Integer = 0

                        If oRow("EnabledVTDesktop") IsNot DBNull.Value Then
                            If oRow("EnabledVTDesktop") = True Then bEnabledDesktop = 1
                        End If

                        If oRow("EnabledVTSupervisor") IsNot DBNull.Value Then
                            If oRow("EnabledVTSupervisor") = True Then bEnabledSupervisor = 1
                        End If

                        sSql = "@UPDATE# sysroPassports SET EnabledVTDesktop=" & bEnabledDesktop & ",EnabledVTSupervisor=" & bEnabledSupervisor & " WHERE ID = " & oRow("IDPassport")
                        bolRet = ExecuteSql(sSql)

                        sSql = "@DELETE# FROM sysroPassports_SaaS_Status WHERE IDPassport = " & oRow("IDPassport")
                        bolRet = ExecuteSql(sSql)

                        If Not bolRet Then Exit For
                    Next
                End If

                If bolRet Then
                    sSql = "@UPDATE# sysroParameters SET Data = '1' WHERE ID='ACTIVE'"
                    bolRet = ExecuteSql(sSql)

                    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    oState.Audit(Audit.Action.aExecuted, Audit.ObjectType.tSaaSService, "ActivateService", tbParameters, -1)
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roGroup::ActivateService")
                bolRet = False
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roGroup::ActivateService")
                bolRet = False
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet

        End Function

        Public Shared Function CancelService(ByRef oState As roGroupState) As Boolean
            Dim bolRet As Boolean = True

            Dim bHaveToClose As Boolean = False
            Try
                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                Dim sSql As String = $"@SELECT# * FROM sysroPassports WHERE (EnabledVTDesktop = 1 OR EnabledVTSupervisor = 1  AND GroupType <> 'U') 
                                            AND IDGroupFeature not in (@SELECT# ID FROM sysroGroupFeatures WHERE Name like '%@@ROBOTICS@@%')"
                Dim dt As DataTable = CreateDataTable(sSql)

                If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                    For Each oRow As DataRow In dt.Rows

                        Dim bEnabledDesktop As Integer = 0
                        Dim bEnabledSupervisor As Integer = 0

                        If oRow("EnabledVTDesktop") IsNot DBNull.Value Then
                            If oRow("EnabledVTDesktop") = True Then bEnabledDesktop = 1
                        End If

                        If oRow("EnabledVTSupervisor") IsNot DBNull.Value Then
                            If oRow("EnabledVTSupervisor") = True Then bEnabledSupervisor = 1
                        End If

                        sSql = "@INSERT# INTO sysroPassports_SaaS_Status VALUES(" & oRow("ID") & "," & bEnabledDesktop & "," & bEnabledSupervisor & ")"
                        bolRet = ExecuteSql(sSql)

                        sSql = "@UPDATE# sysroPassports SET EnabledVTDesktop=0,EnabledVTSupervisor=0 WHERE ID = " & oRow("ID")
                        bolRet = ExecuteSql(sSql)

                        If Not bolRet Then Exit For
                    Next

                    If bolRet Then
                        sSql = "@UPDATE# sysroParameters SET Data = '0' WHERE ID='ACTIVE'"
                        bolRet = ExecuteSql(sSql)

                        Dim tbParameters As DataTable = oState.CreateAuditParameters()
                        oState.Audit(Audit.Action.aExecuted, Audit.ObjectType.tSaaSService, "CancelService", tbParameters, -1)
                    End If
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roGroup::CancelService")
                bolRet = False
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roGroup::CancelService")
                bolRet = False
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet

        End Function

        Public Shared Function RegenerateAllPasswords(ByRef oState As roGroupState) As Boolean
            Dim bolRet As Boolean = True

            Dim bHaveToClose As Boolean = False
            Try
                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                Dim sActive As Integer = roTypes.Any2Integer(ExecuteScalar("@SELECT# Data FROM sysroParameters WHERE ID = 'ACTIVE'"))

                Dim sSql As String = ""
                sSql = $"@SELECT# * from sysropassports WHERE GroupType <> 'U' 
                                    AND IDGroupFeature not in (@SELECT# ID FROM sysroGroupFeatures WHERE Name like '%@@ROBOTICS@@%')"

                Dim dt As DataTable = CreateDataTable(sSql)
                Dim oManager As New roPassportManager(oState.IDPassport)
                Dim oPassport As roPassport
                Dim oPassportAuthenticationMethods As roPassportAuthenticationMethods
                For Each dRow As DataRow In dt.Rows
                    Dim intIDEmployee As Integer = 0
                    Try
                        If dRow("IDEmployee") IsNot DBNull.Value Then
                            intIDEmployee = roTypes.Any2Integer(dRow("IDEmployee"))
                        End If

                        oPassport = oManager.LoadPassport(dRow("ID"), LoadType.Passport)
                    Catch ex As Exception
                        oPassport = Nothing
                    End Try

                    If oPassport IsNot Nothing Then
                        oPassportAuthenticationMethods = oPassport.AuthenticationMethods
                        If oPassportAuthenticationMethods IsNot Nothing AndAlso oPassportAuthenticationMethods.PasswordRow IsNot Nothing AndAlso Not oPassportAuthenticationMethods.PasswordRow.Credential.Contains("\") Then 'Usuario no se valida mediante Active Directory
                            Dim r As New Random()
                            Dim strCode As String = ""
                            For i As Integer = 1 To 10
                                strCode &= CStr(r.Next(0, 9))
                            Next
                            oPassportAuthenticationMethods.PasswordRow.Password = CryptographyHelper.EncryptWithMD5(strCode)
                            oPassportAuthenticationMethods.PasswordRow.RowState = RowState.UpdateRow
                            oPassportAuthenticationMethods.PasswordRow.LastUpdatePassword = roTypes.CreateDateTime(1900, 1, 1)
                            oPassportAuthenticationMethods.PasswordRow.InvalidAccessAttemps = 0
                            oPassportAuthenticationMethods.PasswordRow.LastDateInvalidAccessAttempted = Nothing

                            oManager.Save(oPassport)

                            'Solo enviamos el mail si el servicio esta activo
                            If roPassportManager.IsRoboticsUserOrConsultant(oPassport.ID) OrElse sActive = 1 Then
                                Robotics.DataLayer.AccessHelper.ExecuteSql("@INSERT# INTO sysroNotificationTasks (IDNotification, Key1Numeric, Key2Numeric, Parameters ) VALUES (1903, " & intIDEmployee & ", " & oPassport.ID & ",'" & CryptographyHelper.Encrypt(strCode) & "')")
                            End If
                        End If

                    End If
                Next

                Dim tbParameters As DataTable = oState.CreateAuditParameters()
                oState.Audit(Audit.Action.aExecuted, Audit.ObjectType.tSaaSService, "RegenerateAllPasswords", tbParameters, -1)
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roGroup::RegenerateAllPasswords")
                bolRet = False
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roGroup::RegenerateAllPasswords")
                bolRet = False
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet

        End Function

        Public Shared Function GetServiceStatus(ByRef serviceStauts As Boolean, ByRef oState As roGroupState) As Boolean
            Dim bolRet As Boolean = False

            Try
                Dim sActive As Integer = roTypes.Any2Integer(ExecuteScalar("@SELECT# Data FROM sysroParameters WHERE ID = 'ACTIVE'"))
                serviceStauts = IIf(sActive = 1, True, False)

                bolRet = True
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roGroup::GetServiceStatus")
                bolRet = False
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roGroup::GetServiceStatus")
                bolRet = False
            End Try

            Return bolRet
        End Function

#End Region

    End Class

    <DataContract>
    Public Class roGroupTreeManager

#Region "Declarations - Constructor"

        Private oState As roGroupState


        Public Sub New()
            Me.oState = New roGroupState(-1)
        End Sub

        Public Sub New(ByVal _State As roGroupState)
            Me.oState = _State
        End Sub

#End Region

#Region "Methods"

        Public Function Load(ByVal oSourceGroup As roGroupTree, ByVal tbGroups As DataTable, ByVal strFeatureAlias As String, ByVal strFilterUserFields As String, ByVal strFeatureType As String) As roGroupTree

            Dim oRet As roGroupTree = oSourceGroup

            Try

                Dim oGroup As roGroupTree = Nothing

                Dim oGroups As DataRow() = tbGroups.Select("Path = '" & oSourceGroup.Path & "\' + ID", "Name ASC")
                For Each oRow As DataRow In oGroups

                    oGroup = New roGroupTree(oRow("ID"), oRow("Name"), oRow("Path"))
                    oGroup = Me.Load(oGroup, tbGroups, strFeatureAlias, strFilterUserFields, strFeatureType)

                    oRet.ChildrenGroups.Add(oGroup)

                Next

                Dim dsEmployees As DataSet = roGroup.GetEmployeesFromGroupWithType(oSourceGroup.ID, strFeatureAlias, strFeatureType, strFilterUserFields, Me.oState)
                If dsEmployees IsNot Nothing AndAlso dsEmployees.Tables.Count > 0 Then

                    Dim tbEmployees As DataTable = dsEmployees.Tables(0)

                    Dim ds As New Data.DataView(tbEmployees)
                    ds.Sort = "EmployeeName"

                    If ds.Count > 0 Then
                        Dim oEmployee As roEmployeeTree = Nothing

                        For Each oDataviewRow As DataRowView In ds

                            oEmployee = New roEmployeeTree(oDataviewRow("IDEmployee"), oDataviewRow("EmployeeName"), oDataviewRow("Type"))

                            oRet.Employees.Add(oEmployee)

                        Next

                    End If

                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roGroupTree::Load")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roGroupTree::Load")
            End Try

            Return oRet
        End Function

#Region "Helper methods"

        Public Shared Function GetTree(ByVal strFilterUserFields As String, ByVal strFeature As String, ByVal strType As String, ByRef oState As roGroupState, Optional sRoot As String = "", Optional ByVal GroupID As String = "") As Generic.List(Of roGroupTree)

            Dim oRet As New Generic.List(Of roGroupTree)

            Try

                Dim dsGroups As DataSet = roGroup.GetGroups(strFeature, strType, oState, sRoot, GroupID)
                If dsGroups IsNot Nothing AndAlso dsGroups.Tables.Count > 0 Then

                    Dim dvGroups As New DataView(dsGroups.Tables(0))
                    If GroupID Is Nothing OrElse GroupID.Trim.Length = 0 Then
                        dvGroups.RowFilter = "Convert([ID], 'System.String') = Path"
                    End If

                    dvGroups.Sort = "Name"

                    If dvGroups.Count > 0 Then
                        Dim oGroupManager As New roGroupTreeManager(oState)
                        For Each oRow As DataRowView In dvGroups
                            ''Dim oRow As DataRowView = dvGroups.Item(0)

                            Dim oGroup As New roGroupTree(oRow("ID"), oRow("Name"), oRow("Path"))

                            oGroup = oGroupManager.Load(oGroup, dsGroups.Tables(0), strFeature, strFilterUserFields, strType)

                            oRet.Add(oGroup)

                        Next

                    End If

                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roGroupTree::GetTree")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roGroupTree::GetTree")
            End Try

            Return oRet

        End Function

#End Region

#End Region

    End Class


End Namespace