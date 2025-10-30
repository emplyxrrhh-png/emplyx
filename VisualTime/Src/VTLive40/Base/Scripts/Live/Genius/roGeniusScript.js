//*******************************************************************************************
// ** Author: I. Santaularia.
// ** Date: 23/03/2021.
// ** Description: javascript file for Genius module.
//*******************************************************************************************

var currentGeniusView = {};
var currentGeniusEditParameters = {};
var currentExecution = null;
var existingView = false;
var updating = false;
var monthlyType = -1;
var onInitializing = false;
var userFieldsLoaded = false;
var userFieldsOpened = false;

(function () {
    //[ Datagrid  ..................

    //Properties
    var viewHandler = null;
    var flGrid = null;
    var data;
    var checkForFinishTimeout = null;
    var bLaunchOnCallback = false;
    var longFormat = 'dd/M/yyyy'; 

    $(document).ready(async function () {
        if (isGeniusAdvanced == "False")
            $('.btnShare').parent().hide();
        else
            $('.btnShare').parent().show();

        viewUtilsManager.initAccordions();
        viewUtilsManager.setupCardListFilterButton("Genius");

        viewUtilsManager.print("Genius Module loaded");
        viewHandler = viewUtilsManager.createViewStateHandler();

        // -----------------------------
        // GET DATA VIEW (ON READY) ----
        //------------------------------
        window.loadRequest = loadGeniusSelected;
        window.openReportConfigurationPopUp = openReportConfigurationPopUp;
        window.saveData = saveData;
        window.deleteCurrentGenius = validationDeleteGeniusView;
        window.deleteConfirmed = deleteGeniusView;
        window.runGeniusView = runGeniusView;
        window.saveGeniusView = saveGeniusView;
        window.createGeniusView = createGeniusView;
        window.updateGeniusView = updateGeniusView;
        window.editCurrentGenius = editCurrentGenius;
        window.switchToExecution = switchToExecution;
        window.backToLauncher = backToLauncher;
        window.saveGeniusLayout = saveGeniusLayout;
        window.loadGeniusByTask = loadGeniusByTask;
        window.waitForExecutionToFinish = waitForExecutionToFinish;
        window.shareCurrentGenius = shareCurrentGenius;
        window.convert2SystemCurrentGenius = convertCurrentGenius;
        window.addPlanificationCurrentGenius = addPlanificationCurrentGenius;
        window.onShareGeniusAccept = onShareGeniusAccept;
        window.saveCustomView = saveCustomView;
        window.cancelCostCenterInfo = cancelCostCenterInfo;
        window.saveCostCenterInfo = saveCostCenterInfo;
        window.cancelConceptsInfo = cancelConceptsInfo;
        window.saveConceptsInfo = saveConceptsInfo;
        window.saveCausesInfo = saveCausesInfo;
        window.cancelCausesInfo = cancelCausesInfo;
        window.ckIncludeAllCauses_OnChange = ckIncludeAllCauses_OnChange;
        window.cancelRequestsInfo = cancelRequestsInfo;
        window.saveRequestsInfo = saveRequestsInfo;
        window.clearNewForm = clearNewForm;
        window.onBeforeSendPlanning = onBeforeSendPlanning;
        window.onPlanningListLoaded = onPlanningListLoaded;
        window.ckIncludeCausesWithZero_Changed = ckIncludeCausesWithZero_Changed;

        document.addEventListener("startStateEvent", (data) => viewHandlerEvent(data), false);

        viewHandler.transition(viewHandler.value, "read");

        viewUtilsManager.loadViewOptions("Genius", "read", function () {
            let iTask = parseInt(SelectedTask, 10);
            if (iTask != -1) loadGeniusByTask(iTask);
        }, () => { }, 'LiveGenius');

        //Cargamos la analítica Genius si viene informada en la url
        const qParamsURL = new Proxy(new URLSearchParams(window.location.search), {
            get: (searchParams, prop) => searchParams.get(prop),
        });
        // Get the value of "loadview" in eg "https://example.com/?loadview=some_value"
        const qIdTask = qParamsURL.loadView;
        if (qIdTask) loadGeniusByTask(qIdTask);                 
        
        switch (JSON.parse(localStorage.getItem("roLanguage")).key) {
            case "es":
            case "ca":
            case "pt":
            case "gl":
            case "it":
            case "fr":
            case "eu":
                longFormat = 'DD/M/yyyy HH:mm';
                shortFormat = 'DD/M/yyyy';
                break;
            case "en":
            default:
                longFormat = 'M/DD/yyyy HH:mm';
                shortFormat = 'M/DD/yyyy';
                break;
        }

        $('#ReportUsersNew').off("click");
        $('#ReportUsersNew').on("click", function (e) {
            existingView = false;

            let currentSelectorView = { Employees: [], Groups: [], Filter: "11110", UserFields: "", ComposeMode: 'Custom', ComposeFilter: "", Advanced: false, Operation: "or" };

            if (currentGeniusView != null && currentGeniusView.Employees != '' && (existingView || ($("#ReportUsersNew").dxTextBox("instance").option("value") != null && $("#ReportUsersNew").dxTextBox("instance").option("value") != ''))) {
                let employeeGroups = currentGeniusView.Employees.split('#')[0].split(',');
                for (let i = 0; i < employeeGroups.length; i++) {
                    if (employeeGroups[i].charAt(0) == 'A')
                        currentSelectorView.ComposeFilter += currentSelectorView.ComposeFilter == '' ? `A${employeeGroups[i].substring(1)}` : `,A${employeeGroups[i].substring(1)}`;
                    else if (employeeGroups[i].charAt(0) == 'B')
                        currentSelectorView.ComposeFilter += currentSelectorView.ComposeFilter == '' ? `B${employeeGroups[i].substring(1)}` : `,B${employeeGroups[i].substring(1)}`;
                }
            }

            parent.showLoader(true);
            $("#divGeniusEmployeeSelector").load('/Employee/EmployeeSelectorPartial?feature=Employees&pageName=genius&config=100&unionType=or&advancedMode=1&advancedFilter=0&allowAll=0&allowNone=0', function () {
                loadPartialInfo();
                initUniversalSelector(currentSelectorView,false,'geniusSelector');
                parent.showLoader(false);
            });
        });

        $('#geniusGeneral').off("click");
        $('#geniusGeneral').on("click", function (e) {
            $(".generalDiv").show();
            $("#planificationDiv").hide();
            $("#geniusGeneral").addClass("bTabGeniusMenu-active")
            $("#geniusPlanification").removeClass("bTabGeniusMenu-active")
        });

        $('#geniusPlanification').off("click");
        $('#geniusPlanification').on("click", function (e) {
            $(".generalDiv").hide();
            $("#planificationDiv").show();
            $("#geniusGeneral").removeClass("bTabGeniusMenu-active");
            $("#geniusPlanification").addClass("bTabGeniusMenu-active");
        });

        $('#monthlyOption1').off("click");
        $('#monthlyOption1').on("click", function (e) {
            monthlyType = 0;
            if ($("#txtDayOfMonth").dxNumberBox("instance").option("value") == null)
                $("#txtDayOfMonth").dxNumberBox("instance").option("value", 1);
            $('#monthlyOption2').css('color', '#D3D3D3');
            $('#monthlyOption1').css('color', '#212529');
            $('#monthlyOption2 input').css('color', '#D3D3D3');
            $('#monthlyOption1 input').css('color', '#212529');
        });

        $('#monthlyOption2').off("click");
        $('#monthlyOption2').on("click", function (e) {
            monthlyType = 1;
            $('#monthlyOption1').css('color', '#D3D3D3');
            $('#monthlyOption2').css('color', '#212529');
            $('#monthlyOption1 input').css('color', '#D3D3D3');
            $('#monthlyOption2 input').css('color', '#212529');
        });

        //Click derecho sobre un estudio de genius
        $('.dxcvCard').off("contextmenu");
        $('.dxcvCard').on("contextmenu", (e) => {
            showSharedContextMenu(e);
        }
        );
    });

    function showSharedContextMenu(e) {
        e.preventDefault();
        //Peticion asincrona para obtener listado de personas a las que se ha compartido el estudio genius
        const idCard = $(e.currentTarget).find('.cardsTree-CardInfo').attr('data-card-id');
        viewUtilsManager.makeServiceCall(
            "Genius",
            "GetGeniusViewSharedList",
            "POST",
            { id: idCard },
            null,
            function (response) {
                if (response != null) {
                    const lenShared = response.length;
                    if ($('.sharedInfo[data-id="' + idCard + '"]').length == 0) {
                        const popupHtml = (lenShared > 0) ? "<h5><span class='btnShare'></span>" + viewUtilsManager.DXTranslate('sharedInfoGeniusTitle') + "</h5>" : "<p>" + viewUtilsManager.DXTranslate('sharedInfoGeniusNotYet') + "</p>";
                        const sharedUsers = response.map(u => "<p>" + u + "</p>");
                        const popup = $('<div class="sharedInfo" data-id="' + idCard + '">' + popupHtml + '</div>').css({
                            "left": e.pageX + 'px',
                            "top": e.pageY + 'px'
                        }).append($(sharedUsers.join('')))
                            .appendTo(document.body);
                        setTimeout(function () { popup.remove(); }, (2500 + lenShared * 500));
                    }
                }
                else {
                    DevExpress.ui.notify(viewUtilsManager.DXTranslate('roPermissionsError'), 'error', 2000);
                }
            },
            null,
            null
        );
    }

    function rbPeriodSelection_changed(data) {
        var dateInf = viewUtilsManager.getHtmlControl("dtDateInf");
        var dateSup = viewUtilsManager.getHtmlControl("dtDateSup");

        var dateInfVal = moment();
        var dateSupVal = moment();

        var dateInfEnabled = true;
        var dateSupEnabled = true;

        switch (parseInt(data.value, 10)) {
            case 0: //PeriodOther
                dateInfEnabled = false;
                dateSupEnabled = false;
                break;
            case 1: //PeriodTomorrow
                dateInfVal = moment().startOf('day').add(1, 'days');
                dateSupVal = dateInfVal.clone().endOf('day');
                break;
            case 2: //PeriodToday
                dateInfVal = moment().startOf('day');
                dateSupVal = dateInfVal.clone().endOf('day');
                break;
            case 3: //PeriodYesterday
                dateInfVal = moment().startOf('day').add(-1, 'days');
                dateSupVal = dateInfVal.clone().endOf('day');
                break;
            case 4: //PeriodCurrentWeek
                dateInfVal = moment().startOf('week');
                dateSupVal = dateInfVal.clone().endOf('week');
                break;
            case 5: //PeriodLastWeek
                dateInfVal = moment().startOf('week').add(-1, 'week');
                dateSupVal = dateInfVal.clone().endOf('week');
                break;
            case 6: //PeriodCurrentMonth
                dateInfVal = moment().startOf('month');
                dateSupVal = dateInfVal.clone().endOf('month');
                break;
            case 7: //PeriodLastMonth
                dateInfVal = moment().startOf('month').add(-1, 'month');
                dateSupVal = dateInfVal.clone().endOf('month');
                break;
            case 8: //PeriodCurrentYear
                dateInfVal = moment().startOf('year');
                dateSupVal = moment().endOf('day');
                break;
            case 9: //PeriodNextWeek
                dateInfVal = moment().startOf('week').add(1, 'week');
                dateSupVal = dateInfVal.clone().endOf('week');
                break;
            case 10: //PeriodNextMonth
                dateInfVal = moment().startOf('month').add(1, 'month');
                dateSupVal = dateInfVal.clone().endOf('month');
                break;
            default:
                break;
        }

        dateInf.option('disabled', dateInfEnabled);
        dateSup.option('disabled', dateSupEnabled);

        dateInf.option('value', dateInfVal.toDate());
        dateSup.option('value', dateSupVal.toDate());
    }

    function viewHandlerEvent(eventData) {
        var eventDetails = eventData.detail;

        viewUtilsManager.print(eventDetails.currentState + " state");
        switch (eventDetails.currentState) {
            case "read":
                //Código para limpiar los controles
                //console.log('READDDDDDD');
                break;

            case "create":
                //console.log('CREATEEEEEE');

                break;

            case "update":
                //console.log('UPDATEEEEEEEEEE');
                break;

            case "delete":
                //console.log('DELETEEEEEEEEEEEE');
                break;
        }
    };

    //*******************************************************************************************
    //FUNCTIONS              ********************************************************************
    //*******************************************************************************************

    const bindGeniusView = (response) => {
        currentGeniusView = response;

        $("#PlanningList").dxDataGrid("instance").refresh();
        $("#geniusPlannerScheduler").hide();
        $("#geniusConfigScheduler").hide();

        if (bLaunchOnCallback) {
            bLaunchOnCallback = false;
            runGeniusView();
        } else {
            //$('#ctl00_contentMainBody_lblHeader').text(response.Name);
            response.DateInf = null;
            response.DateSup = null;//moment(response.DateSup).toDate();
            response.CreatedOn = moment(response.CreatedOn).toDate();

            viewUtilsManager.viewBinding(response);
            viewUtilsManager.clearValidationForm();

            var navData = [];
            const loadingBIText = "Cargando datos...";

            for (var i = 0; i < response.Executions.length; i++) {
                response.Executions[i].ExecutionDate = moment(response.Executions[i].ExecutionDate).toDate();

                if (response.CustomFields.DownloadBI) {
                    if (response.Executions[i].FileLink != "Error" && response.Executions[i].Id != "")
                        navData.push({ 'Fecha': moment(response.Executions[i].ExecutionDate).format(longFormat), 'Enlace': (response.Executions[i].SASLink || loadingBIText), 'ID': response.Executions[i].Id, 'FileName': response.Executions[i].FileLink });
                } else {
                    if (response.Executions[i].FileLink != "Error")
                        if (response.Executions[i].FileLink != "")
                            navData.push({ 'text': moment(response.Executions[i].ExecutionDate).format(longFormat), 'icon': 'chart', 'data': response.Executions[i] });
                        else
                            navData.push({ 'text': moment(response.Executions[i].ExecutionDate).format(longFormat), 'icon': 'refresh', 'data': response.Executions[i] });
                    else
                        navData.push({ 'text': moment(response.Executions[i].ExecutionDate).format(longFormat), 'icon': 'clear', 'data': response.Executions[i] });
                }
            }

            if (BIIntegration == "True" && currentGeniusView.CustomFields.DownloadBI) {
                $('#geniusExecutionsBI').dxDataGrid({
                    dataSource: navData,
                    columns: [{ dataField: 'Fecha', width: 150 }, 'Enlace'],
                    showBorders: false,
                    rowAlternationEnabled: true,
                    filterRow: {
                        visible: true,
                        applyFilter: 'auto',
                    },
                    headerFilter: {
                        visible: true,
                    },
                    hoverStateEnabled: true,
                    selection: {
                        mode: 'single',
                    },
                    onRowClick(e) {
                        const data = e.data;
                        if (data && data.Enlace.length > 0 && data.Enlace != loadingBIText) {
                            navigator.clipboard.writeText(data.Enlace);
                            const toast = $('#toastBILinkCopied').dxToast({ displayTime: 1000 }).dxToast('instance');
                            toast.option({
                                type: 'custom', contentTemplate: function () {
                                    return $("<p />").text("Enlace copiado")
                                        .css("background-color", "#0046fe")
                                        .css("padding", "3px 20px")
                                        .css("border-radius", "5px");
                                }
                            });
                            toast.show();
                        }
                    },
                    editing: {
                        allowDeleting: true
                    },
                    onRowRemoving: function (e) {
                        deleteGeniusExecution(e);
                    },
                });
                $('#geniusExecutions').hide();
                $('#geniusExecutionsBI').show();
            } else {
                $('#geniusExecutions').dxTileView({
                    dataSource: navData,
                    noDataText: viewUtilsManager.DXTranslate('roNotExecuted'),
                    direction: 'vertical',
                    height: 450,
                    itemTemplate: function (itemData, itemIndex, itemElement) {
                        itemElement.parent().addClass("roGeniusTile");
                        itemElement.append(
                            "<div class='roTile dx-icon dx-icon-" + itemData.icon + "'></div>",
                            "<p class='roTileDesc'>" + itemData.text + "</p>"
                        )
                    },
                    onItemClick: function (e) {
                        getAzureFileInfo(e.itemData.data);
                    }
                });
                $('#geniusExecutions').show();
                $('#geniusExecutionsBI').hide();
            }
        }
    }

    const shareCurrentGenius = () => {
        if (currentGeniusView != null) {
            $("#sharePopup").dxPopup("instance").show();
            $("#ShareEmployeeText").dxTagBox("instance").option("value", []);
        } else {
            DevExpress.ui.notify(viewUtilsManager.DXTranslate('roNoGeniusViewSelected'), 'error', 2000);
        }
    }

    const onShareGeniusAccept = async () => {
        let model = createModel();
        let selectedUsers = $("#ShareEmployeeText").dxTagBox("instance").option("value");
        if (model != null && selectedUsers != null) {
            await $.ajax({
                url: `${BASE_URL}Genius/ShareGeniusView`,
                data: { idView: model.Id, users: selectedUsers },
                type: "POST",
                dataType: "json",
                success: (data) => {
                    if (data) {
                        $("#sharePopup").dxPopup("instance").hide();
                        DevExpress.ui.notify(viewUtilsManager.DXTranslate('roShared'), 'success', 2000);
                    } else {
                        DevExpress.ui.notify(viewUtilsManager.DXTranslate('roShareSaveError'), 'error', 2000);
                    }

                    selectedUsers = null;
                },
                error: (error) => console.error(error),
            });
        } else {
            DevExpress.ui.notify(viewUtilsManager.DXTranslate('roNoSelectedUserToShare'), 'error', 2000);
        }
    }

    const convertCurrentGenius = async () => {
        let model = createModel();
        let selectedUser = 0;
        if (model != null && selectedUser != null) {
            await $.ajax({
                url: `${BASE_URL}Genius/ConvertGeniusView`,
                data: { idView: model.Id, idPassport: selectedUser },
                type: "POST",
                dataType: "json",
                success: (data) => {
                    if (data) {
                        DevExpress.ui.notify(viewUtilsManager.DXTranslate('roConverted'), 'success', 2000);
                    } else {
                        DevExpress.ui.notify(viewUtilsManager.DXTranslate('roConvertedError'), 'error', 2000);
                    }

                    selectedUser = null;
                },
                error: (error) => console.error(error),
            });
        } else {
            DevExpress.ui.notify(viewUtilsManager.DXTranslate('roNoSelectedUserToShare'), 'error', 2000);
        }
    }

    const saveGeniusLayout = () => {
        var reportCongif = flGrid.getReport();

        var params = { ...reportCongif };

        delete params.dataSource;
        delete params.version;
        //params.slice.measures = params.slice.measures.filter(m => m.active !== false);

        currentExecution.CubeLayout = JSON.stringify(params);

        viewUtilsManager.makeServiceCall(
            "Genius",
            "updateGeniusView",
            "PUT",
            { "geniusExecution": currentExecution },
            null,
            (data) => {
                data ? DevExpress.ui.notify(viewUtilsManager.DXTranslate('roLayoutSaved'), 'success', 2000) : DevExpress.ui.notify(viewUtilsManager.DXTranslate('roLayoutSaveError'), 'error', 2000);
            },
            (error) => console.error(error),
            null
        );
    }

    const saveGeniusView = () => {
        var execModel = createModel();

        var validationInfo = isValid(execModel);
        if (validationInfo.valid) {
            viewUtilsManager.makeServiceCall(
                "Genius",
                "saveGeniusView",
                "POST",
                { "geniusView": execModel },
                null,
                (data) => { currentGeniusView = data; RefreshScreen("save", data.Id); data ? DevExpress.ui.notify(viewUtilsManager.DXTranslate('roLayoutSaved'), 'success', 2000) : DevExpress.ui.notify(viewUtilsManager.DXTranslate('roLayoutSaveError'), 'error', 2000); runGeniusView(); },
                (error) => console.error(error),
                null
            );
            $("#reportConfigurationPopup").dxPopup("instance").hide();
        }
        else
            DevExpress.ui.notify(validationInfo.msg, 'error', 2000);
    }

    const updateGeniusView = () => {
        var execModel = createUpdateModel();
        var validationInfo = isValid(execModel);
        if (validationInfo.valid) {
            viewUtilsManager.makeServiceCall(
                "Genius",
                "saveGeniusView",
                "POST",
                { "geniusView": execModel },
                null,
                (data) => {
                    currentGeniusView = data; execModel = data; RefreshScreen("save", data.Id); data ? DevExpress.ui.notify(viewUtilsManager.DXTranslate('roLayoutSaved'), 'success', 2000) : DevExpress.ui.notify(viewUtilsManager.DXTranslate('roLayoutSaveError'), 'error', 2000); clearForm(); loadGeniusSelected();
                },
                (error) => console.error(error),
                null
            );
            $("#reportConfigurationPopup").dxPopup("instance").hide();
        }
        else
            DevExpress.ui.notify(validationInfo.msg, 'error', 2000);
    }

    function isValidByCheckBox(model) {
        var validationInfo = { valid: true, msg: "" };
        if (isCostCenterCustomView(model) && (model.BusinessCenters == null || model.BusinessCenters == ""))
            validationInfo = { valid: false, msg: viewUtilsManager.DXTranslate('roLayoutSaveCostCenterError') };
        if (isBalancesCustomView(model) && (model.Concepts == null || model.Concepts == ""))
            validationInfo = { valid: false, msg: viewUtilsManager.DXTranslate('roLayoutSaveConceptsError') };
        if (isRequestsCustomView(model) && (model.CustomFields == null || model.CustomFields.RequestTypes == null || model.CustomFields.RequestTypes == ""))
            validationInfo = { valid: false, msg: viewUtilsManager.DXTranslate('roLayoutSaveRequestsError') };
        if (model.Employees == null || model.Employees == "")
            validationInfo = { valid: false, msg: viewUtilsManager.DXTranslate('MSG_VALIDATION_EMPLOYEE') };
        if (model.Name == null || model.Name == "")
            validationInfo = { valid: false, msg: viewUtilsManager.DXTranslate('roLayoutSaveNameError') };
        if (model.CheckedCheckBoxes == null || model.CheckedCheckBoxes == "")
            validationInfo = { valid: false, msg: viewUtilsManager.DXTranslate('roLayoutSaveCheckboxError') };
        if ((model.DateInf == null || model.DateInf == "" || model.DateSup == null || model.DateSup == "") && !updating)
            validationInfo = { valid: false, msg: viewUtilsManager.DXTranslate('geniusWarning') };
        return validationInfo;
    }

    function isValid(model) {
        var validationInfo = { valid: true, msg: "" };
        if (isCostCenterDefaultView(model) && (model.BusinessCenters == null || model.BusinessCenters == ""))
            validationInfo = { valid: false, msg: viewUtilsManager.DXTranslate('roLayoutSaveCostCenterError') };
        if (isBalancesDefaultView(model) && (model.Concepts == null || model.Concepts == ""))
            validationInfo = { valid: false, msg: viewUtilsManager.DXTranslate('roLayoutSaveConceptsError') };
        if (isRequestsDefaultView(model) && (model.CustomFields == null || model.CustomFields.RequestTypes == null || model.CustomFields.RequestTypes == ""))
            validationInfo = { valid: false, msg: viewUtilsManager.DXTranslate('roLayoutSaveRequestsError') };
        if (model.Employees == null || model.Employees == "")
            validationInfo = { valid: false, msg: viewUtilsManager.DXTranslate('MSG_VALIDATION_EMPLOYEE') };
        if (model.Name == null || model.Name == "")
            validationInfo = { valid: false, msg: viewUtilsManager.DXTranslate('roLayoutSaveNameError') };
        if ((model.DateInf == null || model.DateInf == "" || model.DateSup == null || model.DateSup == "") && !updating)
            validationInfo = { valid: false, msg: viewUtilsManager.DXTranslate('geniusWarning') };
        return validationInfo;
    }

    const saveCustomView = () => {
        var execModel = createNewModel();
        var validationInfo = isValidByCheckBox(execModel);
        if (validationInfo.valid) {
            viewUtilsManager.makeServiceCall(
                "Genius",
                "saveCustomView",
                "POST",
                { "geniusView": execModel },
                null,
                (data) => {
                    currentGeniusView = data;
                    execModel = data;
                    RefreshScreen("save", data.Id); data ? DevExpress.ui.notify(viewUtilsManager.DXTranslate('roLayoutSaved'), 'success', 2000) : DevExpress.ui.notify(viewUtilsManager.DXTranslate('roLayoutSaveError'), 'error', 2000);
                    runCustomGeniusView(execModel);
                },
                (error) => DevExpress.ui.notify(viewUtilsManager.DXTranslate('roInvalidCombination'), 'error', 2000),
                null
            );
        }
        else
            DevExpress.ui.notify(validationInfo.msg, 'error', 2000);
    }

    function clearNewForm() {
        $("#txtName").dxTextBox("instance").option("value", null);
        resetCheckboxes();
        $("#ReportUsersNew").dxTextBox("instance").option("value", null);
        $("#ReportUsersNew").dxTextBox("instance").option("value", null);
        $("#UserFieldsText").dxTagBox("instance").option("value", null);
        $("#dtDateInfNew").dxDateBox("instance").option("value", null);
        $("#dtDateInfNew").dxDateBox("instance").repaint();
        $("#dtDateSupNew").dxDateBox("instance").option("value", null);
        $("#dtDateSupNew").dxDateBox("instance").repaint();
        if (typeof lstCostCenters !== 'undefined')
            lstCostCenters.UnselectAll();
        if (typeof lstConcepts !== 'undefined')
            lstConcepts.UnselectAll();
        if (typeof lstCauses !== 'undefined') {
            $("#ckIncludeAllCauses").dxRadioGroup("instance").option("value", "0");
            lstCauses.UnselectAll();
        }
        if (typeof lstRequests !== 'undefined')
            lstRequests.UnselectAll();
        $("#ckIncludeConceptsWithZerosNew").dxCheckBox("instance").option("value", false);
        $("#ckIncludeCausesWithZerosNew").dxCheckBox("instance").option("value", false);
        $("#ckIncludeBusinessCenterWithZerosNew").dxCheckBox("instance").option("value", true);
        $("#ckSendEmail").dxCheckBox("instance").option("value", false);
        if (BIIntegration == "True") {
            $("#ckDownloadBI").dxCheckBox("instance").option("value", false);
            $("#ckOverwriteResults").dxCheckBox("instance").option("value", false);
        }
    }

    function clearExecutionForm() {
        $("#dtDateInf").dxDateBox("instance").option("value", null);
        $("#dtDateInf").dxDateBox("instance").repaint();
        $("#dtDateSup").dxDateBox("instance").option("value", null);
        $("#dtDateSup").dxDateBox("instance").repaint();
        $("#txtInitialTime").dxDateBox("instance").option("value", null);
        $("#txtInitialTime").dxDateBox("instance").repaint();
        $("#txtEndTime").dxDateBox("instance").option("value", null);
        $("#txtEndTime").dxDateBox("instance").repaint();
    }

    function clearForm() {
        $("#ReportUsers").dxTextBox("instance").option("value", null);
        $("#UserFieldsUpdateText").dxTagBox("instance").option("value", null);
        $("#txtRequests").dxTagBox("instance").option("value", null);
        $("#txtConcepts").dxTagBox("instance").option("value", null);
        $("#txtCostCenters").dxTagBox("instance").option("value", null);
    }

    function getAzureFileInfo(data) {
        viewUtilsManager.makeServiceCall(
            "Genius",
            "GetAzureFileInfo",
            "POST",
            { "id": data.Id },
            null,
            (data) => {
                if (data && typeof data.FileSize != 'undefined') {
                    if (data.FileSize <= 520192000)
                        switchToExecution(data);
                    else { DevExpress.ui.notify(viewUtilsManager.DXTranslate('roLargeFileError'), 'error', 2000); }
                }
                else { DevExpress.ui.notify(viewUtilsManager.DXTranslate('roDownloadFileError'), 'error', 2000); }
            },
            (error) => console.error(error),
            null
        );
    }

    const createGeniusView = () => {
        var execModel = createModel();

        var validationInfo = isValid(execModel);
        if (validationInfo.valid) {
            viewUtilsManager.makeServiceCall(
                "Genius",
                "createGeniusView",
                "POST",
                { "geniusView": execModel },
                null,
                (data) => { currentGeniusView = data; RefreshScreen("save", data.Id); data ? DevExpress.ui.notify(viewUtilsManager.DXTranslate('roLayoutSaved'), 'success', 2000) : DevExpress.ui.notify(viewUtilsManager.DXTranslate('roLayoutSaveError'), 'error', 2000); runGeniusView(); clearForm(); },
                (error) => console.error(error),
                null
            );
            $("#reportConfigurationPopup").dxPopup("instance").hide();
        }
        else
            DevExpress.ui.notify(validationInfo.msg, 'error', 2000);
    }

    const backToLauncher = () => {
        $('#geniusAnalyticViewer').hide();
        $('#geniusAnalyticLauncher').show();
        $('#geniusAnalyticNew').show();
        if (currentGeniusView != null) selectCardById(currentGeniusView.Id);
        currentExecution = null;
        if (flGrid != null) {
            flGrid.dispose();
            $('#flxAnalytic').empty();
            flGrid = null;
        }
    }

    const switchToExecution = (data) => {
        if (!currentGeniusView.CustomFields.DownloadBI) {
            $('#geniusAnalyticViewer').show();
            $('#geniusAnalyticLauncher').hide();
            $('#geniusAnalyticNew').hide();

            var langKey = JSON.parse(localStorage.getItem("roLanguage")).key;
            if (data.ExecutionLanguage != null)
                langKey = data.ExecutionLanguage;

            currentExecution = data;

            if (flGrid != null) {
                flGrid.dispose();
                $('#flxAnalytic').empty();
                flGrid = null;
            }            

            var reportConfig = {
                dataSourceType: "json",
                filename: data.AzureSaSKey,
                options: {
                    defaultDateType: "date string",
                    datePattern: shortFormat
                },
                formats: [{
                    name: "",
                    thousandsSeparator: "",
                    decimalSeparator: ",",
                    decimalPlaces: 2
                }]
            };
            

            if (data.CubeLayout != '') {
                var reportDefaultConfig = JSON.parse(data.CubeLayout);
                for (const [key, value] of Object.entries(reportDefaultConfig)) {
                    if (reportConfig[key] == null)
                        reportConfig[key] = reportDefaultConfig[key];
                    else {
                        if (Array.isArray(reportConfig[key]) && Array.isArray(reportDefaultConfig[key])) {
                            reportConfig[key] = [...reportConfig[key], ...reportDefaultConfig[key]];
                        } else {
                            reportConfig[key] = { ...reportConfig[key], ...reportDefaultConfig[key] };
                        }

                    }                        
                }
            }

            if (typeof reportConfig.slice == 'undefined') {
                reportConfig['slice'] = {
                    "expands": { "expandAll": true }
                };
            } else {
                if (typeof reportConfig.slice.expands == 'undefined') {
                    reportConfig.slice["expands"] = {
                        "expandAll": true
                    };
                }
            }

            if (typeof reportConfig.slice.measures != 'undefined' && reportConfig.slice.measures.some(e => e.uniqueName === 'dynamic_concepts')) {
                reportConfig.slice.measures = [];
                for (var i = 0; i < currentGeniusView.Concepts.split(',').length; i++) {
                    var concept = concepts.filter(c => c.ID == currentGeniusView.Concepts.split(',')[i]);
                    if (concept != null && concept != undefined && concept.length > 0)
                        reportConfig.slice.measures = reportConfig.slice.measures.add({ uniqueName: (199 + concept[0].ID) + '', caption: concept[0].Name + "(HH:MM)", formula: 'sum(\'' + (99 + concept[0].ID) + '\')' })
                }
            }                                    

            flGrid = new Flexmonster({
                container: "flxAnalytic",
                componentFolder: FLEX_BASE_URL,
                beforetoolbarcreated: customizeToolbar,
                toolbar: true,
                height: 800,
                report: reportConfig,
                customizeCell: customCellText,
                customizeContextMenu: customContextMenu,
                global: {
                    localization: FLEX_BASE_URL + "locale/" + langKey + ".json"
                },
                licenseKey: _LCODE_                
            });                                    

            flGrid.on('update', function () {
                if (isGeniusAdvanced == "False") {
                    $("#fm-tab-format-cells").addClass("disabled");
                    $("#fm-tab-format-conditional").addClass("disabled");
                }
            });

            if (isGeniusAdvanced == "False") {
                flGrid.setOptions({ showCalculatedValuesButton: false });
                flGrid.refresh();
            }
        }
    };

    function convertNumToTime(number) {
        var sign = (number >= 0) ? 1 : -1;
        number = number * sign;
        var hour = Math.floor(number);
        var decpart = number - hour;
        var min = 1 / 60;
        decpart = min * Math.round(decpart / min);
        var minute = Math.floor(decpart * 60) + '';
        if (minute.length < 2) {
            minute = '0' + minute;
        }
        if (minute == '60') {
            minute = '00';
            hour = hour + 1;
        }
        sign = sign == 1 ? '' : '-';
        time = sign + hour + ':' + minute;
        return time;
    }

    function customContextMenu(items, data, viewType) {
        if (!isNaN(data.value)) {
            if (typeof currentGeniusView.ContextMenu != null && currentGeniusView.ContextMenu != "") {
                var contextMenuOptions = JSON.parse(currentGeniusView.ContextMenu)
                var i;
                for (i = 0; i < contextMenuOptions.length; i++) {
                    var showOption = true;
                    var parameters = "";
                    if (typeof contextMenuOptions[i].parameters != undefined && contextMenuOptions[i].parameters != "") {
                        var j;
                        for (j = 0; j < contextMenuOptions[i].parameters.length; j++) {
                            var rowParameter = data.rows.filter(element => element.dimensionName.toString() == contextMenuOptions[i].parameters[j].toString());
                            var columnParameter = data.columns.filter(element => element.dimensionName.toString() == contextMenuOptions[i].parameters[j].toString());
                            if (rowParameter.length == 0 && columnParameter.length == 0) {
                                showOption = false;
                                break;
                            }
                            else {
                                if (rowParameter.length > 0)
                                    parameters += " " + rowParameter[0].caption;
                                else
                                    parameters += " " + columnParameter[0].caption;
                            }
                        }
                    }
                    if (showOption) {
                        items.push({
                            id: i,
                            parameters: parameters,
                            label: viewUtilsManager.DXTranslate(contextMenuOptions[i].text),
                            handler: function () {
                                alert("Action = " + contextMenuOptions[this.id].action + " - " + contextMenuOptions[this.id].link + " - " + this.parameters);
                            }
                        });
                    }
                }
            }
        }
        return items;
    }

    function customCellText(cell, data) {
        if ((data.measure != null && data.measure.uniqueName.indexOf('_ToHours') > 0) ||
            (data.measure != null && data.measure.caption.indexOf('_ToHours') > 0) ||
            (data.measure != null && data.measure.caption.indexOf('(HH:MM)') > 0)
        ) {
            var cellText = cell.text.replace(/\s/g, '').replace(",", ".");
            if (!isNaN(cellText)) {
                if (cellText != "") {
                    cell.text = convertNumToTime(cellText);
                }
            }
        } else if (cell.text == 'Infinity') {
            cell.text = '';
        } else if (data.member != null && data.member.hierarchyCaption != null && (data.member.hierarchyCaption == 'Dia semana' || data.member.hierarchyCaption == 'Dia setmana' || data.member.hierarchyCaption == 'Asteko eguna' || data.member.hierarchyCaption == 'Day week' || data.member.hierarchyCaption == 'Jour semaine' || data.member.hierarchyCaption == 'Giorno settimana' || data.member.hierarchyCaption == 'Dia da semana')) {
            cell.text = cell.text.split("- ")[1];
        }
    }

    function customizeToolbar(toolbar) {
        // get all tabs
        var tabs = toolbar.getTabs();
        toolbar.getTabs = function () {
            // add new tab at desired index
            // arr.splice(desired_index, 0, item_to_insert)
            tabs.shift();
            tabs.shift();
            tabs.shift();
            tabs.splice(0, 0, {
                id: "saveLayout",
                title: viewUtilsManager.DXTranslate('roScheduleYes'),
                handler: saveGeniusLayout,
                icon: this.icons.save
            });
            tabs.splice(0, 0, {
                divider: "true"
            });
            tabs.splice(0, 0, {
                id: "backToLauncher",
                title: "Volver",
                handler: backToLauncher,
                icon: ic
            });

            tabs[3].menu = tabs[3].menu.slice(2, 4).concat(tabs[3].menu.slice(5, 6));
            return tabs;
        }

        var ic = "<svg xmlns='http://www.w3.org/2000/svg'  viewBox='0 0 24 24' width='42px' height='42px'><path d='M10.121,14.121c0.586-0.586,6.414-6.414,7-7c1.172-1.172,1.172-3.071,0-4.243	c-1.172-1.172-3.071-1.172-4.243,0c-0.586,0.586-6.414,6.414-7,7c-1.172,1.172-1.172,3.071,0,4.243	C7.05,15.293,8.95,15.293,10.121,14.121z' opacity='.35'/><path d='M5.879,14.121c0.586,0.586,6.414,6.414,7,7c1.172,1.172,3.071,1.172,4.243,0c1.172-1.172,1.172-3.071,0-4.243	c-0.586-0.586-6.414-6.414-7-7c-1.172-1.172-3.071-1.172-4.243,0C4.707,11.05,4.707,12.95,5.879,14.121z'/></svg>"
    }

    const refreshExecutions = (response) => {
        currentExecution = response;
        waitForExecutionToFinish();

        $("#popupContainer").dxPopup({
            showTitle: false,
            visible: true,
            width: 400,
            height: 220,
            contentTemplate: function (contentElement) {
                contentElement.append(

                    $("<div class='waitingFile'/>").append(
                        $("<p />").text(viewUtilsManager.DXTranslate('roGeniusContinueText')),
                        $("<div />").dxLoadIndicator({
                            height: 60,
                            width: 60
                        }),
                        $("<br />"),
                        $("<br />"),
                        $("<div />").attr("id", "buttonContainer").dxButton({
                            text: viewUtilsManager.DXTranslate('roGeniusContinue'),
                            type: "default",
                            onClick: function (e) {
                                $("#popupContainer").dxPopup("toggle", false);
                                clearTimeout(checkForFinishTimeout);
                                selectCardById(currentGeniusView.Id);
                            }
                        }))
                )
            }
        });

        $("#popupContainer").dxPopup("toggle", true);
    }

    const deleteGeniusExecution = async (e) => {
        window.loadingRequest = true;
        try {
            await $.ajax({
                url: `${BASE_URL}Genius/DeleteGeniusBIExecution`,
                data: { geniusExecutionID: e.data.ID, fileName: e.data.FileName },
                type: "DELETE",
                dataType: "json",
                success: (data) => {
                    console.log(data);
                    //OK
                    const toast = $('#toastBIDeleted').dxToast({ displayTime: 1500 }).dxToast('instance');
                    toast.option({
                        type: 'success', message: "Ejecución eliminada correctamente"
                    });
                    toast.show();
                },
                error: (error) => {
                    console.error(error);
                    e.cancel = true;
                    const toast = $('#toastBIDeleted').dxToast({ displayTime: 2000 }).dxToast('instance');
                    toast.option({
                        type: 'error', message: "Ha habido un problema al eliminar la ejecución"
                    });
                    toast.show();
                },
            });
        } catch (error) {
            console.error(error);
            e.cancel = true;
            const toast = $('#toastBIDeleted').dxToast({ displayTime: 2000 }).dxToast('instance');
            toast.option({
                type: 'error', message: "Ha habido un problema al eliminar la ejecución"
            });
            toast.show();
        } finally {
            window.loadingRequest = false;
        }
    };

    const waitForExecutionToFinish = () => {
        viewUtilsManager.makeServiceCall(
            "Genius",
            "getGeniusViewStatus",
            "POST",
            { "id": currentExecution.Id },
            null,
            (data) => {
                if (data) {
                    loadGeniusByTask(currentExecution.IdTask);
                    $("#popupContainer").dxPopup("toggle", false);
                }
                else checkForFinishTimeout = setTimeout(() => { window.waitForExecutionToFinish() }, 2000);
            },
            (error) => console.error(error),
            null
        );
    };

    const editCurrentGenius = () => {
        $("#reportConfigurationPopup").dxPopup("instance").show();
        setTimeout(() => $("#reportConfigurationPopup").dxPopup("instance").repaint(), 0);
        $(".updateReport").show();
        $(".runReport").hide();
        updating = true;
    }

    function waitForRefreshCardTreeToFinish() {
        const targetNode = document.body; // Puedes ajustar esto según donde esperes cambios en el DOM
        const config = { childList: true, subtree: true };

        return new Promise((resolve) => {
            const observer = new MutationObserver(() => {
                if (!targetNode.contains(document.querySelector(".dxcvCardToUpdate"))) {
                    observer.disconnect();
                    resolve();
                }
            });

            observer.observe(targetNode, config);
        });
    }

    async function RefreshScreen(DataType, params) {
        if (DataType === 'save') {
            $(".dxcvCard").addClass("dxcvCardToUpdate");
            refreshCardTree(params);

            await waitForRefreshCardTreeToFinish();

            $('.dxcvCard').off("contextmenu");
            $('.dxcvCard').on("contextmenu", (e) => {
                showSharedContextMenu(e);
            });
        }
        else {
            bLaunchOnCallback = false;
        }
    }

    const openReportConfigurationPopUp = () => {
        updating = false;
        $("#reportConfigurationPopup").dxPopup("instance").show();
        setTimeout(() => $("#reportConfigurationPopup").dxPopup("instance").repaint(), 0);
    }

    const loadGeniusSelected = () => {
        $('#geniusAnalyticViewer').hide();
        $('#geniusAnalyticLauncher').show();
        $('#geniusAnalyticNew').show();
        $("#PlanningList").dxDataGrid("instance").cancelEditData();
        viewUtilsManager.makeServiceCall(
            "Genius",
            "GetGeniusView",
            "POST",
            { id: viewUtilsManager.getSelectedCardId() },
            null,
            function (response) {
                if (response != null) {
                    data = response;
                    bindGeniusView(response);

                    if (isConsultor == "False")
                        $('.btnConvert').parent().hide();
                    else
                        $('.btnConvert').parent().show();

                    if (isGeniusAdvanced == "False")
                        $('.btnShare').parent().hide();
                    else
                        $('.btnShare').parent().show();

                    if (response.IdPassport == 0) {
                        $('.btnTbDel2').parent().hide();
                        $('.btnEditGenius').parent().hide();
                        $('.btnShare').parent().hide();
                        $('.btnConvert').parent().hide();
                        $('.btnAddPlanification').parent().hide();
                        $('#geniusPlanification').hide();
                    }
                    else {
                        $('.btnTbDel2').parent().show();
                        $('.btnEditGenius').parent().show();
                        $('.btnAddPlanification').parent().show();
                        $('#geniusPlanification').show();
                    }
                }
                else {
                    DevExpress.ui.notify(viewUtilsManager.DXTranslate('roPermissionsError'), 'error', 2000);
                }
            },
            null,
            null
        );
    }

    const saveData = async () => {
        window.loadingRequest = true;
    };

    function createModel() {
        var model = { ...currentGeniusView };
        if (model.Id == null) {
            let id = parseInt(viewUtilsManager.getSelectedCardId(), 10);
            model.Id = id;
        }
        if (viewUtilsManager.getHtmlControl("rgDateFilterType") != null) {
            //model.DateFilterType = viewUtilsManager.getHtmlControl("rgDateFilterType").option('value');
            model.DateInf = viewUtilsManager.getHtmlControl("dtDateInf").option('value');
            model.DateSup = viewUtilsManager.getHtmlControl("dtDateSup").option('value');

            if (model.DateInf != null) {
                if (typeof model.DateInf == 'string') model.DateInf = new Date(model.DateInf).toISOString();
                else model.DateInf = model.DateInf.toISOString();
            }

            if (model.DateSup != null) {
                if (typeof model.DateSup == 'string') model.DateSup = new Date(model.DateSup).toISOString();
                else model.DateSup = model.DateSup.toISOString();
            }
        }

        if ($("#txtInitialTime").dxDateBox("instance") != null) {
            var initialTime = $("#txtInitialTime").dxDateBox("instance").option("value");
            if (initialTime != null)
                model.TimeInf = initialTime.toISOString();
            else
                model.TimeInf = null;
        }

        if ($("#txtEndTime").dxDateBox("instance") != null) {
            var endTime = $("#txtEndTime").dxDateBox("instance").option("value");
            if (endTime != null)
                model.TimeSup = endTime.toISOString();
            else
                model.TimeSup = null;
        }

        var costCentersSelected = costCentersSelected2StrTagBox();
        if (costCentersSelected != "")
            model.BusinessCenters = costCentersSelected;

        var conceptsSelected = conceptsSelected2StrTagBox();
        if (conceptsSelected != "")
            model.Concepts = conceptsSelected;

        var causesSelected = causesSelected2Str();
        if (causesSelected != "") model.Causes = causesSelected;

        if (model.CheckedCheckBoxes == null)
            model.CheckedCheckBoxes = "";

        var requestsSelected = requestsSelected2StrTagBox();
        model.CustomFields.RequestTypes = requestsSelected;

        model.Executions = [];

        return model;
    }

    function createNewModel() {
        var model = {};

        if ($("#ReportUsersNew").dxTextBox("instance").option("value") != null && $("#ReportUsersNew").dxTextBox("instance").option("value") != '')
            model.Employees = currentGeniusView.Employees;
        else
            model.Employees = "";
        model.DateInf = viewUtilsManager.getHtmlControl("dtDateInfNew").option('value');
        model.DateSup = viewUtilsManager.getHtmlControl("dtDateSupNew").option('value');

        if (model.DateInf != null) {
            if (typeof model.DateInf == 'string') model.DateInf = new Date(model.DateInf).toISOString();
            else model.DateInf = model.DateInf.toISOString();
        }

        if (model.DateSup != null) {
            if (typeof model.DateSup == 'string') model.DateSup = new Date(model.DateSup).toISOString();
            else model.DateSup = model.DateSup.toISOString();
        }
        model.Executions = [];

        model.Name = $("#txtName").dxTextBox("instance").option("value");
        var checkedCheckboxes = getCheckedCheckboxes();
        model.CheckedCheckBoxes = checkedCheckboxes;
        var customFieldsSelected = $("#UserFieldsText").dxTagBox('instance').option('selectedItems');
        var customFieldsSelected2Str = "";
        for (var i = 0; i < customFieldsSelected.length; i++) {
            if (i == 0)
                customFieldsSelected2Str = customFieldsSelected[i].Name;
            else
                customFieldsSelected2Str += "," + customFieldsSelected[i].Name;
        }
        if (customFieldsSelected2Str != "")
            model.UserFields = customFieldsSelected2Str;
        var costCentersSelected = costCentersSelected2Str();
        if (costCentersSelected != "")
            model.BusinessCenters = costCentersSelected;

        var conceptsSelected = conceptsSelected2Str();
        if (conceptsSelected != "")
            model.Concepts = conceptsSelected;

        var causesSelected = causesSelected2Str();
        if (causesSelected != "") model.Causes = causesSelected;

        model.CustomFields = new Object();
        model.CustomFields.IncludeZeros = $("#ckIncludeConceptsWithZerosNew").dxCheckBox('instance').option('value');
        model.CustomFields.IncludeZeroCauses = $("#ckIncludeCausesWithZerosNew").dxCheckBox('instance').option('value');
        model.CustomFields.IncludeZeroBusinessCenter = $("#ckIncludeBusinessCenterWithZerosNew").dxCheckBox('instance').option('value');

        model.CustomFields.SendEmail = $("#ckSendEmail").dxCheckBox('instance').option('value');
        if (BIIntegration == "True") {
            model.CustomFields.DownloadBI = $("#ckDownloadBI").dxCheckBox('instance').option('value');
            model.CustomFields.OverwriteResults = $("#ckOverwriteResults").dxCheckBox('instance').option('value');
        }
        var requestsSelected = requestsSelected2Str();
        if (requestsSelected != "")
            model.CustomFields.RequestTypes = requestsSelected;

        return model;
    }

    function createUpdateModel() {
        var model = { ...currentGeniusView };
        if (model.Id == null) {
            let id = parseInt(viewUtilsManager.getSelectedCardId(), 10);
            model.Id = id;
        }

        model.Employees = currentGeniusView.Employees;
        model.Name = $("#txtUpdatedName").dxTextBox("instance").option("value");
        model.CustomFields.IncludeZeros = $("#ckIncludeConceptsWithZeros").dxCheckBox('instance').option('value');
        model.CustomFields.IncludeZeroCauses = $("#ckIncludeCausesWithZeros").dxCheckBox('instance').option('value');
        model.CustomFields.IncludeZeroBusinessCenter = $("#ckIncludeBusinessCenterWithZeros").dxCheckBox('instance').option('value');
        model.CustomFields.LanguageKey = $("#selectLanguage").dxSelectBox('instance').option('value');
        var customFieldsSelected = $("#UserFieldsUpdateText").dxTagBox('instance').option('selectedItems');
        var customFieldsSelected2Str = "";
        for (var i = 0; i < customFieldsSelected.length; i++) {
            if (i == 0)
                customFieldsSelected2Str = customFieldsSelected[i].Name;
            else
                customFieldsSelected2Str += "," + customFieldsSelected[i].Name;
        }
        model.UserFields = customFieldsSelected2Str;
        var costCentersSelected = costCentersSelected2StrTagBox();
        model.BusinessCenters = costCentersSelected;
        var conceptsSelected = conceptsSelected2StrTagBox();
        model.Concepts = conceptsSelected;

        if (currentGeniusEditParameters.UpdatedCauses) {
            var causesSelected = causesSelected2Str();
            model.Causes = causesSelected;
        } else {
            model.Causes = currentGeniusEditParameters.Causes;
        }

        var requestsSelected = requestsSelected2StrTagBox();
        model.CustomFields.RequestTypes = requestsSelected;
        model.CustomFields.SendEmail = $("#ckUpdatedSendEmail").dxCheckBox('instance').option('value');
        if (BIIntegration == "True") {
            model.CustomFields.DownloadBI = $("#ckUpdatedDownloadBI").dxCheckBox('instance').option('value');
            model.CustomFields.OverwriteResults = $("#ckUpdatedOverwriteResults").dxCheckBox('instance').option('value');
        }
        return model;
    }

    const deleteGeniusView = async () => {
        window.loadingRequest = true;

        let model = createModel();
        if (validateModel(model)) {
            try {
                await $.ajax({
                    url: `${BASE_URL}Genius/DeleteGeniusView`,
                    data: { geniusView: model },
                    type: "DELETE",
                    dataType: "json",
                    success: (data) => {
                        currentGeniusView = null;
                    },
                    error: (error) => console.error(error),
                });
            } catch (error) {
                console.error(error);
                response = false;
            } finally {
                window.refreshCardTree("-1");
                window.loadingRequest = false;
            }
        }
    };

    const loadGeniusByTask = async (idTask) => {
        let response = null;
        await $.ajax({
            url: `${BASE_URL}Genius/GetGeniusByTask`,
            data: { id: idTask },
            type: "POST",
            dataType: "json",
            success: (data) => {
                if (data != null) {
                    response = data;
                    if (data.Id > 0 && response.Executions.length > 0) {
                        selectCardById(data.Id);
                        bindGeniusView(response);
                        if (!currentGeniusView.CustomFields.DownloadBI)
                            getAzureFileInfo(response.Executions[0]);
                    } else {
                        DevExpress.ui.notify(viewUtilsManager.DXTranslate('roNoSelectedTaskAvailable'), 'error', 2000);
                    }
                } else {
                    DevExpress.ui.notify(viewUtilsManager.DXTranslate('roPermissionsError'), 'error', 2000);
                }
            },
            error: (error) => DevExpress.ui.notify(viewUtilsManager.DXTranslate('roNoSelectedTaskAvailable'), 'error', 2000),
        });
    }

    var validationDeleteGeniusView = function () {
        if (currentGeniusView != null) {
            if (currentGeniusView.PlanningList != null && currentGeniusView.PlanningList.length > 0) {
                DevExpress.ui.notify(viewUtilsManager.DXTranslate('roHasPlanifications'), 'error', 2000);
            }
            else {
                var buttonsDialog = [];
                buttonsDialog.push(viewUtilsManager.buildButtonDialog('Yes', 'Yes', deleteGeniusView));
                buttonsDialog.push(viewUtilsManager.buildButtonDialog('No', 'No', function () { }));

                viewUtilsManager.showModalDialog(this.parent.Robotics.Client.JSErrors.JSErrorTypes.roJsWarning,
                    '',
                    'roJsWarning', 'roGeniusConfirmDelete',
                    buttonsDialog);
            }
        } else {
            DevExpress.ui.notify(viewUtilsManager.DXTranslate('roNoGeniusViewSelected'), 'error', 2000);
        }
    };

    function validateModel(model) {
        //TODO: Validate Model Data
        return true;
    }

    function runGeniusView() {
        if (currentGeniusView != null) {
            var execModel = createModel();

            var bLaunchConfig = false;
            if (currentGeniusView.DSFunction != '') {
                if (currentGeniusView.DSFunction.indexOf('employeeFilter') > 0 && currentGeniusView.Employees == '') bLaunchConfig = true;

                if (currentGeniusView.DSFunction.indexOf('conceptsFilter') > 0 && currentGeniusView.Concepts == '') bLaunchConfig = true;

                if (currentGeniusView.DSFunction.indexOf('businessCenterFilter') > 0 && currentGeniusView.BusinessCenters == '') bLaunchConfig = true;
            } else {
                if (currentGeniusView.DS != "B") {
                    if (currentGeniusView.Employees == '') bLaunchConfig = true;
                } else {
                    if (currentGeniusView.Employees == '' && currentGeniusView.TypeView != 1) bLaunchConfig = true;
                }

                if (currentGeniusView.DS == "S" && currentGeniusView.TypeView == 1 && currentGeniusView.Concepts == '') bLaunchConfig = true;

                if (currentGeniusView.DS == "B" && currentGeniusView.BusinessCenters == '') bLaunchConfig = true;
            }

            if (!bLaunchConfig) {
                if (execModel.DateInf != null && execModel.DateSup != null && (!isJustificationsByTimeDefaultView(execModel) || execModel.TimeInf != null && execModel.TimeSup != null)) {
                    viewUtilsManager.makeServiceCall(
                        "Genius",
                        "runGeniusView",
                        "POST",
                        { "geniusView": execModel },
                        null,
                        (data) => {
                            if (data != null) refreshExecutions(data);
                            else {
                                DevExpress.ui.notify(viewUtilsManager.DXTranslate('roPermissionsError'), 'error', 2000);
                            }
                        },
                        (error) => console.error(error),
                        null
                    );
                } else {
                    if (execModel.DateInf == null || execModel.DateSup == null) {
                        this.parent.Robotics.Client.JSErrors.showJSerrorPopup(this.parent.Robotics.Client.JSErrors.JSErrorTypes.roJsInfo, '',
                            { text: '', key: 'roJsError' }, { text: '', key: 'geniusWarning' },
                            { text: '', textkey: 'roErrorClose', desc: '', desckey: '', script: '' },
                            this.parent.Robotics.Client.JSErrors.createEmptyButton(), this.parent.Robotics.Client.JSErrors.createEmptyButton(), this.parent.Robotics.Client.JSErrors.createEmptyButton());
                    }
                    else {
                        this.parent.Robotics.Client.JSErrors.showJSerrorPopup(this.parent.Robotics.Client.JSErrors.JSErrorTypes.roJsInfo, '',
                            { text: '', key: 'roJsError' }, { text: '', key: 'geniusTimeWarning' },
                            { text: '', textkey: 'roErrorClose', desc: '', desckey: '', script: '' },
                            this.parent.Robotics.Client.JSErrors.createEmptyButton(), this.parent.Robotics.Client.JSErrors.createEmptyButton(), this.parent.Robotics.Client.JSErrors.createEmptyButton());
                    }
                }
            } else {
                bLaunchOnCallback = false;
                var validationInfo = isValid(execModel);
                DevExpress.ui.notify(validationInfo.msg, 'error', 2000);                
            }
        } else {
            DevExpress.ui.notify(viewUtilsManager.DXTranslate('roNoGeniusViewSelected'), 'error', 2000);
        }
        $("#reportConfigurationPopup").dxPopup("instance").hide();
        clearExecutionForm();
    }

    function addPlanificationCurrentGenius() {
        $(".generalDiv").hide();
        $("#planificationDiv").show();
        $("#geniusGeneral").removeClass("bTabGeniusMenu-active");
        $("#geniusPlanification").addClass("bTabGeniusMenu-active");
        $('#PlanningList').dxDataGrid('instance').addRow();
        $("#geniusPlannerScheduler").show();
        $("#geniusConfigScheduler").show();
    }

    function runCustomGeniusView(execModel) {
        if (currentGeniusView != null) {
            execModel.DateInf = viewUtilsManager.getHtmlControl("dtDateInfNew").option('value');
            execModel.DateSup = viewUtilsManager.getHtmlControl("dtDateSupNew").option('value');

            if (execModel.DateInf != null) {
                if (typeof execModel.DateInf == 'string') execModel.DateInf = new Date(execModel.DateInf).toISOString();
                else execModel.DateInf = execModel.DateInf.toISOString();
            }

            if (execModel.DateSup != null) {
                if (typeof execModel.DateSup == 'string') execModel.DateSup = new Date(execModel.DateSup).toISOString();
                else execModel.DateSup = execModel.DateSup.toISOString();
            }
            var bLaunchConfig = false;
            if (currentGeniusView.DSFunction != '') {
                if (currentGeniusView.DSFunction.indexOf('employeeFilter') > 0 && currentGeniusView.Employees == '') bLaunchConfig = true;

                if (currentGeniusView.DSFunction.indexOf('conceptsFilter') > 0 && currentGeniusView.Concepts == '') bLaunchConfig = true;

                if (currentGeniusView.DSFunction.indexOf('businessCenterFilter') > 0 && currentGeniusView.BusinessCenters == '') bLaunchConfig = true;
            } else {
                if (currentGeniusView.DS != "B") {
                    if (currentGeniusView.Employees == '') bLaunchConfig = true;
                } else {
                    if (currentGeniusView.Employees == '' && currentGeniusView.TypeView != 1) bLaunchConfig = true;
                }

                if (currentGeniusView.DS == "S" && currentGeniusView.TypeView == 1 && currentGeniusView.Concepts == '') bLaunchConfig = true;

                if (currentGeniusView.DS == "B" && currentGeniusView.BusinessCenters == '') bLaunchConfig = true;
            }

            if (!bLaunchConfig) {
                if (execModel.DateInf != null && execModel.DateSup != null) {
                    viewUtilsManager.makeServiceCall(
                        "Genius",
                        "runGeniusView",
                        "POST",
                        { "geniusView": execModel },
                        null,
                        (data) => {
                            if (data != null) refreshExecutions(data);
                            else {
                                DevExpress.ui.notify(viewUtilsManager.DXTranslate('roPermissionsError'), 'error', 2000);
                            }
                        },
                        (error) => console.error(error),
                        null
                    );
                } else {
                    this.parent.Robotics.Client.JSErrors.showJSerrorPopup(this.parent.Robotics.Client.JSErrors.JSErrorTypes.roJsInfo, '',
                        { text: '', key: 'roJsError' }, { text: '', key: 'geniusWarning' },
                        { text: '', textkey: 'roErrorClose', desc: '', desckey: '', script: '' },
                        this.parent.Robotics.Client.JSErrors.createEmptyButton(), this.parent.Robotics.Client.JSErrors.createEmptyButton(), this.parent.Robotics.Client.JSErrors.createEmptyButton());
                }
            } else {
                bLaunchOnCallback = true;
            }
        } else {
            DevExpress.ui.notify(viewUtilsManager.DXTranslate('roNoGeniusViewSelected'), 'error', 2000);
        }
        $("#reportConfigurationPopup").dxPopup("instance").hide();
        clearNewForm();
    }

    function saveGeniusPlanification(schedulerModel) {
        if (currentGeniusView != null) {
            viewUtilsManager.makeServiceCall(
                "Genius",
                "InsertNewPlanning",
                "POST",
                { "geniusSchedule": schedulerModel },
                null,
                (data) => {
                    if (data != null) refreshExecutions(data);
                    else {
                        DevExpress.ui.notify(viewUtilsManager.DXTranslate('roPermissionsError'), 'error', 2000);
                    }
                },
                (error) => console.error(error),
                null
            );
        } else {
            this.parent.Robotics.Client.JSErrors.showJSerrorPopup(this.parent.Robotics.Client.JSErrors.JSErrorTypes.roJsInfo, '',
                { text: '', key: 'roJsError' }, { text: '', key: 'geniusWarning' },
                { text: '', textkey: 'roErrorClose', desc: '', desckey: '', script: '' },
                this.parent.Robotics.Client.JSErrors.createEmptyButton(), this.parent.Robotics.Client.JSErrors.createEmptyButton(), this.parent.Robotics.Client.JSErrors.createEmptyButton());
        }
    }

    function saveCostCenterInfo() {
        var popup = $("#CostsCenterPopUp").dxPopup("instance");
        popup.hide();
    }

    function cancelCostCenterInfo() {
        if (typeof lstCostCenters != 'undefined')
            lstCostCenters.UnselectAll();
        var popup = $("#CostsCenterPopUp").dxPopup("instance");
        popup.hide();
    }

    function saveConceptsInfo() {
        var popup = $("#ConceptsPopUp").dxPopup("instance");
        popup.hide();
    }

    function cancelConceptsInfo() {
        lstConcepts.UnselectAll();
        var popup = $("#ConceptsPopUp").dxPopup("instance");
        popup.hide();
    }

    function saveCausesInfo() {
        var popup = $("#CauesPopUp").dxPopup("instance");
        popup.hide();
    }

    function cancelCausesInfo() {
        lstCauses.UnselectAll();
        var popup = $("#CauesPopUp").dxPopup("instance");
        popup.hide();
    }

    function ckIncludeAllCauses_OnChange(s, e) {
        if (s.value == 0) {
            lstCauses.UnselectAll();
            lstCauses.SetEnabled(false);
        } else {
            lstCauses.SetEnabled(true);
        }
    }

    function saveRequestsInfo() {
        var popup = $("#ReuqestsPopUp").dxPopup("instance");
        popup.hide();
    }

    function cancelRequestsInfo() {
        lstRequests.UnselectAll();
        var popup = $("#ReuqestsPopUp").dxPopup("instance");
        popup.hide();
    }

})();

function onReportConfigurationPopUpHidden() {
    $(".updateReport").hide();
    $(".runReport").show();
    loadRequest();
    updating = false;
}

function onReportConfigurationPopupShown() {
    initializePopUpComponents();
    existingView = true;
    viewUtilsManager.loadViewOptions("Genius", "read", function () {
        let iTask = parseInt(SelectedTask, 10);
        if (iTask != -1) loadGeniusByTask(iTask);
    }, () => { }, 'LiveGenius');
    $('#ReportUsers').off("click");
    $('#ReportUsers').on("click", function (e) {

        let currentSelectorView = { Employees: [], Groups: [], Filter: "11110", UserFields: "", ComposeMode: 'Custom', ComposeFilter: "", Advanced: false, Operation: "or" };

        if (currentGeniusView != null) {
            let employeeGroups = currentGeniusView.Employees.split('#')[0].split(',');
            for (let i = 0; i < employeeGroups.length; i++) {
                if (employeeGroups[i].charAt(0) == 'A')
                    currentSelectorView.ComposeFilter += currentSelectorView.ComposeFilter == '' ? `A${employeeGroups[i].substring(1)}` : `,A${employeeGroups[i].substring(1)}`;
                else if (employeeGroups[i].charAt(0) == 'B')
                    currentSelectorView.ComposeFilter += currentSelectorView.ComposeFilter == '' ? `B${employeeGroups[i].substring(1)}` : `,B${employeeGroups[i].substring(1)}`;
            }
        }
        parent.showLoader(true);
        $("#divGeniusEmployeeSelector").load('/Employee/EmployeeSelectorPartial?feature=Employees&pageName=genius&config=100&unionType=or&advancedMode=1&advancedFilter=0&allowAll=0&allowNone=0', function () {
            loadPartialInfo();
            initUniversalSelector(currentSelectorView, false, 'geniusSelector');
            parent.showLoader(false);
        });
        existingView = true;
    });

    currentGeniusEditParameters.Causes = currentGeniusView.Causes;
    currentGeniusEditParameters.UpdatedCauses = false;

    $('#txtCauses').off("click");
    $('#txtCauses').on("click", function (e) {
        $("#CauesPopUp").dxPopup("instance").show();
        setTimeout(function () {
            currentGeniusEditParameters.UpdatedCauses = true;
            if (currentGeniusView.Causes != '') {
                $("#ckIncludeAllCauses").dxRadioGroup("instance").option("value", "1");

                lstCauses.SelectValues(currentGeniusView.Causes.split(","));

                lstCauses.SetEnabled(true);
            } else {
                $("#ckIncludeAllCauses").dxRadioGroup("instance").option("value", "0");
                lstCauses.UnselectAll();
                lstCauses.SetEnabled(false);
            }
        }, 100);
    });

    initializeReportPopUpFields();

    if (isCostCenterDefaultView(currentGeniusView))
        $(".costCenterInfo").show();
    else
        $(".costCenterInfo").hide();

    if (isCostCenterAndIncidencesDefaultView(currentGeniusView))
        $(".costcenterAndIncidencesinfo").show();
    else
        $(".costcenterAndIncidencesinfo").hide();

    if (isBalancesDefaultView(currentGeniusView))
        $(".conceptsInfo").show();
    else
        $(".conceptsInfo").hide();

    if (isCausesDefaultView(currentGeniusView))
        $(".causesInfo").show();
    else
        $(".causesInfo").hide();

    if (isRequestsDefaultView(currentGeniusView))
        $(".requestsInfo").show();
    else
        $(".requestsInfo").hide();

    if (isJustificationsByTimeDefaultView(currentGeniusView) && !updating)
        $(".timeInfo").show();
    else
        $(".timeInfo").hide();

    if (currentGeniusView.IdPassport <= 0) {
        $('.divDestination').show();
        $('#newView').show();
        $('.runsDescriptionPopUp').hide();
        $('#runView').hide();
    }
    else {
        if (!updating) {
            //shared
            if (!currentGeniusView.Employees) {
                $('.divDestination').show();
            } else {
                $('.divDestination').hide();
            }
            $(".costCenterInfo").hide();
            $(".costcenterAndIncidencesinfo").hide();
            $(".conceptsInfo").hide();
            $(".causesInfo").hide();
            $(".zeroAvailable").hide();
            $(".zeroCausesAvailable").hide();
            $(".requestsInfo").hide();
        }
        else {
            $('.divDestination').show();
        }
        $('#newView').hide();
        $('.runsDescriptionPopUp').show();
        $('#runView').show();
    }
    clearNewForm();
}

window.parentCloseAndApplySelector = function (currentSelection) {
    if (currentGeniusView == null) currentGeniusView = new Object();
    currentGeniusView.Employees = currentSelection.ComposeFilter + "#" + currentSelection.Filter + "#" + currentSelection.UserFields;

    if (existingView)
        $("#ReportUsers").dxTextBox("instance").option("value", buildSelectedEmployeesString(currentSelection));
    else
        $("#ReportUsersNew").dxTextBox("instance").option("value", buildSelectedEmployeesString(currentSelection));
}


function initializeUserFieldsTagBox() {
    if (currentGeniusView.UserFields != null && currentGeniusView.UserFields != "") {
        var selectedUserFields = currentGeniusView.UserFields.split(",");
        $("#UserFieldsUpdateText").dxTagBox("instance").option("value", selectedUserFields);
    }
    else
        $("#UserFieldsUpdateText").dxTagBox("instance").option("value", null);
}

function costCentersSelected2Str() {
    if (typeof lstCostCenters !== 'undefined') {
        var businessCentersSelected = lstCostCenters.GetSelectedValues();
        var businessCentersSelected2Str = "";
        for (var i = 0; i < businessCentersSelected.length; i++) {
            if (i == 0)
                businessCentersSelected2Str = businessCentersSelected[i];
            else
                businessCentersSelected2Str += "," + businessCentersSelected[i];
        }
        return businessCentersSelected2Str;
    }
    else
        return "";
}

function costCentersSelected2StrTagBox() {
    if ($("#txtCostCenters").dxTagBox("instance") != null) {
        var businessCentersSelected = $("#txtCostCenters").dxTagBox("instance").option("value");
        if (businessCentersSelected != null) {
            var businessCentersSelected2Str = "";
            for (var i = 0; i < businessCentersSelected.length; i++) {
                if (i == 0)
                    businessCentersSelected2Str = businessCentersSelected[i];
                else
                    businessCentersSelected2Str += "," + businessCentersSelected[i];
            }
            return businessCentersSelected2Str;
        }
        else
            return "";
    }
    else
        return "";
}

function conceptsSelected2Str() {
    if (typeof lstConcepts !== 'undefined') {
        var conceptsSelected = lstConcepts.GetSelectedValues();
        var conceptsSelected2Str = "";
        for (var i = 0; i < conceptsSelected.length; i++) {
            if (i == 0)
                conceptsSelected2Str = conceptsSelected[i];
            else
                conceptsSelected2Str += "," + conceptsSelected[i];
        }
        return conceptsSelected2Str;
    }
    return "";
}

function conceptsSelected2StrTagBox() {
    if ($("#txtConcepts").dxTagBox("instance") != null) {
        var conceptsSelected = $("#txtConcepts").dxTagBox("instance").option("value");
        if (conceptsSelected != null) {
            var conceptssSelected2Str = "";
            for (var i = 0; i < conceptsSelected.length; i++) {
                if (i == 0)
                    conceptssSelected2Str = conceptsSelected[i];
                else
                    conceptssSelected2Str += "," + conceptsSelected[i];
            }
            return conceptssSelected2Str;
        }
        else
            return "";
    }
    return "";
}

function causesSelected2Str() {
    if (typeof lstCauses !== 'undefined') {
        var causesSelected = lstCauses.GetSelectedValues();
        var causesSelected2Str = "";
        for (var i = 0; i < causesSelected.length; i++) {
            if (i == 0)
                causesSelected2Str = causesSelected[i];
            else
                causesSelected2Str += "," + causesSelected[i];
        }
        return causesSelected2Str;
    }
    return "";
}

function requestsSelected2Str() {
    if (typeof lstRequests !== 'undefined') {
        var requestsSelected = lstRequests.GetSelectedValues();
        var requestsSelected2Str = "";
        for (var i = 0; i < requestsSelected.length; i++) {
            if (i == 0)
                requestsSelected2Str = requestsSelected[i];
            else
                requestsSelected2Str += "," + requestsSelected[i];
        }
        return requestsSelected2Str;
    }
    return "";
}

function requestsSelected2StrTagBox() {
    if ($("#txtRequests").dxTagBox("instance") != null) {
        var requestsSelected = $("#txtRequests").dxTagBox("instance").option("value");
        if (requestsSelected != null) {
            var requestssSelected2Str = "";
            for (var i = 0; i < requestsSelected.length; i++) {
                if (i == 0)
                    requestssSelected2Str = requestsSelected[i];
                else
                    requestssSelected2Str += "," + requestsSelected[i];
            }
            return requestssSelected2Str;
        }
        else
            return "";
    }
    return "";
}

function onPlanningListLoaded(e) {
    currentGeniusView.PlanningList = e;
}

function initializeCostCentersTagBox() {
    if (currentGeniusView.BusinessCenters != null && currentGeniusView.BusinessCenters != "") {
        var selectedBusinessCenters = currentGeniusView.BusinessCenters.split(",").map(function (item) {
            return parseInt(item);
        });
        $("#txtCostCenters").dxTagBox("instance").option("value", selectedBusinessCenters);
    }
    else
        $("#txtCostCenters").dxTagBox("instance").option("value", null);
}

function initializeConceptssTagBox() {
    if (currentGeniusView.Concepts != null && currentGeniusView.Concepts != "") {
        var selectedConcepts = currentGeniusView.Concepts.split(",").map(function (item) {
            return parseInt(item);
        });
        $("#txtConcepts").dxTagBox("instance").option("value", selectedConcepts);
    }
    else
        $("#txtConcepts").dxTagBox("instance").option("value", null);
}

function initializeRequestsTagBox() {
    if (currentGeniusView.CustomFields != null && currentGeniusView.CustomFields.RequestTypes != null && currentGeniusView.CustomFields.RequestTypes != "") {
        var selectedRequests = currentGeniusView.CustomFields.RequestTypes.split(",").map(function (item) {
            return parseInt(item);
        });
        $("#txtRequests").dxTagBox("instance").option("value", selectedRequests);
    }
    else
        $("#txtRequests").dxTagBox("instance").option("value", null);
}

function initializeCheckedChekboxes() {
    resetReadCheckboxes();
    if (currentGeniusView.CheckedCheckBoxes != null && currentGeniusView.CustomFields.CheckedCheckBoxes != "") {
        var checkedCheckboxes = currentGeniusView.CheckedCheckBoxes;
        for (var i = 0; i < checkedCheckboxes.length; i++)
            $("#chkCheckbox" + checkedCheckboxes[i]).dxCheckBox("instance").option("value", true);
    }
}

function initializeEmployeeTextFields() {
    var selectedEmployees = [];
    var selectedGroups = [];
    if (currentGeniusView.Employees != '') {
        var employeeGroups = currentGeniusView.Employees.split('#')[0].split(',');
        var i = 0;
        for (i = 0; i < employeeGroups.length; i++)
            if (employeeGroups[i].charAt(0) == 'A')
                selectedGroups.append(employeeGroups[i].substring(1, employeeGroups[i].length));
            else
                selectedEmployees.append(employeeGroups[i].substring(1, employeeGroups[i].length));
        if (selectedEmployees.length > 0 && selectedGroups.length > 0) {
            $("#ReportUsers").dxTextBox("instance").option("value", selectedEmployees.length + ' ' + viewUtilsManager.DXTranslate('roGeniusEmployees') + ' ' + viewUtilsManager.DXTranslate('roGeniusAnd') + ' ' + selectedGroups.length + ' ' + viewUtilsManager.DXTranslate('roGeniusGroups'));
        }
        else if (selectedEmployees.length > 0) {
            $("#ReportUsers").dxTextBox("instance").option("value", selectedEmployees.length + ' ' + viewUtilsManager.DXTranslate('roGeniusEmployees'));
        }
        else if (selectedGroups.length > 0) {
            $("#ReportUsers").dxTextBox("instance").option("value", selectedGroups.length + ' ' + viewUtilsManager.DXTranslate('roGeniusGroups'));
        }
        else {
            $("#ReportUsers").dxTextBox("instance").option("value", "");
        }
    }
    else
        $("#ReportUsers").dxTextBox("instance").option("value", null);
}
function onCauesPopUpHidden() {
    if (updating) {
        var causesSelected = causesSelected2Str();

        currentGeniusEditParameters.Causes = causesSelected;
        currentGeniusView.Causes = causesSelected;
        currentGeniusEditParameters.UpdatedCauses = false;

        if (causesSelected == '') {
            $("#txtCauses").dxTextBox("instance").option("value", $("#hdnAllCausesText").val());
        } else {
            var causesLength = causesSelected.split(",").length;
            if (causesLength == 1) $("#txtCauses").dxTextBox("instance").option("value", causesLength + " " + $("#hdnCauseSelectedText").val());
            else $("#txtCauses").dxTextBox("instance").option("value", causesLength + " " + $("#hdnSomeCausesSelectedText").val());
        }
    }
}
function initializeReportPopUpFields() {
    initializeEmployeeTextFields();
    $("#txtUpdatedName").dxTextBox("instance").option("value", currentGeniusView.Name);
    $("#selectLanguage").dxSelectBox("instance").option("value", currentGeniusView.CustomFields.LanguageKey);
    $("#ckIncludeConceptsWithZeros").dxCheckBox("instance").option("value", currentGeniusView.CustomFields.IncludeZeros);
    $("#ckIncludeCausesWithZeros").dxCheckBox("instance").option("value", currentGeniusView.CustomFields.IncludeZeroCauses);
    $("#ckIncludeBusinessCenterWithZeros").dxCheckBox("instance").option("value", currentGeniusView.CustomFields.IncludeZeroBusinessCenter);
    $("#ckUpdatedSendEmail").dxCheckBox("instance").option("value", currentGeniusView.CustomFields.SendEmail);
    if (BIIntegration == "True") {
        $("#ckUpdatedDownloadBI").dxCheckBox("instance").option("value", currentGeniusView.CustomFields.DownloadBI);
        if (currentGeniusView.CustomFields.DownloadBI) {
            $("#ckUpdatedOverwriteResults").dxCheckBox("instance").option("value", currentGeniusView.CustomFields.OverwriteResults);
            $("#ckUpdatedOverwriteResults").show();
        }
        else
            $("#ckUpdatedOverwriteResults").hide();
    }
    if (currentGeniusView.Causes == '') {
        $("#txtCauses").dxTextBox("instance").option("value", $("#hdnAllCausesText").val());
    } else {
        var causesLength = currentGeniusView.Causes.split(",").length;
        if (causesLength == 1) $("#txtCauses").dxTextBox("instance").option("value", causesLength + " " + $("#hdnCauseSelectedText").val());
        else $("#txtCauses").dxTextBox("instance").option("value", causesLength + " " + $("#hdnSomeCausesSelectedText").val());
    }

    if (currentGeniusView.DSFunction.includes("@causesFilter"))
        $(".causesInfo").show();
    else
        $(".causesInfo").hide();

    if (currentGeniusView.DSFunction.includes("Genius_Concepts(") || currentGeniusView.DSFunction.includes("Genius_ConceptsAndPunches"))
        $(".zeroAvailable").show();
    else
        $(".zeroAvailable").hide();

    if (currentGeniusView.DSFunction.includes("Genius_CostCenters_Detail("))
        $(".zeroCausesAvailable").show();
    else
        $(".zeroCausesAvailable").hide();

    initializeUserFieldsTagBox();
    initializeCostCentersTagBox();
    initializeConceptssTagBox();
    initializeRequestsTagBox();
    initializeCheckedChekboxes();
}

function showCausesPopUp() {
    $("#CauesPopUp").dxPopup("instance").show();
}

function showCostsCenterPopUp() {
    $("#CostsCenterPopUp").dxPopup("instance").show();
}

function showBalancesPopUp() {
    $("#ConceptsPopUp").dxPopup("instance").show();
}

function showRequestsPopUp() {
    $("#ReuqestsPopUp").dxPopup("instance").show();
}

//function selectorSimpleGroupsShown() {
//    var selectorDataSource = $("#EmployeeText").dxTagBox("instance").option("dataSource");
//    if (selectorDataSource.store._array.length > 200) {
//        $("#EmployeeText").dxTagBox("instance").option("minSearchLength", 3);
//        $("#EmployeeText").dxTagBox("instance").option("openOnFieldClick", false);
//    }
//    else {
//        $("#EmployeeText").dxTagBox("instance").option("minSearchLength", 0);
//        $("#EmployeeText").dxTagBox("instance").option("openOnFieldClick", true);
//    }
//    var selectedEmployees = [];
//    var selectedGroups = [];
//    if (currentGeniusView.Employees != '' && (existingView || ($("#ReportUsersNew").dxTextBox("instance").option("value") != null && $("#ReportUsersNew").dxTextBox("instance").option("value") != ''))) {
//        var employeeGroups = currentGeniusView.Employees.split('#')[0].split(',');
//        var i = 0;
//        for (i = 0; i < employeeGroups.length; i++)
//            if (employeeGroups[i].charAt(0) == 'A')
//                selectedGroups.append(parseInt(employeeGroups[i].substring(1, employeeGroups[i].length)));
//            else
//                selectedEmployees.append(parseInt(employeeGroups[i].substring(1, employeeGroups[i].length)));

//        $("#EmployeeText").dxTagBox("instance").option("value", selectedEmployees);
//        $("#GroupText").dxTagBox("instance").option("value", selectedGroups);

//    }
//    else {
//        $("#EmployeeText").dxTagBox("instance").option("value", null);
//        $("#GroupText").dxTagBox("instance").option("value", null);
//    }
//    if (currentGeniusView.Status == 0 || typeof currentGeniusView.Status === 'undefined') {
//        $("#GroupText").dxTagBox("instance").option("disabled", false);
//        $("#EmployeeText").dxTagBox("instance").option("disabled", false);
//    }
//    else {
//        $("#GroupText").dxTagBox("instance").option("disabled", true);
//        $("#EmployeeText").dxTagBox("instance").option("disabled", true);
//    }
//}

function userFieldsSelected(e) {
    if (e.addedItems.length > 0 || e.removedItems.length > 0) {
        $('#hdnUserFieldsSelected').val(e.addedItems);

        if (currentGeniusView != null && currentGeniusView.UserFields != null && currentGeniusView.UserFields.length > 0) {
            var selectedUserFields = currentGeniusView.UserFields.split(",");
            e.component.option('items').map(item => {
                item.disabled = (selectedUserFields.includes(item.Name))
            })
        }
        else {
            e.component.option('items').map(item => {
                item.disabled = false
            })
        }
        e.component.repaint();
    }
}

function userFieldValueChanged(e) {
    if (e.value != null && e.value.length > 10) {
        const allPrevValues = e.previousValue;
        e.component.option("value", allPrevValues);
        DevExpress.ui.notify(viewUtilsManager.DXTranslate('roMaxUserFieldsSelected'), 'error', 2000);
    }
}

function userFieldContentReady(e) {
    if (!userFieldsLoaded && e.component.option('items') != null && e.component.option('items').length > 0) {
        var selectedUserFields = currentGeniusView.UserFields.split(",");
        e.component.option('items').map(item => {
            item.disabled = (selectedUserFields.includes(item.Name))
        })
        userFieldsLoaded = true;
        //   e.component.repaint();
    }
}

function userFieldsInitialized(e) {
    e.component.registerKeyHandler("backspace", function () {
        return true;
    })
}

function userFieldsKeyDown(e) {
    if (e.evet.keycode == 8) {
        e.event.stopPropagation();
        e.event.preventDefault();
    }
}

function userFieldListOpened(e) {
    if (!userFieldsOpened) {
        userFieldsOpened = true;
        e.component.repaint();
    }
}

function costCentersSelected(e) {
    if (e.addedItems.length > 0) {
        $('#hdnCostCentersSelected').val(e.addedItems);
    }
}

function conceptsSelected(e) {
    if (e.addedItems.length > 0) {
        $('#hdnConceptsSelected').val(e.addedItems);
    }
}

function requestsSelected(e) {
    if (e.addedItems.length > 0) {
        $('#hdnRequestsSelected').val(e.addedItems);
    }
}

function onChkAnalyticTypeChange(e) {
    disableCheckBoxes(e);
    $(".conceptsInfoNew").hide();
    $(".causesInfoNew").hide();

    switch (e.element[0].id) {
        case "chkAnalyticType1":
            if (e.value == true) {
                $("#btnCausesInfo").dxButton("instance").option("disabled", false);
                if (typeof lstCauses != 'undefined') {
                    $("#ckIncludeAllCauses").dxRadioGroup("instance").option("value", "0");
                    lstCauses.UnselectAll();
                    lstCauses.SetEnabled(false);
                }
            } else {
                $("#btnCausesInfo").dxButton("instance").option("disabled", true);
            }
            break;
        case "chkAnalyticType4":
            if (e.value == true) {
                $("#btnCostCenterInfo").dxButton("instance").option("disabled", false);
            }
            else
                $("#btnCostCenterInfo").dxButton("instance").option("disabled", true);
            break;
        case "chkAnalyticType6":
            if (e.value == true) {
                $("#btnBalancesInfo").dxButton("instance").option("disabled", false);
            }
            else {
                $("#btnBalancesInfo").dxButton("instance").option("disabled", true);
            }
            break;
        case "chkAnalyticType5":
            if (e.value == true) {
                $("#btnRequestsInfo").dxButton("instance").option("disabled", false);
            }
            else
                $("#btnRequestsInfo").dxButton("instance").option("disabled", true);
            break;
    }
    var checkedCheckboxes = getCheckedCheckboxes();
    if ((typeof checkedCheckboxes != undefined) && ((checkedCheckboxes.length == 1 && checkedCheckboxes[0] == 6) || (checkedCheckboxes.length == 2 && checkedCheckboxes.includes('6') && checkedCheckboxes.includes('2'))))
        $(".conceptsInfoNew").show();
    else
        $(".conceptsInfoNew").hide();

    if ((typeof checkedCheckboxes != undefined) && (checkedCheckboxes.length == 2 && checkedCheckboxes.includes('1') && checkedCheckboxes.includes('4')))
        $(".causesInfoNew").show();
    else
        $(".causesInfoNew").hide();
}

function getCheckedClasses() {
    var analyticChks = $(".checkBoxAnalyticType").dxCheckBox();
    var checkedClasses = [];
    for (var i = 0; i < analyticChks.length; i++) {
        var analyticChkId = analyticChks[i].id;
        if ($("#" + analyticChkId).dxCheckBox("instance").option("value")) {
            if (checkedClasses.length == 0)
                checkedClasses = $("#" + analyticChkId).dxCheckBox("instance").element().attr('class').replace("dx-widget dx-checkbox dx-checkbox-has-text ", "").replace("checkBoxAnalyticType", "").split(" ");
            else {
                var currentCheckedClasses = $("#" + analyticChkId).dxCheckBox("instance").element().attr('class').replace("dx-widget dx-checkbox dx-checkbox-has-text ", "").replace("checkBoxAnalyticType", "").split(" ");
                checkedClasses = checkedClasses.filter(value => value != "" && currentCheckedClasses.includes(value));
            }
        }
    }
    return checkedClasses;
}

function getCheckedCheckboxes() {
    var analyticChks = $(".checkBoxAnalyticType").dxCheckBox();
    var checkedCheckBoxes = [];
    for (var i = 0; i < analyticChks.length; i++) {
        var analyticChkId = analyticChks[i].id;
        if ($("#" + analyticChkId).dxCheckBox("instance").option("value")) {
            checkedCheckBoxes.append(analyticChkId.replace("chkAnalyticType", ""));
        }
    }
    return checkedCheckBoxes;
}

function resetCheckboxes() {
    var analyticChks = $(".checkBoxAnalyticType").dxCheckBox();
    for (var i = 0; i < analyticChks.length; i++) {
        var analyticChkId = analyticChks[i].id;
        $("#" + analyticChkId).dxCheckBox("instance").option("value", false);
    }
}

function resetReadCheckboxes() {
    var analyticChks = $(".checkBoxAnalyticTypeRead").dxCheckBox();
    for (var i = 0; i < analyticChks.length; i++) {
        var analyticChkId = analyticChks[i].id;
        $("#" + analyticChkId).dxCheckBox("instance").option("value", false);
    }
}

function disableCheckBoxes() {
    var checkedClasses = getCheckedClasses();
    var analyticChks = $(".checkBoxAnalyticType").dxCheckBox();
    if (checkedClasses.length > 0) {
        for (var i = 0; i < analyticChks.length; i++) {
            var analyticChkId = analyticChks[i].id;

            var cClass = $("#" + analyticChkId).dxCheckBox("instance").element().attr('class').replace("dx-widget dx-checkbox dx-checkbox-has-text ", "").replace("checkBoxAnalyticType", "").split(" ");
            const filteredArray = checkedClasses.filter(value => value != "" && cClass.includes(value));
            if (filteredArray.length > 0)
                $("#" + analyticChkId).dxCheckBox("instance").option("disabled", false);
            else
                $("#" + analyticChkId).dxCheckBox("instance").option("disabled", true);
        }
    }
    else {
        for (var i = 0; i < analyticChks.length; i++) {
            var analyticChkId = analyticChks[i].id;
            $("#" + analyticChkId).dxCheckBox("instance").option("disabled", false);
        }
    }
}

function closeUpdatePopUp() {
    $("#reportConfigurationPopup").dxPopup("instance").hide();
}

function isCostCenterDefaultView(model) {
    return model.DS == 5;
}

function isCostCenterAndIncidencesDefaultView(model) {
    return model.DSFunction.indexOf("Genius_CostCenters_Detail") >= 0;
}

function isCostCenterCustomView(model) {
    return model.CheckedCheckBoxes != null && model.CheckedCheckBoxes.includes("4");
}

function isBalancesDefaultView(model) {
    return model.DS == 2;
}

function isBalancesCustomView(model) {
    return model.CheckedCheckBoxes != null && model.CheckedCheckBoxes.includes("6");
}

function isCausesDefaultView(model) {
    return model.DSFunction.toString().toLowerCase().indexOf("incidences") >= 0 || model.DSFunction.toString().indexOf("Genius_CostCenters_Detail") >= 0;
}

function isRequestsDefaultView(model) {
    return model.DS == 9;
}

function isRequestsCustomView(model) {
    return model.CheckedCheckBoxes != null && model.CheckedCheckBoxes.includes("5");
}

function isJustificationsByTimeDefaultView(model) {
    return model.DSFunction.indexOf("Genius_IncidencesByTime") >= 0;
}

function RefreshPlanningList() {
    $("#PlanningList").dxDataGrid("instance").refresh();
}

function GetCurrentGeniusView() {
    if (currentGeniusView != null && currentGeniusView.Id != null)
        return currentGeniusView.Id;
    else
        return null;
}

function onBeforeSendPlanning(operation, ajaxSettings) {
    if (operation == 'update' || operation == 'insert') {
        var periodInfo = calculatePeriodInfo(ajaxSettings.data.Name);
        ajaxSettings.data.schedulerText = periodInfo.scheduledSentence;
        ajaxSettings.data.scheduledParam = periodInfo.scheduledParam
        ajaxSettings.data.scheduledFrequency = periodInfo.shceduledFrequency;
        ajaxSettings.data.periodType = $("#radioGeniusSchedulerConfig").dxRadioGroup("instance").option("value");
        ajaxSettings.data.beginPeriod = date2DateTime(new Date($("#selectedFromDate").dxDateBox("instance").option("value")));
        ajaxSettings.data.endPeriod = date2DateTime(new Date($("#selectedToDate").dxDateBox("instance").option("value")));
        if (ajaxSettings.data.ID == null || ajaxSettings.data.ID == undefined)
            ajaxSettings.data.ID = -1;
    }
    if (operation == 'insert')
        ajaxSettings.data.idGeniusView = currentGeniusView.Id;
}

function OnPlanningListLoading(e) {
    if (currentGeniusView == null || currentGeniusView.Id == null) {
        e.cancel = true;
    }
}

function onConfigChanged(e) {
    const today = new Date();
    switch (e.value) {
        // Mañana
        case 1:
            const tomorrow = new Date(today);
            tomorrow.setDate(tomorrow.getDate() + 1);
            var from = new Date(tomorrow);
            from.setHours(0);
            from.setMinutes(0);
            var to = new Date(tomorrow);
            to.setHours(23);
            to.setMinutes(59);
            $("#selectedFromDate").dxDateBox("instance").option("value", from);
            $("#selectedToDate").dxDateBox("instance").option("value", to);
            break;
        // Hoy
        case 2:
            var from = new Date(today);
            from.setHours(0);
            from.setMinutes(0);
            var to = new Date(today);
            to.setHours(23);
            to.setMinutes(59);
            $("#selectedFromDate").dxDateBox("instance").option("value", from);
            $("#selectedToDate").dxDateBox("instance").option("value", to);
            break;
        // Ayer
        case 3:
            const yesterday = new Date(today);
            yesterday.setDate(yesterday.getDate() - 1);
            var from = new Date(yesterday);
            from.setHours(0);
            from.setMinutes(0);
            var to = new Date(yesterday);
            to.setHours(23);
            to.setMinutes(59);
            $("#selectedFromDate").dxDateBox("instance").option("value", from);
            $("#selectedToDate").dxDateBox("instance").option("value", to);
            break;
        // Semana actual
        case 4:
            var from = new Date(today.setDate(today.getDate() - today.getDay() + 1));
            var to = new Date(today.setDate(today.getDate() - today.getDay() + 7));
            from.setHours(0);
            from.setMinutes(0);
            to.setHours(23);
            to.setMinutes(59);
            $("#selectedFromDate").dxDateBox("instance").option("value", from);
            $("#selectedToDate").dxDateBox("instance").option("value", to);
            break;
        // Semana pasada
        case 5:
            var from = new Date(today.setDate(today.getDate() - today.getDay() + 1));
            var to = new Date(today.setDate(today.getDate() - today.getDay() + 7));
            from.setDate(from.getDate() - 7);
            from.setHours(0);
            from.setMinutes(0);
            to.setDate(to.getDate() - 7);
            to.setHours(23);
            to.setMinutes(59);
            $("#selectedFromDate").dxDateBox("instance").option("value", from);
            $("#selectedToDate").dxDateBox("instance").option("value", to);
            break;
        // Mes actual
        case 6:
            var from = new Date(today.getFullYear(), today.getMonth(), 1);
            var to = new Date(today.getFullYear(), today.getMonth() + 1, 0);
            from.setHours(0);
            from.setMinutes(0);
            to.setHours(23);
            to.setMinutes(59);
            $("#selectedFromDate").dxDateBox("instance").option("value", from);
            $("#selectedToDate").dxDateBox("instance").option("value", to);
            break;
        // Mes anterior
        case 7:
            var from = new Date(today.getFullYear(), today.getMonth() - 1, 1);
            var to = new Date(today.getFullYear(), today.getMonth(), 0);
            from.setHours(0);
            from.setMinutes(0);
            to.setHours(23);
            to.setMinutes(59);
            $("#selectedFromDate").dxDateBox("instance").option("value", from);
            $("#selectedToDate").dxDateBox("instance").option("value", to);
            break;
        // Año actual
        case 8:
            var from = new Date(today.getFullYear(), 0, 1);
            var to = new Date(today);
            from.setHours(0);
            from.setMinutes(0);
            to.setHours(23);
            to.setMinutes(59);
            $("#selectedFromDate").dxDateBox("instance").option("value", from);
            $("#selectedToDate").dxDateBox("instance").option("value", to);
            break;
        // Semana siguiente
        case 9:
            var from = new Date(today.setDate(today.getDate() - today.getDay() + 1));
            var to = new Date(today.setDate(today.getDate() - today.getDay() + 7));
            from.setDate(from.getDate() + 7);
            from.setHours(0);
            from.setMinutes(0);
            to.setDate(to.getDate() + 7);
            to.setHours(23);
            to.setMinutes(59);
            $("#selectedFromDate").dxDateBox("instance").option("value", from);
            $("#selectedToDate").dxDateBox("instance").option("value", to);
            break;
        // Mes siguiente
        case 10:
            var from = new Date(today.getFullYear(), today.getMonth() + 1, 1);
            var to = new Date(today.getFullYear(), today.getMonth() + 2, 0);
            from.setHours(0);
            from.setMinutes(0);
            to.setHours(23);
            to.setMinutes(59);
            $("#selectedFromDate").dxDateBox("instance").option("value", from);
            $("#selectedToDate").dxDateBox("instance").option("value", to);
            break;
    }
}

function markRowAsUpdated(e) {
    if (!onInitializing) {
        var dg = $("#PlanningList").dxDataGrid("instance");
        var rows = dg.getVisibleRows();
        for (var i = 0; i < rows.length; i++) {
            if (rows[i].isEditing) {
                dg.cellValue(i, "LastDateTimeUpdated", new Date());
                dg.cellValue(i, "Name", dg.cellValue(i, "Name"));
            }
        }
    }
}

function onSchedulerChanged(e) {
    markRowAsUpdated(e);
    switch (e.value) {
        case 0: $("#dailySchedule").show();
            $("#weeklySchedule").hide();
            $("#monthlySchedule").hide();
            $("#onceSchedule").hide();
            $("#intervalSchedule").hide();
            break;
        case 1: $("#dailySchedule").hide();
            $("#weeklySchedule").show();
            $("#monthlySchedule").hide();
            $("#onceSchedule").hide();
            $("#intervalSchedule").hide();
            break;
        case 2: $("#dailySchedule").hide();
            $("#weeklySchedule").hide();
            $("#monthlySchedule").show();
            $("#onceSchedule").hide();
            $("#intervalSchedule").hide();
            break;
        case 3: $("#dailySchedule").hide();
            $("#weeklySchedule").hide();
            $("#monthlySchedule").hide();
            $("#onceSchedule").show();
            $("#intervalSchedule").hide();
            break;
        case 4: $("#dailySchedule").hide();
            $("#weeklySchedule").hide();
            $("#monthlySchedule").hide();
            $("#onceSchedule").hide();
            $("#intervalSchedule").show();
            break;
    }
}

function onPlanningUpdating(e) {
    if (e == null || e.newData == null || e.newData.length == 0 || (e.newData.Name != null && e.newData.Name == "")) {
        DevExpress.ui.notify(viewUtilsManager.DXTranslate('roSchedulerSaveNameError'), 'error', 2000);
        e.cancel = true;
    }
}

function onPlanningInserting(e) {
    if (e == null || e.data == null || e.data.length == 0 || e.data.Name == null || e.data.Name == "") {
        DevExpress.ui.notify(viewUtilsManager.DXTranslate('roSchedulerSaveNameError'), 'error', 2000);
        e.cancel = true;
    }
}

function calculatePeriodInfo(scheduleName) {
    let scheduleStr = "";
    let sentence = "";
    switch ($("#radioGeniusSchedulerPlan").dxRadioGroup("instance").option("value")) {
        case 0:
            scheduleStr = $("#radioGeniusSchedulerPlan").dxRadioGroup("instance").option("value") + "@" + $("#timeOfDay").dxDateBox("instance").option("text") + "@";
            scheduleStr += $("#txtDays").dxNumberBox("instance").option("value");
            sentence = "Cada " + $("#txtDays").dxNumberBox("instance").option("value") + " días, a las " + $("#timeOfDay").dxDateBox("instance").option("text").padStart(5, '0');
            break;
        case 1:
            scheduleStr = $("#radioGeniusSchedulerPlan").dxRadioGroup("instance").option("value") + "@" + $("#timeOfDayWeekly").dxDateBox("instance").option("text") + "@";
            scheduleStr += $("#txtWeeks").dxNumberBox("instance").option("value") + "@" + selectedWeekDaysToStr();
            sentence = "Cada " + $("#txtWeeks").dxNumberBox("instance").option("value") + " semanas, los " + selectedWeekDaysToSentence() + ", a las " + $("#timeOfDayWeekly").dxDateBox("instance").option("text").padStart(5, '0');
            break;
        case 2:
            scheduleStr = $("#radioGeniusSchedulerPlan").dxRadioGroup("instance").option("value") + "@" + $("#timeOfDayMonthly").dxDateBox("instance").option("text") + "@";
            scheduleStr += monthlyType + "@";
            if (monthlyType === 0)
                scheduleStr += $("#txtDayOfMonth").dxNumberBox("instance").option("value") +
                    "@" +
                    $("#lstDaysOfWeekMonthly").dxSelectBox("instance").option("value");
            else
                scheduleStr += $("#lstWeeksOfMonth").dxSelectBox("instance").option("value") +
                    "@" +
                    $("#lstDaysOfWeekMonthly").dxSelectBox("instance").option("value");

            sentence =
                monthlyType === 0
                    ? "El día " + $("#txtDayOfMonth").dxNumberBox("instance").option("text") +
                    " de cada mes, a las " + $("#timeOfDayMonthly").dxDateBox("instance").option("text").padStart(5, '0')
                    : "El " + $("#lstWeeksOfMonth").dxSelectBox("instance").option("text") + " " + $("#lstDaysOfWeekMonthly").dxSelectBox("instance").option("text") +
                    " de cada mes, a las " + $("#timeOfDayMonthly").dxDateBox("instance").option("text").padStart(5, '0');
            break;
        case 3:
            var date = new Date($("#selectedDateTime").dxDateBox("instance").option("value"));
            scheduleStr = $("#radioGeniusSchedulerPlan").dxRadioGroup("instance").option("value") + "@" + date2Time(date) + "@";
            scheduleStr += date2DateTime(date);
            sentence = "El " + date2Date(date) + " a las " + date2ShortTime(date);
            break;
        case 4:
            var date = new Date($("#timeOfInterval").dxDateBox("instance").option("value"));
            scheduleStr = $("#radioGeniusSchedulerPlan").dxRadioGroup("instance").option("value") + "@" + date2ShortTime(date) + "@";
            sentence = "Cada " + date2ShortTime(date) + " horas";
            break;
    }
    var scheduleInfo = { "scheduledSentence": sentence, "description": scheduleName, "scheduledParam": scheduleStr }
    return scheduleInfo;
}

function date2DateTime(d) {
    var datestring = d.getFullYear() + "/" + ("0" + (d.getMonth() + 1)).slice(-2) + "/" + ("0" + d.getDate()).slice(-2)
        + " " + ("0" + d.getHours()).slice(-2) + ":" + ("0" + d.getMinutes()).slice(-2);
    return datestring;
}

function dateTime2Str(d) {
    var datestring = ("0" + d.getDate()).slice(-2) + "-" + ("0" + (d.getMonth() + 1)).slice(-2) + "-" + d.getFullYear() +
        " " + d.getHours() + ("0" + d.getHours()).slice(-2);// + ":" + ("0" + d.getMinutes()).slice(-2);
    return datestring;
}

function date2Date(d) {
    var datestring = ("0" + d.getDate()).slice(-2) + "-" + ("0" + (d.getMonth() + 1)).slice(-2) + "-" +
        d.getFullYear();
    return datestring;
}

function date2Time(d) {
    var datestring = ("0" + d.getHours()).slice(-2) + ":" + ("0" + d.getMinutes()).slice(-2) + ":" + ("0" + d.getSeconds()).slice(-2);
    return datestring;
}

function date2ShortTime(d) {
    var datestring = ("0" + d.getHours()).slice(-2) + ":" + ("0" + d.getMinutes()).slice(-2);
    return datestring;
}

String.prototype.replaceAt = function (index, replacement) {
    return this.substr(0, index) + replacement + this.substr(index + replacement.length);
}

function onPlanningInitNewRow(e) {
    e.data.Name = viewUtilsManager.DXTranslate('roGeniusNewPlanification');
    clearPlanningConfig();
    clearPlanningScheduler();
}

function clearPlanningScheduler() {
    $("#radioGeniusSchedulerPlan").dxRadioGroup("instance").option("value", 3);
    $("#txtDays").dxNumberBox("instance").option("value", 1);
    $("#timeOfDay").dxDateBox("instance").option("value", null);
    $("#txtWeeks").dxNumberBox("instance").option("value", 1);
    $("#lstDaysOfWeek").dxTagBox("instance").option("value", null);
    $("#timeOfDayWeekly").dxDateBox("instance").option("value", null);
    $("#txtDayOfMonth").dxNumberBox("instance").option("value", null);
    $("#lstWeeksOfMonth").dxSelectBox("instance").option("value", 0);
    $("#lstDaysOfWeekMonthly").dxSelectBox("instance").option("value", null);
    $("#timeOfDayMonthly").dxDateBox("instance").option("value", null);
    const today = new Date();
    const tomorrow = new Date(today);
    tomorrow.setDate(tomorrow.getDate() + 1);
    $("#selectedDateTime").dxDateBox("instance").option("value", tomorrow);
    $("#timeOfInterval").dxDateBox("instance").option("value", 0);
}

function clearPlanningConfig() {
    $("#radioGeniusSchedulerConfig").dxRadioGroup("instance").option("value", 2);
    var from = new Date();
    from.setHours(0);
    from.setMinutes(0);
    var to = new Date();
    to.setHours(23);
    to.setMinutes(59);
    $("#selectedFromDate").dxDateBox("instance").option("value", from);
    $("#selectedToDate").dxDateBox("instance").option("value", to);
}

function onPlanningEditingStart(e) {
    $("#geniusPlannerScheduler").show();
    $("#geniusConfigScheduler").show();
    clearPlanningConfig();
    clearPlanningScheduler();
    initializePlanningConfig(e);
    initializePlanningScheduler(e);
}

function getWeekDays(str) {
    var weekDays = new Array();
    for (var i = 0; i < 7; i++)
        if (str[i] == 1)
            weekDays.push(i);
    return weekDays;
}

function initializePlanningConfig(e) {
    onInitializing = true;
    $("#radioGeniusSchedulerConfig").dxRadioGroup("instance").option("value", e.data.PeriodType);
    if (e.data.ScheduleBeginDate != null)
        $("#selectedFromDate").dxDateBox("instance").option("value", e.data.ScheduleBeginDate);
    if (e.data.ScheduleEndDate != null)
        $("#selectedToDate").dxDateBox("instance").option("value", e.data.ScheduleEndDate);
    onInitializing = false;
}

function initializePlanningScheduler(e) {
    onInitializing = true;
    $("#radioGeniusSchedulerPlan").dxRadioGroup("instance").option("value", e.data.Scheduler.ScheduleType);
    var date = new Date(e.data.Scheduler.DateSchedule.replace("00:00:00", e.data.Scheduler.Hour.padStart(5, '0')));
    switch (e.data.Scheduler.ScheduleType) {
        case 0:
            $("#txtDays").dxNumberBox("instance").option("value", e.data.Scheduler.Days);
            $("#timeOfDay").dxDateBox("instance").option("value", date2DateTime(date));
            break;
        case 1:
            $("#txtWeeks").dxNumberBox("instance").option("value", e.data.Scheduler.Weeks);
            $("#lstDaysOfWeek").dxTagBox("instance").option("value", getWeekDays(e.data.Scheduler.WeekDays));
            $("#timeOfDayWeekly").dxDateBox("instance").option("value", date2DateTime(date));
            break;
        case 2:
            if (e.data.Scheduler.MonthlyType == 0) {
                $("#txtDayOfMonth").dxNumberBox("instance").option("value", e.data.Scheduler.Day);
                $('#monthlyOption2').css('color', '#D3D3D3');
                $('#monthlyOption1').css('color', '#212529');
                $('#monthlyOption2 input').css('color', '#D3D3D3');
                $('#monthlyOption1 input').css('color', '#212529');
                monthlyType = 0;
            }
            else {
                $("#lstWeeksOfMonth").dxSelectBox("instance").option("value", e.data.Scheduler.Start);
                $("#lstDaysOfWeekMonthly").dxSelectBox("instance").option("value", e.data.Scheduler.WeekDay);
                $('#monthlyOption1').css('color', '#D3D3D3');
                $('#monthlyOption2').css('color', '#212529');
                $('#monthlyOption1 input').css('color', '#D3D3D3');
                $('#monthlyOption2 input').css('color', '#212529');

                monthlyType = 1;
            }
            $("#timeOfDayMonthly").dxDateBox("instance").option("value", date2DateTime(date));
            break;
        case 3:
            $("#selectedDateTime").dxDateBox("instance").option("value", date);
            break;
        case 4:
            $("#timeOfInterval").dxDateBox("instance").option("value", date2DateTime(date));
            break;
    }
    onInitializing = false;
}

function validateSchedulerData(e) {
    switch ($("#radioGeniusSchedulerPlan").dxRadioGroup("instance").option("value")) {
        case 0:
            if ($("#timeOfDay").dxDateBox("instance").option("value") === null || $("#timeOfDay").dxDateBox("instance").option("value") === undefined || $("#timeOfDay").dxDateBox("instance").option("value") === ""
                || $("#txtDays").dxNumberBox("instance").option("value") === null || $("#txtDays").dxNumberBox("instance").option("value") === undefined || $("#txtDays").dxNumberBox("instance").option("value") === ""
                || $("#selectedFromDate").dxDateBox("instance").option("value") === null || $("#selectedFromDate").dxDateBox("instance").option("value") === undefined || $("#selectedFromDate").dxDateBox("instance").option("value") === ""
                || $("#selectedToDate").dxDateBox("instance").option("value") === null || $("#selectedToDate").dxDateBox("instance").option("value") === undefined || $("#selectedToDate").dxDateBox("instance").option("value") === "") {
                DevExpress.ui.notify(viewUtilsManager.DXTranslate('roValidationError'), 'error', 2000);
                e.isValid = false;
            }
            else
                e.isValid = true;
            break;
        case 1:
            if ($("#timeOfDayWeekly").dxDateBox("instance").option("value") === null || $("#timeOfDayWeekly").dxDateBox("instance").option("value") === undefined || $("#timeOfDayWeekly").dxDateBox("instance").option("value") === ""
                || $("#txtWeeks").dxNumberBox("instance").option("value") === null || $("#txtWeeks").dxNumberBox("instance").option("value") === undefined || $("#txtWeeks").dxNumberBox("instance").option("value") === ""
                || $("#lstDaysOfWeek").dxTagBox("instance").option("value") === null || $("#lstDaysOfWeek").dxTagBox("instance").option("value") === undefined || $("#lstDaysOfWeek").dxTagBox("instance").option("value") === ""
                || $("#selectedFromDate").dxDateBox("instance").option("value") === null || $("#selectedFromDate").dxDateBox("instance").option("value") === undefined || $("#selectedFromDate").dxDateBox("instance").option("value") === ""
                || $("#selectedToDate").dxDateBox("instance").option("value") === null || $("#selectedToDate").dxDateBox("instance").option("value") === undefined || $("#selectedToDate").dxDateBox("instance").option("value") === "") {
                DevExpress.ui.notify(viewUtilsManager.DXTranslate('roValidationError'), 'error', 2000);
                e.isValid = false;
            }
            else
                e.isValid = true;
            break;
        case 2:
            if (monthlyType === 0) {
                if ($("#txtDayOfMonth").dxNumberBox("instance").option("value") === null || $("#txtDayOfMonth").dxNumberBox("instance").option("value") === undefined || $("#txtDayOfMonth").dxNumberBox("instance").option("value") === ""
                    || $("#timeOfDayMonthly").dxDateBox("instance").option("value") === null || $("#timeOfDayMonthly").dxDateBox("instance").option("value") === undefined || $("#timeOfDayMonthly").dxDateBox("instance").option("value") === ""
                    || $("#selectedFromDate").dxDateBox("instance").option("value") === null || $("#selectedFromDate").dxDateBox("instance").option("value") === undefined || $("#selectedFromDate").dxDateBox("instance").option("value") === ""
                    || $("#selectedToDate").dxDateBox("instance").option("value") === null || $("#selectedToDate").dxDateBox("instance").option("value") === undefined || $("#selectedToDate").dxDateBox("instance").option("value") === "") {
                    DevExpress.ui.notify(viewUtilsManager.DXTranslate('roValidationError'), 'error', 2000);
                    e.isValid = false;
                }
                else
                    e.isValid = true;
            }
            else {
                if ($("#lstWeeksOfMonth").dxSelectBox("instance").option("value") === null || $("#lstWeeksOfMonth").dxSelectBox("instance").option("value") === undefined || $("#lstWeeksOfMonth").dxSelectBox("instance").option("value") === ""
                    || $("#lstWeeksOfMonth").dxSelectBox("instance").option("value") === null || $("#lstWeeksOfMonth").dxSelectBox("instance").option("value") === undefined || $("#lstWeeksOfMonth").dxSelectBox("instance").option("value") === ""
                    || $("#lstDaysOfWeekMonthly").dxSelectBox("instance").option("value") === null || $("#lstDaysOfWeekMonthly").dxSelectBox("instance").option("value") === undefined || $("#lstDaysOfWeekMonthly").dxSelectBox("instance").option("value") === ""
                    || $("#timeOfDayMonthly").dxDateBox("instance").option("value") === null || $("#timeOfDayMonthly").dxDateBox("instance").option("value") === undefined || $("#timeOfDayMonthly").dxDateBox("instance").option("value") === ""
                    || $("#selectedFromDate").dxDateBox("instance").option("value") === null || $("#selectedFromDate").dxDateBox("instance").option("value") === undefined || $("#selectedFromDate").dxDateBox("instance").option("value") === ""
                    || $("#selectedToDate").dxDateBox("instance").option("value") === null || $("#selectedToDate").dxDateBox("instance").option("value") === undefined || $("#selectedToDate").dxDateBox("instance").option("value") === "") {
                    DevExpress.ui.notify(viewUtilsManager.DXTranslate('roValidationError'), 'error', 2000);
                    e.isValid = false;
                }
                else
                    e.isValid = true;
            }
            break;
        case 3:
            if ($("#selectedDateTime").dxDateBox("instance").option("value") === null || $("#selectedDateTime").dxDateBox("instance").option("value") === undefined || $("#selectedDateTime").dxDateBox("instance").option("value") === ""
                || $("#selectedFromDate").dxDateBox("instance").option("value") === null || $("#selectedFromDate").dxDateBox("instance").option("value") === undefined || $("#selectedFromDate").dxDateBox("instance").option("value") === ""
                || $("#selectedToDate").dxDateBox("instance").option("value") === null || $("#selectedToDate").dxDateBox("instance").option("value") === undefined || $("#selectedToDate").dxDateBox("instance").option("value") === "") {
                DevExpress.ui.notify(viewUtilsManager.DXTranslate('roValidationError'), 'error', 2000);
                e.isValid = false;
            }
            else
                e.isValid = true;
            break;
        case 4:
            if ($("#timeOfInterval").dxDateBox("instance").option("value") === null || $("#timeOfInterval").dxDateBox("instance").option("value") === undefined || $("#timeOfInterval").dxDateBox("instance").option("value") === ""
                || $("#selectedFromDate").dxDateBox("instance").option("value") === null || $("#selectedFromDate").dxDateBox("instance").option("value") === undefined || $("#selectedFromDate").dxDateBox("instance").option("value") === ""
                || $("#selectedToDate").dxDateBox("instance").option("value") === null || $("#selectedToDate").dxDateBox("instance").option("value") === undefined || $("#selectedToDate").dxDateBox("instance").option("value") === "") {
                DevExpress.ui.notify(viewUtilsManager.DXTranslate('roValidationError'), 'error', 2000);
                e.isValid = false;
            }
            else
                e.isValid = true;
            break;
    }
}

function selectedWeekDaysToStr() {
    var selectedDaysStr = "0000000";
    $("#lstDaysOfWeek").dxTagBox("instance").option("selectedItems").forEach(element => selectedDaysStr = selectedDaysStr.replaceAt(element.Id, "1"));
    return selectedDaysStr;
}

function selectedWeekDaysToSentence() {
    var selectedDaysStr = "";
    $("#lstDaysOfWeek").dxTagBox("instance").option("selectedItems").forEach(element => { if (selectedDaysStr == "") selectedDaysStr = element.Text; else selectedDaysStr += ", " + element.Text });
    return selectedDaysStr;
}

function formatDate(rowDate) {
    if (rowDate != null && rowDate.NextDateTimeExecuted != null)
        return dateTime2Str(new Date(rowDate.NextDateTimeExecuted));
}

function formatNextDate(rowDate) {
    let newDate = "";
    if (rowDate != null && rowDate.NextDateTimeExecuted != null) {
        newDate = new Date(rowDate.NextDateTimeExecuted).toLocaleString();
    }
    return newDate;
}

function ckDownloadBI_valueChanged(data) {
    if (data.value)
        $("#ckOverwriteResults").show();
    else {
        $("#ckOverwriteResults").hide();
        $("#ckOverwriteResults").dxCheckBox("instance").option("value", null);
    }
}

function initializePopUpComponents() {
    $("#dtDateInf").dxDateBox();
    $("#dtDateSup").dxDateBox();
    $("#rgDateFilterType").dxRadioGroup();
}

function ckIncludeCausesWithZero_Changed(s, e) {
    if (s.value == true) {
        $("#ckIncludeBusinessCenterWithZerosNew").dxCheckBox("instance").option("value", false);
    } else {
        $("#ckIncludeBusinessCenterWithZerosNew").dxCheckBox("instance").option("value", true);
    }
}   