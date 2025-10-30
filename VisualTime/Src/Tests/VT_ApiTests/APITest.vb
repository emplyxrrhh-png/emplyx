Imports System.ComponentModel
Imports Robotics.ExternalSystems.DataLink.RoboticsExternAccess
Imports VTLiveApi
Imports VT_XU_Base
Imports VT_XU_Common
Imports VT_XU_Security
Imports VT_XU_Datalink
Imports Xunit
Imports Robotics.ExternalSystems.DataLink.RoboticsExternAccess.roDatalinkDailyCause
Imports Robotics.ExternalSystems.DataLink

Namespace Unit.Test

    <Collection("Employee")>
    <CollectionDefinition("Employee", DisableParallelization:=True)>
    <Category("Employee")>
    Public Class APITest

        Private ReadOnly helperPassport As PassportHelper
        Private ReadOnly helperDatalayer As DatalayerHelper
        Private ReadOnly helperWeb As WebHelper
        Private ReadOnly helperAPI As APIHelper
        Private ReadOnly helperPunches As PunchHelper
        Private ReadOnly helperBase As BaseHelper
        Private ReadOnly helperDatalink As DatalinkHelper
        Private ReadOnly helperBusiness As BusinessHelper
        Private ReadOnly helperEmployee As EmployeeHelper
        Private ReadOnly helperParameters As ParametersHelper

        ''' <summary>
        ''' Test Setup
        ''' </summary>
        Sub New()
            helperPassport = New PassportHelper
            helperDatalayer = New DatalayerHelper
            helperWeb = New WebHelper
            helperAPI = New APIHelper
            helperPunches = New PunchHelper
            helperBase = New BaseHelper
            helperDatalink = New DatalinkHelper
            helperBusiness = New BusinessHelper
            helperEmployee = New EmployeeHelper
            helperParameters = New ParametersHelper
        End Sub

        <Fact(DisplayName:="Should return validation error when try to create not manual daily cause")>
        Function ShouldReturnValidationErrorWhenTryToCreateNotManualDailyCause()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperWeb.HttpContextStub()
                helperPassport.PassportStub(1, helperDatalayer)
                helperDatalink.ValidateTokenSpy()
                helperAPI.XCauseConverter()

                'Act
                Dim dailyCauseService As New VTLiveApi.DailyCausesService_v2
                Dim response As roDatalinkStandarDailyCauseResponse
                Dim oDailyCause As New roDailyCause
                oDailyCause.Manual = False

                response = dailyCauseService.CreateOrUpdateDailyCause("token", oDailyCause)

                'Assert
                Assert.True(response.ResultCode = Core.DTOs.ReturnCode._InvalidManualValue)
            End Using
        End Function


        <Fact(DisplayName:="Should Deny Access To Calendar Data If No Token Provided")>
        Sub ShouldDenyAccessToCalendarDataIfNoTokenProvided()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperWeb.HttpContextStub()
                helperPassport.PassportStub(1, helperDatalayer)
                helperDatalink.GetCalendarSpy()
                'Act
                Dim calendarService As New ScheduleService_v2
                Dim response As roWSResponse(Of roCalendar())
                response = calendarService.GetCalendar(String.Empty, New Robotics.ExternalSystems.DataLink.RoboticsExternAccess.roCalendarCriteria, String.Empty)

                'Assert
                Assert.True(response.Status = Core.DTOs.ReturnCode._PasswordUsernameEmpty AndAlso Not helperDatalink.GetCalendarCalled)
            End Using
        End Sub

        'TODO ExternalSystems.RoboticExternanAccess.GetCalendar

        <Fact(DisplayName:="Should Deny Access To Calendar Data If Provided Token Does Not Match Primary Token Neither Secondary")>
        Sub ShouldDenyAccessToCalendarDataIfProvidedTokenDoesNotMatchPrimaryTokenNeitherSecondary()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperWeb.HttpContextStub()
                helperPassport.PassportStub(1, helperDatalayer)
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{(Robotics.Base.DTOs.AdvancedParameterType.ExternAccessToken1).ToString, "firsttoken"}, {(Robotics.Base.DTOs.AdvancedParameterType.ExternAccessToken2).ToString, "secondtoken"}})
                helperDatalink.GetCalendarSpy()
                'Act
                Dim calendarService As New ScheduleService_v2
                Dim response As roWSResponse(Of roCalendar())
                response = calendarService.GetCalendar("wrongtoken", New Robotics.ExternalSystems.DataLink.RoboticsExternAccess.roCalendarCriteria, String.Empty)

                'Assert
                Assert.True(response.Status = Core.DTOs.ReturnCode._LoginError AndAlso Not helperDatalink.GetCalendarCalled)
            End Using
        End Sub

        <Fact(DisplayName:="Should Grant Access To Calendar Data If Provided Token Match Primary Token")>
        Sub ShouldGrantAccessToCalendarDataIfProvidedTokenMatchPrimaryToken()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperWeb.HttpContextStub()
                helperPassport.PassportStub(1, helperDatalayer)
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{(Robotics.Base.DTOs.AdvancedParameterType.ExternAccessToken1).ToString, "firsttoken"}, {(Robotics.Base.DTOs.AdvancedParameterType.ExternAccessToken2).ToString, "secondtoken"}})
                helperDatalink.GetCalendarSpy()
                'Act
                Dim calendarService As New ScheduleService_v2
                Dim response As roWSResponse(Of roCalendar())
                response = calendarService.GetCalendar("firsttoken", New Robotics.ExternalSystems.DataLink.RoboticsExternAccess.roCalendarCriteria, String.Empty)

                'Assert
                Assert.True(response.Status = Core.DTOs.ReturnCode._OK AndAlso helperDatalink.GetCalendarCalled)
            End Using
        End Sub

        <Fact(DisplayName:="Should Grant Access To Calendar Data If Provided Token Match Secondary Token")>
        Sub ShouldGrantAccessToCalendarDataIfProvidedTokenMatchSecondaryToken()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperWeb.HttpContextStub()
                helperPassport.PassportStub(1, helperDatalayer)
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{(Robotics.Base.DTOs.AdvancedParameterType.ExternAccessToken1).ToString, "firsttoken"}, {(Robotics.Base.DTOs.AdvancedParameterType.ExternAccessToken2).ToString, "secondtoken"}})
                helperDatalink.GetCalendarSpy()
                'Act
                Dim calendarService As New ScheduleService_v2
                Dim response As roWSResponse(Of roCalendar())
                response = calendarService.GetCalendar("secondtoken", New Robotics.ExternalSystems.DataLink.RoboticsExternAccess.roCalendarCriteria, String.Empty)

                'Assert
                Assert.True(response.Status = Core.DTOs.ReturnCode._OK AndAlso helperDatalink.GetCalendarCalled)
            End Using
        End Sub

        <Fact(DisplayName:="Should Grant Access To Calendar Data If UserHostAddress In White List And Ip Filter Set")>
        Sub ShouldGrantAccessToCalendarDataIfUserHostAddressInWhiteListAndIpFilterSet()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperWeb.HttpContextStub(,,, "8.8.8.8")
                helperPassport.PassportStub(1, helperDatalayer)
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{(Robotics.Base.DTOs.AdvancedParameterType.ExternAccessIPs).ToString, "8.8.8.8"}, {(Robotics.Base.DTOs.AdvancedParameterType.ExternAccessToken1).ToString, "token1"}, {(Robotics.Base.DTOs.AdvancedParameterType.ExternAccessToken2).ToString, "token2"}})
                helperDatalink.GetCalendarSpy()

                'Act
                Dim calendarService As New ScheduleService_v2
                Dim response As roWSResponse(Of roCalendar())
                response = calendarService.GetCalendar("token1", New Robotics.ExternalSystems.DataLink.RoboticsExternAccess.roCalendarCriteria, String.Empty)

                'Assert
                Assert.True(response.Status = Core.DTOs.ReturnCode._OK AndAlso helperDatalink.GetCalendarCalled)
            End Using
        End Sub

        <Fact(DisplayName:="Should Deny Access To Calendar Data If UserHostAddress Not In White List And Ip Filter Set")>
        Sub ShouldDenyAccessToCalendarDataIfUserHostAddressNotInWhiteListAndIpFilterSet()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                'helperWeb.HttpContextStub(New Dictionary(Of String, String) From {{"sessionparameter", "test"}}, New Dictionary(Of String, String) From {{"requestheader1", "testrq"}}, New Dictionary(Of String, String) From {{"requestparams1", "testrp"}}, "8.8.8.8")
                helperWeb.HttpContextStub(,,, "8.8.8.8")
                helperPassport.PassportStub(1, helperDatalayer)
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{(Robotics.Base.DTOs.AdvancedParameterType.ExternAccessIPs).ToString, "1.1.1.1"}, {(Robotics.Base.DTOs.AdvancedParameterType.ExternAccessToken1).ToString, "token1"}, {(Robotics.Base.DTOs.AdvancedParameterType.ExternAccessToken2).ToString, "token2"}})
                helperDatalink.GetCalendarSpy()

                'Act
                Dim calendarService As New ScheduleService_v2
                Dim response As roWSResponse(Of roCalendar())
                response = calendarService.GetCalendar("token1", New Robotics.ExternalSystems.DataLink.RoboticsExternAccess.roCalendarCriteria, String.Empty)

                'Assert
                Assert.True(response.Status = Core.DTOs.ReturnCode._PermissionDenied AndAlso Not helperDatalink.GetCalendarCalled)
            End Using
        End Sub

        <Fact(DisplayName:="Should Validate Provided Token Prior To Get Absences Data")>
        Sub ShouldValidateProvidedTokenPriorToGetAbsencesData()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperWeb.HttpContextStub()
                helperPassport.PassportStub(1, helperDatalayer)
                helperDatalink.ValidateTokenSpy()
                helperDatalink.GetAbsencesStub()

                'Act
                Dim absenceService As New AbsencesService_v2
                Dim response As roWSResponse(Of roAbsence())
                response = absenceService.GetAbsences("token", Now.Date.AddDays(-1), Now.Date)

                'Assert
                Assert.True(response.Status = Core.DTOs.ReturnCode._OK AndAlso helperDatalink.ValidateTokenCalled)
            End Using
        End Sub

        ' TODO RoboticsExternAccess.GetAbsence

        <Fact(DisplayName:="Should Validate Provided Token Prior To Create Or Update Absences Data")>
        Sub ShouldValidateProvidedTokenPriorToCreateOrUpdateAbsencesData()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperWeb.HttpContextStub()
                helperPassport.PassportStub(1, helperDatalayer)
                helperDatalink.ValidateTokenSpy()
                helperAPI.XStandardAbsenceToAbsenceConverterStub()
                helperAPI.XAbsenceToStandardAbsenceConverterStub()
                helperDatalink.CreateOrUpdateAbsenceStub()

                'Act
                Dim absenceService As New AbsencesService_v2
                Dim response As roWSResponse(Of roAbsence())
                Dim oAbsence As New roAbsence

                response = absenceService.CreateOrUpdateAbsence("token", oAbsence)

                'Assert
                Assert.True(response.Status = Core.DTOs.ReturnCode._OK AndAlso helperDatalink.ValidateTokenCalled)
            End Using
        End Sub

        ' TODO RoboticsExternAccess.CreateOrUpdateAbsence

        <Fact(DisplayName:="Should Validate Provided Token Prior To Get Accruals Data")>
        Sub ShouldValidateProvidedTokenPriorToGetAccrualsData()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperWeb.HttpContextStub()
                helperPassport.PassportStub(1, helperDatalayer)
                helperDatalink.ValidateTokenSpy()
                helperDatalink.GetAccrualsStub()

                'Act
                Dim accrualsService As New AccrualsService_v2
                Dim response As roWSResponse(Of roAccrual())
                response = accrualsService.GetAccruals("token", Now.Date, Now.Date)

                'Assert
                Assert.True(response.Status = Core.DTOs.ReturnCode._OK AndAlso helperDatalink.ValidateTokenCalled)
            End Using
        End Sub

        ' TODO RoboticsExternAccess.GetAccruals

        <Fact(DisplayName:="Should Validate Provided Token Prior To Get Accruals Data At Date")>
        Sub ShouldValidateProvidedTokenPriorToGetAccrualsDataAtDate()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperWeb.HttpContextStub()
                helperPassport.PassportStub(1, helperDatalayer)
                helperDatalink.ValidateTokenSpy()
                helperDatalink.GetAccrualsStub()

                'Act
                Dim accrualsService As New AccrualsService_v2
                Dim response As roWSResponse(Of roAccrual())
                response = accrualsService.GetAccrualsAtDate("token", Now.Date)

                'Assert
                Assert.True(response.Status = Core.DTOs.ReturnCode._OK AndAlso helperDatalink.ValidateTokenCalled)
            End Using
        End Sub

        ' TODO RoboticsExternAccess.GetAccrualsAtDate

        <Fact(DisplayName:="Should Validate Provided Token Prior To Add Document")>
        Sub ShouldValidateProvidedTokenPriorToAddDocument()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperWeb.HttpContextStub()
                helperPassport.PassportStub(1, helperDatalayer)
                helperDatalink.ValidateTokenSpy()
                helperAPI.XDocumentToStandardDocumentConverterStub()
                helperDatalink.CreateOrUpdateDocumentStub()

                'Act
                Dim documentsService As New DocumentsService_v2
                Dim response As roWSResponse(Of roDocument)
                response = documentsService.AddDocument("token", New roDocument)

                'Assert
                Assert.True(response.Status = Core.DTOs.ReturnCode._OK AndAlso helperDatalink.ValidateTokenCalled)
            End Using
        End Sub

        ' TODO RoboticsExternAccess.CreateOrUpdateDocument

        <Fact(DisplayName:="Should Validate Provided Token Prior To Get Employees Data")>
        Sub ShouldValidateProvidedTokenPriorToGetEmployeesData()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperWeb.HttpContextStub()
                helperPassport.PassportStub(1, helperDatalayer)
                helperDatalink.ValidateTokenSpy()
                helperDatalink.GetEmployeesStub()

                'Act
                Dim employeesService As New EmployeeService_v2
                Dim response As roWSResponse(Of roEmployee())
                response = employeesService.GetEmployees("token", True, True)

                'Assert
                Assert.True(response.Status = Core.DTOs.ReturnCode._OK AndAlso helperDatalink.ValidateTokenCalled)
            End Using
        End Sub

        ' TODO RoboticsExternAccess.GetEmployees

        <Fact(DisplayName:="Should Validate Provided Token Prior To Get Employees Data By TimeStamp")>
        Sub ShouldValidateProvidedTokenPriorToGetEmployeesDataByTimeStamp()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperWeb.HttpContextStub()
                helperPassport.PassportStub(1, helperDatalayer)
                helperDatalink.ValidateTokenSpy()
                helperDatalink.GetEmployeesStub()

                'Act
                Dim employeesService As New EmployeeService_v2
                Dim response As roWSResponse(Of roEmployee())
                response = employeesService.GetEmployees("token", True, True)

                'Assert
                Assert.True(response.Status = Core.DTOs.ReturnCode._OK AndAlso helperDatalink.ValidateTokenCalled)
            End Using
        End Sub

        <Fact(DisplayName:="Should Validate Provided Token Prior To Create Or Update Employee Data")>
        Sub ShouldValidateProvidedTokenPriorToCreateOrUpdateEmployeeData()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperWeb.HttpContextStub()
                helperPassport.PassportStub(1, helperDatalayer)
                helperDatalink.ValidateTokenSpy()
                helperDatalink.CreateOrUpdateEmployeeStub()

                'Act
                Dim employeesService As New EmployeeService_v2
                Dim response As roDatalinkStandarEmployeeResponse
                Dim oEmployee As New roDatalinkStandarEmployee

                response = employeesService.CreateOrUpdateEmployee("token", oEmployee)

                'Assert
                Assert.True(response.ResultCode = Core.DTOs.ReturnCode._OK AndAlso helperDatalink.ValidateTokenCalled)
            End Using
        End Sub

        ' TODO RoboticsExternAccess.CreateOrUpdateEmployee
        ' TODO RoboticsExternAccess.GetEmployeeById

        <Fact(DisplayName:="Should Validate Provided Token Prior To Get Groups Data")>
        Sub ShouldValidateProvidedTokenPriorToGetGroupsData()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperWeb.HttpContextStub()
                helperPassport.PassportStub(1, helperDatalayer)
                helperDatalink.ValidateTokenSpy()
                helperDatalink.GetGroupsStub()

                'Act
                Dim groupsService As New GroupService_v2
                Dim response As roWSResponse(Of roGroup())
                response = groupsService.GetGroups("token", True, True)

                'Assert
                Assert.True(response.Status = Core.DTOs.ReturnCode._OK AndAlso helperDatalink.ValidateTokenCalled)
            End Using
        End Sub

        ' TODO RoboticsExternAccess.GetGroups

        <Fact(DisplayName:="Should Validate Provided Token Prior To Get Punches Data")>
        Sub ShouldValidateProvidedTokenPriorToGetPunchesData()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperWeb.HttpContextStub()
                helperPassport.PassportStub(1, helperDatalayer)
                helperDatalink.ValidateTokenSpy()
                helperDatalink.GetPunchesExStub()

                'Act
                Dim punchesService As New PunchesService_v2
                Dim response As roWSResponse(Of roPunch())
                response = punchesService.GetPunches("token", Now)

                'Assert
                Assert.True(response.Status = Core.DTOs.ReturnCode._OK AndAlso helperDatalink.ValidateTokenCalled)
            End Using
        End Sub

        ' TODO RoboticsExternAccess.GetPunchesEx
        ' TODO LiveApi.PunchesServices_v2.XStandardPunchToPunchConverter

        <Fact(DisplayName:="Should Validate Provided Token Prior To Get Punches Data Between Dates")>
        Sub ShouldValidateProvidedTokenPriorToGetPunchesDataBetweenDates()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperWeb.HttpContextStub()
                helperPassport.PassportStub(1, helperDatalayer)
                helperDatalink.ValidateTokenSpy()
                helperDatalink.GetPunchesStub()

                'Act
                Dim punchesService As New PunchesService_v2
                Dim response As roWSResponse(Of roPunch())
                response = punchesService.GetPunchesBetweenDates("token", Now, Now)

                'Assert
                Assert.True(response.Status = Core.DTOs.ReturnCode._OK AndAlso helperDatalink.ValidateTokenCalled)
            End Using
        End Sub

        ' TODO RoboticsExternAccess.GetPunches

        <Fact(DisplayName:="Should Validate Provided Token Prior To Add Punches Data")>
        Sub ShouldValidateProvidedTokenPriorToAddPunchesData()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperWeb.HttpContextStub()
                helperPassport.PassportStub(1, helperDatalayer)
                helperDatalink.ValidateTokenSpy()
                helperDatalink.AddPunchesStub()

                'Act
                Dim punchesService As New PunchesService_v2
                Dim response As roWSResponse(Of roPunchesResponse())
                response = punchesService.AddPunches("token", (New List(Of roPunch)).ToArray())

                'Assert
                Assert.True(response.Status = Core.DTOs.ReturnCode._OK AndAlso helperDatalink.ValidateTokenCalled)
            End Using
        End Sub

        ' TODO RoboticsExternAccess.AddPunches

        <Fact(DisplayName:="Should Validate Provided Token Prior To Update Punches Data")>
        Sub ShouldValidateProvidedTokenPriorToUpdatePunchesData()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperWeb.HttpContextStub()
                helperPassport.PassportStub(1, helperDatalayer)
                helperDatalink.ValidateTokenSpy()
                helperDatalink.UpdatePunchesStub()

                'Act
                Dim punchesService As New PunchesService_v2
                Dim response As roWSResponse(Of List(Of roPunchesResponse))
                response = punchesService.UpdatePunches("token", (New List(Of roPunchCriteria)).ToArray)

                'Assert
                Assert.True(response.Status = Core.DTOs.ReturnCode._OK AndAlso helperDatalink.ValidateTokenCalled)
            End Using
        End Sub

        <Fact(DisplayName:="Should Update Punches When Valid Punches Is Provided")>
        Sub ShouldUpdatePunchesWhenValidPunchesIsProvided()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperWeb.HttpContextStub()
                helperPassport.PassportStub(1, helperDatalayer)
                helperDatalink.ValidateTokenSpy()
                helperDatalink.UpdateGetPunchesWithIDExStub()
                Dim punchesBD As New List(Of roDatalinkStandardPunch)
                punchesBD.Add(New roDatalinkStandardPunch With {.ID = 1, .DateTime = Date.Now, .Type = 1})
                punchesBD.Add(New roDatalinkStandardPunch With {.ID = 2, .DateTime = Date.Now, .Type = 2})
                helperDatalink.UpdatePunchesStub(punchesBD)

                'Act
                Dim punchesService As New PunchesService_v2
                Dim response As roWSResponse(Of List(Of roPunchesResponse))
                Dim punches As New List(Of roPunchCriteria)
                punches.Add(New roPunchCriteria With {.ID = 1, .DateTimeToUpdate = New Robotics.VTBase.roWCFDate(Date.Now), .Type = 1})
                punches.Add(New roPunchCriteria With {.ID = 2, .DateTimeToUpdate = New Robotics.VTBase.roWCFDate(Date.Now), .Type = 1})

                response = punchesService.UpdatePunches("token", punches.ToArray)

                'Assert
                Assert.True(response.Status = Core.DTOs.ReturnCode._OK AndAlso response.Value.Count = 0) 'Debe devolver una lista vacía ya que se actualizó correctamente
            End Using
        End Sub

        <Fact(DisplayName:="Should Return 2 Not Updated Punches When 2 Invalid IDs Are Provided On UpdatePunches")>
        Sub ShouldReturn2NotUpdatedPunchesWhen2InvalidIDsAreProvidedOnUpdatePunches()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperWeb.HttpContextStub()
                helperPassport.PassportStub(1, helperDatalayer)
                helperDatalink.ValidateTokenSpy()
                Dim punchesBD As New List(Of roDatalinkStandardPunch)
                punchesBD.Add(New roDatalinkStandardPunch With {.ID = 1, .DateTime = Date.Now, .Type = 1})
                punchesBD.Add(New roDatalinkStandardPunch With {.ID = 2, .DateTime = Date.Now, .Type = 2})
                punchesBD.Add(New roDatalinkStandardPunch With {.ID = 3, .DateTime = Date.Now, .Type = 2})
                helperDatalink.UpdateGetPunchesWithIDExStub(punchesBD)
                helperDatalink.UpdatePunchesStub(punchesBD)

                'Act
                Dim punchesService As New PunchesService_v2
                Dim response As roWSResponse(Of List(Of roPunchesResponse))
                Dim punches As New List(Of roPunchCriteria)
                punches.Add(New roPunchCriteria With {.ID = 3, .DateTimeToUpdate = New Robotics.VTBase.roWCFDate(Date.Now), .Type = 1})
                punches.Add(New roPunchCriteria With {.ID = 4, .DateTimeToUpdate = New Robotics.VTBase.roWCFDate(Date.Now), .Type = 1})
                punches.Add(New roPunchCriteria With {.ID = 5, .DateTimeToUpdate = New Robotics.VTBase.roWCFDate(Date.Now), .Type = 1})

                response = punchesService.UpdatePunches("token", punches.ToArray)

                'Assert
                Assert.True(response.Status = Core.DTOs.ReturnCode._OK AndAlso response.Value.Count = 2)
            End Using
        End Sub

        <Fact(DisplayName:="Should Return 2 Not Found Punches When 2 Invalid IDs Are Provided On UpdatePunches")>
        Sub ShouldReturn2NotFoundPunchesWhen2InvalidIDsAreProvidedOnUpdatePunches()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperWeb.HttpContextStub()
                helperPassport.PassportStub(1, helperDatalayer)
                helperDatalink.ValidateTokenSpy()
                Dim punchesBD As New List(Of roDatalinkStandardPunch)
                punchesBD.Add(New roDatalinkStandardPunch With {.ID = 1, .DateTime = Date.Now, .Type = 1})
                punchesBD.Add(New roDatalinkStandardPunch With {.ID = 2, .DateTime = Date.Now, .Type = 2})
                punchesBD.Add(New roDatalinkStandardPunch With {.ID = 3, .DateTime = Date.Now, .Type = 2})
                helperDatalink.UpdateGetPunchesWithIDExStub(punchesBD)
                helperDatalink.UpdatePunchesStub(punchesBD)

                'Act
                Dim punchesService As New PunchesService_v2
                Dim response As roWSResponse(Of List(Of roPunchesResponse))
                Dim punches As New List(Of roPunchCriteria)
                punches.Add(New roPunchCriteria With {.ID = 3, .DateTimeToUpdate = New Robotics.VTBase.roWCFDate(Date.Now), .Type = 1})
                punches.Add(New roPunchCriteria With {.ID = 4, .DateTimeToUpdate = New Robotics.VTBase.roWCFDate(Date.Now), .Type = 1})
                punches.Add(New roPunchCriteria With {.ID = 5, .DateTimeToUpdate = New Robotics.VTBase.roWCFDate(Date.Now), .Type = 1})

                response = punchesService.UpdatePunches("token", punches.ToArray)

                'Assert
                Assert.True(response.Value(0).Status = Core.DTOs.ReturnCode._PunchNotFound AndAlso response.Value(1).Status = Core.DTOs.ReturnCode._PunchNotFound)
            End Using
        End Sub

        'TODO: Otros casos para UpdatePunches incorrectos, como un tipo incorrecto, fecha fuera de periodo de congelación, fecha fuera de contrato, etc.

        <Fact(DisplayName:="Should Validate Provided Token Prior To Delete Punches Data")>
        Sub ShouldValidateProvidedTokenPriorToDeletePunchesData()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperWeb.HttpContextStub()
                helperPassport.PassportStub(1, helperDatalayer)
                helperDatalink.ValidateTokenSpy()
                helperDatalink.DeletePunchesStub()

                'Act
                Dim punchesService As New PunchesService_v2
                Dim response As roWSResponse(Of List(Of roPunchesResponse))
                response = punchesService.DeletePunches("token", (New List(Of roPunchToDeleteCriteria)).ToArray)

                'Assert
                Assert.True(response.Status = Core.DTOs.ReturnCode._OK AndAlso helperDatalink.ValidateTokenCalled)
            End Using
        End Sub

        <Fact(DisplayName:="Should Delete Punches When Valid Punches Is Provided")>
        Sub ShouldDeletePunchesWhenValidPunchesIsProvided()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperWeb.HttpContextStub()
                helperPassport.PassportStub(1, helperDatalayer)
                helperDatalink.ValidateTokenSpy()
                Dim punchesBD As New List(Of roDatalinkStandardPunch)
                punchesBD.Add(New roDatalinkStandardPunch With {.ID = 1, .DateTime = Date.Now, .Type = 1})
                punchesBD.Add(New roDatalinkStandardPunch With {.ID = 2, .DateTime = Date.Now, .Type = 2})
                helperDatalink.UpdateGetPunchesWithIDExStub(punchesBD)
                helperDatalink.DeletePunchesStub()

                'Act
                Dim punchesService As New PunchesService_v2
                Dim response As roWSResponse(Of List(Of roPunchesResponse))
                Dim punches As New List(Of roPunchToDeleteCriteria)
                punches.Add(New roPunchToDeleteCriteria With {.ID = 1})
                punches.Add(New roPunchToDeleteCriteria With {.ID = 2})

                response = punchesService.DeletePunches("token", punches.ToArray)

                'Assert
                Assert.True(response.Status = Core.DTOs.ReturnCode._OK AndAlso response.Value.Count = 0) 'Debe devolver una lista vacía ya que se actualizó correctamente
            End Using
        End Sub

        <Fact(DisplayName:="Should Delete 1 Punch Of 2 When 1 Valid Punch Is Provided")>
        Sub ShouldDelete1PunchOf2When1ValidPunchIsProvided()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperWeb.HttpContextStub()
                helperPassport.PassportStub(1, helperDatalayer)
                helperDatalink.ValidateTokenSpy()
                Dim punchesBD As New List(Of roDatalinkStandardPunch)
                punchesBD.Add(New roDatalinkStandardPunch With {.ID = 1, .DateTime = Date.Now, .Type = 1})
                helperDatalink.UpdateGetPunchesWithIDExStub(punchesBD)

                'Act
                Dim punchesService As New PunchesService_v2
                Dim response As roWSResponse(Of List(Of roPunchesResponse))
                Dim punches As New List(Of roPunchToDeleteCriteria)
                punches.Add(New roPunchToDeleteCriteria With {.ID = 1})
                punches.Add(New roPunchToDeleteCriteria With {.ID = 99})

                response = punchesService.DeletePunches("token", punches.ToArray)

                'Assert
                Assert.True(response.Status = Core.DTOs.ReturnCode._OK AndAlso response.Value.Count = 1)
            End Using
        End Sub

        <Fact(DisplayName:="Should Delete 0 Punches When 2 Invalid Punches Are Provided")>
        Sub ShouldDelete0PunchesWhen2InvalidPunchesAreProvided()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperWeb.HttpContextStub()
                helperPassport.PassportStub(1, helperDatalayer)
                helperDatalink.ValidateTokenSpy()
                Dim punchesBD = New Generic.List(Of roDatalinkStandardPunch)
                helperDatalink.UpdateGetPunchesWithIDExStub(punchesBD) 'Return 0 good punches on BD
                helperDatalink.DeletePunchesStub()

                'Act
                Dim punchesService As New PunchesService_v2
                Dim response As roWSResponse(Of List(Of roPunchesResponse))
                Dim punches As New List(Of roPunchToDeleteCriteria)
                punches.Add(New roPunchToDeleteCriteria With {.ID = 98})
                punches.Add(New roPunchToDeleteCriteria With {.ID = 99})

                response = punchesService.DeletePunches("token", punches.ToArray)

                'Assert
                Assert.True(response.Status = Core.DTOs.ReturnCode._OK AndAlso response.Value.Count = 2)
            End Using
        End Sub

        <Fact(DisplayName:="Should Not Delete Punch If InvalidEmployee Punch Is Provided")>
        Sub ShouldNotDeletePunchIfInvalidEmployeePunchIsProvided()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperWeb.HttpContextStub()
                helperPassport.PassportStub(1, helperDatalayer)
                helperDatalink.ValidateTokenSpy()
                Dim punchesBD As New List(Of roDatalinkStandardPunch)
                punchesBD.Add(New roDatalinkStandardPunch With {.ID = 1, .DateTime = Date.Now, .Type = 1})
                punchesBD.Add(New roDatalinkStandardPunch With {.ID = 2, .DateTime = Date.Now, .Type = 2})
                helperDatalink.UpdateGetPunchesWithIDExStub(punchesBD)

                'Act
                Dim punchesService As New PunchesService_v2
                Dim response As roWSResponse(Of List(Of roPunchesResponse))
                Dim punches As New List(Of roPunchToDeleteCriteria)
                punches.Add(New roPunchToDeleteCriteria With {.ID = 1})

                response = punchesService.DeletePunches("token", punches.ToArray)

                'Assert
                Assert.True(response.Value(0).Status = Core.DTOs.ReturnCode._InvalidEmployee)
                Assert.True(response.Value(1).Status = Core.DTOs.ReturnCode._InvalidEmployee)
            End Using
        End Sub

        <Fact(DisplayName:="Should Delete Punches If ValidEmployeeID Of Punches Are Provided")>
        Sub ShouldDeletePunchesIfValidEmployeeIDOfPunchesAreProvided()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperWeb.HttpContextStub()
                helperPassport.PassportStub(1, helperDatalayer)
                helperDatalink.ValidateTokenSpy()
                Dim punchesBD As New List(Of roDatalinkStandardPunch)
                punchesBD.Add(New roDatalinkStandardPunch With {.ID = 1, .DateTime = Date.Now, .Type = 1})
                punchesBD.Add(New roDatalinkStandardPunch With {.ID = 2, .DateTime = Date.Now, .Type = 2})
                helperDatalink.UpdateGetPunchesWithIDExStub(punchesBD)
                helperDatalink.isEmployeeNewStub(2) 'Devolvemos un ID de empleado cualquiera para forzar el idEmployee válido
                helperPunches.DeleteStub() 'Devolvemos true del delete

                'Act
                Dim punchesService As New PunchesService_v2
                Dim response As roWSResponse(Of List(Of roPunchesResponse))
                Dim punches As New List(Of roPunchToDeleteCriteria)
                punches.Add(New roPunchToDeleteCriteria With {.ID = 1})
                punches.Add(New roPunchToDeleteCriteria With {.ID = 2})

                response = punchesService.DeletePunches("token", punches.ToArray)

                'Assert
                Assert.True(response.Status = Core.DTOs.ReturnCode._OK AndAlso response.Value.Count = 0)
            End Using
        End Sub

        <Fact(DisplayName:="Should not delete punches if DateTime is on freezeDate")>
        Sub ShouldNotDeletePunchesIfDateTimeIsOnFreezeDate()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperWeb.HttpContextStub()
                helperPassport.PassportStub(1, helperDatalayer)
                helperDatalink.ValidateTokenSpy()
                Dim punchesBD As New List(Of roDatalinkStandardPunch)
                punchesBD.Add(New roDatalinkStandardPunch With {.ID = 1, .DateTime = Date.Now, .Type = 1})
                helperDatalink.UpdateGetPunchesWithIDExStub(punchesBD)
                helperDatalink.isEmployeeNewStub(2) 'Devolvemos un ID de empleado cualquiera para forzar el idEmployee válido
                helperBusiness.GetEmployeeLockDatetoApplyStub(Date.Now.AddDays(1))
                helperPunches.DeleteStub() 'Devolvemos true del delete

                'Act
                Dim punchesService As New PunchesService_v2
                Dim response As roWSResponse(Of List(Of roPunchesResponse))
                Dim punches As New List(Of roPunchToDeleteCriteria)
                punches.Add(New roPunchToDeleteCriteria With {.ID = 1})

                response = punchesService.DeletePunches("token", punches.ToArray)

                'Assert
                Assert.True(response.Value(0).Status = Core.DTOs.ReturnCode._InvalidPunchDate)
            End Using
        End Sub

        <Fact(DisplayName:="Should not delete punches if PunchType is not valid")>
        Sub ShouldNotDeletePunchesIfPunchTypeIsNotValid()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperWeb.HttpContextStub()
                helperPassport.PassportStub(1, helperDatalayer)
                helperDatalink.ValidateTokenSpy()
                Dim punchesBD As New List(Of roDatalinkStandardPunch)
                punchesBD.Add(New roDatalinkStandardPunch With {.ID = 1, .DateTime = Date.Now, .Type = 50})
                helperDatalink.UpdateGetPunchesWithIDExStub(punchesBD)
                helperDatalink.isEmployeeNewStub(1) 'Devolvemos un ID de empleado cualquiera para forzar el idEmployee válido
                helperBusiness.GetEmployeeLockDatetoApplyStub(Date.Now)
                helperPunches.DeleteStub() 'Devolvemos true del delete

                'Act
                Dim punchesService As New PunchesService_v2
                Dim response As roWSResponse(Of List(Of roPunchesResponse))
                Dim punches As New List(Of roPunchToDeleteCriteria)
                punches.Add(New roPunchToDeleteCriteria With {.ID = 1})

                response = punchesService.DeletePunches("token", punches.ToArray)

                'Assert
                Assert.True(response.Value(0).Status = Core.DTOs.ReturnCode._InvalidPunchType)
            End Using
        End Sub

        <Fact(DisplayName:="Should not delete punches if PunchID not exists")>
        Sub ShouldNotDeletePunchesIfPunchIDNotExists()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperWeb.HttpContextStub()
                helperPassport.PassportStub(1, helperDatalayer)
                helperDatalink.ValidateTokenSpy()
                Dim punchesBD As New List(Of roDatalinkStandardPunch)
                punchesBD.Add(New roDatalinkStandardPunch With {.ID = 0, .DateTime = Date.Now, .Type = 1})
                helperDatalink.UpdateGetPunchesWithIDExStub(punchesBD)
                helperDatalink.isEmployeeNewStub(2) 'Devolvemos un ID de empleado cualquiera para forzar el idEmployee válido
                helperBusiness.GetEmployeeLockDatetoApplyStub(Date.Now)
                helperPunches.DeleteStub() 'Devolvemos true del delete

                'Act
                Dim punchesService As New PunchesService_v2
                Dim response As roWSResponse(Of List(Of roPunchesResponse))
                Dim punches As New List(Of roPunchToDeleteCriteria)
                punches.Add(New roPunchToDeleteCriteria With {.ID = 0})

                response = punchesService.DeletePunches("token", punches.ToArray)

                'Assert
                Assert.True(response.Value(0).Status = Core.DTOs.ReturnCode._PunchNotFound)
            End Using
        End Sub

        <Fact(DisplayName:="Should return _ErrorUpdatingPunch if DeletePunch failed for some reason")>
        Sub ShouldReturn_ErrorUpdatingPunchIfDeletePunchFailedForSomeReason()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperWeb.HttpContextStub()
                helperPassport.PassportStub(1, helperDatalayer)
                helperDatalink.ValidateTokenSpy()
                Dim punchesBD As New List(Of roDatalinkStandardPunch)
                punchesBD.Add(New roDatalinkStandardPunch With {.ID = 1, .DateTime = Date.Now, .Type = 1})
                helperDatalink.UpdateGetPunchesWithIDExStub(punchesBD)
                helperDatalink.isEmployeeNewStub(2) 'Devolvemos un ID de empleado cualquiera para forzar el idEmployee válido
                helperBusiness.GetEmployeeLockDatetoApplyStub(Date.Now.AddDays(-1))
                helperPunches.DeleteStub(False) 'Forzamos FALSE del delete

                'Act
                Dim punchesService As New PunchesService_v2
                Dim response As roWSResponse(Of List(Of roPunchesResponse))
                Dim punches As New List(Of roPunchToDeleteCriteria)
                punches.Add(New roPunchToDeleteCriteria With {.ID = 1})

                response = punchesService.DeletePunches("token", punches.ToArray)

                'Assert
                Assert.True(response.Value(0).Status = Core.DTOs.ReturnCode._ErrorUpdatingPunch)
            End Using
        End Sub

        <Fact(DisplayName:="Should recalcPunches when punches are deleted")>
        Sub ShouldRecalcPunchesWhenPunchesAreDeleted()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperWeb.HttpContextStub()
                helperPassport.PassportStub(1, helperDatalayer)
                helperDatalink.ValidateTokenSpy()
                Dim punchesBD As New List(Of roDatalinkStandardPunch)
                punchesBD.Add(New roDatalinkStandardPunch With {.ID = 1, .DateTime = Date.Now, .Type = 1})
                helperDatalink.UpdateGetPunchesWithIDExStub(punchesBD)
                helperDatalink.isEmployeeNewStub(1) 'Devolvemos un ID de empleado cualquiera para forzar el idEmployee válido
                helperBusiness.GetEmployeeLockDatetoApplyStub(Date.Now.AddDays(-1))
                helperPunches.DeleteStub() 'Devolvemos true del delete
                helperBase.RecalcPunchesSpy()

                'Act
                Dim punchesService As New PunchesService_v2
                Dim response As roWSResponse(Of List(Of roPunchesResponse))
                Dim punches As New List(Of roPunchToDeleteCriteria)
                punches.Add(New roPunchToDeleteCriteria With {.ID = 1})

                response = punchesService.DeletePunches("token", punches.ToArray)

                'Assert
                Assert.True(response.Status = Core.DTOs.ReturnCode._OK AndAlso response.Value.Count = 0 AndAlso helperBase.RecalcPunchesCalled)
            End Using
        End Sub

        <Fact(DisplayName:="Should Not RecalcPunches If InvalidEmployee Punch Is Provided")>
        Sub ShouldNotRecalcPunchesIfInvalidEmployeePunchIsProvided()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperWeb.HttpContextStub()
                helperPassport.PassportStub(1, helperDatalayer)
                helperDatalink.ValidateTokenSpy()
                Dim punchesBD As New List(Of roDatalinkStandardPunch)
                punchesBD.Add(New roDatalinkStandardPunch With {.ID = 1, .DateTime = Date.Now, .Type = 1})
                helperDatalink.UpdateGetPunchesWithIDExStub(punchesBD)
                helperBase.RecalcPunchesSpy()

                'Act
                Dim punchesService As New PunchesService_v2
                Dim response As roWSResponse(Of List(Of roPunchesResponse))
                Dim punches As New List(Of roPunchToDeleteCriteria)
                punches.Add(New roPunchToDeleteCriteria With {.ID = 1})

                response = punchesService.DeletePunches("token", punches.ToArray)

                'Assert
                Assert.True(response.Value(0).Status = Core.DTOs.ReturnCode._InvalidEmployee AndAlso helperBase.RecalcPunchesCalled = False)
            End Using
        End Sub

        <Fact(DisplayName:="Should not recalc punches if DateTime is on freezeDate")>
        Sub ShouldNotRecalcPunchesIfDateTimeIsOnFreezeDate()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperWeb.HttpContextStub()
                helperPassport.PassportStub(1, helperDatalayer)
                helperDatalink.ValidateTokenSpy()
                Dim punchesBD As New List(Of roDatalinkStandardPunch)
                punchesBD.Add(New roDatalinkStandardPunch With {.ID = 1, .DateTime = Date.Now, .Type = 1})
                helperDatalink.UpdateGetPunchesWithIDExStub(punchesBD)
                helperDatalink.isEmployeeNewStub(2) 'Devolvemos un ID de empleado cualquiera para forzar el idEmployee válido
                helperBusiness.GetEmployeeLockDatetoApplyStub(Date.Now.AddDays(1))
                helperPunches.DeleteStub() 'Devolvemos true del delete
                helperBase.RecalcPunchesSpy()

                'Act
                Dim punchesService As New PunchesService_v2
                Dim response As roWSResponse(Of List(Of roPunchesResponse))
                Dim punches As New List(Of roPunchToDeleteCriteria)
                punches.Add(New roPunchToDeleteCriteria With {.ID = 1})

                response = punchesService.DeletePunches("token", punches.ToArray)

                'Assert
                Assert.True(response.Value(0).Status = Core.DTOs.ReturnCode._InvalidPunchDate AndAlso helperBase.RecalcPunchesCalled = False)
            End Using
        End Sub

        <Fact(DisplayName:="Should not recalc punches if PunchType is not valid")>
        Sub ShouldNotRecalcPunchesIfPunchTypeIsNotValid()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperWeb.HttpContextStub()
                helperPassport.PassportStub(1, helperDatalayer)
                helperDatalink.ValidateTokenSpy()
                Dim punchesBD As New List(Of roDatalinkStandardPunch)
                punchesBD.Add(New roDatalinkStandardPunch With {.ID = 1, .DateTime = Date.Now, .Type = 50})
                helperDatalink.UpdateGetPunchesWithIDExStub(punchesBD)
                helperDatalink.isEmployeeNewStub(1) 'Devolvemos un ID de empleado cualquiera para forzar el idEmployee válido
                helperBusiness.GetEmployeeLockDatetoApplyStub(Date.Now)
                helperPunches.DeleteStub() 'Devolvemos true del delete
                helperBase.RecalcPunchesSpy()

                'Act
                Dim punchesService As New PunchesService_v2
                Dim response As roWSResponse(Of List(Of roPunchesResponse))
                Dim punches As New List(Of roPunchToDeleteCriteria)
                punches.Add(New roPunchToDeleteCriteria With {.ID = 1})

                response = punchesService.DeletePunches("token", punches.ToArray)

                'Assert
                Assert.True(response.Value(0).Status = Core.DTOs.ReturnCode._InvalidPunchType AndAlso helperBase.RecalcPunchesCalled = False)
            End Using
        End Sub

        <Fact(DisplayName:="Should not recalc punches if PunchID not exists")>
        Sub ShouldNotRecalcPunchesIfPunchIDNotExists()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperWeb.HttpContextStub()
                helperPassport.PassportStub(1, helperDatalayer)
                helperDatalink.ValidateTokenSpy()
                Dim punchesBD As New List(Of roDatalinkStandardPunch)
                punchesBD.Add(New roDatalinkStandardPunch With {.ID = 0, .DateTime = Date.Now, .Type = 1})
                helperDatalink.UpdateGetPunchesWithIDExStub(punchesBD)
                helperDatalink.isEmployeeNewStub(2) 'Devolvemos un ID de empleado cualquiera para forzar el idEmployee válido
                helperBusiness.GetEmployeeLockDatetoApplyStub(Date.Now)
                helperPunches.DeleteStub() 'Devolvemos true del delete
                helperBase.RecalcPunchesSpy()

                'Act
                Dim punchesService As New PunchesService_v2
                Dim response As roWSResponse(Of List(Of roPunchesResponse))
                Dim punches As New List(Of roPunchToDeleteCriteria)
                punches.Add(New roPunchToDeleteCriteria With {.ID = 0})

                response = punchesService.DeletePunches("token", punches.ToArray)

                'Assert
                Assert.True(response.Value(0).Status = Core.DTOs.ReturnCode._PunchNotFound AndAlso helperBase.RecalcPunchesCalled = False)
            End Using
        End Sub

        <Fact(DisplayName:="Should not recalcPunches when punches are not deleted")>
        Sub ShouldNotRecalcPunchesWhenPunchesAreNotDeleted()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperWeb.HttpContextStub()
                helperPassport.PassportStub(1, helperDatalayer)
                helperDatalink.ValidateTokenSpy()
                Dim punchesBD As New List(Of roDatalinkStandardPunch)
                punchesBD.Add(New roDatalinkStandardPunch With {.ID = 1, .DateTime = Date.Now, .Type = 1})
                helperDatalink.UpdateGetPunchesWithIDExStub(punchesBD)
                helperDatalink.isEmployeeNewStub(2) 'Devolvemos un ID de empleado cualquiera para forzar el idEmployee válido
                helperBusiness.GetEmployeeLockDatetoApplyStub(Date.Now.AddDays(-1))
                helperPunches.DeleteStub(False) 'Forzamos FALSE del delete
                helperBase.RecalcPunchesSpy()

                'Act
                Dim punchesService As New PunchesService_v2
                Dim response As roWSResponse(Of List(Of roPunchesResponse))
                Dim punches As New List(Of roPunchToDeleteCriteria)
                punches.Add(New roPunchToDeleteCriteria With {.ID = 1})

                response = punchesService.DeletePunches("token", punches.ToArray)

                'Assert
                Assert.True(response.Value(0).Status = Core.DTOs.ReturnCode._ErrorUpdatingPunch AndAlso helperBase.RecalcPunchesCalled = False)
            End Using
        End Sub

        <Fact(DisplayName:="Should Return invalid punch If InvalidEmployee Punch Is Provided")>
        Sub ShouldReturnInvalidPunchIfInvalidEmployeePunchIsProvided()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperWeb.HttpContextStub()
                helperPassport.PassportStub(1, helperDatalayer)
                helperDatalink.ValidateTokenSpy()
                Dim punchesBD As New List(Of roDatalinkStandardPunch)
                punchesBD.Add(New roDatalinkStandardPunch With {.ID = 5, .DateTime = Date.Now, .Type = 1})
                helperDatalink.UpdateGetPunchesWithIDExStub(punchesBD)
                helperBase.RecalcPunchesSpy()

                'Act
                Dim punchesService As New PunchesService_v2
                Dim response As roWSResponse(Of List(Of roPunchesResponse))
                Dim punches As New List(Of roPunchToDeleteCriteria)
                punches.Add(New roPunchToDeleteCriteria With {.ID = 5})

                response = punchesService.DeletePunches("token", punches.ToArray)

                'Assert
                Assert.True(response.Value.Count = 1 AndAlso response.Value(0).oPunch.ID = 5)
            End Using
        End Sub

        <Fact(DisplayName:="Should Return invalid punch if DateTime is on freezeDate")>
        Sub ShouldReturnInvalidPunchIfDateTimeIsOnFreezeDate()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperWeb.HttpContextStub()
                helperPassport.PassportStub(1, helperDatalayer)
                helperDatalink.ValidateTokenSpy()
                Dim punchesBD As New List(Of roDatalinkStandardPunch)
                punchesBD.Add(New roDatalinkStandardPunch With {.ID = 6, .DateTime = Date.Now, .Type = 1})
                helperDatalink.UpdateGetPunchesWithIDExStub(punchesBD)
                helperDatalink.isEmployeeNewStub(2) 'Devolvemos un ID de empleado cualquiera para forzar el idEmployee válido
                helperBusiness.GetEmployeeLockDatetoApplyStub(Date.Now.AddDays(1))
                helperPunches.DeleteStub() 'Devolvemos true del delete
                helperBase.RecalcPunchesSpy()

                'Act
                Dim punchesService As New PunchesService_v2
                Dim response As roWSResponse(Of List(Of roPunchesResponse))
                Dim punches As New List(Of roPunchToDeleteCriteria)
                punches.Add(New roPunchToDeleteCriteria With {.ID = 6})

                response = punchesService.DeletePunches("token", punches.ToArray)

                'Assert
                Assert.True(response.Value.Count = 1 AndAlso response.Value(0).oPunch IsNot Nothing)
            End Using
        End Sub

        <Fact(DisplayName:="Should Return invalid punch if PunchType is not valid")>
        Sub ShouldReturnInvalidPunchIfPunchTypeIsNotValid()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperWeb.HttpContextStub()
                helperPassport.PassportStub(1, helperDatalayer)
                helperDatalink.ValidateTokenSpy()
                Dim punchesBD As New List(Of roDatalinkStandardPunch)
                punchesBD.Add(New roDatalinkStandardPunch With {.ID = 4, .DateTime = Date.Now, .Type = 50})
                punchesBD.Add(New roDatalinkStandardPunch With {.ID = 6, .DateTime = Date.Now, .Type = 1})
                helperDatalink.UpdateGetPunchesWithIDExStub(punchesBD)
                helperDatalink.isEmployeeNewStub(1) 'Devolvemos un ID de empleado cualquiera para forzar el idEmployee válido
                helperBusiness.GetEmployeeLockDatetoApplyStub(Date.Now.AddDays(-1))
                helperPunches.DeleteStub() 'Devolvemos true del delete
                helperBase.RecalcPunchesSpy()

                'Act
                Dim punchesService As New PunchesService_v2
                Dim response As roWSResponse(Of List(Of roPunchesResponse))
                Dim punches As New List(Of roPunchToDeleteCriteria)
                punches.Add(New roPunchToDeleteCriteria With {.ID = 4})
                punches.Add(New roPunchToDeleteCriteria With {.ID = 6})

                response = punchesService.DeletePunches("token", punches.ToArray)

                'Assert
                Assert.True(response.Value.Count = 1 AndAlso response.Value(0).oPunch.ID = 4)
            End Using
        End Sub

        <Fact(DisplayName:="Should Return invalid punch if PunchID not exists")>
        Sub ShouldReturnInvalidPunchIfPunchIDNotExists()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperWeb.HttpContextStub()
                helperPassport.PassportStub(1, helperDatalayer)
                helperDatalink.ValidateTokenSpy()
                Dim punchesBD As New List(Of roDatalinkStandardPunch)
                punchesBD.Add(New roDatalinkStandardPunch With {.ID = 0, .DateTime = Date.Now, .Type = 1})
                helperDatalink.UpdateGetPunchesWithIDExStub(punchesBD)
                helperDatalink.isEmployeeNewStub(2) 'Devolvemos un ID de empleado cualquiera para forzar el idEmployee válido
                helperBusiness.GetEmployeeLockDatetoApplyStub(Date.Now.AddDays(-1))
                helperPunches.DeleteStub() 'Devolvemos true del delete
                helperBase.RecalcPunchesSpy()

                'Act
                Dim punchesService As New PunchesService_v2
                Dim response As roWSResponse(Of List(Of roPunchesResponse))
                Dim punches As New List(Of roPunchToDeleteCriteria)
                punches.Add(New roPunchToDeleteCriteria With {.ID = 0})

                response = punchesService.DeletePunches("token", punches.ToArray)

                'Assert
                Assert.True(response.Value.Count = 1 AndAlso response.Value(0).oPunch.ID = 0)
            End Using
        End Sub

        <Fact(DisplayName:="Should Return invalid punch when punch are not deleted")>
        Sub ShouldReturnInvalidPunchWhenPunchAreNotDeleted()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperWeb.HttpContextStub()
                helperPassport.PassportStub(1, helperDatalayer)
                helperDatalink.ValidateTokenSpy()
                Dim punchesBD As New List(Of roDatalinkStandardPunch)
                punchesBD.Add(New roDatalinkStandardPunch With {.ID = 4, .DateTime = Date.Now, .Type = 1})
                helperDatalink.UpdateGetPunchesWithIDExStub(punchesBD)
                helperDatalink.isEmployeeNewStub(2) 'Devolvemos un ID de empleado cualquiera para forzar el idEmployee válido
                helperBusiness.GetEmployeeLockDatetoApplyStub(Date.Now.AddDays(-1))
                helperPunches.DeleteStub(False) 'Forzamos FALSE del delete
                helperBase.RecalcPunchesSpy()

                'Act
                Dim punchesService As New PunchesService_v2
                Dim response As roWSResponse(Of List(Of roPunchesResponse))
                Dim punches As New List(Of roPunchToDeleteCriteria)
                punches.Add(New roPunchToDeleteCriteria With {.ID = 4})

                response = punchesService.DeletePunches("token", punches.ToArray)

                'Assert
                Assert.True(response.Value.Count = 1 AndAlso response.Value(0).oPunch.ID = 4)
            End Using
        End Sub

        ' TODO VTLiveAPI.TasksService_v2

        ' TODO VTLiveAPI.ScheduleService_v2



        <Fact(DisplayName:="Should save passport when createorupdateemployee is called with no auhtenticationmethods and biometrics are active")>
        Sub ShouldSavePassportWhenCreateOrUpdateEmployeeIsCalledWithNoAuhtenticationMethodsAndBiometricsAreActive()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperWeb.HttpContextStub()
                helperDatalink.ValidateTokenSpy()
                helperPassport.PassportStub(1, helperDatalayer)
                helperPassport.Update()
                helperDatalink.isEmployeeNewStub(-1) 'Forzamos a que sea un empleado nuevo
                helperDatalink.GetEmployeeByIdStub()
                helperEmployee.SaveEmployeeSpy()
                helperEmployee.SaveEmployeeUserField()
                helperEmployee.SaveUserField()
                helperEmployee.GetUserFieldsStub()
                helperEmployee.SaveMobility()
                helperEmployee.SaveContract()
                helperParameters.ParameterStub(New Dictionary(Of String, String) From {{"DisableBiometricData", "0"}})

                'Act
                Dim employeeService As New VTLiveApi.EmployeeService_v2
                Dim response As roDatalinkStandarEmployeeResponse
                Dim oEmployee As New roDatalinkStandarEmployee With {
                    .CompositeContractType = eCompositeContractType.None,
                    .NifEmpleado = "123456789A",
                    .UniqueEmployeeID = "123456789A",
                    .IDContract = "123456789A.20240513",
                    .NombreEmpleado = "Test",
                    .StartContractDate = New DateTime(2024, 1, 1)
                    }

                response = employeeService.CreateOrUpdateEmployee("token", oEmployee)

                'Assert
                Assert.True(response.ResultCode = Core.DTOs.ReturnCode._OK)
                Assert.True(helperPassport.UpdatePassportCalled)
                Assert.True(helperEmployee.SaveEmployeeCalled)
            End Using
        End Sub

        <Fact(DisplayName:="Should save passport when createorupdateemployee is called with no auhtenticationmethods and biometrics are inactive")>
        Sub ShouldSavePassportWhenCreateOrUpdateEmployeeIsCalledWithNoAuhtenticationMethodsAndBiometricsAreInactive()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperWeb.HttpContextStub()
                helperDatalink.ValidateTokenSpy()
                helperPassport.PassportStub(1, helperDatalayer)
                helperPassport.Update()
                helperDatalink.isEmployeeNewStub(-1) 'Forzamos a que sea un empleado nuevo
                helperDatalink.GetEmployeeByIdStub()
                helperEmployee.SaveEmployeeSpy()
                helperEmployee.SaveEmployeeUserField()
                helperEmployee.SaveUserField()
                helperEmployee.GetUserFieldsStub()
                helperEmployee.SaveMobility()
                helperEmployee.SaveContract()
                helperParameters.ParameterStub(New Dictionary(Of String, String) From {{"DisableBiometricData", "1"}})

                'Act
                Dim employeeService As New VTLiveApi.EmployeeService_v2
                Dim response As roDatalinkStandarEmployeeResponse
                Dim oEmployee As New roDatalinkStandarEmployee With {
                    .CompositeContractType = eCompositeContractType.None,
                    .NifEmpleado = "123456789A",
                    .UniqueEmployeeID = "123456789A",
                    .IDContract = "123456789A.20240513",
                    .NombreEmpleado = "Test",
                    .StartContractDate = New DateTime(2024, 1, 1)
                    }

                response = employeeService.CreateOrUpdateEmployee("token", oEmployee)

                'Assert
                Assert.True(response.ResultCode = Core.DTOs.ReturnCode._OK)
                Assert.True(helperPassport.UpdatePassportCalled)
                Assert.True(helperEmployee.SaveEmployeeCalled)
            End Using
        End Sub

        <Fact(DisplayName:="Should save passport when createorupdateemployee is called with credential auhtenticationmethods and biometrics are inactive")>
        Sub ShouldSavePassportWhenCreateOrUpdateEmployeeIsCalledWithCredentialAuhtenticationMethodsAndBiometricsAreInactive()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperWeb.HttpContextStub()
                helperDatalink.ValidateTokenSpy()
                helperPassport.PassportStub(1, helperDatalayer)
                helperPassport.Update()
                helperDatalink.isEmployeeNewStub(-1) 'Forzamos a que sea un empleado nuevo
                helperDatalink.GetEmployeeByIdStub()
                helperEmployee.SaveEmployeeSpy()
                helperEmployee.SaveEmployeeUserField()
                helperEmployee.SaveUserField()
                helperEmployee.GetUserFieldsStub()
                helperEmployee.SaveMobility()
                helperEmployee.SaveContract()
                helperParameters.ParameterStub(New Dictionary(Of String, String) From {{"DisableBiometricData", "1"}})

                'Act
                Dim employeeService As New VTLiveApi.EmployeeService_v2
                Dim response As roDatalinkStandarEmployeeResponse
                Dim oEmployee As New roDatalinkStandarEmployee With {
                    .CompositeContractType = eCompositeContractType.None,
                    .NifEmpleado = "123456789A",
                    .UniqueEmployeeID = "123456789A",
                    .IDContract = "123456789A.20240513",
                    .NombreEmpleado = "Test",
                    .UserName = "test",
                    .StartContractDate = New DateTime(2024, 1, 1)
                    }

                response = employeeService.CreateOrUpdateEmployee("token", oEmployee)

                'Assert
                Assert.True(response.ResultCode = Core.DTOs.ReturnCode._OK)
                Assert.True(helperPassport.UpdatePassportCalled)
                Assert.True(helperEmployee.SaveEmployeeCalled)
            End Using
        End Sub

        <Fact(DisplayName:="Should save passport when createorupdateemployee is called with card auhtenticationmethods and biometrics are inactive")>
        Sub ShouldSavePassportWhenCreateOrUpdateEmployeeIsCalledWithCardAuhtenticationMethodsAndBiometricsAreInactive()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperWeb.HttpContextStub()
                helperDatalink.ValidateTokenSpy()
                helperPassport.PassportStub(1, helperDatalayer)
                helperPassport.Update()
                helperDatalink.isEmployeeNewStub(-1) 'Forzamos a que sea un empleado nuevo
                helperDatalink.GetEmployeeByIdStub()
                helperEmployee.SaveEmployeeSpy()
                helperEmployee.SaveEmployeeUserField()
                helperEmployee.SaveUserField()
                helperEmployee.GetUserFieldsStub()
                helperEmployee.SaveMobility()
                helperEmployee.SaveContract()
                helperParameters.ParameterStub(New Dictionary(Of String, String) From {{"DisableBiometricData", "1"}})

                'Act
                Dim employeeService As New VTLiveApi.EmployeeService_v2
                Dim response As roDatalinkStandarEmployeeResponse
                Dim oEmployee As New roDatalinkStandarEmployee With {
                    .CompositeContractType = eCompositeContractType.None,
                    .NifEmpleado = "123456789A",
                    .UniqueEmployeeID = "123456789A",
                    .IDContract = "123456789A.20240513",
                    .NombreEmpleado = "Test",
                    .CardNumber = "12345678",
                    .StartContractDate = New DateTime(2024, 1, 1)
                    }

                response = employeeService.CreateOrUpdateEmployee("token", oEmployee)

                'Assert
                Assert.True(response.ResultCode = Core.DTOs.ReturnCode._OK)
                Assert.True(helperPassport.UpdatePassportCalled)
                Assert.True(helperEmployee.SaveEmployeeCalled)
            End Using
        End Sub

        <Fact(DisplayName:="Should save passport when createorupdateemployee is called with pin auhtenticationmethods and biometrics are inactive")>
        Sub ShouldSavePassportWhenCreateOrUpdateEmployeeIsCalledWithPinAuhtenticationMethodsAndBiometricsAreInactive()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperWeb.HttpContextStub()
                helperDatalink.ValidateTokenSpy()
                helperPassport.PassportStub(1, helperDatalayer)
                helperPassport.Update()
                helperDatalink.isEmployeeNewStub(-1) 'Forzamos a que sea un empleado nuevo
                helperDatalink.GetEmployeeByIdStub()
                helperEmployee.SaveEmployeeSpy()
                helperEmployee.SaveEmployeeUserField()
                helperEmployee.SaveUserField()
                helperEmployee.GetUserFieldsStub()
                helperEmployee.SaveMobility()
                helperEmployee.SaveContract()
                helperParameters.ParameterStub(New Dictionary(Of String, String) From {{"DisableBiometricData", "1"}})

                'Act
                Dim employeeService As New VTLiveApi.EmployeeService_v2
                Dim response As roDatalinkStandarEmployeeResponse
                Dim oEmployee As New roDatalinkStandarEmployee With {
                    .CompositeContractType = eCompositeContractType.None,
                    .NifEmpleado = "123456789A",
                    .UniqueEmployeeID = "123456789A",
                    .IDContract = "123456789A.20240513",
                    .NombreEmpleado = "Test",
                    .Pin = "123123",
                    .StartContractDate = New DateTime(2024, 1, 1)
                    }

                response = employeeService.CreateOrUpdateEmployee("token", oEmployee)

                'Assert
                Assert.True(response.ResultCode = Core.DTOs.ReturnCode._OK)
                Assert.True(helperPassport.UpdatePassportCalled)
                Assert.True(helperEmployee.SaveEmployeeCalled)
            End Using
        End Sub

        <Fact(DisplayName:="Should not save employee when pin length is less than four")>
        Sub ShouldNotSaveEmployeeWhenPinLengthIsLessThanFour()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperWeb.HttpContextStub()
                helperDatalink.ValidateTokenSpy()
                helperPassport.PassportStub(1, helperDatalayer)
                helperPassport.Update()
                helperDatalink.isEmployeeNewStub(-1) 'Forzamos a que sea un empleado nuevo
                helperDatalink.GetEmployeeByIdStub()
                helperEmployee.SaveEmployeeSpy()
                helperEmployee.SaveEmployeeUserField()
                helperEmployee.SaveUserField()
                helperEmployee.GetUserFieldsStub()
                helperEmployee.SaveMobility()
                helperEmployee.SaveContract()
                helperParameters.ParameterStub(New Dictionary(Of String, String) From {{"DisableBiometricData", "1"}})

                'Act
                Dim employeeService As New VTLiveApi.EmployeeService_v2
                Dim response As roDatalinkStandarEmployeeResponse
                Dim oEmployee As New roDatalinkStandarEmployee With {
                    .CompositeContractType = eCompositeContractType.None,
                    .NifEmpleado = "123456789A",
                    .UniqueEmployeeID = "123456789A",
                    .IDContract = "123456789A.20240513",
                    .NombreEmpleado = "Test",
                    .Pin = "123",
                    .StartContractDate = New DateTime(2024, 1, 1)
                    }

                response = employeeService.CreateOrUpdateEmployee("token", oEmployee)

                'Assert
                Assert.True(response.ResultCode = Core.DTOs.ReturnCode._InvalidPinLength)
            End Using
        End Sub

    End Class

End Namespace