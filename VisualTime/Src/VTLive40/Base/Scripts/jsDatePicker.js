function showDatePicker(bol) {
    try {
        var divDP = document.getElementById('dtPicker');
        if (divDP == null) { return; }
        if (bol == null) {
            if (divDP.style.display == "") {
                divDP.style.display = 'none';
            } else {
                divDP.style.display = '';
            }
        } else {
            if (bol) {
                divDP.style.display = '';
            } else {
                divDP.style.display = 'none';
            }
        }
    } catch (e) { showError("showDatePicker", e); }
}

function ChangeColor(color) {
    try {
        var divCol = document.getElementById('colorZone');
        if (divCol == null) { return; }
        divCol.style.backgroundColor = color;
    } catch (e) { showError("Changecolor", e); }
}