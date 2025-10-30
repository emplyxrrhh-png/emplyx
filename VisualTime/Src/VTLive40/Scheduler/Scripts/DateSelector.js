var planView = null;
var rangeDates = null;
var localLanguage = 'es';
var localFormat = 'DD-MM-YYYY';
var localseparator = ' hasta ';
var currentRangeSelection = null;
var forceRefresh = true;

function LoadDateSelector(languageVT) {
    clearCalendars();
    switch (languageVT) {
        case "ESP":
            localLanguage = 'es';
            localFormat = 'DD-MM-YYYY';
            localseparator = ' hasta ';
            break;
        case "CAT":
            localLanguage = 'ca';
            localFormat = 'DD-MM-YYYY';
            localseparator = ' fins ';
            break;
        case "POR":
            localLanguage = 'pt';
            localFormat = 'DD-MM-YYYY';
            localseparator = ' até ';
            break;
        case "GAL":
            localLanguage = 'gl';
            localFormat = 'DD-MM-YYYY';
            localseparator = ' ata ';
            break;
        case "ITA":
            localLanguage = 'it';
            localFormat = 'DD-MM-YYYY';
            localseparator = ' fino a ';
            break;
        case "FRA":
            localLanguage = 'fr';
            localFormat = 'DD-MM-YYYY';
            localseparator = ' au ';
            break;
        case "ENG":
        default:
            localLanguage = 'en';
            localFormat = 'M-DD-YYYY';
            localseparator = ' to ';
            break;
    }

    planView = localStorage.getItem('PlanView') || "5";
    var rangeDates = localStorage.getItem('SchedulerIntervalDates') || "0";

    moment.locale(localLanguage);
    var startDate = null;
    var endDate = null;
    var arrayDates = null;
    if (rangeDates != "0") {
        arrayDates = rangeDates.split(',');
        arrayDates.forEach(function (item, index) {
            if (item.indexOf('#') > -1) {
                if (startDate == null)
                    startDate = convertDateIn(item)
                else
                    endDate = convertDateIn(item)
            }
        });

        if (arrayDates.length <= 1) {
            endDate = startDate;
        }
    }
    else {
        startDate = moment();
        endDate = moment();
    }

    $('#divDayCalendar').dateRangePicker(
        {
            language: localLanguage,
            separator: localseparator,
            format: localFormat,
            autoClose: true,
            startOfWeek: 'monday',
            customTopBar: window.parent.Globalize.formatMessage("SELECT_DAY"),
            singleMonth: true,
            singleDate: true,
            setValue: function (s) {
                manageText(s, 0, $('#divDayCalendar'))
            }
        });

    $('#divWeekCalendar').dateRangePicker(
        {
            language: localLanguage,
            separator: localseparator,
            format: localFormat,
            autoClose: true,
            startOfWeek: 'monday',
            batchMode: 'week',
            showShortcuts: false,
            setValue: function (s) {
                manageText(s, 1, $('#divWeekCalendar'))
            }
        });

    $('#divTwoCalendar').dateRangePicker(
        {
            language: localLanguage,
            separator: localseparator,
            format: localFormat,
            autoClose: true,
            startOfWeek: 'monday',
            batchMode: 'twoWeek',
            showShortcuts: false,
            setValue: function (s) {
                manageText(s, 2, $('#divTwoCalendar'))
            }
        });

    $('#divMonthCalendar').dateRangePicker(
        {
            language: localLanguage,
            separator: localseparator,
            format: localFormat,
            autoClose: true,
            startOfWeek: 'monday',
            batchMode: 'month',
            showShortcuts: false,
            setValue: function (s) {
                manageText(s, 3, $('#divMonthCalendar'))
            }
        });

    $('#divFreeCalendar').dateRangePicker(
        {
            language: localLanguage,
            separator: localseparator,
            format: localFormat,
            autoClose: true,
            startOfWeek: 'monday',
            setValue: function (s) {
                manageText(s, 4, $('#divFreeCalendar'))
            }
        });

    switch (planView) {
        case "0":
            $('#divDayCalendar').data('dateRangePicker').setDateRange(moment(startDate, localFormat).format(localFormat), moment(endDate, localFormat).format(localFormat));
            break;
        case "1":
            $('#divWeekCalendar').data('dateRangePicker').setDateRange(moment(startDate, localFormat).format(localFormat), moment(endDate, localFormat).format(localFormat));
            break;
        case "2":
            $('#divTwoCalendar').data('dateRangePicker').setDateRange(moment(startDate, localFormat).format(localFormat), moment(endDate, localFormat).format(localFormat));
            break;
        case "3":
            $('#divMonthCalendar').data('dateRangePicker').setDateRange(moment(startDate, localFormat).format(localFormat), moment(endDate, localFormat).format(localFormat));
            break;
        case "4":
            $('#divFreeCalendar').data('dateRangePicker').setDateRange(moment(startDate, localFormat).format(localFormat), moment(endDate, localFormat).format(localFormat));
            break;
        default:
            startDate = moment(startDate, localFormat).startOf('isoweek').format(localFormat);
            endDate = moment(endDate, localFormat).add(1, 'week').endOf('isoweek').format(localFormat);
            $('#divTwoCalendar').data('dateRangePicker').setDateRange(startDate, endDate);
            break;
    }
}

function refreshCalendar(rangeText, forceLoad) {
    var force = false;

    if (typeof (rangeText) == 'undefined' || rangeText == '') {
        rangeText = currentRangeSelection;
    }

    if (typeof (forceLoad) != 'undefined') {
        force = forceLoad;
    }

    var startDate = new Date()
    var endDate = new Date()
    var arrayDates = null;

    arrayDates = rangeText.split(' ');
    if (arrayDates.length > 2) {
        startDate = arrayDates[0];
        endDate = arrayDates[2];
    }
    else {
        startDate = arrayDates[0];
        endDate = arrayDates[0];
    }
    if (roScheduleCalendar != null) {
        var calFilters = (localStorage.getItem('CalendarLoadRecursive') || "false@@0@false@false").split('@');

        if (force == true) roScheduleCalendar.hasChanges = false;
        roScheduleCalendar.loadData(moment(startDate, localFormat).toDate(), moment(endDate, localFormat).toDate(), getEmployeeFilter(), calFilters[0] == 'false' ? false : true, actualTab, calFilters[1], typeof calFilters[3] == 'undefined' ? false : (calFilters[3] == 'false' ? false : true), typeof calFilters[4] == 'undefined' ? false : (calFilters[4] == 'false' ? false : true));
    }
}

function SchedulerNavigateV2(oNavigate) {
    try {
        planView = localStorage.getItem('PlanView') || "0";
        rangeDates = localStorage.getItem('SchedulerIntervalDates') || "0";

        var startDate = new Date()
        var endDate = new Date()
        var arrayDates = null;

        arrayDates = txtDateRangeClient.GetText().split(' ');
        if (arrayDates.length == 3) {
            startDate = arrayDates[0];
            endDate = arrayDates[2];
        }
        else {
            startDate = arrayDates[0];
            endDate = arrayDates[0];
        }

        if (oNavigate.toUpperCase() === "PREVIOUS") {
            switch (planView) {
                case "0":
                case "4":
                    startDate = moment(startDate, localFormat).add(-1, 'day');
                    endDate = moment(endDate, localFormat).add(-1, 'day');
                    if (planView === "0")
                        $('#divDayCalendar').data('dateRangePicker').setDateRange(startDate.format(localFormat), endDate.format(localFormat));
                    else
                        $('#divFreeCalendar').data('dateRangePicker').setDateRange(startDate.format(localFormat), endDate.format(localFormat));
                    break;
                case "1":
                    startDate = moment(startDate, localFormat).add(-1, 'week');
                    endDate = moment(endDate, localFormat).add(-1, 'week');
                    $('#divWeekCalendar').data('dateRangePicker').setDateRange(startDate.format(localFormat), endDate.format(localFormat));
                    break;
                case "2":
                    startDate = moment(startDate, localFormat).add(-2, 'week');
                    endDate = moment(endDate, localFormat).add(-2, 'week');
                    $('#divTwoCalendar').data('dateRangePicker').setDateRange(startDate.format(localFormat), endDate.format(localFormat));
                    break;
                case "3":
                    startDate = moment(startDate, localFormat).add(-1, 'month');
                    endDate = moment(endDate, localFormat).add(-1, 'month');
                    $('#divMonthCalendar').data('dateRangePicker').setDateRange(startDate.format(localFormat), endDate.endOf('month').format(localFormat));
                    break;
                default:
                    $('#divDayCalendar').data('dateRangePicker').setDateRange(startDate, endDate);
                    break;
            }
        }
        else if (oNavigate.toUpperCase() === "NEXT") {
            switch (planView) {
                case "0":
                case "4":
                    startDate = moment(startDate, localFormat).add(1, 'day');
                    endDate = moment(endDate, localFormat).add(1, 'day');
                    if (planView === "0")
                        $('#divDayCalendar').data('dateRangePicker').setDateRange(startDate.format(localFormat), endDate.format(localFormat));
                    else
                        $('#divFreeCalendar').data('dateRangePicker').setDateRange(startDate.format(localFormat), endDate.format(localFormat));
                    break;
                case "1":
                    startDate = moment(startDate, localFormat).add(1, 'week');
                    endDate = moment(endDate, localFormat).add(1, 'week');
                    $('#divWeekCalendar').data('dateRangePicker').setDateRange(startDate.format(localFormat), endDate.format(localFormat));
                    break;
                case "2":
                    startDate = moment(startDate, localFormat).add(2, 'week');
                    endDate = moment(endDate, localFormat).add(2, 'week');
                    $('#divTwoCalendar').data('dateRangePicker').setDateRange(startDate.format(localFormat), endDate.format(localFormat));
                    break;
                case "3":
                    startDate = moment(startDate, localFormat).add(1, 'month');
                    endDate = moment(endDate, localFormat).add(1, 'month');;
                    $('#divMonthCalendar').data('dateRangePicker').setDateRange(startDate.format(localFormat), endDate.endOf('month').format(localFormat));
                    break;
                default:
                    $('#divDayCalendar').data('dateRangePicker').setDateRange(startDate, endDate);
                    break;
            }
        }
    }
    catch (e) {
        showError("DataSelector::SchedulerNavigateV2", e);
    }
}

function clearSelectorCss(controlActivate) {
    $('#divDayCalendar').css("background-color", "transparent");
    $('#divWeekCalendar').css("background-color", "transparent");
    $('#divTwoCalendar').css("background-color", "transparent");
    $('#divMonthCalendar').css("background-color", "transparent");
    $('#divFreeCalendar').css("background-color", "transparent");
    controlActivate.css("background-color", "#C7D2D6 ");
}

function manageText(text, plan, control) {
    txtDateRangeClient.SetText(text);
    var dateSeparator = text.split(localseparator);
    currentRangeSelection = text;
    if (forceRefresh) {
        refreshCalendar(text);
    }
    localStorage.setItem('PlanView', plan);
    if (dateSeparator[1] == null || dateSeparator[1] === '')
        convertDateOut(dateSeparator[0], dateSeparator[0])
    else
        convertDateOut(dateSeparator[0], dateSeparator[1])
    clearSelectorCss(control);
}

function convertDateIn(item) {
    var itemsDate = item.split('#');
    var testDate = moment(item, 'YYYY#MM#DD')
    return testDate.format(localFormat)
}

function convertDateOut(item1, item2) {
    var dateStart = moment(item1, localFormat).format('YYYY#MM#DD');
    var dateEnd = moment(item2, localFormat).format('YYYY#MM#DD');
    localStorage.setItem('SchedulerIntervalDates', dateStart + ',' + dateEnd);
}

function clearCalendars() {
    if ($('#divDayCalendar').data('dateRangePicker') != null)
        $('#divDayCalendar').data('dateRangePicker').destroy();
    if ($('#divWeekCalendar').data('dateRangePicker') != null)
        $('#divWeekCalendar').data('dateRangePicker').destroy();
    if ($('#divTwoCalendar').data('dateRangePicker') != null)
        $('#divTwoCalendar').data('dateRangePicker').destroy();
    if ($('#divMonthCalendar').data('dateRangePicker') != null)
        $('#divMonthCalendar').data('dateRangePicker').destroy();
    if ($('#divFreeCalendar').data('dateRangePicker') != null)
        $('#divFreeCalendar').data('dateRangePicker').destroy();
}