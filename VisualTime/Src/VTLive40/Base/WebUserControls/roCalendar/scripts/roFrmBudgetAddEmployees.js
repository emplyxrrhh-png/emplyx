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

Robotics.Client.Controls.Forms.BudgetAddEmployeeForm = function (prefix, showErrorPopup) {
    this.prefix = prefix;
    this.showErrorPopup = showErrorPopup;

    this.lblInHolidays = eval(this.prefix + "_lblInHolidays");
    this.lblInRest = eval(this.prefix + "_lblInRest");
    this.lblOnAbsence = eval(this.prefix + "_lblOnAbsence");
    this.lblWithoutAssignment = eval(this.prefix + "_lblWithoutAssignment");

    this.gridId = '#' + prefix + '_divAvailableEmployeesGrid';
    this.gridInstance = null;

    this.lblDayInformation = $('#' + this.prefix + '_lblDayInformation');

    this.selectedEmployee = null;
    this.selectedMultipleEmployee = null;

    this.employeesDS = null;
    this.statusDS = null;

    this.tooltips = { t0: null, t1: null, t2: null, t3: null };
};

Robotics.Client.Controls.Forms.BudgetAddEmployeeForm.prototype.InitializeDialog = function (employees, status, oNodeInfo) {
    this.selectedEmployee = null;
    this.selectedMultipleEmployee = null;

    this.employeesDS = Object.clone(employees, true);
    this.statusDS = Object.clone(status, true);

    this.initGrid();
    this.initTooltips();
    this.setUpDescription(oNodeInfo);
};

Robotics.Client.Controls.Forms.BudgetAddEmployeeForm.prototype.initGrid = function () {
    var form = this;
    try {
        try {
            this.gridInstance = $(this.gridId).dxDataGrid('instance');
        } catch (e) {
            this.gridInstance = null;
        }

        if (this.gridInstance == null) {
            this.gridInstance = $(this.gridId).dxDataGrid({
                showColumnLines: true,
                showRowLines: true,
                height: '250px',
                allowColumnResizing: true,
                rowAlternationEnabled: true,
                showBorders: true,
                dataSource: {
                    store: form.employeesDS
                },
                headerFilter: {
                    visible: true
                },
                filterRow: {
                    visible: true
                },
                selection: {
                    mode: 'multiple',
                    showCheckBoxesMode: 'always'
                },
                editing: {
                    mode: "row",
                    allowUpdating: false,
                    allowDeleting: false,
                    texts: { deleteRow: 'Delete', editRow: 'Edit' }
                },
                onCellPrepared: function (e) {
                    if (e.rowType === "data" && e.column.command === "edit") {
                        var isEditing = e.row.isEditing, $links = e.cellElement.find(".dx-link");
                        $links.text("");

                        if (isEditing) {
                            $links.filter(".dx-link-save").addClass("dx-icon-save");
                            $links.filter(".dx-link-cancel").addClass("dx-icon-revert");
                        } else {
                            $links.filter(".dx-link-edit").addClass("dx-icon-edit");
                            $links.filter(".dx-link-delete").addClass("dx-icon-trash");
                        }
                    }
                },
                onSelectionChanged: function (selectedItems) {
                    var data = selectedItems.selectedRowsData;
                    if (data.length > 0) {
                        form.selectedMultipleEmployee = data;
                    } else {
                        form.selectedMultipleEmployee = null;
                    }
                },
                onRowClick: function (e) {
                    var data = e.data;
                    if (data) {
                        form.selectedEmployee = data;
                    } else {
                        form.selectedEmployee = null;
                    }
                },
                onContentReady: function (e) {
                    $('.ratingControl').each(function (index) {
                        var tmpControl = $($(this)[0].childNodes[0]);

                        if (tmpControl[0].tagName.toUpperCase() == 'SELECT') {
                            tmpControl.barrating({
                                theme: 'fontawesome-stars',
                                readonly: true,
                                hoverState: false,
                                initialRating: tmpControl.attr('data-value')
                            });

                            if ($(this)[0].childNodes.length > 1) {
                                var tmpTooltip = $($(this)[0].childNodes[1]);
                                var tooltipTarget = this;
                                tmpTooltip.dxTooltip({
                                    target: tooltipTarget,
                                    showEvent: { name: "mouseenter", delay: 150 },
                                    hideEvent: "mouseleave",
                                    position: 'bottom'
                                });
                            }
                        }
                    });
                },
                remoteOperations: {
                    sorting: true
                },
                paging: {
                    enabled: false
                },
                columns: [
                    {
                        caption: Globalize.formatMessage('roSuitability'),
                        dataField: "TotalIndictments",
                        allowEditing: false,
                        width: '120px',
                        allowFiltering: false,
                        allowSearch: false,
                        cellTemplate: function (container, options) {
                            container.addClass("ratingControl");
                            var starRatingControl = $('<select><option value="1"> 1</option><option value="2">2</option><option value="3">3</option><option value="4">4</option><option value="5">5</option></select >');
                            starRatingControl.attr('data-value', (5 - options.data.TotalIndictments));
                            container.append(starRatingControl);

                            if (options.data.TotalIndictments > 0) {
                                var indictmentTooltipContainer = $('<div></div>').attr('id', 'tooltipIndictmentsContainer').attr('class', 'tooltipIndictmentsContainer');
                                var divIndictMentsTooltipTitle = $('<div></div>').html('<span class="fontBold fontBig" style="text-align:left">' + Globalize.formatMessage("roIndictmentsTitle") + '</span>');

                                var ulInditmentsTooltipList = $('<ul></ul>');

                                for (var iDayI = 0; iDayI < options.data.TotalIndictments; iDayI++) {
                                    ulInditmentsTooltipList.append($('<li></li>').html(options.data.Indictments[iDayI].ErrorText))
                                }

                                indictmentTooltipContainer.append(divIndictMentsTooltipTitle, ulInditmentsTooltipList);
                                container.append(indictmentTooltipContainer);
                            }
                        }
                    },
                    { caption: Globalize.formatMessage('roEmployeeName'), dataField: "EmployeeName", allowEditing: false, width: '180px' },
                    { caption: Globalize.formatMessage('roActualShift'), dataField: "ShiftName", allowEditing: false, width: '180px' },
                    { caption: Globalize.formatMessage('roActualAssignment'), dataField: "AssignmentName", allowEditing: false, width: '100px' },
                    { caption: Globalize.formatMessage('roActualGroup'), dataField: "FullGroupName", allowEditing: false, width: '350px' },
                    //{ caption: Globalize.formatMessage('roScore'), dataField: "Name", allowEditing: false },
                    { caption: Globalize.formatMessage('roEmployeeCost'), dataField: "Cost", allowEditing: false, width: '80px' }

                ]
            }).dxDataGrid('instance');
        } else {
            this.gridInstance.option('dataSource', { store: form.employeesDS });

            this.gridInstance.option('onSelectionChanged', function (selectedItems) {
                var data = selectedItems.selectedRowsData;
                if (data.length > 0) {
                    form.selectedMultipleEmployee = data;
                } else {
                    form.selectedMultipleEmployee = null;
                }
            });
            this.gridInstance.option('onRowClick', function (e) {
                var data = e.data;
                if (data) {
                    form.selectedEmployee = data;
                } else {
                    form.selectedEmployee = null;
                }
            });

            this.gridInstance.clearSelection();
        }
    } catch (e) { }

    //setTimeout(function () {
    //}, 100);
};

Robotics.Client.Controls.Forms.BudgetAddEmployeeForm.prototype.initTooltips = function () {
    var form = this;

    this.lblOnAbsence.SetText(form.statusDS.EmployeesOnProgrammedAbsence.length);
    this.lblInRest.SetText(form.statusDS.EmployeesNoWorkingShift.length);
    this.lblInHolidays.SetText(form.statusDS.EmployeesOnHolidays.length);
    this.lblWithoutAssignment.SetText(form.statusDS.EmployeesWithoutAssignment.length);

    this.tooltips.t0 = $("#" + this.prefix + "_lblWithoutAssignmentTooltip").dxTooltip({
        target: "#" + this.prefix + '_lblWithoutAssignmentContainer',
        showEvent: { name: "mouseenter", delay: 300 },
        hideEvent: "dxclick",
        position: 'top',
        onShowing: form.fulfillList(0, this.prefix + "_lstEmployeesWithousAssignment", form.statusDS.EmployeesWithoutAssignment)
    }).dxTooltip('instance');

    this.tooltips.t1 = $("#" + this.prefix + "_lblInRestTooltip").dxTooltip({
        target: "#" + this.prefix + '_lblInRestContainer',
        showEvent: { name: "mouseenter", delay: 300 },
        hideEvent: "dxclick",
        position: 'top',
        onShowing: form.fulfillList(1, this.prefix + "_lstEmployeesInRest", form.statusDS.EmployeesNoWorkingShift)
    }).dxTooltip('instance');

    this.tooltips.t2 = $("#" + this.prefix + "_lblInHolidaysTooltip").dxTooltip({
        target: "#" + this.prefix + '_lblInHolidaysContainer',
        showEvent: { name: "mouseenter", delay: 300 },
        hideEvent: "dxclick",
        position: 'top',
        onShowing: form.fulfillList(2, this.prefix + "_lstEmployeesInHolidays", form.statusDS.EmployeesOnHolidays)
    }).dxTooltip('instance');

    this.tooltips.t3 = $("#" + this.prefix + "_lblOnAbsenceTooltip").dxTooltip({
        target: "#" + this.prefix + '_lblOnAbsenceContainer',
        showEvent: { name: "mouseenter", delay: 300 },
        hideEvent: "dxclick",
        position: 'top',
        onShowing: form.fulfillList(3, this.prefix + "_lstEmployeesOnAbsence", form.statusDS.EmployeesOnProgrammedAbsence),
    }).dxTooltip('instance');
};

Robotics.Client.Controls.Forms.BudgetAddEmployeeForm.prototype.setUpDescription = function (oNodeInfo) {
    this.lblDayInformation.empty();

    var nodeName = $('<div></div>').attr('class', 'alignLeft fontBold').html('<span class="lineDescriptionHeight" style="padding-left:10px">' + oNodeInfo.pUnit.Name + '</span>');

    var nodeDate = $('<div></div>').attr('class', 'alignLeft').html('<span class="lineDescriptionHeight">' + Globalize.formatDate(oNodeInfo.sDate, { date: "full" }) + ':</span>');

    var modeTitle = $('<div></div>').attr('class', 'alignLeft paddingLeft').html('<span  class="lineDescriptionHeight">' + Globalize.formatMessage('roPUnitMode') + ':</span>');
    var modeDescription = $('<div></div>').attr('class', 'alignLeft').attr('style', 'padding:6px;margin-left:5px;background-color:' + oNodeInfo.pUnitMode.HtmlColor + ';color:' + new Robotics.Client.Common.roHtmlColor().invertCssColor(oNodeInfo.pUnitMode.HtmlColor)).html(oNodeInfo.pUnitMode.Name);

    var positionTitle = $('<div></div>').attr('class', 'alignLeft paddingLeft').html('<span  class="lineDescriptionHeight">' + Globalize.formatMessage('roPosition') + ':</span>');
    var positionDescription = $('<div></div>').attr('class', 'alignLeft').attr('style', 'padding:6px;margin-left:5px;background-color:' + oNodeInfo.pUnitModePosition.AssignmentData.Color + ';color:' + new Robotics.Client.Common.roHtmlColor().invertCssColor(oNodeInfo.pUnitModePosition.AssignmentData.Color)).html(oNodeInfo.pUnitModePosition.AssignmentData.Name);

    var shiftTitle = $('<div></div>').attr('class', 'alignLeft paddingLeft').html('<span  class="lineDescriptionHeight">' + Globalize.formatMessage('roShift') + ':</span>');
    var shiftDescription = $('<div></div>').attr('class', 'alignLeft').attr('style', 'padding:6px;margin-left:5px;background-color:' + oNodeInfo.pUnitModePosition.ShiftData.Color + ';color:' + new Robotics.Client.Common.roHtmlColor().invertCssColor(oNodeInfo.pUnitModePosition.ShiftData.Color)).html(oNodeInfo.pUnitModePosition.ShiftData.Name);

    var actual = 0;
    for (var i = 0; i < oNodeInfo.pUnitModePosition.EmployeesData.length; i++) {
        if (oNodeInfo.pUnitModePosition.EmployeesData[i].IDEmployee > 0) { actual++; }
    }

    var employeesTitle = $('<div></div>').attr('class', 'alignLeft paddingLeft').html('<span  class="lineDescriptionHeight">' + Globalize.formatMessage('roEmployeesAssigned') + ':</span>');
    var employeesDescription = $('<div></div>').attr('class', 'alignLeft fontBold').html('<span  class="lineDescriptionHeight" style="padding-left:6px">' + Globalize.formatNumber(actual) + '/' + Globalize.formatNumber(oNodeInfo.pUnitModePosition.Quantity) + '</span>');

    this.lblDayInformation.append(nodeDate, nodeName, modeTitle, modeDescription, positionTitle, positionDescription, shiftTitle, shiftDescription, employeesTitle, employeesDescription);
};

Robotics.Client.Controls.Forms.BudgetAddEmployeeForm.prototype.fulfillList = function (idShow, idList, objects) {
    var form = this;
    return function () {
        if (idShow != 0 && form.tooltips.t0 != null) { form.tooltips.t0.hide() }
        if (idShow != 1 && form.tooltips.t1 != null) { form.tooltips.t1.hide() }
        if (idShow != 2 && form.tooltips.t2 != null) { form.tooltips.t2.hide() }
        if (idShow != 3 && form.tooltips.t3 != null) { form.tooltips.t3.hide() }

        $("#" + idList).dxList({
            dataSource: objects,
            height: '300px',
            width: '200px',
            pageLoadMode: 'scrollBottom',
            itemTemplate: function (data, index) {
                return data.EmployeeName;
            }
        }).dxList("instance");
    }
};

Robotics.Client.Controls.Forms.BudgetAddEmployeeForm.prototype.getSelectedEmployee = function () {
    return this.selectedEmployee;
};

Robotics.Client.Controls.Forms.BudgetAddEmployeeForm.prototype.getMultipleEmployeeSelection = function () {
    return this.selectedMultipleEmployee;
};