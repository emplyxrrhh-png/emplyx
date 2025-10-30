Imports System.ComponentModel
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTChannels
Imports Robotics.Base.VTNotifications.Notifications
Imports Robotics.Base.VTNotificationsCore
Imports Robotics.DataLayer
Imports Robotics.Base
Imports VT_XU_Base
Imports VT_XU_Common
Imports VT_XU_Security
Imports Xunit
Imports Robotics.Base.VTCommuniques

Namespace Unit.Test

    <CollectionDefinition("Notification", DisableParallelization:=True)>
    <Collection("Notification")>
    <Category("Notification")>
    Public Class NotificationTest
        Private ReadOnly helperDataLayer As DatalayerHelper
        Private ReadOnly helperAdvancedParameters As AdvancedParametersHelper
        Private ReadOnly helperNotifications As NotificationsHelper
        Private ReadOnly helperPassport As PassportHelper
        Private ReadOnly helperBusiness As BusinessHelper
        Private ReadOnly helperBase As BaseHelper
        Private ReadOnly helperSecurity As SecurityHelper
        Private ReadOnly helperEmployee As EmployeeHelper
        Private ReadOnly helperAzure As AzureHelper

        Sub New()
            helperDataLayer = New DatalayerHelper
            helperAdvancedParameters = New AdvancedParametersHelper
            helperNotifications = New NotificationsHelper
            helperPassport = New PassportHelper
            helperBusiness = New BusinessHelper
            helperBase = New BaseHelper
            helperSecurity = New SecurityHelper
            helperEmployee = New EmployeeHelper
            helperAzure = New AzureHelper
        End Sub

        <Fact(DisplayName:="Must not notify Unmatched Time Record if it's a Night Shift")>
        Sub MustNotNotifyIfNightShift()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperAdvancedParameters.AdvancedParameterCacheStub(New Dictionary(Of String, String) From {{"OnlyFirstNotificationBetweenSupervisors", "0"}, {"Customization", "0"}, {"VTLive.Notification.DisableUserField", "0"}, {"VTLive.Notification.DisabledNotificationsList", ""}})
                Fakes.ShimAccessHelper.CreateDataTableStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                         Dim tableName As String = helperDataLayer.GetTableNameFromQuery(strQuery)
                                                                                         If tableName = "Notifications" Then
                                                                                             Return helperDataLayer.CreateDataTableMock({"ID", "ShowOnDesktop"}, New Object()() {New Object() {"1", 0}})

                                                                                             'Return Fake Night Shift Record'
                                                                                         ElseIf tableName = "sysrovwIncompletedDays" Then
                                                                                             Return helperDataLayer.CreateDataTableMock({"IDEmployee", "Date"}, {New Object() {"1", DateTime.Now.AddDays(-1).Date}})
                                                                                             'The actual Test'
                                                                                         ElseIf tableName = "DailySchedule" Then
                                                                                             Return helperDataLayer.CreateDataTableMock({"ShiftUsed", "IsFloating", "StartLimit", "EndLimit"}, {New Object() {1, 0, DateTime.Now, DateTime.Now.AddDays(1)}})
                                                                                         End If

                                                                                     End Function
                helperDataLayer.ExecuteSqlSpy()
                'Act
                Dim oManager As New roNotificationTask_Day_With_Unmatched_Time_Record_Manager("GUID")
                Dim result = oManager.GenerateNotificationTasks(Nothing)
                'Assert
                Assert.True(helperDataLayer.ExecuteSqlWasCalled = DatalayerHelper.SqlExecuteString.None)
            End Using
        End Sub

        <Fact(DisplayName:="Must notify Unmatched Time Record if not a Night Shift")>
        Sub MustNotifyIfNotNightShift()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperAdvancedParameters.AdvancedParameterCacheStub(New Dictionary(Of String, String) From {{"OnlyFirstNotificationBetweenSupervisors", "0"}, {"Customization", "0"}, {"VTLive.Notification.DisableUserField", "0"}, {"VTLive.Notification.DisabledNotificationsList", ""}})
                Fakes.ShimAccessHelper.CreateDataTableStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                         Dim tableName As String = helperDataLayer.GetTableNameFromQuery(strQuery)
                                                                                         If tableName = "Notifications" Then
                                                                                             Return helperDataLayer.CreateDataTableMock({"ID", "ShowOnDesktop"}, New Object()() {New Object() {"1", 0}})

                                                                                             'Return Fake Night Shift Record'
                                                                                         ElseIf tableName = "sysrovwIncompletedDays" Then
                                                                                             Return helperDataLayer.CreateDataTableMock({"IDEmployee", "Date"}, {New Object() {"1", DateTime.Now.AddDays(-1).Date}})
                                                                                             'The actual Test'
                                                                                         ElseIf tableName = "DailySchedule" Then
                                                                                             Return helperDataLayer.CreateDataTableMock({"ShiftUsed", "StartLimit", "EndLimit"}, {New Object() {1, DateTime.Now, DateTime.Now}})
                                                                                         End If

                                                                                     End Function
                helperDataLayer.ExecuteSqlSpy()
                'Act
                Dim oManager As New Robotics.Base.VTNotificationsCore.roNotificationTask_Day_With_Unmatched_Time_Record_Manager("GUID")
                Dim result = oManager.GenerateNotificationTasks(Nothing)
                'Assert
                Assert.True(helperDataLayer.ExecuteSqlWasCalled = DatalayerHelper.SqlExecuteString.InsertNotification)
            End Using
        End Sub

        <Fact(DisplayName:="Must notify Unmatched Time Record if Shift Date is Today")>
        Sub MustNotifyIfShiftIsToday()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperAdvancedParameters.AdvancedParameterCacheStub(New Dictionary(Of String, String) From {{"OnlyFirstNotificationBetweenSupervisors", "0"}, {"Customization", "0"}, {"VTLive.Notification.DisableUserField", "0"}, {"VTLive.Notification.DisabledNotificationsList", ""}})
                Fakes.ShimAccessHelper.CreateDataTableStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                         Dim tableName As String = helperDataLayer.GetTableNameFromQuery(strQuery)
                                                                                         If tableName = "Notifications" Then
                                                                                             Return helperDataLayer.CreateDataTableMock({"ID", "ShowOnDesktop"}, New Object()() {New Object() {"1", 0}})
                                                                                             'The actual Test
                                                                                         ElseIf tableName = "sysrovwIncompletedDays" Then
                                                                                             Return helperDataLayer.CreateDataTableMock({"IDEmployee", "Date"}, {New Object() {"1", DateTime.Now.Date}})
                                                                                         End If
                                                                                     End Function
                helperDataLayer.ExecuteSqlSpy()
                'Act
                Dim oManager As New Robotics.Base.VTNotificationsCore.roNotificationTask_Day_With_Unmatched_Time_Record_Manager("GUID")
                Dim result = oManager.GenerateNotificationTasks(Nothing)

                'Assert
                Assert.True(helperDataLayer.ExecuteSqlWasCalled = DatalayerHelper.SqlExecuteString.InsertNotification)
            End Using
        End Sub

        <Fact(DisplayName:="Must show unreliable punches alert on desktop if advanced parameter not set")>
        Sub MustShowUnreliablePunchesAlertOnDesktopIfAdvancedParameterNotSet()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {})
                helperNotifications.GetDesktopAlerts_EmployeesWithNonReliablePunchesSpy()
                helperPassport.PassportStub(1, helperDataLayer)
                'Act
                roNotification.GetDesktopAlerts(New roNotificationState)

                'Assert
                Assert.True(helperNotifications.NotReliableNotificationsCalled)
            End Using
        End Sub

        <Fact(DisplayName:="Must not show unreliable punches alert on desktop if advanced parameter set")>
        Sub MustNotShowUnreliablePunchesAlertOnDesktopIfAdvancedParameterSet()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"VTPortal.NotifyUnreliablePunch", "false"}})
                helperNotifications.GetDesktopAlerts_EmployeesWithNonReliablePunchesSpy()
                helperPassport.PassportStub(1, helperDataLayer)
                'Act
                roNotification.GetDesktopAlerts(New roNotificationState)

                'Assert
                Assert.False(helperNotifications.NotReliableNotificationsCalled)
            End Using
        End Sub

        <Fact(DisplayName:="Must show unreliable punches alert details on desktop if advanced parameter not set")>
        Sub MustShowUnreliablePunchesAlertDetailsOnDesktopIfAdvancedParameterNotSet()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {})
                helperNotifications.GetDesktopAlerts_EmployeesWithNonReliablePunchesSpy()
                'Act
                roNotification.GetDesktopAlertsDetails(Robotics.Base.DTOs.eNotificationType.Day_with_Unreliable_Time_Record, New roNotificationState)

                'Assert
                Assert.True(helperNotifications.NotReliableNotificationsCalled)
            End Using
        End Sub

        <Fact(DisplayName:="Should show unreliable punches alert details on desktop if there are unreliable punches")>
        Sub ShouldShowUnreliablePunchesAlertDetailsOnDesktopIfThereAreUnreliablePunches()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperPassport.PassportStub(1, helperDataLayer, Nothing)
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {})
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                Dim tMock As New DataTableMock With {.columns = {"EmployeeName", "IDEmployee", "ShiftDate", "GroupName", "exclude"},
                    .values = New Object()() {New Object() {"Test", 1, DateTime.Now(), "Test", 0}}}
                dDataTStub.Add("Employees", tMock)
                helperDataLayer.CreateDataTableStub(dDataTStub)


                'Act
                Dim desktopAlerts As DataSet = roNotification.GetDesktopAlertsDetails(Robotics.Base.DTOs.eNotificationType.Day_with_Unreliable_Time_Record, New roNotificationState)

                'Assert
                Assert.True(desktopAlerts.Tables.Count > 0 AndAlso desktopAlerts.Tables(0).Rows.Count > 0)
            End Using
        End Sub

        <Fact(DisplayName:="Must not show unreliable punches alert details on desktop if advanced parameter set")>
        Sub MustNotShowUnreliablePunchesAlertDetailsOnDesktopIfAdvancedParameterSet()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"VTPortal.NotifyUnreliablePunch", "false"}})
                helperNotifications.GetDesktopAlerts_EmployeesWithNonReliablePunchesSpy()
                'Act
                roNotification.GetDesktopAlertsDetails(Robotics.Base.DTOs.eNotificationType.Day_with_Unreliable_Time_Record, New roNotificationState)

                'Assert
                Assert.False(helperNotifications.NotReliableNotificationsCalled)
            End Using
        End Sub

        <Fact(DisplayName:="Must notify unreliable punches if advanced parameter not set")>
        Sub MustNotifyUnreliablePunchesIfAdvancedParameterNotSet()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperAdvancedParameters.AdvancedParameterCacheStub(New Dictionary(Of String, String) From {})
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {})
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                Dim tMock As New DataTableMock With {.columns = {"IDEmployee", "Date"}, .values = New Object()() {New Object() {"1", Now.Date}}}
                dDataTStub.Add("sysroNotificationTasks", tMock)
                Dim dtNotification As DataTable = helperDataLayer.CreateDataTableStub(dDataTStub)
                helperDataLayer.ExecuteSqlSpy()

                'Act
                Dim oNotificationCore As New Robotics.Base.VTNotificationsCore.roNotificationTask_Day_with_Unreliable_Time_Record_Manager("GUID")
                oNotificationCore.GenerateNotificationTasks(helperDataLayer.CreateDataTableMock({"ID", "ShowOnDesktop"}, New Object()() {New Object() {"20", 0}}).Rows(0))

                'Assert
                Assert.True(helperDataLayer.ExecuteSqlWasCalled = DatalayerHelper.SqlExecuteString.InsertNotification)
            End Using
        End Sub

        <Fact(DisplayName:="Must not notify unreliable punches if advanced parameter set")>
        Sub MustNotNotifyUnreliablePunchesIfAdvancedParameterSet()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperAdvancedParameters.AdvancedParameterCacheStub(New Dictionary(Of String, String) From {})
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"VTPortal.NotifyUnreliablePunch", "false"}})
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                Dim tMock As New DataTableMock With {.columns = {"IDEmployee", "Date"}, .values = New Object()() {New Object() {"1", Now.Date}}}
                dDataTStub.Add("sysroNotificationTasks", tMock)
                Dim dtNotification As DataTable = helperDataLayer.CreateDataTableStub(dDataTStub)
                helperDataLayer.ExecuteSqlSpy()

                'Act
                Dim oNotificationCore As New Robotics.Base.VTNotificationsCore.roNotificationTask_Day_with_Unreliable_Time_Record_Manager("GUID")
                oNotificationCore.GenerateNotificationTasks(helperDataLayer.CreateDataTableMock({"ID", "ShowOnDesktop"}, New Object()() {New Object() {"20", 0}}).Rows(0))

                'Assert
                Assert.False(helperDataLayer.ExecuteSqlWasCalled = DatalayerHelper.SqlExecuteString.InsertNotification)
            End Using
        End Sub

        <Fact(DisplayName:="Must Update NotificationTask If It has Sent")>
        Sub MustUpdateNotificationTaskIfIthasSent()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperAdvancedParameters.AdvancedParameterCacheStub(New Dictionary(Of String, String) From {{"OnlyFirstNotificationBetweenSupervisors", "0"}, {"Customization", "0"}, {"VTLive.Notification.DisableUserField", "0"}, {"VTLive.Notification.DisabledNotificationsList", ""}})
                helperDataLayer.ExecuteSqlSpy()
                'Act
                Dim oManager As New roNotificationTask_Advice_For_New_Conversation("GUID")
                Dim newRow As DataRow = helperNotifications.FillRowMock()
                oManager.Send(newRow)

                'Assert
                Assert.True(helperDataLayer.ExecuteSqlWasCalled = DatalayerHelper.SqlExecuteString.UpdateNotification)
            End Using
        End Sub

        <Fact(DisplayName:="Must not send NotificationTask If It is nothing")>
        Sub MustNotSendNotificationIfItIsNothing()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperAdvancedParameters.AdvancedParameterCacheStub(New Dictionary(Of String, String) From {{"OnlyFirstNotificationBetweenSupervisors", "0"}, {"Customization", "0"}, {"VTLive.Notification.DisableUserField", "0"}, {"VTLive.Notification.DisabledNotificationsList", ""}})
                helperDataLayer.ExecuteSqlSpy()
                'Act
                Dim oManager As New roNotificationTask_Advice_For_New_Conversation("GUID")
                Dim bSend As Boolean = oManager.Send(Nothing)

                'Assert
                Assert.True(Not bSend)
            End Using
        End Sub

        <Fact(DisplayName:="Must not Load NotificationTask If Datarow is nothing")>
        Sub MustNotLoadNotificationIfDataRowIsNothing()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperAdvancedParameters.AdvancedParameterCacheStub(New Dictionary(Of String, String) From {{"OnlyFirstNotificationBetweenSupervisors", "0"}, {"Customization", "0"}, {"VTLive.Notification.DisableUserField", "0"}, {"VTLive.Notification.DisabledNotificationsList", ""}})
                helperDataLayer.ExecuteSqlSpy()
                'Act
                Dim oManager As New roNotificationTask_Advice_For_New_Conversation("GUID")
                Dim bLoad As Boolean = False
                Try
                    bLoad = oManager.Load(Nothing)
                Catch ex As Exception
                    bLoad = False
                End Try

                'Assert
                Assert.True(Not bLoad)
            End Using
        End Sub

        <Fact(DisplayName:="Must Generate Notification When Create Conversation")>
        Sub MustGenerateNotificationWhenCreateConversation()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                CommonHelper.InitLicense(New List(Of String) From {"Feature\Channels", "Feature\Complaints"})
                'Arrange
                helperAdvancedParameters.AdvancedParameterCacheStub(New Dictionary(Of String, String) From {{"OnlyFirstNotificationBetweenSupervisors", "0"}, {"Customization", "0"}, {"VTLive.Notification.DisableUserField", "0"}, {"VTLive.Notification.DisabledNotificationsList", ""}})
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                Dim tMock As New DataTableMock With {.columns = {"Id", "Title", "IdCreatedBy", "Status", "CreatedOn", "IdModifiedBy", "ModifiedOn", "PublishedOn", "ReceiptAcknowledgment", "AllowAnonymous", "Deleted", "IdDeletedBy", "DeletedOn", "IsComplaintChannel"},
                    .values = New Object()() {New Object() {"1", "Canal de pruebas", "1", "1", "2023-10-01", "1", "2023-10-01", "2023-10-01", "1", "1", "0", "0", "2023-10-01", "0"}}}
                dDataTStub.Add("\bSELECT\b.*\bChannels\b.*", tMock)
                tMock = New DataTableMock With {.columns = {"ID", "IDType"}, .values = New Object()() {New Object() {1, eNotificationType.Advice_For_NewConversation}}}
                dDataTStub.Add("\bSELECT\b.*\bNotifications\b.*", tMock)
                Dim dtEmployeeChannels As DataTable = helperDataLayer.CreateDataTableStub(dDataTStub)
                helperPassport.PassportStub(1, helperDataLayer)
                helperDataLayer.ExecuteSqlSpy()
                'Act
                Dim oManager As New roConversationManager
                Dim oConversation As New roConversation
                Dim oChannel As New roChannel
                oChannel.Id = 1

                oConversation.Id = 0
                oConversation.Title = "Test"
                oConversation.Channel = oChannel
                oConversation.CreatedBy = 1

                Dim oMessage As New roMessage With {.Body = "Hola qué hase?"}

                Dim result = oManager.CreateConversation(oConversation, oMessage, False)
                'Assert
                Assert.True(helperDataLayer.ExecuteSqlWasCalled = DatalayerHelper.SqlExecuteString.InsertNotification)
            End Using
        End Sub

        <Fact(DisplayName:="Must Generate Advice for Employee When Create Conversation")>
        Sub MustGenerateAdviceForEmployeeWhenCreateConversation()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                CommonHelper.InitLicense(New List(Of String) From {"Feature\Channels", "Feature\Complaints"})
                'Arrange
                helperAdvancedParameters.AdvancedParameterCacheStub(New Dictionary(Of String, String) From {{"OnlyFirstNotificationBetweenSupervisors", "0"}, {"Customization", "0"}, {"VTLive.Notification.DisableUserField", "0"}, {"VTLive.Notification.DisabledNotificationsList", ""}})
                'Fakes.ShimAccessHelper.CreateDataTableStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                '                                                                         Dim tableName As String = helperDataLayer.GetTableNameFromQuery(strQuery)
                '                                                                         If tableName = "Channels" Then
                '                                                                             Return helperDataLayer.CreateDataTableMock({"ID", "ShowOnDesktop"}, New Object()() {New Object() {"1", 0}})
                '                                                                         End If

                '                                                                         If tableName = "Notifications" Then
                '                                                                             Return helperDataLayer.CreateDataTableMock({"ID", "IDType"}, New Object()() {New Object() {1, eNotificationType.Advice_For_NewConversation}})
                '                                                                         End If
                '                                                                     End Function

                'Visibilidad como empleado sólo del canal 12
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                Dim tMock As New DataTableMock With {.columns = {"Id"}, .values = New Object()() {New Object() {"1"}}}
                dDataTStub.Add("sysrovwChannelEmployees.*IdEmployee.*\=.*" & viewerEmployeeId & ".*", tMock)
                tMock = New DataTableMock With {.columns = {"Id", "Title", "IdCreatedBy", "Status", "CreatedOn", "IdModifiedBy", "ModifiedOn", "PublishedOn", "ReceiptAcknowledgment", "AllowAnonymous", "Deleted", "IdDeletedBy", "DeletedOn", "IsComplaintChannel"},
                    .values = New Object()() {New Object() {"1", "Canal de pruebas", "1", "1", "2023-10-01", "1", "2023-10-01", "2023-10-01", "1", "1", "0", "0", "2023-10-01", "0"}}}
                dDataTStub.Add("\bSELECT\b.*\bChannels\b.*", tMock)
                tMock = New DataTableMock With {.columns = {"ID", "IDType"}, .values = New Object()() {New Object() {1, eNotificationType.Advice_For_NewConversation}}}
                dDataTStub.Add("\bSELECT\b.*\bNotifications\b.*", tMock)
                Dim dtEmployeeChannels As DataTable = helperDataLayer.CreateDataTableStub(dDataTStub)
                helperPassport.PassportStub(1, helperDataLayer)
                helperDataLayer.ExecuteSqlSpy()
                helperDataLayer.StartTransaction()
                helperDataLayer.EndTransaction()

                'Act
                Dim oManager As New roConversationManager
                Dim oConversation As New roConversation
                Dim oChannel As New roChannel
                oChannel.Id = 1
                oChannel.ReceiptAcknowledgment = True
                oChannel.Status = ChannelStatusEnum.Published

                oConversation.Id = 0
                oConversation.Title = "Test"
                oConversation.Channel = oChannel
                oConversation.CreatedBy = 1

                Dim oMessage As New roMessage With {.Body = "Hola qué hase?", .CreatedBy = 1}
                oMessage.Conversation = oConversation

                Dim result = oManager.CreateConversation(oConversation, oMessage, False)
                'Assert
                Assert.True(helperDataLayer.ExecuteSqlWasCalled = DatalayerHelper.SqlExecuteString.InsertNotification)
            End Using
        End Sub

        <Fact(DisplayName:="Must Generate Notification When Create Message")>
        Sub MustGenerateNotificationWhenCreateMessage()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                CommonHelper.InitLicense(New List(Of String) From {"Feature\Channels", "Feature\Complaints"})
                'Arrange
                helperAdvancedParameters.AdvancedParameterCacheStub(New Dictionary(Of String, String) From {{"OnlyFirstNotificationBetweenSupervisors", "0"}, {"Customization", "0"}, {"VTLive.Notification.DisableUserField", "0"}, {"VTLive.Notification.DisabledNotificationsList", ""}})
                Fakes.ShimAccessHelper.CreateDataTableStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                         Dim tableName As String = helperDataLayer.GetTableNameFromQuery(strQuery)
                                                                                         If tableName = "Channels" Then
                                                                                             Return helperDataLayer.CreateDataTableMock({"ID", "ShowOnDesktop"}, New Object()() {New Object() {"1", 0}})
                                                                                         End If

                                                                                         If tableName = "Notifications" Then
                                                                                             Return helperDataLayer.CreateDataTableMock({"ID", "IDType"}, New Object()() {New Object() {1, eNotificationType.NewMessage_FromEmployee_InChannel}})
                                                                                         End If
                                                                                     End Function
                helperDataLayer.ExecuteSqlSpy()
                helperPassport.PassportStub(1, helperDataLayer)
                helperDataLayer.StartTransaction()
                helperDataLayer.EndTransaction()

                'Act
                Dim oManager As New roMessageManager
                Dim oConversation As New roConversation
                Dim oChannel As New roChannel
                oChannel.Id = 1
                oChannel.ReceiptAcknowledgment = True

                oConversation.Id = 0
                oConversation.Title = "Test"
                oConversation.Channel = oChannel
                oConversation.CreatedBy = 1

                Dim oMessage As New roMessage
                oMessage.IsResponse = False
                oMessage.Conversation = oConversation
                oMessage.CreatedBy = 1
                oManager.CreateMessage(oMessage, False, True)

                'Assert
                Assert.True(helperDataLayer.ExecuteSqlWasCalled = DatalayerHelper.SqlExecuteString.InsertNotification)
            End Using
        End Sub

        '------------------------ BREAK NOT TAKEN -------------------------
        <Fact(DisplayName:="Must Not Generate Break Not Taken Notification Task For Supervisor When Employee Has Not Started Working Even If Break Not Started")>
        Sub MustNotGenerateBreakNotTakenNotificationTaskForSupervisorWhenEmployeeHasNotStartedWorkingEvenIfBreakNotStarted()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim dtPunches As New DataTable
                Dim dBeginWorkDate As Date = New Date(Now.Date.Year, Now.Date.Month, Now.Date.Day, 9, 0, 0)
                Dim dStartBreak As Date = New Date(Now.Date.Year, Now.Date.Month, Now.Date.Day, 10, 0, 0)
                Dim dEndBreak As Date = New Date(Now.Date.Year, Now.Date.Month, Now.Date.Day, 12, 0, 0)
                dtPunches.Columns.Add("DateTime", GetType(DateTime))
                dtPunches.Columns.Add("ActualType", GetType(Integer))
                dtPunches.Rows.Add(New Date(Now.Date.Year, Now.Date.Month, Now.Date.Day, 10, 30, 0), "2")
                helperDataLayer.ExecuteSqlSpy()

                'Act
                Notifications.roNotificationHelper.CreateBreakNotification(dtPunches, TimeSpan.Parse("00:00"), dStartBreak, dEndBreak, eNotificationType.BreakNotTaken, 1, 1, Now.Date, dBeginWorkDate)

                'Assert
                Assert.False(helperDataLayer.ExecuteSqlWasCalled = DatalayerHelper.SqlExecuteString.InsertNotification)
            End Using
        End Sub

        <Fact(DisplayName:="Must Not Generate Break Not Taken Notification Task For Supervisor When Employee Has Started Working And Break Started Inside Break Period")>
        Sub MustNotGenerateBreakNotTakenNotificationTaskForSupervisorWhenEmployeeHasStartedWorkingAndBreakStartedInsideBreakPeriod()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim dtPunches As New DataTable
                Dim dBeginWorkDate As Date = New Date(Now.Date.Year, Now.Date.Month, Now.Date.Day, 9, 0, 0)
                Dim dStartBreak As Date = New Date(Now.Date.Year, Now.Date.Month, Now.Date.Day, 10, 0, 0)
                Dim dEndBreak As Date = New Date(Now.Date.Year, Now.Date.Month, Now.Date.Day, 12, 0, 0)
                dtPunches.Columns.Add("DateTime", GetType(DateTime))
                dtPunches.Columns.Add("ActualType", GetType(Integer))
                dtPunches.Rows.Add(New Date(Now.Date.Year, Now.Date.Month, Now.Date.Day, 8, 30, 0), "1")
                dtPunches.Rows.Add(New Date(Now.Date.Year, Now.Date.Month, Now.Date.Day, 10, 30, 0), "2")
                dtPunches.Rows.Add(New Date(Now.Date.Year, Now.Date.Month, Now.Date.Day, 10, 50, 0), "1")
                dtPunches.Rows.Add(New Date(Now.Date.Year, Now.Date.Month, Now.Date.Day, 14, 30, 0), "2")
                helperDataLayer.ExecuteSqlSpy()

                'Act
                Notifications.roNotificationHelper.CreateBreakNotification(dtPunches, TimeSpan.Parse("00:00"), dStartBreak, dEndBreak, eNotificationType.BreakNotTaken, 1, 1, Now.Date, dBeginWorkDate)

                'Assert
                Assert.False(helperDataLayer.ExecuteSqlWasCalled = DatalayerHelper.SqlExecuteString.InsertNotification)
            End Using
        End Sub

        <Fact(DisplayName:="Must Not Generate Break Not Taken Notification Task For Supervisor When Employee Has Started Working Inside Break Period And Break Started Also Inside Break Period")>
        Sub MustNotGenerateBreakNotTakenNotificationTaskForSupervisorWhenEmployeeHasStartedWorkingInsideBreakPeriodAndBreakStartedAlsoInsideBreakPeriod()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim dtPunches As New DataTable
                Dim dBeginWorkDate As Date = New Date(Now.Date.Year, Now.Date.Month, Now.Date.Day, 9, 0, 0)
                Dim dStartBreak As Date = New Date(Now.Date.Year, Now.Date.Month, Now.Date.Day, 10, 0, 0)
                Dim dEndBreak As Date = New Date(Now.Date.Year, Now.Date.Month, Now.Date.Day, 12, 0, 0)
                dtPunches.Columns.Add("DateTime", GetType(DateTime))
                dtPunches.Columns.Add("ActualType", GetType(Integer))
                dtPunches.Rows.Add(New Date(Now.Date.Year, Now.Date.Month, Now.Date.Day, 10, 15, 0), "1")
                dtPunches.Rows.Add(New Date(Now.Date.Year, Now.Date.Month, Now.Date.Day, 10, 30, 0), "2")
                dtPunches.Rows.Add(New Date(Now.Date.Year, Now.Date.Month, Now.Date.Day, 10, 50, 0), "1")
                dtPunches.Rows.Add(New Date(Now.Date.Year, Now.Date.Month, Now.Date.Day, 14, 30, 0), "2")
                helperDataLayer.ExecuteSqlSpy()

                'Act
                Notifications.roNotificationHelper.CreateBreakNotification(dtPunches, TimeSpan.Parse("00:00"), dStartBreak, dEndBreak, eNotificationType.BreakNotTaken, 1, 1, Now.Date, dBeginWorkDate)

                'Assert
                Assert.False(helperDataLayer.ExecuteSqlWasCalled = DatalayerHelper.SqlExecuteString.InsertNotification)
            End Using
        End Sub

        <Fact(DisplayName:="Must Generate Break Not Taken Notification Task For Supervisor When Employee Has Started Working And Break Started Outside Break Period")>
        Sub MustGenerateBreakNotTakenNotificationTaskForSupervisorWhenEmployeeHasStartedWorkingAndBreakStartedOutsideBreakPeriod()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim dtPunches As New DataTable
                Dim dBeginWorkDate As Date = New Date(Now.Date.Year, Now.Date.Month, Now.Date.Day, 9, 0, 0)
                Dim dStartBreak As Date = New Date(Now.Date.Year, Now.Date.Month, Now.Date.Day, 10, 0, 0)
                Dim dEndBreak As Date = New Date(Now.Date.Year, Now.Date.Month, Now.Date.Day, 12, 0, 0)
                dtPunches.Columns.Add("DateTime", GetType(DateTime))
                dtPunches.Columns.Add("ActualType", GetType(Integer))
                dtPunches.Rows.Add(New Date(Now.Date.Year, Now.Date.Month, Now.Date.Day, 8, 30, 0), "1")
                dtPunches.Rows.Add(New Date(Now.Date.Year, Now.Date.Month, Now.Date.Day, 9, 30, 0), "2")
                dtPunches.Rows.Add(New Date(Now.Date.Year, Now.Date.Month, Now.Date.Day, 10, 30, 0), "1")
                dtPunches.Rows.Add(New Date(Now.Date.Year, Now.Date.Month, Now.Date.Day, 14, 30, 0), "2")
                helperDataLayer.ExecuteSqlSpy()

                'Act
                Notifications.roNotificationHelper.CreateBreakNotification(dtPunches, TimeSpan.Parse("00:00"), dStartBreak, dEndBreak, eNotificationType.BreakNotTaken, 1, 1, Now.Date, dBeginWorkDate)

                'Assert
                Assert.True(helperDataLayer.ExecuteSqlWasCalled = DatalayerHelper.SqlExecuteString.InsertNotification)
            End Using
        End Sub

        <Fact(DisplayName:="Must Generate Break Not Taken Notification Task For Supervisor When Employee Has Started Working And Break Not Started")>
        Sub MustGenerateBreakNotTakenNotificationTaskForSupervisorWhenEmployeeHasStartedWorkingAndBreakNotStarted()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim dtPunches As New DataTable
                Dim dBeginWorkDate As Date = New Date(Now.Date.Year, Now.Date.Month, Now.Date.Day, 9, 0, 0)
                Dim dStartBreak As Date = New Date(Now.Date.Year, Now.Date.Month, Now.Date.Day, 10, 0, 0)
                Dim dEndBreak As Date = New Date(Now.Date.Year, Now.Date.Month, Now.Date.Day, 12, 0, 0)
                dtPunches.Columns.Add("DateTime", GetType(DateTime))
                dtPunches.Columns.Add("ActualType", GetType(Integer))
                dtPunches.Rows.Add(New Date(Now.Date.Year, Now.Date.Month, Now.Date.Day, 8, 30, 0), "1")
                dtPunches.Rows.Add(New Date(Now.Date.Year, Now.Date.Month, Now.Date.Day, 14, 30, 0), "2")
                helperDataLayer.ExecuteSqlSpy()

                'Act
                Notifications.roNotificationHelper.CreateBreakNotification(dtPunches, TimeSpan.Parse("00:00"), dStartBreak, dEndBreak, eNotificationType.BreakNotTaken, 1, 1, Now.Date, dBeginWorkDate)

                'Assert
                Assert.True(helperDataLayer.ExecuteSqlWasCalled = DatalayerHelper.SqlExecuteString.InsertNotification)
            End Using
        End Sub

        <Fact(DisplayName:="Must Generate Break Not Taken Notification Task For Supervisor When Employee Has Started Working After Break Period Started And Break Not Started")>
        Sub MustGenerateBreakNotTakenNotificationTaskForSupervisorWhenEmployeeHasStartedWorkingAfterBreakPeriodStartedAndBreakNotStarted()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim dtPunches As New DataTable
                Dim dBeginWorkDate As Date = New Date(Now.Date.Year, Now.Date.Month, Now.Date.Day, 9, 0, 0)
                Dim dStartBreak As Date = New Date(Now.Date.Year, Now.Date.Month, Now.Date.Day, 10, 0, 0)
                Dim dEndBreak As Date = New Date(Now.Date.Year, Now.Date.Month, Now.Date.Day, 12, 0, 0)
                dtPunches.Columns.Add("DateTime", GetType(DateTime))
                dtPunches.Columns.Add("ActualType", GetType(Integer))
                dtPunches.Rows.Add(New Date(Now.Date.Year, Now.Date.Month, Now.Date.Day, 10, 10, 0), "1")
                dtPunches.Rows.Add(New Date(Now.Date.Year, Now.Date.Month, Now.Date.Day, 14, 30, 0), "2")
                helperDataLayer.ExecuteSqlSpy()

                'Act
                Notifications.roNotificationHelper.CreateBreakNotification(dtPunches, TimeSpan.Parse("00:00"), dStartBreak, dEndBreak, eNotificationType.BreakNotTaken, 1, 1, Now.Date, dBeginWorkDate)

                'Assert
                Assert.True(helperDataLayer.ExecuteSqlWasCalled = DatalayerHelper.SqlExecuteString.InsertNotification)
            End Using
        End Sub

        <Fact(DisplayName:="Must Not Generate Break Not Taken Notification Task For Supervisor When Employee Has Started Working But More Than Four Hours Before Start Shift")>
        Sub MustNotGenerateBreakNotTakenNotificationTaskForSupervisorWhenEmployeeHasStartedWorkingButMoreThanFourHoursBeforeStartShift()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim dtPunches As New DataTable
                Dim dBeginWorkDate As Date = New Date(Now.Date.Year, Now.Date.Month, Now.Date.Day, 9, 0, 0)
                Dim dStartBreak As Date = New Date(Now.Date.Year, Now.Date.Month, Now.Date.Day, 10, 0, 0)
                Dim dEndBreak As Date = New Date(Now.Date.Year, Now.Date.Month, Now.Date.Day, 12, 0, 0)
                dtPunches.Columns.Add("DateTime", GetType(DateTime))
                dtPunches.Columns.Add("ActualType", GetType(Integer))
                dtPunches.Rows.Add(New Date(Now.Date.Year, Now.Date.Month, Now.Date.Day, 4, 59, 0), "1")
                helperDataLayer.ExecuteSqlSpy()

                'Act
                Notifications.roNotificationHelper.CreateBreakNotification(dtPunches, TimeSpan.Parse("00:00"), dStartBreak, dEndBreak, eNotificationType.BreakNotTaken, 1, 1, Now.Date, dBeginWorkDate)

                'Assert
                Assert.False(helperDataLayer.ExecuteSqlWasCalled = DatalayerHelper.SqlExecuteString.InsertNotification)
            End Using
        End Sub

        <Fact(DisplayName:="Must Not Generate Break Not Taken Notification Task For Supervisor When Employee Has Not Started Working Today And Yesterday Didnt Take Break")>
        Sub MustNotGenerateBreakNotTakenNotificationTaskForSupervisorWhenEmployeeHasNotStartedWorkingTodayAndYesterdayDidntTakeBreak()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim dtPunches As New DataTable
                Dim dBeginWorkDate As Date = New Date(Now.Date.Year, Now.Date.Month, Now.Date.Day, 9, 0, 0)
                Dim dStartBreak As Date = New Date(Now.Date.Year, Now.Date.Month, Now.Date.Day, 10, 0, 0)
                Dim dEndBreak As Date = New Date(Now.Date.Year, Now.Date.Month, Now.Date.Day, 12, 0, 0)
                dtPunches.Columns.Add("DateTime", GetType(DateTime))
                dtPunches.Columns.Add("ActualType", GetType(Integer))
                dtPunches.Rows.Add(New Date(Now.Date.Year, Now.Date.Month, Now.Date.Day, 9, 30, 0).AddDays(-1), "1")
                dtPunches.Rows.Add(New Date(Now.Date.Year, Now.Date.Month, Now.Date.Day, 11, 30, 0).AddDays(-1), "2")
                dtPunches.Rows.Add(New Date(Now.Date.Year, Now.Date.Month, Now.Date.Day, 12, 30, 0).AddDays(-1), "1")
                helperDataLayer.ExecuteSqlSpy()

                'Act
                Notifications.roNotificationHelper.CreateBreakNotification(dtPunches, TimeSpan.Parse("00:00"), dStartBreak, dEndBreak, eNotificationType.BreakNotTaken, 1, 1, Now.Date, dBeginWorkDate)

                'Assert
                Assert.False(helperDataLayer.ExecuteSqlWasCalled = DatalayerHelper.SqlExecuteString.InsertNotification)
            End Using
        End Sub

        '------------------------ BREAK NOT STARTED -------------------------
        <Fact(DisplayName:="Must Generate Break Not Started Notification For Employee When Employee Has Started Working And Not Started Break Inside Break Period")>
        Sub MustGenerateBreakNotStartedNotificationForEmployeeWhenEmployeeHasStartedWorkingAndNotStartedBreakInsideBreakPeriod()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim dtPunches As New DataTable
                Dim dBeginWorkDate As Date = New Date(Now.Date.Year, Now.Date.Month, Now.Date.Day, 9, 0, 0)
                Dim dStartBreak As Date = New Date(Now.Date.Year, Now.Date.Month, Now.Date.Day, 10, 0, 0)
                Dim dEndBreak As Date = New Date(Now.Date.Year, Now.Date.Month, Now.Date.Day, 12, 0, 0)
                dtPunches.Columns.Add("DateTime", GetType(DateTime))
                dtPunches.Columns.Add("ActualType", GetType(Integer))
                dtPunches.Rows.Add(New Date(Now.Date.Year, Now.Date.Month, Now.Date.Day, 8, 59, 0), "1")
                dtPunches.Rows.Add(New Date(Now.Date.Year, Now.Date.Month, Now.Date.Day, 10, 30, 0), "2")
                helperDataLayer.ExecuteSqlSpy()

                'Act
                Notifications.roNotificationHelper.CreateBreakNotification(dtPunches, TimeSpan.Parse("00:10"), dStartBreak, dStartBreak, eNotificationType.BreakStart, 1, 1, Now.Date, dBeginWorkDate)

                'Assert
                Assert.True(helperDataLayer.ExecuteSqlWasCalled = DatalayerHelper.SqlExecuteString.InsertNotification)
            End Using
        End Sub

        <Fact(DisplayName:="Must Generate Break Not Started Notification For Employee When Employee Has Started Working And Not Started Break At All")>
        Sub MustGenerateBreakNotStartedNotificationForEmployeeWhenEmployeeHasStartedWorkingAndNotStartedBreakAtAll()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim dtPunches As New DataTable
                Dim dBeginWorkDate As Date = New Date(Now.Date.Year, Now.Date.Month, Now.Date.Day, 9, 0, 0)
                Dim dStartBreak As Date = New Date(Now.Date.Year, Now.Date.Month, Now.Date.Day, 10, 0, 0)
                Dim dEndBreak As Date = New Date(Now.Date.Year, Now.Date.Month, Now.Date.Day, 12, 0, 0)
                dtPunches.Columns.Add("DateTime", GetType(DateTime))
                dtPunches.Columns.Add("ActualType", GetType(Integer))
                dtPunches.Rows.Add(New Date(Now.Date.Year, Now.Date.Month, Now.Date.Day, 8, 59, 0), "1")
                helperDataLayer.ExecuteSqlSpy()

                'Act
                Notifications.roNotificationHelper.CreateBreakNotification(dtPunches, TimeSpan.Parse("00:10"), dStartBreak, dStartBreak, eNotificationType.BreakStart, 1, 1, Now.Date, dBeginWorkDate)

                'Assert
                Assert.True(helperDataLayer.ExecuteSqlWasCalled = DatalayerHelper.SqlExecuteString.InsertNotification)
            End Using
        End Sub

        <Fact(DisplayName:="Must Not Generate Break Not Started Notification For Employee When Employee Has Not Started Working Even If Started Break Inside Break Period")>
        Sub MustNotGenerateBreakNotStartedNotificationForEmployeeWhenEmployeeHasNotStartedWorkingEvenIfStartedBreakInsideBreakPeriod()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim dtPunches As New DataTable
                Dim dBeginWorkDate As Date = New Date(Now.Date.Year, Now.Date.Month, Now.Date.Day, 9, 0, 0)
                Dim dStartBreak As Date = New Date(Now.Date.Year, Now.Date.Month, Now.Date.Day, 10, 0, 0)
                Dim dEndBreak As Date = New Date(Now.Date.Year, Now.Date.Month, Now.Date.Day, 12, 0, 0)
                dtPunches.Columns.Add("DateTime", GetType(DateTime))
                dtPunches.Columns.Add("ActualType", GetType(Integer))
                dtPunches.Rows.Add(New Date(Now.Date.Year, Now.Date.Month, Now.Date.Day, 10, 5, 0), "2")
                helperDataLayer.ExecuteSqlSpy()

                'Act
                Notifications.roNotificationHelper.CreateBreakNotification(dtPunches, TimeSpan.Parse("00:10"), dStartBreak, dStartBreak, eNotificationType.BreakStart, 1, 1, Now.Date, dBeginWorkDate)

                'Assert
                Assert.False(helperDataLayer.ExecuteSqlWasCalled = DatalayerHelper.SqlExecuteString.InsertNotification)
            End Using
        End Sub

        <Fact(DisplayName:="Must Not Generate Break Not Started Notification For Employee When Employee Has Started Working And Has Started Break Inside Break Period")>
        Sub MustNotGenerateBreakNotStartedNotificationForEmployeeWhenEmployeeHasStartedWorkingAndHasStartedBreakInsideBreakPeriod()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim dtPunches As New DataTable
                Dim dBeginWorkDate As Date = New Date(Now.Date.Year, Now.Date.Month, Now.Date.Day, 9, 0, 0)
                Dim dStartBreak As Date = New Date(Now.Date.Year, Now.Date.Month, Now.Date.Day, 10, 0, 0)
                Dim dEndBreak As Date = New Date(Now.Date.Year, Now.Date.Month, Now.Date.Day, 12, 0, 0)
                dtPunches.Columns.Add("DateTime", GetType(DateTime))
                dtPunches.Columns.Add("ActualType", GetType(Integer))
                dtPunches.Rows.Add(New Date(Now.Date.Year, Now.Date.Month, Now.Date.Day, 9, 4, 0), "1")
                dtPunches.Rows.Add(New Date(Now.Date.Year, Now.Date.Month, Now.Date.Day, 10, 5, 0), "2")
                helperDataLayer.ExecuteSqlSpy()

                'Act
                Notifications.roNotificationHelper.CreateBreakNotification(dtPunches, TimeSpan.Parse("00:10"), dStartBreak, dStartBreak, eNotificationType.BreakStart, 1, 1, Now.Date, dBeginWorkDate)

                'Assert
                Assert.False(helperDataLayer.ExecuteSqlWasCalled = DatalayerHelper.SqlExecuteString.InsertNotification)
            End Using
        End Sub

        <Fact(DisplayName:="Must Not Generate Break Not Started Notification For Employee When Employee Has Started Working Inside Break Period And Has Started Break Inside Break Period")>
        Sub MustNotGenerateBreakNotStartedNotificationForEmployeeWhenEmployeeHasStartedWorkingInsideBreakPeriodAndHasStartedBreakInsideBreakPeriod()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim dtPunches As New DataTable
                Dim dBeginWorkDate As Date = New Date(Now.Date.Year, Now.Date.Month, Now.Date.Day, 9, 0, 0)
                Dim dStartBreak As Date = New Date(Now.Date.Year, Now.Date.Month, Now.Date.Day, 10, 0, 0)
                Dim dEndBreak As Date = New Date(Now.Date.Year, Now.Date.Month, Now.Date.Day, 12, 0, 0)
                dtPunches.Columns.Add("DateTime", GetType(DateTime))
                dtPunches.Columns.Add("ActualType", GetType(Integer))
                dtPunches.Rows.Add(New Date(Now.Date.Year, Now.Date.Month, Now.Date.Day, 10, 4, 0), "1")
                dtPunches.Rows.Add(New Date(Now.Date.Year, Now.Date.Month, Now.Date.Day, 10, 5, 0), "2")
                helperDataLayer.ExecuteSqlSpy()

                'Act
                Notifications.roNotificationHelper.CreateBreakNotification(dtPunches, TimeSpan.Parse("00:10"), dStartBreak, dStartBreak, eNotificationType.BreakStart, 1, 1, Now.Date, dBeginWorkDate)

                'Assert
                Assert.False(helperDataLayer.ExecuteSqlWasCalled = DatalayerHelper.SqlExecuteString.InsertNotification)
            End Using
        End Sub

        <Fact(DisplayName:="Must Not Generate Break Not Started Notification For Employee When Employee Has Started Working After End Courtesy Time For Break Start")>
        Sub MustGenerateBreakNotStartedNotificationForEmployeeWhenEmployeeHasStartedWorkingAfterEndCourtesyTimeForBreakStart()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim dtPunches As New DataTable
                Dim dBeginWorkDate As Date = New Date(Now.Date.Year, Now.Date.Month, Now.Date.Day, 9, 0, 0)
                Dim dStartBreak As Date = New Date(Now.Date.Year, Now.Date.Month, Now.Date.Day, 10, 0, 0)
                Dim dEndBreak As Date = New Date(Now.Date.Year, Now.Date.Month, Now.Date.Day, 12, 0, 0)
                dtPunches.Columns.Add("DateTime", GetType(DateTime))
                dtPunches.Columns.Add("ActualType", GetType(Integer))
                dtPunches.Rows.Add(New Date(Now.Date.Year, Now.Date.Month, Now.Date.Day, 10, 11, 0), "1")
                dtPunches.Rows.Add(New Date(Now.Date.Year, Now.Date.Month, Now.Date.Day, 10, 30, 0), "2")
                helperDataLayer.ExecuteSqlSpy()

                'Act
                Notifications.roNotificationHelper.CreateBreakNotification(dtPunches, TimeSpan.Parse("00:10"), dStartBreak, dStartBreak, eNotificationType.BreakStart, 1, 1, Now.Date, dBeginWorkDate)

                'Assert
                Assert.False(helperDataLayer.ExecuteSqlWasCalled = DatalayerHelper.SqlExecuteString.InsertNotification)
            End Using
        End Sub

        '------------------------ BREAK NOT FINISHED -------------------------
        <Fact(DisplayName:="Must Generate Break Not Finished Notification For Employee When Employee Has Started Working And Has Started Break But Has Not Finished Break Inside Break Period")>
        Sub MustGenerateBreakNotFinishedNotificationForEmployeeWhenEmployeeHasStartedWorkingAndHasStartedBreakButHasNotFinishedBreakInsideBreakPeriod()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim dtPunches As New DataTable
                Dim dBeginWorkDate As Date = New Date(Now.Date.Year, Now.Date.Month, Now.Date.Day, 9, 0, 0)
                Dim dStartBreak As Date = New Date(Now.Date.Year, Now.Date.Month, Now.Date.Day, 10, 0, 0)
                Dim dEndBreak As Date = New Date(Now.Date.Year, Now.Date.Month, Now.Date.Day, 12, 0, 0)
                dtPunches.Columns.Add("DateTime", GetType(DateTime))
                dtPunches.Columns.Add("ActualType", GetType(Integer))
                dtPunches.Rows.Add(New Date(Now.Date.Year, Now.Date.Month, Now.Date.Day, 8, 59, 0), "1")
                dtPunches.Rows.Add(New Date(Now.Date.Year, Now.Date.Month, Now.Date.Day, 10, 30, 0), "2")
                helperDataLayer.ExecuteSqlSpy()

                'Act
                Notifications.roNotificationHelper.CreateBreakNotification(dtPunches, TimeSpan.Parse("00:10"), dStartBreak, dEndBreak, eNotificationType.BreakFinish, 1, 1, Now.Date, dBeginWorkDate)

                'Assert
                Assert.True(helperDataLayer.ExecuteSqlWasCalled = DatalayerHelper.SqlExecuteString.InsertNotification)
            End Using
        End Sub

        <Fact(DisplayName:="Must Not Generate Break Not Finished Notification For Employee When Employee Has Not Started Working Even If Started But Not Finished Break Inside Break Period")>
        Sub MustNotGenerateBreakNotFinishedNotificationForEmployeeWhenEmployeeHasNotStartedWorkingEvenIfStartedButNotFinishedBreakInsideBreakPeriod()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim dtPunches As New DataTable
                Dim dBeginWorkDate As Date = New Date(Now.Date.Year, Now.Date.Month, Now.Date.Day, 9, 0, 0)
                Dim dStartBreak As Date = New Date(Now.Date.Year, Now.Date.Month, Now.Date.Day, 10, 0, 0)
                Dim dEndBreak As Date = New Date(Now.Date.Year, Now.Date.Month, Now.Date.Day, 12, 0, 0)
                dtPunches.Columns.Add("DateTime", GetType(DateTime))
                dtPunches.Columns.Add("ActualType", GetType(Integer))
                dtPunches.Rows.Add(New Date(Now.Date.Year, Now.Date.Month, Now.Date.Day, 10, 30, 0), "2")
                helperDataLayer.ExecuteSqlSpy()

                'Act
                Notifications.roNotificationHelper.CreateBreakNotification(dtPunches, TimeSpan.Parse("00:10"), dStartBreak, dEndBreak, eNotificationType.BreakFinish, 1, 1, Now.Date, dBeginWorkDate)

                'Assert
                Assert.False(helperDataLayer.ExecuteSqlWasCalled = DatalayerHelper.SqlExecuteString.InsertNotification)
            End Using
        End Sub

        <Fact(DisplayName:="Must Not Generate Break Not Finished Notification For Employee When Employee Has Started Working And Has Started Break And Finished Inside Break Period")>
        Sub MustNotGenerateBreakNotFinishedNotificationForEmployeeWhenEmployeeHasStartedWorkingAndHasStartedBreakAndFinishedInsideBreakPeriod()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim dtPunches As New DataTable
                Dim dBeginWorkDate As Date = New Date(Now.Date.Year, Now.Date.Month, Now.Date.Day, 9, 0, 0)
                Dim dStartBreak As Date = New Date(Now.Date.Year, Now.Date.Month, Now.Date.Day, 10, 0, 0)
                Dim dEndBreak As Date = New Date(Now.Date.Year, Now.Date.Month, Now.Date.Day, 12, 0, 0)
                dtPunches.Columns.Add("DateTime", GetType(DateTime))
                dtPunches.Columns.Add("ActualType", GetType(Integer))
                dtPunches.Rows.Add(New Date(Now.Date.Year, Now.Date.Month, Now.Date.Day, 8, 45, 0), "1")
                dtPunches.Rows.Add(New Date(Now.Date.Year, Now.Date.Month, Now.Date.Day, 10, 5, 0), "2")
                dtPunches.Rows.Add(New Date(Now.Date.Year, Now.Date.Month, Now.Date.Day, 10, 45, 0), "1")
                helperDataLayer.ExecuteSqlSpy()

                'Act
                Notifications.roNotificationHelper.CreateBreakNotification(dtPunches, TimeSpan.Parse("00:10"), dStartBreak, dEndBreak, eNotificationType.BreakFinish, 1, 1, Now.Date, dBeginWorkDate)

                'Assert
                Assert.False(helperDataLayer.ExecuteSqlWasCalled = DatalayerHelper.SqlExecuteString.InsertNotification)
            End Using
        End Sub

        <Fact(DisplayName:="Must Not Generate Break Not Finished Notification For Employee When Employee Has Started Working And Has Started Break And Finished Before Courtesy Break Period")>
        Sub MustNotGenerateBreakNotFinishedNotificationForEmployeeWhenEmployeeHasStartedWorkingAndHasStartedBreakAndFinishedBeforeCourtesyBreakPeriod()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim dtPunches As New DataTable
                Dim dBeginWorkDate As Date = New Date(Now.Date.Year, Now.Date.Month, Now.Date.Day, 9, 0, 0)
                Dim dStartBreak As Date = New Date(Now.Date.Year, Now.Date.Month, Now.Date.Day, 10, 0, 0)
                Dim dEndBreak As Date = New Date(Now.Date.Year, Now.Date.Month, Now.Date.Day, 12, 0, 0)
                dtPunches.Columns.Add("DateTime", GetType(DateTime))
                dtPunches.Columns.Add("ActualType", GetType(Integer))
                dtPunches.Rows.Add(New Date(Now.Date.Year, Now.Date.Month, Now.Date.Day, 8, 45, 0), "1")
                dtPunches.Rows.Add(New Date(Now.Date.Year, Now.Date.Month, Now.Date.Day, 10, 5, 0), "2")
                dtPunches.Rows.Add(New Date(Now.Date.Year, Now.Date.Month, Now.Date.Day, 12, 10, 0), "1")
                helperDataLayer.ExecuteSqlSpy()

                'Act
                Notifications.roNotificationHelper.CreateBreakNotification(dtPunches, TimeSpan.Parse("00:10"), dStartBreak, dEndBreak, eNotificationType.BreakFinish, 1, 1, Now.Date, dBeginWorkDate)

                'Assert
                Assert.False(helperDataLayer.ExecuteSqlWasCalled = DatalayerHelper.SqlExecuteString.InsertNotification)
            End Using
        End Sub

        <Fact(DisplayName:="Must Not Generate Break Not Finished Notification For Employee When Employee Has Started Working After End Courtesy Time For Break Start")>
        Sub MustGenerateBreakNotFinishedNotificationForEmployeeWhenEmployeeHasStartedWorkingAfterEndCourtesyTimeForBreakStart()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim dtPunches As New DataTable
                Dim dBeginWorkDate As Date = New Date(Now.Date.Year, Now.Date.Month, Now.Date.Day, 9, 0, 0)
                Dim dStartBreak As Date = New Date(Now.Date.Year, Now.Date.Month, Now.Date.Day, 10, 0, 0)
                Dim dEndBreak As Date = New Date(Now.Date.Year, Now.Date.Month, Now.Date.Day, 12, 0, 0)
                dtPunches.Columns.Add("DateTime", GetType(DateTime))
                dtPunches.Columns.Add("ActualType", GetType(Integer))
                dtPunches.Rows.Add(New Date(Now.Date.Year, Now.Date.Month, Now.Date.Day, 12, 30, 0), "1")
                dtPunches.Rows.Add(New Date(Now.Date.Year, Now.Date.Month, Now.Date.Day, 14, 30, 0), "2")
                helperDataLayer.ExecuteSqlSpy()

                'Act
                Notifications.roNotificationHelper.CreateBreakNotification(dtPunches, TimeSpan.Parse("00:10"), dStartBreak, dEndBreak, eNotificationType.BreakFinish, 1, 1, Now.Date, dBeginWorkDate)

                'Assert
                Assert.False(helperDataLayer.ExecuteSqlWasCalled = DatalayerHelper.SqlExecuteString.InsertNotification)
            End Using
        End Sub

        <Fact(DisplayName:="Should send push notification for schedule communique")>
        Sub ShouldSendPushNotificationForScheduleCommunique()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperAdvancedParameters.AdvancedParameterCacheStub(New Dictionary(Of String, String) From {{"OnlyFirstNotificationBetweenSupervisors", "0"}, {"Customization", "0"}, {"VTLive.Notification.DisableUserField", "0"}, {"VTLive.Notification.DisabledNotificationsList", ""}})
                Fakes.ShimAccessHelper.CreateDataTableStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                         Dim tableName As String = helperDataLayer.GetTableNameFromQuery(strQuery)
                                                                                         If tableName = "Notifications" Then
                                                                                             Return helperDataLayer.CreateDataTableMock({"ID", "ShowOnDesktop"}, New Object()() {New Object() {"1", 0}})
                                                                                         ElseIf tableName = "Communiques" Then
                                                                                             Return helperDataLayer.CreateDataTableMock({"Id", "IdCompany", "Subject", "IdCreatedBy", "CreatedOn", "Mandatory", "Message", "AllowChangeResponse", "AllowedResponses", "ResponseLimitPercentage", "ExpirationDate", "PlanificationDate", "SentOn", "Status", "Archived"}, New Object()() {New Object() {"1", "1", "test", "1", Now, "1", "test", "1", "1", "50", "", Now, Now, "1", "0"}})
                                                                                         ElseIf tableName = "GetEmployeeUserFieldValue" Then
                                                                                             Return New DataTable()
                                                                                         End If

                                                                                     End Function
                helperDataLayer.ExecuteSqlSpy()
                helperSecurity.GetEmployeeList()
                helperEmployee.GetEmployee()
                helperBase.SendNotificationPushToPassport()
                'Act
                Dim notificationFactory As New roNotificationTaskFactory
                Dim instanceGUID As Guid = Guid.NewGuid()
                Dim notificationTable As New DataTable("NotificationTask")
                notificationTable.Columns.Add("ID", GetType(Integer))
                notificationTable.Columns.Add("IDNotification", GetType(Integer))
                notificationTable.Columns.Add("Key1Numeric", GetType(Integer))
                notificationTable.Columns.Add("Key2Numeric", GetType(Integer))
                notificationTable.Columns.Add("Key5Numeric", GetType(Integer))
                notificationTable.Columns.Add("Key3DateTime", GetType(DateTime))
                notificationTable.Columns.Add("Key4DateTime", GetType(DateTime))
                notificationTable.Columns.Add("Key6DateTime", GetType(DateTime))
                notificationTable.Columns.Add("Executed", GetType(Boolean))
                notificationTable.Columns.Add("IsReaded", GetType(Boolean))
                notificationTable.Columns.Add("Parameters", GetType(String))
                notificationTable.Columns.Add("GUID", GetType(String))
                notificationTable.Columns.Add("Repetition", GetType(Integer))
                notificationTable.Columns.Add("NextRepetition", GetType(DateTime))
                notificationTable.Columns.Add("Condition", GetType(String))
                notificationTable.Columns.Add("Destination", GetType(String))
                notificationTable.Columns.Add("AllowMail", GetType(Boolean))
                notificationTable.Columns.Add("FiredDate", GetType(DateTime))
                notificationTable.Columns.Add("NotificationName", GetType(String))

                Dim newRow As DataRow = notificationTable.NewRow()
                newRow("ID") = 1
                newRow("IDNotification") = 4991
                newRow("Key1Numeric") = 123
                newRow("Key2Numeric") = 456
                newRow("Key5Numeric") = 789
                newRow("Key3DateTime") = DateTime.Now
                newRow("Key4DateTime") = DateTime.Now.AddHours(1)
                newRow("Key6DateTime") = DateTime.Now.AddHours(2)
                newRow("Executed") = True
                newRow("IsReaded") = False
                newRow("Parameters") = "Some parameters"
                newRow("GUID") = Guid.NewGuid().ToString()
                newRow("Repetition") = 3
                newRow("NextRepetition") = DateTime.Now.AddDays(1)
                newRow("Condition") = "<xml></xml>"
                newRow("Destination") = "<xml></xml>"
                newRow("AllowMail") = True
                newRow("FiredDate") = DateTime.Now.AddMinutes(30)
                newRow("NotificationName") = "Sample Notification"
                Dim send As Boolean = False

                'Act

                Dim manager As roNotificationTaskManager = notificationFactory.GetNotificationTaskManager(instanceGUID.ToString(), 92)
                If manager IsNot Nothing Then send = manager.Send(newRow)
                'Assert
                Assert.True(send AndAlso helperBase.SendPushNotificationWasCalled)
            End Using
        End Sub

        <Fact(DisplayName:="Should not send email notification for schedule communique")>
        Sub ShouldNotSendEmailNotificationForScheduleCommunique()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperAdvancedParameters.AdvancedParameterCacheStub(New Dictionary(Of String, String) From {{"OnlyFirstNotificationBetweenSupervisors", "0"}, {"Customization", "0"}, {"VTLive.Notification.DisableUserField", "0"}, {"VTLive.Notification.DisabledNotificationsList", ""}})
                Fakes.ShimAccessHelper.CreateDataTableStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                         Dim tableName As String = helperDataLayer.GetTableNameFromQuery(strQuery)
                                                                                         If tableName = "Notifications" Then
                                                                                             Return helperDataLayer.CreateDataTableMock({"ID", "ShowOnDesktop"}, New Object()() {New Object() {"1", 0}})
                                                                                         ElseIf tableName = "Communiques" Then
                                                                                             Return helperDataLayer.CreateDataTableMock({"Id", "IdCompany", "Subject", "IdCreatedBy", "CreatedOn", "Mandatory", "Message", "AllowChangeResponse", "AllowedResponses", "ResponseLimitPercentage", "ExpirationDate", "PlanificationDate", "SentOn", "Status", "Archived"}, New Object()() {New Object() {"1", "1", "test", "1", Now, "1", "test", "1", "1", "50", "", Now, Now, "1", "0"}})
                                                                                         ElseIf tableName = "GetEmployeeUserFieldValue" Then
                                                                                             Return New DataTable()
                                                                                         End If

                                                                                     End Function
                helperDataLayer.ExecuteSqlSpy()
                helperSecurity.GetEmployeeList()
                helperEmployee.GetEmployee()
                helperBase.SendNotificationPushToPassport()
                helperAzure.SendTaskToQueue()

                'Act
                Dim notificationFactory As New roNotificationTaskFactory
                Dim instanceGUID As Guid = Guid.NewGuid()
                Dim notificationTable As New DataTable("NotificationTask")
                notificationTable.Columns.Add("ID", GetType(Integer))
                notificationTable.Columns.Add("IDNotification", GetType(Integer))
                notificationTable.Columns.Add("Key1Numeric", GetType(Integer))
                notificationTable.Columns.Add("Key2Numeric", GetType(Integer))
                notificationTable.Columns.Add("Key5Numeric", GetType(Integer))
                notificationTable.Columns.Add("Key3DateTime", GetType(DateTime))
                notificationTable.Columns.Add("Key4DateTime", GetType(DateTime))
                notificationTable.Columns.Add("Key6DateTime", GetType(DateTime))
                notificationTable.Columns.Add("Executed", GetType(Boolean))
                notificationTable.Columns.Add("IsReaded", GetType(Boolean))
                notificationTable.Columns.Add("Parameters", GetType(String))
                notificationTable.Columns.Add("GUID", GetType(String))
                notificationTable.Columns.Add("Repetition", GetType(Integer))
                notificationTable.Columns.Add("NextRepetition", GetType(DateTime))
                notificationTable.Columns.Add("Condition", GetType(String))
                notificationTable.Columns.Add("Destination", GetType(String))
                notificationTable.Columns.Add("AllowMail", GetType(Boolean))
                notificationTable.Columns.Add("FiredDate", GetType(DateTime))
                notificationTable.Columns.Add("NotificationName", GetType(String))

                Dim newRow As DataRow = notificationTable.NewRow()
                newRow("ID") = 1
                newRow("IDNotification") = 4991
                newRow("Key1Numeric") = 123
                newRow("Key2Numeric") = 456
                newRow("Key5Numeric") = 789
                newRow("Key3DateTime") = DateTime.Now
                newRow("Key4DateTime") = DateTime.Now.AddHours(1)
                newRow("Key6DateTime") = DateTime.Now.AddHours(2)
                newRow("Executed") = True
                newRow("IsReaded") = False
                newRow("Parameters") = "Some parameters"
                newRow("GUID") = Guid.NewGuid().ToString()
                newRow("Repetition") = 3
                newRow("NextRepetition") = DateTime.Now.AddDays(1)
                newRow("Condition") = "<xml></xml>"
                newRow("Destination") = "<xml></xml>"
                newRow("AllowMail") = True
                newRow("FiredDate") = DateTime.Now.AddMinutes(30)
                newRow("NotificationName") = "Sample Notification"
                Dim send As Boolean = False

                'Act

                Dim manager As roNotificationTaskManager = notificationFactory.GetNotificationTaskManager(instanceGUID.ToString(), 92)
                If manager IsNot Nothing Then send = manager.Send(newRow)
                'Assert
                Assert.True(send AndAlso Not helperAzure.emailWasSended)
            End Using
        End Sub

        <Fact(DisplayName:="Should not send push notification for schedule communique if planification date is greater than current date")>
        Sub ShouldNotSendPushNotificationForScheduleCommuniqueIfPlanificationDateIsGreaterThanCurrentDate()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperAdvancedParameters.AdvancedParameterCacheStub(New Dictionary(Of String, String) From {{"OnlyFirstNotificationBetweenSupervisors", "0"}, {"Customization", "0"}, {"VTLive.Notification.DisableUserField", "0"}, {"VTLive.Notification.DisabledNotificationsList", ""}})
                Fakes.ShimAccessHelper.CreateDataTableStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                         Dim tableName As String = helperDataLayer.GetTableNameFromQuery(strQuery)
                                                                                         If tableName = "Notifications" Then
                                                                                             Return helperDataLayer.CreateDataTableMock({"ID", "ShowOnDesktop"}, New Object()() {New Object() {"1", 0}})
                                                                                         ElseIf tableName = "Communiques" Then
                                                                                             Return helperDataLayer.CreateDataTableMock({"Id", "IdCompany", "Subject", "IdCreatedBy", "CreatedOn", "Mandatory", "Message", "AllowChangeResponse", "AllowedResponses", "ResponseLimitPercentage", "ExpirationDate", "PlanificationDate", "SentOn", "Status", "Archived"}, New Object()() {New Object() {"1", "1", "test", "1", Now, "1", "test", "1", "1", "50", "", Now, Now.AddDays(1), "1", "0"}})
                                                                                         ElseIf tableName = "GetEmployeeUserFieldValue" Then
                                                                                             Return New DataTable()
                                                                                         End If

                                                                                     End Function
                helperDataLayer.ExecuteSqlSpy()
                helperSecurity.GetEmployeeList()
                helperEmployee.GetEmployee()
                helperBase.SendNotificationPushToPassport()
                'Act
                Dim notificationFactory As New roNotificationTaskFactory
                Dim instanceGUID As Guid = Guid.NewGuid()
                Dim notificationTable As New DataTable("NotificationTask")
                notificationTable.Columns.Add("ID", GetType(Integer))
                notificationTable.Columns.Add("IDNotification", GetType(Integer))
                notificationTable.Columns.Add("Key1Numeric", GetType(Integer))
                notificationTable.Columns.Add("Key2Numeric", GetType(Integer))
                notificationTable.Columns.Add("Key5Numeric", GetType(Integer))
                notificationTable.Columns.Add("Key3DateTime", GetType(DateTime))
                notificationTable.Columns.Add("Key4DateTime", GetType(DateTime))
                notificationTable.Columns.Add("Key6DateTime", GetType(DateTime))
                notificationTable.Columns.Add("Executed", GetType(Boolean))
                notificationTable.Columns.Add("IsReaded", GetType(Boolean))
                notificationTable.Columns.Add("Parameters", GetType(String))
                notificationTable.Columns.Add("GUID", GetType(String))
                notificationTable.Columns.Add("Repetition", GetType(Integer))
                notificationTable.Columns.Add("NextRepetition", GetType(DateTime))
                notificationTable.Columns.Add("Condition", GetType(String))
                notificationTable.Columns.Add("Destination", GetType(String))
                notificationTable.Columns.Add("AllowMail", GetType(Boolean))
                notificationTable.Columns.Add("FiredDate", GetType(DateTime))
                notificationTable.Columns.Add("NotificationName", GetType(String))

                Dim newRow As DataRow = notificationTable.NewRow()
                newRow("ID") = 1
                newRow("IDNotification") = 4991
                newRow("Key1Numeric") = 123
                newRow("Key2Numeric") = 456
                newRow("Key5Numeric") = 789
                newRow("Key3DateTime") = DateTime.Now.AddDays(1)
                newRow("Key4DateTime") = DateTime.Now.AddHours(1)
                newRow("Key6DateTime") = DateTime.Now.AddHours(2)
                newRow("Executed") = True
                newRow("IsReaded") = False
                newRow("Parameters") = "Some parameters"
                newRow("GUID") = Guid.NewGuid().ToString()
                newRow("Repetition") = 3
                newRow("NextRepetition") = DateTime.Now.AddDays(1)
                newRow("Condition") = "<xml></xml>"
                newRow("Destination") = "<xml></xml>"
                newRow("AllowMail") = True
                newRow("FiredDate") = DateTime.Now.AddMinutes(30)
                newRow("NotificationName") = "Sample Notification"
                Dim send As Boolean = False

                'Act

                Dim manager As roNotificationTaskManager = notificationFactory.GetNotificationTaskManager(instanceGUID.ToString(), 92)
                If manager IsNot Nothing Then send = manager.Send(newRow)
                'Assert
                Assert.True(Not send AndAlso Not helperBase.SendPushNotificationWasCalled)
            End Using
        End Sub

        <Fact(DisplayName:="Should not send push notification for not published schedule communique")>
        Sub ShouldNotSendPushNotificationForNotPublishedScheduleCommunique()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperAdvancedParameters.AdvancedParameterCacheStub(New Dictionary(Of String, String) From {{"OnlyFirstNotificationBetweenSupervisors", "0"}, {"Customization", "0"}, {"VTLive.Notification.DisableUserField", "0"}, {"VTLive.Notification.DisabledNotificationsList", ""}})
                Fakes.ShimAccessHelper.CreateDataTableStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                         Dim tableName As String = helperDataLayer.GetTableNameFromQuery(strQuery)
                                                                                         If tableName = "Notifications" Then
                                                                                             Return helperDataLayer.CreateDataTableMock({"ID", "ShowOnDesktop"}, New Object()() {New Object() {"1", 0}})
                                                                                         ElseIf tableName = "Communiques" Then
                                                                                             Return helperDataLayer.CreateDataTableMock({"Id", "IdCompany", "Subject", "IdCreatedBy", "CreatedOn", "Mandatory", "Message", "AllowChangeResponse", "AllowedResponses", "ResponseLimitPercentage", "ExpirationDate", "PlanificationDate", "SentOn", "Status", "Archived"}, New Object()() {New Object() {"1", "1", "test", "1", Now, "1", "test", "1", "1", "50", "", Now, Now, "0", "0"}})
                                                                                         ElseIf tableName = "GetEmployeeUserFieldValue" Then
                                                                                             Return New DataTable()
                                                                                         End If

                                                                                     End Function
                helperDataLayer.ExecuteSqlSpy()
                helperSecurity.GetEmployeeList()
                helperEmployee.GetEmployee()
                helperBase.SendNotificationPushToPassport()
                'Act
                Dim notificationFactory As New roNotificationTaskFactory
                Dim instanceGUID As Guid = Guid.NewGuid()
                Dim notificationTable As New DataTable("NotificationTask")
                notificationTable.Columns.Add("ID", GetType(Integer))
                notificationTable.Columns.Add("IDNotification", GetType(Integer))
                notificationTable.Columns.Add("Key1Numeric", GetType(Integer))
                notificationTable.Columns.Add("Key2Numeric", GetType(Integer))
                notificationTable.Columns.Add("Key5Numeric", GetType(Integer))
                notificationTable.Columns.Add("Key3DateTime", GetType(DateTime))
                notificationTable.Columns.Add("Key4DateTime", GetType(DateTime))
                notificationTable.Columns.Add("Key6DateTime", GetType(DateTime))
                notificationTable.Columns.Add("Executed", GetType(Boolean))
                notificationTable.Columns.Add("IsReaded", GetType(Boolean))
                notificationTable.Columns.Add("Parameters", GetType(String))
                notificationTable.Columns.Add("GUID", GetType(String))
                notificationTable.Columns.Add("Repetition", GetType(Integer))
                notificationTable.Columns.Add("NextRepetition", GetType(DateTime))
                notificationTable.Columns.Add("Condition", GetType(String))
                notificationTable.Columns.Add("Destination", GetType(String))
                notificationTable.Columns.Add("AllowMail", GetType(Boolean))
                notificationTable.Columns.Add("FiredDate", GetType(DateTime))
                notificationTable.Columns.Add("NotificationName", GetType(String))

                Dim newRow As DataRow = notificationTable.NewRow()
                newRow("ID") = 1
                newRow("IDNotification") = 4991
                newRow("Key1Numeric") = 123
                newRow("Key2Numeric") = 456
                newRow("Key5Numeric") = 789
                newRow("Key3DateTime") = DateTime.Now
                newRow("Key4DateTime") = DateTime.Now.AddHours(1)
                newRow("Key6DateTime") = DateTime.Now.AddHours(2)
                newRow("Executed") = True
                newRow("IsReaded") = False
                newRow("Parameters") = "Some parameters"
                newRow("GUID") = Guid.NewGuid().ToString()
                newRow("Repetition") = 3
                newRow("NextRepetition") = DateTime.Now.AddDays(1)
                newRow("Condition") = "<xml></xml>"
                newRow("Destination") = "<xml></xml>"
                newRow("AllowMail") = True
                newRow("FiredDate") = DateTime.Now.AddMinutes(30)
                newRow("NotificationName") = "Sample Notification"
                Dim send As Boolean = False

                'Act

                Dim manager As roNotificationTaskManager = notificationFactory.GetNotificationTaskManager(instanceGUID.ToString(), 92)
                If manager IsNot Nothing Then send = manager.Send(newRow)
                'Assert
                Assert.True(Not send AndAlso Not helperBase.SendPushNotificationWasCalled)
            End Using
        End Sub

    End Class

End Namespace