function DataAccessStatInfo(oId) {
    var arrControls;

    var arrStatInfo;
    var arrData;

    var AccessStatInfoID; //ID actual
    var AccessStatInfoIDName = 'ID';
    var AccessStatInfoPositionsName = 'Position';
    var AccessStatInfoImageIDName = 'ImageID';

    var oPositions; //Parametres de posicionament de les zones
    var ImageID;    //Imatge que es te de carregar desde el Flash

    //*****************************************************************************************/
    // getExportById
    // Recupera el objecte Export per ID
    //********************************************************************************************/
    this.getAccessStatInfoById = function (oId) {
        try {
            if (oId != null) {
                AccessStatInfoID = oId;
                AsyncCall("POST", "srvAccessStatInfo.aspx?action=getXAccessStatInfo&ID=" + oId, "json", "arrControls", "loadAccessStatInfo(arrControls);")
            } else {
                if (AccessStatInfoID != null) {
                    AsyncCall("POST", "srvAccessStatInfo.aspx?action=getXAccessStatInfo&ID=" + AccessStatInfoID, "json", "arrControls", "loadAccessStatInfo(arrControls);")
                }
            }
        } catch (e) { showError("getAccessStatInfoById", e); }
    }         //end function

    //*****************************************************************************************/
    // loadExport
    // Carrega els camps
    //********************************************************************************************/
    loadAccessStatInfo = function (arrFields) {
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
                if (fieldName == AccessStatInfoIDName.toUpperCase()) {
                    AccessStatInfoID = value;
                }

                //Si son les posicions, asignem
                if (fieldName == AccessStatInfoPositionsName.toUpperCase()) {
                    oPositions = value;
                }

                //Si son les posicions, asignem
                if (fieldName == AccessStatInfoImageIDName.toUpperCase()) {
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
            read = document.getElementById('readOnlyNameAccessStatInfo');
            txt = document.getElementById('txtName');

            if (read == null) {
                setTimeout("loadAccessStatInfo();", 1000);
                return;
            }

            txt.value = read.innerHTML;

            var objDiv = document.getElementById('divContenido');
            if (objDiv != null) {
                objDiv.style.display = '';
            }

            if (oPositions != "") {
                var oParmArray = new Array();
                oParmArray = oPositions.split(",");
                var strPositions = "";

                if (oParmArray.length > 10) {
                    var oCnt = Math.round(oParmArray.length / 10);
                    for (var xy = 0; xy < oCnt; xy++) {
                        var oIdx = xy * 10;
                        var strColor = oParmArray[oIdx + 6];
                        if (!strColor.startsWith("#")) {
                            var color = new RGBColor(strColor);
                            var colorHTML;

                            if (color.ok) { // 'ok' is true when the parsing was a success
                                colorHTML = color.toHex();
                            } else {
                                colorHTML = oParmArray[oIdx + 6];
                            } //end if

                            strPositions += oParmArray[oIdx]
                                + "," + oParmArray[oIdx + 1]
                                + "," + oParmArray[oIdx + 2]
                                + "," + oParmArray[oIdx + 3]
                                + "," + oParmArray[oIdx + 4]
                                + "," + oParmArray[oIdx + 5]
                                + "," + colorHTML.substring(1)
                                + "," + oParmArray[oIdx + 7]
                                + "," + +oParmArray[oIdx + 8]
                                + "," + +oParmArray[oIdx + 9]
                                + ",";
                        } //end if
                    } //end for

                    strPositions = strPositions.substring(0, strPositions.length - 1);
                    oPositions = strPositions;
                } else {
                    var strColor = oParmArray[6];
                    if (!strColor.startsWith("#")) {
                        var color = new RGBColor(strColor);
                        var colorHTML;
                        if (color.ok) { // 'ok' is true when the parsing was a success
                            colorHTML = color.toHex();
                        } else {
                            colorHTML = oParmArray[6];
                        } //end if
                        oPositions = oParmArray[0]
                            + "," + oParmArray[1]
                            + "," + oParmArray[2]
                            + "," + oParmArray[3]
                            + "," + oParmArray[4]
                            + "," + oParmArray[5]
                            + "," + colorHTML.substring(1)
                            + "," + oParmArray[7]
                            + "," + oParmArray[8]
                            + "," + oParmArray[9];
                    } //end if
                } //end if

                if (document.getElementById('divStatusMap').innerHTML != "") {
                    setTimeout("StatusMap_DoFSCommand('REPOSITION', '" + oPositions + "');", 1500);
                    setTimeout("enableTimer();", 3000);
                } else {                    
                    setTimeout("enableTimer();", 5000);
                }
            } else {
                if (document.getElementById('divStatusMap').innerHTML != "") {
                    setTimeout("StatusMap_DoFSCommand('RESET', '');", 1500);
                    setTimeout("enableTimer();", 3000);
                } else {                    
                    setTimeout("enableTimer();", 5000);
                }
            }

            top.focus();
        } catch (e) { showError('loadAccessStatInfo', e); }
    }                                                                      //end loadExport function
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
    this.validateAccessStatInfo = function () {
        try {
            return true;
        } catch (e) { showError('validateAccessStatInfo', e); }
    }

    //*****************************************************************************************/
    // saveAccessStatInfo
    //********************************************************************************************/
    this.saveAccessStatInfo = function () {
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
                    strParam = "ID=" + AccessStatInfoID;
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
            AsyncCall("POST", "srvAccessStatInfo.aspx?action=saveXAccessZone&" + oFields, "json", "arrStatus", "showLoadingGrid(false);checkStatus(arrStatus); if(arrStatus[0].error == 'false'){ refreshBeforeSave(arrStatus); }");

            return true;
        } catch (e) {
            showError('saveAccessStatInfo', e);
            return false;
        }
    }

    //*****************************************************************************************/
    // getAccessStatInfoID
    // Recupera ID del AccessZone
    //********************************************************************************************/
    this.getAccessStatInfoID = function () {
        try {
            return AccessStatInfoID;
        } catch (e) {
            showError('getAccessStatInfoID', e);
            return "";
        }
    }

    //*****************************************************************************************/
    // getAccessStatInfoImageID
    // Recupera ImageID del AccessZone
    //********************************************************************************************/
    this.getAccessStatInfoImageID = function () {
        try {
            return ImageID;
        } catch (e) {
            showError('getAccessStatInfoImageID', e);
            return "";
        }
    }

    //*****************************************************************************************/
    // getAccessStatInfoParams
    // Recupera Params del AccessStatInfo
    //********************************************************************************************/
    this.getAccessStatInfoPositions = function () {
        try {
            return oPositions;
        } catch (e) {
            showError('getAccessStatInfoPositions', e);
            return "";
        }
    }

    //*****************************************************************************************/
    // setAccessStatInfoParams
    // Recupera Params del AccessStatInfo
    //********************************************************************************************/
    this.setAccessStatInfoPositions = function (oValue) {
        try {
            oPositions = oValue;
        } catch (e) {
            showError('setAccessStatInfoPositions', e);
        }
    }

    //*****************************************************************************************/
    // setAccessStatInfoImageID
    //********************************************************************************************/
    this.setAccessStatInfoImageID = function (oValue) {
        try {
            ImageID = oValue;
        } catch (e) {
            showError('setAccessStatInfoImageID', e);
        }
    }

    //*****************************************************************************************/
    // setAccessStatInfoID
    //********************************************************************************************/
    this.setAccessStatInfoID = function (oValue) {
        try {
            AccessStatInfoID = oValue;
        } catch (e) {
            showError('setAccessStatInfoID', e);
        }
    }

    //Si tenim el ID, carreguem el oExport
    if (oId != null) {
        this.getAccessStatInfoById(oId);
    } else {
        //this.newExport();
    }
}