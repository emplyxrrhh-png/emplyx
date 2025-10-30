Imports System.Threading
Imports System.IO
Imports System.Data.Common
Imports Robotics.VTBase
Imports Robotics.VTBase.roTypes
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.Base.VTBusiness
Imports Robotics.Base.VTBusiness.Task
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Group
Imports Robotics.VTBase.Extensions.VTLiveTasks

Public Class roSenerValidateTasks
    Inherits roSener

#Region "Constantes privadas"

    Private Const _VALIDATIONS_PATH = "Validations"
    Private Const _VALIDATIONS_LOG_PATH = "ValidationsLogs"
    Private Const _VALIDATIONS_ERROR_FILE = "Validations_RESULT_"

#End Region

#Region "Enums"

    Private Enum ErrorType
        eGlobal
        eImportar
        eCrearTareas
        eLeerOrdenes
        eLeerOperaciones
        eExportar
        eActivarTarea
        eModificarFichaje
        eLeerFichajes
        eValidarTareas
        eExistsValidacionS

    End Enum

    Private Enum TaskListType
        produccion
        proyecto
    End Enum


#End Region

#Region "Declarations - Constructor"

    Private ReadOnly _oLog As New roLog("VTDataLinkServer")

    Private _strValidationFileS = String.Empty
    Private _strValidationFileE = String.Empty
    Private _strDefaultBc = String.Empty
    Private _strSeparator = String.Empty
    Private _strOpenTaskValue = String.Empty
    Private _strEmployeeCode = String.Empty
    Private _strErrorsToFile As List(Of String) = New List(Of String)

    Public Sub New()
        roLog.GetInstance.logMessage(roLog.EventType.roDebug, "VTSener::Start::roSenerValidateTasks(" +
        FileVersionInfo.GetVersionInfo(Reflection.Assembly.GetExecutingAssembly.Location).FileVersion +
        ") inicialized.")

        'Recuperar los registros necesarios
        Dim keysFounded As Boolean = GetDataLinkServerKeys
        If Not keysFounded Then
            Throw New Exception("Necessary keys not founded")
        End If

        'Vaciamos el fichero de logs general, por si ya existen datos antiguos
        ResetErrorLogs()
    End Sub

#End Region

#Region "Validación de tareas"
    Public Function ValidateTasks() As Boolean
        Dim oTaskState As New roTaskState
        Dim strLog As String = String.Empty
        Dim strSourceValidationSFileName = _VALIDATIONS_PATH + "/" + _strValidationFileS
        Dim strSourceValidationEFileName = _VALIDATIONS_PATH + "/" + _strValidationFileE
        Dim strSourceBackupValidationEFileName = _VALIDATIONS_PATH + "/bck/" + _strValidationFileE

        Try
            'OBTENEMOS LOS FICHEROS DE VALIDACIÓN
            Dim oFileValidationS As Byte() = Azure.RoAzureSupport.DownloadFileFromCompanyContainer(strSourceValidationSFileName, roLiveDatalinkFolders.custom.ToString, roLiveQueueTypes.dataLink)
            Dim oFileValidationE As Byte() = Azure.RoAzureSupport.DownloadFileFromCompanyContainer(strSourceValidationEFileName, roLiveDatalinkFolders.custom.ToString, roLiveQueueTypes.dataLink)

            If oFileValidationS Is Nothing Then
                If oFileValidationE IsNot Nothing AndAlso oFileValidationE.Length > 0 Then
                    'Hacemos backup del fichero
                    Azure.RoAzureSupport.RenameFileInCompanyContainer(strSourceValidationEFileName, strSourceBackupValidationEFileName & "." & Now.ToString("yyyyMMddHHmmss") & ".bck", roLiveDatalinkFolders.custom.ToString, roLiveQueueTypes.dataLink)

                    Dim bolValidationRet = ValidationTasks(oFileValidationS, oFileValidationE, oTaskState, strLog)

                    If Not bolValidationRet Then
                        strLog = strLog & "," & oTaskState.ErrorText
                        _oLog.logMessage(roLog.EventType.roInfo, "CDataLinkServer::ValidateTasks :" & strLog)
                    End If
                End If
            End If

        Catch ex As Exception
            _oLog.logMessage(roLog.EventType.roError, "CDataLinkServer::ValidateTasks :" & ex.Message)
            SaveErrorToStringList(ErrorType.eGlobal.ToString(), String.Format("{0}{1}", "Ha surgido una incidencia en el proceso de importación: ", ex.Message))
        Finally
            'Crear / enviar doc errores al servidor
            UploadErrorFile(_VALIDATIONS_ERROR_FILE, _VALIDATIONS_LOG_PATH)
        End Try

        Return True


    End Function

    Private Function ValidationTasks(ByVal oFileValidationS As Byte(), ByVal oFileValidationE As Byte(), oTaskState As roTaskState, ByRef strLog As String) As Boolean
        Dim lstValidation = New SortedDictionary(Of roOrder, List(Of roOperation))
        lstValidation = GetValidationETasks(oFileValidationE)
        Return ProcessValidationTasks(lstValidation, oTaskState, strLog)
    End Function

    Private Function GetValidationETasks(ByVal oFileValidationE As Byte()) As SortedDictionary(Of roOrder, List(Of roOperation))
        Dim lstValidation = New SortedDictionary(Of roOrder, List(Of roOperation))
        Dim strOrderLine As String = String.Empty
        Dim index As Int16 = 1
        Try
            Dim memStream As New MemoryStream(oFileValidationE)
            Dim oReader As New StreamReader(memStream)
            While Not oReader.EndOfStream
                Dim lstOperations = New List(Of roOperation)
                strOrderLine = oReader.ReadLine()
                Dim values As String() = strOrderLine.Split(_strSeparator)
                If (values.Count().Equals(3)) Then
                    Dim orderValidate = New roOrder()  'TODO: Revisar si va así o con params: New roOrder(strOrderLine, _strSeparator)
                    With orderValidate
                        .ProyectCode = values(0)
                    End With
                    Dim opeValidate = New roOperation()
                    With opeValidate
                        .TaskCode = values(1)
                        .TaskState = If(values(2).Trim().Equals(_strOpenTaskValue), TaskStatusEnum._ON, TaskStatusEnum._COMPLETED)
                    End With
                    If (lstValidation.ContainsKey(orderValidate)) Then
                        lstOperations = lstValidation.Item(orderValidate)
                        lstOperations.Add(opeValidate)
                    Else
                        lstOperations.Add(opeValidate)
                        lstValidation.Add(orderValidate, lstOperations)
                    End If
                End If
                index += 1
            End While
            oReader.Close()
            memStream.Close()
        Catch ex As Exception
            If Not String.IsNullOrEmpty(strOrderLine) Then
                SaveErrorToStringList(ErrorType.eGlobal.ToString(), String.Format("{0}{1}{2}{3}", "No se ha podido leer correctamente la orden de la línea ", index, ": ", strOrderLine))
            End If
            SaveErrorToStringList(ErrorType.eGlobal.ToString(), String.Format("Ha surgido una incidencia en el proceso obtención de validaciones de entrada: {0}", ex.Message))
            roLog.GetInstance.logMessage(roLog.EventType.roError, "roSennerValidateTasks::GetOrders::Unhandled exception::" & ex.Message)
            lstValidation = Nothing
        End Try

        Return lstValidation

    End Function

    Private Function ProcessValidationTasks(ByVal tasks As SortedDictionary(Of roOrder, List(Of roOperation)), oTaskState As roTaskState, ByRef strLog As String) As Boolean
        Try
            strLog = "OK"
            Dim lstTaskToExport = New List(Of ExportTasks)
            Dim oState As New Common.roDataLinkState
            'Recuperamos todos los empleados que estén fichando en tareas
            Dim lstEmployeeTasks = GetLastPunchesFromEmployees()
            Dim fileToExport As List(Of String) = Nothing
            Dim intIdCenter = roTypes.Any2Integer(ExecuteScalar("@SELECT# ID  from BusinessCenters WHERE Name='" + _strDefaultBc + "' "))
            For Each pairTask As KeyValuePair(Of roOrder, List(Of roOperation)) In tasks
                Dim project = pairTask.Key
                Dim tasksProject = pairTask.Value
                Dim idTasksFromOperations = New List(Of Integer)
                For Each roOperation As roOperation In tasksProject
                    Try
                        Dim strSql = "@SELECT# count(ID) FROM Tasks WHERE NAME LIKE '%" + project.ProyectCode + "/" + roOperation.TaskCode + "%'"
                        Dim intIdTask = 0

                        If roTypes.Any2Integer(ExecuteScalar(strSql)) > 1 Then
                            strSql = "@SELECT# ID FROM Tasks WHERE BARCODE LIKE '" + project.ProyectCode + roOperation.TaskCode + "'"
                            intIdTask = roTypes.Any2Integer(ExecuteScalar(strSql))
                        Else
                            strSql = "@SELECT# ID FROM Tasks WHERE NAME LIKE '%" + project.ProyectCode + "/" + roOperation.TaskCode + "%'"
                            intIdTask = roTypes.Any2Integer(ExecuteScalar(strSql))
                        End If

                        If (intIdTask.Equals(0)) Then
                            Try
                                CreateOrUpdateTask(0, intIdCenter, New roTaskState(), roOperation, project, idTasksFromOperations)
                            Catch ex As Exception
                                _oLog.logMessage(roLog.EventType.roError, "CDataLinkServer::ProcessValidationTasks :" & ex.Message)
                                If roOperation IsNot Nothing Then
                                    SaveErrorToStringList(ErrorType.eValidarTareas.ToString(), String.Format("No se ha podido crear la nueva tarea con id {0}: {1}", roOperation.TaskCode, ex.Message))
                                Else
                                    SaveErrorToStringList(ErrorType.eValidarTareas.ToString(), String.Format("No se ha podido crear una nueva tarea: {0}", ex.Message))
                                End If
                            End Try
                            Continue For
                        Else
                            idTasksFromOperations.Add(intIdTask)
                        End If
                        _oLog.logMessage(roLog.EventType.roInfo, "CDataLinkServer::ProcessValidationTasks : Se validará la tarea: " & project.ProyectCode + "/" + roOperation.TaskCode & "con ID: " & intIdTask.ToString)
                        Dim oTask As New roTask(intIdTask, oTaskState, True)
                        If (oTask IsNot Nothing) Then
                            _oLog.logMessage(roLog.EventType.roInfo, "CDataLinkServer::ProcessValidationTasks : Con datos: Proyecto: " & oTask.Project & "- Tarea: " & oTask.Name)
                        End If
                        Select Case roOperation.TaskState
                            Case TaskStatusEnum._ON
                                oTask.Status = TaskStatusEnum._ON
                                If (Not oTask.Save(True)) Then
                                    strLog = oTaskState.ErrorText
                                End If
                                Continue For
                            Case TaskStatusEnum._COMPLETED
                                'Si no existen empleados fichando en tareas, cerramos todas las que lleguen
                                If (lstEmployeeTasks Is Nothing) Then
                                    oTask.Status = TaskStatusEnum._COMPLETED
                                    oTask.Save(True)
                                Else
                                    'Validamos si hay empleados trabajando sobre esta tarea.
                                    Dim employeeInTask = lstEmployeeTasks.Where(Function(t) t.TaskId.Equals(intIdTask)).ToList()
                                    'Si existen empleados sobre la tarea, no la cierro e informo
                                    If (employeeInTask IsNot Nothing AndAlso employeeInTask.Count > 0) Then
                                        For Each employeeLastTask As ExportTasks In employeeInTask
                                            lstTaskToExport.Add(employeeLastTask)
                                            SaveErrorToStringList(ErrorType.eValidarTareas.ToString(), String.Format("No se ha podido cerrar la tarea con id {0} porqué el empleado {1} está trabajando en ella.", roOperation.TaskCode, employeeLastTask.EmployeeCode))
                                        Next
                                        'Si no hay empleados en esa tarea la cierro
                                    Else
                                        Try
                                            oTask.Status = TaskStatusEnum._COMPLETED
                                            oTask.Save(True)
                                        Catch ex As Exception
                                            _oLog.logMessage(roLog.EventType.roError, "CDataLinkServer::ProcessValidationTasks :" & ex.Message)
                                            SaveErrorToStringList(ErrorType.eValidarTareas.ToString(), String.Format("No se ha podido cerrar la tarea con id {0}: {1}", roOperation.TaskCode, ex.Message))
                                            Return False
                                        End Try
                                    End If
                                End If
                        End Select
                    Catch ex As Exception
                        _oLog.logMessage(roLog.EventType.roError, "CDataLinkServer::ProcessValidationTasks :" & ex.Message)
                        SaveErrorToStringList(ErrorType.eValidarTareas.ToString(), String.Format("Ha surgido una incidencia en la validación de tareas: {0}", ex.Message))
                        strLog = strLog & ex.ToString()
                        Return False
                    End Try
                Next

                'Obtenemos todas aquellas tareas que existen en VTLive y tienen algún empleado trabajando (punch) y NO existen en NAV
                Dim activeTasksWithPunches = GetProjectActiveTasksWithOrWithoutPunches(project.ProyectCode, True)
                For Each taskID As Integer In activeTasksWithPunches
                    Dim isTaskFound As Boolean = False
                    For Each intIdTask As Integer In idTasksFromOperations
                        If taskID = intIdTask Then
                            isTaskFound = True
                            Exit For
                        End If
                    Next
                    If Not isTaskFound Then
                        Dim lstTaskPunches = GetLastPunchesFromTask(taskID)
                        For Each employeeLastTask As ExportTasks In lstTaskPunches
                            lstTaskToExport.Add(employeeLastTask)
                            SaveErrorToStringList(ErrorType.eValidarTareas.ToString(), String.Format("No se ha podido cerrar la tarea con id {0} porqué el empleado {1} está trabajando en ella.", employeeLastTask.TaskCode, employeeLastTask.EmployeeCode))
                        Next
                    End If
                Next
                'Obtenemos todas aquellas tareas que existen en VTLive y NO tienen ningún empleado trabajando (punch) y NO existen en NAV
                Dim activeTasksWithoutPunches = GetProjectActiveTasksWithOrWithoutPunches(project.ProyectCode, False)

                Dim tmpError As String = String.Empty
                For Each taskID As Integer In activeTasksWithoutPunches
                    Dim isTaskFound As Boolean = False
                    For Each intIdTask As Integer In idTasksFromOperations
                        If taskID = intIdTask Then
                            isTaskFound = True
                            Exit For
                        End If
                    Next
                    If Not isTaskFound Then
                        tmpError = String.Empty
                        roTask.CompleteTask(taskID, tmpError, oTaskState)
                    End If
                Next
            Next

            'Crear el fichero y exportarlo al blob y borramos fichero original
            If (lstTaskToExport IsNot Nothing And lstTaskToExport.Count > 0) Then
                fileToExport = CreateExportTaskValidation(lstTaskToExport)
                'Dim nameFile As String = String.Format("{0}{1}{2}", _VALIDATION_S_TMP_FILE, Date.Now.ToString("yyyyMMddHHmmss"), ".txt")
                If (fileToExport IsNot Nothing And fileToExport.Count > 0) Then
                    Dim file As Stream = GenerateStreamFromString(fileToExport)
                    Azure.RoAzureSupport.SaveFileOnCompanyContainer(file, _strValidationFileS, roLiveDatalinkFolders.custom.ToString + "/" + _VALIDATIONS_PATH, roLiveQueueTypes.dataLink, True)
                Else
                    _oLog.logMessage(roLog.EventType.roInfo, "CDataLinkServer::ProcessValidationTasks::1 : No hay tareas para exportar")
                    SaveErrorToStringList(ErrorType.eValidarTareas.ToString(), String.Format("No hay tareas para exportar (1)"))
                    'Crear ValidationS.txt vacío a petición del cliente 28/01/2023
                    Dim file As Stream = GenerateStreamFromString(New List(Of String))
                    Azure.RoAzureSupport.SaveFileOnCompanyContainer(file, _strValidationFileS, roLiveDatalinkFolders.custom.ToString + "/" + _VALIDATIONS_PATH, roLiveQueueTypes.dataLink, True)
                End If
            Else
                _oLog.logMessage(roLog.EventType.roInfo, "CDataLinkServer::ProcessValidationTasks::0 : No hay tareas para exportar")
                SaveErrorToStringList(ErrorType.eValidarTareas.ToString(), String.Format("No hay tareas para exportar (0)"))
                'Crear ValidationS.txt vacío a petición del cliente 28/01/2023
                Dim file As Stream = GenerateStreamFromString(New List(Of String))
                Azure.RoAzureSupport.SaveFileOnCompanyContainer(file, _strValidationFileS, roLiveDatalinkFolders.custom.ToString + "/" + _VALIDATIONS_PATH, roLiveQueueTypes.dataLink, True)
            End If

            Return True
        Catch ex As Exception
            _oLog.logMessage(roLog.EventType.roError, "CDataLinkServer::ProcessValidationTasks :" & ex.Message)
            strLog = strLog & ex.ToString()
            Return False
        End Try

    End Function

    Private Function GetProjectActiveTasksWithOrWithoutPunches(idProject As String, withPunches As Boolean) As List(Of Integer)

        Dim sqlQuery As Object
        Dim tasks = New List(Of Integer)
        Try
            sqlQuery = "@SELECT# Id " &
                       "from tasks " &
                       "WHERE Status = 0 " &
                       "AND Project like '%{0}%' " &
                       "AND Id " + IIf(withPunches, "", "NOT") + " IN (@SELECT# typedata " &
                                       "from punches p " &
                                       "inner join (@SELECT# idemployee, max(datetime) as m " &
                                                "from punches where actualtype=4 group by idemployee) l " &
                                                "on p.IDEmployee=l.IDEmployee and p.ActualType=4 and p.DateTime =l.m)"

            sqlQuery = String.Format(sqlQuery, idProject)

            Dim dt = CreateDataTable(sqlQuery, "NonExistingTasks")
            If (dt.Rows.Count > 0) Then
                For Each rowTask As DataRow In dt.Rows
                    tasks.Add(roTypes.Any2Integer(rowTask("Id")))
                Next
            End If
        Catch ex As Exception
            _oLog.logMessage(roLog.EventType.roError, "VTSener::roSenerValidateTasks::GetProjectActiveTasksWithOrWithoutPunches :" & ex.Message)
            SaveErrorToStringList(ErrorType.eLeerFichajes.ToString(), String.Format("No se han podido recuperar todas las tareas del proyecto {0}: {1}", idProject, ex.Message))
        End Try
        Return tasks
    End Function

    Private Function GetLastPunchesFromEmployees() As List(Of ExportTasks)
        Dim sqlQuery As Object
        Dim lstLastPunches = New List(Of ExportTasks)

        Try
            sqlQuery = "@SELECT# P.IDEmployee, TypeData, t.Project, t.Name as TaskName, p.DateTime, EUF.Value as Type, " &
                        "p.ShiftDate, p.Field1, t.id, p.ActualType, p.ID as IdPunch, p.Exported, " &
                        "ceg.FullGroupName " &
                        "from punches p " &
                        "inner join (@SELECT# idemployee, max(datetime) as m " &
                        "from punches where actualtype=4 group by idemployee) l " &
                        "on p.IDEmployee=l.IDEmployee and p.ActualType=4 and p.DateTime =l.m " &
                        "Inner join EmployeeUserFieldValues EUF ON P.IDEmployee = EUF.IDEmployee " &
                        "Left join Tasks T on T.ID=P.typedata " &
                        "Inner join sysrovwCurrentEmployeeGroups ceg on ceg.IDEmployee = P.IDEmployee " &
                        "where EUF.FieldName = '" + _strEmployeeCode + "' AND datetime >= DATEADD(m, -6, GetDate()) " & 'César Chamorro especifica que solo es necesario obtener aquellos fichajes de los últimos 6 meses
                        "order by TypeData"
            Dim dt = CreateDataTable(sqlQuery, "LastTask")
            If (dt.Rows.Count > 0) Then
                For Each punch As DataRow In dt.Rows
                    Dim drPunch = CreateExportTask(punch)
                    lstLastPunches.Add(drPunch)
                Next
            End If
        Catch ex As Exception
            _oLog.logMessage(roLog.EventType.roError, "VTSener::roSenerValidateTasks::GetLastPunchesFromEmployees :" & ex.Message)
            SaveErrorToStringList(ErrorType.eLeerFichajes.ToString(), String.Format("No se han podido recuperar los fichajes de los empleados: {0}", ex.Message))
        End Try

        Return lstLastPunches
    End Function

    Private Shared Function CreateExportTask(row As DataRow) As ExportTasks
        Dim exportTask = New ExportTasks
        With exportTask
            .EmployeeCode = Any2String(row("Type"))
            .EmployeeId = Any2String(row("IDEmployee"))
            .ShiftDate = Any2DateTime(row("ShiftDate"))
            .PunchDate = Any2DateTime(row("DateTime"))
            .TaskCode = Any2String(row("TaskName"))
            .ProjectCode = Any2String(row("Project"))
            .PunchTypeNav = Any2String(row("Field1"))
            .TaskId = Any2Integer(row("Id"))
            .PunchActualType = Any2Integer(row("ActualType"))
            .IdPunch = Any2Integer(row("IdPunch"))
            .IsExported = Any2Boolean(row("Exported"))
            .FullGroupName = Any2String(row("FullGroupName"))
        End With
        If Not String.IsNullOrEmpty(exportTask.FullGroupName) Then
            Dim token As String() = exportTask.FullGroupName.Split("\")
            If token.Length > 0 Then
                exportTask.FullGroupName = token(0).Trim()
            End If
        End If
        Return exportTask
    End Function

    Private Function GetLastPunchesFromTask(idTask As Integer) As List(Of ExportTasks)
        Dim sqlQuery As Object
        Dim lstLastPunches = New List(Of ExportTasks)

        Try
            sqlQuery = "@SELECT# P.IDEmployee, TypeData, t.Project, t.Name as TaskName, p.DateTime, EUF.Value as Type, " &
                        "p.ShiftDate, p.Field1, t.id, p.ActualType, p.ID as IdPunch, p.Exported, " &
                        "ceg.FullGroupName " &
                        "from punches p " &
                        "inner join (@SELECT# idemployee, max(datetime) as m " &
                        "from punches where actualtype=4 group by idemployee) l " &
                        "on p.IDEmployee=l.IDEmployee and p.ActualType=4 and p.DateTime =l.m " &
                        "Inner join EmployeeUserFieldValues EUF ON P.IDEmployee = EUF.IDEmployee " &
                        "Left join Tasks T on T.ID=P.typedata " &
                        "Inner join sysrovwCurrentEmployeeGroups ceg on ceg.IDEmployee = P.IDEmployee " &
                        "where EUF.FieldName = '" + _strEmployeeCode + "' AND datetime >= DATEADD(m, -6, GetDate()) and t.Id = " + idTask.ToString() + " " & 'César Chamorro especifica que solo es necesario obtener aquellos fichajes de los últimos 6 meses
                        "order by TypeData"
            Dim dt = CreateDataTable(sqlQuery, "LastTask")
            If (dt.Rows.Count > 0) Then
                For Each punch As DataRow In dt.Rows
                    Dim drPunch = CreateExportTask(punch)
                    lstLastPunches.Add(drPunch)
                Next
            End If
        Catch ex As Exception
            _oLog.logMessage(roLog.EventType.roError, "VTSener::roSenerValidateTasks::GetLastPunchesFromTask :" & ex.Message)
            SaveErrorToStringList(ErrorType.eLeerFichajes.ToString(), String.Format("No se han podido recuperar los fichajes de los empleados: {0}", ex.Message))
        End Try

        Return lstLastPunches
    End Function

    Private Function CreateExportTaskValidation(lstTaskValidation As List(Of ExportTasks)) As List(Of String)

        Dim file As List(Of String) = New List(Of String)

        Try
            For Each productionTask As ExportTasks In lstTaskValidation
                Dim strLine = String.Empty
                strLine += productionTask.EmployeeCode.Trim() + _strSeparator
                strLine += productionTask.ProjectCode.Trim() + _strSeparator
                strLine += productionTask.TaskCode.Trim() + _strSeparator
                strLine += productionTask.PunchDate.ToString("yyyyMMdd") + _strSeparator
                strLine += productionTask.PunchDate.ToString("HH:mm")
                file.Add(strLine)
            Next
        Catch ex As Exception
            _oLog.logMessage(roLog.EventType.roError, "VTSener::roSenerValidateTasks::CreateExportTaskValidation :" & ex.Message)
            SaveErrorToStringList(ErrorType.eGlobal.ToString(), String.Format("Se ha producido una incidencia al procesar las tareas de validación: {0}", ex.Message))
        End Try

        Return file
    End Function

    Private ReadOnly Property GetDataLinkServerKeys() As Boolean
        Get
            Dim bolret = True

            Try
                _strSeparator = Any2String(roLiveTask.getAdvancedParameterValue("VisualTime.Link.Sener.Separator"))
                _strValidationFileS = Any2String(roLiveTask.getAdvancedParameterValue("VisualTime.Link.Sener.ValidationS"))
                _strValidationFileE = Any2String(roLiveTask.getAdvancedParameterValue("VisualTime.Link.Sener.ValidationE"))
                _strDefaultBc = Any2String(roLiveTask.getAdvancedParameterValue("VisualTime.Link.Sener.DefaultBc"))
                _strOpenTaskValue = Any2String(roLiveTask.getAdvancedParameterValue("VisualTime.Link.Sener.OpenTask"))
                _strEmployeeCode = Any2String(roLiveTask.getAdvancedParameterValue("VisualTime.Link.Sener.EmployeeCode"))

                'ADD DEFAULT VALUES
                If (_strSeparator.Equals("")) Then
                    _strSeparator = "@"
                End If
                If (_strValidationFileS.Equals("")) Then
                    _strValidationFileS = "ValidacionS.txt"
                End If
                If (_strValidationFileE.Equals("")) Then
                    _strValidationFileE = "ValidacionE.txt"
                End If
                If (_strDefaultBc.Equals("")) Then
                    _strDefaultBc = "General"
                End If
                If (_strOpenTaskValue.Equals("")) Then
                    _strOpenTaskValue = "A"
                End If
                If (_strEmployeeCode.Equals("")) Then
                    _strEmployeeCode = "CODIGO EMPLEADO"
                End If
            Catch ex As Exception
                _oLog.logMessage(roLog.EventType.roError, "VTSener::roSenerValidateTasks::GetDataLinkServerKeys :" & ex.Message)
                bolret = False
            End Try

            Return bolret

        End Get
    End Property

#End Region
End Class


