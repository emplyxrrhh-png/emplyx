//*****************************************************************************************/
// nuevoAjax
// Carrega d'un nou objecte Ajax
//********************************************************************************************/
function nuevoAjax() {
    var xmlhttp = false;

    switch (BrowserDetect.browser) {
        case 'Firefox':
        case 'Safari':
        case 'Chrome':
            if (!xmlhttp && typeof XMLHttpRequest != 'undefined') {
                xmlhttp = new XMLHttpRequest();
            }
            break;
        case 'Explorer':
            try {
                // Creación del objeto ajax para navegadores diferentes a Explorer
                xmlhttp = new ActiveXObject("Msxml2.XMLHTTP");
            } catch (e) {
                // o bien
                try {
                    // Creación del objet ajax para Explorer
                    xmlhttp = new ActiveXObject("Microsoft.XMLHTTP");
                } catch (E) {
                    xmlhttp = false;
                }
            }
            break;
        default:
            try {
                // Creación del objeto ajax para navegadores diferentes a Explorer
                xmlhttp = new ActiveXObject("Msxml2.XMLHTTP");
            } catch (e) {
                // o bien
                try {
                    // Creación del objet ajax para Explorer
                    xmlhttp = new ActiveXObject("Microsoft.XMLHTTP");
                } catch (E) {
                    xmlhttp = false;
                }
            }

            break;
    }

    return xmlhttp;
    xmlhttp.setRequestHeader("Connection", "close");
}

//*****************************************************************************************/
// AsyncCall
// Fa una crida AJAX
// method = GET | POST (forçat a GET per errors en algunes planes)
// Url = Crida a la web corresponent
// typeCall = CONTAINER | JSON
// si typeCall = CONTAINER, objContainerId = ID del contenidor que rebrá les dades
// si typeCall = JSON, objContainerId = Nom de la variable Global que rebrá les dades
//********************************************************************************************/
function AsyncCall(method, Url, typeCall, objContainerId, func) {
    try {        
        var obj;
        var stamp = '&StampParam=' + new Date().getMilliseconds();

        var ajax = nuevoAjax();


        if (method.toUpperCase() == "POST") {

            Url = Url + stamp;

            var simpleURL = Url.split("?")[0];
            var simpleParms = Url.split("?")[1];

            ajax.open(method, simpleURL, true);
            ajax.setRequestHeader("Content-type", "application/x-www-form-urlencoded");

            ajax.onreadystatechange = function() {
                if (ajax.readyState == 4) {
                    strMessage = ajax.responseText;                    
                    if (strMessage.indexOf("<!-- LOGINREDIRECT ") > -1) {
                        parent.document.location.href = hBaseRef + '/LoginRedirect.aspx';
                        return;
                    }
                    if (strMessage.indexOf("ERROR") == 0 || strMessage.indexOf("MSG") == 0 || strMessage.indexOf("MESSAGE") == 0) {
                        try { showLoadingGrid(false); } catch (e) { }

                        if (strMessage.substr(0, 7) == 'MESSAGE') strMessage = strMessage.substr(7, strMessage.length - 7)
                        if (strMessage.substr(0, 7) == 'MSG') strMessage = strMessage.substr(3, strMessage.length - 3)
                        if (strMessage.substr(0, 7) == 'ERROR') strMessage = strMessage.substr(5, strMessage.length - 5)

                        var url = "Common/srvMsgBoxCommon.aspx?action=Message&Parameters=" + encodeURIComponent(strMessage);
                        parent.ShowMsgBoxForm(url, 500, 300, '');

                    } else { // No hi ha error, comprobem el tipus de missatge
                        switch (typeCall.toUpperCase()) {
                            case "CONTAINER": //Omple el contenidor amb el missatge rebut
                                let container = document.getElementById(objContainerId);
                                if (container != null) container.innerHTML = strMessage;
                                break;
                            case "JSON":
                                eval(objContainerId + " = [" + strMessage + "]");
                                break;
                            case "JSON2":
                                eval(objContainerId + " ='" + strMessage + "'");
                                break;
                            case "JSON3":
                                objContainerId = JSON.parse(strMessage);
                                break;
                        } //end switch
                        if (func != "") {
                            eval(func);
                        }
                    } //end if                
                } //end if
            }          //end function

            ajax.send(simpleParms);

        } else { // GET
            ajax.open(method, Url + stamp, true);

            ajax.onreadystatechange = function() {
                if (ajax.readyState == 4) {
                    strMessage = ajax.responseText;                    
                    if (strMessage.indexOf("<!-- LOGINREDIRECT ") > -1) {
                        parent.document.location.href = hBaseRef + '/LoginRedirect.aspx';
                        return;
                    }
                    //Primer comprobem si ha retornat algun tipus de missatge d'error
                    if (strMessage.indexOf("ERROR") == 0 || strMessage.indexOf("MSG") == 0 || strMessage.indexOf("MESSAGE") == 0) {
                        try { showLoadingGrid(false); } catch (e) { }

                        if (strMessage.substr(0, 7) == 'MESSAGE') strMessage = strMessage.substr(7, strMessage.length - 7)
                        if (strMessage.substr(0, 7) == 'MSG') strMessage = strMessage.substr(3, strMessage.length - 3)
                        if (strMessage.substr(0, 7) == 'ERROR') strMessage = strMessage.substr(5, strMessage.length - 5)

                        var url = "Common/srvMsgBoxCommon.aspx?action=Message&Parameters=" + encodeURIComponent(strMessage);
                        parent.ShowMsgBoxForm(url, 500, 300, '');
                    } else { // No hi ha error, comprobem el tipus de missatge
                        switch (typeCall.toUpperCase()) {
                            case "CONTAINER": //Omple el contenidor amb el missatge rebut
                                var container = document.getElementById(objContainerId);
                                if (container != null) {
                                    container.innerHTML = strMessage;
                                }
                                break;
                            case "JSON":
                                eval(objContainerId + " = [" + strMessage + "]");
                                break;
                        } //end switch
                        if (func != "") {
                            eval(func);
                        }
                    } //end if                
                } //end if
            }            //end function

            ajax.send(null);
        }

    } catch (e) { alert('AsyncCall: ' + e.description); }

}

function AsyncCall2(method, Url, typeCall, objContainerId, func) {
    try {

        var obj;
        var stamp = '&StampParam=' + new Date().getMilliseconds();

        var ajax = nuevoAjax();

        if (method.toUpperCase() == "POST") {

            Url = Url + stamp;

            var simpleURL = Url.split("?")[0];
            var simpleParms = Url.split("?")[1];

            ajax.open(method, simpleURL, true);
            ajax.setRequestHeader("Content-type", "application/x-www-form-urlencoded");
            //ajax.setRequestHeader("Content-length", simpleParms.length);
            //ajax.setRequestHeader("Connection", "close");

            ajax.onreadystatechange = function() {
                if (ajax.readyState == 4) {
                    strMessage = ajax.responseText;
                    if (strMessage.indexOf("<!-- LOGINREDIRECT ") > -1) {
                        parent.document.location.href = hBaseRef + '/LoginRedirect.aspx';
                        return;
                    }
                    if (strMessage.indexOf("ERROR") == 0 || strMessage.indexOf("MSG") == 0 || strMessage.indexOf("MESSAGE") == 0) {
                        try { showLoadingGrid(false); } catch (e) { }

                        if (strMessage.substr(0, 7) == 'MESSAGE') strMessage = strMessage.substr(7, strMessage.length - 7)
                        if (strMessage.substr(0, 7) == 'MSG') strMessage = strMessage.substr(3, strMessage.length - 3)
                        if (strMessage.substr(0, 7) == 'ERROR') strMessage = strMessage.substr(5, strMessage.length - 5)

                        var url = "Common/srvMsgBoxCommon.aspx?action=Message&Parameters=" + encodeURIComponent(strMessage);
                        parent.ShowMsgBoxForm(url, 500, 300, '');
                    } else { // No hi ha error, comprobem el tipus de missatge
                        switch (typeCall.toUpperCase()) {
                            case "CONTAINER": //Omple el contenidor amb el missatge rebut
                                var container = document.getElementById(objContainerId);
                                if (container != null) {
                                    container.innerHTML = strMessage;
                                }
                                break;
                            case "JSON":
                                eval(objContainerId + " = [" + strMessage + "]");
                                break;
                        } //end switch
                        if (func != "") {
                            eval(func);
                        }
                    } //end if                
                } //end if
            }         //end function

            ajax.send(simpleParms);

        } else { // GET
            ajax.open(method, Url + stamp, true);

            ajax.onreadystatechange = function() {
                if (ajax.readyState == 4) {
                    strMessage = ajax.responseText;
                    if (strMessage.indexOf("<!-- LOGINREDIRECT ") > -1) {
                        parent.document.location.href = hBaseRef + '/LoginRedirect.aspx';
                        return;
                    }
                    if (strMessage.indexOf("ERROR") == 0 || strMessage.indexOf("MSG") == 0 || strMessage.indexOf("MESSAGE") == 0) {
                        try { showLoadingGrid(false); } catch (e) { }

                        if (strMessage.substr(0, 7) == 'MESSAGE') strMessage = strMessage.substr(7, strMessage.length - 7)
                        if (strMessage.substr(0, 7) == 'MSG') strMessage = strMessage.substr(3, strMessage.length - 3)
                        if (strMessage.substr(0, 7) == 'ERROR') strMessage = strMessage.substr(5, strMessage.length - 5)

                        var url = "Common/srvMsgBoxCommon.aspx?action=Message&Parameters=" + encodeURIComponent(strMessage);
                        parent.ShowMsgBoxForm(url, 500, 300, '');
                    } else { // No hi ha error, comprobem el tipus de missatge
                        switch (typeCall.toUpperCase()) {
                            case "CONTAINER": //Omple el contenidor amb el missatge rebut
                                var container = document.getElementById(objContainerId);
                                if (container != null) {
                                    container.innerHTML = strMessage;
                                }
                                break;
                            case "JSON":
                                eval(objContainerId + " = [" + strMessage + "]");
                                break;
                        } //end switch
                        if (func != "") {
                            eval(func);
                        }
                    } //end if                
                } //end if
            }            //end function

            ajax.send(null);
        }

    } catch (e) { alert('AsyncCall2: ' + e.description); }

}

function AjaxCall(method, DataType, Url, parms, typeCall, objContainerId, func, timeout) {
    try {
        var defaultTimeout = 15000;

        if (typeof timeout != 'undefined') {
            defaultTimeout = timeout * 1000;
        }

        $.ajax({
            type: method,
            dataType: DataType,
            url: Url,
            data: parms,
            async: true,
            beforeSend: function(objeto) {
                //roLoader_Show('Loader');
            },
            complete: function(objeto, exito) {
                if (exito == "success") {
                    //roLoader_Hide('Loader');
                    if (func != "") {
                        eval(func);
                    }
                }
            },
            contentType: "application/x-www-form-urlencoded",
            dataType: "html",
            error: function(objeto, quepaso, otroobj) {
                alert("Error: " + quepaso);
            },
            global: true,
            ifModified: false,
            processData: true,
            success: function(datos) {
                switch (typeCall.toUpperCase()) {
                    case "CONTAINER":
                        $("#" + objContainerId).html(datos);
                        break;
                    case "JSON":
                        eval(objContainerId + " = [" + datos + "]");
                        break;
                } //end switch
            },
            timeout: defaultTimeout
        });

    } catch (e) { showError("Ajax3::AjaxCall", e); }

}

function LoadScriptAndExec(urlJs, jsFunct) {
    try {
        $.ajax({
            type: "GET",
            url: urlJs,
            dataType: "script",
            async: false,
            complete: function(objeto, exito) {
                if (exito == "success") {
                    eval(jsFunct + "();");
                } //end if
            } //end function
        });
    } catch (e) { showError("Ajax3::LoadScriptAndExec", e); }
}

function LoadScript(urlJs) {
    try {
        $.ajax({
            type: "GET",
            url: urlJs,
            dataType: "script",
            async: false
        });
    } catch (e) { showError("Ajax3::LoadScript", e); }
}

function CheckLoadScript(funct, urlJs, exec) {
    var isFunct;
    eval("isFunct = (typeof " + funct + " == 'function')");
    if (isFunct) {
        if (funct != "" || !exec) { eval(funct + "()"); }
    } else {
        if (exec) {
            LoadScriptAndExec(urlJs, funct);
        } else {
            LoadScript(urlJs);
        }
    }
}
