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
    namespace("Robotics.Client.Global");
}());

Robotics.Client.Global.deferredItemData = { oCalendar: null, d: null, itemData: null, idRow: -1 };
Robotics.Client.Global.rowContext = { id: -1, oObject: null, quantity: 0 };

Robotics.Client.Controls.roDayDetailSchedule = function (baseControl) {
    this.name = "Robotics.Client.Controls.roDayDetailSchedule";

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

    this.employeeLists = {};
}

Robotics.Client.Controls.roDayDetailSchedule.prototype.getLayout = function () {
    return this.pageLayoutOptions;
}

Robotics.Client.Controls.roDayDetailSchedule.prototype.setHasChanges = function (bolHasChanges) {
    var oPUnit = this.oBaseControl;

    oPUnit.hasChanges = bolHasChanges;
};

Robotics.Client.Controls.roDayDetailSchedule.prototype.create = function () {
    var oPUnit = this.oBaseControl;
    var oPUnitData = oPUnit.oCalendar;

    this.buildHTMLStructure();

    oPUnit.pageLayout = oPUnit.container.layout(this.pageLayoutOptions);
};

Robotics.Client.Controls.roDayDetailSchedule.prototype.loadData = function (oUnitModePositions, idOrgChart, sDate, pUnit, pUnitMode, loadIndictments) {
    var oPUnit = this.oBaseControl;

    oPUnit.firstDate = moment().startOf('day').toDate();
    oPUnit.endDate = oPUnit.firstDate;
    clearTimeout(oPUnit.refreshTimmer);
    oPUnit.refreshTimmer = -1;
    oPUnit.sortColumn = -1;
    oPUnit.firstDate = sDate;
    oPUnit.loadIndictments = loadIndictments;
    oPUnit.employeeFilter = idOrgChart;
    oPUnit.oCalendar = oUnitModePositions;
    this.selectedPUnit = pUnit;
    this.selectedPUnitMode = pUnitMode;

    oPUnit.initialize();
    oPUnit.setHasChanges(false);

    oPUnit.refreshTables(null, false, true);
};

Robotics.Client.Controls.roDayDetailSchedule.prototype.getPositions = function () {
    var oPUnit = this.oBaseControl;

    return oPUnit.oCalendar;
};

Robotics.Client.Controls.roDayDetailSchedule.prototype.buildHTMLStructure = function () {
    var oPUnit = this.oBaseControl;
    var oPUnitData = oPUnit.oCalendar;

    var mainCenterLayout = $('<div></div>').attr('id', oPUnit.prefix + Robotics.Client.Constants.LayoutNames.Center).attr('class', 'ui-layout-center');

    //Panel para el calendario
    var calendarLayout = $('<div></div>').attr('id', oPUnit.prefix + Robotics.Client.Constants.LayoutNames.Calendar).attr('class', 'ui-layout-center');

    // Añadimos todo al panel central
    mainCenterLayout.append(calendarLayout);

    oPUnit.container.append(mainCenterLayout);
};

Robotics.Client.Controls.roDayDetailSchedule.prototype.onresize = function (oProductiveUnitCalendar) {
    return function () {
        oProductiveUnitCalendar.loadingFunctionExtended(true);
        oProductiveUnitCalendar.oBaseControl.refreshTables(oProductiveUnitCalendar, true, true);
        oProductiveUnitCalendar.loadingFunctionExtended(false);
    };
};

Robotics.Client.Controls.roDayDetailSchedule.prototype.loadingFunctionExtended = function (showLoading) {
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

Robotics.Client.Controls.roDayDetailSchedule.prototype.getContextMenuSelector = function () {
    var selector = '.calendarDailyBodyCell';
    return selector;
}

Robotics.Client.Controls.roDayDetailSchedule.prototype.getContextMenuHeaderSelector = function () {
    return null;
}

Robotics.Client.Controls.roDayDetailSchedule.prototype.buildContextMenu = function (sender) {
    var oPUnit = this.oBaseControl;

    var controlText = sender[0].innerText;

    items = {
        'split1': { name: '---------', disabled: true }
    };

    return items;
};

Robotics.Client.Controls.roDayDetailSchedule.prototype.executeContextMenuAction = function (key, container) {
    var oPUnit = this.oBaseControl;
    var oPUnitData = oPUnit.oCalendar;

    switch (key) {
        default:
            break;
    }
};

Robotics.Client.Controls.roDayDetailSchedule.prototype.processKeyUpEvent = function (e) {
    var oPUnit = this.oBaseControl;
    var oPUnitData = oPUnit.oCalendar;

    //Function requiered for inheritance
}

Robotics.Client.Controls.roDayDetailSchedule.prototype.processKeyDownEvent = function (e) {
    var oPUnit = this.oBaseControl;
    var oPUnitData = oPUnit.oCalendar;

    //Function requiered for inheritance
}

Robotics.Client.Controls.roDayDetailSchedule.prototype.onDrop = function (event) {
    var oPUnit = this.oBaseControl;
    var oPUnitData = oPUnit.oCalendar;

    //Function requiered for inheritance
}

Robotics.Client.Controls.roDayDetailSchedule.prototype.onUpdateRowQuantity = function (idRow, oNode, newValue, srcElement) {
    var oClientMode = this;
    var oPUnit = this.oBaseControl;
    var oPUnitData = oPUnit.oCalendar;

    var currentEmployeesAssigned = oNode.EmployeesData.count(function (elem) { return elem.IDEmployee != -1 })

    if (oNode.IsExpandable == false && currentEmployeesAssigned > newValue) {
        DevExpress.ui.dialog.alert(Globalize.formatMessage('roEmployeesExceedQuantity'), Globalize.formatMessage('roAlert'));
        srcElement.val(currentEmployeesAssigned);
    } else {
        if (newValue <= 0) {
            DevExpress.ui.dialog.alert(Globalize.formatMessage('roEmployeesGreaterThanZero'), Globalize.formatMessage('roAlert'));
            srcElement.val(oNode.Quantity);
        } else {
            if (oPUnitData.length > 0 && idRow >= 0) {
                Robotics.Client.Global.rowContext.id = idRow;
                Robotics.Client.Global.rowContext.quantity = newValue;

                var oParameters = {};
                oParameters.idOrgChartNode = parseInt(oPUnit.employeeFilter, 10);
                oParameters.pUnitModePosition = oPUnitData[idRow];
                oParameters.sDate = oPUnit.firstDate;
                oParameters.quantity = newValue;
                oParameters.iPUnit = oClientMode.selectedPUnit.ID;
                oParameters.iPUnitMode = oClientMode.selectedPUnitMode.ID;
                oParameters.StampParam = new Date().getMilliseconds();

                oPUnit.performAction(oParameters, Robotics.Client.Constants.Actions.UpdateProductiveUnitQuantity);
            } else {
                Robotics.Client.Global.rowContext.id = -1;
            }
        }
    }
}

Robotics.Client.Controls.roDayDetailSchedule.prototype.onFulFillPositionEmployees = function (idRow) {
    var oClientMode = this;
    var oPUnit = this.oBaseControl;
    var oPUnitData = oPUnit.oCalendar;

    if (oPUnitData.length > 0 && idRow >= 0) {
        Robotics.Client.Global.rowContext.id = idRow;

        var oParameters = {};
        oParameters.idOrgChartNode = parseInt(oPUnit.employeeFilter, 10);
        oParameters.pUnitModePosition = oPUnitData[idRow];
        oParameters.sDate = oPUnit.firstDate;
        oParameters.StampParam = new Date().getMilliseconds();

        oPUnit.performAction(oParameters, Robotics.Client.Constants.Actions.GetAvailablePositionEmployeesForFulFill);
    } else {
        Robotics.Client.Global.rowContext.id = -1;
    }
}

Robotics.Client.Controls.roDayDetailSchedule.prototype.onAddPositionEmployee = function (idRow) {
    var oClientMode = this;
    var oPUnit = this.oBaseControl;
    var oPUnitData = oPUnit.oCalendar;

    if (oPUnitData.length > 0 && idRow >= 0) {
        Robotics.Client.Global.rowContext.id = idRow;

        var oParameters = {};
        oParameters.idOrgChartNode = parseInt(oPUnit.employeeFilter, 10);
        oParameters.pUnitModePosition = oPUnitData[idRow];
        oParameters.sDate = oPUnit.firstDate;
        oParameters.StampParam = new Date().getMilliseconds();

        oPUnit.performAction(oParameters, Robotics.Client.Constants.Actions.GetAvailablePositionEmployees);
    } else {
        Robotics.Client.Global.rowContext.id = -1;
    }
}

Robotics.Client.Controls.roDayDetailSchedule.prototype.mapModeEvents = function () {
    var oClientMode = this;
    var oPUnit = this.oBaseControl;
    var oPUnitData = oPUnit.oCalendar;

    var selector = '.calendarDailyBodyCell';

    $(selector + ',.fulfillEmployees').off('click');
    $(selector + ',.fulfillEmployees').on('click', function (event) {
        var srcElement = null;
        if (typeof (event.srcElement) != 'undefined') {
            srcElement = $(event.srcElement);
        } else {
            srcElement = $(event.originalEvent.target);
        }

        oClientMode.onFulFillPositionEmployees(parseInt(srcElement.attr("data-IDRow"), 10));
    });

    $(selector + ',.addEmployee').off('click');
    $(selector + ',.addEmployee').on('click', function (event) {
        var srcElement = null;
        if (typeof (event.srcElement) != 'undefined') {
            srcElement = $(event.srcElement);
        } else {
            srcElement = $(event.originalEvent.target);
        }

        oClientMode.onAddPositionEmployee(parseInt(srcElement.attr("data-IDRow"), 10));
    });

    $(selector + ',.productiveUnit-quantity').off('change');
    $(selector + ',.productiveUnit-quantity').on('change', function (e) {
        var srcElement = null;
        if (typeof (event.srcElement) != 'undefined') {
            srcElement = $(event.srcElement);
        } else {
            srcElement = $(event.originalEvent.target);
        }
        var iRow = parseInt(srcElement.attr('data-IDRow'), 10);

        oClientMode.onUpdateRowQuantity(parseInt(srcElement.attr("data-IDRow"), 10), oPUnitData[iRow], parseInt(srcElement.val(), 10), srcElement);
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
        oPUnitData[iRow].IsExpandable = $(this).prop('checked');
    });

    $('#' + oPUnit.prefix + 'Main div.fht-fixed-body div.fht-tbody').scrollLeft((oPUnit.firstCellPrinted - 46) * 30);
    oPUnit.firstCellPrinted = -1;

    var oDetail = this;

    Object.keys(oDetail.employeeLists).forEach(function (key, indexList) {
        lstConfig = oDetail.employeeLists[key];

        lstConfig.instance = $("#" + lstConfig.id).dxList({
            dataSource: lstConfig.ds,
            height: "100%",
            allowItemDeleting: true,
            itemDeleteMode: 'static',
            selectionMode: 'none',
            pageLoadMode: 'scrollBottom',
            noDataText: '',
            itemTemplate: function (data, indexEmp) {
                var result = $("<div>");

                if (data.IDEmployee >= 0) {
                    var cssClass = "";
                    var divTitle = "";

                    var dayIndictments = [];
                    if (data.Alerts != null && data.Alerts.OnAbsenceDays != null && data.Alerts.OnAbsenceDays) {
                        dayIndictments.push({ employeeName: data.EmployeeName, indictment: { ErrorText: Globalize.formatMessage('roEmployeeAbsent') } });
                    }

                    if (data.Alerts != null && data.Alerts.Indictments != null && data.Alerts.Indictments.length > 0) {
                        for (iInd = 0; iInd < data.Alerts.Indictments.length; iInd++) {
                            dayIndictments.push({ employeeName: data.EmployeeName, indictment: Object.clone(data.Alerts.Indictments[iInd], true) });
                        }
                    }

                    if (dayIndictments.length > 0) {
                        cssClass = 'budgetDetailOnAbsence';
                    }

                    result.append($("<div>").attr('id', oPUnit.ascxPrefix + '_tooltipIndictments_' + indexList + '_' + indexEmp).attr('class', cssClass).attr('title', divTitle).text(data.EmployeeName));

                    if (dayIndictments.length > 0) {
                        //var button = $('<div></div>').attr('id', oPUnit.ascxPrefix + '_tooltipIndictments_' + rowPosition + '_' + columnPosition).attr('style', 'float:left;').attr('class', 'budgetMainDetailOnAbsence');

                        var indictmentTooltipContainer = $('<div></div>').attr('id', oPUnit.ascxPrefix + '_tooltipIndictmentsContainer_' + indexList + '_' + indexEmp).attr('class', 'tooltipIndictmentsContainer');
                        var divIndictMentsTooltipTitle = $('<div></div>').html('<span class="fontBold fontBig" style="text-align:left">' + Globalize.formatMessage("roIndictmentsTitle") + '</span>');

                        var ulInditmentsTooltipList = $('<ul></ul>');

                        for (var iDayI = 0; iDayI < dayIndictments.length; iDayI++) {
                            ulInditmentsTooltipList.append($('<li></li>').html(dayIndictments[iDayI].indictment.ErrorText))
                        }

                        indictmentTooltipContainer.append(divIndictMentsTooltipTitle, ulInditmentsTooltipList);
                        result.append(indictmentTooltipContainer);
                    }
                } else {
                    if (data.EmployeeName == Globalize.formatMessage('roEmployeeNotAssigned')) $("<div>").attr('style', 'color:red').text(data.EmployeeName).appendTo(result);
                    else $("<div>").attr('style', 'color:#00cc00').text(data.EmployeeName).appendTo(result);
                }

                return result;
            },
            onItemDeleting: function (e) {
                var d = $.Deferred();
                oDetail.AskForDelete(d, e.itemData, parseInt(e.element.attr('data-IdRow'), 10));
                e.cancel = d.promise();
            },
            onItemDeleted: function () {
                oClientMode.refreshRowsHeight();
            },
            onItemRendered: function (e, data) {
                if (e.itemData.IDEmployee < 0) {
                    e.itemElement.find('.dx-list-static-delete-button-container').each(function (e) {
                        $(this).remove();
                    });
                }
            },
            onItemClick: function (e, data) {
                if (e.itemData.IDEmployee < 0) {
                    oDetail.onAddPositionEmployee(parseInt(e.element.attr('data-IdRow'), 10));
                }
            }
        }).dxList("instance");
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

    setTimeout(function () { oClientMode.refreshRowsHeight() }, 200);
};

Robotics.Client.Controls.roDayDetailSchedule.prototype.AskForDelete = function (d, itemData, idRow) {
    var oClientmode = this;
    var finishCallback = function (itemData) {
        Robotics.Client.Global.deferredItemData = { oCalendar: oClientmode, d: d, itemData: itemData, idRow: idRow };

        return function (value) {
            if (value) {
                Robotics.Client.Global.deferredItemData.oCalendar.finallyDelete();
            } else Robotics.Client.Global.deferredItemData.d.resolve(!value);
        }
    };

    DevExpress.ui.dialog.confirm(Globalize.formatMessage('roUnassignEmployeeConfirmation')).done(finishCallback(itemData)).fail(d.reject);
}

Robotics.Client.Controls.roDayDetailSchedule.prototype.finallyDelete = function () {
    var oPUnit = this.oBaseControl;
    var oPUnitData = oPUnit.oCalendar;

    var oParameters = {};

    oParameters.idOrgChartNode = parseInt(oPUnit.employeeFilter, 10);
    oParameters.pUnitModePosition = oPUnitData[Robotics.Client.Global.deferredItemData.idRow];;
    oParameters.sDate = oPUnit.firstDate;
    oParameters.employeeData = [Robotics.Client.Global.deferredItemData.itemData];
    oParameters.StampParam = new Date().getMilliseconds();

    oPUnit.performAction(oParameters, Robotics.Client.Constants.Actions.RemoveEmployeeFromPosition);
}

Robotics.Client.Controls.roDayDetailSchedule.prototype.refreshRowsHeight = function () {
    var oPUnit = this.oBaseControl;
    var oPUnitData = oPUnit.oCalendar;

    $('.calendarDailyBodyCell.DailyCell').each(function (index) {
        var idRow = this.id.replace(oPUnit.ascxPrefix + '_calDailyCell_', '').split('_')[0];
        var newHeight = 'height:' + $('#' + oPUnit.ascxPrefix + '_calEmployeeResumeCell_' + idRow).height() + 'px !important';
        $('#' + this.id).attr('style', newHeight);

        if (index == 0) {
            $('#' + oPUnit.ascxPrefix + '_calPositionCell_' + idRow).parent().attr('style', newHeight);
            $('#' + oPUnit.ascxPrefix + '_calQuantityCell_' + idRow).parent().attr('style', newHeight);
            $('#' + oPUnit.ascxPrefix + '_calEmployeeResumeCell_' + idRow).parent().attr('style', newHeight);
        }
    });
}

Robotics.Client.Controls.roDayDetailSchedule.prototype.setSingleSelectedObject = function (sender) {
    var oPUnit = this.oBaseControl;
    var oPUnitData = oPUnit.oCalendar;

    if (sender == null) {
        oPUnit.selectedEmployee = null;
        oPUnit.selectedContainer = null;
    } else {
        oPUnit.selectedEmployee = oPUnitData[parseInt($(sender).attr('data-IDRow'), 10)];
    }
};

Robotics.Client.Controls.roDayDetailSchedule.prototype.refreshModeTables = function (objectRef, isResizing, refreshCalendar) {
    var oPUnit = this;

    if (objectRef != null) {
        if (objectRef instanceof Robotics.Client.Controls.roCalendar) oPUnit = objectRef.clientMode;
        else oPUnit = objectRef;
    }

    if (refreshCalendar) {
        oPUnit.refreshMainTable(isResizing);
        this.oBaseControl.mapEvents();
    }
};

Robotics.Client.Controls.roDayDetailSchedule.prototype.refreshMainTable = function (isResizing) {
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

Robotics.Client.Controls.roDayDetailSchedule.prototype.createResumeTables = function (idTable, parentId) {
    switch (idTable) {
        case Robotics.Client.Constants.TableNames.Calendar:
            this.createPUnitTable(idTable, parentId);
            break;
    }
};

Robotics.Client.Controls.roDayDetailSchedule.prototype.createPUnitTable = function (idTable, parentId) {
    var oPUnit = this.oBaseControl;

    var tableContainer = $('#' + oPUnit.prefix + parentId);
    tableContainer.empty();

    var tableElement = $('<table></table>').attr('id', oPUnit.prefix + idTable).attr('class', 'fancyTable');

    tableElement.append(this.createPUnitTableHeader(idTable, parentId));

    tableElement.append(this.createPUnitTableBody(idTable, parentId));

    tableContainer.append(tableElement);
};

Robotics.Client.Controls.roDayDetailSchedule.prototype.createPUnitTableHeader = function (idTable, parentId) {
    var oPUnit = this.oBaseControl;
    var oPUnitData = oPUnit.oCalendar;

    //Creamos el header
    var tHeader = $('<thead></thead>').attr('id', oPUnit.prefix + idTable + '_thead').attr('class', 'HeaderSelectable');

    var tHeaderRow = $('<tr></tr>');

    //Creamos la primera columna que sera el header
    var tFixedHeaderCell = $('<th></th>');
    var mainFixedHeaderDiv = $('<div></div>').attr('class', 'DetailScheduleFixed CalendarFixedHeader');

    var summaryHeaderCell = $('<div></div>').attr('class', 'Position_DetailScheduleFixed CalendarSummaryFixedHeader');

    var northSummaryCell = $('<div></div>').attr('class', 'NorthCell');
    northSummaryCell.append($('<div></div>').attr('class', 'ProductiveUnitHeaderText').html(Globalize.formatMessage("roPUnitPosition")));

    var southSummaryCell = $('<div></div>').attr('class', 'SouthCell');
    southSummaryCell.append($('<div></div>').attr('class', 'EmpCellTextSouth').html(''));

    summaryHeaderCell.append(northSummaryCell, southSummaryCell);

    var employeeHeaderCell = $('<div></div>').attr('class', 'Quantity_DetailScheduleFixed CalendarEmployeeFixedHeader');
    var northCell = $('<div></div>').attr('class', 'NorthCell NorthCellHeaderEmployee');

    northCell.append($('<div></div>').attr('class', 'ProductiveUnitHeaderText').html(Globalize.formatMessage("roPUnitQuantity")));

    var southCell = $('<div></div>').attr('class', 'SouthCell');
    southCell.append($('<div></div>').attr('class', 'EmpCellTextSouth').html(''));

    employeeHeaderCell.append(northCell, southCell);

    var employeeResumeHeaderCell = $('<div></div>').attr('class', 'EmployeeResume_DetailScheduleFixed CalendarEmployeeFixedHeader');
    var northCell = $('<div></div>').attr('class', 'NorthCell NorthCellHeaderEmployee');
    northCell.append($('<div></div>').attr('class', 'ProductiveUnitHeaderText').html(Globalize.formatMessage("roPUnitEmployees")));

    var southCell = $('<div></div>').attr('class', 'SouthCell');
    southCell.append($('<div></div>').attr('class', 'EmpCellTextSouth').html(''));

    employeeResumeHeaderCell.append(northCell, southCell);

    tHeaderRow.append(tFixedHeaderCell.append(mainFixedHeaderDiv.append(summaryHeaderCell, employeeHeaderCell, employeeResumeHeaderCell)));

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

            var mainDayHeaderCell = $('<div></div>').attr('id', oPUnit.ascxPrefix + '_calHeaderCell_' + i).attr('class', 'CalendarDayFixed ProductiveUnitDetailHeaderCell DailyCell').attr('data-IDColumn', i).attr('style', 'background:' + backColor);
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

Robotics.Client.Controls.roDayDetailSchedule.prototype.createPUnitTableBody = function (idTable, parentId) {
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

            var mainFixedBodyCell = $('<div></div>').attr('class', 'DetailScheduleFixed CalendarFixedBody');

            var fixedEmployeeBodyCell = $('<div></div>').attr('class', 'Position_DetailScheduleFixed CalendarSummaryFixedBody');
            fixedEmployeeBodyCell.append(this.generatePositionSection(i, oPUnitData[i]));

            var fixedSummaryBodyCell = $('<div></div>').attr('class', 'Quantity_DetailScheduleFixed CalendarEmployeeFixedBody');
            fixedSummaryBodyCell.append(this.generateQuantitySection(i, oPUnitData[i]));

            var employeeResumeHeaderCell = $('<div></div>').attr('class', 'EmployeeResume_DetailScheduleFixed CalendarEmployeeFixedBody');
            employeeResumeHeaderCell.append(this.generateEmployeeResumeSection(i, oPUnitData[i]));

            tBodyRow.append(tFixedBodyCell.append(mainFixedBodyCell.append(fixedEmployeeBodyCell, fixedSummaryBodyCell, employeeResumeHeaderCell)));

            this.descriptionExists = false;
            this.buildPUnitModePostionRow(i, tBodyRow);

            tmpCell = $('<td style="width:100%"></td>').html('&nbsp;');
            tBodyRow.append(tmpCell);

            tBody.append(tBodyRow);
        }
    } else {
        var tBodyRow = $('<tr></tr>');
        var tFixedBodyCell = $('<td></td>');
        var mainFixedBodyCell = $('<div></div>').attr('class', 'DetailScheduleFixed CalendarFixedBody');
        var fixedEmployeeBodyCell = $('<div></div>').attr('class', 'Position_DetailScheduleFixed CalendarSummaryFixedBody');
        var fixedSummaryBodyCell = $('<div></div>').attr('class', 'Quantity_DetailScheduleFixed CalendarEmployeeFixedBody');
        var employeeResumeHeaderCell = $('<div></div>').attr('class', 'EmployeeResume_DetailScheduleFixed CalendarEmployeeFixedBody');

        tBodyRow.append(tFixedBodyCell.append(mainFixedBodyCell.append(fixedEmployeeBodyCell, fixedSummaryBodyCell, employeeResumeHeaderCell)));

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

Robotics.Client.Controls.roDayDetailSchedule.prototype.generatePositionSection = function (rowIndex, oUnitMode) {
    var oPUnit = this.oBaseControl;
    var oPUnitData = oPUnit.oCalendar;

    var positionDiv = $('<div></div>').attr('id', oPUnit.ascxPrefix + '_calPositionCell_' + rowIndex).attr('class', 'productiveUnit-position centerVerticallPUnitHeader');
    positionDiv.attr('data-IDRow', rowIndex).html((rowIndex + 1));

    return positionDiv;
}

Robotics.Client.Controls.roDayDetailSchedule.prototype.generateQuantitySection = function (rowIndex, oUnitMode) {
    var oPUnit = this.oBaseControl;
    var oPUnitData = oPUnit.oCalendar;

    var quantityDiv = $('<div></div>').attr('id', oPUnit.ascxPrefix + '_calQuantityCell_' + rowIndex).attr('data-IDRow', rowIndex).attr('class', 'centerVerticallPUnitHeader');

    var firstLine = $('<div></div>').attr('class', 'qtyPosition');
    var txtquantityDiv = $('<input>').attr({ 'type': 'text', 'id': oPUnit.ascxPrefix + '_qty_' + rowIndex, 'data-IDRow': rowIndex, 'class': 'productiveUnit-quantity' });
    $(txtquantityDiv).val(oUnitMode.Quantity);
    firstLine.append(txtquantityDiv);

    quantityDiv.append(firstLine);
    var visibleIcons = 1;
    var fourthLine = $('<div></div>').attr('class', 'actionsExpandable');
    //if (oUnitMode.IsExpandable) {
    //    var addIcon = $('<div></div>').attr('data-IDRow', rowIndex).attr('class', 'addEmployee addEmployeeIcon').attr('title', Globalize.formatMessage('roAddEmployee'));
    //    fourthLine.append(addIcon);
    //    visibleIcons += 1;
    //}

    var addIcon = $('<div></div>').attr('data-IDRow', rowIndex).attr('class', 'fulfillEmployees fulfillEmployeesIcon').attr('title', Globalize.formatMessage('roFulfillEmployees'));
    //if (oUnitMode.IsExpandable) addIcon.attr('style', 'margin-left:5px');

    fourthLine.append(addIcon);
    fourthLine.attr('style', 'margin-left:calc(50% - ' + (visibleIcons * 8) + 'px)');

    quantityDiv.append(fourthLine);

    return quantityDiv;
}

Robotics.Client.Controls.roDayDetailSchedule.prototype.generateEmployeeResumeSection = function (rowIndex, oUnitMode) {
    var oPUnit = this.oBaseControl;
    var oPUnitData = oPUnit.oCalendar;

    var employeeResumeDiv = $('<div></div>').attr('id', oPUnit.ascxPrefix + '_calEmployeeResumeCell_' + rowIndex).attr('data-IDRow', rowIndex);

    var employeeList = $('<div></div>').attr({ id: oPUnit.ascxPrefix + '_calEmployeeResumeCell_Lst_' + rowIndex, style: 'min-height:80px;overflow-y: auto;', 'data-IDRow': rowIndex });

    oUnitMode.EmployeesData = oUnitMode.EmployeesData.sortBy(function (n) {
        return n.EmployeeName;
    });

    var allEmployeeAssigned = oUnitMode.EmployeesData.length >= oUnitMode.Quantity ? true : false;
    var iLength = oUnitMode.Quantity - oUnitMode.EmployeesData.length;
    for (var i = 0; i < iLength; i++) {
        oUnitMode.EmployeesData.unshift({ IDEmployee: -1, EmployeeName: Globalize.formatMessage('roEmployeeNotAssigned') });
    }

    if (allEmployeeAssigned && oUnitMode.IsExpandable) {
        oUnitMode.EmployeesData.append({ IDEmployee: -1, EmployeeName: Globalize.formatMessage('roEmployeeAddExtraEmployee') });
    }

    if (typeof this.employeeLists[rowIndex] == 'undefined') this.employeeLists[rowIndex] = { ds: oUnitMode.EmployeesData, instance: null, id: (oPUnit.ascxPrefix + '_calEmployeeResumeCell_Lst_' + rowIndex) };

    employeeResumeDiv.append(employeeList);

    return employeeResumeDiv;
}

Robotics.Client.Controls.roDayDetailSchedule.prototype.buildPUnitModePostionRow = function (rowIndex, tBodyRow) {
    var oPUnit = this.oBaseControl;
    var oPUnitData = oPUnit.oCalendar;

    var dayData = oPUnitData[rowIndex].ShiftHourData;

    for (var columnIndex = oPUnit.getMinDailyCell(); columnIndex < oPUnit.getMaxDailyCell(); columnIndex++) {
        //for (var columnIndex = 0; columnIndex < oCalData.CalendarHeader.PeriodHeaderData.length; columnIndex++) {
        tBodyRow.append(this.createDailyCalendarCell(oPUnitData[rowIndex], dayData[columnIndex], rowIndex, columnIndex));
    }
}

Robotics.Client.Controls.roDayDetailSchedule.prototype.createDailyCalendarCell = function (positionData, cellInfo, rowPosition, columnPosition) {
    var oPUnit = this.oBaseControl;
    var oPUnitData = oPUnit.oCalendar;

    var calendarCell = $('<td></td>');

    if (columnPosition % 2 == 0) {
        calendarCell.addClass('columnDailyCalendarOdd');
    }

    var calendarOuterContent = $('<div></div>').attr('id', oPUnit.ascxPrefix + '_calDailyCell_' + rowPosition + '_' + columnPosition).attr('class', 'calendarDailyBodyCell DailyCell DailySeparator');
    calendarOuterContent.attr('data-IDRow', rowPosition).attr('data-IDColumn', columnPosition);

    this.createDailyCalendarCellContent(calendarOuterContent, positionData, cellInfo, columnPosition);

    return calendarCell.append(calendarOuterContent);
};

Robotics.Client.Controls.roDayDetailSchedule.prototype.createDailyCalendarCellContent = function (containter, positionData, cellInfo, columnPosition) {
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

Robotics.Client.Controls.roDayDetailSchedule.prototype.generateShiftInfoDailyContainer = function (shiftName, color) {
    var oPUnit = this.oBaseControl;
    var oPUnitData = oPUnit.oCalendar;

    var shiftInfoContainer = $('<div></div>').attr('class', 'calendarShiftInfoDailyContainer').attr('title', shiftName).append($("<span class='fontBold calendarFeastLeft'></span>").html(shiftName));

    shiftInfoContainer.attr('style', 'color:' + new Robotics.Client.Common.roHtmlColor().invertCssColor(color));

    return shiftInfoContainer;
};

Robotics.Client.Controls.roDayDetailSchedule.prototype.generateShiftTypeInfoDailyContainer = function (assignmentText, color, hours, marginAssignment) {
    var oPUnit = this.oBaseControl;
    var oPUnitData = oPUnit.oCalendar;

    var cellText = assignmentText;
    var shiftInfoContainer = $('<div></div>').attr('class', 'calendarShiftTypeInfoDailyContainer').attr('title', cellText).html(cellText);

    shiftInfoContainer.attr('style', 'color:' + new Robotics.Client.Common.roHtmlColor().invertCssColor(color) + ';' + marginAssignment);

    return shiftInfoContainer;
};

Robotics.Client.Controls.roDayDetailSchedule.prototype.endCallback = function (action, objResult, objResultParams) {
    var oClientMode = this;
    var oCalendar = this.oBaseControl;
    var oCalData = oCalendar.oCalendar;

    switch (action) {
        case Robotics.Client.Constants.Actions.GetAvailablePositionEmployees:
            oCalendar.prepareBudgetEmployeesDialog(objResult, objResultParams);
            break;
        case Robotics.Client.Constants.Actions.GetAvailablePositionEmployeesForFulFill:
            oClientMode.assignPossibleEmployees(objResult, objResultParams);
            break;
        case Robotics.Client.Constants.Actions.AddEmployeePlanOnPosition:
            oClientMode.manageEmployeeList(objResult, true);
            break;
        case Robotics.Client.Constants.Actions.RemoveEmployeeFromPosition:
            oClientMode.manageEmployeeList(objResult, false);
            break;
        case Robotics.Client.Constants.Actions.UpdateProductiveUnitQuantity:
            oClientMode.resizeEmployeeList(objResult, false);
            break;
        default:
            break;
    }

    if (action != Robotics.Client.Constants.Actions.GetAvailablePositionEmployeesForFulFill) this.loadingFunctionExtended(false);
};

Robotics.Client.Controls.roDayDetailSchedule.prototype.resizeEmployeeList = function (bRes) {
    var oClientMode = this;
    var oPUnit = this.oBaseControl;
    var oPUnitData = oPUnit.oCalendar;

    if (bRes) {
        var cUnitMode = oPUnitData[Robotics.Client.Global.rowContext.id];
        var idRow = Robotics.Client.Global.rowContext.id;

        cUnitMode.Quantity = Robotics.Client.Global.rowContext.quantity;

        cUnitMode.EmployeesData = cUnitMode.EmployeesData.remove(function (n) {
            return n.IDEmployee == -1;
        });

        var allEmployeeAssigned = cUnitMode.EmployeesData.length >= cUnitMode.Quantity ? true : false;
        var iLength = cUnitMode.Quantity - cUnitMode.EmployeesData.length;
        for (var i = 0; i < iLength; i++) {
            cUnitMode.EmployeesData.unshift({ IDEmployee: -1, EmployeeName: Globalize.formatMessage('roEmployeeNotAssigned') });
        }

        if (allEmployeeAssigned && cUnitMode.IsExpandable) {
            cUnitMode.EmployeesData.append({ IDEmployee: -1, EmployeeName: Globalize.formatMessage('roEmployeeAddExtraEmployee') });
        }

        oClientMode.employeeLists[idRow].instance.option('dataSource', { store: oClientMode.employeeLists[idRow].ds });

        Robotics.Client.Global.deferredItemData = { oCalendar: null, d: null, itemData: null, idRow: -1 };
        Robotics.Client.Global.rowContext = { id: -1, oObject: null, quantity: 0 };

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

        setTimeout(function () { oClientMode.refreshRowsHeight(); oClientMode.loadingFunctionExtended(false); }, 200);
    }
}
Robotics.Client.Controls.roDayDetailSchedule.prototype.manageEmployeeList = function (bRes, isAddingEmployee) {
    var oClientMode = this;
    var oPUnit = this.oBaseControl;
    var oPUnitData = oPUnit.oCalendar;

    if (bRes) {
        var cUnitMode = null;
        var allEmployeeAssigned = false;
        if (isAddingEmployee) {
            cUnitMode = oPUnitData[Robotics.Client.Global.rowContext.id];
            var idRow = Robotics.Client.Global.rowContext.id;

            cUnitMode.EmployeesData = cUnitMode.EmployeesData.remove(function (n) {
                return n.IDEmployee == -1;
            });

            for (var i = 0; i < Robotics.Client.Global.rowContext.oObject.length; i++) {
                cUnitMode.EmployeesData.push(Object.clone(Robotics.Client.Global.rowContext.oObject[i], true));
            }

            cUnitMode.EmployeesData = cUnitMode.EmployeesData.sortBy(function (n) {
                return n.EmployeeName;
            });

            allEmployeeAssigned = cUnitMode.EmployeesData.length >= cUnitMode.Quantity ? true : false;
            var iLength = cUnitMode.Quantity - cUnitMode.EmployeesData.length;
            for (var i = 0; i < iLength; i++) {
                cUnitMode.EmployeesData.unshift({ IDEmployee: -1, EmployeeName: Globalize.formatMessage('roEmployeeNotAssigned') });
            }
        } else {
            cUnitMode = oPUnitData[Robotics.Client.Global.deferredItemData.idRow];
            var idRow = Robotics.Client.Global.deferredItemData.idRow;

            cUnitMode.EmployeesData = cUnitMode.EmployeesData.remove(function (n) {
                return n.IDEmployee == Robotics.Client.Global.deferredItemData.itemData.IDEmployee;
            });

            cUnitMode.EmployeesData = cUnitMode.EmployeesData.remove(function (n) {
                return n.IDEmployee == -1;
            });

            cUnitMode.EmployeesData = cUnitMode.EmployeesData.sortBy(function (n) {
                return n.EmployeeName;
            });

            allEmployeeAssigned = cUnitMode.EmployeesData.length >= cUnitMode.Quantity ? true : false;
            var iLength = cUnitMode.Quantity - cUnitMode.EmployeesData.length;
            for (var i = 0; i < iLength; i++) {
                cUnitMode.EmployeesData.unshift({ IDEmployee: -1, EmployeeName: Globalize.formatMessage('roEmployeeNotAssigned') });
            }

            var value = true;
            Robotics.Client.Global.deferredItemData.d.resolve(!value);
        }

        if (allEmployeeAssigned && cUnitMode.IsExpandable) {
            cUnitMode.EmployeesData.append({ IDEmployee: -1, EmployeeName: Globalize.formatMessage('roEmployeeAddExtraEmployee') });
        }

        oClientMode.employeeLists[idRow].instance.option('dataSource', { store: oClientMode.employeeLists[idRow].ds });

        Robotics.Client.Global.deferredItemData = { oCalendar: null, d: null, itemData: null, idRow: -1 };
        Robotics.Client.Global.rowContext = { id: -1, oObject: null };

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

        setTimeout(function () { oClientMode.refreshRowsHeight(); oClientMode.loadingFunctionExtended(false); }, 200);
    } else {
    }
}

Robotics.Client.Controls.roDayDetailSchedule.prototype.onAcceptBudgetEmployeesDialog = function () {
    var oClientMode = this;
    var oPUnit = this.oBaseControl;
    var oPUnitData = oPUnit.oCalendar;

    var oParameters = {};
    var selEmps = oPUnit.budgetEmployeesManager.getMultipleEmployeeSelection();

    var empParam = [];
    var maxEmployees = selEmps.length;

    if (!oPUnitData[Robotics.Client.Global.rowContext.id].IsExpandable) {
        maxEmployees = 0;
        for (var iEmp = 0; iEmp < oPUnitData[Robotics.Client.Global.rowContext.id].EmployeesData.length; iEmp++) {
            if (oPUnitData[Robotics.Client.Global.rowContext.id].EmployeesData[iEmp].IDEmployee < 0) maxEmployees += 1;
        }
    }

    if (maxEmployees > selEmps.length) maxEmployees = selEmps.length;

    for (var i = 0; i < maxEmployees; i++) {
        empParam.push({
            EmployeeName: selEmps[i].EmployeeName,
            IDEmployee: selEmps[i].IDEmployee,
            Alerts: { Indictments: Object.clone(selEmps[i].Indictments, true) },
            ShiftData: Object.clone(oPUnitData[Robotics.Client.Global.rowContext.id].ShiftData, true)
        });
    }

    Robotics.Client.Global.rowContext.oObject = empParam;

    oParameters.idOrgChartNode = parseInt(oPUnit.employeeFilter, 10);
    oParameters.pUnitModePosition = oPUnitData[Robotics.Client.Global.rowContext.id];
    oParameters.sDate = oPUnit.firstDate;
    oParameters.employeeData = Robotics.Client.Global.rowContext.oObject;
    oParameters.StampParam = new Date().getMilliseconds();

    oPUnit.performAction(oParameters, Robotics.Client.Constants.Actions.AddEmployeePlanOnPosition);

    oPUnit.budgetEmployeesDialog.dialog("close");
};

Robotics.Client.Controls.roDayDetailSchedule.prototype.prepareBudgetEmployeesDialog = function (oEmployeeList, oNodeStatus) {
    var oPUnit = this.oBaseControl;
    var oPUnitData = oPUnit.oCalendar;

    var idRow = Robotics.Client.Global.rowContext.id;

    var oParameters = {};
    oParameters.pUnit = this.selectedPUnit;
    oParameters.pUnitMode = this.selectedPUnitMode;
    oParameters.pUnitModePosition = oPUnitData[idRow];
    oParameters.sDate = oPUnit.firstDate;

    oPUnit.budgetEmployeesDialog.dialog("open");
    oPUnit.budgetEmployeesManager.InitializeDialog(oEmployeeList, oNodeStatus, oParameters);
};

Robotics.Client.Controls.roDayDetailSchedule.prototype.assignPossibleEmployees = function (oEmployeeList, oNodeStatus) {
    var oPUnit = this.oBaseControl;
    var oPUnitData = oPUnit.oCalendar;

    var idRow = Robotics.Client.Global.rowContext.id;
    var cUnitModePosition = oPUnitData[idRow];
    var maxEmployees = cUnitModePosition.Quantity;

    if (oEmployeeList.length != 0) {
        var empParam = [];

        for (var iEmp = 0; iEmp < cUnitModePosition.EmployeesData.length; iEmp++) {
            if (cUnitModePosition.EmployeesData[iEmp].IDEmployee > 0) maxEmployees -= 1;
        }

        for (var i = 0; i < maxEmployees; i++) {
            if (i < oEmployeeList.length) {
                empParam.push({
                    EmployeeName: oEmployeeList[i].EmployeeName,
                    IDEmployee: oEmployeeList[i].IDEmployee,
                    ShiftData: Object.clone(oPUnitData[Robotics.Client.Global.rowContext.id].ShiftData, true)
                });
            }
        }

        Robotics.Client.Global.rowContext.oObject = empParam;

        var oParameters = {};
        oParameters.idOrgChartNode = parseInt(oPUnit.employeeFilter, 10);
        oParameters.pUnitModePosition = oPUnitData[Robotics.Client.Global.rowContext.id];
        oParameters.sDate = oPUnit.firstDate;
        oParameters.employeeData = Robotics.Client.Global.rowContext.oObject;
        oParameters.StampParam = new Date().getMilliseconds();

        oPUnit.performAction(oParameters, Robotics.Client.Constants.Actions.AddEmployeePlanOnPosition);
    } else {
        this.loadingFunctionExtended(false);
    }

    if (oEmployeeList.length < maxEmployees) {
        DevExpress.ui.dialog.alert(Globalize.formatMessage('roNotEnoughtEmployees'), Globalize.formatMessage('roAlert'));
    }
};