Imports System.Net
Imports System.Web.Mvc
Imports DevExpress.DataProcessing
Imports DevExtreme.AspNet.Data
Imports DevExtreme.AspNet.Mvc
Imports Newtonsoft.Json
Imports Robotics.Base
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Zone
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions
Imports Robotics.Web.Base
Imports VTLive40.Controllers.Base

<PermissionsAtrribute(FeatureAlias:="Employees", Permission:=Robotics.Base.DTOs.Permission.Read)>
<LoggedInAtrribute(Requiered:=True)>
Public Class ZonesController
    Inherits BaseController
    Private oLanguage As roLanguageWeb

    Private requieredLabels = {"headerTitle", "headerTitleNewZone", "descriptionInfo", "definedZones", "createZone", "zonesManagement", "deleteZoneInfo", "deleteZoneHeader", "empty", "loading", "name", "description", "isWorkingZone",
        "color", "plane", "camera", "timeZone", "priority", "maps", "showInMap", "maxCapacity", "showZoneCapacity", "isEmergencyZone", "zoneLocation",
        "roZoneConfiguration", "roZoneAdvanced", "zoneWorkCenter", "selectZoneWorkCenter", "lblAccessZonesMainTitlePeriods", "lblZonesInactivityTitle", "emptyPeriods", "weekDayName", "dateEnd", "dateBegin",
        "deleteInactivityZoneInfo", "deleteInactivityZoneHeader", "inactivityEmpty", "lblZonesExceptionTitle", "deleteExceptionZoneInfo", "deleteExceptionZoneHeader", "exceptionEmpty",
        "dateException", "zoneCurrentCapacity", "numberPeople", "lblZoneType", "zoneStatus", "zoneAdministration", "usersInZone",
        "usersInZoneLastHour", "descriptionLabel", "isTelecommutingZone", "selectTelecommutingType", "askZoneType", "presenceZoneType", "telecommutingZoneType", "zoneSupervisor", "selectZoneSupervisor", "selectTimeZone",
        "zonesWorkingMode", "zonesWorkingModeLbl", "activeLbl", "inactiveLbl", "zonesWorkingModeLblDesc", "ips", "ZoneName", "zoneAllowedIp", "ipsEmpty", "lblYes", "lblNo"}

    Private oMapsKey = "AIzaSyDEu7Qo0HTVOFAw3xDla35s_wMhvY7qiGw"

    Private Const UNESPECIFIED_ZONE As String = "255"

    Function Index() As ActionResult
        Me.InitializeBase(CardTreeTypes.Zones, TabBarButtonTypes.Zones, "Zones", requieredLabels, "LiveAccess") _
                          .SetBarButton(BarButtonTypes.Zones) _
                          .SetViewInfo("LiveAccess", "Zones", "Title", "Title", "AccessImages/AccessZones80.png", "TitleDesc")
        LoadDefaultTimeZones()
        LoadZoneTypes()
        LoadWorkCenters()
        LoadSupervisors()
        LoadZones()

        ViewBag.TelecommutingInstalled = HelperSession.GetFeatureIsInstalledFromApplication("Feature\Telecommuting")
        ViewBag.CapacityManagementInstalled = True

        Dim oLicSupport As New roLicenseSupport()
        Dim oLicInfo As roVTLicense = oLicSupport.GetVTLicenseInfo()

        If oLicInfo.Edition = roServerLicense.roVisualTimeEdition.NotSet Then
            ViewBag.CapacityManagementInstalled = True
        Else
            If (oLicInfo.Edition = roServerLicense.roVisualTimeEdition.Advanced Or oLicInfo.Edition = roServerLicense.roVisualTimeEdition.Premium) Then
                ViewBag.CapacityManagementInstalled = HelperSession.GetFeatureIsInstalledFromApplication("Feature\CapacityManagement")
            Else
                ViewBag.CapacityManagementInstalled = False
            End If

        End If


        Dim oParam = API.CommonServiceMethods.GetAdvancedParameter(Nothing, "VTPortal.ZoneRestrictedByIP", False)
        ViewBag.ZoneRestrictedByIP = roTypes.Any2Boolean(oParam.Value)

        Return View("index")
    End Function

    <HttpPost>
    Public Function InsertOrUpdateZone(ByVal values As roZone) As ActionResult
        If values IsNot Nothing Then API.ZoneServiceMethods.SaveZone(Nothing, values, True)

        If values IsNot Nothing AndAlso API.ZoneServiceMethods.LastErrorText = "" Then
            Return New HttpStatusCodeResult(HttpStatusCode.OK)
        Else
            Return New HttpStatusCodeResult(HttpStatusCode.BadRequest, API.ZoneServiceMethods.LastErrorText)
        End If

    End Function

    <HttpGet>
    Public Function LoadInfoZone(ByVal id As String) As JsonResult

        Dim infoZone As roZone = New roZone()
        infoZone = API.ZoneServiceMethods.GetZoneByID(Nothing, id, True)
        Return Json(infoZone, JsonRequestBehavior.AllowGet)

    End Function

    Public Function GetGoogleAPIKey() As String
        Return oMapsKey
    End Function

    Private Sub LoadZoneTypes()
        Dim telecommutingZoneType As New ZoneType()
        Dim labels = ViewData(VTLive40.Helpers.Constants.DefaultLanguagesEntries)

        telecommutingZoneType.Name = labels("Zones#telecommutingZoneType")
        telecommutingZoneType.ID = ZoneTelecommutingType.Telecommuting

        Dim presenceZoneType As New ZoneType()
        presenceZoneType.Name = labels("Zones#presenceZoneType")
        presenceZoneType.ID = ZoneTelecommutingType.Presence

        Dim askZoneType As New ZoneType()
        askZoneType.Name = labels("Zones#askZoneType")
        askZoneType.ID = ZoneTelecommutingType.AskUser

        Dim zoneTypes As Array
        zoneTypes = {telecommutingZoneType, presenceZoneType, askZoneType}
        ViewBag.ZoneTypes = zoneTypes
    End Sub

    Private Sub LoadDefaultTimeZones()
        Dim oTimeZones As ObjectModel.ReadOnlyCollection(Of TimeZoneInfo) = TimeZoneInfo.GetSystemTimeZones()
        Dim languageFile = GetServerLanguage("LiveAccess")
        Dim timeZonesList As New List(Of DropdownElement)
        For Each dr As TimeZoneInfo In oTimeZones
            timeZonesList.Add(New DropdownElement(dr.Id, languageFile.Translate("Timezone." & dr.Id, "Zones")))
        Next
        ViewBag.DefaultTimeZones = timeZonesList
    End Sub

    Private Sub LoadWorkCenters()
        Dim dTblWC As DataTable = API.ZoneServiceMethods.GetZoneWorkCenters(Nothing)
        ViewBag.AvailableWorkCenters = From dr As DataRow In dTblWC.Rows Select dTblWC.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col))
    End Sub

    Private Sub LoadSupervisors()
        Dim supervisorsList As roPassport() = API.SecurityV3ServiceMethods.GetAllAvailableSupervisorsList(Nothing)
        ViewBag.AvailableSupervisors = supervisorsList
    End Sub

    Public Function ScriptsVersion() As String
        Return "?v=" & Robotics.VTBase.roTypes.Any2String(Reflection.Assembly.GetExecutingAssembly().GetCustomAttributes(GetType(Reflection.AssemblyDescriptionAttribute), False).FirstOrDefault().Description)
    End Function

    <HttpGet>
    Public Function LoadEmployeesInZone(ByVal id As String) As JsonResult
        Dim employeesInZone As New List(Of EmployeeInfo)
        If (id IsNot Nothing AndAlso id <> "") Then

            employeesInZone = VTBusiness.Zone.roZone.GetInfoEmployeesInZone(id, New VTBusiness.Zone.roZoneState(-1))
        End If
        Return Json(employeesInZone, JsonRequestBehavior.AllowGet)

    End Function

    <HttpGet>
    Public Function LoadEmployeesInZoneLastHour(ByVal id As String) As JsonResult
        Dim employeesInZone As New List(Of EmployeeInfo)
        If (id IsNot Nothing And id <> "") Then
            employeesInZone = VTBusiness.Zone.roZone.GetInfoEmployeesInZoneLastHour(id, New VTBusiness.Zone.roZoneState(-1))
        End If
        Return Json(employeesInZone, JsonRequestBehavior.AllowGet)
    End Function

    Public Function GetZones(ByVal loadOptions As DataSourceLoadOptions) As ActionResult
        LoadZones()

        Dim zonesList As List(Of roZone) = ViewBag.Zones

        Dim result = DataSourceLoader.Load(zonesList.ToArray, loadOptions)
        Dim resultJson = JsonConvert.SerializeObject(result)

        Return Content(resultJson, "application/json")

    End Function

    Public Sub LoadZones()
        Dim zonesList As New List(Of roZone)
        Dim tbZones As DataTable

        tbZones = API.ZoneServiceMethods.GetZones(Nothing, 0) 'No se le pasa el ID para evitar el filtraje por AccessGroup en BD al recuperar las zonas

        For Each rowZone As DataRow In tbZones.Rows
            If rowZone("Name") <> "Mi Empresa" And rowZone("ID").ToString() <> UNESPECIFIED_ZONE Then
                Dim zoneUpdated As New roZone
                zoneUpdated.ID = rowZone("ID")
                zoneUpdated.Name = rowZone("Name")
                If rowZone("Description") IsNot DBNull.Value Then
                    zoneUpdated.Description = rowZone("Description")
                Else
                    zoneUpdated.Description = String.Empty
                End If

                If rowZone("WorkCenter") IsNot DBNull.Value Then
                    zoneUpdated.WorkCenter = rowZone("WorkCenter")
                Else
                    zoneUpdated.WorkCenter = String.Empty
                End If

                If rowZone("MapInfo") IsNot DBNull.Value Then
                    zoneUpdated.GoogleMapInfo = JsonConvert.DeserializeObject(Of roGoogleMapInfo)(rowZone("MapInfo"))
                Else
                    zoneUpdated.GoogleMapInfo = Nothing
                End If

                If rowZone("Color") IsNot DBNull.Value Then
                    zoneUpdated.Color = rowZone("Color")
                Else
                    zoneUpdated.GoogleMapInfo = Nothing
                End If

                If rowZone("Capacity") IsNot DBNull.Value Then
                    zoneUpdated.Capacity = rowZone("Capacity")
                Else
                    zoneUpdated.Capacity = Nothing
                End If

                If rowZone("IpsRestriction") IsNot DBNull.Value AndAlso roTypes.Any2String(rowZone("IpsRestriction")) <> String.Empty Then
                    zoneUpdated.IpsRestriction = roTypes.Any2String(rowZone("IpsRestriction")).Split("@").ToList()
                Else
                    zoneUpdated.IpsRestriction = New List(Of String)
                End If

                zonesList.Add(zoneUpdated)
            End If
        Next

        ViewBag.Zones = zonesList

    End Sub

    Public Function GetIsWorkingZoneValues(ByVal loadOptions As DataSourceLoadOptions) As ActionResult

        Dim test As Dictionary(Of Int32, String) = New Dictionary(Of Int32, String) From {{1, "Sí"}, {2, "No"}} ' S''haurien d''agafar aquests valors de BD o d'algun diccionari taula global de l'APP

        Dim result = DataSourceLoader.Load(test, loadOptions)
        Dim resultJson = JsonConvert.SerializeObject(result)
        Return Content(resultJson, "application/json")
    End Function

    <HttpDelete>
    Public Function DeleteZone(ByVal key As Integer) As ActionResult

        Dim List As New roZone()

        If Not API.ZoneServiceMethods.DeleteAccessZone(Nothing, key, True) Then
            Return New HttpStatusCodeResult(HttpStatusCode.BadRequest, API.ZoneServiceMethods.LastErrorText)
        End If
        Return New HttpStatusCodeResult(HttpStatusCode.OK)

    End Function

    <HttpPost>
    Public Function SetIpRestrictionStatus(status As Boolean) As JsonResult
        Dim bResult As Boolean = API.ZoneServiceMethods.SetIpRestrictionStatus(Nothing, status)

        If bResult Then
            Return Json(bResult)
        Else
            Return Json(API.ZoneServiceMethods.LastErrorText)
        End If

    End Function
End Class

Class ZoneType

    Public Sub New()
    End Sub

    Public Property Name As String
    Public Property ID As Integer
End Class

Class DropdownElement

    Public Sub New(ByVal id As String, ByVal name As String)
        Me.Id = id
        Me.DisplayName = name
    End Sub

    Public Property DisplayName As String
    Public Property Id As String
End Class