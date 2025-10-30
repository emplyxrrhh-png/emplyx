@Imports Robotics.Base.DTOs

@Code
    Layout = "~/Views/Shared/_layout.vbhtml"

    Dim labels = ViewData(VTLive40.Helpers.Constants.DefaultLanguagesEntries)
    Dim baseURL = Url.Content("~")
    Dim scriptVersion As String = ViewData(VTLive40.Helpers.Constants.ScriptVersion)
    Dim operationValue As Boolean = If(ViewBag.unionType = "custom" OrElse ViewBag.unionType = "or", False, True)
    Dim allowSelectAll As Boolean = ViewBag.allowSelectAll
    Dim allowSelectNone As Boolean = ViewBag.allowSelectNone
    ViewBag.ClientWidth = "66vw"
End Code



<script>
    if (!document.querySelector('script[src*="roTreeState.js"]')) {
        var script = document.createElement('script');
        script.src = '@Url.Content("~/Base/Scripts/rocontrols/roTrees/roTreeState.js")@Html.Raw(scriptVersion)';
        script.type = 'text/javascript';
        document.head.appendChild(script);
    }
</script>

<link href="@Url.Content("~/Base/Styles/roStart.min.css")@Html.Raw(scriptVersion)" rel="stylesheet" type="text/css" />
<link href="@Url.Content("~/Base/Styles/roLiveStyles.min.css")@Html.Raw(scriptVersion)" rel="stylesheet" type="text/css" />

<style>
    .dx-overlay-shader {
        background-color: transparent !important;
    }

    .dx-field-label {
        width: 100px !important;
    }

    .dx-field-value {
        width: calc(100vw - 220px) !important;
    }

    .dx-tagbox:not(.dx-tagbox-single-line) .dx-texteditor-input-container {
        max-height: 70px;
        overflow-y: auto;
    }
</style>

<script>
    var BASE_URL = "@baseURL";
    var chekPage_URL = "@Html.Raw(ViewBag.checkURL)";
    jsLabels = JSON.parse('@Html.Raw(Robotics.VTBase.roJSONHelper.SerializeNewtonSoft(labels).Replace("'", "\'")) ');
    selectAllEnabled = @Html.Raw(allowSelectAll.ToString().ToLower());
    selectNoneEnabled = @Html.Raw(allowSelectNone.ToString().ToLower());
    partialMode = false;
    operationValue = @Html.Raw(operationValue.ToString().ToLower());
    pageName = 'empMVCSelector';

    const cat = localStorage.getItem(pageName);
    var currentView = { Employees: [], Groups: [], Collectives: [], LabAgrees: [], Operation: "or", Filter: "11110", UserFields: "", ComposeMode: (selectAllEnabled ? "All" : "Custom"), ComposeFilter: "", Advanced: false };
    if (cat != null) {
        tmpOptions = JSON.parse(cat);
        currentView = tmpOptions.view;
        currentView.Advanced = false;
    }

    if (typeof currentView["ComposeMode"] == 'undefined') currentView["ComposeMode"] = selectAllEnabled ? "All" : "Custom";
    if (typeof currentView["Operation"] == 'undefined') currentView["Operation"] = "or";
    if (typeof currentView["Collectives"] == 'undefined') currentView["Collectives"] = [];
    if (typeof currentView["LabAgrees"] == 'undefined') currentView["LabAgrees"] = [];
</script>

<input type='hidden' id='hdnMVCEmployees' runat='server' value='' />
<input type="hidden" id="hdnMVCFilter" runat="server" value='' />
<input type="hidden" id="hdnMVCFilterUser" runat="server" value='' />

@Code
    Html.DevExtreme().Popup() _
.ID("desinationPopup") _
.Width(890) _
.Height(310) _
.ShowTitle(False) _
.Title("Seleccionar desinatarios") _
.DragEnabled(False) _
.OnShown("plainSelectorEmployeeShown") _
.HideOnOutsideClick(True) _
.OnHiding("selectorOnHiding") _
.ContentTemplate(Sub()
@<text>
    @Html.Partial("_PlainSelectorEmployees")
</text>
End Sub).Render()
End Code

@Code
    Html.DevExtreme().Popup() _
.ID("desinationPopupAdvanced") _
.Width(690) _
.Height(610) _
.ShowTitle(False) _
.Title("Seleccionar desinatarios") _
.DragEnabled(False) _
.OnShown("advSelectorEmployeeShown") _
.HideOnOutsideClick(True) _
.OnHiding("selectorOnHiding") _
.ContentTemplate(Sub()
@<text>
    @Html.Partial("_AdvSelectorEmployees")
</text>
End Sub).Render()
End Code