Imports Robotics.VTBase
Imports Robotics.Web.Base

Public Class CheckSSO_Login
    Inherits PageBase

    Public ReadOnly Property SSOTestUserName(Optional ByVal bolreload As Boolean = False) As String
        Get
            Dim oParamSSOType As String = API.CommonServiceMethods.GetAdvancedParameterLite("CHECKSSOType")

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

        Dim oParamSSOType As String = API.CommonServiceMethods.GetAdvancedParameterLite("CHECKSSOType")

        Dim bLoggedIn As Boolean
        Try
            Dim sCookieName As String = String.Empty

            sCookieName = "clientscheme_" & roTypes.Any2String(HttpContext.Current.Session("roMultiCompanyId")).Trim.ToLower
            sCookieName = "ExternalCookie"

            If Request.GetOwinContext().Authentication.AuthenticateAsync(sCookieName).Result IsNot Nothing Then
                bLoggedIn = Request.GetOwinContext().Authentication.AuthenticateAsync(sCookieName).Result.Identity.IsAuthenticated
            End If
        Catch ex As Exception
            bLoggedIn = False
        End Try

        If Not bLoggedIn Then
            CommonAuth.RedirectToLoginIfNecesary(HttpContext.Current.Session("roMultiCompanyId"), oParamSSOType, Me.Context.GetOwinContext(), Robotics.Web.Base.Configuration.LiveDesktopAppUrl & "VTCheckSSO/Default.aspx?referer=" & HttpContext.Current.Session("roMultiCompanyId"))
        Else
            Me.lblResult.Text = "Usuario detectado:" & Me.SSOTestUserName
        End If

    End Sub

End Class