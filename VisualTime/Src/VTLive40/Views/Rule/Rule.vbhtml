@Code
    Dim scriptVersion As String = ViewData(VTLive40.Helpers.Constants.ScriptVersion)
End Code

<div id="rulesGroupBody">
    <div style="display:flex;flex-direction:column;gap: 10px">
        @Html.Partial("_stateBarIcons", New With {.Section = "Rule", .ShowClose = True, .ShowUndo = True, .ShowDelete = True, .ShowSave = True, .ShowVisualize = False})
    </div>
    @Html.Partial("~/Views/Rule/_RuleGeneralSection.vbhtml")
    @Html.Partial("~/Views/Rule/_RuleHistorySection.vbhtml")
</div>


@(Html.DevExtreme().Button() _
                    .ID("btnLinkEditRule") _
                    .Text("Simular cambios") _
                    .OnClick("openDailyRule") _
                    .Visible(False) _
                    .Type(ButtonType.Success) _
 )


@Code

    Html.DevExtreme().Popup() _
.ID("editDailyRuleFrm") _
.Width(1260) _
.Height(860) _
.ShowCloseButton(False) _
.FullScreen(False) _
.ShowTitle(False) _
.DragEnabled(False) _
.OnShown("function(s,e) { window.RuleEditor.Events.onLoad(s,e); }") _
.HideOnOutsideClick(False) _
.ContentTemplate(Sub()
@<text>
    @Html.Partial("~/Views/Rule/DailyRuleWrapper.vbhtml")
</text>
                         End Sub).Render()
End Code



