Imports System.Net
Imports System.ServiceModel.Activation
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.ExternalSystems.DataLink.RoboticsExternAccess
Imports SwaggerWcf.Attributes

<AspNetCompatibilityRequirements(RequirementsMode:=AspNetCompatibilityRequirementsMode.Allowed)>
Public Class GroupService
    Implements IGroupsService

    <SwaggerWcfTag("Groups")>
    <SwaggerWcfResponse(HttpStatusCode.OK, "OK")>
    <SwaggerWcfResponse(HttpStatusCode.BadRequest, "Bad request", True)>
    <SwaggerWcfResponse(HttpStatusCode.InternalServerError, "Internal error (can be forced using ERROR_500 as book title)", True)>
    Public Function GetGroups(ByVal UserName As String, ByVal UserPwd As String, ByVal IncludeEmployees As Boolean, Optional ByVal Root As String = "", Optional ByVal CompanyCode As String = "") As roWSResponse(Of roGroup()) Implements IGroupsService.GetGroups
        Dim oWSResponse As roWSResponse(Of roGroup()) = New roWSResponse(Of roGroup())
        Dim oGroups As New List(Of roGroup)
        Try
            If Root Is Nothing Then Root = ""
            HttpContext.Current.Session("roClientCompanyId") = CompanyCode
            Dim externAccessInstance As RoboticsExternAccess = RoboticsExternAccess.GetInstance(True, CompanyCode)

            oWSResponse.ApiVersion = Robotics.VTBase.roTypes.Any2String(AdvancedParameter.roAdvancedParameter.GetAdvancedParameterValue("VTWebAPIVersion", Nothing))

            Dim resultStatus As Core.DTOs.ReturnCode = Core.DTOs.ReturnCode._UnknownError
            Dim sResultText As String = ""

            If externAccessInstance.ValidateUserNamePassword(UserName, UserPwd, HttpContext.Current.Request.UserHostAddress, resultStatus, True) Then
                resultStatus = Core.DTOs.ReturnCode._UnknownError
                externAccessInstance.GetGroups(oGroups, IncludeEmployees, Root, resultStatus, sResultText)
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