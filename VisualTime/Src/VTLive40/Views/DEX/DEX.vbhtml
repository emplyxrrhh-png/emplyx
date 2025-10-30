@Imports Robotics.Base.DTOs
@Imports Robotics.Web.Base.roLanguageWeb

@ModelType roChannel

@Code
    Layout = "~/Views/Shared/_layoutBS.vbhtml"
    Dim baseURL = Url.Content("~")
    Dim labels = ViewData(VTLive40.Helpers.Constants.DefaultLanguagesEntries)
    Dim scriptVersion As String = ViewData(VTLive40.Helpers.Constants.ScriptVersion)
    Dim companyName As String = Robotics.VTBase.roTypes.Any2String(ViewData("CompanyName"))
    Dim privacyPolicyText As String = Robotics.VTBase.roTypes.Any2String(ViewData("PrivacyPolicyText"))
End Code

<script type="text/javascript">
    var RootUrl = "@Html.Raw(ViewBag.RootUrl)";
    var BASE_URL = "@baseURL";
    var jsLabels = JSON.parse('@Html.Raw(Robotics.VTBase.roJSONHelper.SerializeNewtonSoft(labels)) ');
</script>

<link href="@Url.Content("~/Base/Styles/roLiveStyles.min.css")@Html.Raw(scriptVersion)" rel="stylesheet" type="text/css" />
<link href="@Url.Content("~/Base/Styles/dx.robotics.main.css")@Html.Raw(scriptVersion)" rel="stylesheet" type="text/css" />
<link href="@Url.Content("~/Base/Styles/dx.robotics.css")@Html.Raw(scriptVersion)" rel="stylesheet" type="text/css" />
<link href="@Url.Content("~/Base/Styles/Live/DEX/roDEX.min.css")@Html.Raw(scriptVersion)" rel="stylesheet" type="text/css" />

<script src="https://www.google.com/recaptcha/api.js" async defer></script>
<script src="@Url.Content("~/Base/Scripts/Ajax.js")@Html.Raw(scriptVersion)" type="text/javascript"></script>
<script src="@Url.Content("~/Base/Scripts/Live/DEX/roDEX.min.js")@Html.Raw(scriptVersion)" type="text/javascript"></script>

<div id="news" style="float:left"></div>
<div id="newsOk" style="float:right; margin-top:60px;"></div>
<div class="bg-color-eb h-fill">
    <div id="loadingDiv" style=" z-index: 20000; position: absolute;top: 25%; left: 50%;">
        @(Html.DevExtreme().LoadIndicator() _
                                                .ID("loading") _
                                                .Height(60) _
                                                .Width(60) _
                                                .Visible(False) _
    )
    </div>
    <div class="MainToolbarPad">
        <div id="tbToolbar" class="Toolbar">
            <div class="tbd_logo" onclick="location.href = 'https://visualtime.net';" style="cursor: pointer;">
            </div>
            <div id="tdLogoBar" class="tbd_bar">
                <div id="vtLogoTextDiv" class="tbd_bar_text" style="cursor: pointer; position: relative;" onclick="location.href = 'https://visualtime.net';">
                    <div id="vtLogoVersionDiv" style="margin-top: 13px;" class="tbd_bar_textVersion">
                        <span class="notranslate">cegid Visualtime</span>
                    </div>
                </div>
                <div id="tdMenuToolbar" runat="server" align="center" class="tdMenuToolbar">
                </div>
            </div>
            <div id="menu_icons">
            </div>
        </div>
    </div>
    <div class="d-flex justify-content-around min-vh-94 align-items-center">
        <div class="d-flex p-4 gap-4 flex-column flex-md-row max-width-1400 min-vh-72 w-100">
            <div id="companyInfoMobile" class="border border-3 border-dark rounded-5 p-4 p-md-5 bg-color-grey d-block d-md-none">
                <h2 class="d-inline">@Html.Raw(labels("DEX#CompanyLocation")):</h2><h2 class="d-inline text-decoration-none"> @Html.Raw(companyName)</h2>
            </div>
            <div id="divNewComplaint" class="d-flex" style="flex: 60vw;">
                <div class="border border-3 border-dark rounded-5 flex-grow-1 p-4 p-md-5 bg-color-grey">

                    <div id="sendDEX">
                        <h2>@Html.Raw(labels("DEX#SendComplaint"))</h2>
                        <p>@Html.Raw(labels("DEX#SendComplaintDesc"))</p>
                        <div id="sendDEX1">
                            <div class="identification mt-4 mb-4">
                                <h4>@Html.Raw(labels("DEX#IdentificationTitle"))</h4>
                                <div id="identificationOptions"></div>
                                <div id="identificationData" class="dx-fieldset">
                                    <div class="dx-field">
                                        <div class="dx-field-label">
                                            @Html.Raw(labels("DEX#IdentificationName"))
                                        </div>
                                        <div class="dx-field-value">
                                            @(Html.DevExtreme().TextBox() _
.ID("IdentificationName") _
.MaxLength(250) _
.InputAttr("AutoComplete", "new-password"))
                                        </div>
                                    </div>
                                    <div class="dx-field">
                                        <div class="dx-field-label">
                                            @Html.Raw(labels("DEX#IdentificationPhone"))
                                        </div>
                                        <div class="dx-field-value">
                                            @(Html.DevExtreme().TextBox() _
.ID("IdentificationPhone") _
.InputAttr("AutoComplete", "new-password"))
                                        </div>
                                    </div>
                                    <div class="dx-field">
                                        <div class="dx-field-label">
                                            @Html.Raw(labels("DEX#IdentificationMail"))
                                        </div>
                                        <div class="dx-field-value">
                                            @(Html.DevExtreme().TextBox() _
.ID("IdentificationMail") _
.InputAttr("AutoComplete", "new-password"))
                                        </div>
                                    </div>
                                    <div class="dx-field">
                                        <div class="dx-field-label">
                                            @Html.Raw(labels("DEX#IdentificationCompany"))
                                        </div>
                                        <div class="dx-field-value">
                                            @(Html.DevExtreme().TextBox() _
.ID("IdentificationCompany") _
.InputAttr("AutoComplete", "new-password"))
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="password">
                                <p>@Html.Raw(labels("DEX#PasswordInfo"))</p>
                                <p>@Html.Raw(labels("DEX#PasswordInfoDesc"))</p>
                                <div id="passwordData">
                                    <div class="dx-field">
                                        <div class="dx-field-label">
                                            @Html.Raw(labels("DEX#IdentificationPwd"))
                                        </div>
                                        <div class="dx-field-value">
                                            @(Html.DevExtreme().TextBox() _
.ID("IdentificationPwd") _
.Mode(TextBoxMode.Password) _
.InputAttr("AutoComplete", "new-password"))
                                        </div>
                                    </div>
                                    <div class="dx-field">
                                        <div class="dx-field-label">
                                            @Html.Raw(labels("DEX#IdentificationRepeatPwd"))
                                        </div>
                                        <div class="dx-field-value">
                                            @(Html.DevExtreme().TextBox() _
.ID("IdentificationRepeatPwd") _
.Mode(TextBoxMode.Password) _
.InputAttr("AutoComplete", "new-password"))
                                        </div>
                                    </div>
                                    <div class="dx-field-value mb-3 checkBoxPolicy">
                                        @(Html.DevExtreme().CheckBox() _
                                        .ID("Privacy") _
                                        .Text(labels("DEX#ReadAndAccept")) _
                                        .Value(False))
                                        @If privacyPolicyText.Length > 0 Then
                                            @<span id="privacyPolicyCTA">@Html.Raw(labels("DEX#privacyPolicyCTA"))</span>
                                        Else
                                            @<span class="privacyPolicyCTAEmpty">@Html.Raw(labels("DEX#privacyPolicyCTAEmpty"))</span>
                                        End If
                                    </div>
                                    @(Html.DevExtreme().Button() _
.ID("btnNext") _
.Text(labels("DEX#IdentificationBtnNext").ToString()) _
.OnClick("btnNextDEXClick") _
.Type(ButtonType.Default) _
.StylingMode(ButtonStylingMode.Contained))
                                </div>
                            </div>
                        </div>
                        <div id="sendDEX2" style="display: none;">
                            <div id="passwordData" class="dx-fieldset">
                                <div class="dx-field">
                                    <div class="dx-field-label">
                                        @Html.Raw(labels("DEX#ComplaintTitle"))
                                    </div>
                                    <div class="dx-field-value">
                                        @(Html.DevExtreme().TextBox() _
.ID("DEXTitle") _
.InputAttr("AutoComplete", "new-password"))
                                    </div>
                                </div>
                                <div class="dx-field">
                                    <div class="dx-field-label">
                                        @Html.Raw(labels("DEX#ComplaintBody"))
                                    </div>
                                    <div class="dx-field-value">
                                        @(Html.DevExtreme().TextArea() _
.ID("DEXMessage") _
.Height(90))
                                    </div>
                                </div>
                                @(Html.DevExtreme().Button() _
.ID("btnNext2") _
.Text(labels("DEX#IdentificationBtnNext").ToString()) _
.OnClick("btnSaveDEX") _
.Type(ButtonType.Default) _
.StylingMode(ButtonStylingMode.Contained))
                            </div>
                        </div>
                    </div>

                    <div id="sendDEXResult" style="display:none">
                        <h2>@Html.Raw(labels("DEX#ComplaintResponse"))</h2>
                        <p>@Html.Raw(labels("DEX#ComplaintResponseDesc"))</p>
                        <div id="resultsData" class="pt-5">
                            <div class="dx-field">
                                <div class="dx-field-value">
                                    <span>@Html.Raw(labels("DEX#ComplaintGuidDesc"))</span>
                                </div>
                            </div>
                            <div class="dx-field">
                                <div class="dx-field-value">
                                    <div>
                                        @(Html.DevExtreme().TextBox() _
.ID("DEXIdentifier") _
.InputAttr("AutoComplete", "new-password"))
                                    </div>
                                </div>
                            </div>
                            <div class="mt-3 mb-4">
                                @(Html.DevExtreme().Button() _
.ID("btnCopyClipboard") _
.Text(labels("DEX#copyGUID")) _
.OnClick("btnCopyClipboard") _
.Type(ButtonType.Default) _
.StylingMode(ButtonStylingMode.Contained))
                            </div>
                            <div class="dexImporantInfo">@Html.Raw(labels("DEX#LostDataWarning"))</div>
                            <div class="closeDEX mt-5">
                                @(Html.DevExtreme().Button() _
.ID("btnCcloseDEX") _
.Text(labels("DEX#closeDEX")) _
.OnClick("btnCloseDEX") _
.Type(ButtonType.Default) _
.StylingMode(ButtonStylingMode.Contained))
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <div style="display: flex; flex-direction: column; gap: 2rem; flex: 40vw;">
                <div id="companyInfo" class="border border-3 border-dark rounded-5 p-4 p-md-5 bg-color-grey d-md-block d-none">
                    <h2 class="d-inline">@Html.Raw(labels("DEX#CompanyLocation")):</h2><h2 class="d-inline text-decoration-none"> @Html.Raw(companyName)</h2>
                </div>
                <div class="border border-3 border-dark rounded-5 flex-grow-1 p-4 p-md-5 bg-color-grey">
                    <form action="/DEX/@Html.Raw(ViewData("CompanyGUID"))/ContentMessages" method="post">
                        <div id="divQueryComplaint">
                            <h2>@Html.Raw(labels("DEX#recoverComplaint"))</h2>
                            <p>@Html.Raw(labels("DEX#recoverComplaintDesc"))</p>
                            <div id="complaintInfo" class="dx-field">
                                <div class="dx-field-label">
                                    @Html.Raw(labels("DEX#recoverComplaintID"))
                                </div>
                                <div class="dx-field-value">
                                    @(Html.DevExtreme().TextBox() _
.ID("complaintReference") _
.Name("complaintReference") _
.InputAttr("AutoComplete", "new-password"))
                                </div>
                            </div>
                            <div class="dx-field">
                                <div class="dx-field-label">
                                    @Html.Raw(labels("DEX#IdentificationPwd"))
                                </div>
                                <div class="dx-field-value">
                                    @(Html.DevExtreme().TextBox() _
.ID("password") _
.Name("password") _
.Mode(TextBoxMode.Password) _
.InputAttr("AutoComplete", "new-password"))
                                </div>
                            </div>
                            <div id="recaptcha" class="g-recaptcha"
                                 data-sitekey="6LfSKR4pAAAAAMGL6eocU4SwDDzb7RuN8CNCF0rj"
                                 data-callback="onDexValidationSubmit"
                                 data-size="invisible"></div>

                            @(Html.DevExtreme().Button() _
.ID("btnRecoverComplaint") _
.Text(labels("DEX#btnRecoverComplaint").ToString()) _
.Type(ButtonType.Default) _
.OnClick("btnRecoverComplaintOnClick") _
.UseSubmitBehavior(False) _
.StylingMode(ButtonStylingMode.Contained))
                        </div>
                    </form>
                </div>
            </div>
        </div>
    </div>
</div>

@Code
    Html.DevExtreme().Popup() _
    .ID("privacyPolicyPopup") _
    .ShowTitle(False) _
    .Title("") _
    .DragEnabled(False) _
    .HideOnOutsideClick(True) _
    .ContentTemplate(Sub()
    @<text>
        @Html.Raw(privacyPolicyText)
    </text>
End Sub) _
.Render()
End Code

<script>
    initDEX();
</script>