Imports System.Data.Common
Imports System.Runtime.Serialization
Imports System.ServiceModel
Imports Robotics.Base.DTOs
Imports Robotics.DataLayer
Imports Robotics.DataLayer.AccessHelper

Namespace VTLiveTasks

    <DataContract>
    Public Class roLiveTask

#Region "Declarations - Constructor"

        Private oState As roLiveTaskState

        Private intID As Integer
        Private intIDPassport As Integer
        Private strAction As String = String.Empty
        Private intStatus As Integer
        Private strErrorCode As String
        Private dateTimeStamp As Nullable(Of DateTime)
        Private dateExecutionDate As Nullable(Of DateTime)
        Private dateEndDate As Nullable(Of DateTime)
        Private intProgress As Integer
        Private strXmlParameters As String
        Private _traceGroup As String
        Private _guid As Nullable(Of Guid)
        Private _isAliveAt As Nullable(Of DateTime)

        Private oCollection As roCollection

        Public Sub New()
            Me.oState = New roLiveTaskState()
            Me.intID = -1
            oCollection = Nothing
            _guid = Nothing
            Me._traceGroup = roTrace.GetInstance().GetCurrentTraceGroup()
        End Sub

        Public Sub New(ByVal _oState As roLiveTaskState)
            Me.oState = _oState
            Me.intID = -1
            oCollection = Nothing
            _guid = Nothing
            Me._traceGroup = roTrace.GetInstance().GetCurrentTraceGroup()
        End Sub

        Public Sub New(ByVal _ID As Integer, ByVal _State As roLiveTaskState, Optional ByVal bAudit As Boolean = False)
            Me.oState = _State
            Me.intID = _ID
            oCollection = Nothing
            Me.Load(bAudit)
            Me._traceGroup = roTrace.GetInstance().GetCurrentTraceGroup()
        End Sub

#End Region

#Region "Properties"

        <IgnoreDataMember()>
        Public Property State() As roLiveTaskState
            Get
                Return Me.oState
            End Get
            Set(ByVal value As roLiveTaskState)
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
        Public Property IDPassport() As Integer
            Get
                Return Me.intIDPassport
            End Get
            Set(ByVal value As Integer)
                Me.intIDPassport = value
            End Set
        End Property

        <DataMember()>
        Public Property Action() As String
            Get
                Return Me.strAction
            End Get
            Set(ByVal value As String)
                Me.strAction = value
            End Set
        End Property

        <DataMember()>
        Public Property Parameters() As roCollection
            Get
                Return oCollection
            End Get
            Set(ByVal value As roCollection)
                Me.oCollection = value
            End Set
        End Property

        <DataMember()>
        Public Property Status() As Integer
            Get
                Return Me.intStatus
            End Get
            Set(ByVal value As Integer)
                Me.intStatus = value
            End Set
        End Property

        <DataMember()>
        Public Property ErrorCode() As String
            Get
                Return Me.strErrorCode
            End Get
            Set(value As String)
                Me.strErrorCode = value
            End Set
        End Property

        <DataMember()>
        Public Property TimeStamp() As Nullable(Of DateTime)
            Get
                Return Me.dateTimeStamp
            End Get
            Set(ByVal value As Nullable(Of DateTime))
                Me.dateTimeStamp = value
            End Set
        End Property

        <DataMember()>
        Public Property IsAliveAt() As Nullable(Of DateTime)
            Get
                Return Me._isAliveAt
            End Get
            Set(ByVal value As Nullable(Of DateTime))
                Me._isAliveAt = value
            End Set
        End Property

        <DataMember()>
        Public Property ExecutionDate() As Nullable(Of DateTime)
            Get
                Return Me.dateExecutionDate
            End Get
            Set(ByVal value As Nullable(Of DateTime))
                Me.dateExecutionDate = value
            End Set
        End Property

        <DataMember()>
        Public Property EndDate() As Nullable(Of DateTime)
            Get
                Return Me.dateEndDate
            End Get
            Set(ByVal value As Nullable(Of DateTime))
                Me.dateEndDate = value
            End Set
        End Property

        <DataMember()>
        Public Property Progress() As Integer
            Get
                Return Me.intProgress
            End Get
            Set(ByVal value As Integer)
                Me.intProgress = value
            End Set
        End Property

        <DataMember()>
        Public Property GUID() As Nullable(Of Guid)
            Get
                Return Me._guid
            End Get
            Set(ByVal value As Nullable(Of Guid))
                Me._guid = value
            End Set
        End Property

        <DataMember()>
        Public Property XmlParameters() As String
            Get
                Return Me.strXmlParameters
            End Get
            Set(ByVal value As String)
                Me.strXmlParameters = value
            End Set
        End Property

        <DataMember()>
        Public Property TraceGroup() As String
            Get
                Return Me._traceGroup
            End Get
            Set(ByVal value As String)
                Me._traceGroup = value
            End Set
        End Property

#End Region

#Region "Methods"

        Public Function Load(Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Try

                Dim strSQL As String = "@SELECT# * FROM sysroLiveTasks WITH (nolock) " &
                                   "WHERE [ID] = " & Me.intID.ToString
                Dim tb As DataTable = CreateDataTable(strSQL)
                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then

                    Dim oRow As DataRow = tb.Rows(0)

                    If Not IsDBNull(oRow("IDPassport")) Then Me.intIDPassport = oRow("IDPassport")
                    If Not IsDBNull(oRow("Action")) Then Me.strAction = oRow("Action")
                    If Not IsDBNull(oRow("Parameters")) Then
                        Me.oCollection = BuildFromXML(oRow("Parameters"))
                        Me.strXmlParameters = oRow("Parameters")
                    End If

                    If Not IsDBNull(oRow("Status")) Then Me.intStatus = oRow("Status")

                    If Not IsDBNull(oRow("ErrorCode")) Then
                        Me.strErrorCode = oRow("ErrorCode")
                    Else
                        Me.strErrorCode = ""
                    End If

                    If Not IsDBNull(oRow("TimeStamp")) Then
                        Me.dateTimeStamp = oRow("TimeStamp")
                    Else
                        Me.dateTimeStamp = Nothing
                    End If

                    Try
                        If Not IsDBNull(oRow("IsAliveAt")) Then
                            Me._isAliveAt = oRow("IsAliveAt")
                        Else
                            Me._isAliveAt = Nothing
                        End If
                    Catch ex As Exception
                        Me._isAliveAt = Nothing
                    End Try

                    If Not IsDBNull(oRow("ExecutionDate")) Then
                        Me.dateExecutionDate = oRow("ExecutionDate")
                    Else
                        Me.dateExecutionDate = Nothing
                    End If

                    If Not IsDBNull(oRow("EndDate")) Then
                        Me.dateEndDate = oRow("EndDate")
                    Else
                        Me.dateEndDate = Nothing
                    End If

                    If Not IsDBNull(oRow("Progress")) Then
                        Me.Progress = oRow("Progress")
                    Else
                        Me.Progress = 0
                    End If

                    If Not IsDBNull(oRow("GUID")) Then
                        Me._guid = System.Guid.Parse(oRow("GUID"))
                    Else
                        Me._guid = System.Guid.NewGuid()
                    End If

                    If Not IsDBNull(oRow("TraceGroup")) Then
                        Me._traceGroup = roTypes.Any2String(oRow("TraceGroup"))
                    Else
                        Me._traceGroup = String.Empty
                    End If

                    bolRet = True
                End If

                ' Auditar lectura
                If bolRet AndAlso bAudit Then
                    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    oState.AddAuditParameter(tbParameters, "{Action}", Me.strAction, "", 1)
                    Me.oState.Audit("", Audit.Action.aSelect, Audit.ObjectType.tLiveTask, Me.strAction, tbParameters, -1)
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roLiveTask::Load")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roLiveTask::Load")
            End Try

            Return bolRet

        End Function

        Public Function Save(Optional ByVal oTaskConnection As roBaseConnection = Nothing, Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Try

                If Me.Validate() Then
                    Dim bTaskDoesNotExist As Boolean = False

                    Dim oAuditDataOld As DataRow = Nothing
                    Dim oAuditDataNew As DataRow = Nothing

                    Dim bolIsNew As Boolean = False

                    Dim tb As New DataTable("sysroLiveTasks")
                    Dim strSQL As String = "@SELECT# * FROM sysroLiveTasks WITH (nolock) WHERE ID = " & Me.intID.ToString
                    Dim cmd As DbCommand = CreateCommand(strSQL, oTaskConnection)
                    Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)
                    da.Fill(tb)

                    Dim oRow As DataRow
                    If tb.Rows.Count = 0 Then
                        If Me.intID > 0 Then bTaskDoesNotExist = True

                        oRow = tb.NewRow
                        bolIsNew = True
                    Else
                        oRow = tb.Rows(0)
                        oAuditDataOld = VTBase.Extensions.roAudit.CloneRow(oRow)
                    End If


                    If bTaskDoesNotExist Then Return True

                    oRow("IDPassport") = Me.intIDPassport
                    oRow("Action") = Me.strAction
                    oRow("Parameters") = oCollection.XML
                    oRow("Status") = Me.intStatus

                    If Me.strErrorCode IsNot Nothing Then
                        oRow("ErrorCode") = Me.strErrorCode
                    Else
                        oRow("ErrorCode") = DBNull.Value
                    End If

                    Try
                        Me._isAliveAt = DateTime.Now
                        oRow("IsAliveAt") = DateTime.Now
                    Catch ex As Exception

                    End Try

                    If bolIsNew Then
                        oRow("TimeStamp") = DateTime.Now
                    Else
                        If Me.dateTimeStamp.HasValue Then
                            oRow("TimeStamp") = Me.dateTimeStamp
                        Else
                            oRow("TimeStamp") = DBNull.Value
                        End If
                    End If

                    If Me.dateExecutionDate.HasValue Then
                        oRow("ExecutionDate") = Me.dateExecutionDate
                    Else
                        oRow("ExecutionDate") = DBNull.Value
                    End If

                    If Me.dateEndDate.HasValue Then
                        oRow("EndDate") = Me.dateEndDate
                    Else
                        oRow("EndDate") = DBNull.Value
                    End If

                    oRow("Progress") = IIf(Me.intProgress > 100, 100, Me.intProgress)

                    If _guid Is Nothing Then _guid = System.Guid.NewGuid
                    oRow("GUID") = _guid.ToString()

                    oRow("TraceGroup") = Me._traceGroup

                    If tb.Rows.Count = 0 Then
                        tb.Rows.Add(oRow)
                    End If
                    da.Update(tb)

                    If Me.ID <= 0 Then
                        Dim tmpIdTable As DataTable = CreateDataTable("@SELECT# TOP 1 [ID] FROM sysroLiveTasks WITH (nolock) WHERE GUID ='" & _guid.ToString() & "' ORDER BY [ID] DESC", oTaskConnection)
                        If tmpIdTable IsNot Nothing AndAlso tmpIdTable.Rows.Count = 1 Then
                            Me.ID = tmpIdTable.Rows(0)("ID")
                        End If
                    End If

                    oAuditDataNew = oRow
                    bolRet = True

                    If bolRet AndAlso bAudit Then
                        ' Auditamos
                        Dim tbAuditParameters As DataTable = VTBase.Extensions.roAudit.CreateParametersTable()
                        VTBase.Extensions.roAudit.AddFieldsValues(tbAuditParameters, oAuditDataNew, oAuditDataOld)
                        Dim oAuditAction As Audit.Action = IIf(oAuditDataOld Is Nothing, Audit.Action.aInsert, Audit.Action.aUpdate)
                        Dim strObjectName As String
                        If oAuditAction = Audit.Action.aInsert Then
                            strObjectName = oAuditDataNew("Action")
                        Else
                            strObjectName = oAuditDataOld("Action")
                        End If
                        bolRet = Me.oState.Audit("", oAuditAction, Audit.ObjectType.tLiveTask, strObjectName, tbAuditParameters, -1)
                    End If
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roLiveTask::Save:Action=" & Me.strAction)
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roLiveTask::Save:Action=" & Me.strAction)
            End Try

            Return bolRet

        End Function

        Public Function UpdateStatus(ByVal iNewState As Integer, Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Try

                Try
                    Dim strSQL As String = "@UPDATE# sysroLiveTasks SET Status = " & iNewState & ", IsAliveAt = GETDATE() WHERE ID = " & Me.intID.ToString
                    bolRet = DataLayer.AccessHelper.ExecuteSql(strSQL)
                Catch ex As Exception
                    Dim strSQL As String = "@UPDATE# sysroLiveTasks SET Status = " & iNewState & " WHERE ID = " & Me.intID.ToString
                    bolRet = DataLayer.AccessHelper.ExecuteSql(strSQL)
                End Try
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roLiveTask::UpdateStatus")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roLiveTask::UpdateStatus")
            End Try

            Return bolRet

        End Function

        Public Function DeleteIfExists(Optional ByVal oTaskConnection As roBaseConnection = Nothing, Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = True

            Try

                Dim bolIsNew As Boolean = False

                ' Optimización pendiente
                ' 0.- Si ya existe una AllTerminals (<Item key="AllTerminals" type="8">true</Item>, no creo ninguna que venga con un IDTerminal (Accediendo a propiedades de Me.Parameters
                ' 1.- Si ya existe una RESET_TERMINAL para el mismo terminal (<Item key="IDTerminal" type="2">xxxx</Item>), no creo tarea

                Dim strSQL As String = "@DELETE# sysroLiveTasks WHERE Action LIKE '" & Me.Action.ToUpper.ToString & "%' AND Status = 0 AND Parameters = '" & Me.Parameters.XML & "'"
                bolRet = DataLayer.AccessHelper.ExecuteSql(strSQL, oTaskConnection)
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roLiveTask::Exits")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roLiveTask::Exits")
            End Try

            Return bolRet

        End Function

        Public Function Exists(Optional ByVal oTaskConnection As roBaseConnection = Nothing, Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Try
                Dim bolIsNew As Boolean = False

                ' Optimización pendiente
                ' 0.- Si ya existe una AllTerminals (<Item key="AllTerminals" type="8">true</Item>, no creo ninguna que venga con un IDTerminal (Accediendo a propiedades de Me.Parameters
                ' 1.- Si ya existe una RESET_TERMINAL para el mismo terminal (<Item key="IDTerminal" type="2">xxxx</Item>), no creo tarea

                Dim tb As New DataTable("sysroLiveTasks")
                Dim strSQL As String = "@SELECT# * FROM sysroLiveTasks WITH (nolock) WHERE Action LIKE '" & Me.Action.ToUpper.ToString & "%' AND Status = 0 AND Parameters = '" & Me.Parameters.XML & "'"
                Dim cmd As DbCommand = CreateCommand(strSQL, oTaskConnection)
                Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)
                da.Fill(tb)

                bolRet = (tb.Rows.Count > 0)
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roLiveTask::Exits")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roLiveTask::Exits")
            End Try

            Return bolRet

        End Function

        Public Function DeleteOldTasks(Optional ByVal oTaskConnection As roBaseConnection = Nothing, Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = True
            Try

                'When deleteing a task, depending the type we delete by ID, or by type, all the tasks created before the one executed.
                Dim bDeleteByType As Boolean = False

                Dim eAction As roLiveTaskTypes = [Enum].Parse(GetType(roLiveTaskTypes), Me.Action, True)

                Select Case eAction
                    Case roLiveTaskTypes.PunchConnectorTask,
                         roLiveTaskTypes.SendNotifications, roLiveTaskTypes.GenerateNotifications, roLiveTaskTypes.CheckConcurrenceData,
                         roLiveTaskTypes.GenerateAnalyticsTasks, roLiveTaskTypes.GenerateDatalinkTasks, roLiveTaskTypes.GenerateReportsDxTasks, roLiveTaskTypes.CheckInvalidEntries,
                         roLiveTaskTypes.ManageVisits, roLiveTaskTypes.SynchronizeTerminals, roLiveTaskTypes.CheckCloseDate,
                         roLiveTaskTypes.DeleteOldAudit, roLiveTaskTypes.ValidityDocuments, roLiveTaskTypes.DocumentTracking,
                         roLiveTaskTypes.DeleteOldPhotos, roLiveTaskTypes.DeleteOldPunches, roLiveTaskTypes.DeleteOldBiometricData,
                         roLiveTaskTypes.PurgeNotifications, roLiveTaskTypes.DeleteOldDocuments, roLiveTaskTypes.DeleteAccessMovesHistory,
                         roLiveTaskTypes.CheckScheduleRulesFaults, roLiveTaskTypes.RemoveExpiredTasks, roLiveTaskTypes.BlockInactivePassports,
                         roLiveTaskTypes.DeleteOldComplaints, roLiveTaskTypes.DataMonitoring, roLiveTaskTypes.Suprema
                        bDeleteByType = True
                    Case Else
                        bDeleteByType = False
                End Select

                Dim strSQL As String = String.Empty

                If bDeleteByType Then
                    strSQL = "@DELETE# FROM sysroLiveTasks WHERE Action = '" & eAction.ToString.ToUpper & "' And Status <> 3  And DATEAdd(minute,15,IsAliveAt) < GETDATE()"
                    bolRet = ExecuteSql(strSQL, oTaskConnection)
                Else
                    bolRet = True
                End If
            Catch ex As DbException
                'Me.oState.UpdateStateInfo(ex, "roLiveTask::DeleteOldTasks")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roLiveTask::DeleteOldTasks")
            End Try

            Return bolRet

        End Function

        Public Function Delete(Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = True

            Try
                Try

                    If Me.Status = 2 AndAlso Me.ErrorCode <> String.Empty Then
                        If Me.Action = roLiveTaskTypes.AnalyticsTask.ToString().ToUpper Then
                            If Me.Parameters.Item("DownloadBI") Is Nothing OrElse Me.Parameters.Item("DownloadBI").ToString = "0" Then
                                If Robotics.Azure.RoAzureSupport.DeleteFileFromAzure(Me.ErrorCode.Replace((Azure.RoAzureSupport.GetCompanyName() & "/"), ""), roLiveQueueTypes.analytics) Then
                                    Dim executionSQL As String = "@DELETE# FROM GeniusExecutions WHERE IdTask = " & Me.ID
                                    ExecuteSql(executionSQL)
                                End If
                            Else
                                If Me.Parameters.Item("DownloadBI") IsNot Nothing AndAlso Me.Parameters.Item("DownloadBI").ToString = "1" Then
                                    If Robotics.Azure.RoAzureSupport.DeleteFileFromCompanyContainer(Me.ErrorCode.Replace((Azure.RoAzureSupport.GetCompanyName() & "/"), ""), "", roLiveQueueTypes.analyticsbi) Then
                                        Dim executionSQL As String = "@DELETE# FROM GeniusExecutions WHERE IdTask = " & Me.ID
                                        ExecuteSql(executionSQL)
                                    End If
                                End If
                            End If
                        ElseIf Me.Action = roLiveTaskTypes.ReportTaskDX.ToString().ToUpper Then

                            Dim strGuidExecution As String = roTypes.Any2String(Me.Parameters("Guid"))
                            Dim idReport As Integer = roTypes.Any2Integer(Me.ErrorCode)

                            Dim executionSQL As String = "@SELECT# ISNULL(FileLink,'') FROM ReportExecutions WHERE LayoutID = " & idReport & " AND Guid='" & strGuidExecution & "'"
                            Dim reportFileName As String = roTypes.Any2String(ExecuteScalar(executionSQL))

                            If reportFileName <> String.Empty Then
                                Robotics.Azure.RoAzureSupport.DeleteFileFromAzure(reportFileName, roLiveQueueTypes.reports)
                            End If

                            executionSQL = "@DELETE# FROM ReportExecutions WHERE LayoutID = " & idReport & " AND Guid='" & strGuidExecution & "'"
                            ExecuteSql(executionSQL)

                        ElseIf Me.Action = roLiveTaskTypes.Export.ToString().ToUpper Then
                            Robotics.Azure.RoAzureSupport.DeleteFileFromAzure(Me.ErrorCode.Replace((Azure.RoAzureSupport.GetCompanyName() & "/"), ""), roLiveQueueTypes.analytics)
                        End If

                    End If
                Catch ex As Exception
                    Me.oState.UpdateStateInfo(ex, "roLiveTask::Delete::Can not delete related file")
                End Try

                'When deleteing a task, depending the type we delete by ID, or by type, all the tasks created before the one executed.
                Dim bDeleteByType As Boolean = False

                Dim eAction As roLiveTaskTypes = [Enum].Parse(GetType(roLiveTaskTypes), Me.Action, True)

                Select Case eAction
                    Case roLiveTaskTypes.PunchConnectorTask,
                         roLiveTaskTypes.SendNotifications, roLiveTaskTypes.GenerateNotifications,
                         roLiveTaskTypes.GenerateAnalyticsTasks, roLiveTaskTypes.GenerateDatalinkTasks, roLiveTaskTypes.GenerateReportsDxTasks, roLiveTaskTypes.CheckInvalidEntries,
                         roLiveTaskTypes.ManageVisits, roLiveTaskTypes.SynchronizeTerminals, roLiveTaskTypes.CheckCloseDate,
                         roLiveTaskTypes.DeleteOldAudit, roLiveTaskTypes.ValidityDocuments, roLiveTaskTypes.DocumentTracking,
                         roLiveTaskTypes.DeleteOldPhotos, roLiveTaskTypes.DeleteOldPunches, roLiveTaskTypes.DeleteOldBiometricData,
                         roLiveTaskTypes.PurgeNotifications, roLiveTaskTypes.DeleteOldDocuments, roLiveTaskTypes.DeleteAccessMovesHistory,
                         roLiveTaskTypes.CheckScheduleRulesFaults, roLiveTaskTypes.RemoveExpiredTasks, roLiveTaskTypes.BlockInactivePassports, roLiveTaskTypes.DeleteOldComplaints, roLiveTaskTypes.DataMonitoring
                        bDeleteByType = True
                    Case Else
                        bDeleteByType = False
                End Select

                Dim strSQL As String = String.Empty

                If bDeleteByType Then
                    strSQL = "@DELETE# FROM sysroLiveTasks WHERE Action = '" & eAction.ToString.ToUpper & "' And Status <> 3 "
                Else
                    strSQL = "@DELETE# FROM sysroLiveTasks WHERE ID = " & Me.intID
                End If

                bolRet = ExecuteSql(strSQL)

                If bolRet AndAlso bAudit Then
                    ' Auditamos
                    bolRet = Me.oState.Audit("", Audit.Action.aDelete, Audit.ObjectType.tLiveTask, Me.strAction, Nothing, -1)
                End If
            Catch ex As DbException
                'Me.oState.UpdateStateInfo(ex, "roLiveTask::Delete")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roLiveTask::Delete")
            End Try

            Return bolRet

        End Function

        Public Function Validate() As Boolean

            Dim bolRet As Boolean = True

            Try
                If Me.strAction = Nothing Or Me.strAction = String.Empty Then
                    bolRet = False
                    oState.Result = LiveTasksResultEnum.ActionEmpty
                End If

                If oCollection Is Nothing Or oCollection.Count = 0 Then
                    bolRet = False
                    oState.Result = LiveTasksResultEnum.ParametersEmpty
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roLiveTask::Validate")
                bolRet = False
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roLiveTask::Validate")
                bolRet = False
            End Try

            Return bolRet

        End Function

        Public Function BuildFromXML(ByVal strXml As String) As roCollection
            If strXml <> "" Then
                ' Añadimos la composición a la colección
                Dim oCollection As New roCollection(strXml)
                Return oCollection
            Else
                Return Nothing
            End If
        End Function

#End Region

#Region "Create Tasks"

        Public Shared Function CreateEmptyLiveTask(ByVal action As roLiveTaskTypes, ByRef oState As roLiveTaskState, Optional ByVal bSendMessageSync As Boolean = True, Optional ByVal oTaskForceConnection As roBaseConnection = Nothing) As Integer
            Dim iNewTaskInteger As Integer = -1
            Dim bCloseConnection As Boolean = False
            Try
                If action <> roLiveTaskTypes.ReportTask Then
                    Dim oTaskConnection As roBaseConnection = Nothing
                    If oTaskForceConnection IsNot Nothing Then
                        oTaskConnection = oTaskForceConnection
                    Else
                        Dim oTmpConnection As roBaseConnection = roCacheManager.GetInstance.GetConnection()
                        If oTmpConnection IsNot Nothing AndAlso oTmpConnection.GetType = GetType(roConnection) Then
                            oTaskConnection = oTmpConnection
                        Else
                            oTaskConnection = roBaseConnection.ForceNewConnection(Nothing)
                            bCloseConnection = True
                        End If
                    End If

                    Dim oParameters As New roCollection
                    oParameters.Add("emptyTask", True)

                    Dim oNewTask As New roLiveTask(oState)
                    oNewTask.Action = action.ToString().ToUpper()
                    oNewTask.IDPassport = oState.IDPassport
                    oNewTask.Parameters = oParameters
                    oNewTask.Progress = 0

                    Dim bOnlyOneTaskByType As Boolean = False

                    Dim eAction As roLiveTaskTypes = [Enum].Parse(GetType(roLiveTaskTypes), oNewTask.Action, True)

                    Select Case eAction
                        Case roLiveTaskTypes.PunchConnectorTask,
                         roLiveTaskTypes.SendNotifications, roLiveTaskTypes.GenerateNotifications, roLiveTaskTypes.CheckConcurrenceData,
                         roLiveTaskTypes.GenerateAnalyticsTasks, roLiveTaskTypes.GenerateDatalinkTasks, roLiveTaskTypes.GenerateReportsDxTasks, roLiveTaskTypes.CheckInvalidEntries,
                         roLiveTaskTypes.ManageVisits, roLiveTaskTypes.SynchronizeTerminals, roLiveTaskTypes.CheckCloseDate,
                         roLiveTaskTypes.DeleteOldAudit, roLiveTaskTypes.ValidityDocuments, roLiveTaskTypes.DocumentTracking,
                         roLiveTaskTypes.DeleteOldPhotos, roLiveTaskTypes.DeleteOldPunches, roLiveTaskTypes.DeleteOldBiometricData,
                         roLiveTaskTypes.PurgeNotifications, roLiveTaskTypes.DeleteOldDocuments, roLiveTaskTypes.DeleteAccessMovesHistory,
                         roLiveTaskTypes.CheckScheduleRulesFaults, roLiveTaskTypes.RemoveExpiredTasks, roLiveTaskTypes.BlockInactivePassports,
                         roLiveTaskTypes.DeleteOldComplaints, roLiveTaskTypes.DataMonitoring, roLiveTaskTypes.Suprema
                            bOnlyOneTaskByType = True
                        Case Else
                            bOnlyOneTaskByType = False
                    End Select

                    If action = roLiveTaskTypes.CheckAutomaticRequests Then
                        Dim sql = $"@SELECT# count(ID) from sysrolivetasks where Action = '{roLiveTaskTypes.CheckAutomaticRequests.ToString().ToUpper}' AND Status IN (0,1) AND DATEAdd(minute,{60},IsAliveAt) > GETDATE()"

                        Dim taskCount As Integer = roTypes.Any2Integer(DataLayer.AccessHelper.ExecuteScalar(sql, oTaskConnection))
                        If taskCount > 0 Then
                            Return -1
                        Else
                            bOnlyOneTaskByType = True
                        End If
                    End If



                    'Si solo puede existir una tarea por tipo borramos esta tarea. Borrara de la tabla todas las existentes y creara la nueva
                    If bOnlyOneTaskByType Then oNewTask.DeleteOldTasks(oTaskConnection)

                    If oNewTask.Save(oTaskConnection, False) Then
                        iNewTaskInteger = oNewTask.ID

                        If bSendMessageSync AndAlso Not SendMessageToTask(oNewTask, action, oState, oTaskConnection) Then
                            oNewTask.Delete()
                            iNewTaskInteger = -1
                            oState.Result = LiveTasksResultEnum.TaskNotExecuted
                        End If
                    End If

                    If bCloseConnection AndAlso oTaskConnection IsNot Nothing Then oTaskConnection.CloseIfNeeded()
                Else
                    oState.Result = LiveTasksResultEnum.TaskNotExecuted
                End If
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roLiveTask::CreateEmptyLiveTask")
            End Try

            Return iNewTaskInteger
        End Function

        Public Shared Function CreateReportTask(ByVal intIdReportTask As Integer, ByRef oState As roLiveTaskState, Optional ByVal bSendMessageSync As Boolean = True) As Boolean
            Dim bStarted As Boolean = True

            Dim oNewTask As New roLiveTask(oState)
            oNewTask.ID = intIdReportTask
            oNewTask.Action = roLiveTaskTypes.ReportTask.ToString()

            If bSendMessageSync Then SendMessageToTask(oNewTask, roLiveTaskTypes.ReportTask, oState, Nothing)

            Return bStarted
        End Function

        Public Shared Function CreateLiveTask(ByVal action As roLiveTaskTypes, ByVal oParameters As roCollection, ByRef oState As roLiveTaskState, Optional ByVal bSendMessageSync As Boolean = True, Optional ByVal oTaskForceConnection As roBaseConnection = Nothing) As Integer
            Dim iNewTaskInteger As Integer = -1
            Dim bCloseConnection As Boolean = False
            Try

                ' Miro si debo lanzar las tareas de cálculo de permisos (sólo debo hacerlo en entornos con soprote para entorno Bridge
                If action = roLiveTaskTypes.SecurityPermissions OrElse action = roLiveTaskTypes.ChangeRequestPermissions Then
                    If roTypes.Any2String(roLiveTask.getAdvancedParameterValue("VisualTime.BridgeSupport")) = "0" Then
                        oState.Result = LiveTasksResultEnum.TaskNotExecuted
                        Return iNewTaskInteger
                    End If
                End If

                If action <> roLiveTaskTypes.ReportTask Then
                    Dim oTaskConnection As roBaseConnection = Nothing
                    If oTaskForceConnection IsNot Nothing Then
                        oTaskConnection = oTaskForceConnection
                    Else
                        Dim oTmpConnection As roBaseConnection = roCacheManager.GetInstance.GetConnection()

                        If oTmpConnection IsNot Nothing AndAlso oTmpConnection.GetType = GetType(roConnection) Then
                            oTaskConnection = oTmpConnection
                        Else
                            oTaskConnection = roBaseConnection.ForceNewConnection(Nothing)
                            bCloseConnection = True
                        End If
                    End If

                    Dim oNewTask As roLiveTask

                    ' Sólo guardo una tarea si no existe una equivalente en estado pendiente (0)
                    ' Estas tareas se envía el mensaje automáticamente a los 30 segundos.
                    ' Así conseguimos acumular las tareas de un mismo tipo y no saturar bb.dd.
                    If action = roLiveTaskTypes.BroadcasterTask OrElse action = roLiveTaskTypes.ChangeRequestPermissions OrElse action = roLiveTaskTypes.SecurityPermissions Then

                        If action = roLiveTaskTypes.BroadcasterTask AndAlso Not oParameters.Exists("IDTerminal") Then
                            Dim sSQL As String = " @SELECT# ID FROM Terminals where type not in ('LivePortal' , 'NFC' , 'Virtual', 'masterASP', 'Suprema', 'Time Gate') AND Enabled = 1"
                            Dim dtTerminals As DataTable = AccessHelper.CreateDataTable(sSQL, oTaskConnection)
                            For Each oRow As DataRow In dtTerminals.Rows
                                Try
                                    oParameters = New roCollection
                                    oParameters.Add("IDTerminal", oRow("ID"))

                                    oNewTask = New roLiveTask(oState)
                                    oNewTask.Action = action.ToString().ToUpper()
                                    oNewTask.IDPassport = oState.IDPassport
                                    oNewTask.Parameters = oParameters
                                    oNewTask.Progress = 0
                                    oNewTask.DeleteIfExists(oTaskConnection)

                                    If oNewTask.Save(oTaskConnection, False) Then
                                        iNewTaskInteger = oNewTask.ID
                                    End If
                                Catch ex As Exception
                                    roLog.GetInstance().logMessage(roLog.EventType.roError, "roLiveTask::CreateBroadcasterTask::", ex)
                                End Try
                            Next
                        Else
                            oNewTask = New roLiveTask(oState)
                            oNewTask.Action = action.ToString().ToUpper()
                            oNewTask.IDPassport = oState.IDPassport
                            oNewTask.Parameters = oParameters
                            oNewTask.Progress = 0
                            oNewTask.DeleteIfExists(oTaskConnection)

                            If oNewTask.Save(oTaskConnection, False) Then
                                iNewTaskInteger = oNewTask.ID
                            End If
                        End If
                    Else
                        oNewTask = New roLiveTask(oState)
                        oNewTask.Action = action.ToString().ToUpper()
                        oNewTask.IDPassport = oState.IDPassport
                        oNewTask.Parameters = oParameters
                        oNewTask.Progress = 0
                        Dim bolSaveTask As Boolean = True
                        If action = roLiveTaskTypes.RunEngineEmployee Then
                            If oNewTask.Exists(oTaskConnection) Then bolSaveTask = False
                        End If

                        Dim bOnlyOneTaskByType As Boolean = False

                        Dim eAction As roLiveTaskTypes = [Enum].Parse(GetType(roLiveTaskTypes), oNewTask.Action, True)

                        Select Case eAction
                            Case roLiveTaskTypes.PunchConnectorTask,
                         roLiveTaskTypes.SendNotifications, roLiveTaskTypes.GenerateNotifications, roLiveTaskTypes.CheckConcurrenceData,
                         roLiveTaskTypes.GenerateAnalyticsTasks, roLiveTaskTypes.GenerateDatalinkTasks, roLiveTaskTypes.GenerateReportsDxTasks, roLiveTaskTypes.CheckInvalidEntries,
                         roLiveTaskTypes.ManageVisits, roLiveTaskTypes.SynchronizeTerminals, roLiveTaskTypes.CheckCloseDate,
                         roLiveTaskTypes.DeleteOldAudit, roLiveTaskTypes.ValidityDocuments, roLiveTaskTypes.DocumentTracking,
                         roLiveTaskTypes.DeleteOldPhotos, roLiveTaskTypes.DeleteOldPunches, roLiveTaskTypes.DeleteOldBiometricData,
                         roLiveTaskTypes.PurgeNotifications, roLiveTaskTypes.DeleteOldDocuments, roLiveTaskTypes.DeleteAccessMovesHistory,
                         roLiveTaskTypes.CheckScheduleRulesFaults, roLiveTaskTypes.RemoveExpiredTasks, roLiveTaskTypes.BlockInactivePassports,
                         roLiveTaskTypes.DeleteOldComplaints, roLiveTaskTypes.DataMonitoring, roLiveTaskTypes.Suprema
                                bOnlyOneTaskByType = True
                            Case Else
                                bOnlyOneTaskByType = False
                        End Select


                        If action = roLiveTaskTypes.CheckAutomaticRequests Then
                            Dim sql = $"@SELECT# count(ID) from sysrolivetasks where Action = '{roLiveTaskTypes.CheckAutomaticRequests.ToString().ToUpper}' AND Status IN (0,1) AND DATEAdd(minute,{60},IsAliveAt) > GETDATE()"

                            Dim taskCount As Integer = roTypes.Any2Integer(DataLayer.AccessHelper.ExecuteScalar(sql, oTaskConnection))
                            If taskCount > 0 Then
                                Return -1
                            Else
                                bOnlyOneTaskByType = True
                            End If
                        End If


                        'Si solo puede existir una tarea por tipo borramos esta tarea. Borrara de la tabla todas las existentes y creara la nueva
                        If bOnlyOneTaskByType Then oNewTask.DeleteOldTasks(oTaskConnection)

                        If bolSaveTask AndAlso oNewTask.Save(oTaskConnection, False) Then
                            iNewTaskInteger = oNewTask.ID
                            If bSendMessageSync Then SendMessageToTask(oNewTask, action, oState, oTaskConnection)
                        End If

                        If bCloseConnection AndAlso oTaskConnection IsNot Nothing Then oTaskConnection.CloseIfNeeded()
                    End If
                Else
                    oState.Result = LiveTasksResultEnum.TaskNotExecuted
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roLiveTask::CreateLiveTask")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roLiveTask::CreateLiveTask")
            End Try

            Return iNewTaskInteger
        End Function

#End Region

#Region "Helper methods"

        Shared Function GetReportsTaksStatus(ByVal oTaskStatus As roLiveTaskStatus, ByVal onlyUser As Boolean, oState As roLiveTaskState) As DataTable
            Dim tbRet As DataTable = Nothing

            Try
                Dim strWhere As String = ""
                If onlyUser Then
                    strWhere = " AND IDPassport = " & oState.IDPassport
                End If

                Select Case oTaskStatus
                    Case roLiveTaskStatus.Stopped
                        strWhere &= " AND [Status] = 0 "
                    Case roLiveTaskStatus.Running
                        strWhere &= " AND [Status] = 1 "
                    Case roLiveTaskStatus.Finished
                        strWhere &= " AND [Status] > 1 "
                End Select

                Dim strSQL As String = "@SELECT# ROW_NUMBER() OVER(ORDER BY [Status] DESC) AS ID, tmp.* from (" &
                                    " @SELECT# ID AS TaskID, [Action], [Parameters] AS Name,ErrorCode As [FileName], [Status], IDPassport, [TimeStamp], ExecutionDate, EndDate FROM sysroLiveTasks WHERE Action IN('IMPORT','EXPORT', 'ANALYTICSTASK', 'ANALYTICSTASKV2','REPORTTASKDX') AND ERRORCODE NOT LIKE '%AuditAnalytics%' " & strWhere
                strSQL &= " )tmp"

                If oTaskStatus = roLiveTaskStatus.Finished Then
                    strSQL &= " ORDER BY TimeStamp DESC"
                End If

                tbRet = CreateDataTable(strSQL)
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roIndicator::GetReportsTaksStatus")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roIndicator::GetReportsTaksStatus")
            End Try

            Return tbRet

        End Function

        Shared Function DeleteLiveTask(ByVal IDTask As Integer, ByVal Action As String, ByRef oState As roLiveTaskState) As Boolean

            Dim oRet As Boolean = False

            Try
                Dim strSQL As String

                Dim strAction As String = String.Empty

                Select Case Action.ToUpper()

                    Case "ANALYTICSTASK"
                        strAction = "3"
                        strSQL = "@SELECT# DISTINCT [ID] FROM sysroLiveTasks WHERE ID=" & IDTask & " AND Status=0 AND Action='AnalyticsTask' "
                        Dim tbMoves As DataTable = CreateDataTable(strSQL)
                        If tbMoves IsNot Nothing Then
                            For Each oRow As DataRow In tbMoves.Rows
                                strSQL = "@DELETE# FROM sysroLiveTasks WHERE ID = " & IDTask
                                ExecuteSql(strSQL)
                            Next
                        End If

                        oRet = True
                    Case "ANALYTICSTASKV2"
                        strAction = "3"
                        strSQL = "@SELECT# DISTINCT [ID] FROM sysroLiveTasks WHERE ID=" & IDTask & " AND Status=0 AND Action='AnalyticsTaskV2' "
                        Dim tbMoves As DataTable = CreateDataTable(strSQL)
                        If tbMoves IsNot Nothing Then
                            For Each oRow As DataRow In tbMoves.Rows
                                strSQL = "@DELETE# FROM sysroLiveTasks WHERE ID = " & IDTask
                                ExecuteSql(strSQL)
                            Next
                        End If

                        oRet = True

                    Case "REPORT"
                        strAction = "1"
                        strSQL = "@SELECT# DISTINCT [ID] FROM sysroReportTasks  WHERE ID=" & IDTask & " AND Status=0"
                        Dim tbMoves As DataTable = CreateDataTable(strSQL)
                        If tbMoves IsNot Nothing Then
                            For Each oRow As DataRow In tbMoves.Rows
                                strSQL = "@DELETE# FROM sysroReportTasks WHERE ID = " & IDTask
                                ExecuteSql(strSQL)
                            Next
                        End If

                        oRet = True

                    Case Else
                        oRet = False
                End Select

                If oRet Then
                    Dim pipeFactory As New ChannelFactory(Of IBackgroundMessage)(New NetNamedPipeBinding(), New EndpointAddress("net.pipe://localhost/roDataHubTask"))
                    Dim pipeProxy As IBackgroundMessage = pipeFactory.CreateChannel()
                    pipeProxy.DeleteTask(strAction & "@" & IDTask.ToString)
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roLiveTask::DeleteLiveTask")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roLiveTask::DeleteLiveTask")
            End Try

            Return oRet
        End Function

        Public Shared Function getAdvancedParameterValue(ByVal parameterName As String) As String
            Dim strValue As String = String.Empty

            Try
                Dim strSQL As String = "@SELECT# Value FROM sysroLiveAdvancedParameters WHERE ParameterName = '" & parameterName & "'"
                strValue = roTypes.Any2String(Robotics.DataLayer.AccessHelper.ExecuteScalar(strSQL))
            Catch ex As Exception
            End Try

            Return strValue
        End Function

        Shared Function IsTaskTypeAlreadyRunning(ByVal action As String, ByVal excludeIDTask As Integer) As Boolean

            Try
                Dim strCheckIsRunning As String = ""


                If excludeIDTask <> -1 Then
                    strCheckIsRunning = $"@SELECT# 
                                            count(*) as Total
                                        FROM 
                                            sysroLiveTasks with (nolock)
                                        WHERE Action = '{action}' AND Status IN (0,1) AND ID <> {excludeIDTask} AND DATEAdd(minute,30,IsAliveAt) > GETDATE()"
                Else
                    strCheckIsRunning = $"@SELECT# 
                                            count(*) as Total
                                        FROM 
                                            sysroLiveTasks with (nolock)
                                        WHERE Action = '{action}' AND Status IN (0,1) AND DATEAdd(minute,30,IsAliveAt) > GETDATE()"
                End If

                If roTypes.Any2Integer(DataLayer.AccessHelper.ExecuteScalar(strCheckIsRunning)) > 0 Then
                    Return True
                Else
                    Return False
                End If

            Catch ex As DbException
                roLog.GetInstance().logMessage(roLog.EventType.roError, "roLiveTask::IsTaskAlreadyRunning", ex)
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "roLiveTask::IsTaskAlreadyRunning", ex)
            End Try

            Return False
        End Function

#End Region

#Region "Send Message"

        Public Shared Function SendMessageToTask(ByVal oNewTask As roLiveTask, ByVal action As roLiveTaskTypes, ByRef oState As roLiveTaskState, ByVal oTaskForceConnection As roBaseConnection) As Boolean
            Return SendMessageToTask(oNewTask.ID, oNewTask.Action.ToString, oState, oTaskForceConnection)
        End Function

        Private Shared Function SendMessageToTask(ByVal oTaskId As Integer, ByVal taskAction As String, ByRef oState As roLiveTaskState, ByVal oTaskForceConnection As roBaseConnection) As Boolean
            Dim bRet As Boolean
            Try
                Dim action As roLiveTaskTypes = [Enum].Parse(GetType(roLiveTaskTypes), taskAction, True)

                Select Case action
                    'Tareas que no existen en HA
                    Case roLiveTaskTypes.CustomExport, roLiveTaskTypes.StartPunchConnector, roLiveTaskTypes.StopPunchConnector,
                         roLiveTaskTypes.ExportPunch, roLiveTaskTypes.ReportTask
                        bRet = False
                    Case Else
                        bRet = Azure.RoAzureSupport.SendTaskToQueue(oTaskId, Azure.RoAzureSupport.GetCompanyName, action)

                        If bRet Then
                            DataLayer.AccessHelper.ExecuteSql("@UPDATE# sysroLiveTasks SET IsAliveAt = getdate() where id = " & oTaskId, oTaskForceConnection)
                        End If
                End Select
            Catch ex As Exception
                bRet = False
                oState.Result = LiveTasksResultEnum.TaskNotExecuted
            End Try

            Return bRet
        End Function

#End Region

    End Class

End Namespace