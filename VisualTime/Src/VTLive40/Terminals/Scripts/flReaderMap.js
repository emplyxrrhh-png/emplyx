function loadReaderMapFlash(objID, bgImageLocation, argsStr) {
    try {
        if (document.getElementById(objID) != null) {
            var oId = objID.replace("ctl00_contentMainBody_ASPxCallbackPanelContenido_tabCtl01_frmTR", "");
            var oId = oId.replace("_divReaderMap", "");
            ret = '<object classid="clsid:d27cdb6e-ae6d-11cf-96b8-444553540000" codebase="http://fpdownload.macromedia.com/pub/shockwave/cabs/flash/swflash.cab#version=9,0,0,0" id="ReaderMap' + oId + '" width="100%" height="100%" align="middle">';
            ret += '<param name="allowScriptAccess" value="always" />';
            ret += '<param name="movie" value="fla/ReadersMap.swf" />';
            ret += '<param name="quality" value="high" />';
            ret += '<param name="bgcolor" value="#cccccc" />';
            ret += '<param name="wmode" value="transparent" />';
            ret += '<embed src="fla/ReadersMap.swf?bgImageLocation=' + bgImageLocation;
            ret += '" wmode="transparent" quality="high" bgcolor="#cccccc" width="100%" height="100%" swLiveConnect=true id="ReaderMap' + oId + '" name="ReaderMap' + oId + '" align="middle" allowScriptAccess="always" type="application/x-shockwave-flash" pluginspage="http://www.macromedia.com/go/getflashplayer" />';
            ret += '</object>';
            document.getElementById(objID).innerHTML = ret;

            setTimeout("ReaderMap_DoFSCommand('ReaderMap" + oId + "','REPOSITION', '" + argsStr + "');", 800);
        }
    } catch (e) { showError("loaderMapFlash", e); }
}

var isInternetExplorer = navigator.appName.indexOf("Microsoft") != -1;

function ReaderMap_DoFSCommand(objID, command, args) {
    try {
        var oResult = "";
        var ReaderMapObj;
        if (isInternetExplorer) {
            ReaderMapObj = eval("document.all." + objID);
        } else {
            ReaderMapObj = eval("document." + objID);
        }
        if (ReaderMapObj == null) { return; }
        switch (command.toUpperCase()) {
            case "REPOSITION":
                try {
                    ReaderMapObj.jsReposition(args);
                } catch (e) { }
                break;
            case "RESET":
                try {
                    ReaderMapObj.jsReset();
                } catch (e) { }
                break;
            case "CHANGEZONE":
                try {
                    ReaderMapObj.jsChangeZone(args);
                } catch (e) { }
                break;
            case "GETPARMS":
                try {
                    oResult = ReaderMapObj.jsRetParms(args);
                } catch (e) { }
                break;
            case "RELOADBG":
                try {
                    ReaderMapObj.jsReloadBg(args);
                } catch (e) { }
            case "RELOADBGANDCHANGE":
                try {
                    var ret = ReaderMapObj.jsReloadBg(args);
                } catch (e) { }
        }
        return oResult;
    } catch (e) { showError("ReaderMap_DoFSCommand", e); }
}

function changePosZoneIn(IDReader, strPos, IDPlane) {
    try {
        var oParms = new Array();
        oParms = strPos.split("*|*");
        if (oParms[0] != null) { document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_tabCtl01_frmTR' + IDReader + '_FlPositionIn').value = oParms[0]; }
        if (IDPlane != "" || IDPlane != null) { document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_tabCtl01_frmTR' + IDReader + '_FlZoneImgIn').value = IDPlane; }

        //var action = "CHANGEZONE";
        //var args = strPos;

        //if (IDPlane != "") {
        action = "RELOADBGANDCHANGE";
        args = IDPlane + "_" + strPos;
        //}

        setTimeout("ReaderMap_DoFSCommand('ReaderMap" + IDReader + "In','" + action + "', '" + args + "');", 800);
    } catch (e) { showError("changePosZoneIn", e); }
}

function changePosZoneOut(IDReader, strPos, IDPlane) {
    try {
        var oParms = new Array();
        oParms = strPos.split("*|*");
        if (oParms[0] != null) { document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_tabCtl01_frmTR' + IDReader + '_FlPositionOut').value = oParms[0]; }
        if (IDPlane != "" || IDPlane != null) { document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_tabCtl01_frmTR' + IDReader + '_FlZoneImgOut').value = IDPlane; }

        //var action = "CHANGEZONE";
        //var args = strPos;

        //if (IDPlane != "") {
        action = "RELOADBGANDCHANGE";
        args = IDPlane + "_" + strPos;
        //}

        setTimeout("ReaderMap_DoFSCommand('ReaderMap" + IDReader + "Out','" + action + "', '" + args + "');", 800);
    } catch (e) { showError("changePosZoneOut", e); }
}

// Funcio que canvia el Reader de posicio (no canvia la zona!) (each readers)
function changePosReaderIn(IDReader, strPos) {
    try {
        var oParms = new Array();
        oParms = strPos.split("*|*");
        if (oParms[1] != null) { document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_tabCtl01_frmTR' + IDReader + '_FlReadersIn').value = oParms[1]; }
        if (oParms[2] != null) { document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_tabCtl01_frmTR' + IDReader + '_FlActualReaderIn').value = IDReader; }
    } catch (e) { showError("changePosReaderIn", e); }
}

function changePosReaderOut(IDReader, strPos) {
    try {
        var oParms = new Array();
        oParms = strPos.split("*|*");
        if (oParms[1] != null) { document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_tabCtl01_frmTR' + IDReader + '_FlReadersOut').value = oParms[1]; }
        if (oParms[2] != null) { document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_tabCtl01_frmTR' + IDReader + '_FlActualReaderOut').value = IDReader; }
    } catch (e) { showError("changePosReaderOut", e); }
}