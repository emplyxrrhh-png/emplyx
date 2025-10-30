Imports Robotics.Base.DTOs
Imports Robotics.Base.VTNotifications.Notifications

Public Class NotificationsHelper

    Public Property NotReliableNotificationsCalled As Boolean = False

    Public Sub New()
    End Sub

    Function GetDesktopAlerts_EmployeesWithNonReliablePunchesSpy()
        Robotics.Base.VTNotifications.Notifications.Fakes.ShimroNotification.GetDesktopAlerts_EmployeesWithNonReliablePunchesStringRefBooleanRefBooleanRefDateTimeRefDateTimeRefBooleanroNotificationStateRef =
            Function(ByRef sResume As String, ByRef bIsUrgent As Boolean, ByRef bIsCritic As Boolean, ByRef dDateFrom As DateTime, ByRef dDateTo As DateTime, ByVal bNeedDetail As Boolean, ByRef oState As roNotificationState) As DataTable
                Dim sMessage As String = "DesktopAlerts"
                Dim tbRes As DataTable = New DataTable()
                tbRes.Columns.Add("IdAlertType", GetType(Integer))
                tbRes.Columns.Add("DesktopDescription", GetType(String))
                tbRes.Columns.Add("IsUrgent", GetType(Boolean))
                tbRes.Columns.Add("DateFrom", GetType(Date))
                tbRes.Columns.Add("DateTo", GetType(Date))
                tbRes.Columns.Add("IsCritic", GetType(Boolean))
                tbRes.Columns.Add("DetailCount", GetType(Integer))
                'Put some data here if necessary
                'tbRes.Rows.Add(1, sMessage, DateTime.Now, False, False, False, 0, sResume)
                NotReliableNotificationsCalled = True
                Return tbRes
            End Function
    End Function

    Function FillRowMock()
        Dim dt As New DataTable()

        ' Agrega columnas al DataTable
        dt.Columns.Add("ID", GetType(Integer))
        dt.Columns.Add("IDNotification", GetType(Integer))
        dt.Columns.Add("Key1Numeric", GetType(Integer))
        dt.Columns.Add("Key2Numeric", GetType(Integer))
        dt.Columns.Add("Key5Numeric", GetType(Integer))
        dt.Columns.Add("Key3DateTime", GetType(DateTime))
        dt.Columns.Add("Key4DateTime", GetType(DateTime))
        dt.Columns.Add("Key6DateTime", GetType(DateTime))
        dt.Columns.Add("Executed", GetType(Integer))
        dt.Columns.Add("IsReaded", GetType(Integer))
        dt.Columns.Add("Parameters", GetType(String))
        dt.Columns.Add("GUID", GetType(String))
        dt.Columns.Add("Repetition", GetType(Integer))
        dt.Columns.Add("NextRepetition", GetType(Integer))
        dt.Columns.Add("Condition", GetType(String))
        dt.Columns.Add("Destination", GetType(String))
        dt.Columns.Add("AllowMail", GetType(Boolean))
        dt.Columns.Add("FiredDate", GetType(DateTime))
        dt.Columns.Add("NotificationName", GetType(String))

        ' Asigna valores al DataRow
        Dim newRow As DataRow = dt.NewRow()
        newRow("ID") = 1
        newRow("IDNotification") = 3400
        newRow("Key1Numeric") = 1
        newRow("Key1Numeric") = 2
        newRow("Condition") = ""
        newRow("Destination") = ""
        newRow("NotificationName") = ""
        Return newRow

    End Function

    Function DeleteIncompletePunchNotificationIfExist()
        Robotics.Base.VTNotifications.Notifications.Fakes.ShimroNotification.DeleteIncompletePunchNotificationIfExistroNotificationStateInt32DateTime = Function(oNotificationState As roNotificationState, iNotificationID As Integer, dDate As DateTime)
                                                                                                                                                            Return True
                                                                                                                                                        End Function
    End Function

    Function GenerateNotificationTaskMock()
        Robotics.Base.VTNotifications.Notifications.Fakes.ShimroNotification.GenerateNotificationTaskBooleaneNotificationTyperoNotificationStateRefNullableOfInt32NullableOfInt32NullableOfDateTimeNullableOfDateTimeStringNullableOfDateTimeBoolean =
            Function(bSendMail As Boolean, eNotificationType As eNotificationType, ByRef oNotificationState As roNotificationState, iKey1 As Integer?, iKey2 As Integer?, dDate1 As DateTime?, dDate2 As DateTime?, sMessage As String, dDate3 As DateTime?, bIsUrgent As Boolean) As Boolean
                oNotificationState = New roNotificationState()
                Return True
            End Function
    End Function


End Class