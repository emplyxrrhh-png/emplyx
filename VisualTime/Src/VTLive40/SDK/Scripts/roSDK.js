//finds out what TAB _BUTTONS_ should be shown
function cargaTabSuperior(actualTab) {
    try {
        AsyncCall("POST", "Handlers/srvManagePunches.ashx?action=getTabs&aTab=" + actualTab, "CONTAINER", "divManagePunches", "Init()");
    }
    catch (e) {
        showError("cargaTabSuperior", e);
    }
}

var actualTab = 0;
function changeTabs(numTab) {
    var arrButtons = new Array('TABBUTTON_ErasePunches', 'TABBUTTON_ImportPunches');

    for (n = 0; n < arrButtons.length; n++) {
        var tab = document.getElementById(arrButtons[n]);
        if (n == numTab) {
            if (tab != null) tab.className = 'bTab-active';
        } else {
            if (tab != null) tab.className = 'bTab';
        }
    }
    actualTab = numTab;
}

function ASPxCallbackPanelContenidoClient_EndCallBack(s, e) {
    if (s.cpResultRO != 'OK') {
        showPopup("Popup.Error", "ERROR", "", s.cpResultRO, "Error.OK", "", "");
    } else {
        switch (s.cpActionRO) {
            case "uploadFileVTX":
                GridPunchesToImportClient.Refresh();
                importRowCount = s.cpImportRowCount;
                recalcLabel();
                break;
            case "execute":
                showPopup("Popup.ExecuteSuccess", "INFO", "", "", "Error.OK", "", "");
                GridPunchesToImportClient.Refresh();
                $("#txtFileToErase").val("");
                break;
        }
    }
    LoadingPanelSDKClient.Hide();
}

var importRowCount = 0;

function recalcLabel() {
    //lblCountToImport_Client.SetText("Se importarán " + importRowCount + " fichajes."); //TODO: translation
}

function Init() {
    //changeTabs(actualTab);
    $('#txtFileToErase').on('click', function () {
        $(this).attr("value", "");
    })

    $('#txtFileToErase').on('change', function () {
        UploadFile(this);
    });
}

function UploadFile(sender) {
    var action = "uploadFileVTX";
    LoadingPanelSDKClient.Show();
    var data = new FormData();
    if (sender.files.length > 0)
        data.append('importFile', sender.files[0]);
    $.ajax({
        url: '../sdk/Handlers/srvManagePunches.ashx?action=' + action,
        type: 'POST',
        data: data,
        cache: false,
        processData: false, // Don't process the files
        contentType: false, // Set content type to false as jQuery will tell the server its a query string request
        success: function (data, textStatus, jqXHR) {
            var oParameters = {};
            oParameters.StampParam = new Date().getMilliseconds();
            oParameters.action = action;
            ASPxCallbackPanelContenidoClient.PerformCallback(encodeURIComponent(JSON.stringify(oParameters)));
        },
        error: function (jqXHR, textStatus, errorThrown) {
            LoadingPanelSDKClient.Hide();
            showPopup("Popup.UploadFileFailed", "ERROR", "", jqXHR.responseText, "Error.OK", "", "");
        }
    });
}

function showCaptchaImport(e, s) {
    var contentUrl = "../Base/Popups/GenericCaptchaValidator.aspx?Action=EXECUTEIMPORT";
    CaptchaObjectPopup_Client.SetContentUrl(contentUrl);
    CaptchaObjectPopup_Client.Show();
    e.preventDefault();  //to avoid refreshing the page...
}

function ExecuteImport() {
    LoadingPanelSDKClient.Show();

    var oParameters = {};
    oParameters.StampParam = new Date().getMilliseconds();
    oParameters.action = 'execute';
    ASPxCallbackPanelContenidoClient.PerformCallback(encodeURIComponent(JSON.stringify(oParameters)));
}

function captchaCallback(action) {
    switch (action) {
        case "EXECUTEIMPORT":
            ExecuteImport();
            break;
        case "ERROR":
            showPopup("Popup.ValidationFailed", "ERROR", "", "", "Error.OK", "", "");
            break;
    }
}

function showPopup(Title, typeIcon, DescriptionKey, DescriptionText, Opt1Text, Opt1Desc, strScript1, Opt2Text, Opt2Desc, strScript2, Opt3Text, Opt3Desc, strScript3) {
    try {
        var url = "SDK/srvMsgBoxSDK.aspx?action=Message";
        url = url + "&TitleKey=" + Title;
        if (DescriptionKey != "") url = url + "&DescriptionKey=" + DescriptionKey;
        if (DescriptionText != "") url = url + "&DescriptionText=" + DescriptionText;
        url = url + "&Option1TextKey=" + Opt1Text;
        url = url + "&Option1DescriptionKey=" + Opt1Desc;
        url = url + "&Option1OnClickScript=HideMsgBoxForm();" + strScript1 + "; return false;";
        if (Opt2Text != null) {
            url = url + "&Option2TextKey=" + Opt2Text;
            url = url + "&Option2DescriptionKey=" + Opt2Desc;
            url = url + "&Option2OnClickScript=HideMsgBoxForm();" + strScript2 + "; return false;";
        }
        if (Opt3Text != null) {
            url = url + "&Option3TextKey=" + Opt3Text;
            url = url + "&Option3DescriptionKey=" + Opt3Desc;
            url = url + "&Option3OnClickScript=HideMsgBoxForm();" + strScript3 + "; return false;";
        }
        if (typeIcon.toUpperCase() == "TRASH") {
            url = url + "&IconUrl=~/Base/Images/MessageFrame/trash64.png";
        } else if (typeIcon.toUpperCase() == "ERROR") {
            url = url + "&IconUrl=~/Base/Images/MessageFrame/alert32.png";
        } else if (typeIcon.toUpperCase() == "INFO") {
            url = url + "&IconUrl=~/Base/Images/MessageFrame/dialog-information.png";
        }

        parent.ShowMsgBoxForm(url, 400, 300, '');
    } catch (e) { showError("showPopup", e); }
}