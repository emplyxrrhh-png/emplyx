@Imports Robotics.Base.DTOs

@ModelType roGeniusView
@Code
    Dim labels = ViewData(VTLive40.Helpers.Constants.DefaultLanguagesEntries)
    Dim selectOptions As SelectField() = {New SelectField With {.FieldValue = 0, .FieldName = labels("Genius#ckIncludeAllCauses")},
                    New SelectField With {.FieldValue = 1, .FieldName = labels("Genius#ckSelectCauses")}}
End Code

<div class="aParent" style="float:right; margin-top:1px;margin-bottom:20px;width:21%;margin-right:10px">
    <div id="zoneSave" style="display: inline-block;margin-left:10px; float:right;">
        @(Html.DevExtreme().Button() _
                            .ID("btnAddCausesInfo") _
                            .Icon("todo") _
                            .OnClick("saveCausesInfo") _
                            .Type(ButtonType.Default) _
            )
    </div>
    <div id="zoneCancel" style="display: inline-block; margin-left: 10px;float:right;">
        @(Html.DevExtreme().Button() _
                            .ID("btnCancelCausesInfo") _
                            .Icon("close") _
                            .OnClick("cancelCausesInfo") _
                            .Type(ButtonType.Danger) _
            )
    </div>
</div>
<div class="panHeader4" style="float:left;width:75%;padding: 10px;">
    <span class="panelTitleSpan">
        <span id="">@Html.Raw(labels("Genius#causesLbl"))</span>
    </span>
</div>
<div style="min-height:15px"></div>
<div style="width: 95%;margin: 0 auto;float:left;max-height:345px">

    <div style="padding-left:10px">
        @(Html.DevExtreme().RadioGroup() _
                        .ID("ckIncludeAllCauses") _
                        .Name("ckIncludeAllCauses") _
                        .DataSource(selectOptions) _
                        .ValueExpr("FieldValue") _
                        .DisplayExpr("FieldName") _
                        .Value("0") _
                        .OnValueChanged("ckIncludeAllCauses_OnChange")            )
    </div>
    <div class="scrollSection" style="padding-left: 35px;height:250px; overflow-y: auto">
        @Html.DevExpress().CheckBoxList(Sub(settings)
                                            settings.ClientEnabled = False
                                            settings.Name = "lstCauses"
                                            settings.Properties.ValueField = "ID"
                                            settings.Properties.TextField = "Name"
                                            settings.Style("border") = "none"
                                        End Sub).BindList(ViewBag.tbCauses).GetHtml()
    </div>
</div>