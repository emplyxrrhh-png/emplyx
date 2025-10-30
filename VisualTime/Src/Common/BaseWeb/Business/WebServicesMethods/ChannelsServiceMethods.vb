Imports Robotics.Base.DTOs
Imports Robotics.Base.VTChannels

Namespace API

    Public NotInheritable Class ChannelsServiceMethods

        Public Shared Function GetAllChannels(ByVal oPage As PageBase) As roChannel()
            Dim oRet As roChannel() = {}
            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roChannelState = oSession.States.ChannelState

            WebServiceHelper.SetState(oState)

            Try
                Dim oChannelsManager As New roChannelManager(oState)
                Dim oChannels As Generic.List(Of roChannel) = oChannelsManager.GetAllChannels()

                oSession.States.ChannelState = oChannelsManager.State
                roWsUserManagement.SessionObject = oSession

                If oSession.States.ChannelState.Result = ChannelResultEnum.NoError Then
                    oRet = oChannels.ToArray
                Else
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.ChannelState)
                End If
            Catch ex As Exception
                Dim oTmpState As New roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") & System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-063")
            End Try

            Return oRet
        End Function

        Public Shared Function GetChannel(ByVal idChannel As Integer, ByVal oPage As PageBase, Optional ByVal bAudit As Boolean = False) As roChannel
            Dim oRet As roChannel = Nothing
            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roChannelState = oSession.States.ChannelState

            WebServiceHelper.SetState(oState)

            Try
                Dim oChannelsManager As New roChannelManager(oState)
                oRet = oChannelsManager.GetChannel(idChannel, bAudit)

                oSession.States.ChannelState = oChannelsManager.State
                roWsUserManagement.SessionObject = oSession

                If oSession.States.ChannelState.Result <> ChannelResultEnum.NoError Then
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.ChannelState)
                End If
            Catch ex As Exception
                Dim oTmpState As New roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") & System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-063")
            End Try

            Return oRet
        End Function

        Public Shared Function GetComplaintChannel(ByVal oPage As PageBase, Optional ByVal bAudit As Boolean = False) As roChannel
            Dim oRet As roChannel = Nothing
            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roChannelState = oSession.States.ChannelState

            WebServiceHelper.SetState(oState)

            Try
                Dim oChannelsManager As New roChannelManager(oState)
                oRet = oChannelsManager.GetComplaintChannel(bAudit, False, -1)

                oSession.States.ChannelState = oChannelsManager.State
                roWsUserManagement.SessionObject = oSession

                If oSession.States.ChannelState.Result <> ChannelResultEnum.NoError Then
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.ChannelState)
                End If
            Catch ex As Exception
                Dim oTmpState As New roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") & System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-063")
            End Try

            Return oRet
        End Function

        Public Shared Function UpdateChannelStatus(ByVal oPage As PageBase, ByRef oChannel As roChannel, Optional ByVal bAudit As Boolean = False) As Boolean
            Dim oRet As Boolean = False
            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roChannelState = oSession.States.ChannelState

            WebServiceHelper.SetState(oState)

            Try
                Dim oChannelsManager As New roChannelManager(oState)
                oRet = oChannelsManager.CreateOrUpdateChannel(oChannel, True)

                oSession.States.ChannelState = oChannelsManager.State
                roWsUserManagement.SessionObject = oSession

                If oSession.States.ChannelState.Result <> ChannelResultEnum.NoError Then
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.ChannelState)
                End If
            Catch ex As Exception
                Dim oTmpState As New roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") & System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-063")
            End Try

            Return oRet
        End Function

        Public Shared Function CreateOrUpdateChannel(ByVal oPage As PageBase, ByRef oChannel As roChannel, Optional ByVal bAudit As Boolean = False) As Boolean
            Dim oRet As Boolean = False
            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roChannelState = oSession.States.ChannelState

            WebServiceHelper.SetState(oState)

            Try
                Dim oChannelsManager As New roChannelManager(oState)
                oRet = oChannelsManager.CreateOrUpdateChannel(oChannel, True)

                oSession.States.ChannelState = oChannelsManager.State
                roWsUserManagement.SessionObject = oSession

                If oSession.States.ChannelState.Result <> ChannelResultEnum.NoError Then
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.ChannelState)
                End If
            Catch ex As Exception
                Dim oTmpState As New roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") & System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-063")
            End Try

            Return oRet
        End Function

        Public Shared Function CreateMessage(ByVal oPage As PageBase, ByRef oMessage As roMessage, Optional ByVal bAudit As Boolean = False) As Boolean
            Dim oRet As Boolean = False
            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roMessageState = oSession.States.MessageState

            WebServiceHelper.SetState(oState)

            Try
                Dim oMessageManager As New roMessageManager(New roMessageState(oState.IDPassport))
                oRet = oMessageManager.CreateMessage(oMessage, bAudit)

                oSession.States.MessageState = oMessageManager.State
                roWsUserManagement.SessionObject = oSession

                If oSession.States.MessageState.Result <> MessageResultEnum.NoError Then
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.MessageState)
                End If
            Catch ex As Exception
                Dim oTmpState As New roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") & System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-063")
            End Try

            Return oRet
        End Function

        Public Shared Function CreateConversation(ByVal oPage As PageBase, ByRef oMessage As roMessage, Optional ByVal bAudit As Boolean = False) As Boolean
            Dim oRet As Boolean = False
            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roConversationState = oSession.States.ConversationState

            WebServiceHelper.SetState(oState)

            Try
                Dim oConversationManager As New roConversationManager(oState)
                oRet = oConversationManager.CreateConversation(oMessage.Conversation, oMessage, bAudit)

                oSession.States.ConversationState = oConversationManager.State
                roWsUserManagement.SessionObject = oSession

                If oSession.States.ConversationState.Result <> ConversationResultEnum.NoError Then
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.ConversationState)
                End If
            Catch ex As Exception
                Dim oTmpState As New roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") & System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-063")
            End Try

            Return oRet
        End Function

        Public Shared Function ChangeConversationState(ByVal oPage As PageBase, ByRef oConversation As roConversation, Optional ByVal bAudit As Boolean = False) As Boolean
            Dim oRet As Boolean = False
            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roConversationState = oSession.States.ConversationState

            WebServiceHelper.SetState(oState)

            Try
                Dim oConversationManager As New roConversationManager(oState)
                oRet = oConversationManager.ChangeConversationState(oConversation, False)

                oSession.States.ConversationState = oConversationManager.State
                roWsUserManagement.SessionObject = oSession

                If oSession.States.ConversationState.Result <> ConversationResultEnum.NoError Then
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.ConversationState)
                End If
            Catch ex As Exception
                Dim oTmpState As New roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-063")
            End Try

            Return oRet
        End Function

        Public Shared Function ChangeComplaintComplexity(ByVal oPage As PageBase, ByRef oConversation As roConversation, Optional ByVal bAudit As Boolean = False) As Boolean
            Dim oRet As Boolean = False
            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roConversationState = oSession.States.ConversationState

            WebServiceHelper.SetState(oState)

            Try
                Dim oConversationManager As New roConversationManager(oState)
                oRet = oConversationManager.ChangeComplaintComplexity(oConversation, bAudit)

                oSession.States.ConversationState = oConversationManager.State
                roWsUserManagement.SessionObject = oSession

                If oSession.States.ConversationState.Result <> ConversationResultEnum.NoError Then
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.ConversationState)
                End If
            Catch ex As Exception
                Dim oTmpState As New roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") & System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-063")
            End Try

            Return oRet
        End Function

        Public Shared Function DeleteChannel(oChannel As roChannel, ByVal oPage As PageBase, Optional ByVal bAudit As Boolean = False) As Boolean
            Dim oRet As Boolean = False
            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roChannelState = oSession.States.ChannelState

            WebServiceHelper.SetState(oState)

            Try
                Dim oChannelsManager As New roChannelManager(oState)
                oRet = oChannelsManager.DeleteChannel(oChannel, bAudit)

                oSession.States.ChannelState = oChannelsManager.State
                roWsUserManagement.SessionObject = oSession

                If oSession.States.ChannelState.Result <> ChannelResultEnum.NoError Then
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.ChannelState)
                End If
            Catch ex As Exception
                Dim oTmpState As New roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") & System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-063")
            End Try

            Return oRet
        End Function

        Public Shared Function GetChannelConversations(ByVal idChannel As Integer, ByVal oPage As PageBase, Optional ByVal bAudit As Boolean = False) As List(Of roConversation)
            Dim oRet As List(Of roConversation) = Nothing
            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roConversationState = oSession.States.ConversationState

            WebServiceHelper.SetState(oState)

            Try
                Dim oConversationsManager As New roConversationManager(oState)
                oRet = oConversationsManager.GetAllChannelConversations(idChannel, 0)

                oSession.States.ConversationState = oConversationsManager.State
                roWsUserManagement.SessionObject = oSession

                If oSession.States.ConversationState.Result <> ConversationResultEnum.NoError Then
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.ConversationState)
                End If
            Catch ex As Exception
                Dim oTmpState As New roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") & System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-063")
            End Try

            Return oRet
        End Function

        Public Shared Function GetConversationById(ByVal idConversation As Integer, ByVal oPage As PageBase, Optional ByVal viewerIdUser As Integer = 0, Optional ByVal bAudit As Boolean = False) As roConversation
            Dim oRet As roConversation = Nothing
            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roConversationState = oSession.States.ConversationState

            WebServiceHelper.SetState(oState)

            Try
                Dim oConversationsManager As New roConversationManager(New roConversationState(oState.IDPassport))
                oRet = oConversationsManager.GetConversation(idConversation, viewerIdUser)

                oSession.States.ConversationState = oConversationsManager.State
                roWsUserManagement.SessionObject = oSession

                If oSession.States.ConversationState.Result <> ConversationResultEnum.NoError Then
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.ConversationState)
                End If
            Catch ex As Exception
                Dim oTmpState As New roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") & System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-063")
            End Try

            Return oRet
        End Function

        Public Shared Function GetConversationMessages(ByVal idConversation As Integer, ByVal oPage As PageBase, Optional ByVal bAudit As Boolean = False) As List(Of roMessage)
            Dim oRet As List(Of roMessage) = Nothing
            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roMessageState = oSession.States.MessageState

            WebServiceHelper.SetState(oState)

            Try
                Dim oMessageManager As New roMessageManager(oState)
                oRet = oMessageManager.GetAllConversationMessages(idConversation, 0, bAudit)

                oSession.States.MessageState = oMessageManager.State
                roWsUserManagement.SessionObject = oSession

                If oSession.States.MessageState.Result <> MessageResultEnum.NoError Then
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.MessageState)
                End If
            Catch ex As Exception
                Dim oTmpState As New roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") & System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-063")
            End Try

            Return oRet
        End Function

        Public Shared Function GetAllComplaintMessages(ByVal idComplaint As String, ByVal password As String, ByVal viewerIdUser As Integer, ByVal oPage As PageBase, Optional ByVal bAudit As Boolean = False) As List(Of roMessage)
            Dim oRet As List(Of roMessage) = Nothing
            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roMessageState = oSession.States.MessageState

            WebServiceHelper.SetState(oState)

            Try
                Dim oMessageManager As New roMessageManager(New roMessageState(oState.IDPassport))
                oRet = oMessageManager.GetAllComplaintMessages(idComplaint, password, viewerIdUser, bAudit)

                oSession.States.MessageState = oMessageManager.State
                roWsUserManagement.SessionObject = oSession

                If oSession.States.MessageState.Result <> MessageResultEnum.NoError Then
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.MessageState)
                End If
            Catch ex As Exception
                Dim oTmpState As New roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") & System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-063")
            End Try

            Return oRet
        End Function

        Public Shared Function ReloadComplaintMessages(ByVal idComplaint As String, ByVal dexAuthCookie As String, ByVal viewerIdUser As Integer, ByVal oPage As PageBase, Optional ByVal bAudit As Boolean = False) As List(Of roMessage)
            Dim oRet As List(Of roMessage) = Nothing
            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roMessageState = oSession.States.MessageState

            WebServiceHelper.SetState(oState)

            Try
                Dim oMessageManager As New roMessageManager(New roMessageState(oState.IDPassport))
                oRet = oMessageManager.ReloadComplaintMessages(idComplaint, dexAuthCookie, viewerIdUser, bAudit)

                oSession.States.MessageState = oMessageManager.State
                roWsUserManagement.SessionObject = oSession

                If oSession.States.MessageState.Result <> MessageResultEnum.NoError Then
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.MessageState)
                End If
            Catch ex As Exception
                Dim oTmpState As New roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") & System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-063")
            End Try

            Return oRet
        End Function

        Public Shared Function SetConversationMessagesReaded(ByVal idConversation As Integer, ByVal oPage As PageBase) As Boolean
            Dim oRet As Boolean
            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roMessageState = oSession.States.MessageState

            WebServiceHelper.SetState(oState)

            Try
                Dim oMessageManager As New roMessageManager(oState)
                oRet = oMessageManager.SetAllConversationMessagesRead(idConversation, 0)

                oSession.States.MessageState = oMessageManager.State
                roWsUserManagement.SessionObject = oSession

                If oSession.States.MessageState.Result <> MessageResultEnum.NoError Then
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.MessageState)
                End If
            Catch ex As Exception
                Dim oTmpState As New roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") & System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-063")
            End Try

            Return oRet
        End Function

#Region "Last errors"

        Public Shared Function LastChannelErrorText() As String
            Dim strRet As String = ""
            If roWsUserManagement.SessionObject() IsNot Nothing Then
                strRet = roWsUserManagement.SessionObject().States.ChannelState.ErrorText
            End If
            Return strRet
        End Function

        Public Shared Function LastChannelResult() As ChannelResultEnum
            Dim strRet As String = ""
            If roWsUserManagement.SessionObject() IsNot Nothing Then
                strRet = roWsUserManagement.SessionObject().States.ChannelState.Result
            End If
            Return strRet
        End Function

        Public Shared Function LastConversationErrorText() As String
            Dim strRet As String = ""
            If roWsUserManagement.SessionObject() IsNot Nothing Then
                strRet = roWsUserManagement.SessionObject().States.ConversationState.ErrorText
            End If
            Return strRet
        End Function

        Public Shared Function LastConversationResult() As ConversationResultEnum
            Dim strRet As String = ""
            If roWsUserManagement.SessionObject() IsNot Nothing Then
                strRet = roWsUserManagement.SessionObject().States.ConversationState.Result
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