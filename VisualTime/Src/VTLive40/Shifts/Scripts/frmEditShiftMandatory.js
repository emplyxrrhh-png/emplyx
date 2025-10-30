function frmEditShiftMandatory_Show(arrNames, arrValues) {
    try {
        let shiftType = 'unknown';

        if (typeof curShiftType != 'undefined') shiftType = curShiftType;

        var objPrTab = "ctl00_contentMainBody_ASPxCallbackPanelContenido_frmEditShiftMandatory_tbCont1_";
        var n;
        var oDis = document.getElementById('ctl00_contentMainBody_hdnModeEdit').value;
        //Valors pre-establers
        loadMandatoryBlanks();

        if (oDis == "false") {
            cmbStartSelMandatoryClient.SetEnabled(true);
            if (shiftType === 'PerHours') cmbStartSelMandatoryClient.SetEnabled(false);
        }

        for (n = 0; n < arrNames.length; n++) {
            switch (arrNames[n].toLowerCase()) {
                case "rolt_tlid":
                    loadGenericData("LAYERID", "hdnMandatoryLayerID", arrValues[n], "X_NUMBER", "", oDis, false);
                    break;
                case "rolt_begin":
                    txtStartAt1MandatoryClient.SetValue(new Date('1900/01/01 ' + arrValues[n] + ':00'));
                    txtStartAt2FromMandatoryClient.SetValue(new Date('1900/01/01 ' + arrValues[n] + ':00'));
                    break;
                case "rolt_beginday":
                    cmbStartAt1MandatoryClient.SetValue(arrValues[n]);
                    cmbStartAt2AFromMandatoryClient.SetValue(arrValues[n]);
                    break;
                case "rolt_finish":
                    txtEndAt1MandatoryClient.SetValue(new Date('1900/01/01 ' + arrValues[n] + ':00'));
                    break;
                case "rolt_finishday":
                    cmbEndAt1MandatoryClient.SetValue(arrValues[n]);
                    break;
                case "rolt_datediffmin":
                    break;
                case "rolt_parentid":
                    if (arrValues[n] != "") {
                        loadGenericData("PARENTID", "hdnMandatoryParentID", arrValues[n], "X_NUMBER", "", oDis, false);
                    }
                    break;
                case "rolt_floatingbeginupto":
                    if (arrValues[n] != "") {
                        cmbStartSelMandatoryClient.SetValue(1);
                        txtStartAt2ToMandatoryClient.SetValue(new Date('1900/01/01 ' + arrValues[n] + ':00'));
                        Mandatory_ShowPanEntrance(2);
                        if (curShiftType === 'PerHours')
                            ShowAdvancedCheckIni(cmbStartSelMandatoryClient);
                    }
                    break;
                case "rolt_floatingbeginuptoday":
                    if (arrValues[n] != "") {
                        cmbStartAt2AToMandatoryClient.SetValue(arrValues[n]);
                    }
                    break;
                case "rolt_floatingfinishminutes":
                    //TODO: Calcular los minutos para insertar el final
                    if (arrValues[n] != "") {
                        //alert(arrValues[n]);
                        var strHourResult = Mandatory_ConvertMinutesToHour(arrValues[n]);
                        txtEndAt2MandatoryClient.SetValue(new Date('1900/01/01 ' + strHourResult + ':00'));
                        cmbEndSelMandatoryClient.SetValue(1);
                        if (curShiftType === 'PerHours')
                            ShowAdvancedCheckEnd(cmbEndSelMandatoryClient);
                        Mandatory_ShowPanExit(2);
                    }
                    break;
                case "rolt_unit_1020_id":
                    loadGenericData("LAYER1020ID", "hdnMandatory1020LayerID", arrValues[n], "X_NUMBER", "", oDis, false);
                    break;
                case "rolt_unit_1020_parentid":
                    loadGenericData("LAYER1020PARENTID", "hdnMandatory1020ParentID", arrValues[n], "X_NUMBER", "", oDis, false);
                    break;
                case "rolt_unit_1021_id":
                    loadGenericData("LAYER1021ID", "hdnMandatory1021LayerID", arrValues[n], "X_NUMBER", "", oDis, false);
                    break;
                case "rolt_unit_1021_parentid":
                    loadGenericData("LAYER1021PARENTID", "hdnMandatory1021ParentID", arrValues[n], "X_NUMBER", "", oDis, false);
                    break;
                case "rolt_unit_1022_id":
                    loadGenericData("LAYER1022ID", "hdnMandatory1022LayerID", arrValues[n], "X_NUMBER", "", oDis, false);
                    break;
                case "rolt_unit_1022_parentid":
                    loadGenericData("LAYER1022PARENTID", "hdnMandatory1022ParentID", arrValues[n], "X_NUMBER", "", oDis, false);
                    break;
                case "rolt_unit_1020_value":
                    if (arrValues[n] != "") {
                        txtRetInfMandatoryClient.SetValue(new Date('1900/01/01 ' + arrValues[n] + ':00'));
                        loadGenericData("CHKRETINF", "chkRetInf", "true", "X_CHECKBOX", "", oDis, false);
                    }
                    break;
                case "rolt_unit_1020_action":
                    var oAction1020 = 0;
                    if (arrValues[n].toLowerCase() != "treataswork") { oAction1020 = 1; }
                    cmbRetInfMandatoryClient.SetValue(oAction1020);
                    break;
                case "rolt_unit_1021_value":
                    if (arrValues[n] != "") {
                        txtIntInfMandatoryClient.SetValue(new Date('1900/01/01 ' + arrValues[n] + ':00'));
                        loadGenericData("CHKINTINF", "chkIntInf", "true", "X_CHECKBOX", "", oDis, false);
                    }
                    break;
                case "rolt_unit_1021_action":
                    var oAction1021 = 0;
                    if (arrValues[n].toLowerCase() != "treataswork") { oAction1021 = 1; }
                    cmbIntInfMandatoryClient.SetValue(oAction1021);
                    break;
                case "rolt_unit_1022_value":
                    if (arrValues[n] != "") {
                        txtSalAntMandatoryClient.SetValue(new Date('1900/01/01 ' + arrValues[n] + ':00'));
                        loadGenericData("CHKSALANT", "chkSalAnt", "true", "X_CHECKBOX", "", oDis, false);
                    }
                    break;
                case "rolt_unit_1022_action":
                    var oAction1022 = 0;
                    if (arrValues[n].toLowerCase() != "treataswork") { oAction1022 = 1; }
                    cmbSalAntMandatoryClient.SetValue(oAction1022);
                    break;
                case "rolt_allowmodini":
                    if (arrValues[n] != "" && arrValues[n].toUpperCase() === 'TRUE') {
                        chkmodifyini.SetChecked(true);
                        cmbStartSelMandatoryClient.SetValue(0);
                    }
                    ShowAdvancedCheckIni(cmbStartSelMandatoryClient);
                    break;
                case "rolt_allowmodduration":
                    if (arrValues[n] != "" && arrValues[n].toUpperCase() === 'TRUE') {
                        chkmodifyduration.SetChecked(true);
                        cmbEndSelMandatoryClient.SetValue(1);
                    }
                    ShowAdvancedCheckEnd(cmbEndSelMandatoryClient);
                    break;
            }
        }

        activeTabContainer('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmEditShiftMandatory_tbCont1', 0);

        //show te form
        showWUF('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmEditShiftMandatory', true);
    } catch (e) { showError("frmEditShiftMandatory_Show", e); }
}

function frmEditShiftMandatory_Close(grabar) {
    try {
        //show te form
        showWUF('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmEditShiftMandatory', false);
    } catch (e) { showError("frmEditShiftMandatory_Close", e); }
}

function frmEditShiftMandatory_Load() {
    try {
        //Carrega dels Camps

        frmEditShiftMandatory_Show();
    } catch (e) { showError("frmEditShiftMandatory_Load", e); }
}

function frmEditShiftMandatory_Validate() {
    try {
        var objPrTab = "ctl00_contentMainBody_ASPxCallbackPanelContenido_frmEditShiftMandatory_tbCont1_";

        var cmbStartSel_Value = cmbStartSelMandatoryClient.GetValue();

        if (cmbStartSel_Value == 0) { //hora fija
            if (Mandatory_CheckTime(txtStartAt1MandatoryClient)) { return false; }
        } else { // entre dos horas
            if (Mandatory_CheckTime(txtStartAt2FromMandatoryClient)) { return false; }
            if (Mandatory_CheckTime(txtStartAt2ToMandatoryClient)) { return false; }
        } //end if

        var cmbEndSel_Value = cmbEndSelMandatoryClient.GetValue();

        if (cmbEndSel_Value == 0) { //hora fija
            if (Mandatory_CheckTime(txtEndAt1MandatoryClient)) { return false; }
        } else { //segun entrada
            if (Mandatory_CheckTime(txtEndAt2MandatoryClient)) { return false; }
        } //end if

        //Tab filtros
        var chkRetInf = document.getElementById('chkRetInf');
        var chkIntInf = document.getElementById('chkIntInf');
        var chkSalAnt = document.getElementById('chkSalAnt');

        if (chkRetInf.checked) {
            if (Mandatory_CheckTime(txtRetInfMandatoryClient)) { return false; }
        }

        if (chkIntInf.checked) {
            if (Mandatory_CheckTime(txtIntInfMandatoryClient)) { return false; }
        }

        if (chkSalAnt.checked) {
            if (Mandatory_CheckTime(txtIntInfMandatoryClient)) { return false; }
        }
        return true;
    } catch (e) {
        showError("frmEditShiftMandatory_Load", e);
        return false;
    }
}

function frmEditShiftMandatory_Save() {
    try {
        if (document.getElementById('ctl00_contentMainBody_hdnModeEdit').value != "false") { frmEditShiftMandatory_Close(); return; }
        if (frmEditShiftMandatory_Validate()) {
            var oMF = Mandatory_FieldsToJSON();

            var IdLayer = document.getElementById('hdnMandatoryLayerID').value;
            if (IdLayer != "-1") {
                oTMDefinitions.updateTimeZone("roLTMandatory", IdLayer, oMF);
            } else {
                oTMDefinitions.createTimeZone("roLTMandatory", oMF);
            }
            hasChanges(true);
            frmEditShiftMandatory_Close(true);
        }
    } catch (e) { showError("frmEditShiftMandatory_Load", e); }
}

function frmEditShiftMandatory_ShowNew() {
    try {
        loadMandatoryBlanks();
        activeTabContainer('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmEditShiftMandatory_tbCont1', 0);

        let shiftType = 'unknown';
        var oDis = document.getElementById('ctl00_contentMainBody_hdnModeEdit').value;

        if (oDis == "false") {
            cmbStartSelMandatoryClient.SetEnabled(true);
            if (typeof curShiftType != 'undefined') shiftType = curShiftType;
            if (shiftType === 'PerHours') cmbStartSelMandatoryClient.SetEnabled(false);
        }

        //show te form
        showWUF('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmEditShiftMandatory', true);
    } catch (e) { showError("frmEditShiftMandatory_Show", e); }
}

function Mandatory_ShowPanEntrance(pan) {
    try {
        if (pan == 1) {
            document.getElementById('panEntrance1').style.display = '';
            document.getElementById('panEntrance2').style.display = 'none';
        } else {
            document.getElementById('panEntrance1').style.display = 'none';
            document.getElementById('panEntrance2').style.display = '';
        }
    } catch (e) { showError("Mandatory_ShowPanEntrance", e); }
}

function Mandatory_ShowPanExit(pan) {
    try {
        if (pan == 1) {
            document.getElementById('panExit1').style.display = '';
            document.getElementById('panExit2').style.display = 'none';
        } else {
            document.getElementById('panExit1').style.display = 'none';
            document.getElementById('panExit2').style.display = '';
        }
    } catch (e) { showError("Mandatory_ShowPanExit", e); }
}

function loadMandatoryBlanks() {
    try {
        var objPrTab = "ctl00_contentMainBody_ASPxCallbackPanelContenido_frmEditShiftMandatory_tbCont1_";
        var oDis = document.getElementById('ctl00_contentMainBody_hdnModeEdit').value;
        var showEntrance = 1;
        var showExit = 1;
        //blank fields
        loadGenericData("LAYERID", "hdnMandatoryLayerID", "-1", "X_NUMBER", "", oDis, false);
        loadGenericData("PARENTID", "hdnMandatoryParentID", "-1", "X_NUMBER", "", oDis, false);

        cmbStartSelMandatoryClient.SetValue(0);
        txtStartAt1MandatoryClient.SetValue(new Date('1900/01/01 00:00:00'));
        cmbStartAt1MandatoryClient.SetValue(1);
        txtStartAt2FromMandatoryClient.SetValue(new Date('1900/01/01 00:00:00'));
        txtStartAt2ToMandatoryClient.SetValue(new Date('1900/01/01 00:00:00'));
        cmbStartAt2AFromMandatoryClient.SetValue(1);
        cmbStartAt2AToMandatoryClient.SetValue(1);
        cmbEndSelMandatoryClient.SetValue(0);
        txtEndAt1MandatoryClient.SetValue(new Date('1900/01/01 00:00:00'));
        cmbEndAt1MandatoryClient.SetValue(1);
        txtEndAt2MandatoryClient.SetValue(new Date('1900/01/01 00:00:00'));
        txtRetInfMandatoryClient.SetValue(new Date('1900/01/01 00:00:00'));
        cmbRetInfMandatoryClient.SetValue(0);
        txtIntInfMandatoryClient.SetValue(new Date('1900/01/01 00:00:00'));
        cmbIntInfMandatoryClient.SetValue(0);
        txtSalAntMandatoryClient.SetValue(new Date('1900/01/01 00:00:00'));
        cmbSalAntMandatoryClient.SetValue(0);
        ShowAdvancedCheckIni(cmbStartSelMandatoryClient);
        ShowAdvancedCheckEnd(cmbEndSelMandatoryClient);
        chkmodifyini.SetChecked(false);
        chkmodifyduration.SetChecked(false);

        if (listParameters.length > 0) {
            var someLayer = listParameters[0];
            var someChekIni = someLayer.IniCheck;
            var someChekDur = someLayer.DurCheck;
            if (someChekIni) {
                cmbStartSelMandatoryClient.SetValue(0);
                ShowAdvancedCheckIni(cmbStartSelMandatoryClient);
                chkmodifyini.SetChecked(true);
            }
            if (someChekDur) {
                cmbEndSelMandatoryClient.SetValue(1);
                ShowAdvancedCheckEnd(cmbEndSelMandatoryClient);
                chkmodifyduration.SetChecked(true);
                showExit = 0;
            }
        }

        //Filtres
        loadGenericData("CHKRETINF", "chkRetInf", "false", "X_CHECKBOX", "", oDis, false);
        loadGenericData("CHKINTINF", "chkIntInf", "false", "X_CHECKBOX", "", oDis, false);
        loadGenericData("CHKSALANT", "chkSalAnt", "false", "X_CHECKBOX", "", oDis, false);

        Mandatory_ShowPanEntrance(showEntrance);
        Mandatory_ShowPanExit(showExit);
    } catch (e) { showError("loadMandatoryBlanks", e); }
}

function Mandatory_CalculateFinalHour(minutesToAdd, cmbStartSelValue) {
    try {
        var tStart2 = null;
        tStart2 = (cmbStartSelValue == 1) ? txtStartAt2ToMandatoryClient.GetValue().format2Time() : txtStartAt1MandatoryClient.GetValue().format2Time();

        var tStart2Hour = tStart2.split(":")[0]; //Hora
        var tStart2Min = tStart2.split(":")[1];  //Minutos

        if (tStart2Hour.length == 2 && tStart2Hour.startsWith("0")) { tStart2Hour = tStart2Hour.substring(1); }
        if (tStart2Min.length == 2 && tStart2Min.startsWith("0")) { tStart2Min = tStart2Min.substring(1); }

        //Definim la data per poder fer la suma
        var hStart2 = new Date();

        hStart2.setFullYear(1989, 11, 30);
        hStart2.setHours(tStart2Hour, tStart2Min, 0, 0);

        var HourResult = new Date();
        HourResult = dateAdd("n", minutesToAdd, hStart2);

        var strHour = HourResult.getHours().toString();
        if (strHour.length == 1) { strHour = "0" + strHour; }
        var strMinutes = HourResult.getMinutes().toString();
        if (strMinutes.length == 1) { strMinutes = "0" + strMinutes; }

        var strHourResult = strHour + ':' + strMinutes;

        return strHourResult
    } catch (e) { showError("Mandatory_CalculateFinalHour", e); }
}

function Mandatory_ConvertMinutesToHour(Minutes) {
    try {
        var Hours = Math.floor(parseInt(Minutes) / 60);
        var MinutesRest = "00";
        if ((parseInt(Minutes) ^ 60) > 0) { //Si no son horas justas, sacar los minutos
            MinutesRest = parseInt(Minutes) - (Hours * 60);
        }

        if (Hours.toString().length == 1) { Hours = "0" + Hours; }
        if (MinutesRest.toString().length == 1) { MinutesRest = "0" + MinutesRest; }

        return Hours + ":" + MinutesRest;
    } catch (e) { showError("Mandatory_ConvertMinutesToHour", e); }
}

function Mandatory_ConvertHoursToMinutes(Hours) {
    try {
        var HourStr = Hours.split(":")[0]; if (HourStr.substr(0, 1) == "0") HourStr = HourStr.substr(1, 1);
        var Hour = parseInt(HourStr);
        var MinutesStr = Hours.split(":")[1]; if (MinutesStr.substr(0, 1) == "0") MinutesStr = MinutesStr.substr(1, 1);
        var Minutes = parseInt(MinutesStr);
        var oResult = 0;

        oResult = Hour * 60;
        oResult = parseInt(oResult + Minutes);

        //alert(oResult);
        return oResult;
    } catch (e) { showError("Mandatory_ConvertHoursToMinutes", e); return 0; }
}

function Mandatory_FieldsToJSON() {
    try {
        var objPrTab = "ctl00_contentMainBody_ASPxCallbackPanelContenido_frmEditShiftMandatory_tbCont1_";

        var Begin;
        var BeginDay;
        var Finish;
        var FinishDay;
        var FloatingBeginUpTo = "";
        var FloatingBeginUpToDay = "";
        var FloatingFinishMinutes = "";
        var Unit_1020_Value = "";
        var Unit_1020_Action = "";
        var Unit_1020_Begin = "";
        var Unit_1020_BeginDay = "";
        var Unit_1020_Finish = "";
        var Unit_1020_FinishDay = "";

        var Unit_1021_Value = "";
        var Unit_1021_Action = "";
        var Unit_1021_Begin = "";
        var Unit_1021_BeginDay = "";
        var Unit_1021_Finish = "";
        var Unit_1021_FinishDay = "";

        var Unit_1022_Value = "";
        var Unit_1022_Action = "";
        var Unit_1022_Begin = "";
        var Unit_1022_BeginDay = "";
        var Unit_1022_Finish = "";
        var Unit_1022_FinishDay = "";

        var cmbStartSelValue = cmbStartSelMandatoryClient.GetValue();
        var cmbEndSelValue = cmbEndSelMandatoryClient.GetValue();
        if (cmbStartSelValue == 0) {
            Begin = txtStartAt1MandatoryClient.GetValue().format2Time();
            BeginDay = cmbStartAt1MandatoryClient.GetValue();
            //FloatingBeginUpTo = Begin;
            //FloatingBeginUpToDay = BeginDay;
        }
        else {
            Begin = txtStartAt2FromMandatoryClient.GetValue().format2Time();
            BeginDay = cmbStartAt2AFromMandatoryClient.GetValue();
            FloatingBeginUpTo = txtStartAt2ToMandatoryClient.GetValue().format2Time();
            FloatingBeginUpToDay = cmbStartAt2AToMandatoryClient.GetValue();
        }

        Finish = txtEndAt1MandatoryClient.GetValue().format2Time();
        FinishDay = cmbEndAt1MandatoryClient.GetValue();

        if (cmbEndSelValue == 1) {
            FloatingFinishMinutes = Mandatory_ConvertHoursToMinutes(txtEndAt2MandatoryClient.GetValue().format2Time());
            Finish = Mandatory_CalculateFinalHour(Mandatory_ConvertHoursToMinutes(txtEndAt2MandatoryClient.GetValue().format2Time()), cmbStartSelValue);
            FinishDay = cmbStartAt2AToMandatoryClient.GetValue();
            //PPR
            var MinutesBegin = 0;
            if (cmbStartSelValue == 0) {
                MinutesBegin = Mandatory_ConvertHoursToMinutes(Begin);
            }
            else {
                MinutesBegin = Mandatory_ConvertHoursToMinutes(FloatingBeginUpTo);
            }

            MinutesBegin = MinutesBegin + FloatingFinishMinutes;
            if (MinutesBegin >= 1440)
                FinishDay = parseInt(FloatingBeginUpToDay != '' ? FloatingBeginUpToDay : BeginDay) + 1
            else
                FinishDay = FloatingBeginUpToDay != '' ? FloatingBeginUpToDay : BeginDay;
        }

        var chkRetInf = document.getElementById('chkRetInf');
        var chkIntInf = document.getElementById('chkIntInf');
        var chkSalAnt = document.getElementById('chkSalAnt');
        AllowModifyIniHour = false;
        AllowModifyDuration = false;
        if (curShiftType === 'PerHours') {
            var iniChecked = chkmodifyini.GetChecked();
            var durChecked = chkmodifyduration.GetChecked();
            var oNewParameters = listParameters[layerNumber - 1];
            if (oNewParameters != null) {
                oNewParameters.IniCheck = iniChecked;
                oNewParameters.DurCheck = durChecked;
            }
            else {
                var oNewParameters = {};
                oNewParameters.IniCheck = iniChecked;
                oNewParameters.DurCheck = durChecked;
                oNewParameters.IdTimeZone = document.getElementById('hdnMandatoryLayerID').value;
                oNewParameters.LayerNumber = layerNumber;
                listParameters.push(oNewParameters)
                layerNumber += 1;
            }
            AllowModifyIniHour = iniChecked;
            AllowModifyDuration = durChecked;
        }

        if (chkRetInf.checked) {
            Unit_1020_Value = txtRetInfMandatoryClient.GetValue().format2Time();
            var cmbRetInfValue = cmbRetInfMandatoryClient.GetValue();
            if (cmbRetInfValue == 0) {
                Unit_1020_Action = "TreatAsWork"
            } else {
                Unit_1020_Action = "Ignore"
            }
            Unit_1020_Begin = Begin;
            Unit_1020_BeginDay = BeginDay;
            Unit_1020_Finish = Finish;
            Unit_1020_FinishDay = FinishDay;
        }

        if (chkIntInf.checked) {
            Unit_1021_Value = txtIntInfMandatoryClient.GetValue().format2Time();
            var cmbIntInfValue = cmbIntInfMandatoryClient.GetValue();
            if (cmbIntInfValue == 0) {
                Unit_1021_Action = "TreatAsWork"
            } else {
                Unit_1021_Action = "Ignore"
            }
            Unit_1021_Begin = Begin;
            Unit_1021_BeginDay = BeginDay;
            Unit_1021_Finish = Finish;
            Unit_1021_FinishDay = FinishDay;
        }

        if (chkSalAnt.checked) {
            Unit_1022_Value = txtSalAntMandatoryClient.GetValue().format2Time();
            var cmbSalAntValue = cmbSalAntMandatoryClient.GetValue();
            if (cmbSalAntValue == 0) {
                Unit_1022_Action = "TreatAsWork"
            } else {
                Unit_1022_Action = "Ignore"
            }
            Unit_1022_Begin = Begin;
            Unit_1022_BeginDay = BeginDay;
            Unit_1022_Finish = Finish;
            Unit_1022_FinishDay = FinishDay;
        }

        var oAtts = [{ 'attname': 'rolt_tlid', 'value': document.getElementById('hdnMandatoryLayerID').value },
        { 'attname': 'rolt_begin', 'value': Begin },
        { 'attname': 'rolt_beginday', 'value': BeginDay },
        { 'attname': 'rolt_finish', 'value': Finish },
        { 'attname': 'rolt_finishday', 'value': FinishDay },
        { 'attname': 'rolt_allowModIni', 'value': AllowModifyIniHour },
        { 'attname': 'rolt_allowModDuration', 'value': AllowModifyDuration },
        { 'attname': 'rolt_datediffmin', 'value': 0 },
        { 'attname': 'rolt_parentid', 'value': document.getElementById('hdnMandatoryParentID').value },
        { 'attname': 'rolt_floatingbeginupto', 'value': FloatingBeginUpTo },
        { 'attname': 'rolt_floatingbeginuptoday', 'value': FloatingBeginUpToDay },
        { 'attname': 'rolt_floatingfinishminutes', 'value': FloatingFinishMinutes },
        { 'attname': 'rolt_unit_1020_id', 'value': document.getElementById('hdnMandatory1020LayerID').value },
        { 'attname': 'rolt_unit_1020_parentid', 'value': document.getElementById('hdnMandatory1020ParentID').value },
        { 'attname': 'rolt_unit_1021_id', 'value': document.getElementById('hdnMandatory1021LayerID').value },
        { 'attname': 'rolt_unit_1021_parentid', 'value': document.getElementById('hdnMandatory1021ParentID').value },
        { 'attname': 'rolt_unit_1022_id', 'value': document.getElementById('hdnMandatory1022LayerID').value },
        { 'attname': 'rolt_unit_1022_parentid', 'value': document.getElementById('hdnMandatory1022ParentID').value },
        { 'attname': 'rolt_unit_1020_value', 'value': Unit_1020_Value },
        { 'attname': 'rolt_unit_1020_action', 'value': Unit_1020_Action },
        { 'attname': 'rolt_unit_1020_begin', 'value': Unit_1020_Begin },
        { 'attname': 'rolt_unit_1020_beginday', 'value': Unit_1020_BeginDay },
        { 'attname': 'rolt_unit_1020_finish', 'value': Unit_1020_Finish },
        { 'attname': 'rolt_unit_1020_finishday', 'value': Unit_1020_FinishDay },
        { 'attname': 'rolt_unit_1021_value', 'value': Unit_1021_Value },
        { 'attname': 'rolt_unit_1021_action', 'value': Unit_1021_Action },
        { 'attname': 'rolt_unit_1021_begin', 'value': Unit_1021_Begin },
        { 'attname': 'rolt_unit_1021_beginday', 'value': Unit_1021_BeginDay },
        { 'attname': 'rolt_unit_1021_finish', 'value': Unit_1021_Finish },
        { 'attname': 'rolt_unit_1021_finishday', 'value': Unit_1021_FinishDay },
        { 'attname': 'rolt_unit_1022_value', 'value': Unit_1022_Value },
        { 'attname': 'rolt_unit_1022_action', 'value': Unit_1022_Action },
        { 'attname': 'rolt_unit_1022_begin', 'value': Unit_1022_Begin },
        { 'attname': 'rolt_unit_1022_beginday', 'value': Unit_1022_BeginDay },
        { 'attname': 'rolt_unit_1022_finish', 'value': Unit_1022_Finish },
        { 'attname': 'rolt_unit_1022_finishday', 'value': Unit_1022_FinishDay }
        ];

        return oAtts;
    } catch (e) {
        showError("Mandatory_FieldsToJSON", e);
        return null;
    }
}

function Mandatory_CheckTime(obj, permitBlank) {
    try {
        if (permitBlank == false) {
            if (obj.GetValue() == "") {
                showErrorPopup("Error.frmMandatory." + obj.id + "Title", "ERROR", "Error.frmMandatory." + obj.id + "Desc", "Error.frmMandatory.OK", "Error.frmMandatory.OKDesc", "");
                return true;
            }
        }
        return false;
    } catch (e) { showError("Mandatory_CheckTime", e); }
}