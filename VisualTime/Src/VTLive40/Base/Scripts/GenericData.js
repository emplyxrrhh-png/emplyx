var VT_DEBUG = "1";

//*****************************************************************************************/
// loadGenericData
// Carrega els camps
//********************************************************************************************/
function loadGenericData(fieldName, controls, value, typeControl, list, disabled, disHasChanges, fnHasChanges) {
    try {
        if (controls != null) {
            if (controls.endsWith(",")) { controls = controls.substring(0, controls.length - 1); }
        }
        switch (typeControl) {
            case "X_TEXT":
                if (value == null) value = "";
                loadValue(controls, value, typeControl, disabled, disHasChanges, fnHasChanges);
                break;
            case "X_NUMBER":
                if (value == "") value = "0";
                loadValue(controls, value, typeControl, disabled, disHasChanges, fnHasChanges);
                break;
            case "X_DATE":
                loadValue(controls, value, typeControl, disabled, disHasChanges, fnHasChanges);
                break;
            case "X_TIME":
                loadValue(controls, value, typeControl, disabled, disHasChanges, fnHasChanges);
                break;
            case "X_RADIO":
                loadValue(controls, value, typeControl, disabled, disHasChanges, fnHasChanges);
                break;
            case "X_HIDDEN":
                loadValue(controls, value, typeControl, disabled, disHasChanges, fnHasChanges);
                break;
            case "X_COMBOBOX":
                loadComboBox(controls, value, list, typeControl, disabled, disHasChanges, fnHasChanges);
                checkOnChangeCombox(controls, disHasChanges, fnHasChanges);
                break;
            case "X_CHECKBOX":
                loadValue(controls, value, typeControl, disabled, disHasChanges, fnHasChanges);
                break;
            case "X_OPTIONGROUP":
                loadOptionPanel(controls, value, typeControl, disabled, disHasChanges, fnHasChanges);
                break;
            case "X_OPTIONCHECK":
                loadOptionCheck(controls, value, typeControl, disabled, disHasChanges, fnHasChanges);
                break;
            case "X_DATEPICKER":
                loadDatePicker(controls, value, typeControl, disabled, disHasChanges, fnHasChanges);
                break;
            case "X_TIME":
                loadValue(controls, value, typeControl, disabled, disHasChanges, fnHasChanges);
                break;
            // Control de estats (sols estats, NO CAMBIA VALORS) ------------------------------------------------- 
            case "X_TEXT_STATE":
                //loadValue(controls, value, typeControl, disabled, disHasChanges, fnHasChanges);
                break;
            case "X_NUMBER_STATE":
                //loadValue(controls, value, typeControl, disabled, disHasChanges, fnHasChanges);
                break;
            case "X_DATE_STATE":
                //loadValue(controls, value, typeControl, disabled, disHasChanges, fnHasChanges);
                break;
            case "X_RADIO_STATE":
                //loadValue(controls, value, typeControl, disabled, disHasChanges, fnHasChanges);
                break;
            case "X_COMBOBOX_STATE":
                //loadComboBox(controls, value, list, typeControl, disabled, disHasChanges, fnHasChanges);
                //checkOnChangeCombox(controls, disHasChanges, fnHasChanges);
                break;
            case "X_CHECKBOX_STATE":
                //loadValue(controls, value, typeControl, disabled, disHasChanges, fnHasChanges);
                break;
            case "X_OPTIONGROUP_STATE":
                //Sols es passa un control del grup
                loadOptionPanelState(controls, value, typeControl, disabled, disHasChanges, fnHasChanges);
                break;
            case "X_OPTIONCHECK_STATE":
                //loadOptionPanelState(controls, value, typeControl, disabled, disHasChanges, fnHasChanges);
                break;
            case "X_DATEPICKER_STATE":
                //loadDatePicker(controls, value, typeControl, disabled, disHasChanges, fnHasChanges);
                break;
            case "X_TIME_STATE":
                //loadValue(controls, value, typeControl, disabled, disHasChanges, fnHasChanges);
                break;
        } //end switch
    } catch (e) { showError('GenericData:loadGenericData', e); }
} //end loadGenericData function

//*****************************************************************************************/
// loadValue
// Carrega el valor(s) als controls corresponents
//********************************************************************************************/
function loadValue(controls, value, typeControl, disabled, disHasChanges, fnHasChanges) {
    try {
        if (controls == "") { return; }

        var n;
        var objs = controls.toString().split(",");

        for (n = 0; n < objs.length; n++) {
            var objId = objs[n].replace(/^\s*|\s*$/g, ""); //equivalent a funcio trim()
            var obj = document.getElementById(objId);
            if (obj != null) {
                switch (obj.tagName) {
                    case "SPAN":
                        obj.innerHTML = value;
                        break;
                    case "DIV":
                        obj.innerHTML = value;
                        break;
                    case "TEXTAREA":
                        obj.value = value;
                        obj.innerHTML = value;
                        setDisable(obj, disabled);
                        checkOnChange(obj, disHasChanges, fnHasChanges);
                        checkCControl(obj, "X_TEXTAREA") //typeControl);
                        //obj.focus();
                        break;
                    case "INPUT":
                        var Type = obj.getAttribute("type");
                        if (Type != null) {
                            switch (Type.toUpperCase()) {
                                case "TEXTAREA":
                                    obj.innerHTML = value;
                                    setDisable(obj, disabled);
                                    checkOnChange(obj, disHasChanges, fnHasChanges);
                                    checkCControl(obj, "X_TEXTAREA") //typeControl);
                                    break;
                                case "TEXT":
                                    obj.value = value;
                                    setDisable(obj, disabled);
                                    checkOnChange(obj, disHasChanges, fnHasChanges);
                                    checkCControl(obj, typeControl);
                                    break;
                                case "CHECKBOX":
                                    if (value.toUpperCase() == "TRUE") {
                                        obj.checked = true;
                                    } else {
                                        obj.checked = false;
                                    }
                                    setDisable(obj, disabled);
                                    checkOnChange(obj, disHasChanges, fnHasChanges);
                                    checkCControl(obj, typeControl);
                                    break;
                                case "RADIO":
                                    if (value.toUpperCase() == "TRUE") {
                                        obj.checked = true;
                                    } else {
                                        obj.checked = false;
                                    }
                                    setDisable(obj, disabled);
                                    checkOnChange(obj, disHasChanges, fnHasChanges);
                                    checkCControl(obj, typeControl);
                                    break;
                                case "HIDDEN":
                                    obj.value = value;
                                    break;
                            } //end switch
                        }     // end if                            
                }
            } //end if
        } //end for
    } catch (e) { showError("GenericData:loadValue", e); }
} //end loadValue function


//*****************************************************************************************/
// loadState
// Carrega el estat (actiu / inactiu)
//********************************************************************************************/
function loadState(controls, value, typeControl, disabled, disHasChanges, fnHasChanges) {
    try {
        if (controls == "") { return; }

        var n;
        var objs = controls.toString().split(",");

        for (n = 0; n < objs.length; n++) {
            var objId = objs[n].replace(/^\s*|\s*$/g, ""); //equivalent a funcio trim()
            var obj = document.getElementById(objId);
            if (obj != null) {
                switch (obj.tagName) {
                    case "TEXTAREA":
                        setDisable(obj, disabled);
                        break;
                    case "INPUT":
                        var Type = obj.getAttribute("type");
                        if (Type != null) {
                            switch (Type.toUpperCase()) {
                                case "TEXTAREA":
                                    setDisable(obj, disabled);
                                    break;
                                case "TEXT":
                                    setDisable(obj, disabled);
                                    break;
                                case "CHECKBOX":
                                    setDisable(obj, disabled);
                                    break;
                                case "RADIO":
                                    setDisable(obj, disabled);
                                    break;
                            } //end switch
                        }     // end if                            
                }
            } //end if
        } //end for
    } catch (e) { showError("GenericData:loadValue", e); }
} //end loadValue function


//*****************************************************************************************/
// checkCControl
// Comproba que el camp estigui convertit al seu corresponent ext, si cal, converteix
//********************************************************************************************/
function checkCControl(obj, typeControl) {
    try {
            switch (typeControl) {
                case "X_HIDDEN":
                    break;
                case "X_TEXT":
                    TextFieldApply(obj);
                    break;
                case "X_TEXTAREA":
                    TextAreaApply(obj);
                    break;
                case "X_NUMBER":
                    NumberFieldApply(obj);
                    break;
                case "X_DATE":
                    DatePickerApply(obj);
                    break;
                case "X_TIME":
                    TimeFieldApply(obj);
                    break;
                case "X_HIDDEN":
                    break;
                case "X_RADIO":
                    break;
                case "X_COMBOBOX":
                    break;
                case "X_CHECKBOX":
                    break;
                case "X_OPTIONGROUP":
                    break;
                case "X_OPTIONCHECK":
                    break;
            } //end switch
        } catch (e) { showError("GenericData:checkCControl", e); }
} //end function checkCControl

//*****************************************************************************************/
// checkOnChange
// Afegeix al event onchange del control "hasChanges(true);"
// Sols controls que acceptin onchange (no anchors)
//********************************************************************************************/
function checkOnChange(obj, disHasChanges, fnHasChanges) {
    try {
        if (disHasChanges != null) {
            if (disHasChanges == "false" || disHasChanges == false) { return; }
        }
        
        var strHasChanges = "hasChanges(true);";    
        if (fnHasChanges != null) { strHasChanges = fnHasChanges; }

        if (obj != null) {
            if (window.addEventListener) { // Mozilla, Netscape, Firefox
                AddScript(obj.id, "onchange", strHasChanges);
            }
            else { // IE
                AddScriptIE(obj.id, "onchange", strHasChanges);
            }
        }
        else {
            //obj null
        }
    }
    catch (e) { showError("GenericData:checkOnChange", e); }
}

function AddScriptIE(objID, att, evtscript, firstScript) {
    try {
        var obj = document.getElementById(objID);
        if (obj != null) {
            var oAttStr = obj.getAttribute(att);
            if (oAttStr != null) {
                oAttStr = oAttStr.replace(evtscript + ";", ""); //Eliminem si ja existeix el script
                oAttStr = oAttStr.replace(evtscript, ""); //Eliminem si ja existeix el script
                if (oAttStr.startsWith(";")) { oAttStr = oAttStr.substring(1, oAttStr.length); }
                if (oAttStr.endsWith(";")) { oAttStr = oAttStr.substring(0, oAttStr.length - 1); }
                if (oAttStr != "") {
                    if (firstScript == null || firstScript == false) {
                        obj.setAttribute(att, oAttStr + ";" + evtscript);
                    } else {
                        obj.setAttribute(att, oAttStr + ";" + evtscript);
                    }
                } else {
                    obj.setAttribute(att, evtscript);
                }
            } else {
                obj.setAttribute(att, evtscript);
            }
        }
    } catch (e) { showError("GenericData:AddScriptIE", e); }
}

//*****************************************************************************************/
// checkOnChangeOPanel
// Funcio equivalent a checkOnChange pero per OptionalPanels lligats "X_OPTIONGROUP"
//********************************************************************************************/
function checkOnChangeOPanel(objId, vmode, disabled, disHasChanges, fnHasChanges) {
    try {
        if (objId == "" || objId == null) { return; }
        if (disHasChanges != null) {
            if (disHasChanges == "false" || disHasChanges == false) { return; }
        }

        var strHasChanges = "hasChanges(true);";
        if (fnHasChanges != null) { strHasChanges = fnHasChanges; }    
    
        var objButton;
        var aTitle;
        var aDescription;
        switch (vmode) {
            case "0": //radio
                objButton = document.getElementById(objId + '_rButton');
                break;
            case "1": //checkbox
                objButton = document.getElementById(objId + '_chkButton');
                break;
        } //end switch

        aTitle = document.getElementById(objId + '_aTitle');
        aDescription = document.getElementById(objId + '_aDescription');

        // objButton--------------------------------------------
        var onChangeStr = '';
        if (objButton != null) {
            if (window.addEventListener) { // Mozilla, Netscape, Firefox -----------------------------------------
                if (objButton.getAttribute("disabled") == "false" || objButton.getAttribute("disabled") == null) {
                    AddScript(objButton.id, "onclick", strHasChanges);
                    //AddScript(objButton.id, "onchangeClick", strHasChanges);
                } // end if
            } 
            else { // IE ----------------------------------------------------------------------------------
                if (objButton.getAttribute("disabled") == false || objButton.getAttribute("disabled") == "false" || objButton.getAttribute("disabled") == null) {

                    on_change = new Function(strHasChanges);
                    objButton.attachEvent('onclick', on_change);
                    //objButton.attachEvent('onchangeClick', on_change);
                } // end if
            } //end if addeventlistener
           }  //end if obj = nul


           // aTitle -----------------------------------------
           var onChangeStr = '';
           if (aTitle != null) {
               if (window.addEventListener) { // Mozilla, Netscape, Firefox -----------------------------------------
                   if (aTitle.getAttribute("disabled") == "false" || aTitle.getAttribute("disabled") == null) {
                       //AddScript(aTitle.id, "onclick", strHasChanges);
                       AddScript(aTitle.id, "onclick", strHasChanges);
                   } // end if
               } 
               else { // IE ----------------------------------------------------------------------------------
                   if (aTitle.getAttribute("disabled") == false || aTitle.getAttribute("disabled") == "false" || aTitle.getAttribute("disabled") == null) {
                   
                       on_changeclick = new Function(strHasChanges);
                       aTitle.attachEvent('onclick', on_changeclick);
                       //aTitle.attachEvent('onchangeClick', on_changeclick);
                   } // end if 
               } //end if addeventlistener
           }  //end if obj = null

           // aDescription -----------------------------------------
           var onChangeStr = '';
           if (aDescription != null) {
               if (window.addEventListener) { // Mozilla, Netscape, Firefox -----------------------------------------
                   if (aDescription.getAttribute("disabled") == "false" || aDescription.getAttribute("disabled") == null) {
                       //AddScript(aDescription.id, "onclick", strHasChanges);
                       AddScript(aDescription.id, "onclick", strHasChanges);
                   } // end if
               } 
               else { // IE ----------------------------------------------------------------------------------
                   if (aDescription.getAttribute("disabled") == false || aDescription.getAttribute("disabled") == "false" || aDescription.getAttribute("disabled") == null) {

                       on_changeclick = new Function(strHasChanges);
                       aDescription.attachEvent('onclick', on_changeclick);
                       //aDescription.attachEvent('onchangeClick', on_changeclick);

                   } // end if 
               } //end if addeventlistener
           }  //end if obj = null

       } catch (e) { showError('GenericData:checkOnChangeOPanel', e); }
}


//*****************************************************************************************/
// loadOptionPanel
// Carrega del OptionPanel lligats (X_OPTIONGROUP)
//********************************************************************************************/
function loadOptionPanel(controls, value, typeControl, disabled, disHasChanges, fnHasChanges) {
    try {
        if (controls == "") { return; }

        var n;
        var objs = controls.toString().split(",");
        linkOPCItems(controls);
        
        
        for (n = 0; n < objs.length; n++) {
            var objId = objs[n].replace(/^\s*|\s*$/g, ""); //equivalent a funcio trim()
            var objContainer = document.getElementById(objId + '_panOptionPanel');
            var objButton;
            if (objContainer != null) {
                var vmode = objContainer.getAttribute("vmode");
                if (vmode == null) { return; }
                if (vmode == "") { return; }
                //Tipus de OptionPanel (radio, checkbox)
                switch (objContainer.getAttribute("vmode")) {
                    case "0": //radio
                        objButton = document.getElementById(objId + '_rButton');
                        break;
                    case "1": //checkbox
                        objButton = document.getElementById(objId + '_chkButton');
                        break;
                } //end switch

                if (objButton != null) {
                    if (value == n) {
                        objButton.checked = true;
                    } else {
                        objButton.checked = false;
                    }
                    venableOPC(objId);
                } //end if
            } //end if
            if (disabled != null) {
                if (disabled == "true") {
                    if (objContainer != null) {
                        objContainer.setAttribute("disabled", "true");
                        objContainer.setAttribute("venabled", "False");
                        venableOPC(objId);
                    }
                } else {
                    checkOnChangeOPanel(objId, vmode, disabled, disHasChanges, fnHasChanges);
                }
            } else {
                checkOnChangeOPanel(objId, vmode, disabled, disHasChanges, fnHasChanges);
            }
        } //end for

    } catch (e) { showError('GenericData:loadOptionPanel', e); }
}

//*****************************************************************************************/
// loadOptionCheck
// Carrega del OptionPanel Checkbox (X_OPTIONCHECK)
//********************************************************************************************/
function loadOptionCheck(controls, value, typeControl, disabled, disHasChanges, fnHasChanges) {
    try {
        if (controls == "") { return; }

        var n;
        var objs = controls.toString().split(",");

        for (n = 0; n < objs.length; n++) {
            var objId = objs[n].replace(/^\s*|\s*$/g, ""); //equivalent a funcio trim()
            var objContainer = document.getElementById(objId + '_panOptionPanel');
            var objButton;
            if (objContainer != null) {
                var vmode = objContainer.getAttribute("vmode");
                if (vmode == null) { return; }
                if (vmode == "") { return; }
                //Tipus de OptionPanel (radio, checkbox)
                switch (objContainer.getAttribute("vmode")) {
                    case "0": //radio
                        objButton = document.getElementById(objId + '_rButton');
                        break;
                    case "1": //checkbox
                        objButton = document.getElementById(objId + '_chkButton');
                        break;
                } //end switch

                if (objButton != null) {
                    if (value.toUpperCase() == "TRUE") {
                        objButton.checked = true;
                    } else {
                        objButton.checked = false;
                    }
                    venableOPC(objId);
                } //end if
            } //end if


            if (disabled != null) {
                if (disabled == "true") {
                        setDisable(objContainer, "true");
                        objContainer.setAttribute("venabled", "False");
                        venableOPC(objId);
                } else {
                    checkOnChangeOPCheck(objId, disabled, disHasChanges, fnHasChanges);
                    setDisable(objContainer, "false");
                    objContainer.setAttribute("venabled", "True");
                    venableOPC(objId);
                }
            } else {
                    checkOnChangeOPCheck(objId, disabled, disHasChanges, fnHasChanges);
                    setDisable(objContainer, "false");
                    objContainer.setAttribute("venabled", "True");
                    venableOPC(objId);
            }

            
        } //end for

    } catch (e) { showError('GenericData:loadOptionPanel', e); }
}

//*****************************************************************************************/
// loadOptionCheckState
// Carrega del OptionPanel Checkbox (X_OPTIONCHECK_STATE) 
//********************************************************************************************/
function loadOptionPanelState(controls, value, typeControl, disabled, disHasChanges, fnHasChanges) {
    try {
        if (controls == "") { return; }
        //alert(controls + ':' + disabled);
        var n;
        var objs = controls.toString().split(",");
        //linkOPCItems(controls);

        for (n = 0; n < objs.length; n++) {
            var objId = objs[n].replace(/^\s*|\s*$/g, ""); //equivalent a funcio trim()
            var objContainer = document.getElementById(objId + '_panOptionPanel');
            var objButton;
            if (objContainer != null) {
                var vmode = objContainer.getAttribute("vmode");
                if (vmode == null) { return; }
                if (vmode == "") { return; }
                //Tipus de OptionPanel (radio, checkbox)
                switch (objContainer.getAttribute("vmode")) {
                    case "0": //radio
                        objButton = document.getElementById(objId + '_rButton');
                        break;
                    case "1": //checkbox
                        objButton = document.getElementById(objId + '_chkButton');
                        break;
                } //end switch

            } //end if
            if (disabled != null) {
                if (disabled == "true") {
                        objContainer.setAttribute("disabled", "true");
                        objContainer.setAttribute("venabled", "False");
                        venableOPC(objId);
                } else {
                        objContainer.setAttribute("disabled", "false");
                        objContainer.setAttribute("venabled", "True");
                        venableOPC(objId);
                        checkOnChangeOPanel(objId, vmode, disabled, disHasChanges, fnHasChanges);
                }
            } else {
                objContainer.setAttribute("disabled", "false");
                objContainer.setAttribute("venabled", "True");
                venableOPC(objId);
                checkOnChangeOPanel(objId, vmode, disabled, disHasChanges, fnHasChanges);
            }
        } //end for

    } catch (e) { showError('GenericData:loadOptionPanelState', e); }
}

function loadDatePicker(controls, value, typeControl) {
    try {
        if (controls == "") { return; }

        var n;
        var objs = controls.toString().split(",");

        for (n = 0; n < objs.length; n++) {
            var objId = objs[n].replace(/^\s*|\s*$/g, ""); //equivalent a funcio trim()
            var objDatePicker = document.getElementById(objId);
            if (objDatePicker != null) {
                objDatePicker.style.backgroundColor = value;
            } //end if
        } //end for

    } catch (e) { showError('GenericData:loadDatePicker', e); }
}

//*****************************************************************************************/
// checkOnChangeOPCheck
// Funcio equivalent a checkOnChange per OptionPanel Checkbox (X_OPTIONCHECK)
//********************************************************************************************/
function checkOnChangeOPCheck(objId, disabled, disHasChanges, fnHasChanges) {
    try {
        if (disHasChanges != null) {
            if (disHasChanges == "false" || disHasChanges == false) { return; }
        }

        var strHasChanges = "hasChanges(true);";
        if (fnHasChanges != null) { strHasChanges = fnHasChanges; }    

        var objContainer = document.getElementById(objId + '_panOptionPanel');
        var objButton;

        if (objContainer != null) {
            var vmode = objContainer.getAttribute("vmode");
            if (vmode == null) { return; }
            if (vmode == "") { return; }
            
            //Tipus de OptionPanel (radio, checkbox)
            switch (objContainer.getAttribute("vmode")) {
                case "0": //radio
                    objButton = document.getElementById(objId + '_rButton');
                    break;
                case "1": //checkbox
                    objButton = document.getElementById(objId + '_chkButton');
                    break;
            } //end switch
            
            aTitle = document.getElementById(objId + '_aTitle');
            aDescription = document.getElementById(objId + '_aDescription');

            if (objButton != null) {
                if (objButton.getAttribute("disabled") != "true") {

                    if (window.addEventListener) { // Mozilla, Netscape, Firefox
                    
                        AddScript(objButton.id, "onclick", strHasChanges);
                        AddScript(objButton.id, "onchangeClick", strHasChanges);

                    }
                    else { // IE

                        on_change = new Function(strHasChanges);
                        objButton.attachEvent('onclick', on_change);
                        objButton.attachEvent('onchangeClick', on_change);

                   } //end if

                 } //end if
            } //end if


            if (aTitle != null) {
                if (aTitle.getAttribute("disabled") != "true") {
                        var onChangeStr;

                        if (window.addEventListener) { // Mozilla, Netscape, Firefox
                            /*onChangeStr = aTitle.getAttribute("onclick");
                            onChangeStr = onChangeStr.replace(strHasChanges, "");

                            if (aTitle.getAttribute("onchangeClick") == null || aTitle.getAttribute("onchangeClick") == "") {
                                aTitle.setAttribute("onchangeClick", strHasChanges + onChangeStr);
                            }

                            aTitle.setAttribute("onclick", strHasChanges + onChangeStr); */
                        } else { // IE
                        } //end if
                } //end if 
            } //end if 


            if (aDescription != null) {
                if (aDescription.getAttribute("disabled") != "true") {
                        var onChangeStr;

                        if (window.addEventListener) { // Mozilla, Netscape, Firefox
                            /*onChangeStr = aDescription.getAttribute("onclick");
                            onChangeStr = onChangeStr.replace(strHasChanges, "");

                            if (aDescription.getAttribute("onchangeClick") == null || aDescription.getAttribute("onchangeClick") == "") {
                                aDescription.setAttribute("onchangeClick", strHasChanges + onChangeStr);
                            }

                            aDescription.setAttribute("onclick", strHasChanges + onChangeStr);*/
                        } else { // IE
                        } //end if

                } // end if
            } // end if 

        } //end if

    } catch (e) { showError('GenericData:checkOnChangeOPCheck', e); }
}

function deleteFunctionAnon(anonStr) {
    try {
        var retStr = '';
        //retStr = anonStr.replace('function anonymous()\n{', '');
        if (anonStr.indexOf('function anonymous()\n{') != -1) {
            retStr = anonStr.replace('function anonymous()\n{', '');
            retStr = retStr.replace('\n}', '');
            retStr = retStr.replace('\n', '');
        }
        if (anonStr.indexOf('function() {') != -1) {
            retStr = anonStr.replace('function() {', '');
            retStr = retStr.substring(0, retStr.length - 1);
            retStr = retStr.replace('\n', '');
        }
        return retStr;
    } catch (e) {
    showError('GenericData:deleteFunctionAnon', e);
        return ""; 
    }

    
}

//*****************************************************************************************/
// loadComboBox
// Carrega el valor al ComboBox.
// Si list no ve buit, recarregará els items (No implementat encara...)
//********************************************************************************************/
function loadComboBox(controls, value, list, typeControl, disabled, disHasChanges) {
    try {
        if (controls == "") { return; }

        var oCombo, oComboId, oCombo_Text, oCombo_Value;
        var objs = controls.toString().split(",");
        oComboId = objs[0].replace(/^\s*|\s*$/g, "");
        oCombo = objs[0].replace(/^\s*|\s*$/g, "") + "_ComboBoxLabel";
        if (objs.length == 3) {
            oCombo_Text = objs[1].replace(/^\s*|\s*$/g, "");
            oCombo_Value = objs[2].replace(/^\s*|\s*$/g, "");
        } else {
            oCombo_Text = "";
            oCombo_Value = "";
        }

        //Si es passa llista de valors, buidem i carreguem el roComboBox
        if (list != "") {
            //borrem els camps del roCombobox
            roCB_clearItems(oComboId, oCombo_Text, oCombo_Value)
            
            var cbItems = list.split("|*|");
            var nItems;
            //Recuperem els items
            for (nItems = 0; nItems < cbItems.length; nItems++) {
                var cbParms = cbItems[nItems].split("~*~");
                //Recuperem els parametres
                if (cbParms.length == 3) { //Texte, Valor, funcio JS
                    roCB_addItem(oComboId, cbParms[0], cbParms[1], cbParms[2]);
                } else if (cbParms.length == 2) { //Texte, valor
                    roCB_addItem(oComboId, cbParms[0], cbParms[1], '');
                } else if (cbParms.length == 1) { //Texte
                    roCB_addItem(oComboId, cbParms[0], cbParms[0], '');
                } //end if 
            } //end for 
        } //end if


        if (disabled != null) {
            if (disabled == "true" || disabled == true) {

            
                var objComboID = document.getElementById(oComboId);
                if (objComboID != null) {
                    roCB_disable(oComboId, true);  //ppr IE
                    objComboID.setAttribute("disabled", disabled.toString());
                }
                var objCombo = document.getElementById(oCombo);
                if (objCombo != null) {
                    objCombo.setAttribute("disabled", disabled.toString());
                }
            }
            else if (disabled == "false" || disabled == false) {
                var objComboID = document.getElementById(oComboId);
                if (objComboID != null) {
                    roCB_disable(oComboId, false);  //ppr IE
                    if (objComboID.attributes.getNamedItem("disabled") != null) {
                        objComboID.attributes.removeNamedItem("disabled");
                    }
                    
                }
                var objCombo = document.getElementById(oCombo);
                if (objCombo != null) {
                    if (objCombo.attributes.getNamedItem("disabled") != null) {
                        objCombo.attributes.removeNamedItem("disabled");
                    }
                }
            }
        }

        //Posicionem el rocombobox per valor
        roCB_setValue(value, oCombo, oCombo_Text, oCombo_Value);


    } catch (e) { showError('GenericData:loadComboBox', e); }
}


function checkOnChangeCombox(controls, disHasChanges, fnHasChanges) {
    try {
        if (disHasChanges != null) {
            if (disHasChanges == "false" || disHasChanges == false) { return; }
        }

        var strHasChanges = "hasChanges(true);";
        if (fnHasChanges != null) { strHasChanges = fnHasChanges; }    

        if (controls == "") { return; }

        var oCombo, oComboId;
        var objs = controls.toString().split(",");
        oComboId = objs[0].replace(/^\s*|\s*$/g, "") + "_ComboBoxLabel";
        oCombo = document.getElementById(oComboId);
        if (oCombo != null) {
            var strJSChange = oCombo.getAttribute("cbonchange");
            if (strJSChange != null && strJSChange != "") {
                strJSChange = strJSChange.replace(strHasChanges, "");
                oCombo.setAttribute("cbonchange", strJSChange + ";" + strHasChanges);
            } else {
                oCombo.setAttribute("cbonchange", strHasChanges);
            }
        }

    } catch (e) { showError('GenericData:checkOnChangeCombox', e); }
}



//*****************************************************************************************/
// validateGenericData
// Validem els camps
//********************************************************************************************/
function validateGenericData(fieldName, controls, value, typeControl, list) {
    try {
        switch (typeControl) {
            case "X_TEXT":
                return validateText(controls);
                break;
            case "X_NUMBER":
                return validateText(controls);
                break;
            case "X_DATE":
                return validateText(controls);
                break;
            case "X_TIME":
                return validateText(controls);
                break;
        } //end switch
        return null;
    } catch (e) { showError('GenericData:validateGenericData', e); }
} //end loadGenericData function

//*****************************************************************************************/
// createGenericDataParam
// Creació de parametres per pasar per parametres
//********************************************************************************************/
function createGenericDataParam(fieldName, controls, value, typeControl) {
    try {
        var strParam = '';
        switch (typeControl) {
            case "X_TEXT":
                strParam = createTextParam(fieldName, controls, value, typeControl);
                break;
            case "X_NUMBER":
                strParam = createTextParam(fieldName, controls, value, typeControl);
                break;
            case "X_DATE":
                strParam = createTextParam(fieldName, controls, value, typeControl);
                break;
            case "X_TIME":
                strParam = createTextParam(fieldName, controls, value, typeControl);
                break;
            case "X_HIDDEN":
                strParam = createTextParam(fieldName, controls, value, typeControl);
                break;
            case "X_COMBOBOX":
                strParam = createComboBoxParam(fieldName, controls, value, typeControl);
                break;
            case "X_RADIO":
                strParam = createTextParam(fieldName, controls, value, typeControl);
                break;
            case "X_CHECKBOX":
                strParam = createTextParam(fieldName, controls, value, typeControl);
                break;
            case "X_OPTIONGROUP":
                strParam = createOptionGroupParam(fieldName, controls, value, typeControl);
                break;
            case "X_OPTIONCHECK":
                strParam = createOptionCheckParam(fieldName, controls, value, typeControl);
                break;
        } //end switch
        return strParam;
    } catch (e) {
    showError('GenericData:createGenericDataParam', e);
        return "";
     }
}

//*****************************************************************************************/
// createOptionGroupParam
// Creació de parametres per OptionPanelGroup (agrupats)
//********************************************************************************************/
function createOptionGroupParam(fieldName, controls, value, typeControl) {
    try {
        if (controls == "") { return; }

        var strParam = fieldName + '=';
        var strValue = '';

        var n;
        var objs = controls.toString().split(",");

        for (n = 0; n < objs.length; n++) {
            var objId = objs[n].replace(/^\s*|\s*$/g, ""); //equivalent a funcio trim()
            var objContainer = document.getElementById(objId + '_panOptionPanel');
            var objButton;
            if (objContainer != null) {
                var vmode = objContainer.getAttribute("vmode");
                if (vmode == null) { return; }
                if (vmode == "") { return; }
                //Tipus de OptionPanel (radio, checkbox)
                switch (objContainer.getAttribute("vmode")) {
                    case "0": //radio
                        objButton = document.getElementById(objId + '_rButton');
                        break;
                    case "1": //checkbox
                        objButton = document.getElementById(objId + '_chkButton');
                        break;
                } //end switch

                if (objButton != null) {
                    if (objButton.checked == true) {
                        if (objContainer.getAttribute("value") != "") {
                            strValue = objContainer.getAttribute("value");
                        } else {
                            strValue = n;
                        } //end if
                    } // end if
                } //end if 
            } //end if
        } //end for

        return strParam + encodeURIComponent(strValue);
        
    } catch (e) {
    showError('GenericData:createOptionGroupParam', e);
        return ""; 
    }
}

//*****************************************************************************************/
// createOptionCheckParam
// Creació de parametres per OptionPanelGroup (checkbox independent)
//********************************************************************************************/
function createOptionCheckParam(fieldName, controls, value, typeControl) {
    try {
        if (controls == "") { return; }

        var strParam = fieldName + '=';
        var strValue = '';

        var n;
        var objs = controls.toString().split(",");

        
        var objId = objs[0].replace(/^\s*|\s*$/g, ""); //equivalent a funcio trim()
        var obj = document.getElementById(objId + '_panOptionPanel');

        if (obj != null) {
            var vmode = obj.getAttribute("vmode");
            if (vmode == null) { return; }
            if (vmode == "") { return; }
            //Tipus de OptionPanel (radio, checkbox)
            switch (obj.getAttribute("vmode")) {
                case "0": //radio
                    objButton = document.getElementById(objId + '_rButton');
                    break;
                case "1": //checkbox
                    objButton = document.getElementById(objId + '_chkButton');
                    break;
            } //end switch

            if (objButton != null) {
                if (objButton.checked == true) {
                    strValue = 'TRUE';
                } else {
                    strValue = 'FALSE';
                }
            } //end if     
        } //end if

        return strParam + encodeURIComponent(strValue);    
    } catch(e) {
    showError('GenericData:createOptionCheckParam', e);
        return "";
    }
}

//*****************************************************************************************/
// createComboBoxParam
// Creació de parametres per combobox
//********************************************************************************************/
function createComboBoxParam(fieldName, controls, value, typeControl) {
    try {
    if (controls == "") { return; }

        var strParam = fieldName + '=';
        var strValue = '';
        
        var n;
        var objs = controls.toString().split(",");

            var objId = objs[0].replace(/^\s*|\s*$/g, ""); //equivalent a funcio trim()
            var obj = document.getElementById(objId + '_ComboBoxLabel');
            if (obj != null) {
                strValue = obj.getAttribute("value");
            }
            return strParam + encodeURIComponent(strValue);
    } catch (e) {
    showError('GenericData:createComboBoxParam', e);
        return "";
    }
}


//*****************************************************************************************/
// createTextParam
// Creació de parametres per input text, datepicker, number, etc.
//********************************************************************************************/
function createTextParam(fieldName, controls, value, typeControl) {
    try {
        if (controls == "") { return; }
        
        var strParam = fieldName + '=';
        var strValue = '';
        
        var n;
        var objs = controls.toString().split(",");

            var objId = objs[0].replace(/^\s*|\s*$/g, ""); //equivalent a funcio trim()
            var obj = document.getElementById(objId);
            
            if (obj != null) {
                switch (obj.tagName) {
                    case "SPAN":
                        strValue = obj.innerHTML;
                        break;
                    case "DIV":
                        strValue = obj.innerHTML;
                        break;
                    case "TEXTAREA":
                        strValue = obj.innerHTML;
                        strValue = obj.value;
                        break;
                    case "INPUT":
                        var Type = obj.getAttribute("type");
                        if (Type != null) {
                            switch (Type.toUpperCase()) {
                                case "TEXTAREA":
                                    strValue = obj.innerHTML;
                                    strValue = obj.value;
                                    break;
                                case "TEXT":
                                    strValue = obj.value;
                                    break;
                                case "CHECKBOX":
                                    if (obj.checked == true) {
                                        strValue = "TRUE";
                                    } else {
                                        strValue = "FALSE";
                                    }
                                    break;
                                case "RADIO":
                                    if (obj.checked == true) {
                                        strValue = "TRUE";
                                    } else {
                                        strValue = "FALSE";
                                    }
                                    break;
                                case "HIDDEN":
                                    strValue = obj.value;
                                    break;
                            } //end switch
                        } // end if                            
                } //end switch
            } //end if
            
            return strParam + encodeURIComponent(strValue);
        } catch (e) { showError('GenericData:createTextParam', e); }
}

//*****************************************************************************************/
// validateText
// Validem els camps Textes, numerics i datepickers
//********************************************************************************************/
function validateText(controls) {
    try {
        if (controls == "") { return; }

        var n;
        var objs = controls.toString().split(",");

        for (n = 0; n < objs.length; n++) {
            var objError = { 'error': 'false', 'tab': '', 'tabContainer': '', 'id': '', 'msg': '', 'typemsg':'0' };
            var objId = objs[n].replace(/^\s*|\s*$/g, ""); //equivalent a funcio trim()
            objError.id = objId;
            var obj = document.getElementById(objId);
            if (obj != null) {
                if (obj.getAttribute("Converted") == "true") {
                    var oField = Ext.getCmp(obj.getAttribute("ConvertedId"));
                    if (oField != null) {
                            oField.validate();
                            if (oField.isValid(false) == false) {
                                objError.error = 'true';
                                objError.tab = retrieveTab(objId); //busquem el tab
                                objError.tabContainer = retrieveTabContainer(objId); //busquem en tabcontainers
                                objError.id = objId;
                                objError.msg = '';
                                objError.typemsg = '0'; //0 - Inline, 1- Popup
                                return objError;
                            }  //end if 
                    } //end if 
                 } //end if 
            } //end if 
        } //end for

    } catch (e) { showError("GenericData:ValidateText", e); }
} //end function validateText

//*****************************************************************************************/
// retrieveTab
// Busqueda del tab principal
//********************************************************************************************/
function retrieveTab(objId) {
    try {
        var n, oDiv;
        var divs;

        switch (BrowserDetect.browser) {
            case 'Firefox':
            case 'Safari':
            case 'Chrome':
                divs = document.getElementsByName("menuPanel");            
                break;
            case 'Explorer':
                divs = getElementsByName_iefix("DIV", "menuPanel");        
                break;
            default:
                divs = document.getElementsByName("menuPanel");
                if (divs.length == 0) {
                    divs = getElementsByName_iefix("DIV", "menuPanel");
                }
                break;
        }
        
        for (n = 0; n < divs.length; n++) {
            oDiv = divs[n];
            if (findField(oDiv, objId) == true) {
                return oDiv.id;
            } //end if 
        } //end for
    } catch (e) { showError('GenericData:retrieveTab', e); }
}

function findField(obj, objIdFind) {
    for (var i = 0; i < obj.childNodes.length; i++) {
        var childObj = obj.childNodes[i];
        if (childObj.id == objIdFind) {
            return true;
        } //end if
        if (childObj.hasChildNodes()) {
            if (findField(childObj, objIdFind) == true) {
                return true;
            }
        } //end if
    } //end for
    return false;
}

function getElementsByName_iefix(tag, name) {

    var elem = document.getElementsByTagName(tag);
    var arr = new Array();
    for (i = 0, iarr = 0; i < elem.length; i++) {
        att = elem[i].getAttribute("name");
        if (att == name) {
            arr[iarr] = elem[i];
            iarr++;
        }
    }
    return arr;
}

//*****************************************************************************************/
// retrieveTabContainer
// Busqueda de tabs containers
//********************************************************************************************/
function retrieveTabContainer(objId) {
    try {
        var n, oDiv;
        var divs = document.getElementsByName("tabContainer");
        for (n = 0; n < divs.length; n++) {
            oDiv = divs[n];
            if (findField(oDiv, objId) == true) {
                return oDiv.id;
            } //end if 
        } //end for
    } catch (e) { showError('GenericData:retrieveTabContainer', e); }

}

//*****************************************************************************************/
// positionTabContainer
// Posicionar el tabcontainer actiu
//********************************************************************************************/
function positionTabContainer(oTabContainer) {
    //FALTA PROGRAMAR...
}

//*****************************************************************************************/
// positionTab
// Posicionar al div dels tab principals
//********************************************************************************************/
function positionTab(tab) {
    try {
        changeTabsByName(tab);
    } catch (e) { showError('GenericData:positionTab', e); }
}

//*****************************************************************************************/
// retJSONValue
// Retorna un valor d'un array JSON, pasant el id (field)
//********************************************************************************************/
function retJSONValue(arrFields, fieldName) {
    try {
        var n;
        for (n = 0; n < arrFields.length; n++) {
            if (arrFields[n].field.toUpperCase() == fieldName.toUpperCase()) {
                return arrFields[n].value;
            }
        } //end for

    } catch (e) { showError("GenericData:retJSONValue", e); }
}

//*****************************************************************************************/
// showError
// Controla els avisos d'error segons la variable debug
//********************************************************************************************/
function showError(trace, e) {
    if (window.location.href.indexOf("localhost") >= 0) {
        switch (BrowserDetect.browser) {
            case 'Firefox':
            case 'Safari':
            case 'Chrome':
                alert(trace + ': ' + e);
                break;
            case 'Explorer':
                alert(trace + ': ' + e.description);
                break;
            default:
                alert(trace + ': ' + e);            
                break;
        }
    }
}

//*****************************************************************************************/
// retElementsByName
// Recupera elements per nom (funcio generica)
//********************************************************************************************/
function retElementsByName(objName,oTag) {
    try {
        var colA;
        
        switch (BrowserDetect.browser) {
            case 'Firefox':
            case 'Safari':
            case 'Chrome':
                colA = document.getElementsByName(objName);
                break;
            case 'Explorer':
                if (oTag != null) {
                    colA = getElementsByName_iefix(oTag, objName);
                } else {
                    colA = getElementsByName_iefix("A", objName);
                    if (colA.length == 0) {
                        colA = getElementsByName_iefix("DIV", objName);
                    }
                }
                break;
            default:
                colA = document.getElementsByName(objName);
                if (colA.length == 0) {
                    colA = getElementsByName_iefix("A", objName);
                }
                break;
        }

        return colA;
    } catch (e) { showError("GenericData:retElementsByName", e); }

}

function setDisable(obj, disabled) {
    try {
        if (disabled != null) {
            if (disabled == "true" || disabled == true) {
                obj.setAttribute("disabled", "true");
            } else {
                obj.removeAttribute("disabled");
            }
        } else {
            obj.removeAttribute("disabled");
        }
    }
    catch (e) {
        showError("GenericData:setDisable", e); 
    }
}

function CheckLinkChange(objID) {
    try {
        var chk = document.getElementById(objID);
        if (chk.disabled) { return; }
        if (chk != null) { chk.checked = !chk.checked; }        
        if (chk.getAttribute("onchange") != "") {
            eval(chk.getAttribute("onchange"));
        }
    } catch (e) { showError("GenericData:CheckLinkChange", e); }
}

function CheckLinkClick(objID) {
    try {
        
        var chk = document.getElementById(objID);
        if (chk.disabled) { return; }
 
        if (window.addEventListener) { // Mozilla, Netscape, Firefox

            if (chk != null) { chk.checked = !chk.checked; }

            var evtScript = "";
            if (chk.getAttribute("onclick") != null) {
                if (chk.getAttribute("onclick") != "") {
                    evtScript = chk.getAttribute("onclick");
                }
            }
            if (chk.getAttribute("onchange") != null) {
                if (chk.getAttribute("onchange") != "") {
                    evtScript += ";" + chk.getAttribute("onchange");
                }
            }
            
            if (evtScript != "") {
                evtScript = evtScript.replace("this", "document.getElementById('" + objID + "')");
                eval(evtScript);
            }
        }
        else { // IE
            if (chk.type == "checkbox") {
                if (chk != null) { chk.checked = !chk.checked; }
                $(chk).click();
                if (chk != null) { chk.checked = !chk.checked; }
                $(chk).change();
            }
            else {
                if (chk != null) { chk.checked = !chk.checked; }
                $(chk).click();
                $(chk).change();
            }
        }
        
    }
    catch (e) {
        showError("GenericData:CheckLinkClick", e); 
    }
}

/*** FUNCION ORIGINAL ANTES DE PROVAR COMPATIBILIDAD CON EXPLORER
function CheckLinkClick(objID) {
    try {
        var chk = document.getElementById(objID);
        if (chk.disabled) { return; }
        if (chk != null) { chk.checked = !chk.checked; }
        var evtScript = "";
        if (chk.getAttribute("onclick") != null) {
            if (chk.getAttribute("onclick") != "") {
                evtScript = chk.getAttribute("onclick");
            }
        }
        if (chk.getAttribute("onchange") != null) {
            if (chk.getAttribute("onchange") != "") {
                evtScript += ";" + chk.getAttribute("onchange");
            }
        }
        
        if (evtScript != "") {
            evtScript = evtScript.replace("this", "document.getElementById('" + objID + "')");
            eval(evtScript);
        }
    } catch (e) { showError("GenericData:CheckLinkClick", e); }
}
***/


function CheckRadioClick(objID) {
    try {
        var chk = document.getElementById(objID);
        if (chk.disabled) { return; }
        if (chk != null) {
            if (chk.checked) { return; } //Si ja esta marcat, no fem res
            chk.checked = !chk.checked; 
        }
        var evtScript = "";
        if (chk.getAttribute("onclick") != null) {
            if (chk.getAttribute("onclick") != "") {
                evtScript = chk.getAttribute("onclick");
            }
        }
        if (chk.getAttribute("onchange") != null) {
            if (chk.getAttribute("onchange") != "") {
                evtScript += ";" + chk.getAttribute("onchange");
            }
        }

        if (evtScript != "") {
            evtScript = evtScript.replace("this", "document.getElementById('" + objID + "')");
            eval(evtScript);
        }
    } catch (e) { showError("GenericData:CheckRadioClick", e); }
}


// Afegir un script a un atribut
function AddScript(objID, att, evtscript, firstScript) {
    try {
        var obj = document.getElementById(objID);
        if (obj != null) {
            var oAttStr = obj.getAttribute(att);
            if (oAttStr != null) {
                oAttStr = oAttStr.replace(evtscript + ";", ""); //Eliminem si ja existeix el script
                oAttStr = oAttStr.replace(evtscript, ""); //Eliminem si ja existeix el script
                if (oAttStr.startsWith(";")) { oAttStr = oAttStr.substring(1, oAttStr.length); }
                if (oAttStr.endsWith(";")) { oAttStr = oAttStr.substring(0, oAttStr.length - 1); }
                if (oAttStr != "") {
                    if (firstScript == null || firstScript == false) {
                        obj.setAttribute(att, oAttStr + ";" + evtscript);
                    } else {
                        obj.setAttribute(att, oAttStr + ";" + evtscript);
                    }
                } else {
                    obj.setAttribute(att, evtscript);
                }
            } else {
                obj.setAttribute(att, evtscript);
            }
        }
    } catch (e) { showError("GenericData:AddScript", e); }
}

//Funcio que recupera objectes input amb name parcial (modificats per runat=server amb $)
function getElementsByPartialName(partialName, objContainer) {
    try {
        var retVal = new Array();
        var elems;
        if (objContainer != null) {
            elems = objContainer.getElementsByTagName("input");
        } else {
            elems = document.getElementsByTagName("input");
        }

        for (var i = 0; i < elems.length; i++) {
            var nameProp = elems[i].getAttribute('name');
            if (nameProp != null) {
                if (nameProp.startsWith(partialName)) {
                    retVal.push(elems[i]);
                }
            }
        }

        return retVal;
    }
    catch (e) {
        showError("GenericData:getElementByPartialName", e); return null; 
    }
}

function findObjByClassName(obj, classNameFind) {
    for (var i = 0; i < obj.childNodes.length; i++) {
        var childObj = obj.childNodes[i];
        if (childObj.className == classNameFind) {
            return childObj;
        } //end if
        if (childObj.hasChildNodes()) {
            objRet = findObjByClassName(childObj, classNameFind)
            if (objRet != null) {
                return objRet;
            }
        } //end if
    } //end for
    return null;
}