Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Security.Base

Namespace API

    Public NotInheritable Class SecurityV3ServiceMethods

        Public Shared Function GetRequestCategories(ByVal oPage As System.Web.UI.Page) As List(Of roSecurityCategory)
            Dim oRet As List(Of roSecurityCategory) = Nothing

            Try
                Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
                Dim oState As roWsState = oSession.States.SecurityV3State

                WebServiceHelper.SetState(oState)

                Dim oTmpObject As roGenericVtResponse(Of List(Of roSecurityCategory)) = VTLiveApi.SecurityV3Methods.GetRequestCategories(oState)
                oRet = oTmpObject.Value.ToList

                oSession.States.SecurityV3State = oTmpObject.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.SecurityV3State.Result <> SecurityResultEnum.NoError Then
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.SecurityV3State)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-850")
            End Try

            Return oRet
        End Function

        Public Shared Function GetRequestTypesWithCategories(ByVal oPage As System.Web.UI.Page) As List(Of roRequestType)
            Dim oRet As List(Of roRequestType) = Nothing

            Try
                Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
                Dim oState As roWsState = oSession.States.SecurityV3State

                WebServiceHelper.SetState(oState)

                'oRet = oSession.WebServices.CalendarService.ChangeCalenarModeToV1(strErrorTag, oState)
                Dim oTmpObject As roGenericVtResponse(Of List(Of roRequestType)) = VTLiveApi.SecurityV3Methods.GetRequestTypesWithCategories(oState)
                oRet = oTmpObject.Value.ToList

                oSession.States.SecurityV3State = oTmpObject.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.SecurityV3State.Result <> SecurityResultEnum.NoError Then
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.SecurityV3State)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-851")
            End Try

            Return oRet
        End Function

        Public Shared Function GetNotificationTypesWithCategories(ByVal oPage As System.Web.UI.Page) As List(Of roNotificationType)
            Dim oRet As List(Of roNotificationType) = Nothing

            Try
                Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
                Dim oState As roWsState = oSession.States.SecurityV3State

                WebServiceHelper.SetState(oState)

                Dim oTmpObject As roGenericVtResponse(Of List(Of roNotificationType)) = VTLiveApi.SecurityV3Methods.GetNotificationTypesWithCategories(oState)
                oRet = oTmpObject.Value.ToList

                oSession.States.SecurityV3State = oTmpObject.Status
                roWsUserManagement.SessionObject = oSession
                If oSession.States.SecurityV3State.Result <> SecurityResultEnum.NoError Then
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.SecurityV3State)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-854")
            End Try

            Return oRet
        End Function

        Public Shared Function SaveNotificationTypesWithCategories(ByVal oPage As PageBase, ByVal oNotificationTypeList As List(Of roNotificationType), ByVal bAudit As Boolean) As Boolean

            Dim oRet As Boolean
            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()

            Try
                Dim oState As roWsState = oSession.States.SecurityV3State
                WebServiceHelper.SetState(oState)

                Dim wsRet As roGenericVtResponse(Of Boolean) = VTLiveApi.SecurityV3Methods.SaveNotificationTypesWithCategories(oNotificationTypeList, oState)

                oSession.States.SecurityV3State = wsRet.Status
                roWsUserManagement.SessionObject = oSession
                If oSession.States.SecurityV3State.Result <> SecurityResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.SecurityV3State)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-853")
            End Try

            Return oRet

        End Function

        Public Shared Function GetPassportLevelOfAuthority(ByVal oPage As System.Web.UI.Page, ByVal IDPassport As Integer, ByVal xRequestType As eRequestType, ByVal IDCause As Integer, ByVal IDRequest As Integer) As Integer
            Dim oRet As Integer = 0

            Try
                Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
                Dim oState As roWsState = oSession.States.SecurityV3State

                WebServiceHelper.SetState(oState)

                Dim oTmpObject As roGenericVtResponse(Of Integer) = VTLiveApi.SecurityV3Methods.GetPassportLevelOfAuthority(oState, IDPassport, xRequestType, IDCause, IDRequest)
                oRet = oTmpObject.Value

                oSession.States.SecurityV3State = oTmpObject.Status
                roWsUserManagement.SessionObject = oSession
                If oSession.States.SecurityV3State.Result <> SecurityResultEnum.NoError Then
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.SecurityV3State)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-855")
            End Try

            Return oRet
        End Function

        Public Shared Function GetAllAvailableSupervisorsList(ByVal oPage As System.Web.UI.Page, Optional ByVal bLoadUserSystem As Boolean = False) As roPassport()
            Dim bolRet As roPassport() = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.SecurityV3State

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of roPassport()) = VTLiveApi.SecurityV3Methods.GetAllAvailableSupervisorsList(oState, bLoadUserSystem)

                oSession.States.SecurityV3State = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.SecurityV3State.Result <> SecurityNodeResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.SecurityV3State)
                End If

                bolRet = wsRet.Value
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-587")
            End Try

            Return bolRet
        End Function


        Public Shared Function GetPassportsByEmail(ByVal oPage As System.Web.UI.Page, ByVal sEmail As String) As Generic.List(Of roPassportTicket)

            Dim oRet As Generic.List(Of roPassportTicket) = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.UserAdminSecurityState

            WebServiceHelper.SetStateSmall(oState)

            Try
                Dim response As roGenericVtResponse(Of roPassportTicket()) = VTLiveApi.SecurityV3Methods.GetPassportsByEmail(oState, sEmail)

                oSession.States.UserAdminSecurityState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.SecurityV3State.Result = SecurityNodeResultEnum.NoError Then
                    oRet = response.Value.ToList
                Else
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.WscState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-760")
            End Try

            Return oRet

        End Function

        Public Shared Function CopySupervisorProperties(ByVal oPage As System.Web.UI.Page, ByVal iPassportID As Integer, ByVal iDestinationPassportIDs As Integer(), ByVal copyRestrictions As Boolean, ByVal copyCostCenters As Boolean, ByVal copyBusinessGroups As Boolean, ByVal copyCategories As Boolean, ByVal copyGroups As Boolean, ByVal copyRoles As Boolean) As Boolean
            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.SecurityV3State

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of Boolean) = VTLiveApi.SecurityV3Methods.CopySupervisorProperties(iPassportID, iDestinationPassportIDs, copyRestrictions, copyCostCenters, copyBusinessGroups, copyCategories, copyGroups, copyRoles, oState)

                oSession.States.SecurityV3State = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.SecurityV3State.Result <> SecurityNodeResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.SecurityV3State)
                End If

                bolRet = wsRet.Value
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-591")
            End Try

            Return bolRet
        End Function

        Public Shared Function GetTreeState(ByVal oPage As System.Web.UI.Page, ByVal IDPassport As Integer, ByVal idTreeState As String) As String
            Dim bolRet As String = String.Empty

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.SecurityV3State

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of String) = VTLiveApi.SecurityV3Methods.GetTreeState(oState, IDPassport, idTreeState)

                oSession.States.SecurityV3State = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.SecurityV3State.Result <> SecurityNodeResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.SecurityV3State)
                End If

                bolRet = wsRet.Value
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-591")
            End Try

            Return bolRet
        End Function

        Public Shared Function SaveTreeState(ByVal oPage As System.Web.UI.Page, ByVal IDPassport As Integer, ByVal idTreeState As String, ByVal value As String) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.SecurityV3State

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of Boolean) = VTLiveApi.SecurityV3Methods.SaveTreeState(oState, IDPassport, idTreeState, value)

                oSession.States.SecurityV3State = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.SecurityV3State.Result <> SecurityNodeResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.SecurityV3State)
                End If

                bolRet = wsRet.Value
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-591")
            End Try

            Return bolRet
        End Function

        Public Shared Function DeleteTreeState(ByVal oPage As System.Web.UI.Page, ByVal IDPassport As Integer, ByVal idTreeState As String) As Boolean
            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.SecurityV3State

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of Boolean) = VTLiveApi.SecurityV3Methods.DeleteTreeState(oState, IDPassport, idTreeState)

                oSession.States.SecurityV3State = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.SecurityV3State.Result <> SecurityNodeResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.SecurityV3State)
                End If

                bolRet = wsRet.Value
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-591")
            End Try

            Return bolRet
        End Function




        Public Shared Function GetUniversalSelector(ByVal oPage As System.Web.UI.Page, ByVal idPassport As Integer, ByVal idUniversalSelector As String) As String
            Dim bolRet As String = String.Empty

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.SecurityV3State

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of String) = VTLiveApi.SecurityV3Methods.GetUniversalSelector(oState, idPassport, idUniversalSelector)

                oSession.States.SecurityV3State = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.SecurityV3State.Result <> SecurityNodeResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.SecurityV3State)
                End If

                bolRet = wsRet.Value
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-591")
            End Try

            Return bolRet
        End Function

        Public Shared Function SaveUniversalSelector(ByVal oPage As System.Web.UI.Page, ByVal idPassport As Integer, ByVal idUniversalSelector As String, ByVal value As String) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.SecurityV3State

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of Boolean) = VTLiveApi.SecurityV3Methods.SaveUniversalSelector(oState, idPassport, idUniversalSelector, value)

                oSession.States.SecurityV3State = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.SecurityV3State.Result <> SecurityNodeResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.SecurityV3State)
                End If

                bolRet = wsRet.Value
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-591")
            End Try

            Return bolRet
        End Function

        Public Shared Function DeleteUniversalSelector(ByVal oPage As System.Web.UI.Page, ByVal idPassport As Integer, ByVal idUniversalSelector As String) As Boolean
            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.SecurityV3State

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of Boolean) = VTLiveApi.SecurityV3Methods.DeleteUniversalSelector(oState, idPassport, idUniversalSelector)

                oSession.States.SecurityV3State = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.SecurityV3State.Result <> SecurityNodeResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.SecurityV3State)
                End If

                bolRet = wsRet.Value
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-591")
            End Try

            Return bolRet
        End Function

    End Class

End Namespace