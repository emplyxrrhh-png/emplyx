@Imports Robotics.Base.DTOs
@Imports Robotics.Web.Base.roLanguageWeb

@ModelType roSurvey

@Code
    Layout = "~/Views/Shared/_layoutSP.vbhtml"
    Dim oSerializer As System.Web.Script.Serialization.JavaScriptSerializer
    oSerializer = New System.Web.Script.Serialization.JavaScriptSerializer()
    Dim SurveysController As VTLive40.SurveysController = New VTLive40.SurveysController()
    Dim baseURL = Url.Content("~")
    Dim scriptVersion As String = ViewData(VTLive40.Helpers.Constants.ScriptVersion)
End Code

<script>
    var RootUrl = "@Html.Raw(ViewBag.RootUrl)";
    var Permission = "@Html.Raw(ViewBag.PermissionOverEmployees)";
    var IsAdvancedEdition = "@Html.Raw(ViewBag.IsAdvancedEdition)";
    var BASE_URL = "@baseURL";
    var jsLabels = null;
</script>

<link href="@Url.Content("~/Base/Styles/roStart.min.css")@Html.Raw(scriptVersion)" rel="stylesheet" type="text/css" />
<link href="@Url.Content("~/Base/Styles/roLiveStyles.min.css")@Html.Raw(scriptVersion)" rel="stylesheet" type="text/css" />
<link href="@Url.Content("~/Base/Styles/dx.robotics.main.css")@Html.Raw(scriptVersion)" rel="stylesheet" type="text/css" />
<link href="@Url.Content("~/Base/Styles/dx.robotics.css")@Html.Raw(scriptVersion)" rel="stylesheet" type="text/css" />
<link href="@Url.Content("~/Base/surveyjs/survey.min.css")@Html.Raw(scriptVersion)" rel="stylesheet" type="text/css" />
<link href="@Url.Content("~/Base/surveyjs/survey-creator.css")@Html.Raw(scriptVersion)" rel="stylesheet" type="text/css" />
<link href="@Url.Content("~/Base/surveyjs/survey.analytics.min.css")@Html.Raw(scriptVersion)" rel="stylesheet" type="text/css" />
<link href="@Url.Content("~/Base/surveyjs/tabulator.min.css")@Html.Raw(scriptVersion)" rel="stylesheet" />
<link href="@Url.Content("~/Base/surveyjs/survey.analytics.tabulator.min.css")@Html.Raw(scriptVersion)" rel="stylesheet" />
<script src="@Url.Content("~/Base/Scripts/Ajax.js")@Html.Raw(scriptVersion)" type="text/javascript"></script>
<script src="@Url.Content("~/Base/surveyjs/survey.jquery.min.js")@Html.Raw(scriptVersion)" type="text/javascript"></script>
<script src="@Url.Content("~/Base/surveyjs/ace.min.js")@Html.Raw(scriptVersion)" type="text/javascript"></script>
<script src="@Url.Content("~/Base/surveyjs/ext-language_tools.js")@Html.Raw(scriptVersion)" type="text/javascript"></script>
<script src="@Url.Content("~/Base/surveyjs/typedarray.js")@Html.Raw(scriptVersion)" type="text/javascript"></script>
<script src="@Url.Content("~/Base/surveyjs/polyfill.min.js")@Html.Raw(scriptVersion)" type="text/javascript"></script>
<script src="@Url.Content("~/Base/surveyjs/plotly-2.1.0.min.js")@Html.Raw(scriptVersion)" type="text/javascript"></script>
<script src="@Url.Content("~/Base/surveyjs/wordcloud2.js")@Html.Raw(scriptVersion)" type="text/javascript"></script>
<script src="@Url.Content("~/Base/surveyjs/jspdf.min.js")@Html.Raw(scriptVersion)" type="text/javascript"></script>
<script src="@Url.Content("~/Base/surveyjs/jspdf.plugin.autotable.min.js")@Html.Raw(scriptVersion)" type="text/javascript"></script>
<script src="@Url.Content("~/Base/surveyjs/xlsx.full.min.js")@Html.Raw(scriptVersion)" type="text/javascript"></script>
<script src="@Url.Content("~/Base/surveyjs/tabulator.min.js")@Html.Raw(scriptVersion)" type="text/javascript"></script>
<script src="@Url.Content("~/Base/surveyjs/survey.ko.min.js")@Html.Raw(scriptVersion)" type="text/javascript"></script>
<script src="@Url.Content("~/Base/surveyjs/survey-creator.min.js")@Html.Raw(scriptVersion)" type="text/javascript"></script>
<script src="@Url.Content("~/Base/surveyjs/survey.analytics.min.js")@Html.Raw(scriptVersion)" type="text/javascript"></script>
<script src="@Url.Content("~/Base/surveyjs/survey.analytics.tabulator.min.js")@Html.Raw(scriptVersion)" type="text/javascript"></script>
<script src="@Url.Content("~/Base/Scripts/Live/Surveys/roSurvey.js")@Html.Raw(scriptVersion)" type="text/javascript"></script>
<script src="@Url.Content("~/Base/surveyjs/emotion-ratings.js")@Html.Raw(scriptVersion)" type="text/javascript"></script>
<script src="@Url.Content("~/Base/surveyjs/surveyjs-widgets.js")@Html.Raw(scriptVersion)" type="text/javascript"></script>

<div id="news" style="float:left"></div>
<div id="newsOk" style="float:right; margin-top:60px;"></div>

<div style="display:flex;">
    <div style="border-left: 1vw solid white;border-top: 1vw solid white; width:100%;border-right: 1vw solid white;">
        <div class="panHeader5" style="display: flex;align-items: center;">
            <div style="margin-right:0.5vw"><img src="~/Base/Images/StartMenuIcos/Surveys96.png" width="48" height="48" /></div>
            <div>
                <div style="font-size:20px;width:100%">@SurveysController.GetServerLanguage().Translate("roSurveysTitle", "Survey")</div>
                <div><span id="readOnlyDescritionCompany" style="font-size: 11px;font-weight: 100;">@SurveysController.GetServerLanguage().Translate("roSurveyInfo", "Survey")</span></div>
            </div>
        </div>
    </div>
</div>
<br />

<div id="divMainBody" style="min-height:unset !important; height:unset !important">
    <!-- TAB SUPERIOR -->
    <div id="divTabData">

        <input id="idSurveySelected" type="hidden" value="" />
        <input id="surveySelectedStatus" type="hidden" value="" />
        <input id="surveyMode" type="hidden" value="" />
        <input type='hidden' id='hdnEmployees' runat='server' value='' />
        <input type="hidden" id="hdnFilter" runat="server" value='' />
        <input type="hidden" id="hdnFilterUser" runat="server" value='' />

        <div id="divContenido" Class="divAllContentSurveys twoWindowsFlexLayout">

            <div id="divContenido" class="twoWindowsMainPanel maxHeight divRightContent">

                <div class="form ro-tab-section container" style="display: flex;">

                    <div class="sectionOnBoarding" style="flex-grow: 1;" id="section2">
                        <div id="surveyStatusResume" class="panHeader4">@SurveysController.GetServerLanguage().Translate("roSurveyStatus", "Survey")</div>
                        <br />

                        <div id="surveyActions" style="display: flex; justify-content: flex-end">
                            <div id="surveyRefresh" style="display: inline-block;margin-left:10px;">
                                @(Html.DevExtreme().Button() _
.ID("refreshSurvey") _
.Icon("refresh") _
.OnClick("refreshSurveys") _
.Type(ButtonType.Default) _
            )
                            </div>
                            <div id="surveyRefresh" style="display: inline-block;margin-left:10px;">
                                @If (ViewBag.PermissionOverEmployees > 3) Then
                                    @<div id="employeeStatus" style=" margin-left: auto; margin-right: 0;">
                                        @(Html.DevExtreme().Button() _
.ID("addSurvey") _
.Icon("plus") _
.OnClick("addNewSurvey") _
.Text(SurveysController.GetServerLanguage().Translate("roSurveyNew", "Survey")) _
.Type(ButtonType.Default)
            )
                                    </div>
                                End If
                            </div>
                        </div>

                        <br />

                        <div id="divSurveys" runat="server" Class="jsGridContentSurvey dextremeGrid">
                            @(Html.DevExtreme().DataGrid() _
.ID("gridStatusSurveys") _
.DataSource(Function(ds)
                Return ds.Mvc() _
                .Controller("Surveys") _
                .LoadAction("GetSurveys") _
                .Key("Id") _
                .OnBeforeSend("beforeSend") _
                .UpdateAction("UpdateSurvey") _
                .DeleteAction("DeleteSurvey") _
                .OnModified("RefreshSurveySurvey")
            End Function) _
.ShowColumnLines(False) _
.ShowRowLines(True) _
.Editing(Sub(edit)
             edit.AllowDeleting(New JS("AllowModify"))
             edit.Mode(GridEditMode.Cell)
             edit.RefreshMode(GridEditRefreshMode.Reshape)
             edit.Texts(Sub(texts)
                            texts.ConfirmDeleteMessage(SurveysController.GetServerLanguage().Translate("roSurveyDelete", "Survey"))
                            texts.ConfirmDeleteTitle(SurveysController.GetServerLanguage().Translate("roSurveyDeleteTitle", "Survey"))
                        End Sub)
             edit.UseIcons(True)
         End Sub) _
.Selection(Sub(columns)
               columns.Mode(SelectionMode.Single)
           End Sub) _
.RowAlternationEnabled(False) _
.ShowBorders(False) _
.ColumnHidingEnabled(False) _
.ColumnAutoWidth(False) _
.AllowColumnResizing(True) _
.OnRowRemoved("SurveyRemoved") _
.Export(Sub(columns)
            columns.Enabled(False)
            columns.AllowExportSelectedData(True)
        End Sub) _
.NoDataText(SurveysController.GetServerLanguage().Translate("roNoData", "Start")) _
.LoadPanel(Sub(columns)
               columns.Text(SurveysController.GetServerLanguage().Translate("roOnBoardingLoading", "OnBoarding"))
           End Sub) _
.Paging(Sub(columns)
            columns.PageSize(25)
        End Sub) _
.Pager(Sub(columns)
           columns.ShowPageSizeSelector(True)
           columns.AllowedPageSizes({25, 50, 100})
           columns.ShowInfo(False)
       End Sub) _
.OnCellClick("surveySelected") _
.OnCellPrepared("onCellPrepared") _
.OnContextMenuPreparing("context_menu") _
.FilterRow(Sub(columns)
               columns.Visible(True)
           End Sub) _
.HeaderFilter(Sub(columns)
                  columns.Visible(True)
              End Sub) _
.Columns(Sub(columns)
             columns.Add().DataField("Id").Visible(False)
             columns.Add().DataField("Title").AllowEditing(False).Caption(SurveysController.GetServerLanguage().Translate("roSurveyName", "Survey"))
             columns.Add().DataField("Status").AllowEditing(False).Caption(SurveysController.GetServerLanguage().Translate("roSurveyStatus", "Survey")) _
             .HeaderFilter(Sub(filter)
                               filter.DataSource(New JS("headerFilter"))
                           End Sub) _
             .CellTemplate(New JS("progressBar"))
             columns.Add().DataField("ExpirationDate").Width(150).AllowEditing(False).SortIndex(0).SortOrder(SortOrder.Desc).DataType(GridColumnDataType.Date).Format("dd/MM/yyyy").Alignment(HorizontalAlignment.Center).Caption(SurveysController.GetServerLanguage().Translate("roSurveyExpireOn", "Survey"))
             columns.Add().DataField("CreatedOn").Width(150).AllowEditing(False).DataType(GridColumnDataType.Date).Format("dd/MM/yyyy").Alignment(HorizontalAlignment.Center).Caption(SurveysController.GetServerLanguage().Translate("roSurveyCreatedOn", "Survey"))
             columns.Add().Type(GridCommandColumnType.Buttons).Buttons(Sub(b)
                                                                           b.Add().Hint("Clonar").Icon("unselectall").OnClick("copySurvey")
                                                                           b.Add().Name(GridColumnButtonName.Delete)
                                                                       End Sub)

         End Sub) _
.OnToolbarPreparing("toolbar_preparing") _
                                                                                )

                            @Code
                                Html.DevExtreme().Popup() _
                                                                                                                                        .ID("newSurveyPopup") _
                                                                                                                                        .Width(New JS("getWidth")) _
                                                                                                                                        .Height(New JS("getHeight")) _
                                                                                                                                        .ShowTitle(False) _
                                                                                                                                        .Title("Crear encuesta") _
                                                                                                                                        .OnShown("onSurveyPopupShown") _
                                                                                                                                        .OnHiding("onSurveyPopupHiding") _
                                                                                                                                        .DragEnabled(False) _
                                                                                                                                        .HideOnOutsideClick(True) _
                                                                                                                                        .ContentTemplate(Sub()
                                                                                                                                        @<text>
                                                                                                                                            @Html.Partial("CreateSurvey")
                                                                                                                                        </text>
                                                                                                                                    End Sub) _
.Render()
                            End Code
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>
<div id="divSurveysEmployeeSelector">
</div>

<Script>
</Script>