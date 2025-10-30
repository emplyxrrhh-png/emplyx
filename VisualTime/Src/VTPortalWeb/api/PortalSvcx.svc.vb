Imports System.Drawing
Imports System.IO
Imports System.Security.Principal
Imports System.ServiceModel
Imports System.ServiceModel.Activation
Imports System.ServiceModel.Web
Imports System.Web.Hosting
Imports Robotics.Base
Imports Robotics.Base.DTOs
Imports Robotics.Base.DTOs.UserFieldsTypes
Imports Robotics.Base.VTBusiness
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTBusiness.Terminal
Imports Robotics.Base.VTBusiness.Zone
Imports Robotics.Base.VTChannels
Imports Robotics.Base.VTCommuniques
Imports Robotics.Base.VTEmployees
Imports Robotics.Base.VTPortal
Imports Robotics.Base.VTPortal.VTPortal
Imports Robotics.Base.VTRequests
Imports Robotics.Base.VTServiceApi
Imports Robotics.Base.VTSurveys
Imports Robotics.Base.VTToDoLists
Imports Robotics.Base.VTWebLinks
Imports Robotics.DataLayer
Imports Robotics.ExternalSystems.DataLink.RoboticsExternAccess
Imports Robotics.Security
Imports Robotics.Security.Base
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions
Imports VTLiveApi

<ServiceContract(Namespace:="")>
<AspNetCompatibilityRequirements(RequirementsMode:=AspNetCompatibilityRequirementsMode.Allowed)>
<CustomErrorBehavior>
Public Class PortalSvcx

#Region "Webservice Constants"

    Private Shared SERVER_VERSION = Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString

    Private cCulture As Globalization.CultureInfo = System.Threading.Thread.CurrentThread.CurrentCulture

#End Region

    ''' <summary>
    ''' Esta función responde a un método GET del servidor.
    ''' Realiza el inicio del inicio de sesión del empleado especificado en los parametros en el sistema de VisualTime
    ''' </summary>
    ''' <param name="usr">Nombre de usuario del empleado</param>
    ''' <param name="pwd">Contraseña del empleado</param>
    ''' <param name="language">Idioma de inicio de sesión. Valores permitiods (ESP/CAT/ENG/POR)</param>
    ''' <param name="appVersion">Versión de la aplicación para iniciar el protocolo de comunicaciones</param>
    ''' <param name="validationCode">Código de validación subministrado por el empleado para iniciar sesión si se ha configurado visualtime para requerir este nivel de validación.</param>
    ''' <param name="timeZone">Código OLSON que describe la zona horaria desde donde se realiza la conexión del portal.</param>
    ''' <param name="accessFromApp">Incidca si el usuario esta realizando login mediante una aplicación.</param>
    ''' <returns>Obtenemos un objeto con la información relativa al inicio de sesión. Si es válida, ha expirado o si se han introducido mal las credenciales. Así como la palabra de seguridad utilizada para identificar la sesión.</returns>
    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function isAdfsActive() As StdServerVersion
        Return GetSsoConfiguration()
    End Function

    ''' <summary>
    ''' Esta función responde a un método GET del servidor.
    ''' Realiza el inicio del inicio de sesión del empleado especificado en los parametros en el sistema de VisualTime
    ''' </summary>
    ''' <param name="usr">Nombre de usuario del empleado</param>
    ''' <param name="pwd">Contraseña del empleado</param>
    ''' <param name="language">Idioma de inicio de sesión. Valores permitiods (ESP/CAT/ENG/POR)</param>
    ''' <param name="appVersion">Versión de la aplicación para iniciar el protocolo de comunicaciones</param>
    ''' <param name="validationCode">Código de validación subministrado por el empleado para iniciar sesión si se ha configurado visualtime para requerir este nivel de validación.</param>
    ''' <param name="timeZone">Código OLSON que describe la zona horaria desde donde se realiza la conexión del portal.</param>
    ''' <param name="accessFromApp">Incidca si el usuario esta realizando login mediante una aplicación.</param>
    ''' <returns>Obtenemos un objeto con la información relativa al inicio de sesión. Si es válida, ha expirado o si se han introducido mal las credenciales. Así como la palabra de seguridad utilizada para identificar la sesión.</returns>
    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function GetSsoConfiguration() As StdServerVersion
        Dim oState As New roSecurityState(0, HttpContext.Current)
        Dim lrret As New StdServerVersion

        Try
            Dim oCn As roBaseConnection = roCacheManager.GetInstance.GetConnection
            If oCn Is Nothing OrElse Not oCn.IsInitialized() Then
                lrret.Status = ErrorCodes.GENERAL_ERROR
            Else
                Dim oAdfs As New AdvancedParameter.roAdvancedParameter("ADFSEnabled", New AdvancedParameter.roAdvancedParameterState())

                If (oAdfs.State.Result = AdvancedParameterResultEnum.NoError) Then
                    Dim oApiVersion As New AdvancedParameter.roAdvancedParameter("VTPortalApiVersion", New AdvancedParameter.roAdvancedParameterState())
                    Dim oDefaultVersion As New AdvancedParameter.roAdvancedParameter("VTPortal.DefaultVersion", New AdvancedParameter.roAdvancedParameterState())
                    Dim oParamSSOType As New AdvancedParameter.roAdvancedParameter("SSOType", New AdvancedParameter.roAdvancedParameterState())
                    Dim oVTPortalMixModeEnabled As New AdvancedParameter.roAdvancedParameter("VisualTime.SSO.VTPortalMixedAuthEnabled", New AdvancedParameter.roAdvancedParameterState())
                    Dim oSSOConigVersion As New AdvancedParameter.roAdvancedParameter("VisualTime.SSO.ConfigurationVersion", New AdvancedParameter.roAdvancedParameterState())
                    Dim oRefreshConfig As New AdvancedParameter.roAdvancedParameter("VTPortal.RefreshConfiguration", New AdvancedParameter.roAdvancedParameterState())


                    If oParamSSOType.Value = "SSO" Then
                        Try
                            Dim user As WindowsPrincipal = CType(HttpContext.Current.User, WindowsPrincipal)
                            lrret.SSOServerEnabled = True
                            lrret.Result = False
                            lrret.SSOmixedModeEnabled = False
                            lrret.SSOUserLoggedIn = user.Identity.IsAuthenticated
                            lrret.SSOUserName = user.Identity.Name
                            lrret.SSOVersion = 1
                        Catch ex As Exception
                            lrret.SSOServerEnabled = False
                            lrret.Result = False
                            lrret.SSOmixedModeEnabled = False
                            lrret.SSOUserLoggedIn = False
                            lrret.SSOUserName = ""
                            lrret.SSOVersion = 0
                        End Try
                    Else
                        lrret.SSOServerEnabled = False
                        lrret.SSOUserLoggedIn = False
                        lrret.SSOUserName = ""
                        lrret.SSOmixedModeEnabled = roTypes.Any2Boolean(oVTPortalMixModeEnabled.Value)
                        lrret.Result = roTypes.Any2Boolean(oAdfs.Value)
                        lrret.SSOVersion = roTypes.Any2Integer(oSSOConigVersion.Value)
                    End If

                    lrret.ApiVersion = roTypes.Any2Integer(oApiVersion.Value)
                    lrret.DefaultVersion = roTypes.Any2String(oDefaultVersion.Value)

                    Dim sConf As String = roTypes.Any2String(oRefreshConfig.Value)
                    If sConf <> String.Empty Then
                        Dim byt As Byte() = System.Text.Encoding.UTF8.GetBytes(sConf)
                        lrret.RefreshConfiguration = Convert.ToBase64String(byt)
                    End If

                    lrret.Status = ErrorCodes.OK
                Else
                    lrret.Status = ErrorCodes.GENERAL_ERROR
                End If
            End If
        Catch ex As Exception
            lrret.Status = ErrorCodes.GENERAL_ERROR

            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::PortalSvcx::GetAdfsConfiguration")
        End Try

        Return lrret
    End Function



    ''' <summary>
    ''' Esta función responde a un método GET del servidor.
    ''' Realiza el inicio del inicio de sesión del empleado especificado en los parametros en el sistema de VisualTime
    ''' </summary>
    ''' <param name="usr">Nombre de usuario del empleado</param>
    ''' <param name="pwd">Contraseña del empleado</param>
    ''' <param name="language">Idioma de inicio de sesión. Valores permitiods (ESP/CAT/ENG/POR)</param>
    ''' <param name="appVersion">Versión de la aplicación para iniciar el protocolo de comunicaciones</param>
    ''' <param name="validationCode">Código de validación subministrado por el empleado para iniciar sesión si se ha configurado visualtime para requerir este nivel de validación.</param>
    ''' <param name="timeZone">Código OLSON que describe la zona horaria desde donde se realiza la conexión del portal.</param>
    ''' <param name="accessFromApp">Incidca si el usuario esta realizando login mediante una aplicación.</param>
    ''' <returns>Obtenemos un objeto con la información relativa al inicio de sesión. Si es válida, ha expirado o si se han introducido mal las credenciales. Así como la palabra de seguridad utilizada para identificar la sesión.</returns>
    <OperationContract()>
    <WebInvoke(Method:="POST", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function Authenticate() As LoginResult
        Dim oState As New roSecurityState(-1, HttpContext.Current)

        Dim lrret As New LoginResult
        Dim accessFromApp As Boolean = False
        Try

            If Global_asax.LoggedIn Then
                'Puede ser que lanze una excepción ya que los tiempos de timeout son distintos
                ' no pasa nada.
                Try
                    oState = New roSecurityState(Global_asax.Identity.ID, HttpContext.Current)
                    SessionHelper.SessionRemove(Global_asax.Identity.ID & "*" & Global_asax.ApplicationSource.ToString(), oState)
                Catch ex As Exception
                End Try
            End If

            roBaseState.SetSessionSmall(oState, -1, Global_asax.ApplicationSource, "")
            Dim strAuthToken As String = If(HttpContext.Current.Request.Headers("roAuth") IsNot Nothing, HttpContext.Current.Request.Headers("roAuth"), "")

            Dim usr As String = HttpContext.Current.Request.Params("user")
            Dim pwd As String = HttpContext.Current.Request.Params("password")

            Dim language As String = HttpContext.Current.Request.Params("language")
            Dim appVersion As String = HttpContext.Current.Request.Params("appVersion")
            Dim validationCode As String = HttpContext.Current.Request.Params("validationCode")
            Dim timeZone As String = HttpContext.Current.Request.Params("timeZone")
            Dim buttonLoginPressed As Boolean = roTypes.Any2Boolean(HttpContext.Current.Request.Params("buttonLogin"))
            accessFromApp = roTypes.Any2Boolean(HttpContext.Current.Request.Params("accessFromApp"))

            Dim loginPwd As String = String.Empty
            Try
                Dim result As (Boolean, String) = Robotics.VTBase.CryptographyHelper.DecryptLiveApi(pwd)

                If result.Item1 Then
                    loginPwd = result.Item2
                Else
                    loginPwd = pwd
                End If
            Catch ex As Exception
                loginPwd = pwd
            End Try

            lrret = VTPortal.SecurityHelper.Login(usr, loginPwd, language, SERVER_VERSION, appVersion, validationCode, timeZone, strAuthToken, accessFromApp, Global_asax.ApplicationSource, oState, String.Empty, buttonLoginPressed)

        Catch ex As Exception
            lrret.Status = ErrorCodes.BAD_CREDENTIALS

            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::PortalSvcx::Login")
        End Try


        Dim bAuthenticated As Boolean = False
        If lrret.Status = ErrorCodes.OK OrElse
                lrret.Status = ErrorCodes.LOGIN_PASSWORD_EXPIRED OrElse
                lrret.Status = ErrorCodes.LOGIN_RESULT_LOW_STRENGHT_ERROR OrElse
                lrret.Status = ErrorCodes.LOGIN_RESULT_MEDIUM_STRENGHT_ERROR OrElse
                lrret.Status = ErrorCodes.LOGIN_RESULT_HIGH_STRENGHT_ERROR OrElse
                lrret.Status = ErrorCodes.LOGIN_NEED_TEMPORANY_KEY OrElse
                lrret.Status = ErrorCodes.LOGIN_TEMPORANY_KEY_EXPIRED Then
            bAuthenticated = True
        End If

        Robotics.Web.Base.CookieSession.CreateAuthenticationCookie(bAuthenticated, lrret.Token, StrConv(roAppType.VTPortal.ToString(), VbStrConv.ProperCase))

        Return lrret
    End Function

    ''' <summary>
    ''' Esta función responde a un método GET del servidor.
    ''' Realiza el inicio del inicio de sesión del empleado especificado en los parametros en el sistema de VisualTime
    ''' </summary>
    ''' <param name="usr">Nombre de usuario del empleado</param>
    ''' <param name="pwd">Contraseña del empleado</param>
    ''' <param name="language">Idioma de inicio de sesión. Valores permitiods (ESP/CAT/ENG/POR)</param>
    ''' <param name="appVersion">Versión de la aplicación para iniciar el protocolo de comunicaciones</param>
    ''' <param name="validationCode">Código de validación subministrado por el empleado para iniciar sesión si se ha configurado visualtime para requerir este nivel de validación.</param>
    ''' <param name="timeZone">Código OLSON que describe la zona horaria desde donde se realiza la conexión del portal.</param>
    ''' <param name="accessFromApp">Incidca si el usuario esta realizando login mediante una aplicación.</param>
    ''' <returns>Obtenemos un objeto con la información relativa al inicio de sesión. Si es válida, ha expirado o si se han introducido mal las credenciales. Así como la palabra de seguridad utilizada para identificar la sesión.</returns>
    <OperationContract()>
    <WebInvoke(Method:="POST", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function AuthenticateSession() As LoggedInUserInfo
        Dim oState As New roSecurityState(-1, HttpContext.Current)

        Dim lrret As New LoggedInUserInfo
        Dim bIsModeapp As Boolean = False

        If Not Global_asax.LoggedIn Then
            lrret.Status = VTPortal.StatusHelper.GetStatusFromState(Global_asax.SecurityResult)
            If lrret.Status = ErrorCodes.OK Then lrret.Status = ErrorCodes.NO_SESSION
            Return lrret
        End If
        lrret.Status = ErrorCodes.OK

        Try
            roBaseState.SetSessionSmall(oState, -1, Global_asax.ApplicationSource, "")
            bIsModeapp = roTypes.Any2Boolean(HttpContext.Current.Request.Params("isModeApp"))

            lrret = VTPortal.SecurityHelper.AuthenticatedSessionInfo(Global_asax.Supervisor, Global_asax.Identity, Global_asax.IsSupervisor, Global_asax.AuthenticationUsed, bIsModeapp, oState, Robotics.Web.Base.CookieSession.GetAuthenticationCookie(StrConv(roAppType.VTPortal.ToString(), VbStrConv.ProperCase)))

            Dim bAuthenticated As Boolean = False
            If lrret.Status = ErrorCodes.OK OrElse
            lrret.Status = ErrorCodes.LOGIN_PASSWORD_EXPIRED OrElse
            lrret.Status = ErrorCodes.LOGIN_RESULT_LOW_STRENGHT_ERROR OrElse
            lrret.Status = ErrorCodes.LOGIN_RESULT_MEDIUM_STRENGHT_ERROR OrElse
            lrret.Status = ErrorCodes.LOGIN_RESULT_HIGH_STRENGHT_ERROR Then
                bAuthenticated = True
            End If

            Robotics.Web.Base.CookieSession.CreateAuthenticationCookie(bAuthenticated, lrret.Token, StrConv(roAppType.VTPortal.ToString(), VbStrConv.ProperCase))
        Catch ex As Exception
            lrret.Status = ErrorCodes.GENERAL_ERROR
            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::PortalSvcx::AuthenticateSession")
        End Try

        Return lrret
    End Function

    <OperationContract()>
    <WebInvoke(Method:="POST", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function ValidateSession() As StdResponse
        Dim oState As New roSecurityState(-1, HttpContext.Current)

        Dim lrret As New StdResponse

        If Not Global_asax.LoggedIn Then
            lrret.Status = VTPortal.StatusHelper.GetStatusFromState(Global_asax.SecurityResult)
            If lrret.Status = ErrorCodes.OK Then lrret.Status = ErrorCodes.NO_SESSION
            Return lrret
        End If
        lrret.Status = ErrorCodes.OK

        Try
            Dim validationCode = roTypes.Any2String(HttpContext.Current.Request.Params("code"))

            roBaseState.SetSessionSmall(oState, Global_asax.Identity.ID, Global_asax.ApplicationSource, "")
            lrret = VTPortal.SecurityHelper.validateCode(Global_asax.Identity, validationCode, oState)

            If Not lrret.Result Then
                Robotics.Web.Base.CookieSession.ClearAuthenticationCookie(StrConv(roAppType.VTPortal.ToString(), VbStrConv.ProperCase))
            End If

        Catch ex As Exception
            lrret.Status = ErrorCodes.GENERAL_ERROR
            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::PortalSvcx::AuthenticateSession")
        End Try

        Return lrret
    End Function

    ''' <summary>
    ''' Se requiere que la sesión este iniciada en el sistema. Esta función responde a un método POST del servidor.
    ''' En caso de estar habilitado el adfs en el servidor obtenemos la información necesaria del usuario logeado para realizar el inicio de sesión.
    ''' Esta función recibe los mismos parametros que en la petición equivalente sin fotografia con un objeto imagen en la propiedad Request.Files
    ''' </summary>
    ''' <returns>Obtenemos un objeto del tipo LoggedInUserInfo, contiene la información del usuario que ha realizado login en el sistema.</returns>
    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function GetLoggedInUserInfo() As LoggedInUserInfo
        Dim lrret As New LoggedInUserInfo

        If Not Global_asax.LoggedIn Then
            lrret.Status = VTPortal.StatusHelper.GetStatusFromState(Global_asax.SecurityResult)
            If lrret.Status = ErrorCodes.OK Then lrret.Status = ErrorCodes.NO_SESSION
            Return lrret
        End If
        lrret.Status = ErrorCodes.OK
        Try
            Dim oState As New roSecurityState(-1, HttpContext.Current)
            roBaseState.SetSessionSmall(oState, Global_asax.Identity.ID, Global_asax.ApplicationSource, "")
            Dim bIsModeapp As Boolean = If(HttpContext.Current.Request.Headers("roSrc") IsNot Nothing, roTypes.Any2Boolean(HttpContext.Current.Request.Headers("roSrc")), False)
            lrret = VTPortal.SecurityHelper.AuthenticatedSessionInfo(Global_asax.Supervisor, Global_asax.Identity, Global_asax.IsSupervisor, Global_asax.AuthenticationUsed, bIsModeapp, oState, String.Empty)

            Dim bAuthenticated As Boolean = False
            If lrret.Status = ErrorCodes.OK OrElse
            lrret.Status = ErrorCodes.LOGIN_PASSWORD_EXPIRED OrElse
            lrret.Status = ErrorCodes.LOGIN_RESULT_LOW_STRENGHT_ERROR OrElse
            lrret.Status = ErrorCodes.LOGIN_RESULT_MEDIUM_STRENGHT_ERROR OrElse
            lrret.Status = ErrorCodes.LOGIN_RESULT_HIGH_STRENGHT_ERROR Then
                bAuthenticated = True
            End If

            Robotics.Web.Base.CookieSession.CreateAuthenticationCookie(bAuthenticated, lrret.Token, StrConv(roAppType.VTPortal.ToString(), VbStrConv.ProperCase))
        Catch ex As Exception
            lrret.Status = ErrorCodes.GENERAL_ERROR
            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::PortalSvcx::GetLoggedInUserInfo")
        End Try

        Return lrret
    End Function

    ''' <summary>
    ''' Esta función responde a un método GET del servidor.
    ''' Realiza una petición de reset de contraseña de empleado
    ''' </summary>
    ''' <param name="userName">Nombre de usuario del empleado</param>
    ''' <param name="email">Dirección de correo que tiene dada de alta en VisualTime el empleado</param>
    ''' <returns>Obtenemos un objeto con la información relativa al inicio de sesión. Si es válida, ha expirado o si se han introducido mal las credenciales. Así como la palabra de seguridad utilizada para identificar la sesión.</returns>
    <OperationContract()>
    <WebInvoke(Method:="POST", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function RecoverMyPassword() As StdResponse
        Dim oState As New roSecurityState(-1, HttpContext.Current)

        Dim lrret As New StdResponse

        Try
            lrret.Status = ErrorCodes.OK

            Dim userName As String = HttpContext.Current.Request.Params("userName")
            Dim email As String = HttpContext.Current.Request.Params("email")

            Dim oSecurityState As New roSecurityState()
            roBaseState.SetSessionSmall(oSecurityState, -1, Global_asax.ApplicationSource, "")
            lrret = VTPortal.SecurityHelper.RecoverEmployeePassword(userName, email, oSecurityState)

        Catch ex As Exception
            lrret.Status = ErrorCodes.USER_OR_EMAIL_NOTFOUND

            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::PortalSvcx::RecoverPassword")
        End Try

        Return lrret
    End Function

    ''' <summary>
    ''' Esta función responde a un método GET del servidor.
    ''' Realiza una petición de reset de contraseña de empleado
    ''' </summary>
    ''' <param name="userName">Nombre de usuario del empleado</param>
    ''' <param name="requestKey">Clave recibida en el correo electrónico que valida la petición de cambio de contraseña</param>
    ''' <param name="newPassword">Nueva contraseña del empleado</param>
    ''' <returns>Obtenemos un objeto con la información relativa al inicio de sesión. Si es válida, ha expirado o si se han introducido mal las credenciales. Así como la palabra de seguridad utilizada para identificar la sesión.</returns>
    <OperationContract()>
    <WebInvoke(Method:="POST", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function ResetPassword() As StdResponse
        Dim oState As New roSecurityState(-1, HttpContext.Current)

        Dim lrret As New StdResponse

        Try
            lrret.Status = ErrorCodes.OK

            Dim userName As String = HttpContext.Current.Request.Params("userName")
            Dim requestKey As String = HttpContext.Current.Request.Params("requestKey")
            Dim newPassword As String = HttpContext.Current.Request.Params("newPassword")
            Dim strToken As String = If(HttpContext.Current.Request.Headers("roToken") IsNot Nothing, HttpContext.Current.Request.Headers("roToken"), "")

            Dim oSecurityState As New roSecurityState()
            roBaseState.SetSessionSmall(oSecurityState, -1, Global_asax.ApplicationSource, "")
            lrret = VTPortal.SecurityHelper.ResetPasswordToNew(userName, requestKey, newPassword, roAppType.VTPortal, strToken, oSecurityState)
        Catch ex As Exception
            lrret.Status = ErrorCodes.USER_OR_EMAIL_NOTFOUND

            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::PortalSvcx::ResetPasswordToNew")
        End Try

        Return lrret
    End Function

    <OperationContract()>
    <WebInvoke(Method:="POST", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function UpdateMyLanguage() As StdResponse
        Dim oState As New roSecurityState(-1, HttpContext.Current)

        Dim lrret As New StdResponse
        Try

            If Global_asax.LoggedIn Then
                'Puede ser que lanze una excepción ya que los tiempos de timeout son distintos
                ' no pasa nada.
                Try
                    oState = New roSecurityState(Global_asax.Identity.ID, HttpContext.Current)
                    SessionHelper.SessionRemove(Global_asax.Identity.ID & "*" & Global_asax.ApplicationSource.ToString, oState)
                Catch ex As Exception
                End Try
            End If

            roBaseState.SetSessionSmall(oState, -1, Global_asax.ApplicationSource, "")
            Dim language As String = HttpContext.Current.Request.Params("language")

            Dim idPassport As Integer = -1

            If Global_asax.Supervisor IsNot Nothing Then
                idPassport = Global_asax.Supervisor.ID
            Else
                idPassport = Global_asax.Identity.ID
            End If

            lrret = VTPortal.SecurityHelper.UpdateServerLanguage(idPassport, language, oState)
        Catch ex As Exception
            lrret.Status = ErrorCodes.BAD_CREDENTIALS

            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::PortalSvcx::Login")
        End Try

        Return lrret
    End Function

    ''' <summary>
    ''' Se requiere que la sesión este iniciada en el sistema. Esta función responde a un método GET del servidor.
    ''' Finaliza la sesión iniciada por el empleado, invalidando los códigos de seguridad utilizados para la sesión.
    ''' </summary>
    ''' <returns>Obtenemos un objeto estandar de respuesta de petición indicando si se ha podido cerrar la sesión y en caso contrario el código de error obtenido.</returns>
    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function Logout(ByVal uuid As String) As StdResponse
        Dim lrret As New StdResponse()

        If Global_asax.ApplicationSource = roAppSource.TimeGate Then
            lrret.Status = ErrorCodes.OK
            Try
                If Global_asax.TerminalIdentity IsNot Nothing Then
                    lrret = VTPortal.SecurityHelper.LogoutTimegateSession(Global_asax.TerminalIdentity.SerialNumber, Global_asax.ApplicationSource)
                    Robotics.Web.Base.CookieSession.ClearAuthenticationCookie(StrConv(roAppType.VTPortal.ToString(), VbStrConv.ProperCase))
                    Global_asax.Logout()
                End If
            Catch ex As Exception
                lrret.Status = ErrorCodes.GENERAL_ERROR
                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTLiveApi::Timegate::Logout")
            End Try

            Return lrret
        Else
            If Not Global_asax.LoggedIn Then
                lrret.Status = VTPortal.StatusHelper.GetStatusFromState(Global_asax.SecurityResult)
                If lrret.Status = ErrorCodes.OK Then lrret.Status = ErrorCodes.NO_SESSION
                Return lrret
            End If

            Try
                lrret = VTPortal.SecurityHelper.LogoutUserSession(If(Global_asax.Supervisor IsNot Nothing, Global_asax.Supervisor.ID, Global_asax.Identity.ID), Global_asax.ApplicationSource, uuid)
                Robotics.Web.Base.CookieSession.ClearAuthenticationCookie(StrConv(roAppType.VTPortal.ToString(), VbStrConv.ProperCase))
                Global_asax.Logout()
            Catch ex As Exception
                lrret.Status = ErrorCodes.GENERAL_ERROR
                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTLiveApi::PortalSvcx::Logout")
            End Try
        End If

        Return lrret
    End Function

    ''' <summary>
    ''' Se requiere que la sesión este iniciada en el sistema. Esta función responde a un método GET del servidor.
    ''' Cambia la contraseña del usuario que ha iniciado sesión en el sistema, validando la contraseña antigua y la complejidad de la nueva contraseña con el mínimo requerido por VisualTime.
    ''' </summary>
    ''' <param name="oldPassword">Contraseña antigua del empleado</param>
    ''' <param name="newPassword">Nueva contraseña del empleado</param>
    ''' <returns>Obtenemos un objeto estandar de respuesta de petición indicando si se ha podido cambiar la contraseña y en caso contrario el código de error obtenido.</returns>
    <OperationContract()>
    <WebInvoke(Method:="POST", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function ChangeMyPassword() As StdResponse
        Dim lrret As New StdResponse()
        Dim bolCorrect As Boolean = True

        Try

            If Not Global_asax.LoggedIn Then
                lrret.Status = VTPortal.StatusHelper.GetStatusFromState(Global_asax.SecurityResult)
                If lrret.Status = ErrorCodes.OK Then lrret.Status = ErrorCodes.NO_SESSION
                Return lrret
            End If

            Dim strToken As String = If(HttpContext.Current.Request.Headers("roToken") IsNot Nothing, HttpContext.Current.Request.Headers("roToken"), "")
            Dim oldPassword As String = HttpContext.Current.Request.Params("oldPassword")
            Dim newPassword As String = HttpContext.Current.Request.Params("newPassword")

            Dim oSecurityState As New roSecurityState()
            roBaseState.SetSessionSmall(oSecurityState, Global_asax.Identity.ID, Global_asax.ApplicationSource, "")

            lrret = VTPortal.SecurityHelper.ChangePassword(Global_asax.Identity.ID, oldPassword, newPassword, strToken, oSecurityState)
        Catch ex As Exception
            lrret.Status = ErrorCodes.GENERAL_ERROR
            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::PortalSvcx::ChangePassword")
        End Try

        Return lrret
    End Function

    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function CheckTelecommutingChange(ByVal selectedDay As String, ByVal type As String, ByVal impersonating As Boolean) As StdCheckTelecommuteResponse
        Dim lrret As New StdCheckTelecommuteResponse()
        Dim bolCorrect As Boolean = True

        Try
            If Not Global_asax.LoggedIn Then
                lrret.Status = VTPortal.StatusHelper.GetStatusFromState(Global_asax.SecurityResult)
                If lrret.Status = ErrorCodes.OK Then lrret.Status = ErrorCodes.NO_SESSION
                Return lrret
            End If

            Dim oSecurityState As New roSecurityState()
            roBaseState.SetSessionSmall(oSecurityState, Global_asax.Identity.ID, Global_asax.ApplicationSource, "")

            Dim sDate As DateTime = DateTime.ParseExact(selectedDay, "yyyy/MM/dd", Nothing)
            Dim eType As New TelecommutingTypeEnum

            If type = "home" Then
                eType = TelecommutingTypeEnum._AtOffice
            Else
                eType = TelecommutingTypeEnum._AtHome
            End If
            Dim oEmpState As New VTEmployees.Employee.roEmployeeState(Global_asax.Identity.ID)
            Dim ret As TelecommutingCheckChangeResult
            ret = VTPortal.EmployeesHelper.CheckTelecommutingChange(Global_asax.Identity.IDEmployee, sDate.Date, eType, impersonating, oEmpState)

            If ret = TelecommutingCheckChangeResult._Request Then
                '
                lrret.Result = True
                lrret.Status = ErrorCodes.OK
                lrret.NeedRequest = True
            ElseIf ret = TelecommutingCheckChangeResult._Direct Then
                ' No se precisa solicitud. Hago el cambio
                Dim retaux As StdResponse
                retaux = VTPortal.RequestsHelper.ChangeTelecommuting(Global_asax.Identity.IDEmployee, sDate.Date, eType, impersonating)

                lrret.Result = retaux.Result
                lrret.Status = retaux.Status
                lrret.NeedRequest = False
            Else
                lrret.Result = False
                lrret.Status = ErrorCodes.CHANGE_TELECOMMUTING_ERROR
            End If
        Catch ex As Exception
            lrret.Status = ErrorCodes.GENERAL_ERROR
            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::PortalSvcx::CheckTelecommutingChange")
        End Try

        Return lrret
    End Function

    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function GetMyTeamPlanEmployees(ByVal startDay As String, ByVal endDay As String, ByVal idEmployee As Integer, ByVal oldShift As Integer, ByVal newShift As Integer) As MyTeamPlanInfo
        Dim lrret As New MyTeamPlanInfo()
        Dim bolCorrect As Boolean = True

        Try
            If Not Global_asax.LoggedIn Then
                lrret.Status = VTPortal.StatusHelper.GetStatusFromState(Global_asax.SecurityResult)
                If lrret.Status = ErrorCodes.OK Then lrret.Status = ErrorCodes.NO_SESSION
                Return lrret
            End If

            Dim oSecurityState As New roSecurityState()
            roBaseState.SetSessionSmall(oSecurityState, Global_asax.Identity.ID, Global_asax.ApplicationSource, "")

            Dim sDate As DateTime = DateTime.ParseExact(startDay, "yyyy/MM/dd", Nothing)
            Dim eDate As DateTime = DateTime.ParseExact(endDay, "yyyy/MM/dd", Nothing)

            lrret = VTPortal.RequestsHelper.GetMyTeamPlanEmployees(idEmployee, sDate.Date, eDate.Date, oldShift, newShift, Global_asax.Identity.ID)

            Dim fileName As String = Path.Combine(HostingEnvironment.ApplicationPhysicalPath, "Resources/userDefault.png")
            Dim fileStream As New FileStream(fileName, FileMode.Open, FileAccess.Read)

            Dim ImageData As Byte()
            ImageData = New Byte(fileStream.Length - 1) {}
            fileStream.Read(ImageData, 0, System.Convert.ToInt32(fileStream.Length))
            fileStream.Close()

            lrret.DefaultImage = "url(data:image/png;base64," & Convert.ToBase64String(ImageData) & ") no-repeat center center"
        Catch ex As Exception
            lrret.Status = ErrorCodes.GENERAL_ERROR
            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::PortalSvcx::GetMyTeamPlanEmployees")
        End Try

        Return lrret
    End Function

    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function ChangeTelecommuting(ByVal selectedDay As String, ByVal type As String, ByVal impersonating As Boolean) As StdResponse
        Dim lrret As New StdResponse()
        Dim bolCorrect As Boolean = True

        Try
            If Not Global_asax.LoggedIn Then
                lrret.Status = VTPortal.StatusHelper.GetStatusFromState(Global_asax.SecurityResult)
                If lrret.Status = ErrorCodes.OK Then lrret.Status = ErrorCodes.NO_SESSION
                Return lrret
            End If

            Dim oSecurityState As New roSecurityState()
            roBaseState.SetSessionSmall(oSecurityState, Global_asax.Identity.ID, Global_asax.ApplicationSource, "")

            Dim sDate As DateTime = DateTime.ParseExact(selectedDay, "yyyy/MM/dd", Nothing)
            Dim eType As New TelecommutingTypeEnum

            If type = "home" Then
                eType = TelecommutingTypeEnum._AtOffice
            Else
                eType = TelecommutingTypeEnum._AtHome
            End If

            lrret = VTPortal.RequestsHelper.ChangeTelecommuting(Global_asax.Identity.IDEmployee, sDate.Date, eType, impersonating)
        Catch ex As Exception
            lrret.Status = ErrorCodes.GENERAL_ERROR
            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::PortalSvcx::ChangeTelecommuting")
        End Try

        Return lrret
    End Function

    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function ChangeTelecommutingByRequest(ByVal selectedDay As String, ByVal type As String, ByVal impersonating As Boolean) As StdRequestResponse
        Dim lrret As New StdRequestResponse()
        Dim bolCorrect As Boolean = True

        Try
            If Not Global_asax.LoggedIn Then
                lrret.Status = VTPortal.StatusHelper.GetStatusFromState(Global_asax.SecurityResult)
                If lrret.Status = ErrorCodes.OK Then lrret.Status = ErrorCodes.NO_SESSION
                Return lrret
            End If

            Dim oSecurityState As New roSecurityState()
            roBaseState.SetSessionSmall(oSecurityState, Global_asax.Identity.ID, Global_asax.ApplicationSource, "")

            Dim eType As New TelecommutingTypeEnum

            If type = "home" Then
                eType = TelecommutingTypeEnum._AtOffice
            Else
                eType = TelecommutingTypeEnum._AtHome
            End If

            Dim oReqState As New Requests.roRequestState
            roBaseState.SetSessionSmall(oReqState, Global_asax.Identity.ID, Global_asax.ApplicationSource, "")

            Dim oLng As New roLanguage()
            oLng.SetLanguageReference("LiveOne", Global_asax.Identity.Language.Key)

            lrret = VTPortal.RequestsHelper.SaveRequest(eRequestType.Telecommute, Global_asax.Identity, Global_asax.Identity.IDEmployee, selectedDay, selectedDay, "", "", "", Nothing, -1, "", "", "", False, "", "", "", "", "", "", "", "", False, "", False, Nothing, oLng, Global_asax.TimeZone, oReqState, False, Global_asax.Supervisor, Nothing, 0, True, eType)
        Catch ex As Exception
            lrret.Status = ErrorCodes.GENERAL_ERROR
            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::PortalSvcx::ChangeTelecommuting")
        End Try

        Return lrret
    End Function


    ''' <summary>
    ''' Se requiere que la sesión este iniciada en el sistema. Esta función responde a un método GET del servidor.
    ''' Indica que el empleado que ha realizado login acepta las condiciones de uso y licencia del programa.
    ''' </summary>
    ''' <returns>Obtenemos un objeto estandar de respuesta de petición indicando si se ha podido cambiar la contraseña y en caso contrario el código de error obtenido.</returns>
    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function AcceptMyLicense(ByVal bAcceptLicense As Boolean) As StdResponse
        Dim lrret As New StdResponse()
        Dim bolCorrect As Boolean = True

        Try
            If Not Global_asax.LoggedIn Then
                lrret.Status = VTPortal.StatusHelper.GetStatusFromState(Global_asax.SecurityResult)
                If lrret.Status = ErrorCodes.OK Then lrret.Status = ErrorCodes.NO_SESSION
                Return lrret
            End If

            Dim oLng As New roLanguage()
            oLng.SetLanguageReference("VTPortal", Global_asax.Identity.Language.Key)

            lrret.Result = False 'VTPortal.SecurityHelper.AcceptLicense(Global_asax.Identity, bAcceptLicense, oSecurityState)
            lrret.Status = ErrorCodes.LOGIN_INVALID_VERSION_APP
        Catch ex As Exception
            lrret.Status = ErrorCodes.GENERAL_ERROR
            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::PortalSvcx::ChangePassword")
        End Try

        Return lrret
    End Function

    ''' <summary>
    ''' Se requiere que la sesión este iniciada en el sistema. Esta función responde a un método GET del servidor.
    ''' Indica que el empleado que ha realizado login acepta las condiciones de uso y licencia del programa.
    ''' </summary>
    ''' <returns>Obtenemos un objeto estandar de respuesta de petición indicando si se ha podido cambiar la contraseña y en caso contrario el código de error obtenido.</returns>
    <OperationContract()>
    <WebInvoke(Method:="POST", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function AcceptConsent() As StdResponse
        Dim lrret As New StdResponse()

        Try
            If Not Global_asax.LoggedIn Then
                lrret.Status = VTPortal.StatusHelper.GetStatusFromState(Global_asax.SecurityResult)
                If lrret.Status = ErrorCodes.OK Then lrret.Status = ErrorCodes.NO_SESSION
                Return lrret
            End If

            Dim oSecurityState As New roSecurityState()
            roBaseState.SetSessionSmall(oSecurityState, Global_asax.Identity.ID, Global_asax.ApplicationSource, "")

            lrret.Result = True
            lrret.Status = ErrorCodes.OK
        Catch ex As Exception
            lrret.Status = ErrorCodes.GENERAL_ERROR
            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::PortalSvcx::ChangePassword")
        End Try

        Return lrret
    End Function

    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function GetEmployeeDailyRecordPunchesPattern(ByVal selectedDate As String) As roGenericResponse(Of roDailyRecordPunchesPattern)

        Dim lrret As New roGenericResponse(Of roDailyRecordPunchesPattern)
        If Not Global_asax.LoggedIn Then
            lrret.Status = VTPortal.StatusHelper.GetStatusFromState(Global_asax.SecurityResult)
            If lrret.Status = ErrorCodes.OK Then lrret.Status = ErrorCodes.NO_SESSION
            Return lrret
        End If

        Try
            Dim dDate As DateTime = DateTime.ParseExact(selectedDate, "dd/MM/yyyy", Nothing)
            lrret = VTPortal.ShiftsHelper.GetEmployeeDailyRecordPunchesPattern(Global_asax.Identity, Global_asax.Identity.IDEmployee, dDate)
        Catch ex As Exception
            lrret.Status = ErrorCodes.GENERAL_ERROR
            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::PortalSvcx::GetEmployeeDailyRecordPunchesPattern")
        End Try

        Return lrret
    End Function

    ''' <summary>
    ''' Se requiere que la sesión este iniciada en el sistema. Esta función responde a un método GET del servidor.
    ''' Obtiene el estado del empleado en el sistema, hora y sentido del último fichaje, estado de producción y centro de costo. También obtenemos la hora del servidor, así como las alertas pendientes de atender por parte del empleado.
    ''' </summary>
    ''' <returns>Obtenemos un objecto de tipo 'UserStatus' con toda la información relativa al empleado con sesión activa.</returns>
    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function GetUserScheduleStatus() As EmployeeStatus
        Dim lrret As New EmployeeStatus

        If Not Global_asax.LoggedIn Then
            lrret.Status = VTPortal.StatusHelper.GetStatusFromState(Global_asax.SecurityResult)
            If lrret.Status = ErrorCodes.OK Then lrret.Status = ErrorCodes.NO_SESSION
            Return lrret
        End If

        Try
            If Global_asax.Identity.IDEmployee IsNot Nothing AndAlso Global_asax.Identity.IDEmployee.HasValue AndAlso Global_asax.Identity.IDEmployee > 0 Then
                lrret = VTPortal.StatusHelper.GetEmployeeScheduleStatus(Global_asax.Identity, Global_asax.Identity.IDEmployee, Global_asax.TimeZone)
            Else
                lrret = VTPortal.StatusHelper.GetSupervisorScheduleStatus(Global_asax.Identity, Global_asax.Identity.ID, Global_asax.TimeZone)
            End If

            Dim bLoadAlerts As Boolean = Global_asax.IsSupervisor AndAlso WLHelper.HasPermissionOverFeature(Global_asax.Identity.ID, "Administration.Alerts", "U", Permission.Read)
            If Global_asax.Supervisor IsNot Nothing AndAlso Global_asax.Identity IsNot Nothing Then bLoadAlerts = False

            lrret.HasAlertPermission = bLoadAlerts
        Catch ex As Exception
            lrret.Status = ErrorCodes.GENERAL_ERROR

            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::PortalSvcx::GetEmployeeScheduleStatus")
        End Try

        Return lrret
    End Function

    ''' <summary>
    ''' Se requiere que la sesión este iniciada en el sistema. Esta función responde a un método GET del servidor.
    ''' Obtiene el estado del empleado en el sistema, hora y sentido del último fichaje, estado de producción y centro de costo. También obtenemos la hora del servidor, así como las alertas pendientes de atender por parte del empleado.
    ''' </summary>
    ''' <returns>Obtenemos un objecto de tipo 'UserStatus' con toda la información relativa al empleado con sesión activa.</returns>
    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function GetUserNotifications() As EmployeeNotifications
        Dim lrret As New EmployeeNotifications

        If Not Global_asax.LoggedIn Then
            lrret.Status = VTPortal.StatusHelper.GetStatusFromState(Global_asax.SecurityResult)
            If lrret.Status = ErrorCodes.OK Then lrret.Status = ErrorCodes.NO_SESSION
            Return lrret
        End If

        Try
            Dim bLoadAlerts As Boolean = Global_asax.IsSupervisor
            bLoadAlerts = bLoadAlerts AndAlso WLHelper.HasPermissionOverFeature(Global_asax.Identity.ID, "Administration.Alerts", "U", Permission.Read)

            If Global_asax.Supervisor IsNot Nothing AndAlso Global_asax.Identity IsNot Nothing Then
                bLoadAlerts = False
            End If


            If Global_asax.Identity.IDEmployee IsNot Nothing AndAlso Global_asax.Identity.IDEmployee.HasValue AndAlso Global_asax.Identity.IDEmployee > 0 Then
                lrret = VTPortal.StatusHelper.GetEmployeeNotifications(Global_asax.Identity, Global_asax.Identity.IDEmployee, bLoadAlerts, Global_asax.TimeZone)
            Else
                lrret = VTPortal.StatusHelper.GetSupervisorNotifications(Global_asax.Identity, Global_asax.Identity.ID, bLoadAlerts, Global_asax.TimeZone)
            End If



        Catch ex As Exception
            lrret.Status = ErrorCodes.GENERAL_ERROR

            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::PortalSvcx::GetEmployeeNotifications")
        End Try

        Return lrret
    End Function

    ''' <summary>
    ''' Se requiere que la sesión este iniciada en el sistema. Esta función responde a un método GET del servidor.
    ''' Obtiene el estado del empleado en el sistema, hora y sentido del último fichaje, estado de producción y centro de costo. También obtenemos la hora del servidor, así como las alertas pendientes de atender por parte del empleado.
    ''' </summary>
    ''' <returns>Obtenemos un objecto de tipo 'UserStatus' con toda la información relativa al empleado con sesión activa.</returns>
    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function GetUserNotificationsStatus() As EmployeeNotificationStatus
        Dim lrret As New EmployeeNotificationStatus

        If Not Global_asax.LoggedIn Then
            lrret.Status = VTPortal.StatusHelper.GetStatusFromState(Global_asax.SecurityResult)
            If lrret.Status = ErrorCodes.OK Then lrret.Status = ErrorCodes.NO_SESSION
            Return lrret
        End If

        Try
            Dim bLoadAlerts As Boolean = Global_asax.IsSupervisor
            bLoadAlerts = bLoadAlerts AndAlso WLHelper.HasPermissionOverFeature(Global_asax.Identity.ID, "Administration.Alerts", "U", Permission.Read)

            If Global_asax.Supervisor IsNot Nothing AndAlso Global_asax.Identity IsNot Nothing Then
                bLoadAlerts = False
            End If


            If Global_asax.Identity.IDEmployee IsNot Nothing AndAlso Global_asax.Identity.IDEmployee.HasValue AndAlso Global_asax.Identity.IDEmployee > 0 Then
                lrret = VTPortal.StatusHelper.GetEmployeeNotificationStatus(Global_asax.Identity, Global_asax.Identity.IDEmployee, bLoadAlerts, Global_asax.TimeZone)
            Else
                lrret = VTPortal.StatusHelper.GetSupervisorNotificationStatus(Global_asax.Identity, Global_asax.Identity.ID, bLoadAlerts, Global_asax.TimeZone)
            End If

        Catch ex As Exception
            lrret.Status = ErrorCodes.GENERAL_ERROR

            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::PortalSvcx::GetEmployeeNotifications")
        End Try

        Return lrret
    End Function


    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function GetServerZones() As Zones
        Dim lrret As New Zones

        If Not Global_asax.LoggedIn Then
            lrret.Status = VTPortal.StatusHelper.GetStatusFromState(Global_asax.SecurityResult)
            If lrret.Status = ErrorCodes.OK Then lrret.Status = ErrorCodes.NO_SESSION
            Return lrret
        End If

        Try
            If Global_asax.Identity.IDEmployee IsNot Nothing AndAlso Global_asax.Identity.IDEmployee.HasValue AndAlso Global_asax.Identity.IDEmployee > 0 Then
                lrret = VTPortal.ZonesHelper.GetListAllZones(Global_asax.Identity.IDEmployee)
            Else
                lrret.Status = ErrorCodes.OK
                lrret.ListZones = New List(Of ZoneElement)
            End If

        Catch ex As Exception
            lrret.Status = ErrorCodes.GENERAL_ERROR

            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::PortalSvcx::GetServerZones")
        End Try

        Return lrret
    End Function


    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function GetEmployeeCommuniques() As EmployeeCommuniques
        Dim lrret As New EmployeeCommuniques

        If Not Global_asax.LoggedIn Then
            lrret.Status = VTPortal.StatusHelper.GetStatusFromState(Global_asax.SecurityResult)
            If lrret.Status = ErrorCodes.OK Then lrret.Status = ErrorCodes.NO_SESSION
            Return lrret
        End If

        Try
            If Global_asax.Identity.IDEmployee IsNot Nothing AndAlso Global_asax.Identity.IDEmployee.HasValue AndAlso Global_asax.Identity.IDEmployee > 0 Then
                lrret = VTPortal.EmployeesHelper.GetEmployeeCommuniques(Global_asax.Identity.IDEmployee)
            Else
                lrret.Status = ErrorCodes.OK
                lrret.Communiques = {}
            End If

        Catch ex As Exception
            lrret.Status = ErrorCodes.GENERAL_ERROR

            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::PortalSvcx::GetServerZones")
        End Try

        Return lrret
    End Function

    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function GetEmployeeSurveys() As EmployeeSurveys
        Dim lrret As New EmployeeSurveys

        If Not Global_asax.LoggedIn Then
            lrret.Status = VTPortal.StatusHelper.GetStatusFromState(Global_asax.SecurityResult)
            If lrret.Status = ErrorCodes.OK Then lrret.Status = ErrorCodes.NO_SESSION
            Return lrret
        End If

        Try
            If Global_asax.Identity.IDEmployee IsNot Nothing AndAlso Global_asax.Identity.IDEmployee.HasValue AndAlso Global_asax.Identity.IDEmployee > 0 Then
                lrret = VTPortal.EmployeesHelper.GetEmployeeSurveys(Global_asax.Identity.IDEmployee)
            Else
                lrret.Status = ErrorCodes.OK
                lrret.Surveys = {}
            End If

        Catch ex As Exception
            lrret.Status = ErrorCodes.GENERAL_ERROR

            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::PortalSvcx::GetServerZones")
        End Try

        Return lrret
    End Function

    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function GetEmployeeTelecommutingInfo() As EmployeeTelecommuting
        Dim lrret As New EmployeeTelecommuting

        If Not Global_asax.LoggedIn Then
            lrret.Status = VTPortal.StatusHelper.GetStatusFromState(Global_asax.SecurityResult)
            If lrret.Status = ErrorCodes.OK Then lrret.Status = ErrorCodes.NO_SESSION
            Return lrret
        End If

        Try
            If Global_asax.Identity.IDEmployee IsNot Nothing AndAlso Global_asax.Identity.IDEmployee.HasValue AndAlso Global_asax.Identity.IDEmployee > 0 Then
                lrret = VTPortal.EmployeesHelper.GetEmployeeTelecommuting(Global_asax.Identity.IDEmployee)
            Else
                lrret.Status = ErrorCodes.OK
            End If

        Catch ex As Exception
            lrret.Status = ErrorCodes.GENERAL_ERROR

            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::PortalSvcx::GetServerZones")
        End Try

        Return lrret
    End Function




    ''' <summary>
    ''' Se requiere que la sesión este iniciada en el sistema. Esta función responde a un método GET del servidor.
    ''' Obtiene el estado del empleado en el sistema, hora y sentido del último fichaje, estado de producción y centro de costo. También obtenemos la hora del servidor, así como las alertas pendientes de atender por parte del empleado.
    ''' </summary>
    ''' <returns>Obtenemos un objecto de tipo 'UserStatus' con toda la información relativa al empleado con sesión activa.</returns>
    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function GetEmployeeStatus() As UserStatus
        Dim lrret As New UserStatus

        If Not Global_asax.LoggedIn Then
            lrret.Status = VTPortal.StatusHelper.GetStatusFromState(Global_asax.SecurityResult)
            If lrret.Status = ErrorCodes.OK Then lrret.Status = ErrorCodes.NO_SESSION
            Return lrret
        End If

        Try

            Dim bLoadAlerts As Boolean = Global_asax.IsSupervisor
            bLoadAlerts = bLoadAlerts AndAlso WLHelper.HasPermissionOverFeature(Global_asax.Identity.ID, "Administration.Alerts", "U", Permission.Read)

            If Global_asax.Supervisor IsNot Nothing AndAlso Global_asax.Identity IsNot Nothing Then
                bLoadAlerts = False
            End If

            If Global_asax.Identity.IDEmployee IsNot Nothing AndAlso Global_asax.Identity.IDEmployee.HasValue AndAlso Global_asax.Identity.IDEmployee > 0 Then
                lrret = VTPortal.StatusHelper.GetEmployeeStatus(Global_asax.Identity, Global_asax.Identity.IDEmployee, bLoadAlerts, Global_asax.TimeZone, False)
            Else
                lrret = VTPortal.StatusHelper.GetSupervisorStatus(Global_asax.Identity, Global_asax.TimeZone, bLoadAlerts)
            End If

            lrret.HasAlertPermission = bLoadAlerts
        Catch ex As Exception
            lrret.Status = ErrorCodes.GENERAL_ERROR

            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::PortalSvcx::GetEmployeeStatus")
        End Try

        Return lrret
    End Function

    ''' <summary>
    ''' Se requiere que la sesión este iniciada en el sistema. Esta función responde a un método GET del servidor.
    ''' Obtiene el estado del empleado en el sistema, hora y sentido del último fichaje, estado de producción y centro de costo. También obtenemos la hora del servidor, así como las alertas pendientes de atender por parte del empleado.
    ''' </summary>
    ''' <returns>Obtenemos un objecto de tipo 'UserStatus' con toda la información relativa al empleado con sesión activa.</returns>
    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function GetEmployeeAlerts() As UserStatus
        Dim lrret As New UserStatus

        If Not Global_asax.LoggedIn Then
            lrret.Status = VTPortal.StatusHelper.GetStatusFromState(Global_asax.SecurityResult)
            If lrret.Status = ErrorCodes.OK Then lrret.Status = ErrorCodes.NO_SESSION
            Return lrret
        End If

        Try
            Dim bLoadAlerts As Boolean = Global_asax.IsSupervisor
            bLoadAlerts = bLoadAlerts AndAlso WLHelper.HasPermissionOverFeature(Global_asax.Identity.ID, "Administration.Alerts", "U", Permission.Read)

            If Global_asax.Supervisor IsNot Nothing AndAlso Global_asax.Identity IsNot Nothing Then
                bLoadAlerts = False
            End If

            If Global_asax.Identity.IDEmployee IsNot Nothing AndAlso Global_asax.Identity.IDEmployee.HasValue AndAlso Global_asax.Identity.IDEmployee > 0 Then
                lrret = VTPortal.StatusHelper.GetEmployeeStatus(Global_asax.Identity, Global_asax.Identity.IDEmployee, bLoadAlerts, Global_asax.TimeZone, True)
            Else
                lrret = VTPortal.StatusHelper.GetSupervisorStatus(Global_asax.Identity, Global_asax.TimeZone, True)
            End If

            lrret.HasAlertPermission = bLoadAlerts
        Catch ex As Exception
            lrret.Status = ErrorCodes.GENERAL_ERROR

            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::PortalSvcx::GetEmployeeAlerts")
        End Try

        Return lrret
    End Function

    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function GetOverlappingEmployees(ByVal idRequest As Integer) As OverlappingEmployees
        Dim lrret As New OverlappingEmployees
        Dim dt As New DataTable
        If Not Global_asax.LoggedIn Then
            lrret.Status = VTPortal.StatusHelper.GetStatusFromState(Global_asax.SecurityResult)
            If lrret.Status = ErrorCodes.OK Then lrret.Status = ErrorCodes.NO_SESSION
            Return lrret
        End If

        Try
            Dim oReqState As New Requests.roRequestState
            Dim oEmpState As New Employee.roEmployeeState(Global_asax.Identity.ID)
            roBaseState.SetSessionSmall(oReqState, Global_asax.Identity.ID, Global_asax.ApplicationSource, "")

            Dim oLng As New roLanguage()
            oLng.SetLanguageReference("LiveOne", Global_asax.Identity.Language.Key)

            Dim fileName As String = Path.Combine(HostingEnvironment.ApplicationPhysicalPath, "Resources/userDefault.png")
            Dim fileStream As New FileStream(fileName, FileMode.Open, FileAccess.Read)

            Dim ImageData As Byte()
            ImageData = New Byte(fileStream.Length - 1) {}
            fileStream.Read(ImageData, 0, System.Convert.ToInt32(fileStream.Length))
            fileStream.Close()

            lrret.DefaultImage = "url(data:image/png;base64," & Convert.ToBase64String(ImageData) & ") no-repeat center center"

            dt = VTPortal.RequestsHelper.GetOverlappingEmployees(Global_asax.Identity, idRequest, oLng, oReqState)

            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                Dim overlaps As New Generic.List(Of Overlays)
                Dim idInterno = 0
                For Each oRow As DataRow In dt.Rows

                    If roTypes.Any2Integer(oRow("IdRequest")) <> idRequest Then

                        Dim overlap As New Overlays
                        overlap.IdCounter = idInterno + 1
                        overlap.IdEmployee = oRow("IdEmployee")
                        overlap.EmployeeName = oRow("EmployeeName")
                        overlap.EmployeeGroup = ""
                        overlap.AbsenceType = oRow("Tipo")
                        If Not IsDBNull(oRow("IDCause")) Then
                            overlap.IdCause = oRow("IDCause")
                        End If
                        overlap.EmployeeImage = VTPortal.EmployeesHelper.LoadEmployeeImage(oRow("Image"), oEmpState)
                        If Not IsDBNull(oRow("BeginDate")) Then
                            overlap.AbsenceBeginDate = oRow("BeginDate")
                        End If
                        If Not IsDBNull(oRow("FinishDate")) Then
                            overlap.AbsenceEndDate = oRow("FinishDate")
                        End If
                        If Not IsDBNull(oRow("BeginTime")) Then
                            overlap.AbsenceBeginTime = oRow("BeginTime")
                        End If
                        If Not IsDBNull(oRow("EndTime")) Then
                            overlap.AbsenceEndTime = oRow("EndTime")
                        End If
                        If Not IsDBNull(oRow("CantidadDias")) Then
                            overlap.AbsenceDetails = roTypes.Any2String(oRow("CantidadDias"))
                        Else
                            overlap.AbsenceDetails = ""
                        End If
                        overlap.IsRequest = False
                        ''
                        Dim oLang As New roLanguage()
                        oLang.SetLanguageReference("LiveEmployees", Global_asax.Identity.Language.Key)
                        Dim Params As Generic.List(Of String)
                        Params = New Generic.List(Of String)

                        oLang.ClearUserTokens()

                        Select Case overlap.AbsenceType
                            Case "Days"
                                oLang.AddUserToken(CDate(overlap.AbsenceBeginDate).ToShortDateString)
                                oLang.AddUserToken(CDate(overlap.AbsenceEndDate).ToShortDateString)
                                oLang.AddUserToken(Cause.roCause.GetCauseNameByID(overlap.IdCause))
                                If overlap.AbsenceDetails <> "" Then
                                    oLang.AddUserToken(roTypes.Any2String(overlap.AbsenceDetails))
                                Else
                                    oLang.AddUserToken("")
                                End If
                                overlap.AbsenceResume = oLang.Translate("Employees.ProgrammedAbsencePortal.Literal", "")

                            Case "Hours"
                                oLang.AddUserToken(CDate(overlap.AbsenceBeginDate).ToShortDateString)
                                oLang.AddUserToken(CDate(overlap.AbsenceEndDate).ToShortDateString)
                                oLang.AddUserToken(CDate(roTypes.Any2Time(oRow("Duration")).Value).ToShortTimeString)
                                oLang.AddUserToken(Cause.roCause.GetCauseNameByID(overlap.IdCause))
                                If overlap.AbsenceDetails <> "" Then
                                    oLang.AddUserToken(roTypes.Any2String(overlap.AbsenceDetails))
                                Else
                                    oLang.AddUserToken("")
                                End If
                                If (overlap.AbsenceBeginTime Is Nothing) Then
                                    oLang.AddUserToken("00:00")
                                Else
                                    oLang.AddUserToken(CDate(overlap.AbsenceBeginTime).ToShortTimeString)
                                End If
                                If (overlap.AbsenceEndTime Is Nothing) Then
                                    oLang.AddUserToken("23:59")
                                Else
                                    oLang.AddUserToken(CDate(overlap.AbsenceEndTime).ToShortTimeString)
                                End If
                                overlap.AbsenceResume = oLang.Translate("Employees.ProgrammedCausePortal.Literal", "")

                            Case "Holidays Hours"

                                If Not IsDBNull(oRow("AllDay")) AndAlso roTypes.Any2Integer(oRow("AllDay")) = 1 Then
                                    oLang.AddUserToken(CDate(overlap.AbsenceBeginDate).ToShortDateString)
                                    oLang.AddUserToken(CDate(overlap.AbsenceEndDate).ToShortDateString)
                                    oLang.AddUserToken(Cause.roCause.GetCauseNameByID(overlap.IdCause))
                                    If overlap.AbsenceDetails <> "" Then
                                        oLang.AddUserToken(roTypes.Any2String(overlap.AbsenceDetails))
                                    Else
                                        oLang.AddUserToken("")
                                    End If
                                    overlap.AbsenceResume = oLang.Translate("Employees.ProgrammedHolidaysAllDay.Literal", "")
                                Else
                                    oLang.AddUserToken(CDate(overlap.AbsenceBeginDate).ToShortDateString)
                                    oLang.AddUserToken(CDate(roTypes.Any2Time(oRow("Duration")).Value).ToShortTimeString)
                                    oLang.AddUserToken(Cause.roCause.GetCauseNameByID(overlap.IdCause))
                                    If overlap.AbsenceDetails <> "" Then
                                        oLang.AddUserToken(roTypes.Any2String(overlap.AbsenceDetails))
                                    Else
                                        oLang.AddUserToken("")
                                    End If
                                    If (overlap.AbsenceBeginTime Is Nothing) Then
                                        oLang.AddUserToken("00:00")
                                    Else
                                        oLang.AddUserToken(CDate(overlap.AbsenceBeginTime).ToShortTimeString)
                                    End If
                                    If (overlap.AbsenceEndTime Is Nothing) Then
                                        oLang.AddUserToken("23:59")
                                    Else
                                        oLang.AddUserToken(CDate(overlap.AbsenceEndTime).ToShortTimeString)
                                    End If

                                    overlap.AbsenceResume = oLang.Translate("Employees.ProgrammedHolidaysPortal.Literal", "")
                                End If
                            Case "Holidays"
                                oLang.AddUserToken(CDate(overlap.AbsenceBeginDate).ToShortDateString)
                                oLang.AddUserToken(CDate(overlap.AbsenceEndDate).ToShortDateString)
                                If overlap.AbsenceDetails <> "" Then
                                    oLang.AddUserToken(roTypes.Any2String(overlap.AbsenceDetails))
                                Else
                                    oLang.AddUserToken("")
                                End If

                                overlap.AbsenceResume = oLang.Translate("Employees.PlannedHolidaysPortal.Literal", "")
                            Case "Request"

                                overlap.IsRequest = True

                                Select Case oRow("RequestType")
                                    Case 6 'Vacaciones
                                        oLang.AddUserToken(CDate(overlap.AbsenceBeginDate).ToShortDateString)
                                        oLang.AddUserToken(CDate(overlap.AbsenceEndDate).ToShortDateString)
                                        If overlap.AbsenceDetails <> "" Then
                                            oLang.AddUserToken(roTypes.Any2String(overlap.AbsenceDetails))
                                        Else
                                            oLang.AddUserToken("")
                                        End If
                                        overlap.AbsenceResume = oLang.Translate("Employees.RequestPlannedHolidays.Literal", "")
                                    Case 7 'Ausencia dias
                                        oLang.AddUserToken(CDate(overlap.AbsenceBeginDate).ToShortDateString)
                                        oLang.AddUserToken(CDate(overlap.AbsenceEndDate).ToShortDateString)
                                        oLang.AddUserToken(Cause.roCause.GetCauseNameByID(overlap.IdCause))
                                        If overlap.AbsenceDetails <> "" Then
                                            oLang.AddUserToken(roTypes.Any2String(overlap.AbsenceDetails))
                                        Else
                                            oLang.AddUserToken("")
                                        End If
                                        overlap.AbsenceResume = oLang.Translate("Employees.RequestProgrammedAbsence.Literal", "")
                                    Case 9 'Ausencia horas
                                        oLang.AddUserToken(CDate(overlap.AbsenceBeginDate).ToShortDateString)
                                        oLang.AddUserToken(CDate(overlap.AbsenceEndDate).ToShortDateString)
                                        oLang.AddUserToken(CDate(roTypes.Any2Time(oRow("Duration")).Value).ToShortTimeString)
                                        oLang.AddUserToken(Cause.roCause.GetCauseNameByID(overlap.IdCause))
                                        If overlap.AbsenceDetails <> "" Then
                                            oLang.AddUserToken(roTypes.Any2String(overlap.AbsenceDetails))
                                        Else
                                            oLang.AddUserToken("")
                                        End If
                                        If (overlap.AbsenceBeginTime Is Nothing) Then
                                            oLang.AddUserToken("00:00")
                                        Else
                                            oLang.AddUserToken(CDate(overlap.AbsenceBeginTime).ToShortTimeString)
                                        End If
                                        If (overlap.AbsenceEndTime Is Nothing) Then
                                            oLang.AddUserToken("23:59")
                                        Else
                                            oLang.AddUserToken(CDate(overlap.AbsenceEndTime).ToShortTimeString)
                                        End If
                                        overlap.AbsenceResume = oLang.Translate("Employees.RequestProgrammedCause.Literal", "")
                                    Case 13 'Vacaciones horas

                                        If Not IsDBNull(oRow("AllDay")) AndAlso roTypes.Any2Integer(oRow("AllDay")) = 1 Then
                                            oLang.AddUserToken(CDate(overlap.AbsenceBeginDate).ToShortDateString)
                                            oLang.AddUserToken(CDate(overlap.AbsenceEndDate).ToShortDateString)
                                            oLang.AddUserToken(Cause.roCause.GetCauseNameByID(overlap.IdCause))
                                            If overlap.AbsenceDetails <> "" Then
                                                oLang.AddUserToken(roTypes.Any2String(overlap.AbsenceDetails))
                                            Else
                                                oLang.AddUserToken("")
                                            End If
                                            overlap.AbsenceResume = oLang.Translate("Employees.RequestProgrammedHolidaysAllDay.Literal", "")
                                        Else
                                            oLang.AddUserToken(CDate(overlap.AbsenceBeginDate).ToShortDateString)
                                            oLang.AddUserToken(CDate(roTypes.Any2Time(oRow("Duration")).Value).ToShortTimeString)
                                            oLang.AddUserToken(Cause.roCause.GetCauseNameByID(overlap.IdCause))
                                            If overlap.AbsenceDetails <> "" Then
                                                oLang.AddUserToken(roTypes.Any2String(overlap.AbsenceDetails))
                                            Else
                                                oLang.AddUserToken("")
                                            End If
                                            If (overlap.AbsenceBeginTime Is Nothing) Then
                                                oLang.AddUserToken("00:00")
                                            Else
                                                oLang.AddUserToken(CDate(overlap.AbsenceBeginTime).ToShortTimeString)
                                            End If
                                            If (overlap.AbsenceEndTime Is Nothing) Then
                                                oLang.AddUserToken("23:59")
                                            Else
                                                oLang.AddUserToken(CDate(overlap.AbsenceEndTime).ToShortTimeString)
                                            End If
                                            overlap.AbsenceResume = oLang.Translate("Employees.RequestProgrammedHolidays.Literal", "")
                                        End If

                                End Select

                        End Select

                        oLang.ClearUserTokens()

                        overlaps.Add(overlap)
                        idInterno = idInterno + 1
                    End If

                Next
                lrret.Overlays = overlaps.ToArray()
            End If
        Catch ex As Exception
            lrret.Status = ErrorCodes.GENERAL_ERROR

            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::PortalSvcx::GetOverlappingEmployees")
        End Try

        Return lrret
    End Function

    ''' <summary>
    ''' Se requiere que la sesión este iniciada en el sistema. Esta función responde a un método GET del servidor.
    ''' Obtenemos la imagen asociada al empleado que solicitamos.
    ''' </summary>
    ''' <param name="employeeId">Identificador de empleado, si el identificador asociado és un -1 se obtendrá la imagen del empleado que ha iniciado sesión.</param>
    ''' <returns>Obtenemos un objeto de tipo imagen, con una cadena base64 que representa la imagen y el código de error en caso de producirse uno.</returns>
    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function GetWsEmployeePhoto(ByVal employeeId As Integer) As WsImage
        Dim bolPhoto As Boolean = False

        Dim tCount As New WsImage()
        Try
            If Not Global_asax.LoggedIn Then
                tCount.Status = VTPortal.StatusHelper.GetStatusFromState(Global_asax.SecurityResult)
                If tCount.Status = ErrorCodes.OK Then tCount.Status = ErrorCodes.NO_SESSION
                Return tCount
            End If

            Dim oEmpId As Integer = If(employeeId = -1, Global_asax.Identity.IDEmployee, employeeId)
            Dim bHasPermission = SecurityHelper.HasFeaturePermissionByEmployee(Global_asax.Identity.ID, "Employees", Permission.Read, oEmpId) OrElse oEmpId = Global_asax.Identity.IDEmployee

            If Not bHasPermission Then
                tCount.Status = ErrorCodes.GENERAL_ERROR_NoPermissions
                Return tCount
            End If

            'Cargamos el stream con la foto a partir del nombre y userID
            Dim fileName As String = Path.Combine(HostingEnvironment.ApplicationPhysicalPath, "Resources/userDefault.png")
            Dim fileStream As New FileStream(fileName, FileMode.Open, FileAccess.Read)
            Dim oEmployee As Employee.roEmployee = Nothing

            Try
                If SecurityHelper.HasFeaturePermissionByEmployee(Global_asax.Identity.ID, "Employees.NameFoto", Permission.Read, oEmpId) OrElse oEmpId = Global_asax.Identity.IDEmployee Then oEmployee = Employee.roEmployee.GetEmployee(oEmpId, New Employee.roEmployeeState)

                oEmployee = Employee.roEmployee.GetEmployee(oEmpId, New Employee.roEmployeeState)

                If oEmployee IsNot Nothing AndAlso oEmployee.Image IsNot Nothing Then
                    bolPhoto = True
                End If
            Catch ex As Exception
                bolPhoto = False
            End Try

            Dim ImageData As Byte()
            If bolPhoto Then
                ImageData = oEmployee.Image
            Else
                ImageData = New Byte(fileStream.Length - 1) {}
                fileStream.Read(ImageData, 0, System.Convert.ToInt32(fileStream.Length))
                fileStream.Close()
            End If

            tCount.Base64StringContent = Convert.ToBase64String(ImageData)
            tCount.Status = ErrorCodes.OK
        Catch ex As Exception
            tCount.Status = ErrorCodes.GENERAL_ERROR

            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::PortalSvcx::GetWsEmployeePhoto")
        End Try

        Return tCount
    End Function

    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function CurrentEmployeesZoneStatus(ByVal currentZone As Integer, ByVal user As String, ByVal password As String) As EmployeeList
        Dim lrret As New EmployeeList()

        lrret.Status = ErrorCodes.OK

        Dim userM As New AdvancedParameter.roAdvancedParameter("AccessMonitor.User", New AdvancedParameter.roAdvancedParameterState)
        Dim passM As New AdvancedParameter.roAdvancedParameter("AccessMonitor.Pass", New AdvancedParameter.roAdvancedParameterState)

        If user = userM.Value AndAlso password = passM.Value Then
        Else
            lrret.Status = VTPortal.StatusHelper.GetStatusFromState(Global_asax.SecurityResult)
            If lrret.Status = ErrorCodes.OK Then lrret.Status = ErrorCodes.NO_SESSION
            Return lrret
        End If

        Try
            Dim oEmpState As New Employee.roEmployeeState(-1)
            lrret = VTPortal.EmployeesHelper.CurrentEmployeesZoneStatus(currentZone, oEmpState)
        Catch ex As Exception
            lrret.Status = ErrorCodes.GENERAL_ERROR
            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::PortalSvcx::CurrentEmployeesZoneStatus")
        End Try

        Return lrret
    End Function

    ''' <summary>
    ''' Se requiere que la sesión este iniciada en el sistema. Esta función responde a un método GET del servidor.
    ''' Obtenemos la imagen asociada al empleado que solicitamos.
    ''' </summary>
    ''' <param name="viewName">Identificador vista.</param>
    ''' <returns>Obtenemos una vista para mostrar en el portal del empleado.</returns>
    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function GetView(ByVal viewName As String) As WsView
        Dim bolPhoto As Boolean = False
        Dim oMemoryStream = New MemoryStream

        Dim tCount As New WsView()
        Try
            'If Not Global_asax.LoggedIn Then
            '    tCount.Status = VTPortal.StatusHelper.GetStatusFromState(Global_asax.SecurityResult)
            '    If tCount.Status = ErrorCodes.OK Then tCount.Status = ErrorCodes.NO_SESSION
            '    Return tCount
            'End If

            'Cargamos el stream con la foto a partir del nombre y userID
            Dim fileName As String = Path.Combine(HostingEnvironment.ApplicationPhysicalPath, "Resources/dynamicView.txt")
            Dim fileJSName As String = Path.Combine(HostingEnvironment.ApplicationPhysicalPath, "Resources/dynamicView.js")

            tCount.viewContent = File.ReadAllText(fileName)
            tCount.jsContent = File.ReadAllText(fileJSName)
            tCount.Status = ErrorCodes.OK
        Catch ex As Exception
            tCount.Status = ErrorCodes.GENERAL_ERROR

            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::PortalSvcx::GetWsEmployeePhoto")
        End Try

        Return tCount
    End Function

    ''' <summary>
    ''' Se requiere que la sesión este iniciada en el sistema. Esta función responde a un método GET del servidor.
    ''' Obtenemos la imagen que representa la empresa donde trabaja el empleado.
    ''' </summary>
    ''' <returns>Obtenemos un objeto de tipo imagen, con una cadena base64 que representa la imagen y el código de error en caso de producirse uno.</returns>
    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function GetWsBackgroundImage() As roPortalConfigurationForPortal
        Dim result As New roPortalConfigurationForPortal
        Try
            If Not Global_asax.LoggedIn Then
                result.Status = VTPortal.StatusHelper.GetStatusFromState(Global_asax.SecurityResult)
                If result.Status = ErrorCodes.OK Then result.Status = ErrorCodes.NO_SESSION
                Return result
            End If

            Try

                Dim oParameter As New Common.AdvancedParameter.roAdvancedParameter("VTPortal.HeaderConfiguration", New Common.AdvancedParameter.roAdvancedParameterState)
                If Not IsDBNull(oParameter.Value) AndAlso oParameter.Value <> String.Empty Then
                    result.PortalConfiguration = oParameter.Value
                Else
                    result.PortalConfiguration = ""
                End If
            Catch ex As Exception

            End Try

            result.Status = ErrorCodes.OK
        Catch ex As Exception
            result.Status = ErrorCodes.GENERAL_ERROR

            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::PortalSvcx::GetWsBackgroundImage")
        End Try

        Return result
    End Function

    ''' <summary>
    ''' Se requiere que la sesión este iniciada en el sistema. Esta función responde a un método GET del servidor.
    ''' Obtenemos un listado de elementos formados por Nombre/Valor en función del tipo de petición que realicemos.
    ''' </summary>
    ''' <param name="requestType">Tipo de petición que estamos solicitando, este parametro acepta varios valores:
    '''     'userfields': Listado de la ficha del empleado.
    '''     'shifts.permissiontype': Listado de los horarios de vacaciones que tiene disponibles el empleado.
    '''     'shifts.workingtype': Listado de los horarios laborales que tiene disponibles el empleado.
    '''     'shifts': Listado completo de horarios.
    '''     'causes.visibilitypermissions': Justificaciones que puede utilizar el empleado en las distintas solicitudes de sistema.
    '''     'causes.externalwork': Listado de justificaciones que puede utilizar el empledado para indicar trabajo externo.
    '''     'causes.readerinputcode': Justificaciones que puede utilizar el empleado en los fichajes.
    '''     'causes': Listado completo de justificaciones disponibles.
    '''     'startshifts': Valores posibles de inicio de un horario.
    '''     'costcenters': Centros de costo en los que el empleado dispone de permisos para fichar.
    '''     'costcentersfull': Listao completo de centros de costo del sistema.
    '''     'tasks.availabletasks': Listado de tareas en las que el empleado tiene permisos para fichar.
    ''' </param>
    ''' <returns>Obtenemos el listado de valores disponible para la petición solicitada, así como el código de error en caso de producirse.</returns>
    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function GetGenericList(ByVal requestType As String) As GenericList
        Dim lrret As New GenericList

        Try
            If Not Global_asax.LoggedIn Then
                lrret.Status = VTPortal.StatusHelper.GetStatusFromState(Global_asax.SecurityResult)
                If lrret.Status = ErrorCodes.OK Then lrret.Status = ErrorCodes.NO_SESSION
                Return lrret
            End If

            Dim oLng As New roLanguage()
            oLng.SetLanguageReference("VTPortal", Global_asax.Identity.Language.Key)

            Dim oState As New Employee.roEmployeeState(Global_asax.Identity.ID)
            roBaseState.SetSessionSmall(oState, Global_asax.Identity.ID, Global_asax.ApplicationSource, "")

            If Global_asax.Identity.IDEmployee IsNot Nothing Then
                lrret = VTPortal.CommonHelper.GetGenericList(Global_asax.Identity, requestType, oLng, oState)
            Else
                lrret = New GenericList() With {
                    .SelectFields = {},
                    .Status = ErrorCodes.OK
                    }
            End If
        Catch ex As Exception
            lrret.Status = ErrorCodes.GENERAL_ERROR

            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::PortalSvcx::GetGenericList")
        End Try

        Return lrret
    End Function

    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function GetTasksByName(ByVal taskName As String) As GenericList
        Dim lrret As New GenericList

        Try
            If Not Global_asax.LoggedIn Then
                lrret.Status = VTPortal.StatusHelper.GetStatusFromState(Global_asax.SecurityResult)
                If lrret.Status = ErrorCodes.OK Then lrret.Status = ErrorCodes.NO_SESSION
                Return lrret
            End If

            Dim oLng As New roLanguage()
            oLng.SetLanguageReference("VTPortal", Global_asax.Identity.Language.Key)

            Dim oState As New Employee.roEmployeeState(Global_asax.Identity.ID)
            roBaseState.SetSessionSmall(oState, Global_asax.Identity.ID, Global_asax.ApplicationSource, "")

            If Global_asax.Identity.IDEmployee IsNot Nothing Then

                lrret = VTPortal.CommonHelper.GetTasksByName(Global_asax.Identity, taskName, oLng, oState)
            Else
                lrret = New GenericList() With {
                    .SelectFields = {},
                    .Status = ErrorCodes.OK
                    }
            End If
        Catch ex As Exception
            lrret.Status = ErrorCodes.GENERAL_ERROR

            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::PortalSvcx::GetGenericList")
        End Try

        Return lrret
    End Function

    ''' <summary>
    ''' Se requiere que la sesión este iniciada en el sistema. Esta función responde a un método GET del servidor.
    ''' Realiza un fichaje de presencia en el sistema para el empleado que ha iniciado la sesión.
    ''' </summary>
    ''' <param name="causeId">Identificador de la justificación que se ha empledo para fichar. El identificador 0 se utiliza para indicar un 'Sin motivo'</param>
    ''' <param name="direction">Dirección del fichaje que se va a introducir en el sistema. 'E' para indicar entrada, 'S' para indicar salida</param>
    ''' <param name="latitude">Latitud de la posición en la que se ha realizado el fichaje, en caso de no disponer de ella indicar un 0</param>
    ''' <param name="longitude">Longitud de la posición en la que se ha realizado el fichaje, en caso de no disponer de ella indicar un 0</param>
    ''' <param name="identifier"></param>
    ''' <param name="locationZone">Población en la que se ha realizado el fichaje, en caso de no disponer de ella indicar una cadena en blanco</param>
    ''' <param name="fullAddress">Dirección completa en la que se ha realizado el fichaje, en caso de no disponer de ella indicar una cadena en blanco</param>
    ''' <param name="reliable">Fiabilidad del fichaje, valores disponibles són: True/False</param>
    ''' <param name="reliableZone">Fiabilidad de la zona del fichaje, valores disponibles són: True/False</param>
    ''' <param name="selectedZone">Zona seleccionada por el usuario</param>
    ''' <returns>Obtenemos un objeto del tipo UserEstatus, indicando el nuevo estado del empleado una vez realizado el fichaje, si este no se ha podido guardar se indica en la variable de estado del objeto de retorno.</returns>
    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function SetStatus(ByVal causeId As Integer, ByVal direction As String, ByVal latitude As String, ByVal longitude As String, ByVal identifier As String, ByVal locationZone As String, ByVal fullAddress As String, ByVal reliable As Boolean, ByVal nfcTag As String, ByVal tcType As String, ByVal Optional reliableZone As Boolean = True, Optional ByVal selectedZone As Integer = -1) As UserStatus
        Dim lrret As New UserStatus
        If Not Global_asax.LoggedIn Then
            lrret.Status = VTPortal.StatusHelper.GetStatusFromState(Global_asax.SecurityResult)
            If lrret.Status = ErrorCodes.OK Then lrret.Status = ErrorCodes.NO_SESSION
            Return lrret
        End If

        lrret.Status = ErrorCodes.OK

        Try
            Dim logMsg = "PunchRecieved::IDEmployee:{0}:direction:{1}:causeId:{2}:latitude:{3}:longitude:{4}:identifier:{5}:locationZone:{6}:fullAddress:{7}:reliable:{8}:nfcTag:{9}:tcType:{10}:reliableZone:{11}:selectedzone:{12}"
            logMsg = String.Format(logMsg, Global_asax.Identity.ID, direction, causeId, latitude, longitude, identifier, locationZone, fullAddress, reliable, nfcTag, tcType, reliableZone, selectedZone)

            roLog.GetInstance.logMessage(roLog.EventType.roDebug, logMsg)

            Dim isApp As Boolean = HttpContext.Current.Request.Headers("roSrc") IsNot Nothing AndAlso roTypes.Any2Boolean(HttpContext.Current.Request.Headers("roSrc"))

            Dim oParamState As New AdvancedParameter.roAdvancedParameterState
            roBaseState.SetSessionSmall(oParamState, Global_asax.Identity.ID, Global_asax.ApplicationSource, "")
            Dim oParameter As New AdvancedParameter.roAdvancedParameter("VTLive.IPRestriction.OnlyPunches", oParamState)
            Dim proceedPunch As Boolean = True

            If roTypes.Any2Boolean(oParameter.Value) Then
                Dim strClientLocation As String = oParamState.ClientAddress.Split("#")(0)
                proceedPunch = AuthHelper.IsValidClientLocation(Global_asax.Identity.ID, strClientLocation)
            End If

            Dim isTelecommute As Boolean = False

            If direction = "S" Then
                isTelecommute = False
            Else
                isTelecommute = roTypes.Any2Boolean(HttpContext.Current.Request.Params("tcType"))
            End If

            Dim statusInfoMsg As String = ""

            Dim oZone As Zone.roZone = Nothing
            If Not VTPortal.PunchHelper.CheckCapacityOnPunch(direction, latitude, longitude, Global_asax.Identity.IDEmployee, oZone) Then
                Dim oSetStatusLang As New roLanguage()
                oSetStatusLang.SetLanguageReference("PunchService", Global_asax.Identity.Language.Key)
                oSetStatusLang.ClearUserTokens()
                oSetStatusLang.AddUserToken(oZone.Capacity.ToString)
                statusInfoMsg = oSetStatusLang.Translate("MaxCapacityExceeded.Info", "")
                oSetStatusLang.ClearUserTokens()
            End If

            If proceedPunch Then
                Dim dLatitude As Double = roTypes.Any2Double(latitude.Replace(".", cCulture.NumberFormat.NumberDecimalSeparator), cCulture)
                Dim dLongitude As Double = roTypes.Any2Double(longitude.Replace(".", cCulture.NumberFormat.NumberDecimalSeparator), cCulture)
                Dim notReliableCause As String = Nothing
                If Not reliable Then
                    notReliableCause = DTOs.NotReliableCause.PunchWithoutLocation.ToString()
                End If
                Dim tmpResponse As StdResponse = VTPortal.PunchHelper.SavePunch(Global_asax.Identity, Global_asax.TerminalIdentity, Global_asax.Identity.IDEmployee, Global_asax.TimeZone, causeId, direction, dLatitude, dLongitude, identifier, locationZone, fullAddress, Nothing, reliable, nfcTag, Nothing, False, isTelecommute,, reliableZone, selectedZone, oParamState.ClientIP, isApp,, notReliableCause)
                lrret.Status = tmpResponse.Status
                If lrret.Status = ErrorCodes.OK Then
                    lrret = GetEmployeeStatus()
                End If
                lrret.ExistsCause = causeId
            Else
                lrret.Status = ErrorCodes.LOGIN_INVALID_CLIENT_LOCATION
            End If

            If statusInfoMsg <> "" Then
                lrret.StatusInfoMsg = statusInfoMsg
            End If
        Catch ex As Exception
            lrret.Status = ErrorCodes.GENERAL_ERROR
            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::PortalSvcx::SetStatus")
        End Try

        Return lrret
    End Function

    ''' <summary>
    ''' Se requiere que la sesión este iniciada en el sistema. Esta función responde a un método POST del servidor.
    ''' Realiza un fichaje de presencia en el sistema para el empleado que ha iniciado la sesión añadiendo una fotografia del mismo.
    ''' Esta función recibe los mismos parametros que en la petición equivalente sin fotografia con un objeto imagen en la propiedad Request.Files
    ''' </summary>
    ''' <returns>Obtenemos un objeto del tipo UserEstatus, indicando el nuevo estado del empleado una vez realizado el fichaje, si este no se ha podido guardar se indica en la variable de estado del objeto de retorno.</returns>
    <OperationContract()>
    <WebInvoke(Method:="POST", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function SetStatusWithPhoto() As UserStatus
        Dim lrret As New UserStatus()

        If Not Global_asax.LoggedIn Then
            lrret.Status = VTPortal.StatusHelper.GetStatusFromState(Global_asax.SecurityResult)
            If lrret.Status = ErrorCodes.OK Then lrret.Status = ErrorCodes.NO_SESSION
            Return lrret
        End If

        lrret.Status = ErrorCodes.OK
        Try
            Dim isApp As Boolean = HttpContext.Current.Request.Headers("roSrc") IsNot Nothing AndAlso roTypes.Any2Boolean(HttpContext.Current.Request.Headers("roSrc"))

            Dim oParamState As New AdvancedParameter.roAdvancedParameterState
            roBaseState.SetSessionSmall(oParamState, Global_asax.Identity.ID, Global_asax.ApplicationSource, "")
            Dim oParameter As New AdvancedParameter.roAdvancedParameter("VTLive.IPRestriction.OnlyPunches", oParamState)
            Dim proceedPunch As Boolean = True
            Dim statusInfoMsg As String = ""
            If roTypes.Any2Boolean(oParameter.Value) Then
                Dim strClientLocation As String = oParamState.ClientAddress.Split("#")(0)
                proceedPunch = AuthHelper.IsValidClientLocation(Global_asax.Identity.ID, strClientLocation)
            End If

            If proceedPunch Then
                Dim curFile As HttpPostedFile = HttpContext.Current.Request.Files("userfile")
                If curFile Is Nothing Then curFile = HttpContext.Current.Request.Files("file")

                Dim causeId As Integer = HttpContext.Current.Request.Params("causeId")
                Dim direction As String = HttpContext.Current.Request.Params("direction")
                Dim latitudeStr As String = HttpContext.Current.Request.Params("latitude")
                Dim longitudeStr As String = HttpContext.Current.Request.Params("longitude")
                Dim identifier As String = HttpContext.Current.Request.Params("identifier")
                Dim locationZone As String = HttpContext.Current.Request.Params("locationZone")
                Dim fullAddress As String = HttpContext.Current.Request.Params("fullAddress")
                Dim reliable As Boolean = roTypes.Any2Boolean(HttpContext.Current.Request.Params("reliable"))

                Dim latitude As Double = roTypes.Any2Double(latitudeStr.Replace(".", cCulture.NumberFormat.NumberDecimalSeparator), cCulture)
                Dim longitude As Double = roTypes.Any2Double(longitudeStr.Replace(".", cCulture.NumberFormat.NumberDecimalSeparator), cCulture)

                Dim nfcTag As String = HttpContext.Current.Request.Params("nfcTag")
                Dim isTelecommute As Boolean = roTypes.Any2Boolean(HttpContext.Current.Request.Params("tcType"))
                Dim selectedZone As Integer = roTypes.Any2Integer(HttpContext.Current.Request.Params("selectedZone"))
                Dim isReliableZone As Boolean = roTypes.Any2Boolean(HttpContext.Current.Request.Params("reliableZone"))

                Dim logMsg = "PunchRecieved::IDEmployee:{0}:direction:{1}:causeId:{2}:latitude:{3}:longitude:{4}:identifier:{5}:locationZone:{6}:fullAddress:{7}:reliable:{8}:nfcTag:{9}:tcType:{10}:reliableZone:{11}:selectedzone:{12}"
                logMsg = String.Format(logMsg, Global_asax.Identity.ID, direction, causeId, latitude, longitude, identifier, locationZone, fullAddress, reliable, nfcTag, isTelecommute, isReliableZone, selectedZone)

                roLog.GetInstance.logMessage(roLog.EventType.roDebug, logMsg)

                Dim oZone As Zone.roZone = Nothing
                If Not VTPortal.PunchHelper.CheckCapacityOnPunch(direction, latitudeStr, longitudeStr, Global_asax.Identity.IDEmployee, oZone) Then
                    Dim oSetStatusLang As New roLanguage()
                    oSetStatusLang.SetLanguageReference("PunchService", Global_asax.Identity.Language.Key)
                    oSetStatusLang.ClearUserTokens()
                    oSetStatusLang.AddUserToken(oZone.Capacity.ToString)
                    statusInfoMsg = oSetStatusLang.Translate("MaxCapacityExceeded.Info", "")
                    oSetStatusLang.ClearUserTokens()
                End If

                Dim punchImage As Image = New Bitmap(curFile.InputStream)

                Dim notReliableCause As String = Nothing
                If Not reliable Then
                    notReliableCause = DTOs.NotReliableCause.PunchWithoutLocation.ToString()
                End If

                Dim tmpResponse As StdResponse = VTPortal.PunchHelper.SavePunch(Global_asax.Identity, Global_asax.TerminalIdentity, Global_asax.Identity.IDEmployee, Global_asax.TimeZone,
                                                                                causeId, direction, latitude, longitude, identifier, locationZone, fullAddress, punchImage, reliable,
                                                                                nfcTag, Nothing, False, isTelecommute,, isReliableZone, selectedZone, oParamState.ClientIP, isApp,, notReliableCause)
                lrret.Status = tmpResponse.Status

                If lrret.Status = ErrorCodes.OK Then
                    lrret = GetEmployeeStatus()
                End If

                lrret.ExistsCause = causeId

                If statusInfoMsg <> "" Then
                    lrret.StatusInfoMsg = statusInfoMsg
                End If
            Else
                lrret.Status = ErrorCodes.LOGIN_INVALID_CLIENT_LOCATION
            End If
        Catch ex As Exception

            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::PortalSvcx::SetStatusWithPhoto")

            lrret.Status = ErrorCodes.GENERAL_ERROR
        End Try

        Return lrret
    End Function

    ''' <summary>
    ''' Se requiere que la sesión este iniciada en el sistema. Esta función responde a un método GET del servidor.
    ''' Realiza un fichaje de centro de costo en el sistema para el empleado que ha iniciado la sesión.
    ''' </summary>
    ''' <param name="costCenterId">Identificador del centro de costo en el que se va a relizar el fichaje</param>
    ''' <param name="latitude">Latitud de la posición en la que se ha realizado el fichaje, en caso de no disponer de ella indicar un 0</param>
    ''' <param name="longitude">Longitud de la posición en la que se ha realizado el fichaje, en caso de no disponer de ella indicar un 0</param>
    ''' <param name="identifier"></param>
    ''' <param name="locationZone">Población en la que se ha realizado el fichaje, en caso de no disponer de ella indicar una cadena en blanco</param>
    ''' <param name="fullAddress">Dirección completa en la que se ha realizado el fichaje, en caso de no disponer de ella indicar una cadena en blanco</param>
    ''' <param name="reliable">Fiabilidad del fichaje, valores disponibles són: True/False</param>
    ''' <returns>Obtenemos un objeto del tipo UserEstatus, indicando el nuevo estado del empleado una vez realizado el fichaje, si este no se ha podido guardar se indica en la variable de estado del objeto de retorno.</returns>
    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function SetCostCenterStatus(ByVal costCenterId As Integer, ByVal latitude As String, ByVal longitude As String, ByVal identifier As String, ByVal locationZone As String, ByVal fullAddress As String, ByVal reliable As Boolean, ByVal tcType As String) As UserStatus
        Dim lrret As New UserStatus
        If Not Global_asax.LoggedIn Then
            lrret.Status = VTPortal.StatusHelper.GetStatusFromState(Global_asax.SecurityResult)
            If lrret.Status = ErrorCodes.OK Then lrret.Status = ErrorCodes.NO_SESSION
            Return lrret
        End If

        lrret.Status = ErrorCodes.OK
        Try

            Dim oParamState As New AdvancedParameter.roAdvancedParameterState
            roBaseState.SetSessionSmall(oParamState, Global_asax.Identity.ID, Global_asax.ApplicationSource, "")
            Dim oParameter As New AdvancedParameter.roAdvancedParameter("VTLive.IPRestriction.OnlyPunches", oParamState)
            Dim proceedPunch As Boolean = True

            Dim isApp As Boolean = HttpContext.Current.Request.Headers("roSrc") IsNot Nothing AndAlso roTypes.Any2Boolean(HttpContext.Current.Request.Headers("roSrc"))

            If roTypes.Any2Boolean(oParameter.Value) Then
                Dim strClientLocation As String = oParamState.ClientAddress.Split("#")(0)
                proceedPunch = AuthHelper.IsValidClientLocation(Global_asax.Identity.ID, strClientLocation)
            End If

            If proceedPunch Then
                Dim dLatitude As Double = roTypes.Any2Double(latitude.Replace(".", cCulture.NumberFormat.NumberDecimalSeparator), cCulture)
                Dim dLongitude As Double = roTypes.Any2Double(longitude.Replace(".", cCulture.NumberFormat.NumberDecimalSeparator), cCulture)
                Dim notReliableCause As String = Nothing
                If Not reliable Then
                    notReliableCause = DTOs.NotReliableCause.PunchWithoutLocation.ToString()
                End If

                Dim tmpResponse As StdResponse = VTPortal.CostCenterHelper.SaveCostCenterPunch(Global_asax.Identity, Global_asax.Identity.IDEmployee, Global_asax.TimeZone, costCenterId,
                                                                                               dLatitude, dLongitude, identifier, locationZone, fullAddress, Nothing, reliable,, isApp,, notReliableCause)

                If tmpResponse.Result Then
                    lrret = GetEmployeeStatus()
                    lrret.Status = tmpResponse.Status
                Else
                    lrret.Status = ErrorCodes.GENERAL_ERROR
                End If
            Else
                lrret.Status = ErrorCodes.LOGIN_INVALID_CLIENT_LOCATION

            End If
        Catch ex As Exception
            lrret.Status = ErrorCodes.GENERAL_ERROR
            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::PortalSvcx::SetCostCenterStatus")
        End Try

        Return lrret
    End Function

    ''' <summary>
    ''' Se requiere que la sesión este iniciada en el sistema. Esta función responde a un método POST del servidor.
    ''' Realiza un fichaje de centro de costo en el sistema para el empleado que ha iniciado la sesión.
    ''' Esta función recibe los mismos parametros que en la petición equivalente sin fotografia con un objeto imagen en la propiedad Request.Files
    ''' </summary>
    ''' <returns>Obtenemos un objeto del tipo UserEstatus, indicando el nuevo estado del empleado una vez realizado el fichaje, si este no se ha podido guardar se indica en la variable de estado del objeto de retorno.</returns>
    <OperationContract()>
    <WebInvoke(Method:="POST", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function SetCostCenterPunchWithPhoto() As UserStatus
        Dim lrret As New UserStatus()
        If Not Global_asax.LoggedIn Then
            lrret.Status = VTPortal.StatusHelper.GetStatusFromState(Global_asax.SecurityResult)
            If lrret.Status = ErrorCodes.OK Then lrret.Status = ErrorCodes.NO_SESSION
            Return lrret
        End If

        lrret.Status = ErrorCodes.OK

        Try

            Dim oParamState As New AdvancedParameter.roAdvancedParameterState
            roBaseState.SetSessionSmall(oParamState, Global_asax.Identity.ID, Global_asax.ApplicationSource, "")
            Dim oParameter As New AdvancedParameter.roAdvancedParameter("VTLive.IPRestriction.OnlyPunches", oParamState)
            Dim proceedPunch As Boolean = True

            Dim isApp As Boolean = HttpContext.Current.Request.Headers("roSrc") IsNot Nothing AndAlso roTypes.Any2Boolean(HttpContext.Current.Request.Headers("roSrc"))

            If roTypes.Any2Boolean(oParameter.Value) Then
                Dim strClientLocation As String = oParamState.ClientAddress.Split("#")(0)
                proceedPunch = AuthHelper.IsValidClientLocation(Global_asax.Identity.ID, strClientLocation)
            End If

            If proceedPunch Then
                Dim curFile As HttpPostedFile = HttpContext.Current.Request.Files("userfile")
                If curFile Is Nothing Then curFile = HttpContext.Current.Request.Files("file")

                Dim costCenterId As Integer = HttpContext.Current.Request.Params("costCenterId")
                Dim latitudeStr As String = HttpContext.Current.Request.Params("latitude")
                Dim longitudeStr As String = HttpContext.Current.Request.Params("longitude")
                Dim identifier As String = HttpContext.Current.Request.Params("identifier")
                Dim locationZone As String = HttpContext.Current.Request.Params("locationZone")
                Dim fullAddress As String = HttpContext.Current.Request.Params("fullAddress")
                Dim reliable As Boolean = roTypes.Any2Boolean(HttpContext.Current.Request.Params("reliable"))

                Dim latitude As Double = roTypes.Any2Double(latitudeStr.Replace(".", cCulture.NumberFormat.NumberDecimalSeparator), cCulture)
                Dim longitude As Double = roTypes.Any2Double(longitudeStr.Replace(".", cCulture.NumberFormat.NumberDecimalSeparator), cCulture)

                Dim punchImage As Image = New Bitmap(curFile.InputStream)
                Dim isTelecommute As Boolean = roTypes.Any2Boolean(HttpContext.Current.Request.Params("tcType"))

                Dim reliableCause As String = Nothing
                If Not reliable Then
                    reliableCause = DTOs.NotReliableCause.PunchWithoutLocation.ToString()
                End If

                Dim tmpResponse As StdResponse = VTPortal.CostCenterHelper.SaveCostCenterPunch(Global_asax.Identity, Global_asax.Identity.IDEmployee, Global_asax.TimeZone, costCenterId,
                                                                                               latitude, longitude, identifier, locationZone, fullAddress, punchImage, reliable,, isApp,, reliableCause)

                If tmpResponse.Result Then
                    lrret = GetEmployeeStatus()
                    lrret.Status = tmpResponse.Status
                Else
                    lrret.Status = ErrorCodes.GENERAL_ERROR
                End If
            Else
                lrret.Status = ErrorCodes.LOGIN_INVALID_CLIENT_LOCATION

            End If
        Catch ex As Exception
            lrret.Status = ErrorCodes.GENERAL_ERROR
            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::PortalSvcx::SetCostCenterPunchWithPhoto")
        End Try

        Return lrret
    End Function

    ''' <summary>
    ''' Se requiere que la sesión este iniciada en el sistema. Esta función responde a un método GET del servidor.
    ''' Realiza un fichaje de tareas en el sistema para el empleado que ha iniciado la sesión.
    ''' </summary>
    ''' <param name="taskId">Tarea en la que se esta trabajando antes de realizar el fichaje</param>
    ''' <param name="newTaskId">Tarea en la que se empieza a trabajar</param>
    ''' <param name="latitude">Latitud de la posición en la que se ha realizado el fichaje, en caso de no disponer de ella indicar un 0</param>
    ''' <param name="longitude">Longitud de la posición en la que se ha realizado el fichaje, en caso de no disponer de ella indicar un 0</param>
    ''' <param name="identifier"></param>
    ''' <param name="locationZone">Población en la que se ha realizado el fichaje, en caso de no disponer de ella indicar una cadena en blanco</param>
    ''' <param name="fullAddress">Dirección completa en la que se ha realizado el fichaje, en caso de no disponer de ella indicar una cadena en blanco</param>
    ''' <param name="oldValue0">Parametro de tipo cadena que informa el atributo 1 del fichaje de salida en la tarea anterior</param>
    ''' <param name="oldValue1">Parametro de tipo cadena que informa el atributo 2 del fichaje de salida en la tarea anterior</param>
    ''' <param name="oldValue2">Parametro de tipo cadena que informa el atributo 3 del fichaje de salida en la tarea anterior</param>
    ''' <param name="oldValue3">Parametro de tipo numérico que informa el atributo 4 del fichaje de salida en la tarea anterior</param>
    ''' <param name="oldValue4">Parametro de tipo numérico que informa el atributo 5 del fichaje de salida en la tarea anterior</param>
    ''' <param name="oldValue5">Parametro de tipo numérico que informa el atributo 6 del fichaje de salida en la tarea anterior</param>
    ''' <param name="newValue0">Parametro de tipo cadena que informa el atributo 1 del fichaje en el que empezará a trabajar el empleado</param>
    ''' <param name="newValue1">Parametro de tipo cadena que informa el atributo 2 del fichaje en el que empezará a trabajar el empleado</param>
    ''' <param name="newValue2">Parametro de tipo cadena que informa el atributo 3 del fichaje en el que empezará a trabajar el empleado</param>
    ''' <param name="newValue3">Parametro de tipo cadenanumérico que informa el atributo 4 del fichaje en el que empezará a trabajar el empleado</param>
    ''' <param name="newValue4">Parametro de tipo numérico que informa el atributo 5 del fichaje en el que empezará a trabajar el empleado</param>
    ''' <param name="newValue5">Parametro de tipo numérico que informa el atributo 6 del fichaje en el que empezará a trabajar el empleado</param>
    ''' <param name="completeTask">Indica si se ha compleato la tarea inicial marcandola como no disponible para otros empleados y notificando al supervisor para que valide el cierre de tarea</param>
    ''' <param name="reliable">Fiabilidad del fichaje, valores disponibles són: True/False</param>
    ''' <returns>Obtenemos un objeto del tipo UserEstatus, indicando el nuevo estado del empleado una vez realizado el fichaje, si este no se ha podido guardar se indica en la variable de estado del objeto de retorno.</returns>
    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function SetTaskPunch(ByVal taskId As Integer, ByVal newTaskId As Integer, ByVal latitude As String, ByVal longitude As String, ByVal identifier As String, ByVal locationZone As String, ByVal fullAddress As String,
                                    ByVal oldValue0 As String, ByVal oldValue1 As String, ByVal oldValue2 As String, ByVal oldValue3 As String, ByVal oldValue4 As String, ByVal oldValue5 As String,
                                    ByVal newValue0 As String, ByVal newValue1 As String, ByVal newValue2 As String, ByVal newValue3 As String, ByVal newValue4 As String, ByVal newValue5 As String, ByVal completeTask As Boolean, ByVal reliable As Boolean, ByVal tcType As String) As UserStatus
        Dim lrret As New UserStatus()
        If Not Global_asax.LoggedIn Then
            lrret.Status = VTPortal.StatusHelper.GetStatusFromState(Global_asax.SecurityResult)
            If lrret.Status = ErrorCodes.OK Then lrret.Status = ErrorCodes.NO_SESSION
            Return lrret
        End If

        lrret.Status = ErrorCodes.OK

        Try
            Dim isApp As Boolean = HttpContext.Current.Request.Headers("roSrc") IsNot Nothing AndAlso roTypes.Any2Boolean(HttpContext.Current.Request.Headers("roSrc"))

            Dim oParamState As New AdvancedParameter.roAdvancedParameterState
            roBaseState.SetSessionSmall(oParamState, Global_asax.Identity.ID, Global_asax.ApplicationSource, "")
            Dim oParameter As New AdvancedParameter.roAdvancedParameter("VTLive.IPRestriction.OnlyPunches", oParamState)
            Dim proceedPunch As Boolean = True

            If roTypes.Any2Boolean(oParameter.Value) Then
                Dim strClientLocation As String = oParamState.ClientAddress.Split("#")(0)
                proceedPunch = AuthHelper.IsValidClientLocation(Global_asax.Identity.ID, strClientLocation)
            End If

            If proceedPunch Then
                Dim dLatitude As Double = roTypes.Any2Double(latitude.Replace(".", cCulture.NumberFormat.NumberDecimalSeparator), cCulture)
                Dim dLongitude As Double = roTypes.Any2Double(longitude.Replace(".", cCulture.NumberFormat.NumberDecimalSeparator), cCulture)

                Dim notReliableCause As String = Nothing
                If Not reliable Then
                    notReliableCause = DTOs.NotReliableCause.PunchWithoutLocation.ToString()
                End If

                Dim tmpResponse As StdResponse = VTPortal.TaskHelper.SavePunchTaskValues(Global_asax.Identity, Global_asax.Identity.IDEmployee, Global_asax.TimeZone,
                                                           taskId, newTaskId, dLatitude, dLongitude, identifier, locationZone, fullAddress, oldValue0, oldValue1, oldValue2,
                                                           oldValue3, oldValue4, oldValue5, newValue0, newValue1, newValue2, newValue3, newValue4, newValue5, completeTask,
                                                           Nothing, reliable, isApp, notReliableCause)

                If tmpResponse.Result Then
                    lrret = GetEmployeeStatus()
                    lrret.Status = tmpResponse.Status
                Else
                    lrret.Status = ErrorCodes.GENERAL_ERROR
                End If
            Else
                lrret.Status = ErrorCodes.LOGIN_INVALID_CLIENT_LOCATION

            End If
        Catch ex As Exception
            lrret.Status = ErrorCodes.GENERAL_ERROR
            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::PortalSvcx::SetTaskPunch")
        End Try

        Return lrret
    End Function

    ''' <summary>
    ''' Se requiere que la sesión este iniciada en el sistema. Esta función responde a un método POST del servidor.
    ''' Realiza un fichaje de tareas en el sistema para el empleado que ha iniciado la sesión añadiendo una fotografia del mismo.
    ''' Esta función recibe los mismos parametros que en la petición equivalente sin fotografia con un objeto imagen en la propiedad Request.Files
    ''' </summary>
    ''' <returns>Obtenemos un objeto del tipo UserEstatus, indicando el nuevo estado del empleado una vez realizado el fichaje, si este no se ha podido guardar se indica en la variable de estado del objeto de retorno.</returns>
    <OperationContract()>
    <WebInvoke(Method:="POST", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function SetTaskPunchWithPhoto() As UserStatus
        Dim lrret As New UserStatus()
        If Not Global_asax.LoggedIn Then
            lrret.Status = VTPortal.StatusHelper.GetStatusFromState(Global_asax.SecurityResult)
            If lrret.Status = ErrorCodes.OK Then lrret.Status = ErrorCodes.NO_SESSION
            Return lrret
        End If

        lrret.Status = ErrorCodes.OK

        Try
            Dim isApp As Boolean = HttpContext.Current.Request.Headers("roSrc") IsNot Nothing AndAlso roTypes.Any2Boolean(HttpContext.Current.Request.Headers("roSrc"))
            Dim oParamState As New AdvancedParameter.roAdvancedParameterState
            roBaseState.SetSessionSmall(oParamState, Global_asax.Identity.ID, Global_asax.ApplicationSource, "")
            Dim oParameter As New AdvancedParameter.roAdvancedParameter("VTLive.IPRestriction.OnlyPunches", oParamState)
            Dim proceedPunch As Boolean = True

            If roTypes.Any2Boolean(oParameter.Value) Then
                Dim strClientLocation As String = oParamState.ClientAddress.Split("#")(0)
                proceedPunch = AuthHelper.IsValidClientLocation(Global_asax.Identity.ID, strClientLocation)
            End If

            If proceedPunch Then
                Dim curFile As HttpPostedFile = HttpContext.Current.Request.Files("userfile")
                If curFile Is Nothing Then curFile = HttpContext.Current.Request.Files("file")

                Dim taskId As Integer = HttpContext.Current.Request.Params("taskId")
                Dim newTaskId As Integer = HttpContext.Current.Request.Params("newTaskId")
                Dim completeTask As Boolean = roTypes.Any2Boolean(HttpContext.Current.Request.Params("completeTask"))
                Dim latitudeStr As String = HttpContext.Current.Request.Params("latitude")
                Dim longitudeStr As String = HttpContext.Current.Request.Params("longitude")
                Dim identifier As String = HttpContext.Current.Request.Params("identifier")
                Dim locationZone As String = HttpContext.Current.Request.Params("locationZone")
                Dim fullAddress As String = HttpContext.Current.Request.Params("fullAddress")

                Dim latitude As Double = roTypes.Any2Double(latitudeStr.Replace(".", cCulture.NumberFormat.NumberDecimalSeparator), cCulture)
                Dim longitude As Double = roTypes.Any2Double(longitudeStr.Replace(".", cCulture.NumberFormat.NumberDecimalSeparator), cCulture)

                Dim oldValue0 As String = HttpContext.Current.Request.Params("oldValue0")
                Dim oldValue1 As String = HttpContext.Current.Request.Params("oldValue1")
                Dim oldValue2 As String = HttpContext.Current.Request.Params("oldValue2")
                Dim oldValue3 As String = HttpContext.Current.Request.Params("oldValue3")
                Dim oldValue4 As String = HttpContext.Current.Request.Params("oldValue4")
                Dim oldValue5 As String = HttpContext.Current.Request.Params("oldValue5")

                Dim newValue0 As String = HttpContext.Current.Request.Params("newValue0")
                Dim newValue1 As String = HttpContext.Current.Request.Params("newValue1")
                Dim newValue2 As String = HttpContext.Current.Request.Params("newValue2")
                Dim newValue3 As String = HttpContext.Current.Request.Params("newValue3")
                Dim newValue4 As String = HttpContext.Current.Request.Params("newValue4")
                Dim newValue5 As String = HttpContext.Current.Request.Params("newValue5")

                Dim reliable As Boolean = roTypes.Any2Boolean(HttpContext.Current.Request.Params("reliable"))

                Dim isTelecommute As Boolean = roTypes.Any2Boolean(HttpContext.Current.Request.Params("tcType"))

                Dim punchImage As Image = New Bitmap(curFile.InputStream)

                Dim notReliableCause As String = Nothing
                If Not reliable Then
                    notReliableCause = DTOs.NotReliableCause.PunchWithoutLocation.ToString()
                End If

                Dim tmpResponse As StdResponse = VTPortal.TaskHelper.SavePunchTaskValues(Global_asax.Identity, Global_asax.Identity.IDEmployee, Global_asax.TimeZone,
                                                  taskId, newTaskId, latitude, longitude, identifier, locationZone, fullAddress, oldValue0, oldValue1, oldValue2,
                                                  oldValue3, oldValue4, oldValue5, newValue0, newValue1, newValue2, newValue3, newValue4, newValue5, completeTask,
                                                  punchImage, reliable, isApp, notReliableCause)

                If tmpResponse.Result Then
                    lrret = GetEmployeeStatus()
                    lrret.Status = tmpResponse.Status
                Else
                    lrret.Status = ErrorCodes.GENERAL_ERROR
                End If
            Else
                lrret.Status = ErrorCodes.LOGIN_INVALID_CLIENT_LOCATION
            End If
        Catch ex As Exception
            lrret.Status = ErrorCodes.GENERAL_ERROR
            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::PortalSvcx::SetTaskPunchWithPhoto")
        End Try

        Return lrret
    End Function

    ''' <summary>
    ''' Se requiere que la sesión este iniciada en el sistema. Esta función responde a un método GET del servidor.
    ''' Consulta el calendario del empleado para el mes de la fecha seleccionada
    ''' </summary>
    ''' <param name="selectedDate">Cadena de texto que informa la fecha en formato 'dd/MM/yyyy'</param>
    ''' <returns>Obtenemos un objeto del tipo EmployeeCalendar. Si se ha producido un error en la consulta la variable de esato del objeto informa de ello.</returns>
    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function GetMyCalendar(ByVal selectedDate As String) As EmployeeCalendar
        Dim lrret As New EmployeeCalendar

        If Not Global_asax.LoggedIn Then
            lrret.Status = VTPortal.StatusHelper.GetStatusFromState(Global_asax.SecurityResult)
            If lrret.Status = ErrorCodes.OK Then lrret.Status = ErrorCodes.NO_SESSION
            Return lrret
        End If

        Try
            Dim sDate As DateTime = DateTime.ParseExact(selectedDate, "dd/MM/yyyy", Nothing)
            Dim oEmpState As New Robotics.Base.VTCalendar.roCalendarRowPeriodDataState(Global_asax.Identity.ID)
            roBaseState.SetSessionSmall(oEmpState, Global_asax.Identity.ID, Global_asax.ApplicationSource, "")

            lrret = VTPortal.ShiftsHelper.GetEmployeeCalendar(Global_asax.Identity, Global_asax.Identity.IDEmployee, sDate, oEmpState, False, False)
        Catch ex As Exception
            lrret.Status = ErrorCodes.GENERAL_ERROR
            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::PortalSvcx::GetMyCalendar")
        End Try

        Return lrret
    End Function

    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function GetCapacityDetail(ByVal selectedDate As String) As EmployeeList
        Dim lrret As New EmployeeList

        If Not Global_asax.LoggedIn Then
            lrret.Status = VTPortal.StatusHelper.GetStatusFromState(Global_asax.SecurityResult)
            If lrret.Status = ErrorCodes.OK Then lrret.Status = ErrorCodes.NO_SESSION
            Return lrret
        End If

        Try
            Dim fileName As String = Path.Combine(HostingEnvironment.ApplicationPhysicalPath, "Resources/userDefault.png")
            Dim fileStream As New FileStream(fileName, FileMode.Open, FileAccess.Read)

            Dim ImageData As Byte()
            ImageData = New Byte(fileStream.Length - 1) {}
            fileStream.Read(ImageData, 0, System.Convert.ToInt32(fileStream.Length))
            fileStream.Close()

            lrret.DefaultImage = "url(data:image/png;base64," & Convert.ToBase64String(ImageData) & ") no-repeat center center"

            Dim sDate As DateTime = DateTime.ParseExact(selectedDate, "dd/MM/yyyy", Nothing)
            Dim oEmpState As New VTCalendar.roCalendarRowPeriodDataState(Global_asax.Identity.ID)
            roBaseState.SetSessionSmall(oEmpState, Global_asax.Identity.ID, Global_asax.ApplicationSource, "")

            lrret = VTPortal.EmployeesHelper.GetEmployeesOnWorkCenter(Global_asax.Identity, Global_asax.Identity.IDEmployee, sDate, oEmpState)
        Catch ex As Exception
            lrret.Status = ErrorCodes.GENERAL_ERROR
            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::PortalSvcx::GetCapacityDetail")
        End Try

        Return lrret
    End Function

    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function GetLoadSeatingCapacity(ByVal selectedDate As String) As CurrentCapacity
        Dim lrret As New CurrentCapacity

        If Not Global_asax.LoggedIn Then
            lrret.Status = VTPortal.StatusHelper.GetStatusFromState(Global_asax.SecurityResult)
            If lrret.Status = ErrorCodes.OK Then lrret.Status = ErrorCodes.NO_SESSION
            Return lrret
        End If

        Try
            Dim sDate As DateTime = DateTime.ParseExact(selectedDate, "dd/MM/yyyy", Nothing)
            Dim oEmpState As New Robotics.Base.VTCalendar.roCalendarRowPeriodDataState(Global_asax.Identity.ID)
            roBaseState.SetSessionSmall(oEmpState, Global_asax.Identity.ID, Global_asax.ApplicationSource, "")

            lrret = VTPortal.ShiftsHelper.GetCurrentCapacityOnDate(Global_asax.Identity, Global_asax.Identity.IDEmployee, sDate, sDate, oEmpState)
        Catch ex As Exception
            lrret.Status = ErrorCodes.GENERAL_ERROR
            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::PortalSvcx::GetLoadSeatingCapacity")
        End Try

        Return lrret
    End Function

    ''' <summary>
    ''' Se requiere que la sesión este iniciada en el sistema. Esta función responde a un método GET del servidor.
    ''' Consulta el calendario de dias disponibles para solicitar permisos
    ''' </summary>
    ''' <param name="selectedDate">Cadena de texto que informa la fecha en formato 'dd/MM/yyyy'</param>
    ''' <param name="ProgrammedHoliday_IDCause">Identificador de la justificación que queremos tener en cuenta para permisos por días (-1, añade todos)</param>
    ''' <returns>Obtenemos un objeto del tipo EmployeeCalendar. Si se ha producido un error en la consulta la variable de esato del objeto informa de ello.</returns>
    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function GetAvailablePermitsCalendar(ByVal selectedDate As String, ByVal ProgrammedHoliday_IDCause As Integer) As EmployeeCalendar
        Dim lrret As New EmployeeCalendar

        If Not Global_asax.LoggedIn Then
            lrret.Status = VTPortal.StatusHelper.GetStatusFromState(Global_asax.SecurityResult)
            If lrret.Status = ErrorCodes.OK Then lrret.Status = ErrorCodes.NO_SESSION
            Return lrret
        End If

        Try
            Dim sDate As DateTime = DateTime.ParseExact(selectedDate, "dd/MM/yyyy", Nothing)
            Dim oEmpState As New Robotics.Base.VTCalendar.roCalendarRowPeriodDataState(Global_asax.Identity.ID)
            roBaseState.SetSessionSmall(oEmpState, Global_asax.Identity.ID, Global_asax.ApplicationSource, "")

            lrret = VTPortal.ShiftsHelper.GetEmployeePlan(Global_asax.Identity.ID, Global_asax.Identity.IDEmployee, sDate, oEmpState, ProgrammedHoliday_IDCause)
        Catch ex As Exception
            lrret.Status = ErrorCodes.GENERAL_ERROR
            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::PortalSvcx::GetAvailablePermitsCalendar")
        End Try

        Return lrret
    End Function

    ''' <summary>
    ''' Se requiere que la sesión este iniciada en el sistema. Esta función responde a un método GET del servidor.
    ''' Consulta el listado de saldos completo del empleado con la sesión iniciada
    ''' </summary>
    ''' <param name="selectedDate">Cadena de texto que informa la fecha en formato 'dd/MM/yyyy'</param>
    ''' <returns>Obtenemos un objeto del tipo EmployeeAccruals. Si se ha producido un error en la consulta la variable de esato del objeto informa de ello.</returns>
    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function GetMyAccruals(ByVal selectedDate As String) As EmployeeAccruals
        Dim lrret As New EmployeeAccruals

        If Not Global_asax.LoggedIn Then
            lrret.Status = VTPortal.StatusHelper.GetStatusFromState(Global_asax.SecurityResult)
            If lrret.Status = ErrorCodes.OK Then lrret.Status = ErrorCodes.NO_SESSION
            Return lrret
        End If

        Try

            Dim sDate As DateTime = DateTime.ParseExact(selectedDate, "dd/MM/yyyy", Nothing)
            Dim oEmpState As New Employee.roEmployeeState(Global_asax.Identity.ID)
            roBaseState.SetSessionSmall(oEmpState, Global_asax.Identity.ID, Global_asax.ApplicationSource, "")

            lrret = VTPortal.AccrualsHelper.GetEmployeeAccruals(Global_asax.Identity, Global_asax.Identity.IDEmployee, sDate, False, oEmpState)
        Catch ex As Exception
            lrret.Status = ErrorCodes.GENERAL_ERROR
            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::PortalSvcx::GetMyAccruals")
        End Try

        Return lrret
    End Function

    ''' <summary>
    ''' Se requiere que la sesión este iniciada en el sistema. Esta función responde a un método GET del servidor.
    ''' Consulta el listado de saldos completo del empleado con la sesión iniciada
    ''' </summary>
    ''' <param name="selectedDate">Cadena de texto que informa la fecha en formato 'dd/MM/yyyy'</param>
    ''' <returns>Obtenemos un objeto del tipo EmployeeAccruals. Si se ha producido un error en la consulta la variable de esato del objeto informa de ello.</returns>
    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function GetAccrualsSummary(ByVal selectedDate As String) As AccrualsSummary
        Dim lrret As New AccrualsSummary

        If Not Global_asax.LoggedIn Then
            lrret.Status = VTPortal.StatusHelper.GetStatusFromState(Global_asax.SecurityResult)
            If lrret.Status = ErrorCodes.OK Then lrret.Status = ErrorCodes.NO_SESSION
            Return lrret
        End If

        Try

            Dim sDate As DateTime = DateTime.ParseExact(selectedDate, "dd/MM/yyyy", Nothing)
            Dim oEmpState As New Employee.roEmployeeState(Global_asax.Identity.ID)
            roBaseState.SetSessionSmall(oEmpState, Global_asax.Identity.ID, Global_asax.ApplicationSource, "")

            lrret = VTPortal.AccrualsHelper.GetEmployeeAccrualsSummary(Global_asax.Identity, Global_asax.Identity.IDEmployee, sDate, oEmpState)
        Catch ex As Exception
            lrret.Status = ErrorCodes.GENERAL_ERROR
            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::PortalSvcx::GetMyAccruals")
        End Try

        Return lrret
    End Function

    ''' <summary>
    ''' Se requiere que la sesión este iniciada en el sistema. Esta función responde a un método GET del servidor.
    ''' Consulta el listado de saldos diarios completo del empleado con la sesión iniciada
    ''' </summary>
    ''' <param name="selectedDate">Cadena de texto que informa la fecha en formato 'dd/MM/yyyy'</param>
    ''' <returns>Obtenemos un objeto del tipo EmployeeAccruals. Si se ha producido un error en la consulta la variable de esato del objeto informa de ello.</returns>
    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function GetMyDailyAccruals(ByVal selectedDate As String) As EmployeeAccruals
        Dim lrret As New EmployeeAccruals

        If Not Global_asax.LoggedIn Then
            lrret.Status = VTPortal.StatusHelper.GetStatusFromState(Global_asax.SecurityResult)
            If lrret.Status = ErrorCodes.OK Then lrret.Status = ErrorCodes.NO_SESSION
            Return lrret
        End If

        Try

            Dim sDate As DateTime = DateTime.ParseExact(selectedDate, "dd/MM/yyyy", Nothing)
            Dim oEmpState As New Employee.roEmployeeState(Global_asax.Identity.ID)
            roBaseState.SetSessionSmall(oEmpState, Global_asax.Identity.ID, Global_asax.ApplicationSource, "")

            lrret = VTPortal.AccrualsHelper.GetEmployeeAccruals(Global_asax.Identity, Global_asax.Identity.IDEmployee, sDate, True, oEmpState)
        Catch ex As Exception
            lrret.Status = ErrorCodes.GENERAL_ERROR
            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::PortalSvcx::GetMyAccruals")
        End Try

        Return lrret
    End Function

    ''' <summary>
    ''' Se requiere que la sesión este iniciada en el sistema. Esta función responde a un método GET del servidor.
    ''' Consulta el listado de saldos fichajes del empleado con la sesión iniciada
    ''' </summary>
    ''' <param name="selectedDate">Cadena de texto que informa la fecha en formato 'dd/MM/yyyy'</param>
    ''' <returns>Obtenemos un objeto del tipo EmployeePunches. Si se ha producido un error en la consulta la variable de esato del objeto informa de ello.</returns>
    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function GetMyPunches(ByVal selectedDate As String) As EmployeePunches
        Dim lrret As New EmployeePunches

        If Not Global_asax.LoggedIn Then
            lrret.Status = VTPortal.StatusHelper.GetStatusFromState(Global_asax.SecurityResult)
            If lrret.Status = ErrorCodes.OK Then lrret.Status = ErrorCodes.NO_SESSION
            Return lrret
        End If

        Try
            Dim sDate As DateTime = DateTime.ParseExact(selectedDate, "dd/MM/yyyy", Nothing)
            Dim oPunchState As New Punch.roPunchState(Global_asax.Identity.ID)
            roBaseState.SetSessionSmall(oPunchState, Global_asax.Identity.ID, Global_asax.ApplicationSource, "")

            lrret = VTPortal.PunchHelper.GetEmployeePunches(Global_asax.Identity, Global_asax.Identity.IDEmployee, sDate, oPunchState)
        Catch ex As Exception
            lrret.Status = ErrorCodes.GENERAL_ERROR
            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::PortalSvcx::GetMyPunches")
        End Try

        Return lrret
    End Function

    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function GetPunchesOnDate(ByVal idEmployee As Integer, ByVal selectedDate As String) As EmployeePunches
        Dim lrret As New EmployeePunches

        If Not Global_asax.LoggedIn Then
            lrret.Status = VTPortal.StatusHelper.GetStatusFromState(Global_asax.SecurityResult)
            If lrret.Status = ErrorCodes.OK Then lrret.Status = ErrorCodes.NO_SESSION
            Return lrret
        End If

        Try
            Dim sDate As DateTime = DateTime.ParseExact(selectedDate, "dd/MM/yyyy", Nothing)
            Dim oPunchState As New Punch.roPunchState(Global_asax.Identity.ID)
            roBaseState.SetSessionSmall(oPunchState, Global_asax.Identity.ID, Global_asax.ApplicationSource, "")

            lrret = VTPortal.PunchHelper.GetEmployeePunches(Global_asax.Identity, idEmployee, sDate, oPunchState)
        Catch ex As Exception
            lrret.Status = ErrorCodes.GENERAL_ERROR
            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::PortalSvcx::GetPunchesOnDate")
        End Try

        Return lrret
    End Function

    ''' <summary>
    ''' Se requiere que la sesión este iniciada en el sistema. Esta función responde a un método GET del servidor.
    ''' Consulta el estado de las vacaciones del empleado con la sesión iniciada
    ''' </summary>
    ''' <returns>Obtenemos un objeto del tipo HolidaysInfo. Si se ha producido un error en la consulta la variable de esato del objeto informa de ello.</returns>
    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function GetHolidaysInfo() As HolidaysInfo
        Dim lrret As New HolidaysInfo

        If Not Global_asax.LoggedIn Then
            lrret.Status = VTPortal.StatusHelper.GetStatusFromState(Global_asax.SecurityResult)
            If lrret.Status = ErrorCodes.OK Then lrret.Status = ErrorCodes.NO_SESSION
            Return lrret
        End If

        Try

            Dim oShiftState As New Shift.roShiftState(Global_asax.Identity.ID)
            roBaseState.SetSessionSmall(oShiftState, Global_asax.Identity.ID, Global_asax.ApplicationSource, "")

            lrret = VTPortal.ShiftsHelper.GetEmployeeHolidaysInfo(Global_asax.Identity, Global_asax.Identity.IDEmployee, oShiftState)
        Catch ex As Exception
            lrret.Status = ErrorCodes.GENERAL_ERROR
            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::PortalSvcx::GetHolidaysInfo")
        End Try

        Return lrret
    End Function

    ''' <summary>
    ''' Se requiere que la sesión este iniciada en el sistema. Esta función responde a un método GET del servidor.
    ''' Consulta el listado de la ficha del empleado con la sesión iniciada. El listado contiene valores, tipos y toda la información de historicos sobre los campos de la ficha
    ''' </summary>
    ''' <returns>Obtenemos un objeto del tipo UserFields. Si se ha producido un error en la consulta la variable de esato del objeto informa de ello.</returns>
    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function GetUserFields() As UserFields
        Dim lrret As New UserFields

        If Not Global_asax.LoggedIn Then
            lrret.Status = VTPortal.StatusHelper.GetStatusFromState(Global_asax.SecurityResult)
            If lrret.Status = ErrorCodes.OK Then lrret.Status = ErrorCodes.NO_SESSION
            Return lrret
        End If

        Try
            Dim oEmpState As New Employee.roEmployeeState(Global_asax.Identity.ID)
            roBaseState.SetSessionSmall(oEmpState, Global_asax.Identity.ID, Global_asax.ApplicationSource, "")

            If Global_asax.Supervisor IsNot Nothing Then
                lrret = VTPortal.UserFieldHelper.GetEmployeeUserFieldsAsSupervisor(Global_asax.Identity, Global_asax.Supervisor, oEmpState)
            Else
                lrret = VTPortal.UserFieldHelper.GetEmployeeUserFields(Global_asax.Identity, oEmpState)
            End If
        Catch ex As Exception
            lrret.Status = ErrorCodes.GENERAL_ERROR
            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::PortalSvcx::GetUserFields")
        End Try

        Return lrret
    End Function

    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function GetZones() As Zones

        Dim lrret As New Zones()

        If Not Global_asax.LoggedIn Then
            lrret.Status = VTPortal.StatusHelper.GetStatusFromState(Global_asax.SecurityResult)
            If lrret.Status = ErrorCodes.OK Then lrret.Status = ErrorCodes.NO_SESSION
            Return lrret
        End If

        Try
            Dim oEmpState As New roZoneState(Global_asax.Identity.ID)
            roBaseState.SetSessionSmall(oEmpState, Global_asax.Identity.ID, Global_asax.ApplicationSource, "")

            lrret = VTPortal.ZonesHelper.GetListZones(oEmpState, Global_asax.Identity)
        Catch ex As Exception
            lrret.Status = ErrorCodes.GENERAL_ERROR
            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::PortalSvcx::GetZones")
        End Try
        Return lrret
    End Function

    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function GetTelecommutingInfo(ByVal selectedDate As String, ByVal idEmployee As String) As TelecommutingInfo
        Dim lrret As New TelecommutingInfo

        If Not Global_asax.LoggedIn Then
            lrret.Status = VTPortal.StatusHelper.GetStatusFromState(Global_asax.SecurityResult)
            If lrret.Status = ErrorCodes.OK Then lrret.Status = ErrorCodes.NO_SESSION
            Return lrret
        End If

        Try
            Dim IDEmpleado = Global_asax.Identity.IDEmployee

            If idEmployee IsNot Nothing AndAlso idEmployee IsNot "" Then
                IDEmpleado = roTypes.Any2Integer(idEmployee)
            End If

            Dim sDate As DateTime = DateTime.ParseExact(selectedDate, "dd/MM/yyyy", Nothing)
            Dim oEmpState As New Robotics.Base.VTEmployees.Employee.roEmployeeState(Global_asax.Identity.ID)
            Dim oCalendarState As New Robotics.Base.VTCalendar.roCalendarRowPeriodDataState(Global_asax.Identity.ID)
            roBaseState.SetSessionSmall(oEmpState, Global_asax.Identity.ID, Global_asax.ApplicationSource, "")

            'Dim capacitiesToday = VTPortal.ShiftsHelper.GetCurrentCapacityOnDate(Global_asax.Identity, Global_asax.Identity.IDEmployee, Date.Now, Date.Now.AddDays(1), oEmpState)

            Dim currentAgreementUsage = VTEmployees.Employee.roEmployeeTelecommuteAgreement.GetTelecommuteStatsAtDate(IDEmpleado, sDate, Nothing, oEmpState)

            If currentAgreementUsage IsNot Nothing Then
                If currentAgreementUsage.EmployeeTelecommuteAgreement.Agreement.MaxType = TelecommutingMaxType._Percentage Then
                    lrret.Period = currentAgreementUsage.EmployeeTelecommuteAgreement.Agreement.PeriodType

                    '!Double.IsNaN(value) && !Double.IsInfinity(value);
                    If Not Double.IsNaN(currentAgreementUsage.CurrentPercentage) AndAlso Not Double.IsInfinity(currentAgreementUsage.CurrentPercentage) Then
                        lrret.CurrentAgreementPercentageUsed = Conversion.Int(currentAgreementUsage.CurrentPercentage)
                    Else
                        lrret.CurrentAgreementPercentageUsed = 0
                    End If

                    lrret.MaxPercentage = currentAgreementUsage.EmployeeTelecommuteAgreement.Agreement.MaxPercentage
                    lrret.ByPercentage = True
                Else
                    lrret.Period = currentAgreementUsage.EmployeeTelecommuteAgreement.Agreement.PeriodType
                    lrret.MaxDays = currentAgreementUsage.EmployeeTelecommuteAgreement.Agreement.MaxDays
                    lrret.CurrentAgreementDaysUsed = Conversion.Int(currentAgreementUsage.TelecommutePlannedDays)
                    lrret.ByPercentage = False
                End If
                lrret.TotalWorkingPlannedHours = currentAgreementUsage.TotalWorkingPlannedHours
                lrret.TelecommutePlannedHours = currentAgreementUsage.TelecommutePlannedHours
                lrret.PeriodStart = currentAgreementUsage.PeriodStart
                lrret.PeriodEnd = currentAgreementUsage.PeriodEnd
            End If

            'currentAgreementUsage.EmployeeTelecommuteAgreement.Agreement.MaxType
            Dim calendar = VTPortal.ShiftsHelper.GetEmployeeCalendar(Global_asax.Identity, IDEmpleado, Date.Now.Date, oCalendarState, True, True)

            If calendar.oCalendar IsNot Nothing AndAlso calendar.oCalendar.DayData IsNot Nothing AndAlso calendar.oCalendar.DayData.Length > 0 AndAlso calendar.oCalendar.DayData(0) IsNot Nothing Then
                If calendar.oCalendar.DayData(0).MainShift IsNot Nothing AndAlso roTypes.Any2Integer(calendar.oCalendar.DayData(0).MainShift.PlannedHours) > 0 Then
                    lrret.TCToday = roTypes.Any2String(calendar.oCalendar.DayData(0).TelecommutingExpected)
                    lrret.WorkcenterNameToday = roTypes.Any2String(calendar.oCalendar.DayData(0).ZoneName)
                    If lrret.WorkcenterNameToday = "?" OrElse lrret.WorkcenterNameToday.ToLower = "sin especificar" Then lrret.WorkcenterNameToday = ""
                Else
                    lrret.TCToday = False
                    lrret.WorkcenterNameToday = ""
                    lrret.NoWorkToday = True
                End If
            Else
                lrret.TCToday = False
                lrret.WorkcenterNameToday = ""
                lrret.NoWorkToday = True
            End If

            If calendar.oCalendar IsNot Nothing AndAlso calendar.oCalendar.DayData IsNot Nothing AndAlso calendar.oCalendar.DayData.Length > 1 AndAlso calendar.oCalendar.DayData(1) IsNot Nothing Then
                If calendar.oCalendar.DayData(1).MainShift IsNot Nothing AndAlso roTypes.Any2Integer(calendar.oCalendar.DayData(1).MainShift.PlannedHours) > 0 Then
                    lrret.TCTomorrow = roTypes.Any2String(calendar.oCalendar.DayData(1).TelecommutingExpected)
                    lrret.WorkcenterNameTomorrow = roTypes.Any2String(calendar.oCalendar.DayData(1).ZoneName)
                    If lrret.WorkcenterNameTomorrow = "?" OrElse lrret.WorkcenterNameTomorrow.ToLower = "sin especificar" Then lrret.WorkcenterNameTomorrow = ""
                Else
                    lrret.TCTomorrow = False
                    lrret.WorkcenterNameTomorrow = ""
                    lrret.NoWorkTomorrow = True
                End If
            Else
                lrret.TCTomorrow = False
                lrret.WorkcenterNameTomorrow = ""
                lrret.NoWorkToday = True
            End If

            'lrret.WorkcenterStatus = roTypes.Any2String(capacitiesToday.capacities.CurrentSeating) + "/" + roTypes.Any2String(capacitiesToday.capacities.MaxSeatingCapacity)
        Catch ex As Exception
            lrret.Status = ErrorCodes.GENERAL_ERROR
            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::PortalSvcx::GetTelecommutingInfo")
        End Try

        Return lrret
    End Function

    ''' <summary>
    ''' Se requiere que la sesión este iniciada en el sistema. Esta función responde a un método GET del servidor.
    ''' Consulta el listado de solicitudes completa del empleado que tinee la sesión iniciada.
    ''' </summary>
    ''' <param name="showAll">Indica si se deben mostrar todas las solicitudes. Valores aceptados: True/False</param>
    ''' <param name="dateStart">Cadena de texto que informa la fecha incial del filtro de solicitudes en formato 'dd/MM/yyyy'</param>
    ''' <param name="dateEnd">Cadena de texto que informa la fecha final del filtro de solicitudes en formato 'dd/MM/yyyy'</param>
    ''' <param name="filter">Cadena de texto que informa el filtro de solicitudes en el siguiente formato: 0*1*2*3|1*2*3*4*5*6*7*8*9*10*11*12
    '''     0 --> Solicitudes Pendientes
    '''     1 --> Solicitudes en curso
    '''     2 --> Solicitudes aceptadas
    '''     3 --> Solicitudes denegadas
    '''     |
    '''     1 --> Solicitud de campo de la ficha
    '''     2 --> Solicitud de fichaje no realizado
    '''     3 --> Solicitud de justificación de fichaje
    '''     4 --> Solicitud de trabajo externo
    '''     5 --> Solicitud de cambio de horario
    '''     6 --> Solicitud de vacaciones
    '''     7 --> Solicitud de ausencia por dias
    '''     8 --> ---
    '''     9 --> Solicitud de ausencia por horas
    '''     10 --> solicitud de fichaje de tareas no realizado
    '''     11 --> Solicitud de cancelación de vacaciones
    '''     12 --> Solcitud de fichaje de centro de costo no realizado
    '''     13 --> Solcitud de Vacaciones por horas
    '''     14 --> Solcitud de Horas de Exceso
    '''     15 --> Solicitud de trabajo externo
    '''     16 --> Solicitud de teletrabajo
    '''     17 --> Solicitud de Declaración de jornada
    ''' </param>
    ''' <param name="orderBy">Ordenación de las solicitudes con el siguiente formato: 'campo sentido'
    '''     campo:  RequestType: 'Tipo de solicitud'
    '''             RequestDate: 'Fecha de creación de la solicitud'
    '''             Status: 'Estado de la solicitud'
    '''             Date1: 'Fecha solicitada'
    '''     sentido: ASC: Ascendente
    '''              DESC: Descendente
    ''' </param>
    ''' <returns>Obtenemos un objeto del tipo RequestsList. Si se ha producido un error en la consulta la variable de esato del objeto informa de ello.</returns>
    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function GetMyRequests(ByVal showAll As Boolean, ByVal dateStart As String, ByVal dateEnd As String, ByVal filter As String, ByVal orderBy As String, ByVal dateRequestedStart As String, ByVal dateRequestedEnd As String) As RequestsList
        Dim lrret As New RequestsList

        If Not Global_asax.LoggedIn Then
            lrret.Status = VTPortal.StatusHelper.GetStatusFromState(Global_asax.SecurityResult)
            If lrret.Status = ErrorCodes.OK Then lrret.Status = ErrorCodes.NO_SESSION
            Return lrret
        End If

        Try
            Dim langKey As String = String.Empty
            If Global_asax.Supervisor IsNot Nothing Then
                langKey = Global_asax.Supervisor.Language.Key
            Else
                langKey = Global_asax.Identity.Language.Key
            End If

            Dim oLang As New roLanguage()
            oLang.SetLanguageReference("LiveOne", langKey)
            Dim oReqLang As New roLanguage()
            oReqLang.SetLanguageReference("RequestService", langKey)
            Dim oPortalLang As New roLanguage()
            oPortalLang.SetLanguageReference("VTPortal", langKey)

            Dim oReqState As Requests.roRequestState = New Requests.roRequestState(Global_asax.Identity.ID)
            roBaseState.SetSessionSmall(oReqState, Global_asax.Identity.ID, Global_asax.ApplicationSource, "")

            Dim strOrderBy As String = orderBy

            Dim sDate As Date
            If dateStart <> "" Then
                Dim strDate As String
                Dim strTime As String = "00:00"
                Dim strArrDate() As String = dateStart.Split(" ")
                strDate = strArrDate(0)
                If strArrDate.Length = 2 Then strTime = dateStart.Split(" ")(1)
                sDate = New Date(strDate.Split("-")(0), strDate.Split("-")(1), strDate.Split("-")(2), strTime.Split(":")(0), strTime.Split(":")(1), 0)
            End If

            Dim eDate As Date
            If dateEnd <> "" Then
                Dim strDate As String
                Dim strTime As String = "00:00"
                Dim strArrDate() As String = dateEnd.Split(" ")
                strDate = strArrDate(0)
                If strArrDate.Length = 2 Then strTime = dateEnd.Split(" ")(1)
                eDate = New Date(strDate.Split("-")(0), strDate.Split("-")(1), strDate.Split("-")(2), strTime.Split(":")(0), strTime.Split(":")(1), 0)
            End If

            Dim sRequestedDate As Date
            If dateRequestedStart <> "" Then
                Dim strDate As String
                Dim strTime As String = "00:00"
                Dim strArrDate() As String = dateRequestedStart.Split(" ")
                strDate = strArrDate(0)
                If strArrDate.Length = 2 Then strTime = dateRequestedStart.Split(" ")(1)
                sRequestedDate = New Date(strDate.Split("-")(0), strDate.Split("-")(1), strDate.Split("-")(2), strTime.Split(":")(0), strTime.Split(":")(1), 0)
            End If

            Dim eRequestedDate As Date
            If dateRequestedEnd <> "" Then
                Dim strDate As String
                Dim strTime As String = "00:00"
                Dim strArrDate() As String = dateRequestedEnd.Split(" ")
                strDate = strArrDate(0)
                If strArrDate.Length = 2 Then strTime = dateRequestedEnd.Split(" ")(1)
                eRequestedDate = New Date(strDate.Split("-")(0), strDate.Split("-")(1), strDate.Split("-")(2), strTime.Split(":")(0), strTime.Split(":")(1), 0)
            End If

            lrret = VTPortal.RequestsHelper.GetEmployeeRequests(showAll, sDate, eDate, sRequestedDate, eRequestedDate, filter, orderBy, Global_asax.Identity.ID, Global_asax.Identity.IDEmployee, oLang, oReqLang, oPortalLang, oReqState)
        Catch ex As Exception
            lrret.Status = ErrorCodes.GENERAL_ERROR
            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::PortalSvcx::GetRequests")
        End Try

        Return lrret
    End Function

    ''' <summary>
    ''' Se requiere que la sesión este iniciada en el sistema. Esta función responde a un método GET del servidor.
    ''' Consulta el trabajo externo realizado por un empleado en un día concreto.
    ''' </summary>
    ''' <returns>Obtenemos un objeto del tipo RequestsList. Si se ha producido un error en la consulta la variable de esato del objeto informa de ello.</returns>
    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function GetTeleworkingDetail(ByVal iDate As String, ByVal eDate As String) As UserRequestsList
        Dim lrret As New UserRequestsList

        If Not Global_asax.LoggedIn Then
            lrret.Status = VTPortal.StatusHelper.GetStatusFromState(Global_asax.SecurityResult)
            If lrret.Status = ErrorCodes.OK Then lrret.Status = ErrorCodes.NO_SESSION
            Return lrret
        End If

        Try
            Dim initialDate As DateTime = DateTime.ParseExact(iDate, "yyyyMMdd", Nothing)
            Dim endDate As DateTime = DateTime.ParseExact(eDate, "yyyyMMdd", Nothing)

            Dim langKey As String = String.Empty
            If Global_asax.Supervisor IsNot Nothing Then
                langKey = Global_asax.Supervisor.Language.Key
            Else
                langKey = Global_asax.Identity.Language.Key
            End If

            Dim oLang As New roLanguage()
            oLang.SetLanguageReference("LiveOne", langKey)
            Dim oReqLang As New roLanguage()
            oReqLang.SetLanguageReference("RequestService", langKey)
            Dim oPortalLang As New roLanguage()
            oPortalLang.SetLanguageReference("VTPortal", langKey)

            Dim oReqState As Requests.roRequestState = New Requests.roRequestState(Global_asax.Identity.ID)
            roBaseState.SetSessionSmall(oReqState, Global_asax.Identity.ID, Global_asax.ApplicationSource, "")

            Dim oTmpReq As RequestsList = VTPortal.RequestsHelper.GetEmployeeRequests(False, initialDate, endDate, Nothing, Nothing, "0*1*2|4*15", "Date1 ASC", Global_asax.Identity.ID, Global_asax.Identity.IDEmployee, oLang, oReqLang, oPortalLang, oReqState)

            Dim oNewTmpLst As New Generic.List(Of UserRequest)
            For Each oRequest As EmpRequest In oTmpReq.Requests
                oNewTmpLst.Add(VTPortal.RequestsHelper.GetRequestByEmployee(oRequest.Id, Global_asax.Identity.IDEmployee, oLang, oReqState))
            Next

            lrret.CanCreateRequest = oTmpReq.CanCreateRequest
            lrret.Status = oTmpReq.Status
            lrret.Requests = oNewTmpLst.ToArray
        Catch ex As Exception
            lrret.Status = ErrorCodes.GENERAL_ERROR
            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::PortalSvcx::GetTeleworkingDetail")
        Finally
        End Try

        Return lrret
    End Function

    ''' <summary>
    ''' Se requiere que la sesión este iniciada en el sistema. Esta función responde a un método GET del servidor.
    ''' Consulta el trabajo externo realizado por un empleado en un día concreto.
    ''' </summary>
    ''' <returns>Obtenemos un objeto del tipo RequestsList. Si se ha producido un error en la consulta la variable de esato del objeto informa de ello.</returns>
    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function GetMyDailyTeleWorking(ByVal selectedDate As String) As RequestsList
        Dim lrret As New RequestsList

        If Not Global_asax.LoggedIn Then
            lrret.Status = VTPortal.StatusHelper.GetStatusFromState(Global_asax.SecurityResult)
            If lrret.Status = ErrorCodes.OK Then lrret.Status = ErrorCodes.NO_SESSION
            Return lrret
        End If

        Try
            Dim sDate As DateTime = DateTime.ParseExact(selectedDate, "yyyyMMdd", Nothing)

            Dim langKey As String = String.Empty
            If Global_asax.Supervisor IsNot Nothing Then
                langKey = Global_asax.Supervisor.Language.Key
            Else
                langKey = Global_asax.Identity.Language.Key
            End If

            Dim oLang As New roLanguage()
            oLang.SetLanguageReference("LiveOne", langKey)
            Dim oReqLang As New roLanguage()
            oReqLang.SetLanguageReference("RequestService", langKey)
            Dim oPortalLang As New roLanguage()
            oPortalLang.SetLanguageReference("VTPortal", langKey)

            Dim oReqState As Requests.roRequestState = New Requests.roRequestState(Global_asax.Identity.ID)
            roBaseState.SetSessionSmall(oReqState, Global_asax.Identity.ID, Global_asax.ApplicationSource, "")

            lrret = VTPortal.RequestsHelper.GetEmployeeTeleWorkingRequestsOnDay(sDate, Global_asax.Identity.ID, Global_asax.Identity.IDEmployee, oLang, oReqLang, oPortalLang, oReqState)
        Catch ex As Exception
            lrret.Status = ErrorCodes.GENERAL_ERROR
            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::PortalSvcx::GetMyDailyTeleWorking")
        Finally
        End Try

        Return lrret
    End Function

    ''' <summary>
    ''' Se requiere que la sesión este iniciada en el sistema. Esta función responde a un método GET del servidor.
    ''' Consulta los permisos de los que dispone el empleado que ha iniciado sesión en la aplicación
    ''' </summary>
    ''' <returns>Obtenemos un objeto del tipo PermissionList. Si se ha producido un error en la consulta la variable de esato del objeto informa de ello.</returns>
    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function GetMyPermissions() As PermissionList
        Dim lrret As New PermissionList

        If Not Global_asax.LoggedIn Then
            lrret.Status = VTPortal.StatusHelper.GetStatusFromState(Global_asax.SecurityResult)
            If lrret.Status = ErrorCodes.OK Then lrret.Status = ErrorCodes.NO_SESSION
            Return lrret
        End If

        Try
            Dim oState As New Requests.roRequestState(Global_asax.Identity.ID)
            lrret = VTPortal.SecurityHelper.GetEmployeePermissions(Global_asax.Identity, Global_asax.TerminalIdentity, oState)
        Catch ex As Exception
            lrret.Status = ErrorCodes.GENERAL_ERROR
            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::PortalSvcx::GetMyPermissions")
        Finally
        End Try

        Return lrret
    End Function

    ''' <summary>
    ''' Se requiere que la sesión este iniciada en el sistema. Esta función responde a un método GET del servidor.
    ''' Marca como leída una alerta sobre una solicitud de cambio de estado.
    ''' </summary>
    ''' <param name="requestId">Identificador de la solicitud</param>
    ''' <returns>Obtenemos un objeto estandar de respuesta de petición indicando si se ha podido marcar como leida la solicitud y en caso contrario el código de error obtenido.</returns>
    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function SetRequestReaded(ByVal requestId As Integer) As StdResponse
        Dim lrret As New StdResponse

        If Not Global_asax.LoggedIn Then
            lrret.Status = VTPortal.StatusHelper.GetStatusFromState(Global_asax.SecurityResult)
            If lrret.Status = ErrorCodes.OK Then lrret.Status = ErrorCodes.NO_SESSION
            Return lrret
        End If

        Try
            Dim oReqState As New Requests.roRequestState
            roBaseState.SetSessionSmall(oReqState, Global_asax.Identity.IDEmployee, Global_asax.ApplicationSource, "")

            lrret = VTPortal.RequestsHelper.SetRequestAsReaded(requestId, Global_asax.Identity.IDEmployee, oReqState)
        Catch ex As Exception
            lrret.Status = ErrorCodes.GENERAL_ERROR
            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::PortalSvcx::SetRequestReaded")
        End Try

        Return lrret
    End Function

    ''' <summary>
    ''' Se requiere que la sesión este iniciada en el sistema. Esta función responde a un método GET del servidor.
    ''' Marca como leída una alerta sobre una notificación de visualtime.
    ''' </summary>
    ''' <param name="notificationtId">Identificador de la notificación</param>
    ''' <returns>Obtenemos un objeto estandar de respuesta de petición indicando si se ha podido marcar como leida la alerta y en caso contrario el código de error obtenido.</returns>
    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function SetNotificationReaded(ByVal notificationtId As Integer) As StdResponse
        Dim lrret As New StdResponse

        If Not Global_asax.LoggedIn Then
            lrret.Status = VTPortal.StatusHelper.GetStatusFromState(Global_asax.SecurityResult)
            If lrret.Status = ErrorCodes.OK Then lrret.Status = ErrorCodes.NO_SESSION
            Return lrret
        End If

        Try
            Dim oReqState As New Requests.roRequestState
            roBaseState.SetSessionSmall(oReqState, Global_asax.Identity.IDEmployee, Global_asax.ApplicationSource, "")

            lrret = VTPortal.RequestsHelper.SetNotificationReaded(notificationtId, Global_asax.Identity.IDEmployee, oReqState)
        Catch ex As Exception
            lrret.Status = ErrorCodes.GENERAL_ERROR
            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::PortalSvcx::SetRequestReaded")
        End Try

        Return lrret
    End Function

    ''' <summary>
    ''' Se requiere que la sesión este iniciada en el sistema. Esta función responde a un método GET del servidor.
    ''' Obtiene todos los detalles completos de una solicitud
    ''' </summary>
    ''' <param name="requestId">Identificador de la solicitud</param>
    ''' <returns>Obtenemos un objeto del tipo UserRequest. Si se ha producido un error en la consulta la variable de esato del objeto informa de ello.</returns>
    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function GetRequest(ByVal requestId As Integer) As UserRequest
        Dim lrret As New UserRequest

        If Not Global_asax.LoggedIn Then
            lrret.Status = VTPortal.StatusHelper.GetStatusFromState(Global_asax.SecurityResult)
            If lrret.Status = ErrorCodes.OK Then lrret.Status = ErrorCodes.NO_SESSION
            Return lrret
        End If

        Try

            Dim oReqState As New Requests.roRequestState
            roBaseState.SetSessionSmall(oReqState, Global_asax.Identity.ID, Global_asax.ApplicationSource, "")

            Dim oLng As New roLanguage()
            oLng.SetLanguageReference("LiveOne", Global_asax.Identity.Language.Key)

            lrret = VTPortal.RequestsHelper.GetRequestByEmployee(requestId, Global_asax.Identity.IDEmployee, oLng, oReqState)
        Catch ex As Exception
            lrret.Status = ErrorCodes.GENERAL_ERROR
            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::PortalSvcx::GetRequest")
        End Try

        Return lrret
    End Function

    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function GetRequestForSupervisor(ByVal requestId As Integer) As UserRequestForSupervisor
        Dim lrret As New UserRequestForSupervisor

        If Not Global_asax.LoggedIn Then
            lrret.Status = VTPortal.StatusHelper.GetStatusFromState(Global_asax.SecurityResult)
            If lrret.Status = ErrorCodes.OK Then lrret.Status = ErrorCodes.NO_SESSION
            Return lrret
        End If

        Try

            Dim oReqState As New Requests.roRequestState
            roBaseState.SetSessionSmall(oReqState, Global_asax.Identity.ID, Global_asax.ApplicationSource, "")

            Dim oLng As New roLanguage()
            oLng.SetLanguageReference("LiveOne", Global_asax.Identity.Language.Key)

            lrret = VTPortal.RequestsHelper.GetRequestForSupervisor(requestId, Global_asax.Identity.ID, oLng, oReqState)

            If lrret.IdEmployee <> Nothing AndAlso lrret.IdEmployee > 0 Then
                Dim oEmployee = Employee.roEmployee.GetEmployee(lrret.IdEmployee, New Employee.roEmployeeState)

                lrret.EmployeeName = oEmployee.Name
                lrret.EmployeeGroup = oEmployee.GroupName
                Dim Image = GetWsEmployeePhoto(lrret.IdEmployee)
                lrret.EmployeeImage = Image.Base64StringContent

                If (lrret.RequestType = 6) Then

                    'Dim sDate As DateTime = DateTime.ParseExact(Date.Now.AddDays(-1), "dd/MM/yyyy", Nothing)
                    Dim oEmpState As New Employee.roEmployeeState(Global_asax.Identity.ID)
                    roBaseState.SetSessionSmall(oEmpState, Global_asax.Identity.ID, Global_asax.ApplicationSource, "")

                    Dim available As Double = 0
                    Dim pending As Double = 0
                    Dim done As Double = 0
                    Dim lasting As Double = 0
                    Dim beginDate = lrret.Date1
                    Dim beginPeriod = Date.Now
                    Dim endPeriod = Date.Now
                    Dim expired As Double = 0
                    Dim WithoutEnjoyment As Double = 0

                    Dim bRet = Common.roBusinessSupport.VacationsResumeQuery(lrret.IdEmployee, lrret.IdShift, beginDate, beginPeriod, endPeriod, beginDate, done, pending, lasting, available, oReqState, expired, WithoutEnjoyment)

                    If bRet Then

                        Dim holidaysInfo As New HolidayAccrualsResume
                        holidaysInfo.Done = done
                        holidaysInfo.Pending = pending
                        holidaysInfo.Available = available
                        holidaysInfo.Lasting = lasting
                        holidaysInfo.Expired = expired
                        holidaysInfo.WithoutEnjoyment = WithoutEnjoyment
                        lrret.EmployeeAccruals = holidaysInfo
                    End If
                End If

                If (lrret.RequestType = 5) Then
                    Dim oShiftState As New Shift.roShiftState
                    roBaseState.SetSessionSmall(oShiftState, Global_asax.Identity.ID, Global_asax.ApplicationSource, "")

                    Dim oldShift = VTPortal.ShiftsHelper.GetShiftById(lrret.Field4, oShiftState)
                    Dim newShift = VTPortal.ShiftsHelper.GetShiftById(lrret.IdShift, oShiftState)
                    lrret.OldShiftName = oldShift.Name
                    lrret.OldShiftColor = System.Drawing.ColorTranslator.ToHtml(System.Drawing.ColorTranslator.FromWin32(roTypes.Any2Integer(oldShift.Color)))
                    lrret.NewShiftName = newShift.Name
                    lrret.NewShiftColor = System.Drawing.ColorTranslator.ToHtml(System.Drawing.ColorTranslator.FromWin32(roTypes.Any2Integer(newShift.Color)))
                End If
            End If
        Catch ex As Exception
            lrret.Status = ErrorCodes.GENERAL_ERROR
            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::PortalSvcx::GetRequestBySupervisor")
        End Try

        Return lrret
    End Function

    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function GetDailyRecordRequestForSupervisor(ByVal requestId As Integer) As (DailyRecord As roDailyRecord, DailyRecordRequest As UserRequestForSupervisor)
        Dim lrret As New UserRequestForSupervisor
        Dim oDailyRecord As roDailyRecord = Nothing

        If Not Global_asax.LoggedIn Then
            lrret.Status = VTPortal.StatusHelper.GetStatusFromState(Global_asax.SecurityResult)
            If lrret.Status = ErrorCodes.OK Then lrret.Status = ErrorCodes.NO_SESSION
            Return (DailyRecord:=oDailyRecord, DailyRecordRequest:=lrret)
        End If

        Try

            Dim oReqState As New Requests.roRequestState
            roBaseState.SetSessionSmall(oReqState, Global_asax.Identity.ID, Global_asax.ApplicationSource, "")

            Dim oLng As New roLanguage()
            oLng.SetLanguageReference("LiveOne", Global_asax.Identity.Language.Key)

            lrret = VTPortal.RequestsHelper.GetRequestForSupervisor(requestId, Global_asax.Identity.ID, oLng, oReqState)

            If lrret.IdEmployee <> Nothing AndAlso lrret.IdEmployee > 0 Then
                Dim oEmployee = Employee.roEmployee.GetEmployee(lrret.IdEmployee, New Employee.roEmployeeState)

                lrret.EmployeeName = oEmployee.Name
                lrret.EmployeeGroup = oEmployee.GroupName
                Dim Image = GetWsEmployeePhoto(lrret.IdEmployee)
                lrret.EmployeeImage = Image.Base64StringContent

                If (lrret.RequestType = 17) Then
                    Dim retRequest As New UserRequest
                    oDailyRecord = VTPortal.RequestsHelper.GetDailyRecord(requestId, Global_asax.Identity, oReqState, oLng, retRequest).Value
                End If
            End If
        Catch ex As Exception
            lrret.Status = ErrorCodes.GENERAL_ERROR
            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::PortalSvcx::GetDailyRecordRequestForSupervisor")
        End Try

        Return (DailyRecord:=oDailyRecord, DailyRecordRequest:=lrret)
    End Function

    <OperationContract()>
    <WebInvoke(Method:="POST", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function SaveRequestUserField() As StdRequestResponse
        Dim lrret As New StdRequestResponse

        If Not Global_asax.LoggedIn Then
            lrret.Status = VTPortal.StatusHelper.GetStatusFromState(Global_asax.SecurityResult)
            If lrret.Status = ErrorCodes.OK Then lrret.Status = ErrorCodes.NO_SESSION
            Return lrret
        End If

        Try

            Dim oReqState As New Requests.roRequestState
            roBaseState.SetSessionSmall(oReqState, Global_asax.Identity.ID, Global_asax.ApplicationSource, "")

            Dim fieldName As String = HttpContext.Current.Request.Params("fieldName")
            Dim fieldValue As String = HttpContext.Current.Request.Params("fieldValue")
            Dim comments As String = HttpContext.Current.Request.Params("comments")
            Dim hasHistory As Boolean = roTypes.Any2Boolean(HttpContext.Current.Request.Params("hasHistory"))
            Dim historyDate As String = HttpContext.Current.Request.Params("historyDate")
            Dim acceptWarning As Boolean = roTypes.Any2Boolean(HttpContext.Current.Request.Params("acceptWarning"))

            Dim oLng As New roLanguage()
            oLng.SetLanguageReference("LiveOne", Global_asax.Identity.Language.Key)

            lrret = VTPortal.RequestsHelper.SaveRequest(eRequestType.UserFieldsChange, Global_asax.Identity, Global_asax.Identity.IDEmployee, "", "", "", "", "", -1, -1, "", comments, "", False, "", "", "", "", "", "", fieldName, fieldValue, hasHistory, historyDate, False, Nothing, oLng, Global_asax.TimeZone, oReqState, acceptWarning, Global_asax.Supervisor)
        Catch ex As Exception
            lrret.Status = ErrorCodes.GENERAL_ERROR
            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::PortalSvcx::SaveRequest_UserField")
        End Try

        Return lrret
    End Function

    <OperationContract()>
    <WebInvoke(Method:="POST", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function SaveRequestForbiddenPunch() As StdRequestResponse
        Dim lrret As New StdRequestResponse

        If Not Global_asax.LoggedIn Then
            lrret.Status = VTPortal.StatusHelper.GetStatusFromState(Global_asax.SecurityResult)
            If lrret.Status = ErrorCodes.OK Then lrret.Status = ErrorCodes.NO_SESSION
            Return lrret
        End If

        Try

            Dim oReqState As New Requests.roRequestState
            roBaseState.SetSessionSmall(oReqState, Global_asax.Identity.ID, Global_asax.ApplicationSource, "")

            Dim punchDate As String = HttpContext.Current.Request.Params("punchDate")
            Dim idCause As Integer = roTypes.Any2Integer(HttpContext.Current.Request.Params("idCause"))
            Dim comments As String = HttpContext.Current.Request.Params("comments")
            Dim direction As String = HttpContext.Current.Request.Params("direction")
            Dim acceptWarning As Boolean = roTypes.Any2Boolean(HttpContext.Current.Request.Params("acceptWarning"))
            Dim isTelecommute As Boolean = roTypes.Any2Boolean(HttpContext.Current.Request.Params("tcType"))

            Dim oLng As New roLanguage()
            oLng.SetLanguageReference("LiveOne", Global_asax.Identity.Language.Key)

            Dim requieresRequest As Boolean = roTypes.Any2Boolean(New AdvancedParameter.roAdvancedParameter("VTPortal.RequestOnForgottenPunch", New AdvancedParameter.roAdvancedParameterState(Global_asax.Identity.ID)).Value)

            If requieresRequest Then
                lrret = VTPortal.RequestsHelper.SaveRequest(eRequestType.ForbiddenPunch, Global_asax.Identity, Global_asax.Identity.IDEmployee, punchDate, "", "", "", "", idCause, -1, "", comments, direction, False, "", "", "", "", "", "", "", "", False, "", False, Nothing, oLng, Global_asax.TimeZone, oReqState, acceptWarning, Global_asax.Supervisor, Nothing, Nothing, isTelecommute,,, comments)
            Else
                Dim tmpResponse As StdResponse = VTPortal.PunchHelper.SavePunch(Global_asax.Identity, Global_asax.TerminalIdentity, Global_asax.Identity.IDEmployee, Global_asax.TimeZone, idCause, direction, -1, -1, "", "", "", Nothing, False, "", punchDate, True, isTelecommute, True, ,,,, comments, DTOs.NotReliableCause.ForgottenPunch.ToString())
                lrret.PunchWithoutRequest = tmpResponse.Result
                lrret.Status = tmpResponse.Status
            End If
        Catch ex As Exception
            lrret.Status = ErrorCodes.GENERAL_ERROR
            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::PortalSvcx::SaveRequest_ForbiddenPunch")
        End Try

        Return lrret
    End Function

    <OperationContract()>
    <WebInvoke(Method:="POST", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function SaveRequestDailyRecord() As StdRequestResponse
        Dim lrret As New StdRequestResponse

        If Not Global_asax.LoggedIn Then
            lrret.Status = VTPortal.StatusHelper.GetStatusFromState(Global_asax.SecurityResult)
            If lrret.Status = ErrorCodes.OK Then lrret.Status = ErrorCodes.NO_SESSION
            Return lrret
        End If

        Try

            Dim dailyRecord = roTypes.Any2String(HttpContext.Current.Request.Form("dailyRecord"))

            Dim oReqState As New Requests.roRequestState
            roBaseState.SetSessionSmall(oReqState, Global_asax.Identity.ID, Global_asax.ApplicationSource, "")

            Dim oDailyRecord As roDailyRecord = roJSONHelper.DeserializeNewtonSoft(dailyRecord, GetType(roDailyRecord))

            Dim oLng As New roLanguage()
            oLng.SetLanguageReference("LiveOne", Global_asax.Identity.Language.Key)

            lrret = VTPortal.RequestsHelper.SaveRequest(eRequestType.DailyRecord, Global_asax.Identity, Global_asax.Identity.IDEmployee, oDailyRecord.RecordDate, "", "", "", "", -1, -1, "", "", "", False, "", "", "", "", "", "", "", "", False, "", False, Nothing, oLng, Global_asax.TimeZone, oReqState, True, Global_asax.Supervisor, Nothing, Nothing, False,, oDailyRecord)
        Catch ex As Exception
            lrret.Status = ErrorCodes.GENERAL_ERROR
            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::PortalSvcx::SaveRequest_ForbiddenPunch")
        End Try

        Return lrret
    End Function

    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function GetDailyRecord(idDailyRecord As Integer) As (DailyRecord As roDailyRecord, Status As Integer, Request As UserRequest)
        Dim retRequest As New UserRequest
        Dim lrret As New roGenericResponse(Of roDailyRecord)

        If Not Global_asax.LoggedIn Then
            Dim iStatus As Integer
            iStatus = VTPortal.StatusHelper.GetStatusFromState(Global_asax.SecurityResult)
            If iStatus = ErrorCodes.OK Then iStatus = ErrorCodes.NO_SESSION
            Return (DailyRecord:=Nothing, Status:=iStatus, Request:=Nothing)
        End If

        Try

            Dim oReqState As New Requests.roRequestState
            roBaseState.SetSessionSmall(oReqState, Global_asax.Identity.ID, Global_asax.ApplicationSource, "")

            Dim oLng As New roLanguage()
            oLng.SetLanguageReference("LiveOne", Global_asax.Identity.Language.Key)

            lrret = VTPortal.RequestsHelper.GetDailyRecord(idDailyRecord, Global_asax.Identity, oReqState, oLng, retRequest)

            If lrret.Value.IdEmployee <> Nothing AndAlso lrret.Value.IdEmployee > 0 Then
                Dim oEmployee = Employee.roEmployee.GetEmployee(lrret.Value.IdEmployee, New Employee.roEmployeeState)

                lrret.Value.EmployeeName = oEmployee.Name
                lrret.Value.EmployeeGroup = oEmployee.GroupName
                Dim Image = GetWsEmployeePhoto(lrret.Value.IdEmployee)
                lrret.Value.EmployeeImage = Image.Base64StringContent
            End If
        Catch ex As Exception
            lrret.Status = ErrorCodes.GENERAL_ERROR
            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::PortalSvcx::SaveRequest_ForbiddenPunch")
        End Try

        Return (DailyRecord:=lrret.Value, Status:=lrret.Status, Request:=retRequest)
    End Function

    <OperationContract()>
    <WebInvoke(Method:="POST", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function SaveRequestJustifyPunch() As StdRequestResponse
        Dim lrret As New StdRequestResponse

        If Not Global_asax.LoggedIn Then
            lrret.Status = VTPortal.StatusHelper.GetStatusFromState(Global_asax.SecurityResult)
            If lrret.Status = ErrorCodes.OK Then lrret.Status = ErrorCodes.NO_SESSION
            Return lrret
        End If

        Try

            Dim oReqState As New Requests.roRequestState
            roBaseState.SetSessionSmall(oReqState, Global_asax.Identity.ID, Global_asax.ApplicationSource, "")

            Dim punchDate As String = HttpContext.Current.Request.Params("punchDate")
            Dim idCause As Integer = roTypes.Any2Integer(HttpContext.Current.Request.Params("idCause"))
            Dim comments As String = HttpContext.Current.Request.Params("comments")
            Dim acceptWarning As Boolean = roTypes.Any2Boolean(HttpContext.Current.Request.Params("acceptWarning"))

            Dim oLng As New roLanguage()
            oLng.SetLanguageReference("LiveOne", Global_asax.Identity.Language.Key)

            lrret = VTPortal.RequestsHelper.SaveRequest(eRequestType.JustifyPunch, Global_asax.Identity, Global_asax.Identity.IDEmployee, punchDate, "", "", "", "", idCause, -1, "", comments, "", False, "", "", "", "", "", "", "", "", False, "", False, Nothing, oLng, Global_asax.TimeZone, oReqState, acceptWarning, Global_asax.Supervisor)
        Catch ex As Exception
            lrret.Status = ErrorCodes.GENERAL_ERROR
            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::PortalSvcx::SaveRequest_ForbiddenPunch")
        End Try

        Return lrret
    End Function

    <OperationContract()>
    <WebInvoke(Method:="POST", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function SaveRequestExternalWork() As StdRequestResponse
        Dim lrret As New StdRequestResponse

        If Not Global_asax.LoggedIn Then
            lrret.Status = VTPortal.StatusHelper.GetStatusFromState(Global_asax.SecurityResult)
            If lrret.Status = ErrorCodes.OK Then lrret.Status = ErrorCodes.NO_SESSION
            Return lrret
        End If

        Try

            Dim oReqState As New Requests.roRequestState
            roBaseState.SetSessionSmall(oReqState, Global_asax.Identity.ID, Global_asax.ApplicationSource, "")

            Dim externalWorkDate As String = HttpContext.Current.Request.Params("externalWorkDate")
            Dim idCause As Integer = roTypes.Any2Integer(HttpContext.Current.Request.Params("idCause"))
            Dim duration As String = HttpContext.Current.Request.Params("duration")
            Dim comments As String = HttpContext.Current.Request.Params("comments")
            Dim acceptWarning As Boolean = roTypes.Any2Boolean(HttpContext.Current.Request.Params("acceptWarning"))

            Dim oLng As New roLanguage()
            oLng.SetLanguageReference("LiveOne", Global_asax.Identity.Language.Key)

            lrret = VTPortal.RequestsHelper.SaveRequest(eRequestType.ExternalWorkResumePart, Global_asax.Identity, Global_asax.Identity.IDEmployee, externalWorkDate, "", "", "", duration, idCause, -1, "", comments, "", False, "", "", "", "", "", "", "", "", False, "", False, Nothing, oLng, Global_asax.TimeZone, oReqState, acceptWarning, Global_asax.Supervisor)
        Catch ex As Exception
            lrret.Status = ErrorCodes.GENERAL_ERROR
            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::PortalSvcx::SaveRequest_ExternalWork")
        End Try

        Return lrret
    End Function

    <OperationContract()>
    <WebInvoke(Method:="POST", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function SaveRequestChangeShift() As StdRequestResponse
        Dim lrret As New StdRequestResponse

        If Not Global_asax.LoggedIn Then
            lrret.Status = VTPortal.StatusHelper.GetStatusFromState(Global_asax.SecurityResult)
            If lrret.Status = ErrorCodes.OK Then lrret.Status = ErrorCodes.NO_SESSION
            Return lrret
        End If

        Try

            Dim oReqState As New Requests.roRequestState
            roBaseState.SetSessionSmall(oReqState, Global_asax.Identity.ID, Global_asax.ApplicationSource, "")

            Dim fromDate As String = HttpContext.Current.Request.Params("fromDate")
            Dim toDate As String = HttpContext.Current.Request.Params("toDate")
            Dim idRequestedShift As Integer = roTypes.Any2Integer(HttpContext.Current.Request.Params("idRequestedShift"))
            Dim idReplaceShift As Integer = roTypes.Any2Integer(HttpContext.Current.Request.Params("idReplaceShift"))
            Dim strStartShiftHour As String = HttpContext.Current.Request.Params("strStartShiftHour")
            Dim comments As String = HttpContext.Current.Request.Params("comments")
            Dim acceptWarning As Boolean = roTypes.Any2Boolean(HttpContext.Current.Request.Params("acceptWarning"))

            Dim oLng As New roLanguage()
            oLng.SetLanguageReference("LiveOne", Global_asax.Identity.Language.Key)

            lrret = VTPortal.RequestsHelper.SaveRequest(eRequestType.ChangeShift, Global_asax.Identity, Global_asax.Identity.IDEmployee, fromDate, toDate, "", "", "", idRequestedShift, idReplaceShift, strStartShiftHour, comments, "", False, "", "", "", "", "", "", "", "", False, "", False, Nothing, oLng, Global_asax.TimeZone, oReqState, acceptWarning, Global_asax.Supervisor)
        Catch ex As Exception
            lrret.Status = ErrorCodes.GENERAL_ERROR
            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::PortalSvcx::SaveRequest_ChangeShift")
        End Try

        Return lrret
    End Function


    <OperationContract()>
    <WebInvoke(Method:="POST", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function SaveRequestShiftExchange() As StdRequestResponse
        Dim lrret As New StdRequestResponse

        If Not Global_asax.LoggedIn Then
            lrret.Status = VTPortal.StatusHelper.GetStatusFromState(Global_asax.SecurityResult)
            If lrret.Status = ErrorCodes.OK Then lrret.Status = ErrorCodes.NO_SESSION
            Return lrret
        End If

        Try

            Dim oReqState As New Requests.roRequestState
            roBaseState.SetSessionSmall(oReqState, Global_asax.Identity.ID, Global_asax.ApplicationSource, "")

            Dim sDate As String = HttpContext.Current.Request.Params("sDate")
            Dim sCompensateDate As String = HttpContext.Current.Request.Params("sCompensateDate")
            Dim idEmployee As Integer = roTypes.Any2Integer(HttpContext.Current.Request.Params("idEmployee"))
            Dim idShift As Integer = roTypes.Any2Integer(HttpContext.Current.Request.Params("idShift"))
            Dim idSourceShift As Integer = roTypes.Any2Integer(HttpContext.Current.Request.Params("idSourceShift"))
            Dim comments As String = HttpContext.Current.Request.Params("comments")
            Dim acceptWarning As Boolean = roTypes.Any2Boolean(HttpContext.Current.Request.Params("acceptWarning"))

            Dim oLng As New roLanguage()
            oLng.SetLanguageReference("LiveOne", Global_asax.Identity.Language.Key)

            lrret = VTPortal.RequestsHelper.SaveRequest(eRequestType.ExchangeShiftBetweenEmployees, Global_asax.Identity, Global_asax.Identity.IDEmployee, sDate, sCompensateDate, "", "", "", idShift, idEmployee, "", comments, "", False, "", "", "", idSourceShift, "", "", "", "", False, "", False, Nothing, oLng, Global_asax.TimeZone, oReqState, acceptWarning, Global_asax.Supervisor)
        Catch ex As Exception
            lrret.Status = ErrorCodes.GENERAL_ERROR
            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::PortalSvcx::SaveRequest_ChangeShift")
        End Try

        Return lrret
    End Function

    <OperationContract()>
    <WebInvoke(Method:="POST", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function SaveRequestHolidays() As StdRequestResponse
        Dim lrret As New StdRequestResponse

        If Not Global_asax.LoggedIn Then
            lrret.Status = VTPortal.StatusHelper.GetStatusFromState(Global_asax.SecurityResult)
            If lrret.Status = ErrorCodes.OK Then lrret.Status = ErrorCodes.NO_SESSION
            Return lrret
        End If

        Try

            Dim oReqState As New Requests.roRequestState
            roBaseState.SetSessionSmall(oReqState, Global_asax.Identity.ID, Global_asax.ApplicationSource, "")

            Dim strDates As String = HttpContext.Current.Request.Params("strDates")
            Dim idHolidays As Integer = roTypes.Any2Integer(HttpContext.Current.Request.Params("idHolidays"))
            Dim comments As String = HttpContext.Current.Request.Params("comments")
            Dim acceptWarning As Boolean = roTypes.Any2Boolean(HttpContext.Current.Request.Params("acceptWarning"))

            Dim oLng As New roLanguage()
            oLng.SetLanguageReference("LiveOne", Global_asax.Identity.Language.Key)

            lrret = VTPortal.RequestsHelper.SaveRequest(eRequestType.VacationsOrPermissions, Global_asax.Identity, Global_asax.Identity.IDEmployee, strDates, "", "", "", "", idHolidays, -1, "", comments, "", False, "", "", "", "", "", "", "", "", False, "", False, Nothing, oLng, Global_asax.TimeZone, oReqState, acceptWarning, Global_asax.Supervisor)
        Catch ex As Exception
            lrret.Status = ErrorCodes.GENERAL_ERROR
            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::PortalSvcx::SaveRequest_Holidays")
        End Try

        Return lrret
    End Function

    <OperationContract()>
    <WebInvoke(Method:="POST", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function SaveRequestPlannedAbsences() As StdRequestResponse
        Dim lrret As New StdRequestResponse

        If Not Global_asax.LoggedIn Then
            lrret.Status = VTPortal.StatusHelper.GetStatusFromState(Global_asax.SecurityResult)
            If lrret.Status = ErrorCodes.OK Then lrret.Status = ErrorCodes.NO_SESSION
            Return lrret
        End If

        Try

            Dim curFile As HttpPostedFile = HttpContext.Current.Request.Files("userfile")

            Dim oReqState As New Requests.roRequestState
            roBaseState.SetSessionSmall(oReqState, Global_asax.Identity.ID, Global_asax.ApplicationSource, "")

            Dim fromDate As String = HttpContext.Current.Request.Params("fromDate")
            Dim toDate As String = HttpContext.Current.Request.Params("toDate")
            Dim idCause As Integer = roTypes.Any2Integer(HttpContext.Current.Request.Params("idCause"))
            Dim idDocumentTemplate As Long = roTypes.Any2Integer(HttpContext.Current.Request.Form("idTemplateDocument"))
            Dim comments As String = HttpContext.Current.Request.Params("comments")
            Dim acceptWarning As Boolean = roTypes.Any2Boolean(HttpContext.Current.Request.Params("acceptWarning"))

            Dim oLng As New roLanguage()
            oLng.SetLanguageReference("LiveOne", Global_asax.Identity.Language.Key)

            If curFile Is Nothing Then
                lrret = VTPortal.RequestsHelper.SaveRequest(eRequestType.PlannedAbsences, Global_asax.Identity, Global_asax.Identity.IDEmployee, fromDate, toDate, "", "", "", idCause, -1, "", comments, "", False, "", "", "", "", "", "", "", "", False, "", False, Nothing, oLng, Global_asax.TimeZone, oReqState, acceptWarning, Global_asax.Supervisor)
            Else
                lrret = VTPortal.RequestsHelper.SaveRequest(eRequestType.PlannedAbsences, Global_asax.Identity, Global_asax.Identity.IDEmployee, fromDate, toDate, "", "", "", idCause, -1, "", comments, "", False, "", "", "", "", "", "", "", "", False, "", False, Nothing, oLng, Global_asax.TimeZone, oReqState, acceptWarning, Global_asax.Supervisor, curFile, idDocumentTemplate)
            End If
        Catch ex As Exception
            lrret.Status = ErrorCodes.GENERAL_ERROR
            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::PortalSvcx::SaveRequest_PlannedAbsences")
        End Try

        Return lrret
    End Function

    <OperationContract()>
    <WebInvoke(Method:="POST", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function SaveRequestPlannedCauses() As StdRequestResponse
        Dim lrret As New StdRequestResponse

        If Not Global_asax.LoggedIn Then
            lrret.Status = VTPortal.StatusHelper.GetStatusFromState(Global_asax.SecurityResult)
            If lrret.Status = ErrorCodes.OK Then lrret.Status = ErrorCodes.NO_SESSION
            Return lrret
        End If

        Try

            Dim curFile As HttpPostedFile = HttpContext.Current.Request.Files("userfile")
            ' If curFile Is Nothing Then curFile = HttpContext.Current.Request.Files("file")

            Dim oReqState As New Requests.roRequestState
            roBaseState.SetSessionSmall(oReqState, Global_asax.Identity.ID, Global_asax.ApplicationSource, "")

            Dim fromDate As String = HttpContext.Current.Request.Params("fromDate")
            Dim toDate As String = HttpContext.Current.Request.Params("toDate")
            Dim fromHour As String = HttpContext.Current.Request.Params("fromHour")
            Dim toHour As String = HttpContext.Current.Request.Params("toHour")
            Dim duration As String = HttpContext.Current.Request.Params("duration")
            Dim idDocumentTemplate As Long = roTypes.Any2Integer(HttpContext.Current.Request.Form("idTemplateDocument"))
            Dim idCause As Integer = roTypes.Any2Integer(HttpContext.Current.Request.Params("idCause"))
            Dim comments As String = HttpContext.Current.Request.Params("comments")
            Dim acceptWarning As Boolean = roTypes.Any2Boolean(HttpContext.Current.Request.Params("acceptWarning"))

            Dim oLng As New roLanguage()
            oLng.SetLanguageReference("LiveOne", Global_asax.Identity.Language.Key)

            If curFile Is Nothing Then
                lrret = VTPortal.RequestsHelper.SaveRequest(eRequestType.PlannedCauses, Global_asax.Identity, Global_asax.Identity.IDEmployee, fromDate, toDate, fromHour, toHour, duration, idCause, -1, "", comments, "", False, "", "", "", "", "", "", "", "", False, "", False, Nothing, oLng, Global_asax.TimeZone, oReqState, acceptWarning, Global_asax.Supervisor)
            Else
                lrret = VTPortal.RequestsHelper.SaveRequest(eRequestType.PlannedCauses, Global_asax.Identity, Global_asax.Identity.IDEmployee, fromDate, toDate, fromHour, toHour, duration, idCause, -1, "", comments, "", False, "", "", "", "", "", "", "", "", False, "", False, Nothing, oLng, Global_asax.TimeZone, oReqState, acceptWarning, Global_asax.Supervisor, curFile, idDocumentTemplate)
            End If
        Catch ex As Exception
            lrret.Status = ErrorCodes.GENERAL_ERROR
            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::PortalSvcx::SaveRequest_PlannedCauses")
        End Try

        Return lrret
    End Function

    <OperationContract()>
    <WebInvoke(Method:="POST", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function SaveRequestPlannedOvertime() As StdRequestResponse
        Dim lrret As New StdRequestResponse

        If Not Global_asax.LoggedIn Then
            lrret.Status = VTPortal.StatusHelper.GetStatusFromState(Global_asax.SecurityResult)
            If lrret.Status = ErrorCodes.OK Then lrret.Status = ErrorCodes.NO_SESSION
            Return lrret
        End If

        Try

            Dim curFile As HttpPostedFile = HttpContext.Current.Request.Files("userfile")

            Dim oReqState As New Requests.roRequestState
            roBaseState.SetSessionSmall(oReqState, Global_asax.Identity.ID, Global_asax.ApplicationSource, "")

            Dim oLng As New roLanguage()
            oLng.SetLanguageReference("LiveOne", Global_asax.Identity.Language.Key)

            Dim fromDate As String = HttpContext.Current.Request.Params("fromDate")
            Dim toDate As String = HttpContext.Current.Request.Params("toDate")
            Dim fromHour As String = HttpContext.Current.Request.Params("fromHour")
            Dim toHour As String = HttpContext.Current.Request.Params("toHour")
            Dim duration As String = HttpContext.Current.Request.Params("duration")
            Dim idDocumentTemplate As Long = roTypes.Any2Integer(HttpContext.Current.Request.Form("idTemplateDocument"))
            Dim idCause As Integer = roTypes.Any2Integer(HttpContext.Current.Request.Params("idCause"))
            Dim comments As String = HttpContext.Current.Request.Params("comments")
            Dim acceptWarning As Boolean = roTypes.Any2Boolean(HttpContext.Current.Request.Params("acceptWarning"))

            If curFile Is Nothing Then
                lrret = VTPortal.RequestsHelper.SaveRequest(eRequestType.PlannedOvertimes, Global_asax.Identity, Global_asax.Identity.IDEmployee, fromDate, toDate, fromHour, toHour, duration, idCause, -1, "", comments, "", False, "", "", "", "", "", "", "", "", False, "", False, Nothing, oLng, Global_asax.TimeZone, oReqState, acceptWarning, Global_asax.Supervisor)
            Else
                lrret = VTPortal.RequestsHelper.SaveRequest(eRequestType.PlannedOvertimes, Global_asax.Identity, Global_asax.Identity.IDEmployee, fromDate, toDate, fromHour, toHour, duration, idCause, -1, "", comments, "", False, "", "", "", "", "", "", "", "", False, "", False, Nothing, oLng, Global_asax.TimeZone, oReqState, acceptWarning, Global_asax.Supervisor, curFile, idDocumentTemplate)
            End If
        Catch ex As Exception
            lrret.Status = ErrorCodes.GENERAL_ERROR
            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::PortalSvcx::SaveRequest_PlannedOvertime")
        End Try

        Return lrret
    End Function

    <OperationContract()>
    <WebInvoke(Method:="POST", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function SaveRequestForbiddenTaskPunch() As StdRequestResponse
        Dim lrret As New StdRequestResponse

        If Not Global_asax.LoggedIn Then
            lrret.Status = VTPortal.StatusHelper.GetStatusFromState(Global_asax.SecurityResult)
            If lrret.Status = ErrorCodes.OK Then lrret.Status = ErrorCodes.NO_SESSION
            Return lrret
        End If

        Try

            Dim oReqState As New Requests.roRequestState
            roBaseState.SetSessionSmall(oReqState, Global_asax.Identity.ID, Global_asax.ApplicationSource, "")

            Dim punchDate As String = HttpContext.Current.Request.Params("punchDate")
            Dim continueOnPunchDate As String = HttpContext.Current.Request.Params("continueOnPunchDate")
            Dim idTask As Integer = roTypes.Any2Integer(HttpContext.Current.Request.Params("idTask"))
            Dim idContinueOnTask As Integer = roTypes.Any2Integer(HttpContext.Current.Request.Params("idContinueOnTask"))
            Dim completeTask As Boolean = roTypes.Any2Boolean(HttpContext.Current.Request.Params("completeTask"))
            Dim value1 As String = HttpContext.Current.Request.Params("value1")
            Dim value2 As String = HttpContext.Current.Request.Params("value2")
            Dim value3 As String = HttpContext.Current.Request.Params("value3")
            Dim value4 As String = HttpContext.Current.Request.Params("value4")
            Dim value5 As String = HttpContext.Current.Request.Params("value5")
            Dim value6 As String = HttpContext.Current.Request.Params("value6")
            Dim comments As String = HttpContext.Current.Request.Params("comments")
            Dim acceptWarning As Boolean = roTypes.Any2Boolean(HttpContext.Current.Request.Params("acceptWarning"))
            Dim isTelecommute As Boolean = roTypes.Any2Boolean(HttpContext.Current.Request.Params("tcType"))

            Dim oLng As New roLanguage()
            oLng.SetLanguageReference("LiveOne", Global_asax.Identity.Language.Key)

            lrret = VTPortal.RequestsHelper.SaveRequest(eRequestType.ForbiddenTaskPunch, Global_asax.Identity, Global_asax.Identity.IDEmployee, punchDate, continueOnPunchDate, "", "", "", idTask, idContinueOnTask, "", comments, "", completeTask, value1, value2, value3, value4, value5, value6, "", "", False, "", False, Nothing, oLng, Global_asax.TimeZone, oReqState, acceptWarning, Global_asax.Supervisor, Nothing, 0, isTelecommute)
        Catch ex As Exception
            lrret.Status = ErrorCodes.GENERAL_ERROR
            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::PortalSvcx::SaveRequest_ForbiddenTaskPunch")
        End Try

        Return lrret
    End Function

    <OperationContract()>
    <WebInvoke(Method:="POST", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function SaveRequestCancelHolidays() As StdRequestResponse
        Dim lrret As New StdRequestResponse

        If Not Global_asax.LoggedIn Then
            lrret.Status = VTPortal.StatusHelper.GetStatusFromState(Global_asax.SecurityResult)
            If lrret.Status = ErrorCodes.OK Then lrret.Status = ErrorCodes.NO_SESSION
            Return lrret
        End If

        Try

            Dim oReqState As New Requests.roRequestState
            roBaseState.SetSessionSmall(oReqState, Global_asax.Identity.ID, Global_asax.ApplicationSource, "")

            Dim strDates As String = HttpContext.Current.Request.Params("strDates")
            Dim idCause As String = roTypes.Any2Integer(HttpContext.Current.Request.Params("idCause"))
            Dim idShift As Integer = roTypes.Any2Integer(HttpContext.Current.Request.Params("idShift"))
            Dim comments As String = HttpContext.Current.Request.Params("comments")
            Dim acceptWarning As Boolean = roTypes.Any2Boolean(HttpContext.Current.Request.Params("acceptWarning"))

            Dim oLng As New roLanguage()
            oLng.SetLanguageReference("LiveOne", Global_asax.Identity.Language.Key)

            lrret = VTPortal.RequestsHelper.SaveRequest(eRequestType.CancelHolidays, Global_asax.Identity, Global_asax.Identity.IDEmployee, strDates, "", "", "", "", idCause, idShift, "", comments, "", False, "", "", "", "", "", "", "", "", False, "", False, Nothing, oLng, Global_asax.TimeZone, oReqState, acceptWarning, Global_asax.Supervisor)
        Catch ex As Exception
            lrret.Status = ErrorCodes.GENERAL_ERROR
            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::PortalSvcx::SaveRequest_CancelHolidays")
        End Try

        Return lrret
    End Function

    <OperationContract()>
    <WebInvoke(Method:="POST", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function SaveRequestForbiddenCostCenterPunch() As StdRequestResponse
        Dim lrret As New StdRequestResponse

        If Not Global_asax.LoggedIn Then
            lrret.Status = VTPortal.StatusHelper.GetStatusFromState(Global_asax.SecurityResult)
            If lrret.Status = ErrorCodes.OK Then lrret.Status = ErrorCodes.NO_SESSION
            Return lrret
        End If

        Try

            Dim oReqState As New Requests.roRequestState
            roBaseState.SetSessionSmall(oReqState, Global_asax.Identity.ID, Global_asax.ApplicationSource, "")

            Dim punchDate As String = HttpContext.Current.Request.Params("punchDate")
            Dim idCostCenter As String = roTypes.Any2Integer(HttpContext.Current.Request.Params("idCostCenter"))
            Dim comments As String = HttpContext.Current.Request.Params("comments")
            Dim acceptWarning As Boolean = roTypes.Any2Boolean(HttpContext.Current.Request.Params("acceptWarning"))
            Dim isTelecommute As Boolean = roTypes.Any2Boolean(HttpContext.Current.Request.Params("tcType"))

            Dim oLng As New roLanguage()
            oLng.SetLanguageReference("LiveOne", Global_asax.Identity.Language.Key)

            lrret = VTPortal.RequestsHelper.SaveRequest(eRequestType.ForgottenCostCenterPunch, Global_asax.Identity, Global_asax.Identity.IDEmployee, punchDate, "", "", "", "", idCostCenter, -1, "", comments, "", False, "", "", "", "", "", "", "", "", False, "", False, Nothing, oLng, Global_asax.TimeZone, oReqState, acceptWarning, Global_asax.Supervisor, Nothing, 0, isTelecommute,,, comments)
        Catch ex As Exception
            lrret.Status = ErrorCodes.GENERAL_ERROR
            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::PortalSvcx::SaveRequest_ForbiddenCostCenterPunch")
        End Try

        Return lrret
    End Function


    <OperationContract()>
    <WebInvoke(Method:="POST", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function SaveRequestPlannedHoliday() As StdRequestResponse
        Dim lrret As New StdRequestResponse

        If Not Global_asax.LoggedIn Then
            lrret.Status = VTPortal.StatusHelper.GetStatusFromState(Global_asax.SecurityResult)
            If lrret.Status = ErrorCodes.OK Then lrret.Status = ErrorCodes.NO_SESSION
            Return lrret
        End If

        Try
            Dim oReqState As New Requests.roRequestState
            roBaseState.SetSessionSmall(oReqState, Global_asax.Identity.ID, Global_asax.ApplicationSource, "")

            Dim datesStr As String = HttpContext.Current.Request.Params("datesStr")
            Dim allDay As Boolean = roTypes.Any2Boolean(HttpContext.Current.Request.Params("allDay"))
            Dim fromHour As String = HttpContext.Current.Request.Params("fromHour")
            Dim toHour As String = HttpContext.Current.Request.Params("toHour")
            Dim idCause As Integer = roTypes.Any2Integer(HttpContext.Current.Request.Params("idCause"))
            Dim comments As String = HttpContext.Current.Request.Params("comments")
            Dim acceptWarning As Boolean = roTypes.Any2Boolean(HttpContext.Current.Request.Params("acceptWarning"))

            Dim oLng As New roLanguage()
            oLng.SetLanguageReference("LiveOne", Global_asax.Identity.Language.Key)

            lrret = VTPortal.RequestsHelper.SaveRequest(eRequestType.PlannedHolidays, Global_asax.Identity, Global_asax.Identity.IDEmployee, datesStr, "", fromHour, toHour, "", idCause, -1, "", comments, "", False, "", "", "", "", "", "", "", "", False, "", allDay, Nothing, oLng, Global_asax.TimeZone, oReqState, acceptWarning, Global_asax.Supervisor)
        Catch ex As Exception
            lrret.Status = ErrorCodes.GENERAL_ERROR
            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::PortalSvcx::SaveRequest_ForbiddenCostCenterPunch")
        End Try

        Return lrret
    End Function

    ''' <summary>
    ''' Se requiere que la sesión este iniciada en el sistema. Esta función responde a un método GET del servidor.
    ''' Obtiene la información del horario solicitado indicando si este es flotante o no y la hora base en la que empieza
    ''' </summary>
    ''' <param name="shiftId">Identificador del horario </param>
    ''' <returns>Obtenemos un objeto del tipo ShiftFloatingInfo. Si se ha producido un error en la consulta la variable de esato del objeto informa de ello.</returns>
    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function CheckIsFloatingShift(ByVal shiftId As Integer) As ShiftFloatingInfo
        Dim lrret As New ShiftFloatingInfo

        If Not Global_asax.LoggedIn Then
            lrret.Status = VTPortal.StatusHelper.GetStatusFromState(Global_asax.SecurityResult)
            If lrret.Status = ErrorCodes.OK Then lrret.Status = ErrorCodes.NO_SESSION
            Return lrret
        End If

        Try
            Dim oShiftState As New Shift.roShiftState
            roBaseState.SetSessionSmall(oShiftState, Global_asax.Identity.ID, Global_asax.ApplicationSource, "")
            lrret = VTPortal.ShiftsHelper.IsFloatingShift(shiftId, oShiftState)

            lrret.StartFloating = roTypes.CreateDateTime(DateTime.Now.Date.Year, DateTime.Now.Date.Month, DateTime.Now.Date.Day, lrret.StartFloating.Hour, lrret.StartFloating.Minute, 0)
        Catch ex As Exception
            lrret.Status = ErrorCodes.GENERAL_ERROR
            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::PortalSvcx::CheckIsFloatingShift")
        End Try

        Return lrret
    End Function

    ''' <summary>
    ''' Se requiere que la sesión este iniciada en el sistema. Esta función responde a un método GET del servidor.
    ''' Obtiene la información del horario solicitado indicando si es un dia solo para días laborales
    ''' </summary>
    ''' <param name="shiftId">Identificador del horario </param>
    ''' <returns>Obtenemos un objeto del tipo ShiftFloatingInfo. Si se ha producido un error en la consulta la variable de esato del objeto informa de ello.</returns>
    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function CheckIsWorkingdayShift(ByVal shiftId As Integer) As StdResponse
        Dim lrret As New StdResponse

        If Not Global_asax.LoggedIn Then
            lrret.Status = VTPortal.StatusHelper.GetStatusFromState(Global_asax.SecurityResult)
            If lrret.Status = ErrorCodes.OK Then lrret.Status = ErrorCodes.NO_SESSION
            Return lrret
        End If

        Try
            Dim oShiftState As New Shift.roShiftState
            roBaseState.SetSessionSmall(oShiftState, Global_asax.Identity.ID, Global_asax.ApplicationSource, "")
            lrret = VTPortal.ShiftsHelper.IsWorkingdayShift(shiftId, oShiftState)
        Catch ex As Exception
            lrret.Status = ErrorCodes.GENERAL_ERROR
            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::PortalSvcx::CheckIsWorkingdayShift")
        End Try

        Return lrret
    End Function

    ''' <summary>
    ''' Se requiere que la sesión este iniciada en el sistema. Esta función responde a un método GET del servidor.
    ''' Obtiene todos los valores disponibles para un campo de la ficha de tipo lista
    ''' </summary>
    ''' <param name="fieldName">Campo de la ficha del cual queremos obtener los valores disponibles</param>
    ''' <returns>Obtenemos un objeto del tipo FieldValues. Si se ha producido un error en la consulta la variable de esato del objeto informa de ello.</returns>
    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function GetUserFieldAvailableValues(ByVal fieldName As String) As FieldValues
        Dim lrret As New FieldValues

        If Not Global_asax.LoggedIn Then
            lrret.Status = VTPortal.StatusHelper.GetStatusFromState(Global_asax.SecurityResult)
            If lrret.Status = ErrorCodes.OK Then lrret.Status = ErrorCodes.NO_SESSION
            Return lrret
        End If

        Try

            Dim oState As New VTUserFields.UserFields.roUserFieldState(-1)
            roBaseState.SetSessionSmall(oState, Global_asax.Identity.ID, Global_asax.ApplicationSource, "")

            Dim oUserField As New VTUserFields.UserFields.roUserField(oState, fieldName, Types.EmployeeField, False)
            Dim vals As New Generic.List(Of String)
            If oUserField IsNot Nothing Then
                For Each lVal As String In oUserField.ListValues
                    vals.Add(lVal)
                Next
            End If
            lrret.Values = vals.ToArray()
            lrret.Status = ErrorCodes.OK
        Catch ex As Exception
            lrret.Status = ErrorCodes.GENERAL_ERROR
            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::PortalSvcx::GetFieldValues")
        End Try

        Return lrret
    End Function

    ''' <summary>
    ''' Se requiere que la sesión este iniciada en el sistema. Esta función responde a un método GET del servidor.
    ''' Permite borrar una solicitud del empleado que ha iniciado sesión siempre que este en estado pendiente. Una solicitud en otro estado no se puede borrar
    ''' </summary>
    ''' <param name="requestId">Identificador de la solicitud que vamos a borrar</param>
    ''' <returns>Obtenemos un objeto estandar de respuesta de petición indicando si se ha podido borrar la solicitud y en caso contrario el código de error obtenido.</returns>
    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function DeleteRequest(ByVal requestId As Integer) As StdResponse
        Dim lrret As New StdResponse
        If Not Global_asax.LoggedIn Then
            lrret.Status = VTPortal.StatusHelper.GetStatusFromState(Global_asax.SecurityResult)
            If lrret.Status = ErrorCodes.OK Then lrret.Status = ErrorCodes.NO_SESSION
            Return lrret
        End If

        Try
            lrret.Status = ErrorCodes.OK

            Dim oReqState As New Requests.roRequestState(-1)
            roBaseState.SetSessionSmall(oReqState, Global_asax.Identity.ID, Global_asax.ApplicationSource, "")

            lrret = VTPortal.RequestsHelper.DeleteRequest(requestId, oReqState)
        Catch ex As Exception
            lrret.Status = ErrorCodes.GENERAL_ERROR
            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::PortalSvcx::DeleteRequest")
        End Try

        Return lrret

    End Function

    ''' <summary>
    ''' Se requiere que la sesión este iniciada en el sistema. Esta función responde a un método GET del servidor.
    ''' Permite borrar una solicitud del empleado que ha iniciado sesión siempre que este en estado pendiente. Una solicitud en otro estado no se puede borrar
    ''' </summary>
    ''' <param name="requestId">Identificador de la ausencia que vamos a borrar</param>
    ''' <returns>Obtenemos un objeto estandar de respuesta de petición indicando si se ha podido borrar la ausencia y en caso contrario el código de error obtenido.</returns>
    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function DeleteProgrammedAbsence(ByVal forecastId As Integer, ByVal forecastType As String) As StdRequestResponse
        Dim lrret As New StdRequestResponse
        If Not Global_asax.LoggedIn Then
            lrret.Status = VTPortal.StatusHelper.GetStatusFromState(Global_asax.SecurityResult)
            If lrret.Status = ErrorCodes.OK Then lrret.Status = ErrorCodes.NO_SESSION
            Return lrret
        End If

        Try
            lrret.Status = ErrorCodes.OK

            Dim oProgrammedAbsenceState As New Absence.roProgrammedAbsenceState(-1)
            roBaseState.SetSessionSmall(oProgrammedAbsenceState, Global_asax.Identity.ID, Global_asax.ApplicationSource, "")

            lrret = VTPortal.ShiftsHelper.DeleteProgrammedAbsence(forecastId, forecastType, oProgrammedAbsenceState)
        Catch ex As Exception
            lrret.Status = ErrorCodes.GENERAL_ERROR
            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::PortalSvcx::DeleteProgrammedAbsence")
        End Try

        Return lrret

    End Function

    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function GetCommuniqueById(ByVal idCommunique) As roCommuniqueStatusResponse
        Dim lrret As New roCommuniqueStatusResponse
        If Not Global_asax.LoggedIn Then
            lrret.oState.Result = VTPortal.StatusHelper.GetStatusFromState(Global_asax.SecurityResult)
            If lrret.oState.Result = ErrorCodes.OK Then lrret.oState.Result = ErrorCodes.NO_SESSION
            Return lrret
        End If

        Try
            lrret.oState.Result = ErrorCodes.OK

            Dim oComState As New roWsState
            roBaseState.SetSessionSmall(oComState, Global_asax.Identity.ID, Global_asax.ApplicationSource, "")

            lrret = CommuniqueMethods.GetCommuniqueWithStatistics(idCommunique, oComState, Global_asax.Identity.IDEmployee, False)

            If lrret.Status.EmployeeCommuniqueStatus(0).Read = False Then
                CommuniqueMethods.SetCommuniqueRead(idCommunique, Global_asax.Identity.IDEmployee, oComState, True)
            End If
            lrret.Status.Communique.Message = lrret.Status.Communique.Message.Replace(vbCr, "<br>").Replace(vbLf, "<br>")

            lrret.Status.Communique.Message = lrret.Status.Communique.Message.Replace(vbCr, "<br>").Replace(vbLf, "<br>")
        Catch ex As Exception
            lrret.oState.Result = ErrorCodes.GENERAL_ERROR
            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::PortalSvcx::GetCommuniqueById")
        End Try

        Return lrret

    End Function

    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function GetSurveyById(ByVal idSurvey) As roSurveyForPortal
        Dim lrret As New roSurveyForPortal
        If Not Global_asax.LoggedIn Then
            lrret.oState.Result = VTPortal.StatusHelper.GetStatusFromState(Global_asax.SecurityResult)
            If lrret.oState.Result = ErrorCodes.OK Then lrret.oState.Result = ErrorCodes.NO_SESSION
            Return lrret
        End If

        Try
            lrret.oState.Result = ErrorCodes.OK

            Dim oComState As New roWsState
            roBaseState.SetSessionSmall(oComState, Global_asax.Identity.ID, Global_asax.ApplicationSource, "")

            Dim oSurveyManager As New roSurveyManager()
            lrret.Survey = oSurveyManager.GetSurvey(idSurvey, True, False)
        Catch ex As Exception
            lrret.oState.Result = ErrorCodes.GENERAL_ERROR
            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::PortalSvcx::GetSurveyById")
        End Try

        Return lrret

    End Function

    <OperationContract()>
    <WebInvoke(Method:="POST", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function SendSurveyResponse() As roSurveyResponsePortal
        Dim lrret As New roSurveyResponsePortal
        If Not Global_asax.LoggedIn Then
            lrret.oState.Result = VTPortal.StatusHelper.GetStatusFromState(Global_asax.SecurityResult)
            If lrret.oState.Result = ErrorCodes.OK Then lrret.oState.Result = ErrorCodes.NO_SESSION
            Return lrret
        End If

        Try
            lrret.oState.Result = ErrorCodes.OK

            Dim oComState As New roWsState
            roBaseState.SetSessionSmall(oComState, Global_asax.Identity.ID, Global_asax.ApplicationSource, "")

            Dim roSurvey As New roSurveyResponse
            roSurvey.IdSurvey = roTypes.Any2Integer(HttpContext.Current.Request.Form("idSurvey"))
            roSurvey.IdEmployee = roTypes.Any2Integer(HttpContext.Current.Request.Form("idEmployee"))
            roSurvey.ResponseData = roTypes.Any2String(HttpContext.Current.Request.Form("Response"))
            roSurvey.Timestamp = Date.Now

            Dim oSurveyManager As New roSurveyManager()

            lrret.Result = oSurveyManager.SaveSurveyResponse(roSurvey, False)
        Catch ex As Exception
            lrret.oState.Result = ErrorCodes.GENERAL_ERROR
            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::PortalSvcx::GetSurveyById")
        End Try

        Return lrret

    End Function

    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function RegisterFirebaseToken(ByVal token, ByVal uuid) As roTokenResponse
        Dim oState As New roSecurityState(-1, HttpContext.Current)
        Dim lrret As New roTokenResponse
        If Not Global_asax.LoggedIn Then
            lrret.Result = VTPortal.StatusHelper.GetStatusFromState(Global_asax.SecurityResult)
            If lrret.Result = ErrorCodes.OK Then lrret.oState.Result = ErrorCodes.NO_SESSION
            Return lrret
        End If

        Try
            lrret.Result = ErrorCodes.OK

            Dim oComState As New roWsState
            roBaseState.SetSessionSmall(oComState, Global_asax.Identity.ID, Global_asax.ApplicationSource, "")

            lrret = VTPortal.SecurityHelper.RegisterFirebaseToken(token, uuid, Global_asax.Identity.ID, oState)
        Catch ex As Exception
            lrret.Result = ErrorCodes.GENERAL_ERROR
            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::PortalSvcx::RegisterFirebaseToken")
        End Try

        Return lrret

    End Function

    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function AnswerCommunique(ByVal idCommunique, ByVal answer) As roCommuniqueStandarResponse
        Dim lrret As New roCommuniqueStandarResponse
        If Not Global_asax.LoggedIn Then
            lrret.Result = VTPortal.StatusHelper.GetStatusFromState(Global_asax.SecurityResult)
            If lrret.Result = ErrorCodes.OK Then lrret.oState.Result = ErrorCodes.NO_SESSION
            Return lrret
        End If

        Try
            lrret.Result = ErrorCodes.OK

            Dim oComState As New roWsState
            roBaseState.SetSessionSmall(oComState, Global_asax.Identity.ID, Global_asax.ApplicationSource, "")

            lrret = CommuniqueMethods.AnswerCommunique(idCommunique, Global_asax.Identity.IDEmployee, answer, oComState, True)
        Catch ex As Exception
            lrret.Result = ErrorCodes.GENERAL_ERROR
            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::PortalSvcx::AnswerCommunique")
        End Try

        Return lrret

    End Function

    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function SetCommuniqueRead(ByVal idCommunique, ByVal idEmployee) As roCommuniqueStandarResponse
        Dim lrret As New roCommuniqueStandarResponse
        If Not Global_asax.LoggedIn Then
            lrret.Result = VTPortal.StatusHelper.GetStatusFromState(Global_asax.SecurityResult)
            If lrret.Result = ErrorCodes.OK Then lrret.oState.Result = ErrorCodes.NO_SESSION
            Return lrret
        End If

        Try
            lrret.Result = ErrorCodes.OK

            Dim oComState As New roWsState
            roBaseState.SetSessionSmall(oComState, Global_asax.Identity.ID, Global_asax.ApplicationSource, "")

            lrret = CommuniqueMethods.SetCommuniqueRead(idCommunique, idEmployee, oComState, True)
        Catch ex As Exception
            lrret.Result = ErrorCodes.GENERAL_ERROR
            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::PortalSvcx::SetCommuniqueRead")
        End Try

        Return lrret

    End Function

    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function GetMyCommuniques() As roCommuniqueListResponse
        Dim lrret As New roCommuniqueListResponse
        If Not Global_asax.LoggedIn Then
            lrret.oState.Result = VTPortal.StatusHelper.GetStatusFromState(Global_asax.SecurityResult)
            If lrret.oState.Result = ErrorCodes.OK Then lrret.oState.Result = ErrorCodes.NO_SESSION
            Return lrret
        End If

        Try
            lrret.oState.Result = ErrorCodes.OK

            Dim oComState As New roWsState
            roBaseState.SetSessionSmall(oComState, Global_asax.Identity.ID, Global_asax.ApplicationSource, "")

            lrret = CommuniqueMethods.GetAllCommuniques(oComState, Global_asax.Identity.IDEmployee, False)
        Catch ex As Exception
            lrret.oState.Result = ErrorCodes.GENERAL_ERROR
            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::PortalSvcx::GetMyCommuniques")
        End Try

        Return lrret

    End Function

    ''' <summary>
    ''' Se requiere que la sesión este iniciada en el sistema. Esta función responde a un método GET del servidor.
    ''' Consulta el listado de documentos entregados a VisualTime.
    ''' </summary>
    ''' <returns>Obtenemos un objeto de tipo roEmployeeDocumentsResponse con el listado de documentos que ha entregado el empleado en el sistema.</returns>
    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function GetMyDocuments(ByVal dateStart As String, ByVal dateEnd As String, ByVal filter As String, ByVal orderBy As String) As roEmployeeDocumentsResponse
        Dim lrret As New roEmployeeDocumentsResponse
        If Not Global_asax.LoggedIn Then
            lrret.Status = VTPortal.StatusHelper.GetStatusFromState(Global_asax.SecurityResult)
            If lrret.Status = ErrorCodes.OK Then lrret.Status = ErrorCodes.NO_SESSION
            Return lrret
        End If

        Try
            lrret.Status = ErrorCodes.OK

            Dim oDocState As New VTDocuments.roDocumentState(-1)
            roBaseState.SetSessionSmall(oDocState, Global_asax.Identity.ID, Global_asax.ApplicationSource, "")

            Dim strOrderBy As String = orderBy
            Dim strFilter As String = filter

            Dim sDate As Date = New Date(1900, 1, 1)
            If dateStart <> "" Then
                sDate = VTPortal.CommonHelper.ParseDatetime(dateStart)
            End If

            Dim eDate As Date = New Date(2079, 1, 1)
            If dateEnd <> "" Then
                eDate = VTPortal.CommonHelper.ParseDatetime(dateEnd)
            End If

            lrret = VTPortal.DocumentsHelper.GetMyDocuments(Global_asax.Identity.IDEmployee, sDate, eDate, strFilter, strOrderBy, oDocState)
        Catch ex As Exception
            lrret.Status = ErrorCodes.GENERAL_ERROR
            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::PortalSvcx::GetMyDocuments")
        End Try

        Return lrret

    End Function

    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function GetDocumentBytes(ByVal documentId As String) As roGenericResponse(Of roDocumentFile)
        Dim lrret As New roGenericResponse(Of roDocumentFile)
        If Not Global_asax.LoggedIn Then
            lrret.Status = VTPortal.StatusHelper.GetStatusFromState(Global_asax.SecurityResult)
            If lrret.Status = ErrorCodes.OK Then lrret.Status = ErrorCodes.NO_SESSION
            Return lrret
        End If

        Try
            Dim oState As New VTDocuments.roDocumentState(Global_asax.Identity.ID)
            roBaseState.SetSessionSmall(oState, Global_asax.Identity.ID, Global_asax.ApplicationSource, "")
            If Global_asax.Supervisor IsNot Nothing Then
                lrret = VTPortal.DocumentsHelper.GetDocumentBytes(documentId, -1, oState)
            Else
                lrret = VTPortal.DocumentsHelper.GetDocumentBytes(documentId, Global_asax.Identity.IDEmployee, oState)
            End If
        Catch ex As Exception
            lrret.Status = ErrorCodes.GENERAL_ERROR
            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::PortalSvcx::GetDocumentBytes")
        End Try

        Return lrret
    End Function

    ''' <summary>
    ''' Se requiere que la sesión este iniciada en el sistema. Esta función responde a un método POST del servidor.
    ''' Realiza una petición de subida de documento al gestor documental de VisualTime.
    ''' </summary>
    ''' <returns>Obtenemos un objeto del tipo StdResponse indicando si se ha podido subir el documento correctamente.</returns>
    <OperationContract()>
    <WebInvoke(Method:="POST", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function UploadDocument() As StdResponse
        Dim lrret As New StdResponse()
        If Not Global_asax.LoggedIn Then
            lrret.Status = VTPortal.StatusHelper.GetStatusFromState(Global_asax.SecurityResult)
            If lrret.Status = ErrorCodes.OK Then lrret.Status = ErrorCodes.NO_SESSION
            Return lrret
        End If

        lrret.Status = ErrorCodes.OK

        Try
            Dim curFile As HttpPostedFile = HttpContext.Current.Request.Files("userfile")
            If curFile Is Nothing Then curFile = HttpContext.Current.Request.Files("file")

            Dim maxFileSize As Integer = roTypes.Any2Integer(New AdvancedParameter.roAdvancedParameter("VTLive.MaxAllowedFileSize", New AdvancedParameter.roAdvancedParameterState(Global_asax.Identity.ID)).Value)
            If maxFileSize = 0 Then maxFileSize = 256

            If curFile Is Nothing OrElse (curFile.ContentLength <= (maxFileSize * 1024)) Then
                Dim idRelatedObject As Long = roTypes.Any2Integer(HttpContext.Current.Request.Form("idRelatedObject"))
                Dim idDocumentTemplate As Long = roTypes.Any2Integer(HttpContext.Current.Request.Form("idTemplateDocument"))
                Dim remarks As String = roTypes.Any2String(HttpContext.Current.Request.Form("remarks"))
                Dim docRelatedInfo As String = roTypes.Any2String(HttpContext.Current.Request.Form("docRelatedInfo"))
                Dim forecastType As String = roTypes.Any2String(HttpContext.Current.Request.Form("forecastType"))

                Dim oDocState As New VTDocuments.roDocumentState(-1)
                roBaseState.SetSessionSmall(oDocState, Global_asax.Identity.ID, Global_asax.ApplicationSource, "")

                Dim eForecastType As ForecastType = DTOs.ForecastType.AnyAbsence
                Select Case forecastType
                    Case "days", "leave"
                        eForecastType = DTOs.ForecastType.AbsenceDays
                    Case "hours"
                        eForecastType = DTOs.ForecastType.AbsenceHours
                    Case "overtime"
                        eForecastType = DTOs.ForecastType.OverWork

                End Select
                If forecastType = "request" Then
                    lrret = VTPortal.DocumentsHelper.UploadDocument(Global_asax.Identity, curFile, remarks, idDocumentTemplate, idRelatedObject, docRelatedInfo, eForecastType, oDocState, idRelatedObject)
                Else
                    lrret = VTPortal.DocumentsHelper.UploadDocument(Global_asax.Identity, curFile, remarks, idDocumentTemplate, idRelatedObject, docRelatedInfo, eForecastType, oDocState)
                End If
            Else
                lrret.Result = False
                lrret.Status = ErrorCodes.NO_DOCUMENT_ATTACHED

                Dim oReqLang As New roLanguage()
                oReqLang.SetLanguageReference("RequestService", Global_asax.Identity.Language.Key)
                lrret.CustomErrorText = oReqLang.Translate("MaxFileSizeExceeded.Error", "") & " (" & maxFileSize & "Kb)"
            End If
        Catch ex As Exception
            lrret.Status = ErrorCodes.GENERAL_ERROR
            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::PortalSvcx::UploadDocumentDesktop")
        End Try

        Return lrret
    End Function

    ''' <summary>
    ''' Se requiere que la sesión este iniciada en el sistema. Esta función responde a un método POST del servidor.
    ''' Consulta los campos de la ficha necesarios para realizar dicha acción sobre la tarea.
    ''' Esta función recibe los mismos parametros que en la petición equivalente sin fotografia con un objeto imagen en la propiedad Request.Files
    ''' </summary>
    ''' <param name="action">Acción que queremos realizar sobre la tarea 1-Iniciar, 2-Cambiar, 3-Completar</param>
    ''' <param name="taskId">Identificador de la tarea sobre la que realizar la consulta</param>
    ''' <returns>Obtenemos un objeto del tipo TaskUserFields, indicando que campos de la ficha son necearios.</returns>
    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function GetTaskUserFieldsAction(ByVal action As Integer, ByVal taskId As Integer) As TaskUserFields
        Dim lrret As New TaskUserFields
        If Not Global_asax.LoggedIn Then
            lrret.Status = VTPortal.StatusHelper.GetStatusFromState(Global_asax.SecurityResult)
            If lrret.Status = ErrorCodes.OK Then lrret.Status = ErrorCodes.NO_SESSION
            Return lrret
        End If

        lrret.Status = ErrorCodes.OK

        Try
            Dim oTaskState As New Task.roTaskState(-1)
            roBaseState.SetSessionSmall(oTaskState, Global_asax.Identity.ID, Global_asax.ApplicationSource, "")

            lrret = VTPortal.TaskHelper.GetTaskUserFieldsAction(action, taskId, oTaskState)
        Catch ex As Exception
            lrret.Status = ErrorCodes.GENERAL_ERROR
            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::PortalSvcx::GetTaskUserFieldsAction")
        End Try

        Return lrret
    End Function

    ''' <summary>
    ''' Se requiere que la sesión este iniciada en el sistema. Esta función responde a un método POST del servidor.
    ''' Añade una alerta sobre la tarea especificada para que un supervisor pueda atenderla.
    ''' Esta función recibe los mismos parametros que en la petición equivalente sin fotografia con un objeto imagen en la propiedad Request.Files
    ''' </summary>
    ''' <param name="taskId">Identificador de la tarea sobre la que realizar la consulta</param>
    ''' <param name="taskTextAlert">Texto de la alerta que queremos grabar en la tarea especificada</param>
    ''' <returns>Obtenemos un objeto del tipo StdResponse, indicando si la alerta se ha podido guardar la alerta en el sistema.</returns>
    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function SaveNewTaskAlert(ByVal taskId As Integer, ByVal taskTextAlert As String) As StdResponse
        Dim lrret As New StdResponse
        If Not Global_asax.LoggedIn Then
            lrret.Status = VTPortal.StatusHelper.GetStatusFromState(Global_asax.SecurityResult)
            If lrret.Status = ErrorCodes.OK Then lrret.Status = ErrorCodes.NO_SESSION
            Return lrret
        End If

        lrret.Status = ErrorCodes.OK

        Try

            Dim oTaskState As New Task.roTaskState(-1)
            roBaseState.SetSessionSmall(oTaskState, Global_asax.Identity.ID, Global_asax.ApplicationSource, "")
            lrret = VTPortal.TaskHelper.SaveNewTaskAlert(Global_asax.Identity.IDEmployee, taskId, taskTextAlert, Global_asax.TimeZone, oTaskState)
        Catch ex As Exception
            lrret.Status = ErrorCodes.GENERAL_ERROR
            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::PortalSvcx::GetTaskUserFieldsAction")
        End Try

        Return lrret
    End Function

    <OperationContract()>
    <WebInvoke(Method:="POST", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function SaveRequestExternalWorkWeekResume() As StdRequestResponse
        Dim lrret As New StdRequestResponse

        If Not Global_asax.LoggedIn Then
            lrret.Status = VTPortal.StatusHelper.GetStatusFromState(Global_asax.SecurityResult)
            If lrret.Status = ErrorCodes.OK Then lrret.Status = ErrorCodes.NO_SESSION
            Return lrret
        End If

        Try
            Dim oReqState As New Requests.roRequestState
            roBaseState.SetSessionSmall(oReqState, Global_asax.Identity.ID, Global_asax.ApplicationSource, "")

            Dim resumeInfo As String = HttpContext.Current.Request.Params("resumeInfo")
            Dim comments As String = HttpContext.Current.Request.Params("comments")
            Dim acceptWarning As Boolean = roTypes.Any2Boolean(HttpContext.Current.Request.Params("acceptWarning"))

            Dim oResumeInfo As ExternalWorkResume() = roJSONHelper.DeserializeNewtonSoft(resumeInfo, GetType(ExternalWorkResume()))

            Dim oLng As New roLanguage()
            oLng.SetLanguageReference("LiveOne", Global_asax.Identity.Language.Key)

            lrret = VTPortal.RequestsHelper.SaveRequest(eRequestType.ExternalWorkWeekResume, Global_asax.Identity, Global_asax.Identity.IDEmployee, "", "", "", "", "", -1, -1, "", comments, "", False, "", "", "", "", "", "", "", "", False, "", False, oResumeInfo, oLng, Global_asax.TimeZone, oReqState, acceptWarning, Global_asax.Supervisor)
        Catch ex As Exception
            lrret.Status = ErrorCodes.GENERAL_ERROR
            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::PortalSvcx::SaveRequest_ForbiddenCostCenterPunch")
        End Try

        Return lrret
    End Function

    ''' <summary>
    ''' Se requiere que la sesión este iniciada en el sistema. Esta función responde a un método GET del servidor.
    ''' Obtenemos información sobre la justificación solicitada. Si requiere documentación y que documentación en caso afirmativo
    ''' </summary>
    ''' <param name="idCause">Identificador de la justificación sobre la quer requerimos más información</param>
    ''' <param name="isStarting">Indica si estamos solicitando la documentación de inicio de baja o se seguimiento/alta </param>
    ''' <returns>Obtenemos un objeto estandar de respuesta de petición indicando si se ha realizado la petición y en caso contrario el código de error obtenido.</returns>
    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function GetRequieredLeaveDocuments(ByVal idCause As Integer, ByVal isStarting As Boolean) As GenericList
        Dim lrret As New GenericList

        If Not Global_asax.LoggedIn Then
            lrret.Status = VTPortal.StatusHelper.GetStatusFromState(Global_asax.SecurityResult)
            If lrret.Status = ErrorCodes.OK Then lrret.Status = ErrorCodes.NO_SESSION
            Return lrret
        End If

        Try
            Dim oDocState As New VTDocuments.roDocumentState(-1)
            roBaseState.SetSessionSmall(oDocState, Global_asax.Identity.ID, Global_asax.ApplicationSource, "")
            lrret = VTPortal.DocumentsHelper.GetLeaveTemplates(Global_asax.Identity.IDEmployee, idCause, isStarting, oDocState)
        Catch ex As Exception
            lrret.Status = ErrorCodes.GENERAL_ERROR
            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::PortalSvcx::SaveRequest_ForbiddenCostCenterPunch")
        End Try

        Return lrret
    End Function

    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function GetAvailableDocumentTemplateType(ByVal idEmployee As Integer) As GenericList
        Dim lrret As New GenericList

        If Not Global_asax.LoggedIn Then
            lrret.Status = VTPortal.StatusHelper.GetStatusFromState(Global_asax.SecurityResult)
            If lrret.Status = ErrorCodes.OK Then lrret.Status = ErrorCodes.NO_SESSION
            Return lrret
        End If

        Try
            Dim oDocState As New VTDocuments.roDocumentState(-1)
            roBaseState.SetSessionSmall(oDocState, Global_asax.Identity.ID, Global_asax.ApplicationSource, "")
            lrret = VTPortal.DocumentsHelper.GetAvailableDocumentTemplateType(Global_asax.Identity.IDEmployee, oDocState)
        Catch ex As Exception
            lrret.Status = ErrorCodes.GENERAL_ERROR
            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::PortalSvcx::GetAvailableDocumentTemplateType")
        End Try

        Return lrret
    End Function

    ''' <summary>
    ''' Se requiere que la sesión este iniciada en el sistema. Esta función responde a un método GET del servidor.
    ''' Obtenemos información sobre la justificación solicitada. Si requiere documentación y que documentación en caso afirmativo
    ''' </summary>
    ''' <param name="idCause">Identificador de la justificación sobre la quer requerimos más información</param>
    ''' <param name="isStarting">Indica si estamos solicitando la documentación de inicio de baja o se seguimiento/alta </param>
    ''' <returns>Obtenemos un objeto estandar de respuesta de petición indicando si se ha realizado la petición y en caso contrario el código de error obtenido.</returns>
    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function GetRequieredCauseDocuments(ByVal idCause As Integer) As GenericList
        Dim lrret As New GenericList

        If Not Global_asax.LoggedIn Then
            lrret.Status = VTPortal.StatusHelper.GetStatusFromState(Global_asax.SecurityResult)
            If lrret.Status = ErrorCodes.OK Then lrret.Status = ErrorCodes.NO_SESSION
            Return lrret
        End If

        Try
            Dim oDocState As New VTDocuments.roDocumentState(-1)
            roBaseState.SetSessionSmall(oDocState, Global_asax.Identity.ID, Global_asax.ApplicationSource, "")
            lrret = VTPortal.DocumentsHelper.GetCauseTemplates(Global_asax.Identity.IDEmployee, idCause, oDocState)
        Catch ex As Exception
            lrret.Status = ErrorCodes.GENERAL_ERROR
            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::PortalSvcx::SaveRequest_ForbiddenCostCenterPunch")
        End Try

        Return lrret
    End Function

    ''' <summary>
    ''' Se requiere que la sesión este iniciada en el sistema. Esta función responde a un método POST del servidor.
    ''' Realiza una petición de subida de documento al gestor documental de VisualTime.
    ''' Esta función recibe los mismos parametros que en la petición equivalente sin fotografia con un objeto imagen en la propiedad Request.Files
    ''' </summary>
    ''' <returns>Obtenemos un objeto del tipo StdResponse indicando si se ha podido subir el documento correctamente.</returns>
    <OperationContract()>
    <WebInvoke(Method:="POST", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function SaveLeave() As StdResponse
        Dim lrret As New StdResponse()
        If Not Global_asax.LoggedIn Then
            lrret.Status = VTPortal.StatusHelper.GetStatusFromState(Global_asax.SecurityResult)
            If lrret.Status = ErrorCodes.OK Then lrret.Status = ErrorCodes.NO_SESSION
            Return lrret
        End If

        lrret.Status = ErrorCodes.OK

        Try
            Dim curFile As HttpPostedFile = HttpContext.Current.Request.Files("userfile")
            If curFile Is Nothing Then curFile = HttpContext.Current.Request.Files("file")

            Dim maxFileSize As Integer = roTypes.Any2Integer(New AdvancedParameter.roAdvancedParameter("VTLive.MaxAllowedFileSize", New AdvancedParameter.roAdvancedParameterState(Global_asax.Identity.ID)).Value)
            If maxFileSize = 0 Then maxFileSize = 256

            If curFile Is Nothing OrElse (curFile.ContentLength <= (maxFileSize * 1024)) Then
                Dim remarks As String = roTypes.Any2String(HttpContext.Current.Request.Form("remarks"))
                Dim idDocumentTemplate As Long = roTypes.Any2Integer(HttpContext.Current.Request.Form("idTemplateDocument"))
                Dim strDateFrom As String = roTypes.Any2String(HttpContext.Current.Request.Form("from"))
                Dim strDateTo As String = roTypes.Any2String(HttpContext.Current.Request.Form("to"))
                Dim idCause As Long = roTypes.Any2Integer(HttpContext.Current.Request.Form("idCause"))

                Dim oReqState As New Requests.roRequestState
                roBaseState.SetSessionSmall(oReqState, Global_asax.Identity.ID, Global_asax.ApplicationSource, "")

                lrret = VTPortal.LeavesHelper.SaveLeave(Global_asax.Identity, strDateFrom, strDateTo, idCause, idDocumentTemplate, curFile, remarks, oReqState)
            Else
                lrret.Result = False
                lrret.Status = ErrorCodes.NO_DOCUMENT_ATTACHED

                Dim oReqLang As New roLanguage()
                oReqLang.SetLanguageReference("RequestService", Global_asax.Identity.Language.Key)
                lrret.CustomErrorText = oReqLang.Translate("MaxFileSizeExceeded.Error", "") & " (" & maxFileSize & "Kb)"
            End If
        Catch ex As Exception
            lrret.Status = ErrorCodes.GENERAL_ERROR
            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::PortalSvcx::SaveLeave")
        End Try

        Return lrret
    End Function

    ''' <summary>
    ''' Se requiere que la sesión este iniciada en el sistema. Esta función responde a un método POST del servidor.
    ''' Actualiza la fecha de finalización de una ausencia.
    ''' </summary>
    ''' <returns>Obtenemos un objeto del tipo StdResponse indicando si se ha podido subir el documento correctamente.</returns>
    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function SetLeaveEndDate(ByVal leaveId As Integer, ByVal documentTemplate As Integer, Optional ByVal endDate As String = Nothing) As StdResponse
        Dim lrret As New StdResponse()
        If Not Global_asax.LoggedIn Then
            lrret.Status = VTPortal.StatusHelper.GetStatusFromState(Global_asax.SecurityResult)
            If lrret.Status = ErrorCodes.OK Then lrret.Status = ErrorCodes.NO_SESSION
            Return lrret
        End If

        lrret.Status = ErrorCodes.OK

        Try
            Dim oReqState As New Requests.roRequestState
            roBaseState.SetSessionSmall(oReqState, Global_asax.Identity.ID, Global_asax.ApplicationSource, "")

            Dim oDocState As New VTDocuments.roDocumentState(-1)
            Dim oManager As New VTDocuments.roDocumentManager(oDocState)
            Dim docTemplate = oManager.LoadDocumentTemplate(documentTemplate)
            If docTemplate.Scope = DocumentScope.LeaveOrPermission AndAlso docTemplate.LeaveDocumentType = LeaveDocumentType.ReturnReport Then
                lrret = VTPortal.LeavesHelper.UploadLeaveFinishDate(leaveId, roTypes.Any2DateTime(endDate), oReqState)
            Else
                lrret.Result = False
                lrret.Status = ErrorCodes.NO_DOCUMENT_ATTACHED

                Dim oReqLang As New roLanguage()
            End If
        Catch ex As Exception
            lrret.Status = ErrorCodes.GENERAL_ERROR
            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::PortalSvcx::SaveLeave")
        End Try

        Return lrret
    End Function

    ''' <summary>
    ''' Se requiere que la sesión este iniciada en el sistema. Esta función responde a un método POST del servidor.
    ''' Realiza una petición del listado de bajas solicitadas a VisualTime.
    ''' </summary>
    ''' <returns>Obtenemos un objeto del tipo StdResponse indicando si se ha podido subir el documento correctamente.</returns>
    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function GetMyLeaves(ByVal showAll As Boolean, ByVal dateStart As String, ByVal dateEnd As String) As LeavesList
        Dim lrret As New LeavesList
        If Not Global_asax.LoggedIn Then
            lrret.Status = VTPortal.StatusHelper.GetStatusFromState(Global_asax.SecurityResult)
            If lrret.Status = ErrorCodes.OK Then lrret.Status = ErrorCodes.NO_SESSION
            Return lrret
        End If

        lrret.Status = ErrorCodes.OK

        Try
            Dim oAbsState As Absence.roProgrammedAbsenceState = New Absence.roProgrammedAbsenceState(Global_asax.Identity.ID)
            roBaseState.SetSessionSmall(oAbsState, Global_asax.Identity.ID, Global_asax.ApplicationSource, "")

            Dim sDate As Date
            If dateStart <> "" Then
                Dim strDate As String
                Dim strTime As String = "00:00"
                Dim strArrDate() As String = dateStart.Split(" ")
                strDate = strArrDate(0)
                If strArrDate.Length = 2 Then strTime = dateStart.Split(" ")(1)
                sDate = New Date(strDate.Split("-")(0), strDate.Split("-")(1), strDate.Split("-")(2), strTime.Split(":")(0), strTime.Split(":")(1), 0)
            End If

            Dim eDate As Date
            If dateEnd <> "" Then
                Dim strDate As String
                Dim strTime As String = "00:00"
                Dim strArrDate() As String = dateEnd.Split(" ")
                strDate = strArrDate(0)
                If strArrDate.Length = 2 Then strTime = dateEnd.Split(" ")(1)
                eDate = New Date(strDate.Split("-")(0), strDate.Split("-")(1), strDate.Split("-")(2), strTime.Split(":")(0), strTime.Split(":")(1), 0)
            End If

            lrret = VTPortal.LeavesHelper.GetEmployeeLeaves(Global_asax.Identity, showAll, sDate, eDate, oAbsState)
        Catch ex As Exception
            lrret.Status = ErrorCodes.GENERAL_ERROR
            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::PortalSvcx::GetMyLeaves")
        End Try

        Return lrret
    End Function

    ''' <summary>
    ''' Se requiere que la sesión este iniciada en el sistema. Esta función responde a un método POST del servidor.
    ''' Realiza una petición de subida de imagen de perfil del empleado.
    ''' </summary>
    ''' <returns>Obtenemos un objeto del tipo StdResponse indicando si se ha podido subir el documento correctamente.</returns>
    <OperationContract()>
    <WebInvoke(Method:="POST", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function SaveProfileImage() As StdResponse
        Dim lrret As New StdResponse()
        If Not Global_asax.LoggedIn Then
            lrret.Status = VTPortal.StatusHelper.GetStatusFromState(Global_asax.SecurityResult)
            If lrret.Status = ErrorCodes.OK Then lrret.Status = ErrorCodes.NO_SESSION
            Return lrret
        End If

        lrret.Status = ErrorCodes.OK

        Try
            Dim curFile As HttpPostedFile = HttpContext.Current.Request.Files("userfile")
            If curFile Is Nothing Then curFile = HttpContext.Current.Request.Files("file")

            lrret = VTPortal.StatusHelper.UploadUserPhoto(Global_asax.Identity, curFile)
        Catch ex As Exception
            lrret.Status = ErrorCodes.GENERAL_ERROR
            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::PortalSvcx::SaveProfileImage")
        End Try

        Return lrret
    End Function

    ''' <summary>
    ''' Se requiere que la sesión este iniciada en el sistema. Esta función responde a un método GET del servidor.
    ''' Realiza una petición para obtener el estado del día (saldos, ausencias, etc...)
    ''' </summary>
    ''' <returns>Obtenemos un objeto del tipo EmployeeDayInfo toda la información solicitada.</returns>
    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function GetEmployeeDayInfo(ByVal strDayDate As String, ByVal idEmployee As Integer) As EmployeeDayInfo
        Dim lrret As New EmployeeDayInfo()
        Dim idEmployeeInfo As Integer
        If Not Global_asax.LoggedIn Then
            lrret.Status = VTPortal.StatusHelper.GetStatusFromState(Global_asax.SecurityResult)
            If lrret.Status = ErrorCodes.OK Then lrret.Status = ErrorCodes.NO_SESSION
            Return lrret
        End If

        lrret.Status = ErrorCodes.OK

        Try
            If idEmployee > 0 Then
                idEmployeeInfo = idEmployee
            Else
                idEmployeeInfo = Global_asax.Identity.IDEmployee
            End If
            Dim selectedDate As DateTime = DateTime.ParseExact(strDayDate, "dd/MM/yyyy", Nothing)
            Dim oCalState As New Robotics.Base.VTCalendar.roCalendarRowPeriodDataState(Global_asax.Identity.ID)
            roBaseState.SetSessionSmall(oCalState, Global_asax.Identity.ID, Global_asax.ApplicationSource, "")

            lrret = VTPortal.ShiftsHelper.GetEmployeeDayInfo(Global_asax.Identity, idEmployeeInfo, selectedDate, oCalState)
        Catch ex As Exception
            lrret.Status = ErrorCodes.GENERAL_ERROR
            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::PortalSvcx::GetEmployeeDayInfo")
        End Try

        Return lrret
    End Function

    ''' <summary>
    ''' Se requiere que la sesión este iniciada en el sistema. Esta función responde a un método GET del servidor.
    ''' Realiza una petición de consulta de todos los empleados supervisados.
    ''' </summary>
    ''' <returns>Obtenemos un objeto del tipo EmployeeList con el listado completo de los empleados que supervisamos.</returns>
    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function GetMyEmployeesWithoutStatus() As EmployeeList

        Dim lrret As New EmployeeList()
        If Not Global_asax.LoggedIn Then
            lrret.Status = VTPortal.StatusHelper.GetStatusFromState(Global_asax.SecurityResult)
            If lrret.Status = ErrorCodes.OK Then lrret.Status = ErrorCodes.NO_SESSION
            Return lrret
        End If

        lrret.Status = ErrorCodes.OK

        Try
            If Not Global_asax.IsSupervisor Then
                lrret.Status = ErrorCodes.GENERAL_ERROR_NoPermissions
                lrret.Employees = {}
            Else

                Dim oEmpState As New Employee.roEmployeeState(Global_asax.Identity.ID)
                roBaseState.SetSessionSmall(oEmpState, Global_asax.Identity.ID, Global_asax.ApplicationSource, "")

                lrret = VTPortal.EmployeesHelper.GetSupervisedEmployees(Global_asax.Identity, oEmpState)
            End If
        Catch ex As Exception
            lrret.Status = ErrorCodes.GENERAL_ERROR
            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::PortalSvcx::GetMyEmployeesWithoutStatus")
        End Try

        Return lrret
    End Function

    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function GetOnBoardingTasksById(ByVal idOnboarding) As OnBoardingTasks

        Dim lrret As New OnBoardingTasks
        If Not Global_asax.LoggedIn Then
            lrret.Status = VTPortal.StatusHelper.GetStatusFromState(Global_asax.SecurityResult)
            If lrret.Status = ErrorCodes.OK Then lrret.Status = ErrorCodes.NO_SESSION
            Return lrret
        End If

        lrret.Status = ErrorCodes.OK
        Try
            If Not Global_asax.IsSupervisor Then
                lrret.Status = ErrorCodes.GENERAL_ERROR_NoPermissions
                lrret.Tasks = {}
            Else

                Dim oPassport As roPassportTicket = Nothing
                If Global_asax.Supervisor IsNot Nothing AndAlso Global_asax.Identity IsNot Nothing Then
                    oPassport = Global_asax.Supervisor
                Else
                    oPassport = Global_asax.Identity
                End If

                Dim oPortalLang As New roLanguage()
                oPortalLang.SetLanguageReference("VTPortal", oPassport.Language.Key)

                Dim oEmpState As New Employee.roEmployeeState(oPassport.ID)
                roBaseState.SetSessionSmall(oEmpState, oPassport.ID, Global_asax.ApplicationSource, "")

                Dim oToDoState As New roToDoListState(oPassport.ID)
                lrret = VTPortal.ToDoListHelper.GetOnBoardingTasksById(oPassport, idOnboarding, oToDoState)

            End If
        Catch ex As Exception
            lrret.Status = ErrorCodes.GENERAL_ERROR
            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::PortalSvcx::GetMyEmployees")
        End Try

        Return lrret
    End Function

    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function UpdateTaskStatus(ByVal status, ByVal idTask, ByVal idList) As OnBoardingTasks

        Dim lrret As New OnBoardingTasks
        If Not Global_asax.LoggedIn Then
            lrret.Status = VTPortal.StatusHelper.GetStatusFromState(Global_asax.SecurityResult)
            If lrret.Status = ErrorCodes.OK Then lrret.Status = ErrorCodes.NO_SESSION
            Return lrret
        End If

        lrret.Status = ErrorCodes.OK
        Try
            If Not Global_asax.IsSupervisor Then
                lrret.Status = ErrorCodes.GENERAL_ERROR_NoPermissions
                lrret.Tasks = {}
            Else

                Dim oPassport As roPassportTicket = Nothing
                If Global_asax.Supervisor IsNot Nothing AndAlso Global_asax.Identity IsNot Nothing Then
                    oPassport = Global_asax.Supervisor
                Else
                    oPassport = Global_asax.Identity
                End If

                Dim oPortalLang As New roLanguage()
                oPortalLang.SetLanguageReference("VTPortal", oPassport.Language.Key)

                Dim oEmpState As New Employee.roEmployeeState(oPassport.ID)
                roBaseState.SetSessionSmall(oEmpState, oPassport.ID, Global_asax.ApplicationSource, "")

                Dim oToDoState As New roToDoListState(oPassport.ID)
                Dim savedTasks = VTPortal.ToDoListHelper.UpdateTaskStatus(oPassport, status, idTask, oToDoState)
                If savedTasks = True Then
                    lrret = VTPortal.ToDoListHelper.GetOnBoardingTasksById(oPassport, idList, oToDoState)
                Else
                    lrret.Status = ErrorCodes.GENERAL_ERROR
                End If

            End If
        Catch ex As Exception
            lrret.Status = ErrorCodes.GENERAL_ERROR
            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::PortalSvcx::GetMyEmployees")
        End Try

        Return lrret
    End Function

    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function GetMyOnBoardings() As OnBoardingList

        Dim lrret As New OnBoardingList
        If Not Global_asax.LoggedIn Then
            lrret.Status = VTPortal.StatusHelper.GetStatusFromState(Global_asax.SecurityResult)
            If lrret.Status = ErrorCodes.OK Then lrret.Status = ErrorCodes.NO_SESSION
            Return lrret
        End If

        lrret.Status = ErrorCodes.OK
        Try
            If Not Global_asax.IsSupervisor Then
                lrret.Status = ErrorCodes.GENERAL_ERROR_NoPermissions
                lrret.OnBoardings = {}
            Else

                Dim oPassport As roPassportTicket = Nothing
                If Global_asax.Supervisor IsNot Nothing AndAlso Global_asax.Identity IsNot Nothing Then
                    oPassport = Global_asax.Supervisor
                Else
                    oPassport = Global_asax.Identity
                End If

                Dim oPortalLang As New roLanguage()
                oPortalLang.SetLanguageReference("VTPortal", oPassport.Language.Key)

                Dim oEmpState As New Employee.roEmployeeState(oPassport.ID)
                roBaseState.SetSessionSmall(oEmpState, oPassport.ID, Global_asax.ApplicationSource, "")

                Dim oToDoState As New roToDoListState(oPassport.ID)
                lrret = VTPortal.ToDoListHelper.GetAllToDoListsByType(oPassport, ToDoListType.OnBoarding, oToDoState)

            End If
        Catch ex As Exception
            lrret.Status = ErrorCodes.GENERAL_ERROR
            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::PortalSvcx::GetMyEmployees")
        End Try

        Return lrret
    End Function

    ''' <summary>
    ''' Se requiere que la sesión este iniciada en el sistema. Esta función responde a un método GET del servidor.
    ''' Realiza una petición de consulta de todos los empleados supervisados.
    ''' </summary>
    ''' <returns>Obtenemos un objeto del tipo EmployeeList con el listado completo de los empleados que supervisamos.</returns>
    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function GetMyEmployees() As EmployeeList

        Dim lrret As New EmployeeList()
        If Not Global_asax.LoggedIn Then
            lrret.Status = VTPortal.StatusHelper.GetStatusFromState(Global_asax.SecurityResult)
            If lrret.Status = ErrorCodes.OK Then lrret.Status = ErrorCodes.NO_SESSION
            Return lrret
        End If

        lrret.Status = ErrorCodes.OK

        Try
            If Not Global_asax.IsSupervisor Then
                lrret.Status = ErrorCodes.GENERAL_ERROR_NoPermissions
                lrret.Employees = {}
            Else

                Dim oPassport As roPassportTicket = Nothing
                If Global_asax.Supervisor IsNot Nothing AndAlso Global_asax.Identity IsNot Nothing Then
                    oPassport = Global_asax.Supervisor
                Else
                    oPassport = Global_asax.Identity
                End If

                Dim oPortalLang As New roLanguage()
                oPortalLang.SetLanguageReference("VTPortal", oPassport.Language.Key)

                Dim oEmpState As New Employee.roEmployeeState(oPassport.ID)
                roBaseState.SetSessionSmall(oEmpState, oPassport.ID, Global_asax.ApplicationSource, "")

                ' lrret = VTPortal.EmployeesHelper.GetSupervisedEmployees(oPassport, oEmpState)

                Dim fileName As String = Path.Combine(HostingEnvironment.ApplicationPhysicalPath, "Resources/userDefault.png")
                Dim fileStream As New FileStream(fileName, FileMode.Open, FileAccess.Read)

                Dim ImageData As Byte()
                ImageData = New Byte(fileStream.Length - 1) {}
                fileStream.Read(ImageData, 0, System.Convert.ToInt32(fileStream.Length))
                fileStream.Close()

                lrret.DefaultImage = "url(data:image/png;base64," & Convert.ToBase64String(ImageData) & ") no-repeat center center"

                Dim employeesStatus = VTPortal.StatusHelper.GetEmployeesCurrentStatus(oPassport.ID)
                If employeesStatus.Rows.Count > 0 Then

                    Dim empsList As New Generic.List(Of EmployeeInfo)
                    Dim oEmployeeDashboardInfo As EmployeeInfo

                    For Each row In employeesStatus.Rows
                        oEmployeeDashboardInfo = New EmployeeInfo
                        oEmployeeDashboardInfo.EmployeeId = roTypes.Any2String(row("IdEmployee"))
                        oEmployeeDashboardInfo.Name = roTypes.Any2String(row("EmployeeName"))
                        If row("AttStatus") = "In" Then
                            oEmployeeDashboardInfo.PresenceStatus = "Inside"
                        Else
                            oEmployeeDashboardInfo.PresenceStatus = "Outside"
                        End If
                        oEmployeeDashboardInfo.Image = VTPortal.EmployeesHelper.LoadEmployeeImage(row("EmployeeImage"), oEmpState)
                        oEmployeeDashboardInfo.TaskTitle = If(IsDBNull(row("TskName")), "", row("TskName"))
                        oEmployeeDashboardInfo.InTelecommute = If(IsDBNull(row("InTelecommute")), "", row("InTelecommute"))

                        oEmployeeDashboardInfo.BeginDate = Date.Now
                        oEmployeeDashboardInfo.CostCenterName = If(IsDBNull(row("CostCenterName")), "", row("CostCenterName"))
                        empsList.Add(oEmployeeDashboardInfo)
                    Next

                    lrret.Employees = empsList.ToArray

                End If
            End If
        Catch ex As Exception
            lrret.Status = ErrorCodes.GENERAL_ERROR
            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::PortalSvcx::GetMyEmployees")
        End Try

        Return lrret
    End Function

    ''' <summary>
    ''' Se requiere que la sesión este iniciada en el sistema. Esta función responde a un método GET del servidor.
    ''' Realiza una petición para aprobar o denegar una petición de solicitud.
    ''' </summary>
    ''' <returns>Obtenemos un objeto del tipo StdRequestResponse indicando si se ha podido aprobar/rechar/requiere confirmación sobre una solicitud.</returns>
    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function ApproveRefuseRequest(ByVal idRequest As Integer, ByVal bApprove As Boolean, ByVal bForceApprove As Boolean) As StdRequestResponse

        Dim lrret As New StdRequestResponse()
        If Not Global_asax.LoggedIn Then
            lrret.Status = VTPortal.StatusHelper.GetStatusFromState(Global_asax.SecurityResult)
            If lrret.Status = ErrorCodes.OK Then lrret.Status = ErrorCodes.NO_SESSION
            Return lrret
        End If

        lrret.Status = ErrorCodes.OK
        Try
            If Not Global_asax.IsSupervisor OrElse Global_asax.Supervisor Is Nothing Then
                lrret.Status = ErrorCodes.GENERAL_ERROR_NoPermissions
                lrret.Result = False
            Else
                Dim oReqState As New Requests.roRequestState
                roBaseState.SetSessionSmall(oReqState, Global_asax.Supervisor.ID, Global_asax.ApplicationSource, "")

                Dim oLng As New roLanguage()
                oLng.SetLanguageReference("LiveOne", Global_asax.Identity.Language.Key)

                lrret = VTPortal.RequestsHelper.ApproveRefuseRequest(New Requests.roRequest(idRequest, oReqState), Global_asax.Supervisor, oReqState, oLng, bApprove, bForceApprove)
            End If
        Catch ex As Exception
            lrret.Status = ErrorCodes.GENERAL_ERROR
            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::PortalSvcx::GetMyEmployees")
        End Try

        Return lrret
    End Function

    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function ApproveRefuseMultipleDR(ByVal idRequests As String, ByVal action As String) As StdRequestResponse

        Dim lrret As New StdRequestResponse()
        If Not Global_asax.LoggedIn Then
            lrret.Status = VTPortal.StatusHelper.GetStatusFromState(Global_asax.SecurityResult)
            If lrret.Status = ErrorCodes.OK Then lrret.Status = ErrorCodes.NO_SESSION
            Return lrret
        End If

        Dim aRequests As New ArrayList
        aRequests = roJSONHelper.DeserializeNewtonSoft(idRequests, GetType(ArrayList))
        Dim bApprove As Boolean = False
        Dim bForceApprove As Boolean = False

        bApprove = (action.Trim = "approve")

        lrret.Status = ErrorCodes.OK
        Try
            If Not Global_asax.IsSupervisor OrElse Global_asax.Identity Is Nothing Then
                lrret.Status = ErrorCodes.GENERAL_ERROR_NoPermissions
                lrret.Result = False
            Else
                Dim oReqState As New Requests.roRequestState
                roBaseState.SetSessionSmall(oReqState, Global_asax.Identity.ID, Global_asax.ApplicationSource, "")

                Dim oLng As New roLanguage()
                oLng.SetLanguageReference("LiveOne", Global_asax.Identity.Language.Key)

                For Each idRequest As Integer In aRequests
                    lrret = VTPortal.RequestsHelper.ApproveRefuseRequest(New Requests.roRequest(idRequest, oReqState), Global_asax.Identity, oReqState, oLng, bApprove, bForceApprove)
                Next
            End If
        Catch ex As Exception
            lrret.Status = ErrorCodes.GENERAL_ERROR
            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::PortalSvcx::ApproveRefuseMultipleDR")
        End Try

        Return lrret
    End Function

    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function ApproveRefuseRequestNew(ByVal idRequest As Integer, ByVal bApprove As Boolean, ByVal bForceApprove As Boolean) As StdRequestResponse

        Dim lrret As New StdRequestResponse()
        If Not Global_asax.LoggedIn Then
            lrret.Status = VTPortal.StatusHelper.GetStatusFromState(Global_asax.SecurityResult)
            If lrret.Status = ErrorCodes.OK Then lrret.Status = ErrorCodes.NO_SESSION
            Return lrret
        End If

        lrret.Status = ErrorCodes.OK
        Try
            If Not Global_asax.IsSupervisor OrElse Global_asax.Identity Is Nothing Then
                lrret.Status = ErrorCodes.GENERAL_ERROR_NoPermissions
                lrret.Result = False
            Else
                Dim oReqState As New Requests.roRequestState
                roBaseState.SetSessionSmall(oReqState, Global_asax.Identity.ID, Global_asax.ApplicationSource, "")

                Dim oLng As New roLanguage()
                oLng.SetLanguageReference("LiveOne", Global_asax.Identity.Language.Key)

                lrret = VTPortal.RequestsHelper.ApproveRefuseRequest(New Requests.roRequest(idRequest, oReqState), Global_asax.Identity, oReqState, oLng, bApprove, bForceApprove)
            End If
        Catch ex As Exception
            lrret.Status = ErrorCodes.GENERAL_ERROR
            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::PortalSvcx::GetMyEmployees")
        End Try

        Return lrret
    End Function

    ''' <summary>
    ''' Se requiere que la sesión este iniciada en el sistema. Esta función responde a un método GET del servidor.
    ''' Realiza una petición para aprobar o denegar una petición de solicitud.
    ''' </summary>
    ''' <returns>Obtenemos un objeto del tipo StdRequestResponse indicando si se ha podido aprobar/rechar/requiere confirmación sobre una solicitud.</returns>
    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function ApproveRefuseRequestByEmployee(ByVal idRequest As Integer, ByVal bApprove As Boolean, ByVal bForceApprove As Boolean) As StdRequestResponse

        Dim lrret As New StdRequestResponse()
        If Not Global_asax.LoggedIn Then
            lrret.Status = VTPortal.StatusHelper.GetStatusFromState(Global_asax.SecurityResult)
            If lrret.Status = ErrorCodes.OK Then lrret.Status = ErrorCodes.NO_SESSION
            Return lrret
        End If

        lrret.Status = ErrorCodes.OK
        Try
            Dim oReqState As New Requests.roRequestState
            roBaseState.SetSessionSmall(oReqState, Global_asax.Identity.ID, Global_asax.ApplicationSource, "")

            Dim oLng As New roLanguage()
            oLng.SetLanguageReference("LiveOne", Global_asax.Identity.Language.Key)

            lrret = VTPortal.RequestsHelper.ApproveRefuseRequest(New Requests.roRequest(idRequest, oReqState), Global_asax.Identity, oReqState, oLng, bApprove, bForceApprove)
            'lrret = VTPortal.RequestsHelper.ApproveRefuseShiftExchange(Global_asax.Identity, idRequest, bApprove, oReqState, oLng)
        Catch ex As Exception
            lrret.Status = ErrorCodes.GENERAL_ERROR
            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::PortalSvcx::GetMyEmployees")
        End Try

        Return lrret
    End Function

    ''' <summary>
    ''' Se requiere que la sesión este iniciada en el sistema. Esta función responde a un método GET del servidor.
    ''' Realiza una petición de consulta de todos los empleados supervisados.
    ''' </summary>
    ''' <returns>Obtenemos un objeto del tipo StdResponse indicando si se ha podido subir el documento correctamente.</returns>
    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function GetSupervisorAlertsDetail(ByVal idAlertType As Integer) As SupervisorAlertsTypeDetail

        Dim lrret As New SupervisorAlertsTypeDetail()
        If Not Global_asax.LoggedIn Then
            lrret.Status = VTPortal.StatusHelper.GetStatusFromState(Global_asax.SecurityResult)
            If lrret.Status = ErrorCodes.OK Then lrret.Status = ErrorCodes.NO_SESSION
            Return lrret
        End If

        lrret.Status = ErrorCodes.OK
        Try
            If Not Global_asax.IsSupervisor Then
                lrret.Status = ErrorCodes.GENERAL_ERROR_NoPermissions
                lrret.Alerts = {}
            Else
                lrret = VTPortal.StatusHelper.LoadSupervisorAlertsByType(Global_asax.Identity, idAlertType)
            End If
        Catch ex As Exception
            lrret.Status = ErrorCodes.GENERAL_ERROR
            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::PortalSvcx::GetSupervisorAlertsDetail")
        End Try

        Return lrret
    End Function

    ''' <summary>
    ''' Se requiere que la sesión este iniciada en el sistema. Esta función responde a un método GET del servidor.
    ''' Consulta el listado de solicitudes completa del empleado que tinee la sesión iniciada.
    ''' </summary>
    ''' <param name="showAll">Indica si se deben mostrar todas las solicitudes. Valores aceptados: True/False</param>
    ''' <param name="dateStart">Cadena de texto que informa la fecha incial del filtro de solicitudes en formato 'dd/MM/yyyy'</param>
    ''' <param name="dateEnd">Cadena de texto que informa la fecha final del filtro de solicitudes en formato 'dd/MM/yyyy'</param>
    ''' <param name="filter">Cadena de texto que informa el filtro de solicitudes en el siguiente formato: 0*1*2*3|1*2*3*4*5*6*7*8*9*10*11*12
    '''     0 --> Solicitudes Pendientes
    '''     1 --> Solicitudes en curso
    '''     2 --> Solicitudes aceptadas
    '''     3 --> Solicitudes denegadas
    '''     |
    '''     1 --> Solicitud de campo de la ficha
    '''     2 --> Solicitud de fichaje no realizado
    '''     3 --> Solicitud de justificación de fichaje
    '''     4 --> Solicitud de trabajo externo
    '''     5 --> Solicitud de cambio de horario
    '''     6 --> Solicitud de vacaciones
    '''     7 --> Solicitud de ausencia por dias
    '''     8 --> ---
    '''     9 --> Solicitud de ausencia por horas
    '''     10 --> solicitud de fichaje de tareas no realizado
    '''     11 --> Solicitud de cancelación de vacaciones
    '''     12 --> Solcitud de fichaje de centro de costo no realizado
    ''' </param>
    ''' <param name="orderBy">Ordenación de las solicitudes con el siguiente formato: 'campo sentido'
    '''     campo:  RequestType: 'Tipo de solicitud'
    '''             RequestDate: 'Fecha de creación de la solicitud'
    '''             Status: 'Estado de la solicitud'
    '''             Date1: 'Fecha solicitada'
    '''     sentido: ASC: Ascendente
    '''              DESC: Descendente
    ''' </param>
    ''' <returns>Obtenemos un objeto del tipo RequestsList. Si se ha producido un error en la consulta la variable de esato del objeto informa de ello.</returns>
    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function GetSupervisedRequests(ByVal showAll As Boolean, ByVal dateStart As String, ByVal dateEnd As String, ByVal filter As String, ByVal orderBy As String, ByVal dateRequestedStart As String, ByVal dateRequestedEnd As String) As RequestsList
        Dim lrret As New RequestsList

        If Not Global_asax.LoggedIn Then
            lrret.Status = VTPortal.StatusHelper.GetStatusFromState(Global_asax.SecurityResult)
            If lrret.Status = ErrorCodes.OK Then lrret.Status = ErrorCodes.NO_SESSION
            Return lrret
        End If

        Try
            Dim oLang As New roLanguage()
            oLang.SetLanguageReference("LiveOne", Global_asax.Identity.Language.Key)
            Dim oReqLang As New roLanguage()
            oReqLang.SetLanguageReference("RequestService", Global_asax.Identity.Language.Key)
            Dim oPortalLang As New roLanguage()
            oPortalLang.SetLanguageReference("VTPortal", Global_asax.Identity.Language.Key)

            Dim oReqState As Requests.roRequestState = New Requests.roRequestState(Global_asax.Identity.ID)
            roBaseState.SetSessionSmall(oReqState, Global_asax.Identity.ID, Global_asax.ApplicationSource, "")

            Dim strOrderBy As String = orderBy

            Dim sDate As Date
            If dateStart <> "" Then
                Dim strDate As String
                Dim strTime As String = "00:00"
                Dim strArrDate() As String = dateStart.Split(" ")
                strDate = strArrDate(0)
                If strArrDate.Length = 2 Then strTime = dateStart.Split(" ")(1)
                sDate = New Date(strDate.Split("-")(0), strDate.Split("-")(1), strDate.Split("-")(2), strTime.Split(":")(0), strTime.Split(":")(1), 0)
            End If

            Dim eDate As Date
            If dateEnd <> "" Then
                Dim strDate As String
                Dim strTime As String = "00:00"
                Dim strArrDate() As String = dateEnd.Split(" ")
                strDate = strArrDate(0)
                If strArrDate.Length = 2 Then strTime = dateEnd.Split(" ")(1)
                eDate = New Date(strDate.Split("-")(0), strDate.Split("-")(1), strDate.Split("-")(2), strTime.Split(":")(0), strTime.Split(":")(1), 0)
            End If

            Dim sRequestedDate As Date
            If dateRequestedStart <> "" Then
                Dim strDate As String
                Dim strTime As String = "00:00"
                Dim strArrDate() As String = dateRequestedStart.Split(" ")
                strDate = strArrDate(0)
                If strArrDate.Length = 2 Then strTime = dateRequestedStart.Split(" ")(1)
                sRequestedDate = New Date(strDate.Split("-")(0), strDate.Split("-")(1), strDate.Split("-")(2), strTime.Split(":")(0), strTime.Split(":")(1), 0)
            End If

            Dim eRequestedDate As Date
            If dateRequestedEnd <> "" Then
                Dim strDate As String
                Dim strTime As String = "00:00"
                Dim strArrDate() As String = dateRequestedEnd.Split(" ")
                strDate = strArrDate(0)
                If strArrDate.Length = 2 Then strTime = dateRequestedEnd.Split(" ")(1)
                eRequestedDate = New Date(strDate.Split("-")(0), strDate.Split("-")(1), strDate.Split("-")(2), strTime.Split(":")(0), strTime.Split(":")(1), 0)
            End If

            lrret = VTPortal.RequestsHelper.GetSupervisedRequests(showAll, sDate, eDate, sRequestedDate, eRequestedDate, filter, orderBy, Global_asax.Identity.ID, oLang, oReqLang, oPortalLang, oReqState)

            Dim fileName As String = Path.Combine(HostingEnvironment.ApplicationPhysicalPath, "Resources/userDefault.png")
            Dim fileStream As New FileStream(fileName, FileMode.Open, FileAccess.Read)

            Dim ImageData As Byte()
            ImageData = New Byte(fileStream.Length - 1) {}
            fileStream.Read(ImageData, 0, System.Convert.ToInt32(fileStream.Length))
            fileStream.Close()

            lrret.DefaultImage = "url(data:image/png;base64," & Convert.ToBase64String(ImageData) & ") no-repeat center center"
        Catch ex As Exception
            lrret.Status = ErrorCodes.GENERAL_ERROR
            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::PortalSvcx::GetRequests")
        End Try

        Return lrret
    End Function

    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function GetAvailableEmployeesForDate(ByVal sDate As String, ByVal eDate As String, ByVal iSourceShift As Integer, ByVal iDestinationShift As Integer) As GenericList
        Dim lrret As New GenericList

        If Not Global_asax.LoggedIn Then
            lrret.Status = VTPortal.StatusHelper.GetStatusFromState(Global_asax.SecurityResult)
            If lrret.Status = ErrorCodes.OK Then lrret.Status = ErrorCodes.NO_SESSION
            Return lrret
        End If

        Try
            Dim oLng As New roLanguage()
            oLng.SetLanguageReference("VTPortal", Global_asax.Identity.Language.Key)

            Dim oState As New Employee.roEmployeeState(Global_asax.Identity.ID)
            roBaseState.SetSessionSmall(oState, Global_asax.Identity.ID, Global_asax.ApplicationSource, "")

            If Global_asax.Identity.IDEmployee IsNot Nothing Then
                lrret = VTPortal.CommonHelper.GetAvailableEmployeesForDate(Global_asax.Identity, sDate, eDate, iSourceShift, oLng, iDestinationShift, oState)
                If lrret.SelectFields Is Nothing OrElse lrret.SelectFields.Length = 0 Then
                    lrret = New GenericList() With {
                        .SelectFields = {},
                        .Status = ErrorCodes.OK
                    }
                End If
            Else
                lrret = New GenericList() With {
                    .SelectFields = {},
                    .Status = ErrorCodes.OK
                    }
            End If
        Catch ex As Exception
            lrret.Status = ErrorCodes.GENERAL_ERROR
            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::PortalSvcx::GetRequests")
        End Try

        Return lrret
    End Function

    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function GetDaysToCompensate(ByVal sDate As String, ByVal eDate As String, ByVal iExchangeEmployee As Integer) As DaysCount
        Dim lrret As New DaysCount

        If Not Global_asax.LoggedIn Then
            lrret.Status = VTPortal.StatusHelper.GetStatusFromState(Global_asax.SecurityResult)
            If lrret.Status = ErrorCodes.OK Then lrret.Status = ErrorCodes.NO_SESSION
            Return lrret
        End If

        Try
            Dim oLng As New roLanguage()
            oLng.SetLanguageReference("VTPortal", Global_asax.Identity.Language.Key)

            Dim oState As New Employee.roEmployeeState(Global_asax.Identity.ID)
            roBaseState.SetSessionSmall(oState, Global_asax.Identity.ID, Global_asax.ApplicationSource, "")

            If Global_asax.Identity.IDEmployee IsNot Nothing Then
                lrret = VTPortal.CommonHelper.GetDaysToCompensate(Global_asax.Identity, sDate, eDate, iExchangeEmployee, oLng, oState)
            Else
                lrret = New DaysCount() With {
                    .Days = 0,
                    .Status = ErrorCodes.OK
                }
            End If
        Catch ex As Exception
            lrret.Status = ErrorCodes.GENERAL_ERROR
            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::PortalSvcx::GetRequests")
        End Try

        Return lrret
    End Function

    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function GetRankingForDay(ByVal sDate As String, ByVal idRequestType As Integer, ByVal idReason As Integer) As roEmployeeRankingInformation
        Dim lrret As New roEmployeeRankingInformation

        If Not Global_asax.LoggedIn Then
            lrret.Status = VTPortal.StatusHelper.GetStatusFromState(Global_asax.SecurityResult)
            If lrret.Status = ErrorCodes.OK Then lrret.Status = ErrorCodes.NO_SESSION
            Return lrret
        End If

        Try
            Dim oLng As New roLanguage()
            oLng.SetLanguageReference("VTPortal", Global_asax.Identity.Language.Key)

            Dim oReqState As Requests.roRequestState = New Requests.roRequestState(Global_asax.Identity.ID)
            roBaseState.SetSessionSmall(oReqState, Global_asax.Identity.ID, Global_asax.ApplicationSource, "")

            lrret.Status = ErrorCodes.GENERAL_ERROR
            If Global_asax.Identity.IDEmployee IsNot Nothing Then
                Dim xDate As DateTime = DateTime.ParseExact(sDate, "dd/MM/yyyy", Nothing)
                lrret = VTPortal.RequestsHelper.GetRankingForDay(xDate, Global_asax.Identity.IDEmployee, idRequestType, idReason, oReqState)
            End If
        Catch ex As Exception
            lrret.Status = ErrorCodes.GENERAL_ERROR
            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::PortalSvcx::GetRankingForCauseAndDay")
        End Try

        Return lrret
    End Function

    <OperationContract()>
    <WebInvoke(Method:="POST", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function SavePunch() As TerminalStdResponse
        Dim lrret As New TerminalStdResponse

        If Not Global_asax.LoggedIn Then
            lrret.Result = VTPortal.StatusHelper.GetStatusFromState(Global_asax.SecurityResult)
            If lrret.Result = ErrorCodes.OK Then lrret.Result = ErrorCodes.NO_SESSION
            Return lrret
        End If

        Dim oState = New VTTerminals.roTerminalsState(-1)

        If Global_asax.TerminalResult <> TerminalBaseResultEnum.NoError Then
            Select Case Global_asax.TerminalResult
                Case TerminalBaseResultEnum.ServerStopped
                    oState.Result = VTTerminals.roTerminalsState.ResultEnum.ServerStopped
                Case Else
                    oState.Result = VTTerminals.roTerminalsState.ResultEnum.Exception
            End Select
        Else
            If Not Global_asax.LoggedIn Then
                oState.Result = VTTerminals.roTerminalsState.ResultEnum.TerminalNotConnected
            Else
                Try
                    oState.Result = VTTerminals.roTerminalsState.ResultEnum.ErrorSavingPunch

                    Dim strParamText As String = HttpContext.Current.Request.Params("oPunch")

                    Dim oPunch As New roTerminalPunch()
                    oPunch = roJSONHelper.DeserializeNewtonSoft(strParamText, oPunch.GetType())

                    Dim oTerminalManager As New VTTerminals.roTerminalsManager(oState, Global_asax.TerminalIdentity)

                    Select Case oPunch.Command
                        Case PunchCommand.Add
                            If oTerminalManager.SavePunch(oPunch) Then oState.Result = VTTerminals.roTerminalsState.ResultEnum.NoError
                        Case PunchCommand.Delete
                            If oTerminalManager.DeletePunch(oPunch) Then oState.Result = VTTerminals.roTerminalsState.ResultEnum.NoError
                        Case PunchCommand.Update

                    End Select

                    ' Si se mostraron mensajes de empleado, los marco como leídos
                    If oState.Result = VTTerminals.roTerminalsState.ResultEnum.NoError AndAlso Not oPunch.PunchData.ReadMessages Is Nothing AndAlso oPunch.PunchData.ReadMessages.Length > 0 Then
                        oTerminalManager.SetMessagesAsRead(oPunch.PunchData.ReadMessages)
                    End If
                Catch ex As Exception
                    oState.Result = VTTerminals.roTerminalsState.ResultEnum.Exception
                    Dim oLogState As New roBusinessState("Common.BaseState", "")
                    oLogState.UpdateStateInfo(ex, "VTLiveApi::TerminalsSvcx::SavePunch")
                End Try
            End If
        End If

        'Crear el response genérico
        Dim newGState As New roWsTerminalState
        roWsStateManager.CopyTo(oState, newGState, If(Global_asax.LoggedIn, Global_asax.TerminalIdentity.ID, -1))
        lrret.Status = newGState

        Return lrret
    End Function

    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function updateServerLanguage(ByVal lang As String) As StdResponse
        Dim lrret As New StdResponse

        If Not Global_asax.LoggedIn Then
            lrret.Status = VTPortal.StatusHelper.GetStatusFromState(Global_asax.SecurityResult)
            If lrret.Status = ErrorCodes.OK Then lrret.Status = ErrorCodes.NO_SESSION
            Return lrret
        End If

        Try
            Dim oPassport As roPassportTicket = Nothing
            If Global_asax.Supervisor IsNot Nothing AndAlso Global_asax.Identity IsNot Nothing Then
                oPassport = Global_asax.Supervisor
            Else
                oPassport = Global_asax.Identity
            End If

            If oPassport IsNot Nothing Then

                Dim tmp As roPassport = roPassportManager.GetPassport(oPassport.ID)
                Dim oLangManager As New roLanguageManager()
                Dim oLanguages As roPassportLanguage() = oLangManager.LoadLanguages()

                For Each oLanguage As roPassportLanguage In oLanguages
                    If oLanguage.Key = lang AndAlso oLanguage.ID <> tmp.Language.ID Then
                        tmp.Language = oLanguage

                        Dim oPassportManager As New roPassportManager()
                        oPassportManager.Save(tmp)
                    End If
                Next
            End If
            lrret.Status = ErrorCodes.OK
        Catch ex As Exception
            lrret.Status = ErrorCodes.GENERAL_ERROR
            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::PortalSvcx::updateServerLanguage")
        End Try

        Return lrret
    End Function

    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function updateServerTimeZone(ByVal timeZone As String) As StdResponse
        Dim lrret As New StdResponse

        If Not Global_asax.LoggedIn Then
            lrret.Status = VTPortal.StatusHelper.GetStatusFromState(Global_asax.SecurityResult)
            If lrret.Status = ErrorCodes.OK Then lrret.Status = ErrorCodes.NO_SESSION
            Return lrret
        End If

        Try
            Dim oPassport As roPassportTicket = Nothing
            If Global_asax.Supervisor IsNot Nothing AndAlso Global_asax.Identity IsNot Nothing Then
                oPassport = Global_asax.Supervisor
            Else
                oPassport = Global_asax.Identity
            End If

            If oPassport IsNot Nothing Then
                roPassportManager.SetTimeZoneData(roAppType.VTPortal, oPassport.ID, timeZone, New roSecurityState)
            End If
            lrret.Status = ErrorCodes.OK
        Catch ex As Exception
            lrret.Status = ErrorCodes.GENERAL_ERROR
            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::PortalSvcx::updateServerTimeZone")
        End Try

        Return lrret
    End Function

    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function UploadDocumenttoSign(ByVal documentId As String) As roGenericResponse(Of DocumentVID_PostDocResult)
        '
        ' Subimos el documento a la plataforma de Validate ID para ser firmado
        ' y retornamos la URL para que el signante pueda firmarlo

        Dim lrret As New roGenericResponse(Of DocumentVID_PostDocResult)
        If Not Global_asax.LoggedIn Then
            lrret.Status = VTPortal.StatusHelper.GetStatusFromState(Global_asax.SecurityResult)
            If lrret.Status = ErrorCodes.OK Then lrret.Status = ErrorCodes.NO_SESSION
            Return lrret
        End If

        Try
            Dim oState As New VTDocuments.roDocumentState(Global_asax.Identity.ID)
            roBaseState.SetSessionSmall(oState, Global_asax.Identity.ID, Global_asax.ApplicationSource, "")
            lrret = VTPortal.DocumentsHelper.UploadDocumenttoSign(documentId, Global_asax.Identity.IDEmployee, oState)
        Catch ex As Exception
            lrret.Status = ErrorCodes.GENERAL_ERROR
            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::PortalSvcx::UploadDocumenttoSign")
        End Try

        'Console.WriteLine(lrret.Value.SignatureViDRemoteURL)

        Return lrret
    End Function

    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function DocumentSignatureStatus(ByVal guidDoc As String) As roGenericResponse(Of DocumentInfoDTO)
        '
        ' Obtenemos la información sobre el documento subido a ValidationID
        '
        Dim lrret As New roGenericResponse(Of DocumentInfoDTO)
        If Not Global_asax.LoggedIn Then
            lrret.Status = VTPortal.StatusHelper.GetStatusFromState(Global_asax.SecurityResult)
            If lrret.Status = ErrorCodes.OK Then lrret.Status = ErrorCodes.NO_SESSION
            Return lrret
        End If

        Try
            Dim oState As New VTDocuments.roDocumentState(Global_asax.Identity.ID)
            roBaseState.SetSessionSmall(oState, Global_asax.Identity.ID, Global_asax.ApplicationSource, "")
            lrret = VTPortal.DocumentsHelper.DocumentSignatureStatus(guidDoc, oState)
        Catch ex As Exception
            lrret.Status = ErrorCodes.GENERAL_ERROR
            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::PortalSvcx::DocumentSignatureStatus")
        End Try

        Return lrret
    End Function

    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function GetMyDailyRecordCalendar(ByVal selectedDate As String) As roGenericResponse(Of roDailyRecordCalendar)
        '
        ' Obtenemos el calendario de registro de la jornada
        '
        Dim lrret As New roGenericResponse(Of roDailyRecordCalendar)
        If Not Global_asax.LoggedIn Then
            lrret.Status = VTPortal.StatusHelper.GetStatusFromState(Global_asax.SecurityResult)
            If lrret.Status = ErrorCodes.OK Then lrret.Status = ErrorCodes.NO_SESSION
            Return lrret
        End If

        Try
            Dim dDate As DateTime = DateTime.ParseExact(selectedDate, "dd/MM/yyyy", Nothing)
            lrret = VTPortal.ShiftsHelper.GetEmployeeDailyRecordCalendar(Global_asax.Identity, Global_asax.Identity.IDEmployee, dDate)
        Catch ex As Exception
            lrret.Status = ErrorCodes.GENERAL_ERROR
            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::PortalSvcx::GetMyDailyRecordCalendar")
        End Try

        Return lrret
    End Function

    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function DeleteDailyRecord(ByVal dailyRecordId As Integer) As StdRequestResponse
        Dim lrret As New StdRequestResponse
        If Not Global_asax.LoggedIn Then
            lrret.Status = VTPortal.StatusHelper.GetStatusFromState(Global_asax.SecurityResult)
            If lrret.Status = ErrorCodes.OK Then lrret.Status = ErrorCodes.NO_SESSION
            Return lrret
        End If

        Try
            lrret.Status = ErrorCodes.OK

            Dim oProgrammedAbsenceState As New Absence.roProgrammedAbsenceState(-1)
            roBaseState.SetSessionSmall(oProgrammedAbsenceState, Global_asax.Identity.ID, Global_asax.ApplicationSource, "")

            lrret = VTPortal.ShiftsHelper.DeleteDailyRecord(Global_asax.Identity, dailyRecordId)
        Catch ex As Exception
            lrret.Status = ErrorCodes.GENERAL_ERROR
            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::PortalSvcx::DeleteProgrammedAbsence")
        End Try

        Return lrret

    End Function

    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function GetMyChannels() As roGenericResponse(Of roChannel())
        Dim lrret As roGenericResponse(Of roChannel())
        If Not Global_asax.LoggedIn Then
            lrret = New roGenericResponse(Of roChannel()) With {
                .Status = ErrorCodes.GENERAL_ERROR,
                .Value = {}}
            lrret.Status = VTPortal.StatusHelper.GetStatusFromState(Global_asax.SecurityResult)
            If lrret.Status = ErrorCodes.OK Then lrret.Status = ErrorCodes.NO_SESSION
            Return lrret
        End If

        Try

            Dim oChannelState As New roChannelState(-1)
            roBaseState.SetSessionSmall(oChannelState, Global_asax.Identity.ID, Global_asax.ApplicationSource, "")

            lrret = VTPortal.EmployeesHelper.GetEmployeeAvailableChannels(Global_asax.Identity.IDEmployee, oChannelState)
        Catch ex As Exception
            lrret = New roGenericResponse(Of roChannel()) With {
                .Status = ErrorCodes.GENERAL_ERROR,
                .Value = {}}

            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::PortalSvcx::GetMyChannels")
        End Try

        Return lrret

    End Function

    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function GetMyConversationsByChannel(ByVal idChannel As Integer) As roGenericResponse(Of roConversation())
        Dim lrret As roGenericResponse(Of roConversation())
        If Not Global_asax.LoggedIn Then
            lrret = New roGenericResponse(Of roConversation()) With {
                .Status = ErrorCodes.GENERAL_ERROR,
                .Value = {}}
            lrret.Status = VTPortal.StatusHelper.GetStatusFromState(Global_asax.SecurityResult)
            If lrret.Status = ErrorCodes.OK Then lrret.Status = ErrorCodes.NO_SESSION
            Return lrret
        End If

        Try

            Dim oConversationState As New roConversationState(-1)
            roBaseState.SetSessionSmall(oConversationState, Global_asax.Identity.ID, Global_asax.ApplicationSource, "")

            lrret = VTPortal.EmployeesHelper.GetEmployeeAvailableConversationsByChannel(Global_asax.Identity.IDEmployee, idChannel, oConversationState)
        Catch ex As Exception
            lrret = New roGenericResponse(Of roConversation()) With {
                .Status = ErrorCodes.GENERAL_ERROR,
                .Value = {}}

            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::PortalSvcx::GetMyConversationsByChannel")
        End Try

        Return lrret

    End Function

    <OperationContract()>
    <WebInvoke(Method:="POST", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function AddNewConversation() As roGenericResponse(Of roConversation)
        Dim lrret As roGenericResponse(Of roConversation)
        Dim bolCorrect As Boolean = True

        Try
            If Not Global_asax.LoggedIn Then
                lrret = New roGenericResponse(Of roConversation) With {
                .Status = ErrorCodes.GENERAL_ERROR,
                .Value = Nothing}
                lrret.Status = VTPortal.StatusHelper.GetStatusFromState(Global_asax.SecurityResult)
                If lrret.Status = ErrorCodes.OK Then lrret.Status = ErrorCodes.NO_SESSION
                Return lrret
            End If

            Dim idChannel As String = HttpContext.Current.Request.Params("idChannel")
            Dim title As String = HttpContext.Current.Request.Params("title")
            Dim encodedMessage As String = HttpContext.Current.Request.Params("message")
            Dim message As String = HttpUtility.UrlDecode(encodedMessage)
            Dim isAnonymous As Integer = roTypes.Any2Integer(HttpContext.Current.Request.Params("isAnonymous"))

            Dim oConversationState As New roConversationState(-1)
            roBaseState.SetSessionSmall(oConversationState, Global_asax.Identity.ID, Global_asax.ApplicationSource, "")

            lrret = VTPortal.EmployeesHelper.CreateConversation(Global_asax.Identity.IDEmployee, idChannel, title, message, isAnonymous, oConversationState)
        Catch ex As Exception
            lrret = New roGenericResponse(Of roConversation) With {
                .Status = ErrorCodes.GENERAL_ERROR,
                .Value = Nothing}

            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::PortalSvcx::GetMyConversationsByChannel")
        End Try

        Return lrret

    End Function

    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function GetMyMessagesByConversation(ByVal idConversation As Integer) As roGenericResponse(Of roMessage())
        Dim lrret As roGenericResponse(Of roMessage())
        If Not Global_asax.LoggedIn Then
            lrret = New roGenericResponse(Of roMessage()) With {
                .Status = ErrorCodes.GENERAL_ERROR,
                .Value = {}}
            lrret.Status = VTPortal.StatusHelper.GetStatusFromState(Global_asax.SecurityResult)
            If lrret.Status = ErrorCodes.OK Then lrret.Status = ErrorCodes.NO_SESSION
            Return lrret
        End If

        Try

            Dim oMessageState As New roMessageState(-1)
            roBaseState.SetSessionSmall(oMessageState, Global_asax.Identity.ID, Global_asax.ApplicationSource, "")

            lrret = VTPortal.EmployeesHelper.GetEmployeeMessagesByConversation(Global_asax.Identity.IDEmployee, idConversation, oMessageState)
        Catch ex As Exception
            lrret = New roGenericResponse(Of roMessage()) With {
                .Status = ErrorCodes.GENERAL_ERROR,
                .Value = {}}

            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::PortalSvcx::GetMyMessagesByConversation")
        End Try

        Return lrret

    End Function

    <OperationContract()>
    <WebInvoke(Method:="POST", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function AddNewMessage() As roGenericResponse(Of roMessage)
        Dim lrret As roGenericResponse(Of roMessage)
        Dim bolCorrect As Boolean = True

        Try
            If Not Global_asax.LoggedIn Then
                lrret = New roGenericResponse(Of roMessage) With {
                .Status = ErrorCodes.GENERAL_ERROR,
                .Value = Nothing}
                lrret.Status = VTPortal.StatusHelper.GetStatusFromState(Global_asax.SecurityResult)
                If lrret.Status = ErrorCodes.OK Then lrret.Status = ErrorCodes.NO_SESSION
                Return lrret
            End If

            Dim idConversation As String = HttpContext.Current.Request.Params("idConversation")
            Dim encodedMessage As String = HttpContext.Current.Request.Params("message")
            Dim message As String = HttpUtility.UrlDecode(encodedMessage)

            Dim oMessageState As New roMessageState(-1)
            roBaseState.SetSessionSmall(oMessageState, Global_asax.Identity.ID, Global_asax.ApplicationSource, "")

            lrret = VTPortal.EmployeesHelper.CreateMessage(Global_asax.Identity.IDEmployee, idConversation, message, oMessageState)
        Catch ex As Exception
            lrret = New roGenericResponse(Of roMessage) With {
                .Status = ErrorCodes.GENERAL_ERROR,
                .Value = Nothing}

            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::PortalSvcx::AddNewMessage")
        End Try

        Return lrret

    End Function

    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function GetDEXUrl() As roGenericResponse(Of String)
        Dim lrret As roGenericResponse(Of String)
        If Not Global_asax.LoggedIn Then
            lrret = New roGenericResponse(Of String) With {
                .Status = ErrorCodes.GENERAL_ERROR,
                .Value = Nothing}
            lrret.Status = VTPortal.StatusHelper.GetStatusFromState(Global_asax.SecurityResult)
            If lrret.Status = ErrorCodes.OK Then lrret.Status = ErrorCodes.NO_SESSION
            Return lrret
        End If

        Try
            Dim oServiceApiState As New roServiceApiManagerState()
            roBaseState.SetSessionSmall(oServiceApiState, Global_asax.Identity.ID, Global_asax.ApplicationSource, "")
            lrret = VTPortal.EmployeesHelper.GetDEXUrl(oServiceApiState)
        Catch ex As Exception
            lrret = New roGenericResponse(Of String) With {
                .Status = ErrorCodes.GENERAL_ERROR,
                .Value = Nothing}

            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::PortalSvcx::GetDEXUrl")
        End Try

        Return lrret

    End Function

    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function GetConceptsSummaryByShift(ByVal idShift As String) As roGenericResponse(Of List(Of roHolidayConceptsSummary))
        Dim lrret As New roGenericResponse(Of List(Of roHolidayConceptsSummary))

        If Not Global_asax.LoggedIn Then
            lrret.Status = VTPortal.StatusHelper.GetStatusFromState(Global_asax.SecurityResult)
            If lrret.Status = ErrorCodes.OK Then lrret.Status = ErrorCodes.NO_SESSION
            Return lrret
        End If

        Try

            Dim oEmpState As New Employee.roEmployeeState(Global_asax.Identity.ID)
            roBaseState.SetSessionSmall(oEmpState, Global_asax.Identity.ID, Global_asax.ApplicationSource, "")

            lrret = VTPortal.EmployeesHelper.GetEmployeeConceptsSummaryHolidays(Global_asax.Identity.IDEmployee, idShift, Global_asax.Identity.ID)
        Catch ex As Exception
            lrret.Status = ErrorCodes.GENERAL_ERROR
            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::PortalSvcx::GetConceptsSummaryByShift")
        End Try

        Return lrret
    End Function

    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function GetConceptsDetailByShift(ByVal idShift As String) As roGenericResponse(Of List(Of roHolidayConceptsDetail))
        Dim lrret As New roGenericResponse(Of List(Of roHolidayConceptsDetail))

        If Not Global_asax.LoggedIn Then
            lrret.Status = VTPortal.StatusHelper.GetStatusFromState(Global_asax.SecurityResult)
            If lrret.Status = ErrorCodes.OK Then lrret.Status = ErrorCodes.NO_SESSION
            Return lrret
        End If

        Try

            Dim oEmpState As New Employee.roEmployeeState(Global_asax.Identity.ID)
            roBaseState.SetSessionSmall(oEmpState, Global_asax.Identity.ID, Global_asax.ApplicationSource, "")

            lrret = VTPortal.EmployeesHelper.GetEmployeeConceptsDetailHolidays(Global_asax.Identity.IDEmployee, idShift)
        Catch ex As Exception
            lrret.Status = ErrorCodes.GENERAL_ERROR
            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::PortalSvcx::GetConceptsDetailByShift")
        End Try

        Return lrret
    End Function

    ''' <summary>
    ''' Se requiere que la sesión este iniciada en el sistema. Esta función responde a un método GET del servidor.
    ''' Habilita el modo portal compartido en un dispositivo registrándolo como terminal
    ''' </summary>
    ''' <param name="serialNumber">Número de serie del dispositivo</param>
    ''' <param name="name">Nombre del terminal asociado al dispositivo</param>
    ''' <returns>Obtenemos un objeto estandar de respuesta de petición indicando si se ha podido habilitar el modo compartido y en caso contrario el código de error obtenido.</returns>
    <OperationContract()>
    <WebInvoke(Method:="POST", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function EnableTimegate() As roGenericResponse(Of DTOs.Timegate)
        Dim lrret As roGenericResponse(Of DTOs.Timegate) = New roGenericResponse(Of DTOs.Timegate)

        Try
            If Not Global_asax.LoggedIn Then
                lrret.Status = VTPortal.StatusHelper.GetStatusFromState(Global_asax.SecurityResult)
                If lrret.Status = ErrorCodes.OK Then lrret.Status = ErrorCodes.NO_SESSION
                Return lrret
            End If

            Dim terminalState As New roTerminalState
            roBaseState.SetSessionSmall(terminalState, Global_asax.Identity.ID, Global_asax.ApplicationSource, "")

            Dim serialNumber As String = HttpContext.Current.Request.Params("serialNumber")
            Dim apkVersion As String = HttpContext.Current.Request.Params("apkVersion")
            Dim name As String = HttpContext.Current.Request.Params("name")

            lrret = TerminalsHelper.GetTimeGateConfiguration(serialNumber, terminalState)

            If lrret.Status = ErrorCodes.NOT_FOUND Then
                lrret = TerminalsHelper.EnableTimegate(serialNumber, name, apkVersion, terminalState)
                'when time gate is enabled, user session should be closed
                VTPortal.SecurityHelper.LogoutUserSession(If(Global_asax.Supervisor IsNot Nothing, Global_asax.Supervisor.ID, Global_asax.Identity.ID), Global_asax.ApplicationSource, serialNumber)

                Robotics.Web.Base.CookieSession.ClearAuthenticationCookie(StrConv(roAppType.VTPortal.ToString(), VbStrConv.ProperCase))
            End If

        Catch ex As Exception
            lrret.Status = ErrorCodes.GENERAL_ERROR
            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::PortalSvcx::EnableTimegate")
        End Try

        Return lrret
    End Function

    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function GetTimegateConfiguration(ByVal serialNumber) As roGenericResponse(Of DTOs.Timegate)
        Dim lrret As roGenericResponse(Of DTOs.Timegate) = New roGenericResponse(Of DTOs.Timegate)
        If Not Global_asax.LoggedIn Then
            lrret.Status = VTPortal.StatusHelper.GetStatusFromState(Global_asax.SecurityResult)
            If lrret.Status = ErrorCodes.OK Then lrret.Status = ErrorCodes.NO_SESSION
            Return lrret
        End If

        Try
            Dim terminalState As New roTerminalState
            roBaseState.SetSessionSmall(terminalState, Global_asax.Identity.ID, Global_asax.ApplicationSource, "")

            lrret = TerminalsHelper.GetTimeGateConfiguration(serialNumber, terminalState)
        Catch ex As Exception
            lrret.Status = ErrorCodes.GENERAL_ERROR
            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::PortalSvcx::GetTimegateConfiguration")
        End Try

        Return lrret

    End Function

    ''' <summary>
    ''' Se requiere que la sesión este iniciada en el sistema. Esta función responde a un método GET del servidor.
    ''' Deshabilita el modo portal compartido en un dispositivo registrándolo como terminal
    ''' </summary>
    ''' <param name="serialNumber">Número de serie del dispositivo</param>    
    ''' <returns>Obtenemos un objeto estandar de respuesta de petición indicando si se ha podido deshabilitar el modo compartido y en caso contrario el código de error obtenido.</returns>
    <OperationContract()>
    <WebInvoke(Method:="POST", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function DisableTimegate() As StdResponse
        Dim lrret As New StdResponse

        Try
            If Not Global_asax.LoggedIn Then
                lrret.Status = VTPortal.StatusHelper.GetStatusFromState(Global_asax.SecurityResult)
                If lrret.Status = ErrorCodes.OK Then lrret.Status = ErrorCodes.NO_SESSION
                Return lrret
            End If

            Dim oSecurityState As New roWsState()
            roBaseState.SetSessionSmall(oSecurityState, Global_asax.Identity.ID, Global_asax.ApplicationSource, "")

            Dim serialNumber As String = HttpContext.Current.Request.Params("serialNumber")

            lrret = TerminalsHelper.DisableTimegate(serialNumber)

            If lrret.Status = ErrorCodes.OK Then
                VTPortal.SecurityHelper.LogoutUserSession(If(Global_asax.Supervisor IsNot Nothing, Global_asax.Supervisor.ID, Global_asax.Identity.ID), Global_asax.ApplicationSource, serialNumber)


                Robotics.Web.Base.CookieSession.ClearAuthenticationCookie(StrConv(roAppType.VTPortal.ToString(), VbStrConv.ProperCase))
            End If

        Catch ex As Exception
            lrret.Status = ErrorCodes.GENERAL_ERROR
            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::PortalSvcx::DiableTimegate")
        End Try

        Return lrret
    End Function

    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function GetAllPortalWebLinks() As roGenericResponse(Of List(Of roWebLink))
        Dim lrret As New roGenericResponse(Of List(Of roWebLink))

        If Not Global_asax.LoggedIn Then
            lrret.Status = VTPortal.StatusHelper.GetStatusFromState(Global_asax.SecurityResult)
            If lrret.Status = ErrorCodes.OK Then lrret.Status = ErrorCodes.NO_SESSION
            Return lrret
        End If

        Try
            Dim oWebLinkState As New roWebLinksManagerState()
            roBaseState.SetSessionSmall(oWebLinkState, Global_asax.Identity.ID, Global_asax.ApplicationSource, "")
            lrret = VTPortal.WebLinksHelper.GetAllPortalWebLinks(oWebLinkState)
        Catch ex As Exception
            lrret.Status = ErrorCodes.GENERAL_ERROR
            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::PortalSvcx::GetAllWebLinks")
        End Try

        Return lrret

    End Function

End Class