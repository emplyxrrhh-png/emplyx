Imports System.Net
Imports System.Net.Security
Imports System.Security.Cryptography.X509Certificates
Imports Robotics.Base.DTOs
Imports Robotics.Security.Base
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions
Imports Robotics.Web.Base

Public Class VTPortalLogin
    Inherits Page

    <Runtime.Serialization.DataContract()>
    Private Class AdfsUser

        <Runtime.Serialization.DataMember(Name:="UserName")>
        Public UserName As String

        <Runtime.Serialization.DataMember(Name:="Token")>
        Public Token As String

    End Class

    Public ReadOnly Property ADFSUserName(Optional ByVal bolreload As Boolean = False) As String
        Get
            Dim oParamSSOType As String = HelperSession.AdvancedParametersCache("SSOType")

            Return CommonClaim.GetAuthenticationClaim(oParamSSOType, 1, Request.GetOwinContext())
        End Get
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Dim isApp As String = Request.Params("isApp")
        If AuthValidations.IsAlreadyLoggedOnAnotherCompany() Then
            Me.hdnAdfsUserName.Value = "1#not recognized#1"
            Me.hdnAdfsToken.Value = "1#not recognized#1"

            If isApp IsNot Nothing AndAlso roTypes.Any2Boolean(isApp) = False Then
                Robotics.Web.Base.HelperWeb.EraseCookie("VTPortalAdfsUserName")
                Robotics.Web.Base.HelperWeb.CreateCookie("VTPortalAdfsUserName", Me.hdnAdfsUserName.Value, False)

                Robotics.Web.Base.HelperWeb.EraseCookie("VTPortalToken")
                Robotics.Web.Base.HelperWeb.CreateCookie("VTPortalToken", Me.hdnAdfsToken.Value, False)
                Response.Redirect(Configuration.VTPortalAppUrl & "/index.aspx")
            End If


            Return
        End If


        If roTypes.Any2String(Request.Params("referer")) <> String.Empty Then
            HttpContext.Current.Session("roMultiCompanyId") = roTypes.Any2String(Request.Params("referer"))
        End If

        Dim oParamSSOType As String = HelperSession.AdvancedParametersCache("SSOType")

        If WLHelperWeb.ADFSEnabled AndAlso oParamSSOType <> String.Empty Then

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
                Dim urlParams As String = String.Empty
                If isApp IsNot Nothing Then
                    urlParams = IIf(roTypes.Any2Boolean(isApp), "?isApp=1&referer=" & HttpContext.Current.Session("roMultiCompanyId"), "?isApp=0&referer=" & HttpContext.Current.Session("roMultiCompanyId"))
                End If

                Me.hdnAdfsUserName.Value = String.Empty
                Me.hdnAdfsToken.Value = String.Empty

                CommonAuth.RedirectToLoginIfNecesary(HttpContext.Current.Session("roMultiCompanyId"), oParamSSOType, Me.Context.GetOwinContext(), Robotics.Web.Base.Configuration.LiveDesktopAppUrl & "VTLogin/VTPortalLogin.aspx" & urlParams)
            Else
                If Not Me.IsPostBack Then

                    Dim oUser As New AdfsUser() With {.UserName = "", .Token = ""}

                    If ADFSUserName <> String.Empty Then
                        Try
                            Dim ostate As New roSecurityState(-2)
                            Dim oToken As String = String.Empty
                            Dim nGUID As String = Guid.NewGuid.ToString()
                            Dim Pass As roPassportTicket = Nothing

                            Pass = roPassportManager.ValidateCredentials(AuthenticationMethod.Password, "\" & ADFSUserName, "\" & ADFSUserName.ToLower, True, "", True, ostate)

                            oUser.UserName = ADFSUserName

                            If Pass IsNot Nothing Then
                                WebServiceHelper.SetSSOVTPortalState(ostate, Pass.ID)
                                Pass = AuthHelper.Authenticate(Pass, AuthenticationMethod.Password, "\" & ADFSUserName, "\" & ADFSUserName.ToLower, True, ostate, roTypes.Any2Boolean(isApp), "", "", "", True, nGUID, oToken, False)

                                If Pass IsNot Nothing AndAlso ostate.Result = SecurityResultEnum.NoError AndAlso Not String.IsNullOrEmpty(oToken) Then
                                    Dim strSecCode As String = nGUID & "#" & oToken & "#" & HttpContext.Current.Session("roMultiCompanyId")

                                    oUser.Token = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(strSecCode))
                                    oUser.UserName = ADFSUserName
                                Else
                                    oUser.UserName = ADFSUserName
                                    oUser.Token = String.Empty
                                End If
                            Else
                                oUser.UserName = ADFSUserName
                                oUser.Token = String.Empty
                                roLog.GetInstance().logMessage(roLog.EventType.roDebug, "VTLogin::VTPortalLogin::Could not validate credentials")
                            End If
                        Catch ex As Exception
                            oUser.UserName = ADFSUserName
                            oUser.Token = String.Empty
                            roLog.GetInstance().logMessage(roLog.EventType.roError, "VTLogin::VTPortalLogin::Error requesting portal credentials::" & ex.Message)
                        End Try
                    Else
                        oUser.UserName = String.Empty
                        oUser.Token = String.Empty
                    End If

                    Me.hdnAdfsUserName.Value = oUser.UserName
                    Me.hdnAdfsToken.Value = oUser.Token

                End If

                If isApp IsNot Nothing AndAlso roTypes.Any2Boolean(isApp) = False Then
                    Response.Redirect(Configuration.VTPortalAppUrl & "/index.aspx?token=" & Server.UrlEncode(Me.hdnAdfsToken.Value) & "&userName=" & Server.UrlEncode(ADFSUserName))
                End If
            End If
        Else
            Me.hdnAdfsUserName.Value = "1#not recognized#1"
            Me.hdnAdfsToken.Value = "1#not recognized#1"

            If isApp IsNot Nothing AndAlso roTypes.Any2Boolean(isApp) = False Then
                Robotics.Web.Base.HelperWeb.EraseCookie("VTPortalAdfsUserName")
                Robotics.Web.Base.HelperWeb.CreateCookie("VTPortalAdfsUserName", Me.hdnAdfsUserName.Value, False)

                Robotics.Web.Base.HelperWeb.EraseCookie("VTPortalToken")
                Robotics.Web.Base.HelperWeb.CreateCookie("VTPortalToken", Me.hdnAdfsToken.Value, False)
                Response.Redirect(Configuration.VTPortalAppUrl & "/index.aspx")
            End If
        End If

    End Sub

    Private Function MyCertHandler(sender As Object, certificate As X509Certificate, chain As X509Chain, sslPolicyErrors As SslPolicyErrors) As Boolean
        Return True
    End Function

End Class