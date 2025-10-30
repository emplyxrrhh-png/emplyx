// Carrega la part del TAB grisa superior
function roUFC_loadComboVisibilityParms(objPrefix) {
    try {
        var stamp = '&StampParam=' + new Date().getMilliseconds();

        var objValue = document.getElementById(objPrefix + '_cmbVisibilityCriteria1_Text').value;
        var cmb = objPrefix + '_cmbVisibilityValue';
        var cmb_Text = objPrefix + '_cmbVisibilityValue_Text';
        var cmb_Value = objPrefix + '_cmbVisibilityValue_Value';

        ajax = nuevoAjax();
        ajax.open("GET", hBaseRef + "WebUserControls/roUserFieldCriteria.aspx?action=getUserFieldList&fieldName=" + objValue + stamp, true);

        ajax.onreadystatechange = function() {
            if (ajax.readyState == 4) {
                eval("var arrObj = new Array(" + ajax.responseText + ");");
                var n;

                roCB_clearItems(cmb, cmb_Text, cmb_Value);
                for (n = 0; n < arrObj.length; n++) {
                    roCB_addItem(cmb, arrObj[n], arrObj[n], 'hasChanges(true);');
                }
            }
        }

        ajax.send(null)

    } catch (e) { showError("roUserFieldCriteria:loadComboVisibilityParms", e); }
}

function roUFC_hiddenPans(objPrefix) {
    try {
        var panValTxt = document.getElementById(objPrefix + '_panVValueTextBox');
        var panValNum = document.getElementById(objPrefix + '_panVValueNumericBox');
        var panValDec = document.getElementById(objPrefix + '_panVValueDecimalBox');
        var panValMsk = document.getElementById(objPrefix + '_panVValueMaskTextBox');
        var panValMskTime = document.getElementById(objPrefix + '_panVValueMaskTextBoxTime');
        var panValCmb = document.getElementById(objPrefix + '_panVValueComboBox');
        var panValPeriod = document.getElementById(objPrefix + '_panVValuePeriods');
        var panValTimePeriod = document.getElementById(objPrefix + '_panVValueTimePeriods');

        panValTxt.style.display = 'none';
        panValNum.style.display = 'none';
        panValDec.style.display = 'none';
        panValMsk.style.display = 'none';
        panValMskTime.style.display = 'none';
        panValCmb.style.display = 'none';
        panValPeriod.style.display = 'none';
        panValTimePeriod.style.display = 'none';
    } catch (e) { showError("roUserFieldCriteria::hiddenPans", e); }
}

// Comprovacions dels combos a la pestanya visibilitat
function roUFC_chkCombosVisibility(objPrefix, cmbVC) {
    try {
        var cmbValue = document.getElementById(objPrefix + '_cmbVisibilityCriteria1_Value');
        objValue = cmbValue.value.toString().split("*|*")[1];
        var cmb2 = objPrefix + '_cmbVisibilityCriteria2';
        var cmb2_Text = objPrefix + '_cmbVisibilityCriteria2_Text';
        var cmb2_Value = objPrefix + '_cmbVisibilityCriteria2_Value';

        var cmb3 = objPrefix + '_cmbVisibilityCriteria3';
        var cmb3_Text = objPrefix + '_cmbVisibilityCriteria3_Text';
        var cmb3_Value = objPrefix + '_cmbVisibilityCriteria3_Value';

        roUFC_hiddenPans(objPrefix);

        roCB_clearItems(cmb2, cmb2_Text, cmb2_Value);
        //text
        if (objValue == "0") {
            roCB_addItem(cmb2, CriteriaEquals, 'Equals', 'roUFC_checkCombo2("' + objPrefix + '","Equals"); hasChanges(true);');
            roCB_addItem(cmb2, CriteriaDifferent, 'Different', 'roUFC_checkCombo2("' + objPrefix + '"," "); hasChanges(true);');
            roCB_addItem(cmb2, CriteriaStartsWith, 'StartsWith', 'roUFC_checkCombo2("' + objPrefix + '","StartsWith"); hasChanges(true);');
            roCB_addItem(cmb2, CriteriaContains, 'Contains', 'roUFC_checkCombo2("' + objPrefix + '","Contains"); hasChanges(true);');
            //Numeric (1), Data (2), Decimal (3), Hora (4)
        } else if (objValue == "1" || objValue == "2" || objValue == "3" || objValue == "4") {
            roCB_addItem(cmb2, CriteriaEquals, 'Equals', 'roUFC_checkCombo2("' + objPrefix + '","Equals");hasChanges(true);');
            roCB_addItem(cmb2, CriteriaMajor, 'Major', 'roUFC_checkCombo2("' + objPrefix + '","Major");hasChanges(true);');
            roCB_addItem(cmb2, CriteriaMajorOrEquals, 'MajorOrEquals', 'roUFC_checkCombo2("' + objPrefix + '","MajorOrEquals");hasChanges(true);');
            roCB_addItem(cmb2, CriteriaMinor, 'Minor', 'roUFC_checkCombo2("' + objPrefix + '","Minor");hasChanges(true);');
            roCB_addItem(cmb2, CriteriaMinorOrEquals, 'MinorOrEquals', 'roUFC_checkCombo2("' + objPrefix + '","MinorOrEquals");hasChanges(true);');
            roCB_addItem(cmb2, CriteriaDifferent, 'Different', 'roUFC_checkCombo2("' + objPrefix + '","Different");hasChanges(true);');
            //Llista de valors
        } else if (objValue == "5") {
            roCB_addItem(cmb2, CriteriaContains, 'Contains', 'roUFC_checkCombo2("' + objPrefix + '","Contains");hasChanges(true);');
            roCB_addItem(cmb2, CriteriaNoContains, 'NoContains', 'roUFC_checkCombo2("' + objPrefix + '","NoContains");hasChanges(true);');
            //Periodes de data / hora
        } else if (objValue == "6" || objValue == "7") {
            roCB_addItem(cmb2, CriteriaEquals, 'Equals', 'roUFC_checkCombo2("' + objPrefix + '","Equals");hasChanges(true);');
            roCB_addItem(cmb2, CriteriaContains, 'Contains', 'roUFC_checkCombo2("' + objPrefix + '","Contains");hasChanges(true);');
        }

            roCB_clearItems(cmb3, cmb3_Text, cmb3_Value);
        //Texte(0), Numeric(1), Decimal(3)
        if (objValue == "0" || objValue == "1" || objValue == "3") {
            roCB_addItem(cmb3, CriteriaTheValue, 'TheValue', 'roUFC_checkCombo3("' + objPrefix + '","' + objValue + '","TheValue");hasChanges(true);');
            //Data (2)
        } else if (objValue == "2") {
            roCB_addItem(cmb3, CriteriaTheDate, 'TheDate', 'roUFC_checkCombo3("' + objPrefix + '","' + objValue + '","TheDate");hasChanges(true);');
            roCB_addItem(cmb3, CriteriaTheDateOfJustification, 'TheDateActual', 'roUFC_checkCombo3("' + objPrefix + '","' + objValue + '","TheDateActual");hasChanges(true);');
            //Hora (4)
        } else if (objValue == "4") {
            roCB_addItem(cmb3, CriteriaTheTime, 'TheTime', 'roUFC_checkCombo3("' + objPrefix + '","' + objValue + '","TheTime");hasChanges(true);');
            roCB_addItem(cmb3, CriteriaTheTimeOfJustification, 'TheTimeActual', 'roUFC_checkCombo3("' + objPrefix + '","' + objValue + '","TheTimeActual");hasChanges(true);');
            //Llista de valors
        } else if (objValue == "5") {
            roCB_addItem(cmb3, CriteriaTheValue, 'TheValue', 'roUFC_checkCombo3("' + objPrefix + '","' + objValue + '","TheValue");hasChanges(true);');
            //Periodes de data
        } else if (objValue == "6") {
            roCB_addItem(cmb3, CriteriaThePeriod, 'ThePeriod', 'roUFC_checkCombo3("' + objPrefix + '","' + objValue + '","ThePeriod");hasChanges(true);');
            roCB_addItem(cmb3, CriteriaTheDate, 'TheDate', 'roUFC_checkCombo3("' + objPrefix + '","' + objValue + '","TheDate");hasChanges(true);');
            roCB_addItem(cmb3, CriteriaTheDateOfJustification, 'TheDateActual', 'roUFC_checkCombo3("' + objPrefix + '","' + objValue + '","TheDateActual");hasChanges(true);');
            //Periodes de hora
        } else if (objValue == "7") {
            roCB_addItem(cmb3, CriteriaThePeriod, 'ThePeriod', 'roUFC_checkCombo3("' + objPrefix + '","' + objValue + '","ThePeriod");hasChanges(true);');
            roCB_addItem(cmb3, CriteriaTheTime, 'TheTime', 'roUFC_checkCombo3("' + objPrefix + '","' + objValue + '","TheTime");hasChanges(true);');
            roCB_addItem(cmb3, CriteriaTheTimeOfJustification, 'TheTimeActual', 'roUFC_checkCombo3("' + objPrefix + '","' + objValue + '","TheTimeActual");hasChanges(true);');
        }

    } catch (e) { showError("roUserFieldCriteria:chkCombosVisibility", e); }
} //end function

// Comprovacions dels combos a la pestanya visibilitat
function roUFC_chkComboVisibility2(objPrefix,cValue) {
    try {
        var cmbValue = document.getElementById(objPrefix + '_cmbVisibilityCriteria1_Value');
        objValue = cmbValue.value.toString().split("*|*")[1];
        
        var cmb2 = objPrefix + '_cmbVisibilityCriteria2';
        var cmb2_Text = objPrefix + '_cmbVisibilityCriteria2_Text';
        var cmb2_Value = objPrefix + '_cmbVisibilityCriteria2_Value';
        
        var cmbValue = cValue;

        var cmb3 = objPrefix + '_cmbVisibilityCriteria3';
        var cmb3_Text = objPrefix + '_cmbVisibilityCriteria3_Text';
        var cmb3_Value = objPrefix + '_cmbVisibilityCriteria3_Value';

        roUFC_hiddenPans(objPrefix);

        roCB_clearItems(cmb3, cmb3_Text, cmb3_Value);
        //Texte(0), Numeric(1), Decimal(3)
        if (objValue == "0" || objValue == "1" || objValue == "3") {
            roCB_addItem(cmb3, CriteriaTheValue, 'TheValue', 'roUFC_checkCombo3("' + objPrefix + '","' + objValue + '","TheValue");hasChanges(true);');
            //Data (2)
        } else if (objValue == "2") {
            roCB_addItem(cmb3, CriteriaTheDate, 'TheDate', 'roUFC_checkCombo3("' + objPrefix + '","' + objValue + '","TheDate");hasChanges(true);');
            roCB_addItem(cmb3, CriteriaTheDateOfJustification, 'TheDateActual', 'roUFC_checkCombo3("' + objPrefix + '","' + objValue + '","TheDateActual");hasChanges(true);');
            //Hora (4)
        } else if (objValue == "4") {
            roCB_addItem(cmb3, CriteriaTheTime, 'TheTime', 'roUFC_checkCombo3("' + objPrefix + '","' + objValue + '","TheTime");hasChanges(true);');
            roCB_addItem(cmb3, CriteriaTheTimeOfJustification, 'TheTimeActual', 'roUFC_checkCombo3("' + objPrefix + '","' + objValue + '","TheTimeActual");hasChanges(true);');
            //Llista de valors
        } else if (objValue == "5") {
            roCB_addItem(cmb3, CriteriaTheValue, 'TheValue', 'roUFC_checkCombo3("' + objPrefix + '","' + objValue + '","TheValue");hasChanges(true);');
            //Periodes de data
        } else if (objValue == "6") {
            roCB_addItem(cmb3, CriteriaThePeriod, 'ThePeriod', 'roUFC_checkCombo3("' + objPrefix + '","' + objValue + '","ThePeriod");hasChanges(true);');
            if (cmbValue.toUpperCase() != "EQUALS") {
                roCB_addItem(cmb3, CriteriaTheDate, 'TheDate', 'roUFC_checkCombo3("' + objPrefix + '","' + objValue + '","TheDate");hasChanges(true);');
                roCB_addItem(cmb3, CriteriaTheDateOfJustification, 'TheDateActual', 'roUFC_checkCombo3("' + objPrefix + '","' + objValue + '","TheDateActual");hasChanges(true);');
            }
            //Periodes de hora
        } else if (objValue == "7") {
            roCB_addItem(cmb3, CriteriaThePeriod, 'ThePeriod', 'roUFC_checkCombo3("' + objPrefix + '","' + objValue + '","ThePeriod");hasChanges(true);');
            if (cmbValue.toUpperCase() != "EQUALS") {
                roCB_addItem(cmb3, CriteriaTheTime, 'TheTime', 'roUFC_checkCombo3("' + objPrefix + '","' + objValue + '","TheTime");hasChanges(true);');
                roCB_addItem(cmb3, CriteriaTheTimeOfJustification, 'TheTimeActual', 'roUFC_checkCombo3("' + objPrefix + '","' + objValue + '","TheTimeActual");hasChanges(true);');
            }
        }

    } catch (e) { showError("roUserFieldCriteria:chkComboVisibility2", e); }
} //end function 

/*combo 2 itemclick */
function roUFC_checkCombo2(objPrefix,cValue) {
    try {
        roUFC_hiddenPans(objPrefix);
        var cmb2 = document.getElementById(objPrefix + '_cmbVisibilityCriteria2_Value');
        var cmb3 = document.getElementById(objPrefix + '_cmbVisibilityCriteria3_Value');

        roUFC_chkComboVisibility2(objPrefix,cValue);

        if (cmb2.value != "" && cmb3.value != "") {
            roUFC_checkCombo3(objPrefix, cmb2.value, cmb3.value);
        }
    } catch (e) { showError("roUserFieldCriteria:checkCombo2", e); }
} //end function

function roUFC_checkCombo3(objPrefix, val1, val2) {
    try {
        roUFC_hiddenPans(objPrefix);
        var panValTxt = document.getElementById(objPrefix + '_panVValueTextBox');
        var panValNum = document.getElementById(objPrefix + '_panVValueNumericBox');
        var panValDec = document.getElementById(objPrefix + '_panVValueDecimalBox');
        var panValMsk = document.getElementById(objPrefix + '_panVValueMaskTextBox');
        var panValMskTime = document.getElementById(objPrefix + '_panVValueMaskTextBoxTime');                
        var panValCmb = document.getElementById(objPrefix + '_panVValueComboBox');
        var panValPeriod = document.getElementById(objPrefix + '_panVValuePeriods');
        var panValTimePeriod = document.getElementById(objPrefix + '_panVValueTimePeriods');

        var TxtField = document.getElementById(objPrefix + '_txtVisibilityValue');
        var NumField = document.getElementById(objPrefix + '_numVisibilityValue');
        var DecField = document.getElementById(objPrefix + '_decVisibilityValue');

        roUFC_hiddenPans(objPrefix);

        if (val1 == "0") { //Texte(0)
            TxtField.value = "";
            panValTxt.style.display = '';
        } else if (val1 == "1") { //Numeric
            NumField.value = "0";
            panValNum.style.display = '';
        } else if (val1 == "3") { //Decimal
            DecField.value = "0";
            panValDec.style.display = '';
        } else if (val1 == "2") { //Data
            if (val2 == "TheDateOfJustification" || val2 == "TheDateActual") {
                panValMsk.style.display = 'none';
            } else {
                panValMsk.style.display = '';
            }
        } else if (val1 == "4") { //Hora
            if (val2 == "TheTimeOfJustification" || val2 == "TheTimeActual") {
                panValMskTime.style.display = 'none';
            } else {
                panValMskTime.style.display = '';
            }
        } else if (val1 == "5") { //Llista de valors
            roUFC_loadComboVisibilityParms(objPrefix);
            panValCmb.style.display = '';
        } else if (val1 == "6") { //Periodes de data
            if (val2 == "TheDateOfJustification" || val2 == "TheDateActual") {
                panValMsk.style.display = "none";
            } else if (val2 == "TheDate") {
                panValMsk.style.display = "";
            } else if (val2 == "ThePeriod") {
                panValPeriod.style.display = "";
            }
        } else if (val1 == "7") { //Periodes d'hora
            if (val2 == "TheTimeOfJustification" || val2 == "TheTimeActual") {
                panValMskTime.style.display = "none";
            } else if (val2 == "TheTime") {
                panValMskTime.style.display = "";
            } else if (val2 == "ThePeriod") {
                panValTimePeriod.style.display = "";
            }
        } else { //Mostra camp de texte
        }

    } catch (e) { showError("roUserFieldCriteria:checkCombo3", e); }
} //end function


/* Recuperació dels camps de criteris en un objecte JSON */
function roUFC_retJSONValues(objPrefix) {
    try {
        var retJSON = { 'fieldName': '', 'criteria1': '', 'criteria2': '', 'type':'', 'value': '' };
    
        var cmbV1 = document.getElementById(objPrefix + '_cmbVisibilityCriteria1_Value');
        var cmbV2 = document.getElementById(objPrefix + '_cmbVisibilityCriteria2_Value');
        var cmbV3 = document.getElementById(objPrefix + '_cmbVisibilityCriteria3_Value');

        var panValTxt = document.getElementById(objPrefix + '_panVValueTextBox');
        var panValNum = document.getElementById(objPrefix + '_panVValueNumericBox');
        var panValDec = document.getElementById(objPrefix + '_panVValueDecimalBox');
        var panValMsk = document.getElementById(objPrefix + '_panVValueMaskTextBox');
        var panValMskTime = document.getElementById(objPrefix + '_panVValueMaskTextBoxTime');
        var panValCmb = document.getElementById(objPrefix + '_panVValueComboBox');
        var panValPeriod = document.getElementById(objPrefix + '_panVValuePeriods');
        var panValTimePeriod = document.getElementById(objPrefix + '_panVValueTimePeriods');

        //Camps de Valors
        var TxtField = document.getElementById(objPrefix + '_txtVisibilityValue'); //Texte
        var NumField = document.getElementById(objPrefix + '_numVisibilityValue'); //Numeric
        var DecField = document.getElementById(objPrefix + '_decVisibilityValue'); //Decimal
        var mskVisibilityValueTime = document.getElementById(objPrefix + '_mskVisibilityValueTime'); //Hora
        var cmbVisibilityValue = document.getElementById(objPrefix + '_cmbVisibilityValue_Value'); //Valor llista
        
        //Periodes d'hora
        var tBegin = document.getElementById(objPrefix + '_tBegin');
        var tEnd = document.getElementById(objPrefix + '_tEnd');

        if(cmbV1.value.indexOf("*|*") > -1){
            retJSON.fieldName = cmbV1.value.split("*|*")[0];
            retJSON.type = cmbV1.value.split("*|*")[1];
        } else {
            retJSON.fieldName = cmbV1.value;
            retJSON.type = cmbV1.value;
        }
        
        retJSON.criteria1 = cmbV2.value;
        retJSON.criteria2 = cmbV3.value;

        if (panValTxt.style.display == "") { //Texte
            retJSON.value = TxtField.value;
        } else if (panValNum.style.display == "") { //Numero
            retJSON.value = NumField.value;
        } else if (panValDec.style.display == "") { //Decimal
            retJSON.value = DecField.value;
        } else if (panValCmb.style.display == "") { //Llista
            retJSON.value = cmbVisibilityValue.value;
        } else if (panValMsk.style.display == "") { //Data
            retJSON.value = mskVisibilityValueClient.GetDate();
        } else if (panValMskTime.style.display == "") { // Hora
            retJSON.value = mskVisibilityValueTime.value;
        } else if (panValPeriod.style.display == "") { //Periode de dates
            retJSON.value = dtBeginClient.GetDate() + "*" + dtEndClient.GetDate();
        } else if (panValTimePeriod.style.display == "") { //Periode d'hores
            retJSON.value = tBegin.value + "*" + tEnd.value;
        }
        
        return retJSON;
    } catch (e) { showError("roUserFieldCriteria:checkCombo3", e); return null; }
} //end function


/* Validació dels camps */
function roUFC_validateFields(objPrefix) {
    try {
        //Criteris
        var jsonCriteria = { 'fieldName': '', 'criteria1': '', 'criteria2': '', 'type': '', 'value': '' }; //recordatori estructura
        jsonCriteria = roUFC_retJSONValues(objPrefix);
        
        if (jsonCriteria.fieldName != "") {
            if (jsonCriteria.criteria1 == "") {
                showErrorPopup("Error.Criteria.txtCriteria1Title", "ERROR", "Error.Criteria.txtCriteria1Desc", "Error.Criteria.OK", "Error.Criteria.OKDesc", "");
                return false;
            } //end if
            if (jsonCriteria.criteria2 == "") {
                showErrorPopup("Error.Criteria.txtCriteria2Title", "ERROR", "Error.Criteria.txtCriteria2Desc", "Error.Criteria.OK", "Error.Criteria.OKDesc", "");
                return false;
            } //end if

            switch (jsonCriteria.type) {
                case "0": //texte
                    //Res a comprovar
                    break;
                case "1": //Numeric
                    if (jsonCriteria.value == "") { jsonCriteria.value = 0; }
                    if (isNaN(jsonCriteria.value)) {
                        showErrorPopup("Error.Criteria.ValueIsNotNumericTitle", "ERROR", "Error.Criteria.ValueIsNotNumericDesc", "Error.Criteria.OK", "Error.Criteria.OKDesc", "");
                        return false;
                    } //end if
                    break;
                case "2": //Data
                    if (jsonCriteria.criteria2 == "TheDateOfJustification" || jsonCriteria.criteria2 == "TheDateActual") {
                        //nothing to do
                    } else {
                        var dt = jsDate_retDate(jsonCriteria.value);

                        if (dt == "Invalid Date") {
                            showErrorPopup("Error.Criteria.DateIncorrectTitle", "ERROR", "Error.Criteria.DateIncorrectDesc", "Error.Criteria.OK", "Error.Criteria.OKDesc", "");
                            return false;
                        }
                    }
                    break;
                case "3": //Decimal
                    if (jsonCriteria.value == "") { jsonCriteria.value = 0; }
                    if (isNaN(jsonCriteria.value)) {
                        showErrorPopup("Error.Criteria.ValueIsNotNumericTitle", "ERROR", "Error.Criteria.ValueIsNotNumericDesc", "Error.Criteria.OK", "Error.Criteria.OKDesc", "");
                        return false;
                    } //end if
                    break;
                case "4": //Hora
                    if (jsonCriteria.criteria2 == "TheTimeOfJustification" || jsonCriteria.criteria2 == "TheTimeActual") {
                        //nothing to do
                    } else {
                        if (!jsDate_checkTimeLong(jsonCriteria.value)) {
                            showErrorPopup("Error.Criteria.DateTimeInvalidTitle", "ERROR", "Error.Criteria.DateTimeInvalidDesc", "Error.Criteria.OK", "Error.Criteria.OKDesc", "");
                            return false;
                        } //end if
                    }
                    break;
                case "5": //Llista valors
                    if (jsonCriteria.value == "") {
                        showErrorPopup("Error.Criteria.ValueListNotSelectedTitle", "ERROR", "Error.Criteria.ValueListNotSelectedDesc", "Error.Criteria.OK", "Error.Criteria.OKDesc", "");
                        return false;
                    } //end if
                    break;
                case "6": //Periodes Data
                    if (jsonCriteria.criteria2.toUpperCase() == "THEPERIOD") {
                        if (jsonCriteria.value == "") {
                            showErrorPopup("Error.Criteria.InsertDatePeriod1Title", "ERROR", "Error.Criteria.InsertDatePeriod1Desc", "Error.Criteria.OK", "Error.Criteria.OKDesc", "");
                            return false;
                        } //end if

                        var mskDateP1 = jsonCriteria.value.split("*")[0];
                        var mskDateP2 = jsonCriteria.value.split("*")[1];

                        if (mskDateP1 == "") {
                            showErrorPopup("Error.Criteria.InsertDatePeriod1", "ERROR", "Error.Criteria.InsertDatePeriod1", "Error.Criteria.OK", "Error.Criteria.OKDesc", "");
                            return false;
                        } //end if

                        if (mskDateP2 == "") {
                            showErrorPopup("Error.Criteria.InsertDatePeriod2", "ERROR", "Error.Criteria.InsertDatePeriod2", "Error.Criteria.OK", "Error.Criteria.OKDesc", "");
                            return false;
                        } //end if

                        var dt1 = jsDate_retDate(mskDateP1);
                        var dt2 = jsDate_retDate(mskDateP2);

                        if (dt1 == "Invalid Date") {
                            showErrorPopup("Error.Criteria.Date1IncorrectTitle", "ERROR", "Error.Criteria.Date1IncorrectDesc", "Error.Criteria.OK", "Error.Criteria.OKDesc", "");
                            return false;
                        } //end if

                        if (dt2 == "Invalid Date") {
                            showErrorPopup("Error.Criteria.Date2IncorrectTitle", "ERROR", "Error.Criteria.Date2IncorrectDesc", "Error.Criteria.OK", "Error.Criteria.OKDesc", "");
                            return false;
                        } //end if

                        if (dt1 > dt2) {
                            showErrorPopup("Error.Criteria.DateStartSuperiorTitle", "ERROR", "Error.Criteria.DateStartSuperiorDesc", "Error.Criteria.OK", "Error.Criteria.OKDesc", "");
                            return false;
                        } //end if
                    } //end if
                    break;
                case "7": //Periodes Hora
                    if (jsonCriteria.criteria2.toUpperCase() == "THEPERIOD") {
                        if (jsonCriteria.value == "") {
                            showErrorPopup("Error.Criteria.InsertDateTimePeriod1Title", "ERROR", "Error.Criteria.InsertDateTimePeriod1Desc", "Error.Criteria.OK", "Error.Criteria.OKDesc", "");
                            return false;
                        } //end if

                        var mskDateTimeP1 = jsonCriteria.value.split("*")[0];
                        var mskDateTimeP2 = jsonCriteria.value.split("*")[1];

                        if (mskDateTimeP1 == "") {
                            showErrorPopup("Error.Criteria.InsertDateTimePeriod1Title", "ERROR", "Error.Criteria.InsertDateTimePeriod1Desc", "Error.Criteria.OK", "Error.Criteria.OKDesc", "");
                            return false;
                        } //end if

                        if (mskDateTimeP2 == "") {
                            showErrorPopup("Error.Criteria.InsertDateTimePeriod2Title", "ERROR", "Error.Criteria.InsertDateTimePeriod2Desc", "Error.Criteria.OK", "Error.Criteria.OKDesc", "");
                            return false;
                        } //end if

                        if (!jsDate_checkTime(mskDateTimeP1)) {
                            showErrorPopup("Error.Criteria.DateTimeInvalid1Title", "ERROR", "Error.Criteria.DateTimeInvalid1Desc", "Error.Criteria.OK", "Error.Criteria.OKDesc", "");
                            return false;
                        } //end if

                        if (!jsDate_checkTime(mskDateTimeP2)) {
                            showErrorPopup("Error.Criteria.DateTimeInvalid2Title", "ERROR", "Error.Criteria.DateTimeInvalid2Desc", "Error.Criteria.OK", "Error.Criteria.OKDesc", "");
                            return false;
                        } //end if

                        var hr1 = jsDate_retMinutesToTime(mskDateTimeP1);
                        var hr2 = jsDate_retMinutesToTime(mskDateTimeP2);

                        if (hr1 > hr2) {
                            showErrorPopup("Error.Criteria.DateTimeStartMajorTitle", "ERROR", "Error.Criteria.DateTimeStartMajorDesc", "Error.Criteria.OK", "Error.Criteria.OKDesc", "");
                            return false;
                        }

                    } //end if
                    break;
            } //end switch
        }
        return true;
    } catch (e) { showError("roUserFieldCriteria:validateFields", e); }
}
