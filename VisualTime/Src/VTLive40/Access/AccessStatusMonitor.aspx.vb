Imports Robotics.Base.DTOs
Imports Robotics.Web.Base

Partial Class AccessStatusMonitor
    Inherits PageBase

    Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        Me.InsertExtraJavascript("jquery", "~/Base/jquery/jquery-3.7.1.min.js", , True)
        Me.InsertExtraJavascript("AccessStatusMonitor", "~/Access/Scripts/AccessStatusMonitor.js")
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If Not Me.IsPostBack Then

            If Not Me.HasFeaturePermission("Access.Zones.Supervision", Permission.Read) Then
                WLHelperWeb.RedirectAccessDenied(False)
                Exit Sub
            End If

            Dim IdZone As String = HelperWeb.GetCookie("showAccessStatusMonitor")
            FillListzones(IdZone)
            SetSelectedZones()

            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "iniMonitor", "IniMonitor();", True)
        End If

    End Sub

    Private Sub FillListzones(ByVal IdZoneSelected As String)
        chkListzones.Items.Clear()
        Dim it As ListItem
        Dim tbZones As DataTable = API.ZoneServiceMethods.GetZones(Me)
        For Each rowZone As DataRow In tbZones.Rows
            If Not rowZone("IDParent") Is DBNull.Value Then
                it = New ListItem(rowZone("Name"), rowZone("ID"))
                If rowZone("ID") = IdZoneSelected Then
                    it.Selected = True
                End If
                chkListzones.Items.Add(it)
            End If
        Next
    End Sub

    Private Sub SetSelectedZones()
        hdnListZones.Value = ""
        For Each it As ListItem In chkListzones.Items
            If it.Selected Then
                hdnListZones.Value = hdnListZones.Value & it.Value & ","
            End If
        Next
        If hdnListZones.Value <> "" Then hdnListZones.Value = hdnListZones.Value.Substring(0, hdnListZones.Value.Length - 1)
    End Sub

    Protected Sub btnListzones_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnListzones.Click
        SetSelectedZones()
    End Sub

End Class