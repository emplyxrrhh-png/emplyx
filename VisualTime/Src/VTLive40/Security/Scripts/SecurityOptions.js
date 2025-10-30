var jsGridIPs; //Grid
var oSecurityOptions = null;//new SecurityOptionsData(0);

function showLoadingGrid(loading) { parent.showLoader(loading); }

function hasChanges(bolChanges, markRecalc) {
    var divTop = document.getElementById('divMsgTop');
    var divBottom = document.getElementById('divMsgBottom');

    var tagHasChanges = document.getElementById('msgHasChanges');
    var msgChanges = '<changes>';
    if (tagHasChanges != null) {
        msgChanges = tagHasChanges.value;
    }

    setStyleMessage('divMsg2');
    setMessage(msgChanges);

    if (bolChanges) {
        divTop.style.display = '';
        divBottom.style.display = '';
    } else {
        divTop.style.display = 'none';
        divBottom.style.display = 'none';
    }
}

function setMessage(msg) {
    try {
        var msgTop = document.getElementById('msgTop');
        var msgBottom = document.getElementById('msgBottom');
        msgTop.textContent = msg;
        msgBottom.textContent = msg;
    } catch (e) { alert('setMessage: ' + e); }
}

function setStyleMessage(classMsg) {
    try {
        var divTop = document.getElementById('divMsgTop');
        var divBottom = document.getElementById('divMsgBottom');

        divTop.className = classMsg;
        divBottom.className = classMsg;
    } catch (e) { alert('setStyleMessage: ' + e); }
}

function saveChanges() {
    try {
        if (oSecurityOptions != null && oSecurityOptions.validateSecurityOptions()) {
            oSecurityOptions.saveSecurityOptions(oSecurityOptions.getSecurityOptionsID(), null);//jsGridIPs);
        }
    } catch (e) { showError("saveChanges", e); }
}

function undoChanges() {
    try {
        oSecurityOptions = new SecurityOptionsData(0);
    } catch (e) { showError("undoChanges", e); }
}

function retrieveError(objError) {
    try {
        setStyleMessage('divMsg-Error');
        var tagHasErrors = document.getElementById('msgHasErrors');
        var msgErrors = '<errors>';
        if (tagHasErrors != null) {
            msgErrors = tagHasErrors.value;
        }
        setMessage(msgErrors);

        if (objError != null) {
            if (objError.tabContainer != undefined) {
                positionTabContainer(objError.tabContainer);
            }

            if (objError.tab != undefined) {
                positionTab(objError.tab);
            }

            if (objError.id != undefined) {
                try {
                    document.getElementById(objError.id).focus();
                } catch (ex) { }
            }
        }
    } catch (e) { showError('retrieveError', e); }
}

function SetTrackbarValue(s, e) {
    var txtPriority = document.getElementById("ctl00_contentMainBody_groupPassword_txtPasswordSecurity");
    txtPriority.value = s.GetPosition();
}