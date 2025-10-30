Imports System.Net
Imports System.ServiceModel.Activation
Imports Robotics.Base
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.ExternalSystems.DataLink.RoboticsExternAccess
Imports Robotics.Security.Base
Imports Robotics.VTBase
Imports SwaggerWcf.Attributes

<AspNetCompatibilityRequirements(RequirementsMode:=AspNetCompatibilityRequirementsMode.Allowed)>
<SwaggerWcf("/api/v2/ScheduleService.svc")>
<CustomErrorBehavior>
Public Class ScheduleService_v2
    Implements IScheduleService_v2

    <SwaggerWcfTag("Calendar")>
    <SwaggerWcfResponse(HttpStatusCode.OK, "OK")>
    <SwaggerWcfResponse(HttpStatusCode.BadRequest, "Bad request", True)>
    <SwaggerWcfResponse(HttpStatusCode.InternalServerError, "Internal error (can be forced using ERROR_500 as book title)", True)>
    Public Function GetHolidays(ByVal Token As String, ByVal StartDate As Date, ByVal EndDate As Date, Optional ByVal EmployeeID As String = "", Optional Criteria As String = "", Optional ByVal CompanyCode As String = "") As roWSResponse(Of roHoliday()) Implements IScheduleService_v2.GetHolidays
        Dim oWSResponse As New roWSResponse(Of roHoliday()) With {
            .Status = Core.DTOs.ReturnCode._UnknownError,
            .Text = String.Empty
        }

        Try
            Dim iResultStatus As Core.DTOs.ReturnCode = Core.DTOs.ReturnCode._UnknownError

            If Token Is Nothing Then Token = String.Empty
            If CompanyCode Is Nothing OrElse CompanyCode = String.Empty Then
                If Token.Length > 88 Then
                    CompanyCode = RoboticsExternAccess.GetCompanyFromToken(Token, iResultStatus)
                Else
                    CompanyCode = String.Empty
                End If
            End If

            If EmployeeID Is Nothing Then EmployeeID = String.Empty

            HttpContext.Current.Session("roClientCompanyId") = CompanyCode
            Dim externAccessInstance As RoboticsExternAccess = RoboticsExternAccess.GetInstance(True, CompanyCode)

            oWSResponse.ApiVersion = roTypes.Any2String(AdvancedParameter.roAdvancedParameter.GetAdvancedParameterValue("VTWebAPIVersion", Nothing))

            Dim iResultText As String = String.Empty
            Dim oHolidays As New List(Of roHoliday)

            If iResultStatus <> Core.DTOs.ReturnCode._TokenValidation AndAlso externAccessInstance.ValidateToken(Token, HttpContext.Current.Request.UserHostAddress, iResultStatus, True) Then
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
    Public Function CreateOrUpdateHolidays(ByVal Token As String, ByVal oHolidays As roHoliday, Optional ByVal CompanyCode As String = "") As roWSResponse(Of roHoliday()) Implements IScheduleService_v2.CreateOrUpdateHolidays
        Dim oWSResponse As New roWSResponse(Of roHoliday()) With {
            .Status = Core.DTOs.ReturnCode._UnknownError,
            .Text = String.Empty
        }

        Try
            Dim resultStatus As Core.DTOs.ReturnCode = Core.DTOs.ReturnCode._UnknownError
            If Token Is Nothing Then Token = String.Empty
            If CompanyCode Is Nothing OrElse CompanyCode = String.Empty Then
                If Token.Length > 88 Then
                    CompanyCode = RoboticsExternAccess.GetCompanyFromToken(Token, resultStatus)
                Else
                    CompanyCode = String.Empty
                End If
            End If

            HttpContext.Current.Session("roClientCompanyId") = CompanyCode
            Dim externAccessInstance As RoboticsExternAccess = RoboticsExternAccess.GetInstance(True, CompanyCode)

            oWSResponse.ApiVersion = roTypes.Any2String(AdvancedParameter.roAdvancedParameter.GetAdvancedParameterValue("VTWebAPIVersion", Nothing))

            Dim sResultText As String = ""
            Dim oHolidaysDatalink As New roDatalinkStandarHolidays

            If resultStatus <> Core.DTOs.ReturnCode._TokenValidation AndAlso externAccessInstance.ValidateToken(Token, HttpContext.Current.Request.UserHostAddress, resultStatus, True) Then
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
    Public Function CreateOrUpdateCalendar(ByVal Token As String, ByVal oCalendar As roCalendar, Optional ByVal CompanyCode As String = "") As roWSResponse(Of roCalendar()) Implements IScheduleService_v2.CreateOrUpdateCalendar
        Dim oWSResponse As New roWSResponse(Of roCalendar()) With {
            .Status = Core.DTOs.ReturnCode._UnknownError,
            .Text = String.Empty
        }

        Try
            Dim resultStatus As Core.DTOs.ReturnCode = Core.DTOs.ReturnCode._UnknownError
            If Token Is Nothing Then Token = String.Empty
            If CompanyCode Is Nothing OrElse CompanyCode = String.Empty Then
                If Token.Length > 88 Then
                    CompanyCode = RoboticsExternAccess.GetCompanyFromToken(Token, resultStatus)
                Else
                    CompanyCode = String.Empty
                End If
            End If

            HttpContext.Current.Session("roClientCompanyId") = CompanyCode
            Dim externAccessInstance As RoboticsExternAccess = RoboticsExternAccess.GetInstance(True, CompanyCode)

            oWSResponse.ApiVersion = roTypes.Any2String(AdvancedParameter.roAdvancedParameter.GetAdvancedParameterValue("VTWebAPIVersion", Nothing))

            Dim sResultText As String = ""
            Dim oCalendarDatalink As New roDatalinkStandarCalendar

            If resultStatus <> Core.DTOs.ReturnCode._TokenValidation AndAlso externAccessInstance.ValidateToken(Token, HttpContext.Current.Request.UserHostAddress, resultStatus, True) Then
                resultStatus = Core.DTOs.ReturnCode._UnknownError
                oCalendarDatalink = New roDatalinkStandarCalendar
                oCalendarDatalink.UniqueEmployeeID = oCalendar.IDEmployee
                oCalendarDatalink.ShiftKey = oCalendar.IDShift
                oCalendarDatalink.PlanDate = oCalendar.PlannedDate.GetDate().Date
                oCalendarDatalink.ShiftLayerDefinition = oCalendar.ShiftLayerDefinition
                oCalendarDatalink.CanTelecommute = oCalendar.CanTelecommute
                oCalendarDatalink.TelecommuteForced = oCalendar.TelecommuteForced
                oCalendarDatalink.TelecommutingStatus = oCalendar.TelecommutingStatus

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
    Public Function CreateOrUpdateCalendarBatch(ByVal Token As String, ByVal oCalendars As roCalendar(), Optional ByVal CompanyCode As String = "") As roWSResponse(Of roCalendarResponse()) Implements IScheduleService_v2.CreateOrUpdateCalendarBatch
        Dim oWSResponse As New roWSResponse(Of roCalendarResponse()) With {
            .Status = Core.DTOs.ReturnCode._UnknownError,
            .Text = String.Empty
        }

        Try
            Dim resultStatusGlobal As Core.DTOs.ReturnCode = Core.DTOs.ReturnCode._OK
            If Token Is Nothing Then Token = String.Empty
            If CompanyCode Is Nothing OrElse CompanyCode = String.Empty Then
                If Token.Length > 88 Then
                    CompanyCode = RoboticsExternAccess.GetCompanyFromToken(Token, resultStatusGlobal)
                Else
                    CompanyCode = String.Empty
                End If
            End If

            HttpContext.Current.Session("roClientCompanyId") = CompanyCode
            Dim externAccessInstance As RoboticsExternAccess = RoboticsExternAccess.GetInstance(True, CompanyCode)

            oWSResponse.ApiVersion = roTypes.Any2String(AdvancedParameter.roAdvancedParameter.GetAdvancedParameterValue("VTWebAPIVersion", Nothing))

            Dim sResultText As String = ""
            Dim oCalendarDatalink As New roDatalinkStandarCalendar
            Dim resultStatus As Core.DTOs.ReturnCode = Core.DTOs.ReturnCode._UnknownError
            Dim lstCalendarResponse As New Generic.List(Of roCalendarResponse)
            Dim x As Integer = 0

            If resultStatusGlobal <> Core.DTOs.ReturnCode._TokenValidation AndAlso externAccessInstance.ValidateToken(Token, HttpContext.Current.Request.UserHostAddress, resultStatusGlobal, True) Then
                For Each oCalendar As roCalendar In oCalendars
                    resultStatus = Core.DTOs.ReturnCode._UnknownError
                    oCalendarDatalink = New roDatalinkStandarCalendar
                    oCalendarDatalink.UniqueEmployeeID = oCalendar.IDEmployee
                    oCalendarDatalink.ShiftKey = oCalendar.IDShift
                    oCalendarDatalink.PlanDate = oCalendar.PlannedDate.GetDate().Date
                    oCalendarDatalink.ShiftLayerDefinition = oCalendar.ShiftLayerDefinition
                    oCalendarDatalink.CanTelecommute = oCalendar.CanTelecommute
                    oCalendarDatalink.TelecommuteForced = oCalendar.TelecommuteForced
                    oCalendarDatalink.TelecommutingStatus = oCalendar.TelecommutingStatus

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
    <SwaggerWcfResponse(HttpStatusCode.InternalServerError,
"Internal error", True)>
    Public Function UpdateLockDate(ByVal Token As String, ByVal LockDate As Date, Optional ByVal CompanyCode As String = "") As roWSResponse(Of roLockDateResponse) Implements IScheduleService_v2.UpdateLockDate
        Dim oWSResponse As New roWSResponse(Of roLockDateResponse) With {
            .Status = Core.DTOs.ReturnCode._UnknownError,
            .Text = String.Empty
        }

        Try
            Dim resultStatus As Core.DTOs.ReturnCode = Core.DTOs.ReturnCode._UnknownError
            If Token Is Nothing Then Token = String.Empty
            If CompanyCode Is Nothing OrElse CompanyCode = String.Empty Then
                If Token.Length > 88 Then
                    CompanyCode = RoboticsExternAccess.GetCompanyFromToken(Token, resultStatus)
                Else
                    CompanyCode = String.Empty
                End If
            End If

            HttpContext.Current.Session("roClientCompanyId") = CompanyCode
            Dim externAccessInstance As RoboticsExternAccess = RoboticsExternAccess.GetInstance(True, CompanyCode)

            oWSResponse.ApiVersion = roTypes.Any2String(AdvancedParameter.roAdvancedParameter.GetAdvancedParameterValue("VTWebAPIVersion", Nothing))

            Dim sResultText As String = ""
            Dim oLockDateResponse As New roLockDateResponse

            If resultStatus <> Core.DTOs.ReturnCode._TokenValidation AndAlso externAccessInstance.ValidateToken(Token, HttpContext.Current.Request.UserHostAddress, resultStatus, True) Then
                resultStatus = Core.DTOs.ReturnCode._UnknownError
                Dim oLogState As New roBusinessState("Common.BaseState", "")
                Dim bolRet As Boolean = roBusinessSupport.SaveLockDate(LockDate, oLogState, True)
                If bolRet Then
                    resultStatus = Core.DTOs.ReturnCode._OK
                    oLockDateResponse.LockDate = New roWCFDate(LockDate)
                End If
            End If

            oWSResponse.Value = oLockDateResponse
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
    <SwaggerWcfResponse(HttpStatusCode.InternalServerError, "Internal error (can be forced using ERROR_500 as book title)", True)>
    Public Function GetShifts(ByVal Token As String, Optional ByVal CompanyCode As String = "", Optional ByVal ShiftID As String = "") As roWSResponse(Of roShift()) Implements IScheduleService_v2.GetShifts
        Dim oWSResponse As New roWSResponse(Of roShift()) With {
            .Status = Core.DTOs.ReturnCode._UnknownError,
            .Text = String.Empty
        }

        Try
            Dim iResultStatus As Core.DTOs.ReturnCode = Core.DTOs.ReturnCode._UnknownError
            If Token Is Nothing Then Token = String.Empty
            If CompanyCode Is Nothing OrElse CompanyCode = String.Empty Then
                If Token.Length > 88 Then
                    CompanyCode = RoboticsExternAccess.GetCompanyFromToken(Token, iResultStatus)
                Else
                    CompanyCode = String.Empty
                End If
            End If

            If ShiftID Is Nothing Then ShiftID = String.Empty

            HttpContext.Current.Session("roClientCompanyId") = CompanyCode
            Dim externAccessInstance As RoboticsExternAccess = RoboticsExternAccess.GetInstance(True, CompanyCode)

            oWSResponse.ApiVersion = roTypes.Any2String(AdvancedParameter.roAdvancedParameter.GetAdvancedParameterValue("VTWebAPIVersion", Nothing))

            Dim iResultText As String = String.Empty
            Dim oShifts As New List(Of roShift)

            If iResultStatus <> Core.DTOs.ReturnCode._TokenValidation AndAlso externAccessInstance.ValidateToken(Token, HttpContext.Current.Request.UserHostAddress, iResultStatus, True) Then
                iResultStatus = Core.DTOs.ReturnCode._UnknownError
                externAccessInstance.GetShifts(oShifts, iResultStatus, iResultText, ShiftID)
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
    Public Function GetPublicHolidays(ByVal Token As String, Optional ByVal CompanyCode As String = "") As roWSResponse(Of roPublicHoliday()) Implements IScheduleService_v2.GetPublicHolidays
        Dim oWSResponse As New roWSResponse(Of roPublicHoliday()) With {
            .Status = Core.DTOs.ReturnCode._UnknownError,
            .Text = String.Empty
        }

        Try
            Dim iResultStatus As Core.DTOs.ReturnCode = Core.DTOs.ReturnCode._UnknownError
            If Token Is Nothing Then Token = String.Empty
            If CompanyCode Is Nothing OrElse CompanyCode = String.Empty Then
                If Token.Length > 88 Then
                    CompanyCode = RoboticsExternAccess.GetCompanyFromToken(Token, iResultStatus)
                Else
                    CompanyCode = String.Empty
                End If
            End If

            HttpContext.Current.Session("roClientCompanyId") = CompanyCode
            Dim externAccessInstance As RoboticsExternAccess = RoboticsExternAccess.GetInstance(True, CompanyCode)

            oWSResponse.ApiVersion = roTypes.Any2String(AdvancedParameter.roAdvancedParameter.GetAdvancedParameterValue("VTWebAPIVersion", Nothing))

            Dim iResultText As String = String.Empty
            Dim oPublicHolidays As New List(Of roPublicHoliday)

            If iResultStatus <> Core.DTOs.ReturnCode._TokenValidation AndAlso externAccessInstance.ValidateToken(Token, HttpContext.Current.Request.UserHostAddress, iResultStatus, True) Then
                iResultStatus = Core.DTOs.ReturnCode._UnknownError
                externAccessInstance.GetPublicHolidays(oPublicHolidays, iResultStatus, iResultText)
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
    Public Function GetCalendar(ByVal Token As String, ByVal oCalendarCriteria As roCalendarCriteria, Optional ByVal CompanyCode As String = "", Optional ByVal LoadScheduledLayers As Boolean = False) As roWSResponse(Of roCalendar()) Implements IScheduleService_v2.GetCalendar
        Dim oWSResponse As New roWSResponse(Of roCalendar()) With {
            .Status = Core.DTOs.ReturnCode._UnknownError,
            .Text = String.Empty
        }

        Try
            Dim resultStatus As Core.DTOs.ReturnCode = Core.DTOs.ReturnCode._UnknownError
            If Token Is Nothing Then Token = String.Empty
            If CompanyCode Is Nothing OrElse CompanyCode = String.Empty Then
                If Token.Length > 88 Then
                    CompanyCode = RoboticsExternAccess.GetCompanyFromToken(Token, resultStatus)
                Else
                    CompanyCode = String.Empty
                End If
            End If

            HttpContext.Current.Session("roClientCompanyId") = CompanyCode
            Dim externAccessInstance As RoboticsExternAccess = RoboticsExternAccess.GetInstance(True, CompanyCode)

            oWSResponse.ApiVersion = roTypes.Any2String(AdvancedParameter.roAdvancedParameter.GetAdvancedParameterValue("VTWebAPIVersion", Nothing))

            Dim sResultText As String = ""
            Dim oCalendarList As New Generic.List(Of roCalendar)

            If resultStatus <> Core.DTOs.ReturnCode._TokenValidation AndAlso externAccessInstance.ValidateToken(Token, HttpContext.Current.Request.UserHostAddress, resultStatus, True) Then
                resultStatus = Core.DTOs.ReturnCode._UnknownError
                externAccessInstance.GetCalendar(oCalendarCriteria, oCalendarList, resultStatus, sResultText, , LoadScheduledLayers)
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

    <SwaggerWcfTag("Calendar")>
    <SwaggerWcfResponse(HttpStatusCode.OK, "OK")>
    <SwaggerWcfResponse(HttpStatusCode.BadRequest, "Bad request", True)>
    <SwaggerWcfResponse(HttpStatusCode.InternalServerError, "Internal error", True)>
    Public Function GetCalendarByTimestamp(ByVal Token As String, ByVal oCalendarCriteria As roCalendarCriteria, ByVal Timestamp As DateTime, Optional ByVal CompanyCode As String = "", Optional ByVal LoadScheduledLayers As Boolean = False) As roWSResponse(Of roCalendar()) Implements IScheduleService_v2.GetCalendarByTimestamp
        Dim oWSResponse As New roWSResponse(Of roCalendar()) With {
            .Status = Core.DTOs.ReturnCode._UnknownError,
            .Text = String.Empty
        }

        Try
            Dim resultStatus As Core.DTOs.ReturnCode = Core.DTOs.ReturnCode._UnknownError
            If Token Is Nothing Then Token = String.Empty
            If CompanyCode Is Nothing OrElse CompanyCode = String.Empty Then
                If Token.Length > 88 Then
                    CompanyCode = RoboticsExternAccess.GetCompanyFromToken(Token, resultStatus)
                Else
                    CompanyCode = String.Empty
                End If
            End If

            HttpContext.Current.Session("roClientCompanyId") = CompanyCode
            Dim externAccessInstance As RoboticsExternAccess = RoboticsExternAccess.GetInstance(True, CompanyCode)

            oWSResponse.ApiVersion = roTypes.Any2String(AdvancedParameter.roAdvancedParameter.GetAdvancedParameterValue("VTWebAPIVersion", Nothing))

            Dim sResultText As String = ""
            Dim oCalendarList As New Generic.List(Of roCalendar)

            If resultStatus <> Core.DTOs.ReturnCode._TokenValidation AndAlso externAccessInstance.ValidateToken(Token, HttpContext.Current.Request.UserHostAddress, resultStatus, True) Then
                resultStatus = Core.DTOs.ReturnCode._UnknownError
                externAccessInstance.GetCalendar(oCalendarCriteria, oCalendarList, resultStatus, sResultText, Timestamp, LoadScheduledLayers)
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

    <SwaggerWcfTag("Calendar")>
    <SwaggerWcfResponse(HttpStatusCode.OK, "OK")>
    <SwaggerWcfResponse(HttpStatusCode.BadRequest, "Bad request", True)>
    <SwaggerWcfResponse(HttpStatusCode.InternalServerError, "Internal error", True)>
    Public Function GetLockDate(ByVal Token As String, Optional ByVal CompanyCode As String = "") As roWSResponse(Of roLockDateResponse) Implements IScheduleService_v2.GetLockDate
        Dim oWSResponse As New roWSResponse(Of roLockDateResponse) With {
            .Status = Core.DTOs.ReturnCode._UnknownError,
            .Text = String.Empty
        }

        Try
            Dim iResultStatus As Core.DTOs.ReturnCode = Core.DTOs.ReturnCode._UnknownError
            Dim oRet As Date = New Date(1900, 1, 1)

            If Token Is Nothing Then Token = String.Empty
            If CompanyCode Is Nothing OrElse CompanyCode = String.Empty Then
                If Token.Length > 88 Then
                    CompanyCode = RoboticsExternAccess.GetCompanyFromToken(Token, iResultStatus)
                Else
                    CompanyCode = String.Empty
                End If
            End If

            HttpContext.Current.Session("roClientCompanyId") = CompanyCode
            Dim externAccessInstance As RoboticsExternAccess = RoboticsExternAccess.GetInstance(True, CompanyCode)

            oWSResponse.ApiVersion = roTypes.Any2String(AdvancedParameter.roAdvancedParameter.GetAdvancedParameterValue("VTWebAPIVersion", Nothing))

            Dim iResultText As String = String.Empty

            Dim oLockDateResponse As New roLockDateResponse

            If iResultStatus <> Core.DTOs.ReturnCode._TokenValidation AndAlso externAccessInstance.ValidateToken(Token, HttpContext.Current.Request.UserHostAddress, iResultStatus, True) Then
                iResultStatus = Core.DTOs.ReturnCode._UnknownError

                Dim wsRet As roGenericVtResponse(Of Object) = VTLiveApi.ConnectorMethods.GetParameter(DTOs.Parameters.FirstDate, New DTOs.roWsState)

                If IsDate(wsRet.Value) Then
                    oRet = CDate(wsRet.Value)
                End If

                oLockDateResponse.LockDate = New roWCFDate(oRet)
                iResultStatus = Core.DTOs.ReturnCode._OK
            End If
            oWSResponse.Value = oLockDateResponse
            oWSResponse.Status = iResultStatus
            oWSResponse.Text = iResultStatus.ToString

            HttpContext.Current.Session("roClientCompanyId") = String.Empty
        Catch ex As Exception
            oWSResponse.Status = Core.DTOs.ReturnCode._UnknownError
            oWSResponse.Text = Core.DTOs.ReturnCode._UnknownError.ToString
        End Try

        Return oWSResponse
    End Function

End Class