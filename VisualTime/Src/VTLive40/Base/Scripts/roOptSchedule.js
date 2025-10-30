/* roOptSchedule *************************/
/*****************************************************************************/
function optSchedule_changeTab(selTab, objPrefix) {
    try {

        var divDaily = document.getElementById(objPrefix + '_divDaily');
        var divWeekly = document.getElementById(objPrefix + '_divWeekly');
        var divMonthly = document.getElementById(objPrefix + '_divMonthly');
        var divAnnual = document.getElementById(objPrefix + '_divAnnual');

        divDaily.style.display = 'none';
        divWeekly.style.display = 'none';
        divMonthly.style.display = 'none';
        divAnnual.style.display = 'none';

        if (selTab == 0)
            divDaily.style.display = '';
        else if (selTab == 1)
            divWeekly.style.display = '';
        else if (selTab == 2)
            divMonthly.style.display = '';
        else if (selTab == 3)
            divAnnual.style.display = '';

    } catch (e) { showError("roOptSchedule:optSchedule_changeTab", e); }
}

function optSchedule_changeMonth(selDiv, objPrefix) {
    try {

        var cmbM101 = eval(objPrefix + '_cmbM101');
        var cmbM102 = eval(objPrefix + '_cmbM102');
        var cmbM201 = eval(objPrefix + '_cmbM201');
        var cmbM202 = eval(objPrefix + '_cmbM202');
        var cmbM203 = eval(objPrefix + '_cmbM203');
        if (selDiv == 1) {
            cmbM101.SetEnabled(true);
            cmbM102.SetEnabled(true);
            cmbM201.SetEnabled(false);
            cmbM202.SetEnabled(false);
            cmbM203.SetEnabled(false);

        } else {
            cmbM101.SetEnabled(false);
            cmbM102.SetEnabled(false);
            cmbM201.SetEnabled(true);
            cmbM202.SetEnabled(true);
            cmbM203.SetEnabled(true);
        }
    } catch (e) { showError("roOptSchedule:optSchedule_changeMonth", e); }

}

function optSchedule_changeAnual(selDiv, objPrefix) {
    try {

        var cmbA101 = eval(objPrefix + '_cmbA101');
        var cmbA102 = eval(objPrefix + '_cmbA102');
        if (selDiv == 1) {
            cmbA101.SetEnabled(true);
            cmbA102.SetEnabled(true);
        } else {
            cmbA101.SetEnabled(false);
            cmbA102.SetEnabled(false);
        }
    } catch (e) { showError("roOptSchedule:optSchedule_changeAnual", e); }

}

function optSchedule_enableChildElements(objId) {
    try {
        var theObject = document.getElementById(objId);
        var level = 0;
        optSchedule_TraverseDOM(theObject, level, optSchedule_enableElement);
    } catch (e) { showError("roOptSchedule:enableChildElements", e); }
}

function optSchedule_disableChildElements(objId) {
    try {
        var theObject = document.getElementById(objId);
        var level = 0;
        optSchedule_TraverseDOM(theObject, level, optSchedule_disableElement);
    } catch (e) { showError("roOptSchedule:disableChildElements", e); }
}

function optSchedule_TraverseDOM(obj, lvl, actionFunc) {
    try {
        for (var i = 0; i < obj.childNodes.length; i++) {
            var childObj = obj.childNodes[i];
            if (childObj.tagName) {
                actionFunc(childObj);
            }
            TraverseDOM(childObj, lvl + 1, actionFunc);
        }
    } catch (e) { showError("roOptSchedule:TraverseDOM", e); }
}

function optSchedule_enableElement(obj) {
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
    } catch (e) { showError("roOptSchedule:optSchedule_enableElement", e); }
    
}

function optSchedule_disableElement(obj) {
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
    } catch (e) { showError("roOptSchedule:optSchedule_disableElement", e); }
}

function optSchedule_validateField(objPrefix) {
    try {
        var strError = "";
        var optDiary = document.getElementById(objPrefix + '_optDiary');
        var optWeekly = document.getElementById(objPrefix + '_optWeekly');
        var optMonthly = document.getElementById(objPrefix + '_optMonthly');
        var optAnnual = document.getElementById(objPrefix + '_optAnnual');

        var optAnnualFix = document.getElementById(objPrefix + '_anualFixDay');
        var optAnnualLast = document.getElementById(objPrefix + '_anualLastDay');

        var txtDaily = document.getElementById(objPrefix + '_txtDaily');

        var cmbWeeklyTo = document.getElementById(objPrefix + '_cmbWeeklyTo_Value');
        
        var opMonth1 = document.getElementById(objPrefix + '_opMonth1');
        var cmbM1O1 = document.getElementById(objPrefix + '_cmbM1O1_Value');
        var cmbM1O2 = document.getElementById(objPrefix + '_cmbM1O2_Value');
        
        var opMonth2 = document.getElementById(objPrefix + '_opMonth2');
        var cmbM2O1 = document.getElementById(objPrefix + '_cmbM2O1_Value');
        var cmbM2O2 = document.getElementById(objPrefix + '_cmbM2O2_Value');
        var cmbM2O3 = document.getElementById(objPrefix + '_cmbM2O3_Value');

        var cmbA1O1 = document.getElementById(objPrefix + '_cmbA1O1_Value');
        var cmbA1O2 = document.getElementById(objPrefix + '_cmbA1O2_Value');

        if (optDiary.checked) {
            if (txtDaily.value == "") {strError = objPrefix + 'txtDaily';  return strError; }
        }

    
        if (optWeekly.checked) {
            if (cmbWeeklyTo.value == "") { strError = objPrefix + 'cmbWeeklyTo'; return strError; }        
        }

        if (optMonthly.checked) {
            if (opMonth1.checked) {
                if (cmbM1O1.value == "") { strError = objPrefix + 'cmbM1O1'; return strError; }
                if (cmbM1O2.value == "") { strError = objPrefix + 'cmbM1O2'; return strError; }        
            }
            if (opMonth2.checked) {
                if (cmbM2O1.value == "") { strError = objPrefix + 'cmbM2O1'; return strError; }
                if (cmbM2O2.value == "") { strError = objPrefix + 'cmbM2O2'; return strError; }
                if (cmbM2O3.value == "") { strError = objPrefix + 'cmbM2O3'; return strError; }        
            }
        }

        if (optAnnual.checked) {
            if (optAnnualFix.checked) { 
                if (cmbA1O1.value == "") { strError = objPrefix + 'cmbA1O1'; return strError; }
                if (cmbA1O2.value == "") { strError = objPrefix + 'cmbA1O2'; return strError; }
            }
        }
        
        return strError;
    } catch (e) { showError("roOptSchedule:optSchedule_validateField", e); }
}