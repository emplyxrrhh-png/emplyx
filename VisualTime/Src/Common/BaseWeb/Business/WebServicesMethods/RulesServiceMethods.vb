Imports Robotics.Base.DTOs
Imports Robotics.Base.VTRules
Imports Robotics.Base.VTSelectorManager
Imports Robotics.Base.VTUserFields.UserFields

Namespace API

    Public NotInheritable Class RulesServiceMethods

        Public Shared Function GetDummyRulesGroups(ByVal oPage As PageBase, Optional audit As Boolean = False) As roRulesGroup()
            Dim oRet As roRulesGroup() = {}
            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roRulesState = oSession.States.RulesState

            WebServiceHelper.SetState(oState)

            Try
                Dim rulesManager As New roRulesManager(oState)
                Dim ruleGroups As Generic.List(Of roRulesGroup) = rulesManager.GetDummyRulesGroups

                oSession.States.RulesState = rulesManager.State
                roWsUserManagement.SessionObject = oSession

                If oSession.States.RulesState.Result = RulesResult.NoError Then
                    oRet = ruleGroups.ToArray
                Else
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.RulesState)
                End If
            Catch ex As Exception
                Dim oTmpState As New roWsState With {.Result = 1}
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") & System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-064")
            End Try

            Return oRet
        End Function


#Region "Last errors"

        Public Shared Function LastRulesErrorText() As String
            Dim strRet As String = ""
            If roWsUserManagement.SessionObject() IsNot Nothing Then
                strRet = roWsUserManagement.SessionObject().States.RulesState.ErrorText
            End If
            Return strRet
        End Function

        Public Shared Function LastRulesResult() As RulesResult
            Dim strRet As String = ""
            If roWsUserManagement.SessionObject() IsNot Nothing Then
                strRet = roWsUserManagement.SessionObject().States.RulesState.Result
            End If
            Return strRet
        End Function

        Public Shared Function LastMessageErrorText() As String
            Dim strRet As String = ""
            If roWsUserManagement.SessionObject() IsNot Nothing Then
                strRet = roWsUserManagement.SessionObject().States.MessageState.ErrorText
            End If
            Return strRet
        End Function

        Public Shared Function LastMessageResult() As MessageResultEnum
            Dim strRet As String = ""
            If roWsUserManagement.SessionObject() IsNot Nothing Then
                strRet = roWsUserManagement.SessionObject().States.MessageState.Result
            End If
            Return strRet
        End Function

#End Region

    End Class

End Namespace