Imports Robotics.Azure
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTNotificationsCore.Notifications
Imports Robotics.Base.VTRequests.Requests
Imports Robotics.DataLayer
Imports Robotics.VTBase

Public Class roNotificationTask_Request_Pending_Manager
    Inherits roNotificationTaskManager

    Private idEmployee As Integer = -1
    Private idExchangeEmployee As Integer = -1

    Public Sub New(ByVal sGUID As String)
        MyBase.New(eNotificationType.Request_Pending, sGUID)
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
                roLog.GetInstance().logMessage(roLog.EventType.roError, "roNotificationCore::roRequestPending::GetIdEmployee::", ex)
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

            If oRequest.IDEmployee > 0 Then

                Dim RequestName As String = roNotificationHelper.Message("Notification.NewRequestText." & Me._oNotificationTask.RequestType, Nothing, , oConf.Language)
                Dim url As String = ""

                If roTypes.Any2Boolean(roCacheManager.GetInstance().GetAdvParametersCache(RoAzureSupport.GetCompanyName(), "VTPortal.MailRequest")) Then
                    url = "https://vtportal.visualtime.net/default.aspx?VTPortalRequest=" & _oNotificationTask.IdRequest & "_" & GetIdEmployee()
                End If

                Dim SubjectParams As New ArrayList From {RequestName}

                Dim Params As New ArrayList From {
                    roTypes.Any2String(AccessHelper.ExecuteScalar("@SELECT# Name from Employees where ID = " & GetIdEmployee())),
                    RequestName,
                    roTypes.Any2String(AccessHelper.ExecuteScalar("@SELECT# Name FROM Causes WHERE ID = " & _oNotificationTask.key2Numeric))
                }

                oItem.Type = NotificationItemType.email
                oItem.CompanyId = RoAzureSupport.GetCompanyName()
                oItem.Subject = roNotificationHelper.Message("Notification.RequestPending.Subject", SubjectParams, , oConf.Language)
                oItem.Body = roNotificationHelper.Message("Notification.RequestPending.Body", Params, , oConf.Language)

                If url <> "" Then
                    oItem.Body = oItem.Body & "<br/><br/>Puede acceder directamente a la solicitud a través del siguiente <a href='" & url & "'>enlace</a><br/><br/>"
                Else
                    oItem.Body = oItem.Body & "<br/><br/>"
                End If

                oItem.Body = oItem.Body & oRequest.RequestInfo(True)
            Else
                oItem.Body = String.Empty
                oItem.Subject = String.Empty
            End If




            oItem.Content = String.Empty
            oItem.Destination = oConf.Email
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "roNotificationCore::roRequestPending::Translate::", ex)
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