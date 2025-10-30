function showLoadingGrid(loading) { parent.showLoader(loading); }

(function () {
    $(document).ready(async function () {
        var el = document.getElementById("divLastUpdateSite");
        if (el && el.textContent) {
            var utcDate = new Date(el.textContent.trim());
            if (!isNaN(utcDate.getTime())) {
                var localTime = utcDate.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit', second: '2-digit', hour12: false });
                el.textContent = localTime;
            }
        }
    });
})();

var alerts = {
    ASPxCallbackPanelContenidoClient_EndCallBack: function (s, e) {
        if (typeof s.cpLastUpdate != 'undefined') {
            var utcDate = new Date(s.cpLastUpdate);
            var localTime = utcDate.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit', second: '2-digit', hour12: false });
            $("#divLastUpdateSite").html(localTime);
        }
        parent.RefreshUserTasks(false);
        parent.showLoader(false);
        //$(".hidden_refresh").val("1");
    },

    loadData: function () {
        if ($(".hidden_refresh").val() == "1") {
            //if (AlertFirstLoading) parent.showLoader(true);
            var oParameters = {};
            oParameters.StampParam = new Date().getMilliseconds();
            oParameters.action = "REFRESH";

            var strParameters = JSON.stringify(oParameters);
            strParameters = encodeURIComponent(strParameters);
            $(".hidden_refresh").val("0");
            ASPxCallbackPanelContenidoClient.PerformCallback(strParameters);
            AlertFirstLoading = false;
        }
    },
};

function refreshAlertsSite() {
    parent.showLoader(true);
    $(".hidden_refresh").val("1");
    alerts.loadData();
}

var AlertFirstLoading = true;

function ShowResolveUserTask(ResolverID, ResolverValue1, ResolverValue2, ResolverValue3) {
    switch (ResolverID) {
        case 'FN:\\Resolver_MovesInvalidCardID':
            parent.ShowExternalForm2('Employees/CardsCorrector.aspx', 600, 500, '', '', false, false, false);
            break;
        case 'FN:\\Resolver_Over_MaxJobEmployees_Soon':
            var url = "srvMsgBoxUserTasks.aspx?action=ResolveOverMaxJobEmployeesSoon";
            parent.ShowMsgBoxForm(url, 400, 300, '');
            break;
        case 'FN:\\Resolver_Over_MaxEmployees_Soon':
            var url = "srvMsgBoxUserTasks.aspx?action=ResolveOverMaxEmployeesSoon&Date=" + ResolverValue1 + "&ActiveEmployees=" + ResolverValue2 + "&MaxEmployees=" + ResolverValue3;
            parent.ShowMsgBoxForm(url, 400, 300, '');
            break;
        case 'FN:\\Resolver_Terminal_Unrecognized':
            var url = "Wizards/NewTerminalWizard.aspx?action=RegisterTerminal&TerminalSN=" + ResolverValue1 + "&Type=" + ResolverValue2 + "&IP=" + ResolverValue3;
            parent.ShowExternalForm2(url, 500, 450, '', '', false, false, false);
            //ShowMsgBoxForm(url,400,300,'');
            break;
        case 'FN:\\Resolver_Coverage':
            var url = "Scheduler/DailyCoveragePlanned.aspx?IDGroup=" + ResolverValue2 + "&CoverageDate=" + ResolverValue1 + "&IP=" + ResolverValue3;
            parent.ShowExternalForm2(url, 1000, 480, '', '', false, false, false);
            break;
        case 'FN:\\Resolver_Absence':
            var url = "Scheduler/AddCoverage.aspx?IDEmployee=" + ResolverValue2 + "&CoverageDate=" + ResolverValue1 + "&IP=" + ResolverValue3;
            parent.ShowExternalForm2(url, 1000, 450, '', '', false, false, false);
            break;
        case 'FN:\\mxC_NotRegistered':
            var url = "Terminals/Wizards/RegistermxC.aspx?IDTerminal=" + ResolverValue1;
            parent.ShowExternalForm2(url, 500, 450, '', '', false, false, false);
            break;
        case 'FN:\\TERMINAL_NotRegistered':
            var url = "Terminals/Wizards/RegisterWx1.aspx?IDTerminal=" + ResolverValue1 + "&Type=" + ResolverValue2 + "&IP=" + ResolverValue3;
            parent.ShowExternalForm2(url, 500, 450, '', '', false, false, false);
            break;
        case 'FN:\\Resolver_ParserInvalidEntries':
            parent.ShowExternalForm2('Scheduler/PunchesCorrector.aspx', 900, 500, '', '', false, false, false); break;
    }
}

/* Ver dias con fichajes impares */
/*************************************************************************************************************/
function ShowIncompletedDays(BeginDate, EndDate) {
    // nueva pantalla de edicion de fichajes
    var url = vtApplicationPath + 'Scheduler/MovesNew.aspx?action=incompletedDays&DateStart=' + encodeURIComponent(BeginDate) + '&DateEnd=' + encodeURIComponent(EndDate);
    var Title = '';
    parent.ShowExternalForm2(url, 1400, 620, Title, '', false, false, false);
}

function RefreshScreen(DataType, oParms) {
    alerts.loadData();
}

/* Ver dias sin justificar */
/*************************************************************************************************************/
function ShowNotJustifiedDays(BeginDate, EndDate) {
    // nueva pantalla de edicion de fichajes
    var url = vtApplicationPath + 'Scheduler/MovesNew.aspx?action=notjustifiedDays&DateStart=' + encodeURIComponent(BeginDate) + '&DateEnd=' + encodeURIComponent(EndDate);
    var Title = '';
    parent.ShowExternalForm2(url, 1400, 620, Title, '', false, false, false);
}

/* Ver dias no fiables */
/*************************************************************************************************************/
function ShowNotReliabledDays(BeginDate, EndDate) {
    // nueva pantalla de edicion de fichajes
    var url = vtApplicationPath + 'Scheduler/MovesNew.aspx?action=notreliabledDays&DateStart=' + encodeURIComponent(BeginDate) + '&DateEnd=' + encodeURIComponent(EndDate);
    var Title = '';
    parent.ShowExternalForm2(url, 1400, 620, Title, '', false, false, false);
}

function GridDetailsClient_Click(s, e) {
    if (e.buttonID == "ShowEmployeeButton") {
        var notifType = parseInt($("#hdnNotifType").val(), 10);
        if (notifType == 54) {
            GridDetailsClient.GetRowValues(e.visibleIndex, "IDEmployee;Column3Detail", GridAISchedulerDetailsClient_ClickEmployee)
        } else {
            if (notifType >= 0) {
                GridDetailsClient.GetRowValues(e.visibleIndex, "IDEmployee", GridDetailsClient_ClickEmployee)
            } else if (parseInt($("#hdnDocAlertType").val(), 10) >= 0) {
                GridDetailsClient.GetRowValues(e.visibleIndex, "IdRelatedObject;IDDocument", GridDocumentDetailsClient_ClickEmployee)
            }
        }
    }
}

function GridAISchedulerDetailsClient_ClickEmployee(Value) {
    var notifType = $("#hdnNotifType").val();
    if (notifType == '54') {
        var url = $("#AISchedulerURI").val();

        localStorage.setItem('BudgetView', 5);

        localStorage.setItem('BudgetType', 1);

        eraseCookie("Budget_OrgChartFilter");
        createCookie("Budget_OrgChartFilter", Value[0]);

        var dateStart = moment(Value[1], "YYYY/MM/DD").format('YYYY#MM#DD');
        var dateEnd = moment(Value[1], "YYYY/MM/DD").format('YYYY#MM#DD');
        localStorage.setItem('BudgetIntervalDates', dateStart + ',' + dateEnd);

        localStorage.setItem('BudgetStateOptions', "false@@0@false");

        parent.window.open(url);
    } else {
        var url = $("#EmployeeURI").val();
        parent.window.open(url + "?IDEmployee=" + Value[0])
    }
}

function GridDetailsClient_ClickEmployee(Value) {
    let openEmployeePage = false;
    let openEmployeeTab = -1;
    let openIDEmployee = -1;
    let openURLPage = "";


    var notifType = $("#hdnNotifType").val();
    if (notifType == '44' || notifType == '43') {
        var url = $("#CalendarURI").val();
        parent.window.open(url + Value)
    } else if (notifType == '23' || notifType == '24' || notifType == '25' || notifType == '26' || notifType == '29' || notifType == '30' || notifType == '45' || notifType == '64') {
        var url = $("#TasksURI").val();
        parent.window.open(url + Value)
    } else if (notifType == '41') {
        openURLPage = $("#EmployeeURI").val();
        openIDEmployee = Value;
        openEmployeePage = true;
        openEmployeeTab = 7;

        //parent.window.open(url + "?IDEmployee=" + Value + "&TabEmployee=7")
    } else if (notifType == '68') {
        openURLPage = $("#EmployeeURI").val();
        openIDEmployee = Value;
        openEmployeePage = true;
        openEmployeeTab = 0;

        //parent.window.open(url + "?IDEmployee=" + Value + "&TabEmployee=0")
    } else if (notifType == '54') {
        var url = $("#AISchedulerURI").val();

        localStorage.setItem('BudgetView', 5);
        localStorage.setItem('BudgetType', 1);

        eraseCookie("Budget_OrgChartFilter");
        createCookie("Budget_OrgChartFilter", Value);

        var dateStart = moment().format('YYYY#MM#DD');
        var dateEnd = moment().format('YYYY#MM#DD');
        localStorage.setItem('BudgetIntervalDates', dateStart + ',' + dateEnd);

        localStorage.setItem('BudgetStateOptions', "false@@0@false");

        parent.window.open(url);
    } else if (notifType == '58' || notifType == '60' || notifType == '87') {
        var url = $("#SupervisorURI").val();
        if (notifType == '87') {
            parent.window.open(url)
        } else {
            parent.window.open(url + "?IDPassport=" + Value)
        }
    } else {
        openURLPage = $("#EmployeeURI").val();
        openIDEmployee = Value;
        openEmployeePage = true;

        //parent.window.open(url + "?IDEmployee=" + Value)
    }

    if (openEmployeePage) {
        $.ajax({
            url: `/Employee/GetEmployeeTreeSelectionPath/${openIDEmployee}`,
            data: {},
            type: "GET",
            dataType: "json",
            success: async (data) => {
                if (typeof data != 'string') {

                    let treeState = await getroTreeState("ctl00_contentMainBody_roTrees1", true);
                    await treeState.setRedirectData(data.EmployeePath, data.GroupSelectionPath);

                    parent.window.open(openURLPage + "?IDEmployee=" + openIDEmployee + (openEmployeeTab >= 0 ? "&TabEmployee=" + openEmployeeTab : "") )

                } else {
                    DevExpress.ui.notify(data, 'error', 2000);
                }
            },
            error: (error) => console.error(error),
        });
    }
}

function GridDocumentDetailsClient_ClickEmployee(values) {
    let openEmployeePage = false;
    let openEmployeeTab = -1;
    let openIDEmployee = values[0];
    let openURLPage = "";

    if (parseInt($("#hdnDocAlertType").val(), 10) == 3) {
        parent.window.open($("#AbsencesURI").val() + "?IDEmployee=" + values[0] + "&Status=all&DocumentsDelivered=0");
    } else {
        openURLPage = $("#EmployeeURI").val();
        if ($("#hdnDocType").val().toUpperCase() == "0") {
            openEmployeePage = true;
            let alertType = parseInt($("#hdnDocumentAlertType").val(), 10);
            switch (alertType) {
                case 0:
                case 2:
                    openEmployeeTab = 1;
                    break;
                case 1:
                case 4:
                case 6:
                    openEmployeeTab = 4;
                    break;
                default:
                    openEmployeeTab = 1;
                    break;
            }
        } else {
            parent.window.open($("#CompanyURI").val() + "?IDGroup=" + values[0] + "&TabEmployee=4")
        }
    }


    if (openEmployeePage) {
        $.ajax({
            url: `/Employee/GetEmployeeTreeSelectionPath/${openIDEmployee}`,
            data: {},
            type: "GET",
            dataType: "json",
            success: async (data) => {
                if (typeof data != 'string') {

                    let treeState = await getroTreeState("ctl00_contentMainBody_roTrees1", true);
                    await treeState.setRedirectData(data.EmployeePath, data.GroupSelectionPath);

                    parent.window.open(openURLPage + "?IDEmployee=" + openIDEmployee + (openEmployeeTab >= 0 ? "&TabEmployee=" + openEmployeeTab : ""))

                } else {
                    DevExpress.ui.notify(data, 'error', 2000);
                }
            },
            error: (error) => console.error(error),
        });
    }
}