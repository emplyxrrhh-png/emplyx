Imports Robotics.Azure
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.DataLayer
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.Security
Imports Robotics.Security.Base
Imports Robotics.VTBase
Imports System.Data.Common
Imports System.Runtime.Remoting.Messaging

Namespace VTChannels

    Public Class roConversationManager

        Private oState As roConversationState

        Public ReadOnly Property State As roConversationState
            Get
                Return oState
            End Get
        End Property

#Region "Constructores"

        Public Sub New()
            oState = New roConversationState()
        End Sub

        Public Sub New(ByVal _State As roConversationState)
            oState = _State
        End Sub

#End Region

#Region "Methods"

        ''' <summary>
        ''' Recupera toda la información de una conversación
        ''' </summary>
        ''' <param name="idConversation"></param>
        ''' <param name="bAudit"></param>
        ''' <returns></returns>
        Public Function GetConversation(idConversation As Integer, Optional ByVal viewerIdUser As Integer = 0) As roConversation
            Dim retConversation As roConversation = Nothing
            Dim strSQL As String

            Try
                oState.Result = ConversationResultEnum.NoError

                ' Comprobamos permisos de consulta sobre el canal de la conversación
                Dim oChannelManager As roChannelManager = New roChannelManager(New roChannelState(oState.IDPassport))
                Dim lAllowedChannelsId As List(Of Integer) = New List(Of Integer)
                lAllowedChannelsId = oChannelManager.GetAllowedChannelsId(viewerIdUser, oState.IDPassport)

                Dim idChannel As Integer = GetConversationChannel(idConversation)

                If Not lAllowedChannelsId.Contains(idChannel) Then
                    oState.Result = ConversationResultEnum.NoPermission
                    Return Nothing
                End If

                ' Si quien pide la conversación es un empleado, sólo puede ver las suyas
                Dim sqlfilteridemployee As String = String.Empty
                If viewerIdUser > 0 Then
                    sqlfilteridemployee = $" AND CreatedBy = {viewerIdUser}"
                End If

                Dim tb As DataTable
                strSQL = $"@SELECT# [Id], [IdChannel], [CreatedBy], [CreatedOn], [Title], [IsAnonymous], [Status], [LastStatusChangeBy], [LastStatusChangeOn], [ReferenceNumber], [ExtraData], [Complexity] "
                strSQL = $"{strSQL} FROM [dbo].[ChannelConversations] WHERE Id = {idConversation}{sqlfilteridemployee}"

                tb = CreateDataTable(strSQL)

                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    Dim oRow As DataRow = tb.Rows(0)
                    retConversation = New roConversation
                    retConversation.Id = roTypes.Any2Integer(oRow("Id"))
                    retConversation.ReferenceNumber = roTypes.Any2String(oRow("ReferenceNumber"))
                    retConversation.CreatedBy = roTypes.Any2Integer(oRow("CreatedBy"))
                    retConversation.CreatedOn = If(IsDBNull(oRow("CreatedOn")), New Date(1970, 1, 1, 0, 0, 0, DateTimeKind.Local), roTypes.Any2DateTime(oRow("CreatedOn")))
                    retConversation.IsAnonymous = roTypes.Any2Boolean(oRow("IsAnonymous"))
                    If retConversation.IsAnonymous Then
                        retConversation.CreatedByName = oState.Language.Translate("Channel.Conversation.Message.Anonymous", String.Empty)
                    Else
                        Dim employeeName As String = roBusinessSupport.GetEmployeeName(retConversation.CreatedBy, oState)
                        retConversation.CreatedByName = IIf(employeeName.Length > 0, employeeName, oState.Language.Translate("Channel.Conversation.Message.Unknown", String.Empty))
                    End If
                    retConversation.Channel = oChannelManager.GetChannel(roTypes.Any2Integer(oRow("IdChannel")), False, True, viewerIdUser) 'Revisar esto, puede que no deba ser opcional el idEmployee
                    Dim title As String = roTypes.Any2String(oRow("Title"))
                    Dim sCompanyEncryptionKey As String = String.Empty
                    If retConversation.Channel.IsComplaintChannel Then
                        sCompanyEncryptionKey = RoAzureSupport.GetCompanySecret(roCacheManager.GetInstance().GetCompanyGUID(RoAzureSupport.GetCompanyName()))
                        title = CryptographyHelper.DecryptEx(title, sCompanyEncryptionKey)
                    End If
                    retConversation.Title = title
                    retConversation.Status = roTypes.Any2Integer(oRow("Status"))
                    retConversation.LastStatusChangeBy = roTypes.Any2Integer(oRow("LastStatusChangeBy"))
                    retConversation.LastStatusChangeOn = If(IsDBNull(oRow("LastStatusChangeOn")), New Date(1970, 1, 1, 0, 0, 0, DateTimeKind.Local), roTypes.Any2DateTime(oRow("LastStatusChangeOn")))
                    If retConversation.LastStatusChangeBy > 0 Then
                        Dim oPassport As roPassportTicket = roPassportManager.GetPassportTicket(retConversation.LastStatusChangeBy)
                        If oPassport IsNot Nothing AndAlso oPassport.Name.Length > 0 Then
                            retConversation.LastStatusChangeByName = oPassport.Name
                        Else
                            retConversation.LastStatusChangeByName = oState.Language.Translate("Channel.Conversation.Message.Unknown", String.Empty)
                        End If
                    End If
                    retConversation.NewMessages = GetNewMessagesOnConversation(retConversation.Id, viewerIdUser)
                    Dim messageDate As Date = Date.MinValue
                    retConversation.LastMessageHint = GetLastMessageHint(retConversation, messageDate, viewerIdUser)
                    retConversation.LastMessageTimestamp = messageDate
                    retConversation.ExtraData = Nothing
                    Dim decryptedExtraData As String = String.Empty
                    If retConversation.Channel.IsComplaintChannel Then
                        decryptedExtraData = CryptographyHelper.DecryptEx(roTypes.Any2String(oRow("ExtraData")), sCompanyEncryptionKey)
                        retConversation.ExtraData = If(IsDBNull(oRow("ExtraData")), Nothing, roJSONHelper.Deserialize(decryptedExtraData, (New roConversationExtraData).GetType()))
                    End If
                    ' Evitamos filtrar el id de empleado a los clientes
                    If retConversation.IsAnonymous AndAlso retConversation.CreatedBy > 0 Then retConversation.CreatedBy = -1
                    retConversation.Complexity = roTypes.Any2Integer(oRow("Complexity"))
                End If

                'Supervisores no suscritos no pueden participar
                If retConversation IsNot Nothing AndAlso viewerIdUser = 0 AndAlso Not retConversation.Channel.IsComplaintChannel Then
                    If retConversation Is Nothing OrElse retConversation.Channel Is Nothing OrElse Not retConversation.Channel.SubscribedSupervisors.ToList.Contains(oState.IDPassport) Then
                        Return Nothing
                        oState.Result = ConversationResultEnum.NoPermission
                    End If
                End If
            Catch ex As DbException
                oState.Result = ChannelResultEnum.ErrorRecoveringChannel
                Me.oState.UpdateStateInfo(ex, "roConversationManager::GetConversation")
            Catch ex As Exception
                oState.Result = ChannelResultEnum.ErrorRecoveringChannel
                Me.oState.UpdateStateInfo(ex, "roConversationManager::GetConversation")
            End Try
            Return retConversation
        End Function

        ''' <summary>
        ''' Recupera todas las conversaciones de un canal
        ''' </summary>
        ''' <param name="idChannel"></param>
        ''' <param name="viewerIdUser"></param>
        ''' <param name="bAudit"></param>
        ''' <returns></returns>
        Public Function GetAllChannelConversations(ByVal idChannel As Integer, Optional viewerIdUser As Integer = 0) As List(Of roConversation)

            Dim retConversations As New List(Of roConversation)
            Dim sql As String = String.Empty
            Dim bForEmployee As Boolean = (viewerIdUser > 0)

            Try
                oState.Result = ConversationResultEnum.NoError

                ' Cargamos información de canales para validación de permisos.
                Dim oChannelManager As roChannelManager = New roChannelManager(New roChannelState(oState.IDPassport))
                Dim lAllowedChannelsId As List(Of Integer) = New List(Of Integer)
                lAllowedChannelsId = oChannelManager.GetAllowedChannelsId(viewerIdUser, oState.IDPassport)

                If Not lAllowedChannelsId.Contains(idChannel) Then
                    oState.Result = ConversationResultEnum.NoPermission
                    Return retConversations
                End If

                ' Si quien accede es un empleado, sólo puede ver las conversaciones que inició. Los supervisores ven cualquier conversación de canales que gestionen
                Dim idEmployeeFilter As String = String.Empty
                If bForEmployee Then
                    idEmployeeFilter = $" AND CreatedBy = {viewerIdUser}"
                End If

                Dim tb As DataTable
                sql = $"@SELECT# Id FROM ChannelConversations WHERE IdChannel = {idChannel}{idEmployeeFilter} ORDER BY Status ASC"

                tb = CreateDataTable(sql)

                Dim oConversation As roConversation

                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    For Each oRow As DataRow In tb.Rows
                        oConversation = GetConversation(oRow("Id"), viewerIdUser)
                        If oConversation IsNot Nothing Then retConversations.Add(oConversation)
                    Next
                End If

                'Las lista de conversaciones debe ir ordenada por estado primero, y luego por número de mensajes pendientes de leer
                retConversations = retConversations.OrderBy(Function(conv) conv.Status).ThenByDescending(Function(conv) conv.LastMessageTimestamp).ToList()
            Catch ex As DbException
                oState.Result = ConversationResultEnum.ErrorRecoveringAllConversations
                Me.oState.UpdateStateInfo(ex, "roConversationManager::GetAllChannelConversations")
            Catch ex As Exception
                oState.Result = ConversationResultEnum.ErrorRecoveringAllConversations
                Me.oState.UpdateStateInfo(ex, "roConversationManager::GetAllChannelConversations")
            End Try
            Return retConversations
        End Function

        ''' <summary>
        ''' Crea o actualiza una encuesta
        ''' </summary>
        ''' <param name="oChannel"></param>
        ''' <param name="bAudit"></param>
        ''' <returns></returns>
        Public Function CreateConversation(ByRef oConversation As roConversation, ByRef oInitialMessage As roMessage, Optional bAudit As Boolean = True) As Boolean
            Dim bHaveToClose As Boolean = False

            Try
                oState.Result = ConversationResultEnum.NoError

                If oConversation.Id > 0 Then
                    oState.Result = ConversationResultEnum.ErrorConversationAlreadyExists
                    Return False
                End If

                ' Verificamos que el canal esté informado ...
                If oConversation.Channel Is Nothing OrElse oConversation.Channel.Id = 0 Then
                    oState.Result = ConversationResultEnum.ConversationShouldBelongToAChannel
                    Return False
                End If

                If oConversation.CreatedBy = 0 AndAlso Not oConversation.Channel.IsComplaintChannel Then
                    oState.Result = ConversationResultEnum.ConversationShouldBeCreatedByAnEmployee
                    Return False
                End If

                ' Si es una denuncia, no puede venir dede el Portal del empleado. Necesariamente debe ser anónima.
                If oConversation.Channel.IsComplaintChannel AndAlso (Not oConversation.IsAnonymous OrElse oConversation.CreatedBy > 0) Then
                    oState.Result = ConversationResultEnum.ComplaintsShouldBeAnonymous
                    Return False
                End If

                ' Si es una denuncia el password es obligatorio
                If oConversation.Channel.IsComplaintChannel AndAlso (oConversation.Password Is Nothing OrElse oConversation.Password.Trim.Length = 0) Then
                    oState.Result = ConversationResultEnum.ComplainantsShouldProvidePassword
                    Return False
                End If

                If oInitialMessage Is Nothing Then
                    oState.Result = ConversationResultEnum.MessageRequiredToCreateConversation
                    Return False
                ElseIf oInitialMessage.Body IsNot Nothing AndAlso oInitialMessage.Body.Trim.Length = 0 Then
                    oState.Result = ConversationResultEnum.InitialMessageCanNotBeEmpty
                    Return False
                End If

                ' Verificamos que el empleado tenga acceso, a no ser que sea una denuncia
                If Not oConversation.Channel.IsComplaintChannel Then
                    Dim oChannelManager As roChannelManager = New roChannelManager(New roChannelState(oState.IDPassport))
                    Dim oChannel As roChannel = oChannelManager.GetChannel(oConversation.Channel.Id, False, True, oConversation.CreatedBy)
                    oConversation.Channel = oChannel
                    If oChannel Is Nothing Then
                        oState.Result = ConversationResultEnum.NoPermission
                        Return False
                    End If
                End If

                If oConversation.Channel.Status = ChannelStatusEnum.Draft Then
                    oState.Result = ConversationResultEnum.ChannelShouldBePublishedToCreateConversations
                    Return False
                End If

                Dim sqlCommand As String = String.Empty
                Dim parameters As New List(Of CommandParameter)

                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                sqlCommand = $"@INSERT# INTO ChannelConversations ([IdChannel] " &
                               " ,[CreatedBy] " &
                               " ,[CreatedOn] " &
                               " ,[Title] " &
                               " ,[IsAnonymous] " &
                               " ,[ReferenceNumber] " &
                               " ,[Password] " &
                               " ,[ExtraData] " &
                               " ,[Complexity] " &
                               " ,[Status] )" &
                               " OUTPUT INSERTED.ID " &
                               " VALUES (@idchannel, @idcreatedby, " &
                               " @createdon, @title, @isanonymous, @referencenumber, @password, @extradata," &
                               " @status, @complexity)"
                Dim sCompanyEncryptionKey As String = String.Empty
                If oConversation.Channel.IsComplaintChannel Then
                    sCompanyEncryptionKey = RoAzureSupport.GetCompanySecret(roCacheManager.GetInstance().GetCompanyGUID(RoAzureSupport.GetCompanyName()))
                End If

                parameters.Add(New CommandParameter("@idchannel", CommandParameter.ParameterType.tInt, oConversation.Channel.Id))
                Dim title As String = oConversation.Title
                If oConversation.Channel.IsComplaintChannel Then
                    title = CryptographyHelper.EncryptEx(title, sCompanyEncryptionKey)
                End If
                parameters.Add(New CommandParameter("@title", CommandParameter.ParameterType.tString, title))
                parameters.Add(New CommandParameter("@referencenumber", CommandParameter.ParameterType.tString, String.Empty))
                Dim hashPassword As String = String.Empty
                If (oConversation.Password IsNot Nothing AndAlso oConversation.Password.Trim.Length > 0) Then hashPassword = HashCheckSum.CalculateString(oConversation.Password.Trim, Algorithm.SHA256)
                parameters.Add(New CommandParameter("@password", CommandParameter.ParameterType.tString, hashPassword))
                If oConversation.Channel.IsComplaintChannel AndAlso oConversation.ExtraData IsNot Nothing Then
                    Dim extraData As String = roJSONHelper.Serialize(oConversation.ExtraData)
                    extraData = CryptographyHelper.EncryptEx(extraData, sCompanyEncryptionKey)
                    parameters.Add(New CommandParameter("@extradata", CommandParameter.ParameterType.tString, extraData))
                Else
                    parameters.Add(New CommandParameter("@extradata", CommandParameter.ParameterType.tString, String.Empty))
                End If
                parameters.Add(New CommandParameter("@status", CommandParameter.ParameterType.tInt, oConversation.Status))
                parameters.Add(New CommandParameter("@idcreatedby", CommandParameter.ParameterType.tInt, oConversation.CreatedBy))
                parameters.Add(New CommandParameter("@createdon", CommandParameter.ParameterType.tDateTime, oConversation.CreatedOn))
                parameters.Add(New CommandParameter("@isanonymous", CommandParameter.ParameterType.tBoolean, oConversation.IsAnonymous))
                parameters.Add(New CommandParameter("@complexity", CommandParameter.ParameterType.tInt, roTypes.Any2Integer(oConversation.Complexity)))
                Try
                    Dim iNew As Integer = roTypes.Any2Integer(AccessHelper.ExecuteScalar(sqlCommand, parameters))
                    oConversation.Id = iNew

                    sqlCommand = "@UPDATE# ChannelConversations SET ReferenceNumber = @referencenumber WHERE ID = @id"
                    parameters.Clear()
                    parameters.Add(New CommandParameter("@id", CommandParameter.ParameterType.tInt, oConversation.Id))
                    Dim reference As String = String.Empty
                    If oConversation.Channel.IsComplaintChannel Then
                        reference = Guid.NewGuid.ToString
                    Else
                        reference = oConversation.Id.ToString.PadLeft(10, "0")
                    End If
                    oConversation.ReferenceNumber = reference
                    parameters.Add(New CommandParameter("@referencenumber", CommandParameter.ParameterType.tString, reference))
                    AccessHelper.ExecuteSql(sqlCommand, parameters)
                Catch ex As Exception
                    Me.oState.UpdateStateInfo(ex, "roConversationManager::ErrorCreatingConversation")
                    oState.Result = ConversationResultEnum.ErrorCreatingConversation
                End Try

                If (oState.Result = ChannelResultEnum.NoError) Then
                    'Comprobamos si existe la notificación activa de notificar al supervisor o avisar al usuario de nueva conversación creada
                    Dim oSQLIDNotification As String = "@SELECT# ID, IDType  from Notifications where Activated = 1 and IDType in(" & eNotificationType.NewMessage_FromEmployee_InChannel & "," & eNotificationType.Advice_For_NewConversation & ")"
                    Dim dtNotifications As DataTable = AccessHelper.CreateDataTable(oSQLIDNotification)
                    If dtNotifications IsNot Nothing AndAlso dtNotifications.Rows.Count > 0 Then
                        For Each oRow As DataRow In dtNotifications.Rows
                            Dim sSqlNotification As String = String.Empty
                            If roTypes.Any2Integer(oRow("IDType")) = eNotificationType.NewMessage_FromEmployee_InChannel Then
                                ' Supervisor -- Nueva conversacion / denuncia creada
                                Dim iconversationType As Integer = 1
                                If oConversation.Channel.IsComplaintChannel Then
                                    iconversationType = 2
                                End If
                                sSqlNotification = $"@INSERT# INTO sysroNotificationTasks (IDNotification, Key1Numeric, Key2Numeric, key3DateTime, Parameters, Key5Numeric ) VALUES " &
                                                   $" ({roTypes.Any2Integer(oRow("ID"))}, {oConversation.CreatedBy},{oConversation.Id},{roTypes.Any2Time(Now).SQLDateTime()},'', {iconversationType})"
                            ElseIf Not oConversation.Channel.IsComplaintChannel Then
                                ' Usuario -- Aviso de que se ha creado la conversacion con una referencia
                                ' solo en caso que el canal este configurado para respuesta automatica y no sea el de denuncias
                                If oConversation.Channel.ReceiptAcknowledgment Then
                                    sSqlNotification = $"@INSERT# INTO sysroNotificationTasks (IDNotification, Key1Numeric, Key2Numeric, key3DateTime, Parameters) VALUES " &
                                                       $"({roTypes.Any2Integer(oRow("ID"))}, {oConversation.CreatedBy},{oConversation.Id.ToString},{roTypes.Any2Time(Now).SQLDateTime},'')"
                                End If
                            End If
                            If sSqlNotification.Length > 0 Then ExecuteSql(sSqlNotification)
                        Next
                    End If

                    ' 1.- Guardamos el primer mensaje
                    Dim oMessageState As roMessageState = New roMessageState(oState.IDPassport)
                    Dim oMessageManager As roMessageManager = New roMessageManager(oMessageState)
                    ' 1.1.- Si se trata de una denuncia, creo un primer mensaje con el título de la denuncia, y si me los indicaron, los datos de contacto
                    Dim sMessageBody As String = $"{oState.Language.Translate("Conversation.Subject", "")}: {oConversation.Title}{vbCrLf}{vbCrLf}"
                    If oConversation.ExtraData IsNot Nothing AndAlso ConversationExtraDataToString(oConversation.ExtraData) <> String.Empty Then
                        sMessageBody = $"{sMessageBody}{ConversationExtraDataToString(oConversation.ExtraData)}{vbCrLf}"
                    End If
                    Dim oFirstMessage As New roMessage With {.IsResponse = False,
                                                                 .Conversation = oConversation,
                                                                 .Body = sMessageBody,
                                                                 .CreatedOn = oConversation.CreatedOn.AddSeconds(-1),
                                                                 .Status = MessageStatusEnum.Read,
                                                                 .CreatedBy = -1}
                    oMessageManager.CreateMessage(oFirstMessage, bAudit, False, True)

                    ' 1.2.- Creamos el mensaje
                    oInitialMessage.IsAnonymous = oConversation.IsAnonymous
                    If oConversation.Channel.IsComplaintChannel Then
                        oInitialMessage.IsAnonymous = True
                    End If
                    oMessageManager.CreateMessage(oInitialMessage, bAudit, False, True)

                    If oMessageState.Result <> MessageResultEnum.NoError Then
                        oState.Result = ConversationResultEnum.ErrorCreatingConversation
                    End If
                End If

                If (oState.Result = ChannelResultEnum.NoError) AndAlso bAudit Then
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
                    Me.oState.Audit(Audit.Action.aInsert, Audit.ObjectType.tChannelConversation, oConversation.ReferenceNumber, tbParameters, -1)
                End If
            Catch ex As DbException
                oState.Result = ConversationResultEnum.ErrorCreatingConversation
                Me.oState.UpdateStateInfo(ex, "roConversationManager::CreateConversation")
            Catch ex As Exception
                oState.Result = ConversationResultEnum.ErrorCreatingConversation
                Me.oState.UpdateStateInfo(ex, "roConversationManager::CreateConversation")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, oState.Result = ChannelResultEnum.NoError)
            End Try

            Return (oState.Result = ChannelResultEnum.NoError)
        End Function

        ''' <summary>
        ''' Crea o actualiza una encuesta
        ''' </summary>
        ''' <param name="oChannel"></param>
        ''' <param name="bAudit"></param>
        ''' <returns></returns>
        Public Function ChangeConversationState(ByRef oConversation As roConversation, Optional bAudit As Boolean = False) As Boolean
            Try
                oState.Result = ConversationResultEnum.NoError

                ' Cargamos información de canales para validación de permisos.
                Dim oChannelManager As roChannelManager = New roChannelManager(New roChannelState(oState.IDPassport))
                Dim lAllowedChannelsId As List(Of Integer) = New List(Of Integer)
                lAllowedChannelsId = oChannelManager.GetAllowedChannelsId(0, oState.IDPassport)

                If Not lAllowedChannelsId.Contains(oConversation.Channel.Id) Then
                    oState.Result = ConversationResultEnum.NoPermission
                    Return False
                End If

                Dim bStatusChanged As Boolean = False

                Dim sqlCommand As String = String.Empty
                Dim parameters As New List(Of CommandParameter)

                sqlCommand = $"@UPDATE# ChannelConversations SET [Status]=@status, [LastStatusChangeBy] = @laststatuschangedby, [LastStatusChangeOn] = @laststatuschangedon OUTPUT DELETED.Status WHERE [Id] = @id"

                parameters.Add(New CommandParameter("@id", CommandParameter.ParameterType.tInt, oConversation.Id))
                parameters.Add(New CommandParameter("@status", CommandParameter.ParameterType.tInt, oConversation.Status))
                parameters.Add(New CommandParameter("@laststatuschangedby", CommandParameter.ParameterType.tInt, oState.IDPassport))
                parameters.Add(New CommandParameter("@laststatuschangedon", CommandParameter.ParameterType.tDateTime, Now))

                Dim iOldConversationStatus As Integer = 0
                iOldConversationStatus = roTypes.Any2Integer(AccessHelper.ExecuteScalar(sqlCommand, parameters))

                If iOldConversationStatus <> oConversation.Status AndAlso oConversation.Channel.IsComplaintChannel Then
                    bStatusChanged = True

                    Dim oMessageManager As roMessageManager = New roMessageManager(New roMessageState(oState.IDPassport))

                    Dim sCompanyEncryptionKey As String = RoAzureSupport.GetCompanySecret(roCacheManager.GetInstance().GetCompanyGUID(RoAzureSupport.GetCompanyName()))
                    Dim oChangeConversationStatusMessage As New roMessage With {.IsResponse = True,
                                                                 .Conversation = oConversation,
                                                                 .Body = CryptographyHelper.EncryptEx(roTypes.Any2String(oState.Language.Translate($"LogBook.Conversation.StatusChanged.{oConversation.Status}", "")), sCompanyEncryptionKey),
                                                                 .CreatedOn = DateTime.Now,
                                                                 .CreatedBy = oState.IDPassport}

                    Dim oLogBookManager As roLogBookManager = Nothing
                    oMessageManager.SaveMessageOnLogBook(oChangeConversationStatusMessage, oLogBookManager)

                    If oLogBookManager.State.Result <> LogBookResultEnum.NoError Then
                        oState.Result = MessageResultEnum.MessageCannotBeRegisteredOnLogBook
                    End If
                End If

                If (oState.Result = ConversationResultEnum.NoError) AndAlso bAudit Then
                    Dim tbParameters As DataTable = oState.CreateAuditParameters()

                    Dim conversationType As String
                    If oConversation.Channel.IsComplaintChannel Then
                        conversationType = oState.Language.Translate($"Conversation.Type.Complaint", "")
                    Else
                        conversationType = oState.Language.Translate($"Conversation.Type.Conversation", "")
                    End If
                    Extensions.roAudit.AddParameter(tbParameters, "{ConversationType}", conversationType, String.Empty, 1)
                    Extensions.roAudit.AddParameter(tbParameters, "{ConversationReference}", oConversation.ReferenceNumber, String.Empty, 1)

                    Dim newConversationStatus As ConversationStatusEnum = oConversation.Status
                    If bStatusChanged Then
                        Dim oldConversationStatus As ConversationStatusEnum = iOldConversationStatus
                        Extensions.roAudit.AddParameter(tbParameters, "{ConversationStatus}", $"{oState.Language.Translate($"Conversation.Status.{oldConversationStatus}", String.Empty)}->{oState.Language.Translate($"Conversation.Status.{newConversationStatus}", String.Empty)}", String.Empty, 1)
                    Else
                        Extensions.roAudit.AddParameter(tbParameters, "{ConversationStatus}", $"{oState.Language.Translate($"Conversation.Status.{newConversationStatus}", String.Empty)}", String.Empty, 1)
                    End If

                    Me.oState.Audit(Audit.Action.aUpdate, Audit.ObjectType.tChannelConversation, oConversation.Title, tbParameters, -1)
                End If
            Catch ex As DbException
                oState.Result = ConversationResultEnum.ErrorCreatingConversation
                Me.oState.UpdateStateInfo(ex, "roConversationManager::ChangeConversationState")
            Catch ex As Exception
                oState.Result = ConversationResultEnum.ErrorCreatingConversation
                Me.oState.UpdateStateInfo(ex, "roConversationManager::ChangeConversationState")
            End Try

            Return (oState.Result = ConversationResultEnum.NoError)
        End Function

        Public Function ChangeComplaintComplexity(ByRef oConversation As roConversation, Optional bAudit As Boolean = False) As Boolean
            Try
                oState.Result = ConversationResultEnum.NoError

                ' Sólo para denuncias ...
                If Not oConversation.Channel.IsComplaintChannel Then
                    oState.Result = ConversationResultEnum.NoPermission
                    Return False
                End If

                ' Cargamos información de canales para validación de permisos.
                Dim oChannelManager As roChannelManager = New roChannelManager(New roChannelState(oState.IDPassport))
                Dim lAllowedChannelsId As List(Of Integer) = New List(Of Integer)
                lAllowedChannelsId = oChannelManager.GetAllowedChannelsId(0, oState.IDPassport)

                If Not lAllowedChannelsId.Contains(oConversation.Channel.Id) Then
                    oState.Result = ConversationResultEnum.NoPermission
                    Return False
                End If

                Dim sqlCommand As String = String.Empty
                Dim parameters As New List(Of CommandParameter)

                sqlCommand = $"@UPDATE# ChannelConversations SET [Complexity]=@complexity OUTPUT DELETED.Complexity WHERE [Id] = @id"

                parameters.Add(New CommandParameter("@id", CommandParameter.ParameterType.tInt, oConversation.Id))
                parameters.Add(New CommandParameter("@complexity", CommandParameter.ParameterType.tInt, oConversation.Complexity))

                Dim iOldConversationComplexity As Integer = 0
                iOldConversationComplexity = roTypes.Any2Integer(AccessHelper.ExecuteScalar(sqlCommand, parameters))

                If iOldConversationComplexity <> oConversation.Complexity Then
                    Dim oMessageManager As roMessageManager = New roMessageManager(New roMessageState(oState.IDPassport))

                    Dim sCompanyEncryptionKey As String = RoAzureSupport.GetCompanySecret(roCacheManager.GetInstance().GetCompanyGUID(RoAzureSupport.GetCompanyName()))
                    Dim oChangeConversationStatusMessage As New roMessage With {.IsResponse = True,
                                                                 .Conversation = oConversation,
                                                                 .Body = CryptographyHelper.EncryptEx(roTypes.Any2String(oState.Language.Translate($"LogBook.Conversation.ComplexityChanged.{oConversation.Complexity}", "")), sCompanyEncryptionKey),
                                                                 .CreatedOn = DateTime.Now,
                                                                 .CreatedBy = oState.IDPassport}

                    Dim oLogBookManager As roLogBookManager = Nothing
                    oMessageManager.SaveMessageOnLogBook(oChangeConversationStatusMessage, oLogBookManager)

                    If oLogBookManager.State.Result <> LogBookResultEnum.NoError Then
                        oState.Result = MessageResultEnum.MessageCannotBeRegisteredOnLogBook
                    End If
                End If

                Dim bComplexityChanged As Boolean = (iOldConversationComplexity <> oConversation.Complexity)

                If (oState.Result = ConversationResultEnum.NoError) AndAlso bAudit Then
                    Dim tbParameters As DataTable = oState.CreateAuditParameters()

                    Dim newConversationComplexity As ConversationComplexity = oConversation.Complexity
                    If bComplexityChanged Then
                        Dim oldConversationComplexity As ConversationComplexity = iOldConversationComplexity
                        Extensions.roAudit.AddParameter(tbParameters, "{ConversationComplexity}", $"{oState.Language.Translate($"Conversation.Complexity.{oldConversationComplexity}", String.Empty)}->{oState.Language.Translate($"Conversation.Complexity.{newConversationComplexity}", String.Empty)}", String.Empty, 1)
                    Else
                        Extensions.roAudit.AddParameter(tbParameters, "{ConversationComplexity}", $"{oState.Language.Translate($"Conversation.Complexity.{newConversationComplexity}", String.Empty)}", String.Empty, 1)
                    End If

                    Me.oState.Audit(Audit.Action.aUpdate, Audit.ObjectType.tChannelConversationComplexity, oConversation.ReferenceNumber, tbParameters, -1)
                End If
            Catch ex As DbException
                oState.Result = ConversationResultEnum.ErrorCreatingConversation
                Me.oState.UpdateStateInfo(ex, "roConversationManager::ChangeComplaintComplexity")
            Catch ex As Exception
                oState.Result = ConversationResultEnum.ErrorCreatingConversation
                Me.oState.UpdateStateInfo(ex, "roConversationManager::ChangeComplaintComplexity")
            End Try

            Return (oState.Result = ConversationResultEnum.NoError)
        End Function

#End Region

#Region "Helper"

        Private Function GetNewMessagesOnConversation(idConversation As Integer, idEmploye As Integer) As Integer
            Dim ret As Integer = 0
            Try
                Dim sSQL As String = String.Empty

                sSQL = $"@SELECT# COUNT(*) FROM ChannelConversationMessages WHERE ChannelConversationMessages.IdConversation = {idConversation} And ChannelConversationMessages.Status = 0 {IIf(idEmploye > 0, " And ChannelConversationMessages.IdEmployee = 0 ", $" And ChannelConversationMessages.IdEmployee > {idEmploye}")}"

                ret = roTypes.Any2Integer(ExecuteScalar(sSQL))
            Catch ex As DbException
                oState.Result = ConversationResultEnum.ErrorRecoveringNewMessagesInConversation
                Me.oState.UpdateStateInfo(ex, "roConversationManager::GetNewMessagesOnConversation")
            Catch ex As Exception
                oState.Result = ConversationResultEnum.ErrorRecoveringNewMessagesInConversation
                Me.oState.UpdateStateInfo(ex, "roConversationManager::GetNewMessagesOnConversation")
            End Try
            Return ret
        End Function

        Private Function GetLastMessageHint(oConversation As roConversation, ByRef messageDate As Date, Optional viewerIdUser As Integer = 0) As String
            Dim ret As String = String.Empty
            Try
                Dim sSQL As String = String.Empty
                Dim idEmployee As Integer = 0
                Dim idSupervisor As Integer = 0
                Dim iIdMessage As Integer = 0
                Dim sBody As String = String.Empty

                sSQL = $"@SELECT# Top 1 Id, IdEmployee, IdSupervisor, Body, CreatedOn FROM ChannelConversationMessages WHERE ChannelConversationMessages.IdConversation = {oConversation.Id} ORDER BY ChannelConversationMessages.CreatedOn DESC"

                Dim table As DataTable = AccessHelper.CreateDataTable(sSQL)
                If table IsNot Nothing AndAlso table.Rows.Count = 1 Then
                    iIdMessage = roTypes.Any2Integer(table.Rows(0)("Id"))
                    idEmployee = roTypes.Any2Integer(table.Rows(0)("IdEmployee"))
                    idSupervisor = roTypes.Any2Integer(table.Rows(0)("IdSupervisor"))
                    sBody = roTypes.Any2String(table.Rows(0)("Body"))
                    If oConversation.Channel.IsComplaintChannel Then
                        Dim sCompanyEncryptionKey As String = RoAzureSupport.GetCompanySecret(roCacheManager.GetInstance().GetCompanyGUID(RoAzureSupport.GetCompanyName()))
                        sBody = CryptographyHelper.DecryptEx(roTypes.Any2String(sBody), sCompanyEncryptionKey)
                    End If
                    messageDate = roTypes.Any2DateTime(table.Rows(0)("CreatedOn"))
                End If

                If iIdMessage > 0 Then
                    ret = sBody

                    ' Si la conversación la está viendo un empleado ...
                    If viewerIdUser > 0 Then
                        If idEmployee > 0 Then
                            ret = $"{(State.Language.Translate("Conversation.LastMessage.YouAnswered", String.Empty))}: {ret}"
                        Else
                            Dim oPassport As roPassportTicket = roPassportManager.GetPassportTicket(idSupervisor)
                            If oPassport IsNot Nothing AndAlso oPassport.Name.Length > 0 Then
                                ret = $"{oPassport.Name}: {ret}"
                            Else
                                ret = $"{oState.Language.Translate("Channel.Conversation.Message.Unknown", String.Empty)}: {ret}"
                            End If
                        End If
                    Else
                        ' Si la conversación la está viendo un supervisor ...
                        If idSupervisor > 0 Then
                            ' Si el último mensaje es de supervisor ...
                            If idSupervisor = oState.IDPassport Then
                                ret = $"{(State.Language.Translate("Conversation.LastMessage.YouAnswered", String.Empty))}: {ret}"
                            Else
                                Dim oPassport As roPassportTicket = roPassportManager.GetPassportTicket(idSupervisor)
                                If oPassport IsNot Nothing AndAlso oPassport.Name.Length > 0 Then
                                    ret = $"{oPassport.Name}: {ret}"
                                Else
                                    ret = $"{(oState.Language.Translate("Channel.Conversation.Message.Unknown", String.Empty))}: {ret}"
                                End If
                            End If
                        Else
                            ' Si el último mensaje es de empleado, cuidado si es anónimo ...
                            ret = $"{oConversation.CreatedByName}: {ret}"
                        End If
                    End If

                    If ret.Length > 43 Then
                        ret = $"{(sBody.Substring(0, 43))} ..."
                    End If

                End If
            Catch ex As DbException
                oState.Result = ConversationResultEnum.ErrorRecoveringNewMessagesInConversation
                Me.oState.UpdateStateInfo(ex, "roConversationManager::GetLastMessagesHint")
            Catch ex As Exception
                oState.Result = ConversationResultEnum.ErrorRecoveringNewMessagesInConversation
                Me.oState.UpdateStateInfo(ex, "roConversationManager::GetLastMessagesHint")
            End Try
            Return ret
        End Function

        Private Function GetConversationChannel(idConversation As Integer) As Integer
            Dim ret As Integer = -1
            Try
                Dim sSQL As String = String.Empty

                sSQL = $"@SELECT# IdChannel FROM ChannelConversations WHERE Id = {idConversation}"

                ret = roTypes.Any2Integer(ExecuteScalar(sSQL))
            Catch ex As DbException
                oState.Result = ConversationResultEnum.ErrorRecoveringNewMessagesInConversation
                Me.oState.UpdateStateInfo(ex, "roConversationManager::GetConversationChannel")
            Catch ex As Exception
                oState.Result = ConversationResultEnum.ErrorRecoveringNewMessagesInConversation
                Me.oState.UpdateStateInfo(ex, "roConversationManager::GetConversationChannel")
            End Try
            Return ret
        End Function

        Private Function ConversationExtraDataToString(oExtradata As roConversationExtraData) As String
            Dim ret As String = String.Empty
            Try
                If oExtradata.FullName Is Nothing AndAlso oExtradata.Mail Is Nothing AndAlso oExtradata.Customer Is Nothing AndAlso oExtradata.Phone Is Nothing AndAlso oExtradata.Other Is Nothing Then
                    Return ret
                End If

                ret = $"{(oState.Language.Translate("Conversation.ExtraData.FullName", String.Empty))}: {oExtradata.FullName}{vbCrLf}"
                ret = $"{ret}{oState.Language.Translate("Conversation.ExtraData.Phone", String.Empty)}: {oExtradata.Phone}{vbCrLf}"
                ret = $"{ret}{oState.Language.Translate("Conversation.ExtraData.Email", String.Empty)}: {oExtradata.Mail}{vbCrLf}"
                ret = $"{ret}{oState.Language.Translate("Conversation.ExtraData.Customer", String.Empty)}: {oExtradata.Customer}{vbCrLf}"
            Catch
            End Try
            Return ret
        End Function

#End Region

    End Class

End Namespace