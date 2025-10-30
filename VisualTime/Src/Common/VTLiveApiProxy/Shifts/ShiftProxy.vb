Imports Robotics.Base
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTBusiness.Shift
Imports Robotics.DataLayer.AccessHelper


Public Class ShiftProxy
    Implements IShiftSvc

    Public Function KeepAlive() As Boolean Implements IShiftSvc.KeepAlive
        Return True
    End Function

    ''' <summary>
    ''' Obtiene los horarios del grupo de horarios (Shifts).<br />
    ''' </summary>
    ''' <param name="intIDGroup">Id del grupo de horarios. Si és -1, no aplica filtro de grupo.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <param name="IncludeObsoletes">Incluye plantillas obsoletas?</param>
    ''' <returns>DataSet (ID, Name, Color, ShortName)</returns>
    ''' <remarks></remarks>
    Public Function GetShifts(ByVal intIDGroup As Integer, ByVal oState As roWsState, ByVal IncludeObsoletes As Boolean) As roGenericVtResponse(Of DataSet) Implements IShiftSvc.GetShifts
        Return ShiftMethods.GetShifts(intIDGroup, oState, IncludeObsoletes)
    End Function

    ''' <summary>
    ''' Obtiene los horarios del grupo de horarios (Shifts).<br />
    ''' </summary>
    ''' <param name="intIDGroup">Id del grupo de horarios. Si és -1, no aplica filtro de grupo.</param>
    ''' <param name="_IDAssignment">Código del puesto. Si és -1 no aplica filtro de puesto. Sólo filtra si no es Live eXpress.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <param name="IncludeObsoletes">Incluye plantillas obsoletas?</param>
    ''' <returns>DataSet (ID, Name, Color, ShortName)</returns>
    ''' <remarks></remarks>
    Public Function GetShiftsAssignment(ByVal intIDGroup As Integer, ByVal _IDAssignment As Integer, ByVal oState As roWsState, ByVal IncludeObsoletes As Boolean) As roGenericVtResponse(Of DataSet) Implements IShiftSvc.GetShiftsAssignment

        Return ShiftMethods.GetShiftsAssignment(intIDGroup, _IDAssignment, oState, IncludeObsoletes)
    End Function

    ''' <summary>
    ''' Obtiene los horarios del grupo de horarios (Shifts).<br />
    ''' </summary>
    ''' <param name="intIDGroup">Id del grupo de horarios. Si és -1, no aplica filtro de grupo.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <param name="IncludeObsoletes">Incluye plantillas obsoletas?</param>
    ''' <returns>DataSet (ID, Name, Color, ShortName)</returns>
    ''' <remarks></remarks>

    Public Function GetShiftsPlanification(ByVal intIDGroup As Integer, ByVal oState As roWsState, ByVal IncludeObsoletes As Boolean) As roGenericVtResponse(Of DataSet) Implements IShiftSvc.GetShiftsPlanification

        Return ShiftMethods.GetShiftsPlanification(intIDGroup, oState, IncludeObsoletes)
    End Function


    Public Function GetShiftsPortal(ByVal intIDGroup As Integer, ByVal oState As roWsState, ByVal IncludeObsoletes As Boolean) As roGenericVtResponse(Of DataSet) Implements IShiftSvc.GetShiftsPortal

        Return ShiftMethods.GetShiftsPortal(intIDGroup, oState, IncludeObsoletes)

    End Function

    ''' <summary>
    ''' Devuelve la lista de justificaciones a las que tiene acceso de justificar (pestaña visibilidad de la pantalla de definición de justificaciones) el empleado indicado.
    ''' </summary>
    ''' <param name="intIDEmployee">Código del empleado</param>
    ''' <param name="oState"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>

    Public Function GetShiftsByEmployeeVisibilityPermissions(ByVal intIDEmployee As Integer, ByVal oState As roWsState, ByVal intIDGroup As Integer,
                                                                     ByVal IncludeObsoletes As Boolean, ByVal _IDAssignment As Integer, ByVal isPortal As Boolean) As roGenericVtResponse(Of DataSet) Implements IShiftSvc.GetShiftsByEmployeeVisibilityPermissions

        Return ShiftMethods.GetShiftsByEmployeeVisibilityPermissions(intIDEmployee, oState, intIDGroup, IncludeObsoletes, _IDAssignment, isPortal)

    End Function

    ''' <summary>
    ''' Devuelve la lista de horarios de vacaciones
    ''' </summary>
    ''' <param name="oState"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>

    Public Function GetHolidaysShifts(ByVal oState As roWsState, ByVal intIDGroup As Integer, ByVal IncludeObsoletes As Boolean) As roGenericVtResponse(Of DataSet) Implements IShiftSvc.GetHolidaysShifts

        Return ShiftMethods.GetHolidaysShifts(oState, intIDGroup, IncludeObsoletes)

    End Function

    ''' <summary>
    ''' Obtiene los horarios del grupo de horarios (Shifts).<br />
    ''' </summary>
    ''' <param name="intIDGroup">Id del grupo de horarios. Si és -1, no aplica filtro de grupo.</param>
    ''' <param name="_IDAssignment">Código del puesto. Si és -1 no aplica filtro de puesto. Sólo filtra si no es Live eXpress.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <param name="IncludeObsoletes">Incluye plantillas obsoletas?</param>
    ''' <returns>DataSet (ID, Name, Color, ShortName)</returns>
    ''' <remarks></remarks>

    Public Function GetShiftsAssignmentPlanification(ByVal intIDGroup As Integer, ByVal _IDAssignment As Integer, ByVal oState As roWsState, ByVal IncludeObsoletes As Boolean) As roGenericVtResponse(Of DataSet) Implements IShiftSvc.GetShiftsAssignmentPlanification

        Return ShiftMethods.GetShiftsAssignmentPlanification(intIDGroup, _IDAssignment, oState, IncludeObsoletes)
    End Function

    ''' <summary>
    ''' Obtiene las plantillas de horarios (Shift->Template=1).
    ''' </summary>
    ''' <param name="IncludeObsoletes">Incluye plantillas obsoletas?</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>DataSet (ID, Name, Color, ShortName)</returns>
    ''' <remarks></remarks>

    Public Function GetSchemas(ByVal IncludeObsoletes As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet) Implements IShiftSvc.GetSchemas

        Return ShiftMethods.GetSchemas(IncludeObsoletes, oState)
    End Function

    ''' <summary>
    ''' Obtiene las plantillas de horarios (Shift->Template=1).
    ''' </summary>
    ''' <param name="IncludeObsoletes">Incluye plantillas obsoletas?</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>DataSet (ID, Name, Color, ShortName)</returns>
    ''' <remarks></remarks>

    Public Function GetSchemasPlanification(ByVal IncludeObsoletes As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet) Implements IShiftSvc.GetSchemasPlanification

        Return ShiftMethods.GetSchemasPlanification(IncludeObsoletes, oState)
    End Function

    ''' <summary>
    ''' Recupera una lista de grupos de horarios (ShiftGroups)
    ''' </summary>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>DataSet (ID, Name) ordenado por Name</returns>
    ''' <remarks></remarks>

    Public Function GetShiftGroups(ByVal oState As roWsState) As roGenericVtResponse(Of DataSet) Implements IShiftSvc.GetShiftGroups

        Return ShiftMethods.GetShiftGroups(oState)
    End Function

    ''' <summary>
    ''' Recupera una lista de grupos de horarios (ShiftGroups)
    ''' </summary>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>DataSet (ID, Name) ordenado por Name</returns>
    ''' <remarks></remarks>

    Public Function GetShiftGroupsPlanification(ByVal oState As roWsState) As roGenericVtResponse(Of DataSet) Implements IShiftSvc.GetShiftGroupsPlanification

        Return ShiftMethods.GetShiftGroupsPlanification(oState)
    End Function


    ''' <summary>
    ''' Recupera una lista de grupos de horarios (ShiftGroups)
    ''' </summary>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>DataSet (ID, Name) ordenado por Name</returns>
    ''' <remarks></remarks>

    Public Function GetShiftsFromGroup(ByVal IDGroup As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet) Implements IShiftSvc.GetShiftsFromGroup

        Return ShiftMethods.GetShiftsFromGroup(IDGroup, oState)
    End Function


    ''' <summary>
    ''' Recupera un horario
    ''' </summary>
    ''' <param name="ID">ID del horario a recuperar</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Horario (roShift)</returns>
    ''' <remarks></remarks>

    Public Function GetShift(ByVal ID As Integer, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of roShift) Implements IShiftSvc.GetShift
        Return ShiftMethods.GetShift(ID, oState, bAudit)
    End Function

    ''' <summary>
    ''' Recupera un grupo de horario
    ''' </summary>
    ''' <param name="ID">ID del grupo de horario a recuperar</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Grupo de Horario (roShiftGroup)</returns>
    ''' <remarks></remarks>

    Public Function GetShiftGroup(ByVal ID As Integer, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of roShiftGroup) Implements IShiftSvc.GetShiftGroup

        Return ShiftMethods.GetShiftGroup(ID, oState, bAudit)
    End Function

    ''' <summary>
    ''' Recupera un horario (Licencia Express)
    ''' </summary>
    ''' <param name="ID">ID del horario a recuperar</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Horario (roShiftExpress)</returns>
    ''' <remarks></remarks>

    Public Function GetShiftExpress(ByVal ID As Integer, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of roShiftExpress) Implements IShiftSvc.GetShiftExpress

        Return ShiftMethods.GetShiftExpress(ID, oState, bAudit)

    End Function

    ''' <summary>
    ''' Comprueba si el usuario tiene permiso de escritura sobre el horario especificado
    ''' </summary>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <param name="intIDShift">ID del horario a comprobar</param>
    ''' <returns>true si tiene acceso de escritura</returns>
    ''' <remarks></remarks>

    Public Function ShiftIsAllowed(ByVal oState As roWsState, ByVal intIDShift As Integer) As roGenericVtResponse(Of Boolean) Implements IShiftSvc.ShiftIsAllowed

        Return ShiftMethods.ShiftIsAllowed(oState, intIDShift)
    End Function

    ''' <summary>
    ''' Comprueba si el usuario tiene permiso de escritura sobre los horarios especificados.Si se pasa algún horario con el valor -1, ese horario no se comprueba 
    ''' </summary>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <param name="intIDShift">ID del horario a comprobar</param>
    ''' <returns>true si tiene acceso de escritura</returns>
    ''' <remarks></remarks>

    Public Function ShiftsAreAllowed(ByVal oState As roWsState, ByVal intIDShift As Integer, ByVal intIDShift2 As Integer, ByVal intIDShift3 As Integer, ByVal intIDShift4 As Integer) As roGenericVtResponse(Of Boolean) Implements IShiftSvc.ShiftsAreAllowed
        Return ShiftMethods.ShiftsAreAllowed(oState, intIDShift, intIDShift2, intIDShift3, intIDShift4)
    End Function


    ''' <summary>
    ''' Guarda un horario <br />
    ''' Ejecuta ValidateShift() <br />
    ''' Realiza:<br/>
    ''' - Si es una modificación y han indicado fecha, duplica el horario<br />
    ''' - Si es una modificación y no han indicado fecha, actualizamos hasta fecha congelación<br />
    ''' - Calcula limites (CalculateLimits)<br />
    ''' - Notificamos al servidor (Task.SHIFTS)<br />
    ''' * Esta acción queda auditada<br />
    ''' </summary>
    ''' <param name="oShift">Horario (roShift) a guardar</param>
    ''' <param name="bolCheckVacationsEmpty">Si el horario es de tipo vacaciones y tiene datos definidos se devolverá un error.</param>
    ''' <param name="bAudit"></param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Devuelve TRUE si se ha podido realizar la acción</returns>
    ''' <remarks></remarks>

    Public Function SaveShift(ByVal oShift As roShift, ByVal bolCheckVacationsEmpty As Boolean, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of roShift) Implements IShiftSvc.SaveShift

        Return ShiftMethods.SaveShift(oShift, bolCheckVacationsEmpty, oState, bAudit)
    End Function

    ''' <summary>
    ''' Crea una copia de un horario existente.
    ''' </summary>
    ''' <param name="_IDSourceShift">Código del horario del que se quiere realizar la copia</param>
    ''' <param name="_NewName">Nombre del nuevo horario creado. Si no se informa, se utiliza el tag de idioma 'Shifts.ShiftSave.Copy' para generar el nuevo nombre (copia de ...).</param>
    ''' <param name="oState"></param>
    ''' <returns>Nuevo horario creado</returns>
    ''' <remarks></remarks>

    Public Function CopyShift(ByVal _IDSourceShift As Integer, ByVal _NewName As String, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of roShift) Implements IShiftSvc.CopyShift

        Return ShiftMethods.CopyShift(_IDSourceShift, _NewName, oState, bAudit)
    End Function


    ''' <summary>
    ''' Guarda un grupo de horario <br />
    ''' Ejecuta ValidateShiftGroup() <br />
    ''' Realiza:<br/>
    ''' * Esta acción queda auditada<br />
    ''' </summary>
    ''' <param name="oShiftGroup">Grupo de Horario (roShiftGroup) a guardar</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Devuelve TRUE si se ha podido realizar la acción</returns>
    ''' <remarks></remarks>

    Public Function SaveShiftGroup(ByVal oShiftGroup As roShiftGroup, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of roShiftGroup) Implements IShiftSvc.SaveShiftGroup

        Return ShiftMethods.SaveShiftGroup(oShiftGroup, oState, bAudit)

    End Function

    ''' <summary>
    ''' Guarda un horario (Licencia Express)
    ''' Ejecuta ValidateShift() <br />
    ''' Realiza:<br/>
    ''' - Si es una modificación y han indicado fecha, duplica el horario<br />
    ''' - Si es una modificación y no han indicado fecha, actualizamos hasta fecha congelación<br />
    ''' - Calcula limites (CalculateLimits)<br />
    ''' - Notificamos al servidor (Task.SHIFTS)<br />
    ''' * Esta acción queda auditada<br />
    ''' </summary>
    ''' <param name="oShift">Horario (roShiftExpress) a guardar</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Devuelve TRUE si se ha podido realizar la acción</returns>
    ''' <remarks>Si devuelve FALSE, en oState.ResultEnum devuelve el motivo</remarks>

    Public Function SaveShiftExpress(ByVal oShift As roShiftExpress, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of roShiftExpress) Implements IShiftSvc.SaveShiftExpress

        Return ShiftMethods.SaveShiftExpress(oShift, oState, bAudit)
    End Function

    ''' <summary>
    ''' Elimina franja horaria por ID<br />
    ''' Comprueba que:<br />
    ''' - No este asignado en zonas horarias (sysroShiftTimeZones)
    ''' </summary>
    ''' <param name="IDTimeZone">ID de franja horaria</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Devuelve TRUE si se ha podido realizar la acción</returns>
    ''' <remarks>Si devuelve FALSE, en oState.ResultEnum devuelve el motivo</remarks>

    Public Function DeleteTimeZoneByID(ByVal IDTimeZone As Integer, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean) Implements IShiftSvc.DeleteTimeZoneByID

        Return ShiftMethods.DeleteTimeZoneByID(IDTimeZone, oState, bAudit)

    End Function

    ''' <summary>
    ''' Elimina horario por ID <br />
    ''' Realiza: <br />
    ''' - Actualiza DailySchedule (Todos los horarios)<br />
    ''' - Borra capas de horarios (sysroShiftLayers)<br />
    ''' - Borra franjas horarios (sysroShiftTimeZones)<br />
    ''' - Borra justificaciones de horarios (sysroShiftsCausesRules)<br />
    ''' - Borra horarios (Shifts)
    ''' </summary>
    ''' <param name="IDShift">ID del horario a eliminar</param>
    ''' <param name="ostate">Información adicional de estado.</param>
    ''' <returns>Devuelve TRUE si se ha podido realizar la acción</returns>
    ''' <remarks>Si devuelve FALSE, en oState.ResultEnum devuelve el motivo</remarks>

    Public Function DeleteShift(ByVal IDShift As Integer, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean) Implements IShiftSvc.DeleteShift
        Return ShiftMethods.DeleteShift(IDShift, oState, bAudit)
    End Function

    ''' <summary>
    ''' Elimina el grupo de horario por ID <br />
    ''' Realiza: <br />
    ''' </summary>
    ''' <param name="IDShiftgroup">ID del grupo de horario a eliminar</param>
    ''' <param name="ostate">Información adicional de estado.</param>
    ''' <returns>Devuelve TRUE si se ha podido realizar la acción</returns>
    ''' <remarks>Si devuelve FALSE, en oState.ResultEnum devuelve el motivo</remarks>

    Public Function DeleteShiftGroup(ByVal IDShiftGroup As Integer, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean) Implements IShiftSvc.DeleteShiftGroup

        Return ShiftMethods.DeleteShiftGroup(IDShiftGroup, oState, bAudit)
    End Function

    ''' <summary>
    ''' Elimina horario por ID (Licencia Express)<br />
    ''' Realiza: <br />
    ''' - Actualiza DailySchedule (Todos los horarios)<br />
    ''' - Borra capas de horarios (sysroShiftLayers)<br />
    ''' - Borra franjas horarios (sysroShiftTimeZones)<br />
    ''' - Borra justificaciones de horarios (sysroShiftsCausesRules)<br />
    ''' - Borra horarios (Shifts)''' 
    ''' </summary>
    ''' <param name="IDShift">ID de horario</param>
    ''' <param name="ostate">Información adicional de estado.</param>
    ''' <returns>Devuelve TRUE si se ha podido realizar la acción</returns>
    ''' <remarks>Si devuelve FALSE, en oState.ResultEnum devuelve el motivo</remarks>

    Public Function DeleteShiftExpress(ByVal IDShift As Integer, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean) Implements IShiftSvc.DeleteShiftExpress

        Return ShiftMethods.DeleteShiftExpress(IDShift, oState, bAudit)
    End Function

    ''' <summary>
    ''' Crea una capa vacia (roShiftLayer)
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>

    Public Function CreateEmptyLayer() As roGenericVtResponse(Of roShiftLayer) Implements IShiftSvc.CreateEmptyLayer

        Return ShiftMethods.CreateEmptyLayer()
    End Function

    ''' <summary>
    ''' Recupera los tipos de capas para franjas horarias
    ''' </summary>
    ''' <returns>Cadena con los tipos de capas separadas por comas</returns>
    ''' <remarks></remarks>

    Public Function GetChildLayerTypes() As roGenericVtResponse(Of String) Implements IShiftSvc.GetChildLayerTypes

        Return ShiftMethods.GetChildLayerTypes()
    End Function

    ''' <summary>
    ''' Comprueba si el horario esta en uso (DailySchedule)
    ''' </summary>
    ''' <param name="ID">ID de horario a comprobar</param>
    ''' <param name="State">Información adicional de estado.</param>
    ''' <returns>Devuelve TRUE si se ha podido realizar la acción</returns>
    ''' <remarks>Si devuelve FALSE, en oState.ResultEnum devuelve el motivo</remarks>

    Public Function ShiftIsUsed(ByVal ID As Integer, ByVal State As roWsState) As roGenericVtResponse(Of Boolean) Implements IShiftSvc.ShiftIsUsed

        Return ShiftMethods.ShiftIsUsed(ID, State)

    End Function

    ''' <summary>
    ''' Devuelve la fecha más antigua de las que son usadas por el horario informado
    ''' </summary>
    ''' <param name="intID">ID de horario</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Fecha más antigua</returns>
    ''' <remarks></remarks>

    Public Function GetShiftOldestDate(ByVal intID As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of Date) Implements IShiftSvc.GetShiftOldestDate
        Return ShiftMethods.GetShiftOldestDate(intID, oState)
    End Function

    ''' <summary>
    ''' Devuelve las zonas horarias definidas.
    ''' </summary>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>DataSet con un DataTable (ID, Name, Description)</returns>
    ''' <remarks></remarks>

    Public Function GetTimeZones(ByVal oState As roWsState) As roGenericVtResponse(Of DataSet) Implements IShiftSvc.GetTimeZones
        Return ShiftMethods.GetTimeZones(oState)

    End Function

    ''' <summary>
    ''' Guarda una franja horaria
    ''' Ejecuta ValidateTimeZone(): Nombre
    ''' 
    ''' </summary>
    ''' <param name="oTimeZone">Franja horaria a guardar</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Devuelve TRUE si se ha podido realizar la acción</returns>
    ''' <remarks>Si devuelve FALSE, en oState.ResultEnum devuelve el motivo</remarks>

    Public Function SaveTimeZone(ByVal oTimeZone As roTimeZone, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of roTimeZone) Implements IShiftSvc.SaveTimeZone

        Return ShiftMethods.SaveTimeZone(oTimeZone, oState, bAudit)
    End Function


    ''' <summary>
    ''' Devuelve descripciones de las Reglas de justificación de horarios
    ''' </summary>
    ''' <param name="oShiftRule">Regla de justificación (roShiftRule)</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Descripción correspondiente a las reglas de justificación de horarios</returns>
    ''' <remarks></remarks>

    Public Function GetShiftRulesDescriptions(ByVal oShiftRule As roShiftRule, ByVal oState As roWsState) As roGenericVtResponse(Of String) Implements IShiftSvc.GetShiftRulesDescriptions

        Return ShiftMethods.GetShiftRulesDescriptions(oShiftRule, oState)

    End Function

    ''' <summary>
    ''' Validación de las capas de horarios<br />
    ''' ValidateFlexibleLayers (Flexibles)-> comprueba inicio/fin, tiempo max., minimo, solapan<br />
    ''' ValidateMandatoryLayers(Rigidas)-> comprueba inicio/fin flotante<br />
    ''' ValidateBreakLayers(Descansos)->comprueba inicio/fin, capa rigida que englobe, no solape con otro descanso <br />
    ''' ValidateFilters(Filtros)-> comprobación de los filtros<br />
    ''' </summary>
    ''' <param name="oShift">Horario a validar las capas</param>
    ''' <param name="ErrID">ID de Error</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Devuelve TRUE si se ha podido realizar la acción</returns>
    ''' <remarks>Si devuelve FALSE, en oState.ResultEnum devuelve el motivo</remarks>

    Public Function ValidateShiftLayers(ByVal oShift As roShift, ByVal ErrID As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements IShiftSvc.ValidateShiftLayers

        Return ShiftMethods.ValidateShiftLayers(oShift, ErrID, oState)

    End Function

    ''' <summary>
    ''' Validación del horario<br />
    ''' Comprueba que:<br />
    ''' - El nombre no este en uso<br />
    ''' - Validación de las franjas horarias (layers)<br />
    ''' - Validación de las zonas horarias<br />
    ''' </summary>
    ''' <param name="oShift">Horario a validar (roShift)</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Devuelve TRUE si se ha podido realizar la acción</returns>
    ''' <remarks>Si devuelve FALSE, en oState.ResultEnum devuelve el motivo</remarks>

    Public Function ValidateShift(ByVal oShift As roShift, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements IShiftSvc.ValidateShift

        Return ShiftMethods.ValidateShift(oShift, oState)

    End Function

    ''' <summary>
    ''' Recuperar IDs de Reglas de justificación
    ''' </summary>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Devuelve una lista de IDs de reglas de justificación</returns>
    ''' <remarks></remarks>

    Public Function GetIDShiftsCausesRules(ByVal oState As roWsState) As roGenericVtResponse(Of Generic.List(Of Integer)) Implements IShiftSvc.GetIDShiftsCausesRules

        Return ShiftMethods.GetIDShiftsCausesRules(oState)
    End Function

    ''' <summary>
    ''' Devuelve el nombre del horario incluyendo la hora de inici del flotante especificada.
    ''' </summary>
    ''' <param name="intIDShift">Código del horario.</param>
    ''' <param name="xStartShift">Hora de inicio del horario flotante.</param>
    ''' <param name="oState"></param>
    ''' <returns></returns>
    ''' <remarks>Si el horario no es de flotante, devuelve el nombre del horario.</remarks>

    Public Function FloatingShiftName(ByVal intIDShift As Integer, ByVal xStartShift As DateTime, ByVal oState As roWsState) As roGenericVtResponse(Of String) Implements IShiftSvc.FloatingShiftName

        Return ShiftMethods.FloatingShiftName(intIDShift, xStartShift, oState)
    End Function

    ''' <summary>
    ''' Devuelve el nombre del horario incluyendo la hora de inici del flotante especificada.
    ''' </summary>

    Public Function GetBusinessGroupFromShiftGroups(ByVal oState As roWsState) As roGenericVtResponse(Of DataSet) Implements IShiftSvc.GetBusinessGroupFromShiftGroups

        Return ShiftMethods.GetBusinessGroupFromShiftGroups(oState)
    End Function

    ''' <summary>
    ''' Devuelve boolean indicando si algun otro grupo de horario tiene asignado el business group.
    ''' </summary>

    Public Function BusinessGroupListInUse(ByVal oState As roWsState, ByVal strBusinessGroup As String, ByVal idGroup As Integer) As roGenericVtResponse(Of Boolean) Implements IShiftSvc.BusinessGroupListInUse

        Return ShiftMethods.BusinessGroupListInUse(oState, strBusinessGroup, idGroup)

    End Function

    ''' <summary>
    ''' Devuelve dataset con los horarios y totales planificados a un empleado en un año
    ''' </summary>

    Public Function GetShiftsTotalsByEmployee(ByVal oState As roWsState, ByVal Ejercicio As Integer, ByVal IDGroup As Integer, ByVal IDEmployee As Integer) As roGenericVtResponse(Of DataSet) Implements IShiftSvc.GetShiftsTotalsByEmployee

        Return ShiftMethods.GetShiftsTotalsByEmployee(oState, Ejercicio, IDGroup, IDEmployee)

    End Function

    ''' <summary>
    ''' Devuelve dataset con los horarios y totales planificados a un empleado en un año y un contrato
    ''' </summary>

    Public Function GetShiftsTotalsByEmployeeAndContract(ByVal oState As roWsState, ByVal Ejercicio As Integer, ByVal IDGroup As Integer, ByVal IDEmployee As Integer, ByVal IdContract As String) As roGenericVtResponse(Of DataSet) Implements IShiftSvc.GetShiftsTotalsByEmployeeAndContract

        Return ShiftMethods.GetShiftsTotalsByEmployeeAndContract(oState, Ejercicio, IDGroup, IDEmployee, IdContract)

    End Function


End Class
