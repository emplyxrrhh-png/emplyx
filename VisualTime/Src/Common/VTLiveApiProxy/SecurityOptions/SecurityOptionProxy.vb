Imports Robotics.Base
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Security.SecurityOptions

Public Class SecurityOptionProxy
    Implements ISecurityOptionSvc

    Public Function KeepAlive() As Boolean Implements ISecurityOptionSvc.KeepAlive
        Return True
    End Function

    ''' <summary>
    ''' Obtiene la definición de la configuracion de seguridad de un passport
    ''' </summary>
    ''' <param name="intIDPassport">ID Pasaporte.</param>
    ''' <param name="bAudit">Boleano indicador de la necesidad de auditar la acción.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Objeto 'roSecurityOptions' con la información de la configuracion de seguridad de un passport.</returns>
    ''' <remarks></remarks>
    Public Function GetSecurityOptions(ByVal intIDPassport As Integer, ByVal bAudit As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of roSecurityOptions) Implements ISecurityOptionSvc.GetSecurityOptions
        Return SecurityOptionMethods.GetSecurityOptions(intIDPassport, bAudit, oState)
    End Function

    ''' <summary>
    ''' Obtiene la definición de la configuracion de seguridad de un passport
    ''' </summary>
    ''' <param name="oSecurityOptions">Objeto 'roSecurityOptions' con la información de la configuracion de seguridad de un passport.</param>
    ''' <param name="bAudit">Boleano indicador de la necesidad de auditar la acción.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Objeto boleano indicando si se ha podido guardar.</returns>
    ''' <remarks></remarks>
    Public Function SaveSecurityOptions(ByVal oSecurityOptions As roSecurityOptions, ByVal bAudit As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements ISecurityOptionSvc.SaveSecurityOptions

        Return SecurityOptionMethods.SaveSecurityOptions(oSecurityOptions, bAudit, oState)
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
    Public Function IsValidPwdHistory(ByVal intIDPassport As Integer, ByVal strPwd As String, ByVal PreviousPasswordsStored As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements ISecurityOptionSvc.IsValidPwdHistory

        Return SecurityOptionMethods.IsValidPwdHistory(intIDPassport, strPwd, PreviousPasswordsStored, oState)
    End Function

    ''' <summary>
    ''' Obtiene la definición de la configuracion de seguridad heredada por un passport
    ''' </summary>
    ''' <param name="intIDPassport">ID Pasaporte.</param>
    ''' <param name="bAudit">Boleano indicador de la necesidad de auditar la acción.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Objeto 'roSecurityOptions' con la información de la configuracion de seguridad de un passport.</returns>
    ''' <remarks></remarks>
    Public Function GetInheritedSecurityOptions(ByVal intIDPassport As Integer, ByVal bAudit As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of roSecurityOptions) Implements ISecurityOptionSvc.GetInheritedSecurityOptions
        Return SecurityOptionMethods.GetInheritedSecurityOptions(intIDPassport, bAudit, oState)
    End Function

End Class
