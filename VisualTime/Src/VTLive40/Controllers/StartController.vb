Imports System.Drawing
Imports System.IO
Imports System.Web.Hosting
Imports System.Web.Mvc
Imports DevExtreme.AspNet.Data
Imports DevExtreme.AspNet.Mvc
Imports Newtonsoft.Json
Imports Robotics.Azure
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTBusiness.Group
Imports Robotics.Base.VTEmployees
Imports Robotics.Security
Imports Robotics.Security.Base
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions
Imports Robotics.Web.Base
Imports Robotics.Web.Base.API

Public Class StartController
    Inherits System.Web.Mvc.Controller
    Private oLanguage As roLanguageWeb

    Function Index() As ActionResult
        If WLHelperWeb.CurrentPassport IsNot Nothing Then
            ViewData(Helpers.Constants.ScriptVersion) = "?v=" & Robotics.VTBase.roTypes.Any2String(Reflection.Assembly.GetExecutingAssembly().GetCustomAttributes(GetType(Reflection.AssemblyDescriptionAttribute), False).FirstOrDefault().Description)
            ViewBag.RootUrl = ConfigurationManager.AppSettings("RootUrl")

            If HelperSession.AdvancedParametersCache("VTLive.Edition").ToString.ToLower = roServerLicense.roVisualTimeEdition.Starter.ToString.ToLower Then
                ViewBag.Genius = "Starter"
            Else
                ViewBag.Genius = "true"
            End If
            Dim oConfigValue As roAzureConfig = New roConfigRepository().GetConfigParameter(roConfigParameter.showaichatbot)
            If oConfigValue IsNot Nothing AndAlso roTypes.Any2Boolean(Robotics.VTBase.roJSONHelper.DeserializeNewtonSoft(oConfigValue.value, GetType(Boolean))) Then
                ViewBag.ShowAIChatBot = True
            Else
                ViewBag.ShowAIChatBot = False
            End If
            ViewBag.TelecommutingInstalled = HelperSession.GetFeatureIsInstalledFromApplication("Feature\Telecommuting")

            Try
                'UserPilot
                ViewBag.CurrentPassport = WLHelperWeb.CurrentPassport
                ViewBag.CompanyCode = WLHelperWeb.CompanyToken
                ViewBag.CompanyName = RoAzureSupport.GetCompanyName()
                Dim oConfigUserPilotValue As roAzureConfig = New roConfigRepository().GetConfigParameter(roConfigParameter.showuserpilot)
                If oConfigUserPilotValue IsNot Nothing AndAlso roTypes.Any2Boolean(Robotics.VTBase.roJSONHelper.DeserializeNewtonSoft(oConfigUserPilotValue.value, GetType(Boolean))) Then
                    ViewBag.ShowUserPilot = True
                Else
                    ViewBag.ShowUserPilot = False
                End If

                Dim showHelp = API.SecurityServiceMethods.GetHelpVersion(WLHelperWeb.CurrentPassport.ID)
                If showHelp = 0 Then
                    ViewBag.ShowHelp = True
                Else
                    ViewBag.ShowHelp = False
                End If

                Dim oWebLinks As List(Of roWebLink) = WebLinkServiceMethods.GetAllWebLinks(Nothing)
                ViewBag.WebLinks = oWebLinks

                Dim sessionNotifications As Boolean = WLHelperWeb.UpdateVTNotification

                ' Hemos mostrado la notificación de versión en esta sesión, no la volvemos a mostrar
                If sessionNotifications Then
                    ViewBag.ShowVersionNotification = False
                Else
                    ' En caso que no, debemos ver si ha marcado el checkbox de "No volver a mostrar"
                    Dim showUpdatePopup As Boolean = API.SecurityServiceMethods.GetVersionNotificationShown(WLHelperWeb.CurrentPassport.ID)

                    If Not showUpdatePopup Then
                        ' No lo ha marcado, mostramos la notificación
                        ViewBag.ShowVersionNotification = True

                        Dim currentVersion As String = ""
                        Dim currentVersionDate As String = ""
                        Dim versionHistory As String() = Nothing

                        If API.LicenseServiceMethods.VersionInfo(currentVersion, currentVersionDate, versionHistory) Then
                            Dim versionInfo As String = ""

                            If currentVersion.Split(" ").Length > 1 Then
                                Dim versionText As String = currentVersion.Split(" ")(0)
                                Dim additionalInfo As String = currentVersion.Split(" ")(1).Trim()

                                If additionalInfo.StartsWith("(") AndAlso additionalInfo.Length > 1 Then
                                    additionalInfo = additionalInfo.Substring(1)
                                End If
                                If additionalInfo.EndsWith(")") AndAlso additionalInfo.Length > 1 Then
                                    additionalInfo = additionalInfo.Substring(0, additionalInfo.Length - 1)
                                End If

                                versionInfo = $"{currentVersionDate} {versionText} ({additionalInfo})"
                            Else
                                versionInfo = currentVersion
                            End If

                            ViewBag.VersionInfo = versionInfo

                            'Marcamos que ya hemos visto el popup en la Session, así que no debemos mostrarlo hasta que se cierre sesión.
                            WLHelperWeb.UpdateVTNotification = True
                        End If
                    Else
                        'Tenia marcado el checkbox de "No volver a mostrar", no mostramos la notificación
                        ViewBag.ShowVersionNotification = False
                    End If
                End If

            Catch ex As Exception
                ViewBag.ShowHelp = False
            End Try

            Return View("Start")
        Else
            Return View("NoSession")
        End If
    End Function

    Public Sub UpdateHelpVersion()
        Try
            Dim updateHelp = API.SecurityServiceMethods.UpdateHelpVersion(WLHelperWeb.CurrentPassport.ID, 1, False)
        Catch ex As Exception
            'do nothing
        End Try
    End Sub

    Public Sub UpdateVersionNotification()
        Try
            Dim updateVersionShown = API.SecurityServiceMethods.UpdateVersionNotification(WLHelperWeb.CurrentPassport.ID, 1, False)
        Catch ex As Exception

        End Try
    End Sub

    Public Function GetAlerts(ByVal loadOptions As DataSourceLoadOptions) As ActionResult

        If WLHelperWeb.CurrentPassport IsNot Nothing Then
            Dim lrret As New AlertSummary

            Dim userAlerts = WLHelperWeb.UserAlerts
            Dim documentAlerts = WLHelperWeb.EmployeeDocumentationAlerts(, True)
            Dim companyAlerts = WLHelperWeb.CompanyDocumentationAlerts(, True)
            Dim systemAlerts = WLHelperWeb.SystemAlerts

            Dim alerts As New Generic.List(Of AlertSummaryDetail)

            'USUARIO
            Dim alertLineUser As New AlertSummaryDetail
            alertLineUser.Count = userAlerts.Rows.Count
            alertLineUser.Description = GetServerLanguage().Translate("roAlertUser", "")
            alertLineUser.ImageSrc = "Base/Images/PortalAlerts/ico_Task_With_ALerts.png"

            'DOCUMENTOS DE USUARIO
            Dim alertLineDocument As New AlertSummaryDetail
            Dim documentCount As Integer = 0
            documentCount = If(documentAlerts.WorkForecastDocuments.ToList.FindAll(Function(x) x.IDDocument > 0).Count > 0, 1, 0) +
                        If(documentAlerts.WorkForecastDocuments.ToList.FindAll(Function(x) x.IDDocument = 0).Count > 0, 1, 0) +
                        If(documentAlerts.MandatoryDocuments.Any(), 1, 0) +
                        If(documentAlerts.AbsenteeismDocuments.ToList.FindAll(Function(x) x.IDDocument > 0).Count > 0, 1, 0) +
                        If(documentAlerts.AbsenteeismDocuments.ToList.FindAll(Function(x) x.IDDocument = 0).Count > 0, 1, 0) +
                        If(documentAlerts.DocumentsValidation.Any(), 1, 0) +
                        If(documentAlerts.AccessAuthorizationDocuments.ToList.FindAll(Function(x) x.IDDocument > 0).Count > 0, 1, 0) +
                        If(documentAlerts.AccessAuthorizationDocuments.ToList.FindAll(Function(x) x.IDDocument = 0).Count > 0, 1, 0)

            alertLineDocument.Count = documentCount
            alertLineDocument.Description = GetServerLanguage().Translate("roAlertDoc", "")
            alertLineDocument.ImageSrc = "Base/Images/PortalAlerts/ico_AbsenteeismDocuments.png"

            'DOCUMENTOS DE EMPRESA
            Dim alertLineCompany As New AlertSummaryDetail
            Dim companyCount As Integer = 0
            companyCount = If(companyAlerts.DocumentsValidation.Any(), 1, 0) +
                        If(companyAlerts.AccessAuthorizationDocuments.ToList.FindAll(Function(x) x.IDDocument > 0).Count > 0, 1, 0) +
                        If(companyAlerts.AccessAuthorizationDocuments.ToList.FindAll(Function(x) x.IDDocument = 0).Count > 0, 1, 0)

            alertLineCompany.Count = companyCount
            alertLineCompany.Description = GetServerLanguage().Translate("roAlertCom", "")
            alertLineCompany.ImageSrc = "Base/Images/PortalAlerts/ico_AbsenteeismDocuments.png"

            'ALERTAS DE SISTEMA
            Dim alertLineSystem As New AlertSummaryDetail
            alertLineSystem.Count = systemAlerts.Rows.Count
            alertLineSystem.Description = GetServerLanguage().Translate("roAlertSys", "")
            alertLineSystem.ImageSrc = "Base/Images/PortalAlerts/ico_request_params.png"

            If (alertLineUser.Count = 0 AndAlso alertLineCompany.Count = 0 AndAlso alertLineDocument.Count = 0 AndAlso alertLineSystem.Count = 0) Then
                Dim alertLine As New AlertSummaryDetail
                alertLine.ImageSrc = "Base/Images/PortalRequests/exito.png"
                alertLine.Description = GetServerLanguage().Translate("AllAlertsOk", "")
                alertLine.Count = 0
                alerts.Add(alertLine)
            Else
                alerts.Add(alertLineUser)
                alerts.Add(alertLineDocument)
                alerts.Add(alertLineCompany)
                alerts.Add(alertLineSystem)
            End If

            lrret.Alerts = alerts.ToArray

            Dim result = DataSourceLoader.Load(lrret.Alerts, loadOptions)
            Dim resultJson = JsonConvert.SerializeObject(result)
            Return Content(resultJson, "application/json")
        Else
            Dim lrret As New AlertSummary
            Dim result = DataSourceLoader.Load(lrret.Alerts, loadOptions)
            Dim resultJson = JsonConvert.SerializeObject(result)
            Return Content(resultJson, "application/json")
        End If




    End Function

    Public Function GetRequests(ByVal loadOptions As DataSourceLoadOptions) As ActionResult
        If WLHelperWeb.CurrentPassport IsNot Nothing AndAlso AuthHelper.GetPassportKeyValidated(WLHelperWeb.CurrentPassportID, WLHelperWeb.SecurityToken) Then
            Dim oEmpState As New Employee.roEmployeeState(WLHelperWeb.CurrentPassport.ID)

            Dim lrret As New RequestSummary
            Dim oReqState As Robotics.Base.VTRequests.Requests.roRequestState = New Robotics.Base.VTRequests.Requests.roRequestState(oEmpState.IDPassport)
            oReqState.Language.SetLanguageReference("LiveTasks", WLHelperWeb.CurrentLanguage)
            Try
                Dim tbRequests As DataTable = Nothing
                tbRequests = API.RequestServiceMethods.GetRequestsDashboardResume(Nothing, WLHelperWeb.CurrentPassport.ID, "Requests.Status IN (0,1)")

                Dim requestsList As New Generic.List(Of RequestSummaryDetail)
                If tbRequests.Rows.Count > 0 Then

                    Dim othersCount As Integer
                    Dim holidaysCount As Integer

                    For Each row As DataRow In tbRequests.Rows
                        Dim requestLine As New RequestSummaryDetail

                        If (roTypes.Any2String(row("RequestType")) = "7" OrElse roTypes.Any2String(row("RequestType")) = "9") AndAlso roTypes.Any2Integer(row("LevelsBelow")) = 1 Then

                            requestLine.Count = roTypes.Any2Integer(row("Total"))
                            requestLine.Description = oReqState.Language.Translate("Analytics.RequestTypeEnum_" & roTypes.Any2String(row("RequestType")), "")
                            Select Case roTypes.Any2String(row("RequestType"))

                                Case "1"
                                    requestLine.ImageSrc = "Base/Images/PortalRequests/ico_UserFieldChange.png"
                                Case "2"
                                    requestLine.ImageSrc = "Base/Images/PortalRequests/ico_ForbiddenPunch.png"
                                Case "3"
                                    requestLine.ImageSrc = "Base/Images/PortalRequests/ico_JustifyPunch.png"
                                Case "4"
                                    requestLine.ImageSrc = "Base/Images/PortalRequests/ico_ExternalWorkResume.png"
                                Case "5"
                                    requestLine.ImageSrc = "Base/Images/PortalRequests/ico_ChangeShift.png"
                                Case "6"
                                    requestLine.ImageSrc = "Base/Images/PortalRequests/ico_Holidays.png"
                                Case "7"
                                    requestLine.ImageSrc = "Base/Images/PortalRequests/ico_PlannedAbsences.png"
                                Case "8"
                                    requestLine.ImageSrc = "Base/Images/PortalRequests/ico_ShiftExchange.png"
                                Case "9"
                                    requestLine.ImageSrc = "Base/Images/PortalRequests/ico_PlannedCauses.png"
                                Case "10"
                                    requestLine.ImageSrc = "Base/Images/PortalRequests/ico_ForbiddenTaskPunch.png"
                                Case "11"
                                    requestLine.ImageSrc = "Base/Images/PortalRequests/ico_HolidaysCancel.png"
                                Case "12"
                                    requestLine.ImageSrc = "Base/Images/PortalRequests/ico_ForbiddenCostPunch.png"
                                Case "13"
                                    requestLine.ImageSrc = "Base/Images/PortalRequests/ico_Holidays.png"
                                Case "14"
                                    requestLine.ImageSrc = "Base/Images/PortalRequests/ico_ExternalWorkResume.png"
                                Case "15"
                                    requestLine.ImageSrc = "Base/Images/PortalRequests/ico_ExternalWorkResume.png"
                                Case Else
                                    requestLine.ImageSrc = "Base/Images/PortalRequests/ico_JustifyPunch.png"
                            End Select

                            requestsList.Add(requestLine)

                        ElseIf (roTypes.Any2String(row("RequestType")) = "6" OrElse roTypes.Any2String(row("RequestType")) = "11" OrElse roTypes.Any2String(row("RequestType")) = "13") AndAlso roTypes.Any2Integer(row("LevelsBelow")) = 1 Then
                            holidaysCount = holidaysCount + roTypes.Any2Integer(row("Total"))
                        Else
                            If roTypes.Any2Integer(row("LevelsBelow")) = 1 Then
                                othersCount = othersCount + roTypes.Any2Integer(row("Total"))
                            End If
                        End If

                    Next

                    If holidaysCount > 0 Then
                        Dim requestLineHolidays As New RequestSummaryDetail
                        requestLineHolidays.ImageSrc = "Base/Images/PortalRequests/ico_Holidays.png"
                        requestLineHolidays.Description = oReqState.Language.Translate("Analytics.RequestTypeEnum_6", "")
                        requestLineHolidays.Count = holidaysCount
                        requestsList.Add(requestLineHolidays)
                    End If

                    If othersCount > 0 Then
                        Dim requestLineOthers As New RequestSummaryDetail
                        requestLineOthers.ImageSrc = "Base/Images/PortalRequests/ico_ExternalWorkResume.png"
                        requestLineOthers.Description = oReqState.Language.Translate("Analytics.RequestTypeEnum_Others", "")
                        requestLineOthers.Count = othersCount
                        requestsList.Add(requestLineOthers)
                    End If

                    If requestsList.Count > 0 Then
                        lrret.Requests = requestsList.ToArray
                    Else
                        Dim requestLine As New RequestSummaryDetail
                        requestLine.ImageSrc = "Base/Images/PortalRequests/exito.png"
                        requestLine.Description = GetServerLanguage().Translate("AllRequestsOk", "")
                        requestLine.Count = 0
                        requestsList.Add(requestLine)
                        lrret.Requests = requestsList.ToArray
                    End If
                Else
                    Dim requestLine As New RequestSummaryDetail
                    requestLine.ImageSrc = "Base/Images/PortalRequests/exito.png"
                    requestLine.Description = GetServerLanguage().Translate("AllRequestsOk", "")
                    requestLine.Count = 0
                    requestsList.Add(requestLine)
                    lrret.Requests = requestsList.ToArray
                End If
            Catch ex As Exception
                'do nothing
            End Try

            Dim result = DataSourceLoader.Load(lrret.Requests, loadOptions)
            Dim resultJson = JsonConvert.SerializeObject(result)
            Return Content(resultJson, "application/json")
        Else
            Dim lrret As New RequestSummary
            Dim result = DataSourceLoader.Load(lrret.Requests, loadOptions)
            Dim resultJson = JsonConvert.SerializeObject(result)
            Return Content(resultJson, "application/json")
        End If

    End Function


    Public Function GetEmployees(ByVal loadOptions As DataSourceLoadOptions, ByVal hasData As String, ByVal idGroup As String) As ActionResult
        If WLHelperWeb.CurrentPassport IsNot Nothing AndAlso AuthHelper.GetPassportKeyValidated(WLHelperWeb.CurrentPassportID, WLHelperWeb.SecurityToken) Then
            Dim employeeDashboardList As New List(Of EmployeesDashboardInfo)

            If (idGroup IsNot Nothing) Then
                Dim present As Integer = 0
                Dim absent As Integer = 0
                Dim onHolidays As Integer = 0
                Dim onOthers As Integer = 0
                Dim home As Integer = 0
                Dim office As Integer = 0
                Dim isConsultor As Boolean = False
                Dim hasTelecommutingInstalled = HelperSession.GetFeatureIsInstalledFromApplication("Feature\Telecommuting")

                If WLHelperWeb.CurrentUserIsConsultantOrCegid Then
                    isConsultor = True
                End If

                Dim groups = roJSONHelper.Deserialize(idGroup, GetType(List(Of Integer)))
                employeeDashboardList = LoadEmployeesStatus(WLHelperWeb.CurrentPassport.ID, groups, isConsultor)

                For Each emp In employeeDashboardList
                    If emp.PresenceStatus = "In" Then
                        present += 1
                        If hasTelecommutingInstalled Then
                            If emp.InTelecommute Then
                                home += 1
                            Else
                                office += 1
                            End If
                        End If
                    Else
                        absent += 1
                    End If
                    If emp.InHolidays = "1" Or emp.InHoursHolidays = "1" Then
                        onHolidays += 1
                    ElseIf emp.InAbsence = "1" Or emp.InHoursAbsence = "1" Then
                        onOthers += 1
                    End If
                Next

                If hasTelecommutingInstalled Then
                    Dim telecommutingSummaryStatus As New Generic.List(Of TelecommutingSummaryDetail)
                    Dim inOffice As New TelecommutingSummaryDetail
                    inOffice.Count = office
                    inOffice.Description = GetServerLanguage().Translate("roTeamInOffice", "")
                    inOffice.ImageSrc = "Base/Images/PortalRequests/icons8-office-48.png"
                    telecommutingSummaryStatus.Add(inOffice)

                    Dim inHome As New TelecommutingSummaryDetail
                    inHome.Count = home
                    inHome.Description = GetServerLanguage().Translate("roTeamInHome", "")
                    inHome.ImageSrc = "Base/Images/PortalRequests/icons8-home-48.png"
                    telecommutingSummaryStatus.Add(inHome)

                    Dim telecommutingSummaryStatusArray = telecommutingSummaryStatus.ToArray
                    HttpContext.Session("Start_TelecommutingStatusResume") = telecommutingSummaryStatusArray
                End If
                Dim employeeSummaryStatus As New Generic.List(Of EmployeeSummaryDetail)
                Dim requestLinePresent As New EmployeeSummaryDetail
                requestLinePresent.Count = present
                requestLinePresent.Description = GetServerLanguage().Translate("roTeamAvailable", "")
                requestLinePresent.ImageSrc = "Base/Images/PortalRequests/WX_circle_green.png"
                employeeSummaryStatus.Add(requestLinePresent)

                Dim requestLineAbsent As New EmployeeSummaryDetail
                requestLineAbsent.Count = absent
                requestLineAbsent.Description = GetServerLanguage().Translate("roTeamNotAvailable", "")
                requestLineAbsent.ImageSrc = "Base/Images/PortalRequests/WX_circle_red.png"
                employeeSummaryStatus.Add(requestLineAbsent)

                Dim employeeSummaryStatusArray = employeeSummaryStatus.ToArray
                HttpContext.Session("Start_EmployeeStatusResume") = employeeSummaryStatusArray

                Dim absenceSummaryStatus As New Generic.List(Of AbsenceSummaryDetail)

                Dim lineHoliday As New AbsenceSummaryDetail
                lineHoliday.Count = onHolidays
                lineHoliday.Description = GetServerLanguage().Translate("roHolidays", "")
                lineHoliday.ImageSrc = "Base/Images/PortalRequests/ico_Holidays.png"
                absenceSummaryStatus.Add(lineHoliday)

                Dim lineOthers As New AbsenceSummaryDetail
                lineOthers.Count = onOthers
                lineOthers.Description = GetServerLanguage().Translate("roOthers", "")
                lineOthers.ImageSrc = "Base/Images/PortalRequests/ico_PlannedAbsences.png"
                absenceSummaryStatus.Add(lineOthers)
                HttpContext.Session("Start_AbsenceResume") = absenceSummaryStatus.ToArray

                Dim result = DataSourceLoader.Load(employeeDashboardList.ToArray, loadOptions)
                Dim resultJson = JsonConvert.SerializeObject(result)

                HttpContext.Session("Start_PresentCount") = present
                HttpContext.Session("Start_AbsentCount") = absent
                HttpContext.Session("Start_HolidaysCount") = onHolidays
                HttpContext.Session("Start_OthersCount") = onOthers

                HttpContext.Session("Start_EmployeeList") = resultJson

                Return Content(resultJson, "application/json")
            Else
                Dim result = DataSourceLoader.Load(employeeDashboardList.ToArray, loadOptions)
                Dim resultJson = JsonConvert.SerializeObject(result)
                Return Content(resultJson, "application/json")
            End If
        Else
            Dim employeeDashboardList As New List(Of EmployeesDashboardInfo)
            Dim result = DataSourceLoader.Load(employeeDashboardList.ToArray, loadOptions)
            Dim resultJson = JsonConvert.SerializeObject(result)
            Return Content(resultJson, "application/json")
        End If


    End Function

    Private Function LoadEmployeesStatus(ByVal idPassport As Integer, ByVal groups As List(Of Integer), Optional ByVal isConsultor As Boolean = False) As List(Of EmployeesDashboardInfo)

        Dim oRet As New List(Of EmployeesDashboardInfo)

        Try

            Dim tEmployeesDashboardInfo As DataTable = roBusinessSupport.GetEmployeesDashboardStatus(idPassport, groups, isConsultor)

            Dim oEmployeeDashboardInfo As EmployeesDashboardInfo

            If tEmployeesDashboardInfo IsNot Nothing AndAlso tEmployeesDashboardInfo.Rows.Count > 0 Then

                Dim fileName As String = Path.Combine(HostingEnvironment.ApplicationPhysicalPath, "Base/Images/USER_48.PNG")
                Dim fileStream As New FileStream(fileName, FileMode.Open, FileAccess.Read)

                Dim ImageData As Byte()
                ImageData = New Byte(fileStream.Length - 1) {}
                fileStream.Read(ImageData, 0, System.Convert.ToInt32(fileStream.Length))
                fileStream.Close()

                Dim fileNameInside As String = Path.Combine(HostingEnvironment.ApplicationPhysicalPath, "Base/Images/PortalRequests/WX_circle_green.png")
                Dim fileStreamInside As New FileStream(fileNameInside, FileMode.Open, FileAccess.Read)
                Dim ImageDataInside As Byte()
                ImageDataInside = New Byte(fileStreamInside.Length - 1) {}
                fileStreamInside.Read(ImageDataInside, 0, System.Convert.ToInt32(fileStreamInside.Length))
                fileStreamInside.Close()

                Dim fileNameOutside As String = Path.Combine(HostingEnvironment.ApplicationPhysicalPath, "Base/Images/PortalRequests/WX_circle_red.png")
                Dim fileStreamOutside As New FileStream(fileNameOutside, FileMode.Open, FileAccess.Read)
                Dim ImageDataOutside As Byte()
                ImageDataOutside = New Byte(fileStreamOutside.Length - 1) {}
                fileStreamOutside.Read(ImageDataOutside, 0, System.Convert.ToInt32(fileStreamOutside.Length))
                fileStreamOutside.Close()

                Dim fileNameAbsence As String = Path.Combine(HostingEnvironment.ApplicationPhysicalPath, "Base/Images/PortalRequests/ico_PlannedAbsences.png")
                Dim fileStreamAbsence As New FileStream(fileNameAbsence, FileMode.Open, FileAccess.Read)
                Dim ImageDataAbsence As Byte()
                ImageDataAbsence = New Byte(fileStreamAbsence.Length - 1) {}
                fileStreamAbsence.Read(ImageDataAbsence, 0, System.Convert.ToInt32(fileStreamAbsence.Length))
                fileStreamAbsence.Close()

                Dim fileNameHours As String = Path.Combine(HostingEnvironment.ApplicationPhysicalPath, "Base/Images/PortalRequests/ico_PlannedCauses.png")
                Dim fileStreamHours As New FileStream(fileNameHours, FileMode.Open, FileAccess.Read)
                Dim ImageDataHours As Byte()
                ImageDataHours = New Byte(fileStreamHours.Length - 1) {}
                fileStreamHours.Read(ImageDataHours, 0, System.Convert.ToInt32(fileStreamHours.Length))
                fileStreamHours.Close()

                Dim fileNameHolidays As String = Path.Combine(HostingEnvironment.ApplicationPhysicalPath, "Base/Images/PortalRequests/ico_Holidays.png")
                Dim fileStreamHolidays As New FileStream(fileNameHolidays, FileMode.Open, FileAccess.Read)
                Dim ImageDataHolidays As Byte()
                ImageDataHolidays = New Byte(fileStreamHolidays.Length - 1) {}
                fileStreamHolidays.Read(ImageDataHolidays, 0, System.Convert.ToInt32(fileStreamHolidays.Length))
                fileStreamHolidays.Close()

                Dim fileNameRequest As String = Path.Combine(HostingEnvironment.ApplicationPhysicalPath, "Base/Images/PortalRequests/ico_requests_header.png")
                Dim fileStreamRequest As New FileStream(fileNameRequest, FileMode.Open, FileAccess.Read)
                Dim ImageDataRequest As Byte()
                ImageDataRequest = New Byte(fileStreamRequest.Length - 1) {}
                fileStreamRequest.Read(ImageDataRequest, 0, System.Convert.ToInt32(fileStreamRequest.Length))
                fileStreamRequest.Close()

                Dim fileNameTelecommute As String = Path.Combine(HostingEnvironment.ApplicationPhysicalPath, "Base/Images/PortalRequests/icons8-home-48.png")
                Dim fileStreamTelecommute As New FileStream(fileNameTelecommute, FileMode.Open, FileAccess.Read)
                Dim ImageDataTelecommute As Byte()
                ImageDataTelecommute = New Byte(fileStreamTelecommute.Length - 1) {}
                fileStreamTelecommute.Read(ImageDataTelecommute, 0, System.Convert.ToInt32(fileStreamTelecommute.Length))
                fileStreamTelecommute.Close()

                Dim fileNameOffice As String = Path.Combine(HostingEnvironment.ApplicationPhysicalPath, "Base/Images/PortalRequests/icons8-office-48.png")
                Dim fileStreamOffice As New FileStream(fileNameOffice, FileMode.Open, FileAccess.Read)
                Dim ImageDataOffice As Byte()
                ImageDataOffice = New Byte(fileStreamOffice.Length - 1) {}
                fileStreamOffice.Read(ImageDataOffice, 0, System.Convert.ToInt32(fileStreamOffice.Length))
                fileStreamOffice.Close()

                Dim oEmpState As New Employee.roEmployeeState(-1)

                For Each oRow As DataRow In tEmployeesDashboardInfo.Rows
                    oEmployeeDashboardInfo = New EmployeesDashboardInfo
                    oEmployeeDashboardInfo.IdEmployee = roTypes.Any2String(oRow("IdEmployee"))
                    oEmployeeDashboardInfo.GroupPath = roTypes.Any2String(oRow("Path"))
                    oEmployeeDashboardInfo.EmployeeName = roTypes.Any2String(oRow("EmployeeName"))
                    oEmployeeDashboardInfo.LastPunch = roTypes.Any2String(oRow("LastPunchDateTime"))
                    If Not IsDBNull(oRow("LastPunchType")) Then
                        Select Case oRow("LastPunchType")
                            Case PunchTypeEnum._CENTER
                                oEmployeeDashboardInfo.Details = GetServerLanguage().Translate("roCCChange", "")
                            Case PunchTypeEnum._TASK
                                oEmployeeDashboardInfo.Details = GetServerLanguage().Translate("roTaskChange", "")
                            Case PunchTypeEnum._IN, PunchTypeEnum._OUT, PunchTypeEnum._AUTO
                                oEmployeeDashboardInfo.Details = roTypes.Any2String(oRow("LastAttendanceCause"))
                            Case PunchTypeEnum._DR
                                oEmployeeDashboardInfo.Details = GetServerLanguage().Translate("roDinner", "")
                        End Select
                    End If
                    oEmployeeDashboardInfo.CostCenterName = roTypes.Any2String(oRow("CostCenterName"))
                    oEmployeeDashboardInfo.ShiftName = roTypes.Any2String(oRow("ShiftName"))
                    oEmployeeDashboardInfo.TaskName = roTypes.Any2String(oRow("LastTaskName"))
                    If roTypes.Any2String(oRow("ZoneName")) = "Sin especificar" Then
                        oEmployeeDashboardInfo.ZoneName = GetServerLanguage().Translate("WorldZone", "")
                    Else
                        oEmployeeDashboardInfo.ZoneName = roTypes.Any2String(oRow("ZoneName"))
                    End If
                    oEmployeeDashboardInfo.Image = LoadEmployeeImage(oRow("EmployeeImage"), oEmpState, ImageData)
                    oEmployeeDashboardInfo.InAbsence = roTypes.Any2String(oRow("InAbsence"))
                    oEmployeeDashboardInfo.InAbsenceCause = roTypes.Any2String(oRow("InAbsenceCause"))
                    oEmployeeDashboardInfo.InHoursAbsence = roTypes.Any2String(oRow("InHourAbsence"))
                    oEmployeeDashboardInfo.InHoursAbsenceCause = roTypes.Any2String(oRow("InHourAbsenceCause"))
                    oEmployeeDashboardInfo.DaysAbsenceRequested = roTypes.Any2String(oRow("AbsenceRequested"))
                    oEmployeeDashboardInfo.DaysAbsenceRequestedCause = roTypes.Any2String(oRow("AbsenceRequestCause"))
                    oEmployeeDashboardInfo.HoursAbsenceRequested = roTypes.Any2String(oRow("HoursAbsenceRequested"))
                    oEmployeeDashboardInfo.HoursAbsenceRequestedCause = roTypes.Any2String(oRow("HoursAbsenceRequestCause"))
                    oEmployeeDashboardInfo.InHolidays = roTypes.Any2String(oRow("InHolidays"))
                    oEmployeeDashboardInfo.InHoursHolidays = roTypes.Any2String(oRow("InHoursHolidays"))
                    oEmployeeDashboardInfo.LastCause = roTypes.Any2String(oRow("LastAttendanceCause"))
                    oEmployeeDashboardInfo.LocationName = roTypes.Any2String(oRow("LocationName"))
                    oEmployeeDashboardInfo.PresenceStatus = roTypes.Any2String(oRow("AttendanceStatus"))
                    oEmployeeDashboardInfo.RealLastPunch = roTypes.Any2DateTime(oRow("LastPunchDatetime"))
                    oEmployeeDashboardInfo.InTelecommute = roTypes.Any2Boolean(oRow("InTelecommute"))
                    If oEmployeeDashboardInfo.PresenceStatus = "In" Then
                        If (oEmployeeDashboardInfo.InTelecommute) Then
                            oEmployeeDashboardInfo.InRealTimeTC = "1"
                        Else
                            oEmployeeDashboardInfo.InRealTimeTC = "0"
                        End If

                    End If
                    ' Hora del último fichaje, en formato HH:mm (+/- x d)
                    oEmployeeDashboardInfo.LastPunchFormattedDateTime = ""
                    Dim iDaysDiff As Integer = 0
                    If Not IsDBNull(oRow("LastPunchDate")) AndAlso Not IsDBNull(oRow("LastPunchDatetime")) Then
                        iDaysDiff = Now.Date.Subtract(roTypes.Any2DateTime(oRow("LastPunchDate")).Date).TotalDays
                        If iDaysDiff > 0 Then
                            If iDaysDiff < 10 Then
                                oEmployeeDashboardInfo.LastPunchFormattedDateTime = GetServerLanguage().Translate("roHasPassed", "") & " " & iDaysDiff & "d"
                            Else
                                oEmployeeDashboardInfo.LastPunchFormattedDateTime = ""
                            End If
                        Else
                            oEmployeeDashboardInfo.LastPunchFormattedDateTime = roTypes.Any2Time(oRow("LastPunchDatetime")).TimeOnly
                        End If
                    Else
                        oEmployeeDashboardInfo.LastPunchFormattedDateTime = ""
                    End If

                    'Carga de imagenes de Ausencia
                    If oEmployeeDashboardInfo.InAbsence = "1" Then
                        oEmployeeDashboardInfo.InAbsenceImage = LoadStatusImage(ImageDataAbsence)
                    ElseIf oEmployeeDashboardInfo.InHoursAbsence = "1" Then
                        oEmployeeDashboardInfo.InAbsenceImage = LoadStatusImage(ImageDataHours)
                    ElseIf oEmployeeDashboardInfo.DaysAbsenceRequested = "1" Then
                        oEmployeeDashboardInfo.InAbsenceImage = LoadStatusImage(ImageDataRequest)
                    ElseIf oEmployeeDashboardInfo.HoursAbsenceRequested = "1" Then
                        oEmployeeDashboardInfo.InAbsenceImage = LoadStatusImage(ImageDataRequest)
                    Else
                        oEmployeeDashboardInfo.InAbsenceImage = ""
                    End If

                    'Datos de teletrabajo

                    If oEmployeeDashboardInfo.InTelecommute Then
                        If oEmployeeDashboardInfo.PresenceStatus = "In" Then
                            oEmployeeDashboardInfo.InTelecommuteImage = LoadStatusImage(ImageDataTelecommute)
                        Else
                            oEmployeeDashboardInfo.InTelecommuteImage = ""
                        End If
                    Else
                        If oEmployeeDashboardInfo.PresenceStatus = "In" Then
                            oEmployeeDashboardInfo.InTelecommuteImage = LoadStatusImage(ImageDataOffice)
                        Else
                            oEmployeeDashboardInfo.InTelecommuteImage = ""
                        End If
                    End If

                    If oEmployeeDashboardInfo.InAbsence = "1" Or oEmployeeDashboardInfo.InHoursAbsence = "1" Then
                        oEmployeeDashboardInfo.InAnyAbsence = "1"
                    End If
                    If oEmployeeDashboardInfo.InHolidays = "1" Or oEmployeeDashboardInfo.InHoursHolidays = "1" Then
                        oEmployeeDashboardInfo.InAnyHoliday = "1"
                        oEmployeeDashboardInfo.InAbsenceImage = LoadStatusImage(ImageDataHolidays)
                    End If

                    oRet.Add(oEmployeeDashboardInfo)
                Next
            End If
            'End If
        Catch ex As Exception
            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "StartController::LoadEmployeesStatus:Exception:")
        End Try

        Return oRet

    End Function

    Public Function GetEmployeeResume(ByVal loadOptions As DataSourceLoadOptions) As ActionResult
        If WLHelperWeb.CurrentPassport IsNot Nothing Then
            Dim employees = HttpContext.Session("Start_EmployeeStatusResume")
            If employees IsNot Nothing Then
                Dim result = DataSourceLoader.Load(employees, loadOptions)
                Dim resultJson = JsonConvert.SerializeObject(result)
                Return Content(resultJson, "application/json")
            End If
        End If
        Return Nothing

    End Function

    Public Function GetTelecommutingResume(ByVal loadOptions As DataSourceLoadOptions) As ActionResult
        If WLHelperWeb.CurrentPassport IsNot Nothing AndAlso AuthHelper.GetPassportKeyValidated(WLHelperWeb.CurrentPassportID, WLHelperWeb.SecurityToken) Then
            Dim employees = HttpContext.Session("Start_TelecommutingStatusResume")
            If employees IsNot Nothing Then
                Dim result = DataSourceLoader.Load(employees, loadOptions)
                Dim resultJson = JsonConvert.SerializeObject(result)
                Return Content(resultJson, "application/json")
            End If
        End If
        Return Nothing

    End Function

    Public Function GetGroups(ByVal loadOptions As DataSourceLoadOptions) As ActionResult
        If WLHelperWeb.CurrentPassport IsNot Nothing Then
            Dim oEmpState As New Employee.roEmployeeState(WLHelperWeb.CurrentPassport.ID)
            Dim oGroupState As New roGroupState(WLHelperWeb.CurrentPassport.ID, oEmpState.Context, oEmpState.ClientAddress)

            Dim isConsultor = False
            If WLHelperWeb.CurrentUserIsConsultantOrCegid Then
                isConsultor = True
            End If

            Dim dsSuperVisedGroups As DataSet = roGroup.GetSupervisedGroups(oGroupState, isConsultor)


            If dsSuperVisedGroups IsNot Nothing AndAlso dsSuperVisedGroups.Tables.Count = 1 Then
                Dim groupList As List(Of GroupSelector) = dsSuperVisedGroups.Tables(0).AsEnumerable().[Select](Function(dataRow) New GroupSelector(dataRow.Field(Of Integer)("ID"), dataRow.Field(Of String)("FullGroupName"))).ToList()
                groupList = groupList.OrderBy(Function(x) x.Name).ToList()

                Dim lstGroups As New List(Of GroupSelector)
                lstGroups.Add(New GroupSelector(-1, GetServerLanguage().Translate("roDirectSupervisedEmployees", "")))
                lstGroups.AddRange(groupList)

                Dim result = DataSourceLoader.Load(lstGroups, loadOptions)
                Dim resultJson = JsonConvert.SerializeObject(result)
                Return Content(resultJson, "application/json")
            End If
        End If

        Return Nothing

    End Function

    Public Function GetAbsenceResume(ByVal loadOptions As DataSourceLoadOptions) As ActionResult
        If WLHelperWeb.CurrentPassport IsNot Nothing AndAlso AuthHelper.GetPassportKeyValidated(WLHelperWeb.CurrentPassportID, WLHelperWeb.SecurityToken) Then
            Dim absences = HttpContext.Session("Start_AbsenceResume")
            If absences IsNot Nothing Then
                Dim result = DataSourceLoader.Load(absences, loadOptions)
                Dim resultJson = JsonConvert.SerializeObject(result)
                Return Content(resultJson, "application/json")
            End If
        End If

        Return Nothing
    End Function

    Public Function HelloMessage() As String
        If WLHelperWeb.CurrentPassport IsNot Nothing Then
            Dim currentDate = DateTime.Now
            If currentDate.Hour > 5 AndAlso currentDate.Hour < 13 Then
                Return GetServerLanguage().Translate("GoodMorning", "") & " " & If(WLHelperWeb.CurrentPassport IsNot Nothing, WLHelperWeb.CurrentPassport.Name, "") & "! " & GetServerLanguage().Translate("TodayIs", "") & " " & FormatDateTime(Date.Now, DateFormat.LongDate)
            ElseIf currentDate.Hour > 12 AndAlso currentDate.Hour < 20 Then
                Return GetServerLanguage().Translate("GoodEvening", "") & " " & If(WLHelperWeb.CurrentPassport IsNot Nothing, WLHelperWeb.CurrentPassport.Name, "") & "! " & GetServerLanguage().Translate("TodayIs", "") & " " & FormatDateTime(Date.Now, DateFormat.LongDate)
            Else
                Return GetServerLanguage().Translate("GoodNight", "") & " " & If(WLHelperWeb.CurrentPassport IsNot Nothing, WLHelperWeb.CurrentPassport.Name, "") & "! " & GetServerLanguage().Translate("TodayIs", "") & " " & FormatDateTime(Date.Now, DateFormat.LongDate)
            End If
        Else
            Return ""
        End If

    End Function

    Public Function LastLoginMessage() As String
        If WLHelperWeb.CurrentPassport IsNot Nothing Then
            Dim lastLogin = API.SecurityServiceMethods.GetLastLogin(WLHelperWeb.CurrentPassport.ID)
            If lastLogin.Year.ToString <> "2079" Then
                Return GetServerLanguage().Translate("LastLoginMessage", "") & " " & FormatDateTime(lastLogin, DateFormat.LongDate) & " " & GetServerLanguage().Translate("LastLoginMessage2", "") & " " & FormatDateTime(lastLogin, DateFormat.LongTime)
            Else
                Return ""
            End If
        Else
            Return ""
        End If


    End Function

    Public Function refreshEmployeeResume() As ActionResult

        Return PartialView("employeeResume")
    End Function

    Public Function LoadStatusImage(ByVal defaultImage As Byte()) As String
        Dim strImage As String = String.Empty

        Try
            strImage = "data:image/png;base64," & Convert.ToBase64String(defaultImage)
        Catch ex As Exception
            strImage = String.Empty
        End Try
        Return strImage
    End Function

    Private Function LoadEmployeeImage(ByVal objImage As Object, ByVal oEmpState As Employee.roEmployeeState, ByVal defaultImage As Byte()) As String
        Dim strImage As String = String.Empty
        Try
            Dim ImageData As Byte()

            If Not IsDBNull(objImage) Then
                ImageData = CType(objImage, Byte())
                ImageData = MakeThumbnail(ImageData, 32, 32)
                strImage = "data:image/png;base64," & Convert.ToBase64String(ImageData)
            Else
                strImage = "data:image/png;base64," & Convert.ToBase64String(defaultImage)
            End If
        Catch ex As Exception
            strImage = String.Empty
        End Try
        Return strImage
    End Function

    Private Function MakeThumbnail(ByVal myImage As Byte(), ByVal thumbWidth As Integer, ByVal thumbHeight As Integer) As Byte()
        Using ms As MemoryStream = New MemoryStream()

            Using thumbnail As Image = Image.FromStream(New MemoryStream(myImage)).GetThumbnailImage(thumbWidth, thumbHeight, Nothing, New IntPtr())
                thumbnail.Save(ms, Drawing.Imaging.ImageFormat.Png)
                Return ms.ToArray()
            End Using
        End Using
    End Function

    Public Function GetServerLanguage() As roLanguageWeb
        If Me.oLanguage Is Nothing Then
            Me.oLanguage = New roLanguageWeb
            WLHelperWeb.SetLanguage(Me.oLanguage, "LiveGUI")
        End If
        Return Me.oLanguage
    End Function

End Class