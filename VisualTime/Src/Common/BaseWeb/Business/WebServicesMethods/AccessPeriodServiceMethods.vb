Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.AccessPeriod

Namespace API

    Public NotInheritable Class AccessPeriodServiceMethods

#Region "AccessPeriods"

        Public Shared Function GetAccessPeriods(ByVal oPage As System.Web.UI.Page) As DataTable
            Dim oRet As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.AccessMoveState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of DataSet) = VTLiveApi.AccessPeriodMethods.GetAccessPeriodsDataSet(oState)

                oSession.States.AccessPeriodState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oState.Result = AccessPeriodResultEnum.NoError Then
                    If wsRet.Value.Tables.Count > 0 Then
                        oRet = wsRet.Value.Tables(0)
                    End If
                Else
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.AccessPeriodState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-027")
            End Try

            Return oRet

        End Function

        Public Shared Function GetAccessPeriodByID(ByVal oPage As System.Web.UI.Page, ByVal intID As Integer, Optional ByVal bolAudit As Boolean = False) As roAccessPeriod
            Dim oRet As roAccessPeriod = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.AccessPeriodState

            WebServiceHelper.SetState(oState)

            Try
                Dim wsRet As roGenericVtResponse(Of roAccessPeriod) = VTLiveApi.AccessPeriodMethods.GetAccessPeriodByID(intID, oState, bolAudit)

                oSession.States.AccessPeriodState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.AccessPeriodState.Result <> AccessPeriodResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.AccessPeriodState)
                End If

                oRet = wsRet.Value
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-028")
            End Try

            Return oRet

        End Function

        Public Shared Function SaveAccessPeriod(ByVal oPage As System.Web.UI.Page, ByRef oAccessPeriod As roAccessPeriod, Optional ByVal bolAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.AccessPeriodState

            WebServiceHelper.SetState(oState)

            Try
                Dim wsRet As roGenericVtResponse(Of roAccessPeriod) = VTLiveApi.AccessPeriodMethods.SaveAccessPeriod(oAccessPeriod, oState, bolAudit)

                oSession.States.AccessPeriodState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.AccessPeriodState.Result <> AccessPeriodResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.AccessPeriodState)
                End If

                If wsRet.Value IsNot Nothing Then
                    bolRet = True
                    oAccessPeriod = wsRet.Value
                Else
                    bolRet = False
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-029")
            End Try

            Return bolRet

        End Function

        Public Shared Function DeleteAccessPeriod(ByVal oPage As System.Web.UI.Page, ByVal intID As Integer, Optional ByVal bolAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.AccessPeriodState

            WebServiceHelper.SetState(oState)

            Try
                Dim wsRet As roGenericVtResponse(Of Boolean) = VTLiveApi.AccessPeriodMethods.DeleteAccessPeriodByID(intID, oState, bolAudit)

                oSession.States.AccessPeriodState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.AccessPeriodState.Result <> AccessPeriodResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.AccessPeriodState)
                End If

                bolRet = wsRet.Value
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-030")
            End Try

            Return bolRet

        End Function

        Public Shared Function GetAccessPeriodDailyDescription(ByVal oPage As System.Web.UI.Page, ByRef oAccessPeriodDaily As roAccessPeriodDaily) As String

            Dim bolRet As String = ""

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.AccessPeriodState

            WebServiceHelper.SetState(oState)

            Try
                Dim wsRet As roGenericVtResponse(Of String) = VTLiveApi.AccessPeriodMethods.getAccessPeriodDailyDescription(oAccessPeriodDaily, oState)

                oSession.States.AccessPeriodState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.AccessPeriodState.Result <> AccessPeriodResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.AccessPeriodState)
                End If

                bolRet = wsRet.Value
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-031")
            End Try

            Return bolRet

        End Function

        Public Shared Function GetAccessPeriodHolidaysDescription(ByVal oPage As System.Web.UI.Page, ByRef oAccessPeriodHolidays As roAccessPeriodHolidays) As String

            Dim bolRet As String = ""

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.AccessPeriodState

            WebServiceHelper.SetState(oState)

            Try
                Dim wsRet As roGenericVtResponse(Of String) = VTLiveApi.AccessPeriodMethods.getAccessPeriodHolidaysDescription(oAccessPeriodHolidays, oState)

                oSession.States.AccessPeriodState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.AccessPeriodState.Result <> AccessPeriodResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.AccessPeriodState)
                End If

                bolRet = wsRet.Value
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-032")
            End Try

            Return bolRet

        End Function

#End Region

        Public Shared Function LastErrorText() As String
            Dim strRet As String = ""
            If roWsUserManagement.SessionObject() IsNot Nothing Then
                strRet = roWsUserManagement.SessionObject().States.AccessPeriodState.ErrorText
            End If
            Return strRet
        End Function

    End Class

End Namespace