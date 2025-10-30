Imports System.Threading.Tasks
Imports System.Web.Mvc
Imports DevExpress.XtraRichEdit.Import.Doc
Imports Duende.IdentityModel
Imports Duende.IdentityModel.OidcClient
Imports Robotics.Azure
Imports Robotics.Base.DTOs
Imports Robotics.VTBase
Imports Robotics.Web.Base
Imports VTLive40.Controllers.Base

Public Class SSOController
    Inherits BaseController

    Friend Const cLoginRoute As String = "sso/cegidIDLogin"
    Friend Const cContinueLoginRoute As String = "sso/cegidID"
    Friend Const cLogoutRoute As String = "sso/sign-out"
    Friend Const cCommitLogin As String = "Auth/"

    Private requieredLabels = {"Title", "SaveChanges", "UndoChanges", "enableSSO", "enableVTLiveMixAuth", "enableVTPortalMixAuth", "validationDone", "saveValidation", "validateSSO", "headerTitle",
        "adfsInformation", "aadInformation", "oktaInformation", "adfsmetadataURL", "adfsRealm", "aadClient", "aadTenant", "oktaClient", "oktaSecret", "oktaAuthority", "samlInformation", "metadataURL",
        "ipID", "signingBehaviour", "never", "idp", "always", "cegidInformation"}

    <LoggedInAtrribute(Requiered:=True)>
    <PermissionsAtrribute(FeatureAlias:="Administration.SecurityOptions", Permission:=Robotics.Base.DTOs.Permission.Write)>
    Function Index() As ActionResult

        Me.InitializeBase(CardTreeTypes.SSO, TabBarButtonTypes.SSO, "SSO", requieredLabels, "LiveSecurity") _
                          .SetBarButton(BarButtonTypes.SSO) _
                          .SetViewInfo("LiveSecurity", "SSO", "Title", "Title", "Base/Images/SSO/SSOConfig80.png", "TitleDesc")

        ViewBag.checkURL = "https://" & Request.Url.Authority & "/Auth/" & roTypes.Any2String(roConstants.GetGlobalEnvironmentParameter(GlobalAsaxParameter.CompanyId)) & "/"
        Return View("index")
    End Function

    <HttpPost>
    <LoggedInAtrribute(Requiered:=True)>
    <PermissionsAtrribute(FeatureAlias:="Administration.SecurityOptions", Permission:=Robotics.Base.DTOs.Permission.Write)>
    Function GetSSOConfiguration() As JsonResult
        Return Json(API.SecurityServiceMethods.GetSSOConfiguration(Nothing, True))
    End Function

    <HttpPost>
    <LoggedInAtrribute(Requiered:=True)>
    <PermissionsAtrribute(FeatureAlias:="Administration.SecurityOptions", Permission:=Robotics.Base.DTOs.Permission.Write)>
    Function SaveSSOConfiguration(ByVal oConf As roSSOConfiguration) As JsonResult
        Return Json(API.SecurityServiceMethods.SaveSSOConfiguration(Nothing, oConf, Request.Url.Authority))
    End Function


    <HttpGet()>
    Public Async Function cegidIDLogin(id As String) As Task(Of ActionResult)

        Dim client As OidcClient = GetOidcClient()
        Dim additionalParameters As New Dictionary(Of String, String)()

        If id = "reset" Then
            additionalParameters.Add("prompt", OidcConstants.PromptModes.SelectAccount)
        End If

        Try
            Dim state = Await client.PrepareLoginAsync(New Duende.IdentityModel.Client.Parameters(additionalParameters))

            Web.HttpContext.Current.Cache.Add($"{state.State}_OB", state, Nothing, DateTime.Now.AddMinutes(5), TimeSpan.Zero, System.Web.Caching.CacheItemPriority.Normal, Nothing)
            Web.HttpContext.Current.Cache.Add($"{state.State}_CI", id, Nothing, DateTime.Now.AddMinutes(5), TimeSpan.Zero, System.Web.Caching.CacheItemPriority.Normal, Nothing)
            Web.HttpContext.Current.Cache.Add($"{state.State}_QS", Web.HttpContext.Current.Request.QueryString.ToString(), Nothing, DateTime.Now.AddMinutes(5), TimeSpan.Zero, System.Web.Caching.CacheItemPriority.Normal, Nothing)

            Dim url = New Uri(state.StartUrl)
            Dim queryParts = HttpUtility.ParseQueryString(url.Query)

            Dim redirectUrl As String = New UriBuilder(url) With {.Query = queryParts.ToString()}.Uri.ToString

            Return Redirect(redirectUrl)
        Catch e As Exception
            Dim strNoWeb = Me.Request.Url.ToString()
            strNoWeb = strNoWeb.Substring(0, strNoWeb.IndexOf(cLoginRoute)) & "?noWeb=1"
            Return Redirect(strNoWeb)
        End Try
    End Function

    <HttpGet()>
    Public Async Function cegidID() As Task(Of ActionResult)
        If Web.HttpContext.Current.Request.QueryString("skipped") IsNot Nothing Then
            If Web.HttpContext.Current.Request.QueryString("skipped").ToString() = "1" Then
                Return Redirect(GetRedirectUri(cContinueLoginRoute) & $"?Skipped=1")
            End If
        End If

        Dim client As OidcClient = GetOidcClient()
        Dim stateID As String = Web.HttpContext.Current.Request.QueryString("state")

        Dim state = TryCast(Web.HttpContext.Current.Cache.Get($"{stateID}_OB"), AuthorizeState)
        Dim companyId = TryCast(Web.HttpContext.Current.Cache.Get($"{stateID}_CI"), String)
        Dim queryString = TryCast(Web.HttpContext.Current.Cache.Get($"{stateID}_QS"), String)

        'restore original state
        Dim url = New Uri(state.StartUrl)
        Dim queryParts = HttpUtility.ParseQueryString(url.Query)
        queryParts("state") = state.State
        queryParts("code") = Web.HttpContext.Current.Request.QueryString("code")

        Dim result = Await client.ProcessResponseAsync(queryParts.ToString(), state)

        If result?.IsError Then
            Web.HttpContext.Current.Session("TokenManager.TokenType.Access") = ""
            Web.HttpContext.Current.Session("TokenManager.TokenType.Refresh") = ""
            Web.HttpContext.Current.Session("TokenManager.TokenType.Id") = ""
            Web.HttpContext.Current.Session("TokenManager.TokenType.UserInfo") = Nothing
            Return Redirect(GetRedirectUri(cContinueLoginRoute) & $"?Error={result.Error}")
        Else
            Web.HttpContext.Current.Session("TokenManager.TokenType.Access") = result.AccessToken
            Web.HttpContext.Current.Session("TokenManager.TokenType.Refresh") = result.RefreshToken
            Web.HttpContext.Current.Session("TokenManager.TokenType.Id") = result.IdentityToken
            Web.HttpContext.Current.Session("TokenManager.TokenType.UserInfo") = result.User
            Return Redirect(GetRedirectUri(cContinueLoginRoute) & $"{cCommitLogin}{companyId}?{queryString}")
        End If
    End Function

    <Route(cLogoutRoute)>
    <HttpGet()>
    Public Async Function Logout() As Task(Of Http.IHttpActionResult)
        Dim it As String = CType(Web.HttpContext.Current.Session("TokenManager.TokenType.Id"), String)

        If it IsNot Nothing Then
            Dim client As OidcClient = GetOidcClient()

            Await client.LogoutAsync(New LogoutRequest() With {.IdTokenHint = it})
            Web.HttpContext.Current.Session("TokenManager.TokenType.Access") = Nothing
            Web.HttpContext.Current.Session("TokenManager.TokenType.Refresh") = Nothing
            Web.HttpContext.Current.Session("TokenManager.TokenType.Id") = Nothing
        End If
        Return Redirect(GetRedirectUri(cLogoutRoute))
    End Function

    Private Function GetOidcClient() As OidcClient

        Dim oConfigValue As roAzureConfig = New roConfigRepository().GetConfigParameter(roConfigParameter.cegidid)
        Dim oCegidIDConf As roCegidIDConfig = Robotics.VTBase.roJSONHelper.DeserializeNewtonSoft(oConfigValue.value, GetType(roCegidIDConfig))


        Dim clientId = oCegidIDConf.clientid
        Dim clientSecret = oCegidIDConf.secret

        Dim options = New OidcClientOptions With {
            .Authority = $"{oCegidIDConf.authority}",
            .ClientId = clientId,
            .ClientSecret = clientSecret,
            .Scope = "openid profile email offline_access",
            .RedirectUri = $"{oCegidIDConf.returnuri}"
        }

        Dim client = New OidcClient(options)
        Return client
    End Function

    Private Function GetRedirectUri(prefix As String) As String
        Dim request = Web.HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Path)
        Return request.Substring(0, request.Length - prefix.Length)
    End Function
End Class