Imports System.Net
Imports System.ServiceModel.Activation
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.ExternalSystems.DataLink.RoboticsExternAccess
Imports SwaggerWcf.Attributes

<AspNetCompatibilityRequirements(RequirementsMode:=AspNetCompatibilityRequirementsMode.Allowed)>
<SwaggerWcf("/api/v2/RequestsService.svc")> 'Quitar /VTLiveApi para debug
<CustomErrorBehavior>
Public Class RequestsService_v2
    Implements IRequestsService_v2

    <SwaggerWcfTag("Requests")>
    <SwaggerWcfResponse(HttpStatusCode.OK, "OK")>
    <SwaggerWcfResponse(HttpStatusCode.BadRequest, "Bad request", True)>
    <SwaggerWcfResponse(HttpStatusCode.InternalServerError, "Internal error (can be forced using ERROR_500 as book title)", True)>
    Public Function GetRequestsBetweenDates(ByVal Token As String, StartDate As Date, EndDate As Date, Optional ByVal Employee As String = "", Optional ByVal Type As String = "", Optional ByVal CompanyCode As String = "") As roWSResponse(Of roRequest()) Implements IRequestsService_v2.GetRequestsBetweenDates
        Dim oWSResponse As roWSResponse(Of roRequest()) = New roWSResponse(Of roRequest())
        Try
            Dim resultStatus As Core.DTOs.ReturnCode = Core.DTOs.ReturnCode._UnknownError
            ' Tratamiento de opcionales
            If Employee Is Nothing Then Employee = String.Empty
            If Type Is Nothing Then Type = String.Empty

            If Token Is Nothing Then Token = String.Empty
            If CompanyCode Is Nothing OrElse CompanyCode = String.Empty Then
                If Token IsNot Nothing AndAlso Token.Length > 88 Then
                    CompanyCode = RoboticsExternAccess.GetCompanyFromToken(Token, resultStatus)
                Else
                    CompanyCode = String.Empty
                End If
            End If

            ' Máximo de días en periodo
            If EndDate.Subtract(StartDate).TotalDays > 366 Then EndDate = StartDate.AddDays(366)

            HttpContext.Current.Session("roClientCompanyId") = CompanyCode
            Dim externAccessInstance As RoboticsExternAccess = RoboticsExternAccess.GetInstance(True, CompanyCode)

            oWSResponse.ApiVersion = Robotics.VTBase.roTypes.Any2String(AdvancedParameter.roAdvancedParameter.GetAdvancedParameterValue("VTWebAPIVersion", Nothing))

            Dim sResultText As String = ""
            Dim oRequestsResponse As New roDatalinkStandarRequestResponse

            If resultStatus <> Core.DTOs.ReturnCode._TokenValidation AndAlso externAccessInstance.ValidateToken(Token, HttpContext.Current.Request.UserHostAddress, resultStatus, True) Then
                resultStatus = Core.DTOs.ReturnCode._UnknownError
                Dim oCriteria As New roDatalinkStandarRequestCriteria
                oRequestsResponse = New roDatalinkStandarRequestResponse
                oCriteria.EndPeriod = EndDate
                oCriteria.StartPeriod = StartDate
                oCriteria.UniqueEmployeeID = Employee
                oCriteria.Tipo = Type

                If externAccessInstance.GetRequests(oCriteria, oRequestsResponse) Then
                    oWSResponse.Value = oRequestsResponse.Requests.ConvertAll(AddressOf XRequestConverter).ToArray
                    oWSResponse.Status = Convert.ToInt32(oRequestsResponse.ResultCode)
                    oWSResponse.Text = sResultText
                Else
                    oWSResponse.Value = {}
                    oWSResponse.Status = Convert.ToInt32(oRequestsResponse.ResultCode)
                    oWSResponse.Text = sResultText
                End If
            Else
                oWSResponse.Value = {}
                oWSResponse.Status = Convert.ToInt32(resultStatus)
                oWSResponse.Text = resultStatus.ToString()
            End If

            HttpContext.Current.Session("roClientCompanyId") = ""
        Catch ex As Exception
            oWSResponse.Status = Core.DTOs.ReturnCode._UnknownError
        End Try

        Return oWSResponse
    End Function

    Private Function XRequestConverter(oRequest As roDatalinkStandarRequest) As roRequest
        Dim oRet As roRequest
        Try
            oRet = New roRequest()
            oRet.RequestDate = New Robotics.VTBase.roWCFDate(oRequest.RequestDate)
            oRet.RequestType = oRequest.RequestType
            oRet.IDEmployee = oRequest.UniqueEmployeeID
            oRet.RequestType = oRequest.RequestType
            oRet.RequestDate = New Robotics.VTBase.roWCFDate(oRequest.RequestDate)
            oRet.Status = oRequest.Status
            oRet.MinDate = New Robotics.VTBase.roWCFDate(oRequest.Date1)
            oRet.MaxDate = New Robotics.VTBase.roWCFDate(oRequest.Date2)
            oRet.IDCause = oRequest.IDCause
            oRet.IDShift = oRequest.IDShift
            oRet.Comments = oRequest.Comments
            oRet.FieldName = oRequest.FieldName
            oRet.FieldValue = oRequest.FieldValue
            oRet.Hours = oRequest.Hours
            oRet.IDEmployeeExchange = oRequest.IDEmployeeExchange
            oRet.StartShift = New Robotics.VTBase.roWCFDate(oRequest.StartShift)
            oRet.FromTime = New Robotics.VTBase.roWCFDate(oRequest.FromTime)
            oRet.ToTime = New Robotics.VTBase.roWCFDate(oRequest.ToTime)
            oRet.IDCenter = oRequest.IDCenter
            oRet.RequestDays = oRequest.HolidaysDays
            oRet.Approvals = oRequest.Approvals
        Catch ex As Exception
            oRet = Nothing
        End Try

        Return oRet
    End Function

End Class