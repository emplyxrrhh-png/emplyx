var monitor = -1;

function showCaptcha() {
    var contentUrl = "../Base/Popups/GenericCaptchaValidator.aspx?Action=COPYPLAN";
    CaptchaObjectPopup_Client.SetContentUrl(contentUrl);
    CaptchaObjectPopup_Client.Show();
}

function captchaCallback(action) {
    switch (action) {
        case "COPYPLAN":
            AspxLoadingPopup_Client.Show();
            $find('mpxEspecialPasteBehavior').hide();
            PerformAction();
            break;
        case "ERROR":
            showErrorPopup2("Error.ValidationFailed", "ERROR", "Error.ValidationFailedDesc", "Error.OK", "", "");
            break;
    }
}

function PerformValidation() {
    PerformActionCallbackClient.PerformCallback("VALIDATE");
}

function PerformAction() {
    PerformActionCallbackClient.PerformCallback("PERFORM_ACTION");
}

function PerformActionCallback_CallbackComplete(s, e) {
    if (s.cpAction == "VALIDATE") {
        if (s.cpResult == true) {
            showCaptcha();
        } else {
            showErrorPopup2("Error.DatesPeriod", "ERROR", "Error.DatesPeriodDesc", "Error.OK", "", "");
        }
    } else if (s.cpAction == "PERFORM_ACTION") {
        monitor = setInterval(function () { PerformActionCallbackClient.PerformCallback("CHECKPROGRESS"); }, 5000);
    } else if (s.cpAction == "ERROR") {
        clearInterval(monitor);
        AspxLoadingPopup_Client.Hide();
        showErrorPopup2("Error.CopyError", "ERROR", "Error." + s.cpActionMsg, "Error.OK", "", "RefreshScreen('1')");
    } else if (s.cpAction == "CHECKPROGRESS") {
        if (s.cpActionResult == "OK") {
            clearInterval(monitor);
            AspxLoadingPopup_Client.Hide();
            showErrorPopup2("OK.CopyOK", "INFO", "OK." + s.cpActionMsg, "Error.OK", "", "RefreshScreen('1')");
        }
    }
}

function GridIncidences_beginCallback(e, c) {
}

function GridIncidences_EndCallback(s, e) {
    if (s.IsEditing()) {
    }
}

function GridIncidences_FocusedRowChanged(s, e) {
    if (s.IsEditing()) {
        s.UpdateEdit();
    }
}

function GridIncidences_OnRowDblClick(s, e) {
    if (s.IsEditing()) {
        s.UpdateEdit();
    }
    s.StartEditRow(e.visibleIndex);
}

function GridIncidences_SelectionChanged(s, e) {
    s.GetSelectedFieldValues("ValueHoraEditable", GetSelectedFieldValuesCallback);
}

function GetSelectedFieldValuesCallback(values) {
    var totalTime = 0;
    for (var i = 0; i < values.length; i++) {
        totalTime += parseFloat(values[i].filterTimeFormat());
    }

    var timeStr = totalTime.HoursToHHMMSS(false);
    while (timeStr.length < 9) timeStr = "0" + timeStr;
    txtTimeSelectedClient.SetValue(timeStr);
}

function GridCancelEditing() {
    try {
        if (IsGridEditing(false)) {
            GridIncidencesClient.CancelEdit();
        }
    }
    catch (e) {
        showError("IsGridEditing", e);
    }
}

function IsGridEditing(showWarning) {
    try {
        if (typeof (GridIncidencesClient) != "undefined") {
            if (typeof (showWarning) == "undefined" || showWarning == null) showWarning = true;

            var bResult = false
            if (GridIncidencesClient.IsEditing()) {
                bResult = true;
                if (showWarning == true) {
                    showPopupMoves2("DataEditing.Message", "INFO", "", "DataEditing.OK", "", "", "", "", "");
                }
            }
            return bResult;
        }
        else {
            return true;
        }
    }
    catch (e) {
        showError("IsGridEditing", e);
    }
}