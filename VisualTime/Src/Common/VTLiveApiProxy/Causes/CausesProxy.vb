Imports Robotics.Base
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.AccessGroup
Imports Robotics.Base.VTBusiness.Cause
Imports Robotics.Base.VTBusiness.Common

Public Class CausesProxy
    Implements ICausesSvc

    Public Function KeepAlive() As Boolean Implements ICausesSvc.KeepAlive
        Return True
    End Function


    ''' <summary>
    ''' Recupera justificación por ID
    ''' </summary>
    ''' <param name="IDCause">ID de justificación a recuperar</param>
    ''' <param name="oState">Información adicional de estado</param>
    ''' <returns>Devuelve una justificación (roCause)</returns>
    ''' <remarks></remarks>
    Public Function GetCauseByID(ByVal IDCause As Integer, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of roCause) Implements ICausesSvc.GetCauseByID
        Return CausesMethods.GetCauseByID(IDCause, oState, bAudit)
    End Function

    ''' <summary>
    ''' Devuelve una lista de justificaciones
    ''' </summary>
    ''' <param name="strWhere">Extension Where SQL</param>
    ''' <param name="oState">Información adicional de estado</param>
    ''' <returns>DataSet (Causes) ordenado por Name</returns>
    ''' <remarks></remarks>
    Public Function GetCauses(ByVal strWhere As String, ByVal bolFilterBusinessGroups As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet) Implements ICausesSvc.GetCauses
        Return CausesMethods.GetCauses(strWhere, bolFilterBusinessGroups, oState)
    End Function

    ''' <summary>
    ''' Devuelve la lista de justificaciones a las que tiene acceso de fichaje (pestaña fichajes de la pantalla de definición de justificaciones) el empleado indicado.
    ''' </summary>
    ''' <param name="intIDEmployee">Código del empleado</param>
    ''' <param name="oState"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetCausesByEmployeeInputPermissions(ByVal intIDEmployee As Integer, ByVal bolFilterBusinessGroups As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet) Implements ICausesSvc.GetCausesByEmployeeInputPermissions
        Return CausesMethods.GetCausesByEmployeeInputPermissions(intIDEmployee, bolFilterBusinessGroups, oState)

    End Function

    ''' <summary>
    ''' Devuelve la lista de justificaciones a las que tiene acceso de justificar (pestaña visibilidad de la pantalla de definición de justificaciones) el empleado indicado.
    ''' </summary>
    ''' <param name="intIDEmployee">Código del empleado</param>
    ''' <param name="oState"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetCausesByEmployeeVisibilityPermissions(ByVal intIDEmployee As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet) Implements ICausesSvc.GetCausesByEmployeeVisibilityPermissions
        Return CausesMethods.GetCausesByEmployeeVisibilityPermissions(intIDEmployee, oState)
    End Function

    ''' <summary>
    ''' Devuelve una lista de justificaciones (formato corto)
    ''' </summary>
    ''' <param name="oState">Información adicional de estado</param>
    ''' <returns>DataSet (Causes:ID, Causes:Name) ordenado por Name</returns>
    ''' <remarks></remarks>
    Public Function GetCausesShortList(ByVal bolFilterBusinessGroups As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet) Implements ICausesSvc.GetCausesShortList
        Return CausesMethods.GetCausesShortList(bolFilterBusinessGroups, oState)
    End Function

    ''' <summary>
    ''' Devuelve una lista de justificaciones (formato corto)
    ''' </summary>
    ''' <param name="oState">Información adicional de estado</param>
    ''' <returns>DataSet (Causes:ID, Causes:Name) ordenado por Name</returns>
    ''' <remarks></remarks>
    Public Function CausesShortListByRequestType(ByVal eRequestType As eCauseRequest, ByVal bolFilterBusinessGroups As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet) Implements ICausesSvc.CausesShortListByRequestType
        Return CausesMethods.CausesShortListByRequestType(eRequestType, bolFilterBusinessGroups, oState)
    End Function

    ''' <summary>
    ''' Devuelve el nombre del horario incluyendo la hora de inici del flotante especificada.
    ''' </summary>
    Public Function GetBusinessGroupFromCauseGroups(ByVal oState As roWsState) As roGenericVtResponse(Of DataSet) Implements ICausesSvc.GetBusinessGroupFromCauseGroups
        Return CausesMethods.GetBusinessGroupFromCauseGroups(oState)
    End Function

    ''' <summary>
    ''' Guarda la justificación <br/>
    ''' Ejecuta ValidateCause() y Task.CAUSES
    ''' </summary>
    ''' <param name="oCause">Justificación a guardar</param>
    ''' <param name="oState">Información adicional de estado</param>
    ''' <returns>Devuelve TRUE si se ha realizado la acción</returns>
    ''' <remarks>Si devuelve FALSE, en oState.ResultEnum devuelve el motivo</remarks>
    Public Function SaveCause(ByVal oCause As roCause, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of roCause) Implements ICausesSvc.SaveCause
        Return CausesMethods.SaveCause(oCause, oState, bAudit)
    End Function

    ''' <summary>
    ''' Valida la justificación<br/>
    ''' Comprueba que:<br/>
    ''' - El nombre no puede estar en blanco<br/>
    ''' - El nombre corto no puede estar en blanco<br/>
    ''' - El nombre no puede existir en la bdd para otra justificación<br/>
    ''' - El nombre corto no puede existir en la bdd para otra justificación<br/>
    ''' - Valida campos (...)<br/>
    ''' - ReaderInputCode no se este utilizando en otra justificación
    ''' </summary>
    ''' <param name="oCause"></param>
    ''' <param name="oState">Información adicional de estado</param>
    ''' <returns>Devuelve TRUE si se ha realizado la acción</returns>
    ''' <remarks>Si devuelve FALSE, en oState.ResultEnum devuelve el motivo</remarks>
    Public Function ValidateCause(ByVal oCause As roCause, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements ICausesSvc.ValidateCause
        Return CausesMethods.ValidateCause(oCause, oState)
    End Function

    ''' <summary>
    ''' Elimina la justificación por ID<br/>
    ''' Borra: <br/>
    ''' - DailyCauses<br/>
    ''' - ConceptCauses<br/>
    ''' - Causes
    ''' </summary>
    ''' <param name="IDCause">ID de justificación a eliminar</param>
    ''' <param name="oState">Información adicional de estado</param>
    ''' <returns>Devuelve TRUE si se ha realizado la acción</returns>
    ''' <remarks>Si devuelve FALSE, en oState.ResultEnum devuelve el motivo</remarks>
    Public Function DeleteCause(ByVal IDCause As Integer, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean) Implements ICausesSvc.DeleteCause
        Return CausesMethods.DeleteCause(IDCause, oState, bAudit)
    End Function

    ''' <summary>
    ''' Comprueba si la justificación esta en uso<br/>
    ''' - En algún horario (CauseUsedInShift)<br/>
    ''' - En algún acumulado (CauseUsedinAccrual)<br/>
    ''' - En alguna ausencia prolongada (CauseUsedInProgrammedAbsence)<br/>
    ''' - En alguna incidencia prevista (CauseUsedInProgrammedCause)<br/>
    ''' - En algún fichaje con incidencia por terminal (CauseUsedInPunchWithIncidence)
    ''' </summary>
    ''' <param name="IDCause"></param>
    ''' <param name="oState">Información adicional de estado</param>
    ''' <returns>Devuelve TRUE si se encuentra en uso</returns>
    ''' <remarks>Si devuelve FALSE, en oState.ResultEnum devuelve el motivo</remarks>
    Public Function CauseIsUsed(ByVal IDCause As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements ICausesSvc.CauseIsUsed
        Return CausesMethods.CauseIsUsed(IDCause, oState)
    End Function

End Class
