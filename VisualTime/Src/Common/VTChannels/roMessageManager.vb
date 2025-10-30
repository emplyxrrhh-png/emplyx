Imports System.Data.Common
Imports Robotics.Azure
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.DataLayer
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.Security
Imports Robotics.Security.Base
Imports Robotics.VTBase

Namespace VTChannels

    Public Class roMessageManager

        Private oState As roMessageState

        Public ReadOnly Property State As roMessageState
            Get
                Return oState
            End Get
        End Property

#Region "Constructores"

        Public Sub New()
            oState = New roMessageState()
        End Sub

        Public Sub New(ByVal _State As roMessageState)
            oState = _State
        End Sub

#End Region

#Region "Methods"

        ''' <summary>
        ''' Recupera toda la información de un mensaje
        ''' </summary>
        ''' <param name="idMessage"></param>
        ''' <param name="bAudit"></param>
        ''' <returns></returns>
        Public Function GetMessage(idMessaje As Integer, Optional viewerIdUser As Integer = 0, Optional ByVal bAudit As Boolean = False, Optional decryptKey As String = "") As roMessage
            Dim retMessage As roMessage = Nothing
            Dim strSQL As String

            Try

                oState.Result = MessageResultEnum.NoError

                ' Cargamos información de canales para validación de permisos.
                Dim oChannelManager As roChannelManager = New roChannelManager(New roChannelState(oState.IDPassport))
                Dim lAllowedChannelsId As List(Of Integer) = New List(Of Integer)
                lAllowedChannelsId = oChannelManager.GetAllowedChannelsId(viewerIdUser, oState.IDPassport)

                Dim tb As DataTable
                strSQL = $"@SELECT# [Id],[IdConversation],[Body],[IdEmployee],[IdSupervisor],[CreatedOn],[Status] FROM [dbo].[ChannelConversationMessages] WHERE Id = {idMessaje}"

                tb = CreateDataTable(strSQL)

                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    Dim oRow As DataRow = tb.Rows(0)
                    Dim oConversationManager As roConversationManager = New roConversationManager(New roConversationState(oState.IDPassport))

                    retMessage = New roMessage
                    retMessage.Conversation = oConversationManager.GetConversation(roTypes.Any2Integer(oRow("IdConversation")), viewerIdUser)
                    If retMessage.Conversation Is Nothing OrElse Not lAllowedChannelsId.Contains(retMessage.Conversation.Channel.Id) Then
                        oState.Result = MessageResultEnum.NoPermission
                        Return Nothing
                    End If
                    retMessage.Id = roTypes.Any2Integer(oRow("Id"))
                    Dim sBody As String = roTypes.Any2String(oRow("Body"))
                    If retMessage.Conversation.Channel.IsComplaintChannel Then
                        If decryptKey = String.Empty Then
                            decryptKey = RoAzureSupport.GetCompanySecret(roCacheManager.GetInstance().GetCompanyGUID(RoAzureSupport.GetCompanyName()))
                        End If
                        sBody = CryptographyHelper.DecryptEx(sBody, decryptKey)
                    End If
                    retMessage.Body = sBody
                    retMessage.IsAnonymous = retMessage.Conversation.IsAnonymous
                    Dim messageSupervisorId As Integer = roTypes.Any2Integer(oRow("IdSupervisor"))
                    Dim messageEmployeeId As Integer = roTypes.Any2Integer(oRow("IdEmployee"))
                    If messageSupervisorId > 0 Then
                        retMessage.IsResponse = True
                        retMessage.CreatedBy = messageSupervisorId
                        retMessage.CreatedByName = GetSupervisorName(messageSupervisorId)
                    Else
                        retMessage.IsResponse = False
                        retMessage.CreatedBy = messageEmployeeId
                        Dim createdByName As String = String.Empty
                        createdByName = GetEmployeeOrExternName(messageEmployeeId, retMessage.Conversation)
                        retMessage.CreatedByName = createdByName
                        If retMessage.Conversation.IsAnonymous Then
                            retMessage.CreatedBy = -1
                        End If
                    End If

                    retMessage.CreatedOn = roTypes.Any2DateTime(oRow("CreatedOn"))
                    retMessage.Status = roTypes.Any2Integer(oRow("Status"))
                End If

                If (oState.Result = MessageResultEnum.NoError) AndAlso bAudit Then
                    ' Auditamos
                    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    Me.oState.Audit(Audit.Action.aSelect, Audit.ObjectType.tChannelConversationMessage, If(retMessage IsNot Nothing, retMessage.Conversation.Title, ""), tbParameters, -1)
                End If
            Catch ex As DbException
                oState.Result = MessageResultEnum.ErrorRecoveringMessage
                Me.oState.UpdateStateInfo(ex, "roMessageManager::GetMessage")
            Catch ex As Exception
                oState.Result = MessageResultEnum.ErrorRecoveringMessage
                Me.oState.UpdateStateInfo(ex, "roMessageManager::GetMessage")
            End Try
            Return retMessage
        End Function

        Private Function GetSupervisorName(supervisorId As Integer) As String
            Dim oPassport As roPassportTicket = roPassportManager.GetPassportTicket(supervisorId)
            If oPassport IsNot Nothing AndAlso oPassport.Name.Length > 0 Then
                Return oPassport.Name
            Else
                Return oState.Language.Translate("Channel.Conversation.Message.Unknown", "")
            End If
        End Function

        Private Function GetEmployeeOrExternName(messageEmployeeId As Integer, conversation As roConversation) As String
            If conversation.IsAnonymous OrElse messageEmployeeId < 1 Then
                If conversation.ExtraData IsNot Nothing AndAlso conversation.ExtraData.FullName IsNot Nothing AndAlso conversation.ExtraData.FullName.Trim <> String.Empty Then
                    Return conversation.ExtraData.FullName
                End If
                Return oState.Language.Translate("Channel.Conversation.Message.Anonymous", "")
            Else
                Dim employeeName As String = roBusinessSupport.GetEmployeeName(messageEmployeeId, oState)
                Return IIf(employeeName.Length > 0, employeeName, oState.Language.Translate("Channel.Conversation.Message.Unknown", ""))
            End If
        End Function

        ''' <summary>
        ''' Recupera todos los mensajes de una conversación a los que tiene acceso un supervisor o empleado
        ''' </summary>
        ''' <param name="idConversation"></param>
        ''' <param name="bAudit"></param>
        ''' <returns></returns>
        Public Function GetAllConversationMessages(ByVal idConversation As Integer, Optional viewerIdUser As Integer = 0, Optional ByVal bAudit As Boolean = False) As List(Of roMessage)

            Dim retMessages As New List(Of roMessage)
            Dim sql As String = String.Empty

            Try
                oState.Result = MessageResultEnum.NoError

                ' Cargamos información de canales para validación de permisos.
                Dim oChannelManager As roChannelManager = New roChannelManager(New roChannelState(oState.IDPassport))
                Dim lAllowedChannelsId As List(Of Integer) = New List(Of Integer)
                lAllowedChannelsId = oChannelManager.GetAllowedChannelsId(viewerIdUser, oState.IDPassport)

                Dim oConversationManager As roConversationManager = New roConversationManager(New roConversationState(oState.IDPassport))
                Dim oConversation As roConversation = oConversationManager.GetConversation(idConversation, viewerIdUser)

                If oConversation Is Nothing OrElse Not lAllowedChannelsId.Contains(oConversation.Channel.Id) Then
                    oState.Result = ConversationResultEnum.NoPermission
                    Return retMessages
                End If

                Dim tb As DataTable
                sql = $"@SELECT# Id FROM ChannelConversationMessages WHERE IdConversation = {idConversation} ORDER BY CreatedOn ASC"

                tb = CreateDataTable(sql)

                Dim oMessage As roMessage

                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    Dim decryptKey As String = String.Empty
                    If oConversation.Channel.IsComplaintChannel Then
                        decryptKey = RoAzureSupport.GetCompanySecret(roCacheManager.GetInstance().GetCompanyGUID(RoAzureSupport.GetCompanyName()))
                    End If
                    For Each oRow As DataRow In tb.Rows
                        oMessage = GetMessage(oRow("Id"), viewerIdUser, False, decryptKey)
                        If oMessage IsNot Nothing Then retMessages.Add(oMessage)
                    Next
                End If

                If (oState.Result = ConversationResultEnum.NoError) AndAlso bAudit Then
                    ' Auditamos
                    Dim conversationType As String
                    If oConversation.Channel.IsComplaintChannel Then
                        conversationType = oState.Language.Translate($"Conversation.Type.Complaint", "")
                    Else
                        conversationType = oState.Language.Translate($"Conversation.Type.Conversation", "")
                    End If
                    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    Extensions.roAudit.AddParameter(tbParameters, "{ConversationType}", conversationType, String.Empty, 1)
                    Extensions.roAudit.AddParameter(tbParameters, "{ConversationReference}", oConversation.ReferenceNumber, String.Empty, 1)
                    Me.oState.Audit(Audit.Action.aSelect, Audit.ObjectType.tChannelConversation, oConversation.ReferenceNumber, tbParameters, -1)
                End If
            Catch ex As DbException
                oState.Result = ChannelResultEnum.ErrorRecoveringAllChannels
                Me.oState.UpdateStateInfo(ex, "roMessageManager::GetAllConversationMessages")
            Catch ex As Exception
                oState.Result = ChannelResultEnum.ErrorRecoveringAllChannels
                Me.oState.UpdateStateInfo(ex, "roMessageManager::GetAllConversationMessages")
            End Try
            Return retMessages
        End Function

        Public Function GetAllComplaintMessages(ByVal complaintReference As String, ByVal password As String, ByVal viewerIdUser As Integer, Optional ByVal bAudit As Boolean = False) As List(Of roMessage)

            Dim retMessages As New List(Of roMessage)

            Try
                oState.Result = MessageResultEnum.NoError

                Dim strSQL As String = String.Empty
                Dim hashPassword As String = HashCheckSum.CalculateString(password.Trim, Algorithm.SHA256)

                strSQL = $"@SELECT# [Id] FROM [dbo].[ChannelConversations] WHERE ReferenceNumber = '{complaintReference}' AND Password  ='{hashPassword}'"
                Dim idConversation = roTypes.Any2Integer(ExecuteScalar(strSQL))

                If idConversation > 0 Then
                    retMessages = GetAllConversationMessages(idConversation, viewerIdUser, bAudit)
                End If
            Catch ex As DbException
                oState.Result = ChannelResultEnum.ErrorRecoveringAllChannels
                Me.oState.UpdateStateInfo(ex, "roMessageManager::GetAllComplaintMessages")
            Catch ex As Exception
                oState.Result = ChannelResultEnum.ErrorRecoveringAllChannels
                Me.oState.UpdateStateInfo(ex, "roMessageManager::GetAllComplaintMessages")
            Finally

            End Try
            Return retMessages
        End Function

        Public Function ReloadComplaintMessages(ByVal complaintReference As String, ByVal dexAuthCookie As String, ByVal viewerIdUser As Integer, Optional ByVal bAudit As Boolean = False) As List(Of roMessage)

            Dim retMessages As New List(Of roMessage)

            Try
                oState.Result = MessageResultEnum.NoError

                Dim strSQL As String = String.Empty

                If dexAuthCookie = Robotics.VTBase.CryptographyHelper.EncryptWithSHA256($"{roCacheManager.GetInstance().GetCompanyGUID(Azure.RoAzureSupport.GetCompanyName())}#{complaintReference}#XAISLU".ToUpper()) Then
                    strSQL = $"@SELECT# [Id] FROM [dbo].[ChannelConversations] WHERE ReferenceNumber = '{complaintReference}'"
                    Dim idConversation = roTypes.Any2Integer(ExecuteScalar(strSQL))

                    If idConversation > 0 Then
                        retMessages = GetAllConversationMessages(idConversation, viewerIdUser, bAudit)
                    End If
                Else
                    oState.Result = MessageResultEnum.NoPermission
                End If
            Catch ex As DbException
                oState.Result = ChannelResultEnum.ErrorRecoveringAllChannels
                Me.oState.UpdateStateInfo(ex, "roMessageManager::GetAllComplaintMessages")
            Catch ex As Exception
                oState.Result = ChannelResultEnum.ErrorRecoveringAllChannels
                Me.oState.UpdateStateInfo(ex, "roMessageManager::GetAllComplaintMessages")
            Finally

            End Try
            Return retMessages
        End Function

        ''' <summary>
        ''' Recupera todos los mensajes de una conversación a los que tiene acceso un supervisor o empleado
        ''' </summary>
        ''' <param name="idConversation"></param>
        ''' <param name="bAudit"></param>
        ''' <returns></returns>
        Public Function SetAllConversationMessagesRead(ByVal idConversation As Integer, Optional viewerIdUser As Integer = 0) As Boolean

            Dim bRet As Boolean = True

            Try
                oState.Result = MessageResultEnum.NoError

                ' Cargamos información de canales para validación de permisos.
                Dim oChannelManager As roChannelManager = New roChannelManager(New roChannelState(oState.IDPassport))
                Dim lAllowedChannelsId As List(Of Integer) = New List(Of Integer)
                lAllowedChannelsId = oChannelManager.GetAllowedChannelsId(viewerIdUser, oState.IDPassport)

                Dim oConversationManager As roConversationManager = New roConversationManager(New roConversationState(oState.IDPassport))
                Dim oConversation As roConversation = oConversationManager.GetConversation(idConversation, viewerIdUser)

                If oConversation Is Nothing OrElse Not lAllowedChannelsId.Contains(oConversation.Channel.Id) Then
                    oState.Result = ConversationResultEnum.NoPermission
                    bRet = False
                    Return bRet
                End If

                Dim tb As DataTable
                Dim sql As String = $"@UPDATE# ChannelConversationMessages SET Status = 1 WHERE {IIf(viewerIdUser > 0, "IDSupervisor > 0 ", "(IdEmployee > 0 OR IdEmployee = -1 )")} AND IdConversation = {idConversation}"

                tb = CreateDataTable(sql)
            Catch ex As DbException
                oState.Result = ChannelResultEnum.ErrorRecoveringAllChannels
                Me.oState.UpdateStateInfo(ex, "roMessageManager::GetAllConversationMessages")
            Catch ex As Exception
                oState.Result = ChannelResultEnum.ErrorRecoveringAllChannels
                Me.oState.UpdateStateInfo(ex, "roMessageManager::GetAllConversationMessages")
            End Try
            Return bRet
        End Function

        ''' <summary>
        ''' Crea o actualiza una encuesta
        ''' </summary>
        ''' <param name="oMessage"></param>
        ''' <param name="bAudit"></param>
        ''' <returns></returns>
        Public Function CreateMessage(ByRef oMessage As roMessage, Optional bAudit As Boolean = False, Optional bCheckNotifications As Boolean = True, Optional bFirstMessage As Boolean = False) As Boolean
            Dim bHaveToClose As Boolean = False

            Try
                oState.Result = MessageResultEnum.NoError

                If oMessage.Id > 0 Then
                    oState.Result = MessageResultEnum.ErrorMessageAlreadyExists
                    Return False
                End If

                ' Si la conversación está en estado Cerrado o Desestimada, no guaredo el mensaje
                If oMessage.Conversation.Status = ConversationStatusEnum.Closed OrElse oMessage.Conversation.Status = ConversationStatusEnum.Dismissed Then
                    oState.Result = MessageResultEnum.ConversationClosedOrDismissed
                    Return False
                End If

                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                ' Cargamos información de canales para validación de permisos.
                Dim oChannelManager As roChannelManager = New roChannelManager(New roChannelState(oState.IDPassport))
                Dim lAllowedChannelsId As List(Of Integer) = New List(Of Integer)
                If oMessage.IsResponse Then
                    ' Como supervisor
                    lAllowedChannelsId = oChannelManager.GetAllowedChannelsId(0, oMessage.CreatedBy)
                Else
                    ' Como Empleado
                    lAllowedChannelsId = oChannelManager.GetAllowedChannelsId(oMessage.CreatedBy, 0)
                End If

                If Not lAllowedChannelsId.Contains(oMessage.Conversation.Channel.Id) Then
                    oState.Result = ConversationResultEnum.NoPermission
                    Return False
                End If

                If Not bFirstMessage AndAlso oMessage.IsResponse Then
                    ' Cambio estado de la conversación
                    Dim oConversationManager As roConversationManager = New roConversationManager(New roConversationState(oState.IDPassport))
                    oMessage.Conversation.Status = ConversationStatusEnum.Ongoing
                    oConversationManager.ChangeConversationState(oMessage.Conversation, False)
                End If

                Dim sqlCommand As String = String.Empty
                Dim parameters As New List(Of CommandParameter)

                sqlCommand = $"@INSERT# INTO ChannelConversationMessages ([IdConversation] " &
                               " ,[IdEmployee] " &
                               " ,[IdSupervisor] " &
                               " ,[CreatedOn] " &
                               " ,[Body] " &
                               " ,[Status]) " &
                               " OUTPUT INSERTED.ID " &
                               " VALUES (@idconversation,  @idemployee, @idsupervisor, " &
                               " @createdon, @body, " &
                               " @status )"

                parameters.Add(New CommandParameter("@idconversation", CommandParameter.ParameterType.tInt, oMessage.Conversation.Id))
                parameters.Add(New CommandParameter("@status", CommandParameter.ParameterType.tInt, oMessage.Status))
                If oMessage.IsResponse Then
                    parameters.Add(New CommandParameter("@idsupervisor", CommandParameter.ParameterType.tInt, oMessage.CreatedBy))
                    parameters.Add(New CommandParameter("@idemployee", CommandParameter.ParameterType.tInt, 0))
                Else
                    parameters.Add(New CommandParameter("@idemployee", CommandParameter.ParameterType.tInt, oMessage.CreatedBy))
                    parameters.Add(New CommandParameter("@idsupervisor", CommandParameter.ParameterType.tInt, 0))
                End If
                Dim sBody As String = oMessage.Body
                If oMessage.Conversation.Channel.IsComplaintChannel Then
                    Dim sCompanyEncryptionKey As String = RoAzureSupport.GetCompanySecret(roCacheManager.GetInstance().GetCompanyGUID(RoAzureSupport.GetCompanyName()))
                    sBody = CryptographyHelper.EncryptEx(oMessage.Body, sCompanyEncryptionKey)
                    oMessage.Body = sBody
                End If
                parameters.Add(New CommandParameter("@body", CommandParameter.ParameterType.tString, sBody))
                parameters.Add(New CommandParameter("@createdon", CommandParameter.ParameterType.tDateTime, oMessage.CreatedOn))

                Try
                    If oMessage.Conversation.Channel.IsComplaintChannel Then
                        Dim oLogBookManager As roLogBookManager = Nothing
                        SaveMessageOnLogBook(oMessage, oLogBookManager)

                        If oLogBookManager.State.Result <> LogBookResultEnum.NoError Then
                            oState.Result = MessageResultEnum.MessageCannotBeRegisteredOnLogBook
                            Return False
                        End If
                    End If

                    Dim iNew As Integer = roTypes.Any2Integer(AccessHelper.ExecuteScalar(sqlCommand, parameters))
                    oMessage.Id = iNew

                    If bCheckNotifications AndAlso Not oMessage.IsResponse Then
                        'Comprobamos si existe la notificación activa de notificar al supervisor que se ha generado una respuesta en un canal por parte de un usuario
                        Dim oSQLIDNotification As String = "@SELECT# ID FROM Notifications where Activated = 1 and IDType =" & eNotificationType.NewMessage_FromEmployee_InChannel
                        Dim dtNotifications As DataTable = AccessHelper.CreateDataTable(oSQLIDNotification)
                        If dtNotifications IsNot Nothing AndAlso dtNotifications.Rows.Count > 0 Then
                            For Each oRow As DataRow In dtNotifications.Rows
                                Dim imessageType As Integer = 0
                                If oMessage.Conversation.Channel.IsComplaintChannel Then
                                    imessageType = 3
                                End If
                                Dim sSqlNotification As String = $"@INSERT# INTO sysroNotificationTasks (IDNotification, Key1Numeric, Key2Numeric, key3DateTime, Parameters, Key5Numeric ) VALUES ({roTypes.Any2Integer(oRow("ID"))}, {oMessage.CreatedBy},{oMessage.Conversation.Id},{roTypes.Any2Time(Now).SQLDateTime()},'', {imessageType})"
                                ExecuteSql(sSqlNotification)
                            Next
                        End If
                    End If
                Catch ex As Exception
                    Me.oState.UpdateStateInfo(ex, "roMessageManager::ErrorCreatingMessage")
                    oState.Result = ConversationResultEnum.ErrorCreatingConversation
                End Try

                If (oState.Result = ChannelResultEnum.NoError) AndAlso bAudit Then
                    Dim conversationType As String
                    If oMessage.Conversation.Channel.IsComplaintChannel Then
                        conversationType = oState.Language.Translate($"Conversation.Type.Complaint", "")
                    Else
                        conversationType = oState.Language.Translate($"Conversation.Type.Conversation", "")
                    End If
                    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    Extensions.roAudit.AddParameter(tbParameters, "{ConversationType}", conversationType, String.Empty, 1)
                    Extensions.roAudit.AddParameter(tbParameters, "{ConversationReference}", oMessage.Conversation.ReferenceNumber, String.Empty, 1)
                    Me.oState.Audit(Audit.Action.aInsert, Audit.ObjectType.tChannelConversationMessage, oMessage.Conversation.ReferenceNumber, tbParameters, -1)
                End If
            Catch ex As DbException
                oState.Result = MessageResultEnum.ErrorCreatingMessage
                Me.oState.UpdateStateInfo(ex, "roMessageManager::CreateMessage")
            Catch ex As Exception
                oState.Result = MessageResultEnum.ErrorCreatingMessage
                Me.oState.UpdateStateInfo(ex, "roMessageManager::CreateMessage")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, oState.Result = ChannelResultEnum.NoError)
            End Try
            Return (oState.Result = ChannelResultEnum.NoError)
        End Function

        Public Function SaveMessageOnLogBook(ByRef oMessage As roMessage, ByRef oLogBookManager As roLogBookManager) As Boolean
            Dim ret As Boolean = False
            Try
                'Aseguro primero que guardo en el libro de registro
                oLogBookManager = New roLogBookManager(New roLogBookState(oState.IDPassport))

                'Añado al mensaje el nombre del denunciante (si lo facilitó) y el del supervisor
                If oMessage.IsResponse Then
                    If oMessage.CreatedBy = VTBase.roConstants.GetGlobalEnvironmentParameter(GlobalAsaxParameter.SystemPassportID) Then
                        oMessage.CreatedByName = oState.Language.Translate("Conversation.ComplaintFirstMessage.System", "")
                    Else
                        oMessage.CreatedByName = GetSupervisorName(oMessage.CreatedBy)
                    End If
                Else
                    oMessage.CreatedByName = GetEmployeeOrExternName(oMessage.CreatedBy, oMessage.Conversation)
                End If

                ret = oLogBookManager.SaveLogBook(oMessage)
            Catch ex As Exception
                oLogBookManager.State.Result = LogBookResultEnum.ErrorSavingLogBook
                ret = False
            End Try
            Return ret
        End Function

#End Region

    End Class

End Namespace