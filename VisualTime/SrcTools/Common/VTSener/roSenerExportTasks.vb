Imports System.IO
Imports System.Data.Common
Imports Robotics.VTBase.roTypes
Imports Robotics.VTBase
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTUserFields
Imports Robotics.VTBase.Extensions
Imports System.Runtime.InteropServices
Imports Robotics.Base.VTBusiness.Punch
Imports Robotics.Azure
Imports Robotics.VTBase.Extensions.VTLiveTasks
Imports Robotics.Base.VTBusiness.Location
Imports Robotics.Base.VTBusiness
Imports System.Text


Public Class roSenerExportTasks
    Inherits roSener

#Region "Declarations - Constructor"
    Private ReadOnly _oLog As New roLog("VTDataLinkServer")

    Private _strEmployeeCode = String.Empty
    Private _strDateIni = String.Empty
    Private _strSeparator = String.Empty
    Private _strUserFieldProj = String.Empty
    Private _strUserFieldProd = String.Empty
    Private _strErrorsToFile As List(Of String)
    Private _strProdFile = String.Empty
    Private _strProyectFile = String.Empty

    Private Const _EXPORTS_PATH = "Exports"
    Private Const _EXPORT_ERROR_FILE = "Exports_RESULT_"
    Private Const _EXPORT_ERROR_PATH = "ExportsLogs"
    Private Const _PRODUCTION_FILE = "fichprod"
    Private Const _PROJECT_FILE = "fichajes"

    Public Sub New()
    End Sub

    Private ReadOnly Property GetDataLinkServerKeys() As Boolean
        Get
            Dim bolret = True

            Dim oBaseConn As DataLayer.roBaseConnection = Nothing

            _strDateIni = Any2DateTime(roLiveTask.getAdvancedParameterValue("VisualTime.Link.Sener.DateIniExport"))
            _strUserFieldProd = Any2String(roLiveTask.getAdvancedParameterValue("VisualTime.Link.Sener.UserFieldsProd"))
            _strUserFieldProj = Any2String(roLiveTask.getAdvancedParameterValue("VisualTime.Link.Sener.UserFieldsProj"))
            _strEmployeeCode = Any2String(roLiveTask.getAdvancedParameterValue("VisualTime.Link.Sener.EmployeeCode"))
            _strSeparator = Any2String(roLiveTask.getAdvancedParameterValue("VisualTime.Link.Sener.Separator"))
            _strProdFile = Any2String(roLiveTask.getAdvancedParameterValue("VisualTime.Link.Sener.ProductionFile"))
            _strProyectFile = Any2String(roLiveTask.getAdvancedParameterValue("VisualTime.Link.Sener.ProyectFile"))

            If (String.IsNullOrEmpty(_strDateIni)) Then
                _oLog.logMessage(roLog.EventType.roError, "VTSener::ExportTasks::GetKeys : Not found Date start value")
                bolret = False
            End If
            If (String.IsNullOrEmpty(_strUserFieldProd)) Then
                _oLog.logMessage(roLog.EventType.roError, "VTSener::ExportTasks::GetKeys : Not found User Field Production value")
                bolret = False
            End If
            If (String.IsNullOrEmpty(_strUserFieldProj)) Then
                _oLog.logMessage(roLog.EventType.roError, "VTSener::ExportTasks::GetKeys : Not found User Field Project value")
                bolret = False
            End If
            If (String.IsNullOrEmpty(_strEmployeeCode)) Then
                _oLog.logMessage(roLog.EventType.roError, "VTSener::ExportTasks::GetKeys : Not found Employee Code User Field value")
                bolret = False
            End If
            If (String.IsNullOrEmpty(_strSeparator)) Then
                _oLog.logMessage(roLog.EventType.roError, "VTSener::ExportTasks::GetKeys : Not found Separator value")
                bolret = False
            End If
            If (String.IsNullOrEmpty(_strProdFile)) Then
                _oLog.logMessage(roLog.EventType.roError, "VTSener::ExportTasks::GetKeys : Not found ProductionFile value")
                bolret = False
            End If
            If (String.IsNullOrEmpty(_strProyectFile)) Then
                _oLog.logMessage(roLog.EventType.roError, "VTSener::ExportTasks::GetKeys : Not found ProyectFile value")
                bolret = False
            End If
            Return bolret

        End Get
    End Property
#End Region

#Region "Enums"

    Public Enum ErrorType
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

    Public Enum TaskListType
        produccion
        proyecto
    End Enum
#End Region
    Public Sub ExportTasks()
        Dim oState As New roDataLinkState

        'Recuperar los registros necesarios
        Dim bolConnection As Boolean = GetDataLinkServerKeys
        If Not bolConnection Then
            Throw New Exception("No necessary keys founded")
        End If
        _strErrorsToFile = New List(Of String)

        Dim hasExported As Boolean = False
        Dim strSql = String.Empty
        Try

            Dim locationBase As String = _EXPORTS_PATH

            strSql = "@SELECT# 1 FROM sys.columns WHERE NAME = N'USR_CODIGO' AND OBJECT_ID = Object_Id(N'Groups')"

            Dim hasCampoCodigoEmpresa As Boolean = roTypes.Any2Boolean(ExecuteScalar(strSql))

            If hasCampoCodigoEmpresa Then
                ' Crea los ficheros temporales
                Dim lstProjectTask = New List(Of ExportTasks)
                Dim lstProductionTask = New List(Of ExportTasks)
                'Busco si el campo de la ficha del empleado está creado
                Dim oUserFIeldState As New VTUserFields.UserFields.roUserFieldState
                Dim oUserField As VTUserFields.UserFields.roUserField = Nothing
                'oUserField = New roUserField(oUserFIeldState, _strEmployeeCode, Types.EmployeeField, False, oTrans, False)
                oUserField = New VTUserFields.UserFields.roUserField(oUserFIeldState.IDPassport)
                oUserField.FieldName = _strEmployeeCode
                oUserField.Type = UserFieldsTypes.Types.EmployeeField

                If Not oUserField.Load(False, False) Then
                    _oLog.logMessage(roLog.EventType.roError, "VTSener::ExportTasks : El campo de la ficha del empleado " & _strEmployeeCode & " no existe")
                    SaveErrorToStringList(ErrorType.eExportar.ToString(), String.Format("El campo de la ficha del empleado {0} no existe", _strEmployeeCode))
                    Exit Sub
                End If
                'Obtengo los empleados que han fichado
                strSql = "@SELECT# distinct p.IDEmployee " +
                         "from punches p " +
                         "inner join EmployeeUserFieldValues eufv on p.IDEmployee = eufv.IDEmployee " +
                         "where DateTime >= " + Any2Time(_strDateIni).SQLSmallDateTime + " AND ActualType in (1,2,4) " +
                         "AND eufv.FieldName = 'ProductiV' and eufv.Value like 'SI' " +
                         "and Exported = 0 ORDER BY 1 DESC"

                Dim dtEmployees = CreateDataTable(strSql, "Employees")

                For Each employee As DataRow In dtEmployees.Rows
                    'Buscamos el primer fichaje de entrada o tarea para cada empleado
                    strSql = "@SELECT# top 1 * from punches where idEmployee=" + Any2String(employee("idEmployee")) +
                        " and ActualType in (1,4) and exported=0 and ShiftDate >= " + Any2Time(_strDateIni).SQLSmallDateTime +
                        " order by DateTime"
                    Dim dtFirstPunch = CreateDataTable(strSql, "FirstPunch")
                    If (dtFirstPunch.Rows.Count <= 0) Then
                        'verificamos si hay salida
                        '_oLog.logMessage(roLog.EventType.roInfo, "CDataLinkServer::ImportTasks : No records found")
                        Continue For
                    End If
                    Dim actualType = Any2Integer(dtFirstPunch.Rows(0)("ActualType"))
                    Dim isFirstFromDate = DateTime.Parse(_strDateIni).Equals(Any2DateTime(dtFirstPunch.Rows(0)("ShiftDate")))
                    Dim boolSearchTask = False
                    Dim isFirstTask = False
                    If (Not isFirstFromDate) Then
                        If (actualType.Equals(PunchTypeEnum._IN)) Then boolSearchTask = True
                    End If
                    Dim punchAnt As ExportTasks = Nothing
                    Dim punchActiveTask = New ExportTasks()
                    Dim lstExportTasks = GetPunchesData(dtFirstPunch.Rows(0)("idEmployee"), dtFirstPunch.Rows(0)("DateTime"), boolSearchTask)
                    If (lstExportTasks.Any()) Then
                        isFirstTask = lstExportTasks(0).PunchActualType.Equals(PunchTypeEnum._TASK)
                    Else
                        Continue For
                    End If
                    'Si el primer registro y no es el primero desde el día que se empezo a buscar
                    If (Not isFirstFromDate) Then
                        'Si es una entrada y bollSearchTask = True se busco la tarea anterior, la pongo como activa y la elimino para seguir desde la entrada
                        If (lstExportTasks IsNot Nothing AndAlso lstExportTasks.Any() AndAlso boolSearchTask AndAlso isFirstTask) Then
                            punchActiveTask = lstExportTasks(0)
                            lstExportTasks.RemoveAt(0)
                            punchActiveTask.IsActive = True
                        End If
                    End If
                    Dim countTask = 0
                    For Each punch As ExportTasks In lstExportTasks
                        'Guardo el primer registro y sigo, si es una tarea la marca como la activa
                        If (punchAnt Is Nothing) Then
                            punchAnt = punch
                            'Si es tarea y la primera era entrada, creo el primer movimiento con la fecha
                            If (Not isFirstFromDate AndAlso isFirstTask) Then
                                CreateExportTask(punchActiveTask, punch, False, lstProductionTask, lstProjectTask)
                            End If
                            If (actualType.Equals(PunchTypeEnum._TASK)) Then
                                Dim firstTask = lstExportTasks(0)
                                Dim countTasks = lstExportTasks.Count
                                Dim nextOut = GetTask(firstTask.EmployeeId, firstTask.PunchDate, False, (PunchTypeEnum._OUT & "," & PunchTypeEnum._TASK))
                                If (nextOut IsNot Nothing AndAlso nextOut.IsExported) Then
                                    Dim continueFor = False
                                    Dim exitFor = False
                                    SetActiveTask(punchActiveTask, firstTask)
                                    CreateExportTask(punchActiveTask, firstTask, False, lstProductionTask, lstProjectTask)
                                    CreateExportTask(punchActiveTask, nextOut, True, lstProductionTask, lstProjectTask)
                                    UpdateExportedPunch(firstTask)
                                    If (countTasks <= 1) Then Exit For
                                    punchAnt = Nothing
                                    continueFor = GetContinueFor(actualType, lstExportTasks, punchActiveTask, punch, lstProductionTask, lstProjectTask, nextOut, countTask, continueFor)
                                    If (continueFor) Then Continue For
                                End If
                                SetActiveTask(punchActiveTask, punch)
                            End If
                            If (actualType.Equals(PunchTypeEnum._IN) AndAlso punchActiveTask.IsActive = False) Then
                                Dim firstIn = lstExportTasks(0)
                                Dim previousTask = GetTask(firstIn.EmployeeId, firstIn.PunchDate, True, (PunchTypeEnum._TASK))
                                If (previousTask IsNot Nothing) Then
                                    SetActiveTask(punchActiveTask, previousTask)
                                    punchActiveTask.DateIni = firstIn.PunchDate
                                    punchActiveTask.IsActive = True
                                End If
                            End If
                            Continue For
                        End If
                        Select Case punch.PunchActualType
                            Case PunchTypeEnum._TASK
                                If (punchAnt.PunchActualType.Equals(PunchTypeEnum._IN)) Then
                                    If (Not punchActiveTask.IsActive) Then
                                        SetActiveTask(punchActiveTask, punch)
                                        CreateExportTask(punchActiveTask, punch, False, lstProductionTask, lstProjectTask)
                                        UpdateExportedPunch(punchAnt)
                                    Else
                                        punchActiveTask = ChangeTask(punch, punchActiveTask, punchAnt, lstProductionTask, lstProjectTask)
                                    End If

                                ElseIf (punchAnt.PunchActualType.Equals(PunchTypeEnum._TASK)) Then
                                    punchActiveTask = ChangeTask(punch, punchActiveTask, punchAnt, lstProductionTask, lstProjectTask)
                                End If
                            Case PunchTypeEnum._OUT
                                If (punchAnt.PunchActualType.Equals(PunchTypeEnum._TASK)) Then
                                    CreateExportTask(punchActiveTask, punch, True, lstProductionTask, lstProjectTask)
                                    UpdateExportedPunch(punchAnt)
                                ElseIf (punchAnt.PunchActualType.Equals(PunchTypeEnum._IN)) Then
                                    If (punchActiveTask.IsActive) Then
                                        CreateExportTask(punchActiveTask, punch, True, lstProductionTask, lstProjectTask)
                                    End If
                                    UpdateExportedPunch(punchAnt)
                                End If
                            Case PunchTypeEnum._IN
                                If (punchAnt.PunchActualType.Equals(PunchTypeEnum._OUT)) Then
                                    If (punchActiveTask.IsActive) Then
                                        CreateExportTask(punchActiveTask, punch, False, lstProductionTask, lstProjectTask)
                                    End If
                                    UpdateExportedPunch(punchAnt)

                                ElseIf (punchAnt.PunchActualType.Equals(PunchTypeEnum._IN) And punchActiveTask.IsActive) Then
                                    'Si son dos entradas seguidas, puede faltar un fichaje, y no se lo marca y cambio la fecha de la tarea a la nueva entrada
                                    punchActiveTask.DateIni = punch.PunchDate
                                End If
                                Dim lastTaskPunch = GetTask(punchAnt.EmployeeId, punch.PunchDate, True, PunchTypeEnum._TASK)
                                If (lastTaskPunch IsNot Nothing) Then
                                    punchActiveTask = lastTaskPunch
                                    punchActiveTask.IsActive = True
                                    punchActiveTask.DateIni = punch.PunchDate
                                End If
                        End Select
                        punchAnt = punch
                        countTask += 1
                    Next
                    'marco como tratado el último fichaje
                    If (punchAnt IsNot Nothing AndAlso Not punchAnt.PunchActualType.Equals(PunchTypeEnum._TASK) AndAlso Not punchAnt.PunchActualType.Equals(PunchTypeEnum._IN)) Then UpdateExportedPunch(punchAnt)
                Next

                PrepareTasks(lstProductionTask, locationBase, TaskListType.produccion)
                PrepareTasks(lstProjectTask, locationBase, TaskListType.proyecto)

                If lstProductionTask.Count > 0 Or lstProjectTask.Count > 0 Then
                    hasExported = True
                End If

            Else
                _oLog.logMessage(roLog.EventType.roError, "VTSener::ExportTasks : No existe el campo 'Codigo' en la ficha de empresa.")
                SaveErrorToStringList(ErrorType.eGlobal.ToString(), "No existe el campo 'Codigo' en la ficha de empresa.")
            End If

        Catch ex As Exception
            _oLog.logMessage(roLog.EventType.roError, "VTSener::ExportTasks :" & ex.Message)
            _oLog.logMessage(roLog.EventType.roError, "VTSener::ExportTasks : ->" & ex.StackTrace)
            SaveErrorToStringList(ErrorType.eGlobal.ToString(), String.Format("{0}{1}", "Ha surgido una incidencia en el proceso de exportación: ", ex.Message))

        Finally
            'Crear / enviar doc errores al servidor
            UploadErrorFile(_EXPORT_ERROR_FILE, _EXPORT_ERROR_PATH)
        End Try

    End Sub

    Private Sub PrepareTasks(tasksList As List(Of ExportTasks), locationStorage As String, pType As String)
        Dim exportLines As List(Of String) = Nothing
        Dim token As String() = Nothing
        'Creamos los ficheros de tareas Proyectos
        If tasksList.Count > 0 Then
            Dim taskListDistinctCompanies As List(Of ExportTasks) = tasksList.GroupBy(Function(x) x.FullGroupName).Select(Function(x) x.FirstOrDefault()).ToList()
            For Each exportTaskCompany In taskListDistinctCompanies
                Dim auxList As List(Of ExportTasks) = New List(Of ExportTasks)
                For Each exportTask In tasksList
                    If exportTaskCompany.FullGroupName.Equals(exportTask.FullGroupName) Then
                        auxList.Add(exportTask)
                    End If
                Next
                If auxList.Count > 0 Then
                    Select Case pType
                        Case TaskListType.produccion
                            exportLines = CreateExportTaskProduction(String.Format("{0}_{1}_", _PRODUCTION_FILE, exportTaskCompany.FullGroupName), auxList)
                            token = _strProdFile.ToString().Split(".txt".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                        Case TaskListType.proyecto
                            exportLines = CreateExportTaskProject(String.Format("{0}_{1}_", _PROJECT_FILE, exportTaskCompany.FullGroupName), auxList)
                            token = _strProyectFile.ToString().Split(".txt".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                    End Select

                    Dim fullFileName As String = String.Empty
                    If token.Length >= 1 Then
                        fullFileName = String.Format("{0}_{1}.txt", token(0), exportTaskCompany.FullGroupName)
                    Else
                        fullFileName = String.Format("{0}_{1}.txt", _strProdFile, exportTaskCompany.FullGroupName)
                    End If
                    ExportFile(locationStorage, exportLines, fullFileName)
                End If
            Next
        End If
    End Sub

    Private Sub ExportFile(locationStorage As String, exportLines As List(Of String), fileName As String)
        Try
            If exportLines IsNot Nothing Then

                Dim strFileName = locationStorage + "/" + fileName

                Dim file As Stream = GenerateStreamFromString(exportLines)

                If Not Azure.RoAzureSupport.CheckIfFileExists(fileName, roLiveDatalinkFolders.custom.ToString + "/" + locationStorage, roLiveQueueTypes.datalink) Then
                    'Si el fichero no existe, lo guardamos y renombramos a filename.timestamp.txt
                    If Not Azure.RoAzureSupport.SaveFileOnCompanyContainer(file, fileName, roLiveDatalinkFolders.custom.ToString + "/" + locationStorage, roLiveQueueTypes.datalink, True) Then
                        _oLog.logMessage(roLog.EventType.roError, String.Format("DataLinkServer::ExportFile: No se ha podido subir el fichero {0} al servidor FTP.", fileName))
                        SaveErrorToStringList(ErrorType.eExportar.ToString(), String.Format("No se ha podido subir el fichero {0} al servidor FTP.", fileName))
                    Else
                        If Not Azure.RoAzureSupport.RenameFileInCompanyContainer(strFileName, strFileName.Replace(".txt", "") & "." & Now.ToString("yyyyMMddHHmmss") & ".txt", roLiveDatalinkFolders.custom.ToString, roLiveQueueTypes.datalink) Then
                            _oLog.logMessage(roLog.EventType.roError, String.Format("DataLinkServer::ExportFile: No se ha podido renombrar el fichero {0} al servidor FTP.", fileName))
                            SaveErrorToStringList(ErrorType.eExportar.ToString(), String.Format("No se ha podido renombrar el fichero {0} al servidor FTP.", fileName))
                        End If
                    End If
                Else
                    'Si el fichero ya existe, lo guardamos directamente como filename.timestamp.txt
                    If Not Azure.RoAzureSupport.SaveFileOnCompanyContainer(file, fileName.Replace(".txt", "") & "." & Now.ToString("yyyyMMddHHmmss") & ".txt", roLiveDatalinkFolders.custom.ToString + "/" + locationStorage, roLiveQueueTypes.datalink, True) Then
                        _oLog.logMessage(roLog.EventType.roError, String.Format("DataLinkServer::ExportFile: No se ha podido subir el fichero {0} al servidor FTP.", fileName.Replace(".txt", "") & "." & Now.ToString("yyyyMMddHHmmss") & ".txt"))
                        SaveErrorToStringList(ErrorType.eExportar.ToString(), String.Format("No se ha podido subir el fichero {0} al servidor FTP.", fileName.Replace(".txt", "") & "." & Now.ToString("yyyyMMddHHmmss") & ".txt"))
                    End If
                End If
            End If
        Catch ex As Exception
            _oLog.logMessage(roLog.EventType.roError, "VTSener::ExportTasks::ExportFile: " & ex.Message)
            SaveErrorToStringList(ErrorType.eExportar.ToString(), String.Format("No se ha podido exportar el fichero {0}: {1}", fileName, ex.Message))
            Throw New Exception(ex.ToString())
        End Try
    End Sub

    Private Sub UpdateExportedPunch(punch As ExportTasks)
        Try
            Dim punchState = New roPunchState(1)
            Dim updatePunch = New roPunch(punch.EmployeeId, punch.IdPunch, punchState)
            updatePunch.Exported = 1
            updatePunch.Save()
        Catch ex As Exception
            _oLog.logMessage(roLog.EventType.roError, "VTSener::ExportTasks::UpdateExportedPunch: " & ex.Message)
            SaveErrorToStringList(ErrorType.eModificarFichaje.ToString(), String.Format("No se ha podido modificar el fichaje {0}: {1}", punch.IdPunch, ex.Message))
            Throw New Exception(ex.ToString())
        End Try

    End Sub

    Private Sub SetActiveTask(punchActiveTask As ExportTasks, punch As ExportTasks)
        Try
            punchActiveTask.PunchTypeNav = punch.PunchTypeNav
            punchActiveTask.ProjectCode = punch.ProjectCode
            punchActiveTask.TaskCode = punch.TaskCode
            punchActiveTask.DateIni = punch.PunchDate
            punchActiveTask.FullGroupName = punch.FullGroupName
        Catch ex As Exception
            _oLog.logMessage(roLog.EventType.roError, "VTSener::ExportTasks::SetActiveTask: " & ex.Message)
            SaveErrorToStringList(ErrorType.eActivarTarea.ToString(), String.Format("No se ha podido activar la tarea {0}: {1}", punchActiveTask.TaskCode, ex.Message))
            Throw New Exception(ex.ToString())
        End Try
    End Sub

    Private Sub CreateExportTask(punchTask As ExportTasks, punch As ExportTasks, isFinal As Boolean, lstProductionTask As List(Of ExportTasks), lstProjectTask As List(Of ExportTasks))
        Try
            If (isFinal) Then
                Dim exportTask = New ExportTasks()
                With exportTask
                    .DateIni = punchTask.DateIni
                    .DateEnd = punch.PunchDate
                    .ProjectCode = punchTask.ProjectCode
                    .TaskCode = punchTask.TaskCode
                    .EmployeeCode = punch.EmployeeCode
                    .PunchTypeNav = punchTask.PunchTypeNav
                    .FullGroupName = punchTask.FullGroupName
                End With
                If Not String.IsNullOrEmpty(exportTask.FullGroupName) Then
                    Dim token As String() = exportTask.FullGroupName.Split("\")
                    If token.Length > 0 Then
                        exportTask.FullGroupName = token(0).Trim()
                    End If
                End If
                If (punchTask.PunchTypeNav.Equals(_strUserFieldProd)) Then
                    lstProductionTask.Add(exportTask)
                ElseIf (punchTask.PunchTypeNav.Equals(_strUserFieldProj)) Then
                    lstProjectTask.Add(exportTask)
                End If
            Else
                punchTask.DateIni = punch.PunchDate
                punchTask.IsActive = True
            End If
        Catch ex As Exception
            _oLog.logMessage(roLog.EventType.roError, "VTSener::ExportTasks::CreateExportTask: " & ex.Message)
            SaveErrorToStringList(ErrorType.eCrearTareas.ToString(), String.Format("No se ha podido crear la tarea de exportación {0}: {1}", punchTask.TaskCode, ex.Message))
            Throw New Exception(ex.ToString())
        End Try

    End Sub

    Private Function CreateExportTaskProduction(fileName As String, lstProductionTask As List(Of ExportTasks)) As List(Of String)

        Dim nameFile As String = String.Format("{0}{1}{2}", fileName, Date.Now.ToString("yyyyMMddHHmmss"), ".txt")
        Dim strLines As List(Of String) = New List(Of String)()

        Try
            For Each productionTask As ExportTasks In lstProductionTask
                Dim strLine = String.Empty
                strLine += productionTask.EmployeeCode.Trim() + _strSeparator
                strLine += productionTask.DateIni.ToString("yyyyMMdd") + _strSeparator
                strLine += productionTask.DateIni.ToString("HH:mm") + _strSeparator
                strLine += productionTask.DateEnd.ToString("yyyyMMdd") + _strSeparator
                strLine += productionTask.DateEnd.ToString("HH:mm") + _strSeparator
                strLine += productionTask.ProjectCode.Trim() + _strSeparator
                strLine += productionTask.TaskCode.Trim() + _strSeparator
                strLine += productionTask.PunchTypeNav
                strLines.Add(strLine)
            Next


        Catch ex As Exception
            _oLog.logMessage(roLog.EventType.roError, "VTSener::ExportTasks::CreateExportTaskProduction: " & ex.Message)
            Throw New Exception(ex.ToString())
        End Try

        Return strLines

    End Function

    Private Function CreateExportTaskProject(fileName As String, lstProjectTask As List(Of ExportTasks)) As List(Of String)

        Dim nameFile As String = String.Format("{0}{1}{2}", fileName, Date.Now.ToString("yyyyMMddHHmmss"), ".txt")
        Dim strLines As List(Of String) = New List(Of String)()

        Try
            For Each productionTask As ExportTasks In lstProjectTask
                Dim strLine = String.Empty
                strLine += productionTask.EmployeeCode.Trim() + _strSeparator
                strLine += productionTask.ProjectCode.Trim() + _strSeparator
                strLine += productionTask.TaskCode.Trim() + _strSeparator
                strLine += productionTask.DateIni.ToString("ddMMyyyy") + _strSeparator
                Dim ts = productionTask.DateEnd - productionTask.DateIni

                strLine += (Math.Truncate(ts.TotalHours * 1000) / 1000).ToString() + _strSeparator
                strLine += productionTask.PunchTypeNav
                strLines.Add(strLine)
            Next

        Catch ex As Exception
            _oLog.logMessage(roLog.EventType.roError, "VTSener::ExportTasks::CreateExportTaskProject: " & ex.Message)
            Throw New Exception(ex.ToString())
        End Try
        Return strLines

    End Function
    Private Function ChangeTask(punch As ExportTasks, punchActiveTask As ExportTasks, punchAnt As ExportTasks, lstProductionTask As List(Of ExportTasks), lstProjectTask As List(Of ExportTasks)) As ExportTasks
        Try
            CreateExportTask(punchActiveTask, punch, True, lstProductionTask, lstProjectTask)
            punchActiveTask = punch
            punchActiveTask.IsActive = True
            CreateExportTask(punchActiveTask, punch, False, lstProductionTask, lstProjectTask)

            UpdateExportedPunch(punchAnt)
            Return punchActiveTask
        Catch ex As Exception
            _oLog.logMessage(roLog.EventType.roError, "VTSener::ExportTasks::ChangeTask: " & ex.Message)
            SaveErrorToStringList(ErrorType.eActivarTarea.ToString(), String.Format("No se ha podido cambiar la tarea {0}: {1}", punchActiveTask.TaskCode, ex.Message))
            Throw New Exception(ex.ToString())
        End Try

    End Function
    Private Function GetTask(idEmployee As String, dateFirstPunch As String, preview As Boolean, punchType As String) As ExportTasks
        Try
            Dim sqlQuery As Object

            sqlQuery = "@SELECT# top 1 p.IDEmployee, DSC.NumContrato, p.ShiftDate, p.DateTime, p.TypeData, T.Project,P.ActualType," +
                       "T.ShortName, BC.Name as CenterName, t.Field1, T.Id, T.Project, EUF.Value as Type, t.Name as TaskName, p.Id as IdPunch, p.Exported, " +
                       "(@SELECT# USR_Codigo FROM Groups G INNER JOIN sysrovwCurrentEmployeeGroups VG ON G.ID = (@SELECT# SUBSTRING(Path,0,charindex('\', Path,0)) from sysrovwCurrentEmployeeGroups WHERE IDEmployee = " + idEmployee + ") WHERE VG.IDEmployee = " + idEmployee + ") AS FullGroupName " +
                       " FROM Punches P " +
                       " Left join Tasks T on T.ID=P.typedata " +
                       " Left join BusinessCenters BC on BC.ID=T.IDCenter " +
                       " Inner join sysroDailyScheduleByContract DSC on DSC.IDEmployee =P.IDEmployee AND DSC.Date=P.ShiftDate " +
                       " Inner join EmployeeUserFieldValues EUF ON P.IDEmployee = EUF.IDEmployee" +
                       " WHERE p.idEmployee in (" + idEmployee + ") " +
                       " and (P.DateTime " + IIf(preview, " <= " & Any2Time(dateFirstPunch).SQLSmallDateTime, "> CONVERT(DATETIME,'" & dateFirstPunch & "',104)") & ")" +
                       " AND P.ActualType in ( " + punchType + ") and EUF.FieldName = '" + _strEmployeeCode + "'" +
                       " ORDER BY DSC.IDEmployee, p.DateTime " + IIf(preview, "desc", "") + ", p.ActualType"
            Dim dt = CreateDataTable(sqlQuery, "LastTask")
            If (dt.Rows.Count > 0) Then
                Return CreateExportTask(dt.Rows(0))
            Else
                Return Nothing
            End If
        Catch ex As Exception
            _oLog.logMessage(roLog.EventType.roError, "VTSener::ExportTasks::GetTask: " & ex.Message)
            Throw New Exception(ex.ToString())
        End Try


    End Function

    Private Function GetContinueFor(ByRef actualType As Integer, lstExportTasks As List(Of ExportTasks), ByRef punchActiveTask As ExportTasks, punch As ExportTasks, lstProductionTask As List(Of ExportTasks), lstProjectTask As List(Of ExportTasks), nextOut As ExportTasks, countTask As Integer, continueFor As Boolean) As Boolean
        Try
            Dim nextPunch = lstExportTasks(countTask + 1)
            If (nextPunch.PunchActualType.Equals(PunchTypeEnum._IN)) Then
                Dim lastTaskPunch = GetTask(nextOut.EmployeeId, nextPunch.PunchDate, True, PunchTypeEnum._TASK)
                If (lastTaskPunch IsNot Nothing) Then
                    punchActiveTask = lastTaskPunch
                    punchActiveTask.IsActive = True
                    punchActiveTask.DateIni = punch.PunchDate
                    CreateExportTask(punchActiveTask, nextPunch, False, lstProductionTask, lstProjectTask)
                    actualType = nextPunch.PunchActualType
                    continueFor = True
                End If
            End If
            Return continueFor
        Catch ex As Exception
            _oLog.logMessage(roLog.EventType.roError, "VTSener::ExportTasks::GetContinueFor::ExportTasks :" & ex.Message)
            Throw New Exception(ex.ToString())
        End Try

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

    Private Function GetPunchesData(idEmployee As String, dateFirstPunch As String, isFirstFromFate As Boolean) As List(Of ExportTasks)
        Dim lstExportTasks = New List(Of ExportTasks)
        Try

            Dim sqlQuery = "@DECLARE# @dateUser date
                            set @dateUser = GETDATE()
                        @SELECT# p.IDEmployee, p.ShiftDate, p.DateTime, p.TypeData, 
                        T.Project,P.ActualType,T.ShortName, BC.Name as CenterName, t.Field1, T.Id, 
                        T.Project, (@SELECT# value from dbo.GetEmployeeAllUserFieldValue(" + idEmployee + ",@dateUser) where FieldName = '" + _strEmployeeCode + "') as Type, 
                        t.Name as TaskName, p.Id as IdPunch, p.Exported, (@SELECT# USR_Codigo FROM Groups G INNER JOIN sysrovwCurrentEmployeeGroups VG ON G.ID = (@SELECT# SUBSTRING(Path,0,charindex('\', Path,0)) from sysrovwCurrentEmployeeGroups WHERE IDEmployee = " + idEmployee + ") WHERE VG.IDEmployee = " + idEmployee + ") AS FullGroupName
                        FROM Punches P  
                        Left join Tasks T on T.ID=P.typedata  
                        Left join BusinessCenters BC on BC.ID=T.IDCenter  
                        WHERE p.idEmployee in (" + idEmployee + ") and(P.ShiftDate > = " + Any2Time(_strDateIni).SQLSmallDateTime + ") 
                        AND P.ActualType in (1,2,4) and p.Exported = 0 
                        ORDER BY P.IDEmployee, p.DateTime, p.ActualType"

            Dim dt = CreateDataTable(sqlQuery, "ExportTasks")
            If (isFirstFromFate) Then
                Dim task = GetTask(idEmployee, dateFirstPunch, True, PunchTypeEnum._TASK)
                If (task IsNot Nothing) Then lstExportTasks.Add(task)
            End If
            lstExportTasks.AddRange(From row As DataRow In dt.Rows Select CreateExportTask(row))
        Catch ex As Exception
            _oLog.logMessage(roLog.EventType.roError, "VTSener::ExportTasks::GetPunchesData: " & ex.Message)
            SaveErrorToStringList(ErrorType.eLeerFichajes.ToString(), String.Format("No se han podido obtener los fichajes: {0}", ex.Message))
            lstExportTasks = Nothing
        End Try
        Return lstExportTasks
    End Function

End Class



