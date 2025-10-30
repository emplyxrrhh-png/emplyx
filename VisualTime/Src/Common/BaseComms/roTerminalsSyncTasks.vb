Imports System.Text
Imports System.Windows.Forms
Imports Robotics.DataLayer
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.VTBase

Public Class roTerminalsSyncTasks

    Public Enum SyncActions
        none
        addemployee
        delemployee
        delallemployees
        getallemployees
        addcard
        delcard
        delallcards
        addbio
        delbio
        getbio
        delallbios
        addphoto
        delphoto
        delallphotos
        adddocument
        deldocument
        delalldocuments
        addemployeegroup
        delemployeegroup
        delallemployeegroup
        addface
        delface
        delallface
        addtimezone
        deltimezone
        delalltimezones
        addemployeeaccesslevel
        delemployeeaccesslevel
        delallemployeeaccesslevel
        getallemployeeaccesslevel
        getallholidays
        getalltimezones
        getalltransactions
        getallfingerprints
        setterminalconfig
        getterminalconfig
        cleardata
        reboot
        addemployeetimezones
        delemployeetimezones
        delallemployeetimezones
        check
        info
        open
        log
        getnewtransactions
        setsirens
        delsirens
        getnetworkinfo
        setcauses
        delallcauses
        updatefirmware
        addbiodataface
        delbiodataface
        delallbiodataface
        addbiodatapalm
        delbiodatapalm
        delallbiodatapalm
        settimezones
        setaccess
        shell
        refreshallbios
    End Enum

    Public Enum eDbWorkMode
        Direct
        Batch
    End Enum

    Private _task As SyncActions
    Private _IDTerminal As Integer
    Private _IDEmployee As Integer
    Private _IDFinger As Byte
    Private _Parameter1 As Integer
    Private _Parameter2 As Integer
    Private _TaskData As String
    Private _ID As Integer
    Private _Retries As Integer
    Private _MaxSyncTasksRetries As Integer
    Private insertTasksList As New List(Of String)
    Private deleteTasksList As New List(Of String)
    Private dbworkMode As eDbWorkMode = eDbWorkMode.Direct
    Private batchSizeOnBatchMode As Integer = 500

#Region "Propiedades"

    Public ReadOnly Property Task() As SyncActions
        Get
            Return _task
        End Get
    End Property

    Public ReadOnly Property IDEmployee() As Integer
        Get
            Return _IDEmployee
        End Get
    End Property

    Public ReadOnly Property IDFinger() As Byte
        Get
            Return _IDFinger
        End Get
    End Property

    Public ReadOnly Property Parameter1() As Integer
        Get
            Return _Parameter1
        End Get
    End Property

    Public ReadOnly Property Parameter2() As Integer
        Get
            Return _Parameter2
        End Get
    End Property

    Public ReadOnly Property TaskData() As String
        Get
            Return _TaskData
        End Get
    End Property

    Public ReadOnly Property ID As Integer
        Get
            Return _ID
        End Get
    End Property

    Public ReadOnly Property Retries As Integer
        Get
            Return _Retries
        End Get
    End Property

    Public ReadOnly Property WorkMode As eDbWorkMode
        Get
            Return dbworkMode
        End Get
    End Property

    Public Sub SetBatchSize(value As Integer)
        If value > 0 Then
            batchSizeOnBatchMode = value
            dbworkMode = eDbWorkMode.Batch
        Else
            dbworkMode = eDbWorkMode.Direct
        End If
    End Sub
#End Region

#Region "Funciones globales"

    Public Sub New(ByVal IDTerminal As Integer)
        Try
            _IDTerminal = IDTerminal
            Dim iMaxSyncTasksRetries As Integer
            iMaxSyncTasksRetries = roTypes.Any2Integer(roCacheManager.GetInstance().GetAdvParametersCache(Azure.RoAzureSupport.GetCompanyName, "Terminals.MaxSyncTasksRetries"))
            _MaxSyncTasksRetries = 32767
            If iMaxSyncTasksRetries >= 100 AndAlso iMaxSyncTasksRetries <= 32767 Then
                _MaxSyncTasksRetries = iMaxSyncTasksRetries
            ElseIf iMaxSyncTasksRetries = 0 Then
                _MaxSyncTasksRetries = 200
            End If

        Catch ex As Exception
            roLog.GetInstance().logMessage(VTBase.roLog.EventType.roError, "roTerminalsSyncTasks::New " + _IDTerminal.ToString + ":: ", ex)
        End Try
    End Sub

#End Region

    Public Sub addSyncTask(ByVal Action As SyncActions, ByVal IDEmployee As Integer, Optional ByVal IDFinger As Integer = 0, Optional ByVal Delay As Integer = 0, Optional ByVal Parameter1 As Integer = 0, Optional ByVal Parameter2 As Integer = 0, Optional TaskData As String = "", Optional bDeleteRelated As Boolean = True)
        Try
            'Se borran todas las tareas anteriores
            If bDeleteRelated Then delSyncTask(Action, IDEmployee, IDFinger, Parameter1, Parameter2)

            Dim strSQL As String
            strSQL = $"@INSERT# INTO TerminalsSyncTasks"
            strSQL += "(IDTerminal,Task,IDEmployee,IDFinger,Parameter1, Parameter2,TaskData,TaskDate)"
            strSQL += " VALUES (" + _IDTerminal.ToString + ", '" + Action.ToString + "', "
            strSQL += IDEmployee.ToString + ", " + IDFinger.ToString + "," + Parameter1.ToString + "," + Parameter2.ToString + ",'" + TaskData.ToString + "',"
            'Si es una instruccion de delall descontamos 5 seg para asegurarnos que se ejecutara antes que las demas.
            If Delay <> 0 Then
                strSQL += "dateadd(s," + Delay.ToString + ",getdate()))"
            Else
                If Action = SyncActions.deldocument Then
                    'Si es un borrado de documents nos aseguramos que se hará antes que la inserción
                    strSQL += "dateadd(s,-5,getdate()))"
                Else
                    strSQL += "getdate())"
                End If
            End If

            Select Case WorkMode
                Case eDbWorkMode.Batch
                    insertTasksList.Add(strSQL)
                Case eDbWorkMode.Direct
                    If ExecuteSql(strSQL) Then
                        roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roTerminalsSyncTasks::addSyncTask::Terminal " + _IDTerminal.ToString + "::Add new task (" + _IDTerminal.ToString + ", " + Action.ToString + ", " + IDEmployee.ToString + ", " + IDFinger.ToString + ", " + Delay.ToString + ")")
                    End If
            End Select

        Catch ex As Exception
            roLog.GetInstance().logMessage(VTBase.roLog.EventType.roError, "roTerminalsSyncTasks::addSyncTask::Terminal " + _IDTerminal.ToString + "::Error:(" + _IDTerminal.ToString + ", " + Action.ToString + ", " + IDEmployee.ToString + ", " + IDFinger.ToString + ")", ex)
        End Try
    End Sub

    Private Sub delSyncTask(ByVal AddAction As SyncActions, ByVal IDEmployee As Integer, ByVal IDFinger As Integer, ByVal Parameter1 As Integer, ByVal Parameter2 As Integer)
        Try
            Dim strSQL As String
            Dim strdeletecondition As String

            strSQL = $"@DELETE# FROM TerminalsSyncTasks WHERE IDTerminal= {_IDTerminal}"

            'Si no contiene un general solo afecta a los IDs indicados
            If Not AddAction.ToString.Contains("all") Then
                strSQL += $" AND IDEmployee = {IDEmployee}"
                strSQL += $" AND IDFinger = {IDFinger}"
                strSQL += $" AND Parameter1 = {Parameter1}"
                strSQL += $" AND Parameter2 = {Parameter2}"
            End If

            Select Case AddAction
                Case SyncActions.addemployee, SyncActions.delemployee
                    strSQL += $" AND (Task='{SyncActions.addemployee}' OR Task='{SyncActions.delemployee}')"
                Case SyncActions.getallemployees
                    strSQL += $" AND Task ='{SyncActions.getallemployees}'"
                Case SyncActions.delallemployees
                    strSQL += $" AND (Task='{SyncActions.delallemployees}' OR Task='{SyncActions.addemployee}' OR Task='{SyncActions.delemployee}' OR Task='{SyncActions.delallemployeetimezones}')"
                Case SyncActions.addcard, SyncActions.delcard
                    strSQL += $" AND (Task='{SyncActions.addcard}' OR Task='{SyncActions.delcard}')"
                Case SyncActions.delallcards
                    strSQL += $" AND Task like '%card%'"
                Case SyncActions.addbio, SyncActions.delbio
                    strSQL += $" AND (Task='{SyncActions.addbio}' OR Task='{SyncActions.delbio}')"
                Case SyncActions.getbio
                    strSQL += $" AND Task ='{SyncActions.getbio}'"
                Case SyncActions.delallbios
                    strSQL += $" AND (Task='{SyncActions.addbio}' OR Task='{SyncActions.delbio}' OR Task='{SyncActions.delallbios}')"
                Case SyncActions.addbiodataface, SyncActions.delbiodataface
                    strSQL += $" And (Task='{SyncActions.addbiodataface}' OR Task='{SyncActions.delbiodataface}')"
                Case SyncActions.delallbiodataface
                    strSQL += $" AND Task like '%biodataface'"
                Case SyncActions.addbiodatapalm, SyncActions.delbiodatapalm
                    strSQL += $" AND (Task='{SyncActions.addbiodatapalm}' OR Task='{SyncActions.delbiodatapalm}')"
                Case SyncActions.delallbiodatapalm
                    strSQL += $" AND Task like '%biodatapalm'"
                Case SyncActions.addface, SyncActions.delface
                    strSQL += $" AND (Task='{SyncActions.addface}' OR Task='{SyncActions.delface}')"
                Case SyncActions.delallface
                    strSQL += $" AND Task like '%face%'"
                Case SyncActions.addphoto, SyncActions.delphoto
                    strSQL += $" AND (Task='{SyncActions.addphoto}' OR Task='{SyncActions.delphoto}')"
                Case SyncActions.delallphotos
                    strSQL += $" AND Task like '%photo%'"
                Case SyncActions.addemployeegroup, SyncActions.delemployeegroup
                    strSQL += $" AND (Task='{SyncActions.addemployeegroup}' OR Task='{SyncActions.delemployeegroup}')"
                Case SyncActions.delallemployeegroup
                    strSQL += $" AND Task like '%employeegroup%'"
                Case SyncActions.adddocument
                    strSQL += $" AND Task='{SyncActions.adddocument}'"
                Case SyncActions.deldocument
                    strSQL += $" AND Task='{SyncActions.deldocument}'"
                Case SyncActions.delalldocuments
                    strSQL += $" AND Task like '%document%'"
                Case SyncActions.addtimezone, SyncActions.deltimezone
                    strSQL += $" AND (Task='{SyncActions.addtimezone}' OR Task='{SyncActions.deltimezone}')"
                Case SyncActions.delalltimezones
                    strSQL += $" AND (Task='{SyncActions.addtimezone}' OR Task='{SyncActions.deltimezone}' OR Task='{SyncActions.delalltimezones}')"
                Case SyncActions.addemployeeaccesslevel
                    strSQL += $" AND Task='{SyncActions.addemployeeaccesslevel}'"
                Case SyncActions.delemployeeaccesslevel
                    strSQL += $" AND (Task='{SyncActions.addemployeeaccesslevel}' OR Task='{SyncActions.delemployeeaccesslevel}')"
                Case SyncActions.delallemployeeaccesslevel
                    strSQL += $" AND Task like '%employeeaccesslevel%'"
                Case SyncActions.setterminalconfig
                    strSQL += $" AND Task = '{SyncActions.setterminalconfig}'"
                Case SyncActions.delallemployeetimezones
                    strSQL += $" AND Task like '%employeetimezones%'"
                Case SyncActions.delemployeetimezones, SyncActions.addemployeetimezones
                    strSQL += $" AND (Task='{SyncActions.addemployeetimezones}' OR Task='{SyncActions.delemployeetimezones}')"
                Case SyncActions.setsirens
                    strSQL += $" AND Task='{SyncActions.setsirens}'"
                Case Else
                    strSQL += $" AND Task = '{AddAction}'"
            End Select

            Select Case WorkMode
                Case eDbWorkMode.Batch
                    strdeletecondition = strSQL.Substring(strSQL.IndexOf("AND") + 3)
                    deleteTasksList.Add($"({strdeletecondition})")
                Case eDbWorkMode.Direct
                    ' Antes de borrar, miro si existe (el borrado es costoso y puede provocar pequeños bloqueos innecesarios de la tabla)
                    strSQL = strSQL.Replace("@DELETE# FROM ", "IF EXISTS (@SELECT# * FROM ") & ") " & strSQL
                    ExecuteSql(strSQL)
            End Select

        Catch ex As Exception
            roLog.GetInstance().logMessage(VTBase.roLog.EventType.roError, $"roTerminalsSyncTasks::delSyncTask::Terminal {_IDTerminal}::Error:", ex)
        End Try
    End Sub

    Public Function PersistTasksToDatabase() As Boolean
        Dim result As Boolean = True
        Try
            Me.persistsDeleteTasksBatch()
            result = Me.persistsInsertTasksBatch()
        Catch ex As Exception
            result = False
            roLog.GetInstance().logMessage(VTBase.roLog.EventType.roError, $"roTerminalsSyncTasks::PersistTasksToDatabase::Terminal {_IDTerminal}::Error:", ex)
        End Try
        Return result
    End Function

    ''' <summary>
    ''' En modo batch, persiste las tareas de borrado. Si alguna de las instrucciones no se ejecuta correctamente, se ignora y se continúa con el resto al no ser crítico
    ''' </summary>
    Private Sub persistsDeleteTasksBatch()
        Dim batchSize As Integer = batchSizeOnBatchMode ' Tamaño del lote, ajusta este valor según sea necesario
        Dim sqlDeletePart As String = $"@DELETE# TerminalsSyncTasks WHERE IdTerminal = {_IDTerminal} AND ("

        ' Proceso por lotes para evitar límite de longitud de sentencia SQL
        For startIdx As Integer = 0 To deleteTasksList.Count - 1 Step batchSize
            Dim sqlValueBuilder As New StringBuilder()

            ' Bucle interno para agregar elementos dentro del lote actual
            For i As Integer = startIdx To Math.Min(startIdx + batchSize - 1, deleteTasksList.Count - 1)
                Dim sqlTask As String = deleteTasksList(i)
                sqlValueBuilder.Append($"{sqlTask} OR ")
            Next

            ' Quito el último OR
            sqlValueBuilder.Remove(sqlValueBuilder.Length - 4, 4)

            Dim finalSql As String = String.Concat(sqlDeletePart, sqlValueBuilder)

            Try
                ExecuteSql($"{finalSql})")
            Catch ex As Exception
                roLog.GetInstance().logMessage(VTBase.roLog.EventType.roError, $"roTerminalsSyncTasks::persistsDeleteTasksBatch::Terminal {_IDTerminal}::Error:", ex)
            End Try
        Next

    End Sub

    ''' <summary>
    ''' En modo batch, persiste las tareas de inserción
    ''' </summary>
    ''' <returns>True sólo si todos los inserts se realizaron</returns>
    Private Function persistsInsertTasksBatch() As Boolean
        Dim result As Boolean = True
        Dim batchSize As Integer = batchSizeOnBatchMode
        Dim sqlInsertPart As String = "@INSERT# INTO TerminalsSyncTasks(IDTerminal, Task, IDEmployee, IDFinger, Parameter1, Parameter2, TaskData, TaskDate) VALUES "


        Try
            ' Proceso por lotes para evitar límite de longitud de sentencia SQL
            For startIdx As Integer = 0 To insertTasksList.Count - 1 Step batchSize
                Dim sqlValueBuilder As New StringBuilder()

                ' Bucle interno para agregar elementos dentro del lote actual
                For i As Integer = startIdx To Math.Min(startIdx + batchSize - 1, insertTasksList.Count - 1)
                    Dim sqlTask As String = insertTasksList(i)
                    sqlValueBuilder.Append($"{sqlTask.Substring(sqlTask.IndexOf("VALUES") + "VALUES".Length).Trim()},")
                Next

                Dim finalSql As String = String.Concat(sqlInsertPart, sqlValueBuilder.ToString().TrimEnd(","c))

                ExecuteSql(finalSql)
            Next
        Catch ex As Exception
            result = False
            roLog.GetInstance().logMessage(VTBase.roLog.EventType.roError, $"roTerminalsSyncTasks::persistsInsertTasksBatch::Terminal {_IDTerminal}::Error:", ex)
        End Try

        Return result

    End Function

    Public Sub Done(ByVal idTask As Long)
        Try
            Dim strSQL As String
            strSQL = $"@DELETE# FROM TerminalsSyncTasks"
            strSQL += " WHERE IDTerminal=" + _IDTerminal.ToString
            strSQL += " AND DeleteOnConfirm=1 and ID = " + idTask.ToString

            ExecuteSql(strSQL)
        Catch ex As Exception
            roLog.GetInstance().logMessage(VTBase.roLog.EventType.roError, "roTerminalsSyncTasks::DoneByID::Terminal " + _IDTerminal.ToString + "::Error:", ex)
        End Try
    End Sub

    ''' <summary>
    ''' Elimina una tarea dado su ID, independientemente de su estado
    ''' </summary>
    ''' <param name="idTask"></param>
    ''' <remarks></remarks>
    Public Sub DoneEx(ByVal idTask As Long)
        Try
            Dim strSQL As String
            strSQL = $"@DELETE# FROM TerminalsSyncTasks"
            strSQL += " WHERE IDTerminal=" + _IDTerminal.ToString
            strSQL += " AND ID = " + idTask.ToString

            ExecuteSql(strSQL)
        Catch ex As Exception
            roLog.GetInstance().logMessage(VTBase.roLog.EventType.roError, "roTerminalsSyncTasks::DoneByID::Terminal " + _IDTerminal.ToString + "::Error:", ex)
        End Try
    End Sub

    ''' <summary>
    ''' Desmarca las tareas que estan marcadas como en proceso
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub ResetAll()
        Try

            Dim strSQL As String
            strSQL = $"@UPDATE# TerminalsSyncTasks"
            strSQL += " SET DeleteOnConfirm=0"
            strSQL += " WHERE IDTerminal=" + _IDTerminal.ToString

            ExecuteSql(strSQL)
        Catch ex As Exception
            roLog.GetInstance().logMessage(VTBase.roLog.EventType.roError, "roTerminalsSyncTasks::ResetAll::Terminal " + _IDTerminal.ToString + "::Error:", ex)
        End Try

    End Sub

    ''' <summary>
    ''' Desmarca las tareas que estan marcadas como en proceso
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub ResetAllWithDelay(ByVal sSecondsDelay As Integer)
        Dim sSecondsOffset As Integer
        Try
            Dim strSQL As String

            If sSecondsDelay > 0 Then
                strSQL = $"@SELECT# MAX(DATEDIFF(s,TaskDate,getdate())) FROM TerminalsSyncTasks WITH (NOLOCK) WHERE IDTerminal = " + _IDTerminal.ToString
                sSecondsOffset = roTypes.Any2Integer(ExecuteScalar(strSQL))
            End If

            strSQL = $"@UPDATE# TerminalsSyncTasks"
            strSQL += " SET DeleteOnConfirm=0"
            If sSecondsDelay > 0 Then strSQL += ",TaskDate=dateadd(s," + (sSecondsDelay + sSecondsOffset).ToString + ",taskdate)"
            strSQL += " WHERE IDTerminal=" + _IDTerminal.ToString

            ExecuteSql(strSQL)
        Catch ex As Exception
            roLog.GetInstance().logMessage(VTBase.roLog.EventType.roError, "roTerminalsSyncTasks::ResetAll::Terminal " + _IDTerminal.ToString + "::Error:", ex)
        End Try

    End Sub

    ''' <summary>
    ''' Desmarca las tareas que estan marcadas como en proceso
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub DelayEmployeeTasks(ByVal sSecondsDelay As Integer)
        Try

            Dim strSQL As String
            If sSecondsDelay > 0 Then
                strSQL = $"@UPDATE# TerminalsSyncTasks"
                strSQL += " SET TaskDate=dateadd(s,DATEDIFF(s,taskdate,getdate())+" + sSecondsDelay.ToString + ",taskdate)"
                strSQL += " WHERE IDTerminal=" + _IDTerminal.ToString + " AND IDEmployee = " & Me.IDEmployee.ToString
                ExecuteSql(strSQL)
            End If
        Catch ex As Exception
            roLog.GetInstance().logMessage(VTBase.roLog.EventType.roError, "roTerminalsSyncTasks::DelayEmployeeTasks::Terminal " + _IDTerminal.ToString + " Employee " + Me.IDEmployee.ToString + "::Error:", ex)
        Finally

        End Try

    End Sub

    ''' <summary>
    ''' Marca la tarea actual como en proceso
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub WorkingEx()

        Try

            Dim strSQL As String
            strSQL = $"@UPDATE# TerminalsSyncTasks
                       SET DeleteOnConfirm=1, tasksent = getdate(), taskretries = CASE WHEN tasksent IS NULL THEN 0 ELSE IIF(taskretries < {_MaxSyncTasksRetries}, taskretries + 1, {_MaxSyncTasksRetries}) END
                       WHERE ID = {_ID}"

            ExecuteSql(strSQL)
        Catch ex As Exception
            roLog.GetInstance().logMessage(VTBase.roLog.EventType.roError, "roTerminalsSyncTasks::Working::Terminal " + _IDTerminal.ToString + "::Error:", ex)
        Finally

        End Try
    End Sub

    Public Sub WillBeRetried()

        Try

            Dim strSQL As String
            strSQL = $"@UPDATE# TerminalsSyncTasks
                       SET taskretries = CASE WHEN taskretries IS NULL THEN 0 ELSE IIF(taskretries < {_MaxSyncTasksRetries}, taskretries + 1, {_MaxSyncTasksRetries}) END
                       WHERE ID = {_ID}"

            ExecuteSql(strSQL)
        Catch ex As Exception
            roLog.GetInstance().logMessage(VTBase.roLog.EventType.roError, "roTerminalsSyncTasks::WillBeRetried::Terminal " + _IDTerminal.ToString + "::Error:", ex)
        Finally

        End Try
    End Sub

    ''' <summary>
    ''' Carga la tarea a realizar
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub LoadNext()
        Dim nextTask As SyncActions = SyncActions.none
        Try
            Dim dTasks As DataTable
            Dim strSQL As String

            roTrace.GetInstance.InitTraceEvent()

            ' Por compatiblidad, si el límite es 32767, debe seguir intentando esa ...
            ' Si el límite es menor, esa tarea la obvia y pasará a la siguiente si existe
            Dim iMaxRetries As Integer = _MaxSyncTasksRetries
            If _MaxSyncTasksRetries = 32767 Then iMaxRetries = _MaxSyncTasksRetries + 1

            strSQL = $"@SELECT# * FROM ( 
                      @SELECT# ROW_NUMBER() OVER (PARTITION BY IDTerminal ORDER BY TaskDate, ID, Task) as RowNumber, * FROM TerminalsSyncTasks WITH (NOLOCK) 
                      WHERE TaskDate < GETDATE() AND DeleteOnConfirm=0 AND idterminal = {_IDTerminal} AND ISNULL(TaskRetries, 0) < {iMaxRetries}
                      ) AUX WHERE AUX.RowNumber = 1"

            dTasks = CreateDataTable(strSQL, "TerminalsSyncTasks")

            If dTasks.Rows.Count > 0 Then
                Try
                    _ID = roTypes.Any2Long(dTasks.Rows(0).Item("ID"))
                    _task = System.Enum.Parse(GetType(SyncActions), dTasks.Rows(0).Item("Task"), True)
                    _IDEmployee = roTypes.Any2Long(dTasks.Rows(0).Item("IDEmployee"))
                    _IDFinger = roTypes.Any2Long(dTasks.Rows(0).Item("IDFinger"))
                    _Parameter1 = roTypes.Any2Long(dTasks.Rows(0).Item("Parameter1"))
                    _Parameter2 = roTypes.Any2Long(dTasks.Rows(0).Item("Parameter2"))
                    _TaskData = roTypes.Any2String(dTasks.Rows(0).Item("TaskData"))
                    _Retries = roTypes.Any2Integer(dTasks.Rows(0).Item("TaskRetries"))
                Catch ex As Exception
                    _task = SyncActions.none
                    ExecuteSql($"@DELETE# TerminalsSyncTasks WHERE ID=" + dTasks.Rows(0).Item("ID").ToString)
                    roLog.GetInstance().logMessage(roLog.EventType.roError, "roTerminalsSyncTasks::LoadNext::Terminal " + _IDTerminal.ToString + "::Task not recognized, delete task. (" + dTasks.Rows(0).Item("Task") + ")", ex)
                End Try
            Else
                _task = SyncActions.none
            End If
            nextTask = _task
        Catch ex As Exception
            roLog.GetInstance().logMessage(VTBase.roLog.EventType.roError, "roTerminalsSyncTasks::LoadNext::Terminal " + _IDTerminal.ToString + "::Error:", ex)
        Finally
            roTrace.GetInstance.AddTraceEvent(If(nextTask = SyncActions.none, "No more tasks pending", $"Next terminal task loaded ({nextTask})"))
        End Try
    End Sub

    ''' <summary>
    ''' Carga una tarea dado su id
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub Load(ByVal idTask As Long)

        Try
            Dim dTasks As DataTable
            Dim strSQL As String

            ' Por compatiblidad, si el límite es 32767, debe seguir intentando esa ...
            ' Si el límite es menor, esa tarea la obvia y pasará a la siguiente si existe
            Dim iMaxRetries As Integer = _MaxSyncTasksRetries
            If _MaxSyncTasksRetries = 32767 Then iMaxRetries = _MaxSyncTasksRetries + 1

            strSQL = $"@SELECT# * FROM TerminalsSyncTasks WITH (NOLOCK)
                       WHERE ID = {idTask} AND ISNULL(TaskRetries, 0) < {iMaxRetries}"

            dTasks = CreateDataTable(strSQL, "TerminalsSyncTasks")

            If dTasks.Rows.Count > 0 Then
                Try
                    _ID = roTypes.Any2Long(dTasks.Rows(0).Item("ID"))
                    _task = System.Enum.Parse(GetType(SyncActions), dTasks.Rows(0).Item("Task"), True)
                    _IDEmployee = roTypes.Any2Long(dTasks.Rows(0).Item("IDEmployee"))
                    _IDFinger = roTypes.Any2Long(dTasks.Rows(0).Item("IDFinger"))
                    _Parameter1 = roTypes.Any2Long(dTasks.Rows(0).Item("Parameter1"))
                    _Parameter2 = roTypes.Any2Long(dTasks.Rows(0).Item("Parameter2"))
                    _TaskData = roTypes.Any2String(dTasks.Rows(0).Item("TaskData"))
                Catch ex As Exception
                    _task = SyncActions.none
                    ExecuteSql($"@DELETE# TerminalsSyncTasks WHERE ID=" + dTasks.Rows(0).Item("ID").ToString)
                    roLog.GetInstance().logMessage(roLog.EventType.roError, "roTerminalsSyncTasks::LoadNext::Terminal " + _IDTerminal.ToString + "::Task not recognized, delete task. (" + dTasks.Rows(0).Item("Task") + ")", ex)
                End Try
            Else
                _task = SyncActions.none
            End If
        Catch ex As Exception
            roLog.GetInstance().logMessage(VTBase.roLog.EventType.roError, "roTerminalsSyncTasks::LoadNext::Terminal " + _IDTerminal.ToString + "::Error:", ex)
        Finally

        End Try
    End Sub

    Public Overrides Function ToString() As String
        Try
            Return "TaskID: " + _ID.ToString + " Task: " + _task.ToString + " IdEmployee: " + _IDEmployee.ToString + " IDFinger: " + _IDFinger.ToString + " IdTerminal: " + _IDTerminal.ToString
        Catch ex As Exception
            roLog.GetInstance().logMessage(VTBase.roLog.EventType.roDebug, "roTerminalsSyncTasks::ToString::Terminal " + _IDTerminal.ToString + "::Error:", ex)
            Return "error recovering task data"
        End Try
    End Function

End Class