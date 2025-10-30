Imports System.Drawing
Imports System.IO
Imports System.Web.Hosting
Imports System.Web.Mvc
Imports DevExpress.DataProcessing
Imports DevExtreme.AspNet.Data
Imports DevExtreme.AspNet.Mvc
Imports Newtonsoft.Json
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTBusiness.Zone
Imports Robotics.Base.VTEmployees
Imports Robotics.Security.Base
Imports Robotics.VTBase
Imports Robotics.Web.Base
Imports VTLive40.Controllers.Base

<PermissionsAtrribute(FeatureAlias:="Employees", Permission:=Robotics.Base.DTOs.Permission.Read)>
<LoggedInAtrribute(Requiered:=True)>
Public Class ZonesStatusController
    Inherits BaseController
    Private oLanguage As roLanguageWeb

    Private requieredLabels = {"roZonesTitle", "roZoneInfo", "roZoneStatus", "roNoData", "roStartLoading", "roStartUser",
        "roStartDetails", "roStartTelecommute", "roStartCC", "roStartTask", "roStartHour", "roCurrentZone", "roZoneStatusNowPresent",
        "roZoneStatusLastHour"}

    Private oMapsKey = "AIzaSyDEu7Qo0HTVOFAw3xDla35s_wMhvY7qiGw"

    Private Const UNESPECIFIED_ZONE As String = "255"

    Function Index() As ActionResult
        Me.InitializeBase(CardTreeTypes.Zones, TabBarButtonTypes.Zones, "Zone", requieredLabels, "LiveGUI") _
                          .SetBarButton(BarButtonTypes.Zones) _
                          .SetViewInfo("LiveGUI", "Zones", "Title", "Title", "Base/Images/StartMenuIcos/ZonesStatus.png", "TitleDesc")

        LoadZones()
        ViewBag.TelecommutingInstalled = HelperSession.GetFeatureIsInstalledFromApplication("Feature\Telecommuting")
        ViewBag.RootUrl = ConfigurationManager.AppSettings("RootUrl")

        Return View("index")

    End Function

    Public Function GetGoogleAPIKey() As String
        Return oMapsKey
    End Function

    Public Function ScriptsVersion() As String
        Return "?v=" & Robotics.VTBase.roTypes.Any2String(Reflection.Assembly.GetExecutingAssembly().GetCustomAttributes(GetType(Reflection.AssemblyDescriptionAttribute), False).FirstOrDefault().Description)
    End Function

    Public Function GetEmployeesInZoneDuringLastHour(ByVal loadOptions As DataSourceLoadOptions, ByVal hasData As String, ByVal idZone As String) As ActionResult
        Dim employeeDashboardList As New List(Of EmployeesDashboardInfo)

        If (idZone IsNot Nothing) Then
            Dim isConsultor As Boolean = False

            Dim oEmpState As New Employee.roEmployeeState(WLHelperWeb.CurrentPassport.ID)

            If WLHelperWeb.CurrentUserIsConsultantOrCegid Then
                isConsultor = True
            End If

            Dim zone = roJSONHelper.Deserialize(idZone, GetType(Integer))
            employeeDashboardList = LoadEmployeesInZoneDuringLastHour(WLHelperWeb.CurrentPassport.ID, zone, isConsultor)

            Dim result = DataSourceLoader.Load(employeeDashboardList.ToArray, loadOptions)
            Dim resultJson = JsonConvert.SerializeObject(result)

            Return Content(resultJson, "application/json")
        Else
            Dim result = DataSourceLoader.Load(employeeDashboardList.ToArray, loadOptions)
            Dim resultJson = JsonConvert.SerializeObject(result)
            Return Content(resultJson, "application/json")
        End If

    End Function

    Public Function LoadEmployeesInZoneDuringLastHour(ByVal idPassport As Integer, ByVal zone As Integer, Optional ByVal isConsultor As Boolean = False) As List(Of EmployeesDashboardInfo)

        Dim oRet As New List(Of EmployeesDashboardInfo)

        Try
            Dim employeeIdList As String = ""

            ' Empleados que han estado en la zona en la última hora, pero ya no están
            Dim tEmployeesInZoneDuringLastHourInfo As DataTable
            tEmployeesInZoneDuringLastHourInfo = roBusinessSupport.GetEmployeesInZoneDuringLastHour(idPassport, zone)

            Dim oEmployeeDashboardInfo As EmployeesDashboardInfo

            If Not tEmployeesInZoneDuringLastHourInfo Is Nothing AndAlso tEmployeesInZoneDuringLastHourInfo.Rows.Count > 0 Then

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

                Dim oEmpState As New Employee.roEmployeeState(-1)

                For Each oRow As DataRow In tEmployeesInZoneDuringLastHourInfo.Rows
                    oEmployeeDashboardInfo = New EmployeesDashboardInfo
                    oEmployeeDashboardInfo.IdEmployee = roTypes.Any2String(oRow("IDEmployee"))
                    oEmployeeDashboardInfo.EmployeeName = roTypes.Any2String(oRow("Name"))
                    oEmployeeDashboardInfo.LastPunch = roTypes.Any2String(oRow("DateTimeOut"))
                    oEmployeeDashboardInfo.Image = LoadEmployeeImage(oRow("EmployeeImage"), oEmpState, ImageData)
                    oEmployeeDashboardInfo.RealLastPunch = roTypes.Any2DateTime(oRow("DateTimeOut"))
                    ' Hora del último fichaje, en formato HH:mm (+/- x d)
                    oEmployeeDashboardInfo.LastPunchFormattedDateTime = ""
                    Dim iDaysDiff As Integer = 0
                    If Not IsDBNull(oRow("ShiftDateOut")) AndAlso Not IsDBNull(oRow("DateTimeOut")) Then
                        iDaysDiff = Now.Date.Subtract(roTypes.Any2DateTime(oRow("ShiftDateOut")).Date).TotalDays
                        If iDaysDiff > 0 Then
                            If iDaysDiff < 10 Then
                                oEmployeeDashboardInfo.LastPunchFormattedDateTime = GetServerLanguage().Translate("roHasPassed", "LiveGUI") & " " & iDaysDiff & "d"
                            Else
                                oEmployeeDashboardInfo.LastPunchFormattedDateTime = ""
                            End If
                        Else
                            oEmployeeDashboardInfo.LastPunchFormattedDateTime = roTypes.Any2Time(oRow("DateTimeOut")).TimeOnly
                        End If
                    Else
                        oEmployeeDashboardInfo.LastPunchFormattedDateTime = ""
                    End If

                    'Datos de teletrabajo

                    oRet.Add(oEmployeeDashboardInfo)
                Next
            End If
            'End If
        Catch ex As Exception
            Dim oLogState As New roBusinessState("Common.BaseState", "LiveGUI")
            oLogState.UpdateStateInfo(ex, "StartController::LoadEmployeesInZoneDuringLastHour:Exception:")
        End Try

        Return oRet

    End Function

    'Llamada que hace el Grid para saber los usuarios presentes en una idZone
    Public Function GetEmployees(ByVal loadOptions As DataSourceLoadOptions, ByVal hasData As String, ByVal idZone As String) As ActionResult
        Dim employeeDashboardList As New List(Of EmployeesDashboardInfo)

        If (idZone IsNot Nothing) Then
            Dim isConsultor As Boolean = False

            Dim oEmpState As New Employee.roEmployeeState(WLHelperWeb.CurrentPassport.ID)

            If WLHelperWeb.CurrentUserIsConsultantOrCegid Then
                isConsultor = True
            End If

            Dim zone = roJSONHelper.Deserialize(idZone, GetType(Integer))
            employeeDashboardList = LoadEmployeesStatus(WLHelperWeb.CurrentPassport.ID, zone, isConsultor)

            Dim result = DataSourceLoader.Load(employeeDashboardList.ToArray, loadOptions)
            Dim resultJson = JsonConvert.SerializeObject(result)

            Return Content(resultJson, "application/json")
        Else
            Dim result = DataSourceLoader.Load(employeeDashboardList.ToArray, loadOptions)
            Dim resultJson = JsonConvert.SerializeObject(result)
            Return Content(resultJson, "application/json")
        End If

    End Function

    Public Function LoadEmployeesStatus(ByVal idPassport As Integer, ByVal zone As Integer, Optional ByVal isConsultor As Boolean = False) As List(Of EmployeesDashboardInfo)

        Dim oRet As New List(Of EmployeesDashboardInfo)

        Try
            Dim employeeIdList As String = ""

            Dim hasTelecommutingInstalled = HelperSession.GetFeatureIsInstalledFromApplication("Feature\Telecommuting")

            ' Empleado en la zona actualmente
            Dim tEmployeesDashboardInfo As DataTable = roBusinessSupport.GetEmployeesZoneStatus(idPassport, zone, isConsultor)

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
                    oEmployeeDashboardInfo.EmployeeName = roTypes.Any2String(oRow("EmployeeName"))
                    oEmployeeDashboardInfo.LastPunch = roTypes.Any2String(oRow("LastPunchDateTime"))
                    oEmployeeDashboardInfo.CostCenterName = roTypes.Any2String(oRow("CostCenterName"))
                    oEmployeeDashboardInfo.TaskName = roTypes.Any2String(oRow("LastTaskName"))
                    oEmployeeDashboardInfo.Image = LoadEmployeeImage(oRow("EmployeeImage"), oEmpState, ImageData)
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
                                oEmployeeDashboardInfo.LastPunchFormattedDateTime = GetServerLanguage().Translate("roHasPassed", "LiveGUI") & " " & iDaysDiff & "d"
                            Else
                                oEmployeeDashboardInfo.LastPunchFormattedDateTime = ""
                            End If
                        Else
                            oEmployeeDashboardInfo.LastPunchFormattedDateTime = roTypes.Any2Time(oRow("LastPunchDatetime")).TimeOnly
                        End If
                    Else
                        oEmployeeDashboardInfo.LastPunchFormattedDateTime = ""
                    End If

                    'Datos de teletrabajo

                    If hasTelecommutingInstalled Then
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
                    Else
                        oEmployeeDashboardInfo.InTelecommuteImage = ""
                    End If

                    oRet.Add(oEmployeeDashboardInfo)
                Next
            End If
            'End If
        Catch ex As Exception
            Dim oLogState As New roBusinessState("Common.BaseState", "LiveGUI")
            oLogState.UpdateStateInfo(ex, "StartController::LoadEmployeesStatus:Exception:")
        End Try

        Return oRet

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

    Public Function LoadEmployeeImage(ByVal objImage As Object, ByVal oEmpState As Employee.roEmployeeState, ByVal defaultImage As Byte()) As String
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

    Public Function MakeThumbnail(ByVal myImage As Byte(), ByVal thumbWidth As Integer, ByVal thumbHeight As Integer) As Byte()
        Using ms As MemoryStream = New MemoryStream()

            Using thumbnail As Image = Image.FromStream(New MemoryStream(myImage)).GetThumbnailImage(thumbWidth, thumbHeight, Nothing, New IntPtr())
                thumbnail.Save(ms, Drawing.Imaging.ImageFormat.Png)
                Return ms.ToArray()
            End Using
        End Using
    End Function

    'Llamada para pintar el mapa y todos sus marcadores
    Public Sub LoadZones()
        Dim zonesList As New List(Of roZone)
        Dim zonesMax As New ZoneStatusMax
        Dim tbZones As DataTable

        tbZones = API.ZoneServiceMethods.GetZones(Nothing, 0) 'No se le pasa el ID para evitar el filtraje por AccessGroup en BD al recuperar las zonas

        For Each rowZone As DataRow In tbZones.Rows
            If rowZone("Name") <> "Mi Empresa" Then
                Dim zoneUpdated As New roZone
                zoneUpdated.ID = rowZone("ID")
                zoneUpdated.Name = rowZone("Name")

                Dim currentIn As Integer = 0
                currentIn = roBusinessSupport.GetEmployeesZoneStatus(WLHelperWeb.CurrentPassport.ID, zoneUpdated.ID).Rows.Count
                Dim currentOut As Integer = 0
                currentOut = roBusinessSupport.GetEmployeesInZoneDuringLastHour(WLHelperWeb.CurrentPassport.ID, zoneUpdated.ID).Rows.Count
                'currentOut = roZone.GetEmployeesOutOfZoneLastHour(zoneUpdated.ID, New VTBusiness.Zone.roZoneState(-1))

                zoneUpdated.CurrentPeopleIn = currentIn
                zoneUpdated.CurrentPeopleOut = currentOut

                zoneUpdated.NameExtended = zoneUpdated.Name & " (" & currentIn & ")"

                zoneUpdated.CurrentPeopleInDesc = GetServerLanguage().Translate("roPresent", "LiveGUI") & ": " & currentIn.ToString
                zoneUpdated.CurrentPeopleOutDesc = GetServerLanguage().Translate("roAbsent", "LiveGUI") & ": " & currentOut.ToString

                If Not rowZone("Description") Is DBNull.Value Then
                    zoneUpdated.Description = rowZone("Description")
                Else
                    zoneUpdated.Description = String.Empty
                End If

                If Not rowZone("WorkCenter") Is DBNull.Value Then
                    zoneUpdated.WorkCenter = rowZone("WorkCenter")
                Else
                    zoneUpdated.WorkCenter = String.Empty
                End If

                If Not rowZone("MapInfo") Is DBNull.Value Then
                    zoneUpdated.GoogleMapInfo = JsonConvert.DeserializeObject(Of roGoogleMapInfo)(rowZone("MapInfo"))
                Else
                    zoneUpdated.GoogleMapInfo = Nothing
                End If

                If Not rowZone("Color") Is DBNull.Value Then
                    zoneUpdated.Color = rowZone("Color")
                Else
                    zoneUpdated.GoogleMapInfo = Nothing
                End If

                If Not rowZone("Capacity") Is DBNull.Value Then
                    zoneUpdated.Capacity = rowZone("Capacity")
                    zoneUpdated.CapacityDesc = GetServerLanguage().Translate("roCapacity", "LiveGUI") & " " & zoneUpdated.Capacity.ToString
                Else
                    zoneUpdated.Capacity = Nothing
                    zoneUpdated.CapacityDesc = GetServerLanguage().Translate("roNoCapacity", "LiveGUI")
                End If

                zonesList.Add(zoneUpdated)
            End If
        Next

        Dim latitudMax As Single = 0
        Dim longitudMax As Single = 0
        Dim latitudMin As Single = 10000
        Dim longitudMin As Single = 10000

        For Each zone In zonesList
            If zone.GoogleMapInfo IsNot Nothing Then
                For Each zoneCoordinate In zone.GoogleMapInfo.Coordinates
                    If zoneCoordinate.Latitud > latitudMax Then
                        latitudMax = zoneCoordinate.Latitud
                    End If
                    If zoneCoordinate.Longitud > longitudMax Then
                        longitudMax = zoneCoordinate.Longitud
                    End If

                    If zoneCoordinate.Latitud < latitudMin Then
                        latitudMin = zoneCoordinate.Latitud
                    End If
                    If zoneCoordinate.Longitud < longitudMin Then
                        longitudMin = zoneCoordinate.Longitud
                    End If

                Next
            End If

        Next

        zonesMax.MaxLatitude = (CDec(latitudMax) + CDec(latitudMin)) / CDec(2)
        zonesMax.MaxLongitude = (CDec(longitudMax) + CDec(longitudMin)) / CDec(2)

        ViewBag.ZonaGlobal = JsonConvert.SerializeObject(zonesMax)
        ViewBag.Zones = JsonConvert.SerializeObject(zonesList)
        ViewBag.ZonesLite = zonesList.OrderByDescending(Function(x) x.CurrentPeopleIn)

    End Sub

    'Public Function GetZonesLite(ByVal loadOptions As DataSourceLoadOptions) As ActionResult

    '    Dim zonesList As New List(Of roZoneLite)
    '    Dim tbZones = API.ZoneServiceMethods.GetZones(Nothing, 0)

    '    For Each rowZone As DataRow In tbZones.Rows
    '        If rowZone("Name") <> "Mi Empresa" AndAlso rowZone("Name") <> "Sin especificar" Then
    '            Dim zoneUpdated As New roZoneLite
    '            zoneUpdated.Id = rowZone("ID")
    '            zoneUpdated.Name = rowZone("Name")
    '            zonesList.Add(zoneUpdated)
    '        End If
    '    Next

    '    If tbZones IsNot Nothing Then
    '        Dim result = DataSourceLoader.Load(zonesList, loadOptions)
    '        Dim resultJson = JsonConvert.SerializeObject(result)
    '        Return Content(resultJson, "application/json")
    '    Else
    '        Return Nothing
    '    End If

    'End Function

End Class