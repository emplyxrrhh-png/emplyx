Imports System.Net
Imports System.ServiceModel.Activation
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.ExternalSystems.DataLink.RoboticsExternAccess
Imports SwaggerWcf.Attributes

<AspNetCompatibilityRequirements(RequirementsMode:=AspNetCompatibilityRequirementsMode.Allowed)>
<SwaggerWcf("/api/v2/TasksService.svc")> 'Quitar /VTLiveApi para debug
<CustomErrorBehavior>
Public Class TasksService_v2
    Implements ITasksService_v2

    <SwaggerWcfTag("Tasks")>
    <SwaggerWcfResponse(HttpStatusCode.OK, "OK")>
    <SwaggerWcfResponse(HttpStatusCode.BadRequest, "Bad request", True)>
    <SwaggerWcfResponse(HttpStatusCode.InternalServerError, "Internal error (can be forced using ERROR_500 as book title)", True)>
    Public Function GetTaskAccrualsBetweenDates(ByVal Token As String, StartDate As Date, EndDate As Date, Optional ByVal Employee As String = "", Optional ByVal Project As String = "", Optional ByVal Task As String = "", Optional ByVal CompanyCode As String = "") As roWSResponse(Of roTaskAccrual()) Implements ITasksService_v2.GetTaskAccrualsBetweenDates
        Dim oWSResponse As roWSResponse(Of roTaskAccrual()) = New roWSResponse(Of roTaskAccrual())
        Dim oAccruals As New List(Of roTaskAccrual)
        Try
            Dim resultStatus As Core.DTOs.ReturnCode = Core.DTOs.ReturnCode._UnknownError
            ' Tratamiento de opcionales
            If Employee Is Nothing Then Employee = String.Empty
            If Project Is Nothing Then Project = String.Empty
            If Task Is Nothing Then Task = String.Empty

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
            Dim oAccrualsResponse As New roDatalinkStandarTaskAccrualResponse

            If resultStatus <> Core.DTOs.ReturnCode._TokenValidation AndAlso externAccessInstance.ValidateToken(Token, HttpContext.Current.Request.UserHostAddress, resultStatus, True) Then
                resultStatus = Core.DTOs.ReturnCode._UnknownError
                Dim oCriteria As New roDatalinkStandarTaskAccrualCriteria
                oAccrualsResponse = New roDatalinkStandarTaskAccrualResponse
                oCriteria.EndAccrualPeriod = EndDate
                oCriteria.StartAccrualPeriod = StartDate
                oCriteria.UniqueEmployeeID = Employee
                oCriteria.Proyecto = Project
                oCriteria.Tarea = Task

                If externAccessInstance.GetTaskAccruals(oCriteria, oAccrualsResponse) Then
                    oWSResponse.Value = oAccrualsResponse.Accruals.ConvertAll(AddressOf XAccrualConverter).ToArray
                    oWSResponse.Status = Convert.ToInt32(oAccrualsResponse.ResultCode)
                    oWSResponse.Text = sResultText
                Else
                    oWSResponse.Value = {}
                    oWSResponse.Status = Convert.ToInt32(oAccrualsResponse.ResultCode)
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

    Private Function XAccrualConverter(oAccrual As roDatalinkStandarTaskAccrual) As roTaskAccrual
        Dim oRet As roTaskAccrual
        Try
            oRet = New roTaskAccrual
            oRet.AccrualDate = New Robotics.VTBase.roWCFDate(oAccrual.AccrualDate.Date)
            oRet.Project = oAccrual.Project
            oRet.Task = oAccrual.Task
            oRet.AccrualValue = oAccrual.AccrualValue
            oRet.IDEmployee = oAccrual.UniqueEmployeeID
        Catch ex As Exception
            oRet = Nothing
        End Try

        Return oRet
    End Function

End Class