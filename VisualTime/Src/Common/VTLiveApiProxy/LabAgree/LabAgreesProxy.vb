Imports Robotics.Base
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTEmployees.LabAgree
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTUserFields.UserFields

Public Class LabAgreesProxy
    Implements ILabAgreesSvc

    Public Function KeepAlive() As Boolean Implements ILabAgreesSvc.KeepAlive
        Return True
    End Function

#Region "LabAgree"

    ''' <summary>
    ''' Obtiene el convenio (LabAgree) con el ID indicado
    ''' </summary>
    ''' <param name="ID">ID de convenio</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Devuelve el convenio (roLabAgree)</returns>
    ''' <remarks></remarks>
    Public Function GetLabAgreeByID(ByVal ID As Integer, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of roLabAgree) Implements ILabAgreesSvc.GetLabAgreeByID
        Return LabAgreesMethods.GetLabAgreeByID(ID, oState, bAudit)
    End Function

    ''' <summary>
    ''' Devuelve todos los convenios
    ''' </summary>
    ''' <param name="oState">Información adicional de estado</param>
    ''' <returns>Devuelve un DataSet con la tabla LabAgree ordenado por Name </returns>
    ''' <remarks></remarks>

    Public Function GetLabAgrees(ByVal oState As roWsState) As roGenericVtResponse(Of DataSet) Implements ILabAgreesSvc.GetLabAgrees
        Return LabAgreesMethods.GetLabAgrees(oState)
    End Function

    ''' <summary>
    ''' Valida los datos del convenio.<br/>
    ''' Comprueba que:<br/>
    ''' - El nombre no este en blanco<br/>
    ''' - El nombre no exista en convenios<br/>
    ''' - Que no hayan lineas duplicadas en las reglas de convenios<br/>
    ''' - Que no se intente modificar una regla duplicada con fechas
    ''' </summary>
    ''' <param name="oLabAgree">El convenio a validar</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Devuelve TRUE si se ha validado correctamente</returns>
    ''' <remarks>Si devuelve FALSE, en oState.ResultEnum devuelve el motivo</remarks>

    Public Function ValidateLabAgree(ByVal oLabAgree As roLabAgree, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements ILabAgreesSvc.ValidateLabAgree
        Return LabAgreesMethods.ValidateLabAgree(oLabAgree, oState)
    End Function

    ''' <summary>
    ''' Guarda los datos del convenio. Si és nuevo, se actualiza el ID del convenio pasado.<br/>
    ''' Se comprueba ValidateLabAgree()<br/>
    ''' - Añade las reglas de convenio (LabAgreeAccrualsRules)<br/>
    ''' - Añade los valores iniciales (StartupValues)
    ''' </summary>
    ''' <param name="oLabAgree">Convenio a guardar</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Devuelve TRUE si se ha guardado correctamente</returns>
    ''' <remarks>Si devuelve FALSE, en oState.ResultEnum devuelve el motivo.</remarks>

    Public Function SaveLabAgree(ByVal oLabAgree As roLabAgree, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Tuple(Of Boolean, roLabAgree)) Implements ILabAgreesSvc.SaveLabAgree
        Return LabAgreesMethods.SaveLabAgree(oLabAgree, oState, bAudit)
    End Function

    ''' <summary>
    ''' Elimina el convenio con el ID indicado<br/>
    ''' Realiza lo siguiente:<br/>
    ''' - Borra reglas de convenio (LabAgreeAccrualsRules)<br/>
    ''' - Borra valores iniciales (StartupValues)
    ''' </summary>
    ''' <param name="ID">ID de convenio a eliminar</param>
    ''' <param name="oState">Información adicional de estado</param>
    ''' <returns>Devuelve TRUE si se ha eliminado correctamente</returns>
    ''' <remarks>Si devuelve FALSE, en oState.ResultEnum devuelve el motivo.</remarks>

    Public Function DeleteLabAgree(ByVal ID As Integer, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean) Implements ILabAgreesSvc.DeleteLabAgree
        Return LabAgreesMethods.DeleteLabAgree(ID, oState, bAudit)
    End Function

    ''' <summary>
    ''' Indica si el convenio está en uso.<br/>
    ''' Verifica que:<br/>
    ''' - La regla no se este usando en ningún convenio
    ''' </summary>
    ''' <param name="ID">ID de convenio</param>
    ''' <param name="oState">Información adicional de estado</param>
    ''' <returns>Devuelve TRUE si esta en uso. en oState.ResultEnum el motivo</returns>
    ''' <remarks></remarks>

    Public Function LabAgreeIsUsed(ByVal ID As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements ILabAgreesSvc.LabAgreeIsUsed
        Return LabAgreesMethods.LabAgreeIsUsed(ID, oState)
    End Function



    ''' <summary>
    ''' Indica si alguno de los valores que hemos introducido vara las justificaciones sobrepasa el límite establecido en el convenio
    ''' </summary>
    ''' <param name="IDEmployee">ID del empleado</param>
    ''' <param name="valDate">Fecha que debemos comprobar</param>
    ''' <param name="valIdCause">Ids de las justificaciones que debemos comprobar</param>
    ''' <param name="valValue">valores de las justificaciones que hemos modificado</param>
    ''' <param name="oState">Información adicional de estado</param>
    ''' <returns>Devuelve TRUE si esta en uso. en oState.ResultEnum el motivo</returns>
    ''' <remarks></remarks>

    Public Function ValidateLabAgreeDailyCausesOnDate(ByVal IDEmployee As Integer, ByVal valDate As DateTime, ByVal valIdCause() As Integer, ByVal valValue() As Double, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements ILabAgreesSvc.ValidateLabAgreeDailyCausesOnDate
        Return LabAgreesMethods.ValidateLabAgreeDailyCausesOnDate(IDEmployee, valDate, valIdCause, valValue, oState)
    End Function

#End Region

#Region "roLabAgreeRule"

    ''' <summary>
    ''' Obtiene la regla de convenio
    ''' </summary>
    ''' <param name="ID">ID de regla de convenio</param>
    ''' <param name="oState">Información adicional de estado</param>
    ''' <returns>Devuelve la regla de convenio (roLabAgreeRule)</returns>
    ''' <remarks></remarks>

    Public Function GetLabAgreeRuleByID(ByVal ID As Integer, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of roLabAgreeRule) Implements ILabAgreesSvc.GetLabAgreeRuleByID
        Return LabAgreesMethods.GetLabAgreeRuleByID(ID, oState, bAudit)
    End Function

    ''' <summary>
    ''' Indica si la regla de convenio está en uso.<br/>
    ''' - Verifica que la regla no se esté usando en ningún convenio (LabAgree)
    ''' </summary>
    ''' <param name="ID">ID de la regla de convenio</param>
    ''' <param name="oState">Información adicional de estado</param>
    ''' <returns>Devuelve TRUE si se encuentra en uso.</returns>
    ''' <remarks></remarks>

    Public Function LabAgreeRuleIsUsed(ByVal ID As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements ILabAgreesSvc.LabAgreeRuleIsUsed
        Return LabAgreesMethods.LabAgreeRuleIsUsed(ID, oState)
    End Function

#End Region

#Region "roStartupValue"
    ''' <summary>
    ''' Obtiene el Valor inicial con el ID indicado
    ''' </summary>
    ''' <param name="ID">ID del valor inicial</param>
    ''' <param name="oState">Información adicional de estado</param>
    ''' <returns>Devuelve el valor inicial (roStartupValue)</returns>
    ''' <remarks></remarks>

    Public Function GetStartupValueByID(ByVal ID As Integer, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of roStartupValue) Implements ILabAgreesSvc.GetStartupValueByID
        Return LabAgreesMethods.GetStartupValueByID(ID, oState, bAudit)
    End Function

    ''' <summary>
    ''' Indica si el valor inicial está en uso.<br/>
    ''' Comprueba que: <br/>
    ''' - No se este utilizando en ningún convenio.
    ''' </summary>
    ''' <param name="ID">ID del valor inicial a comprobar</param>
    ''' <param name="oState">Información adicional de estado</param>
    ''' <returns>Devuelve TRUE si esta en uso</returns>
    ''' <remarks></remarks>

    Public Function StartupValueIsUsed(ByVal ID As Integer, ByVal IDLabAgree As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements ILabAgreesSvc.StartupValueIsUsed
        Return LabAgreesMethods.StartupValueIsUsed(ID, IDLabAgree, oState)
    End Function

#End Region

#Region "LabAgreeAccrualRules"

#End Region

#Region "LabAgreeStartupValues"

    ''' <summary>
    ''' Devuelve un dataset con los valores iniciales que contienen el UserField
    ''' </summary>
    ''' <param name="FieldName">Campo de la ficha</param>
    ''' <param name="oState">Información adicional de estado</param>
    ''' <returns></returns>
    ''' <remarks></remarks>

    Public Function GetStartupValuesUseUserField(ByVal FieldName As String, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet) Implements ILabAgreesSvc.GetStartupValuesUseUserField
        Return LabAgreesMethods.GetStartupValuesUseUserField(FieldName, oState)
    End Function

#End Region

#Region "LabAgreeCauseLimitValues"

    ''' <summary>
    ''' Devuelve un dataset con los valores iniciales que contienen el UserField
    ''' </summary>
    ''' <param name="FieldName">Campo de la ficha</param>
    ''' <param name="oState">Información adicional de estado</param>
    ''' <returns></returns>
    ''' <remarks></remarks>

    Public Function GetCauseLimitValuesUseUserField(ByVal FieldName As String, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet) Implements ILabAgreesSvc.GetCauseLimitValuesUseUserField
        Return LabAgreesMethods.GetCauseLimitValuesUseUserField(FieldName, oState)
    End Function

#End Region


    ''' <summary>
    ''' Devuelve un dataset con los tipos de reglas de solicitud en funcion del tipo de solicitud
    ''' </summary>
    ''' <param name="oState">Información adicional de estado</param>
    ''' <returns></returns>
    ''' <remarks></remarks>

    Public Function GetAvailableRequestType(ByVal oState As roWsState) As roGenericVtResponse(Of DataSet) Implements ILabAgreesSvc.GetAvailableRequestType
        Return LabAgreesMethods.GetAvailableRequestType(oState)
    End Function


    ''' <summary>
    ''' Devuelve un dataset con los tipos de reglas de solicitud en funcion del tipo de solicitud
    ''' </summary>
    ''' <param name="RequestType">Campo de la ficha</param>
    ''' <param name="oState">Información adicional de estado</param>
    ''' <returns></returns>
    ''' <remarks></remarks>

    Public Function GetRuleTypesByRequestType(ByVal RequestType As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet) Implements ILabAgreesSvc.GetRuleTypesByRequestType
        Return LabAgreesMethods.GetRuleTypesByRequestType(RequestType, oState)
    End Function
End Class
