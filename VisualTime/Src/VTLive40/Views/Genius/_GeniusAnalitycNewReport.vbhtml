@Imports Robotics.Base.DTOs

@ModelType roGeniusView
@Code
    Dim labels = ViewData(VTLive40.Helpers.Constants.DefaultLanguagesEntries)
End Code

<section id="geniusAnalyticNew">
    <div class="panHeader2 panBottomMargin">
        <span class="panelTitleSpan">
            <span id="">@Html.Raw(labels("Genius#createReport"))</span>
        </span>
    </div>
    <div style="min-height:15px"></div>
    <div style="width: 95%;margin: 0 auto;">
        <div class="d-flex justify-content-start">
            <div class="dx-field-label geniusLabel">
                @Html.Raw(labels("Genius#reportName"))
            </div>
            <div class="dx-field-value">
                @(Html.DevExtreme().TextBox() _
                .ID("txtName") _
                )
            </div>
        </div>
    </div>
    <div style="min-height:15px"></div>
    <div style="width: 95%;margin: 0 auto;">
        <div style="display: flex;">
            <div class="dx-field-label geniusLabel">
                @Html.Raw(labels("Genius#dataType"))
            </div>
        </div>
        <div style="display: flex;">
            <br />
            <div id="lstAnalyticsType" style="float:left;width:900px;">
                @For Each item In ViewBag.AnalyticsType
                @<div Class="lstAnalyticsTypeItem">
                    @Html.DevExtreme().CheckBox().OnValueChanged("onChkAnalyticTypeChange").ElementAttr(New Dictionary(Of String, Object) From {{"class", item.Classes2Str + " checkBoxAnalyticType"}}).ID("chkAnalyticType" + item.ID.ToString()).Text(item.Name)
                    @If item.ID = 1 Then
                    @(Html.DevExtreme().Button() _
                                                                    .ID("btnCausesInfo") _
                                                                    .Type(ButtonType.Normal) _
                                                                    .Text("[...]") _
                                                                    .Disabled(True) _
                                                                    .OnClick("function() { showCausesPopUp(); }"))
                        End If
                    @If item.ID = 4 Then
                    @(Html.DevExtreme().Button() _
                                                                    .ID("btnCostCenterInfo") _
                                                                    .Type(ButtonType.Normal) _
                                                                    .Text("[...]") _
                                                                    .Disabled(True) _
                                                                    .OnClick("function() { showCostsCenterPopUp(); }"))
                        End If
                    @If item.ID = 6 Then
                    @(Html.DevExtreme().Button() _
                                                                    .ID("btnBalancesInfo") _
                                                                    .Type(ButtonType.Normal) _
                                                                    .Text("[...]") _
                                                                    .Disabled(True) _
                                                                    .OnClick("function() { showBalancesPopUp(); }"))
                        End If
                    @If item.ID = 5 Then
                    @(Html.DevExtreme().Button() _
                                                                    .ID("btnRequestsInfo") _
                                                                    .Disabled(True) _
                                                                    .Type(ButtonType.Normal) _
                                                                    .Text("[...]") _
                                                                    .OnClick("function() { showRequestsPopUp(); }"))
                        End If
                </div>
                Next
            </div>
        </div>
    </div>
    <div style="min-height:15px"></div>

    <div style="width: 95%;margin: 0 auto;">
        <div id="divDestination" class="d-flex justify-content-start">
            <div class="dx-field-label geniusLabel">
                @Html.Raw(labels("Genius#whoAnalyze"))
            </div>
            <div class="dx-field-value">
                @(Html.DevExtreme().TextBox() _
                .ID("ReportUsersNew") _
                .Placeholder(labels("Genius#selectUsers")) _
                )
            </div>
        </div>
    </div>
    <div style="min-height:15px"></div>
    <div style="width: 95%;margin: 0 auto;">
        <div id="divUserFields" class="d-flex justify-content-start">
            <div class="dx-field-label geniusLabel">
                @Html.Raw(labels("Genius#lblUserFields"))
            </div>
            <div class="dx-field-value">
                @(Html.DevExtreme().TagBox() _
                .ID("UserFieldsText") _
                .Placeholder(labels("Genius#selectUserFields")) _
                .ShowSelectionControls(True) _
                .ValueExpr("ID") _
                .DisplayExpr("Name") _
                .Multiline(False) _
                .ShowClearButton(True) _
                .SearchEnabled(True) _
                .ApplyValueMode(EditorApplyValueMode.UseButtons) _
                .SearchExpr("Name") _
                .OnValueChanged("userFieldValueChanged") _
                .DataSource(ViewBag.LstUserFields) _
                )
            </div>
        </div>
    </div>
    <div style="min-height:15px"></div>
    <div style="width: 95%;margin: 0 auto;">
        <div class="d-flex justify-content-start">
            <div class="dx-field-label geniusLabel">
                @Html.Raw(labels("Genius#lblDefinePeriod"))
            </div>
            <br />

            <div class="d-flex justify-content-start">
                <div class="dx-field-label inlinegeniusLabel">@Html.Raw(labels("Genius#lblDefinePeriodFrom"))</div>
                <div id="dtDateInfNew" data-binding="DateInf" conf-="" date-type="date" date-format="yyyy-MM-dd" date-displayformat="@Html.Raw(ViewBag.Format)" date-pickerType="calendar"></div>
                <div style="margin-left:20px" class="dx-field-label inlinegeniusLabel">@Html.Raw(labels("Genius#lblDefinePeriodTo")) </div>
                <div id="dtDateSupNew" data-binding="DateSup" date-type="date" date-format="yyyy-MM-dd" date-displayformat="@Html.Raw(ViewBag.Format)" date-pickerType="calendar"></div>
            </div>
        </div>
    </div>
    @If ViewBag.BIIntegration = True Then
    @<div style="min-height:15px"></div>
    @<div style="width: 95%;margin: 0 auto;">
        <div class="d-flex justify-content-start">
            <div class="dx-field-value noFixedWidth" style="">
                @(Html.DevExtreme.CheckBox() _
                                .ID("ckDownloadBI").Text(labels("Genius#ckDownloadBI")).Value(False).OnValueChanged("ckDownloadBI_valueChanged")
                )
            </div>
            <div class="dx-field-value noFixedWidth" style="margin-left:20px">
                @(Html.DevExtreme.CheckBox() _
                                    .ID("ckOverwriteResults").Text(labels("Genius#ckOverwriteResults")).Value(False)
                )
            </div>
        </div>
    </div>
    End If
    <div style="min-height:15px"></div>
    <div style="width: 95%;margin: 0 auto;">
        <div class="d-flex justify-content-start">
            <div class="dx-field-value noFixedWidth">
                @(Html.DevExtreme.CheckBox() _
                .ID("ckSendEmail").Text(labels("Genius#ckSendEmail")).Value(False)
                )
            </div>
            <div class="conceptsInfoNew dx-field-value noFixedWidth" style="display: none; margin-left: 20px; ">
                @(Html.DevExtreme.CheckBox() _
                .ID("ckIncludeConceptsWithZerosNew").Text(labels("Genius#ckIncludeConceptsWithZeros")).Value(False)
                )
            </div>
            <div class="causesInfoNew dx-field-value noFixedWidth" style="display:none;margin-left:20px;">
                @(Html.DevExtreme.CheckBox() _
                .ID("ckIncludeCausesWithZerosNew").OnValueChanged("ckIncludeCausesWithZero_Changed").Text(labels("Genius#ckIncludeCausesWithZeros")).Value(False)
                )
            </div>
            <div class="causesInfoNew dx-field-value noFixedWidth" style="display:none;margin-left:20px;">
                @(Html.DevExtreme.CheckBox() _
                .ID("ckIncludeBusinessCenterWithZerosNew").Text(labels("Genius#ckIncludeEntriesWithoutBusinessCenter")).Value(True)
                )
            </div>
            <div style="margin-left: 20px;">
                @(Html.DevExtreme().Button() _
                .ID("newCustomView") _
                .Icon("video") _
                .Type(ButtonType.Default) _
                .Text(labels("Genius#RunGenius")) _
                .OnClick("function() { saveCustomView(); }"))
            </div>
        </div>
        <div style="min-height:15px"></div>
    </div>
</section>