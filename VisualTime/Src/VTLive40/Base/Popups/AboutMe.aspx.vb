Imports Robotics.VTBase
Imports Robotics.Web.Base

Partial Class Base_AboutMe
    Inherits PageBase

    Private strObjectType As String = ""

    Protected Sub form1_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles form1.Load

        If Not Me.IsPostBack Then
            strObjectType = roTypes.Any2String(Me.Request("ObjectType"))

            Select Case strObjectType
                Case "Concept"
                    Me.Img1.Src = Me.ResolveUrl("~/Concepts/Images/Acumulados80.png")
                Case "ConceptGroup"
                    Me.Img1.Src = Me.ResolveUrl("~/Base/Images/StartMenuIcos/ConceptGroups.png")
                Case "DocumentAbsence"
                    Me.Img1.Src = Me.ResolveUrl("~/Absences/Images/DocumentsAbsences80.png")
                Case "Activity"
                    Me.Img1.Src = Me.ResolveUrl("~/Base/Images/StartMenuIcos/Activity.png")
                Case "Indicator"
                    Me.Img1.Src = Me.ResolveUrl("~/Base/Images/StartMenuIcos/Indicators.png")
                Case "LabAgree"
                    Me.Img1.Src = Me.ResolveUrl("~/Base/Images/StartMenuIcos/LabAgree.png")
                Case "LabAgreeRule"
                    Me.Img1.Src = Me.ResolveUrl("~/Base/Images/StartMenuIcos/LabAgreeRules.png")
                Case "LabAgreeStartupValue"
                    Me.Img1.Src = Me.ResolveUrl("~/Base/Images/StartMenuIcos/StartupValues.png")
                Case "AccessGroup"
                    Me.Img1.Src = Me.ResolveUrl("~/Base/Images/StartMenuIcos/AccessGroups.png")
                Case "AccessPeriod"
                    Me.Img1.Src = Me.ResolveUrl("~/Base/Images/StartMenuIcos/AccessPeriods.png")
                Case "AccessZone"
                    Me.Img1.Src = Me.ResolveUrl("~/Base/Images/StartMenuIcos/AccessZones.png")
                Case "Camera"
                    Me.Img1.Src = Me.ResolveUrl("~/Base/Images/StartMenuIcos/Cameras.png")
                Case "Cause"
                    Me.Img1.Src = Me.ResolveUrl("~/Base/Images/StartMenuIcos/Causes.png")
                Case "DiningRoom"
                    Me.Img1.Src = Me.ResolveUrl("~/Base/Images/StartMenuIcos/DiningRoom.png")
                Case "Notification"
                    Me.Img1.Src = Me.ResolveUrl("~/Base/Images/StartMenuIcos/Notificaciones.png")
                Case "ReportScheduler"
                    Me.Img1.Src = Me.ResolveUrl("~/Base/Images/StartMenuIcos/ReportScheduler.png")
                Case "Assignment"
                    Me.Img1.Src = Me.ResolveUrl("~/Base/Images/StartMenuIcos/Assignment.png")
                Case "Shift"
                    Me.Img1.Src = Me.ResolveUrl("~/Base/Images/StartMenuIcos/Shifts.png")
                Case "Project", "DocumentTemplate"
                    Me.Img1.Src = Me.ResolveUrl("~/Base/Images/StartMenuIcos/TaskTemplates.png")
                Case "TaskTemplate"
                    Me.Img1.Src = Me.ResolveUrl("~/Base/Images/StartMenuIcos/TaskTemplates.png")
                Case "BusinessCenter"
                    Me.Img1.Src = Me.ResolveUrl("~/Base/Images/StartMenuIcos/BusinessCenters.png")
                Case "EventScheduler"
                    Me.Img1.Src = Me.ResolveUrl("~/Base/Images/StartMenuIcos/Events.png")
                Case "SecurityFunction"
                    Me.Img1.Src = Me.ResolveUrl("~/Base/Images/StartMenuIcos/SecurityFunctions.png")
                Case "ActivityDoc"
                    Me.Img1.Src = Me.ResolveUrl("~/Base/Images/StartMenuIcos/DocumentActivity.png")
                Case "EmployeeReportScheduler"
                    Me.Img1.Src = Me.ResolveUrl("~/Base/Images/StartMenuIcos/EmployeeReportScheduler.png")
                Case "ProductiveUnit"
                    Me.Img1.Src = Me.ResolveUrl("~/Base/Images/StartMenuIcos/ProductiveUnit.png")
                Case "EmployeeAnalyticsScheduler"
                    Me.Img1.Src = Me.ResolveUrl("~/Base/Images/StartMenuIcos/EmployeeAnalyticsScheduler.png")
            End Select

            Me.lblTitle.Text = Me.Language.Translate("Title" & strObjectType, Me.DefaultScope)
            Me.lblDescription1.Text = Me.Language.Translate("Description" & strObjectType, Me.DefaultScope)
            Me.Img1.Width = 48
            Me.Img1.Height = 48

            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "refreshInitGrid", "lblError_Client.SetVisible(false);", True)
        End If

    End Sub

End Class