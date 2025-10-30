@Code
    Dim oSerializer As System.Web.Script.Serialization.JavaScriptSerializer
    oSerializer = New System.Web.Script.Serialization.JavaScriptSerializer()
    Dim SurveyController As VTLive40.SurveysController = New VTLive40.SurveysController()
End Code

<script src="@Url.Content("~/Base/Scripts/Live/Surveys/roSurveyResults.js")" type="text/javascript"></script>

<div class="aParent" style="margin-top: 1px; width: 50%; z-index: 200000; margin-left: auto; margin-right: 0; display: flex; justify-content: flex-end; margin-bottom:10px; ">

    <div style="display: inline-block; margin-left: 10px; margin-right: 25px; margin-top: 5px;">
        @(Html.DevExtreme().RadioGroup() _
                                                        .ID("radioGroupMode") _
                                                                                                                            .Items({SurveyController.GetServerLanguage().Translate("roSurveyConfiguration", "graphic"), SurveyController.GetServerLanguage().Translate("roSurveyConfiguration", "table")}) _
                                                                                                                    .Value(SurveyController.GetServerLanguage().Translate("roSurveyConfiguration", "graphic")) _
                                                        .OnValueChanged("switchMode") _
                                                                                                            .Layout(Orientation.Horizontal) _
                )
    </div>
    @*<div id="surveyResultsMode" style="display: inline-block;margin-left:10px;">
                @(Html.DevExtreme().Switch() _
                .ID("swSurveyResultsMode") _
                .Value(True) _
                .SwitchedOffText("Tabla") _
            .Width(150) _
        .Height(150) _
                .SwitchedOnText("Gráficos") _
                .OnValueChanged("switchMode") _
                    )
            </div>*@
    <div id="divResultsByEmployee" style="display: inline-block;margin-left:10px; width:60%;">
        @(Html.DevExtreme().TagBox() _
                                                                                        .ID("EmployeeTextResults") _
                                                                                        .Placeholder(SurveyController.GetServerLanguage().Translate("roSurveyConfiguration", "showResponsesof")) _
                                                                                        .ValueExpr("Id") _
                                                                                        .DisplayExpr("EmployeeName") _
                                                                                        .SearchExpr("EmployeeName") _
                                        .ShowSelectionControls(True) _
                                                                                        .ShowClearButton(True) _
                                                .ApplyValueMode(EditorApplyValueMode.UseButtons) _
                                                        .Multiline(False) _
                                                                        .ShowClearButton(True) _
                                                                        .SearchEnabled(True) _
                                                                                        .OnSelectionChanged("employeeResultsSelected") _
                )
    </div>
</div>

<div id="loadingIndicator">
    <span>
        <div id="loading">
            <strong>Cargando...</strong>
            <span></span>
        </div>
    </span>
</div>
<div>
    @Code
        Html.DevExtreme().ScrollView() _
.Width("100%") _
.Height(New JS("getHeightResults")) _
.ShowScrollbar(ShowScrollbarMode.Always) _
.Direction(ScrollDirection.Both) _
.Content(Sub()@<text>
            <div id="results">
            </div>
            <div id="surveyElement" style="display:inline-block;width:100%;"></div>
            <div id="surveyResult"></div>
</text>
End Sub).Render()
    End Code
</div>

<script>
</script>