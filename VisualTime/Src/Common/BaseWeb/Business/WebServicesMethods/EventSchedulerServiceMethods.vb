Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.EventScheduler

Namespace API

    Public NotInheritable Class EventSchedulerMethods

        Public Shared Function CopyEvent(ByVal oPage As System.Web.UI.Page, ByVal IDEventSource As Integer, ByVal NewDate As Date, Optional ByVal NewName As String = "", Optional ByVal bAudit As Boolean = True) As roEventScheduler

            'Dim oRet As roEventScheduler = Nothing

            'Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            'Dim oState As roEventSchedulerState = oSession.States.EventSchedulerState

            'WebServiceHelper.SetState(oState, oPage)

            'Try

            '    oRet = VTLiveApi.EventsSchedulerMethods.CopyEvent(IDEventSource, NewName, NewDate, oState, bAudit)

            '    oSession.States.EventSchedulerState = oState
            '    roWsUserManagement.SessionObject = oSession

            '    If oState.Result <> ResultEnum.NoError Then
            '        HelperWeb.ShowError(oPage, oState)
            '    End If

            'Catch ex As Exception
            '    Dim oTmpState As New Robotics.Base.DTOs.roWsState
            '    oTmpState.Result = 1
            '    Dim oLanguage As New roLanguageWeb
            '    oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
            '    HelperWeb.ShowError(oPage, oTmpState, "9-BW01-348")
            'End Try

            'Return oRet
            Dim oRet As roEventScheduler = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EventSchedulerState

            WebServiceHelper.SetState(oState)

            Try
                Dim wsRet As roGenericVtResponse(Of roEventScheduler) = VTLiveApi.EventsSchedulerMethods.CopyEvent(IDEventSource, NewName, NewDate, oState, bAudit)

                oSession.States.EventSchedulerState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EventSchedulerState.Result <> EventSchedulerResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.EventSchedulerState)
                End If

                oRet = wsRet.Value
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-348")
            End Try

            Return oRet

        End Function

        Public Shared Function GetEventsScheduler(ByVal oPage As System.Web.UI.Page, ByVal bAudit As Boolean) As Generic.List(Of roEventScheduler)

            Dim oRet As Generic.List(Of roEventScheduler) = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EventSchedulerState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of List(Of roEventScheduler)) = VTLiveApi.EventsSchedulerMethods.GetEventsScheduler(oState, bAudit)

                oSession.States.EventSchedulerState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EventSchedulerState.Result = EventSchedulerResultEnum.NoError Then
                    oRet = wsRet.Value
                Else
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.EventSchedulerState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-349")
            End Try

            Return oRet

        End Function

        Public Shared Function GetEventsSchedulerByYear(ByVal oPage As System.Web.UI.Page, ByVal Year As Integer, ByVal bAudit As Boolean) As DataTable

            Dim oRet As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EventSchedulerState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of DataSet) = VTLiveApi.EventsSchedulerMethods.GetEventSchedulerByYear(Year, oState, bAudit)

                oSession.States.EventSchedulerState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EventSchedulerState.Result = EventSchedulerResultEnum.NoError Then
                    If wsRet.Value.Tables.Count > 0 Then
                        oRet = wsRet.Value.Tables(0)
                    End If
                Else
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.EventSchedulerState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-350")
            End Try

            Return oRet

        End Function

        Public Shared Function GetEventsSchedulerByName(ByVal oPage As System.Web.UI.Page, ByVal Name As String, ByVal bAudit As Boolean) As DataTable

            Dim oRet As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EventSchedulerState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of DataSet) = VTLiveApi.EventsSchedulerMethods.GetEventSchedulerByName(Name, oState, bAudit)

                oSession.States.EventSchedulerState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EventSchedulerState.Result = EventSchedulerResultEnum.NoError Then
                    If wsRet.Value.Tables.Count > 0 Then
                        oRet = wsRet.Value.Tables(0)
                    End If
                Else
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.EventSchedulerState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-351")
            End Try

            Return oRet

        End Function

        Public Shared Function GetEventScheduler(ByVal oPage As System.Web.UI.Page, ByVal _IDEventScheduler As Integer, ByVal bAudit As Boolean) As roEventScheduler

            Dim oRet As roEventScheduler = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EventSchedulerState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of roEventScheduler) = VTLiveApi.EventsSchedulerMethods.GetEventScheduler(_IDEventScheduler, oState, bAudit)

                oSession.States.EventSchedulerState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EventSchedulerState.Result <> EventSchedulerResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.EventSchedulerState)
                End If

                oRet = wsRet.Value
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-352")
            End Try

            Return oRet

        End Function

        Public Shared Function GetEventAuthorizations(ByVal oPage As System.Web.UI.Page, ByVal intIDEvent As Integer, Optional ByVal bolAudit As Boolean = False) As DataTable

            Dim oRet As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EventSchedulerState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of DataSet) = VTLiveApi.EventsSchedulerMethods.GetEventAuthorizations(intIDEvent, oState)

                oSession.States.EventSchedulerState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EventSchedulerState.Result = EventSchedulerResultEnum.NoError Then
                    If wsRet.Value.Tables.Count > 0 Then
                        oRet = wsRet.Value.Tables(0)
                    End If
                Else
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.EventSchedulerState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-353")
            End Try

            Return oRet

        End Function

        Public Shared Function SaveEventScheduler(ByVal oPage As System.Web.UI.Page, ByRef oEventScheduler As roEventScheduler, ByVal bAudit As Boolean) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EventSchedulerState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of roEventScheduler) = VTLiveApi.EventsSchedulerMethods.SaveEventScheduler(oEventScheduler, oState, bAudit)

                oSession.States.EventSchedulerState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EventSchedulerState.Result <> EventSchedulerResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.EventSchedulerState)
                End If

                If wsRet.Value IsNot Nothing Then
                    bolRet = True
                    oEventScheduler = wsRet.Value
                Else
                    bolRet = False
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-354")
            End Try

            Return bolRet

        End Function

        Public Shared Function DeleteEventScheduler(ByVal oPage As System.Web.UI.Page, ByVal oEventScheduler As roEventScheduler, ByVal bAudit As Boolean) As Boolean

            'Dim bolRet As Boolean = False

            'Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            'Dim oState As roEventSchedulerState = oSession.States.EventSchedulerState

            'WebServiceHelper.SetState(oState, oPage)

            'Try

            '    bolRet = VTLiveApi.EventsSchedulerMethods.DeleteEventScheduler(oEventScheduler, oState, bAudit)

            '    oSession.States.EventSchedulerState = oState
            '    roWsUserManagement.SessionObject = oSession

            '    If oState.Result <> ResultEnum.NoError Then
            '        ' Mostrar el error
            '        HelperWeb.ShowError(oPage, oState)
            '    End If

            'Catch ex As Exception
            '    Dim oTmpState As New Robotics.Base.DTOs.roWsState
            '    oTmpState.Result = 1
            '    Dim oLanguage As New roLanguageWeb
            '    oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
            '    HelperWeb.ShowError(oPage, oTmpState, "9-BW01-355")
            'End Try

            'Return bolRet
            Dim bolRet As Boolean = False
            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EventSchedulerState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of Boolean) = VTLiveApi.EventsSchedulerMethods.DeleteEventScheduler(oEventScheduler, oState, bAudit)

                oSession.States.EventSchedulerState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EventSchedulerState.Result <> EventSchedulerResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.EventSchedulerState)
                End If

                bolRet = wsRet.Value
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-355")
            End Try

            Return bolRet

        End Function

        Public Shared Function DeleteEventScheduler(ByVal oPage As System.Web.UI.Page, ByVal _IDEventScheduler As Integer, ByVal bAudit As Boolean) As Boolean
            Dim bolRet As Boolean = False
            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EventSchedulerState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of Boolean) = VTLiveApi.EventsSchedulerMethods.DeleteEventSchedulerByID(_IDEventScheduler, oState, bAudit)

                oSession.States.EventSchedulerState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EventSchedulerState.Result <> EventSchedulerResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.EventSchedulerState)
                End If

                bolRet = wsRet.Value
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-356")
            End Try

            Return bolRet

        End Function

        Public Shared Function LastErrorText() As String
            Dim strRet As String = ""
            If roWsUserManagement.SessionObject() IsNot Nothing Then
                strRet = roWsUserManagement.SessionObject().States.EventSchedulerState.ErrorText
            End If
            Return strRet
        End Function

    End Class

End Namespace