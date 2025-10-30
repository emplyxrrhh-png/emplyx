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

Robotics.Client.Controls.roBudgetCalendarDetail = function (baseControl) {
    this.name = "Robotics.Client.Controls.roBudgetCalendarDetail";

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
    this.BudgetData = null;

    this.selectedPunitId = -1;
    this.pUnitsData = null;

    this.selectedPUnit = null;
    this.selectedPUnitMode = null;
    this.selectedPUnitModePosition = null;
    this.selectedEmployee = null;
    this.selectedDate = null;
    this.scrollInitPosition = null;
}

Robotics.Client.Controls.roBudgetCalendarDetail.prototype.getLayout = function () {
    return this.pageLayoutOptions;
}

Robotics.Client.Controls.roBudgetCalendarDetail.prototype.setHasChanges = function (bolHasChanges) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    oCalendar.hasChanges = bolHasChanges;

    try {
        hasChanges(bolHasChanges);
    } catch (e) { }
};

Robotics.Client.Controls.roBudgetCalendarDetail.prototype.create = function () {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    this.buildHTMLStructure();

    oCalendar.pageLayout = oCalendar.container.layout(this.pageLayoutOptions);
};

Robotics.Client.Controls.roBudgetCalendarDetail.prototype.buildHTMLStructure = function () {
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

Robotics.Client.Controls.roBudgetCalendarDetail.prototype.onresize = function (oScheduleCalendar) {
    return function () {
        oScheduleCalendar.loadingFunctionExtended(true);
        oScheduleCalendar.oBaseControl.refreshTables(oScheduleCalendar, true, true);
        oScheduleCalendar.loadingFunctionExtended(false);
    };
};

Robotics.Client.Controls.roBudgetCalendarDetail.prototype.loadingFunctionExtended = function (showLoading) {
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

Robotics.Client.Controls.roBudgetCalendarDetail.prototype.getContextMenuSelector = function () {
    var oClientMode = this;
    var oCal = this.oBaseControl;

    var selector = oCal.viewRange == Robotics.Client.Constants.ViewRange.Daily ? '.calendarDailyBodyCell' : '.calendarOuterBodyCell';

    return selector;
}

Robotics.Client.Controls.roBudgetCalendarDetail.prototype.getContextMenuHeaderSelector = function () {
    var oClientMode = this;
    var oCal = this.oBaseControl;

    var selector = '.CalendarDayFixedHeader';

    return selector;
}

Robotics.Client.Controls.roBudgetCalendarDetail.prototype.processKeyUpEvent = function (e) {
    var oClientMode = this;

    oClientMode.cancelCurrentMultipleSelect(null, false);
    oClientMode.cancelCurrentMultipleHeaderSelect(null, false);
}

Robotics.Client.Controls.roBudgetCalendarDetail.prototype.processKeyDownEvent = function (e) {
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

Robotics.Client.Controls.roBudgetCalendarDetail.prototype.onDrop = function (event) {
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
    }
};

Robotics.Client.Controls.roBudgetCalendarDetail.prototype.mapModeEvents = function () {
    var oClientMode = this;
    var oCal = this.oBaseControl;

    var selector = oCal.viewRange == Robotics.Client.Constants.ViewRange.Daily ? '.calendarDailyBodyCell' : '.calendarOuterBodyCell';

    $(selector + ',.reviewDailyBodyCell').off('click');
    $(selector + ',.reviewDailyBodyCell').on('click', function (e) {
        if (!oCal.shiftDown) oClientMode.setSingleSelectedObject(this);
        else oClientMode.selectMultiple(oCal.selectedContainer, this);
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

    if (oCal.viewRange == Robotics.Client.Constants.ViewRange.Period && oCal.isScheduleActive) {
        try { $("#" + oCal.prefix + Robotics.Client.Constants.TableNames.Calendar + '_thead').selectableScroll('destroy'); } catch (e) { }
        $("#" + oCal.prefix + Robotics.Client.Constants.TableNames.Calendar + '_thead').selectableScroll({
            distance: 10,
            filter: '.CalendarDayFixedHeader',
            stop: function (event, ui) { oClientMode.endHeaderSelectOperation(); },
            scrollElement: $('#' + oCal.prefix + Robotics.Client.Constants.LayoutNames.Calendar + ' .fht-fixed-body .fht-thead')
        });
    }

    $('.LoadIndictments').off('click');
    $('.LoadIndictments').on('click', function (e) {
        oClientMode.alterLoadIndictmentsModeAndReload();
    });

    $('.CalendarDayFixedHeader').off('click');
    $('.CalendarDayFixedHeader').on('click', function (e) {
        if (!oCal.shiftDown) oClientMode.setSingleHeaderSelectedObejct(this);
        else oClientMode.selectHeaderMultiple(oCal.selectedHeaderContainer, this);
    });

    $('#' + oCal.prefix + '_pUnitSelect').off('change');
    $('#' + oCal.prefix + '_pUnitSelect').on('change', function (e) {
        oClientMode.selectedPunitId = this.value;
        oClientMode.refresh();
    });

    $('.assignEmployeeDetail').off('click');
    $('.assignEmployeeDetail').on('click', function (event) {
        var srcElement = null;
        if (typeof (event.srcElement) != 'undefined') {
            srcElement = $(event.srcElement);
        } else {
            srcElement = $(event.originalEvent.target);
        }

        oClientMode.onAddEmployee(srcElement);
    });

    $('.deleteAssignedEmployeeDetail').off('click');
    $('.deleteAssignedEmployeeDetail').on('click', function (event) {
        var srcElement = null;
        if (typeof (event.srcElement) != 'undefined') {
            srcElement = $(event.srcElement);
        } else {
            srcElement = $(event.originalEvent.target);
        }

        oClientMode.onRemoveEmployee(srcElement);
    });

    if (oClientMode.selectedPunitId != -1) $('#' + oCal.prefix + '_pUnitSelect').val(oClientMode.selectedPunitId);

    if (oClientMode.scrollInitPosition != null) {
        $('#' + oCal.prefix + 'Main div.fht-fixed-body div.fht-tbody').scrollTop(oClientMode.scrollInitPosition.top);
        $('#' + oCal.prefix + 'Main div.fht-fixed-body div.fht-tbody').scrollLeft(oClientMode.scrollInitPosition.left);
        oClientMode.scrollInitPosition = null;
    }

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

    //window.parent.frames['ifPrincipal'].window.focus();
    window.focus();
};

Robotics.Client.Controls.roBudgetCalendarDetail.prototype.refreshModeTables = function (objectRef, isResizing, refreshCalendar) {
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

Robotics.Client.Controls.roBudgetCalendarDetail.prototype.refreshCalendarTable = function (isResizing) {
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

Robotics.Client.Controls.roBudgetCalendarDetail.prototype.createResumeTables = function (idTable, parentId) {
    switch (idTable) {
        case Robotics.Client.Constants.TableNames.Calendar:
            this.createCalendarTable(idTable, parentId);
            break;
    }
};

Robotics.Client.Controls.roBudgetCalendarDetail.prototype.createCalendarTable = function (idTable, parentId) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    var tableContainer = $('#' + oCalendar.prefix + parentId);
    tableContainer.empty();

    var tableElement = $('<table></table>').attr('id', oCalendar.prefix + idTable).attr('class', 'fancyTable');

    tableElement.append(this.createCalendarTableHeader(idTable, parentId));

    tableElement.append(this.createCalendarTableBody(idTable, parentId));

    tableContainer.append(tableElement);
};

Robotics.Client.Controls.roBudgetCalendarDetail.prototype.createCalendarTableHeader = function (idTable, parentId) {
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
    pUnitDescription.append($('<span></span>').attr('class', 'ProductiveUnitTextNorth').html(this.BudgetData.BudgetHeader.ProductiveUnitHeaderData.Row1Text));

    var pUnitActions = $('<div"></div>').attr('class', 'BudgetActionsFixed');

    var northCell = $('<div></div>').attr('class', 'NorthCell NorthCellHeaderEmployee');
    var expandIcon = $('<div></div>').attr('class', 'LoadIndictments ' + (oCalendar.loadIndictments == false ? 'showIndictments-inactive' : 'showIndictments-active')).attr('title', Globalize.formatMessage("roShowIndictments"));
    northCell.append($('<div></div>').attr('class', 'EmpCellIconsNorth').append(expandIcon));

    var southCell = $('<div></div>').attr('class', 'SouthCell');
    //var filterIcon = $('<div></div>').attr('class', 'FilterAction ' + (oCalendar.assignmentsFilter == '' ? 'filterGroup' : 'filter-activeGroup')).attr('title', oCalendar.translator.translate(Robotics.Client.Language.Tags.Scheduler_Filter));
    //southCell.append($('<div></div>').attr('class', 'EmpCellIconsSouth').append(oCalendar.isScheduleActive ? filterIcon : '&nbsp;'));

    pUnitActions.append(northCell, southCell);
    pUnitHeaderCell.append(pUnitDescription, pUnitActions)

    tHeaderRow.append(tFixedHeaderCell.append(pUnitHeaderCell));

    Object.keys(oCalData.calendarData).forEach(function (dayKey, index) {
        var day = oCalData.calendarData[dayKey];

        var bNewDay = true;

        for (var i = oCalData.columnLimits[dayKey].min; i < oCalData.columnLimits[dayKey].max; i++) {
            var dayHeaderCell = $('<th></th>');

            var mainDayHeaderCell = $('<div></div>').attr('id', oCalendar.ascxPrefix + '_calHeaderCell_' + i).attr('class', 'CalendarDayFixed CalendarDayFixedHeader DailyCell').attr('data-IDColumn', i).attr('style', 'background:' + day.header[i].BackColor);
            if (bNewDay) {
                dayHeaderCell.attr('class', 'budgetDayDetailChange')

                var dayDescriptionDiv = $('<div></div>').attr('class', 'budgetDailyDayTitle').attr('style', 'width:' + (30 * (oCalData.columnLimits[dayKey].max - oCalData.columnLimits[dayKey].min)) + 'px');

                var dayText = $('<div></div>').html(Globalize.formatDate(oCalData.calendarData[dayKey].dayDate, { date: "full" }));
                if (oCalData.calendarData[dayKey].isFeast) dayDescriptionDiv.append(dayText.attr('class', 'dailyColumnIsFeast'));
                dayDescriptionDiv.append(dayText);

                mainDayHeaderCell.append(dayDescriptionDiv);
                bNewDay = false;
            }

            mainDayHeaderCell.append($('<div></div>').attr('class', 'NorthCell dayInfo DailyCell').attr('style', i != oCalData.columnLimits[dayKey].min ? 'padding-top:20px' : '').html(day.header[i].Row2Text));

            //if (oCalData.BudgetHeader.PeriodHeaderData[i].FeastDay) mainDayHeaderCell.append($('<div></div>').attr('class', 'columnIsFeast'));

            tHeaderRow.append(dayHeaderCell.append(mainDayHeaderCell));
        }
    });

    var tmpCell = $('<th style="width:100%"></th>').html('&nbsp;');
    tHeaderRow.append(tmpCell);
    tHeader.append(tHeaderRow);

    return tHeader;
};

Robotics.Client.Controls.roBudgetCalendarDetail.prototype.createCalendarTableBody = function (idTable, parentId) {
    var oClientMode = this;
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    //Creamos el body
    var tBody = $('<tbody></tbody>').attr('id', oCalendar.prefix + idTable + '_tbody').attr('class', 'bodySelectable');

    var keysCount = 0;
    var rowIndex = 0;

    Object.keys(oCalData.rowMax).forEach(function (pUnitkey, index) {
        keysCount++;
        var employeeRow = 0;

        for (var i = 0; i < oCalData.rowMax[pUnitkey].maxEmployees; i++) {
            var tBodyRow = $('<tr></tr>');
            if (i == 0) {
                var tFixedBodyCell = $('<td></td>').attr('rowspan', oCalData.rowMax[pUnitkey].maxEmployees);
                var mainFixedBodyCell = $('<div></div>').attr('class', 'BudgetFixed CalendarFixedHeader').attr('style', 'height:100%');
                var fixedEmployeeBodyCell = $('<div></div>').attr('class', 'CalendarBudgetFixed CalendarBudgetFixedBody').attr('style', 'height:100%');

                if (oClientMode.pUnitsData.length > 0) {
                    var textContainer = $('<div></div>').attr('class', 'EmployeeName').attr('style', 'height:' + (55 * oCalData.rowMax[pUnitkey].maxEmployees) + 'px !important;width:250px;display:table-cell;vertical-align:middle;');
                    var sObject = $('<select></select>').attr('id', oCalendar.prefix + '_pUnitSelect');

                    //var selectedObject = parseInt(Object.keys(oCalData.rowMax)[0], 10);

                    for (var punitIndex = 0; punitIndex < oClientMode.pUnitsData.length; punitIndex++) {
                        var option = $('<option />').text(oClientMode.pUnitsData[punitIndex].Name).val(oClientMode.pUnitsData[punitIndex].ID);
                        //if (parseInt(selectedObject, 10) == parseInt(oClientMode.pUnitsData[punitIndex].ID, 10)) option = option.prop('selected', 'selected');
                        sObject.append(option);
                    }

                    fixedEmployeeBodyCell.append(textContainer.append(sObject));
                } else {
                    var textContainer = $('<div></div>').attr('class', 'EmployeeName').attr('style', 'height:100% !important;line-height:' + (55 * oCalData.rowMax[pUnitkey].maxEmployees) + 'px').attr('title', oCalData.rowMax[pUnitkey].name);
                    textContainer.append($('<span style="padding-left:15px;white-space: nowrap;"></span>').html(oCalData.rowMax[pUnitkey].name));
                    fixedEmployeeBodyCell.append(textContainer);
                }

                tBodyRow.append(tFixedBodyCell.append(mainFixedBodyCell.append(fixedEmployeeBodyCell)));
            }

            var colIndex = 0;

            Object.keys(oCalData.calendarData).forEach(function (dayKey, index) {
                oClientMode.descriptionExists = false;
                var pUnitInfo = oCalData.calendarData[dayKey].pData[oCalData.rowMax[pUnitkey].idPunit];

                var bNewDay = true;
                var bIsLastRow = false;
                if (employeeRow == (oCalData.rowMax[pUnitkey].maxEmployees - 1)) {
                    bIsLastRow = true;
                }

                if (employeeRow < pUnitInfo.EmployeesData.length) {
                    var cEmployee = pUnitInfo.EmployeesData[employeeRow];

                    for (var z = oCalData.columnLimits[dayKey].min; z < oCalData.columnLimits[dayKey].max; z++) {
                        tBodyRow.append(oClientMode.createDailyCalendarCell(cEmployee, cEmployee.HourData[z], rowIndex, colIndex, z, bNewDay, bIsLastRow));
                        bNewDay = false;
                        colIndex++;
                    }
                } else {
                    for (var z = oCalData.columnLimits[dayKey].min; z < oCalData.columnLimits[dayKey].max; z++) {
                        tBodyRow.append(oClientMode.createEmptyDailyCalendarCell(rowIndex, colIndex, bNewDay, bIsLastRow));
                        bNewDay = false;
                        colIndex++;
                    }
                }
            });

            tmpCell = $('<td style="width:100%"></td>').addClass('budgetDayDetailChange').html('&nbsp;');
            tBodyRow.append(tmpCell);

            tBody.append(tBodyRow);
            employeeRow++;
            rowIndex++;
        }
    });

    if (keysCount == 0) {
        var tBodyRow = $('<tr></tr>');
        var tFixedBodyCell = $('<td></td>');
        var mainFixedBodyCell = $('<div></div>').attr('class', 'BudgetFixed CalendarFixedHeader');
        var fixedEmployeeBodyCell = $('<div></div>').attr('class', 'CalendarBudgetFixed CalendarBudgetFixedHeader');

        tBodyRow.append(tFixedBodyCell.append(mainFixedBodyCell.append(fixedEmployeeBodyCell)));

        tmpCell = $('<td style="width:100%"></td>').html('&nbsp;');
        tBodyRow.append(tmpCell);

        tBody.append(tBodyRow);
    }
    return tBody;
};

Robotics.Client.Controls.roBudgetCalendarDetail.prototype.createDailyCalendarCell = function (positionData, cellInfo, rowPosition, columnPosition, dayColumn, bNewDay, bIsLastRow) {
    var oPUnit = this.oBaseControl;
    var oPUnitData = oPUnit.oCalendar;

    var calendarCell = $('<td></td>');

    if (columnPosition % 2 == 0) calendarCell.addClass('columnDailyCalendarOdd');
    if (bNewDay) calendarCell.addClass('budgetDayDetailChange');
    if (bIsLastRow && Object.keys(oPUnitData.rowMax).length > 1) calendarCell.addClass('budgetRowDetailChange');

    var calendarOuterContent = $('<div></div>').attr('id', oPUnit.ascxPrefix + '_calDailyCell_' + rowPosition + '_' + columnPosition).attr('class', 'calendarDailyBodyCell DailyCell');
    calendarOuterContent.attr('data-IDRow', rowPosition).attr('data-IDColumn', columnPosition);

    this.createDailyCalendarCellContent(calendarOuterContent, positionData, cellInfo, dayColumn, rowPosition);

    return calendarCell.append(calendarOuterContent);
};

Robotics.Client.Controls.roBudgetCalendarDetail.prototype.createEmptyDailyCalendarCell = function (rowPosition, columnPosition, bNewDay, bIsLastRow) {
    var oPUnit = this.oBaseControl;
    var oPUnitData = oPUnit.oCalendar;

    var calendarCell = $('<td></td>');

    if (columnPosition % 2 == 0) calendarCell.addClass('columnDailyCalendarOdd');
    if (bNewDay) calendarCell.addClass('budgetDayDetailChange');
    if (bIsLastRow && Object.keys(oPUnitData.rowMax).length > 1) calendarCell.addClass('budgetRowDetailChange');

    var calendarOuterContent = $('<div></div>').attr('id', oPUnit.ascxPrefix + '_calDailyCell_' + rowPosition + '_' + columnPosition).attr('class', 'calendarDailyBodyCell DailyCell');
    calendarOuterContent.attr('data-IDRow', rowPosition).attr('data-IDColumn', columnPosition);

    calendarOuterContent.append($('<div></div>').attr('class', 'calendarDailyInnerBodyCell'));

    return calendarCell.append(calendarOuterContent);
};

Robotics.Client.Controls.roBudgetCalendarDetail.prototype.createDailyCalendarCellContent = function (containter, positionData, cellInfo, columnPosition, rowPosition) {
    var oPUnit = this.oBaseControl;
    var oPUnitData = oPUnit.oCalendar;

    var calendarInnerContent = $('<div></div>').attr('class', 'calendarDailyInnerBodyCell');
    var nameShift = null;
    var changeShift = null;
    var shiftAssigned = positionData.ShiftData;

    if (cellInfo.DailyHourType != Robotics.Client.Constants.DailyHourType.Untyped) {
        var startColor = '#ffffff';
        var shiftName = '';

        var assignmentStyle = '';
        var assignmentName = '';
        var marginAssignment = '';

        assignmentName = " - " + shiftAssigned.Name;
        assignmentStyle = 'border-bottom: 8px ' + shiftAssigned.Color + ' solid;'
        marginAssignment = 'margin-top: -30px;'

        startColor = positionData.AssignmentData.Color;
        shiftName = positionData.AssignmentData.Name;

        if (positionData.IDEmployee > 0) {
            switch (cellInfo.DailyHourType) {
                case Robotics.Client.Constants.DailyHourType.Complementary:
                    startColor = new Robotics.Client.Common.roHtmlColor().shadeBlendConvert(-0.2, startColor);
                    break;
            }
        }

        calendarInnerContent.attr('style', 'background: ' + startColor + ';' + assignmentStyle);

        if (!this.descriptionExists && ((columnPosition == 0) || (cellInfo.DailyHourType != positionData.HourData[columnPosition - 1].DailyHourType))) {
            this.descriptionExists = true;

            if (positionData.IDEmployee > 0) {
                changeShift = this.generateShiftTypeInfoDailyContainer(positionData.EmployeeName + ' - ' + (positionData.AssignmentData.Name).toUpperCase(), startColor, 0, marginAssignment, positionData, true, columnPosition, rowPosition);
            } else {
                changeShift = this.generateShiftTypeInfoDailyContainer((shiftName + assignmentName).toUpperCase(), startColor, 0, marginAssignment, positionData, false, columnPosition, rowPosition);
            }
        }
    }

    containter.append(nameShift, calendarInnerContent, changeShift);
};

Robotics.Client.Controls.roBudgetCalendarDetail.prototype.generateShiftInfoDailyContainer = function (shiftName, color) {
    var oPUnit = this.oBaseControl;
    var oPUnitData = oPUnit.oCalendar;

    var shiftInfoContainer = $('<div></div>').attr('class', 'calendarShiftInfoDailyContainer').attr('title', shiftName).append($("<span class='fontBold calendarFeastLeft'></span>").html(shiftName));

    shiftInfoContainer.attr('style', 'color:' + new Robotics.Client.Common.roHtmlColor().invertCssColor(color));

    return shiftInfoContainer;
};

Robotics.Client.Controls.roBudgetCalendarDetail.prototype.generateShiftTypeInfoDailyContainer = function (assignmentText, color, hours, marginAssignment, positionData, hasEmployee, columnPosition, rowPosition) {
    var oPUnit = this.oBaseControl;
    var oPUnitData = oPUnit.oCalendar;

    var cellText = assignmentText;
    var shiftInfoContainer = $('<div></div>').attr('class', 'calendarBudgetDetailText');//.attr('title', cellText);

    if (!hasEmployee) {
        var button = $('<div></div>').attr('style', 'float:left').attr('class', 'assignEmployeeDetail');
        button.attr('data-idPosition', positionData.IdPositionData).attr('data-dayKey', positionData.Keys.day).attr('data-idPunit', positionData.Keys.pUnit).attr('data-idEmployee', positionData.IDEmployee);
        shiftInfoContainer.append(button);
    } else {
        var dayIndictments = [];
        if (positionData.Alerts != null && positionData.Alerts.OnAbsenceDays != null && positionData.Alerts.OnAbsenceDays) {
            dayIndictments.push({ employeeName: positionData.EmployeeName, indictment: { ErrorText: Globalize.formatMessage('roEmployeeAbsent') } });
        }

        if (positionData.Alerts != null && positionData.Alerts.Indictments != null && positionData.Alerts.Indictments.length > 0) {
            for (iInd = 0; iInd < positionData.Alerts.Indictments.length; iInd++) {
                dayIndictments.push({ employeeName: positionData.EmployeeName, indictment: Object.clone(positionData.Alerts.Indictments[iInd], true) });
            }
        }

        if (dayIndictments.length > 0) {
            var button = $('<div></div>').attr('id', oPUnit.ascxPrefix + '_tooltipIndictments_' + rowPosition + '_' + columnPosition).attr('style', 'float:left;').attr('class', 'budgetMainDetailOnAbsence');

            var indictmentTooltipContainer = $('<div></div>').attr('id', oPUnit.ascxPrefix + '_tooltipIndictmentsContainer_' + rowPosition + '_' + columnPosition).attr('class', 'tooltipIndictmentsContainer');
            var divIndictMentsTooltipTitle = $('<div></div>').html('<span class="fontBold fontBig" style="text-align:left">' + Globalize.formatMessage("roIndictmentsTitle") + '</span>');

            var ulInditmentsTooltipList = $('<ul></ul>');

            for (var iDayI = 0; iDayI < dayIndictments.length; iDayI++) {
                ulInditmentsTooltipList.append($('<li></li>').html(dayIndictments[iDayI].indictment.ErrorText))
            }

            indictmentTooltipContainer.append(divIndictMentsTooltipTitle, ulInditmentsTooltipList);
            shiftInfoContainer.append(button, indictmentTooltipContainer);
        }
    }

    shiftInfoContainer.append($('<span></span>').attr('style', 'float:left').html(cellText));

    if (hasEmployee) {
        var button = $('<div></div>').attr('style', 'float:left').attr('class', 'deleteAssignedEmployeeDetail');
        button.attr('data-idPosition', positionData.IdPositionData).attr('data-dayKey', positionData.Keys.day).attr('data-idPunit', positionData.Keys.pUnit).attr('data-idEmployee', positionData.IDEmployee);
        shiftInfoContainer.append(button);
    }

    shiftInfoContainer.attr('style', 'color:' + new Robotics.Client.Common.roHtmlColor().invertCssColor(color) + ';' + marginAssignment);

    return shiftInfoContainer;
};

Robotics.Client.Controls.roBudgetCalendarDetail.prototype.generateLoadFilters = function () {
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
    oParameters.pUnitFilter = this.selectedPunitId;

    oParameters.StampParam = new Date().getMilliseconds();

    return oParameters;
};

Robotics.Client.Controls.roBudgetCalendarDetail.prototype.alterLoadIndictmentsModeAndReload = function () {
    var oClientMode = this;
    var oCalendar = this.oBaseControl;

    if (oCalendar.loadIndictments == false) oCalendar.loadIndictments = true;
    else oCalendar.loadIndictments = false;

    oCalendar.setFiltersCookieValue();
    oClientMode.loadData();
};

Robotics.Client.Controls.roBudgetCalendarDetail.prototype.loadData = function (firstDate, endDate, orgChartFilter, typeView, pUnitFilter, loadIndictments) {
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

Robotics.Client.Controls.roBudgetCalendarDetail.prototype.saveChanges = function () {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;
};

Robotics.Client.Controls.roBudgetCalendarDetail.prototype.buildContextMenu = function (sender) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    var controlText = sender[0].innerText;

    items = {
        //'enter': { name: oCalendar.translator.translate(Robotics.Client.Language.Tags.ContextMenu_Details), disabled: false, icon: 'detail' }
    };

    return items;
};

Robotics.Client.Controls.roBudgetCalendarDetail.prototype.executeContextMenuAction = function (key, container) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    switch (key) {
        case "enter":
            this.enterDetailAction(oCalendar.selectedEmployee.ProductiveUnitData.ProductiveUnit.ID, oCalendar.selectedEmployee.ProductiveUnitData.IDNode, moment(oCalendar.selectedDay.PlanDate).format('DD/MM/YYYY'));
            break;
    }
};

Robotics.Client.Controls.roBudgetCalendarDetail.prototype.cancelCurrentMultipleSelect = function (sender, forceCancel) {
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

Robotics.Client.Controls.roBudgetCalendarDetail.prototype.cancelCurrentMultipleHeaderSelect = function (sender, forceCancel) {
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

Robotics.Client.Controls.roBudgetCalendarDetail.prototype.pasteSelection = function (container) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    var clickedRow = parseInt($(container).attr('data-IDRow'), 10);
    var clickedColumn = parseInt($(container).attr('data-IDColumn'), 10);

    this.pasteSelectionEnd(clickedRow, clickedColumn);
};

Robotics.Client.Controls.roBudgetCalendarDetail.prototype.pasteSelectionEnd = function (clickedRow, clickedColumn) {
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

                for (var k = 0; k < pUnitModeToCopy.UnitModePositions.length; k++) {
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
                    //
                }
            }
        }
    } else {
        //if (!oCalendar.isBatchMode()) oCalendar.showErrorPopup("Error.Title", "error", "Calendar.Client.RowChangeNotAllowed", "", "Error.OK", "Error.OKDesc", oCalendar.defaultMessageAction);
    }
};

Robotics.Client.Controls.roBudgetCalendarDetail.prototype.copyHeaderSelection = function (container) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    if ((oCalendar.selectedMinHeaderColumn == -1 || oCalendar.selectedMaxHeaderColumn == -1) && container != null) {
        var clickedColumn = parseInt($(container).attr('data-IDColumn'), 10);

        oCalendar.selectedMinHeaderColumn = oCalendar.selectedMaxHeaderColumn = clickedColumn;
        $(container).addClass("ui-selected");
    }
    oCalendar.selectionHeaderCopied = true;
};

Robotics.Client.Controls.roBudgetCalendarDetail.prototype.copySelection = function (container) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    if ((oCalendar.selectedMinRow == -1 || oCalendar.selectedMaxRow == -1 || oCalendar.selectedMinColumn == -1 || oCalendar.selectedMaxColumn == -1) && container != null) {
        var clickedRow = parseInt($(container).attr('data-IDRow'), 10);
        var clickedColumn = parseInt($(container).attr('data-IDColumn'), 10);

        oCalendar.selectedMinRow = oCalendar.selectedMaxRow = clickedRow;
        oCalendar.selectedMinColumn = oCalendar.selectedMaxColumn = clickedColumn;

        //$(container).addClass("ui-selected");
    }

    oCalendar.selectionCopied = true;
};

Robotics.Client.Controls.roBudgetCalendarDetail.prototype.enterDetailAction = function (idPunit, idOrgCharNode, date) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    if (oCalendar.hasChanges == false) {
        this.enterDetailActionFinally(idPunit, idOrgCharNode, date);
    } else {
        oCalendar.showChangesWarning(oCalendar.clientInstanceName + ".clientMode.enterDetailActionFinally(" + idPunit + "," + idOrgCharNode + ",'" + date + "');");
    }
};

Robotics.Client.Controls.roBudgetCalendarDetail.prototype.enterDetailActionFinally = function (idPunit, idOrgCharNode, date) {
    var url = '' //ISM: TODO To define what to show on cell detail
    var Title = '';
};

Robotics.Client.Controls.roBudgetCalendarDetail.prototype.buildHeaderContextMenu = function (sender) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    var controlText = sender[0].innerText;
    var clickedColumn = parseInt($(sender).attr('data-IDColumn'), 10);

    items = {
        'sort': { name: oCalendar.translator.translate(Robotics.Client.Language.Tags.ContextMenu_Sort), disabled: false, icon: 'sort' }
    };

    return items;
};

Robotics.Client.Controls.roBudgetCalendarDetail.prototype.executeHeaderContextMenuAction = function (key, container) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    switch (key) {
        case "sort":
            oCalendar.sortColumn = parseInt($(container).attr('data-IDColumn'), 10);
            //ISM: To define sort dialog
            oCalendar.calendarSortDialog.dialog('open');
            break;
    }
};

Robotics.Client.Controls.roBudgetCalendarDetail.prototype.selectHeaderMultiple = function (origin, destination) {
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

Robotics.Client.Controls.roBudgetCalendarDetail.prototype.selectMultiple = function (origin, destination) {
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

Robotics.Client.Controls.roBudgetCalendarDetail.prototype.setSingleHeaderSelectedObejct = function (sender) {
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

Robotics.Client.Controls.roBudgetCalendarDetail.prototype.setSingleSelectedObject = function (sender) {
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

        //oCalendar.selectedEmployee = oCalData.BudgetData[parseInt($(sender).attr('data-IDRow'), 10)];
        //if (oCalendar.viewRange != Robotics.Client.Constants.ViewRange.Daily) {
        //    oCalendar.selectedDay = oCalData.BudgetData[parseInt($(sender).attr('data-IDRow'), 10)].PeriodData.DayData[parseInt($(sender).attr('data-IDColumn'), 10)];
        //} else {
        //    oCalendar.selectedDay = oCalData.BudgetData[parseInt($(sender).attr('data-IDRow'), 10)].PeriodData.DayData[0];
        //}

        //oCalendar.selectedContainer = $(sender);
        //oCalendar.selectedContainer.addClass('singleCellSelected');
    }

    if (this.OnSelectedCell != null) this.OnSelectedCell(oCalendar.selectedEmployee, oCalendar.selectedDay, oCalendar.selectedContainer);
};

Robotics.Client.Controls.roBudgetCalendarDetail.prototype.endSelectOperation = function (sender) {
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

Robotics.Client.Controls.roBudgetCalendarDetail.prototype.endHeaderSelectOperation = function (sender) {
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
Robotics.Client.Controls.roBudgetCalendarDetail.prototype.sortCalendar = function () {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    var sortParams = oCalendar.sortElements.split(',');

    oCalData.CalendarData = oCalData.CalendarData.sort(dynamicSortMultiple(sortParams, oCalendar.viewRange == Robotics.Client.Constants.ViewRange.Daily ? 0 : oCalendar.sortColumn));
    oCalendar.refreshTables(null, false, true);
    oCalendar.calendarSortDialog.dialog("close");
};

Robotics.Client.Controls.roBudgetCalendarDetail.prototype.endCallback = function (action, objResult, objResultParams) {
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
        case Robotics.Client.Constants.Actions.GetAvailablePositionEmployees:
            oCalendar.prepareBudgetEmployeesDialog(objResult, objResultParams);
            break;
        case Robotics.Client.Constants.Actions.AddEmployeePlanOnPosition:
        case Robotics.Client.Constants.Actions.RemoveEmployeeFromPosition:
            oClientMode.refreshFullData();
            break;
    }

    if (action == Robotics.Client.Constants.Actions.DiscardBudgetAndContinue || action == Robotics.Client.Constants.Actions.SaveBudgetChangesAndContinue) {
        eval(oCalendar.onContinueFunc);
        oCalendar.onContinueFunc = '';
    }

    if (action != Robotics.Client.Constants.Actions.AddEmployeePlanOnPosition && action != Robotics.Client.Constants.Actions.RemoveEmployeeFromPosition) this.loadingFunctionExtended(false);
};
Robotics.Client.Controls.roBudgetCalendarDetail.prototype.refreshFullData = function () {
    this.BudgetData = null;

    this.pUnitsData = null;

    this.selectedPUnit = null;
    this.selectedPUnitMode = null;
    this.selectedPUnitModePosition = null;
    this.selectedDate = null;
    this.selectedEmployee = null;

    this.loadData();
}

Robotics.Client.Controls.roBudgetCalendarDetail.prototype.saveChangesResponse = function (objResult, objResultParams) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    oCalendar.setHasChanges(false);

    this.BudgetData = objResult;
    oCalendar.oCalendar = this.BuildDayDetailObjectFromCalendar(objResult);

    oCalendar.refreshTables(null, false, true);

    this.cancelCurrentMultipleSelect(null, true);
    this.cancelCurrentMultipleHeaderSelect(null, true);
};

Robotics.Client.Controls.roBudgetCalendarDetail.prototype.loadDataResponse = function (objResult, objResultParams) {
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

    this.BudgetData = objResult;
    this.pUnitsData = objResultParams;

    oCalendar.oCalendar = this.BuildDayDetailObjectFromCalendar(objResult);
    oCalendar.setHasChanges(false);

    oCalendar.refreshTables(null, false, true);
};

Robotics.Client.Controls.roBudgetCalendarDetail.prototype.BuildDayDetailObjectFromCalendar = function (objResult) {
    this.loadingFunctionExtended(false); //Only in development

    var oCalData = objResult;
    var newModel = {
        calendarData: {},
        rowMax: {},
        columnLimits: {}
    };

    //Creamos las cabezeras del nuevo objeto calendario
    var actualStart = 0;
    var actualDay = moment(objResult.FirstDay);
    while (actualDay < moment(objResult.LastDay)) {
        newModel.calendarData[actualDay.format('YYYYMMDD')] = { dayDate: actualDay.toDate(), isFeast: oCalData.BudgetHeader.PeriodHeaderData[actualStart + 48].FeastDay, header: oCalData.BudgetHeader.PeriodHeaderData.slice(actualStart, actualStart + (48 * 3)), pData: {} };

        actualDay = actualDay.add(1, 'day');
        actualStart += 48;
    }
    newModel.calendarData[actualDay.format('YYYYMMDD')] = { dayDate: actualDay.toDate(), isFeast: oCalData.BudgetHeader.PeriodHeaderData[actualStart + 48].FeastDay, header: oCalData.BudgetHeader.PeriodHeaderData.slice(actualStart, actualStart + (48 * 3)), pData: {} };

    //Creamos las filas por unidades productivas
    Object.keys(newModel.calendarData).forEach(function (key, index) {
        for (var i = 0; i < oCalData.BudgetData.length; i++) {
            var pUnitDay = {};
            pUnitDay.ProductiveUnit = Object.clone(oCalData.BudgetData[i].ProductiveUnitData.ProductiveUnit, true);
            pUnitDay.IDProductiveUnit = oCalData.BudgetData[i].ProductiveUnitData.ProductiveUnit.ID;
            pUnitDay.IDSecurityNode = oCalData.BudgetData[i].ProductiveUnitData.IDNode;
            pUnitDay.EmployeesData = [];

            newModel.calendarData[key].pData[oCalData.BudgetData[i].ProductiveUnitData.ProductiveUnit.ID] = pUnitDay;
        }
    });

    //Rellenamos las filas con todos los empleados de los modos disponibles
    for (var i = 0; i < oCalData.BudgetData.length; i++) {
        for (var dayIndex = 0; dayIndex < oCalData.BudgetData[i].PeriodData.DayData.length; dayIndex++) {
            var dayKey = moment(oCalData.BudgetData[i].PeriodData.DayData[dayIndex].PlanDate).format('YYYYMMDD');
            var oActualUnitPositions = typeof oCalData.BudgetData[i].PeriodData.DayData[dayIndex].ProductiveUnitMode != 'undefined' ? oCalData.BudgetData[i].PeriodData.DayData[dayIndex].ProductiveUnitMode.UnitModePositions : [];

            for (var iPos = 0; iPos < oActualUnitPositions.length; iPos++) {
                var oEmployees = oActualUnitPositions[iPos].EmployeesData;
                for (var iEmp = 0; iEmp < oEmployees.length; iEmp++) {
                    var newData = Object.clone(oEmployees[iEmp], true);
                    newData.AssignmentData = Object.clone(oActualUnitPositions[iPos].AssignmentData, true);
                    newData.IdPositionData = Object.clone(oActualUnitPositions[iPos].ID, true);
                    newData.Keys = { day: dayKey, pUnit: oCalData.BudgetData[i].ProductiveUnitData.ProductiveUnit.ID };
                    newModel.calendarData[dayKey].pData[oCalData.BudgetData[i].ProductiveUnitData.ProductiveUnit.ID].EmployeesData.push(newData);
                }
                //añado empleados ficticios hasta el límite.

                var emptyAddedEmployees = oActualUnitPositions[iPos].EmployeesData.length;
                while (emptyAddedEmployees < oActualUnitPositions[iPos].Quantity) {
                    var newDataEmp = {
                        IDEmployee: -1,
                        EmployeeName: Globalize.formatMessage('roAddEmployeeInShiftAssignment', oActualUnitPositions[iPos].AssignmentData.Name, oActualUnitPositions[iPos].ShiftData.Name),
                        AssignmentData: Object.clone(oActualUnitPositions[iPos].AssignmentData, true),
                        ShiftData: Object.clone(oActualUnitPositions[iPos].ShiftData, true),
                        HourData: Object.clone(oActualUnitPositions[iPos].ShiftHourData, true),
                        IdPositionData: Object.clone(oActualUnitPositions[iPos].ID, true),
                        Keys: { day: dayKey, pUnit: oCalData.BudgetData[i].ProductiveUnitData.ProductiveUnit.ID }
                    };

                    newModel.calendarData[dayKey].pData[oCalData.BudgetData[i].ProductiveUnitData.ProductiveUnit.ID].EmployeesData.push(newDataEmp);
                    emptyAddedEmployees++;
                }

                //Si se puede expandir y no estoy en el límite añado uno extra

                if (oActualUnitPositions[iPos].IsExpandable) {
                    var newDataEmp = {
                        IDEmployee: -2,
                        EmployeeName: Globalize.formatMessage('roAddEmployeeInShiftAssignment', oActualUnitPositions[iPos].AssignmentData.Name, oActualUnitPositions[iPos].ShiftData.Name),
                        AssignmentData: Object.clone(oActualUnitPositions[iPos].AssignmentData, true),
                        ShiftData: Object.clone(oActualUnitPositions[iPos].ShiftData, true),
                        HourData: Object.clone(oActualUnitPositions[iPos].ShiftHourData, true),
                        IdPositionData: Object.clone(oActualUnitPositions[iPos].ID, true),
                        Keys: { day: dayKey, pUnit: oCalData.BudgetData[i].ProductiveUnitData.ProductiveUnit.ID }
                    };

                    newModel.calendarData[dayKey].pData[oCalData.BudgetData[i].ProductiveUnitData.ProductiveUnit.ID].EmployeesData.push(newDataEmp);
                }
            }
        }
    }

    //Marcamos el número máximo de filas que tendrá cada unidad productiva para pintar la línea.
    for (var iPunit = 0; iPunit < oCalData.BudgetData.length; iPunit++) {
        var strKey = oCalData.BudgetData[iPunit].ProductiveUnitData.ProductiveUnit.ID;

        Object.keys(newModel.calendarData).forEach(function (key, index) {
            if (typeof newModel.rowMax[strKey] == 'undefined') newModel.rowMax[strKey] = { idPunit: oCalData.BudgetData[iPunit].ProductiveUnitData.ProductiveUnit.ID, name: oCalData.BudgetData[iPunit].ProductiveUnitData.ProductiveUnit.Name, maxEmployees: 0 }
            if (newModel.calendarData[key].pData[newModel.rowMax[strKey].idPunit].EmployeesData.length > newModel.rowMax[strKey].maxEmployees) newModel.rowMax[strKey].maxEmployees = newModel.calendarData[key].pData[newModel.rowMax[strKey].idPunit].EmployeesData.length;
        });

        if (newModel.rowMax[strKey].maxEmployees == 0) newModel.rowMax[strKey].maxEmployees = 2;
    }

    //Marcamos el mínimo y máximo de la columna a pintar entre todo el rango del día
    Object.keys(newModel.calendarData).forEach(function (dayKey, dayIndex) {
        if (typeof newModel.columnLimits[dayKey] == 'undefined') newModel.columnLimits[dayKey] = { min: 144, max: 0 };
        newModel.columnLimits[dayKey].min = 144;
        newModel.columnLimits[dayKey].max = 0;

        Object.keys(newModel.calendarData[dayKey].pData).forEach(function (pUnitKey, pUnitIndex) {
            var minEmployee = 144;
            var maxEmployee = 0;
            var bExists = false;

            for (var i = 0; i < newModel.calendarData[dayKey].pData[pUnitKey].EmployeesData.length; i++) {
                bExists = true;
                var cEmp = newModel.calendarData[dayKey].pData[pUnitKey].EmployeesData[i];

                for (var iHour = 0; iHour < cEmp.HourData.length; iHour++) {
                    if (cEmp.HourData[iHour].DailyHourType != Robotics.Client.Constants.DailyHourType.Untyped && iHour < minEmployee) minEmployee = iHour;

                    if (cEmp.HourData[iHour].DailyHourType != Robotics.Client.Constants.DailyHourType.Untyped && iHour > maxEmployee) maxEmployee = iHour;
                }

                if (minEmployee <= newModel.columnLimits[dayKey].min) newModel.columnLimits[dayKey].min = (minEmployee - 2);
                if (maxEmployee >= newModel.columnLimits[dayKey].max) newModel.columnLimits[dayKey].max = (maxEmployee + 3);
            }

            if (minEmployee > maxEmployee && bExists) {
                newModel.columnLimits[dayKey].min = 64;
                newModel.columnLimits[dayKey].max = 88;
            }
        });
    });

    return newModel;
}

Robotics.Client.Controls.roBudgetCalendarDetail.prototype.onAcceptPUnitsDialog = function () {
};

Robotics.Client.Controls.roBudgetCalendarDetail.prototype.preparePUnitsDialog = function (objResult) {
    var oCalendar = this.oBaseControl;
};

Robotics.Client.Controls.roBudgetCalendarDetail.prototype.saveAndContinue = function () {
    var oCalendar = this.oBaseControl;

    var oParameters = {};
    oParameters.budget = oCalendar.oCalendar;
    oParameters.StampParam = new Date().getMilliseconds();

    oCalendar.performAction(oParameters, Robotics.Client.Constants.Actions.SaveBudgetChangesAndContinue);
};

Robotics.Client.Controls.roBudgetCalendarDetail.prototype.discardAndContinue = function (onAcceptFunc) {
    var oCalendar = this.oBaseControl;

    var oParameters = this.generateLoadFilters();

    oCalendar.performAction(oParameters, Robotics.Client.Constants.Actions.DiscardBudgetAndContinue);
};

Robotics.Client.Controls.roBudgetCalendarDetail.prototype.refresh = function () {
    var oCalendar = this.oBaseControl;

    var oParameters = this.generateLoadFilters();
    oCalendar.sortColumn = -1;
    oCalendar.performAction(oParameters, Robotics.Client.Constants.Actions.LoadBudget);
};

Robotics.Client.Controls.roBudgetCalendarDetail.prototype.getEmployeeCountResume = function () {
    var oCalendar = this.oBaseControl;

    return "";
};

Robotics.Client.Controls.roBudgetCalendarDetail.prototype.selectObjects = function (srcElement) {
    var oClientMode = this;
    var oPUnit = this.oBaseControl;
    var oPUnitData = oPUnit.oCalendar;

    var idPosition = parseInt(srcElement.attr('data-idPosition'), 10);
    var idPunit = parseInt(srcElement.attr('data-idPunit'), 10);
    var idEmployee = parseInt(srcElement.attr('data-idEmployee'), 10);

    oClientMode.selectedDate = moment(srcElement.attr('data-dayKey'), 'YYYYMMDD');

    for (var i = 0; i < this.BudgetData.BudgetData.length; i++) {
        if (this.BudgetData.BudgetData[i].ProductiveUnitData.ProductiveUnit.ID == idPunit) {
            oClientMode.selectedPUnit = this.BudgetData.BudgetData[i].ProductiveUnitData.ProductiveUnit;

            for (var dayIndex = 0; dayIndex < this.BudgetData.BudgetData[i].PeriodData.DayData.length; dayIndex++) {
                if (moment(this.BudgetData.BudgetData[i].PeriodData.DayData[dayIndex].PlanDate).isSame(oClientMode.selectedDate)) {
                    var pUnitMode = this.BudgetData.BudgetData[i].PeriodData.DayData[dayIndex].ProductiveUnitMode;
                    oClientMode.selectedPUnitMode = pUnitMode;

                    for (iPos = 0; iPos < pUnitMode.UnitModePositions.length; iPos++) {
                        if (pUnitMode.UnitModePositions[iPos].ID == idPosition) {
                            oClientMode.selectedPUnitModePosition = Object.clone(pUnitMode.UnitModePositions[iPos]);

                            if (idEmployee > 0) {
                                for (var iEmps = 0; iEmps < pUnitMode.UnitModePositions[iPos].EmployeesData.length; iEmps++) {
                                    if (pUnitMode.UnitModePositions[iPos].EmployeesData[iEmps].IDEmployee == idEmployee) {
                                        oClientMode.selectedEmployee = Object.clone(pUnitMode.UnitModePositions[iPos].EmployeesData[iEmps], true);
                                    }
                                }
                            }

                            break;
                        }
                    }
                }

                if (oClientMode.selectedPUnitModePosition != null) break;
            }
        }
        if (oClientMode.selectedPUnitModePosition != null) break;
    }
}

Robotics.Client.Controls.roBudgetCalendarDetail.prototype.onAddEmployee = function (srcElement) {
    var oClientMode = this;
    var oPUnit = this.oBaseControl;
    var oPUnitData = oPUnit.oCalendar;

    oClientMode.selectObjects(srcElement);

    if (oClientMode.selectedPUnitModePosition != null) {
        var oParameters = {};
        oParameters.orgChartFilter = parseInt(oPUnit.employeeFilter, 10);
        oParameters.pUnitModePosition = oClientMode.selectedPUnitModePosition;
        oParameters.firstDate = oClientMode.selectedDate.toDate();
        oParameters.StampParam = new Date().getMilliseconds();

        oPUnit.performAction(oParameters, Robotics.Client.Constants.Actions.GetAvailablePositionEmployees);
    }
}

Robotics.Client.Controls.roBudgetCalendarDetail.prototype.prepareBudgetEmployeesDialog = function (oEmployeeList, oNodeStatus) {
    var oPUnit = this.oBaseControl;
    var oPUnitData = oPUnit.oCalendar;

    var oParameters = {};
    oParameters.pUnit = this.selectedPUnit;
    oParameters.pUnitMode = this.selectedPUnitMode;
    oParameters.pUnitModePosition = this.selectedPUnitModePosition;
    oParameters.sDate = this.selectedDate.toDate();

    oPUnit.budgetEmployeesDialog.dialog("open");
    oPUnit.budgetEmployeesManager.InitializeDialog(oEmployeeList, oNodeStatus, oParameters);
};

Robotics.Client.Controls.roBudgetCalendarDetail.prototype.onAcceptBudgetEmployeesDialog = function () {
    var oClientMode = this;
    var oPUnit = this.oBaseControl;
    var oPUnitData = oPUnit.oCalendar;

    var oParameters = {};

    oClientMode.scrollInitPosition = { top: $('#' + oPUnit.prefix + 'Main div.fht-fixed-body div.fht-tbody').scrollTop(), left: $('#' + oPUnit.prefix + 'Main div.fht-fixed-body div.fht-tbody').scrollLeft() }

    var selEmps = oPUnit.budgetEmployeesManager.getMultipleEmployeeSelection();

    var empParam = [];
    var maxEmployees = selEmps.length;

    if (!oClientMode.selectedPUnitModePosition.IsExpandable) {
        maxEmployees = oClientMode.selectedPUnitModePosition.Quantity - oClientMode.selectedPUnitModePosition.EmployeesData.length;
    }

    if (maxEmployees > selEmps.length) maxEmployees = selEmps.length;

    for (var i = 0; i < maxEmployees; i++) {
        empParam.push({
            EmployeeName: selEmps[i].EmployeeName,
            IDEmployee: selEmps[i].IDEmployee,
            ShiftData: Object.clone(oClientMode.selectedPUnitModePosition.ShiftData, true)
        });
    };

    oParameters.orgChartFilter = parseInt(oPUnit.employeeFilter, 10);
    oParameters.pUnitModePosition = oClientMode.selectedPUnitModePosition;
    oParameters.firstDate = oClientMode.selectedDate.toDate();
    oParameters.employeeData = empParam;
    oParameters.StampParam = new Date().getMilliseconds();

    oPUnit.performAction(oParameters, Robotics.Client.Constants.Actions.AddEmployeePlanOnPosition);

    oPUnit.budgetEmployeesDialog.dialog("close");
};

Robotics.Client.Controls.roBudgetCalendarDetail.prototype.onRemoveEmployee = function (srcElement) {
    var oClientMode = this;
    var oPUnit = this.oBaseControl;
    var oPUnitData = oPUnit.oCalendar;

    oClientMode.selectObjects(srcElement);

    oClientMode.scrollInitPosition = { top: $('#' + oPUnit.prefix + 'Main div.fht-fixed-body div.fht-tbody').scrollTop(), left: $('#' + oPUnit.prefix + 'Main div.fht-fixed-body div.fht-tbody').scrollLeft() }

    var result = DevExpress.ui.dialog.confirm(Globalize.formatMessage("roDeleteConfirm"), Globalize.formatMessage("roDeleteTitle"));
    result.done(function (dialogResult) {
        if (dialogResult) {
            var oParameters = {};
            oParameters.orgChartFilter = parseInt(oPUnit.employeeFilter, 10);
            oParameters.pUnitModePosition = oClientMode.selectedPUnitModePosition;
            oParameters.firstDate = oClientMode.selectedDate.toDate();
            oParameters.employeeData = [oClientMode.selectedEmployee];
            oParameters.StampParam = new Date().getMilliseconds();

            oPUnit.performAction(oParameters, Robotics.Client.Constants.Actions.RemoveEmployeeFromPosition);
        }
    });
};
