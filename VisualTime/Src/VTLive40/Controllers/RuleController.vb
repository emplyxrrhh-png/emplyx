Imports System.Web.Mvc
Imports Newtonsoft.Json
Imports Robotics.Base.DTOs
Imports Robotics.VTBase
Imports Robotics.Web.Base
Imports VTLive40.Controllers.Base

<PermissionsAtrribute(FeatureAlias:="Administration.Options", Permission:=Permission.Read)>
<LoggedInAtrribute(Requiered:=True)>
Public Class RuleController
    Inherits BaseController

    Private requiredLabels = {"Title"}

    Function Index() As ActionResult
        Me.InitializeBase(CardTreeTypes.Rule, TabBarButtonTypes.Rule, "RulesGroup", requiredLabels, "LiveRules") _
                          .SetBarButton(BarButtonTypes.Rule) _
                          .SetViewInfo("LiveRules", "RulesGroup", "Title", "Title", "Base/Images/StartMenuIcos/RulesGroup.png", "TitleDesc")

        Return View("index")
    End Function


    Function DailyRuleWrapper() As ActionResult
        Me.InitializeBase(CardTreeTypes.Rule, TabBarButtonTypes.Rule, "RulesGroup", requiredLabels, "LiveRules") _
                          .SetBarButton(BarButtonTypes.Rule) _
                          .SetViewInfo("LiveRules", "RulesGroup", "Title", "Title", "Base/Images/StartMenuIcos/RulesGroup.png", "TitleDesc")

        Return View("DailyRuleWrapper")
    End Function


    <HttpPost>
    <IsoDateJsonResult>
    Function ValidateRuleHistory(historyEntry As roRuleDefinition) As JsonResult
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