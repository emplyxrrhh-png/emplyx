Imports System.Data
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTBusiness.Shift
Imports Robotics.DataLayer.AccessHelper

Public Class ShiftMethods

    ''' <summary>
    ''' Obtiene los horarios del grupo de horarios (Shifts).<br />
    ''' </summary>
    ''' <param name="intIDGroup">Id del grupo de horarios. Si és -1, no aplica filtro de grupo.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <param name="IncludeObsoletes">Incluye plantillas obsoletas?</param>
    ''' <returns>DataSet (ID, Name, Color, ShortName)</returns>
    ''' <remarks></remarks>
    Public Shared Function GetShifts(ByVal intIDGroup As Integer, ByVal oState As roWsState, ByVal IncludeObsoletes As Boolean, Optional ByVal addIsRigidInfo As Boolean = False, Optional ByVal addNotifiyAtInfo As Boolean = False) As roGenericVtResponse(Of DataSet)
        'cambio mi state genérico a un estado especifico
        Dim bState = New roShiftState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim oShift As New roShift(-1, bState)
        Dim ds As DataSet = Nothing
        Dim tb As DataTable = oShift.Shifts(intIDGroup, IncludeObsoletes,,, addIsRigidInfo, addNotifiyAtInfo)
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
    ''' Obtiene los horarios del grupo de horarios (Shifts).<br />
    ''' </summary>
    ''' <param name="intIDGroup">Id del grupo de horarios. Si és -1, no aplica filtro de grupo.</param>
    ''' <param name="_IDAssignment">Código del puesto. Si és -1 no aplica filtro de puesto. Sólo filtra si no es Live eXpress.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <param name="IncludeObsoletes">Incluye plantillas obsoletas?</param>
    ''' <returns>DataSet (ID, Name, Color, ShortName)</returns>
    ''' <remarks></remarks>
    Public Shared Function GetShiftsAssignment(ByVal intIDGroup As Integer, ByVal _IDAssignment As Integer, ByVal oState As roWsState, ByVal IncludeObsoletes As Boolean) As roGenericVtResponse(Of DataSet)

        Dim bState = New roShiftState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim oShift As New roShift(-1, bState)
        Dim ds As DataSet = Nothing
        Dim tb As DataTable = oShift.Shifts(intIDGroup, IncludeObsoletes, _IDAssignment)
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
    ''' Obtiene los horarios del grupo de horarios (Shifts).<br />
    ''' </summary>
    ''' <param name="intIDGroup">Id del grupo de horarios. Si és -1, no aplica filtro de grupo.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <param name="IncludeObsoletes">Incluye plantillas obsoletas?</param>
    ''' <returns>DataSet (ID, Name, Color, ShortName)</returns>
    ''' <remarks></remarks>

    Public Shared Function GetShiftsPlanification(ByVal intIDGroup As Integer, ByVal oState As roWsState, ByVal IncludeObsoletes As Boolean) As roGenericVtResponse(Of DataSet)

        Dim bState = New roShiftState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim oShift As New roShift(-1, bState)
        Dim ds As DataSet = Nothing
        Dim tb As DataTable = oShift.ShiftsPlanification(intIDGroup, IncludeObsoletes)
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

    Public Shared Function GetShiftsPortal(ByVal intIDGroup As Integer, ByVal oState As roWsState, ByVal IncludeObsoletes As Boolean) As roGenericVtResponse(Of DataSet)

        Dim bState = New roShiftState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim oShift As New roShift(-1, bState)
        Dim ds As DataSet = Nothing
        Dim tb As DataTable = oShift.ShiftsPlanification(intIDGroup, IncludeObsoletes, -1, True)
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
    ''' Devuelve la lista de justificaciones a las que tiene acceso de justificar (pestaña visibilidad de la pantalla de definición de justificaciones) el empleado indicado.
    ''' </summary>
    ''' <param name="intIDEmployee">Código del empleado</param>
    ''' <param name="oState"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>

    Public Shared Function GetShiftsByEmployeeVisibilityPermissions(ByVal intIDEmployee As Integer, ByVal oState As roWsState, ByVal intIDGroup As Integer,
                                                                     ByVal IncludeObsoletes As Boolean, ByVal _IDAssignment As Integer, ByVal isPortal As Boolean) As roGenericVtResponse(Of DataSet)

        Dim bState = New roShiftState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim oShift As New roShift(-1, bState)
        Dim ds As DataSet = Nothing
        Dim tb As DataTable = roShift.GetShiftsByEmployeeVisibilityPermissions(intIDEmployee, bState, intIDGroup, IncludeObsoletes, _IDAssignment, isPortal)
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
    ''' Devuelve la lista de horarios de vacaciones
    ''' </summary>
    ''' <param name="oState"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>

    Public Shared Function GetHolidaysShifts(ByVal oState As roWsState, ByVal intIDGroup As Integer, ByVal IncludeObsoletes As Boolean, Optional ByVal OrderByGroup As Boolean = True, Optional bOnlyControlledByContractAnnualizedConcept As Boolean = False) As roGenericVtResponse(Of DataSet)

        Dim bState = New roShiftState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim ds As DataSet = Nothing
        Dim tb As DataTable = roShift.GetHolidaysShifts(bState, intIDGroup, IncludeObsoletes, OrderByGroup, bOnlyControlledByContractAnnualizedConcept)
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
    ''' Obtiene los horarios del grupo de horarios (Shifts).<br />
    ''' </summary>
    ''' <param name="intIDGroup">Id del grupo de horarios. Si és -1, no aplica filtro de grupo.</param>
    ''' <param name="_IDAssignment">Código del puesto. Si és -1 no aplica filtro de puesto. Sólo filtra si no es Live eXpress.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <param name="IncludeObsoletes">Incluye plantillas obsoletas?</param>
    ''' <returns>DataSet (ID, Name, Color, ShortName)</returns>
    ''' <remarks></remarks>

    Public Shared Function GetShiftsAssignmentPlanification(ByVal intIDGroup As Integer, ByVal _IDAssignment As Integer, ByVal oState As roWsState, ByVal IncludeObsoletes As Boolean) As roGenericVtResponse(Of DataSet)

        Dim bState = New roShiftState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim oShift As New roShift(-1, bState)
        Dim ds As DataSet = Nothing
        Dim tb As DataTable = oShift.ShiftsPlanification(intIDGroup, IncludeObsoletes, _IDAssignment)
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
    ''' Obtiene las plantillas de horarios (Shift->Template=1).
    ''' </summary>
    ''' <param name="IncludeObsoletes">Incluye plantillas obsoletas?</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>DataSet (ID, Name, Color, ShortName)</returns>
    ''' <remarks></remarks>

    Public Shared Function GetSchemas(ByVal IncludeObsoletes As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet)

        Dim bState = New roShiftState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim oShift As New roShift(-1, bState)
        Dim ds As DataSet = Nothing
        Dim tb As DataTable = oShift.Schemas(IncludeObsoletes)
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
    ''' Obtiene las plantillas de horarios (Shift->Template=1).
    ''' </summary>
    ''' <param name="IncludeObsoletes">Incluye plantillas obsoletas?</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>DataSet (ID, Name, Color, ShortName)</returns>
    ''' <remarks></remarks>

    Public Shared Function GetSchemasPlanification(ByVal IncludeObsoletes As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet)

        Dim bState = New roShiftState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim oShift As New roShift(-1, bState)
        Dim ds As DataSet = Nothing
        Dim tb As DataTable = oShift.SchemasPlanification(IncludeObsoletes)
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
    ''' Recupera una lista de grupos de horarios (ShiftGroups)
    ''' </summary>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>DataSet (ID, Name) ordenado por Name</returns>
    ''' <remarks></remarks>

    Public Shared Function GetShiftGroups(ByVal oState As roWsState) As roGenericVtResponse(Of DataSet)

        Dim bState = New roShiftState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim oShift As New roShift(-1, bState)
        Dim ds As DataSet = Nothing
        Dim tb As DataTable = oShift.ShiftGroups()
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
    ''' Recupera una lista de grupos de horarios (ShiftGroups)
    ''' </summary>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>DataSet (ID, Name) ordenado por Name</returns>
    ''' <remarks></remarks>

    Public Shared Function GetShiftGroupsPlanification(ByVal oState As roWsState) As roGenericVtResponse(Of DataSet)

        Dim bState = New roShiftState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim oShift As New roShift(-1, bState)
        Dim ds As DataSet = Nothing
        Dim tb As DataTable = oShift.ShiftGroupsPlanification()
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
    ''' Recupera una lista de grupos de horarios (ShiftGroups)
    ''' </summary>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>DataSet (ID, Name) ordenado por Name</returns>
    ''' <remarks></remarks>

    Public Shared Function GetShiftsFromGroup(ByVal IDGroup As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet)

        Dim bState = New roShiftState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim oShift As New roShift(-1, bState)
        Dim ds As DataSet = roShiftGroup.GetShiftsFromGroup(IDGroup, bState)

        oResult.Value = ds

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult

    End Function

    ''' <summary>
    ''' Recupera un horario
    ''' </summary>
    ''' <param name="ID">ID del horario a recuperar</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Horario (roShift)</returns>
    ''' <remarks></remarks>

    Public Shared Function GetShift(ByVal ID As Integer, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of roShift)

        Dim bState = New roShiftState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of roShift)
        oResult.Value = New roShift(ID, bState, bAudit)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Recupera un grupo de horario
    ''' </summary>
    ''' <param name="ID">ID del grupo de horario a recuperar</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Grupo de Horario (roShiftGroup)</returns>
    ''' <remarks></remarks>

    Public Shared Function GetShiftGroup(ByVal ID As Integer, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of roShiftGroup)

        Dim bState = New roShiftState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of roShiftGroup)
        oResult.Value = New roShiftGroup(ID, bState, bAudit)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult

    End Function

    ''' <summary>
    ''' Comprueba si el usuario tiene permiso de escritura sobre el horario especificado
    ''' </summary>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <param name="intIDShift">ID del horario a comprobar</param>
    ''' <returns>true si tiene acceso de escritura</returns>
    ''' <remarks></remarks>

    Public Shared Function ShiftIsAllowed(ByVal oState As roWsState, ByVal intIDShift As Integer) As roGenericVtResponse(Of Boolean)

        Dim bState = New roShiftState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)
        oResult.Value = roShift.ShiftIsAllowed(bState, intIDShift)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult

    End Function

    ''' <summary>
    ''' Comprueba si el usuario tiene permiso de escritura sobre los horarios especificados.Si se pasa algún horario con el valor -1, ese horario no se comprueba
    ''' </summary>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <param name="intIDShift">ID del horario a comprobar</param>
    ''' <returns>true si tiene acceso de escritura</returns>
    ''' <remarks></remarks>

    Public Shared Function ShiftsAreAllowed(ByVal oState As roWsState, ByVal intIDShift As Integer, ByVal intIDShift2 As Integer, ByVal intIDShift3 As Integer, ByVal intIDShift4 As Integer) As roGenericVtResponse(Of Boolean)

        Dim bState = New roShiftState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)
        oResult.Value = roShift.ShiftIsAllowed(bState, intIDShift, intIDShift2, intIDShift3, intIDShift4)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
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

    Public Shared Function SaveShift(ByVal oShift As roShift, ByVal bolCheckVacationsEmpty As Boolean, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of roShift)

        oShift.State = New roShiftState(-1)
        roWsStateManager.CopyTo(oState, oShift.State)
        oShift.State.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of roShift)
        If oShift.Save(bAudit, bolCheckVacationsEmpty) Then
            oResult.Value = oShift
        Else
            oResult.Value = Nothing
        End If

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oShift.State, newGState)
        oResult.Status = newGState
        Return oResult

    End Function

    ''' <summary>
    ''' Crea una copia de un horario existente.
    ''' </summary>
    ''' <param name="_IDSourceShift">Código del horario del que se quiere realizar la copia</param>
    ''' <param name="_NewName">Nombre del nuevo horario creado. Si no se informa, se utiliza el tag de idioma 'Shifts.ShiftSave.Copy' para generar el nuevo nombre (copia de ...).</param>
    ''' <param name="oState"></param>
    ''' <returns>Nuevo horario creado</returns>
    ''' <remarks></remarks>

    Public Shared Function CopyShift(ByVal _IDSourceShift As Integer, ByVal _NewName As String, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of roShift)

        Dim bState = New roShiftState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of roShift)

        Try
            oResult.Value = roShift.CopyShift(_IDSourceShift, _NewName, bState, bAudit)
        Catch ex As Exception
        End Try

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
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

    Public Shared Function SaveShiftGroup(ByVal oShiftGroup As roShiftGroup, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of roShiftGroup)

        oShiftGroup.State = New roShiftState(-1)
        roWsStateManager.CopyTo(oState, oShiftGroup.State)
        oShiftGroup.State.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of roShiftGroup)
        Try
            If oShiftGroup.Save(bAudit) Then
                oResult.Value = oShiftGroup
            Else
                oResult.Value = Nothing
            End If
        Catch ex As Exception

        End Try

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oShiftGroup.State, newGState)
        oResult.Status = newGState
        Return oResult
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

    Public Shared Function DeleteTimeZoneByID(ByVal IDTimeZone As Integer, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean)

        'cambio mi state genérico a un estado especifico
        Dim bState = New roShiftState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)
        Try
            oResult.Value = roTimeZone.DeleteTimeZone(IDTimeZone, bState, bAudit)
        Catch ex As Exception
        End Try

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
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

    Public Shared Function DeleteShift(ByVal IDShift As Integer, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean)

        Dim bState = New roShiftState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()
        Dim oNewShift As New roShift()
        oNewShift.State = bState
        oNewShift.ID = IDShift
        oNewShift.Load(False)
        Dim oResult As New roGenericVtResponse(Of Boolean)
        Try
            oResult.Value = oNewShift.Delete(bAudit)
        Catch ex As Exception
        End Try

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oNewShift.State, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Elimina el grupo de horario por ID <br />
    ''' Realiza: <br />
    ''' </summary>
    ''' <param name="IDShiftgroup">ID del grupo de horario a eliminar</param>
    ''' <param name="ostate">Información adicional de estado.</param>
    ''' <returns>Devuelve TRUE si se ha podido realizar la acción</returns>
    ''' <remarks>Si devuelve FALSE, en oState.ResultEnum devuelve el motivo</remarks>

    Public Shared Function DeleteShiftGroup(ByVal IDShiftGroup As Integer, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean)

        Dim bState = New roShiftState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)
        Try
            Dim oNewShiftGroup As New roShiftGroup(IDShiftGroup, bState, bAudit)
            oResult.Value = oNewShiftGroup.Delete(bAudit)
        Catch ex As Exception
        End Try

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Crea una capa vacia (roShiftLayer)
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>

    Public Shared Function CreateEmptyLayer() As roGenericVtResponse(Of roShiftLayer)

        Dim bstate = New roShiftState(-1)

        Dim oResult As New roGenericVtResponse(Of roShiftLayer)
        oResult.Value = New roShiftLayer

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bstate, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Recupera los tipos de capas para franjas horarias
    ''' </summary>
    ''' <returns>Cadena con los tipos de capas separadas por comas</returns>
    ''' <remarks></remarks>

    Public Shared Function GetChildLayerTypes() As roGenericVtResponse(Of String)

        Dim _str As String = ""
        'cambio mi state genérico a un estado especifico
        Dim bstate = New roShiftState(-1)

        _str = roLayerTypes.roLTUnitFilter
        _str = _str & " ," & roLayerTypes.roLTWorkingMaxMinFilter
        _str = _str & " ," & roLayerTypes.roLTGroupFilter
        _str = _str & " ," & roLayerTypes.roLTPaidTime
        _str = _str & " ," & roLayerTypes.roLTDailyTotalsFilter

        Dim oResult As New roGenericVtResponse(Of String)
        oResult.Value = _str

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bstate, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Comprueba si el horario esta en uso (DailySchedule)
    ''' </summary>
    ''' <param name="ID">ID de horario a comprobar</param>
    ''' <param name="State">Información adicional de estado.</param>
    ''' <returns>Devuelve TRUE si se ha podido realizar la acción</returns>
    ''' <remarks>Si devuelve FALSE, en oState.ResultEnum devuelve el motivo</remarks>

    Public Shared Function ShiftIsUsed(ByVal ID As Integer, ByVal State As roWsState) As roGenericVtResponse(Of Boolean)

        'cambio mi state genérico a un estado especifico
        Dim bState = New roShiftState(-1)
        Dim strSQL As String
        Dim oDataset As DataSet

        strSQL = "@SELECT# Count(IDEmployee) As Cuenta"
        strSQL = strSQL & " From DailySchedule "
        strSQL = strSQL & " Where IDShift1 = " & ID
        strSQL = strSQL & " Or IDShift2 = " & ID
        strSQL = strSQL & " Or IDShift3 = " & ID
        strSQL = strSQL & " Or IDShift4 = " & ID
        strSQL = strSQL & " Group By IDShift1, IDShift2, IDShift3, IDShift4 "

        Dim oResult As New roGenericVtResponse(Of Boolean)
        Try
            oDataset = CreateDataSet(strSQL)
            If oDataset.CreateDataReader.HasRows Then
                oResult.Value = True
            Else
                oResult.Value = False
            End If
        Catch ex As Exception
        End Try

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Devuelve la fecha más antigua de las que son usadas por el horario informado
    ''' </summary>
    ''' <param name="intID">ID de horario</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Fecha más antigua</returns>
    ''' <remarks></remarks>

    Public Shared Function GetShiftOldestDate(ByVal intID As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of Date)

        Dim bState = New roShiftState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Date)
        oResult.Value = roShift.GetShiftOldestDate(intID, bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Devuelve las zonas horarias definidas.
    ''' </summary>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>DataSet con un DataTable (ID, Name, Description)</returns>
    ''' <remarks></remarks>

    Public Shared Function GetTimeZones(ByVal oState As roWsState) As roGenericVtResponse(Of DataSet)

        Dim bState = New roShiftState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim oShift As New roShift(-1, bState)
        Dim ds As DataSet = Nothing
        Dim tb As DataTable = roShift.GetTimeZones(bState)
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
    ''' Guarda una franja horaria
    ''' Ejecuta ValidateTimeZone(): Nombre
    '''
    ''' </summary>
    ''' <param name="oTimeZone">Franja horaria a guardar</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Devuelve TRUE si se ha podido realizar la acción</returns>
    ''' <remarks>Si devuelve FALSE, en oState.ResultEnum devuelve el motivo</remarks>

    Public Shared Function SaveTimeZone(ByVal oTimeZone As roTimeZone, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of roTimeZone)

        oTimeZone.State = New roShiftState(-1)
        roWsStateManager.CopyTo(oState, oTimeZone.State)
        oTimeZone.State.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of roTimeZone)
        Try
            If oTimeZone.Save(bAudit) Then
                oResult.Value = oTimeZone
            Else
                oResult.Value = Nothing
            End If
        Catch ex As Exception

        End Try

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oTimeZone.State, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Devuelve descripciones de las Reglas de justificación de horarios
    ''' </summary>
    ''' <param name="oShiftRule">Regla de justificación (roShiftRule)</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Descripción correspondiente a las reglas de justificación de horarios</returns>
    ''' <remarks></remarks>

    Public Shared Function GetShiftRulesDescriptions(ByVal oShiftRule As roShiftRule, ByVal oState As roWsState) As roGenericVtResponse(Of String)

        oShiftRule.State = New roShiftState(-1)
        roWsStateManager.CopyTo(oState, oShiftRule.State)
        oShiftRule.State.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of String)
        oResult.Value = oShiftRule.Description

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oShiftRule.State, newGState)
        oResult.Status = newGState
        Return oResult

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

    Public Shared Function ValidateShiftLayers(ByVal oShift As roShift, ByVal ErrID As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)

        oShift.State = New roShiftState(-1)
        roWsStateManager.CopyTo(oState, oShift.State)
        oShift.State.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)
        Try
            oResult.Value = oShift.ValidateLayers(ErrID, False)
        Catch ex As Exception
        End Try

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oShift.State, newGState)
        oResult.Status = newGState
        Return oResult
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

    Public Shared Function ValidateShift(ByVal oShift As roShift, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)

        oShift.State = New roShiftState(-1)
        roWsStateManager.CopyTo(oState, oShift.State)
        oShift.State.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)
        Try
            oResult.Value = oShift.Validate(True)
        Catch ex As Exception
        End Try

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oShift.State, newGState)
        oResult.Status = newGState
        Return oResult

    End Function

    ''' <summary>
    ''' Devuelve el nombre del horario incluyendo la hora de inici del flotante especificada.
    ''' </summary>
    ''' <param name="intIDShift">Código del horario.</param>
    ''' <param name="xStartShift">Hora de inicio del horario flotante.</param>
    ''' <param name="oState"></param>
    ''' <returns></returns>
    ''' <remarks>Si el horario no es de flotante, devuelve el nombre del horario.</remarks>

    Public Shared Function FloatingShiftName(ByVal intIDShift As Integer, ByVal xStartShift As DateTime, ByVal oState As roWsState) As roGenericVtResponse(Of String)

        Dim bState = New roShiftState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oShift As New roShift(intIDShift, bState)

        Dim oResult As New roGenericVtResponse(Of String)
        oResult.Value = oShift.FloatingShiftName(xStartShift)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oShift.State, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Devuelve el nombre del horario incluyendo la hora de inici del flotante especificada.
    ''' </summary>

    Public Shared Function GetBusinessGroupFromShiftGroups(ByVal oState As roWsState) As roGenericVtResponse(Of DataSet)

        Dim bState = New roShiftState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim oShift As New roShift(-1, bState)
        Dim ds As DataSet = roShiftGroup.GetBusinessGroupFromShiftGroups(bState)
        oResult.Value = ds

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Devuelve boolean indicando si algun otro grupo de horario tiene asignado el business group.
    ''' </summary>

    Public Shared Function BusinessGroupListInUse(ByVal oState As roWsState, ByVal strBusinessGroup As String, ByVal idGroup As Integer) As roGenericVtResponse(Of Boolean)

        Dim bState = New roShiftState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)
        Try
            oResult.Value = roShiftGroup.BusinessGroupListInUse(bState, strBusinessGroup, idGroup)
        Catch ex As Exception
        End Try

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Devuelve dataset con los horarios y totales planificados a un empleado en un año
    ''' </summary>

    Public Shared Function GetShiftsTotalsByEmployee(ByVal oState As roWsState, ByVal Ejercicio As Integer, ByVal IDGroup As Integer, ByVal IDEmployee As Integer) As roGenericVtResponse(Of DataSet)

        Dim bState = New roShiftState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim oShift As New roShift()
        Dim ds As DataSet = oShift.GetShiftsTotalsByEmployee(Ejercicio, IDGroup, IDEmployee)
        oResult.Value = ds

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Devuelve dataset con los horarios y totales planificados a un empleado en un año y un contrato
    ''' </summary>

    Public Shared Function GetShiftsTotalsByEmployeeAndContract(ByVal oState As roWsState, ByVal Ejercicio As Integer, ByVal IDGroup As Integer, ByVal IDEmployee As Integer, ByVal IdContract As String) As roGenericVtResponse(Of DataSet)

        Dim bState = New roShiftState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim oShift As New roShift()
        Dim ds As DataSet = oShift.GetShiftsTotalsByEmployee(Ejercicio, IDGroup, IDEmployee, IdContract)
        oResult.Value = ds

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function GetExistingShortNamesAndExport(ByVal oState As roWsState) As roGenericVtResponse(Of DataSet)

        Dim bState = New roShiftState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim ds As DataSet = Nothing
        Dim tb As DataTable = roShift.GetExistingShortNamesAndExport(bState)
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

End Class