Imports System.Security.Principal
Imports Robotics
Imports Robotics.Base.DTOs
Imports Robotics.Security.Base
Imports Robotics.VTBase
Imports Robotics.Web.Base
Imports Robotics.Web.Base.API

Partial Class LoginWeb
    Inherits PageBase

    Private strUserNameControlID As String = "lblUserName"

    Public Property UserNameControlID() As String
        Get
            Return Me.strUserNameControlID
        End Get
        Set(ByVal value As String)
            Me.strUserNameControlID = value
        End Set
    End Property

#Region "Events"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.InsertExtraJavascript("BrowserDetect", "~/Base/Scripts/BrowserDetect.js", , True)
        Me.InsertExtraJavascript("Cookies", "~/Base/Scripts/Cookies.js", , True)
        Me.InsertExtraJavascript("Generic", "~/Base/Scripts/Generic.js", , True)
        Me.InsertExtraJavascript("Generic", "~/Scripts/login.js", , True)

        Me.InsertCssIncludes(Me.Page)

        If WLHelperWeb.CurrentPassport() Is Nothing AndAlso Not roTypes.Any2Boolean(Request.Params("FORCE_LOAD")) Then
            If WLHelperWeb.ADFSEnabled AndAlso Not WLHelperWeb.VTLiveSSOMixMode Then
                WLHelperWeb.RedirectToVTLogin()
            End If
        Else
            WLHelperWeb.RedirectDefault()
        End If

        Me.lblTitle.Visible = True
        Me.lblResult.Visible = False
        Me.titleVT.Visible = True
        Me.titleVTOne.Visible = False
        Me.versionVTOne.Visible = False

        Me.trMultitennantServer.Style("display") = ""
        If Not Me.IsPostBack Then Me.ServerText.Text = HelperWeb.GetCookie("Login_CompanyName")

        If Not Me.IsPostBack Then
            Me.LoadLanguageCombo()
            'Dim backgroundImage As String = "img-form_C.png"
            Dim backgroundImage As String = "Q" & HelperWeb.getSeason(Date.Now) & "-" & HelperWeb.RandomGenerator.Next(1, 6) & ".jpg"

            Me.rbBackground.Style("background-image") = "url(Base/Images/LoginBackground/" & backgroundImage & ");"

            'Comprobar si tenemos licencia VTOne o Live
            If API.CommonServiceMethods.GetVisualTimeEdition().ToUpper = "ONE" Then

                Me.rbBackground.Style("background-image") = "url(Base/Images/LoginBackground/" & "VTOne.jpg" & ");"
                Me.titleVT.Visible = False
                Me.titleVTOne.Visible = True
                Me.versionVTOne.Visible = True
                Me.versionVTOne.ForeColor = Drawing.Color.FromArgb(90, 105, 112)
                Me.versionVTOne.Font.Size = 10

            End If

            Dim br As HttpBrowserCapabilities = Request.Browser
            Me.hdnOSVersion.Value = br.Platform

            Me.UserName.Text = HelperWeb.GetCookie("Login_UserName")

            Dim lastLanguage As String = WLHelperWeb.GetLastLanguageUsed()

            If Not String.IsNullOrEmpty(lastLanguage) Then
                Me.cmbLanguage.SelectedValue = lastLanguage
            Else
                If Not String.IsNullOrEmpty(ServerText.Text) Then
                    HttpContext.Current.Session("roMultiCompanyId") = ServerText.Text.ToLowerInvariant
                    Global_asax.ReloadSharedData()

                    Me.cmbLanguage.SelectedValue = API.CommonServiceMethods.DefaultLanguage()

                    HttpContext.Current.Session("roMultiCompanyId") = String.Empty
                Else
                    Me.cmbLanguage.SelectedValue = "ESP"
                End If
            End If

        End If



        If Me.UserName.Text = String.Empty Then
            Me.UserName.Focus()
        Else
            Me.Password.Focus()
        End If


        If Me.ServerText.Text = String.Empty Then
            Me.ServerText.Focus()
        End If
    End Sub

    Protected Sub btnRedirectSSO_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnRedirectSSO.Click
        If AuthValidations.IsAlreadyLoggedOnAnotherCompany(ServerText.Text.ToLowerInvariant) Then
            WLHelperWeb.RedirectDefault()
            Return
        End If

        'guardar en sesión si existe el anchor en el elemento hidden
        HttpContext.Current.Session("URL2Redirect") = HiddenFieldRedirectUrl.Value
        If ServerText.Text.ToLowerInvariant = String.Empty Then
            HttpContext.Current.Session("roMultiCompanyId") = String.Empty
            Me.lblResult.Visible = True
            Me.lblResult.Text = oLanguage.Translate("CompanySSO.NotSupplied.Message", Me.DefaultScope)
        Else
            HttpContext.Current.Session("roMultiCompanyId") = ServerText.Text.ToLowerInvariant

            If Not WLHelperWeb.ADFSEnabled Then
                HttpContext.Current.Session("roMultiCompanyId") = String.Empty
                Me.lblResult.Visible = True
                Me.lblResult.Text = oLanguage.Translate("CompanySSO.NotActive.Message", Me.DefaultScope)
            Else
                Robotics.Web.Base.HelperWeb.CreateCookie("SSOCompanyId", ServerText.Text.ToLowerInvariant)
                WLHelperWeb.RedirectToVTLogin()
            End If

        End If
    End Sub

    Protected Sub btnRecoverPasswordBack_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnRecoverPasswordBack.Click

        divRecoverUser.Visible = True
        divRecoverMail.Visible = True
        divRecoverCode.Visible = False
        divRecoverNewPassword.Visible = False
        divbtnRecoverPasswordNext.Visible = True
        divbtnRecoverPasswordEnd.Visible = False
        errorDiv.Visible = False
        errorText.Text = ""
        RecoverPasswordPopUp.CloseAction = DevExpress.Web.CloseAction.OuterMouseClick
    End Sub

    Protected Sub btnRecoverPasswordFinish_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnRecoverPasswordFinish.Click
        Dim result As New StdResponse
        If txtCode.Value <> Nothing AndAlso txtNewPassword.Value <> Nothing Then
            result = Robotics.Base.VTPortal.VTPortal.SecurityHelper.ResetPasswordToNew(txtUser.Value, txtCode.Value, txtNewPassword.Value, roAppType.VTLive, WLHelperWeb.SecurityToken, Nothing)
        End If

        If result.Result Then
            RecoverPasswordPopUp.ShowOnPageLoad = False
        Else
            errorDiv.Visible = True
            RecoverPasswordPopUp.CloseAction = DevExpress.Web.CloseAction.OuterMouseClick
            Select Case result.Status
                Case -59
                    errorText.Text = oLanguage.Translate("lowPassword", Me.DefaultScope)
                Case -60
                    errorText.Text = oLanguage.Translate("mediumPassword", Me.DefaultScope)
                Case -61
                    errorText.Text = oLanguage.Translate("highPassword", Me.DefaultScope)
                Case -87
                    errorText.Text = oLanguage.Translate("recoveryCode", Me.DefaultScope)
                Case Else
                    errorText.Text = oLanguage.Translate("recoverError", Me.DefaultScope)
            End Select

        End If

    End Sub

    Protected Sub btnRecoverPassword_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnRecoverPasswordNext.Click
        If AuthValidations.IsAlreadyLoggedOnAnotherCompany(ServerText.Text.ToLowerInvariant) Then
            WLHelperWeb.RedirectDefault()
            Return
        End If


        Dim bret = False
        Dim canContinue = False
        If ServerText.Text.ToLowerInvariant = String.Empty Then
            HttpContext.Current.Session("roMultiCompanyId") = String.Empty
            errorDiv.Visible = True
            errorText.Text = oLanguage.Translate("CompanySSO.NotSupplied.Message", Me.DefaultScope)
            canContinue = False
        Else
            HttpContext.Current.Session("roMultiCompanyId") = ServerText.Text.ToLowerInvariant
            canContinue = True
        End If

        If canContinue Then
            If txtUser.Value <> Nothing AndAlso txtEmail.Value <> Nothing Then
                bret = Robotics.Security.WLHelper.RecoverEmployeePassword(txtUser.Value, txtEmail.Value, roAppType.VTLive)
            End If

            If bret Then
                divRecoverUser.Visible = False
                divRecoverMail.Visible = False
                divRecoverCode.Visible = True
                divRecoverNewPassword.Visible = True
                divbtnRecoverPasswordNext.Visible = False
                divbtnRecoverPasswordEnd.Visible = True
                errorDiv.Visible = False
                errorText.Text = ""
                RecoverPasswordPopUp.CloseAction = DevExpress.Web.CloseAction.None
            Else
                errorDiv.Visible = True
                errorText.Text = oLanguage.Translate("recoverError", Me.DefaultScope)
            End If
        End If

    End Sub

    Protected Sub btAccept_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btAccept.Click


        If AuthValidations.IsAlreadyLoggedOnAnotherCompany(ServerText.Text.ToLowerInvariant) Then
            WLHelperWeb.RedirectDefault()
            Return
        End If

        WLHelperWeb.ServerLicense = Nothing
        Dim bolAuthenticated As Boolean = False

        HttpContext.Current.Session("roMultiCompanyId") = ServerText.Text.ToLowerInvariant
        Global_asax.ReloadSharedData()

        Dim strPassword As String = Password.Text
        Dim isRoboticsUser As Boolean = False

        Dim strInitialMail As String = ""
        If Me.txtRoboticsMail.Text <> "" Then
            Dim emailRegex As String = "^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"

            If Not Regex.IsMatch(txtRoboticsMail.Text, emailRegex) Then
                Me.lblResult.Text = oLanguage.Translate("ErrorValidatingEmailAddress.Message", Me.DefaultScope)
                Me.lblResult.Visible = True
                WLHelperWeb.CurrentPassport = Nothing
                Return

            Else
                strInitialMail = txtRoboticsMail.Text
            End If

        End If

        Dim ssoLogin As Boolean = False
        Dim Pass As roPassportTicket = API.SecurityServiceMethods.Authenticate(Me.Page, Nothing, AuthenticationMethod.Password, UserName.Text, strPassword, True, , , , , strInitialMail, , ssoLogin, True)
        Dim bolExpired As Boolean = False
        Dim needTemporanyKey As Boolean = False
        Dim TemporanyKeyExpired As Boolean = False
        Dim needEmail As Boolean = False
        Dim clientLocation As String = roWsUserManagement.SessionObject().States.SecurityState.ClientAddress.Split("#")(0)

        If Pass IsNot Nothing Then
            If Pass.IsPrivateUser Then
                isRoboticsUser = True
                needEmail = roWsUserManagement.SessionObject().States.SecurityState.Result = SecurityResultEnum.NeedMailRequest
                needTemporanyKey = roWsUserManagement.SessionObject().States.SecurityState.Result = SecurityResultEnum.NeedTemporaryKeyRequestRobotics
                TemporanyKeyExpired = roWsUserManagement.SessionObject().States.SecurityState.Result = SecurityResultEnum.NeedTemporaryKeyRequestExpiredRobotics
            Else
                needEmail = False
                bolExpired = roWsUserManagement.SessionObject().States.SecurityState.Result = SecurityResultEnum.IsExpired
                needTemporanyKey = roWsUserManagement.SessionObject().States.SecurityState.Result = SecurityResultEnum.NeedTemporaryKeyRequest
                TemporanyKeyExpired = roWsUserManagement.SessionObject().States.SecurityState.Result = SecurityResultEnum.NeedTemporaryKeyRequestExpired
            End If

            roWsUserManagement.RemoveCurrentsession()
            Dim continueLoading As Boolean = True


            Dim oLang As roAzureLocale = Robotics.DataLayer.roCacheManager.GetInstance.GetLocaleByKey(Me.cmbLanguage_Value.Value)
            If oLang Is Nothing OrElse Not API.UserAdminServiceMethods.UpdatePassportNameAndLanguage(Me.Page, Pass.ID, Pass.Name, oLang.id) Then
                continueLoading = False
            End If


            If continueLoading Then
                ' Actualizamos el idioma del pasaporte
                Pass.Language = API.UserAdminServiceMethods.GetLanguageByKey(Me.Page, oLang.key)
                ' Actualizamos la contraseña en el contexto del usuario (necesario para poder establecer la seguridad de los ws)
                Dim oContext As WebCContext = WLHelperWeb.Context(HttpContext.Current.Request, Pass.ID, , True)
                oContext.Password = Robotics.VTBase.CryptographyHelper.EncryptMD5(strPassword)

                Dim oLanguage As New roLanguageWeb()
                WLHelperWeb.SetLanguage(oLanguage, Me.LanguageFile, , Pass)

                If isRoboticsUser Then
                    If needEmail Then
                        continueLoading = False

                        If (Me.trRoboticsMail.Style("display") = "none") Then
                            Me.trRoboticsMail.Style("display") = ""
                            Me.lblResult.Text = oLanguage.Translate("UserRoboticsNeeded.Message", Me.DefaultScope)
                        End If

                        If Me.txtRoboticsMail.Text <> "" Then
                            Pass = API.SecurityServiceMethods.Authenticate(Me.Page, Pass, AuthenticationMethod.Password, UserName.Text, strPassword, True, , , , , strInitialMail, , ssoLogin, True)
                            If Pass IsNot Nothing Then
                                continueLoading = True
                                needTemporanyKey = roWsUserManagement.SessionObject().States.SecurityState.Result = SecurityResultEnum.NeedTemporaryKeyRequestRobotics
                                TemporanyKeyExpired = roWsUserManagement.SessionObject().States.SecurityState.Result = SecurityResultEnum.NeedTemporaryKeyRequestExpiredRobotics
                            End If
                        End If

                        Me.lblResult.Visible = True
                    End If

                    If continueLoading AndAlso (needTemporanyKey OrElse TemporanyKeyExpired) Then
                        continueLoading = False

                        If (Me.trValidationCode.Style("display") = "none") Then
                            Me.trValidationCode.Style("display") = ""
                        End If

                        If TemporanyKeyExpired Then
                            Me.lblResult.Text = oLanguage.Translate("TemporanyKeyExpired.Message", Me.DefaultScope)
                        Else
                            Me.lblResult.Text = oLanguage.Translate("NeedTemporanyKey.Message", Me.DefaultScope)

                            If Me.txtValidationCode.Text <> "" Then
                                If WLHelperWeb.IsValidCode(Pass.ID, Me.txtValidationCode.Text) AndAlso
                                            WLHelperWeb.ResetValidationCodeRobotics(Pass.ID, clientLocation) Then
                                    Pass = API.SecurityServiceMethods.Authenticate(Me.Page, Pass, AuthenticationMethod.Password, UserName.Text, strPassword, True, , , , , , , ssoLogin, True)
                                    bolExpired = roWsUserManagement.SessionObject().States.SecurityState.Result = SecurityResultEnum.IsExpired
                                    continueLoading = True
                                Else
                                    continueLoading = False
                                    Me.lblResult.Text = oLanguage.Translate("ErrorValidatingTemporanyKey.Message", Me.DefaultScope)
                                End If
                            End If

                        End If

                        Me.lblResult.Visible = True

                    End If
                Else
                    If needTemporanyKey OrElse TemporanyKeyExpired Then
                        continueLoading = False

                        If (Me.trValidationCode.Style("display") = "none") Then
                            Me.trValidationCode.Style("display") = ""
                        End If

                        If TemporanyKeyExpired Then
                            Me.lblResult.Text = oLanguage.Translate("TemporanyKeyExpired.Message", Me.DefaultScope)
                        Else
                            Me.lblResult.Text = oLanguage.Translate("NeedTemporanyKey.Message", Me.DefaultScope)

                            If Me.txtValidationCode.Text <> "" Then
                                If WLHelperWeb.IsValidCode(Pass.ID, Me.txtValidationCode.Text) AndAlso
                                            WLHelperWeb.ResetValidationCode(Pass.ID, clientLocation) Then
                                    Pass = API.SecurityServiceMethods.Authenticate(Me.Page, Pass, AuthenticationMethod.Password, UserName.Text, strPassword, True, , , , , , , ssoLogin, True)
                                    bolExpired = roWsUserManagement.SessionObject().States.SecurityState.Result = SecurityResultEnum.IsExpired
                                    continueLoading = True
                                Else
                                    continueLoading = False
                                    Me.lblResult.Text = oLanguage.Translate("ErrorValidatingTemporanyKey.Message", Me.DefaultScope)
                                End If
                            End If

                        End If

                        Me.lblResult.Visible = True
                    End If
                End If

                If continueLoading Then
                    bolAuthenticated = True

                    API.SecurityServiceMethods.UpdateLastLogin(Pass.ID)
                    WLHelperWeb.CommitLogin(Pass, UserName.Text, AuthenticationMethod.Password)
                    WLHelperWeb.SetSessionContext(oContext)

                    ' Actualizar nombre de usuario al control de pantalla
                    Me.UpdateUserNameControl(Pass.Name)

                    Dim credential = roTypes.Any2String(Pass.AuthCredential)
                    If Not isRoboticsUser AndAlso credential.IndexOf("\") = -1 Then
                        If bolExpired Then
                            ' Si la cuenta esta caducada , forzamos a cambiar la contraseña
                            HttpContext.Current.Session("PASSWORDEXPIRED") = True
                            'Me.Session.Add("PASSWORDEXPIREDAlert", True)
                            AuthHelper.SetPassportKeyValidated(Pass.ID, False, WLHelperWeb.SecurityToken, False)
                        Else
                            HttpContext.Current.Session("PASSWORDEXPIRED") = False
                            Dim resultCode As PasswordLevelError = API.SecurityServiceMethods.IsValidPwd(Nothing, Pass, Pass.ID, strPassword, False, "")

                            AuthHelper.SetPassportKeyValidated(Pass.ID, resultCode = PasswordLevelError.No_Error, WLHelperWeb.SecurityToken, False)

                            If resultCode <> PasswordLevelError.No_Error Then
                                HttpContext.Current.Session("LOPD") = True
                                HttpContext.Current.Session("ShowLegalText") = True
                                HttpContext.Current.Session("ShowLegalText.VTLive") = True
                                'Me.Session.Add("LOPDAlert", True)
                            Else
                                HttpContext.Current.Session("LOPD") = False
                                HttpContext.Current.Session("ShowLegalText") = False
                            End If
                        End If
                    End If

                    ' Actualizamos la cookie con el último nombre de usuario utilizado
                    HelperWeb.EraseCookie("Login_UserName")
                    HelperWeb.CreateCookie("Login_UserName", Me.UserName.Text)

                    HelperWeb.EraseCookie("Login_CompanyName")
                    HelperWeb.CreateCookie("Login_CompanyName", Me.ServerText.Text)

                    roWsUserManagement.RemoveCurrentsession()

                    HttpContext.Current.Session("ShowLegalText") = Robotics.VTBase.roTypes.Any2Boolean(HelperSession.AdvancedParametersCache("VisualTime.Security.ShowLegalText"))
                    HttpContext.Current.Session("ShowLegalText.VTLive") = HttpContext.Current.Session("ShowLegalText")
                    'Guardamos en sesión la url para hacer redirect
                    HttpContext.Current.Session("URL2Redirect") = HiddenFieldRedirectUrl.Value
                    WLHelperWeb.RedirectToUrl(String.Format("/{0}/Default.aspx", Configuration.RootUrl))
                End If
            Else
                Me.lblResult.Text = API.SecurityServiceMethods.LastErrorText
                Me.lblResult.Visible = True
                Me.InvalidPassword.Visible = False
                lblTitle.Visible = False
            End If

            If Not bolAuthenticated Then
                If Pass IsNot Nothing Then API.SecurityServiceMethods.SignOut(Nothing, Pass.ID, True)
                WLHelperWeb.CurrentPassport = Nothing
            Else
                roWsUserManagement.RemoveCurrentsession()
            End If
        Else
            ' Otherwise, display message.
            InvalidPassword.Visible = True
            lblTitle.Visible = False
            If roWsUserManagement.SessionObject().States.SecurityState.Result <> SecurityResultEnum.NoError Then
                lblResult.Text = roWsUserManagement.SessionObject().States.SecurityState.ErrorText
                lblResult.Visible = True
                Me.InvalidPassword.Visible = False
            End If
        End If

        If bolAuthenticated Then API.SecurityServiceMethods.SetLastNotificationSended(Me.Page, WLHelperWeb.CurrentPassportID, Nothing)

    End Sub

    Protected Sub btChangeLanguage_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btChangeLanguage.Click

        ' Actualizamos el idioma por defecto
        WLHelperWeb.CommitPassportLanguage(Nothing, Me.cmbLanguage_Value.Value)

        ' Actualizamos la cookie con el último nombre de usuario utilizado
        HelperWeb.EraseCookie("Login_UserName")
        HelperWeb.CreateCookie("Login_UserName", Me.UserName.Text)
        Me.oLanguage = Nothing

        WLHelperWeb.RedirectDefault()

    End Sub

#End Region

#Region "Methods"

    Private Sub UpdateUserNameControl(ByVal strName As String)

        Dim cnUserName As Control = HelperWeb.GetControl(Me.Page.Controls, Me.strUserNameControlID)

        Dim oContainer As Object = Nothing

        Dim oProperty As System.Reflection.PropertyInfo = HelperWeb.GetProperty(cnUserName, {"Text"}, oContainer)
        If oProperty IsNot Nothing Then
            oProperty.SetValue(oContainer, strName, Nothing)
        End If

    End Sub

    Private Sub LoadLanguageCombo()

        With Me.cmbLanguage
            .ClearItems()
            .AddItem("Castellano", "ESP", True, "ChangeLanguage();")
            .AddItem("Català", "CAT", True, "ChangeLanguage();")
            .AddItem("Galego", "GAL", True, "ChangeLanguage();")
            .AddItem("Euskera", "EKR", True, "ChangeLanguage();")
            .AddItem("English", "ENG", True, "ChangeLanguage();")
            .AddItem("Francés", "FRA", True, "ChangeLanguage();")
            .AddItem("Português", "POR", True, "ChangeLanguage();")
            .AddItem("Italiano", "ITA", True, "ChangeLanguage();")
            .AddItem("Eslovaco", "SLK", True, "ChangeLanguage();")
        End With

    End Sub

#End Region

End Class