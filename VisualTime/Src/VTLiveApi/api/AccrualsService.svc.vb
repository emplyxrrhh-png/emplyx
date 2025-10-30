Imports System.Net
Imports System.ServiceModel.Activation
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.ExternalSystems.DataLink.RoboticsExternAccess
Imports SwaggerWcf.Attributes

<AspNetCompatibilityRequirements(RequirementsMode:=AspNetCompatibilityRequirementsMode.Allowed)>
Public Class AccrualsService
    Implements IAccrualsService

    <SwaggerWcfTag("Accruals")>
    <SwaggerWcfResponse(HttpStatusCode.OK, "OK")>
    <SwaggerWcfResponse(HttpStatusCode.BadRequest, "Bad request", True)>
    <SwaggerWcfResponse(HttpStatusCode.InternalServerError, "Internal error (can be forced using ERROR_500 as book title)", True)>
    Public Function GetAccruals(ByVal UserName As String, ByVal UserPwd As String, StartDate As Date, EndDate As Date, Optional ByVal Employee As String = "", Optional ByVal CompanyCode As String = "") As roWSResponse(Of roAccrual()) Implements IAccrualsService.GetAccruals
        Dim oWSResponse As roWSResponse(Of roAccrual()) = New roWSResponse(Of roAccrual())
        Dim oAccruals As New List(Of roAccrual)
        Try
            ' Tratamiento de opcionales
            If Employee Is Nothing Then Employee = String.Empty
            If CompanyCode Is Nothing Then CompanyCode = String.Empty

            ' Máximo de días en periodo
            If EndDate.Subtract(StartDate).TotalDays > 366 Then EndDate = StartDate.AddDays(366)

            HttpContext.Current.Session("roClientCompanyId") = CompanyCode
            Dim externAccessInstance As RoboticsExternAccess = RoboticsExternAccess.GetInstance(True, CompanyCode)

            oWSResponse.ApiVersion = Robotics.VTBase.roTypes.Any2String(AdvancedParameter.roAdvancedParameter.GetAdvancedParameterValue("VTWebAPIVersion", Nothing))

            Dim resultStatus As Core.DTOs.ReturnCode = Core.DTOs.ReturnCode._UnknownError
            Dim sResultText As String = ""
            Dim oAccrualsResponse As New roDatalinkStandarAccrualResponse

            If externAccessInstance.ValidateUserNamePassword(UserName, UserPwd, HttpContext.Current.Request.UserHostAddress, resultStatus, True) Then
                resultStatus = Core.DTOs.ReturnCode._UnknownError
                Dim oCriteria As New roDatalinkStandarAccrualCriteria
                oAccrualsResponse = New roDatalinkStandarAccrualResponse
                oCriteria.EndAccrualPeriod = EndDate
                oCriteria.StartAccrualPeriod = StartDate
                oCriteria.UniqueEmployeeID = Employee

                If externAccessInstance.GetAccruals(oCriteria, oAccrualsResponse) Then
                    oWSResponse.Value = oAccrualsResponse.Accruals.ConvertAll(AddressOf XAccrualConverter).ToArray
                    oWSResponse.Status = Convert.ToInt32(oAccrualsResponse.ResultCode)
                    oWSResponse.Text = sResultText
                Else
                    oWSResponse.Value = {}
                    oWSResponse.Status = Convert.ToInt32(oAccrualsResponse.ResultCode)
                    oWSResponse.Text = sResultText
                End If
            End If

            HttpContext.Current.Session("roClientCompanyId") = ""
        Catch ex As Exception
            oWSResponse.Status = Core.DTOs.ReturnCode._UnknownError
        End Try

        Return oWSResponse
    End Function

    Private Function XAccrualConverter(oAccrual As roDatalinkStandarAccrual) As roAccrual
        Dim oRet As roAccrual
        Try
            oRet = New roAccrual
            oRet.AccrualDate = New Robotics.VTBase.roWCFDate(oAccrual.AccrualDate.Date)
            oRet.AccrualExportKey = oAccrual.AccrualExportKey
            oRet.AccrualShortName = oAccrual.AccrualShortName
            oRet.AccrualValue = oAccrual.AccrualValue
            oRet.IDEmployee = oAccrual.UniqueEmployeeID
        Catch ex As Exception
            oRet = Nothing
        End Try

        Return oRet
    End Function

End Class