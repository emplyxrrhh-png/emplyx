Imports System.Web
Imports Robotics.Azure
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness
Imports Robotics.Base.VTBusiness.Terminal
Imports Robotics.Security
Imports Robotics.Security.Base
Imports Robotics.VTBase

Public Class ApiSession

    Private Const VALIDATE_SESSION As Integer = 300 ' 5 minutes

    Private Sub New()
    End Sub


    Public Shared Function GetNewApiSession(ByVal roAuth As String) As roApiSession
        Return New roApiSession With {
                    .SessionKey = roAuth,
                    .LoggedIn = False,
                    .IdIdentity = -1,
                    .Identity = Nothing,
                    .IdSupervisor = -1,
                    .Supervisor = Nothing,
                    .IdTerminal = -1,
                    .Terminal = Nothing,
                    .CompanyId = Robotics.Azure.RoAzureSupport.GetCompanyName().Trim.ToLower,
                    .Location = roBaseState.GetClientAddress(System.Web.HttpContext.Current.Request),
                    .TimeZone = TimeZoneInfo.Local,
                    .IsRegistered = False,
                    .TerminalSecurityToken = String.Empty,
                    .TerminalResult = TerminalBaseResultEnum.NoError,
                    .LoginMethod = CInt(AuthenticationMethod.Password),
                    .CurrentToken = "",
                    .LastRequest = roTypes.CreateDateTime(1970, 1, 1)
                }

    End Function

    Public Shared Function PrepareVTPortalSession(roAuth As String, oApiSession As roApiSession, ByVal isLogEnabled As Boolean, ByVal updateSessionExculdeMethods As String()) As roApiSession
        Dim applicationSource As roAppSource
        Try
            applicationSource = oApiSession.ApplicationSource
            If HttpContext.Current IsNot Nothing AndAlso HttpContext.Current.Request IsNot Nothing AndAlso oApiSession.CompanyId = RoAzureSupport.GetCompanyName().Trim.ToLower AndAlso oApiSession.CompanyId <> String.Empty Then
                Dim roToken As String = If(HttpContext.Current.Request.Headers("roToken") IsNot Nothing, HttpContext.Current.Request.Headers("roToken"), "")
                Dim roAlias As String = If(HttpContext.Current.Request.Headers("roAlias") IsNot Nothing, HttpContext.Current.Request.Headers("roAlias"), "")
                Dim accessFromApp As Boolean = If(HttpContext.Current.Request.Headers("roSrc") IsNot Nothing, roTypes.Any2Boolean(HttpContext.Current.Request.Headers("roSrc")), False)

                If Not accessFromApp AndAlso HttpContext.Current.Request.Url.AbsolutePath.EndsWith("/AuthenticateSession") Then
                    roToken = Robotics.Web.Base.CookieSession.GetAuthenticationCookie(StrConv(roAppType.VTPortal.ToString(), VbStrConv.ProperCase))
                End If

                Dim idOldIdentity As Integer = -1
                If oApiSession.IdIdentity > 0 Then
                    idOldIdentity = oApiSession.IdIdentity
                End If

                Dim oState As New roSecurityState()
                roBaseState.SetSessionSmall(oState, -1, roAppSource.VTPortal, "")

                Dim timeZone As String = String.Empty
                Dim excludeUpdateSession As Boolean = False

                For Each oExcludedFunction In updateSessionExculdeMethods
                    If HttpContext.Current.Request.Url.PathAndQuery.Contains(oExcludedFunction) Then excludeUpdateSession = True
                Next

                Dim bChangedAlias As Boolean = False
                If oApiSession.Supervisor IsNot Nothing AndAlso oApiSession.IdIdentity <> AuthHelper.GetkAliasPassportId(roAlias, oState) Then
                    bChangedAlias = True
                End If

                'Si tengo session, con el mismo id, token y no ha cambiado la impersonalización, no refresco la sesión
                'Se hará una vez cada 5 minutos aunque no haya tocado nada
                If oApiSession.Identity IsNot Nothing AndAlso oApiSession.SessionKey = roAuth AndAlso oApiSession.CurrentToken = roToken AndAlso
                    oApiSession.ApplicationSource = applicationSource AndAlso Not bChangedAlias AndAlso (roTypes.UnspecifiedNow - oApiSession.LastRequest).TotalSeconds < VALIDATE_SESSION Then
                    Return oApiSession
                End If

                Dim oAuthMethod As AuthenticationMethod = AuthHelper.GetAuthMethodUsed(roAuth, roToken, oState)
                Dim oLoggedInPassport As roPassportTicket = AuthHelper.ValidateSecurityTokens(roAuth, roToken, timeZone, excludeUpdateSession, oAuthMethod, oState)
                Dim oImpersonatedPassport As roPassportTicket = AuthHelper.CheckAliasPassport(roAlias, oState)

                'Añadir validación de supervisor, si tiene apps bloqueadas, etc.
                If (oLoggedInPassport IsNot Nothing AndAlso Not roPassportManager.CheckIfPassportHasPermissionsOverVTPortal(oLoggedInPassport, accessFromApp, oState)) Then
                    oLoggedInPassport = Nothing
                    oImpersonatedPassport = Nothing
                End If

                If oLoggedInPassport IsNot Nothing AndAlso oLoggedInPassport.ID <> idOldIdentity AndAlso idOldIdentity <> -1 Then
                    oApiSession = Robotics.Web.Base.ApiSession.GetNewApiSession(roAuth)
                    oApiSession.ApplicationSource = applicationSource
                End If

                oApiSession.LoginMethod = CInt(oAuthMethod)
                SetSessionIdentities(oApiSession, oLoggedInPassport, oImpersonatedPassport, roAuth, roToken, oAuthMethod, oState)

                If oApiSession.IdTerminal > 0 Then
                    Dim idEmployee As Integer = If(oApiSession.Identity?.IDEmployee, -1)

                    Dim oTerminals As Generic.List(Of Terminal.roTerminal) = Terminal.roTerminal.GetEmployeeTerminals(idEmployee, "LIVEPORTAL", New Terminal.roTerminalState(-1))
                    If oTerminals.Count > 0 Then
                        oApiSession.IdTerminal = oTerminals(0).ID
                        oApiSession.Terminal = oTerminals(0)
                    End If
                End If

                If isLogEnabled Then
                    Dim strParameters As String = String.Empty
                    If HttpContext.Current.Request.Url.PathAndQuery.IndexOf("?") > 0 Then strParameters = HttpContext.Current.Request.Url.PathAndQuery.Substring(HttpContext.Current.Request.Url.PathAndQuery.IndexOf("?") + 1)

                    Dim strPath As String = String.Empty
                    If HttpContext.Current.Request.Url.AbsolutePath.LastIndexOf("/") > 0 Then strPath = HttpContext.Current.Request.Url.AbsolutePath.Substring(HttpContext.Current.Request.Url.AbsolutePath.LastIndexOf("/") + 1)

                    If oApiSession.IdIdentity > 0 Then
                        roLog.GetInstance.logMessage(roLog.EventType.roDebug, "User::" & oApiSession.Identity.ID & "(" & oApiSession.Identity.Name & ")::" & strPath & "::" & strParameters)
                    Else
                        roLog.GetInstance.logMessage(roLog.EventType.roDebug, "NotLoggedInUser::" & strPath & "::" & strParameters)
                    End If
                End If

                If oApiSession.IdIdentity > 0 Then
                    If Not oApiSession.SupervisorPortalEnabled.HasValue Then
                        Dim oSupPassport As roPassportTicket = Nothing
                        If oLoggedInPassport IsNot Nothing Then
                            oSupPassport = roPassportManager.GetPassportTicket(oLoggedInPassport.ID)
                            oApiSession.SupervisorPortalEnabled = oSupPassport.IsSupervisor
                        End If

                    End If

                    oApiSession.LoggedIn = True
                    oApiSession.LastRequest = DateTime.Now
                Else
                    oApiSession.LoggedIn = False
                    oApiSession.LastRequest = DateTime.Now
                End If

                If timeZone <> String.Empty Then
                    Dim oZoneInfo As TimeZoneInfo = roSupport.OlsonTimeZoneToTimeZoneInfo(timeZone)
                    If oZoneInfo Is Nothing Then
                        oZoneInfo = TimeZoneInfo.Local
                    End If

                    oApiSession.TimeZone = oZoneInfo
                Else
                    oApiSession.TimeZone = TimeZoneInfo.Local
                End If

                oApiSession.CurrentToken = roToken
                oApiSession.SecurityResult = oState.Result
            Else
                roLog.GetInstance().logMessage(roLog.EventType.roDebug, "GlobalAsax::PrepareVTPortalSession::Could not obtain VTPortal session info")
                oApiSession = Robotics.Web.Base.ApiSession.GetNewApiSession(roAuth)
                oApiSession.ApplicationSource = applicationSource
            End If
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "GlobalAsax::Error::PrepareVTPortalSession::Could not obtain VTPortal session info", ex)
            oApiSession = Robotics.Web.Base.ApiSession.GetNewApiSession(roAuth)
            oApiSession.ApplicationSource = applicationSource
        End Try

        Return oApiSession
    End Function

    Private Shared Sub SetSessionIdentities(ByRef oApiSession As roApiSession, oLoggedInPassport As roPassportTicket, oImpersonatedPassport As roPassportTicket, roAuth As String, roToken As String, oAuthMethod As AuthenticationMethod, oState As roSecurityState)
        If oImpersonatedPassport IsNot Nothing Then
            oApiSession.IdIdentity = oImpersonatedPassport.ID
            oApiSession.Identity = oImpersonatedPassport

            If oLoggedInPassport IsNot Nothing Then
                oApiSession.IdSupervisor = oLoggedInPassport.ID
                oApiSession.Supervisor = oLoggedInPassport
                SessionHelper.SetSessionAsSupervisor(oLoggedInPassport.ID, roAuth, roToken, oAuthMethod, oState)
            Else
                oApiSession.IdSupervisor = -1
                oApiSession.Supervisor = Nothing
            End If
        Else
            If oLoggedInPassport IsNot Nothing Then
                oApiSession.IdIdentity = oLoggedInPassport.ID
                oApiSession.Identity = oLoggedInPassport
                If HttpContext.Current.Request.Url.AbsolutePath.EndsWith("/Authenticate") OrElse HttpContext.Current.Request.Url.AbsolutePath.EndsWith("/Login") OrElse
                    HttpContext.Current.Request.Url.AbsolutePath.EndsWith("/GetLoggedInUserInfo") Then
                    SessionHelper.SetSessionAsSupervisor(oLoggedInPassport.ID, roAuth, roToken, oAuthMethod, oState)
                End If
            Else
                oApiSession.IdIdentity = -1
                oApiSession.Identity = Nothing
            End If

            oApiSession.Supervisor = Nothing
            oApiSession.IdSupervisor = -1

        End If
    End Sub

    Public Shared Function PrepareTimeGateSession(roAuth As String, oApiSession As roApiSession, ByVal isLogEnabled As Boolean, ByVal _updateSessionExculdeMethods As String()) As roApiSession
        Try
            If HttpContext.Current IsNot Nothing AndAlso HttpContext.Current.Request IsNot Nothing AndAlso oApiSession.CompanyId = RoAzureSupport.GetCompanyName().Trim.ToLower AndAlso oApiSession.CompanyId <> String.Empty Then

                Dim roToken As String = If(HttpContext.Current.Request.Headers("roToken") IsNot Nothing, HttpContext.Current.Request.Headers("roToken"), "")
                Dim roAlias As String = If(HttpContext.Current.Request.Headers("roAlias") IsNot Nothing, HttpContext.Current.Request.Headers("roAlias"), "")
                Dim accessFromApp As Boolean = If(HttpContext.Current.Request.Headers("roSrc") IsNot Nothing, roTypes.Any2Boolean(HttpContext.Current.Request.Headers("roSrc")), False)

                If HttpContext.Current.Request.Url.AbsolutePath.EndsWith("/Logout") Then
                    roToken = Robotics.Web.Base.CookieSession.GetAuthenticationCookie(StrConv(roAppType.VTPortal.ToString(), VbStrConv.ProperCase))
                End If

                Dim idOldIdentity As Integer = -1
                If oApiSession.IdIdentity > 0 Then
                    idOldIdentity = oApiSession.IdIdentity
                End If

                Dim oState As New roSecurityState()
                roBaseState.SetSessionSmall(oState, -1, roAppSource.TimeGate, "")
                Dim oTerminalState As New roTerminalState()
                roBaseState.SetSessionSmall(oTerminalState, -1, roAppSource.TimeGate, "")

                Dim timeGateTerminal As roTerminal = roTerminal.GetTerminalBySerialNumber(roAuth, oTerminalState)
                If timeGateTerminal IsNot Nothing Then
                    oApiSession.IdTerminal = timeGateTerminal.ID
                    oApiSession.Terminal = timeGateTerminal
                End If

                Dim timeZone As String = String.Empty
                Dim excludeUpdateSession As Boolean = False

                For Each oExcludedFunction In _updateSessionExculdeMethods
                    If HttpContext.Current.Request.Url.PathAndQuery.Contains(oExcludedFunction) Then excludeUpdateSession = True
                Next

                Dim oAuthMethod As AuthenticationMethod = AuthHelper.GetAuthMethodUsed(roAuth, roToken, oState)

                Dim oLoggedInPassport As roPassportTicket = AuthHelper.ValidateSecurityTokens(roAuth, roToken, timeZone, excludeUpdateSession, oAuthMethod, oState)
                Dim oImpersonatedPassport As roPassportTicket = AuthHelper.CheckAliasPassport(roAlias, oState)

                If oLoggedInPassport IsNot Nothing AndAlso oLoggedInPassport.ID <> idOldIdentity AndAlso idOldIdentity <> -1 Then
                    oApiSession = Robotics.Web.Base.ApiSession.GetNewApiSession(roAuth)
                End If

                oApiSession.LoginMethod = CInt(oAuthMethod)
                SetSessionIdentities(oApiSession, oLoggedInPassport, oImpersonatedPassport, roAuth, roToken, oAuthMethod, oState)

                If isLogEnabled Then
                    Dim strParameters As String = String.Empty
                    If HttpContext.Current.Request.Url.PathAndQuery.IndexOf("?") > 0 Then strParameters = HttpContext.Current.Request.Url.PathAndQuery.Substring(HttpContext.Current.Request.Url.PathAndQuery.IndexOf("?") + 1)

                    Dim strPath As String = String.Empty
                    If HttpContext.Current.Request.Url.AbsolutePath.LastIndexOf("/") > 0 Then strPath = HttpContext.Current.Request.Url.AbsolutePath.Substring(HttpContext.Current.Request.Url.AbsolutePath.LastIndexOf("/") + 1)

                    If oApiSession.IdIdentity > 0 Then
                        roLog.GetInstance.logMessage(roLog.EventType.roDebug, "User::" & oApiSession.Identity.ID & "(" & oApiSession.Identity.Name & ")::" & strPath & "::" & strParameters)
                    Else
                        roLog.GetInstance.logMessage(roLog.EventType.roDebug, "NotLoggedInUser::" & strPath & "::" & strParameters)
                    End If
                End If

                If oApiSession.IdIdentity > 0 Then
                    If Not oApiSession.SupervisorPortalEnabled.HasValue Then
                        Dim oSupPassport As roPassportTicket = Nothing
                        If oLoggedInPassport IsNot Nothing Then
                            oSupPassport = roPassportManager.GetPassportTicket(oLoggedInPassport.ID)
                            oApiSession.SupervisorPortalEnabled = oSupPassport.IsSupervisor
                        End If
                    End If

                    oApiSession.LoggedIn = True
                    oApiSession.LastRequest = DateTime.Now
                Else
                    oApiSession.LoggedIn = False
                    oApiSession.LastRequest = DateTime.Now
                End If

                If timeZone <> String.Empty Then
                    Dim oZoneInfo As TimeZoneInfo = roSupport.OlsonTimeZoneToTimeZoneInfo(timeZone)
                    If oZoneInfo Is Nothing Then
                        oZoneInfo = TimeZoneInfo.Local
                    End If

                    oApiSession.TimeZone = oZoneInfo
                Else
                    oApiSession.TimeZone = TimeZoneInfo.Local
                End If

                oApiSession.CurrentToken = roToken
                oApiSession.SecurityResult = oState.Result

            Else
                roLog.GetInstance().logMessage(roLog.EventType.roDebug, "GlobalAsax::PrepareTimeGateSession::Could not obtain TimeGate session info")
                oApiSession = Robotics.Web.Base.ApiSession.GetNewApiSession(roAuth)
            End If
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "GlobalAsax::Error::PrepareTimeGateSession::Could not obtain TimeGate session info", ex)
            oApiSession = Robotics.Web.Base.ApiSession.GetNewApiSession(roAuth)
        End Try

        Return oApiSession
    End Function

    Public Shared Function PrepareVisitsSession(roAuth As String, oApiSession As roApiSession, ByVal isLogEnabled As Boolean, ByVal _updateSessionExculdeMethods As String()) As roApiSession
        Try

            If HttpContext.Current IsNot Nothing AndAlso HttpContext.Current.Request IsNot Nothing AndAlso oApiSession.CompanyId = RoAzureSupport.GetCompanyName().Trim.ToLower Then
                Dim roToken As String = If(HttpContext.Current.Request.Headers("roToken") IsNot Nothing, HttpContext.Current.Request.Headers("roToken"), "")
                Dim accessFromApp As Boolean = If(HttpContext.Current.Request.Headers("roSrc") IsNot Nothing, roTypes.Any2Boolean(HttpContext.Current.Request.Headers("roSrc")), False)

                If HttpContext.Current.Request.Url.AbsolutePath.EndsWith("/AuthenticateSession") Then
                    roToken = Robotics.Web.Base.CookieSession.GetAuthenticationCookie(StrConv(roAppType.VTVisits.ToString(), VbStrConv.ProperCase))
                End If

                Dim oState As New roSecurityState()
                Dim timeZone As String = String.Empty

                Dim excludeUpdateSession As Boolean = False

                For Each oExcludedFunction In _updateSessionExculdeMethods
                    If HttpContext.Current.Request.Url.PathAndQuery.Contains(oExcludedFunction) Then excludeUpdateSession = True
                Next

                'Si tengo session, con el mismo id, token y no ha cambiado la impersonalización, no refresco la sesión
                'Se hará una vez cada 5 minutos aunque no haya tocado nada
                If oApiSession.Identity IsNot Nothing AndAlso oApiSession.SessionKey = roAuth AndAlso oApiSession.CurrentToken = roToken AndAlso
                     (roTypes.UnspecifiedNow - oApiSession.LastRequest).TotalSeconds < VALIDATE_SESSION Then
                    Return oApiSession
                End If


                Dim oSupervisorTicket As roPassportTicket = AuthHelper.ValidateSecurityTokens(roAuth, roToken, timeZone, excludeUpdateSession, AuthenticationMethod.Password, oState)

                If oSupervisorTicket IsNot Nothing Then
                    oApiSession.IdIdentity = oSupervisorTicket.ID
                    oApiSession.Identity = oSupervisorTicket
                Else
                    oApiSession.IdIdentity = -1
                    oApiSession.Identity = Nothing
                End If

                oApiSession.IdSupervisor = -1
                oApiSession.Supervisor = Nothing

                If oApiSession.Identity IsNot Nothing Then
                    'WLHelper.SetSessionAsSupervisor(oSupervisorTicket.ID, roAuth, roToken, WLHelper.APPLICATION_VTLIVE, oState)
                    oApiSession.LoggedIn = True
                    oApiSession.LastRequest = DateTime.Now

                    If Not oApiSession.SupervisorPortalEnabled.HasValue Then
                        If accessFromApp Then
                            oApiSession.SupervisorPortalEnabled = oApiSession.Identity.EnabledVTVisitsApp
                        Else
                            oApiSession.SupervisorPortalEnabled = oApiSession.Identity.EnabledVTVisits
                        End If

                        If oApiSession.SupervisorPortalEnabled Then
                            oApiSession.SupervisorPortalEnabled = oApiSession.Identity.IsSupervisor
                        End If
                    End If
                Else
                    oApiSession.LoggedIn = False
                    oApiSession.LastRequest = DateTime.Now
                End If

                oApiSession.CurrentToken = roToken
                oApiSession.TimeZone = TimeZoneInfo.Local
                oApiSession.SecurityResult = oState.Result
            Else
                roLog.GetInstance().logMessage(roLog.EventType.roDebug, "GlobalAsax::PrepareVisitsSession::Could not obtain Visits session info")
                oApiSession = Robotics.Web.Base.ApiSession.GetNewApiSession(roAuth)
            End If
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "GlobalAsax::Error::PrepareVisitsSession::Could not obtain Visits session info", ex)
            oApiSession = Robotics.Web.Base.ApiSession.GetNewApiSession(roAuth)
        End Try

        Return oApiSession
    End Function

    Public Shared Function PrepareTerminalSession(roAuth As String, oApiSession As roApiSession, ByVal _updateSessionExculdeMethods As String()) As roApiSession
        Try

            If HttpContext.Current IsNot Nothing AndAlso HttpContext.Current.Request IsNot Nothing AndAlso oApiSession.CompanyId = RoAzureSupport.GetCompanyName().Trim.ToLower Then
                Dim roToken As String = If(HttpContext.Current.Request.Headers("roToken") IsNot Nothing, HttpContext.Current.Request.Headers("roToken"), "")
                Dim roAlias As String = If(HttpContext.Current.Request.Headers("roAlias") IsNot Nothing, HttpContext.Current.Request.Headers("roAlias"), "")
                Dim accessFromApp As Boolean = If(HttpContext.Current.Request.Headers("roSrc") IsNot Nothing, roTypes.Any2Boolean(HttpContext.Current.Request.Headers("roSrc")), False)

                Dim oState As New Terminal.roTerminalState()
                Dim timeZone As String = String.Empty

                Dim excludeUpdateSession As Boolean = False

                For Each oExcludedFunction In _updateSessionExculdeMethods
                    If HttpContext.Current.Request.Url.PathAndQuery.Contains(oExcludedFunction) Then excludeUpdateSession = True
                Next

                Dim cTerminal As Terminal.roTerminal = Nothing

                Dim nTerminalTimezonInfo As TimeZoneInfo = TimeZoneInfo.Local
                Dim bTerminalRegistered As Boolean = False
                Dim strSecurityToken As String = String.Empty
                Dim bConfigReloaded As Boolean = False

                cTerminal = Terminal.roTerminal.ValidateTerminalSecurityConnection(roAuth, cTerminal, nTerminalTimezonInfo, bTerminalRegistered, strSecurityToken, oState, oApiSession.LastInfoLoaded, bConfigReloaded)

                If Now.Subtract(oApiSession.LastRequest).TotalSeconds > 60 AndAlso cTerminal IsNot Nothing Then
                    If cTerminal.LastStatus <> "Ok" Then
                        cTerminal.UpdateStatus(True)
                    End If
                End If

                If bConfigReloaded Then oApiSession.LastInfoLoaded = Now

                If cTerminal IsNot Nothing Then
                    oApiSession.IdTerminal = cTerminal.ID
                    oApiSession.Terminal = cTerminal
                Else
                    oApiSession.Terminal = Nothing
                    oApiSession.IdTerminal = -1
                End If
                oApiSession.TimeZone = nTerminalTimezonInfo
                oApiSession.IsRegistered = bTerminalRegistered
                oApiSession.TerminalSecurityToken = strSecurityToken
                oApiSession.TerminalResult = oState.Result

                If Robotics.VTBase.HashCheckSum.CalculateString(oApiSession.TerminalSecurityToken, Algorithm.MD5) = roToken Then
                    oApiSession.LoggedIn = True
                Else
                    oApiSession.LoggedIn = False
                End If
                oApiSession.LastRequest = DateTime.Now
            Else
                roLog.GetInstance().logMessage(roLog.EventType.roDebug, "GlobalAsax::PrepareTerminalSession::Could not obtain TerminalsPushServer session info")
                oApiSession = Robotics.Web.Base.ApiSession.GetNewApiSession(roAuth)
            End If
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "GlobalAsax::Error::PrepareTerminalSession::Could not obtain TerminalsPushServer session info", ex)
            oApiSession = Robotics.Web.Base.ApiSession.GetNewApiSession(roAuth)
        End Try

        Return oApiSession
    End Function

End Class