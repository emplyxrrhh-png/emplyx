Imports Robotics.Base.DTOs
Imports Robotics.Base.VTCommuniques
Imports Robotics.Portal.Business
Imports Robotics.VTBase.Extensions

Namespace API

    Public NotInheritable Class PortalServiceMethods

        Public Shared Function GetMainMenu(ByVal oPage As System.Web.UI.Page, ByVal strAppName As String, ByVal intIDPassport As Integer, ByVal FeatureType As FeatureTypes, ByVal oLicense As roVTLicense, ByVal strLanguageReference As String) As wscMenuElementList

            Dim oRet As wscMenuElementList = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.PortalBaseState

            WebServiceHelper.SetState(oState)

            Try

                If WLHelperWeb.MainMenu Is Nothing Then
                    oRet = VTLiveApi.PortalBaseMethods.GetMainMenu(strAppName, intIDPassport, FeatureType, strLanguageReference, oLicense, oState).Value

                    'Quitamos del menú Comunicados según licencia / registros en BD (pbi: 1190266)

                    Dim disabledElement As wscMenuElement = Nothing
                    Dim parentRow As wscMenuElement = Nothing

                    For Each row As wscMenuElement In oRet.List
                        For Each child As wscMenuElement In row.Childs.List
                            If child.Path = "Portal\Communications\Communiques" Then
                                Dim oServerLicense As New roServerLicense
                                Dim oCommunicateManager As New roCommuniqueManager(New roCommuniqueState(intIDPassport))
                                If oServerLicense.FeatureIsInstalled("Feature\AdvancedCommuniques") = False AndAlso oCommunicateManager.GetAllCommuniques().Count = 0 Then
                                    disabledElement = child
                                    parentRow = row
                                End If
                            End If
                        Next
                    Next
                    If (parentRow IsNot Nothing AndAlso disabledElement IsNot Nothing) Then
                        parentRow.Childs.List.Remove(disabledElement)
                        If parentRow.Childs.List IsNot Nothing AndAlso parentRow.Childs.List.Count = 0 Then
                            oRet.List.Remove(parentRow)
                        End If
                    End If

                    oSession.States.PortalBaseState = oState
                    roWsUserManagement.SessionObject = oSession

                    If oSession.States.PortalBaseState.Result <> GuiStateResultEnum.NoError Then
                        ' Mostrar el error
                        HelperWeb.ShowError(oPage, oSession.States.PortalBaseState)
                    Else
                        WLHelperWeb.MainMenu = oRet
                    End If
                Else
                    Return WLHelperWeb.MainMenu
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-433")
            End Try

            Return oRet

        End Function

        Public Shared Function GetServerLicense(ByVal oPage As System.Web.UI.Page) As roVTLicense

            Dim oRet As roVTLicense = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.PortalBaseState

            WebServiceHelper.SetState(oState)

            Try

                'If WLHelperWeb.MainMenu Is Nothing Then
                oRet = VTLiveApi.PortalBaseMethods.GetServerLicencse(oState).Value

                oSession.States.PortalBaseState = oState
                roWsUserManagement.SessionObject = oSession

                If oSession.States.PortalBaseState.Result <> GuiStateResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.PortalBaseState)
                End If
                'Else
                'Return oRet
                'End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-434")
            End Try

            Return oRet

        End Function

        Public Shared Function GetActiveEmployeesCount(ByVal oPage As System.Web.UI.Page, ByVal xDateTime As DateTime) As Integer

            Dim intRet As Integer = 0

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.PortalBaseState

            WebServiceHelper.SetState(oState)

            Try

                intRet = VTLiveApi.PortalBaseMethods.GetActiveEmployeesCount(xDateTime, oState).Value

                oSession.States.PortalBaseState = oState
                roWsUserManagement.SessionObject = oSession

                If oSession.States.PortalBaseState.Result <> GuiStateResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.PortalBaseState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-435")
            End Try

            Return intRet

        End Function

        Public Shared Function GetActiveJobEmployeesCount(ByVal oPage As System.Web.UI.Page, ByVal xDateTime As DateTime) As Integer

            Dim intRet As Integer = 0

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.PortalBaseState

            WebServiceHelper.SetState(oState)

            Try

                intRet = VTLiveApi.PortalBaseMethods.GetActiveJobEmployeesCount(xDateTime, oState).Value

                oSession.States.PortalBaseState = oState
                roWsUserManagement.SessionObject = oSession

                If oSession.States.PortalBaseState.Result <> GuiStateResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.PortalBaseState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-436")
            End Try

            Return intRet

        End Function

        Public Shared Function CheckLicenseLimits(ByVal oPage As System.Web.UI.Page, ByVal xDateTime As DateTime, ByVal oLicenseService As roVTLicense) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.PortalBaseState

            WebServiceHelper.SetState(oState)

            Try
                If oLicenseService IsNot Nothing Then
                    bolRet = VTLiveApi.PortalBaseMethods.CheckLicenseLimits(xDateTime, oState, oLicenseService).Value
                Else
                    bolRet = True
                End If

                oSession.States.PortalBaseState = oState
                roWsUserManagement.SessionObject = oSession

                If oSession.States.PortalBaseState.Result <> GuiStateResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.PortalBaseState)
                End If
            Catch ex As Exception
                bolRet = True
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-437")
            End Try

            Return bolRet

        End Function

        Public Shared Function GetGuiActions(ByVal oPage As System.Web.UI.Page, ByVal strIDGui As String, ByVal intIDPassport As Integer) As Generic.List(Of roGuiAction)

            Dim oRet As Generic.List(Of roGuiAction) = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.PortalBaseState

            WebServiceHelper.SetState(oState)

            Try

                Dim tmpActions() As roGuiAction = VTLiveApi.PortalBaseMethods.GetPathActions(strIDGui, intIDPassport, oState).Value.ToArray

                oSession.States.PortalBaseState = oState
                roWsUserManagement.SessionObject = oSession

                If oSession.States.PortalBaseState.Result <> GuiStateResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.PortalBaseState)
                Else
                    oRet = New Generic.List(Of roGuiAction)
                    oRet.AddRange(tmpActions)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-438")
            End Try

            Return oRet

        End Function

        Public Shared Function LastErrorText() As String
            Dim strRet As String = ""
            If roWsUserManagement.SessionObject() IsNot Nothing Then
                strRet = roWsUserManagement.SessionObject().States.PortalBaseState.ErrorText
            End If
            Return strRet
        End Function

    End Class

End Namespace