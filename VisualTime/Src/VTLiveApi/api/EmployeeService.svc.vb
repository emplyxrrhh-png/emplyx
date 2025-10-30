Imports System.Net
Imports System.ServiceModel.Activation
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.ExternalSystems.DataLink.RoboticsExternAccess
Imports Robotics.VTBase
Imports SwaggerWcf.Attributes

<AspNetCompatibilityRequirements(RequirementsMode:=AspNetCompatibilityRequirementsMode.Allowed)>
Public Class EmployeeService
    Implements IEmployeesService

    <SwaggerWcfTag("Users")>
    <SwaggerWcfResponse(HttpStatusCode.OK, "OK")>
    <SwaggerWcfResponse(HttpStatusCode.BadRequest, "Bad request", True)>
    <SwaggerWcfResponse(HttpStatusCode.InternalServerError, "Internal error (can be forced using ERROR_500 as book title)", True)>
    Public Function GetEmployees(ByVal CompanyCode As String, ByVal UserName As String, ByVal UserPwd As String, ByVal OnlyWithActiveContract As Boolean, ByVal IncludeOldData As Boolean, Optional ByVal FieldName As String = "", Optional ByVal FieldValue As String = "", Optional ByVal EmployeeID As String = "") As roWSResponse(Of roEmployee()) Implements IEmployeesService.GetEmployees
        Dim oWSResponse As New roWSResponse(Of roEmployee()) With {
            .Status = Core.DTOs.ReturnCode._UnknownError,
            .Text = String.Empty
        }

        Try
            Dim cCode As String = CompanyCode
            Dim usr As String = UserName
            Dim pwd As String = UserPwd

            If EmployeeID Is Nothing Then EmployeeID = String.Empty
            If FieldName Is Nothing Then FieldName = String.Empty
            If FieldValue Is Nothing Then FieldValue = String.Empty

            HttpContext.Current.Session("roClientCompanyId") = cCode
            Dim externAccessInstance As RoboticsExternAccess = RoboticsExternAccess.GetInstance(True, cCode)

            oWSResponse.ApiVersion = roTypes.Any2String(AdvancedParameter.roAdvancedParameter.GetAdvancedParameterValue("VTWebAPIVersion", Nothing))

            Dim iResultStatus As Core.DTOs.ReturnCode = Core.DTOs.ReturnCode._UnknownError
            Dim iResultText As String = String.Empty
            Dim oEmployees As New List(Of roEmployee)

            If externAccessInstance.ValidateUserNamePassword(usr, pwd, HttpContext.Current.Request.UserHostAddress, iResultStatus, True) Then
                iResultStatus = Core.DTOs.ReturnCode._UnknownError
                externAccessInstance.GetEmployees(oEmployees, OnlyWithActiveContract, IncludeOldData, EmployeeID, FieldName, FieldValue, iResultStatus, iResultText)
            End If

            oWSResponse.Value = oEmployees.ToArray
            oWSResponse.Status = iResultStatus
            oWSResponse.Text = iResultStatus.ToString

            HttpContext.Current.Session("roClientCompanyId") = String.Empty
        Catch ex As Exception
            oWSResponse.Status = Core.DTOs.ReturnCode._UnknownError
            oWSResponse.Text = Core.DTOs.ReturnCode._UnknownError.ToString
        End Try

        Return oWSResponse

    End Function

    <SwaggerWcfTag("Users")>
    <SwaggerWcfResponse(HttpStatusCode.OK, "OK")>
    <SwaggerWcfResponse(HttpStatusCode.BadRequest, "Bad request", True)>
    <SwaggerWcfResponse(HttpStatusCode.InternalServerError,
        "Internal error", True)>
    Public Function CreateOrUpdateEmployee(ByVal CompanyCode As String, ByVal UserName As String, ByVal UserPwd As String, ByVal oEmployee As roDatalinkStandarEmployee) As roDatalinkStandarEmployeeResponse Implements IEmployeesService.CreateOrUpdateEmployee
        Dim iResultCode As New roDatalinkStandarEmployeeResponse With {
            .ResultCode = Core.DTOs.ReturnCode._UnknownError,
            .ResultDetails = String.Empty,
            .ResultEmployee = Nothing
        }

        Try
            Dim cCode As String = CompanyCode
            Dim usr As String = UserName
            Dim pwd As String = UserPwd

            Dim oEmp As roDatalinkStandarEmployee = oEmployee

            HttpContext.Current.Session("roClientCompanyId") = cCode
            Dim externAccessInstance As RoboticsExternAccess = RoboticsExternAccess.GetInstance(True, cCode)
            Dim iResult As Core.DTOs.ReturnCode = Core.DTOs.ReturnCode._UnknownError
            Dim iNewEmpId As Integer = 0
            If externAccessInstance.ValidateUserNamePassword(usr, pwd, HttpContext.Current.Request.UserHostAddress, iResult, True) Then
                iResult = Core.DTOs.ReturnCode._UnknownError
                externAccessInstance.CreateOrUpdateEmployee(oEmp, iResult, iNewEmpId)
            End If

            iResultCode.ResultCode = iResult
            iResultCode.ResultDetails = iResult.ToString

            HttpContext.Current.Session("roClientCompanyId") = String.Empty
        Catch ex As Exception
            iResultCode.ResultCode = Core.DTOs.ReturnCode._UnknownError
            iResultCode.ResultDetails = Core.DTOs.ReturnCode._UnknownError.ToString
        End Try

        Return iResultCode
    End Function

End Class