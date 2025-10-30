Imports System.Net
Imports System.ServiceModel.Activation
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.ExternalSystems.DataLink.RoboticsExternAccess
Imports Robotics.ExternalSystems.DataLink.RoboticsExternAccess.roDatalinkDailyCause
Imports SwaggerWcf.Attributes

<AspNetCompatibilityRequirements(RequirementsMode:=AspNetCompatibilityRequirementsMode.Allowed)>
<SwaggerWcf("/api/v2/DailyCausesService.svc")>
<CustomErrorBehavior>
Public Class DailyCausesService_v2
    Implements IDailyCausesService_v2

    <SwaggerWcfTag("DailyCauses")>
    <SwaggerWcfResponse(HttpStatusCode.OK, "OK")>
    <SwaggerWcfResponse(HttpStatusCode.BadRequest, "Bad request", True)>
    <SwaggerWcfResponse(HttpStatusCode.InternalServerError,
        "Internal error", True)>
    Public Function CreateOrUpdateDailyCause(ByVal Token As String, ByVal oDailyCause As roDailyCause, Optional ByVal CompanyCode As String = "") As roDatalinkStandarDailyCauseResponse Implements IDailyCausesService_v2.CreateOrUpdateDailyCause
        Dim iResultCode As New roDatalinkStandarDailyCauseResponse With {
            .ResultCode = Core.DTOs.ReturnCode._UnknownError,
            .ResultDetails = String.Empty,
            .ResultDailyCause = Nothing
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

            Dim oDaily As roDailyCause = oDailyCause

            HttpContext.Current.Session("roClientCompanyId") = CompanyCode
            Dim externAccessInstance As RoboticsExternAccess = RoboticsExternAccess.GetInstance(True, CompanyCode)
            Dim oDailyCauseDataLink As New roDatalinkDailyCause

            If iResult <> Core.DTOs.ReturnCode._TokenValidation AndAlso externAccessInstance.ValidateToken(Token, HttpContext.Current.Request.UserHostAddress, iResult, True) Then
                If oDailyCause.Manual IsNot Nothing AndAlso oDailyCause.Manual = False Then
                    iResult = Core.DTOs.ReturnCode._InvalidManualValue
                Else

                    iResult = Core.DTOs.ReturnCode._UnknownError
                    oDailyCauseDataLink = New roDatalinkDailyCause
                    oDailyCauseDataLink.UniqueEmployeeID = oDailyCause.EmployeeId
                    oDailyCauseDataLink.ShortCauseName = oDailyCause.CauseShortName
                    oDailyCauseDataLink.CauseDate = oDailyCause.DateTime.GetDate().Date
                    oDailyCauseDataLink.Value = oDaily.Value
                    If oDailyCauseDataLink.Value <> 0 Then
                        externAccessInstance.CreateOrUpdateDailyCause(oDailyCauseDataLink, iResult)
                    Else
                        externAccessInstance.DeleteDailyCause(oDailyCauseDataLink, iResult)
                    End If
                End If
            End If

            iResultCode.ResultCode = iResult
            iResultCode.ResultDetails = iResult.ToString

            If iResult = Core.DTOs.ReturnCode._OK Then
                Dim oCriteria As New roDatalinkStandarDailyCauseCriteria
                Dim oCausesResponse As New roDatalinkStandarCauseResponse
                oCriteria.EndCausePeriod = oDailyCauseDataLink.CauseDate
                oCriteria.StartCausePeriod = oDailyCauseDataLink.CauseDate
                oCriteria.UniqueEmployeeID = oDailyCauseDataLink.UniqueEmployeeID

                externAccessInstance.GetCauses(oCriteria, oCausesResponse)

                If iResult = Core.DTOs.ReturnCode._OK Then
                    Dim oResultDailyCause = oCausesResponse.Causes.ConvertAll(AddressOf XCauseConverter) _
                        .FirstOrDefault(Function(item) item.Manual) 'Para esa fecha, solo puede haber una manual es decir la que hemos creado/actualizado

                    If oResultDailyCause IsNot Nothing Then
                        iResultCode.ResultDailyCause = oResultDailyCause
                    End If
                End If

            End If
            HttpContext.Current.Session("roClientCompanyId") = String.Empty
        Catch ex As Exception
            iResultCode.ResultCode = Core.DTOs.ReturnCode._UnknownError
            iResultCode.ResultDetails = Core.DTOs.ReturnCode._UnknownError.ToString
        End Try

        Return iResultCode
    End Function

    <SwaggerWcfTag("DailyCauses")>
    <SwaggerWcfResponse(HttpStatusCode.OK, "OK")>
    <SwaggerWcfResponse(HttpStatusCode.BadRequest, "Bad request", True)>
    <SwaggerWcfResponse(HttpStatusCode.InternalServerError, "Internal error (can be forced using ERROR_500 as book title)", True)>
    Public Function GetDailyCauses(ByVal Token As String, StartDate As Date, EndDate As Date, Optional ByVal Employee As String = "", Optional ByVal CompanyCode As String = "", Optional ByVal AddRelatedIncidence As Boolean = False, Optional ByVal Criteria As String = "") As roWSResponse(Of roDailyCause()) Implements IDailyCausesService_v2.GetDailyCauses
        Dim oWSResponse As roWSResponse(Of roDailyCause()) = New roWSResponse(Of roDailyCause())
        Dim oDailyCauses As New List(Of roDailyCause)
        Try
            Dim resultStatus As Core.DTOs.ReturnCode = Core.DTOs.ReturnCode._UnknownError
            ' Tratamiento de opcionales
            If Employee Is Nothing Then Employee = String.Empty
            If Token Is Nothing Then Token = String.Empty
            If Criteria Is Nothing Then Criteria = String.Empty
            If CompanyCode Is Nothing OrElse CompanyCode = String.Empty Then
                If Token IsNot Nothing AndAlso Token.Length > 88 Then
                    CompanyCode = RoboticsExternAccess.GetCompanyFromToken(Token, resultStatus)
                Else
                    CompanyCode = String.Empty
                End If
            End If

            HttpContext.Current.Session("roClientCompanyId") = CompanyCode
            Dim externAccessInstance As RoboticsExternAccess = RoboticsExternAccess.GetInstance(True, CompanyCode)

            oWSResponse.ApiVersion = Robotics.VTBase.roTypes.Any2String(AdvancedParameter.roAdvancedParameter.GetAdvancedParameterValue("VTWebAPIVersion", Nothing))


            If resultStatus <> Core.DTOs.ReturnCode._TokenValidation AndAlso externAccessInstance.ValidateToken(Token, HttpContext.Current.Request.UserHostAddress, resultStatus, True) Then
                resultStatus = Core.DTOs.ReturnCode._UnknownError
                Dim oCriteria As New roDatalinkStandarDailyCauseCriteria
                Dim oCausesResponse As New roDatalinkStandarCauseResponse
                oCriteria.EndCausePeriod = EndDate
                oCriteria.StartCausePeriod = StartDate
                oCriteria.UniqueEmployeeID = Employee
                If IsNothing(AddRelatedIncidence) Then
                    AddRelatedIncidence = False
                End If
                oCriteria.ShowChangesInPeriod = (Criteria.ToUpper = "TIMESTAMP")
                oCriteria.AddRelatedIncidence = AddRelatedIncidence

                If externAccessInstance.GetCauses(oCriteria, oCausesResponse) Then
                    oWSResponse.Value = oCausesResponse.Causes.ConvertAll(AddressOf XCauseConverter).ToArray
                    oWSResponse.Status = Convert.ToInt32(oCausesResponse.ResultCode)
                    oWSResponse.Text = oWSResponse.Status.ToString()
                Else
                    oWSResponse.Value = {}
                    oWSResponse.Status = Convert.ToInt32(oCausesResponse.ResultCode)
                    oWSResponse.Text = oWSResponse.Status.ToString()
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

    <SwaggerWcfTag("DailyCauses")>
    <SwaggerWcfResponse(HttpStatusCode.OK, "OK")>
    <SwaggerWcfResponse(HttpStatusCode.BadRequest, "Bad request", True)>
    <SwaggerWcfResponse(HttpStatusCode.InternalServerError, "Internal error (can be forced using ERROR_500 as book title)", True)>
    Public Function GetDailyCausesByTimestamp(ByVal Token As String, ByVal Timestamp As DateTime, Optional ByVal Employee As String = "", Optional ByVal CompanyCode As String = "", Optional ByVal AddRelatedIncidence As Boolean = False) As roWSResponse(Of roDailyCause()) Implements IDailyCausesService_v2.GetDailyCausesByTimestamp
        Dim oWSResponse As roWSResponse(Of roDailyCause()) = New roWSResponse(Of roDailyCause())
        Dim oDailyCauses As New List(Of roDailyCause)
        Try
            Dim resultStatus As Core.DTOs.ReturnCode = Core.DTOs.ReturnCode._UnknownError
            ' Tratamiento de opcionales
            If Employee Is Nothing Then Employee = String.Empty
            If Token Is Nothing Then Token = String.Empty
            If CompanyCode Is Nothing OrElse CompanyCode = String.Empty Then
                If Token IsNot Nothing AndAlso Token.Length > 88 Then
                    CompanyCode = RoboticsExternAccess.GetCompanyFromToken(Token, resultStatus)
                Else
                    CompanyCode = String.Empty
                End If
            End If

            HttpContext.Current.Session("roClientCompanyId") = CompanyCode
            Dim externAccessInstance As RoboticsExternAccess = RoboticsExternAccess.GetInstance(True, CompanyCode)

            oWSResponse.ApiVersion = Robotics.VTBase.roTypes.Any2String(AdvancedParameter.roAdvancedParameter.GetAdvancedParameterValue("VTWebAPIVersion", Nothing))

            If resultStatus <> Core.DTOs.ReturnCode._TokenValidation AndAlso externAccessInstance.ValidateToken(Token, HttpContext.Current.Request.UserHostAddress, resultStatus, True) Then
                resultStatus = Core.DTOs.ReturnCode._UnknownError
                Dim oCriteria As New roDatalinkStandarDailyCauseCriteria
                Dim oCausesResponse As New roDatalinkStandarCauseResponse
                oCriteria.Timestamp = Timestamp
                oCriteria.UniqueEmployeeID = Employee
                If IsNothing(AddRelatedIncidence) Then
                    AddRelatedIncidence = False
                End If
                oCriteria.AddRelatedIncidence = AddRelatedIncidence

                If externAccessInstance.GetCausesByTimestamp(oCriteria, oCausesResponse) Then
                    oWSResponse.Value = oCausesResponse.Causes.ConvertAll(AddressOf XCauseConverter).ToArray
                    oWSResponse.Status = Convert.ToInt32(oCausesResponse.ResultCode)
                    oWSResponse.Text = oWSResponse.Status.ToString()
                Else
                    oWSResponse.Value = {}
                    oWSResponse.Status = Convert.ToInt32(oCausesResponse.ResultCode)
                    oWSResponse.Text = oWSResponse.Status.ToString()
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

    Private Function XCauseConverter(oCause As roDatalinkStandarDailyCause) As roDailyCause
        Dim oRet As roDailyCause
        Try
            oRet = New roDailyCause
            oRet.DateTime = New Robotics.VTBase.roWCFDate(oCause.CauseDate.Date)
            oRet.CauseShortName = oCause.CauseShortName
            oRet.Value = oCause.CauseValue
            oRet.EmployeeId = oCause.UniqueEmployeeID
            oRet.Manual = oCause.Manual
            oRet.IDIncidence = oCause.Incidence
            oRet.CauseEquivalenceCode = oCause.CauseEquivalenceCode
            oRet.Incidence = Nothing
            If oRet.IDIncidence > 0 AndAlso oCause.IncidenceData IsNot Nothing Then
                oRet.Incidence = New roDailyIncidence
                oRet.Incidence.IDIncidence = oCause.IncidenceData.Incidence
                oRet.Incidence.DateTime = New Robotics.VTBase.roWCFDate(oCause.IncidenceData.IncidenceDate)
                oRet.Incidence.IncidenceType = oCause.IncidenceData.IncidenceType
                oRet.Incidence.BeginTime = New Robotics.VTBase.roWCFDate(oCause.IncidenceData.IncidenceBeginTime)
                oRet.Incidence.EndTime = New Robotics.VTBase.roWCFDate(oCause.IncidenceData.IncidenceEndTime)
                oRet.Incidence.Zone = oCause.IncidenceData.IncidenceZone
                oRet.Incidence.Value = oCause.IncidenceData.IncidenceValue
            End If



        Catch ex As Exception
            oRet = Nothing
        End Try

        Return oRet
    End Function

End Class