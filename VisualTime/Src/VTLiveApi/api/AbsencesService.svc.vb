Imports System.Net
Imports System.ServiceModel.Activation
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.ExternalSystems.DataLink.RoboticsExternAccess
Imports Robotics.VTBase
Imports SwaggerWcf.Attributes

<AspNetCompatibilityRequirements(RequirementsMode:=AspNetCompatibilityRequirementsMode.Allowed)>
Public Class AbsencesService
    Implements IAbsencesService

    <SwaggerWcfTag("Absences")>
    <SwaggerWcfResponse(HttpStatusCode.OK, "OK")>
    <SwaggerWcfResponse(HttpStatusCode.BadRequest, "Bad request", True)>
    <SwaggerWcfResponse(HttpStatusCode.InternalServerError, "Internal Server Error", True)>
    Public Function GetAbsences(ByVal UserName As String, ByVal UserPwd As String, StartDate As Date, EndDate As Date, Optional ByVal Employee As String = "", Optional Criteria As String = "", Optional ByVal CompanyCode As String = "") As roWSResponse(Of roAbsence()) Implements IAbsencesService.GetAbsences
        Dim oWSResponse As roWSResponse(Of roAbsence()) = New roWSResponse(Of roAbsence())
        Dim oAbsences As New List(Of roAbsence)
        Try

            ' Tratamiento de opcionales
            If Employee Is Nothing Then Employee = String.Empty
            If Criteria Is Nothing Then Criteria = String.Empty
            If CompanyCode Is Nothing Then CompanyCode = String.Empty

            HttpContext.Current.Session("roClientCompanyId") = CompanyCode
            Dim externAccessInstance As RoboticsExternAccess = RoboticsExternAccess.GetInstance(True, CompanyCode)

            oWSResponse.ApiVersion = roTypes.Any2String(AdvancedParameter.roAdvancedParameter.GetAdvancedParameterValue("VTWebAPIVersion", Nothing))

            Dim resultStatus As Core.DTOs.ReturnCode = Core.DTOs.ReturnCode._UnknownError
            Dim sResultText As String = ""
            Dim oAbsencesResponse As New roDatalinkStandarAbsenceResponse

            If externAccessInstance.ValidateUserNamePassword(UserName, UserPwd, HttpContext.Current.Request.UserHostAddress, resultStatus, True) Then
                resultStatus = Core.DTOs.ReturnCode._UnknownError
                Dim oCriteria As New roDatalinkStandarAbsenceCriteria
                oAbsencesResponse = New roDatalinkStandarAbsenceResponse
                oCriteria.EndAbsencePeriod = EndDate
                oCriteria.StartAbsencePeriod = StartDate
                oCriteria.UniqueEmployeeID = Employee
                oCriteria.ShowChangesInPeriod = (Criteria.ToUpper = "TIMESTAMP")
                externAccessInstance.GetAbsences(oCriteria, oAbsencesResponse)
            End If

            'TODO: No todo va a ir siempre bien, no?
            oWSResponse.Value = oAbsencesResponse.Absences.ConvertAll(AddressOf XStandardAbsenceToAbsenceConverter).ToArray
            oWSResponse.Status = Convert.ToInt32(oAbsencesResponse.ResultCode)
            oWSResponse.Text = sResultText

            HttpContext.Current.Session("roClientCompanyId") = ""
        Catch ex As Exception
            oWSResponse.Status = Core.DTOs.ReturnCode._UnknownError
        End Try

        Return oWSResponse
    End Function

    <SwaggerWcfTag("Absences")>
    <SwaggerWcfResponse(HttpStatusCode.OK, "OK")>
    <SwaggerWcfResponse(HttpStatusCode.BadRequest, "Bad request", True)>
    <SwaggerWcfResponse(HttpStatusCode.InternalServerError, "Internal error", True)>
    Public Function CreateOrUpdateAbsence(ByVal UserName As String, ByVal UserPwd As String, ByVal oAbsence As roAbsence, Optional ByVal CompanyCode As String = "") As roWSResponse(Of roAbsence()) Implements IAbsencesService.CreateOrUpdateAbsence
        Dim oWSResponse As New roWSResponse(Of roAbsence()) With {
            .Status = Core.DTOs.ReturnCode._UnknownError,
            .Text = String.Empty
        }

        Try
            ' Tratamiento de parámetros opcionales
            If CompanyCode Is Nothing Then CompanyCode = String.Empty

            HttpContext.Current.Session("roClientCompanyId") = CompanyCode
            Dim externAccessInstance As RoboticsExternAccess = RoboticsExternAccess.GetInstance(True, CompanyCode)

            oWSResponse.ApiVersion = roTypes.Any2String(AdvancedParameter.roAdvancedParameter.GetAdvancedParameterValue("VTWebAPIVersion", Nothing))

            Dim iResult As Core.DTOs.ReturnCode = Core.DTOs.ReturnCode._UnknownError

            If externAccessInstance.ValidateUserNamePassword(UserName, UserPwd, HttpContext.Current.Request.UserHostAddress, iResult, True) Then
                Dim oStandardAbsence As New roDatalinkStandarAbsence
                oStandardAbsence = XAbsenceToStandardAbsenceConverter(oAbsence)
                iResult = Core.DTOs.ReturnCode._UnknownError
                externAccessInstance.CreateOrUpdateAbsence(oStandardAbsence, iResult)
            End If

            oWSResponse.Status = iResult
            oWSResponse.Text = iResult.ToString

            HttpContext.Current.Session("roClientCompanyId") = String.Empty
        Catch ex As Exception
            oWSResponse.Status = Core.DTOs.ReturnCode._UnknownError
            oWSResponse.Text = Core.DTOs.ReturnCode._UnknownError.ToString
        End Try

        Return oWSResponse
    End Function

    Private Function XStandardAbsenceToAbsenceConverter(oAbsence As roDatalinkStandarAbsence) As roAbsence
        Dim oRet As roAbsence
        Try
            oRet = New roAbsence
            oRet.Id = oAbsence.AbsenceId
            If oAbsence.StartAbsenceDate.Date <> Date.MinValue Then oRet.BeginDate = New Robotics.VTBase.roWCFDate(oAbsence.StartAbsenceDate.Date)
            oRet.Action = oAbsence.Action
            If oAbsence.TimeStamp <> Date.MinValue Then oRet.Timestamp = New Robotics.VTBase.roWCFDate(oAbsence.TimeStamp)
            If oAbsence.EndAbsenceDate.HasValue Then oRet.EndDate = New Robotics.VTBase.roWCFDate(oAbsence.EndAbsenceDate.Value.Date)
            oRet.CauseShortName = oAbsence.CauseShortName
            oRet.CauseExportKey = oAbsence.CauseExportKey
            If oAbsence.Duration.HasValue Then oRet.Duration = New Robotics.VTBase.roWCFDate(oAbsence.Duration.Value)
            oRet.EmployeeId = oAbsence.UniqueEmployeeID
            If oAbsence.BeginHour.HasValue Then oRet.BeginHour = New Robotics.VTBase.roWCFDate(oAbsence.BeginHour.Value)
            If oAbsence.EndHour.HasValue Then oRet.EndHour = New Robotics.VTBase.roWCFDate(oAbsence.EndHour.Value)
            If oAbsence.MaxDays.HasValue Then oRet.MaxDays = oAbsence.MaxDays
        Catch ex As Exception
            oRet = Nothing
        End Try

        Return oRet
    End Function

    Private Function XAbsenceToStandardAbsenceConverter(oAbsence As roAbsence) As roDatalinkStandarAbsence
        Dim oRet As roDatalinkStandarAbsence
        Try
            oRet = New roDatalinkStandarAbsence
            oRet.AbsenceId = oAbsence.Id
            oRet.StartAbsenceDate = oAbsence.BeginDate.Data
            oRet.Action = oAbsence.Action
            If Not oAbsence.Timestamp Is Nothing Then oRet.TimeStamp = oAbsence.Timestamp.Data
            If Not oAbsence.EndDate Is Nothing Then oRet.EndAbsenceDate = oAbsence.EndDate.Data Else oRet.EndAbsenceDate = Date.MinValue
            oRet.CauseShortName = oAbsence.CauseShortName
            oRet.CauseExportKey = oAbsence.CauseExportKey
            If Not oAbsence.Duration Is Nothing Then oRet.Duration = oAbsence.Duration.Data
            oRet.UniqueEmployeeID = oAbsence.EmployeeId
            If Not oAbsence.BeginHour Is Nothing Then oRet.BeginHour = oAbsence.BeginHour.Data
            If Not oAbsence.EndHour Is Nothing Then oRet.EndHour = oAbsence.EndHour.Data
            If Not oAbsence.MaxDays Is Nothing Then oRet.MaxDays = oAbsence.MaxDays Else oRet.MaxDays = 0
        Catch ex As Exception
            oRet = Nothing
        End Try

        Return oRet
    End Function

End Class