function KeyPressFunction(e, noFunct) {
    tecla = (document.all) ? e.keyCode : e.which;
    if (tecla == 13) {
        if (noFunct) {
            return false;
        } else {
            ButtonClick($get('btAccept'));
        }
        return false;
    }
}

function LoginCheckBrowser() {

    if (BrowserDetect.browser != 'Firefox' && BrowserDetect.browser != 'Explorer' && BrowserDetect.browser != 'Chrome' && BrowserDetect.browser != 'Safari') {
        var InvalidBrowser = document.getElementById('trVersionNotSuportedBrowser');
        if (InvalidBrowser != null) {
            InvalidBrowser.style.display = '';
        }
        return false;
    } else {
        var supported = true;
        if (BrowserDetect.browser == 'Firefox' && BrowserDetect.version < 3) {
            supported = false;
        } else if (BrowserDetect.browser == 'Explorer' && BrowserDetect.version < 9) {
            supported = false;
        } else if (BrowserDetect.browser == 'Chrome' && BrowserDetect.version < 1) {
            supported = false;
        } else if (BrowserDetect.browser == 'Safari' && BrowserDetect.version < 4) {
            supported = false;
        }
        if (supported == false) {
            var UnrecognizedBrowser = document.getElementById('trVersionNotSuportedBrowser');
            if (UnrecognizedBrowser != null) {
                UnrecognizedBrowser.style.display = '';
            }
            var ValidBrowser = document.getElementById('trValidBrowser');
            if (ValidBrowser != null) {
                ValidBrowser.style.display = 'none';
            }
            return false;
        } else {
            return true;
        }
    }
}

function ChangeLanguage() {
    ButtonClick($get('btChangeLanguage'));
}

function CheckRedirectAfterLogin() {
    document.getElementById("HiddenFieldRedirectUrl").value = window.location.hash.substr(1);
}
    
function ShowMessageOutdatedVisibleIfNecessary() {
    //borramnos la cache de navegador de idiomas.

    if (typeof (Storage) !== "undefined") {
        if (localStorage.roLanguage) localStorage.removeItem("roLanguage");
    }


    if (LoginCheckBrowser()) {
        var cookieValue = readCookie("AlreadyLoggedinInOtherLocation", null);
        var permValue = readCookie("SessionInvalidatedByPermissions", null);
        var updateValue = readCookie("UpdateSessionError", null);
        var expiredValue = readCookie("SessionExpired", null);

        if (cookieValue == null) cookieValue = "false";
        if (permValue == null) permValue = "false";
        if (updateValue == null) updateValue = "false";
        if (expiredValue == null) expiredValue = "false";

        var msg = ""
        if (cookieValue == "true") {
            msg = document.getElementById("AlreadyLoggedinInOtherLocation").value;
        }

        if (permValue == "true") {
            msg = document.getElementById("SessionInvalidatedByPermissions").value;
        }

        if (expiredValue == "true") {
            msg = document.getElementById("SessionExpired").value;
        }

        if (updateValue == "true" && expiredValue == "false" && permValue == "false" && cookieValue == "false") {
            msg = document.getElementById("UpdateSessionError").value;
        }


        if (msg != "") {
            var url = "srvMsgBoxUserTasks.aspx?action=MessageEx&TitleKey=CheckSession.Error.GenericTitle&";
            url = url + "DescriptionText=" + msg + "&";
            url = url + "Option1TextKey=CheckSession.Error.Option1Text&";
            url = url + "Option1DescriptionKey=CheckSession.Error.Option1Description&";
            url = url + "Option1OnClickScript=HideMsgBoxForm();removeMsgCookies();return false;&";
            url = url + "IconUrl=~/Base/Images/MessageFrame/dialog-information.png";
            ShowMsgBoxForm(url, 500, 300, '');
        }
    } else {
        removeMsgCookies();
    }
}
function removeMsgCookies() {
    var stamp = '&StampParam=' + new Date().getMilliseconds();
    var _ajax = nuevoAjax();
    _ajax.open("GET", "UserTasksCheck.aspx?action=removeCookies" + stamp, true);
    _ajax.onreadystatechange = function () { }
    _ajax.send(null)

        
}
