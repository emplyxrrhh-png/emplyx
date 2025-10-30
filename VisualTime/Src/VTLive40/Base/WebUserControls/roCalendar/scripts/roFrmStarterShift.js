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

    namespace("Robotics.Client.Controls.Forms");
}());

Robotics.Client.Controls.Forms.StarterForm = function (prefix, showErrorPopup) {
    this.prefix = prefix;
    this.oCalendarShift = null;
    this.oOriginShiftInfo = null;
    this.oShiftData = null;
    this.showErrorPopup = showErrorPopup;

    this.shiftDefinitionCallback = eval(this.prefix + "_shiftDefinitionCallbackClient");

    this.txtShiftStart1 = eval(this.prefix + "_txtShiftStart1Client");
    this.txtShiftEnd1 = eval(this.prefix + "_txtShiftEnd1Client");
    this.txtShiftOrdinary1 = eval(this.prefix + "_txtShiftOrdinary1Client");
};

Robotics.Client.Controls.Forms.StarterForm.prototype.prepareStarterDialog = function (oCalendarShift, idShift, oShiftData) {
    if (typeof (oShiftData) != 'undefined') this.oShiftData = oShiftData;
    else this.oShiftData = null;

    if (oCalendarShift == null) {
        var oParameters = {};
        oParameters.idShift = idShift;
        oParameters.StampParam = new Date().getMilliseconds();
        oParameters.action = "SHIFTLAYERDEFINITION";
        var strParameters = JSON.stringify(oParameters);
        strParameters = encodeURIComponent(strParameters);

        this.shiftDefinitionCallback.PerformCallback(strParameters);

        return false;
    } else {
        if (this.oShiftData != null) {
            this.oCalendarShift = this.getDayDataEditting(oCalendarShift, this.oShiftData);
        } else {
            this.oCalendarShift = oCalendarShift;
        }
        this.finallyPrepareDialogElements();
        return true;
    }
};

Robotics.Client.Controls.Forms.StarterForm.prototype.finallyPrepareDialogElements = function (objResult) {
    if (typeof (objResult) != 'undefined') this.oCalendarShift = objResult;

    this.txtShiftStart1.SetDate(this.oCalendarShift.StartFloating);
    this.txtShiftEnd1.SetDate(this.oCalendarShift.EndFloating);

    var midnight = moment(this.oCalendarShift.StartLayer1).clone().startOf('day');
    this.txtShiftOrdinary1.SetDate(midnight.clone().add(this.oCalendarShift.WorkingHours, 'minutes').toDate());
};

Robotics.Client.Controls.Forms.StarterForm.prototype.setOffset = function (startHour) {
    var incrementMinutes = 0;

    if (incrementMinutes == 0) {
        var ShiftStartDate = new Date(1999, 11, 30, this.oCalendarShift.StartFloating.getHours(), this.oCalendarShift.StartFloating.getMinutes(), 0, 0);
        incrementMinutes = moment(startHour).diff(moment(ShiftStartDate), 'minutes');
    }

    if (incrementMinutes != 0) {
        this.txtShiftStart1.SetDate(moment(this.oCalendarShift.StartFloating).clone().add(incrementMinutes, 'minutes').toDate());
        this.txtEndStart1.SetDate(moment(this.oCalendarShift.EndFloating).clone().add(incrementMinutes, 'minutes').toDate());
    }
};

Robotics.Client.Controls.Forms.StarterForm.prototype.isValid = function (startHour) {
    var tmpData = this;
    var bIsValid = true;
    var ordinaryHours = -1;

    var startLayerTime1 = new Date(1900, 0, 1, 0, 0, 0, 0);
    startLayerTime1 = new Date(1999, 11, tmpData.getDateIfNullFromControl(tmpData.txtShiftStart1).getDate(), tmpData.getDateIfNullFromControl(tmpData.txtShiftStart1).getHours(), tmpData.getDateIfNullFromControl(tmpData.txtShiftStart1).getMinutes(), 0, 0);

    var endLayerTime1 = new Date(1900, 0, 1, 0, 0, 0, 0);
    endLayerTime1 = new Date(1999, 11, tmpData.getDateIfNullFromControl(tmpData.txtShiftEnd1).getDate(), tmpData.getDateIfNullFromControl(tmpData.txtShiftEnd1).getHours(), tmpData.getDateIfNullFromControl(tmpData.txtShiftEnd1).getMinutes(), 0, 0);

    var duration = moment(endLayerTime1).diff(moment(startLayerTime1), 'minutes');

    if (duration < 0) {
        endLayerTime1 = moment(endLayerTime1).clone().add(1, 'days').toDate();
        this.txtShiftEnd1.SetDate(endLayerTime1);
        duration = moment(endLayerTime1).diff(moment(startLayerTime1), 'minutes');
    }

    var startLayerTime1 = new Date(1900, 0, 1, 0, 0, 0, 0);
    startLayerTime1 = new Date(1999, 11, tmpData.getDateIfNullFromControl(tmpData.txtShiftStart1).getDate(), tmpData.getDateIfNullFromControl(tmpData.txtShiftOrdinary1).getHours(), tmpData.getDateIfNullFromControl(tmpData.txtShiftOrdinary1).getMinutes(), 0, 0);

    var midnight = moment(startLayerTime1).clone().startOf('day');
    var workinghours = moment(startLayerTime1).diff(moment(midnight.toDate()), 'minutes');

    if (duration < workinghours) {
        bIsValid = false;
        this.showErrorPopup("Error.Title", "error", "Calendar.Client.WorkingHoursOverDuration", "", "Error.OK", "Error.OKDesc", "");
        return bIsValid;
    }

    return bIsValid;
};

Robotics.Client.Controls.Forms.StarterForm.prototype.getDateIfNullFromControl = function (control) {
    if (control.GetValue() != null) {
        return control.GetDate();
    } else {
        return new Date(1999, 11, 30, 0, 0, 0);
    }
};

Robotics.Client.Controls.Forms.StarterForm.prototype.getDayDataEditting = function (oCalendarShift, originShift) {
    var edittingShiftDefinition = {};

    edittingShiftDefinition.Type = originShift.Type;
    edittingShiftDefinition.AdvancedParameters = originShift.AdvancedParameters;
    edittingShiftDefinition.IDShift = originShift.ID;
    edittingShiftDefinition.IsFloating = oCalendarShift.IsFloating;
    edittingShiftDefinition.WorkingHours = originShift.PlannedHours;
    edittingShiftDefinition.AllowFloating = oCalendarShift.AllowFloating;
    edittingShiftDefinition.StartFloating = moment(originShift.StartHour).clone().toDate();
    edittingShiftDefinition.EndFloating = moment(originShift.EndHour).clone().toDate();
    edittingShiftDefinition.AllowComplementary = oCalendarShift.AllowComplementary;
    edittingShiftDefinition.Color = oCalendarShift.Color;
    edittingShiftDefinition.Name = oCalendarShift.Name;
    edittingShiftDefinition.ShortName = oCalendarShift.ShortName;
    edittingShiftDefinition.IDLayer1 = oCalendarShift.IDLayer1;
    edittingShiftDefinition.IDLayer2 = oCalendarShift.IDLayer2;
    edittingShiftDefinition.CountLayers = oCalendarShift.CountLayers;
    edittingShiftDefinition.HasLayer1FixedEnd = oCalendarShift.HasLayer1FixedEnd;
    edittingShiftDefinition.HasLayer2FixedEnd = oCalendarShift.HasLayer2FixedEnd;
    edittingShiftDefinition.AllowComplementary1 = oCalendarShift.AllowComplementary1;
    edittingShiftDefinition.AllowComplementary2 = oCalendarShift.AllowComplementary2;
    edittingShiftDefinition.AllowModifyIniHour1 = oCalendarShift.AllowModifyIniHour1;
    edittingShiftDefinition.AllowModifyIniHour2 = oCalendarShift.AllowModifyIniHour2;
    edittingShiftDefinition.AllowModifyDuration1 = oCalendarShift.AllowModifyDuration1;
    edittingShiftDefinition.AllowModifyDuration2 = oCalendarShift.AllowModifyDuration2;
    edittingShiftDefinition.BreakHours = oCalendarShift.BreakHours;
    edittingShiftDefinition.StartLayer1 = oCalendarShift.StartLayer1;
    edittingShiftDefinition.EndLayer1 = oCalendarShift.EndLayer1;
    edittingShiftDefinition.StartLayer2 = oCalendarShift.StartLayer2;
    edittingShiftDefinition.EndLayer2 = oCalendarShift.EndLayer2;

    return edittingShiftDefinition;
};

Robotics.Client.Controls.Forms.StarterForm.prototype.getDayData = function () {
    var shiftData = {};
    var tmpData = this;

    var startLayerTime1 = new Date(1900, 0, 1, 0, 0, 0, 0);
    startLayerTime1 = new Date(1999, 11, tmpData.getDateIfNullFromControl(tmpData.txtShiftStart1).getDate(), tmpData.getDateIfNullFromControl(tmpData.txtShiftOrdinary1).getHours(), tmpData.getDateIfNullFromControl(tmpData.txtShiftOrdinary1).getMinutes(), 0, 0);

    var midnight = moment(startLayerTime1).clone().startOf('day');
    var duration = moment(startLayerTime1).diff(moment(midnight.toDate()), 'minutes');

    var shiftName = 'U' + tmpData.getDateIfNullFromControl(tmpData.txtShiftStart1).getHours().toString().padStart(2, '0') + tmpData.getDateIfNullFromControl(tmpData.txtShiftStart1).getMinutes().toString().padStart(2, '0');
    shiftName += tmpData.getDateIfNullFromControl(tmpData.txtShiftEnd1).getHours().toString().padStart(2, '0') + tmpData.getDateIfNullFromControl(tmpData.txtShiftEnd1).getMinutes().toString().padStart(2, '0') + duration;

    var r = tmpData.getDateIfNullFromControl(tmpData.txtShiftStart1).getMinutes().toString().padStart(2, '0') + tmpData.getDateIfNullFromControl(tmpData.txtShiftStart1).getHours().toString().padStart(2, '0');
    var g = tmpData.getDateIfNullFromControl(tmpData.txtShiftEnd1).getMinutes().toString().padStart(2, '0') + tmpData.getDateIfNullFromControl(tmpData.txtShiftEnd1).getHours().toString().padStart(2, '0');
    var b = duration;

    shiftData.ID = this.oCalendarShift.IDShift;
    shiftData.ShortName = shiftName;
    shiftData.Name = shiftName;
    shiftData.Color = new Robotics.Client.Common.roHtmlColor().randomColorWithSeed(r, g, b);
    shiftData.ShiftLayers = this.oCalendarShift.CountLayers;
    shiftData.Type = this.oCalendarShift.Type;
    shiftData.AdvancedParameters = this.oCalendarShift.AdvancedParameters,
        shiftData.ExistComplementaryData = false;
    shiftData.ExistFloatingData = false;
    shiftData.ShiftLayersDefinition = new Array();
    shiftData.BreakHours = this.oCalendarShift.BreakHours;
    shiftData.PlannedHours = this.oCalendarShift.WorkingHours;
    shiftData.AdvancedParameters = this.oCalendarShift.AdvancedParameters;

    shiftData.StartHour = new Date(1999, 11, 30, tmpData.getDateIfNullFromControl(tmpData.txtShiftStart1).getHours(), tmpData.getDateIfNullFromControl(tmpData.txtShiftStart1).getMinutes(), 0, 0);

    if (tmpData.getDateIfNullFromControl(tmpData.txtShiftEnd1).getHours() >= tmpData.getDateIfNullFromControl(tmpData.txtShiftStart1).getHours()) {
        shiftData.EndHour = new Date(1999, 11, 30, tmpData.getDateIfNullFromControl(tmpData.txtShiftEnd1).getHours(), tmpData.getDateIfNullFromControl(tmpData.txtShiftEnd1).getMinutes(), 0, 0);
    } else {
        shiftData.EndHour = new Date(1999, 11, 31, tmpData.getDateIfNullFromControl(tmpData.txtShiftEnd1).getHours(), tmpData.getDateIfNullFromControl(tmpData.txtShiftEnd1).getMinutes(), 0, 0);
    }

    shiftData.PlannedHours = duration;

    return shiftData;
};