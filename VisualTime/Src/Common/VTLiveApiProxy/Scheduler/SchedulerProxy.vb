Imports Robotics.Base
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTBusiness.Scheduler
Imports Robotics.Base.VTUserFields.UserFields

Public Class SchedulerProxy
    Implements ISchedulerSvc

    Public Function KeepAlive() As Boolean Implements ISchedulerSvc.KeepAlive
        Return True
    End Function

    Public Function GetSchedulerRemarks(ByVal _IDEmployee As Integer, ByVal xBegin As Date, ByVal xEnd As Date, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet) Implements ISchedulerSvc.GetSchedulerRemarks
        Return SchedulerMethods.GetSchedulerRemarks(_IDEmployee, xBegin, xEnd, oState)
    End Function

    ''' <summary>
    ''' Obtiene la configuración de realtes del calendario para el pasaporte indicado.
    ''' </summary>
    ''' <param name="_IDPassport">Código del pasaporte del que se obtendrá la configuración</param>
    ''' <param name="oState"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>

    Public Function GetSchedulerRemarksConfig(ByVal _IDPassport As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of roSchedulerRemarks) Implements ISchedulerSvc.GetSchedulerRemarksConfig
        Return SchedulerMethods.GetSchedulerRemarksConfig(_IDPassport, oState)
    End Function

    ''' <summary>
    ''' Graba la configuración de resaltes del calendario en el contexto del pasaporte actual
    ''' </summary>
    ''' <param name="oSchedulerRemarks">Configuración de realtes a grabar.</param>
    ''' <param name="oState"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>

    Public Function SaveSchedulerRemarksConfig(ByVal oSchedulerRemarks As roSchedulerRemarks, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements ISchedulerSvc.SaveSchedulerRemarksConfig
        Return SchedulerMethods.SaveSchedulerRemarksConfig(oSchedulerRemarks, oState)
    End Function

    ''' <summary>
    ''' Devuelve Calendario (entradas y salidas)
    ''' </summary>
    ''' <param name="IDEmployees">ID de Empleado (-1 = todos)</param>
    ''' <param name="oState">roState</param>
    ''' <returns>Devuelve una colección de roSchedulerEmployee</returns>
    ''' <remarks></remarks>

    Public Function GetScheduledEmployeesFromList(ByVal IDEmployees As String, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet) Implements ISchedulerSvc.GetScheduledEmployeesFromList

        Return SchedulerMethods.GetScheduledEmployeesFromList(IDEmployees, oState)
    End Function


    ''' <summary>
    ''' Devuelve Calendario (entradas y salidas)
    ''' </summary>
    ''' <param name="IDGroup">ID de Grupo (-1 = todos)</param>
    ''' <param name="IDEmployee">ID de Empleado (-1 = todos)</param>
    ''' <param name="xDateStart">Fecha inicio</param>
    ''' <param name="xDateEnd">Fecha fin</param>
    ''' <param name="oState">roState</param>
    ''' <returns>Devuelve una colección de roSchedulerEmployee</returns>
    ''' <remarks></remarks>

    Public Function GetScheduledEmployeesForGroup(ByVal IDGroup As Integer,
                                      ByVal IDEmployee As Integer,
                                      ByVal xDateStart As Date,
                                      ByVal xDateEnd As Date,
                                      ByVal oState As roWsState,
                                      ByVal bAudit As Boolean) As roGenericVtResponse(Of DataSet) Implements ISchedulerSvc.GetScheduledEmployeesForGroup

        Return SchedulerMethods.GetScheduledEmployeesForGroup(IDGroup, IDEmployee, xDateStart, xDateEnd, oState, bAudit)
    End Function

    ''' <summary>
    ''' Devuelve Calendario (entradas y salidas)
    ''' </summary>
    ''' <param name="IDGroup">ID de Grupo (-1 = todos)</param>
    ''' <param name="IDEmployee">ID de Empleado (-1 = todos)</param>
    ''' <param name="xDateStart">Fecha inicio</param>
    ''' <param name="xDateEnd">Fecha fin</param>
    ''' <param name="_IncludeSubGroups">Incluir los empleados de los subgrupos. Sólo si se informa código de grupo distinto a -1.</param>
    ''' <param name="oState">roState</param>
    ''' <param name="_RowPosition">Núm. de registro a posicionar</param>
    ''' <param name="_PageRows">Núm. de registros por página</param>
    ''' <param name="_PagesCount">Devuelve el total de páginas</param>
    ''' <returns>Devuelve una colección de roSchedulerEmployee</returns>
    ''' <remarks></remarks>

    Public Function GetSchedulerDaily(ByVal IDGroup As Integer,
                                      ByVal IDEmployee As Integer,
                                      ByVal UserFieldName As String,
                                      ByVal UserFieldValue As String,
                                      ByVal xDateStart As Date,
                                      ByVal xDateEnd As Date,
                                      ByVal _IncludeSubGroups As Boolean,
                                      ByVal oState As roWsState,
                                      ByVal _RowPosition As Integer,
                                      ByVal _PageRows As Integer,
                                      ByVal _PagesCount As Integer, ByVal _EmpCount As Integer,
                                      ByVal bAudit As Boolean) As roGenericVtResponse(Of (Generic.List(Of roSchedulerEmployee), Integer, Integer)) Implements ISchedulerSvc.GetSchedulerDaily

        Return SchedulerMethods.GetSchedulerDaily(IDGroup, IDEmployee, UserFieldName, UserFieldValue, xDateStart, xDateEnd, _IncludeSubGroups, oState, _RowPosition, _PageRows, _PagesCount, _EmpCount, bAudit)
    End Function

    ''' <summary>
    ''' Devuelve Calendario (planificación)
    ''' </summary>
    ''' <param name="IDGroup">ID de Grupo (-1 = todos)</param>
    ''' <param name="IDEmployee">ID de Empleado (-1 = todos)</param>
    ''' <param name="xDateStart">Fecha inicio</param>
    ''' <param name="xDateEnd">Fecha fin</param>
    ''' <param name="_IncludeSubGroups">Incluir los empleados de los subgrupos. Sólo si se informa código de grupo distinto a -1.</param>
    ''' <param name="oState">roState</param>
    ''' <param name="_PageNumber">Núm. de página</param>
    ''' <param name="_PageRows">Núm. de registros por página</param>
    ''' <param name="_PagesCount">Devuelve el total de páginas</param>
    ''' <returns>Devuelve una colección de roSchedulerEmployee</returns>
    ''' <remarks></remarks>

    Public Function GetSchedulerDailyPlan(ByVal IDGroup As Integer,
                                             ByVal IDEmployee As Integer,
                                             ByVal UserFieldName As String,
                                             ByVal UserFieldValue As String,
                                             ByVal xDateStart As Date,
                                             ByVal xDateEnd As Date,
                                             ByVal _IncludeSubGroups As Boolean,
                                             ByVal oState As roWsState,
                                             ByVal _PageNumber As Integer,
                                             ByVal _PageRows As Integer,
                                             ByVal _PagesCount As Integer, ByVal _EmpCount As Integer,
                                             ByVal bAudit As Boolean) As roGenericVtResponse(Of (Generic.List(Of roSchedulerEmployee), Integer, Integer)) Implements ISchedulerSvc.GetSchedulerDailyPlan


        Return SchedulerMethods.GetSchedulerDailyPlan(IDGroup, IDEmployee, UserFieldName, UserFieldValue, xDateStart, xDateEnd, _IncludeSubGroups, oState, _PageNumber, _PageRows, _PagesCount, _EmpCount, bAudit)

    End Function

    ''' <summary>
    ''' Obtiene el calendario de planificación de un grupo.
    ''' </summary>
    ''' <param name="IDGroup">ID de Grupo a obtener</param>
    ''' <param name="IDEmployee">ID de empleado</param>
    ''' <param name="intTypeView">Tipo de Vista (0=25%,1=50%,2=100%)</param>
    ''' <param name="strTypeDate"></param>
    ''' <param name="intOtherType"></param>
    ''' <param name="xDateStart">Fecha Inicio</param>
    ''' <param name="xDateEnd">Fecha Fin</param>
    ''' <param name="intToDays"></param>
    ''' <param name="_ShortDateFormat"></param>
    ''' <param name="_MonthAndDayDateFormat"></param>
    ''' <param name="oState">Devuelve objecto roState</param>
    ''' <returns></returns>
    ''' <remarks></remarks>

    Public Function GetSchedulerPlanification(ByVal IDGroup As Integer,
                                              ByVal IDEmployee As Integer,
                                              ByVal intTypeView As Integer,
                                              ByVal strTypeDate As String,
                                              ByVal intOtherType As Integer,
                                              ByVal xDateStart As Date,
                                              ByVal xDateEnd As Date,
                                              ByVal intToDays As Integer,
                                              ByVal _ShortDateFormat As String,
                                              ByVal _MonthAndDayDateFormat As String,
                                              ByVal oState As roWsState) As roGenericVtResponse(Of String) Implements ISchedulerSvc.GetSchedulerPlanification

        Return SchedulerMethods.GetSchedulerPlanification(IDGroup, IDEmployee, intTypeView, strTypeDate, intOtherType, xDateStart, xDateEnd, intToDays, _ShortDateFormat, _MonthAndDayDateFormat, oState)
    End Function

    ''' <summary>
    ''' Obtiene el calendario principal de un grupo.
    ''' </summary>
    ''' <param name="IDGroup">ID de Grupo a obtener</param>
    ''' <param name="IDEmployee">ID de empleado</param>
    ''' <param name="intTypeView">Tipo de Vista (0=25%,1=50%,2=100%)</param>
    ''' <param name="strTypeDate"></param>
    ''' <param name="intOtherType"></param>
    ''' <param name="xDateStart">Fecha Inicio</param>
    ''' <param name="xDateEnd">Fecha Fin</param>
    ''' <param name="intToDays"></param>
    ''' <param name="_ShortDateFormat"></param>
    ''' <param name="_MonthAndDayDateFormat"></param>
    ''' <param name="oState">Devuelve objecto roState</param>
    ''' <returns></returns>
    ''' <remarks></remarks>

    Public Function GetScheduler(ByVal IDGroup As Integer,
                                 ByVal IDEmployee As Integer,
                                 ByVal intTypeView As Integer,
                                 ByVal strTypeDate As String,
                                 ByVal intOtherType As Integer,
                                 ByVal xDateStart As Date,
                                 ByVal xDateEnd As Date,
                                 ByVal intToDays As Integer,
                                 ByVal _ShortDateFormat As String,
                                 ByVal _MonthAndDayDateFormat As String,
                                 ByVal oState As roWsState) As roGenericVtResponse(Of String) Implements ISchedulerSvc.GetScheduler
        Return SchedulerMethods.GetScheduler(IDGroup, IDEmployee, intTypeView, strTypeDate, intOtherType, xDateStart, xDateEnd, intToDays, _ShortDateFormat, _MonthAndDayDateFormat, oState)
    End Function

    ''' <summary>
    ''' Obtiene la lista de plantillas de calendario definidas ordenadas por nombre.
    ''' </summary>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Lista de objetos 'roScheduleTemplate' con la definición de las distintas plantillas.</returns>
    ''' <remarks></remarks>

    Public Function GetScheduleTemplates(ByVal oState As roWsState) As roGenericVtResponse(Of Generic.List(Of roScheduleTemplate)) Implements ISchedulerSvc.GetScheduleTemplates
        Return SchedulerMethods.GetScheduleTemplates(oState)
    End Function
    ''' <summary>
    ''' Obtiene la lista de plantillas de calendario definidas ordenadas por nombre de la version v2.
    ''' </summary>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Lista de objetos 'roScheduleTemplate' con la definición de las distintas plantillas.</returns>
    ''' <remarks></remarks>


    Public Function GetScheduleTemplatesv2(ByVal oState As roWsState) As roGenericVtResponse(Of Generic.List(Of roScheduleTemplate)) Implements ISchedulerSvc.GetScheduleTemplatesv2
        Return SchedulerMethods.GetScheduleTemplatesv2(oState)
    End Function


    ''' <summary>
    ''' Obtiene la definición de una plantilla de calendario.
    ''' </summary>
    ''' <param name="_ID">Código de la plantilla a obtener.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Un objeto 'roScheduleTemplate' con la definición de la plantilla.</returns>
    ''' <remarks></remarks>

    Public Function GetScheduleTemplate(ByVal _ID As Integer, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of roScheduleTemplate) Implements ISchedulerSvc.GetScheduleTemplate
        Return SchedulerMethods.GetScheduleTemplate(_ID, oState, bAudit)
    End Function

    ''' <summary>
    ''' Guarda la definición de una plantilla de calendario.
    ''' </summary>
    ''' <param name="oScheduleTemplate">Objeto 'roScheduleTemplate' con la definición de la plantilla a guardar.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>True o False</returns>
    ''' <remarks>En caso de retorno a false: a través del parámetro de retorno 'oState' se indica el motivo.</remarks>

    Public Function SaveScheduleTemplate(ByVal oScheduleTemplate As roScheduleTemplate, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean) Implements ISchedulerSvc.SaveScheduleTemplate
        Return SchedulerMethods.SaveScheduleTemplate(oScheduleTemplate, oState, bAudit)
    End Function

    ''' <summary>
    ''' Elimina una plantilla de calendario.
    ''' </summary>
    ''' <param name="_ID">Código de la plantilla de calendario.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>True o False</returns>
    ''' <remarks>En caso de retorno a false: a través del parámetro de retorno 'oState' se indica el motivo.</remarks>

    Public Function DeleteScheduleTemplate(ByVal _ID As Integer, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean) Implements ISchedulerSvc.DeleteScheduleTemplate
        Return SchedulerMethods.DeleteScheduleTemplate(_ID, oState, bAudit)
    End Function


    Public Function AssignScheduleTemplate(ByVal _IDTemplate As Integer, ByVal _IDGroup As Integer, ByVal _UserFieldConditions As Generic.List(Of roUserFieldCondition), ByVal _Year As Integer, ByVal _IDShift As Integer, ByVal _StartShift As DateTime, ByVal _LockDays As Boolean, ByVal _IncludeChildGroups As Boolean, ByVal _LockedDayAction As LockedDayAction, ByVal _CoverageDayAction As LockedDayAction, ByVal xDateLocked As Date, ByVal intIDEmployeeLocked As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements ISchedulerSvc.AssignScheduleTemplate
        Return SchedulerMethods.AssignScheduleTemplate(_IDTemplate, _IDGroup, _UserFieldConditions, _Year, _IDShift, _StartShift, _LockDays, _IncludeChildGroups, _LockedDayAction, _CoverageDayAction, xDateLocked, intIDEmployeeLocked, oState)
    End Function



    Public Function AssignScheduleTemplatev2(ByVal _IDTemplate As Integer, ByVal _lstEmployees As Generic.List(Of Integer), ByVal _IDShift As Integer, ByVal _StartShift As DateTime, ByVal _LockDays As Boolean, ByVal _LockedDayAction As LockedDayAction, ByVal _CoverageDayAction As LockedDayAction, ByVal xDateLocked As Date, ByVal intIDEmployeeLocked As Integer, ByVal oState As roWsState, ByVal _FeastDays As Boolean) As roGenericVtResponse(Of Boolean) Implements ISchedulerSvc.AssignScheduleTemplatev2
        Return SchedulerMethods.AssignScheduleTemplatev2(_IDTemplate, _lstEmployees, _IDShift, _StartShift, _LockDays, _LockedDayAction, _CoverageDayAction, xDateLocked, intIDEmployeeLocked, oState, _FeastDays)
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

    Public Function GetDailyCoverage(ByVal _IDGroup As Integer, ByVal _Date As Date, ByVal _Audit As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of roDailyCoverage) Implements ISchedulerSvc.GetDailyCoverage
        Return SchedulerMethods.GetDailyCoverage(_IDGroup, _Date, _Audit, oState)
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

    Public Function GetDailyCoverageDataTable(ByVal _IDGroup As Integer, ByVal _Date As Date, ByVal _Audit As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet) Implements ISchedulerSvc.GetDailyCoverageDataTable
        Return SchedulerMethods.GetDailyCoverageDataTable(_IDGroup, _Date, _Audit, oState)
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

    Public Function GetDailyCoverages(ByVal _IDGroup As Integer, ByVal _BeginDate As Date, ByVal _EndDate As Date, ByVal _Audit As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of Generic.List(Of roDailyCoverage)) Implements ISchedulerSvc.GetDailyCoverages
        Return SchedulerMethods.GetDailyCoverages(_IDGroup, _BeginDate, _EndDate, _Audit, oState)
    End Function

    ''' <summary>
    ''' Guarda la definición de la dotación para un grupo y fecha.
    ''' </summary>
    ''' <param name="_DailyCoverage"></param>
    ''' <param name="oState"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>

    Public Function SaveTeoricDailyCoverage(ByVal _DailyCoverage As roDailyCoverage, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements ISchedulerSvc.SaveTeoricDailyCoverage

        Return SchedulerMethods.SaveTeoricDailyCoverage(_DailyCoverage, oState)
    End Function


    Public Function CopyTeoricDailyCoverage(ByVal _IDGroup As Integer, ByVal _SourceBeginDate As Date, ByVal _SourceEndDate As Date, ByVal _DestinationBeginDate As Date, ByVal _DestinationEndDate As Date, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements ISchedulerSvc.CopyTeoricDailyCoverage

        Return SchedulerMethods.CopyTeoricDailyCoverage(_IDGroup, _SourceBeginDate, _SourceEndDate, _DestinationBeginDate, _DestinationEndDate, oState)
    End Function


    ''' <summary>
    ''' Elimina toda la dotación de un grupo para una fecha.
    ''' </summary>
    ''' <param name="_IDGroup"></param>
    ''' <param name="_Date"></param>
    ''' <param name="oState"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>

    Public Function DeleteDailyCoverage(ByVal _IDGroup As Integer, ByVal _Date As Date, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements ISchedulerSvc.DeleteDailyCoverage

        Return SchedulerMethods.DeleteDailyCoverage(_IDGroup, _Date, oState)
    End Function

    ''' <summary>
    ''' Devuelve la lista de puestos que puede cubrir un empleado por un horario. Los ordena según la cobertura del puesto en el horario.
    ''' </summary>
    ''' <param name="_IDEmployee"></param>
    ''' <param name="_IDShift"></param>
    ''' <param name="oState"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>

    Public Function GetEmployeAndShiftAssignments(ByVal _IDEmployee As Integer, ByVal _IDShift As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of Generic.List(Of VTBusiness.Assignment.roAssignment)) Implements ISchedulerSvc.GetEmployeAndShiftAssignments

        Return SchedulerMethods.GetEmployeAndShiftAssignments(_IDEmployee, _IDShift, oState)
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

    Public Function GetDailyCoverageAssignmentDetailDataTable(ByVal _IDGroup As Integer, ByVal _Date As Date, ByVal _IDassignment As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet) Implements ISchedulerSvc.GetDailyCoverageAssignmentDetailDataTable

        Return SchedulerMethods.GetDailyCoverageAssignmentDetailDataTable(_IDGroup, _Date, _IDassignment, oState)
    End Function


    Public Function GetDailyCoverageAssignmentFromEmployeeDate(ByVal _IDEmployee As Integer, ByVal _CoverageDate As Date, ByVal oState As roWsState) As roGenericVtResponse(Of roDailyCoverageAssignment) Implements ISchedulerSvc.GetDailyCoverageAssignmentFromEmployeeDate

        Return SchedulerMethods.GetDailyCoverageAssignmentFromEmployeeDate(_IDEmployee, _CoverageDate, oState)
    End Function


    '=========================================
    '============= CUBO ======================
    '=========================================

    Public Function GetSchedulerViewsDataSet(ByVal IdPassport As Integer, ByVal eType As AnalyticsTypeEnum, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet) Implements ISchedulerSvc.GetSchedulerViewsDataSet

        Return SchedulerMethods.GetSchedulerViewsDataSet(IdPassport, eType, oState)
    End Function


    Public Function GetSchedulerViewbyID(ByVal ID As Integer, ByVal IdPassport As Integer, ByVal eType As AnalyticsTypeEnum, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet) Implements ISchedulerSvc.GetSchedulerViewbyID

        Return SchedulerMethods.GetSchedulerViewbyID(ID, IdPassport, eType, oState)
    End Function


    Public Function DeleteSchedulerView(ByVal intID As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements ISchedulerSvc.DeleteSchedulerView

        Return SchedulerMethods.DeleteSchedulerView(intID, oState)
    End Function


    Public Function NewSchedulerView(ByVal IdView As Integer, ByVal IdPassport As Integer, ByVal NameView As String, ByVal Description As String, ByVal DateView As DateTime,
                                      ByVal Employees As String, ByVal Concepts As String, ByVal DateInf As DateTime, ByVal DateSup As DateTime, ByVal CubeLayout As String,
                                      ByVal eType As AnalyticsTypeEnum, ByVal strUserFieldFilter As String, ByVal strGraphOptions As String,
                                     ByVal oState As roWsState, ByVal strBusinessCenters As String) As roGenericVtResponse(Of Boolean) Implements ISchedulerSvc.NewSchedulerView

        Return SchedulerMethods.NewSchedulerView(IdView, IdPassport, NameView, Description, DateView, Employees, Concepts, DateInf, DateSup, CubeLayout, eType, strUserFieldFilter, strGraphOptions, oState, strBusinessCenters)
    End Function

End Class
