/* roOptSchedule2 (Planificador de informes) *************************/
/*****************************************************************************/
function optSchedule2_setEnabled(bEnabled, objPrefix) {
    try {

        document.getElementById(objPrefix + '_optHours').disabled = !bEnabled;
        document.getElementById(objPrefix + '_optDiary').disabled = !bEnabled;
        document.getElementById(objPrefix + '_optWeekly').disabled = !bEnabled;
        document.getElementById(objPrefix + '_optMonthly').disabled = !bEnabled;
        document.getElementById(objPrefix + '_optOneTime').disabled = !bEnabled;

        document.getElementById(objPrefix + '_chkWeekDay1').disabled = !bEnabled;
        document.getElementById(objPrefix + '_chkWeekDay2').disabled = !bEnabled;
        document.getElementById(objPrefix + '_chkWeekDay3').disabled = !bEnabled;
        document.getElementById(objPrefix + '_chkWeekDay4').disabled = !bEnabled;
        document.getElementById(objPrefix + '_chkWeekDay5').disabled = !bEnabled;
        document.getElementById(objPrefix + '_chkWeekDay6').disabled = !bEnabled;
        document.getElementById(objPrefix + '_chkWeekDay7').disabled = !bEnabled;


        txtHoursScheduleClient.SetEnabled(bEnabled);
        cmbDaysClient.SetEnabled(bEnabled);
        cmbWeeklyToClient.SetEnabled(bEnabled);
        cmbM1O1Client.SetEnabled(bEnabled);
        cmbM2O1Client.SetEnabled(bEnabled);
        cmbM2O2Client.SetEnabled(bEnabled);
        txtDateScheduleClient.SetEnabled(bEnabled);
        txtHoursClient.SetEnabled(bEnabled);

    } catch (e) { showError("roOptSchedule2:optSchedule2_setEnabled", e); }
}

function optSchedule2_changeTab(selTab, objPrefix) {
    try {
        var divHours = document.getElementById(objPrefix + "_divHours");
        var divDaily = document.getElementById(objPrefix + '_divDaily');
        var divWeekly = document.getElementById(objPrefix + '_divWeekly');
        var divMonthly = document.getElementById(objPrefix + '_divMonthly');
        var divOneTime = document.getElementById(objPrefix + '_divOneTime');
        var divCommonHours = document.getElementById(objPrefix + '_divCommonHours');

        divHours.style.display = 'none';
        divDaily.style.display = 'none';
        divWeekly.style.display = 'none';
        divMonthly.style.display = 'none';
        divOneTime.style.display = 'none';
        divCommonHours.style.display = 'none';

        if (selTab == 0) {
            divHours.style.display = '';
            divCommonHours.style.display = 'none';
        } else if (selTab == 1) {
            divDaily.style.display = '';
            divCommonHours.style.display = '';
        } else if (selTab == 2) {
            divWeekly.style.display = '';
            divCommonHours.style.display = '';
        } else if (selTab == 3) {
            divMonthly.style.display = '';
            divCommonHours.style.display = '';
        } else if (selTab == 4) {
            divOneTime.style.display = '';
            divCommonHours.style.display = '';
        }
        /* INCOMPATIBLE CON IE. No permite atributo name en controles div
        var arrTabs = document.getElementsByName(objPrefix + '_nameOptSchedule2Tab');
        for (n = 0; n < arrTabs.length; n++) {
            var div = arrTabs[n];
            if (n == selTab) {
                div.style.display = '';
            } else {
                div.style.display = 'none';
            }
        }
        */
        hasChanges(true, false);
        
    } catch (e) { showError("roOptSchedule2:optSchedule2_changeTab", e); }
}



function optSchedule2_changeMonth(selDiv, objPrefix) {
    try {
    if(selDiv == 1){
        optSchedule2_enableChildElements(objPrefix + '_divOpM1');
        optSchedule2_disableChildElements(objPrefix + '_divOpM2');
    } else {
        optSchedule2_enableChildElements(objPrefix + '_divOpM2');
        optSchedule2_disableChildElements(objPrefix + '_divOpM1');
    }

    hasChanges(true, false);

    } catch (e) { showError("roOptSchedule2:optSchedule2_changeMonth", e); }

}

function optSchedule2_enableChildElements(objId) {
    try {
        var theObject = document.getElementById(objId);
        var level = 0;
        optSchedule2_TraverseDOM(theObject, level, optSchedule2_enableElement);
    } catch (e) { showError("roOptSchedule2:enableChildElements", e); }
}

function optSchedule2_disableChildElements(objId) {
    try {
        var theObject = document.getElementById(objId);
        var level = 0;
        optSchedule2_TraverseDOM(theObject, level, optSchedule2_disableElement);
    } catch (e) { showError("roOptSchedule2:disableChildElements", e); }
}

function optSchedule2_TraverseDOM(obj, lvl, actionFunc) {
    try {
        for (var i = 0; i < obj.childNodes.length; i++) {
            var childObj = obj.childNodes[i];
            if (childObj.tagName) {
                actionFunc(childObj);
            }
            TraverseDOM(childObj, lvl + 1, actionFunc);
        }
    } catch (e) { showError("roOptSchedule2:TraverseDOM", e); }
}

function optSchedule2_enableElement(obj) {
    try {
        if (obj.disabled == false) { return; }
        if (obj.tagName == "A") {

            if (window.addEventListener) { // Mozilla, etc.
                if (obj.getAttribute("ononclick") == null) {
                    obj.setAttribute("ononclick", obj.getAttribute("onclick"));
                }
                if (obj.getAttribute("onhref") == null) {
                    obj.setAttribute("onhref", obj.getAttribute("href"));
                }
                obj.setAttribute("onclick", obj.getAttribute("ononclick"));
            }
            else { //IE
                if (obj.getAttribute("ononclick") == null) {
                    obj.setAttribute("ononclick", obj.getAttribute("onclick"));
                }
                if (obj.getAttribute("onhref") == null) {
                    obj.setAttribute("onhref", obj.getAttribute("href"));
                }
                obj.setAttribute("onclick", obj.getAttribute("ononclick"));
            }
        }
        obj.disabled = false;

        if (obj.getAttribute('ConvertControl') != null && obj.getAttribute('ConvertControl') != '') {
            if (obj.getAttribute("Converted") == "true") {
                var oField = Ext.getCmp(obj.getAttribute("ConvertedId"));
                if (oField != null) {
                    oField.enable();
                    oField.validate();
                }
            }
        }
    } catch (e) { showError("roOptSchedule2:optSchedule2_enableElement", e); }
    
}

function optSchedule2_disableElement(obj) {
    try {
        if (obj.disabled == true) { return; }
        if (obj.tagName == "A") {

            if (obj.getAttribute("onclick") == null) {
                obj.setAttribute("ononclick", "");
            }
            else {
                obj.setAttribute("ononclick", obj.getAttribute("onclick"));
            }

            if (obj.getAttribute("href") == null) {
                obj.setAttribute("onhref", "javascript: void(0);");
            }
            else {
                obj.setAttribute("onhref", obj.getAttribute("href"));
            }

            if (window.addEventListener) { // Mozilla, etc.
                obj.setAttribute("onclick", "");
            }
            else { //IE
                $(obj).unbind('click');
            }
        }
        obj.disabled = true;

        if (obj.getAttribute('ConvertControl') != null && obj.getAttribute('ConvertControl') != '') {
            if (obj.getAttribute("Converted") == "true") {
                var oField = Ext.getCmp(obj.getAttribute("ConvertedId"));
                if (oField != null) {
                    oField.disable();
                    oField.clearInvalid();
                }
            }
        }
    } catch (e) { showError("roOptSchedule2:optSchedule2_disableElement", e); }
}

function optSchedule2_validateField(objPrefix) {
    try {
        var strError = "";
        var optHours = document.getElementById(objPrefix + '_optHours');
        var optDiary = document.getElementById(objPrefix + '_optDiary');
        var optWeekly = document.getElementById(objPrefix + '_optWeekly');
        var optMonthly = document.getElementById(objPrefix + '_optMonthly');
        var optOneTime = document.getElementById(objPrefix + '_optOneTime');

        //Diari
        var cmbDays = document.getElementById(objPrefix + '_cmbDays_Value');

        //Semanal
        var cmbWeeklyTo = document.getElementById(objPrefix + '_cmbWeeklyTo_Value');

        var chkDay1 = document.getElementById(objPrefix + '_chkWeekDay1');
        var chkDay2 = document.getElementById(objPrefix + '_chkWeekDay2');
        var chkDay3 = document.getElementById(objPrefix + '_chkWeekDay3');
        var chkDay4 = document.getElementById(objPrefix + '_chkWeekDay4');
        var chkDay5 = document.getElementById(objPrefix + '_chkWeekDay5');
        var chkDay6 = document.getElementById(objPrefix + '_chkWeekDay6');
        var chkDay7 = document.getElementById(objPrefix + '_chkWeekDay7');
        
        var opMonth1 = document.getElementById(objPrefix + '_opMonth1');
        var cmbM1O1 = document.getElementById(objPrefix + '_cmbM1O1_Value');
        
        var opMonth2 = document.getElementById(objPrefix + '_opMonth2');
        var cmbM2O1 = document.getElementById(objPrefix + '_cmbM2O1_Value');
        var cmbM2O2 = document.getElementById(objPrefix + '_cmbM2O2_Value');

        var txtDateSchedule = document.getElementById(objPrefix + '_txtDateSchedule');
        
        var txtHours = document.getElementById(objPrefix + '_txtHours');
        var txtHoursSchedule = document.getElementById(objPrefix + '_txtHoursSchedule');
        
        
        if (optDiary.checked) {
            if (cmbDays.value == "") { strError = objPrefix + 'cmbDays'; return strError; }
        }

        if (optHours.checked) {
            if (txtHoursSchedule.value == "") { strError = objPrefix + 'txtHoursSchedule'; return strError; }
        }

    
        if (optWeekly.checked) {
            if (cmbWeeklyTo.value == "") { strError = objPrefix + 'cmbWeeklyTo'; return strError; }
            if (!chkDay1.checked && !chkDay2.checked && !chkDay3.checked && !chkDay4.checked && !chkDay5.checked && !chkDay6.checked && !chkDay7.checked) {
                strError = objPrefix + 'chkDays';
                return strError; 
            }
        }

        if (optMonthly.checked) {
            if (opMonth1.checked) {
                if (cmbM1O1.value == "") { strError = objPrefix + 'cmbM1O1'; return strError; }
            }
            if (opMonth2.checked) {
                if (cmbM2O1.value == "") { strError = objPrefix + 'cmbM2O1'; return strError; }
                if (cmbM2O2.value == "") { strError = objPrefix + 'cmbM2O2'; return strError; }
            }
            if (!opMonth1.checked && !opMonth2.checked) { strError = objPrefix + 'opMonths'; return strError; }
        }

        if (optOneTime.checked) {
            if (txtDateSchedule.value == "") { strError = objPrefix + 'txtDateSchedule'; return strError; }
            if (txtDateSchedule.getAttribute("Converted") == "true") {

                var oField = Ext.getCmp(txtDateSchedule.getAttribute("ConvertedId"));
                if (oField != null) {
                    oField.validate();
                    if (oField.isValid(false) == false) {
                        strError = objPrefix + 'txtDateSchedule';
                        return strError;
                    } 
                }

            }
        }

        if (txtHours.value == "") { strError = objPrefix + 'txtHours'; return strError; }
        if (txtHours.getAttribute("Converted") == "true") {

            var oField = Ext.getCmp(txtHours.getAttribute("ConvertedId"));
            if (oField != null) {
                oField.validate();
                if (oField.isValid(false) == false) {
                    strError = objPrefix + 'txtHours';
                    return strError;
                }
            }

        }
        
        
        return strError;
    } catch (e) { showError("roOptSchedule2:optSchedule2_validateField", e); }
}