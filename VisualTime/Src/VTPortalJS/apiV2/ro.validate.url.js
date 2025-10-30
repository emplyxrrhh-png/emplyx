(function () {
    var namespace = function (name) {
        var namespaces = name.split('.'),
            namespace = window,
            index;
        for (index = 0; index < namespaces.length; index += 1) {
            namespace = namespace[namespaces[index]] = namespace[namespaces[index]] || {};
        }
        return namespace;
    };

    namespace("Robotics.Client.ValidateURL");
}());

Robotics.Client.ValidateURL = function () {
}

Robotics.Client.ValidateURL.prototype.checkRuntimeURLforClient = function (url, companyId) {
    var checkUrl = url + "/Portalsvcx.svc/isAdfsActive";
    var varData = {};

    return new Promise((resolve, reject) => {
        $.ajax({
            url: checkUrl,
            type: 'GET',
            data: varData,
            timeout: 2000,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            processdata: true,
            headers: {
                'roAuth': VTPortal.roApp.guidToken,
                'roToken': VTPortal.roApp.securityToken,
                'roAlias': VTPortal.roApp.getImpToken(),
                'roSrc': VTPortal.roApp.isRemoteEnvironment(),
                'roCompanyID': companyId,
                'roApp': 'VTPortal'
            },
            xhrFields: {
                withCredentials: true
            },
            success: function (data) {
                if (data.d.Status == 0) {
                    resolve(true);
                } else {
                    resolve(false);
                }
            },
            error: function (error) {
                resolve(false);
            }
        })
    });
}

Robotics.Client.ValidateURL.prototype.getURLforClient = async function (companyId, url, isApp) {
    var serverConfig = { valid: false, isMT: false, url: 'notValid', ssoURL: '', companyId: companyId, defURL: url, defSSL: true };
    var serverURL = url;
    var ssoServerURL = "";

    var bContinueChecking = true;

    //Comprovamos el entorno de debug
    if (window.document.URL.indexOf("localhost:8035") >= 0 && bContinueChecking) {
        var environmentData = companyId.split("-");
        var environmentName = "";
        if (environmentData.length > 1) {
            companyId = environmentData[0];
            environmentName = environmentData[1];
        }

        if (environmentName == "") {
            serverURL = "http://localhost:8035";
            ssoServerURL = "http://localhost:8025";

            var exists = await this.checkRuntimeURLforClient(serverURL + '/api', companyId);
            if (exists) {
                bContinueChecking = false;
                serverConfig = { valid: true, isMT: true, url: serverURL + '/api/Portalsvcx.svc', ssoURL: ssoServerURL, companyId: companyId, defURL: url, defSSL: false };
            }
        } else {
            var newURL = "";
            var newSSOURL = "";
            var bIsEnvironment = true;
            switch (environmentName) {
                case "qa":
                    newURL = "https://qa-vtportal.visualtime.net"
                    newSSOURL = "https://qa-vtlive.visualtime.net";
                    break;
                case "dev":
                    newURL = "https://dev-devvtportal.visualtime.net"
                    newSSOURL = "https://dev-devvtlive.visualtime.net";
                    break;
                case "stage":
                    newURL = "https://staging-vtportal.visualtime.net"
                    newSSOURL = "https://staging-vtlive.visualtime.net";
                    break;
                case "idi01":
                    newURL = "https://vtportaldev-idi01.azurewebsites.net"
                    newSSOURL = "https://vtlivedev-idi01.azurewebsites.net";
                    break;
                case "idi02":
                    newURL = "https://vtportaldev-idi02.azurewebsites.net"
                    newSSOURL = "https://vtlivedev-idi02.azurewebsites.net";
                    break;
                case "idi03":
                    newURL = "https://vtportaldev-idi03.azurewebsites.net"
                    newSSOURL = "https://vtlivedev-idi03.azurewebsites.net";
                    break;
                case "idi04":
                    newURL = "https://vtportaldev-idi04.azurewebsites.net"
                    newSSOURL = "https://vtlivedev-idi04.azurewebsites.net";
                    break;
                case "idi05":
                    newURL = "https://vtportaldev-idi05.azurewebsites.net"
                    newSSOURL = "https://vtlivedev-idi05.azurewebsites.net";
                    break;
                case "idi06":
                    newURL = "https://vtportaldev-idi06.azurewebsites.net"
                    newSSOURL = "https://vtlivedev-idi06.azurewebsites.net";
                    break;
                case "idi07":
                    newURL = "https://vtportaldev-idi07.azurewebsites.net"
                    newSSOURL = "https://vtlivedev-idi07.azurewebsites.net";
                    break;
                case "cegid":
                    newURL = "https://vtportalidi.azurewebsites.net";
                    newSSOURL = "https://vtliveidi.azurewebsites.net";
                    break;
                case "beta":
                    newURL = "https://vtportalidi-stage.azurewebsites.net";
                    newSSOURL = "https://vtportalidi-stage.azurewebsites.net";
                    break;
                default:
                    bIsEnvironment = false;
                    newURL = ""
                    newSSOURL = "";
                    break;
            }

            var exists = await this.checkRuntimeURLforClient(newURL + '/api', companyId);
            if (exists) {
                bContinueChecking = false;
                serverConfig = { valid: true, isMT: true, url: newURL + '/api/Portalsvcx.svc', ssoURL: newSSOURL, companyId: companyId + '-' + environmentName, defURL: url, defSSL: true };
            }
        }
    }

    //Comprovamos si es un entorno HA
    if (bContinueChecking && companyId != "" && companyId != "localhost") {
        if (!isApp) {
            newURL = "";// "/api/portalsvcx.svc ";
            newSSOURL = window.location.origin.replace('portal', 'live');

            var exists = await this.checkRuntimeURLforClient(newURL + '/api', companyId);
            if (exists) {
                bContinueChecking = false;
                serverConfig = { valid: true, isMT: true, url: newURL + '/api/Portalsvcx.svc', ssoURL: newSSOURL, companyId: companyId, defURL: url, defSSL: true };
            }
        } else {
            var environmentData = companyId.split("-");
            var environmentName = "";

            if (environmentData.length > 1) {
                companyId = environmentData[0];
                environmentName = environmentData[1];
            }

            //Comprovamos entornos QA / Stage / idi0x
            var newURL = "";
            var newSSOURL = "";
            var bIsEnvironment = true;
            switch (environmentName) {
                case "qa":
                    newURL = "https://qa-vtportal.visualtime.net"
                    newSSOURL = "https://qa-vtlive.visualtime.net";
                    break;
                case "dev":
                    newURL = "https://dev-devvtportal.visualtime.net"
                    newSSOURL = "https://dev-devvtlive.visualtime.net";
                    break;
                case "stage":
                    newURL = "https://staging-vtportal.visualtime.net"
                    newSSOURL = "https://staging-vtlive.visualtime.net";
                    break;
                case "idi01":
                    newURL = "https://vtportaldev-idi01.azurewebsites.net"
                    newSSOURL = "https://vtlivedev-idi01.azurewebsites.net";
                    break;
                case "idi02":
                    newURL = "https://vtportaldev-idi02.azurewebsites.net"
                    newSSOURL = "https://vtlivedev-idi02.azurewebsites.net";
                    break;
                case "idi03":
                    newURL = "https://vtportaldev-idi03.azurewebsites.net"
                    newSSOURL = "https://vtlivedev-idi03.azurewebsites.net";
                    break;
                case "idi04":
                    newURL = "https://vtportaldev-idi04.azurewebsites.net"
                    newSSOURL = "https://vtlivedev-idi04.azurewebsites.net";
                    break;
                case "idi05":
                    newURL = "https://vtportaldev-idi05.azurewebsites.net"
                    newSSOURL = "https://vtlivedev-idi05.azurewebsites.net";
                    break;
                case "idi06":
                    newURL = "https://vtportaldev-idi06.azurewebsites.net"
                    newSSOURL = "https://vtlivedev-idi06.azurewebsites.net";
                    break;
                case "idi07":
                    newURL = "https://vtportaldev-idi07.azurewebsites.net"
                    newSSOURL = "https://vtlivedev-idi07.azurewebsites.net";
                    break;
                case "cegid":
                    newURL = "https://vtportalidi.azurewebsites.net";
                    newSSOURL = "https://vtliveidi.azurewebsites.net";
                    break;
                case "beta":
                    newURL = "https://vtportalidi-stage.azurewebsites.net";
                    newSSOURL = "https://vtportalidi-stage.azurewebsites.net";
                    break;
                default:
                    bIsEnvironment = false;
                    newURL = "";
                    newSSOURL = "";
                    break;
            }

            if (!bIsEnvironment) {
                //comprovamos si es una url ha
                var exists = await this.checkRuntimeURLforClient('https://vtportal.visualtime.net' + '/api', companyId);
                if (exists) {
                    bContinueChecking = false;
                    serverConfig = {
                        valid: true,
                        isMT: true,
                        url: 'https://vtportal.visualtime.net/api/Portalsvcx.svc',
                        ssoURL: 'https://vtlive.visualtime.net',
                        companyId: companyId,
                        defURL: url,
                        defSSL: true
                    };
                }
                //comprovamos si es una url cannary
                if (bContinueChecking) {
                    exists = await this.checkRuntimeURLforClient('https://portal.visualtime.net' + '/api', companyId);
                    if (exists) {
                        bContinueChecking = false;
                        serverConfig = {
                            valid: true,
                            isMT: true,
                            url: 'https://portal.visualtime.net/api/Portalsvcx.svc',
                            ssoURL: 'https://live.visualtime.net',
                            companyId: companyId,
                            defURL: url,
                            defSSL: true
                        };
                    }
                }
            } else {
                var exists = await this.checkRuntimeURLforClient(newURL + '/api', companyId);
                if (exists) {
                    bContinueChecking = false;
                    serverConfig = { valid: true, isMT: true, url: newURL + '/api/Portalsvcx.svc', ssoURL: newSSOURL, companyId: companyId + '-' + environmentName, defURL: url, defSSL: true };
                }
            }
        }

        //if (bContinueChecking && url == "") url = companyId;
    }

    //comprovamos si es una url SaaS
    if (bContinueChecking) {
        serverURL = (url == "" ? companyId : url);
        serverURL = serverURL.replace(".visualtime.net", "");

        if (serverURL.indexOf(":") > 0) serverURL = serverURL.substring(0, serverURL.indexOf(":"));

        var serverName = serverURL;

        serverURL = "https://" + serverName + '.visualtime.net/VTLiveApi';
        ssoServerURL = "https://" + serverName + '.visualtime.net/VTLive';

        var exists = await this.checkRuntimeURLforClient(serverURL + '/Portal', '');
        if (exists) {
            bContinueChecking = false;
            serverConfig = { valid: true, isMT: false, url: serverURL + '/Portal/Portalsvcx.svc', ssoURL: ssoServerURL, companyId: '', defURL: url, defSSL: true };
        }
    }

    //comprovamos si es una url onPremise
    if (bContinueChecking) {
        serverURL = (url == "" ? companyId : url);

        var serverName = serverURL;

        serverURL = "https://" + serverName + "/VTLiveApi";
        ssoServerURL = "https://" + serverName + "/VTLive";

        var exists = await this.checkRuntimeURLforClient(serverURL + '/Portal', '');
        if (exists) {
            bContinueChecking = false;
            serverConfig = { valid: true, isMT: false, url: serverURL + '/Portal/Portalsvcx.svc', ssoURL: ssoServerURL, companyId: '', defURL: url, defSSL: true };
        }

        if (bContinueChecking) {
            serverURL = "http://" + serverName + "/VTLiveApi";
            ssoServerURL = "http://" + serverName + "/VTLive";

            var exists = await this.checkRuntimeURLforClient(serverURL + '/Portal', '');
            if (exists) {
                bContinueChecking = false;
                serverConfig = { valid: true, isMT: false, url: serverURL + '/Portal/Portalsvcx.svc', ssoURL: ssoServerURL, companyId: '', defURL: url, defSSL: false };
            }
        }
    }

    if (bContinueChecking) serverConfig = { valid: false, isMT: false, url: 'notValid', ssoURL: 'notValid', companyId: '', defURL: url, defSSL: true };

    return serverConfig;
}