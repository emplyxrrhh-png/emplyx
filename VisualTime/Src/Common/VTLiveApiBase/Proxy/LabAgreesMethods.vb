Imports System.Data
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTEmployees.LabAgree
Imports Robotics.Base.VTUserFields.UserFields

Public Class LabAgreesMethods

#Region "LabAgree"

    ''' <summary>
    ''' Obtiene el convenio (LabAgree) con el ID indicado
    ''' </summary>
    ''' <param name="ID">ID de convenio</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Devuelve el convenio (roLabAgree)</returns>
    ''' <remarks></remarks>
    Public Shared Function GetLabAgreeByID(ByVal ID As Integer, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of roLabAgree)

        Dim oResult As New roGenericVtResponse(Of roLabAgree)
        'cambio mi state genérico a un estado especifico
        Dim bState = New roLabAgreeState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        oResult.Value = New roLabAgree(ID, bState, bAudit)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Devuelve todos los convenios
    ''' </summary>
    ''' <param name="oState">Información adicional de estado</param>
    ''' <returns>Devuelve un DataSet con la tabla LabAgree ordenado por Name </returns>
    ''' <remarks></remarks>

    Public Shared Function GetLabAgrees(ByVal oState As roWsState) As roGenericVtResponse(Of DataSet)

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim bState = New roLabAgreeState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oRet As DataSet = Nothing
        Dim tb As DataTable = roLabAgree.GetLabAgrees(bState)
        If tb IsNot Nothing Then
            If tb.DataSet Is Nothing Then
                oResult.Value = New DataSet()
                oResult.Value.Tables.Add(tb)
            Else
                oResult.Value = tb.DataSet
            End If
        End If
        Return oResult

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
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

    Public Shared Function ValidateLabAgree(ByVal oLabAgree As roLabAgree, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)

        Dim oResult As New roGenericVtResponse(Of Boolean)
        'cambio mi state genérico a un estado especifico
        Dim bState = New roLabAgreeState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        oLabAgree.State = bState
        oResult.Value = oLabAgree.Validate()

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
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

    Public Shared Function SaveLabAgree(ByVal oLabAgree As roLabAgree, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Tuple(Of Boolean, roLabAgree))

        Dim tupResult As Tuple(Of Boolean, roLabAgree)
        Dim oResult As New roGenericVtResponse(Of Tuple(Of Boolean, roLabAgree))
        Dim bState = New roLabAgreeState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        oLabAgree.State = bState
        Dim bolRet As Boolean
        bolRet = oLabAgree.Save(bAudit)

        tupResult = New Tuple(Of Boolean, roLabAgree)(bolRet, oLabAgree)

        oResult.Value = tupResult

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
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

    Public Shared Function DeleteLabAgree(ByVal ID As Integer, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean)

        Dim oResult As New roGenericVtResponse(Of Boolean)
        Dim bState = New roLabAgreeState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oLabAgree As New roLabAgree(ID, bState, False)
        oResult.Value = oLabAgree.Delete(bAudit)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
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

    Public Shared Function LabAgreeIsUsed(ByVal ID As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)

        Dim oResult As New roGenericVtResponse(Of Boolean)
        Dim bState = New roLabAgreeState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oLabAgree As New roLabAgree(ID, bState, False)
        oResult.Value = oLabAgree.IsUsed()

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
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

    Public Shared Function ValidateLabAgreeDailyCausesOnDate(ByVal IDEmployee As Integer, ByVal valDate As DateTime, ByVal valIdCause() As Integer, ByVal valValue() As Double, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)

        Dim oResult As New roGenericVtResponse(Of Boolean)
        Dim bState = New roLabAgreeState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        oResult.Value = roLabAgree.ValidateLabAgreeDailyCausesOnDate(IDEmployee, valDate, valIdCause, valValue, bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
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

    Public Shared Function GetLabAgreeRuleByID(ByVal ID As Integer, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of roLabAgreeRule)

        Dim oResult As New roGenericVtResponse(Of roLabAgreeRule)
        Dim bState = New roLabAgreeState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        oResult.Value = New roLabAgreeRule(ID, bState, bAudit)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Indica si la regla de convenio está en uso.<br/>
    ''' - Verifica que la regla no se esté usando en ningún convenio (LabAgree)
    ''' </summary>
    ''' <param name="ID">ID de la regla de convenio</param>
    ''' <param name="oState">Información adicional de estado</param>
    ''' <returns>Devuelve TRUE si se encuentra en uso.</returns>
    ''' <remarks></remarks>

    Public Shared Function LabAgreeRuleIsUsed(ByVal ID As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)

        Dim oResult As New roGenericVtResponse(Of Boolean)
        Dim bState = New roLabAgreeState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oLabAgreeRule As New roLabAgreeRule(ID, bState, False)
        oResult.Value = oLabAgreeRule.IsUsed()

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
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

    Public Shared Function GetStartupValueByID(ByVal ID As Integer, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of roStartupValue)

        Dim oResult As New roGenericVtResponse(Of roStartupValue)
        Dim bState = New roLabAgreeState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        oResult.Value = New roStartupValue(ID, bState, bAudit)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
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

    Public Shared Function StartupValueIsUsed(ByVal ID As Integer, ByVal IDLabAgree As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)

        Dim oResult As New roGenericVtResponse(Of Boolean)
        Dim bState = New roLabAgreeState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oStartupValue As New roStartupValue(ID, bState, False)
        oResult.Value = oStartupValue.IsUsed(IDLabAgree)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

#End Region

#Region "LabAgreeStartupValues"

    ''' <summary>
    ''' Devuelve un dataset con los valores iniciales que contienen el UserField
    ''' </summary>
    ''' <param name="FieldName">Campo de la ficha</param>
    ''' <param name="oState">Información adicional de estado</param>
    ''' <returns></returns>
    ''' <remarks></remarks>

    Public Shared Function GetStartupValuesUseUserField(ByVal FieldName As String, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet)

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim bState = New roLabAgreeState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oUserFieldState As New roUserFieldState(oState.IDPassport)
        Dim tb As DataTable = roUserField.GetUserFieldUsedInStartupValues(FieldName, oUserFieldState)
        If tb IsNot Nothing Then
            If tb.DataSet Is Nothing Then
                oResult.Value = New DataSet()
                oResult.Value.Tables.Add(tb)
            Else
                oResult.Value = tb.DataSet
            End If
        End If

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
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

    Public Shared Function GetCauseLimitValuesUseUserField(ByVal FieldName As String, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet)

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim bState = New roLabAgreeState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oUserFieldState As New roUserFieldState(oState.IDPassport)
        Dim tb As DataTable = roUserField.GetUserFieldUsedInCauseLimitValues(FieldName, oUserFieldState)
        If tb IsNot Nothing Then
            If tb.DataSet Is Nothing Then
                oResult.Value = New DataSet()
                oResult.Value.Tables.Add(tb)
            Else
                oResult.Value = tb.DataSet
            End If
        End If

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

#End Region

    ''' <summary>
    ''' Devuelve un dataset con los tipos de reglas de solicitud en funcion del tipo de solicitud
    ''' </summary>
    ''' <param name="oState">Información adicional de estado</param>
    ''' <returns></returns>
    ''' <remarks></remarks>

    Public Shared Function GetAvailableRequestType(ByVal oState As roWsState) As roGenericVtResponse(Of DataSet)

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim bState = New roLabAgreeState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim tb As DataTable = roRequestRule.GetAvailableRequestType(bState)
        If tb IsNot Nothing Then
            If tb.DataSet Is Nothing Then
                oResult.Value = New DataSet()
                oResult.Value.Tables.Add(tb)
            Else
                oResult.Value = tb.DataSet
            End If
        End If

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Devuelve un dataset con los tipos de reglas de solicitud en funcion del tipo de solicitud
    ''' </summary>
    ''' <param name="RequestType">Campo de la ficha</param>
    ''' <param name="oState">Información adicional de estado</param>
    ''' <returns></returns>
    ''' <remarks></remarks>

    Public Shared Function GetRuleTypesByRequestType(ByVal RequestType As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet)

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim bState = New roLabAgreeState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim tb As DataTable = roRequestRule.GetRuleTypesByRequestType(RequestType, bState)
        If tb IsNot Nothing Then
            If tb.DataSet Is Nothing Then
                oResult.Value = New DataSet()
                oResult.Value.Tables.Add(tb)
            Else
                oResult.Value = tb.DataSet
            End If
        End If

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

End Class