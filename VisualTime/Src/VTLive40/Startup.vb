Imports Microsoft.Owin
Imports Microsoft.Owin.Security.Cookies
Imports Owin
Imports Robotics.Web.Base

<Assembly: OwinStartup(GetType(ADFS_Login.Startup))>

Namespace ADFS_Login

    Public Class Startup

        Public Sub Configuration(app As IAppBuilder)
            'RequireAspNetSession(app)
            Robotics.VTBase.roLog.GetInstance().logMessage(Robotics.VTBase.roLog.EventType.roInfo, "OWINStartup::MT::Start loading SSO configuration")

            'Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = True

            LoadCustomSSOConfiguration(app)

            Robotics.VTBase.roLog.GetInstance().logMessage(Robotics.VTBase.roLog.EventType.roInfo, "OWINStartup::MT::End loading SSO configuration")
        End Sub

        Private Sub LoadCustomSSOConfiguration(app As IAppBuilder)
            Dim oCompanies As Robotics.Base.DTOs.roCompanyConfiguration() = Robotics.DataLayer.roCacheManager.GetInstance().GetOwinCompanies()

            For Each oCompany As Robotics.Base.DTOs.roCompanyConfiguration In oCompanies

                If String.IsNullOrEmpty(oCompany.dbconnectionstring) Then Continue For

                Dim strCompanyId As String = oCompany.companyname
                Try
                    If Robotics.VTBase.roTypes.Any2Boolean(API.CommonServiceMethods.GetCompanyAdvancedParameterLite(strCompanyId, "ADFSEnabled")) Then
                        Dim iSSOVersion As Integer = Robotics.VTBase.roTypes.Any2Integer(API.CommonServiceMethods.GetCompanyAdvancedParameterLite(strCompanyId, "VisualTime.SSO.ConfigurationVersion"))
                        Dim oParamSSOType As String = API.CommonServiceMethods.GetCompanyAdvancedParameterLite(strCompanyId, "SSOType")

                        If iSSOVersion = 2 AndAlso oParamSSOType.ToUpper() <> "CEGIDID" Then
                            app.Map("/Auth/" & oCompany.Id.ToLower, BuildPredicateForVTLogin_V2(oCompany, "Auth"))
                            app.Map("/PortalAuth/" & oCompany.Id.ToLower, BuildPredicateForVTLogin_V2(oCompany, "PortalAuth"))
                        End If
                    End If
                Catch ex As Exception
                    Robotics.VTBase.roLog.GetInstance().logMessage(Robotics.VTBase.roLog.EventType.roError, "OWINStartup::MT::BuildPredicateForConfigCompanyId", ex)
                End Try
            Next


            'Map legacy configuration and default cookie behaviour
            app.Properties("Microsoft.Owin.Security.Constants.DefaultSignInAsAuthenticationType") = "ExternalCookie"
            app.UseCookieAuthentication(New CookieAuthenticationOptions() With
                                        {
                                            .AuthenticationType = "ExternalCookie",
                                            .AuthenticationMode = Microsoft.Owin.Security.AuthenticationMode.Passive
                                        })

            app.Map("/VTLogin", BuildPredicateForVTLogin_V1(oCompanies))
        End Sub

        Private Function BuildPredicateForVTLogin_V1(oCompanies As Robotics.Base.DTOs.roCompanyConfiguration()) As Action(Of IAppBuilder)

            Return Sub(app1 As IAppBuilder)
                       For Each oCompany As Robotics.Base.DTOs.roCompanyConfiguration In oCompanies
                           If String.IsNullOrEmpty(oCompany.dbconnectionstring) Then Continue For

                           Dim strCompanyId As String = oCompany.companyname
                           Try
                               If Robotics.VTBase.roTypes.Any2Boolean(API.CommonServiceMethods.GetCompanyAdvancedParameterLite(strCompanyId, "ADFSEnabled")) Then
                                   Dim iSSOVersion As Integer = Robotics.VTBase.roTypes.Any2Integer(API.CommonServiceMethods.GetCompanyAdvancedParameterLite(strCompanyId, "VisualTime.SSO.ConfigurationVersion"))

                                   If iSSOVersion < 2 Then
                                       Dim oParamSSOType As String = API.CommonServiceMethods.GetCompanyAdvancedParameterLite(strCompanyId, "SSOType")
                                       Dim sReturnURI As String = API.CommonServiceMethods.GetCompanyAdvancedParameterLite(strCompanyId, "VisualTime.SSO.ReturnURL")
                                       If Not sReturnURI.Contains("VTLogin") Then sReturnURI = sReturnURI & "VTLogin"

                                       Robotics.VTBase.roLog.GetInstance().logMessage(Robotics.VTBase.roLog.EventType.roInfo, "OWINStartup::Registering VTLOGIN client::" & oCompany.companyname & "::" & iSSOVersion & "::" & oParamSSOType & "::" & sReturnURI)

                                       Select Case oParamSSOType.Trim.ToUpper
                                           Case "ADFS"
                                               CommonStartup.ConfigADFS(strCompanyId, app1,
                                                                        API.CommonServiceMethods.GetCompanyAdvancedParameterLite(strCompanyId, "VisualTime.SSO.ADFS.Metadata"),
                                                                        API.CommonServiceMethods.GetCompanyAdvancedParameterLite(strCompanyId, "VisualTime.SSO.ADFS.WTRealm"),
                                                                        sReturnURI, 1)
                                           Case "AAD"
                                               CommonStartup.ConfigAAD(strCompanyId, app1,
                                                                       API.CommonServiceMethods.GetCompanyAdvancedParameterLite(strCompanyId, "VisualTime.SSO.AAD.ClientId"),
                                                                       API.CommonServiceMethods.GetCompanyAdvancedParameterLite(strCompanyId, "VisualTime.SSO.AAD.TenantId"),
                                                                       sReturnURI, 1)
                                           Case "OKTA"
                                               CommonStartup.ConfigOKTA(strCompanyId, app1,
                                                                        API.CommonServiceMethods.GetCompanyAdvancedParameterLite(strCompanyId, "VisualTime.SSO.OKTA.ClientId"),
                                                                        API.CommonServiceMethods.GetCompanyAdvancedParameterLite(strCompanyId, "VisualTime.SSO.OKTA.Authority"),
                                                                        API.CommonServiceMethods.GetCompanyAdvancedParameterLite(strCompanyId, "VisualTime.SSO.OKTA.SecretId"),
                                                                        sReturnURI, 1)
                                           Case "SAML"
                                               CommonStartup.ConfigSAML(strCompanyId, app1,
                                                                        API.CommonServiceMethods.GetCompanyAdvancedParameterLite(strCompanyId, "VisualTime.SSO.SAML.MetadataFile"),
                                                                        API.CommonServiceMethods.GetCompanyAdvancedParameterLite(strCompanyId, "VisualTime.SSO.SAML.IPId"),
                                                                        API.CommonServiceMethods.GetCompanyAdvancedParameterLite(strCompanyId, "VisualTime.SSO.SAML.SigningBehaviour"),
                                                                        sReturnURI, 1, "")
                                       End Select
                                   End If
                               End If
                           Catch ex As Exception
                               Robotics.VTBase.roLog.GetInstance().logMessage(Robotics.VTBase.roLog.EventType.roError, "OWINStartup::MT::BuildPredicateForConfigCompanyId", ex)
                           End Try
                       Next
                   End Sub

        End Function

        Private Function BuildPredicateForVTLogin_V2(oCompany As Robotics.Base.DTOs.roCompanyConfiguration, ByVal sourcePath As String) As Action(Of IAppBuilder)

            Return Sub(app1 As IAppBuilder)
                       Dim strCompanyId As String = oCompany.companyname
                       Try
                           If Robotics.VTBase.roTypes.Any2Boolean(API.CommonServiceMethods.GetCompanyAdvancedParameterLite(strCompanyId, "ADFSEnabled")) Then
                               Dim iSSOVersion As Integer = Robotics.VTBase.roTypes.Any2Integer(API.CommonServiceMethods.GetCompanyAdvancedParameterLite(strCompanyId, "VisualTime.SSO.ConfigurationVersion"))

                               If iSSOVersion = 2 Then
                                   Dim oParamSSOType As String = API.CommonServiceMethods.GetCompanyAdvancedParameterLite(strCompanyId, "SSOType")
                                   Dim sReturnURI As String = API.CommonServiceMethods.GetCompanyAdvancedParameterLite(strCompanyId, "VisualTime.SSO.ReturnURL")

                                   Robotics.VTBase.roLog.GetInstance().logMessage(Robotics.VTBase.roLog.EventType.roInfo, "OWINStartup::Registering " & sourcePath & " client::" & oCompany.companyname & "::" & iSSOVersion & "::" & oParamSSOType & "::" & sReturnURI)

                                   Select Case oParamSSOType.Trim.ToUpper
                                       Case "ADFS"
                                           sReturnURI = sReturnURI & "Auth/" & oCompany.Id.ToLower
                                           CommonStartup.ConfigADFS(strCompanyId, app1,
                                                                    API.CommonServiceMethods.GetCompanyAdvancedParameterLite(strCompanyId, "VisualTime.SSO.ADFS.Metadata"),
                                                                    API.CommonServiceMethods.GetCompanyAdvancedParameterLite(strCompanyId, "VisualTime.SSO.ADFS.WTRealm"),
                                                                    sReturnURI, 2)
                                       Case "AAD"
                                           sReturnURI = sReturnURI & "Auth/" & oCompany.Id.ToLower
                                           CommonStartup.ConfigAAD(strCompanyId, app1,
                                                                   API.CommonServiceMethods.GetCompanyAdvancedParameterLite(strCompanyId, "VisualTime.SSO.AAD.ClientId"),
                                                                   API.CommonServiceMethods.GetCompanyAdvancedParameterLite(strCompanyId, "VisualTime.SSO.AAD.TenantId"),
                                                                   sReturnURI, 2)
                                       Case "OKTA"
                                           sReturnURI = sReturnURI & "Auth/" & oCompany.Id.ToLower
                                           CommonStartup.ConfigOKTA(strCompanyId, app1,
                                                                    API.CommonServiceMethods.GetCompanyAdvancedParameterLite(strCompanyId, "VisualTime.SSO.OKTA.ClientId"),
                                                                    API.CommonServiceMethods.GetCompanyAdvancedParameterLite(strCompanyId, "VisualTime.SSO.OKTA.Authority"),
                                                                    API.CommonServiceMethods.GetCompanyAdvancedParameterLite(strCompanyId, "VisualTime.SSO.OKTA.SecretId"),
                                                                    sReturnURI, 2)
                                       Case "SAML"
                                           sReturnURI = sReturnURI & sourcePath & "/" & oCompany.Id.ToLower
                                           CommonStartup.ConfigSAML(strCompanyId, app1,
                                                                    API.CommonServiceMethods.GetCompanyAdvancedParameterLite(strCompanyId, "VisualTime.SSO.SAML.MetadataFile"),
                                                                    API.CommonServiceMethods.GetCompanyAdvancedParameterLite(strCompanyId, "VisualTime.SSO.SAML.IPId"),
                                                                    API.CommonServiceMethods.GetCompanyAdvancedParameterLite(strCompanyId, "VisualTime.SSO.SAML.SigningBehaviour"),
                                                                    sReturnURI, 2, sourcePath)
                                   End Select
                               End If
                           End If
                       Catch ex As Exception
                           Robotics.VTBase.roLog.GetInstance().logMessage(Robotics.VTBase.roLog.EventType.roError, "OWINStartup::MT::BuildPredicateForConfigCompanyId", ex)
                       End Try
                   End Sub
        End Function

    End Class

End Namespace