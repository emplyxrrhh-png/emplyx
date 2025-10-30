<%@ control language="VB" autoeventwireup="false" inherits="VTLive40.WebUserControls_roMainMenu" codebehind="roMainMenu.ascx.vb" %>
<script>
    window.GPTTConfig = {
        uuid: "<%=UIDPulse%>"
    }
</script>

<% If showAIChatBot Then%>
<script src="https://app.gpt-trainer.com/widget-asset.min.js" defer>
</script>
<%End If %>


<script type="text/javascript" language="javascript">
    function showSubMenu() {
        document.getElementById('ToolbarMenuPanel').style.display = '';
    }

    function hideSubMenu() {
        document.getElementById('ToolbarMenuPanel').style.display = 'none';
    }

    function showUserTasksList() {
        document.getElementById('divUserTasksList').style.display = '';
    }

    function hideUserTasksList() {
        document.getElementById('divUserTasksList').style.display = 'none';
    }

    function showHistoryList() {
        document.getElementById('divHistoryList').style.display = '';
    }

    function showAlertsStatus() {
        document.getElementById('divAlertsStatus').style.display = '';
    }

    function hideHistoryList() {
        document.getElementById('divHistoryList').style.display = 'none';
    }

    function hideAlertsStatus() {
        document.getElementById('divAlertsStatus').style.display = 'none';
    }

    function ShowChangePwd(showCancel) {
        parent.ShowExternalForm2('Security/Wizards/ChangePassword.aspx?cCancel=' + (showCancel ? '1' : '0'), 400, 245, '', '', false, false);
    }

    function showUseAndConditions() {
        try {
            var popupContainer = document.getElementById('popupCookiesPolicy');
            if (!popupContainer) return;

            var popup = $("<div>").appendTo(popupContainer).dxPopup({
                width: 800,
                height: 600,
                contentTemplate: function (contentElement) {
                    $("<object>")
                        .attr("data", "/Employee/CookiesPolicyPDF")
                        .attr("type", "application/pdf")
                        .css({ width: "100%", height: "100%", border: "none" })
                        .appendTo(contentElement);
                },
                showCloseButton: true,
                title: "<%=Me.Language.Translate("CookiesPolicy", Me.DefaultScope)%>",
                visible: false,
                dragEnabled: true,
                closeOnOutsideClick: true
            }).dxPopup("instance");

            // Mostramos el popup
            popup.show();
    
        } catch (e) {
            console.error('Error al obtener el pdf: ', e);
        }
    }

    function ShowAboutMe() {

        try {
            var contentUrl = "<%= Me.Page.ResolveUrl("~/Base/Popups/AboutMe.aspx") %>";
            AboutMe_Client.SetContentUrl(contentUrl);
            AboutMe_Client.Show();

        }
        catch (e) { showError('AboutMeError', e); }

    }

    function showLoading(loading) {

        var img = document.getElementById('imgLoading');
        if (img != null) {
            if (loading == true) {
                img.style.display = '';
            }
            else {
                img.style.display = 'none';
            }
        }

        var tbButtons = document.getElementById('tbChangePwdButtons');
        if (tbButtons != null) {
            if (loading == true) {
                tbButtons.style.display = 'none';
            }
            else {
                tbButtons.style.display = '';
            }
        }

    }

    function showLoadingLanguage(loading) {

        var img = document.getElementById('imgLoadingLanguage');
        if (img != null) {
            if (loading == true) {
                img.style.display = '';
            }
            else {
                img.style.display = 'none';
            }
        }

        var tbButtons = document.getElementById('tbChangeLanguageButtons');
        if (tbButtons != null) {
            if (loading == true) {
                tbButtons.style.display = 'none';
            }
            else {
                tbButtons.style.display = '';
            }
        }

    }

    <%--function RunAcceptChangePwd() {
        showLoading(true);
        ButtonClick($get('<%= btChangePwdOKHdn.ClientID %>'));
    }--%>

    function RunAcceptChangeLanguage() {
        showLoadingLanguage(true);
        ButtonClick($get('<%= btChangeLanguageOKHdn.ClientID %>'));
    }

    function logoutClient() {

        for (let i = 0; i < localStorage.length; i++) {
            const key = localStorage.key(i);
            if (key.startsWith('TreeState_')) {
                localStorage.removeItem(key);
                // Al remover un ítem, la longitud de localStorage cambia,
                // así que es necesario ajustar el índice.
                i--;
            }
        }



        var oParameters = {}
        oParameters.action = "LOGOUT";
        oParameters.path = '';
        oParameters.url = '';

        var strParameters = JSON.stringify(oParameters);
        strParameters = encodeURIComponent(strParameters);

        ASPxCallbackMainMenuClient.PerformCallback(strParameters);
    }

    function goToReleaseNotes() {
        window.open('https://helpcenter.ila.cegid.com/es/visualtime/novedades/', '_blank').focus();
    }

    function RefreshMainMenu(path, url, mvcPath) {
        var oParameters = {}
        oParameters.action = "REFRESHMENU";
        oParameters.path = path;
        oParameters.url = url.replace('.aspx', '');
        oParameters.mvcPath = mvcPath;

        var strParameters = JSON.stringify(oParameters);
        strParameters = encodeURIComponent(strParameters);

        ASPxCallbackMainMenuClient.PerformCallback(strParameters);
    }

    function ASPxCallbackMainMenuClient_EndCallBack(s, e) {
        try {

            //Historial
            var oUserHistory = new roUserHistory();
            var VTHistory = new Array();
            VTHistory = oUserHistory.getHistory();

            var divHistory = document.getElementById("divHistoryListContent");
            if (divHistory != null) {

                if (VTHistory.length > 0) {
                    var historyDiv = '';
                    var strRef = '';
                    var bolFirst = true;
                    var strStyle = '';
                    for (n = VTHistory.length - 1; n >= 0; n--) {
                        strRef = "";
                        if (VTHistory[n].LanguageTAG != "") {
                            var urlLink = location.href.split("#")[0] + "#" + VTHistory[n].Url;
                            var onScript = ""
                            if (VTHistory[n].Url.indexOf("?") > 0) {
                                onScript = "reenviaFrame('" + VTHistory[n].Url + ".aspx', '','','" + VTHistory[n].MenuPath + "'); return false;";

                            } else {
                                onScript = "reenviaFrame('" + VTHistory[n].Url.replace("?", ".aspx?") + "', '','','" + VTHistory[n].MenuPath + "'); return false;";
                            }
                            if (bolFirst) {
                                strStyle = 'style="border-left: solid 1px #999999;"';
                                bolFirst = false;
                            } else {
                                strStyle = '';
                            }
                            strRef += '<a href="' + urlLink + '" class="aUserHistory" onclick="' + onScript + '" ' + strStyle + '>' + VTHistory[n].LanguageTAG + '</a>';
                        }

                        historyDiv = historyDiv + strRef;
                    }

                    $("#divHistoryListContent").html(historyDiv);
                }
            }

            if (s.cpNotFoundRO == false) {

                if (s.cpTextRO != "") {
                    var frm = document.getElementById('ifPrincipal');
                    if (frm != null) {
                        if (s.cpUrlRO != "" && s.cpUrlRO != "#Start") {
                            oUserHistory.addHistory(s.cpTextRO, s.cpUrlRO, s.cpPathRO);
                        }
                    }
                }
                var frm = document.getElementById('ifPrincipal');
                var uri = "";
                if (s.cpUrlRO != "#Start") {
                    frm.src = s.cpUrlRO + ((typeof s.cpPathMVCRO != 'undefined' && s.cpPathMVCRO != null) ? s.cpPathMVCRO : '');
                } else {
                    frm.src = "Start";
                }
                document.location.hash = s.cpUrlRO.replace('.aspx', '');

            } else {
                var frm = document.getElementById('ifPrincipal');
                frm.src = 'Start';
                document.location.hash = 'Start';
            }

            if (s.cpLoggedInRO == false) {
                window.location = "LoginWeb.aspx";

            }
        }
        catch (e) {
            alert(e);
        } finally {
            //showLoader(false);
        }
    }

    function showSubMenuClient(subMenuInstanceName) {
        eval(subMenuInstanceName + ".Show();");
    }

    var actionsPopover = null;
    function setUPReportsAndWizards(oResponseObj) {

        if (oResponseObj.HasReports) {
            $('#showReportLauncher').off('click');

            var onClickFunc = function (oReportsAction) {
                return function () {
                    eval('window.parent.frames["ifPrincipal"].' + oReportsAction);
                }
            }

            $('#showReportLauncher > i', top.window.parent.document).on('click', onClickFunc(oResponseObj.ReportsAction));
            $('#showReportLauncher', top.window.parent.document).show();
        } else {
            $('#showReportLauncher > i', top.window.parent.document).off('click');
            $('#showReportLauncher', top.window.parent.document).hide();
        }

        if (oResponseObj.HasAssistants) {
            $('#showWizardLauncher').off('click');

            var onClickFunc = function (oAssistants) {
                actionsPopover = $('#showWizardLauncher_Popover').dxPopup({
                    animation: {
                        hide: "{ type: 'pop', duration: 0, to: { opacity: 0, scale: 0.55 }, from: { opacity: 1, scale: 1 } } }, { type: 'slide', duration: 400, from: { position: { my: 'center', at: 'center', of: window } }, to: { position: { my: 'top', at: 'bottom', of: window } }} (iOS)",
                        show: "{ type: 'pop', duration: 0, from: { scale: 0.55 } }, { type: 'slide', duration: 400, from: { position: { my: 'top', at: 'bottom', of: window } }, to: { position: { my: 'center', at: 'center', of: window } }} (iOS)"
                    },
                    showTitle: false,
                    hideOnOutsideClick: false,
                    shadingColor: 'rgb(0, 0, 0,0.3)',
                    width: 500,
                    height: 'auto',
                    minHeight: 450,
                    onShown: function () {
                        $('#actionsList').dxList({
                            dataSource: oAssistants,
                            height: 400,
                            itemTemplate: function (data, index) {
                                var result = $("<div>").addClass("assistantLine");

                                $("<div>").attr("class", data.Icon).attr('style', 'float:left;width:32px').appendTo(result);

                                var content = $("<div>").attr('style', 'float:left;width:calc(100% - 64px);border-bottom: 2px inset;padding-bottom: 5px;');
                                content.append($("<div>").attr("class", 'popoverText').text(data.Text));
                                content.append($("<div>").attr("class", 'popoverDesc').append($("<span>").text(data.Description)));
                                content.appendTo(result);

                                $("<div>").attr("class", 'flaticon-right2').appendTo(result);

                                return result;

                            },
                            onItemClick: function (data, index) {
                                actionsPopover.hide();
                                eval('window.parent.frames["ifPrincipal"].' + data.itemData.Action);
                            }
                        }).dxList("instance");
                    }
                }).dxPopup("instance");

                return function () {
                    actionsPopover.show();
                }
            }

            $('#showWizardLauncher > i', top.window.parent.document).on('click', onClickFunc(oResponseObj.Assistants));
            $('#showWizardLauncher', top.window.parent.document).show();
        } else {
            $('#showWizardLauncher').off('click');

            $('#showWizardLauncher', top.window.parent.document).hide();
        }
    }
</script>

<body>
<input id="hdnLiveVersion" type="hidden" value="<%=Me.LiveVersion()%>" />
<div class="MainToolbarPad">
    <div id="tbToolbar" class="Toolbar">
        <div class="tbd_logo" onmouseover="showSubMenu();" onmouseout="hideSubMenu();" style="cursor: pointer;">
            <%--<div class="tblogo" onmouseover="showSubMenu();" onmouseout="hideSubMenu();" style="cursor: pointer;"></div>--%>
        </div>
        <div id="tdLogoBar" class="tbd_bar">
            <div id="vtLogoTextDiv" class="tbd_bar_text" style="cursor: pointer" onclick="reenviaFrame('/./Main.aspx','','Inicio','PortalHome')">
                <div id="vtLogoVersionDiv" style="margin-top: 13px; position: fixed;" class="tbd_bar_textVersion" runat="server">
                    <span class="notranslate">cegid Visualtime</span>
                    <%--<img src="Base/Images/Logo_VT5.png" width="50px" alt="" />--%>
                </div>
                <!--Al tener el nombre del cliente, debemos quitar la separacion de arriba ya que no es necesaria (margin-top) -->
                <div id="vtLogoVersionClientNameDiv" style="position: fixed;" class="tbd_bar_textVersion" runat="server">
                    <span class="notranslate cegid-title">cegid Visualtime</span>
                    <span id="ClientName" class="notranslate client-name" runat="server">ClientName</span>
                </div>
            </div>
            <div id="tdMenuToolbar" runat="server" align="center" class="tdMenuToolbar">
                <dx:aspxcallbackpanel id="ASPxCallbackMainMenu" runat="server" width="100%" clientinstancename="ASPxCallbackMainMenuClient">
                    <settingsloadingpanel enabled="false" />
                    <clientsideevents endcallback="ASPxCallbackMainMenuClient_EndCallBack" />
                    <panelcollection>
                        <dx:panelcontent id="ASPxCallbackMainMenuContent" runat="server">
                            <div id="mainMenuBar" runat="server">
                            </div>
                        </dx:panelcontent>
                    </panelcollection>
                </dx:aspxcallbackpanel>
                <!-- Carrega de Botons dinamics -->
            </div>
        </div>
        <div id="menu_icons">
            <div onclick="logoutClient()"><i class="flaticon-simple1"></i></div>
            <div onclick="goToReleaseNotes()"><i class="icon-bullhorn"></i></div>
            <div onmouseover="showHistoryList();" onmouseout="hideHistoryList();"><i class="flaticon-history6"></i></div>
            <div id="showReportLauncher" style="display: none"><i class="icon-printer"></i></div>
            <div id="showWizardLauncher" style="display: none"><i id="iShowWizard" class="icon-cogs1"></i></div>
            <div id="btnAlerts" class="AlertDiv" runat="server" onmouseover="" onmouseout="">
                <div id="btnAlertsMain" class="AlertOk">
                    <div id="btnAlertsCount" class="AlertOutText"></div>
                </div>
            </div>
            <div id="showWizardLauncher_Popover" style="display: none">
                <div class="panBottomMargin">
                    <div class="panHeader2 panBottomMargin">
                        <span class="panelTitleSpan">
                            <asp:Label runat="server" ID="lblWizardsDescrition" Text="Asistentes"></asp:Label>
                            <img src="Base/Images/btnClose.png" onclick="actionsPopover.hide();" style="float: right; padding-right: 10px; cursor: pointer;">
                        </span>
                    </div>
                </div>
                <div id="actionsList" style="margin-bottom: 15px"></div>
            </div>
        </div>
    </div>
</div>
<div id="licenseExpireSoon" runat="server" style="display: none" class="expireSoon">
    <asp:Label ID="lblExpireSoonDescription" runat="server" Font-Size="Small" CssClass="padTop" Text="La licencia esta a punto de expirar"></asp:Label>
</div>
<asp:Label ID="lblMenu_Selected" runat="server" Text="" Style="display: none; visibility: hidden;"></asp:Label>
<asp:Label ID="lblMenuPath_Selected" runat="server" Text="" Style="display: none; visibility: hidden;"></asp:Label>
<!-- Menu START -->
<div id="ToolbarMenuPanel" class="toolbarMenuPanel" style="display: none;" onmouseover="showSubMenu();" onmouseout="hideSubMenu();">
    <rowebcontrols:ropopupframev2 id="Toolbar1" runat="server" borderstyle="None" bordercolor="Transparent" cssprefix="PopupFrameStart">
        <table cellpadding="0" cellspacing="0" width="100%" border="0" class="PopupFrameStart_Center">
            <tr>
                <td valign="top" style="width: 242px;">
                    <!-- Panell Blanc sub-menu vertical -->
                    <div id="microtaula" class="microtaula">
                        <asp:Panel ID="panToolbar1" Width="100%" runat="server">
                        </asp:Panel>
                    </div>
                </td>
                <td></td>
                <td align="center" valign="top" style="height: 100%; position:relative;">
                    <table cellpadding="0" cellspacing="0" border="0" width="100%" style="height: 80%;padding-bottom: 30px;">
                        <tr>
                            <td align="left" valign="top">
                                <asp:Image ID="imgUserPhoto" ImageUrl="UserPhoto.aspx" Height="70" runat="server" Style="border: solid 2px white;" />
                            </td>
                            <td align="right" valign="top" style="padding-left: 10px;">
                                <table cellpadding="0" cellspacing="0">
                                    <tr>
                                        <td align="right">
                                            <asp:Label ID="lblVersionType" runat="server" Font-Bold="true" Font-Size="Small"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="right"  style="padding-top: 5px">
                                            <a href="javascript: void(0);" id="A2" runat="server" class="verHistory" onclick="showVersionHistory();" style="padding-top: 0px">
                                                <asp:Label ID="lblVersionInfo1" runat="server" Font-Bold="true"></asp:Label>
                                            </a>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="right">
                                            <a href="javascript: void(0);" id="A1" runat="server" onclick="showVersionHistory();" style="padding-top: 0px">
                                                <asp:Label ID="lblVersionInfo2" runat="server"></asp:Label>
                                            </a>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td style="" colspan="2">
                                <div style="padding-left: 10px; padding-top: 5px; font-weight: bold; font-size: 12px">
                                    <asp:Label ID="lblUserName2" Text="Usuario" ForeColor="Black" runat="server" />
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td id="tdEmergencyPrint" runat="server" valign="bottom" align="right" colspan="2">
                                <a href="javascript: void(0);" runat="server" id="btEmergencyPrint" onclick="EmergencyPrint();" class="PrintEmer">
                                    <asp:Label ID="lblprintEmer" runat="server" Style="white-space: nowrap; position: relative; top: 22px;"></asp:Label>
                                </a>
                            </td>
                        </tr>
                        <tr>
                            <td valign="bottom" align="right" colspan="2">
                                <table cellpadding="0" cellspacing="0" width="100%">
                                    <tr id="trChangePwd" runat="server">
                                        <td align="right" style="padding-top: 10px">
                                            <a href="javascript: void(0);" id="btChangePwd2" runat="server" onclick="ShowChangePwd(true);" class="chpass"><%=Me.Language.Translate("ChangePassword", Me.DefaultScope)%></a>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="right" style="padding-top: 10px">
                                            <a href="javascript: void(0);" id="btChangeLanguage" runat="server" onclick="showPopup('mpxChangeLanguageBehavior');" class="chlang"><%=Me.Language.Translate("ChangeLanguage", Me.DefaultScope)%></a>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="right" style="padding-top: 5px;">
                                            <a href="javascript: void(0);" id="btSignOut2" runat="server" onclick="" class="closesess"><%=Me.Language.Translate("CloseSession", Me.DefaultScope)%></a>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                    <table cellpadding="0" cellspacing="0" border="0" width="100%" style="position:absolute;bottom:0;">
                        <tr>
                            <td valign="bottom" align="right" colspan="2">
                                <table cellpadding="0" cellspacing="0" width="100%">
                                    <tr id="tr1" runat="server">
                                        <td align="right" style="padding-top:50px">
                                            <a href="javascript: void(0);" id="btAboutMe" runat="server" onclick="showUseAndConditions();"><%=Me.Language.Translate("CookiesPolicy", Me.DefaultScope)%></a>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                    
                    <%-- <table cellpadding="0" cellspacing="0" border="0" width="100%" style="height:20%;">
                        <tr>
                            <td valign="bottom" align="right" colspan="2">
                                <table cellpadding="0" cellspacing="0" width="100%">
                                    <tr id="tr1" runat="server">
                                        <td align="right" style="padding-top:50px">
                                            <a href="javascript: void(0);" id="btAboutMe" runat="server" onclick="ShowAboutMe();" class="chaboutme"><%=Me.Language.Translate("AboutMe", Me.DefaultScope)%></a>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>--%>
                </td>
            </tr>
        </table>
    </rowebcontrols:ropopupframev2>
</div>

<div id="popupCookiesPolicy"></div>
<div id="popupVersionHistory"></div>

<asp:HiddenField ID="hdnLicenseIssue" Value="0" runat="server" />
<asp:HiddenField ID="hdnLOPD" Value="0" runat="server" />
<asp:HiddenField ID="hdnPASSWORDEXPIRED" Value="0" runat="server" />
<asp:HiddenField ID="hdnShowENSPopup" Value="0" runat="server" />
<asp:HiddenField ID="HiddenVersionHistory" Value="0" runat="server" />
<asp:HiddenField ID="VersionHistoryText" Value="0" runat="server" />

<!-- Canvi de Idioma -->
<ajaxtoolkit:modalpopupextender id="mpxChangeLanguage" runat="server" behaviorid="mpxChangeLanguageBehavior"
    targetcontrolid="hiddenTargetControlForChangeLanguagePopup"
    popupcontrolid="divChangeLanguage"
    dropshadow="False"
    popupdraghandlecontrolid="panChangeLanguageDragHandle"
    backgroundcssclass="modalBackground"
    enableviewstate="true">
</ajaxtoolkit:modalpopupextender>

<asp:Button runat="server" ID="hiddenTargetControlForChangeLanguagePopup" Style="display: none" />
<asp:HiddenField ID="hdnChangeLanguagePosition" Value="" runat="server" />
<div id="divChangeLanguage" runat="server" style="display: none">
    <rowebcontrols:ropopupframev2 id="ropfChangeLanguage" runat="server" borderstyle="None" height="75px" width="300px">
        <framecontenttemplate>

            <asp:UpdatePanel ID="updChangeLanguage" runat="server">
                <contenttemplate>

                    <table width="100%" cellspacing="0" border="0" class="bodyPopup">

                        <tr id="panChangeLanguageDragHandle" style="cursor: move; height: 20px;">
                            <td align="center" class="chlangForm"></td>
                            <td>
                                <asp:Label ID="lblChangeLanguageTitle" Text="Cambiar idioma" Font-Size="12px" Font-Bold="true" ForeColor="Black" runat="server" />
                            </td>
                            <td align="right" style="display: none;">
                                <asp:ImageButton ID="ibtChangeLanguageOK" runat="server" ImageUrl="~/Base/Images/ButtonOK_16.png" Style="cursor: hand;"
                                    OnClientClick="ChangeLanguage(); showLoadingLang(true);" />
                                <asp:ImageButton ID="ibtChangeLanguageCancel" runat="server" ImageUrl="~/Base/Images/ButtonCancel_16.png" Style="cursor: hand;"
                                    OnClientClick="hidePopup('mpxChangeLanguageBehavior'); return false;" />
                            </td>
                        </tr>
                        <tr>
                            <td colspan="3" style="padding-left: 10px; padding-top: 5px; vertical-align: middle">
                                <table cellpadding="1" cellspacing="0">
                                    <tr>
                                        <td>
                                            <asp:Label ID="lblLanguage" runat="server" Text="Idioma :" />
                                        </td>
                                        <td style="padding-left: 10px">
                                            <rowebcontrols:rocombobox id="cmbLanguage" runat="server" parentwidth="170px" autoresizechildswidth="true" hiddentext="cmbLanguage_Text" hiddenvalue="cmbLanguage_Value" childsvisible="4"></rowebcontrols:rocombobox>
                                            <input type="hidden" id="cmbLanguage_Text" runat="server" />
                                            <input type="hidden" id="cmbLanguage_Value" runat="server" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="2" align="center">
                                            <asp:Label ID="lblChangeLanguageMessage" Text="" CssClass="errorText" Style="display: none; visibility: hidden" runat="server"></asp:Label>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="3" align="center" style="padding-top: 10px; padding-bottom: 5px;">
                                <table id="tbChangeLanguageButtons" border="0" cellpadding="0" cellspacing="0">
                                    <tr>
                                        <td>
                                            <asp:Button ID="btChangeLanguageOK" Text="${Button.Accept}" runat="server" TabIndex="3" OnClientClick="RunAcceptChangeLanguage(); return false;" CssClass="btnFlat btnFlatBlack btnFlatAsp" />
                                            <asp:Button ID="btChangeLanguageOKHdn" runat="server" Style="display: none;" />
                                        </td>
                                        <td>
                                            <asp:Button ID="btChangeLanguageCancel" Text="${Button.Cancel}" runat="server" TabIndex="4" OnClientClick="hidePopup('mpxChangeLanguageBehavior'); return false;" CssClass="btnFlat btnFlatBlack btnFlatAsp" />
                                        </td>
                                    </tr>
                                </table>
                                <img id="imgLoadingLanguage" src="Base/Images/loading.gif" alt="Loading..." style="display: none;" />
                            </td>
                        </tr>
                    </table>
                </contenttemplate>
            </asp:UpdatePanel>
        </framecontenttemplate>
    </rowebcontrols:ropopupframev2>
</div>

<!-- POPUP AboutMe -->
<dx:aspxpopupcontrol id="AboutMe" runat="server" allowdragging="False" closeaction="None" modal="True" contenturl="~/Base/Popups/AboutMe.aspx"
    popupverticalalign="WindowCenter" popuphorizontalalign="WindowCenter" width="770px" height="600px"
    showheader="False" scrollbars="Auto" showpagescrollbarwhenmodal="True" clientinstancename="AboutMe_Client" popupanimationtype="None" backcolor="Transparent" contentstyle-paddings-padding="0px" border-bordercolor="Transparent" showshadow="false">
    <settingsloadingpanel enabled="false" />
</dx:aspxpopupcontrol>

<!-- Lista TAREAS DE USUARIO -->
<div id="divUserTasksList" class="UserTasksFrame" style="display: none" onmouseover="" onmouseout="">
    <rowebcontrols:ropopupframev2 id="rpUserTasksList" runat="server" borderstyle="None" bordercolor="Transparent" cssprefix="UserTasksFrameContent">
        <div id="divUserTasksListContent" style="width: 710px;">
        </div>
    </rowebcontrols:ropopupframev2>
</div>

<div id="divHistoryList" class="HistoryFrame" style="display: none" onmouseover="showHistoryList();" onmouseout="hideHistoryList();">
    <rowebcontrols:ropopupframev2 id="rpHistoryList" runat="server" borderstyle="None" bordercolor="Transparent" cssprefix="UserTasksFrameContent">
        <div id="divHistoryListContent" style="width: 600px;">
        </div>
    </rowebcontrols:ropopupframev2>
</div>

<div id="divAlertsStatus" class="AlertsFrame" style="display: none" onmouseover="" onmouseout="">
    <rowebcontrols:ropopupframev2 id="rpAlertsStatus" runat="server" borderstyle="None" bordercolor="Transparent" cssprefix="UserTasksFrameContent">
        <div id="divAlertsStatusContent" style="width: 175px; height: 100px;">
            <div class="NewsSupportPosition">
                <asp:Label ID="lblSupportText" runat="server" Font-Bold="true" Style="color: #333333"><%=Me.Language.Translate("LastUpdate", Me.DefaultScope)%></asp:Label><br />
                <div id="divLastUpdate" style="color: #333333">
                    <asp:Label ID="lastUpdateText" runat="server"></asp:Label>
                </div>
                <br />

                <asp:ImageButton ID="ibtRefreshAlerts" runat="server" ImageUrl="~/Base/Images/PortalRequests/refreshAlerts.png" Style="cursor: pointer;" OnClientClick="hideAlertsStatus(); ReloadAlerts(); return false;" />

                <br />
            </div>
        </div>
    </rowebcontrols:ropopupframev2>
</div>
</body>