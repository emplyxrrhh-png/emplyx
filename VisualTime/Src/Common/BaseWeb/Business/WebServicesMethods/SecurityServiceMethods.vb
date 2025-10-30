Imports System.Web
Imports System.Web.UI
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common.AdvancedParameter
Imports Robotics.Security
Imports Robotics.VTBase

Namespace API

    Public NotInheritable Class SecurityServiceMethods

        Public Shared Function GetSSOConfiguration(ByVal oPage As Page, ByVal bolaudit As Boolean) As roSSOConfiguration
            Dim oRet As New roSSOConfiguration

            Try

                Select Case VTBase.roTypes.Any2String(API.CommonServiceMethods.GetAdvancedParameter(Nothing, "SSOType").Value).ToLowerInvariant
                    Case "cegidid"
                        oRet.SSOType = SSOType.cegidId
                    Case "aad"
                        oRet.SSOType = SSOType.AAD
                    Case "adfs"
                        oRet.SSOType = SSOType.Adfs
                    Case "okta"
                        oRet.SSOType = SSOType.Okta
                    Case "saml"
                        oRet.SSOType = SSOType.SAML
                    Case Else
                        oRet.SSOType = SSOType.None
                End Select

                oRet.Active = VTBase.roTypes.Any2Boolean(API.CommonServiceMethods.GetAdvancedParameter(Nothing, "AdfsEnabled").Value)

                oRet.AAD.ClientID = VTBase.roTypes.Any2String(API.CommonServiceMethods.GetAdvancedParameter(Nothing, "VisualTime.SSO.AAD.ClientId").Value)
                oRet.AAD.TenantID = VTBase.roTypes.Any2String(API.CommonServiceMethods.GetAdvancedParameter(Nothing, "VisualTime.SSO.AAD.TenantId").Value)

                oRet.Adfs.FedartionURL = VTBase.roTypes.Any2String(API.CommonServiceMethods.GetAdvancedParameter(Nothing, "VisualTime.SSO.ADFS.Metadata").Value)
                oRet.Adfs.FederationRealm = VTBase.roTypes.Any2String(API.CommonServiceMethods.GetAdvancedParameter(Nothing, "VisualTime.SSO.ADFS.WTRealm").Value)

                oRet.Okta.ClientId = VTBase.roTypes.Any2String(API.CommonServiceMethods.GetAdvancedParameter(Nothing, "VisualTime.SSO.OKTA.ClientId").Value)
                oRet.Okta.Authority = VTBase.roTypes.Any2String(API.CommonServiceMethods.GetAdvancedParameter(Nothing, "VisualTime.SSO.OKTA.Authority").Value)
                oRet.Okta.ClientSecret = VTBase.roTypes.Any2String(API.CommonServiceMethods.GetAdvancedParameter(Nothing, "VisualTime.SSO.OKTA.SecretId").Value)

                oRet.VTLiveMixAuthEnabled = VTBase.roTypes.Any2Boolean(API.CommonServiceMethods.GetAdvancedParameter(Nothing, "VisualTime.SSO.VTLiveMixedAuthEnabled").Value)
                oRet.VTPortalMixAuthEnabled = VTBase.roTypes.Any2Boolean(API.CommonServiceMethods.GetAdvancedParameter(Nothing, "VisualTime.SSO.VTPortalMixedAuthEnabled").Value)

                oRet.SAML.MetadataURL = VTBase.roTypes.Any2String(API.CommonServiceMethods.GetAdvancedParameter(Nothing, "VisualTime.SSO.SAML.MetadataFile").Value)
                oRet.SAML.IdentityProviderID = VTBase.roTypes.Any2String(API.CommonServiceMethods.GetAdvancedParameter(Nothing, "VisualTime.SSO.SAML.IPId").Value)
                oRet.SAML.SigningBehaviour = VTBase.roTypes.Any2String(API.CommonServiceMethods.GetAdvancedParameter(Nothing, "VisualTime.SSO.SAML.SigningBehaviour").Value)

                If bolaudit Then
                    API.AuditServiceMethods.Audit(Robotics.VTBase.Audit.Action.aSelect, Robotics.VTBase.Audit.ObjectType.tSSOConfig, "", New List(Of String), New List(Of String), oPage)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-596")
            End Try

            Return oRet
        End Function

        Public Shared Function SaveSSOConfiguration(ByVal oPage As Page, ByVal oConf As roSSOConfiguration, ByVal serverAuthority As String) As Boolean
            Dim oRet As Boolean = True

            Try
                Dim bSave As Boolean = True
                Select Case oConf.SSOType
                    Case SSOType.AAD
                        If Not String.IsNullOrWhiteSpace(oConf.AAD.ClientID) AndAlso Not String.IsNullOrWhiteSpace(oConf.AAD.TenantID) Then
                            API.CommonServiceMethods.SaveAdvancedParameter(Nothing, New roAdvancedParameter() With {.Name = "SSOType", .Value = "AAD"}, False)
                        Else
                            bSave = False
                        End If
                    Case SSOType.Adfs
                        If Not String.IsNullOrWhiteSpace(oConf.Adfs.FedartionURL) AndAlso Not String.IsNullOrWhiteSpace(oConf.Adfs.FederationRealm) Then
                            API.CommonServiceMethods.SaveAdvancedParameter(Nothing, New roAdvancedParameter() With {.Name = "SSOType", .Value = "ADFS"}, False)
                        Else
                            bSave = False
                        End If

                    Case SSOType.Okta
                        If Not String.IsNullOrWhiteSpace(oConf.Okta.ClientId) AndAlso Not String.IsNullOrWhiteSpace(oConf.Okta.Authority) AndAlso Not String.IsNullOrWhiteSpace(oConf.Okta.ClientSecret) Then
                            API.CommonServiceMethods.SaveAdvancedParameter(Nothing, New roAdvancedParameter() With {.Name = "SSOType", .Value = "OKTA"}, False)
                        Else
                            bSave = False
                        End If

                    Case SSOType.SAML
                        If Not String.IsNullOrWhiteSpace(oConf.SAML.MetadataURL) AndAlso Not String.IsNullOrWhiteSpace(oConf.SAML.IdentityProviderID) AndAlso Not String.IsNullOrWhiteSpace(oConf.SAML.SigningBehaviour) Then
                            API.CommonServiceMethods.SaveAdvancedParameter(Nothing, New roAdvancedParameter() With {.Name = "SSOType", .Value = "SAML"}, False)
                        Else
                            bSave = False
                        End If
                    Case SSOType.cegidId
                        API.CommonServiceMethods.SaveAdvancedParameter(Nothing, New roAdvancedParameter() With {.Name = "SSOType", .Value = "CEGIDID"}, False)
                End Select

                If bSave Then
                    API.CommonServiceMethods.SaveAdvancedParameter(Nothing, New roAdvancedParameter() With {.Name = "AdfsEnabled", .Value = IIf(oConf.Active, "1", "0")}, False)

                    API.CommonServiceMethods.SaveAdvancedParameter(Nothing, New roAdvancedParameter() With {.Name = "VisualTime.SSO.AAD.ClientId", .Value = If(oConf.AAD.ClientID Is Nothing, "", oConf.AAD.ClientID.Trim)}, False)
                    API.CommonServiceMethods.SaveAdvancedParameter(Nothing, New roAdvancedParameter() With {.Name = "VisualTime.SSO.AAD.TenantId", .Value = If(oConf.AAD.TenantID Is Nothing, "", oConf.AAD.TenantID.Trim)}, False)

                    API.CommonServiceMethods.SaveAdvancedParameter(Nothing, New roAdvancedParameter() With {.Name = "VisualTime.SSO.ADFS.Metadata", .Value = If(oConf.Adfs.FedartionURL Is Nothing, "", oConf.Adfs.FedartionURL.Trim)}, False)
                    API.CommonServiceMethods.SaveAdvancedParameter(Nothing, New roAdvancedParameter() With {.Name = "VisualTime.SSO.ADFS.WTRealm", .Value = If(oConf.Adfs.FederationRealm Is Nothing, "", oConf.Adfs.FederationRealm.Trim)}, False)

                    API.CommonServiceMethods.SaveAdvancedParameter(Nothing, New roAdvancedParameter() With {.Name = "VisualTime.SSO.OKTA.ClientId", .Value = If(oConf.Okta.ClientId Is Nothing, "", oConf.Okta.ClientId.Trim)}, False)
                    API.CommonServiceMethods.SaveAdvancedParameter(Nothing, New roAdvancedParameter() With {.Name = "VisualTime.SSO.OKTA.Authority", .Value = If(oConf.Okta.Authority Is Nothing, "", oConf.Okta.Authority.Trim)}, False)
                    API.CommonServiceMethods.SaveAdvancedParameter(Nothing, New roAdvancedParameter() With {.Name = "VisualTime.SSO.OKTA.SecretId", .Value = If(oConf.Okta.ClientSecret Is Nothing, "", oConf.Okta.ClientSecret.Trim)}, False)

                    API.CommonServiceMethods.SaveAdvancedParameter(Nothing, New roAdvancedParameter() With {.Name = "VisualTime.SSO.SAML.MetadataFile", .Value = If(oConf.SAML.MetadataURL Is Nothing, "", oConf.SAML.MetadataURL.Trim)}, False)
                    API.CommonServiceMethods.SaveAdvancedParameter(Nothing, New roAdvancedParameter() With {.Name = "VisualTime.SSO.SAML.IPId", .Value = If(oConf.SAML.IdentityProviderID Is Nothing, "", oConf.SAML.IdentityProviderID.Trim)}, False)
                    API.CommonServiceMethods.SaveAdvancedParameter(Nothing, New roAdvancedParameter() With {.Name = "VisualTime.SSO.SAML.SigningBehaviour", .Value = If(oConf.SAML.SigningBehaviour Is Nothing, "", oConf.SAML.SigningBehaviour.Trim)}, False)

                    API.CommonServiceMethods.SaveAdvancedParameter(Nothing, New roAdvancedParameter() With {.Name = "VisualTime.SSO.VTLiveMixedAuthEnabled", .Value = "1"}, False)
                    API.CommonServiceMethods.SaveAdvancedParameter(Nothing, New roAdvancedParameter() With {.Name = "VisualTime.SSO.VTPortalMixedAuthEnabled", .Value = IIf(oConf.VTPortalMixAuthEnabled, "1", "0")}, False)

                    API.AuditServiceMethods.Audit(Robotics.VTBase.Audit.Action.aUpdate, Robotics.VTBase.Audit.ObjectType.tSSOConfig, "", New List(Of String), New List(Of String), oPage)

                    Dim vtLiveAuthority As String = HttpContext.Current.Request.Url.Authority.ToLower
                    API.CommonServiceMethods.SaveAdvancedParameter(Nothing, New roAdvancedParameter() With {.Name = "VisualTime.SSO.LivePortalAppUrl", .Value = "https://" & vtLiveAuthority.Replace("live", "portal") & "/"}, False)
                    API.CommonServiceMethods.SaveAdvancedParameter(Nothing, New roAdvancedParameter() With {.Name = "VisualTime.SSO.SupervisorPortalAppUrl", .Value = "https://" & vtLiveAuthority & "/"}, False)
                    API.CommonServiceMethods.SaveAdvancedParameter(Nothing, New roAdvancedParameter() With {.Name = "VisualTime.SSO.LiveDesktopAppUrl", .Value = "https://" & vtLiveAuthority & "/"}, False)
                    API.CommonServiceMethods.SaveAdvancedParameter(Nothing, New roAdvancedParameter() With {.Name = "VisualTime.SSO.VTPortalAppUrl", .Value = "https://" & vtLiveAuthority.Replace("live", "portal") & "/"}, False)
                    API.CommonServiceMethods.SaveAdvancedParameter(Nothing, New roAdvancedParameter() With {.Name = "VisualTime.SSO.VTLiveApiUrl", .Value = "https://" & vtLiveAuthority.Replace("live", "liveapi") & "/"}, False)
                    API.CommonServiceMethods.SaveAdvancedParameter(Nothing, New roAdvancedParameter() With {.Name = "VisualTime.SSO.ReturnURL", .Value = "https://" & vtLiveAuthority & "/"}, False)

                    API.CommonServiceMethods.SaveAdvancedParameter(Nothing, New roAdvancedParameter() With {.Name = "VisualTime.SSO.ConfigurationVersion", .Value = "2"}, False)

#If DEBUG Then
                    API.CommonServiceMethods.SaveAdvancedParameter(Nothing, New roAdvancedParameter() With {.Name = "VisualTime.SSO.LivePortalAppUrl", .Value = "https://localhost:8035/"}, False)
                    API.CommonServiceMethods.SaveAdvancedParameter(Nothing, New roAdvancedParameter() With {.Name = "VisualTime.SSO.SupervisorPortalAppUrl", .Value = "http://localhost:8035/"}, False)
                    API.CommonServiceMethods.SaveAdvancedParameter(Nothing, New roAdvancedParameter() With {.Name = "VisualTime.SSO.LiveDesktopAppUrl", .Value = "http://localhost:8025/"}, False)
                    API.CommonServiceMethods.SaveAdvancedParameter(Nothing, New roAdvancedParameter() With {.Name = "VisualTime.SSO.VTPortalAppUrl", .Value = "http://localhost:8035/"}, False)
                    API.CommonServiceMethods.SaveAdvancedParameter(Nothing, New roAdvancedParameter() With {.Name = "VisualTime.SSO.VTLiveApiUrl", .Value = "http://localhost:8031/"}, False)
                    API.CommonServiceMethods.SaveAdvancedParameter(Nothing, New roAdvancedParameter() With {.Name = "VisualTime.SSO.ReturnURL", .Value = "http://localhost:8025/"}, False)
#End If



                Else
                    oRet = False
                End If
            Catch ex As Exception
                oRet = False
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-596")
            End Try

            Return oRet
        End Function

        Public Shared Function Authenticate(ByVal oPage As Page, ByVal oPassportTicket As roPassportTicket, ByVal method As AuthenticationMethod, ByVal credential As String, ByRef password As String, ByVal hashPassword As Boolean, Optional ByVal strAPP As String = "0",
                                            Optional ByVal strVersionAPP As String = "", Optional ByVal strVersionServer As String = "", Optional ByVal excludeState As Boolean = False, Optional ByVal strMail As String = "",
                                            Optional ByVal _passportTicket As roPassportTicket = Nothing, Optional ByVal isSSOLogin As Boolean = False, Optional ByVal bolAudit As Boolean = False, Optional ByVal strAuthToken As String = "") As roPassportTicket

            Dim oRet As roPassportTicket = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.SecurityState

            If _passportTicket Is Nothing Then
                WebServiceHelper.SetState(oState)
            Else
                WebServiceHelper.SetState(oState, _passportTicket.ID)
            End If

            Try

                Dim strSecurityToken As String = ""

                Dim response As roGenericVtResponse(Of (roPassportTicket, String)) = VTLiveApi.SecurityBaseMethods.Authenticate(oPassportTicket, method, credential, password, hashPassword, oState, strAPP, strVersionAPP, strVersionServer, strMail, isSSOLogin, If(strAuthToken = String.Empty, oSession.PassportGUID, strAuthToken), strSecurityToken, bolAudit)

                oRet = response.Value.Item1
                strSecurityToken = response.Value.Item2

                oSession.States.SecurityState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.SecurityState.Result = SecurityResultEnum.NoError OrElse oSession.States.SecurityState.Result = SecurityResultEnum.IsExpired OrElse
                oSession.States.SecurityState.Result = SecurityResultEnum.NeedTemporaryKeyRequest OrElse oSession.States.SecurityState.Result = SecurityResultEnum.NeedTemporaryKeyRequestExpired OrElse
                oSession.States.SecurityState.Result = SecurityResultEnum.NeedTemporaryKeyRequestRobotics OrElse oSession.States.SecurityState.Result = SecurityResultEnum.NeedTemporaryKeyRequestExpiredRobotics OrElse oSession.States.SecurityState.Result = SecurityResultEnum.NeedMailRequest Then
                    WLHelperWeb.SecurityToken = strSecurityToken
                    HelperWeb.EraseCookie("ro_SessionID")
                    HelperWeb.CreateCookie("ro_SessionID", Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(strSecurityToken & "_roSecurityToken_" & HttpContext.Current.Session("WLPASSPORT_GUID"))))
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-596")
            End Try

            Return oRet

        End Function

        Public Shared Function ResetValidationCodeRobotics(ByVal oPage As Page, ByVal intIDPassport As Integer) As Boolean
            Dim oRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.SecurityState

            Try

                Dim response As roGenericVtResponse(Of Boolean) = VTLiveApi.SecurityBaseMethods.ResetValidationCodeRobotics(intIDPassport, oState)
                oRet = response.Value

                oSession.States.SecurityState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.SecurityState.Result <> SecurityResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.SecurityState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-597")
            End Try

            Return oRet

        End Function

        Public Shared Function ResetValidationCode(ByVal oPage As Page, ByVal intIDPassport As Integer, ByVal strClientLocation As String) As Boolean
            Dim oRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.SecurityState

            Try

                Dim response As roGenericVtResponse(Of Boolean) = VTLiveApi.SecurityBaseMethods.ResetValidationCode(intIDPassport, strClientLocation, oState)
                oRet = response.Value

                oSession.States.SecurityState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.SecurityState.Result <> SecurityResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.SecurityState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-598")
            End Try

            Return oRet

        End Function

        Public Shared Function IsValidCode(ByVal oPage As Page, ByVal intIDPassport As Integer, ByVal strCode As String) As Boolean
            Dim oRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.SecurityState

            Try

                Dim response As roGenericVtResponse(Of Boolean) = VTLiveApi.SecurityBaseMethods.IsValidCode(intIDPassport, strCode, oState)
                oRet = response.Value

                oSession.States.SecurityState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.SecurityState.Result <> SecurityResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.SecurityState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-599")
            End Try

            Return oRet
        End Function

        Public Shared Function GetPassportTicket(ByVal oPage As Page, ByVal intIDPassport As Integer) As roPassportTicket

            Dim oRet As roPassportTicket = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.SecurityState

            Try

                Dim response As roGenericVtResponse(Of roPassportTicket) = VTLiveApi.SecurityBaseMethods.GetPassportTicket(intIDPassport, oState)
                oRet = response.Value

                oSession.States.SecurityState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.SecurityState.Result <> SecurityResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.SecurityState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-600")
            End Try

            Return oRet

        End Function

        Public Shared Function GetContext(ByVal oPage As Page, ByVal intIDPassport As Integer, Optional ByVal excludeState As Boolean = False) As CContext

            Dim oRet As CContext = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.SecurityState

            WebServiceHelper.SetState(oState)

            Try
                Dim response As roGenericVtResponse(Of CContext) = VTLiveApi.SecurityBaseMethods.GetContext(intIDPassport, oState)
                oRet = response.Value

                oSession.States.SecurityState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.SecurityState.Result <> SecurityResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.SecurityState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-601")
            End Try

            Return oRet

        End Function

        Public Shared Function SetContext(ByVal oPage As Page, ByVal intIDPassport As Integer, ByVal oContext As CContext) As Boolean
            Dim bRet As Boolean = False
            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.SecurityState

            WebServiceHelper.SetState(oState, intIDPassport)

            Try
                Dim response As roGenericVtResponse(Of Boolean) = VTLiveApi.SecurityBaseMethods.SetContext(intIDPassport, oContext, oState)
                bRet = response.Value

                oSession.States.SecurityState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.SecurityState.Result <> SecurityResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.SecurityState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-602")
            End Try

            Return bRet
        End Function

        Public Shared Function SignOut(ByVal oPage As Page, ByVal intIDPassport As Integer, Optional ByVal excludeState As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.SecurityState

            WebServiceHelper.SetState(oState, intIDPassport)

            Try
                Dim response As roGenericVtResponse(Of Boolean) = VTLiveApi.SecurityBaseMethods.SignOut(intIDPassport, oState)
                bolRet = response.Value

                oSession.States.SecurityState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.SecurityState.Result <> SecurityResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.SecurityState)
                End If

                If bolRet Then
                    WLHelperWeb.CurrentPassport = Nothing
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-603")
            End Try

            Return bolRet

        End Function

        Public Shared Function GetPassportBelongsToAdminGroup(ByVal oPage As Page, ByVal intIdPassport As Integer) As Boolean
            Dim oRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.SecurityState

            WebServiceHelper.SetState(oState)

            Try
                Dim response As roGenericVtResponse(Of Boolean) = VTLiveApi.SecurityBaseMethods.GetPassportBelongsToAdminGroup(intIdPassport, oState)
                oRet = response.Value

                oSession.States.SecurityState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.SecurityState.Result <> SecurityResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.SecurityState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-604")
            End Try

            Return oRet
        End Function

        ''' <summary>
        ''' Returns the permission current passport have over specified feature.
        ''' </summary>
        ''' <param name="featureAlias">The alias of the feature to check permissions for.</param>
        ''' <param name="featureType">The type of feature: 'E' for Employee or 'U' for User.</param>
        Public Shared Function GetPermissionOverFeature(ByVal oPage As Page, ByVal featureAlias As String, ByVal featureType As String) As Permission

            Dim oRet As Permission = Permission.None

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.SecurityState

            WebServiceHelper.SetState(oState)

            Try
                If WLHelperWeb.CurrentPassportID > 0 Then
                    Dim key As String = featureAlias & "_" & featureType
                    If WLHelperWeb.PassportFeautrePemissions.Contains(key) Then
                        oRet = CType(WLHelperWeb.PassportFeautrePemissions(key), Permission)
                    Else
                        Dim response As roGenericVtResponse(Of Permission) = VTLiveApi.SecurityBaseMethods.GetPermissionOverFeature(featureAlias, featureType, oState)
                        oRet = response.Value

                        oSession.States.SecurityState = response.Status
                        roWsUserManagement.SessionObject = oSession

                        If oSession.States.SecurityState.Result <> SecurityResultEnum.NoError Then
                            HelperWeb.ShowError(oPage, oSession.States.SecurityState)
                        Else
                            WLHelperWeb.PassportFeautrePemissions.Add(key, oRet)
                        End If
                    End If
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-605")
            End Try

            Return oRet

        End Function

        ''' <summary>
        ''' Returns the permission current passport have over specified employee.
        ''' </summary>
        ''' <param name="idEmployee">The ID of the employee for which to get permissions.</param>
        ''' <param name="applicationAlias">The ID of the application in which to check permissions.</param>
        Public Shared Function GetPermissionOverEmployeeAppAlias(ByVal oPage As Page, ByVal idEmployee As Integer, ByVal applicationAlias As String, ByVal featureType As String) As Permission

            Dim oRet As Permission = Permission.None

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.SecurityState

            WebServiceHelper.SetState(oState)

            Try
                If WLHelperWeb.CurrentPassportID > 0 Then
                    Dim key As String = idEmployee & "_" & applicationAlias & "_" & featureType
                    If WLHelperWeb.PassportPemissionsOverEmployeeApp.Contains(key) Then
                        oRet = CType(WLHelperWeb.PassportPemissionsOverEmployeeApp(key), Permission)
                    Else
                        Dim response As roGenericVtResponse(Of Permission) = VTLiveApi.SecurityBaseMethods.GetPermissionOverEmployeeAppAlias(idEmployee, applicationAlias, featureType, oState)
                        oRet = response.Value

                        oSession.States.SecurityState = response.Status
                        roWsUserManagement.SessionObject = oSession

                        If oSession.States.SecurityState.Result <> SecurityResultEnum.NoError Then
                            HelperWeb.ShowError(oPage, oSession.States.SecurityState)
                        Else
                            WLHelperWeb.PassportPemissionsOverEmployeeApp.Add(key, oRet)
                        End If
                    End If
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-606")
            End Try

            Return oRet

        End Function

        ''' <summary>
        ''' Returns the permission current passport have over specified employee.
        ''' </summary>
        ''' <param name="idEmployee">The ID of the employee for which to get permissions.</param>
        ''' <param name="applicationAlias">The ID of the application in which to check permissions.</param>
        ''' <param name="dDate">The date which to check permissions.</param>
        Public Shared Function GetPermissionOverEmployeeOnDateAppAlias(ByVal oPage As Page, ByVal idEmployee As Integer, ByVal applicationAlias As String, ByVal dDate As DateTime, ByVal featureType As String) As Permission

            Dim oRet As Permission = Permission.None

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.SecurityState

            WebServiceHelper.SetState(oState)

            Try
                Dim response As roGenericVtResponse(Of Permission) = VTLiveApi.SecurityBaseMethods.GetPermissionOverEmployeeOnDateAppAlias(idEmployee, applicationAlias, dDate, featureType, oState)
                oRet = response.Value

                oSession.States.SecurityState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.SecurityState.Result <> SecurityResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.SecurityState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-607")
            End Try

            Return oRet

        End Function

        ''' <summary>
        ''' Returns the permission current passport have over specified employee.
        ''' </summary>
        ''' <param name="idEmployee">The ID of the employee for which to get permissions.</param>
        ''' <param name="idApplication">The ID of the application in which to check permissions.</param>
        Public Shared Function GetPermissionOverEmployeeAppId(ByVal oPage As Page, ByVal idEmployee As Integer, ByVal idApplication As Integer) As Permission

            Dim oRet As Permission = Permission.None

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.SecurityState

            WebServiceHelper.SetState(oState)

            Try
                If WLHelperWeb.CurrentPassportID > 0 Then
                    Dim key As String = idEmployee & "_" & idApplication
                    If WLHelperWeb.PassportPemissionsOverEmployee.Contains(key) Then
                        oRet = CType(WLHelperWeb.PassportPemissionsOverEmployee(key), Permission)
                    Else
                        Dim response As roGenericVtResponse(Of Permission) = VTLiveApi.SecurityBaseMethods.GetPermissionOverEmployeeAppId(idEmployee, idApplication, oState)
                        oRet = response.Value

                        oSession.States.SecurityState = response.Status
                        roWsUserManagement.SessionObject = oSession

                        If oSession.States.SecurityState.Result <> SecurityResultEnum.NoError Then
                            HelperWeb.ShowError(oPage, oSession.States.SecurityState)
                        Else
                            WLHelperWeb.PassportPemissionsOverEmployee.Add(key, oRet)
                        End If
                    End If
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-608")
            End Try

            Return oRet

        End Function

        ''' <summary>
        ''' Returns the permission current passport have over specified group.
        ''' </summary>
        ''' <param name="idGroup">The ID of the group for which to get permissions.</param>
        ''' <param name="applicationAlias">The alias of the application in which to check permissions.</param>
        Public Shared Function GetPermissionOverGroupAppAlias(ByVal oPage As Page, ByVal idGroup As Integer, ByVal applicationAlias As String, ByVal featureType As String) As Permission

            Dim oRet As Permission = Permission.None

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.SecurityState

            WebServiceHelper.SetState(oState)

            Try
                If WLHelperWeb.CurrentPassportID > 0 Then
                    Dim key As String = idGroup & "_" & applicationAlias & "_" & featureType
                    If WLHelperWeb.PassportPemissionsOverGroupApp.Contains(key) Then
                        oRet = CType(WLHelperWeb.PassportPemissionsOverGroupApp(key), Permission)
                    Else
                        Dim response As roGenericVtResponse(Of Permission) = VTLiveApi.SecurityBaseMethods.GetPermissionOverGroupAppAlias(idGroup, applicationAlias, featureType, oState)
                        oRet = response.Value

                        oSession.States.SecurityState = response.Status
                        roWsUserManagement.SessionObject = oSession

                        If oSession.States.SecurityState.Result <> SecurityResultEnum.NoError Then
                            HelperWeb.ShowError(oPage, oSession.States.SecurityState)
                        Else
                            WLHelperWeb.PassportPemissionsOverGroupApp.Add(key, oRet)
                        End If
                    End If
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-609")
            End Try

            Return oRet

        End Function

        ''' <summary>
        ''' Returns the permission current passport have over specified group.
        ''' </summary>
        ''' <param name="idGroup">The ID of the group for which to get permissions.</param>
        ''' <param name="idApplication">The ID of the application in which to check permissions.</param>
        Public Shared Function GetPermissionOverGroupAppId(ByVal oPage As Page, ByVal idGroup As Integer, ByVal idApplication As Integer) As Permission

            Dim oRet As Permission = Permission.None

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.SecurityState

            WebServiceHelper.SetState(oState)

            Try
                If WLHelperWeb.CurrentPassportID > 0 Then
                    Dim key As String = idGroup & "_" & idApplication
                    If WLHelperWeb.PassportPemissionsOverGroup.Contains(key) Then
                        oRet = CType(WLHelperWeb.PassportPemissionsOverGroup(key), Permission)
                    Else
                        Dim response As roGenericVtResponse(Of Permission) = VTLiveApi.SecurityBaseMethods.GetPermissionOverGroupAppId(idGroup, idApplication, oState)
                        oRet = response.Value

                        oSession.States.SecurityState = response.Status
                        roWsUserManagement.SessionObject = oSession

                        If oSession.States.SecurityState.Result <> SecurityResultEnum.NoError Then
                            HelperWeb.ShowError(oPage, oSession.States.SecurityState)
                        Else
                            WLHelperWeb.PassportPemissionsOverGroup.Add(key, oRet)
                        End If
                    End If
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-610")
            End Try

            Return oRet

        End Function

        ''' <summary>
        ''' Returns whether current passport have specified permission over
        ''' specified feature.
        ''' </summary>
        ''' <param name="featureAlias">The alias of the feature to check permissions for.</param>
        ''' <param name="featureType">The type of feature: 'E' for Employee or 'U' for User.</param>
        ''' <param name="perm">The required permission.</param>
        Public Shared Function HasPermissionOverFeature(ByVal oPage As Page, ByVal featureAlias As String, ByVal featureType As String, ByVal perm As Permission) As Boolean

            Dim bolRet As Boolean = False

            Try
                Dim oPermission As Permission = GetPermissionOverFeature(oPage, featureAlias, featureType)

                If oPermission >= perm Then
                    bolRet = True
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-611")
            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Returns whether current passport have specified permission
        ''' over specified employee.
        ''' </summary>
        ''' <param name="idEmployee">The ID of the employee for which to get permissions.</param>
        ''' <param name="applicationAlias">The alias of the application in which to check permissions.</param>
        ''' <param name="perm">The required permission.</param>
        Public Shared Function HasPermissionOverEmployee(ByVal oPage As Page, ByVal idEmployee As Integer, ByVal applicationAlias As String, ByVal featureType As String, ByVal perm As Permission) As Boolean

            Dim bolRet As Boolean = False

            Try
                Dim oPermission As Permission = GetPermissionOverEmployeeAppAlias(oPage, idEmployee, applicationAlias, featureType)

                If oPermission >= perm Then
                    bolRet = True
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-612")
            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Returns whether current passport have specified permission
        ''' over specified employee.
        ''' </summary>
        ''' <param name="idEmployee">The ID of the employee for which to get permissions.</param>
        ''' <param name="applicationAlias">The alias of the application in which to check permissions.</param>
        ''' <param name="perm">The required permission.</param>
        Public Shared Function HasPermissionOverEmployeeOnDate(ByVal oPage As Page, ByVal idEmployee As Integer, ByVal applicationAlias As String, ByVal featureType As String, ByVal perm As Permission, ByVal dDate As DateTime) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.SecurityState

            WebServiceHelper.SetState(oState)

            Try
                Dim response As roGenericVtResponse(Of Boolean) = VTLiveApi.SecurityBaseMethods.HasPermissionOverEmployeeOnDate(idEmployee, applicationAlias, featureType, perm, dDate, oState)
                bolRet = response.Value

                oSession.States.SecurityState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.SecurityState.Result <> SecurityResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.SecurityState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-613")
            End Try

            Return bolRet

        End Function

        Public Shared Function HasPermissionOverEmployeeOnDateEx(ByVal oPage As Page, ByRef strEmployees As Generic.List(Of String), ByVal applicationAlias As String, ByVal featureType As String, ByVal perm As Permission) As Generic.List(Of String)

            Dim bolRet As New Generic.List(Of String)

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.SecurityState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of String()) = VTLiveApi.SecurityBaseMethods.HasPermissionOverEmployeeOnDateEx(strEmployees.ToArray, applicationAlias, featureType, perm, oState)

                oSession.States.SecurityState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.SecurityState.Result <> SecurityResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.SecurityState)
                Else
                    If Not response.Value Is Nothing Then
                        bolRet = response.Value.ToList
                    End If

                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-614")
            End Try

            Return bolRet

        End Function

        Public Shared Function HasPermissionOverGroupAppAliasEx(ByVal oPage As Page, ByRef strGroups As Generic.List(Of String), ByVal applicationAlias As String, ByVal featureType As String, ByVal perm As Permission) As Generic.List(Of String)

            Dim bolRet As New Generic.List(Of String)

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.SecurityState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of String()) = VTLiveApi.SecurityBaseMethods.HasPermissionOverGroupAppAliasEx(strGroups.ToArray, applicationAlias, featureType, perm, oState)

                oSession.States.SecurityState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.SecurityState.Result <> SecurityResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.SecurityState)
                Else
                    If Not response.Value Is Nothing Then
                        bolRet = response.Value.ToList
                    End If

                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-615")
            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Returns whether current passport have specified permission over specified group.
        ''' </summary>
        ''' <param name="idGroup">The ID of the group for which to get permissions.</param>
        ''' <param name="applicationAlias">The alias of the application in which to check permissions.</param>
        ''' <param name="perm">The required permission.</param>
        Public Shared Function HasPermissionOverGroupAppAlias(ByVal oPage As Page, ByVal idGroup As Integer, ByVal applicationAlias As String, ByVal featureType As String, ByVal perm As Permission) As Boolean

            Dim bolRet As Boolean = False

            Try
                Dim oPermission As Permission = GetPermissionOverGroupAppAlias(oPage, idGroup, applicationAlias, featureType)

                If oPermission >= perm Then
                    bolRet = True
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-616")
            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Returns whether current passport have specified permission over specified group.
        ''' </summary>
        ''' <param name="idGroup">The ID of the group for which to get permissions.</param>
        ''' <param name="idApplication">The ID of the application in which to check permissions.</param>
        ''' <param name="perm">The required permission.</param>
        Public Shared Function HasPermissionOverGroupAppId(ByVal oPage As Page, ByVal idGroup As Integer, ByVal idApplication As Integer, ByVal perm As Permission) As Boolean

            Dim bolRet As Boolean = False

            Try
                Dim oPermission As Permission = GetPermissionOverGroupAppId(oPage, idGroup, idApplication)

                If oPermission >= perm Then
                    bolRet = True
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-617")
            End Try

            Return bolRet

        End Function

        Public Shared Function SetLastNotificationSended(ByVal oPage As Page, ByVal intPassportID As Integer, ByVal oDate As Nullable(Of DateTime)) As Boolean
            Dim bolRet As Boolean = False
            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.SecurityState
            WebServiceHelper.SetState(oState)
            Try

                Dim response As roGenericVtResponse(Of Boolean) = VTLiveApi.SecurityBaseMethods.SetLastNotificationSended(intPassportID, oDate, oState)
                bolRet = response.Value

                oSession.States.SecurityState = response.Status
                roWsUserManagement.SessionObject = oSession
                If oSession.States.SecurityState.Result <> SecurityResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.SecurityState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-618")
            End Try
            Return bolRet
        End Function

        Public Shared Function UpdateLastAccessTimeMVC(ByVal oPage As Page) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.SecurityState

            WebServiceHelper.SetState(oState)
            Try

                If WLHelperWeb.CurrentPassportID <= 0 Then ''And oState.SessionID = "" Then
                    bolRet = False
                Else
                    Dim response As roGenericVtResponse(Of Boolean) = VTLiveApi.SecurityBaseMethods.UpdateLastAccessTimeMVC(oState)
                    bolRet = response.Value

                    oSession.States.SecurityState = response.Status
                    roWsUserManagement.SessionObject = oSession

                    If oSession.States.SecurityState.Result <> SecurityResultEnum.NoError Then
                        HelperWeb.ShowError(oPage, oSession.States.SecurityState)
                    End If
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-619")
            End Try

            Return bolRet

        End Function

        Public Shared Function UpdateLastAccessTime(ByVal oPage As Page, ByVal strSessionID As String, ByVal intPassportID As Integer) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.SecurityState

            WebServiceHelper.SetState(oState)
            Try

                If intPassportID <= 0 Then ''And oState.SessionID = "" Then
                    bolRet = False
                Else
                    If strSessionID = "" Then
                        bolRet = False
                    Else
                        Dim response As roGenericVtResponse(Of Boolean) = VTLiveApi.SecurityBaseMethods.UpdateLastAccessTime(strSessionID, intPassportID, oState)
                        bolRet = response.Value

                        oSession.States.SecurityState = response.Status
                        roWsUserManagement.SessionObject = oSession

                        If oSession.States.SecurityState.Result <> SecurityResultEnum.NoError Then
                            HelperWeb.ShowError(oPage, oSession.States.SecurityState)
                        End If
                    End If
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-619")
            End Try

            Return bolRet

        End Function

        Public Shared Function GetLanguages(ByVal oPage As Page) As roPassportLanguage()

            Dim oRet As roPassportLanguage() = Nothing
            Dim oLanguage As New roLanguageWeb
            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.SecurityState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of roPassportLanguage()) = VTLiveApi.SecurityBaseMethods.GetLanguages(oState)
                oRet = response.Value

                oSession.States.SecurityState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.SecurityState.Result <> SecurityResultEnum.NoError Then
                    oRet = {}
                    ' Mostrar el error

                    Dim oTmpState As New Robotics.Base.DTOs.roWsState
                    oTmpState.Result = 1
                    oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.9-BS01-001.WSNoDBConnection")

                    HelperWeb.ShowError(oPage, oTmpState, "9-BS01-001")
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-620")
            End Try

            Return oRet

        End Function

        Public Shared Function UpdateHelpVersion(ByVal intIDPassport As Integer, ByVal intHelpVersion As Integer, Optional ByVal excludeState As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.SecurityState

            WebServiceHelper.SetState(oState)

            Try
                Dim response As roGenericVtResponse(Of Boolean) = VTLiveApi.SecurityBaseMethods.UpdateHelpVersion(intIDPassport, intHelpVersion, oState)
                bolRet = response.Value

                oSession.States.SecurityState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.SecurityState.Result <> SecurityResultEnum.NoError Then
                    ' Mostrar el error
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
            End Try

            Return bolRet

        End Function

        Public Shared Function UpdateLastLogin(ByVal intIDPassport As Integer) As Boolean

            Dim bolRet As Boolean

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.SecurityState

            WebServiceHelper.SetState(oState)

            Try
                Dim response As Boolean = WLHelper.UpdateLastLogin(intIDPassport)
                bolRet = response

                roWsUserManagement.SessionObject = oSession

                If oSession.States.SecurityState.Result <> SecurityResultEnum.NoError Then
                    ' Mostrar el error

                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()

            End Try

            Return bolRet

        End Function

        Public Shared Function GetLastLogin(ByVal intIDPassport As Integer) As Date

            Dim bolRet As Date

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.SecurityState

            WebServiceHelper.SetState(oState)

            Try
                Dim response As Date = WLHelper.GetLastLogin(intIDPassport)
                bolRet = response

                roWsUserManagement.SessionObject = oSession

                If oSession.States.SecurityState.Result <> SecurityResultEnum.NoError Then
                    ' Mostrar el error

                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()

            End Try

            Return bolRet

        End Function

        Public Shared Function GetHelpVersion(ByVal intIDPassport As Integer, Optional ByVal excludeState As Boolean = False) As Integer

            Dim bolRet As Integer = 0

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.SecurityState

            WebServiceHelper.SetState(oState)

            Try
                Dim response As roGenericVtResponse(Of Integer) = VTLiveApi.SecurityBaseMethods.GetHelpVersion(intIDPassport, oState)
                bolRet = response.Value

                oSession.States.SecurityState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.SecurityState.Result <> SecurityResultEnum.NoError Then
                    ' Mostrar el error

                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()

            End Try

            Return bolRet

        End Function

        Public Shared Function SetLanguage(ByVal oPage As Page, ByVal intIDPassport As Integer, ByVal strLanguageKey As String, Optional ByVal excludeState As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.SecurityState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of Boolean) = VTLiveApi.SecurityBaseMethods.SetLanguage(intIDPassport, strLanguageKey, oState)
                bolRet = response.Value

                oSession.States.SecurityState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.SecurityState.Result <> SecurityResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.SecurityState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-621")
            End Try

            Return bolRet

        End Function

        Public Shared Function GetPassportTicketBySessionID(ByVal oPage As Page, ByVal PassportID As String, ByVal oMethod As AuthenticationMethod) As roPassportTicket

            Dim oRet As roPassportTicket = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.SecurityState

            roBaseState.SetSessionSmall(oState, PassportID, roConstants.GetDefaultSourceForType(roConstants.GetCurrentAppType()), HttpContext.Current.Session.SessionID)

            Try
                Dim response As roGenericVtResponse(Of roPassportTicket) = VTLiveApi.SecurityBaseMethods.GetPassportTicketBySessionID(PassportID, oMethod, oState)
                oRet = response.Value

                oSession.States.SecurityState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.SecurityState.Result <> SecurityResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.SecurityState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-622")
            End Try

            Return oRet

        End Function

        Public Shared Function SaveBlockUserByInactivity(ByVal oPage As Page, ByVal bBlockUser As Integer, ByVal iBlockUserPeriod As Integer) As Boolean
            Dim oRet As Boolean = True
            Try
                If (bBlockUser <> -1) Then
                    API.CommonServiceMethods.SaveAdvancedParameter(Nothing, New roAdvancedParameter() With {.Name = "VisualTime.Security.BlockUser", .Value = bBlockUser}, False)
                    API.CommonServiceMethods.SaveAdvancedParameter(Nothing, New roAdvancedParameter() With {.Name = "VisualTime.Security.BlockUserPeriod", .Value = iBlockUserPeriod}, False)
                End If
                Dim bReset = HelperSession.AdvancedParametersCache("RESETCACHE")
            Catch ex As Exception
                oRet = False
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                'Todo revisar mensaje error
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.SaveBlockUserByInactivity") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-596")
            End Try

            Return oRet
        End Function

        Public Shared Function SaveShowLegalText(ByVal oPage As Page, ByVal bShowText As Integer) As Boolean
            Dim oRet As Boolean = True
            Try
                If (bShowText <> -1) Then
                    API.CommonServiceMethods.SaveAdvancedParameter(Nothing, New roAdvancedParameter() With {.Name = "VisualTime.Security.ShowLegalText", .Value = bShowText}, False)
                End If
                Dim bReset = HelperSession.AdvancedParametersCache("RESETCACHE")
            Catch ex As Exception
                oRet = False
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                'Todo revisar mensaje error
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.SaveShowLegalText") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-596")
            End Try

            Return oRet
        End Function

        Public Shared Function GetVersionNotificationShown(ByVal intIDPassport As Integer, Optional ByVal excludeState As Boolean = False) As Boolean

            Dim bolRet As Integer = 0

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.SecurityState

            WebServiceHelper.SetState(oState)

            Try
                Dim response As roGenericVtResponse(Of Boolean) = VTLiveApi.SecurityBaseMethods.GetVersionNotificationShown(intIDPassport, oState)
                bolRet = response.Value

                oSession.States.SecurityState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.SecurityState.Result <> SecurityResultEnum.NoError Then
                    ' Mostrar el error

                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()

            End Try

            Return bolRet

        End Function

        Public Shared Function UpdateVersionNotification(ByVal intIDPassport As Integer, ByVal intNotificationVersion As Integer, Optional ByVal excludeState As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.SecurityState

            WebServiceHelper.SetState(oState)

            Try
                Dim response As roGenericVtResponse(Of Boolean) = VTLiveApi.SecurityBaseMethods.UpdateVersionNotification(intIDPassport, intNotificationVersion, oState)
                bolRet = response.Value

                oSession.States.SecurityState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.SecurityState.Result <> SecurityResultEnum.NoError Then
                    ' Mostrar el error
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
            End Try

            Return bolRet

        End Function

#Region "Informes de Emergencia anonimos"

        Public Shared Function GetEmergencyReportKey(ByVal oPage As System.Web.UI.Page) As String

            Dim strRet As String = String.Empty

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.SecurityState

            WebServiceHelper.SetState(oState)
            Try

                Dim response As roGenericVtResponse(Of String) = VTLiveApi.SecurityBaseMethods.GetEmergencyReportKey(oState)

                oSession.States.SecurityState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.SecurityState.Result = SecurityResultEnum.NoError Then
                    If response.Value <> String.Empty Then
                        strRet = Robotics.VTBase.CryptographyHelper.Decrypt(response.Value)
                    End If
                Else
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.SecurityState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-624")
            End Try

            Return strRet

        End Function

        Public Shared Function IsEmergencyReportActive(ByVal oPage As System.Web.UI.Page) As Boolean

            Dim oRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.SecurityState

            WebServiceHelper.SetState(oState)
            Try

                Dim response As roGenericVtResponse(Of Boolean) = VTLiveApi.SecurityBaseMethods.IsEmergencyReportActive(oState)
                oRet = response.Value

                oSession.States.SecurityState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.SecurityState.Result <> SecurityResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.SecurityState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-626")
            End Try

            Return oRet

        End Function

        Public Shared Function IsValidPwd(ByVal oPage As System.Web.UI.Page, ByVal loggedInPassport As roPassportTicket, ByVal idPassport As Integer, ByVal strPwd As String, ByVal ValidateHistory As Boolean, ByVal ActualPwd As String) As PasswordLevelError
            Dim oRet As PasswordLevelError = PasswordLevelError.No_Error
            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.SecurityState
            WebServiceHelper.SetState(oState)
            Try

                Dim response As roGenericVtResponse(Of PasswordLevelError) = VTLiveApi.SecurityBaseMethods.IsValidPwd(strPwd, loggedInPassport, idPassport, ValidateHistory, ActualPwd, oState)
                oRet = response.Value

                oSession.States.SecurityState = response.Status
                roWsUserManagement.SessionObject = oSession
                If oSession.States.SecurityState.Result <> SecurityResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.SecurityState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") & System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-627")
            End Try
            Return oRet
        End Function

        Public Shared Function ResetCache(ByVal oPage As System.Web.UI.Page, ByVal strCompanyId As String) As Boolean
            Dim oRet As PasswordLevelError = PasswordLevelError.No_Error
            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.SecurityState
            WebServiceHelper.SetState(oState)
            Try

                Dim response As roGenericVtResponse(Of Boolean) = VTLiveApi.SecurityBaseMethods.ResetCache(strCompanyId, oState)
                oRet = response.Value

                oSession.States.SecurityState = response.Status
                roWsUserManagement.SessionObject = oSession
                If oSession.States.SecurityState.Result <> SecurityResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.SecurityState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-627")
            End Try
            Return oRet
        End Function

#End Region

        Public Shared Function LastErrorText() As String
            Dim strRet As String = ""
            If roWsUserManagement.SessionObject() IsNot Nothing Then
                strRet = roWsUserManagement.SessionObject().States.SecurityState.ErrorText
            End If

            Return strRet
        End Function

    End Class

End Namespace