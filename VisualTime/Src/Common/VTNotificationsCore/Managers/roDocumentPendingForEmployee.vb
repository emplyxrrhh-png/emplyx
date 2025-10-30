Imports Robotics.Azure
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTNotificationsCore.Notifications
Imports Robotics.DataLayer
Imports Robotics.VTBase

Public Class roNotificationTask_Document_Pending_For_Employee_Manager
    Inherits roNotificationTaskManager

    Dim bCheckRepetition As Boolean
    Dim iRepeatEveryDays As Integer = -1
    Dim iMaxRepetition As Integer = -1
    Dim dNextRepetition As Date = Date.MinValue

    Public Sub New(ByVal sGUID As String)
        MyBase.New(eNotificationType.Document_Pending_For_Employee, sGUID)

    End Sub

    Protected Overrides Function GetIdEmployee() As Integer
        Return _oNotificationTask.Key1Numeric
    End Function

    Protected Function GetIdCompany() As Integer
        Return _oNotificationTask.Key1Numeric
    End Function

    Protected Overrides Function GetIdPassport() As Integer
        Return -1
    End Function

    Protected Overrides Function MustSendPushNotification() As Boolean
        Return True
    End Function

    Protected Overrides Function GetNotificationAvailableDestinations() As roNotificationDestinationConfig()
        Return Me.GetDefaultDestinationConfig(True, False, False, False, True)
    End Function

    Protected Overrides Function GetTranslatedNotificationTexts(ByVal oConf As roNotificationDestinationConfig) As roNotificationItem
        Dim oItem As New roNotificationItem
        Dim strSubject As String = String.Empty
        Dim strBody As String = String.Empty
        Try
            Dim Params As New ArrayList
            strSubject = roNotificationHelper.Message("Notification.DocumentNextToBeRequired.Supervisor.Subject", Nothing, , oConf.Language)
            Dim sPRLDocType As String = _oNotificationTask.Parameters.Split("@)")(0)
            Dim sDocName As String = _oNotificationTask.Parameters.Split("@)")(1)
            Select Case sPRLDocType
                Case "EMPLOYEE"
                    Params.Add(roTypes.Any2String(AccessHelper.ExecuteScalar("@SELECT# Name FROM Employees WHERE ID = " & GetIdEmployee())))
                    Params.Add(sDocName)
                    strBody = roNotificationHelper.Message("Notification.DocumentNextToBeRequired.Employee.Supervisor.Body", Params, , oConf.Language)
                Case "COMPANY"
                    Params.Add(roTypes.Any2String(AccessHelper.ExecuteScalar("@SELECT# Name FROM Groups WHERE ID = " & GetIdCompany())))
                    Params.Add(sDocName)
                    strBody = roNotificationHelper.Message("Notification.DocumentNextToBeRequired.Company.Supervisor.Body", Params, , oConf.Language)
            End Select

            oItem.Type = NotificationItemType.email
            oItem.Destination = oConf.Email
            oItem.CompanyId = RoAzureSupport.GetCompanyName()
            oItem.Subject = strSubject
            oItem.Body = strBody
            oItem.Content = String.Empty
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "roNotificationCore::roDocumentPendingForEmployee::Translate::", ex)
        End Try
        Return oItem
    End Function

    Protected Overrides Function PostSendAction() As Boolean
        Dim bRet As Boolean = True
        Try
            ' APV: Control de repeticiones si aplica con detalle (sólo para ausencias)

            If bCheckRepetition Then

                AccessHelper.ExecuteSql("@UPDATE# sysroNotificationTasks SET Repetition = " & _oNotificationTask.Repetition + 1 & " WHERE ID=" & _oNotificationTask.Id)

                If _oNotificationTask.Repetition < iMaxRepetition - 1 Then
                    AccessHelper.ExecuteSql("@UPDATE# sysroNotificationTasks SET NextRepetition = " & roTypes.Any2Time(dNextRepetition.Date).SQLSmallDateTime & " WHERE ID=" & _oNotificationTask.Id)
                Else
                    AccessHelper.ExecuteSql("@UPDATE# sysroNotificationTasks SET NextRepetition = " & roTypes.Any2Time(New Date(2079, 1, 1)).SQLSmallDateTime & " WHERE ID=" & _oNotificationTask.Id)
                End If
            End If
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "roNotificationCore::roDocumentPendingForEmployee::PostSendAction::", ex)
            bRet = False
        End Try

        Return bRet
    End Function

    Public Overrides Function GenerateNotificationTasks(ByVal ads As DataRow) As Boolean
        Return True
    End Function

End Class