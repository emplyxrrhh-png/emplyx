Imports System.Data.Common
Imports Microsoft.SqlServer.Server
Imports Robotics.Base.DTOs
Imports Robotics.DataLayer
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.Security
Imports Robotics.Security.Base
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions

Namespace VTChannels

    Public Class roChannelManager

        Private oState As roChannelState = Nothing
        Private hasChannelsLicense As Boolean = False
        Private hasComplaintsLicense As Boolean = False

        Public ReadOnly Property State As roChannelState
            Get
                Return oState
            End Get
        End Property

#Region "Constructores"

        Public Sub New()
            oState = New roChannelState()
            LoadLicenses()
        End Sub

        Public Sub New(ByVal _State As roChannelState)
            oState = _State
            LoadLicenses()
        End Sub



        Private Sub LoadLicenses()
            Dim serverLic As New roServerLicense
            Dim channelLic As Boolean = serverLic.FeatureIsInstalled("Feature\Channels")
            Dim complaintsLic As Boolean = serverLic.FeatureIsInstalled("Feature\Complaints")

            hasChannelsLicense = channelLic
            hasComplaintsLicense = channelLic OrElse complaintsLic
        End Sub
#End Region

#Region "Methods"

        ''' <summary>
        ''' Recover channel information, applying security, whether  the function is called by a supervisor or an employee
        ''' </summary>
        ''' <param name="idChannel"></param>
        ''' <param name="bAudit"></param>
        ''' <param name="bLoadLists"></param>
        ''' <param name="viewerIdUser"></param>
        ''' <returns></returns>
        Public Function GetChannel(idChannel As Integer, Optional ByVal bAudit As Boolean = False, Optional bLoadLists As Boolean = True, Optional ByVal viewerIdUser As Integer = 0) As roChannel
            Dim retChannel As roChannel = Nothing

            Try
                Dim lAllowedChannelsId As New List(Of Integer)
                lAllowedChannelsId = GetAllowedChannelsId(viewerIdUser, oState.IDPassport)

                If lAllowedChannelsId.Contains(idChannel) Then
                    retChannel = GetChannelById(idChannel, False, bLoadLists, viewerIdUser)
                Else
                    oState.Result = ChannelResultEnum.NoPermission
                End If
            Catch ex As Exception
                oState.Result = ChannelResultEnum.ErrorRecoveringChannel
                Me.oState.UpdateStateInfo(ex, "roChannelManager::GetChannel")
            End Try

            Return retChannel
        End Function

        ''' <summary>
        ''' Recover channel information. No security check
        ''' </summary>
        ''' <param name="idChannel"></param>
        ''' <param name="bAudit"></param>
        ''' <returns></returns>
        Private Function GetChannelById(idChannel As Integer, Optional ByVal bAudit As Boolean = False, Optional bLoadLists As Boolean = True, Optional viewerIdUser As Integer = 0) As roChannel
            Dim retChannel As roChannel = Nothing
            Dim strSQL As String
            Dim lEmployees As List(Of Integer)
            Dim lGroups As List(Of Integer)
            Dim lSupervisors As List(Of Integer)

            Try

                If Not hasComplaintsLicense AndAlso Not hasChannelsLicense Then
                    oState.Result = ChannelResultEnum.NoPermission
                    Return retChannel
                End If

                oState.Result = ChannelResultEnum.NoError

                Dim tb As DataTable

                strSQL = $"@SELECT# [Id],[Title],[IdCreatedBy],[Status],[CreatedOn],[IdModifiedBy],[ModifiedOn],[PublishedOn],[ReceiptAcknowledgment],[AllowAnonymous],[Deleted],[IdDeletedBy],[DeletedOn],[IsComplaintChannel],[PrivacyPolicy] "
                strSQL = $"{strSQL} FROM [dbo].[Channels] WHERE Id = {idChannel} ORDER BY Title ASC"

                tb = CreateDataTable(strSQL)

                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    Dim oRow As DataRow = tb.Rows(0)
                    retChannel = New roChannel
                    retChannel.Id = roTypes.Any2Integer(oRow("Id"))
                    retChannel.Title = roTypes.Any2String(oRow("Title"))
                    retChannel.IsComplaintChannel = roTypes.Any2Boolean(oRow("IsComplaintChannel"))
                    If retChannel.IsComplaintChannel Then retChannel.Title = oState.Language.Translate("Channel.ChannelTitle.Complaint", "")
                    retChannel.CreatedBy = roTypes.Any2Integer(oRow("IdCreatedBy"))
                    retChannel.CreatedOn = If(IsDBNull(oRow("CreatedOn")), New DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local), roTypes.Any2DateTime(oRow("CreatedOn")))
                    retChannel.ModifiedBy = roTypes.Any2Integer(oRow("IdModifiedBy"))
                    retChannel.ModifiedOn = If(IsDBNull(oRow("ModifiedOn")), New DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local), roTypes.Any2DateTime(oRow("ModifiedOn")))
                    retChannel.PublishedOn = If(IsDBNull(oRow("PublishedOn")), New DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local), roTypes.Any2DateTime(oRow("PublishedOn")))
                    retChannel.ReceiptAcknowledgment = roTypes.Any2Boolean(oRow("ReceiptAcknowledgment"))
                    retChannel.AllowAnonymous = roTypes.Any2Boolean(oRow("AllowAnonymous"))
                    retChannel.Deleted = roTypes.Any2Boolean(oRow("Deleted"))
                    retChannel.DeletedBy = roTypes.Any2Integer(oRow("IdDeletedBy"))
                    retChannel.DeletedOn = IIf(IsDBNull(oRow("DeletedOn")), New DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local), roTypes.Any2DateTime(oRow("DeletedOn")))
                    retChannel.Status = roTypes.Any2Integer(oRow("Status"))
                    retChannel.NewMessages = GetNewMessagesOnChannel(retChannel.Id, viewerIdUser)
                    retChannel.OpenConversations = GetOpenConversationsOnChannel(retChannel.Id)
                    retChannel.SubscribedSupervisors = {}
                    retChannel.Employees = {}
                    retChannel.Groups = {}
                    retChannel.PrivacyPolicy = roTypes.Any2String(oRow("PrivacyPolicy"))


                    Dim oPassport As roPassportTicket = roPassportManager.GetPassportTicket(retChannel.CreatedBy)
                    If oPassport IsNot Nothing AndAlso oPassport.Name.Length > 0 Then
                        retChannel.CreatedByName = oPassport.Name
                    Else
                        retChannel.CreatedByName = oState.Language.Translate("Channel.Conversation.Message.Unknown", "")
                    End If
                    If retChannel.ModifiedBy > 0 Then
                        oPassport = roPassportManager.GetPassportTicket(retChannel.ModifiedBy)
                        If oPassport IsNot Nothing AndAlso oPassport.Name.Length > 0 Then
                            retChannel.ModifiedByName = oPassport.Name
                        Else
                            retChannel.ModifiedByName = oState.Language.Translate("Channel.Conversation.Message.Unknown", "")
                        End If
                    End If
                    If retChannel.DeletedBy > 0 Then
                        oPassport = roPassportManager.GetPassportTicket(retChannel.DeletedBy)
                        If oPassport IsNot Nothing AndAlso oPassport.Name.Length > 0 Then
                            retChannel.DeletedByName = oPassport.Name
                        Else
                            retChannel.DeletedByName = oState.Language.Translate("Channel.Conversation.Message.Unknown", "")
                        End If
                    End If
                End If

                If retChannel IsNot Nothing Then
                    '1.- Cargo Empleados
                    tb = CreateDataTable($"@SELECT# * FROM ChannelEmployees WHERE IdChannel = {retChannel.Id}")
                    If bLoadLists AndAlso tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                        lEmployees = New List(Of Integer)
                        For Each oRow As DataRow In tb.Rows
                            lEmployees.Add(oRow("IdEmployee"))
                        Next
                        retChannel.Employees = lEmployees.ToArray
                    Else
                        retChannel.Employees = {}
                    End If

                    '2.- Cargo Grupos
                    tb = CreateDataTable($"@SELECT# * FROM ChannelGroups WHERE IdChannel = {retChannel.Id}")
                    If bLoadLists AndAlso tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                        lGroups = New List(Of Integer)
                        For Each oRow As DataRow In tb.Rows
                            lGroups.Add(oRow("IdGroup"))
                        Next
                        retChannel.Groups = lGroups.ToArray
                    Else
                        retChannel.Groups = {}
                    End If

                    '2.- Cargo Supervisores suscritos
                    tb = CreateDataTable($"@SELECT# * FROM ChannelSupervisors WHERE IdChannel = {retChannel.Id}")
                    If bLoadLists AndAlso tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                        lSupervisors = New List(Of Integer)
                        For Each oRow As DataRow In tb.Rows
                            lSupervisors.Add(oRow("IdSupervisor"))
                        Next
                        retChannel.SubscribedSupervisors = lSupervisors.ToArray
                    Else
                        retChannel.SubscribedSupervisors = {}
                    End If
                End If

                If (oState.Result = ChannelResultEnum.NoError) AndAlso bAudit Then
                    Dim channelTitle As String = If(retChannel IsNot Nothing, retChannel.Title, "")

                    ' Auditamos
                    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    Extensions.roAudit.AddParameter(tbParameters, "{ChannelTitle}", channelTitle, String.Empty, 1)
                    Me.oState.Audit(Audit.Action.aSelect, Audit.ObjectType.tChannel, channelTitle, tbParameters, -1)
                End If
            Catch ex As DbException
                oState.Result = ChannelResultEnum.ErrorRecoveringChannel
                Me.oState.UpdateStateInfo(ex, "roChannelManager::GetChannel")
            Catch ex As Exception
                oState.Result = ChannelResultEnum.ErrorRecoveringChannel
                Me.oState.UpdateStateInfo(ex, "roChannelManager::GetChannel")
            End Try
            Return retChannel
        End Function

        ''' <summary>
        ''' Recover channel information on every channel, applying security, whether the function is called by a supervisor or an employee
        ''' </summary>
        ''' <param name="viewerIdUser"></param>
        ''' <param name="bAudit"></param>
        ''' <returns></returns>
        Public Function GetAllChannels(Optional viewerIdUser As Integer = 0, Optional ByVal bAudit As Boolean = False, Optional bLoadLists As Boolean = True) As List(Of roChannel)
            Dim retChannels As New List(Of roChannel)
            Dim lAllowedChannelsId As New List(Of Integer)

            Try
                oState.Result = ChannelResultEnum.NoError

                lAllowedChannelsId = GetAllowedChannelsId(viewerIdUser, oState.IDPassport)

                Dim oChannel As roChannel

                For Each id As Integer In lAllowedChannelsId
                    oChannel = GetChannelById(id, False, bLoadLists, viewerIdUser)
                    If oChannel IsNot Nothing Then retChannels.Add(oChannel)
                Next
            Catch ex As DbException
                oState.Result = ChannelResultEnum.ErrorRecoveringAllChannels
                Me.oState.UpdateStateInfo(ex, "roChannelManager::GetAllChannels")
            Catch ex As Exception
                oState.Result = ChannelResultEnum.ErrorRecoveringAllChannels
                Me.oState.UpdateStateInfo(ex, "roChannelManager::GetAllChannels")
            End Try
            Return retChannels
        End Function


        ''' <summary>
        ''' Recover channel information on every channel, applying security, whether the function is called by a supervisor or an employee
        ''' </summary>
        ''' <param name="viewerIdUser"></param>
        ''' <param name="bAudit"></param>
        ''' <returns></returns>
        Public Function HasChannelsWithMessages(idEmployee As Integer) As Boolean
            Dim lAllowedChannelsId As New List(Of Integer)

            Try
                oState.Result = ChannelResultEnum.NoError

                lAllowedChannelsId = GetAllowedChannelsId(idEmployee, oState.IDPassport)
                For Each id As Integer In lAllowedChannelsId
                    If GetNewMessagesOnChannel(id, idEmployee) Then Return True
                Next
            Catch ex As DbException
                oState.Result = ChannelResultEnum.ErrorRecoveringAllChannels
                Me.oState.UpdateStateInfo(ex, "roChannelManager::HasChannelsWithMessages")
            Catch ex As Exception
                oState.Result = ChannelResultEnum.ErrorRecoveringAllChannels
                Me.oState.UpdateStateInfo(ex, "roChannelManager::HasChannelsWithMessages")
            End Try

            Return False
        End Function


        Public Function GetAllowedChannelsId(Optional viewerIdUser As Integer = 0, Optional viewerIdSupervisor As Integer = 0) As List(Of Integer)
            Dim retChannelsId As New List(Of Integer)
            Dim sqlChannels As String = String.Empty
            Dim sqlComplaintChannels As String = String.Empty
            Dim bForEmployee As Boolean = (viewerIdUser > 0)
            Dim bForComplainant As Boolean = (viewerIdUser < 0)

            Try
                oState.Result = ChannelResultEnum.NoError

                If viewerIdUser = 0 AndAlso viewerIdSupervisor = 0 Then
                    Return retChannelsId
                End If

                Dim tb As DataTable = Nothing

                If bForEmployee Then
                    ' Un empleado puede ver todos los canales a los que esté suscrito, directamente, o a través de un grupo ...
                    sqlChannels = $"@SELECT# Id FROM Channels INNER JOIN sysrovwChannelEmployees ON sysrovwChannelEmployees.IdChannel = Channels.Id WHERE Channels.Deleted = 0 AND Channels.Status = 1 AND IdEmployee = {viewerIdUser}"
                    ' ... y todos los canales de denuncia
                    sqlComplaintChannels = "@SELECT# Id FROM Channels WHERE IsComplaintChannel = 1 AND Channels.Status = 1 AND Channels.Deleted = 0 "

                    If hasChannelsLicense Then
                        sqlChannels = $"{sqlChannels} UNION {sqlComplaintChannels}"
                    ElseIf Not hasChannelsLicense AndAlso hasComplaintsLicense Then
                        sqlChannels = $"{sqlComplaintChannels}"
                    Else
                        sqlChannels = String.Empty
                    End If

                    If Not String.IsNullOrEmpty(sqlChannels) Then tb = CreateDataTable(sqlChannels)
                ElseIf bForComplainant Then
                    If hasChannelsLicense OrElse hasComplaintsLicense Then
                        ' Un denunciante puede ver todos los canales de denuncia
                        sqlChannels = "@SELECT# Id FROM Channels WHERE IsComplaintChannel = 1 AND Channels.Status = 1 AND Channels.Deleted = 0 "
                    Else
                        sqlChannels = String.Empty
                    End If

                    If Not String.IsNullOrEmpty(sqlChannels) Then tb = CreateDataTable(sqlChannels)
                Else
                    ' --------------------------------- Canales de propósito general -----------------------------------------------
                    ' Vemos permisos del supervisor que está accediendo para averiguar qué canales puede ver
                    Dim iPermissionOverChannels As Integer = WLHelper.GetPermissionOverFeature(viewerIdSupervisor, "Employees.Channels", "U")
                    ' Los ven los supervisores que tiene administración sobre el permiso de canales de denuncia
                    Dim iPermissionOverComplaintChannels As Integer = WLHelper.GetPermissionOverFeature(viewerIdSupervisor, "Employees.Complaints", "U")

                    ' Consultores no pueden acceder
                    If roPassportManager.IsRoboticsUserOrConsultant(viewerIdSupervisor) Then
                        iPermissionOverChannels = 0
                        iPermissionOverComplaintChannels = 0
                    End If

                    Select Case iPermissionOverChannels
                        Case 9
                            'Todos
                            sqlChannels = "@SELECT# Id FROM Channels WHERE Deleted = 0 AND IsComplaintChannel = 0"
                        Case 6
                            'Sólo los suyos
                            sqlChannels = $"@SELECT# Id FROM Channels WHERE Deleted = 0 AND IsComplaintChannel = 0 AND (IdCreatedBy = {viewerIdSupervisor} OR Id IN (@SELECT# IdChannel FROM ChannelSupervisors WHERE IdSupervisor = {viewerIdSupervisor}))"
                        Case 3
                            'Sólo a los que esté suscrito
                            sqlChannels = $"@SELECT# Id FROM Channels WHERE Deleted = 0 AND IsComplaintChannel = 0 AND Id IN (@SELECT# IdChannel FROM ChannelSupervisors WHERE IdSupervisor = {viewerIdSupervisor})"
                        Case Else
                            'No puede ver ninguno
                    End Select

                    ' ----------------------------------------------- Canales de denuncias -------------------------------------------------
                    If iPermissionOverComplaintChannels = 9 Then
                        sqlComplaintChannels = "@SELECT# Id FROM Channels WHERE Deleted = 0 AND IsComplaintChannel = 1"
                    End If

                    If hasChannelsLicense AndAlso hasComplaintsLicense Then
                        If sqlChannels <> String.Empty AndAlso sqlComplaintChannels <> String.Empty Then
                            sqlChannels = sqlChannels & " UNION " & sqlComplaintChannels
                        ElseIf sqlChannels = String.Empty Then
                            sqlChannels = sqlComplaintChannels
                        End If
                    ElseIf Not hasChannelsLicense AndAlso hasComplaintsLicense Then
                        sqlChannels = sqlComplaintChannels
                    Else
                        sqlChannels = String.Empty
                    End If

                    If Not String.IsNullOrEmpty(sqlChannels) Then tb = CreateDataTable(sqlChannels)
                End If

                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    For Each oRow As DataRow In tb.Rows
                        retChannelsId.Add(oRow("Id"))
                    Next
                End If
            Catch ex As DbException
                oState.Result = ChannelResultEnum.ErrorRecoveringAllChannels
                Me.oState.UpdateStateInfo(ex, "roChannelManager::GetAllowedChannelsId")
            Catch ex As Exception
                oState.Result = ChannelResultEnum.ErrorRecoveringAllChannels
                Me.oState.UpdateStateInfo(ex, "roChannelManager::GetAllowedChannelsId")
            End Try
            Return retChannelsId
        End Function

        ''' <summary>
        ''' Creates or update a channel. Security checked.
        ''' </summary>
        ''' <param name="oChannel"></param>
        ''' <param name="bAudit"></param>
        ''' <returns></returns>
        Public Function CreateOrUpdateChannel(ByRef oChannel As roChannel, Optional bAudit As Boolean = False) As Boolean
            Dim bolRet As Boolean = False
            Dim bolIsNew As Boolean = True
            Dim bHaveToClose As Boolean = False
            Dim iNew As Integer
            Dim iOldStatus As Integer

            Try
                oState.Result = ChannelResultEnum.NoError

                If Not DataLayer.roSupport.IsXSSSafe(oChannel) Then
                    oState.Result = ChannelResultEnum.XSSvalidationError
                    Return False
                End If


                bolIsNew = (oChannel.Id = 0)

                ' Verificamos permiso
                Dim featureId As String = "Employees.Channels"
                If oChannel.IsComplaintChannel Then featureId = "Employees.Complaints"


                If (oChannel.IsComplaintChannel AndAlso Not hasComplaintsLicense) OrElse (Not oChannel.IsComplaintChannel AndAlso Not hasChannelsLicense) Then
                    Me.oState.Result = ChannelResultEnum.NoPermission
                    Return False
                End If


                Dim iPermissionOverChannels As Integer = WLHelper.GetPermissionOverFeature(Me.oState.IDPassport, featureId, "U")
                If iPermissionOverChannels < 6 Then
                    Me.oState.Result = ChannelResultEnum.NoPermission
                    Return False
                ElseIf iPermissionOverChannels = 6 Then 'Complaint Channel cannot be 6
                    If Not bolIsNew AndAlso oChannel.CreatedBy <> Me.oState.IDPassport Then
                        Me.oState.Result = ChannelResultEnum.NoPermission
                        Return False
                    End If
                End If

                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                Dim sqlCommand As String = String.Empty
                Dim parameters As New List(Of CommandParameter)

                Dim bStatusChanged As Boolean = False

                If Not bolIsNew Then
                    sqlCommand = $"@UPDATE# Channels SET [Title]=@title, [Status]=@status, " &
                                        " [ModifiedOn]=@modifiedon, [IdModifiedBy]=@idmodifiedby, [PublishedOn]=@publishedon, " &
                                        " [ReceiptAcknowledgment]=@receiptacknowledgment, [AllowAnonymous]=@allowanonymous, [PrivacyPolicy]=@privacypolicy " &
                                        " OUTPUT DELETED.Status" &
                                        " WHERE [Id] = @id"
                Else
                    sqlCommand = $"@INSERT# INTO Channels ([Title] " &
                                    " ,[IdCreatedBy] " &
                                    " ,[Status] " &
                                    " ,[CreatedOn] " &
                                    " ,[ReceiptAcknowledgment] " &
                                    " ,[PrivacyPolicy] " &
                                    " ,[AllowAnonymous]) " &
                                    " OUTPUT INSERTED.ID " &
                                    " VALUES (@title, @idcreatedby, " &
                                    " @status, @createdon, " &
                                    " @receiptacknowledgment, @privacypolicy, @allowanonymous)"
                End If

                parameters.Add(New CommandParameter("@title", CommandParameter.ParameterType.tString, oChannel.Title))
                parameters.Add(New CommandParameter("@status", CommandParameter.ParameterType.tInt, oChannel.Status))
                If bolIsNew Then parameters.Add(New CommandParameter("@idcreatedby", CommandParameter.ParameterType.tInt, oChannel.CreatedBy))
                If bolIsNew Then parameters.Add(New CommandParameter("@createdon", CommandParameter.ParameterType.tDateTime, oChannel.CreatedOn))
                If Not bolIsNew Then parameters.Add(New CommandParameter("@id", CommandParameter.ParameterType.tInt, oChannel.Id))
                If Not bolIsNew Then parameters.Add(New CommandParameter("@idmodifiedby", CommandParameter.ParameterType.tInt, oChannel.ModifiedBy))
                If Not bolIsNew Then parameters.Add(New CommandParameter("@modifiedon", CommandParameter.ParameterType.tDateTime, oChannel.ModifiedOn))
                If Not bolIsNew Then parameters.Add(New CommandParameter("@publishedon", CommandParameter.ParameterType.tDateTime, oChannel.PublishedOn))
                parameters.Add(New CommandParameter("@receiptacknowledgment", CommandParameter.ParameterType.tBoolean, oChannel.ReceiptAcknowledgment))
                parameters.Add(New CommandParameter("@allowanonymous", CommandParameter.ParameterType.tBoolean, oChannel.AllowAnonymous))
                parameters.Add(New CommandParameter("@privacypolicy", CommandParameter.ParameterType.tString, roTypes.Any2String(oChannel.PrivacyPolicy)))

                Try
                    If bolIsNew Then
                        iNew = roTypes.Any2Integer(AccessHelper.ExecuteScalar(sqlCommand, parameters))
                    Else
                        iOldStatus = roTypes.Any2Integer(AccessHelper.ExecuteScalar(sqlCommand, parameters))
                        If iOldStatus <> oChannel.Status Then bStatusChanged = True
                    End If
                    bolRet = True
                    If bolIsNew Then oChannel.Id = iNew
                Catch ex As Exception
                    Me.oState.UpdateStateInfo(ex, "roChannelManager::ErrorCreatingOrUpdatingChannel")
                    oState.Result = ChannelResultEnum.ErrorCreatingOrUpdatingChannel
                End Try

                If (oState.Result = ChannelResultEnum.NoError) Then
                    ' 1.- Guardamos empleados suscritos directamente
                    If Not bolIsNew Then
                        sqlCommand = $"@DELETE# ChannelEmployees WHERE IdChannel = {oChannel.Id}"
                        bolRet = ExecuteSql(sqlCommand)
                    End If

                    If bolRet AndAlso oChannel.Employees IsNot Nothing AndAlso oChannel.Employees.Length > 0 Then
                        For Each idEmployee As Integer In oChannel.Employees
                            sqlCommand = $"@INSERT# INTO ChannelEmployees (IdChannel, IdEmployee) VALUES ({oChannel.Id},{idEmployee})"
                            bolRet = ExecuteSql(sqlCommand)
                            If Not bolRet Then
                                oState.Result = ChannelResultEnum.ErrorSettingChannelEmployees
                            End If
                        Next
                    End If

                    ' 2.- Guardamos grupos suscritos
                    If Not bolIsNew Then
                        sqlCommand = $"@DELETE# ChannelGroups WHERE IdChannel = {oChannel.Id}"
                        bolRet = ExecuteSql(sqlCommand)
                    End If

                    If bolRet AndAlso oChannel.Groups IsNot Nothing AndAlso oChannel.Groups.Length > 0 Then
                        For Each idGroup As Integer In oChannel.Groups
                            sqlCommand = $"@INSERT# INTO ChannelGroups (IdChannel, IdGroup) VALUES ({oChannel.Id},{idGroup})"
                            bolRet = ExecuteSql(sqlCommand)
                            If Not bolRet Then
                                oState.Result = ChannelResultEnum.ErrorSettingChannelGroups
                            End If
                        Next
                    End If

                    ' 3.- Guardamos supervisores suscritos
                    If Not bolIsNew Then
                        sqlCommand = $"@DELETE# ChannelSupervisors WHERE IdChannel = {oChannel.Id}"
                        bolRet = ExecuteSql(sqlCommand)
                    End If

                    If bolRet AndAlso oChannel.SubscribedSupervisors IsNot Nothing AndAlso oChannel.SubscribedSupervisors.Length > 0 Then
                        For Each idSupervisor As Integer In oChannel.SubscribedSupervisors
                            sqlCommand = $"@INSERT# INTO ChannelSupervisors (IdChannel, IdSupervisor) VALUES ({oChannel.Id},{idSupervisor})"
                            bolRet = ExecuteSql(sqlCommand)
                            If Not bolRet Then
                                oState.Result = ChannelResultEnum.ErrorSettingChannelSupervisors
                            End If
                        Next
                    End If
                End If

                If (oState.Result = ChannelResultEnum.NoError) AndAlso bAudit Then
                    ' Auditamos
                    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    Dim strChannelEmployees As String = String.Empty
                    If oChannel.Employees IsNot Nothing AndAlso oChannel.Employees.Length > 0 Then strChannelEmployees = String.Join(",", oChannel.Employees)
                    Dim strChannelGroups As String = String.Empty
                    If oChannel.Groups IsNot Nothing AndAlso oChannel.Groups.Length > 0 Then strChannelGroups = String.Join(",", oChannel.Groups)
                    Dim strChannelSupervisors As String = String.Empty
                    If oChannel.SubscribedSupervisors IsNot Nothing AndAlso oChannel.SubscribedSupervisors.Length > 0 Then strChannelSupervisors = String.Join(",", oChannel.SubscribedSupervisors)
                    Extensions.roAudit.AddParameter(tbParameters, "{ChannelTitle}", oChannel.Title, String.Empty, 1)
                    Extensions.roAudit.AddParameter(tbParameters, "{ChannelSuscribedEmployeesId}", strChannelEmployees, String.Empty, 1)
                    Extensions.roAudit.AddParameter(tbParameters, "{ChannelSuscribedGroupsId}", strChannelGroups, String.Empty, 1)
                    Extensions.roAudit.AddParameter(tbParameters, "{ChannelSuscribedSupervisorId}", strChannelSupervisors, String.Empty, 1)
                    Dim newChannelStatus As ChannelStatusEnum = CType(oChannel.Status, ChannelStatusEnum)
                    If bStatusChanged AndAlso Not bolIsNew Then
                        Dim oldChannelStatus As ChannelStatusEnum = CType(iOldStatus, ChannelStatusEnum)
                        Extensions.roAudit.AddParameter(tbParameters, "{ChannelStatus}", oState.Language.Translate("Channel.Status." & oldChannelStatus.ToString, "") & "->" & oState.Language.Translate("Channel.Status." & newChannelStatus.ToString, ""), String.Empty, 1)
                    Else
                        Extensions.roAudit.AddParameter(tbParameters, "{ChannelStatus}", oState.Language.Translate("Channel.Status." & newChannelStatus.ToString, ""), String.Empty, 1)
                    End If
                    Me.oState.Audit(If(bolIsNew, Audit.Action.aInsert, Audit.Action.aUpdate), Audit.ObjectType.tChannel, oChannel.Title, tbParameters, -1)
                End If
            Catch ex As DbException
                oState.Result = ChannelResultEnum.ErrorCreatingOrUpdatingChannel
                Me.oState.UpdateStateInfo(ex, "roChannelManager::CreateOrUpdateChannel")
            Catch ex As Exception
                oState.Result = ChannelResultEnum.ErrorCreatingOrUpdatingChannel
                Me.oState.UpdateStateInfo(ex, "roChannelManager::CreateOrUpdateChannel")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, oState.Result = ChannelResultEnum.NoError)
            End Try
            Return bolRet
        End Function

        ''' <summary>
        '''Marks a channel as deleted. Security checked.
        ''' </summary>
        ''' <param name="oChannel"></param>
        ''' <param name="bAudit"></param>
        ''' <returns></returns>
        Public Function DeleteChannel(oChannel As roChannel, Optional bAudit As Boolean = False) As Boolean
            Dim bolRet As Boolean = False
            Dim strSQL As String = String.Empty

            Try
                If oChannel.Id <= 0 Then Return True

                If (oChannel.IsComplaintChannel AndAlso Not hasComplaintsLicense) OrElse (Not oChannel.IsComplaintChannel AndAlso Not hasChannelsLicense) Then
                    Me.oState.Result = ChannelResultEnum.NoPermission
                    Return False
                End If

                ' Verificamos permiso
                Dim iPermissionOverChannels As Integer = WLHelper.GetPermissionOverFeature(Me.oState.IDPassport, "Employees.Channels", "U")
                If iPermissionOverChannels < 6 OrElse (iPermissionOverChannels = 6 AndAlso oChannel.CreatedBy <> Me.oState.IDPassport) Then
                    Me.oState.Result = ChannelResultEnum.NoPermission
                    Return False
                End If

                oState.Result = ChannelResultEnum.ErrorDeletingChannel

                strSQL = $"@UPDATE# Channels SET Deleted = 1, IdDeletedBy = {oState.IDPassport}, DeletedOn = {roTypes.Any2Time(Date.Now).SQLSmallDateTime()} WHERE Id = {oChannel.Id}"
                bolRet = ExecuteSql(strSQL)

                If bolRet AndAlso bAudit Then
                    ' Auditamos
                    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    Extensions.roAudit.AddParameter(tbParameters, "{ChannelTitle}", oChannel.Title, String.Empty, 1)
                    Me.oState.Audit(Audit.Action.aDelete, Audit.ObjectType.tChannel, oChannel.Title, tbParameters, -1)
                End If

                oState.Result = ChannelResultEnum.NoError
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roChannelManager::DeleteChannel")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roChannelManager::DeleteChannel")
            End Try
            Return bolRet
        End Function

        Public Function GetComplaintChannel(Optional ByVal bAudit As Boolean = False, Optional bLoadLists As Boolean = True, Optional viewerIdUser As Integer = 0) As roChannel
            Dim bRet As roChannel = Nothing
            Try
                If Not hasComplaintsLicense Then
                    Me.oState.Result = ChannelResultEnum.ErrorRecoveringChannel
                Else
                    Dim strSQL As String = String.Empty
                    strSQL = "@SELECT# Id FROM [dbo].[Channels] WHERE IsComplaintChannel = 1 AND Status = 1"
                    Dim id As Integer = roTypes.Any2Integer(ExecuteScalar(strSQL))
                    If id > 0 Then
                        bRet = GetChannelById(id, bAudit, bLoadLists, viewerIdUser)
                    Else
                        Me.oState.Result = ChannelResultEnum.ErrorRecoveringChannel
                    End If
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roChannelManager::GetComplaintChannel")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roChannelManager::GetComplaintChannel")
            End Try
            Return bRet
        End Function

        Public Function DeleteOldMessagesByComplexity(ByVal complexity As ConversationComplexity, ByVal limitDate As DateTime) As Boolean
            Dim bRet = True
            Try
                Dim sQuery As String = "@SELECT# c.id " &
                                                        "FROM Channels " &
                                                        "INNER JOIN ChannelConversations c ON channels.ID = c.IDChannel " &
                                                        "INNER JOIN ChannelConversationMessages m ON c.id = m.idConversation " &
                                                        "WHERE channels.IsComplaintChannel = 1 AND c.Complexity = " & CInt(complexity) & " " &
                                                        "AND cast(m.CreatedOn as date) < cast('" & limitDate.ToString("yyyy-MM-dd") & "' as date) "

                Dim dt As DataTable = CreateDataTable(sQuery, )
                If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                    For Each oRowaux As DataRow In dt.Rows
                        Dim idConversation As Integer = roTypes.Any2Integer(oRowaux("ID"))
                        If bRet Then bRet = ExecuteSql("@DELETE# FROM ChannelConversationMessages WHERE idConversation = " & idConversation) 'Eliminamos mensajes antiguos
                        If bRet Then bRet = ExecuteSql("@DELETE# FROM ChannelConversations WHERE id = " & idConversation) 'Eliminamos la conversación
                    Next
                End If

            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roChannelManager::DeleteOldMessagesByComplexity")
                bRet = False
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roChannelManager::DeleteOldMessagesByComplexity")
                bRet = False
            End Try
            Return bRet
        End Function

#End Region

#Region "Helper"

        Private Function GetNewMessagesOnChannel(idChannel As Integer, viewerIdUser As Integer) As Integer
            Dim ret As Integer = 0
            Try
                Dim sSQL As String = String.Empty
                Dim bAsUser As Boolean = (viewerIdUser > 0 OrElse viewerIdUser = -1)

                sSQL = $"@SELECT# COUNT(*) FROM Channels INNER JOIN ChannelConversations ON Channels.Id = ChannelConversations.IdChannel {(IIf(bAsUser, " AND ChannelConversations.CreatedBy = " & viewerIdUser.ToString(), ""))}INNER JOIN ChannelConversationMessages ON ChannelConversationMessages.IdConversation = ChannelConversations.id WHERE Channels.Id = {idChannel} And ChannelConversationMessages.Status = 0 AND {(IIf(bAsUser, " ChannelConversationMessages.IdSupervisor > 0 ", $" (ChannelConversationMessages.IdEmployee > {viewerIdUser} OR ChannelConversationMessages.IdEmployee = -1)"))}"

                ret = roTypes.Any2Integer(ExecuteScalar(sSQL))
            Catch ex As DbException
                oState.Result = ChannelResultEnum.ErrorRecoveringNewMessagesInChannel
                Me.oState.UpdateStateInfo(ex, "roChannelManager::GetNewMessagesOnChannel")
            Catch ex As Exception
                oState.Result = ChannelResultEnum.ErrorRecoveringNewMessagesInChannel
                Me.oState.UpdateStateInfo(ex, "roChannelManager::GetNewMessagesOnChannel")
            End Try
            Return ret
        End Function

        Private Function GetOpenConversationsOnChannel(idChannel As Integer) As Integer
            Dim ret As Integer = 0
            Try
                Dim sSQL As String = String.Empty

                sSQL = $"@SELECT# COUNT(*) FROM Channels INNER JOIN ChannelConversations ON Channels.Id = ChannelConversations.IdChannel WHERE Channels.Id = {idChannel} And ChannelConversations.Status IN (0,1) "

                ret = roTypes.Any2Integer(ExecuteScalar(sSQL))
            Catch ex As DbException
                oState.Result = ChannelResultEnum.ErrorRecoveringOpenConversationsInChannel
                Me.oState.UpdateStateInfo(ex, "roChannelManager::GetOpenConversationsOnChannel")
            Catch ex As Exception
                oState.Result = ChannelResultEnum.ErrorRecoveringOpenConversationsInChannel
                Me.oState.UpdateStateInfo(ex, "roChannelManager::GetOpenConversationsOnChannel")
            End Try
            Return ret
        End Function

#End Region

    End Class

End Namespace