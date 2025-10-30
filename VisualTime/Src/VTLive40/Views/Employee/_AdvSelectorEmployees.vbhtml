@Code
    Dim labels = ViewData(VTLive40.Helpers.Constants.DefaultLanguagesEntries)
    Dim baseURL = Url.Content("~")
    Dim scriptVersion As String = ViewData(VTLive40.Helpers.Constants.ScriptVersion)
End Code
<div class="panHeader4" style="padding: 10px;">
    @Html.Raw(labels("EmployeeSelector#roDestination"))
</div>
<br />
<div id="mainSelector">
    @Html.Partial("_TreeSelector")
</div>
<div style="margin: 15px;position: absolute; bottom: 0;right: 0;">
    @(Html.DevExtreme().Button() _
                                    .ID("saveAdvancedFilter") _
                                    .Icon("todo") _
                                    .OnClick("backToSimpleSelector") _
                                    .Type(ButtonType.Default))
</div>