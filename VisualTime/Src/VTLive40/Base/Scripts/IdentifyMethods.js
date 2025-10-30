var controlPrefix = "";

function IdentifyUserNameOnChange(Prefix) {
    var txtName = document.getElementById(Prefix + '_chkUsername_txtUserName');
    var txtPassword = document.getElementById(Prefix + '_chkUsername_txtPassword');
    var trPass = document.getElementById(Prefix + '_chkUsername_trPassword');
    var trValidateByAD = document.getElementById(Prefix + '_chkUsername_trValidateByAD');
    
    if (txtName != null && txtName.value.indexOf("\\") != -1) {
        if(txtPassword != null){
            txtPassword.disabled = true;
            txtPassword.value = "-";
        }
        if(trPass != null) trPass.style.display = 'none';
        if(trValidateByAD != null) trValidateByAD.style.display = '';
    }
    else {
        if(txtPassword != null){
            txtPassword.disabled = false;
            txtPassword.value = "xxxxxxxx";
        }
        if(trPass != null) trPass.style.display = 'none';
        if(trValidateByAD != null) trValidateByAD.style.display = 'none';
    }
    hasIdentifyChanges();
}

function checkMustActivateBlock(Prefix){
    if(Prefix != null && Prefix != ""){
        controlPrefix = Prefix;
    }
    var mustCheck = document.getElementById(controlPrefix + '_chkUsername_hdnMustActivateApplicationAccess');
    var chkUsername = document.getElementById(controlPrefix + '_chkUsername_chkRestictApplicationAccess');
    if(mustCheck != null && mustCheck.value == "0"){
        chkUsername.checked = false;
        chkUsername.disabled = "disabled";
    }
}

function hasIdentifyChanges(){
    try{
        hasChanges(true);
    }catch(e){
    }
}

function IdentifyRestorePwd() {
    try{
        var url = "Security/srvMsgBoxSecurity.aspx?action=Message&TitleKey=ResetPwd.Confirm.Text&" +
                      "DescriptionKey=ResetPwd.Confirm.Description&" +
                      "Option1TextKey=ResetPwd.Confirm.Option1Text&" +
                      "Option1DescriptionKey=ResetPwd.Confirm.Option1Description&" +
                      "Option2TextKey=ResetPwd.Confirm.Option2Text&" +
                      "Option2DescriptionKey=ResetPwd.Confirm.Option2Description&" +
            "Option1OnClickScript=parent.showLoader(true);IdentifyRestorePwdConfirmation(); return false;&" +            
                      "Option2OnClickScript=HideMsgBoxForm(); return false;&" +
                      "IconUrl=~/Base/Images/MessageFrame/dialog-question.png";
        if (parent.ShowMsgBoxForm !== undefined) {
            parent.ShowMsgBoxForm(url, 400, 300, '');
        } else {
            parent.parent.ShowMsgBoxForm(url, 400, 300, '');
        }
        
    } catch (e) {
        console.error(e);
    }
}

function resetCegidIDUser() {
    try {
        var url = "Security/srvMsgBoxSecurity.aspx?action=Message&TitleKey=CegidIdUsr.Confirm.Text&" +
            "DescriptionKey=CegidIdUsr.Confirm.Description&" +
            "Option1TextKey=CegidIdUsr.Confirm.Option1Text&" +
            "Option1DescriptionKey=CegidIdUsr.Confirm.Option1Description&" +
            "Option2TextKey=CegidIdUsr.Confirm.Option2Text&" +
            "Option2DescriptionKey=CegidIdUsr.Confirm.Option2Description&" +
            "Option1OnClickScript=parent.showLoader(true);resetCegidIDUserConfirmation(); return false;&" +
            "Option2OnClickScript=HideMsgBoxForm(); return false;&" +
            "IconUrl=~/Base/Images/MessageFrame/dialog-question.png";
        if (parent.ShowMsgBoxForm !== undefined) {
            parent.ShowMsgBoxForm(url, 400, 300, '');
        } else {
            parent.parent.ShowMsgBoxForm(url, 400, 300, '');
        }

    } catch (e) {
        console.error(e);
    }
} 

function IdentifySendUsername() {
    try {
        var url = "Security/srvMsgBoxSecurity.aspx?action=Message&TitleKey=SendUsername.Confirm.Text&" +
            "DescriptionKey=SendUsername.Confirm.Description&" +
            "Option1TextKey=SendUsername.Confirm.Option1Text&" +
            "Option1DescriptionKey=SendUsername.Confirm.Option1Description&" +
            "Option2TextKey=SendUsername.Confirm.Option2Text&" +
            "Option2DescriptionKey=SendUsernamePwd.Confirm.Option2Description&" +            
            "Option1OnClickScript=parent.showLoader(true);IdentifySendUsernameConfirmation(); return false;&" +
            "Option2OnClickScript=HideMsgBoxForm(); return false;&" +
            "IconUrl=~/Base/Images/MessageFrame/dialog-question.png";
        if (parent.ShowMsgBoxForm !== undefined) {
            parent.ShowMsgBoxForm(url, 400, 300, '');
        } else {
            parent.parent.ShowMsgBoxForm(url, 400, 300, '');
        }

    } catch (e) {
        console.error(e);
    }
}

//======================================================================

function SetFunctionality(Prefix) {
    try {

        // Prefix = 'cnIdentifyMethods'

        var chkUsernameChecked = false;
        var chkUsername = document.getElementById(Prefix + '_chkUsername_chkButton');
        if (chkUsername != null) chkUsernameChecked = chkUsername.checked;
        if (chkUsernameChecked == true) {
            IdentifyUserNameOnChange(Prefix);            
        }
        
        var chkCardChecked = false;
        var chkNFCChecked = false;

        var chkBiometricChecked = false;

        var chkPinChecked = false;

        var chkPlateChecked = false;
        
        var chkCard = document.getElementById(Prefix + '_chkCard_chkButton');
        if (chkCard != null) chkCardChecked = chkCard.checked;

        var chkNFC = document.getElementById(Prefix + '_chkNFC_chkButton');
        if (chkNFC != null) chkNFCChecked = chkNFC.checked;
        
        var chkBiometric = document.getElementById(Prefix + '_chkBiometric_chkButton');
        if (chkBiometric != null) chkBiometricChecked = chkBiometric.checked;

        /*
        if (chkBiometricChecked) {
            document.getElementById(Prefix + '_chkBiometric_tbDeleteHuella').setAttribute("class", "PathWay");
            document.getElementById(Prefix + '_chkBiometric_btDeleteBiometric').setAttribute("class", "btnCancelMode");
            document.getElementById(Prefix + '_chkBiometric_btDeleteBiometric').disabled = false;
        }
        else {
            document.getElementById(Prefix + '_chkBiometric_tbDeleteHuella').setAttribute("class", "");
            document.getElementById(Prefix + '_chkBiometric_btDeleteBiometric').setAttribute("class", "");
            document.getElementById(Prefix + '_chkBiometric_btDeleteBiometric').disabled = true;
        }
        */
        
        var chkPin = document.getElementById(Prefix + '_chkPin_chkButton');
        if (chkPin != null) chkPinChecked = chkPin.checked;

        var chkPlate = document.getElementById(Prefix + '_chkPlate_chkButton');
        if (chkPlate != null) chkPlateChecked = chkPlate.checked;

        var pepe = document.getElementById(Prefix + '_divFunctionality');
        if (pepe == null) return
        
        if ((chkCardChecked && chkBiometricChecked) || (chkCardChecked && chkPinChecked)) {
            roCB_disable(Prefix + '_cmbFunctionality', false);
            if (pepe != null)
                document.getElementById(Prefix + '_divFunctionality').style.display = '';
        }
        else {
            roCB_disable(Prefix + '_cmbFunctionality', true);
            if (pepe != null)
                document.getElementById(Prefix + '_divFunctionality').style.display = 'none';
        }


        
        var ItemsEnabled = new Array(true, true, true, true);

        ItemsEnabled[1] = (chkCardChecked && chkBiometricChecked);
        ItemsEnabled[2] = (chkCardChecked && chkBiometricChecked);
        ItemsEnabled[3] = (chkCardChecked && chkPinChecked);
        var n;
        for (n = 0; n < 4; n++) {
            roCB_disableItem(Prefix + '_cmbFunctionality', n, !ItemsEnabled[n]);
        }

        /* Si está seleccionado un item desactivado, borrar la selección*/
        var FunctionalityValue = document.getElementById(Prefix + '_cmbFunctionality_Value');
        if (FunctionalityValue != null) {

            var ItemValue;
            var combo = document.getElementById(Prefix + '_cmbFunctionality_ComboBoxLabel');
            for (n = 0; n < 4; n++) {
                ItemValue = document.getElementById(Prefix + '_cmbFunctionality_LabelChild_' + n).getAttribute('value');
                if (ItemValue == FunctionalityValue.value) {
                    if (ItemsEnabled[n] == false) {
                        combo.innerHTML = '';
                        combo.setAttribute('value', '');
                        FunctionalityValue.value = '';
                    }
                }
            }
        }
    } 
    catch (e) { 
        showError("IdentifyMethods::SetFunctionality", e); 
    }
}

function ConfirmDeleteBiometricData(IdEmp) {
    try {
        var url = "Employees/srvMsgBoxEmployees.aspx?action=DeleteBiometrics&ID=" + IdEmp;
        parent.ShowMsgBoxForm(url, 400, 300, '');
    }
    catch (e) {
        showError("showErrorPopup", e);
    }
}

