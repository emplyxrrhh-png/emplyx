Imports System.Web.Mvc
Imports Newtonsoft.Json
Imports Robotics.Base.DTOs
Imports Robotics.VTBase
Imports Robotics.Web.Base
Imports VTLive40.Controllers.Base

<PermissionsAtrribute(FeatureAlias:="Bots", Permission:=Permission.Read)>
<LoggedInAtrribute(Requiered:=True)>
Public Class BotsController
    Inherits BaseController

    Private requiredLabels = {"Title", "SaveChanges", "UndoChanges", "DataModified", "CreateBot", "DeleteBot", "type", "whenDescripcion", "condition", "botName", "addNewEmployeeText", "rulesHeader", "roRulesDelete", "roRulesDeleteTitle",
        "roNoData", "roBotConfirmDelete", "roBotNotSaved", "roBotNoViewSelected", "addNewSupervisorText", "TitleDesc", "createRulePanel", "ruleName", "ruleCreationName", "ruleSupervisors", "placeholderSupervisors",
        "ruleEmployees", "placeholderEmployees", "roRuleWithoutName", "roRuleWithoutIdTemplate", "name", "enabled", "addRule", "ruleCopyEmployeeUserFields", "placeholderCopyEmployeesUserFields", "addNewCenterCostText",
        "ruleCopyCenterCostRule", "placeholdercostcenterroles"}

    Function Index() As ActionResult
        Me.InitializeBase(CardTreeTypes.Bots, TabBarButtonTypes.Bots, "Bots", requiredLabels, "LiveBots") _
                          .SetBarButton(BarButtonTypes.Bots) _
                          .SetViewInfo("LiveBots", "Bots", "Title", "Title", "Base/Images/StartMenuIcos/Bots.png", "TitleDesc")

        'ViewBag.AvailableBotTypes = [Enum].GetValues(GetType(BotTypeEnum))
        LoadInitialData()
        Return View("index")
    End Function

    Private Sub LoadInitialData()
        Dim availableSupervisors As New List(Of roAutocompleteItemModel)

        availableSupervisors.AddRange(API.SecurityV3ServiceMethods.GetAllAvailableSupervisorsList(Nothing).Select(Function(user) New roAutocompleteItemModel() With {
                                                        .ID = user.ID,
                                                        .Name = user.Name}))
        ViewBag.Supervisors = availableSupervisors
        ViewBag.AllRules = GetAllRules()
        Try
            ViewBag.AvailableBotTypes = GetBotTypeDescription()
        Catch ex As Exception
            ViewBag.AvailableBotTypes = [Enum].GetValues(GetType(BotTypeEnum)).Cast(Of BotTypeEnum)().Select(Function(i) New With {
          .Id = CInt(i),
          .Name = i.ToString()
      })
        End Try
    End Sub

    Private Function GetBotTypeDescription() As Object
        Dim oLanguageFile = GetServerLanguage("LiveBots")

        Return [Enum].GetValues(GetType(BotTypeEnum)).Cast(Of BotTypeEnum)().Select(Function(botEnum) New IdNamePair With {
            .Id = CInt(botEnum),
            .Name = oLanguageFile.Translate("BotsTypeEnum." & botEnum.ToString(), "Bots")
        })

    End Function

    <HttpPost>
    Public Function GetBot(ByVal idBot As Integer) As ActionResult
        Dim oBot As roBot = API.BotsServiceMethods.GetBot(roTypes.Any2Integer(idBot), Nothing)
        Dim resultJson = JsonConvert.SerializeObject(oBot)

        If oBot IsNot Nothing AndAlso oBot.Id > 0 Then
            Return Content(resultJson, "application/json")
        Else
            Return Json(roWsUserManagement.SessionObject().States.BotState.ErrorText)
        End If

    End Function

    <HttpPost>
    Function CreateOrUpdateBot(oBotParam As roBot) As ActionResult
        If (API.BotsServiceMethods.CreateOrUpdateBot(Nothing, oBotParam, True)) Then
            Dim oBotResult = API.BotsServiceMethods.GetBot(oBotParam.Id, Nothing)
            Dim resultJson = JsonConvert.SerializeObject(oBotResult)
            Return Content(resultJson, "application/json")
        End If

        Return Json(roWsUserManagement.SessionObject().States.BotState.ErrorText)
    End Function

    <HttpDelete>
    Function DeleteBot(idBot As Integer) As JsonResult
        Dim oBot As roBot = API.BotsServiceMethods.GetBot(roTypes.Any2Integer(idBot), Nothing)
        Return Json(API.BotsServiceMethods.DeleteBot(oBot, Nothing, True))
    End Function

    <HttpGet>
    Public Function GetBotView(ByVal idView As Integer, ByVal idTypeBot As Integer) As ActionResult
        Me.InitializeBase(Nothing, Nothing, "Bots", requiredLabels, "LiveBots") _
                          .SetViewInfo("LiveBots", "Bots", "Title", "Title", "Base/Images/StartMenuIcos/Bots.png", "TitleDesc")

        ViewBag.AvailableBotTypes = GetBotTypeDescription()
        ViewBag.AllRules = GetAllRules()
        If (idView > -1) Then
            Dim oBot = API.BotsServiceMethods.GetBot(idView, Nothing)
            ViewBag.AvailableRules = GetAvailableRules(oBot.Type)
            ViewBag.Bot = oBot
        Else
            ViewBag.Bot = API.BotsServiceMethods.GetBot(-1, Nothing)
            ViewBag.AvailableRules = GetAvailableRules(DirectCast(idTypeBot, BotTypeEnum))
        End If
        Return View("_BotsMainView")
    End Function

    <HttpGet>
    Public Function GetRuleView(ByVal idRule As Integer) As ActionResult
        Me.InitializeBase(Nothing, Nothing, "Bots", requiredLabels, "LiveBots") _
                          .SetViewInfo("LiveBots", "Bots", "Title", "Title", "Base/Images/StartMenuIcos/Bots.png", "TitleDesc")

        Dim ruleBot As BotTypeEnum = DirectCast(idRule, BotRuleTypeEnum)
        Select Case ruleBot
            Case BotRuleTypeEnum.CopyEmployeePermissions

                Dim oEmpRows As DataRow() = API.EmployeeServiceMethods.GetAllEmployees(Nothing, "", "Employees", "U").Select()
                oEmpRows = oEmpRows.GroupBy(Function(obj) obj("IDEmployee")).Select(Function(grupo) grupo.First()).ToArray()
                ViewBag.Employees = oEmpRows.Select(Function(dRow) New roAutocompleteItemModel() With {
                                                        .ID = roTypes.Any2Integer(dRow("IDEmployee")),
                                                        .Name = roTypes.Any2String(dRow("EmployeeName"))})
                Return View("_BotsRuleCreateEmployee")
            Case BotRuleTypeEnum.CopySupervisorPermissions
                ViewBag.Supervisors = API.SecurityV3ServiceMethods.GetAllAvailableSupervisorsList(Nothing).Select(Function(user) New roAutocompleteItemModel() With {
                                                        .ID = user.ID,
                                                        .Name = user.Name})
                Return View("_BotsRuleCreateSupervisor")
            Case BotRuleTypeEnum.CopyEmployeeUserFields

                Dim oEmpRows As DataRow() = API.EmployeeServiceMethods.GetAllEmployees(Nothing, "", "Employees", "U").Select()
                oEmpRows = oEmpRows.GroupBy(Function(obj) obj("IDEmployee")).Select(Function(grupo) grupo.First()).ToArray()
                ViewBag.Employees = oEmpRows.Select(Function(dRow) New roAutocompleteItemModel() With {
                                                        .ID = roTypes.Any2Integer(dRow("IDEmployee")),
                                                        .Name = roTypes.Any2String(dRow("EmployeeName"))})
                Return View("_BotsRuleCopyEmployeeUserFields")
            Case BotRuleTypeEnum.CopyCenterCostRole
                If API.SecurityServiceMethods.HasPermissionOverFeature(Nothing, "Administration.Security", "U", Permission.Read) Then

                    Dim oSecurityFuncions As roGroupFeature() = API.SecurityChartServiceMethods.GetGroupFeatures(Nothing)

                    ViewBag.CostCenterRoles = oSecurityFuncions.Select(Function(feature) New roAutocompleteItemModel With {
                                                     .ID = roTypes.Any2Integer(feature.ID),
                                                     .Name = roTypes.Any2String(feature.Name)
                                                   }).ToArray()
                End If
                Return View("_BotsRuleCopyCenterCostRole")
            Case Else
                Return View("_BotsTypeError")
        End Select
    End Function

    Private Function GetAvailableRules(eBotType As BotTypeEnum) As Object
        Dim oLanguageFile = GetServerLanguage("LiveBots")
        Dim oRules = API.BotsServiceMethods.GetAvailableRulesByType(eBotType, Nothing)
        Return oRules.Select(Function(botEnum) New IdNamePair With {
            .Id = CInt(botEnum),
            .Name = oLanguageFile.Translate("RuleTypeEnum." & botEnum.ToString(), "Bots")
        })
    End Function

    Private Function GetAllRules()
        Dim oLanguageFile = GetServerLanguage("LiveBots")
        Dim botRules As New List(Of Object)

        For Each botType As BotRuleTypeEnum In [Enum].GetValues(GetType(BotRuleTypeEnum))
            Dim rule = New IdNamePair With {
            .Id = CInt(botType),
            .Name = oLanguageFile.Translate("RuleTypeEnum." & botType.ToString(), "Bots")
        }
            botRules.Add(rule)
        Next

        Return botRules
    End Function

    <HttpGet>
    Public Function GetBotTypeDescription(ByVal idTypeBot As Integer) As ActionResult
        Me.InitializeBase(Nothing, Nothing, "Bots", requiredLabels, "LiveBots") _
                          .SetViewInfo("LiveBots", "Bots", "Title", "Title", "Base/Images/StartMenuIcos/Bots.png", "", "TitleDesc")

        Dim botType As BotTypeEnum = DirectCast(idTypeBot, BotTypeEnum)

        ViewBag.AvailableRules = GetAvailableRules(botType)
        ViewData("SelectedBotTypeId") = botType
        Select Case botType
            Case BotTypeEnum.NewEmployee
                Return View("_BotsTypeAddEmployeeView")
            Case BotTypeEnum.NewSupervisor
                Return View("_BotsTypeAddSupervisorView")
            Case BotTypeEnum.NewCostCenter
                Return View("_BotsTypeAddCostCenterView")
            Case Else
                Return View("_BotsTypeGeneral")

        End Select
        Return View("_BotTypeError")
    End Function

    <HttpPost>
    Public Function GetAvailableRulesForView(iBotType As Integer) As ActionResult
        Me.InitializeBase(Nothing, Nothing, "Bots", requiredLabels, "LiveBots") _
                          .SetViewInfo("LiveBots", "Bots", "Title", "Title", "Base/Images/StartMenuIcos/Bots.png", "", "TitleDesc")

        ViewBag.AllRules = GetAllRules()
        Dim rules = GetAvailableRules(DirectCast(iBotType, BotTypeEnum))
        Dim resultJson = JsonConvert.SerializeObject(rules)
        Return Content(resultJson, "application/json")
    End Function

End Class