<%@ Page Language="VB" AutoEventWireup="false" EnableEventValidation="false" Inherits="VTLive40._Main" CodeBehind="Main.aspx.vb" %>

<html lang="en" class="notranslate" translate="no" xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta name="robots" content="noindex" />
    <meta name="googlebot" content="noindex" />
    <meta name="robots" content="nofollow" />
    <meta name="robots" content="nosnippet" />
    <meta name="robots" content="noarchive" />

    <title>VisualTime Live</title>
    <link rel="shortcut icon" href="~/Base/Images/logovtl.ico" />
</head>
<body style="margin: 0; min-width: 1200px; overflow: auto;" class="defaultBackgroundColor">
    <script src="Base/Scripts/MainMenu.min.js" type="text/javascript"  defer></script>
    <script type="text/javascript" lang="javascript">
        //var pathEditAuto = true;

        function OnKeyDown_PageBase(e) {
            if (e && e.keyCode == Sys.UI.Key.esc) {
                return false;
            }
        }

        function reenviaFrame(url, objID, oName, Path, mvcPath) {
            try {
                var bContinue = true;
                try {

                    window.parent.setUPReportsAndWizards({ HasReports: false, HasAssistants: false });
                    if (typeof (window.frames['ifPrincipal']) == 'undefined') bContinue = false

                    if (bContinue) {
                        if (typeof (window.frames['ifPrincipal'].roScheduleCalendar) != 'undefined') {
                            bContinue = !window.frames['ifPrincipal'].roScheduleCalendar.hasChanges;
                            var onAcceptFunc = 'top.privateReenviaFrame("' + url + '","' + objID + '","' + oName + '","' + Path + '")';
                            if (!bContinue) window.frames['ifPrincipal'].roScheduleCalendar.showChangesWarning(onAcceptFunc);
                        } else if (typeof (window.frames['ifPrincipal'].securityChartHasChanges) != 'undefined') {
                            bContinue = !window.frames['ifPrincipal'].securityChartHasChanges;
                            var onAcceptFunc = 'top.privateReenviaFrame("' + url + '","' + objID + '","' + oName + '","' + Path + '")';
                            if (!bContinue) window.frames['ifPrincipal'].showChangesWarning(onAcceptFunc);
                        }
                    } else {
                        bContinue = true;
                    }
                } catch (e) {
                    bContinue = true;
                }

                if (bContinue) privateReenviaFrame(url, objID, oName, Path, mvcPath);
            }
            catch (e) {
                alert(e);
                showLoader(false);
            }
        }

        function privateReenviaFrame(url, objID, oName, Path, mvcPath) {
            try {
                //pathEditAuto = true;
                showLoader(true);
                RefreshMainMenu(Path, url, mvcPath);
            }
            catch (e) {
                alert(e);
                showLoader(false);
            }
        }

        window.onload = initialize;

        // Control de historial
        function initialize() {
            // initialize RSH
            dhtmlHistory.initialize();

            // subscribe to DHTML history change events
            //dhtmlHistory.addListener(historyChange);

            loadMainScreen();
        }

        function loadMainScreen() {
            //pathEditAuto = true;

            var frm = document.getElementById('ifPrincipal');
            if (location.hash) {
                var oUserHistory = new roUserHistory();
                var VTHistory = new Array();
                VTHistory = oUserHistory.getHistory();

                var oPath = ""

                if (VTHistory.length > 0) {
                    var strRef = '';
                    var bolFirst = true;
                    var strStyle = '';
                    for (n = VTHistory.length - 1; n >= 0; n--) {
                        if (VTHistory[n].Url == location.hash.substring(1)) {
                            oPath = VTHistory[n].MenuPath;
                        }
                    }
                }

                if (location.hash.indexOf("Start") != -1) {
                    RefreshMainMenu(oPath, "#Start");
                } else {
                    RefreshMainMenu(oPath, location.hash.substring(1));
                }
            } else {
                RefreshMainMenu('', '#Start');
            }

            document.getElementById("ifPrincipal").onload = function () {
                showLoader(false);
            }
        }

        /** Our callback to receive history change events. */
        function historyChange(newLocation, historyData) {
            hash = location.hash;
            var frm = document.getElementById('ifPrincipal');
            if (hash) {
                if (hash.substring(1).indexOf("?") > 0) {
                    uri = hash.substring(1).replace("?", ".aspx?");
                    frm.src = uri;
                } else {
                    frm.src = hash.substring(1) + ".aspx";
                }
                //frm.src = hash.substring(1) + '.aspx';
            }
            else {
                frm.src = 'Start';
            }
        }

        function RefreshScreen(DataType, Parms) {
            // Llamamos la función de refrescar de la página actual.
            try {
                RefreshUserTasks(false);
            } catch (e) { }

            try {
                window.frames['ifPrincipal'].RefreshScreen(DataType, Parms);
            } catch (e) { }

        }

        var IsPostBack = false;

        function PageBase_Load(sender, args) {

            if (document.getElementById('<%= Me.MainMenu.hdnLOPDClientID %>').value == '1') {
                var hBaseRef = '<%= Me.Page.ResolveURL("~/Base/") %>';
                var url = hBaseRef + "srvLoginMsgBox.aspx?action=AlertLOPD";
                ShowMsgBoxForm(url, 600, 300, '');
            }

            if (document.getElementById('<%= Me.MainMenu.hdnPASSWORDEXPIREDClientID %>').value == '1') {
                var hBaseRef = '<%= Me.Page.ResolveUrl("~/Base/") %>';
                var url = hBaseRef + "srvLoginMsgBox.aspx?action=AlertPASSWORDEXPIRED";
                ShowMsgBoxForm(url, 400, 300, '');
            }

            if (document.getElementById('<%= Me.MainMenu.hdnLicenseIssueClientID %>').value == '1') {
                var hBaseRef = '<%= Me.Page.ResolveUrl("~/Base/") %>';
                var url = hBaseRef + "srvLoginMsgBox.aspx?action=AlertLICENSEEXCEED";
                ShowMsgBoxForm(url, 400, 300, '');
            }

            CheckLogo();

            if (document.getElementById('<%= Me.MainMenu.hdnShowENSPopupID %>').value == '1') {
                var hBaseRef = '<%= Me.Page.ResolveUrl("~/Base/") %>';
                var url = hBaseRef + "srvLoginMsgBox.aspx?action=AlertENS";
                ShowMsgBoxForm(url, 600, 300, '');
            }
        }

        function CheckParams() {
            var sURL = window.document.URL.toString();
            if (sURL.indexOf("?") > 0) {
                var arrParams = sURL.split("?");

                var arrURLParams = arrParams[1].split("&");

                var arrParamNames = new Array(arrURLParams.length);
                var arrParamValues = new Array(arrURLParams.length);

                var i = 0;
                for (i = 0; i < arrURLParams.length; i++) {
                    var sParam = arrURLParams[i].split("=");
                    arrParamNames[i] = sParam[0];
                    if (sParam[1] != "")
                        arrParamValues[i] = unescape(sParam[1]);
                    else
                        arrParamValues[i] = "No Value";
                }

                for (i = 0; i < arrURLParams.length; i++) {
                    if (arrParamNames[i] == 'LOPD') {
                        if (arrParamValues[i] == '1') {
                            var url = "Employees/srvMsgBoxEmployees.aspx?action=AlertLOPD";
                            ShowMsgBoxForm(url, 400, 300, '');
                        }
                    }
                }
            }
        }
        function showLoadingGrid(loading) { showLoader(loading); }

        function showLoader(bolLoading) {
            //var myMask = new Ext.LoadMask(Ext.getBody(), { msgCls: "maskLoading", msg: "" });
            if (bolLoading) {
                if (typeof (LoadingPanelClient) != 'undefined') { LoadingPanelClient.Show(); }
            } else {
                if (typeof (LoadingPanelClient) != 'undefined') { LoadingPanelClient.Hide(); }
            }
        }

        //Control de redimension para ocultar el logo de la barra
        window.onresize = function () {
            CheckLogo();
        }

        function CheckLogo() {
            try {
                var strClass = 'tbd_logotitle_' + $('#hdnLiveVersion').val();
                if (strClass == 'LiveExpress') {
                    $('#vtLogoExpressDiv').innerHTML = "Live eXpress";
                } else {
                    $('#vtLogoExpressDiv').innerHTML = "Live";
                }
                if ($('#tdLogoBar').width() > $('#barMenu').width() + 280) {
                    //$("#tdLogoBar").removeClass(strClass);
                    //$("#tdLogoBar").addClass(strClass);
                } else {
                    //$("#tdLogoBar").removeClass(strClass);
                }
            } catch (e) { showError("Default::CheckLogo", e); }
        }

        function EmergencyPrint() {
            try {
                top.ShowExternalForm2('Wizards/EmergencyReports.aspx', 450, 500, '', '', false, false);
            } catch (e) { showError("Default::EmergencyPrint", e); }
        }
    </script>

    <form id="form1" runat="server" class="DefaultForm">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="True" EnableScriptLocalization="True" />

        <div id="divModalBgDefaultDisabled" class="modalBackground" style="position: absolute; left: 0px; top: 0px; z-index: 10000; width: 1680px; height: 105px; display: none;"></div>

        <div class="menuBarHeight">
            <asp:Panel ID="panMainMenu" runat="server">
            </asp:Panel>

            <rousercontrols:romainmenucontrol id="MainMenu" runat="server" />
        </div>

        <div class="DefaultHeight">
            <iframe id="ifPrincipal" name="ifPrincipal" src="about:blank" frameborder="0" runat="server" style="width: 100%; height: 100%;"></iframe>
        </div>

        <local:externalform id="externalform1" runat="server" />
        <local:msgboxform id="MsgBoxForm1" runat="server" />

        <asp:Button ID="btSignOut" runat="server" Style="display: none; visibility: hidden;" />

        <input type="hidden" id="hdnShowPage" value="" runat="server" />
    </form>

    <dx:aspxloadingpanel id="LoadingPanelMain" runat="server" clientinstancename="LoadingPanelClient" imagespacing="10" modal="True" cssclass="LoadingDiv" font-size="1.5em">
        <image url="Base/Images/Loaders/loader_v5.gif" width="80" />
        <loadingdivstyle opacity="30" backcolor="Gray" />
    </dx:aspxloadingpanel>
</body>
</html>