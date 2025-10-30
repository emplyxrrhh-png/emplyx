function showLoadingGrid(loading) { parent.showLoader(loading); }

var tasksqueue = {
    ASPxCallbackPanelContenidoClient_EndCallBack: function (s, e) {
        parent.showLoader(false);
    },

    loadData: function () {
        gridCompleted.PerformCallback("REFRESH");
        GridRunning.PerformCallback("REFRESH");
        gridWaiting.PerformCallback("REFRESH");
    },
};

function grid_CustomButtonClick(s, e) {
    showLoadingGrid(true);
    if (e.buttonID == "SeeAttachFileButton") {
        if (e.visibleIndex > -1) {
            gridCompleted.GetRowValues(e.visibleIndex, 'TaskID;Action;ViewID;AnalyticType', SeeAttachment);
        } else {
            showLoadingGrid(false);
        }
    } else {
        showLoadingGrid(false);
    }
}

function SeeAttachment(Parameters) {
    if (Parameters[1] != "ANALYTICSTASK") {
        window.open("downloadFile.aspx?TaskID=" + Parameters[0] + "&Action=" + Parameters[1]);
    } else {
        var analyticView = parseInt(Parameters[3], 10);
        var viewID = parseInt(Parameters[2], 10);

        if (document.getElementById("ctl00_contentMainBody_IsSaas").value == "True") {
            url = document.getElementById("ctl00_contentMainBody_GeniusURI").value;
            //url += Parameters[0];
            //window.open(url);
            top.reenviaFrame(url, '', 'Genius', 'PortalReportsGenius', '/LoadView/' + Parameters[0]);
        }
        else {
            if (analyticView >= 0 && viewID > 0) {
                var url = "";
                switch (analyticView) {
                    case 0:
                        url = document.getElementById("ctl00_contentMainBody_ScheduleAnalyticURI").value;
                        break;
                    case 1:
                        url = document.getElementById("ctl00_contentMainBody_CostCenterAnalyticURI").value;
                        break;
                    case 2:
                        url = document.getElementById("ctl00_contentMainBody_AccessURI").value;
                        break;
                    case 3:
                        url = document.getElementById("ctl00_contentMainBody_TaskAnalyticURI").value;
                        break;
                }

                url += "?taskid=" + Parameters[0];
                window.open(url);
            } else {
                DevExpress.ui.dialog.alert(Globalize.formatMessage('roNotFound'), Globalize.formatMessage('roAlert'));
            }
        }
    }

    showLoadingGrid(false);
}