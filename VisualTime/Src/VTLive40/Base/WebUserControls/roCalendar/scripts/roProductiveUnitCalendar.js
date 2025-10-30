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

Robotics.Client.Controls.roProductiveUnitCalendar = function (baseControl) {
    this.name = "Robotics.Client.Controls.roProductiveUnitCalendar";

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

    this.descriptionExists = false;
    //Indica si la acción realizada en el calendario el origen es una fila(menu contextual), o viene de eventos externos(nueva fila)
    this.isActionOnRow = false;
};

Robotics.Client.Controls.roProductiveUnitCalendar.prototype.getLayout = function () {
    return this.pageLayoutOptions;
};

Robotics.Client.Controls.roProductiveUnitCalendar.prototype.setHasChanges = function (bolHasChanges) {
    var oPUnit = this.oBaseControl;

    oPUnit.hasChanges = bolHasChanges;
};

Robotics.Client.Controls.roProductiveUnitCalendar.prototype.create = function () {
    var oPUnit = this.oBaseControl;
    var oPUnitData = oPUnit.oCalendar;

    this.buildHTMLStructure();

    oPUnit.pageLayout = oPUnit.container.layout(this.pageLayoutOptions);
};

Robotics.Client.Controls.roProductiveUnitCalendar.prototype.loadData = function (oUnitModePositions) {
    var oPUnit = this.oBaseControl;

    oPUnit.firstDate = moment().startOf('day').toDate();
    oPUnit.endDate = oPUnit.firstDate;
    clearTimeout(oPUnit.refreshTimmer);
    oPUnit.refreshTimmer = -1;
    oPUnit.sortColumn = -1;
    oPUnit.dailyPeriod = 30;
    oPUnit.oCalendar = oUnitModePositions;

    oPUnit.initialize();
    oPUnit.setHasChanges(false);

    oPUnit.refreshTables(null, false, true);
};

Robotics.Client.Controls.roProductiveUnitCalendar.prototype.getPositions = function () {
    var oPUnit = this.oBaseControl;

    return oPUnit.oCalendar;
};

Robotics.Client.Controls.roProductiveUnitCalendar.prototype.buildHTMLStructure = function () {
    var oPUnit = this.oBaseControl;
    var oPUnitData = oPUnit.oCalendar;

    var mainCenterLayout = $('<div></div>').attr('id', oPUnit.prefix + Robotics.Client.Constants.LayoutNames.Center).attr('class', 'ui-layout-center');

    //Panel para el calendario
    var calendarLayout = $('<div></div>').attr('id', oPUnit.prefix + Robotics.Client.Constants.LayoutNames.Calendar).attr('class', 'ui-layout-center');

    // Añadimos todo al panel central
    mainCenterLayout.append(calendarLayout);

    oPUnit.container.append(mainCenterLayout);
};

Robotics.Client.Controls.roProductiveUnitCalendar.prototype.onresize = function (oProductiveUnitCalendar) {
    return function () {
        oProductiveUnitCalendar.loadingFunctionExtended(true);
        oProductiveUnitCalendar.oBaseControl.refreshTables(oProductiveUnitCalendar, true, true);
        oProductiveUnitCalendar.loadingFunctionExtended(false);
    };
};

Robotics.Client.Controls.roProductiveUnitCalendar.prototype.loadingFunctionExtended = function (showLoading) {
    var oPUnit = this.oBaseControl;

    if ((oPUnit.isShowingLoader && !showLoading) || (!oPUnit.isShowingLoader && showLoading)) {
        oPUnit.isShowingLoader = showLoading;

        if (showLoading) {
            $('#loadingCalendar').show();
        } else {
            $('#loadingCalendar').hide();
        }

        oPUnit.loadingFunc(showLoading);
    }
};

Robotics.Client.Controls.roProductiveUnitCalendar.prototype.getContextMenuSelector = function () {
    var selector = '.calendarDailyBodyCell';
    return selector;
}

Robotics.Client.Controls.roProductiveUnitCalendar.prototype.getContextMenuHeaderSelector = function () {
    return null;
};

Robotics.Client.Controls.roProductiveUnitCalendar.prototype.buildContextMenu = function (sender) {
    var oPUnit = this.oBaseControl;

    var controlText = sender[0].innerText;

    items = {
        'changeShift': { name: Globalize.formatMessage('roChangeShift'), disabled: false, icon: 'copyShift' },
        'complementary': { name: Globalize.formatMessage('roEditComplementary'), disabled: true, icon: 'complementary' },
        'assignment': { name: Globalize.formatMessage('roEditAssignment'), disabled: false, icon: 'assignment' },
        'split1': { name: '---------', disabled: true },
        'removePosition': { name: Globalize.formatMessage('roRemovePosition'), disabled: false, icon: 'removeblock' }
    };

    if (oPUnit.selectedEmployee != null && (oPUnit.selectedEmployee.ShiftData.ExistComplementaryData || oPUnit.selectedEmployee.ShiftData.ExistFloatingData)) {
        items['complementary'].disabled = false;
    }

    return items;
};

Robotics.Client.Controls.roProductiveUnitCalendar.prototype.executeContextMenuAction = function (key, container) {
    var oPUnit = this.oBaseControl;
    var oPUnitData = oPUnit.oCalendar;

    switch (key) {
        case "changeShift":
            this.changeShift(container);
            break;
        case "complementary":
            this.editComplementaryInfo(container);
            break;
        case "assignment":
            this.editAssignmentsInfo(container);
            break;
        case 'removePosition':
            this.removeSelectedPosition(container);
            break;
    }
};

Robotics.Client.Controls.roProductiveUnitCalendar.prototype.changeShift = function (container) {
    var oPUnit = this.oBaseControl;
    var oPUnitData = oPUnit.oCalendar;

    if (oPUnit.selectedEmployee != null) {
        oPUnit.complementaryShift = null;
        oPUnit.showComplementaryAssignDialog = false;
        oPUnit.showAssignmentsDialog = false;

        this.isActionOnRow = true;
        this.prepareShfitsDialog(null);
    }
};

Robotics.Client.Controls.roProductiveUnitCalendar.prototype.editComplementaryInfo = function (container) {
    var oPUnit = this.oBaseControl;
    var oPUnitData = oPUnit.oCalendar;

    if (oPUnit.selectedEmployee != null) {
        oPUnit.complementaryShift = oPUnit.selectedEmployee.ShiftData;
        oPUnit.showComplementaryAssignDialog = true;
        oPUnit.showAssignmentsDialog = false;

        this.isActionOnRow = true;
        if (typeof (oPUnit.shiftsExtendedDataCache[oPUnit.complementaryShift.ID]) == 'undefined') {
            var oParameters = {};
            oParameters.idShift = oPUnit.complementaryShift.ID;
            oParameters.StampParam = new Date().getMilliseconds();
            oPUnit.performAction(oParameters, Robotics.Client.Constants.Actions.ShiftLayerDefinition);
        } else {
            oPUnit.prepareComplementaryDialog(null);
        }
    }
};

Robotics.Client.Controls.roProductiveUnitCalendar.prototype.editAssignmentsInfo = function (container) {
    var oPUnit = this.oBaseControl;
    var oPUnitData = oPUnit.oCalendar;

    if (oPUnit.selectedEmployee != null) {
        oPUnit.assignmentShift = oPUnit.selectedEmployee.ShiftData;
        oPUnit.showComplementaryAssignDialog = false;
        oPUnit.showAssignmentsDialog = true;

        this.isActionOnRow = true;
        if (typeof (oPUnit.shiftsExtendedDataCache[oPUnit.assignmentShift.ID]) == 'undefined') {
            var oParameters = {};
            oParameters.idShift = oPUnit.assignmentShift.ID;
            oParameters.StampParam = new Date().getMilliseconds();
            oPUnit.performAction(oParameters, Robotics.Client.Constants.Actions.ShiftLayerDefinition);
        } else {
            this.prepareAssignmentsDialog(null);
        }
    }
};

Robotics.Client.Controls.roProductiveUnitCalendar.prototype.removeSelectedPosition = function (container) {
    var oPUnit = this.oBaseControl;
    var oPUnitData = oPUnit.oCalendar;

    var result = DevExpress.ui.dialog.confirm(Globalize.formatMessage("roDeleteConfirm"), Globalize.formatMessage("roDeleteTitle"));
    result.done(function (dialogResult) {
        if (dialogResult && oPUnit.selectedEmployee != null) {
            oPUnitData.remove(function (n) {
                return n.ID == oPUnit.selectedEmployee.ID;
            });

            oPUnit.refreshTables(null, false, true);
        }
    });
};

Robotics.Client.Controls.roProductiveUnitCalendar.prototype.processKeyUpEvent = function (e) {
    var oPUnit = this.oBaseControl;
    var oPUnitData = oPUnit.oCalendar;

    //Function requiered for inheritance
};

Robotics.Client.Controls.roProductiveUnitCalendar.prototype.processKeyDownEvent = function (e) {
    var oPUnit = this.oBaseControl;
    var oPUnitData = oPUnit.oCalendar;

    //Function requiered for inheritance
};

Robotics.Client.Controls.roProductiveUnitCalendar.prototype.onDrop = function (event) {
    var oPUnit = this.oBaseControl;
    var oPUnitData = oPUnit.oCalendar;

    //Function requiered for inheritance
};

Robotics.Client.Controls.roProductiveUnitCalendar.prototype.mapModeEvents = function () {
    var oClientMode = this;
    var oPUnit = this.oBaseControl;
    var oPUnitData = oPUnit.oCalendar;

    var selector = '.calendarDailyBodyCell';

    $(selector + ',.reviewDailyBodyCell').off('click');
    $(selector + ',.reviewDailyBodyCell').on('click', function (e) {
        oClientMode.setSingleSelectedObject(this);
    });

    //$(selector + ',.reviewDailyBodyCell').off('dblclick');
    //$(selector + ',.reviewDailyBodyCell').on('dblclick', function (e) {
    //    //Edit productiveUnit form requiered
    //});

    //$(selector).droppable({
    //    drop: function (event, ui) {
    //        return oClientMode.onDrop(event);
    //    }
    //});

    $(selector + ',.productiveUnit-quantity').off('change');
    $(selector + ',.productiveUnit-quantity').on('change', function (e) {
        var iRow = parseInt($(this).attr('data-IDRow'), 10);
        oPUnitData[iRow].Quantity = parseInt($(this).val(), 10);
    });

    $(selector + ',.productiveUnit-quantity').off('keydown')
    $(selector + ',.productiveUnit-quantity').on('keydown', function (e) {
        // Allow: backspace, delete, tab, escape, enter and .
        if ($.inArray(e.keyCode, [46, 8, 9, 27, 13, 110, 190]) !== -1 ||
            // Allow: Ctrl+A, Command+A
            (e.keyCode === 65 && (e.ctrlKey === true || e.metaKey === true)) ||
            // Allow: home, end, left, right, down, up
            (e.keyCode >= 35 && e.keyCode <= 40)) {
            // let it happen, don't do anything
            return;
        }
        // Ensure that it is a number and stop the keypress
        if ((e.shiftKey || (e.keyCode < 48 || e.keyCode > 57)) && (e.keyCode < 96 || e.keyCode > 105)) {
            e.preventDefault();
        }
    });

    $(selector + ',.productiveUnit-expand').off('change');
    $(selector + ',.productiveUnit-expand').on('change', function (e) {
        var iRow = parseInt($(this).attr('data-IDRow'), 10);
        oPUnitData[iRow].IsExpandable = $(this).prop('checked');//($(this).val().toUpperCase() == 'TRUE');
    });

    $('#' + oPUnit.prefix + 'Main div.fht-fixed-body div.fht-tbody').scrollLeft((oPUnit.firstCellPrinted - 46) * 30);
    oPUnit.firstCellPrinted = -1;
};

Robotics.Client.Controls.roProductiveUnitCalendar.prototype.setSingleSelectedObject = function (sender) {
    var oPUnit = this.oBaseControl;
    var oPUnitData = oPUnit.oCalendar;

    if (sender == null) {
        oPUnit.selectedEmployee = null;
        oPUnit.selectedContainer = null;
    } else {
        oPUnit.selectedEmployee = oPUnitData[parseInt($(sender).attr('data-IDRow'), 10)];
    }
};

Robotics.Client.Controls.roProductiveUnitCalendar.prototype.refreshModeTables = function (objectRef, isResizing, refreshCalendar) {
    var oPUnit = this;

    if (objectRef != null) {
        if (objectRef instanceof Robotics.Client.Controls.roCalendar) oPUnit = objectRef.clientMode;
        else oPUnit = objectRef;
    }

    if (refreshCalendar) {
        oPUnit.refreshMainTable(isResizing);
        this.oBaseControl.mapEvents()
    }
};

Robotics.Client.Controls.roProductiveUnitCalendar.prototype.refreshMainTable = function (isResizing) {
    var oPUnit = this.oBaseControl;

    try {
        $('#' + oPUnit.prefix + Robotics.Client.Constants.TableNames.Calendar).fixedHeaderTable('destroy');

        $('#' + oPUnit.prefix + Robotics.Client.Constants.TableNames.Calendar).find('tfoot').each(function (index) {
            if (this.id == '') {
                $(this).remove();
            }
        });
    } catch (e) { }

    if (!isResizing) this.createResumeTables(Robotics.Client.Constants.TableNames.Calendar, Robotics.Client.Constants.LayoutNames.Calendar);

    this.fixedCalendarTable = $('#' + oPUnit.prefix + Robotics.Client.Constants.TableNames.Calendar).fixedHeaderTable({
        altClass: 'odd',
        footer: false,
        fixedColumn: true
    });
};

Robotics.Client.Controls.roProductiveUnitCalendar.prototype.createResumeTables = function (idTable, parentId) {
    switch (idTable) {
        case Robotics.Client.Constants.TableNames.Calendar:
            this.createPUnitTable(idTable, parentId);
            break;
    }
};

Robotics.Client.Controls.roProductiveUnitCalendar.prototype.createPUnitTable = function (idTable, parentId) {
    var oPUnit = this.oBaseControl;

    var tableContainer = $('#' + oPUnit.prefix + parentId);
    tableContainer.empty();

    var tableElement = $('<table></table>').attr('id', oPUnit.prefix + idTable).attr('class', 'fancyTable');

    tableElement.append(this.createPUnitTableHeader(idTable, parentId));

    tableElement.append(this.createPUnitTableBody(idTable, parentId));

    tableContainer.append(tableElement);
};

Robotics.Client.Controls.roProductiveUnitCalendar.prototype.createPUnitTableHeader = function (idTable, parentId) {
    var oPUnit = this.oBaseControl;
    var oPUnitData = oPUnit.oCalendar;

    //Creamos el header
    var tHeader = $('<thead></thead>').attr('id', oPUnit.prefix + idTable + '_thead').attr('class', 'HeaderSelectable');

    var tHeaderRow = $('<tr></tr>');

    //Creamos la primera columna que sera el header
    var tFixedHeaderCell = $('<th></th>');
    var mainFixedHeaderDiv = $('<div></div>').attr('class', 'ProductiveUnitFixed CalendarFixedHeader');

    var summaryHeaderCell = $('<div></div>').attr('class', 'ProductiveUnitHeaderCellFixed CalendarSummaryFixedHeader');

    var northSummaryCell = $('<div></div>').attr('class', 'NorthCell');
    northSummaryCell.append($('<div></div>').attr('class', 'ProductiveUnitHeaderText').html(Globalize.formatMessage("roPUnitPosition")));

    var southSummaryCell = $('<div></div>').attr('class', 'SouthCell');
    southSummaryCell.append($('<div></div>').attr('class', 'EmpCellTextSouth').html(''));

    summaryHeaderCell.append(northSummaryCell, southSummaryCell);

    var employeeHeaderCell = $('<div></div>').attr('class', 'ProductiveUnitHeaderCellFixed CalendarEmployeeFixedHeader');
    var northCell = $('<div></div>').attr('class', 'NorthCell NorthCellHeaderEmployee');

    northCell.append($('<div></div>').attr('class', 'ProductiveUnitHeaderText').html(Globalize.formatMessage("roPUnitQuantity")));

    var southCell = $('<div></div>').attr('class', 'SouthCell');
    southCell.append($('<div></div>').attr('class', 'EmpCellTextSouth').html(''));

    employeeHeaderCell.append(northCell, southCell);

    tHeaderRow.append(tFixedHeaderCell.append(mainFixedHeaderDiv.append(summaryHeaderCell, employeeHeaderCell)));

    var tmpFirtsDay = moment(oPUnit.firstDate).add(-1, 'days');
    var tmpEndDay = moment(oPUnit.firstDate).add(2, 'days');

    var cIndex = 0;

    while (tmpFirtsDay < tmpEndDay) {
        if (cIndex >= oPUnit.getMinDailyCell() && cIndex < oPUnit.getMaxDailyCell()) {
            var backColor = '#F2F4F2 ';

            //if (moment() >= tmpFirtsDay && moment() < tmpFirtsDay.add(30, 'minutes')) {
            //    backColor = "#44C57E";
            //}
            var dayHeaderCell = $('<th></th>');

            var mainDayHeaderCell = $('<div></div>').attr('id', oPUnit.ascxPrefix + '_calHeaderCell_' + i).attr('class', 'CalendarDayFixed CalendarDayFixedHeader DailyCell').attr('data-IDColumn', i).attr('style', 'background:' + backColor);
            mainDayHeaderCell.append($('<div></div>').attr('class', 'NorthCell' + ' dayInfo DailyCell').html(tmpFirtsDay.format('HH:mm')));
            tHeaderRow.append(dayHeaderCell.append(mainDayHeaderCell));
        }

        tmpFirtsDay = tmpFirtsDay.add(30, 'minutes');
        cIndex++;
    }

    //var tmpCell = $('<th style="width:100%"></th>').html('&nbsp;');
    //tHeaderRow.append(tmpCell);
    tHeader.append(tHeaderRow);

    return tHeader;
};

Robotics.Client.Controls.roProductiveUnitCalendar.prototype.createPUnitTableBody = function (idTable, parentId) {
    var oPUnit = this.oBaseControl;
    var oPUnitData = oPUnit.oCalendar;

    //Creamos el body
    var tBody = $('<tbody></tbody>').attr('id', oPUnit.prefix + idTable + '_tbody').attr('class', 'bodySelectable');

    var lastGroup = -1, actualGroup = -1;

    if (oPUnitData != null && oPUnitData.length > 0) {
        for (var i = 0; i < oPUnitData.length; i++) {
            //var tBodyRow = $('<tr class="positionRowSeparator"></tr>');
            var tBodyRow = $('<tr></tr>');

            var tFixedBodyCell = $('<td></td>');

            var mainFixedBodyCell = $('<div></div>').attr('class', 'ProductiveUnitFixed CalendarFixedBody');

            var fixedEmployeeBodyCell = $('<div></div>').attr('class', 'ProductiveUnitHeaderCellFixed CalendarSummaryFixedBody');
            fixedEmployeeBodyCell.append(this.generatePositionSection(i, oPUnitData[i]));

            var fixedSummaryBodyCell = $('<div></div>').attr('class', 'ProductiveUnitHeaderCellFixed CalendarEmployeeFixedBody');
            fixedSummaryBodyCell.append(this.generateQuantitySection(i, oPUnitData[i]));

            tBodyRow.append(tFixedBodyCell.append(mainFixedBodyCell.append(fixedEmployeeBodyCell, fixedSummaryBodyCell)));

            this.descriptionExists = false;
            this.buildPUnitModePostionRow(i, tBodyRow);

            tmpCell = $('<td style="width:100%"></td>').html('&nbsp;');
            tBodyRow.append(tmpCell);

            tBody.append(tBodyRow);
        }
    } else {
        var tBodyRow = $('<tr></tr>');
        var tFixedBodyCell = $('<td></td>');
        var mainFixedBodyCell = $('<div></div>').attr('class', 'ProductiveUnitFixed CalendarFixedBody');
        var fixedEmployeeBodyCell = $('<div></div>').attr('class', 'ProductiveUnitHeaderCellFixed CalendarSummaryFixedBody');
        var fixedSummaryBodyCell = $('<div></div>').attr('class', 'ProductiveUnitHeaderCellFixed CalendarEmployeeFixedBody');

        tBodyRow.append(tFixedBodyCell.append(mainFixedBodyCell.append(fixedEmployeeBodyCell, fixedSummaryBodyCell)));

        for (var i = oPUnit.getMinDailyCell(); i < oPUnit.getMaxDailyCell(); i++) {
            var calendarCell = $('<td></td>');
            var calendarOuterContent = $('<div></div>').attr('class', 'calendarOuterBodyCellInOtherDepartment DailyCell');
            tBodyRow.append(calendarCell.append(calendarOuterContent));
        }

        //tmpCell = $('<td style="width:100%"></td>').html('&nbsp;');
        //tBodyRow.append(tmpCell);

        tBody.append(tBodyRow);
    }
    return tBody;
};

Robotics.Client.Controls.roProductiveUnitCalendar.prototype.generatePositionSection = function (rowIndex, oUnitMode) {
    var oPUnit = this.oBaseControl;
    var oPUnitData = oPUnit.oCalendar;

    var positionDiv = $('<div></div>').attr('id', oPUnit.ascxPrefix + '_calPositionCell_' + rowIndex).attr('class', 'productiveUnit-position');
    positionDiv.attr('data-IDRow', rowIndex).html((rowIndex + 1));

    return positionDiv;
}

Robotics.Client.Controls.roProductiveUnitCalendar.prototype.generateQuantitySection = function (rowIndex, oUnitMode) {
    var oPUnit = this.oBaseControl;
    var oPUnitData = oPUnit.oCalendar;

    var quantityDiv = $('<div></div>').attr('id', oPUnit.ascxPrefix + '_calPositionCell_' + rowIndex).attr('data-IDRow', rowIndex);

    var firstLine = $('<div></div>').attr('class', 'qtyPosition');
    var txtquantityDiv = $('<input>').attr({ 'type': 'text', 'id': oPUnit.ascxPrefix + '_qty_' + rowIndex, 'data-IDRow': rowIndex, 'class': 'productiveUnit-quantity' });
    $(txtquantityDiv).val(oUnitMode.Quantity);
    firstLine.append(txtquantityDiv);

    var secondLine = $('<div></div>').attr('class', 'lblExpandable');
    var expandableLbl = $('<label>').attr({ 'class': 'productiveUnit-quantity-text' }).html(Globalize.formatMessage("roPUnitExpandable"));
    secondLine.append(expandableLbl);

    var thirdLine = $('<div></div>').attr('class', 'checkExpandable');
    var expandableDiv = $('<input>').attr({ 'type': 'checkbox', 'id': oPUnit.ascxPrefix + '_expand_' + rowIndex, 'data-IDRow': rowIndex, 'class': 'productiveUnit-expand' });
    $(expandableDiv).prop('checked', oUnitMode.IsExpandable);
    thirdLine.append(expandableDiv);

    quantityDiv.append(firstLine, secondLine, thirdLine);

    return quantityDiv;
}

Robotics.Client.Controls.roProductiveUnitCalendar.prototype.buildPUnitModePostionRow = function (rowIndex, tBodyRow) {
    var oPUnit = this.oBaseControl;
    var oPUnitData = oPUnit.oCalendar;

    var dayData = oPUnitData[rowIndex].ShiftHourData;

    for (var columnIndex = oPUnit.getMinDailyCell(); columnIndex < oPUnit.getMaxDailyCell(); columnIndex++) {
        //for (var columnIndex = 0; columnIndex < oCalData.CalendarHeader.PeriodHeaderData.length; columnIndex++) {
        tBodyRow.append(this.createDailyCalendarCell(oPUnitData[rowIndex], dayData[columnIndex], rowIndex, columnIndex));
    }
}

Robotics.Client.Controls.roProductiveUnitCalendar.prototype.createDailyCalendarCell = function (positionData, cellInfo, rowPosition, columnPosition) {
    var oPUnit = this.oBaseControl;
    var oPUnitData = oPUnit.oCalendar;

    var calendarCell = $('<td></td>');

    if (columnPosition % 2 == 0) {
        calendarCell.addClass('columnDailyCalendarOdd');
    }

    var calendarOuterContent = $('<div></div>').attr('id', oPUnit.ascxPrefix + '_calDailyCell_' + rowPosition + '_' + columnPosition).attr('class', 'calendarDailyBodyCell DailyCell');
    calendarOuterContent.attr('data-IDRow', rowPosition).attr('data-IDColumn', columnPosition);

    this.createDailyCalendarCellContent(calendarOuterContent, positionData, cellInfo, columnPosition);

    return calendarCell.append(calendarOuterContent);
};

Robotics.Client.Controls.roProductiveUnitCalendar.prototype.createDailyCalendarCellContent = function (containter, positionData, cellInfo, columnPosition) {
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

        switch (cellInfo.DailyHourType) {
            case Robotics.Client.Constants.DailyHourType.Complementary:
                startColor = new Robotics.Client.Common.roHtmlColor().shadeBlendConvert(-0.5, startColor);
                break;
        }

        if ((oPUnit.firstCellPrinted == -1 && cellInfo.DailyHourType != Robotics.Client.Constants.DailyHourType.Untyped) || columnPosition < oPUnit.firstCellPrinted) {
            oPUnit.firstCellPrinted = columnPosition;
        }

        calendarInnerContent.attr('style', 'background: ' + startColor + ';' + assignmentStyle);

        if (!this.descriptionExists && ((columnPosition == 0) || (cellInfo.DailyHourType != positionData.ShiftHourData[columnPosition - 1].DailyHourType))) {
            this.descriptionExists = true;
            changeShift = this.generateShiftTypeInfoDailyContainer(shiftName + assignmentName, startColor, 0, marginAssignment);
        }
    }

    containter.append(nameShift, calendarInnerContent, changeShift);
};

Robotics.Client.Controls.roProductiveUnitCalendar.prototype.generateShiftInfoDailyContainer = function (shiftName, color) {
    var oPUnit = this.oBaseControl;
    var oPUnitData = oPUnit.oCalendar;

    var shiftInfoContainer = $('<div></div>').attr('class', 'calendarShiftInfoDailyContainer').attr('title', shiftName).append($("<span class='fontBold calendarFeastLeft'></span>").html(shiftName));

    shiftInfoContainer.attr('style', 'color:' + new Robotics.Client.Common.roHtmlColor().invertCssColor(color));

    return shiftInfoContainer;
};

Robotics.Client.Controls.roProductiveUnitCalendar.prototype.generateShiftTypeInfoDailyContainer = function (assignmentText, color, hours, marginAssignment) {
    var oPUnit = this.oBaseControl;
    var oPUnitData = oPUnit.oCalendar;

    var cellText = assignmentText;
    var shiftInfoContainer = $('<div></div>').attr('class', 'calendarShiftTypeInfoDailyContainer').attr('title', cellText).html(cellText);

    shiftInfoContainer.attr('style', 'color:' + new Robotics.Client.Common.roHtmlColor().invertCssColor(color) + ';' + marginAssignment);

    return shiftInfoContainer;
};

Robotics.Client.Controls.roProductiveUnitCalendar.prototype.endCallback = function (action, objResult, objResultParams) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    switch (action) {
        case Robotics.Client.Constants.Actions.ShiftLayerDefinition:
            this.shiftLayerDefinitionResponse(objResult, objResultParams);
            break;
        case Robotics.Client.Constants.Actions.ShiftLayerDefinitionEdit:
            this.shiftLayerDefinitionEditResponse(objResult, objResultParams);
            break;
        case Robotics.Client.Constants.Actions.PositionModeDayData:
            this.shiftLayerDefinitionAddPosition(objResult, objResultParams);
            break;
        case Robotics.Client.Constants.Actions.UpdateCurrentDayData:
            this.updateCurrentDayDataHours(objResult, objResultParams);
            break;
    }

    this.loadingFunctionExtended(false);
};

Robotics.Client.Controls.roProductiveUnitCalendar.prototype.shiftLayerDefinitionResponse = function (objResult, objResultParams) {
    var oPUnit = this.oBaseControl;

    if (oPUnit.showComplementaryAssignDialog) oPUnit.prepareComplementaryDialog(objResult);
    else if (oPUnit.showAssignmentsDialog) oPUnit.prepareAssignmentsDialog(objResult);
}

Robotics.Client.Controls.roProductiveUnitCalendar.prototype.shiftLayerDefinitionEditResponse = function (objResult, objResultParams) {
    var oPUnit = this.oBaseControl;

    if (oPUnit.showComplementaryAssignDialog) oPUnit.editComplementaryInfoFinally(objResult);
    else if (oPUnit.showAssignmentsDialog) oPUnit.editAssignmentsInfoFinally(objResult);
}

Robotics.Client.Controls.roProductiveUnitCalendar.prototype.prepareShfitsDialog = function (objResult) {
    var oPUnit = this.oBaseControl;

    oPUnit.shiftsDialog.dialog("open");
};

Robotics.Client.Controls.roProductiveUnitCalendar.prototype.onAcceptShiftsDialog = function () {
    var oPUnit = this.oBaseControl;

    var oSelectedShift = oPUnit.shiftSelectorManager.getSelectedItem();
    if (oSelectedShift == null) {
        oPUnit.showErrorPopup("Error.Title", "error", "Calendar.Client.NoShiftSelected", "", "Error.OK", "Error.OKDesc", oPUnit.defaultMessageAction);
    } else {
        oPUnit.showAssignmentsDialog = false;
        if (oSelectedShift.AllowAssignments == 1) oPUnit.showAssignmentsDialog = true;

        oPUnit.showComplementaryAssignDialog = false;
        if (oSelectedShift.AllowComplementary == 1 || oSelectedShift.AllowFloatingData == 1) oPUnit.showComplementaryAssignDialog = true;

        var ShiftTypeVal = Robotics.Client.Constants.ShiftType.Normal;

        switch (oSelectedShift.ShiftType) {
            case 0:
                ShiftTypeVal = Robotics.Client.Constants.ShiftType.Normal;
                break;
            case 1:
                oPUnit.showComplementaryAssignDialog = true;
                ShiftTypeVal = Robotics.Client.Constants.ShiftType.NormalFloating;
                break;
            case 2:
                ShiftTypeVal = Robotics.Client.Constants.ShiftType.Holiday;
                break;
            case 3:
                ShiftTypeVal = Robotics.Client.Constants.ShiftType.Holiday_NonWorking;
                break;
        }

        var mainShift = {
            ID: oSelectedShift.Id,
            ShortName: oSelectedShift.ShortName,
            PlannedHours: oSelectedShift.ShiftHours,
            Color: oSelectedShift.ShiftColor,
            Name: oSelectedShift.Name,
            Type: ShiftTypeVal,
            StartHour: moment(oSelectedShift.StartHour, "YYYY/MM/DD HH:mm").toDate()
        };

        if (oPUnit.showComplementaryAssignDialog || oPUnit.showAssignmentsDialog) {
            if (oPUnit.showComplementaryAssignDialog) oPUnit.complementaryShift = mainShift;
            if (oPUnit.showAssignmentsDialog) oPUnit.assignmentShift = mainShift;

            if (typeof (oPUnit.shiftsExtendedDataCache[mainShift.ID]) == 'undefined') {
                var oParameters = {};
                oParameters.idShift = mainShift.ID;
                oParameters.StampParam = new Date().getMilliseconds();
                oPUnit.performAction(oParameters, Robotics.Client.Constants.Actions.ShiftLayerDefinition);
            } else {
                if (oPUnit.showComplementaryAssignDialog) oPUnit.prepareComplementaryDialog(null);
                else if (oPUnit.showAssignmentsDialog) oPUnit.prepareAssignmentsDialog(null);
            }
        } else {
            oPUnit.complementaryShift = null;
            oPUnit.assignmentShift = null;
            oPUnit.showComplementaryAssignDialog = false;
            oPUnit.showAssignmentsDialog = false;
            oPUnit.showErrorPopup("Error.Title", "error", "Calendar.Client.ShiftWithNoAssignmentsSelected", "", "Error.OK", "Error.OKDesc", oPUnit.defaultMessageAction);
        }

        oPUnit.shiftsDialog.dialog("close");
    }
};

Robotics.Client.Controls.roProductiveUnitCalendar.prototype.prepareComplementaryDialog = function (objResult) {
    var oPUnit = this.oBaseControl;

    if (objResult != null) {
        oPUnit.shiftsExtendedDataCache[oPUnit.complementaryShift.ID] = objResult;
    }

    oPUnit.complementaryManager.prepareComplementaryDialog(oPUnit.shiftsExtendedDataCache[oPUnit.complementaryShift.ID], -1);
    oPUnit.complementaryDialog.dialog("open");
};

Robotics.Client.Controls.roProductiveUnitCalendar.prototype.onAcceptComplementaryDialog = function () {
    var oPUnit = this.oBaseControl;
    var oPUnitData = oPUnit.oCalendar;

    if (oPUnit.complementaryManager.isValid()) {
        oPUnit.showComplementaryAssignDialog = false;
        var mainShift = oPUnit.complementaryManager.getDayData();

        if (this.isActionOnRow && !oPUnit.showAssignmentsDialog) {
            for (var i = 0; i < oPUnitData.length; i++) {
                if (oPUnitData[i].ID == oPUnit.selectedEmployee.ID) {
                    oPUnitData[i].ShiftData = Object.clone(mainShift);

                    oPUnit.complementaryShift = null;
                    oPUnit.assignmentShift = null;
                    oPUnit.showComplementaryAssignDialog = false;
                    oPUnit.showAssignmentsDialog = false;
                    this.isActionOnRow = false;

                    //oPUnit.refreshTables(null, false, true);
                    //oPUnit.complementaryDialog.dialog("close");

                    var oParameters = {};
                    oParameters.idShift = mainShift.ID;
                    oParameters.StampParam = new Date().getMilliseconds();
                    oParameters.endDate = mainShift.StartHour;
                    oParameters.shiftData = mainShift;
                    oParameters.dailyPeriod = 30;
                    oPUnit.performAction(oParameters, Robotics.Client.Constants.Actions.UpdateCurrentDayData);

                    break;
                }
            }
        } else {
            if (!oPUnit.showAssignmentsDialog) {
                oPUnit.showComplementaryAssignDialog = false;
                oPUnit.showAssignmentsDialog = false;
                oPUnit.complementaryShift = null;
                oPUnit.assignmentShift = null;
                oPUnit.showErrorPopup("Error.Title", "error", "Calendar.Client.ShiftWithNoAssignmentsSelected", "", "Error.OK", "Error.OKDesc", oPUnit.defaultMessageAction);
            } else {
                oPUnit.complementaryDialog.dialog("close");
                oPUnit.assignmentShift = mainShift;

                this.prepareAssignmentsDialog(null);
            }
        }
    }
};
Robotics.Client.Controls.roProductiveUnitCalendar.prototype.updateCurrentDayDataHours = function (objResult) {
    var oPUnit = this.oBaseControl;
    var oPUnitData = oPUnit.oCalendar;

    for (var i = 0; i < oPUnitData.length; i++) {
        if (oPUnitData[i].ID == oPUnit.selectedEmployee.ID) {
            oPUnitData[i].ShiftHourData = Object.clone(objResult, true);

            oPUnit.refreshTables(null, false, true);
            oPUnit.complementaryDialog.dialog("close");
            oPUnit.assignmentsDialog.dialog("close");
        }
    }
}

Robotics.Client.Controls.roProductiveUnitCalendar.prototype.prepareAssignmentsDialog = function (objResult) {
    var oPUnit = this.oBaseControl;

    if (objResult != null) {
        oPUnit.shiftsExtendedDataCache[oPUnit.assignmentShift.ID] = objResult;
    }

    oPUnit.assignmentsManager.prepareAssignmentsDialog(oPUnit.shiftsExtendedDataCache[oPUnit.assignmentShift.ID], null, -2, oPUnit.shiftSelectorManager.getAssignmentsDefinition());

    oPUnit.assignmentsDialog.dialog("open");

    setTimeout(function () { oPUnit.assignmentsManager.focusDialog(); }, 100);
};

Robotics.Client.Controls.roProductiveUnitCalendar.prototype.onAcceptAssignmentsDialog = function () {
    var oPUnit = this.oBaseControl;
    var oPUnitData = oPUnit.oCalendar;

    if (oPUnit.assignmentsManager.isValid()) {
        if (this.isActionOnRow) {
            var assigData = oPUnit.assignmentsManager.getDayData();

            for (var i = 0; i < oPUnitData.length; i++) {
                if (oPUnitData[i].ID == oPUnit.selectedEmployee.ID) {
                    if (oPUnit.assignmentShift != null) oPUnitData[i].ShiftData = Object.clone(oPUnit.assignmentShift);
                    oPUnitData[i].AssignmentData = Object.clone(assigData);

                    oPUnit.complementaryShift = null;
                    oPUnit.assignmentShift = null;
                    oPUnit.showComplementaryAssignDialog = false;
                    oPUnit.showAssignmentsDialog = false;
                    this.isActionOnRow = false;

                    var oParameters = {};
                    oParameters.idShift = oPUnitData[i].ShiftData.ID;
                    oParameters.StampParam = new Date().getMilliseconds();
                    oParameters.endDate = oPUnitData[i].ShiftData.StartHour
                    oParameters.shiftData = oPUnitData[i].ShiftData;
                    oPUnit.performAction(oParameters, Robotics.Client.Constants.Actions.UpdateCurrentDayData);

                    break;
                }
            }
        } else {
            var oParameters = {};
            oParameters.idShift = oPUnit.assignmentShift.ID;
            oParameters.StampParam = new Date().getMilliseconds();
            oParameters.endDate = oPUnit.assignmentShift.StartHour;
            oParameters.shiftData = oPUnit.assignmentShift;
            oParameters.dailyPeriod = 30;
            oPUnit.performAction(oParameters, Robotics.Client.Constants.Actions.PositionModeDayData);

            oPUnit.assignmentsDialog.dialog("close");

            oPUnit.showAssignmentsDialog = false;
        }
    } else {
        oPUnit.showComplementaryAssignDialog = false;
        oPUnit.showAssignmentsDialog = false;
        oPUnit.complementaryShift = null;
        oPUnit.assignmentShift = null;
        oPUnit.showErrorPopup("Error.Title", "error", "Calendar.Client.ShiftWithNoAssignmentsSelected", "", "Error.OK", "Error.OKDesc", oPUnit.defaultMessageAction);
    }
}

Robotics.Client.Controls.roProductiveUnitCalendar.prototype.shiftLayerDefinitionAddPosition = function (objResult, objResultParams) {
    var oPUnit = this.oBaseControl;
    var oPUnitData = oPUnit.oCalendar;

    if (oPUnit.assignmentShift != null) {
        var assigData = oPUnit.assignmentsManager.getDayData();

        var actualID = 0;
        for (var i = 0; i < oPUnitData.length; i++) {
            if (oPUnitData[i].ID < actualID) {
                actualID = oPUnitData[i].ID;
            }
        }
        actualID = actualID - 1;

        var oNewPosition = {
            ID: actualID,
            IDProductiveUnitMode: -1,
            Quantity: 1,
            IsExpandable: true,
            ShiftData: Object.clone(oPUnit.assignmentShift, true),
            AssignmentData: assigData,
            ShiftHourData: Object.clone(objResult, true),
            EmployeesData: null,
            Coverage: 0
        };
        oPUnitData.push(oNewPosition);

        oPUnit.complementaryShift = null;
        oPUnit.assignmentShift = null;
        oPUnit.showComplementaryAssignDialog = false;
        oPUnit.showAssignmentsDialog = false;

        oPUnit.refreshTables(null, false, true);
    } else {
        oPUnit.showComplementaryAssignDialog = false;
        oPUnit.showAssignmentsDialog = false;
        oPUnit.complementaryShift = null;
        oPUnit.assignmentShift = null;
        oPUnit.showErrorPopup("Error.Title", "error", "Calendar.Client.ShiftWithNoAssignmentsSelected", "", "Error.OK", "Error.OKDesc", oPUnit.defaultMessageAction);
    }
};