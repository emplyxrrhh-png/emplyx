Imports System.IO
Imports System.Net
Imports System.Web.Hosting
Imports System.Web.Mvc
Imports DevExtreme.AspNet.Data
Imports DevExtreme.AspNet.Mvc
Imports Newtonsoft.Json
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTBusiness.Group
Imports Robotics.Base.VTEmployees
Imports Robotics.Security.Base
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions
Imports Robotics.Web.Base

<PermissionsAtrribute(FeatureAlias:="Employees.Surveys", Permission:=Robotics.Base.DTOs.Permission.Read)>
<LoggedInAtrribute(Requiered:=True)>
Public Class SurveysController
    Inherits System.Web.Mvc.Controller
    Private oLanguage As roLanguageWeb

    Function Index() As ActionResult
        If WLHelperWeb.CurrentPassport IsNot Nothing Then

            Dim oLicSupport As New roLicenseSupport()
            Dim oLicInfo As roVTLicense = oLicSupport.GetVTLicenseInfo()

            ViewBag.IsAdvancedEdition = (oLicInfo.Edition <> roServerLicense.roVisualTimeEdition.Starter)
            ViewBag.RootUrl = ConfigurationManager.AppSettings("RootUrl")
            ViewBag.PermissionOverEmployees = Convert.ToInt32(API.SecurityServiceMethods.GetPermissionOverFeature(Nothing, "Employees.Surveys", "U"))

            Try
                LoadInitialData()
            Catch ex As Exception
            End Try
            Return View("Surveys")
        Else
            Return View("NoSession")
        End If
    End Function

    Private Function LoadInitialData() As Boolean
        Try
            Dim availableEmployees As New List(Of EmployeeSelector)
            Dim oEmpState As New Employee.roEmployeeState(WLHelperWeb.CurrentPassportID)
            Dim employeeList = GetSupervisedEmployees(oEmpState)

            Dim oGroupState As New roGroupState(WLHelperWeb.CurrentPassportID, oEmpState.Context, oEmpState.ClientAddress)
            Dim dsGroups As DataSet = roGroup.GetGroups("Calendar", "U", oGroupState)

            Dim groupList = dsGroups.Tables(0).AsEnumerable().[Select](Function(dataRow) New GroupSelector(dataRow.Field(Of Integer)("ID"), dataRow.Field(Of String)("FullGroupName"))).ToList()

            If employeeList.Employees.Length > 0 Then
                Dim empAvailable As EmployeeSelector

                Dim totalAvailable As New List(Of Integer)
                totalAvailable = employeeList.Employees.ToList.Select(Function(x) x.EmployeeId).ToList()

                Dim finalAvailable As EmployeeInfo()
                finalAvailable = employeeList.Employees.ToList.FindAll(Function(y) totalAvailable.ToArray.Contains(y.EmployeeId)).ToArray()
                finalAvailable = finalAvailable.GroupBy(Function(obj) obj.EmployeeId).Select(Function(grupo) grupo.First()).ToArray()

                For Each emp In finalAvailable
                    empAvailable = New EmployeeSelector
                    empAvailable.IdEmployee = emp.EmployeeId
                    empAvailable.EmployeeName = emp.Name
                    empAvailable.Image = emp.Image
                    availableEmployees.Add(empAvailable)
                Next

            End If

            HttpContext.Session("ListOfAvailableEmployees") = availableEmployees

            ViewBag.AvailableEmployees = availableEmployees
            ViewBag.AvailableGroups = groupList
        Catch ex As Exception
            Return False
        End Try
        Return True
    End Function

    Private Function GetSupervisedEmployees(ByVal oEmpState As Employee.roEmployeeState) As EmployeeList
        Dim lrret As New EmployeeList

        Try
            lrret.Status = ErrorCodes.OK
            Dim fileName As String = Path.Combine(HostingEnvironment.ApplicationPhysicalPath, "Base/Images/USER_48.PNG")
            Dim fileStream As New FileStream(fileName, FileMode.Open, FileAccess.Read)

            Dim ImageData As Byte()
            ImageData = New Byte(fileStream.Length - 1) {}
            fileStream.Read(ImageData, 0, System.Convert.ToInt32(fileStream.Length))
            fileStream.Close()

            lrret.DefaultImage = "url(data:image/png;base64," & Convert.ToBase64String(ImageData) & ") no-repeat center center"

            Dim empsList As New Generic.List(Of EmployeeInfo)

            Dim dtSurveys As DataTable = roBusinessSupport.GetAllEmployees("", "Employees", "U", oEmpState, False)

            If dtSurveys IsNot Nothing AndAlso dtSurveys.Rows.Count > 0 Then
                For Each oRow As DataRow In dtSurveys.Rows
                    empsList.Add(New EmployeeInfo With {
                              .EmployeeId = roTypes.Any2Integer(oRow("IDEmployee")),
                              .Name = roTypes.Any2String(oRow("EmployeeName")),
                              .Image = "Employee/GetEmployeePhoto/" & roTypes.Any2String(oRow("IDEmployee"))
                            })

                Next
            End If

            lrret.Employees = empsList.ToArray()
        Catch ex As Exception
            lrret.Status = ErrorCodes.GENERAL_ERROR
            lrret.Employees = {}

            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTPortal::OnBoardingsHelper::GetSupervisedOnBoardings")
        End Try

        Return lrret
    End Function

    Function CreateSurvey() As ActionResult
        Return View("CreateSurvey")
    End Function

    <HttpGet>
    Public Function GetSurveys(ByVal loadOptions As DataSourceLoadOptions) As ActionResult
        Dim oSurveys() As roSurvey
        Dim oEmpState As New Employee.roEmployeeState(-1)

        oSurveys = API.SurveyServiceMethods.GetAllSurveys(Nothing)

        Dim result = DataSourceLoader.Load(oSurveys, loadOptions)
        Dim resultJson = JsonConvert.SerializeObject(result)

        Return Content(resultJson, "application/json")
    End Function

    <HttpPost>
    Public Function GetSurvey(ByVal idSurvey As String) As JsonResult

        Dim oSurvey As roSurvey = API.SurveyServiceMethods.GetSurvey(roTypes.Any2Integer(idSurvey), Nothing)

        If oSurvey.CreatedBy = WLHelperWeb.CurrentPassportID Then
            ViewBag.CurrentEmployeeResponses = oSurvey.CurrentEmployeeResponses.ToList()
            Return Json(oSurvey)
        Else
            Return Json(New roSurvey)
        End If

    End Function

    <HttpPost>
    Public Function GetSurveyTemplates() As JsonResult

        Dim oSurveyTemplates As New roSurveyTemplates
        Dim oSurveyTemplateInfo As New roSurveyTemplateInfo
        Dim oSurveyTemplatesInfo As New List(Of roSurveyTemplateInfo)

        oSurveyTemplates = API.SurveyServiceMethods.GetSurveyTemplates(Nothing, False)

        For Each template In oSurveyTemplates.Templates
            oSurveyTemplateInfo = New roSurveyTemplateInfo
            oSurveyTemplateInfo.name = template.DefaultTitle
            oSurveyTemplateInfo.json = template.Content
            oSurveyTemplatesInfo.Add(oSurveyTemplateInfo)
        Next

        Return Json(oSurveyTemplatesInfo.ToArray)

    End Function

    <HttpPost>
    Public Function GetSurveyResponses(ByVal idSurvey As String) As JsonResult

        Dim oSurveyResponses As New roSurveyResponses

        Dim oSurvey As roSurvey = API.SurveyServiceMethods.GetSurvey(roTypes.Any2Integer(idSurvey), Nothing)
        If oSurvey IsNot Nothing AndAlso oSurvey.CreatedBy = WLHelperWeb.CurrentPassportID Then
            oSurveyResponses = API.SurveyServiceMethods.GetSurveyResponses(roTypes.Any2Integer(idSurvey), Nothing, False)
        End If

        Return Json(oSurveyResponses)

    End Function

    <HttpPost>
    Public Function GetSurveyResponsesByIdEmployee(ByVal idSurvey As String, ByVal idEmployees As Integer()) As JsonResult

        Dim oSurveyResponses As New roSurveyResponses

        Dim oSurvey As roSurvey = API.SurveyServiceMethods.GetSurvey(roTypes.Any2Integer(idSurvey), Nothing)
        If oSurvey IsNot Nothing AndAlso oSurvey.CreatedBy = WLHelperWeb.CurrentPassportID Then
            oSurveyResponses = API.SurveyServiceMethods.GetSurveyResponsesByIdEmployee(roTypes.Any2Integer(idSurvey), idEmployees, Nothing, False)
        End If

        Return Json(oSurveyResponses)

    End Function

    <HttpPost>
    Public Function InsertSurvey(ByVal Id As Integer, ByVal Employees As Integer(), ByVal Groups As Integer(), ByVal survey As String, ByVal title As String, ByVal mandatory As String, ByVal anonymous As String, Optional ByVal ByPercentage As Boolean = False, Optional ByVal ExpirationDate As String = Nothing, Optional ByVal ExpirationPercentage As String = "", Optional ByVal AdvancedFilter As String() = Nothing, Optional ByVal CurrentPercentage As Integer = 0, Optional ByVal CurrentStatus As SurveyStatusEnum = SurveyStatusEnum.Draft, Optional ByVal SurveyMode As String = "Simple") As JsonResult

        Dim bRes As Boolean = True
        Dim totalEmployees As New List(Of Integer)
        Dim totalGroups As New List(Of Integer)

        If (Id <> 0 AndAlso (CurrentStatus = SurveyStatusEnum.Online OrElse CurrentStatus = SurveyStatusEnum.Expired)) Then

            Dim existingSurvey As New roSurvey
            existingSurvey = API.SurveyServiceMethods.GetSurvey(Id, Nothing)

            If existingSurvey.CreatedBy = WLHelperWeb.CurrentPassportID Then
                If ByPercentage = True Then
                    existingSurvey.ResponseMaxPercentage = ExpirationPercentage
                    If existingSurvey.CurrentPercentage >= roTypes.Any2Integer(ExpirationPercentage) Then
                        existingSurvey.Status = SurveyStatusEnum.Expired
                    Else
                        existingSurvey.Status = SurveyStatusEnum.Online
                    End If

                End If
                bRes = API.SurveyServiceMethods.CreateOrUpdateSurvey(Nothing, existingSurvey, True)
            Else
                bRes = False
            End If
        Else
            Dim newSurvey As New roSurvey With {
                .Id = Id,
                .ExpirationDate = DateTime.ParseExact(ExpirationDate, "yyyy-MM-dd", Nothing),
                .IsMandatory = roTypes.Any2Boolean(mandatory),
                .Anonymous = roTypes.Any2Boolean(anonymous),
                .AdvancedMode = True,
                .CreatedBy = WLHelperWeb.CurrentPassportID,
                .Content = survey,
                .Title = title,
                .Employees = Employees,
                .ModifiedOn = Date.Now,
                .Groups = Groups
            }
            If Id = 0 Then newSurvey.CreatedOn = Date.Now
            If ByPercentage = True Then newSurvey.ResponseMaxPercentage = roTypes.Any2Integer(ExpirationPercentage)

            If SurveyMode = "Simple" Then
                newSurvey.AdvancedMode = False
            Else
                newSurvey.AdvancedMode = True
            End If

            bRes = API.SurveyServiceMethods.CreateOrUpdateSurvey(Nothing, newSurvey, True)
        End If

        If bRes Then
            Dim resultSurvey = API.SurveyServiceMethods.GetSurvey(Id, Nothing)
            Return Json(resultSurvey)
        Else
            Return Json(False)
        End If

    End Function

    <HttpPost>
    Public Function CopySurvey(ByVal Id As Integer) As JsonResult

        Dim employees As New List(Of Integer)
        Dim groups As New List(Of Integer)

        Dim survey As New roSurvey
        survey = API.SurveyServiceMethods.GetSurvey(Id, Nothing)

        If survey.CreatedBy = WLHelperWeb.CurrentPassportID Then
            Dim calculatedExpirated As Date = Date.Now.Date.AddDays(survey.ExpirationDate.Date.Subtract(survey.CreatedOn.Date).Days)
            survey.Id = 0
            survey.Title = survey.Title & " - copia"
            survey.CreatedBy = WLHelperWeb.CurrentPassportID
            survey.Employees = employees.ToArray
            survey.Groups = groups.ToArray
            survey.ExpirationDate = calculatedExpirated
            survey.CreatedOn = Date.Now
            survey.ModifiedOn = Date.Now

            survey.Status = SurveyStatusEnum.Draft
            If API.SurveyServiceMethods.CreateOrUpdateSurvey(Nothing, survey, Nothing) Then
                Return Json(True)
            Else
                Return Json(False)
            End If
        Else
            Return Json(False)
        End If

    End Function

    <HttpPost>
    Public Function SendSurvey(ByVal Id As Integer) As JsonResult

        Dim survey As New roSurvey
        survey = API.SurveyServiceMethods.GetSurvey(Id, Nothing)

        If survey.CreatedBy = WLHelperWeb.CurrentPassportID Then
            survey.Status = SurveyStatusEnum.Online
            survey.SentOn = Date.Now

            If API.SurveyServiceMethods.CreateOrUpdateSurvey(Nothing, survey, Nothing) Then
                Return Json(True)
            Else
                Return Json(False)
            End If
        Else
            Return Json(False)
        End If

    End Function

    <HttpPut>
    Public Function UpdateSurvey(ByVal key As Integer, ByVal values As String) As ActionResult

        Dim survey As New roSurvey
        survey = API.SurveyServiceMethods.GetSurvey(key, Nothing)
        If survey.CreatedBy = WLHelperWeb.CurrentPassportID Then
            JsonConvert.PopulateObject(values, survey)
            survey.ModifiedOn = DateTime.Now

            If API.SurveyServiceMethods.CreateOrUpdateSurvey(Nothing, survey, True) Then
                Return New HttpStatusCodeResult(HttpStatusCode.OK)
            Else
                Return New HttpStatusCodeResult(HttpStatusCode.BadRequest)
            End If
        Else
            Return New HttpStatusCodeResult(HttpStatusCode.BadRequest)
        End If

    End Function

    <HttpDelete>
    Public Function DeleteSurvey(ByVal key As Integer) As ActionResult
        Dim List As New roSurvey()
        List.Id = key

        Dim tmpSurvey As roSurvey = API.SurveyServiceMethods.GetSurvey(key, Nothing)
        If tmpSurvey.CreatedBy = WLHelperWeb.CurrentPassportID Then

            If API.SurveyServiceMethods.DeleteSurvey(List, Nothing) Then
                Return New HttpStatusCodeResult(HttpStatusCode.OK)
            Else
                Return New HttpStatusCodeResult(HttpStatusCode.BadRequest)
            End If
        Else
            Return New HttpStatusCodeResult(HttpStatusCode.BadRequest)
        End If

    End Function

    Public Function GetServerLanguage() As roLanguageWeb
        If Me.oLanguage Is Nothing Then
            Me.oLanguage = New roLanguageWeb
            WLHelperWeb.SetLanguage(Me.oLanguage, "LiveGUI")
        End If
        Return Me.oLanguage
    End Function

End Class