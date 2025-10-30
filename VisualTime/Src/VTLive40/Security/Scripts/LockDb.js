function showCaptcha() {
    var initialDate = initialFreezeDateClient.Get("closeDate");
    var actualDate = txtFreezeDateClient.GetValue()
    if ((initialDate != null && actualDate == null) || (initialDate != null && actualDate != null && initialDate.getTime() > actualDate.getTime())) {
        var contentUrl = "../Base/Popups/GenericCaptchaValidator.aspx?Action=CLOSEDATE";
        CaptchaObjectPopup_Client.SetContentUrl(contentUrl);
        CaptchaObjectPopup_Client.Show();
    } else {
        saveChanges();
    }
}

function captchaCallback(action) {
    switch (action) {
        case "CLOSEDATE":
            saveChanges();
            break;
        case "ERROR":
            showErrorPopup("Error.ValidationFailed", "ERROR", "Error.ValidationFailedDesc", "Error.OK", "", "");
            break;
    }
}