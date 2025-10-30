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

Robotics.Client.Controls.Forms.ComplementaryForm = function (prefix, showErrorPopup) {
    this.prefix = prefix;
    this.oCalendarShift = null;
    this.oOriginShiftInfo = null;
    this.oShiftData = null;
    this.showErrorPopup = showErrorPopup;

    this.shiftDefinitionCallback = eval(this.prefix + "_shiftDefinitionCallbackClient");
    this.txtShiftFloatingStart = eval(this.prefix + "_txtShiftFloatingStartClient");
    this.cmbStartAtFloating = eval(this.prefix + "_cmbStartAtFloatingClient");

    this.txtShiftStart1 = eval(this.prefix + "_txtShiftStart1Client");
    this.cmbStartAt1 = eval(this.prefix + "_cmbStartAt1Client");
    this.txtShiftOrdinary1 = eval(this.prefix + "_txtShiftOrdinary1Client");
    this.txtShiftComplementary1 = eval(this.prefix + "_txtShiftComplementary1Client");

    this.txtShiftStart2 = eval(this.prefix + "_txtShiftStart2Client");
    this.cmbStartAt2 = eval(this.prefix + "_cmbStartAt2Client");
    this.txtShiftOrdinary2 = eval(this.prefix + "_txtShiftOrdinary2Client");
    this.txtShiftComplementary2 = eval(this.prefix + "_txtShiftComplementary2Client");
};

Robotics.Client.Controls.Forms.ComplementaryForm.prototype.prepareComplementaryDialog = function (oCalendarShift, idShift, oShiftData) {
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

Robotics.Client.Controls.Forms.ComplementaryForm.prototype.setOffset = function (startHour) {
    var incrementMinutes = 0;

    if (this.oCalendarShift.IsFloating) {
        var ShiftStartDate = new Date(1999, 11, 30, this.oCalendarShift.StartFloating.getHours(), this.oCalendarShift.StartFloating.getMinutes(), 0, 0);
        incrementMinutes = moment(startHour).diff(moment(ShiftStartDate), 'minutes');

        if (incrementMinutes != 0) this.txtShiftFloatingStart.SetDate(moment(this.oCalendarShift.StartFloating).clone().add(incrementMinutes, 'minutes').toDate());
    }

    if (this.oCalendarShift.CountLayers > 0 && this.oCalendarShift.AllowModifyIniHour1) {
        if (incrementMinutes == 0) {
            var ShiftStartDate = new Date(1999, 11, 30, this.oCalendarShift.StartLayer1.getHours(), this.oCalendarShift.StartLayer1.getMinutes(), 0, 0);
            incrementMinutes = moment(startHour).diff(moment(ShiftStartDate), 'minutes');
        }

        if (incrementMinutes != 0) this.txtShiftStart1.SetDate(moment(this.oCalendarShift.StartLayer1).clone().add(incrementMinutes, 'minutes').toDate());
    }

    if (this.oCalendarShift.CountLayers > 1 && this.oCalendarShift.AllowModifyIniHour2) {
        if (incrementMinutes == 0) {
            var ShiftStartDate = new Date(1999, 11, 30, this.oCalendarShift.StartLayer2.getHours(), this.oCalendarShift.StartLayer2.getMinutes(), 0, 0);
            incrementMinutes = moment(startHour).diff(moment(ShiftStartDate), 'minutes');
        }

        if (incrementMinutes != 0) this.txtShiftStart2.SetDate(moment(this.oCalendarShift.StartLayer2).clone().add(incrementMinutes, 'minutes').toDate());
    }
};

Robotics.Client.Controls.Forms.ComplementaryForm.prototype.finallyPrepareDialogElements = function (objResult) {
    if (typeof (objResult) != 'undefined') this.oCalendarShift = objResult;

    if (!this.oCalendarShift.IsFloating) {
        $('#' + this.prefix + '_advShiftStartFloating').hide();
    } else {
        $('#' + this.prefix + '_advShiftStartFloating').show();
        this.txtShiftFloatingStart.SetDate(this.oCalendarShift.StartFloating);

        if (this.oCalendarShift.StartFloating.getDate() == 29) {
            this.cmbStartAtFloating.SetSelectedItem(this.cmbStartAtFloating.FindItemByValue(0));
        } else if (this.oCalendarShift.StartFloating.getDate() == 30) {
            this.cmbStartAtFloating.SetSelectedItem(this.cmbStartAtFloating.FindItemByValue(1));
        } else if (this.oCalendarShift.StartFloating.getDate() == 31) {
            this.cmbStartAtFloating.SetSelectedItem(this.cmbStartAtFloating.FindItemByValue(2));
        }
    }

    $('#' + this.prefix + '_advFirstLayer').hide();
    $('#' + this.prefix + '_advSecondLayer').hide();

    if (this.oCalendarShift.CountLayers > 0) {
        $('#' + this.prefix + '_advFirstLayer').show();

        this.txtShiftStart1.SetDate(this.oCalendarShift.StartLayer1);

        if (this.oCalendarShift.StartLayer1.getDate() == 29) {
            this.cmbStartAt1.SetSelectedItem(this.cmbStartAt1.FindItemByValue(0));
        } else if (this.oCalendarShift.StartLayer1.getDate() == 30) {
            this.cmbStartAt1.SetSelectedItem(this.cmbStartAt1.FindItemByValue(1));
        } else if (this.oCalendarShift.StartLayer1.getDate() == 31) {
            this.cmbStartAt1.SetSelectedItem(this.cmbStartAt1.FindItemByValue(2));
        }

        var diffMinutes = 0;
        var midnight = moment(this.oCalendarShift.StartLayer1).clone().startOf('day');

        //this.txtShiftComplementary1.SetDate(midnight.clone().toDate());
        //this.txtShiftOrdinary1.SetDate(midnight.clone().toDate());

        if (this.oShiftData == null) {
            diffMinutes = moment(this.oCalendarShift.EndLayer1).diff(moment(this.oCalendarShift.StartLayer1), 'minutes');

            this.txtShiftComplementary1.SetDate(midnight.clone().toDate());
            this.txtShiftOrdinary1.SetDate(midnight.clone().add(diffMinutes, 'minutes').toDate());
        } else {
            if (this.oCalendarShift.AllowComplementary1) {
                this.txtShiftComplementary1.SetDate(midnight.clone().add(parseInt((this.oShiftData.ShiftLayersDefinition[0].LayerComplementaryHours), 10), 'minutes').toDate());
                this.txtShiftOrdinary1.SetDate(midnight.clone().add(parseInt((this.oShiftData.ShiftLayersDefinition[0].LayerOrdinaryHours), 10), 'minutes').toDate());
            } else {
                this.txtShiftComplementary1.SetDate(midnight.clone().toDate());

                if (this.oCalendarShift.AllowModifyIniHour1) {
                    diffMinutes = moment(this.oCalendarShift.EndLayer1).diff(moment(this.txtShiftStart1.GetDate()), 'minutes');
                    this.txtShiftOrdinary1.SetDate(midnight.clone().add(diffMinutes, 'minutes').toDate());
                } else {
                    this.txtShiftOrdinary1.SetDate(midnight.clone().add(parseInt((this.oShiftData.ShiftLayersDefinition[0].LayerDuration), 10), 'minutes').toDate());
                }
            }
        }

        if (this.oCalendarShift.AllowComplementary1) {
            $('#' + this.prefix + '_firstLayerComplementary').show();
            this.txtShiftComplementary1.SetEnabled(true);
        } else {
            $('#' + this.prefix + '_firstLayerComplementary').hide();
            this.txtShiftComplementary1.SetEnabled(false);
        }

        if (this.oCalendarShift.AllowModifyIniHour1) {
            this.txtShiftStart1.SetEnabled(true);
            this.cmbStartAt1.SetEnabled(true);

            if (!this.oCalendarShift.AllowComplementary1 && !this.oCalendarShift.AllowModifyDuration1) {
                $('#' + this.prefix + '_firstLayerOrdinary').hide();
                this.txtShiftOrdinary1.SetEnabled(false);
            } else {
                $('#' + this.prefix + '_firstLayerOrdinary').show();
                this.txtShiftOrdinary1.SetEnabled(true);
            }
        } else {
            this.txtShiftStart1.SetEnabled(false);
            this.cmbStartAt1.SetEnabled(false);
        }
    }

    if (this.oCalendarShift.CountLayers > 1) {
        $('#' + this.prefix + '_advSecondLayer').show();

        this.txtShiftStart2.SetDate(this.oCalendarShift.StartLayer2);

        if (this.oCalendarShift.StartLayer2.getDate() == 29) {
            this.cmbStartAt2.SetSelectedItem(this.cmbStartAt2.FindItemByValue(0));
        } else if (this.oCalendarShift.StartLayer2.getDate() == 30) {
            this.cmbStartAt2.SetSelectedItem(this.cmbStartAt2.FindItemByValue(1));
        } else if (this.oCalendarShift.StartLayer2.getDate() == 31) {
            this.cmbStartAt2.SetSelectedItem(this.cmbStartAt2.FindItemByValue(2));
        }

        var diffMinutes = 0;
        var midnight = moment(this.oCalendarShift.StartLayer2).clone().startOf('day');

        //this.txtShiftComplementary2.SetDate(midnight.clone().toDate());
        //this.txtShiftOrdinary2.SetDate(midnight.clone().toDate());

        if (this.oShiftData == null) {
            diffMinutes = moment(this.oCalendarShift.EndLayer2).diff(moment(this.oCalendarShift.StartLayer2), 'minutes');

            this.txtShiftComplementary2.SetDate(midnight.clone().toDate());
            this.txtShiftOrdinary2.SetDate(midnight.clone().add(diffMinutes, 'minutes').toDate());
        } else {
            if (this.oCalendarShift.AllowComplementary2) {
                this.txtShiftComplementary2.SetDate(midnight.clone().add(parseInt((this.oShiftData.ShiftLayersDefinition[1].LayerComplementaryHours), 10), 'minutes').toDate());
                this.txtShiftOrdinary2.SetDate(midnight.clone().add(parseInt((this.oShiftData.ShiftLayersDefinition[1].LayerOrdinaryHours), 10), 'minutes').toDate());
            } else {
                this.txtShiftComplementary2.SetDate(midnight.clone().toDate());
                if (this.oCalendarShift.AllowModifyIniHour1) {
                    diffMinutes = moment(this.oCalendarShift.EndLayer2).diff(moment(this.txtShiftStart2.GetDate()), 'minutes');
                    this.txtShiftOrdinary2.SetDate(midnight.clone().add(diffMinutes, 'minutes').toDate());
                } else {
                    this.txtShiftOrdinary2.SetDate(midnight.clone().add(parseInt((this.oShiftData.ShiftLayersDefinition[1].LayerDuration), 10), 'minutes').toDate());
                }
            }
        }

        if (this.oCalendarShift.AllowComplementary2) {
            $('#' + this.prefix + '_secondLayerComplementary').show();
            this.txtShiftComplementary2.SetEnabled(true);
        } else {
            $('#' + this.prefix + '_secondLayerComplementary').hide();
            this.txtShiftComplementary2.SetEnabled(false);
        }

        if (this.oCalendarShift.AllowModifyIniHour2) {
            this.txtShiftStart2.SetEnabled(true);
            this.cmbStartAt2.SetEnabled(true);

            if (!this.oCalendarShift.AllowComplementary2 && !this.oCalendarShift.AllowModifyDuration2) {
                $('#' + this.prefix + '_secondLayerOrdinary').hide();
                this.txtShiftOrdinary2.SetEnabled(false);
            } else {
                $('#' + this.prefix + '_secondLayerOrdinary').show();
                this.txtShiftOrdinary2.SetEnabled(true);
            }
        } else {
            this.txtShiftStart2.SetEnabled(false);
            this.cmbStartAt2.SetEnabled(false);
        }
    }
};

Robotics.Client.Controls.Forms.ComplementaryForm.prototype.getDateIfNullFromControl = function (control) {
    if (control.GetValue() != null) {
        return control.GetDate();
    } else {
        return new Date(1999, 11, 30, 0, 0, 0);
    }
};

Robotics.Client.Controls.Forms.ComplementaryForm.prototype.getDayData = function () {
    let shiftData = {};
    let tmpData = this;

    let layer1Duration = moment(this.oCalendarShift.EndLayer1).diff(moment(this.oCalendarShift.StartLayer1), "minutes");
    let layer2Duration = moment(this.oCalendarShift.EndLayer2).diff(moment(this.oCalendarShift.StartLayer2), "minutes");

    shiftData.ID = this.oCalendarShift.IDShift;
    shiftData.ShortName = this.oCalendarShift.ShortName;
    shiftData.Name = this.oCalendarShift.Name;
    shiftData.Color = this.oCalendarShift.Color;
    shiftData.ShiftLayers = this.oCalendarShift.CountLayers;
    shiftData.Type = this.oCalendarShift.Type;
    shiftData.AdvancedParameters = this.oCalendarShift.AdvancedParameters,
        shiftData.ExistComplementaryData = this.oCalendarShift.AllowComplementary;
    shiftData.ExistFloatingData = this.oCalendarShift.AllowFloating;
    shiftData.ShiftLayersDefinition = new Array();
    shiftData.BreakHours = this.oCalendarShift.BreakHours;
    shiftData.PlannedHours = this.oCalendarShift.WorkingHours;

    if (this.oCalendarShift.Type == Robotics.Client.Constants.ShiftType.NormalFloating) {
        let newStartDate = 30;
        if (this.cmbStartAtFloating.GetSelectedItem().value == 0) {
            newStartDate = 29;
        } else if (this.cmbStartAtFloating.GetSelectedItem().value == 2) {
            newStartDate = 31;
        }
        shiftData.StartHour = new Date(1999, 11, newStartDate, tmpData.getDateIfNullFromControl(tmpData.txtShiftFloatingStart).getHours(), tmpData.getDateIfNullFromControl(tmpData.txtShiftFloatingStart).getMinutes(), 0, 0);
    } else if (shiftData.ShiftLayers > 0) {
        let newStartDate = 30;
        if (this.cmbStartAt1.GetSelectedItem().value == 0) {
            newStartDate = 29;
        } else if (this.cmbStartAt1.GetSelectedItem().value == 2) {
            newStartDate = 31;
        }

        shiftData.StartHour = new Date(1999, 11, newStartDate, tmpData.getDateIfNullFromControl(tmpData.txtShiftStart1).getHours(), tmpData.getDateIfNullFromControl(tmpData.txtShiftStart1).getMinutes(), 0, 0);
    } else {
        shiftData.StartHour = this.oCalendarShift.StartFloating;
    }

    if (shiftData.ShiftLayers == 0) {
        shiftData.PlannedHours = this.oCalendarShift.WorkingHours;
        shiftData.EndHour = moment(shiftData.StartHour).add(shiftData.PlannedHours, 'minutes').toDate();
    } else {
        if (shiftData.ShiftLayers > 0) {
            let newStartDate = 30;
            if (this.cmbStartAt1.GetSelectedItem().value == 0) {
                newStartDate = 29;
            } else if (this.cmbStartAt1.GetSelectedItem().value == 2) {
                newStartDate = 31;
            }

            let ordinaryDate = moment(tmpData.getDateIfNullFromControl(tmpData.txtShiftOrdinary1));
            let complentaryDate = moment(tmpData.getDateIfNullFromControl(tmpData.txtShiftComplementary1));
            let midnight = ordinaryDate.clone().startOf('day');

            let ordinaryHours = ordinaryDate.clone().diff(midnight, 'minutes');
            let complementaryHours = complentaryDate.clone().diff(midnight, 'minutes');
            let duration = ordinaryHours + complementaryHours;

            let startLayerTime = new Date(1999, 11, newStartDate, tmpData.getDateIfNullFromControl(tmpData.txtShiftStart1).getHours(), tmpData.getDateIfNullFromControl(tmpData.txtShiftStart1).getMinutes(), 0, 0);

            if (this.oCalendarShift.AllowModifyIniHour1 && !this.oCalendarShift.HasLayer1FixedEnd) {
                if (this.oCalendarShift.AllowModifyDuration1) {
                    shiftData.EndHour = moment(startLayerTime).add(duration, 'minutes').toDate();
                } else {
                    shiftData.EndHour = moment(startLayerTime).add(layer1Duration, 'minutes').toDate();
                }
            } else if (!this.oCalendarShift.AllowModifyIniHour1 && !this.oCalendarShift.HasLayer1FixedEnd) {
                shiftData.EndHour = moment(startLayerTime).add(duration, 'minutes').toDate();
            } else {
                shiftData.EndHour = this.oCalendarShift.EndLayer1;
            }

            shiftData.PlannedHours = duration;
            shiftData.ShiftLayersDefinition.push({
                LayerStartTime: startLayerTime,
                ExistLayerStartTime: tmpData.oCalendarShift.AllowModifyIniHour1,
                LayerOrdinaryHours: ordinaryHours,
                LayerDuration: duration,
                ExistLayerDuration: tmpData.oCalendarShift.AllowModifyDuration1,
                LayerID: tmpData.oCalendarShift.IDLayer1,
                LayerComplementaryHours: complementaryHours
            });
        }

        if (shiftData.ShiftLayers > 1) {
            let newStartDate = 30;
            if (this.cmbStartAt2.GetSelectedItem().value == 0) {
                newStartDate = 29;
            } else if (this.cmbStartAt2.GetSelectedItem().value == 2) {
                newStartDate = 31;
            }

            let ordinaryDate = moment(tmpData.getDateIfNullFromControl(tmpData.txtShiftOrdinary2));
            let complentaryDate = moment(tmpData.getDateIfNullFromControl(tmpData.txtShiftComplementary2));
            let midnight = ordinaryDate.clone().startOf('day');

            let ordinaryHours = ordinaryDate.clone().diff(midnight, 'minutes');
            let complementaryHours = complentaryDate.clone().diff(midnight, 'minutes');
            let duration = ordinaryHours + complementaryHours;

            let startLayerTime = new Date(1999, 11, newStartDate, tmpData.getDateIfNullFromControl(tmpData.txtShiftStart2).getHours(), tmpData.getDateIfNullFromControl(tmpData.txtShiftStart2).getMinutes(), 0, 0);

            if (this.oCalendarShift.AllowModifyIniHour2 && !this.oCalendarShift.HasLayer2FixedEnd) {
                if (this.oCalendarShift.AllowModifyDuration2) {
                    shiftData.EndHour = moment(startLayerTime).add(duration, 'minutes').toDate();
                } else {
                    shiftData.EndHour = moment(startLayerTime).add(layer2Duration, 'minutes').toDate();
                }
            } else if (!this.oCalendarShift.AllowModifyIniHour2 && !this.oCalendarShift.HasLayer2FixedEnd) {
                shiftData.EndHour = moment(startLayerTime).add(duration, 'minutes').toDate();
            } else {
                shiftData.EndHour = this.oCalendarShift.EndLayer2;
            }

            shiftData.PlannedHours = shiftData.PlannedHours + duration;
            shiftData.ShiftLayersDefinition.push({
                LayerStartTime: startLayerTime,
                ExistLayerStartTime: tmpData.oCalendarShift.AllowModifyIniHour2,
                LayerOrdinaryHours: ordinaryHours,
                LayerDuration: duration,
                ExistLayerDuration: tmpData.oCalendarShift.AllowModifyDuration2,
                LayerID: tmpData.oCalendarShift.IDLayer2,
                LayerComplementaryHours: complementaryHours
            });
        }

        shiftData.PlannedHours = shiftData.PlannedHours - tmpData.oCalendarShift.BreakHours;
    }

    return shiftData;
};

Robotics.Client.Controls.Forms.ComplementaryForm.prototype.getDayDataEditting = function (oCalendarShift, originShift) {
    let layer1Duration = moment(oCalendarShift.EndLayer1).diff(moment(oCalendarShift.StartLayer1), "minutes");
    let layer2Duration = moment(oCalendarShift.EndLayer2).diff(moment(oCalendarShift.StartLayer2), "minutes");
    var edittingShiftDefinition = {};

    edittingShiftDefinition.Type = originShift.Type;
    edittingShiftDefinition.IDShift = originShift.ID;
    edittingShiftDefinition.IsFloating = oCalendarShift.IsFloating;
    edittingShiftDefinition.WorkingHours = originShift.PlannedHours;
    edittingShiftDefinition.AllowFloating = oCalendarShift.AllowFloating;
    edittingShiftDefinition.StartFloating = moment(originShift.StartHour).clone().toDate();
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
    edittingShiftDefinition.AdvancedParameters = oCalendarShift.AdvancedParameters;
    edittingShiftDefinition.BreakLayers = oCalendarShift.BreakLayers;

    if (oCalendarShift.CountLayers > 0 && oCalendarShift.AllowModifyIniHour1) {
        edittingShiftDefinition.StartLayer1 = moment(originShift.ShiftLayersDefinition[0].LayerStartTime).clone().toDate();

        if (oCalendarShift.AllowModifyDuration1) edittingShiftDefinition.EndLayer1 = moment(originShift.ShiftLayersDefinition[0].LayerStartTime).clone().add(originShift.ShiftLayersDefinition[0].LayerDuration, 'minutes').toDate();
        else {
            if (oCalendarShift.HasLayer1FixedEnd) {
                edittingShiftDefinition.EndLayer1 = oCalendarShift.EndLayer1;
            } else {
                edittingShiftDefinition.EndLayer1 = moment(edittingShiftDefinition.StartLayer1).add(layer1Duration, 'minutes').toDate();
            }
        }
    } else {
        edittingShiftDefinition.StartLayer1 = oCalendarShift.StartLayer1;
        edittingShiftDefinition.EndLayer1 = oCalendarShift.EndLayer1;
    }
    if (oCalendarShift.CountLayers > 1 && oCalendarShift.AllowModifyIniHour2) {
        edittingShiftDefinition.StartLayer2 = moment(originShift.ShiftLayersDefinition[1].LayerStartTime).clone().toDate();
        if (oCalendarShift.AllowModifyDuration2) edittingShiftDefinition.EndLayer2 = moment(originShift.ShiftLayersDefinition[1].LayerStartTime).clone().add(originShift.ShiftLayersDefinition[1].LayerDuration, 'minutes').toDate();
        else {
            if (oCalendarShift.HasLayer2FixedEnd) {
                edittingShiftDefinition.EndLayer2 = oCalendarShift.EndLayer2;
            } else {
                edittingShiftDefinition.EndLayer2 = moment(edittingShiftDefinition.StartLayer2).add(layer2Duration, 'minutes').toDate();
            }
        }
    } else {
        edittingShiftDefinition.StartLayer2 = oCalendarShift.StartLayer2;
        edittingShiftDefinition.EndLayer2 = oCalendarShift.EndLayer2;
    }

    return edittingShiftDefinition;
};

Robotics.Client.Controls.Forms.ComplementaryForm.prototype.canChangeShift = function (bBelongsToBuget, sourceDefinition, targetShift, targetAssignment) {
    if (!bBelongsToBuget) return true;

    if (targetShift != null && targetAssignment != null && sourceDefinition.MainShift.ID == targetShift.ID && sourceDefinition.AssigData.ID == targetAssignment.ID) {
        if (sourceDefinition.MainShift.PlannedHours != targetShift.PlannedHours) return false;

        if (!moment(sourceDefinition.MainShift.StartHour).isSame(moment(targetShift.StartHour))) return false;
    } else {
        return false;
    }

    return true;
};

Robotics.Client.Controls.Forms.ComplementaryForm.prototype.isValid = function () {
    var tmpData = this;
    var bIsValid = true;
    var duration = -1;
    var ordinaryHours = -1;
    var complementaryHours = -1;

    var startHours = -1;
    var endHours = -1;

    var startHours2 = -1;
    var endHours2 = -1;

    var startLayer1 = null;
    var startLayer2 = null;

    var endLayer1 = null;
    var endLayer2 = null;

    let layer1Duration = moment(this.oCalendarShift.EndLayer1).diff(moment(this.oCalendarShift.StartLayer1), "minutes");
    let layer2Duration = moment(this.oCalendarShift.EndLayer2).diff(moment(this.oCalendarShift.StartLayer2), "minutes");

    //Verificamos los datos de la primera franja, si existe datos
    if (this.oCalendarShift.IDLayer1 > 0) {
        var ordinaryDate = moment(tmpData.getDateIfNullFromControl(tmpData.txtShiftOrdinary1));
        var complentaryDate = moment(tmpData.getDateIfNullFromControl(tmpData.txtShiftComplementary1));
        var startDate = null;
        var endDate = moment(tmpData.oCalendarShift.EndLayer1);

        var newStartDate = 30;
        if (this.cmbStartAt1.GetSelectedItem().value == 0) {
            newStartDate = 29;
        } else if (this.cmbStartAt1.GetSelectedItem().value == 2) {
            newStartDate = 31;
        }
        startDate = moment(new Date(1999, 11, newStartDate, tmpData.getDateIfNullFromControl(tmpData.txtShiftStart1).getHours(), tmpData.getDateIfNullFromControl(tmpData.txtShiftStart1).getMinutes(), 0, 0));
        startLayer1 = startDate.toDate();

        var midnight = ordinaryDate.clone().startOf('day');

        ordinaryHours = ordinaryDate.clone().diff(midnight, 'minutes');
        complementaryHours = complentaryDate.clone().diff(midnight, 'minutes');

        if (this.oCalendarShift.AllowModifyDuration1) {
            endDate = startDate.clone().add(ordinaryHours + complementaryHours, 'minutes');
        } else {
            if (this.oCalendarShift.AllowModifyIniHour1) {
                if (this.oCalendarShift.HasLayer1FixedEnd) {
                    endDate = startDate.clone().add(moment(this.oCalendarShift.EndLayer1).diff(moment(this.oCalendarShift.StartLayer1), 'minutes'), 'minutes');
                } else {
                    endDate = startDate.clone().add(layer1Duration, 'minutes');
                }
            } else {
                endDate = startDate.clone().add(layer1Duration, 'minutes');
            }
        }

        endLayer1 = endDate.toDate();

        startHours = startDate.clone().diff(midnight, 'minutes');
        endHours = endDate.clone().diff(midnight, 'minutes');

        //Si tiene un periodo fijo
        if ((!this.oCalendarShift.AllowModifyDuration1) && (!this.oCalendarShift.AllowModifyIniHour1)) {
            //la suma de ordinarias y complementarias debe ser igual que el periodo de la franja
            if ((ordinaryHours + complementaryHours) != (endHours - startHours)) {
                bIsValid = false;
                this.showErrorPopup("Error.Title", "error", "Calendar.Client.InvalidDataNoDurationNoIniDurationF1", "", "Error.OK", "Error.OKDesc", "");
                return bIsValid;
            }
        }

        //Si tiene un periodo fijo
        if ((!this.oCalendarShift.AllowModifyDuration1) && (this.oCalendarShift.AllowModifyIniHour1)) {
            //la suma de ordinarias y complementarias debe ser igual que el periodo de la franja
            if ((ordinaryHours + complementaryHours) != (endHours - startHours)) {
                bIsValid = false;
                this.showErrorPopup("Error.Title", "error", "Calendar.Client.InvalidDataNoDurationYesIniDurationF1", "", "Error.OK", "Error.OKDesc", "");
                return bIsValid;
            }
        }

        //Si tiene un periodo fijo
        if ((this.oCalendarShift.AllowModifyDuration1) && (!this.oCalendarShift.AllowModifyIniHour1)) {
            //la suma de ordinarias y complementarias debe ser igual que el periodo de la franja
            if ((ordinaryHours + complementaryHours) != (endHours - startHours)) {
                bIsValid = false;
                this.showErrorPopup("Error.Title", "error", "Calendar.Client.InvalidDataYesDurationNoIniDurationF1", "", "Error.OK", "Error.OKDesc", "");
                return bIsValid;
            }
        }

        //Si tiene un periodo fijo
        if ((this.oCalendarShift.AllowModifyDuration1) && (this.oCalendarShift.AllowModifyIniHour1)) {
            //la suma de ordinarias y complementarias debe ser igual que el periodo de la franja
            if ((ordinaryHours + complementaryHours) <= 0) {
                bIsValid = false;
                this.showErrorPopup("Error.Title", "error", "Calendar.Client.InvalidDataYesDurationYesIniDurationF1", "", "Error.OK", "Error.OKDesc", "");
                return bIsValid;
            }
        }
    }

    //Verificamos los datos de la segunda franja, si existen datos
    if (this.oCalendarShift.IDLayer2 > 0) {
        var ordinaryDate = moment(tmpData.getDateIfNullFromControl(tmpData.txtShiftOrdinary2));
        var complentaryDate = moment(tmpData.getDateIfNullFromControl(tmpData.txtShiftComplementary2));
        var startDate = null;
        var endDate = moment(tmpData.oCalendarShift.EndLayer2);

        var newStartDate = 30;
        if (this.cmbStartAt2.GetSelectedItem().value == 0) {
            newStartDate = 29;
        } else if (this.cmbStartAt2.GetSelectedItem().value == 2) {
            newStartDate = 31;
        }
        startDate = moment(new Date(1999, 11, newStartDate, tmpData.getDateIfNullFromControl(tmpData.txtShiftStart2).getHours(), tmpData.getDateIfNullFromControl(tmpData.txtShiftStart2).getMinutes(), 0, 0));
        startLayer2 = startDate.toDate();

        var midnight = ordinaryDate.clone().startOf('day');

        ordinaryHours = ordinaryDate.clone().diff(midnight, 'minutes');
        complementaryHours = complentaryDate.clone().diff(midnight, 'minutes');

        if (this.oCalendarShift.AllowModifyDuration2) {
            endDate = startDate.clone().add(ordinaryHours + complementaryHours, 'minutes');
        } else {
            if (this.oCalendarShift.AllowModifyIniHour2) {
                if (this.oCalendarShift.HasLayer2FixedEnd) {
                    endDate = startDate.clone().add(moment(this.oCalendarShift.EndLayer2).diff(moment(this.oCalendarShift.StartLayer2), 'minutes'), 'minutes');
                } else {
                    endDate = startDate.clone().add(layer2Duration, 'minutes');
                }
            } else {
                endDate = startDate.clone().add(layer2Duration, 'minutes');
            }
        }
        endLayer2 = endDate.toDate();

        startHours = startDate.clone().diff(midnight, 'minutes');
        endHours = endDate.clone().diff(midnight, 'minutes');

        //Si tiene un periodo fijo
        if ((!this.oCalendarShift.AllowModifyDuration2) && (!this.oCalendarShift.AllowModifyIniHour2)) {
            //la suma de ordinarias y complementarias debe ser igual que el periodo de la franja
            if ((ordinaryHours + complementaryHours) != (endHours - startHours)) {
                bIsValid = false;
                this.showErrorPopup("Error.Title", "error", "Calendar.Client.InvalidDataNoDurationNoIniDurationF2", "", "Error.OK", "Error.OKDesc", "");
                return bIsValid;
            }
        }

        //Si tiene un periodo fijo
        if ((!this.oCalendarShift.AllowModifyDuration2) && (this.oCalendarShift.AllowModifyIniHour2)) {
            //la suma de ordinarias y complementarias debe ser igual que el periodo de la franja
            if ((ordinaryHours + complementaryHours) != (endHours - startHours)) {
                bIsValid = false;
                this.showErrorPopup("Error.Title", "error", "Calendar.Client.InvalidDataNoDurationYesIniDurationF2", "", "Error.OK", "Error.OKDesc", "");
                return bIsValid;
            }
        }

        //Si tiene un periodo fijo
        if ((this.oCalendarShift.AllowModifyDuration2) && (!this.oCalendarShift.AllowModifyIniHour2)) {
            //la suma de ordinarias y complementarias debe ser igual que el periodo de la franja
            if ((ordinaryHours + complementaryHours) != (endHours - startHours)) {
                bIsValid = false;
                this.showErrorPopup("Error.Title", "error", "Calendar.Client.InvalidDataYesDurationNoIniDurationF2", "", "Error.OK", "Error.OKDesc", "");
                return bIsValid;
            }
        }

        //Si tiene un periodo fijo
        if ((this.oCalendarShift.AllowModifyDuration2) && (this.oCalendarShift.AllowModifyIniHour2)) {
            //la suma de ordinarias y complementarias debe ser igual que el periodo de la franja
            if ((ordinaryHours + complementaryHours) <= 0) {
                bIsValid = false;
                this.showErrorPopup("Error.Title", "error", "Calendar.Client.InvalidDataYesDurationYesIniDurationF2", "", "Error.OK", "Error.OKDesc", "");
                return bIsValid;
            }
        }
    }

    //En el caso que existan 2 franjas verificamos que no se solapen
    if (this.oCalendarShift.IDLayer2 > 0 && this.oCalendarShift.IDLayer1 > 0) {
        var newStartDate = 30;
        if (this.cmbStartAt1.GetSelectedItem().value == 0) {
            newStartDate = 29;
        } else if (this.cmbStartAt1.GetSelectedItem().value == 2) {
            newStartDate = 31;
        }

        //periodo franja 1
        var startLayerTime1 = new Date(1900, 0, 1, 0, 0, 0, 0);
        startLayerTime1 = new Date(1999, 11, newStartDate, tmpData.getDateIfNullFromControl(tmpData.txtShiftStart1).getHours(), tmpData.getDateIfNullFromControl(tmpData.txtShiftStart1).getMinutes(), 0, 0);

        var endLayerTime1 = new Date(1900, 0, 1, 0, 0, 0, 0);
        endLayerTime1 = new Date(1999, 11, newStartDate, this.oCalendarShift.EndLayer1.getHours(), this.oCalendarShift.EndLayer1.getMinutes(), 0, 0);

        if (this.oCalendarShift.AllowModifyDuration1) {
            var ordinaryDate = moment(tmpData.getDateIfNullFromControl(tmpData.txtShiftOrdinary1));
            var complentaryDate = moment(tmpData.getDateIfNullFromControl(tmpData.txtShiftComplementary1));
            var midnight = ordinaryDate.clone().startOf('day');

            ordinaryHours = ordinaryDate.clone().diff(midnight, 'minutes');
            complementaryHours = complentaryDate.clone().diff(midnight, 'minutes');

            endLayerTime1 = startDate.clone().add(ordinaryHours + complementaryHours, 'minutes').toDate();
        }

        startHours = moment(startLayerTime1).clone().diff(midnight, 'minutes');
        endHours = moment(endLayerTime1).clone().diff(midnight, 'minutes');

        if (this.oCalendarShift.AllowModifyDuration1) {
            endHours = moment(tmpData.getDateIfNullFromControl(tmpData.txtShiftOrdinary1)).diff(midnight, "minutes") + moment(tmpData.getDateIfNullFromControl(tmpData.txtShiftComplementary1)).diff(midnight, "minutes");
            endHours = endHours + moment(startLayerTime1).diff(midnight, "minutes");
        }

        //periodo franja 2

        newStartDate = 30;
        if (this.cmbStartAt2.GetSelectedItem().value == 0) {
            newStartDate = 29;
        } else if (this.cmbStartAt2.GetSelectedItem().value == 2) {
            newStartDate = 31;
        }

        var startLayerTime2 = new Date(1900, 0, 1, 0, 0, 0, 0);
        startLayerTime2 = new Date(1999, 11, newStartDate, tmpData.getDateIfNullFromControl(tmpData.txtShiftStart2).getHours(), tmpData.getDateIfNullFromControl(tmpData.txtShiftStart2).getMinutes(), 0, 0);

        startHours2 = moment(startLayerTime2).clone().diff(midnight, 'minutes');

        if (endHours >= startHours2) {
            bIsValid = false;
            this.showErrorPopup("Error.Title", "error", "Calendar.Client.OverlappedLayers", "", "Error.OK", "Error.OKDesc", "");
            return bIsValid;
        }
    }

    for (var i = 0; i < this.oCalendarShift.BreakLayers.length; i++) {
        var bBreakAllowed = false;

        var realBreakStart = null;
        var realBreakEnd = null;

        if (startLayer1 != null) {
            if (moment(this.oCalendarShift.BreakLayers[i].Start).get('date') == 1) {
                var tmpStart = moment(this.oCalendarShift.BreakLayers[i].Start);
                var tmpFinish = moment(this.oCalendarShift.BreakLayers[i].Finish);
                realBreakStart = moment(startLayer1).add(tmpStart.get('hour'), 'hours').add(tmpStart.get('minute'), 'minutes').toDate();
                realBreakEnd = moment(startLayer1).add(tmpFinish.get('hour'), 'hours').add(tmpFinish.get('minute'), 'minutes').toDate();
            } else {
                realBreakStart = this.oCalendarShift.BreakLayers[i].Start;
                realBreakEnd = this.oCalendarShift.BreakLayers[i].Finish;
            }
            bBreakAllowed = moment(realBreakStart).isBetween(moment(startLayer1), moment(endLayer1), null, '[]')
            bBreakAllowed = bBreakAllowed && moment(realBreakEnd).isBetween(moment(startLayer1), moment(endLayer1), null, '[]')
        }

        if (!bBreakAllowed && startLayer2 != null) {
            if (moment(this.oCalendarShift.BreakLayers[i].Start).get('date') == 1) {
                var tmpStart = moment(this.oCalendarShift.BreakLayers[i].Start);
                var tmpFinish = moment(this.oCalendarShift.BreakLayers[i].Finish);
                realBreakStart = moment(startLayer1).add(tmpStart.get('hour'), 'hours').add(tmpStart.get('minute'), 'minutes').toDate();
                realBreakEnd = moment(startLayer1).add(tmpFinish.get('hour'), 'hours').add(tmpFinish.get('minute'), 'minutes').toDate();
            } else {
                realBreakStart = this.oCalendarShift.BreakLayers[i].Start;
                realBreakEnd = this.oCalendarShift.BreakLayers[i].Finish;
            }
            bBreakAllowed = moment(realBreakStart).isBetween(moment(startLayer2), moment(endLayer2), null, '[]')
            bBreakAllowed = bBreakAllowed && moment(realBreakEnd).isBetween(moment(startLayer2), moment(endLayer2), null, '[]')
        }

        if (!bBreakAllowed && (startLayer1 != null || startLayer2 != null)) {
            bIsValid = false;
            this.showErrorPopup("Error.Title", "error", "Calendar.Client.BreakNotFit", "", "Error.OK", "Error.OKDesc", "");
            return bIsValid;
        }
    }

    return bIsValid;
};