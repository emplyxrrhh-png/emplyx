Imports System.Data
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTBusiness.Task

Public Class TaskTemplateMethods

#Region "TaskTemplates"

    ''' <summary>
    ''' Obtiene la lista de plantillas de un proyecto.
    ''' </summary>
    ''' <param name="_Order"></param>
    ''' <param name="oState"></param>
    ''' <param name="_Audit"></param>
    ''' <returns>Una lista de objetos con la definición de las plantillas de tareas.</returns>
    ''' <remarks></remarks>

    Public Shared Function GetTaskTemplates(ByVal _IDProject As Integer, ByVal _IDsFilter As String, ByVal _Order As String, ByVal _Audit As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of Generic.List(Of roTaskTemplate))

        Dim bState = New roTaskState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Generic.List(Of roTaskTemplate))
        oResult.Value = roTaskTemplate.GetTasksByProject(_IDProject, _IDsFilter, _Order, bState, _Audit)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Obtiene la lista de plantillas de un proyecto.
    ''' </summary>
    ''' <param name="_Order"></param>
    ''' <param name="oState"></param>
    ''' <param name="_Audit"></param>
    ''' <returns>Un objeto dataset con una tabla con las plantillas de tareas.</returns>
    ''' <remarks></remarks>

    Public Shared Function GetTaskTemplatesDataTable(ByVal _IDProject As Integer, ByVal _IDsFilter As String, ByVal _Order As String, ByVal _Audit As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet)

        Dim bState = New roTaskState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim ds As DataSet = Nothing
        Dim tb As DataTable = roTaskTemplate.GetTasksByProjectDataTable(_IDProject, _IDsFilter, _Order, bState, _Audit)
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

    Public Shared Function GetTaskTemplate(ByVal _ID As Integer, ByVal _Audit As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of roTaskTemplate)

        Dim bState = New roTaskState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of roTaskTemplate)
        oResult.Value = New roTaskTemplate(_ID, -1, bState, _Audit)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Guarda los datos de la plantilla. Si és nuevo, se actualiza el ID de la plantilla pasada.<br/>
    ''' </summary>
    ''' <param name="oTaskTemplate">Puesto a guardar (roTaskTemplate)</param>
    ''' <param name="oState">Información adicional de estado</param>
    ''' <returns>Devuelve TRUE si se ha podido guardar el puesto.</returns>
    ''' <remarks></remarks>

    Public Shared Function SaveTaskTemplate(ByVal oTaskTemplate As roTaskTemplate, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of roTaskTemplate)

        oTaskTemplate.State = New roTaskState(-1)
        roWsStateManager.CopyTo(oState, oTaskTemplate.State)
        oTaskTemplate.State.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of roTaskTemplate)
        If oTaskTemplate.Save(bAudit) Then
            oResult.Value = oTaskTemplate
        Else
            oResult.Value = Nothing
        End If

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oTaskTemplate.State, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Elimina la plantilla e tarea
    ''' </summary>
    ''' <param name="ID">ID del la plantilla a eliminar</param>
    ''' <param name="oState">Información adicional de estado</param>
    ''' <returns>Devuelve TRUE si se ha eliminado correctamente</returns>
    ''' <remarks>Si es FALSE, en oState.ResultEnum devuelve el motivo.</remarks>

    Public Shared Function DeleteTaskTemplate(ByVal ID As Integer, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean)

        Dim bState = New roTaskState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)
        Dim oTaskTemplate As New roTaskTemplate(ID, -1, bState, False)
        oResult.Value = oTaskTemplate.Delete(bAudit)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oTaskTemplate.State, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Obtiene el nº de empleados asignados a una plantilla de tarea o el total de empleados en funcion de empleados y grupos seleccionados
    ''' </summary>
    ''' <param name="intIDTask">ID de la Tarea.</param>
    ''' <param name="sEmployees">Contiene empleados seleccionados</param>
    ''' <param name="sGroups">Contiene grupos seleccionados</param>
    ''' <returns>Nº de empleados asignados o seleccionados</returns>
    ''' <remarks></remarks>

    Public Shared Function GetEmployeesFromTask(ByVal intIDTask As Integer, ByVal sEmployees As String, ByVal sGroups As String) As roGenericVtResponse(Of Double)

        Dim bState = New roTaskState(-1)

        Dim oResult As New roGenericVtResponse(Of Double)

        oResult.Value = roTaskTemplate.GetEmployeesFromTask(intIDTask, sEmployees, sGroups)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Obtiene el nombre de una tarea.
    ''' </summary>
    ''' <param name="_IDTaskTemplate">Código de la tarea</param>
    ''' <param name="oState"></param>
    ''' <returns>Nombre de la tarea correspondiente con el ID indicado</returns>
    ''' <remarks></remarks>

    Public Shared Function GetTaskName(ByVal _IDTaskTemplate As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of String)

        Dim bState = New roTaskState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of String)

        oResult.Value = roTaskTemplate.GetTaskName(_IDTaskTemplate, bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
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

    Public Shared Function GetProjectsDataTable(ByVal _Order As String, ByVal _Audit As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet)

        Dim bState = New roTaskState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim ds As DataSet = roProjectTemplates.GetProjects(bState, _Order)

        oResult.Value = ds

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function GetProjectTemplate(ByVal _ID As Integer, ByVal _Audit As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of roProjectTemplates)

        Dim bState = New roTaskState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of roProjectTemplates)
        oResult.Value = New roProjectTemplates(_ID, "", bState, -1, _Audit)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Guarda los datos de la plantilla. Si és nuevo, se actualiza el ID de la plantilla pasada.<br/>
    ''' </summary>
    ''' <param name="oProjectTemplate">Puesto a guardar (oProjectTemplate)</param>
    ''' <param name="oState">Información adicional de estado</param>
    ''' <returns>Devuelve TRUE si se ha podido guardar el puesto.</returns>
    ''' <remarks></remarks>

    Public Shared Function SaveProjectTemplate(ByVal oProjectTemplate As roProjectTemplates, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of roProjectTemplates)

        oProjectTemplate.State = New roTaskState(-1)
        roWsStateManager.CopyTo(oState, oProjectTemplate.State)
        oProjectTemplate.State.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of roProjectTemplates)
        If oProjectTemplate.Save(bAudit) Then
            oResult.Value = oProjectTemplate
        Else
            oResult.Value = Nothing
        End If

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oProjectTemplate.State, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Elimina la plantilla e tarea
    ''' </summary>
    ''' <param name="ID">ID del la plantilla a eliminar</param>
    ''' <param name="oState">Información adicional de estado</param>
    ''' <returns>Devuelve TRUE si se ha eliminado correctamente</returns>
    ''' <remarks>Si es FALSE, en oState.ResultEnum devuelve el motivo.</remarks>

    Public Shared Function DeleteProjectTemplate(ByVal ID As Integer, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean)

        Dim bState = New roTaskState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)

        Dim oProjectTemplate As New roProjectTemplates(ID, "", bState, -1, bAudit)
        oResult.Value = oProjectTemplate.Delete(bAudit)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oProjectTemplate.State, newGState)
        oResult.Status = newGState
        Return oResult

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

    Public Shared Function SaveTasksFromTemplates(ByVal _IDPassport As Integer, ByVal ds As DataSet, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)

        Dim bState = New roTaskState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)

        oResult.Value = roTaskTemplate.SaveTasksFromTemplates(_IDPassport, ds.Tables(0), bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

End Class