Imports System.Web.Mvc
Imports Newtonsoft.Json
Imports Robotics.Base.DTOs
Imports Robotics.VTBase
Imports Robotics.Web.Base
Imports VTLive40.Controllers.Base

<PermissionsAtrribute(FeatureAlias:="Administration.Options", Permission:=Permission.Read)>
<LoggedInAtrribute(Requiered:=True)>
Public Class RulesGroupController
    Inherits BaseController

    Private requiredLabels = {"Title", "lblName", "lblDesc", "GeneralHeader", "lblType", "lblGroup", "lblTag",
        "DefinitionHeader", "lblAvailableFrom", "lblContext", "lblShifts", "roRuleDashboard_View", "roRuleDashboard_Rule", "roRuleDashboard_Shift", "roRuleDashboard_WriteToSearch", "roRuleDashboard_SelectDate",
        "roRuleDashboard_Shifts", "roRuleDashboard_Types", "roRuleDashboard_Tags", "roRuleDashboard_btnEdit", "roRuleDashboard_btnOrder", "roRuleDashboard_UnknownError", "roRuleDashboard_NewGroup",
        "roRuleDetail_HistoryTitle", "roRuleDashboard_NewHistoricRule", "roRuleDashboard_ChangesDetected", "roRuleDashboard_ValidateSave", "roRuleDashboard_ConfirmSave", "roRuleDashboard_CancelByUser",
        "roRuleDashboard_SaveOK", "roRuleDashboard_Attention", "roRuleDashboard_WarnAboutChanges", "lblWhoApplies", "selectWhoApply", "lblConditions", "lblActions", "roPendingChangesOnHistory"}


    Function Index() As ActionResult
        Me.InitializeBase(CardTreeTypes.RulesGroup, TabBarButtonTypes.RulesGroup, "RulesGroup", requiredLabels, "LiveRules") _
                          .SetBarButton(BarButtonTypes.RulesGroup) _
                          .SetViewInfo("LiveRules", "RulesGroup", "Title", "Title", "Base/Images/StartMenuIcos/RulesGroup.png", "TitleDesc")
        Return View("index")
    End Function


    <HttpPost>
    <IsoDateJsonResult>
    Function GetDashboard(filter As RulesGroupFilter) As JsonResult
        Dim rulesGroups As roRulesGroup() = API.RulesServiceMethods.GetDummyRulesGroups(Nothing, False)

        If API.RulesServiceMethods.LastMessageResult() = RulesResult.NoError Then
            Dim result As New With {
                .Data = rulesGroups,
                .Success = True,
                .ErrorText = String.Empty
            }
            Return Json(result)
        Else
            Dim result As New With {
               .Data = Nothing,
               .Success = False,
               .ErrorText = API.RulesServiceMethods.LastMessageErrorText()
           }
            Return Json(result)
        End If

    End Function

    <HttpPost>
    <IsoDateJsonResult>
    Function SaveDashboard(scenario As roRulesGroup(), changes As roRuleChangeHistory()) As JsonResult
        If True Then
            Dim result As New With {
                .Data = Nothing,
                .Success = True,
                .ErrorText = String.Empty
            }
            Return Json(result)
        Else
            Dim result As New With {
               .Data = Nothing,
               .Success = False,
               .ErrorText = "Error validando"
           }
            Return Json(result)
        End If

    End Function

End Class