Imports System.ComponentModel
Imports Robotics.DataLayer
Imports Robotics.ExternalSystems.DataLink.RoboticsExternAccess
Imports Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Core.DTOs
Imports VT_XU_Base
Imports VT_XU_Common
Imports VT_XU_Security
Imports Xunit

Namespace Unit.Test

    <Collection("Schedule")>
    <CollectionDefinition("Schedule", DisableParallelization:=True)>
    <Category("Schedule")>
    Public Class ScheduleTest

        Private ReadOnly helperAdvancedParameters As AdvancedParametersHelper
        Private ReadOnly helperDataLayer As DatalayerHelper
        Private ReadOnly helperPassport As PassportHelper
        Private ReadOnly helperBusiness As BusinessHelper
        Private ReadOnly helperEmployees As EmployeeHelper
        Private ReadOnly helperCalendar As CalendarHelper

        Sub New()
            helperAdvancedParameters = New AdvancedParametersHelper
            helperDataLayer = New DatalayerHelper
            helperPassport = New PassportHelper
            helperBusiness = New BusinessHelper
            helperEmployees = New EmployeeHelper
            helperCalendar = New CalendarHelper
        End Sub

        <Fact(DisplayName:="Should Return Holidays If Timestamp Criteria Is Not Specified And There Are Days With Holiday Schedule Without Timestamp On Db")>
        Sub ShouldReturnHolidaysIfTimestampCriteriaIsNotSpecifiedAndThereAreDaysWithHolidayScheduleWithoutTimestampOnDb()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim oHolidays As List(Of roHoliday) = New List(Of roHoliday)()
                Dim returnCode As ReturnCode = New ReturnCode()
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                Dim tMock As New DataTableMock With {.columns = {"Date", "EmployeeKey", "IdEmployee", "ExportCode", "ShiftName", "Timestamp"}, .values = New Object()() {New Object() {New Date(2023, 1, 2), 1, 1, 1, "Test", DBNull.Value}}}
                dDataTStub.Add("DailySchedule", tMock)
                Dim tMockPH As New DataTableMock With {.columns = {"ID"}, .values = New Object()() {}}
                dDataTStub.Add("ProgrammedHolidays", tMockPH)
                helperDataLayer.CreateDataTableWithoutTimeoutsStub(dDataTStub)
                helperPassport.PassportStub(1, helperDataLayer)
                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.GetHolidays(oHolidays, Nothing, New Date(2023, 1, 1), New Date(2023, 1, 30), returnCode, Nothing)

                'Assert
                Assert.True(returnCode = ReturnCode._OK And oHolidays.Count > 0)

            End Using
        End Sub

        <Fact(DisplayName:="Should Return Holidays If Timestamp Criteria Is Not Specified And There Are Days With Holiday Schedule With Timestamp On Db")>
        Sub ShouldReturnHolidaysIfTimestampCriteriaIsNotSpecifiedAndThereAreDaysWithHolidayScheduleWithTimestampOnDb()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim oHolidays As List(Of roHoliday) = New List(Of roHoliday)()
                Dim returnCode As ReturnCode = New ReturnCode()
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                Dim tMock As New DataTableMock With {.columns = {"Date", "EmployeeKey", "IdEmployee", "ExportCode", "ShiftName", "Timestamp"}, .values = New Object()() {New Object() {New Date(2023, 1, 2), 1, 1, 1, "Test", New Date(2023, 1, 2)}}}
                dDataTStub.Add("DailySchedule", tMock)
                Dim tMockPH As New DataTableMock With {.columns = {"ID"}, .values = New Object()() {}}
                dDataTStub.Add("ProgrammedHolidays", tMockPH)
                helperDataLayer.CreateDataTableWithoutTimeoutsStub(dDataTStub)
                helperPassport.PassportStub(1, helperDataLayer)
                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.GetHolidays(oHolidays, Nothing, New Date(2023, 1, 1), New Date(2023, 1, 30), returnCode, Nothing)

                'Assert
                Assert.True(returnCode = ReturnCode._OK And oHolidays.Count > 0)

            End Using
        End Sub

        <Fact(DisplayName:="Should Return Holidays If Timestamp Criteria Is Not Specified And There Are Programmed Holidays Without Timestamp On Db")>
        Sub ShouldReturnHolidaysIfTimestampCriteriaIsNotSpecifiedAndThereAreProgrammedHolidaysWithoutTimestampOnDb()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim oHolidays As List(Of roHoliday) = New List(Of roHoliday)()
                Dim returnCode As ReturnCode = New ReturnCode()
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                Dim tMock As New DataTableMock With {.columns = {"Date", "EmployeeKey", "IdEmployee", "ExportCode", "ShiftName", "Timestamp"}, .values = New Object()() {}}
                dDataTStub.Add("DailySchedule", tMock)
                Dim tMockPH As New DataTableMock With {.columns = {"ID", "IDEmployee", "IDCause", "Date", "BeginTime", "EndTime", "duration", "AllDay", "ExportCode", "EmployeeKey", "CauseName", "Timestamp"},
                    .values = New Object()() {New Object() {1, 1, 1, New Date(2023, 1, 2), New DateTime(2023, 1, 2, 0, 0, 0), New DateTime(2023, 1, 2, 23, 59, 0), 1, True, 1, 1, "Test", DBNull.Value}}}
                dDataTStub.Add("ProgrammedHolidays", tMockPH)
                helperDataLayer.CreateDataTableWithoutTimeoutsStub(dDataTStub)
                helperPassport.PassportStub(1, helperDataLayer)
                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.GetHolidays(oHolidays, Nothing, New Date(2023, 1, 1), New Date(2023, 1, 30), returnCode, Nothing)

                'Assert
                Assert.True(returnCode = ReturnCode._OK And oHolidays.Count > 0)

            End Using
        End Sub

        <Fact(DisplayName:="Should Return Holidays If Timestamp Criteria Is Not Specified And There Are Programmed Holidays With Timestamp On Db")>
        Sub ShouldReturnHolidaysIfTimestampCriteriaIsNotSpecifiedAndThereAreProgrammedHolidaysWithTimestampOnDb()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim oHolidays As List(Of roHoliday) = New List(Of roHoliday)()
                Dim returnCode As ReturnCode = New ReturnCode()
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                Dim tMock As New DataTableMock With {.columns = {"Date", "EmployeeKey", "IdEmployee", "ExportCode", "ShiftName", "Timestamp"}, .values = New Object()() {}}
                dDataTStub.Add("DailySchedule", tMock)
                Dim tMockPH As New DataTableMock With {.columns = {"ID", "IDEmployee", "IDCause", "Date", "BeginTime", "EndTime", "duration", "AllDay", "ExportCode", "EmployeeKey", "CauseName", "Timestamp"},
                    .values = New Object()() {New Object() {1, 1, 1, New Date(2023, 1, 2), New DateTime(2023, 1, 2, 0, 0, 0), New DateTime(2023, 1, 2, 23, 59, 0), 1, True, 1, 1, "Test", New Date(2023, 1, 2)}}}
                dDataTStub.Add("ProgrammedHolidays", tMockPH)
                helperDataLayer.CreateDataTableWithoutTimeoutsStub(dDataTStub)
                helperPassport.PassportStub(1, helperDataLayer)
                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.GetHolidays(oHolidays, Nothing, New Date(2023, 1, 1), New Date(2023, 1, 30), returnCode, Nothing)

                'Assert
                Assert.True(returnCode = ReturnCode._OK And oHolidays.Count > 0)

            End Using
        End Sub

        <Fact(DisplayName:="Should Not Return Holidays If Timestamp Criteria Is Not Specified And There Arent Days With Holiday Schedule Or Programmed Holiday In Period")>
        Sub ShouldNotReturnHolidaysIfTimestampCriteriaIsNotSpecifiedAndThereArentDaysWithHolidayScheduleOrProgrammedHolidayInPeriod()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim oHolidays As List(Of roHoliday) = New List(Of roHoliday)()
                Dim returnCode As ReturnCode = New ReturnCode()
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                Dim tMock As New DataTableMock With {.columns = {"Date", "EmployeeKey", "IdEmployee", "ExportCode", "ShiftName", "Timestamp"}, .values = New Object()() {}}
                dDataTStub.Add("DailySchedule", tMock)
                Dim tMockPH As New DataTableMock With {.columns = {"ID"}, .values = New Object()() {}}
                dDataTStub.Add("ProgrammedHolidays", tMockPH)
                helperDataLayer.CreateDataTableWithoutTimeoutsStub(dDataTStub)
                helperPassport.PassportStub(1, helperDataLayer)
                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.GetHolidays(oHolidays, Nothing, New Date(2023, 1, 15), New Date(2023, 1, 30), returnCode, Nothing)

                'Assert
                Assert.True(returnCode = ReturnCode._OK And oHolidays.Count = 0)

            End Using
        End Sub

        <Fact(DisplayName:="Should Return Deleted Holidays If Timestamp Criteria Is Specified")>
        Sub ShouldReturnDeletedHolidaysIfTimestampCriteriaIsSpecified()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim oHolidays As List(Of roHoliday) = New List(Of roHoliday)()
                Dim returnCode As ReturnCode = New ReturnCode()
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                Dim tMock As New DataTableMock With {.columns = {"Date", "EmployeeKey", "IdEmployee", "ExportCode", "ShiftName", "Timestamp"}, .values = New Object()() {}}
                dDataTStub.Add("DailySchedule", tMock)
                Dim tMockPH As New DataTableMock With {.columns = {"ID"}, .values = New Object()() {}}
                dDataTStub.Add("ProgrammedHolidays", tMockPH)
                helperDataLayer.CreateDataTableWithoutTimeoutsStub(dDataTStub)
                helperDataLayer.CreateDataTableWithoutTimeoutsSpy()
                helperPassport.PassportStub(1, helperDataLayer)
                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.GetHolidays(oHolidays, Nothing, New Date(2023, 1, 15), New Date(2023, 1, 30), returnCode, Nothing, True)

                'Assert
                Assert.True(returnCode = ReturnCode._OK And helperDataLayer.ExcutedCreateDataTable = DatalayerHelper.CreateDataTableString.SelectDeletedHolidays)

            End Using
        End Sub

        <Fact(DisplayName:="Should Noy Return Deleted Holidays If Timestamp Criteria Is Not Specified")>
        Sub ShouldNoyReturnDeletedHolidaysIfTimestampCriteriaIsNotSpecified()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim oHolidays As List(Of roHoliday) = New List(Of roHoliday)()
                Dim returnCode As ReturnCode = New ReturnCode()
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                Dim tMock As New DataTableMock With {.columns = {"Date", "EmployeeKey", "IdEmployee", "ExportCode", "ShiftName", "Timestamp"}, .values = New Object()() {}}
                dDataTStub.Add("DailySchedule", tMock)
                Dim tMockPH As New DataTableMock With {.columns = {"ID"}, .values = New Object()() {}}
                dDataTStub.Add("ProgrammedHolidays", tMockPH)
                helperDataLayer.CreateDataTableWithoutTimeoutsStub(dDataTStub)
                helperDataLayer.CreateDataTableWithoutTimeoutsSpy()
                helperPassport.PassportStub(1, helperDataLayer)
                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.GetHolidays(oHolidays, Nothing, New Date(2023, 1, 15), New Date(2023, 1, 30), returnCode, Nothing)

                'Assert
                Assert.True(returnCode = ReturnCode._OK And helperDataLayer.ExcutedCreateDataTable = DatalayerHelper.CreateDataTableString.None)

            End Using
        End Sub

        <Fact(DisplayName:="Should Return Holidays If Timestamp Criteria Is Specified And There Are Days With Holiday Schedule Without Timestamp On Db")>
        Sub ShouldReturnHolidaysIfTimestampCriteriaIsSpecifiedAndThereAreDaysWithHolidayScheduleWithoutTimestampOnDb()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim oHolidays As List(Of roHoliday) = New List(Of roHoliday)()
                Dim returnCode As ReturnCode = New ReturnCode()
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                Dim tMock As New DataTableMock With {.columns = {"Date", "EmployeeKey", "IdEmployee", "ExportCode", "ShiftName", "Timestamp"}, .values = New Object()() {New Object() {New Date(2023, 1, 2), 1, 1, 1, "Test", DBNull.Value}}}
                dDataTStub.Add("DailySchedule", tMock)
                Dim tMockPH As New DataTableMock With {.columns = {"ID"}, .values = New Object()() {}}
                dDataTStub.Add("ProgrammedHolidays", tMockPH)
                helperDataLayer.CreateDataTableWithoutTimeoutsStub(dDataTStub)
                helperPassport.PassportStub(1, helperDataLayer)
                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.GetHolidays(oHolidays, Nothing, New Date(2023, 1, 1), New Date(2023, 1, 30), returnCode, Nothing, True)

                'Assert
                Assert.True(returnCode = ReturnCode._OK And oHolidays.Count > 0)

            End Using
        End Sub

        <Fact(DisplayName:="Should Return Holidays If Timestamp Criteria Is Specified And There Are Days With Holiday Schedule With Timestamp On Db")>
        Sub ShouldReturnHolidaysIfTimestampCriteriaIsSpecifiedAndThereAreDaysWithHolidayScheduleWithTimestampOnDb()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim oHolidays As List(Of roHoliday) = New List(Of roHoliday)()
                Dim returnCode As ReturnCode = New ReturnCode()
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                Dim tMock As New DataTableMock With {.columns = {"Date", "EmployeeKey", "IdEmployee", "ExportCode", "ShiftName", "Timestamp"}, .values = New Object()() {New Object() {New Date(2023, 1, 2), 1, 1, 1, "Test", New Date(2023, 1, 2)}}}
                dDataTStub.Add("DailySchedule", tMock)
                Dim tMockPH As New DataTableMock With {.columns = {"ID"}, .values = New Object()() {}}
                dDataTStub.Add("ProgrammedHolidays", tMockPH)
                helperDataLayer.CreateDataTableWithoutTimeoutsStub(dDataTStub)
                helperPassport.PassportStub(1, helperDataLayer)
                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.GetHolidays(oHolidays, Nothing, New Date(2023, 1, 1), New Date(2023, 1, 30), returnCode, Nothing, True)

                'Assert
                Assert.True(returnCode = ReturnCode._OK And oHolidays.Count > 0)

            End Using
        End Sub

        <Fact(DisplayName:="Should Return Holidays If Timestamp Criteria Is Specified And There Are Programmed Holidays Without Timestamp On Db")>
        Sub ShouldReturnHolidaysIfTimestampCriteriaIsSpecifiedAndThereAreProgrammedHolidaysWithoutTimestampOnDb()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim oHolidays As List(Of roHoliday) = New List(Of roHoliday)()
                Dim returnCode As ReturnCode = New ReturnCode()
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                Dim tMock As New DataTableMock With {.columns = {"Date", "EmployeeKey", "IdEmployee", "ExportCode", "ShiftName", "Timestamp"}, .values = New Object()() {}}
                dDataTStub.Add("DailySchedule", tMock)
                Dim tMockDH As New DataTableMock With {.columns = {"IDEmployee", "IDHoursHoliday", "PlannedDate", "Timestamp", "EmployeeKey"}, .values = New Object()() {}}
                dDataTStub.Add("DeletedProgrammedHolidays", tMockDH)
                Dim tMockPH As New DataTableMock With {.columns = {"ID", "IDEmployee", "IDCause", "Date", "BeginTime", "EndTime", "duration", "AllDay", "ExportCode", "EmployeeKey", "CauseName", "Timestamp"},
                    .values = New Object()() {New Object() {1, 1, 1, New Date(2023, 1, 2), New DateTime(2023, 1, 2, 0, 0, 0), New DateTime(2023, 1, 2, 23, 59, 0), 1, True, 1, 1, "Test", DBNull.Value}}}
                dDataTStub.Add("ProgrammedHolidays", tMockPH)
                helperDataLayer.CreateDataTableWithoutTimeoutsStub(dDataTStub)
                helperPassport.PassportStub(1, helperDataLayer)
                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.GetHolidays(oHolidays, Nothing, New Date(2023, 1, 1), New Date(2023, 1, 30), returnCode, Nothing, True)

                'Assert
                Assert.True(returnCode = ReturnCode._OK And oHolidays.Count > 0)

            End Using
        End Sub

        <Fact(DisplayName:="Should Return Holidays If Timestamp Criteria Is Specified And There Are Programmed Holidays With Timestamp On Db")>
        Sub ShouldReturnHolidaysIfTimestampCriteriaIsSpecifiedAndThereAreProgrammedHolidaysWithTimestampOnDb()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim oHolidays As List(Of roHoliday) = New List(Of roHoliday)()
                Dim returnCode As ReturnCode = New ReturnCode()
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                Dim tMock As New DataTableMock With {.columns = {"Date", "EmployeeKey", "IdEmployee", "ExportCode", "ShiftName", "Timestamp"}, .values = New Object()() {}}
                dDataTStub.Add("DailySchedule", tMock)
                Dim tMockDH As New DataTableMock With {.columns = {"IDEmployee", "IDHoursHoliday", "PlannedDate", "Timestamp", "EmployeeKey"}, .values = New Object()() {}}
                dDataTStub.Add("DeletedProgrammedHolidays", tMockDH)
                Dim tMockPH As New DataTableMock With {.columns = {"ID", "IDEmployee", "IDCause", "Date", "BeginTime", "EndTime", "duration", "AllDay", "ExportCode", "EmployeeKey", "CauseName", "Timestamp"},
                    .values = New Object()() {New Object() {1, 1, 1, New Date(2023, 1, 2), New DateTime(2023, 1, 2, 0, 0, 0), New DateTime(2023, 1, 2, 23, 59, 0), 1, True, 1, 1, "Test", New Date(2023, 1, 2)}}}
                dDataTStub.Add("ProgrammedHolidays", tMockPH)
                helperDataLayer.CreateDataTableWithoutTimeoutsStub(dDataTStub)
                helperPassport.PassportStub(1, helperDataLayer)
                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.GetHolidays(oHolidays, Nothing, New Date(2023, 1, 1), New Date(2023, 1, 30), returnCode, Nothing, True)

                'Assert
                Assert.True(returnCode = ReturnCode._OK And oHolidays.Count > 0)

            End Using
        End Sub

        <Fact(DisplayName:="Should Return Deleted Holidays If Timestamp Criteria Is Specified And There Are Deleted Holidays With Timestamp On Db")>
        Sub ShouldReturnDeletedHolidaysIfTimestampCriteriaIsSpecifiedAndThereAreDeletedHolidaysWithTimestampOnDb()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim oHolidays As List(Of roHoliday) = New List(Of roHoliday)()
                Dim returnCode As ReturnCode = New ReturnCode()
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                Dim tMock As New DataTableMock With {.columns = {"Date", "EmployeeKey", "IdEmployee", "ExportCode", "ShiftName", "Timestamp"}, .values = New Object()() {}}
                dDataTStub.Add("DailySchedule", tMock)
                Dim tMockDH As New DataTableMock With {.columns = {"IDEmployee", "IDHoursHoliday", "PlannedDate", "Timestamp", "EmployeeKey"},
                    .values = New Object()() {New Object() {1, DBNull.Value, New Date(2023, 1, 2), New DateTime(2023, 1, 2, 0, 0, 0), 1}}}
                dDataTStub.Add("DeletedProgrammedHolidays", tMockDH)
                Dim tMockPH As New DataTableMock With {.columns = {"ID", "IDEmployee", "IDCause", "Date", "BeginTime", "EndTime", "duration", "AllDay", "ExportCode", "EmployeeKey", "CauseName", "Timestamp"}, .values = New Object()() {}}
                dDataTStub.Add("ProgrammedHolidays", tMockPH)
                helperDataLayer.CreateDataTableWithoutTimeoutsStub(dDataTStub)
                helperPassport.PassportStub(1, helperDataLayer)
                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.GetHolidays(oHolidays, Nothing, New Date(2023, 1, 1), New Date(2023, 1, 30), returnCode, Nothing, True)

                'Assert
                Assert.True(returnCode = ReturnCode._OK And oHolidays.Count > 0)

            End Using
        End Sub

        <Fact(DisplayName:="Should Not Return Holidays If Timestamp Criteria Is Specified And There Are Not Days With Holiday Schedule Or Programmed Holiday In Period")>
        Sub ShouldNotReturnHolidaysIfTimestampCriteriaIsSpecifiedAndThereAreNotDaysWithHolidayScheduleOrProgrammedHolidayInPeriod()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim oHolidays As List(Of roHoliday) = New List(Of roHoliday)()
                Dim returnCode As ReturnCode = New ReturnCode()
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                Dim tMock As New DataTableMock With {.columns = {"Date", "EmployeeKey", "IdEmployee", "ExportCode", "ShiftName", "Timestamp"}, .values = New Object()() {}}
                dDataTStub.Add("DailySchedule", tMock)
                Dim tMockPH As New DataTableMock With {.columns = {"ID"}, .values = New Object()() {}}
                dDataTStub.Add("ProgrammedHolidays", tMockPH)
                helperDataLayer.CreateDataTableWithoutTimeoutsStub(dDataTStub)
                helperPassport.PassportStub(1, helperDataLayer)
                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.GetHolidays(oHolidays, Nothing, New Date(2023, 1, 15), New Date(2023, 1, 30), returnCode, Nothing, True)

                'Assert
                Assert.True(returnCode = ReturnCode._OK And oHolidays.Count = 0)

            End Using
        End Sub

        <Fact(DisplayName:="Should Not Return Shift Base Id If Current Shift Is Not Holiday Shift")>
        Sub ShouldNotReturnShiftBaseIdIfCurrentShiftIsNotHolidayShift()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim calendarCriteria As roCalendarCriteria = New roCalendarCriteria With {.StartDate = New Robotics.VTBase.roWCFDate(New Date(2025, 1, 1)), .EndDate = New Robotics.VTBase.roWCFDate(New Date(2025, 1, 1)), .IDEmployees = New String() {1}}
                Dim calendarList As List(Of roCalendar) = New List(Of roCalendar)()
                Dim returnCode As ReturnCode = New ReturnCode()
                Dim returnText As String = ""
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                Dim tMock As New DataTableMock With {.columns = {"Date", "EmployeeKey", "IdEmployee", "ExportCode", "ShiftName", "Timestamp"}, .values = New Object()() {}}
                dDataTStub.Add("DailySchedule", tMock)
                Dim shiftsMock As New DataTableMock With {.columns = {"ID", "Export"},
                    .values = New Object()() {New Object() {1, "VAC"}, New Object() {2, "HOR"}}}
                dDataTStub.Add("Shifts", shiftsMock)
                helperDataLayer.CreateDataTableStub(dDataTStub)
                helperEmployees.GetIDEmployeesFromUserFieldValueStub()
                helperPassport.PassportStub(1, helperDataLayer)
                helperCalendar.LoadCellsByCalendar(2, Nothing, False)
                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.GetCalendar(calendarCriteria, calendarList, returnCode, returnText)

                'Assert
                Assert.True(returnCode = ReturnCode._OK And calendarList(0).IDShiftBase Is Nothing)

            End Using
        End Sub

        <Fact(DisplayName:="Should Return Shift Base Id If Current Shift Is Holiday Shift")>
        Sub ShouldReturnShiftBaseIdIfCurrentShiftIsHolidayShift()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim calendarCriteria As roCalendarCriteria = New roCalendarCriteria With {.StartDate = New Robotics.VTBase.roWCFDate(New Date(2025, 1, 1)), .EndDate = New Robotics.VTBase.roWCFDate(New Date(2025, 1, 1)), .IDEmployees = New String() {1}}
                Dim calendarList As List(Of roCalendar) = New List(Of roCalendar)()
                Dim returnCode As ReturnCode = New ReturnCode()
                Dim returnText As String = ""
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                Dim tMock As New DataTableMock With {.columns = {"Date", "EmployeeKey", "IdEmployee", "ExportCode", "ShiftName", "Timestamp"}, .values = New Object()() {}}
                dDataTStub.Add("DailySchedule", tMock)
                Dim shiftsMock As New DataTableMock With {.columns = {"ID", "Export"},
                    .values = New Object()() {New Object() {1, "VAC"}, New Object() {2, "HOR"}}}
                dDataTStub.Add("Shifts", shiftsMock)
                helperDataLayer.CreateDataTableStub(dDataTStub)
                helperEmployees.GetIDEmployeesFromUserFieldValueStub()
                helperPassport.PassportStub(1, helperDataLayer)
                helperCalendar.LoadCellsByCalendar(1, 2, True)
                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.GetCalendar(calendarCriteria, calendarList, returnCode, returnText)

                'Assert
                Assert.True(returnCode = ReturnCode._OK And calendarList(0).IDShiftBase.Equals("HOR"))

            End Using
        End Sub

        <Fact(DisplayName:="Should Not Return Shift Base Layers If Current Shift Is Not Holiday Shift")>
        Sub ShouldNotReturnShiftBaseLayersIfCurrentShiftIsNotHolidayShift()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim calendarCriteria As roCalendarCriteria = New roCalendarCriteria With {.StartDate = New Robotics.VTBase.roWCFDate(New Date(2025, 1, 1)), .EndDate = New Robotics.VTBase.roWCFDate(New Date(2025, 1, 1)), .IDEmployees = New String() {1}}
                Dim calendarList As List(Of roCalendar) = New List(Of roCalendar)()
                Dim returnCode As ReturnCode = New ReturnCode()
                Dim returnText As String = ""
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                Dim tMock As New DataTableMock With {.columns = {"Date", "EmployeeKey", "IdEmployee", "ExportCode", "ShiftName", "Timestamp"}, .values = New Object()() {}}
                dDataTStub.Add("DailySchedule", tMock)
                Dim shiftsMock As New DataTableMock With {.columns = {"ID", "Export"},
                    .values = New Object()() {New Object() {1, "VAC"}, New Object() {2, "HOR"}}}
                dDataTStub.Add("Shifts", shiftsMock)
                helperDataLayer.CreateDataTableStub(dDataTStub)
                helperEmployees.GetIDEmployeesFromUserFieldValueStub()
                helperPassport.PassportStub(1, helperDataLayer)
                helperCalendar.LoadCellsByCalendar(2, Nothing, False)
                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.GetCalendar(calendarCriteria, calendarList, returnCode, returnText)

                'Assert
                Assert.True(returnCode = ReturnCode._OK And calendarList(0).ShiftBaseLayerDefinition Is Nothing)

            End Using
        End Sub

        <Fact(DisplayName:="Should Return Shift Base Layers If Current Shift Is Holiday Shift")>
        Sub ShouldReturnShiftBaseLayersIfCurrentShiftIsHolidayShift()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim calendarCriteria As roCalendarCriteria = New roCalendarCriteria With {.StartDate = New Robotics.VTBase.roWCFDate(New Date(2025, 1, 1)), .EndDate = New Robotics.VTBase.roWCFDate(New Date(2025, 1, 1)), .IDEmployees = New String() {1}}
                Dim calendarList As List(Of roCalendar) = New List(Of roCalendar)()
                Dim returnCode As ReturnCode = New ReturnCode()
                Dim returnText As String = ""
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                Dim tMock As New DataTableMock With {.columns = {"Date", "EmployeeKey", "IdEmployee", "ExportCode", "ShiftName", "Timestamp"}, .values = New Object()() {}}
                dDataTStub.Add("DailySchedule", tMock)
                Dim shiftsMock As New DataTableMock With {.columns = {"ID", "Export"},
                    .values = New Object()() {New Object() {1, "VAC"}, New Object() {2, "HOR"}}}
                dDataTStub.Add("Shifts", shiftsMock)
                helperDataLayer.CreateDataTableStub(dDataTStub)
                helperEmployees.GetIDEmployeesFromUserFieldValueStub()
                helperPassport.PassportStub(1, helperDataLayer)
                helperCalendar.LoadCellsByCalendar(1, 2, True)
                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.GetCalendar(calendarCriteria, calendarList, returnCode, returnText, Nothing, True)

                'Assert
                Assert.True(returnCode = ReturnCode._OK And calendarList(0).ShiftBaseLayerDefinition IsNot Nothing AndAlso calendarList(0).ShiftBaseLayerDefinition.Count = 2)

            End Using
        End Sub

        <Fact(DisplayName:="Should Return Shift Base Layers If Load Layers Is True")>
        Sub ShouldReturnShiftBaseLayersIfLoadLayersIsTrue()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim calendarCriteria As roCalendarCriteria = New roCalendarCriteria With {.StartDate = New Robotics.VTBase.roWCFDate(New Date(2025, 1, 1)), .EndDate = New Robotics.VTBase.roWCFDate(New Date(2025, 1, 1)), .IDEmployees = New String() {1}}
                Dim calendarList As List(Of roCalendar) = New List(Of roCalendar)()
                Dim returnCode As ReturnCode = New ReturnCode()
                Dim returnText As String = ""
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                Dim tMock As New DataTableMock With {.columns = {"Date", "EmployeeKey", "IdEmployee", "ExportCode", "ShiftName", "Timestamp"}, .values = New Object()() {}}
                dDataTStub.Add("DailySchedule", tMock)
                Dim shiftsMock As New DataTableMock With {.columns = {"ID", "Export"},
                    .values = New Object()() {New Object() {1, "VAC"}, New Object() {2, "HOR"}}}
                dDataTStub.Add("Shifts", shiftsMock)
                helperDataLayer.CreateDataTableStub(dDataTStub)
                helperEmployees.GetIDEmployeesFromUserFieldValueStub()
                helperPassport.PassportStub(1, helperDataLayer)
                helperCalendar.LoadCellsByCalendar(1, 2, True)
                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.GetCalendar(calendarCriteria, calendarList, returnCode, returnText, Nothing, True)

                'Assert
                Assert.True(returnCode = ReturnCode._OK And calendarList(0).ShiftBaseLayerDefinition IsNot Nothing AndAlso calendarList(0).ShiftBaseLayerDefinition.Count = 2)

            End Using
        End Sub

        <Fact(DisplayName:="Should Not Return Shift Base Layers If Load Layers Is False")>
        Sub ShouldNotReturnShiftBaseLayersIfLoadLayersIsFalse()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim calendarCriteria As roCalendarCriteria = New roCalendarCriteria With {.StartDate = New Robotics.VTBase.roWCFDate(New Date(2025, 1, 1)), .EndDate = New Robotics.VTBase.roWCFDate(New Date(2025, 1, 1)), .IDEmployees = New String() {1}}
                Dim calendarList As List(Of roCalendar) = New List(Of roCalendar)()
                Dim returnCode As ReturnCode = New ReturnCode()
                Dim returnText As String = ""
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                Dim tMock As New DataTableMock With {.columns = {"Date", "EmployeeKey", "IdEmployee", "ExportCode", "ShiftName", "Timestamp"}, .values = New Object()() {}}
                dDataTStub.Add("DailySchedule", tMock)
                Dim shiftsMock As New DataTableMock With {.columns = {"ID", "Export"},
                    .values = New Object()() {New Object() {1, "VAC"}, New Object() {2, "HOR"}}}
                dDataTStub.Add("Shifts", shiftsMock)
                helperDataLayer.CreateDataTableStub(dDataTStub)
                helperEmployees.GetIDEmployeesFromUserFieldValueStub()
                helperPassport.PassportStub(1, helperDataLayer)
                helperCalendar.LoadCellsByCalendar(1, 2, True)
                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.GetCalendar(calendarCriteria, calendarList, returnCode, returnText, Nothing, False)

                'Assert
                Assert.True(returnCode = ReturnCode._OK And calendarList(0).ShiftBaseLayerDefinition Is Nothing)

            End Using
        End Sub

        <Fact(DisplayName:="Should Not Return Shift Base Layers If Load Layers Is Empty")>
        Sub ShouldNotReturnShiftBaseLayersIfLoadLayersIsEmpty()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim calendarCriteria As roCalendarCriteria = New roCalendarCriteria With {.StartDate = New Robotics.VTBase.roWCFDate(New Date(2025, 1, 1)), .EndDate = New Robotics.VTBase.roWCFDate(New Date(2025, 1, 1)), .IDEmployees = New String() {1}}
                Dim calendarList As List(Of roCalendar) = New List(Of roCalendar)()
                Dim returnCode As ReturnCode = New ReturnCode()
                Dim returnText As String = ""
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                Dim tMock As New DataTableMock With {.columns = {"Date", "EmployeeKey", "IdEmployee", "ExportCode", "ShiftName", "Timestamp"}, .values = New Object()() {}}
                dDataTStub.Add("DailySchedule", tMock)
                Dim shiftsMock As New DataTableMock With {.columns = {"ID", "Export"},
                    .values = New Object()() {New Object() {1, "VAC"}, New Object() {2, "HOR"}}}
                dDataTStub.Add("Shifts", shiftsMock)
                helperDataLayer.CreateDataTableStub(dDataTStub)
                helperEmployees.GetIDEmployeesFromUserFieldValueStub()
                helperPassport.PassportStub(1, helperDataLayer)
                helperCalendar.LoadCellsByCalendar(1, 2, True)
                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.GetCalendar(calendarCriteria, calendarList, returnCode, returnText)

                'Assert
                Assert.True(returnCode = ReturnCode._OK And calendarList(0).ShiftBaseLayerDefinition Is Nothing)

            End Using
        End Sub

        <Fact(DisplayName:="Should Return Day Before When Shift Base Starts Day Before")>
        Sub ShouldReturnDayBeforeWhenShiftBaseStartsDayBefore()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim calendarCriteria As roCalendarCriteria = New roCalendarCriteria With {.StartDate = New Robotics.VTBase.roWCFDate(New Date(2025, 1, 1)), .EndDate = New Robotics.VTBase.roWCFDate(New Date(2025, 1, 1)), .IDEmployees = New String() {1}}
                Dim calendarList As List(Of roCalendar) = New List(Of roCalendar)()
                Dim returnCode As ReturnCode = New ReturnCode()
                Dim returnText As String = ""
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                Dim tMock As New DataTableMock With {.columns = {"Date", "EmployeeKey", "IdEmployee", "ExportCode", "ShiftName", "Timestamp"}, .values = New Object()() {}}
                dDataTStub.Add("DailySchedule", tMock)
                Dim shiftsMock As New DataTableMock With {.columns = {"ID", "Export"},
                    .values = New Object()() {New Object() {1, "VAC"}, New Object() {2, "HOR"}}}
                dDataTStub.Add("Shifts", shiftsMock)
                helperDataLayer.CreateDataTableStub(dDataTStub)
                helperEmployees.GetIDEmployeesFromUserFieldValueStub()
                helperPassport.PassportStub(1, helperDataLayer)
                helperCalendar.LoadCellsByCalendar(1, 2, True, New Date(1899, 12, 29))
                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.GetCalendar(calendarCriteria, calendarList, returnCode, returnText,, True)

                'Assert
                Assert.True(returnCode = ReturnCode._OK And calendarList(0).ShiftBaseLayerDefinition(0).StartDay = roDayInfo.DayBefore)

            End Using
        End Sub

        <Fact(DisplayName:="Should Return Current Day When Shift Base Starts On Current Day")>
        Sub ShouldReturnCurrentDayWhenShiftBaseStartsOnCurrentDay()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim calendarCriteria As roCalendarCriteria = New roCalendarCriteria With {.StartDate = New Robotics.VTBase.roWCFDate(New Date(2025, 1, 1)), .EndDate = New Robotics.VTBase.roWCFDate(New Date(2025, 1, 1)), .IDEmployees = New String() {1}}
                Dim calendarList As List(Of roCalendar) = New List(Of roCalendar)()
                Dim returnCode As ReturnCode = New ReturnCode()
                Dim returnText As String = ""
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                Dim tMock As New DataTableMock With {.columns = {"Date", "EmployeeKey", "IdEmployee", "ExportCode", "ShiftName", "Timestamp"}, .values = New Object()() {}}
                dDataTStub.Add("DailySchedule", tMock)
                Dim shiftsMock As New DataTableMock With {.columns = {"ID", "Export"},
                    .values = New Object()() {New Object() {1, "VAC"}, New Object() {2, "HOR"}}}
                dDataTStub.Add("Shifts", shiftsMock)
                helperDataLayer.CreateDataTableStub(dDataTStub)
                helperEmployees.GetIDEmployeesFromUserFieldValueStub()
                helperPassport.PassportStub(1, helperDataLayer)
                helperCalendar.LoadCellsByCalendar(1, 2, True, New Date(1899, 12, 30))
                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.GetCalendar(calendarCriteria, calendarList, returnCode, returnText,, True)

                'Assert
                Assert.True(returnCode = ReturnCode._OK And calendarList(0).ShiftBaseLayerDefinition(0).StartDay = roDayInfo.CurrentDay)

            End Using
        End Sub

        <Fact(DisplayName:="Should Return Day After When Shift Base Starts On Day After")>
        Sub ShouldReturnDayAfterWhenShiftBaseStartsOnDayAfter()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim calendarCriteria As roCalendarCriteria = New roCalendarCriteria With {.StartDate = New Robotics.VTBase.roWCFDate(New Date(2025, 1, 1)), .EndDate = New Robotics.VTBase.roWCFDate(New Date(2025, 1, 1)), .IDEmployees = New String() {1}}
                Dim calendarList As List(Of roCalendar) = New List(Of roCalendar)()
                Dim returnCode As ReturnCode = New ReturnCode()
                Dim returnText As String = ""
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                Dim tMock As New DataTableMock With {.columns = {"Date", "EmployeeKey", "IdEmployee", "ExportCode", "ShiftName", "Timestamp"}, .values = New Object()() {}}
                dDataTStub.Add("DailySchedule", tMock)
                Dim shiftsMock As New DataTableMock With {.columns = {"ID", "Export"},
                    .values = New Object()() {New Object() {1, "VAC"}, New Object() {2, "HOR"}}}
                dDataTStub.Add("Shifts", shiftsMock)
                helperDataLayer.CreateDataTableStub(dDataTStub)
                helperEmployees.GetIDEmployeesFromUserFieldValueStub()
                helperPassport.PassportStub(1, helperDataLayer)
                helperCalendar.LoadCellsByCalendar(1, 2, True, New Date(1899, 12, 31))
                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.GetCalendar(calendarCriteria, calendarList, returnCode, returnText,, True)

                'Assert
                Assert.True(returnCode = ReturnCode._OK And calendarList(0).ShiftBaseLayerDefinition(0).StartDay = roDayInfo.DayAfter)

            End Using
        End Sub

        <Fact(DisplayName:="Should Return Correct Start Time When Shift Base Starts Day Before")>
        Sub ShouldReturnCorrectStartTimeWhenShiftBaseStartsDayBefore()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim calendarCriteria As roCalendarCriteria = New roCalendarCriteria With {.StartDate = New Robotics.VTBase.roWCFDate(New Date(2025, 1, 1)), .EndDate = New Robotics.VTBase.roWCFDate(New Date(2025, 1, 1)), .IDEmployees = New String() {1}}
                Dim calendarList As List(Of roCalendar) = New List(Of roCalendar)()
                Dim returnCode As ReturnCode = New ReturnCode()
                Dim returnText As String = ""
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                Dim tMock As New DataTableMock With {.columns = {"Date", "EmployeeKey", "IdEmployee", "ExportCode", "ShiftName", "Timestamp"}, .values = New Object()() {}}
                dDataTStub.Add("DailySchedule", tMock)
                Dim shiftsMock As New DataTableMock With {.columns = {"ID", "Export"},
                    .values = New Object()() {New Object() {1, "VAC"}, New Object() {2, "HOR"}}}
                dDataTStub.Add("Shifts", shiftsMock)
                helperDataLayer.CreateDataTableStub(dDataTStub)
                helperEmployees.GetIDEmployeesFromUserFieldValueStub()
                helperPassport.PassportStub(1, helperDataLayer)
                helperCalendar.LoadCellsByCalendar(1, 2, True, New DateTime(1899, 12, 29, 22, 0, 0))
                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.GetCalendar(calendarCriteria, calendarList, returnCode, returnText,, True)

                'Assert
                Assert.True(returnCode = ReturnCode._OK And calendarList(0).ShiftBaseLayerDefinition(0).StartTime.Equals("22:00"))

            End Using
        End Sub

        <Fact(DisplayName:="Should Return Correct Start Time When Shift Base Starts Current Day")>
        Sub ShouldReturnCorrectStartTimeWhenShiftBaseStartsCurrentDay()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim calendarCriteria As roCalendarCriteria = New roCalendarCriteria With {.StartDate = New Robotics.VTBase.roWCFDate(New Date(2025, 1, 1)), .EndDate = New Robotics.VTBase.roWCFDate(New Date(2025, 1, 1)), .IDEmployees = New String() {1}}
                Dim calendarList As List(Of roCalendar) = New List(Of roCalendar)()
                Dim returnCode As ReturnCode = New ReturnCode()
                Dim returnText As String = ""
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                Dim tMock As New DataTableMock With {.columns = {"Date", "EmployeeKey", "IdEmployee", "ExportCode", "ShiftName", "Timestamp"}, .values = New Object()() {}}
                dDataTStub.Add("DailySchedule", tMock)
                Dim shiftsMock As New DataTableMock With {.columns = {"ID", "Export"},
                    .values = New Object()() {New Object() {1, "VAC"}, New Object() {2, "HOR"}}}
                dDataTStub.Add("Shifts", shiftsMock)
                helperDataLayer.CreateDataTableStub(dDataTStub)
                helperEmployees.GetIDEmployeesFromUserFieldValueStub()
                helperPassport.PassportStub(1, helperDataLayer)
                helperCalendar.LoadCellsByCalendar(1, 2, True, New DateTime(1899, 12, 30, 22, 0, 0))
                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.GetCalendar(calendarCriteria, calendarList, returnCode, returnText,, True)

                'Assert
                Assert.True(returnCode = ReturnCode._OK And calendarList(0).ShiftBaseLayerDefinition(0).StartTime.Equals("22:00"))

            End Using
        End Sub

        <Fact(DisplayName:="Should Return Correct Start Time When Shift Base Starts After Day")>
        Sub ShouldReturnCorrectStartTimeWhenShiftBaseStartsAfterDay()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim calendarCriteria As roCalendarCriteria = New roCalendarCriteria With {.StartDate = New Robotics.VTBase.roWCFDate(New Date(2025, 1, 1)), .EndDate = New Robotics.VTBase.roWCFDate(New Date(2025, 1, 1)), .IDEmployees = New String() {1}}
                Dim calendarList As List(Of roCalendar) = New List(Of roCalendar)()
                Dim returnCode As ReturnCode = New ReturnCode()
                Dim returnText As String = ""
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                Dim tMock As New DataTableMock With {.columns = {"Date", "EmployeeKey", "IdEmployee", "ExportCode", "ShiftName", "Timestamp"}, .values = New Object()() {}}
                dDataTStub.Add("DailySchedule", tMock)
                Dim shiftsMock As New DataTableMock With {.columns = {"ID", "Export"},
                    .values = New Object()() {New Object() {1, "VAC"}, New Object() {2, "HOR"}}}
                dDataTStub.Add("Shifts", shiftsMock)
                helperDataLayer.CreateDataTableStub(dDataTStub)
                helperEmployees.GetIDEmployeesFromUserFieldValueStub()
                helperPassport.PassportStub(1, helperDataLayer)
                helperCalendar.LoadCellsByCalendar(1, 2, True, New DateTime(1899, 12, 31, 22, 0, 0))
                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.GetCalendar(calendarCriteria, calendarList, returnCode, returnText,, True)

                'Assert
                Assert.True(returnCode = ReturnCode._OK And calendarList(0).ShiftBaseLayerDefinition(0).StartTime.Equals("22:00"))

            End Using
        End Sub

        <Fact(DisplayName:="Should Return Start Base Planned Hour If Main Shift Is Holiday Shift")>
        Sub ShouldReturnStartBasePlannedHourIfMainShiftIsHolidayShift()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim calendarCriteria As roCalendarCriteria = New roCalendarCriteria With {.StartDate = New Robotics.VTBase.roWCFDate(New Date(2025, 1, 1)), .EndDate = New Robotics.VTBase.roWCFDate(New Date(2025, 1, 1)), .IDEmployees = New String() {1}}
                Dim calendarList As List(Of roCalendar) = New List(Of roCalendar)()
                Dim returnCode As ReturnCode = New ReturnCode()
                Dim returnText As String = ""
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                Dim tMock As New DataTableMock With {.columns = {"Date", "EmployeeKey", "IdEmployee", "ExportCode", "ShiftName", "Timestamp"}, .values = New Object()() {}}
                dDataTStub.Add("DailySchedule", tMock)
                Dim shiftsMock As New DataTableMock With {.columns = {"ID", "Export"},
                    .values = New Object()() {New Object() {1, "VAC"}, New Object() {2, "HOR"}}}
                dDataTStub.Add("Shifts", shiftsMock)
                helperDataLayer.CreateDataTableStub(dDataTStub)
                helperEmployees.GetIDEmployeesFromUserFieldValueStub()
                helperPassport.PassportStub(1, helperDataLayer)
                helperCalendar.LoadCellsByCalendar(1, 2, True, New DateTime(1899, 12, 31, 22, 0, 0))
                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.GetCalendar(calendarCriteria, calendarList, returnCode, returnText,, True)

                'Assert
                Assert.True(returnCode = ReturnCode._OK And calendarList(0).StartBasePlanned.GetDate() = New DateTime(2025, 1, 1, 10, 0, 0))

            End Using
        End Sub

        <Fact(DisplayName:="Should Return End Base Planned Hour If Main Shift Is Holiday Shift")>
        Sub ShouldReturnEndBasePlannedHourIfMainShiftIsHolidayShift()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim calendarCriteria As roCalendarCriteria = New roCalendarCriteria With {.StartDate = New Robotics.VTBase.roWCFDate(New Date(2025, 1, 1)), .EndDate = New Robotics.VTBase.roWCFDate(New Date(2025, 1, 1)), .IDEmployees = New String() {1}}
                Dim calendarList As List(Of roCalendar) = New List(Of roCalendar)()
                Dim returnCode As ReturnCode = New ReturnCode()
                Dim returnText As String = ""
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                Dim tMock As New DataTableMock With {.columns = {"Date", "EmployeeKey", "IdEmployee", "ExportCode", "ShiftName", "Timestamp"}, .values = New Object()() {}}
                dDataTStub.Add("DailySchedule", tMock)
                Dim shiftsMock As New DataTableMock With {.columns = {"ID", "Export"},
                    .values = New Object()() {New Object() {1, "VAC"}, New Object() {2, "HOR"}}}
                dDataTStub.Add("Shifts", shiftsMock)
                helperDataLayer.CreateDataTableStub(dDataTStub)
                helperEmployees.GetIDEmployeesFromUserFieldValueStub()
                helperPassport.PassportStub(1, helperDataLayer)
                helperCalendar.LoadCellsByCalendar(1, 2, True, New DateTime(1899, 12, 31, 22, 0, 0))
                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.GetCalendar(calendarCriteria, calendarList, returnCode, returnText,, True)

                'Assert
                Assert.True(returnCode = ReturnCode._OK And calendarList(0).EndBasePlanned.GetDate() = New DateTime(2025, 1, 1, 18, 0, 0))

            End Using
        End Sub

        <Fact(DisplayName:="Should Not Return Start Base Planned Hour If Main Shift Is Not Holiday Shift")>
        Sub ShouldNotReturnStartBasePlannedHourIfMainShiftIsNotHolidayShift()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim calendarCriteria As roCalendarCriteria = New roCalendarCriteria With {.StartDate = New Robotics.VTBase.roWCFDate(New Date(2025, 1, 1)), .EndDate = New Robotics.VTBase.roWCFDate(New Date(2025, 1, 1)), .IDEmployees = New String() {1}}
                Dim calendarList As List(Of roCalendar) = New List(Of roCalendar)()
                Dim returnCode As ReturnCode = New ReturnCode()
                Dim returnText As String = ""
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                Dim tMock As New DataTableMock With {.columns = {"Date", "EmployeeKey", "IdEmployee", "ExportCode", "ShiftName", "Timestamp"}, .values = New Object()() {}}
                dDataTStub.Add("DailySchedule", tMock)
                Dim shiftsMock As New DataTableMock With {.columns = {"ID", "Export"},
                    .values = New Object()() {New Object() {1, "VAC"}, New Object() {2, "HOR"}}}
                dDataTStub.Add("Shifts", shiftsMock)
                helperDataLayer.CreateDataTableStub(dDataTStub)
                helperEmployees.GetIDEmployeesFromUserFieldValueStub()
                helperPassport.PassportStub(1, helperDataLayer)
                helperCalendar.LoadCellsByCalendar(1, 2, False, New DateTime(1899, 12, 31, 22, 0, 0))
                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.GetCalendar(calendarCriteria, calendarList, returnCode, returnText,, True)

                'Assert
                Assert.True(returnCode = ReturnCode._OK And calendarList(0).StartBasePlanned Is Nothing)

            End Using
        End Sub

        <Fact(DisplayName:="Should Not Return End Base Planned Hour If Main Shift Is Not Holiday Shift")>
        Sub ShouldNotReturnEndBasePlannedHourIfMainShiftIsNotHolidayShift()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim calendarCriteria As roCalendarCriteria = New roCalendarCriteria With {.StartDate = New Robotics.VTBase.roWCFDate(New Date(2025, 1, 1)), .EndDate = New Robotics.VTBase.roWCFDate(New Date(2025, 1, 1)), .IDEmployees = New String() {1}}
                Dim calendarList As List(Of roCalendar) = New List(Of roCalendar)()
                Dim returnCode As ReturnCode = New ReturnCode()
                Dim returnText As String = ""
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                Dim tMock As New DataTableMock With {.columns = {"Date", "EmployeeKey", "IdEmployee", "ExportCode", "ShiftName", "Timestamp"}, .values = New Object()() {}}
                dDataTStub.Add("DailySchedule", tMock)
                Dim shiftsMock As New DataTableMock With {.columns = {"ID", "Export"},
                    .values = New Object()() {New Object() {1, "VAC"}, New Object() {2, "HOR"}}}
                dDataTStub.Add("Shifts", shiftsMock)
                helperDataLayer.CreateDataTableStub(dDataTStub)
                helperEmployees.GetIDEmployeesFromUserFieldValueStub()
                helperPassport.PassportStub(1, helperDataLayer)
                helperCalendar.LoadCellsByCalendar(1, 2, False, New DateTime(1899, 12, 31, 22, 0, 0))
                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.GetCalendar(calendarCriteria, calendarList, returnCode, returnText,, True)

                'Assert
                Assert.True(returnCode = ReturnCode._OK And calendarList(0).EndBasePlanned Is Nothing)

            End Using
        End Sub

    End Class

End Namespace