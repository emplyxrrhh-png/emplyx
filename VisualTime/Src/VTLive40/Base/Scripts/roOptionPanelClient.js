function enableChildElements(objId) {
    try {
        var theObject = document.getElementById(objId);
        var level = 0;
        TraverseDOM(theObject, level, enableElement);
    } catch (e) { showError("roOptionPanelClient:enableChildElements", e); }
}

function disableChildElements(objId) {
    try {
        var theObject = document.getElementById(objId);
        var level = 0;
        TraverseDOM(theObject, level, disableElement);
    } catch (e) { showError("roOptionPanelClient:disableChildElements", e); }
}

function TraverseDOM(obj, lvl, actionFunc) {
    try {
        for (var i = 0; i < obj.childNodes.length; i++) {
            var childObj = obj.childNodes[i];
            if (childObj.tagName) {
                actionFunc(childObj);
            }
            TraverseDOM(childObj, lvl + 1, actionFunc);
        }
    }
    catch (e) {
        showError("roOptionPanelClient:TraverseDOM", e); 
    }
}

function enableElement(obj) {
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
    } catch (e) { showError("roOptionPanelClient:enableElement", e); }
    
}

function disableElement(obj) {
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
    } catch (e) { showError("roOptionPanelClient:disableElement", e); }
}

function checkOPCControls(oButton, objPrefix) {
    try {
        var objBtn = document.getElementById(oButton);
        var tbl = document.getElementById(objPrefix + '_tblContainerClient');

        if (objBtn.disabled == false) {
            if (objBtn.checked == true) {
                objBtn.checked = false;
                tbl.className = getStyleTable(objPrefix); //'OPTable';
                clickOPC(objBtn, objPrefix);
            } else {
                objBtn.checked = true;
                tbl.className = getStyleTable(objPrefix) + '-hover'; //'OPTable-hover';
                clickOPC(objBtn, objPrefix);
            }
        }
    } catch (e) { showError("roOptionPanelClient:checkOPCControls", e); }
}

function clickOPC(objChk, objPrefix) {
    try {
        if (objChk.disabled == false) {
            if (objChk.checked == true) {
                enableChildElements(objPrefix + '_panContainer');
                setStyleOPC(objPrefix, true);
                firesOPEvent(objPrefix);
            } else {
                disableChildElements(objPrefix + '_panContainer');
                setStyleOPC(objPrefix, false);
                // Miramos el tipo de Optionpanel. Si es tipo chk, lanzamos evento click.
                var objTbl = document.getElementById(objPrefix + "_panOptionPanel");
                if (objTbl == null) return;
                if (objTbl.getAttribute('vmode') == '1') {
                    firesOPEvent(objPrefix);
                }
            }
        }
                
    } catch (e) { showError("roOptionPanelClient:clickOPC", e); }
}

function venableOPC(objPrefix) {
    try {
        var objTbl = document.getElementById(objPrefix + "_panOptionPanel");
        if (objTbl == null) { return; }
        var objChk;
        if (objTbl.getAttribute("venabled").toString().toUpperCase() == "TRUE") {
            enableChildElements(objPrefix + "_panOptionPanel")
        } else {
            disableChildElements(objPrefix + "_panOptionPanel")
        }
        if (objTbl.getAttribute("vmode") == "0") {
            objChk = document.getElementById(objPrefix + "_rButton");
        } else {
            objChk = document.getElementById(objPrefix + "_chkButton");
        }
        clickOPC(objChk, objPrefix);
    } catch (e) { showError("roOptionPanelClient:venableOPC", e); }
}

function setStyleOPC(objPrefix, checked) {
    try {
        var objTbl = document.getElementById(objPrefix + "_tblContainerClient");
        if (checked == true) {
            //if (window.addEventListener) { // Mozilla, etc.
            //    objTbl.setAttribute("onmouseover", "");
            //    objTbl.setAttribute("onmouseout", "");
            //} 
            //else { //IE
            //    objTbl.onmouseover = function() { return; };
            //    objTbl.onmouseout = function() { return; };
            //}
            objTbl.className = getStyleTable(objPrefix) + "-hover"; //"OPTable-hover";
        } 
        else {
            //if (window.addEventListener) { // Mozilla, etc.
            //    objTbl.setAttribute("onmouseover", "this.className='" + getStyleTable(objPrefix) + "-hover';");
            //    objTbl.setAttribute("onmouseout", "this.className='" + getStyleTable(objPrefix) + "';");
            //}
            //else { //IE
            //    objTbl.onmouseover = function() { this.className = getStyleTable(objPrefix) + '-hover'; };
            //    objTbl.onmouseout = function() { this.className = getStyleTable(objPrefix); };
            //}
            objTbl.className = getStyleTable(objPrefix); //"OPTable";
        }
    } catch (e) { showError("roOptionPanelClient:setStyleOPC", e); }
}

function firesOPEvent(objPrefix) {
    try {
        var objTbl = document.getElementById(objPrefix + "_panOptionPanel");
        if (objTbl == null) { return; }
        var objChk;
        if (objTbl.getAttribute("vmode") == "0") {
            objChk = document.getElementById(objPrefix + "_rButton");
        } else {
            objChk = document.getElementById(objPrefix + "_chkButton");
        }
        if (objTbl.getAttribute("vclientscript") != "") {
            eval(objTbl.getAttribute("vclientscript"));
            
        }
        if (objChk.getAttribute('cconChange') != null && objChk.getAttribute('cconChange') != ''){
            eval(objChk.getAttribute('cconChange'));
        }
        
    }
    catch (e) {
        showError("roOptionPanelClient:firesOPEvent", e); 
    }
}

function getStyleTable(objPrefix) {
    try {
        var objTbl = document.getElementById(objPrefix + "_tblContainerClient");
        if (objTbl == null) { return; }
        var objChk;
        if (objTbl.getAttribute("vclass") != "") {
            return objTbl.getAttribute("vclass");
        }
    } catch (e) { showError("getStyleTable", e); }
}