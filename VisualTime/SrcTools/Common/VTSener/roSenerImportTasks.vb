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

Public Class roSenerImportTasks
    Inherits roSener
#Region "Constantes privadas"

    Private Const _IMPORTS_PATH = "Imports"
    Private Const _IMPORTS_LOG_PATH = "ImportsLogs"
    Private Const _IMPORT_ERROR_FILE = "Imports_RESULT_"

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

    Private _strOrdersFile = String.Empty
    Private _strOperationFile = String.Empty
    Private _strDefaultBc = String.Empty
    Private _strSeparator = String.Empty
    Private _strErrorsToFile As List(Of String) = New List(Of String)

    Public Sub New()
        roLog.GetInstance.logMessage(roLog.EventType.roDebug, "VTSener::Start::roSenerImportTasks(" +
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



#Region "Importación de tareas a ProductiV desde Nav"
    Public Function ImportTasks() As Boolean
        Dim oTaskState As New roTaskState
        Dim strSourceOrdersFileName = _IMPORTS_PATH + "/" + _strOrdersFile
        Dim strSourceOperationsFileName = _IMPORTS_PATH + "/" + _strOperationFile
        Dim strSourceBackupOrdersFileName = _IMPORTS_PATH + "/bck/" + _strOrdersFile
        Dim strSourceBackupOperationsFileName = _IMPORTS_PATH + "/bck/" + _strOperationFile

        Try
            'OBTENEMOS LOS FICHEROS DE ORDERS Y OPERATIONS
            Dim oFileOrders As Byte() = Azure.RoAzureSupport.DownloadFileFromCompanyContainer(strSourceOrdersFileName, roLiveDatalinkFolders.custom.ToString, roLiveQueueTypes.dataLink)
            Dim oFileOperations As Byte() = Azure.RoAzureSupport.DownloadFileFromCompanyContainer(strSourceOperationsFileName, roLiveDatalinkFolders.custom.ToString, roLiveQueueTypes.dataLink)
            If (oFileOrders IsNot Nothing AndAlso oFileOrders.Length > 0) And (oFileOperations IsNot Nothing AndAlso oFileOperations.Length > 0) Then
                'Hacemos backup de los ficheros
                Azure.RoAzureSupport.RenameFileInCompanyContainer(strSourceOperationsFileName, strSourceBackupOperationsFileName & "." & Now.ToString("yyyyMMddHHmmss") & ".bck", roLiveDatalinkFolders.custom.ToString, roLiveQueueTypes.dataLink)
                Azure.RoAzureSupport.RenameFileInCompanyContainer(strSourceOrdersFileName, strSourceBackupOrdersFileName & "." & Now.ToString("yyyyMMddHHmmss") & ".bck", roLiveDatalinkFolders.custom.ToString, roLiveQueueTypes.dataLink)

                'Obtenemos las orders y operations
                Dim orders As List(Of roOrder) = GetOrders(oFileOrders)
                Dim operations = GetOperations(oFileOperations)

                If orders IsNot Nothing OrElse operations IsNot Nothing Then
                    'No pueden existir tareas con el código de proyecto en blanco.
                    Dim blankOrders = orders.FindAll(Function(op) op.ProyectCode = String.Empty)
                    For Each oBlankOrder In blankOrders
                        roLog.GetInstance.logMessage(roLog.EventType.roError, "VTSener::ImportTasks -Orden sin código de proyecto: " & oBlankOrder.ProyectCode & "@" & oBlankOrder.ProyectDescription & "@" & oBlankOrder.PunchType)
                        SaveErrorToStringList(ErrorType.eCrearTareas.ToString(), String.Format("{0}{1}{2}{3}{4}{5}", "Orden sin código de proyecto: ", oBlankOrder.ProyectCode, "@", oBlankOrder.ProyectDescription, "@", oBlankOrder.PunchType))
                    Next
                    orders = orders.FindAll(Function(op) op.ProyectCode <> String.Empty)

                    'No pueden existir tareas con el código de proyecto en blanco.
                    Dim blankOperations = operations.FindAll(Function(op) op.ProyectCode = String.Empty)
                    For Each oBlankOperation In blankOperations
                        roLog.GetInstance.logMessage(roLog.EventType.roError, "VTSener::ImportTasks -Operacion sin código de proyecto: " & oBlankOperation.ProyectCode & "@" & oBlankOperation.TaskCode & "@" & oBlankOperation.TaskDescription & "@" & oBlankOperation.BarCode)
                        SaveErrorToStringList(ErrorType.eCrearTareas.ToString(), String.Format("{0}{1}{2}{3}{4}{5}{6}{7}", "Operacion sin código de proyecto: ", oBlankOperation.ProyectCode, "@", oBlankOperation.TaskCode, "@", oBlankOperation.TaskDescription, "@", oBlankOperation.BarCode))
                    Next
                    operations = operations.FindAll(Function(op) op.ProyectCode <> String.Empty)

                    Dim tasks = New SortedDictionary(Of roOrder, List(Of roOperation))
                    Dim response = ValidateOrderAndOperation(orders, operations, oTaskState)
                    If (Not String.IsNullOrEmpty(response)) Then
                        roLog.GetInstance.logMessage(roLog.EventType.roError, "VTSener::ImportTasks  " + response)
                        Return False
                    End If
                    If (orders.Count.Equals(0) Or operations.Count.Equals(0)) Then Return False
                    For Each roOrder In orders
                        Dim roOperations = operations.Where(Function(ope) ope.ProyectCode.Equals(roOrder.ProyectCode)).ToList()
                        If (Not tasks.ContainsKey(roOrder)) Then tasks.Add(roOrder, roOperations)
                    Next
                    ProcessTasks(tasks, oTaskState)
                Else
                    SaveErrorToStringList(ErrorType.eGlobal.ToString(), String.Format("{0}", "Uno o más documentos no contienen información."))
                End If

                'Else
                'SaveErrorToStringList(ErrorType.eGlobal.ToString(), String.Format("Es necesario que se añadan los dos documentos: {0} y {1}", _strOperationFile, _strOrdersFile))
            End If

        Catch ex As Exception
            _oLog.logMessage(roLog.EventType.roError, "CDataLinkServer::ImportTasks :" & ex.Message)
            SaveErrorToStringList(ErrorType.eGlobal.ToString(), String.Format("{0}{1}", "Ha surgido una incidencia en el proceso de importación: ", ex.Message))
        Finally
            'Crear / enviar doc errores al servidor
            UploadErrorFile(_IMPORT_ERROR_FILE, _IMPORTS_LOG_PATH)
        End Try

        Return True


    End Function

    Private Function ValidateOrderAndOperation(ByRef roOrders As List(Of roOrder), roOperations As List(Of roOperation), oTaskState As roTaskState) As String
        'valido si el proyecto de las operaciones viene en el fichero de ordenes, si no viene lo busco en la bbdd para cargarlo
        Dim oTaskFieldState As New VTUserFields.UserFields.roTaskFieldState()

        Dim projectsOp = roOperations.Select(Function(op) op.ProyectCode).Distinct().ToList()
        For Each operationProject As String In projectsOp
            Dim project = roOrders.FirstOrDefault(Function(pr) pr.ProyectCode.Equals(operationProject))
            If (project IsNot Nothing) Then Continue For
            Dim strSql = "@SELECT# ID FROM Tasks WHERE NAME LIKE '%" & operationProject & "%'"
            Dim intIdTask = roTypes.Any2Integer(ExecuteScalar(strSql))
            If (intIdTask.Equals(0)) Then
                Return "El proyecto indicado no existe " & operationProject
            End If
            Dim oTask As New roTask(intIdTask, oTaskState)
            Dim newProject = New roOrder()
            Dim tbFields As DataTable = VTUserFields.UserFields.roUserField.GetTaskFields(UserFieldsTypes.Types.TaskField, Nothing)
            Dim oTaskField = New VTUserFields.UserFields.roTaskField()
            For Each rw As DataRow In tbFields.Rows
                oTaskField = New VTUserFields.UserFields.roTaskField(oTaskFieldState, oTask.ID, rw("ID"))
                Exit For
            Next
            Dim projectFields = oTask.Project.Split("/")
            With newProject
                .ProyectCode = projectFields(0)
                .ProyectDescription = projectFields(1)
                .PunchType = oTaskField.FieldValue
            End With
            roOrders.Add(newProject)
        Next
        Return String.Empty
    End Function

    Private Sub ProcessTasks(ByVal tasks As SortedDictionary(Of roOrder, List(Of roOperation)), ByVal oTaskState As roTaskState)
        Dim oCn As DbConnection = Nothing
        Dim bolCloseCn As Boolean = False
        Dim operationCode = String.Empty
        Dim boolRet = False

        Try
            bolCloseCn = True

            Dim intIdCenter = roTypes.Any2Integer(ExecuteScalar("@SELECT# ID  from BusinessCenters WHERE Name='" + _strDefaultBc + "' "))
            Dim lstGroupsAlreadySearched As List(Of roGroup) = New List(Of roGroup)

            'Debemos crear el centro de coste por defecto en caso necesario
            If intIdCenter.Equals(0) Then
                Dim oCenter As New BusinessCenter.roBusinessCenter() With {.ID = -1, .Name = _strDefaultBc, .Status = 1, .AuthorizationMode = 0}
                If oCenter.Save(True) Then
                    intIdCenter = oCenter.ID
                End If
            End If
            For Each pairTask As KeyValuePair(Of roOrder, List(Of roOperation)) In tasks
                Dim project = pairTask.Key
                Dim tasksProject = pairTask.Value

                Try
                    If (tasksProject.Count.Equals(0)) Then
                        'valido si existe algúna tarea con ese proyecto para actualizar
                        Dim strSql = "@SELECT# ID FROM Tasks WHERE NAME LIKE '%" & project.ProyectCode & "%'"
                        Dim intIdTask = roTypes.Any2Integer(ExecuteScalar(strSql))
                        Dim emptyOperation = New roOperation()
                        boolRet = CreateOrUpdateTask(intIdTask, intIdCenter, oTaskState, emptyOperation, project)
                        If (Not boolRet) Then
                            roLog.GetInstance.logMessage(roLog.EventType.roError, String.Format("VTSener::ImportTasks - CreateOrUpdateTask, idTask {0}: {1}", intIdTask, oTaskState.Result.ToString()))
                            SaveErrorToStringList(ErrorType.eCrearTareas.ToString(), String.Format("Ha surgido un problema al crear la tarea {0}, se cancela el proceso de importación.", intIdTask))
                            Exit For
                        End If
                    Else
                        Dim lstIdGroups As List(Of Integer)
                        For Each operation As roOperation In tasksProject
                            lstIdGroups = New List(Of Integer)
                            lstIdGroups = GetOperationGroupsIds(operation.GroupsCode, lstGroupsAlreadySearched)
                            Dim intIdTask = 0
                            Dim strSql = "@SELECT# count(ID) FROM Tasks WHERE NAME LIKE '%" + project.ProyectCode + "/" + operation.TaskCode + "%' OR NAME LIKE '%" & project.ProyectCode & "//%'"
                            If roTypes.Any2Integer(ExecuteScalar(strSql)) > 1 Then
                                strSql = "@SELECT# ID FROM Tasks WHERE BARCODE LIKE '" + project.ProyectCode + operation.TaskCode + "'"
                                intIdTask = roTypes.Any2Integer(ExecuteScalar(strSql))
                            Else
                                strSql = "@SELECT# ID FROM Tasks WHERE NAME LIKE '%" + project.ProyectCode + "/" + operation.TaskCode + "%' OR NAME LIKE '%" & project.ProyectCode & "//%'"
                                intIdTask = roTypes.Any2Integer(ExecuteScalar(strSql))
                            End If

                            operationCode = operation.TaskCode
                            'Creamos o actualizamos la tarea
                            boolRet = CreateOrUpdateTask(intIdTask, intIdCenter, oTaskState, operation, project,, lstIdGroups)
                            If (Not boolRet) Then
                                roLog.GetInstance.logMessage(roLog.EventType.roError, String.Format("VTSener::ImportTasks - CreateOrUpdateTask, idTask {0}: {1}", intIdTask, oTaskState.Result.ToString()))
                                SaveErrorToStringList(ErrorType.eCrearTareas.ToString(), String.Format("Ha surgido un problema al crear la tarea {0}, se cancela el proceso de importación.", intIdTask))
                                Exit For
                            End If
                        Next
                    End If
                Catch ex As Exception
                    roLog.GetInstance.logMessage(roLog.EventType.roError, "VTSener::ImportTasks - UnknownErrorOnRegister " + project.ProyectCode + "/" + operationCode)
                Finally
                    If (boolRet) Then
                        roLog.GetInstance.logMessage(roLog.EventType.roInfo, "VTSener::ImportTasks - Se han importado " & tasks.Values.Sum(Function(sm) sm.Count) & " tareas")
                    End If
                End Try
            Next
        Catch ex As Exception
            roLog.GetInstance.logMessage(roLog.EventType.roError, "roSennerImportTasks::ProcessTasks::Exception::" & ex.Message)
        End Try

    End Sub

    ''' <summary>
    ''' Obtiene los ids de los grupos introducidos a importar
    ''' </summary>
    ''' <param name="lstOperationGroups">Lista de grupos de la operación</param>
    ''' <param name="lstGroupsAlreadySearched">Lista de grupos ya buscados</param>
    ''' <returns></returns>
    Private Function GetOperationGroupsIds(lstOperationGroups As String, ByRef lstGroupsAlreadySearched As List(Of roGroup)) As List(Of Integer)

        Dim idGroupArray As String() = lstOperationGroups.Split(",")
        Dim lstDistinctIdGroups As List(Of String) = New List(Of String)
        Dim lstIdGroups As List(Of Integer) = New List(Of Integer)
        For Each idGroup In idGroupArray
            idGroup = idGroup.Trim()
            If Not lstDistinctIdGroups.Contains(idGroup) Then
                lstDistinctIdGroups.Add(idGroup)
            End If
        Next

        For Each idGroup In lstDistinctIdGroups
            Dim group As roGroup = Nothing
            Dim oGroupState As roGroupState = New roGroupState()
            Dim groupFound As Boolean = False

            If Not String.IsNullOrEmpty(idGroup) Then
                For Each groupSearched In lstGroupsAlreadySearched
                    If groupSearched.Name.StartsWith(idGroup) Then
                        group = groupSearched
                        groupFound = True
                        Exit For
                    End If
                Next

                If Not groupFound Then
                    group = roGroup.GetGroupByName(String.Format("{0}{1}", idGroup, "%"), oGroupState,,)
                    If group IsNot Nothing Then
                        lstGroupsAlreadySearched.Add(group)
                    End If
                End If

                If Not group Is Nothing Then
                    If group.Name.StartsWith(idGroup) Then
                        lstIdGroups.Add(group.ID)
                    End If
                End If
            End If
        Next

        Return lstIdGroups

    End Function

    Private Function GetOrders(ByVal oFileOrders As Byte()) As List(Of roOrder)
        Dim strOrderLine As String = String.Empty
        Dim orderLst = New List(Of roOrder)()
        Dim index As Int16 = 1
        Try
            Dim memStream As New MemoryStream(oFileOrders)
            Dim oReader As New StreamReader(memStream)

            While Not oReader.EndOfStream
                strOrderLine = oReader.ReadLine()
                Dim order = New roOrder(strOrderLine, _strSeparator)
                If (Not orderLst.Contains(order)) Then orderLst.Add(order)
                index += 1
            End While
            oReader.Close()
            memStream.Close()
        Catch ex As Exception
            If Not String.IsNullOrEmpty(strOrderLine) Then
                SaveErrorToStringList(ErrorType.eLeerOrdenes.ToString(), String.Format("{0}{1}{2}{3}", "No se ha podido leer correctamente la orden de la línea ", index, ": ", strOrderLine))
            End If
            SaveErrorToStringList(ErrorType.eLeerOrdenes.ToString(), String.Format("{0}{1}", "Ha surgido una incidencia en la lectura de las ordenes: ", ex.Message))
            roLog.GetInstance.logMessage(roLog.EventType.roError, "roSennerImportTasks::GetOrders::Unhandled exception::" & ex.Message)
            Throw New Exception(ex.ToString())
        End Try

        Return orderLst
    End Function

    Private Function GetOperations(ByVal oFileOperations As Byte()) As List(Of roOperation)
        Dim strOrderLine As String = String.Empty
        Dim opeLst = New List(Of roOperation)()
        Dim index As Int16 = 1
        Try
            Dim memStream As New MemoryStream(oFileOperations)
            Dim oReader As New StreamReader(memStream)
            While Not oReader.EndOfStream
                strOrderLine = oReader.ReadLine()
                Dim operationFiels As String() = strOrderLine.Split(_strSeparator)
                Dim operation As roOperation = Nothing
                If (operationFiels.Count().Equals(5)) Then
                    operation = New roOperation(strOrderLine, _strSeparator)
                    If (Not operation Is Nothing AndAlso Not opeLst.Contains(operation)) Then
                        opeLst.Add(operation)
                    End If
                Else
                    roLog.GetInstance.logMessage(roLog.EventType.roError, "roSennerImportTasks::GetOperations::" & String.Format("La operación de la línea {0} no continene los campos necesarios: {1}", index, strOrderLine))
                    SaveErrorToStringList(ErrorType.eLeerOperaciones.ToString(), String.Format("La operación de la línea {0} no continene los campos necesarios: {1}", index, strOrderLine))
                End If
                index += 1
            End While
            oReader.Close()
            memStream.Close()
        Catch ex As Exception
            If Not String.IsNullOrEmpty(strOrderLine) Then
                SaveErrorToStringList(ErrorType.eLeerOperaciones.ToString(), String.Format("{0}{1}{2}{3}", "No se ha podido leer correctamente la operación de la línea ", index, ": ", strOrderLine))
            End If
            SaveErrorToStringList(ErrorType.eLeerOperaciones.ToString(), String.Format("{0}{1}", "Ha surgido una incidencia en la lectura de las operaciones: ", ex.Message))
            Throw New Exception(ex.ToString())
            roLog.GetInstance.logMessage(roLog.EventType.roError, "roSennerImportTasks::GetOperations::Unhandled exception::" & ex.Message)
        End Try

        Return opeLst
    End Function

    Private ReadOnly Property GetDataLinkServerKeys() As Boolean
        Get
            Dim bolret = True

            Try
                _strSeparator = Any2String(roLiveTask.getAdvancedParameterValue("VisualTime.Link.Sener.Separator"))
                _strOrdersFile = Any2String(roLiveTask.getAdvancedParameterValue("VisualTime.Link.Sener.OrdersFile"))
                _strOperationFile = Any2String(roLiveTask.getAdvancedParameterValue("VisualTime.Link.Sener.OperationsFile"))
                _strDefaultBc = Any2String(roLiveTask.getAdvancedParameterValue("VisualTime.Link.Sener.DefaultBc"))

                'ADD DEFAULT VALUES
                If (_strSeparator.Equals("")) Then
                    _strSeparator = "@"
                End If
                If (_strOrdersFile.Equals("")) Then
                    _strOrdersFile = "ordenes.txt"
                End If
                If (_strOperationFile.Equals("")) Then
                    _strOperationFile = "operaciones.txt"
                End If
                If (_strDefaultBc.Equals("")) Then
                    _strDefaultBc = "General"
                End If
            Catch ex As Exception
                _oLog.logMessage(roLog.EventType.roError, "VTSener::roSenerImportTasks::GetDataLinkServerKeys :" & ex.Message)
                bolret = False
            End Try

            Return bolret

        End Get
    End Property
#End Region

End Class


