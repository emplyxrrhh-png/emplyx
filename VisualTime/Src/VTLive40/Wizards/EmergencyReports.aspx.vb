Imports Robotics.Base.DTOs
Imports Robotics.Web.Base

Partial Class Forms_EmergencyReports
    Inherits PageBase

#Region "Declarations"

    Private Const FeatureAlias As String = "Administration.ReportScheduler.EmergencyReport"
    Private oPermission As Permission

#End Region

#Region "Events"

    Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        Me.InsertExtraJavascript("BrowserDetect", "~/Base/Scripts/BrowserDetect.js", , True)
        Me.InsertExtraJavascript("Cookies", "~/Base/Scripts/Cookies.js", , True)
        Me.InsertExtraJavascript("Generic", "~/Base/Scripts/Generic.js", , True)
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.InsertCssIncludes(Me.Page)

        Me.oPermission = Me.GetFeaturePermission(FeatureAlias)

        If oPermission < Permission.Read Then
            WLHelperWeb.RedirectAccessDenied(True)
        End If

        If Not Me.IsPostBack Then
            LoadData()
        End If

    End Sub

    Public Sub LoadData()

        Me.chkAll.Checked = True
        Me.chkAll.Visible = False

        Dim tmpReport As Robotics.Base.Report = API.ReportServiceMethods.GetEmergencyReport(Me.Page, False)
        If tmpReport IsNot Nothing Then
            For Each oExecution As Robotics.Base.ReportPlannedExecution In tmpReport.PlannedExecutionsList
                Dim jsonObject As Object = Newtonsoft.Json.JsonConvert.DeserializeObject(oExecution.ViewFields)
                Dim schRepName As String = CType(jsonObject, Newtonsoft.Json.Linq.JObject)("description") 'Nombre del report scheduled
                Dim osReport As New roEmergencySchedule With {
                    .Name = schRepName,
                    .ProfileName = "",
                    .ID = oExecution.Id,
                    .ReportName = tmpReport.Name
                }
                AddReport(osReport)
            Next
        End If

        Dim oReport As Robotics.Base.Report = API.ReportServiceMethods.GetEmergencyReport(Me.Page, False)

        If oReport IsNot Nothing Then

            Dim lastExecution As Robotics.Base.ReportExecution = Nothing
            If oReport.ExecutionsList.Count > 0 Then
                lastExecution = oReport.ExecutionsList.Find(Function(x) x.ExecutionDate = (oReport.ExecutionsList.Max(Function(y) y.ExecutionDate)))
            End If

            If lastExecution IsNot Nothing Then
                Dim Str As String = Me.Language.Translate(oReport.Name, DefaultScope) & ": "
                Str = Str & Me.Language.Translate("Launched", DefaultScope) & lastExecution.ExecutionDate

                Dim result As String = ""
                Dim executionStatues As Integer = API.ReportServiceMethods.GetExecutionStatus(lastExecution.Guid, Me.Page).Status
                Select Case executionStatues
                    Case 0 : result = Me.Language.Translate("StateReportEnum.NeverUsed", DefaultScope)
                    Case 1 : result = Me.Language.Translate("StateReportEnum.Executing", DefaultScope)
                    Case 2 : result = Me.Language.Translate("StateReportEnum.EndOK", DefaultScope)
                    Case 3 : result = Me.Language.Translate("StateReportEnum.EndWithErrors", DefaultScope)
                End Select
                Str = Str & ". " & Me.Language.Translate("Result", DefaultScope) & result

                Me.lblLastEmergencyReportExecuted.Text = Str
            Else
                Me.lblLastEmergencyReportExecuted.Text = String.Empty
            End If

        End If

    End Sub

    Protected Sub btOK_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btOK.Click

        Dim strReports As String = ""
        For n As Integer = 0 To chkList.Items.Count - 1
            If chkList.Items(n).Selected Then
                strReports &= chkList.Items(n).Value & ","
            End If
        Next
        If strReports.EndsWith(",") Then strReports = strReports.Substring(0, strReports.Length - 1)

        If strReports <> "" Then
            API.ReportServiceMethods.ExecuteEmergencyReport(Me.Page, True, strReports)

            Me.msgSelectOne.Visible = False
            Me.CanClose = True
        Else
            Me.msgSelectOne.Visible = True
        End If
    End Sub

#End Region

    Protected Sub chkAll_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkAll.CheckedChanged
        For n As Integer = 0 To chkList.Items.Count - 1
            chkList.Items(n).Selected = chkAll.Checked
        Next
    End Sub

    Private Sub AddReport(oRS As roEmergencySchedule)
        Dim oListItem As ListItem

        oListItem = New ListItem(oRS.Name, oRS.ID, True)

        oListItem.Selected = True
        chkList.Items.Add(oListItem)
    End Sub

    Private Function CompareZones(accessZones As String(), reportZones As String()) As Boolean
        Dim includeReport = True
        For Each zone As String In reportZones
            If (Not accessZones.Any(Function(f) f.Equals(zone))) Then includeReport = False
        Next

        Return includeReport
    End Function

End Class