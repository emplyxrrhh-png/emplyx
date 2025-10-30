@imports Robotics.Web.Base.API
@imports Robotics.Web.Base.HelperSession
@imports Robotics.VTBase.roTypes
@Code
    Layout = "~/Views/Shared/_layout.vbhtml"
    ViewData("Title") = "Communique"
    Dim CommuniqueController As VTLive40.CommuniqueController = New VTLive40.CommuniqueController()
    Dim reportManagerPermissionsByUser = SecurityServiceMethods.GetPermissionOverFeature(Nothing, "Communique", "U")
    Dim scriptVersion As String = CommuniqueController.ScriptsVersion
    Dim baseURL = Url.Content("~")
    Dim labels = ViewData(VTLive40.Helpers.Constants.DefaultLanguagesEntries)
    Dim maxFileSize As Integer = Any2Integer(AdvancedParametersCache("VTLive.MaxAllowedFileSize"))
    If maxFileSize = 0 Then maxFileSize = 256

    ViewData("SelectorFeature") = "Employees"
End Code

<script>
    var BASE_URL = "@baseURL";
    var chekPage_URL = "@Html.Raw(ViewBag.checkURL)";
    var jsLabels = JSON.parse('@Html.Raw(Robotics.VTBase.roJSONHelper.SerializeNewtonSoft(labels)) ');
    var MAX_FILE_SIZE = @maxFileSize;

</script>
<script>var msg1 = "@CommuniqueController.GetServerLanguage().Translate("roemployeeplaceholderlarge", "EmployeeSelector")";</script>
<script>var msg2 = "@CommuniqueController.GetServerLanguage().Translate("roemployeeplaceholder", "EmployeeSelector")";</script>
@*EDITIONS2022*@
<script>var AdvancedCommuniques = "@Html.Raw(ViewBag.AdvancedCommuniques)";</script>

<script src="@Url.Content("~/Scripts/jszip.min.js")" type="text/javascript"></script>

<link href="@Url.Content("~/Base/Styles/Live/Communique/roCommunique.min.css")@Html.Raw(scriptVersion)" rel="stylesheet" type="text/css" />
<script src="@Url.Content("~/Base/Scripts/Live/utils.min.js")@Html.Raw(scriptVersion)" type="text/javascript"></script>
<script src="@Url.Content("~/Base/Scripts/Live/Communique/roCommuniqueScript.min.js")@Html.Raw(scriptVersion)" type="text/javascript"></script>

<script src="@Url.Content("~/Scripts/exceljs.min.js")@Html.Raw(scriptVersion)" type="text/javascript"></script>
<script src="@Url.Content("~/Scripts/FileSaver.min.js")@Html.Raw(scriptVersion)" type="text/javascript"></script>

@Html.Partial("dataTabInfo")
<div id="divMainBody">
    <!-- TAB SUPERIOR -->
    <div id="divTabData" Class="divDataCells">
        <input type='hidden' id='hdnEmployees' runat='server' value='' />
        <input type="hidden" id="hdnFilter" runat="server" value='' />
        <input type="hidden" id="hdnFilterUser" runat="server" value='' />
        <div id="divContenido" Class="divAllContent twoWindowsFlexLayout">
            <div class="twoWindowsSidePanel maxHeight treeSize" id="divTree">
                <div class="treeCaption"><span>@CommuniqueController.GetServerLanguage().Translate("roComunicados", "Communique")</span></div>
                @Html.Partial("searchPanelCommunique")
                @Html.Partial("cardView", Model)
            </div>
            <div id="divButtons" class="divMiddleButtons">
                @Html.Partial("dataTabBarButtons")
            </div>
            <div id="divContenido" class="twoWindowsMainPanel maxHeight divRightContent" style="height:100%;">
                <div>
                    @Html.Partial("communiqueeBar")
                    @Html.Partial("saveOrDiscartChanges")
                </div>
                <div id="mainPanelDisplay" style="height:100%;overflow:hidden">
                    @Html.Partial("config")
                    <div id="fieldsEditor" style="display:none"></div>
                </div>
            </div>
        </div>
    </div>
</div>

@Code

    Html.DevExtreme().Popup() _
.ID("addImagePopup") _
.Width(500) _
.Height(330) _
.ShowTitle(True) _
.Title("Añadir imagen") _
.ShowCloseButton("False") _
.DragEnabled(False) _
.HideOnOutsideClick(True) _
.OnShown("addImageShown") _
.ContentTemplate(Sub()
@<text>
    @Html.Partial("SelectorImage")
</text>
End Sub) _
                                                               .Render()
End Code