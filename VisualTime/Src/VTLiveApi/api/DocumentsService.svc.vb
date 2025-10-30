Imports System.Net
Imports System.ServiceModel.Activation
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.ExternalSystems.DataLink.RoboticsExternAccess
Imports SwaggerWcf.Attributes

<AspNetCompatibilityRequirements(RequirementsMode:=AspNetCompatibilityRequirementsMode.Allowed)>
Public Class DocumentsService
    Implements IDocumentsService

    <SwaggerWcfTag("Documents")>
    <SwaggerWcfResponse(HttpStatusCode.OK, "OK")>
    <SwaggerWcfResponse(HttpStatusCode.BadRequest, "Bad request", True)>
    <SwaggerWcfResponse(HttpStatusCode.InternalServerError, "Internal error", True)>
    Public Function AddDocument(ByVal UserName As String, ByVal UserPwd As String, ByVal oDocument As roDocument, Optional ByVal CompanyCode As String = "") As roWSResponse(Of roDocument) Implements IDocumentsService.AddDocument
        Dim oWSResponse As New roWSResponse(Of roDocument) With {
            .Status = Core.DTOs.ReturnCode._UnknownError,
            .Text = String.Empty
        }

        Try
            ' Tratamiento de parámetros opcionales
            If CompanyCode Is Nothing Then CompanyCode = String.Empty

            HttpContext.Current.Session("roClientCompanyId") = CompanyCode
            Dim externAccessInstance As RoboticsExternAccess = RoboticsExternAccess.GetInstance(True, CompanyCode)

            oWSResponse.ApiVersion = Robotics.VTBase.roTypes.Any2String(AdvancedParameter.roAdvancedParameter.GetAdvancedParameterValue("VTWebAPIVersion", Nothing))

            Dim iResult As Core.DTOs.ReturnCode = Core.DTOs.ReturnCode._UnknownError

            If externAccessInstance.ValidateUserNamePassword(UserName, UserPwd, HttpContext.Current.Request.UserHostAddress, iResult, True) Then
                Dim oStandardDocument As New roDatalinkStandardDocument
                oStandardDocument = XDocumentToStandardDocumentConverte(oDocument)
                iResult = Core.DTOs.ReturnCode._UnknownError
                externAccessInstance.CreateOrUpdateDocument(oStandardDocument, UserName, iResult, oWSResponse.Text)
            End If

            oWSResponse.Status = iResult

            HttpContext.Current.Session("roClientCompanyId") = String.Empty
        Catch ex As Exception
            oWSResponse.Status = Core.DTOs.ReturnCode._UnknownError
            oWSResponse.Text = Core.DTOs.ReturnCode._UnknownError.ToString
        End Try

        Return oWSResponse
    End Function

    Private Function XDocumentToStandardDocumentConverte(oDocument As roDocument) As roDatalinkStandardDocument
        Dim oRet As roDatalinkStandardDocument
        Try
            oRet = New roDatalinkStandardDocument
            oRet.UniqueEmployeeID = oDocument.EmployeeID
            oRet.DocumentType = oDocument.DocumentType
            oRet.DocumentTitle = oDocument.DocumentTitle
            oRet.DocumentExtension = oDocument.DocumentExtension
            oRet.DocumentData = oDocument.DocumentData
            oRet.DocumentRemarks = oDocument.DocumentRemarks
        Catch ex As Exception
            oRet = Nothing
        End Try

        Return oRet
    End Function

End Class