@Imports Robotics.Base.DTOs

@ModelType roGeniusView
@Code
    Dim labels = ViewData(VTLive40.Helpers.Constants.DefaultLanguagesEntries)
End Code

<section id="geniusAnalyticLauncher">
    <form id="mainForm">
        <div class="aParent updateReport" style="display:none;float:right; margin-top:1px;margin-bottom:20px;width:10%;margin-right:10px">
            <div id="zoneSave" style="display: inline-block;margin-left:10px; float:right;">
                @(Html.DevExtreme().Button() _
                .ID("btnUpdateGeniusView") _
                .Icon("todo") _
                .Type(ButtonType.Default) _
                .OnClick("updateGeniusView") _
                )
            </div>
            <div id="zoneCancel" style="display: inline-block; margin-left: 10px;float:right;">
                @(Html.DevExtreme().Button() _
                .ID("btnCancelUpdateGeniusView") _
                .Icon("close") _
                .Type(ButtonType.Danger) _
                .OnClick("closeUpdatePopUp") _
                )
            </div>
        </div>
        <div class="panHeader2 panBottomMargin updateReport" style="display:none;float:left;width:87%;padding: 10px;">
            <span class="panelTitleSpan">
                <span id="">@Html.Raw(labels("Genius#lblgeniusviewcreate"))</span>
            </span>
        </div>
        <div class="panHeader2 panBottomMargin runReport">
            <span class="panelTitleSpan">
                <span id="">@Html.Raw(labels("Genius#MainSection"))</span>
            </span>
        </div>
        <div class="updateReport" style="display:none;width: 95%;margin: 0 auto;">
            <div class="d-flex justify-content-start">
                <div class="dx-field-label geniusLabel">
                    @Html.Raw(labels("Genius#reportName"))
                </div>
                <br />
                <div class="dx-field-value">
                    @(Html.DevExtreme().TextBox() _
                .ID("txtUpdatedName") _
                    )
                </div>
            </div>
        </div>
        <div class="updateReport" style="display:none;float:left;min-height:15px;width:100%"></div>
        <div class="updateReport" style="display:none">
            <div class="d-flex justify-content-start">
                <div class="dx-field-label geniusLabel">
                    @Html.Raw(labels("Genius#dataType"))
                </div>
                <div id="lstAnalyticsTypeRead" style="float:left;width:95%;">
                    @For Each item In ViewBag.AnalyticsType
                    @<div Class="lstAnalyticsTypeItem">
                        @Html.DevExtreme().CheckBox().ID("chkCheckbox" + item.ID.ToString()).ElementAttr("class", "checkBoxAnalyticTypeRead").Disabled(True).Text(item.Name)
                    </div>
                    Next
                </div>
            </div>
        </div>
        <div style="float:left;min-height:15px;width:100%"></div>
        <div style="float: left; width: 95%; margin: 0 auto;">
            <div id="divDestination" class="divDestination">
                <div class=" d-flex justify-content-start">
                    <div class="dx-field-label geniusLabel">
                        @Html.Raw(labels("Genius#whoAnalyze"))
                    </div>
                    <br />
                    <div class="dx-field-value">
                        @(Html.DevExtreme().TextBox() _
                        .ID("ReportUsers") _
                        .Placeholder(labels("Genius#selectUsers")) _
                    )
                    </div>
                </div>
            </div>
        </div>

        <div class="costCenterInfo" style="float:left;display:none;min-height:15px;width:100%;"></div>

        <div class="costCenterInfo" style="float:left;display:none;width: 95%;margin: 0 auto;">
            <div class="d-flex justify-content-start">
                <div class="dx-field-label geniusLabel">
                    @Html.Raw(labels("Genius#costCenterLbl"))
                </div>
                <br />
                <div class="dx-field-value">
                    @(Html.DevExtreme().TagBox() _
        .ID("txtCostCenters") _
        .Placeholder(labels("Genius#selectCostCenter")) _
        .ShowSelectionControls(True) _
        .ValueExpr("ID") _
        .DisplayExpr("Name") _
        .Multiline(False) _
        .ShowClearButton(True) _
        .SearchEnabled(True) _
        .ApplyValueMode(EditorApplyValueMode.UseButtons) _
        .SearchExpr("Name") _
        .OnSelectionChanged("costCentersSelected") _
        .DataSource(ViewBag.LstCostCenters) _
                    )
                </div>
            </div>
        </div>

        <div class="conceptsInfo" style="float:left;display:none;min-height:15px;width:100%;"></div>

        <div class="conceptsInfo" style="float:left;display:none;width: 95%;margin: 0 auto;">
            <div class="d-flex justify-content-start">
                <div class="dx-field-label geniusLabel">
                    @Html.Raw(labels("Genius#conceptsLbl"))
                </div>
                <br />
                <div class="dx-field-value">
                    @(Html.DevExtreme().TagBox() _
        .ID("txtConcepts") _
        .Placeholder(labels("Genius#selectConcepts")) _
        .ShowSelectionControls(True) _
        .ValueExpr("ID") _
        .DisplayExpr("Name") _
        .Multiline(False) _
        .ShowClearButton(True) _
        .SearchEnabled(True) _
        .ApplyValueMode(EditorApplyValueMode.UseButtons) _
        .SearchExpr("Name") _
        .OnSelectionChanged("conceptsSelected") _
        .DataSource(ViewBag.lstConcepts) _
                    )
                </div>
            </div>
        </div>

        <div class="causesInfo" style="float:left;min-height:15px;width:100%"></div>
        <div class="causesInfo" style="float: left; width: 95%; margin: 0 auto;">
            <div class="d-flex justify-content-start">
                <div class="dx-field-label geniusLabel">
                    @Html.Raw(labels("Genius#causesLbl"))
                </div>
                <br />
                <div class="dx-field-value">
                    <input id="hdnAllCausesText" type="hidden" value="@Html.Raw(labels("Genius#allCausesSelected"))">
                    <input id="hdnSomeCausesSelectedText" type="hidden" value="@Html.Raw(labels("Genius#someCausesSelected"))">
                    <input id="hdnCauseSelectedText" type="hidden" value="@Html.Raw(labels("Genius#causeSelected"))">
                    @(Html.DevExtreme().TextBox() _
        .ID("txtCauses") _
        .Placeholder(labels("Genius#selectCauses")) _
                    )
                </div>
            </div>
        </div>

        <div class="requestsInfo" style="float:left;display:none;min-height:15px;width:100%;"></div>

        <div class="requestsInfo" style="float:left;display:none;width: 95%;margin: 0 auto;">
            <div class="d-flex justify-content-start">
                <div class="dx-field-label geniusLabel">
                    @Html.Raw(labels("Genius#requestsLbl"))
                </div>
                <br />
                <div class="dx-field-value">
                    @(Html.DevExtreme().TagBox() _
    .ID("txtRequests") _
    .Placeholder(labels("Genius#selectRequests")) _
    .ShowSelectionControls(True) _
    .ValueExpr("ID") _
    .DisplayExpr("Name") _
    .Multiline(False) _
    .ShowClearButton(True) _
    .SearchEnabled(True) _
    .ApplyValueMode(EditorApplyValueMode.UseButtons) _
    .SearchExpr("Name") _
    .OnSelectionChanged("requestsSelected") _
    .DataSource(ViewBag.lstRequests) _
                    )
                </div>
            </div>
        </div>

        <div class="timeInfo runReport" style="float:left;display:none;min-height:15px;width:100%;"></div>

        <div class="timeInfo runReport" style="float: left; display: none;width: 95%; margin: 0 auto;">
            <div class="d-flex justify-content-start">
                <div class="dx-field-label geniusLabel">
                    @Html.Raw(labels("Genius#lblDefinePeriod"))
                </div>
                <div style="display:none;width:calc(30%);float:left;margin:10px 0px 10px 10px">
                    <div id="rgDateFilterType" data-binding="DateFilterType"></div>
                </div>
                <div class="d-flex justify-content-start">
                    <div class="dx-field-label inlinegeniusLabel">@Html.Raw(labels("Genius#lblDefinePeriodFrom"))</div>
                    <div>
                        @(Html.DevExtreme().DateBox() _
.ID("txtInitialTime") _
.Type(DateBoxType.Time)
                                )
                    </div>
                    <div style="margin-left:20px" class="dx-field-label inlinegeniusLabel">@Html.Raw(labels("Genius#lblDefinePeriodTo")) </div>
                    <div>
                        @(Html.DevExtreme().DateBox() _
.ID("txtEndTime") _
.Type(DateBoxType.Time)
                                )
                    </div>
                </div>
            </div>
        </div>

        <div class="updateReport" style="display:none;min-height:15px;width:100%;"></div>

        <div class="updateReport" style="display:none;width: 95%;margin: 0 auto;">
            <div id="divUserFields" class="d-flex justify-content-start">
                <div class="dx-field-label geniusLabel">
                    @Html.Raw(labels("Genius#lblUserFields"))
                </div>
                <br />
                <div class="dx-field-value">
                    @(Html.DevExtreme().TagBox() _
.ID("UserFieldsUpdateText") _
.Placeholder("Selecciona los campos") _
.ShowSelectionControls(True) _
.ValueExpr("Name") _
.DisplayExpr("Name") _
.Multiline(False) _
.SearchEnabled(True) _
.OnKeyDown("userFieldsKeyDown") _
.OnInitialized("userFieldsInitialized") _
.ApplyValueMode(EditorApplyValueMode.UseButtons) _
.SearchExpr("Name") _
.OnSelectionChanged("userFieldsSelected") _
.OnContentReady("userFieldContentReady") _
.OnValueChanged("userFieldValueChanged") _
.OnOpened("userFieldListOpened") _
.DataSource(ViewBag.LstUserFields) _
                    )
                </div>
            </div>
        </div>
        <div class="updateReport" style="display:none;min-height:15px"></div>

        <div class="updateReport" style="display:none;width: 95%;margin: 0 auto;">
            <div class="d-flex justify-content-start">
                <div class="dx-field-value geniusLabel">
                    @Html.Raw(labels("Genius#lblConfLanguage"))
                </div>
                <br />
                <div class="dx-field-value">
                    @(Html.DevExtreme.SelectBox() _
        .ID("selectLanguage") _
        .DataSource(ViewBag.LstLanguages) _
        .ValueExpr("ID") _
        .DisplayExpr("Name")
                    )
                </div>
            </div>
        </div>
        <div class="updateReport zeroAvailable" style="display:none;min-height:15px"></div>

        <div class="updateReport zeroAvailable" style="display:none;width: 95%;margin: 0 auto;">
            <div class="d-flex justify-content-start">
                <div class="dx-field-value noFixedWidth" style="width: auto;max-width: 100%;">
                    @(Html.DevExtreme.CheckBox().ID("ckIncludeConceptsWithZeros").Text(labels("Genius#ckIncludeConceptsWithZeros")))
                </div>
            </div>
        </div>
        <div class="updateReport zeroCausesAvailable" style="display:none;min-height:15px"></div>

        <div class="updateReport zeroCausesAvailable" style="display:none;width: 95%;margin: 0 auto;">
            <div class="d-flex justify-content-start">
                <div class="dx-field-value noFixedWidth" style="width: auto;max-width: 100%;">
                    @(Html.DevExtreme.CheckBox().ID("ckIncludeCausesWithZeros").Text(labels("Genius#ckIncludeCausesWithZeros")))
                </div>
            </div>
        </div>

        <div class="updateReport costcenterAndIncidencesinfo" style="min-height:15px"></div>

        <div class="updateReport costcenterAndIncidencesinfo" style="width: 95%;margin: 0 auto;">
            <div class="d-flex justify-content-start">
                <div class="dx-field-value noFixedWidth" style="width: auto;max-width: 100%;">
                    @(Html.DevExtreme.CheckBox().ID("ckIncludeBusinessCenterWithZeros").Text(labels("Genius#ckIncludeEntriesWithoutBusinessCenter")))
                </div>
            </div>
        </div>

        <div style="min-height:15px"></div>
        <div class="updateReport" style="display:none;min-height:15px"></div>
        <div class="updateReport" style="display:none;width: 95%;margin: 0 auto;">
            <div class="d-flex justify-content-start">
                <div class="dx-field-value noFixedWidth" style="">
                    @(Html.DevExtreme.CheckBox() _
         .ID("ckUpdatedSendEmail").Text(labels("Genius#ckSendEmail"))
                    )
                </div>
            </div>
        </div>

        <div style="min-height:15px"></div>
        @if ViewBag.BIIntegration = True Then
        @<div class="updateReport" style="display:none;min-height:15px"></div>
        @<div class="updateReport" style="display:none;width: 95%;margin: 0 auto;">
            <div class="d-flex justify-content-start">
                <div class="dx-field-value noFixedWidth">
                    @(Html.DevExtreme.CheckBox() _
            .ID("ckUpdatedDownloadBI").Disabled(True).Text(labels("Genius#ckDownloadBI"))
                    )
                </div>
                <div class="dx-field-value noFixedWidth" style="margin-left:20px">
                    @(Html.DevExtreme.CheckBox() _
            .ID("ckUpdatedOverwriteResults").Text(labels("Genius#ckOverwriteResults"))
                    )
                </div>
            </div>
        </div>

        End If
        <div style="min-height:15px"></div>

        <div class="runReport" style="min-height:15px"></div>
        <div class="runReport">
            <div class="runsDescriptionPopUp">
                <span id="">@Html.Raw(labels("Genius#RunDesc"))</span>
            </div>

            <div class="d-flex justify-content-start">
                <div class="dx-field-label geniusLabel">
                    @Html.Raw(labels("Genius#lblDefinePeriod"))
                </div>
                <br />
                <div style="display:none;width:calc(30%);float:left;margin:10px 0px 10px 10px">
                    <div id="rgDateFilterType" data-binding="DateFilterType"></div>
                </div>
                <div class="d-flex justify-content-start">
                    <div class="dx-field-label inlinegeniusLabel">@Html.Raw(labels("Genius#lblDefinePeriodFrom"))</div>
                    <div id="dtDateInf" data-binding="DateInf" conf-="" date-type="date" date-format="yyyy-MM-dd" date-displayformat="@Html.Raw(ViewBag.Format)" date-pickerType="calendar"></div>
                    <div style="margin-left:20px" class="dx-field-label inlinegeniusLabel">@Html.Raw(labels("Genius#lblDefinePeriodTo")) </div>
                    <div id="dtDateSup" data-binding="DateSup" date-type="date" date-format="yyyy-MM-dd" date-displayformat="@Html.Raw(ViewBag.Format)" date-pickerType="calendar"></div>
                </div>

                <div style="padding-left: 30px;">
                    @(Html.DevExtreme().Button() _
                .ID("newView") _
                .Icon("video") _
                .Type(ButtonType.Default) _
                .Text(labels("Genius#RunGenius")) _
                .OnClick("function() { createGeniusView(); }"))
                    @(Html.DevExtreme().Button() _
                            .ID("runView") _
                            .Icon("video") _
                            .Type(ButtonType.Default) _
                            .Text(labels("Genius#RunGenius")) _
                            .OnClick("function() { runGeniusView(); }"))
                </div>
            </div>
        </div>
    </form>
</section>