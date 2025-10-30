function UpdFeaturePermission(obj, IDPassport, idFeature, FeatureType, Permission, prefix, srvPagelocation) {
    var stamp = '&StampParam=' + new Date().getMilliseconds();

    var ajax = nuevoAjax();
    ajax.open("GET", srvPagelocation + "Handlers/srvSupervisorsV3.ashx?action=SetFeaturePermission&ID=" + IDPassport + "&IDFeature=" + idFeature + "&FeatureType=" + FeatureType + "&Permission=" + Permission + stamp, true);
    ajax.onreadystatechange = function () {
        if (ajax.readyState == 4) {
            if (ajax.responseText.substr(0, 2) == 'OK') {
                var strResponse = ajax.responseText.substr(2, ajax.responseText.length - 2)
                var FeaturesChanged = new Array();
                FeaturesChanged = strResponse.split('~');

                var Feature = new Array();
                var n;
                for (n = 0; n < FeaturesChanged.length; n++) {
                    Feature = FeaturesChanged[n].split('*');
                    SetFeaturePermission(Feature[0], Feature[1], (Feature[1] == Feature[2]), prefix);
                }

                //cargaPassport(IDPassport);
            }
            else {
                if (ajax.responseText.substr(0, 7) == 'MESSAGE') {
                    var url = srvPagelocation + "srvMsgBoxPassports.aspx?action=Message&Parameters=" + encodeURIComponent(ajax.responseText.substr(7, ajax.responseText.length - 7));
                    ShowMsgBoxForm(url, 500, 300, '');
                }
            }
        }
    }
    ajax.send(null);

    /*
    var optionFilter = '';
    for(n=1;n<=4;n++){
    var PermissionObj = null;
    switch (n){
    case 1: PermissionObj = document.getElementById('a' + idFeature + 'PermissionAdmin'); break;
    case 2: PermissionObj = document.getElementById('a' + idFeature + 'PermissionWrite'); break;
    case 3: PermissionObj = document.getElementById('a' + idFeature + 'PermissionRead'); break;
    case 4: PermissionObj = document.getElementById('a' + idFeature + 'PermissionNone'); break;
    }

        if (PermissionObj != null) {
    var _className='';
    var claseObj = new Array();
    claseObj = PermissionObj.className.split(' ');
    for (i=0; i<claseObj.length-1;i++) {
    _className = _className + ' ' + claseObj[i];
    }
    if (PermissionObj.id == obj.id){
    //Es el seleccionat, modifiquem el valor
    if(claseObj[claseObj.length-1]=='PermissionUnPressed'){
    PermissionObj.className = _className + ' PermissionPressed';
    }
    }
    else {
    PermissionObj.className = _className + ' PermissionUnPressed';
    }
    }
    }
    */
}

function UpdDefaultFeaturePermission(obj, IDPassport, idFeature, FeatureType, prefix, srvPagelocation) {
    var stamp = '&StampParam=' + new Date().getMilliseconds();

    var ajax = nuevoAjax();
    ajax.open("GET", srvPagelocation + "Handlers/srvSupervisorsV3.ashx?action=SetDefaultFeaturePermission&ID=" + IDPassport + "&IDFeature=" + idFeature + "&FeatureType=" + FeatureType + stamp, true);
    ajax.onreadystatechange = function () {
        if (ajax.readyState == 4) {
            if (ajax.responseText.substr(0, 2) == 'OK') {
                var strResponse = ajax.responseText.substr(2, ajax.responseText.length - 2)
                var FeaturesChanged = new Array();
                FeaturesChanged = strResponse.split('~');

                var Feature = new Array();
                var n;
                for (n = 0; n < FeaturesChanged.length; n++) {
                    Feature = FeaturesChanged[n].split('*');
                    SetFeaturePermission(Feature[0], Feature[1], (Feature[1] == Feature[2]), prefix);
                }
            }
            else {
                if (ajax.responseText.substr(0, 7) == 'MESSAGE') {
                    var url = srvPagelocation + "srvMsgBoxPassports.aspx?action=Message&Parameters=" + encodeURIComponent(ajax.responseText.substr(7, ajax.responseText.length - 7));
                    ShowMsgBoxForm(url, 500, 300, '');
                }
            }
        }
    }
    ajax.send(null);
}

function SetFeaturePermission(idFeature, Permission, inherited, prefix) {
    for (n = 1; n <= 4; n++) {
        var PermissionObj = null;
        var PermissionObjType = '';
        switch (n) {
            case 1: PermissionObj = document.getElementById(prefix + 'aFeaturePermissionAdmin_' + idFeature); PermissionObjType = 'Admin'; break;
            case 2: PermissionObj = document.getElementById(prefix + 'aFeaturePermissionWrite_' + idFeature); PermissionObjType = 'Write'; break;
            case 3: PermissionObj = document.getElementById(prefix + 'aFeaturePermissionRead_' + idFeature); PermissionObjType = 'Read'; break;
            case 4: PermissionObj = document.getElementById(prefix + 'aFeaturePermissionNone_' + idFeature); PermissionObjType = 'None'; break;
        }

        if (PermissionObj != null) {
            var _className = '';
            _className = 'Permission' + PermissionObjType;
            if (inherited == true) _className = _className + ' Permission' + PermissionObjType + 'Inherited';
            if (PermissionObjType == Permission) {
                _className = _className + ' PermissionPressed';
            }
            else {
                _className = _className + ' PermissionUnPressed';
            }
            PermissionObj.className = _className;
        }
    }
}

function ShowHideFeatureChilds(idFeature, prefix) {
    var tb = document.getElementById(prefix + 'rowFeatureChilds' + idFeature);
    if (tb != null) {
        if (tb.style.display == '') {
            tb.style.display = 'none';
            document.getElementById(prefix + 'aFeatureOpenImg' + idFeature).src = document.getElementById(prefix + 'aFeatureOpenImg' + idFeature).src.replace('minus', 'plus');
        } else {
            tb.style.display = '';
            document.getElementById(prefix + 'aFeatureOpenImg' + idFeature).src = document.getElementById(prefix + 'aFeatureOpenImg' + idFeature).src.replace('plus', 'minus');
        }
    }
}

function ShowHideFeatureInfo(aFeature, idFeature, prefix) {
    var _className = 'FeatureInfoPressed';
    var claseObj = new Array();
    claseObj = aFeature.className.split(' ');
    if (claseObj.length > 1) {
        if (claseObj[1] == 'FeatureInfoPressed') {
            _className = 'FeatureInfoUnPressed';
        }
    }
    aFeature.className = claseObj[0] + ' ' + _className;

    var tb = document.getElementById(prefix + 'rowFeatureInfo' + idFeature);
    if (tb != null) {
        if (_className == 'FeatureInfoPressed')
            tb.style.display = '';
        else
            tb.style.display = 'none';
    }
}