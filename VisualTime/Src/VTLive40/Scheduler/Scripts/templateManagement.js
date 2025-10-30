var templateManagementPopup = function () {
    this.configuration = {
        popup: null,
        show: null,
        dxnewElement: null,
        dxAcceptButton: null,
        dxTemplateList: null,
        ckIsFeast: null,
        btnAccept: null,
        btnCancel: null,
        newTemplateName: '',
        updatemplateName: '',
        params: {},
        dxTemplateCalendar: null,
        dxUpdateName: null,
        dxSaveButton: null,
        dayEditPopover: null,
        dxBtnRemoveDay: null
    }

    this.selectedItem = null;

    this.minDate = moment().startOf('year').toDate();
    this.maxDate = moment().startOf('day').endOf('year').add(3, 'year').toDate();

    this.calSelectedDays = {};

    this.initialTemplateList = null;
    this.itemLoading = false;

    this.popoverCellKey = '';
}

templateManagementPopup.prototype.show = function () {
    this.itemLoading = true;
    var oParameters = {};
    oParameters.action = "LOADTEMPLATESLIST";
    oParameters.templates = [];
    oParameters.StampParam = new Date().getMilliseconds();
    var strParameters = JSON.stringify(oParameters);
    strParameters = encodeURIComponent(strParameters);

    PerformActionCallbackClient.PerformCallback(strParameters);
}

templateManagementPopup.prototype.parseResponse = function (s, e) {
    if (this.checkResult(s)) {
        switch (s.cpAction) {
            case "LOADTEMPLATESLIST":
                this.initialTemplateList = JSON.parse(s.cpTemplatesLst);
                if (this.itemLoading && this.initialTemplateList != null) {
                    this.configuration.show(this, this.initialTemplateList);
                    this.itemLoading = false;
                }
                break;
            case "SAVETEMPLATE":

                this.initialTemplateList = JSON.parse(s.cpTemplatesLst);
                var maxId = -1;
                var cItem = null;
                if (this.selectedItem.ID != -1) {
                    for (var i = 0; i < this.initialTemplateList.length; i++) {
                        if (this.initialTemplateList[i].Name == this.selectedItem.Name) {
                            cItem = this.initialTemplateList[i];
                        }
                    }
                } else {
                    for (var i = 0; i < this.initialTemplateList.length; i++) {
                        if (this.initialTemplateList[i].ID > maxId) {
                            maxId = this.initialTemplateList[i].ID;
                            cItem = this.initialTemplateList[i];
                        }
                    }
                }

                this.configuration.show(this, this.initialTemplateList);
                if (cItem != null) this.configuration.dxTemplateList.selectItem(cItem);
                this.itemLoading = false;
                break;

            case "DELETETEMPLATE":

                this.initialTemplateList = JSON.parse(s.cpTemplatesLst);
                var cItem = null;
                if (this.initialTemplateList.length > 0) {
                    cItem = this.initialTemplateList[0];
                }

                this.configuration.show(this, this.initialTemplateList);
                if (cItem != null) this.configuration.dxTemplateList.selectItem(cItem);
                this.itemLoading = false;

                break;
            case "RELOADTEMPLATES":
                this.initialTemplateList = JSON.parse(s.cpTemplatesLst);
                this.selectedItem = null;
                this.configuration.show(this, this.initialTemplateList);
                this.itemLoading = false;
                break;
            default:
        }
    }
}

templateManagementPopup.prototype.saveCurrentTemplate = function (s) {
    this.itemLoading = true;
    var oParameters = {};
    oParameters.action = "SAVETEMPLATE";

    var nodes = [];

    var ctl = this;

    Object.keys(ctl.calSelectedDays).forEach(function (key) {
        if (ctl.calSelectedDays[key].selected) {
            nodes.push({ ScheduleDate: moment(key, "YYYYMMDD").toDate(), Description: ctl.calSelectedDays[key].description });
        }
    });

    ctl.selectedItem.ScheduleDates = nodes;
    if (ctl.selectedItem.id != -1) { ctl.selectedItem.Name = ctl.configuration.updatemplateName; }

    var saveNode = [];
    saveNode.push(ctl.selectedItem);

    oParameters.templates = saveNode;
    oParameters.StampParam = new Date().getMilliseconds();
    var strParameters = JSON.stringify(oParameters);
    strParameters = encodeURIComponent(strParameters);

    PerformActionCallbackClient.PerformCallback(strParameters);
}

templateManagementPopup.prototype.checkResult = function (oResult) {
    if (oResult.cpResult == 'KO') {
        DevExpress.ui.dialog.alert(oResult.cpMessage, Globalize.formatMessage("roError"));
        return false;
    }
    return true;
}

templateManagementPopup.prototype.drawSelectedItem = function () {
    if (this.selectedItem != null) {
        this.calSelectedDays = [];
        this.selectedItem.hasChanges = false;

        for (var i = 0; i < this.selectedItem.ScheduleDates.length; i++) {
            this.calSelectedDays[moment(this.selectedItem.ScheduleDates[i].ScheduleDate).format("YYYYMMDD")] = {
                selected: true,
                description: this.selectedItem.ScheduleDates[i].Description
            }
        }

        this.configuration.dxTemplateCalendar = $('#dxTemplatesDays').dxCalendar({
            width: 315,
            useCellTemplate: true,
            cellTemplate: this.getCellTemplate(this),
            firstDayOfWeek: moment()._locale._week.dow,
            maxZoomLevel: 'month',
            zoomLevel: 'year',
            minZoomLevel: 'year',
            onCellClick: this.onCellClicked(this),
            showTodayButton: false,
            min: this.minDate,
            max: this.maxDate
        }).dxCalendar('instance');

        this.configuration.ckIsFeast = $("#dxCkIsFeast").dxCheckBox({
            text: Globalize.formatMessage("roIsFeastTemplate"),
            value: this.selectedItem.FeastTemplate,
            onValueChanged: this.changeTemplateType(this)
        }).dxCheckBox('instance');

        var ctlPopup = this;

        ctlPopup.configuration.updatemplateName = ctlPopup.selectedItem.Name;

        ctlPopup.configuration.dxUpdateName = $('#dxUpdateNameText').dxTextBox({
            placeholder: Globalize.formatMessage("roUpdateTemplate"),
            mode: "text",
            value: this.selectedItem.Name,
            valueChangeEvent: "keyup",
            onValueChanged: function (e) {
                ctlPopup.configuration.updatemplateName = e.value;
                if (!ctlPopup.itemLoading) {
                    ctlPopup.selectedItem.hasChanges = true;
                    ctlPopup.configuration.dxSaveButton.option('disabled', false);
                }
            }
        }).dxTextBox('instance');

        ctlPopup.configuration.dxSaveButton = $('#imgBtnSave').dxButton({
            icon: "save",
            disabled: true,
            onClick: function () {
                ctlPopup.saveCurrentTemplate();
            }
        }).dxButton("instance");
    }
}

templateManagementPopup.prototype.changeTemplateType = function (templatemanager) {
    return function (e) {
        if (!templatemanager.itemLoading) {
            templatemanager.selectedItem.hasChanges = true;
            templatemanager.configuration.dxSaveButton.option('disabled', false);
            templatemanager.selectedItem.FeastTemplate = e.value;
        }
    }
}

templateManagementPopup.prototype.init = function () {
    var editTemplatesPopup = this.configuration;

    this.configuration.popup = $('#templateManagerContainer').dxPopup({
        fullScreen: false,
        width: 600,
        height: 400,
        showTitle: true,
        title: Globalize.formatMessage("roTemplatesManager"),
        visible: false,
        dragEnabled: true,
        hideOnOutsideClick: false
    }).dxPopup("instance");

    this.configuration.show = function (ctlPopup, templatesList) {
        ctlPopup.configuration.newTemplateName = '';
        ctlPopup.configuration.params = {
            lstTemplates: templatesList
        };

        ctlPopup.configuration.popup.show();

        ctlPopup.configuration.dxnewElement = $('#dxAddTemplateText').dxTextBox({
            placeholder: Globalize.formatMessage("roNewTemplate"),
            mode: "text",
            valueChangeEvent: "keyup",
            onValueChanged: function (e) {
                ctlPopup.configuration.newTemplateName = e.value;
            }
        }).dxTextBox('instance');

        ctlPopup.configuration.dxAcceptButton = $('#imgBtnAdd').dxButton({
            icon: "plus",
            onClick: function () {
                if (ctlPopup.configuration.newTemplateName != '') {
                    ctlPopup.calSelectedDays = [];
                    ctlPopup.selectedItem = { id: -1, Name: ctlPopup.configuration.newTemplateName, FeastTemplate: false, ScheduleDates: [], Mode: 2, hasChanges: true }
                    ctlPopup.saveCurrentTemplate();
                    ctlPopup.configuration.newTemplateName = '';
                    ctlPopup.configuration.dxnewElement.option('value', ctlPopup.configuration.newTemplateName);

                    //ctlPopup.configuration.params.lstTemplates.push(ctlPopup.selectedItem)
                    //ctlPopup.configuration.updatemplateName = ctlPopup.configuration.newTemplateName;
                    //ctlPopup.configuration.dxTemplateList.reload();
                    //ctlPopup.configuration.dxTemplateList.selectItem(ctlPopup.configuration.params.lstTemplates.length - 1);
                    //ctlPopup.drawSelectedItem();
                    //ctlPopup.configuration.dxSaveButton.option('disabled', false);
                }
            }
        }).dxButton("instance");

        ctlPopup.configuration.dxTemplateList = $('#dxTemplatesList').dxList({
            editEnabled: true,
            selectionMode: "single",
            dataSource: ctlPopup.configuration.params.lstTemplates,
            height: 300,
            allowItemDeleting: true,
            itemDeleteMode: "static",
            itemTemplate: function (data, index) {
                return $("<div>").append($('<span>').text(data.Name));
            },
            onItemDeleting: ctlPopup.deleteNode(ctlPopup),
            onSelectionChanged: ctlPopup.listItemChanged(ctlPopup)
        }).dxList("instance");

        ctlPopup.configuration.btnAccept = $('#btnOkEditNode').dxButton({
            text: Globalize.formatMessage("roCloseDialog"),
            onClick: function () {
                if (ctlPopup.selectedItem != null && ctlPopup.selectedItem.hasChanges) {
                    var result = DevExpress.ui.dialog.confirm(Globalize.formatMessage("roHasChanges"), Globalize.formatMessage("roQuestionTitle"));
                    result.done(function (dialogResult) {
                        if (dialogResult) {
                            ctlPopup.configuration.popup.hide();
                            reloadTemplates();
                        }
                    });
                } else {
                    ctlPopup.configuration.popup.hide();
                    reloadTemplates();
                }
            }
        }).dxButton("instance");

        if (templatesList.length > 0) {
            ctlPopup.selectedItem = templatesList[0];
            ctlPopup.configuration.dxTemplateList.selectItem(0);
        }
    }
}

templateManagementPopup.prototype.deleteNode = function (ctlPopup) {
    return function (e) {
        var deleteId = e.itemData.ID;
        var result = DevExpress.ui.dialog.confirm(Globalize.formatMessage("roTemplateDelete"), Globalize.formatMessage("roQuestionTitle"));
        result.done(function (dialogResult) {
            if (dialogResult) {
                this.itemLoading = true;
                var oParameters = {};
                oParameters.action = "DELETETEMPLATE";

                var deleteNode = [{ ID: deleteId }];

                oParameters.templates = deleteNode;
                oParameters.StampParam = new Date().getMilliseconds();
                var strParameters = JSON.stringify(oParameters);
                strParameters = encodeURIComponent(strParameters);

                PerformActionCallbackClient.PerformCallback(strParameters);
            } else {
                var oParameters = {};
                oParameters.action = "RELOADTEMPLATES";

                oParameters.templates = [];
                oParameters.StampParam = new Date().getMilliseconds();
                var strParameters = JSON.stringify(oParameters);
                strParameters = encodeURIComponent(strParameters);

                PerformActionCallbackClient.PerformCallback(strParameters);
            }
        });
    }
}

templateManagementPopup.prototype.listItemChanged = function (ctlPopup) {
    return function (e) {
        if (ctlPopup.selectedItem != null) {
            //if (!ctlPopup.selectedItem.hasChanges) {
            if (e.addedItems.length > 0) {
                ctlPopup.selectedItem = e.addedItems[0];
            } else {
                ctlPopup.selectedItem = null;
            }
            ctlPopup.itemLoading = true;
            ctlPopup.drawSelectedItem();
            ctlPopup.itemLoading = false;
            //}
        }
    }
}

templateManagementPopup.prototype.getCellColor = function (calSelectedDays, cDate, period) {
    var color = "#dcdcdc";
    if (period == 'month') {
        if (typeof calSelectedDays[moment(cDate).format("YYYYMMDD")] != 'undefined' && calSelectedDays[moment(cDate).format("YYYYMMDD")].selected == true) {
            color = '#FF5C35';
        }
    } else if (period == 'year') {
        var checkInitial = moment(cDate).startOf('month');
        var checkEnd = moment(cDate).endOf('month');

        while (checkInitial < checkEnd) {
            calSelectedDays[checkInitial.format("YYYYMMDD")]

            if (typeof calSelectedDays[checkInitial.format("YYYYMMDD")] != 'undefined' && calSelectedDays[checkInitial.format("YYYYMMDD")].selected == true) {
                color = '#FF5C35';
                break;
            }
            checkInitial = checkInitial.add(1, 'day');
        }
    }

    return color;
}

templateManagementPopup.prototype.getCellTemplate = function (templatemanager) {
    return function (data, itemIndex, itemElement) {
        var style = '', color = "#dcdcdc", absenceColor = "#dcdcdc", textColor = '#000000';
        var iGradient = 90, eGradient = 95;
        var workingHous = 0;
        var actuallyHolidays = false;

        var actualDayClass = '';
        if (moment(data.date).format("YYYYMMDD") == moment().format("YYYYMMDD")) actualDayClass = 'actualDate';

        color = templatemanager.getCellColor(templatemanager.calSelectedDays, data.date, data.view);

        absenceStyle = 'width:100%;height:100%;color: ' + absenceColor + ';';
        absenceStyle += 'background: ' + absenceColor + ';';
        absenceStyle += 'background: -webkit-radial-gradient(circle closest-side, ' + absenceColor + ' 95%,rgba(255,255,255,0) 100%);';
        absenceStyle += 'background: -moz-radial-gradient(circle closest-side, ' + absenceColor + ' 95%,rgba(255,255,255,0) 100%);';
        absenceStyle += 'background: -ms-radial-gradient(circle closest-side, ' + absenceColor + ' 95%,rgba(255,255,255,0) 100%);';
        absenceStyle += 'background: -o-radial-gradient(circle closest-side, ' + absenceColor + ' 95%,rgba(255,255,255,0) 100%);';
        absenceStyle += 'background: radial-gradient(circle closest-side at center, ' + absenceColor + ' 95%,rgba(255,255,255,0) 100%);';
        absenceStyle += 'background-repeat: no-repeat;';
        absenceStyle += 'background-position: center center;display:table;';//font-weight:bold;';

        style = 'width:100%;height:100%;color: ' + textColor + ';';
        style += 'background: ' + color + ';';
        style += 'background: -webkit-radial-gradient(circle closest-side, ' + color + ' ' + iGradient + '%,rgba(255,255,255,0) ' + eGradient + '%);';
        style += 'background: -moz-radial-gradient(circle closest-side, ' + color + ' ' + iGradient + '%,rgba(255,255,255,0) ' + eGradient + '%);';
        style += 'background: -ms-radial-gradient(circle closest-side, ' + color + ' ' + iGradient + '%,rgba(255,255,255,0) ' + eGradient + '%);';
        style += 'background: -o-radial-gradient(circle closest-side, ' + color + ' ' + iGradient + '%,rgba(255,255,255,0) ' + eGradient + '%);';
        style += 'background: radial-gradient(circle closest-side at center, ' + color + ' ' + iGradient + '%,rgba(255,255,255,0) ' + eGradient + '%);';
        style += 'background-repeat: no-repeat;';
        style += 'background-position: center center;display:table;';

        if (data.view == 'month') {
            if ($(itemElement).attr('class').indexOf('other') == -1) {
                //if (moment(data.date).isAfter(moment(templatemanager.minDate).startOf('month').add(-1, 'day')) && moment(data.date).isBefore(moment(templatemanager.maxDate))) {
                if (moment(data.date).startOf('day') >= moment(templatemanager.minDate).startOf('day')) {
                    return "<div style='width:100%;height:100%'><div class='innerCalCellTemplate' style='" + absenceStyle + "'><div style='display:table-cell;vertical-align:middle'><div id='" + moment(data.date).format("YYYYMMDD") + "' style='" + style + "'> <span class='" + actualDayClass + "' style='display:table-cell;vertical-align:middle;font-weight:bold'>" + data.text + "</span></div></div></div></div>";
                } else {
                    return "<span>" + data.text + "</span>";
                }
            } else {
                return "";
            }
        } else if (data.view == 'year') {
            //if (moment(data.date).isAfter(moment(templatemanager.minDate).startOf('month').add(-1, 'day')) && moment(data.date).isBefore(moment(templatemanager.maxDate))) {
            if ($(itemElement).attr('class').indexOf('other') == -1) {
                var checkInitial = moment(data.date).startOf('month');
                var checkEnd = moment(data.date).endOf('month');
                var bCounter = 0;

                while (checkInitial < checkEnd) {
                    if (typeof templatemanager.calSelectedDays[checkInitial.format("YYYYMMDD")] != 'undefined' && templatemanager.calSelectedDays[checkInitial.format("YYYYMMDD")].selected == true) bCounter += 1;

                    checkInitial = checkInitial.add(1, 'day');
                }

                if (bCounter > 0) return "<div style='width:100%;height:100%'><div class='innerCalCellTemplate' style='" + absenceStyle + "'><div style='display:table-cell;vertical-align:middle'><div id='" + moment(data.date).format("YYYYMMDD") + "' style='" + style + "'> <span style='display:table-cell;vertical-align:middle'>" + data.text.capitalize(true, true) + " (" + bCounter + ")</span></div></div></div></div>";
                else return "<div style='width:100%;height:100%'><div class='innerCalCellTemplate' style='" + absenceStyle + "'><div style='display:table-cell;vertical-align:middle'><div id='" + moment(data.date).format("YYYYMMDD") + "' style='" + style + "'> <span style='display:table-cell;vertical-align:middle'>" + data.text.capitalize(true, true) + "</span></div></div></div></div>";
            } else {
                return "";
            }
        }
    }
}

templateManagementPopup.prototype.onCellClicked = function (templatemanager) {
    return function (e) {
        var color = '#FF5C35', textColor = '#000000', iGradient = 90, eGradient = 95;

        templatemanager.selectedItem.hasChanges = true;
        templatemanager.configuration.dxSaveButton.option('disabled', false);

        if (e.element.attr("class").indexOf('other') == -1) {
            if (typeof templatemanager.calSelectedDays[moment(e.value).format("YYYYMMDD")] != 'undefined' && templatemanager.calSelectedDays[moment(e.value).format("YYYYMMDD")].selected == true) {
                if (templatemanager.selectedItem.FeastTemplate) {
                    templatemanager.showCellProperties(templatemanager, moment(e.value).format("YYYYMMDD"));
                } else {
                    templatemanager.configuration.dayEditPopover = $('#cellEditPopover').dxPopover({
                        target: '#19000101',
                        showEvent: "dxclick",
                        position: "bottom",
                        width: 300,
                        shading: true
                    }).dxPopover("instance");

                    templatemanager.calSelectedDays[moment(e.value).format("YYYYMMDD")].selected = false;
                    templatemanager.calSelectedDays[moment(e.value).format("YYYYMMDD")].description = '';

                    color = "#dcdcdc";
                    var style = 'width:100%;height:100%;color: ' + textColor + ';';
                    style += 'background: ' + color + ';';
                    style += 'background: -webkit-radial-gradient(circle closest-side, ' + color + ' ' + iGradient + '%,rgba(255,255,255,0) ' + eGradient + '%);';
                    style += 'background: -moz-radial-gradient(circle closest-side, ' + color + ' ' + iGradient + '%,rgba(255,255,255,0) ' + eGradient + '%);';
                    style += 'background: -ms-radial-gradient(circle closest-side, ' + color + ' ' + iGradient + '%,rgba(255,255,255,0) ' + eGradient + '%);';
                    style += 'background: -o-radial-gradient(circle closest-side, ' + color + ' ' + iGradient + '%,rgba(255,255,255,0) ' + eGradient + '%);';
                    style += 'background: radial-gradient(circle closest-side at center, ' + color + ' ' + iGradient + '%,rgba(255,255,255,0) ' + eGradient + '%);';
                    style += 'background-repeat: no-repeat;';
                    style += 'background-position: center center;display:table;';

                    $('#' + moment(e.value).format("YYYYMMDD")).attr('style', style);
                }
            } else {
                color = '#FF5C35';
                templatemanager.calSelectedDays[moment(e.value).format("YYYYMMDD")] = { selected: true, description: '' };
                if (templatemanager.selectedItem.FeastTemplate) {
                    templatemanager.showCellProperties(templatemanager, moment(e.value).format("YYYYMMDD"));
                } else {
                    templatemanager.configuration.dayEditPopover = $('#cellEditPopover').dxPopover({
                        target: '#19000101',
                        showEvent: "dxclick",
                        position: "bottom",
                        width: 300,
                        shading: true
                    }).dxPopover("instance");
                }

                var style = 'width:100%;height:100%;color: ' + textColor + ';';
                style += 'background: ' + color + ';';
                style += 'background: -webkit-radial-gradient(circle closest-side, ' + color + ' ' + iGradient + '%,rgba(255,255,255,0) ' + eGradient + '%);';
                style += 'background: -moz-radial-gradient(circle closest-side, ' + color + ' ' + iGradient + '%,rgba(255,255,255,0) ' + eGradient + '%);';
                style += 'background: -ms-radial-gradient(circle closest-side, ' + color + ' ' + iGradient + '%,rgba(255,255,255,0) ' + eGradient + '%);';
                style += 'background: -o-radial-gradient(circle closest-side, ' + color + ' ' + iGradient + '%,rgba(255,255,255,0) ' + eGradient + '%);';
                style += 'background: radial-gradient(circle closest-side at center, ' + color + ' ' + iGradient + '%,rgba(255,255,255,0) ' + eGradient + '%);';
                style += 'background-repeat: no-repeat;';
                style += 'background-position: center center;display:table;';

                $('#' + moment(e.value).format("YYYYMMDD")).attr('style', style);
            }
        }
    }
};

templateManagementPopup.prototype.showCellProperties = function (ctlPopup, cellKey) {
    ctlPopup.configuration.dayEditPopover = $('#cellEditPopover').dxPopover({
        target: '#' + cellKey,
        showEvent: "dxclick",
        position: "bottom",
        width: 300,
        shading: true
    }).dxPopover("instance");

    ctlPopup.popoverCellKey = cellKey;

    setTimeout(function () {
        ctlPopup.configuration.dxUpdateName = $('#dxDayDescriptionText').dxTextBox({
            placeholder: Globalize.formatMessage("roDayDescription"),
            mode: "text",
            value: ctlPopup.calSelectedDays[ctlPopup.popoverCellKey].description,
            valueChangeEvent: "keyup",
            onValueChanged: function (e) {
                ctlPopup.calSelectedDays[ctlPopup.popoverCellKey].description = e.value;
                if (!ctlPopup.itemLoading) {
                    ctlPopup.selectedItem.hasChanges = true;
                    ctlPopup.configuration.dxSaveButton.option('disabled', false);
                }
            }
        }).dxTextBox('instance');

        ctlPopup.configuration.dxBtnRemoveDay = $('#btnRemoveDay').dxButton({
            icon: "trash",
            onClick: function () {
                var color = '#FF5C35', textColor = '#000000', iGradient = 90, eGradient = 95;

                ctlPopup.calSelectedDays[ctlPopup.popoverCellKey].selected = false;
                ctlPopup.calSelectedDays[ctlPopup.popoverCellKey].description = '';
                color = "#dcdcdc";
                var style = 'width:100%;height:100%;color: ' + textColor + ';';
                style += 'background: ' + color + ';';
                style += 'background: -webkit-radial-gradient(circle closest-side, ' + color + ' ' + iGradient + '%,rgba(255,255,255,0) ' + eGradient + '%);';
                style += 'background: -moz-radial-gradient(circle closest-side, ' + color + ' ' + iGradient + '%,rgba(255,255,255,0) ' + eGradient + '%);';
                style += 'background: -ms-radial-gradient(circle closest-side, ' + color + ' ' + iGradient + '%,rgba(255,255,255,0) ' + eGradient + '%);';
                style += 'background: -o-radial-gradient(circle closest-side, ' + color + ' ' + iGradient + '%,rgba(255,255,255,0) ' + eGradient + '%);';
                style += 'background: radial-gradient(circle closest-side at center, ' + color + ' ' + iGradient + '%,rgba(255,255,255,0) ' + eGradient + '%);';
                style += 'background-repeat: no-repeat;';
                style += 'background-position: center center;display:table;';

                $('#' + ctlPopup.popoverCellKey).attr('style', style);
                ctlPopup.configuration.dayEditPopover.hide();
            }
        }).dxButton("instance");
    }, 200);
}