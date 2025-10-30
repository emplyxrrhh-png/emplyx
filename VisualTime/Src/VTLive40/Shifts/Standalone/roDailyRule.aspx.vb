Imports Robotics.Base.DTOs
Imports Robotics.Web.Base
Imports Robotics.Web.Base.API

Partial Class VTLive40_roDailyRule
    Inherits PageBase

    Private Sub Base_WebUserControls_roDestinationSelectorV2_Init(sender As Object, e As EventArgs) Handles Me.Init
        Me.OverrrideDefaultScope = "Shifts"


        Me.InsertExtraJavascript("Ajax", "~/Base/Scripts/Ajax.js")
        Me.InsertExtraJavascript("jsDate", "~/Base/Scripts/jsDate.js")

        Me.InsertExtraJavascript("GenericData", "~/Base/Scripts/GenericData.js")
        Me.InsertExtraJavascript("webUserForms", "~/Base/Scripts/webUserForms.js")
        Me.InsertExtraJavascript("jsGrid", "~/Base/Scripts/jsGrid.js")
        Me.InsertExtraJavascript("roOptionPanelClient", "~/Base/Scripts/roOptionPanelClient.js")
        Me.InsertExtraJavascript("roOptPanelClientGroup", "~/Base/Scripts/roOptPanelClientGroup.js")

        Me.InsertExtraJavascript("roDailyRuleStd", "~/Shifts/Standalone/roDailyRule.js")
        Me.InsertExtraJavascript("frmDailyRule", "~/Shifts/Scripts/frmDailyRule.js")
    End Sub

    Protected Sub Page_Load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load


        If Not Me.IsPostBack Then

        End If

    End Sub


End Class