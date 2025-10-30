Imports System.Net
Imports System.ServiceModel.Activation
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.ExternalSystems.DataLink.RoboticsExternAccess
Imports Robotics.VTBase
Imports SwaggerWcf.Attributes

<AspNetCompatibilityRequirements(RequirementsMode:=AspNetCompatibilityRequirementsMode.Allowed)>
Public Class ScheduleService
    Implements IScheduleService

    <SwaggerWcfTag("Calendar")>
    <SwaggerWcfResponse(HttpStatusCode.OK, "OK")>
    <SwaggerWcfResponse(HttpStatusCode.BadRequest, "Bad request", True)>
    <SwaggerWcfResponse(HttpStatusCode.InternalServerError, "Internal error (can be forced using ERROR_500 as book title)", True)>
    Public Function GetHolidays(ByVal CompanyCode As String, ByVal UserName As String, ByVal UserPwd As String, ByVal EmployeeID As String, ByVal StartDate As Date, ByVal EndDate As Date, Optional Criteria As String = "") As roWSResponse(Of roHoliday()) Implements IScheduleService.GetHolidays
        Dim oWSResponse As New roWSResponse(Of roHoliday()) With {
            .Status = Core.DTOs.ReturnCode._UnknownError,
            .Text = String.Empty
        }

        Try
            Dim cCode As String = CompanyCode
            Dim usr As String = UserName
            Dim pwd As String = UserPwd

            If EmployeeID Is Nothing Then EmployeeID = String.Empty

            HttpContext.Current.Session("roClientCompanyId") = cCode
            Dim externAccessInstance As RoboticsExternAccess = RoboticsExternAccess.GetInstance(True, cCode)

            oWSResponse.ApiVersion = roTypes.Any2String(AdvancedParameter.roAdvancedParameter.GetAdvancedParameterValue("VTWebAPIVersion", Nothing))

            Dim iResultStatus As Core.DTOs.ReturnCode = Core.DTOs.ReturnCode._UnknownError
            Dim iResultText As String = String.Empty
            Dim oHolidays As New List(Of roHoliday)

            If externAccessInstance.ValidateUserNamePassword(usr, pwd, HttpContext.Current.Request.UserHostAddress, iResultStatus, True) Then
                iResultStatus = Core.DTOs.ReturnCode._UnknownError

                Dim GetDeletedHolidays = (Criteria IsNot Nothing AndAlso Criteria.ToUpper = "TIMESTAMP")

                externAccessInstance.GetHolidays(oHolidays, EmployeeID, StartDate, EndDate, iResultStatus, iResultText, GetDeletedHolidays)
            End If

            oWSResponse.Value = oHolidays.ToArray
            oWSResponse.Status = iResultStatus
            oWSResponse.Text = iResultStatus.ToString

            HttpContext.Current.Session("roClientCompanyId") = String.Empty
        Catch ex As Exception
            oWSResponse.Status = Core.DTOs.ReturnCode._UnknownError
            oWSResponse.Text = Core.DTOs.ReturnCode._UnknownError.ToString
        End Try

        Return oWSResponse

    End Function

    <SwaggerWcfTag("Calendar")>
    <SwaggerWcfResponse(HttpStatusCode.OK, "OK")>
    <SwaggerWcfResponse(HttpStatusCode.BadRequest, "Bad request", True)>
    <SwaggerWcfResponse(HttpStatusCode.InternalServerError,
    "Internal error", True)>
    Public Function CreateOrUpdateHolidays(ByVal CompanyCode As String, ByVal UserName As String, ByVal UserPwd As String, ByVal oHolidays As roHoliday) As roWSResponse(Of roHoliday()) Implements IScheduleService.CreateOrUpdateHolidays
        Dim oWSResponse As New roWSResponse(Of roHoliday()) With {
            .Status = Core.DTOs.ReturnCode._UnknownError,
            .Text = String.Empty
        }

        Try
            Dim cCode As String = CompanyCode
            Dim usr As String = UserName
            Dim pwd As String = UserPwd

            HttpContext.Current.Session("roClientCompanyId") = cCode
            Dim externAccessInstance As RoboticsExternAccess = RoboticsExternAccess.GetInstance(True, cCode)

            oWSResponse.ApiVersion = roTypes.Any2String(AdvancedParameter.roAdvancedParameter.GetAdvancedParameterValue("VTWebAPIVersion", Nothing))

            Dim resultStatus As Core.DTOs.ReturnCode = Core.DTOs.ReturnCode._UnknownError
            Dim sResultText As String = ""
            Dim oHolidaysDatalink As New roDatalinkStandarHolidays

            If externAccessInstance.ValidateUserNamePassword(usr, pwd, HttpContext.Current.Request.UserHostAddress, resultStatus, True) Then
                resultStatus = Core.DTOs.ReturnCode._UnknownError
                If oHolidays.HolidayType = HolidayType_Enum.Days Then
                    oHolidaysDatalink = New roDatalinkStandarHolidays
                    oHolidaysDatalink.Action = oHolidays.Action
                    oHolidaysDatalink.UniqueEmployeeID = oHolidays.IDEmployee
                    oHolidaysDatalink.ShiftKey = oHolidays.IDReason
                    oHolidaysDatalink.PlanDate = oHolidays.PlannedDate.GetDate().Date
                    externAccessInstance.CreateOrUpdateHolidays(oHolidaysDatalink, resultStatus)
                Else
                    resultStatus = Core.DTOs.ReturnCode._InvalidShift
                End If
            End If

            oWSResponse.Value = Nothing
            oWSResponse.Status = Convert.ToInt32(resultStatus)
            oWSResponse.Text = resultStatus.ToString

            HttpContext.Current.Session("roClientCompanyId") = String.Empty
        Catch ex As Exception
            oWSResponse.Status = Core.DTOs.ReturnCode._UnknownError
            oWSResponse.Text = Core.DTOs.ReturnCode._UnknownError.ToString
        End Try

        Return oWSResponse
    End Function

    <SwaggerWcfTag("Calendar")>
    <SwaggerWcfResponse(HttpStatusCode.OK, "OK")>
    <SwaggerWcfResponse(HttpStatusCode.BadRequest, "Bad request", True)>
    <SwaggerWcfResponse(HttpStatusCode.InternalServerError, "Internal error", True)>
    Public Function CreateOrUpdateCalendar(ByVal CompanyCode As String, ByVal UserName As String, ByVal UserPwd As String, ByVal oCalendar As roCalendar) As roWSResponse(Of roCalendar()) Implements IScheduleService.CreateOrUpdateCalendar
        Dim oWSResponse As New roWSResponse(Of roCalendar()) With {
            .Status = Core.DTOs.ReturnCode._UnknownError,
            .Text = String.Empty
        }

        Try
            Dim cCode As String = CompanyCode
            Dim usr As String = UserName
            Dim pwd As String = UserPwd

            HttpContext.Current.Session("roClientCompanyId") = cCode
            Dim externAccessInstance As RoboticsExternAccess = RoboticsExternAccess.GetInstance(True, cCode)

            oWSResponse.ApiVersion = roTypes.Any2String(AdvancedParameter.roAdvancedParameter.GetAdvancedParameterValue("VTWebAPIVersion", Nothing))

            Dim resultStatus As Core.DTOs.ReturnCode = Core.DTOs.ReturnCode._UnknownError
            Dim sResultText As String = ""
            Dim oCalendarDatalink As New roDatalinkStandarCalendar

            If externAccessInstance.ValidateUserNamePassword(usr, pwd, HttpContext.Current.Request.UserHostAddress, resultStatus, True) Then
                resultStatus = Core.DTOs.ReturnCode._UnknownError
                oCalendarDatalink = New roDatalinkStandarCalendar
                oCalendarDatalink.UniqueEmployeeID = oCalendar.IDEmployee
                oCalendarDatalink.ShiftKey = oCalendar.IDShift
                oCalendarDatalink.PlanDate = oCalendar.PlannedDate.GetDate().Date
                externAccessInstance.CreateOrUpdateCalendar(oCalendarDatalink, resultStatus)
            End If

            oWSResponse.Value = Nothing
            oWSResponse.Status = Convert.ToInt32(resultStatus)
            oWSResponse.Text = resultStatus.ToString

            HttpContext.Current.Session("roClientCompanyId") = String.Empty
        Catch ex As Exception
            oWSResponse.Status = Core.DTOs.ReturnCode._UnknownError
            oWSResponse.Text = Core.DTOs.ReturnCode._UnknownError.ToString
        End Try

        Return oWSResponse
    End Function

    <SwaggerWcfTag("Calendar")>
    <SwaggerWcfResponse(HttpStatusCode.OK, "OK")>
    <SwaggerWcfResponse(HttpStatusCode.BadRequest, "Bad request", True)>
    <SwaggerWcfResponse(HttpStatusCode.InternalServerError, "Internal error", True)>
    Public Function CreateOrUpdateCalendarBatch(ByVal CompanyCode As String, ByVal UserName As String, ByVal UserPwd As String, ByVal oCalendars As roCalendar()) As roWSResponse(Of roCalendarResponse()) Implements IScheduleService.CreateOrUpdateCalendarBatch
        Dim oWSResponse As New roWSResponse(Of roCalendarResponse()) With {
            .Status = Core.DTOs.ReturnCode._UnknownError,
            .Text = String.Empty
        }

        Try
            Dim cCode As String = CompanyCode
            Dim usr As String = UserName
            Dim pwd As String = UserPwd

            HttpContext.Current.Session("roClientCompanyId") = cCode
            Dim externAccessInstance As RoboticsExternAccess = RoboticsExternAccess.GetInstance(True, cCode)

            oWSResponse.ApiVersion = roTypes.Any2String(AdvancedParameter.roAdvancedParameter.GetAdvancedParameterValue("VTWebAPIVersion", Nothing))

            Dim resultStatus As Core.DTOs.ReturnCode = Core.DTOs.ReturnCode._UnknownError
            Dim sResultText As String = ""
            Dim oCalendarDatalink As New roDatalinkStandarCalendar
            Dim resultStatusGlobal As Core.DTOs.ReturnCode = Core.DTOs.ReturnCode._OK
            Dim lstCalendarResponse As New Generic.List(Of roCalendarResponse)
            Dim x As Integer = 0

            If externAccessInstance.ValidateUserNamePassword(usr, pwd, HttpContext.Current.Request.UserHostAddress, resultStatusGlobal, True) Then
                For Each oCalendar As roCalendar In oCalendars
                    resultStatus = Core.DTOs.ReturnCode._UnknownError
                    oCalendarDatalink = New roDatalinkStandarCalendar
                    oCalendarDatalink.UniqueEmployeeID = oCalendar.IDEmployee
                    oCalendarDatalink.ShiftKey = oCalendar.IDShift
                    oCalendarDatalink.PlanDate = oCalendar.PlannedDate.GetDate().Date
                    externAccessInstance.CreateOrUpdateCalendar(oCalendarDatalink, resultStatus)
                    If resultStatus <> Core.DTOs.ReturnCode._OK Then
                        Dim oCalendarResponse As New roCalendarResponse
                        oCalendarResponse.oCalendar = oCalendar
                        oCalendarResponse.Status = Convert.ToInt32(resultStatus)
                        oCalendarResponse.Text = resultStatus.ToString
                        lstCalendarResponse.Add(oCalendarResponse)
                    End If
                    x += 1
                    If x = 50 Then Exit For
                Next
            End If

            oWSResponse.Value = lstCalendarResponse.ToArray
            oWSResponse.Status = Convert.ToInt32(resultStatusGlobal)
            oWSResponse.Text = resultStatusGlobal.ToString

            HttpContext.Current.Session("roClientCompanyId") = String.Empty
        Catch ex As Exception
            oWSResponse.Status = Core.DTOs.ReturnCode._UnknownError
            oWSResponse.Text = Core.DTOs.ReturnCode._UnknownError.ToString
        End Try

        Return oWSResponse
    End Function

    <SwaggerWcfTag("Calendar")>
    <SwaggerWcfResponse(HttpStatusCode.OK, "OK")>
    <SwaggerWcfResponse(HttpStatusCode.BadRequest, "Bad request", True)>
    <SwaggerWcfResponse(HttpStatusCode.InternalServerError, "Internal error (can be forced using ERROR_500 as book title)", True)>
    Public Function GetShifts(ByVal CompanyCode As String, ByVal UserName As String, ByVal UserPwd As String) As roWSResponse(Of roShift()) Implements IScheduleService.GetShifts
        Dim oWSResponse As New roWSResponse(Of roShift()) With {
            .Status = Core.DTOs.ReturnCode._UnknownError,
            .Text = String.Empty
        }

        Try
            Dim cCode As String = CompanyCode
            Dim usr As String = UserName
            Dim pwd As String = UserPwd

            HttpContext.Current.Session("roClientCompanyId") = cCode
            Dim externAccessInstance As RoboticsExternAccess = RoboticsExternAccess.GetInstance(True, cCode)

            oWSResponse.ApiVersion = roTypes.Any2String(AdvancedParameter.roAdvancedParameter.GetAdvancedParameterValue("VTWebAPIVersion", Nothing))

            Dim iResultStatus As Core.DTOs.ReturnCode = Core.DTOs.ReturnCode._UnknownError
            Dim iResultText As String = String.Empty
            Dim oShifts As New List(Of roShift)

            If externAccessInstance.ValidateUserNamePassword(usr, pwd, HttpContext.Current.Request.UserHostAddress, iResultStatus, True) Then
                iResultStatus = Core.DTOs.ReturnCode._UnknownError
                externAccessInstance.GetShifts(oShifts, iResultStatus, iResultText)
            Else
                oWSResponse.Status = iResultStatus
            End If

            oWSResponse.Status = iResultStatus
            oWSResponse.Text = iResultStatus.ToString
            oWSResponse.Value = oShifts.ToArray

            HttpContext.Current.Session("roClientCompanyId") = String.Empty
        Catch ex As Exception
            oWSResponse.Status = Core.DTOs.ReturnCode._UnknownError
            oWSResponse.Text = Core.DTOs.ReturnCode._UnknownError.ToString
        End Try

        Return oWSResponse

    End Function

    <SwaggerWcfTag("Calendar")>
    <SwaggerWcfResponse(HttpStatusCode.OK, "OK")>
    <SwaggerWcfResponse(HttpStatusCode.BadRequest, "Bad request", True)>
    <SwaggerWcfResponse(HttpStatusCode.InternalServerError, "Internal error (can be forced using ERROR_500 as book title)", True)>
    Public Function GetPublicHolidays(ByVal CompanyCode As String, ByVal UserName As String, ByVal UserPwd As String) As roWSResponse(Of roPublicHoliday()) Implements IScheduleService.GetPublicHolidays
        Dim oWSResponse As New roWSResponse(Of roPublicHoliday()) With {
            .Status = Core.DTOs.ReturnCode._UnknownError,
            .Text = String.Empty
        }

        Try
            Dim cCode As String = CompanyCode
            Dim usr As String = UserName
            Dim pwd As String = UserPwd

            HttpContext.Current.Session("roClientCompanyId") = cCode
            Dim externAccessInstance As RoboticsExternAccess = RoboticsExternAccess.GetInstance(True, cCode)

            oWSResponse.ApiVersion = roTypes.Any2String(AdvancedParameter.roAdvancedParameter.GetAdvancedParameterValue("VTWebAPIVersion", Nothing))

            Dim iResultStatus As Core.DTOs.ReturnCode = Core.DTOs.ReturnCode._UnknownError
            Dim iResultText As String = String.Empty
            Dim oPublicHolidays As New List(Of roPublicHoliday)

            If externAccessInstance.ValidateUserNamePassword(usr, pwd, HttpContext.Current.Request.UserHostAddress, iResultStatus, True) Then
                iResultStatus = Core.DTOs.ReturnCode._UnknownError
                externAccessInstance.GetPublicHolidays(oPublicHolidays, iResultStatus, iResultText)
            Else
                oWSResponse.Status = iResultStatus
            End If

            oWSResponse.Status = iResultStatus
            oWSResponse.Text = iResultStatus.ToString
            oWSResponse.Value = oPublicHolidays.ToArray

            HttpContext.Current.Session("roClientCompanyId") = String.Empty
        Catch ex As Exception
            oWSResponse.Status = Core.DTOs.ReturnCode._UnknownError
            oWSResponse.Text = Core.DTOs.ReturnCode._UnknownError.ToString
        End Try

        Return oWSResponse

    End Function

    <SwaggerWcfTag("Calendar")>
    <SwaggerWcfResponse(HttpStatusCode.OK, "OK")>
    <SwaggerWcfResponse(HttpStatusCode.BadRequest, "Bad request", True)>
    <SwaggerWcfResponse(HttpStatusCode.InternalServerError, "Internal error", True)>
    Public Function GetCalendar(ByVal CompanyCode As String, ByVal UserName As String, ByVal UserPwd As String, ByVal oCalendarCriteria As roCalendarCriteria) As roWSResponse(Of roCalendar()) Implements IScheduleService.GetCalendar
        Dim oWSResponse As New roWSResponse(Of roCalendar()) With {
            .Status = Core.DTOs.ReturnCode._UnknownError,
            .Text = String.Empty
        }

        Try
            Dim cCode As String = CompanyCode
            Dim usr As String = UserName
            Dim pwd As String = UserPwd

            HttpContext.Current.Session("roClientCompanyId") = cCode
            Dim externAccessInstance As RoboticsExternAccess = RoboticsExternAccess.GetInstance(True, cCode)

            oWSResponse.ApiVersion = roTypes.Any2String(AdvancedParameter.roAdvancedParameter.GetAdvancedParameterValue("VTWebAPIVersion", Nothing))

            Dim resultStatus As Core.DTOs.ReturnCode = Core.DTOs.ReturnCode._UnknownError
            Dim sResultText As String = ""
            Dim oCalendarList As New Generic.List(Of roCalendar)

            If externAccessInstance.ValidateUserNamePassword(usr, pwd, HttpContext.Current.Request.UserHostAddress, resultStatus, True) Then
                resultStatus = Core.DTOs.ReturnCode._UnknownError
                externAccessInstance.GetCalendar(oCalendarCriteria, oCalendarList, resultStatus, sResultText)
            End If

            oWSResponse.Value = oCalendarList.ToArray
            oWSResponse.Status = Convert.ToInt32(resultStatus)
            oWSResponse.Text = resultStatus.ToString

            HttpContext.Current.Session("roClientCompanyId") = String.Empty
        Catch ex As Exception
            oWSResponse.Status = Core.DTOs.ReturnCode._UnknownError
            oWSResponse.Text = Core.DTOs.ReturnCode._UnknownError.ToString
        End Try

        Return oWSResponse
    End Function

End Class