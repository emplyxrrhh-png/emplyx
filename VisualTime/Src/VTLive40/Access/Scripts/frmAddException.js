//formato recibido es Year-Month-Day
function BuildDateFromStringDate(strDate) {
    var arrDate = strDate.split("-");
    if (arrHour.length != 3) {
        return null;
    }
    else {
        var iYear = parseInt(arrHour[0]);
        var iMonth = parseInt(arrHour[1]);
        var iDay = parseInt(arrHour[2]);
    }
    return new Date(iYear, iMonth, iDay);
}

function frmAddException_Show(arrValues) {
    try {
        var oDis = document.getElementById('ctl00_contentMainBody_hdnModeEdit').value;

        loadAddExceptionBlanks();

        for (var n = 0; n < arrValues.length; n++) {
            switch (arrValues[n].attname.toLowerCase()) {
                case "jsgridatt_idzone":
                    loadGenericData("IDZONE", "hdnAddExceptionIDZone", arrValues[n].value, "X_NUMBER", "", oDis, false);
                    break;

                case "jsgridatt_exceptiondate":
                    var dateAux = new Date();
                    dateAux.FromStringDate(arrValues[n].value, dateConfig.Get("Format"), dateConfig.Get("Separator"));
                    dpDateExceptionClient.SetDate(dateAux);
                    dpDateExceptionClient.SetEnabled((oDis == 'false' ? true : false));
                    break;
            }
        }
        showWUF('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmAddException1', true);
    }
    catch (e) { showError("frmAddException_Show", e); }
}

function frmAddException_Validate() {
    try {
        if (dpDateExceptionClient.GetDate() == null) return false;
        return true;
    }
    catch (e) {
        showError("frmAddException_Validate", e);
        return false;
    }
}

function frmAddException_Close() {
    try {
        showWUF('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmAddException1', false);
    }
    catch (e) { showError("frmAddException_Close", e); }
}

function frmAddException_Load() {
    try {
        frmAddException_Show();
    }
    catch (e) { showError("frmAddException_Load", e); }
}

function frmAddException_Save() {
    try {
        if (document.getElementById('ctl00_contentMainBody_hdnModeEdit').value != "false") { frmAddException_Close(); return; }
        if (frmAddException_Validate()) {
            var oMF = AddException_FieldsToJSON();
            var rowID = document.getElementById('hdnAddExceptionIDRow').value;
            updateAddExceptionRow(rowID, oMF);
            hasChanges(true);
            frmAddException_Close();
        }
    }
    catch (e) { showError("frmAddException_Save", e); }
}

function frmAddException_ShowNew() {
    try {
        loadAddExceptionBlanks();
        showWUF('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmAddException1', true);
    }
    catch (e) { showError("frmAddException_ShowNew", e); }
}

function loadAddExceptionBlanks() {
    try {
        var oDis = document.getElementById('ctl00_contentMainBody_hdnModeEdit').value;

        loadGenericData("IDZONE", "hdnAddExceptionIDZone", "", "X_NUMBER", "", oDis, false);

        dpDateExceptionClient.SetDate(null);
        dpDateExceptionClient.SetEnabled((oDis == 'false' ? true : false));
    }
    catch (e) { showError("loadAddExceptionBlanks", e); }
}

//Carrega de tots els camps en un objecte JSON
function AddException_FieldsToJSON() {
    try {
        var IDZone = document.getElementById("hdnAddExceptionIDZone").value;
        var DateException = dpDateExceptionClient.GetDate().ToStringDate(dateConfig.Get("Format"), dateConfig.Get("Separator"));

        var oAtts = [{ 'attname': 'idzone', 'value': IDZone },
        { 'attname': 'exceptiondate', 'value': DateException }];
        return oAtts;
    }
    catch (e) {
        showError("AddException_FieldsToJSON", e);
        return null;
    }
}