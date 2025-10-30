//$.support.cors = true;

var jsonEngineURI = "";
var isSSL = false;

var isCompanyFixedByDNS = false;

var serverUrl = window.location.href.toLowerCase().replace('https://', '').replace('http://', '');
serverUrl = serverUrl.substr(0, serverUrl.indexOf('/'));

var appName = serverUrl.split(".")[0];

if (appName.toLowerCase().indexOf("-visits") > 0 || appName.toLowerCase().indexOf("-vtvisits") > 0) {
    isCompanyFixedByDNS = true;
    localStorage.setItem("roCompanyId", appName.split("-")[0]);
}
if (window.location.href.toLowerCase().indexOf('https://') == -1) isSSL = false;
else isSSL = true;

var MTURL = $.cookie('MTLiveApiUrl');
var MTURLUncoded = decodeURIComponent(MTURL);

if (typeof MTURL != 'undefined' && MTURL != null && MTURL != "") {
    jsonEngineURI = MTURLUncoded;
    if (!jsonEngineURI.endsWith("/")) jsonEngineURI += "/";

    if (jsonEngineURI.indexOf('https://') == -1) isSSL = false;
    else isSSL = true;
} else {
    jsonEngineURI = "http" + (isSSL ? "s://" : "://") + serverUrl + "/VTLiveApi/Visits/VisitsSvcx.svc/";
}

jsonEngineURI = 'Api/VisitsSvcx.svc/'; //Descomentar linea en development

var timeout = 90000;
var wsGUID = '';
var wsClientCompanyId = '';

var tmpCompanyId = localStorage.getItem("roCompanyId");
if (typeof tmpCompanyId != 'undefined' && tmpCompanyId != '' && tmpCompanyId != null) wsClientCompanyId = tmpCompanyId;

function guuid() {
    var UUID = 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) { var r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8); return v.toString(16); });
    return UUID;
};

var tmpGuid = localStorage.getItem("roAuth");
if (typeof tmpGuid != 'undefined' && tmpGuid != '' && tmpGuid != null) wsGUID = tmpGuid;
else {
    wsGUID = guuid();
    localStorage.setItem("roAuth", wsGUID);
}

var securityToken = '';

var securityToken = localStorage.getItem("roToken");
if (typeof securityToken == 'undefined' || securityToken == null) securityToken = '';

function callRoboticsWS_GET(url, params, onSuccess, onError) {
    if (typeof onError == 'undefined') onError = function () { };
    $.ajax({
        type: "GET", //GET or POST or PUT or DELETE verb
        url: url, // Location of the service
        timeout: timeout,
        data: decodeURIComponent(params), //Data sent to server
        contentType: "application/json; charset=utf-8", // content type sent to server
        dataType: "json", //Expected data format from server
        processdata: true, //True or False
        success: onSuccess,
        error: function (error) {
            if (typeof onError == "function") {
                onError();
            }
        }
        , // When Service call fails
        headers: { 'roAuth': wsGUID, 'roToken': securityToken, 'roAlias': '-1', 'roSrc': 'false', 'roCompanyID': wsClientCompanyId, 'roApp': 'Visits' },
        xhrFields: {
            withCredentials: true
        }
    });
};

function callRoboticsWS_POST(url, formData, onSuccess, onError) {
    if (typeof onError == 'undefined') onError = function () { };
    $.ajax({
        type: 'POST',
        url: url, // Location of the service
        timeout: timeout,
        data: formData,
        processData: false,
        contentType: false,
        cache: false,
        headers: { 'roAuth': wsGUID, 'roToken': securityToken, 'roAlias': '-1', 'roSrc': 'false', 'roCompanyID': wsClientCompanyId, 'roApp': 'Visits' },
        success: onSuccess,
        error: function (error) {
            if (typeof onError == "function") {
                onError();
            }
        },
        xhrFields: {
            withCredentials: true
        }
    });
};

function ecryptString(oStr) {
    var encryptedStr = oStr; //CryptoJS.enc.Hex.parse(oStr);
    var key = CryptoJS.enc.Hex.parse('152a3243b4157617c81f2a6b1c2d3e4f');
    var ivs = CryptoJS.enc.Hex.parse('101a12641415161713391a1c1c1d9e1f');
    var encryptor = CryptoJS.AES.encrypt(encryptedStr, key, { iv: ivs });
    return encryptor.toLocaleString();
}

$(document).foundation({
    'reveal': { close_on_background_click: false }
});

$(document).foundation({
    'reveal': { close_on_esc: false }
});