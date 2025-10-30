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

Robotics.Client.Controls.roSchedulerCalendar = function (baseControl) {
    this.name = "Robotics.Client.Controls.roSchedulerCalendar";

    //Control ocalendar con la información del modo y las funciones base que debemos sobreescribir
    this.oBaseControl = baseControl;

    //Pestaña del layout derecho visible
    this.rowTableVisible = 1;

    //Pestaña del layout sud visible
    this.columnTableVisible = 1;

    //Tabla principal con los header congelados
    this.fixedCalendarTable = null;

    this.pageLayoutOptions = {
        name: 'pageLayout'
        , resizable: false
        , center__slidable: false
        , center__closable: false
        , center__paneSelector: "#" + this.oBaseControl.prefix + Robotics.Client.Constants.LayoutNames.Center
        , center__children: {
            name: 'tabsContainerLayout'
            , resizable: false
            , center__slidable: false
            , center__closable: false
            , center__paneSelector: "#" + this.oBaseControl.prefix + Robotics.Client.Constants.LayoutNames.Calendar
            , south__paneSelector: "#" + this.oBaseControl.prefix + Robotics.Client.Constants.LayoutNames.ColumnTabs
            , south__slidable: true
            , south__closable: true
            , south__togglerLength_closed: 250
            , south__togglerContent_closed: ('<div class="btnExpand"> <div class="imgToUp"></div> <span style="font-weight: 100; color: #525252;">' + Globalize.formatMessage('roCalendarToggle') + '</span> <div class="imgToUp shiftToRight"></div> </div>')
            , south__onopen: this.synchronizeRowLayout(true, this)
            , south__onclose: this.synchronizeRowLayout(false, this)
            , spacing_closed: 35
            , initClosed: true
        }
        , east__closable: true
        , east__slidable: false
        , east__togglerLength_closed: 250
        , east__togglerContent_closed: ('<div class="btnExpandVer"> <div class="imgToLeft"></div> <span style="font-weight: 100; color: #525252;">' + Globalize.formatMessage('roCalendarToggle') + '</span> <div class="imgToLeft shiftToBottom" ></div> </div>')
        , east__onopen: this.synchronizeColumnLayout(true, this)
        , east__onclose: this.synchronizeColumnLayout(false, this)
        , east__paneSelector: "#" + this.oBaseControl.prefix + Robotics.Client.Constants.LayoutNames.East
        , east__children: {
            name: 'tabsContainerLayout'
            , resizable: false
            , slidable: false
            , closable: true
            , center__slidable: true
            , center__closable: true
            , center__paneSelector: "#" + this.oBaseControl.prefix + Robotics.Client.Constants.LayoutNames.RowTabs
            , south__paneSelector: "#" + this.oBaseControl.prefix + Robotics.Client.Constants.LayoutNames.Resume
            , south__slidable: false
            , south__closable: true
            , south__onopen: this.synchronizeSummaryLayout(true, this)
            , south__onclose: this.synchronizeSummaryLayout(false, this)
            , spacing_closed: 15
            , initClosed: true
        }
        , spacing_closed: 35
    };

    this.calendarIndictments = {};
    this.calendarIndictmentsDS = [];

    this.currentUpdateCell = null;
    this.validationInProgress = false;
    this.copyInProgress = false;

    this.bRelatedTables = false;
    this.dragTarget = null;

    this.resumeInfoLoaded = false;
    this.processingloadingClick = false;
}

Robotics.Client.Controls.roSchedulerCalendar.prototype.getLayout = function () {
    return this.pageLayoutOptions;
}

Robotics.Client.Controls.roSchedulerCalendar.prototype.setHasChanges = function (bolHasChanges) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    oCalendar.hasChanges = bolHasChanges;

    if (!oCalendar.hasChanges && oCalData != null && oCalData.CalendarData != null) {
        for (var i = 0; i < oCalData.CalendarData.length; i++) {
            if (oCalData.CalendarData[i].PeriodData.DayData != null) {
                for (var x = 0; x < oCalData.CalendarData[i].PeriodData.DayData.length; x++) {
                    oCalData.CalendarData[i].PeriodData.DayData[x].HasChanged = false;
                }
            }
        }
    }

    if (!bolHasChanges && oCalendar.capatityEnabled) {
        var oTmpCal = oCalendar;
        var distinctKeys = oTmpCal.capacityList[Object.keys(oTmpCal.capacityList)[0]];

        if (typeof (distinctKeys) != 'undefined') {
            Object.keys(distinctKeys).forEach(function (workcenterKey, index) {
                var cellID = null;
                if (oTmpCal.viewRange == Robotics.Client.Constants.ViewRange.Daily) {
                    for (var x = oTmpCal.getMinDailyCell(); x < oTmpCal.getMaxDailyCell(); x++) {
                        cellID = oTmpCal.oCalendar.CalendarHeader.PeriodHeaderData[x].Row1Text + "_" + oTmpCal.oCalendar.CalendarHeader.PeriodHeaderData[x].Row2Text;
                        cellID = cellID.replace(':', '_');
                        if (typeof (oTmpCal.columnCapacityList[cellID][workcenterKey]) != 'undefined') {
                            oTmpCal.columnCapacityList[cellID][workcenterKey].HasChanges = bolHasChanges;
                        }
                    }
                } else {
                    for (var x = 0; x < oTmpCal.oCalendar.CalendarHeader.PeriodHeaderData.length; x++) {
                        cellID = oTmpCal.oCalendar.CalendarHeader.PeriodHeaderData[x].Row1Text + "_" + oTmpCal.oCalendar.CalendarHeader.PeriodHeaderData[x].Row2Text;
                        cellID = cellID.replace(':', '_');
                        if (typeof (oTmpCal.columnCapacityList[cellID][workcenterKey]) != 'undefined') {
                            oTmpCal.columnCapacityList[cellID][workcenterKey].HasChanges = bolHasChanges;
                        }
                    }
                }
            });
        }
    }

    try {
        hasChanges(bolHasChanges);
    } catch (e) { }
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.create = function () {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    this.buildHTMLStructure();

    oCalendar.pageLayout = oCalendar.container.layout(this.pageLayoutOptions);

    $('#' + oCalendar.prefix + Robotics.Client.Constants.LayoutNames.ColumnTabs).tabs();
    $('#' + oCalendar.prefix + Robotics.Client.Constants.LayoutNames.RowTabs).tabs();

    var clientMode = this;
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.buildHTMLStructure = function () {
    var oClientMode = this;
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    //var mainDiv= $('<div style="width:100%;height:calc(100% - 155px)"></div>')

    var mainCenterLayout = $('<div></div>').attr('id', oCalendar.prefix + Robotics.Client.Constants.LayoutNames.Center).attr('class', 'ui-layout-center'); //'mainLeftCalendar'

    //Panel para el calendario
    var calendarLayout = $('<div></div>').attr('id', oCalendar.prefix + Robotics.Client.Constants.LayoutNames.Calendar).attr('class', 'ui-layout-center'); //topCalendar
    //Panel para el listado de tablas resumen inferior
    var columnInfoLayout = $('<div></div>').attr('id', oCalendar.prefix + Robotics.Client.Constants.LayoutNames.ColumnTabs).attr('class', 'ui-layout-south container tabs'); //bottomCalendar

    //Listado de tabs que se incluyen en el listado inferior
    var columnTabsList = $('<ul></ul>').attr('id', 'tabbuttonsColumn');
    var shiftsTab = $('<li></li>').attr('class', 'tab1').attr('style', 'cursor:pointer').append($('<a></a>').attr('href', '#' + oCalendar.prefix + Robotics.Client.Constants.LayoutNames.ColumnTabShifts).html(oCalendar.translator.translate(Robotics.Client.Language.Tags.Scheduler_Shifts)));
    shiftsTab.on('click', this.clickColumnShiftsTableHeader(this));

    var capacityTab = $('<li></li>').attr('class', 'tab1').attr('style', 'cursor:pointer').append($('<a></a>').attr('href', '#' + oCalendar.prefix + Robotics.Client.Constants.LayoutNames.ColumnTabCapacity).html(Globalize.formatMessage('roCapacityTab')));
    capacityTab.on('click', this.clickColumnCapacityTableHeader(this));

    if (oCalendar.isScheduleActive) {
        var assignmentsTab = $('<li></li>').attr('class', 'tab2').attr('style', 'cursor:pointer').append($('<a></a>').attr('href', '#' + oCalendar.prefix + Robotics.Client.Constants.LayoutNames.ColumnTabAssignments).html(oCalendar.translator.translate(Robotics.Client.Language.Tags.Scheduler_Assignments)));
        assignmentsTab.on('click', this.clickColumnAssignmentsTableHeader(this));

        var indictmentsTab = $('<li></li>').attr('class', 'tab3').attr('style', 'cursor:pointer').append($('<a></a>').attr('href', '#' + oCalendar.prefix + Robotics.Client.Constants.LayoutNames.ColumnTabIndictments).html(Globalize.formatMessage('roIndictmentsTab')));
        indictmentsTab.on('click', this.clickColumnIndictmentsTableHeader(this));

        columnTabsList.append(shiftsTab, assignmentsTab, indictmentsTab);
    } else if (oCalendar.saasPremiumActive) {
        var indictmentsTab = $('<li></li>').attr('class', 'tab3').attr('style', 'cursor:pointer').append($('<a></a>').attr('href', '#' + oCalendar.prefix + Robotics.Client.Constants.LayoutNames.ColumnTabIndictments).html(Globalize.formatMessage('roIndictmentsTab')));
        indictmentsTab.on('click', this.clickColumnIndictmentsTableHeader(this));

        columnTabsList.append(shiftsTab, indictmentsTab);
    } else {
        columnTabsList.append(shiftsTab);
    }

    if (oCalendar.telecommuteEnabled) columnTabsList.append(capacityTab);

    //Paneles con el contenido de los tabs
    var columnTabsPanel = $('<div></div>').attr('id', 'tabpanelsColumn').attr('style', 'border-top: 0');

    var loadFunc = function (calendar) {
        return function () {
            if (calendar.processingloadingClick == false) {
                calendar.processingloadingClick = true;
                $('#columnWaitingIcon').addClass("loadingMove");
                $('#rowWaitingIcon').addClass("loadingMove");

                setTimeout(function () {
                    if (!calendar.resumeInfoLoaded) calendar.generateShiftTotalizerInfo();
                    calendar.loadRelatedTables();
                    calendar.processingloadingClick = false;
                }, 100);
            }
        };
    }

    var waitingPanel = $('<div></div>').attr('id', 'tabColumnWaiting').attr('style', 'display:none;');
    waitingPanel.append($('<div id="columnWaitingIcon" class="loadingRelatedImg"></div>').on('click', loadFunc(oClientMode)));

    columnTabsPanel.append(waitingPanel);
    columnTabsPanel.append($('<div></div>').attr('id', oCalendar.prefix + Robotics.Client.Constants.LayoutNames.ColumnTabShifts).attr('class', 'container columnTableMain'));
    if (oCalendar.isScheduleActive) columnTabsPanel.append($('<div></div>').attr('id', oCalendar.prefix + Robotics.Client.Constants.LayoutNames.ColumnTabAssignments).attr('class', 'container columnTableMain'));
    if (oCalendar.isScheduleActive || oCalendar.saasPremiumActive) columnTabsPanel.append($('<div></div>').attr('id', oCalendar.prefix + Robotics.Client.Constants.LayoutNames.ColumnTabIndictments).attr('class', 'container columnTableMain'));
    columnTabsPanel.append($('<div></div>').attr('id', oCalendar.prefix + Robotics.Client.Constants.LayoutNames.ColumnTabCapacity).attr('class', 'container columnTableMain'));

    columnInfoLayout.append(columnTabsList, columnTabsPanel);

    // Añadimos todo al panel central
    mainCenterLayout.append(calendarLayout, columnInfoLayout);

    var mainLeftLayout = $('<div></div>').attr('id', oCalendar.prefix + Robotics.Client.Constants.LayoutNames.East).attr('class', 'ui-layout-east'); //mainRightCalendar

    //Panel para el calendario
    var rowInfoLayout = $('<div></div>').attr('id', oCalendar.prefix + Robotics.Client.Constants.LayoutNames.RowTabs).attr('class', 'ui-layout-center container tabs'); //topInfo

    //Listado de tabs que se incluyen en el listado inferior
    var rowTabsList = $('<ul></ul>').attr('id', 'tabbuttonsRow');
    var accrualsTab = $('<li></li>').attr('class', 'tab1').attr('style', 'cursor:pointer').append($('<a></a>').attr('href', '#' + oCalendar.prefix + Robotics.Client.Constants.LayoutNames.RowTabAccruals).html(oCalendar.translator.translate(Robotics.Client.Language.Tags.Scheduler_Concepts)));
    accrualsTab.on('click', this.clickRowAccrualsTableHeader(this));
    var shiftsRowTab = $('<li></li>').attr('class', 'tab2').attr('style', 'cursor:pointer').append($('<a></a>').attr('href', '#' + oCalendar.prefix + Robotics.Client.Constants.LayoutNames.RowTabShifts).html(oCalendar.translator.translate(Robotics.Client.Language.Tags.Scheduler_Shifts)));
    shiftsRowTab.on('click', this.clickRowShiftsTableHeader(this));

    if (oCalendar.isScheduleActive) {
        var assignmentsRowTab = $('<li></li>').attr('class', 'tab3').attr('style', 'cursor:pointer').append($('<a></a>').attr('href', '#' + oCalendar.prefix + Robotics.Client.Constants.LayoutNames.RowTabAssignments).html(oCalendar.translator.translate(Robotics.Client.Language.Tags.Scheduler_Assignments)));
        assignmentsRowTab.on('click', this.clickRowAssignmentsTableHeader(this));
        rowTabsList.append(accrualsTab, shiftsRowTab, assignmentsRowTab);
    } else {
        rowTabsList.append(accrualsTab, shiftsRowTab);
    }

    //Paneles con el contenido de los tabs
    var rowTabsPanel = $('<div></div>').attr('id', 'tabpanelsRow').attr('style', 'border-top: 0');

    var waitingRowPanel = $('<div></div>').attr('id', 'tabRowWaiting').attr('style', 'display:none;');
    waitingRowPanel.append($('<div id="rowWaitingIcon" class="rowLoadingRelatedImg"></div>').on('click', loadFunc(oClientMode)));

    rowTabsPanel.append(waitingRowPanel);
    rowTabsPanel.append($('<div></div>').attr('id', oCalendar.prefix + Robotics.Client.Constants.LayoutNames.RowTabAccruals).attr('class', 'container rowTableMain'));
    rowTabsPanel.append($('<div></div>').attr('id', oCalendar.prefix + Robotics.Client.Constants.LayoutNames.RowTabShifts).attr('class', 'container rowTableMain'));
    if (oCalendar.isScheduleActive) rowTabsPanel.append($('<div></div>').attr('id', oCalendar.prefix + Robotics.Client.Constants.LayoutNames.RowTabAssignments).attr('class', 'container rowTableMain'));

    rowInfoLayout.append(rowTabsList, rowTabsPanel);

    var resumeDiv = $('<div></div>');
    var waitingResumePanel = $('<div></div>').attr('id', 'tabResumeWaiting').attr('style', 'display:none;');
    //Panel para el listado de tablas resumen inferior
    var resumeLayout = $('<div></div>').attr('id', oCalendar.prefix + Robotics.Client.Constants.LayoutNames.Resume).attr('class', 'ui-layout-south'); //bottomInfo

    // Añadimos todo al panel lateral
    mainLeftLayout.append(rowInfoLayout, resumeDiv.append(waitingResumePanel, resumeLayout));

    //oCalendar.container.append(mainDiv.append(mainCenterLayout, mainLeftLayout));
    oCalendar.container.append(mainCenterLayout, mainLeftLayout);
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.synchronizeRowLayout = function (isShown, oScheduleCalendar) {
    return function () {
        if (isShown) {
            if (oScheduleCalendar != null && oScheduleCalendar.oBaseControl.pageLayout.east.children.tabsContainerLayout.south != false && oScheduleCalendar.oBaseControl.pageLayout.east.children.tabsContainerLayout.south.state.isClosed) oScheduleCalendar.oBaseControl.pageLayout.east.children.tabsContainerLayout.toggle("south");

            $('#columnWaitingIcon').addClass("loadingMove");
            $('#rowWaitingIcon').addClass("loadingMove");
            $('#tabColumnWaiting').show();
            $('#tabRowWaiting').show();
            $('#tabResumeWaiting').show();

            if (!oScheduleCalendar.resumeInfoLoaded) oScheduleCalendar.generateShiftTotalizerInfo();

            oScheduleCalendar.loadSouthTables();
            oScheduleCalendar.loadResumeTables();
            oScheduleCalendar.prepareLayoutHeight(true);

            if (oScheduleCalendar.oBaseControl.pageLayout.east != false && !oScheduleCalendar.oBaseControl.pageLayout.east.state.isClosed) {
                oScheduleCalendar.prepareLayoutWidth(true);
            } else {
                oScheduleCalendar.prepareLayoutWidth(false);
            }

            $('#columnWaitingIcon').removeClass("loadingMove");
            $('#rowWaitingIcon').removeClass("loadingMove");
            $('#tabColumnWaiting').hide();
            $('#tabRowWaiting').hide();
            $('#tabResumeWaiting').hide();
        } else {
            if (oScheduleCalendar != null && oScheduleCalendar.oBaseControl.pageLayout.east.children.tabsContainerLayout.south != false && !oScheduleCalendar.oBaseControl.pageLayout.east.children.tabsContainerLayout.south.state.isClosed) oScheduleCalendar.oBaseControl.pageLayout.east.children.tabsContainerLayout.toggle("south");
            oScheduleCalendar.prepareLayoutHeight(false);
        }

        prepareFixedHeight();
    };
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.synchronizeColumnLayout = function (isShown, oScheduleCalendar) {
    return function () {
        if (isShown) {
            if (oScheduleCalendar != null && !oScheduleCalendar.oBaseControl.pageLayout.center.children.tabsContainerLayout.south.state.isClosed) {
                oScheduleCalendar.oBaseControl.pageLayout.east.children.tabsContainerLayout.show("south");
            } else {
                oScheduleCalendar.oBaseControl.pageLayout.east.children.tabsContainerLayout.hide("south");
            }

            $('#columnWaitingIcon').addClass("loadingMove");
            $('#rowWaitingIcon').addClass("loadingMove");
            $('#tabColumnWaiting').show();
            $('#tabRowWaiting').show();
            $('#tabResumeWaiting').show();

            if (!oScheduleCalendar.resumeInfoLoaded) oScheduleCalendar.generateShiftTotalizerInfo();

            oScheduleCalendar.loadLeftTables();
            oScheduleCalendar.loadResumeTables();
            oScheduleCalendar.prepareLayoutWidth(true);

            if (oScheduleCalendar.oBaseControl.pageLayout.center.children.tabsContainerLayout.south == false || oScheduleCalendar.oBaseControl.pageLayout.center.children.tabsContainerLayout.south.state.isClosed) {
                oScheduleCalendar.prepareLayoutHeight(false);
            } else {
                oScheduleCalendar.prepareLayoutHeight(true);
            }

            $('#columnWaitingIcon').removeClass("loadingMove");
            $('#rowWaitingIcon').removeClass("loadingMove");
            $('#tabColumnWaiting').hide();
            $('#tabRowWaiting').hide();
            $('#tabResumeWaiting').hide();
        } else {
            oScheduleCalendar.prepareLayoutWidth(false);
        }

        prepareFixedHeight();
    };
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.prepareFixedHeight = function () {
    var oScheduleCalendar = this;

    if (oScheduleCalendar.oBaseControl.pageLayout.center.children.tabsContainerLayout.south == false || oScheduleCalendar.oBaseControl.pageLayout.center.children.tabsContainerLayout.south.state.isClosed) {
        oScheduleCalendar.prepareLayoutHeight(false);
    } else {
        oScheduleCalendar.prepareLayoutHeight(true);
    }

    if (oScheduleCalendar.oBaseControl.pageLayout.east != false && !oScheduleCalendar.oBaseControl.pageLayout.east.state.isClosed) {
        oScheduleCalendar.prepareLayoutWidth(true);
    } else {
        oScheduleCalendar.prepareLayoutWidth(false);
    }
}

Robotics.Client.Controls.roSchedulerCalendar.prototype.prepareLayoutHeight = function (isShown) {
    var oScheduleCalendar = this;
    var oCalendar = this.oBaseControl;

    if (isShown) {
        $('#' + oScheduleCalendar.oBaseControl.prefix + Robotics.Client.Constants.LayoutNames.Calendar + ' .fht-fixed-body .fht-tbody').css("height", 'calc(100vh - 370px)');
        $('#' + oScheduleCalendar.oBaseControl.prefix + Robotics.Client.Constants.LayoutNames.Calendar + ' .fht-fixed-column .fht-tbody').css("height", 'calc(100vh - 388px)');

        $('#' + oScheduleCalendar.oBaseControl.prefix + Robotics.Client.Constants.LayoutNames.RowTabAccruals + ' .fht-table-wrapper').css("height", 'calc(100vh - 250px)');
        $('#' + oScheduleCalendar.oBaseControl.prefix + Robotics.Client.Constants.LayoutNames.RowTabAccruals + ' .fht-table-wrapper .fht-tbody').css("height", 'calc(100vh - 390px)');

        $('#' + oScheduleCalendar.oBaseControl.prefix + Robotics.Client.Constants.LayoutNames.ColumnTabShifts + ' .fht-fixed-column .fht-tbody').css("height", '116px');
        $('#' + oScheduleCalendar.oBaseControl.prefix + Robotics.Client.Constants.LayoutNames.ColumnTabAssignments + ' .fht-fixed-column .fht-tbody').css("height", '116px');
        $('#' + oScheduleCalendar.oBaseControl.prefix + Robotics.Client.Constants.LayoutNames.ColumnTabShifts + ' .fht-fixed-column .fht-tfoot').css("top", '0px');
        $('#' + oScheduleCalendar.oBaseControl.prefix + Robotics.Client.Constants.LayoutNames.ColumnTabAssignments + ' .fht-fixed-column .fht-tfoot').css("top", '0px');
    } else {
        $('#' + oScheduleCalendar.oBaseControl.prefix + Robotics.Client.Constants.LayoutNames.Calendar + ' .fht-fixed-body .fht-tbody').css("height", 'calc(100vh - 235px)');
        $('#' + oScheduleCalendar.oBaseControl.prefix + Robotics.Client.Constants.LayoutNames.Calendar + ' .fht-fixed-column .fht-tbody').css("height", 'calc(100vh - 254px)');

        $('#' + oScheduleCalendar.oBaseControl.prefix + Robotics.Client.Constants.LayoutNames.RowTabAccruals + ' .fht-table-wrapper').css("height", 'calc(100vh - 205px)');
        $('#' + oScheduleCalendar.oBaseControl.prefix + Robotics.Client.Constants.LayoutNames.RowTabAccruals + ' .fht-table-wrapper .fht-tbody').css("height", 'calc(100vh - 250px)');

        $('#' + oScheduleCalendar.oBaseControl.prefix + Robotics.Client.Constants.LayoutNames.ColumnTabShifts + ' .fht-fixed-column .fht-tbody').css("height", '116px');
        $('#' + oScheduleCalendar.oBaseControl.prefix + Robotics.Client.Constants.LayoutNames.ColumnTabAssignments + ' .fht-fixed-column .fht-tbody').css("height", '116px');
        $('#' + oScheduleCalendar.oBaseControl.prefix + Robotics.Client.Constants.LayoutNames.ColumnTabShifts + ' .fht-fixed-column .fht-tfoot').css("top", '0px');
        $('#' + oScheduleCalendar.oBaseControl.prefix + Robotics.Client.Constants.LayoutNames.ColumnTabAssignments + ' .fht-fixed-column .fht-tfoot').css("top", '0px');
    }

    $('#' + oCalendar.prefix + Robotics.Client.Constants.TableNames.RowAccruals).parent().scrollTop($('#' + oCalendar.prefix + Robotics.Client.Constants.TableNames.Calendar).parent().scrollTop());
}

Robotics.Client.Controls.roSchedulerCalendar.prototype.prepareLayoutWidth = function (isShown) {
    var oScheduleCalendar = this;
    var oCalendar = this.oBaseControl;
    if (isShown) {
        $('#' + oScheduleCalendar.oBaseControl.prefix + Robotics.Client.Constants.LayoutNames.Calendar + ' .fht-fixed-body .fht-tbody').css("width", 'calc(100vw - 270px)');
        $('#' + oScheduleCalendar.oBaseControl.prefix + Robotics.Client.Constants.LayoutNames.Calendar + ' .fht-fixed-body .fht-thead').css("width", 'calc(100vw - 270px)');

        $('#' + oScheduleCalendar.oBaseControl.prefix + Robotics.Client.Constants.LayoutNames.ColumnTabShifts + ' .fht-fixed-body .fht-tbody').css("width", 'calc(100vw - 270px)');
        $('#' + oScheduleCalendar.oBaseControl.prefix + Robotics.Client.Constants.LayoutNames.ColumnTabShifts + ' .fht-fixed-body .fht-thead').css("width", 'calc(100vw - 270px)');
        $('#' + oScheduleCalendar.oBaseControl.prefix + Robotics.Client.Constants.LayoutNames.ColumnTabShifts + ' .fht-fixed-body .fht-tfoot').css("width", 'calc(100vw - 270px)');

        $('#' + oScheduleCalendar.oBaseControl.prefix + Robotics.Client.Constants.LayoutNames.ColumnTabAssignments + ' .fht-fixed-body .fht-tbody').css("width", 'calc(100vw - 270px)');
        $('#' + oScheduleCalendar.oBaseControl.prefix + Robotics.Client.Constants.LayoutNames.ColumnTabAssignments + ' .fht-fixed-body .fht-thead').css("width", 'calc(100vw - 270px)');
        $('#' + oScheduleCalendar.oBaseControl.prefix + Robotics.Client.Constants.LayoutNames.ColumnTabAssignments + ' .fht-fixed-body .fht-tfoot').css("width", 'calc(100vw - 270px)');

        $('#' + oScheduleCalendar.oBaseControl.prefix + Robotics.Client.Constants.LayoutNames.ColumnTabCapacity + ' .fht-fixed-body .fht-tbody').css("width", 'calc(100vw - 270px)');
        $('#' + oScheduleCalendar.oBaseControl.prefix + Robotics.Client.Constants.LayoutNames.ColumnTabCapacity + ' .fht-fixed-body .fht-thead').css("width", 'calc(100vw - 270px)');
        $('#' + oScheduleCalendar.oBaseControl.prefix + Robotics.Client.Constants.LayoutNames.ColumnTabCapacity + ' .fht-fixed-body .fht-tfoot').css("width", 'calc(100vw - 270px)');
    } else {
        $('#' + oScheduleCalendar.oBaseControl.prefix + Robotics.Client.Constants.LayoutNames.Calendar + ' .fht-fixed-body .fht-tbody').css("width", 'calc(100vw - 100px)');
        $('#' + oScheduleCalendar.oBaseControl.prefix + Robotics.Client.Constants.LayoutNames.Calendar + ' .fht-fixed-body .fht-thead').css("width", 'calc(100vw - 100px)');

        $('#' + oScheduleCalendar.oBaseControl.prefix + Robotics.Client.Constants.LayoutNames.ColumnTabShifts + ' .fht-fixed-body .fht-tbody').css("width", 'calc(100vw - 100px)');
        $('#' + oScheduleCalendar.oBaseControl.prefix + Robotics.Client.Constants.LayoutNames.ColumnTabShifts + ' .fht-fixed-body .fht-thead').css("width", 'calc(100vw - 100px)');
        $('#' + oScheduleCalendar.oBaseControl.prefix + Robotics.Client.Constants.LayoutNames.ColumnTabShifts + ' .fht-fixed-body .fht-tfoot').css("width", 'calc(100vw - 100px)');

        $('#' + oScheduleCalendar.oBaseControl.prefix + Robotics.Client.Constants.LayoutNames.ColumnTabAssignments + ' .fht-fixed-body .fht-tbody').css("width", 'calc(100vw - 100px)');
        $('#' + oScheduleCalendar.oBaseControl.prefix + Robotics.Client.Constants.LayoutNames.ColumnTabAssignments + ' .fht-fixed-body .fht-thead').css("width", 'calc(100vw - 100px)');
        $('#' + oScheduleCalendar.oBaseControl.prefix + Robotics.Client.Constants.LayoutNames.ColumnTabAssignments + ' .fht-fixed-body .fht-tfoot').css("width", 'calc(100vw - 100px)');

        $('#' + oScheduleCalendar.oBaseControl.prefix + Robotics.Client.Constants.LayoutNames.ColumnTabCapacity + ' .fht-fixed-body .fht-tbody').css("width", 'calc(100vw - 100px)');
        $('#' + oScheduleCalendar.oBaseControl.prefix + Robotics.Client.Constants.LayoutNames.ColumnTabCapacity + ' .fht-fixed-body .fht-thead').css("width", 'calc(100vw - 100px)');
        $('#' + oScheduleCalendar.oBaseControl.prefix + Robotics.Client.Constants.LayoutNames.ColumnTabCapacity + ' .fht-fixed-body .fht-tfoot').css("width", 'calc(100vw - 100px)');
    }

    $('#' + oCalendar.prefix + Robotics.Client.Constants.TableNames.ColShifts).parent().scrollLeft($('#' + oCalendar.prefix + Robotics.Client.Constants.TableNames.Calendar).parent().scrollLeft());
}

Robotics.Client.Controls.roSchedulerCalendar.prototype.synchronizeSummaryLayout = function (isShown, oScheduleCalendar) {
    return function () {
        if (isShown) {
            if (oScheduleCalendar != null && oScheduleCalendar.oBaseControl.pageLayout.center.children.tabsContainerLayout.south.state.isClosed) oScheduleCalendar.oBaseControl.pageLayout.center.children.tabsContainerLayout.toggle("south");
            oScheduleCalendar.loadResumeTables();
        } else {
            if (oScheduleCalendar != null && !oScheduleCalendar.oBaseControl.pageLayout.center.children.tabsContainerLayout.south.state.isClosed) oScheduleCalendar.oBaseControl.pageLayout.center.children.tabsContainerLayout.toggle("south");
        }
    };
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.onresize = function (oScheduleCalendar) {
    return function () {
        oScheduleCalendar.loadingFunctionExtended(true);
        oScheduleCalendar.prepareFixedHeight();
        oScheduleCalendar.loadingFunctionExtended(false);
    };
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.loadingFunctionExtended = function (showLoading) {
    var oCal = this.oBaseControl;

    if ((oCal.isShowingLoader && !showLoading) || (!oCal.isShowingLoader && showLoading)) {
        oCal.isShowingLoader = showLoading;

        if (showLoading) {
            $('#loadingCalendar').show();
        } else {
            if (oCal.pageLayout.east.state.isClosed) {
                setTimeout(function () {
                    if (oCal.typeView == Robotics.Client.Constants.TypeView.Planification) {
                        $('#' + oCal.prefix + 'East-resizer').css('position', 'inherit');
                        $('#' + oCal.prefix + 'Center').css('height', (parseInt($('#' + oCal.prefix + 'Center').css('height'), 10) - 20) + 'px');
                        $('#' + oCal.prefix + 'Center').css('width', (parseInt($('#' + oCal.prefix + 'Center').css('width'), 10) - 20) + 'px');
                    } else {
                        $('#' + oCal.prefix + 'Center').css('height', (parseInt($('#' + oCal.prefix + 'Center').css('height'), 10) - 20) + 'px');
                        $('#' + oCal.prefix + 'Center').css('width', (parseInt($('#' + oCal.prefix + 'Center').css('width'), 10) - 20) + 'px');
                    }
                    setTimeout(function () {
                        if (oCal.typeView == Robotics.Client.Constants.TypeView.Planification) {
                            $('#' + oCal.prefix + 'East-resizer').css('position', 'absolute');
                            $('#' + oCal.prefix + 'Center').css('height', (parseInt($('#' + oCal.prefix + 'Center').css('height'), 10) + 20) + 'px');
                            $('#' + oCal.prefix + 'Center').css('width', (parseInt($('#' + oCal.prefix + 'Center').css('width'), 10) + 20) + 'px');
                        } else {
                            $('#' + oCal.prefix + 'Center').css('height', (parseInt($('#' + oCal.prefix + 'Center').css('height'), 10) + 20) + 'px');
                            $('#' + oCal.prefix + 'Center').css('width', (parseInt($('#' + oCal.prefix + 'Center').css('width'), 10) + 20) + 'px');
                        }

                        $('#loadingCalendar').hide();
                    }, 50);
                }, 50);
            } else {
                $('#loadingCalendar').hide();
            }
        }

        oCal.loadingFunc(showLoading);
    }
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.getContextMenuSelector = function () {
    var oClientMode = this;
    var oCal = this.oBaseControl;

    var selector = '';

    if (oCal.typeView == Robotics.Client.Constants.TypeView.Planification) {
        selector = oCal.viewRange == Robotics.Client.Constants.ViewRange.Daily ? '.calendarDailyBodyCell' : '.calendarOuterBodyCell';
    } else {
        selector = '.reviewDailyBodyCell';
    }

    return selector;
}

Robotics.Client.Controls.roSchedulerCalendar.prototype.getContextMenuHeaderSelector = function () {
    var oClientMode = this;
    var oCal = this.oBaseControl;

    var selector = '.CalendarDayFixedHeader';

    return selector;
}

Robotics.Client.Controls.roSchedulerCalendar.prototype.processKeyUpEvent = function (e) {
    var oClientMode = this;

    oClientMode.cancelCurrentMultipleSelect(null, false);
    oClientMode.cancelCurrentMultipleHeaderSelect(null, false);
}

Robotics.Client.Controls.roSchedulerCalendar.prototype.processKeyDownEvent = function (e) {
    var oClientMode = this;
    var oCal = this.oBaseControl;

    if (oCal.ctrlDown && e.keyCode == oCal.cKey) {
        if (oCal.selectedMinHeaderColumn != -1 && oCal.selectedMaxHeaderColumn != -1) {
            oClientMode.copyHeaderSelection(oCal.selectedHeaderContainer);
        } else {
            oClientMode.copySelection(oCal.selectedContainer, true, true, true);
        }
    }
    if (oCal.ctrlDown && e.keyCode == oCal.vKey) {
        if (oCal.selectionCopied) oClientMode.pasteSelection(oCal.selectedContainer);
        else if (oCal.selectionHeaderCopied) oClientMode.pasteHeaderSelection(oCal.selectedHeaderContainer);
    }
}

Robotics.Client.Controls.roSchedulerCalendar.prototype.onDrop = function (event) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    this.setSingleSelectedObject($(this.dragTarget));

    var srcElement = null;

    if (typeof (event.srcElement) != 'undefined') {
        srcElement = $(event.srcElement);
    } else {
        srcElement = $(event.originalEvent.target);
    }

    if (typeof srcElement.attr("data-IDShift") != 'undefined') {
        var advParams = JSON.parse(srcElement.attr("data-AdvParameters"));

        var isStarterShift = false;
        if (typeof advParams != 'undefined' && advParams != null && advParams.length > 0 && advParams.find(function (x) { return x.Name.toLowerCase() == 'starter' && x.Value == '1' }) != null) {
            isStarterShift = true;
        }

        oCalendar.showAssignmentsDialog = false;
        oCalendar.showStarterDialog = false;
        oCalendar.showComplementaryAssignDialog = false;

        if (isStarterShift) {
            oCalendar.showStarterDialog = true;
        } else {
            if (parseInt(srcElement.attr("data-AllowAssignments"), 10) == 1) oCalendar.showAssignmentsDialog = true;
            if (parseInt(srcElement.attr("data-AllowComplementary"), 10) == 1 || parseInt(srcElement.attr("data-AllowFloatingData"), 10) == 1) oCalendar.showComplementaryAssignDialog = true;
        }

        var ShiftTypeVal = Robotics.Client.Constants.ShiftType.Normal;

        switch (srcElement.attr("data-ShiftType")) {
            case "0":
                ShiftTypeVal = Robotics.Client.Constants.ShiftType.Normal;
                break;
            case "1":
                oCalendar.showComplementaryAssignDialog = true;
                ShiftTypeVal = Robotics.Client.Constants.ShiftType.NormalFloating;
                break;
            case "2":
                ShiftTypeVal = Robotics.Client.Constants.ShiftType.Holiday;
                break;
            case "3":
                ShiftTypeVal = Robotics.Client.Constants.ShiftType.Holiday_NonWorking;
                break;
        }

        var mainShift = {
            ID: parseInt(srcElement.attr("data-IDShift"), 10),
            ShortName: srcElement.attr("data-ShortName"),
            PlannedHours: parseFloat(srcElement.attr("data-ShiftHours")),
            Color: srcElement.attr("data-ShiftColor"),
            Name: srcElement.attr("data-Name"),
            Type: ShiftTypeVal,
            StartHour: moment(srcElement.attr("data-StartHour"), "YYYY/MM/DD HH:mm").toDate(),
            AdvancedParameters: advParams
        };

        if (srcElement.attr("data-EndHour") != '') {
            mainShift.EndHour = moment(srcElement.attr("data-EndHour"), "YYYY/MM/DD HH:mm").toDate();
        }

        if (oCalendar.showComplementaryAssignDialog || oCalendar.showAssignmentsDialog || oCalendar.showStarterDialog) {
            if (oCalendar.showComplementaryAssignDialog) oCalendar.complementaryShift = mainShift;
            if (oCalendar.showAssignmentsDialog) oCalendar.assignmentShift = mainShift;
            if (oCalendar.showStarterDialog) oCalendar.starterShift = mainShift;

            if (typeof (oCalendar.shiftsExtendedDataCache[mainShift.ID]) == 'undefined') {
                var oParameters = {};
                oParameters.idShift = mainShift.ID;
                oParameters.StampParam = new Date().getMilliseconds();
                oCalendar.performAction(oParameters, Robotics.Client.Constants.Actions.ShiftLayerDefinition);
            } else {
                if (oCalendar.showComplementaryAssignDialog) oCalendar.prepareComplementaryDialog(null);
                else if (oCalendar.showAssignmentsDialog) oCalendar.prepareAssignmentsDialog(null);
                else if (oCalendar.showStarterDialog) oCalendar.prepareStarterDialog(null);
            }
        } else {
            oCalendar.complementaryShift = null;
            oCalendar.assignmentShift = null;

            if (!oCalendar.isBatchMode()) {
                if (mainShift.Type == Robotics.Client.Constants.ShiftType.Normal || mainShift.Type == Robotics.Client.Constants.ShiftType.NormalFloating) {
                    this.assignShiftToDay(oCalendar.selectedContainer, oCalendar.selectedDay, mainShift, null, null, false, true, true, oCalendar.selectedEmployee.EmployeeData);//.Assignments, oCalendar.selectedEmployee.EmployeeData.FreezingDate);
                } else {
                    this.assignShiftToDay(oCalendar.selectedContainer, oCalendar.selectedDay, null, mainShift, oCalendar.selectedDay.AssigData, oCalendar.selectedDay.AllowAssignment, true, true, oCalendar.selectedEmployee.EmployeeData);//.Assignments, oCalendar.selectedEmployee.EmployeeData.FreezingDate);
                }
            } else {
                if (mainShift.Type == Robotics.Client.Constants.ShiftType.Normal || mainShift.Type == Robotics.Client.Constants.ShiftType.NormalFloating) {
                    this.assignShiftToDayBatch(mainShift, null, null, false, true, true);
                } else {
                    this.assignShiftToDayBatch(null, mainShift, undefined, undefined, true, true);
                }
            }

            this.waitForUserToRefresh();
        }
    }
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.alteLoadPunchesModeAndReload = function () {
    var oClientMode = this;
    var oCalendar = this.oBaseControl;

    if (oCalendar.loadPunches == false) oCalendar.loadPunches = true;
    else oCalendar.loadPunches = false;

    if (oCalendar.loadPunches) {
        $('.LoadPunches').addClass('showPunches-active');
        $('.LoadPunches').removeClass('showPunches-inactive');
    } else {
        $('.LoadPunches').removeClass('showPunches-active');
        $('.LoadPunches').addClass('showPunches-inactive');
    }

    oCalendar.setFiltersCookieValue();

    oClientMode.loadData();
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.alterLoadCapacitiesModeAndReload = function () {
    var oClientMode = this;
    var oCalendar = this.oBaseControl;

    if (oCalendar.loadCapacities == false) oCalendar.loadCapacities = true;
    else oCalendar.loadCapacities = false;

    if (oCalendar.loadCapacities) {
        $('.LoadCapacities').addClass('showCapacities-active');
        $('.LoadCapacities').removeClass('showCapacities-inactive');
    } else {
        $('.LoadCapacities').removeClass('showCapacities-active');
        $('.LoadCapacities').addClass('showCapacities-inactive');
    }

    oCalendar.setFiltersCookieValue();

    if (oCalendar.hasChanges) {
    } else {
        oClientMode.loadData();
    }
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.alterLoadIndictmentsModeAndReload = function () {
    var oClientMode = this;
    var oCalendar = this.oBaseControl;

    if (oCalendar.isScheduleActive || oCalendar.saasPremiumActive) {
        if (oCalendar.loadIndictments == false) oCalendar.loadIndictments = true;
        else oCalendar.loadIndictments = false;
    } else {
        oCalendar.loadIndictments = false;
    }

    if (oCalendar.loadIndictments) {
        $('.LoadIndictments').addClass('showIndictments-active');
        $('.LoadIndictments').removeClass('showIndictments-inactive');
    } else {
        $('.LoadIndictments').removeClass('showIndictments-active');
        $('.LoadIndictments').addClass('showIndictments-inactive');
    }

    oCalendar.setFiltersCookieValue();

    if (oCalendar.hasChanges) {
        if (oCalendar.loadIndictments) oClientMode.checkCalendarIndictments();
        else oCalendar.refreshTables(this, false, true);
    } else {
        oClientMode.loadData();
    }
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.changeDailyPeriod = function () {
    var oCalendar = this.oBaseControl;

    oCalendar.showDailyConfig();
}

Robotics.Client.Controls.roSchedulerCalendar.prototype.setRecursiveData = function () {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    oCalendar.loadRecursive = !oCalendar.loadRecursive;
    if (oCalendar.loadRecursive) {
        $('.RecursiveAction').addClass('minimizeGroup');
        $('.RecursiveAction').removeClass('expandGroup');
    } else {
        $('.RecursiveAction').removeClass('minimizeGroup');
        $('.RecursiveAction').addClass('expandGroup');
    }

    oCalendar.setFiltersCookieValue();
}

Robotics.Client.Controls.roSchedulerCalendar.prototype.changeLoadRecursive = function () {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    this.setRecursiveData();
    oCalendar.refresh();
}

Robotics.Client.Controls.roSchedulerCalendar.prototype.mapModeEvents = function () {
    var oClientMode = this;
    var oCal = this.oBaseControl;

    var selector = oCal.viewRange == Robotics.Client.Constants.ViewRange.Daily ? '.calendarDailyBodyCell' : '.calendarOuterBodyCell';

    $('.fullCalendarShow').off('click');
    $('.fullCalendarShow').on('click', function (e) {
        var url = 'Scheduler/AnnualView.aspx?EmployeeID=' + $(this).attr('data-IDEmployee') + "&Year=" + oCal.firstDate.getFullYear();
        var Title = '';
        parent.ShowExternalForm2(url, 950, 560, Title, '', true, false, false);
    });

    $(selector + ',.reviewDailyBodyCell').off('click');
    $(selector + ',.reviewDailyBodyCell').on('click', function (e) {
        if (!oCal.shiftDown) oClientMode.setSingleSelectedObject(this);
        else oClientMode.selectMultiple(oCal.selectedContainer, this);
    });

    $('.RecursiveAction').off('click');
    $('.RecursiveAction').on('click', function (e) {
        if (oCal.hasChanges == false) {
            oClientMode.changeLoadRecursive();
        } else {
            oCal.showChangesWarning(oCal.clientInstanceName + ".clientMode.changeLoadRecursive();");
        }
    });

    $('.DailyPeriod').off('click');
    $('.DailyPeriod').on('click', function (e) {
        if (oCal.hasChanges == false) {
            oClientMode.changeDailyPeriod();
        } else {
            oCal.showChangesWarning(oCal.clientInstanceName + ".clientMode.changeDailyPeriod();");
        }
    });

    $('.LoadIndictments').off('click');
    $('.LoadIndictments').on('click', function (e) {
        oClientMode.alterLoadIndictmentsModeAndReload();
    });

    $('.LoadCapacities').off('click');
    $('.LoadCapacities').on('click', function (e) {
        oClientMode.alterLoadCapacitiesModeAndReload();
    });

    $('.LoadPunches').off('click');
    $('.LoadPunches').on('click', function (e) {
        oClientMode.alteLoadPunchesModeAndReload();
    });

    $('.FilterAction').off('click');
    $('.FilterAction').on('click', function (e) {
        if (oCal.hasChanges == false) {
            oCal.openFilterAssignments();
        } else {
            oCal.showChangesWarning(oCal.clientInstanceName + ".openFilterAssignments();");
        }
    });

    $(selector + ',.reviewDailyBodyCell').off('dblclick');
    $(selector + ',.reviewDailyBodyCell').on('dblclick', function (e) {
        oClientMode.enterDetailAction($(this).attr('data-IDGroup'), $(this).attr('data-IDEmployee'), $(this).attr('data-Date'));
    });

    //$(selector).droppable({
    //    drop: function (event, ui) {
    //        return oClientMode.onDrop(event);
    //    }
    //});

    $(selector).off("dragenter");
    $(selector).off("dragover");

    $(selector).on("dragenter", function (event) {
        oClientMode.dragTarget = this;
        event.preventDefault();
        return false;
    });
    $(selector).on("dragover", function (event) {
        oClientMode.dragTarget = this;
        event.preventDefault();
        return false;
    });

    if (oCal.typeView == Robotics.Client.Constants.TypeView.Planification && oCal.viewRange != Robotics.Client.Constants.ViewRange.Daily) {
        try { $("#" + oCal.prefix + Robotics.Client.Constants.TableNames.Calendar + '_tbody').selectableScroll('destroy'); } catch (e) { }
        $("#" + oCal.prefix + Robotics.Client.Constants.TableNames.Calendar + '_tbody').selectableScroll({
            distance: 10,
            filter: '.calendarOuterBodyCell',
            stop: function (event, ui) { oClientMode.endSelectOperation(); },
            scrollElement: $('#' + oCal.prefix + Robotics.Client.Constants.LayoutNames.Calendar + ' .fht-fixed-body .fht-tbody')
        });
    } else {
        try { $("#" + oCal.prefix + Robotics.Client.Constants.TableNames.Calendar + '_tbody').selectableScroll('destroy'); } catch (e) { }
    }

    //Eventos de scheduling
    if (oCal.isScheduleActive && oCal.typeView == Robotics.Client.Constants.TypeView.Planification) {
        if (oCal.viewRange == Robotics.Client.Constants.ViewRange.Period && oCal.isScheduleActive) {
            try { $("#" + oCal.prefix + Robotics.Client.Constants.TableNames.Calendar + '_thead').selectableScroll('destroy'); } catch (e) { }
            $("#" + oCal.prefix + Robotics.Client.Constants.TableNames.Calendar + '_thead').selectableScroll({
                distance: 10,
                filter: '.CalendarDayFixedHeader',
                stop: function (event, ui) { oClientMode.endHeaderSelectOperation(); },
                scrollElement: $('#' + oCal.prefix + Robotics.Client.Constants.LayoutNames.Calendar + ' .fht-fixed-body .fht-thead')
            });
        }

        $('.CalendarDayFixedHeader').off('click');
        $('.CalendarDayFixedHeader').on('click', function (e) {
            if (!oCal.shiftDown) oClientMode.setSingleHeaderSelectedObejct(this);
            else oClientMode.selectHeaderMultiple(oCal.selectedHeaderContainer, this);
        });
    } else {
        $('.CalendarDayFixedHeader').off('click');
        try { $("#" + oCal.prefix + Robotics.Client.Constants.TableNames.Calendar + '_thead').selectableScroll('destroy'); } catch (e) { }
        $.contextMenu('destroy', '.CalendarDayFixedHeader');
    }

    if (oCal.viewRange == Robotics.Client.Constants.ViewRange.Daily) {
        $('#' + oCal.prefix + 'Main div.fht-fixed-body div.fht-tbody').scrollLeft((oCal.firstCellPrinted - oCal.getMinDailyCell() - 2) * 30);
        oCal.firstCellPrinted = -1;
    }

    //window.parent.frames['ifPrincipal'].window.focus();
    window.focus();
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.refreshModeTables = function (objectRef, isResizing, refreshCalendar) {
    var oScheduleCalendar = this;

    if (objectRef != null) {
        if (objectRef instanceof Robotics.Client.Controls.roCalendar) oScheduleCalendar = objectRef.clientMode;
        else oScheduleCalendar = objectRef;
    }

    if (refreshCalendar) {
        this.bRelatedTables = true;
        this.clearRelatedTables();
        oScheduleCalendar.refreshCalendarTable(isResizing);
    }

    if (refreshCalendar) {
        this.prepareFixedHeight();
    }

    this.refreshTooltips();
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.loadSouthTables = function () {
    var oScheduleCalendar = this;
    if (oScheduleCalendar.resumeInfoLoaded) {
        switch (oScheduleCalendar.columnTableVisible) {
            case 1:
                oScheduleCalendar.refreshColumnShiftTable();
                break;
            case 2:
                oScheduleCalendar.refreshColumnassignmentsTable();
                break;
            case 3:
                oScheduleCalendar.refreshColumnIndictmentsTable();
                break;
            case 4:
                oScheduleCalendar.refreshColumnCapacityTable();
                break;
        }
    }
}

Robotics.Client.Controls.roSchedulerCalendar.prototype.loadLeftTables = function () {
    var oScheduleCalendar = this; 
    if (oScheduleCalendar.resumeInfoLoaded) {
        switch (oScheduleCalendar.rowTableVisible) {
            case 1:
                oScheduleCalendar.refreshrowAccrualsTable();
                var tooltipHolidays = true;
                if ($("#ctl00_contentMainBody_oCalendar_hdnHolidayShiftPeriodicity").val() == 'L')
                    tooltipHolidays = false;
                oScheduleCalendar.refreshRelatedTooltips(tooltipHolidays);
                break;
            case 2:
                oScheduleCalendar.refreshrowShiftsTable();
                break;
            case 3:
                oScheduleCalendar.refreshrowAssignmentsTable();
                break;
        }
    }
}

Robotics.Client.Controls.roSchedulerCalendar.prototype.loadResumeTables = function () {
    var oScheduleCalendar = this;
    if (oScheduleCalendar.resumeInfoLoaded) {
        switch (oScheduleCalendar.rowTableVisible) {
            case 1:
                oScheduleCalendar.refreshrowAccrualsTotalTable();
                break;
        }
    }
}

Robotics.Client.Controls.roSchedulerCalendar.prototype.loadRelatedTables = function () {
    var oScheduleCalendar = this;
    var oCalendar = this.oBaseControl;
    this.oBaseControl.mapEvents()

    if (this.resumeInfoLoaded) {
        if (oScheduleCalendar.oBaseControl.typeView == Robotics.Client.Constants.TypeView.Planification) {
            if (!oScheduleCalendar.oBaseControl.pageLayout.center.children.tabsContainerLayout.south.state.isClosed) {
                oScheduleCalendar.loadSouthTables();
            }

            if (!oScheduleCalendar.oBaseControl.pageLayout.east.state.isClosed) {
                oScheduleCalendar.loadLeftTables();
            }

            if (!oScheduleCalendar.oBaseControl.pageLayout.center.children.tabsContainerLayout.south.state.isClosed && !oScheduleCalendar.oBaseControl.pageLayout.east.state.isClosed) {
                oScheduleCalendar.loadResumeTables();
            }

            oScheduleCalendar.oBaseControl.pageLayout.show("east");
            oScheduleCalendar.oBaseControl.pageLayout.center.children.tabsContainerLayout.show("south");
        } else {
            oScheduleCalendar.oBaseControl.pageLayout.hide("east");
            oScheduleCalendar.oBaseControl.pageLayout.center.children.tabsContainerLayout.hide("south");
        }

        $('#' + oCalendar.prefix + Robotics.Client.Constants.TableNames.ColShifts).parent().scrollLeft($('#' + oCalendar.prefix + Robotics.Client.Constants.TableNames.Calendar).parent().scrollLeft());
        $('#' + oCalendar.prefix + Robotics.Client.Constants.TableNames.RowAccruals).parent().scrollTop($('#' + oCalendar.prefix + Robotics.Client.Constants.TableNames.Calendar).parent().scrollTop());
        $('#' + oCalendar.prefix + Robotics.Client.Constants.TableNames.RowShifts).parent().scrollTop($('#' + oCalendar.prefix + Robotics.Client.Constants.TableNames.Calendar).parent().scrollTop());
        $('#' + oCalendar.prefix + Robotics.Client.Constants.TableNames.RowAssignments).parent().scrollTop($('#' + oCalendar.prefix + Robotics.Client.Constants.TableNames.Calendar).parent().scrollTop());
        this.prepareFixedHeight();

        $('#tabColumnWaiting').hide();
        $('#tabRowWaiting').hide();
        $('#tabResumeWaiting').hide();
    }

    this.bRelatedTables = false;
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.refreshTooltips = function () {
    var oCalendar = this.oBaseControl;
    var oScheduleCalendar = this;

    if (oCalendar.loadIndictments == true) {
        $('.indictments_target').off('mouseover');
        $('.indictments_target').on('mouseover', function () {
            var cell = $(this).find('.tooltipIndictmentsContainer');
            if (cell.length == 0) return;
            var cellId = this.id;
            $(cell).dxTooltip({
                target: "#" + cellId,
                hideEvent: "mouseleave",
                position: 'bottom'
            });
            $(cell).dxTooltip("instance").show();
        });
    }

    if (oCalendar.typeView == Robotics.Client.Constants.TypeView.Review && oCalendar.loadPunches) {
        $('.punchTooltipTarget').off('mouseover');
        $('.punchTooltipTarget').on('mouseover', function () {
            var cell = $(this).find('.tooltipPunchesReviewContainer');
            if (cell.length == 0) return;
            var cellId = this.id;
            $(cell).dxTooltip({
                target: "#" + cellId,
                hideEvent: "mouseleave",
                position: 'bottom'
            });
            $(cell).dxTooltip("instance").show();
        });
    }
}

Robotics.Client.Controls.roSchedulerCalendar.prototype.refreshRelatedTooltips = function (tooltipHolidays = true) {
    var oCalendar = this.oBaseControl;
    var oScheduleCalendar = this;

    if (oScheduleCalendar.rowTableVisible) {

        if (tooltipHolidays) {
            $('.resumeHolidayCell').off('mouseover');
            $('.resumeHolidayCell').on('mouseover', function () {
                var cell = $(this).find('.tooltipHolidaysContainer');
                if (cell.length == 0) return;
                var cellId = this.id;
                var containerId = cellId.replace('_tooltipHolidays_', '_tooltipTarget_');
                $(cell).dxTooltip({
                    target: "#" + containerId,
                    hideEvent: "mouseleave",
                    position: 'bottom'
                });
                $(cell).dxTooltip("instance").show();
            });
        }

        $('.resumeAccrualCell').off('mouseover');
        $('.resumeAccrualCell').on('mouseover', function () {
            var cell = $(this).find('.tooltipAccrualsContainer');
            if (cell.length == 0) return;
            var cellId = this.id;
            var containerId = cellId.replace('_tooltipAccruals_', '_tooltipTarget_');
            $(cell).dxTooltip({
                target: "#" + containerId,
                hideEvent: "mouseleave",
                position: 'bottom'
            });
            $(cell).dxTooltip("instance").show();
        });
    }
}

Robotics.Client.Controls.roSchedulerCalendar.prototype.clearRelatedTables = function (isResizing) {
    var oCalendar = this.oBaseControl;

    try {
        var tableContainer = $('#' + oCalendar.prefix + Robotics.Client.Constants.LayoutNames.ColumnTabShifts);
        tableContainer.empty();
    } catch (e) { }

    try {
        var tableContainer = $('#' + oCalendar.prefix + Robotics.Client.Constants.LayoutNames.ColumnTabAssignments);
        tableContainer.empty();
    } catch (e) { }

    try {
        var tableContainer = $('#' + oCalendar.prefix + Robotics.Client.Constants.LayoutNames.ColumnTabIndictments);
        tableContainer.empty();
    } catch (e) { }

    try {
        var tableContainer = $('#' + oCalendar.prefix + Robotics.Client.Constants.LayoutNames.ColumnTabCapacity);
        tableContainer.empty();
    } catch (e) { }

    try {
        var tableContainer = $('#' + oCalendar.prefix + Robotics.Client.Constants.LayoutNames.RowTabAccruals);
        tableContainer.empty();
    } catch (e) { }

    try {
        var tableContainer = $('#' + oCalendar.prefix + Robotics.Client.Constants.LayoutNames.RowTabShifts);
        tableContainer.empty();
    } catch (e) { }

    try {
        var tableContainer = $('#' + oCalendar.prefix + Robotics.Client.Constants.LayoutNames.RowTabAssignments);
        tableContainer.empty();
    } catch (e) { }

    try {
        var tableContainer = $('#' + oCalendar.prefix + Robotics.Client.Constants.LayoutNames.Resume);
        tableContainer.empty();
        //tableContainer.html('<div style="display: block;margin: auto;width: 32px;"> <img alt="" src="../Base/Images/Loaders/loader_v3.gif" style="margin-top: 25px;"></img></div>');
    } catch (e) { }
}

Robotics.Client.Controls.roSchedulerCalendar.prototype.refreshCalendarTable = function (isResizing) {
    var oCalendar = this.oBaseControl;

    try {
        $('#' + oCalendar.prefix + Robotics.Client.Constants.TableNames.Calendar).fixedHeaderTable('destroy');

        $('#' + oCalendar.prefix + Robotics.Client.Constants.TableNames.Calendar).find('tfoot').each(function (index) {
            if (this.id == '') {
                $(this).remove();
            }
        });
    } catch (e) { }

    if (!isResizing) this.createResumeTables(Robotics.Client.Constants.TableNames.Calendar, Robotics.Client.Constants.LayoutNames.Calendar);

    this.fixedCalendarTable = $('#' + oCalendar.prefix + Robotics.Client.Constants.TableNames.Calendar).fixedHeaderTable({
        altClass: 'odd',
        footer: oCalendar.isScheduleActive,
        fixedColumn: true
    });
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.refreshColumnShiftTable = function () {
    var oCalendar = this.oBaseControl;

    try {
        $('#' + oCalendar.prefix + Robotics.Client.Constants.TableNames.ColShifts).fixedHeaderTable('destroy');

        $('#' + oCalendar.prefix + Robotics.Client.Constants.TableNames.ColShifts).find('tfoot').each(function (index) {
            if (this.id == '') {
                $(this).remove();
            }
        });
    } catch (e) { }

    this.createResumeTables(Robotics.Client.Constants.TableNames.ColShifts, Robotics.Client.Constants.LayoutNames.ColumnTabShifts);

    $('#' + oCalendar.prefix + Robotics.Client.Constants.TableNames.ColShifts).fixedHeaderTable({
        altClass: 'odd',
        footer: true,
        fixedColumn: true
    });

    $('#' + oCalendar.prefix + Robotics.Client.Constants.TableNames.Calendar).parent().on('scroll', function () {
        $('#' + oCalendar.prefix + Robotics.Client.Constants.TableNames.ColShifts).parent().scrollLeft($(this).scrollLeft());
    });
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.refreshColumnassignmentsTable = function () {
    var oCalendar = this.oBaseControl;

    try {
        $('#' + oCalendar.prefix + Robotics.Client.Constants.TableNames.ColAssignments).fixedHeaderTable('destroy');

        $('#' + oCalendar.prefix + Robotics.Client.Constants.TableNames.ColAssignments).find('tfoot').each(function (index) {
            if (this.id == '') {
                $(this).remove();
            }
        });
    } catch (e) { }

    if (oCalendar.isScheduleActive) this.createResumeTables(Robotics.Client.Constants.TableNames.ColAssignments, Robotics.Client.Constants.LayoutNames.ColumnTabAssignments);

    $('#' + oCalendar.prefix + Robotics.Client.Constants.TableNames.ColAssignments).fixedHeaderTable({
        altClass: 'odd',
        footer: true,
        fixedColumn: true
    });

    $('#' + oCalendar.prefix + Robotics.Client.Constants.TableNames.Calendar).parent().on('scroll', function () {
        $('#' + oCalendar.prefix + Robotics.Client.Constants.TableNames.ColAssignments).parent().scrollLeft($(this).scrollLeft());
    });
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.refreshColumnIndictmentsTable = function () {
    var oCalendar = this.oBaseControl;

    //try {
    //    $('#' + oCalendar.prefix + Robotics.Client.Constants.TableNames.RowIndictments).fixedHeaderTable('destroy');

    //    $('#' + oCalendar.prefix + Robotics.Client.Constants.TableNames.RowIndictments).find('tfoot').each(function (index) {
    //        if (this.id == '') {
    //            $(this).remove();
    //        }
    //    });
    //} catch (e) { }

    if (oCalendar.isScheduleActive || oCalendar.saasPremiumActive) this.createResumeTables(Robotics.Client.Constants.TableNames.ColIndictments, Robotics.Client.Constants.LayoutNames.ColumnTabIndictments);

    //$('#' + oCalendar.prefix + Robotics.Client.Constants.TableNames.RowIndictments).fixedHeaderTable({
    //    altClass: 'odd',
    //    footer: oCalendar.isScheduleActive,
    //    height: $('#' + oCalendar.prefix + Robotics.Client.Constants.LayoutNames.Calendar).find(".fht-fixed-body")[0].clientHeight - 19
    //});
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.refreshColumnCapacityTable = function () {
    var oCalendar = this.oBaseControl;
    if (oCalendar.telecommuteEnabled) {
        try {
            $('#' + oCalendar.prefix + Robotics.Client.Constants.TableNames.ColCapacity).fixedHeaderTable('destroy');

            $('#' + oCalendar.prefix + Robotics.Client.Constants.TableNames.ColCapacity).find('tfoot').each(function (index) {
                if (this.id == '') {
                    $(this).remove();
                }
            });
        } catch (e) { }

        this.createResumeTables(Robotics.Client.Constants.TableNames.ColCapacity, Robotics.Client.Constants.LayoutNames.ColumnTabCapacity);

        $('#' + oCalendar.prefix + Robotics.Client.Constants.TableNames.ColCapacity).fixedHeaderTable({
            altClass: 'odd',
            footer: true,
            fixedColumn: true
        });

        $('#' + oCalendar.prefix + Robotics.Client.Constants.TableNames.Calendar).parent().on('scroll', function () {
            $('#' + oCalendar.prefix + Robotics.Client.Constants.TableNames.ColCapacity).parent().scrollLeft($(this).scrollLeft());
        });
    }
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.refreshrowAccrualsTable = function () {
    var oCalendar = this.oBaseControl;

    try {
        $('#' + oCalendar.prefix + Robotics.Client.Constants.TableNames.RowAccruals).fixedHeaderTable('destroy');

        $('#' + oCalendar.prefix + Robotics.Client.Constants.TableNames.RowAccruals).find('tfoot').each(function (index) {
            if (this.id == '') {
                $(this).remove();
            }
        });
    } catch (e) { }

    this.createResumeTables(Robotics.Client.Constants.TableNames.RowAccruals, Robotics.Client.Constants.LayoutNames.RowTabAccruals);

    $('#' + oCalendar.prefix + Robotics.Client.Constants.TableNames.RowAccruals).fixedHeaderTable({
        altClass: 'odd',
        footer: oCalendar.isScheduleActive,
        height: $('#' + oCalendar.prefix + Robotics.Client.Constants.LayoutNames.Calendar).find(".fht-fixed-body")[0].clientHeight - 15
    });

    $('#' + oCalendar.prefix + Robotics.Client.Constants.TableNames.Calendar).parent().on('scroll', function () {
        $('#' + oCalendar.prefix + Robotics.Client.Constants.TableNames.RowAccruals).parent().scrollTop($(this).scrollTop());
    });
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.refreshrowShiftsTable = function () {
    var oCalendar = this.oBaseControl;

    try {
        $('#' + oCalendar.prefix + Robotics.Client.Constants.TableNames.RowShifts).fixedHeaderTable('destroy');

        $('#' + oCalendar.prefix + Robotics.Client.Constants.TableNames.RowShifts).find('tfoot').each(function (index) {
            if (this.id == '') {
                $(this).remove();
            }
        });
    } catch (e) { }

    this.createResumeTables(Robotics.Client.Constants.TableNames.RowShifts, Robotics.Client.Constants.LayoutNames.RowTabShifts);

    $('#' + oCalendar.prefix + Robotics.Client.Constants.TableNames.RowShifts).fixedHeaderTable({
        altClass: 'odd',
        footer: oCalendar.isScheduleActive,
        height: $('#' + oCalendar.prefix + Robotics.Client.Constants.LayoutNames.Calendar).find(".fht-fixed-body")[0].clientHeight - 19
    });

    $('#' + oCalendar.prefix + Robotics.Client.Constants.TableNames.Calendar).parent().on('scroll', function () {
        $('#' + oCalendar.prefix + Robotics.Client.Constants.TableNames.RowShifts).parent().scrollTop($(this).scrollTop());
    });
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.refreshrowAssignmentsTable = function () {
    var oCalendar = this.oBaseControl;

    try {
        $('#' + oCalendar.prefix + Robotics.Client.Constants.TableNames.RowAssignments).fixedHeaderTable('destroy');

        $('#' + oCalendar.prefix + Robotics.Client.Constants.TableNames.RowAssignments).find('tfoot').each(function (index) {
            if (this.id == '') {
                $(this).remove();
            }
        });
    } catch (e) { }

    if (oCalendar.isScheduleActive) this.createResumeTables(Robotics.Client.Constants.TableNames.RowAssignments, Robotics.Client.Constants.LayoutNames.RowTabAssignments);

    $('#' + oCalendar.prefix + Robotics.Client.Constants.TableNames.RowAssignments).fixedHeaderTable({
        altClass: 'odd',
        footer: oCalendar.isScheduleActive,
        height: $('#' + oCalendar.prefix + Robotics.Client.Constants.LayoutNames.Calendar).find(".fht-fixed-body")[0].clientHeight - 19
    });

    $('#' + oCalendar.prefix + Robotics.Client.Constants.TableNames.Calendar).parent().on('scroll', function () {
        $('#' + oCalendar.prefix + Robotics.Client.Constants.TableNames.RowAssignments).parent().scrollTop($(this).scrollTop());
    });
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.refreshrowAccrualsTotalTable = function () {
    var oCalendar = this.oBaseControl;

    try {
        $('#' + oCalendar.prefix + Robotics.Client.Constants.TableNames.AccrualTotals).fixedHeaderTable('destroy');

        $('#' + oCalendar.prefix + Robotics.Client.Constants.TableNames.AccrualTotals).find('tfoot').each(function (index) {
            if (this.id == '') {
                $(this).remove();
            }
        });
    } catch (e) { }

    this.createResumeTables(Robotics.Client.Constants.TableNames.AccrualTotals, Robotics.Client.Constants.LayoutNames.Resume);

    $('#' + oCalendar.prefix + Robotics.Client.Constants.TableNames.AccrualTotals).fixedHeaderTable({
        altClass: 'odd',
        footer: true,
        fixedColumn: false
    });
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.clickColumnShiftsTableHeader = function (oScheduleCalendar) {
    return function () {
        if (oScheduleCalendar.columnTableVisible != 1) {
            oScheduleCalendar.columnTableVisible = 1;
            oScheduleCalendar.refreshColumnShiftTable();
            oScheduleCalendar.prepareFixedHeight();
        }
    };
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.clickColumnAssignmentsTableHeader = function (oScheduleCalendar) {
    return function () {
        if (oScheduleCalendar.columnTableVisible != 2) {
            oScheduleCalendar.columnTableVisible = 2;
            oScheduleCalendar.refreshColumnassignmentsTable();
            oScheduleCalendar.prepareFixedHeight();
        }
    };
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.clickColumnIndictmentsTableHeader = function (oScheduleCalendar) {
    return function () {
        if (oScheduleCalendar.columnTableVisible != 3) {
            oScheduleCalendar.columnTableVisible = 3;
            oScheduleCalendar.refreshColumnIndictmentsTable();
            oScheduleCalendar.prepareFixedHeight();
        }
    };
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.clickColumnCapacityTableHeader = function (oScheduleCalendar) {
    return function () {
        if (oScheduleCalendar.columnTableVisible != 4) {
            oScheduleCalendar.columnTableVisible = 4;
            oScheduleCalendar.refreshColumnCapacityTable();
            oScheduleCalendar.prepareFixedHeight();
        }
    };
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.clickRowAccrualsTableHeader = function (oScheduleCalendar) {
    var oCalendar = this;
    return function () {
        if (oScheduleCalendar.rowTableVisible != 1) {
            oScheduleCalendar.rowTableVisible = 1;
            oScheduleCalendar.refreshrowAccrualsTable();
            oScheduleCalendar.refreshrowAccrualsTotalTable();
            oCalendar.refreshTooltips();
            oCalendar.refreshRelatedTooltips();
            oScheduleCalendar.prepareFixedHeight();
        }
    };
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.clickRowShiftsTableHeader = function (oScheduleCalendar) {
    return function () {
        if (oScheduleCalendar.rowTableVisible != 2) {
            oScheduleCalendar.rowTableVisible = 2;
            oScheduleCalendar.refreshrowShiftsTable();
            oScheduleCalendar.prepareFixedHeight();
        }
    };
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.clickRowAssignmentsTableHeader = function (oScheduleCalendar) {
    return function () {
        if (oScheduleCalendar.rowTableVisible != 3) {
            oScheduleCalendar.rowTableVisible = 3;
            oScheduleCalendar.refreshrowAssignmentsTable();
            oScheduleCalendar.prepareFixedHeight();
        }
    };
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.createResumeTables = function (idTable, parentId) {
    switch (idTable) {
        case Robotics.Client.Constants.TableNames.Calendar:
            this.createCalendarTable(idTable, parentId);
            break;
        case Robotics.Client.Constants.TableNames.ColShifts:
            this.createColumnShiftsTable(idTable, parentId);
            break;
        case Robotics.Client.Constants.TableNames.ColAssignments:
            this.createColumnAssignmentsTable(idTable, parentId);
            break;
        case Robotics.Client.Constants.TableNames.ColCapacity:
            this.createCapacityTable(idTable, parentId);
            break;
        case Robotics.Client.Constants.TableNames.ColIndictments:
            this.createIndictmentscolumnTable(idTable, parentId);
            break;
        case Robotics.Client.Constants.TableNames.RowAccruals:
            this.createAccrualsTable(idTable, parentId);
            break;
        case Robotics.Client.Constants.TableNames.RowShifts:
            this.createShiftsRowTable(idTable, parentId);
            break;
        case Robotics.Client.Constants.TableNames.RowAssignments:
            this.createAssignmentsRowTable(idTable, parentId);
            break;
        case Robotics.Client.Constants.TableNames.AccrualTotals:
            this.createAcrrualTotalsTable(idTable, parentId);
            break;
    }
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.createCalendarTable = function (idTable, parentId) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    if (typeof oCalData.CalendarShift != 'undefined' && oCalData.CalendarShift != null && oCalData.CalendarShift.length > 0) {
        oCalendar.shiftsExtendedDataCache = {};
        for (var iIndexCache = 0; iIndexCache < oCalData.CalendarShift.length; iIndexCache++) {
            if (typeof oCalendar.shiftsExtendedDataCache[oCalData.CalendarShift[iIndexCache].IDShift] == 'undefined') {
                oCalendar.shiftsExtendedDataCache[oCalData.CalendarShift[iIndexCache].IDShift] = oCalData.CalendarShift[iIndexCache];
            }
        }
    }

    var tableContainer = $('#' + oCalendar.prefix + parentId);
    tableContainer.empty();

    var tableElement = $('<table></table>').attr('id', oCalendar.prefix + idTable).attr('class', 'fancyTable');

    tableElement.append(this.createCalendarTableHeader(idTable, parentId));

    tableElement.append(this.createCalendarTableBody(idTable, parentId));

    //Creamos el footer solo si hay licencia de scheduler
    if (oCalendar.isScheduleActive) {
        tableElement.append(this.createCalendarTableFooter(idTable, parentId));
    }

    tableContainer.append(tableElement);
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.createCalendarTableHeader = function (idTable, parentId) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    //Creamos el header
    var tHeader = $('<thead></thead>').attr('id', oCalendar.prefix + idTable + '_thead').attr('class', 'HeaderSelectable');

    var tHeaderRow = $('<tr></tr>');

    //Creamos la primera columna que sera el header
    var tFixedHeaderCell = $('<th></th>');
    var mainFixedHeaderDiv = $('<div></div>').attr('class', 'CalendarFixed CalendarFixedHeader');

    var employeeHeaderCell = $('<div></div>').attr('class', 'CalendarEmployeeFixed CalendarEmployeeFixedHeader');
    var northCell = $('<div></div>').attr('class', 'NorthCell NorthCellHeaderEmployee');

    northCell.append($('<div></div>').attr('class', 'EmpCellTextNorth').html(oCalData.CalendarHeader.EmployeeHeaderData.Row1Text));

    var expandIcon = $('<div></div>').attr('class', 'RecursiveAction ' + (oCalendar.loadRecursive ? 'minimizeGroup' : 'expandGroup')).attr("style", "float:left").attr('title', oCalendar.loadRecursive ? oCalendar.translator.translate(Robotics.Client.Language.Tags.Scheduler_Recursive) : oCalendar.translator.translate(Robotics.Client.Language.Tags.Scheduler_NonRecursive));
    var indictmentsIcon = $('<div></div>').attr('class', 'LoadIndictments ' + (oCalendar.loadIndictments == false ? 'showIndictments-inactive' : 'showIndictments-active')).attr("style", "float:left;padding-left:7px;").attr('title', Globalize.formatMessage("roShowIndictments"));
    var capacitiesIcon = $('<div></div>').attr('class', 'LoadCapacities ' + (oCalendar.loadCapacities == false ? 'showCapacities-inactive' : 'showCapacities-active')).attr("style", "float:left;padding-left:7px;").attr('title', Globalize.formatMessage("roShowCapacities"));
    var dailyPeriod = $('<div></div>').attr('class', 'DailyPeriod interval-' + oCalendar.dailyPeriod).attr("style", "float:left").attr('title', oCalendar.dailyPeriodDescription());

    var northIcons = $('<div></div>').attr('class', 'EmpCellIconsNorth');
    if (oCalendar.typeView == Robotics.Client.Constants.TypeView.Review) {
        var punchesIcon = $('<div></div>').attr('class', 'LoadPunches ' + (oCalendar.loadPunches == false ? 'showPunches-inactive' : 'showPunches-active')).attr("style", "float:left;padding-left:7px;").attr('title', Globalize.formatMessage("roShowPunches"));
        northIcons.append(punchesIcon);
    }

    if (oCalendar.viewRange == Robotics.Client.Constants.ViewRange.Daily) {
        northIcons.append(dailyPeriod);
    }

    if (oCalendar.isScheduleActive || oCalendar.saasPremiumActive) {
        northIcons.append(indictmentsIcon, expandIcon, capacitiesIcon);
    } else {
        northIcons.append(expandIcon);
    }
    northCell.append(northIcons);

    var southCell = $('<div></div>').attr('class', 'SouthCell');
    southCell.append($('<div></div>').attr('class', 'EmpCellTextSouth').html(oCalendar.isScheduleActive == false ? '' : (oCalData.CalendarHeader.EmployeeHeaderData.Row2Text + ' - ' + (oCalendar.schedulingView == Robotics.Client.Constants.CoverageDayView.Planified ? oCalendar.translator.translate(Robotics.Client.Language.Tags.Scheduler_Planified) : oCalendar.translator.translate(Robotics.Client.Language.Tags.Scheduler_Real)))));

    var filterIcon = $('<div></div>').attr('class', 'FilterAction ' + (oCalendar.assignmentsFilter == '' ? 'filterGroup' : 'filter-activeGroup')).attr('title', oCalendar.translator.translate(Robotics.Client.Language.Tags.Scheduler_Filter));
    southCell.append($('<div></div>').attr('class', 'EmpCellIconsSouth').append(oCalendar.isScheduleActive ? filterIcon : '&nbsp;'));

    employeeHeaderCell.append(northCell, southCell);

    var summaryHeaderCell = $('<div></div>').attr('class', 'CalendarSummaryFixed CalendarSummaryFixedHeader');

    var northSummaryCell = $('<div></div>').attr('class', 'NorthCell');
    northSummaryCell.append($('<div></div>').attr('class', 'EmpCellTextNorth').html(oCalData.CalendarHeader.SummaryHeaderData.Row1Text));

    var southSummaryCell = $('<div></div>').attr('class', 'SouthCell');
    southSummaryCell.append($('<div></div>').attr('class', 'EmpCellTextSouth').html(oCalData.CalendarHeader.SummaryHeaderData.Row2Text));

    summaryHeaderCell.append(northSummaryCell, southSummaryCell);

    tHeaderRow.append(tFixedHeaderCell.append(mainFixedHeaderDiv.append(employeeHeaderCell, summaryHeaderCell)));

    if (oCalendar.viewRange == Robotics.Client.Constants.ViewRange.Daily) {
        for (var i = oCalendar.getMinDailyCell(); i < oCalendar.getMaxDailyCell(); i++) {
            var dayHeaderCell = $('<th></th>');

            var mainDayHeaderCell = $('<div></div>').attr('id', oCalendar.ascxPrefix + '_calHeaderCell_' + i).attr('class', 'CalendarDayFixed CalendarDayFixedHeader DailyCell').attr('data-IDColumn', i).attr('style', 'background:' + oCalData.CalendarHeader.PeriodHeaderData[i].BackColor);

            mainDayHeaderCell.append($('<div></div>').attr('class', oCalendar.isScheduleActive == false ? 'NorthCellExpanded' : 'NorthCell' + ' dayInfo DailyCell').html(oCalData.CalendarHeader.PeriodHeaderData[i].Row2Text));

            if (oCalendar.isScheduleActive) {
                if (oCalendar.sortColumn == i) mainDayHeaderCell.append($('<div></div>').attr('class', 'columnSorted'));
                mainDayHeaderCell.append(this.buildCellScheduleStatus(0, true).attr('id', oCalendar.ascxPrefix + '_IDHeaderCell_' + i));
            }

            tHeaderRow.append(dayHeaderCell.append(mainDayHeaderCell));
        }
    } else {
        for (var i = 0; i < oCalData.CalendarHeader.PeriodHeaderData.length; i++) {
            var dayHeaderCell = $('<th></th>');

            var mainDayHeaderCell = $('<div></div>').attr('id', oCalendar.ascxPrefix + '_calHeaderCell_' + i).attr('class', 'CalendarDayFixed CalendarDayFixedHeader').attr('data-IDColumn', i).attr('style', 'background:' + oCalData.CalendarHeader.PeriodHeaderData[i].BackColor);

            mainDayHeaderCell.append($('<div></div>').attr('class', oCalendar.isScheduleActive == false ? 'NorthCellExpanded' : 'NorthCell' + ' dayInfo').html(oCalData.CalendarHeader.PeriodHeaderData[i].Row1Text + "</br>" + oCalData.CalendarHeader.PeriodHeaderData[i].Row2Text));

            if (oCalendar.isScheduleActive) {
                if (oCalendar.sortColumn == i) mainDayHeaderCell.append($('<div></div>').attr('class', 'columnSorted'));
                mainDayHeaderCell.append(this.buildCellScheduleStatus(i, true).attr('id', oCalendar.ascxPrefix + '_IDHeaderCell_' + i));
            }

            tHeaderRow.append(dayHeaderCell.append(mainDayHeaderCell));
        }
    }

    var tmpCell = $('<th style="width:100%"></th>').html('&nbsp;');
    tHeaderRow.append(tmpCell);
    tHeader.append(tHeaderRow);

    return tHeader;
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.createCalendarTableBody = function (idTable, parentId) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    //Creamos el body
    var tBody = $('<tbody></tbody>').attr('id', oCalendar.prefix + idTable + '_tbody').attr('class', 'bodySelectable');

    var lastGroup = -1, actualGroup = -1;

    if (oCalData.CalendarData != null && oCalData.CalendarData.length > 0) {
        for (var i = 0; i < oCalData.CalendarData.length; i++) {
            actualGroup = oCalData.CalendarData[i].EmployeeData.IDGroup;

            if (oCalendar.sortColumn == -1 && actualGroup != lastGroup) {
                var tBodyRow = $('<tr></tr>');
                var tFixedBodyCell = $('<td></td>').attr('class', 'calendarOuterBodyFixedSeparator');
                var mainFixedBodyCell = $('<div></div>').attr('class', 'CalendarFixed CalendarFixedBody');
                var fixedEmployeeBodyCell = $('<div></div>').attr('class', 'CalendarEmployeeFixed CalendarEmployeeSepartaorBody').append($('<span></span>').html(oCalData.CalendarData[i].EmployeeData.GroupName));
                var fixedSummaryBodyCell = $('<div></div>').attr('class', 'CalendarSummaryFixed CalendarEmployeeSepartaorBody');

                tBodyRow.append(tFixedBodyCell.append(mainFixedBodyCell.append(fixedEmployeeBodyCell, fixedSummaryBodyCell)));

                for (var columnIndex = 0; columnIndex < oCalData.CalendarHeader.PeriodHeaderData.length; columnIndex++) {
                    var calendarCell = $('<td></td>').attr('class', 'calendarOuterBodyFixedSeparator');
                    var calendarOuterContent = $('<div></div>');

                    if (oCalendar.viewRange == Robotics.Client.Constants.ViewRange.Period) calendarOuterContent.attr('class', 'calendarOuterBodyCellSeparator');

                    tBodyRow.append(calendarCell.append(calendarOuterContent));
                }

                tmpCell = $('<td style="width:100%"></td>').html('&nbsp;');
                tBodyRow.append(tmpCell);

                tBody.append(tBodyRow);
            }

            var tBodyRow = $('<tr></tr>');

            var tFixedBodyCell = $('<td></td>');

            var mainFixedBodyCell = $('<div></div>').attr('class', 'CalendarFixed CalendarFixedBody');

            var fixedEmployeeBodyCell = $('<div></div>').attr('class', 'CalendarEmployeeFixed CalendarEmployeeFixedBody');

            fixedEmployeeBodyCell.append($('<div></div>').attr('class', 'EmployeeName').attr('title', oCalData.CalendarData[i].EmployeeData.GroupName).html(oCalData.CalendarData[i].EmployeeData.EmployeeName));
            fixedEmployeeBodyCell.append(this.generateEmployeeAssignments(oCalData.CalendarData[i].EmployeeData, tBodyRow));

            var fixedSummaryBodyCell = $('<div></div>').attr('class', 'CalendarSummaryFixed CalendarSummaryFixedBody');

            fixedSummaryBodyCell.append(this.generateEmployeeAccrual(oCalData.CalendarData[i].SummaryData));
            fixedSummaryBodyCell.append(this.generateEmployeeAlerts(oCalData.CalendarData[i].SummaryData));

            tBodyRow.append(tFixedBodyCell.append(mainFixedBodyCell.append(fixedEmployeeBodyCell, fixedSummaryBodyCell)));

            if (oCalendar.typeView == Robotics.Client.Constants.TypeView.Planification) this.buildPlanificationRowView(i, tBodyRow);
            else if (oCalendar.typeView == Robotics.Client.Constants.TypeView.Review) this.buildReviewRowView(i, tBodyRow);

            tmpCell = $('<td style="width:100%"></td>').html('&nbsp;');
            tBodyRow.append(tmpCell);

            tBody.append(tBodyRow);

            lastGroup = actualGroup;
        }
    } else {
        var tBodyRow = $('<tr></tr>');
        var tFixedBodyCell = $('<td></td>');
        var mainFixedBodyCell = $('<div></div>').attr('class', 'CalendarFixed CalendarFixedBody');
        var fixedEmployeeBodyCell = $('<div></div>').attr('class', 'CalendarEmployeeFixed CalendarEmployeeFixedBody');
        var fixedSummaryBodyCell = $('<div></div>').attr('class', 'CalendarSummaryFixed CalendarSummaryFixedBody');

        tBodyRow.append(tFixedBodyCell.append(mainFixedBodyCell.append(fixedEmployeeBodyCell, fixedSummaryBodyCell)));

        for (var columnIndex = 0; columnIndex < oCalData.CalendarHeader.PeriodHeaderData.length; columnIndex++) {
            var calendarCell = $('<td></td>');
            var calendarOuterContent = $('<div></div>').attr('class', 'calendarOuterBodyCellInOtherDepartment');
            tBodyRow.append(calendarCell.append(calendarOuterContent));
        }

        tmpCell = $('<td style="width:100%"></td>').html('&nbsp;');
        tBodyRow.append(tmpCell);

        tBody.append(tBodyRow);
    }
    return tBody;
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.createCalendarTableFooter = function (idTable, parentId) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    var tFooter = $('<tfoot></tfoot>').attr('id', oCalendar.prefix + idTable + '_tfoot');

    var tFooterRow = $('<tr></tr>');
    var tFixedFooterCell = $('<td></td>');
    var mainFixedFooterDiv = $('<div></div>').attr('class', 'CalendarFixed CalendarFixedFooter').html('&nbsp');

    tFooterRow.append(tFixedFooterCell.append(mainFixedFooterDiv));

    if (oCalendar.viewRange == Robotics.Client.Constants.ViewRange.Daily) {
        for (var i = oCalendar.getMinDailyCell(); i < oCalendar.getMaxDailyCell(); i++) {
            //for (var i = 0; i < oCalData.CalendarHeader.PeriodHeaderData.length; i++) {
            var dayFooterCell = $('<td></td>');
            var mainDayFooterCell = $('<div></div>').attr('class', 'CalendarDayFixed CalendarDayFixedFooter DailyCell');

            //mainDayFooterCell.append($('<div></div>').attr('class', 'NorthCell DailyCell').attr('style', 'background:' + oCalData.CalendarHeader.PeriodHeaderData[i].BackColor).html("&nbsp;"));

            mainDayFooterCell.append(this.buildCellScheduleStatus(0, false).attr('id', oCalendar.ascxPrefix + '_IDFooterCell_' + i));

            tFooterRow.append(dayFooterCell.append(mainDayFooterCell));
        }
    } else {
        for (var i = 0; i < oCalData.CalendarHeader.PeriodHeaderData.length; i++) {
            var dayFooterCell = $('<td></td>');
            var mainDayFooterCell = $('<div></div>').attr('class', 'CalendarDayFixed CalendarDayFixedFooter');

            //mainDayFooterCell.append($('<div></div>').attr('class', 'NorthCell').attr('style', 'background:' + oCalData.CalendarHeader.PeriodHeaderData[i].BackColor).html("&nbsp;"));
            mainDayFooterCell.append(this.buildCellScheduleStatus(i, false).attr('id', oCalendar.ascxPrefix + '_IDFooterCell_' + i));

            tFooterRow.append(dayFooterCell.append(mainDayFooterCell));
        }
    }

    tmpCell = $('<td style="width:100%"></td>').html('&nbsp;');
    tFooterRow.append(tmpCell);

    tFooter.append(tFooterRow);

    return tFooter;
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.buildCellScheduleStatus = function (currentIndex, addDescriptionIcon) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    var cellStatus = $('<div></div>').attr('class', 'SouthCell dayStatus' + (oCalendar.viewRange == Robotics.Client.Constants.ViewRange.Daily ? ' DailyCell' : ''));

    if (oCalendar.typeView == Robotics.Client.Constants.TypeView.Planification) {
        if (typeof oCalData.CalendarHeader.PeriodCoverageData != 'undefined' && oCalData.CalendarHeader.PeriodCoverageData.length > 0 && oCalData.CalendarHeader.PeriodCoverageData[currentIndex].PlannedStatus != Robotics.Client.Constants.CoverageDayStatus.WITHOUTCOVERAGE) {
            var assignmentDescription = '';

            if (typeof oCalData.CalendarHeader.PeriodCoverageData[currentIndex].AssignmentData != 'undefined' && oCalData.CalendarHeader.PeriodCoverageData[currentIndex].AssignmentData.length > 0) {
                for (var z = 0; z < oCalData.CalendarHeader.PeriodCoverageData[currentIndex].AssignmentData.length; z++) {
                    if (oCalendar.schedulingView == Robotics.Client.Constants.CoverageDayView.Planified) {
                        assignmentDescription += '(' + oCalData.CalendarHeader.PeriodCoverageData[currentIndex].AssignmentData[z].Planned.toString().lpad("0", 2) + '/' + oCalData.CalendarHeader.PeriodCoverageData[currentIndex].AssignmentData[z].Expected.toString().lpad("0", 2) + ')  -' + oCalData.CalendarHeader.PeriodCoverageData[currentIndex].AssignmentData[z].Name + ' \n ';
                    } else {
                        assignmentDescription += '(' + oCalData.CalendarHeader.PeriodCoverageData[currentIndex].AssignmentData[z].Actual.toString().lpad("0", 2) + '/' + oCalData.CalendarHeader.PeriodCoverageData[currentIndex].AssignmentData[z].Expected.toString().lpad("0", 2) + ')  -' + oCalData.CalendarHeader.PeriodCoverageData[currentIndex].AssignmentData[z].Name + ' \n ';
                    }
                }
            }

            if (addDescriptionIcon) cellStatus.append($('<div></div>').attr('class', 'assignmentExists').attr('title', assignmentDescription));

            if (oCalendar.schedulingView == Robotics.Client.Constants.CoverageDayView.Planified) {
                switch (oCalData.CalendarHeader.PeriodCoverageData[currentIndex].PlannedStatus) {
                    case Robotics.Client.Constants.CoverageDayStatus.OK:
                        cellStatus.attr('style', 'background: #38e219;');
                        break;
                    case Robotics.Client.Constants.CoverageDayStatus.KO:
                        cellStatus.attr('style', 'background: #ff2323;');
                        break;
                    case Robotics.Client.Constants.CoverageDayStatus.OVERLOAD:
                        cellStatus.attr('style', 'background: #ffbb23;');
                        break;
                }
            } else {
                switch (oCalData.CalendarHeader.PeriodCoverageData[currentIndex].ActualStatus) {
                    case Robotics.Client.Constants.CoverageDayStatus.OK:
                        cellStatus.attr('style', 'background: #38e219;');
                        break;
                    case Robotics.Client.Constants.CoverageDayStatus.KO:
                        cellStatus.attr('style', 'background: #ff2323;');
                        break;
                    case Robotics.Client.Constants.CoverageDayStatus.OVERLOAD:
                        cellStatus.attr('style', 'background: #ffbb23;');
                        break;
                }
            }
        }
    }

    return cellStatus;
}

Robotics.Client.Controls.roSchedulerCalendar.prototype.generateEmployeeAccrual = function (SummaryData) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    var container = $('<div></div>').attr('class', 'EmployeeAccrual').html(oCalendar.ConvertHoursToHourFormat(SummaryData.Accrual * 60));

    if (SummaryData.Accrual < 0) {
        container.attr('style', 'color:red');
    }

    return container;
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.generateEmployeeAlerts = function (SummaryData) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    var container = $('<div></div>').attr('class', 'EmployeeAlertsContainer');

    if (moment().startOf('day').isBetween(moment(oCalData.FirstDay), moment(oCalData.LastDay)) || moment().startOf('day').isSame(moment(oCalData.FirstDay))) {
        if (SummaryData.Alerts != null) {
            if (SummaryData.Alerts.OnAbsenceDays) container.append($('<div></div>').attr('class', 'calendarPlannedAbsence contentHorizontally').attr('title', oCalendar.translator.translate(Robotics.Client.Language.Tags.Tooltip_absence)));
            if (SummaryData.Alerts.OnAbsenceHours) container.append($('<div></div>').attr('class', 'calendarPlannedIncidence contentHorizontally').attr('title', oCalendar.translator.translate(Robotics.Client.Language.Tags.Tooltip_incidence)));
            if (SummaryData.Alerts.OnOvertimesHours) container.append($('<div></div>').attr('class', 'calendarPlannedOvertme contentHorizontally').attr('title', oCalendar.translator.translate(Robotics.Client.Language.Tags.Tooltip_overtime)));
            if (SummaryData.Alerts.OnHolidaysHours) container.append($('<div></div>').attr('class', 'calendarPlannedHoliday contentHorizontally').attr('title', oCalendar.translator.translate(Robotics.Client.Language.Tags.Tooltip_plannedHoliday)));
            if (SummaryData.Alerts.OnHolidays) container.append($('<div></div>').attr('class', 'calendarHolidays contentHorizontally').attr('title', oCalendar.translator.translate(Robotics.Client.Language.Tags.Tooltip_holidays)));
            if (SummaryData.Alerts.UnexpectedlyAbsent) container.append($('<div></div>').attr('class', 'calendarUnexpectedlyAbsent contentHorizontally').attr('title', oCalendar.translator.translate(Robotics.Client.Language.Tags.Tooltip_absent)));
        }
    }

    return container;
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.generateEmployeeAssignments = function (EmployeeData, tRow) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    var container = $('<div></div>').attr('class', 'EmployeeAssignmentContainer');

    if (oCalendar.isScheduleActive && typeof EmployeeData.Assignments != 'undefined' && EmployeeData.Assignments != null && EmployeeData.Assignments.length > 0) {
        var assignments = $('<div></div>').attr('class', 'assignmentsContainerLeft');
        var sFilterData = '';

        for (var i = 0; i < EmployeeData.Assignments.length; i++) {
            var curAssign = EmployeeData.Assignments[i];

            var cell = $('<div></div>').attr('class', 'AssignmentCell').attr('style', 'background:' + curAssign.Color + ';color:' + new Robotics.Client.Common.roHtmlColor().invertCssColor(curAssign.Color));
            cell.attr('title', curAssign.Name).html(curAssign.ShortName.substring(0, 2));

            assignments.append(cell);
            sFilterData += curAssign.ShortName + ';'
        }

        tRow.attr('data-filterData', sFilterData);

        container.append(assignments);
    } else {
        tRow.attr('data-filterData', '');
    }

    if (oCalendar.typeView != Robotics.Client.Constants.TypeView.Review) {
        var calendarImg = $('<div></div>').attr('data-IDEmployee', EmployeeData.IDEmployee).attr('class', 'fullCalendarShow').attr('style', 'float: right;margin-top: 8px;');

        container.append(calendarImg);
    }

    return container;
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.buildPlanificationRowView = function (rowIndex, tBodyRow) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    var empData = oCalData.CalendarData[rowIndex].EmployeeData;
    var dayData = oCalData.CalendarData[rowIndex].PeriodData.DayData;

    if (oCalendar.viewRange == Robotics.Client.Constants.ViewRange.Daily) {
        for (var columnIndex = oCalendar.getMinDailyCell(); columnIndex < oCalendar.getMaxDailyCell(); columnIndex++) {
            //for (var columnIndex = 0; columnIndex < oCalData.CalendarHeader.PeriodHeaderData.length; columnIndex++) {
            tBodyRow.append(this.createDailyCalendarCell(empData, dayData[0], dayData[0].HourData[columnIndex], rowIndex, columnIndex));
        }
    } else if (oCalendar.viewRange == Robotics.Client.Constants.ViewRange.Period) {
        //Vista multiplos dias
        for (var columnIndex = 0; columnIndex < dayData.length; columnIndex++) {
            tBodyRow.append(this.createCalendarCell(empData, dayData[columnIndex], rowIndex, columnIndex));
        }
    }
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.createDailyCalendarCell = function (EmployeeData, dayData, cellInfo, rowPosition, columnPosition) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    var calendarCell = $('<td></td>');

    if (columnPosition % 2 == 0) {
        calendarCell.addClass('columnDailyCalendarOdd');
    }

    var calendarOuterContent = $('<div></div>').attr('id', oCalendar.ascxPrefix + '_calDailyCell_' + rowPosition + '_' + columnPosition).attr('class', 'calendarDailyBodyCell DailyCell').attr('data-IDEmployee', EmployeeData.IDEmployee).attr('data-IDGroup', EmployeeData.IDGroup);
    calendarOuterContent.attr('data-IDRow', rowPosition).attr('data-IDColumn', columnPosition).attr('data-Date', moment(dayData.PlanDate).format('DD/MM/YYYY'));

    this.createDailyCalendarCellContent(calendarOuterContent, EmployeeData, dayData, cellInfo, columnPosition);

    return calendarCell.append(calendarOuterContent);
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.createDailyCalendarCellContent = function (containter, EmployeeData, dayData, cellInfo, columnPosition) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    var calendarInnerContent = $('<div></div>').attr('class', 'calendarDailyInnerBodyCell');
    var nameShift = null;
    var changeShift = null;
    var shiftAssigned = this.getAssignedShift(dayData);

    if (cellInfo.DailyHourType != Robotics.Client.Constants.DailyHourType.Untyped || (shiftAssigned != null && shiftAssigned.PlannedHours == 0 && (dayData.ShiftBase == null || (dayData.ShiftBase != null && dayData.ShiftBase.PlannedHours == 0)))) {
        if (dayData.Locked) { calendarInnerContent.addClass('dailyLock'); }

        var startColor = '#ffffff';
        var shiftName = '';

        var assignmentStyle = '';
        var assignmentName = '';
        var marginAssignment = '';

        if (oCalendar.isScheduleActive) {
            if (typeof dayData.AssigData != 'undefined' && dayData.AssigData != null) {
                assignmentName = " (" + dayData.AssigData.Name + ")";
                assignmentStyle = 'border-bottom: 8px ' + dayData.AssigData.Color + ' solid;'
                marginAssignment = 'margin-top: -30px;'
            } else {
                assignmentStyle = 'border-bottom: 8px #F2F4F2 solid;'
                marginAssignment = 'margin-top: -30px;'
            }
        }

        if (shiftAssigned != null) {
            startColor = shiftAssigned.Color;
            shiftName = shiftAssigned.Name;
        }

        switch (cellInfo.DailyHourType) {
            case Robotics.Client.Constants.DailyHourType.Mandatory:
                startColor = startColor;
                break;
            case Robotics.Client.Constants.DailyHourType.Flexible:
                startColor = new Robotics.Client.Common.roHtmlColor().shadeBlendConvert(0.5, startColor);
                break;
            case Robotics.Client.Constants.DailyHourType.Complementary:
                startColor = new Robotics.Client.Common.roHtmlColor().shadeBlendConvert(-0.5, startColor);
                break;
        }

        if ((oCalendar.firstCellPrinted == -1 && cellInfo.DailyHourType != Robotics.Client.Constants.DailyHourType.Untyped) || columnPosition < oCalendar.firstCellPrinted) {
            oCalendar.firstCellPrinted = columnPosition;
        }

        if (cellInfo.IsHoursAbsence || cellInfo.IsHoursHoliday) {
            var lighterColor = new Robotics.Client.Common.roHtmlColor().shadeBlendConvert(0.5, startColor);
            var gradientStyle = "";

            gradientStyle += 'background: ' + startColor + ';';
            gradientStyle += 'background: -moz-linear-gradient(top, ' + startColor + ' 0%, ' + lighterColor + ' 50%, ' + lighterColor + ' 50%, ' + lighterColor + ' 50%, ' + startColor + ' 100%);';
            gradientStyle += 'background: -webkit-gradient(left top, left bottom, color-stop(0%, ' + startColor + '), color-stop(50%, ' + lighterColor + '), color-stop(50%, ' + lighterColor + '), color-stop(50%, ' + lighterColor + '), color-stop(100%, ' + startColor + '));';
            gradientStyle += 'background: -webkit-linear-gradient(top, ' + startColor + ' 0%, ' + lighterColor + ' 50%, ' + lighterColor + ' 50%, ' + lighterColor + ' 50%, ' + startColor + ' 100%);';
            gradientStyle += 'background: -o-linear-gradient(top, ' + startColor + ' 0%, ' + lighterColor + ' 50%, ' + lighterColor + ' 50%, ' + lighterColor + ' 50%, ' + startColor + ' 100%);';
            gradientStyle += 'background: -ms-linear-gradient(top, ' + startColor + ' 0%, ' + lighterColor + ' 50%, ' + lighterColor + ' 50%, ' + lighterColor + ' 50%, ' + startColor + ' 100%);';
            gradientStyle += 'background: linear-gradient(to bottom, ' + startColor + ' 0%, ' + lighterColor + ' 50%, ' + lighterColor + ' 50%, ' + lighterColor + ' 50%, ' + startColor + ' 100%);';
            gradientStyle += 'filter: progid:DXImageTransform.Microsoft.gradient( startColorstr="' + startColor + '", endColorstr="' + startColor + '", GradientType=0 );';

            calendarInnerContent.attr('style', gradientStyle + assignmentStyle);
        } else {
            calendarInnerContent.attr('style', 'background: ' + startColor + ';' + assignmentStyle);
        }

        if (columnPosition == 0 ||
            (columnPosition > 0 && dayData.HourData[columnPosition - 1].DailyHourType == Robotics.Client.Constants.DailyHourType.Untyped && dayData.HourData[columnPosition].DailyHourType != Robotics.Client.Constants.DailyHourType.Untyped) ||
            (columnPosition == oCalendar.getMinDailyCell() && shiftAssigned != null && shiftAssigned.PlannedHours == 0 && dayData.ShiftBase == null)
        ) {
            nameShift = this.generateShiftInfoDailyContainer(shiftName + assignmentName, startColor, dayData.Feast, dayData.FeastDescription, dayData.IDDailyBudgetPosition, dayData.ProductiveUnit, (moment(dayData.PlanDate) <= moment(EmployeeData.FreezingDate)));
        }

        if ((shiftAssigned != null && shiftAssigned.PlannedHours != 0) && ((columnPosition == 0) || (cellInfo.DailyHourType != dayData.HourData[columnPosition - 1].DailyHourType))) {
            var hours = oCalendar.dailyPeriodHourFraction();

            var tmpPosition = columnPosition;
            while (tmpPosition > 0 && dayData.HourData[tmpPosition].DailyHourType == dayData.HourData[tmpPosition + 1].DailyHourType) {
                hours = hours + oCalendar.dailyPeriodHourFraction();
                tmpPosition = tmpPosition + 1;
            }

            changeShift = this.generateShiftTypeInfoDailyContainer(cellInfo.DailyHourType, startColor, hours, marginAssignment);
        }
    }

    containter.append(nameShift, calendarInnerContent, changeShift);
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.generateShiftTypeInfoDailyContainer = function (shiftType, color, hours, marginAssignment) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    var shiftDesc = '';
    switch (shiftType) {
        case Robotics.Client.Constants.DailyHourType.Mandatory:
            shiftDesc = oCalendar.translator.translate(Robotics.Client.Language.Tags.Mandatory);
            break;
        case Robotics.Client.Constants.DailyHourType.Flexible:
            shiftDesc = oCalendar.translator.translate(Robotics.Client.Language.Tags.Flexible);
            break;
        case Robotics.Client.Constants.DailyHourType.Complementary:
            shiftDesc = oCalendar.translator.translate(Robotics.Client.Language.Tags.Complementary);
            break;
    }
    var cellText = shiftDesc + '(' + hours + 'h)';
    var shiftInfoContainer = $('<div></div>').attr('class', 'calendarShiftTypeInfoDailyContainer').attr('title', cellText).html(cellText);

    shiftInfoContainer.attr('style', 'color:' + new Robotics.Client.Common.roHtmlColor().invertCssColor(color) + ';' + marginAssignment);

    return shiftInfoContainer;
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.generateShiftInfoDailyContainer = function (shiftName, color, isFeastDay, feastDescription, isBudget, productiveUnitName, isDateFreeze) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    var shiftInfoContainer = $('<div></div>').attr('class', 'calendarShiftInfoDailyContainer').attr('title', shiftName).append($("<span class='fontBold calendarFeastLeft'></span>").html(shiftName));

    if (isFeastDay) {
        shiftInfoContainer.append("<div class='calendarFeast calendarFeastLeft' title='" + (feastDescription != '' ? feastDescription : oCalendar.translator.translate(Robotics.Client.Language.Tags.Tooltip_feast)) + "'></div>");
    }

    if (isDateFreeze) {
        shiftInfoContainer.append("<div class='calendarLockDate calendarFeastLeft' title='" + Globalize.formatMessage('roPeriodInFreezeDate') + "'>  </div>");
    }

    if (isBudget > 0) {
        shiftInfoContainer.append($('<div></div>').attr('class', 'dailyCalendarBudgetResticted calendarFeastLeft').attr('title', Globalize.formatMessage('roAlreadyInBudget', productiveUnitName)));
    }

    shiftInfoContainer.attr('style', 'color:' + new Robotics.Client.Common.roHtmlColor().invertCssColor(color));

    return shiftInfoContainer;
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.createCalendarCell = function (EmployeeData, cellInfo, rowPosition, columnPosition) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    var calendarCell = $('<td></td>');

    var calendarOuterContent = $('<div></div>').attr('id', oCalendar.ascxPrefix + '_calCell_' + rowPosition + '_' + columnPosition);
    calendarOuterContent.attr('data-IDEmployee', EmployeeData.IDEmployee).attr('data-IDGroup', EmployeeData.IDGroup);
    calendarOuterContent.attr('data-IDRow', rowPosition).attr('data-IDColumn', columnPosition).attr('data-Date', moment(cellInfo.PlanDate).format('DD/MM/YYYY'));

    if (cellInfo.EmployeeStatusOnDay == Robotics.Client.Constants.EmployeeStatusOnDay.Ok) {
        calendarOuterContent.attr('class', 'calendarOuterBodyCell');
        this.buildCalendarCellContent(calendarOuterContent, cellInfo, true, EmployeeData.FreezingDate);
    } else if (cellInfo.EmployeeStatusOnDay == Robotics.Client.Constants.EmployeeStatusOnDay.NoContract) {
        calendarOuterContent.attr('class', 'calendarOuterBodyCellNoContract');
    } else if (cellInfo.EmployeeStatusOnDay == Robotics.Client.Constants.EmployeeStatusOnDay.InOtherDepartment) {
        calendarOuterContent.attr('class', 'calendarOuterBodyCellInOtherDepartment');
    }

    return calendarCell.append(calendarOuterContent);
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.buildCalendarCellContent = function (calendarOuterContent, cellInfo, bAddIndictments, employeeFreezeDate) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    var calendarInnerContent = $('<div></div>').attr('class', 'calendarInnerBodyCell').html('&nbsp;');
    calendarInnerContent.attr('style', this.generateGradientFromColor(cellInfo));

    calendarInnerContent.append(this.generateShiftInfoContainer(cellInfo, calendarOuterContent.attr('data-IDRow'), calendarOuterContent.attr('data-IDColumn'), (moment(cellInfo.PlanDate) <= moment(employeeFreezeDate))));
    calendarInnerContent.append(this.generateCalendarAlertsContainer(cellInfo, calendarOuterContent.attr('data-IDRow'), calendarOuterContent.attr('data-IDColumn'), bAddIndictments));

    var isStarterShift = false;
    if (typeof cellInfo != 'undefined' && cellInfo.MainShift != null && cellInfo.MainShift.AdvancedParameters.find(function (x) { return x.Name.toLowerCase() == 'starter' && x.Value == '1' }) != null) isStarterShift = true;
    if (!isStarterShift && typeof cellInfo != 'undefined' && cellInfo.ShiftBase != null && cellInfo.ShiftBase.AdvancedParameters.find(function (x) { return x.Name.toLowerCase() == 'starter' && x.Value == '1' }) != null) isStarterShift = true;

    if (!isStarterShift && oCalendar.isScheduleActive) calendarInnerContent.append(this.generateCalendarAssignmentsContainer(cellInfo));

    calendarOuterContent.append(calendarInnerContent);

    if (cellInfo.Locked) calendarOuterContent.append(this.generateLockedDayMask());
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.generateCalendarAlertsContainer = function (cellInfo, idRow, idColumn, bAddIndictments) {
    var oClientMode = this;
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    var assignmentsContainer = $('<div></div>').attr('class', 'calendarCellContainer');

    if (oClientMode.currentUpdateCell != null && oClientMode.currentUpdateCell.Alerts != null) {
        cellInfo.Alerts = Object.clone(oClientMode.currentUpdateCell.Alerts, true);
        cellInfo.HasChanged = true;
    }
    if (cellInfo.Alerts != null) {
        if (oCalendar.telecommuteEnabled) {
            if (typeof (cellInfo.TelecommuteForced) != 'undefined' && cellInfo.TelecommuteForced) {
                if (typeof (cellInfo.MainShift) != 'undefined' && cellInfo.MainShift != null && cellInfo.MainShift.PlannedHours > 0 && !cellInfo.Alerts.OnAbsenceDays && !cellInfo.Alerts.OnHolidays) {
                    if (cellInfo.TelecommutingExpected) {
                        assignmentsContainer.append($('<div></div>').attr('class', 'calendarTelecommuting').attr('title', Globalize.formatMessage("roOnTelecommute")));
                    } else if (cellInfo.TelecommutingOptional) {
                        assignmentsContainer.append($('<div></div>').attr('class', 'calendarOptionalTelecommuting').attr('title', Globalize.formatMessage("roOnOptional")));
                    } else {
                        assignmentsContainer.append($('<div></div>').attr('class', 'calendarPresence').attr('title', Globalize.formatMessage("roOnPresence")));
                    }
                }
            } else {
                //if (cellInfo.TelecommutingExpected && (typeof (cellInfo.MainShift) != 'undefined' && cellInfo.MainShift != null && cellInfo.CanTelecommute && cellInfo.MainShift.PlannedHours > 0 && !cellInfo.Alerts.OnAbsenceDays && !cellInfo.Alerts.OnHolidays)) {
                //    assignmentsContainer.append($('<div></div>').attr('class', 'calendarTelecommuting').attr('title', Globalize.formatMessage("roOnTelecommute")));
                //}
                //else {
                if (cellInfo.CanTelecommute && (typeof (cellInfo.MainShift) != 'undefined' && cellInfo.MainShift != null) && cellInfo.MainShift.PlannedHours > 0 && !cellInfo.Alerts.OnAbsenceDays && !cellInfo.Alerts.OnHolidays) {
                    if (cellInfo.TelecommutingOptionalDays != null && cellInfo.TelecommutingOptionalDays.indexOf(moment(cellInfo.PlanDate).day().toString()) >= 0) {
                        assignmentsContainer.append($('<div></div>').attr('class', 'calendarOptionalTelecommuting').attr('title', Globalize.formatMessage("roOnOptional")));
                    } else if (cellInfo.PresenceMandatoryDays != null && cellInfo.PresenceMandatoryDays.indexOf(moment(cellInfo.PlanDate).day().toString()) >= 0) {
                        assignmentsContainer.append($('<div></div>').attr('class', 'calendarPresence').attr('title', Globalize.formatMessage("roOnPresence")));
                    } else if (cellInfo.TelecommutingMandatoryDays != null && cellInfo.TelecommutingMandatoryDays.indexOf(moment(cellInfo.PlanDate).day().toString()) >= 0) {
                        assignmentsContainer.append($('<div></div>').attr('class', 'calendarTelecommuting').attr('title', Globalize.formatMessage("roOnTelecommute")));
                    }
                }
                //}
            }
        }

        if (cellInfo.Alerts.OnAbsenceDays) assignmentsContainer.append($('<div></div>').attr('class', 'calendarPlannedAbsence').attr('title', oCalendar.translator.translate(Robotics.Client.Language.Tags.Tooltip_absence)));
        if (cellInfo.Alerts.OnAbsenceHours) assignmentsContainer.append($('<div></div>').attr('class', 'calendarPlannedIncidence').attr('title', oCalendar.translator.translate(Robotics.Client.Language.Tags.Tooltip_incidence)));
        if (cellInfo.Alerts.OnOvertimesHours) assignmentsContainer.append($('<div></div>').attr('class', 'calendarPlannedOvertme').attr('title', oCalendar.translator.translate(Robotics.Client.Language.Tags.Tooltip_overtime)));
        if (cellInfo.Alerts.OnHolidaysHours) assignmentsContainer.append($('<div></div>').attr('class', 'calendarPlannedHoliday').attr('title', oCalendar.translator.translate(Robotics.Client.Language.Tags.Tooltip_plannedHoliday)));
        if (cellInfo.Alerts.OnHolidays) assignmentsContainer.append($('<div></div>').attr('class', 'calendarHolidays').attr('title', oCalendar.translator.translate(Robotics.Client.Language.Tags.Tooltip_holidays)));

        if (oCalendar.loadIndictments) {
            var dayIndictments = [];
            var cssClass = 'calendarUnexpectedlyAbsent';

            if (cellInfo.Alerts != null && cellInfo.Alerts.UnexpectedlyAbsent != null && cellInfo.Alerts.UnexpectedlyAbsent) {
                dayIndictments.push({ employeeName: '', indictment: { ErrorText: oCalendar.translator.translate(Robotics.Client.Language.Tags.Tooltip_absent) } });
            }

            if (oCalendar.loadIndictments && cellInfo.Alerts != null && cellInfo.Alerts.Indictments != null && cellInfo.Alerts.Indictments.length > 0) {
                cssClass = 'calendarIndictments';

                for (iInd = 0; iInd < cellInfo.Alerts.Indictments.length; iInd++) {
                    dayIndictments.push({ employeeName: '', indictment: Object.clone(cellInfo.Alerts.Indictments[iInd], true) });

                    if (bAddIndictments) {
                        if (typeof oClientMode.calendarIndictments[cellInfo.Alerts.Indictments[iInd].RuleName] == 'undefined') {
                            oClientMode.calendarIndictments[cellInfo.Alerts.Indictments[iInd].RuleName] = { key: cellInfo.Alerts.Indictments[iInd].RuleName, counter: 0, items: [] };
                        }

                        var indictmentItem = oClientMode.calendarIndictments[cellInfo.Alerts.Indictments[iInd].RuleName].items.find(function (indict) {
                            return indict.idIndictment == cellInfo.Alerts.Indictments[iInd].ID;
                        })

                        if (indictmentItem == null) {
                            var employeeName = oClientMode.oBaseControl.oCalendar.CalendarData[idRow].EmployeeData.EmployeeName;
                            indictmentItem = { idRow: [idRow], idColumn: [idColumn], idIndictment: cellInfo.Alerts.Indictments[iInd].ID, description: employeeName + ": " + cellInfo.Alerts.Indictments[iInd].ErrorText, dates: [moment(cellInfo.PlanDate).format("DD/MM/YYYY")] }

                            oClientMode.calendarIndictments[cellInfo.Alerts.Indictments[iInd].RuleName].items.push(indictmentItem);
                            oClientMode.calendarIndictments[cellInfo.Alerts.Indictments[iInd].RuleName].counter++;
                        } else {
                            indictmentItem.idRow.push(idRow);
                            indictmentItem.idColumn.push(idColumn);
                            indictmentItem.dates.push(moment(cellInfo.PlanDate).format("DD/MM/YYYY"));
                        }
                    }
                }
            }
            if (dayIndictments.length > 0) {
                if (!oClientMode.validationInProgress && cellInfo.HasChanged && cssClass.indexOf('calendarIndictments') >= 0) cssClass = 'calendarIndictments-recalc';

                var alertDiv = $('<div></div>').attr('id', oCalendar.ascxPrefix + '_tooltipIndictments_' + idRow + '_' + idColumn).attr('class', cssClass + ' indictments_target');

                var hiddenDiv = $('<div></div>').attr("style", "position:absolute; left:-200%");

                var indictmentTooltipContainer = $('<div></div>').attr('id', oCalendar.ascxPrefix + '_tooltipIndictmentsContainer_' + idRow + '_' + idColumn).attr('class', 'tooltipIndictmentsContainer');
                var divIndictMentsTooltipTitle = $('<div></div>').html('<span class="fontBold fontBig" style="text-align:left">' + Globalize.formatMessage("roIndictmentsTitle") + '</span>');

                var ulInditmentsTooltipList = $('<ul></ul>');

                for (var iDayI = 0; iDayI < dayIndictments.length; iDayI++) {
                    ulInditmentsTooltipList.append($('<li></li>').html(dayIndictments[iDayI].indictment.ErrorText))
                }

                indictmentTooltipContainer.append(divIndictMentsTooltipTitle, ulInditmentsTooltipList);
                assignmentsContainer.append(alertDiv.append(hiddenDiv.append(indictmentTooltipContainer)));
            }
        }
    }

    if (cellInfo.Remarks != "0" && cellInfo.Remarks != "") assignmentsContainer.append($('<div></div>').attr('class', 'calendarNotes').attr('title', oCalendar.translator.translate(Robotics.Client.Language.Tags.Tooltip_notes)));

    if (oClientMode.currentUpdateCell != null) { oClientMode.currentUpdateCell = null; }
    return assignmentsContainer;
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.generateCalendarAssignmentsContainer = function (cellInfo) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    var alertsContainer = $('<div></div>').attr('class', 'calendarCellContainer cellborderinset');

    if (typeof cellInfo.AssigData != 'undefined' && cellInfo.AssigData != null) {
        alertsContainer.attr('style', 'background:' + cellInfo.AssigData.Color + ';color:' + new Robotics.Client.Common.roHtmlColor().invertCssColor(cellInfo.AssigData.Color));

        alertsContainer.append($('<div></div>').attr('class', 'AssignmentText').attr('title', cellInfo.AssigData.Name).html(cellInfo.AssigData.ShortName.substring(0, 2)));

        if (cellInfo.IDDailyBudgetPosition > 0) alertsContainer.append($('<div></div>').attr('class', 'calendarBudgetResticted').attr('title', Globalize.formatMessage('roAlreadyInBudget', cellInfo.ProductiveUnit)));
    }

    return alertsContainer;
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.generateGradientFromColor = function (cellInfo) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    var startColor = '#ffffff';
    var endColor = '#ffffff';

    var startChange = '100%';
    var changeToWhite = '100%';

    if (cellInfo.Alerts != null && oCalendar.typeView != Robotics.Client.Constants.TypeView.Review) {
        if (cellInfo.Alerts.OnAbsenceDays) { startChange = '21%'; changeToWhite = '50%'; }
        else if (cellInfo.Alerts.OnAbsenceHours || cellInfo.Alerts.OnHolidaysHours) { startChange = '50%'; changeToWhite = '85%'; }
    }

    var shiftAssigned = this.getAssignedShift(cellInfo);

    if (shiftAssigned != null) {
        startColor = shiftAssigned.Color;
    }

    var gradientStyle = "";
    gradientStyle = 'background: ' + startColor + ';';
    gradientStyle += 'background: -moz-linear-gradient(-45deg, ' + startColor + ' 0%, ' + startColor + ' ' + startChange + ', ' + endColor + ' ' + changeToWhite + ', ' + endColor + ' 100%);';
    gradientStyle += 'background: -webkit-gradient(left top, right bottom, color-stop(0%, ' + startColor + '), color-stop(' + startChange + ', ' + startColor + '), color-stop(' + changeToWhite + ', ' + endColor + '), color-stop(100%, ' + endColor + '));';
    gradientStyle += 'background: -webkit-linear-gradient(-45deg, ' + startColor + ' 0%, ' + startColor + ' ' + startChange + ', ' + endColor + ' ' + changeToWhite + ', ' + endColor + ' 100%);';
    gradientStyle += 'background: -o-linear-gradient(-45deg, ' + startColor + ' 0%, ' + startColor + ' ' + startChange + ', ' + endColor + ' ' + changeToWhite + ', ' + endColor + ' 100%);';
    gradientStyle += 'background: -ms-linear-gradient(-45deg, ' + startColor + ' 0%, ' + startColor + ' ' + startChange + ', ' + endColor + ' ' + changeToWhite + ', ' + endColor + ' 100%);';
    gradientStyle += 'background: linear-gradient(135deg, ' + startColor + ' 0%, ' + startColor + ' ' + startChange + ', ' + endColor + ' ' + changeToWhite + ', ' + endColor + ' 100%);';
    gradientStyle += 'filter: progid:DXImageTransform.Microsoft.gradient( startColorstr="' + startColor + '", endColorstr="' + endColor + '", GradientType=1 );';

    return gradientStyle;
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.generateLockedDayMask = function (cellInfo) {
    var lockedDayContainer = $('<div></div>').attr('class', 'calendarLockedDay');

    return lockedDayContainer;
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.generateShiftInfoContainer = function (cellInfo, idRow, idColumn, isDateFreeze) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    var shiftInfoContainer = null;

    var shiftAssigned = this.getAssignedShift(cellInfo);
    var isStarterShift = false;

    if (shiftAssigned != null) {
        if (typeof cellInfo != 'undefined' && cellInfo.MainShift != null && cellInfo.MainShift.AdvancedParameters.find(function (x) { return x.Name.toLowerCase() == 'starter' && x.Value == '1' }) != null) isStarterShift = true;

        if (oCalendar.typeView != Robotics.Client.Constants.TypeView.Review) {
            shiftInfoContainer = this.generateShiftDetailCell(shiftAssigned, cellInfo.ShiftBase, cellInfo.Feast, cellInfo.FeastDescription, cellInfo.Alerts, isDateFreeze);
        } else {
            shiftInfoContainer = this.generateReviewShiftDetailCell(cellInfo, idRow, idColumn, isDateFreeze);
        }
    } else {
        shiftInfoContainer = this.generateEmptyShiftDetailCell(cellInfo.Feast, cellInfo.FeastDescription, isDateFreeze);
    }

    if (shiftInfoContainer == null) {
        if (oCalendar.typeView != Robotics.Client.Constants.TypeView.Review) {
            shiftInfoContainer = $('<div></div>').attr('class', 'calendarShiftInfoContainer');
        } else {
            shiftInfoContainer = $('<div></div>').attr('class', 'calendarReviewShiftInfoContainer');
        }
        shiftInfoContainer.attr('style', 'color:' + new Robotics.Client.Common.roHtmlColor().invertCssColor("#FFFFFF"));
    } else {
        if (isStarterShift) shiftInfoContainer.attr('style', 'font-size:10px');
    }

    return shiftInfoContainer;
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.generateReviewShiftDetailCell = function (cellInfo, idRow, idColumn, isDateFreeze) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    var shiftData = this.getAssignedShift(cellInfo);

    var shiftInfoContainer = $('<div></div>').attr('class', 'calendarReviewShiftInfoContainer').attr('title', shiftData.Name);

    var titleLine = $('<div></div>').attr('class', 'calendarReviewShiftName');
    titleLine.attr('style', 'background-color:' + shiftData.Color + ';border:2px solid ' + new Robotics.Client.Common.roHtmlColor().shadeBlendConvert(-0.5, shiftData.Color));
    titleLine.append($('<div></div>').attr('class', 'reviewShift').html(shiftData.ShortName.substr(0, 3)));

    if (isDateFreeze) {
        titleLine.append("<div class='calendarLockDate calendarFloatIcon' title='" + Globalize.formatMessage('roPeriodInFreezeDate') + "'>  </div>");
    }

    shiftInfoContainer.append(titleLine);

    var infoDiv = $('<div></div>').attr('class', 'calendarReviewContainter');

    var remarksLine = $('<div></div>').attr('class', 'calendarReviewRemarks');
    remarksLine.append(this.generateReviewCalendarAlertsContainer(cellInfo, idRow, idColumn));
    infoDiv.append(remarksLine);

    var alertsLine = $('<div></div>').attr('class', 'calendarReviewAlerts');
    alertsLine.append(this.generateReviewCalendarRemarksContainer(cellInfo));
    infoDiv.append(alertsLine);

    shiftInfoContainer.append(infoDiv);

    shiftInfoContainer.attr('style', 'color:' + new Robotics.Client.Common.roHtmlColor().invertCssColor(shiftData.Color));

    return shiftInfoContainer;
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.generateReviewCalendarAlertsContainer = function (cellInfo, idRow, idColumn) {
    var oClientMode = this;
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    var assignmentsContainer = $('<div></div>');

    if (cellInfo.Alerts != null) {
        if (!cellInfo.Alerts.OnAbsenceDays && cellInfo.TelecommutingExpected) assignmentsContainer.append($('<div></div>').attr('class', 'calendarTelecommuting contentHorizontally').attr('title', Globalize.formatMessage("roOnTelecommute")));
        if (cellInfo.Alerts.OnAbsenceDays) assignmentsContainer.append($('<div></div>').attr('class', 'calendarPlannedAbsence contentHorizontally').attr('title', oCalendar.translator.translate(Robotics.Client.Language.Tags.Tooltip_absence)));
        if (cellInfo.Alerts.OnAbsenceHours) assignmentsContainer.append($('<div></div>').attr('class', 'calendarPlannedIncidence contentHorizontally').attr('title', oCalendar.translator.translate(Robotics.Client.Language.Tags.Tooltip_incidence)));
        if (cellInfo.Alerts.OnOvertimesHours) assignmentsContainer.append($('<div></div>').attr('class', 'calendarPlannedOvertme contentHorizontally').attr('title', oCalendar.translator.translate(Robotics.Client.Language.Tags.Tooltip_overtime)));
        if (cellInfo.Alerts.OnHolidaysHours) assignmentsContainer.append($('<div></div>').attr('class', 'calendarPlannedHoliday contentHorizontally').attr('title', oCalendar.translator.translate(Robotics.Client.Language.Tags.Tooltip_plannedHoliday)));
        if (cellInfo.Alerts.OnHolidays) assignmentsContainer.append($('<div></div>').attr('class', 'calendarHolidays contentHorizontally').attr('title', oCalendar.translator.translate(Robotics.Client.Language.Tags.Tooltip_holidays)));

        var cssClass = 'calendarUnexpectedlyAbsent contentHorizontally';

        if (oCalendar.loadIndictments) {
            var dayIndictments = [];
            if (cellInfo.Alerts != null && cellInfo.Alerts.UnexpectedlyAbsent != null && cellInfo.Alerts.UnexpectedlyAbsent) {
                dayIndictments.push({ employeeName: '', indictment: { ErrorText: oCalendar.translator.translate(Robotics.Client.Language.Tags.Tooltip_absent) } });
            }

            if (oCalendar.loadIndictments && cellInfo.Alerts != null && cellInfo.Alerts.Indictments != null && cellInfo.Alerts.Indictments.length > 0) {
                cssClass = 'calendarIndictments contentHorizontally';
                for (iInd = 0; iInd < cellInfo.Alerts.Indictments.length; iInd++) {
                    dayIndictments.push({ employeeName: '', indictment: Object.clone(cellInfo.Alerts.Indictments[iInd], true) });

                    if (typeof oClientMode.calendarIndictments[cellInfo.Alerts.Indictments[iInd].IDScheduleRule] == 'undefined') {
                        oClientMode.calendarIndictments[cellInfo.Alerts.Indictments[iInd].IDScheduleRule] = { key: cellInfo.Alerts.Indictments[iInd].RuleName, counter: 0, items: [] };
                    }

                    var indictmentItem = oClientMode.calendarIndictments[cellInfo.Alerts.Indictments[iInd].IDScheduleRule].items.find(function (indict) {
                        return indict.idIndictment == cellInfo.Alerts.Indictments[iInd].ID;
                    })

                    if (indictmentItem == null) {
                        var employeeName = oClientMode.oBaseControl.oCalendar.CalendarData[idRow].EmployeeData.EmployeeName;
                        indictmentItem = { idRow: [idRow], idColumn: [idColumn], idIndictment: cellInfo.Alerts.Indictments[iInd].ID, description: employeeName + ": " + cellInfo.Alerts.Indictments[iInd].ErrorText, dates: [moment(cellInfo.PlanDate).format("DD/MM/YYYY")] }

                        oClientMode.calendarIndictments[cellInfo.Alerts.Indictments[iInd].IDScheduleRule].items.push(indictmentItem);
                        oClientMode.calendarIndictments[cellInfo.Alerts.Indictments[iInd].IDScheduleRule].counter++;
                    } else {
                        indictmentItem.idRow.push(idRow);
                        indictmentItem.idColumn.push(idColumn);
                        indictmentItem.dates.push(moment(cellInfo.PlanDate).format("DD/MM/YYYY"));
                    }

                    //if (typeof oClientMode.calendarIndictments[cellInfo.Alerts.Indictments[iInd].IDScheduleRule] == 'undefined') {
                    //    oClientMode.calendarIndictments[cellInfo.Alerts.Indictments[iInd].IDScheduleRule] = { key: cellInfo.Alerts.Indictments[iInd].RuleName, counter: 0, items: [] };
                    //}

                    //var employeeName = oClientMode.oBaseControl.oCalendar.CalendarData[idRow].EmployeeData.EmployeeName;

                    //oClientMode.calendarIndictments[cellInfo.Alerts.Indictments[iInd].IDScheduleRule].items.push({ idRow: idRow, idColumn: idColumn, description: employeeName + ": " + cellInfo.Alerts.Indictments[iInd].ErrorText });
                    //oClientMode.calendarIndictments[cellInfo.Alerts.Indictments[iInd].IDScheduleRule].counter++;
                }
            }
            if (dayIndictments.length > 0) {
                if (!oClientMode.validationInProgress && cellInfo.HasChanged && cssClass.indexOf('calendarIndictments') >= 0) cssClass = 'calendarIndictments-recalc contentHorizontally';
                var alertsDiv = $('<div></div>').attr('id', oCalendar.ascxPrefix + '_tooltipIndictments_' + idRow + '_' + idColumn).attr('title', '').attr('class', cssClass + ' indictments_target');

                var hiddenDiv = $('<div></div>').attr("style", "position:absolute; left:-200%");
                var indictmentTooltipContainer = $('<div></div>').attr('id', oCalendar.ascxPrefix + '_tooltipIndictmentsContainer_' + idRow + '_' + idColumn).attr('class', 'tooltipIndictmentsContainer');
                var divIndictMentsTooltipTitle = $('<div></div>').html('<span class="fontBold fontBig" style="text-align:left">' + Globalize.formatMessage("roIndictmentsTitle") + '</span>');

                var ulInditmentsTooltipList = $('<ul></ul>');

                for (var iDayI = 0; iDayI < dayIndictments.length; iDayI++) {
                    ulInditmentsTooltipList.append($('<li></li>').html(dayIndictments[iDayI].indictment.ErrorText))
                }

                indictmentTooltipContainer.append(divIndictMentsTooltipTitle, ulInditmentsTooltipList);
                assignmentsContainer.append(alertsDiv.append(hiddenDiv.append(indictmentTooltipContainer)));
            }
        }

        //if (cellInfo.Alerts.UnexpectedlyAbsent) assignmentsContainer.append($('<div></div>').attr('class', 'calendarUnexpectedlyAbsent contentHorizontally').attr('title', oCalendar.translator.translate(Robotics.Client.Language.Tags.Tooltip_absent)));
    }

    if (cellInfo.Remarks != "0" && cellInfo.Remarks != "") assignmentsContainer.append($('<div></div>').attr('class', 'calendarNotes').attr('title', oCalendar.translator.translate(Robotics.Client.Language.Tags.Tooltip_notes)));

    return assignmentsContainer;
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.generateReviewCalendarRemarksContainer = function (cellInfo) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    var assignmentsContainer = $('<div></div>').attr('class', 'calendarRemarks');

    if (cellInfo.IncidenceData != null) {
        if (cellInfo.IncidenceData.Remark1) {
            assignmentsContainer.append($('<div></div>').attr('class', 'calendarRemark1 remark1Active').attr('style', 'background-color:' + oCalendar.remarksColor[0] + ';border:1px solid ' + new Robotics.Client.Common.roHtmlColor().shadeBlendConvert(-0.5, oCalendar.remarksColor[0])).attr('title', oCalendar.translator.translate(Robotics.Client.Language.Tags.Tooltip_remark1)));
        } else {
            assignmentsContainer.append($('<div></div>').attr('class', 'calendarRemark1 remarkInactive').attr('title', ''));
        }

        if (cellInfo.IncidenceData.Remark2) {
            assignmentsContainer.append($('<div></div>').attr('class', 'calendarRemark2 remark2Active').attr('style', 'background-color:' + oCalendar.remarksColor[1] + ';border:1px solid ' + new Robotics.Client.Common.roHtmlColor().shadeBlendConvert(-0.5, oCalendar.remarksColor[1])).attr('title', oCalendar.translator.translate(Robotics.Client.Language.Tags.Tooltip_remark2)));
        } else {
            assignmentsContainer.append($('<div></div>').attr('class', 'calendarRemark2 remarkInactive').attr('title', ''));
        }

        if (cellInfo.IncidenceData.Remark3) {
            assignmentsContainer.append($('<div></div>').attr('class', 'calendarRemark3 remark3Active').attr('style', 'background-color:' + oCalendar.remarksColor[2] + ';border:1px solid ' + new Robotics.Client.Common.roHtmlColor().shadeBlendConvert(-0.5, oCalendar.remarksColor[2])).attr('title', oCalendar.translator.translate(Robotics.Client.Language.Tags.Tooltip_remark3)));
        } else {
            assignmentsContainer.append($('<div></div>').attr('class', 'calendarRemark3 remarkInactive').attr('title', ''));
        }
    } else {
        assignmentsContainer.append($('<div></div>').attr('class', 'calendarRemark1 remarkInactive').attr('title', cellInfo.IncidenceData.Remark1 ? oCalendar.translator.translate(Robotics.Client.Language.Tags.Tooltip_remark1) : ''));
        assignmentsContainer.append($('<div></div>').attr('class', 'calendarRemark2 remarkInactive').attr('title', cellInfo.IncidenceData.Remark2 ? oCalendar.translator.translate(Robotics.Client.Language.Tags.Tooltip_remark2) : ''));
        assignmentsContainer.append($('<div></div>').attr('class', 'calendarRemark3 remarkInactive').attr('title', cellInfo.IncidenceData.Remark3 ? oCalendar.translator.translate(Robotics.Client.Language.Tags.Tooltip_remark3) : ''));
    }

    return assignmentsContainer;
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.generateEmptyShiftDetailCell = function (isFeastDay, feastDescription, isDateFreeze) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    var shiftInfoContainer = $('<div></div>').attr('class', 'calendarShiftInfoContainer').attr('title', '');

    var textColor = "#FFFFFF";

    var titleLine = $('<div></div>');

    titleLine.append($("<span class='fontBold calendarFeastLeft'></span>").html(''));

    if (isFeastDay) {
        titleLine.append("<div class='calendarFeast calendarFeastRight' title='" + (feastDescription != '' ? feastDescription : oCalendar.translator.translate(Robotics.Client.Language.Tags.Tooltip_feast)) + "'>  </div>");
    }

    if (isDateFreeze) {
        titleLine.append("<div class='calendarLockDate calendarFeastRight' title='" + Globalize.formatMessage('roPeriodInFreezeDate') + "'>  </div>");
    }

    shiftInfoContainer.append(titleLine);
    shiftInfoContainer.attr('style', 'color:' + new Robotics.Client.Common.roHtmlColor().invertCssColor(textColor));

    return shiftInfoContainer;
}

Robotics.Client.Controls.roSchedulerCalendar.prototype.generateShiftDetailCell = function (shiftData, shiftBase, isFeastDay, feastDescription, alerts, isDateFreeze) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    var shiftInfoContainer = $('<div></div>').attr('class', 'calendarShiftInfoContainer').attr('title', shiftData.Name);

    var textColor = shiftData.Color;

    var titleLine = $('<div style="clear:both"></div>');

    var sName = titleLine.append($("<span class='fontBold calendarFeastLeft'></span>").html(shiftData.ShortName));

    if (isFeastDay) {
        sName.append("<div class='calendarFeast calendarFeastRight' title='" + (feastDescription != '' ? feastDescription : oCalendar.translator.translate(Robotics.Client.Language.Tags.Tooltip_feast)) + "'>  </div>");
    }

    if (isDateFreeze) {
        sName.append("<div class='calendarLockDate calendarFeastRight' title='" + Globalize.formatMessage('roPeriodInFreezeDate') + "'>  </div>");
    }

    var tmpOrdinaryHours = 0;
    var tmpComplementaryHours = 0;

    var calcShift = null;
    if (shiftData.Type != Robotics.Client.Constants.ShiftType.Holiday && shiftData.Type != Robotics.Client.Constants.ShiftType.Holiday_NonWorking) {
        calcShift = shiftData;
    } else {
        calcShift = shiftBase;
    }

    if (calcShift.ExistComplementaryData) {
        for (var i = 0; i < calcShift.ShiftLayers; i++) {
            tmpOrdinaryHours += calcShift.ShiftLayersDefinition[i].LayerOrdinaryHours;
            tmpComplementaryHours += calcShift.ShiftLayersDefinition[i].LayerComplementaryHours;
        }
    }

    var ordinaryHours = "";
    var complementaryHours = "";

    if (calcShift.ExistComplementaryData) ordinaryHours = oCalendar.ConvertHoursToHourFormat(tmpOrdinaryHours);
    else ordinaryHours = oCalendar.ConvertHoursToHourFormat(calcShift.PlannedHours);

    if (tmpComplementaryHours > 0) complementaryHours = oCalendar.ConvertHoursToHourFormat(tmpComplementaryHours);

    var resultText = '';

    if (ordinaryHours != '' || tmpComplementaryHours > 0) {
        if (ordinaryHours != '') resultText = ordinaryHours;
        else resultText = oCalendar.ConvertHoursToHourFormat(0);
    }

    if (complementaryHours != '') {
        resultText = resultText + '-' + complementaryHours;
    }

    if (shiftData.Type != Robotics.Client.Constants.ShiftType.Holiday && shiftData.Type != Robotics.Client.Constants.ShiftType.Holiday_NonWorking) {
        shiftInfoContainer.append(titleLine);

        if (typeof shiftData.EndHour == 'undefined') { shiftData.EndHour = moment(shiftData.StartHour).add(shiftData.PlannedHours, 'minutes').toDate(); }
        if (shiftData.PlannedHours > 0) shiftInfoContainer.append($('<div style="clear:both"></div>').append($('<span></span>').attr('class', 'fontSmall calendarFeastLeft').html(moment(shiftData.StartHour).format("HH:mm") + '-' + moment(shiftData.EndHour).format("HH:mm"))));

        if (shiftData.PlannedHours <= 0) resultText = '';
    } else {
        shiftInfoContainer.append(titleLine);
        shiftInfoContainer.append($('<div></div>').append($('<span style="clear:both"></span>').attr('class', 'fontBold fontSmall calendarFeastLeft').html(shiftBase.ShortName)));
        //if (shiftBase.PlannedHours > 0) shiftInfoContainer.append($('<div style="clear:both"></div>').append($('<span></span>').attr('class', 'fontSmall calendarFeastLeft').html(moment(shiftBase.StartHour).format("HH:mm") + '-' + moment(shiftBase.EndHour).format("HH:mm"))));

        if (shiftBase.PlannedHours <= 0) resultText = '';
    }

    if (shiftData.Type != Robotics.Client.Constants.ShiftType.Holiday && shiftData.Type != Robotics.Client.Constants.ShiftType.Holiday_NonWorking && resultText != '') {
        var hours = resultText.split('-');

        for (var z = 0; z < hours.length; z++) {
            var hoursLine = $('<div style="clear:both"></div>');
            var sName = hoursLine.append($('<span></span>').attr('class', 'fontBold'));
            hoursLine.append($('<span style="' + (tmpComplementaryHours > 0 ? 'font-size: 8px !important' : '') + '"></span>').attr('class', 'fontSmall').html((z === 0 ? 'HO:' : 'HC:') + hours[z]));

            shiftInfoContainer.append(hoursLine);
        }
    }

    var alertsText = this.buildAlertsShortText(alerts);
    var absenceTitle = $('<div style="clear:both" class="alertCausesContainer"></div>').append($("<span style='" + (tmpComplementaryHours > 0 ? 'font-size: 9px; margin-top:-1px;' : '') + "' title='" + alertsText + "' class='fontBold calendarFeastLeft'></span>").html(alertsText));
    shiftInfoContainer.append(absenceTitle);

    shiftInfoContainer.attr('style', 'color:' + new Robotics.Client.Common.roHtmlColor().invertCssColor(textColor));

    return shiftInfoContainer;
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.buildAlertsShortText = function (alerts) {
    var alertsShortText = '';
    if (typeof alerts != 'undefined' && alerts != null) {
        if (alerts.OnAbsenceDaysInfo != '') alertsShortText += alerts.OnAbsenceDaysInfo + ';';
        if (alerts.OnAbsenceHoursInfo != '') alertsShortText += alerts.OnAbsenceHoursInfo + ';';
        if (alerts.OnHolidaysHoursInfo != '') alertsShortText += alerts.OnHolidaysHoursInfo + ';';
        if (alerts.OnOvertimesHoursInfo != '') alertsShortText += alerts.OnOvertimesHoursInfo + ';';

        if (alertsShortText != '') alertsShortText = alertsShortText.substring(0, alertsShortText.length - 1);
    }

    return alertsShortText;
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.buildReviewRowView = function (rowIndex, tBodyRow) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    var empData = oCalData.CalendarData[rowIndex].EmployeeData;
    var dayData = oCalData.CalendarData[rowIndex].PeriodData.DayData;

    for (var columnIndex = 0; columnIndex < dayData.length; columnIndex++) {
        tBodyRow.append(this.createReviewCell(empData, dayData[columnIndex], rowIndex, columnIndex));
    }
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.createReviewCell = function (EmployeeData, cellInfo, rowPosition, columnPosition) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    var calendarCell = $('<td></td>');

    if (columnPosition % 2 == 0) {
        calendarCell.addClass('columnDailyCalendarOdd');
    }

    var calendarOuterContent = $('<div></div>').attr('id', oCalendar.ascxPrefix + '_reviewDailyCell_' + rowPosition + '_' + columnPosition).attr('class', 'reviewDailyBodyCell reviewOuterCellV2').attr('data-IDEmployee', EmployeeData.IDEmployee).attr('data-IDGroup', EmployeeData.IDGroup);
    calendarOuterContent.attr('data-IDRow', rowPosition).attr('data-IDColumn', columnPosition).attr('data-Date', moment(cellInfo.PlanDate).format('DD/MM/YYYY'));

    if (oCalendar.loadPunches) this.buildReviewCalendarCellPunches(calendarOuterContent, cellInfo);
    else this.buildReviewCalendarCellContentV2(calendarOuterContent, cellInfo, EmployeeData.FreezingDate);

    return calendarCell.append(calendarOuterContent);
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.buildReviewCalendarCellPunches = function (calendarOuterContent, cellInfo) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    var idRow = calendarOuterContent.attr('data-IDRow');
    var idColumn = calendarOuterContent.attr('data-IDColumn');

    var calendarInnerContent = $('<div></div>').attr('id', oCalendar.ascxPrefix + '_tooltipPunchesReview_' + idRow + '_' + idColumn).attr('class', 'reviewInnerCellV2 punchTooltipTarget');

    var iLines = 0;

    var dayUL = $('<div style="width:200px"></div>');

    if (cellInfo.EmployeeStatusOnDay == Robotics.Client.Constants.EmployeeStatusOnDay.Ok) {
        var divCompliment = $('<div></div>').attr('class', 'bottomReview');

        if (typeof cellInfo.PunchData != 'undefined' && cellInfo.PunchData.length > 0) {
            var divLine = $('<div>').attr('style', 'clear:both;line-height:20px;height:20px;color:#000;overflow:hidden;font-size:10px;border-bottom:1px dashed #cdcdcd;text-align:center');
            var divTooltipLine = $('<div>').attr('style', 'clear:both;line-height:20px;height:20px;color:#000;overflow:hidden;font-size:10px;border-bottom:1px dashed #cdcdcd;text-align:center');
            for (var i = 0; i < cellInfo.PunchData.length; i++) {
                if (i % 2 == 0) {
                    divLine = $('<div>').attr('style', 'clear:both;line-height:20px;height:20px;color:#000;overflow:hidden;font-size:10px;border-bottom:1px dashed #cdcdcd;text-align:center');
                    divTooltipLine = $('<div>').attr('style', 'clear:both;line-height:20px;height:20px;color:#000;overflow:hidden;font-size:10px;border-bottom:1px dashed #cdcdcd;text-align:center');
                }

                divLine.append($('<div>').attr('style', 'float:left;width:50%').html("<span style='font-weight:bold;'>" + (cellInfo.PunchData[i].ActualType == 1 ? "E" : "S") + " </span>" + (moment(cellInfo.PunchData[i].DateTimePunch).format("HH:mm"))));
                divTooltipLine.append($('<div>').attr('style', 'float:left;width:50%').html("<span style='font-weight:bold;'>" + (cellInfo.PunchData[i].ActualType == 1 ? "E" : "S") + " </span>" + (moment(cellInfo.PunchData[i].DateTimePunch).format("HH:mm"))));

                if ((i % 2 != 0) || (i == (cellInfo.PunchData.length - 1))) {
                    if (iLines < 3) divCompliment.append(divLine);
                    dayUL.append(divTooltipLine);
                    iLines++;
                }
            }
        }

        var alertsText = this.buildAlertsShortText(cellInfo.Alerts);
        var absenceTitle = $('<div style="clear:both;position:relative" class="alertCausesContainer"></div>').append($("<span style='' title='" + alertsText + "' class='fontBold calendarFeastLeft'></span>").html(alertsText));
        divCompliment.append(absenceTitle);

        calendarInnerContent.append(divCompliment);
    }

    if (iLines > 0) {
        var hiddenDiv = $('<div></div>').attr("style", "position:absolute; left:-200%");
        var indictmentTooltipContainer = $('<div></div>').attr('id', oCalendar.ascxPrefix + '_tooltipPunchesReviewContainer_' + idRow + '_' + idColumn).attr('class', 'tooltipPunchesReviewContainer');
        var divIndictMentsTooltipTitle = $('<div></div>').html('<span class="fontBold fontBig" style="text-align:left">' + Globalize.formatMessage("roPunchesTitle") + '</span>');

        indictmentTooltipContainer.append(divIndictMentsTooltipTitle, dayUL);
        calendarInnerContent.append(hiddenDiv.append(indictmentTooltipContainer));
    }

    calendarOuterContent.append(calendarInnerContent);
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.buildReviewCalendarCellContentV2 = function (calendarOuterContent, cellInfo, employeeFreezeDate) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    var calendarInnerContent = $('<div></div>').attr('class', 'reviewInnerCellV2');

    if (cellInfo.EmployeeStatusOnDay == Robotics.Client.Constants.EmployeeStatusOnDay.Ok) {
        calendarInnerContent.append(this.generateShiftInfoContainer(cellInfo, calendarOuterContent.attr('data-IDRow'), calendarOuterContent.attr('data-IDColumn'), (moment(cellInfo.PlanDate) <= moment(employeeFreezeDate))));

        if (typeof cellInfo.MainShift != 'undefined') {
            var divCompliment = $('<div></div>').attr('class', 'bottomReview')

            var alertsText = this.buildAlertsShortText(cellInfo.Alerts);
            var absenceTitle = $('<div style="clear:both;position:absolute;padding-left:3px;max-width:90px" class="alertCausesContainer"></div>').append($("<span style='color:black;font-size:12px;height:18px;line-height:18px;padding-top:1px;' title='" + alertsText + "' class='calendarFeastLeft'></span>").html(alertsText));
            divCompliment.append(absenceTitle);

            divCompliment.append($('<div></div>').attr('class', 'reviewColor blueBackground').attr('style', cellInfo.IncidenceData.NormalWork == 0 ? 'display:none;' : '' + 'width:calc(' + cellInfo.IncidenceData.NormalWork + '%);'));
            divCompliment.append($('<div></div>').attr('class', 'reviewColor greenBackground').attr('style', cellInfo.IncidenceData.OverWorking == 0 ? 'display:none;' : '' + 'width:calc(' + cellInfo.IncidenceData.OverWorking + '%);'));
            divCompliment.append($('<div></div>').attr('class', 'reviewColor redBackground').attr('style', cellInfo.IncidenceData.Absence == 0 ? 'display:none;' : '' + 'width:calc(' + cellInfo.IncidenceData.Absence + '%);'));

            calendarInnerContent.append(divCompliment);
        }
    }

    calendarOuterContent.append(calendarInnerContent);
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.buildReviewCalendarCellContent = function (calendarOuterContent, cellInfo) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    var calendarInnercontent = $('<div></div>').attr('class', 'reviewInnerCell');

    if (cellInfo.EmployeeStatusOnDay == Robotics.Client.Constants.EmployeeStatusOnDay.Ok) {
        calendarInnercontent.append($('<div></div>').attr('class', 'reviewColor').attr('style', 'background:blue;width:calc(' + cellInfo.IncidenceData.NormalWork + '%);'));
        calendarInnercontent.append($('<div></div>').attr('class', 'reviewColor').attr('style', 'background:green;width:calc(' + cellInfo.IncidenceData.OverWorking + '%);'));
        calendarInnercontent.append($('<div></div>').attr('class', 'reviewColor').attr('style', 'background:red;width:calc(' + cellInfo.IncidenceData.Absence + '%);'));
    }

    calendarOuterContent.append(calendarInnercontent);
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.updateShiftTotalizerInfo = function (oldShift, newShift, idRow, idColumn, dayData, telecommuteChange) {
    if (!this.resumeInfoLoaded) return;

    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    var dayID = oCalData.CalendarHeader.PeriodHeaderData[idColumn].Row1Text + "_" + oCalData.CalendarHeader.PeriodHeaderData[idColumn].Row2Text;
    dayID = dayID.replace(':', '_');

    var empID = oCalData.CalendarData[idRow].EmployeeData.IDEmployee + "_" + oCalData.CalendarData[idRow].EmployeeData.IDGroup;
    oCalendar.accrualsTotal.PlannedHours.WorkingHours -= oCalendar.employeeDataList[empID].WorkingHours;
    oCalendar.accrualsTotal.PlannedHours.ComplementaryHours -= oCalendar.employeeDataList[empID].ComplementaryHours;
    oCalendar.accrualsTotal.HolidayResume.AssignedHolidays -= oCalendar.employeeDataList[empID].AssignedHolidays;

    if (telecommuteChange) {
        if (typeof (dayData.ZoneName) != 'undefined' && dayData.ZoneName != '') {
            var cellInfo = oCalData.CalendarData[idRow].PeriodData.DayData[idColumn];

            if (moment(cellInfo.PlanDate).isSameOrAfter(moment().startOf("day"))) {
                if ((typeof (cellInfo.MainShift) != 'undefined' && cellInfo.MainShift != null && cellInfo.MainShift.PlannedHours > 0) && cellInfo.IsHoliday == false && cellInfo.Alerts.OnAbsenceDays == false) {
                    oCalendar.columnCapacityList[dayID][dayData.ZoneName].OnTelecommute -= (oCalData.CalendarData[idRow].PeriodData.DayData[idColumn].TelecommutingExpected ? 0 : 1);
                    oCalendar.columnCapacityList[dayID][dayData.ZoneName].Actual += (oCalData.CalendarData[idRow].PeriodData.DayData[idColumn].TelecommutingExpected ? -1 : 1);

                    if (oCalendar.columnCapacityList[dayID][dayData.ZoneName].Actual > oCalendar.columnCapacityList[dayID][dayData.ZoneName].Max && oCalendar.columnCapacityList[dayID][dayData.ZoneName].Max > 0) {
                        oCalendar.columnCapacityList[dayID][dayData.ZoneName].HasChanges = true;
                        oCalendar.capacityError = true;
                    }
                }
            }
        }
    } else {
        var inc = 0;

        if (typeof oldShift != 'undefined' && oldShift != null && typeof newShift != 'undefined') {
            var dDay = null;

            if (newShift == null) {
                inc = -1;
            } else if (oldShift.Type != newShift.Type && newShift.Type == 2) {
                inc = -1;
            } else if ((oldShift.Type != newShift.Type && newShift.Type == 0)) {
                inc = 1
            } else if (oldShift.Type == newShift.Type && (oldShift.PlannedHours > 0 && newShift.PlannedHours == 0)) {
                inc = -1;
            } else if (oldShift.Type == newShift.Type && (oldShift.PlannedHours == 0 && newShift.PlannedHours > 0)) {
                inc = 1
            }

            if (oCalendar.viewRange != Robotics.Client.Constants.ViewRange.Daily) {
                dDay = oCalData.CalendarData[idRow].PeriodData.DayData[idColumn];
            } else {
                dDay = oCalData.CalendarData[idRow].PeriodData.DayData[0];
            }

            if (dDay != null && dDay.TelecommutingExpected == false) {
                if (inc != 0 && typeof (dDay.ZoneName) != 'undefined') {
                    oCalendar.columnCapacityList[dayID][dDay.ZoneName].Actual += inc;

                    if (oCalendar.columnCapacityList[dayID][dDay.ZoneName].Actual > oCalendar.columnCapacityList[dayID][dDay.ZoneName].Max && oCalendar.columnCapacityList[dayID][dDay.ZoneName].Max > 0) {
                        oCalendar.columnCapacityList[dayID][dayData.ZoneName].HasChanges = true;
                        oCalendar.capacityError = true;
                    }
                }
            }
        }

        if (typeof oldShift != 'undefined') {
            oCalendar.columnShiftsList[dayID][oldShift.ShortName].Count -= 1;
            if (typeof oldShift.AssigData != 'undefined' && oldShift.AssigData != null) oCalendar.columnAssignmentsList[dayID][oldShift.AssigData.ID].Count -= (1 * oldShift.AssigData.Cover);
            oCalendar.rowsShiftsList[empID][oldShift.ShortName].Count -= 1;
            if (typeof oldShift.AssigData != 'undefined' && oldShift.AssigData != null) oCalendar.rowsAssignmentsList[empID][oldShift.AssigData.ID].Count -= (1 * oldShift.AssigData.Cover);

            if (oldShift.Type == Robotics.Client.Constants.ShiftType.Holiday || oldShift.Type == Robotics.Client.Constants.ShiftType.Holiday_NonWorking) {
                oCalendar.employeeDataList[empID].AssignedHolidays -= 1;
            } else {
                oCalendar.employeeDataList[empID].WorkingHours -= oldShift.PlannedHours;
                oCalendar.employeeDataList[empID].PlannedHours.YearTotal -= (oldShift.PlannedHours / 60);
            }

            if (oldShift.ExistComplementaryData) {
                for (var tmpCompIndex = 0; tmpCompIndex < oldShift.ShiftLayers; tmpCompIndex++) {
                    if (oldShift.ShiftLayersDefinition[tmpCompIndex].LayerComplementaryHours != -1) oCalendar.employeeDataList[empID].ComplementaryHours -= (oldShift.ShiftLayersDefinition[tmpCompIndex].LayerComplementaryHours);
                }
            }
        }

        if (typeof newShift != 'undefined' && newShift != null) {
            if (typeof (oCalendar.shiftsList[newShift.ShortName]) == 'undefined') oCalendar.shiftsList[newShift.ShortName] = { Name: newShift.Name, ShortName: newShift.ShortName, IsHoliday: (newShift.Type != Robotics.Client.Constants.ShiftType.Holiday && newShift.Type != Robotics.Client.Constants.ShiftType.Holiday_NonWorking ? false : true) };
            if (typeof (oCalendar.columnShiftsList[dayID][newShift.ShortName]) == 'undefined') oCalendar.columnShiftsList[dayID][newShift.ShortName] = { Name: newShift.Name, ShortName: newShift.ShortName, Count: 0 };

            oCalendar.columnShiftsList[dayID][newShift.ShortName].Count += 1;

            if (typeof newShift.AssigData != 'undefined' && newShift.AssigData != null) {
                if (typeof (oCalendar.assignmentsList[newShift.AssigData.ID]) == 'undefined') oCalendar.assignmentsList[newShift.AssigData.ID] = { ID: newShift.AssigData.ID, Name: newShift.AssigData.Name, ShortName: newShift.AssigData.ShortName };
                oCalendar.columnAssignmentsList[dayID][newShift.AssigData.ID].Count += (1 * newShift.AssigData.Cover);
            }

            if (typeof (oCalendar.rowsShiftsList[empID][newShift.ShortName]) == 'undefined') oCalendar.rowsShiftsList[empID][newShift.ShortName] = { Name: newShift.Name, ShortName: newShift.ShortName, Count: 0 };
            oCalendar.rowsShiftsList[empID][newShift.ShortName].Count += 1;

            if (typeof newShift.AssigData != 'undefined' && newShift.AssigData != null) {
                if (typeof (oCalendar.rowsAssignmentsList[empID][newShift.AssigData.ID]) == 'undefined') oCalendar.rowsAssignmentsList[empID][newShift.AssigData.ID] = { ID: newShift.AssigData.ID, Name: newShift.AssigData.Name, ShortName: newShift.AssigData.ShortName, Count: 0 };
                oCalendar.rowsAssignmentsList[empID][newShift.AssigData.ID].Count += (1 * newShift.AssigData.Cover);
            }

            if (newShift.Type == Robotics.Client.Constants.ShiftType.Holiday || newShift.Type == Robotics.Client.Constants.ShiftType.Holiday_NonWorking) {
                oCalendar.employeeDataList[empID].AssignedHolidays += 1;
            } else {
                oCalendar.employeeDataList[empID].WorkingHours += newShift.PlannedHours;
                oCalendar.employeeDataList[empID].PlannedHours.YearTotal += (newShift.PlannedHours / 60);
            }

            if (newShift.ExistComplementaryData) {
                for (var tmpCompIndex = 0; tmpCompIndex < newShift.ShiftLayers; tmpCompIndex++) {
                    if (newShift.ShiftLayersDefinition[tmpCompIndex].LayerComplementaryHours != -1) oCalendar.employeeDataList[empID].ComplementaryHours += (newShift.ShiftLayersDefinition[tmpCompIndex].LayerComplementaryHours);
                }
            }
        }

        oCalendar.accrualsTotal.PlannedHours.WorkingHours += oCalendar.employeeDataList[empID].WorkingHours;
        oCalendar.accrualsTotal.PlannedHours.ComplementaryHours += oCalendar.employeeDataList[empID].ComplementaryHours;
        oCalendar.accrualsTotal.HolidayResume.AssignedHolidays += oCalendar.employeeDataList[empID].AssignedHolidays;
    }
}

Robotics.Client.Controls.roSchedulerCalendar.prototype.generateShiftTotalizerInfo = function () {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    oCalendar.shiftsList = {};
    oCalendar.capacityList = {};
    oCalendar.assignmentsList = {};
    oCalendar.columnShiftsList = {};
    oCalendar.columnCapacityList = {};
    oCalendar.rowsShiftsList = {};
    oCalendar.employeeDataList = {};
    oCalendar.columnAssignmentsList = {}
    oCalendar.rowsAssignmentsList = {}

    if (typeof (oCalendar.assignmentsList[0]) == 'undefined') oCalendar.assignmentsList[0] = { ID: 0, Name: oCalendar.translator.translate(Robotics.Client.Language.Tags.Empty_Assignment), ShortName: oCalendar.translator.translate(Robotics.Client.Language.Tags.Empty_Assignment_SN) };

    var allCapacities = [].concat.apply([], oCalData.CalendarHeader.PeriodSeatingCapacityData.map(function (x) { return x.Capacities }));
    var uniqueCapacities = [...new Map(allCapacities.map(item => [item['ZoneName'], item])).values()];

    for (var capIndex = 0; capIndex < oCalData.CalendarHeader.PeriodSeatingCapacityData.length; capIndex++) {
        var dayID = oCalData.CalendarHeader.PeriodHeaderData[capIndex].Row1Text + "_" + oCalData.CalendarHeader.PeriodHeaderData[capIndex].Row2Text;
        dayID = dayID.replace(':', '_');

        var workCenters = oCalData.CalendarHeader.PeriodSeatingCapacityData[capIndex].Capacities;

        var capObj = {};
        for (var workCenterIndex = 0; workCenterIndex < workCenters.length; workCenterIndex++) {
            if (typeof (capObj[workCenters[workCenterIndex].ZoneName]) == 'undefined') capObj[workCenters[workCenterIndex].ZoneName] = { Actual: workCenters[workCenterIndex].CurrentSeating, Max: workCenters[workCenterIndex].MaxSeatingCapacity };
        }

        for (var unCapacitiesIndex = 0; unCapacitiesIndex < uniqueCapacities.length; unCapacitiesIndex++) {
            var wcName = uniqueCapacities[unCapacitiesIndex].ZoneName;

            if (typeof (capObj[wcName]) == 'undefined') capObj[wcName] = { Actual: uniqueCapacities[unCapacitiesIndex].CurrentSeating, Max: uniqueCapacities[unCapacitiesIndex].MaxSeatingCapacity };
        }

        if (typeof (oCalendar.capacityList[dayID]) == 'undefined') oCalendar.capacityList[dayID] = capObj;
    }

    for (var columnIndex = 0; columnIndex < oCalData.CalendarHeader.PeriodHeaderData.length; columnIndex++) {
        var dayID = oCalData.CalendarHeader.PeriodHeaderData[columnIndex].Row1Text + "_" + oCalData.CalendarHeader.PeriodHeaderData[columnIndex].Row2Text;
        dayID = dayID.replace(':', '_');

        if (typeof (oCalendar.columnShiftsList[dayID]) == 'undefined') oCalendar.columnShiftsList[dayID] = {};
        if (typeof (oCalendar.columnCapacityList[dayID]) == 'undefined') oCalendar.columnCapacityList[dayID] = {};
        if (typeof (oCalendar.columnAssignmentsList[dayID]) == 'undefined') oCalendar.columnAssignmentsList[dayID] = {};

        for (var unCapacitiesIndex = 0; unCapacitiesIndex < uniqueCapacities.length; unCapacitiesIndex++) {
            var wcName = uniqueCapacities[unCapacitiesIndex].ZoneName;
            if (typeof (oCalendar.columnCapacityList[dayID][wcName]) == 'undefined') oCalendar.columnCapacityList[dayID][wcName] = { Actual: oCalendar.capacityList[dayID][wcName].Actual, Max: oCalendar.capacityList[dayID][wcName].Max, OnTelecommute: 0, HasChanges: false };
        }

        if ((oCalendar.viewRange == Robotics.Client.Constants.ViewRange.Daily && columnIndex == 0) || oCalendar.viewRange == Robotics.Client.Constants.ViewRange.Period) {
            if (typeof oCalData.CalendarHeader.PeriodCoverageData != 'undefined' && oCalData.CalendarHeader.PeriodCoverageData.length > 0 && typeof oCalData.CalendarHeader.PeriodCoverageData[columnIndex].AssignmentData != 'undefined' && oCalData.CalendarHeader.PeriodCoverageData[columnIndex].AssignmentData.length > 0) {
                if (typeof (oCalendar.columnAssignmentsList[dayID][0]) == 'undefined') oCalendar.columnAssignmentsList[dayID][0] = { ID: 0, Name: oCalendar.translator.translate(Robotics.Client.Language.Tags.Empty_Assignment), ShortName: oCalendar.translator.translate(Robotics.Client.Language.Tags.Empty_Assignment_SN), Count: 0 };

                for (var z = 0; z < oCalData.CalendarHeader.PeriodCoverageData[columnIndex].AssignmentData.length; z++) {
                    var curCoverage = oCalData.CalendarHeader.PeriodCoverageData[columnIndex].AssignmentData[z];

                    if (typeof (oCalendar.columnAssignmentsList[dayID][curCoverage.ID]) == 'undefined') oCalendar.columnAssignmentsList[dayID][curCoverage.ID] = { ID: curCoverage.ID, Name: curCoverage.Name, ShortName: curCoverage.ShortName, Count: 0, Expected: (curCoverage.Expected * -1) };
                    if (typeof (oCalendar.assignmentsList[curCoverage.ID]) == 'undefined') oCalendar.assignmentsList[curCoverage.ID] = { ID: curCoverage.ID, Name: curCoverage.Name, ShortName: curCoverage.ShortName };
                }
            }
        }

        for (var rowIndex = 0; rowIndex < oCalData.CalendarData.length; rowIndex++) {
            if (oCalendar.viewRange == Robotics.Client.Constants.ViewRange.Period) {
                var dayData = oCalData.CalendarData[rowIndex].PeriodData.DayData[columnIndex];
                if (dayData != null && dayData.EmployeeStatusOnDay == Robotics.Client.Constants.EmployeeStatusOnDay.Ok) {
                    if (typeof (dayData.ZoneName) != 'undefined' && dayData.ZoneName != '') oCalendar.columnCapacityList[dayID][dayData.ZoneName].OnTelecommute += (dayData.TelecommutingExpected ? 1 : 0);

                    var usedShift = this.getAssignedShift(dayData);
                    if (usedShift != null) {
                        if (typeof (oCalendar.columnShiftsList[dayID][usedShift.ShortName]) == 'undefined') oCalendar.columnShiftsList[dayID][usedShift.ShortName] = { Name: usedShift.Name, ShortName: usedShift.ShortName, Count: 0, OnAbsence: 0 };
                        if (typeof (oCalendar.shiftsList[usedShift.ShortName]) == 'undefined') oCalendar.shiftsList[usedShift.ShortName] = { Name: usedShift.Name, ShortName: usedShift.ShortName, IsHoliday: (usedShift.Type != Robotics.Client.Constants.ShiftType.Holiday && usedShift.Type != Robotics.Client.Constants.ShiftType.Holiday_NonWorking ? false : true) };

                        oCalendar.columnShiftsList[dayID][usedShift.ShortName].Count += 1;
                        if (dayData.Alerts.OnAbsenceDays) oCalendar.columnShiftsList[dayID][usedShift.ShortName].OnAbsence += 1;

                        if (typeof dayData.AssigData != 'undefined' && dayData.AssigData != null) {
                            if (typeof (oCalendar.columnAssignmentsList[dayID][dayData.AssigData.ID]) == 'undefined') oCalendar.columnAssignmentsList[dayID][dayData.AssigData.ID] = { ID: dayData.AssigData.ID, Name: dayData.AssigData.Name, ShortName: dayData.AssigData.ShortName, Count: 0 };
                            if (typeof (oCalendar.assignmentsList[dayData.AssigData.ID]) == 'undefined') oCalendar.assignmentsList[dayData.AssigData.ID] = { ID: dayData.AssigData.ID, Name: dayData.AssigData.Name, ShortName: dayData.AssigData.ShortName };

                            if (dayData.Alerts.OnAbsenceDays == false && usedShift.Type != Robotics.Client.Constants.ShiftType.Holiday && usedShift.Type != Robotics.Client.Constants.ShiftType.Holiday_NonWorking) {
                                oCalendar.columnAssignmentsList[dayID][dayData.AssigData.ID].Count += (1 * dayData.AssigData.Cover);
                                if (typeof oCalendar.columnAssignmentsList[dayID][dayData.AssigData.ID].Expected != 'undefined') oCalendar.columnAssignmentsList[dayID][dayData.AssigData.ID].Expected += (1 * dayData.AssigData.Cover);
                            }
                        } else {
                            if (typeof (oCalendar.columnAssignmentsList[dayID][0]) == 'undefined') oCalendar.columnAssignmentsList[dayID][0] = { ID: 0, Name: oCalendar.translator.translate(Robotics.Client.Language.Tags.Empty_Assignment), ShortName: oCalendar.translator.translate(Robotics.Client.Language.Tags.Empty_Assignment_SN), Count: 0 };

                            if (dayData.Alerts.OnAbsenceDays == false && usedShift.Type != Robotics.Client.Constants.ShiftType.Holiday && usedShift.Type != Robotics.Client.Constants.ShiftType.Holiday_NonWorking) {
                                oCalendar.columnAssignmentsList[dayID][0].Count += 1;
                            }
                        }
                    }
                }
            } else {
                var dayData = oCalData.CalendarData[rowIndex].PeriodData.DayData[0];
                var usedShift = this.getAssignedShift(dayData);
                var hourInfo = oCalData.CalendarData[rowIndex].PeriodData.DayData[0].HourData[columnIndex];
                if (typeof (dayData.ZoneName) != 'undefined') oCalendar.columnCapacityList[dayID][dayData.ZoneName].OnTelecommute += (dayData.TelecommutingExpected ? 1 : 0);

                if (hourInfo.DailyHourType != Robotics.Client.Constants.DailyHourType.Untyped && usedShift != null) {
                    if (typeof (oCalendar.columnShiftsList[dayID][usedShift.ShortName]) == 'undefined') oCalendar.columnShiftsList[dayID][usedShift.ShortName] = { Name: usedShift.Name, ShortName: usedShift.ShortName, Count: 0, OnAbsence: 0 };
                    if (typeof (oCalendar.shiftsList[usedShift.ShortName]) == 'undefined') oCalendar.shiftsList[usedShift.ShortName] = { Name: usedShift.Name, ShortName: usedShift.ShortName, IsHoliday: (usedShift.Type != Robotics.Client.Constants.ShiftType.Holiday && usedShift.Type != Robotics.Client.Constants.ShiftType.Holiday_NonWorking ? false : true) };

                    oCalendar.columnShiftsList[dayID][usedShift.ShortName].Count += 1;
                    if (dayData.Alerts.OnAbsenceDays) oCalendar.columnShiftsList[dayID][usedShift.ShortName].OnAbsence += 1;

                    if (typeof dayData.AssigData != 'undefined' && dayData.AssigData != null) {
                        if (typeof (oCalendar.columnAssignmentsList[dayID][dayData.AssigData.ID]) == 'undefined') oCalendar.columnAssignmentsList[dayID][dayData.AssigData.ID] = { ID: dayData.AssigData.ID, Name: dayData.AssigData.Name, ShortName: dayData.AssigData.ShortName, Count: 0 };
                        if (typeof (oCalendar.assignmentsList[dayData.AssigData.ID]) == 'undefined') oCalendar.assignmentsList[dayData.AssigData.ID] = { ID: dayData.AssigData.ID, Name: dayData.AssigData.Name, ShortName: dayData.AssigData.ShortName };

                        if (dayData.Alerts.OnAbsenceDays == false && usedShift.Type != Robotics.Client.Constants.ShiftType.Holiday && usedShift.Type != Robotics.Client.Constants.ShiftType.Holiday_NonWorking) {
                            oCalendar.columnAssignmentsList[dayID][dayData.AssigData.ID].Count += (1 * dayData.AssigData.Cover);
                            if (typeof oCalendar.columnAssignmentsList[dayID][dayData.AssigData.ID].Expected != 'undefined') oCalendar.columnAssignmentsList[dayID][dayData.AssigData.ID].Expected += (1 * dayData.AssigData.Cover);
                        } else {
                            if (typeof (oCalendar.columnAssignmentsList[dayID][0]) == 'undefined') oCalendar.columnAssignmentsList[dayID][0] = { ID: 0, Name: oCalendar.translator.translate(Robotics.Client.Language.Tags.Empty_Assignment), ShortName: oCalendar.translator.translate(Robotics.Client.Language.Tags.Empty_Assignment_SN), Count: 0 };

                            if (dayData.Alerts.OnAbsenceDays == false && usedShift.Type != Robotics.Client.Constants.ShiftType.Holiday && usedShift.Type != Robotics.Client.Constants.ShiftType.Holiday_NonWorking) {
                                oCalendar.columnAssignmentsList[dayID][0].Count += 1;
                            }
                        }
                    }
                }
            }
        }
    }

    for (var rowIndex = 0; rowIndex < oCalData.CalendarData.length; rowIndex++) {
        var empID = oCalData.CalendarData[rowIndex].EmployeeData.IDEmployee + "_" + oCalData.CalendarData[rowIndex].EmployeeData.IDGroup;

        if (typeof (oCalendar.rowsShiftsList[empID]) == 'undefined') oCalendar.rowsShiftsList[empID] = {};
        if (typeof (oCalendar.rowsAssignmentsList[empID]) == 'undefined') oCalendar.rowsAssignmentsList[empID] = {};

        if (typeof (oCalendar.employeeDataList[empID]) == 'undefined') {
            oCalendar.employeeDataList[empID] = {
                InitialHolidays: oCalData.CalendarData[rowIndex].SummaryData.AccrualHolidays, AssignedHolidays: 0, WorkingHours: 0, AbsenceHours: 0, ComplementaryHours: 0, PlannedHours: Object.clone(oCalData.CalendarData[rowIndex].SummaryData.PlannedHours, true), HolidayResume: Object.clone(oCalData.CalendarData[rowIndex].SummaryData.HolidayResume, true)
            };
        } else {
            oCalendar.employeeDataList[empID].PlannedHours = Object.clone(oCalData.CalendarData[rowIndex].SummaryData.PlannedHours, true);
            oCalendar.employeeDataList[empID].HolidayResume = Object.clone(oCalData.CalendarData[rowIndex].SummaryData.HolidayResume, true);
        }

        for (var columnIndex = 0; columnIndex < oCalData.CalendarHeader.PeriodHeaderData.length; columnIndex++) {
            var dayData = oCalData.CalendarData[rowIndex].PeriodData.DayData[columnIndex];
            if (dayData != null && dayData.EmployeeStatusOnDay == Robotics.Client.Constants.EmployeeStatusOnDay.Ok) {
                var usedShift = this.getAssignedShift(dayData);
                if (usedShift != null) {
                    if (typeof (oCalendar.rowsShiftsList[empID][usedShift.ShortName]) == 'undefined') oCalendar.rowsShiftsList[empID][usedShift.ShortName] = { Name: usedShift.Name, ShortName: usedShift.ShortName, Count: 0 };
                    if (typeof (oCalendar.shiftsList[usedShift.ShortName]) == 'undefined') oCalendar.shiftsList[usedShift.ShortName] = { Name: usedShift.Name, ShortName: usedShift.ShortName, IsHoliday: (usedShift.Type != Robotics.Client.Constants.ShiftType.Holiday && usedShift.Type != Robotics.Client.Constants.ShiftType.Holiday_NonWorking ? false : true) };

                    oCalendar.rowsShiftsList[empID][usedShift.ShortName].Count += 1;

                    if (usedShift.Type == Robotics.Client.Constants.ShiftType.Holiday || usedShift.Type == Robotics.Client.Constants.ShiftType.Holiday_NonWorking || dayData.Alerts.OnAbsenceDays) {
                        if (usedShift.Type == Robotics.Client.Constants.ShiftType.Holiday || usedShift.Type == Robotics.Client.Constants.ShiftType.Holiday_NonWorking) {
                            oCalendar.employeeDataList[empID].AssignedHolidays += 1;
                            //oCalendar.employeeDataList[empID].AbsenceHours += dayData.ShiftBase.PlannedHours;
                        } else {
                            oCalendar.employeeDataList[empID].AbsenceHours += usedShift.PlannedHours;
                        }
                    } else {
                        oCalendar.employeeDataList[empID].WorkingHours += usedShift.PlannedHours;
                        if (usedShift.ExistComplementaryData) {
                            for (var tmpCompIndex = 0; tmpCompIndex < usedShift.ShiftLayers; tmpCompIndex++) {
                                if (usedShift.ShiftLayersDefinition[tmpCompIndex].LayerComplementaryHours != -1) oCalendar.employeeDataList[empID].ComplementaryHours += (usedShift.ShiftLayersDefinition[tmpCompIndex].LayerComplementaryHours);
                            }
                        }
                    }
                }

                if (typeof dayData.AssigData != 'undefined' && dayData.AssigData != null) {
                    if (typeof (oCalendar.rowsAssignmentsList[empID][dayData.AssigData.ID]) == 'undefined') oCalendar.rowsAssignmentsList[empID][dayData.AssigData.ID] = { ID: dayData.AssigData.ID, Name: dayData.AssigData.Name, ShortName: dayData.AssigData.ShortName, Count: 0 };
                    if (typeof (oCalendar.assignmentsList[dayData.AssigData.ID]) == 'undefined') oCalendar.assignmentsList[dayData.AssigData.ID] = { ID: dayData.AssigData.ID, Name: dayData.AssigData.Name, ShortName: dayData.AssigData.ShortName };

                    if (typeof dayData.Alerts != 'undefined' && dayData.Alerts.OnAbsenceDays == false && usedShift != null && usedShift.Type != Robotics.Client.Constants.ShiftType.Holiday && usedShift.Type != Robotics.Client.Constants.ShiftType.Holiday_NonWorking) {
                        oCalendar.rowsAssignmentsList[empID][dayData.AssigData.ID].Count += (1 * dayData.AssigData.Cover);
                    }
                } else {
                    if (typeof (oCalendar.rowsAssignmentsList[empID][0]) == 'undefined') oCalendar.rowsAssignmentsList[empID][0] = { ID: 0, Name: oCalendar.translator.translate(Robotics.Client.Language.Tags.Empty_Assignment), ShortName: oCalendar.translator.translate(Robotics.Client.Language.Tags.Empty_Assignment_SN), Count: 0 };

                    if (typeof dayData.Alerts != 'undefined' && dayData.Alerts.OnAbsenceDays == false && usedShift != null && usedShift.Type != Robotics.Client.Constants.ShiftType.Holiday && usedShift.Type != Robotics.Client.Constants.ShiftType.Holiday_NonWorking) {
                        oCalendar.rowsAssignmentsList[empID][0].Count += 1;
                    }
                }
            }
        }
        oCalendar.employeeDataList[empID].PlannedHours.YearTotal = oCalendar.employeeDataList[empID].PlannedHours.YearTotal + (oCalendar.employeeDataList[empID].WorkingHours / 60);
    }

    oCalendar.accrualsTotal.PlannedHours.WorkingHours = 0;
    oCalendar.accrualsTotal.PlannedHours.ComplementaryHours = 0;
    oCalendar.accrualsTotal.HolidayResume.AssignedHolidays = 0;

    Object.keys(oCalendar.employeeDataList).forEach(function (empKey, index) {
        oCalendar.accrualsTotal.PlannedHours.WorkingHours += oCalendar.employeeDataList[empKey].WorkingHours;
        oCalendar.accrualsTotal.PlannedHours.ComplementaryHours += oCalendar.employeeDataList[empKey].ComplementaryHours;
        oCalendar.accrualsTotal.HolidayResume.AssignedHolidays += oCalendar.employeeDataList[empKey].AssignedHolidays;
    });

    this.resumeInfoLoaded = true;
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.createAcrrualTotalsTable = function (idTable, parentId) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    var tableContainer = $('#' + oCalendar.prefix + parentId);
    tableContainer.empty();

    var tableElement = $('<table></table>').attr('id', oCalendar.prefix + idTable).attr('class', 'fancyColumnTable');

    //Necesitamos crear un header sin altura para poder utilizar el plugin de sticky headers y columns
    tableElement.append(this.createAccrualsTotalTableHeader(idTable, parentId));

    tableElement.append(this.createAccrualsTotalsTableBody(idTable, parentId));

    tableElement.append(this.createAccrualsTotalTableFooter(idTable, parentId));

    tableContainer.append(tableElement);
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.createAccrualsTotalTableHeader = function (idTable, parentId) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    //Creamos el header
    var tHeader = $('<thead></thead>').attr('id', oCalendar.prefix + idTable + '_thead');

    var tHeaderRow = $('<tr></tr>');

    var accrualCell = $('<th></th>');
    var accrualContentCell = $('<div></div>').attr('class', 'CalendarDayFixed CalendarRowFixedHeader');
    accrualContentCell.append($('<div></div>').attr('class', 'NorthRowCell').html(oCalendar.translator.translate(Robotics.Client.Language.Tags.PlannedHours)));

    var holidaysCell = $('<th></th>');
    var holidaysContentCell = $('<div></div>').attr('class', 'CalendarDayFixed CalendarRowFixedHeader');
    holidaysContentCell.append($('<div></div>').attr('class', 'NorthRowCell').html(oCalendar.translator.translate(Robotics.Client.Language.Tags.Holidays)));

    tHeaderRow.append(accrualCell.append(accrualContentCell));
    tHeaderRow.append(holidaysCell.append(holidaysContentCell));

    tHeader.append(tHeaderRow);

    return tHeader;
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.createAccrualsTotalsTableBody = function (idTable, parentId) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    //Creamos el body
    var tBody = $('<tbody></tbody>').attr('id', oCalendar.prefix + idTable + '_tbody');

    var tBodyRow = $('<tr></tr>').attr('style', 'color: #333333;');

    var accrualCell = $('<td></td>');
    var accrualContentCell = $('<div></div>').attr('class', 'ColumnAccrualsTableBodyCell');

    var accrualInfoCell = $('<div></div>').attr('class', 'ContentAccrualsRowCell');
    accrualInfoCell.append($('<div></div>').attr('class', 'totalAccrual').html(oCalendar.ConvertHoursToHourFormat(oCalendar.accrualsTotal.PlannedHours.WorkingHours)));

    if (oCalendar.accrualsTotal.PlannedHours.ComplementaryHours > 0) {
        var divComplementary = $('<div></div>');
        divComplementary.html('(HO:' + oCalendar.ConvertHoursToHourFormat(oCalendar.accrualsTotal.PlannedHours.WorkingHours - oCalendar.accrualsTotal.PlannedHours.ComplementaryHours) + ' HC:' + oCalendar.ConvertHoursToHourFormat(oCalendar.accrualsTotal.PlannedHours.ComplementaryHours) + ')');
        accrualInfoCell.append(divComplementary);
    }
    accrualContentCell.append(accrualInfoCell);

    var holidaysCell = $('<td></td>');
    var holidaysContentCell = $('<div></div>').attr('class', 'ColumnAccrualsTableBodyCell');

    var holidayInfoDiv = $('<div></div>').attr('class', 'ContentAccrualsRowCell');
    holidayInfoDiv.append($('<div></div>').attr('class', 'totalAccrual').html(oCalendar.accrualsTotal.HolidayResume.AssignedHolidays));

    holidaysContentCell.append(holidayInfoDiv);

    tBodyRow.append(accrualCell.append(accrualContentCell));
    tBodyRow.append(holidaysCell.append(holidaysContentCell));

    tmpCell = $('<td style="width:100%"></td>').html('&nbsp;');
    tBodyRow.append(tmpCell);

    tBody.append(tBodyRow);

    return tBody;
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.createAccrualsTotalTableFooter = function (idTable, parentId) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    //Creamos el header
    var tFoot = $('<tfoot></tfoot>').attr('id', oCalendar.prefix + idTable + '_tfoot');

    var tFootRow = $('<tr></tr>');

    var accrualCell = $('<td></td>');
    var accrualContentCell = $('<div></div>').attr('class', 'CalendarDayFixed CalendarRowFixedHeader');
    accrualContentCell.append($('<div></div>').attr('class', 'SouthRowCell').html(oCalendar.translator.translate(Robotics.Client.Language.Tags.PlannedHours)));

    var holidaysCell = $('<td></td>');
    var holidaysContentCell = $('<div></div>').attr('class', 'CalendarDayFixed CalendarRowFixedHeader');
    holidaysContentCell.append($('<div></div>').attr('class', 'SouthRowCell').html(oCalendar.translator.translate(Robotics.Client.Language.Tags.Holidays)));

    tFootRow.append(accrualCell.append(accrualContentCell));
    tFootRow.append(holidaysCell.append(holidaysContentCell));

    tFoot.append(tFootRow);

    return tFoot;
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.createColumnShiftsTable = function (idTable, parentId) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    var tableContainer = $('#' + oCalendar.prefix + parentId);
    tableContainer.empty();

    var tableElement = $('<table></table>').attr('id', oCalendar.prefix + idTable).attr('class', 'fancyColumnTable');

    //Necesitamos crear un header sin altura para poder utilizar el plugin de sticky headers y columns
    tableElement.append(this.createEmptyColumnTableHeader(idTable, parentId));

    tableElement.append(this.createColumnShiftsTableBody(idTable, parentId));

    tableElement.append(this.createColumnShiftsTableFooter(idTable, parentId));

    tableContainer.append(tableElement);
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.createEmptyColumnTableHeader = function (idTable, parentId) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    //Creamos el header
    var tHeader = $('<thead></thead>').attr('id', oCalendar.prefix + idTable + '_thead');

    var tHeaderRow = $('<tr></tr>');

    //Creamos la primera columna que sera el header
    var tFixedHeaderCell = $('<th></th>');
    var mainFixedHeaderDiv = $('<div></div>').attr('class', 'CalendarFixed CalendarFixedHeader');
    var objHeaderCell = $('<div></div>').attr('class', 'CalendarEmployeeFixed CalendarEmployeeFixedHeader').attr('style', 'height:0px');

    tHeaderRow.append(tFixedHeaderCell.append(mainFixedHeaderDiv.append(objHeaderCell)));

    if (oCalendar.viewRange == Robotics.Client.Constants.ViewRange.Daily) {
        for (var x = oCalendar.getMinDailyCell(); x < oCalendar.getMaxDailyCell(); x++) {
            //for (var x = 0; x < oCalData.CalendarHeader.PeriodHeaderData.length; x++) {
            var dayHeaderCell = $('<th></th>');

            var mainDayHeaderCell = $('<div></div>').attr('class', 'CalendarDayFixed CalendarDayFixedHeader DailyCell').attr('style', 'height:0px');

            tHeaderRow.append(dayHeaderCell.append(mainDayHeaderCell));
        }
    } else {
        for (var i = 0; i < oCalData.CalendarHeader.PeriodHeaderData.length; i++) {
            var dayHeaderCell = $('<th></th>');

            var mainDayHeaderCell = $('<div></div>').attr('class', 'CalendarDayFixed CalendarDayFixedHeader').attr('style', 'height:0px');

            tHeaderRow.append(dayHeaderCell.append(mainDayHeaderCell));
        }
    }

    var tmpCell = $('<th style="width:100%"></th>').attr('style', 'height:0px');
    tHeaderRow.append(tmpCell);
    tHeader.append(tHeaderRow);

    return tHeader;
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.createColumnShiftsTableBody = function (idTable, parentId) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    //Creamos el body
    var tBody = $('<tbody></tbody>').attr('id', oCalendar.prefix + idTable + '_tbody');

    var oTmpCal = oCalendar;
    Object.keys(oTmpCal.shiftsList).forEach(function (shiftKey, index) {
        var tBodyRow = $('<tr></tr>').attr('class', 'shiftColumnRow');

        var tFixedBodyCell = $('<td></td>');

        var mainFixedBodyCell = $('<div></div>').attr('class', 'CalendarShiftsFixed CalendarShiftFixedBody');

        mainFixedBodyCell.append($('<div></div>').attr('class', 'CalendarShiftContentFixedBody').html('<span style="font-weight:bold">' + shiftKey + "</span>:   " + oTmpCal.shiftsList[shiftKey].Name));

        tBodyRow.append(tFixedBodyCell.append(mainFixedBodyCell));

        var cellID = null;
        var cellValue = 0;
        var dayBodyCell = null;
        var columnContentCell = null;

        if (oTmpCal.viewRange == Robotics.Client.Constants.ViewRange.Daily) {
            for (var x = oTmpCal.getMinDailyCell(); x < oTmpCal.getMaxDailyCell(); x++) {
                //for (var x = 0; x < oTmpCal.oCalendar.CalendarHeader.PeriodHeaderData.length; x++) {
                cellID = oTmpCal.oCalendar.CalendarHeader.PeriodHeaderData[x].Row1Text + "_" + oTmpCal.oCalendar.CalendarHeader.PeriodHeaderData[x].Row2Text;
                cellID = cellID.replace(':', '_');
                cellValue = 0;
                if (typeof (oTmpCal.columnShiftsList[cellID][oTmpCal.shiftsList[shiftKey].ShortName]) != 'undefined') cellValue = oTmpCal.columnShiftsList[cellID][oTmpCal.shiftsList[shiftKey].ShortName].Count;

                dayBodyCell = $('<td></td>');
                columnContentCell = $('<div></div>').attr('class', 'ColumnShiftTableBodyCell DailyCell');
                columnContentCell.append($('<div></div>').attr('class', 'ContentShiftsColumnCell').html(cellValue));

                tBodyRow.append(dayBodyCell.append(columnContentCell));
            }
        } else {
            for (var x = 0; x < oTmpCal.oCalendar.CalendarHeader.PeriodHeaderData.length; x++) {
                cellID = oTmpCal.oCalendar.CalendarHeader.PeriodHeaderData[x].Row1Text + "_" + oTmpCal.oCalendar.CalendarHeader.PeriodHeaderData[x].Row2Text;
                cellID = cellID.replace(':', '_');
                cellValue = 0;
                if (typeof (oTmpCal.columnShiftsList[cellID][oTmpCal.shiftsList[shiftKey].ShortName]) != 'undefined') cellValue = oTmpCal.columnShiftsList[cellID][oTmpCal.shiftsList[shiftKey].ShortName].Count;

                dayBodyCell = $('<td></td>');
                columnContentCell = $('<div></div>').attr('class', 'ColumnShiftTableBodyCell');
                columnContentCell.append($('<div></div>').attr('class', 'ContentShiftsColumnCell').html(cellValue));

                tBodyRow.append(dayBodyCell.append(columnContentCell));
            }
        }

        tmpCell = $('<td style="width:100%"></td>').html('&nbsp;');
        tBodyRow.append(tmpCell);

        tBody.append(tBodyRow);
    });

    return tBody;
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.createColumnShiftsTableFooter = function (idTable, parentId) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    //Creamos el body
    var tBody = $('<tfoot></tfoot>').attr('id', oCalendar.prefix + idTable + '_tfoot');

    var oTmpCal = oCalendar;

    var tBodyRow = $('<tr></tr>').attr('class', 'shiftColumnRow');

    var tFixedBodyCell = $('<td></td>');

    var mainFixedBodyCell = $('<div></div>').attr('class', 'CalendarShiftsFixed CalendarShiftFixedBody');

    mainFixedBodyCell.append($('<div></div>').attr('class', 'CalendarShiftContentFixedBody').html('&nbsp;'));

    tBodyRow.append(tFixedBodyCell.append(mainFixedBodyCell));

    var cellID = null;
    var cellValue = 0;
    var dayBodyCell = null;
    var columnContentCell = null;

    if (oTmpCal.viewRange == Robotics.Client.Constants.ViewRange.Daily) {
        for (var x = oTmpCal.getMinDailyCell(); x < oTmpCal.getMaxDailyCell(); x++) {
            //for (var x = 0; x < oTmpCal.oCalendar.CalendarHeader.PeriodHeaderData.length; x++) {
            cellID = oTmpCal.oCalendar.CalendarHeader.PeriodHeaderData[x].Row1Text + "_" + oTmpCal.oCalendar.CalendarHeader.PeriodHeaderData[x].Row2Text;
            cellID = cellID.replace(':', '_');

            dayBodyCell = $('<td></td>');
            columnContentCell = $('<div></div>').attr('class', 'ColumnShiftTableBodyCell DailyCell totalRowBackground');

            cellValue = 0;
            Object.keys(oTmpCal.shiftsList).forEach(function (shiftKey, index) {
                if (typeof (oTmpCal.columnShiftsList[cellID][oTmpCal.shiftsList[shiftKey].ShortName]) != 'undefined') {
                    if (!oTmpCal.shiftsList[shiftKey].IsHoliday) cellValue += oTmpCal.columnShiftsList[cellID][oTmpCal.shiftsList[shiftKey].ShortName].Count;
                    cellValue -= oTmpCal.columnShiftsList[cellID][oTmpCal.shiftsList[shiftKey].ShortName].OnAbsence;
                }
            });

            columnContentCell.append($('<div></div>').attr('class', 'ContentShiftsColumnCell').html(cellValue));
            tBodyRow.append(dayBodyCell.append(columnContentCell));
        }
    } else {
        for (var x = 0; x < oTmpCal.oCalendar.CalendarHeader.PeriodHeaderData.length; x++) {
            cellID = oTmpCal.oCalendar.CalendarHeader.PeriodHeaderData[x].Row1Text + "_" + oTmpCal.oCalendar.CalendarHeader.PeriodHeaderData[x].Row2Text;
            cellID = cellID.replace(':', '_');

            dayBodyCell = $('<td></td>');
            columnContentCell = $('<div></div>').attr('class', 'ColumnShiftTableBodyCell totalRowBackground');

            cellValue = 0;
            Object.keys(oTmpCal.shiftsList).forEach(function (shiftKey, index) {
                if (typeof (oTmpCal.columnShiftsList[cellID][oTmpCal.shiftsList[shiftKey].ShortName]) != 'undefined') {
                    if (!oTmpCal.shiftsList[shiftKey].IsHoliday) cellValue += oTmpCal.columnShiftsList[cellID][oTmpCal.shiftsList[shiftKey].ShortName].Count;
                    cellValue -= oTmpCal.columnShiftsList[cellID][oTmpCal.shiftsList[shiftKey].ShortName].OnAbsence;
                }
            });

            columnContentCell.append($('<div></div>').attr('class', 'ContentShiftsColumnCell').html(cellValue));
            tBodyRow.append(dayBodyCell.append(columnContentCell));
        }
    }

    tmpCell = $('<td style="width:100%"></td>').attr('class', 'totalRowBackground').html('&nbsp;');
    tBodyRow.append(tmpCell);

    tBody.append(tBodyRow);

    return tBody;
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.createCapacityTable = function (idTable, parentId) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    var tableContainer = $('#' + oCalendar.prefix + parentId);
    tableContainer.empty();
    var tableElement = $('<table></table>').attr('id', oCalendar.prefix + idTable).attr('class', 'fancyColumnTable');
    //Necesitamos crear un header sin altura para poder utilizar el plugin de sticky headers y columns
    tableElement.append(this.createEmptyColumnTableHeader(idTable, parentId));
    tableElement.append(this.createColumnCapacityTableBody(idTable, parentId));
    tableElement.append(this.createColumnCapacityTableFooter(idTable, parentId));
    tableContainer.append(tableElement);
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.createColumnCapacityTableBody = function (idTable, parentId) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    //Creamos el body
    var tBody = $('<tbody></tbody>').attr('id', oCalendar.prefix + idTable + '_tbody');

    var oTmpCal = oCalendar;

    var distinctKeys = oTmpCal.capacityList[Object.keys(oTmpCal.capacityList)[0]];

    var bPaintTable = false;
    if (typeof (distinctKeys) != 'undefined') {
        Object.keys(distinctKeys).forEach(function (workcenterKey, index) {
            if (workcenterKey != '?') bPaintTable = true;
        });
    }

    if (bPaintTable) {
        Object.keys(distinctKeys).forEach(function (workcenterKey, index) {
            if (workcenterKey != '?') {
                var tBodyRow = $('<tr></tr>').attr('class', 'shiftColumnRow');

                var tFixedBodyCell = $('<td></td>');

                var mainFixedBodyCell = $('<div></div>').attr('class', 'CalendarShiftsFixed CalendarShiftFixedBody');

                mainFixedBodyCell.append($('<div></div>').attr('class', 'CalendarShiftContentFixedBody').html('<span style="font-weight:bold">' + workcenterKey + "</span>"));

                tBodyRow.append(tFixedBodyCell.append(mainFixedBodyCell));

                var cellID = null;
                var cellValue = "";
                var dayBodyCell = null;
                var columnContentCell = null;

                if (oTmpCal.viewRange == Robotics.Client.Constants.ViewRange.Daily) {
                    for (var x = oTmpCal.getMinDailyCell(); x < oTmpCal.getMaxDailyCell(); x++) {
                        //for (var x = 0; x < oTmpCal.oCalendar.CalendarHeader.PeriodHeaderData.length; x++) {
                        cellID = oTmpCal.oCalendar.CalendarHeader.PeriodHeaderData[x].Row1Text + "_" + oTmpCal.oCalendar.CalendarHeader.PeriodHeaderData[x].Row2Text;
                        cellID = cellID.replace(':', '_');
                        cellValue = "";

                        var bWarning = false;

                        if (typeof (oTmpCal.columnCapacityList[cellID][workcenterKey]) != 'undefined') {
                            if (oTmpCal.columnCapacityList[cellID][workcenterKey].Max > 0) {
                                cellValue = (oTmpCal.columnCapacityList[cellID][workcenterKey].Actual + "/" + oTmpCal.columnCapacityList[cellID][workcenterKey].Max);
                                if (oTmpCal.columnCapacityList[cellID][workcenterKey].Actual > oTmpCal.columnCapacityList[cellID][workcenterKey].Max) bWarning = true;
                            } else {
                                cellValue = (oTmpCal.columnCapacityList[cellID][workcenterKey].Actual + "/ --");
                            }
                        }

                        dayBodyCell = $('<td></td>');
                        columnContentCell = $('<div></div>').attr('class', 'ColumnShiftTableBodyCell DailyCell');
                        columnContentCell.append($('<div></div>').attr('class', 'ContentShiftsColumnCell').attr("style", (bWarning ? "color:red" : "color:black")).html(cellValue));

                        tBodyRow.append(dayBodyCell.append(columnContentCell));
                    }
                } else {
                    for (var x = 0; x < oTmpCal.oCalendar.CalendarHeader.PeriodHeaderData.length; x++) {
                        cellID = oTmpCal.oCalendar.CalendarHeader.PeriodHeaderData[x].Row1Text + "_" + oTmpCal.oCalendar.CalendarHeader.PeriodHeaderData[x].Row2Text;
                        cellID = cellID.replace(':', '_');
                        cellValue = "";
                        var bWarning = false;
                        if (typeof (oTmpCal.columnCapacityList[cellID][workcenterKey]) != 'undefined') {
                            if (oTmpCal.columnCapacityList[cellID][workcenterKey].Max > 0) {
                                cellValue = (oTmpCal.columnCapacityList[cellID][workcenterKey].Actual + "/" + oTmpCal.columnCapacityList[cellID][workcenterKey].Max);
                                if (oTmpCal.columnCapacityList[cellID][workcenterKey].Actual > oTmpCal.columnCapacityList[cellID][workcenterKey].Max) bWarning = true;
                            } else {
                                cellValue = (oTmpCal.columnCapacityList[cellID][workcenterKey].Actual + "/ --");
                            }
                        }

                        dayBodyCell = $('<td></td>');
                        columnContentCell = $('<div></div>').attr('class', 'ColumnShiftTableBodyCell');
                        columnContentCell.append($('<div></div>').attr('class', 'ContentShiftsColumnCell').attr("style", (bWarning ? "color:red" : "color:gray")).html(cellValue));

                        tBodyRow.append(dayBodyCell.append(columnContentCell));
                    }
                }

                tmpCell = $('<td style="width:100%"></td>').html('&nbsp;');
                tBodyRow.append(tmpCell);

                tBody.append(tBodyRow);
            }
        });
    } else {
        var tBodyRow = $('<tr></tr>').attr('class', 'shiftColumnRow');

        var tFixedBodyCell = $('<td></td>');

        var mainFixedBodyCell = $('<div></div>').attr('class', 'CalendarShiftsFixed CalendarShiftFixedBody');

        mainFixedBodyCell.append($('<div></div>').attr('class', 'CalendarShiftContentFixedBody').html('<span style="font-weight:bold">' + Globalize.formatMessage("roNoWorkCenter") + "</span>"));

        tBodyRow.append(tFixedBodyCell.append(mainFixedBodyCell));

        tBodyRow.append($('<td style="width:100%"></td>').html('&nbsp;'));

        tBody.append(tBodyRow);
    }

    return tBody;
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.createColumnCapacityTableFooter = function (idTable, parentId) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    //Creamos el body
    var tBody = $('<tfoot></tfoot>').attr('id', oCalendar.prefix + idTable + '_tfoot');

    var oTmpCal = oCalendar;

    var tBodyRow = $('<tr></tr>').attr('class', 'shiftColumnRow');

    var tFixedBodyCell = $('<td></td>');

    var mainFixedBodyCell = $('<div></div>').attr('class', '');

    mainFixedBodyCell.append($('<div></div>').attr('class', 'CalendarShiftContentFixedBody').html('&nbsp;'));

    tBodyRow.append(tFixedBodyCell.append(mainFixedBodyCell));

    var cellID = null;
    var cellValue = 0;
    var dayBodyCell = null;
    var columnContentCell = null;

    if (oTmpCal.viewRange == Robotics.Client.Constants.ViewRange.Daily) {
        for (var x = oTmpCal.getMinDailyCell(); x < oTmpCal.getMaxDailyCell(); x++) {
            cellID = oTmpCal.oCalendar.CalendarHeader.PeriodHeaderData[x].Row1Text + "_" + oTmpCal.oCalendar.CalendarHeader.PeriodHeaderData[x].Row2Text;
            cellID = cellID.replace(':', '_');

            dayBodyCell = $('<td></td>');
            tBodyRow.append(dayBodyCell);
        }
        //columnContentCell = $('<div></div>').attr('class', 'ColumnShiftTableBodyCell DailyCell totalRowBackground');

        //cellValue = 0;
        //var distinctKeys = oTmpCal.capacityList[Object.keys(oTmpCal.capacityList)[0]];
        //Object.keys(distinctKeys).forEach(function (workcenterKey, index) {
        //    if (typeof (oTmpCal.columnCapacityList[cellID][workcenterKey]) != 'undefined') {
        //        cellValue += oTmpCal.columnCapacityList[cellID][workcenterKey].OnTelecommute;
        //    }
        //});

        //columnContentCell.append($('<div></div>').attr('class', 'ContentShiftsColumnCell').html(cellValue));
        //tBodyRow.append(dayBodyCell.append(columnContentCell));
        //}
    } else {
        for (var x = 0; x < oTmpCal.oCalendar.CalendarHeader.PeriodHeaderData.length; x++) {
            cellID = oTmpCal.oCalendar.CalendarHeader.PeriodHeaderData[x].Row1Text + "_" + oTmpCal.oCalendar.CalendarHeader.PeriodHeaderData[x].Row2Text;
            cellID = cellID.replace(':', '_');

            dayBodyCell = $('<td></td>');
            tBodyRow.append(dayBodyCell);
        }
        //    columnContentCell = $('<div></div>').attr('class', 'ColumnShiftTableBodyCell totalRowBackground');

        //    cellValue = 0;
        //    var distinctKeys = oTmpCal.capacityList[Object.keys(oTmpCal.capacityList)[0]];
        //    Object.keys(distinctKeys).forEach(function (workcenterKey, index) {
        //        if (typeof (oTmpCal.columnCapacityList[cellID][workcenterKey]) != 'undefined') {
        //            cellValue += oTmpCal.columnCapacityList[cellID][workcenterKey].OnTelecommute;
        //        }
        //    });

        //    columnContentCell.append($('<div></div>').attr('class', 'ContentShiftsColumnCell').html(cellValue));
        //    tBodyRow.append(dayBodyCell.append(columnContentCell));
        //}
    }

    tmpCell = $('<td style="width:100%"></td>').html('&nbsp;');
    tBodyRow.append(tmpCell);

    tBody.append(tBodyRow);

    return tBody;
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.createColumnAssignmentsTable = function (idTable, parentId) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    var tableContainer = $('#' + oCalendar.prefix + parentId);
    tableContainer.empty();
    var tableElement = $('<table></table>').attr('id', oCalendar.prefix + idTable).attr('class', 'fancyColumnTable');
    //Necesitamos crear un header sin altura para poder utilizar el plugin de sticky headers y columns
    tableElement.append(this.createEmptyColumnTableHeader(idTable, parentId));
    tableElement.append(this.createColumnAssignmentsTableBody(idTable, parentId));
    tableElement.append(this.createColumnAssignmentsTableFooter(idTable, parentId));
    tableContainer.append(tableElement);
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.createColumnAssignmentsTableBody = function (idTable, parentId) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    //Creamos el body
    var tBody = $('<tbody></tbody>').attr('id', oCalendar.prefix + idTable + '_tbody');
    var oTmpCal = oCalendar;
    var assignments = oTmpCal.assignmentsFilter.split(',');

    Object.keys(oTmpCal.assignmentsList).forEach(function (assignmentKey, index) {
        var bInsert = false;

        if (oTmpCal.assignmentsFilter != '') {
            for (var aIndex = 0; aIndex < assignments.length; aIndex++) {
                if (parseInt(assignmentKey, 10) == parseInt(assignments[aIndex], 10)) { //|| parseInt(assignmentKey, 10) == 0) {
                    bInsert = true;
                    break;
                }
            }
        } else {
            bInsert = true;
        }

        if (bInsert) {
            var tBodyRow = $('<tr></tr>').attr('class', 'shiftColumnRow');
            var tFixedBodyCell = $('<td></td>');
            var mainFixedBodyCell = $('<div></div>').attr('class', 'CalendarShiftsFixed CalendarShiftFixedBody');
            mainFixedBodyCell.append($('<div></div>').attr('class', 'CalendarShiftContentFixedBody').html('<span style="font-weight:bold">' + oTmpCal.assignmentsList[assignmentKey].ShortName + "</span>:   " + oTmpCal.assignmentsList[assignmentKey].Name));
            tBodyRow.append(tFixedBodyCell.append(mainFixedBodyCell));
            var cellID = null;
            var cellValue = 0;
            var cellExpected = undefined;
            var dayBodyCell = null;
            var columnContentCell = null;
            if (oTmpCal.viewRange == Robotics.Client.Constants.ViewRange.Daily) {
                for (var x = oTmpCal.getMinDailyCell(); x < oTmpCal.getMaxDailyCell(); x++) {
                    //for (var x = 0; x < oTmpCal.oCalendar.CalendarHeader.PeriodHeaderData.length; x++) {
                    cellExpected = undefined;
                    cellID = oTmpCal.oCalendar.CalendarHeader.PeriodHeaderData[x].Row1Text + "_" + oTmpCal.oCalendar.CalendarHeader.PeriodHeaderData[x].Row2Text;
                    cellID = cellID.replace(':', '_');
                    cellValue = 0;
                    if (typeof (oTmpCal.columnAssignmentsList[cellID][oTmpCal.assignmentsList[assignmentKey].ID]) != 'undefined') {
                        cellValue = oTmpCal.columnAssignmentsList[cellID][oTmpCal.assignmentsList[assignmentKey].ID].Count;
                        if (oTmpCal.loadRecursive && typeof oTmpCal.columnAssignmentsList[cellID][oTmpCal.assignmentsList[assignmentKey].ID].Expected != 'undefined') cellExpected = oTmpCal.columnAssignmentsList[cellID][oTmpCal.assignmentsList[assignmentKey].ID].Expected;

                        cellValue = Math.round(cellValue * 100) / 100;
                        if (typeof cellExpected != 'undefined') cellExpected = Math.round(cellExpected * 100) / 100;
                    }
                    dayBodyCell = $('<td></td>');
                    columnContentCell = $('<div></div>').attr('class', 'ColumnShiftTableBodyCell DailyCell');

                    var outerStyle = '';
                    var cellContent = cellValue;

                    if (typeof cellExpected != 'undefined' && cellExpected == 0) outerStyle = 'color:#38e219';

                    if (typeof cellExpected != 'undefined' && cellExpected != 0 && oTmpCal.schedulingView == Robotics.Client.Constants.CoverageDayView.Planified) {
                        cellContent += '<span style="color:' + (cellExpected >= 0 ? 'orange"> (+ ' : 'red"> (') + cellExpected + ')</span>';
                    }

                    columnContentCell.append($('<div></div>').attr('class', 'ContentShiftsColumnCell').attr('style', outerStyle).html(cellContent));
                    tBodyRow.append(dayBodyCell.append(columnContentCell));
                }
            } else {
                for (var x = 0; x < oTmpCal.oCalendar.CalendarHeader.PeriodHeaderData.length; x++) {
                    cellExpected = undefined;
                    cellID = oTmpCal.oCalendar.CalendarHeader.PeriodHeaderData[x].Row1Text + "_" + oTmpCal.oCalendar.CalendarHeader.PeriodHeaderData[x].Row2Text;
                    cellID = cellID.replace(':', '_');
                    cellValue = 0;
                    if (typeof (oTmpCal.columnAssignmentsList[cellID][oTmpCal.assignmentsList[assignmentKey].ID]) != 'undefined') {
                        cellValue = oTmpCal.columnAssignmentsList[cellID][oTmpCal.assignmentsList[assignmentKey].ID].Count;
                        if (oTmpCal.loadRecursive && typeof oTmpCal.columnAssignmentsList[cellID][oTmpCal.assignmentsList[assignmentKey].ID].Expected != 'undefined') cellExpected = oTmpCal.columnAssignmentsList[cellID][oTmpCal.assignmentsList[assignmentKey].ID].Expected;

                        cellValue = Math.round(cellValue * 100) / 100;
                        if (typeof cellExpected != 'undefined') cellExpected = Math.round(cellExpected * 100) / 100;
                    }
                    dayBodyCell = $('<td></td>');
                    columnContentCell = $('<div></div>').attr('class', 'ColumnShiftTableBodyCell');

                    var outerStyle = '';
                    var cellContent = cellValue;

                    if (typeof cellExpected != 'undefined' && cellExpected == 0) outerStyle = 'color:#38e219';

                    if (typeof cellExpected != 'undefined' && cellExpected != 0 && oTmpCal.schedulingView == Robotics.Client.Constants.CoverageDayView.Planified) {
                        cellContent += '<span style="color:' + (cellExpected >= 0 ? 'orange"> (+ ' : 'red"> (') + cellExpected + ')</span>';
                    }

                    columnContentCell.append($('<div></div>').attr('class', 'ContentShiftsColumnCell').attr('style', outerStyle).html(cellContent));
                    tBodyRow.append(dayBodyCell.append(columnContentCell));
                }
            }

            tmpCell = $('<td style="width:100%"></td>').html('&nbsp;');
            tBodyRow.append(tmpCell);
            tBody.append(tBodyRow);
        }
    });
    return tBody;
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.createColumnAssignmentsTableFooter = function (idTable, parentId) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    //Creamos el body
    var tBody = $('<tfoot></tfoot>').attr('id', oCalendar.prefix + idTable + '_tfoot');
    var oTmpCal = oCalendar;
    var assignments = oTmpCal.assignmentsFilter.split(',');

    var tBodyRow = $('<tr></tr>').attr('class', 'shiftColumnRow');
    var tFixedBodyCell = $('<td></td>');
    var mainFixedBodyCell = $('<div></div>').attr('class', 'CalendarShiftsFixed CalendarShiftFixedBody');
    mainFixedBodyCell.append($('<div></div>').attr('class', 'CalendarShiftContentFixedBody').html('&nbsp;'));
    tBodyRow.append(tFixedBodyCell.append(mainFixedBodyCell));
    var cellID = null;
    var cellValue = 0;
    var dayBodyCell = null;
    var columnContentCell = null;
    if (oTmpCal.viewRange == Robotics.Client.Constants.ViewRange.Daily) {
        for (var x = oTmpCal.getMinDailyCell(); x < oTmpCal.getMaxDailyCell(); x++) {
            //for (var x = 0; x < oTmpCal.oCalendar.CalendarHeader.PeriodHeaderData.length; x++) {
            cellID = oTmpCal.oCalendar.CalendarHeader.PeriodHeaderData[x].Row1Text + "_" + oTmpCal.oCalendar.CalendarHeader.PeriodHeaderData[x].Row2Text;
            cellID = cellID.replace(':', '_');
            dayBodyCell = $('<td></td>');
            columnContentCell = $('<div></div>').attr('class', 'ColumnShiftTableBodyCell DailyCell totalRowBackground');
            cellValue = 0;

            Object.keys(oTmpCal.assignmentsList).forEach(function (assignmentKey, index) {
                var bInsert = false;

                if (oTmpCal.assignmentsFilter != '') {
                    for (var aIndex = 0; aIndex < assignments.length; aIndex++) {
                        if (parseInt(assignmentKey, 10) == parseInt(assignments[aIndex], 10)) { //|| parseInt(assignmentKey, 10) == 0) {
                            bInsert = true;
                            break;
                        }
                    }
                } else {
                    bInsert = true;
                }
                if (bInsert && typeof (oTmpCal.columnAssignmentsList[cellID][oTmpCal.assignmentsList[assignmentKey].ID]) != 'undefined') cellValue += oTmpCal.columnAssignmentsList[cellID][oTmpCal.assignmentsList[assignmentKey].ID].Count;
            });
            columnContentCell.append($('<div></div>').attr('class', 'ContentShiftsColumnCell').html(Math.round(cellValue * 100) / 100));
            tBodyRow.append(dayBodyCell.append(columnContentCell));
        }
    } else {
        for (var x = 0; x < oTmpCal.oCalendar.CalendarHeader.PeriodHeaderData.length; x++) {
            cellID = oTmpCal.oCalendar.CalendarHeader.PeriodHeaderData[x].Row1Text + "_" + oTmpCal.oCalendar.CalendarHeader.PeriodHeaderData[x].Row2Text;
            cellID = cellID.replace(':', '_');
            dayBodyCell = $('<td></td>');
            columnContentCell = $('<div></div>').attr('class', 'ColumnShiftTableBodyCell totalRowBackground');
            cellValue = 0;

            Object.keys(oTmpCal.assignmentsList).forEach(function (assignmentKey, index) {
                var bInsert = false;

                if (oTmpCal.assignmentsFilter != '') {
                    for (var aIndex = 0; aIndex < assignments.length; aIndex++) {
                        if (parseInt(assignmentKey, 10) == parseInt(assignments[aIndex], 10)) { //|| parseInt(assignmentKey, 10) == 0) {
                            bInsert = true;
                            break;
                        }
                    }
                } else {
                    bInsert = true;
                }

                if (bInsert && typeof (oTmpCal.columnAssignmentsList[cellID][oTmpCal.assignmentsList[assignmentKey].ID]) != 'undefined') cellValue += oTmpCal.columnAssignmentsList[cellID][oTmpCal.assignmentsList[assignmentKey].ID].Count;
            });
            columnContentCell.append($('<div></div>').attr('class', 'ContentShiftsColumnCell').html(Math.round(cellValue * 100) / 100));
            tBodyRow.append(dayBodyCell.append(columnContentCell));
        }
    }
    tmpCell = $('<td style="width:100%"></td>').attr('class', 'totalRowBackground').html('&nbsp;');
    tBodyRow.append(tmpCell);
    tBody.append(tBodyRow);
    return tBody;
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.createAccrualsTable = function (idTable, parentId) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    var tableContainer = $('#' + oCalendar.prefix + parentId);
    tableContainer.empty();

    var tableElement = $('<table></table>').attr('id', oCalendar.prefix + idTable).attr('class', 'fancyTable');

    tableElement.append(this.createAccrualsTableHeader(idTable, parentId));

    tableElement.append(this.createAccrualsTableBody(idTable, parentId));

    if (oCalendar.isScheduleActive) {
        tableElement.append(this.createAccrualsTableFooter(idTable, parentId));
    }

    tableContainer.append(tableElement);
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.createAccrualsTooltipContainer = function (rowIndex, empID) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    var hiddenDiv = $('<div></div>').attr("style", "position:absolute; left:-200%");
    var accrualTooltipContainer = $('<div></div>').attr('id', oCalendar.ascxPrefix + '_tooltipAccrualsContainer_' + rowIndex + '_' + 0).attr('class', 'tooltipAccrualsContainer');
    var divAccrualsTooltipTitle = $('<div></div>').html('<span class="fontBold fontBig" style="text-align:left">' + Globalize.formatMessage("roAccrualDetail") + '</span>');

    var divAccrualsTooltipList = $('<div style="margin-top:5px;clear:both"></div>');

    var assignedHour = $('<div style="clear:both;width:250px"></div>').html('<div class="tooltipAccrualsLeftCell"><span style="font-weight:bold">' + Globalize.formatMessage("roWorkingHoursAssigned") + '</span></div><div class="tooltipAccrualsRightCell"> ' + oCalendar.ConvertHoursToHourFormat(oCalendar.employeeDataList[empID].WorkingHours) + '</div>');
    var ordinaryHours = $('<div style="clear:both;width:250px"></div>').html('<div class="tooltipAccrualsLeftCell"><span style="font-weight:bold">' + Globalize.formatMessage("roCommonHoursAssigned") + '</span></div><div class="tooltipAccrualsRightCell"> ' + oCalendar.ConvertHoursToHourFormat(oCalendar.employeeDataList[empID].WorkingHours - oCalendar.employeeDataList[empID].ComplementaryHours) + '</div>');
    var complementrayHours = $('<div style="clear:both;width:250px"></div>').html('<div class="tooltipAccrualsLeftCell"><span style="font-weight:bold">' + Globalize.formatMessage("roComplementaryHoursAssigned") + '</span></div><div class="tooltipAccrualsRightCell"> ' + oCalendar.ConvertHoursToHourFormat(oCalendar.employeeDataList[empID].ComplementaryHours) + '</div>');
    var absenceHours = $('<div style="clear:both;width:250px"></div>').html('<div class="tooltipAccrualsLeftCell"><span style="font-weight:bold">' + Globalize.formatMessage("roAbsenceHoursAssigned") + '</span></div><div class="tooltipAccrualsRightCell"> ' + oCalendar.ConvertHoursToHourFormat(oCalendar.employeeDataList[empID].AbsenceHours) + '</div>');

    var accruedWorkingHours = $('<div style="clear:both;width:250px"></div>').html('<div class="tooltipAccrualsLeftCell"><span style="font-weight:bold">' + Globalize.formatMessage("roAccruedHours") + '</span></div><div class="tooltipAccrualsRightCell"> ' + oCalendar.ConvertHoursToHourFormat(oCalendar.employeeDataList[empID].PlannedHours.AccruedToDate * 60) + '</div>');
    var totalWorkingHours = $('<div style="clear:both;width:250px"></div>').html('<div class="tooltipAccrualsLeftCell"><span style="font-weight:bold">' + Globalize.formatMessage("roTotalHours") + '</span></div><div class="tooltipAccrualsRightCell"> ' + oCalendar.ConvertHoursToHourFormat(oCalendar.employeeDataList[empID].PlannedHours.YearTotal * 60) + '</div>');

    accrualTooltipContainer.append(divAccrualsTooltipTitle, divAccrualsTooltipList.append(assignedHour, ordinaryHours, complementrayHours, absenceHours, accruedWorkingHours, totalWorkingHours));

    return hiddenDiv.append(accrualTooltipContainer);
}

Robotics.Client.Controls.roSchedulerCalendar.prototype.createHolidaysTooltipContainer = function (rowIndex, empID) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    var hiddenDiv = $('<div></div>').attr("style", "position:absolute; left:-200%");
    var holidayTooltipContainer = $('<div></div>').attr('id', oCalendar.ascxPrefix + '_tooltipHolidaysContainer_' + rowIndex + '_' + 1).attr('class', 'tooltipHolidaysContainer');
    var divHolidaysTooltipTitle = $('<div></div>').html('<span class="fontBold fontBig" style="text-align:left">' + Globalize.formatMessage("roHolidayDetail") + '</span>');

    var divHolidaysTooltipList = $('<div style="margin-top:5px;clear:both"></div>');

    var availableHolidays = $('<div style="clear:both;width:175px"></div>').html('<div class="tooltipHolidaysLeftCell"><span style="font-weight:bold">' + Globalize.formatMessage("roHolidayAvilableDetail") + '</span></div><div class="tooltipHolidaysRightCell">  ' + (oCalendar.employeeDataList[empID].InitialHolidays - oCalendar.employeeDataList[empID].AssignedHolidays) + '</div>')
    var holidaysInPeriod = $('<div style="clear:both;width:175px"></div>').html('<div class="tooltipHolidaysLeftCell"><span style="font-weight:bold">' + Globalize.formatMessage("roHolidayInPeriod") + '</span></div><div class="tooltipHolidaysRightCell"> ' + (oCalendar.employeeDataList[empID].AssignedHolidays) + '</div>');
    var holidaysDone = $('<div style="clear:both;width:175px"></div>').html('<div class="tooltipHolidaysLeftCell"><span style="font-weight:bold">' + Globalize.formatMessage("roHolidayDoneDetail") + '</span></div><div class="tooltipHolidaysRightCell"> ' + (oCalendar.employeeDataList[empID].HolidayResume.Done) + '</div>');
    var holidaysRequested = $('<div style="clear:both;width:175px"></div>').html('<div class="tooltipHolidaysLeftCell"><span style="font-weight:bold">' + Globalize.formatMessage("roHolidayRequestedDetail") + '</span></div><div class="tooltipHolidaysRightCell"> ' + (oCalendar.employeeDataList[empID].HolidayResume.Requested) + '</div>');
    var holidaysPending = $('<div style="clear:both;width:175px"></div>').html('<div class="tooltipHolidaysLeftCell"><span style="font-weight:bold">' + Globalize.formatMessage("roHolidayPendingDetail") + '</span></div><div class="tooltipHolidaysRightCell"> ' + (oCalendar.employeeDataList[empID].HolidayResume.Pending) + '</div>');

    divHolidaysTooltipList.append(availableHolidays, holidaysInPeriod, holidaysDone, holidaysRequested, holidaysPending);

    holidayTooltipContainer.append(divHolidaysTooltipTitle, divHolidaysTooltipList);

    return hiddenDiv.append(holidayTooltipContainer);
}

Robotics.Client.Controls.roSchedulerCalendar.prototype.createAccrualsTableHeader = function (idTable, parentId) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    //Creamos el header
    var tHeader = $('<thead></thead>').attr('id', oCalendar.prefix + idTable + '_thead');

    var tHeaderRow = $('<tr></tr>');

    var accrualCell = $('<th></th>');
    var accrualContentCell = $('<div></div>').attr('class', 'CalendarDayFixed CalendarRowFixedHeader');
    accrualContentCell.append($('<div></div>').attr('class', 'NorthRowCell').html(oCalendar.translator.translate(Robotics.Client.Language.Tags.PlannedHours)));

    var holidaysCell = $('<th></th>');
    var holidaysContentCell = $('<div></div>').attr('class', 'CalendarDayFixed CalendarRowFixedHeader');
    holidaysContentCell.append($('<div></div>').attr('class', 'NorthRowCell').html(oCalendar.translator.translate(Robotics.Client.Language.Tags.Holidays)));

    tHeaderRow.append(accrualCell.append(accrualContentCell));
    tHeaderRow.append(holidaysCell.append(holidaysContentCell));

    tHeader.append(tHeaderRow);

    return tHeader;
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.createAccrualsTableBody = function (idTable, parentId) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    //Creamos el body
    var tBody = $('<tbody></tbody>').attr('id', oCalendar.prefix + idTable + '_tbody');

    var lastGroup = -1, actualGroup = -1;
    for (var rowIndex = 0; rowIndex < oCalData.CalendarData.length; rowIndex++) {
        actualGroup = oCalData.CalendarData[rowIndex].EmployeeData.IDGroup;
        if (oCalendar.sortColumn == -1 && actualGroup != lastGroup) {
            var tBodyRow = $('<tr></tr>');
            var tFixedAccrualBodyCell = $('<td></td>').attr('class', 'calendarOuterBodyFixedSeparator');
            var tFixedHolidayBodyCell = $('<td></td>').attr('class', 'calendarOuterBodyFixedSeparator');
            tBodyRow.append(tFixedAccrualBodyCell, tFixedHolidayBodyCell);

            tBody.append(tBodyRow);
        }

        var tBodyRow = $('<tr></tr>');

        var empID = oCalData.CalendarData[rowIndex].EmployeeData.IDEmployee + "_" + oCalData.CalendarData[rowIndex].EmployeeData.IDGroup;

        var accrualCell = $('<td></td>');
        var accrualContentCell = $('<div></div>').attr('id', oCalendar.ascxPrefix + '_tooltipAccruals_' + rowIndex + '_' + 0).attr('class', 'ColumnAccrualsTableBodyCell resumeAccrualCell');

        var accrualInfoCell = $('<div></div>').attr('id', oCalendar.ascxPrefix + '_tooltipTarget_' + rowIndex + '_' + 0).attr('class', 'ContentAccrualsRowCell');
        accrualInfoCell.append($('<div></div>').attr('class', 'totalAccrual').html(oCalendar.ConvertHoursToHourFormat(oCalendar.employeeDataList[empID].WorkingHours + oCalendar.employeeDataList[empID].AbsenceHours)));

        if (oCalendar.employeeDataList[empID].ComplementaryHours > 0) {
            var divComplementary = $('<div></div>');
            divComplementary.html('(HO:' + oCalendar.ConvertHoursToHourFormat(oCalendar.employeeDataList[empID].WorkingHours - oCalendar.employeeDataList[empID].ComplementaryHours) + ' HC:' + oCalendar.ConvertHoursToHourFormat(oCalendar.employeeDataList[empID].ComplementaryHours) + ')');

            accrualInfoCell.append(divComplementary);
        }
        accrualContentCell.append(accrualInfoCell, this.createAccrualsTooltipContainer(rowIndex, empID));

        var holidaysCell = $('<td></td>');
        var holidaysContentCell = $('<div></div>').attr('id', oCalendar.ascxPrefix + '_tooltipHolidays_' + rowIndex + '_' + 1).attr('class', 'ColumnAccrualsTableBodyCell resumeHolidayCell');

        var holidayInfoDiv = $('<div></div>').attr('id', oCalendar.ascxPrefix + '_tooltipTarget_' + rowIndex + '_' + 1).attr('class', 'ContentAccrualsRowCell');
        holidayInfoDiv.append($('<div></div>').attr('class', 'totalAccrual').html(oCalendar.employeeDataList[empID].InitialHolidays - oCalendar.employeeDataList[empID].AssignedHolidays));

        holidaysContentCell.append(holidayInfoDiv, this.createHolidaysTooltipContainer(rowIndex, empID));

        tBodyRow.append(accrualCell.append(accrualContentCell));
        tBodyRow.append(holidaysCell.append(holidaysContentCell));

        tmpCell = $('<td style="width:100%"></td>').html('&nbsp;');
        tBodyRow.append(tmpCell);

        tBody.append(tBodyRow);

        lastGroup = actualGroup;
    }

    return tBody;
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.createAccrualsTableFooter = function (idTable, parentId) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    //Creamos el header
    var tFoot = $('<tfoot></tfoot>').attr('id', oCalendar.prefix + idTable + '_tfoot');

    var tFootRow = $('<tr></tr>');

    var accrualCell = $('<td></td>');
    var accrualContentCell = $('<div></div>').attr('class', 'CalendarDayFixed CalendarRowFixedHeader');
    accrualContentCell.append($('<div></div>').attr('class', 'SouthRowCell').html(oCalendar.translator.translate(Robotics.Client.Language.Tags.PlannedHours)));

    var holidaysCell = $('<td></td>');
    var holidaysContentCell = $('<div></div>').attr('class', 'CalendarDayFixed CalendarRowFixedHeader');
    holidaysContentCell.append($('<div></div>').attr('class', 'SouthRowCell').html(oCalendar.translator.translate(Robotics.Client.Language.Tags.Holidays)));

    tFootRow.append(accrualCell.append(accrualContentCell));
    tFootRow.append(holidaysCell.append(holidaysContentCell));

    tFoot.append(tFootRow);

    return tFoot;
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.createShiftsRowTable = function (idTable, parentId) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    var tableContainer = $('#' + oCalendar.prefix + parentId);
    tableContainer.empty();

    var tableElement = $('<table></table>').attr('id', oCalendar.prefix + idTable).attr('class', 'fancyTable');

    tableElement.append(this.createShiftsRowTableHeader(idTable, parentId));

    tableElement.append(this.createShiftsRowTableBody(idTable, parentId));

    if (oCalendar.isScheduleActive) {
        tableElement.append(this.createShiftsRowTableFooter(idTable, parentId));
    }

    tableContainer.append(tableElement);
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.createShiftsRowTableHeader = function (idTable, parentId) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    //Creamos el header
    var tHeader = $('<thead></thead>').attr('id', oCalendar.prefix + idTable + '_thead');

    var tHeaderRow = $('<tr></tr>');

    var oTmpCal = oCalendar;
    Object.keys(oTmpCal.shiftsList).forEach(function (shiftKey, index) {
        var dayHeaderCell = $('<th></th>');

        var mainDayHeaderCell = $('<div></div>').attr('class', 'CalendarShiftsDayFixed CalendarShiftRowFixedHeader');

        mainDayHeaderCell.append($('<div></div>').attr('class', 'NorthRowCell').html(shiftKey));

        tHeaderRow.append(dayHeaderCell.append(mainDayHeaderCell));
    });

    tHeader.append(tHeaderRow);

    return tHeader;
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.createShiftsRowTableBody = function (idTable, parentId) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;
    //Creamos el body
    var tBody = $('<tbody></tbody>').attr('id', oCalendar.prefix + idTable + '_tbody');
    var oTmpCal = oCalendar;
    var lastGroup = -1, actualGroup = -1;

    for (var rowIndex = 0; rowIndex < oCalData.CalendarData.length; rowIndex++) {
        actualGroup = oCalData.CalendarData[rowIndex].EmployeeData.IDGroup;
        if (oCalendar.sortColumn == -1 && actualGroup != lastGroup) {
            var tBodyRow = $('<tr></tr>');
            Object.keys(oTmpCal.shiftsList).forEach(function (shiftKey, index) {
                tBodyRow.append($('<td></td>').attr('class', 'calendarOuterBodyFixedSeparator'));
            });
            tBody.append(tBodyRow);
        }

        var tBodyRow = $('<tr></tr>');

        Object.keys(oTmpCal.shiftsList).forEach(function (shiftKey, index) {
            var columnCell = $('<td></td>');

            var empID = oTmpCal.oCalendar.CalendarData[rowIndex].EmployeeData.IDEmployee + "_" + oTmpCal.oCalendar.CalendarData[rowIndex].EmployeeData.IDGroup;
            var cellValue = 0;
            if (typeof (oTmpCal.rowsShiftsList[empID][oTmpCal.shiftsList[shiftKey].ShortName]) != 'undefined') cellValue = oTmpCal.rowsShiftsList[empID][oTmpCal.shiftsList[shiftKey].ShortName].Count;

            var columnContentCell = $('<div></div>').attr('class', 'ColumnShiftsTableBodyCell');
            columnContentCell.append($('<div></div>').attr('class', 'ContentShiftsRowCell').html(cellValue));

            tBodyRow.append(columnCell.append(columnContentCell));
        });

        tmpCell = $('<td style="width:100%"></td>').html('&nbsp;');
        tBodyRow.append(tmpCell);

        tBody.append(tBodyRow);
        lastGroup = actualGroup;
    }

    return tBody;
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.createShiftsRowTableFooter = function (idTable, parentId) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    //Creamos el header
    var tFoot = $('<tfoot></tfoot>').attr('id', oCalendar.prefix + idTable + '_tfoot');

    var tFootRow = $('<tr></tr>');

    var oTmpCal = oCalendar;
    Object.keys(oTmpCal.shiftsList).forEach(function (shiftKey, index) {
        var dayHeaderCell = $('<td></td>');

        var mainDayHeaderCell = $('<div></div>').attr('class', 'CalendarShiftsDayFixed CalendarShiftRowFixedHeader');

        mainDayHeaderCell.append($('<div></div>').attr('class', 'NorthRowCell').html(shiftKey));

        tFootRow.append(dayHeaderCell.append(mainDayHeaderCell));
    });

    tFoot.append(tFootRow);

    return tFoot;
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.createAssignmentsRowTable = function (idTable, parentId) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    var tableContainer = $('#' + oCalendar.prefix + parentId);
    tableContainer.empty();
    var tableElement = $('<table></table>').attr('id', oCalendar.prefix + idTable).attr('class', 'fancyTable');
    tableElement.append(this.createAssignmentsRowTableHeader(idTable, parentId));
    tableElement.append(this.createAssignmentsRowTableBody(idTable, parentId));
    if (oCalendar.isScheduleActive) {
        tableElement.append(this.createAssignmentsRowTableFooter(idTable, parentId));
    }
    tableContainer.append(tableElement);
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.createAssignmentsRowTableHeader = function (idTable, parentId) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    //Creamos el header
    var tHeader = $('<thead></thead>').attr('id', oCalendar.prefix + idTable + '_thead');
    var tHeaderRow = $('<tr></tr>');
    var oTmpCal = oCalendar;
    var assignments = oTmpCal.assignmentsFilter.split(',');

    Object.keys(oTmpCal.assignmentsList).forEach(function (assignmentKey, index) {
        var bInsert = false;

        if (oTmpCal.assignmentsFilter != '') {
            for (var aIndex = 0; aIndex < assignments.length; aIndex++) {
                if (parseInt(assignmentKey, 10) == parseInt(assignments[aIndex], 10)) { //|| parseInt(assignmentKey, 10) == 0) {
                    bInsert = true;
                    break;
                }
            }
        } else {
            bInsert = true;
        }
        if (bInsert) {
            var dayHeaderCell = $('<th></th>');
            var mainDayHeaderCell = $('<div></div>').attr('class', 'CalendarShiftsDayFixed CalendarShiftRowFixedHeader');
            mainDayHeaderCell.append($('<div></div>').attr('class', 'NorthRowCell').attr('title', oTmpCal.assignmentsList[assignmentKey].Name).html(oTmpCal.assignmentsList[assignmentKey].ShortName));
            tHeaderRow.append(dayHeaderCell.append(mainDayHeaderCell));
        }
    });
    tHeader.append(tHeaderRow);
    return tHeader;
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.createAssignmentsRowTableBody = function (idTable, parentId) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;
    //Creamos el body
    var tBody = $('<tbody></tbody>').attr('id', oCalendar.prefix + idTable + '_tbody');
    var oTmpCal = oCalendar;
    var assignments = oTmpCal.assignmentsFilter.split(',');
    var lastGroup = -1, actualGroup = -1;

    for (var rowIndex = 0; rowIndex < oCalData.CalendarData.length; rowIndex++) {
        actualGroup = oCalData.CalendarData[rowIndex].EmployeeData.IDGroup;
        if (oCalendar.sortColumn == -1 && actualGroup != lastGroup) {
            var tBodyRow = $('<tr></tr>');
            Object.keys(oTmpCal.assignmentsList).forEach(function (assignmentKey, index) {
                tBodyRow.append($('<td></td>').attr('class', 'calendarOuterBodyFixedSeparator'));
            });

            tBody.append(tBodyRow);
        }

        var tBodyRow = $('<tr></tr>');
        Object.keys(oTmpCal.assignmentsList).forEach(function (assignmentKey, index) {
            var bInsert = false;

            if (oTmpCal.assignmentsFilter != '') {
                for (var aIndex = 0; aIndex < assignments.length; aIndex++) {
                    if (parseInt(assignmentKey, 10) == parseInt(assignments[aIndex], 10)) { //|| parseInt(assignmentKey, 10) == 0) {
                        bInsert = true;
                        break;
                    }
                }
            } else {
                bInsert = true;
            }
            if (bInsert) {
                var columnCell = $('<td></td>');
                var empID = oTmpCal.oCalendar.CalendarData[rowIndex].EmployeeData.IDEmployee + "_" + oTmpCal.oCalendar.CalendarData[rowIndex].EmployeeData.IDGroup;
                var cellValue = 0;
                if (typeof (oTmpCal.rowsAssignmentsList[empID][oTmpCal.assignmentsList[assignmentKey].ID]) != 'undefined') cellValue = oTmpCal.rowsAssignmentsList[empID][oTmpCal.assignmentsList[assignmentKey].ID].Count;
                var columnContentCell = $('<div></div>').attr('class', 'ColumnShiftsTableBodyCell');
                columnContentCell.append($('<div></div>').attr('class', 'ContentShiftsRowCell').html(Math.round(cellValue * 100) / 100));
                tBodyRow.append(columnCell.append(columnContentCell));
            }
        });
        tmpCell = $('<td style="width:100%"></td>').html('&nbsp;');
        tBodyRow.append(tmpCell);
        tBody.append(tBodyRow);

        lastGroup = actualGroup;
    }
    return tBody;
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.createAssignmentsRowTableFooter = function (idTable, parentId) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    //Creamos el header
    var tFoot = $('<tfoot></tfoot>').attr('id', oCalendar.prefix + idTable + '_tfoot');
    var tFootRow = $('<tr></tr>');
    var oTmpCal = oCalendar;
    var assignments = oTmpCal.assignmentsFilter.split(',');

    Object.keys(oTmpCal.assignmentsList).forEach(function (assignmentKey, index) {
        var bInsert = false;

        if (oTmpCal.assignmentsFilter != '') {
            for (var aIndex = 0; aIndex < assignments.length; aIndex++) {
                if (parseInt(assignmentKey, 10) == parseInt(assignments[aIndex], 10)) { //|| parseInt(assignmentKey, 10) == 0) {
                    bInsert = true;
                    break;
                }
            }
        } else {
            bInsert = true;
        }
        if (bInsert) {
            var dayHeaderCell = $('<td></td>');
            var mainDayHeaderCell = $('<div></div>').attr('class', 'CalendarShiftsDayFixed CalendarShiftRowFixedHeader');
            mainDayHeaderCell.append($('<div></div>').attr('class', 'NorthRowCell').attr('title', oTmpCal.assignmentsList[assignmentKey].Name).html(oTmpCal.assignmentsList[assignmentKey].ShortName));
            tFootRow.append(dayHeaderCell.append(mainDayHeaderCell));
        }
    });
    tFoot.append(tFootRow);
    return tFoot;
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.createIndictmentscolumnTable = function (idTable, parentId) {
    var oClientMode = this;
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    var tableContainer = $('#' + oCalendar.prefix + parentId);
    tableContainer.empty();

    tableContainer.append($('<div></div>').attr('id', 'calendar_indictments_groupedList'));

    oClientMode.calendarIndictmentsDS = [];
    Object.keys(oClientMode.calendarIndictments).forEach(function (key, index) {
        oClientMode.calendarIndictmentsDS.push(oClientMode.calendarIndictments[key]);
    });

    setTimeout(function () {
        $('#calendar_indictments_groupedList').dxList({
            dataSource: oClientMode.calendarIndictmentsDS,
            height: "100%",
            grouped: true,
            collapsibleGroups: true,
            groupTemplate: function (data) {
                return $("<div>" + Globalize.formatMessage('roIndictmentRule') + ": " + data.key + "(" + data.counter + ")</div>");
            },
            itemTemplate: function (data) {
                return $('<div style="cursor:pointer">' + data.description + '</div>');
            },
            onContentReady: function (e) {
                setTimeout(function () {
                    var items = e.component.option("items");
                    for (var i = 0; i < items.length; i++)
                        e.component.collapseGroup(i);
                }, 50);
            },
            onItemClick: function (e) {
                var selectRow = e.itemData.idRow[0];
                var selectColumn = e.itemData.idColumn[0];

                $('#' + oCalendar.prefix + 'Main div.fht-fixed-body div.fht-tbody').scrollLeft((selectColumn - 5) * 100);

                oClientMode.setSingleSelectedObject($('#' + oCalendar.ascxPrefix + '_calCell_' + selectRow + '_' + selectColumn));

                for (var tmpIndex = 0; tmpIndex < e.itemData.idRow.length; tmpIndex++) {
                    var iRow = e.itemData.idRow[tmpIndex];
                    var iCol = e.itemData.idColumn[tmpIndex];

                    $('#' + oCalendar.ascxPrefix + '_calCell_' + iRow + '_' + iCol).addClass('scaledObject');
                }

                setTimeout(function () {
                    for (var tmpIndex = 0; tmpIndex < e.itemData.idRow.length; tmpIndex++) {
                        var iRow = e.itemData.idRow[tmpIndex];
                        var iCol = e.itemData.idColumn[tmpIndex];

                        $('#' + oCalendar.ascxPrefix + '_calCell_' + iRow + '_' + iCol).removeClass('scaledObject');
                    }
                }, 350);
            }
        });
    }, 100);
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.getSelectedIDEmployees = function () {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    var selectedIds = '';

    if (oCalData != null) {
        for (var i = 0; i < oCalData.CalendarData.length; i++) {
            if (selectedIds != '') selectedIds += ',';
            selectedIds += oCalData.CalendarData[i].EmployeeData.IDEmployee;
        }
    }

    return selectedIds;
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.getAssignedShift = function (destinationDay) {
    return destinationDay.MainShift;
    //return moment(destinationDay.PlanDate) <= moment(new Date()) ? destinationDay.ShiftUsed : destinationDay.MainShift;
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.selectCellByEmployeeDate = function (idEmployee, selectedDay) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    var bFound = false;

    if (oCalData != null && oCalData.CalendarData != null && oCalData.CalendarData.length > 0) {
        for (var rowIndex = 0; rowIndex < oCalData.CalendarData.length; rowIndex++) {
            if (oCalData.CalendarData[rowIndex].EmployeeData.IDEmployee == idEmployee) {
                oCalendar.selectedEmployee = oCalData.CalendarData[rowIndex];

                for (var columnIndex = 0; columnIndex < oCalData.CalendarData[rowIndex].PeriodData.DayData.length; columnIndex++) {
                    if (moment(oCalData.CalendarData[rowIndex].PeriodData.DayData[columnIndex].PlanDate).isSame(selectedDay)) {
                        oCalendar.selectedDay = oCalData.CalendarData[rowIndex].PeriodData.DayData[columnIndex];

                        if (oCalendar.typeView == Robotics.Client.Constants.TypeView.Planification) {
                            if (oCalendar.viewRange == Robotics.Client.Constants.ViewRange.Period) {
                                oCalendar.selectedContainer = $('#' + oCalendar.ascxPrefix + '_calCell_' + rowIndex + '_' + columnIndex);
                            } else if (oCalendar.viewRange == Robotics.Client.Constants.ViewRange.Daily) {
                                oCalendar.selectedContainer = $('#' + oCalendar.ascxPrefix + '_calDailyCell_' + rowIndex + '_60');
                            }
                        } else if (oCalendar.typeView == Robotics.Client.Constants.TypeView.Review) {
                            oCalendar.selectedContainer = $('#' + oCalendar.ascxPrefix + '_reviewDailyCell_' + rowIndex + '_' + columnIndex);
                        }

                        bFound = true;
                        break;
                    }
                }

                break;
            }
        }
    }
    return bFound;
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.generateLoadFilters = function () {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    var oParameters = {};

    if (oCalendar.firstDate == null) {
        oCalendar.firstDate = moment().startOf('month').toDate();
        oCalendar.endDate = moment(oCalendar.firstDate).add(1, 'month').subtract(1, 'day').startOf('day').toDate();
    }

    oParameters.firstDate = oCalendar.firstDate;
    oParameters.endDate = oCalendar.endDate;

    if (oCalendar.employeeFilter != "") {
        oParameters.employeeFilter = oCalendar.employeeFilter;
    } else {
        //Por defecto no se selecciona ningún empleado
        oParameters.employeeFilter = "1=2";
    }

    oParameters.loadRecursive = oCalendar.loadRecursive;

    if (oCalendar.isScheduleActive || oCalendar.saasPremiumActive) oParameters.loadIndictments = oCalendar.loadIndictments;
    else oParameters.loadIndictments = false;

    oParameters.loadPunches = oCalendar.loadPunches;
    oParameters.loadCapacities = oCalendar.loadCapacities;
    oParameters.calendar = null;
    oParameters.typeView = oCalendar.typeView;
    oParameters.assignmentsFilter = oCalendar.assignmentsFilter;
    oParameters.dailyPeriod = oCalendar.dailyPeriod;

    oParameters.StampParam = new Date().getMilliseconds();

    return oParameters;
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.loadData = function (firstDate, endDate, employeeFilter, loadRecursive, typeView, calendarFilter, loadIndictments, loadPunches, dailyPeriod, loadCapacities) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    if (oCalendar.hasChanges == false) {
        if (oCalendar.refreshTimmer != -1) clearTimeout(oCalendar.refreshTimmer);
        oCalendar.refreshTimmer = -1;
        oCalendar.sortColumn = -1;

        if (typeof (employeeFilter) != 'undefined' && employeeFilter != null) oCalendar.employeeFilter = employeeFilter;
        if (typeof (loadRecursive) != 'undefined' && loadRecursive != null) oCalendar.loadRecursive = loadRecursive;
        if (oCalendar.isScheduleActive || oCalendar.saasPremiumActive) {
            if (typeof (loadIndictments) != 'undefined' && loadIndictments != null) oCalendar.loadIndictments = loadIndictments;
        } else oCalendar.loadIndictments = false;

        if (typeof (loadPunches) != 'undefined' && loadPunches != null) oCalendar.loadPunches = loadPunches;
        if (typeof (loadCapacities) != 'undefined' && loadCapacities != null) oCalendar.loadPunches = loadCapacities;
        if (typeof (firstDate) != 'undefined' && firstDate != null) oCalendar.firstDate = firstDate;
        if (typeof (endDate) != 'undefined' && endDate != null) oCalendar.endDate = endDate;
        if (typeof (typeView) != 'undefined' && typeView != null) oCalendar.typeView = typeView;
        if (typeof (calendarFilter) != 'undefined' && calendarFilter != null) oCalendar.assignmentsFilter = calendarFilter;

        if (typeof (dailyPeriod) != 'undefined' && dailyPeriod != null) oCalendar.dailyPeriod = dailyPeriod;

        oCalendar.refresh();
    } else {
        oCalendar.showChangesWarning("refreshCalendar('',true);");
    }
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.saveChanges = function () {
    var oCalendar = this.oBaseControl;

    if (oCalendar.capatityEnabled) {
        var bShowCapacityWarning = false;

        if (oCalendar.capacityError) {
            var oTmpCal = oCalendar;
            var distinctKeys = oTmpCal.capacityList[Object.keys(oTmpCal.capacityList)[0]];

            if (typeof (distinctKeys) != 'undefined') {
                Object.keys(distinctKeys).forEach(function (workcenterKey, index) {
                    var cellID = null;
                    if (oTmpCal.viewRange == Robotics.Client.Constants.ViewRange.Daily) {
                        for (var x = oTmpCal.getMinDailyCell(); x < oTmpCal.getMaxDailyCell(); x++) {
                            cellID = oTmpCal.oCalendar.CalendarHeader.PeriodHeaderData[x].Row1Text + "_" + oTmpCal.oCalendar.CalendarHeader.PeriodHeaderData[x].Row2Text;
                            cellID = cellID.replace(':', '_');
                            if (typeof (oTmpCal.columnCapacityList[cellID][workcenterKey]) != 'undefined') {
                                if (oTmpCal.columnCapacityList[cellID][workcenterKey].HasChanges && oTmpCal.columnCapacityList[cellID][workcenterKey].Max > 0 && oTmpCal.columnCapacityList[cellID][workcenterKey].Actual > oTmpCal.columnCapacityList[cellID][workcenterKey].Max)
                                    bShowCapacityWarning = true;
                            }
                        }
                    } else {
                        for (var x = 0; x < oTmpCal.oCalendar.CalendarHeader.PeriodHeaderData.length; x++) {
                            cellID = oTmpCal.oCalendar.CalendarHeader.PeriodHeaderData[x].Row1Text + "_" + oTmpCal.oCalendar.CalendarHeader.PeriodHeaderData[x].Row2Text;
                            cellID = cellID.replace(':', '_');
                            if (typeof (oTmpCal.columnCapacityList[cellID][workcenterKey]) != 'undefined') {
                                if (oTmpCal.columnCapacityList[cellID][workcenterKey].HasChanges && oTmpCal.columnCapacityList[cellID][workcenterKey].Max > 0 && oTmpCal.columnCapacityList[cellID][workcenterKey].Actual > oTmpCal.columnCapacityList[cellID][workcenterKey].Max)
                                    bShowCapacityWarning = true;
                            }
                        }
                    }
                });
            }
        }

        if (bShowCapacityWarning) {
            oCalendar.showCapacityWarning();
        } else {
            this.saveChangesFinally();
        }
    } else {
        this.saveChangesFinally();
    }
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.saveChangesFinally = function () {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    var oParameters = {};

    oParameters.calendar = oCalData;

    oParameters.StampParam = new Date().getMilliseconds();

    oCalendar.performAction(oParameters, Robotics.Client.Constants.Actions.SaveChanges);
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.reloadHourDayData = function () {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    var oParameters = {};

    oParameters.StampParam = new Date().getMilliseconds();
    oParameters.idEmployee = oCalendar.selectedEmployee.EmployeeData.IDEmployee;
    oParameters.idGroup = oCalendar.selectedEmployee.EmployeeData.IDGroup;

    var currentDayShift = this.getAssignedShift(oCalendar.selectedDay);

    var ShiftBase = null;
    ShiftBase = {
        ID: currentDayShift.ID,
        ShortName: currentDayShift.ShortName,
        PlannedHours: currentDayShift.PlannedHours,
        Color: currentDayShift.Color,
        Name: currentDayShift.Name,
        Type: currentDayShift.Type,
        StartHour: currentDayShift.StartHour,
        EndHour: moment(currentDayShift.EndHour).clone().toDate(),
        ShiftLayers: oCalendar.selectedDay.MainShift.ShiftLayers,
        ShiftLayersDefinition: oCalendar.selectedDay.MainShift.ShiftLayersDefinition
    };

    if (currentDayShift != null) {
        if (currentDayShift.Type == Robotics.Client.Constants.ShiftType.Holiday || currentDayShift.Type == Robotics.Client.Constants.ShiftType.Holiday_NonWorking) {
            oParameters.idShift = oCalendar.selectedDay.ShiftBase.ID;
            ShiftBase.ShiftLayers = oCalendar.selectedDay.ShiftBase.ShiftLayers;
            ShiftBase.ShiftLayersDefinition = oCalendar.selectedDay.ShiftBase.ShiftLayersDefinition;
        } else {
            oParameters.idShift = currentDayShift.ID;
        }
    } else {
        oParameters.idShift = -1;
    }

    oParameters.shiftData = ShiftBase;
    oParameters.firstDate = oCalendar.selectedDay.PlanDate;
    oParameters.dailyPeriod = oCalendar.dailyPeriod;

    if ((oCalendar.selectedDay.MainShift.Type == Robotics.Client.Constants.ShiftType.Holiday || oCalendar.selectedDay.MainShift.Type == Robotics.Client.Constants.ShiftType.Holiday_NonWorking) &&
        (typeof oCalendar.selectedDay.ShiftBase != 'undefined' && oCalendar.selectedDay.ShiftBase != null && oCalendar.selectedDay.ShiftBase.Type == Robotics.Client.Constants.ShiftType.NormalFloating)) {
        oParameters.endDate = oCalendar.selectedDay.ShiftBase.StartHour;
    } else {
        oParameters.endDate = oCalendar.selectedDay.MainShift.StartHour;
    }

    oCalendar.performAction(oParameters, Robotics.Client.Constants.Actions.LoadHourData);
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.refreshFullDay = function () {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    if (oCalendar.typeView == Robotics.Client.Constants.TypeView.Planification) {
        if (oCalendar.viewRange == Robotics.Client.Constants.ViewRange.Period) {
            oCalendar.selectedContainer.empty();
            this.buildCalendarCellContent(oCalendar.selectedContainer, oCalendar.selectedDay, false, oCalendar.selectedEmployee.EmployeeData.FreezingDate);
        } else if (oCalendar.viewRange == Robotics.Client.Constants.ViewRange.Daily) {
            this.reloadHourDayData();
        }

        this.waitForUserToRefresh();
    } else if (oCalendar.typeView == Robotics.Client.Constants.TypeView.Review) {
        oCalendar.selectedContainer.empty();
        if (oCalendar.loadPunches) this.buildReviewCalendarCellPunches(oCalendar.selectedContainer, oCalendar.selectedDay);
        else this.buildReviewCalendarCellContentV2(oCalendar.selectedContainer, oCalendar.selectedDay, oCalendar.selectedEmployee.EmployeeData.FreezingDate);
    }
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.refreshDailySelectedData = function (markHasChanges) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    var selectedRow = parseInt(oCalendar.selectedContainer.attr('data-IDRow'), 10);

    for (var x = oCalendar.getMinDailyCell(); x < oCalendar.getMaxDailyCell(); x++) {
        //for (var x = 0; x < oCalData.CalendarHeader.PeriodHeaderData.length; x++) {
        var cellContainer = $('#' + oCalendar.ascxPrefix + '_calDailyCell_' + selectedRow + '_' + x);
        cellContainer.empty();
        this.createDailyCalendarCellContent(cellContainer, oCalData.CalendarData[selectedRow].EmployeeData, oCalData.CalendarData[selectedRow].PeriodData.DayData[0], oCalData.CalendarData[selectedRow].PeriodData.DayData[0].HourData[x], x);
    }

    if (markHasChanges) {
        oCalData.CalendarData[selectedRow].PeriodData.DayData[0].HasChanged = true;
        this.setHasChanges(true);
    }

    this.waitForUserToRefresh();
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.buildContextMenu = function (sender) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    var controlText = sender[0].innerText;

    if (oCalendar.typeView == Robotics.Client.Constants.TypeView.Planification) {
        items = {
            'enter': { name: oCalendar.translator.translate(Robotics.Client.Language.Tags.ContextMenu_Details), disabled: false, icon: 'detail' },
            'complementary': { name: oCalendar.translator.translate(Robotics.Client.Language.Tags.ContextMenu_EditComplementary), disabled: false, icon: 'complementary' },
            'assignment': { name: oCalendar.translator.translate(Robotics.Client.Language.Tags.ContextMenu_EditAssignments), disabled: false, icon: 'assignment' },
            'removeShift': { name: Globalize.formatMessage('roRemoveShift'), disabled: false, icon: 'removeShift' },
            'removeBudget': { name: Globalize.formatMessage('roRemoveFromBudget'), disabled: false, icon: 'removeBudget' },
            'split1': { name: '---------', disabled: true },
            'setTelecommuteOffice': { name: Globalize.formatMessage('roSetTelecommuteOffice'), disabled: false, icon: 'tc_office' },
            'setTelecommuteHome': { name: Globalize.formatMessage('roSetTelecommuteHome'), disabled: false, icon: 'tc_home' },
            'setTelecommuteOptional': { name: Globalize.formatMessage('roSetTelecommuteOptional'), disabled: false, icon: 'tc_optional' },
            'setTelecommuteLabAgree': { name: Globalize.formatMessage('roSetTelecommuteLabAgree'), disabled: false, icon: 'tc_labagree' },
            'split2': { name: '---------', disabled: true },
            'vacation': { name: oCalendar.translator.translate(Robotics.Client.Language.Tags.ContextMenu_RemoveHolidays), disabled: false, icon: 'vacation' },
            'addFeast': { name: oCalendar.translator.translate(Robotics.Client.Language.Tags.ContextMenu_SetFeast), disabled: false, icon: 'setFeast' },
            'removeFeast': { name: oCalendar.translator.translate(Robotics.Client.Language.Tags.ContextMenu_RemoveFeast), disabled: false, icon: 'removeFeast' },
            'split3': { name: '---------', disabled: true },
            'copyShift': { name: oCalendar.translator.translate(Robotics.Client.Language.Tags.ContextMenu_Copy), disabled: false, icon: 'copyShift' },
            'copyHolidays': { name: oCalendar.translator.translate(Robotics.Client.Language.Tags.ContextMenu_CopyHolidays), disabled: false, icon: 'copyHolidays' },
            'copyWorking': { name: oCalendar.translator.translate(Robotics.Client.Language.Tags.ContextMenu_CopyWorking), disabled: false, icon: 'copyWorking' },
            'copyAssignments': { name: oCalendar.translator.translate(Robotics.Client.Language.Tags.ContextMenu_CopyAssignments), disabled: false, icon: 'copyAssignments' },
            'split4': { name: '---------', disabled: true },
            'advPaste': { name: oCalendar.translator.translate(Robotics.Client.Language.Tags.ContextMenu_AdvPaste), disabled: false, icon: 'advPaste' },
            'pasteShift': { name: oCalendar.translator.translate(Robotics.Client.Language.Tags.ContextMenu_Paste), disabled: false, icon: 'pasteShift' },
            'pasteTelecommute': { name: Globalize.formatMessage('roPasteTelecommute'), disabled: false, icon: 'tc_pasteTelecommute' },
            'cancelSelection': { name: oCalendar.translator.translate(Robotics.Client.Language.Tags.ContextMenu_CancelSelection), disabled: false, icon: 'cancelSelection' },
            'split5': { name: '---------', disabled: true },
            'block': { name: oCalendar.translator.translate(Robotics.Client.Language.Tags.ContextMenu_Block), disabled: false, icon: 'block' },
            'removeblock': { name: oCalendar.translator.translate(Robotics.Client.Language.Tags.ContextMenu_UnBlock), disabled: false, icon: 'removeblock' }
        };

        var currentDayShift = this.getAssignedShift(oCalendar.selectedDay);

        if (oCalendar.hasChanges) {
            items['enter'].disabled = true;
        }

        if (!oCalendar.telecommuteEnabled) {
            delete items['split2'];
            delete items['setTelecommuteOffice'];
            delete items['setTelecommuteHome'];
            delete items['setTelecommuteOptional'];
            delete items['pasteTelecommute'];

            delete items['setTelecommuteLabAgree'];
        } else {
            if (oCalendar.selectedDay.CanTelecommute && typeof oCalendar.selectedDay.MainShift != 'undefined') {
                items['setTelecommuteOffice'].disabled = false;
                items['setTelecommuteHome'].disabled = false;
                items['setTelecommuteOptional'].disabled = false;
                items['setTelecommuteLabAgree'].disabled = false;
            } else {
                items['setTelecommuteOffice'].disabled = true;
                items['setTelecommuteHome'].disabled = true;
                items['setTelecommuteOptional'].disabled = true;

                //La opción de quitar teletrabajo se deja habilitada a futuro.
                if (moment(oCalendar.selectedDay.PlanDate).isAfter(moment().startOf('day'))) {
                    items['setTelecommuteLabAgree'].disabled = false;
                } else {
                    items['setTelecommuteLabAgree'].disabled = true;
                }
            }
        }

        if (oCalendar.selectedDay.IDDailyBudgetPosition > 0) {
            items['removeBudget'].disabled = false;
            items['removeShift'].disabled = true;
            items['assignment'].disabled = true;
            items['vacation'].disabled = true;
            items['addFeast'].disabled = true;
            items['removeFeast'].disabled = true;
            items['copyShift'].disabled = true;
            items['copyHolidays'].disabled = true;
            items['copyWorking'].disabled = true;
            items['copyAssignments'].disabled = true;
            items['advPaste'].disabled = true;
            items['pasteShift'].disabled = true;
            items['cancelSelection'].disabled = true;
            items['block'].disabled = true;
            items['removeblock'].disabled = true;

            if (!(currentDayShift != null && (currentDayShift.ExistComplementaryData || currentDayShift.ExistFloatingData))) {
                items['complementary'].disabled = true;
            }
        } else {
            items['removeBudget'].disabled = true;

            if (!oCalendar.isScheduleActive) {
                delete items['assignment'];
                delete items['copyAssignments'];
            } else {
                if (oCalendar.selectedDay.AllowAssignment != 'undefined' && (!oCalendar.selectedDay.AllowAssignment || oCalendar.selectedDay.IsHoliday)) items['assignment'].disabled = true;
            }

            if (!oCalendar.selectionCopied) {
                items['pasteShift'].disabled = true;
                items['advPaste'].disabled = true;
                items['cancelSelection'].disabled = true;
            }

            var isStarterShift = false;
            if (typeof currentDayShift != 'undefined' && currentDayShift != null && currentDayShift.AdvancedParameters.find(function (x) { return x.Name.toLowerCase() == 'starter' && x.Value == '1' }) != null) {
                isStarterShift = true;
            }

            if (isStarterShift) {
                items['complementary'].disabled = false;
            } else {
                if (!(currentDayShift != null && (currentDayShift.Type == Robotics.Client.Constants.ShiftType.NormalFloating || currentDayShift.ExistComplementaryData || currentDayShift.ExistFloatingData))) {
                    items['complementary'].disabled = true;
                }
            }

            if (oCalendar.selectedDay.Feast) {
                items['addFeast'].disabled = true;
            }

            if (!oCalendar.selectedDay.Feast) {
                items['removeFeast'].disabled = true;
            }

            if (!oCalendar.selectedDay.IsHoliday) {
                items['vacation'].disabled = true;
            }

            var clickedRow = parseInt($(sender).attr('data-IDRow'), 10);
            var clickedColumn = parseInt($(sender).attr('data-IDColumn'), 10);

            if ((clickedRow >= oCalendar.selectedMinRow && clickedRow <= oCalendar.selectedMaxRow) && (clickedColumn >= oCalendar.selectedMinColumn && clickedColumn <= oCalendar.selectedMaxColumn)) {
                items['enter'].disabled = true;
                items['complementary'].disabled = true;
                if (oCalendar.isScheduleActive) items['assignment'].disabled = true;
                items['pasteShift'].disabled = true;
                items['advPaste'].disabled = true;
            } else {
                if (oCalendar.selectionCopied) {
                    items['removeShift'].disabled = true;
                    items['enter'].disabled = true;
                    items['complementary'].disabled = true;
                    if (oCalendar.isScheduleActive) items['assignment'].disabled = true;
                    items['vacation'].disabled = true;
                    items['copyShift'].disabled = true;
                    items['copyHolidays'].disabled = true;
                    items['copyWorking'].disabled = true;
                    items['block'].disabled = true;
                    items['removeblock'].disabled = true;

                    if (oCalendar.isScheduleActive) items['copyAssignments'].disabled = true;

                    if (oCalendar.selectedMinRow != oCalendar.selectedMaxRow) items['advPaste'].disabled = true;
                } else {
                    if (currentDayShift == null) items['removeShift'].disabled = true;

                    if (!this.initialPeriodHasHolidays(null)) {
                        items['copyHolidays'].disabled = true;
                    }
                }
            }
        }
    } else {
        items = {
            'enter': { name: oCalendar.translator.translate(Robotics.Client.Language.Tags.ContextMenu_Details), disabled: false, icon: 'detail' }
        };
    }
    return items;
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.initialPeriodHasHolidays = function (container) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    var hasHolidays = false;

    if (oCalendar.selectedMinRow > -1 && oCalendar.selectedMinColumn > -1) {
        for (var i = 0; i <= (oCalendar.selectedMaxRow - oCalendar.selectedMinRow); i++) {
            for (var x = 0; x <= (oCalendar.selectedMaxColumn - oCalendar.selectedMinColumn); x++) {
                var originCellRow = oCalendar.selectedMinRow + i;
                var originCellColumn = oCalendar.selectedMinColumn + x;

                var dayData = oCalData.CalendarData[originCellRow].PeriodData.DayData[originCellColumn];

                if (dayData.IsHoliday) {
                    hasHolidays = true;
                    break;
                }
            }

            if (hasHolidays) break;
        }
    } else {
        hasHolidays = oCalendar.selectedDay.IsHoliday;
    }
    return hasHolidays;
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.executeContextMenuAction = function (key, container) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    switch (key) {
        case "enter":
            this.enterDetailAction(oCalendar.selectedEmployee.EmployeeData.IDGroup, oCalendar.selectedEmployee.EmployeeData.IDEmployee, moment(oCalendar.selectedDay.PlanDate).format('DD/MM/YYYY'));
            break;
        case "complementary":
            this.editShiftInfo(container);
            break;
        case "assignment":
            this.editAssignmentsInfo(container);
            break;
        case "vacation":
            this.removeHolidays(container);
            break;
        case "copyWorking":
            this.copySelection(container, true, false, false);
            break;
        case "copyHolidays":
            this.copySelection(container, false, true, false);
            break;
        case "copyShift":
            this.copySelection(container, true, true, true);
            break;
        case "copyAssignments":
            this.copySelection(container, false, false, true);
            break;
        case "pasteShift":
            this.pasteSelection(container);
            break;
        case "pasteTelecommute":
            this.pasteTelecommuteSelection(container);
            break;
        case "block":
            this.lockUnlockSelectedDays(container, true);
            break;
        case "removeblock":
            this.lockUnlockSelectedDays(container, false);
            break;
        case "cancelSelection":
            this.cancelCurrentMultipleSelect(container, true);
            break;
        case "advPaste":
            this.advancedPastePrepare(container, true);
            break;
        case 'addFeast':
            this.assignRemoveFeastDays(container, true);
            break;
        case 'removeFeast':
            this.assignRemoveFeastDays(container, false);
            break;
        case 'removeShift':
            this.removeCurrentDayShift(container);
            break;
        case 'removeBudget':
            this.removeCurrentDayAssignedBudget(container);
            break;
        case 'setTelecommuteOffice':
            this.assignTelecommuteDays(container, false, false, false);
            break;
        case 'setTelecommuteHome':
            this.assignTelecommuteDays(container, false, false, true);
            break;
        case 'setTelecommuteOptional':
            this.assignTelecommuteDays(container, false, true, false);
            break;
        case 'setTelecommuteLabAgree':
            this.assignTelecommuteDays(container, true, false, false);
            break;
    }
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.removeCurrentDayShift = function (container) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    var bAnyDayUpdated = false;
    if (!oCalendar.isBatchMode()) {
        oCalendar.selectedMaxRow = oCalendar.selectedMinRow = parseInt($(container).attr('data-IDRow'), 10);
        oCalendar.selectedMaxColumn = oCalendar.selectedMinColumn = parseInt($(container).attr('data-IDColumn'), 10);
    }

    for (var i = 0; i <= (oCalendar.selectedMaxRow - oCalendar.selectedMinRow); i++) {
        for (var x = 0; x <= (oCalendar.selectedMaxColumn - oCalendar.selectedMinColumn); x++) {
            var originCellRow = oCalendar.selectedMinRow + i;
            var originCellColumn = oCalendar.selectedMinColumn + x;

            var bAssign = true;
            var dayData = null;
            var freezingDate = oCalData.CalendarData[originCellRow].EmployeeData.FreezingDate;
            if (oCalendar.viewRange != Robotics.Client.Constants.ViewRange.Daily) {
                dayData = oCalData.CalendarData[originCellRow].PeriodData.DayData[originCellColumn];
            } else {
                dayData = oCalData.CalendarData[originCellRow].PeriodData.DayData[0];
            }

            if (dayData != null) {
                if (!dayData.CanBeModified) {
                    if (!oCalendar.isBatchMode()) oCalendar.showErrorPopup("Error.Title", "error", "Calendar.Client.NoPermission", "", "Error.OK", "Error.OKDesc", oCalendar.defaultMessageAction);
                    bAssign = false;
                } else if (moment(dayData.PlanDate) <= moment(freezingDate)) {
                    if (!oCalendar.isBatchMode()) oCalendar.showErrorPopup("Error.Title", "error", "Calendar.Client.FreezeDate", "", "Error.OK", "Error.OKDesc", oCalendar.defaultMessageAction);
                    bAssign = false;
                } else if (dayData.Locked) {
                    if (!oCalendar.isBatchMode()) oCalendar.showErrorPopup("Error.Title", "error", "Calendar.Client.LockedDay", "", "Error.OK", "Error.OKDesc", oCalendar.defaultMessageAction);
                    bAssign = false;
                } else if (dayData.IDDailyBudgetPosition > 0) {
                    if (!oCalendar.isBatchMode()) oCalendar.showErrorPopup("Error.Title", "error", "Calendar.Client.BudgetAssigned", "", "Error.OK", "Error.OKDesc", oCalendar.defaultMessageAction);
                    bAssign = false;
                }

                if (bAssign) {
                    bAnyDayUpdated = true;
                    var currentDayShift = this.getAssignedShift(dayData);
                    var oldShiftCopy = Object.clone(currentDayShift, true);

                    dayData.AssigData = null;
                    dayData.ShiftBase = null;
                    dayData.MainShift = null;
                    dayData.IsHoliday = false;
                    dayData.HasChanged = true;

                    if (oCalendar.viewRange != Robotics.Client.Constants.ViewRange.Daily) {
                        var destDiv = $('#' + oCalendar.ascxPrefix + '_calCell_' + originCellRow + '_' + originCellColumn);
                        destDiv.empty();
                        this.buildCalendarCellContent(destDiv, dayData, false, freezingDate);
                    } else {
                        this.refreshDailySelectedData(true);
                    }

                    this.updateShiftTotalizerInfo(oldShiftCopy, null, originCellRow, originCellColumn, dayData, false);
                }
            }
            if (bAssign) {
                oCalendar.setHasChanges(true);
                this.waitForUserToRefresh();
            }
        }
    }

    //if (bAnyDayUpdated) oCalendar.mapEvents();

    if (!oCalendar.isBatchMode()) {
        oCalendar.selectedMaxRow = oCalendar.selectedMinRow = -1;
        oCalendar.selectedMaxColumn = oCalendar.selectedMinColumn = -1;
    }
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.removeCurrentDayAssignedBudget = function (container) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    if (!oCalendar.isBatchMode()) {
        oCalendar.selectedMaxRow = oCalendar.selectedMinRow = parseInt($(container).attr('data-IDRow'), 10);
        oCalendar.selectedMaxColumn = oCalendar.selectedMinColumn = parseInt($(container).attr('data-IDColumn'), 10);
    }

    for (var i = 0; i <= (oCalendar.selectedMaxRow - oCalendar.selectedMinRow); i++) {
        for (var x = 0; x <= (oCalendar.selectedMaxColumn - oCalendar.selectedMinColumn); x++) {
            var originCellRow = oCalendar.selectedMinRow + i;
            var originCellColumn = oCalendar.selectedMinColumn + x;

            var bAssign = true;
            var dayData = null;
            var freezingDate = oCalData.CalendarData[originCellRow].EmployeeData.FreezingDate;

            if (oCalendar.viewRange != Robotics.Client.Constants.ViewRange.Daily) {
                dayData = oCalData.CalendarData[originCellRow].PeriodData.DayData[originCellColumn];
            } else {
                dayData = oCalData.CalendarData[originCellRow].PeriodData.DayData[0];
            }

            if (dayData != null) {
                if (!dayData.CanBeModified) {
                    if (!oCalendar.isBatchMode()) oCalendar.showErrorPopup("Error.Title", "error", "Calendar.Client.NoPermission", "", "Error.OK", "Error.OKDesc", oCalendar.defaultMessageAction);
                    bAssign = false;
                } else if (moment(dayData.PlanDate) <= moment(freezingDate)) {
                    if (!oCalendar.isBatchMode()) oCalendar.showErrorPopup("Error.Title", "error", "Calendar.Client.FreezeDate", "", "Error.OK", "Error.OKDesc", oCalendar.defaultMessageAction);
                    bAssign = false;
                } else if (dayData.Locked) {
                    if (!oCalendar.isBatchMode()) oCalendar.showErrorPopup("Error.Title", "error", "Calendar.Client.LockedDay", "", "Error.OK", "Error.OKDesc", oCalendar.defaultMessageAction);
                    bAssign = false;
                }

                if (bAssign) {
                    dayData.IDDailyBudgetPosition = 0;
                    dayData.HasChanged = true;

                    if (oCalendar.viewRange != Robotics.Client.Constants.ViewRange.Daily) {
                        var destDiv = $('#' + oCalendar.ascxPrefix + '_calCell_' + originCellRow + '_' + originCellColumn);
                        destDiv.empty();
                        this.buildCalendarCellContent(destDiv, dayData, false, freezingDate);
                    } else {
                        this.refreshDailySelectedData(true);
                    }
                }
            }
            if (bAssign) oCalendar.setHasChanges(true);
        }
    }

    if (!oCalendar.isBatchMode()) {
        oCalendar.selectedMaxRow = oCalendar.selectedMinRow = -1;
        oCalendar.selectedMaxColumn = oCalendar.selectedMinColumn = -1;
    }
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.advancedPastePrepare = function (container) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    if (oCalendar.hasChanges == false) {
        oCalendar.advCopyManager.prepareForm((oCalendar.selectedMaxColumn - oCalendar.selectedMinColumn + 1), oCalendar.selectedDay.PlanDate, Robotics.Client.Constants.TypeView.Planification);
        oCalendar.advCopyDialog.dialog("open");
    } else {
        oCalendar.showChangesWarning(oCalendar.clientInstanceName + ".advancedPastePrepare();");
    }
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.cancelCurrentMultipleSelect = function (sender, forceCancel) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    if (!oCalendar.selectionCopied || forceCancel) {
        oCalendar.selectionCopied = false;
        var selectedItems = $('.calendarOuterBodyCell.ui-selected,.calendarDailyBodyCell.ui-selected');

        for (var i = 0; i < selectedItems.length; i++) {
            $(selectedItems[i]).removeClass('ui-selected');
        }

        oCalendar.selectedMinRow = -1;
        oCalendar.selectedMaxRow = -1;

        oCalendar.selectedMinColumn = -1;
        oCalendar.selectedMaxColumn = -1;

        oCalendar.copyWorkingShifts = false;
        oCalendar.copyHolidaysShifts = false;
        oCalendar.copyAssignmentsShifts = false;
    }
}

Robotics.Client.Controls.roSchedulerCalendar.prototype.cancelCurrentMultipleHeaderSelect = function (sender, forceCancel) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    if (!oCalendar.selectionHeaderCopied || forceCancel) {
        oCalendar.selectionHeaderCopied = false;

        var selectedItems = $('.CalendarDayFixedHeader.ui-selected');

        for (var i = 0; i < selectedItems.length; i++) {
            $(selectedItems[i]).removeClass('ui-selected');
        }

        oCalendar.selectedMinHeaderColumn = -1;
        oCalendar.selectedMaxHeaderColumn = -1;
    }
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.lockUnlockSelectedDays = function (container, lockDay) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    if (!oCalendar.isBatchMode()) {
        oCalendar.selectedMaxRow = oCalendar.selectedMinRow = parseInt($(container).attr('data-IDRow'), 10);
        oCalendar.selectedMaxColumn = oCalendar.selectedMinColumn = parseInt($(container).attr('data-IDColumn'), 10);
    }

    for (var i = 0; i <= (oCalendar.selectedMaxRow - oCalendar.selectedMinRow); i++) {
        for (var x = 0; x <= (oCalendar.selectedMaxColumn - oCalendar.selectedMinColumn); x++) {
            var originCellRow = oCalendar.selectedMinRow + i;
            var originCellColumn = oCalendar.selectedMinColumn + x;

            var bAssign = true;

            if (oCalendar.viewRange != Robotics.Client.Constants.ViewRange.Daily) {
                var dayData = oCalData.CalendarData[originCellRow].PeriodData.DayData[originCellColumn];
                var freezingDate = oCalData.CalendarData[originCellRow].EmployeeData.FreezingDate;

                if (!dayData.CanBeModified) {
                    if (!oCalendar.isBatchMode()) oCalendar.showErrorPopup("Error.Title", "error", "Calendar.Client.NoPermission", "", "Error.OK", "Error.OKDesc", oCalendar.defaultMessageAction);
                    bAssign = false;
                } else if (moment(dayData.PlanDate) <= moment(freezingDate)) {
                    if (!oCalendar.isBatchMode()) oCalendar.showErrorPopup("Error.Title", "error", "Calendar.Client.FreezeDate", "", "Error.OK", "Error.OKDesc", oCalendar.defaultMessageAction);
                    bAssign = false;
                }

                if (bAssign) {
                    dayData.Locked = lockDay;
                    dayData.HasChanged = true;

                    var destDiv = $('#' + oCalendar.ascxPrefix + '_calCell_' + originCellRow + '_' + originCellColumn);
                    destDiv.empty();
                    this.buildCalendarCellContent(destDiv, dayData, false, freezingDate);
                }
            } else {
                var dayData = oCalData.CalendarData[originCellRow].PeriodData.DayData[0];
                var freezingDate = oCalData.CalendarData[originCellRow].EmployeeData.FreezingDate;

                if (!dayData.CanBeModified) {
                    if (!oCalendar.isBatchMode()) oCalendar.showErrorPopup("Error.Title", "error", "Calendar.Client.NoPermission", "", "Error.OK", "Error.OKDesc", oCalendar.defaultMessageAction);
                    bAssign = false;
                } else if (moment(dayData.PlanDate) <= moment(freezingDate)) {
                    if (!oCalendar.isBatchMode()) oCalendar.showErrorPopup("Error.Title", "error", "Calendar.Client.FreezeDate", "", "Error.OK", "Error.OKDesc", oCalendar.defaultMessageAction);
                    bAssign = false;
                }

                if (bAssign) {
                    dayData.Locked = lockDay;
                    dayData.HasChanged = true;
                    this.refreshDailySelectedData(true);
                }
            }

            if (bAssign) oCalendar.setHasChanges(true);
        }
    }

    if (!oCalendar.isBatchMode()) {
        oCalendar.selectedMaxRow = oCalendar.selectedMinRow = -1;
        oCalendar.selectedMaxColumn = oCalendar.selectedMinColumn = -1;
    }
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.assignTelecommuteDays = function (container, bSetDefaultValue, bSetAsOptional, bSetTelecommute) {
    var oClientMode = this;
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    this.copyInProgress = true;

    if (!oCalendar.isBatchMode()) {
        oCalendar.selectedMaxRow = oCalendar.selectedMinRow = parseInt($(container).attr('data-IDRow'), 10);
        oCalendar.selectedMaxColumn = oCalendar.selectedMinColumn = parseInt($(container).attr('data-IDColumn'), 10);
    }

    for (var i = 0; i <= (oCalendar.selectedMaxRow - oCalendar.selectedMinRow); i++) {
        for (var x = 0; x <= (oCalendar.selectedMaxColumn - oCalendar.selectedMinColumn); x++) {
            var originCellRow = oCalendar.selectedMinRow + i;
            var originCellColumn = oCalendar.selectedMinColumn + x;

            var bAssign = true;

            if (oCalendar.viewRange != Robotics.Client.Constants.ViewRange.Daily) {
                var dayData = oCalData.CalendarData[originCellRow].PeriodData.DayData[originCellColumn];
                var freezingDate = oCalData.CalendarData[originCellRow].EmployeeData.FreezingDate;

                if (!dayData.CanBeModified) {
                    if (!oCalendar.isBatchMode()) oCalendar.showErrorPopup("Error.Title", "error", "Calendar.Client.NoPermission", "", "Error.OK", "Error.OKDesc", oCalendar.defaultMessageAction);
                    bAssign = false;
                } else if (moment(dayData.PlanDate) <= moment(freezingDate)) {
                    if (!oCalendar.isBatchMode()) oCalendar.showErrorPopup("Error.Title", "error", "Calendar.Client.FreezeDate", "", "Error.OK", "Error.OKDesc", oCalendar.defaultMessageAction);
                    bAssign = false;
                } else if (dayData.Locked) {
                    if (!oCalendar.isBatchMode()) oCalendar.showErrorPopup("Error.Title", "error", "Calendar.Client.LockedDay", "", "Error.OK", "Error.OKDesc", oCalendar.defaultMessageAction);
                    bAssign = false;
                }

                if (bAssign) {
                    var bInitialTelecommute = dayData.TelecommutingExpected;

                    if (!bSetDefaultValue && !bSetAsOptional) {
                        dayData.TelecommutingExpected = bSetTelecommute;
                        dayData.TelecommuteForced = true;
                        dayData.TelecommutingOptional = false;
                    } else if (bSetAsOptional) {
                        dayData.TelecommuteForced = true;
                        dayData.TelecommutingExpected = false;
                        dayData.TelecommutingOptional = true;
                    } else {
                        if (typeof dayData.PresenceMandatoryDays != 'undefined') {
                            if (dayData.PresenceMandatoryDays.indexOf(dayData.PlanDate.getDay() + '') >= 0) {
                                dayData.TelecommutingExpected = false;
                            } else if (dayData.TelecommutingOptionalDays.indexOf(dayData.PlanDate.getDay() + '') >= 0) {
                                dayData.TelecommutingExpected = false;
                            } else if (dayData.TelecommutingMandatoryDays.indexOf(dayData.PlanDate.getDay() + '') >= 0) {
                                dayData.TelecommutingExpected = true;
                            } else {
                                dayData.TelecommutingExpected = false;
                            }
                            dayData.TelecommutingOptional = false;
                            dayData.TelecommuteForced = false;
                        } else {
                            dayData.TelecommutingExpected = false;
                            dayData.TelecommutingOptional = false;
                            dayData.TelecommuteForced = false;
                        }
                    }
                    dayData.HasChanged = true;

                    var destDiv = $('#' + oCalendar.ascxPrefix + '_calCell_' + originCellRow + '_' + originCellColumn);
                    destDiv.empty();
                    this.buildCalendarCellContent(destDiv, dayData, false, freezingDate);

                    var currentDayShift = this.getAssignedShift(dayData);
                    var oldShiftCopy = Object.clone(currentDayShift, true);
                    oClientMode.updateShiftTotalizerInfo(oldShiftCopy, oldShiftCopy, originCellRow, originCellColumn, dayData, (bInitialTelecommute != dayData.TelecommutingExpected));
                }
            } else {
                var dayData = oCalData.CalendarData[originCellRow].PeriodData.DayData[0];
                var freezingDate = oCalData.CalendarData[originCellRow].EmployeeData.FreezingDate;

                if (!dayData.CanBeModified) {
                    if (!oCalendar.isBatchMode()) oCalendar.showErrorPopup("Error.Title", "error", "Calendar.Client.NoPermission", "", "Error.OK", "Error.OKDesc", oCalendar.defaultMessageAction);
                    bAssign = false;
                } else if (moment(dayData.PlanDate) <= moment(freezingDate)) {
                    if (!oCalendar.isBatchMode()) oCalendar.showErrorPopup("Error.Title", "error", "Calendar.Client.FreezeDate", "", "Error.OK", "Error.OKDesc", oCalendar.defaultMessageAction);
                    bAssign = false;
                } else if (dayData.Locked) {
                    if (!oCalendar.isBatchMode()) oCalendar.showErrorPopup("Error.Title", "error", "Calendar.Client.LockedDay", "", "Error.OK", "Error.OKDesc", oCalendar.defaultMessageAction);
                    bAssign = false;
                }

                if (bAssign) {
                    var bInitialTelecommute = dayData.TelecommutingExpected;

                    if (!bSetDefaultValue && !bSetAsOptional) {
                        dayData.TelecommutingExpected = bSetTelecommute;
                        dayData.TelecommuteForced = true;
                        dayData.TelecommutingOptional = false;
                    } else if (bSetAsOptional) {
                        dayData.TelecommuteForced = true;
                        dayData.TelecommutingExpected = false;
                        dayData.TelecommutingOptional = true;
                    } else {
                        if (typeof dayData.PresenceMandatoryDays != 'undefined') {
                            if (dayData.PresenceMandatoryDays.indexOf(dayData.PlanDate.getDay() + '') >= 0) {
                                dayData.TelecommutingExpected = false;
                            } else if (dayData.TelecommutingOptionalDays.indexOf(dayData.PlanDate.getDay() + '') >= 0) {
                                dayData.TelecommutingExpected = false;
                            } else if (dayData.TelecommutingMandatoryDays.indexOf(dayData.PlanDate.getDay() + '') >= 0) {
                                dayData.TelecommutingExpected = true;
                            } else {
                                dayData.TelecommutingExpected = false;
                            }
                            dayData.TelecommutingOptional = false;
                            dayData.TelecommuteForced = false;
                        } else {
                            dayData.TelecommutingExpected = false;
                            dayData.TelecommutingOptional = false;
                            dayData.TelecommuteForced = false;
                        }
                    }
                    dayData.HasChanged = true;

                    this.refreshDailySelectedData(true);

                    var currentDayShift = this.getAssignedShift(dayData);
                    var oldShiftCopy = Object.clone(currentDayShift, true);
                    oClientMode.updateShiftTotalizerInfo(oldShiftCopy, oldShiftCopy, originCellRow, 0, dayData, (bInitialTelecommute != dayData.TelecommutingExpected));
                }
            }

            if (bAssign) {
                oCalendar.setHasChanges(true);
                this.waitForUserToRefresh();
            }
        }
    }

    if (oCalendar.loadIndictments) oClientMode.checkCalendarIndictments();
    this.copyInProgress = false;

    if (!oCalendar.isBatchMode()) {
        oCalendar.selectedMaxRow = oCalendar.selectedMinRow = -1;
        oCalendar.selectedMaxColumn = oCalendar.selectedMinColumn = -1;
    }
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.assignRemoveFeastDays = function (container, bAddFeast) {
    var oClientMode = this;
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    this.copyInProgress = true;

    if (!oCalendar.isBatchMode()) {
        oCalendar.selectedMaxRow = oCalendar.selectedMinRow = parseInt($(container).attr('data-IDRow'), 10);
        oCalendar.selectedMaxColumn = oCalendar.selectedMinColumn = parseInt($(container).attr('data-IDColumn'), 10);
    }

    for (var i = 0; i <= (oCalendar.selectedMaxRow - oCalendar.selectedMinRow); i++) {
        for (var x = 0; x <= (oCalendar.selectedMaxColumn - oCalendar.selectedMinColumn); x++) {
            var originCellRow = oCalendar.selectedMinRow + i;
            var originCellColumn = oCalendar.selectedMinColumn + x;

            var bAssign = true;

            if (oCalendar.viewRange != Robotics.Client.Constants.ViewRange.Daily) {
                var dayData = oCalData.CalendarData[originCellRow].PeriodData.DayData[originCellColumn];
                var freezingDate = oCalData.CalendarData[originCellRow].EmployeeData.FreezingDate;

                if (!dayData.CanBeModified) {
                    if (!oCalendar.isBatchMode()) oCalendar.showErrorPopup("Error.Title", "error", "Calendar.Client.NoPermission", "", "Error.OK", "Error.OKDesc", oCalendar.defaultMessageAction);
                    bAssign = false;
                } else if (moment(dayData.PlanDate) <= moment(freezingDate)) {
                    if (!oCalendar.isBatchMode()) oCalendar.showErrorPopup("Error.Title", "error", "Calendar.Client.FreezeDate", "", "Error.OK", "Error.OKDesc", oCalendar.defaultMessageAction);
                    bAssign = false;
                } else if (dayData.Locked) {
                    if (!oCalendar.isBatchMode()) oCalendar.showErrorPopup("Error.Title", "error", "Calendar.Client.LockedDay", "", "Error.OK", "Error.OKDesc", oCalendar.defaultMessageAction);
                    bAssign = false;
                }

                if (bAssign) {
                    dayData.Feast = bAddFeast;
                    dayData.HasChanged = true;

                    var destDiv = $('#' + oCalendar.ascxPrefix + '_calCell_' + originCellRow + '_' + originCellColumn);
                    destDiv.empty();
                    this.buildCalendarCellContent(destDiv, dayData, false, freezingDate);
                }
            } else {
                var dayData = oCalData.CalendarData[originCellRow].PeriodData.DayData[0];
                var freezingDate = oCalData.CalendarData[originCellRow].EmployeeData.FreezingDate;

                if (!dayData.CanBeModified) {
                    if (!oCalendar.isBatchMode()) oCalendar.showErrorPopup("Error.Title", "error", "Calendar.Client.NoPermission", "", "Error.OK", "Error.OKDesc", oCalendar.defaultMessageAction);
                    bAssign = false;
                } else if (moment(dayData.PlanDate) <= moment(freezingDate)) {
                    if (!oCalendar.isBatchMode()) oCalendar.showErrorPopup("Error.Title", "error", "Calendar.Client.FreezeDate", "", "Error.OK", "Error.OKDesc", oCalendar.defaultMessageAction);
                    bAssign = false;
                } else if (dayData.Locked) {
                    if (!oCalendar.isBatchMode()) oCalendar.showErrorPopup("Error.Title", "error", "Calendar.Client.LockedDay", "", "Error.OK", "Error.OKDesc", oCalendar.defaultMessageAction);
                    bAssign = false;
                }

                if (bAssign) {
                    dayData.Feast = bAddFeast;
                    dayData.HasChanged = true;
                    this.refreshDailySelectedData(true);
                }
            }

            if (bAssign) {
                oCalendar.setHasChanges(true);
            }
        }
    }

    if (oCalendar.loadIndictments) oClientMode.checkCalendarIndictments();
    this.copyInProgress = false;

    if (!oCalendar.isBatchMode()) {
        oCalendar.selectedMaxRow = oCalendar.selectedMinRow = -1;
        oCalendar.selectedMaxColumn = oCalendar.selectedMinColumn = -1;
    }
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.editAssignmentsInfo = function (container) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    var clickedRow = parseInt($(container).attr('data-IDRow'), 10);
    var clickedColumn = parseInt($(container).attr('data-IDColumn'), 10);

    var currentDayShift = this.getAssignedShift(oCalendar.selectedDay);
    if (currentDayShift != null && oCalendar.selectedDay.AllowAssignment == true) {
        oCalendar.assignmentShift = currentDayShift;

        if (typeof (oCalendar.shiftsExtendedDataCache[currentDayShift.ID]) == 'undefined') {
            oCalendar.showAssignmentsDialog = true;
            var oParameters = {};
            oParameters.idShift = currentDayShift.ID;
            oParameters.StampParam = new Date().getMilliseconds();
            oCalendar.performAction(oParameters, Robotics.Client.Constants.Actions.ShiftLayerDefinitionEdit);
        } else {
            this.editAssignmentsInfoFinally(oCalendar.shiftsExtendedDataCache[currentDayShift.ID]);
        }
    }
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.editAssignmentsInfoFinally = function (oCalendarShift) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    if (oCalendarShift != null) {
        oCalendar.shiftsExtendedDataCache[oCalendar.assignmentShift.ID] = oCalendarShift;
    }

    oCalendar.assignmentsManager.prepareAssignmentsDialog(oCalendarShift, oCalendar.selectedEmployee.EmployeeData.Assignments, -1);
    oCalendar.assignmentsManager.selectcurrentAssignment(oCalendar.selectedDay.AssigData);

    oCalendar.assignmentsDialog.dialog("open");

    var oCal = this;
    setTimeout(function () { oCal.assignmentsManager.focusDialog(); }, 100);
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.editShiftInfo = function (container) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    var clickedRow = parseInt($(container).attr('data-IDRow'), 10);
    var clickedColumn = parseInt($(container).attr('data-IDColumn'), 10);

    var currentDayShift = this.getAssignedShift(oCalendar.selectedDay);

    var isStarterShift = false;
    if (currentDayShift.AdvancedParameters.find(function (x) { return x.Name.toLowerCase() == 'starter' && x.Value == '1' }) != null) {
        isStarterShift = true;
    }

    if (isStarterShift) {
        if (typeof (oCalendar.shiftsExtendedDataCache[currentDayShift.ID]) == 'undefined') {
            oCalendar.showStarterDialog = true;
            var oParameters = {};
            oParameters.idShift = currentDayShift.ID;
            oParameters.StampParam = new Date().getMilliseconds();
            oCalendar.performAction(oParameters, Robotics.Client.Constants.Actions.ShiftLayerDefinitionEdit);
        } else {
            this.editStarterInfoFinally(oCalendar.shiftsExtendedDataCache[currentDayShift.ID], currentDayShift);
        }
    } else {
        this.editComplementaryInfo(container);
    }
}

Robotics.Client.Controls.roSchedulerCalendar.prototype.editStarterInfoFinally = function (oCalendarShift, currentDayShift) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    var originShift = null;
    oCalendar.showComplementaryAssignDialog = false;

    if (typeof (currentDayShift) != 'undefined') {
        originShift = currentDayShift;
    } else {
        originShift = this.getAssignedShift(oCalendar.selectedDay);
    }

    oCalendar.starterManager.prepareStarterDialog(oCalendarShift, -1, originShift);
    oCalendar.starterDialog.dialog("open");
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.editComplementaryInfo = function (container) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    var clickedRow = parseInt($(container).attr('data-IDRow'), 10);
    var clickedColumn = parseInt($(container).attr('data-IDColumn'), 10);

    var currentDayShift = this.getAssignedShift(oCalendar.selectedDay);
    if (currentDayShift != null && (currentDayShift.Type == Robotics.Client.Constants.ShiftType.NormalFloating || currentDayShift.ExistComplementaryData || currentDayShift.ExistFloatingData)) {
        if (typeof (oCalendar.shiftsExtendedDataCache[currentDayShift.ID]) == 'undefined') {
            oCalendar.showComplementaryAssignDialog = true;
            var oParameters = {};
            oParameters.idShift = currentDayShift.ID;
            oParameters.StampParam = new Date().getMilliseconds();
            oCalendar.performAction(oParameters, Robotics.Client.Constants.Actions.ShiftLayerDefinitionEdit);
        } else {
            this.editComplementaryInfoFinally(oCalendar.shiftsExtendedDataCache[currentDayShift.ID], currentDayShift);
        }
    }
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.editComplementaryInfoFinally = function (oCalendarShift, currentDayShift) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    var originShift = null;
    oCalendar.showComplementaryAssignDialog = false;

    if (typeof (currentDayShift) != 'undefined') {
        originShift = currentDayShift;
    } else {
        originShift = this.getAssignedShift(oCalendar.selectedDay);
    }

    oCalendar.complementaryManager.prepareComplementaryDialog(oCalendarShift, -1, originShift);
    oCalendar.complementaryDialog.dialog("open");
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.pasteSelection = function (container) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    var clickedRow = parseInt($(container).attr('data-IDRow'), 10);
    var clickedColumn = parseInt($(container).attr('data-IDColumn'), 10);

    var bHasBloquedDays = this.endPeriodHasBloquedDays(container);
    var bHasEndHolidays = this.endPeriodHasHolidays(container);

    if (!bHasBloquedDays && !bHasEndHolidays) {
        this.pasteSelectionEnd(clickedRow, clickedColumn, oCalendar.copyWorkingShifts, oCalendar.copyHolidaysShifts, oCalendar.copyAssignmentsShifts, true, true);
    } else {
        if (!bHasBloquedDays) $('#' + oCalendar.ascxPrefix + '_dlgCopy_advCopyKeepBloqued').css('display', 'none');
        else $('#' + oCalendar.ascxPrefix + '_dlgCopy_advCopyKeepBloqued').css('display', '');

        if (!bHasEndHolidays) $('#' + oCalendar.ascxPrefix + '_dlgCopy_advCopyKeepHolidays').css('display', 'none');
        else $('#' + oCalendar.ascxPrefix + '_dlgCopy_advCopyKeepHolidays').css('display', '');

        if (!bHasBloquedDays && !bHasEndHolidays) $('#' + oCalendar.ascxPrefix + '_dlgCopy_advCopyKeepObjects').css('display', 'none');
        else $('#' + oCalendar.ascxPrefix + '_dlgCopy_advCopyKeepObjects').css('display', '');

        eval(oCalendar.ascxPrefix + '_dlgCopy_ckSPKeepBloquedDaysClient').SetChecked(true);
        eval(oCalendar.ascxPrefix + '_dlgCopy_ckSPKeepHolidaysClient').SetChecked(true);

        oCalendar.sourceDialogRow = clickedRow;
        oCalendar.sourceDialogColumn = clickedColumn;

        oCalendar.copyDialog.dialog("open");
    }
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.pasteTelecommuteSelection = function (container) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    var clickedRow = parseInt($(container).attr('data-IDRow'), 10);
    var clickedColumn = parseInt($(container).attr('data-IDColumn'), 10);

    var bHasBloquedDays = this.endPeriodHasBloquedDays(container);
    var bHasEndHolidays = this.endPeriodHasHolidays(container);

    if (!bHasBloquedDays && !bHasEndHolidays) {
        this.pasteTelecommuteSelectionEnd(clickedRow, clickedColumn, oCalendar.copyWorkingShifts, oCalendar.copyHolidaysShifts, oCalendar.copyAssignmentsShifts, true, true);
    } else {
        if (!bHasBloquedDays) $('#' + oCalendar.ascxPrefix + '_dlgCopy_advCopyKeepBloqued').css('display', 'none');
        else $('#' + oCalendar.ascxPrefix + '_dlgCopy_advCopyKeepBloqued').css('display', '');

        if (!bHasEndHolidays) $('#' + oCalendar.ascxPrefix + '_dlgCopy_advCopyKeepHolidays').css('display', 'none');
        else $('#' + oCalendar.ascxPrefix + '_dlgCopy_advCopyKeepHolidays').css('display', '');

        if (!bHasBloquedDays && !bHasEndHolidays) $('#' + oCalendar.ascxPrefix + '_dlgCopy_advCopyKeepObjects').css('display', 'none');
        else $('#' + oCalendar.ascxPrefix + '_dlgCopy_advCopyKeepObjects').css('display', '');

        eval(oCalendar.ascxPrefix + '_dlgCopy_ckSPKeepBloquedDaysClient').SetChecked(true);
        eval(oCalendar.ascxPrefix + '_dlgCopy_ckSPKeepHolidaysClient').SetChecked(true);

        oCalendar.sourceDialogRow = clickedRow;
        oCalendar.sourceDialogColumn = clickedColumn;

        oCalendar.copyDialog.dialog("open");
    }
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.endPeriodHasBloquedDays = function (container) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    var hasBoquedDays = false;

    var clickedRow = parseInt($(container).attr('data-IDRow'), 10);
    var clickedColumn = parseInt($(container).attr('data-IDColumn'), 10);

    for (var i = 0; i <= (oCalendar.selectedMaxRow - oCalendar.selectedMinRow); i++) {
        for (var x = 0; x <= (oCalendar.selectedMaxColumn - oCalendar.selectedMinColumn); x++) {
            var destinationCellRow = clickedRow + i;
            var destinationCellColumn = clickedColumn + x;

            if (destinationCellRow < oCalData.CalendarData.length && destinationCellColumn < oCalData.CalendarData[destinationCellRow].PeriodData.DayData.length) {
                var dayData = oCalData.CalendarData[destinationCellRow].PeriodData.DayData[destinationCellColumn];
                if (dayData.Locked) {
                    hasBoquedDays = true;
                    break;
                }
            }
        }
        if (hasBoquedDays) break;
    }
    return hasBoquedDays;
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.endPeriodHasHolidays = function (container) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    var hasHolidays = false;

    var clickedRow = parseInt($(container).attr('data-IDRow'), 10);
    var clickedColumn = parseInt($(container).attr('data-IDColumn'), 10);

    for (var i = 0; i <= (oCalendar.selectedMaxRow - oCalendar.selectedMinRow); i++) {
        for (var x = 0; x <= (oCalendar.selectedMaxColumn - oCalendar.selectedMinColumn); x++) {
            var destinationCellRow = clickedRow + i;
            var destinationCellColumn = clickedColumn + x;

            if (destinationCellRow < oCalData.CalendarData.length && destinationCellColumn < oCalData.CalendarData[destinationCellRow].PeriodData.DayData.length) {
                var dayData = oCalData.CalendarData[destinationCellRow].PeriodData.DayData[destinationCellColumn];
                if (dayData.IsHoliday) {
                    hasHolidays = true;
                    break;
                }
            }
        }
        if (hasHolidays) break;
    }
    return hasHolidays;
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.pasteSelectionEnd = function (clickedRow, clickedColumn, copyMainShifts, copyHolidays, copyAssignments, keepLocked, keepDesHolidays) {
    var oClientMode = this;
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    this.copyInProgress = true;
    for (var i = 0; i <= (oCalendar.selectedMaxRow - oCalendar.selectedMinRow); i++) {
        for (var x = 0; x <= (oCalendar.selectedMaxColumn - oCalendar.selectedMinColumn); x++) {
            var originCellRow = oCalendar.selectedMinRow + i;
            var originCellColumn = oCalendar.selectedMinColumn + x;

            var destinationCellRow = clickedRow + i;
            var destinationCellColumn = clickedColumn + x;

            var dayData = null;

            if (oCalendar.viewRange != Robotics.Client.Constants.ViewRange.Daily) {
                dayData = oCalData.CalendarData[originCellRow].PeriodData.DayData[originCellColumn];
            } else {
                dayData = oCalData.CalendarData[originCellRow].PeriodData.DayData[0];
            }

            var ShiftToCopy = null;
            var holidayShiftToCopy = null;

            dayData.IsHoliday == true ? ShiftToCopy = dayData.ShiftBase : ShiftToCopy = this.getAssignedShift(dayData);

            if (dayData.IsHoliday) {
                ShiftToCopy = dayData.ShiftBase;
                holidayShiftToCopy = this.getAssignedShift(dayData);
            } else {
                ShiftToCopy = this.getAssignedShift(dayData);
                holidayShiftToCopy = null;
            }

            var bAssign = true;

            var destinationDay = null;
            if (oCalendar.viewRange != Robotics.Client.Constants.ViewRange.Daily) {
                if (destinationCellRow < oCalData.CalendarData.length && destinationCellColumn < oCalData.CalendarData[destinationCellRow].PeriodData.DayData.length) {
                    bAssign = true;
                    destinationDay = oCalData.CalendarData[destinationCellRow].PeriodData.DayData[destinationCellColumn];
                } else {
                    bAssign = false;
                }
            } else {
                if (destinationCellRow < oCalData.CalendarData.length) {
                    bAssign = true;
                    destinationDay = oCalData.CalendarData[destinationCellRow].PeriodData.DayData[0];
                } else {
                    bAssign = false;
                }
            }

            if (bAssign) {
                var mainShift = null;
                var holidayShift = null;
                var assignmentShift = null;
                var allowAssignment = false;

                if (copyMainShifts && ShiftToCopy != null) {
                    mainShift = {
                        ID: ShiftToCopy.ID,
                        ShortName: ShiftToCopy.ShortName,
                        PlannedHours: ShiftToCopy.PlannedHours,
                        Color: ShiftToCopy.Color,
                        Name: ShiftToCopy.Name,
                        Type: ShiftToCopy.Type,
                        StartHour: moment(ShiftToCopy.StartHour).clone().toDate(),
                        EndHour: moment(ShiftToCopy.EndHour).clone().toDate(),
                        AdvancedParameters: Object.clone(ShiftToCopy.AdvancedParameters, true),
                        BreakHours: ShiftToCopy.BreakHours,
                        ExistComplementaryData: ShiftToCopy.ExistComplementaryData,
                        ExistFloatingData: ShiftToCopy.ExistFloatingData,
                        ShiftLayers: ShiftToCopy.ShiftLayers,
                        ShiftLayersDefinition: []
                    };
                    for (var tmpLayerIndex = 0; tmpLayerIndex < mainShift.ShiftLayers; tmpLayerIndex++) {
                        mainShift.ShiftLayersDefinition.push({
                            LayerStartTime: moment(ShiftToCopy.ShiftLayersDefinition[tmpLayerIndex].LayerStartTime).clone().toDate(),
                            ExistLayerStartTime: ShiftToCopy.ShiftLayersDefinition[tmpLayerIndex].ExistLayerStartTime,
                            LayerOrdinaryHours: ShiftToCopy.ShiftLayersDefinition[tmpLayerIndex].LayerOrdinaryHours,
                            LayerDuration: ShiftToCopy.ShiftLayersDefinition[tmpLayerIndex].LayerDuration,
                            ExistLayerDuration: ShiftToCopy.ShiftLayersDefinition[tmpLayerIndex].ExistLayerDuration,
                            LayerID: ShiftToCopy.ShiftLayersDefinition[tmpLayerIndex].LayerID,
                            LayerComplementaryHours: ShiftToCopy.ShiftLayersDefinition[tmpLayerIndex].LayerComplementaryHours
                        });
                    }
                }

                if (copyHolidays && holidayShiftToCopy != null) {
                    holidayShift = {
                        ID: holidayShiftToCopy.ID,
                        ShortName: holidayShiftToCopy.ShortName,
                        PlannedHours: holidayShiftToCopy.PlannedHours,
                        Color: holidayShiftToCopy.Color,
                        Name: holidayShiftToCopy.Name,
                        Type: holidayShiftToCopy.Type,
                        StartHour: moment(holidayShiftToCopy.StartHour).clone().toDate(),
                        EndHour: moment(holidayShiftToCopy.EndHour).clone().toDate(),
                        AdvancedParameters: Object.clone(holidayShiftToCopy.AdvancedParameters, true),
                        BreakHours: holidayShiftToCopy.BreakHours,
                        ExistComplementaryData: holidayShiftToCopy.ExistComplementaryData,
                        ExistFloatingData: holidayShiftToCopy.ExistFloatingData,
                        ShiftLayers: holidayShiftToCopy.ShiftLayers,
                        ShiftLayersDefinition: []
                    };
                    for (var tmpLayerIndex = 0; tmpLayerIndex < holidayShift.ShiftLayers; tmpLayerIndex++) {
                        holidayShift.ShiftLayersDefinition.push({
                            LayerStartTime: moment(holidayShiftToCopy.ShiftLayersDefinition[tmpLayerIndex].LayerStartTime).clone().toDate(),
                            ExistLayerStartTime: holidayShiftToCopy.ShiftLayersDefinition[tmpLayerIndex].ExistLayerStartTime,
                            LayerOrdinaryHours: holidayShiftToCopy.ShiftLayersDefinition[tmpLayerIndex].LayerOrdinaryHours,
                            LayerDuration: holidayShiftToCopy.ShiftLayersDefinition[tmpLayerIndex].LayerDuration,
                            ExistLayerDuration: holidayShiftToCopy.ShiftLayersDefinition[tmpLayerIndex].ExistLayerDuration,
                            LayerID: holidayShiftToCopy.ShiftLayersDefinition[tmpLayerIndex].LayerID,
                            LayerComplementaryHours: holidayShiftToCopy.ShiftLayersDefinition[tmpLayerIndex].LayerComplementaryHours
                        });
                    }
                }

                if (copyMainShifts) allowAssignment = dayData.AllowAssignment;
                else allowAssignment = destinationDay.AllowAssignment;

                if (copyAssignments) assignmentShift = dayData.AssigData;
                else assignmentShift = destinationDay.AssigData;

                this.assignShiftToDay($('#' + oCalendar.ascxPrefix + '_calCell_' + destinationCellRow + '_' + destinationCellColumn), destinationDay, mainShift, holidayShift, assignmentShift, allowAssignment, keepLocked, keepDesHolidays, oCalData.CalendarData[destinationCellRow].EmployeeData);//.Assignments, oCalData.CalendarData[destinationCellRow].EmployeeData.FreezingDate);
            }
        }
    }
    this.copyInProgress = false;
    if (oCalendar.loadIndictments) oClientMode.checkCalendarIndictments();

    this.waitForUserToRefresh();
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.pasteTelecommuteSelectionEnd = function (clickedRow, clickedColumn, copyMainShifts, copyHolidays, copyAssignments, keepLocked, keepDesHolidays) {
    var oClientMode = this;
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    this.copyInProgress = true;
    for (var i = 0; i <= (oCalendar.selectedMaxRow - oCalendar.selectedMinRow); i++) {
        for (var x = 0; x <= (oCalendar.selectedMaxColumn - oCalendar.selectedMinColumn); x++) {
            var originCellRow = oCalendar.selectedMinRow + i;
            var originCellColumn = oCalendar.selectedMinColumn + x;

            var destinationCellRow = clickedRow + i;
            var destinationCellColumn = clickedColumn + x;

            var dayData = null;

            if (oCalendar.viewRange != Robotics.Client.Constants.ViewRange.Daily) {
                dayData = oCalData.CalendarData[originCellRow].PeriodData.DayData[originCellColumn];
            } else {
                dayData = oCalData.CalendarData[originCellRow].PeriodData.DayData[0];
            }

            var bAssign = true;

            var destinationDay = null;
            if (oCalendar.viewRange != Robotics.Client.Constants.ViewRange.Daily) {
                if (destinationCellRow < oCalData.CalendarData.length && destinationCellColumn < oCalData.CalendarData[destinationCellRow].PeriodData.DayData.length) {
                    bAssign = true;
                    destinationDay = oCalData.CalendarData[destinationCellRow].PeriodData.DayData[destinationCellColumn];
                } else {
                    bAssign = false;
                }
            } else {
                if (destinationCellRow < oCalData.CalendarData.length) {
                    bAssign = true;
                    destinationDay = oCalData.CalendarData[destinationCellRow].PeriodData.DayData[0];
                } else {
                    bAssign = false;
                }
            }

            if (bAssign) {
                if (dayData != null && destinationDay.CanTelecommute) {
                    var freezingDate = oCalData.CalendarData[originCellRow].EmployeeData.FreezingDate;

                    if (dayData.TelecommuteForced || dayData.TelecommutingOptional) {
                        destinationDay.TelecommutingExpected = dayData.TelecommutingExpected;
                        destinationDay.TelecommuteForced = true;
                        destinationDay.TelecommutingOptional = dayData.TelecommutingOptional;
                    } else {
                        if (typeof dayData.PresenceMandatoryDays != 'undefined') {
                            if (dayData.PresenceMandatoryDays.indexOf(destinationDay.PlanDate.getDay() + '') >= 0) {
                                destinationDay.TelecommutingExpected = false;
                            } else if (dayData.TelecommutingOptionalDays.indexOf(destinationDay.PlanDate.getDay() + '') >= 0) {
                                destinationDay.TelecommutingExpected = false;
                            } else if (dayData.TelecommutingMandatoryDays.indexOf(destinationDay.PlanDate.getDay() + '') >= 0) {
                                destinationDay.TelecommutingExpected = true;
                            } else {
                                destinationDay.TelecommutingExpected = false;
                            }
                            destinationDay.TelecommutingOptional = false;
                            destinationDay.TelecommuteForced = true;
                        } else {
                            destinationDay.TelecommutingExpected = false;
                            destinationDay.TelecommutingOptional = false;
                            destinationDay.TelecommuteForced = false;
                        }
                    }
                    destinationDay.HasChanged = true;

                    var destDiv = $('#' + oCalendar.ascxPrefix + '_calCell_' + destinationCellRow + '_' + destinationCellColumn);
                    destDiv.empty();
                    this.buildCalendarCellContent(destDiv, destinationDay, false, freezingDate);

                    var currentDayShift = this.getAssignedShift(destinationDay);
                    var oldShiftCopy = Object.clone(currentDayShift, true);
                    oClientMode.updateShiftTotalizerInfo(oldShiftCopy, oldShiftCopy, destinationCellRow, destinationCellColumn, destinationDay, true);
                }
            }

            if (bAssign) {
                oCalendar.setHasChanges(true);
                this.waitForUserToRefresh();
            }
        }
    }
    this.copyInProgress = false;
    if (oCalendar.loadIndictments) oClientMode.checkCalendarIndictments();

    this.waitForUserToRefresh();
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.copyHeaderSelection = function (container) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    if ((oCalendar.selectedMinHeaderColumn == -1 || oCalendar.selectedMaxHeaderColumn == -1) && container != null) {
        var clickedColumn = parseInt($(container).attr('data-IDColumn'), 10);

        oCalendar.selectedMinHeaderColumn = oCalendar.selectedMaxHeaderColumn = clickedColumn;
        $(container).addClass("ui-selected");
    }
    oCalendar.selectionHeaderCopied = true;
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.copySelection = function (container, copyWorking, copyHolidays, copyAssignments) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    if ((oCalendar.selectedMinRow == -1 || oCalendar.selectedMaxRow == -1 || oCalendar.selectedMinColumn == -1 || oCalendar.selectedMaxColumn == -1) && container != null) {
        var clickedRow = parseInt($(container).attr('data-IDRow'), 10);
        var clickedColumn = parseInt($(container).attr('data-IDColumn'), 10);

        oCalendar.selectedMinRow = oCalendar.selectedMaxRow = clickedRow;
        oCalendar.selectedMinColumn = oCalendar.selectedMaxColumn = clickedColumn;

        $(container).addClass("ui-selected");
    }

    oCalendar.copyWorkingShifts = copyWorking;
    oCalendar.copyHolidaysShifts = copyHolidays;
    oCalendar.copyAssignmentsShifts = copyAssignments;

    oCalendar.selectionCopied = true;
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.enterDetailAction = function (idGroup, idEmployee, date) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    if (oCalendar.hasChanges == false) {
        oCalendar.isShowingDialog = true;

        var empFiler = this.getSelectedIDEmployees()

        if (empFiler.length > 200) empFiler = idEmployee;

        var url = 'Scheduler/MovesNew.aspx?&EmpFilter=' + empFiler + '&EmployeeID=' + idEmployee + '&Date=' + date + '&CalendarV2=1';
        var Title = '';
        parent.ShowExternalForm2(url, 1400, 620, Title, '', false, false, false);
    }
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.removeHolidays = function (container) {
    var oClientMode = this;
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    if (!oCalendar.isBatchMode()) {
        if (oCalendar.selectedDay.ShiftBase != null) {
            var copyShift = {
                ID: oCalendar.selectedDay.ShiftBase.ID,
                ShortName: oCalendar.selectedDay.ShiftBase.ShortName,
                PlannedHours: oCalendar.selectedDay.ShiftBase.PlannedHours,
                Color: oCalendar.selectedDay.ShiftBase.Color,
                Name: oCalendar.selectedDay.ShiftBase.Name,
                Type: oCalendar.selectedDay.ShiftBase.Type,
                StartHour: moment(oCalendar.selectedDay.ShiftBase.StartHour).clone().toDate(),
                EndHour: moment(oCalendar.selectedDay.ShiftBase.EndHour).clone().toDate(),
                AdvancedParameters: Object.clone(oCalendar.selectedDay.ShiftBase.AdvancedParameters, true),
                BreakHours: oCalendar.selectedDay.ShiftBase.BreakHours,
                ExistComplementaryData: oCalendar.selectedDay.ShiftBase.ExistComplementaryData,
                ExistFloatingData: oCalendar.selectedDay.ShiftBase.ExistFloatingData,
                ShiftLayers: oCalendar.selectedDay.ShiftBase.ShiftLayers,
                ShiftLayersDefinition: []
            };
            for (var tmpIndex = 0; tmpIndex < copyShift.ShiftLayers; tmpIndex++) {
                copyShift.ShiftLayersDefinition.push({
                    LayerStartTime: moment(oCalendar.selectedDay.ShiftBase.ShiftLayersDefinition[tmpIndex].LayerStartTime).clone().toDate(),
                    ExistLayerStartTime: oCalendar.selectedDay.ShiftBase.ShiftLayersDefinition[tmpIndex].ExistLayerStartTime,
                    LayerOrdinaryHours: oCalendar.selectedDay.ShiftBase.ShiftLayersDefinition[tmpIndex].LayerOrdinaryHours,
                    LayerDuration: oCalendar.selectedDay.ShiftBase.ShiftLayersDefinition[tmpIndex].LayerDuration,
                    ExistLayerDuration: oCalendar.selectedDay.ShiftBase.ShiftLayersDefinition[tmpIndex].ExistLayerDuration,
                    LayerID: oCalendar.selectedDay.ShiftBase.ShiftLayersDefinition[tmpIndex].LayerID,
                    LayerComplementaryHours: oCalendar.selectedDay.ShiftBase.ShiftLayersDefinition[tmpIndex].LayerComplementaryHours
                });
            }

            if (copyShift.Type == Robotics.Client.Constants.ShiftType.Normal || copyShift.Type == Robotics.Client.Constants.ShiftType.NormalFloating) {
                this.assignShiftToDay($(container), oCalendar.selectedDay, copyShift, null, oCalendar.selectedDay.AssigData, oCalendar.selectedDay.AllowAssignment, true, false, oCalendar.selectedEmployee.EmployeeData);//.Assignments, oCalendar.selectedEmployee.EmployeeData.FreezingDate);
            }
        }
    } else {
        this.copyInProgress = true;
        for (var i = 0; i <= (oCalendar.selectedMaxRow - oCalendar.selectedMinRow); i++) {
            for (var x = 0; x <= (oCalendar.selectedMaxColumn - oCalendar.selectedMinColumn); x++) {
                var originCellRow = oCalendar.selectedMinRow + i;
                var originCellColumn = oCalendar.selectedMinColumn + x;

                var dayData = null;
                var dayContainer = null;
                if (oCalendar.viewRange != Robotics.Client.Constants.ViewRange.Daily) {
                    dayData = oCalData.CalendarData[originCellRow].PeriodData.DayData[originCellColumn];
                    dayContainer = $('#' + oCalendar.ascxPrefix + '_calCell_' + originCellRow + '_' + originCellColumn);
                } else {
                    dayData = oCalData.CalendarData[originCellRow].PeriodData.DayData[0];
                    dayContainer = $('#' + oCalendar.ascxPrefix + '_calDailyCell_' + originCellRow + '_' + originCellColumn);
                }

                if (dayData != null && dayContainer != null && dayData.ShiftBase != null) {
                    var copyShift = {
                        ID: dayData.ShiftBase.ID,
                        ShortName: dayData.ShiftBase.ShortName,
                        PlannedHours: dayData.ShiftBase.PlannedHours,
                        Color: dayData.ShiftBase.Color,
                        Name: dayData.ShiftBase.Name,
                        Type: dayData.ShiftBase.Type,
                        StartHour: moment(dayData.ShiftBase.StartHour).clone().toDate(),
                        EndHour: moment(dayData.ShiftBase.EndHour).clone().toDate(),
                        AdvancedParameters: Object.clone(dayData.ShiftBase.AdvancedParameters, true),
                        BreakHours: dayData.ShiftBase.BreakHours,
                        ExistComplementaryData: dayData.ShiftBase.ExistComplementaryData,
                        ExistFloatingData: dayData.ShiftBase.ExistFloatingData,
                        ShiftLayers: dayData.ShiftBase.ShiftLayers,
                        ShiftLayersDefinition: []
                    };
                    for (var tmpIndex = 0; tmpIndex < copyShift.ShiftLayers; tmpIndex++) {
                        copyShift.ShiftLayersDefinition.push({
                            LayerStartTime: moment(dayData.ShiftBase.ShiftLayersDefinition[tmpIndex].LayerStartTime).clone().toDate(),
                            ExistLayerStartTime: dayData.ShiftBase.ShiftLayersDefinition[tmpIndex].ExistLayerStartTime,
                            LayerOrdinaryHours: dayData.ShiftBase.ShiftLayersDefinition[tmpIndex].LayerOrdinaryHours,
                            LayerDuration: dayData.ShiftBase.ShiftLayersDefinition[tmpIndex].LayerDuration,
                            ExistLayerDuration: dayData.ShiftBase.ShiftLayersDefinition[tmpIndex].ExistLayerDuration,
                            LayerID: dayData.ShiftBase.ShiftLayersDefinition[tmpIndex].LayerID,
                            LayerComplementaryHours: dayData.ShiftBase.ShiftLayersDefinition[tmpIndex].LayerComplementaryHours
                        });
                    }

                    if (copyShift.Type == Robotics.Client.Constants.ShiftType.Normal || copyShift.Type == Robotics.Client.Constants.ShiftType.NormalFloating) {
                        this.assignShiftToDay(dayContainer, dayData, copyShift, null, dayData.AssigData, dayData.AllowAssignment, true, false, oCalData.CalendarData[originCellRow].EmployeeData);//.Assignments, oCalData.CalendarData[originCellRow].EmployeeData.FreezingDate);
                    }
                }
            }
        }
        if (oCalendar.loadIndictments) oClientMode.checkCalendarIndictments();
        this.copyInProgress = false;
    }

    this.waitForUserToRefresh();
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.waitForUserToRefresh = function () {
    $('#columnWaitingIcon').removeClass("loadingMove");
    $('#rowWaitingIcon').removeClass("loadingMove");
    $('#tabColumnWaiting').show();
    $('#tabRowWaiting').show();
    $('#tabResumeWaiting').show();
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.refreshShiftTotalizerInfo = function () {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    if (oCalendar.typeView == Robotics.Client.Constants.TypeView.Planification) {
        this.loadRelatedTables();
    }
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.getFirstObjectSelected = function () {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    var dayData = null;
    if (oCalendar.selectedMinRow != -1 && oCalendar.selectedMinColumn != -1) {
        dayData = oCalData.CalendarData[oCalendar.selectedMinRow].EmployeeData;
    }

    return dayData;
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.getFirstSelectedDay = function () {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    var dayData = null;
    if (oCalendar.selectedMinRow != -1 && oCalendar.selectedMinColumn != -1) {
        if (oCalendar.viewRange != Robotics.Client.Constants.ViewRange.Daily) {
            dayData = oCalData.CalendarData[oCalendar.selectedMinRow].PeriodData.DayData[oCalendar.selectedMinColumn];
        } else {
            dayData = oCalData.CalendarData[oCalendar.selectedMinRow].PeriodData.DayData[0];
        }
    }

    return dayData;
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.buildHeaderContextMenu = function (sender) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    var controlText = sender[0].innerText;
    var clickedColumn = parseInt($(sender).attr('data-IDColumn'), 10);

    items = {
        //'setCoverage': { name: oCalendar.translator.translate(Robotics.Client.Language.Tags.ContextMenu_SetCoverage), disabled: false, icon: 'setCoverage' },
        //'planifyCoverage': { name: oCalendar.translator.translate(Robotics.Client.Language.Tags.ContextMenu_PlanifyCoverage), disabled: false, icon: 'planifyCoverage' },
        //'split1': { name: '---------', disabled: true },
        //'copyAssignments': { name: oCalendar.translator.translate(Robotics.Client.Language.Tags.ContextMenu_Copy), disabled: false, icon: 'copyAssignments' },
        //'pasteAssignment': { name: oCalendar.translator.translate(Robotics.Client.Language.Tags.ContextMenu_Paste), disabled: false, icon: 'pasteShift' },
        ////'advPaste': { name: oCalendar.translator.translate(Robotics.Client.Language.Tags.ContextMenu_AdvPaste), disabled: false, icon: 'advPaste' },
        //'cancelSelection': { name: oCalendar.translator.translate(Robotics.Client.Language.Tags.ContextMenu_CancelSelection), disabled: false, icon: 'cancelSelection' },
        //'split2': { name: '---------', disabled: true },
        'sort': { name: oCalendar.translator.translate(Robotics.Client.Language.Tags.ContextMenu_Sort), disabled: false, icon: 'sort' }
    };

    //if (typeof oCalData.CalendarHeader.PeriodCoverageData == 'undefined' || (typeof oCalData.CalendarHeader.PeriodCoverageData != 'undefined' && oCalData.CalendarHeader.PeriodCoverageData.length == 0)) {
    //    items['setCoverage'].disabled = true;
    //    items['planifyCoverage'].disabled = true;
    //}

    //if (!oCalendar.selectionHeaderCopied) {
    //    items['pasteAssignment'].disabled = true;
    //    items['cancelSelection'].disabled = true;
    //}

    if (clickedColumn >= oCalendar.selectedMinHeaderColumn && clickedColumn <= oCalendar.selectedMaxHeaderColumn) {
        //items['pasteAssignment'].disabled = true;
    } else {
        if (oCalendar.selectionHeaderCopied) {
            //items['setCoverage'].disabled = true;
            //items['planifyCoverage'].disabled = true;
            //items['copyAssignments'].disabled = true;
            items['sort'].disabled = true;
        }
    }

    return items;
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.executeHeaderContextMenuAction = function (key, container) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    switch (key) {
        case "setCoverage":
            this.setDayCoverage(container);
            break;
        case "planifyCoverage":
            this.planifyDayCoverage(container);
            break;
        case "copyAssignments":
            this.copyHeaderSelection(container);
            break;
        case "pasteAssignment":
            this.pasteHeaderSelection(container);
            break;
        case "sort":
            oCalendar.sortColumn = parseInt($(container).attr('data-IDColumn'), 10);
            oCalendar.calendarSortDialog.dialog('open');
            break;
        case "cancelSelection":
            this.cancelCurrentMultipleHeaderSelect(container, true);
            break;
    }
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.setDayCoverage = function (container) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    var clickedColumn = parseInt($(container).attr('data-IDColumn'), 10);
    var coverage = oCalData.CalendarHeader.PeriodCoverageData[oCalendar.viewRange == Robotics.Client.Constants.ViewRange.Daily ? 0 : clickedColumn];

    ShowDailyCoverage(coverage.IDGroup, moment(coverage.CoverageDate).format(('DD/MM/YYYY')));
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.planifyDayCoverage = function (container) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    var clickedColumn = parseInt($(container).attr('data-IDColumn'), 10);
    var coverage = oCalData.CalendarHeader.PeriodCoverageData[oCalendar.viewRange == Robotics.Client.Constants.ViewRange.Daily ? 0 : clickedColumn];

    ShowDailyCoveragePlanned(coverage.IDGroup, moment(coverage.CoverageDate).format(('DD/MM/YYYY')));
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.pasteHeaderSelection = function (container, selectedBeginDate, selectedEndDate) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    if (typeof oCalData.CalendarHeader.PeriodCoverageData != 'undefined' && oCalData.CalendarHeader.PeriodCoverageData != null && oCalData.CalendarHeader.PeriodCoverageData.length > 0) {
        var pasteBegindDate = null;
        var pasteEndDate = null;

        if (container != null) {
            var clickedColumn = parseInt($(container).attr('data-IDColumn'), 10);
            var destCoverage = oCalData.CalendarHeader.PeriodCoverageData[oCalendar.viewRange == Robotics.Client.Constants.ViewRange.Daily ? 0 : clickedColumn];

            if (typeof destCoverage != 'undefined' && destCoverage != null) {
                pasteBegindDate = destCoverage.CoverageDate;
            }
        } else if (selectedBeginDate != null && selectedEndDate != null) {
            pasteBegindDate = selectedBeginDate;
            pasteEndDate = selectedEndDate;
        }

        if (pasteBegindDate != null && oCalendar.selectedMinHeaderColumn != -1 && oCalendar.selectedMaxHeaderColumn != -1) {
            var minCoverage = oCalData.CalendarHeader.PeriodCoverageData[oCalendar.viewRange == Robotics.Client.Constants.ViewRange.Daily ? 0 : oCalendar.selectedMinHeaderColumn];
            var maxCoverage = oCalData.CalendarHeader.PeriodCoverageData[oCalendar.viewRange == Robotics.Client.Constants.ViewRange.Daily ? 0 : oCalendar.selectedMaxHeaderColumn];

            var copyGroup = null;
            var copyBeginDate = null;
            var copyEndDate = null;

            if (typeof minCoverage != 'undefined' && minCoverage != null && typeof maxCoverage != 'undefined' && maxCoverage != null) {
                copyGroup = minCoverage.IDGroup;
                copyBeginDate = minCoverage.CoverageDate;
                copyEndDate = maxCoverage.CoverageDate;

                if (pasteEndDate == null) {
                    pasteEndDate = moment(pasteBegindDate).add(moment(copyEndDate).diff(moment(copyBeginDate), 'days'), 'days').toDate();
                }

                var oParameters = {};

                oParameters.firstDate = oCalendar.firstDate;
                oParameters.endDate = oCalendar.endDate;
                oParameters.employeeFilter = oCalendar.employeeFilter;
                oParameters.loadRecursive = oCalendar.loadRecursive;
                oParameters.calendar = null;
                oParameters.typeView = oCalendar.typeView;
                oParameters.assignmentsFilter = oCalendar.assignmentsFilter;

                oParameters.idGroup = copyGroup;
                oParameters.copyBeginDate = copyBeginDate;
                oParameters.copyEndDate = copyEndDate;

                oParameters.pasteBegindDate = pasteBegindDate;
                oParameters.pasteEndDate = pasteEndDate;

                oParameters.StampParam = new Date().getMilliseconds();

                oCalendar.performAction(oParameters, Robotics.Client.Constants.Actions.CopyCoverages, false);
            }
        }
    }
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.selectHeaderMultiple = function (origin, destination) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    if (origin != null && oCalendar.viewRange != Robotics.Client.Constants.ViewRange.Daily) {
        var originColumn = parseInt($(origin).attr('data-IDColumn'), 10);
        var destinationColumn = parseInt($(destination).attr('data-IDColumn'), 10);

        this.cancelCurrentMultipleSelect(null, false);

        if (originColumn <= destinationColumn) {
            oCalendar.selectedMinHeaderColumn = originColumn;
            oCalendar.selectedMaxHeaderColumn = destinationColumn;
        } else {
            oCalendar.selectedMinHeaderColumn = destinationColumn;
            oCalendar.selectedMaxHeaderColumn = originColumn;
        }

        for (var i = oCalendar.selectedMinHeaderColumn; i <= oCalendar.selectedMaxHeaderColumn; i++) {
            $('#' + oCalendar.ascxPrefix + '_calHeaderCell_' + i).addClass('ui-selected');
        }
    }
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.selectMultiple = function (origin, destination) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    if (origin != null && oCalendar.viewRange != Robotics.Client.Constants.ViewRange.Daily) {
        var originRow = parseInt($(origin).attr('data-IDRow'), 10);
        var destinationRow = parseInt($(destination).attr('data-IDRow'), 10);

        var originColumn = parseInt($(origin).attr('data-IDColumn'), 10);
        var destinationColumn = parseInt($(destination).attr('data-IDColumn'), 10);

        this.cancelCurrentMultipleSelect(null, false);

        if (originRow <= destinationRow) {
            oCalendar.selectedMinRow = originRow;
            oCalendar.selectedMaxRow = destinationRow;
        } else {
            oCalendar.selectedMinRow = destinationRow;
            oCalendar.selectedMaxRow = originRow;
        }

        if (originColumn <= destinationColumn) {
            oCalendar.selectedMinColumn = originColumn;
            oCalendar.selectedMaxColumn = destinationColumn;
        } else {
            oCalendar.selectedMinColumn = destinationColumn;
            oCalendar.selectedMaxColumn = originColumn;
        }

        for (var i = oCalendar.selectedMinRow; i <= oCalendar.selectedMaxRow; i++) {
            for (var x = oCalendar.selectedMinColumn; x <= oCalendar.selectedMaxColumn; x++) {
                $('#' + oCalendar.ascxPrefix + '_calCell_' + i + '_' + x).addClass('ui-selected');
            }
        }
    }
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.setSingleHeaderSelectedObejct = function (sender) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    if (sender != null) this.setSingleSelectedObject(null);

    if (oCalendar.selectedHeaderContainer != null) {
        oCalendar.selectedHeaderContainer.removeClass('singleCellSelected');
    }

    if (sender == null) {
        oCalendar.selectedHeaderContainer = null;
    } else {
        var clickedColumn = parseInt($(sender).attr('data-IDColumn'), 10);

        if (!(clickedColumn >= oCalendar.selectedMinHeaderColumn && clickedColumn <= oCalendar.selectedMaxHeaderColumn)) {
            if (!oCalendar.selectionHeaderCopied) {
                this.cancelCurrentMultipleSelect(sender, true);
                this.cancelCurrentMultipleHeaderSelect(sender, true);
            }
        }

        oCalendar.selectedHeaderContainer = $(sender);
        oCalendar.selectedHeaderContainer.addClass('singleCellSelected');
    }
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.setSingleSelectedObject = function (sender) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    if (sender != null) this.setSingleHeaderSelectedObejct(null);

    if (oCalendar.selectedContainer != null) {
        oCalendar.selectedContainer.removeClass('singleCellSelected');
    }
    if (sender == null) {
        oCalendar.selectedEmployee = null;
        oCalendar.selectedDay = null;
        oCalendar.selectedContainer = null;
    } else {
        var clickedRow = parseInt($(sender).attr('data-IDRow'), 10);
        var clickedColumn = parseInt($(sender).attr('data-IDColumn'), 10);

        if (!((clickedRow >= oCalendar.selectedMinRow && clickedRow <= oCalendar.selectedMaxRow) && (clickedColumn >= oCalendar.selectedMinColumn && clickedColumn <= oCalendar.selectedMaxColumn))) {
            if (!oCalendar.selectionCopied) {
                this.cancelCurrentMultipleSelect(null, false);
                this.cancelCurrentMultipleHeaderSelect(null, false);
            }
        }

        oCalendar.selectedEmployee = oCalData.CalendarData[parseInt($(sender).attr('data-IDRow'), 10)];
        if (oCalendar.viewRange != Robotics.Client.Constants.ViewRange.Daily) {
            oCalendar.selectedDay = oCalData.CalendarData[parseInt($(sender).attr('data-IDRow'), 10)].PeriodData.DayData[parseInt($(sender).attr('data-IDColumn'), 10)];
        } else {
            oCalendar.selectedDay = oCalData.CalendarData[parseInt($(sender).attr('data-IDRow'), 10)].PeriodData.DayData[0];
        }

        oCalendar.selectedContainer = $(sender);
        oCalendar.selectedContainer.addClass('singleCellSelected');
    }
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.endSelectOperation = function (sender) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    var selectedItems = $('.calendarOuterBodyCell.ui-selected');

    this.cancelCurrentMultipleHeaderSelect(sender, true);
    this.setSingleHeaderSelectedObejct(null);
    this.setSingleSelectedObject(null);
    var minRow = 0;
    var maxRow = 0;
    var minColumn = 0;
    var maxColumn = 0;

    for (var i = 0; i < selectedItems.length; i++) {
        if (i == 0) {
            minRow = parseInt($(selectedItems[i]).attr('data-IDRow'), 10);
            maxRow = parseInt($(selectedItems[i]).attr('data-IDRow'), 10);

            minColumn = parseInt($(selectedItems[i]).attr('data-IDColumn'), 10);
            maxColumn = parseInt($(selectedItems[i]).attr('data-IDColumn'), 10);
        } else {
            if (parseInt($(selectedItems[i]).attr('data-IDRow'), 10) < minRow) minRow = parseInt($(selectedItems[i]).attr('data-IDRow'), 10);
            if (parseInt($(selectedItems[i]).attr('data-IDRow'), 10) > maxRow) maxRow = parseInt($(selectedItems[i]).attr('data-IDRow'), 10);

            if (parseInt($(selectedItems[i]).attr('data-IDColumn'), 10) < minColumn) minColumn = parseInt($(selectedItems[i]).attr('data-IDColumn'), 10);
            if (parseInt($(selectedItems[i]).attr('data-IDColumn'), 10) > maxColumn) maxColumn = parseInt($(selectedItems[i]).attr('data-IDColumn'), 10);
        }
    }

    oCalendar.selectedMinRow = minRow;
    oCalendar.selectedMaxRow = maxRow;

    oCalendar.selectedMinColumn = minColumn;
    oCalendar.selectedMaxColumn = maxColumn;
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.endHeaderSelectOperation = function (sender) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    var selectedItems = $('.CalendarDayFixedHeader.ui-selected');

    this.cancelCurrentMultipleSelect(sender, true);
    this.setSingleHeaderSelectedObejct(null);
    this.setSingleSelectedObject(null);

    var minColumn = 0;
    var maxColumn = 0;

    for (var i = 0; i < selectedItems.length; i++) {
        if (i == 0) {
            minColumn = parseInt($(selectedItems[i]).attr('data-IDColumn'), 10);
            maxColumn = parseInt($(selectedItems[i]).attr('data-IDColumn'), 10);
        } else {
            if (parseInt($(selectedItems[i]).attr('data-IDColumn'), 10) < minColumn) minColumn = parseInt($(selectedItems[i]).attr('data-IDColumn'), 10);
            if (parseInt($(selectedItems[i]).attr('data-IDColumn'), 10) > maxColumn) maxColumn = parseInt($(selectedItems[i]).attr('data-IDColumn'), 10);
        }
    }

    oCalendar.selectedMinHeaderColumn = minColumn;
    oCalendar.selectedMaxHeaderColumn = maxColumn;
}

Robotics.Client.Controls.roSchedulerCalendar.prototype.sortCalendar = function () {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    var sortParams = oCalendar.sortElements.split(',');

    oCalData.CalendarData = oCalData.CalendarData.sort(dynamicSortMultiple(sortParams, oCalendar.viewRange == Robotics.Client.Constants.ViewRange.Daily ? 0 : oCalendar.sortColumn));
    oCalendar.refreshTables(null, false, true);
    this.refreshCoveragesHeadersInfo(null);
    oCalendar.calendarSortDialog.dialog("close");
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.setCallbackReturnData = function (objReturn, isHourData) {
    var oClientMode = this;
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    var selectedRow = parseInt(oCalendar.selectedContainer.attr('data-IDRow'), 10);
    var selectedColumn = parseInt(oCalendar.selectedContainer.attr('data-IDColumn'), 10);

    oClientMode.currentUpdateCell = Object.clone(oCalendar.selectedDay, true);

    if (oCalendar.viewRange == Robotics.Client.Constants.ViewRange.Daily) {
        if (isHourData) {
            oCalendar.selectedDay.HourData = objReturn;
            oCalData.CalendarData[selectedRow].PeriodData.DayData[0].HourData = objReturn;
        } else {
            oCalendar.selectedDay = objReturn;
            oCalData.CalendarData[selectedRow].PeriodData.DayData[0] = objReturn;
        }
    } else {
        oCalendar.selectedDay = objReturn;
        oCalData.CalendarData[selectedRow].PeriodData.DayData[selectedColumn] = objReturn;

        setTimeout(function () {
            if (oCalendar.loadIndictments) {
                var cellId = oCalendar.ascxPrefix + '_tooltipIndictment_' + selectedRow + '_' + selectedColumn;

                $('#' + cellId).off('mouseover');
                $('#' + cellId).on('mouseover', function () {
                    var cell = $(this).find('.tooltipIndictmentsContainer');
                    if (cell.length == 0) return;
                    var cellId = this.id;
                    $(cell).dxTooltip({
                        target: "#" + cellId,
                        hideEvent: "mouseleave",
                        position: 'bottom'
                    });
                    $(cell).dxTooltip("instance").show();
                });
            }

            if (oCalendar.loadPunches) {
                var cellId = oCalendar.ascxPrefix + '_tooltipPunchesReview_' + selectedRow + '_' + selectedColumn;

                $('#' + cellId).off('mouseover');
                $('#' + cellId).on('mouseover', function () {
                    var cell = $(this).find('.tooltipPunchesReviewContainer');
                    if (cell.length == 0) return;
                    var cellId = this.id;
                    $(cell).dxTooltip({
                        target: "#" + cellId,
                        hideEvent: "mouseleave",
                        position: 'bottom'
                    });
                    $(cell).dxTooltip("instance").show();
                });
            }
        }, 100);
    }
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.endCallback = function (action, objResult, objResultParams) {
    var request = this;
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    switch (action) {
        case Robotics.Client.Constants.Actions.CopyCoverages:
            this.copyCoverageResponse(objResult, objResultParams);
            break;
        case Robotics.Client.Constants.Actions.LoadCoverages:
            this.loadCoveragesResponse(objResult, objResultParams);
            break;
        case Robotics.Client.Constants.Actions.DiscardAndContinue:
        case Robotics.Client.Constants.Actions.LoadData:
            this.loadDataResponse(objResult, objResultParams);

            oCalendar.capatityEnabled = false;
            if (typeof (objResult.CalendarHeader.PeriodSeatingCapacityData) != 'undefined') {
                var allCapacities = [].concat.apply([], objResult.CalendarHeader.PeriodSeatingCapacityData.map(function (x) { return x.Capacities }));
                var uniqueCapacities = [...new Map(allCapacities.map(item => [item['WorkCenter'], item])).values()];

                for (var i = 0; i < uniqueCapacities.length; i++) {
                    if (uniqueCapacities[i].MaxSeatingCapacity > 0) {
                        oCalendar.capatityEnabled = true;
                    }
                }
            }

            if (oCalendar.capatityEnabled || (request.oBaseControl.pageLayout.center.children.tabsContainerLayout.south != false && !request.oBaseControl.pageLayout.center.children.tabsContainerLayout.south.state.isClosed)) {
                request.generateShiftTotalizerInfo();
            }

            break;
        case Robotics.Client.Constants.Actions.CheckCalendarIndictments:
            this.printInditcmentsResult(objResult, objResultParams);
            break;
        case Robotics.Client.Constants.Actions.SaveAndContinue:
        case Robotics.Client.Constants.Actions.SaveChanges:
            this.saveChangesResponse(objResult, objResultParams);
            break;
        case Robotics.Client.Constants.Actions.LoadHourData:
            this.loadHourDataResponse(objResult, objResultParams);
            break;
        case Robotics.Client.Constants.Actions.LoadDayDefinition:
            this.loadDayDefinitionResponse(objResult, objResultParams);
            break;
        case Robotics.Client.Constants.Actions.ExportToExcel:
            this.exportToExcelResponse(objResult, objResultParams);
            break;
        case Robotics.Client.Constants.Actions.ImportFromExcel:
            this.importFromExcelResponse(objResult, objResultParams);
            break;
        case Robotics.Client.Constants.Actions.ImportFromExcelKO:
            this.importFromExcelKOResponse(objResult, objResultParams);
            break;
        case Robotics.Client.Constants.Actions.ImportFromExcelWarning:
            this.importFromExcelWarningResponse(objResult, objResultParams);
            break;
        case Robotics.Client.Constants.Actions.ShiftLayerDefinition:
            this.shiftLayerDefinitionResponse(objResult, objResultParams);
            break;
        case Robotics.Client.Constants.Actions.ShiftLayerDefinitionEdit:
            this.shiftLayerDefinitionEditResponse(objResult, objResultParams);
            break;
    }

    if (action != Robotics.Client.Constants.Actions.SaveChanges) this.loadingFunctionExtended(false);

    if (action == Robotics.Client.Constants.Actions.DiscardAndContinue || action == Robotics.Client.Constants.Actions.SaveAndContinue) {
        eval(oCalendar.onContinueFunc);
        oCalendar.onContinueFunc = '';
    }

    if (this.bRelatedTables == true) {
        setTimeout(function () { request.loadRelatedTables(); }, 300);
    }
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.saveChangesResponse = function (objResult, objResultParams) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    oCalendar.capacityError = false;
    oCalendar.setHasChanges(false);
    if (oCalendar.isScheduleActive) this.setUpSchedulingTimmer(1);

    if (oCalendar.loadIndictments) this.refresh();
    else this.loadingFunctionExtended(false);
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.printInditcmentsResult = function (objResult, objResultParams) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    this.validationInProgress = true;

    //oCalendar.initialize();
    oCalendar.oCalendar = objResult;
    oCalendar.setHasChanges(true);

    oCalendar.refreshTables(null, false, true);
    this.validationInProgress = false;
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.loadDataResponse = function (objResult, objResultParams) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    if (moment(objResult.FirstDay).diff(moment(objResult.LastDay), 'days') == 0) {
        if (oCalendar.typeView == Robotics.Client.Constants.TypeView.Review) {
            oCalendar.viewRange = Robotics.Client.Constants.ViewRange.Period;
        } else {
            oCalendar.viewRange = Robotics.Client.Constants.ViewRange.Daily;
        }
    } else {
        oCalendar.viewRange = Robotics.Client.Constants.ViewRange.Period;
    }

    oCalendar.initialize();
    oCalendar.oCalendar = objResult;
    oCalendar.remarksColor = objResultParams;
    oCalendar.setHasChanges(false);

    oCalendar.refreshTables(null, false, true);
    if (oCalendar.isScheduleActive) this.refreshCoveragesHeadersInfo(null);
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.loadCoveragesResponse = function (objResult, objResultParams) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    this.refreshCoveragesHeadersInfo(objResult);
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.copyCoverageResponse = function (objResult, objResultParams) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    this.refreshCoveragesHeadersInfo(objResult);
    oCalendar.selectionCopied = false;
    oCalendar.selectionHeaderCopied = false;
    this.cancelCurrentMultipleSelect(null, false);
    this.cancelCurrentMultipleHeaderSelect(null, false);
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.loadHourDataResponse = function (objResult, objResultParams) {
    this.setCallbackReturnData(objResult, true);
    this.refreshDailySelectedData(false);
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.loadDayDefinitionResponse = function (objResult, objResultParams) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    this.setCallbackReturnData(objResult, false);
    this.refreshFullDay();
    this.setUpSchedulingTimmer(1);
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.exportToExcelResponse = function (objResult, objResultParams) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    window.open("./../Base/WebUserControls/roCalendar/Wizards/downloadCalendar.aspx");
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.importFromExcelResponse = function (objResult, objResultParams) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    if (this.calendarHasChanges(objResult)) {
        oCalendar.selectedDay = null;
        oCalendar.selectedContainer = null;
        oCalendar.selectedEmployee = null;

        oCalendar.oCalendar = objResult;
        oCalendar.refreshTables(null, false, true);
        oCalendar.setHasChanges(true);
    } else {
        oCalendar.showErrorPopup("Info.ChangesTitle", "info", "Calendar.Client.NoDaysUpdated", "", "Error.OK", "Error.OKDesc", oCalendar.defaultMessageAction);
    }
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.importFromExcelKOResponse = function (objResult, objResultParams) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    oCalendar.buildErrorMessage(objResult, true);
    oCalendar.importErrorDialog.dialog("open");

    if (objResult.CalendarDataResult['0'].ErrorCode == 15) {
        var bt = document.getElementById('btDownload');
        bt.style.visibility = 'hidden';
    }
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.importFromExcelWarningResponse = function (objResult, objResultParams) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    if (this.calendarHasChanges(objResult)) {
        oCalendar.setHasChanges(true);
        oCalendar.selectedDay = null;
        oCalendar.selectedContainer = null;
        oCalendar.selectedEmployee = null;
        oCalendar.oCalendar = objResult;
        oCalendar.refreshTables(null, false, true);
        oCalendar.buildErrorMessage(objResultParams, true);
        oCalendar.importErrorDialog.dialog("open");
    } else {
        oCalendar.showErrorPopup("Info.ChangesTitle", "info", "Calendar.Client.NoDaysUpdated", "", "Error.OK", "Error.OKDesc", oCalendar.defaultMessageAction);
    }
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.shiftLayerDefinitionResponse = function (objResult, objResultParams) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    if (oCalendar.showComplementaryAssignDialog) oCalendar.prepareComplementaryDialog(objResult);
    else if (oCalendar.showAssignmentsDialog) oCalendar.prepareAssignmentsDialog(objResult);
    else if (oCalendar.showStarterDialog) oCalendar.prepareStarterDialog(objResult);
}

Robotics.Client.Controls.roSchedulerCalendar.prototype.shiftLayerDefinitionEditResponse = function (objResult, objResultParams) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    if (oCalendar.showComplementaryAssignDialog) this.editComplementaryInfoFinally(objResult);
    else if (oCalendar.showAssignmentsDialog) this.editAssignmentsInfoFinally(objResult);
    else if (oCalendar.showStarterDialog) this.editStarterInfoFinally(objResult);
}

Robotics.Client.Controls.roSchedulerCalendar.prototype.calendarHasChanges = function (oCal) {
    var oCalendar = this.oBaseControl;

    var hasChanges = false;
    var checkCalendar = oCalendar.oCalendar;
    if (typeof (oCal) != 'undefined') {
        checkCalendar = oCal;
    }

    if (checkCalendar.CalendarData != null && checkCalendar.CalendarData.length > 0) {
        for (var i = 0; i < checkCalendar.CalendarData.length; i++) {
            var curRow = checkCalendar.CalendarData[i];

            if (curRow.PeriodData != null && curRow.PeriodData.DayData != null && curRow.PeriodData.DayData.length > 0) {
                for (var x = 0; x < curRow.PeriodData.DayData.length; x++) {
                    var curCol = curRow.PeriodData.DayData[x];
                    if (curCol.HasChanged) {
                        hasChanges = true;
                    }
                    if (hasChanges) break;
                }
            }

            if (hasChanges) break;
        }
    }

    return hasChanges;
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.setUpSchedulingTimmer = function (timmerSeconds) {
    var oCalendar = this.oBaseControl;
    var oMode = this;

    //$('.assignmentExists').addClass('loadingAssignments');
    //if (oCalendar.timmerEnabled && oCalendar.refreshTimmer == -1) { oCalendar.refreshTimmer = setTimeout(function () { oMode.refreshTimmerFunc(); }, timmerSeconds * 1000); }
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.refreshTimmerFunc = function () {
    var oCalendar = this.oBaseControl;
    //if (!this.hasChanges && !this.isShowingDialog && !this.advCopyDialog.dialog('isOpen') && !this.copyDialog.dialog('isOpen') && !this.errorDialog.dialog('isOpen') &&
    //    !this.importDialog.dialog('isOpen') && !this.importErrorDialog.dialog('isOpen') && !this.complementaryDialog.dialog('isOpen') &&
    //    !this.filterCalendarDialog.dialog('isOpen') && !this.assignmentsDialog.dialog('isOpen') && !this.calendarSortDialog.dialog('isOpen')) {
    //if (oCalendar.isScheduleActive) {
    //    this.reloadCoveragesData();  // TODO: reloadCoveragesData
    //    oCalendar.refreshTimmer = -1;
    //}
    //this.refresh();
    //}
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.refreshCoveragesHeadersInfo = function (objResult) {
    //var oCalendar = this.oBaseControl;
    //var oCalData = oCalendar.oCalendar;

    //if (objResult != null) oCalData.CalendarHeader.PeriodCoverageData = objResult.CalendarHeader.PeriodCoverageData;

    //var dataOk = true;
    //if (typeof oCalData.CalendarHeader.PeriodCoverageData != 'undefined' && oCalData.CalendarHeader.PeriodCoverageData != null && oCalData.CalendarHeader.PeriodCoverageData.length > 0) {
    //    for (var i = 0; i < oCalData.CalendarHeader.PeriodCoverageData.length; i++) {
    //        var status = oCalendar.schedulingView == Robotics.Client.Constants.CoverageDayView.Planified ? oCalData.CalendarHeader.PeriodCoverageData[i].PlannedProcessed : oCalData.CalendarHeader.PeriodCoverageData[i].ActualProcessed;

    //        if (!(oCalendar.schedulingView == Robotics.Client.Constants.CoverageDayView.Real && moment(oCalData.CalendarHeader.PeriodCoverageData[i].CoverageDate).isAfter(moment()))) {
    //            if (!status) {
    //                dataOk = false;
    //                break;
    //            }
    //        }
    //    }
    //}

    //if (dataOk) {
    //    if (oCalendar.viewRange == Robotics.Client.Constants.ViewRange.Daily) {
    //        //for (var i = 0; i < oCalData.CalendarHeader.PeriodHeaderData.length; i++) {
    //        for (var i = oCalendar.getMinDailyCell(); i < oCalendar.getMaxDailyCell(); i++) {
    //            $('#' + oCalendar.ascxPrefix + '_IDHeaderCell_' + i).replaceWith(oCalendar.clientMode.buildCellScheduleStatus(0, true).attr('id', oCalendar.ascxPrefix + '_IDHeaderCell_' + i));
    //            $('#' + oCalendar.ascxPrefix + '_IDFooterCell_' + i).replaceWith(oCalendar.clientMode.buildCellScheduleStatus(0, false).attr('id', oCalendar.ascxPrefix + '_IDFooterCell_' + i));
    //        }

    //    } else {
    //        for (var i = 0; i < oCalData.CalendarHeader.PeriodHeaderData.length; i++) {
    //            $('#' + oCalendar.ascxPrefix + '_IDHeaderCell_' + i).replaceWith(oCalendar.clientMode.buildCellScheduleStatus(i, true).attr('id', oCalendar.ascxPrefix + '_IDHeaderCell_' + i));
    //            $('#' + oCalendar.ascxPrefix + '_IDFooterCell_' + i).replaceWith(oCalendar.clientMode.buildCellScheduleStatus(i, false).attr('id', oCalendar.ascxPrefix + '_IDFooterCell_' + i));
    //        }
    //    }

    //    this.waitForUserToRefresh();

    //    //$('.assignmentExists').tooltip({
    //    //    content: function (callback) {
    //    //        callback($(this).prop('title').replaceAll('&#;', '<br />'));
    //    //    }
    //    //});

    //} else this.setUpSchedulingTimmer(1);
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.reloadCoveragesData = function () {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    if (oCalendar.employeeFilter.count('A') == 1) {
        var oParameters = this.generateLoadFilters();
        oCalendar.performAction(oParameters, Robotics.Client.Constants.Actions.LoadCoverages, false);
    }
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.onAcceptCopyDialog = function (keepLockedDays, KeepDestHolidayDays) {
    var oCalendar = this.oBaseControl;

    this.pasteSelectionEnd(oCalendar.sourceDialogRow, oCalendar.sourceDialogColumn, oCalendar.copyWorkingShifts, oCalendar.copyHolidaysShifts, oCalendar.copyAssignmentsShifts, keepLockedDays, KeepDestHolidayDays);
}

Robotics.Client.Controls.roSchedulerCalendar.prototype.prepareStarterDialog = function (objResult) {
    var oCalendar = this.oBaseControl;

    if (objResult != null) {
        oCalendar.shiftsExtendedDataCache[oCalendar.starterShift.ID] = objResult;
    }

    oCalendar.starterManager.prepareStarterDialog(oCalendar.shiftsExtendedDataCache[oCalendar.starterShift.ID], -1);

    if (oCalendar.viewRange == Robotics.Client.Constants.ViewRange.Daily) {
        var selColumn = parseInt(oCalendar.selectedContainer.attr('data-IDColumn'), 10);
        var selectedStartDate = "1899/12/30 " + oCalendar.oCalendar.CalendarHeader.PeriodHeaderData[selColumn].Row2Text;
        var startHour = moment(selectedStartDate, "YYYY/MM/DD HH:mm").toDate();

        oCalendar.starterManager.setOffset(startHour);
    }

    oCalendar.starterDialog.dialog("open");
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.onAcceptStarterDialog = function () {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    if (oCalendar.starterManager.isValid()) {
        oCalendar.starterDialog.dialog("close");

        oCalendar.showStarterDialog = false;

        var mainShift = oCalendar.starterManager.getDayData();

        if (!oCalendar.isBatchMode()) {
            if (mainShift.Type == Robotics.Client.Constants.ShiftType.Normal || mainShift.Type == Robotics.Client.Constants.ShiftType.NormalFloating) {
                this.assignShiftToDay(oCalendar.selectedContainer, oCalendar.selectedDay, mainShift, null, oCalendar.selectedDay.AssigData, oCalendar.selectedDay.AllowAssignment, true, true, oCalendar.selectedEmployee.EmployeeData);//.Assignments, oCalendar.selectedEmployee.EmployeeData.FreezingDate);
            } else {
                this.assignShiftToDay(oCalendar.selectedContainer, oCalendar.selectedDay, null, mainShift, oCalendar.selectedDay.AssigData, oCalendar.selectedDay.AllowAssignment, true, true, oCalendar.selectedEmployee.EmployeeData);//.Assignments, oCalendar.selectedEmployee.EmployeeData.FreezingDate);
            }
        } else {
            if (mainShift.Type == Robotics.Client.Constants.ShiftType.Normal || mainShift.Type == Robotics.Client.Constants.ShiftType.NormalFloating) {
                this.assignShiftToDayBatch(mainShift, null, undefined, undefined, true, true);
            } else {
                this.assignShiftToDayBatch(null, mainShift, undefined, undefined, true, true);
            }
        }

        oCalendar.complementaryShift = null;
        this.waitForUserToRefresh();
    }
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.prepareComplementaryDialog = function (objResult) {
    var oCalendar = this.oBaseControl;

    if (objResult != null) {
        oCalendar.shiftsExtendedDataCache[oCalendar.complementaryShift.ID] = objResult;
    }

    oCalendar.complementaryManager.prepareComplementaryDialog(oCalendar.shiftsExtendedDataCache[oCalendar.complementaryShift.ID], -1);

    if (oCalendar.viewRange == Robotics.Client.Constants.ViewRange.Daily) {
        var selColumn = parseInt(oCalendar.selectedContainer.attr('data-IDColumn'), 10);
        var selectedStartDate = "1899/12/30 " + oCalendar.oCalendar.CalendarHeader.PeriodHeaderData[selColumn].Row2Text;
        var startHour = moment(selectedStartDate, "YYYY/MM/DD HH:mm").toDate();

        oCalendar.complementaryManager.setOffset(startHour);
    }

    oCalendar.complementaryDialog.dialog("open");
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.onAcceptComplementaryDialog = function () {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    if (oCalendar.complementaryManager.isValid()) {
        oCalendar.complementaryDialog.dialog("close");

        oCalendar.showComplementaryAssignDialog = false;

        var mainShift = oCalendar.complementaryManager.getDayData();

        if (!oCalendar.showAssignmentsDialog) {
            if (!oCalendar.isBatchMode()) {
                if (mainShift.Type == Robotics.Client.Constants.ShiftType.Normal || mainShift.Type == Robotics.Client.Constants.ShiftType.NormalFloating) {
                    this.assignShiftToDay(oCalendar.selectedContainer, oCalendar.selectedDay, mainShift, null, oCalendar.selectedDay.AssigData, oCalendar.selectedDay.AllowAssignment, true, true, oCalendar.selectedEmployee.EmployeeData);//.Assignments, oCalendar.selectedEmployee.EmployeeData.FreezingDate);
                } else {
                    this.assignShiftToDay(oCalendar.selectedContainer, oCalendar.selectedDay, null, mainShift, oCalendar.selectedDay.AssigData, oCalendar.selectedDay.AllowAssignment, true, true, oCalendar.selectedEmployee.EmployeeData);//.Assignments, oCalendar.selectedEmployee.EmployeeData.FreezingDate);
                }
            } else {
                if (mainShift.Type == Robotics.Client.Constants.ShiftType.Normal || mainShift.Type == Robotics.Client.Constants.ShiftType.NormalFloating) {
                    this.assignShiftToDayBatch(mainShift, null, undefined, undefined, true, true);
                } else {
                    this.assignShiftToDayBatch(null, mainShift, undefined, undefined, true, true);
                }
            }

            oCalendar.complementaryShift = null;
            this.waitForUserToRefresh();
        } else {
            oCalendar.assignmentShift = mainShift;
            this.prepareAssignmentsDialog(null);
        }
    }
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.prepareAssignmentsDialog = function (objResult) {
    var oCalendar = this.oBaseControl;

    if (objResult != null) {
        oCalendar.shiftsExtendedDataCache[oCalendar.assignmentShift.ID] = objResult;
    }

    oCalendar.assignmentsManager.prepareAssignmentsDialog(oCalendar.shiftsExtendedDataCache[oCalendar.assignmentShift.ID], oCalendar.selectedEmployee.EmployeeData.Assignments, -1);

    oCalendar.assignmentsDialog.dialog("open");

    setTimeout(function () { oCalendar.assignmentsManager.focusDialog(); }, 100);
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.onAcceptAssignmentsDialog = function () {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    if (oCalendar.assignmentsManager.isValid()) {
        oCalendar.assignmentsDialog.dialog("close");

        oCalendar.showAssignmentsDialog = false;

        var assigData = oCalendar.assignmentsManager.getDayData();

        if (!oCalendar.isBatchMode()) {
            if (oCalendar.assignmentShift.Type == Robotics.Client.Constants.ShiftType.Normal || oCalendar.assignmentShift.Type == Robotics.Client.Constants.ShiftType.NormalFloating) {
                this.assignShiftToDay(oCalendar.selectedContainer, oCalendar.selectedDay, oCalendar.assignmentShift, null, assigData, true, true, true, oCalendar.selectedEmployee.EmployeeData);//.Assignments, oCalendar.selectedEmployee.EmployeeData.FreezingDate);
            } else {
                this.assignShiftToDay(oCalendar.selectedContainer, oCalendar.selectedDay, null, oCalendar.assignmentShift, assigData, true, true, true, oCalendar.selectedEmployee.EmployeeData);//.Assignments, oCalendar.selectedEmployee.EmployeeData.FreezingDate);
            }
        } else {
            if (oCalendar.assignmentShift.Type == Robotics.Client.Constants.ShiftType.Normal || oCalendar.assignmentShift.Type == Robotics.Client.Constants.ShiftType.NormalFloating) {
                this.assignShiftToDayBatch(oCalendar.assignmentShift, null, assigData, true, true, true);
            } else {
                this.assignShiftToDayBatch(null, oCalendar.assignmentShift, assigData, true, true, true);
            }
        }

        oCalendar.assignmentShift = null;

        this.waitForUserToRefresh();
    }
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.changeFilterAssignments = function () {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    var assignmentsGrid = eval(oCalendar.ascxPrefix + '_dlgFilterCalendar_grdAssignmentsClient');
    var rbPlannedView = eval(oCalendar.ascxPrefix + '_dlgFilterCalendar_rbPlannedView');
    var rbRealView = eval(oCalendar.ascxPrefix + '_dlgFilterCalendar_rbRealView');

    var selectedValues = assignmentsGrid.GetSelectedKeysOnPage();
    var calendarFilter = "";

    for (var i = 0; i < selectedValues.length; i++) {
        calendarFilter += selectedValues[i] + ','
    }
    if (calendarFilter != '') calendarFilter = calendarFilter.substring(0, calendarFilter.length - 1);

    oCalendar.assignmentsFilter = calendarFilter;

    if (rbPlannedView.GetChecked()) {
        this.schedulingView = Robotics.Client.Constants.CoverageDayView.Planified;
    } else {
        this.schedulingView = Robotics.Client.Constants.CoverageDayView.Real;
    }

    oCalendar.setFiltersCookieValue();
    oCalendar.refresh();
}

Robotics.Client.Controls.roSchedulerCalendar.prototype.importFile = function () {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    var oParameters = {};

    oParameters.firstDate = oCalendar.firstDate;
    oParameters.endDate = oCalendar.endDate;
    oParameters.employeeFilter = oCalendar.employeeFilter;
    oParameters.assignmentsFilter = oCalendar.assignmentsFilter;
    oParameters.loadRecursive = oCalendar.loadRecursive;

    oParameters.excelType = eval(oCalendar.ascxPrefix + '_dlgImport_rbExcelType2').GetChecked();
    oParameters.copyMainShifts = eval(oCalendar.ascxPrefix + '_dlgImport_ckImportCopyMainShiftsClient').GetChecked();
    oParameters.copyHolidays = eval(oCalendar.ascxPrefix + '_dlgImport_ckImportCopyHolidaysClient').GetChecked();
    oParameters.keepHolidays = eval(oCalendar.ascxPrefix + '_dlgImport_ckImportKeepHolidaysClient').GetChecked();
    oParameters.keepBloqued = eval(oCalendar.ascxPrefix + '_dlgImport_ckImportKeepBloquedDaysClient').GetChecked();

    oParameters.StampParam = new Date().getMilliseconds();

    oCalendar.performAction(oParameters, Robotics.Client.Constants.Actions.ImportFromExcel);
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.exportToExcel = function () {
    var oCalendar = this.oBaseControl;

    var oParameters = {};

    oParameters.calendar = oCalendar.oCalendar;
    oParameters.StampParam = new Date().getMilliseconds();

    oCalendar.performAction(oParameters, Robotics.Client.Constants.Actions.ExportToExcel);
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.saveAndContinue = function () {
    var oCalendar = this.oBaseControl;

    var oParameters = {};
    oParameters.calendar = oCalendar.oCalendar;
    oParameters.StampParam = new Date().getMilliseconds();

    oCalendar.performAction(oParameters, Robotics.Client.Constants.Actions.SaveAndContinue);
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.discardAndContinue = function (onAcceptFunc) {
    var oCalendar = this.oBaseControl;

    eval(oCalendar.onContinueFunc);
    oCalendar.onContinueFunc = '';
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.refresh = function () {
    var oClientMode = this;
    var oCalendar = this.oBaseControl;

    var oParameters = this.generateLoadFilters();
    oCalendar.sortColumn = -1;
    oClientMode.calendarIndictments = {};
    oClientMode.calendarIndictmentsDS = [];

    oCalendar.shiftsList = {};
    oCalendar.capacityList = {};
    oCalendar.assignmentsList = {};
    oCalendar.columnShiftsList = {};
    oCalendar.columnCapacityList = {};
    oCalendar.rowsShiftsList = {};
    oCalendar.employeeDataList = {};
    oCalendar.columnAssignmentsList = {}
    oCalendar.rowsAssignmentsList = {}
    this.resumeInfoLoaded = false;

    if (oClientMode.oBaseControl.pageLayout.east != null) oClientMode.oBaseControl.pageLayout.east.children.tabsContainerLayout.hide("south");
    if (oClientMode.oBaseControl.pageLayout.east != false && !oClientMode.oBaseControl.pageLayout.east.state.isClosed) {
        oClientMode.oBaseControl.pageLayout.toggle("east");
    }

    oCalendar.performAction(oParameters, Robotics.Client.Constants.Actions.LoadData);
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.checkCalendarIndictments = function () {
    var oClientMode = this;
    var oCalendar = this.oBaseControl;

    var oParameters = {};
    oParameters.calendar = oCalendar.oCalendar;
    oParameters.StampParam = new Date().getMilliseconds();

    oClientMode.calendarIndictments = {};
    oClientMode.calendarIndictmentsDS = [];

    oCalendar.performAction(oParameters, Robotics.Client.Constants.Actions.CheckCalendarIndictments);
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.refreshDayWithParams = function (idEmployee, selectedDay) {
    var oCalendar = this.oBaseControl;

    if (this.selectCellByEmployeeDate(idEmployee, selectedDay)) {
        var oParameters = {};

        oParameters.StampParam = new Date().getMilliseconds();
        oParameters.idEmployee = oCalendar.selectedEmployee.EmployeeData.IDEmployee;
        oParameters.idGroup = oCalendar.selectedEmployee.EmployeeData.IDGroup;
        oParameters.typeView = oCalendar.typeView;
        oParameters.loadPunches = oCalendar.loadPunches;
        oParameters.dailyPeriod = oCalendar.dailyPeriod;

        oParameters.firstDate = oCalendar.selectedDay.PlanDate;

        oCalendar.performAction(oParameters, Robotics.Client.Constants.Actions.LoadDayDefinition);
    }
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.getEmployeeCountResume = function () {
    var oCalendar = this.oBaseControl;

    var resume = "";

    if (oCalendar.oCalendar.CalendarData.length == 0) {
        var totalGroups = oCalendar.employeeFilter.count('A');
        if (totalGroups == 0) {
            resume += oCalendar.translator.translate(Robotics.Client.Language.Tags.NoSolection);
        } else {
            resume += "0 " + oCalendar.translator.translate(Robotics.Client.Language.Tags.Employees);
            if (totalGroups > 0) {
                resume += " " + oCalendar.translator.translate(Robotics.Client.Language.Tags.Connector) + " " + totalGroups;

                if (totalGroups == 1) {
                    resume += " " + oCalendar.translator.translate(Robotics.Client.Language.Tags.Department);
                } else {
                    resume += " " + oCalendar.translator.translate(Robotics.Client.Language.Tags.Departments);
                }
            }
        }
    } else {
        var resumeCounter = {};
        var resumeGroups = {};
        var totalEmployees = 0;
        var totalGroups = 0;
        for (var tmpIndex = 0; tmpIndex < oCalendar.oCalendar.CalendarData.length; tmpIndex++) {
            var idEmp = oCalendar.oCalendar.CalendarData[tmpIndex].EmployeeData.IDEmployee;
            var idGroup = oCalendar.oCalendar.CalendarData[tmpIndex].EmployeeData.IDGroup;

            if (typeof (resumeCounter[idEmp]) == 'undefined') {
                resumeCounter[idEmp] = idEmp;
                totalEmployees += 1;
            }

            if (typeof (resumeGroups[idGroup]) == 'undefined') {
                resumeGroups[idGroup] = idGroup;
                totalGroups += 1;
            }
        }

        resume += totalEmployees;

        if (totalEmployees == 1) {
            resume += " " + oCalendar.translator.translate(Robotics.Client.Language.Tags.Employee);
        } else {
            resume += " " + oCalendar.translator.translate(Robotics.Client.Language.Tags.Employees);
        }

        if (totalGroups > 0) {
            resume += " " + oCalendar.translator.translate(Robotics.Client.Language.Tags.Connector) + " " + totalGroups;
            if (totalGroups == 1) {
                resume += " " + oCalendar.translator.translate(Robotics.Client.Language.Tags.Department);
            } else {
                resume += " " + oCalendar.translator.translate(Robotics.Client.Language.Tags.Departments);
            }
        }
    }

    return resume;
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.assignShiftToDayBatch = function (mainShift, holidayShift, shiftAssignment, allowAssignment, keepLocked, keepDesHolidays) {
    var oClientMode = this;
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    this.copyInProgress = true;
    for (var i = oCalendar.selectedMinRow; i <= oCalendar.selectedMaxRow; i++) {
        for (var x = oCalendar.selectedMinColumn; x <= oCalendar.selectedMaxColumn; x++) {
            var destinationCellRow = i;
            var destinationCellColumn = x;

            var bAssign = true;

            var destinationDay = null;
            if (oCalendar.viewRange != Robotics.Client.Constants.ViewRange.Daily) {
                if (destinationCellRow < oCalData.CalendarData.length && destinationCellColumn < oCalData.CalendarData[destinationCellRow].PeriodData.DayData.length) {
                    bAssign = true;
                    destinationDay = oCalData.CalendarData[destinationCellRow].PeriodData.DayData[destinationCellColumn];
                } else {
                    bAssign = false;
                }
            } else {
                if (destinationCellRow < oCalData.CalendarData.length) {
                    bAssign = true;
                    destinationDay = oCalData.CalendarData[destinationCellRow].PeriodData.DayData[0];
                } else {
                    bAssign = false;
                }
            }

            if (bAssign) {
                if (typeof allowAssignment == 'undefined') allowAssignment = destinationDay.AllowAssignment;
                if (typeof shiftAssignment == 'undefined') shiftAssignment = destinationDay.AssigData;

                if ($('#' + oCalendar.ascxPrefix + '_calCell_' + destinationCellRow + '_' + destinationCellColumn).hasClass('ui-selected')) {
                    this.assignShiftToDay($('#' + oCalendar.ascxPrefix + '_calCell_' + destinationCellRow + '_' + destinationCellColumn), destinationDay, mainShift, holidayShift, shiftAssignment, allowAssignment, keepLocked, keepDesHolidays, oCalData.CalendarData[destinationCellRow].EmployeeData);//.Assignments, oCalData.CalendarData[destinationCellRow].EmployeeData.FreezingDate);
                }
            }
        }
    }
    if (oCalendar.loadIndictments) oClientMode.checkCalendarIndictments();
    this.copyInProgress = false;
};

Robotics.Client.Controls.roSchedulerCalendar.prototype.assignShiftToDay = function (container, destinationDay, mainShift, holidayShift, shiftAssignment, allowAssignment, keepLocked, keepDesHolidays, employeeData) {// lstEmployeeAssignments, employeeFreezingDate) {
    var oClientMode = this;
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    if (destinationDay.EmployeeStatusOnDay == Robotics.Client.Constants.EmployeeStatusOnDay.Ok) {
        var bAssign = true;

        if (!destinationDay.CanBeModified) {
            if (!oCalendar.isBatchMode()) oCalendar.showErrorPopup("Error.Title", "error", "Calendar.Client.NoPermission", "", "Error.OK", "Error.OKDesc", oCalendar.defaultMessageAction);
            bAssign = false;
        } else if (moment(destinationDay.PlanDate) <= moment(employeeData.FreezingDate)) {
            if (!oCalendar.isBatchMode()) oCalendar.showErrorPopup("Error.Title", "error", "Calendar.Client.FreezeDate", "", "Error.OK", "Error.OKDesc", oCalendar.defaultMessageAction);
            bAssign = false;
        } else if (typeof destinationDay.Alerts != 'undefined' && holidayShift != null && destinationDay.Alerts.OnHolidaysHours) {
            if (!oCalendar.isBatchMode()) oCalendar.showErrorPopup("Error.Title", "error", "Calendar.Client.OverlappedHolidays", "", "Error.OK", "Error.OKDesc", oCalendar.defaultMessageAction);
            bAssign = false;
        } else if (destinationDay.IDDailyBudgetPosition > 0) {
            if (!(mainShift != null && (destinationDay.MainShift.ExistComplementaryData || destinationDay.MainShift.ExistFloatingData) && oCalendar.complementaryManager.canChangeShift(true, destinationDay, mainShift, shiftAssignment))) {
                if (!oCalendar.isBatchMode()) oCalendar.showErrorPopup("Error.Title", "error", "Calendar.Client.BudgetAssigned", "", "Error.OK", "Error.OKDesc", oCalendar.defaultMessageAction);
                bAssign = false;
            }
        } else if (destinationDay.Locked) {
            if (!oCalendar.isBatchMode() && !oCalendar.selectionCopied) {
                oCalendar.showErrorPopup("Error.Title", "error", "Calendar.Client.LockedDay", "", "Error.OK", "Error.OKDesc", oCalendar.defaultMessageAction);
                bAssign = false;
            } else {
                if (keepLocked) bAssign = false;
            }
        }

        if (bAssign) {
            var currentDayShift = this.getAssignedShift(destinationDay);
            var oldShiftCopy = Object.clone(currentDayShift, true);

            if (holidayShift != null) {
                if (currentDayShift != null && (currentDayShift.Type == Robotics.Client.Constants.ShiftType.Holiday || currentDayShift.Type == Robotics.Client.Constants.ShiftType.Holiday_NonWorking)) { currentDayShift = destinationDay.ShiftBase; }

                if (currentDayShift == null && mainShift == null) {
                    if (!oCalendar.isBatchMode()) oCalendar.showErrorPopup("Error.Title", "error", "Calendar.Client.BaseShiftNeeded", "", "Error.OK", "Error.OKDesc", oCalendar.defaultMessageAction);
                    bAssign = false;
                } else {
                    if (holidayShift.Type == Robotics.Client.Constants.ShiftType.Holiday && currentDayShift.PlannedHours == 0) {
                        if (!oCalendar.isBatchMode()) oCalendar.showErrorPopup("Error.Title", "error", "Calendar.Client.WorkingBaseShiftNeeded", "", "Error.OK", "Error.OKDesc", oCalendar.defaultMessageAction);
                        bAssign = false;
                    }
                }

                if (bAssign) {
                    var shiftUsed = holidayShift;

                    var newBaseShift = null;
                    if (mainShift != null) {
                        newBaseShift = mainShift;
                    } else {
                        newBaseShift = {
                            ID: currentDayShift.ID,
                            ShortName: currentDayShift.ShortName,
                            PlannedHours: currentDayShift.PlannedHours,
                            Color: currentDayShift.Color,
                            Name: currentDayShift.Name,
                            Type: currentDayShift.Type,
                            StartHour: moment(currentDayShift.StartHour).clone().toDate(),
                            EndHour: moment(currentDayShift.EndHour).clone().toDate(),
                            AdvancedParameters: Object.clone(currentDayShift.AdvancedParameters, true),
                            BreakHours: currentDayShift.BreakHours,
                            ExistComplementaryData: currentDayShift.ExistComplementaryData,
                            ExistFloatingData: currentDayShift.ExistFloatingData,
                            ShiftLayers: currentDayShift.ShiftLayers,
                            ShiftLayersDefinition: []
                        };
                        for (var i = 0; i < newBaseShift.ShiftLayers; i++) {
                            newBaseShift.ShiftLayersDefinition.push({
                                LayerStartTime: moment(currentDayShift.ShiftLayersDefinition[i].LayerStartTime).clone().toDate(),
                                ExistLayerStartTime: currentDayShift.ShiftLayersDefinition[i].ExistLayerStartTime,
                                LayerOrdinaryHours: currentDayShift.ShiftLayersDefinition[i].LayerOrdinaryHours,
                                LayerDuration: currentDayShift.ShiftLayersDefinition[i].LayerDuration,
                                ExistLayerDuration: currentDayShift.ShiftLayersDefinition[i].ExistLayerDuration,
                                LayerID: currentDayShift.ShiftLayersDefinition[i].LayerID,
                                LayerComplementaryHours: currentDayShift.ShiftLayersDefinition[i].LayerComplementaryHours
                            });
                        }
                    }

                    destinationDay.ShiftBase = newBaseShift;
                    destinationDay.MainShift = shiftUsed;
                    destinationDay.IsHoliday = true;

                    if (destinationDay.Alerts != null) {
                        destinationDay.Alerts.OnHolidays = true;
                    } else {
                        destinationDay.Alerts = { OnAbsenceDays: false, OnAbsenceHours: false, OnHolidays: true, UnexpectedlyAbsent: false, OnHolidaysHours: false, OnOvertimesHours: false };
                    }

                    if (moment(destinationDay.PlanDate) <= moment(new Date())) destinationDay.ShiftUsed = shiftUsed;
                }
            } else {
                if (mainShift != null) {
                    var shiftUsed = mainShift;

                    if (keepDesHolidays && currentDayShift != null && (currentDayShift.Type == Robotics.Client.Constants.ShiftType.Holiday || currentDayShift.Type == Robotics.Client.Constants.ShiftType.Holiday_NonWorking)) {
                        if (shiftUsed != null) destinationDay.ShiftBase = shiftUsed;
                    } else {
                        if (currentDayShift != null && (currentDayShift.Type == Robotics.Client.Constants.ShiftType.Holiday || currentDayShift.Type == Robotics.Client.Constants.ShiftType.Holiday_NonWorking)) {
                            destinationDay.ShiftBase = null;
                        }

                        destinationDay.IsHoliday = false;
                        destinationDay.MainShift = shiftUsed;

                        if (destinationDay.Alerts != null) {
                            destinationDay.Alerts.OnHolidays = false;
                        } else {
                            destinationDay.Alerts = { OnAbsenceDays: false, OnAbsenceHours: false, OnHolidays: false, UnexpectedlyAbsent: false, OnHolidaysHours: false, OnOvertimesHours: false };
                        }

                        if (moment(destinationDay.PlanDate) <= moment(new Date())) destinationDay.ShiftUsed = shiftUsed;
                    }
                } else {
                    bAssign = false;
                }
            }

            if (oCalendar.isScheduleActive) {
                destinationDay.AllowAssignment = allowAssignment;

                if (allowAssignment) {
                    if (typeof destinationDay.AssigData == 'undefined') destinationDay.AssigData = null;
                    if (typeof shiftAssignment == 'undefined') shiftAssignment = null;

                    if ((shiftAssignment == null && destinationDay.AssigData != null) || (shiftAssignment != null && destinationDay.AssigData == null) || (shiftAssignment != null && destinationDay.AssigData != null && shiftAssignment.ID != destinationDay.AssigData.ID)) {
                        var bValidAssignment = true;
                        if (shiftAssignment != null) {
                            if (typeof employeeData != 'undefined' && employeeData.Assignments != null && employeeData.Assignments.length > 0) {
                                bValidAssignment = false;
                                for (var iAssIndex = 0; iAssIndex < employeeData.Assignments.length; iAssIndex++) {
                                    if (employeeData.Assignments[iAssIndex].ID == shiftAssignment.ID) {
                                        bValidAssignment = true;
                                        break;
                                    }
                                }
                            } else {
                                bValidAssignment = false;
                            }

                            if (!bValidAssignment) {
                                if (!oCalendar.isBatchMode()) oCalendar.showErrorPopup("Error.Title", "error", "Calendar.Client.AssignmentNotAllowedByEmployee", "", "Error.OK", "Error.OKDesc", oCalendar.defaultMessageAction);
                            }

                            if (bValidAssignment) {// #TODO El puesto se puede asignar al horario
                                var oTmpCal = oCalendar;
                                bValidAssignment = false;
                                Object.keys(oTmpCal.shiftsExtendedDataCache).forEach(function (shiftKey, index) {
                                    if ((destinationDay.IsHoliday == false && shiftKey == destinationDay.MainShift.ID) || (destinationDay.IsHoliday && shiftKey == destinationDay.ShiftBase.ID)) {
                                        var shiftDefinition = oTmpCal.shiftsExtendedDataCache[shiftKey];
                                        if (typeof shiftDefinition.Assignments != 'undefined' && shiftDefinition.Assignments != 'null' && shiftDefinition.Assignments.length > 0) {
                                            for (var iAssIndex = 0; iAssIndex < shiftDefinition.Assignments.length; iAssIndex++) {
                                                if (shiftDefinition.Assignments[iAssIndex].IDAssig == shiftAssignment.ID) {
                                                    bValidAssignment = true;
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                });

                                if (!bValidAssignment) {
                                    if (!oCalendar.isBatchMode()) oCalendar.showErrorPopup("Error.Title", "error", "Calendar.Client.AssignmentNotAllowedByShift", "", "Error.OK", "Error.OKDesc", oCalendar.defaultMessageAction);
                                }
                            }
                        }

                        if (bValidAssignment) {
                            destinationDay.AllowAssignment = allowAssignment;
                            destinationDay.AssigData = shiftAssignment;
                            bAssign = true;
                        } else {
                            bAssign = false;
                        }
                    }
                } else {
                    if (typeof shiftAssignment == 'undefined') shiftAssignment = null;

                    if (shiftAssignment != null) { //Intentan asignar un puesto a un horario que no admite puestos
                        bAssign = false;
                        if (!oCalendar.isBatchMode()) oCalendar.showErrorPopup("Error.Title", "error", "Calendar.Client.ShiftNotAllowAssignments", "", "Error.OK", "Error.OKDesc", oCalendar.defaultMessageAction);
                    } else {
                        if (destinationDay.AssigData != null) {
                            bAssign = true;
                            destinationDay.AssigData = null;
                            destinationDay.AllowAssignment = allowAssignment;
                        }
                    }
                }
            }

            if (bAssign) {
                destinationDay.HasChanged = true;

                if (typeof (destinationDay.TelecommuteForced) != 'undefined' && destinationDay.TelecommuteForced == false) {
                    destinationDay.TelecommutingExpected = false;
                    destinationDay.TelecommutingOptional = false;

                    if (destinationDay.CanTelecommute) {
                        if ((typeof (destinationDay.MainShift) != 'undefined' && destinationDay.MainShift != null)) {
                            var dayOfWeek = '' + moment(destinationDay.PlanDate).day();
                            if (destinationDay.TelecommutingMandatoryDays.indexOf(dayOfWeek) >= 0) {
                                destinationDay.TelecommutingExpected = true;
                                destinationDay.TelecommutingOptional = false;
                            } else if (destinationDay.TelecommutingOptionalDays.indexOf(dayOfWeek) >= 0) {
                                destinationDay.TelecommutingExpected = false;
                                destinationDay.TelecommutingOptional = true;
                            }
                        }
                    }
                }

                if (oCalendar.viewRange != Robotics.Client.Constants.ViewRange.Daily) {
                    container.empty();
                    this.buildCalendarCellContent(container, destinationDay, false, employeeData.FreezingDate);
                } else {
                    this.reloadHourDayData();
                }

                setTimeout(function () {
                    if (oCalendar.loadIndictments == true) {
                        var cellId = oCalendar.ascxPrefix + '_tooltipIndictment_' + container.attr("data-IDRow") + '_' + container.attr("data-IDColumn");

                        $('#' + cellId).off('mouseover');
                        $('#' + cellId).on('mouseover', function () {
                            var cell = $(this).find('.tooltipIndictmentsContainer');
                            if (cell.length == 0) return;
                            var cellId = this.id;
                            $(cell).dxTooltip({
                                target: "#" + cellId,
                                hideEvent: "mouseleave",
                                position: 'bottom'
                            });
                            $(cell).dxTooltip("instance").show();
                        });
                    }

                    if (oCalendar.loadPunches) {
                        var cellId = oCalendar.ascxPrefix + '_tooltipPunchesReview_' + container.attr("data-IDRow") + '_' + container.attr("data-IDColumn");

                        $('#' + cellId).off('mouseover');
                        $('#' + cellId).on('mouseover', function () {
                            var cell = $(this).find('.tooltipPunchesReviewContainer');
                            if (cell.length == 0) return;
                            var cellId = this.id;
                            $(cell).dxTooltip({
                                target: "#" + cellId,
                                hideEvent: "mouseleave",
                                position: 'bottom'
                            });
                            $(cell).dxTooltip("instance").show();
                        });
                    }
                }, 100);

                oCalendar.setHasChanges(true);

                if (oClientMode.copyInProgress == false && oCalendar.loadIndictments) oClientMode.checkCalendarIndictments();

                var newShiftInfo = Object.clone(destinationDay.MainShift, true);
                oClientMode.updateShiftTotalizerInfo(oldShiftCopy, newShiftInfo, container.attr("data-IDRow"), container.attr("data-IDColumn"), destinationDay, false);
            }
        }
    } else {
        if (destinationDay.EmployeeStatusOnDay == Robotics.Client.Constants.EmployeeStatusOnDay.NoContract) {
            if (!oCalendar.isBatchMode()) oCalendar.showErrorPopup("Error.Title", "error", "Calendar.Client.NoContract", "", "Error.OK", "Error.OKDesc", oCalendar.defaultMessageAction);
        } else if (destinationDay.EmployeeStatusOnDay == Robotics.Client.Constants.EmployeeStatusOnDay.InOtherDepartment) {
            if (!oCalendar.isBatchMode()) oCalendar.showErrorPopup("Error.Title", "error", "Calendar.Client.InOtherDepartment", "", "Error.OK", "Error.OKDesc", oCalendar.defaultMessageAction);
        }
    }
};