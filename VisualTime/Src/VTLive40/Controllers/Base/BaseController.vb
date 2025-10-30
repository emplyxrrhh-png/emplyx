Imports System.Web.Mvc
Imports DevExpress.Web.Internal.Dialogs
Imports Robotics.Base.DTOs
Imports Robotics.Base.DTOs.Enums
Imports Robotics.VTBase
Imports Robotics.Web.Base
Imports VTLive40.CardsTree.Model

Namespace Controllers.Base

    Public Enum ViewStates
        Create = 1
        Read = 2
        Update = 3
        Delete = 4
    End Enum

    Public Enum ViewTypes
        Genius = 1
        DocumentaryManagement = 2
        SSO = 3
        EmployeeSelector = 4
        Supervisors = 6
        Bots = 8
        Collectives = 9
        RulesGroup = 10
        Rule = 11
    End Enum

    Public Enum BarButtonTypes
        None = 0
        Genius = 1
        DocumentaryManagement = 2
        SSO = 3
        EmployeeSelector = 4
        Zones = 5
        Supervisors = 6
        Conversations = 7
        Bots = 8
        Collectives = 9
        RulesGroup = 10
        Rule = 11
    End Enum

    Public Enum CardTreeTypes
        None = 0
        Genius = 1
        DocumentaryManagement = 2
        SSO = 3
        EmployeeSelector = 4
        Zones = 5
        Supervisors = 6
        Conversations = 7
        Bots = 8
        Collectives = 9
        RulesGroup = 10
        Rule = 11
    End Enum

    Public Enum TabBarButtonTypes
        None = 0
        Genius = 1
        DocumentaryManagement = 2
        SSO = 3
        EmployeeSelector = 4
        Zones = 5
        Supervisors = 6
        Conversations = 7
        Bots = 8
        Collectives = 9
        RulesGroup = 10
        Rule = 11
    End Enum

    ''' <summary>
    ''' Base Controller for VTNext MVC Application
    ''' </summary>
    Public Class BaseController
        Inherits Controller


        Private systemLabels = {"SaveChanges", "UndoChanges", "DataModified"}

        Private oLanguage As roLanguageWeb
        Private _languageFile As String = "LivePortal"

#Region "Initialize"

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="cardTreeType"></param>
        ''' <param name="tabBarButtonType"></param>
        ''' <param name="languageScope"></param>
        ''' <param name="languageLabels"></param>
        Protected Function InitializeBase(cardTreeType As CardTreeTypes,
                                          tabBarButtonType As TabBarButtonTypes,
                                          languageScope As String,
                                          languageLabels As String(),
                                          Optional languageFile As String = "LivePortal") As BaseController

            Me._languageFile = languageFile

            'Clear the viewData object.
            ViewData.Clear()

            'Load the language labels to translate.
            LoadTranslateLabels(languageScope, languageLabels, languageFile)

            'Load the CardView data.
            LoadCardView(cardTreeType)

            'Load the TabBarButton data.
            LoadTabBarButtons(tabBarButtonType)

            LoadScriptsVersion()

            Return Me

        End Function

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="languageFile"></param>
        ''' <param name="languageScope"></param>
        ''' <param name="titleViewKey"></param>
        ''' <param name="captionViewKey"></param>
        ''' <param name="iconViewType"></param>
        ''' <returns></returns>
        Friend Function SetViewInfo(languageFile As String,
                                    languageScope As String,
                                    titleViewKey As String,
                                    captionViewKey As String,
                                    iconViewType As String,
                                    Optional ByVal descriptionkey As String = "",
                                    Optional ByVal filterTextKey As String = "") As BaseController

            ViewData(Helpers.Constants.DefaultViewTitle) = GetServerLanguage(languageFile).Translate(titleViewKey, languageScope)
            ViewData(Helpers.Constants.SearchText) = GetServerLanguage(languageFile).Translate(titleViewKey & ".search", languageScope)
            If (filterTextKey <> "") Then
                ViewData(Helpers.Constants.FilterText) = GetServerLanguage(languageFile).Translate(filterTextKey & ".filter", languageScope)
            Else
                ViewData(Helpers.Constants.FilterText) = ""
            End If
            If descriptionkey <> String.Empty Then
                ViewData(Helpers.Constants.DefaultViewDescription) = GetServerLanguage(languageFile).Translate(descriptionkey, languageScope)
            Else
                ViewData(Helpers.Constants.DefaultViewDescription) = ""
            End If
            ViewData(Helpers.Constants.DefaultViewCaption) = GetServerLanguage(languageFile).Translate(captionViewKey, languageScope)
            ViewData(Helpers.Constants.DefaultViewIcon) = If(Url IsNot Nothing, Url.Content("~"), "") & iconViewType

            Return Me

        End Function

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="cardTreeType"></param>
        ''' <returns></returns>
        Friend Function SetCardView(cardTreeType As CardTreeTypes) As BaseController

            LoadCardView(cardTreeType)
            Return Me

        End Function

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="TabBarButtonType"></param>
        ''' <returns></returns>
        Friend Function SetTabBarButton(TabBarButtonType As TabBarButtonTypes) As BaseController

            LoadTabBarButtons(TabBarButtonType)
            Return Me

        End Function

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="barButtonType"></param>
        ''' <returns></returns>
        Friend Function SetBarButton(barButtonType As BarButtonTypes) As BaseController

            LoadBarButtons(barButtonType)
            Return Me

        End Function

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="languageScope"></param>
        ''' <param name="languageLabels"></param>
        ''' <param name="languageFile"></param>
        ''' <returns></returns>
        Friend Function SetTranslateLabels(languageScope As String,
                                          languageLabels As String(),
                                          Optional languageFile As String = "") As BaseController

            If languageFile = String.Empty Then languageFile = _languageFile

            LoadTranslateLabels(languageScope, languageLabels, languageFile)
            Return Me

        End Function

        Protected Sub LoadScriptsVersion()
            ViewData(Helpers.Constants.ScriptVersion) = "?v=" & Robotics.VTBase.roTypes.Any2String(Reflection.Assembly.GetExecutingAssembly().GetCustomAttributes(GetType(Reflection.AssemblyDescriptionAttribute), False).FirstOrDefault().Description)
        End Sub

#End Region

#Region "Languages"

        ''' <summary>
        ''' Return Dictionary with all translate labels passed by parameters.
        ''' </summary>
        ''' <param name="scope">Language scope.</param>
        ''' <param name="labels">List of labels to translate</param>
        ''' <returns>Dictionary with translated labels</returns>
        Public Function GetLabelsTranslate(scope As String, labels() As String, Optional file As String = "") As Dictionary(Of String, String)

            If file = String.Empty Then file = _languageFile

            Dim languagesEntries = New Dictionary(Of String, String)
            Dim languageServerHandler = GetServerLanguage(file)
            labels.ToList().ForEach(Sub(label)
                                        If Not languagesEntries.Any(Function(e) e.Key.Equals("")) Then
                                            languagesEntries.Add(scope & "#" & label, languageServerHandler.Translate(label, scope))
                                        End If
                                    End Sub)

            Dim generalLanguageServerHandler = GetServerLanguage("LivePortal")
            For Each label As String In systemLabels
                If Not languagesEntries.Any(Function(e) e.Key.Equals("")) Then
                    languagesEntries.Add("Common#" & label, generalLanguageServerHandler.Translate(label, "Common"))
                End If
            Next
            Return languagesEntries

        End Function

        Public Sub AddAdditionalLanguage(scope As String, labels() As String, Optional file As String = "")

            If file = String.Empty Then file = _languageFile

            Dim languagesEntries As Dictionary(Of String, String) = ViewData(Helpers.Constants.DefaultLanguagesEntries)
            Dim languageServerHandler = GetServerLanguage(file)
            labels.ToList().ForEach(Sub(label)
                                        If Not languagesEntries.Any(Function(e) e.Key.Equals("")) Then
                                            languagesEntries.Add(scope & "#" & label, languageServerHandler.Translate(label, scope))
                                        End If
                                    End Sub)

            ViewData(Helpers.Constants.DefaultLanguagesEntries) = languagesEntries

        End Sub

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="file"></param>
        ''' <returns></returns>
        Public Function GetServerLanguage(Optional file As String = "") As roLanguageWeb
            If Me.oLanguage Is Nothing Then
                Me.oLanguage = New roLanguageWeb
            End If

            If file = String.Empty Then file = Me._languageFile

            SetLanguageFile(file)

            Return Me.oLanguage

        End Function

        Private Sub SetLanguageFile(file As String)
            WLHelperWeb.SetLanguage(Me.oLanguage, file)
        End Sub

        ''' <summary>
        ''' Translate the label with (not found) result if the translation dont exist
        ''' </summary>
        ''' <param name="label"></param>
        ''' <param name="languageFile"></param>
        ''' <returns></returns>
        Private Function TranslateLabel(label As String, Optional languageFile As String = "")
            If languageFile = String.Empty Then languageFile = _languageFile

            Dim labelChunks = label.Split("#")
            Dim labelText = label
            If (labelChunks IsNot Nothing AndAlso labelChunks.Length > 1) Then
                labelText = GetServerLanguage(languageFile).Translate(labelChunks.ElementAt(1), labelChunks.ElementAt(0))
            Else
                labelText &= "(not found)"
            End If
            Return labelText
        End Function

        ''' <summary>
        ''' Return Datagrid translated terms
        ''' </summary>
        ''' <param name="columnConfig"></param>
        ''' <param name="languageFile"></param>
        ''' <returns></returns>
        Private Function TranslateColumnConfig(columnConfig As List(Of roDataGridColumnConfig), Optional languageFile As String = "")
            If languageFile = String.Empty Then languageFile = _languageFile

            For Each c As roDataGridColumnConfig In columnConfig
                c.caption = If(Not String.IsNullOrEmpty(c.caption), TranslateLabel(c.caption, languageFile), Nothing)
                If c.validationRules IsNot Nothing AndAlso c.validationRules.Count > 0 Then
                    For Each v As roDataGridValidationRule In c.validationRules
                        v.message = If(Not String.IsNullOrEmpty(v.message), TranslateLabel(v.message, languageFile), Nothing)
                    Next
                End If
            Next
            Return columnConfig
        End Function

#End Region

#Region "CardTree"

        Public Function CardView(cardTreeType As String) As ActionResult

            Dim cardTreeEnum As CardTreeTypes = CType([Enum].Parse(GetType(CardTreeTypes), cardTreeType, True), CardTreeTypes)
            LoadCardView(cardTreeEnum)
            Return PartialView("CardTree/_CardView")

        End Function

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="cardTreeTypes"></param>
        ''' <returns></returns>
        Protected Function GetCardViewData(cardTreeTypes As CardTreeTypes) As List(Of Card)

            Dim cardsList As New List(Of Card)
            Dim oLanguageFile = Nothing
            Select Case cardTreeTypes
                Case CardTreeTypes.Genius
                    Dim oGeniusLst = API.GeniusServiceMethods.GetUserGeniusViews(Nothing).Union(API.GeniusServiceMethods.GetGeniusViewsTemplates(Nothing))

                    For Each oGenius As roGeniusView In oGeniusLst.OrderByDescending(Function(x) x.IdPassport).ToList()
                        If oGenius.IdPassport <> 0 Or Not oGeniusLst.Any(Function(myObject) myObject.IdSystemView = oGenius.Id And myObject.IdPassport = WLHelperWeb.CurrentPassportID) Then
                            Dim lstObject = New Card() With {
                        .Id = oGenius.Id,
                        .Name = oGenius.Name,
                        .Description = oGenius.Description,
                        .CreatedOn = oGenius.CreatedOn,
                        .Type = Robotics.VTBase.roConstants.GetGeniusCodeFromEnum(oGenius.TypeView),
                        .Icon = Url.Content("~/Base/Images/Genius/GeniusAnalytics.png"),
                        .Filterfield = Robotics.VTBase.roConstants.GetGeniusCodeFromEnum(oGenius.DS),
                        .AllowHtml = False
                    }

                            Select Case oGenius.DS
                                Case GeniusTypeEnum._ACCESS
                                    lstObject.Icon = Url.Content("~/Base/Images/StartMenuIcos/AccessStatus.png")
                                Case GeniusTypeEnum._COSTCENTERS
                                    lstObject.Icon = Url.Content("~/Base/Images/StartMenuIcos/BusinessCenters.png")
                                Case GeniusTypeEnum._PRODUCTIV
                                    lstObject.Icon = Url.Content("~/Base/Images/StartMenuIcos/Task.png")
                                Case GeniusTypeEnum._SCHEDULER
                                    lstObject.Icon = Url.Content("~/Base/Images/StartMenuIcos/calendar.png")
                                Case GeniusTypeEnum._ACCRUALS
                                    lstObject.Icon = Url.Content("~/Base/Images/StartMenuIcos/concepts.png")
                                Case GeniusTypeEnum._PUNCHES
                                    lstObject.Icon = Url.Content("~/Base/Images/StartMenuIcos/terminals.png")
                                Case GeniusTypeEnum._REQUESTS
                                    lstObject.Icon = Url.Content("~/Base/Images/StartMenuIcos/requests.png")
                                Case GeniusTypeEnum._EQUALITYPLAN
                                    lstObject.Icon = Url.Content("~/Base/Images/StartMenuIcos/reportscheduler.png")
                                Case GeniusTypeEnum._USERS
                                    lstObject.Icon = Url.Content("~/Base/Images/StartMenuIcos/employees.png")

                            End Select

                            cardsList.Add(lstObject)
                        End If
                    Next

                    oLanguageFile = GetServerLanguage("LiveCommunique")

                    ViewData(Helpers.Constants.DefaultViewCategories) = True' New Dictionary(Of String, String) From {{"", oLanguageFile.Translate("roAllCategories", "Communique")}}

                Case CardTreeTypes.DocumentaryManagement

                    Dim list As List(Of Integer) = WLHelperWeb.CardListSearchFiltering()

                    Dim oTemplateDocumentsLst = API.DocumentsServiceMethods.GetTemplateDocumentsList(False, Nothing, Nothing, False)
                    oLanguageFile = GetServerLanguage("LiveDocuments")
                    cardsList.AddRange(oTemplateDocumentsLst.Where(Function(oDocTemp) (oDocTemp.Name.ToUpper() <> "COMUNICADO") AndAlso (oDocTemp.Name.ToUpper <> "BIOCERTIFICATE") AndAlso list Is Nothing OrElse (list IsNot Nothing AndAlso list.Contains(oDocTemp.Id))).Select(Function(oDocumentTemplate) New Card() With {
                                                            .Id = oDocumentTemplate.Id,
                                                            .Name = oDocumentTemplate.Name,
                                                            .Icon = Url.Content("~/Base/Images/StartMenuIcos/Documents.png"),
                                                            .IsSystem = oDocumentTemplate.IsSystem,
                                                            .Description = oDocumentTemplate.Description,
                                                            .Filterfield = CInt(oDocumentTemplate.Area),
                                                            .AllowHtml = False})) 'oLanguageFile.Translate("Area." & oDocumentTemplate.Area.ToString(), "DocumentTemplate")}))

                    oLanguageFile = GetServerLanguage("LiveCommunique")
                    ViewData(Helpers.Constants.DefaultViewCategories) = True 'New Dictionary(Of String, String) From {{"", oLanguageFile.Translate("roAllCategories", "Communique")}}

                Case CardTreeTypes.Supervisors

                    'Dim rand = New Random()

                    Dim iconUrl = "~/Security/Images/"
                    Dim strIconGroup = "PassportGroup_80.png"
                    Dim strIconPassport = "Passport_80.png"
                    Dim strIconPassportEmployee = "PassportEmployee_80.png"
                    Dim strIcon = ""
                    Dim listado = API.SecurityV3ServiceMethods.GetAllAvailableSupervisorsList(Nothing)

                    For Each oSupervisor As roPassport In listado.OrderBy(Function(x) x.Name).ToList()
                        If oSupervisor.ID <> 0 Then
                            If oSupervisor.GroupType.Equals("U") Then
                                strIcon = strIconGroup
                            ElseIf oSupervisor.GroupType.Equals("E") Then
                                strIcon = strIconPassportEmployee
                            Else
                                strIcon = strIconPassport
                            End If
                            Dim lstObject = New Card() With {
                            .Id = oSupervisor.ID,
                            .Name = oSupervisor.Name.ToLower,
                            .Icon = Url.Content(String.Concat(iconUrl, strIcon)),
                            .Type = oSupervisor.GroupType,
                            .Filterfield = String.Concat("r", oSupervisor.IDGroupFeature & "r"),
                            .AllowHtml = False ', .Filterfield2 = String.Concat("g", rand.Next(1, 4))
                            }
                            cardsList.Add(lstObject)
                        End If
                    Next

                    oLanguageFile = GetServerLanguage("LiveSecurity")
                    ViewData(Helpers.Constants.DefaultViewCategories) = True
                Case CardTreeTypes.Bots
                    Dim oBotsLst = API.BotsServiceMethods.GetAllBots(Nothing)

                    For i As Integer = 0 To oBotsLst.Count - 1
                        Dim bot As roBot = oBotsLst(i)

                        Dim nombre As String = bot.Name
                        Dim estado As BotStatusEnum = bot.Status
                        Dim estadoString As String
                        Dim tipo As BotTypeEnum = bot.Type
                        Dim fechaUltimaEjecucion As String = If(bot.LastExecutionDate = #1/1/1970 12:00:00 AM#, String.Empty, bot.LastExecutionDate.ToString("dd/MM/yyyy hh:mm:ss tt"))
                        Dim descripcion = ""

                        oLanguageFile = GetServerLanguage("LiveBots")

                        If estado = BotStatusEnum.Failed Then
                            estadoString = oLanguageFile.Translate("Bots.statusKO", "Bots")
                            descripcion = $"<span style='color: red; font-weight: bold;'>{estadoString}</span><br>{fechaUltimaEjecucion}"
                        Else
                            estadoString = oLanguageFile.Translate("Bots.statusOK", "Bots")
                            descripcion = $"{estadoString}<br>{fechaUltimaEjecucion}"
                        End If

                        Dim icon = ""
                        Select Case tipo
                            Case BotTypeEnum.NewEmployee
                                icon = Url.Content("~/Base/Images/StartMenuIcos/Employees.png")
                            Case BotTypeEnum.NewSupervisor
                                icon = Url.Content("~/Base/Images/StartMenuIcos/Supervisors.png")
                            Case BotTypeEnum.NewCostCenter
                                icon = Url.Content("~/Base/Images/StartMenuIcos/BusinessCenters.png")
                            Case Else
                                icon = Url.Content("~/Base/Images/StartMenuIcos/ReportScheduler.png")
                        End Select

                        Dim lstObject = New Card() With {
                            .Id = bot.Id,
                            .Name = nombre,
                            .Icon = icon,
                            .Description = descripcion,
                            .AllowHtml = True
                            }

                        cardsList.Add(lstObject)
                    Next

                Case CardTreeTypes.Collectives
                    Dim iconUrl = "~/Security/Images/"
                    Dim strIconGroup = "PassportGroup_80.png"
                    Dim strIcon = ""
                    Dim listado = API.CollectiveServiceMethods.GetAllCollectives(Nothing, False, False)

                    For Each oCollective As roCollective In listado.OrderBy(Function(x) x.Name).ToList()
                        If oCollective.Id <> 0 Then
                            strIcon = strIconGroup
                            Dim lstObject = New Card() With {
                            .Id = oCollective.Id,
                            .Name = oCollective.Name,
                            .Icon = Url.Content(String.Concat(iconUrl, strIcon)),
                            .AllowHtml = False ', .Filterfield2 = String.Concat("g", rand.Next(1, 4))
                            }
                            cardsList.Add(lstObject)
                        End If
                    Next

                    oLanguageFile = GetServerLanguage("LiveCollectives")
                    ViewData(Helpers.Constants.DefaultViewCategories) = False
                Case CardTreeTypes.RulesGroup, CardTreeTypes.Rule
                    cardsList = New List(Of Card)
            End Select

            Return cardsList
        End Function

#End Region

#Region "TabBarButtons"

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="tabBarButtonType"></param>
        ''' <returns></returns>
        Public Function GetTabBarButtons(tabBarButtonType As String) As List(Of roGuiAction)

            Dim tabBarButtonEnum As TabBarButtonTypes = CType([Enum].Parse(GetType(TabBarButtonTypes), tabBarButtonType, True), TabBarButtonTypes)
            Return GetTabBarButtonsData(tabBarButtonEnum)

        End Function

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="tabBarButtonType"></param>
        ''' <returns></returns>
        Protected Function GetTabBarButtonsData(tabBarButtonType As TabBarButtonTypes) As List(Of roGuiAction)

            Dim languageScope As String = String.Empty

            Dim guiActions As New List(Of roGuiAction)

            Select Case tabBarButtonType
                Case TabBarButtonTypes.Genius
                    guiActions = API.PortalServiceMethods.GetGuiActions(Nothing, "Portal\General\Genius", WLHelperWeb.CurrentPassportID)
                    languageScope = "Genius"
                Case TabBarButtonTypes.DocumentaryManagement
                    guiActions = API.PortalServiceMethods.GetGuiActions(Nothing, "Portal\General\DocumentaryManagement", WLHelperWeb.CurrentPassportID)
                    languageScope = "DocumentaryManagement"
                Case TabBarButtonTypes.Bots
                    guiActions = API.PortalServiceMethods.GetGuiActions(Nothing, "Portal\General\Bots", WLHelperWeb.CurrentPassportID)
                    languageScope = "Bots"
                Case TabBarButtonTypes.Supervisors
                    guiActions = API.PortalServiceMethods.GetGuiActions(Nothing, "Portal\Security\Passports\Supervisors", WLHelperWeb.CurrentPassportID)
                    For Each ga As roGuiAction In guiActions
                        ga.AfterFunction = "" 'Eliminamos las acciones del onclick porque lo tratamos en roSupervisorsScript.js
                    Next
                    languageScope = "Supervisors"
                Case TabBarButtonTypes.RulesGroup, CardTreeTypes.Rule
                    guiActions = New List(Of roGuiAction)
                    languageScope = tabBarButtonType.ToString()
            End Select

            TranslateGuiActionText(languageScope, guiActions)

            Return guiActions
        End Function

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="scope"></param>
        ''' <param name="guiActions"></param>
        Private Sub TranslateGuiActionText(scope As String, ByRef guiActions As List(Of roGuiAction))
            Dim serverLanguage = GetServerLanguage()
            guiActions.ForEach(Sub(actions As roGuiAction)
                                   actions.Text = IIf(Not IsDBNull(actions.LanguageTag) And Not actions.LanguageTag.Equals(String.Empty), serverLanguage.Translate(actions.LanguageTag, scope), actions.Text)
                               End Sub)
        End Sub

#End Region

#Region "BarButton"

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="barButtonType"></param>
        ''' <returns></returns>
        Function GetBarButtonData(barButtonType As BarButtonTypes) As List(Of String)

            Dim tabsList As New List(Of String)

            Select Case barButtonType
                Case BarButtonTypes.Genius
                    tabsList.AddRange(
                    {
                       GetServerLanguage("LiveGenius").Translate("generalGenius", "genius"),
                       GetServerLanguage("LiveGenius").Translate("geniusPlanification", "genius")
                    })
                Case BarButtonTypes.Supervisors
                    tabsList.AddRange(
                    {
                       GetServerLanguage("LiveSecurity").Translate("tabgeneral", "Supervisors"),
                       GetServerLanguage("LiveSecurity").Translate("tabrestrictions", "Supervisors"),
                       GetServerLanguage("LiveSecurity").Translate("tabidentifymethods", "Supervisors"),
                       GetServerLanguage("LiveSecurity").Translate("tabnotifications", "Supervisors"),
                       GetServerLanguage("LiveSecurity").Translate("taballowedapplications", "Supervisors")
                    })

                Case BarButtonTypes.DocumentaryManagement

            End Select

            Return tabsList

        End Function

#End Region

#Region "ViewOptions"

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="type"></param>
        ''' <returns></returns>
        Public Function GetOptionTypePrefix(type As eViewOptionType) As String

            Dim prefix = String.Empty

            Select Case (type)
                Case eViewOptionType.Button
                    prefix = "bt"
                Case eViewOptionType.CheckBox
                    prefix = "ck"
                Case eViewOptionType.ColorBox
                    prefix = "cl"
                Case eViewOptionType.DataGrid
                    prefix = "dg"
                Case eViewOptionType.InputNumber,
                     eViewOptionType.InputDate,
                     eViewOptionType.InputText,
                     eViewOptionType.InputHoursAndMinutes
                    prefix = "tx"
                Case eViewOptionType.RadioButton
                    prefix = "rg"
                Case eViewOptionType.SelectOption
                    prefix = "cb"
                Case eViewOptionType.TextArea
                    prefix = "ta"
                Case eViewOptionType.TagBox
                    prefix = "tb"
                Case eViewOptionType.DateBox
                    prefix = "dt"
                Case eViewOptionType.FileManager
                    prefix = "fm"
            End Select

            Return prefix

        End Function

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="id"></param>
        ''' <param name="uniqueKey"></param>
        ''' <param name="type"></param>
        ''' <param name="labelKey"></param>
        ''' <param name="languageFile"></param>
        ''' <returns></returns>
        Private Function AddViewOption(id As Integer,
                                       uniqueKey As String,
                                       type As eViewOptionType,
                                       labelKey As String, Optional languageFile As String = "") As roViewOptions

            If languageFile = String.Empty Then languageFile = _languageFile
            Dim controlPrefix As String = GetOptionTypePrefix(type)

            Return AddViewOption(id,
                                 uniqueKey,
                                 String.Format("{0}{1}", controlPrefix, uniqueKey),
                                 type,
                                 String.Format("#{0}{1}", controlPrefix, uniqueKey),
                                 labelKey,
                                 languageFile)

        End Function

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="id"></param>
        ''' <param name="uniqueKey"></param>
        ''' <param name="name"></param>
        ''' <param name="type"></param>
        ''' <param name="htmlSelector"></param>
        ''' <param name="labelKey"></param>
        ''' <returns></returns>
        Private Function AddViewOption(id As Integer,
                                       uniqueKey As String,
                                       name As String, type As eViewOptionType,
                                       htmlSelector As String,
                                       labelKey As String, Optional languageFile As String = "") As roViewOptions

            If languageFile = String.Empty Then languageFile = _languageFile
            Dim labelText = TranslateLabel(labelKey, languageFile)

            Return New roViewOptions() With
            {
                .ID = id,
                .UniqueKey = uniqueKey,
                .Name = name,
                .Type = type,
                .ControlType = type.ToString(),
                .HtmlSelector = htmlSelector,
                .LabelKey = labelKey,
                .LabelText = labelText
            }

        End Function

        Private Function AddViewOption(id As Integer,
                                        uniqueKey As String,
                                        columnConfig As List(Of roDataGridColumnConfig),
                                        Optional languageFile As String = "") As roDataGridViewOptions

            If languageFile = String.Empty Then languageFile = _languageFile
            Dim dataGridViewOption = New roDataGridViewOptions(AddViewOption(id, uniqueKey, eViewOptionType.DataGrid, String.Empty, languageFile))
            dataGridViewOption.DataGridColumnConfig = TranslateColumnConfig(columnConfig, languageFile)

            Return dataGridViewOption
        End Function

        Private Function AddViewOption(id As Integer,
                                        uniqueKey As String,
                                        name As String,
                                        columnConfig As List(Of roDataGridColumnConfig),
                                        Optional languageFile As String = "") As roDataGridViewOptions

            If languageFile = String.Empty Then languageFile = _languageFile
            Dim dataGridViewOption = New roDataGridViewOptions(AddViewOption(id, uniqueKey, name, eViewOptionType.DataGrid, Nothing, languageFile))
            dataGridViewOption.DataGridColumnConfig = TranslateColumnConfig(columnConfig, languageFile)

            Return dataGridViewOption
        End Function

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="viewType"></param>
        ''' <param name="viewState"></param>
        ''' <returns></returns>
        <HttpPost()>
        Public Function LoadViewOptions(viewType As String, viewState As String, languageFile As String) As JsonResult
            _languageFile = languageFile

            Dim eViewType As ViewTypes = CType([Enum].Parse(GetType(ViewTypes), viewType, True), ViewTypes)
            Dim eViewState As ViewStates = CType([Enum].Parse(GetType(ViewStates), viewState, True), ViewStates)
            Dim response = Json(LoadViewOptions(eViewType, eViewState))
            Return response

        End Function

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="viewType"></param>
        ''' <param name="viewState"></param>
        ''' <returns></returns>
        Private Function LoadViewOptions(viewType As ViewTypes, viewState As ViewStates) As List(Of roViewOptions)

            Dim viewOptionsArr As New List(Of roViewOptions)
            Dim languageFile As String = ""
            Dim viewOption As roViewOptions
            Dim options As New List(Of roItem)
            Dim listFields As New List(Of roResultField)
            Dim IdViewOption As Integer = 0

            Select Case (viewType)
                Case ViewTypes.Genius
                    Select Case viewState
                        Case ViewStates.Create
                        Case ViewStates.Read
                            IdViewOption = 1
                            viewOption = AddViewOption(IdViewOption, "DateFilterType", eViewOptionType.RadioButton, "Genius#DatePeriod", _languageFile)

                            options = New List(Of roItem)

                            Dim oPortalLang = GetServerLanguage("LivePortal")
                            For Each eType As TypePeriodEnum In System.Enum.GetValues(GetType(TypePeriodEnum))
                                options.Add(New roItem With {.Id = CInt(eType), .Value = CInt(eType), .Text = oPortalLang.Translate("scope." & eType.ToString, "roOptSchedulePeriod")})
                            Next

                            viewOption.Values = options.OrderBy(Function(x) x.Text).ToList().ToArray
                            viewOptionsArr.Add(viewOption)

                            IdViewOption = IdViewOption + 1
                            viewOption = AddViewOption(IdViewOption, "DateInf", eViewOptionType.DateBox, "Genius#DateInf", _languageFile)
                            viewOption.ElementAttr = New ElementAttributes() With {.Type = "DateBoxTime"}
                            viewOption.Caption = "00: 00"
                            viewOptionsArr.Add(viewOption)

                            IdViewOption = IdViewOption + 1
                            viewOption = AddViewOption(IdViewOption, "DateInfNew", eViewOptionType.DateBox, "Genius#DateInf", _languageFile)
                            viewOption.ElementAttr = New ElementAttributes() With {.Type = "DateBoxTime"}
                            viewOption.Caption = "00: 00"
                            viewOptionsArr.Add(viewOption)

                            IdViewOption = IdViewOption + 1
                            viewOption = AddViewOption(IdViewOption, "DateSup", eViewOptionType.DateBox, "Genius#DateSup", _languageFile)
                            viewOption.ElementAttr = New ElementAttributes() With {.Type = "DateBoxTime"}
                            viewOption.Caption = "00: 00"
                            viewOptionsArr.Add(viewOption)

                            IdViewOption = IdViewOption + 1
                            viewOption = AddViewOption(IdViewOption, "DateSupNew", eViewOptionType.DateBox, "Genius#DateSup", _languageFile)
                            viewOption.ElementAttr = New ElementAttributes() With {.Type = "DateBoxTime"}
                            viewOption.Caption = "00: 00"
                            viewOptionsArr.Add(viewOption)
                        Case ViewStates.Update
                        Case ViewStates.Delete
                    End Select
                Case ViewTypes.DocumentaryManagement
                    Select Case viewState
                        Case ViewStates.Create
                        Case ViewStates.Read
                            languageFile = "LiveDocumentaryManagement"
                            viewOptionsArr.Add(AddViewOption(0, "Documents", eViewOptionType.FileManager, String.Empty, languageFile))
                        Case ViewStates.Update
                        Case ViewStates.Delete
                    End Select

            End Select

            Return viewOptionsArr

        End Function

        <HttpPost>
        Public Function GetCardViewSearchFilter(viewType As ViewTypes) As String

            Select Case (viewType)

                Case ViewTypes.DocumentaryManagement
                    Dim oLst As New Generic.List(Of CardsTreeFilter)

                    Dim oLanguageFile = GetServerLanguage("LiveCommunique")
                    oLst.Add(New CardsTreeFilter() With {.ID = "", .Description = oLanguageFile.Translate("roAllCategories", "Communique"), .Parent = "1"})

                    oLanguageFile = GetServerLanguage("LiveDocuments")
                    oLst.AddRange([Enum].GetValues(GetType(DocumentArea)).Cast(Of DocumentArea).Select(Function(card) New CardsTreeFilter() With {.ID = CInt(card), .Description = oLanguageFile.Translate("Area." & card.ToString(), "DocumentTemplate")}))

                    Return roJSONHelper.SerializeNewtonSoft(oLst)

                Case ViewTypes.Genius

                    Dim oLst As New Generic.List(Of CardsTreeFilter)

                    Dim oLanguageFile = GetServerLanguage("LiveCommunique")
                    oLst.Add(New CardsTreeFilter() With {.ID = "", .Description = oLanguageFile.Translate("roAllCategories", "Communique"), .Parent = "1"})

                    oLanguageFile = GetServerLanguage("LiveReportScheduler")
                    oLst.Add(New CardsTreeFilter() With {.Description = oLanguageFile.Translate("GeniusTypeEnum.Type." & GeniusTypeEnum._ACCESS.ToString(), "ReportScheduler"), .ID = roConstants.GetGeniusCodeFromEnum(GeniusTypeEnum._ACCESS)})
                    oLst.Add(New CardsTreeFilter() With {.Description = oLanguageFile.Translate("GeniusTypeEnum.Type." & GeniusTypeEnum._SCHEDULER.ToString(), "ReportScheduler"), .ID = roConstants.GetGeniusCodeFromEnum(GeniusTypeEnum._SCHEDULER)})
                    oLst.Add(New CardsTreeFilter() With {.Description = oLanguageFile.Translate("GeniusTypeEnum.Type." & GeniusTypeEnum._COSTCENTERS.ToString(), "ReportScheduler"), .ID = roConstants.GetGeniusCodeFromEnum(GeniusTypeEnum._COSTCENTERS)})
                    oLst.Add(New CardsTreeFilter() With {.Description = oLanguageFile.Translate("GeniusTypeEnum.Type." & GeniusTypeEnum._PUNCHES.ToString(), "ReportScheduler"), .ID = roConstants.GetGeniusCodeFromEnum(GeniusTypeEnum._PUNCHES)})
                    oLst.Add(New CardsTreeFilter() With {.Description = oLanguageFile.Translate("GeniusTypeEnum.Type." & GeniusTypeEnum._EQUALITYPLAN.ToString(), "ReportScheduler"), .ID = roConstants.GetGeniusCodeFromEnum(GeniusTypeEnum._EQUALITYPLAN)}) ' cmbAnalyticType.Items.Add(tmpLang.Translate("AnalyticSchedule.Type." & AnalyticsTypeEnum._EQUALITYPLAN.ToString(), Me.ReportSchedulerScope), roConstants.GetAnalyticsCodeFromEnum(AnalyticsTypeEnum._EQUALITYPLAN))
                    oLst.Add(New CardsTreeFilter() With {.Description = oLanguageFile.Translate("GeniusTypeEnum.Type." & GeniusTypeEnum._PRODUCTIV.ToString(), "ReportScheduler"), .ID = roConstants.GetGeniusCodeFromEnum(GeniusTypeEnum._PRODUCTIV)})
                    oLst.Add(New CardsTreeFilter() With {.Description = oLanguageFile.Translate("GeniusTypeEnum.Type." & GeniusTypeEnum._ACCRUALS.ToString(), "ReportScheduler"), .ID = roConstants.GetGeniusCodeFromEnum(GeniusTypeEnum._ACCRUALS)})
                    oLst.Add(New CardsTreeFilter() With {.Description = oLanguageFile.Translate("GeniusTypeEnum.Type." & GeniusTypeEnum._REQUESTS.ToString(), "ReportScheduler"), .ID = roConstants.GetGeniusCodeFromEnum(GeniusTypeEnum._REQUESTS)})
                    oLst.Add(New CardsTreeFilter() With {.Description = oLanguageFile.Translate("GeniusTypeEnum.Type." & GeniusTypeEnum._USERS.ToString(), "ReportScheduler"), .ID = roConstants.GetGeniusCodeFromEnum(GeniusTypeEnum._USERS)})

                    Return roJSONHelper.SerializeNewtonSoft(oLst)

                Case ViewTypes.Supervisors

                    Dim oLst As New Generic.List(Of CardsTreeFilter)

                    Dim oLanguageFile = GetServerLanguage("LiveSecurity")
                    oLanguageFile = GetServerLanguage("LiveSecurity")

                    oLst.Add(New CardsTreeFilter() With {.Description = oLanguageFile.Translate("allRoles", "Supervisors"), .ID = "", .Parent = "1"})

                    Dim oSecurityFuncions As roGroupFeature() = API.SecurityChartServiceMethods.GetGroupFeatures(Nothing)
                    If oSecurityFuncions IsNot Nothing AndAlso oSecurityFuncions.Length > 0 Then
                        For Each oFeature As roGroupFeature In oSecurityFuncions
                            oLst.Add(New CardsTreeFilter() With {.Description = oFeature.Name, .ID = String.Concat("r", oFeature.ID & "r")})
                        Next
                    End If

                    Return roJSONHelper.SerializeNewtonSoft(oLst)
            End Select

            Return String.Empty

        End Function

#End Region

#Region "Private Methods"

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="languageScope"></param>
        ''' <param name="languageLabels"></param>
        ''' <param name="languageFile"></param>
        Private Sub LoadTranslateLabels(languageScope As String,
                         languageLabels As String(),
                         Optional languageFile As String = "")

            If languageFile = String.Empty Then languageFile = _languageFile
            ViewData(Helpers.Constants.DefaultLanguagesEntries) = GetLabelsTranslate(languageScope, languageLabels, languageFile)

        End Sub

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="cardTreeType"></param>
        Private Sub LoadCardView(cardTreeType As CardTreeTypes)

            ViewData(Helpers.Constants.DefaultCardTreeType) = cardTreeType.ToString()
            ViewData(Helpers.Constants.DefaultCardTreeData) = GetCardViewData(cardTreeType)

        End Sub

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="TabBarButtonType"></param>
        Private Sub LoadTabBarButtons(tabBarButtonType As TabBarButtonTypes)

            ViewData(Helpers.Constants.DefaultTabBarButtonType) = tabBarButtonType.ToString()
            ViewData(Helpers.Constants.DefaultTabBarButtonData) = GetTabBarButtonsData(tabBarButtonType)

        End Sub

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="barButtonType"></param>
        Private Sub LoadBarButtons(barButtonType As BarButtonTypes)

            ViewData(Helpers.Constants.DefaultBarButtonType) = barButtonType.ToString()
            ViewData(Helpers.Constants.DefaultBarButtonData) = GetBarButtonData(barButtonType)

        End Sub

#End Region

    End Class

End Namespace