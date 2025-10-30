Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Security
Imports Robotics.VTBase

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

    Public Shared ReadOnly Property TimeGateSerial As String
        Get
            Dim roAuth As String = If(HttpContext.Current.Request.Headers("roAuth") IsNot Nothing, HttpContext.Current.Request.Headers("roAuth"), "")

            Dim oApiSession As roApiSession = ObtainSession(roAuth)
            If oApiSession IsNot Nothing Then
                Return oApiSession.SessionKey
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

    Public Shared ReadOnly Property AuthenticationUsed As Integer
        Get
            Dim roAuth As String = If(HttpContext.Current.Request.Headers("roAuth") IsNot Nothing, HttpContext.Current.Request.Headers("roAuth"), "")

            Dim oApiSession As roApiSession = ObtainSession(roAuth)
            If oApiSession IsNot Nothing Then
                Return oApiSession.LoginMethod
            End If

            Return 1
        End Get
    End Property

#End Region

#Region "Base"
    Protected Overrides Function GetLoggedInPassportFromSession() As roPassportTicket
        Dim roAuth As String = If(HttpContext.Current.Request.Headers("roAuth") IsNot Nothing, HttpContext.Current.Request.Headers("roAuth"), "")
        Dim roApp As String = If(HttpContext.Current.Request.Headers("roApp") IsNot Nothing, HttpContext.Current.Request.Headers("roApp"), "")
        If roAuth = String.Empty Then Return Nothing

        Dim oApiSession As roApiSession = ObtainSession(roAuth)

        If oApiSession.Supervisor IsNot Nothing AndAlso oApiSession.Supervisor.Id > 0 Then
            Return CType(oApiSession.Supervisor, roPassportTicket)
        Else
            If oApiSession.Identity IsNot Nothing AndAlso oApiSession.Identity.Id > 0 Then
                Return CType(oApiSession.Identity, roPassportTicket)
            End If
        End If

        Return Nothing
    End Function

    Protected Overrides Function GetLoggedInPassportIdFromSession() As Integer
        Dim roAuth As String = If(HttpContext.Current.Request.Headers("roAuth") IsNot Nothing, HttpContext.Current.Request.Headers("roAuth"), "")
        Dim roApp As String = If(HttpContext.Current.Request.Headers("roApp") IsNot Nothing, HttpContext.Current.Request.Headers("roApp"), "")
        If roAuth = String.Empty Then Return -1

        Dim oApiSession As roApiSession = ObtainSession(roAuth)

        If oApiSession.Supervisor IsNot Nothing AndAlso oApiSession.Supervisor.Id > 0 Then
            Return CType(oApiSession.Supervisor, roPassportTicket).ID
        Else
            If oApiSession.Identity IsNot Nothing AndAlso oApiSession.Identity.Id > 0 Then
                Return CType(oApiSession.Identity, roPassportTicket).ID
            End If
        End If

        Return -1
    End Function
#End Region

    Sub Application_PreSendRequestHeaders()
        Response.Headers.Remove("Server")
        Response.Headers.Remove("X-AspNet-Version")
        Response.Headers.Remove("X-AspNetMvc-Version")
    End Sub

    Sub Application_Start(ByVal sender As Object, ByVal e As EventArgs)
        Robotics.DataLayer.AccessHelper.InitializeSharedInstanceData(roAppType.VTPortal, roLiveQueueTypes.vtportal)
    End Sub

    Sub Application_BeginRequest(ByVal sender As Object, ByVal e As EventArgs)
        Robotics.Web.Base.CorsSupport.HandlePreflightRequest(HttpContext.Current)

        Try
            Dim roAuth As String = If(HttpContext.Current.Request.Headers("roAuth") IsNot Nothing, HttpContext.Current.Request.Headers("roAuth"), "")
            Dim roApp As String = If(HttpContext.Current.Request.Headers("roApp") IsNot Nothing, HttpContext.Current.Request.Headers("roApp"), "")
            If roAuth = String.Empty Then Return

            Me.OnApplicationReloadSharedData()

            Dim oApiSession As roApiSession = ObtainSession(roAuth)
            Select Case roApp.ToUpper
                Case roAppSource.VTPortal.ToString.ToUpper
                    oApiSession.ApplicationSource = roAppSource.VTPortal
                    oApiSession = Robotics.Web.Base.ApiSession.PrepareVTPortalSession(roAuth, oApiSession, False, _updateSessionExculdeMethods) 'Me.LogEnabled
                Case roAppSource.TimeGate.ToString.ToUpper
                    oApiSession.ApplicationSource = roAppSource.TimeGate
                    oApiSession = Robotics.Web.Base.ApiSession.PrepareTimeGateSession(roAuth, oApiSession, False, _updateSessionExculdeMethods) 'Me.LogEnabled
                Case Else
                    oApiSession.ApplicationSource = roAppSource.VTLive
                    oApiSession = Robotics.Web.Base.ApiSession.GetNewApiSession(roAuth)
            End Select

            SaveSessionData(oApiSession)
        Catch ex As Exception
            roLog.GetInstance.logMessage(roLog.EventType.roError, "roliveApi::GlobalASAX::Application_BeginRequest::" & HttpContext.Current.Request.Url.AbsolutePath & "::" & ex.Message, ex)
        End Try

    End Sub

    Sub Application_EndRequest(ByVal sender As Object, ByVal e As EventArgs)
        Me.OnApplicationEndRequest()
    End Sub

#Region "Session management"
    Public Shared Sub Logout()
        Dim roAuth As String = If(HttpContext.Current.Request.Headers("roAuth") IsNot Nothing, HttpContext.Current.Request.Headers("roAuth"), "")
        Robotics.DataLayer.roCacheManager.GetInstance.DeleteVTLiveApiSession(roAuth)
    End Sub

    Private Shared Sub SaveSessionData(oApiSession As roApiSession)
        Robotics.DataLayer.roCacheManager.GetInstance.UpdateVTLiveApiSession(oApiSession)
    End Sub

    Private Shared Function ObtainSession(roAuth As String) As roApiSession
        Dim oApiSession As roApiSession = Robotics.DataLayer.roCacheManager.GetInstance.GetVTLiveApiSession(roAuth)
        If oApiSession Is Nothing Then oApiSession = Robotics.Web.Base.ApiSession.GetNewApiSession(roAuth)

        Return oApiSession
    End Function
#End Region




End Class