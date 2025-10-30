<%@ Control Language="VB" AutoEventWireup="false" Inherits="VTLive40.WebUserControls_MsgBoxForm" CodeBehind="MsgBoxForm.ascx.vb" %>

<script language="javascript" type="text/javascript">
    function ShowMsgBoxForm(Url, Width, Height, title, parametername, parametervalue) {
        ajax = nuevoAjax();
        ajax.open("GET", Url, true);

        ajax.onreadystatechange = function () {
            if (ajax.readyState == 4) {
                //Carrega el div
                var oFrame = $get("<%=MsgBoxFormFrame.clientid%>");
                var oLabelTitle = $get("<%=Title_Label.clientid %>");
                var oRoPopupFrame2 = $get("<%=RoPopupFrameMsgBoxForm.clientid %>");

                //oRoPopupFrame2.title = title;
                oLabelTitle.textContent = title;

                oFrame.setAttribute('width', Width + "px");
                oFrame.setAttribute('height', Height + "px");
                oFrame.innerHTML = ajax.responseText;

                var oFrame = $get("<%=RoPopupFrameMsgBoxForm.clientid%>");
                oFrame.setAttribute('width', (Width + 10) + "px");
                oFrame.setAttribute('height', Height + "px");
                oFrame.style.Width = (Width + 10) + "px";
                oFrame.style.Height = Height + "px";

                $('#RoPopupFrameMsgBoxFormMyPopupFrame_DIV').css("width", (Width + 10) + "px");

                var oCloseButton = $get("<%=ControlBox_Close.clientid%>");
                oCloseButton.style.display = 'none';

                var oPopupButton = $get("<%=ControlBox_Popup.clientid%>");
                oPopupButton.style.display = 'none';

                var popup = $find('ModalPopupMsgBoxFormBehavior');

                popup._xCoordinate = (screen.width / 2) - (Width / 2);
                popup._yCoordinate = (screen.height / 2) - (Height / 2);

                showPopup("ModalPopupMsgBoxFormBehavior");

                zIndex = 19000;
                setTimeout(function () {
                    $('#RoPopupFrameMsgBoxFormMyPopupFrame_DIV').css("zIndex", zIndex + 1);
                    $('#ModalPopupMsgBoxFormBehavior_backgroundElement').css("zIndex", zIndex);
                }, 500);

                try {
                    $removeHandler(document, "keydown", onKeyDown);
                } catch (e) { }
                $addHandler(document, "keydown", onKeyDown);
            }
        }

        ajax.send(null)

    }

    function ShowMsgBoxForm2(Url, Width, Height, title, parameters, closebutton_visible, popupbutton_visible, popupresizable) {
        if (parameters != '') Url = Url + '?' + parameters;

        ShowMsgBoxForm(Url, Width, Height, title, '', '');

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
            oPopupButton.onclick = function () { OpenPopup(Url, Width, Height, popupresizable); HideMsgBoxForm(); };
        }
        else
            oPopupButton.style.display = 'none';
    }

    function ShowMsgBoxFormPosition(Url, Width, Height, X, Y, title, parametername, parametervalue) {

        var popup = $find('ModalPopupMsgBoxFormBehavior');
        popup._xCoordinate = X;
        popup._yCoordinate = Y;

        ShowMsgBoxForm(Url, Width, Height, title, parametername, parametervalue);

    }

    function HideMsgBoxForm() {

        //PPR
        var oFrame = $get("<%=RoPopupFrameMsgBoxForm.clientid%>");
        oFrame.removeAttribute('width');
        oFrame.removeAttribute('height');
        oFrame.style.Width = "";
        oFrame.style.Height = "";

        var oFrame = $get("<%=MsgBoxFormFrame.clientid%>");
        //oFrame.setAttribute('src', '<%=Page.ResolveUrl("~/Base/BlankPage.aspx")%>');
        oFrame.innerHTML = '';

        hidePopup('ModalPopupMsgBoxFormBehavior');

        $removeHandler(document, "keydown", onKeyDown);

    }

    function onKeyDown(e) {
        if (e && e.keyCode == Sys.UI.Key.esc) {
            try {
                //window.frames['<%= MsgBoxFormFrame.ClientID %>'].Close();
            } catch (ex) { }
        }
    }

    function IdentifyRestorePwdConfirmation() {
        try {
            HideMsgBoxForm();

            let executed = 0;
            for (var index = 0; index < window.frames.length; index++) {

                try {
                    if (typeof (window.frames[index].RestorePwd) == 'function') {
                        executed = 1;
                        window.frames[index].RestorePwd();
                    }
                } catch (e1) {}

                
            }
            if (executed === 0) {
                for (var index = 0; index < window.frames.length; index++) {
                    try {
                        if (typeof (window.frames[index][0].RestorePwd) == 'function') window.frames[index][0].RestorePwd();
                    } catch (e2) { }
                    
                }
            }
        } catch (e) {
            parent.showLoader(false);
        }
    }

    function resetCegidIDUserConfirmation() {
        try {
            HideMsgBoxForm();

            let executed = 0;
            for (var index = 0; index < window.frames.length; index++) {
                try {
                    if (typeof (window.frames[index].RestoreCegidID) == 'function') {
                        executed = 1;
                        window.frames[index].RestoreCegidID();
                    }
                } catch (e1) { }
                
            }
            if (executed === 0) {
                for (var index = 0; index < window.frames.length; index++) {
                    try {
                        if (typeof (window.frames[index][0].RestorePwd) == 'function') window.frames[index][0].RestoreCegidID();
                    } catch (e2) { }
                    
                }
            }
        } catch (e) {
            parent.showLoader(false);
        }
    }

    function IdentifySendUsernameConfirmation() {
        try {
            HideMsgBoxForm();

            let executed = 0;
            for (var index = 0; index < window.frames.length; index++) {
                try {
                    if (typeof (window.frames[index].SendUsername) == 'function') {
                        executed = 1;
                        window.frames[index].SendUsername();
                    }
                } catch (e1) { }
                
            }
            if (executed === 0) {
                for (var index = 0; index < window.frames.length; index++) {
                    try {
                        if (typeof (window.frames[index][0].SendUsername) == 'function') window.frames[index][0].SendUsername();
                    } catch (e2) { }
                    
                }
            }
        } catch (e) {
            parent.showLoader(false);
        }
    }

    function DisableBiometricDataConfirmation() {
    try {
        HideMsgBoxForm();

        let executed = 0;
        for (var index = 0; index < window.frames.length; index++) {
            try {
                if (typeof (window.frames[index].SaveChanges) == 'function') {
                    executed = 1;
                    window.frames[index].SaveChanges();
                }
            } catch (e1) { }
            
        }
        if (executed === 0) {
            for (var index = 0; index < window.frames.length; index++) {
                try {
                    if (typeof (window.frames[index][0].SaveChanges) == 'function') window.frames[index][0].SaveChanges();
                } catch (e2) { }
                
            }
        }
    } catch (e) {
        parent.showLoader(false);
    }
    }

    function DisableUnregisterA3PayrollConfirmation() {
            HideMsgBoxForm();
    }

    function DeleteBiometricDataConfirmation() {        
try {
    HideMsgBoxForm();

    let executed = 0;
    for (var index = 0; index < window.frames.length; index++) {
        try {
            if (typeof (window.frames[index].DeleteBiometricData) == 'function') {
                executed = 1;
                window.frames[index].DeleteBiometricData();
            }
        } catch (e1) { }
        
    }
    if (executed === 0) {
        for (var index = 0; index < window.frames.length; index++) {
            try {
                if (typeof (window.frames[index][0].DeleteBiometricData) == 'function') window.frames[index][0].DeleteBiometricData();
            } catch (e2) { }
            
        }
    }
} catch (e) {
    parent.showLoader(false);
}
}
</script>

<roWebControls:roPopupFrameV2 ID="RoPopupFrameMsgBoxForm" runat="server" ShowTitleBar="true" BehaviorID="ModalPopupMsgBoxFormBehavior">
    <FrameContentTemplate>
        <img id="ControlBox_Popup" style="cursor: pointer; margin-left: -35px; position: absolute; display: none;" src="~/Base/Images/btnNewWindow.png" runat="server" />
        <img id="ControlBox_Close" onclick='HideMsgBoxForm();' style="cursor: pointer; margin-left: -20px; position: absolute;" src="~/Base/Images/btnClose.png" runat="server" />
        <asp:Label ID="Title_Label" Text="label titulo" runat="server" Style="position: absolute;" />
        <div id="MsgBoxFormFrame" runat="server" style="width: auto; height: auto;"></div>
    </FrameContentTemplate>
</roWebControls:roPopupFrameV2>