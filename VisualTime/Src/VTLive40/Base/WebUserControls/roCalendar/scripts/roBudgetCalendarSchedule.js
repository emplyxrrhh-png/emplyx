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

Robotics.Client.Controls.roBudgetCalendarSchedule = function (baseControl) {
    this.name = "Robotics.Client.Controls.roBudgetCalendarSchedule";

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
            , center__onresize_start: this.onresize(this)
            , center__onresize_end: this.onresize(this)
            , south__paneSelector: "#" + this.oBaseControl.prefix + Robotics.Client.Constants.LayoutNames.ColumnTabs
            , south__slidable: true
            , south__closable: true
            , south__onopen: this.synchronizeContentLayout(true, this)
            , south__onclose: this.synchronizeContentLayout(false, this)
            , south__initClosed: true
            , spacing_closed: 25
        }
    };

    this.OnSelectedCell = null;
    this.OnDayClick = null;

    this.employeeAssignmentData = null;
}

Robotics.Client.Controls.roBudgetCalendarSchedule.prototype.getLayout = function () {
    return this.pageLayoutOptions;
}

Robotics.Client.Controls.roBudgetCalendarSchedule.prototype.setHasChanges = function (bolHasChanges) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    oCalendar.hasChanges = bolHasChanges;

    if (!oCalendar.hasChanges && oCalData != null && oCalData.BudgetData != null) {
        for (var i = 0; i < oCalData.BudgetData.length; i++) {
            oCalData.BudgetData[i].RowState = Robotics.Client.Constants.BudgetRowState.NoChanged;
            if (oCalData.BudgetData[i].PeriodData.DayData != null) {
                for (var x = 0; x < oCalData.BudgetData[i].PeriodData.DayData.length; x++) {
                    oCalData.BudgetData[i].PeriodData.DayData[x].HasChanged = false;
                }
            }
        }
    }

    try {
        hasChanges(bolHasChanges);
    } catch (e) { }
};

Robotics.Client.Controls.roBudgetCalendarSchedule.prototype.create = function () {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    this.buildHTMLStructure();

    oCalendar.pageLayout = oCalendar.container.layout(this.pageLayoutOptions);

    $('#' + oCalendar.prefix + Robotics.Client.Constants.LayoutNames.ColumnTabs).tabs();
};

Robotics.Client.Controls.roBudgetCalendarSchedule.prototype.buildHTMLStructure = function () {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    var mainCenterLayout = $('<div></div>').attr('id', oCalendar.prefix + Robotics.Client.Constants.LayoutNames.Center).attr('class', 'ui-layout-center');

    //Panel para el calendario
    var calendarLayout = $('<div></div>').attr('id', oCalendar.prefix + Robotics.Client.Constants.LayoutNames.Calendar).attr('class', 'ui-layout-center');

    //Panel para el listado de tablas resumen inferior
    var columnInfoLayout = $('<div></div>').attr('id', oCalendar.prefix + Robotics.Client.Constants.LayoutNames.ColumnTabs).attr('class', 'ui-layout-south container tabs');

    //Listado de tabs que se incluyen en el listado inferior
    var columnTabsList = $('<ul></ul>').attr('id', 'tabbuttonsColumn');
    var assignmentsTab = $('<li></li>').attr('class', 'tab2').attr('style', 'cursor:pointer').append($('<a></a>').attr('href', '#' + oCalendar.prefix + Robotics.Client.Constants.LayoutNames.ColumnTabAssignments).html(Globalize.formatMessage('roAvailableEmployeeAssignments')));
    columnTabsList.append(assignmentsTab);

    //Paneles con el contenido de los tabs
    var columnTabsPanel = $('<div></div>').attr('id', 'tabpanelsColumn').attr('style', 'border-top: 0');
    columnTabsPanel.append($('<div></div>').attr('id', oCalendar.prefix + Robotics.Client.Constants.LayoutNames.ColumnTabAssignments).attr('class', 'container columnTableMain'));

    columnInfoLayout.append(columnTabsList, columnTabsPanel);

    // Añadimos todo al panel central
    mainCenterLayout.append(calendarLayout, columnInfoLayout);

    oCalendar.container.empty();
    oCalendar.container.append(mainCenterLayout);
};

Robotics.Client.Controls.roBudgetCalendarSchedule.prototype.onresize = function (oScheduleCalendar) {
    return function () {
        oScheduleCalendar.loadingFunctionExtended(true);
        oScheduleCalendar.oBaseControl.refreshTables(oScheduleCalendar, true, true);
        oScheduleCalendar.loadingFunctionExtended(false);
    };
};

Robotics.Client.Controls.roBudgetCalendarSchedule.prototype.synchronizeContentLayout = function (isShown, oScheduleCalendar) {
    return function () {
        if (isShown) {
            var oTmpCalendar = eval(oScheduleCalendar.oBaseControl.clientInstanceName + ".clientMode.employeeAssignmentData");

            if (oTmpCalendar == null) {
                oScheduleCalendar.loadEmployeeDetail();
            } else {
                eval(oScheduleCalendar.oBaseControl.clientInstanceName + ".clientMode.refreshColumnassignmentsTable();");
            }
        }
    };
};

Robotics.Client.Controls.roBudgetCalendarSchedule.prototype.loadingFunctionExtended = function (showLoading) {
    var oCal = this.oBaseControl;

    if ((oCal.isShowingLoader && !showLoading) || (!oCal.isShowingLoader && showLoading)) {
        oCal.isShowingLoader = showLoading;

        if (showLoading) {
            $('#loadingCalendar').show();
        } else {
            $('#loadingCalendar').hide();
        }

        oCal.loadingFunc(showLoading);
    }
};

Robotics.Client.Controls.roBudgetCalendarSchedule.prototype.getContextMenuSelector = function () {
    var oClientMode = this;
    var oCal = this.oBaseControl;

    var selector = oCal.viewRange == Robotics.Client.Constants.ViewRange.Daily ? '.calendarDailyBodyCell' : '.calendarOuterBodyCell';

    return selector;
}

Robotics.Client.Controls.roBudgetCalendarSchedule.prototype.getContextMenuHeaderSelector = function () {
    var oClientMode = this;
    var oCal = this.oBaseControl;

    var selector = '.CalendarDayFixedHeader';

    return selector;
}

Robotics.Client.Controls.roBudgetCalendarSchedule.prototype.processKeyUpEvent = function (e) {
    var oClientMode = this;

    oClientMode.cancelCurrentMultipleSelect(null, false);
    oClientMode.cancelCurrentMultipleHeaderSelect(null, false);
}

Robotics.Client.Controls.roBudgetCalendarSchedule.prototype.processKeyDownEvent = function (e) {
    var oClientMode = this;
    var oCal = this.oBaseControl;

    if (oCal.ctrlDown && e.keyCode == oCal.cKey) {
        if (oCal.selectedMinHeaderColumn != -1 && oCal.selectedMaxHeaderColumn != -1) {
            oClientMode.copyHeaderSelection(oCal.selectedHeaderContainer);
        } else {
            oClientMode.copySelection(oCal.selectedContainer);
        }
    }
    if (oCal.ctrlDown && e.keyCode == oCal.vKey) {
        if (oCal.selectionCopied) oClientMode.pasteSelection(oCal.selectedContainer);
        else if (oCal.selectionHeaderCopied) oClientMode.pasteHeaderSelection(oCal.selectedHeaderContainer);
    }
}

Robotics.Client.Controls.roBudgetCalendarSchedule.prototype.onDrop = function (event) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    this.setSingleSelectedObject($(event.target));

    var srcElement = null;

    if (typeof (event.srcElement) != 'undefined') {
        srcElement = $(event.srcElement);
    } else {
        srcElement = $(event.originalEvent.target);
    }

    if (typeof srcElement.attr("data-IDMode") != 'undefined') {
        var modePositions = oCalendar.selectedEmployee.ProductiveUnitData.ProductiveUnit.UnitModes.find(function (mode) {
            return mode.ID == parseInt(srcElement.attr("data-IDMode"), 10);
        });

        if (typeof modePositions != 'undefined') {
            var selMode = {
                CostValue: parseFloat(srcElement.attr('data-CostValue')),
                Coverage: 0,
                HtmlColor: srcElement.attr("data-HtmlColor"),
                ID: parseInt(srcElement.attr("data-IDMode"), 10),
                IDProductiveUnit: oCalendar.selectedEmployee.ProductiveUnitData.ProductiveUnit.ID,
                Name: srcElement.attr("data-Name"),
                ShortName: srcElement.attr("data-ShortName"),
                UnitModePositions: Object.clone(modePositions.UnitModePositions)
            }

            for (var i = 0; i < selMode.UnitModePositions.length; i++) {
                selMode.UnitModePositions[i].ID = -1;
            }

            if (!oCalendar.isBatchMode()) {
                this.assignUnitModeToDay(oCalendar.selectedContainer, oCalendar.selectedDay, selMode);
            } else {
                this.assignUnitModeToDayBatch(selMode);
            }
        } else {
            oCalendar.showErrorPopup("Error.Title", "error", "Calendar.Client.ModeNotAllowedForUnit", "", "Error.OK", "Error.OKDesc", oCalendar.defaultMessageAction);
        }
    }
};

Robotics.Client.Controls.roBudgetCalendarSchedule.prototype.mapModeEvents = function () {
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

    //$('.RecursiveAction').off('click');
    //$('.RecursiveAction').on('click', function (e) {
    //    oCal.preparePUnitsDialog();
    //});

    $('.LoadIndictments').off('click');
    $('.LoadIndictments').on('click', function (e) {
        oClientMode.alterLoadIndictmentsModeAndReload();
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
        oClientMode.enterDetailAction(parseInt($(this).attr('data-IDPunit'), 10), parseInt(oCal.employeeFilter, 10), $(this).attr('data-Date'));
    });

    $(selector).droppable({
        drop: function (event, ui) {
            return oClientMode.onDrop(event);
        }
    });

    try { $("#" + oCal.prefix + Robotics.Client.Constants.TableNames.Calendar + '_tbody').selectableScroll('destroy'); } catch (e) { }
    $("#" + oCal.prefix + Robotics.Client.Constants.TableNames.Calendar + '_tbody').selectableScroll({
        distance: 10,
        filter: '.calendarOuterBodyCell',
        stop: function (event, ui) { oClientMode.endSelectOperation(); },
        scrollElement: $('#' + oCal.prefix + Robotics.Client.Constants.LayoutNames.Calendar + ' .fht-fixed-body .fht-tbody')
    });

    $('.CalendarDayFixedHeader').off('click');
    $('.CalendarDayFixedHeader').on('click', function (e) {
        if (!oCal.shiftDown) oClientMode.setSingleHeaderSelectedObejct(this);
        else oClientMode.selectHeaderMultiple(oCal.selectedHeaderContainer, this);
    });

    if (oCal.viewRange == Robotics.Client.Constants.ViewRange.Daily) {
        $('#' + oCal.prefix + 'Main div.fht-fixed-body div.fht-tbody').scrollLeft((oCal.firstCellPrinted - 46) * 30);
        oCal.firstCellPrinted = -1;
    }
    $('.tooltipDiv').each(function (index) {
        var cellId = this.id;
        var containerId = cellId.replace('_tooltipInfo_', '_calCell_');
        $("#" + cellId).dxTooltip({
            target: "#" + containerId,
            showEvent: { name: "mouseenter", delay: 300 },
            hideEvent: "mouseleave",
            position: 'top'
        });
    });

    $('.tooltipIndictmentsContainer').each(function (index) {
        var cellId = this.id;
        var containerId = cellId.replace('_tooltipIndictmentsContainer_', '_tooltipIndictments_');
        $("#" + cellId).dxTooltip({
            target: "#" + containerId,
            showEvent: { name: "mouseenter", delay: 300 },
            hideEvent: "mouseleave",
            position: 'bottom'
        });
    });

    window.focus();
};

Robotics.Client.Controls.roBudgetCalendarSchedule.prototype.refreshModeTables = function (objectRef, isResizing, refreshCalendar) {
    var oScheduleCalendar = this;

    if (objectRef != null) {
        if (objectRef instanceof Robotics.Client.Controls.roCalendar) oScheduleCalendar = objectRef.clientMode;
        else oScheduleCalendar = objectRef;
    }

    if (refreshCalendar) oScheduleCalendar.refreshCalendarTable(isResizing);

    if (!isResizing && !oScheduleCalendar.oBaseControl.pageLayout.center.children.tabsContainerLayout.south.state.isClosed) {
        oScheduleCalendar.refreshColumnassignmentsTable();
    }

    if (refreshCalendar) this.oBaseControl.mapEvents();
};

Robotics.Client.Controls.roBudgetCalendarSchedule.prototype.refreshCalendarTable = function (isResizing) {
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

Robotics.Client.Controls.roBudgetCalendarSchedule.prototype.refreshColumnassignmentsTable = function () {
    var oCalendar = this.oBaseControl;

    try {
        $('#' + oCalendar.prefix + Robotics.Client.Constants.TableNames.ColAssignments).fixedHeaderTable('destroy');

        $('#' + oCalendar.prefix + Robotics.Client.Constants.TableNames.ColAssignments).find('tfoot').each(function (index) {
            if (this.id == '') {
                $(this).remove();
            }
        });
    } catch (e) { }

    this.createResumeTables(Robotics.Client.Constants.TableNames.ColAssignments, Robotics.Client.Constants.LayoutNames.ColumnTabAssignments);

    $('#' + oCalendar.prefix + Robotics.Client.Constants.TableNames.ColAssignments).fixedHeaderTable({
        altClass: 'odd',
        footer: true,
        fixedColumn: true
    });

    $('#' + oCalendar.prefix + Robotics.Client.Constants.TableNames.Calendar).parent().on('scroll', function () {
        $('#' + oCalendar.prefix + Robotics.Client.Constants.TableNames.ColAssignments).parent().scrollLeft($(this).scrollLeft());
    });

    $('.employeeTooltipDiv').each(function (index) {
        var cellId = this.id;
        var containerId = cellId.replace('_EmployeeTooltipInfo_', '_EmployeeAvailableCell_');
        $("#" + cellId).dxTooltip({
            target: "#" + containerId,
            showEvent: { name: "mouseenter", delay: 300 },
            hideEvent: "mouseleave",
            position: 'top'
        });
    });
};

Robotics.Client.Controls.roBudgetCalendarSchedule.prototype.createResumeTables = function (idTable, parentId) {
    switch (idTable) {
        case Robotics.Client.Constants.TableNames.Calendar:
            this.createCalendarTable(idTable, parentId);
            break;
        case Robotics.Client.Constants.TableNames.ColAssignments:
            this.createColumnAssignmentsTable(idTable, parentId);
            break;
    }
};

Robotics.Client.Controls.roBudgetCalendarSchedule.prototype.createCalendarTable = function (idTable, parentId) {
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

    tableContainer.append(tableElement);
};

Robotics.Client.Controls.roBudgetCalendarSchedule.prototype.createCalendarTableHeader = function (idTable, parentId) {
    var oClientMode = this;
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    //Creamos el header
    var tHeader = $('<thead></thead>').attr('id', oCalendar.prefix + idTable + '_thead').attr('class', 'HeaderSelectable');

    var tHeaderRow = $('<tr></tr>');

    //Creamos la primera columna que sera el header
    var tFixedHeaderCell = $('<th></th>');
    var mainFixedHeaderDiv = $('<div></div>').attr('class', 'BudgetFixed CalendarFixedHeader');

    var pUnitHeaderCell = $('<div></div>').attr('class', 'CalendarBudgetFixed CalendarBudgetFixedHeader');

    var pUnitDescription = $('<div ></div>').attr('class', 'BudgetTitleFixed');
    pUnitDescription.append($('<span></span>').attr('class', 'ProductiveUnitTextNorth').html(oCalData.BudgetHeader.ProductiveUnitHeaderData.Row1Text));

    var pUnitActions = $('<div"></div>').attr('class', 'BudgetActionsFixed');

    var northCell = $('<div></div>').attr('class', 'NorthCell NorthCellHeaderEmployee');
    var expandIcon = $('<div></div>').attr('class', 'LoadIndictments ' + (oCalendar.loadIndictments == false ? 'showIndictments-inactive' : 'showIndictments-active')).attr('title', Globalize.formatMessage("roShowIndictments"));
    northCell.append($('<div></div>').attr('class', 'EmpCellIconsNorth').append(expandIcon));

    var southCell = $('<div></div>').attr('class', 'SouthCell');
    var filterIcon = $('<div></div>').attr('class', 'FilterAction ' + (oCalendar.assignmentsFilter == '' ? 'filterGroup' : 'filter-activeGroup')).attr('title', oCalendar.translator.translate(Robotics.Client.Language.Tags.Scheduler_Filter));
    southCell.append($('<div></div>').attr('class', 'EmpCellIconsSouth').append(filterIcon));

    pUnitActions.append(northCell, southCell);
    pUnitHeaderCell.append(pUnitDescription, pUnitActions)

    tHeaderRow.append(tFixedHeaderCell.append(pUnitHeaderCell));

    if (oCalendar.viewRange == Robotics.Client.Constants.ViewRange.Daily) {
        for (var i = oCalendar.getMinDailyCell(); i < oCalendar.getMaxDailyCell(); i++) {
            var dayHeaderCell = $('<th></th>');

            var mainDayHeaderCell = $('<div></div>').attr('id', oCalendar.ascxPrefix + '_calHeaderCell_' + i).attr('class', 'CalendarDayFixed CalendarDayFixedHeader DailyCell').attr('data-IDColumn', i).attr('style', 'background:' + oCalData.BudgetHeader.PeriodHeaderData[i].BackColor);

            mainDayHeaderCell.append($('<div></div>').attr('class', 'NorthCell dayInfo DailyCell').html(oCalData.BudgetHeader.PeriodHeaderData[i].Row2Text));

            if (oCalData.BudgetHeader.PeriodHeaderData[i].FeastDay) mainDayHeaderCell.append($('<div></div>').attr('class', 'columnIsFeast'));

            tHeaderRow.append(dayHeaderCell.append(mainDayHeaderCell));
        }
    } else {
        for (var i = 0; i < oCalData.BudgetHeader.PeriodHeaderData.length; i++) {
            var dayHeaderCell = $('<th></th>');

            var mainDayHeaderCell = $('<div></div>').attr('id', oCalendar.ascxPrefix + '_calHeaderCell_' + i).attr('class', 'CalendarDayFixed CalendarDayFixedHeader').attr('data-IDColumn', i).attr('style', 'background:' + oCalData.BudgetHeader.PeriodHeaderData[i].BackColor);

            mainDayHeaderCell.append($('<div></div>').attr('class', 'NorthCell dayInfo').html(oCalData.BudgetHeader.PeriodHeaderData[i].Row1Text + "</br>" + oCalData.BudgetHeader.PeriodHeaderData[i].Row2Text));

            if (oCalData.BudgetHeader.PeriodHeaderData[i].FeastDay) mainDayHeaderCell.append($('<div></div>').attr('class', 'columnIsFeast'));

            tHeaderRow.append(dayHeaderCell.append(mainDayHeaderCell));
        }
    }

    var tmpCell = $('<th style="width:100%"></th>').html('&nbsp;');
    tHeaderRow.append(tmpCell);
    tHeader.append(tHeaderRow);

    return tHeader;
};

Robotics.Client.Controls.roBudgetCalendarSchedule.prototype.createCalendarTableBody = function (idTable, parentId) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    //Creamos el body
    var tBody = $('<tbody></tbody>').attr('id', oCalendar.prefix + idTable + '_tbody').attr('class', 'bodySelectable');

    if (oCalData.BudgetData != null && oCalData.BudgetData.length > 0) {
        for (var i = 0; i < oCalData.BudgetData.length; i++) {
            var tBodyRow = $('<tr></tr>');

            var tFixedBodyCell = $('<td></td>');

            var mainFixedBodyCell = $('<div></div>').attr('class', 'BudgetFixed CalendarFixedHeader');

            var fixedEmployeeBodyCell = $('<div></div>').attr('class', 'CalendarBudgetFixed CalendarBudgetFixedBody');

            fixedEmployeeBodyCell.append($('<div></div>').attr('class', 'EmployeeName').attr('title', oCalData.BudgetData[i].ProductiveUnitData.ProductiveUnit.Name).html(oCalData.BudgetData[i].ProductiveUnitData.ProductiveUnit.Name));

            tBodyRow.append(tFixedBodyCell.append(mainFixedBodyCell.append(fixedEmployeeBodyCell)));

            this.buildPlanificationRowView(i, tBodyRow);

            tmpCell = $('<td style="width:100%"></td>').html('&nbsp;');
            tBodyRow.append(tmpCell);

            tBody.append(tBodyRow);
        }
    } else {
        var tBodyRow = $('<tr></tr>');
        var tFixedBodyCell = $('<td></td>');
        var mainFixedBodyCell = $('<div></div>').attr('class', 'BudgetFixed CalendarFixedHeader');
        var fixedEmployeeBodyCell = $('<div></div>').attr('class', 'CalendarBudgetFixed CalendarBudgetFixedHeader');

        tBodyRow.append(tFixedBodyCell.append(mainFixedBodyCell.append(fixedEmployeeBodyCell)));

        for (var columnIndex = 0; columnIndex < oCalData.BudgetHeader.PeriodHeaderData.length; columnIndex++) {
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

Robotics.Client.Controls.roBudgetCalendarSchedule.prototype.buildPlanificationRowView = function (rowIndex, tBodyRow) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    var pUnitData = oCalData.BudgetData[rowIndex].ProductiveUnitData.ProductiveUnit;
    var dayData = oCalData.BudgetData[rowIndex].PeriodData.DayData;

    for (var columnIndex = 0; columnIndex < dayData.length; columnIndex++) {
        tBodyRow.append(this.createCalendarCell(pUnitData, dayData[columnIndex], rowIndex, columnIndex));
    }
};

Robotics.Client.Controls.roBudgetCalendarSchedule.prototype.createCalendarCell = function (pUnitData, cellInfo, rowPosition, columnPosition) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    var calendarCell = $('<td></td>');

    var calendarOuterContent = $('<div></div>').attr('id', oCalendar.ascxPrefix + '_calCell_' + rowPosition + '_' + columnPosition);
    if (cellInfo.ProductiveUnitStatus == Robotics.Client.Constants.ProductiveUnitStatusOnDay.Ok) {
        calendarOuterContent.attr('class', 'calendarOuterBodyCell');
        this.buildCalendarCellContent(calendarOuterContent, cellInfo, rowPosition, columnPosition);
    } else if (Robotics.Client.Constants.ProductiveUnitStatusOnDay.NoPlanned) {
        calendarOuterContent.attr('class', 'calendarOuterBodyCell');
        //calendarOuterContent.attr('class', 'calendarOuterBodyCellNoContract');
    }

    calendarOuterContent.attr('data-IDPunit', pUnitData.ID);
    calendarOuterContent.attr('data-IDRow', rowPosition).attr('data-IDColumn', columnPosition).attr('data-Date', moment(cellInfo.PlanDate).format('DD/MM/YYYY'));

    return calendarCell.append(calendarOuterContent);
};

Robotics.Client.Controls.roBudgetCalendarSchedule.prototype.buildCalendarCellContent = function (calendarOuterContent, cellInfo, rowPosition, columnPosition) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    var calendarInnerContent = $('<div></div>').attr('class', 'calendarInnerBodyCell').html('&nbsp;');
    calendarInnerContent.attr('style', this.generateGradientFromColor(cellInfo));

    pUnitModeContainer = this.generateModeDetailCell(cellInfo, cellInfo.ProductiveUnitMode, rowPosition, columnPosition);

    if (pUnitModeContainer == null) {
        pUnitModeContainer = $('<div></div>').attr('class', 'calendarShiftInfoContainer');
        pUnitModeContainer.attr('style', 'color:' + new Robotics.Client.Common.roHtmlColor().invertCssColor("#FFFFFF"));
    }

    calendarInnerContent.append(pUnitModeContainer);

    calendarOuterContent.append(calendarInnerContent);
};

Robotics.Client.Controls.roBudgetCalendarSchedule.prototype.generateModeDetailCell = function (dayData, pUnitMode, rowPosition, columnPosition) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    var shiftInfoContainer = $('<div></div>').attr('class', 'calendarShiftInfoContainer pUnitModeWidth');

    var textColor = pUnitMode.HtmlColor;

    var titleLine = $('<div style="float:left;width:67px;"></div>');
    titleLine.append($("<span class='fontBold'></span>").html(pUnitMode.Name));

    var infoContainter = $('<div style="float:right;"></div>');

    if (pUnitMode.Coverage != 100) {
        if (pUnitMode.Coverage > 100) {
            var difHeight = pUnitMode.Coverage - 100;
            var coverageBar = $('<div></div>').attr({ class: 'overCoverageState' });
            var percentageBar = $('<div></div>').attr({ style: 'height:' + difHeight + '%;background-color:#1e571a;position: relative;top:' + (100 - difHeight) + '%;left: -1px;width: 21px;' })
            infoContainter.append(coverageBar.append(percentageBar));
        } else {
            var difHeight = 100 - pUnitMode.Coverage;
            var coverageBar = $('<div></div>').attr({ class: 'overCoverageState' });
            var percentageBar = $('<div></div>').attr({ style: 'max-height: 95%;height:' + difHeight + '%;background-color:#ff0000;position: relative;width: 11px;border: 1px solid rgba(0, 0, 0, 0);' })
            infoContainter.append(coverageBar.append(percentageBar));
        }
    } else {
        var coverageBar = $('<div></div>').attr({ class: 'coverageContainer' });
        infoContainter.append(coverageBar.append($('<div></div>').attr({ class: 'coverageOK' })));
    }

    if (pUnitMode.UnitModePositions.length > 0) {
        var dayIndictments = [];

        for (var iPos = 0; iPos < pUnitMode.UnitModePositions.length; iPos++) {
            var posAlerts = pUnitMode.UnitModePositions[iPos].Alerts;

            //if (typeof posAlerts != 'undefined' && typeof posAlerts.OnAbsenceDays != 'undefined') {
            //    onAbsence = posAlerts.OnAbsenceDays;
            //    if (onAbsence) break;
            //}

            for (var iEmp = 0; iEmp < pUnitMode.UnitModePositions[iPos].EmployeesData.length; iEmp++) {
                var cEmp = pUnitMode.UnitModePositions[iPos].EmployeesData[iEmp];

                if (cEmp.Alerts != null && cEmp.Alerts.OnAbsenceDays != null && cEmp.Alerts.OnAbsenceDays) {
                    dayIndictments.push({ employeeName: cEmp.EmployeeName, indictment: { ErrorText: Globalize.formatMessage('roEmployeeAbsent') } });
                }

                if (cEmp.Alerts != null && cEmp.Alerts.Indictments != null && cEmp.Alerts.Indictments.length > 0) {
                    for (iInd = 0; iInd < cEmp.Alerts.Indictments.length; iInd++) {
                        dayIndictments.push({ employeeName: cEmp.EmployeeName, indictment: Object.clone(cEmp.Alerts.Indictments[iInd], true) });
                    }
                }
            }
        }

        if (dayIndictments.length > 0) {
            var button = $('<div></div>').attr('id', oCalendar.ascxPrefix + '_tooltipIndictments_' + rowPosition + '_' + columnPosition).attr('style', 'float:left;margin-top:20px;').attr('class', 'budgetMainDetailOnAbsence');//.attr('title', Globalize.formatMessage('roEmployeeAbsent'));

            var indictmentTooltipContainer = $('<div></div>').attr('id', oCalendar.ascxPrefix + '_tooltipIndictmentsContainer_' + rowPosition + '_' + columnPosition).attr('class', 'tooltipIndictmentsContainer');
            var divIndictMentsTooltipTitle = $('<div></div>').html('<span class="fontBold fontBig" style="text-align:left">' + Globalize.formatMessage("roIndictmentsTitle") + '</span>');

            var ulInditmentsTooltipList = $('<ul></ul>');

            for (var iDayI = 0; iDayI < dayIndictments.length; iDayI++) {
                ulInditmentsTooltipList.append($('<li></li>').html(dayIndictments[iDayI].employeeName + ':' + dayIndictments[iDayI].indictment.ErrorText))
            }

            indictmentTooltipContainer.append(divIndictMentsTooltipTitle, ulInditmentsTooltipList);
            infoContainter.append(button, indictmentTooltipContainer);
        }
    }

    shiftInfoContainer.append(titleLine, infoContainter)

    var ulTooltipMessage = $('<ul></ul>');

    for (var i = 0; i < pUnitMode.UnitModePositions.length; i++) {
        var tmpOrdinaryHours = 0;
        var tmpComplementaryHours = 0;

        var calcShift = pUnitMode.UnitModePositions[i].ShiftData;

        if (calcShift.ExistComplementaryData) {
            for (var z = 0; z < calcShift.ShiftLayers; z++) {
                tmpOrdinaryHours += calcShift.ShiftLayersDefinition[z].LayerOrdinaryHours;
                tmpComplementaryHours += calcShift.ShiftLayersDefinition[z].LayerComplementaryHours;
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

        var shiftHoursLine = '';
        if (calcShift.Type != Robotics.Client.Constants.ShiftType.Holiday && calcShift.Type != Robotics.Client.Constants.ShiftType.Holiday_NonWorking && resultText != '') {
            var hours = resultText.split('-');

            for (var z = 0; z < hours.length; z++) {
                var hoursLine = '<span class="fontBig"> ' + (z === 0 ? 'HO:' : 'HC:') + hours[z] + ' </span>';
                shiftHoursLine += hoursLine;
            }
        }

        shiftDescription = '<span class="fontBold fontBig">' + pUnitMode.UnitModePositions[i].Quantity + " " + pUnitMode.UnitModePositions[i].AssignmentData.Name + ' - ' + pUnitMode.UnitModePositions[i].ShiftData.Name + '</span><span class="fontBig">[' + moment(calcShift.StartHour).format("HH:mm") + ']</span>';

        shiftDescription = shiftDescription + shiftHoursLine;

        ulTooltipMessage.append($('<li></li>').html(shiftDescription))
    }

    shiftInfoContainer.attr('style', 'color:' + new Robotics.Client.Common.roHtmlColor().invertCssColor(textColor));

    var divTooltip = $('<div></div>').attr('id', oCalendar.ascxPrefix + '_tooltipInfo_' + rowPosition + '_' + columnPosition).attr('class', 'tooltipDiv');
    var divTooltipTitle = $('<div></div>').html('<span class="fontBold fontBig">' + pUnitMode.Name + '</span>');
    shiftInfoContainer.append(divTooltip.append(divTooltipTitle, ulTooltipMessage));

    return shiftInfoContainer;
};

Robotics.Client.Controls.roBudgetCalendarSchedule.prototype.generateGradientFromColor = function (pUnitMode) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    var startColor = '#ffffff';
    var endColor = '#ffffff';

    var startChange = '100%';
    var changeToWhite = '100%';

    startColor = endColor = pUnitMode.ProductiveUnitMode.HtmlColor;

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

Robotics.Client.Controls.roBudgetCalendarSchedule.prototype.createColumnAssignmentsTable = function (idTable, parentId) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    if (oCalendar.clientMode.employeeAssignmentData != null) {
        var tableContainer = $('#' + oCalendar.prefix + parentId);
        tableContainer.empty();
        var tableElement = $('<table></table>').attr('id', oCalendar.prefix + idTable).attr('class', 'fancyColumnTable');
        //Necesitamos crear un header sin altura para poder utilizar el plugin de sticky headers y columns
        tableElement.append(this.createEmptyColumnTableHeader(idTable, parentId));
        tableElement.append(this.createColumnAssignmentsTableBody(idTable, parentId));
        tableElement.append(this.createColumnAssignmentsTableFooter(idTable, parentId));
        tableContainer.append(tableElement);
    } else {
        this.loadEmployeeDetail();
    }
};

Robotics.Client.Controls.roBudgetCalendarSchedule.prototype.createEmptyColumnTableHeader = function (idTable, parentId) {
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
        for (var i = 0; i < oCalData.BudgetHeader.PeriodHeaderData.length; i++) {
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

Robotics.Client.Controls.roBudgetCalendarSchedule.prototype.createColumnAssignmentsTableBody = function (idTable, parentId) {
    var oClientMode = this;
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    //Creamos el body
    var tBody = $('<tbody></tbody>').attr('id', oCalendar.prefix + idTable + '_tbody');
    var oTmpCal = oCalendar;

    var rowPosition = 0;

    Object.keys(oClientMode.employeeAssignmentData.assignmentKeys).forEach(function (assignmentKey, index) {
        var tBodyRow = $('<tr></tr>').attr('class', 'shiftColumnRow');
        var tFixedBodyCell = $('<td></td>');
        var mainFixedBodyCell = $('<div></div>').attr('class', 'CalendarShiftsFixed CalendarShiftFixedBody');
        mainFixedBodyCell.append($('<div></div>').attr('class', 'CalendarShiftContentFixedBody').html('<span style="font-weight:bold">' + oClientMode.employeeAssignmentData.assignmentKeys[assignmentKey].Name + '</span>'));
        tBodyRow.append(tFixedBodyCell.append(mainFixedBodyCell));

        var dayBodyCell = null;
        var columnContentCell = null;
        var columnPosition = 0;
        for (var i = 0; i < oCalData.BudgetHeader.PeriodHeaderData.length; i++) {
            dayBodyCell = $('<td></td>');
            columnContentCell = $('<div></div>').attr('class', 'ColumnShiftTableBodyCell');

            var outerStyle = '';
            var dayKey = moment(oCalData.BudgetHeader.PeriodHeaderData[i].Row2Text, "DD/MM/YYYY").format('YYYYMMDD');

            if (typeof oClientMode.employeeAssignmentData.tableData[dayKey] != 'undefined') {
                var cellContent = typeof oClientMode.employeeAssignmentData.tableData[dayKey].Assignments[assignmentKey] == 'undefined' ? '0' : oClientMode.employeeAssignmentData.tableData[dayKey].Assignments[assignmentKey].Counter;

                columnContentCell.append($('<div></div>').attr('id', oCalendar.ascxPrefix + '_EmployeeAvailableCell_' + rowPosition + '_' + columnPosition).attr('class', 'ContentShiftsColumnCell').attr('style', outerStyle).html(cellContent));

                var employeesAssignment = [];
                if (typeof oClientMode.employeeAssignmentData.tableData[dayKey].Assignments[assignmentKey] != 'undefined') employeesAssignment = oClientMode.employeeAssignmentData.tableData[dayKey].Assignments[assignmentKey].EmployeesData;

                var ulTooltipMessage = $('<ul></ul>');
                for (var iEmp = 0; iEmp < employeesAssignment.length; iEmp++) {
                    var empDesc = '<span class="fontBold fontBig">' + employeesAssignment[iEmp].EmployeeName + '</span>';

                    ulTooltipMessage.append($('<li></li>').html(empDesc))
                }

                var divTooltip = $('<div></div>').attr('id', oCalendar.ascxPrefix + '_EmployeeTooltipInfo_' + rowPosition + '_' + columnPosition).attr('class', 'employeeTooltipDiv');
                var divTooltipTitle = $('<div></div>').html('<span class="fontBold fontBig">' + Globalize.formatMessage('roNoAvailableEmployeeAssignments') + '</span>');

                if (employeesAssignment.length > 0) {
                    divTooltip.append(ulTooltipMessage);
                } else {
                    divTooltip.append(divTooltipTitle);
                }

                columnContentCell.append(divTooltip);
            }
            tBodyRow.append(dayBodyCell.append(columnContentCell));
            columnPosition += 1;
        }

        tmpCell = $('<td style="width:100%"></td>').html('&nbsp;');
        tBodyRow.append(tmpCell);
        tBody.append(tBodyRow);
        rowPosition += 1;
    });

    return tBody;
};

Robotics.Client.Controls.roBudgetCalendarSchedule.prototype.createColumnAssignmentsTableFooter = function (idTable, parentId) {
    var oClientMode = this;
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

    for (var i = 0; i < oCalData.BudgetHeader.PeriodHeaderData.length; i++) {
        cellID = oTmpCal.oCalendar.BudgetHeader.PeriodHeaderData[i].Row1Text + "_" + oTmpCal.oCalendar.BudgetHeader.PeriodHeaderData[i].Row2Text;
        cellID = cellID.replace(':', '_');
        dayBodyCell = $('<td></td>');
        columnContentCell = $('<div></div>').attr('class', 'ColumnShiftTableBodyCell totalRowBackground');

        var dayKey = moment(oTmpCal.oCalendar.BudgetHeader.PeriodHeaderData[i].Row2Text, "DD/MM/YYYY").format('YYYYMMDD');
        if (typeof oClientMode.employeeAssignmentData.tableData[dayKey] != 'undefined') {
            cellValue = oClientMode.employeeAssignmentData.tableData[dayKey].Total;
        } else {
            cellValue = '0';
        }

        columnContentCell.append($('<div></div>').attr('class', 'ContentShiftsColumnCell').html(Math.round(cellValue * 100) / 100));
        tBodyRow.append(dayBodyCell.append(columnContentCell));
    }

    tmpCell = $('<td style="width:100%"></td>').attr('class', 'totalRowBackground').html('&nbsp;');
    tBodyRow.append(tmpCell);
    tBody.append(tBodyRow);
    return tBody;
};

Robotics.Client.Controls.roBudgetCalendarSchedule.prototype.generateLoadFilters = function () {
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
        oParameters.orgChartFilter = oCalendar.employeeFilter;
    } else {
        //Por defecto no se selecciona ningún nodo del organigrama
        oParameters.orgChartFilter = "-1";
    }

    oParameters.loadIndictments = oCalendar.loadIndictments;
    oParameters.budget = null;
    oParameters.typeView = oCalendar.typeView;
    oParameters.pUnitFilter = oCalendar.assignmentsFilter;

    oParameters.StampParam = new Date().getMilliseconds();

    return oParameters;
};

Robotics.Client.Controls.roBudgetCalendarSchedule.prototype.alterLoadIndictmentsModeAndReload = function () {
    var oClientMode = this;
    var oCalendar = this.oBaseControl;

    if (oCalendar.loadIndictments == false) oCalendar.loadIndictments = true;
    else oCalendar.loadIndictments = false;

    oCalendar.setFiltersCookieValue();
    oClientMode.loadData();
};

Robotics.Client.Controls.roBudgetCalendarSchedule.prototype.loadData = function (firstDate, endDate, orgChartFilter, typeView, pUnitFilter, loadIndictments) {
    var oClientMode = this;
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    if (oCalendar.hasChanges == false) {
        if (oCalendar.refreshTimmer != -1) clearTimeout(oCalendar.refreshTimmer);
        oCalendar.refreshTimmer = -1;
        oCalendar.sortColumn = -1;

        if (typeof (loadIndictments) != 'undefined' && loadIndictments != null) oCalendar.loadIndictments = loadIndictments;
        if (typeof (orgChartFilter) != 'undefined' && orgChartFilter != null) oCalendar.employeeFilter = orgChartFilter;
        if (typeof (firstDate) != 'undefined' && firstDate != null) oCalendar.firstDate = firstDate;
        if (typeof (endDate) != 'undefined' && endDate != null) oCalendar.endDate = endDate;
        if (typeof (typeView) != 'undefined' && typeView != null) oCalendar.typeView = typeView;
        if (typeof (pUnitFilter) != 'undefined' && pUnitFilter != null) oCalendar.assignmentsFilter = pUnitFilter;

        oCalendar.refresh();
    } else {
        oCalendar.showChangesWarning("refreshBudget();");
    }
};

Robotics.Client.Controls.roBudgetCalendarSchedule.prototype.saveChanges = function () {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    var oParameters = {};
    oParameters.budget = oCalData;
    oParameters.StampParam = new Date().getMilliseconds();

    oCalendar.performAction(oParameters, Robotics.Client.Constants.Actions.SaveBudgetChanges);
};

Robotics.Client.Controls.roBudgetCalendarSchedule.prototype.runAIPlanner = function () {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    var oParameters = {};
    oParameters.budget = oCalData;
    oParameters.StampParam = new Date().getMilliseconds();

    oCalendar.performAction(oParameters, Robotics.Client.Constants.Actions.RunAIPlanner);
};

Robotics.Client.Controls.roBudgetCalendarSchedule.prototype.cleanAIPlanner = function () {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    var oParameters = {};
    oParameters.budget = oCalData;
    oParameters.StampParam = new Date().getMilliseconds();

    oCalendar.performAction(oParameters, Robotics.Client.Constants.Actions.CleanAIPlanner);
};

Robotics.Client.Controls.roBudgetCalendarSchedule.prototype.buildContextMenu = function (sender) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    var controlText = sender[0].innerText;

    var clickedRow = parseInt($(sender).attr('data-IDRow'), 10);
    var clickedColumn = parseInt($(sender).attr('data-IDColumn'), 10);

    var existingEmployees = {};
    if (typeof oCalendar.selectedDay.ProductiveUnitMode != 'undefined') {
        for (var iMode = 0; iMode < oCalendar.selectedDay.ProductiveUnitMode.UnitModePositions.length; iMode++) {
            var cPosition = oCalendar.selectedDay.ProductiveUnitMode.UnitModePositions[iMode];

            for (var iEmp = 0; iEmp < cPosition.EmployeesData.length; iEmp++) {
                if (cPosition.EmployeesData[iEmp].IDEmployee > 0) {
                    existingEmployees["delEmployee_" + cPosition.EmployeesData[iEmp].IDEmployee] = { name: cPosition.EmployeesData[iEmp].EmployeeName, disabled: false, icon: 'removeEmp' }
                }
            }
        }
    }

    items = {
        'enter': { name: oCalendar.translator.translate(Robotics.Client.Language.Tags.ContextMenu_Details), disabled: false, icon: 'detail' },
        'split1': { name: '---------', disabled: true },
        'copy': { name: oCalendar.translator.translate(Robotics.Client.Language.Tags.ContextMenu_Copy), disabled: false, icon: 'copyShift' },
        'advPaste': { name: oCalendar.translator.translate(Robotics.Client.Language.Tags.ContextMenu_AdvPaste), disabled: false, icon: 'advPaste' },
        'cancelSelection': { name: oCalendar.translator.translate(Robotics.Client.Language.Tags.ContextMenu_CancelSelection), disabled: false, icon: 'cancelSelection' },
        'split2': { name: '---------', disabled: true },
        'removeEmployee': {
            name: Globalize.formatMessage('roRemoveEmployee'),
            disabled: false,
            icon: 'removeEmpMenu',
            items: existingEmployees
        }
    };

    if (Object.keys(existingEmployees).length == 0) items['removeEmployee'].disabled = true;
    if (typeof oCalendar.selectedDay.ProductiveUnitMode == 'undefined') {
        items['enter'].disabled = true;
        items['copy'].disabled = true;
    }

    if (!oCalendar.selectionCopied) {
        items['advPaste'].disabled = true;
        items['cancelSelection'].disabled = true;
    }

    if ((clickedRow >= oCalendar.selectedMinRow && clickedRow <= oCalendar.selectedMaxRow) && (clickedColumn >= oCalendar.selectedMinColumn && clickedColumn <= oCalendar.selectedMaxColumn)) {
        items['enter'].disabled = true;
        items['advPaste'].disabled = true;
    } else {
        if (oCalendar.selectionCopied) {
            items['enter'].disabled = true;
            items['copy'].disabled = true;

            if (oCalendar.selectedMinRow != oCalendar.selectedMaxRow || clickedRow != oCalendar.selectedMinRow) items['advPaste'].disabled = true;
        }
    }

    return items;
};

Robotics.Client.Controls.roBudgetCalendarSchedule.prototype.executeContextMenuAction = function (key, container) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    switch (key) {
        case "enter":
            this.enterDetailAction(oCalendar.selectedEmployee.ProductiveUnitData.ProductiveUnit.ID, oCalendar.selectedEmployee.ProductiveUnitData.IDNode, moment(oCalendar.selectedDay.PlanDate).format('DD/MM/YYYY'));
            break;
        case "cancelSelection":
            this.cancelCurrentMultipleSelect(container, true);
            break;
        case "copy":
            this.copySelection(container);
            break;
        case "advPaste":
            this.advancedPastePrepare(container);
            break;
        default:
            if (key.indexOf('delEmployee_') == 0) {
                this.removeEmployeeFromMenu(key, container);
            }
            break;
    }
};

Robotics.Client.Controls.roBudgetCalendarSchedule.prototype.removeEmployeeFromMenu = function (key, container) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    var idEmployee = parseInt(key.replace('delEmployee_', ''), 10);

    var result = DevExpress.ui.dialog.confirm(Globalize.formatMessage("roUnassignEmployeeConfirmation"), Globalize.formatMessage("roDeleteTitle"));
    result.done(function (dialogResult) {
        if (dialogResult) {
            var cEmployee = null;
            var cPosition = null;
            for (var iMode = 0; iMode < oCalendar.selectedDay.ProductiveUnitMode.UnitModePositions.length; iMode++) {
                var tmpPosition = oCalendar.selectedDay.ProductiveUnitMode.UnitModePositions[iMode];

                for (var iEmp = 0; iEmp < tmpPosition.EmployeesData.length; iEmp++) {
                    if (tmpPosition.EmployeesData[iEmp].IDEmployee == idEmployee) {
                        cEmployee = tmpPosition.EmployeesData[iEmp];
                        cPosition = tmpPosition;
                    }
                }
            }

            var oParameters = {};
            oParameters.orgChartFilter = parseInt(oCalendar.employeeFilter, 10);
            oParameters.pUnitModePosition = cPosition;
            oParameters.firstDate = oCalendar.selectedDay.PlanDate;
            oParameters.employeeData = [cEmployee];
            oParameters.StampParam = new Date().getMilliseconds();

            oCalendar.performAction(oParameters, Robotics.Client.Constants.Actions.RemoveEmployeeFromPosition);
        }
    });
}

Robotics.Client.Controls.roBudgetCalendarSchedule.prototype.cancelCurrentMultipleSelect = function (sender, forceCancel) {
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
    }
}

Robotics.Client.Controls.roBudgetCalendarSchedule.prototype.cancelCurrentMultipleHeaderSelect = function (sender, forceCancel) {
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

Robotics.Client.Controls.roBudgetCalendarSchedule.prototype.advancedPastePrepare = function (container) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    if (oCalendar.hasChanges == false) {
        oCalendar.advCopyManager.prepareForm((oCalendar.selectedMaxColumn - oCalendar.selectedMinColumn + 1), oCalendar.selectedDay.PlanDate, Robotics.Client.Constants.TypeView.Planification);
        oCalendar.advCopyDialog.dialog("open");
    } else {
        oCalendar.showChangesWarning(oCal.clientInstanceName + ".advancedPastePrepare();");
    }
};

Robotics.Client.Controls.roBudgetCalendarSchedule.prototype.copyHeaderSelection = function (container) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    if ((oCalendar.selectedMinHeaderColumn == -1 || oCalendar.selectedMaxHeaderColumn == -1) && container != null) {
        var clickedColumn = parseInt($(container).attr('data-IDColumn'), 10);

        oCalendar.selectedMinHeaderColumn = oCalendar.selectedMaxHeaderColumn = clickedColumn;
        $(container).addClass("ui-selected");
    }
    oCalendar.selectionHeaderCopied = true;
};

Robotics.Client.Controls.roBudgetCalendarSchedule.prototype.copySelection = function (container) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    if ((oCalendar.selectedMinRow == -1 || oCalendar.selectedMaxRow == -1 || oCalendar.selectedMinColumn == -1 || oCalendar.selectedMaxColumn == -1) && container != null) {
        var clickedRow = parseInt($(container).attr('data-IDRow'), 10);
        var clickedColumn = parseInt($(container).attr('data-IDColumn'), 10);

        oCalendar.selectedMinRow = oCalendar.selectedMaxRow = clickedRow;
        oCalendar.selectedMinColumn = oCalendar.selectedMaxColumn = clickedColumn;

        $(container).addClass("ui-selected");
    }

    oCalendar.selectionCopied = true;
};

Robotics.Client.Controls.roBudgetCalendarSchedule.prototype.enterDetailAction = function (idPunit, idOrgCharNode, date) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    if (oCalendar.hasChanges == false) {
        this.enterDetailActionFinally(idPunit, idOrgCharNode, date);
    } else {
        oCalendar.showChangesWarning(oCalendar.clientInstanceName + ".clientMode.enterDetailActionFinally(" + idPunit + "," + idOrgCharNode + ",'" + date + "');");
    }
};

Robotics.Client.Controls.roBudgetCalendarSchedule.prototype.enterDetailActionFinally = function (idEmployee, date) {
    var Title = '';

    var oCalendar = this.oBaseControl;

    var oParameters = {};

    oParameters.firstDate = oCalendar.selectedDay.PlanDate;
    oParameters.endDate = oCalendar.selectedDay.PlanDate;

    oParameters.loadIndictments = oCalendar.loadIndictments;
    oParameters.orgChartFilter = oCalendar.employeeFilter;
    oParameters.pUnitFilter = oCalendar.selectedDay.ProductiveUnitMode.IDProductiveUnit;
    oParameters.StampParam = new Date().getMilliseconds();

    oCalendar.performAction(oParameters, Robotics.Client.Constants.Actions.GetBudgetHourPeriodDeinition);
};

Robotics.Client.Controls.roBudgetCalendarSchedule.prototype.showBudgetHourPeriodDetail = function (objResult, objResultParams) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    if (this.OnDayClick != null) this.OnDayClick(objResult, oCalendar.employeeFilter, oCalendar.selectedDay.PlanDate, oCalendar.selectedDay.ProductiveUnitMode, oCalendar.loadIndictments);
};

Robotics.Client.Controls.roBudgetCalendarSchedule.prototype.buildHeaderContextMenu = function (sender) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    var controlText = sender[0].innerText;
    var clickedColumn = parseInt($(sender).attr('data-IDColumn'), 10);

    items = {
        'sort': { name: oCalendar.translator.translate(Robotics.Client.Language.Tags.ContextMenu_Sort), disabled: false, icon: 'sort' }
    };

    return items;
};

Robotics.Client.Controls.roBudgetCalendarSchedule.prototype.executeHeaderContextMenuAction = function (key, container) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    switch (key) {
        case "sort":
            oCalendar.sortColumn = parseInt($(container).attr('data-IDColumn'), 10);
            //ISM: To define sort dialog
            oCalendar.calendarSortDialog.dialog('open');
            break;
        case "cancelSelection":
            this.cancelCurrentMultipleHeaderSelect(container, true);
            break;
    }
};

Robotics.Client.Controls.roBudgetCalendarSchedule.prototype.selectHeaderMultiple = function (origin, destination) {
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

Robotics.Client.Controls.roBudgetCalendarSchedule.prototype.selectMultiple = function (origin, destination) {
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

Robotics.Client.Controls.roBudgetCalendarSchedule.prototype.setSingleHeaderSelectedObejct = function (sender) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    if (sender != null) {
        this.setSingleSelectedObject(null);
    }

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

Robotics.Client.Controls.roBudgetCalendarSchedule.prototype.setSingleSelectedObject = function (sender) {
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

        if (this.OnSelectedCell != null) this.OnSelectedCell(null, null, null);
    } else {
        var clickedRow = parseInt($(sender).attr('data-IDRow'), 10);
        var clickedColumn = parseInt($(sender).attr('data-IDColumn'), 10);

        if (!((clickedRow >= oCalendar.selectedMinRow && clickedRow <= oCalendar.selectedMaxRow) && (clickedColumn >= oCalendar.selectedMinColumn && clickedColumn <= oCalendar.selectedMaxColumn))) {
            if (!oCalendar.selectionCopied) {
                this.cancelCurrentMultipleSelect(null, false);
                this.cancelCurrentMultipleHeaderSelect(null, false);
            }
        }

        oCalendar.selectedEmployee = oCalData.BudgetData[parseInt($(sender).attr('data-IDRow'), 10)];
        if (oCalendar.viewRange != Robotics.Client.Constants.ViewRange.Daily) {
            oCalendar.selectedDay = oCalData.BudgetData[parseInt($(sender).attr('data-IDRow'), 10)].PeriodData.DayData[parseInt($(sender).attr('data-IDColumn'), 10)];
        } else {
            oCalendar.selectedDay = oCalData.BudgetData[parseInt($(sender).attr('data-IDRow'), 10)].PeriodData.DayData[0];
        }

        oCalendar.selectedContainer = $(sender);
        oCalendar.selectedContainer.addClass('singleCellSelected');
    }

    if (this.OnSelectedCell != null) this.OnSelectedCell(oCalendar.selectedEmployee, oCalendar.selectedDay, oCalendar.selectedContainer);
};

Robotics.Client.Controls.roBudgetCalendarSchedule.prototype.endSelectOperation = function (sender) {
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

Robotics.Client.Controls.roBudgetCalendarSchedule.prototype.endHeaderSelectOperation = function (sender) {
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
};

//ISM: To define sort dialog
Robotics.Client.Controls.roBudgetCalendarSchedule.prototype.sortCalendar = function () {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    var sortParams = oCalendar.sortElements.split(',');

    oCalData.CalendarData = oCalData.CalendarData.sort(dynamicSortMultiple(sortParams, oCalendar.viewRange == Robotics.Client.Constants.ViewRange.Daily ? 0 : oCalendar.sortColumn));
    oCalendar.refreshTables(null, false, true);
    oCalendar.calendarSortDialog.dialog("close");
};

Robotics.Client.Controls.roBudgetCalendarSchedule.prototype.endCallback = function (action, objResult, objResultParams) {
    var oClientMode = this;
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    switch (action) {
        case Robotics.Client.Constants.Actions.LoadBudget:
        case Robotics.Client.Constants.Actions.DiscardBudgetAndContinue:
            this.loadDataResponse(objResult, objResultParams);
            break;
        case Robotics.Client.Constants.Actions.SaveBudgetChangesAndContinue:
        case Robotics.Client.Constants.Actions.SaveBudgetChanges:
            this.saveChangesResponse(objResult, objResultParams);
            break;
        case Robotics.Client.Constants.Actions.GetBudgetHourPeriodDeinition:
            this.showBudgetHourPeriodDetail(objResult, objResultParams);
            break;
        case Robotics.Client.Constants.Actions.RemoveEmployeeFromPosition:
            oClientMode.refreshFullData();
            break;
        case Robotics.Client.Constants.Actions.LoadAvailableEmployeesDetail:
            oClientMode.refreshAssignmentsTable(objResult, objResultParams);
            break;
        case Robotics.Client.Constants.Actions.RunAIPlanner:
        case Robotics.Client.Constants.Actions.CleanAIPlanner:
            oClientMode.refreshFullData();
            break;
    }

    if (action != Robotics.Client.Constants.Actions.RemoveEmployeeFromPosition) this.loadingFunctionExtended(false);

    if (action == Robotics.Client.Constants.Actions.DiscardBudgetAndContinue || action == Robotics.Client.Constants.Actions.SaveBudgetChangesAndContinue) {
        eval(oCalendar.onContinueFunc);
        oCalendar.onContinueFunc = '';
    }
};

Robotics.Client.Controls.roBudgetCalendarSchedule.prototype.loadEmployeeDetail = function () {
    var oClientMode = this;
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    var oParameters = oClientMode.generateLoadFilters();
    oClientMode.oBaseControl.sortColumn = -1;
    oClientMode.oBaseControl.performAction(oParameters, Robotics.Client.Constants.Actions.LoadAvailableEmployeesDetail);

    var tableContainer = $('#' + oCalendar.prefix + Robotics.Client.Constants.LayoutNames.ColumnTabAssignments);
    tableContainer.empty();

    var loadingDiv = $('<div></div>').attr('class', 'loadingLayoutZone')
    tableContainer.append(loadingDiv);

    oClientMode.loadingFunctionExtended(false);
};

Robotics.Client.Controls.roBudgetCalendarSchedule.prototype.refreshAssignmentsTable = function (objResult, objResultParams) {
    var oClientMode = this;
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    var tableData = {};
    var tmpAssignmentKeys = {};

    for (var i = 0; i < objResult.length; i++) {
        var iKey = moment(objResult[i].BudgetDate).format('YYYYMMDD');
        if (typeof tableData[iKey] == 'undefined') tableData[iKey] = { Assignments: {}, Total: 0 };

        for (var iEmp = 0; iEmp < objResult[i].BudgetEmployeeAvailableForNode.length; iEmp++) {
            var cEmp = objResult[i].BudgetEmployeeAvailableForNode[iEmp];
            if (typeof tmpAssignmentKeys[cEmp.IDAssignment] == 'undefined') tmpAssignmentKeys[cEmp.IDAssignment] = { IDAssignment: cEmp.IDAssignment, Name: cEmp.AssignmentName, Total: {} };
            if (typeof tableData[iKey].Assignments[cEmp.IDAssignment] == 'undefined') tableData[iKey].Assignments[cEmp.IDAssignment] = { Counter: 0, EmployeesData: [] };

            tableData[iKey].Total += 1;
            tableData[iKey].Assignments[cEmp.IDAssignment].Counter += 1;
            tableData[iKey].Assignments[cEmp.IDAssignment].EmployeesData.push(Object.clone(cEmp, true));
        }
    }

    this.employeeAssignmentData = {
        assignmentKeys: tmpAssignmentKeys,
        tableData: tableData,
        serverData: objResult
    }

    oClientMode.refreshColumnassignmentsTable();
}

Robotics.Client.Controls.roBudgetCalendarSchedule.prototype.refreshFullData = function () {
    this.loadData();
}

Robotics.Client.Controls.roBudgetCalendarSchedule.prototype.saveChangesResponse = function (objResult, objResultParams) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    this.employeeAssignmentData = null;
    oCalendar.setHasChanges(false);
    oCalendar.refreshTables(null, false, true);

    this.cancelCurrentMultipleSelect(null, true);
    this.cancelCurrentMultipleHeaderSelect(null, true);
};

Robotics.Client.Controls.roBudgetCalendarSchedule.prototype.loadDataResponse = function (objResult, objResultParams) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    this.employeeAssignmentData = null;
    oCalendar.viewRange = Robotics.Client.Constants.ViewRange.Period;
    oCalendar.initialize();
    oCalendar.oCalendar = objResult;
    oCalendar.setHasChanges(false);

    oCalendar.refreshTables(null, false, true);
};

Robotics.Client.Controls.roBudgetCalendarSchedule.prototype.onAcceptPUnitsDialog = function () {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;
};

Robotics.Client.Controls.roBudgetCalendarSchedule.prototype.preparePUnitsDialog = function (objResult) {
    var oCalendar = this.oBaseControl;

    oCalendar.pUnitsDialog.dialog("open");
};

Robotics.Client.Controls.roBudgetCalendarSchedule.prototype.saveAndContinue = function () {
    var oCalendar = this.oBaseControl;

    var oParameters = {};
    oParameters.budget = oCalendar.oCalendar;
    oParameters.StampParam = new Date().getMilliseconds();

    oCalendar.performAction(oParameters, Robotics.Client.Constants.Actions.SaveBudgetChangesAndContinue);
};

Robotics.Client.Controls.roBudgetCalendarSchedule.prototype.discardAndContinue = function (onAcceptFunc) {
    var oCalendar = this.oBaseControl;

    var oParameters = this.generateLoadFilters();

    oCalendar.performAction(oParameters, Robotics.Client.Constants.Actions.DiscardBudgetAndContinue);
};

Robotics.Client.Controls.roBudgetCalendarSchedule.prototype.refresh = function () {
    var oCalendar = this.oBaseControl;

    var oParameters = this.generateLoadFilters();
    oCalendar.sortColumn = -1;
    oCalendar.performAction(oParameters, Robotics.Client.Constants.Actions.LoadBudget);
};

Robotics.Client.Controls.roBudgetCalendarSchedule.prototype.getEmployeeCountResume = function () {
    var oCalendar = this.oBaseControl;

    return "";
};

Robotics.Client.Controls.roBudgetCalendarSchedule.prototype.onAcceptCopyDialog = function (keepLockedDays, KeepDestHolidayDays) {
};

Robotics.Client.Controls.roBudgetCalendarSchedule.prototype.changeFilterAssignments = function () {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    var assignmentsGrid = eval(oCalendar.ascxPrefix + '_dlgFilterCalendar_grdAssignmentsClient');
    var selectedValues = assignmentsGrid.GetSelectedKeysOnPage();

    var calendarFilter = "";

    for (var i = 0; i < selectedValues.length; i++) {
        calendarFilter += selectedValues[i] + ','
    }
    if (calendarFilter != '') calendarFilter = calendarFilter.substring(0, calendarFilter.length - 1);

    oCalendar.assignmentsFilter = calendarFilter;

    oCalendar.setFiltersCookieValue();
    oCalendar.refresh();
};