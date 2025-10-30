function LoadDetailsPage(IsNewMove, params) {
    if (IsNewMove) {
        var jasonificado = jasonificaMoveDetail();
        CallbackPanelDetailsClient.PerformCallback(jasonificado);
        PopDetailsClient.Show();
    }
    else {
        try {
            var objParams = JSON.parse(params);
            if (objParams.actualtype != "6") {
                var jasonificado = jasonificaMoveDetail("FALSE", objParams.canedit, objParams.stateofrow, objParams.idmove, objParams.keyrow, objParams.movetype, objParams.actualtype, objParams.idterminal,
                    objParams.idaction, objParams.idcause_idtask_iddiningroom, objParams.reliability, objParams.idzone, objParams.actualdate, objParams.shiftdate, objParams.position,
                    objParams.city, objParams.idpassport, objParams.field1, objParams.field2, objParams.field3, objParams.field4, objParams.field5, objParams.field6, objParams.invalidtype,
                    objParams.typedetails, objParams.timeZone, objParams.center, objParams.fullAddress, objParams.maskAlert, objParams.temperatureAlert, objParams.verifyType, objParams.telecommuting, objParams.IDRequest);

                CallbackPanelDetailsClient.PerformCallback(jasonificado);

                PopDetailsClient.Show();
            }
        }
        catch (e) {
            alert(e);
        }
    }
}

function CallbackPanelDetailsClient_BeginCallback() {
    LoadingPanelDetailsClient.Show();
    document.getElementById("tbMoveDetail").style.display = "none";
}

function changeReliability() {
    var wSwitch = $("#switchReliability").dxSwitch("instance");
    var valueswitch = wSwitch.option("value");

    document.getElementById("PopDetails_CallbackPanelDetails_hdnDetailsReliabilityPop").value = valueswitch;
}

function changeTelecommuting() {
    var wSwitch = $("#switchTelecommuting").dxSwitch("instance");
    var valueswitch = wSwitch.option("value");

    document.getElementById("PopDetails_CallbackPanelDetails_hdnDetailsTelecommutingPop").value = valueswitch;
}

function CallbackPanelDetailsClient_EndCallback() {
    const disabled = (document.getElementById("PopDetails_CallbackPanelDetails_hdnCanEdit").value).toUpperCase() == "TRUE" ? false : true;

    $("#switchReliability").dxSwitch({
        accessKey: null,
        activeStateEnabled: true,
        disabled,
        elementAttr: {},
        focusStateEnabled: false,
        height: undefined,
        hint: undefined,
        hoverStateEnabled: true,
        isValid: true,
        name: "",
        offText: "No Fiable",
        onContentReady: null,
        onDisposing: null,
        onInitialized: null,
        onOptionChanged: null,
        onText: "Fiable",
        onValueChanged: changeReliability,
        readOnly: false,
        rtlEnabled: false,
        tabIndex: 0,
        validationError: undefined,
        validationMessageMode: "auto",
        value: (document.getElementById("PopDetails_CallbackPanelDetails_hdnDetailsReliabilityPop").value).toUpperCase() == "TRUE" ? true : false,
        visible: true,
        width: "120px"
    });

    $("#switchTelecommuting").dxSwitch({
        accessKey: null,
        activeStateEnabled: true,
        disabled,
        elementAttr: {},
        focusStateEnabled: false,
        height: undefined,
        hint: undefined,
        hoverStateEnabled: true,
        isValid: true,
        name: "",
        offText: "No",
        onContentReady: null,
        onDisposing: null,
        onInitialized: null,
        onOptionChanged: null,
        onText: "Sí",
        onValueChanged: changeTelecommuting,
        readOnly: false,
        rtlEnabled: false,
        tabIndex: 0,
        validationError: undefined,
        validationMessageMode: "auto",
        value: (document.getElementById("PopDetails_CallbackPanelDetails_hdnDetailsTelecommutingPop").value).toUpperCase() == "TRUE" ? true : false,
        visible: true,
        width: "120px"
    });

    document.getElementById("tbMoveDetail").style.display = "";
    LoadingPanelDetailsClient.Hide();
}

function btnAcceptClick() {
    var wSwitch = $("#switchReliability").dxSwitch("instance");
    var valueswitch = wSwitch.option("value");

    var wSwitchTC = $("#switchTelecommuting").dxSwitch("instance");

    if (typeof wSwitchTC == 'undefined') {
        var valueswitchTC = false;
    }
    else {
        var valueswitchTC = wSwitchTC.option("value");
    }

    document.getElementById("PopDetails_CallbackPanelDetails_hdnDetailsReliabilityPop").value = valueswitch;
    document.getElementById("PopDetails_CallbackPanelDetails_hdnDetailsTelecommutingPop").value = valueswitchTC;

    var sType = cmbDetailsTypeClient.GetSelectedItem().value;
    if (sType == "10") {
        if (cmbDiningRoomTurnClient.GetSelectedItem().value == "0") {
            var lblErrorDetail = document.getElementById("PopDetails_CallbackPanelDetails_lblErrorDetail");
            lblErrorDetail.textContent = "Debe seleccionar un turno de comedor";
            var tdlblErrorDetail = document.getElementById("tdlblErrorDetail");
            tdlblErrorDetail.style.display = "";
            setTimeout(function () { lblErrorDetail.textContent = ""; document.getElementById("tdlblErrorDetail").style.display = "none"; }, 2000);
            return;
        }

        if (cmbTerminalClient.GetSelectedItem().value == "0") {
            var lblErrorDetail = document.getElementById("PopDetails_CallbackPanelDetails_lblErrorDetail");
            lblErrorDetail.textContent = "Debe seleccionar un terminal";
            var tdlblErrorDetail = document.getElementById("tdlblErrorDetail");
            tdlblErrorDetail.style.display = "";
            setTimeout(function () { lblErrorDetail.textContent = ""; document.getElementById("tdlblErrorDetail").style.display = "none"; }, 2000);
            return;
        }
    }

    if (sType == "13") {
        if (cmbDetailsCenterClient.GetSelectedItem().value == "0") {
            var lblErrorDetail = document.getElementById("PopDetails_CallbackPanelDetails_lblErrorDetail");
            lblErrorDetail.textContent = "Debe seleccionar un centro de coste";
            var tdlblErrorDetail = document.getElementById("tdlblErrorDetail");
            tdlblErrorDetail.style.display = "";
            setTimeout(function () { lblErrorDetail.textContent = ""; document.getElementById("tdlblErrorDetail").style.display = "none"; }, 2000);
            return;
        }
    }

    sMoveDate = document.getElementById("PopDetails_CallbackPanelDetails_hdnDetailsMoveTimePop").value;
    if (sMoveDate != "")
        sMoveDate = sMoveDate.substring(0, 5);

    var txtTime = txtDetailsTimeClient.GetText();
    var hdnDetailsMoveTime = document.getElementById("PopDetails_CallbackPanelDetails_hdnDetailsMoveTimePop");
    var hdnDetailsMoveDate = document.getElementById("PopDetails_CallbackPanelDetails_hdnDetailsMoveDatePop");

    if (txtTime != sMoveDate || txtTime == "") {
        hdnDetailsMoveTime.value = txtTime + ":00"
    }

    var hdnDetailsIdMovePop = document.getElementById("PopDetails_CallbackPanelDetails_hdnDetailsIdMovePop");
    var hdnDetailsHasChangesPop = document.getElementById("PopDetails_CallbackPanelDetails_hdnDetailsHasChangesPop");

    if (hdnDetailsIdMovePop.value == "0") {
        hdnDetailsHasChangesPop.value = "2"
    }
    else {
        hdnDetailsHasChangesPop.value = "1"
    }

    var hasChanges = document.getElementById("PopDetails_CallbackPanelDetails_hdnDetailsHasChangesPop").value;
    if (hasChanges != "0") {
        try {
            var type = document.getElementById("PopDetails_CallbackPanelDetails_hdnDetailsTypePop").value;
            var actualType = cmbDetailsTypeClient.GetSelectedItem().value;
            var sDate = document.getElementById("PopDetails_CallbackPanelDetails_hdnDetailsShiftDatePop").value;
            var mDate = document.getElementById("PopDetails_CallbackPanelDetails_hdnDetailsMoveDatePop").value;
            var time = document.getElementById("PopDetails_CallbackPanelDetails_hdnDetailsMoveTimePop").value;
            var terminal = cmbTerminalClient.GetSelectedItem().value;
            var action = document.getElementById("PopDetails_CallbackPanelDetails_hdnDetailsIdCurrentActionPop").value;
            var idpassport = document.getElementById("PopDetails_CallbackPanelDetails_hdnDetailsIdPassportModifiedPop").value;
            var row = document.getElementById("PopDetails_CallbackPanelDetails_hdnDetailsKeyRowPop").value;
            var city = document.getElementById("PopDetails_CallbackPanelDetails_hdnDetailsCityPop").value;
            var reliable = document.getElementById("PopDetails_CallbackPanelDetails_hdnDetailsReliabilityPop").value;
            var telecommuting = document.getElementById("PopDetails_CallbackPanelDetails_hdnDetailsTelecommutingPop").value;
            var zone = 0;
            if (typeof (cmbZoneClient) != 'undefined') {
                if (cmbZoneClient.GetSelectedIndex() > -1) {
                    zone = cmbZoneClient.GetSelectedItem().value;
                }
            }
            var position = document.getElementById("PopDetails_CallbackPanelDetails_hdnDetailsPositionPop").value;

            var cause = 0;
            var center = 0;
            var task = '';
            var idtask = 0;
            var field1 = '';
            var field2 = '';
            var field3 = '';
            var field4 = 0;
            var field5 = 0;
            var field6 = 0;

            field1 = txtFieldTask1Client.GetText();
            field2 = txtFieldTask2Client.GetText();
            field3 = txtFieldTask3Client.GetText();

            field4 = txtFieldTask4Client.GetText();
            field5 = txtFieldTask5Client.GetText();
            field6 = txtFieldTask6Client.GetText();

            if (actualType == 4) {
                task = document.getElementById("PopDetails_CallbackPanelDetails_txtTask").value;
                idtask = document.getElementById("PopDetails_CallbackPanelDetails_hdnDetailsIDTaskPop").value;
            }

            if (actualType != 4 && actualType != 10 && actualType != 13) {
                cause = cmbDetailsCauseClient.GetSelectedItem().value;
            }

            if (actualType == 10) {
                cause = cmbDiningRoomTurnClient.GetSelectedItem().value;
            }

            if (actualType == 13) {
                center = cmbDetailsCenterClient.GetSelectedItem().value;
            }

            var daysToAdd = "0";
            if (cmbDetailsDateSelectionClient.GetSelectedIndex() > -1) {
                daysToAdd = parseInt(cmbDetailsDateSelectionClient.GetSelectedItem().value); //dias a sumar o restar
            }

            if (action == "0") {
                if (hdnDetailsIdMovePop.value != "0") {
                    action = "2";
                }
            }

            var typeDetails = "";
            var invalidType = "-1";

            if (chkValidateDinningRoomClient.GetValue() == true) {
                invalidType = document.getElementById("PopDetails_CallbackPanelDetails_hdnDetailsInvalidTypePop").value;
                typeDetails = document.getElementById("PopDetails_CallbackPanelDetails_hdnDetailsTypeDetailsPop").value;
            }

            DetailsChange(hasChanges, type, actualType, sDate, mDate, time, terminal, cause, action, idpassport, row, city, zone, position, task, idtask, daysToAdd, field1, field2, field3, field4, field5, field6, invalidType, typeDetails, center, reliable, telecommuting);

            document.getElementById("PopDetails_CallbackPanelDetails_txtCity").value = "";
            document.getElementById("PopDetails_CallbackPanelDetails_hdnDetailsPositionPop").value = "";
            document.getElementById("PopDetails_CallbackPanelDetails_hdnDetailsCityPop").value = "";

            PopDetailsClient.Hide();
        }
        catch (e) {
            alert(e);
        }
    }
}

function CloseClick() {
    document.getElementById("PopDetails_CallbackPanelDetails_hdnDetailsPositionPop").value = "";
    document.getElementById("PopDetails_CallbackPanelDetails_txtCity").value = "";
    document.getElementById("PopDetails_CallbackPanelDetails_hdnDetailsCityPop").value = "";

    PopDetailsClient.Hide();
}

function cmbDetailsTypeClient_ValueChanged() {
    try {
        var sType = cmbDetailsTypeClient.GetSelectedItem().value;

        document.getElementById("PopDetails_CallbackPanelDetails_divCauseCaption").style.display = 'none';
        document.getElementById("PopDetails_CallbackPanelDetails_divCauseCmb").style.display = 'none';
        document.getElementById("PopDetails_CallbackPanelDetails_divCenterCaption").style.display = 'none';
        document.getElementById("PopDetails_CallbackPanelDetails_divCenterCmb").style.display = 'none';

        if (sType == "4") {
            txtFieldTask4Client.SetVisible(true);
            txtFieldTask4Client.SetText("0");

            txtFieldTask5Client.SetVisible(true);
            txtFieldTask5Client.SetText("0");

            txtFieldTask6Client.SetVisible(true);
            txtFieldTask6Client.SetText("0");

            document.getElementById("PopDetails_CallbackPanelDetails_txtTask").style.display = '';
            document.getElementById("PopDetails_CallbackPanelDetails_lblTask").style.display = '';
            document.getElementById("PopDetails_CallbackPanelDetails_ImgTask").style.display = '';
            document.getElementById("PopDetails_CallbackPanelDetails_txtTask").value = '';

            document.getElementById("PopDetails_CallbackPanelDetails_lblFieldTask1").style.display = '';
            document.getElementById("PopDetails_CallbackPanelDetails_txtFieldTask1").style.display = '';
            document.getElementById("PopDetails_CallbackPanelDetails_txtFieldTask1").value = '';

            document.getElementById("PopDetails_CallbackPanelDetails_lblFieldTask2").style.display = '';
            document.getElementById("PopDetails_CallbackPanelDetails_txtFieldTask2").style.display = '';
            document.getElementById("PopDetails_CallbackPanelDetails_txtFieldTask2").value = '';

            document.getElementById("PopDetails_CallbackPanelDetails_lblFieldTask3").style.display = '';
            document.getElementById("PopDetails_CallbackPanelDetails_txtFieldTask3").style.display = '';
            document.getElementById("PopDetails_CallbackPanelDetails_txtFieldTask3").value = '';

            document.getElementById("PopDetails_CallbackPanelDetails_lblFieldTask4").style.display = '';

            document.getElementById("PopDetails_CallbackPanelDetails_lblFieldTask5").style.display = '';

            document.getElementById("PopDetails_CallbackPanelDetails_lblFieldTask6").style.display = '';

            document.getElementById("PopDetails_CallbackPanelDetails_divTaskFields").style.display = ''
            document.getElementById("PopDetails_CallbackPanelDetails_divTaskFields2").style.display = ''

            document.getElementById("PopDetails_CallbackPanelDetails_hdnDetailsIDTaskPop").value = '0';
        }
        else {
            if (sType == "13") {
                document.getElementById("PopDetails_CallbackPanelDetails_divCenterCaption").style.display = '';
                document.getElementById("PopDetails_CallbackPanelDetails_divCenterCmb").style.display = '';
            }
            else {
                document.getElementById("PopDetails_CallbackPanelDetails_divCauseCaption").style.display = '';
                document.getElementById("PopDetails_CallbackPanelDetails_divCauseCmb").style.display = '';
            }

            document.getElementById("PopDetails_CallbackPanelDetails_txtTask").style.display = 'none';
            document.getElementById("PopDetails_CallbackPanelDetails_lblTask").style.display = 'none';
            document.getElementById("PopDetails_CallbackPanelDetails_ImgTask").style.display = 'none';

            document.getElementById("PopDetails_CallbackPanelDetails_lblFieldTask1").style.display = 'none';
            document.getElementById("PopDetails_CallbackPanelDetails_txtFieldTask1").style.display = 'none';

            document.getElementById("PopDetails_CallbackPanelDetails_lblFieldTask2").style.display = 'none';
            document.getElementById("PopDetails_CallbackPanelDetails_txtFieldTask2").style.display = 'none';

            document.getElementById("PopDetails_CallbackPanelDetails_lblFieldTask3").style.display = 'none';
            document.getElementById("PopDetails_CallbackPanelDetails_txtFieldTask3").style.display = 'none';

            document.getElementById("PopDetails_CallbackPanelDetails_lblFieldTask4").style.display = 'none';

            txtFieldTask4Client.SetVisible(false);
            txtFieldTask5Client.SetVisible(false);
            txtFieldTask6Client.SetVisible(false);

            document.getElementById("PopDetails_CallbackPanelDetails_lblFieldTask5").style.display = 'none';
            document.getElementById("PopDetails_CallbackPanelDetails_lblFieldTask6").style.display = 'none';

            document.getElementById("PopDetails_CallbackPanelDetails_divTaskFields").style.display = 'none'
            document.getElementById("PopDetails_CallbackPanelDetails_divTaskFields2").style.display = 'none'
        }

        if (parseInt(document.getElementById("PopDetails_CallbackPanelDetails_hdnDetailsIdMovePop").value) <= 0 && (parseInt(sType) < 5 || parseInt(sType) == 13)) {
            document.getElementById("PopDetails_CallbackPanelDetails_hdnDetailsTypePop").value = sType;
        }

        if (parseInt(document.getElementById("PopDetails_CallbackPanelDetails_hdnDetailsIdMovePop").value) > 0) {
            document.getElementById("PopDetails_CallbackPanelDetails_hdnDetailsIdCurrentActionPop").value = '3';
        }
    }
    catch (e) {
        alert(e);
    }
}

function ShowMap() {
    var Title = '';
    var url = 'SetLocalization.aspx';
    var position = document.getElementById("PopDetails_CallbackPanelDetails_hdnDetailsPositionPop").value;
    var city = document.getElementById("PopDetails_CallbackPanelDetails_txtCity").value;
    var param = '';
    if (position.length > 0) {
        param += '?pos=' + position;
    }
    if (city.length > 0) {
        if (param.length > 0)
            param += '&';
        else
            param += '?';
        param += 'city=' + city;
    }
    url += param;
    ShowExternalForm2(url, 470, 420, Title, '', false, false, false, 19000);
}

function ShowTasks() {
    try {
        // Muestra la pàgina de selección de horario
        var Title = '';
        var hBase = "../Base/WebUserControls/roFilterListSelector.aspx";
        ShowExternalForm2(hBase, 300, 270, Title, '', false, false, false, 19000);
    }
    catch (e) {
        alert(e);
    }
}

function ShowField(IDField) {
    try {
        // Muestra la pàgina de selección de lista de valores
        var Title = '';
        var hBase = "../Base/WebUserControls/roFilterListValues.aspx?IDField=" + IDField;
        ShowExternalForm2(hBase, 300, 270, Title, '', false, false, false, 19000);
    }
    catch (e) {
        alert(e);
    }
}

function CityChange(city, position) {
    document.getElementById("PopDetails_CallbackPanelDetails_txtCity").value = city;
    document.getElementById("PopDetails_CallbackPanelDetails_hdnDetailsPositionPop").value = position;
    document.getElementById("PopDetails_CallbackPanelDetails_hdnDetailsCityPop").value = city;
}

function TaskChange(IDTask, NameTask) {
    document.getElementById("PopDetails_CallbackPanelDetails_txtTask").value = NameTask;
    document.getElementById("PopDetails_CallbackPanelDetails_txtTask").title = NameTask;
    document.getElementById("PopDetails_CallbackPanelDetails_hdnDetailsIDTaskPop").value = IDTask;
}

function ListChange(IDField, NameValue) {
    switch (IDField) {
        case "1":
            txtFieldTask1Client.SetValue(NameValue);
            document.getElementById("PopDetails_CallbackPanelDetails_hdnDetailsField1Pop").value = NameValue;
            break;
        case "2":
            txtFieldTask2Client.SetValue(NameValue);
            document.getElementById("PopDetails_CallbackPanelDetails_hdnDetailsField2Pop").value = NameValue;
            break;
        case "3":
            txtFieldTask3Client.SetValue(NameValue);
            document.getElementById("PopDetails_CallbackPanelDetails_hdnDetailsField3Pop").value = NameValue;
            break;
        case "4":
            txtFieldTask4Client.SetValue(NameValue);
            document.getElementById("PopDetails_CallbackPanelDetails_hdnDetailsField4Pop").value = NameValue;
            break;
        case "5":
            txtFieldTask5Client.SetValue(NameValue);
            document.getElementById("PopDetails_CallbackPanelDetails_hdnDetailsField5Pop").value = NameValue;
            break;
        case "6":
            txtFieldTask6Client.SetValue(NameValue);
            document.getElementById("PopDetails_CallbackPanelDetails_hdnDetailsField6Pop").value = NameValue;
            break;
    }
}

function jasonificaMoveDetail(isnewmove, canedit, stateofrow, idmove, keyrow, movetype, actualtype, idterminal, idaction, idcause_idtask_iddiningroom, reliability, idzone, actualdate,
    shiftdate, position, city, idpassport, field1, field2, field3, field4, field5, field6, invalidType, typeDetails, timeZone, center, fullAddress, maskAlert, temperatureAlert, verifyType, telecommuting, IDRequest) {
    if (typeof (isnewmove) == "undefined" || isnewmove == null || isnewmove.toUpperCase() == "TRUE") isnewmove = "true"; else isnewmove = "false";
    if (typeof (canedit) == "undefined" || canedit == null || canedit.toUpperCase() == "TRUE") canedit = "true"; else canedit = "false";
    if (typeof (stateofrow) == "undefined" || stateofrow == null) stateofrow = "";
    if (typeof (idmove) == "undefined" || idmove == null) idmove = 0;
    if (typeof (keyrow) == "undefined" || keyrow == null) keyrow = "";
    if (typeof (movetype) == "undefined" || movetype == null) movetype = 0;
    if (typeof (actualtype) == "undefined" || actualtype == null) actualtype = 0;
    if (typeof (actualtype) == "undefined" || actualtype == "" || actualtype == null) actualtype = 0;
    if (typeof (idterminal) == "undefined" || idterminal == "" || idterminal == null) idterminal = 0;
    if (typeof (idaction) == "undefined" || idaction == "" || idaction == null) idaction = 0;
    if (typeof (idcause_idtask_iddiningroom) == "undefined" || idcause_idtask_iddiningroom == "" || idcause_idtask_iddiningroom == null) idcause_idtask_iddiningroom = 0;
    if (typeof (reliability) == "undefined" || reliability == null || reliability.toUpperCase() == "TRUE") reliability = "true"; else reliability = "false";
    if (typeof (telecommuting) == "undefined" || telecommuting == null || telecommuting.toUpperCase() == "TRUE") telecommuting = "true"; else telecommuting = "false";
    if (typeof (idzone) == "undefined" || idzone == "" || idzone == null) idzone = 0;
    if (typeof (actualdate) == "undefined" || actualdate == null) actualdate = "";
    if (typeof (shiftdate) == "undefined" || shiftdate == null) shiftdate = "";
    if (typeof (position) == "undefined" || position == null) position = "";
    if (typeof (city) == "undefined" || city == null) city = "";
    if (typeof (idpassport) == "undefined" || idpassport == "" || idpassport == null) idpassport = 0;

    if (typeof (field1) == "undefined" || field1 == null) field1 = "";
    if (typeof (field2) == "undefined" || field2 == null) field2 = "";
    if (typeof (field3) == "undefined" || field3 == null) field3 = "";
    if (typeof (field4) == "undefined" || field4 == null) field4 = 0;
    if (typeof (field5) == "undefined" || field5 == null) field5 = 0;
    if (typeof (field6) == "undefined" || field6 == null) field6 = 0;

    if (typeof (invalidType) == "undefined" || invalidType == null) invalidType = -1;
    if (typeof (typeDetails) == "undefined" || typeDetails == null) typeDetails = "";
    if (typeof (timeZone) == "undefined" || timeZone == null) timeZone = "";
    if (typeof (fullAddress) == "undefined" || fullAddress == null) fullAddress = "";
    if (typeof (center) == "undefined" || center == null) center = 0;

    if (typeof (maskAlert) == "undefined" || maskAlert == null) maskAlert = -1;
    if (typeof (temperatureAlert) == "undefined" || temperatureAlert == null) temperatureAlert = -1;
    if (typeof (verifyType) == "undefined" || verifyType == null) verifyType = 0;

    if (typeof (IDRequest) == "undefined" || IDRequest == null) IDRequest = -1;

    var oParameters = {};

    oParameters.isnewmove = isnewmove;
    oParameters.canedit = canedit;
    oParameters.stateofrow = stateofrow;
    oParameters.idmove = idmove;
    oParameters.keyrow = keyrow;
    oParameters.movetype = movetype;
    oParameters.actualtype = actualtype;
    oParameters.idterminal = idterminal;
    oParameters.idaction = idaction;
    oParameters.idcause_idtask_iddiningroom = idcause_idtask_iddiningroom;
    oParameters.reliability = reliability;
    oParameters.idzone = idzone;
    oParameters.actualdate = actualdate;
    oParameters.shiftdate = shiftdate;
    oParameters.position = position;
    oParameters.city = city;
    oParameters.idpassport = idpassport;
    oParameters.field1 = field1;
    oParameters.field2 = field2;
    oParameters.field3 = field3;
    oParameters.field4 = field4;
    oParameters.field5 = field5;
    oParameters.field6 = field6;
    oParameters.invalidType = invalidType;
    oParameters.typeDetails = typeDetails;
    oParameters.timeZone = timeZone;
    oParameters.fullAddress = fullAddress;
    oParameters.center = center;
    oParameters.maskAlert = maskAlert;
    oParameters.temperatureAlert = temperatureAlert;
    oParameters.verifyType = verifyType;
    oParameters.telecommuting = telecommuting;
    oParameters.IDRequest = IDRequest;

    var strParameters = JSON.stringify(oParameters);
    strParameters = encodeURIComponent(strParameters);
    return strParameters;
}

function auditAndShowPunchImage() {
    $("#dvSummaryLoadPunch").css('display', 'none');
    $("#dvEmployeePunch").css('display', '');
    $("#imgEmployeePunch").attr('src', $('#imgEmployeeCapture').attr('attr-urlPunch'));
}

// You can create the Switch widget using the following code.

// Read more at https://js.devexpress.com/Documentation/18_1/Guide/UI_Widgets/Basics/Widget_Basics_-_jQuery/.