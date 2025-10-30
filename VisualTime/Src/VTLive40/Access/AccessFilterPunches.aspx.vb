Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Zone
Imports Robotics.Base.VTEmployees.Employee
Imports Robotics.Web.Base

Partial Class AccessFilterPunches
    Inherits PageBase

    Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        Me.InsertExtraJavascript("Cookies", "~/Base/Scripts/Cookies.js", , True)
        Me.InsertExtraJavascript("OpenWindow", "~/Base/Scripts/OpenWindow.js", , True)
        Me.InsertExtraJavascript("jsGrid", "~/Base/Scripts/jsGrid.js", , True)
        Me.InsertExtraJavascript("roTreeState", "~/Base/Scripts/rocontrols/roTrees/roTreeState.js")
        Me.InsertExtraJavascript("AccessFilterData", "~/Access/Scripts/AccessFilterData.js", , True)
    End Sub

    Protected Sub Page_Load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.InsertCssIncludes(Me.Page)

        If Not Me.HasFeaturePermission("Access.Zones.Supervision", Permission.Read) Then
            WLHelperWeb.RedirectAccessDenied(True)
            Exit Sub
        End If

        'Busquem l'usuari
        If Me.Request("IDEmployee") <> "" And Me.Request("IDEmployee") <> "ALL" Then
            Dim oEmp As roEmployee = API.EmployeeServiceMethods.GetEmployee(Me.Page, Request("IDEmployee"), False)
            If oEmp IsNot Nothing Then
                aFEmployees.InnerText = oEmp.Name
            End If
            hdnEmployees.Value = oEmp.ID
        End If

        'Busquem el nom de la zona
        If Me.Request("IDZone") <> "" And Me.Request("IDZone") <> "ALL" Then
            Dim oZone As roZone = API.ZoneServiceMethods.GetZoneByID(Me.Page, Request("IDZone"), False)
            If oZone IsNot Nothing Then
                aFZones.InnerText = oZone.Name
            End If
            hdnZones.Value = oZone.ID
        End If

        txtHourBegin.Value = "00:00"
        txtHourEnd.Value = "23:59"

        dpDateBegin.Value = Now.Date
        dpDateEnd.Value = Now.Date

        If Not IsPostBack Then
            'INICIALIZAR COOKIES DE USO PARA EL SELECTOR DE EMPLEADOS
            HelperWeb.roSelector_Initialize("objContainerTreeV3_treeEmpFilterPunches")
            HelperWeb.roSelector_Initialize("objContainerTreeV3_treeEmpFilterPunchesGrid")
        End If

    End Sub

End Class