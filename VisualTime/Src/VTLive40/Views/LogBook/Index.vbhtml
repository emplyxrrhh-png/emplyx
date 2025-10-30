@imports Robotics.Web.Base.API

@Code
    Layout = "~/Views/Shared/_layout.vbhtml"

    Dim scriptVersion As String = ViewData(VTLive40.Helpers.Constants.ScriptVersion)
    Dim baseURL = Url.Content("~")
    Dim labels = ViewData(VTLive40.Helpers.Constants.DefaultLanguagesEntries)
End Code

<script>
    var BASE_URL = "@baseURL";
    var hasMessages = false;        
</script>
<!-- Crear los scripts correspondientes para supervisors -->
<link href="@Url.Content("~/Content/bootstrap.min.css")" rel="stylesheet" type="text/css" />
<link href="@Url.Content("~/Base/Styles/Live/liveMVC.min.css")@Html.Raw(scriptVersion)" rel="stylesheet" type="text/css" />
<link href="@Url.Content("~/Base/Styles/roLiveStyles.min.css")@Html.Raw(scriptVersion)" rel="stylesheet" type="text/css" />
<link href="@Url.Content("~/Base/Styles/Live/LogBook/roLogBook.min.css")@Html.Raw(scriptVersion)" rel="stylesheet" type="text/css" />

<script src="@Url.Content("~/Scripts/jszip.min.js")@Html.Raw(scriptVersion)" type="text/javascript"></script>
<script src="@Url.Content("~/Base/Scripts/Live/LogBook/roLogBook.min.js")@Html.Raw(scriptVersion)" type="text/javascript"></script>
<div style="display:flex;">
    <div style="border-left: 1vw solid white;border-top: 1vw solid white; width:100%;border-right: 1vw solid white;">
        <div class="panHeader5" style="display: flex;align-items: center;">
            <div style="margin-right:0.5vw"><img src="~/Base/Images/StartMenuIcos/LogBook.png" width="48" height="48" /></div>
            <div>
                <div style="font-size:20px;width:100%">@Html.Raw(labels("Conversations#roLogBookTitle"))</div>
                <div><span id="readOnlyDescritionCompany" style="font-size: 11px;font-weight: 100;">@Html.Raw(labels("Conversations#roLogBookInfo"))</span></div>
            </div>
        </div>
    </div>
</div>
<br />
<div id="divMainBody">
    <div id="divTabData" Class="divDataCells">

        <div id="divContenido" Class="divAllContent twoWindowsFlexLayout">
            <div class="form ro-tab-section container" style="display: flex;">

                <div style="flex-grow: 1;" id="section2">
                    <div class="list-containerRequests">
                        <br />
                        <div id="searchContent">
                            <div style="width: 95%;margin: 0 auto;">
                                <div class="configField" style="display: flex;justify-content: center;">
                                    <div class="dx-field-label" style="width:20%">
                                        @Html.Raw(labels("Conversations#roComplaintId"))
                                    </div>
                                    <div class="dx-field-value" style="width:70%">
                                        <div style="width:65%;float:left">
                                            @(Html.DevExtreme().TextBox() _
.ID("ComplaintId") _
.MaxLength(250) _
.OnValueChanged("OnComplaintIdChange") _
.ValueChangeEvent("keyup") _
.OnEnterKey("showComplaintLogBook") _
.Placeholder(labels("Conversations#roComplaintIdDesc").ToString()))
                                        </div>
                                        <div style="float:left;margin-left:10px">
                                            @(Html.DevExtreme().Button() _
.ID("showComplaintLogBook") _
.OnClick("showComplaintLogBook") _
.Text(labels("Conversations#roShowComplaint")) _
.Type(ButtonType.Default) _
.Disabled(True)
            )
                                        </div>
                                        <div class="printLogBook">
                                            @(Html.DevExtreme().Button() _
                    .ID("printLogBook") _
                    .Icon("print") _
                    .OnClick("printLogBook") _
                    .Visible(False) _
                    .Text(labels("Conversations#roPrintComplaint")) _
                    .Type(ButtonType.Default)
            )
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div style="width: 95%;margin: 0 auto;">
                            <div id="divConversationContent" class="list-containerRequests">
                            </div>
                        </div>
                    </div>