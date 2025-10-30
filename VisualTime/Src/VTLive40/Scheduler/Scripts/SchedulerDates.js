//=============================================================================================================
//Devuelve array con la fecha inferior y fecha superior en formato cadena con los valores iniciales por defecto
function SetIntervalDates() {
    try {
        var arFechas = null;

        var selectedDates = localStorage.getItem('SchedulerIntervalDates') || "";

        if (selectedDates == "") {
            arFechas = getPeriodWeek(0);
            SaveDatesInCookie(arFechas);
        }
        else {
            if (ReviewDates(selectedDates, "FromCookie") == -1) {
                var arTmpFechas2 = selectedDates.split(",");
                var arFechas = new Array();
                arFechas[0] = BuildFecha(arTmpFechas2[0]);
                arFechas[1] = BuildFecha(arTmpFechas2[1]);
            }
            else {
                arFechas = getPeriodWeek(0);
                SaveDatesInCookie(arFechas);
            }
        }

        arFechas[2] = arFechas[0];
        arFechas[3] = arFechas[1];
        arFechas[0] = objDate2strDate(arFechas[0]);
        arFechas[1] = objDate2strDate(arFechas[1]);

        return arFechas;
    }
    catch (e) {
        showError("SchedulerDates::SetIntervalDates", e);
    }
}

//=================================================================================================
//Devuelve array con la fecha inferior y fecha superior en formato cadena
//Requiere el tipo de periodo para calcular un periodo hacia atras, hacia adelante o sin movimiento
//Requiere las dos fechas del ntervalo en formato cadena
function GetPeriodOfDates(Mode, TypeSelection, strDateInf, strDateSup) {
    try {
        if (typeof (Mode) == "undefined" || Mode == null || Mode == "") Mode = "PRESENT";
        if (typeof (TypeSelection) == "undefined" || TypeSelection == null || TypeSelection == "") TypeSelection = "L";    //TypeSelection -> L M o S

        var arrNewDates = null;

        var objDateInf = strDateInf;
        var objDateSup = strDateSup;

        var numDias = GetNumDaysFromDates(objDateInf, objDateSup);
        if (numDias < 0) numDias = 0;

        arrNewDates = new Array();

        if (Mode.toUpperCase() == "NEXT") {
            switch (TypeSelection) {
                case 'L':
                    if (IsMonthComplete(objDateInf, objDateSup) == true) {
                        arrNewDates[0] = new Date(objDateInf.getFullYear(), objDateInf.getMonth() + 1, 1);
                        arrNewDates[1] = new Date(objDateInf.getFullYear(), objDateInf.getMonth() + 2, 0);
                    }
                    else {
                        arrNewDates[0] = new Date(objDateSup.getFullYear(), objDateSup.getMonth(), objDateSup.getDate() + 1);
                        arrNewDates[1] = new Date(arrNewDates[0].getFullYear(), arrNewDates[0].getMonth(), arrNewDates[0].getDate() + numDias);
                    }
                    break;

                case 'S':
                    arrNewDates = getPeriodWeek(1, objDateInf);
                    break;

                case 'M':
                    arrNewDates = getPeriodMonth(1, objDateInf);
                    break;
            }
        }
        else if (Mode.toUpperCase() == "PREVIOUS") {
            switch (TypeSelection) {
                case 'L':
                    if (IsMonthComplete(objDateInf, objDateSup) == true) {
                        arrNewDates[0] = new Date(objDateInf.getFullYear(), objDateInf.getMonth() - 1, 1);
                        arrNewDates[1] = new Date(objDateInf.getFullYear(), objDateInf.getMonth(), 0);
                    }
                    else {
                        arrNewDates[1] = new Date(objDateInf.getFullYear(), objDateInf.getMonth(), objDateInf.getDate() - 1);
                        arrNewDates[0] = new Date(arrNewDates[1].getFullYear(), arrNewDates[1].getMonth(), arrNewDates[1].getDate() - numDias);
                    }
                    break;

                case 'S':
                    arrNewDates = getPeriodWeek(-1, objDateInf);
                    break;

                case 'M':
                    arrNewDates = getPeriodMonth(-1, objDateInf);
                    break;
            }
        }
        else if (Mode.toUpperCase() == "PRESENT") {
            switch (TypeSelection) {
                case 'L':
                    arrNewDates[0] = objDateInf;
                    arrNewDates[1] = objDateSup;
                    break;

                case 'S':
                    arrNewDates = getPeriodWeek(0, objDateInf);
                    break;

                case 'M':
                    arrNewDates = getPeriodMonth(0, objDateInf);
                    break;
            }
        }

        //Guarda las nuevas fechas en la cookie
        SaveDatesInCookie(arrNewDates);

        arrNewDates[2] = arrNewDates[0];
        arrNewDates[3] = arrNewDates[1];
        arrNewDates[0] = objDate2strDate(arrNewDates[0]);
        arrNewDates[1] = objDate2strDate(arrNewDates[1]);

        return arrNewDates;
    }
    catch (e) {
        showError("SchedulerDates::GetPeriodOfDates", e);
    }
}

function IsMonthComplete(d1, d2) {
    if (d1.getFullYear() == d2.getFullYear() && d1.getMonth() == d2.getMonth()) {
        var numDias = daysInMonth(d2.getMonth(), d2.getFullYear());
        if (d1.getDate() == 1 && d2.getDate() == numDias) {
            return true;
        }
    }
    return false;
}

function daysInMonth(Month, Year) {
    var n = 32 - new Date(Year, Month, 32).getDate();
    return n;
}

function SaveDatesInCookie(arFechas) {
    var textocookie = FormateaFecha(arFechas[0]) + "," + FormateaFecha(arFechas[1]);
    localStorage.setItem('SchedulerIntervalDates', textocookie);
}

function SaveDatesInCookieEx(arFechas) {
    var textocookie = arFechas[0] + "," + arFechas[1];
    localStorage.setItem('SchedulerIntervalDates', textocookie);
}

function FormateaFecha(d) {
    var strFecha = "";
    strFecha = d.getFullYear() + "#" + (d.getMonth() + 1) + "#" + d.getDate();
    return strFecha;
}

function BuildFecha(strFecha) {
    var arFechaAux = strFecha.split("#");
    var d = new Date(arFechaAux[0], (arFechaAux[1] - 1), arFechaAux[2]);
    return d;
}

//Retorna -1 si todo OK
//Retorna 0: Falta alguna fecha
//Retorna 1: DateMode es incorrecto
//Retorna 2: Fecha inferior es igual o mayor que fecha superior
//Retorna 3: Numero de dias superior a 93
function ReviewDates(selectedDates, DateMode) {
    try {
        var arTmpFechas2 = selectedDates.split(",");
        if (arTmpFechas2[0] == "" || arTmpFechas2[1] == "") {
            return 0;
        }

        var arTmpFechas = new Array();
        if (DateMode.toUpperCase() == "FROMCOOKIE") {
            arTmpFechas[0] = BuildFecha(arTmpFechas2[0]);
            arTmpFechas[1] = BuildFecha(arTmpFechas2[1]);
        }
        else if (DateMode.toUpperCase() == "FROMSTRING") {
            arTmpFechas[0] = strDate2objDate(arTmpFechas2[0]);
            arTmpFechas[1] = strDate2objDate(arTmpFechas2[1]);
        }
        else {
            return 1;
        }

        if (arTmpFechas[0].getTime() > arTmpFechas[1].getTime()) {
            return 2;
        }

        var x = null;
        x = objDate2strDate(arTmpFechas[0]);
        x = objDate2strDate(arTmpFechas[1]);

        var dias = GetNumDaysFromDates(arTmpFechas[0], arTmpFechas[1]);
        if (dias > 93)
            return 3;
        else
            return -1;
    }
    catch (e) {
        return 0;
    }
}

//A partir de una fecha d devuelve las fechas del Lunes y la fecha del Domingo de su semana en un array
//Si mode = 0 -> la semana actual | Si mode = 0 -> la semana anterior | Si mode = 1 -> la semana siguiente
function getPeriodWeek(Mode, d) {
    if (typeof (Mode) == "undefined" || Mode == null || Mode == "") Mode = "0";

    var FirstDay;
    var LastDay;

    if (d == undefined)
        d = new Date();
    else
        d = new Date(d);

    var day = d.getDay();
    var diff = d.getDate() - day + (day == 0 ? -6 : 1);

    if (Mode == 0) { //Semana actual
        FirstDay = new Date(d.setDate(diff));
        LastDay = new Date(FirstDay.getFullYear(), FirstDay.getMonth(), FirstDay.getDate() + 6);
    }

    if (Mode == -1) { //Semana anterior
        FirstDay = new Date(d.setDate(diff - 7));
        LastDay = new Date(FirstDay.getFullYear(), FirstDay.getMonth(), FirstDay.getDate() + 6);
    }

    if (Mode == 1) { //Semana siguiente
        FirstDay = new Date(d.setDate(diff + 7));
        LastDay = new Date(FirstDay.getFullYear(), FirstDay.getMonth(), FirstDay.getDate() + 6);
    }

    var ar = [FirstDay, LastDay];

    return ar;
}

//A partir de una fecha devuelve la fecha del 1 y el 31 de su mes en un array
//Si mode = 0 -> el mes actual | Si mode = 0 -> el mes anterior | Si mode = 1 -> el mes siguiente
function getPeriodMonth(Mode, d) {
    if (typeof (Mode) == "undefined" || Mode == null || Mode == "") Mode = "0";

    var FirstDay;
    var LastDay;

    if (d == undefined)
        d = new Date();
    else
        d = new Date(d);

    if (Mode == 0) { //Mes actual
        FirstDay = new Date(d.getFullYear(), d.getMonth(), 1);
        LastDay = new Date(FirstDay.getFullYear(), FirstDay.getMonth() + 1, 0);
    }

    if (Mode == -1) { //Mes anterior
        FirstDay = new Date(d.getFullYear(), d.getMonth() - 1, 1);
        LastDay = new Date(FirstDay.getFullYear(), FirstDay.getMonth() + 1, 0);
    }

    if (Mode == 1) { //Mes siguiente
        FirstDay = new Date(d.getFullYear(), d.getMonth() + 1, 1);
        LastDay = new Date(FirstDay.getFullYear(), FirstDay.getMonth() + 1, 0);
    }

    var ar = [FirstDay, LastDay];

    return ar;
}

//Devuelve una fecha en formato cadena
//Requiere un objeto de tipo fecha
function objDate2strDate(dateValue) {
    var format = retDateFormatText();

    var fmt = format.toUpperCase();
    var re = /^(M|MM|D|DD|YYYY)([\-\/]{1})(M|MM|D|DD|YYYY)(\2)(M|MM|D|DD|YYYY)$/;
    if (!re.test(fmt)) { fmt = "MM/DD/YYYY"; }
    if (fmt.indexOf("M") == -1) { fmt = "MM/DD/YYYY"; }
    if (fmt.indexOf("D") == -1) { fmt = "MM/DD/YYYY"; }
    if (fmt.indexOf("YYYY") == -1) { fmt = "MM/DD/YYYY"; }

    var M = "" + (dateValue.getMonth() + 1);
    var MM = "0" + M;
    MM = MM.substring(MM.length - 2, MM.length);
    var D = "" + (dateValue.getDate());
    var DD = "0" + D;
    DD = DD.substring(DD.length - 2, DD.length);
    var YYYY = "" + (dateValue.getFullYear());

    var sep = "/";
    if (fmt.indexOf("-") != -1) { sep = "-"; }
    var pieces = fmt.split(sep);
    var result = "";

    switch (pieces[0]) {
        case "M": result += M + sep; break;
        case "MM": result += MM + sep; break;
        case "D": result += D + sep; break;
        case "DD": result += DD + sep; break;
        case "YYYY": result += YYYY + sep; break;
    }

    switch (pieces[1]) {
        case "M": result += M + sep; break;
        case "MM": result += MM + sep; break;
        case "D": result += D + sep; break;
        case "DD": result += DD + sep; break;
        case "YYYY": result += YYYY + sep; break;
    }

    switch (pieces[2]) {
        case "M": result += M; break;
        case "MM": result += MM; break;
        case "D": result += D; break;
        case "DD": result += DD; break;
        case "YYYY": result += YYYY; break;
    }

    return result;
}

//Devuelve un objeto fecha
//Requiere una fecha de tipo cadena
function strDate2objDate(sDate) {
    try {
        var retDate = null;

        var aDate = new Array();
        aDate = sDate.split('/');

        var rFormat = new String();
        rFormat = retDateFormat();

        var pos1 = rFormat.substring(0, 1);
        var pos2 = rFormat.substring(1, 2);
        var pos3 = rFormat.substring(2, 3);

        if (pos1 == "1" && pos2 == "0" && pos3 == "2") {
            var retDate = new Date(parseInt(aDate[2], 10), parseInt(aDate[0], 10) - 1, parseInt(aDate[1], 10), 0, 0, 0);
        }
        else if (pos1 == "0" && pos2 == "1" && pos3 == "2") {
            var retDate = new Date(parseInt(aDate[2], 10), parseInt(aDate[1], 10) - 1, parseInt(aDate[0], 10), 0, 0, 0);
        }

        return retDate;
    }
    catch (e) {
        showError("SchedulerDates:strDate2objDate", e);
    }
}

//Retorna el numero de dias entre dos fechas
function GetNumDaysFromDates(d1, d2) {
    var t1 = d1.getTime();
    var t2 = d2.getTime();
    var dias = parseInt((t2 - t1) / (24 * 3600 * 1000));
    return dias;
}

//Funcion utilizada en el retorno de AnnualView
//Devuelve array con la fecha inferior y fecha superior en formato cadena
//Requiere las fechas F1, F2, FView en formato cadena
//Requiere TypeSelection (L, M o S)
function ConstructPeriodOfDates(F1, F2, FView, TypeSelection) {
    try {
        if (typeof (TypeSelection) == "undefined" || TypeSelection == null || TypeSelection == "") TypeSelection = "L";    //TypeSelection -> L M o S

        var arrNewDates = new Array();

        var objDateInf = F1;
        var objDateSup = F2;
        var objDatetoView = strDate2objDate(FView);

        switch (TypeSelection) {
            case 'L':

                var numDias = GetNumDaysFromDates(objDateInf, objDateSup);
                if (numDias < 0) numDias = 0;

                var tmpDate = new Date(objDatetoView.getFullYear(), objDatetoView.getMonth(), objDatetoView.getDate() + numDias);

                arrNewDates[0] = objDatetoView;
                arrNewDates[1] = tmpDate;
                break;

            case 'S':
                arrNewDates = getPeriodWeek(0, objDatetoView);
                break;

            case 'M':
                arrNewDates = getPeriodMonth(0, objDatetoView);
                break;
        }

        arrNewDates[2] = arrNewDates[0];
        arrNewDates[3] = arrNewDates[1];
        arrNewDates[0] = objDate2strDate(arrNewDates[0]);
        arrNewDates[1] = objDate2strDate(arrNewDates[1]);

        return arrNewDates;
    }
    catch (e) {
        showError("SchedulerDates::GetPeriodOfDates", e);
    }
}