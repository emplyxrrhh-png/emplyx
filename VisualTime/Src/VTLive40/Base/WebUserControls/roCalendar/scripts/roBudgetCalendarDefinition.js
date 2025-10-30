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

Robotics.Client.Controls.roBudgetCalendarDefinition = function (baseControl) {
    this.name = "Robotics.Client.Controls.roBudgetCalendarDefinition";

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
        }
        , spacing_closed: 15
    };

    this.OnSelectedCell = null;
    this.OnDayClick = null;

    this.idUnitSelected = -1;
}

Robotics.Client.Controls.roBudgetCalendarDefinition.prototype.getLayout = function () {
    return this.pageLayoutOptions;
}

Robotics.Client.Controls.roBudgetCalendarDefinition.prototype.setHasChanges = function(bolHasChanges) {
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

Robotics.Client.Controls.roBudgetCalendarDefinition.prototype.create = function () {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    this.buildHTMLStructure();

    oCalendar.pageLayout = oCalendar.container.layout(this.pageLayoutOptions);
};

Robotics.Client.Controls.roBudgetCalendarDefinition.prototype.buildHTMLStructure = function () {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    var mainCenterLayout = $('<div></div>').attr('id', oCalendar.prefix + Robotics.Client.Constants.LayoutNames.Center).attr('class', 'ui-layout-center');

    //Panel para el calendario
    var calendarLayout = $('<div></div>').attr('id', oCalendar.prefix + Robotics.Client.Constants.LayoutNames.Calendar).attr('class', 'ui-layout-center');

    // Añadimos todo al panel central
    mainCenterLayout.append(calendarLayout);

    oCalendar.container.empty();
    oCalendar.container.append(mainCenterLayout);
};

Robotics.Client.Controls.roBudgetCalendarDefinition.prototype.onresize = function (oScheduleCalendar) {
    return function () {
        oScheduleCalendar.loadingFunctionExtended(true);
        oScheduleCalendar.oBaseControl.refreshTables(oScheduleCalendar, true, true);
        oScheduleCalendar.loadingFunctionExtended(false);
    };
};

Robotics.Client.Controls.roBudgetCalendarDefinition.prototype.loadingFunctionExtended = function (showLoading) {
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

Robotics.Client.Controls.roBudgetCalendarDefinition.prototype.getContextMenuSelector = function () {
    var oClientMode = this;
    var oCal = this.oBaseControl;

    var selector = oCal.viewRange == Robotics.Client.Constants.ViewRange.Daily ? '.calendarDailyBodyCell' : '.calendarOuterBodyCell';

    return selector;
}

Robotics.Client.Controls.roBudgetCalendarDefinition.prototype.getContextMenuHeaderSelector = function() {
    var oClientMode = this;
    var oCal = this.oBaseControl;

    var selector = '.CalendarDayFixedHeader';

    return selector;
}

Robotics.Client.Controls.roBudgetCalendarDefinition.prototype.processKeyUpEvent = function(e) {
    var oClientMode = this;

    oClientMode.cancelCurrentMultipleSelect(null, false);
    oClientMode.cancelCurrentMultipleHeaderSelect(null, false);
}

Robotics.Client.Controls.roBudgetCalendarDefinition.prototype.processKeyDownEvent = function(e) {
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

Robotics.Client.Controls.roBudgetCalendarDefinition.prototype.onDrop = function(event) {
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

Robotics.Client.Controls.roBudgetCalendarDefinition.prototype.mapModeEvents = function () {
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
        oCal.preparePUnitsDialog();
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
        oClientMode.enterDetailAction(parseInt($(this).attr('data-IDPunit'),10), parseInt(oCal.employeeFilter,10), $(this).attr('data-Date'));
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

    window.focus();
};

Robotics.Client.Controls.roBudgetCalendarDefinition.prototype.refreshModeTables = function (objectRef, isResizing, refreshCalendar) {
    var oScheduleCalendar = this;

    if (objectRef != null) {
        if (objectRef instanceof Robotics.Client.Controls.roCalendar) oScheduleCalendar = objectRef.clientMode;
        else oScheduleCalendar = objectRef;
    }

    if (refreshCalendar) {
        oScheduleCalendar.refreshCalendarTable(isResizing);
        this.oBaseControl.mapEvents()
    }
};

Robotics.Client.Controls.roBudgetCalendarDefinition.prototype.refreshCalendarTable = function (isResizing) {
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

Robotics.Client.Controls.roBudgetCalendarDefinition.prototype.createResumeTables = function (idTable, parentId) {
    switch (idTable) {
        case Robotics.Client.Constants.TableNames.Calendar:
            this.createCalendarTable(idTable, parentId);
            break;
    }
};

Robotics.Client.Controls.roBudgetCalendarDefinition.prototype.createCalendarTable = function (idTable, parentId) {
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

Robotics.Client.Controls.roBudgetCalendarDefinition.prototype.createCalendarTableHeader = function (idTable, parentId) {
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
    pUnitDescription.append($('<span></span>').attr('class','ProductiveUnitTextNorth').html(oCalData.BudgetHeader.ProductiveUnitHeaderData.Row1Text));

    var pUnitActions = $('<div"></div>').attr('class','BudgetActionsFixed');

    var northCell = $('<div></div>').attr('class', 'NorthCell NorthCellHeaderEmployee');
    var expandIcon = $('<div></div>').attr('class', 'RecursiveAction expandGroup').attr('title', oCalendar.loadIndictments ? oCalendar.translator.translate(Robotics.Client.Language.Tags.Scheduler_Recursive) : oCalendar.translator.translate(Robotics.Client.Language.Tags.Scheduler_NonRecursive));
    if (oCalendar.assignmentsFilter == '') northCell.append($('<div></div>').attr('class', 'EmpCellIconsNorth').append(expandIcon));
    else northCell.append($('<div></div>').attr('class', 'EmpCellIconsNorth').append('&nbsp;'));

    var southCell = $('<div></div>').attr('class', 'SouthCell');
    var filterIcon = $('<div></div>').attr('class', 'FilterAction ' + (oCalendar.assignmentsFilter == '' ? 'filterGroup' : 'filter-activeGroup')).attr('title', oCalendar.translator.translate(Robotics.Client.Language.Tags.Scheduler_Filter));
    southCell.append($('<div></div>').attr('class', 'EmpCellIconsSouth').append(filterIcon));

    pUnitActions.append(northCell, southCell);
    pUnitHeaderCell.append(pUnitDescription,pUnitActions)

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

Robotics.Client.Controls.roBudgetCalendarDefinition.prototype.createCalendarTableBody = function (idTable, parentId) {
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

            this.buildDefinitionRowView(i, tBodyRow);

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

Robotics.Client.Controls.roBudgetCalendarDefinition.prototype.buildDefinitionRowView = function (rowIndex, tBodyRow) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    var pUnitData = oCalData.BudgetData[rowIndex].ProductiveUnitData.ProductiveUnit;
    var dayData = oCalData.BudgetData[rowIndex].PeriodData.DayData;

    for (var columnIndex = 0; columnIndex < dayData.length; columnIndex++) {
        tBodyRow.append(this.createCalendarCell(pUnitData, dayData[columnIndex], rowIndex, columnIndex));
    }
};

Robotics.Client.Controls.roBudgetCalendarDefinition.prototype.createCalendarCell = function (pUnitData, cellInfo, rowPosition, columnPosition) {
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

Robotics.Client.Controls.roBudgetCalendarDefinition.prototype.buildCalendarCellContent = function (calendarOuterContent, cellInfo, rowPosition, columnPosition) {
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

Robotics.Client.Controls.roBudgetCalendarDefinition.prototype.generateModeDetailCell = function (dayData, pUnitMode, rowPosition, columnPosition) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    var shiftInfoContainer = $('<div></div>').attr('class', 'calendarShiftInfoContainer pUnitModeWidth');

    var textColor = pUnitMode.HtmlColor;

    var titleLine = $('<div></div>');

    var sName = titleLine.append($("<span class='fontBold'></span>").html(pUnitMode.Name));

    shiftInfoContainer.append(titleLine);

    var ulTooltipMessage = $('<ul></ul>');

    for (var i = 0; i < pUnitMode.UnitModePositions.length; i++) {
        var shiftInfo = pUnitMode.UnitModePositions[i].Quantity + " " + pUnitMode.UnitModePositions[i].AssignmentData.ShortName.toUpperCase() + " - " + pUnitMode.UnitModePositions[i].ShiftData.ShortName.toUpperCase() + ""

        if (i == 2 && pUnitMode.UnitModePositions.length > 3) {
            shiftInfo += '       ...'
        }

        if (i <= 2) shiftInfoContainer.append($('<div></div>').append($('<span></span>').attr('class', 'fontMedium').html(shiftInfo)));

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

Robotics.Client.Controls.roBudgetCalendarDefinition.prototype.generateGradientFromColor = function (pUnitMode) {
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

Robotics.Client.Controls.roBudgetCalendarDefinition.prototype.generateLoadFilters = function () {
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

    oParameters.loadIndictments = false;
    oParameters.budget = null;
    oParameters.typeView = oCalendar.typeView;
    oParameters.pUnitFilter = oCalendar.assignmentsFilter;

    oParameters.StampParam = new Date().getMilliseconds();

    return oParameters;
};

Robotics.Client.Controls.roBudgetCalendarDefinition.prototype.loadData = function (firstDate, endDate, orgChartFilter, typeView, pUnitFilter, loadIndictments) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    if (oCalendar.hasChanges == false) {
        if (oCalendar.refreshTimmer != -1) clearTimeout(oCalendar.refreshTimmer);
        oCalendar.refreshTimmer = -1;
        oCalendar.sortColumn = -1;

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

Robotics.Client.Controls.roBudgetCalendarDefinition.prototype.saveChanges = function () {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    var oParameters = {};
    oParameters.budget = oCalData;
    oParameters.StampParam = new Date().getMilliseconds();

    oCalendar.performAction(oParameters, Robotics.Client.Constants.Actions.SaveBudgetChanges);
};

Robotics.Client.Controls.roBudgetCalendarDefinition.prototype.buildContextMenu = function (sender) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    var controlText = sender[0].innerText;

    var clickedRow = parseInt($(sender).attr('data-IDRow'), 10);
    var clickedColumn = parseInt($(sender).attr('data-IDColumn'), 10);

    var availableModes = {};

    for (var iMode = 0; iMode < oCalendar.selectedEmployee.ProductiveUnitData.ProductiveUnit.UnitModes.length; iMode++) {
        availableModes["setMode" + iMode] = { name: oCalendar.selectedEmployee.ProductiveUnitData.ProductiveUnit.UnitModes[iMode].Name, disabled: false }
    }

    items = {
        'enter': { name: oCalendar.translator.translate(Robotics.Client.Language.Tags.ContextMenu_Details), disabled: false, icon: 'detail' },
        'split1': { name: '---------', disabled: true },
        'assignMode': {
            name: Globalize.formatMessage('roModesTitle'),
            disabled: false,
            icon: 'changeMode',
            items: availableModes
        },
        'reassign': { name: Globalize.formatMessage('roRefreshNode'), disabled: false, icon: 'refreshMode' },
        'split2': { name: '---------', disabled: true },
        'copy': { name: oCalendar.translator.translate(Robotics.Client.Language.Tags.ContextMenu_Copy), disabled: false, icon: 'copyShift' },
        'split3': { name: '---------', disabled: true },
        'paste': { name: oCalendar.translator.translate(Robotics.Client.Language.Tags.ContextMenu_Paste), disabled: false, icon: 'pasteShift' },
        'advPaste': { name: oCalendar.translator.translate(Robotics.Client.Language.Tags.ContextMenu_AdvPaste), disabled: false, icon: 'advPaste' },
        'cancelSelection': { name: oCalendar.translator.translate(Robotics.Client.Language.Tags.ContextMenu_CancelSelection), disabled: false, icon: 'cancelSelection' },
        'split3': { name: '---------', disabled: true },
        'removeUnit': { name: Globalize.formatMessage('roDeleteRowMode'), disabled: false, icon: 'removeUnit' }
    };

    if (!oCalendar.selectionCopied) {
        items['paste'].disabled = true;
        items['cancelSelection'].disabled = true;
        items['advPaste'].disabled = true;
    } else {
        items['assignMode'].disabled = true;
        items['reassign'].disabled = true;
    }

    if ((clickedRow >= oCalendar.selectedMinRow && clickedRow <= oCalendar.selectedMaxRow) && (clickedColumn >= oCalendar.selectedMinColumn && clickedColumn <= oCalendar.selectedMaxColumn)) {
        items['enter'].disabled = true;
        items['paste'].disabled = true;
        items['removeUnit'].disabled = true;
        items['advPaste'].disabled = true;
    } else {
        if (oCalendar.selectionCopied) {
            items['enter'].disabled = true;
            items['copy'].disabled = true;
            items['removeUnit'].disabled = true;

            if (oCalendar.selectedMinRow != oCalendar.selectedMaxRow || clickedRow != oCalendar.selectedMinRow) {
                items['advPaste'].disabled = true;
                items['paste'].disabled = true;
            }
        }
    }

    if (oCalendar.hasChanges) items['removeUnit'].disabled = true;
    return items;
};

Robotics.Client.Controls.roBudgetCalendarDefinition.prototype.executeContextMenuAction = function (key, container) {
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
        case "paste":
            this.pasteSelection(container);
            break;
        case 'removeUnit':
            this.removeSelectedProductiveUnit(container);
            break;
        case 'reassign':
            this.refreshSelectedCellMode(key, container);
            break;
        case "advPaste":
            this.advancedPastePrepare(container);
            break;
        default:
            if (key.indexOf('setMode') == 0) {
                this.assignContextMode(key, container);
            }
            break;
    }
};

Robotics.Client.Controls.roBudgetCalendarDefinition.prototype.advancedPastePrepare = function (container) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    if (oCalendar.hasChanges == false) {
        oCalendar.advCopyManager.prepareForm((oCalendar.selectedMaxColumn - oCalendar.selectedMinColumn + 1), oCalendar.selectedDay.PlanDate, Robotics.Client.Constants.TypeView.Definition);
        oCalendar.advCopyDialog.dialog("open");
    } else {
        oCalendar.showChangesWarning(oCalendar.clientInstanceName + ".clientMode.advancedPastePrepare();");
    }
};
Robotics.Client.Controls.roBudgetCalendarDefinition.prototype.cancelCurrentMultipleSelect = function (sender, forceCancel) {
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

Robotics.Client.Controls.roBudgetCalendarDefinition.prototype.cancelCurrentMultipleHeaderSelect = function (sender, forceCancel) {
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

Robotics.Client.Controls.roBudgetCalendarDefinition.prototype.assignContextMode = function (key, container) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    var modeIndex = parseInt(key.replace('setMode', ''), 10);

    var modePositions = oCalendar.selectedEmployee.ProductiveUnitData.ProductiveUnit.UnitModes[modeIndex];

    if (typeof modePositions != 'undefined') {
        var selMode = {
            CostValue: modePositions.CostValue,
            Coverage: modePositions.Coverage,
            HtmlColor: modePositions.HtmlColor,
            ID: modePositions.ID,
            IDProductiveUnit: modePositions.IDProductiveUnit,
            Name: modePositions.Name,
            ShortName: modePositions.ShortName,
            UnitModePositions: Object.clone(modePositions.UnitModePositions)
        }

        for (var i = 0; i < selMode.UnitModePositions.length; i++) {
            selMode.UnitModePositions[i].ID = -1;
        }

        this.assignUnitModeToDay(oCalendar.selectedContainer, oCalendar.selectedDay, selMode);
    } else {
        oCalendar.showErrorPopup("Error.Title", "error", "Calendar.Client.ModeNotAllowedForUnit", "", "Error.OK", "Error.OKDesc", oCalendar.defaultMessageAction);
    }
}

Robotics.Client.Controls.roBudgetCalendarDefinition.prototype.refreshSelectedCellMode = function (key, container) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    var modePositions = oCalendar.selectedEmployee.ProductiveUnitData.ProductiveUnit.UnitModes.find(function (mode) {
        return mode.ID == oCalendar.selectedDay.ProductiveUnitMode.ID;
    });

    if (typeof modePositions != 'undefined') {
        var selMode = {
            CostValue: modePositions.CostValue,
            Coverage: modePositions.Coverage,
            HtmlColor: modePositions.HtmlColor,
            ID: modePositions.ID,
            IDProductiveUnit: modePositions.IDProductiveUnit,
            Name: modePositions.Name,
            ShortName: modePositions.ShortName,
            UnitModePositions: Object.clone(modePositions.UnitModePositions)
        }

        for (var i = 0; i < selMode.UnitModePositions.length; i++) {
            selMode.UnitModePositions[i].ID = -1;
        }

        this.assignUnitModeToDay(oCalendar.selectedContainer, oCalendar.selectedDay, selMode);
    } else {
        oCalendar.showErrorPopup("Error.Title", "error", "Calendar.Client.ModeNotAllowedForUnit", "", "Error.OK", "Error.OKDesc", oCalendar.defaultMessageAction);
    }
}

Robotics.Client.Controls.roBudgetCalendarDefinition.prototype.pasteSelection = function (container) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    var clickedRow = parseInt($(container).attr('data-IDRow'), 10);
    var clickedColumn = parseInt($(container).attr('data-IDColumn'), 10);

    this.pasteSelectionEnd(clickedRow, clickedColumn);
};

Robotics.Client.Controls.roBudgetCalendarDefinition.prototype.pasteSelectionEnd = function (clickedRow, clickedColumn) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    if (oCalendar.selectedMaxRow == oCalendar.selectedMinRow && clickedRow == oCalendar.selectedMaxRow) {
        for (var i = 0; i <= (oCalendar.selectedMaxRow - oCalendar.selectedMinRow); i++) {
            for (var x = 0; x <= (oCalendar.selectedMaxColumn - oCalendar.selectedMinColumn); x++) {
                var originCellRow = oCalendar.selectedMinRow + i;
                var originCellColumn = oCalendar.selectedMinColumn + x;

                var destinationCellRow = clickedRow + i;
                var destinationCellColumn = clickedColumn + x;

                var dayData = null;

                if (oCalendar.viewRange != Robotics.Client.Constants.ViewRange.Daily) {
                    dayData = oCalData.BudgetData[originCellRow].PeriodData.DayData[originCellColumn];
                } else {
                    dayData = oCalData.BudgetData[originCellRow].PeriodData.DayData[0];
                }

                var pUnitModeToCopy = Object.clone(dayData.ProductiveUnitMode, true)

                for (var k = 0;k < pUnitModeToCopy.UnitModePositions.length; k++) {
                    pUnitModeToCopy.UnitModePositions[k].ID = -1;
                }

                var bAssign = true;

                var destinationDay = null;
                if (oCalendar.viewRange != Robotics.Client.Constants.ViewRange.Daily) {
                    if (destinationCellRow < oCalData.BudgetData.length && destinationCellColumn < oCalData.BudgetData[destinationCellRow].PeriodData.DayData.length) {
                        bAssign = true;
                        destinationDay = oCalData.BudgetData[destinationCellRow].PeriodData.DayData[destinationCellColumn];
                    } else {
                        bAssign = false;
                    }
                } else {
                    if (destinationCellRow < oCalData.BudgetData.length) {
                        bAssign = true;
                        destinationDay = oCalData.BudgetData[destinationCellRow].PeriodData.DayData[0];
                    } else {
                        bAssign = false;
                    }
                }

                if (bAssign) {
                    this.assignUnitModeToDay($('#' + oCalendar.ascxPrefix + '_calCell_' + destinationCellRow + '_' + destinationCellColumn), destinationDay, pUnitModeToCopy);
                }
            }
        }
    } else {
        if (!oCalendar.isBatchMode()) oCalendar.showErrorPopup("Error.Title", "error", "Calendar.Client.RowChangeNotAllowed", "", "Error.OK", "Error.OKDesc", oCalendar.defaultMessageAction);
    }
};

Robotics.Client.Controls.roBudgetCalendarDefinition.prototype.copyHeaderSelection = function (container) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    if ((oCalendar.selectedMinHeaderColumn == -1 || oCalendar.selectedMaxHeaderColumn == -1) && container != null) {
        var clickedColumn = parseInt($(container).attr('data-IDColumn'), 10);

        oCalendar.selectedMinHeaderColumn = oCalendar.selectedMaxHeaderColumn = clickedColumn;
        $(container).addClass("ui-selected");
    }
    oCalendar.selectionHeaderCopied = true;
};

Robotics.Client.Controls.roBudgetCalendarDefinition.prototype.copySelection = function (container) {
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

Robotics.Client.Controls.roBudgetCalendarDefinition.prototype.removeSelectedProductiveUnitFinally = function (idPunit) {
    var oClientMode = this;
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    var result = DevExpress.ui.dialog.confirm(Globalize.formatMessage("roDeleteUnitConfirm"), Globalize.formatMessage("roDeleteTitle"));
    result.done(function (dialogResult) {
        if (dialogResult) {
            oClientMode.idUnitSelected = idPunit;
            showCaptcha(true);
        }
    });
}

Robotics.Client.Controls.roBudgetCalendarDefinition.prototype.finallyRemoveUnit = function () {
    var oClientMode = this;
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    var oParameters = {};

    oParameters.firstDate = oCalendar.firstDate;
    oParameters.endDate = oCalendar.endDate;

    oParameters.orgChartFilter = oCalendar.employeeFilter;
    oParameters.pUnitFilter = oClientMode.idUnitSelected;
    oParameters.StampParam = new Date().getMilliseconds();

    oCalendar.performAction(oParameters, Robotics.Client.Constants.Actions.DeleteBudgetRow);
}

Robotics.Client.Controls.roBudgetCalendarDefinition.prototype.removeSelectedProductiveUnit = function (container) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    if (oCalendar.hasChanges == false) {
        this.removeSelectedProductiveUnitFinally(oCalendar.selectedEmployee.ProductiveUnitData.ProductiveUnit.ID);
    } else {
        oCalendar.showChangesWarning(oCalendar.clientInstanceName + ".clientMode.removeSelectedProductiveUnitFinally(" + oCalendar.selectedEmployee.ProductiveUnitData.ProductiveUnit.ID + ");");
    }
}

Robotics.Client.Controls.roBudgetCalendarDefinition.prototype.enterDetailAction = function (idPunit, idOrgCharNode, date) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    if (oCalendar.hasChanges == false) {
        this.enterDetailActionFinally(idPunit, idOrgCharNode, date);
    } else {
        oCalendar.showChangesWarning(oCalendar.clientInstanceName + ".clientMode.enterDetailActionFinally(" + idPunit + "," + idOrgCharNode + ",'" + date + "');");
    }
};

Robotics.Client.Controls.roBudgetCalendarDefinition.prototype.enterDetailActionFinally = function (idEmployee, date) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    var oParameters = {};

    oParameters.firstDate = oCalendar.selectedDay.PlanDate;
    oParameters.endDate = oCalendar.selectedDay.PlanDate;

    oParameters.loadIndictments = false;
    oParameters.orgChartFilter = oCalendar.employeeFilter;
    oParameters.pUnitFilter = oCalendar.selectedDay.ProductiveUnitMode.IDProductiveUnit;
    oParameters.StampParam = new Date().getMilliseconds();

    oCalendar.performAction(oParameters, Robotics.Client.Constants.Actions.GetBudgetHourPeriodDeinition);
};

Robotics.Client.Controls.roBudgetCalendarDefinition.prototype.buildHeaderContextMenu = function (sender) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    var controlText = sender[0].innerText;
    var clickedColumn = parseInt($(sender).attr('data-IDColumn'), 10);

    items = {
        'sort': { name: oCalendar.translator.translate(Robotics.Client.Language.Tags.ContextMenu_Sort), disabled: false, icon: 'sort' }
    };

    return items;
};

Robotics.Client.Controls.roBudgetCalendarDefinition.prototype.executeHeaderContextMenuAction = function (key, container) {
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

Robotics.Client.Controls.roBudgetCalendarDefinition.prototype.selectHeaderMultiple = function (origin, destination) {
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

Robotics.Client.Controls.roBudgetCalendarDefinition.prototype.selectMultiple = function (origin, destination) {
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

Robotics.Client.Controls.roBudgetCalendarDefinition.prototype.setSingleHeaderSelectedObejct = function (sender) {
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

Robotics.Client.Controls.roBudgetCalendarDefinition.prototype.setSingleSelectedObject = function (sender) {
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

Robotics.Client.Controls.roBudgetCalendarDefinition.prototype.endSelectOperation = function(sender) {
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

Robotics.Client.Controls.roBudgetCalendarDefinition.prototype.endHeaderSelectOperation = function(sender) {
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

 //ISM: To define sort dialog
Robotics.Client.Controls.roBudgetCalendarDefinition.prototype.sortCalendar = function() {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    var sortParams = oCalendar.sortElements.split(',');

    oCalData.CalendarData = oCalData.CalendarData.sort(dynamicSortMultiple(sortParams, oCalendar.viewRange == Robotics.Client.Constants.ViewRange.Daily ? 0 : oCalendar.sortColumn));
    oCalendar.refreshTables(null, false, true);
    oCalendar.calendarSortDialog.dialog("close");
};

Robotics.Client.Controls.roBudgetCalendarDefinition.prototype.endCallback = function(action, objResult, objResultParams) {
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
        case Robotics.Client.Constants.Actions.GetNewBudgetRow:
            this.addBudgetRowToBudget(objResult, objResultParams);
            break;
        case Robotics.Client.Constants.Actions.DeleteBudgetRow:
            this.refresh();
            break;
        case Robotics.Client.Constants.Actions.GetBudgetHourPeriodDeinition:
            this.showBudgetHourPeriodDetail(objResult, objResultParams);
            break;
    }

    if (action != Robotics.Client.Constants.Actions.DeleteBudgetRow) this.loadingFunctionExtended(false);

    if (action == Robotics.Client.Constants.Actions.DiscardBudgetAndContinue || action == Robotics.Client.Constants.Actions.SaveBudgetChangesAndContinue) {
        eval(oCalendar.onContinueFunc);
        oCalendar.onContinueFunc = '';
    }
};

Robotics.Client.Controls.roBudgetCalendarDefinition.prototype.saveChangesResponse = function(objResult, objResultParams) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    oCalendar.setHasChanges(false);
    oCalendar.refreshTables(null, false, true);

    //this.cancelCurrentMultipleSelect(null, true);
    //this.cancelCurrentMultipleHeaderSelect(null, true);
};

Robotics.Client.Controls.roBudgetCalendarDefinition.prototype.loadDataResponse = function(objResult, objResultParams) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    oCalendar.viewRange = Robotics.Client.Constants.ViewRange.Period;
    oCalendar.initialize();
    oCalendar.oCalendar = objResult;
    oCalendar.setHasChanges(false);

    oCalendar.refreshTables(null, false, true);
};

Robotics.Client.Controls.roBudgetCalendarDefinition.prototype.addBudgetRowToBudget = function (objResult, objResultParams) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    oCalData.BudgetData.push(objResult.BudgetData[0]);
    oCalendar.setHasChanges(true);

    oCalendar.refreshTables(null, false, true);
    oCalendar.pUnitsDialog.dialog("close");
};

Robotics.Client.Controls.roBudgetCalendarDefinition.prototype.showBudgetHourPeriodDetail = function (objResult, objResultParams) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    if (this.OnDayClick != null) this.OnDayClick(objResult, oCalendar.employeeFilter, oCalendar.selectedDay.PlanDate, oCalendar.selectedDay.ProductiveUnitMode, false);
};

Robotics.Client.Controls.roBudgetCalendarDefinition.prototype.onAcceptPUnitsDialog = function () {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    var oUnitID = parseInt(oCalendar.pUnitSelectorManager.getSelectedItem(), 10);

    var bFound = false;

    for (var i = 0; i < oCalData.BudgetData.length; i++) {
        if (oCalData.BudgetData[i].ProductiveUnitData.ProductiveUnit.ID == oUnitID) {
            bFound = true;
            break;
        }
    }

    if (parseInt(oCalendar.employeeFilter,10) > 0 && !bFound) {
        this.onAddNewRow();
    } else {
        if (!bFound) oCalendar.showErrorPopup("Error.Title", "error", "Calendar.Client.ProductiveUnitAlreadyExists", "", "Error.OK", "Error.OKDesc", oCalendar.defaultMessageAction);
        else oCalendar.showErrorPopup("Error.Title", "error", "Calendar.Client.SelectOrgChartNode", "", "Error.OK", "Error.OKDesc", oCalendar.defaultMessageAction);
    }
};

Robotics.Client.Controls.roBudgetCalendarDefinition.prototype.changeFilterAssignments = function () {
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
}

Robotics.Client.Controls.roBudgetCalendarDefinition.prototype.preparePUnitsDialog = function (objResult) {
    var oCalendar = this.oBaseControl;

    oCalendar.pUnitsDialog.dialog("open");
};

Robotics.Client.Controls.roBudgetCalendarDefinition.prototype.onAddNewRow = function () {
    var oCalendar = this.oBaseControl;

    var oParameters = {};

    oParameters.firstDate = oCalendar.firstDate;
    oParameters.endDate = oCalendar.endDate;

    oParameters.orgChartFilter = oCalendar.employeeFilter;
    oParameters.pUnitFilter = oCalendar.pUnitSelectorManager.getSelectedItem();
    oParameters.StampParam = new Date().getMilliseconds();

    oCalendar.performAction(oParameters, Robotics.Client.Constants.Actions.GetNewBudgetRow);
};

Robotics.Client.Controls.roBudgetCalendarDefinition.prototype.saveAndContinue = function() {
    var oCalendar = this.oBaseControl;

    var oParameters = {};
    oParameters.budget = oCalendar.oCalendar;
    oParameters.StampParam = new Date().getMilliseconds();

    oCalendar.performAction(oParameters, Robotics.Client.Constants.Actions.SaveBudgetChangesAndContinue);
};

Robotics.Client.Controls.roBudgetCalendarDefinition.prototype.discardAndContinue = function(onAcceptFunc) {
    var oCalendar = this.oBaseControl;

    var oParameters = this.generateLoadFilters();

    oCalendar.performAction(oParameters, Robotics.Client.Constants.Actions.DiscardBudgetAndContinue);
};

Robotics.Client.Controls.roBudgetCalendarDefinition.prototype.refresh = function() {
    var oCalendar = this.oBaseControl;

    var oParameters = this.generateLoadFilters();
    oCalendar.sortColumn = -1;
    oCalendar.performAction(oParameters, Robotics.Client.Constants.Actions.LoadBudget);
};

Robotics.Client.Controls.roBudgetCalendarDefinition.prototype.getEmployeeCountResume = function() {
    var oCalendar = this.oBaseControl;

    return "";
};

Robotics.Client.Controls.roBudgetCalendarDefinition.prototype.assignUnitModeToDayBatch = function (oUnitMode) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    for (var i = oCalendar.selectedMinRow; i <= oCalendar.selectedMaxRow; i++) {
        for (var x = oCalendar.selectedMinColumn; x <= oCalendar.selectedMaxColumn; x++) {
            var destinationCellRow = i;
            var destinationCellColumn = x;

            var bAssign = true;

            var destinationDay = null;
            if (oCalendar.viewRange != Robotics.Client.Constants.ViewRange.Daily) {
                if (destinationCellRow < oCalData.BudgetData.length && destinationCellColumn < oCalData.BudgetData[destinationCellRow].PeriodData.DayData.length) {
                    bAssign = true;
                    destinationDay = oCalData.BudgetData[destinationCellRow].PeriodData.DayData[destinationCellColumn];
                } else {
                    bAssign = false;
                }
            } else {
                if (destinationCellRow < oCalData.BudgetData.length) {
                    bAssign = true;
                    destinationDay = oCalData.BudgetData[destinationCellRow].PeriodData.DayData[0];
                } else {
                    bAssign = false;
                }
            }

            if (bAssign) {
                this.assignUnitModeToDay($('#' + oCalendar.ascxPrefix + '_calCell_' + destinationCellRow + '_' + destinationCellColumn), destinationDay, oUnitMode);
            }
        }
    }
};

Robotics.Client.Controls.roBudgetCalendarDefinition.prototype.assignUnitModeToDay = function (container, destinationDay, oUnitMode) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    if (destinationDay.ProductiveUnitStatus == Robotics.Client.Constants.ProductiveUnitStatusOnDay.Ok ||
        destinationDay.ProductiveUnitStatus == Robotics.Client.Constants.ProductiveUnitStatusOnDay.NoPlanned) {
        var bAssign = true;
        var clickedRow = parseInt($(container).attr('data-IDRow'), 10);
        var clickedColumn = parseInt($(container).attr('data-IDColumn'), 10);

        var originPUnit = parseInt($(container).attr('data-IDPunit'), 10);

        if (!destinationDay.CanBeModified) {
            if (!oCalendar.isBatchMode()) oCalendar.showErrorPopup("Error.Title", "error", "Calendar.Client.NoPermissionOrEmployeesAssigned", "", "Error.OK", "Error.OKDesc", oCalendar.defaultMessageAction);
            bAssign = false;
        } else if (moment(destinationDay.PlanDate) <= moment(oCalData.FreezingDate)) {
            if (!oCalendar.isBatchMode()) oCalendar.showErrorPopup("Error.Title", "error", "Calendar.Client.FreezeDate", "", "Error.OK", "Error.OKDesc", oCalendar.defaultMessageAction);
            bAssign = false;
        } else if (moment(destinationDay.PlanDate) <= moment(oCalData.FreezingDate)) {
            if (!oCalendar.isBatchMode()) oCalendar.showErrorPopup("Error.Title", "error", "Calendar.Client.FreezeDate", "", "Error.OK", "Error.OKDesc", oCalendar.defaultMessageAction);
            bAssign = false;
        } else if (originPUnit != oUnitMode.IDProductiveUnit) {
            if (!oCalendar.isBatchMode()) oCalendar.showErrorPopup("Error.Title", "error", "Calendar.Client.IncorrectProductiveUnit", "", "Error.OK", "Error.OKDesc", oCalendar.defaultMessageAction);
            bAssign = false;
        }

        if (bAssign) {
            destinationDay.ProductiveUnitMode = Object.clone(oUnitMode, true);
            destinationDay.HasChanged = true;
            destinationDay.ProductiveUnitStatus = Robotics.Client.Constants.ProductiveUnitStatusOnDay.Ok;
            oCalendar.setHasChanges(true);

            if (oCalData.BudgetData[parseInt(container.attr('data-idrow'), 10)].RowState != Robotics.Client.Constants.BudgetRowState.New) {
                oCalData.BudgetData[parseInt(container.attr('data-idrow'), 10)].RowState = Robotics.Client.Constants.BudgetRowState.Updated;
            }

            container.empty();
            this.buildCalendarCellContent(container, destinationDay, clickedRow, clickedColumn);

            var cellId = oCalendar.ascxPrefix + '_tooltipInfo_' + clickedRow + '_' + clickedColumn;
            var containerId = cellId.replace('_tooltipInfo_', '_calCell_');
            $("#" + cellId).dxTooltip({
                target: "#" + containerId,
                showEvent: { name: "mouseenter", delay: 300},
                hideEvent: "mouseleave",
                position: 'top'
            });
        }
    }
};