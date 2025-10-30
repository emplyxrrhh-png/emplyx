function validatePeriod() {
    const input = document.querySelector("[id*='txtBlockUserPeriod'] input")
    if (input.value == "") input.value = "3"; //asignamos el valor mínimo
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