Imports System.Web.UI
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Incidence

Namespace API

    Public NotInheritable Class ProgrammedCausesServiceMethods

        Public Shared Function GetProgrammedCause(ByVal oPage As System.Web.UI.Page, ByVal IDEmployee As Integer, ByVal BeginDate As Date, ByVal IDAbsence As Integer, Optional ByVal bolAudit As Boolean = True) As roProgrammedCause
            Dim oProgrammedCause As roProgrammedCause = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.ProgrammedCauseState

            WebServiceHelper.SetState(oState)

            Try

                oState.IDPassport = WLHelperWeb.CurrentPassport.ID
                Dim wsRet As roGenericVtResponse(Of roProgrammedCause) = VTLiveApi.ProgrammedCausesMethods.GetProgrammedCause(IDEmployee, BeginDate, IDAbsence, oState, bolAudit)

                oProgrammedCause = wsRet.Value

                oSession.States.ProgrammedCauseState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.ProgrammedCauseState.Result <> ProgrammedCausesResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.ProgrammedCauseState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-449")
            End Try

            Return oProgrammedCause

        End Function

        Public Shared Function GetProgrammedCauses(ByVal oPage As PageBase, ByVal IDEmployee As Integer) As DataTable

            Dim oRet As DataSet = Nothing
            Dim oRetTable As DataTable = Nothing
            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.ProgrammedCauseState

            WebServiceHelper.SetState(oState)

            Try

                oState.IDPassport = WLHelperWeb.CurrentPassport.ID
                Dim wsRet As roGenericVtResponse(Of DataSet) = VTLiveApi.ProgrammedCausesMethods.GetProgrammedCauses(IDEmployee, oState)

                oRet = wsRet.Value

                oSession.States.ProgrammedCauseState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.ProgrammedCauseState.Result = ProgrammedCausesResultEnum.NoError Then
                    If oRet IsNot Nothing AndAlso oRet.Tables.Count > 0 Then
                        oRetTable = oRet.Tables(0)
                    End If
                Else
                    HelperWeb.ShowError(oPage, oSession.States.ProgrammedCauseState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-450")
            End Try

            Return oRetTable

        End Function

        Public Shared Function GetProgrammedCauses(ByVal oPage As PageBase, ByVal _IDEmployee As Integer, ByVal xBegin As DateTime, ByVal xEnd As DateTime) As DataTable

            Dim oRet As DataSet = Nothing
            Dim oRetTable As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.ProgrammedCauseState

            WebServiceHelper.SetState(oState)

            Try

                oState.IDPassport = WLHelperWeb.CurrentPassport.ID
                Dim wsRet As roGenericVtResponse(Of DataSet) = VTLiveApi.ProgrammedCausesMethods.GetProgrammedCauses(_IDEmployee, oState)

                oRet = wsRet.Value

                oSession.States.ProgrammedCauseState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.ProgrammedCauseState.Result = ProgrammedCausesResultEnum.NoError Then
                    If oRet IsNot Nothing AndAlso oRet.Tables.Count > 0 Then
                        oRetTable = oRet.Tables(0)
                    End If
                Else
                    HelperWeb.ShowError(oPage, oSession.States.ProgrammedCauseState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-451")
            End Try

            Return oRetTable

        End Function

        Public Shared Function DeleteProgrammedCause(ByVal oPage As PageBase, ByVal oProgrammedCause As roProgrammedCause, Optional ByVal bolAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.ProgrammedCauseState

            WebServiceHelper.SetState(oState)

            Try
                Dim wsRet As roGenericVtResponse(Of Boolean) = VTLiveApi.ProgrammedCausesMethods.DeleteProgrammedCause(oProgrammedCause, oState, bolAudit)

                bolRet = wsRet.Value

                oSession.States.ProgrammedCauseState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.ProgrammedCauseState.Result <> ProgrammedCausesResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.ProgrammedCauseState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-452")
            End Try

            Return bolRet

        End Function

        Public Shared Function SaveProgrammedCause(ByVal oPage As Page, ByRef _ProgrammedCause As roProgrammedCause, Optional ByVal bolAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.ProgrammedCauseState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of roProgrammedCause) = VTLiveApi.ProgrammedCausesMethods.SaveProgrammedCause(_ProgrammedCause, oState, bolAudit)

                oSession.States.ProgrammedCauseState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.ProgrammedCauseState.Result <> ProgrammedCausesResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.ProgrammedCauseState)
                End If

                If wsRet.Value IsNot Nothing Then
                    bolRet = True
                    _ProgrammedCause = wsRet.Value
                Else
                    bolRet = False
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-453")
            End Try

            Return bolRet

        End Function

        Public Shared Function LastErrorText() As String
            Dim strRet As String = ""
            If roWsUserManagement.SessionObject() IsNot Nothing Then
                strRet = roWsUserManagement.SessionObject().States.ProgrammedCauseState.ErrorText
            End If
            Return strRet
        End Function

    End Class

End Namespace