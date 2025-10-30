Imports DevExpress.CodeParser
Imports Microsoft.Web.Services3.Xml
Imports Robotics.Azure
Imports Robotics.Base.DTOs
Imports Robotics.Security.Base
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions
Imports Robotics.Web.Base

Public Class VTLiveAuth
    Inherits PageBase

    Private oLng As roLanguage

    <Runtime.Serialization.DataContract()>
    Private Class AdfsUser

        <Runtime.Serialization.DataMember(Name:="UserName")>
        Public UserName As String

        <Runtime.Serialization.DataMember(Name:="Token")>
        Public Token As String

    End Class

    Public Property ADFSUserName As String
    Public Property ADFSUserNameError As String

    Public ReadOnly Property SSOConfigVersion() As Integer
        Get
            Return roTypes.Any2Integer(HelperSession.AdvancedParametersCache("VisualTime.SSO.ConfigurationVersion"))
        End Get
    End Property


    Public Property SSOLoginRetries As Integer
        Get
            If Web.HttpContext.Current.Session("SSO_RETRYCOUNTER") Is Nothing Then
                Return 0
            Else
                Return roTypes.Any2Integer(Web.HttpContext.Current.Session("SSO_RETRYCOUNTER"))
            End If
        End Get
        Set(value As Integer)
            Web.HttpContext.Current.Session("SSO_RETRYCOUNTER") = value
        End Set
    End Property


    Public Property SSOLoginLastRetry As DateTime
        Get
            If Web.HttpContext.Current.Session("SSOL_LASTRETRY") Is Nothing Then
                Return DateTime.MinValue
            Else
                Return roTypes.Any2DateTime(Web.HttpContext.Current.Session("SSOL_LASTRETRY"))
            End If
        End Get
        Set(value As DateTime)
            Web.HttpContext.Current.Session("SSOL_LASTRETRY") = value
        End Set
    End Property




    Public Sub LoadAdfsUserName()
        Dim oParamSSOType As String = HelperSession.AdvancedParametersCache("SSOType")

        If oParamSSOType.ToUpper().StartsWith("CEGIDID") Then
            Dim user = Web.HttpContext.Current.Session("TokenManager.TokenType.UserInfo")

            Dim sEmail As String = CommonClaim.GetClaimValue(user.claims, "email")
            Dim sToken As String = CommonClaim.GetClaimValue(user.claims, "sub")

            sToken = New System.Guid(sToken).ToString("N").ToLower()

            Dim bexistToken As Boolean = API.UserAdminServiceMethods.CredentialExists(Nothing, $"\{sToken}", AuthenticationMethod.IntegratedSecurity, "", Nothing)

            ADFSUserNameError = ""
            If bexistToken Then
                ADFSUserName = sToken
            Else
                Dim dtEmployees As List(Of roPassportTicket) = API.SecurityV3ServiceMethods.GetPassportsByEmail(Nothing, sEmail)

                ADFSUserName = String.Empty
                ADFSUserNameError = String.Empty

                If dtEmployees Is Nothing Then ADFSUserNameError = Me.oLng.Translate("cegidid.Error.couldnotqueryemployees", Me.DefaultScope)

                If dtEmployees IsNot Nothing Then
                    If dtEmployees.Count = 1 Then
                        'guardar la credencial al empleado
                        Dim idPassport As Integer = roTypes.Any2Integer(dtEmployees(0).ID)

                        Dim oPassport As roPassport = API.UserAdminServiceMethods.GetPassport(Nothing, idPassport, LoadType.Passport)

                        Dim bAssign As Boolean = False
                        If oPassport IsNot Nothing Then
                            Dim oMethod = oPassport.AuthenticationMethods.IntegratedSecurityRow
                            If oMethod Is Nothing Then
                                oMethod = New roPassportAuthenticationMethodsRow
                                With oMethod
                                    .IDPassport = oPassport.ID
                                    .Method = AuthenticationMethod.IntegratedSecurity
                                    .Version = ""
                                    .BiometricID = 0
                                    .Enabled = True
                                End With
                            End If

                            oMethod.RowState = RowState.UpdateRow
                            oMethod.LastUpdatePassword = DateTime.Now()
                            oMethod.Credential = $"\{sToken}"
                            oMethod.Password = CryptographyHelper.EncryptMD5($"\{sToken}".ToLower())

                            oPassport.AuthenticationMethods.IntegratedSecurityRow = oMethod
                            bAssign = API.UserAdminServiceMethods.SavePassport(Nothing, oPassport)
                        End If

                        If bAssign Then ADFSUserName = sToken

                    ElseIf dtEmployees.Count > 1 Then
                        ADFSUserName = String.Empty
                        ADFSUserNameError = Me.oLng.Translate("cegidid.Error.emailisnotunique", Me.DefaultScope)
                        roLog.GetInstance().logMessage(roLog.EventType.roError, $"CommonClaim::LoadAdfsUserName::Email is not unique({roTypes.Any2String(HttpContext.Current.Session("roMultiCompanyId"))}:{sEmail})")
                    Else
                        ADFSUserName = String.Empty
                        ADFSUserNameError = Me.oLng.Translate("cegidid.Error.usernotexists", Me.DefaultScope)
                        roLog.GetInstance().logMessage(roLog.EventType.roError, $"CommonClaim::LoadAdfsUserName::Email not found({roTypes.Any2String(HttpContext.Current.Session("roMultiCompanyId"))}:{sEmail})")
                    End If
                End If
            End If
        Else
            ADFSUserName = CommonClaim.GetAuthenticationClaim(oParamSSOType, SSOConfigVersion, Request.GetOwinContext())
        End If
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.InsertExtraJavascript("BrowserDetect", "~/Base/Scripts/BrowserDetect.js", , True)
        Me.InsertExtraJavascript("Cookies", "~/Base/Scripts/Cookies.js", , True)
        Me.InsertExtraJavascript("Generic", "~/Base/Scripts/Generic.js", , True)
        Me.InsertExtraJavascript("Generic", "~/Scripts/login.js", , True)

        Me.InsertCssIncludes(Me.Page)

        oLng = New roLanguage()
        oLng.SetLanguageReference("LivePortal", "ESP")

        If Not IsPostBack Then
            Dim backgroundImage As String = "Q" & HelperWeb.getSeason(Date.Now) & "-" & HelperWeb.RandomGenerator.Next(1, 6) & ".jpg"
            Me.rbBackground.Style("background-image") = "url(../Base/Images/LoginBackground/" & backgroundImage & ");"
        End If
        Dim isApp As String = Request.Params("isApp")

        If AuthValidations.IsAlreadyLoggedOnAnotherCompany() Then
            Me.lblResult.Text = Me.oLng.Translate("Adfs.Error.SystemNotConfigured", Me.DefaultScope)
            Me.errorRow.Style("Display") = ""

            Me.hdnAdfsUserName.Value = "unknown(-1)"
            Me.hdnAdfsToken.Value = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("1#unknown#" & HttpContext.Current.Session("roMultiCompanyId")))

            If isApp IsNot Nothing AndAlso Not roTypes.Any2Boolean(isApp) Then
                Robotics.Web.Base.HelperWeb.EraseCookie("VTPortalAdfsUserName")
                Robotics.Web.Base.HelperWeb.CreateCookie("VTPortalAdfsUserName", Me.hdnAdfsUserName.Value, False)

                Robotics.Web.Base.HelperWeb.EraseCookie("VTPortalToken")
                Robotics.Web.Base.HelperWeb.CreateCookie("VTPortalToken", Me.hdnAdfsToken.Value, False)
                Response.Redirect(Configuration.VTPortalAppUrl & "/index.aspx")
            End If

            Return
        End If


        Dim sIDCompany As String = HttpContext.Current.Session("roMultiCompanyId")
        Dim requestPaths As String() = Request.Url.LocalPath.Split("/")

        If requestPaths(1).ToUpper = "AUTH" AndAlso requestPaths.Length = 3 Then
            sIDCompany = requestPaths(2)
        ElseIf roTypes.Any2String(Request.Params("referer")) <> String.Empty Then
            sIDCompany = roTypes.Any2String(Request.Params("referer"))
        End If
        HttpContext.Current.Session("roMultiCompanyId") = sIDCompany.ToLower().Trim()

        If Not String.IsNullOrEmpty(sIDCompany) Then

            Global_asax.ReloadSharedData()

            Dim sourceApp As String = roTypes.Any2String(Request.Params("source"))
            Dim oParamSSOType As String = HelperSession.AdvancedParametersCache("SSOType")
            Dim iMaxRetries As Integer = roTypes.Any2Integer(HelperSession.AdvancedParametersCache("SSO.LoginRetriesAvailable"))
            Dim iResetTime As Integer = roTypes.Any2Integer(HelperSession.AdvancedParametersCache("SSO.RetriesResetTime"))
            If iMaxRetries <= 0 Then iMaxRetries = 3
            If iResetTime <= 0 Then iResetTime = 2

            If oParamSSOType <> String.Empty AndAlso WLHelperWeb.ADFSEnabled Then

                Dim bLoggedIn As Boolean = False
                Dim bLoginRetriesExceeded As Boolean = False

                Dim now As DateTime = DateTime.Now
                If Me.SSOLoginLastRetry = DateTime.MinValue OrElse now.Subtract(Me.SSOLoginLastRetry).TotalMinutes < iResetTime Then
                    If Me.SSOLoginRetries >= iMaxRetries Then
                        bLoginRetriesExceeded = True
                    Else
                        Me.SSOLoginRetries += 1
                    End If
                Else
                    Me.SSOLoginRetries = 1
                End If

                If Not bLoginRetriesExceeded Then
                    Me.SSOLoginLastRetry = now

                    If oParamSSOType.ToUpper().StartsWith("CEGIDID") Then
                        bLoggedIn = (Web.HttpContext.Current.Session("TokenManager.TokenType.UserInfo") IsNot Nothing)
                    Else
                        Try
                            Dim sCookieName As String = "ExternalCookie"
                            If SSOConfigVersion = 2 Then sCookieName = "clientscheme_" & HttpContext.Current.Session("roMultiCompanyId")

                            If Request.GetOwinContext().Authentication.AuthenticateAsync(sCookieName).Result IsNot Nothing Then
                                bLoggedIn = Request.GetOwinContext().Authentication.AuthenticateAsync(sCookieName).Result.Identity.IsAuthenticated
                            End If
                        Catch ex As Exception
                            bLoggedIn = False
                        End Try
                    End If

                    If Not bLoggedIn Then
                        Dim urlParams As String = String.Empty
                        If sourceApp.ToUpper = "VTPORTAL" AndAlso isApp IsNot Nothing Then
                            urlParams = IIf(roTypes.Any2Boolean(isApp), "?source=VTPORTAL&isApp=1&referer=" & HttpContext.Current.Session("roMultiCompanyId"), "?source=VTPORTAL&isApp=0&referer=" & HttpContext.Current.Session("roMultiCompanyId"))
                        End If

                        Me.hdnAdfsUserName.Value = String.Empty
                        Me.hdnAdfsToken.Value = String.Empty

                        If oParamSSOType.ToUpper().StartsWith("CEGIDID") Then
                            Dim url As String = $"{Robotics.Web.Base.Configuration.LiveDesktopAppUrl}sso/cegidIdLogin?id={sIDCompany}"
                            If urlParams <> String.Empty Then url = $"{url}&{urlParams.Substring(1)}"
                            HttpContext.Current.Response.Redirect(url, False)
                            HttpContext.Current.ApplicationInstance.CompleteRequest()
                        Else
                            CommonAuth.RedirectToLoginIfNecesary(HttpContext.Current.Session("roMultiCompanyId"), oParamSSOType, Me.Context.GetOwinContext(), Robotics.Web.Base.Configuration.LiveDesktopAppUrl & "Auth/" & sIDCompany & urlParams)
                        End If

                    Else
                        Me.SSOLoginRetries = 0

                        If Not WLHelperWeb.ADFSEnabled Then
                            WLHelperWeb.RedirectToUrl(Robotics.Web.Base.Configuration.LiveDesktopAppUrl)
                        Else
                            LoadAdfsUserName()

                            Select Case sourceApp.ToUpper
                                Case "VTLIVE"
                                    SSOLoginOnVTLive()
                                Case "VTPORTAL"
                                    SSOLoginOnVTPortal(isApp)
                                Case Else
                                    SSOLoginOnVTLive()
                            End Select
                        End If
                    End If
                Else
                    Me.lblResult.Text = Me.oLng.Translate("Adfs.Error.RetriesExceeded", Me.DefaultScope)
                    Me.errorRow.Style("Display") = ""

                    Me.hdnAdfsUserName.Value = "unknown(-1)"
                    Me.hdnAdfsToken.Value = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("1#unknown#" & HttpContext.Current.Session("roMultiCompanyId")))

                    If isApp IsNot Nothing AndAlso Not roTypes.Any2Boolean(isApp) Then
                        Robotics.Web.Base.HelperWeb.EraseCookie("VTPortalAdfsUserName")
                        Robotics.Web.Base.HelperWeb.CreateCookie("VTPortalAdfsUserName", Me.hdnAdfsUserName.Value, False)

                        Robotics.Web.Base.HelperWeb.EraseCookie("VTPortalToken")
                        Robotics.Web.Base.HelperWeb.CreateCookie("VTPortalToken", Me.hdnAdfsToken.Value, False)
                        Response.Redirect(Configuration.VTPortalAppUrl & "/index.aspx")
                    End If
                End If


            Else
                Me.lblResult.Text = Me.oLng.Translate("Adfs.Error.SystemNotConfigured", Me.DefaultScope)
                Me.errorRow.Style("Display") = ""

                Me.hdnAdfsUserName.Value = "unknown(-1)"
                Me.hdnAdfsToken.Value = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("1#unknown#" & HttpContext.Current.Session("roMultiCompanyId")))

                If isApp IsNot Nothing AndAlso Not roTypes.Any2Boolean(isApp) Then
                    Robotics.Web.Base.HelperWeb.EraseCookie("VTPortalAdfsUserName")
                    Robotics.Web.Base.HelperWeb.CreateCookie("VTPortalAdfsUserName", Me.hdnAdfsUserName.Value, False)

                    Robotics.Web.Base.HelperWeb.EraseCookie("VTPortalToken")
                    Robotics.Web.Base.HelperWeb.CreateCookie("VTPortalToken", Me.hdnAdfsToken.Value, False)
                    Response.Redirect(Configuration.VTPortalAppUrl & "/index.aspx")
                End If
            End If

        End If

    End Sub

#Region "Live Desktop"

    Protected Sub SSOLoginOnVTLive()
        Dim bolAuthenticated As Boolean = False

        Me.hdnAdfsUserName.Value = "unknown(-1)"
        Me.hdnAdfsToken.Value = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("1#unknown#" & HttpContext.Current.Session("roMultiCompanyId")))
        Dim oParamSSOType As String = HelperSession.AdvancedParametersCache("SSOType")


        If ADFSUserName <> String.Empty Then
            ' Try to login.
            Dim Pass As roPassportTicket = Nothing

            Dim strPassword As String = ""
            Dim ssoLogin As Boolean = True

            Dim oAuthType As AuthenticationMethod = AuthenticationMethod.Password
            If oParamSSOType.Trim.ToUpper = "CEGIDID" Then oAuthType = AuthenticationMethod.IntegratedSecurity



            Pass = API.SecurityServiceMethods.Authenticate(Me.Page, Nothing, oAuthType, "\" & ADFSUserName, strPassword, True, , , , , "", , ssoLogin, True)

            If Pass IsNot Nothing Then
                roWsUserManagement.SessionObject.AccessApi.InitWebServices()

                If Not API.SecurityServiceMethods.SetLanguage(Me.Page, Pass.ID, WLHelperWeb.GetLastLanguageUsed()) Then bolAuthenticated = False

                Dim oContext As WebCContext = WLHelperWeb.Context(HttpContext.Current.Request, Pass.ID, , True)
                oContext.Password = strPassword

                API.SecurityServiceMethods.UpdateLastLogin(Pass.ID)

                If ADFSUserNameError <> String.Empty Then
                    Me.lblResult.Text = $"{ADFSUserNameError}"
                Else
                    Me.lblResult.Text = $"{Me.oLng.Translate("Adfs.Error.LoggedInSuccessfully", Me.DefaultScope)}"
                End If

                If Not oParamSSOType.ToUpper().StartsWith("CEGIDID") Then
                    Me.lblResult.Text = Me.lblResult.Text & " (" & oParamSSOType & ")"
                End If

                WLHelperWeb.CommitLogin(Pass, "\" & ADFSUserName, oAuthType)
                WLHelperWeb.SetSessionContext(oContext)

                bolAuthenticated = True

                HelperWeb.EraseCookie("Login_UserName")

                HelperWeb.EraseCookie("Login_CompanyName")
                HelperWeb.CreateCookie("Login_CompanyName", HttpContext.Current.Session("roMultiCompanyId"))
                HelperWeb.CreateCookie("LastLoginOnSSO", "true")
            End If

            If Not bolAuthenticated Then

                roLog.GetInstance().logSystemMessage(roLog.EventType.roInfo, $"SSO login denied on VTLive for company {HttpContext.Current.Session("roMultiCompanyId")} with user {ADFSUserName}")

                If ADFSUserNameError <> String.Empty Then
                    Me.lblResult.Text = $"{ADFSUserNameError}"
                Else
                    Me.lblResult.Text = $"{roWsUserManagement.SessionObject().States.SecurityState.ErrorText}"
                End If

                If Not oParamSSOType.ToUpper().StartsWith("CEGIDID") Then
                    Me.lblResult.Text = Me.lblResult.Text & " (" & oParamSSOType & ")"
                End If

                Me.errorRow.Style("Display") = ""

                Robotics.Web.Base.HelperWeb.EraseCookie("SSOCompanyId")
                HttpContext.Current.Session("roMultiCompanyId") = String.Empty

                WLHelperWeb.CurrentPassport = Nothing
            Else
                roWsUserManagement.SessionObject.AccessApi.InitWebServices()
                WLHelperWeb.RedirectToUrl(Robotics.Web.Base.Configuration.LiveDesktopAppUrl & "Default.aspx")
            End If
        Else
            If ADFSUserNameError <> String.Empty Then
                Me.lblResult.Text = $"{ADFSUserNameError}"
            Else
                Me.lblResult.Text = Me.oLng.Translate("Adfs.Error.UserNotRecognized", Me.DefaultScope)
            End If
            Robotics.Web.Base.HelperWeb.EraseCookie("SSOCompanyId")
            HttpContext.Current.Session("roMultiCompanyId") = String.Empty
            'Mensaje de error
            Me.errorRow.Style("Display") = ""
        End If

    End Sub

    Protected Sub SSOLoginOnVTPortal(ByVal isApp As String)
        Dim oUser As New AdfsUser() With {.UserName = "", .Token = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("1#unknown#" & HttpContext.Current.Session("roMultiCompanyId")))}
        Dim oParamSSOType As String = HelperSession.AdvancedParametersCache("SSOType")

        If ADFSUserName <> String.Empty Then
            Try
                Dim ostate As New roSecurityState(-2)
                WebServiceHelper.SetSSOVTPortalState(ostate)
                Dim oToken As String = String.Empty
                Dim nGUID As String = Guid.NewGuid.ToString()
                Dim Pass As roPassportTicket = Nothing

                Dim oAuthType As AuthenticationMethod = AuthenticationMethod.Password
                If oParamSSOType.Trim.ToUpper = "CEGIDID" Then oAuthType = AuthenticationMethod.IntegratedSecurity

                Pass = roPassportManager.ValidateCredentials(oAuthType, "\" & ADFSUserName, "\" & ADFSUserName.ToLower, True, "", True, ostate)

                oUser.UserName = ADFSUserName

                If Pass IsNot Nothing Then
                    WebServiceHelper.SetSSOVTPortalState(ostate, Pass.ID)

                    Pass = AuthHelper.Authenticate(Pass, oAuthType, "\" & ADFSUserName, "\" & ADFSUserName.ToLower, True, ostate, roTypes.Any2Boolean(isApp), "", "", "", True, nGUID, oToken, True)

                    If Pass IsNot Nothing AndAlso ostate.Result = SecurityResultEnum.NoError AndAlso Not String.IsNullOrEmpty(oToken) Then
                        Dim strSecCode As String = nGUID & "#" & oToken & "#" & HttpContext.Current.Session("roMultiCompanyId")

                        oUser.Token = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(strSecCode))
                        oUser.UserName = ADFSUserName

                        Me.lblResult.Text = $"{Me.oLng.Translate("Adfs.Error.LoggedInSuccessfully", Me.DefaultScope)}({ADFSUserName})"
                    Else
                        If ADFSUserNameError <> String.Empty Then
                            Me.lblResult.Text = $"{ADFSUserNameError}"
                        Else
                            Me.lblResult.Text = $"{ostate.ErrorText}"  'Me.oLng.Translate("Adfs.Error.UserNotExistsOrAllowed", Me.DefaultScope) & "(" & ADFSUserName & ")" & "(" & ostate.ErrorText & ")"
                        End If


                        If Not oParamSSOType.ToUpper().StartsWith("CEGIDID") Then
                            Me.lblResult.Text = Me.lblResult.Text & " (" & oParamSSOType & ")"
                        End If

                        oUser.UserName = $"{ADFSUserName}({CInt(ostate.Result)})"
                    End If
                Else
                    Dim tbParameters As System.Data.DataTable = ostate.CreateAuditParameters()
                    ostate.AddAuditParameter(tbParameters, "{ErrorText}", ostate.Result.ToString, "", 1)
                    ostate.Audit(Robotics.VTBase.Audit.Action.aConnectFail, Robotics.VTBase.Audit.ObjectType.tConnection, ADFSUserName, tbParameters, -1)

                    roLog.GetInstance().logSystemMessage(roLog.EventType.roInfo, $"SSO login denied on VTPortal for company {HttpContext.Current.Session("roMultiCompanyId")} with user {ADFSUserName}")

                    oUser.UserName = ADFSUserName & "(not found)"
                    If ADFSUserNameError <> String.Empty Then
                        Me.lblResult.Text = $"{ADFSUserNameError}"
                    Else
                        Me.lblResult.Text = Me.oLng.Translate("Adfs.Error.UserNotExistsOrAllowed", Me.DefaultScope) & "(Unknown error)"
                    End If

                    If Not oParamSSOType.ToUpper().StartsWith("CEGIDID") Then
                        Me.lblResult.Text = Me.lblResult.Text & " (" & oParamSSOType & ")"
                    End If

                    roLog.GetInstance().logMessage(roLog.EventType.roDebug, "VTLogin::VTPortalLogin::Could not validate credentials")
                End If
            Catch ex As Exception
                oUser.UserName = ADFSUserName
                roLog.GetInstance().logMessage(roLog.EventType.roError, "VTLogin::VTPortalLogin::Error requesting portal credentials::", ex)
            End Try
        Else
            Me.hdnAdfsUserName.Value = "unknown(-1)"
            If ADFSUserNameError <> String.Empty Then
                Me.lblResult.Text = $"{ADFSUserNameError}"
            Else
                Me.lblResult.Text = Me.oLng.Translate("Adfs.Error.UserNotRecognized", Me.DefaultScope)
            End If
            Me.hdnAdfsToken.Value = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("1#unknown#" & HttpContext.Current.Session("roMultiCompanyId")))
        End If

        Me.hdnAdfsUserName.Value = oUser.UserName
        Me.hdnAdfsToken.Value = oUser.Token

        If isApp IsNot Nothing AndAlso Not roTypes.Any2Boolean(isApp) Then
            Response.Redirect(Configuration.VTPortalAppUrl & "index.aspx?token=" & Server.UrlEncode(Me.hdnAdfsToken.Value) & "&userName=" & Server.UrlEncode(Me.hdnAdfsUserName.Value))
        End If

    End Sub

    Private Sub btReturnToVT_Click(sender As Object, e As EventArgs) Handles btReturnToVT.Click

        Dim sourceApp As String = roTypes.Any2String(Request.Params("source"))

        Dim url As String = String.Empty
        If sourceApp IsNot Nothing AndAlso sourceApp = "VTPORTAL" Then
            url = HelperSession.AdvancedParametersCache("VisualTime.SSO.LivePortalAppUrl") & "LoginWeb.aspx?force_load=1"
        Else
            url = HelperSession.AdvancedParametersCache("VisualTime.SSO.LiveDesktopAppUrl") & "LoginWeb.aspx?force_load=1"
        End If

        HttpContext.Current.Response.Redirect(url, False)
        HttpContext.Current.ApplicationInstance.CompleteRequest()
    End Sub

#End Region

End Class