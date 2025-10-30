@Imports Robotics.Base.DTOs
@Imports Robotics.Web.Base.roLanguageWeb

@ModelType roPortalConfiguration

@Code
    Layout = "~/Views/Shared/_layout.vbhtml"
    Dim oSerializer As System.Web.Script.Serialization.JavaScriptSerializer
    oSerializer = New System.Web.Script.Serialization.JavaScriptSerializer()
    Dim PortalConfigurationController As VTLive40.PortalConfigurationController = New VTLive40.PortalConfigurationController()
    Dim baseURL = Url.Content("~")
    Dim bIpStatus As Boolean = ViewBag.ZoneRestrictedByIP
End Code

<script>
var RootUrl = "@Html.Raw(ViewBag.RootUrl)";
    var Permission = "@Html.Raw(ViewBag.PermissionOverEmployees)";
    var BASE_URL = "@baseURL";
</script>

<link href="@Url.Content("~/Base/Styles/roStart.min.css")" rel="stylesheet" type="text/css" />
<link href="@Url.Content("~/Base/Styles/roLiveStyles.min.css")" rel="stylesheet" type="text/css" />
<script src="@Url.Content("~/Base/Scripts/Ajax.js")" type="text/javascript"></script>
<script src="@Url.Content("~/Base/Scripts/Live/PortalConfiguration/roPortalConfiguration.min.js")" type="text/javascript"></script>

<div id="news" style="float:left"></div>
<div id="newsOk" style="float:right; margin-top:60px;"></div>

<div style="display:flex;">
    <div style="border-left: 1vw solid white;border-top: 1vw solid white; width:100%;border-right: 1vw solid white;">
        <div class="panHeader5" style="display: flex;align-items: center;">
            <div style="margin-right:0.5vw"><img src="~/Base/Images/StartMenuIcos/Portal96.png" width="48" height="48" /></div>
            <div style="width:90%">
                <div style="font-size:20px;width:100%">@PortalConfigurationController.GetServerLanguage().Translate("roPortalConfigurationTitle", "PortalConfiguration")</div>
                <div><span id="readOnlyDescritionCompany" style="font-size: 11px;font-weight: 100;">@PortalConfigurationController.GetServerLanguage().Translate("roPortalConfigurationInfo", "PortalConfiguration")</span></div>
            </div>
            <div class="blackRibbonButtons" style="width:450px;">

                <div style="float: right; width: 170px;">
                    <div id="timegateConfiguration" class="bTabPortalConfigurationMenu" style="cursor: pointer;">
                        @PortalConfigurationController.GetServerLanguage().Translate("roTimegateTitle", "PortalConfiguration")
                    </div>
                    <div id="portalGeolocalizationConfiguration" class="bTabPortalConfigurationMenu" style="cursor: pointer;">
                        @PortalConfigurationController.GetServerLanguage().Translate("roPortalGeolocalizationConfigurationTitle", "PortalConfiguration")
                    </div>
                    @If (ViewBag.DailyRecord.ToString.ToUpper = "TRUE") Then
                        @<div id="portalDRConfiguration" Class="bTabPortalConfigurationMenu" style="cursor: pointer;">
                            @PortalConfigurationController.GetServerLanguage().Translate("roPortalDRConfigurationTitle", "PortalConfiguration")
                        </div>
                    Else
                        @<div></div>
                    End if
                </div>
                <div style="float: right; width: 170px; padding-right:10px">
                    <div id="portalGeneralConfiguration" class="bTabPortalConfigurationMenu bTabPortalConfigurationMenu-active" style="cursor: pointer;">
                        @PortalConfigurationController.GetServerLanguage().Translate("roPortalGeneralConfiguration", "PortalConfiguration")
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>
<br />

<div id="divMainBody" style="min-height:unset !important; height:unset !important">
    <div class="configField" style="display: flex; justify-content: flex-end; margin-right:1vw">
        <div id="portalConfigurationDefault" Class="generalConfigurationDiv" style="display: inline-block;margin-left:10px;">
            @(Html.DevExtreme().Button() _
                                                                                                                          .ID("btnRestorePortalConfiguration") _
                                                                                                                          .Icon("undo") _
                                                                                                                          .Type(ButtonType.Normal) _
                                                                    .Text(PortalConfigurationController.GetServerLanguage().Translate("roPortalConfigurationDefault", "PortalConfiguration")) _
                                                                    .OnClick("restorePortalConfiguration")
            )
        </div>
        <div id="portalConfigurationSave" style="display: inline-block;margin-left:10px;">
            @(Html.DevExtreme().Button() _
                                                                                                                                          .ID("btnSavePortalConfiguration") _
                                                                                                                                          .Icon("todo") _
                                                                                                                                          .Type(ButtonType.Default) _
                                                                                                                                          .OnClick("saveApplicationChanges")
            )
        </div>
        <div id="surveyRefresh" style="display: inline-block;margin-left:10px;">
            <div id="portalConfigurationCancel" style=" margin-left: auto; margin-right: 0;">
                @(Html.DevExtreme().Button() _
                                                  .ID("btnCancelSurvey") _
                                                  .Icon("close") _
                                                  .OnClick("cancelPortalConfiguration") _
                                                  .Type(ButtonType.Danger)
            )
            </div>
        </div>
    </div>
    <br />
    <div Class="generalConfigurationDiv">
        <div Class="panHeader4" style="margin-left:1vw; margin-right:1vw">
            @PortalConfigurationController.GetServerLanguage().Translate("roPortalGeneralConfigurationHeader", "PortalConfiguration")
        </div>
        <br />
        <div id="GeneralOptionsDiv">
            <div class="list-containerRequests" style="height:90%;margin-left:1vw; margin-right:1vw">
                <br />
                <div style="width: 100%;height:100%;margin: 0 auto;">

                    <div class="configField" style="padding-left:25px;display: flex;justify-content: center;">
                        <div class="dx-field-label" style="overflow:inherit; width: 100%;">
                            @PortalConfigurationController.GetServerLanguage().Translate("roPortalActivateImpersonalizationLbl", "PortalConfiguration")
                            <div style="margin-left: 1em;display:inline;">
                                @(Html.DevExtreme().Switch() _
.ID("chkImpersonate") _
.Value(False) _
.OnValueChanged("onPortalChanges") _
.SwitchedOffText(PortalConfigurationController.GetServerLanguage().Translate("roPortalNo", "PortalConfiguration")) _
.SwitchedOnText(PortalConfigurationController.GetServerLanguage().Translate("roPortalYes", "PortalConfiguration")))
                            </div>
                            <div class="OptionPanelDescStyle" style="overflow:inherit;padding-left:0px;padding-top:10px;">
                                @PortalConfigurationController.GetServerLanguage().Translate("roPortalActivateImpersonalizationDesc", "PortalConfiguration")
                            </div>
                        </div>
                    </div>
                    <br />
                    <br />
                    <br />
                </div>
            </div>
        </div>
        <br />
        <div Class="panHeader4" style="margin-left:1vw; margin-right:1vw">
            @PortalConfigurationController.GetServerLanguage().Translate("roPortalConfigurationHeader", "PortalConfiguration")
        </div>
        <br />
        <div Class="rightPane">
            <div class="configField" style="display: flex;justify-content: center;">
                <div Class="widget-container flex-box">
                    <Span>@PortalConfigurationController.GetServerLanguage().Translate("roPortalConfigurationImage", "PortalConfiguration")</Span>

                    <div id="dropzone-external" Class="flex-box dx-theme-border-color">
                        <img id="dropzone-image" src="#" hidden alt="" onload="toggleImageVisible(true)" />
                        <div id="dropzone-text" Class="flex-box">
                            <Span>@PortalConfigurationController.GetServerLanguage().Translate("roPortalConfigurationDrag", "PortalConfiguration")</Span>
                            <Span>@PortalConfigurationController.GetServerLanguage().Translate("roPortalConfigurationClick", "PortalConfiguration")</Span>
                            <Span>@PortalConfigurationController.GetServerLanguage().Translate("roPortalConfigurationExtensions", "PortalConfiguration")<Span>.jpg, .jpeg, .gif, .png</Span>.</Span>
                        </div>
                        @(Html.DevExtreme().ProgressBar() _
                    .ID("upload-progress") _
                    .Min(0) _
                    .Max(100) _
                    .Width("30%") _
                    .ShowStatus(False) _
                    .Visible(False) )
                    </div>
                    @(Html.DevExtreme().FileUploader() _
                            .DialogTrigger("#dropzone-external") _
                            .DropZone("#dropzone-external") _
                            .ID("file-uploader") _
                            .Name("myFile") _
                            .Multiple(False) _
                            .AllowCanceling(True) _
                            .AllowedFileExtensions(New String() {".jpg", ".jpeg", ".gif", ".png"}) _
                            .UploadMode(FileUploadMode.Instantly) _
                            .UploadUrl(Url.Action("Upload", "FileUploader")) _
                            .Visible(False) _
                            .OnDropZoneEnter("fileUploader_dropZoneEnter") _
                            .OnDropZoneLeave("fileUploader_dropZoneLeave") _
                            .OnUploaded("fileUploader_uploaded") _
                            .OnProgress("fileUploader_progress") _
                            .OnUploadStarted("fileUploader_uploadStarted") )
                    @(Html.DevExtreme().Button() _
                        .ID("btnDeleteImage") _
                        .Icon("close") _
                        .Type(ButtonType.Normal) _
                        .Text(PortalConfigurationController.GetServerLanguage().Translate("roClearImage", "PortalConfiguration")) _
                        .OnClick("fileUploaderReset") )
                </div>
            </div>
            <br />
        </div>
        <div class="configField" style="display: flex; justify-content: center; flex-direction: column;">
            <div Class="dx-field-label">
                @PortalConfigurationController.GetServerLanguage().Translate("roPortalConfigurationPreview", "PortalConfiguration")
            </div>
            <div id="previewDiv" style="border-radius:5px;" Class="dx-field-value">
                <div id="backgroundPhoto"></div>
            </div>

            <div id="configDiv" Class="leftPane" style="float:none;width: 425px;">
                <div Class="list-containerRequests" style="height:90%;width:100%;float:left;">
                    <br />
                    <div style="width: 100%;height:100%;margin: 0 auto;">

                        <div class="configField" style="display: flex;justify-content: center;">
                            <div Class="dx-field-label">
                                @PortalConfigurationController.GetServerLanguage().Translate("roPortalConfigurationLeftColor", "PortalConfiguration")
                            </div>
                            <div Class="dx-field-value">
                                @(Html.DevExtreme().ColorBox() _
                    .ID("selectLeftColor") _
                    .ApplyButtonText(PortalConfigurationController.GetServerLanguage().Translate("PortalConfiguration.dxColorPicker.AcceptButton", "PortalConfiguration")) _
                    .CancelButtonText(PortalConfigurationController.GetServerLanguage().Translate("PortalConfiguration.dxColorPicker.CancelButton", "PortalConfiguration")) _
                    .ApplyValueMode(EditorApplyValueMode.Instantly) _
                    .OpenOnFieldClick("true") _
                    .OnValueChanged("onColorChangedLeft") )
                            </div>
                        </div>
                        <br />
                        <div class="configField" style="display: flex;justify-content: center;">
                            <div Class="dx-field-label">
                                @PortalConfigurationController.GetServerLanguage().Translate("roPortalConfigurationRightColor", "PortalConfiguration")
                            </div>
                            <div Class="dx-field-value">
                                @(Html.DevExtreme().ColorBox() _
                    .ID("selectRightColor") _
                    .ApplyButtonText(PortalConfigurationController.GetServerLanguage().Translate("PortalConfiguration.dxColorPicker.AcceptButton", "PortalConfiguration")) _
                    .CancelButtonText(PortalConfigurationController.GetServerLanguage().Translate("PortalConfiguration.dxColorPicker.CancelButton", "PortalConfiguration")) _
                    .ApplyValueMode(EditorApplyValueMode.Instantly) _
                    .OpenOnFieldClick("true") _
                    .OnValueChanged("onColorChangedRight") )
                            </div>
                        </div>
                        <br />
                        <div class="configField" style="display: none;justify-content: center;">
                            <div Class="dx-field-label">
                                @PortalConfigurationController.GetServerLanguage().Translate("roPortalConfigurationPosition", "PortalConfiguration")
                            </div>
                            <div Class="dx-field-value">
                                @(Html.DevExtreme.SelectBox() _
                            .ID("selectPositionPortalConfiguration") _
                            .OnValueChanged("changePosition") _
                            .DataSource(ViewBag.Positions) _
                            .ValueExpr("Id") _
                            .DisplayExpr("Name") _
                            .ShowSelectionControls(False) )
                            </div>
                        </div>
                        <div class="configField" style="display: flex;justify-content: center;">
                            <div Class="dx-field-label">
                                @PortalConfigurationController.GetServerLanguage().Translate("roPortalConfigurationOpacity", "PortalConfiguration")
                            </div>
                            <div Class="dx-field-value">
                                @(Html.DevExtreme().Slider() _
                            .ID("selectOpacityPortalConfiguration") _
                            .Min(0) _
                            .Max(10) _
                            .Value(10) _
                            .OnValueChanged("changeOpacity") _
                            .Tooltip(Sub(columns)
                                         columns.Enabled(True)
                                         columns.Position(VerticalEdge.Bottom)
                                         columns.ShowMode(SliderTooltipShowMode.Always)
                                     End Sub) )
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div Class="timegateConfDiv" style="display: none; margin-right: 1vw">
        <div Class="panHeader4" style="margin-left:1vw">
            @PortalConfigurationController.GetServerLanguage().Translate("roTimegateConfiguration", "PortalConfiguration")
        </div>
        <br />
        <div>


            <div class="list-containerRequests" style="height:90%;margin-left:1vw; margin-right:1vw">
                <br />
                <div style="width: 100%;height:100%;margin: 0 auto;">

                    <div id="timegateId" style="padding-left:25px;display: flex;justify-content: center;">
                        <div class="dx-field-label" style="overflow:inherit; width: 100%;">
                            <div style="margin-left: 1em; float: left; margin-top: -3px">
                                @(Html.DevExtreme().Switch() _
.ID("chkTimegateUsesUserField") _
.Value(False) _
.OnValueChanged("onTimegateIDChange") _
.SwitchedOffText(PortalConfigurationController.GetServerLanguage().Translate("roPortalNo", "PortalConfiguration")) _
.SwitchedOnText(PortalConfigurationController.GetServerLanguage().Translate("roPortalYes", "PortalConfiguration")))
                            </div>
                            <div style="margin-left: 1em;float:left">
                                @PortalConfigurationController.GetServerLanguage().Translate("roTimegateUsesCustomId", "PortalConfiguration")
                            </div>

                            <div style="margin-left: 1em; float: left;margin-top:-10px">
                                @(Html.DevExtreme.SelectBox() _
    .ID("timegateUserFieldId") _
    .Width(300) _
    .ValueExpr("FieldValue") _
    .DisplayExpr("FieldName") _
    .OnValueChanged("onTimegateChanges") _
    .ShowSelectionControls(False) _
    .DataSource(ViewBag.TimegateUserfields))
                            </div>

                            <div class="OptionPanelDescStyle" style="clear:both;overflow:inherit;padding-left:0px;padding-top:10px;">
                                @PortalConfigurationController.GetServerLanguage().Translate("roTimegateWarning1", "PortalConfiguration")
                            </div>
                            <div class="OptionPanelDescStyle" style="overflow:inherit;padding-left:0px;padding-top:10px;">
                                @PortalConfigurationController.GetServerLanguage().Translate("roTimegateWarning2", "PortalConfiguration")
                            </div>
                            <div class="OptionPanelDescStyle" style="overflow:inherit;padding-left:0px;padding-top:10px;">
                                @PortalConfigurationController.GetServerLanguage().Translate("roTimegateWarning3", "PortalConfiguration")
                            </div>
                        </div>
                    </div>
                </div>
            </div>

        </div>
        <br />
        <div Class="panHeader4" style="margin-left:1vw; margin-right:1vw">
            @PortalConfigurationController.GetServerLanguage().Translate("roTimeGateBackground", "PortalConfiguration")
        </div>
        <br />
        <div Class="rightPane">
            <div class="configField" style="display: flex;justify-content: center;">
                <div Class="widget-container flex-box">
                    <Span>@PortalConfigurationController.GetServerLanguage().Translate("roTimeGateImage", "PortalConfiguration")</Span>

                    <div id="dropzonetg-external" Class="flex-box dx-theme-border-color">
                        <img id="dropzonetg-image" src="#" hidden alt="" onload="toggleTGImageVisible(true)" />
                        <div id="dropzonetg-text" Class="flex-box">
                            <Span>@PortalConfigurationController.GetServerLanguage().Translate("roPortalConfigurationDrag", "PortalConfiguration")</Span>
                            <Span>@PortalConfigurationController.GetServerLanguage().Translate("roPortalConfigurationClick", "PortalConfiguration")</Span>
                            <Span>@PortalConfigurationController.GetServerLanguage().Translate("roPortalConfigurationExtensions", "PortalConfiguration")<Span>.jpg, .jpeg, .gif, .png</Span>.</Span>
                        </div>
                        @(Html.DevExtreme().ProgressBar() _
    .ID("uploadtg-progress") _
    .Min(0) _
    .Max(100) _
    .Width("30%") _
    .ShowStatus(False) _
    .Visible(False) )
                    </div>
                    @(Html.DevExtreme().FileUploader() _
                .DialogTrigger("#dropzonetg-external") _
                .DropZone("#dropzonetg-external") _
                .ID("file-uploadertg") _
                .Name("myFileTG") _
                .Multiple(False) _
                .AllowCanceling(True) _
                .AllowedFileExtensions(New String() {".jpg", ".jpeg", ".gif", ".png"}) _
                .UploadMode(FileUploadMode.Instantly) _
                .UploadUrl(Url.Action("UploadTG", "FileUploader")) _
                .Visible(False) _
                .OnDropZoneEnter("fileUploaderTG_dropZoneEnter") _
                .OnDropZoneLeave("fileUploaderTG_dropZoneLeave") _
                .OnUploaded("fileUploaderTG_uploaded") _
                .OnProgress("fileUploaderTG_progress") _
                .OnUploadStarted("fileUploaderTG_uploadStarted") )
                    @(Html.DevExtreme().Button() _
        .ID("btnDeleteTGImage") _
        .Icon("close") _
        .Type(ButtonType.Normal) _
        .Text(PortalConfigurationController.GetServerLanguage().Translate("roClearImage", "PortalConfiguration")) _
        .OnClick("fileUploaderTGReset") )
                </div>
            </div>
            <br />
        </div>
        <div class="configField" style="display: flex; justify-content: center; flex-direction: column;">
            <div Class="dx-field-label">
                @PortalConfigurationController.GetServerLanguage().Translate("roPortalConfigurationPreview", "PortalConfiguration")

            </div>
            <div id="previewTGDiv" style="border-radius:5px;" Class="dx-field-value">
                <div id="backgroundTGPhoto"></div>
            </div>

            <div id="configTGDiv" Class="leftPane" style="float:none;width: 425px;">
                <div Class="list-containerRequests" style="height:90%;width:100%;float:left;">
                    <br />
                    <div style="width: 100%;height:100%;margin: 0 auto;">

                        <div class="configField" style="display: flex;justify-content: center;">
                            <div Class="dx-field-label">
                                @PortalConfigurationController.GetServerLanguage().Translate("roPortalConfigurationLeftColor", "PortalConfiguration")
                            </div>
                            <div Class="dx-field-value">
                                @(Html.DevExtreme().ColorBox() _
.ID("selectTGLeftColor") _
.ApplyButtonText(PortalConfigurationController.GetServerLanguage().Translate("PortalConfiguration.dxColorPicker.AcceptButton", "PortalConfiguration")) _
.CancelButtonText(PortalConfigurationController.GetServerLanguage().Translate("PortalConfiguration.dxColorPicker.CancelButton", "PortalConfiguration")) _
.ApplyValueMode(EditorApplyValueMode.Instantly) _
.OpenOnFieldClick("true") _
.OnValueChanged("onTGColorChangedLeft") )
                            </div>
                        </div>
                        <br />
                        <div class="configField" style="display: flex;justify-content: center;">
                            <div Class="dx-field-label">
                                @PortalConfigurationController.GetServerLanguage().Translate("roPortalConfigurationRightColor", "PortalConfiguration")
                            </div>
                            <div Class="dx-field-value">
                                @(Html.DevExtreme().ColorBox() _
.ID("selectTGRightColor") _
.ApplyButtonText(PortalConfigurationController.GetServerLanguage().Translate("PortalConfiguration.dxColorPicker.AcceptButton", "PortalConfiguration")) _
.CancelButtonText(PortalConfigurationController.GetServerLanguage().Translate("PortalConfiguration.dxColorPicker.CancelButton", "PortalConfiguration")) _
.ApplyValueMode(EditorApplyValueMode.Instantly) _
.OpenOnFieldClick("true") _
.OnValueChanged("onTGColorChangedRight") )
                            </div>
                        </div>
                        <br />
                        <div class="configField" style="display: none;justify-content: center;">
                            <div Class="dx-field-label">
                                @PortalConfigurationController.GetServerLanguage().Translate("roPortalConfigurationPosition", "PortalConfiguration")
                            </div>
                            <div Class="dx-field-value">
                                @(Html.DevExtreme.SelectBox() _
    .ID("selectPositionTimeGateBackground") _
    .OnValueChanged("changeTGPosition") _
    .DataSource(ViewBag.Positions) _
    .ValueExpr("Id") _
    .DisplayExpr("Name") _
    .ShowSelectionControls(False) )
                            </div>
                        </div>
                        <div class="configField" style="display: flex;justify-content: center;">
                            <div Class="dx-field-label">
                                @PortalConfigurationController.GetServerLanguage().Translate("roPortalConfigurationOpacity", "PortalConfiguration")
                            </div>
                            <div Class="dx-field-value">
                                @(Html.DevExtreme().Slider() _
.ID("selectOpacityTimegateBackground") _
.Min(0) _
.Max(10) _
.Value(10) _
.OnValueChanged("changeTimeGateOpacity") _
.Tooltip(Sub(columns)
             columns.Enabled(True)
             columns.Position(VerticalEdge.Bottom)
             columns.ShowMode(SliderTooltipShowMode.Always)
         End Sub) )
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div Class="geolocalizationConfigurationDiv" style="display: none; margin-right: 1vw">
        <div Class="panHeader4" style="margin-left:1vw">
            @PortalConfigurationController.GetServerLanguage().Translate("roportalgeolocalizationconfiguration", "PortalConfiguration")
        </div>
        <br />

        @If (bIpStatus) Then
            @<div Class="list-containerRequests" style="height:90%;width:100%;float:left;">
                <br />
                <div style="width: 100%;height:100%;margin: 0 auto;">
                    <div class="configField" style="padding-left:50px;display: flex;justify-content: center;">
                        <div class="dx-field-label" style="overflow:inherit;width:100%;">
                            <div>
                                <div style="float: left">
                                    @PortalConfigurationController.GetServerLanguage().Translate("roPortalConfigNotAllowed", "PortalConfiguration")
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        End If
        <div id="configGeolocalizationDiv">
            <div class="list-containerRequests" style="height:90%;width:100%;float:left;">
                <br />
                <div style="width: 100%;height:100%;margin: 0 auto;">

                    <div class="configField" style="padding-left:50px;display: flex;justify-content: center;">
                        <div class="dx-field-label" style="overflow:inherit;width:100%;">
                            <div>
                                <div style="float: left">
                                    @PortalConfigurationController.GetServerLanguage().Translate("roPortalGeolocalizationConfigurationDescription", "PortalConfiguration")
                                </div>
                                <div style="float: left; padding-left: 20px; margin-top: -10px;">
                                    @(Html.DevExtreme.SelectBox() _
                .ID("selectGeolocalization") _
                .Width(300) _
                .ValueExpr("Id") _
                .DisplayExpr("Name") _
                .OnValueChanged("onPortalChanges") _
                .Disabled(bIpStatus) _
                .ShowSelectionControls(False) _
                .DataSource(ViewBag.GeolocalizationTypes))
                                </div>
                            </div>


                            <div class="OptionPanelDescStyle" style="overflow:inherit;padding-left:0px;padding-top:10px;clear:both">
                                @PortalConfigurationController.GetServerLanguage().Translate("roPortalGeolocalizationDesc", "PortalConfiguration")
                            </div>
                        </div>
                    </div>
                    <br />
                    <br />
                    <br />
                </div>
            </div>
        </div>
        <br />

        <div id="`punchesOptionsDiv">
            <div class="list-containerRequests" style="height:90%;width:100%;float:left;">
                <br />
                <div style="width: 100%;height:100%;margin: 0 auto;">
                    <div class="configField" style="padding-left:50px;display: flex;justify-content: center;">
                        <div class="dx-field-label" style="overflow:inherit;width:100%;">
                            @PortalConfigurationController.GetServerLanguage().Translate("roPortalPunchZoneRequired", "PortalConfiguration")
                            <div style="margin-left: 1em;display:inline;">
                                @(Html.DevExtreme().Switch() _
.ID("chkPunchRequireZone") _
.Value(False) _
.Disabled(bIpStatus) _
.OnValueChanged("onPortalChanges") _
.SwitchedOffText(PortalConfigurationController.GetServerLanguage().Translate("roPortalNo", "PortalConfiguration")) _
.SwitchedOnText(PortalConfigurationController.GetServerLanguage().Translate("roPortalYes", "PortalConfiguration")))
                            </div>
                            <div class="OptionPanelDescStyle" style="overflow:inherit;padding-left:0px;padding-top:10px;">
                                @PortalConfigurationController.GetServerLanguage().Translate("roPortalPunchZoneRequiredDesc", "PortalConfiguration")
                            </div>
                        </div>
                    </div>
                    <br />
                    <br />
                    <br />
                </div>
            </div>
        </div>
    </div>

    @If (ViewBag.DailyRecord.ToString.ToUpper = "TRUE") Then
        @<div class="drConfigurationDiv" style="display: none; margin-right: 1vw">
            <input type="hidden" id="isDailyRecordEnabled" value="1" />
            <div class="panHeader4" style="margin-left:1vw">
                @PortalConfigurationController.GetServerLanguage().Translate("roPortalDRConfiguration", "PortalConfiguration")
            </div>
            <br />
            <div id="drOptionsDiv">
                <div class="list-containerRequests" style="height:90%;width:100%;float:left;">
                    <br />
                    <div style="width: 100%;height:100%;margin: 0 auto;">
                        <div class="configField" style="padding-left:50px;display: flex;justify-content: center;">
                            <div class="dx-field-label" style="overflow:inherit;width:100%;">
                                @PortalConfigurationController.GetServerLanguage().Translate("roPortalDR", "PortalConfiguration")
                                <div style="margin-left: 1em;display:inline;">
                                    @(Html.DevExtreme().Switch() _
.ID("chkDR") _
.Value(False) _
.OnValueChanged("onPortalChanges") _
.SwitchedOffText(PortalConfigurationController.GetServerLanguage().Translate("roPortalNo", "PortalConfiguration")) _
.SwitchedOnText(PortalConfigurationController.GetServerLanguage().Translate("roPortalYes", "PortalConfiguration")) _
                )
                                </div>
                                <div class="OptionPanelDescStyle" style="overflow:inherit;padding-left:0px;padding-top:10px;">
                                    @PortalConfigurationController.GetServerLanguage().Translate("roPortalDRDesc", "PortalConfiguration")
                                </div>
                            </div>
                        </div>

                        <div class="configField" style="padding-left:50px;display: flex;justify-content: center;">
                            <div class="dx-field-label" style="overflow:inherit;width:100%;">
                                @PortalConfigurationController.GetServerLanguage().Translate("LimitMaxDRDaysOnPast", "PortalConfiguration")
                                <div style="margin-left: 1em; display: inline-flex;">
                                    @(Html.DevExtreme().Switch() _
.ID("chkLimitDRDays") _
.Value(False) _
.OnValueChanged("onLimitDRDays") _
.SwitchedOffText(PortalConfigurationController.GetServerLanguage().Translate("roPortalNo", "PortalConfiguration")) _
.SwitchedOnText(PortalConfigurationController.GetServerLanguage().Translate("roPortalYes", "PortalConfiguration")) _
                                                )
                                </div>
                                <div class="OptionPanelDescStyle" style="overflow: inherit; padding-left: 0px; padding-top: 10px;">
                                    @PortalConfigurationController.GetServerLanguage().Translate("LimitMaxDRDaysOnPastDesc", "PortalConfiguration")
                                    <div style="margin-left: 1em; display: inline-flex;">
                                        @(Html.DevExtreme().NumberBox() _
.ID("drLimitDays") _
.ShowSpinButtons(True) _
.ShowClearButton(True) _
.Min(0) _
.Width(120) _
.OnValueChanged("drLimitDaysValueChanged") _
)
                                    </div>
                                    <div style="margin-left: 1em; display: inline-flex;">
                                        <!--@PortalConfigurationController.GetServerLanguage().Translate("LimitMaxDRDaysOnPastDesc", "PortalConfiguration")-->
                                    </div>
                                </div>
                            </div>
                        </div>
                        <br />
                        <br />
                        <br />
                    </div>
                </div>
            </div>
        </div>
    Else
        @<div><input type="hidden" id="isDailyRecordEnabled" value="0" /></div>
    End if
</div>
