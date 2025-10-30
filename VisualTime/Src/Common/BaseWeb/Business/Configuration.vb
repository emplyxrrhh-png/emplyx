Imports System.Configuration

''' <summary>
''' Exposes configuration values defined in web.config.
''' </summary>
Public NotInheritable Class Configuration

    Private Sub New()
    End Sub

    Public Shared ReadOnly ApplicationName As String = ConfigurationManager.AppSettings("ApplicationName")

    Public Shared ReadOnly LoginCookieName As String = "LoginSessionID_" & ApplicationName
    Public Shared ReadOnly DiagnosticsServiceUrl As String = ConfigurationManager.AppSettings("LiveDiagnosticsServiceUrl")
    Public Shared ReadOnly MTLiveApiUrl As String = ConfigurationManager.AppSettings("MTLiveApiUrl")

    Public Shared ReadOnly RootUrl As String = ConfigurationManager.AppSettings("RootUrl")

    Public Shared ReadOnly Property SSOCheckPageType As String
        Get
            Try
                Dim strUrl As String = API.CommonServiceMethods.GetAdvancedParameterLite("VisualTime.SSO.CheckPageType")

                Return strUrl
            Catch ex As Exception
                Return String.Empty
            End Try

        End Get
    End Property

    Public Shared ReadOnly Property VTLiveMixedAuthEnabled As Boolean
        Get
            Try
                Dim strUrl As Boolean = VTBase.roTypes.Any2Boolean(API.CommonServiceMethods.GetAdvancedParameterLite("VisualTime.SSO.VTLiveMixedAuthEnabled"))

                Return strUrl
            Catch ex As Exception
                Return True
            End Try
        End Get
    End Property

    Public Shared ReadOnly Property VTPortalMixedAuthEnabled As Boolean
        Get
            Try
                Dim strUrl As Boolean = VTBase.roTypes.Any2Boolean(API.CommonServiceMethods.GetAdvancedParameterLite("VisualTime.SSO.VTPortalMixedAuthEnabled"))

                Return strUrl
            Catch ex As Exception
                Return True
            End Try
        End Get
    End Property

    Public Shared ReadOnly Property LivePortalAppUrl As String
        Get
            Try
                Dim strUrl As String = API.CommonServiceMethods.GetAdvancedParameterLite("VisualTime.SSO.LivePortalAppUrl")
                If strUrl = String.Empty Then strUrl = ConfigurationManager.AppSettings("LivePortalAppUrl")
                If strUrl = Nothing Then strUrl = String.Empty

                Return strUrl
            Catch ex As Exception
                Return String.Empty
            End Try
        End Get
    End Property

    Public Shared ReadOnly Property SupervisorPortalAppUrl As String
        Get
            Try
                Dim strUrl As String = API.CommonServiceMethods.GetAdvancedParameterLite("VisualTime.SSO.SupervisorPortalAppUrl")
                If strUrl = String.Empty Then strUrl = ConfigurationManager.AppSettings("SupervisorPortalAppUrl")
                If strUrl = Nothing Then strUrl = String.Empty

                Return strUrl
            Catch ex As Exception
                Return String.Empty
            End Try

        End Get
    End Property

    Public Shared ReadOnly Property LiveDesktopAppUrl As String
        Get
            Try
                Dim strUrl As String = API.CommonServiceMethods.GetAdvancedParameterLite("VisualTime.SSO.LiveDesktopAppUrl")
                If strUrl = String.Empty Then strUrl = ConfigurationManager.AppSettings("LiveDesktopAppUrl")
                If strUrl = Nothing Then strUrl = String.Empty

                Return strUrl
            Catch ex As Exception
                Return String.Empty
            End Try

        End Get
    End Property

    Public Shared ReadOnly Property VTPortalAppUrl As String
        Get
            Try
                Dim strUrl As String = API.CommonServiceMethods.GetAdvancedParameterLite("VisualTime.SSO.VTPortalAppUrl")
                If strUrl = String.Empty Then strUrl = ConfigurationManager.AppSettings("VTPortalAppUrl")
                If strUrl = Nothing Then strUrl = String.Empty

                Return strUrl
            Catch ex As Exception
                Return String.Empty
            End Try
        End Get
    End Property

    Public Shared ReadOnly Property VTLiveApiUrl As String
        Get
            Try
                Dim strUrl As String = API.CommonServiceMethods.GetAdvancedParameterLite("VisualTime.SSO.VTLiveApiUrl")
                If strUrl = String.Empty Then strUrl = ConfigurationManager.AppSettings("VTLiveApiUrl")
                If strUrl = Nothing Then strUrl = String.Empty

                Return strUrl
            Catch ex As Exception
                Return String.Empty
            End Try

        End Get
    End Property

End Class