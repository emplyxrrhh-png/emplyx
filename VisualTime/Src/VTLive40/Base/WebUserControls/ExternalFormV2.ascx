<%@ Control Language="VB" AutoEventWireup="false" Inherits="VTLive40.WebUserControls_ExternalFormV2" CodeBehind="ExternalFormV2.ascx.vb" %>

<script language="javascript" type="text/javascript">

    var lastTitle = "";
    var lastVisibleClose = "";
    var lastVisiblePopup = "";

    function ShowExternalForm(Url, Width, Height, title, parametername, parametervalue, zIndex) {

        try {
            if (typeof (window.frames['ifPrincipal'].roScheduleCalendar) != 'undefined' && window.frames['ifPrincipal'].roScheduleCalendar != null) {
                window.frames['ifPrincipal'].roScheduleCalendar.isShowingDialog = true;
            }
        } catch (e) {

        }

        lastTitle = title;
        lastVisibleClose = true;
        lastVisiblePopup = false;

        if (typeof (zIndex) == "undefined" || zIndex == null) zIndex = -1;

        var oRoPopupFrame2 = $get("<%=RoPopupFrameExternalForm.clientid %>");
        oRoPopupFrame2.style.minHeight = (parseInt(Height, 10) + 10) + "px";
        oRoPopupFrame2.style.minWidth = (parseInt(Width, 10)) + "px";

        var oFrame = $get("<%=ExternalFormFrame.clientid%>");

        if (parametername == "ReportsType") {
            oFrame.style.minHeight = Height - 30 + "px";
        }
        else {
            oFrame.style.minHeight = Height + 20 + "px";
        }
        oFrame.style.minWidth = Width + "px";
        oFrame.onload = function () {
            checkTitleVisibility();
        }

        var oCloseButton = $get("<%=ControlBox_Close.clientid%>");
        oCloseButton.style.display = '';

        var oPopupButton = $get("<%=ControlBox_Popup.clientid%>");
        oPopupButton.style.display = 'none';

        var mainDiv = $get('RoPopupFrameExternalFormMyPopupFrame_DIV');
        if (zIndex > 0) {

            setTimeout(function () {
                $('#RoPopupFrameExternalFormMyPopupFrame_DIV').css("zIndex", zIndex + 1);
                $('#ModalPopupExternalFormBehavior_backgroundElement').css("zIndex", zIndex);
            }, 500);

        }
        mainDiv.style.height = (parseInt(Height, 10) + 20) + "px";
        mainDiv.style.width = (parseInt(Width, 10)) + "px";

        var src;
        if (parametername != "") {
            src = Url;
            if (src.indexOf('?') == -1) {
                src = src + '?';
            } else {
                src = src + '&';
            }
            src = src + parametername + "=" + parametervalue;
        } else {
            src = Url;
        }
        if (src.indexOf('?') == -1)
            src = src + '?';
        else
            src = src + '&';
        src = src + 'StampParam=' + new Date().getMilliseconds();
        oFrame.setAttribute('src', src);

        showPopup("ModalPopupExternalFormBehavior");

        try {
            $removeHandler(document, "keydown", onKeyDown);
        } catch (e) { }
        $addHandler(document, "keydown", onKeyDown);

        var oHeaderText = $get("<%=lblTitleText.ClientID%>");
        if (title != "") {
            oHeaderText.textContent = title;
        }

        var mainDiv = $get('RoPopupFrameExternalFormMyPopupFrame_DIV');
        var titleRow = mainDiv.childNodes[1].childNodes[1];
        titleRow.style.display = "none";

    }

    function ShowExternalForm2(Url, Width, Height, title, parameters, closebutton_visible, popupbutton_visible, popupresizable, zIndex) {

        try {
            if (typeof (window.frames['ifPrincipal'].roScheduleCalendar) != 'undefined' && window.frames['ifPrincipal'].roScheduleCalendar != null) {
                window.frames['ifPrincipal'].roScheduleCalendar.isShowingDialog = true;
            }
        } catch (e) {

        }

        if (parameters != '') Url = Url + '?' + parameters;

        ShowExternalForm(Url, Width, Height, title, '', '', zIndex);

        lastVisibleClose = closebutton_visible;
        lastVisiblePopup = popupbutton_visible;

        var oCloseButton = $get("<%=ControlBox_Close.clientid%>");
        if (closebutton_visible == true)
            oCloseButton.style.display = '';
        else
            oCloseButton.style.display = 'none';

        var oPopupButton = $get("<%=ControlBox_Popup.clientid%>");
        if (popupbutton_visible == true) {
            oPopupButton.style.display = '';
            if (parameters != '') {
                Url = Url + "&";
            }
            else
                Url = Url + '?';
            Url = Url + 'IsPopup=true';
            oPopupButton.onclick = function () { OpenPopup(Url, Width, Height, popupresizable); HideExternalForm(); };
        }
        else
            oPopupButton.style.display = 'none';
    }

    function checkTitleVisibility() {
        if (!(lastTitle == "" && lastVisibleClose == false && lastVisiblePopup == false)) {
            var mainDiv = $get('RoPopupFrameExternalFormMyPopupFrame_DIV');
            var titleRow = mainDiv.childNodes[1].childNodes[1];
            titleRow.style.display = "";
        }
    }

    function ShowExternalFormPosition(Url, Width, Height, X, Y, title, parametername, parametervalue) {

        var popup = $find('ModalPopupExternalFormBehavior');
        popup._xCoordinate = X;
        popup._yCoordinate = Y;

        ShowExternalForm(Url, Width, Height, title, parametername, parametervalue);

    }

    function HideExternalForm() {

        try {
            if (typeof (window.frames['ifPrincipal'].roScheduleCalendar) != 'undefined' && window.frames['ifPrincipal'].roScheduleCalendar != null) {
                window.frames['ifPrincipal'].roScheduleCalendar.isShowingDialog = false;
            }
        } catch (e) {

        }

        //PPR
        var oFrame = $get("<%=RoPopupFrameExternalForm.clientid%>");
        oFrame.removeAttribute('width');
        oFrame.removeAttribute('height');
        oFrame.style.Width = "";
        oFrame.style.Height = "";

        var oFrame = $get("<%=ExternalFormFrame.clientid%>");
        oFrame.setAttribute('src', '<%=Page.ResolveUrl("~/Base/BlankPage.aspx")%>');
        //oFrame.setAttribute('src', 'about:blank');

        hidePopup('ModalPopupExternalFormBehavior');

        $removeHandler(document, "keydown", onKeyDown);

    }

    function onKeyDown(e) {
        if (e && e.keyCode == Sys.UI.Key.esc) {
            try {
                window.frames['<%= ExternalFormFrame.ClientID %>'].Close();
            } catch (ex) { }
        } else if (e && e.keyCode == Sys.UI.Key.enter) {
            try {
                alert('enter');
            } catch (ex) { }
        }
    }
</script>

<roWebControls:roPopupFrameV2 ID="RoPopupFrameExternalForm" runat="server" ShowTitleBar="true" BehaviorID="ModalPopupExternalFormBehavior">
    <FrameContentTemplate>

        <div id="DivTitle_Bar" class="popupDefaultButtons">
            <div style="min-height: 5px"></div>
            <div class="panHeaderWizard">
                <div style="min-height: 5px"></div>
                <asp:Label ID="lblTitleText" CssClass="panelTitleSpanLow" Text="label titulo" runat="server" Style="">&nbsp;</asp:Label>
                <div class="headerButtonsCell">
                    <img alt="" id="ControlBox_Popup" style="display: none; cursor: pointer" src="~/Base/Images/btnNewWindow.png" runat="server" />
                    <img alt="" id="ControlBox_Close" onclick='HideExternalForm();' style="cursor: pointer" src="~/Base/Images/btnClose.png" runat="server" />
                </div>
                <div style="min-height: 5px"></div>
            </div>
        </div>

        <iframe id="ExternalFormFrame" runat="server" class="popupInsideFrame" scrolling="no" frameborder="0" marginheight="0" marginwidth="0" />
    </FrameContentTemplate>
</roWebControls:roPopupFrameV2>