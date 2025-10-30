function frmCfgInteractive_Show(IDReader, arrValues) {
    try {
        //load fields
        var n;
        var objPrTab = "tabCtl01_frmTR" + IDReader + "_gBoxHowPunches_optHPInteractive_frmCfgInteractive1_";
        var oDis = document.getElementById('ctl00_contentMainBody_hdnModeEdit').value;

        showWUF('tabCtl01_frmTR' + IDReader + '_gBoxHowPunches_optHPInteractive_frmCfgInteractive1', true);
    } catch (e) { showError("frmCfgInteractive_Show", e); }
}

//Validacio del formulari
function frmCfgInteractive_Validate(IDReader) {
    try {
        var objPrTab = "tabCtl01_frmTR" + IDReader + "_gBoxHowPunches_optHPInteractive_frmCfgInteractive1_";
        if (AddCfgInteractive_CheckTime(document.getElementById(objPrTab + 'txtMinimESTime'))) { return false; }
        if (AddCfgInteractive_CheckTime(document.getElementById(objPrTab + 'txtMinimSETime'))) { return false; }

        return true;
    } catch (e) {
        showError("frmCfgInteractive_Validate", e);
        return false;
    }
}

function frmCfgInteractive_Close(IDReader) {
    try {
        //show te form
        showWUF('tabCtl01_frmTR' + IDReader + '_gBoxHowPunches_optHPInteractive_frmCfgInteractive1', false);
    } catch (e) { showError("frmCfgInteractive_Close", e); }
}

function frmCfgInteractive_Load(IDReader) {
    try {
        //Carrega dels Camps
        frmCfgInteractive_Show(IDReader);
    } catch (e) { showError("frmCfgInteractive_Load", e); }
}

//Grabació del formulari en el roTimeLine
function frmCfgInteractive_Save(IDReader) {
    try {
        if (document.getElementById('ctl00_contentMainBody_hdnModeEdit').value != "false") { frmCfgInteractive_Close(); return; }
        if (frmCfgInteractive_Validate(IDReader)) {
            var objPrTab = "tabCtl01_frmTR" + IDReader + "_gBoxHowPunches_optHPInteractive_frmCfgInteractive1_";

            var oMF = AddCfgInteractive_FieldsToJSON(IDReader);

            var rowID = document.getElementById('hdnAddSirenIDRow').value;

            //updateAddCfgInteractiveRow(rowID, oMF);

            hasChanges(true);
            frmCfgInteractive_Close(IDReader);
        }
    } catch (e) { showError("frmCfgInteractive_Save", e); }
}

function loadAddCfgInteractiveBlanks(IDReader) {
    try {
        var objPrTab = "tabCtl01_frmTR" + IDReader + "_gBoxHowPunches_optHPInteractive_frmCfgInteractive1_";
        var oDis = document.getElementById('ctl00_contentMainBody_hdnModeEdit').value;

        /*
        loadGenericData("ID", "hdnAddSirenID", "-1", "X_NUMBER", "", oDis, false);
        loadGenericData("WEEKDAY", objPrTab + "cmbSirenWeekDay", "", "X_COMBOBOX", "", oDis, false);
        loadGenericData("HOUR", "txtSirenTime", "", "X_TEXT", "", oDis, false);
        loadGenericData("DURATION", "txtSirenDuration", "", "X_NUMBER", "", oDis, false);
        */
    } catch (e) { showError("loadAddCfgInteractiveBlanks", e); }
}

// Comprobació de temps (funcio repetitiva)
function AddCfgInteractive_CheckTime(obj, permitBlank) {
    try {
        var oName = new Array();
        oName = obj.id.toString().split("_");
        var strName = oName[oName.length - 1];

        if (permitBlank == false) {
            if (obj.value == "") {
                obj.focus();
                showErrorPopup("Error.frmCfgInteractive." + strName + "Title", "ERROR", "Error.frmCfgInteractive." + strName + "Desc", "Error.frmCfgInteractive.OK", "Error.frmCfgInteractive.OKDesc", "");
                return true;
            }
        }

        var oField = Ext.getCmp(obj.getAttribute("ConvertedId"));
        if (oField != null) {
            oField.validate();
            if (oField.isValid(false) == false) {
                obj.focus();
                showErrorPopup("Error.frmCfgInteractive." + strName + "Title", "ERROR", "Error.frmCfgInteractive." + strName + "Desc", "Error.frmCfgInteractive.OK", "Error.frmCfgInteractive.OKDesc", "");
                return true;
            }
        }

        return false;
    } catch (e) { showError("AddCfgInteractive_CheckTime", e); }
}

//Carrega de tots els camps en un objecte JSON
function AddCfgInteractive_FieldsToJSON(IDReader) {
    try {
        var objPrTab = "tabCtl01_frmTR" + IDReader + "_gBoxHowPunches_optHPInteractive_frmCfgInteractive1_";

        /*        var ID = document.getElementById("hdnAddSirenID").value;
                var WeekDay = document.getElementById(objPrTab + "cmbSirenWeekDay_Value").value;
                var WeekDayName = document.getElementById(objPrTab + "cmbSirenWeekDay_Text").value;
                var Hour = document.getElementById('txtSirenTime').value;
                var Duration = document.getElementById('txtSirenDuration').value;

                var oAtts = [  { 'attname': 'id', 'value': ID },
                               { 'attname': 'weekdayname', 'value': WeekDayName },
                               { 'attname': 'weekday', 'value': WeekDay  },
                               { 'attname': 'hour', 'value': Hour },
                               { 'attname': 'duration', 'value': Duration }
                              ];

                return oAtts;
        */
    } catch (e) {
        showError("AddCfgInteractive_FieldsToJSON", e);
        return null;
    }
}