var isShowingLoading = 0;

function showPopupLoader() {
    if (isShowingLoading == 0) {
        if (typeof (window.parent.frames["ifPrincipal"]) != "undefined") {
            window.parent.frames["ifPrincipal"].showLoadingGrid(true);
        } else {
            window.parent.parent.frames["ifPrincipal"].showLoadingGrid(true);
        }
        isShowingLoading += 1;
    }
}

function hidePopupLoader(bForceHide) {
    isShowingLoading -= 1;
    if (isShowingLoading <= 0 || bForceHide) {
        isShowingLoading = 0;
        if (typeof (window.parent.frames["ifPrincipal"]) != "undefined") {
            window.parent.frames["ifPrincipal"].showLoadingGrid(false);
        } else {
            window.parent.parent.frames["ifPrincipal"].showLoadingGrid(false);
        }
    }
}
var IDEmployeeSelector = 0;

function EmployeeSelected(Nodo) {
    if (Nodo.id.substr(0, 1) == 'B') {
        IDEmployeeSelector = Nodo.id.substring(1);
    }
    else {
        IDEmployeeSelector = 0;
    }
}


function ShowEmployeeSelector(button) {
    //IDEmployeeSelector = 0;

    var ButtonBounds = Sys.UI.DomElement.getBounds(button);
    var popup = $find('RoPopupFrame1Behavior');
    popup._xCoordinate = ButtonBounds.x - 300;
    popup._yCoordinate = ButtonBounds.y;
    popup.show();

    document.getElementById('RoPopupFrame1').style.display = '';
}

function HideEmployeeSelector() {
    $find('RoPopupFrame1Behavior').hide();
    document.getElementById('RoPopupFrame1').style.display = 'none';
}