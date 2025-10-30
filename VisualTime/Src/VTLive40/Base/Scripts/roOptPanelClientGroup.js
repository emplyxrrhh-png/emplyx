/* linkItems: Enllaça els controls OptionPanelClient *************************/
/* oItems -> ClientID dels Controls OptionPanelClient separats per comes (,) */
/*****************************************************************************/
function linkOPCItems(oItems, oGroup) {
    try {
    var arrItems = new Array();
    arrItems = oItems.split(",");
    var n;
    for (n = 0; n < arrItems.length; n++) {
        var oPanel = document.getElementById(arrItems[n] + '_panOptionPanel');
        var oButton;
        var oText = document.getElementById(arrItems[n] + '_aTitle');
        var oDesc = document.getElementById(arrItems[n] + '_aDescription');
        if (oPanel == null) { return; }
        switch (oPanel.getAttribute("vmode")) {
            case "0": //RadioButton
                oButton = document.getElementById(arrItems[n] + '_rButton');
                break;
            case "1": //CheckBox
                oButton = document.getElementById(arrItems[n] + '_chkButton');
                break;
        }

        if (window.addEventListener) { // Mozilla, Netscape, Firefox
        
            AddScript(oButton.id, "onclick", "chgOPCItems('" + n + "','" + oItems + "','" + oGroup + "');");
            oButton.setAttribute("onclick", "chgOPCItems('" + n + "','" + oItems + "','" + oGroup + "');");
            
            AddScript(oText.id, "onclick", "CheckLinkClick('" + oButton.id + "');",true);
            AddScript(oDesc.id, "onclick", "CheckLinkClick('" + oButton.id + "');",true);

        }
        else { // IE

            on_click = new Function("chgOPCItems('" + n + "','" + oItems + "','" + oGroup + "');");
            oButton.attachEvent('onclick', on_click);

            on_click2 = new Function("CheckLinkClick('" + oButton.id + "');");
            oText.attachEvent('onclick', on_click2);
            oDesc.attachEvent('onclick', on_click2);
            
            }

    }
} catch (e) { showError("roOptPanelClientGroup:linkOPCItems", e); }       
}

/* Funcio q controla el canvi dels items */
function chgOPCItems(oIndex, oItems, oGroupID, doClick) {
    var mustThrowClick = true;

    if (typeof doClick != 'undefined') mustThrowClick = doClick;

    try {
    var arrItems = new Array();
    arrItems = oItems.split(",");
    var n;
    for (n = 0; n < arrItems.length; n++) {
        var oPanel = document.getElementById(arrItems[n] + '_panOptionPanel');
        if (oPanel == null) { return; }
        if(oPanel.getAttribute("venabled") == "False"){ continue; }
        var oGroup = document.getElementById(oGroupID);
        var oButton;
        switch (oPanel.getAttribute("vmode")) {
            case "0": //RadioButton
                oButton = document.getElementById(arrItems[n] + '_rButton');
                break;
            case "1": //CheckBox
                oButton = document.getElementById(arrItems[n] + '_chkButton');
                break;
        }

        if (n == oIndex) {
            oButton.checked = true;
            if (oGroup != null) {
                if (oPanel.getAttribute("value") == "") {
                    oGroup.setAttribute("value", "0");
                } else {
                    oGroup.setAttribute("value", oPanel.getAttribute("value"));
                }
            }
            setStyleOPC(arrItems[n], true);
            if(mustThrowClick) clickOPC(oButton, arrItems[n])

        } else {
            oButton.checked = false;
            setStyleOPC(arrItems[n], false);
            if (mustThrowClick) clickOPC(oButton, arrItems[n])
        }
        if (mustThrowClick && oButton.getAttribute('cconChange') != null && oButton.getAttribute('cconChange') != '') {
            eval(oButton.getAttribute('cconChange'));
        }
    }
} catch (e) { showError("roOptPanelClientGroup:chgOPCItems", e); }
}