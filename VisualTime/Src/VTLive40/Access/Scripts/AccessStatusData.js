function DataAccessStatus(oId) {
    var arrControls;

    var arrStatus;
    var arrData;

    var AccessStatusID; //ID actual
    var AccessStatusIDName = 'ID';
    var AccessStatusPositionsName = 'Position';
    var AccessStatusImageIDName = 'ImageID';

    var oPositions; //Parametres de posicionament de les Status
    var ImageID;    //Imatge que es te de carregar desde el Flash

    //*****************************************************************************************/
    // getExportById
    // Recupera el objecte Export per ID
    //********************************************************************************************/
    this.getAccessStatusById = function (oId) {
        try {
            if (oId != null) {
                AccessStatusID = oId;
                //AsyncCall("POST", "srvAccessStatus.aspx?action=getXAccessStatus&ID=" + oId, "json", "arrControls", "loadAccessStatus(arrControls);")
            } else {
                if (AccessStatusID != null) {
                    //AsyncCall("POST", "srvAccessStatus.aspx?action=getXAccessStatus&ID=" + AccessStatusID, "json", "arrControls", "loadAccessStatus(arrControls);")
                }
            }
        } catch (e) { showError("getAccessStatusById", e); }
    }         //end function

    //*****************************************************************************************/
    // loadExport
    // Carrega els camps
    //********************************************************************************************/
    loadAccessStatus = function (arrFields) {
        try {
            //Carreguem el array global per mantenir els valors
            if (arrFields != null) {
                arrControls = arrFields[0];

                if (arrControls[0].error == "true") {
                    checkStatus(arrControls);
                    return;
                }

                oDis = false;

                arrFields = arrFields[0];
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
                if (fieldName == AccessStatusIDName.toUpperCase()) {
                    AccessStatusID = value;
                }

                //Si son les posicions, asignem
                if (fieldName == AccessStatusPositionsName.toUpperCase()) {
                    oPositions = value;
                }

                //Si son les posicions, asignem
                if (fieldName == AccessStatusImageIDName.toUpperCase()) {
                    ImageID = value;
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

                loadGenericData(fieldName, strControls, value, typeControl, strList, strDisable);
            } //end for

            //TODO: El que es tingui de fer darrera la carrega generica...
            var read, txt;
            read = document.getElementById('readOnlyNameAccessStatus');
            txt = document.getElementById('txtName');

            if (read == null) {
                setTimeout("loadAccessStatus();", 1000);
                return;
            }

            txt.value = read.innerHTML;
            if (read.innerHTML.replace(/^\s*|\s*$/g, "") == "") {
                EditNameAccessZone('true');
            } else {
                EditNameAccessZone('false');
            }

            var objDiv = document.getElementById('divContenido');
            if (objDiv != null) {
                objDiv.style.display = '';
            }

            if (oPositions != "") {
                var oParmArray = new Array();
                oParmArray = oPositions.split(",");
                var strPositions = "";

                if (oParmArray.length > 6) {
                    var oCnt = Math.round(oParmArray.length / 6);
                    for (var xy = 0; xy < oCnt; xy++) {
                        var oIdx = xy * 6;
                        var strColor = oParmArray[oIdx + 5];
                        if (!strColor.startsWith("#")) {
                            var color = new RGBColor(strColor);
                            var colorHTML;

                            if (color.ok) { // 'ok' is true when the parsing was a success
                                colorHTML = color.toHex();
                            } else {
                                colorHTML = oParmArray[oIdx + 5];
                            } //end if

                            strPositions += oParmArray[oIdx] + "," + oParmArray[oIdx + 1] + "," + oParmArray[oIdx + 2] + "," + oParmArray[oIdx + 3] + "," + oParmArray[oIdx + 4] + "," + colorHTML.substring(1) + ",";
                        } //end if
                    } //end for

                    strPositions = strPositions.substring(0, strPositions.length - 1);
                    oPositions = strPositions;
                } else {
                    var strColor = oParmArray[5];
                    if (!strColor.startsWith("#")) {
                        var color = new RGBColor(strColor);
                        var colorHTML;
                        if (color.ok) { // 'ok' is true when the parsing was a success
                            colorHTML = color.toHex();
                        } else {
                            colorHTML = oParmArray[5];
                        } //end if
                        oPositions = oParmArray[0] + "," + oParmArray[1] + "," + oParmArray[2] + "," + oParmArray[3] + "," + oParmArray[4] + "," + colorHTML.substring(1);
                    } //end if
                } //end if

                if (document.getElementById('divLocationMap').innerHTML != "") {
                    loadLocationMapFlash(ImageID, oPositions);
                    //setTimeout("LocationMap_DoFSCommand('REPOSITION', '" + oPositions + "');", 1500);
                } else {
                    loadLocationMapFlash(ImageID, oPositions);
                }
            } else {
                if (document.getElementById('divLocationMap').innerHTML != "") {
                    loadLocationMapFlash(ImageID, '');
                    //setTimeout("LocationMap_DoFSCommand('RESET', '');", 1500);
                } else {
                    loadLocationMapFlash(ImageID, '');
                }
            }

            top.focus();
        } catch (e) { showError('loadAccessStatus', e); }
    }                                                                     //end loadExport function
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
                    var url = "Access/srvMsgBoxAccess.aspx?action=Message&TitleKey=SaveName.Error.Text&" +
                        "DescriptionText=" + objError.msg + "&" +
                        "Option1TextKey=SaveName.Error.Option1Text&" +
                        "Option1DescriptionKey=SaveName.Error.Option1Description&" +
                        "Option1OnClickScript=HideMsgBoxForm(); return false;&" +
                        "IconUrl=~/Base/Images/MessageFrame/stock_dialog-error.png";
                    parent.ShowMsgBoxForm(url, 400, 300, '');
                } else { //Missatge estil inline
                }
                hasChanges(true);
            }
        } catch (e) { showError("checkStatus", e); }
    }             //end checkStatus function

    //*****************************************************************************************/
    // validateExport
    //********************************************************************************************/
    this.validateAccessStatus = function () {
        try {
            return true;
        } catch (e) { showError('validateAccessStatus', e); }
    }

    //*****************************************************************************************/
    // saveAccessStatus
    //********************************************************************************************/
    this.saveAccessStatus = function () {
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
                    strParam = "ID=" + AccessStatusID;
                    oFields += strParam + "&";
                    /*} else if (fieldName == RecalcDateName.toUpperCase() || fieldName == RecalcDifferenceName.toUpperCase() || fieldName == RecalcOldestDateName.toUpperCase() || fieldName == firstDayOfMonthName.toUpperCase()) {
                    //Nothing to do*/
                } else if (fieldName == "POSITION") {
                } else if (fieldName == "IMAGEID") {
                } else {
                    var strParam = createGenericDataParam(fieldName, strControls, value, typeControl);
                    if (strParam != "") {
                        oFields += strParam + "&";
                    }
                }
            } //end for

            showLoadingGrid(true);
            AsyncCall("POST", "srvAccessStatus.aspx?action=saveXAccessZone&" + oFields, "json", "arrStatus", "showLoadingGrid(false);checkStatus(arrStatus); if(arrStatus[0].error == 'false'){ refreshBeforeSave(arrStatus); }");

            return true;
        } catch (e) {
            showError('saveAccessStatus', e);
            return false;
        }
    }

    //*****************************************************************************************/
    // getAccessStatusID
    // Recupera ID del AccessZone
    //********************************************************************************************/
    this.getAccessStatusID = function () {
        try {
            return AccessStatusID;
        } catch (e) {
            showError('getAccessStatusID', e);
            return "";
        }
    }

    //*****************************************************************************************/
    // getAccessStatusImageID
    // Recupera ImageID del AccessZone
    //********************************************************************************************/
    this.getAccessStatusImageID = function () {
        try {
            return ImageID;
        } catch (e) {
            showError('getAccessStatusImageID', e);
            return "";
        }
    }

    //*****************************************************************************************/
    // getAccessStatusParams
    // Recupera Params del AccessStatus
    //********************************************************************************************/
    this.getAccessStatusPositions = function () {
        try {
            return oPositions;
        } catch (e) {
            showError('getAccessStatusPositions', e);
            return "";
        }
    }

    //*****************************************************************************************/
    // setAccessStatusParams
    // Recupera Params del AccessStatus
    //********************************************************************************************/
    this.setAccessStatusPositions = function (oValue) {
        try {
            oPositions = oValue;
        } catch (e) {
            showError('setAccessStatusPositions', e);
        }
    }

    //*****************************************************************************************/
    // setAccessStatusParams
    // Recupera Params del AccessStatus
    //********************************************************************************************/
    this.setAccessStatusImageID = function (oValue) {
        try {
            ImageID = oValue;
        } catch (e) {
            showError('setAccessStatusImageID', e);
        }
    }

    //Si tenim el ID, carreguem el oExport
    if (oId != null) {
        this.getAccessStatusById(oId);
    } else {
        //this.newExport();
    }
}