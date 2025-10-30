

var isInternetExplorer = navigator.appName.indexOf("Microsoft") != -1;

function StatusMap_DoFSCommand(command, args) {
    try {
        var oResult = "";
        var StatusMapObj = isInternetExplorer ? document.all.StatusMap : document.StatusMap;
        switch (command.toUpperCase()) {
            case "REPOSITION":
                try {
                    StatusMapObj.jsReposition(args);
                } catch (e) { }
                break;
            case "RESET":
                try {
                    StatusMapObj.jsReset();
                } catch (e) { }
                break;
            case "GETPARMS":
                try {
                    oResult = StatusMapObj.jsRetParms(args);
                } catch (e) { }
                break;
            case "RELOADBG":
                try {
                    StatusMapObj.jsReloadBg(args);
                } catch (e) { }
        }
        return oResult;
    } catch (e) { showError("StatusMap_DoFSCommand", e); }
}