@Imports Robotics.Base.DTOs

@ModelType roGeniusView
@Code
    Dim labels = ViewData(VTLive40.Helpers.Constants.DefaultLanguagesEntries)
End Code
<div class="aParent" style="float:right; margin-top:1px;margin-bottom:20px;width:21%;margin-right:10px">
    <div id="zoneSave" style="display: inline-block;margin-left:10px; float:right;">
        @(Html.DevExtreme().Button() _
                                                                                                                                          .ID("btnAddConceptsInfo") _
                                                                                                                                          .Icon("todo") _
                .OnClick("saveConceptsInfo") _
                                                                                                                                          .Type(ButtonType.Default) _
            )
    </div>
    <div id="zoneCancel" style="display: inline-block; margin-left: 10px;float:right;">
        @(Html.DevExtreme().Button() _
                                                                                  .ID("btnCancelConceptsInfo") _
                                                                                  .Icon("close") _
                .OnClick("cancelConceptsInfo") _
                                                                                  .Type(ButtonType.Danger) _
            )
    </div>
</div>
<div class="panHeader4" style="float:left;width:75%;padding: 10px;">
    <span class="panelTitleSpan">
        <span id="">@Html.Raw(labels("Genius#conceptsLbl"))</span>
    </span>
</div>
<div style="min-height:15px"></div>
<div style="width: 95%;margin: 0 auto;float:left;max-height:345px;overflow-y:auto;border:solid;border-color:grey;border-width:thin;">
    <div style="display: flex;">

        @Html.DevExpress().CheckBoxList(Sub(settings)
                                            settings.Name = "lstConcepts"
                                            settings.Properties.ValueField = "ID"
                                            settings.Properties.TextField = "Name"
                                            settings.ControlStyle.CssClass = "chkbxLstExtraInfo"
                                            settings.Properties.CheckBoxStyle.CssClass = "chkExtraInfo"
                                        End Sub).BindList(ViewBag.TbConcepts).GetHtml()
    </div>
</div>