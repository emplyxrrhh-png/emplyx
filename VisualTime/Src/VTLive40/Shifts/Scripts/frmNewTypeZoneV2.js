function frmNewTypeZone_ShowAddListValue() {
    try {
        loadNewTypeZoneBlanks();
        //show te form
        showWUF('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmAddZone1_frmNewTypeZone', true, true);
    } catch (e) { showError("frmNewTypeZone_ShowAddListValue", e); }
}

function frmNewTypeZone_ShowEditListValue(oID) {
    try {
        if (oID == "") { return; }
        loadNewTypeZoneBlanks();

        frmNewTypeZone_AjaxLoad(oID);
    } catch (e) { showError("frmNewTypeZone_ShowEditListValue", e); }
}

function frmNewTypeZone_RemoveListValue(oID) {
    try {
        if (oID == "" || oID == "-1") { return; }

        var url = "Shifts/srvMsgBoxShifts.aspx?action=Message";
        url = url + "&TitleKey=deleteTypeZone.Title";
        url = url + "&DescriptionKey=deleteTypeZone.Description";
        url = url + "&Option1TextKey=deleteTypeZone.Option1Text";
        url = url + "&Option1DescriptionKey=deleteTypeZone.Option1Description";
        url = url + "&Option1OnClickScript=HideMsgBoxForm(); window.frames['ifPrincipal'].frmNewTypeZone_AjaxDelete('" + oID + "'); return false;";
        url = url + "&Option2TextKey=deleteTypeZone.Option2Text";
        url = url + "&Option2DescriptionKey=deleteTypeZone.Option2Description";
        url = url + "&Option2OnClickScript=HideMsgBoxForm(); return false;";
        url = url + "&IconUrl=~/Base/Images/MessageFrame/trash64.png";

        parent.ShowMsgBoxForm(url, 400, 300, '');
    } catch (e) { showError("frmNewTypeZone_RemoveListValue", e); }
}

//Validacio del formulari
function frmNewTypeZone_Validate() {
    try {
        if (txtNewZoneNameClient.GetValue() == "") {
            showErrorPopup("Error.frmNewTypeZone.txtNewZoneNameTitle", "ERROR", "Error.frmNewTypeZone.txtNewZoneNameDesc", "", "Error.frmNewTypeZone.OK", "Error.frmNewTypeZone.OKDesc", "");
            return false;
        }

        return true;
    } catch (e) {
        showError("frmNewTypeZone_Validate", e);
        return false;
    }
}

function frmNewTypeZone_Save() {
    try {
        if (document.getElementById('ctl00_contentMainBody_hdnModeEdit').value != 'false') { frmNewTypeZone_Close; return; }
        if (frmNewTypeZone_Validate()) {
            frmNewTypeZone_AjaxSave();
        }
    } catch (e) { showError("frmNewTypeZone_Save", e); }
}

function frmNewTypeZone_Close() {
    try {
        //abans de res, recarregar el combo
        frmNewTypeZone_reloadCombo();
    } catch (e) { showError("frmNewTypeZone_Close", e); }
}

function loadNewTypeZoneBlanks() {
    try {
        var objPrTab = "ctl00_contentMainBody_ASPxCallbackPanelContenido_frmNewTypeZone_";
        var oDis = document.getElementById('ctl00_contentMainBody_hdnModeEdit').value;
        //blank fields
        loadGenericData("ZONEID", "hdnNewTypeZoneLayer", "-1", "X_NUMBER", "", oDis, false);

        txtNewZoneNameClient.SetValue("");
        txtNewZoneDescClient.SetValue("");
    } catch (e) { showError("loadNewTypeZoneBlanks", e); }
}

function frmNewTypeZone_AjaxSave() {
    try {
        var IDTypeZone = document.getElementById('hdnNewTypeZoneLayer').value;
        var Name = txtNewZoneNameClient.GetValue();
        var Desc = txtNewZoneDescClient.GetValue();

        var oFields = "IDTypeZone=" + IDTypeZone + "&Name=" + encodeURIComponent(Name) + "&Desc=" + encodeURIComponent(Desc);

        AsyncCall("POST", "Handlers/srvShiftsv2.ashx?action=updateTypeZone&" + oFields, "json", "arrStatus", "checkShiftStatus(arrStatus); if(arrStatus[0].error == 'false'){ hasChanges(true); frmNewTypeZone_Close(); }");
    } catch (e) { showError("frmNewTypeZone_AjaxSave", e); }
}

function frmNewTypeZone_AjaxLoad(oId) {
    try {
        var oFields = "IDTypeZone=" + oId;

        AsyncCall("POST", "Handlers/srvShiftsv2.ashx?action=loadTypeZone&" + oFields, "json", "arrFields", "frmNewTypeZone_loadValues(arrFields);");
    } catch (e) { showError("frmNewTypeZone_AjaxLoad", e); }
}

function frmNewTypeZone_AjaxDelete(oId) {
    try {
        var oFields = "IDTypeZone=" + oId;

        AsyncCall("POST", "Handlers/srvShiftsv2.ashx?action=deleteTypeZone&" + oFields, "json", "arrFields", "frmNewTypeZone_Close();");
    } catch (e) { showError("frmNewTypeZone_AjaxDelete", e); }
}

function frmNewTypeZone_loadValues(oArr) {
    try {
        var arrControls = oArr;

        if (arrControls[0].error == "true") {
            checkShiftStatus(arrControls);
            return;
        }
        var oDis = document.getElementById('ctl00_contentMainBody_hdnModeEdit').value;

        var n;
        for (n = 0; n < arrFields.length; n++) {
            var fieldName = arrFields[n].field.toUpperCase();
            var controls = arrFields[n].control;
            var value = arrFields[n].value;
            var typeControl = arrFields[n].type.toUpperCase();
            var list = arrFields[n].list;

            if (fieldName == "NAME") {
                txtNewZoneNameClient.SetValue(value);
            } else if (fieldName == "DESCRIPTION") {
                txtNewZoneDescClient.SetValue(value);
            } else {
                loadGenericData(fieldName, controls, value, typeControl, list, oDis, false);
            }
        }
        showWUF('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmAddZone1_frmNewTypeZone', true, true);
    } catch (e) { showError("frmNewTypeZone_loadValues", e); }
}

function frmNewTypeZone_reloadCombo() {
    try {
        AsyncCall("POST", "Handlers/srvShiftsv2.ashx?action=reloadTypeZones", "json", "arrFields", "frmNewTypeZone_loadCombo(arrFields);");
    } catch (e) { showError("frmNewTypeZone_reloadCombo", e); }
}

function frmNewTypeZone_loadCombo(arrFields) {
    try {
        var arrControls = arrFields;

        if (arrControls[0].error == "true") {
            checkShiftStatus(arrControls);
            return;
        }
        var oDis = document.getElementById('ctl00_contentMainBody_hdnModeEdit').value;

        var n;
        for (n = 0; n < arrFields.length; n++) {
            var fieldName = arrFields[n].field.toUpperCase();
            var controls = arrFields[n].control;
            var value = arrFields[n].value;
            var typeControl = arrFields[n].type.toUpperCase();
            var list = arrFields[n].list;

            if (fieldName == "CMBTYPEZONE") {
                if (list != "") {
                    cmbTypeAddZoneClient.ClearItems();

                    var cbItems = list.split("|*|");
                    var nItems;
                    //Recuperem els items
                    for (nItems = 0; nItems < cbItems.length; nItems++) {
                        var cbParms = cbItems[nItems].split("~*~");
                        if (cbParms.length == 3) { //Texte, Valor, funcio JS
                            cmbTypeAddZoneClient.AddItem(cbParms[0], cbParms[1]);
                        } else if (cbParms.length == 2) { //Texte, valor
                            cmbTypeAddZoneClient.AddItem(cbParms[0], cbParms[1]);
                        } else if (cbParms.length == 1) { //Texte
                            cmbTypeAddZoneClient.AddItem(cbParms[0], cbParms[0]);
                        }
                    }
                }
            } else if (fieldName == "CMBRULEZONE1") {
                if (list != "") {
                    cmbRuleZone1Client.ClearItems();

                    var cbItems = list.split("|*|");
                    var nItems;
                    //Recuperem els items
                    for (nItems = 0; nItems < cbItems.length; nItems++) {
                        var cbParms = cbItems[nItems].split("~*~");
                        if (cbParms.length == 3) { //Texte, Valor, funcio JS
                            cmbRuleZone1Client.AddItem(cbParms[0], cbParms[1]);
                        } else if (cbParms.length == 2) { //Texte, valor
                            cmbRuleZone1Client.AddItem(cbParms[0], cbParms[1]);
                        } else if (cbParms.length == 1) { //Texte
                            cmbRuleZone1Client.AddItem(cbParms[0], cbParms[0]);
                        }
                    }
                }
            }
        }

        showWUF('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmAddZone1_frmNewTypeZone', false, true);
    } catch (e) { showError("frmNewTypeZone_loadCombo", e); }
}