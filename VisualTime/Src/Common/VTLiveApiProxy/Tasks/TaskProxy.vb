Imports Robotics.Base
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.BusinessCenter
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTBusiness.Task
Imports Robotics.Base.VTUserFields.UserFields

Public Class TaskProxy
    Implements ITaskSvc

    Public Function KeepAlive() As Boolean Implements ITaskSvc.KeepAlive
        Return True
    End Function
    ''' <summary>
    ''' Obtiene la definición de una tarea por código.
    ''' </summary>
    ''' <param name="intIDTask">Código tarea.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Objeto 'roTask' con la información de la tarea.</returns>
    ''' <remarks></remarks>

    Public Function GetTask(ByVal intIDTask As Integer, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of roTask) Implements ITaskSvc.GetTask
        Return TaskMethods.GetTask(intIDTask, oState, bAudit)
    End Function

    ''' <summary>
    ''' Guarda una Tarea. <br/>
    ''' * Se audita la grabación.
    ''' </summary>
    ''' <param name="oTask">Definición de la tarea.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>True o False</returns>
    ''' <remarks>En caso de retorno a false: a través del parámetro de retorno 'oState' se indica el motivo.</remarks>

    Public Function SaveTask(ByVal oTask As roTask, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of roTask) Implements ITaskSvc.SaveTask

        Return TaskMethods.SaveTask(oTask, oState, bAudit)
    End Function


    ''' <summary>
    ''' Elimina una tarea.<br/>
    ''' * Se audita la acción.
    ''' </summary>
    ''' <param name="oTask">Objeto 'roTask' con la definición de la tarea a eliminar.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>True o False</returns>
    ''' <remarks>En caso de retorno a false: a través del parámetro de retorno 'oState' se indica el motivo.</remarks>

    Public Function DeleteTask(ByVal oTask As roTask, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean) Implements ITaskSvc.DeleteTask
        Return TaskMethods.DeleteTask(oTask, oState, bAudit)
    End Function

    ''' <summary>
    ''' Elimina una tarea.<br/>
    ''' * Se audita la acción.
    ''' </summary>
    ''' <param name="intIDTask">Código que identifica la tarea.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>True o False</returns>
    ''' <remarks>En caso de retorno a false: a través del parámetro de retorno 'oState' se indica el motivo.</remarks>


    Public Function DeleteTaskByID(ByVal intIDTask As Integer, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean) Implements ITaskSvc.DeleteTaskByID

        Return TaskMethods.DeleteTaskByID(intIDTask, oState, bAudit)
    End Function

    ''' <summary>
    ''' Obtiene la lista de tareas que cumplen el filtro por nombre.
    ''' </summary>
    ''' <param name="strLikeName">Filtro que se aplica al nombre de la tarea.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Tabla con las columnas: ID, Name  </returns>
    ''' <remarks></remarks>

    Public Function GetTasksByName(ByVal strLikeName As String, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet) Implements ITaskSvc.GetTasksByName
        Return TaskMethods.GetTasksByName(strLikeName, oState)
    End Function

    ''' <summary>
    ''' Obtiene la lista de tareas que cumplen el filtro por nombre.
    ''' </summary>
    ''' <param name="strLikeName">Filtro que se aplica al nombre de la tarea.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Tabla con las columnas: ID, Name  </returns>
    ''' <remarks></remarks>

    Public Function GetTasksByProjectName(ByVal strLikeName As String, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet) Implements ITaskSvc.GetTasksByProjectName

        Return TaskMethods.GetTasksByProjectName(strLikeName, oState)
    End Function

    ''' <summary>
    ''' Obtiene la lista de proyectos que cumplen el filtro por nombre.
    ''' </summary>
    ''' <param name="strLikeName">Filtro que se aplica al nombre del proyecto.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Tabla con las columnas: Name  </returns>
    ''' <remarks></remarks>

    Public Function GetProjectsByName(ByVal strLikeName As String, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet) Implements ITaskSvc.GetProjectsByName

        Return TaskMethods.GetProjectsByName(strLikeName, oState)
    End Function

    ''' <summary>
    ''' Obtiene la lista de tareas recientes de un empleado.
    ''' </summary>
    ''' <param name="intIDEmployee">ID del empleado.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Tabla con las columnas: ID, Name  </returns>
    ''' <remarks></remarks>

    Public Function GetLastTasksByEmployee(ByVal intIDEmployee As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet) Implements ITaskSvc.GetLastTasksByEmployee
        Return TaskMethods.GetLastTasksByEmployee(intIDEmployee, oState)
    End Function

    ''' <summary>
    ''' Obtiene la última tarea de un empleado
    ''' </summary>
    ''' <param name="intIDEmployee">ID del empleado.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns> roTask </returns>
    ''' <remarks></remarks>

    Public Function GetLastTaskByEmployee(ByVal intIDEmployee As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of roTask) Implements ITaskSvc.GetLastTaskByEmployee

        Return TaskMethods.GetLastTaskByEmployee(intIDEmployee, oState)
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

    Public Function GetStatistics(ByVal intIDTask As Integer, ByVal ViewType As TaskStatisticsViewEnum, ByVal GroupBy As TaskStatisticsGroupByEnum, ByVal xBegin As Date, ByVal xEnd As Date, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet) Implements ITaskSvc.GetStatistics

        Return TaskMethods.GetStatistics(intIDTask, ViewType, GroupBy, xBegin, xEnd, oState)
    End Function

    ''' <summary>
    ''' Crea una copia de una tarea existente.
    ''' </summary>
    ''' <param name="_IDSourceTask">Código de la tarea del que se quiere realizar la copia</param>
    ''' <param name="_NewName">Nombre de la nueva tarea creada. Si no se informa, se utiliza el tag de idioma 'Tasks.TaskSave.Copy' para generar el nuevo nombre (copia de ...).</param>
    ''' <param name="oState"></param>
    ''' <returns>ID Nueva tarea creada</returns>
    ''' <remarks></remarks>

    Public Function CopyTask(ByVal _IDSourceTask As Integer, ByVal _NewName As String, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Integer) Implements ITaskSvc.CopyTask

        Return TaskMethods.CopyTask(_IDSourceTask, _NewName, oState, bAudit)
    End Function
    ''' <summary>
    ''' Obtiene el progreso de la tarea.
    ''' </summary>
    ''' <param name="_IDTask">Código de la tarea del que se quiere realizar la copia</param>
    ''' <param name="oState"></param>
    ''' <returns>Obtiene el progreso de la tarea</returns>
    ''' <remarks></remarks>

    Public Function GetStatusTask(ByVal _IDTask As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of Double) Implements ITaskSvc.GetStatusTask

        Return TaskMethods.GetStatusTask(_IDTask, oState)
    End Function

    ''' <summary>
    ''' Obtiene el progreso de la tarea.
    ''' </summary>
    ''' <param name="_IDTask">Código de la tarea del que se quiere realizar la copia</param>
    ''' <param name="oState"></param>
    ''' <returns>Obtiene el progreso de la tarea</returns>
    ''' <remarks></remarks>

    Public Function GetTaskWorkedTime(ByVal _IDTask As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of Double) Implements ITaskSvc.GetTaskWorkedTime

        Return TaskMethods.GetTaskWorkedTime(_IDTask, oState)
    End Function

    ''' <summary>
    ''' Obtiene el nombre de una tarea.
    ''' </summary>
    ''' <param name="_IDTask">Código de la tarea</param>
    ''' <param name="oState"></param>
    ''' <returns>Nombre de la tarea correspondiente con el ID indicado</returns>
    ''' <remarks></remarks>

    Public Function GetTaskName(ByVal _IDTask As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of String) Implements ITaskSvc.GetTaskName

        Return TaskMethods.GetTaskName(_IDTask, oState)
    End Function

    ''' <summary>
    ''' Obtiene el nº de empleados asignados a una tarea o el total de empleados en funcion de empleados y grupos seleccionados
    ''' </summary>
    ''' <param name="intIDTask">ID de la Tarea.</param>
    ''' <param name="sEmployees">Contiene empleados seleccionados</param>
    ''' <param name="sGroups">Contiene grupos seleccionados</param>
    ''' <returns>Nº de empleados asignados o seleccionados</returns>
    ''' <remarks></remarks>

    Public Function GetEmployeesFromTask(ByVal intIDTask As Integer, ByVal sEmployees As String, ByVal sGroups As String) As roGenericVtResponse(Of Double) Implements ITaskSvc.GetEmployeesFromTask
        Return TaskMethods.GetEmployeesFromTask(intIDTask, sEmployees, sGroups)
    End Function

    ''' <summary>
    ''' Obtiene el nº de empleados trabajando en una tarea 
    ''' </summary>
    ''' <param name="intIDTask">ID de la Tarea.</param>
    ''' <returns>Nº de empleados trabajando en una tarea</returns>
    ''' <remarks></remarks>

    Public Function GetEmployeesWorkingInTask(ByVal intIDTask As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of Double) Implements ITaskSvc.GetEmployeesWorkingInTask

        Return TaskMethods.GetEmployeesWorkingInTask(intIDTask, oState)
    End Function

    ''' <summary>
    ''' Obtiene el listado de empleados que estan trabajando en una tarea
    ''' </summary>
    ''' <param name="intIDTask">ID de la Tarea.</param>
    ''' <returns>Listado de empleados que estan trabajando en una tarea </returns>
    ''' <remarks></remarks>

    Public Function GetEmployeesWorkingInTaskList(ByVal intIDTask As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet) Implements ITaskSvc.GetEmployeesWorkingInTaskList

        Return TaskMethods.GetEmployeesWorkingInTaskList(intIDTask, oState)
    End Function

    ''' <summary>
    ''' Obtiene el listado de empleados que han trabajando en una tarea en los 2 ultimos dias
    ''' </summary>
    ''' <param name="intIDTask">ID de la Tarea.</param>
    ''' <returns>Listado de empleados que han trabajando en una tarea en los 2 ultimos dias</returns>
    ''' <remarks></remarks>

    Public Function GetEmployeesWorkedInTaskList(ByVal intIDTask As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet) Implements ITaskSvc.GetEmployeesWorkedInTaskList

        Return TaskMethods.GetEmployeesWorkedInTaskList(intIDTask, oState)
    End Function

    ''' <summary>
    ''' Obtiene los empleados trabajando en una tarea 
    ''' </summary>
    ''' <param name="intIDTask">ID de la Tarea.</param>
    ''' <returns>Datatable con los usuarios trabajando en una tarea</returns>
    ''' <remarks></remarks>

    Public Function GetEmployeesWorkingInTaskDatatable(ByVal intIDTask As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet) Implements ITaskSvc.GetEmployeesWorkingInTaskDatatable

        Return TaskMethods.GetEmployeesWorkingInTaskDatatable(intIDTask, oState)
    End Function

    '===================== BusinessCenters

    Public Function GetBusinessCenters(ByVal oState As roWsState, ByVal bolCheckStatus As Boolean) As roGenericVtResponse(Of DataSet) Implements ITaskSvc.GetBusinessCenters

        Return TaskMethods.GetBusinessCenters(oState, bolCheckStatus)

    End Function


    Public Function GetBusinessCentersByFilter(ByVal oState As roWsState, strFilter As String) As roGenericVtResponse(Of DataSet) Implements ITaskSvc.GetBusinessCentersByFilter

        Return TaskMethods.GetBusinessCentersByFilter(oState, strFilter)
    End Function


    Public Function GetBusinessCenterZones(ByVal IDCenter As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet) Implements ITaskSvc.GetBusinessCenterZones

        Return TaskMethods.GetBusinessCenterZones(IDCenter, oState)
    End Function



    Public Function GetBusinessCenterByPassportDataTable(ByVal oState As roWsState, ByVal intIDPassport As Integer, ByVal bolCheckStatus As Boolean) As roGenericVtResponse(Of DataSet) Implements ITaskSvc.GetBusinessCenterByPassportDataTable

        Return TaskMethods.GetBusinessCenterByPassportDataTable(oState, intIDPassport, bolCheckStatus)

    End Function


    Public Function GetBusinessCenterBySecurityGroupDataTable(ByVal oState As roWsState, ByVal intIDSecurityGroup As Integer, ByVal bolCheckStatus As Boolean) As roGenericVtResponse(Of DataSet) Implements ITaskSvc.GetBusinessCenterBySecurityGroupDataTable

        Return TaskMethods.GetBusinessCenterBySecurityGroupDataTable(oState, intIDSecurityGroup, bolCheckStatus)

    End Function



    Public Function GetEmployeeBusinessCentersDataTable(ByVal oState As roWsState, ByVal intIDEmployee As Integer, ByVal bolDefaultCenter As Boolean, ByVal bolOnlyActiveCenters As Boolean) As roGenericVtResponse(Of DataSet) Implements ITaskSvc.GetEmployeeBusinessCentersDataTable

        Return TaskMethods.GetEmployeeBusinessCentersDataTable(oState, intIDEmployee, bolDefaultCenter, bolOnlyActiveCenters)
    End Function


    Public Function GetEmployeeDefaultBusinessCenter(ByVal oState As roWsState, ByVal intIDEmployee As Integer, ByVal xDate As Date) As roGenericVtResponse(Of Integer) Implements ITaskSvc.GetEmployeeDefaultBusinessCenter
        Return TaskMethods.GetEmployeeDefaultBusinessCenter(oState, intIDEmployee, xDate)
    End Function




    Public Function SavEmployeeBusinessCenters(ByVal IDEmployee As Integer, ByVal dsCenters As DataSet, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements ITaskSvc.SavEmployeeBusinessCenters
        Return TaskMethods.SavEmployeeBusinessCenters(IDEmployee, dsCenters, oState)
    End Function




    Public Function GetBusinessCenterByPassport(ByVal oState As roWsState, ByVal intIDPassport As Integer, ByVal bolCheckStatus As Boolean, ByVal bAudit As Boolean) As roGenericVtResponse(Of Integer()) Implements ITaskSvc.GetBusinessCenterByPassport

        Return TaskMethods.GetBusinessCenterByPassport(oState, intIDPassport, bolCheckStatus, bAudit)

    End Function


    Public Function GetBusinessCenterByID(ByVal intIDBusinessCenter As Integer, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of roBusinessCenter) Implements ITaskSvc.GetBusinessCenterByID

        Return TaskMethods.GetBusinessCenterByID(intIDBusinessCenter, oState, bAudit)
    End Function

    ''' <summary>
    ''' Guarda un roBusinessCenter. <br/>
    ''' * Se audita la grabación.
    ''' </summary>
    ''' <param name="oBusinessCenter">Definición del BusinessCenter.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>True o False</returns>
    ''' <remarks>En caso de retorno a false: a través del parámetro de retorno 'oState' se indica el motivo.</remarks>

    Public Function SaveBusinessCenter(ByVal oBusinessCenter As roBusinessCenter, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of roBusinessCenter) Implements ITaskSvc.SaveBusinessCenter

        Return TaskMethods.SaveBusinessCenter(oBusinessCenter, oState, bAudit)
    End Function


    ''' <summary>
    ''' Guarda los centros de coste de un passport concreto. <br/>
    ''' </summary>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <param name="intIDPassport">Passport al que se le asignan los centros de coste.</param>
    ''' <param name="intIDBusinessCenters">Centros de coste asignados al passport.</param>
    ''' <returns>True o False</returns>
    ''' <remarks>En caso de retorno a false: a través del parámetro de retorno 'oState' se indica el motivo.</remarks>

    Public Function SaveBusinessCenterByPassport(ByVal oState As roWsState, ByVal intIDPassport As Integer, ByVal intIDBusinessCenters() As Integer, ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean) Implements ITaskSvc.SaveBusinessCenterByPassport
        Return TaskMethods.SaveBusinessCenterByPassport(oState, intIDPassport, intIDBusinessCenters, bAudit)
    End Function

    ''' <summary>
    ''' Guarda los centros de coste de un passport concreto. <br/>
    ''' </summary>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <param name="intIDSecurityGroup">Group de funciones al que se le asignan los centros de coste.</param>
    ''' <param name="intIDBusinessCenters">Centros de coste asignados al passport.</param>
    ''' <returns>True o False</returns>
    ''' <remarks>En caso de retorno a false: a través del parámetro de retorno 'oState' se indica el motivo.</remarks>

    Public Function SaveBusinessCenterBySecurityGroup(ByVal oState As roWsState, ByVal intIDSecurityGroup As Integer, ByVal intIDBusinessCenters() As Integer, ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean) Implements ITaskSvc.SaveBusinessCenterBySecurityGroup

        Return TaskMethods.SaveBusinessCenterBySecurityGroup(oState, intIDSecurityGroup, intIDBusinessCenters, bAudit)
    End Function
    ''' <summary>
    ''' Elimina un roBusinessCenter.<br/>
    ''' * Se audita la acción.
    ''' </summary>
    ''' <param name="oBusinessCenter">Objeto 'roBusinessCenter' con la definición del roBusinessCenter a eliminar.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>True o False</returns>
    ''' <remarks>En caso de retorno a false: a través del parámetro de retorno 'oState' se indica el motivo.</remarks>

    Public Function DeleteBusinessCenter(ByVal oBusinessCenter As roBusinessCenter, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean) Implements ITaskSvc.DeleteBusinessCenter

        Return TaskMethods.DeleteBusinessCenter(oBusinessCenter, oState, bAudit)
    End Function

    ''' <summary>
    ''' Elimina una tarea.<br/>
    ''' * Se audita la acción.
    ''' </summary>
    ''' <param name="intIDBusinessCenter">Código que identifica el roBusinessCenter.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>True o False</returns>
    ''' <remarks>En caso de retorno a false: a través del parámetro de retorno 'oState' se indica el motivo.</remarks>

    Public Function DeleteBusinessCenterByID(ByVal intIDBusinessCenter As Integer, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean) Implements ITaskSvc.DeleteBusinessCenterByID

        Return TaskMethods.DeleteBusinessCenterByID(intIDBusinessCenter, oState, bAudit)
    End Function

    '=========================================
    '============= CUBO ======================
    '=========================================


    Public Function GetTaskFieldsList(ByVal intID As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of Generic.List(Of roTaskField)) Implements ITaskSvc.GetTaskFieldsList

        Return TaskMethods.GetTaskFieldsList(intID, oState)
    End Function


    Public Function GetTaskFieldsDataTable(ByVal intID As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet) Implements ITaskSvc.GetTaskFieldsDataTable

        Return TaskMethods.GetTaskFieldsDataTable(intID, oState)
    End Function


    Public Function GetTaskAlertsDataTable(ByVal intID As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet) Implements ITaskSvc.GetTaskAlertsDataTable

        Return TaskMethods.GetTaskAlertsDataTable(intID, oState)
    End Function

    Public Function GetTaskAlertDataTable(ByVal intID As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet) Implements ITaskSvc.GetTaskAlertDataTable

        Return TaskMethods.GetTaskAlertDataTable(intID, oState)
    End Function



    Public Function SaveTaskFields(ByVal intID As Integer, ByVal _TaskFields As Generic.List(Of roTaskField), ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements ITaskSvc.SaveTaskFields
        Return TaskMethods.SaveTaskFields(intID, _TaskFields, oState)
    End Function


    Public Function SaveTaskAlerts(ByVal intID As Integer, ByVal _TaskAlerts As Generic.List(Of roTaskAlert), ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements ITaskSvc.SaveTaskAlerts

        Return TaskMethods.SaveTaskAlerts(intID, _TaskAlerts, oState)
    End Function


    Public Function GetAntTaskPunchDate(ByVal _IDTask As Integer, ByVal _IDEmployee As Integer, ByVal _Date As DateTime, ByVal oState As roWsState) As roGenericVtResponse(Of DateTime) Implements ITaskSvc.GetAntTaskPunchDate
        Return TaskMethods.GetAntTaskPunchDate(_IDTask, _IDEmployee, _Date, oState)

    End Function


    Public Function GetNextTaskPunchDate(ByVal _IDTask As Integer, ByVal _IDEmployee As Integer, ByVal _Date As DateTime, ByVal oState As roWsState) As roGenericVtResponse(Of DateTime) Implements ITaskSvc.GetNextTaskPunchDate
        Return TaskMethods.GetNextTaskPunchDate(_IDTask, _IDEmployee, _Date, oState)
    End Function


    Public Function SaveTaskFieldsFromPunch(ByVal _IDTask As Integer, ByVal Field1 As String, ByVal Field2 As String, ByVal Field3 As String, ByVal Field4 As Double, ByVal Field5 As Double, ByVal Field6 As Double, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements ITaskSvc.SaveTaskFieldsFromPunch
        Return TaskMethods.SaveTaskFieldsFromPunch(_IDTask, Field1, Field2, Field3, Field4, Field5, Field6, oState)
    End Function


    Public Function SaveTaskAlertsFromPunch(ByVal _IDTask As Integer, ByVal _IDEmployee As Integer, ByVal _DateTime As Date, ByVal _Comment As String, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements ITaskSvc.SaveTaskAlertsFromPunch
        Return TaskMethods.SaveTaskAlertsFromPunch(_IDTask, _IDEmployee, _DateTime, _Comment, oState)

    End Function


#Region "Assignments"
    ''' <summary>
    ''' Obtiene los puestos que tiene asignada la tarea
    ''' </summary>
    ''' <param name="intIDTask">Código de la tarea.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Tabla con las columnas: IDAssignment </returns>
    ''' <remarks></remarks>

    Public Function GetTaskAssignments(ByVal intIDTask As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of Generic.List(Of roTaskAssignment)) Implements ITaskSvc.GetTaskAssignments

        Return TaskMethods.GetTaskAssignments(intIDTask, oState)
    End Function

    ''' <summary>
    ''' Obtiene los puestos que tiene asignada la tarea en un dataset.
    ''' </summary>
    ''' <param name="intIDTask">Código de la tarea.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Tabla con las columnas: IDAssignment </returns>
    ''' <remarks></remarks>

    Public Function GetTaskAssignmentsDatatable(ByVal intIDTask As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet) Implements ITaskSvc.GetTaskAssignmentsDatatable
        Return TaskMethods.GetTaskAssignmentsDatatable(intIDTask, oState)
    End Function
    ''' <summary>
    ''' Guardamos los puestos asignados a la tarea
    ''' </summary>
    ''' <param name="intIDTask">Código de la tarea.</param>
    ''' <param name="lstTaskAssignments">Lista de puestos asignados .</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Tabla con las columnas: IDAssignment </returns>
    ''' <remarks></remarks>

    Public Function SaveTaskAssignments(ByVal intIDTask As Integer, ByVal lstTaskAssignments As Generic.List(Of roTaskAssignment), ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements ITaskSvc.SaveTaskAssignments

        Return TaskMethods.SaveTaskAssignments(intIDTask, lstTaskAssignments, oState)
    End Function



#End Region



End Class
