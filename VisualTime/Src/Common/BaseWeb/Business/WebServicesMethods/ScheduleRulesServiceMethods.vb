Imports Robotics.Base.DTOs

Namespace API

    Public NotInheritable Class ScheduleRulesServiceMethods

#Region "Productive Units"

        Public Shared Function GetLabAgreeScheduleRules(ByVal oPage As System.Web.UI.Page, ByVal IdLabAgree As Integer) As roScheduleRule()
            Dim oRet As roScheduleRulesResponse

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.ScheduleRulesState
            WebServiceHelper.SetState(oState)

            Try
                oRet = VTLiveApi.ScheduleRulesMethods.GetLabAgreeScheduleRules(IdLabAgree, oState)
                oSession.States.ScheduleRulesState = oRet.oState
                roWsUserManagement.SessionObject = oSession

                If oSession.States.ScheduleRulesState.Result <> ScheduleRulesResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.ScheduleRulesState)
                End If
            Catch ex As Exception
                oRet = New roScheduleRulesResponse()
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-563")
            End Try

            Return oRet.Rules

        End Function

        Public Shared Function GetContractScheduleRules(ByVal oPage As System.Web.UI.Page, ByVal IdContract As String) As roScheduleRule()
            Dim oRet As roScheduleRulesResponse

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.ScheduleRulesState
            WebServiceHelper.SetState(oState)

            Try
                oRet = VTLiveApi.ScheduleRulesMethods.GetContractScheduleRules(IdContract, oState)
                oSession.States.ScheduleRulesState = oRet.oState
                roWsUserManagement.SessionObject = oSession

                If oSession.States.ScheduleRulesState.Result <> ScheduleRulesResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.ScheduleRulesState)
                End If
            Catch ex As Exception
                oRet = New roScheduleRulesResponse()
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-564")
            End Try

            Return oRet.Rules

        End Function

        Public Shared Function GetEmployeeCurrentScheduleRules(ByVal oPage As System.Web.UI.Page, ByVal IdContract As String) As roScheduleRule()
            Dim oRet As roScheduleRulesResponse

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.ScheduleRulesState
            WebServiceHelper.SetState(oState)

            Try
                oRet = VTLiveApi.ScheduleRulesMethods.GetEmployeeCurrentScheduleRules(IdContract, oState)
                oSession.States.ScheduleRulesState = oRet.oState
                roWsUserManagement.SessionObject = oSession

                If oSession.States.ScheduleRulesState.Result <> ScheduleRulesResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.ScheduleRulesState)
                End If
            Catch ex As Exception
                oRet = New roScheduleRulesResponse()
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-565")
            End Try

            Return oRet.Rules

        End Function

        Public Shared Function SaveLabAgreeScheduleRules(ByVal oPage As System.Web.UI.Page, ByVal oRules As roScheduleRule(), ByVal IdLabAgree As Integer) As Boolean
            Dim oRet As roScheduleRulesStdResponse

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.ScheduleRulesState
            WebServiceHelper.SetState(oState)

            Try
                oRet = VTLiveApi.ScheduleRulesMethods.SaveLabAgreeScheduleRules(IdLabAgree, oRules, oState)
                oSession.States.ScheduleRulesState = oRet.oState
                roWsUserManagement.SessionObject = oSession

                If oSession.States.ScheduleRulesState.Result <> ScheduleRulesResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.ScheduleRulesState)
                End If
            Catch ex As Exception
                oRet = New roScheduleRulesStdResponse()
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-566")
            End Try

            Return oRet.Result

        End Function

        Public Shared Function SaveContractScheduleRules(ByVal oPage As System.Web.UI.Page, ByVal oRules As roScheduleRule(), ByVal IdContract As String) As Boolean
            Dim oRet As roScheduleRulesStdResponse

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.ScheduleRulesState
            WebServiceHelper.SetState(oState)

            Try
                oRet = VTLiveApi.ScheduleRulesMethods.SaveContractScheduleRules(IdContract, oRules, oState)
                oSession.States.ScheduleRulesState = oRet.oState
                roWsUserManagement.SessionObject = oSession

                If oSession.States.ScheduleRulesState.Result <> ScheduleRulesResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.ScheduleRulesState)
                End If
            Catch ex As Exception
                oRet = New roScheduleRulesStdResponse()
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-567")
            End Try

            Return oRet.Result

        End Function

        Public Shared Function GetUserScheduleRulesTypes(ByVal oPage As System.Web.UI.Page) As Integer()
            Dim oRet As roScheduleRulesTypesResponse

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.ScheduleRulesState
            WebServiceHelper.SetState(oState)

            Try
                oRet = VTLiveApi.ScheduleRulesMethods.GetUserScheduleRulesTypes(oState)
                oSession.States.ScheduleRulesState = oRet.oState
                roWsUserManagement.SessionObject = oSession

                If oSession.States.ScheduleRulesState.Result <> ScheduleRulesResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.ScheduleRulesState)
                End If
            Catch ex As Exception
                oRet = New roScheduleRulesTypesResponse()
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-568")
            End Try

            Return oRet.Rules
        End Function

#End Region

#Region "Last errors"

        Public Shared Function LastErrorText() As String
            Dim strRet As String = ""
            If roWsUserManagement.SessionObject() IsNot Nothing Then
                strRet = roWsUserManagement.SessionObject().States.ScheduleRulesState.ErrorText
            End If
            Return strRet
        End Function

        Public Shared Function LastResult() As ProductiveUnitResultEnum
            Dim strRet As String = ""
            If roWsUserManagement.SessionObject() IsNot Nothing Then
                strRet = roWsUserManagement.SessionObject().States.ScheduleRulesState.Result
            End If
            Return strRet
        End Function

#End Region

    End Class

End Namespace