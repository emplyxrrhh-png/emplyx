Imports System.Data
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTEmployees.Contract

Public Class ContractsMethods

    ''' <summary>
    ''' Recupera el Contrato por su núm. de ID.
    ''' </summary>
    ''' <param name="IDContract">ID de contrato</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Devuelve un objeto contrato (roContract)</returns>
    ''' <remarks>En caso de error: a través del parámetro de retorno 'oState' se indica el motivo.</remarks>

    Public Shared Function GetContract(ByVal IDContract As String, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of roContract)

        Dim oResult As New roGenericVtResponse(Of roContract)
        'cambio mi state genérico a un estado especifico
        Dim bState = New roContractState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oRet As New roContract(bState, IDContract)
        oRet.Load(bAudit)
        oResult.Value = oRet

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Recupera un DataSet con la lista de contratos por ID Empleado.
    ''' </summary>
    ''' <param name="IDEmployee">ID de Empleado</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Devuelve un DataSet con DataTable devuelta por la Tabla EmployeeContracts y el nombre del Convenio asignado (LabAgreeName). Ordenado por Fecha Inicio (BeginDate)</returns>
    ''' <remarks>En caso de error: a través del parámetro de retorno 'oState' se indica el motivo.</remarks>

    Public Shared Function GetContractsByIDEmployee(ByVal IDEmployee As Integer, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of DataSet)
        Dim oResult As New roGenericVtResponse(Of DataSet)
        'cambio mi state genérico a un estado especifico
        Dim bState = New roContractState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim ds As DataSet = Nothing
        Dim tb As DataTable = roContract.GetContractsByIDEmployee(IDEmployee, bState, bAudit)
        If tb IsNot Nothing Then
            If tb.DataSet Is Nothing Then
                ds = New DataSet
                ds.Tables.Add(tb)
            Else
                ds = tb.DataSet
            End If
        End If
        oResult.Value = ds

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function GetDatesOfEmployeePeriodByContractInDate(type As SummaryType, ByVal IDEmployee As Integer, ByVal xDate As DateTime, ByVal oState As roWsState) As roGenericVtResponse(Of List(Of Date))
        Dim oResult As New roGenericVtResponse(Of List(Of Date))
        'cambio mi state genérico a un estado especifico
        Dim bState = New roContractState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim tb As List(Of Date) = roBusinessSupport.GetPeriodDatesByContractAtDate(type, IDEmployee, xDate, bState, True)
        oResult.Value = tb

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function GetDatesOfAnnualWorkPeriodsInDate(ByVal IDEmployee As Integer, ByVal xDate As DateTime, ByVal oState As roWsState) As roGenericVtResponse(Of List(Of Date))
        Dim oResult As New roGenericVtResponse(Of List(Of Date))
        'cambio mi state genérico a un estado especifico
        Dim bState = New roContractState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim tb As List(Of Date) = roBusinessSupport.GetDatesOfAnnualWorkPeriodsInDate(IDEmployee, xDate, bState)
        oResult.Value = tb

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Recupera el contrato activo por ID de empleado.<br/>
    ''' Se recupera mirando que la fecha actual se encuentre dentro de las fechas del contrato (BeginDate y EndDate)<br/>
    ''' En caso de no encontrarse ningún contrato activo, el oState devuelve un ResultEnum.ContractNotFound
    ''' </summary>
    ''' <param name="IDEmployee">ID de Empleado a recuperar el contrato activo</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Contrato activo (roContract)</returns>
    ''' <remarks>En caso de error: a través del parámetro de retorno 'oState' se indica el motivo.</remarks>

    Public Shared Function GetActiveContract(ByVal IDEmployee As Integer, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of roContract)

        Dim oResult As New roGenericVtResponse(Of roContract)
        'cambio mi state genérico a un estado especifico
        Dim bState = New roContractState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        oResult.Value = roContract.GetActiveContract(IDEmployee, bState, bAudit)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Actualiza el núm. (ID) de un contrato por otro (EmployeeContracts)
    ''' </summary>
    ''' <param name="OldIDContract">ID de contrato a actualizar</param>
    ''' <param name="NewIDContract">Nuevo ID de contrato</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Devuelve TRUE si se ha podido realizar la acción</returns>
    ''' <remarks>En caso de retorno a FALSE: a través del parámetro de retorno 'oState' se indica el motivo.</remarks>

    Public Shared Function UpdateContractID(ByVal OldIDContract As String, ByVal NewIDContract As String, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)

        Dim oResult As New roGenericVtResponse(Of Boolean)
        'cambio mi state genérico a un estado especifico
        Dim bState = New roContractState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        oResult.Value = roContract.UpdateContractID(OldIDContract, NewIDContract, bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Elimina el contrato por núm. de contrato (ID).<br/>
    ''' Antes de eliminar el contrato se comprueba que:<br/>
    '''  - No tiene movimientos (Moves)<br/>
    ''' Elimina<br/>
    '''  - Justificaciones diarias (DailyCauses)<br/>
    '''  - Planificación (DailySchedule)<br/>
    '''  - Incidencias   (DailyIncidences)<br/>
    '''  - Acumulados    (DailyAccruals)<br/>
    ''' * Este proceso queda auditado
    ''' </summary>
    ''' <param name="IDContract">ID de contrato a eliminar</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Devuelve TRUE si se ha podido realizar la acción</returns>
    ''' <remarks>En caso de error: a través del parámetro de retorno 'oState' se indica el motivo.</remarks>

    Public Shared Function DeleteContract(ByVal IDContract As String, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean)

        Dim oResult As New roGenericVtResponse(Of Boolean)
        'cambio mi state genérico a un estado especifico
        Dim bState = New roContractState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oContract As New roContract(bState, IDContract)
        oResult.Value = oContract.Delete(bAudit)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Comrpueba si los datos del contrato son válidos.<br/>
    ''' Se comprueba:<br/>
    '''  - Que el número de contrato no exista.<br/>
    '''  - Fecha inicio sea inferior a la fecha fin<br/>
    '''  - La nueva definición del contrato los métodos de autentificación del pasaporte relacionado sean correctos
    ''' </summary>
    ''' <param name="Contract">Contracto a validar (roContract)</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Devuelve TRUE si el contrato es válido</returns>
    ''' <remarks>En caso de devolver FALSE: a través del parámetro de retorno 'oState' se indica el motivo.</remarks>

    Public Shared Function ValidateContract(ByVal oContract As roContract, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)

        Dim oResult As New roGenericVtResponse(Of Boolean)
        'cambio mi state genérico a un estado especifico
        Dim bState = New roContractState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        oContract.State = bState
        oResult.Value = oContract.ValidateContract()

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Guarda el contrato.<br/>
    ''' Se realiza:<br/>
    '''  - Validación de los datos del contrato a través de ValidateContract<br/>
    '''  - Si es el primer contrato, actualiza los movimientos anteriores a la fecha inicio del contrato (EmployeeGroups)<br/>
    '''  - Si es el primer contrato, actualiza la fecha de inicio de la primera movilidad (EmployeeGroups)<br/>
    '''  - Se lanza el proceso de recalculo (roContract.Recalculate()).<br/>
    '''  - Se actualizan los Tareas de los Terminales (roPassport.UpdateDriverTasks)<br/>
    ''' * Este proceso queda auditado
    ''' </summary>
    ''' <param name="Contract">Contrato a guardar (roContract)</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Devuelve TRUE si el contrato es válido</returns>
    ''' <remarks>En caso de devolver FALSE: a través del parámetro de retorno 'oState' se indica el motivo.</remarks>

    Public Shared Function SaveContract(ByVal oContract As roContract, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean)

        Dim oResult As New roGenericVtResponse(Of Boolean)
        'cambio mi state genérico a un estado especifico
        Dim bState = New roContractState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        oContract.State = bState
        oResult.Value = oContract.Save(bAudit)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Comprueba si existe el ID de tarjeta en la fecha especificada
    ''' </summary>
    ''' <param name="strCardID">ID de tarjeta a comprobar</param>
    ''' <param name="xDay">Dia a comprobar</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Devuelve TRUE si existe la tarjeta</returns>
    ''' <remarks>Si no existe, devuelve FALSE</remarks>

    Public Shared Function ExistsCardID(ByVal strCardID As String, ByVal xDay As DateTime, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)

        Dim oResult As New roGenericVtResponse(Of Boolean)
        'cambio mi state genérico a un estado especifico
        Dim bState = New roContractState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        oResult.Value = roContract.ExistsCardID(strCardID, xDay, bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Comprueba si existe el ID de contrato.
    ''' </summary>
    ''' <param name="strContractID">ID de contrato a comprobar</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Devuelve TRUE si existe el contrato</returns>
    ''' <remarks>Si no existe, devuelve FALSE</remarks>

    Public Shared Function ExistsContractID(ByVal strContractID As String, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)
        Dim oResult As New roGenericVtResponse(Of Boolean)
        'cambio mi state genérico a un estado especifico
        Dim bState = New roContractState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        oResult.Value = roContract.ExistsContractID(strContractID, bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Comprueba que el ID de tarjeta sea válido.<br/>
    ''' Realiza lo siguiente:<br/>
    ''' - Mira las tarjetas de empleado (EmployeeContracts->IDCard)<br/>
    ''' - Mira las tarjetas de equipos, por si estuviera en producción (Teams->IDCard)<br/>
    ''' - Mira que no la tenga asignada una justificación (Causes->ReaderInputCode, AllowInputFromReader = 1)<br/>
    ''' - Mira las tarjetas de incidencias de producción (JobIncidences->ReaderInputCode)
    ''' </summary>
    ''' <param name="strCardID">ID de tarjeta</param>
    ''' <param name="intIDEmployee">ID de empleado</param>
    ''' <param name="xBeginContract">Fecha de inicio de contrato</param>
    ''' <param name="xEndContract">Fecha fin de contrato</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Devuelve TRUE si es valida la tarjeta.</returns>
    ''' <remarks>Si es FALSE, devuelve el motivo en oState</remarks>

    Public Shared Function ValidateCardID(ByVal strCardID As String, ByVal intIDEmployee As Integer, ByVal xBeginContract As Date, ByVal xEndContract As Date, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)
        Dim oResult As New roGenericVtResponse(Of Boolean)
        'cambio mi state genérico a un estado especifico
        Dim bState = New roContractState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        oResult.Value = roContract.ValidateCardID(strCardID, intIDEmployee, xBeginContract, xEndContract, bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Devuelve el proximo ID de contrato.
    ''' </summary>
    ''' <param name="oState">Información adicional de estado</param>
    ''' <returns>Devuelve el número de contrato siguiente</returns>
    ''' <remarks></remarks>

    Public Shared Function GetMaxIDContract(ByVal oState As roWsState) As roGenericVtResponse(Of ULong)
        Dim oResult As New roGenericVtResponse(Of ULong)
        'cambio mi state genérico a un estado especifico
        Dim bState = New roContractState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        oResult.Value = roContract.GetMaxIDContract(bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

End Class