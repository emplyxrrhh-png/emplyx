var arrStatus;

function btnLanzarInforme_Click() {
    try {
        var filas = GridReportsClient.GetSelectedRowCount();
        if (filas > 0) {
            GridReportsClient.GetSelectedFieldValues('ID', launchReport);
        }
        else {
            url = "srvMsgBoxEmergency.aspx?action=Message&TitleKey=LaunchReport.Error.Text&" +
                "DescriptionText=" + document.getElementById("hdnReportNameEmpty").value + "&" +
                "Option1TextKey=LaunchReport.Error.Option1Text&" +
                "Option1DescriptionKey=LaunchReport.Error.Option1Description&" +
                "Option1OnClickScript=HideMsgBoxForm(); return false;&" +
                "IconUrl=~/Base/Images/MessageFrame/dialog-information.png";
            ShowMsgBoxForm(url, 400, 300, '');
        }
    }
    catch (e) {
        showError('btnLanzarInforme_Click', e);
    }
}

function launchReport(selectedIDs) {
    try {
        let sCompany = document.getElementById("txtCompany").value;

        if (selectedIDs.length > 0 && sCompany.length > 0) {
            let idReports = "";
            for (j = 0; j < selectedIDs.length; j++) {
                idReports = idReports + selectedIDs[j] + ",";
            }
            if (idReports.length > 0) {
                idReports = idReports.substr(0, idReports.length - 1);
                let keyReport = document.getElementById("txtEmergencyReportKey").value;
                showLoadingGrid(true);
                btnLanzarInforme.SetEnabled(false);
                AsyncCall("POST", "srvEmergency.aspx?action=executeEmergencyReport&idReports=" + idReports + '&keyReport=' + keyReport + '&sCompany=' + sCompany, "json", "arrStatus", "launchedReport(arrStatus); ");
            }
        }
    }
    catch (e) {
        showLoadingGrid(false);
        btnLanzarInforme.SetEnabled(true);
        showError('launchReport', e);
    }
}

function launchedReport(oStatus) {
    try {
        arrStatus = oStatus;
        objError = arrStatus[0];

        showLoadingGrid(false);
        btnLanzarInforme.SetEnabled(true);

        if (objError.error != "true") {
            GridReportsClient.UnselectRows();
            document.getElementById("txtEmergencyReportKey").value = "";
        }

        var url = "";
        if (objError.error == "true") {
            url = "srvMsgBoxEmergency.aspx?action=Message&TitleKey=LaunchReport.Error.Text&" +
                "DescriptionText=" + objError.msg + "&" +
                "Option1TextKey=LaunchReport.Error.Option1Text&" +
                "Option1DescriptionKey=LaunchReport.Error.Option1Description&" +
                "Option1OnClickScript=HideMsgBoxForm(); return false;&" +
                "IconUrl=~/Base/Images/MessageFrame/stock_dialog-error.png";
        }
        else {
            url = "srvMsgBoxEmergency.aspx?action=Message&TitleKey=LaunchReport.Error.Text&" +
                "DescriptionText=" + objError.msg + "&" +
                "Option1TextKey=LaunchReport.Error.Option1Text&" +
                "Option1DescriptionKey=LaunchReport.Error.Option1Description&" +
                "Option1OnClickScript=HideMsgBoxForm(); return false;&" +
                "IconUrl=~/Base/Images/MessageFrame/dialog-information.png";
        }
        ShowMsgBoxForm(url, 400, 300, '');
    }
    catch (e) {
        showError("checkStatus", e);
    }
}

function showLoadingGrid(loading) { parent.showLoader(loading); }