//*******************************************************************************************
// ** Author: A. Plaza
// ** Date: 14/12/2020
// ** Description: Javascript Common functionality for VTNext
//*******************************************************************************************

const handleShrinkExpandCommTree = () => {
    const divTree =$("#divTree");
    if (divTree.css('display') == 'none') {
        divTree.show();
        $(".btnMinimize2").removeClass('btnMinimize2').addClass('btnMaximize2');
    } else {
        divTree.hide();
        $(".btnMaximize2").removeClass('btnMaximize2').addClass('btnMinimize2');
    }
};

const toggleHasChanges = (bHasChanges) => {
    const divTree = $("#saveBar");
    if (bHasChanges) {
        divTree.show();
    } else {
        divTree.hide();
    }
};
window.MaxMinimize = handleShrinkExpandCommTree;
window.hasChanges = toggleHasChanges;



var viewUtilsManager = (function () {
    const defaultTextNotFound = "TEXT NOT FOUND";
    const defaultGridNoDataText = "NO DATA";
    const defaultInputTextWidth = 400;
    const defaultSelectOptWidth = 180;
    const defaultTextAreaWidth = 400;
    const defaultColorBoxWith = 180;
    const defaultButtonPrefix = "bt";
    const defaultInputTextPrefix = "tx";
    const defaultColorBoxPrefix = "cl";
    const defaultRadioButtonPrefix = "rg";
    const defaultCheckBoxPrefix = "ck";
    const defaultTextAreaPrefix = "ta";
    const defaultSelectOptionPrefix = "cb";
    const defaultDataGridPrefix = "dg";
    const defaultTagBoxPrefix = "tb";
    const defaultDateBoxPrefix = "dt";
    const defaultFileManagerPrefix = "fm";
    const defaultDivTabContainerSelector = "#divScreenTabsGeneric .rigthTabButtons";

    let searchCategory = null;
    let isMenuCategoriesDisplayed = false;
    let categoriesLoaded = false;
    let filterList = null;
    let viewTypeSetup = null;
    let currentControls = [];
    let originViewModel = {};
    let onChangeControlsDefaultFunc = null;
    let viewModalInstance = false;
    let oldTab = NaN;
    let onChangeTabFunc = null;
 
    let makeServiceCallBase = async function (controller,
        action,
        type,
        data,
        beforeCallback,
        successCallback,
        errorCallback,
        finallyCallback,
        dataType) {

        let response = null;
        let errorResponse = null;
        try {

            if (beforeCallback !== null)
                beforeCallback();

            let ajaxOptions = {
                url: `${BASE_URL}` + controller + "/" + action,
                data: data,
                type: type,
                success: function (data) {
                    response = data;

                    if (successCallback !== null)
                        successCallback(response);
                },
                error: function (error) {
                    errorResponse = error;

                    if (errorCallback !== null)
                        errorCallback(errorResponse)
                },
            };

            if (dataType != 'text') {
                ajaxOptions["dataType"] = dataType;
            }


            await $.ajax(ajaxOptions);
        } catch (error) {
            if (errorCallback !== null)
                errorCallback(error)

        } finally {
            if (finallyCallback !== null)
                finallyCallback();
        }
        return response;
    };

    let makeServiceCall = async function (controller,
        action,
        type,
        data,
        beforeCallback,
        successCallback,
        errorCallback,
        finallyCallback) {

        return makeServiceCallBase(controller, action, type, data, beforeCallback, successCallback, errorCallback, finallyCallback, 'json');
    };

    let makeServiceCallText = async function (controller,
        action,
        type,
        data,
        beforeCallback,
        successCallback,
        errorCallback,
        finallyCallback) {

        return makeServiceCallBase(controller, action, type, data, beforeCallback, successCallback, errorCallback, finallyCallback, 'script');
    };

    let getSelectedCardId = function () {
        let selectedCard = document.querySelector(".dxcvCSD > #CardView_DXMainTable > tbody > tr > td.CardsTree-cardClicked .cardsTree-CardInfo");
        return selectedCard ? parseInt(selectedCard.getAttribute("data-card-id"),10) : 0;
    };

    let getSelectedCardIsSystem = function () {
        let selectedCard = document.querySelector(".dxcvCSD > #CardView_DXMainTable > tbody > tr > td.CardsTree-cardClicked .cardsTree-CardInfo");
        return selectedCard ? parseInt(selectedCard.getAttribute("data-card-issystem")) : 0;
    };

    let setSelectedCardId = function (id) {

        let querySelector = ".dxcvCSD > #CardView_DXMainTable > tbody > tr";
        let itemCard = null; 

        if (id != undefined && id != null) {
            querySelector += " [data-card-id = '" + id + "']";
        }
        itemCard = $(querySelector).first();
        
        if (itemCard != null)
            $(itemCard).click();
    };

    let initAccordions = (selector) => {
        let s = (selector && selector != '' ? selector : '.accordion');
        $(s).on('click', (e) => {
            e.target.classList.toggle("active");
            let panel = $(e.target.nextElementSibling);
            panel.slideToggle();
            e.preventDefault();
        });
    };

    let getControlPrefix = function (type, uniqueKey) {

        let prefix = null;
        switch (type) {

            case "Button":
                prefix = defaultButtonPrefix + uniqueKey;
                break;

            case "InputNumber":
                prefix = defaultInputTextPrefix + uniqueKey;
                break;

            case "InputText":
                prefix = defaultInputTextPrefix + uniqueKey;
                break;

            case "SelectOption":
                prefix = defaultSelectOptionPrefix + uniqueKey;
                break;

            case "TextArea":
                prefix = defaultTextAreaPrefix + uniqueKey;
                break;

            case "CheckBox":
                prefix = defaultCheckBoxPrefix + uniqueKey;
                break;

            case "RadioButton":
                prefix = defaultRadioButtonPrefix + uniqueKey;
                break;

            case "ColorBox":
                prefix = defaultColorBoxPrefix + uniqueKey;
                break;

            case "DataGrid":
                prefix = defaultDataGridPrefix + uniqueKey;
                break;
        }

        return prefix;
    };

    let buildDataGrid = function (
        id,
        datasource,
        columnsConfig,
        cellPreparedCallback,
        rowInsertingCallback,
        rowRemovingCallback,
        rowPreparedCallback,
        rowUpdatedCallback,
        editorPreparingCallback,
        editingConfig
    ) {

        if (!editingConfig) { // Default value for editingConfig
            editingConfig = {
                allowDeleting: true,
                allowAdding: true,
                confirmDelete: true,
                texts: { confirmDeleteMessage: "" } // no display msg delete confirm
            }
        }

        let data = {
            dataSource: { store: datasource },
            showColumnLines: false,
            showRowLines: true,
            height: '300px',
            showBorders: true,
            dateSerializationFormat: 'yyyy-MM-ddTHH:mm:ss',
            rowAlternationEnabled: true,
            noDataText: defaultGridNoDataText,
            editing: editingConfig,
            onCellPrepared: function (e) {
                if (cellPreparedCallback != null)
                    cellPreparedCallback(e);
            },
            onRowInserting: async function (e) {
                if (rowInsertingCallback != null)
                    rowInsertingCallback(e);
            },
            onRowRemoving: async function (e) {
                if (rowRemovingCallback != null)
                    rowRemovingCallback(e);
            },
            onRowPrepared: function (e) {
                if (rowPreparedCallback != null)
                    rowPreparedCallback(e);
            },
            onRowUpdated: async function (e) {
                if (rowUpdatedCallback != null)
                    rowUpdatedCallback(e);
            },
            onEditorPreparing: function (e) {
                if (editorPreparingCallback != null)
                    editorPreparingCallback(e);
            }
        };

        if (columnsConfig) {
            data.columns = columnsConfig;
        }

        $("#" + id).dxDataGrid(data);
    };

    let getItemControl = function (id) {
        let arr = null;

        arr = filterInArray(currentControls, function (e) {
            return e.Name == id;
        });
        if (arr != null && arr.length > 0) {
            return arr[0];
        }
        return undefined;
    }

    let getHtmlControl = function (id) {

        let item = getItemControl(id);
        let ctrl = null;

        if (item && item != null) {

            switch (item.ControlType) {

                case "Button":
                    ctrl = $(item.HtmlSelector).dxButton("instance");
                    break;

                case "InputNumber":
                    ctrl = $(item.HtmlSelector).dxNumberBox("instance");
                    break;

                case "InputText":
                case "InputHoursAndMinutes":
                    ctrl = $(item.HtmlSelector).dxTextBox("instance");
                    break;

                case "SelectOption":
                    ctrl = $(item.HtmlSelector).dxSelectBox("instance");
                    break;

                case "TextArea":
                    ctrl = $(item.HtmlSelector).dxTextArea("instance");
                    break;

                case "CheckBox":
                    ctrl = $(item.HtmlSelector).dxCheckBox("instance");
                    break;

                case "RadioButton":
                    ctrl = $(item.HtmlSelector).dxRadioGroup("instance");
                    break;

                case "ColorBox":
                    ctrl = $(item.HtmlSelector).dxColorBox("instance");
                    break;

                case "TagBox":
                    ctrl = $(item.HtmlSelector).dxTagBox("instance");
                    break;

                case "DateBox":
                    ctrl = $(item.HtmlSelector).dxDateBox("instance");
                    break;

                case "DataGrid":
                    ctrl = $(item.HtmlSelector).dxDataGrid("instance");
                    break;

                case "FileManager":
                    ctrl = $(item.HtmlSelector).dxFileManager("instance");
                    break;
            }
        }
        return ctrl;
    }

    let viewBinding = function (model) {

        let modelProperties = Object.keys(model);
        modelProperties.forEach(function (prop) {

            //Select the element with the attribute
            let element = $("[data-binding=" + prop + "]");

            if (element != null) {
                let control = getHtmlControl(element.attr("id"));
                if (control != null) {


                    originViewModel[prop] = model[prop];
                    let value = model[prop];

                    switch (getHtmlControlType(control)) {
                        case "CheckBox":
                        case "DateBox":
                        case "TagBox":
                            value = value != null ? value : null;
                            break;
                        default:
                            value = value != null ? value.toString() : null;
                    }

                    if (originViewModel[prop] != null)
                        originViewModel[prop] = originViewModel[prop].toString();

                    applyFormatCalc(value, control, element.attr("elementType"));

                }
            }

        });
    };

    function getHtmlControlType(control) {
        let result = control.NAME;
        if (result.length > 2) {
            return result.substring(2);
        }
        return result;
    }

    function applyFormatCalc(value, control, elementType) {
        switch (elementType) {
            case "DateBoxTime":

                switch (control.option("type")) {
                    case "datetime":
                    case "date":
                        control.option("value", value);
                        break;
                    case "time":
                        control.option("value", GetTimeValueFromDateTime(value.Hours, value.Minutes, false));
                        break;
                    default:
                        control.option("value", GetTimeValueFromDateTime(value.Hours, value.Minutes, false));
                        break;
                }

                break;
            case "FromMinutesToHours":
                control.option("value", GetTimeValueFromMinutes(value, false));
                break;
            case "FromMinutesToHoursMask":
                control.option("value", GetTimeValueFromMinutes(value, true));
                break;
            default:
                control.option("value", value);
        }
    }

    let loadViewOptions = function (type, state, callback, onChangeFunc, langFile) { 

        onChangeControlsDefaultFunc = onChangeFunc;

        if (typeof langFile == 'undefined') langFile = 'LivePortal';

        makeServiceCall(
            "Base",
            "LoadViewOptions",
            "POST",
            { viewType: type, viewState: state, languageFile: langFile },
            null,
            function (controls) {

                controls.forEach(function (item) {

                    let controlIdentifier = getControlPrefix(item.ControlType, item.UniqueKey);

                    buildHtmlControl(item.ControlType, item.UniqueKey, item.LabelText, item.Caption, item.Values, null, item.ElementAttr, item.DataGridColumnConfig, onChangeFunc);
                    item.viewBindingAttr = $("#" + controlIdentifier).attr("data-binding");
                    currentControls.push(item);

                });
                if (callback != null) {
                    callback();
                }
            },
            null,
            null);
    };

    let onValueChangedEvent = function (id, onChangeFunc) {

        let control = getHtmlControl(id);
        if (control != null) {
            control.off("valueChanged");
            control.on("valueChanged", function (data) {
                if (onChangeFunc != null) onChangeFunc(data);
                else if(onChangeControlsDefaultFunc != null) onChangeControlsDefaultFunc(data);
            });
        }
    };

    let buildHtmlControl = function (
        type,
        uniqueKey,
        label,
        defaultValue,
        values,
        selectedValue,
        elementAttr,
        columnConfig, onChangeFunc) {

        let controlIdentifier = getControlPrefix(type, uniqueKey);

        //Build a specific UI Control.
        switch (type) {

            case "Button":

                $("#" + controlIdentifier).dxButton({

                    elementAttr: {
                        id: elementAttr != null ? elementAttr.Id : '',
                        class: elementAttr != null ? elementAttr.ClassName : '',
                        name: uniqueKey
                    },
                    focusStateEnabled: true,
                    hoverStateEnabled: true,
                    icon: elementAttr != null ? elementAttr.Icon : '',
                    template: "content",
                    text: defaultValue,
                    type: elementAttr != null ? elementAttr.Type : 'default',
                    useSubmitBehavior: false,
                    onValueChanged: function (data) {
                        if (onChangeFunc != null)
                            onChangeFunc(data);
                    }
                });
                break;


            case "InputNumber":

                $("#" + controlIdentifier).dxNumberBox({
                    text: defaultValue,
                    readOnly: false,
                    width: defaultInputTextWidth + "px",
                    placeholder: "",
                    value: "",
                    onValueChanged: function (data) {
                        if (onChangeFunc != null)
                            onChangeFunc(data);
                    }
                });
                break;

            case "InputText":

                $("#" + controlIdentifier).dxTextBox({
                    text: defaultValue,
                    readOnly: false,
                    width: defaultInputTextWidth + "px",
                    placeholder: "",
                    value: "",
                    onValueChanged: function (data) {
                        if (onChangeFunc != null)
                            onChangeFunc(data);
                    }
                });
                break;

            case "SelectOption":

                $("#" + controlIdentifier).dxSelectBox({
                    items: values,
                    value: selectedValue != null ? selectedValue : (values != null && values.length > 0) ? values[0].Id : null,
                    valueExpr: "Value",
                    displayExpr: "Text",
                    width: defaultSelectOptWidth + "px",
                    onValueChanged: function (data) {
                        if (onChangeFunc != null)
                            onChangeFunc(data);
                    }
                });
                break;

            case "TextArea":

                $("#" + controlIdentifier).dxTextArea({
                    value: "",
                    width: defaultTextAreaWidth + "px",
                    height: 90,
                    onValueChanged: function (data) {
                        if (onChangeFunc != null)
                            onChangeFunc(data);
                    }
                });
                break;

            case "CheckBox":

                $("#" + controlIdentifier).dxCheckBox({
                    text: defaultValue,
                    hint: " ",
                    valueExpr: "Value",
                    height: 25,
                    onContentReady: function (data) { },
                    onValueChanged: function (data) {
                        if (onChangeFunc != null)
                            onChangeFunc(data);
                    }
                });
                break;

            case "RadioButton":

                $("#" + controlIdentifier).dxRadioGroup({
                    items: values,
                    value: selectedValue != null ? selectedValue : (values != null && values.length > 0) ? values[0].Text : null,
                    displayExpr: "Text",
                    valueExpr: "Value",
                    layout: "vertical",
                    onValueChanged: function (data) {
                        if (onChangeFunc != null)
                            onChangeFunc(data);
                    }
                });
                break;

            case "ColorBox":
                $("#" + controlIdentifier).dxColorBox({
                    value: defaultValue,
                    width: defaultColorBoxWith + "px",
                    height: 30,
                    onValueChanged: function (data) {
                        if (onChangeFunc != null)
                            onChangeFunc(data);
                    }
                })
                break;

            case "DataGrid":
                if (columnConfig) {
                    columnConfig.forEach((i) => i.visible = (i.visible == null) ? true : i.visible);
                    $("#" + controlIdentifier).dxDataGrid({
                        filterRow: { visible: true },
                        columns: columnConfig
                    });
                }
                break;

            case "TagBox":
                $("#" + defaultTagBoxPrefix + uniqueKey).dxTagBox({
                    items: values,
                    searchEnabled: true,
                    displayExpr: "Text",
                    valueExpr: "Value"
                })
                break;

            case "DateBox":
                let dateType = $("#" + defaultDateBoxPrefix + uniqueKey).attr("date-type");
                let dateFormat = $("#" + defaultDateBoxPrefix + uniqueKey).attr("date-format");
                let displayFormat = $("#" + defaultDateBoxPrefix + uniqueKey).attr("date-displayformat");
                let pickerType = $("#" + defaultDateBoxPrefix + uniqueKey).attr("date-pickerType");

                if (typeof dateType === 'undefined') dateType = 'time';
                if (typeof dateFormat === 'undefined') dateFormat = 'yyyy-MM-ddTHH:mm:ss';
                if (typeof displayFormat === 'undefined') displayFormat = dateFormat;
                if (typeof pickerType === 'undefined') pickerType = 'rollers';


                $("#" + defaultDateBoxPrefix + uniqueKey).dxDateBox({
                    dateSerializationFormat: dateFormat, //'yyyy-MM-ddTHH:mm:ss',
                    displayFormat: displayFormat,
                    type: dateType, //"time",
                    pickerType: pickerType ,//"rollers",
                    defaultValue: defaultValue,
                    elementAttr: {
                        elementType: elementAttr != null ? elementAttr.Type : ''
                    }
                })
                break;

            case "InputHoursAndMinutes":
                $("#" + defaultInputTextPrefix + uniqueKey).dxTextBox({
                    text: defaultValue,
                    readOnly: false,
                    width: defaultInputTextWidth + "px",
                    placeholder: "",
                    value: "",
                    mask: "00:00",
                    maskChar: "_",
                    useMaskedValue: true,
                    elementAttr: {
                        elementType: elementAttr != null ? elementAttr.Type : ''
                    }
                });
                break;

            case "FileManager":
                $("#" + defaultFileManagerPrefix + uniqueKey).dxFileManager({
                    name: "fileManager",
                    width: "100%",
                    fileSystemProvider: [],
                    permissions: {
                        create: false,
                        delete: true,
                        rename: false,
                        download: true,
                        upload: true,
                        move: false,
                        copy: false
                    },
                    upload: {
                        chunkSize: 10000000,
                        maxFileSize: 10000000
                    },
                    itemView: {
                        mode: "thumbnails",
                        details: {
                            columns: [
                                {
                                    dataField: "thumbnail",
                                    caption: "Miniatura"
                                },
                                {
                                    dataField: "name",
                                    caption: "Nombre"
                                },
                                {
                                    dataField: "dateModified",
                                    caption: "Fecha",
                                    format: "shortDate"
                                },
                                {
                                    dataField: "size",
                                    caption: "Tamaño"
                                }
                                
                            ]
                        },
                        showParentFolder: false
                    }
                });
                break;

            case "None":
                break;
        }
        //Set the label text.
        $("#lb" + uniqueKey).text(label);

    };

    let getCurrentModelData = function () {

        let model = {};

        currentControls.forEach(function (item) {
            if (item.viewBindingAttr != undefined) {
                let htmlControl = getHtmlControl(item.Name);

                if (htmlControl != null)
                    model[item.viewBindingAttr] = htmlControl.option('value');

                if (model[item.viewBindingAttr] != null)
                    model[item.viewBindingAttr] = model[item.viewBindingAttr].toString();
            }
        });

        return model;
    };

    let formHasChanged = function () {

        const currentObj = Object.keys(getCurrentModelData()).sort().reduce(
            (obj, key) => {
                obj[key] = getCurrentModelData()[key];
                return obj;
            },
            {}
        );

        const originObj = Object.keys(originViewModel).sort().reduce(
            (obj, key) => {
                obj[key] = originViewModel[key];
                return obj;
            },
            {}
        );

        return (JSON.stringify(originObj) != JSON.stringify(currentObj));
    };

    let validateForm = function () {
        let result = true;
        currentControls.forEach(function (item) {
            if (!validateItem(item)) {
                result = false;
            }
        });
        return result;
    };

    let clearValidationForm = function () {
        currentControls.forEach(function (item) {
            hideValidationMessage(item);
            let htmlControl = getHtmlControl(item.Name);
            if (htmlControl != null) {
                htmlControl.option("isValid", true);
            }
        });
    }

    let validateItem = function (item) {
        let result = true;
        if (item.HtmlSelector != undefined && item.HtmlSelector != "") {
            let validation = $(item.HtmlSelector).data("validation");
            if (validation) {
                let htmlControl = getHtmlControl(item.Name);
                if (htmlControl != null && validation === "required") {
                    if (htmlControl.option("value") == "" || htmlControl.option("value") == null) {
                        result = false;
                        htmlControl.option("isValid", false);
                        showValidationMessage(item);
                    } else {
                        htmlControl.option("isValid", true);
                        hideValidationMessage(item);
                    }
                }
            }
        }
        return result;
    }

    let hideValidationMessage = function (item) {
        var element = $(`[data-validation-binding="${item.HtmlSelector}"]`);
        if (element != undefined) {
            element.addClass('d-none');
            element.removeClass('d-block');
        }
    };

    let showValidationMessage = function (item) {
        var element = $(`[data-validation-binding="${item.HtmlSelector}"]`);
        if (element.length > 0) {
            var validationMessageKey = element.data("validation-message");
            if (validationMessageKey != undefined) {
                element.html(translateValidationMessage(validationMessageKey));
                element.removeClass('d-none');
                element.addClass('d-block');
            }
        }

        var htmlControl = getHtmlControl(item.Name);
        if (htmlControl != null) {
            var validationMessage = translateValidationMessage($(item.HtmlSelector).data("validation-message"));
            htmlControl.option("validationError", { message: validationMessage });
        }
    };

    let translateValidationMessage = function (key) {
        var validationMessage = "";
        if (key != undefined) {
            try {
                validationMessage = DXTranslate(key);
            } catch (error) {

            }
            return validationMessage;
        }
        return "";
    }

    let setupCardListFilterButton = function (viewType) {
        viewTypeSetup = viewType;
        searchCategory = document.querySelector("#filterListBtn > nobr > span");

        $("#filterListBtn").off("click");
        $("#filterListBtn").on("click", handleCardListFilterButton);
        
    };

    let setBarButtonCallback = function (callback) {
        onChangeTabFunc = callback;
    };

    let handleCardListFilterButton = async function () {
        if (!isMenuCategoriesDisplayed) {
            if (!categoriesLoaded) {
                await makeServiceCall(
                    "Base",
                    "GetCardViewSearchFilter",
                    "POST",
                    { viewType: viewTypeSetup },
                    null,
                    function (filterListResult) {
                        categoriesLoaded = true;
                        filterList = filterListResult;

                    },
                    null,
                    null);
            }

            if (filterList != null) {
                $("#cardTreeSearchFilter").dxList({
                    dataSource: filterList,
                    height: function () { return $(window).height() * 0.6 },
                    itemTemplate: function (data, index) {
                        let result = $("<div>").html(data.Description);
                        if (data?.Parent != "1") $(result).css('padding-left', '2rem');
                        return result;
                    },
                    onItemClick: function (e) {
                        handleFilterMenuClick(e.itemData);
                        handleCardListFilterButtonStyles(true);
                    }
                });

                $("#cardTreeSearchFilter").css("max-height", (filterList.length * 40) + "px")
            }
            $('#searchButtonDirection').addClass('fa-caret-down').removeClass('fa-caret-right');
            $('#cardTreeSearchFilter').show();
            isMenuCategoriesDisplayed = true;
        } else {
            $('#searchButtonDirection').addClass('fa-caret-right').removeClass('fa-caret-down');
            $('#cardTreeSearchFilter').hide();
            isMenuCategoriesDisplayed = false;
        }
        handleCardListFilterButtonStyles();   //styles of Btn;
    }

    let handleFilterMenuClick = function (item) {

        $('#cardTreeSearchFilter').hide();
        $('#searchButtonDirection').addClass('fa-caret-right').removeClass('fa-caret-down');
        isMenuCategoriesDisplayed = false;

        searchCategory.innerText = item.Description;
        
        handleFilterSearch(item);
    }

    let handleFilterSearch = function (item) {
        //let searchValue = ((item.Id =="") ? "" : `Filterfield:${item.ID}`);
        const filterType = item.Filter || "Filterfield";
        var categoryFilterSearch = ((item.ID == "") ? "" : filterType + ':' + item.ID);
        window.handleCardTreeCustomSearch("", categoryFilterSearch);
    }

    let handleCardListFilterButtonStyles = function (catClick = false) {

        const $reportCategoriesBtn = $("#filterListBtn");
        const $reportCategoriesInnerScope = $("#filterInnerScope");

   
        if (!isMenuCategoriesDisplayed) {

            $reportCategoriesBtn.addClass("reportTagsBtnActive");
            $reportCategoriesInnerScope.addClass("active");
            $reportCategoriesBtn
                .find(".fa.fa-caret-down")
                .addClass("fa-caret-downDroped");
        } else {
            $reportCategoriesBtn.removeClass("reportTagsBtnActive");
            $reportCategoriesInnerScope.removeClass("active");
            $reportCategoriesBtn
                .find(".fa.fa-caret-down.fa-caret-downDroped")
                .removeClass("fa-caret-downDroped");
        }
    };

    let filterInArray = function (arr, conditionFunc) {
        return $.grep(arr, conditionFunc);
    };

    let getFormObject = function (formId) {

        let formDataAsArray = $("#" + formId).serializeArray();
        let res = {};
        $(formDataAsArray).each(function (index, obj) {
            res[obj.name] = obj.value;
        });
        return res;
    }

    let setToolbarButtonsState = function (state) {

        let hasChanged = formHasChanged();
        let buttonsActiveState = false;

        switch (state) {
            case "read":
            case "update":
            case "create":
                buttonsActiveState = hasChanged;
                break;
            case "delete":
                break;
        }

        if (buttonsActiveState === true) $(defaultDivTabContainerSelector).addClass("active");
        else $(defaultDivTabContainerSelector).removeClass("active");

    };

    let resetHtmlControls = function () {

        currentControls.forEach(function (item) {
            let htmlControl = getHtmlControl(item.Name);
            if (htmlControl != null) {
                try {
                    htmlControl.reset();
                }
                catch {
                    htmlControl.option('value', null);
                }
            }
        });
    };

    let buildButtonDialog = function (captionKey, descriptionKey, callbackFunc) {

        return { captionKey: captionKey, descriptionKey: descriptionKey, callbackFunc: callbackFunc };
    }

    let showModalDialog = function (typeIcon, errorCode, title, description, buttons) {

        let dialogObj = {
            typeIcon: typeIcon,
            titleKey: title,
            descriptionKey: description,
            returnCode: errorCode,
            buttons: buttons
        };

        if (!viewModalInstance) {
            viewModalInstance = true;
            buildModalDialog(dialogObj, '');
        }
    };

    let buildModalDialog = function (dialogObj, errorMsg, stateObj) {

        let mainWindow = window;

        let callbackFunc = function () {

            var errorText = "";
            var computedButtons = [];
            var title = "";

            if (dialogObj.descriptionKey != "") {
                if (dialogObj.returnCode != "") errorText = dialogObj.returnCode + ": " + (dialogObj.descriptionKey != '' ? _self.Globalize.formatMessage(dialogObj.descriptionKey) : defaultTextNotFound);
                else errorText = (dialogObj.descriptionKey != '' ? _self.Globalize.formatMessage(dialogObj.descriptionKey) : defaultTextNotFound)
            } else {
                if (errorMsg == "" && typeof stateObj != 'undefined') {
                    let oState = JSON.parse(stateObj);

                    errorText = (typeof oState.errorTextField == 'undefined' ? oState.ErrorText : (oState.errorTextField == null ? '' : oState.errorTextField));
                    if (oState.ReturnCode != "") errorText = (typeof oState.returnCodeField == 'undefined' ? oState.ReturnCode : (oState.returnCodeField == null ? '' : oState.returnCodeField)) + ": " + errorText;
                } else {
                    errorText = errorMsg;
                }
            }

            if (dialogObj.titleKey != "") title = _self.Globalize.formatMessage(dialogObj.titleKey);
            else title = defaultTextNotFound;


            let customMessageDiv = $('<div>').attr('class', 'roPopupMsg');
            customMessageDiv.append($('<div>').attr('class', 'roPopupIcon ro-icon-errorType-' + dialogObj.iconType));
            customMessageDiv.append($('<div>').attr('class', 'roPopupText').html(errorText));

            let heightDiv = $('<div>').attr('style', 'clear:both');

            $.each(dialogObj.buttons, function (i, e) {

                if (e.captionKey != "") {

                    computedButtons.push(
                        {

                            text: (e.captionKey != "" ? _self.Globalize.formatMessage(e.captionKey) : ''),
                            hint: (e.descriptionKey != "" ? _self.Globalize.formatMessage(e.descriptionKey) : ''),
                            onClick: function () { return e.captionKey; }
                        });
                }
            });


            let customDialog = DevExpress.ui.dialog.custom({
                title: title,
                messageHtml: $('<div>').append(customMessageDiv, heightDiv).html(),
                css: 'roCustomDialog',
                buttons: computedButtons
            });


            customDialog.show().done(function (dialogResult) {


                const buttonClicked = filterInArray(dialogObj.buttons, function (e) {

                    return e.captionKey == dialogResult;
                });


                if ((buttonClicked.length > 0) &&
                    (buttonClicked[0].callbackFunc != undefined && buttonClicked[0].callbackFunc != null)) {

                    buttonClicked[0].callbackFunc();
                }

                viewModalInstance = false;
                customDialog.hide();
            });
        }

        // Make sure loadJSErrorLanguages exists
        if (typeof mainWindow.loadJSErrorLanguages === 'function') {
            mainWindow.loadJSErrorLanguages(callbackFunc);
        }
    }

    async function ChangeTab(indexTab, callbackFunc) {

        if (isNaN(oldTab) || (!isNaN(oldTab) && oldTab != indexTab)) {

            if (indexTab === undefined)
                indexTab = 0;

            let tabButtons = $("#divScreenTabsGeneric .switchMainViewTabsGeneric");
            let tabSection = $("#divContenido .ro-tab-section");

            $(tabButtons).children(".mainActionBtnGeneric").removeClass("activeTab");
            $(tabButtons).children(".mainActionBtnGeneric[data-tabIntex='" + indexTab + "']").addClass("activeTab");

            tabSection.children(".ro-tab-pane").removeClass("active");
            tabSection.children(".ro-tab-pane[data-tabIndex='" + indexTab + "']").addClass("active");

            if (typeof callbackFunc === 'function') onChangeTabFunc = callbackFunc;

            if (onChangeTabFunc != null) await onChangeTabFunc(indexTab);
            oldTab = indexTab;
        }
    }

    let dispatchEvent = function (state, event) {

        document.dispatchEvent(new CustomEvent("startStateEvent", {
            "detail": {
                "currentState": state, "event": event
            }
        }));
    };

    let viewStateHandler = function (handlerDefinition, callback) {

        const handler = {
            value: handlerDefinition.initialState,
            transition(currentState, event) {
                const currentStateDefinition = handlerDefinition[currentState];
                const destinationTransition = currentStateDefinition.transitions[event];
                if (!destinationTransition) {
                    return;
                }
                const destinationState = destinationTransition.target;
                const destinationStateDefinition = handlerDefinition[destinationState];
                destinationTransition.action();
                currentStateDefinition.actions.onExit();
                destinationStateDefinition.actions.onEnter();
                handler.value = destinationState;
                return handler.value;
            },
        }

        if (callback != null)
            callback();

        return handler;
    };

    let createViewStateHandler = function () {
        return viewStateHandler({
            initialState: 'read',
            read: {
                actions: {
                    onEnter() {
                        dispatchEvent("read", "enter");
                    },
                    onExit() {
                        dispatchEvent("read", "exit");
                    }
                },
                transitions: {
                    create: {
                        target: 'create',
                        action() {
                        },
                    },
                    update: {
                        target: 'update',
                        action() {
                        }
                    },
                    delete: {
                        target: 'delete',
                        action() {
                        }
                    }
                },
            },
            create: {
                actions: {
                    onEnter() {
                        dispatchEvent("create", "enter");
                    },
                    onExit() {
                        dispatchEvent("create", "exit");
                    }
                },
                transitions: {
                    read: {
                        target: 'read',
                        action() {
                        },
                    },
                },
            },
            update: {
                actions: {
                    onEnter() {

                        dispatchEvent("update", "enter");
                    },
                    onExit() {
                        dispatchEvent("update", "exit");
                    }
                },
                transitions: {
                    read: {
                        target: 'read',
                        action() {
                        },
                    },
                },
            },
            delete: {
                actions: {
                    onEnter() {
                        dispatchEvent("delete", "enter");
                    },
                    onExit() {
                        dispatchEvent("delete", "exit");
                    },
                },
                transitions: {
                    read: {
                        target: 'read',
                        action() {
                        },
                    },
                },
            }
        });
    };

    let DXTranslate = function (key) {
        try {
            return parent.Globalize.formatMessage(key);
        } catch {
            return '';
        }
    };

    function GetTimeValueFromDateTime(hours, minutes, isMask) {

        if (!hours) {
            hours = "";
        }
        if (!minutes) {
            minutes = "";
        }

        hours = (hours + "").padStart(2, "0");
        minutes = (minutes + "").padStart(2, "0");
        if (isMask) {
            return `${hours}:${minutes}:00`;
        }
        return `01/01/1990 ${hours}:${minutes}:00`;
    }

    function GetTimeValueFromMinutes(minutes, isMask) {
        return GetTimeValueFromDateTime(Math.round(minutes / 60), (minutes % 60), isMask);
    }

    return {
        setupCardListFilterButton: setupCardListFilterButton,
        oldTab: function () { return oldTab; },
        changeTab: ChangeTab,
        getCurrentModelData: getCurrentModelData,
        filterInArray: filterInArray,
        formHasChanged: formHasChanged,
        onValueChangedEvent: onValueChangedEvent,
        getFormObject: getFormObject,
        setToolbarButtonsState: setToolbarButtonsState,
        makeServiceCall: makeServiceCall,
        makeServiceCallText: makeServiceCallText,
        getHtmlControl: getHtmlControl,
        buildHtmlControl: buildHtmlControl,
        loadViewOptions: loadViewOptions,
        viewStateHandler: viewStateHandler,
        createViewStateHandler: createViewStateHandler,
        buildDataGrid: buildDataGrid,
        viewBinding: viewBinding,
        getSelectedCardId: getSelectedCardId,
        getSelectedCardIsSystem: getSelectedCardIsSystem,
        setSelectedCardId: setSelectedCardId,
        resetHtmlControls: resetHtmlControls,
        initAccordions: initAccordions,
        showModalDialog: showModalDialog,
        buildButtonDialog: buildButtonDialog,
        DXTranslate: DXTranslate,
        validateForm: validateForm,
        clearValidationForm: clearValidationForm,
        validateItem: validateItem,
        getItemControl: getItemControl,
        hideValidationMessage: hideValidationMessage,
        showValidationMessage: showValidationMessage,
        setBarButtonCallback: setBarButtonCallback,
        print: function (msg) {
        }
    };
})();

