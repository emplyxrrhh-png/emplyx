Imports Robotics.Azure
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTNotificationsCore.Notifications
Imports Robotics.Base.VTRequests.Requests
Imports Robotics.DataLayer
Imports Robotics.VTBase

Public Class roNotificationTask_Request_Impersonation_Manager
    Inherits roNotificationTaskManager

    Private idEmployee As Integer = -1
    Private idExchangeEmployee As Integer = -1

    Public Sub New(ByVal sGUID As String)
        MyBase.New(eNotificationType.Request_Impersonation, sGUID)
    End Sub

    Protected Overrides Function GetIdEmployee() As Integer

        If idEmployee = -1 Then
            Try
                Dim oRes As DataTable = AccessHelper.CreateDataTable("@SELECT# idEmployee,IDEmployeeExchange FROM Requests WHERE ID = " & _oNotificationTask.Key1Numeric)
                If oRes IsNot Nothing AndAlso oRes.Rows.Count > 0 Then
                    idEmployee = roTypes.Any2Integer(oRes(0)("idEmployee"))
                    idExchangeEmployee = roTypes.Any2Integer(oRes(0)("IDEmployeeExchange"))
                End If
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "roNotificationCore::roRequestImpersonation::GetIdEmployee::", ex)
            End Try
        End If

        If _oNotificationTask.RequestType = eRequestType.ExchangeShiftBetweenEmployees Then
            Return idExchangeEmployee
        Else
            Return idEmployee
        End If
    End Function

    Protected Overrides Function GetIdPassport() As Integer
        Return -1
    End Function

    Protected Overrides Function MustSendPushNotification() As Boolean
        Return True
    End Function

    Protected Overrides Function GetNotificationAvailableDestinations() As roNotificationDestinationConfig()
        Return Me.GetDefaultDestinationConfig()
    End Function

    Protected Overrides Function GetTranslatedNotificationTexts(ByVal oConf As roNotificationDestinationConfig) As roNotificationItem

        Dim oItem As New roNotificationItem
        Try
            Dim oReqState As New roRequestState()
            oReqState.Language.SetLanguageReference("", oConf.Language)
            Dim oRequest As New roRequest(_oNotificationTask.IdRequest, oReqState)
            Dim strComment As String = String.Empty
            strComment = oRequest.RequestApprovals(oRequest.RequestApprovals.FindLastIndex(Function(x) x.IDRequest = oRequest.ID)).Comments
            Dim strRequestDetail As String = String.Empty
            strRequestDetail = oRequest.RequestInfo(True)

            Dim ParamsSubject As New ArrayList From {
                roNotificationHelper.Message("Notification.NewRequestText." & _oNotificationTask.RequestType, Nothing, , oConf.Language)
            }

            Dim ParamsBody As New ArrayList From {
                roTypes.Any2String(AccessHelper.ExecuteScalar("@SELECT# Name from sysroPassports where id = " & _oNotificationTask.key2Numeric)),
                roNotificationHelper.Message("Notification.NewRequestText." & _oNotificationTask.RequestType, Nothing, , oConf.Language),
                strRequestDetail.Replace("<b>", "").Replace("</b>", ""),
                roTypes.Any2String(AccessHelper.ExecuteScalar("@SELECT# Name FROM Causes WHERE ID = " & _oNotificationTask.key2Numeric)),
                strComment
            }

            oItem.Type = NotificationItemType.email
            oItem.CompanyId = RoAzureSupport.GetCompanyName()
            oItem.Subject = roNotificationHelper.Message("Notification.RequestImpersonation.Subject", ParamsSubject, , oConf.Language)
            oItem.Body = roNotificationHelper.Message("Notification.RequestImpersonation.Body", ParamsBody, , oConf.Language)
            oItem.Content = String.Empty
            oItem.Destination = oConf.Email
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "roNotificationCore::roRequestImpersonation::Translate::", ex)
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