var _usr = { tku: "0000", id: 0, lang: "es", hasaccess: 0, hascreate: 0, hascreateEmployee: 0, haschange: 0, hasfields: 0 };

function login() {
    rologin($("#companyId").val(), $("#login").val(), $("#password").val());
}

function changepassword() {
    if ($("#newpassword2").val() == $("#newpassword").val()) {
        rochangepassword($("#currentpass").val(), $("#newpassword").val());
    } else {
        showErrorMsg("#changepassword-response", $.t("diffpasswored", "La contraseña y su validación son diferentes"));
        $("#changepassword-buttons").removeClass("hide");
    }
}

function hasError(data) {
    try {
        if (_.has(data.d, "Status")) {
            if (data.d.Status >= 0) {
                return false;
            } else if (data.d.Status == "NoError" || data.d.Status == "Ok") {
                return false;
            } else {
                return true;
            }
        }
    }
    catch (err) {
        return true;
    }
    return true;
}

function validationcode() {
    $('#tempkey').foundation('reveal', 'close');
    rologin($("#companyId").val(), $("#login").val(), $("#password").val(), $("#code").val());
}

function rologin(companyId, usr, pwd, code) {
    $.ajaxSetup({ cache: false });
    wsClientCompanyId = companyId;
    $("#login-response").html(showLoading());
    $("#login-buttons").addClass("hide");

    //pwd = pwd.replace("+", "%2B");
    //var params = 'usr=' + encodeURIComponent(usr);
    //params += "&pwd=" + encodeURIComponent(ecryptString(pwd));
    //if (_.isString(code)) { params += "&validationCode=" + encodeURIComponent(code); }
    //params += "&isApp=false&appVersion=1";

    //if (params != '') params += '&timestamp=' + new Date().getTime();
    //else params = 'timestamp=' + new Date().getTime();

    var formData = new FormData();
    formData.append("usr", usr);
    formData.append("pwd", ecryptString(pwd));
    if (_.isString(code)) { formData.append("validationCode", code); }
    formData.append("isApp", false);
    formData.append("appVersion", 1);
    formData.append("timestamp", new Date().getTime());
    var jqxhr = callRoboticsWS_POST(jsonEngineURI + "Authenticate", formData, function (data) {
        _usr.showLegalText = (data.d.showLegalText) ? "1" : "0";
        localStorage.setItem("ShowLegalText.VTVisits", "1");

        if (data.d.status == 0) {
            localStorage.setItem("roCompanyId", wsClientCompanyId);
            localStorage.setItem("roToken", data.d.token);
            securityToken = data.d.token;
            _usr.tku = data.d.token;
            _usr.lang = data.d.Language;
            _usr.id = data.d.userid;
            _usr.hasaccess = data.d.hasaccess;
            _usr.hascreate = data.d.hascreate;
            _usr.hascreateEmployee = data.d.hascreateEmployee;
            _usr.haschange = data.d.haschange;
            _usr.hasfields = data.d.hasfields;
            switch (_usr.lang) {
                case 'CAT': changeLocale('ca', false); _usr.lang = 'ca'; break;
                case 'ESP': changeLocale('es', false); _usr.lang = 'es'; break;
            }
            localStorage.setItem("usr", JSON.stringify(_usr));
            $.cookie("tku", _usr.tku, { expires: 2 })

            var params = 'usr=' + encodeURIComponent(usr);
            params += "&pwd=" + encodeURIComponent(ecryptString(pwd));
            if (_.isString(code)) { params += "&validationCode=" + encodeURIComponent(code); }
            params += "&isApp=false&appVersion=1";

            if (params != '') params += '&timestamp=' + new Date().getTime();
            else params = 'timestamp=' + new Date().getTime();

            var hash = localStorage.getItem("hash");
            localStorage.removeItem("hash");
            location.href = location.origin + location.pathname.substring(0, location.pathname.lastIndexOf("/") + 1) + (_.isString(hash) ? hash : "");
        } else {
            localStorage.removeItem("usr");
            if (_.isString(data.d.token)) {
                _usr.tku = data.d.token;
                _usr.id = data.d.userid;
                securityToken = data.d.token;
            }
            var resp = "";
            switch (data.d.status) {
                case -1:
                    resp = $.t("NO_SESSION")
                    break;
                case -2:
                    resp = $.t("BAD_CREDENTIALS")
                    break;
                case -3:
                    resp = $.t("NOT_FOUND")
                    break;
                case -4:
                    resp = $.t("GENERAL_ERROR")
                    break;
                case -5:
                    resp = $.t("WRONG_MEDIA_TYPE")
                    break;
                case -6:
                    resp = $.t("NOT_LICENSED")
                    break;
                case -7:
                    resp = $.t("SERVER_NOT_RUNNING")
                    break;
                case -8:
                    resp = $.t("NO_LIVE_PORTAL")
                    break;
                case -9:
                    resp = $.t("NO_PERMISSIONS")
                    break;
                case -59:
                    resp = $.t("LOGIN_RESULT_LOW_STRENGHT_ERROR")
                    $("#changepassword-response").html(showError({ text: resp }));
                    $('#changepassword').foundation('reveal', 'open');
                    resp = "";
                    break;
                case -60:
                    resp = $.t("LOGIN_RESULT_MEDIUM_STRENGHT_ERROR")
                    $("#changepassword-response").html(showError({ text: resp }));
                    $('#changepassword').foundation('reveal', 'open');
                    resp = "";
                    break;
                case -61:
                    resp = $.t("LOGIN_RESULT_HIGH_STRENGHT_ERROR")
                    $("#changepassword-response").html(showError({ text: resp }));
                    $('#changepassword').foundation('reveal', 'open');
                    resp = "";
                    break;
                case -62:
                    resp = $.t("LOGIN_PASSWORD_EXPIRED")
                    $("#changepassword-response").html(showError({ text: resp }));
                    $('#changepassword').foundation('reveal', 'open');
                    resp = "";
                    break;
                case -63:
                    resp = $.t("LOGIN_NEED_TEMPORANY_KEY")
                    $("#tempkey-response").html(showError({ text: resp }));
                    $('#tempkey').foundation('reveal', 'open');
                    resp = "";
                    break;
                case -64:
                    resp = $.t("LOGIN_TEMPORANY_KEY_EXPIRED")
                    $("#tempkey-response").html(showError({ text: resp }));
                    $('#tempkey').foundation('reveal', 'open');
                    resp = "";
                    break;
                case -65:
                    resp = $.t("LOGIN_INVALID_VALIDATION_CODE")
                    $("#tempkey-response").html(showError({ text: resp }));
                    $('#tempkey').foundation('reveal', 'open');
                    resp = "";
                    break;
                case -66:
                    resp = $.t("LOGIN_BLOCKED_ACCESS_APP")
                    break;
                case -67:
                    resp = $.t("LOGIN_TEMPORANY_BLOQUED")
                    break;
                case -68:
                    resp = $.t("LOGIN_GENERAL_BLOCK_ACCESS")
                    break;
                case -69:
                    resp = $.t("LOGIN_INVALID_CLIENT_LOCATION")
                    break;
                case -70:
                    resp = $.t("LOGIN_INVALID_VERSION_APP")
                    break;
                case -71:
                    resp = $.t("LOGIN_INVALID_APP")
                    break;
                default:
                    resp = $.t("unknownerror") + "(" + data.d.status + ")";
            }
            if (resp != "") { $("#login-response").html(showError({ text: resp })); }
            $(".showerror").delay(8000).fadeOut("slow");
            $("#login-buttons").removeClass("hide");
        }
    }, function () {
        securityToken = '';
        $("#login-response").html(showError({ text: $.t("unknownerror") }));
        $(".showerror").delay(8000).fadeOut("slow");
        $("#login-buttons").removeClass("hide");
    });
}

function rochangepassword(oldpwd, newpwd) {
    $("#login-response").html(showLoading());
    $("#login-buttons").addClass("hide");

    var formData = new FormData();
    formData.append("userId", _usr.id);
    formData.append("oldPassword", ecryptString(oldpwd));
    formData.append("newPassword", ecryptString(newpwd));
    formData.append("timestamp", new Date().getTime());
    var jqxhr = callRoboticsWS_POST(jsonEngineURI + "ChangePassword", formData, function (data) {
        if (data.d.status == 0) {
            localStorage.setItem("roCompanyId", wsClientCompanyId);
            localStorage.setItem("roToken", securityToken);
            _usr.tku = securityToken;
            _usr.lang = typeof data.d.Language != 'undefined' ? data.d.Language : _usr.lang;
            _usr.id = data.d.userid;
            _usr.hasaccess = data.d.hasaccess;
            _usr.hascreate = data.d.hascreate;
            _usr.hascreateEmployee = data.d.hascreateEmployee;
            _usr.haschange = data.d.haschange;
            _usr.hasfields = data.d.hasfields;
            switch (_usr.lang) {
                case 'CAT': changeLocale('ca', false); _usr.lang = 'ca'; break;
                case 'ESP': changeLocale('es', false); _usr.lang = 'es'; break;
            }
            localStorage.setItem("usr", JSON.stringify(_usr));
            $.cookie("tku", _usr.tku, { expires: 2 })
            var hash = localStorage.getItem("hash")
            localStorage.removeItem("hash")
            location.href = location.origin + location.pathname.substring(0, location.pathname.lastIndexOf("/") + 1) + (_.isString(hash) ? hash : "");
        } else {
            localStorage.removeItem("usr");
            if (_.isString(data.d.token)) {
                securityToken = data.d.token;
                _usr.tku = data.d.token;
                _usr.id = data.d.userid;
            }
            var resp = "";
            switch (data.d.status) {
                case -1:
                    resp = $.t("NO_SESSION")
                    break;
                case -2:
                    resp = $.t("BAD_CREDENTIALS")
                    break;
                case -3:
                    resp = $.t("NOT_FOUND")
                    break;
                case -4:
                    resp = $.t("GENERAL_ERROR")
                    break;
                case -5:
                    resp = $.t("WRONG_MEDIA_TYPE")
                    break;
                case -6:
                    resp = $.t("NOT_LICENSED")
                    break;
                case -7:
                    resp = $.t("SERVER_NOT_RUNNING")
                    break;
                case -8:
                    resp = $.t("NO_LIVE_PORTAL")
                    break;
                case -9:
                    resp = $.t("NO_PERMISSIONS")
                    break;
                case -59:
                    resp = $.t("LOGIN_RESULT_LOW_STRENGHT_ERROR")
                    $('#changepassword').foundation('reveal', 'open');
                    $("#changepassword-response").html(showError({ text: resp }));
                    resp = "";
                    break;
                case -60:
                    resp = $.t("LOGIN_RESULT_MEDIUM_STRENGHT_ERROR")
                    $('#changepassword').foundation('reveal', 'open');
                    $("#changepassword-response").html(showError({ text: resp }));
                    resp = "";
                    break;
                case -61:
                    resp = $.t("LOGIN_RESULT_HIGH_STRENGHT_ERROR")
                    $('#changepassword').foundation('reveal', 'open');
                    $("#changepassword-response").html(showError({ text: resp }));
                    resp = "";
                    break;
                case -62:
                    resp = $.t("LOGIN_PASSWORD_EXPIRED")
                    $('#changepassword').foundation('reveal', 'open');
                    $("#changepassword-response").html(showError({ text: resp }));
                    resp = "";
                    break;
                case -63:
                    resp = $.t("LOGIN_NEED_TEMPORANY_KEY")
                    $('#tempkey').foundation('reveal', 'open');
                    $("#tempkey-response").html(showError({ text: resp }));
                    resp = "";
                    break;
                case -64:
                    resp = $.t("LOGIN_TEMPORANY_KEY_EXPIRED")
                    $('#tempkey').foundation('reveal', 'open');
                    $("#tempkey-response").html(showError({ text: resp }));
                    resp = "";
                    break;
                case -65:
                    resp = $.t("LOGIN_INVALID_VALIDATION_CODE")
                    $('#tempkey').foundation('reveal', 'open');
                    $("#tempkey-response").html(showError({ text: resp }));
                    resp = "";
                    break;
                case -66:
                    resp = $.t("LOGIN_BLOCKED_ACCESS_APP")
                    break;
                case -67:
                    resp = $.t("LOGIN_TEMPORANY_BLOQUED")
                    break;
                case -68:
                    resp = $.t("LOGIN_GENERAL_BLOCK_ACCESS")
                    break;
                case -69:
                    resp = $.t("LOGIN_INVALID_CLIENT_LOCATION")
                    break;
                case -70:
                    resp = $.t("LOGIN_INVALID_VERSION_APP")
                    break;
                default:
                    resp = $.t("unknownerror") + "(" + data.d.status + ")";
                    break;
            }
            if (resp != "") { showErrorMsg("#login-response", resp); }
            $("#login-buttons").removeClass("hide");
        }
    }, function () {
        securityToken = '';
        showErrorMsg("#login-response", $.t("unknownerror"));
        $("#login-buttons").removeClass("hide");
    });
}

function roChecklogin() {
    var params = 'timestamp=' + new Date().getTime();

    var jqxhr = callRoboticsWS_POST(jsonEngineURI + "AuthenticateSession", params, function (data) {
        if (data.d.Status == 0) {
            localStorage.setItem("roToken", data.d.token);
            securityToken = data.d.token;
            _usr.tku = data.d.Token;
            _usr.lang = data.d.Language;
            _usr.id = data.d.UserId;
            _usr.perm = data.d.Permission;
            switch (_usr.lang) {
                case 'CAT': changeLocale('ca', false); _usr.lang = 'ca'; break;
                case 'ESP': changeLocale('es', false); _usr.lang = 'es'; break;
            }
            localStorage.setItem("usr", JSON.stringify(_usr));
            $.cookie("tku", _usr.tku, { expires: 2 })
            var hash = localStorage.getItem("hash")
            localStorage.removeItem("hash")
            location.href = location.origin + location.pathname.substring(0, location.pathname.lastIndexOf("/") + 1) + (_.isString(hash) ? hash : "");
        } else {
            $.removeCookie("tku");
        }
    });
}
function changeLocale(locale, bInitLists) {
}

function changeBackground() {
    var jqxhr = callRoboticsWS_GET(jsonEngineURI + "LoginBackground", "", function (data) {
        if (data.d.result == "NoError") {
            $("#backgroundcolumn").css("background-image", "url(" + data.d.base64 + ")");
        }
    });
}

var tpl = ""
tpl = '<div class="text-center">';
tpl += '    <img src="img/loader.gif" class="imgrow" />&nbsp;';
tpl += '    <span data-i18n="loading">Cargando...</span>';
tpl += '</div>';
var showLoading = _.template(tpl);
tpl = '<div class="text-center showerror">';
tpl += '    <div data-alert class="alert-box alert">';
tpl += '        <%= text %>';
tpl += '    </div>';
tpl += '</div>';
var showError = _.template(tpl);
tpl = '<div class="text-center">';
tpl += '    <div data-alert class="alert-box success">';
tpl += '        <%= text %>';
tpl += '    </div>';
tpl += '</div>';
var showSuccess = _.template(tpl);

function showErrorMsg(obj, msg) {
    $(obj).html(showError({ text: msg }));
    $(obj + ">div").delay(1500).slideUp("fast", function () { this.remove(); });
}
function showSuccessMsg(obj, msg) {
    $(obj).html(showSuccess({ text: msg }));
    $(obj + ">div").delay(1500).slideUp("fast", function () { this.remove(); });
}

$(document).ready(function () {
    if (_.isString(localStorage.usr)) {
        if (_usr.tku != "0000") {
            roChecklogin();
        } else {
            var tku = $.cookie("tku");
            if (_.isString(tku)) {
                _usr.tku = tku;
                roChecklogin();
            }
        }
    } else {
        var tku = $.cookie("tku");
        if (_.isString(tku)) {
            _usr.tku = tku;
            roChecklogin();
        }
    }

    Foundation.global.namespace = '';
    $(document).foundation();

    i18n.init({ shortcutFunction: 'defaultValue', resGetPath: 'locales/__ns__.__lng__.json' }, function (t) {
        $(document).i18n();
    });

    if (_.isString(localStorage.roerror)) {
        if (localStorage.roerror == "nosession") {
            localStorage.removeItem("roerror")
            $("#login-response").html(showError({ text: "Se ha caducado la sesión." }));
        }
    }

    $('#password').keypress(function (e) {
        if (e.keyCode == 13)
            $('#loginbtn').click();
    });

    $("#companyId").val(wsClientCompanyId);
    if (isCompanyFixedByDNS) {
        $("#companyId").prop('disabled', true);
    } else {
        $("#companyId").prop('disabled', false);
    }
});

$(window).resize(function () {
    $(".login-form-space").css("height", $(window).height() / 2 - 150);
});