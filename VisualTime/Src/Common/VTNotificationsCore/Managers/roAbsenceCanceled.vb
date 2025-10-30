Imports Robotics.Azure
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTNotificationsCore.Notifications
Imports Robotics.Base.VTRequests
Imports Robotics.DataLayer
Imports Robotics.VTBase

Public Class roNotificationTask_Absence_Canceled_By_User_Manager
    Inherits roNotificationTaskManager

    Private idEmployee As Integer = -1
    Private idExchangeEmployee As Integer = -1

    Public Sub New(ByVal sGUID As String)
        MyBase.New(eNotificationType.Absence_Canceled_By_User, sGUID)
    End Sub

    Protected Overrides Function GetIdEmployee() As Integer
        Dim iRetIdEmployee As Integer = idEmployee
        Try
            If idEmployee = -1 Then
                Dim oRes As DataTable = AccessHelper.CreateDataTable("@SELECT# idEmployee,IDEmployeeExchange FROM Requests WHERE ID = " & _oNotificationTask.Key1Numeric)
                If oRes IsNot Nothing AndAlso oRes.Rows.Count > 0 Then
                    idEmployee = roTypes.Any2Integer(oRes(0)("idEmployee"))
                    idExchangeEmployee = roTypes.Any2Integer(oRes(0)("IDEmployeeExchange"))
                End If
            End If

            If _oNotificationTask.RequestType = eRequestType.ExchangeShiftBetweenEmployees Then
                iRetIdEmployee = idExchangeEmployee
            Else
                iRetIdEmployee = idEmployee
            End If
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "roNotificationCore::roAbsenceCanceled::GetIdEmployee::", ex)
            iRetIdEmployee = -1
        End Try

        Return iRetIdEmployee
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
            Dim oReqState As New Requests.roRequestState()
            oReqState.Language.SetLanguageReference("", oConf.Language)
            Dim RequestName As String = roNotificationHelper.Message("Notification.CanceledRequestText", Nothing, , oConf.Language)
            Dim oRequest As New Requests.roRequest(_oNotificationTask.IdRequest, oReqState)

            Dim Params As New ArrayList From {
                roTypes.Any2String(AccessHelper.ExecuteScalar("@SELECT# Name from Employees where ID = " & idEmployee)),
                oRequest.RequestInfo(True)
            }

            oItem.Type = NotificationItemType.email
            oItem.CompanyId = RoAzureSupport.GetCompanyName()
            oItem.Subject = roNotificationHelper.Message("Notification.RequestCanceled.Subject", Nothing, , oConf.Language)
            oItem.Body = roNotificationHelper.Message("Notification.RequestCanceled.Body", Params, , oConf.Language)
            oItem.Content = String.Empty
            oItem.Destination = oConf.Email
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "roNotificationCore::roAbsenceCanceled::Translate::", ex)
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