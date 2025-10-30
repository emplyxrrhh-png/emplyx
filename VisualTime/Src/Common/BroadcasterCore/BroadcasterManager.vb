Imports System.Data
Imports System.Drawing
Imports System.Linq
Imports System.Text
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBroadcasterCore.BusinessLogicLayer
Imports Robotics.Base.VTBroadcasterCore.DataLogicLayer
Imports Robotics.Base.VTBusiness
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Comms.Base
Imports Robotics.DataLayer
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.Security.VTSecurity
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions
Imports Robotics.VTBase.roTypes

Public Class BroadcasterManager

    Private oBCData As New BCData
    Private _BiometricVersion As String = ""
    Private _TerminalID As Integer
    Private _Terminal As TerminalData
    Private slTasksmx As New Dictionary(Of Byte, List(Of String))
    Private slTasksEx As New Dictionary(Of Byte, List(Of String))
    Private AdvancedAccessMode As Boolean = False
    Private RestrictedAttendance As Boolean = False
    Private oHelper As New BroadcasterHelper
    Private broadcasterTaskBatchSize As Integer
    Private deleteExistingTasks As Boolean = True
    Private Const BroadcastSystemVersion = 1
    Private LegacyRestrictedModeUsed As Boolean = False

    Public ReadOnly Property TerminalData As TerminalData
        Get
            Return _Terminal
        End Get
    End Property

    Public Sub RunBroadcaster(ByVal IdTerminal As Integer, Optional ByVal _TaskCommand As String = "", Optional ByVal _TaskEmployeeID As Integer = 0, Optional ByVal _TaskOnly As Boolean = True, Optional ByVal _TaskFingerID As Integer = 0)
        Dim TaskCommand As String = _TaskCommand
        Dim TaskEmployeeID As Integer = _TaskEmployeeID
        Dim TaskOnly As Boolean = _TaskOnly
        Dim TaskFingerID As Integer = _TaskFingerID
        Dim command As String = ""
        Dim strSQL As String

        Try
            _TerminalID = IdTerminal

            roTrace.GetInstance().AddTraceEvent($"BroadcasterManager::RunBroadcaster:Terminal {_TerminalID}: ({Reflection.Assembly.GetExecutingAssembly.GetName.Version}) inicialized.[{command.Trim}]")

            If _TerminalID > 0 Then
                _Terminal = New TerminalData(_TerminalID)

                If _Terminal.Enabled Then
                    'Recupero el modo de Accesos de la instalación
                    Dim companyName As String = Azure.RoAzureSupport.GetCompanyName
                    Dim sAdvancedAccessModeValue As String = ""
                    sAdvancedAccessModeValue = roCacheManager.GetInstance().GetAdvParametersCache(companyName, "AdvancedAccessMode")
                    AdvancedAccessMode = (Any2Integer(sAdvancedAccessModeValue) = 1)
                    'Si la instalación está en modo avanzado, miro si el terminal lo soporta
                    If AdvancedAccessMode Then AdvancedAccessMode = _Terminal.SupportsAdvancedAccessMode

                    Dim sRestrictedAttendance As String = ""
                    sRestrictedAttendance = roCacheManager.GetInstance().GetAdvParametersCache(companyName, "CheckAccessAuthorizationOnNoAccessReaders")
                    RestrictedAttendance = (Any2Integer(sRestrictedAttendance) = 1)

                    ' Miro debe estar habilitado el modo de restricción de presencia antiguo (descripción de grupos y/o lista de empleados a nivel de lector)
                    If Not RestrictedAttendance Then
                        Try
                            strSQL = "@SELECT# SUM(total) total from (@SELECT# COUNT(*) total from TerminalReaderEmployees WITH (NOLOCK) UNION @SELECT# COUNT(*) total from Groups WITH (NOLOCK) where DescriptionGroup like '%TERMINAL=%') aux"
                            Dim tbControl As DataTable = Nothing
                            tbControl = CreateDataTable(strSQL, "Control")
                            LegacyRestrictedModeUsed = False
                            If tbControl IsNot Nothing AndAlso tbControl.Rows.Count > 0 Then
                                If Any2Boolean(tbControl.Rows(0).Item("total")) Then
                                    LegacyRestrictedModeUsed = True
                                    CreateLegacyAttRestrictionUserTask()
                                Else
                                    DelLegacyAttRestrictionUserTask()
                                End If
                            End If
                        Catch ex As Exception
                        End Try
                    End If

                    Dim sBroadcasterTaskBatchSize As String = ""
                    sBroadcasterTaskBatchSize = roCacheManager.GetInstance().GetAdvParametersCache(companyName, "BroadcasterTaskBatchSize")
                    broadcasterTaskBatchSize = Any2Integer(sBroadcasterTaskBatchSize)

                    Try
                        ' Cargo número de tareas pendientes, para optimizar la generación de tareas
                        _Terminal.PendingTasks = roTypes.Any2Integer(ExecuteScalar("@SELECT# isnull(count(*),0) from TerminalsSyncTasks where IDTerminal = " + _Terminal.ID.ToString))
                        deleteExistingTasks = (_Terminal.PendingTasks > 0)
                    Catch ex As Exception
                    End Try

                    CleanSyncTables()

                    BCGlobal.SirensOutput = _Terminal.SirensOutput

                    Select Case _Terminal.Type
                        Case TerminalData.eTerminalType.mx8, TerminalData.eTerminalType.mcxinbio, TerminalData.eTerminalType.rxcp, TerminalData.eTerminalType.rxcep, TerminalData.eTerminalType.rx1, TerminalData.eTerminalType.mx9, TerminalData.eTerminalType.mxs
                            _BiometricVersion = "RXFFNG"
                        Case TerminalData.eTerminalType.rxfp
                            _BiometricVersion = "ZKUNIFAC,ZKUNIPAL"
                        Case TerminalData.eTerminalType.rxfl
                            _BiometricVersion = "RXFFNG,ZKUNIFAC,ZKUNIPAL"
                        Case TerminalData.eTerminalType.rxfe
                            _BiometricVersion = "RXFFNG,ZKUNIFAC"
                        Case TerminalData.eTerminalType.rxfptd
                            _BiometricVersion = "ZKUNIFAC,ZKUNIPAL"
                        Case TerminalData.eTerminalType.rxte
                            _BiometricVersion = "NONE"
                        Case Else
                            _BiometricVersion = "RXFFNG"
                    End Select

                    'Miramos si nos han pasado un parametro de tareas
                    If TaskCommand.Length > 0 Then
                        Select Case TaskCommand.ToLower
                            Case roTerminalsSyncTasks.SyncActions.getallemployees.ToString
                                Dim oTask As roTerminalsSyncTasks = New roTerminalsSyncTasks(_Terminal.ID)
                                oTask.addSyncTask(roTerminalsSyncTasks.SyncActions.getallemployees, 0,,,,,, deleteExistingTasks)
                            Case roTerminalsSyncTasks.SyncActions.getbio.ToString
                                Execute_GetBio()
                            Case roTerminalsSyncTasks.SyncActions.addemployee.ToString
                                Execute_AddEmployee(TaskEmployeeID)
                            Case roTerminalsSyncTasks.SyncActions.delemployee.ToString
                                Execute_DelEmployee(TaskEmployeeID)
                            Case roTerminalsSyncTasks.SyncActions.delbio.ToString
                                Execute_DelBio(TaskEmployeeID, TaskFingerID)
                            Case roTerminalsSyncTasks.SyncActions.addbio.ToString
                                Execute_AddBio(TaskEmployeeID, TaskFingerID)
                            Case roTerminalsSyncTasks.SyncActions.addcard.ToString
                                Execute_AddCard(TaskEmployeeID)
                            Case roTerminalsSyncTasks.SyncActions.delbiodataface.ToString
                                Execute_DelBioDataFace(TaskEmployeeID, TaskFingerID)
                            Case roTerminalsSyncTasks.SyncActions.addbiodataface.ToString
                                Execute_AddBioDataFace(TaskEmployeeID, TaskFingerID)
                            Case roTerminalsSyncTasks.SyncActions.delbiodatapalm.ToString
                                Execute_DelBioDataPalm(TaskEmployeeID, TaskFingerID)
                            Case roTerminalsSyncTasks.SyncActions.addbiodatapalm.ToString
                                Execute_AddBioDataPalm(TaskEmployeeID, TaskFingerID)
                            Case roTerminalsSyncTasks.SyncActions.none.ToString
                                TaskOnly = False
                            Case Else
                                TaskOnly = False
                                roTrace.GetInstance().AddTraceEvent($"BroadcasterManager::RunBroadcaster:Terminal {_TerminalID}:Task command not exist.({TaskCommand})")
                        End Select
                        If TaskOnly Then
                            Exit Sub
                        Else
                            Threading.Thread.Sleep(1000)
                        End If
                    End If

                    ' Si por un casual hay otra sincronización del mismo terminal, espero
                    Try
                        strSQL = "@SELECT# SyncStartDate FROM Terminals WHERE ID = " & _TerminalID
                        Dim dLastSyncStart As Object
                        Dim dStartWaiting As DateTime = Now
                        Dim iWaitMinutes As Integer = 2
                        dLastSyncStart = ExecuteScalar(strSQL)
                        Dim bKeepWaiting As Boolean = Not (IsDBNull(dLastSyncStart) OrElse dStartWaiting.Subtract(dLastSyncStart).TotalMinutes > iWaitMinutes)
                        Dim iIteration As Integer = 0

                        While bKeepWaiting
                            iIteration = iIteration + 1
                            roLog.GetInstance().logMessage(Robotics.VTBase.roLog.EventType.roDebug, $"BroadcasterManager::RunBroadcaster:Terminal {_TerminalID}: Waiting a running Broadcast for this terminal to finish!. Iteration {iIteration} ")
                            Threading.Thread.Sleep(30000)
                            dLastSyncStart = ExecuteScalar(strSQL)
                            bKeepWaiting = Not (IsDBNull(dLastSyncStart) OrElse DateTime.Now.Subtract(dStartWaiting).TotalMinutes > iWaitMinutes)
                        End While

                        If iIteration > 0 Then
                            roLog.GetInstance().logMessage(Robotics.VTBase.roLog.EventType.roDebug, $"BroadcasterManager::RunBroadcaster:Terminal {_TerminalID}:Resuming Broadcast for this terminal after prior one finished!.")
                        End If
                    Catch ex As Exception
                    End Try

                    ' Marco en inicio de cálculo en la tabla de terminales
                    Try
                        strSQL = "@UPDATE# Terminals SET SyncStartDate = getdate() WHERE ID = " & _TerminalID
                        ExecuteSql(strSQL)
                    Catch ex As Exception
                    End Try

                    Select Case _Terminal.Type
                        Case TerminalData.eTerminalType.mx8, TerminalData.eTerminalType.mx9
                            ' Si el terminal no tiene comportamiento, no calculo nada ...
                            If _Terminal.RDRBehavior(1) <> "" Then
                                UpdateFilesMx7ToDatabase(_TerminalID)
                            Else
                                roTrace.GetInstance().AddTraceEvent($"BroadcasterManager::RunBroadcaster:Terminal {_TerminalID}:No behaviour defined, no task will be generated. Exiting!")
                            End If
                        Case TerminalData.eTerminalType.mcxinbio, TerminalData.eTerminalType.mxs
                            If _Terminal.RDRBehavior(1) <> "" Then
                                UpdateFilesMxaPlusToDataBase(_TerminalID)
                            End If
                        Case TerminalData.eTerminalType.rxcp, TerminalData.eTerminalType.rxcep, TerminalData.eTerminalType.rx1, TerminalData.eTerminalType.rxfp,
                             TerminalData.eTerminalType.rxfl, TerminalData.eTerminalType.rxfptd, TerminalData.eTerminalType.rxfe, TerminalData.eTerminalType.rxte
                            If _Terminal.RDRBehavior(1) <> "" Then
                                UpdateFilesZKPush2ToDatabase(_TerminalID)
                            End If
                        Case Else
                            roTrace.GetInstance().AddTraceEvent($"BroadcasterManager::RunBroadcaster::Terminal {_TerminalID}: Unknown terminal type: {_Terminal.TypeStr}")
                    End Select

                    ' Marco fin de cálculo en trabla de terminales
                    Try
                        strSQL = "@UPDATE# Terminals SET SyncStartDate = null WHERE ID = " & _TerminalID
                        ExecuteSql(strSQL)
                    Catch ex As Exception
                        roLog.GetInstance.logMessage(roLog.EventType.roError, "BroadcasterManager::RunBroadcaster::Exception setting task as finished. Detail: ", ex)
                    End Try
                End If
            Else
                roTrace.GetInstance().AddTraceEvent("BroadcasterManager::RunBroadcaster::Terminal ID disabled. Avoid Broadcaster")
            End If

            roTrace.GetInstance().AddTraceEvent($"BroadcasterManager::RunBroadcaster::Terminal {_TerminalID} finished.")
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, $"BroadcasterManager::RunBroadcaster::Terminal {_TerminalID}:: Unexpected error: ", ex)
        End Try
    End Sub

    Private Sub CleanSyncTables()
        ' Limpio posibles duplicados en tablas de sincronización que provocan errores en las funciones CompareXML (prevenirlos es más costoso que eliminarlos si se produjeron)
        Try
            Dim cleanDuplicatesSentence As String = $"WITH CTE AS (
                                                                    @SELECT# *,
                                                                           ROW_NUMBER() OVER (PARTITION BY EmployeeID, TerminalId ORDER BY TimeStamp DESC) AS RowNum
                                                                    FROM [dbo].[TerminalsSyncCardsData] WHERE TerminalId = {_TerminalID}
                                                                )
                                                              @DELETE# FROM CTE
                                                              WHERE RowNum > 1;"
            AccessHelper.ExecuteSql(cleanDuplicatesSentence)

            cleanDuplicatesSentence = $"WITH CTE AS (
                                                                @SELECT# *,
                                                                       ROW_NUMBER() OVER (PARTITION BY EmployeeID, TerminalId ORDER BY TimeStamp DESC) AS RowNum
                                                                FROM [dbo].[TerminalsSyncEmployeesData]  WHERE TerminalId = {_TerminalID}
                                                            )
                                                          @DELETE# FROM CTE
                                                          WHERE RowNum > 1;"
            AccessHelper.ExecuteSql(cleanDuplicatesSentence)

            cleanDuplicatesSentence = $"WITH CTE AS (
                                                            @SELECT# *,
                                                                   ROW_NUMBER() OVER (PARTITION BY IDEmployee, TerminalId ORDER BY TimeStamp DESC) AS RowNum
                                                            FROM [dbo].[TerminalsSyncPushEmployeeTimeZonesData]  WHERE TerminalId = {_TerminalID}
                                                            )
                                                         @DELETE# FROM CTE
                                                         WHERE RowNum > 1;"
            AccessHelper.ExecuteSql(cleanDuplicatesSentence)

            cleanDuplicatesSentence = $"WITH CTE AS (
                                                                @SELECT# *,
                                                                       ROW_NUMBER() OVER (PARTITION BY GroupId, ReaderId, DayOf, BeginTime, TerminalId ORDER BY TimeStamp DESC) AS RowNum
                                                                FROM [dbo].[TerminalsSyncTimeZonesData]  WHERE TerminalId = {_TerminalID}
                                                            )
                                                          @DELETE# FROM CTE
                                                          WHERE RowNum > 1;"
            AccessHelper.ExecuteSql(cleanDuplicatesSentence)
        Catch ex As Exception
            roLog.GetInstance().logMessage(Robotics.VTBase.roLog.EventType.roError, $"BroadcasterManager::RunBroadcaster:Terminal {_TerminalID}. Exception cleaning sync tables.", ex)
        End Try
    End Sub

    Private Sub Execute_GetBio()
        Try
            Dim oTask As roTerminalsSyncTasks = New roTerminalsSyncTasks(_Terminal.ID)

            Dim dt As DataTable = oBCData.GetEmployeesAllBio_Live(_BiometricVersion, AdvancedAccessMode)
            For Each row As DataRow In dt.Rows
                oTask.addSyncTask(roTerminalsSyncTasks.SyncActions.getbio, row.Item("EmployeeID"), row.Item("BiometricID"), -100,,,, deleteExistingTasks)
            Next
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, $"BroadcasterNet::Main::Terminal {_TerminalID}::Execute_GetBio:: Unexpected error: ", ex)
        End Try
    End Sub

    Private Sub Execute_AddEmployee(ByVal IDEmployee As Integer)
        Try
            If _Terminal.EmployeePermit(IDEmployee) Then
                Dim oTask As roTerminalsSyncTasks = New roTerminalsSyncTasks(_Terminal.ID)
                'Añadimos al empleado
                oTask.addSyncTask(roTerminalsSyncTasks.SyncActions.addemployee, IDEmployee,,,,,, deleteExistingTasks)

                'Añadimos las huellas

                Dim dt As DataTable = oBCData.GetEmployeesAllBio_Live(_BiometricVersion, AdvancedAccessMode, IDEmployee)
                For Each row As DataRow In dt.Rows
                    oTask.addSyncTask(roTerminalsSyncTasks.SyncActions.addbio, row.Item("EmployeeID"), row.Item("BiometricID"), 10,,,, deleteExistingTasks)
                Next
            End If
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, $"BroadcasterNet::Main::Terminal {_TerminalID}::Execute_AddEmployee:: Unexpected error: ", ex)
        End Try
    End Sub

    Private Sub Execute_DelEmployee(ByVal IDEmployee As Integer)
        Try
            Dim oTask As roTerminalsSyncTasks = New roTerminalsSyncTasks(_Terminal.ID)
            'Añadimos al empleado
            oTask.addSyncTask(roTerminalsSyncTasks.SyncActions.delemployee, IDEmployee,,,,,, deleteExistingTasks)

            'PDTE: Borramos biometría y resto de credenciales y datos ... (lo debería hacer el terminal) ...
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, $"BroadcasterNet::Main::Terminal {_TerminalID}::Execute_DelEmployee:: Unexpected error: ", ex)
        End Try
    End Sub

    Private Sub Execute_DelBio(ByVal IDEmployee As Integer, ByVal IDFinger As Integer)
        Try

            Dim oTask As roTerminalsSyncTasks = New roTerminalsSyncTasks(_Terminal.ID)
            'Añadimos al empleado
            oTask.addSyncTask(roTerminalsSyncTasks.SyncActions.delbio, IDEmployee, IDFinger,,,,, deleteExistingTasks)
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, $"BroadcasterNet::Main::Terminal {_TerminalID}::Execute_DelBio:: Unexpected error: ", ex)
        End Try
    End Sub

    Private Sub Execute_AddBio(ByVal IDEmployee As Integer, ByVal IDFinger As Integer)
        Try

            Dim oTask As roTerminalsSyncTasks = New roTerminalsSyncTasks(_Terminal.ID)
            'Añadimos al empleado
            oTask.addSyncTask(roTerminalsSyncTasks.SyncActions.addbio, IDEmployee, IDFinger,,,,, deleteExistingTasks)
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, $"BroadcasterNet::Main::Terminal {_TerminalID}::Execute_AddBio:: Unexpected error: ", ex)
        End Try
    End Sub

    Private Sub Execute_AddBioDataFace(ByVal IDEmployee As Integer, ByVal IDFace As Integer)
        Try

            Dim oTask As roTerminalsSyncTasks = New roTerminalsSyncTasks(_Terminal.ID)
            'Añadimos al empleado
            oTask.addSyncTask(roTerminalsSyncTasks.SyncActions.addbiodataface, IDEmployee, IDFace,,,,, deleteExistingTasks)
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, $"BroadcasterNet::Main::Terminal {_TerminalID}::Execute_AddBioDataFace:: Unexpected error: ", ex)
        End Try
    End Sub

    Private Sub Execute_DelBioDataFace(ByVal IDEmployee As Integer, ByVal IDFace As Integer)
        Try

            Dim oTask As roTerminalsSyncTasks = New roTerminalsSyncTasks(_Terminal.ID)
            'Añadimos al empleado
            oTask.addSyncTask(roTerminalsSyncTasks.SyncActions.delbiodataface, IDEmployee, IDFace,,,,, deleteExistingTasks)
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, $"BroadcasterNet::Main::Terminal {_TerminalID}::Execute_DelBioDataFace:: Unexpected error: ", ex)
        End Try
    End Sub

    Private Sub Execute_AddBioDataPalm(ByVal IDEmployee As Integer, ByVal IDPalm As Integer)
        Try

            Dim oTask As roTerminalsSyncTasks = New roTerminalsSyncTasks(_Terminal.ID)
            'Añadimos al empleado
            oTask.addSyncTask(roTerminalsSyncTasks.SyncActions.addbiodatapalm, IDEmployee, IDPalm,,,,, deleteExistingTasks)
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, $"BroadcasterNet::Main::Terminal {_TerminalID}::Execute_AddBioDataPalm:: Unexpected error: ", ex)
        End Try
    End Sub

    Private Sub Execute_DelBioDataPalm(ByVal IDEmployee As Integer, ByVal IDPalm As Integer)
        Try

            Dim oTask As roTerminalsSyncTasks = New roTerminalsSyncTasks(_Terminal.ID)
            'Añadimos al empleado
            oTask.addSyncTask(roTerminalsSyncTasks.SyncActions.delbiodatapalm, IDEmployee, IDPalm,,,,, deleteExistingTasks)
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, $"BroadcasterNet::Main::Terminal {_TerminalID}::Execute_DelBioDataFace:: Unexpected error: ", ex)
        End Try
    End Sub

    Private Sub Execute_AddCard(ByVal IDEmployee As Integer)
        Try

            Dim oTask As roTerminalsSyncTasks = New roTerminalsSyncTasks(_Terminal.ID)
            'Añadimos al empleado
            oTask.addSyncTask(roTerminalsSyncTasks.SyncActions.addcard, IDEmployee,,,,,, deleteExistingTasks)
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, $"BroadcasterNet::Main::Terminal {_TerminalID}::Execute_AddBio:: Unexpected error: ", ex)
        End Try
    End Sub

    Private Sub UpdateFilesMx7ToDatabase(ByVal IDTerminal As Integer)
        Dim bolCheckAcc As Boolean = False
        Dim sBehavior As String = "TA"
        Dim fConfig As TerminalConfigFile
        Dim bIsMX9 As Boolean = False
        Dim bIsMX8 As Boolean = False

        Try
            bIsMX9 = (_Terminal.Type = TerminalData.eTerminalType.mx9)
            bIsMX8 = (_Terminal.Type = TerminalData.eTerminalType.mx8)

            ' Miramos si el lector está configurado cómo TAACC (presencia restringida)
            fConfig = New TerminalConfigFile
            Dim tbReaders As Data.DataTable = oBCData.GetTerminalReaders(IDTerminal)
            If tbReaders IsNot Nothing Then
                Dim oRows As DataRow() = tbReaders.Select("ID = 1")
                If oRows.Length = 1 Then
                    bolCheckAcc = (oRows(0)("Mode").ToString.IndexOf("ACC") >= 0)  ' = "TAACC")
                    If bolCheckAcc OrElse bIsMX9 OrElse bIsMX8 Then
                        If Any2Long(oRows(0)("Output")) > 0 Then
                            fConfig.Add(TerminalConfigFile.eParameterNames.RDR1OpenTime, oRows(0)("Duration").ToString)
                        Else
                            fConfig.Add(TerminalConfigFile.eParameterNames.RDR1OpenTime, "0")
                        End If
                    End If
                    If bIsMX9 OrElse bIsMX8 Then
                        fConfig.Add(TerminalConfigFile.eParameterNames.Mode, oRows(0)("Mode").ToString)
                        fConfig.Add(TerminalConfigFile.eParameterNames.InteractionAction, oRows(0)("InteractionAction").ToString)
                        fConfig.Add(TerminalConfigFile.eParameterNames.ValdationMode, oRows(0)("ValidationMode").ToString)
                        fConfig.Add(TerminalConfigFile.eParameterNames.Zone, oRows(0)("IDZone").ToString)

                        ' Captura de imagen al fichar
                        Dim oConfData As New roCollection(Any2String(_Terminal.ConfData))
                        fConfig.Add(TerminalConfigFile.eParameterNames.TakePhoto, Any2Boolean(oConfData.Item("CaptureImage")).ToString())
                    End If
                    sBehavior = Any2String(oRows(0)("Mode"))
                End If
            End If

            ' Para terminales mx9, genero tarea de actuazalición de firmware si existe nuevo instalador
            If bIsMX9 Then
                Dim sql As String = $"@SELECT# FirmwareUpgradeAvailable FROM Terminals WHERE ID = {_Terminal.ID}"
                Dim mustUpdate As Boolean = roTypes.Any2Boolean(AccessHelper.ExecuteScalar(sql))

                If mustUpdate Then
                    sql = $"@UPDATE# terminals SET FirmwareUpgradeAvailable = 0 WHERE ID = {_Terminal.ID}"
                    AccessHelper.ExecuteSql(sql)
                    AddTaskEx(Robotics.Comms.Base.roTerminalsSyncTasks.SyncActions.updatefirmware, 0)
                End If

                If slTasksEx.ContainsKey(roTerminalsSyncTasks.SyncActions.updatefirmware) Then
                    Dim otask1 As roTerminalsSyncTasks = New roTerminalsSyncTasks(IDTerminal)
                    For Each strTask As String In slTasksEx(roTerminalsSyncTasks.SyncActions.updatefirmware)
                        otask1.addSyncTask(roTerminalsSyncTasks.SyncActions.updatefirmware, strTask.Split("|")(0), strTask.Split("|")(1), , strTask.Split("|")(2), strTask.Split("|")(3), strTask.Split("|")(4), deleteExistingTasks)
                    Next
                End If
            End If

            If fConfig.HasChanged(_Terminal.ID) Then
                fConfig.SaveToDataBase(_Terminal.ID)
                If bIsMX9 OrElse bIsMX8 Then
                    AddTaskEx(Robotics.Comms.Base.roTerminalsSyncTasks.SyncActions.setterminalconfig, 0, , 0, 0, fConfig.ToXml)
                End If
            End If

            If slTasksEx.ContainsKey(roTerminalsSyncTasks.SyncActions.setterminalconfig) Then
                Dim otask1 As roTerminalsSyncTasks = New roTerminalsSyncTasks(IDTerminal)
                For Each strTask As String In slTasksEx(roTerminalsSyncTasks.SyncActions.setterminalconfig)
                    otask1.addSyncTask(roTerminalsSyncTasks.SyncActions.setterminalconfig, strTask.Split("|")(0), strTask.Split("|")(1), , strTask.Split("|")(2), strTask.Split("|")(3), strTask.Split("|")(4), deleteExistingTasks)
                Next
            End If

            UpdateEmployeesMX7plusToDatabase(IDTerminal, bolCheckAcc)
            Dim bCreateSirensTask As Boolean = False
                bCreateSirensTask = (_Terminal.Type = TerminalData.eTerminalType.mx9 OrElse _Terminal.Type = TerminalData.eTerminalType.mx8)

            'No hace falta sirenas en external, asi que esto nada.
            UpdateSirensToDatabase(IDTerminal, bCreateSirensTask)
            If bCreateSirensTask Then
                Dim otask As roTerminalsSyncTasks = New roTerminalsSyncTasks(IDTerminal)
                If slTasksEx.ContainsKey(roTerminalsSyncTasks.SyncActions.setsirens) Then
                    For Each strTask As String In slTasksEx(roTerminalsSyncTasks.SyncActions.setsirens)
                        otask.addSyncTask(roTerminalsSyncTasks.SyncActions.setsirens, strTask.Split("|")(0), strTask.Split("|")(1), , strTask.Split("|")(2), strTask.Split("|")(3), strTask.Split("|")(4), deleteExistingTasks)
                    Next
                End If
            End If

            If sBehavior.IndexOf("ACC") > -1 AndAlso sBehavior <> "TAACC" Then
                UpdateAccessToDatabase(IDTerminal)
                UpdateTimeZonesToDatabase(IDTerminal, False)
            Else
                'Creamos lo fichero vacios para asegurar que se borran los temas de accesos
                UpdateAccessToDatabase(IDTerminal)
                UpdateTimeZonesToDatabase(IDTerminal, True)
            End If

            'Si venimos del external no lo ejecutamos
            UpdateCausesToDatabase()

            ' Para modos de presencia y terminales mx8, se debe diferenciar TA (Entrada o Salida rápido), TAE (Solo Entrada rápida), TAS (Solo Salida rápida), TAX (Modo Smart)
            If sBehavior.Contains("TA") Then
                sBehavior = sBehavior.Replace("TA", "TA" + _Terminal.RDRInteractionAction(1))
            End If

            ' Marcamos en tabla terminales que puede que haya cambios para que quien lo crea necesario recargue definición ... (mx9, ... y posteriores)
            Dim ostate As New VTBusiness.Terminal.roTerminalState(1)
            VTBusiness.Terminal.roTerminal.SetForceConfig(_Terminal.ID, ostate)

        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, $"BroadcasterNet::UpdateFilesMx7ToDatabase::Terminal {_TerminalID}:: Unexpected error: ", ex)
        End Try

    End Sub

    Private Sub UpdateFilesMxaPlusToDataBase(ByVal IDTerminal As Integer)

        Try

            ' Los lectores de la centralita siempre tienen comportamiento de accesos
            Dim bolCheckAcc As Boolean = True
            Dim fConfig As TerminalConfigFile = New TerminalConfigFile

            Dim tbReaders As Data.DataTable = oBCData.GetTerminalReaders(IDTerminal)
            If tbReaders IsNot Nothing Then
                For Each oRow As DataRow In tbReaders.Rows
                    bolCheckAcc = (oRow("Mode").ToString.IndexOf("ACC") >= 0)
                    If bolCheckAcc Then
                        Select Case oRow("ID")
                            Case 1
                                fConfig.Add(TerminalConfigFile.eParameterNames.RDR1OpenTime, oRow("Duration").ToString)
                                fConfig.Add(TerminalConfigFile.eParameterNames.Door1ValidTZ, "999999")
                                fConfig.Add(TerminalConfigFile.eParameterNames.Door1Intertime, "0")
                            Case 2
                                fConfig.Add(TerminalConfigFile.eParameterNames.RDR2OpenTime, oRow("Duration").ToString)
                                fConfig.Add(TerminalConfigFile.eParameterNames.Door2ValidTZ, "999999")
                                fConfig.Add(TerminalConfigFile.eParameterNames.Door2Intertime, "0")
                            Case 3
                                fConfig.Add(TerminalConfigFile.eParameterNames.RDR3OpenTime, oRow("Duration").ToString)
                                fConfig.Add(TerminalConfigFile.eParameterNames.Door3ValidTZ, "999999")
                                fConfig.Add(TerminalConfigFile.eParameterNames.Door3Intertime, "0")
                            Case 4
                                fConfig.Add(TerminalConfigFile.eParameterNames.RDR4OpenTime, oRow("Duration").ToString)
                                fConfig.Add(TerminalConfigFile.eParameterNames.Door4ValidTZ, "999999")
                                fConfig.Add(TerminalConfigFile.eParameterNames.Door4Intertime, "0")
                            Case Else

                        End Select
                    End If
                    fConfig.Add(TerminalConfigFile.eParameterNames.Mode, (roTypes.Any2String(oRow("Mode"))).ToString)
                    fConfig.Add(TerminalConfigFile.eParameterNames.TimezoneName, (roTypes.Any2String(oRow("TimeZoneName"))).ToString)
                    fConfig.Add(TerminalConfigFile.eParameterNames.IsDifferentZoneTime, (roTypes.Any2String(oRow("IsDifferentZoneTime"))).ToString)
                    fConfig.Add(TerminalConfigFile.eParameterNames.AutoDaylight, (roTypes.Any2String(oRow("AutoDaylight"))).ToString)
                    fConfig.Add(TerminalConfigFile.eParameterNames.VTVersion, "2." & Date.Now.Month.ToString) 'Forzamos un setterminalconfig al menos una vez al mes, para asegurar que la configuración de DST se actualiza
                Next
            End If

            If fConfig.HasChanged(IDTerminal) Then
                fConfig.SaveToDataBase(IDTerminal)
                ' Hubo cambio. Creo tarea
                AddTaskEx(Robotics.Comms.Base.roTerminalsSyncTasks.SyncActions.setterminalconfig, 0, , 0, 0, fConfig.ToXml)
            End If

            Dim otask As roTerminalsSyncTasks = New roTerminalsSyncTasks(IDTerminal)

            If slTasksEx.ContainsKey(roTerminalsSyncTasks.SyncActions.setterminalconfig) Then
                For Each strTask As String In slTasksEx(roTerminalsSyncTasks.SyncActions.setterminalconfig)
                    otask.addSyncTask(roTerminalsSyncTasks.SyncActions.setterminalconfig, strTask.Split("|")(0), strTask.Split("|")(1), , strTask.Split("|")(2), strTask.Split("|")(3), strTask.Split("|")(4), deleteExistingTasks)
                Next
            End If

            If _Terminal.Type = TerminalData.eTerminalType.mcxinbio OrElse _Terminal.Type = TerminalData.eTerminalType.mxs Then
                Dim aVirtualTimeZones As New ArrayList
                UpdateEmployeesMxaPlusToDatabase(IDTerminal, True, aVirtualTimeZones)
                UpdateTimeZonesMxaPlusToDatabase(IDTerminal, aVirtualTimeZones)
            End If

            ' Marcamos en tabla terminales que puede que haya cambios para que quien lo crea necesario recargue definición ... (rx1, rxF, ... y posteriores)
            Dim ostate As New Terminal.roTerminalState(-1)
            Terminal.roTerminal.SetForceConfig(_Terminal.ID, ostate)
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, $"BroadcasterNet::UpdateFilesMxaPlus::Terminal {_TerminalID}:: Unexpected error: ", ex)
        End Try

    End Sub

    Private Sub UpdateFilesZKPush2ToDatabase(ByVal IDTerminal As Integer)

        Try
            ' Los lectores de la centralita siempre tienen comportamiento de accesos
            Dim bolCheckAcc As Boolean = False
            Dim sBehavior As String = ""
            Dim fConfig As TerminalConfigFile = New TerminalConfigFile

            Dim tbReaders As Data.DataTable = oBCData.GetTerminalReaders(IDTerminal)
            If tbReaders IsNot Nothing Then
                For Each oRow As DataRow In tbReaders.Rows
                    bolCheckAcc = (oRow("Mode").ToString.IndexOf("ACC") >= 0)
                    Select Case oRow("ID")
                        Case 1
                            If _Terminal.Type = TerminalData.eTerminalType.rxcp OrElse _Terminal.Type = TerminalData.eTerminalType.rxcep Then
                                ' rxC y rxCe PUSH aplican una corrección en el tiempo de appertura de relé
                                fConfig.Add(TerminalConfigFile.eParameterNames.LockOn, (roTypes.Any2Integer(oRow("Duration")) * 20).ToString)
                            Else
                                fConfig.Add(TerminalConfigFile.eParameterNames.LockOn, (roTypes.Any2Integer(oRow("Duration"))).ToString)
                                fConfig.Add(TerminalConfigFile.eParameterNames.TimezoneName, (roTypes.Any2String(oRow("TimeZoneName"))).ToString)
                                fConfig.Add(TerminalConfigFile.eParameterNames.IsDifferentZoneTime, (roTypes.Any2String(oRow("IsDifferentZoneTime"))).ToString)
                            End If
                        Case Else
                    End Select
                    sBehavior += Any2String(oRow("Mode")) + Any2String(oRow("InteractionAction")) + vbCrLf
                Next
            End If

            ' En todo caso, añado parámetros constantes
            If _Terminal.Type = TerminalData.eTerminalType.rx1 OrElse _Terminal.Type = TerminalData.eTerminalType.rxfp Then
                ' Para terminales rx1 hay que indicarle que debe enviar los fichajes denegados por fuera de periodo
                fConfig.Add(TerminalConfigFile.eParameterNames.SaveFailedLog, "4") ' 4-> envía fuera de periodo, 8 -> combinación incorrecta, 12 -> las dos anteriores
            End If

            ' Para forzar que se fije una clave de comunicaciones
            fConfig.Add(TerminalConfigFile.eParameterNames.VTVersion, "2." & Date.Now.Month.ToString) 'El valor fuerza cambio en terminalconfig, que luego el driver usa para actualizar lo que corresponda

            'NESTOR: Este Save no debería devolver True si no hubieron cambios ...
            If fConfig.HasChanged(IDTerminal) Then
                fConfig.SaveToDataBase(IDTerminal)
                ' Hubo cambio. Creo tarea
                AddTaskEx(Robotics.Comms.Base.roTerminalsSyncTasks.SyncActions.setterminalconfig, 0, , 0, 0, fConfig.ToXml)
            End If

            Dim otask As roTerminalsSyncTasks = New roTerminalsSyncTasks(IDTerminal)

            If slTasksEx.ContainsKey(roTerminalsSyncTasks.SyncActions.setterminalconfig) Then
                For Each strTask As String In slTasksEx(roTerminalsSyncTasks.SyncActions.setterminalconfig)
                    otask.addSyncTask(roTerminalsSyncTasks.SyncActions.setterminalconfig, strTask.Split("|")(0), strTask.Split("|")(1), , strTask.Split("|")(2), strTask.Split("|")(3), strTask.Split("|")(4), deleteExistingTasks)
                Next
            End If

            Dim aVirtualTimeZones As New Dictionary(Of Integer, ArrayList)
            ' Primero definición de timezones. Si no, las tareas de asignación de TZ a empleados fallarían.
            UpdateTimeZonesZKPush2ToDatabase(IDTerminal, aVirtualTimeZones, bolCheckAcc)
            UpdateEmployeesZKPush2ToDatabase(IDTerminal, bolCheckAcc, aVirtualTimeZones)
            UpdateSirensToDatabase(IDTerminal, True)
            If slTasksEx.ContainsKey(roTerminalsSyncTasks.SyncActions.setsirens) Then
                For Each strTask As String In slTasksEx(roTerminalsSyncTasks.SyncActions.setsirens)
                    otask.addSyncTask(roTerminalsSyncTasks.SyncActions.setsirens, strTask.Split("|")(0), strTask.Split("|")(1), , strTask.Split("|")(2), strTask.Split("|")(3), strTask.Split("|")(4), deleteExistingTasks)
                Next
            End If

            ' Lista de justificaciones para rx1
            UpdateCausesToDatabase(True)

            Dim sAdminPWD As String = String.Empty
            sAdminPWD = roCacheManager.GetInstance.GetAdvParametersCache(Azure.RoAzureSupport.GetCompanyName, "PushTerminalsAdminPWD")
            If Not (sAdminPWD.Trim.Length = 4 AndAlso IsNumeric(sAdminPWD)) Then sAdminPWD = "9999"

            ' Marcamos en tabla terminales que puede que haya cambios para que quien lo crea necesario recargue definición ... (rx1, rxF, ... y posteriores)
            Dim ostate As New Terminal.roTerminalState(-1)
            Terminal.roTerminal.SetForceConfig(_Terminal.ID, ostate)
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, $"BroadcasterNet::UpdateFilesZKPush2ToDatabase::Terminal {_TerminalID}:: Unexpected error: ", ex)
        End Try

    End Sub

    Private Sub UpdateEmployeesMX7plusToDatabase(ByVal IDTerminal As Integer,
                                                 ByVal CheckAcc As Boolean)

        Try

            Dim fCards As New CardsFile
            Dim fGroup As New GroupEmployeesFile
            Dim fBio As New BiodataBinFile
            Dim fEmployees As New EmployeesFile
            Dim fDocument As New DocumentFile
            Dim oDT As New Data.DataTable
            Dim row As Data.DataRow

            oDT = oBCData.GetEmployeesLive(IDTerminal, AdvancedAccessMode, CheckAcc OrElse (RestrictedAttendance AndAlso Not LegacyRestrictedModeUsed))

            Dim strName As String = ""
            Dim oImage As Image = Nothing
            Dim strImageCRC As String = String.Empty
            Dim strLanguageKey As String
            Dim strPIN As String = ""
            Dim sAllowedCauses As String = ""
            Dim bIsOnline As Boolean = False
            Dim bEmployeeLimitedCauses As Boolean = False
            Dim EmployeeOnTerminal As List(Of Integer) = New List(Of Integer)
            Dim dEmployeeCauses As New Dictionary(Of Integer, List(Of String))

            DelUserTaskGeneric("USERTASK:\\TERMINAL_BROADCAST_ERROR", _Terminal.ID)

            fCards.ConvertCardID = False

            'TODO: REVISAR: Si no usa restricción de presenica Legacy, ni hay control de PRL por documentos, ni se tiene en cuenta el campo de la ficha ENTRA para denegar el acceso, EmployeePermit no aporta nada sobre la función GetEmployeesLive filtrada por parámetros de accesos
            '      Se podría llenar el diccionario dEmployeePermit con los empleados de oDT, y bdicEmployeePermit = True
            '      Sería más que conveniente porque en entornos cargados, la función EmployeePermit suele arrojar errores, y en ese caso acaba retornando True
            Dim dEmployeePermit As New Dictionary(Of Integer, Boolean)
            Dim idEmployee As Integer
            Dim bdicEmployeePermit As Boolean
            For Each row In oDT.Rows
                idEmployee = row.Item("EmployeeID")
                bdicEmployeePermit = _Terminal.EmployeePermit(row.Item("EmployeeID"), , AdvancedAccessMode)
                If Not dEmployeePermit.ContainsKey(idEmployee) Then dEmployeePermit.Add(idEmployee, bdicEmployeePermit)
                If bdicEmployeePermit Then
                    If Not EmployeeOnTerminal.Contains(idEmployee) Then EmployeeOnTerminal.Add(idEmployee)
                End If
            Next

            bEmployeeLimitedCauses = oBCData.SomeCausesDependsOnEmployee()
            If bEmployeeLimitedCauses Then
                Dim oCauseState As New VTBusiness.Cause.roCauseState
                dEmployeeCauses = VTBusiness.Cause.roCause.GetEmployeeCausesByInputPermissions(oCauseState,, EmployeeOnTerminal)
            End If

            For Each row In oDT.Rows
                Dim bEmployeePermit As Boolean = False
                bEmployeePermit = (dEmployeePermit.ContainsKey(row.Item("EmployeeID")) AndAlso dEmployeePermit(row.Item("EmployeeID")))
                If bEmployeePermit Then
                    'Si tiene permitido fichar por tarjeta guardamos la tarjeta

                    strName = roTypes.Any2String(row.Item("Name"))

                    'oBCData.GetEmployeeHasLimitedPermit(_TerminalID, row.Item("EmployeeID")) Then
                    strPIN = Any2String(row.Item("PINData"))

                    If Not bEmployeeLimitedCauses Then
                        ' Todos los empleados pueden usar las mismas justificaciones
                        sAllowedCauses = ""
                    Else
                        ' Algunos empleados podrán fichar unas justificaciones y otros otras, en función de su ficha
                        ' Indico un - 1 para señalar que no tiene acceso a ninguna justificación
                        sAllowedCauses = "-1"
                        'Dim sAllowedCausesex As String
                        'sAllowedCausesex = oBCData.GetEmployeeAllowedCausesForPunch(row.Item("EmployeeID"))
                        If dEmployeeCauses.ContainsKey(row.Item("EmployeeID")) Then
                            sAllowedCauses = String.Join(",", dEmployeeCauses(row.Item("EmployeeID")))
                        End If
                    End If

                    Dim oEmpState As VTEmployees.Employee.roEmployeeState = New VTEmployees.Employee.roEmployeeState
                    bIsOnline = roTerminalMessage.PendingTerminalMessages(row.Item("EmployeeID"), oEmpState)

                    'Guardamos las tarjetas
                    If Not IsDBNull(row.Item("RealValue")) Then
                        fCards.Add(row.Item("EmployeeID"), row.Item("RealValue"))
                    ElseIf Not IsDBNull(row.Item("IDCard")) Then
                        fCards.Add(row.Item("EmployeeID"), row.Item("IDCard"))
                    End If

                    'General para Live y Pro
                    'Si hay imagen la guardamos
                    Dim rowImage As Data.DataRow = oBCData.GetEmployeeImage(row.Item("EmployeeID"))
                    strImageCRC = String.Empty
                    If Not IsDBNull(rowImage.Item("Image")) Then
                        'oImage = roTypes.Bytes2Image(rowImage.Item("Image"))
                        oImage = Nothing
                        Dim oEmpFile As New EmployeesFile
                        Try
                            strImageCRC = CryptographyHelper.EncryptWithMD5(Convert.ToBase64String(roTypes.Image2Bytes(oEmpFile.FixedSize(roTypes.Bytes2Image(rowImage.Item("Image")), 200, 200, True))))
                        Catch ex As Exception
                            roLog.GetInstance().logMessage(roLog.EventType.roError, $"BroadcasterNet::UpdateEmployeesMX7plusToDatabase:{_Terminal.ToString}. Unable to load employee photo for employee ID = {roTypes.Any2String(row.Item("EmployeeID"))}", ex)
                        End Try
                    Else
                        oImage = Nothing
                    End If

                    strLanguageKey = roTypes.Any2String(row.Item("LanguageKey"))

                    fEmployees.Add(row.Item("EmployeeID"), strName, oImage, strLanguageKey, strPIN, , sAllowedCauses, bIsOnline, False, strImageCRC)

                    'Si esta asignado a algun grupo de accesso lo guardamos
                    ' Miramos si se trata de modo avanzado de accesos
                    If Not AdvancedAccessMode Then
                        'Modo de accesos Clásico
                        If Not row.Item("GroupID").Equals(DBNull.Value) AndAlso CheckAcc Then '
                            fGroup.Add(row.Item("EmployeeID"), row.Item("GroupID"))
                        End If
                    Else
                        'Modo de accesos Avanzado
                        If CheckAcc Then
                            Dim aAuthorizations As ArrayList = oBCData.GetEmployeeAccessAuthorization(row.Item("EmployeeID"), IDTerminal, 1)
                            For Each iAuthorizationID As Integer In aAuthorizations
                                fGroup.Add(row.Item("EmployeeID"), iAuthorizationID)
                            Next
                        End If
                    End If
                End If
            Next

            'Guardamos las huellas
            Dim dtBio As Data.DataTable = oBCData.GetEmployeesAllBio_Live(_BiometricVersion, AdvancedAccessMode, 0, IIf(CheckAcc OrElse (RestrictedAttendance AndAlso Not LegacyRestrictedModeUsed), _TerminalID, 0))
            Dim rowLive As Data.DataRow
            For Each rowLive In dtBio.Rows
                If dEmployeePermit.ContainsKey(rowLive.Item("EmployeeID")) AndAlso dEmployeePermit(rowLive.Item("EmployeeID")) Then
                    If Not IsDBNull(rowLive.Item("BioData")) Then
                        fBio.Add(rowLive.Item("EmployeeID"), rowLive.Item("BiometricID"), rowLive.Item("BioData"), "RXFFNG")
                    End If
                End If
            Next

            'Comprobamos PRL
            UpdateTerminalDocumentsMx7pToDatabase(fDocument, IDTerminal, EmployeeOnTerminal)

            'Generamos los ficheros
            fEmployees.CompareXmlFromDatabase(_Terminal.ID, Me)
            fCards.CompareXmlFromDatabase(_Terminal.ID, Me)
            fBio.CompareXmlFromDatabase(_Terminal.ID, Me, , False)
            fGroup.CompareXmlFromDatabase(_Terminal.ID, Me)
            fDocument.CompareXmlFromDatabase(_Terminal.ID, Me)

            Dim otask As roTerminalsSyncTasks = New roTerminalsSyncTasks(IDTerminal)
            otask.SetBatchSize(broadcasterTaskBatchSize)

            ' Si la biometría está deshabilitada a nivel general, y tengo alguna instrucción individual de borrado de biometría, las obvio todas y creo la tarea delall correspondiente
            Dim bioDisabled As Boolean = False
            Dim oParameters As New roParameters("OPTIONS", True)

            'Si esta activado la biometria globalmente y le pasas por parametro True, creamos biometria
            bioDisabled = roTypes.Any2Boolean(oParameters.Parameter(Parameters.DisableBiometricData))

            If slTasksmx.ContainsKey(roTerminalsSyncTasks.SyncActions.delallphotos) Then
                For Each strTask As String In slTasksmx(roTerminalsSyncTasks.SyncActions.delallphotos)
                    otask.addSyncTask(roTerminalsSyncTasks.SyncActions.delallphotos, strTask.Split("|")(0), strTask.Split("|")(1), -10,,,, deleteExistingTasks)
                Next
            End If
            If slTasksmx.ContainsKey(roTerminalsSyncTasks.SyncActions.delallbios) Then
                For Each strTask As String In slTasksmx(roTerminalsSyncTasks.SyncActions.delallbios)
                    otask.addSyncTask(roTerminalsSyncTasks.SyncActions.delallbios, strTask.Split("|")(0), strTask.Split("|")(1), -10,,,, deleteExistingTasks)
                Next
            End If
            If slTasksmx.ContainsKey(roTerminalsSyncTasks.SyncActions.delallcards) Then
                For Each strTask As String In slTasksmx(roTerminalsSyncTasks.SyncActions.delallcards)
                    otask.addSyncTask(roTerminalsSyncTasks.SyncActions.delallcards, strTask.Split("|")(0), strTask.Split("|")(1), -10,,,, deleteExistingTasks)
                Next
            End If
            If slTasksmx.ContainsKey(roTerminalsSyncTasks.SyncActions.delallemployeegroup) Then
                For Each strTask As String In slTasksmx(roTerminalsSyncTasks.SyncActions.delallemployeegroup)
                    otask.addSyncTask(roTerminalsSyncTasks.SyncActions.delallemployeegroup, strTask.Split("|")(0), strTask.Split("|")(1), -10,,,, deleteExistingTasks)
                Next
            End If
            If slTasksmx.ContainsKey(roTerminalsSyncTasks.SyncActions.delallemployees) Then
                For Each strTask As String In slTasksmx(roTerminalsSyncTasks.SyncActions.delallemployees)
                    otask.addSyncTask(roTerminalsSyncTasks.SyncActions.delallemployees, strTask.Split("|")(0), strTask.Split("|")(1), -10,,,, deleteExistingTasks)
                Next
            End If
            If slTasksmx.ContainsKey(roTerminalsSyncTasks.SyncActions.delalldocuments) Then
                For Each strTask As String In slTasksmx(roTerminalsSyncTasks.SyncActions.delalldocuments)
                    otask.addSyncTask(roTerminalsSyncTasks.SyncActions.delalldocuments, strTask.Split("|")(0), strTask.Split("|")(1), -10,,,, deleteExistingTasks)
                Next
            End If

            If slTasksmx.ContainsKey(roTerminalsSyncTasks.SyncActions.delphoto) Then
                For Each strTask As String In slTasksmx(roTerminalsSyncTasks.SyncActions.delphoto)
                    otask.addSyncTask(roTerminalsSyncTasks.SyncActions.delphoto, strTask.Split("|")(0), strTask.Split("|")(1), -5,,,, deleteExistingTasks)
                Next
            End If

            If slTasksmx.ContainsKey(roTerminalsSyncTasks.SyncActions.delbio) Then
                If Not bioDisabled Then
                    For Each strTask As String In slTasksmx(roTerminalsSyncTasks.SyncActions.delbio)
                        otask.addSyncTask(roTerminalsSyncTasks.SyncActions.delbio, strTask.Split("|")(0), strTask.Split("|")(1), -5,,,, deleteExistingTasks)
                    Next
                Else
                    otask.addSyncTask(roTerminalsSyncTasks.SyncActions.delallbios, 0, 0, -10,,,, deleteExistingTasks)
                End If
            End If
            If slTasksmx.ContainsKey(roTerminalsSyncTasks.SyncActions.delcard) Then
                For Each strTask As String In slTasksmx(roTerminalsSyncTasks.SyncActions.delcard)
                    otask.addSyncTask(roTerminalsSyncTasks.SyncActions.delcard, strTask.Split("|")(0), strTask.Split("|")(1), -5,,,, deleteExistingTasks)
                Next
            End If
            If slTasksmx.ContainsKey(roTerminalsSyncTasks.SyncActions.delemployeegroup) Then
                For Each strTask As String In slTasksmx(roTerminalsSyncTasks.SyncActions.delemployeegroup)
                    otask.addSyncTask(roTerminalsSyncTasks.SyncActions.delemployeegroup, strTask.Split("|")(0), strTask.Split("|")(1), -5,,,, deleteExistingTasks)
                Next
            End If
            If slTasksmx.ContainsKey(roTerminalsSyncTasks.SyncActions.deldocument) Then
                For Each strTask As String In slTasksmx(roTerminalsSyncTasks.SyncActions.deldocument)
                    otask.addSyncTask(roTerminalsSyncTasks.SyncActions.deldocument, strTask.Split("|")(0), strTask.Split("|")(1), -5,,,, deleteExistingTasks)
                Next
            End If
            If slTasksmx.ContainsKey(roTerminalsSyncTasks.SyncActions.delemployee) Then
                For Each strTask As String In slTasksmx(roTerminalsSyncTasks.SyncActions.delemployee)
                    otask.addSyncTask(roTerminalsSyncTasks.SyncActions.delemployee, strTask.Split("|")(0), strTask.Split("|")(1), -5,,,, deleteExistingTasks)
                Next
            End If
            If _Terminal.Type = TerminalData.eTerminalType.mx9 Then
                If slTasksmx.ContainsKey(roTerminalsSyncTasks.SyncActions.refreshallbios) Then
                    For Each strTask As String In slTasksmx(roTerminalsSyncTasks.SyncActions.refreshallbios)
                        otask.addSyncTask(roTerminalsSyncTasks.SyncActions.refreshallbios, strTask.Split("|")(0), strTask.Split("|")(1), -5,,,, deleteExistingTasks)
                    Next
                End If
            End If

            Threading.Thread.Sleep(1000)

            If slTasksEx.ContainsKey(roTerminalsSyncTasks.SyncActions.addemployee) Then
                For Each strTask As String In slTasksEx(roTerminalsSyncTasks.SyncActions.addemployee)
                    If (dEmployeePermit.ContainsKey(Integer.Parse(strTask.Split("|")(0))) AndAlso dEmployeePermit(Integer.Parse(strTask.Split("|")(0)))) Then
                        otask.addSyncTask(roTerminalsSyncTasks.SyncActions.addemployee, strTask.Split("|")(0), strTask.Split("|")(1),,,, strTask.Split("|")(4), deleteExistingTasks)
                    Else
                        roLog.GetInstance().logMessage(roLog.EventType.roDebug, $"BroadcasterNet::UpdateEmployeesMX7plusToDatabase:{_Terminal.ToString} Task not created because employee not permit.({roTerminalsSyncTasks.SyncActions.addemployee},{strTask.Split("|")(0)})")
                    End If
                Next
            End If
            If slTasksmx.ContainsKey(roTerminalsSyncTasks.SyncActions.addcard) Then
                For Each strTask As String In slTasksmx(roTerminalsSyncTasks.SyncActions.addcard)
                    If (dEmployeePermit.ContainsKey(Integer.Parse(strTask.Split("|")(0))) AndAlso dEmployeePermit(Integer.Parse(strTask.Split("|")(0)))) Then
                        otask.addSyncTask(roTerminalsSyncTasks.SyncActions.addcard, strTask.Split("|")(0), strTask.Split("|")(1),,,,, deleteExistingTasks)
                    Else
                        roLog.GetInstance().logMessage(roLog.EventType.roDebug, $"BroadcasterNet::UpdateEmployeesMX7plusToDatabase:{_Terminal.ToString} Task not created because employee not permit.({roTerminalsSyncTasks.SyncActions.addcard},{strTask.Split("|")(0)})")
                    End If
                Next
            End If
            If slTasksmx.ContainsKey(roTerminalsSyncTasks.SyncActions.addbio) Then
                For Each strTask As String In slTasksmx(roTerminalsSyncTasks.SyncActions.addbio)
                    If (dEmployeePermit.ContainsKey(Integer.Parse(strTask.Split("|")(0))) AndAlso dEmployeePermit(Integer.Parse(strTask.Split("|")(0)))) Then
                        otask.addSyncTask(roTerminalsSyncTasks.SyncActions.addbio, strTask.Split("|")(0), strTask.Split("|")(1),,,,, deleteExistingTasks)
                    Else
                        roLog.GetInstance().logMessage(roLog.EventType.roDebug, $"BroadcasterNet::UpdateEmployeesMX7plusToDatabase:{_Terminal.ToString} Task not created because employee not permit.({roTerminalsSyncTasks.SyncActions.addbio},{strTask.Split("|")(0)})")
                    End If
                Next
            End If
            If slTasksmx.ContainsKey(roTerminalsSyncTasks.SyncActions.addphoto) Then
                For Each strTask As String In slTasksmx(roTerminalsSyncTasks.SyncActions.addphoto)
                    If (dEmployeePermit.ContainsKey(Integer.Parse(strTask.Split("|")(0))) AndAlso dEmployeePermit(Integer.Parse(strTask.Split("|")(0)))) Then
                        otask.addSyncTask(roTerminalsSyncTasks.SyncActions.addphoto, strTask.Split("|")(0), strTask.Split("|")(1),,,,, deleteExistingTasks)
                    Else
                        roLog.GetInstance().logMessage(roLog.EventType.roDebug, $"BroadcasterNet::UpdateEmployeesMX7plusToDatabase:{_Terminal.ToString} Task not created because employee not permit.({roTerminalsSyncTasks.SyncActions.addphoto},{strTask.Split("|")(0)})")
                    End If
                Next
            End If
            If slTasksmx.ContainsKey(roTerminalsSyncTasks.SyncActions.addemployeegroup) Then
                For Each strTask As String In slTasksmx(roTerminalsSyncTasks.SyncActions.addemployeegroup)
                    If (dEmployeePermit.ContainsKey(Integer.Parse(strTask.Split("|")(0))) AndAlso dEmployeePermit(Integer.Parse(strTask.Split("|")(0)))) Then
                        otask.addSyncTask(roTerminalsSyncTasks.SyncActions.addemployeegroup, strTask.Split("|")(0), strTask.Split("|")(1),,,,, deleteExistingTasks)
                    Else
                        roLog.GetInstance().logMessage(roLog.EventType.roDebug, $"BroadcasterNet::UpdateEmployeesMX7plusToDatabase:{_Terminal.ToString} Task not created because employee not permit.({roTerminalsSyncTasks.SyncActions.addemployeegroup},{strTask.Split("|")(0)})")
                    End If
                Next
            End If
            If slTasksmx.ContainsKey(roTerminalsSyncTasks.SyncActions.adddocument) Then
                For Each strTask As String In slTasksmx(roTerminalsSyncTasks.SyncActions.adddocument)
                    If (dEmployeePermit.ContainsKey(Integer.Parse(strTask.Split("|")(0))) AndAlso dEmployeePermit(Integer.Parse(strTask.Split("|")(0)))) Then
                        otask.addSyncTask(roTerminalsSyncTasks.SyncActions.adddocument, strTask.Split("|")(0), strTask.Split("|")(1),,,,, deleteExistingTasks)
                    Else
                        roLog.GetInstance().logMessage(roLog.EventType.roDebug, $"BroadcasterNet::UpdateEmployeesMX7plusToDatabase:{_Terminal.ToString} Task not created because employee not permit.({roTerminalsSyncTasks.SyncActions.adddocument},{strTask.Split("|")(0)})")
                    End If
                Next
            End If

            Dim persistSyncTables As Boolean = True
            If otask.WorkMode = roTerminalsSyncTasks.eDbWorkMode.Batch Then persistSyncTables = otask.PersistTasksToDatabase()

            ' Guardamos ficheros si procede
            If persistSyncTables Then
                If fEmployees.HasChange Then fEmployees.SaveToDataBase(_Terminal.ID, Me)
                If fCards.HasChange Then fCards.SaveToDataBase(_Terminal.ID)
                If fBio.HasChange Then fBio.SaveToDataBase(_Terminal.ID)
                If fGroup.HasChange Then fGroup.SaveToDataBase(_Terminal.ID)
                If fDocument.HasChange Then fDocument.SaveToDataBase(_Terminal.ID)
            Else
                roLog.GetInstance().logMessage(roLog.EventType.roError, $"BroadcasterNet::UpdateEmployeesMX7plusToDatabase: Error persisting tasks for terminal id {_Terminal}. Will try next time, no later than {DateTime.Now.AddHours(1).ToString("yyyy-MM-dd HH:00:00")}")
                CreateUserTaskGeneric("USERTASK:\\TERMINAL_BROADCAST_ERROR", "TerminalBroadcastError.Title", _Terminal.ID)
            End If

        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, $"BroadcasterNet::UpdateEmployeesMX7plusToDatabase:{_Terminal.ToString} Unexpected error: ", ex)
            CreateUserTaskGeneric("USERTASK:\\TERMINAL_BROADCAST_ERROR", "TerminalBroadcastError.Title", _Terminal.ID)
        End Try
    End Sub

    Private Sub UpdateEmployeesMxaPlusToDatabase(ByVal IDTerminal As Integer, ByVal CheckAcc As Boolean, ByRef aVirtualTimeZones As ArrayList)

        Try
            Dim fCards As New CardsFile
            Dim fEmployees As New EmployeesFile
            Dim fEmployeeAccessLevel As New EmployeeAccessLevelMxapFile
            Dim oDT As New Data.DataTable
            Dim row As Data.DataRow
            Dim oDTAuxAccessPeriod As New Data.DataTable

            DelUserTaskGeneric("USERTASK:\\TERMINAL_BROADCAST_ERROR", _Terminal.ID)

            oDT = oBCData.GetEmployeesLive(IDTerminal, AdvancedAccessMode, CheckAcc, True)

            ' Obtenemos información sobre periodos de accesos asociados al lector del terminal
            If CheckAcc Then oDTAuxAccessPeriod = oBCData.GetEmployeeAccessLevelMxaAdvancedMode_Data(IDTerminal, AdvancedAccessMode)

            Dim strName As String = ""
            Dim strLanguageKey As String
            Dim strPIN As String = ""
            Dim EmployeeOnTerminal As List(Of Integer) = New List(Of Integer)

            Dim bEmployeePermit As Boolean = False

            fCards.ConvertCardID = False

            Dim dEmployeePermit As New Dictionary(Of Integer, Boolean)
            Dim idEmployee As Integer
            Dim bdicEmployeePermit As Boolean
            For Each row In oDT.Rows
                idEmployee = row.Item("EmployeeID")
                bdicEmployeePermit = (_Terminal.EmployeePermit(row.Item("EmployeeID"), 1, AdvancedAccessMode) OrElse _Terminal.EmployeePermit(row.Item("EmployeeID"), 2, AdvancedAccessMode) OrElse _Terminal.EmployeePermit(row.Item("EmployeeID"), 3, AdvancedAccessMode) OrElse _Terminal.EmployeePermit(row.Item("EmployeeID"), 4, AdvancedAccessMode))
                If Not dEmployeePermit.ContainsKey(idEmployee) Then dEmployeePermit.Add(idEmployee, bdicEmployeePermit)
            Next

            For Each row In oDT.Rows
                bEmployeePermit = dEmployeePermit(row.Item("EmployeeID"))
                If bEmployeePermit Then

                    'Visualtime Live
                    If bEmployeePermit Then
                        If Not EmployeeOnTerminal.Contains(row.Item("EmployeeID")) Then EmployeeOnTerminal.Add(row.Item("EmployeeID"))

                        strName = roTypes.Any2String(row.Item("Name"))
                        strPIN = Any2String(row.Item("PINData"))
                        'Guardamos las tarjetas
                        If Not IsDBNull(row.Item("RealValue")) Then
                            fCards.Add(row.Item("EmployeeID"), row.Item("RealValue"))
                        ElseIf Not IsDBNull(row.Item("IDCard")) Then
                            fCards.Add(row.Item("EmployeeID"), row.Item("IDCard"))
                        End If
                    End If

                    'General para Live y Pro
                    If bEmployeePermit Then
                        strLanguageKey = roTypes.Any2String(row.Item("LanguageKey"))

                        fEmployees.Add(row.Item("EmployeeID"), strName, Nothing, strLanguageKey, strPIN)

                        If CheckAcc Then
                            ' Niveles de acceso
                            Dim aEmployeeAccessLevel As ArrayList
                            If AdvancedAccessMode Then
                                aEmployeeAccessLevel = oBCData.GetEmployeeAccessLevelMxaAdvancedMode_Ex(IDTerminal, oDTAuxAccessPeriod, row.Item("EmployeeID"), aVirtualTimeZones, True)
                            Else
                                aEmployeeAccessLevel = oBCData.GetEmployeeAccessLevelMxaAdvancedMode_Ex(IDTerminal, oDTAuxAccessPeriod, row.Item("GroupID"), aVirtualTimeZones, False)
                            End If
                            If aEmployeeAccessLevel IsNot Nothing Then
                                For Each sEmployeeAccessLevel As String In aEmployeeAccessLevel
                                    fEmployeeAccessLevel.Add(row.Item("EmployeeID"), sEmployeeAccessLevel.Split("@")(0), sEmployeeAccessLevel.Split("@")(1).PadLeft(4, "0").ToCharArray(3, 1) = "1", sEmployeeAccessLevel.Split("@")(1).PadLeft(4, "0").ToCharArray(2, 1) = "1", sEmployeeAccessLevel.Split("@")(1).PadLeft(4, "0").ToCharArray(1, 1) = "1", sEmployeeAccessLevel.Split("@")(1).PadLeft(4, "0").ToCharArray(0, 1) = "1")
                                Next
                            End If
                        End If
                    End If
                End If
            Next

            'Generamos tareas
            fCards.CompareXmlFromDatabase(IDTerminal, Me)
            fEmployees.CompareXmlFromDatabase(IDTerminal, Me, String.Empty)
            fEmployeeAccessLevel.CompareXmlFromDatabase(IDTerminal, Me)

            'Tareas para mxA+
            Dim otask As roTerminalsSyncTasks = New roTerminalsSyncTasks(IDTerminal)
            otask.SetBatchSize(broadcasterTaskBatchSize)

            If slTasksmx.ContainsKey(roTerminalsSyncTasks.SyncActions.delallbios) Then
                For Each strTask As String In slTasksmx(roTerminalsSyncTasks.SyncActions.delallbios)
                    otask.addSyncTask(roTerminalsSyncTasks.SyncActions.delallbios, strTask.Split("|")(0), strTask.Split("|")(1), -10,,,, deleteExistingTasks)
                Next
            End If
            If slTasksmx.ContainsKey(roTerminalsSyncTasks.SyncActions.delallcards) Then
                For Each strTask As String In slTasksmx(roTerminalsSyncTasks.SyncActions.delallcards)
                    otask.addSyncTask(roTerminalsSyncTasks.SyncActions.delallcards, strTask.Split("|")(0), strTask.Split("|")(1), -10,,,, deleteExistingTasks)
                Next
            End If
            If slTasksEx.ContainsKey(roTerminalsSyncTasks.SyncActions.delallemployeeaccesslevel) Then
                For Each strTask As String In slTasksEx(roTerminalsSyncTasks.SyncActions.delallemployeeaccesslevel)
                    otask.addSyncTask(roTerminalsSyncTasks.SyncActions.delallemployeeaccesslevel, strTask.Split("|")(0), strTask.Split("|")(1), -10,,,, deleteExistingTasks)
                Next
            End If
            If slTasksmx.ContainsKey(roTerminalsSyncTasks.SyncActions.delallemployees) Then
                For Each strTask As String In slTasksmx(roTerminalsSyncTasks.SyncActions.delallemployees)
                    otask.addSyncTask(roTerminalsSyncTasks.SyncActions.delallemployees, strTask.Split("|")(0), strTask.Split("|")(1), -10,,,, deleteExistingTasks)
                Next
            End If
            If slTasksmx.ContainsKey(roTerminalsSyncTasks.SyncActions.delbio) Then
                For Each strTask As String In slTasksmx(roTerminalsSyncTasks.SyncActions.delbio)
                    otask.addSyncTask(roTerminalsSyncTasks.SyncActions.delbio, strTask.Split("|")(0), strTask.Split("|")(1), -5,,,, deleteExistingTasks)
                Next
            End If
            If slTasksmx.ContainsKey(roTerminalsSyncTasks.SyncActions.delcard) Then
                For Each strTask As String In slTasksmx(roTerminalsSyncTasks.SyncActions.delcard)
                    otask.addSyncTask(roTerminalsSyncTasks.SyncActions.delcard, strTask.Split("|")(0), strTask.Split("|")(1), -5,,,, deleteExistingTasks)
                Next
            End If
            If slTasksEx.ContainsKey(roTerminalsSyncTasks.SyncActions.delemployeeaccesslevel) Then
                For Each strTask As String In slTasksEx(roTerminalsSyncTasks.SyncActions.delemployeeaccesslevel)
                    otask.addSyncTask(roTerminalsSyncTasks.SyncActions.delemployeeaccesslevel, strTask.Split("|")(0), strTask.Split("|")(1), -5, strTask.Split("|")(2),,, deleteExistingTasks)
                Next
            End If
            If slTasksmx.ContainsKey(roTerminalsSyncTasks.SyncActions.delemployee) Then
                For Each strTask As String In slTasksmx(roTerminalsSyncTasks.SyncActions.delemployee)
                    otask.addSyncTask(roTerminalsSyncTasks.SyncActions.delemployee, strTask.Split("|")(0), strTask.Split("|")(1), -5,,,, deleteExistingTasks)
                Next
            End If

            Threading.Thread.Sleep(1000)

            If slTasksmx.ContainsKey(roTerminalsSyncTasks.SyncActions.addemployee) Then
                For Each strTask As String In slTasksmx(roTerminalsSyncTasks.SyncActions.addemployee)
                    If dEmployeePermit(Integer.Parse(strTask.Split("|")(0))) Then
                        otask.addSyncTask(roTerminalsSyncTasks.SyncActions.addemployee, strTask.Split("|")(0), strTask.Split("|")(1),,,,, deleteExistingTasks)
                    Else
                        roLog.GetInstance().logMessage(roLog.EventType.roDebug, $"BroadcasterNet::UpdateEmployeesMxaPlusToDatabase:{_Terminal.ToString} Task not created because employee not permit.({roTerminalsSyncTasks.SyncActions.addemployee.ToString},{strTask.Split("|")(0)})")
                    End If
                Next
            End If
            If slTasksmx.ContainsKey(roTerminalsSyncTasks.SyncActions.addcard) Then
                For Each strTask As String In slTasksmx(roTerminalsSyncTasks.SyncActions.addcard)
                    If dEmployeePermit(Integer.Parse(strTask.Split("|")(0))) Then
                        otask.addSyncTask(roTerminalsSyncTasks.SyncActions.addcard, strTask.Split("|")(0), strTask.Split("|")(1),,,,, deleteExistingTasks)
                    Else
                        roLog.GetInstance().logMessage(roLog.EventType.roDebug, $"BroadcasterNet::UpdateEmployeesMxaPlusToDatabase:{_Terminal.ToString} Task not created because employee not permit.({roTerminalsSyncTasks.SyncActions.addcard.ToString},{strTask.Split("|")(0)})")
                    End If
                Next
            End If
            If slTasksEx.ContainsKey(roTerminalsSyncTasks.SyncActions.addemployeeaccesslevel) Then
                For Each strTask As String In slTasksEx(roTerminalsSyncTasks.SyncActions.addemployeeaccesslevel)
                    If dEmployeePermit(Integer.Parse(strTask.Split("|")(0))) Then
                        otask.addSyncTask(roTerminalsSyncTasks.SyncActions.addemployeeaccesslevel, strTask.Split("|")(0), strTask.Split("|")(1), , strTask.Split("|")(2), , strTask.Split("|")(4), deleteExistingTasks)
                    Else
                        roLog.GetInstance().logMessage(roLog.EventType.roDebug, $"BroadcasterNet::UpdateEmployeesMxaPlusToDatabase:{_Terminal.ToString} Task not created because employee not permit.({roTerminalsSyncTasks.SyncActions.addbio.ToString},{strTask.Split("|")(0)})")
                    End If
                Next
            End If

            Dim persistSyncTables As Boolean = True
            If otask.WorkMode = roTerminalsSyncTasks.eDbWorkMode.Batch Then persistSyncTables = otask.PersistTasksToDatabase()

            ' Guardamos ficheros si procede
            If persistSyncTables Then
                If fEmployees.HasChange Then fEmployees.SaveToDataBase(IDTerminal, Me)
                If fCards.HasChange Then fCards.SaveToDataBase(IDTerminal)
                If fEmployeeAccessLevel.HasChange Then fEmployeeAccessLevel.SaveToDataBase(IDTerminal)
            Else
                roLog.GetInstance().logMessage(roLog.EventType.roError, $"BroadcasterNet::UpdateEmployeesMxaPlusToDatabase: Error persisting tasks for terminal id {_Terminal}. Will try next time, no later than {DateTime.Now.AddHours(1).ToString("yyyy-MM-dd HH:00:00")}")
                CreateUserTaskGeneric("USERTASK:\\TERMINAL_BROADCAST_ERROR", "TerminalBroadcastError.Title", _Terminal.ID)
            End If

        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, $"BroadcasterNet::UpdateEmployeesMxaPlusToDatabase:{_Terminal.ToString} Unexpected error: ", ex)
            CreateUserTaskGeneric("USERTASK:\\TERMINAL_BROADCAST_ERROR", "TerminalBroadcastError.Title", _Terminal.ID)
        End Try
    End Sub

    Private Sub UpdateEmployeesZKPush2ToDatabase(ByVal IDTerminal As Integer,
                                                 ByVal CheckAcc As Boolean,
                                                 ByRef aVirtualTimeZones As Dictionary(Of Integer, ArrayList))

        Try
            Dim fCards As New CardsFile
            Dim fBioData As New BiodataBinFile
            Dim fEmployees As New EmployeesFile
            Dim fEmployeeTimeZones As New EmployeeTimeZonesPush2File
            Dim oDT As New Data.DataTable
            Dim oDTAuxAccessPeriod As New Data.DataTable
            Dim row As Data.DataRow
            Dim strImageCRC As String = String.Empty
            Dim oImage As Image = Nothing

            oDT = oBCData.GetEmployeesLive(IDTerminal, AdvancedAccessMode, CheckAcc OrElse (RestrictedAttendance AndAlso Not LegacyRestrictedModeUsed), True)

            DelUserTaskGeneric("USERTASK:\\TERMINAL_BROADCAST_ERROR", _Terminal.ID)
            ' Obtenemos información sobre periodos de accesos asociados al lector del terminal
            If CheckAcc Then oDTAuxAccessPeriod = oBCData.GetEmployeeTimeZonesZKPush2_Data(IDTerminal, AdvancedAccessMode)

            Dim strName As String = ""
            Dim strLanguageKey As String
            Dim strPIN As String = ""
            Dim EmployeeOnTerminal As List(Of Integer) = New List(Of Integer)

            Dim bEmployeePermit As Boolean = False

            fCards.ConvertCardID = False

            Dim dEmployeePermit As New Dictionary(Of Integer, Boolean)
            Dim idEmployee As Integer
            Dim bdicEmployeePermit As Boolean
            For Each row In oDT.Rows
                idEmployee = row.Item("EmployeeID")
                bdicEmployeePermit = _Terminal.EmployeePermit(row.Item("EmployeeID"), 1, AdvancedAccessMode)
                If Not dEmployeePermit.ContainsKey(idEmployee) Then dEmployeePermit.Add(idEmployee, bdicEmployeePermit)
            Next

            For Each row In oDT.Rows
                bEmployeePermit = (dEmployeePermit.ContainsKey(row.Item("EmployeeID")) AndAlso dEmployeePermit(row.Item("EmployeeID")))
                If bEmployeePermit Then

                    'Visualtime Live
                    If bEmployeePermit Then
                        If Not EmployeeOnTerminal.Contains(row.Item("EmployeeID")) Then EmployeeOnTerminal.Add(row.Item("EmployeeID"))
                        strName = roTypes.Any2String(row.Item("Name"))
                        strPIN = Any2String(row.Item("PINData"))
                        'Guardamos las tarjetas
                        If Not IsDBNull(row.Item("RealValue")) Then
                            fCards.Add(row.Item("EmployeeID"), row.Item("RealValue"))
                        ElseIf Not IsDBNull(row.Item("IDCard")) Then
                            fCards.Add(row.Item("EmployeeID"), row.Item("IDCard"))
                        End If
                    End If

                    'General para Live y Pro
                    If bEmployeePermit Then
                        strLanguageKey = roTypes.Any2String(row.Item("LanguageKey"))
                        If _Terminal.Type = TerminalData.eTerminalType.rxfp OrElse _Terminal.Type = TerminalData.eTerminalType.rxfl OrElse _Terminal.Type = TerminalData.eTerminalType.rxfptd OrElse _Terminal.Type = TerminalData.eTerminalType.rxfe OrElse _Terminal.Type = TerminalData.eTerminalType.rxte Then
                            Dim rowImage As Data.DataRow = oBCData.GetEmployeeImage(row.Item("EmployeeID"))
                            strImageCRC = String.Empty
                            If Not IsDBNull(rowImage.Item("Image")) Then
                                oImage = Nothing
                                Dim oEmpFile As New EmployeesFile
                                Try
                                    strImageCRC = CryptographyHelper.EncryptWithMD5(Convert.ToBase64String(roTypes.Image2Bytes(oEmpFile.FixedSize(roTypes.Bytes2Image(rowImage.Item("Image")), 200, 200, True))))
                                Catch ex As Exception
                                    roLog.GetInstance().logMessage(roLog.EventType.roError, $"BroadcasterNet::UpdateEmployeesZKPush2ToDatabase:{_Terminal.ToString} Unable to load employee photo for employee ID = {roTypes.Any2String(row.Item("EmployeeID"))}", ex)
                                End Try
                            Else
                                oImage = Nothing
                            End If
                        End If

                        fEmployees.Add(row.Item("EmployeeID"), strName, oImage, strLanguageKey, strPIN,,,,, strImageCRC)

                        If CheckAcc Then
                            ' Niveles de acceso
                            Dim aEmployeeAccessLevel As ArrayList
                            Dim iTZCount As Integer = 0
                            Dim sEmpTZs As String = ""

                            If AdvancedAccessMode Then
                                aEmployeeAccessLevel = oBCData.GetEmployeeTimeZonesZKPush2_Ex(IDTerminal, oDTAuxAccessPeriod, row.Item("EmployeeID"), aVirtualTimeZones, True)
                            Else
                                aEmployeeAccessLevel = oBCData.GetEmployeeTimeZonesZKPush2_Ex(IDTerminal, oDTAuxAccessPeriod, row.Item("GroupID"), aVirtualTimeZones, False)
                            End If

                            ' Completo hasta tres TZ, por si tenía menos asignados
                            While aEmployeeAccessLevel.Count < 3
                                aEmployeeAccessLevel.Add(0)
                            End While

                            For Each iVirtualPeriod As Integer In aEmployeeAccessLevel
                                iTZCount += 1
                                If iTZCount > 3 Then
                                    ' Más de tres zonas para un empleado. Alerta ...
                                    roLog.GetInstance().logMessage(roLog.EventType.roDebug, $"BroadcasterNet::UpdateEmployeesZKPush2ToDatabase: More than 3 timeperiods for employee {row("EmployeeID")} in terminal {_TerminalID}. Period {iVirtualPeriod} ignored")
                                Else
                                    If iVirtualPeriod > 49 Then
                                        roLog.GetInstance().logMessage(roLog.EventType.roDebug, $"BroadcasterNet::UpdateEmployeesZKPush2ToDatabase: Employee {row("EmployeeID")} requires timeperiod that is ver 50 available. Period {iVirtualPeriod} ignored")
                                    Else
                                        If sEmpTZs.Trim <> "" Then
                                            sEmpTZs = sEmpTZs + ";" + iVirtualPeriod.ToString
                                        Else
                                            sEmpTZs = iVirtualPeriod.ToString
                                        End If
                                    End If
                                End If
                            Next
                            fEmployeeTimeZones.Add(row.Item("EmployeeID"), sEmpTZs)
                        Else
                            ' Terminal no es de accesos.
                            ' Igualmente se deben generar los 50 periodos (a permitido), y asignar a los empleados a uno de ellos (p.ej el 1)
                            fEmployeeTimeZones.Add(row.Item("EmployeeID"), "50;0;0")
                        End If
                    End If
                End If
            Next

            'Gestión biometría
            Dim dtBio As Data.DataTable
            Dim rowLive As Data.DataRow
            Dim obiotsk As roTerminalsSyncTasks.SyncActions

            If _Terminal.Type <> TerminalData.eTerminalType.rxte Then
                For Each sBioVersion As String In _BiometricVersion.Split(",")
                    dtBio = oBCData.GetEmployeesAllBio_Live(sBioVersion, AdvancedAccessMode, 0, IIf(CheckAcc OrElse (RestrictedAttendance AndAlso Not LegacyRestrictedModeUsed), _TerminalID, 0))
                    For Each rowLive In dtBio.Rows
                        If dEmployeePermit.ContainsKey(rowLive.Item("EmployeeID")) AndAlso dEmployeePermit(rowLive.Item("EmployeeID")) Then
                            If Not IsDBNull(rowLive.Item("BioData")) Then
                                fBioData.Add(rowLive.Item("EmployeeID"), rowLive.Item("BiometricID"), rowLive.Item("BioData"), sBioVersion)
                            End If
                        End If
                    Next

                    Select Case sBioVersion
                        Case "RXFFNG"
                            obiotsk = roTerminalsSyncTasks.SyncActions.addbio
                        Case "ZKUNIFAC"
                            obiotsk = roTerminalsSyncTasks.SyncActions.addbiodataface
                        Case "ZKUNIPAL"
                            obiotsk = roTerminalsSyncTasks.SyncActions.addbiodatapalm
                    End Select
                    fBioData.CompareXmlFromDatabase(IDTerminal, Me, obiotsk)
                Next
            End If

            'Tarjetas, timezones y finalmente empleados
            fCards.CompareXmlFromDatabase(IDTerminal, Me)
            fEmployeeTimeZones.CompareXmlFromDatabase(IDTerminal, Me)
            fEmployees.CompareXmlFromDatabase(IDTerminal, Me,)

            'Tareas para terminales con protocolo ZK PUSH v2
            Dim otask As roTerminalsSyncTasks = New roTerminalsSyncTasks(IDTerminal)
            otask.SetBatchSize(broadcasterTaskBatchSize)

            ' Si la biometría está deshabilitada a nivel general, y tengo alguna instrucción individual de borrado de biometría, las obvio todas y creo la tarea delall correspondiente
            Dim bioDisabled As Boolean = False
            Dim oParameters As New roParameters("OPTIONS", True)

            ' Algunos terminales de la serie rxF no hacen caso al borrado de biometría hasta que no reinicias el terminal.
            Dim rebootDeviceAfterDelallbiodataConfigured As Boolean = False
            Dim addRebootTask As Boolean = False
            Dim advvancedParamenter As String = String.Empty
            advvancedParamenter = roCacheManager.GetInstance.GetAdvParametersCache(Azure.RoAzureSupport.GetCompanyName, "PushTerminal.RebootRxfAfterDelAllbios").Trim.ToUpper
            rebootDeviceAfterDelallbiodataConfigured = ((advvancedParamenter = "" OrElse advvancedParamenter = "1" OrElse advvancedParamenter = "TRUE") AndAlso (_Terminal.Type = TerminalData.eTerminalType.rxfe _
                                                                                                                                 OrElse _Terminal.Type = TerminalData.eTerminalType.rxfptd _
                                                                                                                                 OrElse _Terminal.Type = TerminalData.eTerminalType.rxfp _
                                                                                                                                 OrElse _Terminal.Type = TerminalData.eTerminalType.rxfl))

            'Si esta activado la biometria globalmente y le pasas por parametro True, creamos biometria
            bioDisabled = roTypes.Any2Boolean(oParameters.Parameter(Parameters.DisableBiometricData))

            If slTasksmx.ContainsKey(roTerminalsSyncTasks.SyncActions.delallbios) Then
                For Each strTask As String In slTasksmx(roTerminalsSyncTasks.SyncActions.delallbios)
                    otask.addSyncTask(roTerminalsSyncTasks.SyncActions.delallbios, strTask.Split("|")(0), strTask.Split("|")(1), -10,,,, deleteExistingTasks)
                    addRebootTask = rebootDeviceAfterDelallbiodataConfigured
                Next
            End If
            If slTasksmx.ContainsKey(roTerminalsSyncTasks.SyncActions.delallbiodataface) Then
                For Each strTask As String In slTasksmx(roTerminalsSyncTasks.SyncActions.delallbiodataface)
                    otask.addSyncTask(roTerminalsSyncTasks.SyncActions.delallbiodataface, strTask.Split("|")(0), strTask.Split("|")(1), -10,,,, deleteExistingTasks)
                    addRebootTask = rebootDeviceAfterDelallbiodataConfigured
                Next
            End If
            If slTasksmx.ContainsKey(roTerminalsSyncTasks.SyncActions.delallbiodatapalm) Then
                For Each strTask As String In slTasksmx(roTerminalsSyncTasks.SyncActions.delallbiodatapalm)
                    otask.addSyncTask(roTerminalsSyncTasks.SyncActions.delallbiodatapalm, strTask.Split("|")(0), strTask.Split("|")(1), -10,,,, deleteExistingTasks)
                    addRebootTask = rebootDeviceAfterDelallbiodataConfigured
                Next
            End If
            If slTasksmx.ContainsKey(roTerminalsSyncTasks.SyncActions.delallcards) Then
                For Each strTask As String In slTasksmx(roTerminalsSyncTasks.SyncActions.delallcards)
                    otask.addSyncTask(roTerminalsSyncTasks.SyncActions.delallcards, strTask.Split("|")(0), strTask.Split("|")(1), -10,,,, deleteExistingTasks)
                Next
            End If
            If slTasksEx.ContainsKey(roTerminalsSyncTasks.SyncActions.delallemployeetimezones) Then
                For Each strTask As String In slTasksEx(roTerminalsSyncTasks.SyncActions.delallemployeetimezones)
                    otask.addSyncTask(roTerminalsSyncTasks.SyncActions.delallemployeetimezones, strTask.Split("|")(0), strTask.Split("|")(1), -10,,,, deleteExistingTasks)
                Next
            End If
            If slTasksmx.ContainsKey(roTerminalsSyncTasks.SyncActions.delallemployees) Then
                For Each strTask As String In slTasksmx(roTerminalsSyncTasks.SyncActions.delallemployees)
                    otask.addSyncTask(roTerminalsSyncTasks.SyncActions.delallemployees, strTask.Split("|")(0), strTask.Split("|")(1), -10,,,, deleteExistingTasks)
                Next
            End If
            If _Terminal.Type = TerminalData.eTerminalType.rxfp OrElse _Terminal.Type = TerminalData.eTerminalType.rxfl OrElse _Terminal.Type = TerminalData.eTerminalType.rxfptd OrElse _Terminal.Type = TerminalData.eTerminalType.rxfe OrElse _Terminal.Type = TerminalData.eTerminalType.rxte Then
                If slTasksmx.ContainsKey(roTerminalsSyncTasks.SyncActions.delphoto) Then
                    For Each strTask As String In slTasksmx(roTerminalsSyncTasks.SyncActions.delphoto)
                        otask.addSyncTask(roTerminalsSyncTasks.SyncActions.delphoto, strTask.Split("|")(0), strTask.Split("|")(1), -5,,,, deleteExistingTasks)
                    Next
                End If
            End If

            If slTasksmx.ContainsKey(roTerminalsSyncTasks.SyncActions.delbio) Then
                If Not bioDisabled Then
                    For Each strTask As String In slTasksmx(roTerminalsSyncTasks.SyncActions.delbio)
                        otask.addSyncTask(roTerminalsSyncTasks.SyncActions.delbio, strTask.Split("|")(0), strTask.Split("|")(1), -5,,,, deleteExistingTasks)
                    Next
                Else
                    otask.addSyncTask(roTerminalsSyncTasks.SyncActions.delallbios, 0, 0, -10,,,, deleteExistingTasks)
                    addRebootTask = rebootDeviceAfterDelallbiodataConfigured
                End If

            End If
            If slTasksmx.ContainsKey(roTerminalsSyncTasks.SyncActions.delbiodataface) Then
                If Not bioDisabled Then
                    For Each strTask As String In slTasksmx(roTerminalsSyncTasks.SyncActions.delbiodataface)
                        otask.addSyncTask(roTerminalsSyncTasks.SyncActions.delbiodataface, strTask.Split("|")(0), strTask.Split("|")(1), -5,,,, deleteExistingTasks)
                    Next
                Else
                    otask.addSyncTask(roTerminalsSyncTasks.SyncActions.delallbiodataface, 0, 0, -10,,,, deleteExistingTasks)
                    addRebootTask = rebootDeviceAfterDelallbiodataConfigured
                End If
            End If
            If slTasksmx.ContainsKey(roTerminalsSyncTasks.SyncActions.delbiodatapalm) Then
                If Not bioDisabled Then
                    For Each strTask As String In slTasksmx(roTerminalsSyncTasks.SyncActions.delbiodatapalm)
                        otask.addSyncTask(roTerminalsSyncTasks.SyncActions.delbiodatapalm, strTask.Split("|")(0), strTask.Split("|")(1), -5,,,, deleteExistingTasks)
                    Next
                Else
                    otask.addSyncTask(roTerminalsSyncTasks.SyncActions.delallbiodatapalm, 0, 0, -10,,,, deleteExistingTasks)
                    addRebootTask = rebootDeviceAfterDelallbiodataConfigured
                End If
            End If
            If slTasksmx.ContainsKey(roTerminalsSyncTasks.SyncActions.delcard) Then
                For Each strTask As String In slTasksmx(roTerminalsSyncTasks.SyncActions.delcard)
                    otask.addSyncTask(roTerminalsSyncTasks.SyncActions.delcard, strTask.Split("|")(0), strTask.Split("|")(1), -5,,,, deleteExistingTasks)
                Next
            End If
            If slTasksEx.ContainsKey(roTerminalsSyncTasks.SyncActions.delemployeetimezones) Then
                For Each strTask As String In slTasksEx(roTerminalsSyncTasks.SyncActions.delemployeetimezones)
                    otask.addSyncTask(roTerminalsSyncTasks.SyncActions.delemployeetimezones, strTask.Split("|")(0), strTask.Split("|")(1), -5, strTask.Split("|")(2),,, deleteExistingTasks)
                Next
            End If
            If slTasksmx.ContainsKey(roTerminalsSyncTasks.SyncActions.delemployee) Then
                For Each strTask As String In slTasksmx(roTerminalsSyncTasks.SyncActions.delemployee)
                    otask.addSyncTask(roTerminalsSyncTasks.SyncActions.delemployee, strTask.Split("|")(0), strTask.Split("|")(1), -5,,,, deleteExistingTasks)
                Next
            End If

            Threading.Thread.Sleep(1000)

            If slTasksmx.ContainsKey(roTerminalsSyncTasks.SyncActions.addemployee) Then
                For Each strTask As String In slTasksmx(roTerminalsSyncTasks.SyncActions.addemployee)
                    If dEmployeePermit.ContainsKey(Integer.Parse(strTask.Split("|")(0))) AndAlso dEmployeePermit(Integer.Parse(strTask.Split("|")(0))) Then
                        otask.addSyncTask(roTerminalsSyncTasks.SyncActions.addemployee, strTask.Split("|")(0), strTask.Split("|")(1),,,,, deleteExistingTasks)
                    Else
                        roLog.GetInstance().logMessage(roLog.EventType.roDebug, $"BroadcasterNet::UpdateEmployeesZKPush2ToDatabase:{_Terminal.ToString} Task not created because employee not permit.({roTerminalsSyncTasks.SyncActions.addemployee},{strTask.Split("|")(0)})")
                    End If
                Next
            End If
            If slTasksmx.ContainsKey(roTerminalsSyncTasks.SyncActions.addcard) Then
                For Each strTask As String In slTasksmx(roTerminalsSyncTasks.SyncActions.addcard)
                    ' En terminales ZK, la tarjeta se envía con la info del empleado. Si se envía el empleado, no hace falta enviar la tarjeta
                    If Not (slTasksmx.ContainsKey(roTerminalsSyncTasks.SyncActions.addemployee) AndAlso slTasksmx(roTerminalsSyncTasks.SyncActions.addemployee).Contains(strTask)) Then
                        If dEmployeePermit.ContainsKey(Integer.Parse(strTask.Split("|")(0))) AndAlso dEmployeePermit(Integer.Parse(strTask.Split("|")(0))) Then
                            otask.addSyncTask(roTerminalsSyncTasks.SyncActions.addcard, strTask.Split("|")(0), strTask.Split("|")(1),,,,, deleteExistingTasks)
                        Else
                            roLog.GetInstance().logMessage(roLog.EventType.roDebug, $"BroadcasterNet::UpdateEmployeesZKPush2ToDatabase:{_Terminal.ToString} Task not created because employee not permit.({roTerminalsSyncTasks.SyncActions.addcard},{strTask.Split("|")(0)})")
                        End If
                    End If
                Next
            End If
            If slTasksmx.ContainsKey(roTerminalsSyncTasks.SyncActions.addbio) Then
                For Each strTask As String In slTasksmx(roTerminalsSyncTasks.SyncActions.addbio)
                    If dEmployeePermit.ContainsKey(Integer.Parse(strTask.Split("|")(0))) AndAlso dEmployeePermit(Integer.Parse(strTask.Split("|")(0))) Then
                        otask.addSyncTask(roTerminalsSyncTasks.SyncActions.addbio, strTask.Split("|")(0), strTask.Split("|")(1),,,,, deleteExistingTasks)
                    Else
                        roLog.GetInstance().logMessage(roLog.EventType.roDebug, $"BroadcasterNet::UpdateEmployeesZKPush2ToDatabase:{_Terminal.ToString} Task not created because employee not permit.({roTerminalsSyncTasks.SyncActions.addbio},{strTask.Split("|")(0)})")
                    End If
                Next
            End If
            If _Terminal.Type = TerminalData.eTerminalType.rxfp OrElse _Terminal.Type = TerminalData.eTerminalType.rxfl OrElse _Terminal.Type = TerminalData.eTerminalType.rxfptd OrElse _Terminal.Type = TerminalData.eTerminalType.rxfe OrElse _Terminal.Type = TerminalData.eTerminalType.rxte Then
                If slTasksmx.ContainsKey(roTerminalsSyncTasks.SyncActions.addphoto) Then
                    For Each strTask As String In slTasksmx(roTerminalsSyncTasks.SyncActions.addphoto)
                        If dEmployeePermit.ContainsKey(Integer.Parse(strTask.Split("|")(0))) AndAlso dEmployeePermit(Integer.Parse(strTask.Split("|")(0))) Then
                            otask.addSyncTask(roTerminalsSyncTasks.SyncActions.addphoto, strTask.Split("|")(0), strTask.Split("|")(1),,,,, deleteExistingTasks)
                        Else
                            roLog.GetInstance().logMessage(roLog.EventType.roDebug, $"BroadcasterNet::UpdateEmployeesZKPushToDatabase:{_Terminal.ToString} Task not created because employee not permit.({roTerminalsSyncTasks.SyncActions.addphoto},{strTask.Split("|")(0)})")
                        End If
                    Next
                End If
            End If
            If slTasksmx.ContainsKey(roTerminalsSyncTasks.SyncActions.addbiodataface) Then
                For Each strTask As String In slTasksmx(roTerminalsSyncTasks.SyncActions.addbiodataface)
                    otask.addSyncTask(roTerminalsSyncTasks.SyncActions.addbiodataface, strTask.Split("|")(0), strTask.Split("|")(1),,,,, deleteExistingTasks)
                Next
            End If
            If slTasksmx.ContainsKey(roTerminalsSyncTasks.SyncActions.addbiodatapalm) Then
                For Each strTask As String In slTasksmx(roTerminalsSyncTasks.SyncActions.addbiodatapalm)
                    otask.addSyncTask(roTerminalsSyncTasks.SyncActions.addbiodatapalm, strTask.Split("|")(0), strTask.Split("|")(1),,,,, deleteExistingTasks)
                Next
            End If
            If slTasksEx.ContainsKey(roTerminalsSyncTasks.SyncActions.addemployeetimezones) Then
                For Each strTask As String In slTasksEx(roTerminalsSyncTasks.SyncActions.addemployeetimezones)
                    If dEmployeePermit.ContainsKey(Integer.Parse(strTask.Split("|")(0))) AndAlso dEmployeePermit(Integer.Parse(strTask.Split("|")(0))) Then
                        otask.addSyncTask(roTerminalsSyncTasks.SyncActions.addemployeetimezones, strTask.Split("|")(0), strTask.Split("|")(1), , strTask.Split("|")(2), , strTask.Split("|")(4), deleteExistingTasks)
                    Else
                        roLog.GetInstance().logMessage(roLog.EventType.roDebug, $"BroadcasterNet::UpdateEmployeesZKPush2ToDatabase:{_Terminal.ToString} Task not created because employee not permit.({roTerminalsSyncTasks.SyncActions.addbio},{strTask.Split("|")(0)})")
                    End If
                Next
            End If

            If addRebootTask Then
                otask.addSyncTask(roTerminalsSyncTasks.SyncActions.reboot, 0, 0, 60,,,, deleteExistingTasks)
            End If

            Dim persistSyncTables As Boolean = True
            If otask.WorkMode = roTerminalsSyncTasks.eDbWorkMode.Batch Then persistSyncTables = otask.PersistTasksToDatabase()

            ' Guardamos ficheros si procede
            If persistSyncTables Then
                If fEmployees.HasChange Then fEmployees.SaveToDataBase(IDTerminal, Me)
                If fCards.HasChange Then fCards.SaveToDataBase(IDTerminal)
                If fBioData.HasChange Then fBioData.SaveToDataBase(IDTerminal)
                If fEmployeeTimeZones.HasChange Then fEmployeeTimeZones.SaveToDataBase(IDTerminal)
            Else
                roLog.GetInstance().logMessage(roLog.EventType.roError, $"BroadcasterNet::UpdateEmployeesZKPush2ToDatabase: Error persisting tasks for terminal id {_Terminal}. Will try next time, no later than {DateTime.Now.AddHours(1).ToString("yyyy-MM-dd HH:00:00")}")
                CreateUserTaskGeneric("USERTASK:\\TERMINAL_BROADCAST_ERROR", "TerminalBroadcastError.Title", _Terminal.ID)
            End If

        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, $"BroadcasterNet::UpdateEmployeesZKPush2ToDatabase:{_Terminal.ToString} Unexpected error: ", ex)
            CreateUserTaskGeneric("USERTASK:\\TERMINAL_BROADCAST_ERROR", "TerminalBroadcastError.Title", _Terminal.ID)
        End Try
    End Sub

    Private Sub UpdateSirensToDatabase(ByVal TerminalID As Byte, Optional bCreateSyncTask As Boolean = False)

        Try

            Dim bolEnableSirens As Boolean = True

            ' Verificamos si hay algún acceso configurado con el mismo relé que la sirenas. Si es así, no generamos sirenas.
            Dim dtRelays As DataTable = oBCData.GetRelays(TerminalID)
            For Each oRelayRow As DataRow In dtRelays.Rows
                If BCGlobal.SirensOutput = Any2Integer(oRelayRow.Item("Output")) Then
                    If Any2String(oRelayRow.Item("Mode")) = "ACC" OrElse Any2String(oRelayRow.Item("Mode")) = "TAACC" Then
                        bolEnableSirens = False
                        roLog.GetInstance().logMessage(roLog.EventType.roDebug, $"BroadcasterNet::UpdateSirens:{_Terminal.ToString} Sirens relay used in access configuration. Sirens cancelled.")
                        Exit For
                    End If
                End If
            Next

            Dim fSirens As New SirensFile

            If bolEnableSirens Then
                Dim oDS As New Data.DataSet
                Dim row As Data.DataRow
                Dim iCount As Integer
                oDS = oBCData.GetSirens(TerminalID)
                For iCount = 0 To oDS.Tables.Count - 1
                    For Each row In oDS.Tables(iCount).Rows
                        If iCount = 0 Then
                            'Guardamos la sirena
                            fSirens.Add(row.Item("DayOf"), row.Item("StartDate"), row.Item("Duration"))
                        Else
                            'Guardamos las inactividades
                            fSirens.Add(row.Item("DayOf"), row.Item("StartDate"), row.Item("FinishDate"), row.Item("OutP"))
                        End If
                    Next
                Next
            End If

            ' Persistimos la información
            If fSirens.HasChanged(_Terminal.ID) Then
                fSirens.SaveToDataBase(_Terminal.ID)
                ' Para terminales PUSH creo tarea
                If bCreateSyncTask Then
                    AddTaskEx(Robotics.Comms.Base.roTerminalsSyncTasks.SyncActions.setsirens, 0, , 0, 0, fSirens.ToXml)
                End If
            End If
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, $"BroadcasterNet::UpdateSirens:{_Terminal.ToString} Unexpected error: ", ex)
        End Try
    End Sub

    Private Sub UpdateTimeZonesToDatabase(ByVal TerminalID As Byte, Optional ByVal void As Boolean = False)

        Try
            Dim fTimeZone As New TimeZonesFile
            Dim oDS As New Data.DataSet
            Dim row As Data.DataRow

            'Si no queremo un fichero en blanco lo rellenamos
            If Not void Then
                oDS = oBCData.GetTimeZones(TerminalID)
                ' Periodos normales
                For Each row In oDS.Tables(0).Rows
                    fTimeZone.Add(row.Item("GroupID"), row.Item("ReaderID"), row.Item("DayOfWeek"), row.Item("BeginTime"), row.Item("EndTime"))
                Next
                ' Periodos festivos
                For Each row In oDS.Tables(1).Rows
                    If row("Day") = Now.Day AndAlso row("Month") = Now.Month Then
                        Dim intDayOf As Integer = IIf(Now.DayOfWeek = DayOfWeek.Sunday, 7, Now.DayOfWeek)
                        If Format(CDate(row("BeginTime")), "HH:mm") = "00:00" AndAlso Format(CDate(row("EndTime")), "HH:mm") = "00:00" Then
                            ' No se concede acceso para ese dia
                            ' Eliminamos los periodos del día de la semana actual
                            fTimeZone.Remove(row.Item("GroupID"), row.Item("ReaderID"), intDayOf)
                        Else
                            ' Conceder acceso
                            fTimeZone.Add(row("GroupID"), row("ReaderID"), intDayOf, row.Item("BeginTime"), row.Item("EndTime"))
                        End If
                    End If
                Next
                ' Eventos
                For Each row In oDS.Tables(2).Rows
                    Dim intDayOf As Integer
                    If CDate(row("EndTime")) < CDate(row("BeginTime")) Then
                        ' Eventos que empiezan y acaban el mismo día
                        If CDate(row("Date")) = Now.Date Then
                            ' Estamos en el día de inicio del evento
                            ' Programa para hoy el primer tramo (desde inicio de periodo hasta media noche)
                            intDayOf = IIf(Now.DayOfWeek = DayOfWeek.Sunday, 7, Now.DayOfWeek)
                            fTimeZone.Add(row("GroupID"), row("ReaderID"), intDayOf, row.Item("BeginTime"), "23:59:59")
                            ' Programo para mañana el segundo tramo (desde medianoche hasta el fin de periodo)
                            intDayOf = IIf(Now.AddDays(1).DayOfWeek = DayOfWeek.Sunday, 7, Now.AddDays(1).DayOfWeek)
                            fTimeZone.Add(row("GroupID"), row("ReaderID"), intDayOf, "00:00", row.Item("EndTime"))
                        ElseIf CDate(row("Date")) = Now.Date.AddDays(-1) Then
                            ' Estamos en el día siguiente al inicio del evento
                            ' Programo sólo el tramo desde medianoche hasta fin de periodo
                            intDayOf = IIf(Now.DayOfWeek = DayOfWeek.Sunday, 7, Now.DayOfWeek)
                            fTimeZone.Add(row("GroupID"), row("ReaderID"), intDayOf, "00:00", row.Item("EndTime"))
                        End If
                    Else
                        ' Eventos que empiezan un día y acaban el siguiente
                        If CDate(row("Date")) = Now.Date Then
                            intDayOf = IIf(Now.DayOfWeek = DayOfWeek.Sunday, 7, Now.DayOfWeek)
                            fTimeZone.Add(row("GroupID"), row("ReaderID"), intDayOf, row.Item("BeginTime"), row.Item("EndTime"))
                        End If
                    End If
                Next
            End If

            'Guardamos info
            If fTimeZone.HasChanged(_Terminal.ID) Then
                fTimeZone.SaveToDataBase(_Terminal.ID, (_Terminal.Type = TerminalData.eTerminalType.mx8 OrElse _Terminal.Type = TerminalData.eTerminalType.mx9))
                If _Terminal.Type = TerminalData.eTerminalType.mx9 Then
                    ' Los terminales Android de ROBOTICS sincronizan los periodos de acceso mediante tarea
                    fTimeZone.CompareXmlFromDatabase(_Terminal.ID, Me)
                    Dim otask As roTerminalsSyncTasks = New roTerminalsSyncTasks(_Terminal.ID)
                    If slTasksEx.ContainsKey(roTerminalsSyncTasks.SyncActions.delalltimezones) Then
                        For Each strTask As String In slTasksEx(roTerminalsSyncTasks.SyncActions.delalltimezones)
                            otask.addSyncTask(roTerminalsSyncTasks.SyncActions.delalltimezones, strTask.Split("|")(0), strTask.Split("|")(1), , strTask.Split("|")(2), strTask.Split("|")(3), strTask.Split("|")(4), deleteExistingTasks)
                        Next
                    End If

                    If slTasksEx.ContainsKey(roTerminalsSyncTasks.SyncActions.addtimezone) Then
                        For Each strTask As String In slTasksEx(roTerminalsSyncTasks.SyncActions.addtimezone)
                            otask.addSyncTask(roTerminalsSyncTasks.SyncActions.addtimezone, strTask.Split("|")(0), strTask.Split("|")(1), , strTask.Split("|")(2), strTask.Split("|")(3), strTask.Split("|")(4), deleteExistingTasks)
                        Next
                    End If
                ElseIf _Terminal.Type = TerminalData.eTerminalType.mx8 Then
                    ' Los mx8 programan todos los TimeZones a la vez
                    Dim otask As roTerminalsSyncTasks = New roTerminalsSyncTasks(_Terminal.ID)
                    otask.addSyncTask(roTerminalsSyncTasks.SyncActions.settimezones, 0, 0, 0, 0, 0, fTimeZone.ToXml, deleteExistingTasks)
                End If
            End If
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, $"BroadcasterNet::UpdateTimeZones:{_Terminal.ToString} Unexpected error: ", ex)
        End Try
    End Sub

    Private Sub UpdateTimeZonesMxaPlusToDatabase(ByVal TerminalID As Byte, ByRef aVirtualTimeZones As ArrayList)

        Try
            Dim fTimeZones As New TimeZoneMxapFile
            Dim oDS As New Data.DataSet
            Dim row As Data.DataRow
            Dim sHolidays As String = ","

            'Si no queremo un fichero en blanco lo rellenamos
            oDS = oBCData.GetTimeZonesMxa(TerminalID)
            ' Periodos normales
            For Each row In oDS.Tables(0).Rows
                fTimeZones.Add(row.Item("PeriodID"), row.Item("DayOfWeek"), row.Item("BeginTime"), row.Item("EndTime"))
            Next
            ' Periodos festivos
            For Each row In oDS.Tables(1).Rows
                If row("Day") = Now.Day AndAlso row("Month") = Now.Month Then
                    Dim intDayOf As Integer = IIf(Now.DayOfWeek = DayOfWeek.Sunday, 7, Now.DayOfWeek)
                    If Format(CDate(row("BeginTime")), "HH:mm") = "00:00" AndAlso Format(CDate(row("EndTime")), "HH:mm") = "00:00" Then
                        ' No se concede acceso para ese dia
                        ' Eliminamos los periodos del día de la semana actual
                        fTimeZones.Remove(row.Item("IDAccessPeriod"), intDayOf)
                    Else
                        ' Elimino posible periodos normales ...  si no lo hice ya
                        If sHolidays.IndexOf("," + intDayOf.ToString + ",") < 0 Then fTimeZones.Remove(row.Item("IDAccessPeriod"), intDayOf)
                        sHolidays += intDayOf.ToString + ","
                        ' Conceder acceso en el periodo indicado
                        fTimeZones.Add(row.Item("IDAccessPeriod"), intDayOf, row.Item("BeginTime"), row.Item("EndTime"))
                    End If
                End If
            Next
            '---------------------------------------------NO BORRAR-------------------------------------------------------------------------
            ' Eventos: FASE II
            '    For Each row In oDS.Tables(2).Rows
            '        Dim intDayOf As Integer
            '        If CDate(row("EndTime")) < CDate(row("BeginTime")) Then
            '            ' Eventos que empiezan y acaban el mismo día
            '            If CDate(row("Date")) = Now.Date Then
            '                ' Estamos en el día de inicio del evento
            '                ' Programa para hoy el primer tramo (desde inicio de periodo hasta media noche)
            '                intDayOf = IIf(Now.DayOfWeek = DayOfWeek.Sunday, 7, Now.DayOfWeek)
            '                fTimeZone.Add(row("GroupID"), row("ReaderID"), intDayOf, row.Item("BeginTime"), "23:59:59")
            '                ' Programo para mañana el segundo tramo (desde medianoche hasta el fin de periodo)
            '                intDayOf = IIf(Now.AddDays(1).DayOfWeek = DayOfWeek.Sunday, 7, Now.AddDays(1).DayOfWeek)
            '                fTimeZone.Add(row("GroupID"), row("ReaderID"), intDayOf, "00:00", row.Item("EndTime"))
            '            ElseIf CDate(row("Date")) = Now.Date.AddDays(-1) Then
            '                ' Estamos en el día siguiente al inicio del evento
            '                ' Programo sólo el tramo desde medianoche hasta fin de periodo
            '                intDayOf = IIf(Now.DayOfWeek = DayOfWeek.Sunday, 7, Now.DayOfWeek)
            '                fTimeZone.Add(row("GroupID"), row("ReaderID"), intDayOf, "00:00", row.Item("EndTime"))
            '            End If
            '        Else
            '            ' Eventos que empiezan un día y acaban el siguiente
            '            If CDate(row("Date")) = Now.Date Then
            '                intDayOf = IIf(Now.DayOfWeek = DayOfWeek.Sunday, 7, Now.DayOfWeek)
            '                fTimeZone.Add(row("GroupID"), row("ReaderID"), intDayOf, row.Item("BeginTime"), row.Item("EndTime"))
            '            End If
            '        End If
            '    Next
            '---------------------------------------------NO BORRAR-------------------------------------------------------------------------

            '---------------------------------------------NO BORRAR-------------------------------------------------------------------------
            ' VIRTUALIZACIÓN DE ZONAS: Se generaron timezones virtuales, de manera que un empleado sólo tenía un timezone para una puerta dada.
            ' DE MOMENTO DESACTIVADO
            ''For Each oVTZ As String In aVirtualTimeZones
            ''    Dim sWhere As String = ""
            ''    Select Case oVTZ.Length
            ''        Case 2
            ''            ' No se debe dar. Sería el caso de un único timezone, y por tanto no es virtual
            ''        Case 4
            ''            sWhere = CInt(oVTZ.Substring(0, 2)).ToString + "," + CInt(oVTZ.Substring(2, 2)).ToString
            ''        Case 6
            ''            sWhere = CInt(oVTZ.Substring(0, 2)).ToString + "," + CInt(oVTZ.Substring(2, 2)).ToString + "," + CInt(oVTZ.Substring(4, 2)).ToString
            ''    End Select
            ''    oDS = oBCData.GetTimeZonesMxa(TerminalID, sWhere)  '-> Ejemplo, si compactamos los timezones 1 y 2, strWhere = "1,2"
            ''    ' Periodos normales
            ''    For Each row In oDS.Tables(0).Rows
            ''        fTimeZones.Add(CInt(oVTZ).ToString, row.Item("DayOfWeek"), row.Item("BeginTime"), row.Item("EndTime"))  '-> Ejemplo, si compactamos los timezones 1 y 2, oVTZ = 1,2*100 = 201"
            ''    Next
            ''    ' Periodos festivos
            ''    sHolidays = ""
            ''    For Each row In oDS.Tables(1).Rows
            ''        If row("Day") = Now.Day And row("Month") = Now.Month Then
            ''            Dim intDayOf As Integer = IIf(Now.DayOfWeek = DayOfWeek.Sunday, 7, Now.DayOfWeek)
            ''            If Format(CDate(row("BeginTime")), "HH:mm") = "00:00" And Format(CDate(row("EndTime")), "HH:mm") = "00:00" Then
            ''                ' No se concede acceso para ese dia
            ''                ' Eliminamos los periodos del día de la semana actual
            ''                fTimeZones.Remove(CInt(oVTZ).ToString, intDayOf)
            ''            Else
            ''                ' Elimino posible periodos normales ...  si no lo hice ya
            ''                If sHolidays.IndexOf("," + intDayOf.ToString + ",") < 0 Then fTimeZones.Remove(CInt(oVTZ).ToString, intDayOf)
            ''                sHolidays += intDayOf.ToString + ","
            ''                ' Conceder acceso en el periodo indicado
            ''                fTimeZones.Add(CInt(oVTZ).ToString, intDayOf, row.Item("BeginTime"), row.Item("EndTime"))
            ''            End If
            ''        End If
            ''    Next
            ''Next
            '---------------------------------------------NO BORRAR-------------------------------------------------------------------------

            'Generamos tareas si hay cambios
            fTimeZones.CompareXmlFromDatabase(TerminalID, Me)

            Dim otask As roTerminalsSyncTasks = New roTerminalsSyncTasks(TerminalID)

            If slTasksEx.ContainsKey(roTerminalsSyncTasks.SyncActions.delalltimezones) Then
                For Each strTask As String In slTasksEx(roTerminalsSyncTasks.SyncActions.delalltimezones)
                    otask.addSyncTask(roTerminalsSyncTasks.SyncActions.delalltimezones, strTask.Split("|")(0), strTask.Split("|")(1), -10, strTask.Split("|")(2), strTask.Split("|")(3),, deleteExistingTasks)
                Next
            End If
            If slTasksEx.ContainsKey(roTerminalsSyncTasks.SyncActions.addtimezone) Then
                For Each strTask As String In slTasksEx(roTerminalsSyncTasks.SyncActions.addtimezone)
                    otask.addSyncTask(roTerminalsSyncTasks.SyncActions.addtimezone, strTask.Split("|")(0), strTask.Split("|")(1), , strTask.Split("|")(2), strTask.Split("|")(3), strTask.Split("|")(4), deleteExistingTasks)
                Next
            End If
            If slTasksEx.ContainsKey(roTerminalsSyncTasks.SyncActions.deltimezone) Then
                For Each strTask As String In slTasksEx(roTerminalsSyncTasks.SyncActions.deltimezone)
                    otask.addSyncTask(roTerminalsSyncTasks.SyncActions.deltimezone, strTask.Split("|")(0), strTask.Split("|")(1), , strTask.Split("|")(2), strTask.Split("|")(3), strTask.Split("|")(4), deleteExistingTasks)
                Next
            End If

            If fTimeZones.HasChange Then fTimeZones.SaveToDataBase(TerminalID)
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, $"BroadcasterNet::UpdateTimeZones:{_Terminal.ToString} Unexpected error: ", ex)
        End Try
    End Sub

    Private Sub UpdateTimeZonesZKPush2ToDatabase(ByVal TerminalID As Byte, ByRef aTimeZoneRelations As Dictionary(Of Integer, ArrayList), Optional ByVal bIsAccess As Boolean = False)

        Try
            Dim fTimeZones As New TimeZoneMxapFile
            Dim oDS As New Data.DataSet
            Dim row As Data.DataRow
            Dim sHolidays As String = ","

            'Si no queremo un fichero en blanco lo rellenamos
            If bIsAccess Then
                oDS = oBCData.GetTimeZonesMxa(TerminalID)
                ' Periodos normales
                For Each row In oDS.Tables(0).Rows
                    fTimeZones.Add(row.Item("PeriodID"), row.Item("DayOfWeek"), row.Item("BeginTime"), row.Item("EndTime"))
                Next

                ' Periodos festivos
                For Each row In oDS.Tables(1).Rows
                    If row("Day") = Now.Day AndAlso row("Month") = Now.Month Then
                        Dim intDayOf As Integer = IIf(Now.DayOfWeek = DayOfWeek.Sunday, 7, Now.DayOfWeek)
                        If Format(CDate(row("BeginTime")), "HH:mm") = "00:00" AndAlso Format(CDate(row("EndTime")), "HH:mm") = "00:00" Then
                            ' No se concede acceso para ese dia
                            ' Eliminamos los periodos del día de la semana actual
                            fTimeZones.Remove(row.Item("IDAccessPeriod"), intDayOf)
                        Else
                            ' Elimino posible periodos normales ...  si no lo hice ya
                            If sHolidays.IndexOf("," + intDayOf.ToString + ",") < 0 Then fTimeZones.Remove(row.Item("IDAccessPeriod"), intDayOf)
                            sHolidays += intDayOf.ToString + ","
                            ' Conceder acceso en el periodo indicado
                            fTimeZones.Add(row.Item("IDAccessPeriod"), intDayOf, row.Item("BeginTime"), row.Item("EndTime"))
                        End If
                    End If
                Next
                '---------------------------------------------NO BORRAR-------------------------------------------------------------------------
                ' Eventos: FASE II
                '    For Each row In oDS.Tables(2).Rows
                '        Dim intDayOf As Integer
                '        If CDate(row("EndTime")) < CDate(row("BeginTime")) Then
                '            ' Eventos que empiezan y acaban el mismo día
                '            If CDate(row("Date")) = Now.Date Then
                '                ' Estamos en el día de inicio del evento
                '                ' Programa para hoy el primer tramo (desde inicio de periodo hasta media noche)
                '                intDayOf = IIf(Now.DayOfWeek = DayOfWeek.Sunday, 7, Now.DayOfWeek)
                '                fTimeZone.Add(row("GroupID"), row("ReaderID"), intDayOf, row.Item("BeginTime"), "23:59:59")
                '                ' Programo para mañana el segundo tramo (desde medianoche hasta el fin de periodo)
                '                intDayOf = IIf(Now.AddDays(1).DayOfWeek = DayOfWeek.Sunday, 7, Now.AddDays(1).DayOfWeek)
                '                fTimeZone.Add(row("GroupID"), row("ReaderID"), intDayOf, "00:00", row.Item("EndTime"))
                '            ElseIf CDate(row("Date")) = Now.Date.AddDays(-1) Then
                '                ' Estamos en el día siguiente al inicio del evento
                '                ' Programo sólo el tramo desde medianoche hasta fin de periodo
                '                intDayOf = IIf(Now.DayOfWeek = DayOfWeek.Sunday, 7, Now.DayOfWeek)
                '                fTimeZone.Add(row("GroupID"), row("ReaderID"), intDayOf, "00:00", row.Item("EndTime"))
                '            End If
                '        Else
                '            ' Eventos que empiezan un día y acaban el siguiente
                '            If CDate(row("Date")) = Now.Date Then
                '                intDayOf = IIf(Now.DayOfWeek = DayOfWeek.Sunday, 7, Now.DayOfWeek)
                '                fTimeZone.Add(row("GroupID"), row("ReaderID"), intDayOf, row.Item("BeginTime"), row.Item("EndTime"))
                '            End If
                '        End If
                '    Next
                '---------------------------------------------NO BORRAR-------------------------------------------------------------------------
            End If

            ' Ahora debo virtualizar los periodos registrados en fTimeZones. En esta lista, cada periodo puede tener hasta tres franjas por día.
            ' En terminales PUSH v2, los periodos tienen una única franja por día. Luego por cada periodo de fTimeZones puedo tener que crear hasta 3 periodos distintos.
            ' Además, los periodos que genere, deben tener id's correlativos desde el 1, hasta el 49. El 50 reservado para presencia.

            Dim fTimeZonesZKPush As New TimeZoneZKPush2File
            Dim iVirtualTimeZoneID As Integer = 0
            For Each oTZ As TimeZoneMxap In fTimeZones.TimeZones.Values
                ' Miro cuántos periodos necesitaré
                Dim iDepth As Byte = 0
                Dim aVirtualTimeZones As New ArrayList

                For i As Integer = 1 To 10
                    If oTZ.SlotsInDay(i) > iDepth Then iDepth = oTZ.SlotsInDay(i)
                Next
                iVirtualTimeZoneID = iVirtualTimeZoneID + 1

                ' Añado primer periodo (Siempre existirá), si no se superó el máximo de 49
                If iVirtualTimeZoneID < 49 Then
                    For i As Integer = 1 To 10
                        fTimeZonesZKPush.Add(iVirtualTimeZoneID, i, oTZ.BeginTime1(i), oTZ.EndTime1(i))
                    Next
                Else
                    roLog.GetInstance().logMessage(roLog.EventType.roDebug, $"BroadcasterNet::UpdateTimeZonesZKPush2:More than 49 timeperiods required. Some of them will be ignored!")
                End If
                aVirtualTimeZones.Add(iVirtualTimeZoneID)
                If Not aTimeZoneRelations.ContainsKey(oTZ.ID) Then
                    aTimeZoneRelations.Add(oTZ.ID, aVirtualTimeZones)
                Else
                    aTimeZoneRelations.Item(oTZ.ID) = aVirtualTimeZones
                End If

                If iDepth > 1 Then
                    iVirtualTimeZoneID = iVirtualTimeZoneID + 1
                    ' Añado el segundo periodo, si no se superó el máximo de 49
                    If iVirtualTimeZoneID < 49 Then
                        For i As Integer = 1 To 10
                            fTimeZonesZKPush.Add(iVirtualTimeZoneID, i, oTZ.BeginTime2(i), oTZ.EndTime2(i))
                        Next
                    Else
                        roLog.GetInstance().logMessage(roLog.EventType.roDebug, $"BroadcasterNet::UpdateTimeZonesZKPush2:More than 49 timeperiods required. Some of them will be ignored!")
                    End If
                    aVirtualTimeZones.Add(iVirtualTimeZoneID)
                    If Not aTimeZoneRelations.ContainsKey(oTZ.ID) Then
                        aTimeZoneRelations.Add(oTZ.ID, aVirtualTimeZones)
                    Else
                        aTimeZoneRelations.Item(oTZ.ID) = aVirtualTimeZones
                    End If
                End If

                If iDepth > 2 Then
                    iVirtualTimeZoneID = iVirtualTimeZoneID + 1
                    ' Añado el tercer periodo
                    If iVirtualTimeZoneID < 49 Then
                        For i As Integer = 1 To 10
                            fTimeZonesZKPush.Add(iVirtualTimeZoneID, i, oTZ.BeginTime3(i), oTZ.EndTime3(i))
                        Next
                    Else
                        roLog.GetInstance().logMessage(roLog.EventType.roDebug, $"BroadcasterNet::UpdateTimeZonesZKPush2:More than 49 timeperiods required. Some of them will be ignored!")
                    End If
                    aVirtualTimeZones.Add(iVirtualTimeZoneID)
                    If Not aTimeZoneRelations.ContainsKey(oTZ.ID) Then
                        aTimeZoneRelations.Add(oTZ.ID, aVirtualTimeZones)
                    Else
                        aTimeZoneRelations.Item(oTZ.ID) = aVirtualTimeZones
                    End If
                End If
            Next

            ' Completo los periodos hasta los 50 del terminal
            If iVirtualTimeZoneID < 49 Then
                For j As Integer = iVirtualTimeZoneID + 1 To 49
                    For i As Integer = 1 To 10
                        fTimeZonesZKPush.Add(j, i, Date.Parse("00:01"), Date.Parse("00:00"))
                    Next
                Next
            End If

            ' Añado el periodo 50 como acceso siempre, para terminal de presencia
            For i As Integer = 1 To 10
                fTimeZonesZKPush.Add(50, i, Date.Parse("00:00"), Date.Parse("23:59"))
            Next

            'Generamos tareas si hay cambios
            fTimeZonesZKPush.CompareXmlFromDatabase(TerminalID, Me)

            'Guardamos el fichero
            If fTimeZonesZKPush.HasChange Then fTimeZonesZKPush.SaveToDataBase(TerminalID)

            Dim otask As roTerminalsSyncTasks = New roTerminalsSyncTasks(TerminalID)

            ' Nunca generamos tareas de delalltimezones. El protocolo no lo soporta, y siempre se redefinen las 50 timezones disponibles.
            If slTasksEx.ContainsKey(roTerminalsSyncTasks.SyncActions.addtimezone) Then
                For Each strTask As String In slTasksEx(roTerminalsSyncTasks.SyncActions.addtimezone)
                    otask.addSyncTask(roTerminalsSyncTasks.SyncActions.addtimezone, strTask.Split("|")(0), strTask.Split("|")(1), , strTask.Split("|")(2), strTask.Split("|")(3), strTask.Split("|")(4), deleteExistingTasks)
                Next
            End If
            If slTasksEx.ContainsKey(roTerminalsSyncTasks.SyncActions.deltimezone) Then
                For Each strTask As String In slTasksEx(roTerminalsSyncTasks.SyncActions.deltimezone)
                    otask.addSyncTask(roTerminalsSyncTasks.SyncActions.deltimezone, strTask.Split("|")(0), strTask.Split("|")(1), , strTask.Split("|")(2), strTask.Split("|")(3), strTask.Split("|")(4), deleteExistingTasks)
                Next
            End If
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, $"BroadcasterNet::UpdateTimeZonesZKPush2:{_Terminal.ToString} Unexpected error: ", ex)
        End Try
    End Sub

    Private Sub UpdateAccessToDatabase(ByVal TerminalID As Byte)

        Try
            Dim fAccess As New AccessFile
            Dim oDS As New Data.DataSet
            Dim row As Data.DataRow
            Dim iCount As Integer

            oDS = oBCData.GetAccess(TerminalID)

            For iCount = 0 To oDS.Tables.Count - 1
                For Each row In oDS.Tables(iCount).Rows
                    'Guardamos la sirena
                    fAccess.Add(row.Item("IDAccessGroup"), row.Item("ReaderID"))
                Next
            Next
            If fAccess.HasChanged(_Terminal.ID) Then
                fAccess.SaveToDataBase(_Terminal.ID)
                ' Para mx8 creo tarea para que se sincronice en el terminal
                If _Terminal.Type = TerminalData.eTerminalType.mx8 Then
                    Dim otask As roTerminalsSyncTasks = New roTerminalsSyncTasks(_Terminal.ID)
                    otask.addSyncTask(roTerminalsSyncTasks.SyncActions.setaccess, 0, 0, 0, 0, 0, fAccess.ToXml, deleteExistingTasks)
                End If
            End If
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, $"BroadcasterNet::UpdateAccess:{_Terminal.ToString} Unexpected error: ", ex)
        End Try
    End Sub

    Private Sub UpdateCausesToDatabase(Optional avoidReaderInputCondeZero As Boolean = False)

        Try
            Dim fCauses As New CausesFile
            Dim oDT As New Data.DataTable
            Dim row As Data.DataRow

            oDT = oBCData.GetCauses(avoidReaderInputCondeZero)

            'CRN: Revisamos si el terminal es un Rx1 y está configurado con un centro de coste para no pasar los WorkingType
            If Not ((_Terminal.Type = TerminalData.eTerminalType.rx1 _
                      OrElse _Terminal.Type = TerminalData.eTerminalType.rxfl _
                      OrElse _Terminal.Type = TerminalData.eTerminalType.rxfe _
                      OrElse _Terminal.Type = TerminalData.eTerminalType.rxfptd _
                      OrElse _Terminal.Type = TerminalData.eTerminalType.rxte _
                      OrElse _Terminal.Type = TerminalData.eTerminalType.rxfp) AndAlso _Terminal.RDRBehavior(1) = "CO") Then
                For Each row In oDT.Rows
                    fCauses.Add(row.Item("ID"), row.Item("Name"), row.Item("ReaderInputCode"), row.Item("WorkingType"))
                Next
            End If

            ' Persistimos tabla de sincronización
            If fCauses.HasChanged(_Terminal.ID) AndAlso (_Terminal.Type = TerminalData.eTerminalType.mx8 _
                                                          OrElse _Terminal.Type = TerminalData.eTerminalType.mx9 _
                                                          OrElse _Terminal.Type = TerminalData.eTerminalType.rx1 _
                                                          OrElse _Terminal.Type = TerminalData.eTerminalType.rxfl _
                                                          OrElse _Terminal.Type = TerminalData.eTerminalType.rxfe _
                                                          OrElse _Terminal.Type = TerminalData.eTerminalType.rxte _
                                                          OrElse _Terminal.Type = TerminalData.eTerminalType.rxfptd _
                                                          OrElse _Terminal.Type = TerminalData.eTerminalType.rxfp) Then
                fCauses.SaveToDataBase(_Terminal.ID)
                AddTaskEx(Robotics.Comms.Base.roTerminalsSyncTasks.SyncActions.setcauses, 0, 0, 0, 0, fCauses.ToXml)
                Dim otask As roTerminalsSyncTasks = New roTerminalsSyncTasks(_Terminal.ID)
                If slTasksEx.ContainsKey(roTerminalsSyncTasks.SyncActions.setcauses) Then
                    For Each strTask As String In slTasksEx(roTerminalsSyncTasks.SyncActions.setcauses)
                        otask.addSyncTask(roTerminalsSyncTasks.SyncActions.setcauses, strTask.Split("|")(0), strTask.Split("|")(1), , strTask.Split("|")(2), strTask.Split("|")(3), strTask.Split("|")(4), deleteExistingTasks)
                    Next
                End If
            End If
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, $"BroadcasterNet::UpdateCauses:{_Terminal.ToString} Unexpected error: ", ex)
        End Try
    End Sub

    Private Sub UpdateTerminalDocumentsMx7pToDatabase(ByRef fDocument As DocumentFile, ByVal TerminalID As Byte, ByVal EmployeeOnTerminal As List(Of Integer))
        Try
            ' Subo al terminal todos los documentos que a día de hoy son incorrectos y deniegan el acceso.
            ' Si un documento entregado caduca, se subirá cuando caduque y por tanto genere una alerta.
            Dim oLicense As New roServerLicense
            Dim strQuery As String = String.Empty

            If (oLicense.FeatureIsInstalled("Feature\Documents") OrElse oLicense.FeatureIsInstalled("Feature\OHP")) AndAlso _Terminal.RDRHasAccess(1) Then
                Dim tbRet As New DataTable
                strQuery = "@SELECT# sepd.IDEmployee, tr.ID as IDReader, dt.Name, gr.Name CompanyName  from sysrovwEmployeePRLDocumentaionFaults sepd " &
                        "Left join TerminalReaders tr on tr.IDZone = sepd.idzone " &
                        "inner join DocumentTemplates dt on dt.Id = sepd.templateid " &
                        "left join Groups gr on gr.ID = sepd.idcompany " &
                        "where tr.idterminal = " & TerminalID.ToString & " " &
                        "and sepd.accessvalidation = " & DTOs.DocumentAccessValidation.AccessDenied

                tbRet = CreateDataTable(strQuery)
                If tbRet IsNot Nothing AndAlso tbRet.Rows.Count > 0 Then
                    For Each row As DataRow In tbRet.Rows
                        If EmployeeOnTerminal.Contains(Any2Integer(row.Item("IDEmployee"))) Then
                            fDocument.Add(Any2Integer(row.Item("IDEmployee")), Any2Integer(row.Item("IDReader")), Any2String(row.Item("Name")), Any2String(row.Item("CompanyName")), DateSerial(2000, 1, 1), DateSerial(2000, 1, 1), True)
                        End If
                    Next
                End If
            End If
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, $"BroadcasterNet::UpdateTerminalDocumentsMx7p:{_Terminal.ToString} Unexpected error: ", ex)
        End Try
    End Sub

    Public Sub AddTask(ByVal Task As roTerminalsSyncTasks.SyncActions, ByVal IDEmployee As Integer, Optional ByVal IDFinger As Integer = 0)
        Try

            Dim strKey As String = IDEmployee.ToString + "|" + IDFinger.ToString
            If Not slTasksmx.ContainsKey(Task) Then
                slTasksmx.Add(Task, New List(Of String))
            End If
            If Not slTasksmx(Task).Contains(strKey) Then
                slTasksmx(Task).Add(strKey)
            End If
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, $"BroadcasterNet::AddTask:{_Terminal.ToString} Unexpected error: ", ex)
        End Try
    End Sub

    Public Sub AddTaskEx(ByVal Task As roTerminalsSyncTasks.SyncActions, ByVal IDEmployee As Integer, Optional ByVal IDFinger As Integer = 0, Optional ByVal Parameter1 As Integer = 0, Optional ByVal Parameter2 As Integer = 0, Optional ByVal TaskData As String = "")
        Try

            Dim strKey As String = IDEmployee.ToString + "|" + IDFinger.ToString + "|" + Parameter1.ToString + "|" + Parameter2.ToString + "|" + TaskData.ToString
            If Not slTasksEx.ContainsKey(Task) Then
                slTasksEx.Add(Task, New List(Of String))
            End If
            If Not slTasksEx(Task).Contains(strKey) Then
                slTasksEx(Task).Add(strKey)
            End If
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, $"BroadcasterNet::AddTaskEx:{_Terminal.ToString} Unexpected error: ", ex)
        End Try
    End Sub

    Private Sub CreateLegacyAttRestrictionUserTask()
        Try
            Dim oState As New VTBusiness.UserTask.roUserTaskState()
            Dim oTaskExist As New VTBusiness.UserTask.roUserTask("USERTASK:\\LEGACYATTRESTRICTION", oState)
            Dim Language As New Robotics.VTBase.roLanguage()
            Language.SetLanguageReference("ProcessBroadcasterNET", oHelper.OSettings.GetVTSetting(eKeys.DefaultLanguage))
            If oTaskExist.Message = "" Then
                Dim oTask As New VTBusiness.UserTask.roUserTask()
                With oTask
                    .ID = "USERTASK:\\LEGACYATTRESTRICTION"
                    Language.ClearUserTokens()
                    .Message = Language.Translate("LegacyAttendandeRestriction.Title", "")
                    Language.ClearUserTokens()
                    .DateCreated = Now
                    .TaskType = VTBusiness.UserTask.TaskType.UserTaskRepair
                    .ResolverURL = "FN:\\LegacyAttendandeRestriction"
                    .Save()
                    roLog.GetInstance().logMessage(roLog.EventType.roDebug, "BroadcasterNET::CreateUserTask:Legacy attendande restriction detected:User task created.")
                End With
            End If
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "BroadcasterNET::CreateUserTask:Unknown error creating legacy attendande restriction:Error:", ex)
        End Try
    End Sub

    Private Sub DelLegacyAttRestrictionUserTask()
        Try
            Dim oState As New VTBusiness.UserTask.roUserTaskState()
            Dim oTaskExist As New VTBusiness.UserTask.roUserTask("USERTASK:\\LEGACYATTRESTRICTION", oState)
            'Si existe la tarea la borramos
            If oTaskExist.Message <> "" Then
                oTaskExist.Delete()
                roLog.GetInstance().logMessage(roLog.EventType.roDebug, "BroadcasterNET::DelUserTask::User task deleted because the no legacy attendance restriction applied")
            End If
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "BroadcasterNET::DelUserTask:Unknown error deleting legacy attendance restrictio alert :Error:", ex)
        End Try
    End Sub

    Private Sub CreateUserTaskGeneric(sTaskID As String, sLangTag As String, iIDTerminal As Integer)
        Dim oState As New VTBusiness.UserTask.roUserTaskState()
        Dim oTaskExist As New VTBusiness.UserTask.roUserTask(sTaskID & iIDTerminal.ToString, oState)
        If oTaskExist.Message = "" Then
            Dim oTask As New VTBusiness.UserTask.roUserTask()
            With oTask
                .ID = sTaskID & iIDTerminal.ToString
                .Message = sLangTag + "¬" + iIDTerminal.ToString
                .DateCreated = Now
                .TaskType = VTBusiness.UserTask.TaskType.UserTaskRepair
                .ResolverURL = ""
                .Save()
                roLog.GetInstance().logMessage(roLog.EventType.roDebug, $"BroadcasterNet::CreateUserTaskGeneric:{_Terminal.ToString} User task created.")
            End With
        End If
    End Sub

    Private Sub DelUserTaskGeneric(sTaskID As String, iIDTerminal As Integer)
        Try
            Dim oState As New VTBusiness.UserTask.roUserTaskState()
            Dim oTaskExist As New VTBusiness.UserTask.roUserTask(sTaskID & iIDTerminal.ToString, oState)
            'Si existe la tarea la borramos
            If oTaskExist.Message <> "" Then
                oTaskExist.Delete()
                roLog.GetInstance().logMessage(roLog.EventType.roDebug, $"BroadcasterNet::DelUserTaskGeneric:{_Terminal.ToString} User task deleted because the terminal registered yet")
            End If
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, $"BroadcasterNet::DelUserTask:Terminal {iIDTerminal} :Error:", ex)
        End Try
    End Sub

End Class