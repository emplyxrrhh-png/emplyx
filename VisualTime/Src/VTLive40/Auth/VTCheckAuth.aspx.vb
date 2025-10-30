Imports Robotics.VTBase
Imports Robotics.Web.Base

Public Class VTCheckAuth
    Inherits PageBase

    Public ReadOnly Property SSOTestUserName(Optional ByVal bolreload As Boolean = False) As String
        Get
            Dim oParamSSOType As String = API.CommonServiceMethods.GetAdvancedParameterLite("CHECKSSOType")

            Return CommonClaim.GetAuthenticationClaim(oParamSSOType, SSOConfigVersion, Request.GetOwinContext())
        End Get
    End Property

    Public ReadOnly Property SSOConfigVersion(Optional ByVal bolreload As Boolean = False) As Integer
        Get
            Return roTypes.Any2Integer(HelperSession.AdvancedParametersCache("VisualTime.SSO.ConfigurationVersion"))
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

        Dim oParamSSOType As String = API.CommonServiceMethods.GetAdvancedParameterLite("CHECKSSOType")

        Dim bLoggedIn As Boolean
        Try
            Dim sCookieName As String = "ExternalCookie"
            If SSOConfigVersion = 2 Then
                sCookieName = "clientscheme_" & HttpContext.Current.Session("roMultiCompanyId")
            End If

            If Request.GetOwinContext().Authentication.AuthenticateAsync(sCookieName).Result IsNot Nothing Then
                bLoggedIn = Request.GetOwinContext().Authentication.AuthenticateAsync(sCookieName).Result.Identity.IsAuthenticated
            End If
        Catch ex As Exception
            bLoggedIn = False
        End Try

        If Not bLoggedIn Then

            CommonAuth.RedirectToLoginIfNecesary(HttpContext.Current.Session("roMultiCompanyId"), oParamSSOType, Me.Context.GetOwinContext(), Robotics.Web.Base.Configuration.LiveDesktopAppUrl & "AuthCheck/" & HttpContext.Current.Session("roMultiCompanyId").ToString().ToLower.Trim)
        Else
            Me.lblResult.Text = "Usuario detectado:" & Me.SSOTestUserName
        End If

    End Sub

End Class