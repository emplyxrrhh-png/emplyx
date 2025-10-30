Imports System.Data.Common
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Punch
Imports Robotics.Base.VTEmployees.Employee
Imports Robotics.Base.VTNotifications.Notifications
Imports Robotics.DataLayer

Public Class PunchHelper

    Public Enum SqlExecuteString
        None
        GeneralExecute
        SendCapacityZoneNotification
    End Enum

    Public Property ExecuteSqlWasCalled As SqlExecuteString = SqlExecuteString.None

    Public Property RecalcTypeCalled As Boolean = False
    Public Property EngineLaunchedCalled As Boolean = False

    Public Property MaskNotificationCalled As Boolean = False
    Public Property TemperatureNotificationCalled As Boolean = False
    Public Property RemarksSaved As String = String.Empty
    Public Property NotReliableCauseSaved As String = String.Empty

    Function FillEmptyMock(dataTableArray As DataTable(), startRecord As Integer, maxRecords As Integer, dbCommand As DbCommand, commandBehaviour As CommandBehavior)
        Dim dataTable As DataTable = dataTableArray.GetValue(startRecord)
        dataTable.Columns.Add("ID")
        dataTable.Columns.Add("IDEmployee")
        dataTable.Columns.Add("IDCredential")
        dataTable.Columns.Add("VerificationType")
        dataTable.Columns.Add("Type")
        dataTable.Columns.Add("ActualType")
        dataTable.Columns.Add("InvalidType")
        dataTable.Columns.Add("ShiftDate")
        dataTable.Columns.Add("DateTime")
        dataTable.Columns.Add("IDTerminal")
        dataTable.Columns.Add("IDReader")
        dataTable.Columns.Add("IDZone")
        dataTable.Columns.Add("ZoneIsNotReliable")
        dataTable.Columns.Add("TypeData")
        dataTable.Columns.Add("TypeDetails")
        dataTable.Columns.Add("Location")
        dataTable.Columns.Add("LocationZone")
        dataTable.Columns.Add("FullAddress")
        dataTable.Columns.Add("IP")
        dataTable.Columns.Add("Action")
        dataTable.Columns.Add("IDPassport")
        dataTable.Columns.Add("Field1")
        dataTable.Columns.Add("Field2")
        dataTable.Columns.Add("Field3")
        dataTable.Columns.Add("Field4")
        dataTable.Columns.Add("Field5")
        dataTable.Columns.Add("Field6")
        dataTable.Columns.Add("Exported")
        dataTable.Columns.Add("SystemDetails")
        dataTable.Columns.Add("IsNotReliable")
        dataTable.Columns.Add("TimeStamp")
        dataTable.Columns.Add("TimeZone")
        dataTable.Columns.Add("InTelecommute")
        dataTable.Columns.Add("IDRequest")
        dataTable.Columns.Add("MaskAlert")
        dataTable.Columns.Add("Timespan")
        dataTable.Columns.Add("CRC")
        dataTable.Columns.Add("Source")
        dataTable.Columns.Add("HasPhoto")
        dataTable.Columns.Add("PhotoOnAzure")
        dataTable.Columns.Add("TemperatureAlert")
        dataTable.Columns.Add("Workcenter")
        dataTable.Columns.Add("Remarks")
        dataTable.Columns.Add("NotReliableCause")
        Return startRecord
    End Function

    Function FillMockWithOnePunch(dataTableArray As DataTable(), startRecord As Integer, maxRecords As Integer, dbCommand As DbCommand, commandBehaviour As CommandBehavior, oPunch As roPunch)
        Dim dataTable As DataTable = dataTableArray.GetValue(startRecord)
        dataTable.Columns.Add("ID")
        dataTable.Columns("ID").DefaultValue = oPunch.ID
        dataTable.Columns.Add("IDEmployee")
        dataTable.Columns("IDEmployee").DefaultValue = oPunch.IDEmployee
        dataTable.Columns.Add("IDCredential")
        dataTable.Columns.Add("VerificationType")
        dataTable.Columns.Add("Type")
        dataTable.Columns("Type").DefaultValue = oPunch.Type
        dataTable.Columns.Add("ActualType")
        dataTable.Columns("ActualType").DefaultValue = oPunch.ActualType
        dataTable.Columns.Add("InvalidType")
        dataTable.Columns.Add("ShiftDate")
        dataTable.Columns("ShiftDate").DefaultValue = oPunch.ShiftDate
        dataTable.Columns.Add("DateTime")
        dataTable.Columns("DateTime").DefaultValue = oPunch.DateTime
        dataTable.Columns.Add("IDTerminal")
        dataTable.Columns.Add("IDReader")
        dataTable.Columns.Add("IDZone")
        dataTable.Columns.Add("ZoneIsNotReliable")
        dataTable.Columns.Add("TypeData")
        dataTable.Columns.Add("TypeDetails")
        dataTable.Columns.Add("Location")
        dataTable.Columns.Add("LocationZone")
        dataTable.Columns.Add("FullAddress")
        dataTable.Columns.Add("IP")
        dataTable.Columns.Add("Action")
        dataTable.Columns.Add("IDPassport")
        dataTable.Columns.Add("Field1")
        dataTable.Columns.Add("Field2")
        dataTable.Columns.Add("Field3")
        dataTable.Columns.Add("Field4")
        dataTable.Columns.Add("Field5")
        dataTable.Columns.Add("Field6")
        dataTable.Columns.Add("Exported")
        dataTable.Columns.Add("SystemDetails")
        dataTable.Columns.Add("IsNotReliable")
        dataTable.Columns.Add("TimeStamp")
        dataTable.Columns.Add("TimeZone")
        dataTable.Columns.Add("InTelecommute")
        dataTable.Columns.Add("IDRequest")
        dataTable.Columns.Add("MaskAlert")
        dataTable.Columns.Add("Timespan")
        dataTable.Columns.Add("CRC")
        dataTable.Columns.Add("Source")
        dataTable.Columns.Add("HasPhoto")
        dataTable.Columns.Add("PhotoOnAzure")
        dataTable.Columns.Add("TemperatureAlert")
        dataTable.Columns.Add("Workcenter")
        dataTable.Columns.Add("Remarks")
        dataTable.Columns.Add("NotReliableCause")
        dataTable.Rows.Add(dataTable.NewRow())
        Return startRecord
    End Function

    Public Sub EmptyPunchesTableFromAdapterStub()
        Robotics.DataLayer.Fakes.ShimAccessHelper.CreateDataAdapterDbCommandBoolean =
            Function()
                Dim stubDbDataAdapter = New Fakes.StubDbDataAdapter With {
                .FillDataTableArrayInt32Int32IDbCommandCommandBehavior =
                    Function(dataTableArray, startRecord, maxRecords, dbCommand, commandBehaviour)
                        Return FillEmptyMock(dataTableArray, startRecord, maxRecords, dbCommand, commandBehaviour)
                    End Function,
                    .UpdateDataRowArrayDataTableMapping =
                    Function(dataRows, dataTable)
                        If Not IsDBNull(dataRows(0)("Remarks")) Then
                            RemarksSaved = dataRows(0)("Remarks")
                        End If
                        If Not IsDBNull(dataRows(0)("NotReliableCause")) Then
                            NotReliableCauseSaved = dataRows(0)("NotReliableCause")
                        End If
                    End Function
                }
                Return stubDbDataAdapter

            End Function
    End Sub

    Public Sub PunchesTableFromAdapterStub(oPunch As roPunch)
        Robotics.DataLayer.Fakes.ShimAccessHelper.CreateDataAdapterDbCommandBoolean =
            Function()
                Dim stubDbDataAdapter = New Fakes.StubDbDataAdapter With {
                .FillDataTableArrayInt32Int32IDbCommandCommandBehavior =
                    Function(dataTableArray, startRecord, maxRecords, dbCommand, commandBehaviour)
                        Return FillMockWithOnePunch(dataTableArray, startRecord, maxRecords, dbCommand, commandBehaviour, oPunch)
                    End Function,
                    .UpdateDataRowArrayDataTableMapping =
                    Function(dataRows, dataTable)
                        If Not IsDBNull(dataRows(0)("Remarks")) Then
                            RemarksSaved = dataRows(0)("Remarks")
                        End If
                        If Not IsDBNull(dataRows(0)("NotReliableCause")) Then
                            NotReliableCauseSaved = dataRows(0)("NotReliableCause")
                        End If
                    End Function
                }

                Return stubDbDataAdapter
            End Function
    End Sub

    Public Function SaveStub()
        Robotics.Base.VTBusiness.Punch.Fakes.ShimroPunch.AllInstances.SaveBooleanBooleanBooleanBooleanBooleanBooleanBooleanBooleanBooleanStringString =
            Function(oPunch, savePhoto, savePhotoOnAzure, savePhotoOnAzureWithNewName, savePhotoOnAzureWithNewNameAndExtension, savePhotoOnAzureWithNewNameAndExtensionAndFolder, savePhotoOnAzureWithNewNameAndExtensionAndFolderAndContainer, savePhotoOnAzureWithNewNameAndExtensionAndFolderAndContainerAndBlob, savePhotoOnAzureWithNewNameAndExtensionAndFolderAndContainerAndBlobAndResize, savePhotoOnAzureWithNewNameAndExtensionAndFolderAndContainerAndBlobAndResizeAndQuality, remarks, causeNotReliable)
                Return True
            End Function
    End Function

    Public Sub GenerateNotificationStub()
        Robotics.Base.VTNotifications.Notifications.Fakes.ShimroNotification.GetNotificationsStringroNotificationStateBooleanBoolean = Function(ByVal _SQLFilter As String, ByVal _State As roNotificationState,
                ByVal bAudit As Boolean, ByVal bolIncludeSystem As Boolean)
                                                                                                                                           If _SQLFilter.ToLower.Replace(" ", "").Contains("idtype=86andactivated=1") Then
                                                                                                                                               ExecuteSqlWasCalled = SqlExecuteString.SendCapacityZoneNotification
                                                                                                                                           End If
                                                                                                                                       End Function
    End Sub

    Public Function DeleteStub(Optional ByVal ret As Boolean = True)
        Robotics.Base.VTBusiness.Punch.Fakes.ShimroPunch.AllInstances.DeleteBooleanBooleanBooleanBooleanBoolean =
            Function(oPunch, deletePhoto, deletePhotoOnAzure, deletePhotoOnAzureWithNewName, deletePhotoOnAzureWithNewNameAndExtension, deletePhotoOnAzureWithNewNameAndExtensionAndFolder)
                Return ret
            End Function
    End Function

    Public Function CalculateTypeSpy()
        Robotics.Base.VTBusiness.Punch.Fakes.ShimroPunch.AllInstances.CalculateTypeAttBoolean = Function()
                                                                                                    RecalcTypeCalled = True
                                                                                                End Function
    End Function


    Public Function EngineLaunchedSpy()
        Robotics.Base.VTBusiness.Punch.Fakes.ShimroPunch.AllInstances.UpdateDailySchedule = Function()
                                                                                                EngineLaunchedCalled = True
                                                                                            End Function
    End Function

    Public Function MaskNotificationSpy()
        Robotics.Base.VTNotifications.Notifications.Fakes.ShimroNotification.GetNotificationsStringroNotificationStateBooleanBoolean = Function(ByVal _SQLFilter As String, ByVal _State As roNotificationState,
                                                ByVal bAudit As Boolean, ByVal bolIncludeSystem As Boolean)
                                                                                                                                           If _SQLFilter.ToLower.Replace(" ", "").Contains("idtype=69andactivated=1") Then
                                                                                                                                               MaskNotificationCalled = True
                                                                                                                                           End If
                                                                                                                                       End Function
    End Function

    Public Function TemperatureNotificationSpy()
        Robotics.Base.VTNotifications.Notifications.Fakes.ShimroNotification.GetNotificationsStringroNotificationStateBooleanBoolean = Function(ByVal _SQLFilter As String, ByVal _State As roNotificationState,
                                                ByVal bAudit As Boolean, ByVal bolIncludeSystem As Boolean)
                                                                                                                                           If _SQLFilter.ToLower.Replace(" ", "").Contains("idtype=73andactivated=1") Then
                                                                                                                                               TemperatureNotificationCalled = True
                                                                                                                                           End If
                                                                                                                                       End Function
    End Function

    Public Function GetLastPunchPres()
        Robotics.Base.VTBusiness.Punch.Fakes.ShimroPunch.AllInstances.GetLastPunchPresPunchStatusRefDateTimeRefInt64Ref =
            Function(oPunch As roPunch, ByRef oPunchStatus As PunchStatus, ByRef dDateTime As DateTime, ByRef idTerminal As Long) As Boolean
                oPunchStatus = PunchStatus.In_
                dDateTime = DateTime.Now
                idTerminal = 1
                oPunch.ID = 1
                Return True
            End Function
    End Function



End Class

