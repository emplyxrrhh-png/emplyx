Imports System.Web.UI
Imports Robotics.Base.DTOs

Namespace API

    Public NotInheritable Class ProgrammedHolidaysServiceMethods

#Region "Prevision de vacaciones por horas"

        Public Shared Function GetProgrammedHolidayById(ByVal oPage As Page, ByVal idProgrammedHoliday As Long, ByVal bAudit As Boolean) As roProgrammedHoliday
            Dim oRet As roProgrammedHolidayResponse = Nothing
            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.ProgrammedHolidayState
            WebServiceHelper.SetState(oState)
            Try
                oRet = VTLiveApi.ProgrammedHolidaysMethods.GetProgrammedHolidayById(idProgrammedHoliday, bAudit, oState)
                oSession.States.ProgrammedHolidayState = oRet.oState
                roWsUserManagement.SessionObject = oSession
                If oSession.States.ProgrammedHolidayState.Result <> HolidayResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.ProgrammedHolidayState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-454")
            End Try

            Return oRet.ProgrammedHoliday
        End Function

        Public Shared Function GetProgrammedHolidaysList(ByVal idEmployee As Integer, ByVal strWhere As String, oPage As Page, bAudit As Boolean) As List(Of roProgrammedHoliday)
            Dim oRet As roProgrammedHolidayListResponse = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.ProgrammedHolidayState
            WebServiceHelper.SetState(oState)

            Try
                oRet = VTLiveApi.ProgrammedHolidaysMethods.GetProgrammedHolidays(idEmployee, strWhere, oState)
                oSession.States.ProgrammedHolidayState = oRet.oState
                roWsUserManagement.SessionObject = oSession

                If oSession.States.ProgrammedHolidayState.Result <> HolidayResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.ProgrammedHolidayState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-455")
            End Try

            Return oRet.ProgrammedHolidays.ToList
        End Function

        Public Shared Function SaveProgrammedHoliday(oProgrammedHoliday As roProgrammedHoliday, ByVal oPage As Page, ByVal bAudit As Boolean) As roProgrammedHoliday
            Dim oRet As roProgrammedHolidayResponse = Nothing
            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.ProgrammedHolidayState
            WebServiceHelper.SetState(oState)
            Try
                oRet = VTLiveApi.ProgrammedHolidaysMethods.SaveProgrammedHoliday(oProgrammedHoliday, bAudit, oState)
                oSession.States.ProgrammedHolidayState = oRet.oState
                roWsUserManagement.SessionObject = oSession
                If oSession.States.ProgrammedHolidayState.Result <> HolidayResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.ProgrammedHolidayState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-456")
            End Try

            Return oRet.ProgrammedHoliday
        End Function

        Public Shared Function DeleteProgrammedHoliday(oProgrammedHoliday As roProgrammedHoliday, oPage As Page, bAudit As Boolean) As Boolean
            Dim oRet As roProgrammedHolidayStandarResponse = Nothing
            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.ProgrammedHolidayState
            WebServiceHelper.SetState(oState)
            Try
                oRet = VTLiveApi.ProgrammedHolidaysMethods.DeleteProgrammedHoliday(oProgrammedHoliday, bAudit, oState)
                oSession.States.ProgrammedHolidayState = oRet.oState
                roWsUserManagement.SessionObject = oSession
                If oSession.States.ProgrammedHolidayState.Result <> HolidayResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.ProgrammedHolidayState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-457")
            End Try

            Return oRet.Result
        End Function

#End Region

        Public Shared Function LastErrorText() As String
            Dim strRet As String = ""
            If roWsUserManagement.SessionObject() IsNot Nothing Then
                strRet = roWsUserManagement.SessionObject().States.ProgrammedHolidayState.ErrorText
            End If
            Return strRet
        End Function

        Public Shared Function LastResult() As HolidayResultEnum
            Dim strRet As String = ""
            If roWsUserManagement.SessionObject() IsNot Nothing Then
                strRet = roWsUserManagement.SessionObject().States.ProgrammedHolidayState.Result
            End If
            Return strRet
        End Function

    End Class

End Namespace