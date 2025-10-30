let dataLoading = false;

function frmAddDestinationV2_ShowNew() {
    try {
        dataLoading = true;
        loadAddDestinationBlanks();
        setTimeout(function () {
            window.parent.pushPreviowsParamsDestination()
            dataLoading = false;
        }, 1000);
    } catch (e) { showError("frmAddDestination_ShowNew", e); }
}

function loadAddDestinationBlanks() {
    try {
        var objPrFrm = "";
        var oDis = 'false';

        loadGenericData("OPTIONSEL", objPrFrm + "opTypeEmployeeExport_opEmployeeDocumentTemplate," + objPrFrm + "opTypeEmployeeExport_opEmployeeMailDestination", "0", "X_OPTIONGROUP", "", oDis, false);
        loadGenericData("TYPEOPTIONSEL", objPrFrm + 'opTypeEmployeeExport,' + objPrFrm + 'opTypeMultipleExport', "1", "X_OPTIONGROUP", "", oDis, false);
        loadGenericData("TYPEOPTIONSEL", objPrFrm + 'opTypeSingleExport', "0", "X_OPTIONGROUP", "", oDis, false);

        txtEmailClient.SetValue("");
        txtEmailClient.SetEnabled((oDis == 'false' ? true : false));

        cmbEmployeeDocumentTemplateClient.SetValue('');
        cmbEmployeeDocumentTemplateClient.SetEnabled((oDis == 'false' ? true : false));

        tbAvailableSupervisorsClient.SetValue("");
        tbAvailableSupervisorsClient.SetEnabled((oDis == 'false' ? true : false));


        venableOPC(objPrFrm + 'opTypeEmployeeExport_opEmployeeDocumentTemplate');
        venableOPC(objPrFrm + 'opTypeEmployeeExport_opEmployeeMailDestination');
        linkOPCItems(objPrFrm + 'opTypeEmployeeExport_opEmployeeDocumentTemplate,' + objPrFrm + 'opTypeEmployeeExport_opEmployeeMailDestination');


        venableOPC(objPrFrm + 'opTypeSingleExport');
        venableOPC(objPrFrm + 'opTypeMultipleExport');
        venableOPC(objPrFrm + 'opTypeEmployeeExport');
        linkOPCItems(objPrFrm + 'opTypeSingleExport,' + objPrFrm + 'opTypeEmployeeExport,' + objPrFrm + 'opTypeMultipleExport');



        //        chgOPCItems('2', objPrFrm + 'opTypeSingleExport,' + objPrFrm + 'opTypeEmployeeExport,' + objPrFrm + 'opTypeMultipleExport', 'undefined', false);
        //        chgOPCItems('0', objPrFrm + 'opTypeEmployeeExport_opEmployeeDocumentTemplate,' + objPrFrm + 'opTypeEmployeeExport_opEmployeeMailDestination', 'undefined', false);


    } catch (e) { showError("loadAddDestinationBlanks", e); }
}

function showWUF(frmID, bol, ignoreBg) {
    try {
        var divC = document.getElementById(frmID + '_frm');
        if (divC != null) {
            if (bol == true) {
                if (ignoreBg != true) {
                    disableScreenWUF(frmID, true);
                }

                divC.style.display = '';
                divC.style.marginLeft = ((divC.offsetWidth / 2) * -1) + "px";
                divC.style.marginTop = (((divC.offsetHeight / 2) * -1)) + "px";   //- 160;
            } else {
                if (ignoreBg != true) {
                    disableScreenWUF(frmID, false);
                }

                divC.style.display = 'none';
            }
        }
    } catch (e) { showError("showWUF", e); }
}

//importa parametres a l'iFrame
function frmAddDestinationV2_Show(reportDestination) {
    try {
        //load fields
        var objPrFrm = "";

        var optType = 'supervisors';
        var optValue = '';
        var oDis = 'false';

        optType = typeof reportDestination.type != 'undefined' ? reportDestination.type : 'supervisors';
        optValue = typeof reportDestination.value != 'undefined' ? reportDestination.value : '';

        var valPrinter = '';
        var valEmail = '';
        var valLocation = '';
        var valOption = '0';
        var valEmpRoute = '';
        var valEmpDoc = '';
        var valSupervisors = '';

        var valType = '0';
        var valSupervisorOption = '0';
        var valEmpOption = '0';

        loadAddDestinationBlanks();

        switch (optType) {
            //case "printer":
            //    valType = '0';
            //    valSupervisorOption = '0';
            //    valEmpOption = '0';
            //    valPrinter = optValue;
            //    break;
            case "email":
                valType = '0';
                valSupervisorOption = '1';
                valEmpOption = '0';
                valEmail = optValue;
                break;
            //case "location":
            //    valType = '0';
            //    valSupervisorOption = '2';
            //    valEmpOption = '0';
            //    valLocation = optValue;
            //    break;
            case "empdocument":
                valType = '1';
                valSupervisorOption = '0';
                valEmpOption = '0';
                valEmpDoc = optValue;
                break;
            case "empemail":
                valType = '1';
                valSupervisorOption = '0';
                valEmpOption = '1';
                break;
            //case "emproute":
            //    valType = '1';
            //    valSupervisorOption = '0';
            //    valEmpOption = '2';
            //    valEmpRoute = optValue;
            //    break;
            case "supervisors":
                valType = '2';
                valSupervisorOption = '0';
                valEmpOption = '0';
                valSupervisors = optValue;
                break;
        }

        //loadGenericData("TYPEOPTIONSEL", objPrFrm + 'opTypeSingleExport,' + objPrFrm + 'opTypeEmployeeExport,' + objPrFrm + 'opTypeMultipleExport', valType, "X_OPTIONGROUP", "", oDis, false);
        //loadGenericData("OPTIONSEL", objPrFrm + "opTypeSingleExport_opTypeDestinationPrinter," + objPrFrm + "opTypeSingleExport_opTypeDestinationEmail," + objPrFrm + "opTypeSingleExport_opTypeDestinationLocation", valSupervisorOption, "X_OPTIONGROUP", "", oDis, false);
        //loadGenericData("OPTIONSEL", objPrFrm + 'opTypeEmployeeExport_opEmployeeDocumentTemplate,' + objPrFrm + 'opTypeEmployeeExport_opEmployeeMailDestination,' + objPrFrm + 'opTypeEmployeeExport_opEmployeeRouteDestination', valEmpOption, "X_OPTIONGROUP", "", oDis, false);


        txtEmailClient.SetValue(valEmail);
        txtEmailClient.SetEnabled((oDis == 'false' ? true : false));

        cmbEmployeeDocumentTemplateClient.SetValue(valEmpDoc);
        cmbEmployeeDocumentTemplateClient.SetEnabled((oDis == 'false' ? true : false));

        tbAvailableSupervisorsClient.SetValue(valSupervisors);
        tbAvailableSupervisorsClient.SetEnabled((oDis == 'false' ? true : false));

        //document.getElementById('opTypeEmployeeExport_opEmployeeDocumentTemplate_panOptionPanel').setAttribute('venabled', valType == '1' ? 'True' : 'False');
        //document.getElementById('opTypeEmployeeExport_opEmployeeMailDestination_panOptionPanel').setAttribute('venabled', valType == '1' ? 'True' : 'False');
        //document.getElementById('opTypeEmployeeExport_opEmployeeRouteDestination_panOptionPanel').setAttribute('venabled', valType == '1' ? 'True' : 'False');


        venableOPC(objPrFrm + 'opTypeEmployeeExport_opEmployeeDocumentTemplate');
        venableOPC(objPrFrm + 'opTypeEmployeeExport_opEmployeeMailDestination');
        linkOPCItems(objPrFrm + 'opTypeEmployeeExport_opEmployeeDocumentTemplate,' + objPrFrm + 'opTypeEmployeeExport_opEmployeeMailDestination');

        venableOPC(objPrFrm + 'opTypeSingleExport');
        venableOPC(objPrFrm + 'opTypeMultipleExport');
        venableOPC(objPrFrm + 'opTypeEmployeeExport');
        linkOPCItems(objPrFrm + 'opTypeSingleExport,' + objPrFrm + 'opTypeEmployeeExport,' + objPrFrm + 'opTypeMultipleExport');

        chgOPCItems(valEmpOption, objPrFrm + 'opTypeEmployeeExport_opEmployeeDocumentTemplate,' + objPrFrm + 'opTypeEmployeeExport_opEmployeeMailDestination', 'undefined', false);
        chgOPCItems(valType, objPrFrm + 'opTypeSingleExport,' + objPrFrm + 'opTypeEmployeeExport,' + objPrFrm + 'opTypeMultipleExport', 'undefined', false);


        //show te form
        showWUF('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmAddDestination1', true);
    } catch (e) { console.error("frmAddDestination_Show", e); }
}
window.parent.frmAddDestinationV2_Show = frmAddDestinationV2_Show

function frmAddDestinationV2_Validate() {
    try {
        var objPrFrm = "ctl00_contentMainBody_ASPxCallbackPanelContenido_frmAddDestination1_";

        if (document.getElementById(objPrFrm + 'opTypeSingleExport_rButton').checked) {
            if (txtEmailClient.GetValue() == "" || txtEmailClient.GetValue() == null) {
                showErrorPopup("Error.frmAddDestination.txtEmailTitle", "ERROR", "Error.frmAddDestination.txtEmailTitleDesc", "Error.frmAddDestination.OK", "Error.frmAddDestination.OKDesc", "");
                return false;
            }
        } else if (document.getElementById(objPrFrm + 'opTypeEmployeeExport_rButton').checked) {
            if (document.getElementById(objPrFrm + "opTypeEmployeeExport_opEmployeeDocumentTemplate_rButton").checked) {
                if (cmbEmployeeDocumentTemplateClient.GetValue() == "" || cmbEmployeeDocumentTemplateClient.GetValue() == null) {
                    showErrorPopup("Error.frmAddDocumentTemplate.cmbPrintersTitle", "ERROR", "Error.frmAddDocumentTemplate.cmbPrintersTitleDesc", "Error.frmAddDocumentTemplate.OK", "Error.frmAddDocumentTemplate.OKDesc", "");
                    return false;
                }
            } else if (document.getElementById(objPrFrm + "opTypeEmployeeExport_opEmployeeMailDestination_rButton").checked) {
                return true;
            }
        } else if (document.getElementById(objPrFrm + 'opTypeMultipleExport_rButton').checked) {
            if (tbAvailableSupervisorsClient.GetValue() == "" || tbAvailableSupervisorsClient.GetValue() == null) {
                showErrorPopup("Error.frmAddDestination.txtSupervisorsTitle", "ERROR", "Error.frmAddDestination.txtSupervisorsTitleDesc", "Error.frmAddDestination.OK", "Error.frmAddDestination.OKDesc", "");
                return false;
            }
        }



        return true;

    } catch (e) {
        showError("frmAddDestination_Validate", e);
        return false;
    }
}

//Carrega de tots els camps en un objecte JSON
function frmAddDestinationV2_GetSelected() {
    try {
        var objPrFrm = "";

        var strType = 'supervisors';
        var strValue = '';
        var strDisplay = '';

        if (document.getElementById(objPrFrm + 'opTypeSingleExport_rButton').checked) {
            strType = 'email';
            strValue = txtEmailClient.GetValue();

            if (strValue == "" || strValue == null) {
                strDisplay = document.getElementById('hdnNoHardcodedField').value;
            } else {
                strDisplay = strValue;
            }


        } else if (document.getElementById(objPrFrm + 'opTypeEmployeeExport_rButton').checked) {
            if (document.getElementById(objPrFrm + "opTypeEmployeeExport_opEmployeeDocumentTemplate_rButton").checked) {
                strType = 'empdocument';
                if (cmbEmployeeDocumentTemplateClient.GetSelectedItem() == null) {
                    strValue = '';
                    strDisplay = document.getElementById('hdnLngNoDocument').value;
                } else {
                    strValue = cmbEmployeeDocumentTemplateClient.GetValue();
                    strDisplay = document.getElementById('hdnLngToDocument').value + " " + cmbEmployeeDocumentTemplateClient.GetSelectedItem().text;
                }
            }
            else if (document.getElementById(objPrFrm + "opTypeEmployeeExport_opEmployeeMailDestination_rButton").checked) {
                strType = 'empemail';
                strValue = '';
                strDisplay = document.getElementById('hdnEmployeeField').value;
            }
        } else if (document.getElementById(objPrFrm + 'opTypeMultipleExport_rButton').checked) {
            strType = 'supervisors';
            if (tbAvailableSupervisorsClient.GetValue() == null) {
                strValue = '';
                strDisplay = "0 " + document.getElementById('hdnLngSupervisorsTextSelected').value;
            } else {
                strValue = tbAvailableSupervisorsClient.GetValue();

                if (strValue != "") {
                    var supCount = strValue.split(",").length;

                    if (supCount == 1) strDisplay = supCount + " " + document.getElementById('hdnLngSupervisorTextSelected').value;
                    else strDisplay = supCount + " " + document.getElementById('hdnLngSupervisorsTextSelected').value;
                } else {
                    strDisplay = "0 " + document.getElementById('hdnLngSupervisorsTextSelected').value;
                }
            }
        }

        var returnDestination = {
            type: strType,
            value: strValue,
            display: strDisplay
        }

        return returnDestination;

    } catch (e) {
        showError("AddDestination_FieldsToJSON", e);
        return null;
    }
}

function frmDestinationV2OnChange(s, e) {
    try {

        var selectedItem = frmAddDestinationV2_GetSelected();
        //Don't call parent collectParamValues if data is loading
        if (!dataLoading) {
            window.parent.collectParamValues(selectedItem);
        }

    } catch (e) {

    }
}