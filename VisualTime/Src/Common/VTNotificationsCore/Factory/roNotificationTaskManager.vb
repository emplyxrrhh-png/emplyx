Imports System.Data.Common
Imports System.Security
Imports Robotics.Azure
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTNotificationsCore.Notifications
Imports Robotics.DataLayer
Imports Robotics.Security
Imports Robotics.Security.Base
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions

Public MustInherit Class roNotificationTaskManager
    Implements IDisposable

    Private _sGUID As String
    Private disposedValue As Boolean

    Protected ReadOnly Property NotificationType As eNotificationType
    Protected _oNotificationTask As roNotificationTask = Nothing
    Protected defaultLanguage As String = "ESP"
    Protected bSendAllNotificationsBetweenSupervisor As Boolean
    Protected sCustomizationCode As String
    Protected strDisableNotificationUserField As String = String.Empty
    Protected strDisabledNotificationsList As Generic.List(Of String) = Nothing

    Protected bSendNotificationToSupervisor As Boolean = False

    Sub New(ByVal _notificationType As eNotificationType, ByVal sGUID As String)
        Me.NotificationType = _notificationType

        _sGUID = sGUID

        defaultLanguage = "ESP"
        bSendAllNotificationsBetweenSupervisor = (roCacheManager.GetInstance().GetAdvParametersCache(RoAzureSupport.GetCompanyName(), "OnlyFirstNotificationBetweenSupervisors") = "0")
        sCustomizationCode = roCacheManager.GetInstance().GetAdvParametersCache(RoAzureSupport.GetCompanyName(), "Customization")

        strDisableNotificationUserField = roCacheManager.GetInstance().GetAdvParametersCache(RoAzureSupport.GetCompanyName(), "VTLive.Notification.DisableUserField")
        strDisabledNotificationsList = New Generic.List(Of String)(roCacheManager.GetInstance().GetAdvParametersCache(RoAzureSupport.GetCompanyName(), "VTLive.Notification.DisabledNotificationsList").Split(","))

    End Sub

    Protected MustOverride Function GetIdEmployee() As Integer

    Protected MustOverride Function GetIdPassport() As Integer

    Protected MustOverride Function MustSendPushNotification() As Boolean

    Protected MustOverride Function GetNotificationAvailableDestinations() As roNotificationDestinationConfig()

    Protected MustOverride Function GetTranslatedNotificationTexts(ByVal oConf As roNotificationDestinationConfig) As roNotificationItem

    Protected MustOverride Function PostSendAction() As Boolean

    Public MustOverride Function GenerateNotificationTasks(ByVal ads As DataRow) As Boolean

    Protected Function GetDefaultDestinationConfig(Optional ByVal bAddEmailFromList As Boolean = True, Optional ByVal bAddDestinationUserField As Boolean = True, Optional ByVal bAddDirectSupervisor As Boolean = True, Optional ByVal bCheckDefaultUserField As Boolean = False, Optional ByVal bAddDocumentSupervisor As Boolean = False, Optional ByVal bAddSupervisorLevel As Boolean = False, Optional ByVal bAddChannelSupervisor As Boolean = False) As roNotificationDestinationConfig()
        Dim oConfigs As New Generic.List(Of roNotificationDestinationConfig)

        If bAddEmailFromList Then
            'Añadimos el destino si han seleccionado una dirección de la lista
            If _oNotificationTask.Destination.Exists("Destination") Then
                Dim strDestination = Trim(_oNotificationTask.Destination("Destination").ToString)

                For Each email As String In strDestination.Split(";")
                    Dim oConf = roNotificationHelper.CreateNewDestination(email, defaultLanguage, -1, -1, False, False)
                    If oConf IsNot Nothing Then oConfigs.Add(oConf)
                Next
            End If
        End If

        If bAddDestinationUserField Then
            ' Añadimos el destino si han seleccionado a un campo de la ficha del empleado
            Dim strFieldName As String = String.Empty
            If _oNotificationTask.Conditions.Exists("MailListUserfield") Then
                strFieldName = _oNotificationTask.Conditions("MailListUserfield").ToString
            End If

            If bCheckDefaultUserField OrElse (Not bCheckDefaultUserField AndAlso strFieldName.Length > 0) Then
                Dim oUserConf As roNotificationDestinationConfig = GetEmailConfiguration(GetIdEmployee(), GetIdPassport(), strFieldName, True)
                If oUserConf IsNot Nothing Then oConfigs.Add(oUserConf)
            End If
        End If

        If bAddDirectSupervisor Then
            ' Añadimos el destino si han seleccionado "Al supervisor directo mediante correo"
            If Not (NotificationType = eNotificationType.Request_Pending AndAlso _oNotificationTask.RequestType = eRequestType.ExchangeShiftBetweenEmployees) AndAlso _oNotificationTask.AllowMail Then
                Dim idsDirectSupervisor As String
                idsDirectSupervisor = GetDirectSupervisorNotification(NotificationType, GetIdEmployee(), _oNotificationTask.RequestType, _oNotificationTask.IdRequest, bSendAllNotificationsBetweenSupervisor, _oNotificationTask.Parameters, 0, 0)

                If idsDirectSupervisor.Length > 0 Then
                    For Each idPassport As String In idsDirectSupervisor.Split(";")
                        If idPassport <> String.Empty Then
                            bSendNotificationToSupervisor = True
                            Dim oUserConf As roNotificationDestinationConfig = GetEmailConfiguration(-1, roTypes.Any2Integer(idPassport), "", False)
                            If oUserConf IsNot Nothing Then oConfigs.Add(oUserConf)
                        End If
                    Next
                End If
            End If
        End If

        If bAddDocumentSupervisor Then
            If (NotificationType <> eNotificationType.Document_Pending AndAlso _oNotificationTask.IDDocument > 0) OrElse
            (NotificationType = eNotificationType.Document_Pending AndAlso _oNotificationTask.CheckNotificationRepeat AndAlso _oNotificationTask.Repetition >= 0) Then
                Dim idsDirectSupervisor As String = GetDirectSupervisorNotification(NotificationType, GetIdEmployee(), 0, 0, bSendAllNotificationsBetweenSupervisor, _oNotificationTask.Parameters, _oNotificationTask.IDDocument, 0)

                If idsDirectSupervisor.Length > 0 Then
                    For Each idPassport As String In idsDirectSupervisor.Split(";")
                        If idPassport <> String.Empty Then
                            bSendNotificationToSupervisor = True
                            Dim oUserConf As roNotificationDestinationConfig = GetEmailConfiguration(-1, roTypes.Any2Integer(idPassport), "", False)
                            If oUserConf IsNot Nothing Then oConfigs.Add(oUserConf)
                        End If
                    Next
                End If
            End If
        End If

        If bAddSupervisorLevel Then
            Dim sRole As String = String.Empty
            If _oNotificationTask.Conditions.Exists("ConditionRole") Then
                sRole = roTypes.Any2String(_oNotificationTask.Conditions("ConditionRole"))
            End If
            If sRole <> "" Then
                Dim idsDirectSupervisor As String = GetSupervisorWithRoleNotification(NotificationType, GetIdEmployee(), sRole)
                If idsDirectSupervisor.Length > 0 Then
                    For Each idPassport As String In idsDirectSupervisor.Split(";")
                        If idPassport <> String.Empty Then
                            bSendNotificationToSupervisor = True
                            Dim oUserConf As roNotificationDestinationConfig = GetEmailConfiguration(-1, roTypes.Any2Integer(idPassport), "", False)
                            If oUserConf IsNot Nothing Then oConfigs.Add(oUserConf)
                        End If
                    Next
                End If
            End If
        End If

        If bAddChannelSupervisor Then
            Dim idsDirectSupervisor As String = GetDirectSupervisorNotification(NotificationType, GetIdEmployee(), 0, 0, False, _oNotificationTask.Parameters, 0, _oNotificationTask.IDChannel)

            If idsDirectSupervisor.Length > 0 Then
                For Each idPassport As String In idsDirectSupervisor.Split(";")
                    If idPassport <> String.Empty Then
                        Dim oUserConf As roNotificationDestinationConfig = GetEmailConfiguration(-1, roTypes.Any2Integer(idPassport), "", False)
                        If oUserConf IsNot Nothing Then oConfigs.Add(oUserConf)
                    End If
                Next
            End If
        End If

        Return oConfigs.ToArray()
    End Function

    Protected Function PreparePushNotificationMessage(ByVal strMessage As String) As String
        Return VTBase.roSupport.Sanitize(strMessage)
    End Function

    Public Function Send(ByVal oRow As DataRow) As Boolean

        If oRow Is Nothing OrElse Not Me.Load(oRow) Then Return False

        Dim bSend As Boolean = True

        If _oNotificationTask Is Nothing Then
            'Maracamos la notificación como enviada ya que no existen los datos asociados a la request.
            DataLayer.AccessHelper.ExecuteSql($"@UPDATE# sysroNotificationTasks SET Executed= 1, InProgress = 0 WHERE ID= {roTypes.Any2Integer(oRow("ID"))}")
            roLog.GetInstance.logMessage(roLog.EventType.roError, "roNotificationTaskManager::Send::could not load notification info")
            Return False
        End If

        Dim oAvaliableDestinations As roNotificationDestinationConfig() = GetNotificationAvailableDestinations()

        Dim bSkipDueToPermissionCheck As Boolean = False
        If Me.bSendNotificationToSupervisor Then
            'Tenemos que comprovar si hay alguna tarea de permisos en curso. Si hay alguna tarea no podemos generar los destinatarios hasta que hayan terminado.
            bSkipDueToPermissionCheck = (roTypes.Any2Integer(DataLayer.AccessHelper.ExecuteScalar("@SELECT# count(*) from sysrolivetasks where Action='SECURITYPERMISSIONS' AND Status = 1")) > 0)
        End If

        If Not bSkipDueToPermissionCheck Then

            DataLayer.AccessHelper.ExecuteSql($"@UPDATE# sysroNotificationTasks SET DestinationsNotified = {oAvaliableDestinations.Length} WHERE ID= {_oNotificationTask.Id}")

            For Each oConf As roNotificationDestinationConfig In oAvaliableDestinations

                Dim oItem As roNotificationItem = GetTranslatedNotificationTexts(oConf)
                Dim bPushSent As Boolean = True
                Dim bMailSend As Boolean

                If oItem IsNot Nothing AndAlso (oItem.Body.Length > 0 OrElse oItem.Content.Length > 0) AndAlso Not IsNotificationExcluded(oConf) Then
                    'Envio push si toca
                    bPushSent = True
                    If MustSendPushNotification() AndAlso oItem.Type = NotificationItemType.email AndAlso (oConf.IdUser.IdPassport > 0) Then
                        bPushSent = False
                        If oConf.IdUser.IdPassport > 0 Then bPushSent = VTBase.Extensions.roPushNotification.SendNotificationPushToPassport(oConf.IdUser.IdPassport, LoadType.Passport, Me.PreparePushNotificationMessage(oItem.Subject), Me.PreparePushNotificationMessage(oItem.Body))

                        If bPushSent Then
                            roLog.GetInstance().logMessage(roLog.EventType.roDebug, "CNotificationServer::ExecuteSendPushNotifications::" & _oNotificationTask.Id & "::SentMessageToQueue")
                        Else
                            roLog.GetInstance().logMessage(roLog.EventType.roDebug, "CNotificationServer::ExecuteSendPushNotifications::" & _oNotificationTask.Id & "::ErrorSendingMessageToQueue")
                        End If
                    ElseIf oItem.Type = NotificationItemType.push AndAlso (oConf.IdUser.IdEmployee > 0) AndAlso MustSendPushNotification() Then
                        bPushSent = False
                        If oConf.IdUser.IdEmployee > 0 Then bPushSent = VTBase.Extensions.roPushNotification.SendNotificationPushToPassport(oConf.IdUser.IdEmployee, LoadType.Employee, Me.PreparePushNotificationMessage(oItem.Subject), Me.PreparePushNotificationMessage(oItem.Body))

                        If bPushSent Then
                            roLog.GetInstance().logMessage(roLog.EventType.roDebug, "CNotificationServer::ExecuteSendPushNotifications::" & _oNotificationTask.Id & "::SentMessageToQueue")
                        Else
                            roLog.GetInstance().logMessage(roLog.EventType.roDebug, "CNotificationServer::ExecuteSendPushNotifications::" & _oNotificationTask.Id & "::ErrorSendingMessageToQueue")
                        End If
                    ElseIf oItem.Type = NotificationItemType.push Then
                        bPushSent = False
                    End If

                    If oItem.Type <> NotificationItemType.push Then
                        'Si el email va a un empleado en concreto añadimos el mensaje de privacidad
                        If oConf.AddEmployeeWarning AndAlso GetIdEmployee() > 0 Then
                            Dim Params As New ArrayList From {
                            AccessHelper.ExecuteScalar("@SELECT# Name from Employees where ID = " & GetIdEmployee())
                        }
                            oItem.Body = oItem.Body & " </br></br> " & roNotificationHelper.Message("Notification.Message.EndBody.EmployeeNameAdvice", Params, , oConf.Language)
                        Else
                            Dim Params As New ArrayList
                            oItem.Body = oItem.Body & " </br></br> " & roNotificationHelper.Message("Notification.Message.EndBody.SecurityAdvice", Params, , oConf.Language)
                        End If

                        'Añadimos la coletilla legal
                        If oItem.Body.Length > 0 Then
                            oItem.Body = oItem.Body & " </br></br> " & roNotificationHelper.Message("Notification.Message.EndBody", , , oConf.Language)
                        End If

                        'Envio del item.
                        bMailSend = Azure.RoAzureSupport.SendTaskToQueue(_oNotificationTask.Id, Azure.RoAzureSupport.GetCompanyName(), roLiveTaskTypes.SendEmail, VTBase.roJSONHelper.SerializeNewtonSoft(oItem))
                        If bMailSend Then
                            roLog.GetInstance().logMessage(roLog.EventType.roDebug, "CNotificationServer::ExecuteSendNotifications::" & _oNotificationTask.Id & "::SentMessageToQueue")
                        Else
                            roLog.GetInstance().logMessage(roLog.EventType.roDebug, "CNotificationServer::ExecuteSendNotifications::" & _oNotificationTask.Id & "::ErrorSendingMessageToQueue")
                        End If

                        'Log si ha ido bien
                        If bMailSend AndAlso bPushSent Then
                            ' En el caso que sea una notificación que lleve una contraseña, eliminamos dicha informacion en el log
                            Select Case NotificationType
                                Case eNotificationType.Advice_For_New_password, eNotificationType.Advice_For_Password_Recover, eNotificationType.Advice_For_validation_code, eNotificationType.Advice_For_Send_Username : oItem.Body = ""
                            End Select

                            If GetIdEmployee() <> -1 Then
                                Dim strLogMsg As String = "Se ha enviado la notificación(" & _oNotificationTask.Id & "):'" & _oNotificationTask.NotificationName & "' a las " & If(_oNotificationTask.FiredDate.HasValue AndAlso _oNotificationTask.FiredDate = Date.MinValue, DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), _oNotificationTask.FiredDate.Value.ToString("dd/MM/yyyy HH:mm:ss")) &
                                          " al empleado con id: " & GetIdEmployee() & "."
                                roLog.GetInstance().logMessage(roLog.EventType.roDebug, "CNotificationServer::Notification sent:Instance " & _oNotificationTask.GUID.ToString & ": " & strLogMsg)
                            Else
                                Dim strLogMsg As String = "Se ha enviado la notificación(" & _oNotificationTask.Id & "):'" & _oNotificationTask.NotificationName & "' a las " & If(_oNotificationTask.FiredDate.HasValue AndAlso _oNotificationTask.FiredDate = Date.MinValue, DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), _oNotificationTask.FiredDate.Value.ToString("dd/MM/yyyy HH:mm:ss")) & "."

                                roLog.GetInstance().logMessage(roLog.EventType.roDebug, "CNotificationServer::Notification sent:Instance " & _oNotificationTask.GUID.ToString & ": " & strLogMsg)
                            End If

                        End If
                    Else
                        'Log si ha ido bien
                        If bPushSent Then
                            bMailSend = True
                            If GetIdEmployee() <> -1 Then
                                Dim strLogMsg As String = "Se ha enviado la notificación(" & _oNotificationTask.Id & "):'" & _oNotificationTask.NotificationName & "' a las " & If(_oNotificationTask.FiredDate.HasValue AndAlso _oNotificationTask.FiredDate = Date.MinValue, DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), _oNotificationTask.FiredDate.Value.ToString("dd/MM/yyyy HH:mm:ss")) &
                                          " al empleado con id: " & GetIdEmployee() & "."
                                roLog.GetInstance().logMessage(roLog.EventType.roDebug, "CNotificationServer::Notification sent:Instance " & _oNotificationTask.GUID.ToString & ": " & strLogMsg)
                            Else
                                Dim strLogMsg As String = "Se ha enviado la notificación(" & _oNotificationTask.Id & "):'" & _oNotificationTask.NotificationName & "' a las " & If(_oNotificationTask.FiredDate.HasValue AndAlso _oNotificationTask.FiredDate = Date.MinValue, DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), _oNotificationTask.FiredDate.Value.ToString("dd/MM/yyyy HH:mm:ss")) & "."

                                roLog.GetInstance().logMessage(roLog.EventType.roDebug, "CNotificationServer::Notification sent:Instance " & _oNotificationTask.GUID.ToString & ": " & strLogMsg)
                            End If

                        End If
                    End If
                Else
                    bMailSend = True
                    bPushSent = True
                End If

                bSend = bSend AndAlso bMailSend AndAlso bPushSent
            Next

            bSend = bSend AndAlso PostSendAction()
        Else
            bSend = False
        End If

        'Marcar como enviada la notificacion
        If Not bSend Then
            ' Si hubo un error, y solo si fue enviando la notificación, marco para que se vuelva a procesar
            DataLayer.AccessHelper.ExecuteSql("@UPDATE# sysroNotificationTasks SET Executed= 0, InProgress = 0 WHERE ID=" & _oNotificationTask.Id)

            If Not bSkipDueToPermissionCheck Then
                roLog.GetInstance().logMessage(roLog.EventType.roDebug, "CNotificationServer::roNotificationSendManager:Error sending email notification(" & _oNotificationTask.Id & ") to some of " & oAvaliableDestinations.Length.ToString & " destinations. Will retry!")
            Else
                roLog.GetInstance().logMessage(roLog.EventType.roDebug, "CNotificationServer::roNotificationSendManager:Skip notification(" & _oNotificationTask.Id & ") due to permissions beeing modified. Will retry!")
            End If
        Else
            If oAvaliableDestinations.Length > 0 Then
                roLog.GetInstance().logMessage(roLog.EventType.roDebug, "CNotificationServer::roNotificationSendManager:End sending email notification(" & _oNotificationTask.Id & ") to " & oAvaliableDestinations.Length.ToString & " destinations")
            End If

            ' Marcamos como enviada la notificacion
            DataLayer.AccessHelper.ExecuteSql($"@UPDATE# sysroNotificationTasks SET Executed= 1, InProgress = 0, DestinationsNotified = {oAvaliableDestinations.Length} WHERE ID= {_oNotificationTask.Id}")
        End If

        Return bSend
    End Function

    Private Function IsNotificationExcluded(ByVal oConf As roNotificationDestinationConfig) As Boolean
        Dim bExcludeNotificacion As Boolean = False
        Try
            Dim strSQL As String = String.Empty

            If oConf.IdUser.IdEmployee > 0 Then
                If strDisabledNotificationsList.Contains(roTypes.Any2String(CInt(Me.NotificationType))) Then
                    strSQL = "@DECLARE# @Date smalldatetime " &
                                             "SET @Date = " & roTypes.Any2Time(Now.Date).SQLSmallDateTime & " " &
                                             "@SELECT# * FROM GetEmployeeUserFieldValue(" & roTypes.Any2String(oConf.IdUser.IdEmployee) & ",'" & strDisableNotificationUserField & "', @Date)"
                    Dim tbs As DataTable = AccessHelper.CreateDataTable(strSQL)
                    If tbs IsNot Nothing AndAlso tbs.Rows.Count > 0 Then
                        If roTypes.Any2String(tbs.Rows(0).Item("Value").ToString) = "1" Then bExcludeNotificacion = True
                    End If
                End If

                ' Sólo envío notificaciones referentes a empleados con contrato
                If Not bExcludeNotificacion Then
                    Dim oEmployee As New VTEmployees.Employee.roEmployee
                    oEmployee = Robotics.Base.VTEmployees.Employee.roEmployee.GetEmployee(oConf.IdUser.IdEmployee, New VTEmployees.Employee.roEmployeeState(-1))
                    If Not oEmployee Is Nothing Then bExcludeNotificacion = (oEmployee.EmployeeStatus = DTOs.EmployeeStatusEnum.Old)
                    'If contract is Old, check if has future Contract, Notification recovers password/user and can login without contract
                    If bExcludeNotificacion AndAlso (Me.NotificationType = eNotificationType.Advice_For_Password_Recover OrElse Me.NotificationType = eNotificationType.Advice_For_New_password OrElse
                            Me.NotificationType = eNotificationType.Advice_For_Send_Username) AndAlso roPassportManager.HasReadModeEnabled(oConf.IdUser.IdEmployee) Then
                        bExcludeNotificacion = False
                    End If
                End If
            End If
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "CNotificationServer::Work:Error cheking if notifications should be excluded:" & _oNotificationTask.Id, ex)
            bExcludeNotificacion = False
        End Try

        Return bExcludeNotificacion
    End Function

    Public Function Load(ByVal oNotificationRow As DataRow) As Boolean
        Dim bLoaded As Boolean = True

        _oNotificationTask = New roNotificationTask With {
            .Id = roTypes.Any2Integer(oNotificationRow("ID")),
            .IdNotification = roTypes.Any2Integer(oNotificationRow("IDNotification")),
            .Key1Numeric = roTypes.Any2Integer(oNotificationRow("Key1Numeric")),
            .key2Numeric = roTypes.Any2Integer(oNotificationRow("Key2Numeric")),
            .key5Numeric = roTypes.Any2Integer(oNotificationRow("Key5Numeric")),
            .key3Datetime = If(IsDBNull(oNotificationRow("Key3DateTime")), Nothing, roTypes.Any2DateTime(oNotificationRow("Key3DateTime"))),
            .key4Datetime = If(IsDBNull(oNotificationRow("Key4DateTime")), Nothing, roTypes.Any2DateTime(oNotificationRow("Key4DateTime"))),
            .key6Datetime = If(IsDBNull(oNotificationRow("Key6DateTime")), Nothing, roTypes.Any2DateTime(oNotificationRow("Key6DateTime"))),
            .Executed = roTypes.Any2Boolean(oNotificationRow("Executed")),
            .IsReaded = roTypes.Any2Boolean(oNotificationRow("IsReaded")),
            .Parameters = roTypes.Any2String(oNotificationRow("Parameters")),
            .GUID = roTypes.Any2String(oNotificationRow("GUID")),
            .Repetition = If(IsDBNull(oNotificationRow("Repetition")), -1, roTypes.Any2Integer(oNotificationRow("Repetition"))),'roTypes.Any2Integer(oNotificationRow("Repetition")),
            .NextRepetition = If(IsDBNull(oNotificationRow("NextRepetition")), Nothing, roTypes.Any2DateTime(oNotificationRow("NextRepetition"))),
            .Conditions = New roCollection(oNotificationRow("Condition")),
            .Destination = New roCollection(oNotificationRow("Destination")),
            .AllowMail = roTypes.Any2Boolean(oNotificationRow("AllowMail")),
            .FiredDate = If(IsDBNull(oNotificationRow("FiredDate")), Nothing, roTypes.Any2DateTime(oNotificationRow("FiredDate"))),
            .NotificationName = roTypes.Any2String(oNotificationRow("NotificationName"))
            }

        If _sGUID <> String.Empty Then
            _oNotificationTask.GUID = _sGUID
        End If

        _oNotificationTask.RequestType = eRequestType.None
        _oNotificationTask.IdRequest = -1
        _oNotificationTask.IDDocument = -1
        _oNotificationTask.IDChannel = -1
        _oNotificationTask.DocumentScope = DocumentScope.EmployeeContract
        _oNotificationTask.CheckNotificationRepeat = False

        If NotificationType = eNotificationType.Request_Pending OrElse NotificationType = eNotificationType.Request_Impersonation OrElse NotificationType = eNotificationType.Absence_Canceled_By_User Then

            Dim dtRequestInfo As DataTable = DataLayer.AccessHelper.CreateDataTable("@SELECT# RequestType FROM Requests WHERE ID = " & _oNotificationTask.Key1Numeric)

            If dtRequestInfo IsNot Nothing AndAlso dtRequestInfo.Rows.Count > 0 Then
                _oNotificationTask.RequestType = roTypes.Any2Double(dtRequestInfo.Rows(0)("RequestType"))
                _oNotificationTask.IdRequest = _oNotificationTask.Key1Numeric
            Else
                _oNotificationTask = Nothing
            End If

        ElseIf NotificationType = eNotificationType.Requests_Result Then
            Dim dtRequestInfo As DataTable = DataLayer.AccessHelper.CreateDataTable("@SELECT# RequestType FROM Requests WHERE ID = " & _oNotificationTask.key2Numeric)

            If dtRequestInfo IsNot Nothing AndAlso dtRequestInfo.Rows.Count > 0 Then
                _oNotificationTask.RequestType = roTypes.Any2Double(dtRequestInfo.Rows(0)("RequestType"))
                _oNotificationTask.IdRequest = _oNotificationTask.key2Numeric
            Else
                _oNotificationTask = Nothing
            End If

        ElseIf NotificationType = eNotificationType.Document_delivered OrElse NotificationType = eNotificationType.Document_Pending OrElse NotificationType = eNotificationType.Document_Pending_For_Employee OrElse NotificationType = eNotificationType.Document_rejected Then
            _oNotificationTask.IDDocument = _oNotificationTask.key2Numeric
            If NotificationType <> eNotificationType.Document_Pending_For_Employee Then
                _oNotificationTask.DocumentScope = roTypes.Any2Double(DataLayer.AccessHelper.ExecuteScalar("@SELECT# Scope from DocumentTemplates where id = (@SELECT# IdDocumentTemplate from Documents where Id = " & _oNotificationTask.key2Numeric & ")"))
                If NotificationType = eNotificationType.Document_Pending AndAlso (_oNotificationTask.DocumentScope = DocumentScope.CauseNote OrElse _oNotificationTask.DocumentScope = DocumentScope.LeaveOrPermission) Then
                    If roTypes.Any2Integer(roCacheManager.GetInstance().GetAdvParametersCache(RoAzureSupport.GetCompanyName(), "DocumentPendingNotification.RepeatTimes")) > 0 AndAlso
                            roTypes.Any2Integer(roCacheManager.GetInstance().GetAdvParametersCache(RoAzureSupport.GetCompanyName(), "DocumentPendingNotification.RepeatEachDays")) > 0 Then
                        _oNotificationTask.CheckNotificationRepeat = True
                    End If
                End If
            End If
        ElseIf NotificationType = eNotificationType.NewMessage_FromEmployee_InChannel OrElse NotificationType = eNotificationType.Advice_For_NewConversation Then
            _oNotificationTask.IDChannel = _oNotificationTask.key2Numeric
        End If

        Return bLoaded
    End Function

    Protected Function GetEmailConfiguration(idEmployee As Long, idPassport As Long, strFieldName As String, bEmployeeNotification As Boolean) As roNotificationDestinationConfig
        Dim strDestinationFieldName As String = String.Empty
        Dim oRet As roNotificationDestinationConfig = Nothing
        Dim employeeLanguage As String = defaultLanguage
        Dim bUseFixedFieldName As Boolean = True
        Dim bIsRoboticsUser As Boolean = False

        Try
            If strFieldName = String.Empty Then
                bUseFixedFieldName = False

                Dim strSQL As String = "@SELECT# FieldName from sysroUserFields where Alias = 'sysroEmail'"
                strFieldName = roTypes.Any2String(DataLayer.AccessHelper.ExecuteScalar(strSQL))
            End If

            If strFieldName.Length > 0 Then

                Dim tbs As DataTable = Nothing

                Dim strSQL As String = "@SELECT# ISNULL(ID, 0) AS id, isnull(Email, '') AS Email, isnull(IDEmployee, 0) as IDEmployee, Description FROM sysroPassports "

                If idEmployee > 0 Then
                    strSQL = strSQL & "where IDEmployee =" + idEmployee.ToString
                Else
                    strSQL = strSQL & "where ID =" + idPassport.ToString
                End If

                Dim tbPassports As DataTable = DataLayer.AccessHelper.CreateDataTable(strSQL)
                If tbPassports.Rows.Count > 0 Then
                    ' Intetamos obtener el mail del empleado
                    idEmployee = roTypes.Any2Double(tbPassports.Rows(0)("IDEmployee"))
                    idPassport = roTypes.Any2Double(tbPassports.Rows(0)("ID"))

                    bIsRoboticsUser = roTypes.Any2String(tbPassports.Rows(0)("Description")).Contains("@@ROBOTICS@@")

                    If idEmployee > 0 Then
                        If strFieldName.Length > 0 Then
                            Dim strAux As String = "@DECLARE# @Date smalldatetime " &
                                         "SET @Date = " & roTypes.Any2Time(Now.Date).SQLSmallDateTime & " " &
                                         "@SELECT# * FROM GetEmployeeUserFieldValue(" & roTypes.Any2String(idEmployee) & ",'" & strFieldName & "', @Date)"
                            tbs = DataLayer.AccessHelper.CreateDataTable(strAux)
                            If tbs IsNot Nothing AndAlso tbs.Rows.Count > 0 Then
                                strDestinationFieldName = roTypes.Any2String(tbs.Rows(0).Item("Value").ToString).Trim
                            End If
                        End If
                    End If

                    If strDestinationFieldName.Length = 0 AndAlso bUseFixedFieldName = False Then
                        strDestinationFieldName = roTypes.Any2String(tbPassports.Rows(0)("Email")).Trim

                        If NotificationType = eNotificationType.Advice_For_temporary_blocked_account OrElse NotificationType = eNotificationType.Advice_For_temporary_blocked_account OrElse
                        NotificationType = eNotificationType.Advice_For_temporary_blocked_account OrElse NotificationType = eNotificationType.Advice_For_temporary_blocked_account Then
                            If _oNotificationTask.Parameters.Length > 0 Then strDestinationFieldName = _oNotificationTask.Parameters
                        End If
                    End If

                    ' No personalizamos pie del email
                    strSQL = "@SELECT# LanguageKey from sysroLanguages where ID = (@SELECT# idLanguage from sysroPassports where ID = " & idPassport & ")"
                    tbs = DataLayer.AccessHelper.CreateDataTable(strSQL)
                    If tbs IsNot Nothing AndAlso tbs.Rows.Count > 0 Then
                        employeeLanguage = roTypes.Any2String(tbs.Rows(0).Item("LanguageKey").ToString)
                        If employeeLanguage = String.Empty Then employeeLanguage = defaultLanguage
                    End If

                End If

                For Each email As String In strDestinationFieldName.Split(";")
                    If email <> String.Empty AndAlso Not email.Contains("@ReplacedBy_LOPD.COM") Then oRet = roNotificationHelper.CreateNewDestination(email, employeeLanguage, idEmployee, idPassport, bEmployeeNotification, bIsRoboticsUser)
                Next
            End If
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "roNotificationTaskManager::GetEmailConfiguration::", ex)
            oRet = Nothing
        End Try

        Return oRet
    End Function

    Protected Function GetDirectSupervisorNotification(ByVal IDType As Double, ByVal IDEmployee As Double, ByVal RequestType As Double, ByVal IDRequest As Long, ByVal bSendAllNotificationsBetweenSupervisor As Boolean, ByVal strParameters As String, ByVal idDocumentOrIdTemplate As Integer, ByVal idConversation As Integer) As String
        Dim strPassportIDs As String = ""
        Dim strPassports As String = "-1"

        Try
            Dim strFeatureAlias As String = ""
            Dim strFeatureType As String = ""
            Dim bRequiresPermissionOverEmployee As Boolean = False
            Dim bIsCompanyDoc As Boolean = False
            Dim oDocumenManager As VTDocuments.roDocumentManager
            Dim strSQL As String = ""
            Dim tbCheckSecurity As DataTable = Nothing

            strFeatureAlias = roTypes.Any2String(DataLayer.AccessHelper.ExecuteScalar("@SELECT# isnull(Feature, '') as Feature  FROM sysroNotificationTypes WHERE ID = " & IDType))
            strFeatureType = roTypes.Any2String(DataLayer.AccessHelper.ExecuteScalar("@SELECT# isnull(FeatureType, '') as FeatureType  FROM sysroNotificationTypes WHERE ID = " & IDType))
            bRequiresPermissionOverEmployee = roTypes.Any2Boolean(DataLayer.AccessHelper.ExecuteScalar("@SELECT# ISNULL(RequiresPermissionOverEmployees, 0) as RequiresPermission  FROM sysroNotificationTypes WHERE ID = " & IDType))

            ' Tratamiento en función de la notificación. Alguna requieren un flujo específico
            Select Case strFeatureAlias
                Case "*Channels*"
                    ' Si es de canal de comunicaciones
                    Dim bolIsComplaintChannel As Boolean = False

                    bolIsComplaintChannel = roTypes.Any2Boolean(DataLayer.AccessHelper.ExecuteScalar("@SELECT# isnull(iscomplaintChannel,0) from Channels where id = (@SELECT# idchannel from ChannelConversations where id = " & idConversation & ")"))
                    If bolIsComplaintChannel Then
                        ' Si es el canal de denuncias
                        ' obtenenmos todos los supervisores que tengan permisos Admin sobre la funcionalidad de canal de denuncias "Employees.Complaints"
                        strSQL = " @SELECT# sysrovwSecurity_PermissionOverFeatures.IdPassport ID FROM  sysrovwSecurity_PermissionOverFeatures With (NOLOCK) " &
                            " WHERE sysrovwSecurity_PermissionOverFeatures.IsRoboticsUser = 0 AND sysrovwSecurity_PermissionOverFeatures.FeatureAlias = 'Employees.Complaints' AND sysrovwSecurity_PermissionOverFeatures.Permission = 9 "
                    Else
                        ' Es un canal de comunicaciones
                        ' Obtenemos lo supervisores del canal y que tenga permisos como minimo de lectura sobre la funcionalidad de "Employees.Channels"
                        strSQL = "@SELECT# sysrovwSecurity_PermissionOverFeatures.IdPassport ID FROM sysrovwSecurity_PermissionOverFeatures With (NOLOCK)" &
                            " INNER JOIN ChannelSupervisors  With (NOLOCK) ON sysrovwSecurity_PermissionOverFeatures.IdPassport = ChannelSupervisors.IdSupervisor " &
                            "    AND sysrovwSecurity_PermissionOverFeatures.IsRoboticsUser = 0 " &
                            "    AND sysrovwSecurity_PermissionOverFeatures.FeatureAlias = 'Employees.Channels'  " &
                            "    AND sysrovwSecurity_PermissionOverFeatures.Permission >= 3  " &
                            " INNER JOIN ChannelConversations With (NOLOCK) On ChannelConversations.IdChannel = ChannelSupervisors.IdChannel and ChannelConversations.ID = " & idConversation
                    End If

                    tbCheckSecurity = DataLayer.AccessHelper.CreateDataTable(strSQL)
                Case "*Requests*"
                    If IDRequest > 0 Then
                        'En función de si la solicitud está aún pendiente, o está finalizada
                        Select Case IDType
                            Case eNotificationType.Absence_Canceled_By_User
                                strSQL = $"WITH RankedPassports AS (
                                                @SELECT#
                                                    IdPassport,
                                                    SupervisorLevelOfAuthority,
                                                    ROW_NUMBER() OVER (ORDER BY SupervisorLevelOfAuthority DESC) AS RowNum
                                                FROM
                                                    sysrovwSecurity_PermissionOverRequests
                                                WHERE IdRequest = {IDRequest} AND IsRoboticsUser = 0 AND RequestCurrentApprovalLevel >= SupervisorLevelOfAuthority
                                           )
                                           @SELECT#
                                               IdPassport
                                           FROM
                                               RankedPassports
                                           WHERE
                                               RowNum = 1"
                                strSQL &= SQLServerHint.GetSQLHint(SQLServerHint.SelectHinted.GetDirectSupervisorNotification)
                            Case Else
                                'Notificación para solicitudes pendientes (en este momento, únicamente la id 40 - Requests Pending)
                                strSQL = $"@SELECT# IdPassport FROM sysrovwSecurity_PendingRequestsDependencies WHERE IdRequest = {IDRequest} AND DirectDependence = 1 AND IsRoboticsUser = 0 "
                                strSQL &= SQLServerHint.GetSQLHint(SQLServerHint.SelectHinted.GetDirectSupervisorNotification)
                        End Select
                        tbCheckSecurity = DataLayer.AccessHelper.CreateDataTable(strSQL)
                    End If
                Case "*Documents*"
                    If idDocumentOrIdTemplate > 0 Then
                        ' Busco el ámbito del documento
                        oDocumenManager = New VTDocuments.roDocumentManager
                        If IDType = 49 OrElse IDType = 91 OrElse IDType = 52 Then
                            ' El id es de documento
                            Dim oDocument As New DTOs.roDocument
                            oDocument = oDocumenManager.LoadDocument(idDocumentOrIdTemplate)
                            idDocumentOrIdTemplate = oDocument.DocumentTemplate.Id
                            bIsCompanyDoc = (oDocument.DocumentTemplate.Scope = DocumentScope.Company OrElse oDocument.DocumentTemplate.Scope = DocumentScope.CompanyAccessAuthorization)
                        ElseIf IDType = 53 OrElse IDType = 50 Then
                            ' El id es de plantilla
                            Dim oDocumentTemplate As New DTOs.roDocumentTemplate
                            oDocumentTemplate = oDocumenManager.LoadDocumentTemplate(idDocumentOrIdTemplate)
                            bIsCompanyDoc = (oDocumentTemplate.Scope = DocumentScope.Company OrElse oDocumentTemplate.Scope = DocumentScope.CompanyAccessAuthorization)
                        End If

                        If Not bIsCompanyDoc Then
                            ' Si es de tipo empleado
                            strSQL = $"@SELECT# IdPassport FROM sysrofnSecurity_PermissionsOverDocumentTemplatesWithDependencies({idDocumentOrIdTemplate},{IDEmployee}, -1, 0) WHERE DirectDependence = 1 AND IsRoboticsUser = 0"
                            tbCheckSecurity = DataLayer.AccessHelper.CreateDataTable(strSQL)
                        Else
                            ' Si es un documento de tipo Empresa
                            Dim idCompany As Integer = IDEmployee
                            strSQL = $"@SELECT# IdPassport FROM sysrofnSecurity_PermissionsOverDocumentTemplatesWithDependencies({idDocumentOrIdTemplate},-1, {idCompany}, 0) WHERE DirectDependence = 1 AND IsRoboticsUser = 0"
                            tbCheckSecurity = DataLayer.AccessHelper.CreateDataTable(strSQL)
                        End If
                    End If
                Case Else
                    ' Notificaciones con flujo de notificación Estandar
                    If bRequiresPermissionOverEmployee Then
                        strSQL = $"@SELECT# IdPassport FROM sysrofnSecurity_GeneralNotificationsDependencies({IDType},{IDEmployee}) WHERE ShouldBeNotified = 1 "
                    Else
                        strSQL = $"@SELECT# IdPassport FROM sysrofnSecurity_GeneralNotificationsDependencies({IDType},0) WHERE ShouldBeNotified = 1 "
                    End If
                    tbCheckSecurity = DataLayer.AccessHelper.CreateDataTable(strSQL)
            End Select

            If tbCheckSecurity IsNot Nothing AndAlso tbCheckSecurity.Rows.Count > 0 AndAlso strPassports = "-1" Then
                For Each oRow As DataRow In tbCheckSecurity.Rows
                    strPassports = $"{strPassports},{oRow(0)}"
                Next
            End If

            If strPassports.Length > 0 AndAlso strPassports <> "-1" Then
                strSQL = "@SELECT# ISNULL(ID, 0) As id, isnull(Email, '') AS Email, isnull(IDEmployee, 0) as IDEmployee FROM sysroPassports where ID in(" & strPassports & ")"
                Dim tbPassports As DataTable = DataLayer.AccessHelper.CreateDataTable(strSQL)
                ' Para cada usuario obtenemos su cuenta de correo
                For Each oRow As DataRow In tbPassports.Rows
                    ' Intetamos obtener el mail del empleado
                    Dim bInsert As Boolean = True

                    If Not bSendAllNotificationsBetweenSupervisor AndAlso IDType = eNotificationType.Request_Pending AndAlso IDRequest > 0 Then
                        Dim objLastRequestNotification As Nullable(Of DateTime) = roPassportManager.GetLastNotificationSended(roTypes.Any2Double(oRow("id")))

                        If strParameters = "SENDALLWAYS" OrElse Not objLastRequestNotification.HasValue Then
                            roPassportManager.SetLastNotificationSended(roTypes.Any2Integer(oRow("id")), DateTime.Now)
                        Else
                            bInsert = False
                        End If
                    End If

                    If bInsert Then
                        If strPassportIDs.Length > 0 Then
                            strPassportIDs = $"{strPassportIDs};{roTypes.Any2Integer(oRow("id"))}"
                        Else
                            strPassportIDs = roTypes.Any2Integer(oRow("id"))
                        End If

                    End If
                Next
            End If
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "roNotificationTaskManager::GetDirectSupervisorNotification::", ex)
            strPassportIDs = ""
        End Try

        Return strPassportIDs

    End Function

    Protected Function GetSupervisorWithRoleNotification(ByVal IDType As Double, ByVal IDEmployee As Double, ByVal sRoles As String) As String

        Dim strPassportIDs As String = ""

        Try
            Dim table As DataTable = New DataTable
            Dim strSQL As String = String.Empty
            strSQL = $"@SELECT# SP.ID FROM sysrofnSecurity_GeneralNotificationsDependencies(@idtype, @idemployee) SGN
                       INNER JOIN sysroPassports SP ON SP.Id = SGN.IdPassport
                       INNER JOIN sysroGroupFeatures SGF ON SGF.ID = SP.IDGroupFeature 
                       WHERE SGF.ID IN (@SELECT# Name as ID FROM SplitString(@roles, ','))
                       ORDER BY SP.ID ASC"
            Dim Command As DbCommand = AccessHelper.CreateCommand(strSQL)
            AccessHelper.AddParameter(Command, "@idtype", DbType.Int32).Value = IDType
            AccessHelper.AddParameter(Command, "@idemployee", DbType.Int32).Value = IDEmployee
            AccessHelper.AddParameter(Command, "@roles", DbType.String).Value = sRoles
            Dim Adapter As DbDataAdapter = AccessHelper.CreateDataAdapter(Command)
            Adapter.Fill(table)
            If table IsNot Nothing AndAlso table.Rows.Count > 0 Then
                For Each oRow As DataRow In table.Rows
                    If strPassportIDs.Length > 0 Then
                        strPassportIDs = $"{strPassportIDs};{roTypes.Any2Integer(oRow("ID"))}"
                    Else
                        strPassportIDs = roTypes.Any2String(oRow("ID"))
                    End If
                Next
            End If

        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "roNotificationTaskManager::GetSupervisorAtLevelNotification::", ex)
            strPassportIDs = ""
        End Try

        Return strPassportIDs

    End Function

    Protected Overridable Sub Dispose(disposing As Boolean)
        'If Not Me.disposedValue Then
        'End If
        Me.disposedValue = True
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub

End Class