Imports System.Net
Imports System.ServiceModel.Activation
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.ExternalSystems.DataLink.RoboticsExternAccess
Imports Robotics.VTBase
Imports SwaggerWcf.Attributes

<AspNetCompatibilityRequirements(RequirementsMode:=AspNetCompatibilityRequirementsMode.Allowed)>
<SwaggerWcf("/api/v2/AccessService.svc")> 'Quitar /VTLiveApi para debug
<CustomErrorBehavior>
Public Class AccessService_v2
    Implements IAccessService_v2

    <SwaggerWcfTag("Devices")>
    <SwaggerWcfResponse(HttpStatusCode.OK, "OK")>
    <SwaggerWcfResponse(HttpStatusCode.BadRequest, "Bad request", True)>
    <SwaggerWcfResponse(HttpStatusCode.InternalServerError, "Internal Server Error", True)>
    Public Function GetTerminalConfiguration(ByVal Token As String, ByVal TerminalID As String, Optional ByVal CompanyCode As String = "") As roWSResponse(Of roTerminalConfiguration) Implements IAccessService_v2.GetTerminalConfiguration
        Dim oWSResponse As New roWSResponse(Of roTerminalConfiguration) With {
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
            Dim oTerminalConfiguration As roTerminalConfiguration = New roTerminalConfiguration()

            If iResultStatus <> Core.DTOs.ReturnCode._TokenValidation AndAlso externAccessInstance.ValidateToken(Token, HttpContext.Current.Request.UserHostAddress, iResultStatus, True) Then
                iResultStatus = Core.DTOs.ReturnCode._UnknownError

                externAccessInstance.GetTerminalConfiguration(oTerminalConfiguration, TerminalID, iResultStatus, iResultText)
            End If

            oWSResponse.Value = oTerminalConfiguration
            oWSResponse.Status = iResultStatus
            oWSResponse.Text = iResultStatus.ToString

            HttpContext.Current.Session("roClientCompanyId") = String.Empty
        Catch ex As Exception
            oWSResponse.Status = Core.DTOs.ReturnCode._UnknownError
            oWSResponse.Text = Core.DTOs.ReturnCode._UnknownError.ToString
        End Try

        Return oWSResponse
    End Function

    <SwaggerWcfTag("Devices")>
    <SwaggerWcfResponse(HttpStatusCode.OK, "OK")>
    <SwaggerWcfResponse(HttpStatusCode.BadRequest, "Bad request", True)>
    <SwaggerWcfResponse(HttpStatusCode.InternalServerError, "Internal Server Error", True)>
    Public Function GetTerminalDateTime(ByVal Token As String, ByVal TerminalID As String, Optional ByVal CompanyCode As String = "") As roWSResponse(Of roWCFDate) Implements IAccessService_v2.GetTerminalDateTime
        Dim oWSResponse As New roWSResponse(Of roWCFDate) With {
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
            Dim oTerminalDateTime As DateTime = Nothing

            If iResultStatus <> Core.DTOs.ReturnCode._TokenValidation AndAlso externAccessInstance.ValidateToken(Token, HttpContext.Current.Request.UserHostAddress, iResultStatus, True) Then
                iResultStatus = Core.DTOs.ReturnCode._UnknownError

                externAccessInstance.GetTerminalDateTime(oTerminalDateTime, TerminalID, iResultStatus, iResultText)
            End If

            oWSResponse.Value = New Robotics.VTBase.roWCFDate(oTerminalDateTime)
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