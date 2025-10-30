Imports Robotics.Base
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTBusiness.Report
Imports Robotics.Security
Imports Robotics.Security.SecurityOptions
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions

Public Class SecurityBaseProxy
    Implements ISecurityBaseSvc

    Public Function KeepAlive() As Boolean Implements ISecurityBaseSvc.KeepAlive
        Return True
    End Function
    ''' <summary>
    ''' Authenticates a user by validating it's credential and password for a specified login method.
    ''' </summary>
    ''' <param name="method">The authentication method to use.</param>
    ''' <param name="credential">Credentials: username, windows account, biometrical data, etc.</param>
    ''' <param name="password">Hashed password to validate credential.</param>
    ''' <param name="hashPassword">Wether to hash password.</param>
    ''' <returns>A passport if authentication succeeds, otherwise, nothing.</returns>

    Public Function Authenticate(ByVal method As AuthenticationMethod, ByVal credential As String, ByVal password As String, ByVal hashPassword As Boolean, ByVal version As String, ByVal oState As roWsState,
                                 ByVal strAPP As String, ByVal strVersionAPP As String, ByVal strVersionServer As String, ByVal strMail As String, ByVal isSSOLogin As Boolean, ByVal strAuthToken As String,
                                 ByVal strSecurityToken As String, ByVal bolAudit As Boolean) As roGenericVtResponse(Of (roPassport, String)) Implements ISecurityBaseSvc.Authenticate

        Return SecurityBaseMethods.Authenticate(method, credential, password, hashPassword, version, oState, strAPP, strVersionAPP, strVersionServer, strMail, isSSOLogin, strAuthToken, strSecurityToken, bolAudit)
    End Function

    ''' <summary>
    ''' Autentifica un usuario validando la información biométrica
    ''' </summary>
    ''' <param name="version">Versión del fw de la información biométrica.</param>
    ''' <param name="BiometricData">Información biomñetrica a comparar.</param>
    ''' <param name="BiometricID">Si se informa -1, se comparará con todos los ids biometricos, sinó con el especificado.</param>
    ''' <returns>El passport si la validación es correcta, o nothing si es erronea.</returns>
    ''' <param name="oState"></param>    

    Public Function AuthenticateBiometric(ByVal version As String, ByVal biometricID As Integer, ByVal biometricData() As Byte, ByVal oState As roWsState) As roGenericVtResponse(Of roPassport) Implements ISecurityBaseSvc.AuthenticateBiometric
        Return SecurityBaseMethods.AuthenticateBiometric(version, biometricID, biometricData, oState)
    End Function


    Public Function GetPassportBelongsToAdminGroup(ByVal intIdPassport As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements ISecurityBaseSvc.GetPassportBelongsToAdminGroup
        Return SecurityBaseMethods.GetPassportBelongsToAdminGroup(intIdPassport, oState)
    End Function

    ''' <summary>
    ''' Returns the permission current passport have over specified feature.
    ''' </summary>
    ''' <param name="featureAlias">The alias of the feature to check permissions for.</param>
    ''' <param name="featureType">The type of feature: 'E' for Employee or 'U' for User.</param>
    ''' <param name="oState">The result error information</param>

    Public Function GetPermissionOverFeature(ByVal featureAlias As String, ByVal featureType As String, ByVal oState As roWsState) As roGenericVtResponse(Of Permission) Implements ISecurityBaseSvc.GetPermissionOverFeature
        Return SecurityBaseMethods.GetPermissionOverFeature(featureAlias, featureType, oState)
    End Function


    Public Function SetLastNotificationSended(ByVal iPassportID As Integer, ByVal oDate As Nullable(Of DateTime), ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements ISecurityBaseSvc.SetLastNotificationSended
        Return SecurityBaseMethods.SetLastNotificationSended(iPassportID, oDate, oState)
    End Function


    Public Function UpdateLastAccessTime(ByVal sessionID As String, ByVal iPassportID As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements ISecurityBaseSvc.UpdateLastAccessTime
        Return SecurityBaseMethods.UpdateLastAccessTime(sessionID, iPassportID, oState)
    End Function



    ''' <summary>
    ''' Returns the permission current passport have over specified employee.
    ''' </summary>
    ''' <param name="idEmployee">The ID of the employee for which to get permissions.</param>
    ''' <param name="applicationAlias">The ID of the application in which to check permissions.</param>
    ''' <param name="oState">The result error information</param>

    Public Function GetPermissionOverEmployeeAppAlias(ByVal idEmployee As Integer, ByVal applicationAlias As String, ByVal featureType As String, ByVal oState As roWsState) As roGenericVtResponse(Of Permission) Implements ISecurityBaseSvc.GetPermissionOverEmployeeAppAlias
        Return SecurityBaseMethods.GetPermissionOverEmployeeAppAlias(idEmployee, applicationAlias, featureType, oState)
    End Function

    ''' <summary>
    ''' Returns the permission current passport have over specified employee.
    ''' </summary>
    ''' <param name="idEmployee">The ID of the employee for which to get permissions.</param>
    ''' <param name="applicationAlias">The ID of the application in which to check permissions.</param>
    ''' <param name="dDate">The date in which to check permissions.</param>
    ''' <param name="oState">The result error information</param>

    Public Function GetPermissionOverEmployeeOnDateAppAlias(ByVal idEmployee As Integer, ByVal applicationAlias As String, ByVal dDate As DateTime, ByVal featureType As String, ByVal oState As roWsState) As roGenericVtResponse(Of Permission) Implements ISecurityBaseSvc.GetPermissionOverEmployeeOnDateAppAlias
        Return SecurityBaseMethods.GetPermissionOverEmployeeOnDateAppAlias(idEmployee, applicationAlias, dDate, featureType, oState)
    End Function

    Public Function HasPermissionOverEmployeeOnDateEx(ByVal strEmployees() As String, ByVal applicationAlias As String, ByVal featureType As String, ByVal perm As Permission, ByVal oState As roWsState) As roGenericVtResponse(Of String()) Implements ISecurityBaseSvc.HasPermissionOverEmployeeOnDateEx
        Return SecurityBaseMethods.HasPermissionOverEmployeeOnDateEx(strEmployees, applicationAlias, featureType, perm, oState)
    End Function




    Public Function HasPermissionOverGroupAppAliasEx(ByVal strGroups() As String, ByVal applicationAlias As String, ByVal featureType As String, ByVal perm As Permission, ByVal oState As roWsState) As roGenericVtResponse(Of String()) Implements ISecurityBaseSvc.HasPermissionOverGroupAppAliasEx
        Return SecurityBaseMethods.HasPermissionOverGroupAppAliasEx(strGroups, applicationAlias, featureType, perm, oState)
    End Function

    ''' <summary>
    ''' Returns the permission current passport have over specified employee.
    ''' </summary>
    ''' <param name="idEmployee">The ID of the employee for which to get permissions.</param>
    ''' <param name="idApplication">The ID of the application in which to check permissions.</param>
    ''' <param name="oState">The result error information</param>

    Public Function GetPermissionOverEmployeeAppId(ByVal idEmployee As Integer, ByVal idApplication As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of Permission) Implements ISecurityBaseSvc.GetPermissionOverEmployeeAppId
        Return SecurityBaseMethods.GetPermissionOverEmployeeAppId(idEmployee, idApplication, oState)
    End Function

    ''' <summary>
    ''' Returns the permission current passport have over specified group.
    ''' </summary>
    ''' <param name="idGroup">The ID of the group for which to get permissions.</param>
    ''' <param name="applicationAlias">The alias of the application in which to check permissions.</param>
    ''' <param name="oState">The result error information</param>

    Public Function GetPermissionOverGroupAppAlias(ByVal idGroup As Integer, ByVal applicationAlias As String, ByVal featureType As String, ByVal oState As roWsState) As roGenericVtResponse(Of Permission) Implements ISecurityBaseSvc.GetPermissionOverGroupAppAlias
        Return SecurityBaseMethods.GetPermissionOverGroupAppAlias(idGroup, applicationAlias, featureType, oState)
    End Function

    ''' <summary>
    ''' Returns the permission current passport have over specified group.
    ''' </summary>
    ''' <param name="idGroup">The ID of the group for which to get permissions.</param>
    ''' <param name="idApplication">The ID of the application in which to check permissions.</param>
    ''' <param name="oState">The result error information</param>

    Public Function GetPermissionOverGroupAppId(ByVal idGroup As Integer, ByVal idApplication As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of Permission) Implements ISecurityBaseSvc.GetPermissionOverGroupAppId
        Return SecurityBaseMethods.GetPermissionOverGroupAppId(idGroup, idApplication, oState)
    End Function

    ''' <summary>
    ''' Returns whether current passport have specified permission over 
    ''' specified feature.
    ''' </summary>
    ''' <param name="featureAlias">The alias of the feature to check permissions for.</param>
    ''' <param name="featureType">The type of feature: 'E' for Employee or 'U' for User.</param>
    ''' <param name="perm">The required permission.</param>
    ''' <param name="oState">The result error information</param>

    Public Function HasPermissionOverFeature(ByVal featureAlias As String, ByVal featureType As String, ByVal perm As Permission, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements ISecurityBaseSvc.HasPermissionOverFeature
        Return SecurityBaseMethods.HasPermissionOverFeature(featureAlias, featureType, perm, oState)
    End Function

    ''' <summary>
    ''' Returns whether current passport have specified permission
    ''' over specified employee.
    ''' </summary>
    ''' <param name="idEmployee">The ID of the employee for which to get permissions.</param>
    ''' <param name="applicationAlias">The alias of the application in which to check permissions.</param>
    ''' <param name="perm">The required permission.</param>
    ''' <param name="oState">The result error information</param>

    Public Function HasPermissionOverEmployee(ByVal idEmployee As Integer, ByVal applicationAlias As String, ByVal featureType As String, ByVal perm As Permission, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements ISecurityBaseSvc.HasPermissionOverEmployee
        Return SecurityBaseMethods.HasPermissionOverEmployee(idEmployee, applicationAlias, featureType, perm, oState)
    End Function

    ''' <summary>
    ''' Returns whether current passport have specified permission
    ''' over specified employee.
    ''' </summary>
    ''' <param name="idEmployee">The ID of the employee for which to get permissions.</param>
    ''' <param name="applicationAlias">The alias of the application in which to check permissions.</param>
    ''' <param name="perm">The required permission.</param>
    ''' <param name="oState">The result error information</param>

    Public Function HasPermissionOverEmployeeOnDate(ByVal idEmployee As Integer, ByVal applicationAlias As String, ByVal featureType As String, ByVal perm As Permission, ByVal dDate As DateTime, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements ISecurityBaseSvc.HasPermissionOverEmployeeOnDate
        Return SecurityBaseMethods.HasPermissionOverEmployeeOnDate(idEmployee, applicationAlias, featureType, perm, dDate, oState)
    End Function

    ''' <summary>
    ''' Returns whether current passport have specified permission over specified group.
    ''' </summary>
    ''' <param name="idGroup">The ID of the group for which to get permissions.</param>
    ''' <param name="applicationAlias">The alias of the application in which to check permissions.</param>
    ''' <param name="perm">The required permission.</param>
    ''' <param name="oState">The result error information</param>

    Public Function HasPermissionOverGroupAppAlias(ByVal idGroup As Integer, ByVal applicationAlias As String, ByVal featureType As String, ByVal perm As Permission, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements ISecurityBaseSvc.HasPermissionOverGroupAppAlias
        Return SecurityBaseMethods.HasPermissionOverGroupAppAlias(idGroup, applicationAlias, featureType, perm, oState)
    End Function

    ''' <summary>
    ''' Returns whether current passport have specified permission over specified group.
    ''' </summary>
    ''' <param name="idGroup">The ID of the group for which to get permissions.</param>
    ''' <param name="idApplication">The ID of the application in which to check permissions.</param>
    ''' <param name="perm">The required permission.</param>
    ''' <param name="oState">The result error information</param>

    Public Function HasPermissionOverGroupAppId(ByVal idGroup As Integer, ByVal idApplication As Integer, ByVal perm As Permission, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements ISecurityBaseSvc.HasPermissionOverGroupAppId
        Return SecurityBaseMethods.HasPermissionOverGroupAppId(idGroup, idApplication, perm, oState)
    End Function

    ''' <summary>
    ''' Returns a ticket containing passport information for caching.
    ''' </summary>
    ''' <param name="idPassport">The ID of the passport to return ticket for.</param>

    Public Function GetPassportTicket(ByVal idPassport As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of PassportTicket) Implements ISecurityBaseSvc.GetPassportTicket
        Return SecurityBaseMethods.GetPassportTicket(idPassport, oState)
    End Function

    ''' <summary>
    ''' Returns if reset validation code is correct.
    ''' </summary>
    ''' <param name="idPassport">The ID of the passport </param>

    Public Function ResetValidationCodeRobotics(ByVal idPassport As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements ISecurityBaseSvc.ResetValidationCodeRobotics
        Return SecurityBaseMethods.ResetValidationCodeRobotics(idPassport, oState)
    End Function

    ''' <summary>
    ''' Returns if reset validation code is correct.
    ''' </summary>
    ''' <param name="idPassport">The ID of the passport </param>

    Public Function ResetValidationCode(ByVal idPassport As Integer, ByVal strClientLocation As String, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements ISecurityBaseSvc.ResetValidationCode
        Return SecurityBaseMethods.ResetValidationCode(idPassport, strClientLocation, oState)
    End Function

    ''' <summary>
    ''' Returns if valid code validation.
    ''' </summary>
    ''' <param name="idPassport">The ID of the passport </param>
    ''' <param name="strCode">Vaidation code </param>

    Public Function IsValidCode(ByVal idPassport As Integer, ByVal strCode As String, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements ISecurityBaseSvc.IsValidCode
        Return SecurityBaseMethods.IsValidCode(idPassport, strCode, oState)
    End Function


    ''' <summary>
    ''' Returns a ticket containing passport information for caching.
    ''' </summary>
    ''' <param name="SessionID">The ID of the session to return ticket for.</param>

    Public Function GetPassportTicketBySessionID(ByVal SessionID As String, ByVal PassportID As String, ByVal oState As roWsState) As roGenericVtResponse(Of PassportTicket) Implements ISecurityBaseSvc.GetPassportTicketBySessionID
        Return SecurityBaseMethods.GetPassportTicketBySessionID(SessionID, PassportID, oState)
    End Function


    Public Function GetContext(ByVal intIDPassport As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of CContext) Implements ISecurityBaseSvc.GetContext
        Return SecurityBaseMethods.GetContext(intIDPassport, oState)
    End Function


    Public Function SetContext(ByVal intIDPassport As Integer, ByVal oContext As CContext, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements ISecurityBaseSvc.SetContext
        Return SecurityBaseMethods.SetContext(intIDPassport, oContext, oState)
    End Function


    Public Function SignOut(ByVal intIDPassport As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements ISecurityBaseSvc.SignOut
        Return SecurityBaseMethods.SignOut(intIDPassport, oState)
    End Function

    ''' <summary>
    ''' Verifica que la sesión sea correcta.
    ''' </summary>
    ''' <param name="intIDPassport">Código del pasaporte activo.</param>
    ''' <param name="oState">Estado actual. Contiene el id de sesión único (SessionID) y la localización del cliente (ClientAddress).</param>
    ''' <returns></returns>
    ''' <remarks></remarks>

    Public Function CheckSession(ByVal intIDPassport As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements ISecurityBaseSvc.CheckSession
        Return SecurityBaseMethods.CheckSession(intIDPassport, oState)
    End Function

    ''' <summary>
    ''' Obtiene la lista de idiomas definidos en la bd
    ''' </summary>
    ''' <param name="oState"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>

    Public Function GetLanguages(ByVal oState As roWsState) As roGenericVtResponse(Of Generic.List(Of Language)) Implements ISecurityBaseSvc.GetLanguages
        Return SecurityBaseMethods.GetLanguages(oState)
    End Function


    Public Function SetLanguage(ByVal intIDPassport As Integer, ByVal strLanguageKey As String, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements ISecurityBaseSvc.SetLanguage
        Return SecurityBaseMethods.SetLanguage(intIDPassport, strLanguageKey, oState)
    End Function

#Region "Informes de Emergencia anonimos"


    Public Function GetEmergencyReports(ByVal oState As roWsState) As roGenericVtResponse(Of Generic.List(Of roReportScheduler)) Implements ISecurityBaseSvc.GetEmergencyReports
        Return SecurityBaseMethods.GetEmergencyReports(oState)
    End Function


    Public Function GetEmergencyReportKey(ByVal oState As roWsState) As roGenericVtResponse(Of String) Implements ISecurityBaseSvc.GetEmergencyReportKey
        Return SecurityBaseMethods.GetEmergencyReportKey(oState)
    End Function


    Public Function ExecuteEmergencyReport(ByVal strReportName As String, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements ISecurityBaseSvc.ExecuteEmergencyReport
        Return SecurityBaseMethods.ExecuteEmergencyReport(strReportName, oState)
    End Function


    Public Function IsEmergencyReportActive(ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements ISecurityBaseSvc.IsEmergencyReportActive
        Return SecurityBaseMethods.IsEmergencyReportActive(oState)
    End Function



    Public Function IsValidPwd(ByVal strPwd As String, ByVal _idPassport As Integer, ByVal ValidateHistory As Boolean, ByVal ActualPwd As String, ByVal oState As roWsState) As roGenericVtResponse(Of PasswordLevelError) Implements ISecurityBaseSvc.IsValidPwd
        Return SecurityBaseMethods.IsValidPwd(strPwd, _idPassport, ValidateHistory, ActualPwd, oState)
    End Function

#End Region

End Class
