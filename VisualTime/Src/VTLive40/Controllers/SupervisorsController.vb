Imports System.Web.Mvc
Imports VTLive40.Controllers.Base

<PermissionsAtrribute(FeatureAlias:="Administration.Security", Permission:=Robotics.Base.DTOs.Permission.Read)>
<LoggedInAtrribute(Requiered:=True)>
Public Class SupervisorsController
    Inherits BaseController

    Private requieredLabels = {"Title", "SaveChanges", "UndoChanges", "DataModified"}
    '{"Title", "SaveChanges", "UndoChanges", "DataModified", "OldGenius", "MainSection", "RunGenius", "Results", "LastRunsInfo", "RunDesc", "shareView", "whoShares", "selectUser", "shareDescription", "whoAnalyze", "selectUsers", "roAnalyzeGroups", "roAnalyzeUsers", "roSurveyUsers", "roUsersAdvanced", "createReport", "reportName", "dataType", "costCenterSelector", "conceptsSelector", "requestSelector", "lblUserFields", "txtUserFields", "lblConfLanguage", "lblgeniusviewcreate", "costCenterLbl", "selectCostCenter", "conceptsLbl", "selectConcepts", "requestsLbl", "selectRequests", "lblConvert2SystemView", "timeLbl"}

    Function Index() As ActionResult

        Me.InitializeBase(CardTreeTypes.Supervisors, TabBarButtonTypes.Supervisors, "Supervisors", requieredLabels, "LiveSecurity") _
                          .SetBarButton(BarButtonTypes.Supervisors) _
                          .SetViewInfo("LiveSecurity", "Supervisors", "Title", "Title", "Base/Images/StartMenuIcos/employees.png", "", "roles")
        ViewData(Helpers.Constants.CustomChangeTab) = "SupervisorsChangeTab"

        Return View("index")
    End Function

End Class