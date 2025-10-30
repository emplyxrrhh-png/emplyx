Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Absence

Namespace API

    Public NotInheritable Class ProgrammedAbsencesServiceMethods

        Public Shared Function GetProgrammedAbsence(ByVal oPage As System.Web.UI.Page, ByVal IDEmployee As Integer, ByVal BeginDate As Date, Optional ByVal bolAudit As Boolean = True) As roProgrammedAbsence
            Dim oProgrammedAbsence As roProgrammedAbsence = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.ProgrammedAbsenceState

            WebServiceHelper.SetState(oState)

            Try

                'oProgrammedAbsence = VTLiveApi.ProgrammedAbsencesMethods.GetProgrammedAbsence(IDEmployee, BeginDate, oState, bolAudit)

                'oSession.States.ProgrammedAbsenceState = oState
                'roWsUserManagement.SessionObject = oSession

                'If oSession.States.ProgrammedAbsenceState.Result <> ProgrammedAbsencesService.ResultEnum.NoError Then
                '    ' Mostrar el error
                '    HelperWeb.ShowError(oPage, oSession.States.ProgrammedAbsenceState)
                'End If

                oState.IDPassport = WLHelperWeb.CurrentPassport.ID
                Dim wsRet As roGenericVtResponse(Of roProgrammedAbsence) = VTLiveApi.ProgrammedAbsencesMethods.GetProgrammedAbsence(IDEmployee, BeginDate, oState, bolAudit)

                oProgrammedAbsence = wsRet.Value

                oSession.States.ProgrammedAbsenceState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.ProgrammedAbsenceState.Result <> ProgrammedAbsencesResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.ProgrammedAbsenceState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-443")
            End Try

            Return oProgrammedAbsence

        End Function

        Public Shared Function GetProgrammedAbsences(ByVal oPage As System.Web.UI.Page, ByVal IDEmployee As Integer) As DataTable

            Dim oRet As DataSet = Nothing
            Dim oRetTable As DataTable = Nothing
            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.ProgrammedAbsenceState

            WebServiceHelper.SetState(oState)

            Try

                'Dim ds As DataSet = VTLiveApi.ProgrammedAbsencesMethods.GetProgrammedAbsences(IDEmployee, oState)

                'oSession.States.ProgrammedAbsenceState = oState
                'roWsUserManagement.SessionObject = oSession

                'If oSession.States.ProgrammedAbsenceState.Result = ProgrammedAbsencesService.ResultEnum.NoError Then
                '    If ds IsNot Nothing AndAlso ds.Tables.Count > 0 Then
                '        oRet = ds.Tables(0)
                '    End If
                'Else
                '    HelperWeb.ShowError(oPage, oSession.States.ProgrammedAbsenceState)
                'End If

                oState.IDPassport = WLHelperWeb.CurrentPassport.ID
                Dim wsRet As roGenericVtResponse(Of DataSet) = VTLiveApi.ProgrammedAbsencesMethods.GetProgrammedAbsences(IDEmployee, oState)

                oRet = wsRet.Value

                oSession.States.ProgrammedAbsenceState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.ProgrammedAbsenceState.Result = ProgrammedAbsencesResultEnum.NoError Then
                    If oRet IsNot Nothing AndAlso oRet.Tables.Count > 0 Then
                        oRetTable = oRet.Tables(0)
                    End If
                Else
                    HelperWeb.ShowError(oPage, oSession.States.ProgrammedAbsenceState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-444")
            End Try

            Return oRetTable

        End Function

        Public Shared Function GetProgrammedAbsencesInPeriod(ByVal oPage As System.Web.UI.Page, ByVal IDEmployee As Integer, ByVal _BeginDate As Date, ByVal _EndDate As Date) As DataTable

            Dim oRet As DataSet = Nothing
            Dim oRetTable As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.ProgrammedAbsenceState

            WebServiceHelper.SetState(oState)

            Try

                'Dim ds As DataSet = VTLiveApi.ProgrammedAbsencesMethods.GetProgrammedAbsencesInPeriod(IDEmployee, _BeginDate, _EndDate, oState)

                'oSession.States.ProgrammedAbsenceState = oState
                'roWsUserManagement.SessionObject = oSession

                'If oSession.States.ProgrammedAbsenceState.Result = ProgrammedAbsencesService.ResultEnum.NoError Then
                '    If ds IsNot Nothing AndAlso ds.Tables.Count > 0 Then
                '        oRet = ds.Tables(0)
                '    End If
                'Else
                '    HelperWeb.ShowError(oPage, oSession.States.ProgrammedAbsenceState)
                'End If

                oState.IDPassport = WLHelperWeb.CurrentPassport.ID
                Dim wsRet As roGenericVtResponse(Of DataSet) = VTLiveApi.ProgrammedAbsencesMethods.GetProgrammedAbsencesInPeriod(IDEmployee, _BeginDate, _EndDate, oState)

                oRet = wsRet.Value

                oSession.States.ProgrammedAbsenceState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.ProgrammedAbsenceState.Result = ProgrammedAbsencesResultEnum.NoError Then
                    If oRet IsNot Nothing AndAlso oRet.Tables.Count > 0 Then
                        oRetTable = oRet.Tables(0)
                    End If
                Else
                    HelperWeb.ShowError(oPage, oSession.States.ProgrammedAbsenceState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-445")
            End Try

            Return oRetTable

        End Function

        Public Shared Function GetProgrammedCauses(ByVal oPage As System.Web.UI.Page, ByVal _IDEmployee As Integer, ByVal xBegin As DateTime, ByVal xEnd As DateTime) As DataTable
            Dim oRet As DataSet = Nothing
            Dim oRetTable As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.ProgrammedAbsenceState

            WebServiceHelper.SetState(oState)

            Try

                'Dim ds As DataSet = VTLiveApi.ProgrammedAbsencesMethods.GetProgrammedCauses(_IDEmployee, xBegin, xEnd, oState)

                'oSession.States.ProgrammedAbsenceState = oState
                'roWsUserManagement.SessionObject = oSession

                'If oSession.States.ProgrammedAbsenceState.Result = ProgrammedAbsencesService.ResultEnum.NoError Then
                '    If ds IsNot Nothing AndAlso ds.Tables.Count > 0 Then
                '        tb = ds.Tables(0)
                '    End If
                'Else
                '    ' Mostrar el error
                '    HelperWeb.ShowError(oPage, oSession.States.ProgrammedAbsenceState)
                'End If

                oState.IDPassport = WLHelperWeb.CurrentPassport.ID
                Dim wsRet As roGenericVtResponse(Of DataSet) = VTLiveApi.ProgrammedAbsencesMethods.GetProgrammedCauses(_IDEmployee, xBegin, xEnd, oState)

                oRet = wsRet.Value

                oSession.States.ProgrammedAbsenceState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.ProgrammedAbsenceState.Result = ProgrammedAbsencesResultEnum.NoError Then
                    If oRet IsNot Nothing AndAlso oRet.Tables.Count > 0 Then
                        oRetTable = oRet.Tables(0)
                    End If
                Else
                    HelperWeb.ShowError(oPage, oSession.States.ProgrammedAbsenceState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-446")
            End Try

            Return oRetTable

        End Function

        Public Shared Function DeleteProgrammedAbsence(ByVal oPage As System.Web.UI.Page, ByVal oProgrammedAbsence As roProgrammedAbsence, Optional ByVal bolAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.ProgrammedAbsenceState

            WebServiceHelper.SetState(oState)

            Try

                'bolRet = VTLiveApi.ProgrammedAbsencesMethods.DeleteProgrammedAbsence(oProgrammedAbsence, oState, bolAudit)

                'oSession.States.ProgrammedAbsenceState = oState
                'roWsUserManagement.SessionObject = oSession

                'If oSession.States.ProgrammedAbsenceState.Result <> ProgrammedAbsencesService.ResultEnum.NoError Then
                '    HelperWeb.ShowError(oPage, oSession.States.ProgrammedAbsenceState)
                'End If
                Dim wsRet As roGenericVtResponse(Of Boolean) = VTLiveApi.ProgrammedAbsencesMethods.DeleteProgrammedAbsence(oProgrammedAbsence, oState, bolAudit)

                bolRet = wsRet.Value

                oSession.States.ProgrammedAbsenceState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.ProgrammedAbsenceState.Result <> ProgrammedAbsencesResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.ProgrammedAbsenceState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-447")
            End Try
            Return bolRet

        End Function

        Public Shared Function SaveProgrammedAbsence(ByVal oPage As System.Web.UI.Page, ByVal _ProgrammedAbsence As roProgrammedAbsence, Optional ByVal bolAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.ProgrammedAbsenceState

            WebServiceHelper.SetState(oState)

            Try

                'bolRet = VTLiveApi.ProgrammedAbsencesMethods.SaveProgrammedAbsence(_ProgrammedAbsence, oState, bolAudit)

                'oSession.States.ProgrammedAbsenceState = oState
                'roWsUserManagement.SessionObject = oSession

                'If oSession.States.ProgrammedAbsenceState.Result <> ResultEnum.NoError Then
                '    ' Mostrar el error
                '    HelperWeb.ShowError(oPage, oSession.States.ProgrammedAbsenceState)
                'End If

                Dim wsRet As roGenericVtResponse(Of roProgrammedAbsence) = VTLiveApi.ProgrammedAbsencesMethods.SaveProgrammedAbsence(_ProgrammedAbsence, oState, bolAudit)

                oSession.States.ProgrammedAbsenceState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.ProgrammedAbsenceState.Result <> ProgrammedAbsencesResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.ProgrammedAbsenceState)
                End If

                If wsRet.Value IsNot Nothing Then
                    bolRet = True
                Else
                    bolRet = False
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-448")
            End Try

            Return bolRet

        End Function

        Public Shared Function LastErrorText() As String
            Dim strRet As String = ""
            If roWsUserManagement.SessionObject() IsNot Nothing Then
                strRet = roWsUserManagement.SessionObject().States.ProgrammedAbsenceState.ErrorText
            End If
            Return strRet
        End Function

    End Class

End Namespace