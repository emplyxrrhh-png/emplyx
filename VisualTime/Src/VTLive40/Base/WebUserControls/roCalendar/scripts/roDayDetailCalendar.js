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

Robotics.Client.Controls.roDayDetailCalendar = function (baseControl) {
    this.name = "Robotics.Client.Controls.roDayDetailCalendar";

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

    this.selectedPUnit = null;
    this.selectedPUnitMode = null;
}

Robotics.Client.Controls.roDayDetailCalendar.prototype.getLayout = function () {
    return this.pageLayoutOptions;
}

Robotics.Client.Controls.roDayDetailCalendar.prototype.setHasChanges = function (bolHasChanges) {
    var oPUnit = this.oBaseControl;

    oPUnit.hasChanges = bolHasChanges;
};

Robotics.Client.Controls.roDayDetailCalendar.prototype.create = function () {
    var oPUnit = this.oBaseControl;
    var oPUnitData = oPUnit.oCalendar;

    this.buildHTMLStructure();

    oPUnit.pageLayout = oPUnit.container.layout(this.pageLayoutOptions);
};

Robotics.Client.Controls.roDayDetailCalendar.prototype.loadData = function (oUnitModePositions, idOrgChart, sDate, pUnit, pUnitMode, loadIndictments) {
    var oPUnit = this.oBaseControl;

    oPUnit.firstDate = moment().startOf('day').toDate();
    oPUnit.endDate = oPUnit.firstDate;
    clearTimeout(oPUnit.refreshTimmer);
    oPUnit.refreshTimmer = -1;
    oPUnit.sortColumn = -1;
    oPUnit.firstDate = sDate;
    oPUnit.employeeFilter = idOrgChart;
    oPUnit.loadIndictments = loadIndictments;
    oPUnit.oCalendar = oUnitModePositions;
    this.selectedPUnit = pUnit;
    this.selectedPUnitMode = pUnitMode;

    oPUnit.initialize();
    oPUnit.setHasChanges(false);

    oPUnit.refreshTables(null, false, true);
};

Robotics.Client.Controls.roDayDetailCalendar.prototype.getPositions = function () {
    var oPUnit = this.oBaseControl;

    return oPUnit.oCalendar;
};

Robotics.Client.Controls.roDayDetailCalendar.prototype.buildHTMLStructure = function () {
    var oPUnit = this.oBaseControl;
    var oPUnitData = oPUnit.oCalendar;

    var mainCenterLayout = $('<div></div>').attr('id', oPUnit.prefix + Robotics.Client.Constants.LayoutNames.Center).attr('class', 'ui-layout-center');

    //Panel para el calendario
    var calendarLayout = $('<div></div>').attr('id', oPUnit.prefix + Robotics.Client.Constants.LayoutNames.Calendar).attr('class', 'ui-layout-center');

    // Añadimos todo al panel central
    mainCenterLayout.append(calendarLayout);

    oPUnit.container.append(mainCenterLayout);
};

Robotics.Client.Controls.roDayDetailCalendar.prototype.onresize = function (oProductiveUnitCalendar) {
    return function () {
        oProductiveUnitCalendar.loadingFunctionExtended(true);
        oProductiveUnitCalendar.oBaseControl.refreshTables(oProductiveUnitCalendar, true, true);
        oProductiveUnitCalendar.loadingFunctionExtended(false);
    };
};

Robotics.Client.Controls.roDayDetailCalendar.prototype.loadingFunctionExtended = function (showLoading) {
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

Robotics.Client.Controls.roDayDetailCalendar.prototype.getContextMenuSelector = function () {
    var selector = '.calendarDailyBodyCell';
    return selector;
}

Robotics.Client.Controls.roDayDetailCalendar.prototype.getContextMenuHeaderSelector = function () {
    return null;
}

Robotics.Client.Controls.roDayDetailCalendar.prototype.buildContextMenu = function (sender) {
    var oPUnit = this.oBaseControl;

    var controlText = sender[0].innerText;

    items = {
        'split1': { name: '---------', disabled: true }
    };

    return items;
};

Robotics.Client.Controls.roDayDetailCalendar.prototype.executeContextMenuAction = function (key, container) {
    var oPUnit = this.oBaseControl;
    var oPUnitData = oPUnit.oCalendar;

    switch (key) {
        default:
            break;
    }
};

Robotics.Client.Controls.roDayDetailCalendar.prototype.processKeyUpEvent = function (e) {
    var oPUnit = this.oBaseControl;
    var oPUnitData = oPUnit.oCalendar;

    //Function requiered for inheritance
}

Robotics.Client.Controls.roDayDetailCalendar.prototype.processKeyDownEvent = function (e) {
    var oPUnit = this.oBaseControl;
    var oPUnitData = oPUnit.oCalendar;

    //Function requiered for inheritance
}

Robotics.Client.Controls.roDayDetailCalendar.prototype.onDrop = function (event) {
    var oPUnit = this.oBaseControl;
    var oPUnitData = oPUnit.oCalendar;

    //Function requiered for inheritance
}

Robotics.Client.Controls.roDayDetailCalendar.prototype.mapModeEvents = function () {
    var oClientMode = this;
    var oPUnit = this.oBaseControl;
    var oPUnitData = oPUnit.oCalendar;

    var selector = '.calendarDailyBodyCell';

    //$(selector + ',.reviewDailyBodyCell').off('click');
    //$(selector + ',.reviewDailyBodyCell').on('click', function (e) {
    //    oClientMode.setSingleSelectedObject(this);
    //});

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

Robotics.Client.Controls.roDayDetailCalendar.prototype.setSingleSelectedObject = function (sender) {
    var oPUnit = this.oBaseControl;
    var oPUnitData = oPUnit.oCalendar;

    if (sender == null) {
        oPUnit.selectedEmployee = null;
        oPUnit.selectedContainer = null;
    } else {
        oPUnit.selectedEmployee = oPUnitData[parseInt($(sender).attr('data-IDRow'), 10)];
    }
};

Robotics.Client.Controls.roDayDetailCalendar.prototype.refreshModeTables = function (objectRef, isResizing, refreshCalendar) {
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

Robotics.Client.Controls.roDayDetailCalendar.prototype.refreshMainTable = function (isResizing) {
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

Robotics.Client.Controls.roDayDetailCalendar.prototype.createResumeTables = function (idTable, parentId) {
    switch (idTable) {
        case Robotics.Client.Constants.TableNames.Calendar:
            this.createPUnitTable(idTable, parentId);
            break;
    }
};

Robotics.Client.Controls.roDayDetailCalendar.prototype.createPUnitTable = function (idTable, parentId) {
    var oPUnit = this.oBaseControl;

    var tableContainer = $('#' + oPUnit.prefix + parentId);
    tableContainer.empty();

    var tableElement = $('<table></table>').attr('id', oPUnit.prefix + idTable).attr('class', 'fancyTable');

    tableElement.append(this.createPUnitTableHeader(idTable, parentId));

    tableElement.append(this.createPUnitTableBody(idTable, parentId));

    tableContainer.append(tableElement);
};

Robotics.Client.Controls.roDayDetailCalendar.prototype.createPUnitTableHeader = function (idTable, parentId) {
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

Robotics.Client.Controls.roDayDetailCalendar.prototype.createPUnitTableBody = function (idTable, parentId) {
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

Robotics.Client.Controls.roDayDetailCalendar.prototype.generatePositionSection = function (rowIndex, oUnitMode) {
    var oPUnit = this.oBaseControl;
    var oPUnitData = oPUnit.oCalendar;

    var positionDiv = $('<div></div>').attr('id', oPUnit.ascxPrefix + '_calPositionCell_' + rowIndex).attr('class', 'productiveUnit-position');
    positionDiv.attr('data-IDRow', rowIndex).html((rowIndex + 1));

    return positionDiv;
}

Robotics.Client.Controls.roDayDetailCalendar.prototype.generateQuantitySection = function (rowIndex, oUnitMode) {
    var oPUnit = this.oBaseControl;
    var oPUnitData = oPUnit.oCalendar;

    var quantityDiv = $('<div></div>').attr('id', oPUnit.ascxPrefix + '_calPositionCell_' + rowIndex).attr('data-IDRow', rowIndex);

    var firstLine = $('<div></div>').attr('class', 'qtyPosition');
    var txtquantityDiv = $('<input>').attr({ 'type': 'text', 'id': oPUnit.ascxPrefix + '_qty_' + rowIndex, 'data-IDRow': rowIndex, 'class': 'productiveUnit-quantity', disabled: 'true' });
    $(txtquantityDiv).val(oUnitMode.Quantity);
    firstLine.append(txtquantityDiv);

    var secondLine = $('<div></div>').attr('class', 'lblExpandable');
    var expandableLbl = $('<label>').attr({ 'class': 'productiveUnit-quantity-text' }).html(Globalize.formatMessage("roPUnitExpandable"));
    secondLine.append(expandableLbl);

    var thirdLine = $('<div></div>').attr('class', 'checkExpandable');
    var expandableDiv = $('<input>').attr({ 'type': 'checkbox', 'id': oPUnit.ascxPrefix + '_expand_' + rowIndex, 'data-IDRow': rowIndex, 'class': 'productiveUnit-expand', disabled: 'true' });
    $(expandableDiv).prop('checked', oUnitMode.IsExpandable);
    thirdLine.append(expandableDiv);

    quantityDiv.append(firstLine, secondLine, thirdLine);

    return quantityDiv;
}

Robotics.Client.Controls.roDayDetailCalendar.prototype.buildPUnitModePostionRow = function (rowIndex, tBodyRow) {
    var oPUnit = this.oBaseControl;
    var oPUnitData = oPUnit.oCalendar;

    var dayData = oPUnitData[rowIndex].ShiftHourData;

    for (var columnIndex = oPUnit.getMinDailyCell(); columnIndex < oPUnit.getMaxDailyCell(); columnIndex++) {
        //for (var columnIndex = 0; columnIndex < oCalData.CalendarHeader.PeriodHeaderData.length; columnIndex++) {
        tBodyRow.append(this.createDailyCalendarCell(oPUnitData[rowIndex], dayData[columnIndex], rowIndex, columnIndex));
    }
}

Robotics.Client.Controls.roDayDetailCalendar.prototype.createDailyCalendarCell = function (positionData, cellInfo, rowPosition, columnPosition) {
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

Robotics.Client.Controls.roDayDetailCalendar.prototype.createDailyCalendarCellContent = function (containter, positionData, cellInfo, columnPosition) {
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

Robotics.Client.Controls.roDayDetailCalendar.prototype.generateShiftInfoDailyContainer = function (shiftName, color) {
    var oPUnit = this.oBaseControl;
    var oPUnitData = oPUnit.oCalendar;

    var shiftInfoContainer = $('<div></div>').attr('class', 'calendarShiftInfoDailyContainer').attr('title', shiftName).append($("<span class='fontBold calendarFeastLeft'></span>").html(shiftName));

    shiftInfoContainer.attr('style', 'color:' + new Robotics.Client.Common.roHtmlColor().invertCssColor(color));

    return shiftInfoContainer;
};

Robotics.Client.Controls.roDayDetailCalendar.prototype.generateShiftTypeInfoDailyContainer = function (assignmentText, color, hours, marginAssignment) {
    var oPUnit = this.oBaseControl;
    var oPUnitData = oPUnit.oCalendar;

    var cellText = assignmentText;
    var shiftInfoContainer = $('<div></div>').attr('class', 'calendarShiftTypeInfoDailyContainer').attr('title', cellText).html(cellText);

    shiftInfoContainer.attr('style', 'color:' + new Robotics.Client.Common.roHtmlColor().invertCssColor(color) + ';' + marginAssignment);

    return shiftInfoContainer;
};

Robotics.Client.Controls.roDayDetailCalendar.prototype.endCallback = function (action, objResult, objResultParams) {
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    switch (action) {
        default:
            break;
    }

    this.loadingFunctionExtended(false);
};

Robotics.Client.Controls.roDayDetailCalendar.prototype.prepareShfitsDialog = function (objResult) {
};

Robotics.Client.Controls.roDayDetailCalendar.prototype.onAcceptShiftsDialog = function () {
};

Robotics.Client.Controls.roDayDetailCalendar.prototype.prepareComplementaryDialog = function (objResult) {
};

Robotics.Client.Controls.roDayDetailCalendar.prototype.onAcceptComplementaryDialog = function () {
};

Robotics.Client.Controls.roDayDetailCalendar.prototype.prepareAssignmentsDialog = function (objResult) {
};

Robotics.Client.Controls.roDayDetailCalendar.prototype.onAcceptAssignmentsDialog = function () {
};
