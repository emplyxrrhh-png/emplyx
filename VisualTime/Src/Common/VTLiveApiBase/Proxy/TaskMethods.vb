Imports System.Data
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.BusinessCenter
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTBusiness.Task
Imports Robotics.Base.VTCostCenter.CostCenter
Imports Robotics.Base.VTUserFields.UserFields

Public Class TaskMethods
    ''' <summary>
    ''' Obtiene la definición de una tarea por código.
    ''' </summary>
    ''' <param name="intIDTask">Código tarea.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Objeto 'roTask' con la información de la tarea.</returns>
    ''' <remarks></remarks>

    Public Shared Function GetTask(ByVal intIDTask As Integer, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of roTask)
        Dim bState = New roTaskState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of roTask)
        oResult.Value = New roTask(intIDTask, bState, bAudit)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Guarda una Tarea. <br/>
    ''' * Se audita la grabación.
    ''' </summary>
    ''' <param name="oTask">Definición de la tarea.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>True o False</returns>
    ''' <remarks>En caso de retorno a false: a través del parámetro de retorno 'oState' se indica el motivo.</remarks>

    Public Shared Function SaveTask(ByVal oTask As roTask, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of roTask)

        oTask.State = New roTaskState(-1)
        roWsStateManager.CopyTo(oState, oTask.State)
        oTask.State.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of roTask)
        If oTask.Save(bAudit) Then
            oResult.Value = oTask
        Else
            oResult.Value = Nothing
        End If

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oTask.State, newGState)
        oResult.Status = newGState
        Return oResult

    End Function

    ''' <summary>
    ''' Elimina una tarea.<br/>
    ''' * Se audita la acción.
    ''' </summary>
    ''' <param name="oTask">Objeto 'roTask' con la definición de la tarea a eliminar.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>True o False</returns>
    ''' <remarks>En caso de retorno a false: a través del parámetro de retorno 'oState' se indica el motivo.</remarks>

    Public Shared Function DeleteTask(ByVal oTask As roTask, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean)

        Dim bState = New roTaskState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)
        oTask.State = bState
        oResult.Value = oTask.Delete(bAudit)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oTask.State, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Elimina una tarea.<br/>
    ''' * Se audita la acción.
    ''' </summary>
    ''' <param name="intIDTask">Código que identifica la tarea.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>True o False</returns>
    ''' <remarks>En caso de retorno a false: a través del parámetro de retorno 'oState' se indica el motivo.</remarks>

    Public Shared Function DeleteTaskByID(ByVal intIDTask As Integer, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean)

        Dim bState = New roTaskState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)

        Dim oTask As New roTask(intIDTask, bState)
        oResult.Value = oTask.Delete(bAudit)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oTask.State, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Obtiene la lista de tareas que cumplen el filtro por nombre.
    ''' </summary>
    ''' <param name="strLikeName">Filtro que se aplica al nombre de la tarea.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Tabla con las columnas: ID, Name  </returns>
    ''' <remarks></remarks>

    Public Shared Function GetTasksByName(ByVal strLikeName As String, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet)

        Dim bState = New roTaskState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim ds As DataSet = Nothing
        Dim tb As DataTable = roTask.GetTasksByName(strLikeName, bState)
        If tb IsNot Nothing Then
            If tb.DataSet IsNot Nothing Then
                ds = tb.DataSet
            Else
                ds = New DataSet
                ds.Tables.Add(tb)
            End If
        End If

        oResult.Value = ds

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Obtiene la lista de tareas que cumplen el filtro por nombre.
    ''' </summary>
    ''' <param name="strLikeName">Filtro que se aplica al nombre de la tarea.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Tabla con las columnas: ID, Name  </returns>
    ''' <remarks></remarks>

    Public Shared Function GetTasksByProjectName(ByVal strLikeName As String, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet)

        Dim bState = New roTaskState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim ds As DataSet = Nothing
        Dim tb As DataTable = roTask.GetTasksByProject(strLikeName, bState)
        If tb IsNot Nothing Then
            If tb.DataSet IsNot Nothing Then
                ds = tb.DataSet
            Else
                ds = New DataSet
                ds.Tables.Add(tb)
            End If
        End If

        oResult.Value = ds

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Obtiene la lista de proyectos que cumplen el filtro por nombre.
    ''' </summary>
    ''' <param name="strLikeName">Filtro que se aplica al nombre del proyecto.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Tabla con las columnas: Name  </returns>
    ''' <remarks></remarks>

    Public Shared Function GetProjectsByName(ByVal strLikeName As String, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet)

        Dim bState = New roTaskState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim ds As DataSet = Nothing
        Dim tb As DataTable = roTask.GetProjectsByName(strLikeName, bState)
        If tb IsNot Nothing Then
            If tb.DataSet IsNot Nothing Then
                ds = tb.DataSet
            Else
                ds = New DataSet
                ds.Tables.Add(tb)
            End If
        End If

        oResult.Value = ds

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Obtiene la lista de tareas recientes de un empleado.
    ''' </summary>
    ''' <param name="intIDEmployee">ID del empleado.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Tabla con las columnas: ID, Name  </returns>
    ''' <remarks></remarks>

    Public Shared Function GetLastTasksByEmployee(ByVal intIDEmployee As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet)
        Dim bState = New roTaskState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim ds As DataSet = Nothing
        Dim tb As DataTable = roTask.GetLastTasksByEmployee(intIDEmployee, bState)
        If tb IsNot Nothing Then
            If tb.DataSet IsNot Nothing Then
                ds = tb.DataSet
            Else
                ds = New DataSet
                ds.Tables.Add(tb)
            End If
        End If

        oResult.Value = ds

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Obtiene la última tarea de un empleado
    ''' </summary>
    ''' <param name="intIDEmployee">ID del empleado.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns> roTask </returns>
    ''' <remarks></remarks>

    Public Shared Function GetLastTaskByEmployee(ByVal intIDEmployee As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of roTask)

        Dim bState = New roTaskState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of roTask)
        oResult.Value = roTask.GetLastTaskByEmployee(intIDEmployee, bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Obtiene estadísticas de la tarea indicada.
    ''' </summary>
    ''' <param name="intIDTask">ID de la Tarea.</param>
    ''' <param name="ViewType">Tipo de Vista.</param>
    ''' <param name="GroupBy">Tipo de agrupacion.</param>
    ''' <param name="xBegin">Inicio de periodo.</param>
    ''' <param name="xEnd">Fin de periodo.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Tabla con las columnas: Concept, Total  </returns>
    ''' <remarks></remarks>

    Public Shared Function GetStatistics(ByVal intIDTask As Integer, ByVal ViewType As TaskStatisticsViewEnum, ByVal GroupBy As TaskStatisticsGroupByEnum, ByVal xBegin As Date, ByVal xEnd As Date, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet)

        Dim bState = New roTaskState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim ds As DataSet = Nothing
        Dim tb As DataTable = roTask.GetStatistics(intIDTask, ViewType, GroupBy, bState, xBegin, xEnd)
        If tb IsNot Nothing Then
            If tb.DataSet IsNot Nothing Then
                ds = tb.DataSet
            Else
                ds = New DataSet
                ds.Tables.Add(tb)
            End If
        End If

        oResult.Value = ds

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Crea una copia de una tarea existente.
    ''' </summary>
    ''' <param name="_IDSourceTask">Código de la tarea del que se quiere realizar la copia</param>
    ''' <param name="_NewName">Nombre de la nueva tarea creada. Si no se informa, se utiliza el tag de idioma 'Tasks.TaskSave.Copy' para generar el nuevo nombre (copia de ...).</param>
    ''' <param name="oState"></param>
    ''' <returns>ID Nueva tarea creada</returns>
    ''' <remarks></remarks>

    Public Shared Function CopyTask(ByVal _IDSourceTask As Integer, ByVal _NewName As String, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Integer)

        Dim bState = New roTaskState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Integer)

        oResult.Value = roTask.CopyTask(_IDSourceTask, _NewName, bState, bAudit)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Obtiene el progreso de la tarea.
    ''' </summary>
    ''' <param name="_IDTask">Código de la tarea del que se quiere realizar la copia</param>
    ''' <param name="oState"></param>
    ''' <returns>Obtiene el progreso de la tarea</returns>
    ''' <remarks></remarks>

    Public Shared Function GetStatusTask(ByVal _IDTask As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of Double)

        Dim bState = New roTaskState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Double)

        oResult.Value = roTask.GetStatusTask(_IDTask, bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Obtiene el progreso de la tarea.
    ''' </summary>
    ''' <param name="_IDTask">Código de la tarea del que se quiere realizar la copia</param>
    ''' <param name="oState"></param>
    ''' <returns>Obtiene el progreso de la tarea</returns>
    ''' <remarks></remarks>

    Public Shared Function GetTaskWorkedTime(ByVal _IDTask As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of Double)

        Dim bState = New roTaskState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Double)

        oResult.Value = roTask.GetWorkedTimeInTask(_IDTask, bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Obtiene el nombre de una tarea.
    ''' </summary>
    ''' <param name="_IDTask">Código de la tarea</param>
    ''' <param name="oState"></param>
    ''' <returns>Nombre de la tarea correspondiente con el ID indicado</returns>
    ''' <remarks></remarks>

    Public Shared Function GetTaskName(ByVal _IDTask As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of String)

        Dim bState = New roTaskState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of String)

        oResult.Value = roTask.GetTaskName(_IDTask, bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Obtiene el nº de empleados asignados a una tarea o el total de empleados en funcion de empleados y grupos seleccionados
    ''' </summary>
    ''' <param name="intIDTask">ID de la Tarea.</param>
    ''' <param name="sEmployees">Contiene empleados seleccionados</param>
    ''' <param name="sGroups">Contiene grupos seleccionados</param>
    ''' <returns>Nº de empleados asignados o seleccionados</returns>
    ''' <remarks></remarks>

    Public Shared Function GetEmployeesFromTask(ByVal intIDTask As Integer, ByVal sEmployees As String, ByVal sGroups As String) As roGenericVtResponse(Of Double)

        Dim bState = New roTaskState(-1)
        Dim oResult As New roGenericVtResponse(Of Double)

        oResult.Value = roTask.GetEmployeesFromTask(intIDTask, sEmployees, sGroups)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Obtiene el nº de empleados trabajando en una tarea
    ''' </summary>
    ''' <param name="intIDTask">ID de la Tarea.</param>
    ''' <returns>Nº de empleados trabajando en una tarea</returns>
    ''' <remarks></remarks>

    Public Shared Function GetEmployeesWorkingInTask(ByVal intIDTask As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of Double)

        Dim bState = New roTaskState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Double)

        oResult.Value = roTask.GetEmployeesWorkingInTask(intIDTask, bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Obtiene el listado de empleados que estan trabajando en una tarea
    ''' </summary>
    ''' <param name="intIDTask">ID de la Tarea.</param>
    ''' <returns>Listado de empleados que estan trabajando en una tarea </returns>
    ''' <remarks></remarks>

    Public Shared Function GetEmployeesWorkingInTaskList(ByVal intIDTask As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet)

        Dim bState = New roTaskState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim ds As DataSet = Nothing
        Dim tb As DataTable = roTask.GetEmployeesWorkingInTaskList(intIDTask, bState)
        If tb IsNot Nothing Then
            If tb.DataSet IsNot Nothing Then
                ds = tb.DataSet
            Else
                ds = New DataSet
                ds.Tables.Add(tb)
            End If
        End If

        oResult.Value = ds

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult

    End Function

    ''' <summary>
    ''' Obtiene el listado de empleados que han trabajando en una tarea en los 2 ultimos dias
    ''' </summary>
    ''' <param name="intIDTask">ID de la Tarea.</param>
    ''' <returns>Listado de empleados que han trabajando en una tarea en los 2 ultimos dias</returns>
    ''' <remarks></remarks>

    Public Shared Function GetEmployeesWorkedInTaskList(ByVal intIDTask As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet)

        Dim bState = New roTaskState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim ds As DataSet = Nothing
        Dim tb As DataTable = roTask.GetEmployeesWorkedInTaskList(intIDTask, bState)
        If tb IsNot Nothing Then
            If tb.DataSet IsNot Nothing Then
                ds = tb.DataSet
            Else
                ds = New DataSet
                ds.Tables.Add(tb)
            End If
        End If

        oResult.Value = ds

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Obtiene los empleados trabajando en una tarea
    ''' </summary>
    ''' <param name="intIDTask">ID de la Tarea.</param>
    ''' <returns>Datatable con los usuarios trabajando en una tarea</returns>
    ''' <remarks></remarks>

    Public Shared Function GetEmployeesWorkingInTaskDatatable(ByVal intIDTask As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet)

        Dim bState = New roTaskState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim ds As DataSet = Nothing
        Dim tb As DataTable = roTask.GetEmployeesWorkingInTaskDatatable(intIDTask, bState)
        If tb IsNot Nothing Then
            If tb.DataSet IsNot Nothing Then
                ds = tb.DataSet
            Else
                ds = New DataSet
                ds.Tables.Add(tb)
            End If
        End If

        oResult.Value = ds

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    '===================== BusinessCenters

    Public Shared Function GetBusinessCenters(ByVal oState As roWsState, ByVal bolCheckStatus As Boolean) As roGenericVtResponse(Of DataSet)

        Dim bState = New roBusinessCenterState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim ds As DataSet = Nothing
        Dim tb As DataTable = roBusinessCenter.GetBusinessCenters(bState, bolCheckStatus)
        If tb IsNot Nothing Then
            If tb.DataSet IsNot Nothing Then
                ds = tb.DataSet
            Else
                ds = New DataSet
                ds.Tables.Add(tb)
            End If
        End If

        oResult.Value = ds

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function GetBusinessCentersByFilter(ByVal oState As roWsState, strFilter As String) As roGenericVtResponse(Of DataSet)

        Dim bState = New roBusinessCenterState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim ds As DataSet = Nothing
        Dim tb As DataTable = roBusinessCenter.GetBusinessCentersByFilter(bState, strFilter)
        If tb IsNot Nothing Then
            If tb.DataSet IsNot Nothing Then
                ds = tb.DataSet
            Else
                ds = New DataSet
                ds.Tables.Add(tb)
            End If
        End If

        oResult.Value = ds

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function GetBusinessCenterZones(ByVal IDCenter As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet)

        Dim bState = New roBusinessCenterState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim ds As DataSet = Nothing
        Dim tb As DataTable = roBusinessCenterZone.GetBusinessCenterZones(IDCenter, bState)
        If tb IsNot Nothing Then
            If tb.DataSet IsNot Nothing Then
                ds = tb.DataSet
            Else
                ds = New DataSet
                ds.Tables.Add(tb)
            End If
        End If

        oResult.Value = ds

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function GetBusinessCenterByPassportDataTable(ByVal oState As roWsState, ByVal intIDPassport As Integer, ByVal bolCheckStatus As Boolean) As roGenericVtResponse(Of DataSet)

        Dim bState = New roBusinessCenterState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim ds As DataSet = Nothing
        Dim tb As DataTable = roBusinessCenter.GetBusinessCenterByPassportDataTable(bState, intIDPassport, bolCheckStatus)
        If tb IsNot Nothing Then
            If tb.DataSet IsNot Nothing Then
                ds = tb.DataSet
            Else
                ds = New DataSet
                ds.Tables.Add(tb)
            End If
        End If

        oResult.Value = ds

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function GetBusinessCenterBySecurityGroupDataTable(ByVal oState As roWsState, ByVal intIDSecurityGroup As Integer, ByVal bolCheckStatus As Boolean) As roGenericVtResponse(Of DataSet)

        Dim bState = New roCostCenterManagerState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim ds As DataSet = Nothing
        Dim tb As DataTable = roCostCenterManager.GetBusinessCenterBySecurityGroupDataTable(bState, intIDSecurityGroup, bolCheckStatus)
        If tb IsNot Nothing Then
            If tb.DataSet IsNot Nothing Then
                ds = tb.DataSet
            Else
                ds = New DataSet
                ds.Tables.Add(tb)
            End If
        End If

        oResult.Value = ds

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function GetEmployeeBusinessCentersDataTable(ByVal oState As roWsState, ByVal intIDEmployee As Integer, ByVal bolDefaultCenter As Boolean, ByVal bolOnlyActiveCenters As Boolean) As roGenericVtResponse(Of DataSet)

        Dim bState = New roBusinessCenterState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim ds As DataSet = Nothing
        Dim tb As DataTable = roBusinessCenter.GetEmployeeBusinessCentersDataTable(bState, intIDEmployee, bolDefaultCenter, bolOnlyActiveCenters)
        If tb IsNot Nothing Then
            If tb.DataSet IsNot Nothing Then
                ds = tb.DataSet
            Else
                ds = New DataSet
                ds.Tables.Add(tb)
            End If
        End If

        oResult.Value = ds

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function GetEmployeeDefaultBusinessCenter(ByVal oState As roWsState, ByVal intIDEmployee As Integer, ByVal xDate As Date) As roGenericVtResponse(Of Integer)

        Dim bState = New roBusinessCenterState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Integer)

        oResult.Value = roBusinessCenter.GetEmployeeDefaultBusinessCenter(bState, intIDEmployee, xDate)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function SavEmployeeBusinessCenters(ByVal IDEmployee As Integer, ByVal dsCenters As DataSet, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)

        Dim bState = New roBusinessCenterState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)

        oResult.Value = roBusinessCenter.SaveEmployeeCenters(IDEmployee, dsCenters, bState, True)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function GetBusinessCenterByPassport(ByVal oState As roWsState, ByVal intIDPassport As Integer, ByVal bolCheckStatus As Boolean, ByVal bAudit As Boolean) As roGenericVtResponse(Of Integer())

        Dim bState = New roBusinessCenterState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Integer())

        oResult.Value = roBusinessCenter.GetBusinessCenterByPassport(bState, intIDPassport, bolCheckStatus)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function GetBusinessCenterByID(ByVal intIDBusinessCenter As Integer, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of roBusinessCenter)

        Dim bState = New roBusinessCenterState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of roBusinessCenter)
        oResult.Value = New roBusinessCenter(intIDBusinessCenter, bState, bAudit)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult

    End Function

    ''' <summary>
    ''' Guarda un roBusinessCenter. <br/>
    ''' * Se audita la grabación.
    ''' </summary>
    ''' <param name="oBusinessCenter">Definición del BusinessCenter.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>True o False</returns>
    ''' <remarks>En caso de retorno a false: a través del parámetro de retorno 'oState' se indica el motivo.</remarks>

    Public Shared Function SaveBusinessCenter(ByVal oBusinessCenter As roBusinessCenter, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of roBusinessCenter)

        oBusinessCenter.State = New roBusinessCenterState(-1)
        roWsStateManager.CopyTo(oState, oBusinessCenter.State)
        oBusinessCenter.State.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of roBusinessCenter)
        If oBusinessCenter.Save(bAudit) Then
            oResult.Value = oBusinessCenter
        Else
            oResult.Value = Nothing
        End If

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oBusinessCenter.State, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Guarda los centros de coste de un passport concreto. <br/>
    ''' </summary>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <param name="intIDPassport">Passport al que se le asignan los centros de coste.</param>
    ''' <param name="intIDBusinessCenters">Centros de coste asignados al passport.</param>
    ''' <returns>True o False</returns>
    ''' <remarks>En caso de retorno a false: a través del parámetro de retorno 'oState' se indica el motivo.</remarks>

    Public Shared Function SaveBusinessCenterByPassport(ByVal oState As roWsState, ByVal intIDPassport As Integer, ByVal intIDBusinessCenters() As Integer, ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean)

        Dim bState = New roBusinessCenterState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)

        oResult.Value = roBusinessCenter.SaveBusinessCenterByPassport(bState, intIDPassport, intIDBusinessCenters, bAudit)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Guarda los centros de coste de un passport concreto. <br/>
    ''' </summary>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <param name="intIDSecurityGroup">Group de funciones al que se le asignan los centros de coste.</param>
    ''' <param name="intIDBusinessCenters">Centros de coste asignados al passport.</param>
    ''' <returns>True o False</returns>
    ''' <remarks>En caso de retorno a false: a través del parámetro de retorno 'oState' se indica el motivo.</remarks>

    Public Shared Function SaveBusinessCenterBySecurityGroup(ByVal oState As roWsState, ByVal intIDSecurityGroup As Integer, ByVal intIDBusinessCenters() As Integer, ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean)

        Dim bState = New roCostCenterManagerState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)

        oResult.Value = roCostCenterManager.SaveBusinessCenterBySecurityGroup(bState, intIDSecurityGroup, intIDBusinessCenters, bAudit)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Elimina un roBusinessCenter.<br/>
    ''' * Se audita la acción.
    ''' </summary>
    ''' <param name="oBusinessCenter">Objeto 'roBusinessCenter' con la definición del roBusinessCenter a eliminar.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>True o False</returns>
    ''' <remarks>En caso de retorno a false: a través del parámetro de retorno 'oState' se indica el motivo.</remarks>

    Public Shared Function DeleteBusinessCenter(ByVal oBusinessCenter As roBusinessCenter, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean)

        Dim bState = New roBusinessCenterState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)
        oBusinessCenter.State = bState
        oResult.Value = oBusinessCenter.Delete(bAudit)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oBusinessCenter.State, newGState)
        oResult.Status = newGState
        Return oResult

    End Function

    ''' <summary>
    ''' Elimina una tarea.<br/>
    ''' * Se audita la acción.
    ''' </summary>
    ''' <param name="intIDBusinessCenter">Código que identifica el roBusinessCenter.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>True o False</returns>
    ''' <remarks>En caso de retorno a false: a través del parámetro de retorno 'oState' se indica el motivo.</remarks>

    Public Shared Function DeleteBusinessCenterByID(ByVal intIDBusinessCenter As Integer, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean)

        Dim bState = New roBusinessCenterState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)

        Dim oBusinessCenter As New roBusinessCenter(intIDBusinessCenter, bState)
        oResult.Value = oBusinessCenter.Delete(bAudit)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oBusinessCenter.State, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    '=========================================
    '============= CUBO ======================
    '=========================================

    Public Shared Function GetTaskFieldsList(ByVal intID As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of Generic.List(Of roTaskField))

        Dim bState = New roTaskFieldState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Generic.List(Of roTaskField))
        oResult.Value = roTaskField.GetTaskFieldsList(intID, bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function GetTaskFieldsDataTable(ByVal intID As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet)

        Dim bState = New roTaskFieldState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim ds As DataSet = Nothing
        Dim tb As DataTable = roTaskField.GetTaskFieldsDataTable(intID, bState)
        If tb IsNot Nothing Then
            If tb.DataSet IsNot Nothing Then
                ds = tb.DataSet
            Else
                ds = New DataSet
                ds.Tables.Add(tb)
            End If
        End If

        oResult.Value = ds

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function GetTaskAlertsDataTable(ByVal intID As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet)

        Dim bState = New roTaskState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim ds As DataSet = Nothing
        Dim tb As DataTable = roTaskAlert.GetTaskAlertsDataTable(intID, bState)
        If tb IsNot Nothing Then
            If tb.DataSet IsNot Nothing Then
                ds = tb.DataSet
            Else
                ds = New DataSet
                ds.Tables.Add(tb)
            End If
        End If

        oResult.Value = ds

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function GetTaskAlertDataTable(ByVal intID As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet)

        Dim bState = New roTaskState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim ds As DataSet = Nothing
        Dim tb As DataTable = roTaskAlert.GetTaskAlertDataTable(intID, bState)
        If tb IsNot Nothing Then
            If tb.DataSet IsNot Nothing Then
                ds = tb.DataSet
            Else
                ds = New DataSet
                ds.Tables.Add(tb)
            End If
        End If

        oResult.Value = ds

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function SaveTaskFields(ByVal intID As Integer, ByVal _TaskFields As Generic.List(Of roTaskField), ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)

        Dim bState = New roTaskFieldState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)

        oResult.Value = roTaskField.SaveTaskFields(intID, _TaskFields, bState, True)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function SaveTaskAlerts(ByVal intID As Integer, ByVal _TaskAlerts As Generic.List(Of roTaskAlert), ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)

        Dim bState = New roTaskState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)

        oResult.Value = roTaskAlert.SaveTaskAlerts(intID, _TaskAlerts, bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function GetAntTaskPunchDate(ByVal _IDTask As Integer, ByVal _IDEmployee As Integer, ByVal _Date As DateTime, ByVal oState As roWsState) As roGenericVtResponse(Of DateTime)

        Dim bState = New roTaskState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DateTime)

        oResult.Value = roTask.GetAntTaskPunchDate(bState, _IDTask, _IDEmployee, _Date)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function GetNextTaskPunchDate(ByVal _IDTask As Integer, ByVal _IDEmployee As Integer, ByVal _Date As DateTime, ByVal oState As roWsState) As roGenericVtResponse(Of DateTime)

        Dim bState = New roTaskState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DateTime)

        oResult.Value = roTask.GetNextTaskPunchDate(bState, _IDTask, _IDEmployee, _Date)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function SaveTaskFieldsFromPunch(ByVal _IDTask As Integer, ByVal Field1 As String, ByVal Field2 As String, ByVal Field3 As String, ByVal Field4 As Double, ByVal Field5 As Double, ByVal Field6 As Double, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)

        Dim bState = New roTaskState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)

        oResult.Value = roTask.SaveTaskFieldsFromPunch(_IDTask, Field1, Field2, Field3, Field4, Field5, Field6, bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function SaveTaskAlertsFromPunch(ByVal _IDTask As Integer, ByVal _IDEmployee As Integer, ByVal _DateTime As Date, ByVal _Comment As String, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)

        Dim bState = New roTaskState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)

        oResult.Value = roTaskAlert.SaveTaskAlertsFromPunch(_IDTask, _IDEmployee, _DateTime, _Comment, bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

#Region "Assignments"

    ''' <summary>
    ''' Obtiene los puestos que tiene asignada la tarea
    ''' </summary>
    ''' <param name="intIDTask">Código de la tarea.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Tabla con las columnas: IDAssignment </returns>
    ''' <remarks></remarks>

    Public Shared Function GetTaskAssignments(ByVal intIDTask As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of Generic.List(Of roTaskAssignment))

        Dim bState = New roTaskState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Generic.List(Of roTaskAssignment))
        oResult.Value = roTaskAssignment.GetTaskAssignments(intIDTask, bState, False)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Obtiene los puestos que tiene asignada la tarea en un dataset.
    ''' </summary>
    ''' <param name="intIDTask">Código de la tarea.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Tabla con las columnas: IDAssignment </returns>
    ''' <remarks></remarks>

    Public Shared Function GetTaskAssignmentsDatatable(ByVal intIDTask As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet)

        Dim bState = New roTaskState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim ds As DataSet = Nothing
        Dim tb As DataTable = roTaskAssignment.GetTaskAssignmentsDataTable(intIDTask, bState, False)
        If tb IsNot Nothing Then
            If tb.DataSet IsNot Nothing Then
                ds = tb.DataSet
            Else
                ds = New DataSet
                ds.Tables.Add(tb)
            End If
        End If

        oResult.Value = ds

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Guardamos los puestos asignados a la tarea
    ''' </summary>
    ''' <param name="intIDTask">Código de la tarea.</param>
    ''' <param name="lstTaskAssignments">Lista de puestos asignados .</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Tabla con las columnas: IDAssignment </returns>
    ''' <remarks></remarks>

    Public Shared Function SaveTaskAssignments(ByVal intIDTask As Integer, ByVal lstTaskAssignments As Generic.List(Of roTaskAssignment), ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)

        Dim bState = New roTaskState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)

        oResult.Value = roTaskAssignment.SaveTaskAssignments(intIDTask, lstTaskAssignments, bState, True)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

#End Region

End Class