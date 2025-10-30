Imports Robotics.Base
Imports Robotics.Base.VTEmployees.Contract
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common

Public Class ContractsProxy
    Implements IContractsSvc

    Public Function KeepAlive() As Boolean Implements IContractsSvc.KeepAlive
        Return True
    End Function

    ''' <summary>
    ''' Recupera el Contrato por su núm. de ID.
    ''' </summary>
    ''' <param name="IDContract">ID de contrato</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Devuelve un objeto contrato (roContract)</returns>
    ''' <remarks>En caso de error: a través del parámetro de retorno 'oState' se indica el motivo.</remarks>

    Public Function GetContract(ByVal IDContract As String, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of roContract) Implements IContractsSvc.GetContract
        Return ContractsMethods.GetContract(IDContract, oState, bAudit)
    End Function

    ''' <summary>
    ''' Recupera un DataSet de Contratos (EmployeeContracts) que empiecen por el ID de contrato especificado.
    ''' </summary>
    ''' <param name="IDContract">Comienzo de ID de Contrato a buscar</param>
    ''' <param name="Feature">Nombre de la funcionalidad de seguridad del passport. Si no se informa no se realiza ninguna verificación de seguridad.</param>
    ''' <param name="Type">Tipo de la funcionalidad de seguridad: 'U' tipo usuario, 'E' tipo empleado.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Devuelve un DataSet con Datatable devuelta por la Tabla EmployeeContracts</returns>
    ''' <remarks></remarks>

    Public Function GetContractsLikeID(ByVal IDContract As String, ByVal Feature As String, ByVal Type As String, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet) Implements IContractsSvc.GetContractsLikeID
        Return ContractsMethods.GetContractsLikeID(IDContract, Feature, Type, oState)
    End Function

    ''' <summary>
    ''' Recupera un DataSet con la lista de contratos por ID Empleado.
    ''' </summary>
    ''' <param name="IDEmployee">ID de Empleado</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Devuelve un DataSet con DataTable devuelta por la Tabla EmployeeContracts y el nombre del Convenio asignado (LabAgreeName). Ordenado por Fecha Inicio (BeginDate)</returns>
    ''' <remarks>En caso de error: a través del parámetro de retorno 'oState' se indica el motivo.</remarks>

    Public Function GetContractsByIDEmployee(ByVal IDEmployee As Integer, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of DataSet) Implements IContractsSvc.GetContractsByIDEmployee
        Return ContractsMethods.GetContractsByIDEmployee(IDEmployee, oState, bAudit)
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

    Public Function GetActiveContract(ByVal IDEmployee As Integer, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of roContract) Implements IContractsSvc.GetActiveContract
        Return ContractsMethods.GetActiveContract(IDEmployee, oState, bAudit)
    End Function

    ''' <summary>
    ''' Actualiza el núm. (ID) de un contrato por otro (EmployeeContracts)
    ''' </summary>
    ''' <param name="OldIDContract">ID de contrato a actualizar</param>
    ''' <param name="NewIDContract">Nuevo ID de contrato</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Devuelve TRUE si se ha podido realizar la acción</returns>
    ''' <remarks>En caso de retorno a FALSE: a través del parámetro de retorno 'oState' se indica el motivo.</remarks>

    Public Function UpdateContractID(ByVal OldIDContract As String, ByVal NewIDContract As String, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements IContractsSvc.UpdateContractID
        Return ContractsMethods.UpdateContractID(OldIDContract, NewIDContract, oState)
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

    Public Function DeleteContract(ByVal IDContract As String, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean) Implements IContractsSvc.DeleteContract
        Return ContractsMethods.DeleteContract(IDContract, oState, bAudit)
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

    Public Function ValidateContract(ByVal oContract As roContract, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements IContractsSvc.ValidateContract
        Return ContractsMethods.ValidateContract(oContract, oState)
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

    Public Function SaveContract(ByVal oContract As roContract, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean) Implements IContractsSvc.SaveContract
        Return ContractsMethods.SaveContract(oContract, oState, bAudit)
    End Function

    ''' <summary>
    ''' Recupera un DataSet (EmployeeContracts) con las tarjetas que empiecen por el parametro pasado
    ''' </summary>
    ''' <param name="IDCard">Núm. de tarjeta (IDCard) que empiecen por</param>
    ''' <param name="Feature">Nombre de la funcionalidad de seguridad del passport. Si no se informa no se realiza ninguna verificación de seguridad.</param>
    ''' <param name="Type">Tipo de la funcionalidad de seguridad: 'U' tipo usuario, 'E' tipo empleado.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Devuelve un DataSet con Datatable devuelta por la Tabla EmployeeContracts</returns>
    ''' <remarks></remarks>

    Public Function GetCardsLikeID(ByVal IDCard As String, ByVal Feature As String, ByVal Type As String, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet) Implements IContractsSvc.GetCardsLikeID
        Return ContractsMethods.GetCardsLikeID(IDCard, Feature, Type, oState)
    End Function

    ''' <summary>
    ''' Comprueba si existe el ID de tarjeta en la fecha especificada
    ''' </summary>
    ''' <param name="strCardID">ID de tarjeta a comprobar</param>
    ''' <param name="xDay">Dia a comprobar</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Devuelve TRUE si existe la tarjeta</returns>
    ''' <remarks>Si no existe, devuelve FALSE</remarks>

    Public Function ExistsCardID(ByVal strCardID As String, ByVal xDay As DateTime, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements IContractsSvc.ExistsCardID
        Return ContractsMethods.ExistsCardID(strCardID, xDay, oState)
    End Function

    ''' <summary>
    ''' Comprueba si existe el ID de contrato.
    ''' </summary>
    ''' <param name="strContractID">ID de contrato a comprobar</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Devuelve TRUE si existe el contrato</returns>
    ''' <remarks>Si no existe, devuelve FALSE</remarks>

    Public Function ExistsContractID(ByVal strContractID As String, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements IContractsSvc.ExistsContractID
        Return ContractsMethods.ExistsContractID(strContractID, oState)
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

    Public Function ValidateCardID(ByVal strCardID As String, ByVal intIDEmployee As Integer, ByVal xBeginContract As Date, ByVal xEndContract As Date, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements IContractsSvc.ValidateCardID
        Return ContractsMethods.ValidateCardID(strCardID, intIDEmployee, xBeginContract, xEndContract, oState)
    End Function

    ''' <summary>
    ''' Devuelve el proximo ID de contrato.
    ''' </summary>
    ''' <param name="oState">Información adicional de estado</param>
    ''' <returns>Devuelve el número de contrato siguiente</returns>
    ''' <remarks></remarks>

    Public Function GetMaxIDContract(ByVal oState As roWsState) As roGenericVtResponse(Of ULong) Implements IContractsSvc.GetMaxIDContract
        Return ContractsMethods.GetMaxIDContract(oState)
    End Function
End Class
