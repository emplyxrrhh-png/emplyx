Imports System.Net
Imports System.ServiceModel.Activation
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTBusiness.Job.roPerformance
Imports Robotics.ExternalSystems.DataLink.RoboticsExternAccess
Imports SwaggerWcf.Attributes

<AspNetCompatibilityRequirements(RequirementsMode:=AspNetCompatibilityRequirementsMode.Allowed)>
<SwaggerWcf("/api/v2/DocumentsService.svc")> 'Quitar /VTLiveApi para debug
<CustomErrorBehavior>
Public Class DocumentsService_v2
    Implements IDocumentsService_v2

    <SwaggerWcfTag("Documents")>
    <SwaggerWcfResponse(HttpStatusCode.OK, "OK")>
    <SwaggerWcfResponse(HttpStatusCode.BadRequest, "Bad request", True)>
    <SwaggerWcfResponse(HttpStatusCode.InternalServerError, "Internal error", True)>
    Public Function AddDocument(ByVal Token As String, ByVal oDocument As roDocument, Optional ByVal CompanyCode As String = "") As roWSResponse(Of roDocument) Implements IDocumentsService_v2.AddDocument
        Dim oWSResponse As New roWSResponse(Of roDocument) With {
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
                Dim oStandardDocument As New roDatalinkStandardDocument
                oStandardDocument = XDocumentToStandardDocumentConverte(oDocument)
                iResult = Core.DTOs.ReturnCode._UnknownError
                externAccessInstance.CreateOrUpdateDocument(oStandardDocument, "REST API v2", iResult, oWSResponse.Text)
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

    <SwaggerWcfTag("Documents")>
    <SwaggerWcfResponse(HttpStatusCode.OK, "OK")>
    <SwaggerWcfResponse(HttpStatusCode.BadRequest, "Bad request", True)>
    <SwaggerWcfResponse(HttpStatusCode.InternalServerError, "Internal error (can be forced using ERROR_500 as book title)", True)>
    Public Function GetDocuments(ByVal Token As String, ByVal Type As String, Optional ByVal Title As String = "", Optional ByVal Employee As String = "", Optional ByVal Company As String = "", Optional ByVal Timestamp As DateTime = Nothing, Optional ByVal UpdateType As String = "", Optional ByVal Extension As String = "", Optional ByVal Template As String = "", Optional ByVal CompanyCode As String = "") As roWSResponse(Of roDocument()) Implements IDocumentsService_v2.GetDocuments
        Dim oWSResponse As New roWSResponse(Of roDocument()) With {
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
                iResult = Core.DTOs.ReturnCode._UnknownError
                Dim documentCriteria As roDatalinkStandarDocumentCriteria = New roDatalinkStandarDocumentCriteria
                documentCriteria.Company = Company
                documentCriteria.Template = Template
                documentCriteria.Title = Title
                documentCriteria.Timestamp = Timestamp
                documentCriteria.Extension = Extension
                documentCriteria.UniqueEmployeeID = Employee
                documentCriteria.UpdateType = UpdateType
                documentCriteria.Type = Type
                Dim documentsResponse As roDatalinkStandarDocumentResponse = New roDatalinkStandarDocumentResponse()
                Dim sResultText As String = ""
                If externAccessInstance.GetDocuments(documentCriteria, documentsResponse) Then
                    oWSResponse.Value = documentsResponse.Documents.ToArray()
                    oWSResponse.Status = Convert.ToInt32(documentsResponse.ResultCode)
                    oWSResponse.Text = sResultText
                Else
                    oWSResponse.Value = {}
                    oWSResponse.Status = CInt(documentsResponse.ResultCode)
                    oWSResponse.Text = documentsResponse.ResultDetails
                End If
            Else
                oWSResponse.Value = {}
                oWSResponse.Status = iResult
                oWSResponse.Text = iResult.ToString
            End If

            HttpContext.Current.Session("roClientCompanyId") = String.Empty
        Catch ex As Exception
            oWSResponse.Status = Core.DTOs.ReturnCode._UnknownError
            oWSResponse.Text = Core.DTOs.ReturnCode._UnknownError.ToString
        End Try

        Return oWSResponse
    End Function

    <SwaggerWcfTag("Documents")>
    <SwaggerWcfResponse(HttpStatusCode.OK, "OK")>
    <SwaggerWcfResponse(HttpStatusCode.BadRequest, "Bad request", True)>
    <SwaggerWcfResponse(HttpStatusCode.InternalServerError, "Internal error", True)>
    Public Function DeleteDocument(ByVal Token As String, ByVal ExternalId As String, Optional ByVal CompanyCode As String = "") As roWSResponse(Of roDocumentResponse) Implements IDocumentsService_v2.DeleteDocument
        Dim oWSResponse As New roWSResponse(Of roDocumentResponse) With {
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
                iResult = Core.DTOs.ReturnCode._UnknownError
                externAccessInstance.DeleteDocument(ExternalId, iResult, oWSResponse.Text)
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
            oRet.DocumentExternalId = oDocument.DocumentExternalID
        Catch ex As Exception
            oRet = Nothing
        End Try

        Return oRet
    End Function


End Class