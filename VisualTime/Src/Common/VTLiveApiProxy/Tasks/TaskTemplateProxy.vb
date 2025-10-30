Imports Robotics.Base
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTBusiness.Task

Public Class TaskTemplateProxy
    Implements ITaskTemplateSvc

    Public Function KeepAlive() As Boolean Implements ITaskTemplateSvc.KeepAlive
        Return True
    End Function



#Region "TaskTemplates"
    ''' <summary>
    ''' Obtiene la lista de plantillas de un proyecto.
    ''' </summary>
    ''' <param name="_Order"></param>
    ''' <param name="oState"></param>
    ''' <param name="_Audit"></param>
    ''' <returns>Una lista de objetos con la definición de las plantillas de tareas.</returns>
    ''' <remarks></remarks>

    Public Function GetTaskTemplates(ByVal _IDProject As Integer, ByVal _IDsFilter As String, ByVal _Order As String, ByVal _Audit As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of Generic.List(Of roTaskTemplate)) Implements ITaskTemplateSvc.GetTaskTemplates

        Return TaskTemplateMethods.GetTaskTemplates(_IDProject, _IDsFilter, _Order, _Audit, oState)
    End Function

    ''' <summary>
    ''' Obtiene la lista de plantillas de un proyecto.
    ''' </summary>
    ''' <param name="_Order"></param>
    ''' <param name="oState"></param>
    ''' <param name="_Audit"></param>
    ''' <returns>Un objeto dataset con una tabla con las plantillas de tareas.</returns>
    ''' <remarks></remarks>

    Public Function GetTaskTemplatesDataTable(ByVal _IDProject As Integer, ByVal _IDsFilter As String, ByVal _Order As String, ByVal _Audit As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet) Implements ITaskTemplateSvc.GetTaskTemplatesDataTable

        Return TaskTemplateMethods.GetTaskTemplatesDataTable(_IDProject, _IDsFilter, _Order, _Audit, oState)
    End Function


    Public Function GetTaskTemplate(ByVal _ID As Integer, ByVal _Audit As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of roTaskTemplate) Implements ITaskTemplateSvc.GetTaskTemplate

        Return TaskTemplateMethods.GetTaskTemplate(_ID, _Audit, oState)

    End Function


    ''' <summary>
    ''' Guarda los datos de la plantilla. Si és nuevo, se actualiza el ID de la plantilla pasada.<br/>
    ''' </summary>
    ''' <param name="oTaskTemplate">Puesto a guardar (roTaskTemplate)</param>
    ''' <param name="oState">Información adicional de estado</param>
    ''' <returns>Devuelve TRUE si se ha podido guardar el puesto.</returns>
    ''' <remarks></remarks>

    Public Function SaveTaskTemplate(ByVal oTaskTemplate As roTaskTemplate, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of roTaskTemplate) Implements ITaskTemplateSvc.SaveTaskTemplate
        Return TaskTemplateMethods.SaveTaskTemplate(oTaskTemplate, oState, bAudit)
    End Function


    ''' <summary>
    ''' Elimina la plantilla e tarea
    ''' </summary>
    ''' <param name="ID">ID del la plantilla a eliminar</param>
    ''' <param name="oState">Información adicional de estado</param>
    ''' <returns>Devuelve TRUE si se ha eliminado correctamente</returns>
    ''' <remarks>Si es FALSE, en oState.ResultEnum devuelve el motivo.</remarks>

    Public Function DeleteTaskTemplate(ByVal ID As Integer, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean) Implements ITaskTemplateSvc.DeleteTaskTemplate

        Return TaskTemplateMethods.DeleteTaskTemplate(ID, oState, bAudit)
    End Function

    ''' <summary>
    ''' Obtiene el nº de empleados asignados a una plantilla de tarea o el total de empleados en funcion de empleados y grupos seleccionados
    ''' </summary>
    ''' <param name="intIDTask">ID de la Tarea.</param>
    ''' <param name="sEmployees">Contiene empleados seleccionados</param>
    ''' <param name="sGroups">Contiene grupos seleccionados</param>
    ''' <returns>Nº de empleados asignados o seleccionados</returns>
    ''' <remarks></remarks>

    Public Function GetEmployeesFromTask(ByVal intIDTask As Integer, ByVal sEmployees As String, ByVal sGroups As String) As roGenericVtResponse(Of Double) Implements ITaskTemplateSvc.GetEmployeesFromTask

        Return TaskTemplateMethods.GetEmployeesFromTask(intIDTask, sEmployees, sGroups)
    End Function

    ''' <summary>
    ''' Obtiene el nombre de una tarea.
    ''' </summary>
    ''' <param name="_IDTaskTemplate">Código de la tarea</param>
    ''' <param name="oState"></param>
    ''' <returns>Nombre de la tarea correspondiente con el ID indicado</returns>
    ''' <remarks></remarks>

    Public Function GetTaskName(ByVal _IDTaskTemplate As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of String) Implements ITaskTemplateSvc.GetTaskName
        Return TaskTemplateMethods.GetTaskName(_IDTaskTemplate, oState)
    End Function
#End Region

#Region "Projects"
    ''' <summary>
    ''' Obtiene la lista de proyectos.
    ''' </summary>
    ''' <param name="_Order"></param>
    ''' <param name="oState"></param>
    ''' <param name="_Audit"></param>
    ''' <returns>Una lista de objetos con la definición de los proyectos.</returns>
    ''' <remarks></remarks>

    Public Function GetProjectsDataTable(ByVal _Order As String, ByVal _Audit As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet) Implements ITaskTemplateSvc.GetProjectsDataTable
        Return TaskTemplateMethods.GetProjectsDataTable(_Order, _Audit, oState)
    End Function


    Public Function GetProjectTemplate(ByVal _ID As Integer, ByVal _Audit As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of roProjectTemplates) Implements ITaskTemplateSvc.GetProjectTemplate

        Return TaskTemplateMethods.GetProjectTemplate(_ID, _Audit, oState)
    End Function


    ''' <summary>
    ''' Guarda los datos de la plantilla. Si és nuevo, se actualiza el ID de la plantilla pasada.<br/>
    ''' </summary>
    ''' <param name="oProjectTemplate">Puesto a guardar (oProjectTemplate)</param>
    ''' <param name="oState">Información adicional de estado</param>
    ''' <returns>Devuelve TRUE si se ha podido guardar el puesto.</returns>
    ''' <remarks></remarks>

    Public Function SaveProjectTemplate(ByVal oProjectTemplate As roProjectTemplates, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of roProjectTemplates) Implements ITaskTemplateSvc.SaveProjectTemplate

        Return TaskTemplateMethods.SaveProjectTemplate(oProjectTemplate, oState, bAudit)

    End Function


    ''' <summary>
    ''' Elimina la plantilla e tarea
    ''' </summary>
    ''' <param name="ID">ID del la plantilla a eliminar</param>
    ''' <param name="oState">Información adicional de estado</param>
    ''' <returns>Devuelve TRUE si se ha eliminado correctamente</returns>
    ''' <remarks>Si es FALSE, en oState.ResultEnum devuelve el motivo.</remarks>

    Public Function DeleteProjectTemplate(ByVal ID As Integer, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean) Implements ITaskTemplateSvc.DeleteProjectTemplate

        Return TaskTemplateMethods.DeleteProjectTemplate(ID, oState, bAudit)

    End Function
#End Region

    ''' <summary>
    ''' Genera las tareas a partir de la plantilla.<br/>
    ''' </summary>
    ''' <param name="_IDPassport">Passport del usuario que crea las tareas.</param>
    ''' <param name="ds">Dataset con una tabla que contiene las plantillas ya personalizadas.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>True o False</returns>
    ''' <remarks>En caso de retorno a false: a través del parámetro de retorno 'oState' se indica el motivo.</remarks>

    Public Function SaveTasksFromTemplates(ByVal _IDPassport As Integer, ByVal ds As DataSet, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements ITaskTemplateSvc.SaveTasksFromTemplates

        Return TaskTemplateMethods.SaveTasksFromTemplates(_IDPassport, ds, oState)
    End Function


End Class
