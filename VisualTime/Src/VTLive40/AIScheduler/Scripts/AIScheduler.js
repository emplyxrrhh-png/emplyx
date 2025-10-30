var actualTab = 0;

function changeTabs(numTab) {
    arrButtons = new Array('TABBUTTON_00', 'TABBUTTON_01');
    arrDivs = new Array('ctl00_contentMainBody_ASPxCallbackPanelContenido_div00', 'ctl00_contentMainBody_ASPxCallbackPanelContenido_div01');

    for (n = 0; n < arrButtons.length; n++) {
        var tab = document.getElementById(arrButtons[n]);
        var div = document.getElementById(arrDivs[n]);
        if (n == numTab) {
            if (tab != null) { tab.className = 'bTab-active'; }
            if (div != null) { div.style.display = ''; }
        } else {
            if (tab != null) { tab.className = 'bTab'; }
            if (div != null) { div.style.display = 'none'; }
        }
    }
    actualTab = numTab;
}

function changeTabsByName(nameTab) {
    arrButtons = new Array('TABBUTTON_00', 'TABBUTTON_01');
    arrDivs = new Array('ctl00_contentMainBody_ASPxCallbackPanelContenido_div00', 'ctl00_contentMainBody_ASPxCallbackPanelContenido_div01');

    for (n = 0; n < arrButtons.length; n++) {
        var tab = document.getElementById(arrButtons[n]);
        var div = document.getElementById(arrDivs[n]);
        if (div.id == nameTab) {
            if (tab != null) { tab.className = 'bTab-active'; }
            if (div != null) { div.style.display = ''; }
            actualTab = n;
        } else {
            if (tab != null) { tab.className = 'bTab'; }
            if (div != null) { div.style.display = 'none'; }
        }
    }
}

function hasChanges(bolChanges, markRecalc) {
    var divTop = document.getElementById('divMsgTop');
    var divBottom = document.getElementById('divMsgBottom');

    var tagHasChanges = document.getElementById('msgHasChanges');
    var msgChanges = '<changes>';
    if (tagHasChanges != null) {
        msgChanges = tagHasChanges.value;
    }

    setMessage(msgChanges);

    if (bolChanges) {
        divTop.style.display = '';
        divBottom.style.display = '';
        document.getElementById('divContentPanels').className = "divContentPanelsWithMessage";
    } else {
        divTop.style.display = 'none';
        divBottom.style.display = 'none';
        document.getElementById('divContentPanels').className = "divContentPanelsWithOutMessage";
    }
}

function setMessage(msg) {
    try {
        var msgTop = document.getElementById('msgTop');
        var msgBottom = document.getElementById('msgBottom');
        msgTop.textContent = msg;
        msgBottom.textContent = msg;
    } catch (e) { alert('setMessage: ' + e); }
}

function ShowReports(Title, ReportsTitle, ReportsType, DefaultReportsVersion, RootURL) {
    if (DefaultReportsVersion == 1) {
        if (ReportsTitle != '') Title = Title + ' - ' + ReportsTitle;
        parent.ShowExternalForm('Reports/Reports.aspx', 900, 570, Title, 'ReportsType', ReportsType);
    } else {
        parent.reenviaFrame('/' + RootURL + '//Report', '', 'Reports', 'Portal\Reports\AdvReport');
    }
}

function showTbTip(tip) {
    document.getElementById(tip).style.display = '';
}

function hideTbTip(tip) {
    document.getElementById(tip).style.display = 'none';
}

function showLoadingGrid(loading) { parent.showLoader(loading); }

function showErrorPopup(Title, typeIcon, Description, DescriptionText, Opt1Text, Opt1Desc, strScript1, Opt2Text, Opt2Desc, strScript2, Opt3Text, Opt3Desc, strScript3) {
    try {
        var url = "AIScheduler/srvMsgBoxAIScheduler.aspx?action=Message";
        url = url + "&TitleKey=" + Title;

        if (Description != "") { url = url + "&DescriptionKey=" + Description; }
        else { url = url + "&DescriptionText=" + DescriptionText; }

        url = url + "&Option1TextKey=" + Opt1Text;
        url = url + "&Option1DescriptionKey=" + Opt1Desc;
        url = url + "&Option1OnClickScript=HideMsgBoxForm();" + strScript1 + "; return false;";
        if (Opt2Text != null) {
            url = url + "&Option2TextKey=" + Opt2Text;
            url = url + "&Option2DescriptionKey=" + Opt2Desc;
            url = url + "&Option2OnClickScript=HideMsgBoxForm();" + strScript2 + "; return false;";
        }
        if (Opt3Text != null) {
            url = url + "&Option3TextKey=" + Opt3Text;
            url = url + "&Option3DescriptionKey=" + Opt3Desc;
            url = url + "&Option3OnClickScript=HideMsgBoxForm();" + strScript3 + "; return false;";
        }
        if (typeIcon.toUpperCase() == "TRASH") {
            url = url + "&IconUrl=~/Base/Images/MessageFrame/trash64.png";
        } else if (typeIcon.toUpperCase() == "ERROR") {
            url = url + "&IconUrl=~/Base/Images/MessageFrame/alert32.png";
        } else if (typeIcon.toUpperCase() == "INFO") {
            url = url + "&IconUrl=~/Base/Images/MessageFrame/dialog-information.png";
        }

        parent.ShowMsgBoxForm(url, 400, 300, '');
    } catch (e) { showError("showErrorPopup", e); }
}