Imports Robotics.Base.DTOs
Imports Robotics.Security
Imports Robotics.Security.Base

Namespace API

    Public NotInheritable Class SecurityOptionsServiceMethods

        Public Shared Function GetSecurityOptions(ByVal oPage As System.Web.UI.Page, ByVal _IDPassport As Integer, ByVal bAudit As Boolean, Optional ByVal passportTicket As roPassportTicket = Nothing, Optional ByVal excludeState As Boolean = False) As roSecurityOptions

            Dim oRet As roSecurityOptions = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.SecurityOptionState

            WebServiceHelper.SetState(oState)

            Try
                Dim wsRet As roGenericVtResponse(Of roSecurityOptions) = VTLiveApi.SecurityOptionMethods.GetSecurityOptions(_IDPassport, bAudit, oState)

                oSession.States.SecurityOptionState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.SecurityOptionState.Result <> SecurityOptionsResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.SecurityOptionState)
                End If

                oRet = wsRet.Value
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-592")
            End Try

            Return oRet

        End Function

        Public Shared Function IsValidPwdHistory(ByVal oPage As System.Web.UI.Page, ByVal _IDPassport As Integer, ByVal strPwd As String, ByVal PreviousPasswordsStored As Integer, ByVal bAudit As Boolean) As Boolean

            'Dim oRet As Boolean

            'Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            'Dim oState As roSecurityOptionsState = oSession.States.SecurityOptionsState

            'WebServiceHelper.SetState(oState, oPage)

            'Try

            '    oRet = oSession.AccessApi.WebServices.SecurityOptionsService.IsValidPwdHistory(_IDPassport, strPwd, PreviousPasswordsStored, oState)

            '    oSession.States.SecurityOptionsState = oState
            '    roWsUserManagement.SessionObject = oSession

            '    If oSession.States.SecurityOptionState.Result <> ResultSecurityOptionsEnum.NoError Then
            '        HelperWeb.ShowError(oPage, oSession.States.SecurityOptionState)
            '    End If
            'Catch ex As Exception
            '    Dim oTmpState As New Robotics.Base.DTOs.roWsState
            '    oTmpState.Result = 1
            '    Dim oLanguage As New roLanguageWeb
            '    oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
            '    HelperWeb.ShowError(oPage, oTmpState, "9-BW01-593")
            'End Try

            'Return oRet

            Dim oRet As Boolean

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.SecurityOptionState

            WebServiceHelper.SetState(oState)

            Try
                Dim wsRet As roGenericVtResponse(Of Boolean) = VTLiveApi.SecurityOptionMethods.IsValidPwdHistory(_IDPassport, strPwd, PreviousPasswordsStored, oState)

                oSession.States.SecurityOptionState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                oRet = wsRet.Value

                If oSession.States.SecurityOptionState.Result <> SecurityOptionsResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.SecurityOptionState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-593")
            End Try

            Return oRet

        End Function

        Public Shared Function SaveSecurityOptions(ByVal oPage As PageBase, ByVal oSecurityOptions As roSecurityOptions, ByVal bAudit As Boolean) As Boolean

            'Dim bolRet As Boolean = False

            'Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            'Dim oState As roSecurityOptionsState = oSession.States.SecurityOptionsState

            'WebServiceHelper.SetState(oState, oPage)

            'Try

            '    bolRet = oSession.AccessApi.WebServices.SecurityOptionsService.SaveSecurityOptions(oSecurityOptions, bAudit, oState)

            '    oSession.States.SecurityOptionsState = oState
            '    roWsUserManagement.SessionObject = oSession

            '    If oSession.States.SecurityOptionState.Result <> ResultSecurityOptionsEnum.NoError Then
            '        HelperWeb.ShowError(oPage, oSession.States.SecurityOptionState)
            '    End If
            'Catch ex As Exception
            '    Dim oTmpState As New Robotics.Base.DTOs.roWsState
            '    oTmpState.Result = 1
            '    Dim oLanguage As New roLanguageWeb
            '    oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
            '    HelperWeb.ShowError(oPage, oTmpState, "9-BW01-594")
            'End Try

            'Return bolRet

            Dim oRet As Boolean

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.SecurityOptionState

            WebServiceHelper.SetState(oState)

            Try
                Dim wsRet As roGenericVtResponse(Of Boolean) = VTLiveApi.SecurityOptionMethods.SaveSecurityOptions(oSecurityOptions, bAudit, oState)

                oSession.States.SecurityOptionState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                oRet = wsRet.Value

                If oSession.States.SecurityOptionState.Result <> SecurityOptionsResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.SecurityOptionState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-594")
            End Try

            Return oRet

        End Function

        Public Shared Function GetInheritedSecurityOptions(ByVal oPage As System.Web.UI.Page, ByVal _IDPassport As Integer, ByVal bAudit As Boolean) As roSecurityOptions

            'Dim oRet As SecurityOptionsService.roSecurityOptions = Nothing

            'Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            'Dim oState As roSecurityOptionsState = oSession.States.SecurityOptionsState

            'WebServiceHelper.SetState(oState, oPage)

            'Try

            '    oRet = oSession.AccessApi.WebServices.SecurityOptionsService.GetInheritedSecurityOptions(_IDPassport, bAudit, oState)

            '    oSession.States.SecurityOptionsState = oState
            '    roWsUserManagement.SessionObject = oSession

            '    If oSession.States.SecurityOptionState.Result <> ResultSecurityOptionsEnum.NoError Then
            '        HelperWeb.ShowError(oPage, oSession.States.SecurityOptionState)
            '    End If

            'Catch ex As Exception
            '    Dim oTmpState As New Robotics.Base.DTOs.roWsState
            '    oTmpState.Result = 1
            '    Dim oLanguage As New roLanguageWeb
            '    oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
            '    HelperWeb.ShowError(oPage, oTmpState, "9-BW01-595")
            'End Try

            'Return oRet

            Dim oRet As roSecurityOptions = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.SecurityOptionState

            WebServiceHelper.SetState(oState)

            Try
                Dim wsRet As roGenericVtResponse(Of roSecurityOptions) = VTLiveApi.SecurityOptionMethods.GetInheritedSecurityOptions(_IDPassport, bAudit, oState)

                oSession.States.SecurityOptionState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.SecurityOptionState.Result <> SecurityOptionsResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.SecurityOptionState)
                End If

                oRet = wsRet.Value
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-595")
            End Try

            Return oRet

        End Function

        Public Shared Function LastErrorText() As String
            Dim strRet As String = ""
            If roWsUserManagement.SessionObject() IsNot Nothing Then
                strRet = roWsUserManagement.SessionObject().States.SecurityOptionState.ErrorText
            End If
            Return strRet
        End Function

    End Class

End Namespace