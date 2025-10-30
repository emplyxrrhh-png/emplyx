function frmAddAuthorization_Show(arrValues, beginDate, endDate) {
    try {
        var n;

        loadAddAuthorizationBlanks();
        initializeTokenBoxDateEvent(beginDate, endDate);

        for (n = 0; n < arrValues.length; n++) {
            switch (arrValues[n].attname.toLowerCase()) {
                case "jsgridatt_idauthorization":
                    //var combo = document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmAddAuthorization1_tbAuthorizations_I');
                    //combo.value = arrValues[n].value;
                    tbAuthorizationsClient.SetSelectedItem(tbAuthorizationsClient.FindItemByValue(arrValues[n].value));

                    break;
                case "jsgridatt_authorization":
                    tbAvailableDatesClient.ClearTokenCollection();
                    var arrDates = arrValues[n].value.split(",");
                    for (var i = 0; i < arrDates.length; i += 1) {
                        tbAvailableDatesClient.AddToken(arrDates[i]);
                    }
                    break;
            }
        }

        tbAvailableDatesClient.SetEnabled(true);

        showWUF('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmAddAuthorization1', true);
    } catch (e) { showError("frmAddAuthorization_Show", e); }
}

function frmAddAuthorization_Validate() {
    try {
        if (tbAuthorizationsClient.GetValue() == "" || tbAuthorizationsClient.GetValue() == null) {
            showErrorPopup("Error.frmAddAuthorization.txtDatesTitle", "ERROR", "Error.frmAddAuthorization.txtDatesTitleDesc", "Error.frmAddAuthorization.OK", "Error.frmAddAuthorization.OKDesc", "");
            return false;
        }

        if (tbAvailableDatesClient.GetValue() == "" || tbAvailableDatesClient.GetValue() == null) {
            showErrorPopup("Error.frmAddAuthorization.txtDatesTitle", "ERROR", "Error.frmAddAuthorization.txtDatesTitleDesc", "Error.frmAddAuthorization.OK", "Error.frmAddAuthorization.OKDesc", "");
            return false;
        }

        return true;
    } catch (e) {
        showError("frmAddAuthorization_Validate", e);
        return false;
    }
}

function afterValidatePath(arrStatus) {
    showLoadingGrid(false);
    frmAddAuthorization_SaveOk();
}

function frmAddAuthorization_Close() {
    try {
        showWUF('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmAddAuthorization1', false);
    } catch (e) { showError("frmAddAuthorization_Close", e); }
}

function frmAddAuthorization_Load() {
    try {
        frmAddAuthorization_Show();
    } catch (e) { showError("frmAddAuthorization_Load", e); }
}

function frmAddAuthorization_Save() {
    try {
        if (frmAddAuthorization_Validate()) {
            frmAddAuthorization_SaveOk();
        }
    } catch (e) { showError("frmAddAuthorization_Save", e); }
}

function frmAddAuthorization_SaveOk() {
    try {
        var oMF = AddAuthorization_FieldsToJSON();

        var rowID = document.getElementById('hdnAddAuthorizationIDRow').value;
        updateAddAuthorizationRow(rowID, oMF);

        setEventFilter(tbAvailableDatesClient.GetValue());
        hasChanges(true);
        frmAddAuthorization_Close();
    } catch (e) { showError("frmAddAuthorization_SaveOk", e); }
}

var beginD;
var endD;

function frmAddAuthorization_ShowNew(beginDate, endDate) {
    try {
        if (beginDate != "" && endDate != "") {
            document.getElementById('hdnbeginDate').value = beginDate;
            document.getElementById('hdnendDate').value = endDate;

            initializeTokenBoxDateEvent(beginDate, endDate);
            showWUF('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmAddAuthorization1', true);
        }
        else {
            Robotics.Client.JSErrors.showJSerrorPopup(Robotics.Client.JSErrors.JSErrorTypes.roJsError, '',
                { text: '', key: 'roJsError' }, { text: "Debe seleccionar las fechas de inicio y fin del evento", key: '' },
                { text: '', textkey: 'roErrorClose', desc: '', desckey: '', script: '' },
                Robotics.Client.JSErrors.createEmptyButton(), Robotics.Client.JSErrors.createEmptyButton(), Robotics.Client.JSErrors.createEmptyButton())
        }
    } catch (e) { showError("frmAddAuthorization_ShowNew", e); }
}

function initializeTokenBoxDateEvent(beginDate, endDate) {
    arrBegin = beginDate.split("/");
    arrEnd = endDate.split("/");

    var dateArr = getDateArray(arrBegin, arrEnd);

    tbAvailableDatesClient.ClearTokenCollection();
    tbAvailableDatesClient.ClearItems();
    for (var i = 0; i < dateArr.length; i += 1) {
        tbAvailableDatesClient.AddItem(moment(dateArr[i]).format("DD/MM/YYYY"));
        tbAvailableDatesClient.AddToken(moment(dateArr[i]).format("DD/MM/YYYY"));
    }
}

var getDateArray = function (start, end) {
    var arr = new Array();
    var dt = new Date(start[2], start[1] - 1, start[0]);
    var endt = new Date(end[2], end[1] - 1, end[0]);
    while (dt <= endt) {
        arr.push(new Date(dt));
        dt.setDate(dt.getDate() + 1);
    }
    return arr;
}

function loadAddAuthorizationBlanks() {
    try {
        tbAvailableDatesClient.SetValue("");
        tbAvailableDatesClient.SetEnabled(true);
    } catch (e) { showError("loadAddAuthorizationBlanks", e); }
}

//Carrega de tots els camps en un objecte JSON
function AddAuthorization_FieldsToJSON() {
    try {
        var ID = document.getElementById("hdnAddAuthorizationID").value;
        var strType = 'dates';
        var strValue = '';
        var strDisplay = '';
        // if (document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmAddAuthorization1_opTypeMultipleDates_chkButton').checked) {
        strType = 'dates';
        strValue = tbAvailableDatesClient.GetValue();

        var supCount = strValue.split(",").length
        //var auth = document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmAddAuthorization1_tbAuthorizations_I').value;
        var auth = tbAuthorizationsClient.GetSelectedItem().text;
        var authVal = tbAuthorizationsClient.GetSelectedItem().value;
        strDisplay = auth + " (" + strValue + ")";
        //}

        var oAtts = [{ 'attname': 'idAuthorization', 'value': authVal },
        { 'attname': 'type', 'value': strType },
        { 'attname': 'Authorization', 'value': strValue },
        { 'attname': 'display', 'value': strDisplay }
        ];

        return oAtts;
    } catch (e) {
        showError("AddAuthorization_FieldsToJSON", e);
        return null;
    }
}