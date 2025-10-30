// "Clase" SchedulerClipBoard (copiar/enganxar estructura calendari)
// -------------------------------------------------------------------------------------------------
function roUserHistory() {
    var VTHistory = new Array();
    eraseCookie("VTHistory");

    /* Reseteja la clase clipboard */
    this.clear = function () {
        VTHistory = new Array();
        this.saveInStorage();
    }

    this.update = function () {
        try {
            this.getFromStorage();
        } catch (e) { showError("roUserHistory::update", e); }
    }

    this.getHistory = function () { return VTHistory; }

    this.setHistory = function (oVTHistory) {
        VTHistory = oVTHistory;
        this.saveInStorage();
    }

    /* Afegeix un History i actualitza les cookies */
    this.addHistory = function (LanguageTag, Url, MenuPathSelected) {
        try {
            var objHistory = { "LanguageTAG": LanguageTag, "Url": Url.split("?")[0], "MenuPath": MenuPathSelected };
            var bolFound = false;

            if (VTHistory.length >= 6) {
                for (n = VTHistory.length - 1; n > VTHistory.length - 7; n--) {
                    if (VTHistory[n].Url == objHistory.Url) {
                        bolFound = true;
                        break;
                    }
                }
            } else {
                for (n = 0; n < VTHistory.length; n++) {
                    if (VTHistory[n].Url == objHistory.Url) {
                        bolFound = true;
                        break;
                    }
                }
            }

            if (!bolFound) {
                VTHistory.push(objHistory);
                this.saveInStorage();
            }

        } catch (e) { showError("roUserHistory::addHistory", e); }
    }

    this.saveInStorage = function () {
        try {
            var VTHistorySave = new Array();
            if (VTHistory.length >= 6) {
                for (n = VTHistory.length - 6; n < VTHistory.length; n++) {
                    VTHistorySave.push(VTHistory[n]);
                }
            } else {
                for (n = 0; n < VTHistory.length; n++) {
                    VTHistorySave.push(VTHistory[n]);
                }
            }

            var objHistory = { "History": VTHistorySave };
            localStorage.setItem('tabHistory', JSON.stringify(objHistory));
        } catch (e) { showError("roUserHistory::saveInStorage", e); }
    }

    this.getFromStorage = function () {
        try {
            var objClipBoard;

            var oHistory = localStorage.getItem('tabHistory');
            if (oHistory != null) {
                VTHistory = JSON.parse(oHistory).History;
            }
        } catch (e) { showError("roUserHistory::getFromStorage", e); }
    }

    this.getFromStorage();

} // End Class SchedulerClipBoard

