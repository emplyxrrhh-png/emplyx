@Code
    Dim oSerializer As System.Web.Script.Serialization.JavaScriptSerializer
    oSerializer = New System.Web.Script.Serialization.JavaScriptSerializer()
    Dim CommuniqueController As VTLive40.CommuniqueController = New VTLive40.CommuniqueController()
End Code

<div class="dx-formdialog-form dx-form dx-widget dx-visibility-change-handler" role="form">
    <div class="dx-layout-manager dx-widget">
        <div class="dx-widget dx-collection dx-responsivebox-screen-lg dx-responsivebox" style="width: 100%; height: 100%;">
            <div class="dx-box-flex dx-box dx-widget dx-visibility-change-handler dx-collection" style="display: flex; flex-direction: column; width: 100%; height: 100%; justify-content: flex-start; align-items: stretch;">
                <div class="dx-item dx-box-item" style="display: flex; min-height: auto; flex-grow: 1; flex-shrink: 1;">
                    <div class="dx-item-content dx-box-item-content dx-root-simple-item" style="width: auto; height: auto; display: flex; flex-basis: 0px; flex-grow: 1; flex-direction: column;">
                        <div class="dx-first-row dx-first-col dx-last-col dx-field-item dx-col-0 dx-field-item-optional dx-flex-layout dx-label-h-align">
                            <label class="dx-field-item-label dx-field-item-label-location-left" for="dx_dx-6299ba44-cd3d-29da-0b18-dc369b7e86d7_src">
                                <span class="dx-field-item-label-content" style="width: 100px;">
                                    <span class="dx-field-item-label-text">@Html.Raw(CommuniqueController.GetServerLanguage().Translate("roImageURL", "Communique"))</span>
                                </span>
                            </label>
                            <div class="dx-field-item-content dx-field-item-content-location-right">
                                @(Html.DevExtreme().TextBox() _
.ID("txtImageURL") _
                    )
                            </div>
                        </div>
                    </div>
                </div>
                <div class="dx-item dx-box-item" style="display: flex; min-height: auto; flex-grow: 1; flex-shrink: 1;">
                    <div class="dx-item-content dx-box-item-content dx-root-simple-item" style="width: auto; height: auto; display: flex; flex-basis: 0px; flex-grow: 1; flex-direction: column;">
                        <div class="dx-first-col dx-last-col dx-field-item dx-col-0 dx-field-item-optional dx-flex-layout dx-label-h-align">
                            <label class="dx-field-item-label dx-field-item-label-location-left" for="dx_dx-6299ba44-cd3d-29da-0b18-dc369b7e86d7_width">
                                <span class="dx-field-item-label-content" style="width: 100px;">
                                    <span class="dx-field-item-label-text">@Html.Raw(CommuniqueController.GetServerLanguage().Translate("roImageWidth", "Communique"))</span>
                                </span>
                            </label>
                            <div class="dx-field-item-content dx-field-item-content-location-right">
                                @(Html.DevExtreme().TextBox() _
.ID("txtWidthImage") _
                    )
                            </div>
                        </div>
                    </div>
                </div>
                <div class="dx-item dx-box-item" style="display: flex; min-height: auto; flex-grow: 1; flex-shrink: 1;">
                    <div class="dx-item-content dx-box-item-content dx-root-simple-item" style="width: auto; height: auto; display: flex; flex-basis: 0px; flex-grow: 1; flex-direction: column;">
                        <div class="dx-first-col dx-last-col dx-field-item dx-col-0 dx-field-item-optional dx-flex-layout dx-label-h-align">
                            <label class="dx-field-item-label dx-field-item-label-location-left" for="dx_dx-6299ba44-cd3d-29da-0b18-dc369b7e86d7_width">
                                <span class="dx-field-item-label-content" style="width: 100px;">
                                    <span class="dx-field-item-label-text">@Html.Raw(CommuniqueController.GetServerLanguage().Translate("roImageHeight", "Communique"))</span>
                                </span>
                            </label>
                            <div class="dx-field-item-content dx-field-item-content-location-right">
                                @(Html.DevExtreme().TextBox() _
.ID("txtHeightImage") _
                    )
                            </div>
                        </div>
                    </div>
                </div>
                <div class="dx-item dx-box-item" style="display: flex; min-height: auto; flex-grow: 1; flex-shrink: 1;">
                    <div class="dx-item-content dx-box-item-content dx-root-simple-item" style="width: auto; height: auto; display: flex; flex-basis: 0px; flex-grow: 1; flex-direction: column;">
                        <div class="dx-first-col dx-last-col dx-field-item dx-col-0 dx-field-item-optional dx-flex-layout dx-label-h-align">
                            <label class="dx-field-item-label dx-field-item-label-location-left" for="dx_dx-6299ba44-cd3d-29da-0b18-dc369b7e86d7_width">
                                <span class="dx-field-item-label-content" style="width: 100px;">
                                    <span class="dx-field-item-label-text">@Html.Raw(CommuniqueController.GetServerLanguage().Translate("roImageAlt", "Communique"))</span>
                                </span>
                            </label>
                            <div class="dx-field-item-content dx-field-item-content-location-right">
                                @(Html.DevExtreme().TextBox() _
.ID("txtAltImage") _
                    )
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>
<div class="dx-toolbar dx-widget dx-visibility-change-handler dx-collection dx-popup-bottom" role="toolbar">
    <div class="dx-toolbar-items-container">
        <div class="dx-toolbar-before"></div><div class="dx-toolbar-center" style="margin: 0px 225px 0px 0px; float: right;"></div><div class="dx-toolbar-after">
            <div class="dx-item dx-toolbar-item dx-toolbar-button">
                <div class="dx-item-content dx-toolbar-item-content">
                    @(Html.DevExtreme().Button() _
                                                                                        .ID("btnAddImage") _
                                                                                        .Text(CommuniqueController.GetServerLanguage().Translate("roAcceptsUploadImage", "Communique")) _
                                                                .OnClick("addImage")
    )
                </div>
            </div>
            <div class="dx-item dx-toolbar-item dx-toolbar-button">
                @(Html.DevExtreme().Button() _
.ID("btnCancelAddImage") _
.Text(CommuniqueController.GetServerLanguage().Translate("roCancelUploadImage", "Communique")) _
.OnClick("cancelImage")
    )
            </div>
        </div>
    </div>
</div>