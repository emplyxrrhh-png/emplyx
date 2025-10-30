Imports System.Net
Imports System.ServiceModel.Activation
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.ExternalSystems.DataLink.RoboticsExternAccess
Imports Robotics.VTBase
Imports SwaggerWcf.Attributes

<AspNetCompatibilityRequirements(RequirementsMode:=AspNetCompatibilityRequirementsMode.Allowed)>
<SwaggerWcf("/api/v2/EmployeeService.svc")>
<CustomErrorBehavior>
Public Class EmployeeService_v2
    Implements IEmployeesService_v2

    <SwaggerWcfTag("Users")>
    <SwaggerWcfResponse(HttpStatusCode.OK, "OK")>
    <SwaggerWcfResponse(HttpStatusCode.BadRequest, "Bad request", True)>
    <SwaggerWcfResponse(HttpStatusCode.InternalServerError, "Internal error (can be forced using ERROR_500 as book title)", True)>
    Public Function GetEmployees(ByVal Token As String, ByVal OnlyWithActiveContract As Boolean, ByVal IncludeOldData As Boolean, Optional ByVal FieldName As String = "", Optional ByVal FieldValue As String = "", Optional ByVal EmployeeID As String = "", Optional ByVal CompanyCode As String = "") As roWSResponse(Of roEmployee()) Implements IEmployeesService_v2.GetEmployees
        Dim oWSResponse As New roWSResponse(Of roEmployee()) With {
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
            If FieldName Is Nothing Then FieldName = String.Empty
            If FieldValue Is Nothing Then FieldValue = String.Empty

            HttpContext.Current.Session("roClientCompanyId") = CompanyCode
            Dim externAccessInstance As RoboticsExternAccess = RoboticsExternAccess.GetInstance(True, CompanyCode)

            oWSResponse.ApiVersion = roTypes.Any2String(AdvancedParameter.roAdvancedParameter.GetAdvancedParameterValue("VTWebAPIVersion", Nothing))

            Dim iResultText As String = String.Empty
            Dim oEmployees As New List(Of roEmployee)

            If iResultStatus <> Core.DTOs.ReturnCode._TokenValidation AndAlso externAccessInstance.ValidateToken(Token, HttpContext.Current.Request.UserHostAddress, iResultStatus, True) Then
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
    <SwaggerWcfResponse(HttpStatusCode.InternalServerError, "Internal error (can be forced using ERROR_500 as book title)", True)>
    Public Function GetEmployeesByTimestamp(ByVal Token As String, ByVal OnlyWithActiveContract As Boolean, ByVal IncludeOldData As Boolean, ByVal Timestamp As DateTime, Optional ByVal FieldName As String = "", Optional ByVal FieldValue As String = "", Optional ByVal EmployeeID As String = "", Optional ByVal CompanyCode As String = "") As roWSResponse(Of roEmployee()) Implements IEmployeesService_v2.GetEmployeesByTimestamp
        Dim oWSResponse As New roWSResponse(Of roEmployee()) With {
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
            If FieldName Is Nothing Then FieldName = String.Empty
            If FieldValue Is Nothing Then FieldValue = String.Empty

            HttpContext.Current.Session("roClientCompanyId") = CompanyCode
            Dim externAccessInstance As RoboticsExternAccess = RoboticsExternAccess.GetInstance(True, CompanyCode)

            oWSResponse.ApiVersion = roTypes.Any2String(AdvancedParameter.roAdvancedParameter.GetAdvancedParameterValue("VTWebAPIVersion", Nothing))

            Dim iResultText As String = String.Empty
            Dim oEmployees As New List(Of roEmployee)

            If iResultStatus <> Core.DTOs.ReturnCode._TokenValidation AndAlso externAccessInstance.ValidateToken(Token, HttpContext.Current.Request.UserHostAddress, iResultStatus, True) Then
                iResultStatus = Core.DTOs.ReturnCode._UnknownError
                externAccessInstance.GetEmployees(oEmployees, OnlyWithActiveContract, IncludeOldData, EmployeeID, FieldName, FieldValue, iResultStatus, iResultText, Timestamp)
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
    Public Function CreateOrUpdateEmployee(ByVal Token As String, ByVal oEmployee As roDatalinkStandarEmployee, Optional ByVal CompanyCode As String = "") As roDatalinkStandarEmployeeResponse Implements IEmployeesService_v2.CreateOrUpdateEmployee
        Dim iResultCode As New roDatalinkStandarEmployeeResponse With {
            .ResultCode = Core.DTOs.ReturnCode._UnknownError,
            .ResultDetails = String.Empty,
            .ResultEmployee = Nothing
        }

        Try
            Dim iResult As Core.DTOs.ReturnCode = Core.DTOs.ReturnCode._UnknownError

            If Token Is Nothing Then Token = String.Empty
            If CompanyCode Is Nothing OrElse CompanyCode = String.Empty Then
                If Token.Length > 88 Then
                    CompanyCode = RoboticsExternAccess.GetCompanyFromToken(Token, iResult)
                Else
                    CompanyCode = String.Empty
                End If
            End If

            Dim oEmp As roDatalinkStandarEmployee = oEmployee

            HttpContext.Current.Session("roClientCompanyId") = CompanyCode
            Dim externAccessInstance As RoboticsExternAccess = RoboticsExternAccess.GetInstance(True, CompanyCode)
            Dim iNewEmpId As Integer = 0

            If iResult <> Core.DTOs.ReturnCode._TokenValidation AndAlso externAccessInstance.ValidateToken(Token, HttpContext.Current.Request.UserHostAddress, iResult, True) Then
                iResult = Core.DTOs.ReturnCode._UnknownError
                externAccessInstance.CreateOrUpdateEmployee(oEmp, iResult, iNewEmpId)
            End If

            iResultCode.ResultCode = iResult
            iResultCode.ResultDetails = iResult.ToString

            If iResult = Core.DTOs.ReturnCode._OK Then
                Dim oReturnData As roEmployee = Nothing
                Dim strResult As String = String.Empty
                externAccessInstance.GetEmployeeById(oReturnData, iNewEmpId, iResult, strResult)
                If iResult = Core.DTOs.ReturnCode._OK Then
                    iResultCode.ResultEmployee = oReturnData
                Else
                    iResultCode.ResultEmployee = Nothing
                End If

            End If

            HttpContext.Current.Session("roClientCompanyId") = String.Empty
        Catch ex As Exception
            iResultCode.ResultCode = Core.DTOs.ReturnCode._UnknownError
            iResultCode.ResultDetails = Core.DTOs.ReturnCode._UnknownError.ToString
        End Try

        Return iResultCode
    End Function

    <SwaggerWcfTag("Users")>
    <SwaggerWcfResponse(HttpStatusCode.OK, "OK")>
    <SwaggerWcfResponse(HttpStatusCode.BadRequest, "Bad request", True)>
    <SwaggerWcfResponse(HttpStatusCode.InternalServerError,
        "Internal error", True)>
    Public Function UploadEmployeePhoto(ByVal Token As String, ByVal oPhoto As roEmployeePhoto, Optional ByVal CompanyCode As String = "") As roWSResponse(Of roEmployeePhoto) Implements IEmployeesService_v2.UploadEmployeePhoto
        Dim oWSResponse As New roWSResponse(Of roEmployeePhoto) With {
            .Status = Core.DTOs.ReturnCode._UnknownError,
            .Text = String.Empty
        }

        Try
            Dim iResult As Core.DTOs.ReturnCode = Core.DTOs.ReturnCode._UnknownError
            ' Tratamiento de parámetros opcionales
            If Token Is Nothing Then Token = String.Empty
            If CompanyCode Is Nothing OrElse CompanyCode = String.Empty Then
                If Token.Length > 88 Then
                    CompanyCode = RoboticsExternAccess.GetCompanyFromToken(Token, iResult)
                Else
                    CompanyCode = String.Empty
                End If
            End If

            HttpContext.Current.Session("roClientCompanyId") = CompanyCode
            Dim externAccessInstance As RoboticsExternAccess = RoboticsExternAccess.GetInstance(True, CompanyCode)

            oWSResponse.ApiVersion = Robotics.VTBase.roTypes.Any2String(AdvancedParameter.roAdvancedParameter.GetAdvancedParameterValue("VTWebAPIVersion", Nothing))

            If iResult <> Core.DTOs.ReturnCode._TokenValidation AndAlso externAccessInstance.ValidateToken(Token, HttpContext.Current.Request.UserHostAddress, iResult, True) Then
                Dim oStandardPhoto As New roDatalinkStandardPhoto
                oStandardPhoto = XEmployeePhotoToStandardEmployeePhotoConverte(oPhoto)
                iResult = Core.DTOs.ReturnCode._UnknownError
                externAccessInstance.CreateOrUpdateEmployeePhoto(oStandardPhoto, "REST API v2", iResult, oWSResponse.Text)
            End If

            oWSResponse.Status = CInt(iResult)
            oWSResponse.Text = iResult.ToString

            HttpContext.Current.Session("roClientCompanyId") = String.Empty
        Catch ex As Exception
            oWSResponse.Status = Core.DTOs.ReturnCode._UnknownError
            oWSResponse.Text = Core.DTOs.ReturnCode._UnknownError.ToString
        End Try

        Return oWSResponse
    End Function

    Private Function XEmployeePhotoToStandardEmployeePhotoConverte(oEmployeePhoto As roEmployeePhoto) As roDatalinkStandardPhoto
        Dim oRet As roDatalinkStandardPhoto
        Try
            oRet = New roDatalinkStandardPhoto
            oRet.UniqueEmployeeID = oEmployeePhoto.EmployeeID
            oRet.PhotoData = oEmployeePhoto.Photo
        Catch ex As Exception
            oRet = Nothing
        End Try

        Return oRet
    End Function

End Class