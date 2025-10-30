var isGlobal = false;
function loadLocationMapFlash(bgImageLocation, strParams) {
    if (document.getElementById('divLocationMap') != null) {
        isGlobal = false;
        ret = '<object classid="clsid:d27cdb6e-ae6d-11cf-96b8-444553540000" codebase="http://fpdownload.macromedia.com/pub/shockwave/cabs/flash/swflash.cab#version=9,0,0,0" id="LocationMap" width="100%" height="100%" align="middle">';
        ret += '<param name="allowScriptAccess" value="always" />';
        ret += '<param name="movie" value="fla/LocationMap.swf" />';
        ret += '<param name="quality" value="high" />';
        ret += '<param name="bgcolor" value="#cccccc" />';
        ret += '<param name="wmode" value="transparent" />';
        ret += '<embed src="fla/LocationMap.swf?bgImageLocation=' + bgImageLocation;
        if (strParams != "") {
            //ret += '&strParamZones=' + strParams;
        }
        ret += '" wmode="transparent" quality="high" bgcolor="#cccccc" width="100%" height="100%" swLiveConnect=true id="LocationMap" name="LocationMap" align="middle" allowScriptAccess="always" type="application/x-shockwave-flash" pluginspage="http://www.macromedia.com/go/getflashplayer" />';
        ret += '</object>';
        document.getElementById('divLocationMap').innerHTML = ret;

        setTimeout("LocationMap_DoFSCommand('REPOSITION', '" + strParams + "');", 800);
    }
}

function loadLocationGlobalMapFlash(bgImageLocation, strParams) {
    if (document.getElementById('divLocationMap1') != null) {
        isGlobal = true;
        ret = '<object classid="clsid:d27cdb6e-ae6d-11cf-96b8-444553540000" codebase="http://fpdownload.macromedia.com/pub/shockwave/cabs/flash/swflash.cab#version=9,0,0,0" id="LocationMap" width="100%" height="100%" align="middle">';
        ret += '<param name="allowScriptAccess" value="always" />';
        ret += '<param name="movie" value="fla/LocGlobalMap.swf" />';
        ret += '<param name="quality" value="high" />';
        ret += '<param name="bgcolor" value="#cccccc" />';
        ret += '<param name="wmode" value="transparent" />';
        ret += '<embed src="fla/LocGlobalMap.swf?bgImageLocation=' + bgImageLocation;
        if (strParams != "") {
            //ret += '&strParamZones=' + strParams;
        }
        ret += '" wmode="transparent" quality="high" bgcolor="#cccccc" width="100%" height="100%" swLiveConnect=true id="LocationMap2" name="LocationMap" align="middle" allowScriptAccess="always" type="application/x-shockwave-flash" pluginspage="http://www.macromedia.com/go/getflashplayer" />';
        ret += '</object>';
        document.getElementById('divLocationMap1').innerHTML = ret;

        setTimeout("LocationMap_DoFSCommand('REPOSITION', '" + strParams + "');", 800);
    }
}

var isInternetExplorer = navigator.appName.indexOf("Microsoft") != -1;

function LocationMap_DoFSCommand(command, args) {
    try {
        var oResult = "";
        var LocationMapObj = null;

        if (isGlobal) {
            LocationMapObj = isInternetExplorer ? document.all.LocationMap2 : document.LocationMap2;
        } else {
            LocationMapObj = isInternetExplorer ? document.all.LocationMap : document.LocationMap;
        }

        switch (command.toUpperCase()) {
            case "REPOSITION":
                try {
                    LocationMapObj.jsReposition(args);
                } catch (e) { }
                break;
            case "RESET":
                try {
                    LocationMapObj.jsReset();
                } catch (e) { }
                break;
            case "GETPARMS":
                try {
                    oResult = LocationMapObj.jsRetParms(args);
                } catch (e) { }
                break;
            case "RELOADBG":
                try {
                    LocationMapObj.jsReloadBg(args);
                } catch (e) { }
        }
        return oResult;
    } catch (e) { showError("LocationMap_DoFSCommand", e); }
}