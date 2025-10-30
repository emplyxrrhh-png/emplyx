Imports System.Data
Imports Robotics.Base
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTBusiness.Scheduler
Imports Robotics.Security.Base

Public Class SchedulerMethods

    ''' <summary>
    ''' Obtiene la configuración de realtes del calendario para el pasaporte indicado.
    ''' </summary>
    ''' <param name="_IDPassport">Código del pasaporte del que se obtendrá la configuración</param>
    ''' <param name="oState"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>

    Public Shared Function GetSchedulerRemarksConfig(ByVal _IDPassport As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of roSchedulerRemarks)

        Dim oResult As New roGenericVtResponse(Of roSchedulerRemarks)
        Dim bState = New roSchedulerState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        oResult.Value = New roSchedulerRemarks(_IDPassport, bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult

    End Function

    ''' <summary>
    ''' Graba la configuración de resaltes del calendario en el contexto del pasaporte actual
    ''' </summary>
    ''' <param name="oSchedulerRemarks">Configuración de realtes a grabar.</param>
    ''' <param name="oState"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>

    Public Shared Function SaveSchedulerRemarksConfig(ByVal oSchedulerRemarks As roSchedulerRemarks, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)

        Dim oResult As New roGenericVtResponse(Of Boolean)
        Dim bState = New roSchedulerState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        oResult.Value = oSchedulerRemarks.Save()

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Devuelve Calendario (entradas y salidas)
    ''' </summary>
    ''' <param name="IDEmployees">ID de Empleado (-1 = todos)</param>
    ''' <param name="oState">roState</param>
    ''' <returns>Devuelve una colección</returns>
    ''' <remarks></remarks>

    Public Shared Function GetScheduledEmployeesFromList(ByVal IDEmployees As String, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet)

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim bState = New roSchedulerState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim ds As DataSet = Nothing
        Dim tb As DataTable = roScheduler.GetScheduledEmployeesFromList(IDEmployees, bState)
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
    ''' Obtiene la lista de plantillas de calendario definidas ordenadas por nombre.
    ''' </summary>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Lista de objetos 'roScheduleTemplate' con la definición de las distintas plantillas.</returns>
    ''' <remarks></remarks>

    Public Shared Function GetScheduleTemplates(ByVal oState As roWsState) As roGenericVtResponse(Of Generic.List(Of roScheduleTemplate))

        Dim oResult As New roGenericVtResponse(Of Generic.List(Of roScheduleTemplate))
        Dim bState = New roSchedulerState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        oResult.Value = roScheduleTemplate.GetScheduleTemplates(bState)
        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult

    End Function

    ''' <summary>
    ''' Obtiene la lista de plantillas de calendario definidas ordenadas por nombre de la version v2.
    ''' </summary>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Lista de objetos 'roScheduleTemplate' con la definición de las distintas plantillas.</returns>
    ''' <remarks></remarks>

    Public Shared Function GetScheduleTemplatesv2(ByVal oState As roWsState) As roGenericVtResponse(Of Generic.List(Of roScheduleTemplate))

        Dim oResult As New roGenericVtResponse(Of Generic.List(Of roScheduleTemplate))
        Dim bState = New roSchedulerState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        oResult.Value = roScheduleTemplate.GetScheduleTemplates(bState, 2)
        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Obtiene la definición de una plantilla de calendario.
    ''' </summary>
    ''' <param name="_ID">Código de la plantilla a obtener.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Un objeto 'roScheduleTemplate' con la definición de la plantilla.</returns>
    ''' <remarks></remarks>

    Public Shared Function GetScheduleTemplate(ByVal _ID As Integer, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of roScheduleTemplate)

        Dim oResult As New roGenericVtResponse(Of roScheduleTemplate)
        Dim bState = New roSchedulerState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        oResult.Value = New roScheduleTemplate(_ID, bState, bAudit)
        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Guarda la definición de una plantilla de calendario.
    ''' </summary>
    ''' <param name="oScheduleTemplate">Objeto 'roScheduleTemplate' con la definición de la plantilla a guardar.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>True o False</returns>
    ''' <remarks>En caso de retorno a false: a través del parámetro de retorno 'oState' se indica el motivo.</remarks>

    Public Shared Function SaveScheduleTemplate(ByVal oScheduleTemplate As roScheduleTemplate, ByVal oState As roWsState, ByVal bAudit As Boolean, ByVal idPassport As Integer) As roGenericVtResponse(Of Boolean)

        Dim oResult As New roGenericVtResponse(Of Boolean)
        Dim bState = New roSchedulerState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        oResult.Value = oScheduleTemplate.Save(bAudit, idPassport)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Elimina una plantilla de calendario.
    ''' </summary>
    ''' <param name="_ID">Código de la plantilla de calendario.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>True o False</returns>
    ''' <remarks>En caso de retorno a false: a través del parámetro de retorno 'oState' se indica el motivo.</remarks>

    Public Shared Function DeleteScheduleTemplate(ByVal _ID As Integer, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean)

        Dim oResult As New roGenericVtResponse(Of Boolean)
        Dim bState = New roSchedulerState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oScheduleTemplate As New roScheduleTemplate(_ID, bState, bAudit)
        oResult.Value = oScheduleTemplate.Delete(bAudit)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Obtiene la definición de la dotación para un grupo y fecha.
    ''' </summary>
    ''' <param name="_IDGroup">Código del grupo.</param>
    ''' <param name="_Date">Fecha de la dotación.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <param name="_Audit">Indica si se tiene que auditar o no la consulta.</param>
    ''' <returns>Objeto con la definición de la dotación.</returns>
    ''' <remarks></remarks>

    Public Shared Function GetDailyCoverage(ByVal _IDGroup As Integer, ByVal _Date As Date, ByVal _Audit As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of roDailyCoverage)

        Dim oResult As New roGenericVtResponse(Of roDailyCoverage)
        'cambio mi state genérico a un estado especifico
        Dim bState = New roSchedulerState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        oResult.Value = New roDailyCoverage(_IDGroup, _Date, bState, _Audit)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Obtiene la definición de la dotación para un grupo y fecha.
    ''' </summary>
    ''' <param name="_IDGroup">Código del grupo.</param>
    ''' <param name="_Date">Fecha de la dotación.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <param name="_Audit">Indica si se tiene que auditar o no la consulta.</param>
    ''' <returns>Un objeto dataset con una tabla con los datos.</returns>
    ''' <remarks></remarks>

    Public Shared Function GetDailyCoverageDataTable(ByVal _IDGroup As Integer, ByVal _Date As Date, ByVal _Audit As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet)

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim bState = New roSchedulerState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim ds As New DataSet
        Dim tb As DataTable = roDailyCoverageAssignment.GetDailyCoverageAssignmentsDataTable(_IDGroup, _Date, bState, _Audit)
        If tb IsNot Nothing Then ds.Tables.Add(tb)
        oResult.Value = ds

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Obtiene una lista con las definiciones de las dotaciones teóricas de un grupo para un periodo de fechas.
    ''' </summary>
    ''' <param name="_IDGroup"></param>
    ''' <param name="_BeginDate"></param>
    ''' <param name="_EndDate"></param>
    ''' <param name="_Audit"></param>
    ''' <param name="oState"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>

    Public Shared Function GetDailyCoverages(ByVal _IDGroup As Integer, ByVal _BeginDate As Date, ByVal _EndDate As Date, ByVal _Audit As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of Generic.List(Of roDailyCoverage))

        Dim oResult As New roGenericVtResponse(Of Generic.List(Of roDailyCoverage))
        Dim bState = New roSchedulerState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        oResult.Value = roDailyCoverage.GetDailyCoverages(_IDGroup, _BeginDate, _EndDate, bState, _Audit)
        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Guarda la definición de la dotación para un grupo y fecha.
    ''' </summary>
    ''' <param name="_DailyCoverage"></param>
    ''' <param name="oState"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>

    Public Shared Function SaveTeoricDailyCoverage(ByVal _DailyCoverage As roDailyCoverage, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)

        Dim oResult As New roGenericVtResponse(Of Boolean)
        Dim bState = New roSchedulerState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        _DailyCoverage.State = bState
        oResult.Value = _DailyCoverage.SaveTeoric()

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function CopyTeoricDailyCoverage(ByVal _IDGroup As Integer, ByVal _SourceBeginDate As Date, ByVal _SourceEndDate As Date, ByVal _DestinationBeginDate As Date, ByVal _DestinationEndDate As Date, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)

        Dim oResult As New roGenericVtResponse(Of Boolean)
        Dim bState = New roSchedulerState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        oResult.Value = roDailyCoverage.CopyTeoricDailyCoverage(_IDGroup, _SourceBeginDate, _SourceEndDate, _DestinationBeginDate, _DestinationEndDate, bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult

    End Function

    ''' <summary>
    ''' Elimina toda la dotación de un grupo para una fecha.
    ''' </summary>
    ''' <param name="_IDGroup"></param>
    ''' <param name="_Date"></param>
    ''' <param name="oState"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>

    Public Shared Function DeleteDailyCoverage(ByVal _IDGroup As Integer, ByVal _Date As Date, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)

        Dim oResult As New roGenericVtResponse(Of Boolean)
        Dim bState = New roSchedulerState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oDailyCoverage As New roDailyCoverage(_IDGroup, _Date, bState, False)
        oResult.Value = oDailyCoverage.Delete()

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult

    End Function

    ''' <summary>
    ''' Devuelve la lista de puestos que puede cubrir un empleado por un horario. Los ordena según la cobertura del puesto en el horario.
    ''' </summary>
    ''' <param name="_IDEmployee"></param>
    ''' <param name="_IDShift"></param>
    ''' <param name="oState"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>

    Public Shared Function GetEmployeAndShiftAssignments(ByVal _IDEmployee As Integer, ByVal _IDShift As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of Generic.List(Of VTBusiness.Assignment.roAssignment))

        Dim oResult As New roGenericVtResponse(Of Generic.List(Of VTBusiness.Assignment.roAssignment))
        Dim bState = New roSchedulerState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oAssignmentState As New Assignment.roAssignmentState
        roBusinessState.CopyTo(bState, oAssignmentState)
        Dim oRet As Generic.List(Of Assignment.roAssignment) = Assignment.roAssignment.GetEmployeeAndShiftAssignments(_IDEmployee, _IDShift, oAssignmentState)
        roBusinessState.CopyTo(oAssignmentState, bState)

        oResult.Value = oRet

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult

    End Function

    ''' <summary>
    ''' Devuelve el detalle de los empleados planificados por una covertura (grupo, fecha y puesto).
    ''' </summary>
    ''' <param name="_IDGroup"></param>
    ''' <param name="_Date"></param>
    ''' <param name="_IDassignment"></param>
    ''' <param name="oState"></param>
    ''' <returns>Dataset con un solo Datatable.</returns>
    ''' <remarks></remarks>

    Public Shared Function GetDailyCoverageAssignmentDetailDataTable(ByVal _IDGroup As Integer, ByVal _Date As Date, ByVal _IDassignment As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet)

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim bState = New roSchedulerState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim ds As New DataSet
        Dim oDailyCoverageAssignment As New roDailyCoverageAssignment(_IDGroup, _Date, _IDassignment, bState)
        Dim tb As DataTable = oDailyCoverageAssignment.GetPlannedDetail()
        If tb IsNot Nothing Then ds.Tables.Add(tb)
        oResult.Value = ds

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function GetDailyCoverageAssignmentFromEmployeeDate(ByVal _IDEmployee As Integer, ByVal _CoverageDate As Date, ByVal oState As roWsState) As roGenericVtResponse(Of roDailyCoverageAssignment)

        Dim oResult As New roGenericVtResponse(Of roDailyCoverageAssignment)
        Dim bState = New roSchedulerState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        oResult.Value = roDailyCoverageAssignment.GetDailyCoverageAssignmentFromEmployeeDate(_IDEmployee, _CoverageDate, bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

End Class