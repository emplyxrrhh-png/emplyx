@Imports Robotics.Base.DTOs

@Code
    Dim labels = ViewData(VTLive40.Helpers.Constants.DefaultLanguagesEntries)
    Dim baseURL = Url.Content("~")
End Code

<style>
    #divSupervisors .dx-field-label, #divDestination .dx-field-label, .channelField .dx-field-label {
        width: 25% !important;
    }

    #divSupervisors .dx-field-value, #divDestination .dx-field-value, .channelField .dx-field-value {
        width: 75% !important;
    }
</style>

<div class="aParent" style="float:left; margin-bottom:20px;">
    <div id="ChannelConfiguration" class="bTabChannels" style="height:35px !important; cursor: pointer;">
        @Html.Raw(labels("Channels#roChannelConfiguration"))
    </div>
    <div id="ChannelConversations" class="bTabChannels" style="height: 35px !important; cursor: pointer;">
        @Html.Raw(labels("Channels#roChannelConversations"))
    </div>
</div>

<div class="aParent" style="float:right; margin-top:1px;">
    <div id="onlyEditButtons" style="display: inline-block;">
        <div id="ChannelSend" style="display: inline-block;">
            @(Html.DevExtreme().Button() _
                                    .ID("btnPublishChannel") _
                                    .Icon("chevrondoubleright") _
                                    .OnClick("publishChannel") _
                                    .Text(labels("Channels#roChannelPublished").ToString()) _
                                    .Type(ButtonType.Success) _
                )
        </div>
        <div id="ChannelRestrict" style="display: inline-block;">
            @(Html.DevExtreme().Button() _
                                        .ID("btnRestrictChannel") _
                                        .Icon("chevrondoubleleft") _
                                        .OnClick("restrictChannel") _
                                        .Text(labels("Channels#roRestrictChannel").ToString()) _
                                        .Type(ButtonType.Danger) _
            )
        </div>
        <div id="ChannelSave" style="display: inline-block;margin-left:10px;">
            @(Html.DevExtreme().Button() _
                                            .ID("addNewChannel") _
                                            .Icon("todo") _
                                            .OnClick("createNewChannel") _
                                            .Type(ButtonType.Default) _
                )
        </div>
        <div id="ChannelSaveComplaint" style="display: inline-block;margin-left:10px;">
            @(Html.DevExtreme().Button() _
                                                .ID("saveComplaintChannel") _
                                                .Icon("todo") _
                                                .OnClick("saveComplaintChannel") _
                                                .Type(ButtonType.Default) _
                )
        </div>
    </div>
    <div id="ChannelCancel" style="display: inline-block; margin-left: 10px;">
        @(Html.DevExtreme().Button() _
                                        .ID("btnCancelChannel") _
                                        .Icon("close") _
                                        .OnClick("cancelChannel") _
                                        .Type(ButtonType.Danger) _
            )
    </div>
</div>

<br />
<br />
<div id="divMainBody" style="min-height:unset !important; height:unset !important">
    <!-- TAB SUPERIOR -->
    <div id="divTabData">
        <div id="divContenido" Class="divAllContentChannels twoWindowsFlexLayout">

            <div id="divContenido" class="twoWindowsMainPanel maxHeight divRightContent">

                <div class="form ro-tab-section container" style="display: flex;">

                    <div class="sectionChannels" style="flex-grow: 1;" id="section2">

                        <!--DIV CONFIGURACION-->
                        <div id="configDiv" style="width: 100%; margin: 0 auto;">
                            <div class="panHeader4" style="padding: 10px;">
                                @Html.Raw(labels("Channels#roChannelGeneral"))
                            </div>

                            <div id="configChannels" class="list-containerRequests">
                                <br />
                                <div style="width: 95%;margin: 0 auto;">
                                    <div class="channelField" style="display: flex;justify-content: center;">
                                        <div class="dx-field-label">
                                            @Html.Raw(labels("Channels#roChannelName"))
                                        </div>
                                        <div class="dx-field-value">
                                            @(Html.DevExtreme().TextBox() _
.ID("ChannelName") _
.MaxLength(250) _
.Placeholder(labels("Channels#roChannelTitleDesc").ToString()) _
.OnValueChanged("onChannelChange"))
                                        </div>
                                    </div>
                                    <br />
                                    <br />
                                    <div id="divSupervisors" style="display: flex;justify-content: center;">
                                        <div class="dx-field-label">
                                            @Html.Raw(labels("Channels#roChannelSupervisors"))
                                        </div>
                                        <div class="dx-field-value">
                                            @(Html.DevExtreme().TagBox() _
.ID("ChannelSupervisorsText") _
.Placeholder(labels("Channels#roChannelSupervisorsDesc")) _
.ValueExpr("ID") _
.DisplayExpr("Name") _
.SearchEnabled(True) _
.ShowSelectionControls(True) _
.ApplyValueMode(EditorApplyValueMode.Instantly) _
.DataSource(ViewBag.Supervisors) _
.OnValueChanged("onChannelChange"))
                                        </div>
                                    </div>
                                    <br />
                                    <div id="divDestination" style="display: flex;justify-content: center;">
                                        <div class="dx-field-label">
                                            @Html.Raw(labels("Channels#roChannelDestination"))
                                        </div>
                                        <div class="dx-field-value">
                                            @(Html.DevExtreme().TextBox() _
.ID("ChannelDestination") _
.Placeholder(labels("Channels#roChannelDestinationDesc").ToString()) _
.OnValueChanged("onChannelChange"))
                                        </div>
                                    </div>
                                    <br />
                                    <div class="channelField" style="display: flex;justify-content: center;">

                                        <div Class="dx-field-label">

                                            @Html.Raw(labels("Channels#roChannelReceiptAknowledgment"))
                                        </div>
                                        <div Class="dx-field-value" style="display:flex">
                                            @(Html.DevExtreme().Switch() _
.ID("ChannelReceiptAknowledgment") _
.Value(False) _
.SwitchedOffText(labels("Channels#roChannelNo").ToString()) _
.SwitchedOnText(labels("Channels#roChannelYes").ToString()) _
.OnValueChanged("onChannelChange"))
                                        </div>
                                    </div>

                                    <br />
                                    <div class="channelField" style="display: flex;justify-content: center;">

                                        <div Class="dx-field-label">

                                            @Html.Raw(labels("Channels#roChannelAnonymous"))
                                        </div>
                                        <div Class="dx-field-value" style="display:flex">
                                            @(Html.DevExtreme().Switch() _
.ID("ChannelAnonymous") _
.Value(False) _
.SwitchedOffText(labels("Channels#roChannelNo").ToString()) _
.SwitchedOnText(labels("Channels#roChannelYes").ToString()) _
.OnValueChanged("onChannelChange"))
                                        </div>
                                    </div>
                                </div>
                                <br />
                                <br />

                                <br />
                            </div>
                            <div id="configComplaint" class="list-containerRequests">
                                <br />
                                <div style="width: 95%;margin: 0 auto;">
                                    <div id="resumeStatusComplaint">
                                        <div class="dx-field-label">
                                            Política de privacidad
                                        </div>
                                        <div class="dx-field-value">
                                            @(Html.DevExtreme().HtmlEditor() _
                .ID("ChannelPolicy") _
                .Height(600))
                                        </div>
                                    </div>
                                </div>
                                <br />
                                <br />

                                <br />
                            </div>
                        </div>

                        <!--DIV CONVERSATIONS-->
                        <div id="ChannelDiv" class="channelContentDiv">
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    @Code
        Html.DevExtreme().Popup() _
.ID("validatePublish") _
.Width(New JS("getValidationWidth")) _
.Height(New JS("getValidationHeight")) _
.ShowTitle(False) _
.Title("") _
.DragEnabled(False) _
.HideOnOutsideClick(True) _
.OnShown("onValidateShown") _
.ContentTemplate(Sub()
@<text>
    @Html.Partial("ValidateComplaint")
</text>
End Sub) _
.Render()
    End Code
</div>

<Script>
</Script>