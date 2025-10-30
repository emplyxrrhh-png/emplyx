Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Security.Base

Public Class SecurityOptionMethods

    ''' <summary>
    ''' Obtiene la definición de la configuracion de seguridad de un passport
    ''' </summary>
    ''' <param name="intIDPassport">ID Pasaporte.</param>
    ''' <param name="bAudit">Boleano indicador de la necesidad de auditar la acción.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Objeto 'roSecurityOptions' con la información de la configuracion de seguridad de un passport.</returns>
    ''' <remarks></remarks>
    Public Shared Function GetSecurityOptions(ByVal intIDPassport As Integer, ByVal bAudit As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of roSecurityOptions)
        Dim bState = New roSecurityOptionsState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of roSecurityOptions)
        oResult.Value = New roSecurityOptions(intIDPassport, bState, bAudit)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Obtiene la definición de la configuracion de seguridad de un passport
    ''' </summary>
    ''' <param name="oSecurityOptions">Objeto 'roSecurityOptions' con la información de la configuracion de seguridad de un passport.</param>
    ''' <param name="bAudit">Boleano indicador de la necesidad de auditar la acción.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Objeto boleano indicando si se ha podido guardar.</returns>
    ''' <remarks></remarks>
    Public Shared Function SaveSecurityOptions(ByVal oSecurityOptions As roSecurityOptions, ByVal bAudit As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)

        'cambio mi state genérico a un estado especifico
        Dim bState = New roSecurityOptionsState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)
        oResult.Value = oSecurityOptions.Save(bAudit, bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult

    End Function

    ''' <summary>
    ''' Obtiene si la contraseña no coincide con ninguna de las anteriores almacenadas
    ''' </summary>
    ''' <param name="intIDPassport">ID Pasaporte.</param>
    ''' <param name="strPwd">Información adicional de estado.</param>
    ''' <param name="PreviousPasswordsStored">Numero de contraseñas previas almacenadas.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Boolean.</returns>
    ''' <remarks></remarks>
    Public Shared Function IsValidPwdHistory(ByVal intIDPassport As Integer, ByVal strPwd As String, ByVal PreviousPasswordsStored As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)

        Dim bState = New roSecurityOptionsState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)
        oResult.Value = roSecurityOptions.IsValidPwdHistory(intIDPassport, strPwd, PreviousPasswordsStored, bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult

    End Function

    ''' <summary>
    ''' Obtiene la definición de la configuracion de seguridad heredada por un passport
    ''' </summary>
    ''' <param name="intIDPassport">ID Pasaporte.</param>
    ''' <param name="bAudit">Boleano indicador de la necesidad de auditar la acción.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Objeto 'roSecurityOptions' con la información de la configuracion de seguridad de un passport.</returns>
    ''' <remarks></remarks>
    Public Shared Function GetInheritedSecurityOptions(ByVal intIDPassport As Integer, ByVal bAudit As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of roSecurityOptions)

        'cambio mi state genérico a un estado especifico
        Dim bState = New roSecurityOptionsState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of roSecurityOptions)
        oResult.Value = roSecurityOptions.GetInheritedSecurityOptions(intIDPassport, bState, bAudit)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

End Class