Imports System.Security.Principal
Imports System.ServiceModel
Imports System.ServiceModel.Activation
Imports System.ServiceModel.Web
Imports System.Web.Hosting
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTPortal
Imports Robotics.Base.VTVisits
Imports Robotics.Security
Imports Robotics.Security.Base
Imports Robotics.VTBase

<ServiceContract(Namespace:="")>
<AspNetCompatibilityRequirements(RequirementsMode:=AspNetCompatibilityRequirementsMode.Allowed)>
Public Class VisitsSvcx

    Public Const OK As Integer = 0
    Public Const NO_SESSION As Integer = -1
    Public Const BAD_CREDENTIALS As Integer = -2
    Public Const NOT_FOUND As Integer = -3
    Public Const GENERAL_ERROR As Integer = -4
    Public Const WRONG_MEDIA_TYPE As Integer = -5
    Public Const NOT_LICENSED As Integer = -6
    Public Const SERVER_NOT_RUNNING As Integer = -7
    Public Const NO_LIVE_PORTAL As Integer = -8
    Public Const NO_PERMISSIONS As Integer = -9

    Public Const LOGIN_RESULT_LOW_STRENGHT_ERROR As Integer = -59
    Public Const LOGIN_RESULT_MEDIUM_STRENGHT_ERROR As Integer = -60
    Public Const LOGIN_RESULT_HIGH_STRENGHT_ERROR As Integer = -61
    Public Const LOGIN_PASSWORD_EXPIRED As Integer = -62
    Public Const LOGIN_NEED_TEMPORANY_KEY As Integer = -63
    Public Const LOGIN_TEMPORANY_KEY_EXPIRED As Integer = -64
    Public Const LOGIN_INVALID_VALIDATION_CODE As Integer = -65
    Public Const LOGIN_BLOCKED_ACCESS_APP As Integer = -66
    Public Const LOGIN_TEMPORANY_BLOQUED As Integer = -67
    Public Const LOGIN_GENERAL_BLOCK_ACCESS As Integer = -68
    Public Const LOGIN_INVALID_CLIENT_LOCATION As Integer = -69
    Public Const LOGIN_INVALID_VERSION_APP As Integer = -70
    Public Const LOGIN_INVALID_APP As Integer = -71

    Private Const ResutException = "Exception"

    Private Shared SERVER_VERSION = Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString

#Region "User Functions"

    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function CheckIsSSO() As SSO_Windows_Data
        Dim oSSO As New SSO_Windows_Data()
        Try
            Dim user As WindowsPrincipal = CType(HttpContext.Current.User, WindowsPrincipal)
            oSSO.SSOEnabled = True
            oSSO.SSOLoggedIn = user.Identity.IsAuthenticated
            oSSO.SSOUser = user.Identity.Name
        Catch ex As Exception
            oSSO.SSOEnabled = False
            oSSO.SSOLoggedIn = False
            oSSO.SSOUser = ""
        End Try
        oSSO.Status = OK
        Return oSSO
    End Function

    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function IsAlive() As IsAlive
        Dim oAlive As New IsAlive()
        oAlive.Status = OK
        oAlive.ServerVersion = SERVER_VERSION
        Return oAlive
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
    Public Function Login(ByVal usr As String, ByVal pwd As String, ByVal isApp As Boolean, ByVal appVersion As String, ByVal validationCode As String) As roUser
        Dim oState As New roSecurityState(-1, HttpContext.Current)
        Dim user As New roUser()
        Dim lrret As New LoginResult

        Try

            If Global_asax.LoggedIn Then
                'Puede ser que lanze una excepción ya que los tiempos de timeout son distintos
                ' no pasa nada.
                Try
                    oState = New roSecurityState(Global_asax.Identity.ID, HttpContext.Current)
                    SessionHelper.SessionRemove(Global_asax.Identity.ID & "*" & roAppType.VTVisits.ToString(), oState)
                Catch ex As Exception
                End Try
            End If

            roBaseState.SetSessionSmall(oState, -1, Global_asax.ApplicationSource, "")
            Dim strAuthToken As String = If(HttpContext.Current.Request.Headers("roAuth") IsNot Nothing, HttpContext.Current.Request.Headers("roAuth"), "")

            lrret = VTPortal.SecurityHelper.Login(usr, pwd, "ESP", SERVER_VERSION, appVersion, validationCode, Afk.ZoneInfo.TzTimeZone.CurrentTzTimeZone.Name, strAuthToken, False, roAppSource.Visits, oState, "", False)

            user.status = lrret.Status

            If lrret.Status = ErrorCodes.OK Then
                user.token = lrret.Token
                user.userid = lrret.UserId
                user.language = "ESP"
                user.hascreate = VTPortal.SecurityHelper.GetFeaturePermission(lrret.UserId, "Visits.Create", "U")
                If user.hascreate = 0 Then
                    user.hascreateEmployee = VTPortal.SecurityHelper.GetFeaturePermission(lrret.UserId, "Visits.Receive", "E")
                End If
                user.haschange = VTPortal.SecurityHelper.GetFeaturePermission(lrret.UserId, "Visits.Manage", "U")
                user.hasfields = VTPortal.SecurityHelper.GetFeaturePermission(lrret.UserId, "Visits.UserFields", "U")
                user.hasaccess = VTPortal.SecurityHelper.GetFeaturePermission(lrret.UserId, "Visits", "U")
                user.idemployee = lrret.EmployeeId
            End If

            Robotics.Web.Base.CookieSession.CreateAuthenticationCookie(lrret.Status = ErrorCodes.OK, lrret.Token, StrConv(roAppType.VTVisits.ToString(), VbStrConv.ProperCase))
        Catch ex As Exception
            lrret.Status = ErrorCodes.BAD_CREDENTIALS

            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::VisitsSvcx::Login")
        End Try

        Return user
    End Function

    <OperationContract()>
    <WebInvoke(Method:="POST", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function Authenticate() As roUser
        Dim oState As New roSecurityState(-1, HttpContext.Current)
        Dim user As New roUser()
        Dim lrret As New LoginResult

        Try

            If Global_asax.LoggedIn Then
                'Puede ser que lanze una excepción ya que los tiempos de timeout son distintos
                ' no pasa nada.
                Try
                    oState = New roSecurityState(Global_asax.Identity.ID, HttpContext.Current)
                    SessionHelper.SessionRemove(Global_asax.Identity.ID & "*" & roAppType.VTVisits.ToString(), oState)
                Catch ex As Exception
                End Try
            End If

            roBaseState.SetSessionSmall(oState, -1, Global_asax.ApplicationSource, "")
            Dim strAuthToken As String = If(HttpContext.Current.Request.Headers("roAuth") IsNot Nothing, HttpContext.Current.Request.Headers("roAuth"), "")

            Dim usr As String = HttpContext.Current.Request.Params("usr")
            Dim pwd As String = HttpContext.Current.Request.Params("pwd")
            Dim appVersion As String = HttpContext.Current.Request.Params("appVersion")
            Dim validationCode As String = HttpContext.Current.Request.Params("validationCode")
            Dim isApp As Boolean = roTypes.Any2Boolean(HttpContext.Current.Request.Params("isApp"))

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

            lrret = VTPortal.SecurityHelper.Login(usr, loginPwd, "ESP", SERVER_VERSION, appVersion, validationCode, Afk.ZoneInfo.TzTimeZone.CurrentTzTimeZone.Name, strAuthToken, False, roAppSource.Visits, oState, "", False)

            user.status = lrret.Status

            Dim bAuthenticated As Boolean = False
            If lrret.Status = ErrorCodes.OK OrElse
            lrret.Status = ErrorCodes.LOGIN_PASSWORD_EXPIRED OrElse
            lrret.Status = ErrorCodes.LOGIN_RESULT_LOW_STRENGHT_ERROR OrElse
            lrret.Status = ErrorCodes.LOGIN_RESULT_MEDIUM_STRENGHT_ERROR OrElse
            lrret.Status = ErrorCodes.LOGIN_RESULT_HIGH_STRENGHT_ERROR Then
                bAuthenticated = True
                user.token = lrret.Token
                user.userid = lrret.UserId
                user.language = "ESP"
                user.hascreate = VTPortal.SecurityHelper.GetFeaturePermission(lrret.UserId, "Visits.Create", "U")
                If user.hascreate = 0 Then
                    user.hascreateEmployee = VTPortal.SecurityHelper.GetFeaturePermission(lrret.UserId, "Visits.Receive", "E")
                End If
                user.haschange = VTPortal.SecurityHelper.GetFeaturePermission(lrret.UserId, "Visits.Manage", "U")
                user.hasfields = VTPortal.SecurityHelper.GetFeaturePermission(lrret.UserId, "Visits.UserFields", "U")
                user.hasaccess = VTPortal.SecurityHelper.GetFeaturePermission(lrret.UserId, "Visits", "U")
                user.idemployee = lrret.EmployeeId
                user.showLegalText = lrret.ShowLegalText
            End If

            Robotics.Web.Base.CookieSession.CreateAuthenticationCookie(bAuthenticated, lrret.Token, StrConv(roAppType.VTVisits.ToString(), VbStrConv.ProperCase))
        Catch ex As Exception
            lrret.Status = ErrorCodes.BAD_CREDENTIALS

            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::VisitsSvcx::Login")
        End Try

        Return user
    End Function

    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function CheckConsent() As roPassportConsentPortals
        Dim oState As New roSecurityState(-1, HttpContext.Current)
        Dim lrret As New roPassportConsentPortals
        Try
            lrret = New roPassportConsentPortals() With {.Status = ErrorCodes.OK, .PassportConsent = New roPassportConsent() With {.IsValid = True, .Type = ConsentTypeEnum._Visits, .IDPassport = Global_asax.Identity.ID}}
        Catch ex As Exception
            lrret.Status = ErrorCodes.GENERAL_ERROR

            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::VisitsSvcx::CheckConsent")
        End Try

        Return lrret

    End Function

    <OperationContract()>
    <WebInvoke(Method:="POST", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function SaveConsent() As roPassportConsentPortals
        Dim oState As New roSecurityState(-1, HttpContext.Current)
        Dim lrret As New roPassportConsentPortals

        Dim values = HttpContext.Current.Request.Form("values")

        Dim message = HttpUtility.HtmlDecode(values)

        Try
            If Not Global_asax.LoggedIn Then
                'Puede ser que lanze una excepción ya que los tiempos de timeout son distintos
                ' no pasa nada.
                Try
                    oState = New roSecurityState(Global_asax.Identity.ID, HttpContext.Current)
                    SessionHelper.SessionRemove(Global_asax.Identity.ID & "*" & roAppType.VTVisits.ToString(), oState)
                Catch ex As Exception
                End Try
            End If

            roBaseState.SetSessionSmall(oState, -1, Global_asax.ApplicationSource, "")
            lrret = New roPassportConsentPortals() With {.Status = ErrorCodes.OK, .PassportConsent = New roPassportConsent() With {.IsValid = True, .Type = ConsentTypeEnum._Visits, .IDPassport = Global_asax.Identity.ID}}
        Catch ex As Exception
            lrret.Status = ErrorCodes.GENERAL_ERROR

            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::VisitsSvcx::CheckConsent")
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
    Public Function Logout(ByVal userId As Integer) As roUser
        Dim lrret As New StdResponse()
        Dim user As New roUser()
        If Not Global_asax.LoggedIn Then
            lrret.Status = VTPortal.StatusHelper.GetStatusFromState(Global_asax.SecurityResult)
            If lrret.Status = ErrorCodes.OK Then lrret.Status = ErrorCodes.NO_SESSION
            user.status = lrret.Status
            Return user
        End If

        Try

            Dim oState As New roSecurityState()
            roBaseState.SetSessionSmall(oState, Global_asax.Identity.ID, Global_asax.ApplicationSource, "")

            lrret = VTPortal.SecurityHelper.Logout(Global_asax.Identity.ID, oState)
            user.status = lrret.Status

            Robotics.Web.Base.CookieSession.ClearAuthenticationCookie(StrConv(roAppType.VTVisits.ToString(), VbStrConv.ProperCase))
            Global_asax.Logout()
        Catch ex As Exception
            lrret.Status = ErrorCodes.GENERAL_ERROR
            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::VisitsSvcx::Logout")
        End Try

        Return user
    End Function

    <OperationContract()>
    <WebInvoke(Method:="POST", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function AuthenticateSession() As roUser
        Dim user As New roUser

        If Global_asax.LoggedIn Then

            Dim Ticket As roPassportTicket = Global_asax.Identity
            Try
                Dim oRet As Object = "True"

                If Not roTypes.Any2Boolean(oRet) Then
                    user.status = ErrorCodes.SERVER_NOT_RUNNING
                Else

                    user.token = Robotics.Web.Base.CookieSession.GetAuthenticationCookie(StrConv(roAppType.VTVisits.ToString(), VbStrConv.ProperCase))
                    user.userid = Ticket.ID
                    user.language = "ESP"
                    user.hascreate = VTPortal.SecurityHelper.GetFeaturePermission(Ticket.ID, "Visits.Create", "U")
                    If user.hascreate = 0 Then
                        user.hascreateEmployee = VTPortal.SecurityHelper.GetFeaturePermission(user.userid, "Visits.Receive", "E")
                    End If
                    user.haschange = VTPortal.SecurityHelper.GetFeaturePermission(Ticket.ID, "Visits.Manage", "U")
                    user.hasfields = VTPortal.SecurityHelper.GetFeaturePermission(Ticket.ID, "Visits.UserFields", "U")
                    user.hasaccess = VTPortal.SecurityHelper.GetFeaturePermission(Ticket.ID, "Visits", "U")
                    user.idemployee = If(Ticket.IDEmployee Is Nothing, -1, Ticket.IDEmployee)

                End If
            Catch ex As Exception

                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::SecurityHelper::GetLoggedInUserInfo")
            End Try

        End If
        Return user

    End Function

    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function ChangeLanguage(ByVal Language As String) As roUser
        Dim lrret As New roUser

        If Global_asax.LoggedIn Then

            Dim Ticket As roPassportTicket = Global_asax.Identity

            Dim oManager As New roPassportManager

            Dim oPassport = oManager.LoadPassport(Ticket.ID, LoadType.Passport)

            Dim lng As String
            Select Case Language
                Case "es"
                    lng = "ESP"
                Case "ca"
                    lng = "CAT"
                Case "en"
                    lng = "ENG"
                Case Else
                    lng = "ESP"
            End Select

            Try
                Dim oLangManager As New roLanguageManager
                oPassport.Language = oLangManager.LoadByKey(lng)
                oManager.Save(oPassport)

                lrret.language = lng
            Catch ex As Exception

            End Try
        End If
        Return lrret
    End Function

    <OperationContract()>
    <WebInvoke(Method:="POST", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function ChangePassword() As roUser
        Dim lrret As New StdResponse()
        Dim bolCorrect As Boolean = True
        Dim user As New roUser()
        Try
            If Not Global_asax.LoggedIn Then
                lrret.Status = VTPortal.StatusHelper.GetStatusFromState(Global_asax.SecurityResult)
                If lrret.Status = ErrorCodes.OK Then lrret.Status = ErrorCodes.NO_SESSION
                user.status = lrret.Status
                Return user
            End If

            Dim oSecurityState As New roSecurityState()
            roBaseState.SetSessionSmall(oSecurityState, Global_asax.Identity.ID, Global_asax.ApplicationSource, "")
            Dim strToken As String = If(HttpContext.Current.Request.Headers("roToken") IsNot Nothing, HttpContext.Current.Request.Headers("roToken"), "")

            Dim oldPassword As String = HttpContext.Current.Request.Params("oldPassword")
            Dim newPassword As String = HttpContext.Current.Request.Params("newPassword")
            Dim userId As Integer = roTypes.Any2Integer(HttpContext.Current.Request.Params("userId"))

            Dim oldPwd As (Boolean, String) = Robotics.VTBase.CryptographyHelper.DecryptLiveApi(oldPassword)
            Dim newPwd As (Boolean, String) = Robotics.VTBase.CryptographyHelper.DecryptLiveApi(newPassword)
            If oldPwd.Item1 AndAlso newPwd.Item1 Then
                lrret = VTPortal.SecurityHelper.ChangePassword(Global_asax.Identity.ID, oldPwd.Item2, newPwd.Item2, strToken, oSecurityState)
                user.status = lrret.Status
            Else
                lrret.Status = ErrorCodes.GENERAL_ERROR
                user.status = lrret.Status
            End If
        Catch ex As Exception
            lrret.Status = ErrorCodes.GENERAL_ERROR
            user.status = lrret.Status
            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTLiveApi::VisitsSvcx::ChangePassword")
        End Try

        Return user
    End Function

#End Region

    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function LoginBackground() As BackgroundPhoto
        Dim jbck As New BackgroundPhoto
        Try
            jbck.url = "Q" & VTPortal.CommonHelper.getSeason(Date.Now) & "-" & VTPortal.CommonHelper.RandomGenerator.Next(1, 6) & ".jpg"

            Dim fileName As String = IO.Path.Combine(HostingEnvironment.ApplicationPhysicalPath, "Resources\" + jbck.url)
            jbck.base64 = "data:image/" + IO.Path.GetExtension(fileName).Replace(".", "") + ";base64," + File2base64(fileName)
        Catch ex As Exception
            jbck.result = ResutException
        End Try
        Return jbck

    End Function

    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function lastvisitchange(ByVal filter As String) As Lastchange
        Dim jlast As New Lastchange

        Try
            If Not Global_asax.LoggedIn Then
                jlast.result = "NO_SESSION"
                Return jlast
            End If

            If Not filter Is Nothing AndAlso filter.Length > 0 Then
                jlast.lastdate = roVisitList.lastModification(parsejson(filter))
            Else
                jlast.lastdate = roVisitList.lastModification
            End If
        Catch ex As Exception
            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "Visits::lastvisitchange::" + ex.Message)
        End Try

        Return jlast
    End Function

    <OperationContract()>
    <WebInvoke(Method:="POST", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function visit() As roVisit
        Dim jvisit As New roVisit

        Dim idtype = HttpContext.Current.Request.Form("idtype")
        Dim idvisit = HttpContext.Current.Request.Form("idvisit")
        Dim values = HttpContext.Current.Request.Form("values")
        Try
            If Not Global_asax.LoggedIn Then
                jvisit.result = "NO_SESSION"
                Return jvisit
            End If

            If Not idvisit Is Nothing Then

                Dim vstate As New roVisitState(Global_asax.Identity.ID)
                jvisit = New roVisit(idvisit, idtype, VTPortal.SecurityHelper.GetFeaturePermission(Nothing, "Visits", "U"), vstate)
            Else
                If Not values Is Nothing Then
                    Dim jss As New Script.Serialization.JavaScriptSerializer()
                    jvisit = jss.Deserialize(Of roVisit)(values)
                    'TODO: controlar los permisos para guardar el objeto
                    If jvisit.idvisit = "new" Then
                        jvisit.createdby = Global_asax.Identity.ID
                    End If
                    jvisit.modifiedBy = Global_asax.Identity.ID

                    Dim url = HttpContext.Current.Request.Url.AbsoluteUri.ToString.Split("/")

                    jvisit.save(url(0) + "//" + url(2))
                End If
            End If
        Catch ex As Exception
            jvisit.result = ResutException

            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "Visits::visit::" + ex.Message)
        End Try
        Return jvisit
    End Function

    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function visitfield(ByVal idfield As String, ByVal values As String) As roVisitField
        Dim jvf As New roVisitField

        Try
            If Not Global_asax.LoggedIn Then
                jvf.result = "NO_SESSION"
                Return jvf
            End If
            If Not idfield Is Nothing Then

                Dim vstate As New roVisitField
                Dim ostate As New roVisitState(Global_asax.Identity.ID)
                jvf = New roVisitField(idfield, ostate)
            Else
                If Not values Is Nothing Then
                    Dim jss As New System.Web.Script.Serialization.JavaScriptSerializer()
                    jvf = jss.Deserialize(Of roVisitField)(values)
                    'TODO: controlar los permisos para guardar el objeto
                    jvf.save()
                End If
            End If
        Catch ex As Exception
            jvf.result = ResutException

            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "Visits::visitfield::" + ex.Message)
        End Try
        Return jvf
    End Function

    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function visittype(ByVal idtype As String, ByVal values As String) As roVisitType
        Dim jvf As New roVisitType

        Try
            If Not Global_asax.LoggedIn Then
                jvf.result = "NO_SESSION"
                Return jvf
            End If
            If Not idtype Is Nothing Then
                Dim idtypeint As Integer
                Dim vstate As New roVisitType
                Dim ostate As New roVisitState(Global_asax.Identity.ID)
                If idtype = "new" Then
                    idtypeint = 0
                    jvf = New roVisitType(idtypeint, ostate)
                Else
                    jvf = New roVisitType(idtype, ostate)
                End If
            Else
                If Not values Is Nothing Then
                    Dim jss As New System.Web.Script.Serialization.JavaScriptSerializer()
                    jvf = jss.Deserialize(Of roVisitType)(values)
                    'TODO: controlar los permisos para guardar el objeto
                    jvf.save()
                End If
            End If
        Catch ex As Exception
            jvf.result = ResutException

            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "Visits::visittype::" + ex.Message)
        End Try
        Return jvf
    End Function

    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function visittypeslist() As roVisitTypeList
        Dim jvisittypelst As New roVisitTypeList
        Try
            If Not Global_asax.LoggedIn Then
                jvisittypelst.result = "NO_SESSION"
                Return jvisittypelst
            End If
            jvisittypelst.load()
        Catch ex As Exception
            jvisittypelst.result = ResutException

            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "Visits::visittypelist::" + ex.Message)
        End Try
        Return jvisittypelst
    End Function

    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function visitfilters(ByVal filter As String, ByVal searchparams As String) As roVisitList
        Dim jvisitlst As roVisitList = Nothing

        Try
            If Not Global_asax.LoggedIn Then
                jvisitlst = New roVisitList(-1, Nothing, False) With {.result = "NO_SESSION"}
                Return jvisitlst
            End If

            jvisitlst = New roVisitList(Global_asax.Identity.ID, New roVisitState(Global_asax.Identity.ID), False)

            If Not filter Is Nothing AndAlso filter.Length > 0 Then
                jvisitlst.filter = parsejson(filter)
            End If
            If Not searchparams Is Nothing AndAlso searchparams.Length > 0 Then
                jvisitlst.searchparams = parsejson(searchparams)
            End If
        Catch ex As Exception
            jvisitlst.result = ResutException

            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "Visits::visitfilters::" + ex.Message)
        End Try
        Return jvisitlst
    End Function

    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function visitlist(ByVal filter As String, ByVal searchparams As String) As roVisitList
        Dim jvisitlst As roVisitList = Nothing
        Try
            If Not Global_asax.LoggedIn Then
                jvisitlst = New roVisitList(-1, Nothing, False) With {.result = "NO_SESSION"}
                Return jvisitlst
            End If
            jvisitlst = New roVisitList(Global_asax.Identity.ID, New roVisitState(Global_asax.Identity.ID), False)

            If Not filter Is Nothing AndAlso filter.Length > 0 Then
                jvisitlst.filter = parsejson(filter)
            End If
            If Not searchparams Is Nothing AndAlso searchparams.Length > 0 Then
                jvisitlst.searchparams = parsejson(searchparams)
            End If
            If jvisitlst.searchparams.Count > 0 Then
                If Not jvisitlst.search(Global_asax.Identity.ID) Then
                    roLog.GetInstance().logMessage(roLog.EventType.roError, "Visits::visitList::Filter::" + filter)
                End If
            Else
                If Not jvisitlst.load(Global_asax.Identity.ID) Then
                    roLog.GetInstance().logMessage(roLog.EventType.roError, "Visits::visitList::Filter::" + filter)
                End If
            End If
        Catch ex As Exception

            Dim oLogState As New roBusinessState("Common.BaseState", "")

            oLogState.UpdateStateInfo(ex, "Visits::visitList::" + ex.Message)
        End Try
        Return jvisitlst
    End Function

    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function delvisit(ByVal idvisit As String, ByVal filter As String) As roVisitList
        Dim jvisitlst As roVisitList = Nothing
        Dim oState As New roVisitState(Global_asax.Identity.ID)
        Dim jvisit As New roVisit(idvisit, VTPortal.SecurityHelper.GetFeaturePermission(Nothing, "Visits", "U"), Nothing, oState)

        Try
            If Not Global_asax.LoggedIn Then
                jvisitlst.result = "NO_SESSION"
                Return jvisitlst
            End If
            jvisitlst = New roVisitList(Global_asax.Identity.ID, New roVisitState(Global_asax.Identity.ID), False)

            If jvisit.delete(idvisit, True) Then
                If Not filter Is Nothing AndAlso filter.Length > 0 Then
                    jvisitlst.filter = parsejson(filter)
                End If
                jvisitlst.load(Global_asax.Identity.ID)
            Else
                jvisitlst.result = NOT_FOUND
            End If
        Catch ex As Exception
            jvisitlst.result = ResutException

            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "Visits::delvisit::" + ex.Message)
        End Try
        Return jvisitlst
    End Function

    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function visitfieldslist() As roVisitFieldList
        Dim jvisitfldlst As New roVisitFieldList

        Try
            If Not Global_asax.LoggedIn Then
                jvisitfldlst.result = "NO_SESSION"
                Return jvisitfldlst
            End If
            jvisitfldlst.load()
        Catch ex As Exception
            jvisitfldlst.result = ResutException

            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "Visits::visitfieldlist::" + ex.Message)
        End Try
        Return jvisitfldlst
    End Function

    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function printconfig() As roPrintConfig
        Dim jvisitprintconfig As New roPrintConfig

        Try
            If Not Global_asax.LoggedIn Then
                jvisitprintconfig.result = "NO_SESSION"
                Return jvisitprintconfig
            End If

            jvisitprintconfig.load()
        Catch ex As Exception
            jvisitprintconfig.result = ResutException

            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "Visits::visitfieldlist::" + ex.Message)
        End Try
        Return jvisitprintconfig
    End Function

    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function getCurrentLegalTexts() As roVisitLaws
        Dim jvisitLegalTexts As New roVisitLaws

        Try

            jvisitLegalTexts.load()
        Catch ex As Exception
            jvisitLegalTexts.result = ResutException

            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "Visits::visitfieldlist::" + ex.Message)
        End Try
        Return jvisitLegalTexts
    End Function

    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function delvisitfield(ByVal idfield As String) As roVisitFieldList
        Dim jvisitfldlst As New roVisitFieldList

        Try
            If Not Global_asax.LoggedIn Then
                jvisitfldlst.result = "NO_SESSION"
                Return jvisitfldlst
            End If

            roVisitField.DeleteVisitField(idfield)
            jvisitfldlst.load()
        Catch ex As Exception
            jvisitfldlst.result = ResutException

            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "Visits::delvisitfield::" + ex.Message)
        End Try
        Return jvisitfldlst
    End Function

    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function delvisittype(ByVal idtype As String) As roVisitTypeList
        Dim jvisitfldlst As New roVisitTypeList

        Try
            If Not Global_asax.LoggedIn Then
                jvisitfldlst.result = "NO_SESSION"
                Return jvisitfldlst
            End If

            roVisitType.DeleteVisitType(idtype)
            jvisitfldlst.load()
        Catch ex As Exception
            jvisitfldlst.result = ResutException

            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "Visits::delvisittype::" + ex.Message)
        End Try
        Return jvisitfldlst
    End Function

    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function upvisitfield(ByVal idfield As String) As roVisitFieldList
        Dim jvisitfldlst As New roVisitFieldList

        Try
            If Not Global_asax.LoggedIn Then
                jvisitfldlst.result = "NO_SESSION"
                Return jvisitfldlst
            End If

            If roVisitField.upVisitField(idfield) Then
                jvisitfldlst.load()
            End If
        Catch ex As Exception
            jvisitfldlst.result = ResutException

            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "Visits::upvisitfield::" + ex.Message)
        End Try
        Return jvisitfldlst
    End Function

    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function downvisitfield(ByVal idfield As String) As roVisitFieldList
        Dim jvisitfldlst As New roVisitFieldList

        Try
            If Not Global_asax.LoggedIn Then
                jvisitfldlst.result = "NO_SESSION"
                Return jvisitfldlst
            End If

            If roVisitField.downVisitField(idfield) Then
                jvisitfldlst.load()
            End If
        Catch ex As Exception
            jvisitfldlst.result = ResutException

            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "Visits::downvisitfield::" + ex.Message)
        End Try
        Return jvisitfldlst
    End Function

    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function visitor(ByVal idvisitor As String, ByVal values As String) As roVisitor
        Dim jvisitor = New roVisitor

        Try

            If Not Global_asax.LoggedIn Then
                jvisitor.result = "NO_SESSION"
                Return jvisitor
            End If

            If Not idvisitor Is Nothing Then
                Dim vstate As New roVisitorState(Global_asax.Identity.ID)
                jvisitor = New roVisitor(idvisitor, VTPortal.SecurityHelper.GetFeaturePermission(Nothing, "Visits", "U"), vstate)
            Else
                If Not values Is Nothing Then
                    Dim jss As New System.Web.Script.Serialization.JavaScriptSerializer()
                    jvisitor = jss.Deserialize(Of roVisitor)(values)
                    'TODO: controlar los permisos para guardar el objeto
                    jvisitor.save()
                End If
            End If
        Catch ex As Exception
            jvisitor.result = ResutException

            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "Visits::visitor::" + ex.Message)
        End Try
        Return jvisitor
    End Function

    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function visitorfield(ByVal idfield As String, ByVal values As String) As roVisitorField
        Dim jvf As New roVisitorField()

        Try

            If Not Global_asax.LoggedIn Then
                jvf.result = "NO_SESSION"
                Return jvf
            End If
            If Not idfield Is Nothing Then

                Dim vstate As New roVisitField
                Dim ostate As New roVisitorState(Global_asax.Identity.ID)
                jvf = New roVisitorField(idfield, ostate)
            Else
                If Not values Is Nothing Then
                    Dim jss As New Script.Serialization.JavaScriptSerializer()
                    jvf = jss.Deserialize(Of roVisitorField)(values)
                    'TODO: controlar los permisos para guardar el objeto
                    jvf.save()
                End If
            End If
        Catch ex As Exception
            jvf.result = ResutException

            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "Visits::visitorfield::" + ex.Message)
        End Try
        Return jvf
    End Function

    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function printfields(ByVal values As String) As roPrintConfig

        Dim jprintConfig As New roPrintConfig

        Try
            If Not Global_asax.LoggedIn Then
                jprintConfig.result = "NO_SESSION"
                Return jprintConfig
            End If
            If Not values Is Nothing Then
                Dim jss As New System.Web.Script.Serialization.JavaScriptSerializer()
                Dim joptions = New roPrintConfig
                joptions = jss.Deserialize(Of roPrintConfig)(values)
                'TODO: controlar los permisos para guardar el objeto
                jprintConfig.getPrintConfig(joptions)
            End If
        Catch ex As Exception

            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "Visits::options::" + ex.Message)
        End Try
        Return jprintConfig
    End Function

    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function visitorlist(ByVal filter As String) As roVisitorList
        Dim jvisitorlst As New roVisitorList

        Try
            If Not Global_asax.LoggedIn Then
                jvisitorlst.result = "NO_SESSION"
                Return jvisitorlst
            End If
            Dim vstate As New roVisitorState(Global_asax.Identity.ID)
            If Not filter Is Nothing AndAlso filter.Length > 0 Then
                jvisitorlst.filter = parsejson(filter)
            End If
            jvisitorlst.load(, vstate)
        Catch ex As Exception
            jvisitorlst.result = ResutException

            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "Visits::visitorlist::" + ex.Message)
        End Try
        Return jvisitorlst
    End Function

    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function delvisitor(ByVal idvisitor As String, ByVal filter As String) As roVisitorList
        Dim jvisitorlst As New roVisitorList

        Try
            If Not Global_asax.LoggedIn Then
                jvisitorlst.result = "NO_SESSION"
                Return jvisitorlst
            End If
            If roVisitor.delete(idvisitor) Then
                If Not filter Is Nothing AndAlso filter.Length > 0 Then
                    jvisitorlst.filter = parsejson(filter)
                End If
                Dim vstate As New roVisitorState(Global_asax.Identity.ID)
                jvisitorlst.load(, vstate)
            Else
                jvisitorlst.result = NOT_FOUND
            End If
        Catch ex As Exception
            jvisitorlst.result = ResutException

            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "Visits::delvisitor::" + ex.Message)
        End Try
        Return jvisitorlst
    End Function

    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function visitorfieldslist(ByVal filter As String) As roVisitorFieldList
        Dim jvisitorfldlst As New roVisitorFieldList
        Try
            If Not Global_asax.LoggedIn Then
                jvisitorfldlst.result = "NO_SESSION"
                Return jvisitorfldlst
            End If
            jvisitorfldlst.load()
        Catch ex As Exception
            jvisitorfldlst.result = ResutException

            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "Visits::visitorfieldslist::" + ex.Message)
        End Try
        Return jvisitorfldlst
    End Function

    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function delvisitorfield(ByVal idfield As String) As roVisitorFieldList
        Dim jvisitorfldlst As New roVisitorFieldList
        Try
            If Not Global_asax.LoggedIn Then
                jvisitorfldlst.result = "NO_SESSION"
                Return jvisitorfldlst
            End If
            roVisitorField.DeleteVisitField(idfield)
            jvisitorfldlst.load()
        Catch ex As Exception
            jvisitorfldlst.result = ResutException

            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "Visits::delvisitorfield::" + ex.Message)
        End Try
        Return jvisitorfldlst
    End Function

    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function upvisitorfield(ByVal idfield As String) As roVisitorFieldList
        Dim jvisitorfldlst As New roVisitorFieldList

        Try
            If Not Global_asax.LoggedIn Then
                jvisitorfldlst.result = "NO_SESSION"
                Return jvisitorfldlst
            End If
            If roVisitorField.upVisitorField(idfield) Then
                jvisitorfldlst.load()
            End If
        Catch ex As Exception
            jvisitorfldlst.result = ResutException

            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "Visits::upvisitorfield::" + ex.Message)
        End Try
        Return jvisitorfldlst
    End Function

    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function downvisitorfield(ByVal idfield As String) As roVisitorFieldList
        Dim jvisitorfldlst As New roVisitorFieldList

        Try
            If Not Global_asax.LoggedIn Then
                jvisitorfldlst.result = "NO_SESSION"
                Return jvisitorfldlst
            End If
            If roVisitorField.downVisitorField(idfield) Then
                jvisitorfldlst.load()
            End If
        Catch ex As Exception
            jvisitorfldlst.result = ResutException

            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "Visits::downvisitorfield::" + ex.Message)
        End Try
        Return jvisitorfldlst
    End Function

    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function employeelist(ByVal filter As String) As roEmployeeList
        Dim jemplst As New roEmployeeList

        Try
            If Not Global_asax.LoggedIn Then
                jemplst.result = "NO_SESSION"
                Return jemplst
            End If
            If Not filter Is Nothing AndAlso filter.Length > 0 Then
                jemplst.filter = parsejson(filter)
            End If
            jemplst.load(Global_asax.Identity.ID)
        Catch ex As Exception
            jemplst.result = ResutException

            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "Visits::employeelist::" + ex.Message)
        End Try
        Return jemplst
    End Function

    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function employeestatus(ByVal idemployee As Integer, ByVal filter As String) As roJSON_Integer
        Dim response As New roJSON_Integer
        'Dim session As SessionObject = SessionManagement.CheckSession(token)
        'If session Is Nothing Then
        '    response.result = "NO_SESSION"
        '    Return response
        'End If
        Try
            If Not Global_asax.LoggedIn Then
                response.result = "NO_SESSION"
                Return response
            End If
            response.value = roEmployeeDB.getEmployeeStatus(idemployee)
            response.result = "Ok"
        Catch ex As Exception
            response.result = ResutException

            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "Visits::employeestatus::" + ex.Message)
        End Try
        Return response
    End Function

    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function options(ByVal values As String) As roOptions
        Dim optSt As New roOptionsState
        Dim joptions As New roOptions(optSt)

        Try
            If Not Global_asax.LoggedIn Then
                joptions.result = "NO_SESSION"
                Return joptions
            End If
            If Not values Is Nothing Then
                Dim jss As New System.Web.Script.Serialization.JavaScriptSerializer()
                Dim joptions2 = New roOptions(optSt)
                joptions2 = jss.Deserialize(Of roOptions)(values)
                'TODO: controlar los permisos para guardar el objeto
                joptions.save(joptions2)
            End If
        Catch ex As Exception
            joptions.result = ResutException

            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "Visits::options::" + ex.Message)
        End Try
        Return joptions
    End Function

    <OperationContract()>
    <WebInvoke(Method:="POST", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function saveVisitLaws() As roVisitLaws

        Dim jvisitLaws As New roVisitLaws

        Try
            If Not Global_asax.LoggedIn Then
                jvisitLaws.result = "NO_SESSION"
                Return jvisitLaws
            End If

            jvisitLaws.title1 = HttpUtility.HtmlDecode(HttpUtility.UrlDecode(HttpContext.Current.Request.Form("title1")))
            jvisitLaws.value1 = HttpUtility.HtmlDecode(HttpUtility.UrlDecode(HttpContext.Current.Request.Form("value1")))
            jvisitLaws.title2 = HttpUtility.HtmlDecode(HttpUtility.UrlDecode(HttpContext.Current.Request.Form("title2")))
            jvisitLaws.value2 = HttpUtility.HtmlDecode(HttpUtility.UrlDecode(HttpContext.Current.Request.Form("value2")))

            If Not jvisitLaws Is Nothing Then
                jvisitLaws.saveVisitLaws(jvisitLaws)
            End If
        Catch ex As Exception

            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "Visits::options::" + ex.Message)
        End Try
        Return jvisitLaws
    End Function

    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function savePrintConfig(ByVal values As String) As roPrintConfig

        Dim jprintConfig As New roPrintConfig

        Try
            If Not Global_asax.LoggedIn Then
                jprintConfig.result = "NO_SESSION"
                Return jprintConfig
            End If
            If Not values Is Nothing Then
                Dim jss As New System.Web.Script.Serialization.JavaScriptSerializer()
                Dim joptions = New roPrintConfig
                joptions = jss.Deserialize(Of roPrintConfig)(values)
                'TODO: controlar los permisos para guardar el objeto
                jprintConfig.savePrintConfig(joptions)
            End If
        Catch ex As Exception

            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "Visits::options::" + ex.Message)
        End Try
        Return jprintConfig
    End Function

    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function zoneListByWorkingOut() As roZoneList
        Dim optSt As New roZoneState
        Dim jzone As New roZone(optSt)
        Dim jzoneList As New roZoneList

        Try
            If Not Global_asax.LoggedIn Then
                jzoneList.result = "NO_SESSION"
                Return jzoneList
            End If
            jzone.FindAllByWorkingZone(0)
            jzoneList = jzone.Zone
        Catch ex As Exception
            jzoneList.result = ResutException

            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "Visits::zoneListByWorkingOut::" + ex.Message)
        End Try
        Return jzoneList
    End Function

    <OperationContract()>
    <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function visitors(ByVal idvisit As String, ByVal values As String) As roVisitor
        Dim jvisitor = New roVisitor

        Try

            If values Is Nothing Then
                Dim sIDvisit As String = roVisit.getIDbyUniqueID(idvisit)
                If sIDvisit.Length > 0 Then
                    jvisitor = New roVisitor("new", Nothing, Nothing)
                Else
                    sIDvisit = roVisitor.getIDbyUniqueID(idvisit)
                    If sIDvisit.Length > 0 Then
                        jvisitor = New roVisitor(sIDvisit, Nothing, Nothing)
                    Else
                        jvisitor.result = "NotFound"
                    End If

                End If
            Else
                If Not values Is Nothing Then
                    Dim jss As New System.Web.Script.Serialization.JavaScriptSerializer()
                    jvisitor = jss.Deserialize(Of roVisitor)(values)
                    'TODO: controlar los permisos para guardar el objeto
                    jvisitor.save()
                    Dim sIDvisit As String = roVisit.getIDbyUniqueID(idvisit)
                    If sIDvisit.Length > 0 Then
                        roVisit.saveVisitorOnVisit(sIDvisit, jvisitor.idvisitor)
                        'Else
                        '    jvisitor.result = "NotFound"
                    End If
                End If
            End If
        Catch ex As Exception
            jvisitor.result = ResutException

            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "Visits::visitor::" + ex.Message)
        End Try
        Return jvisitor
    End Function

End Class