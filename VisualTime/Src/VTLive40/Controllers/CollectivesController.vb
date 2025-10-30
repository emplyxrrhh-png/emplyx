Imports System.Web.Mvc
Imports DevExpress.CodeParser
Imports Newtonsoft.Json
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTUserFields.UserFields
Imports Robotics.VTBase
Imports Robotics.Web.Base
Imports VTLive40.Controllers.Base

<PermissionsAtrribute(FeatureAlias:="Administration.Security", Permission:=Robotics.Base.DTOs.Permission.Read)>
<LoggedInAtrribute(Requiered:=True)>
Public Class CollectivesController
    Inherits BaseController

    Private requieredLabels = {"Title", "GeneralHeader", "DefinitionHeader", "FilterHeader", "lblName", "lblDesc", "roCollectives_PendingChangesOnDefinition", "rocollectives_historictitle", "rocollectives_pendingchangesoncollective", "rocollectives_savebeforenewcollective", "rocollectives_savebeforenewdefinition", "DefinitionDateLabel", "ModifiedBy1", "ModifiedBy2", "ModifiedBy3", "roCollectives_viewBtn", "roCollectives_EmployeeName", "roCollectives_Group", "roCollectives_atDate", "rocollectives_removeduserfields", "roCollectives_EmployeesDataGrid_TotalCount", "roCollectives_EmployeesDataGrid_GroupCount"}

    Function Index() As ActionResult

        Me.InitializeBase(CardTreeTypes.Collectives, TabBarButtonTypes.Collectives, "Collectives", requieredLabels, "LiveCollectives") _
                          .SetBarButton(BarButtonTypes.Collectives) _
                          .SetViewInfo("LiveCollectives", "Collectives", "Title", "Title", "Base/Images/StartMenuIcos/Groups.png", "", "roles")
        ViewData(Helpers.Constants.CustomChangeTab) = "CollectivesChangeTab"

        Return View("index")
    End Function

    <HttpGet>
    Public Function GetCollectiveView(ByVal idView As Integer) As ActionResult

        If (idView > -1) Then
            Dim oCollective = API.CollectiveServiceMethods.GetCollective(idView, Nothing)
            ViewBag.Collective = oCollective
        Else
            ViewBag.Collective = API.CollectiveServiceMethods.GetCollective(-1, Nothing)
        End If
        Return View("index")
    End Function

    <HttpPost>
    <IsoDateJsonResult>
    Public Function GetCollective(ByVal idCollective As Integer) As JsonResult
        Dim oCollective As roCollective = API.CollectiveServiceMethods.GetCollective(roTypes.Any2Integer(idCollective), Nothing, True)

        If oCollective IsNot Nothing AndAlso oCollective.Id > 0 Then
            Return Json(oCollective)
        Else
            Return Json(roWsUserManagement.SessionObject().States.CollectiveState.ErrorText)
        End If

    End Function

    <HttpPost>
    <IsoDateJsonResult>
    Function CreateOrUpdateCollective(oCollectiveParam As roCollective) As JsonResult
        If (API.CollectiveServiceMethods.CreateOrUpdateCollective(Nothing, oCollectiveParam, True)) Then
            Return Json(oCollectiveParam)
        End If

        Return Json(roWsUserManagement.SessionObject().States.CollectiveState.ErrorText)
    End Function

    <HttpPost>
    <IsoDateJsonResult>
    Function ValidateCollectiveDefinition(oCollectiveDefinitionParam As roCollectiveDefinition) As JsonResult
        Dim oCollectiveResult = API.CollectiveServiceMethods.ValidateCollectiveDefinition(Nothing, oCollectiveDefinitionParam)
        If oCollectiveResult Then
            Return Json(oCollectiveResult)
        Else
            Return Json(roWsUserManagement.SessionObject().States.CollectiveState.ErrorText)
        End If
    End Function

    <HttpPost>
    Public Function GetEmployeeUserfields() As ActionResult
        Dim oFields As roUserField() = API.CollectiveServiceMethods.GetEmployeeUserFields(Nothing)
        Dim resultJson = JsonConvert.SerializeObject(oFields)

        If oFields IsNot Nothing Then
            Return Content(resultJson, "application/json")
        Else
            Return Json(roWsUserManagement.SessionObject().States.CollectiveState.ErrorText)
        End If

    End Function

    <HttpPost>
    <IsoDateJsonResult>
    Public Function GetCollectiveEmployees(ByVal filterExpression As String, ByVal refdate As DateTime) As ActionResult
        Dim oCollectiveEmployees As List(Of roSelectedEmployee) = API.CollectiveServiceMethods.GetCollectiveEmployees(filterExpression, refdate, Nothing)
        Dim resultJson = JsonConvert.SerializeObject(oCollectiveEmployees)

        If oCollectiveEmployees IsNot Nothing Then
            Return Content(resultJson, "application/json")
        Else
            Return Json(roWsUserManagement.SessionObject().States.CollectiveState.ErrorText)
        End If

    End Function

    <HttpDelete>
    Function DeleteCollective(idCollective As Integer) As JsonResult
        Dim oCollective As roCollective = API.CollectiveServiceMethods.GetCollective(roTypes.Any2Integer(idCollective), Nothing)
        Dim result As Boolean = API.CollectiveServiceMethods.DeleteCollective(oCollective, Nothing, True)
        If result Then
            Return Json(result)
        Else
            Return Json(roWsUserManagement.SessionObject().States.CollectiveState.ErrorText)
        End If
    End Function

End Class
