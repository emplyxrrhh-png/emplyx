Imports Robotics.Azure
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTNotificationsCore.Notifications
Imports Robotics.Base.VTRequests.Requests
Imports Robotics.VTBase

Public Class roNotificationTask_Requests_Result_Manager
    Inherits roNotificationTaskManager

    Private idEmployee As Integer = -1
    Private idExchangeEmployee As Integer = -1

    Public Sub New(ByVal sGUID As String)
        MyBase.New(eNotificationType.Requests_Result, sGUID)
    End Sub

    Protected Overrides Function GetIdEmployee() As Integer
        Return _oNotificationTask.Key1Numeric
    End Function

    Protected Overrides Function GetIdPassport() As Integer
        Return -1
    End Function

    Protected Overrides Function MustSendPushNotification() As Boolean
        Return False
    End Function

    Protected Overrides Function GetNotificationAvailableDestinations() As roNotificationDestinationConfig()
        Return Me.GetDefaultDestinationConfig(True, True, True, True)
    End Function

    Protected Overrides Function GetTranslatedNotificationTexts(ByVal oConf As roNotificationDestinationConfig) As roNotificationItem
        Dim oItem As New roNotificationItem
        Try
            Dim oReqState As New roRequestState()
            oReqState.Language.SetLanguageReference("", oConf.Language)
            Dim oRequest As New roRequest(_oNotificationTask.IdRequest, oReqState)

            If oRequest.IDEmployee > 0 Then
                Dim strComment As String = String.Empty
                strComment = oRequest.RequestApprovals(oRequest.RequestApprovals.FindLastIndex(Function(x) x.IDRequest = oRequest.ID)).Comments
                Dim strRequestDetail As String = String.Empty
                strRequestDetail = oRequest.RequestInfo(True)

                Dim RequestName As String = roNotificationHelper.Message("Notification.NewRequestText." & _oNotificationTask.RequestType, Nothing, , oConf.Language)

                Dim SubjectParams As New ArrayList From {RequestName}
                Dim Params As New ArrayList From {
                    RequestName,
                    strRequestDetail.Replace("<b>", "").Replace("</b>", ""),
                    strComment
                }

                oItem.Type = NotificationItemType.email
                oItem.CompanyId = RoAzureSupport.GetCompanyName()
                If _oNotificationTask.Parameters = "ACCEPTED" Then
                    oItem.Subject = roNotificationHelper.Message("Notification.EmployeeRequestApproved.Subject", SubjectParams, , oConf.Language)
                    oItem.Body = roNotificationHelper.Message("Notification.EmployeeRequestApproved.Body", Params, , oConf.Language)
                Else
                    oItem.Subject = roNotificationHelper.Message("Notification.EmployeeRequestRejected.Subject", SubjectParams, , oConf.Language)
                    oItem.Body = roNotificationHelper.Message("Notification.EmployeeRequestRejected.Body", Params, , oConf.Language)
                End If
            Else
                oItem.Subject = String.Empty
                oItem.Body = String.Empty
            End If

            oItem.Content = String.Empty
            oItem.Destination = oConf.Email
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "roNotificationCore::roRequestResult::Translate::", ex)
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