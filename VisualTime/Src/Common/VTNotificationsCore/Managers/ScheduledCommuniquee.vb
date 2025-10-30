Imports Robotics.Azure
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness
Imports Robotics.Base.VTCommuniques
Imports Robotics.Base.VTNotificationsCore.Notifications
Imports Robotics.DataLayer
Imports Robotics.Security
Imports Robotics.VTBase

Public Class NotificationTaskScheduledCommuniqueManager
    Inherits roNotificationTaskManager

    Public Sub New(ByVal sGUID As String)
        MyBase.New(eNotificationType.ScheduledCommuniquee, sGUID)
    End Sub

    Protected Overrides Function GetIdEmployee() As Integer
        Return _oNotificationTask.Key1Numeric
    End Function

    Protected Overrides Function GetIdPassport() As Integer
        Return -1
    End Function

    Protected Overrides Function MustSendPushNotification() As Boolean
        If _oNotificationTask.key3Datetime <= Now Then
            Dim state As New roCommuniqueState
            Dim communiqueManager As New roCommuniqueManager(state)
            Dim communique As roCommunique = communiqueManager.LoadCommunique(_oNotificationTask.Key1Numeric, False)
            If communique.Status = CommuniqueStatusEnum.Online Then
                Return True
            Else
                Return False
            End If
        Else
            Return False
        End If
    End Function

    Protected Overrides Function GetNotificationAvailableDestinations() As roNotificationDestinationConfig()
        Dim configs As New Generic.List(Of roNotificationDestinationConfig)
        Dim state As New roCommuniqueState
        Dim communiqueManager As New roCommuniqueManager(state)
        Dim communique As roCommunique = communiqueManager.LoadCommunique(_oNotificationTask.Key1Numeric, False)
        Dim selector As String = roSelector.BuildSelectionStringFromIDs(communique.Employees, communique.Groups)

        Dim lstEmployees As Generic.List(Of Integer) = roSelector.GetEmployeeList(communique.CreatedBy.IdPassport, "Employees", "U", Permission.Read, selector, "11110", "", False, Nothing, Nothing)
        For Each idEmp As Integer In lstEmployees
            Dim conf = roNotificationHelper.CreateNewPushDestination(defaultLanguage, idEmp, GetIdPassport())
            If conf IsNot Nothing Then configs.Add(conf)
        Next
        Return configs.ToArray()
    End Function

    Protected Overrides Function GetTranslatedNotificationTexts(ByVal oConf As roNotificationDestinationConfig) As roNotificationItem
        Dim oItem As New roNotificationItem
        Try
            oItem.Type = NotificationItemType.push
            oItem.CompanyId = RoAzureSupport.GetCompanyName()
            oItem.Subject = roNotificationHelper.Message("Push.CommuniqueSubject", Nothing, , oConf.Language)
            oItem.Body = roNotificationHelper.Message("Push.CommuniqueBody", Nothing, , oConf.Language)
            oItem.Content = String.Empty
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "roNotificationCore::NotificationTaskScheduledCommuniqueManager::Translate::", ex)
            oItem = Nothing
        End Try
        Return oItem
    End Function

    Protected Overrides Function PostSendAction() As Boolean
        Return True
    End Function

    Public Overrides Function GenerateNotificationTasks(ByVal ads As DataRow) As Boolean
        Return True
    End Function

End Class