Imports System.Drawing
Imports System.IO
Imports System.Web.Mvc
Imports System.Windows.Input
Imports DevExpress.XtraSpreadsheet.Model
Imports Microsoft.Ajax.Utilities
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTEmployees.Employee
Imports Robotics.VTBase
Imports Robotics.Web.Base
Imports VTLive40.Controllers.Base

<PermissionsAtrribute(FeatureAlias:="Employees,Calendar", Permission:=Robotics.Base.DTOs.Permission.Read)>
<LoggedInAtrribute(Requiered:=True)>
Public Class EmployeeController
    Inherits BaseController


    <HttpGet>
    <PermissionsAtrribute(FeatureAlias:="Employees", Permission:=Robotics.Base.DTOs.Permission.Read)>
    Function GetEmployeeTreeSelectionPath(id As Integer) As JsonResult

        Dim oEmployee As roEmployee = Nothing

        Dim bEmpPermission As Boolean = API.SecurityServiceMethods.HasPermissionOverEmployee(Nothing, id, "Employees", "U", Permission.Read)
        If Not bEmpPermission Then Return Json(False, JsonRequestBehavior.AllowGet)

        oEmployee = API.EmployeeServiceMethods.GetEmployee(Nothing, id, False)
        If oEmployee IsNot Nothing Then

            Dim employeeInfo As roEmployeeSelectionPath = API.EmployeeServiceMethods.GetEmployeeSelectionPath(Nothing, id)
            If employeeInfo IsNot Nothing Then
                Return Json(employeeInfo, JsonRequestBehavior.AllowGet)
            Else
                Return Json(API.EmployeeServiceMethods.LastErrorText, JsonRequestBehavior.AllowGet)
            End If
        Else
            Return Json(False, JsonRequestBehavior.AllowGet)
        End If
    End Function

    <HttpGet>
    <PermissionsAtrribute(FeatureAlias:="Employees", Permission:=Robotics.Base.DTOs.Permission.Read)>
    Function GetEmployeePhoto(id As Integer) As ActionResult

        Dim oEmployee As roEmployee = Nothing
        Dim oImg = Nothing

        Dim bEmpPermission As Boolean = API.SecurityServiceMethods.HasPermissionOverEmployee(Nothing, id, "Employees", "U", Permission.Read)
        If Not bEmpPermission Then Return HttpNotFound()

        If API.SecurityServiceMethods.HasPermissionOverEmployee(Nothing, id, "Employees.NameFoto", "U", Permission.Read) Then oEmployee = API.EmployeeServiceMethods.GetEmployee(Nothing, id, False)
        If oEmployee IsNot Nothing Then oImg = oEmployee.Image

        Try
            Return File(LoadEmployeeImage(oImg), "image/png")
        Catch ex As Exception
            Return HttpNotFound()
        End Try

    End Function

    <HttpGet>
    <PermissionsAtrribute(FeatureAlias:="Employees", Permission:=Robotics.Base.DTOs.Permission.Read)>
    Function GetSupervisorPhoto(id As Integer) As ActionResult

        Dim oEmployee As roEmployee = Nothing
        Dim oImg = Nothing

        Dim tmpPassport As roPassport = API.UserAdminServiceMethods.GetPassport(Nothing, id, LoadType.Passport, False)

        If tmpPassport IsNot Nothing AndAlso tmpPassport.IDEmployee > 0 Then
            If tmpPassport.ID = WLHelperWeb.CurrentPassportID OrElse API.SecurityServiceMethods.HasPermissionOverEmployee(Nothing, tmpPassport.IDEmployee, "Employees", "U", Permission.Read) Then
                If tmpPassport.ID = WLHelperWeb.CurrentPassportID OrElse API.SecurityServiceMethods.HasPermissionOverEmployee(Nothing, tmpPassport.IDEmployee, "Employees.NameFoto", "U", Permission.Read) Then
                    oEmployee = API.EmployeeServiceMethods.GetEmployee(Nothing, tmpPassport.IDEmployee, False)
                End If
            Else
                Return HttpNotFound()
            End If
        Else
            Return HttpNotFound()
        End If

        If oEmployee IsNot Nothing Then oImg = oEmployee.Image

        Try
            Return File(LoadEmployeeImage(oImg), "image/png")
        Catch ex As Exception
            Return HttpNotFound()
        End Try

    End Function

    <HttpGet>
    <PermissionsAtrribute(FeatureAlias:="Employees", Permission:=Robotics.Base.DTOs.Permission.Read)>
    Function GetEmployeeLargePhoto(id As Integer) As ActionResult

        Try
            Dim oEmployee As roEmployee = Nothing

            Dim bEmpPermission As Boolean = API.SecurityServiceMethods.HasPermissionOverEmployee(Nothing, id, "Employees", "U", Permission.Read)
            If Not bEmpPermission Then Return HttpNotFound()


            If API.SecurityServiceMethods.HasPermissionOverEmployee(Nothing, id, "Employees.NameFoto", "U", Permission.Read) Then
                oEmployee = API.EmployeeServiceMethods.GetEmployee(Nothing, id, False)
                Return File(LoadEmployeeLargeImage(oEmployee.Image), "image/png")
            Else
                Return File(LoadEmployeeLargeImage(Nothing), "image/png")
            End If
        Catch ex As Exception
            Return HttpNotFound()
        End Try

    End Function

    Private requieredSelectorLabels = {"Title", "TitleDesc", "roDestination", "roGroupsDestination", "roEmployeeDestination", "advanced", "rogroupplaceholder", "rogroupplaceholderlarge", "roemployeeplaceholder", "roemployeeplaceholderlarge",
        "rocollectivedestination", "rocollectiveplaceholder", "rocollectiveplaceholderlarge", "rolabagreedestination", "rolabagreeplaceholder", "rolabagreeplaceholderlarge", "union", "intersection", "advancedfilterincluded",
        "selectall", "selectcustom", "selectnone"}

    <HttpGet>
    <PermissionsAtrribute(FeatureAlias:="Employees", Permission:=Robotics.Base.DTOs.Permission.Read)>
    Function EmployeeSelector(ByVal feature As String, ByVal pageName As String, ByVal config As String, advancedMode As String, advancedFilter As String, unionType As String, allowAll As String, allowNone As String) As ActionResult
        Me.InitializeBase(CardTreeTypes.EmployeeSelector, TabBarButtonTypes.EmployeeSelector, "EmployeeSelector", requieredSelectorLabels, "LivePortal") _
                          .SetBarButton(BarButtonTypes.EmployeeSelector) _
                          .SetViewInfo("LivePortal", "EmployeeSelector", "Title", "Title", "", "TitleDesc")

        Try
            If Not String.IsNullOrEmpty(pageName) Then LoadDefaultTranslations(pageName)

            FillCollectivesViewBag(config)

            FillLabagreesViewBag(config)

            FillGroupsViewBag(feature, config)

            FillEmployeesViewBag(feature, config)

            ViewBag.unionType = If(String.IsNullOrEmpty(unionType), "or", unionType.ToLower())
            ViewBag.allowSelectAll = (Not String.IsNullOrEmpty(allowAll) AndAlso roTypes.Any2Boolean(allowAll))
            ViewBag.allowSelectNone = (Not String.IsNullOrEmpty(allowNone) AndAlso roTypes.Any2Boolean(allowNone))
            ViewBag.enableAdvancedMode = (Not String.IsNullOrEmpty(advancedMode) AndAlso roTypes.Any2Boolean(advancedMode))
            ViewBag.advancedFilterEnabled = (ViewBag.enableAdvancedMode AndAlso (Not String.IsNullOrEmpty(advancedFilter) AndAlso roTypes.Any2Boolean(advancedFilter)))
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "EmployeeController::EmployeeSelector::" & ex.Message)
        End Try

        Return View("Selector")
    End Function

    <HttpGet>
    <PermissionsAtrribute(FeatureAlias:="Employees", Permission:=Robotics.Base.DTOs.Permission.Read)>
    Function EmployeeSelectorPartial(ByVal feature As String, ByVal pageName As String, ByVal config As String, advancedMode As String, advancedFilter As String, unionType As String, allowAll As String, allowNone As String) As ActionResult
        Me.InitializeBase(CardTreeTypes.EmployeeSelector, TabBarButtonTypes.EmployeeSelector, "EmployeeSelector", requieredSelectorLabels, "LivePortal") _
                          .SetBarButton(BarButtonTypes.EmployeeSelector) _
                          .SetViewInfo("LivePortal", "EmployeeSelector", "Title", "Title", "", "TitleDesc")



        Try
            If Not String.IsNullOrEmpty(pageName) Then LoadDefaultTranslations(pageName)

            LoadDefaultTranslations(pageName)

            FillCollectivesViewBag(config)

            FillLabagreesViewBag(config)

            FillGroupsViewBag(feature, config)

            FillEmployeesViewBag(feature, config)

            ViewBag.RequieredFeature = feature
            ViewBag.Origin = pageName
            ViewBag.unionType = If(String.IsNullOrEmpty(unionType), "or", unionType.ToLower())
            ViewBag.allowSelectAll = (Not String.IsNullOrEmpty(allowAll) AndAlso roTypes.Any2Boolean(allowAll))
            ViewBag.allowSelectNone = (Not String.IsNullOrEmpty(allowNone) AndAlso roTypes.Any2Boolean(allowNone))
            ViewBag.enableAdvancedMode = (Not String.IsNullOrEmpty(advancedMode) AndAlso roTypes.Any2Boolean(advancedMode))
            ViewBag.advancedFilterEnabled = (ViewBag.enableAdvancedMode AndAlso (Not String.IsNullOrEmpty(advancedFilter) AndAlso roTypes.Any2Boolean(advancedFilter)))
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "EmployeeController::EmployeeSelector::" & ex.Message)
        End Try

        Return View("PartialSelector")
    End Function

    Private Sub LoadDefaultTranslations(pageName As String)
        Dim customLang As Dictionary(Of String, String) = ViewData(Helpers.Constants.DefaultLanguagesEntries)

        Dim languageServerHandler = GetServerLanguage("LivePortal")

        If customLang.ContainsKey("EmployeeSelector#roDestination") Then
            customLang("EmployeeSelector#roDestination") = languageServerHandler.Translate("roDestination." & pageName, "EmployeeSelector")
        Else
            customLang.Add("EmployeeSelector#roDestination", languageServerHandler.Translate("roDestination." & pageName, "EmployeeSelector"))
        End If

        ViewData(Helpers.Constants.DefaultLanguagesEntries) = customLang
    End Sub

    Private Sub FillEmployeesViewBag(feature As String, config As String)
        If config = String.Empty OrElse (config.Length = 3 AndAlso config(0) = "1") Then
            Dim employeeList = GetSupervisedEmployees(feature)
            Dim availableEmployees As New List(Of EmployeeSelector)
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
            Else
                availableEmployees = New List(Of EmployeeSelector)
            End If

            ViewBag.AvailableEmployees = availableEmployees
        Else
            ViewBag.AvailableEmployees = Nothing
        End If
    End Sub

    Private Sub FillGroupsViewBag(feature As String, config As String)
        If config = String.Empty OrElse (config.Length = 3 AndAlso config(0) = "1") Then
            Dim dtGroups As DataTable = API.EmployeeGroupsServiceMethods.GetGroups(Nothing, feature)

            If dtGroups IsNot Nothing AndAlso dtGroups.Rows.Count > 0 Then
                Dim groupList = dtGroups.AsEnumerable().[Select](Function(dataRow) New GroupSelector(dataRow.Field(Of Integer)("ID"), dataRow.Field(Of String)("FullGroupName"))).ToList()
                ViewBag.AvailableGroups = groupList
            Else
                ViewBag.AvailableGroups = New List(Of GroupSelector)
            End If

        Else
            ViewBag.AvailableGroups = Nothing
        End If
    End Sub

    Private Sub FillLabagreesViewBag(config As String)
        If config = String.Empty OrElse (config.Length = 3 AndAlso config(2) = "1") Then
            Dim labagrees As DataTable = API.LabAgreeServiceMethods.GetLabAgrees(Nothing)

            If labagrees IsNot Nothing AndAlso labagrees.Rows.Count > 0 Then
                ViewBag.AvailableLabAgrees = labagrees.AsEnumerable().[Select](Function(dataRow) New SelectField(dataRow("Name"), dataRow("ID"))).ToList()
            Else
                ViewBag.AvailableGroups = New List(Of SelectField)
            End If

        Else
            ViewBag.AvailableLabAgrees = Nothing
        End If
    End Sub

    Private Sub FillCollectivesViewBag(config As String)
        If config = String.Empty OrElse (config.Length = 3 AndAlso config(1) = "1") Then
            Dim collectives As roCollective() = API.CollectiveServiceMethods.GetAllCollectives(Nothing)

            If collectives IsNot Nothing AndAlso collectives.Length > 0 Then
                ViewBag.AvailableCollectives = collectives.AsEnumerable().[Select](Function(collective) New SelectField(collective.Name, collective.Id)).ToList()
            Else
                ViewBag.AvailableCollectives = New List(Of SelectField)
            End If

        Else
            ViewBag.AvailableCollectives = Nothing
        End If
    End Sub
    <HttpPost>
    <PermissionsAtrribute(FeatureAlias:="Employees", Permission:=Robotics.Base.DTOs.Permission.Read)>
    Function GetEmployeeCount(ByVal filter As EmployeeFilter) As JsonResult
        Dim iEmployeeCount As Integer
        Try
            iEmployeeCount = Robotics.Security.roSelector.GetEmployeeCount(WLHelperWeb.CurrentPassportID, "Calendar", "U", filter.Filter, "", Not filter.Recursive, DateTime.ParseExact(filter.When, "yyyyMMdd", Nothing).Date)
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "EmployeeController::EmployeeSelector::" & ex.Message)
        End Try

        Return Json(iEmployeeCount)
    End Function

    <HttpGet>
    <PermissionsAtrribute(FeatureAlias:="Employees", Permission:=Robotics.Base.DTOs.Permission.Read)>
    Public Function CookiesPolicyPDF() As FileResult
        Try
            Dim bytes As Byte() = API.LiveTasksServiceMethods.GetCommonTemplateFile(Nothing, "VisualtimeCookiesPolicy.pdf")
            Return File(bytes, "application/pdf")
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "EmployeeController::CookiesPolicyPDF::" & ex.Message)
        End Try
        Return Nothing
    End Function

    Private Function GetSupervisedEmployees(ByVal feature As String) As EmployeeList
        Dim lrret As New EmployeeList

        Try
            lrret.Status = ErrorCodes.OK
            Dim fileName As String = Path.Combine(Hosting.HostingEnvironment.ApplicationPhysicalPath, "Base/Images/USER_48.PNG")
            Dim fileStream As New FileStream(fileName, FileMode.Open, FileAccess.Read)

            Dim ImageData As Byte()
            ImageData = New Byte(fileStream.Length - 1) {}
            fileStream.Read(ImageData, 0, System.Convert.ToInt32(fileStream.Length))
            fileStream.Close()

            lrret.DefaultImage = "url(data:image/png;base64," & Convert.ToBase64String(ImageData) & ") no-repeat center center"

            Dim empsList As New Generic.List(Of EmployeeInfo)

            Dim dtSurveys As DataTable = API.EmployeeServiceMethods.GetAllEmployees(Nothing, "", feature)

            If dtSurveys IsNot Nothing AndAlso dtSurveys.Rows.Count > 0 Then
                For Each oRow As DataRow In dtSurveys.Rows
                    empsList.Add(New EmployeeInfo With {
                              .EmployeeId = roTypes.Any2Integer(oRow("IDEmployee")),
                              .Name = roTypes.Any2String(oRow("EmployeeName")),
                              .Image = "/Employee/GetEmployeePhoto/" & roTypes.Any2String(oRow("IDEmployee"))
                            })

                Next
            End If

            lrret.Employees = empsList.ToArray()
        Catch ex As Exception
            lrret.Status = ErrorCodes.GENERAL_ERROR
            lrret.Employees = {}

            roLog.GetInstance().logMessage(roLog.EventType.roError, "EmployeeController::GetSupervisedEmployees::", ex)
        End Try

        Return lrret
    End Function

    Private Function LoadEmployeeImage(ByVal objImage As Byte()) As Byte()
        Dim strImage As String = String.Empty
        Try
            If objImage IsNot Nothing AndAlso objImage.Length > 0 Then
                Return MakeThumbnail(objImage, 32, 32)
                'strImage = Convert.ToBase64String(ImageData)
            Else
                Dim fileName As String = Path.Combine(Hosting.HostingEnvironment.ApplicationPhysicalPath, "Base/Images/USER_48.PNG")
                Dim fileStream As New FileStream(fileName, FileMode.Open, FileAccess.Read)

                Dim ImageData As Byte() = New Byte(fileStream.Length - 1) {}
                fileStream.Read(ImageData, 0, System.Convert.ToInt32(fileStream.Length))
                fileStream.Close()

                Return MakeThumbnail(ImageData, 32, 32)
                'strImage = Convert.ToBase64String(ImageData)
            End If
        Catch ex As Exception
            Return Convert.FromBase64String("iVBORw0KGgoAAAANSUhEUgAAADAAAAAwCAYAAABXAvmHAAAABmJLR0QA/wD/AP+gvaeTAAAFOElEQVRoge2YW2xTdRzHP//TrqPbsrVj3crkulsAuSugC5GixCAvLgiMy4PGFzUxJvJgfNGgvhowGBN4kESCMkoI4IMS0AFmYQIqIO7GxpDd6LbuXrZutOfvw+YyoO35n3YLL/smp2n7u32//8vvf86BaUxjGtN4mhCTlUh6PGlkN+9H6JsR5KALyxPZpfaN8Da8P1k1YRIESI8nDXfTJXRWGWaThIS3MSnRmhORkAC5Y+5WpPU4oJkNBfqR8ojw3v0gEQ5xC5Bv5H2IlX2JFB/DMGLGelFWfSWe4LgEyNKC15H66cnbQUiEViLKGn40G2iagvR40shp6sP8sjHKrBMcni/OtDWbiTJPwn3valxxhhAa9mTTy8gUEblr/gqkWGS2iHoBMUuWFuw2E2JuJHXLSVP+E5Fug43zoMhp4Bj+2kxaZQFyLzakzDOTHIAkDZ53Q0kBzE6DF2ZBtj1GgHDKXUuV66jPQHXBfmVfGG0PBQ7YUghLZoI21i80ARvmgD3GeRZ+cEi1jLoAoW9X9nXZYXMerHsG7NYn7fakURFa1Ca4RrVUhOxRIJlp2HTtVliZDUUODDt0th2Kc6GiNZI1XZWW0gzIrQuLETEYaWJ0mWwpHNukisdLgSN6zZ2FO1VSqM2AbeRtwlFsuWmwxg2OZKVUytBDpcAxIzc1AWFWR7W9Ok+ZkzmIxSpeaptYyDkqbge9tRzy1sZtfwxuFSc1AbpIUXET4x/x2R/zjnVYjENtCQmUHkLe2b4wIfsjkNKi4qY2A3Iqbt4MEKvrTYDqHkiIy1RCcWSjD4YM63EXl3riA5Pw0qg5VR13bN2ZGB1JaDdVciguIfqjmTpuddJa2aKUZiJafm/Gd8MXzVwmyhpWqORREiDKGjPQxBGizHj92XpqT9dAsMs4WdBP7akaGn5uiGyX4lNxvFHpNgJMLCFx7M6bSO1ZpOyNZPdd90H7ZWivhMA9eBgAPTR6PQyM/tdeCe2VkUdeIkErEd47X6hyAjN3o4A40VANOGVp3lEg8qNf0D96mUosRtAsy8Wx28rH9Hio2YD/IXcsXosMlh9dvC7lpPshrf57bErv5vOk2Bw+GVnE2QEnc10L2NIi2F132Y/eOFecYCgeHkqnXSSkfPfte4cXpXquOu9rgeE+BroGqBtMIj3FwjIt8p7/Pjybw50OwuEQwgm38yQXXlxvq9n0ceiXgz9cioeH6Rk48NdvrruBa7cbuioccsKu7m3rpd/Xj8WqcTD7DmtEzyNxV6WTdzvyCYd0MtwZZORmTGAhKJr5Uvdsy9rCPcXF3VMm4Kuq8rVVbecr7gdqntw7Evz/+hnsGWRGsoXTWbeYRRCA+8ygxL+U4HAYu8NOVl5WxDuF7NTC8BL3a6/sWfay8mwoC/jyn4urrzd7r/QF26LGSF3SXt/OyIMRXGkWfnL8CcDm3ufoDIRJsttwF+UgLNHLOuy5clnOtlUfrdxwQ4WXUhfySq/l1/LyiljkAYQmyFrgor3WR2cgzFtJywHoDISxWC248l0xyQP0DrWJxp6LVwClRzylc6D6j9D51v6/bSq+VpuFrHwXQhNU9YxeQgiy8rOw2tR6RkvfTdtn146fU/E1FLCv+bK9zn/Ro1R5DMmpNjLnZY7/zpyfSXKquWfmOv+FjXurqgwHzVDAUKfvwECww3S3SnWmRvyuiv6gTyQH6w1fphkK6H7QVGK6+iSha6hpm5GPoYCeYKvR29gpQ/dQc6aRj6GAwHBH3Kd1olCpbShgODQ4OWziwNOsPQ1V/Advx6dYI9TN5QAAAABJRU5ErkJggg==")
        End Try
        Return Nothing
    End Function

    Private Function LoadEmployeeLargeImage(ByVal objImage As Byte()) As Byte()
        Dim strImage As String = String.Empty
        Try
            If objImage IsNot Nothing AndAlso objImage.Length > 0 Then
                Return MakeThumbnail(objImage, 200, 200)
                'strImage = Convert.ToBase64String(ImageData)
            Else
                Dim fileName As String = Path.Combine(Hosting.HostingEnvironment.ApplicationPhysicalPath, "Base/Images/USER_48.PNG")
                Dim fileStream As New FileStream(fileName, FileMode.Open, FileAccess.Read)

                Dim ImageData As Byte() = New Byte(fileStream.Length - 1) {}
                fileStream.Read(ImageData, 0, System.Convert.ToInt32(fileStream.Length))
                fileStream.Close()

                Return MakeThumbnail(ImageData, 200, 200)
                'strImage = Convert.ToBase64String(ImageData)
            End If
        Catch ex As Exception
            Return Convert.FromBase64String("iVBORw0KGgoAAAANSUhEUgAAADAAAAAwCAYAAABXAvmHAAAABmJLR0QA/wD/AP+gvaeTAAAFOElEQVRoge2YW2xTdRzHP//TrqPbsrVj3crkulsAuSugC5GixCAvLgiMy4PGFzUxJvJgfNGgvhowGBN4kESCMkoI4IMS0AFmYQIqIO7GxpDd6LbuXrZutOfvw+YyoO35n3YLL/smp2n7u32//8vvf86BaUxjGtN4mhCTlUh6PGlkN+9H6JsR5KALyxPZpfaN8Da8P1k1YRIESI8nDXfTJXRWGWaThIS3MSnRmhORkAC5Y+5WpPU4oJkNBfqR8ojw3v0gEQ5xC5Bv5H2IlX2JFB/DMGLGelFWfSWe4LgEyNKC15H66cnbQUiEViLKGn40G2iagvR40shp6sP8sjHKrBMcni/OtDWbiTJPwn3valxxhhAa9mTTy8gUEblr/gqkWGS2iHoBMUuWFuw2E2JuJHXLSVP+E5Fug43zoMhp4Bj+2kxaZQFyLzakzDOTHIAkDZ53Q0kBzE6DF2ZBtj1GgHDKXUuV66jPQHXBfmVfGG0PBQ7YUghLZoI21i80ARvmgD3GeRZ+cEi1jLoAoW9X9nXZYXMerHsG7NYn7fakURFa1Ca4RrVUhOxRIJlp2HTtVliZDUUODDt0th2Kc6GiNZI1XZWW0gzIrQuLETEYaWJ0mWwpHNukisdLgSN6zZ2FO1VSqM2AbeRtwlFsuWmwxg2OZKVUytBDpcAxIzc1AWFWR7W9Ok+ZkzmIxSpeaptYyDkqbge9tRzy1sZtfwxuFSc1AbpIUXET4x/x2R/zjnVYjENtCQmUHkLe2b4wIfsjkNKi4qY2A3Iqbt4MEKvrTYDqHkiIy1RCcWSjD4YM63EXl3riA5Pw0qg5VR13bN2ZGB1JaDdVciguIfqjmTpuddJa2aKUZiJafm/Gd8MXzVwmyhpWqORREiDKGjPQxBGizHj92XpqT9dAsMs4WdBP7akaGn5uiGyX4lNxvFHpNgJMLCFx7M6bSO1ZpOyNZPdd90H7ZWivhMA9eBgAPTR6PQyM/tdeCe2VkUdeIkErEd47X6hyAjN3o4A40VANOGVp3lEg8qNf0D96mUosRtAsy8Wx28rH9Hio2YD/IXcsXosMlh9dvC7lpPshrf57bErv5vOk2Bw+GVnE2QEnc10L2NIi2F132Y/eOFecYCgeHkqnXSSkfPfte4cXpXquOu9rgeE+BroGqBtMIj3FwjIt8p7/Pjybw50OwuEQwgm38yQXXlxvq9n0ceiXgz9cioeH6Rk48NdvrruBa7cbuioccsKu7m3rpd/Xj8WqcTD7DmtEzyNxV6WTdzvyCYd0MtwZZORmTGAhKJr5Uvdsy9rCPcXF3VMm4Kuq8rVVbecr7gdqntw7Evz/+hnsGWRGsoXTWbeYRRCA+8ygxL+U4HAYu8NOVl5WxDuF7NTC8BL3a6/sWfay8mwoC/jyn4urrzd7r/QF26LGSF3SXt/OyIMRXGkWfnL8CcDm3ufoDIRJsttwF+UgLNHLOuy5clnOtlUfrdxwQ4WXUhfySq/l1/LyiljkAYQmyFrgor3WR2cgzFtJywHoDISxWC248l0xyQP0DrWJxp6LVwClRzylc6D6j9D51v6/bSq+VpuFrHwXQhNU9YxeQgiy8rOw2tR6RkvfTdtn146fU/E1FLCv+bK9zn/Ro1R5DMmpNjLnZY7/zpyfSXKquWfmOv+FjXurqgwHzVDAUKfvwECww3S3SnWmRvyuiv6gTyQH6w1fphkK6H7QVGK6+iSha6hpm5GPoYCeYKvR29gpQ/dQc6aRj6GAwHBH3Kd1olCpbShgODQ4OWziwNOsPQ1V/Advx6dYI9TN5QAAAABJRU5ErkJggg==")
        End Try
        Return Nothing
    End Function

    Private Function MakeThumbnail(ByVal myImage As Byte(), ByVal thumbWidth As Integer, ByVal thumbHeight As Integer) As Byte()
        Using ms As MemoryStream = New MemoryStream()

            Using thumbnail As Image = Image.FromStream(New MemoryStream(myImage)).GetThumbnailImage(thumbWidth, thumbHeight, Nothing, New IntPtr())
                thumbnail.Save(ms, Drawing.Imaging.ImageFormat.Png)
                Return ms.ToArray()
            End Using
        End Using
    End Function

End Class