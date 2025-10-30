Imports System.IO
Imports System.ServiceModel.Activation
Imports System.Web.Routing
Imports Robotics.Azure
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Security
Imports Robotics.Security.Base
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions
Imports Robotics.VTBase.roConstants
Imports SwaggerWcf
Imports VTLiveApi.DataServicesCORS

Public Class Global_asax
    Inherits roBaseGlobalAsax

    Private Shared _updateSessionExculdeMethods = {"GetDocumentationFaultAlerts", "RecoverPassword", "ResetPasswordToNew"}

#Region "Properties"

    Public Shared ReadOnly Property ApplicationSource As roAppSource
        Get
            Dim roAuth As String = If(HttpContext.Current.Request.Headers("roAuth") IsNot Nothing, HttpContext.Current.Request.Headers("roAuth"), "")

            Dim oApiSession As roApiSession = ObtainSession(roAuth)
            If oApiSession IsNot Nothing Then
                Return oApiSession.ApplicationSource
            End If

            Return Nothing
        End Get
    End Property

    Public Shared ReadOnly Property TerminalSecurityToken As String
        Get
            Dim roAuth As String = If(HttpContext.Current.Request.Headers("roAuth") IsNot Nothing, HttpContext.Current.Request.Headers("roAuth"), "")

            Dim oApiSession As roApiSession = ObtainSession(roAuth)
            If oApiSession IsNot Nothing Then
                Return oApiSession.TerminalSecurityToken
            End If

            Return Nothing
        End Get
    End Property

    Public Shared ReadOnly Property TerminalIdentity As Terminal.roTerminal
        Get
            Dim roAuth As String = If(HttpContext.Current.Request.Headers("roAuth") IsNot Nothing, HttpContext.Current.Request.Headers("roAuth"), "")

            Try
                Dim oApiSession As roApiSession = ObtainSession(roAuth)
                If oApiSession IsNot Nothing Then
                    If oApiSession.IdTerminal = -1 Then
                        Return Nothing
                    Else
                        Return CType(oApiSession.Terminal, Terminal.roTerminal)
                    End If
                End If
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "GlobalAsax::TerminalIdentity::Could not obtain TerminalIdentity", ex)
                Return Nothing
            End Try

            Return Nothing
        End Get
    End Property

    Public Shared ReadOnly Property Identity As roPassportTicket
        Get
            Dim roAuth As String = If(HttpContext.Current.Request.Headers("roAuth") IsNot Nothing, HttpContext.Current.Request.Headers("roAuth"), "")

            Try
                Dim oApiSession As roApiSession = ObtainSession(roAuth)
                If oApiSession IsNot Nothing Then
                    If oApiSession.IdIdentity = -1 Then
                        Return Nothing
                    Else
                        Return CType(oApiSession.Identity, roPassportTicket)
                    End If
                End If
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "GlobalAsax::Identity::Could not obtain Identity", ex)
                Return Nothing
            End Try

            Return Nothing
        End Get
    End Property

    Public Shared ReadOnly Property Supervisor As roPassportTicket
        Get
            Dim roAuth As String = If(HttpContext.Current.Request.Headers("roAuth") IsNot Nothing, HttpContext.Current.Request.Headers("roAuth"), "")

            Try
                Dim oApiSession As roApiSession = ObtainSession(roAuth)
                If oApiSession IsNot Nothing Then
                    If oApiSession.IdSupervisor = -1 Then
                        Return Nothing
                    Else
                        Return CType(oApiSession.Supervisor, roPassportTicket)
                    End If
                End If
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "GlobalAsax::Supervisor::Could not obtain Supervisor Identity", ex)
                Return Nothing
            End Try

            Return Nothing
        End Get
    End Property

    Public Shared ReadOnly Property IsRegistered As Boolean
        Get
            Dim roAuth As String = If(HttpContext.Current.Request.Headers("roAuth") IsNot Nothing, HttpContext.Current.Request.Headers("roAuth"), "")

            Dim oApiSession As roApiSession = ObtainSession(roAuth)
            If oApiSession IsNot Nothing Then
                Return oApiSession.IsRegistered
            End If

            Return False

        End Get
    End Property

    Public Shared ReadOnly Property LoggedIn As Boolean
        Get
            Dim roAuth As String = If(HttpContext.Current.Request.Headers("roAuth") IsNot Nothing, HttpContext.Current.Request.Headers("roAuth"), "")

            Dim oApiSession As roApiSession = ObtainSession(roAuth)
            If oApiSession IsNot Nothing Then
                Return oApiSession.LoggedIn AndAlso Global_asax.SecurityResult <> SecurityResultEnum.MaxCurrentSessionsExceeded
            End If

            Return False

        End Get
    End Property

    Public Shared ReadOnly Property IsSupervisor As Boolean
        Get
            Dim roAuth As String = If(HttpContext.Current.Request.Headers("roAuth") IsNot Nothing, HttpContext.Current.Request.Headers("roAuth"), "")

            Dim oApiSession As roApiSession = ObtainSession(roAuth)
            If oApiSession IsNot Nothing Then
                Return oApiSession.SupervisorPortalEnabled
            End If

            Return False

        End Get
    End Property

    Public Shared ReadOnly Property SecurityResult As SecurityResultEnum
        Get
            Dim roAuth As String = If(HttpContext.Current.Request.Headers("roAuth") IsNot Nothing, HttpContext.Current.Request.Headers("roAuth"), "")

            Dim oApiSession As roApiSession = ObtainSession(roAuth)
            If oApiSession IsNot Nothing Then
                Return oApiSession.SecurityResult
            End If

            Return SecurityResultEnum.NoError
        End Get
    End Property

    Public Shared ReadOnly Property TerminalResult As TerminalBaseResultEnum
        Get
            Dim roAuth As String = If(HttpContext.Current.Request.Headers("roAuth") IsNot Nothing, HttpContext.Current.Request.Headers("roAuth"), "")

            Dim oApiSession As roApiSession = ObtainSession(roAuth)
            If oApiSession IsNot Nothing Then
                Return oApiSession.TerminalResult
            End If

            Return TerminalBaseResultEnum.NoError
        End Get
    End Property

    Public Shared Property CurrentPunch As roTerminalPunch
        Get
            Dim roAuth As String = If(HttpContext.Current.Request.Headers("roAuth") IsNot Nothing, HttpContext.Current.Request.Headers("roAuth"), "")
            Dim oApiSession As roApiSession = ObtainSession(roAuth)
            If oApiSession IsNot Nothing Then
                Return oApiSession.CurrentPunch
            End If

            Return Nothing
        End Get
        Set(value As roTerminalPunch)
            Dim roAuth As String = If(HttpContext.Current.Request.Headers("roAuth") IsNot Nothing, HttpContext.Current.Request.Headers("roAuth"), "")

            Dim oApiSession As roApiSession = ObtainSession(roAuth)
            If oApiSession IsNot Nothing Then
                oApiSession.CurrentPunch = value
                SaveSessionData(oApiSession)
            End If

        End Set

    End Property

    Public Shared ReadOnly Property TimeZone As TimeZoneInfo
        Get
            Dim roAuth As String = If(HttpContext.Current.Request.Headers("roAuth") IsNot Nothing, HttpContext.Current.Request.Headers("roAuth"), "")

            Dim oApiSession As roApiSession = ObtainSession(roAuth)
            If oApiSession IsNot Nothing Then
                Return oApiSession.TimeZone
            End If

            Return TimeZoneInfo.Local
        End Get
    End Property

#End Region

    Private Shared Sub SaveSessionData(oApiSession As roApiSession)
        Robotics.DataLayer.roCacheManager.GetInstance.UpdateVTLiveApiSession(oApiSession)
    End Sub

    Private Shared Function ObtainSession(roAuth As String) As roApiSession
        Dim oApiSession As roApiSession = Robotics.DataLayer.roCacheManager.GetInstance.GetVTLiveApiSession(roAuth)
        If oApiSession Is Nothing Then oApiSession = GetNewApiSession(roAuth)

        Return oApiSession
    End Function

    Sub Application_Start(ByVal sender As Object, ByVal e As EventArgs)
        Robotics.DataLayer.AccessHelper.InitializeSharedInstanceData(roAppType.VTLiveApi, roLiveQueueTypes.vtliveapi)


        Dim fileName As String = Path.Combine(Hosting.HostingEnvironment.ApplicationPhysicalPath, "Resources/swagger-ui.zip")
        Dim memoryStream As New MemoryStream
        Using oFile As New FileStream(fileName, FileMode.Open, FileAccess.Read)
            oFile.CopyTo(memoryStream)
        End Using

        SwaggerWcfEndpoint.SetCustomZip(memoryStream)

        RouteTable.Routes.Add(New ServiceRoute("api-docs", New WebServiceHostFactory(), GetType(SwaggerWcfEndpoint)))
    End Sub

    Sub Application_BeginRequest(ByVal sender As Object, ByVal e As EventArgs)

        Try
            Me.OnApplicationBeginRequest()
        Catch ex As Robotics.DataLayer.ConnectionStringException
            roLog.GetInstance().logMessage(roLog.EventType.roInfo, "roliveApi::GlobalASAX::Application_BeginRequest::" & HttpContext.Current.Request.Url.AbsolutePath & "::" & ex.Message, ex)
            Return
        End Try

        If Request IsNot Nothing AndAlso Request.Path.Contains("api-docs") Then Return

        DataServicesCORS.CorsSupport.HandlePreflightRequest(HttpContext.Current)

        If HttpContext.Current.Request.Url.PathAndQuery.Contains("KeepAlive") Then
            Return
        End If

        Try
            Dim roAuth As String = If(HttpContext.Current.Request.Headers("roAuth") IsNot Nothing, HttpContext.Current.Request.Headers("roAuth"), "")
            Dim roApp As String = If(HttpContext.Current.Request.Headers("roApp") IsNot Nothing, HttpContext.Current.Request.Headers("roApp"), "")
            If roAuth = String.Empty Then Return

            Me.OnApplicationReloadSharedData()

            Dim oApiSession As roApiSession = ObtainSession(roAuth)
            Select Case roApp.ToUpper
                Case roAppSource.mx9.ToString.ToUpper
                    oApiSession.ApplicationSource = roAppSource.mx9
                    oApiSession = PrepareTerminalSession(roAuth, oApiSession, False) 'Me.LogEnabled
                Case Else
                    oApiSession.ApplicationSource = roAppSource.unknown
            End Select

            SaveSessionData(oApiSession)
        Catch ex As Exception
            roLog.GetInstance.logMessage(roLog.EventType.roInfo, "roliveApi::GlobalASAX::Application_BeginRequest::" & HttpContext.Current.Request.Url.AbsolutePath & "::" & ex.Message, ex)
        End Try

    End Sub

    Sub Application_PreSendRequestHeaders()
        Response.Headers.Remove("Server")
        Response.Headers.Remove("X-AspNet-Version")
        Response.Headers.Remove("X-AspNetMvc-Version")
    End Sub

    Sub Application_EndRequest(ByVal sender As Object, ByVal e As EventArgs)
        Me.OnApplicationEndRequest()
    End Sub

#Region "Base"
    Protected Overrides Function GetLoggedInPassportFromSession() As roPassportTicket
        Return Nothing
    End Function

    Protected Overrides Function GetLoggedInPassportIdFromSession() As Integer
        Return -1
    End Function
#End Region

#Region "Prepare session"

    Private Shared Function PrepareTerminalSession(roAuth As String, oApiSession As roApiSession, ByVal isLogEnabled As Boolean) As roApiSession
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

                If isLogEnabled Then
                    Dim strParameters As String = String.Empty
                    If HttpContext.Current.Request.Url.PathAndQuery.IndexOf("?") > 0 Then strParameters = HttpContext.Current.Request.Url.PathAndQuery.Substring(HttpContext.Current.Request.Url.PathAndQuery.IndexOf("?") + 1)

                    Dim strPath As String = String.Empty
                    If HttpContext.Current.Request.Url.AbsolutePath.LastIndexOf("/") > 0 Then strPath = HttpContext.Current.Request.Url.AbsolutePath.Substring(HttpContext.Current.Request.Url.AbsolutePath.LastIndexOf("/") + 1)

                    If oApiSession.IdTerminal > 0 Then
                        oApiSession.Terminal = TerminalIdentity
                        Robotics.Base.VTPortal.VTPortal.CommonHelper.LogMessage("Terminal::" & cTerminal.ID & "(" & cTerminal.Description & ")::" & strPath & "::" & strParameters)
                    Else
                        Robotics.Base.VTPortal.VTPortal.CommonHelper.LogMessage("NotLoggedInTerminal::" & strPath & "::" & strParameters)
                    End If
                End If

                Dim nTerminalTimezonInfo As TimeZoneInfo = TimeZoneInfo.Local
                Dim bTerminalRegistered As Boolean = False
                Dim strSecurityToken As String = String.Empty
                Dim bConfigReloaded As Boolean = False

                cTerminal = Terminal.roTerminal.ValidateTerminalSecurityConnection(roAuth, cTerminal, nTerminalTimezonInfo, bTerminalRegistered, strSecurityToken, oState, oApiSession.LastInfoLoaded, bConfigReloaded)

                If Now.Subtract(oApiSession.LastRequest).TotalSeconds > 60 AndAlso Not cTerminal Is Nothing Then
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
                roLog.GetInstance().logMessage(roLog.EventType.roError, "GlobalAsax::PrepareTerminalSession::Could not obtain TerminalsPushServer session info")
                oApiSession = Global_asax.GetNewApiSession(roAuth)
            End If
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "GlobalAsax::Error::PrepareTerminalSession::Could not obtain TerminalsPushServer session info", ex)
            oApiSession = Global_asax.GetNewApiSession(roAuth)
        End Try

        Return oApiSession
    End Function

    Private Shared Function GetNewApiSession(ByVal roAuth As String) As roApiSession
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
                    .TerminalResult = TerminalBaseResultEnum.NoError
                }

    End Function

#End Region

End Class