Imports System.Data.Common
Imports System.IO
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Task
Imports Robotics.VTBase

Public Class roSener

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

    Private _strErrorsToFile As List(Of String) = New List(Of String)

    Public Sub New()

    End Sub

#End Region

#Region "Methods"
    ''' <summary>
    ''' Actualiza los grupos que tiene la task asignados
    ''' </summary>
    ''' <param name="oTask">Tarea a tratar</param>
    ''' <param name="lstIdsGroups">Lista actualizada de grupos a asignar</param>
    Sub UpdateTaskGroups(ByRef oTask As roTask, lstIdsGroups As List(Of Integer))
        If Not lstIdsGroups Is Nothing AndAlso lstIdsGroups.Count > 0 Then
            oTask.Groups = New List(Of roGroupTaskDescription)
            oTask.TypeAuthorization = TaskTypeAuthorizationEnum._SELECTEDEMPLOYEES
            For Each idGroup In lstIdsGroups
                Dim oGroupTaskState As roGroupTaskState = New roGroupTaskState()
                oTask.Groups.Add(New roGroupTaskDescription(idGroup, oGroupTaskState))
            Next
        Else
            oTask.TypeAuthorization = TaskTypeAuthorizationEnum._ANYEMPLOYEE
        End If
    End Sub

    Function CreateOrUpdateTask(intIdTask As Integer, intIdCenter As Integer, oTaskState As roTaskState, operation As roOperation, project As roOrder, Optional ByRef idTasksFromOperations As List(Of Integer) = Nothing, Optional lstIdsGroups As List(Of Integer) = Nothing) As Boolean
        Dim oTask As New roTask(If(intIdTask.Equals(0), -1, intIdTask), oTaskState, False)
        Dim isOK As Boolean = False
        With oTask
            .IDCenter = intIdCenter
            .Name = project.ProyectCode + "/" + operation.TaskCode + "/" + operation.TaskDescription
            .ShortName = String.Empty
            .Description = operation.TaskDescription
            .Status = TaskStatusEnum._ON
            .Project = project.ProyectCode + "/" + project.ProyectDescription
            .BarCode = operation.BarCode
        End With

        If oTask.Name.Length <= 75 Then
            UpdateTaskGroups(oTask, lstIdsGroups)

            roLog.GetInstance.logMessage(roLog.EventType.roInfo, "VTSener::roSener - CreateOrUpdateTask-> Se creara o actualizará la siguiente tarea 
                                                                      ID: " & intIdTask.ToString & "-Centro: " & intIdCenter.ToString & "-Nombre: " & oTask.Name &
                                                                      "-Proyecto: " & oTask.Project & "- Codigo de barras: " & oTask.BarCode)
            If (oTask.Save(True)) Then
                Dim lstTaskField As New List(Of VTUserFields.UserFields.roTaskField)
                Dim tbFields As DataTable = VTUserFields.UserFields.roUserField.GetTaskFields(UserFieldsTypes.Types.TaskField, Nothing, )
                For Each rw As DataRow In tbFields.Rows
                    Dim oTaskField As New VTUserFields.UserFields.roTaskField() With {.IDTask = oTask.ID, .IDField = rw("ID"), .Type = rw("Type"), .FieldName = rw("Name"), .Action = rw("Action"), .FieldValue = project.PunchType}
                    lstTaskField.Add(oTaskField)
                    Exit For
                Next
                Dim oTaskFieldState As New VTUserFields.UserFields.roTaskFieldState()
                If idTasksFromOperations IsNot Nothing Then
                    idTasksFromOperations.Add(oTask.ID)
                End If
                Try
                    isOK = VTUserFields.UserFields.roTaskField.SaveTaskFields(oTask.ID, lstTaskField, oTaskFieldState, True)
                Catch ex As Exception
                    roLog.GetInstance.logMessage(roLog.EventType.roError, String.Format("VTSener::CreateOrUpdateTask : No se han podido guardar los campos de la tarea {0}: {1}", oTask.ID, ex.Message))
                    SaveErrorToStringList(ErrorType.eCrearTareas.ToString(), String.Format("No se han podido guardar los campos de la tarea {0} - {1} ", operation.TaskCode, oTask.Name))
                End Try
            End If
        Else
            isOK = True
            roLog.GetInstance.logMessage(roLog.EventType.roError, "roSener::CreateOrUpdateTask::" & String.Format("No se ha podido crear la tarea {0}, su nombre es superior a 75 carácteres de longitud: {1} ", operation.TaskCode, oTask.Name))
            SaveErrorToStringList(ErrorType.eCrearTareas.ToString(), String.Format("No se ha podido crear la tarea {0}, su nombre es superior a 75 carácteres de longitud: {1} ", operation.TaskCode, oTask.Name))
        End If
        Return isOK

    End Function

    Sub ResetErrorLogs()
        _strErrorsToFile = New List(Of String)
    End Sub

    Sub SaveErrorToStringList(id As String, message As String)
        If Not String.IsNullOrEmpty(id) And Not String.IsNullOrEmpty(message) Then
            _strErrorsToFile.Add(String.Format("{0}||{1}", id, message))
        End If
    End Sub

    Sub UploadErrorFile(logFileName As String, logPath As String)

        Try
            If _strErrorsToFile.Count > 0 Then
                Dim vSettings = New roSettings()
                Dim nameFile As String = String.Format("{0}{1}{2}", logFileName, Date.Now.ToString("yyyyMMddHHmmss"), ".txt")
                Dim fullPathFile As String = String.Format("{0}{1}{2}", vSettings.GetVTSetting(eKeys.DataLink).ToString(), Path.DirectorySeparatorChar, nameFile)

                Dim file As Stream = GenerateStreamFromString(_strErrorsToFile)
                Azure.RoAzureSupport.SaveFileOnCompanyContainer(file, nameFile, roLiveDatalinkFolders.custom.ToString + "/" + logPath, roLiveQueueTypes.dataLink, True)
            End If
        Catch ex As Exception
            _oLog.logMessage(roLog.EventType.roError, "roSener::UploadErrorFile: " & ex.Message)
        End Try

    End Sub

    Public Function GenerateStreamFromString(ByVal lst As List(Of String)) As Stream
        Dim stream = New MemoryStream()
        Dim writer = New StreamWriter(stream)
        For Each s In lst
            writer.WriteLine(s)
        Next
        writer.Flush()
        stream.Position = 0
        Return stream
    End Function

#End Region

End Class

' ReSharper disable once InconsistentNaming
Public Class roOperation
    Implements IComparable
#Region "Properties"
    Property ProyectCode() As String
    Property TaskCode() As String
    Property TaskDescription() As String
    Property BarCode() As String
    Property GroupsCode() As String
    Property TaskState() As TaskStatusEnum
#End Region

    Public Sub New(operation As String, separator As String)
        Dim operationFiels As String() = operation.Split(separator)
        Try
            ProyectCode = operationFiels(0)
            TaskCode = operationFiels(1)
            TaskDescription = operationFiels(2)
            BarCode = operationFiels(3)
            GroupsCode = operationFiels(4)
        Catch ex As Exception
            Throw New Exception(String.Format("The line for the Operation doesn´t contain the necessary fields: {0}", operation))
        End Try
    End Sub
    Public Sub New()
        ProyectCode = String.Empty
        TaskCode = String.Empty
        TaskDescription = String.Empty
        BarCode = String.Empty
    End Sub

    Public Function CompareTo(obj As Object) As Integer Implements IComparable.CompareTo
        If obj Is Nothing Then Return 1
        Dim compareOpe = TryCast(obj, roOperation)

        Dim result = String.Compare(TaskCode, compareOpe.TaskCode, StringComparison.Ordinal)
        If (result.Equals(0)) Then
            result = String.Compare(TaskDescription, compareOpe.TaskDescription, StringComparison.Ordinal)
        End If
        Return result
    End Function
End Class

' ReSharper disable once InconsistentNaming
Public Class roOrder
    Implements IComparable
#Region "Properties"

    Property ProyectCode() As String
    Property ProyectDescription() As String
    Property PunchType() As String
#End Region

    Public Sub New(order As String, separator As String)
        Dim orderFiels As String() = order.Split(separator)
        If (orderFiels.Count().Equals(3)) Then
            ProyectCode = orderFiels(0)
            ProyectDescription = orderFiels(1)
            PunchType = orderFiels(2)
        Else
            Throw New Exception("The line for the Order doesn´t contain the necessary fields")
        End If
    End Sub

    Public Sub New()
        ProyectDescription = String.Empty
        ProyectCode = String.Empty
    End Sub

    Public Function CompareTo(obj As Object) As Integer Implements IComparable.CompareTo
        If obj Is Nothing Then Return 1
        Dim compareOrder = TryCast(obj, roOrder)
        'Return String.Compare(Me.ProyectCode, compareOrder.ProyectCode, StringComparison.Ordinal)

        Dim result = String.Compare(ProyectCode, compareOrder.ProyectCode, StringComparison.Ordinal)
        If (result.Equals(0)) Then
            result = String.Compare(ProyectDescription, compareOrder.ProyectDescription, StringComparison.Ordinal)
        End If
        Return result
    End Function
End Class

Public Class ExportTasks
    Private _strProjectCode As String
    Private _strTaskCode As String

    Property EmployeeCode() As String
    Property EmployeeId() As Integer
    Property DateIni() As DateTime
    Property HourIni() As DateTime
    Property DateEnd() As DateTime
    Property HourEnd() As DateTime
    Property PunchTypeNav() As String
    Property PunchDate() As DateTime
    Property ShiftDate() As DateTime
    Property TaskId() As Integer
    Property PunchActualType() As PunchTypeEnum
    Property IdPunch() As Integer
    Property IsExported As Boolean
    Property IsActive As Boolean
    Property FullGroupName() As String

    Public Property ProjectCode() As String
        Get
            Dim strValues() = _strProjectCode.Split("/")
            If (strValues.Any()) Then
                Return strValues(0)
            Else
                Return _strProjectCode
            End If
        End Get
        Set(value As String)
            _strProjectCode = value
        End Set
    End Property

    Public Property TaskCode() As String
        Get
            Dim strValues() = _strTaskCode.Split("/")
            If (strValues.Count() > 1) Then
                Return strValues(1)
            Else
                Return _strTaskCode
            End If

        End Get
        Set(value As String)
            _strTaskCode = value
        End Set
    End Property

    Public Sub New()

    End Sub
End Class