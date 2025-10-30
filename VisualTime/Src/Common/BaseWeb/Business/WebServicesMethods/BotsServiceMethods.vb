Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBots

Namespace API

    Public NotInheritable Class BotsServiceMethods

        Public Shared Function GetAllBots(ByVal oPage As PageBase) As roBot()
            Dim oRet As roBot() = {}
            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roBotState = oSession.States.BotState

            WebServiceHelper.SetState(oState)

            Try
                Dim oBotsManager As New roBotManager(oState)
                Dim oBots As Generic.List(Of roBot) = oBotsManager.GetAllBots()

                oSession.States.BotState = oBotsManager.State
                roWsUserManagement.SessionObject = oSession

                If oSession.States.BotState.Result = BotResultEnum.NoError Then
                    oRet = oBots.ToArray
                Else
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.BotState)
                End If
            Catch ex As Exception
                Dim oTmpState As New roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-064")
            End Try

            Return oRet
        End Function

        Public Shared Function GetBot(ByVal idBot As Integer, ByVal oPage As PageBase, Optional ByVal bAudit As Boolean = False) As roBot
            Dim oRet As roBot = Nothing
            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roBotState = oSession.States.BotState

            WebServiceHelper.SetState(oState)

            Try
                Dim oBotsManager As New roBotManager(oState)
                oRet = oBotsManager.GetBotById(idBot, bAudit)

                oSession.States.BotState = oBotsManager.State
                roWsUserManagement.SessionObject = oSession

                If oSession.States.BotState.Result <> BotResultEnum.NoError Then
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.BotState)
                End If
            Catch ex As Exception
                Dim oTmpState As New roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-064")
            End Try

            Return oRet
        End Function

        Public Shared Function CreateOrUpdateBot(ByVal oPage As PageBase, ByRef oBot As roBot, Optional ByVal bAudit As Boolean = False) As Boolean
            Dim oRet As Boolean = False
            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roBotState = oSession.States.BotState

            WebServiceHelper.SetState(oState)

            Try
                Dim oBotsManager As New roBotManager(oState)
                oRet = oBotsManager.CreateOrUpdateBot(oBot, bAudit)

                oSession.States.BotState = oBotsManager.State
                roWsUserManagement.SessionObject = oSession

                If oSession.States.BotState.Result <> BotResultEnum.NoError Then
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.BotState)
                End If
            Catch ex As Exception
                Dim oTmpState As New roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-064")
            End Try

            Return oRet
        End Function

        Public Shared Function DeleteBot(oBot As roBot, ByVal oPage As PageBase, Optional ByVal bAudit As Boolean = False) As Boolean
            Dim oRet As Boolean = False
            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roBotState = oSession.States.BotState

            WebServiceHelper.SetState(oState)

            Try
                Dim oBotsManager As New roBotManager(oState)
                oRet = oBotsManager.DeleteBot(oBot, bAudit)

                oSession.States.BotState = oBotsManager.State
                roWsUserManagement.SessionObject = oSession

                If oSession.States.BotState.Result <> BotResultEnum.NoError Then
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.BotState)
                End If
            Catch ex As Exception
                Dim oTmpState As New roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-091")
            End Try

            Return oRet
        End Function

        Public Shared Function GetAvailableRulesByType(eBotType As BotTypeEnum, ByVal oPage As PageBase, Optional ByVal bAudit As Boolean = False) As Generic.List(Of BotRuleTypeEnum)
            Dim oRet As New List(Of BotRuleTypeEnum)()
            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roBotState = oSession.States.BotState

            WebServiceHelper.SetState(oState)

            Try
                Dim oBotsManager As New roBotManager(oState)
                oRet = oBotsManager.GetRulesByBotType(eBotType)

                oSession.States.BotState = oBotsManager.State
                roWsUserManagement.SessionObject = oSession

                If oSession.States.BotState.Result = BotResultEnum.NoError Then
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.BotState)
                End If
            Catch ex As Exception
                Dim oTmpState As New roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-091")
            End Try

            Return oRet
        End Function

#Region "Last errors"

        Public Shared Function LastBotErrorText() As String
            Dim strRet As String = ""
            If roWsUserManagement.SessionObject() IsNot Nothing Then
                strRet = roWsUserManagement.SessionObject().States.BotState.ErrorText
            End If
            Return strRet
        End Function

        Public Shared Function LastBotResult() As BotResultEnum
            Dim strRet As String = ""
            If roWsUserManagement.SessionObject() IsNot Nothing Then
                strRet = roWsUserManagement.SessionObject().States.BotState.Result
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