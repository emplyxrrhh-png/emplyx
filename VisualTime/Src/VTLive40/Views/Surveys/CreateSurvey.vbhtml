@Imports Robotics.Base.DTOs

@Code
    Dim oSerializer As System.Web.Script.Serialization.JavaScriptSerializer
    oSerializer = New System.Web.Script.Serialization.JavaScriptSerializer()
    Dim SurveysController As VTLive40.SurveysController = New VTLive40.SurveysController()
    Dim items = {New roItem With {.Id = 0, .Text = SurveysController.GetServerLanguage().Translate("roSurveySimple", "Survey")}, New roItem With {.Id = 1, .Text = SurveysController.GetServerLanguage().Translate("roSurveyAdvanced", "Survey")}}
End Code

<script src="@Url.Content("~/Base/Scripts/Live/Surveys/roSurveyCreator.js")" type="text/javascript"></script>

<style>
    .dx-overlay-shader {
        background-color: transparent !important;
    }

    .surveyField .dx-field-label, #divDestination .dx-field-label {
        width: 20% !important;
    }

    .surveyField .dx-field-value, #divDestination .dx-field-value {
        width: 80% !important;
    }
</style>

<div class="aParent" style="float:left; margin-bottom:20px;">
    <div id="surveyConfiguration" class="bTabSurveys" style="height:35px !important; cursor: pointer;">
        @SurveysController.GetServerLanguage().Translate("roSurveyConfiguration", "Survey")
    </div>
    <div id="surveyDesign" class="bTabSurveys" style="height: 35px !important; cursor: pointer;">
        @SurveysController.GetServerLanguage().Translate("roSurveyDesign", "Survey")
    </div>
    <div id="surveyResults" class="bTabSurveys" style="height: 35px !important; cursor: pointer;">
        @SurveysController.GetServerLanguage().Translate("roSurveyResults", "Survey")
    </div>
</div>

<div class="aParent" style="float:right; margin-top:1px;">
    <div id="surveySend" style="display: inline-block;">
        @(Html.DevExtreme().Button() _
                .ID("btnSendSurvey") _
                .Icon("chevrondoubleright") _
                .OnClick("sendSurvey") _
                .Text(SurveysController.GetServerLanguage().Translate("roSurveySend", "Survey")) _
                .Type(ButtonType.Success) _
            )
    </div>
    <div id="surveySave" style="display: inline-block;margin-left:10px;">
        @(Html.DevExtreme().Button() _
                        .ID("addNewSurvey") _
                        .Icon("todo") _
                        .OnClick("createNewSurvey") _
                        .Type(ButtonType.Default) _
            )
    </div>
    <div id="surveyCancel" style="display: inline-block; margin-left: 10px;">
        @(Html.DevExtreme().Button() _
                .ID("btnCancelSurvey") _
                .Icon("close") _
                .OnClick("cancelSurvey") _
                .Type(ButtonType.Danger) _
            )
    </div>
</div>

<br />
<br />
<div id="loadingDiv" style=" z-index: 20000; position: absolute;top: 25%; left: 50%;">
    @(Html.DevExtreme().LoadIndicator() _
                        .ID("loading") _
                        .Height(60) _
                        .Width(60) _
                        .Visible(False) _
    )
</div>
<div id="divMainBody" style="min-height:unset !important; height:unset !important">
    <!-- TAB SUPERIOR -->
    <div id="divTabData">
        <div id="divContenido" Class="divAllContentSurveys twoWindowsFlexLayout">

            <div id="divContenido" class="twoWindowsMainPanel maxHeight divRightContent">

                <div class="form ro-tab-section container" style="display: flex;">

                    <div class="sectionSurveys" style="flex-grow: 1;" id="section2">

                        <!--DIV CONFIGURACION-->
                        <div id="configDiv" style="width: 100%; margin: 0 auto;">
                            <div class="panHeader4" style="padding: 10px;">
                                @SurveysController.GetServerLanguage().Translate("roSurveyGeneral", "Survey")
                            </div>

                            <div class="list-containerRequests">
                                <br />
                                <div style="width: 95%;margin: 0 auto;">
                                    <div class="surveyField" style="display: flex;justify-content: center;">
                                        <div class="dx-field-label">
                                            @SurveysController.GetServerLanguage().Translate("roSurveyName", "Survey")
                                        </div>
                                        <div class="dx-field-value">
                                            @(Html.DevExtreme().TextBox() _
.ID("SurveyName") _
.Placeholder(SurveysController.GetServerLanguage().Translate("roSurveyTitleDesc", "Survey")) _
                )
                                        </div>
                                    </div>
                                    <br />
                                    <div id="divDestination" style="display: flex;justify-content: center;">
                                        <div class="dx-field-label">
                                            @SurveysController.GetServerLanguage().Translate("roSurveyDestination", "Survey")
                                        </div>
                                        <div class="dx-field-value">
                                            @(Html.DevExtreme().TextBox() _
.ID("SurveyDestination") _
.Placeholder(SurveysController.GetServerLanguage().Translate("roSurveyDestinationDesc", "Survey")) _
                )
                                        </div>
                                    </div>
                                    <br />
                                    <div class="surveyField" style="display: flex;justify-content: center;">

                                        <div Class="dx-field-label">
                                            @SurveysController.GetServerLanguage().Translate("roSurveyExpireDateInfo", "Survey")
                                        </div>

                                        <div id="divExpireDate" class="dx-field-value">

                                            @(Html.DevExtreme().DateBox() _
.ID("dateSelector") _
.DisplayFormat("dd/MM/yyyy") _
.Type(DateBoxType.Date) _
            )
                                        </div>
                                    </div>
                                    <br />
                                    <div class="surveyField" style="display: flex;justify-content: center;">

                                        @If (ViewBag.IsAdvancedEdition) Then
                                        @<div Class="dx-field-label">

                                            @SurveysController.GetServerLanguage().Translate("roSurveyMandatory", "Survey")
                                        </div>
                                        End If
                                        <div Class="dx-field-value" style="display:flex">
                                            @(Html.DevExtreme().Switch() _
.ID("SurveyMandatory") _
.Value(False) _
.Visible(ViewBag.IsAdvancedEdition) _
.SwitchedOffText(SurveysController.GetServerLanguage().Translate("roSurveyNo", "Survey")) _
.SwitchedOnText(SurveysController.GetServerLanguage().Translate("roSurveyYes", "Survey")) _
                )
                                        </div>
                                    </div>

                                    <br />
                                    <div class="surveyField" style="display: flex;justify-content: center;">

                                        @If (ViewBag.IsAdvancedEdition) Then
                                        @<div Class="dx-field-label">

                                            @SurveysController.GetServerLanguage().Translate("roSurveyAnonymous", "Survey")
                                        </div>
                                        End If
                                        <div Class="dx-field-value" style="display:flex">
                                            @(Html.DevExtreme().Switch() _
.ID("SurveyAnonymous") _
.Value(False) _
.Visible(ViewBag.IsAdvancedEdition) _
.SwitchedOffText(SurveysController.GetServerLanguage().Translate("roSurveyNo", "Survey")) _
.SwitchedOnText(SurveysController.GetServerLanguage().Translate("roSurveyYes", "Survey")) _
                )
                                        </div>
                                    </div>
                                </div>
                                <br />
                                <br />

                                <div Class="panHeaderListOptional">
                                    @SurveysController.GetServerLanguage().Translate("roSurveyExpiration", "Survey")
                                </div>

                                <br />

                                <div style="width: 60%; ">

                                    <div class="surveyField" style="display: flex;justify-content: center;">

                                        <div Class="dx-field-label" style="width:30%">
                                            @(Html.DevExtreme().CheckBox() _
.ID("SurveyExpiredByPercentage") _
.Value(False) _
.OnValueChanged("byPercentage") _
.Text(SurveysController.GetServerLanguage().Translate("roSurveyPercentageInfo", "Survey")) _
                )
                                        </div>
                                        <div id="divExpirePercentage" Class="dx-field-value">
                                            <br />
                                            <br />
                                            <div class="surveyField" style="display: flex;justify-content: center;">

                                                <div Class="dx-field-value">
                                                    @(Html.DevExtreme().Slider() _
.ID("SurveyPercentage") _
.Min(0) _
.Max(100) _
.Value(35) _
.Disabled(True) _
.Tooltip(Sub(columns)
             columns.Enabled(True)
             columns.Format(New JS("formatLabel"))
             columns.Position(VerticalEdge.Top)
             columns.ShowMode(SliderTooltipShowMode.Always)
         End Sub) _
                )
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <!--DIV DESIGN-->

                        <div id="surveyDiv" style="width: 100%; margin: 0 auto;">
                            <div id="divSurveyMode" Class="aParent" style="margin-top: 1px; width: 50%; z-index: 200000; margin-left: auto; margin-right: 0; display: flex; justify-content: flex-end;  ">
                                <div style="display: inline-block; margin-left: 10px; margin-right: 25px; margin-top: 5px;">
                                    @(Html.DevExtreme().RadioGroup() _
.ID("radioSurveyMode") _
.DataSource(items) _
.ValueExpr("Id") _
.DisplayExpr("Text") _
.Visible(ViewBag.IsAdvancedEdition) _
.Value(SurveysController.GetServerLanguage().Translate("roSurveySimple", "Survey")) _
.OnValueChanged("switchSurveyMode") _
.Layout(Orientation.Horizontal) _
                )
                                </div>
                            </div>

                            <Script type="text/html" id="custom-tab-survey-templates">
                                <h3 style="padding: 5px 5px; font-weight: 600; /* text-align: justify; */ -moz-box-shadow: -4px 0 0 0 #000; /* -webkit-box-shadow: -4px 0 0 0 #000; */ /* box-shadow: -4px 0 0 0 #000; */ background-color: #FF5C35; border-radius: 5px; color: #fff; box-shadow: 0 0 5px rgb(52 52 52 / 50%);" data-bind="text: title"></h3>
                                <div style="background-color: white; border-radius: 10px;">
                                    <table class="table" style="width: 500px; margin: 0 auto;">
                                        <thead>
                                            <tr>
                                                <th></th>
                                                <th></th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            <!-- ko foreach: surveys -->
                                            <tr>
                                                <td style="padding: 8px 15px 9px 0; font-size:12px;" data-bind="text:name"></td>
                                                <td>
                                                    <button style="border-radius: 5px; border-width: 0px; width: 70px; height: 25px;" data-bind="click:$parent.load($data)">Cargar</button>
                                                </td>
                                            </tr>
                                            <!-- /ko -->
                                        </tbody>
                                    </table>
                                </div>
                            </Script>

                            <div id="surveyCreador" style="width: 100%; margin: 0 auto;">
                            </div>
                        </div>

                        <div id="resultsDiv" style="width: 100%; margin: 0 auto;">
                            @Html.Partial("SurveyResults")
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>

<Script>
</Script>