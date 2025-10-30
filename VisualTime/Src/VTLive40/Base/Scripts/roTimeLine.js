// Detect si el navegador es IE
var IE = document.all ? true : false

// Si NS -- configuracio de la captura d'events
if (!IE) document.captureEvents(Event.MOUSEMOVE)

// Afegim event onmousemove de tota la plana per omplir les variables globals
document.onmousemove = getMouseXY;

var tempX = 0
var tempY = 0

function getMouseXY(e) {
    try {
        if (IE) { 
            tempX = event.clientX + document.body.scrollLeft
            tempY = event.clientY + document.body.scrollTop
        } else {  
            tempX = e.pageX
            tempY = e.pageY
        }
        if (tempX < 0) { tempX = 0 }
        if (tempY < 0) { tempY = 0 }
        return true
    } catch (e) { showError("getMouseXY", e);return true; }
}

function showBubble(objPrefix, legend) {
    try {
        var divBubble = document.getElementById(objPrefix + '_Bubble');
        divBubble.innerHTML = legend;
        divBubble.style.top = tempY + "px";
        divBubble.style.left = tempX + "px";
        divBubble.style.display = '';
    } catch (e) { showError("showBubble", e); }
}

function hideBubble(objPrefix) {
    try {
        var divBubble = document.getElementById(objPrefix + '_Bubble');
        divBubble.style.top = 0 + "px";
        divBubble.style.left = 0 + "px";
        divBubble.style.display = 'none';
    } catch (e) { showError("hideBubble", e); }
}

// "Clase" roTimeLine (Linea de Temps)
// objPrefix: Prefix per el control
// jsonData: Array de dades per crear els layers
// -------------------------------------------------------------------------------------------------
function roTimeLine(objPrefix, threeDefBand, jsonData, func) {

    var oPrefix = objPrefix;

    var funct = func; //Funcio que s'executara al fer doble click en els layers
    if (func == null) { funct = "showTLLayer"; }
    
    var tlHeaders = "_TLHeaders"; //TD de les capceleres
    var tlScroll = "_divTLScroll"; //Div que conte scroll
    var tableTL = "_tableTL"; //Taula que conte les linies de temps
    var tlBubble = "_Bubble"; //Bombolla de Tooltip

    //Tipus de colors de les franjes
    var colorRed = "Red";  //Mandatory
    var colorBlue = "Blue"; //Working  
    var colorGreen = "Green"; //Break

    var MandatoryCreated = false;       //Capa OBLIGADAS creada
    var BreakCreated = false;           //Capa DESCANSO creada
    var WorkingCreated = false;         //Capa FLEXIBLES creada

    var DefineBands;
    if (threeDefBand == null) {
        DefineBands = false;
    } else {
        DefineBands = threeDefBand;
    }

//Carrega del layer desde objecte JSON
this.createJSONLayer = function(jsonLayer) {

}

//Creació de una linea de temps
    this.createLayer = function (color, Type, LayerID, ParentID, Begin, BeginDay, Finish, FinishDay, DateDiffMin, FloatingBeginUpTo, FloatingBeginUpToDay, FloatingFinishMinutes, MaxBreakTime, MaxTime, MaxTimeAction, MinTime, Target, Value, ValueDay, Action, MinBreakAction, MinBreakTime, NoPunchBreakTime, AllowModifyIniHour, AllowModifyShiftDuration, captionTitle, IsLocked, NotificationForUser, NotificationForSupervisor, NotificationForUserBeforeTime, NotificationForUserAfterTime, RealBegin, RealFinish) {
    try {
        //Definicio de les 3 bandes desde un inici
        if (DefineBands == true) {
            if (!WorkingCreated) {
                this.drawLayerTimeLine(document.getElementById(objPrefix + '_TitleWorking').value, "roLTWorking");
                WorkingCreated = true;
            }
            if (!MandatoryCreated) {
                this.drawLayerTimeLine(document.getElementById(objPrefix + '_TitleMandatory').value, "roLTMandatory");
                MandatoryCreated = true;
            }
            if (!BreakCreated) {
                this.drawLayerTimeLine(document.getElementById(objPrefix + '_TitleBreak').value, "roLTBreak");
                BreakCreated = true;
            }
        }

        if (Type.toUpperCase() == "ROLTMANDATORY") { //Layer OBLIGATORIAS
            if (!MandatoryCreated) {
                this.drawLayerTimeLine(document.getElementById(objPrefix + '_TitleMandatory').value, Type);
                MandatoryCreated = true;
            }
        } else if (Type.toUpperCase() == "ROLTBREAK") { //Layer DESCANSO
            if (!BreakCreated) {
                this.drawLayerTimeLine(document.getElementById(objPrefix + '_TitleBreak').value, Type);
                BreakCreated = true;
            }
        } else if (Type.toUpperCase() == "ROLTWORKING") { //Layer FLEXIBLE
            if (!WorkingCreated) {
                this.drawLayerTimeLine(document.getElementById(objPrefix + '_TitleWorking').value, Type);
                WorkingCreated = true;
            }
        } else {
            //Futur TimeZones
            if (document.getElementById(objPrefix + "_cellTL_" + Type) == null) {
                this.drawLayerTimeLine(captionTitle, Type);
            }
        }

        //Si estem creant una linia desde javascript, tindrem de recuperar manualment la diferencia en minuts
        this.drawTimeZone(color, Type, LayerID, ParentID, Begin, BeginDay, Finish, FinishDay, DateDiffMin, FloatingBeginUpTo, FloatingBeginUpToDay, FloatingFinishMinutes, MaxBreakTime, MaxTime, MaxTimeAction, MinTime, Target, Value, ValueDay, Action, MinBreakAction, MinBreakTime, NoPunchBreakTime, IsLocked, AllowModifyIniHour, AllowModifyShiftDuration, NotificationForUser, NotificationForSupervisor, NotificationForUserBeforeTime, NotificationForUserAfterTime, RealBegin, RealFinish);

    } catch (e) { showError("roTimeLine.CreateLayer", e); }
}

//Afegir propietats filtre a capa existent (Working = Flexible)
this.addWorkingLayerFilter = function (Type, LayerID, ParentID, Begin, BeginDay, Finish, FinishDay, DateDiffMin, FloatingBeginUpTo, FloatingBeginUpToDay, FloatingFinishMinutes, MaxBreakTime, MaxTime, MaxTimeAction, MinTime, Target, Value, ValueDay, Action, MinBreakAction, MinBreakTime, NoPunchBreakTime, AllowModifyIniHour, AllowModifyShiftDuration) {
    try {
        var oDivLayers = retTZLayers(objPrefix, "Working");
        var n = 0;

        for (n = 0; n < oDivLayers.length; n++) {
            if (oDivLayers[n].getAttribute("roLT_tlID") == ParentID) { //Es el layer
                oDivLayers[n].setAttribute("roLT_working_ParentID", ParentID);
                oDivLayers[n].setAttribute("roLT_working_ID", LayerID);
                oDivLayers[n].setAttribute("roLT_working_Begin", Begin);
                oDivLayers[n].setAttribute("roLT_working_BeginDay", BeginDay);
                oDivLayers[n].setAttribute("roLT_working_Finish", Finish);
                oDivLayers[n].setAttribute("roLT_working_FinishDay", FinishDay);
                oDivLayers[n].setAttribute("roLT_working_DateDiffMin", DateDiffMin);
                oDivLayers[n].setAttribute("roLT_working_FloatingBeginUpTo", FloatingBeginUpTo);
                oDivLayers[n].setAttribute("roLT_working_FloatingBeginUpToDay", FloatingBeginUpToDay);
                oDivLayers[n].setAttribute("roLT_working_FloatingFinishMinutes", FloatingFinishMinutes);
                oDivLayers[n].setAttribute("roLT_working_MaxBreakTime", MaxBreakTime);
                oDivLayers[n].setAttribute("roLT_working_MaxTime", MaxTime);
                oDivLayers[n].setAttribute("roLT_working_MaxTimeAction", MaxTimeAction);
                oDivLayers[n].setAttribute("roLT_working_MinTime", MinTime);
                oDivLayers[n].setAttribute("roLT_working_Target", Target);
                oDivLayers[n].setAttribute("roLT_working_Value", Value);
                oDivLayers[n].setAttribute("roLT_working_ValueDay", ValueDay);
                oDivLayers[n].setAttribute("roLT_working_Action", Action);
                oDivLayers[n].setAttribute("roLT_working_MinBreakAction", MinBreakAction);
                oDivLayers[n].setAttribute("roLT_working_MinBreakTime", MinBreakTime);
                oDivLayers[n].setAttribute("roLT_working_NoPunchBreakTime", NoPunchBreakTime);
                oDivLayers[n].setAttribute("roLT_working_AllowModIni", AllowModifyIniHour);
                oDivLayers[n].setAttribute("roLT_working_AllowModDuration", AllowModifyShiftDuration);
                //, MinBreakAction, MinBreakTime, NoPunchBreakTime
            } //end if 
        } //end for 

    } catch (e) { showError("roTimeLine.addWorkingLayerFilter", e); }
}

//Afegir propietats filtre a capa existent (Mandatory = Rigida)
this.addUnitLayerFilter = function(Type, LayerID, ParentID, Begin, BeginDay, Finish, FinishDay, DateDiffMin, FloatingBeginUpTo, FloatingBeginUpToDay, FloatingFinishMinutes, MaxBreakTime, MaxTime, MaxTimeAction, MinTime, Target, Value, ValueDay, Action, MinBreakAction, MinBreakTime, NoPunchBreakTime, AllowModifyIniHour, AllowModifyShiftDuration) {
try {
        var oDivLayers = retTZLayers(objPrefix, "Mandatory");
        var n = 0;

        for (n = 0; n < oDivLayers.length; n++) {
            if (oDivLayers[n].getAttribute("roLT_tlID") == ParentID) { //Es el layer
                oDivLayers[n].setAttribute("roLT_unit_" + Target + "_ParentID", ParentID);
                oDivLayers[n].setAttribute("roLT_unit_" + Target + "_ID", LayerID);
                oDivLayers[n].setAttribute("roLT_unit_" + Target + "_Begin", Begin);
                oDivLayers[n].setAttribute("roLT_unit_" + Target + "_BeginDay", BeginDay);
                oDivLayers[n].setAttribute("roLT_unit_" + Target + "_Finish", Finish);
                oDivLayers[n].setAttribute("roLT_unit_" + Target + "_FinishDay", FinishDay);
                oDivLayers[n].setAttribute("roLT_unit_" + Target + "_DateDiffMin", DateDiffMin);
                oDivLayers[n].setAttribute("roLT_unit_" + Target + "_FloatingBeginUpTo", FloatingBeginUpTo);
                oDivLayers[n].setAttribute("roLT_unit_" + Target + "_FloatingBeginUpToDay", FloatingBeginUpToDay);
                oDivLayers[n].setAttribute("roLT_unit_" + Target + "_FloatingFinishMinutes", FloatingFinishMinutes);
                oDivLayers[n].setAttribute("roLT_unit_" + Target + "_MaxBreakTime", MaxBreakTime);
                oDivLayers[n].setAttribute("roLT_unit_" + Target + "_MaxTime", MaxTime);
                oDivLayers[n].setAttribute("roLT_unit_" + Target + "_MaxTimeAction", MaxTimeAction);
                oDivLayers[n].setAttribute("roLT_unit_" + Target + "_MinTime", MinTime);
                oDivLayers[n].setAttribute("roLT_unit_" + Target + "_Value", Value);
                oDivLayers[n].setAttribute("roLT_unit_" + Target + "_ValueDay", ValueDay);
                oDivLayers[n].setAttribute("roLT_unit_" + Target + "_Action", Action);
                oDivLayers[n].setAttribute("roLT_unit_" + Target + "_MinBreakAction", MinBreakAction);
                oDivLayers[n].setAttribute("roLT_unit_" + Target + "_MinBreakTime", MinBreakTime);
                oDivLayers[n].setAttribute("roLT_unit_" + Target + "_NoPunchBreakTime", NoPunchBreakTime);
                oDivLayers[n].setAttribute("roLT_unit_" + Target + "_AllowModIni", AllowModifyIniHour);
                oDivLayers[n].setAttribute("roLT_unit_" + Target + "_AllowModDuration", AllowModifyShiftDuration);
                //MinBreakAction, MinBreakTime, NoPunchBreakTime
            } //end if 
        } //end for 

    } catch (e) { showError("roTimeLine.addUnitLayerFilter", e); }
}

//Afegir propietats filtre a capa existent (Break = Descans)
this.addPaidLayerFilter = function (Type, LayerID, ParentID, Begin, BeginDay, Finish, FinishDay, DateDiffMin, FloatingBeginUpTo, FloatingBeginUpToDay, FloatingFinishMinutes, MaxBreakTime, MaxTime, MaxTimeAction, MinTime, Target, Value, ValueDay, Action, MinBreakAction, MinBreakTime, NoPunchBreakTime, AllowModifyIniHour, AllowModifyShiftDuration) {
    try {
        var oDivLayers = retTZLayers(objPrefix, "Break");
        var n = 0;

        for (n = 0; n < oDivLayers.length; n++) {
            if (oDivLayers[n].getAttribute("roLT_tlID") == ParentID) { //Es el layer
                oDivLayers[n].setAttribute("roLT_paid_" + Target + "_ParentID", ParentID);
                oDivLayers[n].setAttribute("roLT_paid_" + Target + "_ID", LayerID);
                oDivLayers[n].setAttribute("roLT_paid_" + Target + "_Begin", Begin);
                oDivLayers[n].setAttribute("roLT_paid_" + Target + "_BeginDay", BeginDay);
                oDivLayers[n].setAttribute("roLT_paid_" + Target + "_Finish", Finish);
                oDivLayers[n].setAttribute("roLT_paid_" + Target + "_FinishDay", FinishDay);
                oDivLayers[n].setAttribute("roLT_paid_" + Target + "_DateDiffMin", DateDiffMin);
                oDivLayers[n].setAttribute("roLT_paid_" + Target + "_FloatingBeginUpTo", FloatingBeginUpTo);
                oDivLayers[n].setAttribute("roLT_paid_" + Target + "_FloatingBeginUpToDay", FloatingBeginUpToDay);
                oDivLayers[n].setAttribute("roLT_paid_" + Target + "_FloatingFinishMinutes", FloatingFinishMinutes);
                oDivLayers[n].setAttribute("roLT_paid_" + Target + "_MaxBreakTime", MaxBreakTime);
                oDivLayers[n].setAttribute("roLT_paid_" + Target + "_MaxTime", MaxTime);
                oDivLayers[n].setAttribute("roLT_paid_" + Target + "_MaxTimeAction", MaxTimeAction);
                oDivLayers[n].setAttribute("roLT_paid_" + Target + "_MinTime", MinTime);
                oDivLayers[n].setAttribute("roLT_paid_" + Target + "_Value", Value);
                oDivLayers[n].setAttribute("roLT_paid_" + Target + "_ValueDay", ValueDay);
                oDivLayers[n].setAttribute("roLT_paid_" + Target + "_Action", Action);
                oDivLayers[n].setAttribute("roLT_paid_" + Target + "_MinBreakAction", MinBreakAction);
                oDivLayers[n].setAttribute("roLT_paid_" + Target + "_MinBreakTime", MinBreakTime);
                oDivLayers[n].setAttribute("roLT_paid_" + Target + "_NoPunchBreakTime", NoPunchBreakTime);
                oDivLayers[n].setAttribute("roLT_paid_" + Target + "_AllowModIni", AllowModifyIniHour);
                oDivLayers[n].setAttribute("roLT_paid_" + Target + "_AllowModDuration", AllowModifyShiftDuration);
                //MinBreakAction, MinBreakTime, NoPunchBreakTime
            } //end if 
        } //end for 

    } catch (e) { showError("roTimeLine.addPaidLayerFilter", e); }
}

//Busquem el ID per les capes per tipus
this.retMaxIDLayer = function(Type) {
    try {
        var oDivLayers = retTZLayers(objPrefix, Type.replace("roLT", ""));
        var n = 0;

        var oTypeId = "";
        var oMaxID = 0;

        var compositeID = false;

        for (n = 0; n < oDivLayers.length; n++) {
            var oLayerID = parseInt(oDivLayers[n].getAttribute("roLT_tlID"));
            if (isNaN(oLayerID)) {
                compositeID = true;
                oTypeId = oLayerID.split("_")[0];
                if (parseInt(oLayerID.split("_")[1]) > parseInt(oMaxID)) {
                    oTypeId = parseInt(oLayerID.split("_")[0]);
                    oMaxID = parseInt(oLayerID.split("_")[1]);
                }
            } else {
                if (oLayerID > oMaxID) { //Es el layer
                    oMaxID = oLayerID;
                } //end if
            } //end if 
        }
        oMaxID = parseInt(oMaxID) + 1;

        if (Type.toUpperCase().startsWith("HOURZONE")) {
            oMaxID = Type.replace("HourZone", "") + "_" + oMaxID;
        }
        
        return oMaxID;
    } catch (e) { showError("roTimeLine.retMaxIDLayer", e); return -1; }
}

// Creació de les timezones (javascript only)
this.createTimeZone = function(Type, oLayerAtt) {
    try {
        var cellZone = document.getElementById(objPrefix + "_cellTL_" + Type);
        var divNewZone = document.createElement("DIV");

        //Busquem el ID mes alt per poder afegir el nou registe
        var oIDMax = this.retMaxIDLayer(Type);

        switch (Type.toUpperCase()) {
            case "ROLTMANDATORY": color = colorRed; break;
            case "ROLTWORKING": color = colorBlue; break;
            case "ROLTBREAK": color = colorGreen; break;
            default: color = colorBlue; break;
        }

        for (var x = 0; x < oLayerAtt.length; x++) {

            switch (oLayerAtt[x].attname.toLowerCase()) {
                case "rolt_tlid": LayerID = oIDMax; break;
            } //end switch

            if (oLayerAtt[x].attname.toLowerCase() == "rolt_tlid") {
                divNewZone.setAttribute(oLayerAtt[x].attname, oIDMax);
            } else {
                divNewZone.setAttribute(oLayerAtt[x].attname, oLayerAtt[x].value);
            } //end if

        } //end for

        divNewZone.setAttribute("roLT_tlType", Type);

        if (cellZone != null) {
            cellZone.appendChild(divNewZone);
        } else {
            this.drawLayerTimeLine(divNewZone.getAttribute("roLT_title"), Type);
            var cellZoneDef = document.getElementById(objPrefix + "_cellTL_" + Type);
            cellZoneDef.appendChild(divNewZone);
        }

        this.redrawTimeZone(Type, divNewZone);

    } catch (e) { showError("roTimeLine.createTimeZone", e); }
}

//Actualització de les timezones
this.updateTimeZone = function(Type, IDLayer, oLayerAtt) {
    try {
        var oDivLayers = retTZLayers(objPrefix, Type.replace("roLT",""));
        var n = 0;

        for (n = 0; n < oDivLayers.length; n++) {
            if (oDivLayers[n].getAttribute("roLT_tlID") == IDLayer) { //Es el layer
                for (var x = 0; x < oLayerAtt.length; x++) {
                   oDivLayers[n].setAttribute(oLayerAtt[x].attname, oLayerAtt[x].value);
                }
                this.redrawTimeZone(Type, oDivLayers[n]);
            } //end if
        } //end for 

    } catch (e) { showError("roTimeLine.updateLayer", e); }
}

//Redibuixa / redimensiona el TimeZone
this.redrawTimeZone = function(Type, oDivLayer) {
    try {
        //Variables
        var color, Type, LayerID, ParentID, Begin, BeginDay, Finish, FinishDay, RealBegin, RealFinish, DateDiffMin, FloatingBeginUpTo, FloatingBeginUpToDay, FloatingFinishMinutes, MaxBreakTime, MaxTime, MaxTimeAction, MinTime, Target, Value, ValueDay, Action, MinBreakAction, MinBreakTime, NoPunchBreakTime, AllowModifyIniHour, AllowModifyShiftDuration;

        oDivLayer.innerHTML = ""; //Blanqueja bloc

        Type = oDivLayer.getAttribute("roLT_tlType");
        LayerID = oDivLayer.getAttribute("roLT_tlID");
        Begin = oDivLayer.getAttribute("roLT_Begin");
        RealBegin = oDivLayer.getAttribute("roLT_RealBegin");
        BeginDay = oDivLayer.getAttribute("roLT_BeginDay");
        Finish = oDivLayer.getAttribute("roLT_Finish");
        RealFinish = oDivLayer.getAttribute("roLT_RealFinish");
        FinishDay = oDivLayer.getAttribute("roLT_FinishDay");
        DateDiffMin = oDivLayer.getAttribute("roLT_DateDiffMin");
        ParentID = oDivLayer.getAttribute("roLT_ParentID");
        FloatingBeginUpTo = oDivLayer.getAttribute("roLT_FloatingBeginUpTo");
        FloatingBeginUpToDay = oDivLayer.getAttribute("roLT_FloatingBeginUpToDay");
        FloatingFinishMinutes = oDivLayer.getAttribute("roLT_FloatingFinishMinutes");
        MaxBreakTime = oDivLayer.getAttribute("roLT_MaxBreakTime");
        MaxTime = oDivLayer.getAttribute("roLT_MaxTime");
        MaxTimeAction = oDivLayer.getAttribute("roLT_MaxTimeAction");
        MinTime = oDivLayer.getAttribute("roLT_MinTime");
        Value = oDivLayer.getAttribute("roLT_Value");
        ValueDay = oDivLayer.getAttribute("roLT_ValueDay");
        Target = oDivLayer.getAttribute("roLT_Target");
        Action = oDivLayer.getAttribute("roLT_Action");
        MinBreakAction = oDivLayer.getAttribute("roLT_MinBreakAction");
        MinBreakTime = oDivLayer.getAttribute("roLT_MinBreakTime");
        NoPunchBreakTime = oDivLayer.getAttribute("roLT_NoPunchBreakTime");
        AllowModifyIniHour = oDivLayer.getAttribute("roLT_AllowModIni");
        AllowModifyShiftDuration = oDivLayer.getAttribute("roLT_AllowModDuration");

        switch (Type.toUpperCase()) {
            case "ROLTMANDATORY":
                color = colorRed;
                break;
            case "ROLTWORKING":
                color = colorBlue;
                break;
            case "ROLTBREAK":
                color = colorGreen;
                break;
            default:
                color = colorBlue;
                break;
        }

        if (RealBegin != null && RealBegin != "") {
            Begin = RealBegin;
        }
        if (RealFinish != null && RealFinish != "") {
            Finish = RealFinish;
        }

        var RFinish = Finish;

        //Si no be calculat els minuts (javascript, no server), els calculem
        if (DateDiffMin == 0) {
            if (FloatingFinishMinutes != "" && FloatingFinishMinutes != null) {
                var FinishMinutes = parseInt(FloatingFinishMinutes);

                if (FloatingBeginUpTo == "" || FloatingBeginUpTo == null) {
                    FinishMinutes = FinishMinutes + parseInt(this.roTimeLine_ConvertHoursToMinutes(Begin));
                }
                else {
                    FinishMinutes = FinishMinutes + parseInt(this.roTimeLine_ConvertHoursToMinutes(FloatingBeginUpTo));
                }
                if (FinishMinutes >= 1440)
                    FinishMinutes = FinishMinutes - 1440;

                RFinish = this.roTimeLine_ConvertMinutesToHour(FinishMinutes);
            }
            DateDiffMin = this.DifferenceMinutes(Begin, BeginDay, RFinish, FinishDay);
        }

        var cellZone = document.getElementById(objPrefix + "_cellTL_" + Type);
        var TZPosition = 0; //Posicio del TimeZone

        
        if (BeginDay == "3") {
            TZPosition = (19 * 24) * parseInt(1); //Dia actual del horario
        }
        else {
            TZPosition = (19 * 24) * parseInt(BeginDay); //Dia actual del horario
        }
        

        var HourStr = Begin.split(":")[0];
        if (HourStr.substr(0, 1) == "0") HourStr = HourStr.substr(1, 1);
        var hour = parseInt(HourStr); //parseInt(Begin.split(":")[0]);
        var MinuteStr = Begin.split(":")[1];
        if (MinuteStr.substr(0, 1) == "0") MinuteStr = MinuteStr.substr(1, 1);
        var minute = parseInt(MinuteStr);  //parseInt(Begin.split(":")[1]);

        TZPosition = TZPosition + (hour * 19); //Sumem les hores

        if (minute != 0 && minute > 3) { //Si els minuts son diferents de 0, sumem
            TZPosition = TZPosition + Math.round(minute / 3.1);
        }

        //Creem el anchor per mostrar el tooltip i reenviar a la funcio corresponent
        var aTL = document.createElement("A");
        aTL.href = "javascript: void(0);";
        aTL.style.display = "block";
        aTL.style.cursor = "pointer"; //Aixo es per IE, al contenir una taula
        aTL.setAttribute("objPrefix", objPrefix);

        //error al pintar la franja var oLegend = Begin + " - " + RealFinish; //original
        var oLegend = Begin + " - " + Finish;

        if (Type.toUpperCase() == "ROLTBREAK") {
            var MaxMin = parseInt((MaxBreakTime.split(":")[0] * 60)) + parseInt(MaxBreakTime.split(":")[1]);
            if (isNaN(MaxMin)) {
                oLegend = DateDiffMin + "´";
            } else {
                oLegend = MaxMin + "´";
            }
        }

        aTL.setAttribute("legend", oLegend);
        aTL.setAttribute("name", objPrefix + "_bandzone");

        var wBar = 1; //Calculem el tamany de la barra
        if (DateDiffMin > 3) { wBar = Math.round(parseInt(DateDiffMin) / 3.157); }

        var func_Dbl;
        var divZType = "TimeZone";
        switch (Type.toUpperCase()) {
            case "ROLTMANDATORY": //Rigidas
                func_Dbl = "roTimeLine_Show('" + objPrefix + "','Mandatory','" + LayerID + "');";
                divZType = "Mandatory";
                break;
            case "ROLTBREAK": //Descanso
                func_Dbl = "roTimeLine_Show('" + objPrefix + "','Break','" + LayerID + "');";
                divZType = "Break";
                break;
            case "ROLTWORKING": //Flexible
                func_Dbl = "roTimeLine_Show('" + objPrefix + "','Working','" + LayerID + "');";
                divZType = "Working";
                break;
            default: //Otras capas (zonas horarias)
                func_Dbl = "roTimeLine_Show('" + objPrefix + "','" + Type + "','" + LayerID + "');";
                divZType = Type;
                break;
        }

        oDivLayer.setAttribute("name", objPrefix + "_rtdiv_" + divZType)
        aTL.setAttribute("vdblclick", func_Dbl);
        aTL.setAttribute("objPrefix", objPrefix);
        aTL.setAttribute("atltype", divZType);
        aTL.setAttribute("atlid", LayerID);

        if (window.addEventListener) { // Mozilla, Netscape, Firefox
            aTL.setAttribute("onmouseover", "showBubble('" + objPrefix + "','" + oLegend + "');");
            aTL.setAttribute("onmouseout", "hideBubble('" + objPrefix + "');");
            aTL.setAttribute("onclick", "selectedTL('" + objPrefix + "',this);");
            aTL.setAttribute("ondblclick", func_Dbl);
        } else { // IE
            aTL.onmouseover = function() { showBubble(this.getAttribute("objPrefix"), this.getAttribute("legend")); }
            aTL.onmouseout = function() { hideBubble(this.getAttribute("objPrefix")); }
            aTL.onclick = function() { selectedTL(this.getAttribute("objPrefix"), this); }
            //aTL.ondblclick = function() { roTimeLineShow(this.getAttribute("objPrefix"), this.getAttribute("atltype"), this.getAttribute("atlid")); }
        }

        var visibleLegend = false;
        var visibleRounds = false;
        var longFormat = false;

        if (parseInt(DateDiffMin) > 42) { //Si es mes gran q 42 minuts, treurem els bordes redondejats, i restem 14px
            visibleRounds = true;
        }

        if (parseInt(DateDiffMin) > 72) { //Si es mes gran que 72 minuts, ens cap per mostrar la llegenda dins
            visibleLegend = true;
        }

        if (parseInt(DateDiffMin) > 258) { //Si es mes gran q 258 minuts, podem mostrar complert
            longFormat = true;
        }

        aTL.style.width = wBar + "px"; //Amplada de la barra de temps

        var oTable = document.createElement("TABLE");
        oTable.setAttribute("border", "0");
        oTable.setAttribute("cellPadding", "0");
        oTable.setAttribute("cellSpacing", "0");

        var oTR = oTable.insertRow(-1);

        var oTD = oTR.insertCell(-1); //left
        if (visibleRounds) { oTD.className = "bg" + color + "-left"; }

        var oTD = oTR.insertCell(-1); //center
        oTD.className = "bg" + color + " bgTLBody";

        if (visibleRounds) {
            wBar = wBar - 14;
        }

        oTD.style.width = wBar + "px"; //tamany
        oTD.innerHTML = "&nbsp;";

        if (visibleLegend) { //Si cap una llegenda
            if (longFormat) { //Format d'hora llarg
                oTD.innerHTML = oLegend;
            } else {
                if (Type.toUpperCase() != "ROLTBREAK") {
                    oTD.innerHTML = DateDiffMin + "'";
                } else {
                    oTD.innerHTML = oLegend;
                }

            }
        }

        aTL.appendChild(oTable); //afegim el cos

        var oTD = oTR.insertCell(-1); //right
        if (visibleRounds) { oTD.className = "bg" + color + "-right"; }

        oDivLayer.appendChild(aTL);

        //Posicionem el layer
//        oDivLayer.style.marginLeft = TZPosition + "px";
//        oDivLayer.style.position = "relative";
//        oDivLayer.style.top = 2 + "px";
//        if (cellZone.childNodes.length == 1) {
//            oDivLayer.style.cssFloat = "left";
//            oDivLayer.style.styleFloat = "left";
//        }

        //PPR 10/02/2012
        //Posicionar layer
        oDivLayer.style.marginLeft = TZPosition + "px";
        oDivLayer.style.position = "absolute";

        switch (Type.toUpperCase()) {
            case "ROLTMANDATORY": //Rigidas
                oDivLayer.style.top = 55 + "px";
                break;
            case "ROLTBREAK": //Descanso
                oDivLayer.style.top = 100 + "px";
                break;
            case "ROLTWORKING": //Flexible
                oDivLayer.style.top = 10 + "px";
                break;
            default: //Otras capas (zonas horarias)
                break;
        }
        //FIN PPR 

    } catch (e) { showError("roTimeLine.redrawTimeZone", e); }
}


//Recuperació de Layers
retTZLayers = function(objPrefix, TypeZ) {
    try {
        var oDivLayers = retElementsByName(objPrefix + "_rtdiv_" + TypeZ);
        return oDivLayers;
    } catch (e) { showError("roTimeLine.reTZLayers", e); }
}

//Draw TimeZone
    this.drawTimeZone = function (color, Type, LayerID, ParentID, Begin, BeginDay, Finish, FinishDay, DateDiffMin, FloatingBeginUpTo, FloatingBeginUpToDay, FloatingFinishMinutes, MaxBreakTime, MaxTime, MaxTimeAction, MinTime, Target, Value, ValueDay, Action, MinBreakAction, MinBreakTime, NoPunchBreakTime, IsLocked, AllowModifyIniHour, AllowModifyShiftDuration, NotificationForUser, NotificationForSupervisor, NotificationForUserBeforeTime, NotificationForUserAfterTime, RealBegin, RealFinish) {
    try {
        var cellZone = document.getElementById(objPrefix + "_cellTL_" + Type);
        var divNewZone = document.createElement("DIV");

        divNewZone.setAttribute("lt_func", funct);

        divNewZone.setAttribute("roLT_tlType", Type);
        divNewZone.setAttribute("roLT_tlID", LayerID);
        divNewZone.setAttribute("roLT_Begin", Begin);
        divNewZone.setAttribute("roLT_BeginDay", BeginDay);
        divNewZone.setAttribute("roLT_Finish", Finish);
        divNewZone.setAttribute("roLT_FinishDay", FinishDay);
        divNewZone.setAttribute("roLT_DateDiffMin", DateDiffMin);

        if (ParentID != null) { divNewZone.setAttribute("roLT_ParentID", ParentID); }
        if (FloatingBeginUpTo != null) { divNewZone.setAttribute("roLT_FloatingBeginUpTo", FloatingBeginUpTo); }
        if (FloatingBeginUpToDay != null) { divNewZone.setAttribute("roLT_FloatingBeginUpToDay", FloatingBeginUpToDay); }
        if (FloatingFinishMinutes != null) { divNewZone.setAttribute("roLT_FloatingFinishMinutes", FloatingFinishMinutes); }
        if (MaxBreakTime != null) { divNewZone.setAttribute("roLT_MaxBreakTime", MaxBreakTime); }
        if (MaxTime != null) { divNewZone.setAttribute("roLT_MaxTime", MaxTime); }
        if (MaxTimeAction != null) { divNewZone.setAttribute("roLT_MaxTimeAction", MaxTimeAction); }
        if (MinTime != null) { divNewZone.setAttribute("roLT_MinTime", MinTime); }
        if (Value != null) { divNewZone.setAttribute("roLT_Value", Value); }
        if (ValueDay != null) { divNewZone.setAttribute("roLT_ValueDay", ValueDay); }
        if (Target != null) { divNewZone.setAttribute("roLT_Target", Target); }
        if (Action != null) { divNewZone.setAttribute("roLT_Action", Action); }
        if (MinBreakAction != null) { divNewZone.setAttribute("roLT_MinBreakAction", MinBreakAction); }
        if (MinBreakTime != null) { divNewZone.setAttribute("roLT_MinBreakTime", MinBreakTime); }
        if (NoPunchBreakTime != null) { divNewZone.setAttribute("roLT_NoPunchBreakTime", NoPunchBreakTime); }
        if (IsLocked != null) { divNewZone.setAttribute("roLT_IsLocked", IsLocked); }
        if (AllowModifyIniHour != null) { divNewZone.setAttribute("roLT_AllowModIni", AllowModifyIniHour); }
        if (AllowModifyShiftDuration != null) { divNewZone.setAttribute("roLT_AllowModDuration", AllowModifyShiftDuration); }
        if (NotificationForUser != null) { divNewZone.setAttribute("roLT_NotificationForUser", NotificationForUser); }
        if (NotificationForSupervisor != null) { divNewZone.setAttribute("roLT_NotificationForSupervisor", NotificationForSupervisor); }
        if (NotificationForUserBeforeTime != null) { divNewZone.setAttribute("roLT_NotificationForUserBeforeTime", NotificationForUserBeforeTime); }
        if (NotificationForUserAfterTime != null) { divNewZone.setAttribute("roLT_NotificationForUserAfterTime", NotificationForUserAfterTime); }
        if (NotificationForUserAfterTime != null) { divNewZone.setAttribute("roLT_NotificationForUserAfterTime", NotificationForUserAfterTime); }
        if (RealBegin != null && RealBegin != "") {
            Begin = RealBegin;
            divNewZone.setAttribute("roLT_RealBegin", RealBegin);
        }
        if (RealFinish != null && RealFinish != "") {
            Finish = RealFinish;
            divNewZone.setAttribute("roLT_RealFinish", RealFinish);
        }
        

        var TZPosition = 0; //Posicio del TimeZone

        if (BeginDay == "3") {

            TZPosition = (19 * 24) * parseInt(1); //Dia actual del horario
        }
        else {
            TZPosition = (19 * 24) * parseInt(BeginDay); //Dia actual del horario
        }

        var HourStr = Begin.split(":")[0]; if (HourStr.substr(0, 1) == "0") HourStr = HourStr.substr(1, 1);
        var hour = parseInt(HourStr); //parseInt(Begin.split(":")[0]);
        var MinuteStr = Begin.split(":")[1]; if (MinuteStr.substr(0, 1) == "0") MinuteStr = MinuteStr.substr(1, 1);
        var minute = parseInt(MinuteStr);  //parseInt(Begin.split(":")[1]);

        TZPosition = TZPosition + (hour * 19); //Sumem les hores

        if (minute != 0 && minute > 3) { //Si els minuts son diferents de 0, sumem
            TZPosition = TZPosition + Math.round(minute / 3.1);
        }

        //Creem el anchor per mostrar el tooltip i reenviar a la funcio corresponent
        var aTL = document.createElement("A");
        aTL.href = "javascript: void(0);";
        aTL.style.display = "block";
        aTL.style.cursor = "pointer"; //Aixo es per IE, al contenir una taula
        aTL.setAttribute("objPrefix", objPrefix);

        var oLegend = Begin + " - " + Finish;
        if (Type.toUpperCase() == "ROLTBREAK") {
            var MaxMin = parseInt((parseInt(MaxBreakTime.split(":")[0]) * 60)) + parseInt(parseInt(MaxBreakTime.split(":")[1]));
            if (isNaN(MaxMin)) {
                oLegend = DateDiffMin + "´";
            } else {
                oLegend = MaxMin + "´";
            }
        }

        aTL.setAttribute("legend", oLegend);
        aTL.setAttribute("name", objPrefix + "_bandzone");

        var wBar = 1; //Calculem el tamany de la barra
        if (parseInt(DateDiffMin) > 3) { wBar = Math.round(parseInt(DateDiffMin) / 3.157); }

        var func_Dbl;
        var divZType = "TimeZone";
        switch (Type.toUpperCase()) {
            case "ROLTMANDATORY": //Rigidas
                func_Dbl = "roTimeLine_Show('" + objPrefix + "','Mandatory','" + LayerID + "');"
                divZType = "Mandatory";
                break;
            case "ROLTBREAK": //Descanso
                func_Dbl = "roTimeLine_Show('" + objPrefix + "','Break','" + LayerID + "');"
                divZType = "Break";
                break;
            case "ROLTWORKING": //Flexible
                func_Dbl = "roTimeLine_Show('" + objPrefix + "','Working','" + LayerID + "');"
                divZType = "Working";
                break;
            default:  //Otros TimeZones
                func_Dbl = "roTimeLine_Show('" + objPrefix + "','" + Type + "','" + LayerID + "');"
                divZType = Type;
                break;
        }

        divNewZone.setAttribute("name", objPrefix + "_rtdiv_" + divZType)
        aTL.setAttribute("vdblclick", func_Dbl);
        aTL.setAttribute("objPrefix", objPrefix);
        aTL.setAttribute("atltype", divZType);
        aTL.setAttribute("atlid", LayerID);

        if (window.addEventListener) { // Mozilla, Netscape, Firefox
            aTL.setAttribute("onmouseover", "showBubble('" + objPrefix + "','" + oLegend + "');");
            aTL.setAttribute("onmouseout", "hideBubble('" + objPrefix + "');");
            aTL.setAttribute("onclick", "selectedTL('" + objPrefix + "',this);");
            aTL.setAttribute("ondblclick", func_Dbl);
        } else { // IE
            aTL.onmouseover = function() { showBubble(this.getAttribute("objPrefix"), this.getAttribute("legend")); }
            aTL.onmouseout = function() { hideBubble(this.getAttribute("objPrefix")); }
            aTL.onclick = function() { selectedTL(this.getAttribute("objPrefix"), this); }
            //aTL.ondblclick = function() { roTimeLineShow(this.getAttribute("objPrefix"), this.getAttribute("atltype"), this.getAttribute("atlid")); }
        }

        var visibleLegend = false;
        var visibleRounds = false;
        var longFormat = false;

        if (parseInt(DateDiffMin) > 42) { //Si es mes gran q 42 minuts, treurem els bordes redondejats, i restem 14px
            visibleRounds = true;
        }

        if (parseInt(DateDiffMin) > 72) { //Si es mes gran que 72 minuts, ens cap per mostrar la llegenda dins
            visibleLegend = true;
        }

        if (parseInt(DateDiffMin) > 258) { //Si es mes gran q 258 minuts, podem mostrar complert
            longFormat = true;
        }

        aTL.style.width = wBar + "px"; //Amplada de la barra de temps

        var oTable = document.createElement("TABLE");
        oTable.setAttribute("border", "0");
        oTable.setAttribute("cellPadding", "0");
        oTable.setAttribute("cellSpacing", "0");

        var oTR = oTable.insertRow(-1);

        var oTD = oTR.insertCell(-1); //left
        if (visibleRounds) { oTD.className = "bg" + color + "-left"; }

        var oTD = oTR.insertCell(-1); //center
        oTD.className = "bg" + color + " bgTLBody";

        if (visibleRounds) {
            wBar = wBar - 14;
        }

        oTD.style.width = wBar + "px"; //tamany
        oTD.innerHTML = "&nbsp;";

        if (visibleLegend) { //Si cap una llegenda
            if (longFormat) { //Format d'hora llarg
                oTD.innerHTML = oLegend;
            } else {
                if (Type.toUpperCase() != "ROLTBREAK") {
                    oTD.innerHTML = DateDiffMin + "'";
                } else {
                    oTD.innerHTML = oLegend;
                }

            }
        }

        aTL.appendChild(oTable); //afegim el cos

        var oTD = oTR.insertCell(-1); //right
        if (visibleRounds) { oTD.className = "bg" + color + "-right"; }

        divNewZone.appendChild(aTL);

        //Recuperem la celda on es te de posicionar el layer
        var tCell = document.getElementById(objPrefix + "_cellTL_" + Type);
        tCell.appendChild(divNewZone);

        //Posicionem el layer
//        divNewZone.style.marginLeft = TZPosition + "px";
//        divNewZone.style.position = "relative";
//        divNewZone.style.top = 2 + "px";
//        if (cellZone.childNodes.length == 1) {
//            divNewZone.style.cssFloat = "left";
//            divNewZone.style.styleFloat = "left";
//        }

        //PPR 10/02/2012
        //Posicionar layer
        divNewZone.style.marginLeft = TZPosition + "px";
        divNewZone.style.position = "absolute";

        switch (Type.toUpperCase()) {
            case "ROLTMANDATORY": //Rigidas
                divNewZone.style.top = 55 + "px";
                break;
            case "ROLTBREAK": //Descanso
                divNewZone.style.top = 100 + "px";
                break;
            case "ROLTWORKING": //Flexible
                divNewZone.style.top = 10 + "px";
                break;
            default: //Otras capas (zonas horarias)
                break;
        }
        //FIN PPR 



    } catch (e) { showError("drawTimeZone", e); }
}

//DrawLayer TimeLine
this.drawLayerTimeLine = function(caption, Type) {
    try {
        //Creem la capcelera del timeline layer
        var tdHeaders = document.getElementById(objPrefix + tlHeaders);
        var divHeader = document.createElement("DIV");
        divHeader.className = "tlHeaderClass";
        divHeader.innerHTML = caption;

        tdHeaders.appendChild(divHeader);

        //Creem el timeline layer
        var tblTimeLine = document.getElementById(objPrefix + tableTL);

        var tRow = tblTimeLine.insertRow(tblTimeLine.rows.length - 2);
        var tCell = tRow.insertCell(-1);
        tCell.id = objPrefix + "_cellTL_" + Type;
        tCell.setAttribute("cellTL", Type);
        tCell.className = "bgTimeLine";
        tCell.setAttribute("valign", "top");
        tCell.setAttribute("tmType", Type.toLowerCase());
        tCell.style.display = "block";

    } catch (e) { showError("drawLayerTimeLine", e); }
}

//Eliminació dels Layers existents
this.clearLayers = function() {
    try {
        var tHeaders = document.getElementById(objPrefix + tlHeaders);
        tHeaders.innerHTML = ''; //Eliminació de les capceleres DIVs

        var tTL = document.getElementById(objPrefix + tableTL);
        var totalRows = tTL.rows.length - 2;

        for (var x = 0; x < totalRows; x++) {
            tTL.deleteRow(0);
        }

        MandatoryCreated = false;
        BreakCreated = false;
        WorkingCreated = false;

        //Definicio de les 3 bandes desde un inici
        if (DefineBands == true) {
            if (!WorkingCreated) {
                this.drawLayerTimeLine(document.getElementById(objPrefix + '_TitleWorking').value, "roLTWorking");
                WorkingCreated = true;
            }
            if (!MandatoryCreated) {
                this.drawLayerTimeLine(document.getElementById(objPrefix + '_TitleMandatory').value, "roLTMandatory");
                MandatoryCreated = true;
            }
            if (!BreakCreated) {
                this.drawLayerTimeLine(document.getElementById(objPrefix + '_TitleBreak').value, "roLTBreak");
                BreakCreated = true;
            }
        }
    } catch (e) { showError("roTimeLine.clearLayers", e); }
}

// Funcio que retorna la diferencia en minuts (per calcular el tamany de la barra)
this.DifferenceMinutes = function(Begin, BeginDay, Finish, FinishDay) {
    try {
        var date1 = new Date();
        var date2 = new Date();

        date1.setFullYear(1989, 11, 30);
        date2.setFullYear(1989, 11, 30);

        switch (BeginDay) {
            case "1": date1.setDate(30); break;
            case "2": date1.setDate(31); break;
            case "0": date1.setDate(29); break;
        }

        switch (FinishDay) {
            case "1": date2.setDate(30); break;
            case "2": date2.setDate(31); break;
            case "0": date2.setDate(29); break;
        }

        date1.setHours(Begin.split(":")[0], Begin.split(":")[1], 0, 0);
        date2.setHours(Finish.split(":")[0], Finish.split(":")[1], 0, 0);

        var totalMinutes = dateDiff("n", date1, date2);

        return totalMinutes;

    } catch (e) { showError("DifferenceMinutes", e); return 0; }
}

//Redimensio del TimeLine
this.resizeTimeLine = function() {

}

//Resetejem el scroll al centre del control
this.resetScrollPos = function() {
    try {
        var divScroll = document.getElementById(objPrefix + tlScroll);
        if (divScroll != null) {
            divScroll.scrollLeft = 432;
        }
    } catch (e) { showError("resetScrollPos", e); }
}

//Recupera un objecte JSON
roTimeLine_Show = function(objPrefix, Type, ID) {
    try {
        //Busquem el DIV amb els atributs
        var oDivLayers = retTZLayers(objPrefix, Type);
        var n;
        var arrNames = new Array();
        var arrValues = new Array();
        for (n = 0; n < oDivLayers.length; n++) {
            if (oDivLayers[n].getAttribute("rolt_tlid") == ID) { //Es el layer
                for (var x = 0; x < oDivLayers[n].attributes.length; x++) {
                    if (oDivLayers[n].attributes[x].nodeName.toLowerCase() == "lt_func") { funct = oDivLayers[n].attributes[x].nodeValue; }
                    if (oDivLayers[n].attributes[x].nodeName.toLowerCase().startsWith("rolt_")) {
                        arrNames.push(oDivLayers[n].attributes[x].nodeName.toLowerCase());
                        arrValues.push(oDivLayers[n].attributes[x].nodeValue);
                    } // end if 
                } // end for 
            } //end if
        } // end for 

        //Executem la funcio predefinida al crear roTimeLine
        eval(funct + "(objPrefix, ID, Type, arrNames, arrValues);");

    } catch (e) { showError("roTimeLineShow", e); }
}

//Activar una franja
selectedTL = function(objPrefix, obj) {
    try {
        //
        //comprobem si el objecte ja es troba marcat, si es aixi, disparem roTimeLineshow
        var oTds = obj.getElementsByTagName("TD");
        var oNameOrig = oTds[1].className.split(" ")[0];

        //Invents per el IE        
        switch (BrowserDetect.browser) {
            case 'Explorer':
                if (oNameOrig.toUpperCase().indexOf("ACTIVE") > -1) {
                    roTimeLine_Show(obj.getAttribute("objPrefix"), obj.getAttribute("atltype"), obj.getAttribute("atlid"));
                }
                break;
        }

        var colA = retElementsByName(objPrefix + "_bandzone");

        for (var x = 0; x < colA.length; x++) {
            //Activem la franja seleccionada
            var oTds = colA[x].getElementsByTagName("TD");
            oTds[0].className = oTds[0].className.replace("-active", "");
            oTds[1].className = oTds[1].className.replace("-active", "");
            oTds[2].className = oTds[2].className.replace("-active", "");
        }

        //Activem la franja seleccionada
        oTds = obj.getElementsByTagName("TD");
        oNameOrig = oTds[1].className.split(" ")[0];
        oNameOrig = oNameOrig.replace("-active", "");

        if (oTds[0].className != "") { oTds[0].className = oNameOrig + '-left-active'; }
        oTds[1].className = oNameOrig + '-active bgTLBody';
        if (oTds[2].className != "") { oTds[2].className = oNameOrig + '-right-active'; }

    } catch (e) { showError("roTimeLine.selectedTL", e); }
}

// Retorna la capa seleccionada
this.selectedLayer = function() {
    try {
        var colA = retElementsByName(objPrefix + "_bandzone");
        var oResult = null;
        for (var x = 0; x < colA.length; x++) {
            //Activem la franja seleccionada
            var oTds = colA[x].getElementsByTagName("TD");

            if (oTds[1].className.indexOf("-active") > -1) {
                oResult = colA[x];
                return oResult;
            }
        }

        return oResult;

    } catch (e) { showError("roTimeLine.selectedLayer", e); }

}

//Eliminem el layer seleccionat
this.deleteSelectedLayer = function() {
    try {
        var oResult = false;
        var oLayer = this.selectedLayer();

        //Si hi ha una definicio seleccionada, eliminem
        if (oLayer != null) {
            var oType = oLayer.getAttribute("atltype");
            var oLayerID = oLayer.getAttribute("atlid");

            //Busquem el Layer (div) per ID
            var oLayers = retElementsByName(objPrefix + "_rtdiv_" + oType);

            var oParent;

            if (oType.toUpperCase().startsWith("HOURZONE")) {
                oParent = document.getElementById(objPrefix + "_cellTL_" + oType);            
            } else {
                oParent = document.getElementById(objPrefix + "_cellTL_roLT" + oType);
            }
            for (var x = 0; x < oLayers.length; x++) {
                if (oLayers[x].getAttribute("roLT_tlID") == oLayerID) { //Es aquest, eliminem buscant el pare que el conte
                    oParent.removeChild(oLayers[x]);
                    oResult = true;
                } //end if 
            } //end for
        } //end if
        listParameters.splice(layerNumber - 1, 1);
        layerNumber -= 1;

        return oResult;
    } catch (e) { showError("roTimeLine.deleteSelectedLayer", e); }
}

//Pintem tots els layers
this.loadLayers = function(jsonData) {
    try {
        this.clearLayers(); //Eliminem primer totes les capes

        var n, y;
        for (n = 0; n < jsonData.length; n++) {

            var Type = "";
            var LayerID = "";
            var ParentID = "";
            var Begin = "";
            var BeginDay = "";
            var Finish = "";
            var FinishDay = "";
            var DateDiffMin = "";
            var FloatingBeginUpTo = "";
            var FloatingBeginUpToDay = "";
            var FloatingFinishMinutes = "";
            var MaxBreakTime = "";
            var Action = "";
            var Target = "";
            var Value = "";
            var ValueDay = "";
            var MaxTime = "";
            var MaxTimeAction = "";
            var MinTime = "";
            var MinBreakAction = "";
            var MinBreakTime = "";
            var NoPunchBreakTime = "";
            var IsLocked = "";
            var AllowModifyIniHour = "";
            var AllowModifyShiftDuration = "";
            var NotificationForUser = "";
            var NotificationForSupervisor = "";
            var NotificationForUserBeforeTime = "";
            var NotificationForUserAfterTime = "";
            var RealBegin = "";
            var RealFinish = "";

            var captionTitle = "";

            for (y = 0; y < jsonData[n].layers.length; y++) {
                var fieldName = jsonData[n].layers[y].field.toUpperCase();
                var controls = jsonData[n].layers[y].control;
                var value = jsonData[n].layers[y].value;
                var typeControl = jsonData[n].layers[y].type.toUpperCase();
                var list = jsonData[n].layers[y].list;

                switch (fieldName) {
                    case "TYPE": Type = value; break;
                    case "LAYERID": LayerID = value; break;
                    case "PARENTID": ParentID = value; break;
                    case "BEGIN": Begin = value; break;
                    case "BEGINDAY": BeginDay = value; break;
                    case "FINISH": Finish = value; break;
                    case "FINISHDAY": FinishDay = value; break;
                    case "DATEDIFFMIN": DateDiffMin = value; break;
                    case "FLOATINGBEGINUPTO": FloatingBeginUpTo = value; break;
                    case "FLOATINGBEGINUPTODAY": FloatingBeginUpToDay = value; break;
                    case "FLOATINGFINISHMINUTES": FloatingFinishMinutes = value; break;
                    case "MAXBREAKTIME": MaxBreakTime = value; break;
                    case "ACTION": Action = value; break;
                    case "TARGET": Target = value; break;
                    case "VALUE": Value = value; break;
                    case "VALUEDAY": ValueDay = value; break;
                    case "MAXTIME": MaxTime = value; break;
                    case "MAXTIMEACTION": MaxTimeAction = value; break;
                    case "MINTIME": MinTime = value; break;
                    case "MINBREAKACTION": MinBreakAction = value; break;
                    case "MINBREAKTIME": MinBreakTime = value; break;
                    case "NOPUNCHBREAKTIME": NoPunchBreakTime = value; break;
                    case "TITLE": captionTitle = value; break;
                    case "ISLOCKED": IsLocked = value; break;
                    case "ALLOWMODINI": AllowModifyIniHour = value; break;
                    case "ALLOWMODDURATION": AllowModifyShiftDuration = value; break;
                    case "NOTIFICATIONFORUSER": NotificationForUser = value; break;
                    case "NOTIFICATIONFORSUPERVISOR": NotificationForSupervisor = value; break;
                    case "NOTIFICATIONFORUSERBEFORETIME": NotificationForUserBeforeTime = value; break;
                    case "NOTIFICATIONFORUSERAFTERTIME": NotificationForUserAfterTime = value; break;
                    case "REALBEGIN": RealBegin = value; break;
                    case "REALFINISH": RealFinish = value; break;
                } //end switch
            } //end for

            if (Type.toUpperCase() == "ROLTMANDATORY") { //Layer OBLIGATORIAS
                this.createLayer(colorRed, Type, LayerID, ParentID, Begin, BeginDay, Finish, FinishDay, DateDiffMin, FloatingBeginUpTo, FloatingBeginUpToDay, FloatingFinishMinutes, MaxBreakTime, MaxTime, MaxTimeAction, MinTime, Target, Value, ValueDay, Action, MinBreakAction, MinBreakTime, NoPunchBreakTime, AllowModifyIniHour, AllowModifyShiftDuration, captionTitle, IsLocked, NotificationForUser, NotificationForSupervisor, NotificationForUserBeforeTime, NotificationForUserAfterTime, RealBegin, RealFinish);
            } else if (Type.toUpperCase() == "ROLTBREAK") { //Layer DESCANSO
                this.createLayer(colorGreen, Type, LayerID, ParentID, Begin, BeginDay, Finish, FinishDay, DateDiffMin, FloatingBeginUpTo, FloatingBeginUpToDay, FloatingFinishMinutes, MaxBreakTime, MaxTime, MaxTimeAction, MinTime, Target, Value, ValueDay, Action, MinBreakAction, MinBreakTime, NoPunchBreakTime, AllowModifyIniHour, AllowModifyShiftDuration, captionTitle, IsLocked, NotificationForUser, NotificationForSupervisor, NotificationForUserBeforeTime, NotificationForUserAfterTime, RealBegin, RealFinish);
            } else if (Type.toUpperCase() == "ROLTWORKING") { //Layer FLEXIBLE
                this.createLayer(colorBlue, Type, LayerID, ParentID, Begin, BeginDay, Finish, FinishDay, DateDiffMin, FloatingBeginUpTo, FloatingBeginUpToDay, FloatingFinishMinutes, MaxBreakTime, MaxTime, MaxTimeAction, MinTime, Target, Value, ValueDay, Action, MinBreakAction, MinBreakTime, NoPunchBreakTime, AllowModifyIniHour, AllowModifyShiftDuration, captionTitle, IsLocked, NotificationForUser, NotificationForSupervisor, NotificationForUserBeforeTime, NotificationForUserAfterTime, RealBegin, RealFinish);
            } else if (Type.toUpperCase() == "ROLTWORKINGMAXMINFILTER") { //Layer Propiedades Filtro Flexible
                this.addWorkingLayerFilter(Type, LayerID, ParentID, Begin, BeginDay, Finish, FinishDay, DateDiffMin, FloatingBeginUpTo, FloatingBeginUpToDay, FloatingFinishMinutes, MaxBreakTime, MaxTime, MaxTimeAction, MinTime, Target, Value, ValueDay, Action, MinBreakAction, MinBreakTime, NoPunchBreakTime, AllowModifyIniHour, AllowModifyShiftDuration, captionTitle, IsLocked, NotificationForUser, NotificationForSupervisor, NotificationForUserBeforeTime, NotificationForUserAfterTime, RealBegin, RealFinish);
            } else if (Type.toUpperCase() == "ROLTUNITFILTER") { //Layer
                this.addUnitLayerFilter(Type, LayerID, ParentID, Begin, BeginDay, Finish, FinishDay, DateDiffMin, FloatingBeginUpTo, FloatingBeginUpToDay, FloatingFinishMinutes, MaxBreakTime, MaxTime, MaxTimeAction, MinTime, Target, Value, ValueDay, Action, MinBreakAction, MinBreakTime, NoPunchBreakTime, AllowModifyIniHour, AllowModifyShiftDuration, captionTitle, IsLocked, NotificationForUser, NotificationForSupervisor, NotificationForUserBeforeTime, NotificationForUserAfterTime, RealBegin, RealFinish);
            } else if (Type.toUpperCase() == "ROLTPAIDTIME") { //Layer
                this.addPaidLayerFilter(Type, LayerID, ParentID, Begin, BeginDay, Finish, FinishDay, DateDiffMin, FloatingBeginUpTo, FloatingBeginUpToDay, FloatingFinishMinutes, MaxBreakTime, MaxTime, MaxTimeAction, MinTime, Target, Value, ValueDay, Action, MinBreakAction, MinBreakTime, NoPunchBreakTime, AllowModifyIniHour, AllowModifyShiftDuration, captionTitle, IsLocked, NotificationForUser, NotificationForSupervisor, NotificationForUserBeforeTime, NotificationForUserAfterTime, RealBegin, RealFinish);
            } else {
                //HourZone
                this.createLayer(colorBlue, Type, LayerID, ParentID, Begin, BeginDay, Finish, FinishDay, DateDiffMin, FloatingBeginUpTo, FloatingBeginUpToDay, FloatingFinishMinutes, MaxBreakTime, MaxTime, MaxTimeAction, MinTime, Target, Value, ValueDay, Action, MinBreakAction, MinBreakTime, NoPunchBreakTime, AllowModifyIniHour, AllowModifyShiftDuration, captionTitle, IsLocked);
            }

        } //end for

        this.resetScrollPos(); //Reposicionem l'scroll al centre

    } catch (e) { showError("roTimeLine.loadLayers", e); }
}

//funcio que recorre totes les capes i els seus valors i els possa a un objecte json
this.retrieveJSONLayers = function() {
    try {

        var oTable = document.getElementById(objPrefix + "_tableTL");
        if (oTable == null) { return; }

        var oLayers = new Array();
        var JSONString = "";

        //Si conte alguna layer que recorre
        if (oTable.rows.length > 2) {

            JSONString += "[";

            for (var n = 0; n < oTable.rows.length - 2; n++) {
                oTd = oTable.rows[n];
                var oDivs = oTd.getElementsByTagName("DIV");

                if (oDivs.length > 0) {
                    for (var n1 = 0; n1 < oDivs.length; n1++) {
                        JSONString += "{ 'layer': [";
                        for (var x = 0; x < oDivs[n1].attributes.length; x++) {
                            if (oDivs[n1].attributes[x].nodeName.toLowerCase().startsWith("rolt_")) {
                                JSONString += "{";
                                JSONString += "'field':'" + oDivs[n1].attributes[x].nodeName.toLowerCase() + "',";
                                JSONString += "'value':'" + oDivs[n1].attributes[x].nodeValue + "'";
                                JSONString += "},";
                            } // end if
                        } // end for
                        if (JSONString.charAt(JSONString.length - 1) == ",")
                            JSONString = JSONString.substring(0, JSONString.length - 1);

                        JSONString += "] },";
                    } //end for


                } //end if
            } //end for
            if (JSONString.charAt(JSONString.length - 1) == ",")
                JSONString = JSONString.substring(0, JSONString.length - 1);

            JSONString += "]";

            var oJSONObj = eval(JSONString);

            return oJSONObj;

        } //end if

    } catch (e) { showError("retrieveJSONLayers", e); }
}

this.roTimeLine_ConvertMinutesToHour = function (Minutes) {
    try {
        var Hours = Math.floor(parseInt(Minutes) / 60);
        var MinutesRest = "00";
        if ((parseInt(Minutes) ^ 60) > 0) { //Si no son horas justas, sacar los minutos
            MinutesRest = parseInt(Minutes) - (Hours * 60);
        }

        if (Hours.toString().length == 1) { Hours = "0" + Hours; }
        if (MinutesRest.toString().length == 1) { MinutesRest = "0" + MinutesRest; }

        return Hours + ":" + MinutesRest;

    } catch (e) { showError("roTimeLine_ConvertMinutesToHour", e); }
}

this.roTimeLine_ConvertHoursToMinutes = function(Hours) {
    try {
        var HourStr = Hours.split(":")[0];
        if (HourStr == "0")
            HourStr == "0";
        else {
            if (HourStr.substr(0, 1) == "0") 
                HourStr = HourStr.substr(1, 1);
        }
        
        var Hour = parseInt(HourStr); // parseInt(Hours.split(":")[0]);
        var MinutesStr = Hours.split(":")[1];
        if (MinutesStr.substr(0, 1) == "0") MinutesStr = MinutesStr.substr(1, 1);

        var Minutes = parseInt(MinutesStr);  //parseInt(Hours.split(":")[1]);
        var oResult = 0;

        oResult = Hour * 60;
        oResult = parseInt(oResult + Minutes);

        //alert(oResult);
        return oResult;

    } catch (e) { showError("roTimeLine_ConvertHoursToMinutes", e); return 0; }
}

//Load
//Carrega de les dades, si existeix
if (jsonData != null) {
    loadLayers(jsonData);
}
    
    
} // End Class roTimeLine
    
    