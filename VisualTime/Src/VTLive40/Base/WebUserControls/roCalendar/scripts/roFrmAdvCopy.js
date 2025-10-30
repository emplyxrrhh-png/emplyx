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

Robotics.Client.Controls.Forms.AdvCopyForm = function (prefix, calendarType) {
    this.prefix = prefix;
    this.workMode = calendarType;

    this.labelCounter = $("#" + this.prefix + '_shiftCount');
    this.dateInfo = $("#" + this.prefix + '_shiftStartDate');

    this.rbRepeatUntil = eval(this.prefix + "_rbRepeatUntilClient");
    this.txtToDate = eval(this.prefix + "_txtToDateClient");

    this.rbRepeatTimes = eval(this.prefix + "_rbRepeatTimesClient");
    this.txtRepeatTimes = eval(this.prefix + "_txtRepeatTimesClient");

    this.ckBloqDestDays = eval(this.prefix + "_ckBloqDestDaysClient");

    this.rbAdvancedRepeatDisabled = eval(this.prefix + "_rbAdvancedRepeatDisabledClient");
    this.rbAdvancedRepeatEnabled = eval(this.prefix + "_rbAdvancedRepeatEnabledClient");

    this.rbAdvancedBloquedDisabled = eval(this.prefix + "_rbAdvancedBloquedDisabledClient");
    this.rbAdvancedBloquedEnabled = eval(this.prefix + "_rbAdvancedBloquedEnabledClient");

    this.rbAdvancedHolidaysDisabled = eval(this.prefix + "_rbAdvancedHolidaysDisabledClient");
    this.rbAdvancedHolidaysEnabled = eval(this.prefix + "_rbAdvancedHolidaysEnabledClient");

    this.rbStartsInmediately = eval(this.prefix + "_rbStartsInmediately");
    this.rbStartsNextDay = eval(this.prefix + "_rbStartsNextDayClient");
    this.cmbRepeatStartsDay = eval(this.prefix + "_cmbRepeatStartsDayClient");
    this.rbStartsNextMonth = eval(this.prefix + "_rbStartsNextMonthClient");
    this.txtRepeatStartsMonth = eval(this.prefix + "_txtRepeatStartsMonthClient");

    this.ckSkipOptions = eval(this.prefix + "_ckSkipOptionsClient");
    this.txtSkipOptions = eval(this.prefix + "_txtSkipOptionsClient");
    this.rbRepeatSkipWeek = eval(this.prefix + "_rbRepeatSkipWeekClient");
    this.cmbSkipsWeekDay = eval(this.prefix + "_cmbSkipsWeekDayClient");
    this.rbRepeatSkipMonth = eval(this.prefix + "_rbRepeatSkipMonthClient");
    this.txtSkipMonthDayValue = eval(this.prefix + "_txtSkipMonthDayValueClient");
    this.rbRepeatSkipDays = eval(this.prefix + "_rbRepeatSkipDaysClient");
    this.txtRepeatSkipDays = eval(this.prefix + "_txtRepeatSkipDaysClient");

    this.rbHolidaySkip = eval(this.prefix + "_rbHolidaySkipClient");
    this.rbHolidayIgnore = eval(this.prefix + "_rbHolidayIgnoreClient");
    this.rbHolidayOverwrite = eval(this.prefix + "_rbHolidayOverwriteClient");

    this.rbBloquedSkip = eval(this.prefix + "_rbBloquedSkipClient");
    this.rbBloquedIgnore = eval(this.prefix + "_rbBloquedIgnoreClient");
    this.rbBloquedOverWrite = eval(this.prefix + "_rbBloquedOverWriteClient");

    this.rbTelecommuteKeep = eval(this.prefix + "_rbTelecommuteKeepClient");
    this.rbTelecommuteCopy = eval(this.prefix + "_rbTelecommuteCopyClient");
    this.rbTelecommuteDefault = eval(this.prefix + "_rbTelecommuteDefaultClient");
};

Robotics.Client.Controls.Forms.AdvCopyForm.prototype.prepareForm = function (countDays, startDate, typeView) {
    this.rbRepeatUntil.SetChecked(false);

    this.labelCounter.html(countDays);
    this.dateInfo.html(moment(startDate).format('DD/MM/YYYY'));

    var now = new Date();
    this.txtToDate.SetDate(moment(startDate).clone().toDate());

    this.rbRepeatTimes.SetChecked(true);
    this.txtRepeatTimes.SetValue(1);

    this.ckBloqDestDays.SetChecked(false);

    this.rbAdvancedRepeatDisabled.SetChecked(true);
    this.rbAdvancedRepeatEnabled.SetChecked(false);

    this.rbAdvancedBloquedDisabled.SetChecked(true);
    this.rbAdvancedBloquedEnabled.SetChecked(false);

    this.rbAdvancedHolidaysDisabled.SetChecked(true);
    this.rbAdvancedHolidaysEnabled.SetChecked(false);

    this.rbStartsInmediately.SetChecked(true);
    this.rbStartsNextDay.SetChecked(false);
    this.cmbRepeatStartsDay.SetSelectedItem(this.cmbRepeatStartsDay.FindItemByValue("1"));
    this.rbStartsNextMonth.SetChecked(false);
    this.txtRepeatStartsMonth.SetValue(1);

    this.ckSkipOptions.SetChecked(false);
    this.txtSkipOptions.SetValue(1);
    this.rbRepeatSkipWeek.SetChecked(true);
    this.cmbSkipsWeekDay.SetSelectedItem(this.cmbSkipsWeekDay.FindItemByValue("1"));
    this.rbRepeatSkipMonth.SetChecked(false);
    this.txtSkipMonthDayValue.SetValue(1);
    this.rbRepeatSkipDays.SetChecked(false);
    this.txtRepeatSkipDays.SetValue(1);

    this.rbHolidaySkip.SetChecked(true);
    this.rbHolidayIgnore.SetChecked(false);
    this.rbHolidayOverwrite.SetChecked(false);

    this.rbBloquedSkip.SetChecked(true);
    this.rbBloquedIgnore.SetChecked(false);
    this.rbBloquedOverWrite.SetChecked(false);

    this.rbTelecommuteKeep.SetChecked(true);
    this.rbTelecommuteCopy.SetChecked(false);
    this.rbTelecommuteDefault.SetChecked(false);

    switch (this.workMode) {
        case Robotics.Client.Constants.WorkMode.roCalendar:
            $('#' + this.prefix + '_divRepeatOptions').show();
            $('#' + this.prefix + '_divBloquedDaysOptions').show();
            $('#' + this.prefix + '_RoGroupBox3_divBlockDestinationDays').show();
            $('#' + this.prefix + '_RoGroupBox2_divHolidaysIgnore').show();
            $('#' + this.prefix + '_divBudgetWarning').hide();

            break;
        case Robotics.Client.Constants.WorkMode.roBudget:
            $('#' + this.prefix + '_divRepeatOptions').hide();
            $('#' + this.prefix + '_divBloquedDaysOptions').hide();
            $('#' + this.prefix + '_RoGroupBox3_divBlockDestinationDays').hide();
            $('#' + this.prefix + '_RoGroupBox2_divHolidaysIgnore').hide();

            switch (typeView) {
                case Robotics.Client.Constants.TypeView.Planification:
                    $('#' + this.prefix + '_divHolidaysOptions').show();
                    $('#' + this.prefix + '_divBudgetWarning').show();
                    break;
                case Robotics.Client.Constants.TypeView.Definition:
                    $('#' + this.prefix + '_divHolidaysOptions').hide();
                    $('#' + this.prefix + '_divBudgetWarning').hide();
                    break;
            }

            break;
        default:
            break;
    }
};

Robotics.Client.Controls.Forms.AdvCopyForm.prototype.getFiltersValue = function () {
    var filters = {}

    if (this.rbRepeatUntil.GetChecked()) {
        filters.RepeatMode = 1;
        filters.RepeatModeValue = moment(this.txtToDate.GetDate()).format('YYYY/MM/DD');
    }

    if (this.rbRepeatTimes.GetChecked()) {
        filters.RepeatMode = 0;
        filters.RepeatModeValue = this.txtRepeatTimes.GetValue();
    }

    this.ckBloqDestDays.GetChecked() ? filters.LockDestDays = 1 : filters.LockDestDays = 0;

    if (this.rbAdvancedRepeatDisabled.GetChecked()) {
        filters.RepeatStartMode = 0;
        filters.RepeatStartModeValue = '';
        filters.RepeatSkipMode = 0;
        filters.RepeatSkipTimes = 0;
        filters.RepeatSkipModeValue = '';
    } else {
        if (this.rbStartsInmediately.GetChecked()) {
            filters.RepeatStartMode = 0;
            filters.RepeatStartModeValue = '';
        } else if (this.rbStartsNextDay.GetChecked()) {
            filters.RepeatStartMode = 1;
            filters.RepeatStartModeValue = this.cmbRepeatStartsDay.GetSelectedItem().value;
        } else if (this.rbStartsNextMonth.GetChecked()) {
            filters.RepeatStartMode = 2;
            filters.RepeatStartModeValue = this.txtRepeatStartsMonth.GetValue();
        }

        if (this.ckSkipOptions.GetChecked()) {
            filters.RepeatSkipTimes = this.txtSkipOptions.GetValue();
            if (this.rbRepeatSkipWeek.GetChecked()) {
                filters.RepeatSkipMode = 1;
                filters.RepeatSkipModeValue = this.cmbSkipsWeekDay.GetSelectedItem().value;
            } else if (this.rbRepeatSkipMonth.GetChecked()) {
                filters.RepeatSkipMode = 2;
                filters.RepeatSkipModeValue = this.txtSkipMonthDayValue.GetValue();
            } else if (this.rbRepeatSkipDays.GetChecked()) {
                filters.RepeatSkipMode = 3;
                filters.RepeatSkipModeValue = this.txtRepeatSkipDays.GetValue();
            }
        } else {
            filters.RepeatSkipMode = 0;
            filters.RepeatSkipTimes = 0;
            filters.RepeatSkipModeValue = '';
        }
    }

    if (!this.rbAdvancedHolidaysDisabled.GetChecked()) {
        if (this.rbHolidaySkip.GetChecked()) {
            filters.HolidaysMode = 0;
        } else if (this.rbHolidayIgnore.GetChecked()) {
            filters.HolidaysMode = 1;
        } else if (this.rbHolidayOverwrite.GetChecked()) {
            filters.HolidaysMode = 2;
        }
    } else {
        filters.HolidaysMode = 0;
    }

    if (!this.rbAdvancedBloquedDisabled.GetChecked()) {
        if (this.rbBloquedSkip.GetChecked()) {
            filters.BloquedMode = 0;
        } else if (this.rbBloquedIgnore.GetChecked()) {
            filters.BloquedMode = 1;
        } else if (this.rbBloquedOverWrite.GetChecked()) {
            filters.BloquedMode = 2;
        }
    } else {
        filters.BloquedMode = 0;
    }

    if (this.rbTelecommuteKeep.GetChecked()) {
        filters.TelecommuteCopy = 0;
    } else if (this.rbTelecommuteCopy.GetChecked()) {
        filters.TelecommuteCopy = 1
    } else if (this.rbTelecommuteDefault.GetChecked()) {
        filters.TelecommuteCopy = 2;
    }

    return filters;
};

function ckTelecommuteVisibilityChanged(s, e) {
    if (ctl00_contentMainBody_oCalendar_dlgAdvCopy_rbTelecommuteKeepClient.GetChecked()) {
        //this.rbAdvancedHolidaysEnabled.SetChecked(false);

        ctl00_contentMainBody_oCalendar_dlgAdvCopy_rbAdvancedHolidaysDisabledClient.SetEnabled(true);
        ctl00_contentMainBody_oCalendar_dlgAdvCopy_rbAdvancedHolidaysEnabledClient.SetEnabled(true);
    } else {
        ctl00_contentMainBody_oCalendar_dlgAdvCopy_rbAdvancedHolidaysDisabledClient.SetChecked(true);
        //ctl00_contentMainBody_oCalendar_dlgAdvCopy_rbAdvancedHolidaysDisabled.SetChecked(true);
        //ctl00_contentMainBody_oCalendar_dlgAdvCopy_rbAdvancedHolidaysEnabled.SetChecked(false);

        ctl00_contentMainBody_oCalendar_dlgAdvCopy_rbAdvancedHolidaysDisabledClient.SetEnabled(false);
        ctl00_contentMainBody_oCalendar_dlgAdvCopy_rbAdvancedHolidaysEnabledClient.SetEnabled(false);
    }
};