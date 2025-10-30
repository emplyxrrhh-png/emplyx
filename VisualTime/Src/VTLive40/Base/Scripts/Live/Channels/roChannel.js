var currentChannelView = null;
var hasChanges = false;
var bSendingMessage = false;

$(document).ready(function () {
    window.loadConversation = loadSelectedConversation;
});

function toolbar_preparing(e) {
}

function getWidth() {
    return document.documentElement.clientWidth - 40;
}

function getHeight() {
    return document.documentElement.clientHeight - 40;
}

function onChannelChange() {
    hasChanges = true;
    $("#ChannelRestrict").hide();
    $("#ChannelSend").hide();
}

function getValidationWidth() {
    return 600;
}

function getValidationHeight() {
    return 600;
}

function getHeightResults() {
    return document.documentElement.clientHeight - 150;
}

function addNewChannel() {
    currentChannelView = { Employees: [], Groups: [], Id: 0, AllowAnonymous: 1 }
    $("#newChannelPopup").dxPopup("instance").show();
}

function restrictChannel() {
    $.ajax({
        type: "POST",
        url: BASE_URL + 'Channels/RestrictChannel',
        dataType: "json",
        data: { Id: currentChannelView.Id },
        success: function (e) {
            if (e == true) {
                DevExpress.ui.notify(viewUtilsManager.DXTranslate('roChannelRestricted'), "success", 2000);
                $("#newChannelPopup").dxPopup("instance").hide();
                $("#gridStatusChannels").dxDataGrid("instance").refresh();
            } else DevExpress.ui.notify(viewUtilsManager.DXTranslate('roChannelSendError'), "error", 2000);
        },
        error: function (e) { DevExpress.ui.notify(viewUtilsManager.DXTranslate('roChannelUnkown'), "error", 2000); }
    });
}

function publishChannel() {
    if (currentChannelView.IsComplaintChannel) {
        saveComplaintChannel("publish");
    } else {
        confirmPublishChannel();
    }
}

function confirmPublishChannel() {
    $.ajax({
        type: "POST",
        url: BASE_URL + 'Channels/PublishChannel',
        dataType: "json",
        data: { Id: currentChannelView.Id },
        success: function (e) {
            if (e == true) {
                DevExpress.ui.notify(viewUtilsManager.DXTranslate('roChannelSent'), "success", 2000);
                $("#validatePublish").dxPopup("instance").hide();
                $("#newChannelPopup").dxPopup("instance").hide();
                $("#gridStatusChannels").dxDataGrid("instance").refresh();
            } else DevExpress.ui.notify(viewUtilsManager.DXTranslate('roChannelSendError'), "error", 2000);
        },
        error: function (e) { DevExpress.ui.notify(viewUtilsManager.DXTranslate('roChannelSendError'), "error", 2000); }
    });
}

function cancelPublishChannel() {
    $("#validatePublish").dxPopup("instance").hide();
}

function createNewChannel() {
    disableSaveButton();
    var title = $("#ChannelName").dxTextBox("instance").option("value");
    var supervisors = $("#ChannelSupervisorsText").dxTagBox("instance").option("value");
    var anonymous = $("#ChannelAnonymous").dxSwitch("instance").option("value");
    var automatic = $("#ChannelReceiptAknowledgment").dxSwitch("instance").option("value");
    var id = currentChannelView.Id;
    if (typeof id == 'undefined') {
        id = 0;
    }

    var idEmployees = [];
    var idGroups = []

    if (typeof currentChannelView != 'undefined' && currentChannelView.Employees != null && currentChannelView.Employees.length > 0) {
        if (typeof currentChannelView.Employees[0].IdEmployee == 'undefined') {
            idEmployees = currentChannelView.Employees;
        }
        else {
            idEmployees = currentChannelView.Employees.map(a => a.IdEmployee);
        }
    }

    if (typeof currentChannelView != 'undefined' && currentChannelView.Groups != null && currentChannelView.Groups.length > 0) {
        if (typeof currentChannelView.Groups[0].IdGroup == 'undefined') {
            idGroups = currentChannelView.Groups;
        }
        else {
            idGroups = currentChannelView.Groups.map(a => a.IdGroup);
        }
    }

    if (title != null && title != "") {
        $.ajax({
            type: "POST",
            url: BASE_URL + 'Channels/InsertChannel',
            dataType: "json",
            data: { Id: id, Employees: idEmployees, Groups: idGroups, Title: title, Anonymous: anonymous, SubscribedSupervisors: supervisors, Automatic: automatic },
            success: function (e) {
                if (typeof e == 'string') {
                    enableSaveButton();
                    DevExpress.ui.notify(e, "error", 5000);
                } else {
                    enableSaveButton();
                    DevExpress.ui.notify(viewUtilsManager.DXTranslate('roChannelSaved'), "success", 2000);
                    currentChannelView = e;
                    hasChanges = false;

                    let currentSelectorView = { Employees: [], Groups: [], Filter: "11110", UserFields: "", ComposeMode: 'Custom', ComposeFilter: "", Advanced: false, Operation: "or" };

                    if (currentChannelView != null) {
                        if (currentChannelView.Employees != null) {
                            for (let i = 0; i < currentChannelView.Employees.length; i++) currentSelectorView.ComposeFilter += currentSelectorView.ComposeFilter == '' ? `B${currentChannelView.Employees[i]}` : `,B${currentChannelView.Employees[i]}`;
                        }

                        if (currentChannelView.Groups != null) {
                            for (let i = 0; i < currentChannelView.Groups.length; i++) currentSelectorView.ComposeFilter += currentSelectorView.ComposeFilter == '' ? `A${currentChannelView.Groups[i]}` : `,A${currentChannelView.Groups[i]}`;
                        }
                    }

                    $("#ChannelDestination").dxTextBox("instance").option("value", buildSelectedEmployeesString(currentSelectorView));
                    setTabsVisibility();
                    showConfigurationTab();
                }
            },
            error: function (e) {
                enableSaveButton();
                DevExpress.ui.notify(viewUtilsManager.DXTranslate('roChannelSaveError'), "error", 5000);
            }
        });
    }
    else {
        enableSaveButton();
        DevExpress.ui.notify(viewUtilsManager.DXTranslate('roChannelSaveErrorTitle'), "error", 3000);
    }
}

function onValidateShown(s, e) {
    $("#dexConditions").load('/Channels/DEXUseAndConditions', function () {
        let pdfobj = $('<object>');
        pdfobj.attr("data", "/Channels/UseAndConditionsPDF");
        pdfobj.attr("type", "application/pdf");
        pdfobj.addClass("w100");

        $("#pdfdiv_content").append(pdfobj);
    });
}

function saveComplaintChannel(publish = null) {
    disableSaveComplaintButton();

    var privacyPolicy = $("#ChannelPolicy").dxHtmlEditor("instance").option("value");
    var id = currentChannelView.Id;
    if (typeof id == 'undefined') {
        id = 0;
    }

    if (privacyPolicy != null && privacyPolicy != "") {
        $.ajax({
            type: "POST",
            url: BASE_URL + 'Channels/SaveComplaintChannel',
            dataType: "json",
            contentType: "application/json",
            data: JSON.stringify({ Id: id, PrivacyPolicy: privacyPolicy }),
            success: function (e) {
                if (typeof e == 'string') {
                    enableSaveComplaintButton();
                    DevExpress.ui.notify(e, "error", 5000);
                } else {
                    enableSaveComplaintButton();
                    DevExpress.ui.notify(viewUtilsManager.DXTranslate('roChannelComplaintSaved'), "success", 2000);
                    currentChannelView = e;
                    hasChanges = false;

                    if (publish != null && publish == "publish") {
                        $("#validatePublish").dxPopup("instance").show();
                    }
                }
            },
            error: function (e) {
                enableSaveComplaintButton();
                DevExpress.ui.notify(viewUtilsManager.DXTranslate('roChannelComplaintSaveError'), "error", 5000);
            }
        });
    }
    else {
        enableSaveComplaintButton();
        DevExpress.ui.notify(viewUtilsManager.DXTranslate('roChannelSaveErrorPolicy'), "error", 3000);
    }
}

function showSaveButton() {
    $("#ChannelSave").show();
}

function showSaveComplaintButton() {
    $("#ChannelSaveComplaint").show();
}

function disableSaveButton() {
    $("#addNewChannel").dxButton("instance").option("disabled", true);
}

function enableSaveButton() {
    $("#addNewChannel").dxButton("instance").option("disabled", false);
}

function disableSaveComplaintButton() {
    $("#saveComplaintChannel").dxButton("instance").option("disabled", true);
}

function enableSaveComplaintButton() {
    $("#saveComplaintChannel").dxButton("instance").option("disabled", false);
}

function setTabsVisibility() {
    $("#ChannelConversations").hide();
    $("#ChannelConversations").addClass("bTabChannels");
    $("#ChannelConversations").removeClass("bTabChannels-active");
    $("#ChannelConfiguration").hide();
    $("#ChannelConfiguration").addClass("bTabChannels");
    $("#ChannelConfiguration").removeClass("bTabChannels-active");
    $("#configDiv").hide();
    $("#ChannelDiv").hide();
    $("#ChannelSend").hide();
    $("#ChannelSave").hide();
    $("#ChannelSaveComplaint").hide();
    $("#ChannelRestrict").hide();
    $('#onlyEditButtons').hide();
    $("#configComplaint").hide();
    $("#configChannels").hide();

    if (currentChannelView != null) {
        if (currentChannelView.Id > 0) {
            // Estoy editando
            if (currentChannelView.IsComplaintChannel) {
                $("#ChannelConfiguration").show();
                $("#ChannelSaveComplaint").show();
                $("#configComplaint").show();
                if (currentChannelView.Status == 0) {
                    $('#onlyEditButtons').show();
                    $("#ChannelSend").show();
                    $("#ChannelConfiguration").addClass("bTabChannels-active");
                    $("#configComplaint").show();
                    $("#configChannels").hide();
                    $("#configDiv").show();
                } else {
                    $("#ChannelConversations").show();
                    $("#ChannelConversations").addClass("bTabChannels-active");
                    $("#ChannelDiv").show();
                }
            } else if (Permission < 9 && currentChannelView.CreatedBy != idPassport) {
                $("#ChannelConversations").show();
                $("#ChannelConfiguration").addClass("bTabChannels-active")
                $("#ChannelDiv").show();
                $("#ChannelSend").hide();
            } else {
                if (currentChannelView.SubscribedSupervisors != null && currentChannelView.SubscribedSupervisors.find(function (x) { return x == idPassport }) != null) {
                    $("#ChannelConfiguration").show();
                    $("#ChannelConversations").show();
                    $("#ChannelSave").show();

                    if (currentChannelView.Status == 1) {
                        $("#ChannelConversations").addClass("bTabChannels-active");
                        $('#onlyEditButtons').hide();
                        $("#ChannelSend").hide();
                        $("#ChannelDiv").show();
                        $("#ChannelRestrict").show();
                    } else {
                        $("#ChannelConfiguration").addClass("bTabChannels-active");
                        $('#onlyEditButtons').show();
                        $("#configChannels").show();
                        $("#configDiv").show();
                        $("#ChannelRestrict").hide();
                        if ((currentChannelView.Employees != null && currentChannelView.Employees.length > 0) || (currentChannelView.Groups != null && currentChannelView.Groups.length > 0))
                            $("#ChannelSend").show();
                    }
                } else {
                    $("#ChannelConfiguration").show();
                    $("#ChannelConversations").hide();
                    $("#ChannelSave").show();

                    $("#ChannelConfiguration").addClass("bTabChannels-active");
                    $('#onlyEditButtons').show();
                    $("#configChannels").show();
                    $("#configDiv").show();

                    if (currentChannelView.Status == 1) {
                        $("#ChannelRestrict").show();
                    } else {
                        $("#ChannelRestrict").hide();
                        if ((currentChannelView.Employees != null && currentChannelView.Employees.length > 0) || (currentChannelView.Groups != null && currentChannelView.Groups.length > 0))
                            $("#ChannelSend").show();
                    }
                }
            }
        } else {
            $('#onlyEditButtons').show();
            $("#configChannels").show();
            $("#configDiv").show();
            $("#ChannelConfiguration").show();
            $("#ChannelConfiguration").addClass("bTabChannels-active")
            $("#ChannelSave").show();

            if ((currentChannelView.Employees != null && currentChannelView.Employees.length > 0) || (currentChannelView.Groups != null && currentChannelView.Groups.length > 0)) $("#ChannelSend").show();
            else $("#ChannelSend").hide();
        }
    } else {
        DevExpress.ui.notify(viewUtilsManager.DXTranslate('roError'), "error", 2000);
    }
}

function showConversationsTab() {
    $("#configDiv").hide();
    $("#ChannelDiv").show();
    $("#ChannelConfiguration").removeClass("bTabChannels-active");
    $("#ChannelConversations").addClass("bTabChannels-active");
    $("#ChannelConfiguration").addClass("bTabChannels");
    $('#onlyEditButtons').hide();
}

function showConfigurationTab() {
    $("#configDiv").show();
    $("#ChannelDiv").hide();
    $("#ChannelConfiguration").addClass("bTabChannels-active");
    $("#ChannelConversations").removeClass("bTabChannels-active");
    $("#ChannelConversations").addClass("bTabChannels");
    $('#onlyEditButtons').show();
}

function initializeEmployeeTextFields() {
    if ((currentChannelView != null && currentChannelView.Employees != null && currentChannelView.Employees != '') || (currentChannelView != null && currentChannelView.Groups != null && currentChannelView.Groups != '')) {
        let currentSelectorView = { Employees: [], Groups: [], Filter: "11110", UserFields: "", ComposeMode: 'Custom', ComposeFilter: "", Advanced: false, Operation: "or" };

        if (currentChannelView != null) {
            if (currentChannelView.Employees != null) {
                for (let i = 0; i < currentChannelView.Employees.length; i++) currentSelectorView.ComposeFilter += currentSelectorView.ComposeFilter == '' ? `B${currentChannelView.Employees[i]}` : `,B${currentChannelView.Employees[i]}`;
            }

            if (currentChannelView.Groups != null) {
                for (let i = 0; i < currentChannelView.Groups.length; i++) currentSelectorView.ComposeFilter += currentSelectorView.ComposeFilter == '' ? `A${currentChannelView.Groups[i]}` : `,A${currentChannelView.Groups[i]}`;
            }
        }

        $("#ChannelDestination").dxTextBox("instance").option("value", buildSelectedEmployeesString(currentSelectorView));
    } else $("#ChannelDestination").dxTextBox("instance").option("value", null);
}

function onChannelPopupShown() {
    hasChanges = false;

    $("#loading").dxLoadIndicator("instance").option("visible", false);
    $("#ChannelName").dxTextBox("instance").option("value", "");
    $("#ChannelSupervisorsText").dxTagBox("instance").option("value", []);
    $("#ChannelAnonymous").dxSwitch("instance").option("value", "");
    $("#ChannelReceiptAknowledgment").dxSwitch("instance").option("value", "");
    $("#ChannelDestination").dxTextBox("instance").option("value", "");
    $("#ChannelName").dxTextBox("instance").option("disabled", false);
    $("#ChannelAnonymous").dxSwitch("instance").option("disabled", false);
    $("#ChannelPolicy").dxHtmlEditor("instance").option("value", "");
    $("#ChannelSend").hide();
    $("#configComplaint").hide();
    $("#configChannels").hide();

    $('#ChannelDestination').off("click");
    $('#ChannelDestination').on("click", function (e) {
        let currentSelectorView = { Employees: [], Groups: [], Filter: "11110", UserFields: "", ComposeMode: 'Custom', ComposeFilter: "", Advanced: false, Operation: "or" };

        if (currentChannelView != null) {
            if (currentChannelView.Employees != null) {
                for (let i = 0; i < currentChannelView.Employees.length; i++) currentSelectorView.ComposeFilter += currentSelectorView.ComposeFilter == '' ? `B${currentChannelView.Employees[i]}` : `,B${currentChannelView.Employees[i]}`;
            }

            if (currentChannelView.Groups != null) {
                for (let i = 0; i < currentChannelView.Groups.length; i++) currentSelectorView.ComposeFilter += currentSelectorView.ComposeFilter == '' ? `A${currentChannelView.Groups[i]}` : `,A${currentChannelView.Groups[i]}`;
            }
        }

        parent.showLoader(true);
        $("#divChannelsEmployeeSelector").load('/Employee/EmployeeSelectorPartial?feature=&pageName=channels&config=100&unionType=or&advancedMode=1&advancedFilter=0&allowAll=0&allowNone=0', function () {
            loadPartialInfo();
            initUniversalSelector(currentSelectorView, false, 'channelsSelector');
            parent.showLoader(false);
        });
    });

    $("#ChannelDiv").hide();
    $("#ChannelConfiguration").addClass("bTabChannels-active")
    $("#ChannelConversations").removeClass("bTabChannels-active")
    $("#ChannelConversations").addClass("bTabChannels")

    $('#ChannelConfiguration').off("click");
    $('#ChannelConfiguration').on("click", function (e) {
        showConfigurationTab();
    });

    $('#ChannelConversations').off("click");
    $('#ChannelConversations').on("click", function (e) {
        showConversationsTab();
    });

    initializeEmployeeTextFields();

    if (currentChannelView.IsComplaintChannel) {
        $("#ChannelPolicy").dxHtmlEditor("instance").option("value", currentChannelView.PrivacyPolicy);
        $("#ChannelPolicy").dxHtmlEditor("instance").option("toolbar", {
            items: [
                {
                    name: "undo",
                    options: {
                        hint: window.parent.Globalize.formatMessage("dxDiagram-commandUndo")
                    }
                },
                {
                    name: "redo",
                    options: {
                        hint: window.parent.Globalize.formatMessage("dxDiagram-commandRedo")
                    }
                },
                "separator",
                {
                    name: "size",
                    acceptedValues: ["8pt", "10pt", "12pt", "14pt", "18pt", "24pt", "36pt"],
                    options: {
                        placeholder: window.parent.Globalize.formatMessage('dxHtmlEditor-commandFontSize'),
                        hint: window.parent.Globalize.formatMessage("dxHtmlEditor-commandFontSize"),
                        displayExpr: function (item) {
                            return window.parent.Globalize.formatMessage('dxHtmlEditor-commandFontSize')
                        },
                        itemTemplate: function (data, index, element) {
                            element.text(data)
                        }
                    }
                },
                {
                    name: "font",
                    acceptedValues: ["Arial", "Courier New", "Georgia", "Impact", "Lucida Console", "Tahoma", "Times New Roman", "Verdana"],
                    options: {
                        hint: window.parent.Globalize.formatMessage("dxHtmlEditor-commandFontName"),
                        placeholder: window.parent.Globalize.formatMessage('dxHtmlEditor-commandFontName'),
                        displayExpr: function (item) {
                            return window.parent.Globalize.formatMessage('dxHtmlEditor-commandFontName')
                        },
                        itemTemplate: function (data, index, element) {
                            element.text(data)
                        }
                    }
                },
                "separator", {
                    name: "bold",
                    options: {
                        hint: window.parent.Globalize.formatMessage("dxDiagram-commandBold")
                    }
                }, {
                    name: "italic",
                    options: {
                        hint: window.parent.Globalize.formatMessage("dxDiagram-commandItalic")
                    }
                }, {
                    name: "strike",
                    options: {
                        hint: window.parent.Globalize.formatMessage("dxDiagram-commandStrike")
                    }
                }, {
                    name: "underline",
                    options: {
                        hint: window.parent.Globalize.formatMessage("dxDiagram-commandUnderline")
                    }
                }, "separator",
                {
                    name: "alignLeft",
                    options: {
                        hint: window.parent.Globalize.formatMessage("dxDiagram-commandAlignLeft")
                    }
                }, {
                    name: "alignCenter",
                    options: {
                        hint: window.parent.Globalize.formatMessage("dxDiagram-commandAlignCenter")
                    }
                }, {
                    name: "alignRight",
                    options: {
                        hint: window.parent.Globalize.formatMessage("dxDiagram-commandAlignRight")
                    }
                }, {
                    name: "alignJustify",
                    options: {
                        hint: window.parent.Globalize.formatMessage("dxDiagram-commandAlignJustify")
                    }
                }, "separator",
                {
                    name: "orderedList",
                    options: {
                        hint: window.parent.Globalize.formatMessage("dxHtmlEditor-orderedList")
                    }
                }, {
                    name: "bulletList",
                    options: {
                        hint: window.parent.Globalize.formatMessage("dxHtmlEditor-bulletList")
                    }
                }, "separator",
                {
                    name: "header",
                    acceptedValues: [false, 1, 2, 3, 4, 5],
                    options: {
                        placeholder: window.parent.Globalize.formatMessage('dxHtmlEditor-heading'),
                        displayExpr: function (item) {
                            return window.parent.Globalize.formatMessage('dxHtmlEditor-heading')
                        },
                        itemTemplate: function (data, index, element) {
                            if (index === 0) {
                                element.text(window.parent.Globalize.formatMessage("dxHtmlEditor-normalText"))
                            }
                            else {
                                element.text(window.parent.Globalize.formatMessage("dxHtmlEditor-heading" + data))
                            }
                        },
                    }
                }, "separator",
                {
                    name: "color",
                    options: {
                        hint: window.parent.Globalize.formatMessage("dxHtmlEditor-color")
                    }
                }, {
                    name: "background",
                    options: {
                        hint: window.parent.Globalize.formatMessage("dxHtmlEditor-background")
                    }
                }, "separator",
                {
                    name: "link",
                    options: {
                        hint: window.parent.Globalize.formatMessage("dxHtmlEditor-link")
                    }
                }, "separator",
                {
                    name: "clear",
                    options: {
                        hint: window.parent.Globalize.formatMessage("Clear")
                    }
                }]
        });

        $("#configComplaint").show();
    } else {
        $("#configChannels").show();
        $("#ChannelName").dxTextBox("instance").option("value", currentChannelView.Title);
        $("#ChannelSupervisorsText").dxTagBox("instance").option("value", currentChannelView.SubscribedSupervisors);
        $("#ChannelSupervisorsText").dxTagBox("instance").repaint();
        $("#ChannelAnonymous").dxSwitch("instance").option("value", currentChannelView.AllowAnonymous);
        $("#ChannelReceiptAknowledgment").dxSwitch("instance").option("value", currentChannelView.ReceiptAcknowledgment);
    }

    setTabsVisibility();
    $("#ChannelDiv").load('/Channels/GetChannelConversations?idChannel=' + currentChannelView.Id, function () {
        $("#ChannelDiv").css("height", (document.documentElement.clientHeight - 160) + "px");
    });
}

window.parentCloseAndApplySelector = function (currentSelection) {
    currentChannelView.Employees = currentSelection.Employees;
    currentChannelView.Groups = currentSelection.Groups;


    $("#ChannelDestination").dxTextBox("instance").option("value", buildSelectedEmployeesString(currentSelection));

};


function refreshChannels() {
    $("#gridStatusChannels").dxDataGrid("instance").refresh();
}

function onChannelPopupHiding() {
    currentChannelView = null;
    $("#ChannelDiv").html("");
    $('#idChannelSelected').val("");
    $("#gridStatusChannels").dxDataGrid("instance").refresh();
}

function ChannelSelected(selectedItems) {
    if (typeof selectedItems != 'undefined' && typeof selectedItems.row != 'undefined' && selectedItems.row.rowType == "data") {
        $('#idChannelSelected').val(selectedItems.row.data.Id);
    }

    if (typeof $('#idChannelSelected').val() != "undefined" && $('#idChannelSelected').val() != "") {
        $("#loading").dxLoadIndicator("instance").option("visible", true);

        $.ajax({
            type: "POST",
            url: BASE_URL + 'Channels/GetChannel',
            dataType: "json",
            data: { idChannel: $('#idChannelSelected').val() },
            success: function (data) {
                if (typeof e == 'string') {
                    DevExpress.ui.notify(e, "error", 5000);
                } else {
                    currentChannelView = data;
                    $("#loading").dxLoadIndicator("instance").option("visible", false);
                    $("#newChannelPopup").dxPopup("instance").show();
                }
            },
            error: function () {
                currentChannelView = null;
                $("#loading").dxLoadIndicator("instance").option("visible", false);
                DevExpress.ui.notify(viewUtilsManager.DXTranslate('roError'), "error", 2000);
            }
        });
    } else {
        currentChannelView = null;
    }
}

function ChannelRemoved() {
    $('#onboardingRemoved').val("true");
    $.ajax({
        type: "GET",
        url: BASE_URL + 'Channel/LoadInitialData',
        contentType: "application/json; charset=utf-8",
        success: function (data) {
        },
        error: function () { }
    });
}

function context_menu(e) {
}

var headerFilter = {
    load: function (loadOptions) {
        return [{
            text: viewUtilsManager.DXTranslate('roChannelRestrict'),
            value: [['Status', '=', '0']]
        }, {
            text: viewUtilsManager.DXTranslate('roChannelShared'),
            value: [['Status', '=', '1']]
        }];
    }
}

function AllowModify() {
    if (Permission > 3) {
        return true;
    }
    else {
        return false;
    }
}

function onCellPrepared(e) {
    if (e.rowType == "header") {
        e.cellElement.css("text-align", "left");
    }

    if (e.rowType == "data") {
        if (e.data.IsComplaintChannel || (Permission < 9 && e.data.CreatedBy != idPassport))
            e.cellElement.find(".dx-icon-trash").remove();
        if (e.data.IsComplaintChannel || (Permission < 9 && e.data.CreatedBy != idPassport))
            e.cellElement.find(".dx-icon-edit").remove();
    }
}

function progressBar(container, options) {
    if (options.displayValue != null) {
        switch (options.row.data.Status) {
            case 0:
                $("<span style='float: left;background: #FF5C3575;border-radius: 4px;text-align: center;color: white;width: 300px;'> " + viewUtilsManager.DXTranslate('roChannelRestrict') + " </span>").appendTo(container);
                break;
            case 1:
                $("<span style='float: left;background: #0f8ed075;border-radius: 4px;text-align: center;color: white;width: 300px;'> " + viewUtilsManager.DXTranslate('roChannelShared') + " </span>").appendTo(container);
                break;
            default:
        }
    }
}

function cancelChannel() {
    var popup = $("#newChannelPopup").dxPopup("instance");
    popup.hide();
    $("#configComplaint").hide();
    $("#configChannels").hide();
}

function SetGridHeight(e) {
    e.rowElement.css({ height: 70 });
}

function beforeSend(operation, ajaxSettings) {
}
function loadSelectedConversation() {
    $("#divConversationContent").load('/Channels/GetConversationMessages?idConversation=' + viewUtilsManager.getSelectedCardId(), function () { });
}

function changeMsgState(s, e) {
    var iNewStatus = -1;
    switch (s.element[0].id) {
        case "btnMsgPending":
            break;
        case "btnOngoing":
            break;
        case "btnMsgClosed":
            iNewStatus = 3;
            break;
        case "btnMsgRejected":
            iNewStatus = 2;
            break;
        default:
            break;
    }

    if (iNewStatus >= 0) setConverstationStatus(viewUtilsManager.getSelectedCardId(), iNewStatus);
}

function calcMessageWidth() {
    return document.documentElement.clientWidth - 610;
}

function setConverstationStatus(idConversation, iNewStatus) {
    $.ajax({
        type: "POST",
        url: BASE_URL + 'Channels/SetConversationStatus',
        dataType: "json",
        data: { IdConversation: idConversation, Status: iNewStatus },
        success: function (e) {
            if (typeof e == 'string') {
                DevExpress.ui.notify(e, "error", 5000);
            } else {
                if (iNewStatus == 2) {
                    $("#spanStatusText").text(jsLabels["Messages#stateDismissed"]);
                    DevExpress.ui.notify(viewUtilsManager.DXTranslate('roChannelMessageSetAsDismissed'), "success", 2000);
                }
                else if (iNewStatus == 3) {
                    $("#spanStatusText").text(jsLabels["Messages#stateClosed"]);
                    DevExpress.ui.notify(viewUtilsManager.DXTranslate('roChannelMessageSetAsClosed'), "success", 2000);
                }

                refreshCardTree(idConversation);
            }
        },
        error: function (e) {
            DevExpress.ui.notify(viewUtilsManager.DXTranslate('roChannelUnkown'), "error", 5000);
        }
    });
}

function createNewMessage() {
    var sMessage = $("#newMsgText").dxTextArea("instance").option("value");
    var iConversation = viewUtilsManager.getSelectedCardId();

    if (sMessage.length < 6000) {
        if (!bSendingMessage) {
            if (sMessage != null && sMessage.trim().length > 0) {
                bSendingMessage = true;
                $.ajax({
                    type: "POST",
                    url: BASE_URL + 'Channels/AddMessage',
                    dataType: "json",
                    data: { IdConversation: iConversation, Message: sMessage },
                    success: function (e) {
                        bSendingMessage = false;
                        if (typeof e == 'string') {
                            DevExpress.ui.notify(e, "error", 5000);
                        } else {
                            refreshCardTree(iConversation);
                            DevExpress.ui.notify(viewUtilsManager.DXTranslate('roChannelMessageSent'), "success", 2000);
                        }
                    },
                    error: function (e) {
                        bSendingMessage = false;
                        DevExpress.ui.notify(viewUtilsManager.DXTranslate('roChannelUnkown'), "error", 5000);
                    }
                });
            }
            else {
                DevExpress.ui.notify(viewUtilsManager.DXTranslate('roChannelEmptyMessage'), "error", 3000);
            }
        }
    }
    else {
        DevExpress.ui.notify(viewUtilsManager.DXTranslate('roMessageTooLarge'), "error", 5000);
    }
}

function changeConversationComplexity(idConversation, iNewComplexity) {
    $.ajax({
        type: "POST",
        url: BASE_URL + 'Channels/SetConversationComplexity',
        dataType: "json",
        data: { IdConversation: idConversation, Complexity: iNewComplexity },
        success: function (e) {
            if (e == true) {
                DevExpress.ui.notify(viewUtilsManager.DXTranslate('roChannelComplexityUpdated'), "success", 2000);
            } else DevExpress.ui.notify(viewUtilsManager.DXTranslate('roChannelComplexityUpdatedError'), "error", 2000);
        },
        error: function (e) { DevExpress.ui.notify(viewUtilsManager.DXTranslate('roChannelUnkown'), "error", 2000); }
    });
}

function printConversation() {
    var $print = $("#divMessagesList")
        .clone()
        .addClass('print')
        .prependTo('body');
    window.print();
    $print.remove();
}