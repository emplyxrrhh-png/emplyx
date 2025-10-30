Imports System.Net
Imports System.ServiceModel.Activation
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.ExternalSystems.DataLink.RoboticsExternAccess
Imports SwaggerWcf.Attributes

<AspNetCompatibilityRequirements(RequirementsMode:=AspNetCompatibilityRequirementsMode.Allowed)>
<SwaggerWcf("/api/v2/GroupService.svc")>
<CustomErrorBehavior>
Public Class GroupService_v2
    Implements IGroupsService_v2

    <SwaggerWcfTag("Groups")>
    <SwaggerWcfResponse(HttpStatusCode.OK, "OK")>
    <SwaggerWcfResponse(HttpStatusCode.BadRequest, "Bad request", True)>
    <SwaggerWcfResponse(HttpStatusCode.InternalServerError, "Internal error (can be forced using ERROR_500 as book title)", True)>
    Public Function GetGroups(ByVal Token As String, ByVal IncludeEmployees As Boolean, Optional ByVal Root As String = "", Optional ByVal CompanyCode As String = "", Optional ByVal GroupID As String = "") As roWSResponse(Of roGroup()) Implements IGroupsService_v2.GetGroups
        Dim oWSResponse As roWSResponse(Of roGroup()) = New roWSResponse(Of roGroup())
        Dim oGroups As New List(Of roGroup)
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

            If GroupID Is Nothing Then GroupID = String.Empty

            If Root Is Nothing Then Root = ""
            HttpContext.Current.Session("roClientCompanyId") = CompanyCode
            Dim externAccessInstance As RoboticsExternAccess = RoboticsExternAccess.GetInstance(True, CompanyCode)

            oWSResponse.ApiVersion = Robotics.VTBase.roTypes.Any2String(AdvancedParameter.roAdvancedParameter.GetAdvancedParameterValue("VTWebAPIVersion", Nothing))

            Dim sResultText As String = ""

            If resultStatus <> Core.DTOs.ReturnCode._TokenValidation AndAlso externAccessInstance.ValidateToken(Token, HttpContext.Current.Request.UserHostAddress, resultStatus, True) Then
                resultStatus = Core.DTOs.ReturnCode._UnknownError
                externAccessInstance.GetGroups(oGroups, IncludeEmployees, Root, resultStatus, sResultText, GroupID)
            Else
                sResultText = resultStatus.ToString
            End If

            oWSResponse.Value = oGroups.ToArray
            oWSResponse.Status = Convert.ToInt32(resultStatus)
            oWSResponse.Text = sResultText

            HttpContext.Current.Session("roClientCompanyId") = ""
        Catch ex As Exception
            oWSResponse.Status = Core.DTOs.ReturnCode._UnknownError
        End Try

        Return oWSResponse
    End Function

End Class