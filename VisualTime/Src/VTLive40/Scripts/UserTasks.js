
let bolUserTasks = true;
let userTasksTimer = -1;

function ShowResolveUserTask(ResolverID, ResolverValue1, ResolverValue2, ResolverValue3) {
    switch (ResolverID) {
        case 'FN:\\Resolver_MovesInvalidCardID': { ShowExternalForm2('Employees/CardsCorrector.aspx', 600, 500, '', '', false, false, false); break; }
        case 'FN:\\Resolver_Over_MaxJobEmployees_Soon': {
            let url = "srvMsgBoxUserTasks.aspx?action=ResolveOverMaxJobEmployeesSoon";
            ShowMsgBoxForm(url, 400, 300, '');
            break;
        }
        case 'FN:\\Resolver_Over_MaxEmployees_Soon': {
            let url = "srvMsgBoxUserTasks.aspx?action=ResolveOverMaxEmployeesSoon&Date=" + ResolverValue1 + "&ActiveEmployees=" + ResolverValue2 + "&MaxEmployees=" + ResolverValue3;
            ShowMsgBoxForm(url, 400, 300, '');
            break;
        }
        case 'FN:\\Resolver_Terminal_Unrecognized': {
            let url = "Wizards/NewTerminalWizard.aspx?action=RegisterTerminal&TerminalSN=" + ResolverValue1 + "&Type=" + ResolverValue2 + "&IP=" + ResolverValue3;
            ShowExternalForm2(url, 500, 450, '', '', false, false, false);
            //ShowMsgBoxForm(url,400,300,'');    
            break;
        }
        case 'FN:\\Resolver_Coverage': {
            let url = "Scheduler/DailyCoveragePlanned.aspx?IDGroup=" + ResolverValue2 + "&CoverageDate=" + ResolverValue1 + "&IP=" + ResolverValue3;
            ShowExternalForm2(url, 1000, 480, '', '', false, false, false);
            break;
        }
        case 'FN:\\Resolver_Absence': {
            let url = "Scheduler/AddCoverage.aspx?IDEmployee=" + ResolverValue2 + "&CoverageDate=" + ResolverValue1 + "&IP=" + ResolverValue3;
            ShowExternalForm2(url, 1000, 450, '', '', false, false, false);
            break;
        }
    }
}

function ReloadAlerts() {
    document.getElementById("btnAlertsCount").innerHTML = '...';
    let stamp = '&StampParam=' + new Date().getMilliseconds();
    let _ajax = nuevoAjax();
    _ajax.open("GET", "UserTasksCheck.aspx?action=reloadAlerts" + stamp, true);
    _ajax.onreadystatechange = function () {
        if (_ajax.readyState == 4) {
            if (_ajax.status == 200) {
                if (_ajax.responseText.substr(0, 2) == 'OK') {
                    
                    clearTimeout(userTasksTimer);
                    RefreshUserTasks(true);
                    $('#divLastUpdate').html(_ajax.responseText.substr(3))
                }
            } 
        }
    }
    _ajax.send(null);
}

// *** COMPRUEBA SI HAY ALERTAS DE USUARIO Y MUESTRA ICONO Y AVISO
function RefreshUserTasks(activeTimer) {

    document.getElementById("btnAlertsMain").className = 'AlertOut';
    document.getElementById("btnAlertsCount").innerHTML = '...';

    let stamp = '&StampParam=' + new Date().getMilliseconds();
    let _ajax = nuevoAjax();
    _ajax.open("GET", "UserTasksCheck.aspx?action=checkSession" + stamp, true);
    _ajax.onreadystatechange = function () {
        if (_ajax.readyState == 4) {
            if (_ajax.status == 200) {
                if (_ajax.responseText.substr(0, 5) == 'FALSE') {
                    clearTimeout(userTasksTimer);
                    let url = "srvMsgBoxUserTasks.aspx?action=Message&Parameters=" + encodeURIComponent(_ajax.responseText.substr(5, _ajax.responseText.length - 5));
                    parent.ShowMsgBoxForm(url, 500, 300, '');
                } else {

                    let fullResponse = _ajax.responseText.split("#");

                    if (fullResponse.length > 1 && fullResponse[1] == "RELOAD") {
                        let msgParameter = "TitleKey=License.Error.Title & " +
                            "DescriptionKey=License.Error.Description&" +
                            "Option1TextKey=License.Accept.Option1Text&" +
                            "Option1DescriptionKey=License.Accept.Option1Description&" +
                            "Option1OnClickScript=HideMsgBoxForm(); location.reload(); return false;&" +
                            "IconUrl=~/Base/Images/MessageFrame/stock_dialog-error.png";
                        let url = "srvMsgBoxUserTasks.aspx?action=Message&Parameters=" + encodeURIComponent(msgParameter)

                        parent.ShowMsgBoxForm(url, 500, 300, '');

                    } else {
                        let stamp2 = '&StampParam=' + new Date().getMilliseconds();

                        let _ajax2 = nuevoAjax();
                        _ajax2.open("GET", "UserTasksCheck.aspx?action=getUserTasksCheck" + stamp2, true);

                        _ajax2.onreadystatechange = function () {
                            if (_ajax2.readyState == 4) {
                                let strResponse2 = _ajax2.responseText;
                                let strResponse3 = strResponse2.split('*');
                                if (strResponse3[0] == 'KO') return;

                                if (parseInt(strResponse3[1].split("#")[0]) > 0) {
                                    if (strResponse3[0].startsWith('UserTasks')) {
                                        let alertNum = strResponse3[0].replace('UserTasks=', '');

                                        $('#divLastUpdate').html(strResponse3[1].split("#")[1])

                                        if (alertNum == "0") {
                                            document.getElementById("btnAlertsMain").className = 'AlertOk';
                                            document.getElementById("btnAlertsCount").innerHTML = '';
                                        } else {
                                            document.getElementById("btnAlertsMain").className = 'AlertOut';
                                            document.getElementById("btnAlertsCount").innerHTML = alertNum;
                                        }
                                    }
                                }

                                if (activeTimer) {
                                    userTasksTimer = setTimeout("RefreshUserTasks(true)", 75000); //Periodic call to server.    
                                }
                            }
                        }
                        _ajax2.send(null);
                    }
                }
            } else {
                clearTimeout(userTasksTimer);
            }
        }
    }
    _ajax.send(null);
}
