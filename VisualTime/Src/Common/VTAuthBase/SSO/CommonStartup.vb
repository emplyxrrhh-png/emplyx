Imports System.Net.Http
Imports System.Security.Claims
Imports System.Security.Cryptography
Imports System.ServiceModel.Configuration
Imports System.Text
Imports System.Web
Imports Microsoft.IdentityModel.Protocols.OpenIdConnect
Imports Microsoft.IdentityModel.Protocols.WsFederation
Imports Microsoft.IdentityModel.Tokens
Imports Microsoft.Owin
Imports Microsoft.Owin.Security
Imports Microsoft.Owin.Security.Cookies
Imports Microsoft.Owin.Security.Notifications
Imports Microsoft.Owin.Security.OpenIdConnect
Imports Microsoft.Owin.Security.WsFederation
Imports Newtonsoft.Json
Imports Owin
Imports Robotics.Azure
Imports Robotics.Base.DTOs
Imports Sustainsys.Saml2
Imports Sustainsys.Saml2.Configuration
Imports Sustainsys.Saml2.Metadata
Imports Sustainsys.Saml2.Owin

Public Class CommonStartup

    Public Shared Sub ConfigADFS(companyCode As String, app As IAppBuilder, ByVal metadata As String, ByVal realm As String, ByVal returnURI As String, ByVal iConfigVersion As Integer)
        If metadata = String.Empty OrElse realm = String.Empty Then Return

        If iConfigVersion = 2 Then
            app.Properties("Microsoft.Owin.Security.Constants.DefaultSignInAsAuthenticationType") = "clientscheme_" & companyCode.Trim.ToLower
            app.UseCookieAuthentication(New CookieAuthenticationOptions() With
                                            {
                                                .AuthenticationType = "clientscheme_" & companyCode.Trim.ToLower,
                                                .AuthenticationMode = Microsoft.Owin.Security.AuthenticationMode.Passive
                                            })
        End If

        app.UseWsFederationAuthentication(New WsFederationAuthenticationOptions With {
            .AuthenticationType = "clientscheme_" & companyCode.Trim.ToLower,
            .MetadataAddress = metadata,
            .Wtrealm = realm,
            .Wreply = returnURI,
            .Caption = companyCode,
            .BackchannelCertificateValidator = Nothing,
            .TokenValidationParameters = New TokenValidationParameters With {
                .ValidateIssuer = False
            },
            .Notifications = New WsFederationAuthenticationNotifications With {
                .AuthenticationFailed = Function(context As AuthenticationFailedNotification(Of WsFederationMessage, WsFederationAuthenticationOptions)) As Task
                                            context.HandleResponse()
                                            'context.Response.Redirect("/?errormessage=" + context.Exception.Message)
                                            Return Task.FromResult(0)
                                        End Function
            }
        })

    End Sub

    Public Shared Sub ConfigAAD(companyCode As String, app As IAppBuilder, ByVal clientId As String, ByVal tenantId As String, ByVal returnURI As String, ByVal iConfigVersion As Integer)
        If clientId = String.Empty OrElse tenantId = String.Empty Then Return

        If iConfigVersion = 2 Then
            app.Properties("Microsoft.Owin.Security.Constants.DefaultSignInAsAuthenticationType") = "clientscheme_" & companyCode.Trim.ToLower
            app.UseCookieAuthentication(New CookieAuthenticationOptions() With
                                            {
                                                .AuthenticationType = "clientscheme_" & companyCode.Trim.ToLower,
                                                .AuthenticationMode = Microsoft.Owin.Security.AuthenticationMode.Passive
                                            })
        End If

        app.UseOpenIdConnectAuthentication(
            New OpenIdConnectAuthenticationOptions With {
                .AuthenticationType = "clientscheme_" & companyCode.Trim.ToLower,
                .ClientId = clientId,
                .Authority = String.Format(System.Globalization.CultureInfo.InvariantCulture, "https://login.microsoftonline.com/{0}/v2.0", tenantId),
                .RedirectUri = returnURI,
                .PostLogoutRedirectUri = returnURI,
                .Scope = OpenIdConnectScope.OpenIdProfile,
                .Caption = companyCode,
                .ResponseType = OpenIdConnectResponseType.IdToken,
                .TokenValidationParameters = New TokenValidationParameters With {
                    .ValidateIssuer = False
                },
                .Notifications = New OpenIdConnectAuthenticationNotifications With {
                    .AuthenticationFailed = Function(context As AuthenticationFailedNotification(Of OpenIdConnectMessage, OpenIdConnectAuthenticationOptions)) As Task

                                                Robotics.VTBase.roLog.GetInstance().logMessage(VTBase.roLog.EventType.roDebug, "Error::SSOToken::Not validated due to::" & context.Exception.Message.ToString)

                                                context.HandleResponse()
                                                Return Task.FromResult(0)
                                            End Function
                }
            }
        )

    End Sub

    Public Shared Sub ConfigOKTA(companyCode As String, app As IAppBuilder, ByVal clientId As String, ByVal authority As String, ByVal secret As String, ByVal returnURI As String, ByVal iConfigVersion As Integer)
        If clientId = String.Empty OrElse authority = String.Empty OrElse secret = String.Empty Then Return

        If iConfigVersion = 2 Then
            app.Properties("Microsoft.Owin.Security.Constants.DefaultSignInAsAuthenticationType") = "clientscheme_" & companyCode.Trim.ToLower
            app.UseCookieAuthentication(New CookieAuthenticationOptions() With
                                            {
                                                .AuthenticationType = "clientscheme_" & companyCode.Trim.ToLower,
                                                .AuthenticationMode = Microsoft.Owin.Security.AuthenticationMode.Passive
                                            })
        End If

        app.UseOpenIdConnectAuthentication(
            New OpenIdConnectAuthenticationOptions With {
                .AuthenticationType = "clientscheme_" & companyCode.Trim.ToLower,
                .ClientId = clientId,
                .Authority = authority,
                .ClientSecret = secret,
                .RedirectUri = returnURI,
                .PostLogoutRedirectUri = returnURI,
                .Scope = OpenIdConnectScope.OpenIdProfile,
                .ResponseType = OpenIdConnectResponseType.IdToken,
                .Caption = companyCode,
                .TokenValidationParameters = New TokenValidationParameters With {
                    .ValidateIssuer = False
                },
                .Notifications = New OpenIdConnectAuthenticationNotifications With {
                    .AuthenticationFailed = Function(context As AuthenticationFailedNotification(Of OpenIdConnectMessage, OpenIdConnectAuthenticationOptions)) As Task
                                                context.HandleResponse()
                                                Return Task.FromResult(0)
                                            End Function,
                    .AuthorizationCodeReceived = Function(context As AuthorizationCodeReceivedNotification) As Task
                                                     context.HandleResponse()
                                                     Return Task.FromResult(0)
                                                 End Function
                }
            }
        )

    End Sub

    Public Shared Sub ConfigSAML(companyCode As String, app As IAppBuilder, ByVal sMetadataFile As String, ByVal sMetadataId As String, ByVal sSigningBehaviour As String, ByVal returnURI As String, ByVal iConfigVersion As Integer, ByVal sSourceApp As String)
        If sMetadataFile = String.Empty OrElse sMetadataId = String.Empty Then Return

        If iConfigVersion = 2 Then
            app.Properties("Microsoft.Owin.Security.Constants.DefaultSignInAsAuthenticationType") = "clientscheme_" & companyCode.Trim.ToLower
            app.UseCookieAuthentication(New CookieAuthenticationOptions() With
                                            {
                                                .AuthenticationType = "clientscheme_" & companyCode.Trim.ToLower,
                                                .AuthenticationMode = Microsoft.Owin.Security.AuthenticationMode.Passive
                                            })
        End If

        app.UseSaml2Authentication(CreateSaml2Options(companyCode, sMetadataFile, sMetadataId, sSigningBehaviour, returnURI, sSourceApp))

    End Sub

    Private Shared Function CreateSaml2Options(companyCode As String, ByVal sMetadataFile As String, ByVal sMetadataId As String, ByVal sSigningBehaviour As String, ByVal returnURI As String, ByVal sSourceApp As String) As Saml2AuthenticationOptions

        Dim spOptions = CreateSPOptions(returnURI, companyCode, sSourceApp, sSigningBehaviour)
        Dim Saml2Options = New Saml2AuthenticationOptions(False) With {
            .AuthenticationType = "clientscheme_" & companyCode,
            .SPOptions = spOptions,
            .Caption = companyCode
        }

        Dim idp As IdentityProvider = Nothing

        Try
            idp = New IdentityProvider(New EntityId(sMetadataId), spOptions) With {
                .AllowUnsolicitedAuthnResponse = True,
                .MetadataLocation = sMetadataFile,
                .LoadMetadata = True
            }
        Catch ex As Exception
            'Mantenemos esto por compatibilidad con versiones anteriores
            idp = New IdentityProvider(New EntityId(sMetadataId), spOptions) With {
                .LoadMetadata = True,
                .AllowUnsolicitedAuthnResponse = True,
                .MetadataLocation = sMetadataFile
            }
        End Try

        Saml2Options.IdentityProviders.Add(idp)
        Saml2Options.Notifications = New Saml2Notifications() With {
            .AcsCommandResultCreated = Sub(sso As WebSso.CommandResult, response As Saml2P.Saml2Response)
                                           If sso.Principal.Identity.GetType() = GetType(ClaimsIdentity) Then
                                               Dim identity As ClaimsIdentity = CType(sso.Principal.Identity, ClaimsIdentity)

                                               identity.AddClaim(New Claim("in_response_to", response.InResponseTo.Value))
                                           End If
                                       End Sub
            }
        Return Saml2Options
    End Function

    Private Shared Function CreateSPOptions(ByVal returnURI As String, ByVal companyCode As String, ByVal sSourceApp As String, ByVal sSigningBehaviour As String) As SPOptions
        Dim spOptions As SPOptions = Nothing

        Dim sEntityId As String = "VisualTime"

        If sSourceApp = "PortalAuth" Then
            sEntityId = "VisualTimePortal"
        End If

        'spOptions = New SPOptions With {
        '    .EntityId = New EntityId(sEntityId),
        '    .ReturnUrl = New Uri(returnURI)
        '}

        Dim eSigningBehaviour As SigningBehavior = SigningBehavior.IfIdpWantAuthnRequestsSigned

        If Not String.IsNullOrEmpty(sSigningBehaviour) Then eSigningBehaviour = System.Enum.Parse(GetType(SigningBehavior), sSigningBehaviour, True)

        spOptions = New SPOptions With {
            .EntityId = New EntityId(sEntityId),
            .ReturnUrl = New Uri(returnURI),
            .AuthenticateRequestSigningBehavior = eSigningBehaviour
        }

        Return spOptions
    End Function

End Class