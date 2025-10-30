Imports Robotics.Base.DTOs

Namespace API

    Public NotInheritable Class SecurityChartServiceMethods

#Region "SecurityNode"

        Public Shared Function GetGroupFeatures(ByVal oPage As System.Web.UI.Page) As roGroupFeature()
            Dim bolRet As roGroupFeature() = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.GroupFeatureState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of roGroupFeature()) = VTLiveApi.SecurityChartMethods.GetGroupFeatures(oState)

                oSession.States.GroupFeatureState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.GroupFeatureState.Result <> SecurityNodeResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.GroupFeatureState)
                End If

                bolRet = wsRet.Value
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-582")
            End Try

            Return bolRet

        End Function

        Public Shared Function GetConsultantGroupFeature(ByVal oPage As System.Web.UI.Page) As roGroupFeature
            Dim bolRet As roGroupFeature = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.GroupFeatureState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of roGroupFeature) = VTLiveApi.SecurityChartMethods.GetConsultantGroupFeature(oState)

                oSession.States.GroupFeatureState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.GroupFeatureState.Result <> SecurityNodeResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.GroupFeatureState)
                End If

                bolRet = wsRet.Value
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-582")
            End Try

            Return bolRet

        End Function

        Public Shared Function GetRoboticsGroupFeaturesList(ByVal oPage As System.Web.UI.Page) As roGroupFeature()
            Dim bolRet As roGroupFeature() = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.GroupFeatureState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of roGroupFeature()) = VTLiveApi.SecurityChartMethods.GetRoboticsGroupFeaturesList(oState)

                oSession.States.GroupFeatureState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.GroupFeatureState.Result <> SecurityNodeResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.GroupFeatureState)
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




        Public Shared Function GetGroupFeatureByID(ByVal iIDFeatureGroup As Integer, ByVal oPage As System.Web.UI.Page) As roGroupFeature
            Dim bolRet As roGroupFeature = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.GroupFeatureState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of roGroupFeature) = VTLiveApi.SecurityChartMethods.GetGroupFeaturesById(iIDFeatureGroup, oState)

                oSession.States.GroupFeatureState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.GroupFeatureState.Result <> GroupFeatureResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.GroupFeatureState)
                End If

                bolRet = wsRet.Value
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-583")
            End Try

            Return bolRet

        End Function

        Public Shared Function DeleteGroupFeature(ByVal oGroupFeature As roGroupFeature, ByVal oPage As System.Web.UI.Page) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.GroupFeatureState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of Boolean) = VTLiveApi.SecurityChartMethods.DeleteGroupFeature(oGroupFeature, oState)

                oSession.States.GroupFeatureState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.GroupFeatureState.Result <> GroupFeatureResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.GroupFeatureState)
                End If

                bolRet = wsRet.Value
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-584")
            End Try

            Return bolRet

        End Function

        Public Shared Function SaveGroupFeature(ByRef oGroupFeature As roGroupFeature, ByVal oPage As System.Web.UI.Page) As Boolean
            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.GroupFeatureState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of roGroupFeature) = VTLiveApi.SecurityChartMethods.SaveGroupFeatures(oGroupFeature, oState)

                oSession.States.GroupFeatureState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.GroupFeatureState.Result <> GroupFeatureResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.GroupFeatureState)
                End If

                If wsRet.Value IsNot Nothing Then
                    bolRet = True
                    oGroupFeature = wsRet.Value
                Else
                    bolRet = False
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-585")
            End Try

            Return bolRet

        End Function

        Public Shared Function SetGroupFeaturePermission(ByVal iGroupFeatureID As Integer, ByVal iFeatureID As Integer, ByVal iPermission As Integer, ByVal oPage As System.Web.UI.Page) As Boolean
            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.GroupFeatureState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of Boolean) = VTLiveApi.SecurityChartMethods.SetGroupFeaturePermission(iGroupFeatureID, iFeatureID, iPermission, oState)

                oSession.States.GroupFeatureState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.GroupFeatureState.Result <> GroupFeatureResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.GroupFeatureState)
                End If

                bolRet = wsRet.Value
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-586")
            End Try

            Return bolRet

        End Function

        Public Shared Function CopyGroupFeature(ByVal oPage As System.Web.UI.Page, ByVal iGroupFeatureID As Integer) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.GroupFeatureState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of Boolean) = VTLiveApi.SecurityChartMethods.CopyGroupFeature(iGroupFeatureID, oState)

                oSession.States.GroupFeatureState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.GroupFeatureState.Result <> GroupFeatureResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.GroupFeatureState)
                End If

                bolRet = wsRet.Value
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-590")
            End Try

            Return bolRet
        End Function

        Public Shared Function GetAllExternalIds(ByVal oPage As System.Web.UI.Page) As DataTable

            Dim oRet As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.GroupFeatureState

            WebServiceHelper.SetState(oState)

            Try
                Dim wsRet As roGenericVtResponse(Of DataSet) = VTLiveApi.SecurityChartMethods.GetAllExternalIds(oState)

                oSession.States.GroupFeatureState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.GroupFeatureState.Result = ShiftResultEnum.NoError Then
                    If wsRet.Value.Tables.Count > 0 Then
                        oRet = wsRet.Value.Tables(0)
                    End If
                Else
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.GroupFeatureState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                Dim oLanguage As New roLanguageWeb
                oTmpState.Result = 1
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-592")
            End Try

            Return oRet

        End Function

#End Region

        Public Shared Function LastErrorText() As String
            Dim strRet As String = ""
            If roWsUserManagement.SessionObject() IsNot Nothing Then
                strRet = roWsUserManagement.SessionObject().States.GroupFeatureState.ErrorText
            End If
            Return strRet
        End Function

    End Class

End Namespace