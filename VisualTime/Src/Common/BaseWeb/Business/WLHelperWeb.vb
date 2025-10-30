Imports System.Web
Imports System.Web.Security
Imports System.Web.UI
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common.AdvancedParameter
Imports Robotics.Portal.Business
Imports Robotics.Security
Imports Robotics.Security.Base
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions
Imports Robotics.VTBase.roConstants

''' <summary>
''' Provides helper methods to use WebLogin with ASP.NET
''' </summary>
Public NotInheritable Class WLHelperWeb

    Private Const ReturnUrlCookie As String = "ReturnUrl"
    Private Const SessionTicket As String = "WLPassportTicket"
    Private Const SessionTicketID As String = "WLPassportTicketID"
    Private Const SessionGroupType As String = "WLPassportGroupType"
    Private Const SessionConsutant As String = "WLPassportConsultant"
    Private Const SessionTicketCredential As String = "WLSessionTicketCredentialID"
    Private Const SessionTicketMethod As String = "WLSessionTicketMethod"
    Private Const SessionPassportCheck As String = "WLPassportCheck"
    Private Const SessionTimeoutCheck As String = "WLTimeoutCheck"
    Private Const SecurityTokenID As String = "WLSecurityTokenID"
    Private Const AuthTokenID As String = "WLAuthTokenID"
    Private Const CustomizationID As String = "WLCustomizationID"
    Private Const ThemeUsedID As String = "WLThemeUsedID"
    Private Const CurrentLanguageID As String = "WLCurrentLanguageID"

    Private Const FeaturePermissionsID As String = "WLPassportFeaturePermissions"
    Private Const InstalledFeaturesID As String = "WLInstalledFeatures"
    Private Const InstalledFeaturesDataID As String = "WLInstalledFeaturesData"
    Private Const PassportPermissionOverGroupID As String = "WLPassportPermissionsOverGroup"
    Private Const PassportPermissionOverEmployeeID As String = "WLPassportPermissionsOverEmployee"
    Private Const PassportPermissionOverGroupAppID As String = "WLPassportPermissionsOverGroupApp"
    Private Const PassportPermissionOverEmployeeAppID As String = "WLPassportPermissionsOverEmployeeApp"
    Private Const PassportCurrentCulture As String = "WLPassportCurrentCulture"

    Private Const PassportEmployeeDocAlerts As String = "WLPassportEmployeeDocAlerts"
    Private Const PassportCompanyDocAlerts As String = "WLPassportCompanyDocAlerts"
    Private Const PassportUserAlerts As String = "WLPassportUserAlerts"
    Private Const PassportSystemAlerts As String = "WLPassportSystemAlerts"
    'Private Const PassportEmployeeDocAlertsCheckDate As String = "WLPassportEmployeeDocAlertsCheckDate"
    'Private Const PassportDocumentsDocAlertsCheckDate As String = "WLPassportDocumentsDocAlertsCheckDate"
    Private Const PassportAlertsCheckDate As String = "WLPassportAlertsCheck"

    Private Const PassportMainMenu As String = "WLPassportMainMenu"
    Private Const PassportLicenseCache As String = "WLPassportLicense"

    Private Const AnalyticsDataClean As String = "WLAnalyticsDataClean"

    Private Const CardListSearchFilter As String = "WLCardListSearchFiltering"
    Private Const updateNotificationID As String = "WLUpdateNotification"

    Public Shared ReadOnly NotAuthenticatedRedirectUrl As String = String.Format("/{0}/LoginWeb.aspx", Configuration.RootUrl)
    Public Shared ReadOnly AccessDeniedRedirectUrl As String = String.Format("/{0}/Base/AccessDenied.aspx", Configuration.RootUrl)
    Public Shared ReadOnly DefaultRedirectUrl As String = String.Format("/{0}/Default.aspx", Configuration.RootUrl)
    Public Shared ReadOnly DefaultVTLogin_V1_RedirectUrl As String = String.Format("/{0}/VTLogin/", Configuration.RootUrl)
    Public Shared ReadOnly DefaultVTLogin_V2_RedirectUrl As String = String.Format("/{0}/Auth/", Configuration.RootUrl)
    Public Shared ReadOnly DefaultVTLogin_CEGIDID As String = String.Format("/{0}/sso/cegidIDLogin", Configuration.RootUrl)

    'Public Enum PasswordLevelError
    '    No_Error = 0
    '    Low_Error = 1
    '    Medium_Error = 2
    '    High_Error = 3
    '    No_Passport_Error = 4
    'End Enum

    '=======================================================================

    Public Shared Property CardListSearchFiltering() As List(Of Integer)
        Get
            Return HttpContext.Current.Session(CardListSearchFilter)
        End Get
        Set(ByVal value As List(Of Integer))
            HttpContext.Current.Session.Add(CardListSearchFilter, value)
        End Set

    End Property
    '=======================================================================

    ''' <summary>
    ''' Commits login, store passport ticket and redirects user.
    ''' </summary>
    ''' <param name="pass">The authorized passport.</param>
    Public Shared Sub CommitLoginAndRedirect(ByVal pass As roPassport, ByVal strPassword As String)
        ' Commit login.
        FormsAuthentication.SetAuthCookie(pass.ID.ToString(), False)

        Dim oTicket As roPassportTicket = API.SecurityServiceMethods.GetPassportTicket(Nothing, pass.ID)
        CurrentPassport = oTicket

        ' Redirect.
        Dim ReturnUrl As String
        If HttpContext.Current.Request.Cookies(ReturnUrlCookie) Is Nothing Then
            ReturnUrl = "/" & Configuration.RootUrl & "/"
            'ReturnUrl = "/Portal/"
        Else
            ReturnUrl = HttpContext.Current.Request.Cookies(ReturnUrlCookie).Value
        End If
        HttpContext.Current.Request.Cookies.Remove(ReturnUrlCookie)
        RedirectToUrl(ReturnUrl)
    End Sub

    Public Shared Sub CommitLogin(ByVal oPassportTicket As roPassportTicket, ByVal strUserName As String, ByVal authmethod As AuthenticationMethod)
        ' Commit login.
        WLHelperWeb.CommitPassportLanguage(oPassportTicket, String.Empty)

        CurrentPassport = oPassportTicket
        CurrentPassportID = oPassportTicket.ID
        CurrentPassportCredential = strUserName
        CurrentPassportAuthMethod = authmethod

        HttpContext.Current.Session.Add(Configuration.LoginCookieName & "2", HttpContext.Current.Session.SessionID & "*" & Configuration.ApplicationName)

        If Configuration.ApplicationName.ToUpper = "VTLIVE" Then
            API.SecurityServiceMethods.SetLastNotificationSended(Nothing, oPassportTicket.ID, Nothing)
        End If

        HelperWeb.EraseCookie("AlreadyLoggedinInOtherLocation")
        HelperWeb.EraseCookie("SessionInvalidatedByPermissions")
        HelperWeb.EraseCookie("UpdateSessionError")
        HelperWeb.EraseCookie("SessionExpired")

        ' Redirect.
        Dim ReturnUrl As String
        If HttpContext.Current.Request.Cookies(ReturnUrlCookie) Is Nothing Then
            ReturnUrl = "/" & Configuration.RootUrl & "/"
        Else
            ReturnUrl = HttpContext.Current.Request.Cookies(ReturnUrlCookie).Value
        End If
        HttpContext.Current.Request.Cookies.Remove(ReturnUrlCookie)
    End Sub

    ''' <summary>
    ''' Signs user out and removes authentication tickets.
    ''' </summary>
    Public Shared Sub SignOut(ByVal oPage As Page, ByVal _CurrentPassport As roPassportTicket)
        Dim cContext As WebCContext = Context(HttpContext.Current.Request)

        If cContext IsNot Nothing Then
            cContext.MenuGroup = Nothing
        End If

        If _CurrentPassport IsNot Nothing Then API.SecurityServiceMethods.SignOut(oPage, _CurrentPassport.ID)
        CurrentPassport = Nothing
        CurrentPassportID = -1
        CurrentPassportCredential = ""
        CurrentPassportAuthMethod = AuthenticationMethod.Password
        roWsUserManagement.RemoveCurrentsession()
    End Sub

    Public Shared Property Customization As String
        Get
            Dim strCustomizationID As String = String.Empty
            Try
                If HttpContext.Current.Session(CustomizationID) IsNot Nothing Then
                    strCustomizationID = CType(HttpContext.Current.Session(CustomizationID), String)
                Else
                    strCustomizationID = HelperSession.AdvancedParametersCache("Customization")
                    HttpContext.Current.Session(CustomizationID) = strCustomizationID
                End If
            Catch ex As Exception
                strCustomizationID = String.Empty
            End Try

            Return strCustomizationID
        End Get
        Set(value As String)
            HttpContext.Current.Session(CustomizationID) = value
        End Set
    End Property

    Public Shared ReadOnly Property ADFSEnabled As Boolean
        Get
            Return roTypes.Any2Boolean(HelperSession.AdvancedParametersCache("ADFSEnabled"))
        End Get
    End Property

    Public Shared ReadOnly Property VTLiveSSOMixMode As Boolean
        Get
            Return roTypes.Any2Boolean(HelperSession.AdvancedParametersCache("VisualTime.SSO.VTLiveMixedAuthEnabled"))
        End Get
    End Property

    Public Shared ReadOnly Property VTPortalMixMode As Boolean
        Get
            Return roTypes.Any2Boolean(HelperSession.AdvancedParametersCache("VisualTime.SSO.VTPortalMixedAuthEnabled"))
        End Get
    End Property

    Public Shared ReadOnly Property SSOType As String
        Get
            Return roTypes.Any2String(HelperSession.AdvancedParametersCache("SSOType").Trim)
        End Get
    End Property

    Public Shared Property AuthToken() As String
        Get
            Dim oSessionID As String = String.Empty
            Try
                oSessionID = CType(HttpContext.Current.Session(AuthTokenID), String)
            Catch ex As Exception
                oSessionID = Guid.NewGuid().ToString
            End Try

            Return oSessionID
        End Get
        Set(ByVal value As String)
            Try
                HttpContext.Current.Session(AuthTokenID) = value
            Catch ex As Exception
                'do nothing
            End Try

        End Set
    End Property

    Public Shared Property SecurityToken() As String
        Get
            Dim oSessionID As String = String.Empty
            Try
                oSessionID = CType(HttpContext.Current.Session(SecurityTokenID), String)
            Catch ex As Exception
                oSessionID = String.Empty
            End Try

            Return oSessionID
        End Get
        Set(ByVal value As String)
            HttpContext.Current.Session(SecurityTokenID) = value
        End Set
    End Property

    Public Shared ReadOnly Property CompanyToken() As String
        Get
            Dim oSessionID As String = "VTLive"
            Try
                oSessionID = roTypes.Any2String(VTBase.roConstants.GetGlobalEnvironmentParameter(GlobalAsaxParameter.CompanyId))
            Catch ex As Exception
                oSessionID = String.Empty
            End Try

            Return oSessionID
        End Get
    End Property

    Public Shared Property CurrentPassportID() As Integer
        Get
            Dim oPassportID As Integer = -1
            Try
                oPassportID = roTypes.Any2Integer(HttpContext.Current.Session(SessionTicketID))
            Catch ex As Exception
                oPassportID = -1
            End Try

            Return oPassportID
        End Get
        Set(ByVal value As Integer)
            HttpContext.Current.Session(SessionTicketID) = value
        End Set
    End Property

    Public Shared Property CurrentUserIsConsultantOrCegid() As Boolean
        Get
            Dim isRoboticsOrConsultant As Boolean
            Try
                If HttpContext.Current.Session(SessionGroupType) Is Nothing Then
                    HttpContext.Current.Session(SessionGroupType) = API.UserAdminServiceMethods.IsRoboticsUserOrConsultant(Nothing, WLHelperWeb.CurrentPassportID)
                End If

                isRoboticsOrConsultant = roTypes.Any2Boolean(HttpContext.Current.Session(SessionGroupType))
            Catch ex As Exception
                isRoboticsOrConsultant = False
            End Try

            Return isRoboticsOrConsultant
        End Get
        Set(ByVal value As Boolean)
            HttpContext.Current.Session(SessionGroupType) = value
        End Set
    End Property

    Public Shared Property CurrentUserIsConsultant() As Boolean
        Get
            Dim isRoboticsOrConsultant As Boolean
            Try
                If HttpContext.Current.Session(SessionConsutant) Is Nothing Then
                    HttpContext.Current.Session(SessionConsutant) = API.UserAdminServiceMethods.IsConsultant(Nothing, WLHelperWeb.CurrentPassportID)
                End If

                isRoboticsOrConsultant = roTypes.Any2Boolean(HttpContext.Current.Session(SessionConsutant))
            Catch ex As Exception
                isRoboticsOrConsultant = False
            End Try

            Return isRoboticsOrConsultant
        End Get
        Set(ByVal value As Boolean)
            HttpContext.Current.Session(SessionConsutant) = value
        End Set
    End Property

    Public Shared Property CurrentPassportCredential() As String
        Get
            Dim strPassportID As String = String.Empty
            Try
                strPassportID = roTypes.Any2String(HttpContext.Current.Session(SessionTicketCredential))
            Catch ex As Exception
                strPassportID = String.Empty
            End Try

            Return strPassportID
        End Get
        Set(ByVal value As String)
            HttpContext.Current.Session(SessionTicketCredential) = value
        End Set
    End Property

    Public Shared Property CurrentPassportAuthMethod() As AuthenticationMethod
        Get
            Dim method As AuthenticationMethod = AuthenticationMethod.Password
            Try
                method = HttpContext.Current.Session(SessionTicketMethod)
            Catch ex As Exception
                method = AuthenticationMethod.Password
            End Try

            Return method
        End Get
        Set(ByVal value As AuthenticationMethod)
            HttpContext.Current.Session(SessionTicketMethod) = value
        End Set
    End Property

    Public Shared Property ThemeUsed() As String
        Get
            Dim cssFile As String = "roLiveStyles.min.css"
            Try
                If HttpContext.Current.Session IsNot Nothing Then
                    Dim tmpFile As String = roTypes.Any2String(HttpContext.Current.Session(ThemeUsedID))
                    If tmpFile <> String.Empty Then cssFile = tmpFile
                End If
            Catch ex As Exception
                cssFile = "roLiveStyles.min.css"
            End Try

            Return cssFile
        End Get
        Set(ByVal value As String)
            HttpContext.Current.Session(ThemeUsedID) = value
        End Set
    End Property

#Region "Cache de permisos sobre funcionalidades, licencias y permisos"

    Public Shared Property PassportFeautrePemissions() As Hashtable
        Get
            Dim featureFermissions As Hashtable
            Try
                featureFermissions = CType(HttpContext.Current.Session(FeaturePermissionsID), Hashtable)
            Catch ex As Exception
                featureFermissions = Nothing
            End Try

            If featureFermissions Is Nothing Then
                featureFermissions = New Hashtable
                HttpContext.Current.Session(FeaturePermissionsID) = featureFermissions
            End If

            Return featureFermissions
        End Get
        Set(ByVal value As Hashtable)
            HttpContext.Current.Session(FeaturePermissionsID) = value
        End Set
    End Property

    Public Shared Property InstalledFeatures() As Hashtable
        Get
            Dim iFeatures As Hashtable
            Try
                iFeatures = CType(HttpContext.Current.Session(InstalledFeaturesID), Hashtable)
            Catch ex As Exception
                iFeatures = Nothing
            End Try

            If iFeatures Is Nothing Then
                iFeatures = New Hashtable
                HttpContext.Current.Session(InstalledFeaturesID) = iFeatures
            End If

            Return iFeatures
        End Get
        Set(ByVal value As Hashtable)
            HttpContext.Current.Session(InstalledFeaturesID) = value
        End Set
    End Property

    Public Shared Property InstalledFeaturesData() As Hashtable
        Get
            Dim iFeatures As Hashtable
            Try
                iFeatures = CType(HttpContext.Current.Session(InstalledFeaturesDataID), Hashtable)
            Catch ex As Exception
                iFeatures = Nothing
            End Try

            If iFeatures Is Nothing Then
                iFeatures = New Hashtable
                HttpContext.Current.Session(InstalledFeaturesDataID) = iFeatures
            End If

            Return iFeatures
        End Get
        Set(ByVal value As Hashtable)
            HttpContext.Current.Session(InstalledFeaturesID) = value
        End Set
    End Property

    Public Shared Property PassportPemissionsOverEmployee() As Hashtable
        Get
            Dim featureFermissions As Hashtable
            Try
                featureFermissions = CType(HttpContext.Current.Session(PassportPermissionOverEmployeeID), Hashtable)
            Catch ex As Exception
                featureFermissions = Nothing
            End Try

            If featureFermissions Is Nothing Then
                featureFermissions = New Hashtable
                HttpContext.Current.Session(PassportPermissionOverEmployeeID) = featureFermissions
            End If

            Return featureFermissions
        End Get
        Set(ByVal value As Hashtable)
            HttpContext.Current.Session(PassportPermissionOverEmployeeID) = value
        End Set
    End Property

    Public Shared Property PassportPemissionsOverEmployeeApp() As Hashtable
        Get
            Dim featureFermissions As Hashtable
            Try
                featureFermissions = CType(HttpContext.Current.Session(PassportPermissionOverEmployeeAppID), Hashtable)
            Catch ex As Exception
                featureFermissions = Nothing
            End Try

            If featureFermissions Is Nothing Then
                featureFermissions = New Hashtable
                HttpContext.Current.Session(PassportPermissionOverEmployeeAppID) = featureFermissions
            End If

            Return featureFermissions
        End Get
        Set(ByVal value As Hashtable)
            HttpContext.Current.Session(PassportPermissionOverEmployeeAppID) = value
        End Set
    End Property

    Public Shared Property PassportPemissionsOverGroup() As Hashtable
        Get
            Dim featureFermissions As Hashtable
            Try
                featureFermissions = CType(HttpContext.Current.Session(PassportPermissionOverGroupID), Hashtable)
            Catch ex As Exception
                featureFermissions = Nothing
            End Try

            If featureFermissions Is Nothing Then
                featureFermissions = New Hashtable
                HttpContext.Current.Session(PassportPermissionOverGroupID) = featureFermissions
            End If

            Return featureFermissions
        End Get
        Set(ByVal value As Hashtable)
            HttpContext.Current.Session(PassportPermissionOverGroupID) = value
        End Set
    End Property

    Public Shared Property PassportPemissionsOverGroupApp() As Hashtable
        Get
            Dim featureFermissions As Hashtable
            Try
                featureFermissions = CType(HttpContext.Current.Session(PassportPermissionOverGroupAppID), Hashtable)
            Catch ex As Exception
                featureFermissions = Nothing
            End Try

            If featureFermissions Is Nothing Then
                featureFermissions = New Hashtable
                HttpContext.Current.Session(PassportPermissionOverGroupAppID) = featureFermissions
            End If

            Return featureFermissions
        End Get
        Set(ByVal value As Hashtable)
            HttpContext.Current.Session(PassportPermissionOverGroupAppID) = value
        End Set
    End Property

    Public Shared Function ResetPassportPermissionCache(ByVal idPassport As Integer) As Boolean
        Dim bolRet As Boolean = False

        PassportFeautrePemissions = Nothing
        InstalledFeatures = Nothing
        InstalledFeaturesData = Nothing
        PassportPemissionsOverEmployee = Nothing
        PassportPemissionsOverEmployeeApp = Nothing
        PassportPemissionsOverGroup = Nothing
        PassportPemissionsOverGroupApp = Nothing

        Return bolRet
    End Function

#End Region

#Region "Cache del menu principal"

    Public Shared Property CurrentLanguage As String
        Get
            Dim _strLanguage As String = String.Empty
            Try
                _strLanguage = roTypes.Any2String(HttpContext.Current.Session(CurrentLanguageID))
            Catch ex As Exception
                _strLanguage = "ESP"
            End Try
            If _strLanguage.Trim.Length = 0 Then _strLanguage = "ESP"

            Return _strLanguage
        End Get
        Set(value As String)
            HttpContext.Current.Session(CurrentLanguageID) = value
        End Set
    End Property

    Public Shared Property MainMenu As wscMenuElementList
        Get
            Dim _mainMenu As wscMenuElementList
            Try
                _mainMenu = CType(HttpContext.Current.Session(PassportMainMenu), wscMenuElementList)
            Catch ex As Exception
                _mainMenu = Nothing
            End Try

            Return _mainMenu
        End Get
        Set(value As wscMenuElementList)
            HttpContext.Current.Session(PassportMainMenu) = value
        End Set
    End Property

    Public Shared Property ServerLicense As roVTLicense
        Get
            Dim _serverLicense As roVTLicense
            Try
                If VTBase.roConstants.GetGlobalEnvironmentParameter(GlobalAsaxParameter.CompanyId) <> String.Empty Then
                    If HttpContext.Current.Session(PassportLicenseCache) Is Nothing Then
                        _serverLicense = API.PortalServiceMethods.GetServerLicense(Nothing)
                        HttpContext.Current.Session(PassportLicenseCache) = _serverLicense
                    Else
                        _serverLicense = CType(HttpContext.Current.Session(PassportLicenseCache), roVTLicense)
                    End If
                Else
                    _serverLicense = Nothing
                End If
            Catch ex As Exception
                _serverLicense = Nothing
            End Try

            Return _serverLicense
        End Get
        Set(value As roVTLicense)
            HttpContext.Current.Session(PassportLicenseCache) = value
        End Set
    End Property

#End Region

#Region "Cache de alertas de sistema y usuario"

    Public Shared ReadOnly Property LastAlertsLoadDate() As DateTime
        Get
            Dim lastDate As DateTime = DateTime.Now
            Try
                Try
                    lastDate = CType(HttpContext.Current.Session(PassportAlertsCheckDate), DateTime)
                Catch ex As Exception
                    lastDate = roTypes.CreateDateTime(1970, 1, 1, 0, 0, 0)
                End Try
            Catch
                lastDate = roTypes.CreateDateTime(1970, 1, 1, 0, 0, 0)
            End Try

            Return lastDate
        End Get
    End Property

    Public Shared Property EmployeeDocumentationAlerts(Optional ByVal bolReload As Boolean = False, Optional checkStatusLevel As Boolean = False) As DocumentAlerts
        Get
            If Not (WLHelperWeb.CurrentPassportID > 0 AndAlso AuthHelper.GetPassportKeyValidated(WLHelperWeb.CurrentPassportID, WLHelperWeb.SecurityToken)) Then
                Return New DocumentAlerts
            End If


            Dim Result As DocumentAlerts = Nothing
            Dim lastDate As DateTime = DateTime.Now
            Try
                Result = CType(HttpContext.Current.Session(PassportEmployeeDocAlerts), DocumentAlerts)
                Try
                    lastDate = CType(HttpContext.Current.Session(PassportAlertsCheckDate), DateTime)
                Catch ex As Exception
                    lastDate = DateTime.Now
                End Try

                If Result Is Nothing OrElse bolReload Then ' ISM: Desactivamos temporizador de las alertas. Or Date.Now.Subtract(lastDate).TotalMinutes > 10 Then
                    HttpContext.Current.Session(PassportAlertsCheckDate) = Date.Now

                    Result = API.DocumentsServiceMethods.GetDocumentationFaults(Nothing, DocumentType.Employee,,,, checkStatusLevel)

                    HttpContext.Current.Session(PassportEmployeeDocAlerts) = Result
                End If
            Catch
                Result = Nothing
            End Try

            Return Result
        End Get
        Set(value As DocumentAlerts)
            HttpContext.Current.Session(PassportEmployeeDocAlerts) = value
        End Set
    End Property

    Public Shared Property CompanyDocumentationAlerts(Optional ByVal bolReload As Boolean = False, Optional checkStatusLevel As Boolean = False) As DocumentAlerts
        Get
            If Not (WLHelperWeb.CurrentPassportID > 0 AndAlso AuthHelper.GetPassportKeyValidated(WLHelperWeb.CurrentPassportID, WLHelperWeb.SecurityToken)) Then
                Return New DocumentAlerts
            End If


            Dim Result As DocumentAlerts = Nothing
            Dim lastDate As DateTime = DateTime.Now
            Try
                Result = CType(HttpContext.Current.Session(PassportCompanyDocAlerts), DocumentAlerts)
                Try
                    lastDate = CType(HttpContext.Current.Session(PassportAlertsCheckDate), DateTime)
                Catch ex As Exception
                    lastDate = DateTime.Now
                End Try

                If Result Is Nothing OrElse bolReload Then ' ISM: Desactivamos temporizador de las alertas. Or Date.Now.Subtract(lastDate).TotalMinutes > 10 Then
                    HttpContext.Current.Session(PassportAlertsCheckDate) = Date.Now

                    Result = API.DocumentsServiceMethods.GetDocumentationFaults(Nothing, DocumentType.Company,,,, checkStatusLevel)

                    HttpContext.Current.Session(PassportCompanyDocAlerts) = Result
                End If
            Catch
                Result = Nothing
            End Try

            Return Result
        End Get
        Set(value As DocumentAlerts)
            HttpContext.Current.Session(PassportCompanyDocAlerts) = value
        End Set
    End Property

    Public Shared Property UserAlerts(Optional ByVal bolReload As Boolean = False) As DataTable
        Get
            If Not (WLHelperWeb.CurrentPassportID > 0 AndAlso AuthHelper.GetPassportKeyValidated(WLHelperWeb.CurrentPassportID, WLHelperWeb.SecurityToken)) Then
                Return New DataTable
            End If

            Dim Result As DataTable = Nothing
            Dim lastDate As DateTime = DateTime.Now
            Try
                Result = CType(HttpContext.Current.Session(PassportUserAlerts), DataTable)
                Try
                    If (HttpContext.Current.Session(PassportAlertsCheckDate) IsNot Nothing) Then
                        lastDate = CType(HttpContext.Current.Session(PassportAlertsCheckDate), DateTime)
                    Else
                        lastDate = DateTime.Now
                        HttpContext.Current.Session(PassportAlertsCheckDate) = DateTime.Now
                    End If
                Catch ex As Exception
                    lastDate = DateTime.Now
                End Try

                If Result Is Nothing OrElse bolReload Then ' ISM: Desactivamos temporizador de las alertas. Or Date.Now.Subtract(lastDate).TotalMinutes > 10 Then
                    HttpContext.Current.Session(PassportAlertsCheckDate) = Date.Now

                    Dim tmpDs As DataSet = API.NotificationServiceMethods.GetDesktopAlerts(Date.MinValue, Nothing)
                    HttpContext.Current.Session(PassportAlertsCheckDate) = DateTime.Now
                    If (tmpDs IsNot Nothing AndAlso tmpDs.Tables.Count > 0) Then
                        Result = tmpDs.Tables(0)
                    Else
                        Result = New DataTable
                    End If
                    HttpContext.Current.Session(PassportUserAlerts) = Result
                End If
            Catch
                Result = Nothing
            End Try

            Return Result
        End Get
        Set(value As DataTable)
            HttpContext.Current.Session(PassportUserAlerts) = value
        End Set
    End Property

    Public Shared Property SystemAlerts(Optional ByVal bolReload As Boolean = False) As DataTable
        Get
            If Not (WLHelperWeb.CurrentPassportID > 0 AndAlso AuthHelper.GetPassportKeyValidated(WLHelperWeb.CurrentPassportID, WLHelperWeb.SecurityToken)) Then
                Return New DataTable
            End If

            Dim Result As DataTable = Nothing
            Try
                Result = CType(HttpContext.Current.Session(PassportSystemAlerts), DataTable)
                If Result Is Nothing OrElse bolReload Then
                    Result = API.UserTaskServiceMethods.GetUserTasks(Nothing, Robotics.Base.VTBusiness.UserTask.TaskType.UserTaskRepair, Robotics.Base.VTBusiness.UserTask.TaskCompletedState.All)

                    HttpContext.Current.Session(PassportSystemAlerts) = Result
                End If
            Catch
                Result = Nothing
            End Try

            Return Result
        End Get
        Set(value As DataTable)
            HttpContext.Current.Session(PassportSystemAlerts) = value
        End Set
    End Property

    Public Shared Property UpdateVTNotification() As Boolean
        Get
            Dim notificationShown As Boolean
            Try
                notificationShown = CType(HttpContext.Current.Session(updateNotificationID), Boolean)
            Catch ex As Exception
                notificationShown = False
            End Try

            Return notificationShown
        End Get
        Set(ByVal value As Boolean)
            HttpContext.Current.Session(updateNotificationID) = value
        End Set
    End Property

#End Region

    ''' <summary>
    ''' Property CurrentPassport is used to get or set the passport ticket for the logged on user.
    ''' </summary>
    Public Shared Property CurrentPassport(Optional ByVal onlyExists As Boolean = False) As roPassportTicket
        Get
            Dim Result As roPassportTicket = Nothing
            Dim oSessionCheck As Date = DateTime.MinValue
            Dim curPassportTicketID As Integer = -1

            Try
                Result = CType(HttpContext.Current.Session(SessionTicket), roPassportTicket)
            Catch
                Result = Nothing
            End Try

            Try
                curPassportTicketID = roTypes.Any2Integer(HttpContext.Current.Session(SessionTicketID))
            Catch
                curPassportTicketID = -1
            End Try

            Try
                oSessionCheck = CType(HttpContext.Current.Session(SessionPassportCheck), Date)
            Catch ex As Exception
                oSessionCheck = Date.Now.AddMinutes(-10)
            End Try

            If roTypes.Any2String(HttpContext.Current.Session("roMultiCompanyId")) = String.Empty Then
                Return Nothing
            End If

            If onlyExists Then
                Return Result
            End If

            Dim invalidateSession As Boolean = False

            Try
                Dim NewTicket As roPassportTicket = Result
                If NewTicket Is Nothing Then
                    NewTicket = API.SecurityServiceMethods.GetPassportTicketBySessionID(Nothing, curPassportTicketID, WLHelperWeb.CurrentPassportAuthMethod)
                End If

                If WLHelperWeb.SecurityToken = String.Empty AndAlso NewTicket IsNot Nothing Then
                    Dim tmpCookieValue As String = HelperWeb.GetCookie("ro_SessionID")
                    Dim unParsedString As String = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(tmpCookieValue))
                    Dim tokens As String() = unParsedString.Split("_roSecurityToken_")

                    HttpContext.Current.Session("WLPASSPORT_GUID") = tokens(2)
                    WLHelperWeb.SecurityToken = tokens(0)
                    roWsUserManagement.ResetSessionObject()
                End If

                If NewTicket Is Nothing Then
                    Result = Nothing
                    invalidateSession = False
                Else
                    Dim now As Date = DateTime.Now
                    If (now - oSessionCheck).TotalSeconds > 60 Then
                        If NewTicket.IsPrivateUser Then
                            Result = NewTicket
                        Else
                            Dim oldSecToken As String = WLHelperWeb.SecurityToken

                            If NewTicket.IsSSO OrElse WLHelperWeb.CurrentPassportAuthMethod = AuthenticationMethod.IntegratedSecurity Then

                                Dim credential As String = String.Empty
                                If CurrentPassportAuthMethod = AuthenticationMethod.Password Then
                                    credential = NewTicket.AuthCredential
                                ElseIf CurrentPassportAuthMethod = AuthenticationMethod.IntegratedSecurity Then
                                    credential = NewTicket.CegidIdCredential
                                End If

                                Dim authPassport As roPassportTicket = API.SecurityServiceMethods.Authenticate(Nothing, NewTicket, WLHelperWeb.CurrentPassportAuthMethod, credential, "", False, , , , True, , , True, False)
                                Dim bolExpired As Boolean = roWsUserManagement.SessionObject.States.SecurityState.Result = SecurityResultEnum.IsExpired
                                Dim needTemporanyKey As Boolean = roWsUserManagement.SessionObject.States.SecurityState.Result = SecurityResultEnum.NeedTemporaryKeyRequest
                                Dim TemporanyKeyExpired As Boolean = roWsUserManagement.SessionObject.States.SecurityState.Result = SecurityResultEnum.NeedTemporaryKeyRequestExpired

                                If authPassport IsNot Nothing Then
                                    If WLHelperWeb.CurrentPassportAuthMethod = AuthenticationMethod.IntegratedSecurity OrElse
                                                WLHelperWeb.ValidatePasswordState(needTemporanyKey, TemporanyKeyExpired, bolExpired, NewTicket.IsSSO) Then
                                        Result = NewTicket
                                    Else
                                        invalidateSession = True
                                    End If
                                Else
                                    invalidateSession = True
                                End If
                            Else
                                If Not AuthHelper.GetPassportKeyValidated(NewTicket.ID, WLHelperWeb.SecurityToken) Then
                                    Result = Nothing
                                    invalidateSession = True
                                    roWsUserManagement.SessionObject().States.SecurityState.Result = SecurityResultEnum.IsExpired
                                Else

                                    Dim strDefaultDomain As String = roTypes.Any2String(HelperSession.AdvancedParametersCache("VTLive.AD.DefaultDomain"))

                                    Dim credential As String = NewTicket.AuthCredential

                                    If strDefaultDomain.Trim <> String.Empty AndAlso Not NewTicket.IsSSO Then
                                        credential = ".\" & credential
                                    End If

                                    Dim authPassport As roPassportTicket = API.SecurityServiceMethods.Authenticate(Nothing, NewTicket, CurrentPassportAuthMethod, credential, "", False, , , , True, , , , False)
                                    Dim bolExpired As Boolean = roWsUserManagement.SessionObject.States.SecurityState.Result = SecurityResultEnum.IsExpired
                                    Dim needTemporanyKey As Boolean = roWsUserManagement.SessionObject.States.SecurityState.Result = SecurityResultEnum.NeedTemporaryKeyRequest
                                    Dim TemporanyKeyExpired As Boolean = roWsUserManagement.SessionObject.States.SecurityState.Result = SecurityResultEnum.NeedTemporaryKeyRequestExpired

                                    If authPassport IsNot Nothing Then
                                        If CurrentPassportAuthMethod = AuthenticationMethod.IntegratedSecurity OrElse
                                                WLHelperWeb.ValidatePasswordState(needTemporanyKey, TemporanyKeyExpired, bolExpired, authPassport.IsSSO) Then
                                            Result = NewTicket
                                        Else
                                            invalidateSession = True
                                        End If
                                    Else
                                        invalidateSession = True
                                    End If
                                End If

                            End If

                            If Not invalidateSession AndAlso oldSecToken <> WLHelperWeb.SecurityToken Then
                                roWsUserManagement.ResetSessionObject()
                            End If
                        End If
                        HttpContext.Current.Session(SessionPassportCheck) = Date.Now
                    Else
                        Result = NewTicket
                    End If
                End If
            Catch ex As Exception
                Result = Nothing
                invalidateSession = True
            End Try

            If invalidateSession AndAlso Result IsNot Nothing Then
                SignOut(Nothing, Result)
                RedirectNotAuthenticated()
            End If
            'End If

            If Result Is Nothing AndAlso roWsUserManagement.SessionObject.States.SecurityState.Result = SecurityResultEnum.SessionExpired Then
                HelperWeb.CreateCookie("SessionExpired", "true")
                Return Nothing
            End If

            If Result Is Nothing AndAlso (roWsUserManagement.SessionObject.States.SecurityState.Result = SecurityResultEnum.SessionInvalidatedOtherUserWithSameSession) Then
                HelperWeb.CreateCookie("UpdateSessionError", "true")
                Return Nothing
            End If

            If Result Is Nothing AndAlso roWsUserManagement.SessionObject.States.SecurityState.Result = SecurityResultEnum.AlreadyLoggedinInOtherLocation Then
                HelperWeb.CreateCookie("AlreadyLoggedinInOtherLocation", "true")
                Return Nothing
            End If

            If Result IsNot Nothing AndAlso roWsUserManagement.SessionObject.States.SecurityState.Result = SecurityResultEnum.SessionInvalidatedByPermissions Then
                WLHelperWeb.ResetPassportPermissionCache(Result.ID)
            End If

            If Result Is Nothing Then
                Dim oPassportGUID = roTypes.Any2String(HttpContext.Current.Session("WLPASSPORT_GUID"))
                Dim oldGUID As String = roConstants.GetGlobalEnvironmentParameter(GlobalAsaxParameter.RequestGUID)

                HttpContext.Current.Session(SessionTicket) = Nothing
                HttpContext.Current.Session(SessionPassportCheck) = roTypes.CreateDateTime(1900, 1, 1)
                HttpContext.Current.Session(SessionTimeoutCheck) = roTypes.CreateDateTime(1900, 1, 1)
                HttpContext.Current.Session(SessionTicketID) = -1
                HttpContext.Current.Session(SessionGroupType) = Nothing
                HttpContext.Current.Session(SessionConsutant) = Nothing

                HttpContext.Current.Session.RemoveAll()
                HttpContext.Current.Session("WLPASSPORT_GUID") = oPassportGUID
                HttpContext.Current.Session("roRequestGUID") = oldGUID
            Else
                HttpContext.Current.Session(SessionTicket) = Result
                HttpContext.Current.Session(SessionTicketID) = Result.ID
            End If

            ''End If
            Return Result
        End Get
        Set(ByVal Value As roPassportTicket)
            If Value Is Nothing Then
                Dim oldGUID As String = roConstants.GetGlobalEnvironmentParameter(GlobalAsaxParameter.RequestGUID)

                HttpContext.Current.Session(SessionTicket) = Nothing
                HttpContext.Current.Session(SessionPassportCheck) = DateTime.MinValue
                HttpContext.Current.Session(SessionTimeoutCheck) = DateTime.MinValue
                HttpContext.Current.Session(SessionTicketID) = -1
                HttpContext.Current.Session(PassportLicenseCache) = Nothing
                HttpContext.Current.Session(SessionGroupType) = Nothing
                HttpContext.Current.Session(SessionConsutant) = Nothing
                HttpContext.Current.Session.RemoveAll()
                HttpContext.Current.Session.Abandon()
                HttpContext.Current.Session("roRequestGUID") = oldGUID
            Else
                HttpContext.Current.Session(SessionTicket) = Value
                HttpContext.Current.Session(SessionTicketID) = Value.ID
                HttpContext.Current.Session(SessionPassportCheck) = Date.Now
                HttpContext.Current.Session(SessionTimeoutCheck) = Date.Now
            End If
        End Set
    End Property

    Public Shared Function ValidatePasswordState(ByVal needTemporanyKey As Boolean, ByVal temporanyKeyExpired As Boolean, ByVal bolExpired As Boolean, ByVal isSSO As Boolean) As Boolean
        Dim continueLoading As Boolean = True
        If needTemporanyKey OrElse temporanyKeyExpired Then
            continueLoading = False
        End If

        If continueLoading AndAlso Not isSSO Then
            If bolExpired Then
                ' Si la cuenta esta caducada , forzamos a cambiar la contraseña
                HttpContext.Current.Session.Add("PASSWORDEXPIRED", True)
            Else
                HttpContext.Current.Session("PASSWORDEXPIRED") = False
                HttpContext.Current.Session("LOPD") = False
            End If
        End If

        Return continueLoading
    End Function

    ''' <summary>
    ''' Redirects to the login page if current user is not authenticated.
    ''' </summary>
    Public Shared Sub RedirectNotAuthenticated()
        If CurrentPassport(True) Is Nothing Then
            WLHelperWeb.RedirectToUrl(WLHelperWeb.NotAuthenticatedRedirectUrl)
        End If
    End Sub

    ''' <summary>
    ''' Redirects to the access denied page.
    ''' </summary>
    Public Shared Sub RedirectAccessDenied(Optional ByVal bolCloseButton As Boolean = True)
        RedirectToUrl(AccessDeniedRedirectUrl & "?CloseButton=" & IIf(bolCloseButton, "true", "false"))
    End Sub

    ''' <summary>
    ''' Redirects to the default page
    ''' </summary>
    Public Shared Sub RedirectDefault()
        RedirectToUrl(DefaultRedirectUrl)
    End Sub

    Public Shared Sub RedirectToVTLogin()
        Dim iSSOVersion As Integer = Robotics.VTBase.roTypes.Any2Integer(HelperSession.AdvancedParametersCache("VisualTime.SSO.ConfigurationVersion"))
        Dim ssoType As String = Robotics.VTBase.roTypes.Any2String(HelperSession.AdvancedParametersCache("SSOType"))

        If ssoType = "CEGIDID" Then
            RedirectToUrl(DefaultVTLogin_CEGIDID & "?id=" & HttpContext.Current.Session("roMultiCompanyId").ToString.ToLower.Trim)
        Else
            If iSSOVersion < 2 Then
                RedirectToUrl(DefaultVTLogin_V1_RedirectUrl)
            Else
                RedirectToUrl(DefaultVTLogin_V2_RedirectUrl & HttpContext.Current.Session("roMultiCompanyId").ToString.ToLower.Trim)
            End If
        End If



    End Sub

    Public Shared Sub RedirectToUrl(ByVal url As String)
        Try
            Dim cPage As Page = CType(HttpContext.Current.Handler, Page)
            If cPage IsNot Nothing Then
                If cPage.IsCallback Then
                    DevExpress.Web.ASPxWebControl.RedirectOnCallback(url)
                Else
                    HttpContext.Current.Response.Redirect(url, False)
                    HttpContext.Current.ApplicationInstance.CompleteRequest()
                End If
            Else
                HttpContext.Current.Response.Redirect(url, False)
                HttpContext.Current.ApplicationInstance.CompleteRequest()
            End If
        Catch ex As Exception
            DevExpress.Web.ASPxWebControl.RedirectOnCallback(url)
        End Try
    End Sub


    Public Shared Sub CommitPassportLanguage(ByVal passportTicket As roPassportTicket, ByVal defaultSelected As String)
        Dim languageKey As String

        If passportTicket IsNot Nothing Then
            languageKey = passportTicket.Language.Key
        Else
            languageKey = defaultSelected
        End If

        If HttpContext.Current IsNot Nothing AndAlso (WLHelperWeb.CurrentLanguage = String.Empty OrElse WLHelperWeb.CurrentLanguage <> languageKey) Then
            WLHelperWeb.CurrentLanguage = languageKey
            WLHelperWeb.MainMenu = Nothing
        End If

        Dim jsLocale As String = "es"
        Select Case languageKey
            Case "CAT"
                jsLocale = "ca"
            Case "ESP"
                jsLocale = "es"
            Case "ENG"
                jsLocale = "en"
            Case "GAL"
                jsLocale = "gl"
            Case "POR"
                jsLocale = "pt"
            Case "FRA"
                jsLocale = "fr"
            Case "EKR"
                jsLocale = "eu"
            Case "CAT"
                jsLocale = "sk"
        End Select


        HelperWeb.EraseCookie("VTLive_Language")
        HelperWeb.CreateCookie("VTLive_Language", jsLocale, False)

        HelperWeb.EraseCookie("DefaultLanguageKey")
        HelperWeb.CreateCookie("DefaultLanguageKey", WLHelperWeb.CurrentLanguage)
    End Sub

    Public Shared Function GetLastLanguageUsed() As String
        Return roTypes.Any2String(HelperWeb.GetCookie("DefaultLanguageKey"))
    End Function


    Public Shared Sub SetLanguage(ByRef oLanguage As roLanguageWeb, ByVal strFileReference As String, Optional ByVal oPage As Page = Nothing, Optional ByVal _PassportTicket As roPassportTicket = Nothing)

        Dim strLanguage As String = ""

        If _PassportTicket IsNot Nothing Then
            strLanguage = _PassportTicket.Language.Key
        Else
            If CurrentPassport(True) IsNot Nothing Then
                strLanguage = CurrentPassport(True).Language.Key
            Else
                Dim lastLangUsed As String = GetLastLanguageUsed()
                If Not String.IsNullOrEmpty(lastLangUsed) Then
                    strLanguage = lastLangUsed
                Else
                    strLanguage = API.CommonServiceMethods.DefaultLanguage
                End If

            End If
        End If

        If HttpContext.Current IsNot Nothing AndAlso (WLHelperWeb.CurrentLanguage = String.Empty OrElse WLHelperWeb.CurrentLanguage <> strLanguage) Then
            WLHelperWeb.CurrentLanguage = strLanguage
            WLHelperWeb.MainMenu = Nothing
        End If

        oLanguage.SetLanguageReference(strFileReference, strLanguage)

        If HttpContext.Current IsNot Nothing Then HttpContext.Current.Session(PassportCurrentCulture) = oLanguage.GetLanguageKey()
    End Sub

    Public Shared Function CurrentCulture() As String

        Dim _currentCulture As String = String.Empty
        Try
            _currentCulture = VTBase.roTypes.Any2String(HttpContext.Current.Session(PassportCurrentCulture))
            If _currentCulture = String.Empty Then _currentCulture = "es-ES"
        Catch ex As Exception
            _currentCulture = "es-ES"
        End Try

        If WLHelperWeb.CurrentPassport(True) IsNot Nothing Then
            _currentCulture = WLHelperWeb.CurrentPassport(True).Language.Culture
        End If
        Return _currentCulture
    End Function

    Public Shared Function CurrentExtLanguage() As String
        Dim strLanguage As String = "sp"
        If WLHelperWeb.CurrentPassport(True) IsNot Nothing Then
            Dim oParameters As New roCollection(WLHelperWeb.CurrentPassport(True).Language.ParametersXml)
            If oParameters.Item("ExtLanguage").ToString <> "" Then
                strLanguage = oParameters.Item("ExtLanguage")
            End If
        End If
        Return strLanguage
    End Function

    Public Shared Function CurrentExtDatePicketFormat() As String
        Dim strFormat As String = "d/m/Y"
        If WLHelperWeb.CurrentPassport(True) IsNot Nothing Then
            Dim oParameters As New roCollection(WLHelperWeb.CurrentPassport(True).Language.ParametersXml)
            If oParameters.Item("ExtDatePickerFormat").ToString <> "" Then
                strFormat = oParameters.Item("ExtDatePickerFormat")
            End If
        End If
        Return strFormat
    End Function

    Public Shared Function CurrentExtDatePicketStartDay() As Integer
        Dim intStartDay As Integer = 1
        If WLHelperWeb.CurrentPassport(True) IsNot Nothing Then
            Dim oParameters As New roCollection(WLHelperWeb.CurrentPassport(True).Language.ParametersXml)
            If oParameters.Item("ExtDatePickerStartDay") IsNot Nothing Then
                intStartDay = oParameters.Item("ExtDatePickerStartDay")
            End If
        End If
        Return intStartDay
    End Function

    ''Private Shared oContext As CContext = Nothing
    Public Shared Function GetSessionContext() As WebCContext
        Return CType(HttpContext.Current.Session("WLSessionContext"), WebCContext)
    End Function

    Public Shared Sub SetSessionContext(ByVal oContext As WebCContext)
        If oContext Is Nothing Then
            HttpContext.Current.Session.Remove("WLSessionContext")
        Else
            HttpContext.Current.Session("WLSessionContext") = oContext
        End If

    End Sub

    Public Shared Function Context(ByVal oRequest As HttpRequest, Optional ByVal idPassport As Integer = -1, Optional ByVal bolUpdateSession As Boolean = False, Optional ByVal excludeState As Boolean = False) As WebCContext

        If idPassport = -1 Then
            If WLHelperWeb.CurrentPassport(True) IsNot Nothing Then _
                idPassport = WLHelperWeb.CurrentPassport(True).ID
        End If

        Dim oContext As WebCContext = Nothing

        If idPassport <> -1 Then

            Dim strApplicationPath As String = ""
            If oRequest IsNot Nothing Then strApplicationPath = oRequest.ApplicationPath

            If Not bolUpdateSession Then
                oContext = GetSessionContext()
            End If

            If oContext Is Nothing OrElse oContext.IDPassport <> idPassport OrElse oContext.AppContext <> strApplicationPath Then
                oContext = New WebCContext(idPassport, strApplicationPath, excludeState)
            End If
        Else
            oContext = Nothing
        End If

        SetSessionContext(oContext)

        Return oContext

    End Function

    Public Shared Sub IniContext()
        SetSessionContext(Nothing)
        'oContext = Nothing
    End Sub

    Public Shared Function GetCurrentPassportPhoto() As Byte()
        Dim bImage As Byte() = Nothing
        Dim oPassport As roPassportTicket = WLHelperWeb.CurrentPassport(True)
        If oPassport IsNot Nothing Then

            If oPassport.IDEmployee.HasValue Then
                bImage = GetEmployeeImage(oPassport.IDEmployee.Value)
            Else
                bImage = Nothing
            End If
        End If
        Return bImage
    End Function

    Private Shared Function GetEmployeeImage(ByVal _IDEmployee As Integer) As Byte()

        Dim bImage(-1) As Byte

        Try

            Dim oEmployee As Robotics.Base.VTEmployees.Employee.roEmployee = API.EmployeeServiceMethods.GetEmployee(Nothing, _IDEmployee, False)
            If oEmployee IsNot Nothing Then
                bImage = oEmployee.Image
            End If
        Catch ex As Exception
            'do nothing
        End Try

        Return bImage

    End Function

    Public Shared Function ResetValidationCodeRobotics(ByVal IDPassport As Integer, ByVal strClientLocation As String) As Boolean
        '
        ' En el caso que el codigo de validacion sea correcto, reseteamos los campos para que no vuelva a pedir el código
        '

        Dim bolret As Boolean = False
        Try
            bolret = API.SecurityServiceMethods.ResetValidationCodeRobotics(Nothing, IDPassport)
        Catch ex As Exception
            bolret = False
        End Try

        Return bolret

    End Function

    Public Shared Function ResetValidationCode(ByVal IDPassport As Integer, ByVal strClientLocation As String) As Boolean
        '
        ' En el caso que el codigo de validacion sea correcto, reseteamos los campos para que no vuelva a pedir el código
        '

        Dim bolret As Boolean = False
        Try
            bolret = API.SecurityServiceMethods.ResetValidationCode(Nothing, IDPassport, strClientLocation)
        Catch ex As Exception
            bolret = False
        End Try

        Return bolret

    End Function

    Public Shared Function IsValidCode(ByVal IDPassport As Integer, ByVal strCode As String) As Boolean
        '
        ' Validams el codigo de validacion
        '

        Dim bolret As Boolean = False
        Try
            bolret = API.SecurityServiceMethods.IsValidCode(Nothing, IDPassport, strCode)
        Catch ex As Exception
            bolret = False
        End Try

        Return bolret

    End Function

    Public Shared Sub SetLastAnalyticAccessData()
        HttpContext.Current.Session(AnalyticsDataClean) = DateTime.Now
    End Sub

    Public Shared Function RemoveUserSessionData() As Boolean

        Dim bNeedToClean As Boolean = False
        Try
            Dim oLastAnalyticsAccess As DateTime = DateTime.Now
            If HttpContext.Current.Session(AnalyticsDataClean) IsNot Nothing Then
                oLastAnalyticsAccess = CType(HttpContext.Current.Session(AnalyticsDataClean), Date)
            End If

            If Date.Now.Subtract(oLastAnalyticsAccess).TotalSeconds > 300 Then
                bNeedToClean = True
            End If
        Catch ex As Exception
            bNeedToClean = False
        End Try

        If bNeedToClean Then
            HttpContext.Current.Session("AnalyticsScheduler_AnaliticsData") = Nothing
            HttpContext.Current.Session("AnalyticsCost_AnaliticsData") = Nothing
            HttpContext.Current.Session("AnalyticsAccess_AnaliticsData") = Nothing
            HttpContext.Current.Session("AnalyticsTask_TasksData") = Nothing
            HttpContext.Current.Session(AnalyticsDataClean) = Nothing
        End If

    End Function

    Public Shared Function GetAdvancedParameterValue(strTreeKey As String, sKeytoFound As String) As String
        Try
            Dim advancedParameter As roAdvancedParameter = API.CommonServiceMethods.GetAdvancedParameter(Nothing, sKeytoFound)
            If Not advancedParameter.Exists Then
                advancedParameter.Value = ""

                Dim _RegistryRoot As String = "HKEY_LOCAL_MACHINE\Software\"
                If Microsoft.Win32.Registry.GetValue("HKEY_LOCAL_MACHINE\Software\Wow6432node\Robotics\VisualTime\Server", "Running", "False") <> Nothing Then
                    _RegistryRoot = "HKEY_LOCAL_MACHINE\Software\Wow6432node\"
                End If

                Dim registryKeyValue = Microsoft.Win32.Registry.GetValue(_RegistryRoot & "Robotics\VisualTime\" & strTreeKey, sKeytoFound, advancedParameter.Value)
                If registryKeyValue IsNot Nothing Then
                    advancedParameter.Value = registryKeyValue.ToString()
                End If

                API.CommonServiceMethods.SaveAdvancedParameter(Nothing, advancedParameter, False)
            End If
            Return advancedParameter.Value
        Catch ex As Exception
            Return String.Empty
        End Try
    End Function

    ''' <summary>
    ''' Obtiene el nobre y carpeta del servidor ej:/VTLive
    ''' </summary>
    ''' <param name="oPage"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function ApplicationPath(ByVal oPage As Page) As String
        Dim path As String = "/"
        Try
            oPage.ResolveUrl("~")
            If path(path.Length - 1) <> "/" Then path += "/"
        Catch ex As Exception
            'do nothing    
        End Try
        Return path
    End Function

    ''' <summary>
    '''  Obtiene la ruta relativa de ej: /VTLive/#/VTLive/"
    ''' </summary>
    ''' <param name="oPage"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function RelativeURL(ByVal oPage As Page) As String
        Dim path As String = ""
        Try
            path = ApplicationPath(oPage) + "#/"
            If Configuration.RootUrl(0) = "/" Then
                path += Configuration.RootUrl.Substring(1)
            Else
                path += Configuration.RootUrl
            End If
            If path(path.Length - 1) <> "/" Then path += "/"
        Catch ex As Exception
            'do nothing
        End Try
        Return path
    End Function

End Class