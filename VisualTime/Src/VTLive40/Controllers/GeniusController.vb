Imports System.IO
Imports System.Web.Hosting
Imports System.Web.Mvc
Imports DevExtreme.AspNet.Data
Imports DevExtreme.AspNet.Mvc
Imports Newtonsoft.Json
Imports Robotics.Base.DTOs
Imports Robotics.Base.DTOs.UserFieldsTypes
Imports Robotics.Base.VTBusiness.BusinessCenter
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTBusiness.Group
Imports Robotics.Base.VTEmployees
Imports Robotics.Security.Base
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions
Imports Robotics.Web.Base
Imports Robotics.Web.Base.API
Imports VTLive40.Controllers.Base

<PermissionsAtrribute(FeatureAlias:="Calendar,Tasks.Definition,Access.Analytics,BusinessCenters.Definition", Permission:=Robotics.Base.DTOs.Permission.Read)>
<LoggedInAtrribute(Requiered:=True)>
Public Class GeniusController
    Inherits BaseController

    Private requieredLabels = {"Title", "SaveChanges", "UndoChanges", "DataModified", "OldGenius", "MainSection", "RunGenius", "Results", "LastRunsInfo", "RunDesc", "shareView", "whoShares",
        "selectUser", "shareDescription", "whoAnalyze", "selectUsers", "roAnalyzeGroups", "roAnalyzeUsers", "roSurveyUsers", "roUsersAdvanced", "createReport", "reportName", "dataType",
        "costCenterSelector", "conceptsSelector", "requestSelector", "lblUserFields", "txtUserFields", "lblConfLanguage", "lblgeniusviewcreate", "costCenterLbl", "selectCostCenter", "conceptsLbl",
        "selectConcepts", "requestsLbl", "selectRequests", "lblConvert2SystemView", "timeLbl", "generalGenius", "geniusPlanification", "TitleDesc", "scheduleReport", "deletePlanning",
        "deletePlanningHeader", "empty", "loading", "planningDescription", "planningPeriod", "nextPlanning", "configScheduler", "scheduleDaily", "scheduleWeekly", "each", "days", "at", "weeks",
        "monday", "tuesday", "wednesday", "thursday", "friday", "saturday", "sunday", "theDay", "ofEachMonth", "the", "first", "second", "third", "fourth", "last", "scheduleMonthly",
        "scheduleOnce", "scheduleInterval", "hours", "planScheduler", "from", "to", "optPeriodOther", "optPeriodTomorrow", "optPeriodToday", "optPeriodYesterday", "optPeriodCurrentWeek",
        "optPeriodLastWeek", "optPeriodCurrentMonth", "optPeriodLastMonth", "optPeriodCurrentYear", "optPeriodNextWeek", "optPeriodNextMonth", "ckIncludeConceptsWithZeros", "ckSendEmail",
        "ckDownloadBI", "ckOverwriteResults", "ckIncludeAllCauses", "ckSelectCauses", "causesLbl", "ckIncludeCausesWithZeros", "selectCauses", "allCausesSelected", "someCausesSelected", "causeSelected", "ckIncludeEntriesWithoutBusinessCenter", "lblDefinePeriod", "lblDefinePeriodFrom", "lblDefinePeriodTo",
        "selectUserFields"}

    Function Index() As ActionResult

        Me.InitializeBase(CardTreeTypes.Genius, TabBarButtonTypes.Genius, "Genius", requieredLabels, "LiveGenius") _
                          .SetBarButton(BarButtonTypes.Genius) _
                          .SetViewInfo("LiveGenius", "Genius", "Title", "Title", "Base/Images/Genius/GeniusAnalytics80.png", "TitleDesc")

#If DEBUG Then
        ViewBag.LCode = "Z7LE-XI104O-2L0I6K-1W1U56-2S6L5M-2N4Y07-2K0L37-160T5O-1O4U08-1M1X0S-2T1G45-1M3O0M-3417"
#Else
        ViewBag.LCode = API.CommonServiceMethods.GetRuntimeId()
#End If

        LoadViewData(-1)
        Try
            LoadInitialData()
            LoadCheckboxes()
            LoadBusinessCentersList()
            LoadConcepts()
            LoadRequests()
            LoadUserFields()
            LoadLanguageCombo()
            If WLHelperWeb.CurrentUserIsConsultantOrCegid Then
                ViewBag.IsConsultor = True
            Else
                ViewBag.IsConsultor = False
            End If

            Dim oLicSupport As New roLicenseSupport()
            Dim oLicInfo As roVTLicense = oLicSupport.GetVTLicenseInfo()

            ViewBag.AdvancedGenius = True
            If oLicInfo.Edition = roServerLicense.roVisualTimeEdition.Starter Then
                ViewBag.AdvancedGenius = False
            End If

            ViewBag.BIIntegration = HelperSession.GetFeatureIsInstalledFromApplication("Feature\BIIntegration")
            ViewBag.PlannedGenius = New List(Of roGeniusScheduler)
            ViewBag.Format = HelperWeb.GetShortDateFormat()

        Catch ex As Exception
        End Try

        Return View("index")
    End Function

    Function LoadView(id As Integer) As ActionResult
        LoadViewData(id)
        ViewBag.BIIntegration = HelperSession.GetFeatureIsInstalledFromApplication("Feature\BIIntegration")

        Return View("index")
    End Function

    Private Sub LoadViewData(id)
        Me.InitializeBase(CardTreeTypes.Genius, TabBarButtonTypes.Genius, "Genius", requieredLabels, "LiveGenius") _
                          .SetBarButton(BarButtonTypes.Genius) _
                          .SetViewInfo("LiveGenius", "Genius", "Title", "Title", "Base/Images/Genius/GeniusAnalytics80.png", "TitleDesc")

#If DEBUG Then
        ViewBag.LCode = "Z7LE-XI104O-2L0I6K-1W1U56-2S6L5M-2N4Y07-2K0L37-160T5O-1O4U08-1M1X0S-2T1G45-1M3O0M-3417"
#Else
        ViewBag.LCode = API.CommonServiceMethods.GetRuntimeId()
#End If
        Dim availableEmployees As New List(Of roAutocompleteItemModel)

        availableEmployees.AddRange(API.SecurityV3ServiceMethods.GetAllAvailableSupervisorsList(Nothing).Where(Function(x) x.ID <> WLHelperWeb.CurrentPassportID).Select(Function(user) New roAutocompleteItemModel() With {
                                                        .ID = user.ID,
                                                        .Name = user.Name}))

        ViewBag.SelectedTask = id
        ViewBag.SharePassports = availableEmployees

        If ViewBag.AnalyticsType = Nothing Then
            LoadCheckboxes()
        End If
        If ViewBag.LstUserFields = Nothing Then
            LoadUserFields()
        End If
        If ViewBag.LstCostCenters = Nothing Then
            LoadBusinessCentersList()
        End If
        If ViewBag.lstCauses = Nothing Then
            LoadCauses()
        End If
        If ViewBag.lstConcepts = Nothing Then
            LoadConcepts()
        End If
        If ViewBag.lstRequests = Nothing Then
            LoadRequests()
        End If

        If ViewBag.LstLanguages = Nothing Then
            LoadLanguageCombo()
        End If

        If ViewBag.AvailableGroups = Nothing Then
            LoadInitialData()
        End If

    End Sub

#Region "Private methods"

    Private ReadOnly Property GetGeniusViewTemplates(Optional ByVal bolReload As Boolean = False) As List(Of roGeniusView)
        Get
            Dim tbRequestTypes As List(Of roGeniusView) = Session("AnalyticsSchedule_GeniusTemplates")

            If bolReload OrElse tbRequestTypes Is Nothing Then
                tbRequestTypes = API.GeniusServiceMethods.GetGeniusViewsTemplates(Nothing)

                Session("AnalyticsSchedule_GeniusTemplates") = tbRequestTypes
            End If

            Return tbRequestTypes
        End Get
    End Property

    Private ReadOnly Property GetGeniusCheckboxes(Optional ByVal bolReload As Boolean = False) As List(Of roGeniusCheckbox)
        Get
            Dim tbRequestTypes As List(Of roGeniusCheckbox) = New List(Of roGeniusCheckbox)()

            tbRequestTypes = API.GeniusServiceMethods.GetGeniusCheckboxes(Nothing)

            Return tbRequestTypes
        End Get
    End Property

    Private Sub LoadCauses()
        Dim tbCauses As DataTable
        tbCauses = API.CausesServiceMethods.GetCauses(Nothing, "", True)
        Dim lstCauses As List(Of roAutocompleteItemModel) = New List(Of roAutocompleteItemModel)()
        ViewBag.tbCauses = tbCauses
        For Each concept As DataRow In tbCauses.Rows
            lstCauses.Add(New roAutocompleteItemModel() With {
                                                            .ID = concept("ID"),
                                                            .Name = concept("Name")})
        Next
        ViewBag.lstCauses = lstCauses
    End Sub

    Private Sub LoadConcepts()
        Dim tbConcepts As DataTable
        tbConcepts = API.ConceptsServiceMethods.GetConceptList(Nothing, True)
        Dim lstConcepts As List(Of roAutocompleteItemModel) = New List(Of roAutocompleteItemModel)()
        ViewBag.TbConcepts = tbConcepts
        For Each concept As DataRow In tbConcepts.Rows
            lstConcepts.Add(New roAutocompleteItemModel() With {
                                                            .ID = concept("ID"),
                                                            .Name = concept("Name")})
        Next
        ViewBag.lstConcepts = lstConcepts
    End Sub

    Private Sub LoadUserFields()
        Dim tbUserFields As DataTable
        Dim lstUserFields As List(Of roAutocompleteItemModel) = New List(Of roAutocompleteItemModel)()
        tbUserFields = API.UserFieldServiceMethods.GetUserFields(Nothing, Types.EmployeeField, "Used=1", False)
        For Each userField As DataRow In tbUserFields.Rows
            lstUserFields.Add(New roAutocompleteItemModel() With {
                                                            .ID = userField("ID"),
                                                            .Name = userField("FieldName")})
        Next
        ViewBag.LstUserFields = lstUserFields
    End Sub

    Private Sub LoadLanguageCombo()
        Dim oAllLanguages As roPassportLanguage() = API.UserAdminServiceMethods.GetLanguages(Nothing)
        Dim oLanguages As List(Of LanguageType) = New List(Of LanguageType)()
        Dim bolInstalled As Boolean = False

        For Each oLanguage As roPassportLanguage In oAllLanguages
            bolInstalled = IIf(oLanguage.Key = "ESP", True, False)
            If oLanguage.Installed Or bolInstalled Then
                oLanguages.Add(New LanguageType(oLanguage.Key, GetServerLanguage().Translate("Language." & oLanguage.Key, "GeniusView")))
            End If
        Next
        ViewBag.LstLanguages = oLanguages
    End Sub

    Private Sub LoadRequests()
        Dim tbRequestTypes As DataTable
        tbRequestTypes = API.RequestServiceMethods.GetRequestTypes(Nothing)
        ViewBag.TbRequests = tbRequestTypes
        Dim lstRequests As List(Of roAutocompleteItemModel) = New List(Of roAutocompleteItemModel)()
        For Each request As DataRow In tbRequestTypes.Rows
            lstRequests.Add(New roAutocompleteItemModel() With {
                                                            .ID = request("ElementID"),
                                                            .Name = request("ElementDesc")})
        Next
        ViewBag.lstRequests = lstRequests
    End Sub

    Private Sub LoadBusinessCentersList()
        Dim lstPassportCenters = New List(Of roBusinessCenter)
        Dim tbBusinessCenter As DataTable
        Dim idPassport As Integer
        idPassport = WLHelperWeb.CurrentPassportID
        Dim oPassport As roPassport = API.UserAdminServiceMethods.GetPassport(Nothing, roTypes.Any2Integer(idPassport), LoadType.Passport)
        Dim IDRelatedObject As Integer
        If idPassport = WLHelperWeb.CurrentPassportID Then
            IDRelatedObject = oPassport.IDParentPassport
        Else
            IDRelatedObject = oPassport.ID
        End If

        tbBusinessCenter = TasksServiceMethods.GetBusinessCenterByPassportDataTable(Nothing, roTypes.Any2Integer(idPassport), False)
        lstPassportCenters = GetBusinessCenterFromDataTable(tbBusinessCenter)
        ViewBag.LstCostCenters = lstPassportCenters
    End Sub

    Private Function GetBusinessCenterFromDataTable(oCentersDt As DataTable) As List(Of roBusinessCenter)
        Dim oCentersList As New List(Of roBusinessCenter)
        For Each oCenterRow In oCentersDt.Rows
            Dim oCenter = New roBusinessCenter
            oCenter.Name = roTypes.Any2String(oCenterRow("Name"))
            oCenter.ID = roTypes.Any2Integer(oCenterRow("ID"))
            oCenter.Field1 = roTypes.Any2String(oCenterRow("Field1"))
            oCenter.Field2 = roTypes.Any2String(oCenterRow("Field2"))
            oCenter.Field3 = roTypes.Any2String(oCenterRow("Field3"))
            oCenter.Field4 = roTypes.Any2String(oCenterRow("Field4"))
            oCenter.Field5 = roTypes.Any2String(oCenterRow("Field5"))
            oCentersList.Add(oCenter)
        Next
        Return oCentersList
    End Function

    Private Sub LoadCheckboxes()

        Dim tb As List(Of roGeniusCheckbox) = GetGeniusCheckboxes()

        ViewBag.AnalyticsType = tb
    End Sub

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

    Private Function LoadInitialData() As Boolean
        Try
            Dim availableEmployees As New List(Of EmployeeSelector)
            Dim oEmpState As New Employee.roEmployeeState(WLHelperWeb.CurrentPassport.ID)
            Dim employeeList = GetSupervisedEmployees(oEmpState)

            Dim oGroupState As New roGroupState(WLHelperWeb.CurrentPassport.ID, oEmpState.Context, oEmpState.ClientAddress)
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

#End Region

    <HttpGet>
    Public Function GetPlanningList(ByVal loadOptions As DataSourceLoadOptions, ByVal idGeniusView As String) As ActionResult
        If idGeniusView IsNot Nothing And idGeniusView <> "" Then

            Dim oView As roGeniusView = API.GeniusServiceMethods.GetGeniusViewById(Nothing, idGeniusView, False)

            If oView.IdPassport = WLHelperWeb.CurrentPassportID Then
                Dim planningList As List(Of roGeniusScheduler) = API.GeniusServiceMethods.GetUserGeniusPlanifications(Nothing, idGeniusView, WLHelperWeb.CurrentPassportID)

                If planningList IsNot Nothing Then
                    Dim result = DataSourceLoader.Load(planningList, loadOptions)
                    Dim resultJson = JsonConvert.SerializeObject(result)
                    Return Content(resultJson, "application/json")
                Else
                    Return Nothing
                End If
            End If
        End If

        Return Nothing

    End Function

    <HttpPost>
    Public Function InsertNewPlanning(ByVal key As String, ByVal values As String, ByVal idGeniusView As String, ByVal schedulerText As String, ByVal scheduledParam As String, ByVal periodType As String, ByVal beginPeriod As String, ByVal endPeriod As String) As ActionResult
        Dim Planning As New roGeniusScheduler()
        JsonConvert.PopulateObject(values, Planning)
        Planning.IDGeniusView = idGeniusView
        Planning.IDPassport = WLHelperWeb.CurrentPassportID
        Planning.Scheduler = roReportSchedulerScheduleManager.Load(scheduledParam)
        Planning.SchedulerText = schedulerText
        Planning.PeriodType = periodType
        If beginPeriod IsNot Nothing Then
            Planning.ScheduleBeginDate = Convert.ToDateTime(beginPeriod)
        End If
        If endPeriod IsNot Nothing Then
            Planning.ScheduleEndDate = Convert.ToDateTime(endPeriod)
        End If
        API.GeniusServiceMethods.SaveGeniusPlanification(Nothing, Planning)
        Return Json(Planning)
    End Function

    <HttpPost>
    Public Function UpdatePlanning(ByVal key As String, ByVal values As String, ByVal schedulerText As String, ByVal scheduledParam As String, ByVal periodType As String, ByVal beginPeriod As String, ByVal endPeriod As String) As ActionResult
        Dim updatedPlanning As New roGeniusScheduler()
        JsonConvert.PopulateObject(values, updatedPlanning)
        Dim initialPlanning = API.GeniusServiceMethods.GetGeniusPlanificationById(Nothing, key, False)
        If updatedPlanning.Name IsNot Nothing Then
            initialPlanning.Name = updatedPlanning.Name
        End If
        If scheduledParam IsNot Nothing Then
            initialPlanning.Scheduler = roReportSchedulerScheduleManager.Load(scheduledParam)
        End If
        If schedulerText IsNot Nothing Then
            initialPlanning.SchedulerText = schedulerText
        End If
        initialPlanning.PeriodType = periodType
        If beginPeriod IsNot Nothing Then
            initialPlanning.ScheduleBeginDate = Convert.ToDateTime(beginPeriod)
        End If
        If endPeriod IsNot Nothing Then
            initialPlanning.ScheduleEndDate = Convert.ToDateTime(endPeriod)
        End If
        API.GeniusServiceMethods.SaveGeniusPlanification(Nothing, initialPlanning)
        Return Json(initialPlanning)
    End Function

    <HttpDelete>
    Public Function DeletePlanning(ByVal key As String, ByVal values As String, ByVal idGeniusView As String) As ActionResult
        Dim Planning = API.GeniusServiceMethods.GetGeniusPlanificationById(Nothing, key, False)

        If Planning.IDPassport = WLHelperWeb.CurrentPassportID Then
            Return Json(API.GeniusServiceMethods.DeleteGeniusPlanification(Nothing, Planning))
        Else
            Return Json(False)
        End If

    End Function

    <HttpPost>
    Function GetGeniusView(id As Integer) As JsonResult
        Dim oView As roGeniusView = API.GeniusServiceMethods.GetGeniusViewById(Nothing, id, False)

        If oView.IdPassport = 0 OrElse oView.IdPassport = WLHelperWeb.CurrentPassportID Then
            Return Json(oView)
        Else
            Return Json(Nothing)
        End If
    End Function

    <HttpPost>
    Function GetGeniusViewSharedList(id As Integer) As JsonResult
        Return Json(API.GeniusServiceMethods.GetGeniusViewSharedListById(Nothing, id, False))
    End Function

    <HttpPost>
    Function runGeniusView(geniusView As roGeniusView) As JsonResult

        If geniusView.IdPassport = WLHelperWeb.CurrentPassportID Then
            Dim oView As roGeniusView = API.GeniusServiceMethods.GetGeniusViewById(Nothing, geniusView.Id, False)
            If oView IsNot Nothing AndAlso (oView.Employees Is Nothing OrElse oView.Employees = "") Then
                API.GeniusServiceMethods.SaveGeniusView(Nothing, geniusView)
            End If
            Return Json(API.GeniusServiceMethods.ExecuteGeniusView(Nothing, geniusView))
        Else
            Return Json(Nothing)
        End If
    End Function

    <HttpPost>
    Function ShareGeniusView(idView As Integer, users As Array) As JsonResult
        Dim obj = API.GeniusServiceMethods.GetGeniusViewById(Nothing, idView, False)
        If obj IsNot Nothing AndAlso obj.IdPassport = WLHelperWeb.CurrentPassportID Then
            If API.GeniusServiceMethods.ShareGeniusView(Nothing, obj, WLHelperWeb.CurrentPassportID, users) Then
                Return Json(True)
            Else
                Return Json(False)
            End If
        Else
            Return Json(False)
        End If

    End Function

    <HttpPost>
    Function ConvertGeniusView(idView As Integer, idPassport As Integer) As JsonResult
        Dim obj = API.GeniusServiceMethods.GetGeniusViewById(Nothing, idView, False)
        obj.BusinessCenters = ""
        obj.Concepts = ""
        obj.Causes = ""
        If obj IsNot Nothing Then
            If API.GeniusServiceMethods.ShareGeniusView(Nothing, obj, idPassport) Then
                Return Json(True)
            Else
                Return Json(False)
            End If
        Else
            Return Json(False)
        End If

    End Function

    <HttpPost>
    Function getGeniusViewStatus(id As Integer) As JsonResult
        Dim obj As roGeniusExecution = API.GeniusServiceMethods.GetGeniusExecutionById(Nothing, id)

        If obj IsNot Nothing Then
            Dim geniusView As roGeniusView = API.GeniusServiceMethods.GetGeniusViewById(Nothing, obj.IdGeniusView, False)
            If geniusView IsNot Nothing AndAlso geniusView.IdPassport = WLHelperWeb.CurrentPassportID AndAlso obj.FileLink <> String.Empty AndAlso obj.FileLink <> "Error" Then
                Return Json(True)
            Else
                Return Json(False)
            End If
        Else
            Return Json(False)
        End If

    End Function

    <HttpPost>
    Function GetAzureFileInfo(id As Integer) As JsonResult
        Dim obj = API.GeniusServiceMethods.GetGeniusExecutionWithSasKeyById(Nothing, id)

        If obj IsNot Nothing Then
            Dim geniusView As roGeniusView = API.GeniusServiceMethods.GetGeniusViewById(Nothing, obj.IdGeniusView, False)
            If geniusView IsNot Nothing AndAlso geniusView.IdPassport = WLHelperWeb.CurrentPassportID AndAlso obj.AzureSaSKey <> String.Empty Then
                Return Json(obj)
            Else
                Return Json(False)
            End If
        Else
            Return Json(False)
        End If

    End Function

    <HttpDelete>
    Function DeleteGeniusView(geniusView As roGeniusView) As JsonResult
        If geniusView.IdPassport = WLHelperWeb.CurrentPassportID Then
            Return Json(API.GeniusServiceMethods.DeleteGeniusView(Nothing, geniusView))
        Else
            Return Json(False)
        End If

    End Function

    <HttpPut>
    Function updateGeniusView(geniusExecution As roGeniusExecution) As JsonResult
        Dim geniusView As roGeniusView = API.GeniusServiceMethods.GetGeniusViewById(Nothing, geniusExecution.IdGeniusView, False)

        If geniusView IsNot Nothing AndAlso geniusView.IdPassport = WLHelperWeb.CurrentPassportID Then
            Return Json(API.GeniusServiceMethods.UpdateGeniusViewLayout(Nothing, geniusExecution))
        Else
            Return Json(False)
        End If
    End Function

    <HttpPost>
    Function saveGeniusView(geniusView As roGeniusView) As JsonResult
        geniusView.initialize()

        If geniusView.IdPassport = WLHelperWeb.CurrentPassportID Then
            Return Json(API.GeniusServiceMethods.SaveGeniusView(Nothing, geniusView))
        Else
            Return Json(False)
        End If

    End Function

    <HttpPost>
    Function saveCustomView(geniusView As roGeniusView) As JsonResult
        Dim geniusViewBase As roGeniusView = API.GeniusServiceMethods.GetGeniusViewByCheckBoxes(Nothing, geniusView.CheckedCheckBoxes, True)
        If geniusViewBase.Id <> -1 Then
            geniusView.initialize(WLHelperWeb.CurrentPassportID, geniusViewBase, GetServerLanguage.LanguageKey)
            UpdateCubeLayout(geniusView)
            API.GeniusServiceMethods.SaveGeniusView(Nothing, geniusView)
            Return Json(geniusView)
        Else
            Throw New Exception()
        End If
    End Function

    <HttpPost>
    Function createGeniusView(geniusView As roGeniusView) As JsonResult
        geniusView.initialize(WLHelperWeb.CurrentPassportID, geniusView, GetServerLanguage().LanguageKey)
        UpdateCubeLayout(geniusView)
        API.GeniusServiceMethods.SaveGeniusView(Nothing, geniusView)
        Return Json(geniusView)
    End Function

    <HttpPost>
    Function GetGeniusByTask(id As Integer) As JsonResult
        Dim oView As roGeniusView = API.GeniusServiceMethods.GetGeniusViewByTask(Nothing, id, False)

        If oView.IdPassport = WLHelperWeb.CurrentPassportID Then
            Return Json(oView)
        Else
            Return Json(False)
        End If

    End Function

    <HttpDelete>
    Function DeleteGeniusBIExecution(geniusExecutionID As Integer, fileName As String) As JsonResult
        Dim oExecution As roGeniusExecution = API.GeniusServiceMethods.GetGeniusExecutionById(Nothing, geniusExecutionID)

        If oExecution IsNot Nothing Then

            Dim geniusView As roGeniusView = API.GeniusServiceMethods.GetGeniusViewById(Nothing, oExecution.IdGeniusView, False)
            If geniusView IsNot Nothing AndAlso geniusView.IdPassport = WLHelperWeb.CurrentPassportID Then
                Return Json(API.GeniusServiceMethods.DeleteGeniusBIExecution(Nothing, geniusExecutionID, fileName))
            Else
                Return Json(False)
            End If
        Else
            Return Json(False)
        End If

    End Function

    Sub UpdateCubeLayout(ByVal geniusView As roGeniusView)

        Dim jsonObject As Newtonsoft.Json.Linq.JObject = CType(Newtonsoft.Json.JsonConvert.DeserializeObject(geniusView.CubeLayout), Newtonsoft.Json.Linq.JObject)

        If jsonObject IsNot Nothing AndAlso jsonObject.Count > 0 AndAlso jsonObject("slice")("measures") IsNot Nothing AndAlso jsonObject("slice")("measures").Count > 0 Then
            For i As Integer = 0 To jsonObject("slice")("measures").Count - 1
                Dim measure As Newtonsoft.Json.Linq.JObject = jsonObject("slice")("measures")(i)
                If measure("caption") Is Nothing Then
                    Dim translation = GetServerLanguage("LiveGenius").Translate("measure." & measure("uniqueName").ToString() & ".rotext", "")
                    If translation <> "NotFound" Then
                        measure("caption") = translation
                        jsonObject("slice")("measures")(i) = measure
                    End If
                End If
            Next
            geniusView.CubeLayout = jsonObject.ToString(Formatting.None)
        End If
    End Sub

End Class

Class LanguageType

    Public Sub New(ID As String, Name As String)
        Me.ID = ID
        Me.Name = Name
    End Sub

    Public Property Name As String
    Public Property ID As String
End Class