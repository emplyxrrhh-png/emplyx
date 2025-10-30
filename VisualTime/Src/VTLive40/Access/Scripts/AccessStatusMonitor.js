var Monitor_Seconds = 2000; //se expresa en milisegundos
var Monitor_timeOutAccess;

//var Monitor_Hours = 1;
//function SetHours() {
//    alert("SetHours");
//    var txtHours = document.getElementById("txtHours");
//    var hdnHours = document.getElementById("hdnHours");
//    if (txtHours != null && hdnHours != null) {
//        if (parseInt(txtHours.value) >= 1 && parseInt(txtHours.value) <= 24)
//            hdnHours.value = txtHours.value;
//        else {
//            txtHours.value = Monitor_Hours;
//            hdnHours.value = txtHours.value;
//        }
//    }
//}

//function SetSeconds() {
//    alert("SetSeconds");
//    var txtSeconds = document.getElementById("txtSeconds");
//    var hdnSeconds = document.getElementById("hdnSeconds");
//    if (txtSeconds != null && hdnSeconds != null) {
//        if (parseInt(txtSeconds.value) >= 0 && parseInt(txtSeconds.value) <= 1000)
//            hdnSeconds.value = txtSeconds.value;
//        else {
//            txtSeconds.value = Monitor_Seconds;
//            hdnSeconds.value = txtSeconds.value;
//        }

//        if (hdnSeconds.value == 0) {
//            InterruptorRecarga(false);
//        }
//        else {
//            IniMonitor();
//        }

//    }
//}

function IniMonitor() {
    clearTimeout(Monitor_timeOutAccess);
    cargaAccessStatusMonitor(true);
}

//retorna numero de seconds entre refrescos
//function GetTimeRefresh() {
//        var hdnSeconds = document.getElementById("hdnSeconds");
//        if (hdnSeconds == null)
//            return Monitor_Seconds;
//        else
//            if (hdnSeconds.value == 0)
//                hdnSeconds.value = Monitor_Seconds;
//            return parseInt(hdnSeconds.value);
//}

function cargaAccessStatusMonitor(activeTime) {
    try {
        var listZones = document.getElementById("hdnListZones");
        if (listZones == null)
            return;

        //        var valueHours = Monitor_Hours;
        //        var hdnHours = document.getElementById("hdnHours");
        //        if (hdnHours != null)
        //            valueHours = hdnHours.value;

        $.ajax({
            type: "POST",
            url: "srvAccessStatusMonitor.aspx",
            data: "action=getAccessStatusMonitor&IdZones=" + listZones.value, // + "&Hours=" + valueHours,
            success: function (msg) {
                var container = document.getElementById("divContent");
                if (container != null) {
                    container.innerHTML = msg;
                }
            }
        });

        if (activeTime == true) {
            //var sec = GetTimeRefresh();
            Monitor_timeOutAccess = setTimeout("cargaAccessStatusMonitor(true)", Monitor_Seconds);
        }
        else
            clearTimeout(Monitor_timeOutAccess);
    }
    catch (e) {
        showError("cargaAccessStatusMonitor", e);
    }
}

function InterruptorRecarga(activeTime) {
    if (activeTime == true) {
        var sec = GetTimeRefresh();
        Monitor_timeOutAccess = setTimeout("cargaAccessStatusMonitor(true)", Monitor_Seconds);
    }
    else
        clearTimeout(Monitor_timeOutAccess);
}

function StopMonitor() {
    clearTimeout(Monitor_timeOutAccess);
}