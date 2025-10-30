let stdWorkingMode = 'advRule';
let actualShift = -1;
let onAcceptRule;
let onCancelEdition;
let onGetRuleDefinition;


function onAdvancedRulePopupShown(oDailyRuleDefinition) {
    frmDailyRule_Show(onGetRuleDefinition());
}

function setGetCurrentRuleDefinition(onGetCallback) {
    onGetRuleDefinition = onGetCallback;
}

function setOnCancelEdition(onCancelCallback) {
    onCancelEdition = onCancelCallback;
}

function setOnSaveRuleEdition(onSaveCallback) {
    onAcceptRule = onSaveCallback;
}

function showErrorPopup(Title, typeIcon, DescriptionKey, DescriptionText, Opt1Text, Opt1Desc, strScript1, Opt2Text, Opt2Desc, strScript2, Opt3Text, Opt3Desc, strScript3) {
    try {
        var url = "/Shifts/srvMsgBoxShifts.aspx?action=Message";
        url = url + "&TitleKey=" + Title;
        if (DescriptionKey != "") url = url + "&DescriptionKey=" + DescriptionKey;
        if (DescriptionText != "") url = url + "&DescriptionText=" + DescriptionText;
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

        parent.parent.ShowMsgBoxForm(url, 400, 300, '');
    } catch (e) { showError("showErrorPopup", e); }
}

