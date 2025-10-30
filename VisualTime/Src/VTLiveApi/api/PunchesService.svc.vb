Imports System.Net
Imports System.ServiceModel.Activation
Imports Robotics.Base
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.ExternalSystems.DataLink.RoboticsExternAccess
Imports Robotics.VTBase
Imports SwaggerWcf.Attributes

<AspNetCompatibilityRequirements(RequirementsMode:=AspNetCompatibilityRequirementsMode.Allowed)>
Public Class PunchesService
    Implements IPunchesService

    <SwaggerWcfTag("Punches")>
    <SwaggerWcfResponse(HttpStatusCode.OK, "OK")>
    <SwaggerWcfResponse(HttpStatusCode.BadRequest, "Bad request", True)>
    <SwaggerWcfResponse(HttpStatusCode.InternalServerError, "Internal error (can be forced using ERROR_500 as book title)", True)>
    Public Function GetPunches(ByVal CompanyCode As String, ByVal UserName As String, ByVal UserPwd As String, ByVal Timestamp As DateTime) As roWSResponse(Of roPunch()) Implements IPunchesService.GetPunches
        Dim oWSResponse As New roWSResponse(Of roPunch()) With {
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
            Dim oPunches As New List(Of roPunch)
            Dim oDatalinkStandardPunchResponse As New roDatalinkStandardPunchResponse

            If externAccessInstance.ValidateUserNamePassword(usr, pwd, HttpContext.Current.Request.UserHostAddress, iResultStatus, True) Then
                iResultStatus = Core.DTOs.ReturnCode._UnknownError
                oDatalinkStandardPunchResponse.ResultCode = Core.DTOs.ReturnCode._UnknownError
                externAccessInstance.GetPunchesEx(DTOs.PunchFilterType.ByTimeStamp, Timestamp, Timestamp.Date, Timestamp.Date, oDatalinkStandardPunchResponse)
            Else
                oDatalinkStandardPunchResponse.ResultCode = iResultStatus
            End If

            If oDatalinkStandardPunchResponse IsNot Nothing Then
                If oDatalinkStandardPunchResponse.Punches IsNot Nothing Then oWSResponse.Value = oDatalinkStandardPunchResponse.Punches.ConvertAll(AddressOf XStandardPunchToPunchConverter).ToArray
                'If oDatalinkStandardPunchResponse.PunchesListError IsNot Nothing Then iResponse.ErrorValue = oDatalinkStandardPunchResponse.PunchesListError.ConvertAll(AddressOf XPunchConverter).ToArray
            End If

            oWSResponse.Status = oDatalinkStandardPunchResponse.ResultCode
            oWSResponse.Text = iResultText

            HttpContext.Current.Session("roClientCompanyId") = String.Empty
        Catch ex As Exception
            oWSResponse.Status = Core.DTOs.ReturnCode._UnknownError
        End Try

        Return oWSResponse

    End Function

    <SwaggerWcfTag("Punches")>
    <SwaggerWcfResponse(HttpStatusCode.OK, "OK")>
    <SwaggerWcfResponse(HttpStatusCode.BadRequest, "Bad request", True)>
    <SwaggerWcfResponse(HttpStatusCode.InternalServerError,
"Internal error (can be forced using ERROR_500 as book title)", True)>
    Public Function GetPunchesBetweenDates(ByVal CompanyCode As String, ByVal UserName As String, ByVal UserPwd As String, ByVal StartDate As Date, ByVal EndDate As Date) As roWSResponse(Of roPunch()) Implements IPunchesService.GetPunchesBetweenDates
        Dim oWSResponse As New roWSResponse(Of roPunch()) With {
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
            Dim oPunches As New List(Of roPunch)
            Dim oDatalinkStandardPunchResponse As New roDatalinkStandardPunchResponse

            If externAccessInstance.ValidateUserNamePassword(usr, pwd, HttpContext.Current.Request.UserHostAddress, iResultStatus, True) Then
                iResultStatus = Core.DTOs.ReturnCode._UnknownError
                oDatalinkStandardPunchResponse.ResultCode = Core.DTOs.ReturnCode._UnknownError
                externAccessInstance.GetPunches(DTOs.PunchFilterType.ByDatePeriod, StartDate, StartDate, EndDate, oDatalinkStandardPunchResponse)
            Else
                oDatalinkStandardPunchResponse.ResultCode = iResultStatus
            End If

            If oDatalinkStandardPunchResponse IsNot Nothing Then
                If oDatalinkStandardPunchResponse.Punches IsNot Nothing Then oWSResponse.Value = oDatalinkStandardPunchResponse.Punches.ConvertAll(AddressOf XStandardPunchToPunchConverter).ToArray
                'If oDatalinkStandardPunchResponse.PunchesListError IsNot Nothing Then iResponse.ErrorValue = oDatalinkStandardPunchResponse.PunchesListError.ConvertAll(AddressOf XPunchConverter).ToArray
            End If

            oWSResponse.Status = oDatalinkStandardPunchResponse.ResultCode
            oWSResponse.Text = iResultText

            HttpContext.Current.Session("roClientCompanyId") = String.Empty
        Catch ex As Exception
            oWSResponse.Status = Core.DTOs.ReturnCode._UnknownError
        End Try

        Return oWSResponse

    End Function

    <SwaggerWcfTag("Punches")>
    <SwaggerWcfResponse(HttpStatusCode.OK, "OK")>
    <SwaggerWcfResponse(HttpStatusCode.BadRequest, "Bad request", True)>
    <SwaggerWcfResponse(HttpStatusCode.InternalServerError, "Internal error", True)>
    Public Function AddPunches(ByVal UserName As String, ByVal UserPwd As String, ByVal lPunches As roPunch(), Optional ByVal CompanyCode As String = "") As roWSResponse(Of roPunchesResponse()) Implements IPunchesService.AddPunches
        Dim oWSResponse As New roWSResponse(Of roPunchesResponse()) With {
            .Status = Core.DTOs.ReturnCode._UnknownError,
            .Text = String.Empty
        }

        Try
            ' Tratamiento de opcionales
            If CompanyCode Is Nothing Then CompanyCode = String.Empty

            HttpContext.Current.Session("roClientCompanyId") = CompanyCode
            Dim externAccessInstance As RoboticsExternAccess = RoboticsExternAccess.GetInstance(True, CompanyCode)

            oWSResponse.ApiVersion = roTypes.Any2String(AdvancedParameter.roAdvancedParameter.GetAdvancedParameterValue("VTWebAPIVersion", Nothing))

            Dim resultStatus As Core.DTOs.ReturnCode = Core.DTOs.ReturnCode._UnknownError
            Dim lPunchesDatalink As New List(Of roDatalinkStandardPunch)
            Dim resultStatusGlobal As Core.DTOs.ReturnCode = Core.DTOs.ReturnCode._OK
            Dim lstStandardPunchesResponse As New roDatalinkStandardPunchResponse
            Dim lstPunchesResponse As New List(Of roPunchesResponse)

            If externAccessInstance.ValidateUserNamePassword(UserName, UserPwd, HttpContext.Current.Request.UserHostAddress, resultStatusGlobal, True) Then
                resultStatus = Core.DTOs.ReturnCode._UnknownError
                lPunchesDatalink = lPunches.ToList.ConvertAll(AddressOf XPunchToStandardPunchConverter)
                externAccessInstance.AddPunches(lPunchesDatalink, lstStandardPunchesResponse)
            End If

            oWSResponse.Value = lstStandardPunchesResponse.PunchesListError.ConvertAll(AddressOf XStandardPunchToPunchResponseConverter).ToArray
            oWSResponse.Status = Convert.ToInt32(resultStatusGlobal)
            oWSResponse.Text = resultStatusGlobal.ToString

            HttpContext.Current.Session("roClientCompanyId") = String.Empty
        Catch ex As Exception
            oWSResponse.Status = Core.DTOs.ReturnCode._UnknownError
            oWSResponse.Text = Core.DTOs.ReturnCode._UnknownError.ToString
        End Try

        Return oWSResponse
    End Function

    Private Function XStandardPunchToPunchConverter(oPunch As roDatalinkStandardPunch) As roPunch
        Dim oRet As roPunch
        Try
            oRet = New roPunch
            oRet.IDEmployee = oPunch.UniqueEmployeeID
            oRet.Type = oPunch.Type
            oRet.ActualType = oPunch.ActualType
            oRet.DateTime = New Robotics.VTBase.roWCFDate(oPunch.DateTime)
            oRet.IDTerminal = oPunch.IDTerminal
            oRet.TypeData = oPunch.TypeData
            oRet.GPS = oPunch.GPS
            If Not oPunch.Field1 Is Nothing Then oRet.Field1 = oPunch.Field1
            If Not oPunch.Field2 Is Nothing Then oRet.Field2 = oPunch.Field2
            If Not oPunch.Field3 Is Nothing Then oRet.Field3 = oPunch.Field3
            If Not oPunch.Field4 Is Nothing Then oRet.Field4 = oPunch.Field4
            If Not oPunch.Field5 Is Nothing Then oRet.Field5 = oPunch.Field5
            If Not oPunch.Field6 Is Nothing Then oRet.Field6 = oPunch.Field6
            oRet.Timestamp = New Robotics.VTBase.roWCFDate(oPunch.Timestamp)
            oRet.ResultCode = oPunch.ResultCode
            oRet.ResultDescription = oPunch.ResultDescription
        Catch ex As Exception
            oRet = Nothing
        End Try

        Return oRet
    End Function

    Private Function XPunchToStandardPunchConverter(oPunch As roPunch) As roDatalinkStandardPunch
        Dim oRet As roDatalinkStandardPunch
        Try
            oRet = New roDatalinkStandardPunch
            oRet.Type = oPunch.Type
            oRet.ActualType = oPunch.ActualType
            oRet.DateTime = oPunch.DateTime.Data
            oRet.Field1 = oPunch.Field1
            oRet.Field2 = oPunch.Field2
            oRet.Field3 = oPunch.Field3
            oRet.Field4 = oPunch.Field4
            oRet.Field5 = oPunch.Field5
            oRet.Field6 = oPunch.Field6
            oRet.GPS = oPunch.GPS
            oRet.UniqueEmployeeID = oPunch.IDEmployee
            oRet.IDTerminal = oPunch.IDTerminal
            If Not oPunch.Timestamp Is Nothing Then oRet.Timestamp = oPunch.Timestamp.Data
            oRet.TypeData = oPunch.TypeData
        Catch ex As Exception
            oRet = Nothing
        End Try

        Return oRet
    End Function

    Private Function XStandardPunchToPunchResponseConverter(oPunch As roDatalinkStandardPunch) As roPunchesResponse
        Dim oRet As roPunchesResponse
        Try
            oRet = New roPunchesResponse
            oRet.oPunch = XStandardPunchToPunchConverter(oPunch)
            oRet.Status = oPunch.ResultCode
            oRet.Text = oPunch.ResultDescription
        Catch ex As Exception
            oRet = Nothing
        End Try

        Return oRet
    End Function

End Class