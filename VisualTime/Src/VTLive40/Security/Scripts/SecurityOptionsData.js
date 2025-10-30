function SecurityOptionsData(oId) {
    var arrControls;

    var arrStatus;
    var arrData;

    var SecurityOptionsID; //ID actual
    var SecurityOptionsIDName = 'ID';

    //*****************************************************************************************/
    // getSecurityOptionsById
    // Recupera el objecte SecurityOptions per ID
    //********************************************************************************************/
    this.getSecurityOptionsById = function (oId) {
        try {
            if (jsGridIPs != null) { jsGridIPs.destroyGrid(); }
            showLoadingGrid(true);

            if (oId != null) {
                SecurityOptionsID = oId;
                AsyncCall("POST", "Handlers/srvSecurityOptions.ashx?action=getXSecurityOptions&ID=" + oId, "json", "arrControls", "loadSecurityOptions(arrControls);")
            } else {
                if (TaskTemplateID != null) {
                    AsyncCall("POST", "Handlers/srvSecurityOptions.ashx?action=getXSecurityOptions&ID=" + oId, "json", "arrControls", "loadSecurityOptions(arrControls);")
                }
            }
        } catch (e) { alert('getSecurityOptionsById: ' + e.description); }
    }         //end function

    //*****************************************************************************************/
    // new newSecurityOptions
    // Recupera nou objecte SecurityOptions (id = -1)
    //********************************************************************************************/
    this.newSecurityOptions = function () {
        try {
            if (jsGridIPs != null) { jsGridIPs.destroyGrid(); }
            AsyncCall("POST", "Handlers/srvSecurityOptions.ashx?action=newSecurityOptions", "json", "arrControls", "loadSecurityOptions(arrControls);");
        } catch (e) { alert('newSecurityOptions: ' + e.description); }
    }    //end function

    //*****************************************************************************************/
    // loadSecurityOptions
    // Carrega els camps
    //********************************************************************************************/
    loadSecurityOptions = function (arrFields) {
        try {
            //Carreguem el array global per mantenir els valors
            var arrIPsGrid = [];

            if (arrFields != null) {
                if (typeof (arrFields[0].error) != "undefined") {
                    if (arrFields[0].error == "true") {
                        checkStatus(arrFields);
                        return;
                    }
                }

                if (arrFields.length == 1) {
                    arrControls = arrFields[0];
                }
                else {
                    arrControls = arrFields[0][0];
                }

                oDis = false;

                if (arrFields.length == 1) {
                    arrFields = arrFields[0];
                }
                else {
                    arrIPsGrid = arrFields[1][0];
                    arrFields = arrFields[0][0];
                }
            } else {
                arrFields = arrControls;
            }

            var n;
            for (n = 0; n < arrFields.length; n++) {
                var fieldName = arrFields[n].field.toUpperCase();
                var controls = arrFields[n].control;
                var value = arrFields[n].value;
                var typeControl = arrFields[n].type.toUpperCase();
                var list = arrFields[n].list;
                var disabled = arrFields[n].disable;

                //Si es el ID, asignem
                if (fieldName == SecurityOptionsIDName.toUpperCase()) {
                    SecurityOptionsID = value;
                }

                var strList = "";
                if (list != null) {
                    for (nl = 0; nl < list.length; nl++) {
                        strList += list[n] + ","
                    }
                }

                var strControls = "";
                if (controls != null) {
                    for (nc = 0; nc < controls.length; nc++) {
                        strControls += controls[nc] + ","
                    }
                }

                var strDisable = "false";
                if (disabled) { strDisable = "true"; } else { strDisable = "false"; }

                var bHasChanges = true;
                var fnHC = '';
                if (fieldName.toUpperCase() == "NAME" ||
                    fieldName.toUpperCase() == "SHORTNAME" ||
                    fieldName.toUpperCase() == "DESCRIPTION" ||
                    fieldName.toUpperCase() == "ADVANCEDPARAMETERS" ||
                    fieldName.toUpperCase() == "ISOBSOLETE" ||
                    fieldName.toUpperCase() == "ISTEMPLATE" ||
                    fieldName.toUpperCase() == "IDGROUP" ||
                    fieldName.toUpperCase() == "SHIFTTYPE") {
                    fnHC = "hasChanges(true, false);"
                } else {
                    fnHC = "hasChanges(true);";
                }

                loadGenericData(fieldName, strControls, value, typeControl, strList, strDisable, bHasChanges, fnHC);
            } //end for

            showLoadingGrid(false);
            hasChanges(false);
            top.focus();

            //Crear grid de DocumentAdvice
            createSecurityOptionsIPsGrid(arrIPsGrid);
        } catch (e) { showError('loadTaskTemplate', e); }
    }    //end loadSecurityOptions function

    loadIPsGrid = function (paramObj) {
        try {
            getSecurityOptionsIPsGrids(paramObj);
        }
        catch (e) { showError('loadIPsGrid', e); }
    }

    //*****************************************************************************************/
    // createSecurityOptionsIPsGrid
    // Carrega del Grids (ips permeses)
    //********************************************************************************************/
    createSecurityOptionsIPsGrid = function (arrCC) {
        try {
            arrGrids = arrCC;

            if (arrGrids == null) { return; }

            //Creació dels grids
            createGridSecurityOptionsIP(arrGrids);

            showLoadingGrid(false);
        } catch (e) { showError('createSecurityOptionsIPsGrid', e); }
    }

    //*****************************************************************************************/
    // createGridSecurityOptionsIP
    // Carrega les ips permeses
    //********************************************************************************************/
    createGridSecurityOptionsIP = function (arrGridIPs) {
        try {
            //Grid user fields
            var hdGridIPs = [{ 'fieldname': 'value', 'description': '', 'size': '100%' }];
            hdGridIPs[0].description = document.getElementById('ctl00_contentMainBody_hdnValueGridName').value;
            jsGridIPs = new jsGrid('ctl00_contentMainBody_groupAccessRestrictions_ChkRestrictedIP_gridAllowedIPs', hdGridIPs, arrGridIPs, true, true, false, 'AllowedIPs');
        } catch (e) { showError("createGridSecurityOptionsIP", e); }
    }

    //*****************************************************************************************/
    // checkStatus
    // Retorn d'objecte Status de la crida Ajax correcte
    //********************************************************************************************/
    checkStatus = function (oStatus) {
        try {
            //Carreguem el array global per mantenir els valors
            arrStatus = oStatus;
            objError = arrStatus[0];

            //Si es un error, mostrem el missatge
            if (objError.error == "true") {
                if (objError.typemsg == "1") { //Missatge estil pop-up
                    var url = "Security/srvMsgBoxSecurity.aspx?action=Message&TitleKey=SaveName.Error.Text&" +
                        "DescriptionText=" + encodeURIComponent(objError.msg) + "&" +
                        "Option1TextKey=SaveName.Error.Option1Text&" +
                        "Option1DescriptionKey=SaveName.Error.Option1Description&" +
                        "Option1OnClickScript=HideMsgBoxForm(); return false;&" +
                        "IconUrl=~/Base/Images/MessageFrame/stock_dialog-error.png";
                    parent.ShowMsgBoxForm(url, 400, 300, '');
                } else { //Missatge estil inline
                }
                hasChanges(true);
            }
            else {
                if (objError.objprefix == 'QUESTION') {
                    var url = "Security/srvMsgBoxSecurity?action=Message&TitleText=" + encodeURIComponent(objError.title) + "&" +
                        "DescriptionText=" + encodeURIComponent(objError.description) + "&";

                    if (objError.buttonvisible1 == true) {
                        url += "Option1TextText=" + encodeURIComponent(objError.buttontext1) + "&" +
                            "Option1DescriptionText=" + encodeURIComponent(objError.buttondescription1) + "&" +
                            "Option1OnClickScript=" + objError.buttonfunct1 + " return false;&";
                    }
                    if (objError.buttonvisible2 == true) {
                        url += "Option2TextText=" + encodeURIComponent(objError.buttontext2) + "&" +
                            "Option2DescriptionText=" + encodeURIComponent(objError.buttondescription2) + "&" +
                            "Option2OnClickScript=" + objError.buttonfunct2 + " return false;&";
                    }
                    url += "IconUrl=~/Base/Images/MessageFrame/dialog-question.png";

                    parent.ShowMsgBoxForm(url, 400, 300, '');

                    hasChanges(true);
                }
            }
        } catch (e) { alert('checkStatus: ' + e); }
    }                  //end checkStatus function

    //*****************************************************************************************/
    // deleteSecurityOptions
    // Eliminació de SecurityOptions per ID
    //********************************************************************************************/
    //    this.deleteSecurityOptions = function(oId) {
    //        try {
    //            showLoadingGrid(true);
    //            AsyncCall("POST", "Handlers/srvSecurityOptions.ashx?action=deleteXSecurityOptions&ID=" + oId, "json", "arrStatus", "checkStatus(arrStatus); if(arrStatus[0].error == 'false'){ deleteSelectedNode(); }")
    //        } catch (e) { showError('deleteSecurityOptions', e); }
    //    }

    //*****************************************************************************************/
    // validateSecurityOptions
    //********************************************************************************************/
    this.validateSecurityOptions = function () {
        try {
            var objError = { 'error': 'false', 'tab': '', 'tabContainer': '', 'id': '', 'msg': '', 'typemsg': '0' };
            var n;
            for (n = 0; n < arrControls.length; n++) {
                var fieldName = arrControls[n].field.toUpperCase();
                var controls = arrControls[n].control;
                var value = arrControls[n].value;
                var typeControl = arrControls[n].type.toUpperCase();
                var list = arrControls[n].list;

                var strControls = "";
                if (controls != null) {
                    for (nc = 0; nc < controls.length; nc++) {
                        strControls += controls[nc] + ","
                    }
                }

            } //end for

            return true;
        } catch (e) { alert('validateSecurityOptions: ' + e); }
    }

    //*****************************************************************************************/
    // saveSecurityOptions
    //********************************************************************************************/
    this.saveSecurityOptions = function (idSecurityTemplate, arrAllowedIPs) {
        try {
            var oFields = '';

            for (n = 0; n < arrControls.length; n++) {
                var fieldName = arrControls[n].field.toUpperCase();
                var controls = arrControls[n].control;
                var value = arrControls[n].value;
                var typeControl = arrControls[n].type.toUpperCase();
                var list = arrControls[n].list;

                var strControls = "";
                if (controls != null) {
                    for (nc = 0; nc < controls.length; nc++) {
                        strControls += controls[nc] + ","
                    }
                }

                //Validem els camps
                if (fieldName == "ID") {
                    strParam = "ID=" + idSecurityTemplate;
                    oFields += strParam + "&";
                } else {
                    value = JSON.stringify(value);
                    var strParam = createGenericDataParam(fieldName, strControls, value, typeControl);
                    oFields += strParam + "&";
                }
            } //end for

            oFields = oFields.substring(0, oFields.length - 1);

            //Si s'han passat Grups, carrega
            if (jsGridIPs != null) {
                //Bucle per totes les composicions
                var ipRows = jsGridIPs.getRows();
                var arrAllowdIp = "";
                for (var x = 0; x < ipRows.length; x++) {
                    //Bucle per els camps
                    if (x != 0) arrAllowdIp = arrAllowdIp + "#";
                    arrAllowdIp = arrAllowdIp + jsGridIPs.retRowJSON(ipRows[x].id)[1].value
                }
                if (arrAllowdIp != "") {
                    oFields = oFields + "&TXTALLOWEDIPS=" + arrAllowdIp;
                }
            }

            showLoadingGrid(true);
            AsyncCall2("POST", "Handlers/srvSecurityOptions.ashx?action=saveXSecurityOptions&" + oFields, "json", "arrStatus", "showLoadingGrid(false);checkStatus(arrStatus); if(arrStatus[0].error == 'false'){ hasChanges(false);oSecurityOptions = new SecurityOptionsData(0); }");

            return true;
        } catch (e) {
            showError('saveSecurityOptions', e);
            return false;
        }
    }

    //*****************************************************************************************/
    // getTemplateTaskID
    // Recupera ID del TaskTemplate
    //********************************************************************************************/
    this.getSecurityOptionsID = function () {
        try {
            return SecurityOptionsID;
        } catch (e) {
            showError('getSecurityOptionsID', e);
            return "";
        }
    }

    //Si tenim el ID, carreguem el oSecurityOptions
    if (oId != null) {
        this.getSecurityOptionsById(oId);
    } else {
        this.newSecurityOptions();
    }
}

if (oSecurityOptions == null) {
    showLoadingGrid(true);
    setTimeout(function () { oSecurityOptions = new SecurityOptionsData(0); }, 500);
}