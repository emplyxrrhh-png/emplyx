Imports System.Web.UI
Imports Robotics.Base.DTOs

Namespace API

    Public NotInheritable Class ProgrammedOvertimesServiceMethods

#Region "Prevision de horas de exceso"

        Public Shared Function GetProgrammedOvertimeById(ByVal oPage As Page, ByVal idProgrammedOvertime As Long, ByVal bAudit As Boolean) As roProgrammedOvertime
            Dim oRet As roProgrammedOvertimeResponse = Nothing
            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.ProgrammedOvertimeState
            WebServiceHelper.SetState(oState)
            Try
                oRet = VTLiveApi.ProgrammedOvertimesMethods.GetProgrammedOvertimeById(idProgrammedOvertime, bAudit, oState)
                oSession.States.ProgrammedOvertimeState = oRet.oState
                roWsUserManagement.SessionObject = oSession
                If oSession.States.ProgrammedOvertimeState.Result <> HolidayResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.ProgrammedOvertimeState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-458")
            End Try
            Return oRet.ProgrammedOvertime
        End Function

        Public Shared Function GetProgrammedOvertimesList(ByVal idEmployee As Integer, ByVal strWhere As String, oPage As Page, bAudit As Boolean) As List(Of roProgrammedOvertime)
            Dim oRet As roProgrammedOvertimeListResponse = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.ProgrammedOvertimeState
            WebServiceHelper.SetState(oState)

            Try
                oRet = VTLiveApi.ProgrammedOvertimesMethods.GetProgrammedOvertimes(idEmployee, strWhere, oState)
                oSession.States.ProgrammedOvertimeState = oRet.oState
                roWsUserManagement.SessionObject = oSession

                If oSession.States.ProgrammedOvertimeState.Result <> HolidayResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.ProgrammedOvertimeState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-459")
            End Try

            Return oRet.ProgrammedOvertimes.ToList
        End Function

        Public Shared Function SaveProgrammedOvertime(oProgrammedOvertime As roProgrammedOvertime, ByVal oPage As Page, ByVal bAudit As Boolean) As roProgrammedOvertime
            Dim oRet As roProgrammedOvertimeResponse = Nothing
            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.ProgrammedOvertimeState
            WebServiceHelper.SetState(oState)
            Try
                oRet = VTLiveApi.ProgrammedOvertimesMethods.SaveProgrammedOvertime(oProgrammedOvertime, bAudit, oState)
                oSession.States.ProgrammedOvertimeState = oRet.oState
                roWsUserManagement.SessionObject = oSession
                If oSession.States.ProgrammedOvertimeState.Result <> HolidayResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.ProgrammedOvertimeState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-460")
            End Try
            Return oRet.ProgrammedOvertime
        End Function

        Public Shared Function DeleteProgrammedOvertime(oProgrammedOvertime As roProgrammedOvertime, oPage As Page, bAudit As Boolean) As Boolean
            Dim oRet As roProgrammedOvertimeStandarResponse = Nothing
            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.ProgrammedOvertimeState
            WebServiceHelper.SetState(oState)
            Try
                oRet = VTLiveApi.ProgrammedOvertimesMethods.DeleteProgrammedOvertime(oProgrammedOvertime, bAudit, oState)
                oSession.States.ProgrammedOvertimeState = oRet.oState
                roWsUserManagement.SessionObject = oSession
                If oSession.States.ProgrammedOvertimeState.Result <> HolidayResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.ProgrammedOvertimeState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-461")
            End Try
            Return oRet.Result
        End Function

#End Region

        Public Shared Function LastErrorText() As String
            Dim strRet As String = ""
            If roWsUserManagement.SessionObject() IsNot Nothing Then
                strRet = roWsUserManagement.SessionObject().States.ProgrammedOvertimeState.ErrorText
            End If
            Return strRet
        End Function

        Public Shared Function LastResult() As HolidayResultEnum
            Dim strRet As String = ""
            If roWsUserManagement.SessionObject() IsNot Nothing Then
                strRet = roWsUserManagement.SessionObject().States.ProgrammedOvertimeState.Result
            End If
            Return strRet
        End Function

    End Class

End Namespace