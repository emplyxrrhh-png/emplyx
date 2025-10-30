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

    namespace("Robotics.Client.Controls");
    namespace("Robotics.Client.Language");
    namespace("Robotics.Client.Constants");
}());

Robotics.Client.Controls.roCalendar = function (ascxPrefix, typeView, loadingFunction, showErrorFunction, calendarStateCookie) {
    this.showErrorPopup = showErrorFunction;

    this.workMode = parseInt($('#' + ascxPrefix + '_hdnCalendarWorkMode').val(), 10);
    this.clientInstanceName = $('#' + ascxPrefix + '_hdnClientInstanceName').val();

    //0 - Review, 1 - Planification
    this.typeView = typeView;
    this.viewRange = Robotics.Client.Constants.ViewRange.Period;

    var clientConfig = eval(ascxPrefix + '_hdnCalendarConfigClient');
    this.cConfig = clientConfig;

    //Dispone de licencia de HRScheduling
    this.isScheduleActive = clientConfig.Get('HRScheduling');
    //Dispone de licencia de SaasPremium
    this.saasPremiumActive = clientConfig.Get('SaasPremium');

    //Dispone de licencia Starter
    this.isStarter = clientConfig.Get('VTLiveEdition').toLowerCase() == "starter";

    //Dispone de licencia de Teletrabajo
    this.telecommuteEnabled = clientConfig.Get('TelecommuteEnabled');

    //this.isScheduleActive = false;
    this.languageKey = clientConfig.Get('Language');

    //Obtenemos el valor para avisar que estas cargando muchos registros de Calendario
    this.loadHugeDataValue = clientConfig.Get('MinimumCalendarDataWarning');

    this.translator = new Robotics.Client.Language.translator(clientConfig);

    //Prefijo del control
    this.ascxPrefix = ascxPrefix;
    this.prefix = ascxPrefix + '_roCalendarRender';

    //Contenedor del calendario
    this.container = $('#' + ascxPrefix + '_roCalendarRender');

    //Función que muestra el spinner de loading
    this.loadingFunc = loadingFunction;

    //Objecto donde se tienen que hacer las peticiones de datos mediante callbacks
    this.requestObject = eval(ascxPrefix + "_ASPxRoCalendarCallbackClient");

    this.setTypeView(this.typeView);

    // Layout que utiliza el calendario
    this.pageLayout = null;

    //Primera columna ocupada para la vista diaria
    this.firstCellPrinted = -1;

    this.remarksColor = ["#f00", "#ff0", "#228b22"];

    //Filtros del calendario
    this.firstDate = null;
    this.endDate = null;
    this.employeeFilter = "";
    this.calendarStateCookie = calendarStateCookie;

    var cookieValue = (localStorage.getItem(this.calendarStateCookie) || "false@@0@false@false@30").split('@');

    this.dailyPeriod = 30;
    if (cookieValue.length > 4 && typeof cookieValue[5] != 'undefined') this.dailyPeriod = parseInt(cookieValue[5], 10);
    this.loadRecursive = cookieValue[0] == 'false' ? false : true;
    this.assignmentsFilter = cookieValue[1];
    this.schedulingView = cookieValue[2] == '0' ? Robotics.Client.Constants.CoverageDayView.Planified : Robotics.Client.Constants.CoverageDayView.Real;
    this.loadIndictments = false;
    this.loadCapacities = false;
    this.loadPunches = false;
    if (typeof cookieValue[4] != 'undefined') cookieValue[4] == 'false' ? false : true;

    //Objecto con la estructura del calendario que nos da el servidor
    this.oCalendar = null;
    this.hasChanges = false;

    //Empleado - Empleado-Dia - DivSeleccionado Seleccionado pertenece a oCalendar
    this.selectedEmployee = null;
    this.selectedDay = null;
    this.selectedContainer = null;

    //Rango seleccionado fila - columna
    this.selectedMinRow = -1;
    this.selectedMaxRow = -1;

    this.selectedMinColumn = -1;
    this.selectedMaxColumn = -1;

    this.selectionCopied = false;

    //Copy paste header
    this.selectedHeaderContainer = null
    this.selectedMinHeaderColumn = -1;
    this.selectedMaxHeaderColumn = -1;;

    this.selectionHeaderCopied = false;

    // que copiamos
    this.copyWorkingShifts = false;
    this.copyHolidaysShifts = false;
    this.copyAssignmentsShifts = false;

    //Control de combinaciones de teclado
    this.ctrlDown = false;
    this.shiftDown = false;
    this.shiftKey = 16;
    this.ctrlKey = 17;
    this.vKey = 86;
    this.cKey = 67;

    //dialogo utilizado para opciones de copi
    this.copyDialog = this.createCopyDialog();
    this.advCopyDialog = this.createAdvancedCopyDialog();
    this.importDialog = this.createImportDialog();
    this.complementaryDialog = this.createComplementaryDialog();
    this.errorDialog = this.createErrorDialog(false);
    this.importErrorDialog = this.createErrorDialog(true);
    this.filterCalendarDialog = this.createFilterCalendarDialog();
    this.assignmentsDialog = this.createAssignmentsDialog();
    this.calendarSortDialog = this.createSortCalendarDialog();
    this.shiftsDialog = this.createShiftsDialog();
    this.pUnitsDialog = this.createPUnitsDialog();
    this.budgetEmployeesDialog = this.createBudgetEmployeesDialog();
    this.editDailyViewDialog = this.createDailyConfig();
    this.starterDialog = this.createStarterDialog();

    this.sourceDialogRow = -1;
    this.sourceDialogColumn = -1;
    this.files = null;

    this.showStarterDialog = false;
    this.showComplementaryAssignDialog = false;
    this.complementaryShift = null;
    this.showAssignmentsDialog = false;
    this.assignmentShift = null;
    this.starterShift = null;

    //timer de auto-refresco
    this.isShowingDialog = false;
    this.refreshTimmer = -1;
    this.timmerEnabled = true;
    this.isShowingLoader = false;

    this.capacityError = false;
    this.capatityEnabled = false;

    //listas de datos para totalizadores
    this.shiftsList = {};
    this.capacityList = {};
    this.assignmentsList = {};
    this.columnShiftsList = {};
    this.columnCapacityList = {};
    this.rowsShiftsList = {};
    this.employeeDataList = {};
    this.columnAssignmentsList = {};
    this.rowsAssignmentsList = {}
    this.accrualsTotal = {
        PlannedHours: { WorkingHours: 0, ComplementaryHours: 0 },
        HolidayResume: { AssignedHolidays: 0 }
    }

    //cache de horarios complementarios
    this.shiftsExtendedDataCache = {};
    this.starterManager = new Robotics.Client.Controls.Forms.StarterForm(this.ascxPrefix + '_dlgStarter', showErrorFunction);
    this.complementaryManager = new Robotics.Client.Controls.Forms.ComplementaryForm(this.ascxPrefix + '_dlgComplementary', showErrorFunction);
    this.assignmentsManager = new Robotics.Client.Controls.Forms.AssignmentsForm(this.ascxPrefix + '_dlgAssignments', showErrorFunction);
    this.shiftSelectorManager = new Robotics.Client.Controls.Forms.ShiftSelectorForm(this.ascxPrefix + '_dlgShiftSelector', showErrorFunction);
    this.pUnitSelectorManager = new Robotics.Client.Controls.Forms.PUnitSelectorForm(this.ascxPrefix + '_dlgProductiveUnitSelector', showErrorFunction);
    this.budgetEmployeesManager = new Robotics.Client.Controls.Forms.BudgetAddEmployeeForm(this.ascxPrefix + '_dlgBudgetAddEmployees', showErrorFunction);

    this.advCopyManager = new Robotics.Client.Controls.Forms.AdvCopyForm(this.ascxPrefix + '_dlgAdvCopy', this.workMode);

    this.onContinueFunc = '';
    this.defaultMessageAction = 'window.frames["ifPrincipal"].focus();';

    this.sortElements = 'assignment,group,employee,shift,budget';
    this.sortColumn = -1;
    //Informacion del calendario que esta cargando
    this.loadingInfo = {
        initializing: false,
        firstDate: undefined,
        endDate: undefined,
        employeeFilter: undefined,
        loadRecursive: undefined,
        typeView: undefined,
        calendarFilter: undefined,
        loadIndictments: undefined,
        loadPunches: undefined,
        loadCapacities: undefined
    };
    this.lastSelectionLoaded = {
        employeeFilter: "¬11110¬",
        loadRecursive: false,
        firstDate: moment().startOf('week').toDate(),
        endDate: moment().add(1, 'week').endOf('week').toDate(),
    };
};

Robotics.Client.Controls.roCalendar.prototype.setTypeView = function (newView) {
    this.typeView = newView;

    var oldClientmode = "";
    var newClientmode = "";

    if (this.clientMode != null) oldClientmode = this.clientMode.name;

    switch (this.workMode) {
        case Robotics.Client.Constants.WorkMode.roCalendar:
            this.clientMode = new Robotics.Client.Controls.roSchedulerCalendar(this);
            break;
        case Robotics.Client.Constants.WorkMode.roProductiveUnit:
            this.dailyPeriod = 30;
            this.clientMode = new Robotics.Client.Controls.roProductiveUnitCalendar(this);
            break;
        case Robotics.Client.Constants.WorkMode.roDayDetail:
            this.dailyPeriod = 30;
            if (this.typeView == Robotics.Client.Constants.TypeView.DayDetail) {
                this.clientMode = new Robotics.Client.Controls.roDayDetailCalendar(this);
            } else if (this.typeView == Robotics.Client.Constants.TypeView.DaySchedule) {
                this.clientMode = new Robotics.Client.Controls.roDayDetailSchedule(this);
            }
            break;
        case Robotics.Client.Constants.WorkMode.roBudget:
            this.dailyPeriod = 30;
            var oldMode = (this.clientMode != null ? this.clientMode.OnSelectedCell : null);
            var oldDetail = (this.clientMode != null ? this.clientMode.OnDayClick : null);

            if (this.typeView == Robotics.Client.Constants.TypeView.Definition) {
                this.clientMode = new Robotics.Client.Controls.roBudgetCalendarDefinition(this);
            } else if (this.typeView == Robotics.Client.Constants.TypeView.Planification) {
                this.clientMode = new Robotics.Client.Controls.roBudgetCalendarSchedule(this);
            } else if (this.typeView == Robotics.Client.Constants.TypeView.Detail) {
                this.clientMode = new Robotics.Client.Controls.roBudgetCalendarDetail(this);
            }

            this.clientMode.OnSelectedCell = oldMode;
            this.clientMode.OnDayClick = oldDetail;
            break;
        default:
            this.clientMode = new Robotics.Client.Controls.roSchedulerCalendar(this);
            break;
    }

    newClientmode = this.clientMode.name;

    if (newClientmode != oldClientmode && this.pageLayout != null) {
        this.pageLayout.destroy();

        //Definición del layout del componente
        this.pageLayoutOptions = this.clientMode.getLayout();

        if (this.workMode == Robotics.Client.Constants.WorkMode.roBudget) {
            this.clientMode.create();
        }
    }
}

Robotics.Client.Controls.roCalendar.prototype.setFiltersCookieValue = function () {
    localStorage.setItem(this.calendarStateCookie, (this.loadRecursive ? "true" : "false") + "@" + this.assignmentsFilter + "@" + (this.schedulingView == Robotics.Client.Constants.CoverageDayView.Planified ? '0' : '1') + "@" + (this.loadIndictments ? "true" : "false") + "@" + (this.loadPunches ? "true" : "false") + "@" + (this.loadCapacities ? "true" : "false") + "@" + this.dailyPeriod);
}

Robotics.Client.Controls.roCalendar.prototype.initialize = function () {
    this.selectedMinRow = -1;
    this.selectedMaxRow = -1;

    this.selectedMinColumn = -1;
    this.selectedMaxColumn = -1;

    this.selectedMinHeaderColumn = -1;
    this.selectedMaxHeaderColumn = -1;

    this.copyWorkingShifts = false;
    this.copyHolidaysShifts = false;
    this.copyAssignmentsShifts = false;

    this.selectedEmployee = null;
    this.selectedDay = null;
    this.selectedContainer = null;
    this.selectedHeaderContainer = null;

    this.hasChanges = false;
    this.selectionCopied = false;
    this.selectionHeaderCopied = false;

    if (this.refreshTimmer != -1) clearTimeout(this.refreshTimmer);
    this.refreshTimmer = -1;
    this.isShowingDialog = false;
    this.columnShiftsList = {};
    this.rowsShiftsList = {};
    this.shiftsList = {};
    this.assignmentsList = {};
    this.employeeDataList = {};
    this.rowsAssignmentsList = {};
    this.columnsAssignmentsList = {};
};

Robotics.Client.Controls.roCalendar.prototype.isBatchMode = function () {
    if ((this.selectedMinRow != this.selectedMaxRow) || (this.selectedMinColumn != this.selectedMaxColumn)) {
        return true;
    } else {
        return false;
    }
};

Robotics.Client.Controls.roCalendar.prototype.performAction = function (oParameters, action, showLoader) {
    oParameters.action = action;

    var strParameters = JSON.stringify(oParameters);
    strParameters = encodeURIComponent(strParameters);

    if (typeof showLoader == 'undefined' || (typeof showLoader != 'undefined' && showLoader == true)) this.clientMode.loadingFunctionExtended(true);

    this.requestObject.PerformCallback(strParameters);
};

Robotics.Client.Controls.roCalendar.prototype.loadingFunctionExtended = function (showLoader) {
    this.clientMode.loadingFunctionExtended(showLoader);
}

Robotics.Client.Controls.roCalendar.prototype.showChangesWarning = function (onAcceptFunc) {
    this.onContinueFunc = onAcceptFunc;
    this.loadingFunctionExtended(false);
    var scriptAcceptContinue = 'window.frames["ifPrincipal"].' + this.clientInstanceName + '.saveAndContinue();window.frames["ifPrincipal"].focus();';
    var scriptDiscardContinue = 'window.frames["ifPrincipal"].' + this.clientInstanceName + '.discardAndContinue();window.frames["ifPrincipal"].focus();';
    var scriptCancel = 'window.frames["ifPrincipal"].focus();';

    this.showErrorPopup("Info.Title", "info", "Calendar.Client.SelectOption", "", "Info.SaveContinue", "Info.SaveContinueDesc", scriptAcceptContinue, "Info.DiscardContinue", "Info.DiscardContinueDesc", scriptDiscardContinue, "Info.Cancel", "Info.CancelDesc", scriptCancel);
};

Robotics.Client.Controls.roCalendar.prototype.showCapacityWarning = function (onAcceptFunc) {
    var scriptAcceptContinue = 'window.frames["ifPrincipal"].' + this.clientInstanceName + '.clientMode.saveChangesFinally();window.frames["ifPrincipal"].focus();';
    var scriptCancel = 'window.frames["ifPrincipal"].focus();';

    this.showErrorPopup("Capacity.Title", "info", "Calendar.Client.SelectOption", "", "Info.SaveAnyway", "Info.SaveAnywayDesc", scriptAcceptContinue, "Info.CancelAndCorrect", "Info.CancelAndCorrectDesc", scriptCancel);
};

Robotics.Client.Controls.roCalendar.prototype.getEmployeeCountResume = function () {
    return this.clientMode.getEmployeeCountResume();
};

Robotics.Client.Controls.roCalendar.prototype.create = function () {
    this.clientMode.create();
};

Robotics.Client.Controls.roCalendar.prototype.saveAndContinue = function () {
    this.clientMode.saveAndContinue();
};

Robotics.Client.Controls.roCalendar.prototype.discardAndContinue = function () {
    this.clientMode.discardAndContinue();
};

Robotics.Client.Controls.roCalendar.prototype.refresh = function () {
    var oTmpCalendar = this;

    if (!this.loadingInfo.initializing) {
        let oFilter = {
            Filter: this.employeeFilter,
            Recursive: this.loadRecursive,
            When: moment(this.firstDate).format("YYYYMMDD")
        }

        this.loadingInfo.loadRecursive = this.loadRecursive;

        $.ajax({
            type: "POST",
            url: "./../Employee/GetEmployeeCount",
            dataType: "json",
            data: { Filter: oFilter },
            success: function (e) {
                let viewDays = moment(oTmpCalendar.endDate).diff(moment(oTmpCalendar.firstDate), 'days') + 1;

                if (oTmpCalendar.loadHugeDataValue > 0 && oTmpCalendar.loadHugeDataValue <= (parseInt(e, 10) * viewDays)) {
                    oTmpCalendar.loadingFunc(false);
                    let scriptAcceptContinue = 'window.frames["ifPrincipal"].' + oTmpCalendar.clientInstanceName + '.refreshHugeData();window.frames["ifPrincipal"].focus();';
                    let scriptDiscardContinue = 'window.frames["ifPrincipal"].' + oTmpCalendar.clientInstanceName + '.discardHugeData();window.frames["ifPrincipal"].focus();';
                    oTmpCalendar.showErrorPopup("loadcalendardata.title", "info", "Calendar.Client.SelectOption", "", "Info.loadcalendardata", "Info.loadcalendardatadesc", scriptAcceptContinue, "Info.cancelloadcalendardata", "Info.cancelloadcalendardatarefreshdesc", scriptDiscardContinue);
                } else {
                    oTmpCalendar.lastSelectionLoaded = {
                        employeeFilter: oTmpCalendar.loadingInfo.employeeFilter,
                        loadRecursive: oTmpCalendar.loadingInfo.loadRecursive,
                        firstDate: oTmpCalendar.loadingInfo.firstDate,
                        endDate: oTmpCalendar.loadingInfo.endDate
                    }

                    oTmpCalendar.refreshHugeData();
                }
            },
            error: function (e) { }
        });
    } else {
        oTmpCalendar.refreshHugeData();
    }
};

Robotics.Client.Controls.roCalendar.prototype.refreshHugeData = function () {
    this.clientMode.refresh();
};

Robotics.Client.Controls.roCalendar.prototype.generateLoadFilters = function () {
    return this.clientMode.generateLoadFilters();
};

Robotics.Client.Controls.roCalendar.prototype.loadData = function (firstDate, endDate, employeeFilter, loadRecursive, typeView, calendarFilter, loadIndictments, loadPunches, loadCapacities) {
    var oTmpCalendar = this;
    employeeFilter = (typeof (employeeFilter) != 'undefined' && employeeFilter != null) ? employeeFilter : this.employeeFilter;
    loadRecursive = (typeof (loadRecursive) != 'undefined' && loadRecursive != null) ? loadRecursive : this.loadRecursive;
    firstDate = (typeof (firstDate) != 'undefined' && firstDate != null) ? firstDate : this.firstDate;
    endDate = (typeof (endDate) != 'undefined' && endDate != null) ? endDate : this.endDate;

    this.loadingInfo = {
        initializing: false,
        firstDate: firstDate,
        endDate: endDate,
        employeeFilter: employeeFilter,
        loadRecursive: loadRecursive,
        typeView: typeView,
        calendarFilter: calendarFilter,
        loadIndictments: loadIndictments,
        loadPunches: loadPunches,
        loadCapacities: loadCapacities
    };

    let oFilter = {
        Filter: employeeFilter,
        Recursive: loadRecursive,
        When: moment(firstDate).format("YYYYMMDD")
    }

    //Si no ha cambiado nada, no hay necesidad de mostrar el popup
    if (employeeFilter == this.employeeFilter && moment(firstDate).format("YYYYMMDD") == moment(this.firstDate).format("YYYYMMDD")
        && moment(endDate).format("YYYYMMDD") == moment(this.endDate).format("YYYYMMDD")) {
        oTmpCalendar.loadHugeData();
    } else {
        $.ajax({
            type: "POST",
            url: "./../Employee/GetEmployeeCount",
            dataType: "json",
            data: { Filter: oFilter },
            success: function (e) {
                let viewDays = moment(oTmpCalendar.loadingInfo.endDate).diff(moment(oTmpCalendar.loadingInfo.firstDate), 'days') + 1;

                if (oTmpCalendar.loadHugeDataValue > 0 && oTmpCalendar.loadHugeDataValue <= (parseInt(e, 10) * viewDays)) {
                    oTmpCalendar.loadingFunc(false);
                    let scriptAcceptContinue = 'window.frames["ifPrincipal"].' + oTmpCalendar.clientInstanceName + '.loadHugeData();window.frames["ifPrincipal"].focus();';
                    let scriptDiscardContinue = 'window.frames["ifPrincipal"].' + oTmpCalendar.clientInstanceName + '.discardHugeData();window.frames["ifPrincipal"].focus();';
                    oTmpCalendar.showErrorPopup("loadcalendardata.title", "info", "Calendar.Client.SelectOption", "", "Info.loadcalendardata", "Info.loadcalendardatadesc", scriptAcceptContinue, "Info.cancelloadcalendardata", "Info.cancelloadcalendardatadesc", scriptDiscardContinue);
                } else {
                    oTmpCalendar.lastSelectionLoaded = {
                        employeeFilter: oTmpCalendar.loadingInfo.employeeFilter,
                        loadRecursive: oTmpCalendar.loadingInfo.loadRecursive,
                        firstDate: oTmpCalendar.loadingInfo.firstDate,
                        endDate: oTmpCalendar.loadingInfo.endDate
                    }
                    oTmpCalendar.loadHugeData();
                }
            },
            error: function (e) { }
        });
    }
};

Robotics.Client.Controls.roCalendar.prototype.discardHugeData = function () {
    this.clientMode.loadingFunctionExtended(true);

    if (this.loadingInfo.employeeFilter != this.lastSelectionLoaded.employeeFilter) {
        if (typeof window.frames['ifEmployeeSelector'].currentView != 'undefined' && window.frames['ifEmployeeSelector'].currentView != null) {
            const oldSelection = JSON.parse(localStorage.getItem('empMVCSelector'));
            oldSelection.view.ComposeFilter = this.lastSelectionLoaded.employeeFilter.split("¬")[0];

            oldSelection.view.Employees = [];
            oldSelection.view.Groups = [];

            oldSelection.timestamp = Date.now();
            oldSelection.view.ComposeFilter.split(",").forEach(function (item) {
                if (item.indexOf("A") == 0) oldSelection.view.Groups.push({ IdGroup: item.replace("A", ""), Name: '' })
                else if (item.indexOf("B") == 0) oldSelection.view.Employees.push({ IdEmployee: item.replace("B", ""), Name: '' })
            })

            window.frames['ifEmployeeSelector'].currentView = oldSelection.view;
            localStorage.setItem('empMVCSelector', JSON.stringify(oldSelection));
        }

        this.loadingInfo.employeeFilter = this.lastSelectionLoaded.employeeFilter;
    }

    if (this.loadingInfo.loadRecursive != this.lastSelectionLoaded.loadRecursive) {
        if (typeof this.clientMode.setRecursiveData == "function") this.clientMode.setRecursiveData();

        this.loadingInfo.loadRecursive = this.lastSelectionLoaded.loadRecursive;
    }

    if (moment(this.loadingInfo.firstDate).format("YYYYMMDD") != moment(this.lastSelectionLoaded.firstDate).format("YYYYMMDD") ||
        moment(this.loadingInfo.endDate).format("YYYYMMDD") != moment(this.lastSelectionLoaded.endDate).format("YYYYMMDD")) {
        this.loadingInfo.firstDate = moment().startOf('week').toDate();
        this.loadingInfo.endDate = moment().add(1, 'week').endOf('week').toDate();

        let textodates = FormateaFecha(moment().startOf('week').toDate()) + "," + FormateaFecha(moment().add(1, 'week').endOf('week').toDate());
        localStorage.setItem('SchedulerIntervalDates', textodates);
        localStorage.setItem('PlanView', "2");

        try {
            forceRefresh = false;
            LoadDateSelector(this.cConfig.Get('Language'));
            forceRefresh = true;
        } catch (e) { }
    }

    this.loadHugeData();
};

Robotics.Client.Controls.roCalendar.prototype.loadHugeData = function () {
    this.clientMode.loadingFunctionExtended(true);
    this.loadingFunc(true);

    this.loadingInfo.initializing = true;

    this.clientMode.loadData(this.loadingInfo.firstDate, this.loadingInfo.endDate, this.loadingInfo.employeeFilter, this.loadingInfo.loadRecursive, this.loadingInfo.typeView,
        this.loadingInfo.calendarFilter, this.loadingInfo.loadIndictments, this.loadingInfo.loadPunches, this.loadingInfo.loadCapacities);

    this.loadingInfo.initializing = false;
};

Robotics.Client.Controls.roCalendar.prototype.saveChanges = function () {
    this.clientMode.saveChanges();
};

Robotics.Client.Controls.roCalendar.prototype.refreshDayWithParams = function (idEmployee, selectedDay) {
    this.clientMode.refreshDayWithParams(idEmployee, selectedDay);
};

Robotics.Client.Controls.roCalendar.prototype.setHasChanges = function (bolHasChanges) {
    this.clientMode.setHasChanges(bolHasChanges);
};

Robotics.Client.Controls.roCalendar.prototype.endCallback = function (action, objResult, objResultParams) {
    this.clientMode.endCallback(action, objResult, objResultParams);
};

Robotics.Client.Controls.roCalendar.prototype.setCallbackReturnData = function (objReturn, isHourData) {
    this.clientMode.setCallbackReturnData(objReturn, isHourData);
};

Robotics.Client.Controls.roCalendar.prototype.refreshFullDay = function () {
    this.clientMode.refreshFullDay();
};

Robotics.Client.Controls.roCalendar.prototype.onDrop = function (e) {
    this.clientMode.onDrop(e);
};

/* Dialogo de importación y exportación de fichero excel */
Robotics.Client.Controls.roCalendar.prototype.createImportDialog = function () {
    var oCal = this;

    $('#' + oCal.ascxPrefix + '_dlgImport_txtFileToImport').on('change', function (event) {
        oCal.prepareUpload(event, oCal);
    });

    return $("#" + oCal.ascxPrefix + "_dlgImport_frm").dialog({
        autoOpen: false,
        height: 'auto',
        width: 'auto',
        modal: true,
        resizable: false,
        draggable: false,
        buttons: [{
            text: oCal.translator.translate(Robotics.Client.Language.Tags.Accept),
            "class": 'btnFlat btnFlatBlack',
            click: function () {
                oCal.launchImport();
            }
        }, {
            text: oCal.translator.translate(Robotics.Client.Language.Tags.Cancel),
            "class": 'btnFlat btnFlatBlack',
            click: function () {
                oCal.importDialog.dialog("close");
            }
        }],
        close: function () {
        }
    });
};

Robotics.Client.Controls.roCalendar.prototype.importFromExcel = function () {
    $('#' + this.ascxPrefix + '_dlgImport_txtFileToImport').val('');
    eval(this.ascxPrefix + '_dlgImport_ckImportCopyMainShiftsClient').SetChecked(true);
    eval(this.ascxPrefix + '_dlgImport_ckImportCopyHolidaysClient').SetChecked(false);
    eval(this.ascxPrefix + '_dlgImport_ckImportKeepHolidaysClient').SetChecked(true);
    eval(this.ascxPrefix + '_dlgImport_ckImportKeepBloquedDaysClient').SetChecked(true);

    this.importDialog.dialog("open");
};

Robotics.Client.Controls.roCalendar.prototype.prepareUpload = function (event, oCal) {
    oCal.files = event.target.files;
};

Robotics.Client.Controls.roCalendar.prototype.launchImport = function () {
    var data = new FormData();
    var oCal = this;
    if (this.files.length > 0) {
        data.append('importFile', this.files[0]);
    }

    $.ajax({
        url: '../Base/WebUserControls/roCalendar/Handlers/srvCalendarImport.ashx',
        type: 'POST',
        data: data,
        cache: false,
        dataType: 'json',
        processData: false, // Don't process the files
        contentType: false, // Set content type to false as jQuery will tell the server its a query string request
        success: function (data, textStatus, jqXHR) {
            oCal.importDialog.dialog("close");
            oCal.finallyImportFile();
        },
        error: function (jqXHR, textStatus, errorThrown) {
            oCal.showErrorPopup("Error.Title", "error", "Calendar.Client.UploadFile", "", "Error.OK", "Error.OKDesc", oCal.defaultMessageAction);
        }
    });
};

Robotics.Client.Controls.roCalendar.prototype.finallyImportFile = function () {
    this.clientMode.importFile();
};

Robotics.Client.Controls.roCalendar.prototype.exportToExcel = function () {
    this.clientMode.exportToExcel();
};
/* Fin Dialogo de importación y exportación de fichero excel */

/* Dialogo de filtrado de puestos (Solo disponible en modo roScheduleCalendar */
Robotics.Client.Controls.roCalendar.prototype.createFilterCalendarDialog = function (isImport) {
    var oCal = this;

    var formID = '';
    var buttons = [];

    buttons = [{
        text: oCal.translator.translate(Robotics.Client.Language.Tags.Empty),
        "class": 'btnFlat btnFlatBlack',
        click: function () {
            oCal.clearFilterAssignments();
            oCal.filterCalendarDialog.dialog("close");
        }
    }, {
        text: oCal.translator.translate(Robotics.Client.Language.Tags.Accept),
        "class": 'btnFlat btnFlatBlack',
        click: function () {
            oCal.changeFilterAssignments();
            oCal.filterCalendarDialog.dialog("close");
        }
    }, {
        text: oCal.translator.translate(Robotics.Client.Language.Tags.Cancel),
        "class": 'btnFlat btnFlatBlack',
        click: function () {
            oCal.filterCalendarDialog.dialog("close");
        }
    }];

    return $("#" + oCal.ascxPrefix + "_dlgFilterCalendar_frm").dialog({
        autoOpen: false,
        height: 'auto',
        width: '350px',
        modal: true,
        resizable: false,
        draggable: false,
        buttons: buttons,
        close: function () {
        }
    });
};

Robotics.Client.Controls.roCalendar.prototype.changeFilterAssignments = function () {
    this.clientMode.changeFilterAssignments();
}

Robotics.Client.Controls.roCalendar.prototype.openFilterAssignments = function () {
    var oCal = this;
    var assignmentsGrid = eval(this.ascxPrefix + '_dlgFilterCalendar_grdAssignmentsClient');
    var rbPlannedView = eval(this.ascxPrefix + '_dlgFilterCalendar_rbPlannedView');
    var rbRealView = eval(this.ascxPrefix + '_dlgFilterCalendar_rbRealView');

    if (oCal.assignmentsFilter != '') {
        var keys = [];

        if (typeof assignmentsGrid != 'undefined' && assignmentsGrid != null) {
            for (var tmpIndex = 0; tmpIndex < oCal.assignmentsFilter.split(',').length; tmpIndex++) {
                keys.push(parseInt(oCal.assignmentsFilter.split(',')[tmpIndex], 10));
            }
            assignmentsGrid.SelectRowsByKey(keys);
        }
    } else {
        if (typeof assignmentsGrid != 'undefined' && assignmentsGrid != null) assignmentsGrid.UnselectRows();
    }

    if (oCal.schedulingView == Robotics.Client.Constants.CoverageDayView.Planified) {
        rbPlannedView.SetChecked(true);
    } else {
        rbRealView.SetChecked(true);
    }

    oCal.filterCalendarDialog.dialog("open");
}

Robotics.Client.Controls.roCalendar.prototype.clearFilterAssignments = function () {
    this.assignmentsFilter = '';
    this.schedulingView = Robotics.Client.Constants.CoverageDayView.Planified;

    this.setFiltersCookieValue();
    this.refresh();
}
/* Fin dialogo de filtrado de puestos (Solo disponible en modo roScheduleCalendar */

/* Dialogo gestión de los mensajes de error */
Robotics.Client.Controls.roCalendar.prototype.buildErrorMessage = function (objResult, isImport) {
    var errorText = '';
    if (objResult.CalendarDataResult != null) {
        for (var i = 0; i < objResult.CalendarDataResult.length; i++) {
            if (errorText != '') errorText += '\r\n';

            errorText += objResult.CalendarDataResult[i].ErrorText;
        }
    }

    if (objResult.BudgetDataResult != null) {
        for (var i = 0; i < objResult.BudgetDataResult.length; i++) {
            if (errorText != '') errorText += '\r\n';

            errorText += objResult.BudgetDataResult[i].ErrorText;
        }
    }

    var destiny = isImport ? eval(this.ascxPrefix + '_dlgImportError_txtErrorMemoClient') : eval(this.ascxPrefix + '_dlgError_txtErrorMemoClient');
    destiny.SetValue(errorText);
};

Robotics.Client.Controls.roCalendar.prototype.parseErrorMessage = function (action, objResult) {
    var bParsed = false;
    switch (action) {
        case Robotics.Client.Constants.Actions.SaveBudgetAndContinue:
        case Robotics.Client.Constants.Actions.SaveBudgetChanges:
        case Robotics.Client.Constants.Actions.SaveAndContinue:
        case Robotics.Client.Constants.Actions.SaveChanges:
            this.buildErrorMessage(objResult, false);
            this.errorDialog.dialog("open");
            bParsed = true;
            break;
    }

    return bParsed;
};

Robotics.Client.Controls.roCalendar.prototype.showDailyConfig = function () {
    var oCal = this;
    var rbviewmode = eval(oCal.ascxPrefix + "_dlgEditDailyView_rbViewModesClient");
    rbviewmode.SetValue(oCal.dailyPeriod + '');
    this.editDailyViewDialog.dialog("open");
}

Robotics.Client.Controls.roCalendar.prototype.createDailyConfig = function () {
    var oCal = this;

    var formID = '';
    var buttons = [];

    formID = "#" + oCal.ascxPrefix + "_dlgEditDailyView_frm";
    buttons = [{
        text: oCal.translator.translate(Robotics.Client.Language.Tags.Accept),
        "class": 'btnFlat btnFlatBlack',
        click: function () {
            var rbviewmode = eval(oCal.ascxPrefix + "_dlgEditDailyView_rbViewModesClient");
            oCal.dailyPeriod = parseInt(rbviewmode.GetValue(), 10);
            oCal.setFiltersCookieValue();
            oCal.refresh();
            oCal.editDailyViewDialog.dialog("close");
        }
    }, {
        text: oCal.translator.translate(Robotics.Client.Language.Tags.Cancel),
        "class": 'btnFlat btnFlatBlack',
        click: function () {
            oCal.editDailyViewDialog.dialog("close");
        }
    }];

    return $(formID).dialog({
        autoOpen: false,
        height: 'auto',
        width: '350px',
        modal: true,
        resizable: false,
        draggable: false,
        buttons: buttons,
        close: function () {
        }
    });
}

Robotics.Client.Controls.roCalendar.prototype.createErrorDialog = function (isImport) {
    var oCal = this;

    var formID = '';
    var buttons = [];

    if (!isImport) {
        formID = "#" + oCal.ascxPrefix + "_dlgError_frm";
        buttons = [{
            text: oCal.translator.translate(Robotics.Client.Language.Tags.Accept),
            "class": 'btnFlat btnFlatBlack',
            click: function () {
                oCal.errorDialog.dialog("close");
            }
        }];
    } else {
        formID = "#" + oCal.ascxPrefix + "_dlgImportError_frm";
        buttons = [{
            id: 'btDownload',
            text: oCal.translator.translate(Robotics.Client.Language.Tags.Download),
            "class": 'btnFlat btnFlatBlack',
            click: function () {
                /* Check url is correct ISM TODO */
                window.open("./../Base/WebUserControls/roCalendar/Wizards/downloadCalendar.aspx");
            }
        }, {
            text: oCal.translator.translate(Robotics.Client.Language.Tags.Accept),
            "class": 'btnFlat btnFlatBlack',
            click: function () {
                oCal.importErrorDialog.dialog("close");
            }
        }];
    }

    return $(formID).dialog({
        autoOpen: false,
        height: 'auto',
        width: '350px',
        modal: true,
        resizable: false,
        draggable: false,
        buttons: buttons,
        close: function () {
        }
    });
};
/* Fin dialogo gestión de los mensajes de error */

/* Dialogo gestión del orden de la tabla */
Robotics.Client.Controls.roCalendar.prototype.createSortCalendarDialog = function () {
    var oCal = this;

    $(".sortCalendar").sortable({
        stop: function (event, ui) {
            oCal.sortElements = "";

            $(".sortCalendar li").each(function (i, el) {
                var p = $(el).attr('data-orderElement');
                oCal.sortElements += p + ",";
            });

            if (oCal.sortElements != '') oCal.sortElements = oCal.sortElements.substring(0, oCal.sortElements.length - 1);
        }
    });

    $(".sortCalendar").disableSelection();

    return $("#" + oCal.ascxPrefix + "_dlgSortCalendar_frm").dialog({
        autoOpen: false,
        height: 'auto',
        width: '250px',
        modal: true,
        resizable: false,
        draggable: false,
        buttons: [{
            text: oCal.translator.translate(Robotics.Client.Language.Tags.Accept),
            "class": 'btnFlat btnFlatBlack',
            click: function () {
                oCal.sortCalendar();
            }
        }, {
            text: oCal.translator.translate(Robotics.Client.Language.Tags.Cancel),
            "class": 'btnFlat btnFlatBlack',
            click: function () {
                oCal.calendarSortDialog.dialog("close");
            }
        }],
        close: function () {
        }
    });
};

Robotics.Client.Controls.roCalendar.prototype.sortCalendar = function () {
    this.clientMode.sortCalendar();
};
/* Fin dialogo gestión del orden de la tabla */

Robotics.Client.Controls.roCalendar.prototype.createShiftsDialog = function () {
    var oCal = this;
    return $("#" + oCal.ascxPrefix + "_dlgShiftSelector_frm").dialog({
        autoOpen: false,
        height: 'auto',
        width: 'auto',
        modal: true,
        resizable: false,
        draggable: false,
        buttons: [{
            text: Globalize.formatMessage("roNext"),
            "class": 'btnFlat btnFlatBlack',
            click: function () {
                oCal.onAcceptShiftsDialog();
            }
        }, {
            text: oCal.translator.translate(Robotics.Client.Language.Tags.Cancel),
            "class": 'btnFlat btnFlatBlack',
            click: function () {
                oCal.shiftsDialog.dialog("close");
            }
        }],
        close: function () {
        }
    });
};

Robotics.Client.Controls.roCalendar.prototype.onAcceptShiftsDialog = function () {
    this.clientMode.onAcceptShiftsDialog();
};

Robotics.Client.Controls.roCalendar.prototype.prepareShfitsDialog = function (objResult) {
    this.clientMode.prepareShfitsDialog(objResult);
};

/* Dialogo de selección de unidades productivas*/
Robotics.Client.Controls.roCalendar.prototype.createPUnitsDialog = function () {
    var oCal = this;
    return $("#" + oCal.ascxPrefix + "_dlgProductiveUnitSelector_frm").dialog({
        autoOpen: false,
        height: 'auto',
        width: 'auto',
        modal: true,
        resizable: false,
        draggable: false,
        buttons: [{
            text: Globalize.formatMessage("roNext"),
            "class": 'btnFlat btnFlatBlack',
            click: function () {
                oCal.onAcceptPUnitsDialog();
            }
        }, {
            text: oCal.translator.translate(Robotics.Client.Language.Tags.Cancel),
            "class": 'btnFlat btnFlatBlack',
            click: function () {
                oCal.pUnitsDialog.dialog("close");
            }
        }],
        close: function () {
        }
    });
};

Robotics.Client.Controls.roCalendar.prototype.onAcceptPUnitsDialog = function () {
    this.clientMode.onAcceptPUnitsDialog();
};

Robotics.Client.Controls.roCalendar.prototype.preparePUnitsDialog = function (objResult) {
    this.clientMode.preparePUnitsDialog(objResult);
};

/* Dialogo de gestión de empleados en presupuesto*/
Robotics.Client.Controls.roCalendar.prototype.createBudgetEmployeesDialog = function () {
    var oCal = this;
    return $("#" + oCal.ascxPrefix + "_dlgBudgetAddEmployees_frm").dialog({
        autoOpen: false,
        height: 'auto',
        width: '1100px',
        modal: true,
        resizable: false,
        draggable: false,
        buttons: [{
            text: Globalize.formatMessage("roAssign"),
            "class": 'btnFlat btnFlatBlack',
            click: function () {
                oCal.onAcceptBudgetEmployeesDialog();
            }
        }, {
            text: oCal.translator.translate(Robotics.Client.Language.Tags.Cancel),
            "class": 'btnFlat btnFlatBlack',
            click: function () {
                oCal.budgetEmployeesDialog.dialog("close");
            }
        }],
        close: function () {
        }
    });
};

Robotics.Client.Controls.roCalendar.prototype.onAcceptBudgetEmployeesDialog = function () {
    this.clientMode.onAcceptBudgetEmployeesDialog();
};

Robotics.Client.Controls.roCalendar.prototype.prepareBudgetEmployeesDialog = function (objResult, objResultParams) {
    this.clientMode.prepareBudgetEmployeesDialog(objResult, objResultParams);
};

/* Dialogo gestión de los puestos de empleado*/
Robotics.Client.Controls.roCalendar.prototype.createAssignmentsDialog = function () {
    var oCal = this;
    return $("#" + oCal.ascxPrefix + "_dlgAssignments_frm").dialog({
        autoOpen: false,
        height: 'auto',
        width: 'auto',
        modal: true,
        resizable: false,
        draggable: false,
        buttons: [{
            text: oCal.translator.translate(Robotics.Client.Language.Tags.Accept),
            "class": 'btnFlat btnFlatBlack',
            click: function () {
                oCal.onAcceptAssignmentsDialog();
            }
        }, {
            text: oCal.translator.translate(Robotics.Client.Language.Tags.Cancel),
            "class": 'btnFlat btnFlatBlack',
            click: function () {
                oCal.assignmentsDialog.dialog("close");
            }
        }],
        close: function () {
        }
    });
};

Robotics.Client.Controls.roCalendar.prototype.onAcceptAssignmentsDialog = function () {
    this.clientMode.onAcceptAssignmentsDialog();
};

Robotics.Client.Controls.roCalendar.prototype.prepareAssignmentsDialog = function (objResult) {
    this.clientMode.prepareAssignmentsDialog(objResult);
};
/* Fin dialogo gestión de los puestos de empleado*/

Robotics.Client.Controls.roCalendar.prototype.createStarterDialog = function () {
    var oCal = this;
    return $("#" + oCal.ascxPrefix + "_dlgStarter_frm").dialog({
        autoOpen: false,
        height: 'auto',
        width: 'auto',
        modal: true,
        resizable: false,
        draggable: false,
        buttons: [{
            text: oCal.translator.translate(Robotics.Client.Language.Tags.Accept),
            "class": 'btnFlat btnFlatBlack',
            click: function () {
                oCal.onAcceptStarterDialog();
            }
        }, {
            text: oCal.translator.translate(Robotics.Client.Language.Tags.Cancel),
            "class": 'btnFlat btnFlatBlack',
            click: function () {
                oCal.starterDialog.dialog("close");
            }
        }],
        close: function () {
        }
    });
};

Robotics.Client.Controls.roCalendar.prototype.onAcceptStarterDialog = function () {
    this.clientMode.onAcceptStarterDialog();
};

Robotics.Client.Controls.roCalendar.prototype.prepareStarterDialog = function (objResult) {
    this.clientMode.prepareStarterDialog(objResult);
};

/* Dialogo gestión de las horas complementarias*/
Robotics.Client.Controls.roCalendar.prototype.createComplementaryDialog = function () {
    var oCal = this;
    return $("#" + oCal.ascxPrefix + "_dlgComplementary_frm").dialog({
        autoOpen: false,
        height: 'auto',
        width: 'auto',
        modal: true,
        resizable: false,
        draggable: false,
        buttons: [{
            text: oCal.translator.translate(Robotics.Client.Language.Tags.Accept),
            "class": 'btnFlat btnFlatBlack',
            click: function () {
                oCal.onAcceptComplementaryDialog();
            }
        }, {
            text: oCal.translator.translate(Robotics.Client.Language.Tags.Cancel),
            "class": 'btnFlat btnFlatBlack',
            click: function () {
                oCal.complementaryDialog.dialog("close");
            }
        }],
        close: function () {
        }
    });
};

Robotics.Client.Controls.roCalendar.prototype.onAcceptComplementaryDialog = function () {
    this.clientMode.onAcceptComplementaryDialog();
};

Robotics.Client.Controls.roCalendar.prototype.prepareComplementaryDialog = function (objResult) {
    this.clientMode.prepareComplementaryDialog(objResult);
};
/* Fin dialogo gestión de las horas complementarias*/

/* Dialogo copia de horarios y copia avanzada de horarios*/
Robotics.Client.Controls.roCalendar.prototype.createAdvancedCopyDialog = function () {
    var oCal = this;
    return $("#" + oCal.ascxPrefix + "_dlgAdvCopy_frm").dialog({
        autoOpen: false,
        height: 'auto',
        width: 'auto',
        modal: true,
        resizable: false,
        draggable: false,
        buttons: [{
            text: oCal.translator.translate(Robotics.Client.Language.Tags.Accept),
            "class": 'btnFlat btnFlatBlack',
            click: function () {
                showCaptcha(false);
            }
        }, {
            text: oCal.translator.translate(Robotics.Client.Language.Tags.Cancel),
            "class": 'btnFlat btnFlatBlack',
            click: function () {
                oCal.advCopyDialog.dialog("close");
            }
        }],
        close: function () {
        }
    });
};

Robotics.Client.Controls.roCalendar.prototype.closeAdvancedCopyDialog = function () {
    this.advCopyDialog.dialog("close");
};

Robotics.Client.Controls.roCalendar.prototype.onAcceptCopyDialog = function (keepLockedDays, KeepDestHolidayDays) {
    this.clientMode.onAcceptCopyDialog(keepLockedDays, KeepDestHolidayDays);
}

Robotics.Client.Controls.roCalendar.prototype.createCopyDialog = function () {
    var oCal = this;
    return $("#" + oCal.ascxPrefix + "_dlgCopy_frm").dialog({
        autoOpen: false,
        height: 'auto',
        width: 'auto',
        modal: true,
        resizable: false,
        draggable: false,
        buttons: [{
            text: oCal.translator.translate(Robotics.Client.Language.Tags.Accept),
            "class": 'btnFlat btnFlatBlack',
            click: function () {
                var keepLockedDays = eval(oCal.ascxPrefix + '_dlgCopy_ckSPKeepBloquedDaysClient').GetChecked(); //ckSPKeepBloquedDays_Client.GetChecked();
                var KeepDestHolidayDays = eval(oCal.ascxPrefix + '_dlgCopy_ckSPKeepHolidaysClient').GetChecked(); //ckSPKeepHolidays_Client.GetChecked();

                oCal.onAcceptCopyDialog(keepLockedDays, KeepDestHolidayDays);

                oCal.sourceDialogRow = -1;
                oCal.sourceDialogColumn = -1;
                oCal.copyDialog.dialog("close");
            }
        }, {
            text: oCal.translator.translate(Robotics.Client.Language.Tags.Cancel),
            "class": 'btnFlat btnFlatBlack',
            click: function () {
                oCal.sourceDialogRow = -1;
                oCal.sourceDialogColumn = -1;
                oCal.copyDialog.dialog("close");
            }
        }],
        close: function () {
        }
    });
};
/* Fin dialogo copia de horarios y copia avanzada de horarios*/

Robotics.Client.Controls.roCalendar.prototype.buildHeaderContextMenu = function (sender) {
    return this.clientMode.buildHeaderContextMenu(sender);
};

Robotics.Client.Controls.roCalendar.prototype.executeHeaderContextMenuAction = function (key, container) {
    return this.clientMode.executeHeaderContextMenuAction(key, container);
};

Robotics.Client.Controls.roCalendar.prototype.setSingleHeaderSelectedObejct = function (sender) {
    return this.clientMode.setSingleHeaderSelectedObejct(sender);
};

Robotics.Client.Controls.roCalendar.prototype.setSingleSelectedObject = function (sender) {
    return this.clientMode.setSingleSelectedObject(sender);
};

Robotics.Client.Controls.roCalendar.prototype.buildContextMenu = function (sender) {
    return this.clientMode.buildContextMenu(sender);
};

Robotics.Client.Controls.roCalendar.prototype.executeContextMenuAction = function (key, container) {
    return this.clientMode.executeContextMenuAction(key, container);
};

Robotics.Client.Controls.roCalendar.prototype.getFirstSelectedDay = function () {
    return this.clientMode.getFirstSelectedDay();
};

Robotics.Client.Controls.roCalendar.prototype.getFirstObjectSelected = function () {
    return this.clientMode.getFirstObjectSelected();
};

Robotics.Client.Controls.roCalendar.prototype.mapEvents = function () {
    var oCalendar = this;

    var cSelector = this.clientMode.getContextMenuSelector();
    var cHeaderSelector = this.clientMode.getContextMenuHeaderSelector();

    $(document).off('keyup');
    $(document).on('keyup', function (e) {
        if (e.keyCode == 27) {
            oCalendar.selectionCopied = false;
            oCalendar.selectionHeaderCopied = false;
            oCalendar.clientMode.processKeyUpEvent(e);
        } else if (e.keyCode == oCalendar.ctrlKey) oCalendar.ctrlDown = false;
        else if (e.keyCode == oCalendar.shiftKey) oCalendar.shiftDown = false;
    });

    $(document).off('keydown');
    $(document).on('keydown', function (e) {
        if (e.keyCode == oCalendar.ctrlKey) oCalendar.ctrlDown = true;
        else if (e.keyCode == oCalendar.shiftKey) oCalendar.shiftDown = true;
        else { oCalendar.clientMode.processKeyDownEvent(e); }
    });

    $.contextMenu('destroy', cSelector);
    $.contextMenu({
        selector: cSelector,
        callback: function (key, options) {
            oCalendar.executeContextMenuAction(key, this);
        },
        build: function ($trigger, e) {
            oCalendar.setSingleSelectedObject($trigger);
            return {
                items: oCalendar.buildContextMenu($trigger)
            };
        }
    });

    if (cHeaderSelector != null && oCalendar.employeeFilter.count('A') == 1) {
        $.contextMenu('destroy', cHeaderSelector);
        $.contextMenu({
            selector: cHeaderSelector,
            callback: function (key, options) {
                oCalendar.executeHeaderContextMenuAction(key, this);
            },
            build: function ($trigger, e) {
                oCalendar.setSingleHeaderSelectedObejct($trigger);
                return {
                    items: oCalendar.buildHeaderContextMenu($trigger)
                };
            }
        });
    }

    this.clientMode.mapModeEvents();
};

Robotics.Client.Controls.roCalendar.prototype.refreshTables = function (objectRef, isResizing, refreshCalendar) {
    this.clientMode.refreshModeTables(objectRef, isResizing, refreshCalendar);
};

Robotics.Client.Controls.roCalendar.prototype.getSelectedIDEmployees = function () {
    return this.clientMode.getSelectedIDEmployees();
};

Robotics.Client.Controls.roCalendar.prototype.ConvertHoursToHourFormat = function (value) {
    try {
        //value = value * 60;

        var sign = "";
        if (value < 0) {
            sign = "-";
            value = value * -1;
        }

        var Hours = Math.floor(parseInt(value) / 60);
        var MinutesRest = "00";
        if ((parseInt(value) ^ 60) > 0) { //Si no son horas justas, sacar los minutos
            MinutesRest = parseInt(value) - (Hours * 60);
        }

        if (Hours.toString().length == 1) { Hours = "0" + Hours; }
        if (MinutesRest.toString().length == 1) { MinutesRest = "0" + MinutesRest; }

        return sign + Hours + ":" + MinutesRest;
    } catch (e) { showError("Robotics.Client.Controls.roCalendar.prototype.ConvertMinutesToHour", e); }
};

Robotics.Client.Controls.roCalendar.prototype.getMinDailyCell = function (destinationDay) {
    switch (this.dailyPeriod) {
        case 15:
            return 88;
            break;
        case 30:
            return 44;
            break;
        case 60:
            return 22;
            break;
        default:
            return 44;
            break;
    }

    return 44;
    //return 0;
};

Robotics.Client.Controls.roCalendar.prototype.getMaxDailyCell = function (destinationDay) {
    switch (this.dailyPeriod) {
        case 15:
            return 224;
            break;
        case 30:
            return 112;
            break;
        case 60:
            return 66;
            break;
        default:
            return 112;
            break;
    }

    return 112;
    //return this.oCalendar.CalendarHeader.PeriodHeaderData.length;
};

Robotics.Client.Controls.roCalendar.prototype.dailyPeriodHourFraction = function () {
    switch (this.dailyPeriod) {
        case 15:
            return 0.25;
            break;
        case 30:
            return 0.5;
            break;
        case 60:
            return 1;
            break;
        default:
            return 0.5;
            break;
    }
};

Robotics.Client.Controls.roCalendar.prototype.dailyPeriodDescription = function () {
    switch (this.dailyPeriod) {
        case 15:
            return Globalize.formatMessage("roPeriod15Min");
            break;
        case 30:
            return Globalize.formatMessage("roPeriod30Min");
            break;
        case 60:
            return Globalize.formatMessage("roPeriod60Min");
            break;
        default:
            return Globalize.formatMessage("roPeriod30Min");
            break;
    }
};