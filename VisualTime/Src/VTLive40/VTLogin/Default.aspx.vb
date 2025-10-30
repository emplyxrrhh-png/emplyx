Imports Robotics.Base.DTOs
Imports Robotics.VTBase
Imports Robotics.Web.Base

Public Class VTLogin_Login
    Inherits PageBase

    Public ReadOnly Property ADFSUserName(Optional ByVal bolreload As Boolean = False) As String
        Get
            Dim oParamSSOType As String = HelperSession.AdvancedParametersCache("SSOType")

            Return CommonClaim.GetAuthenticationClaim(oParamSSOType, 1, Request.GetOwinContext())
        End Get
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.InsertExtraJavascript("BrowserDetect", "~/Base/Scripts/BrowserDetect.js", , True)
        Me.InsertExtraJavascript("Cookies", "~/Base/Scripts/Cookies.js", , True)
        Me.InsertExtraJavascript("Generic", "~/Base/Scripts/Generic.js", , True)
        Me.InsertExtraJavascript("Generic", "~/Scripts/login.js", , True)

        Me.InsertCssIncludes(Me.Page)

        If Not IsPostBack Then
            Dim backgroundImage As String = "Q" & HelperWeb.getSeason(Date.Now) & "-" & HelperWeb.RandomGenerator.Next(1, 6) & ".jpg"
            Me.rbBackground.Style("background-image") = "url(../Base/Images/LoginBackground/" & backgroundImage & ");"
        End If

        If Me.oLanguage Is Nothing Then
            Me.oLanguage = New roLanguageWeb()
            Me.oLanguage.SetLanguageReference("LivePortal", API.CommonServiceMethods.DefaultLanguage)
        End If

        Dim sourceApp As String = roTypes.Any2String(Request.Params("source"))
        Dim oParamSSOType As String = HelperSession.AdvancedParametersCache("SSOType")

        If Not Me.IsPostBack Then

            If oParamSSOType <> String.Empty AndAlso WLHelperWeb.ADFSEnabled Then

                Dim bLoggedIn As Boolean
                Try
                    Dim sCookieName As String = "ExternalCookie"

                    If Request.GetOwinContext().Authentication.AuthenticateAsync(sCookieName).Result IsNot Nothing Then
                        bLoggedIn = Request.GetOwinContext().Authentication.AuthenticateAsync(sCookieName).Result.Identity.IsAuthenticated
                    End If
                Catch ex As Exception
                    bLoggedIn = False
                End Try

                If Not bLoggedIn Then
                    CommonAuth.RedirectToLoginIfNecesary(HttpContext.Current.Session("roMultiCompanyId"), oParamSSOType, Me.Context.GetOwinContext(), Robotics.Web.Base.Configuration.LiveDesktopAppUrl & "VTLogin/Default.aspx?referer=" & HttpContext.Current.Session("roMultiCompanyId"))
                Else
                    If WLHelperWeb.ADFSEnabled = False Then
                        WLHelperWeb.RedirectToUrl(Robotics.Web.Base.Configuration.LiveDesktopAppUrl)
                    Else
                        lblUserName.Text = lblUserName.Text

                        If sourceApp = String.Empty Then
                            If WLHelperWeb.CurrentPassport(True) IsNot Nothing Then
                                WLHelperWeb.RedirectToUrl(Robotics.Web.Base.Configuration.LiveDesktopAppUrl & "Default.aspx")
                            Else
                                btAdfsLogin_Click()
                            End If
                        Else
                            Dim oParam As String = HelperSession.AdvancedParametersCache("VisualTime.SSO.VTLiveMixedAuthEnabled")
                            If roTypes.Any2String(oParam) <> "1" Then
                                Select Case sourceApp.ToUpper
                                    Case "VTLIVE"
                                        If WLHelperWeb.CurrentPassport(True) IsNot Nothing Then
                                            WLHelperWeb.RedirectToUrl(Robotics.Web.Base.Configuration.LiveDesktopAppUrl & "Default.aspx")
                                        Else
                                            btAdfsLogin_Click()
                                        End If
                                    Case Else
                                        Me.lblResult.Text = Me.oLanguage.Translate("Adfs.Error.SystemNotConfigured", Me.DefaultScope)
                                        Me.errorRow.Style("Display") = ""
                                End Select
                            Else
                                Me.lblResult.Text = Me.oLanguage.Translate("Adfs.Error.SystemNotConfigured", Me.DefaultScope)
                                Me.errorRow.Style("Display") = ""
                            End If
                        End If
                    End If
                End If
            Else
                Me.lblResult.Text = Me.oLanguage.Translate("Adfs.Error.SystemNotConfigured", Me.DefaultScope)
                Me.errorRow.Style("Display") = ""
            End If
        End If

    End Sub

#Region "Live Desktop"

    Protected Sub btAdfsLogin_Click()
        Dim bolAuthenticated As Boolean = False

        If ADFSUserName <> String.Empty Then
            ' Try to login.
            Dim Pass As roPassportTicket = Nothing

            Dim strPassword As String = ""
            Dim ssoLogin As Boolean = True

            Pass = API.SecurityServiceMethods.Authenticate(Me.Page, Nothing, AuthenticationMethod.Password, "\" & ADFSUserName, strPassword, True, , , , , "", , ssoLogin, True)
            Dim bolExpired As Boolean = False
            Dim needTemporanyKey As Boolean = False
            Dim TemporanyKeyExpired As Boolean = False
            Dim needEmail As Boolean = False
            Dim clientLocation As String = roWsUserManagement.SessionObject.States.SecurityState.ClientAddress.Split("#")(0)

            If Pass IsNot Nothing Then
                roWsUserManagement.SessionObject.AccessApi.InitWebServices()

                If API.SecurityServiceMethods.SetLanguage(Me.Page, Pass.ID, WLHelperWeb.GetLastLanguageUsed()) Then
                    bolAuthenticated = True
                End If

                If bolAuthenticated Then
                    Dim oContext As WebCContext = WLHelperWeb.Context(HttpContext.Current.Request, Pass.ID, , True)
                    oContext.Password = strPassword

                    Dim lastLogin = API.SecurityServiceMethods.UpdateLastLogin(Pass.ID)

                    WLHelperWeb.CommitLogin(Pass, "\" & ADFSUserName, AuthenticationMethod.Password)
                    WLHelperWeb.SetSessionContext(oContext)

                    bolAuthenticated = True
                End If


                HelperWeb.EraseCookie("Login_UserName")

                HelperWeb.EraseCookie("Login_CompanyName")
                HelperWeb.CreateCookie("Login_CompanyName", HttpContext.Current.Session("roMultiCompanyId"))
                HelperWeb.CreateCookie("LastLoginOnSSO", "true")
            End If

            If Not bolAuthenticated Then
                Me.lblResult.Text = Me.oLanguage.Translate("Adfs.Error.UserNotExistsOrAllowed", Me.DefaultScope) & "(" & ADFSUserName & ")"
                Me.errorRow.Style("Display") = ""
                ' Eliminamos la sesión
                If Pass IsNot Nothing Then API.SecurityServiceMethods.SignOut(Nothing, Pass.ID, True)

                Robotics.Web.Base.HelperWeb.EraseCookie("SSOCompanyId")
                HttpContext.Current.Session("roMultiCompanyId") = String.Empty

                WLHelperWeb.CurrentPassport = Nothing
            Else
                roWsUserManagement.SessionObject.AccessApi.InitWebServices()
                WLHelperWeb.RedirectToUrl(Robotics.Web.Base.Configuration.LiveDesktopAppUrl & "Default.aspx")
            End If
        Else
            'Mensaje de error
            Me.lblResult.Text = Me.oLanguage.Translate("Adfs.Error.UserNotRecognized", Me.DefaultScope)
            Me.errorRow.Style("Display") = ""
        End If

    End Sub

    Private Sub btReturnToVT_Click(sender As Object, e As EventArgs) Handles btReturnToVT.Click
#If DEBUG Then
        WLHelperWeb.RedirectToUrl("https://vtlive.visualtime.net/LoginWeb.aspx?force_load=1")
#Else
        WLHelperWeb.RedirectToUrl("https://vtlive.visualtime.net/LoginWeb.aspx?force_load=1")
#End If
    End Sub

#End Region

End Class