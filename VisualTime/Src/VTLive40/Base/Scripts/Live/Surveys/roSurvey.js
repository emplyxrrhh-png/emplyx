var surveyCreator;

var yourNewSurveyJSON;
var currentView = null;
var resultsSelected = null;
var currentSurveyContent = null;
var surveyTemplates = null;

async function getSurveyTemplates() {
    var result = [];

    await $.ajax({
        type: "POST",
        url: BASE_URL + 'Surveys/GetSurveyTemplates',
        dataType: "json",
        success: function (e) {
            var resultadoAJAX = e.map(obj => {
                var payload = {};
                payload.name = obj.name;
                payload.json = JSON.parse(obj.json);
                return payload;
            });

            result = resultadoAJAX;
        },
        error: function (e) { }
    });

    surveyTemplates = result;
}

$(document).ready(function () {
    getSurveyTemplates();
});

function toolbar_preparing(e) {
}

function getWidth() {
    return document.documentElement.clientWidth - 40;
}

function getHeight() {
    return document.documentElement.clientHeight - 40;
}

function getHeightResults() {
    return document.documentElement.clientHeight - 150;
}

function addNewSurvey() {
    $('#surveySelectedStatus').val("0");
    $('#surveyMode').val("0");
    var popup = $("#newSurveyPopup").dxPopup("instance");
    popup.show();
}

function sendSurvey() {
    $.ajax({
        type: "POST",
        url: BASE_URL + 'Surveys/SendSurvey',
        dataType: "json",
        data: { Id: currentView.Id },
        success: function (e) {
            if (e == true) {
                DevExpress.ui.notify(viewUtilsManager.DXTranslate('roSurveySent'), "success", 2000);
                var popup = $("#newSurveyPopup").dxPopup("instance");
                popup.hide();

                var surveySurvey = $("#gridStatusSurveys").dxDataGrid("instance");
                surveySurvey.refresh();
            }
            else {
                DevExpress.ui.notify(viewUtilsManager.DXTranslate('roSurveySendError'), "error", 2000);
            }
        },
        error: function (e) { DevExpress.ui.notify(viewUtilsManager.DXTranslate('roSurveySendError'), "error", 2000); }
    });
}

function copySurvey(e) {
    $.ajax({
        type: "POST",
        url: BASE_URL + 'Surveys/CopySurvey',
        dataType: "json",
        data: { Id: e.row.data.Id },
        success: function (e) {
            if (e == true) {
                DevExpress.ui.notify(viewUtilsManager.DXTranslate('roSurveyCloned'), "success", 2000);
                var popup = $("#newSurveyPopup").dxPopup("instance");
                popup.hide();

                var surveySurvey = $("#gridStatusSurveys").dxDataGrid("instance");
                surveySurvey.refresh();
            }
            else {
                DevExpress.ui.notify(viewUtilsManager.DXTranslate('roSurveyCloneError'), "error", 2000);
            }
        },
        error: function (e) { DevExpress.ui.notify(viewUtilsManager.DXTranslate('roSurveyCloneError'), "error", 2000); }
    });
}
function createNewSurvey() {
    disableSaveButton();
    var title = $("#SurveyName").dxTextBox("instance").option("value");
    var mandatory = $("#SurveyMandatory").dxSwitch("instance").option("value");
    var anonymous = $("#SurveyAnonymous").dxSwitch("instance").option("value");
    var percentageExpiration = $("#SurveyExpiredByPercentage").dxCheckBox("instance").option("value");
    var dateExpirationValue = moment($("#dateSelector").dxDateBox("instance").option("value")).format("YYYY-MM-DD");
    var percentageExpirationValue = $("#SurveyPercentage").dxSlider("instance").option("value");
    var currentPercentage = currentView.currentPercentage;
    var currentStatus = currentView.Status;
    var surveyMode = $("#radioSurveyMode").dxRadioGroup("instance").option("value");
    if (currentView.Id == null) currentView.Id = 0;
    var id = currentView.Id;

    var survey = surveyCreator.text;
    var advancedFilter = [document.getElementById('hdnMVCEmployees')?.value, document.getElementById('hdnMVCFilter')?.value, document.getElementById('hdnMVCFilterUser')?.value];

    if (title != "") {
        if ((percentageExpiration == false) || (percentageExpiration == true && percentageExpirationValue > 0)) {
            if (moment($("#dateSelector").dxDateBox("instance").option("value")).isAfter(moment())) {
                $.ajax({
                    type: "POST",
                    url: BASE_URL + 'Surveys/InsertSurvey',
                    dataType: "json",
                    data: { Id: id, Employees: currentView.Employees, Groups: currentView.Groups, Survey: survey, Title: title, Mandatory: mandatory, Anonymous: anonymous, ByPercentage: percentageExpiration, ExpirationDate: dateExpirationValue, ExpirationPercentage: percentageExpirationValue, AdvancedFilter: advancedFilter, CurrentPercentage: currentPercentage, CurrentStatus: currentStatus, SurveyMode: surveyMode },
                    success: function (e) {
                        if (e == false) {
                            enableSaveButton();
                            DevExpress.ui.notify(viewUtilsManager.DXTranslate('roSurveySaveError'), "error", 2000);
                        }
                        else {
                            enableSaveButton();
                            DevExpress.ui.notify(viewUtilsManager.DXTranslate('roSurveySaved'), "success", 2000);
                            currentView = e;
                            repaintPopUp(currentView);
                            cancelSurvey();
                        }
                    },
                    error: function (e) {
                        enableSaveButton();
                        DevExpress.ui.notify(viewUtilsManager.DXTranslate('roSurveySaveError'), "error", 2000);
                    }
                });
            }
            else {
                enableSaveButton();
                DevExpress.ui.notify(viewUtilsManager.DXTranslate('roSurveySaveErrorDate'), "error", 3000);
            }
        }
        else {
            enableSaveButton();
            DevExpress.ui.notify(viewUtilsManager.DXTranslate('roSurveySaveErrorPercentage'), "error", 3000);
        }
    }
    else {
        enableSaveButton();
        DevExpress.ui.notify(viewUtilsManager.DXTranslate('roSurveySaveErrorTitle'), "error", 3000);
    }
}

function setDefaultDate() {
    var date = new Date();
    date.setDate(date.getDate() + 7);
    return date;
}
function showSaveButton() {
    $("#surveySave").show();
}

function disableSaveButton() {
    $("#addNewSurvey").dxButton("instance").option("disabled", true);
}

function enableSaveButton() {
    $("#addNewSurvey").dxButton("instance").option("disabled", false);
}

function repaintPopUp(model) {
    switch (model.Status) {
        case 0:
            if ((model.Employees.length > 0 || model.Groups.length > 0) && model.Content != "") {
                $("#surveySend").show();
            }
            else {
                $("#surveySend").hide();
            }
            $("#surveyResults").hide();

            $("#SurveyName").dxTextBox("instance").option("disabled", false);
            $("#SurveyMandatory").dxSwitch("instance").option("disabled", false);
            $("#SurveyAnonymous").dxSwitch("instance").option("disabled", false);
            $("#SurveyExpiredByPercentage").dxCheckBox("instance").option("disabled", false);
            $("#SurveyPercentage").dxSlider("instance").option("disabled", false);
            $("#dateSelector").dxDateBox("instance").option("disabled", false);

            break;
        case 1:
            $("#surveySend").hide();
            $("#surveyResults").show();

            $("#SurveyName").dxTextBox("instance").option("disabled", true);
            $("#SurveyMandatory").dxSwitch("instance").option("disabled", true);
            $("#SurveyAnonymous").dxSwitch("instance").option("disabled", true);
            $("#SurveyExpiredByPercentage").dxCheckBox("instance").option("disabled", true);
            if (model.CurrentPercentage == 100) {
                $("#SurveyPercentage").dxSlider("instance").option("disabled", true);
                $("#dateSelector").dxDateBox("instance").option("disabled", true);
            }

            break;
        case 2:
            $("#surveySend").hide();
            $("#surveyResults").show();

            $("#SurveyName").dxTextBox("instance").option("disabled", true);
            $("#SurveyMandatory").dxSwitch("instance").option("disabled", true);
            $("#SurveyAnonymous").dxSwitch("instance").option("disabled", true);
            $("#SurveyExpiredByPercentage").dxCheckBox("instance").option("disabled", true);
            if (model.CurrentPercentage == 100) {
                $("#SurveyPercentage").dxSlider("instance").option("disabled", true);
                $("#dateSelector").dxDateBox("instance").option("disabled", true);
            }
            break;
        case 3:

            $("#surveySend").hide();
            $("#surveyResults").show();
            $("#SurveyName").dxTextBox("instance").option("disabled", true);
            $("#SurveyMandatory").dxSwitch("instance").option("disabled", true);
            $("#SurveyAnonymous").dxSwitch("instance").option("disabled", true);
            $("#SurveyExpiredByPercentage").dxCheckBox("instance").option("disabled", true);
            if (model.CurrentPercentage == 100) {
                $("#SurveyPercentage").dxSlider("instance").option("disabled", true);
                $("#dateSelector").dxDateBox("instance").option("disabled", true);
            }

            break;
    }
}

function onSurveyPopupShown() {
    $("#loading").dxLoadIndicator("instance").option("visible", true);
    $("#divSurveyMode").hide();
    $("#configDiv").show();
    $("#surveyDiv").hide();
    $("#resultsDiv").hide();
    $("#surveyResults").hide();
    $("#surveyConfiguration").addClass("bTabSurveys-active")
    $("#surveyDesign").removeClass("bTabSurveys-active")
    $("#surveyResults").removeClass("bTabSurveys-active")
    $("#surveyDesign").addClass("bTabSurveys")
    $("#surveyResults").addClass("bTabSurveys")

    $('#surveyConfiguration').off("click");
    $('#surveyConfiguration').on("click", function (e) {
        $("#configDiv").show();
        $("#surveyDiv").hide();
        $("#resultsDiv").hide();
        $("#surveyConfiguration").addClass("bTabSurveys-active")
        $("#surveyDesign").removeClass("bTabSurveys-active")
        $("#surveyResults").removeClass("bTabSurveys-active")
    });

    $('#surveyDesign').off("click");
    $('#surveyDesign').on("click", function (e) {
        $("#configDiv").hide();
        $("#surveyDiv").show();
        $("#resultsDiv").hide();
        $("#surveyConfiguration").removeClass("bTabSurveys-active")
        $("#surveyDesign").addClass("bTabSurveys-active")
        $("#surveyResults").removeClass("bTabSurveys-active")
    });

    $('#surveyResults').off("click");
    $('#surveyResults').on("click", function (e) {
        $("#configDiv").hide();
        $("#surveyDiv").hide();
        $("#resultsDiv").show();
        if (currentView != null) {
            getSurveyResponses(currentView.Content);
        }
        $("#surveyConfiguration").removeClass("bTabSurveys-active")
        $("#surveyDesign").removeClass("bTabSurveys-active")
        $("#surveyResults").addClass("bTabSurveys-active")
    });
    $('#divDestination').off("click");
    $('#divDestination').on("click", function (e) {
        
        let currentSelectorView = { Employees: [], Groups: [], Filter: "11110", UserFields: "", ComposeMode: 'Custom', ComposeFilter: "", Advanced: false, Operation: "or" };

        if (currentView != null) {
            if (currentView.Employees != null) {
                for (let i = 0; i < currentView.Employees.length; i++) currentSelectorView.ComposeFilter += currentSelectorView.ComposeFilter == '' ? `B${currentView.Employees[i]}` : `,B${currentView.Employees[i]}`;
            }

            if (currentView.Groups != null) {
                for (let i = 0; i < currentView.Groups.length; i++) currentSelectorView.ComposeFilter += currentSelectorView.ComposeFilter == '' ? `A${currentView.Groups[i]}` : `,A${currentView.Groups[i]}`;
            }
        }


        parent.showLoader(true);
        $("#divSurveysEmployeeSelector").load('/Employee/EmployeeSelectorPartial?feature=&pageName=surveys&config=100&unionType=or&advancedMode=1&advancedFilter=0&allowAll=0&allowNone=0', function () {
            loadPartialInfo();
            initUniversalSelector(currentSelectorView, false, 'surveysSelector');
            parent.showLoader(false);
        });
    });

    if ($('#idSurveySelected').val() != "") {
        $.ajax({
            type: "POST",
            url: BASE_URL + 'Surveys/GetSurvey',
            dataType: "json",
            data: { idSurvey: $('#idSurveySelected').val() },
            success: function (data) {
                currentView = data;

                if (data.Status == 0) {
                    $("#divSurveyMode").show();
                    if (data.AdvancedMode == false) {
                        configureSurveyCreator(true, 0);
                        $("#radioSurveyMode").dxRadioGroup("instance").option("value", 0);
                    }
                    else {
                        $("#radioSurveyMode").dxRadioGroup("instance").option("value", 1);
                        configureSurveyCreator(true, 1);
                    }
                }
                else {
                    $("#divSurveyMode").hide();
                    configureSurveyCreator(false, 0);
                }

                surveyCreator.text = data.Content;
                $("#EmployeeTextResults").dxTagBox("instance").option("dataSource", data.CurrentEmployeeResponses);
                $("#SurveyName").dxTextBox("instance").option("value", data.Title);
                $("#SurveyMandatory").dxSwitch("instance").option("value", data.IsMandatory);
                $("#SurveyAnonymous").dxSwitch("instance").option("value", data.Anonymous);
                $("#dateSelector").dxDateBox("instance").option("value", moment(data.ExpirationDate));

                if (data.ResponseMaxPercentage > 0) {
                    $("#SurveyExpiredByPercentage").dxCheckBox("instance").option("value", true);
                    $("#SurveyPercentage").dxSlider("instance").option("value", data.ResponseMaxPercentage);
                }

                if (data.Anonymous == true) {
                    $("#divResultsByEmployee").hide();
                } else {
                    $("#divResultsByEmployee").show();
                }

                let currentSelectorView = { Employees: [], Groups: [], Filter: "11110", UserFields: "", ComposeMode: 'Custom', ComposeFilter: "", Advanced: false, Operation: "or" };

                if (data != null) {
                    if (data.Employees != null) {
                        for (let i = 0; i < data.Employees.length; i++) currentSelectorView.ComposeFilter += currentSelectorView.ComposeFilter == '' ? `B${data.Employees[i]}` : `,B${data.Employees[i]}`;
                    }

                    if (data.Groups != null) {
                        for (let i = 0; i < data.Groups.length; i++) currentSelectorView.ComposeFilter += currentSelectorView.ComposeFilter == '' ? `A${data.Groups[i]}` : `,A${data.Groups[i]}`;
                    }
                }

                $("#SurveyDestination").dxTextBox("instance").option("value", buildSelectedEmployeesString(currentSelectorView));

                $("#surveySave").show();

                switch (currentView.Status) {
                    case 0:
                        if ((data.Employees.length > 0 || data.Groups.length > 0) && currentView.Content != "") {
                            $("#surveySend").show();
                        }
                        else {
                            $("#surveySend").hide();
                        }
                        $("#surveyResults").hide();
                        break;
                    case 1:
                        $("#surveySend").hide();
                        $("#surveyResults").show();
                        break;
                    case 2:
                        if (currentView.CurrentPercentage == 100) {
                            $("#surveySave").hide();
                        }

                        $("#surveySend").hide();
                        $("#surveyResults").show();
                        break;
                    case 3:
                        $("#surveySend").hide();
                        $("#surveyResults").show();
                        break;
                }

                if (currentView.Status != 0) {
                    $("#SurveyName").dxTextBox("instance").option("disabled", true);
                    $("#SurveyMandatory").dxSwitch("instance").option("disabled", true);
                    $("#SurveyAnonymous").dxSwitch("instance").option("disabled", true);
                    $("#SurveyExpiredByPercentage").dxCheckBox("instance").option("disabled", false);
                    if (currentView.CurrentPercentage == 100) {
                        $("#SurveyPercentage").dxSlider("instance").option("disabled", true);
                        $("#dateSelector").dxDateBox("instance").option("disabled", true);
                    }
                }
                else {
                    $("#SurveyName").dxTextBox("instance").option("disabled", false);
                    $("#SurveyMandatory").dxSwitch("instance").option("disabled", false);
                    $("#SurveyAnonymous").dxSwitch("instance").option("disabled", false);
                    $("#SurveyExpiredByPercentage").dxCheckBox("instance").option("disabled", false);
                    $("#SurveyPercentage").dxSlider("instance").option("disabled", false);
                    $("#dateSelector").dxDateBox("instance").option("disabled", false);
                }

                $("#loading").dxLoadIndicator("instance").option("visible", false);
            },
            error: function () { }
        });
    }
    else {
        $("#divSurveyMode").show();
        $("#radioSurveyMode").dxRadioGroup("instance").option("value", 0);
        configureSurveyCreator(true, 0);
        currentView = { Employees: [], Groups: [], Id: 0 }
        surveyCreator.text = "";
        $("#SurveyName").dxTextBox("instance").option("value", "");
        $("#SurveyMandatory").dxSwitch("instance").option("value", "");
        $("#SurveyAnonymous").dxSwitch("instance").option("value", "");
        $("#SurveyExpiredByPercentage").dxCheckBox("instance").option("value", false);
        $("#SurveyPercentage").dxSlider("instance").option("value", 0);
        $("#dateSelector").dxDateBox("instance").option("value", moment(Date.now()).add(7, 'days'));
        $("#SurveyDestination").dxTextBox("instance").option("value", "");
        $("#surveySend").hide();
        $("#surveyResults").hide();
        $("#SurveyName").dxTextBox("instance").option("disabled", false);
        $("#SurveyMandatory").dxSwitch("instance").option("disabled", false);
        $("#SurveyAnonymous").dxSwitch("instance").option("disabled", false);
        $("#SurveyExpiredByPercentage").dxCheckBox("instance").option("disabled", false);
        $("#SurveyPercentage").dxSlider("instance").option("disabled", false);
        $("#dateSelector").dxDateBox("instance").option("disabled", false);
    }
    $("#loading").dxLoadIndicator("instance").option("visible", false);
}

function loadedSurveyTemplates() {
    return surveyTemplates;

}

function getNPSJSON() {
    return {
        "completedHtml": "<h3>Thank you for your feedback.</h3><h5>Your thoughts and ideas will help us to create a great product!</h5>",
        "completedHtmlOnCondition": [
            {
                "expression": "{nps_score} > 8",
                "html": "<h3>Thank you for your feedback.</h3><h5>We glad that you love our product. Your ideas and suggestions will help us to make our product even better!</h5>"
            }, {
                "expression": "{nps_score} < 7",
                "html": "<h3>Thank you for your feedback.</h3><h5> We are glad that you share with us your ideas.We highly value all suggestions from our customers. We do our best to improve the product and reach your expectation.</h5><br/>"
            }
        ],
        "pages": [
            {
                "name": "page1",
                "elements": [
                    {
                        "type": "rating",
                        "name": "nps_score",
                        "title": "On a scale of zero to ten, how likely are you to recommend our product to a friend or colleague?",
                        "isRequired": true,
                        "rateMin": 0,
                        "rateMax": 10,
                        "minRateDescription": "(Most unlikely)",
                        "maxRateDescription": "(Most likely)"
                    }, {
                        "type": "checkbox",
                        "name": "promoter_features",
                        "visibleIf": "{nps_score} >= 9",
                        "title": "What features do you value the most?",
                        "isRequired": true,
                        "validators": [
                            {
                                "type": "answercount",
                                "text": "Please select two features maximum.",
                                "maxCount": 2
                            }
                        ],
                        "hasOther": true,
                        "choices": [
                            "Performance", "Stability", "User Interface", "Complete Functionality"
                        ],
                        "otherText": "Other feature:",
                        "colCount": 2
                    }, {
                        "type": "comment",
                        "name": "passive_experience",
                        "visibleIf": "{nps_score} > 6  and {nps_score} < 9",
                        "title": "What is the primary reason for your score?"
                    }, {
                        "type": "comment",
                        "name": "disappointed_experience",
                        "visibleIf": "{nps_score} notempty",
                        "title": "What do you miss and what was disappointing in your experience with us?"
                    }
                ]
            }
        ],
        "showQuestionNumbers": "off"
    };
}

function getDummyCheckboxJSON() {
    return {
        questions: [
            {
                type: "checkbox",
                name: "car",
                title: "What car are you driving?",
                isRequired: true,
                hasNone: true,
                colCount: 4,
                choices: [
                    "Ford",
                    "Vauxhall",
                    "Volkswagen",
                    "Nissan",
                    "Audi",
                    "Mercedes-Benz",
                    "BMW",
                    "Peugeot",
                    "Toyota",
                    "Citroen"
                ]
            }
        ]
    };
}

function configureSurveyCreatorSimple(canEdit) {
    Survey.settings.allowShowEmptyTitleInDesignMode = false;

    Survey
        .Serializer
        .findProperty("itemvalue", "visibleIf")
        .visible = false;
    Survey
        .Serializer
        .findProperty("itemvalue", "enableIf")
        .visible = false;
    Survey
        .Serializer
        .findProperty("itemvalue", "text")
        .visible = false;
    // Make the detail editor for itemvalue invisible, hide Edit button
    SurveyCreator
        .SurveyQuestionEditorDefinition
        .definition["itemvalue[]@choices"]
        ;

    if (canEdit == true) {
        var options = {
            showLogicTab: false,
            showJSONEditorTab: false,
            showPropertyGrid: false,
            haveCommercialLicense: true,
            questionTypes: [
                "text",
                "checkbox",
                "emotionsratings",
                "radiogroup",
                "rating",
                "comment",
                "boolean"
            ],
            pageEditMode: "single",
            showTitlesInExpressions: true,
            allowEditExpressionsInTextEditor: false,
            showSurveyTitle: "always",
            designerHeight: document.documentElement.clientHeight - 175
        };
    }
    else {
        var options = {
            showLogicTab: false,
            showDesignerTab: false,
            showPropertyGrid: false,
            showInvisibleElementsInTestSurveyTab: false,
            showJSONEditorTab: false,
            haveCommercialLicense: true,
            questionTypes: [
                "text",
                "checkbox",
                "emotionsratings",
                "radiogroup",
                "rating",
                "comment",
                "boolean"

            ],
            pageEditMode: "single",
            showTitlesInExpressions: true,
            allowEditExpressionsInTextEditor: false,
            showSurveyTitle: "always",
            designerHeight: document.documentElement.clientHeight - 175
        };
    }

    surveyCreator = new SurveyCreator.SurveyCreator(null, options);
    //SurveyCreator.SurveyQuestionEditorDefinition.definition = {};

    var maxVisibleIndex = 0;
    function showTheProperty(className, propertyName, visibleIndex) {
        if (!visibleIndex)
            visibleIndex = ++maxVisibleIndex;
        else {
            if (visibleIndex > maxVisibleIndex)
                maxVisibleIndex = visibleIndex;
        }
        //Use Survey Serializer to find the property, it looks for property in the class and all it's parents
        var property = Survey
            .Serializer
            .findProperty(className, propertyName)
        if (!property)
            return;
        property.visibleIndex = visibleIndex;
        //Custom JavaScript attribute that we will use in onShowingProperty event
        property.showProperty = true;
    }

    showTheProperty("question", "name");
    showTheProperty("question", "title");
    showTheProperty("question", "description");
    showTheProperty("checkbox", "choices");
    showTheProperty("text", "inputType");
    showTheProperty("text", "placeHolder");
    showTheProperty("comment", "placeHolder");
    showTheProperty("comment", "rows");

    surveyCreator
        .onShowingProperty
        .add(function (sender, options) {
            options.canShow = options.property.showProperty === true;
        });

    // Remove toolbar items except undo/redo buttons
    surveyCreator
        .toolbarItems
        .splice(2, 5);

    //surveyCreator.placeholderHtml = '<div style="position: absolute; top: 50%; left: 50%; transform: translate(-50%, -50%); text-align: center;">' + '' + '<div style="font-size: 16px; max-width: 210px;">' + '¡Arrastra un tipo de pregunta para empezar a diseñar tu encuesta!' + '</div>' + '</div>';

    // Adorners for item inplace editing edit itemvalue.value and not itemvalue.text
    surveyCreator.inplaceEditForValues = true;
    // Hide Fast Entry option for ItemValue[] editor
    surveyCreator
        .onSetPropertyEditorOptions
        .add(function (sender, options) {
            options.editorOptions.showTextView = false;
        });


    if (!window.surveyCreator && surveyCreator !== undefined) {
        window.surveyCreator = surveyCreator;
    }

    surveyCreator.isAutoSave = true;
    surveyCreator.saveSurveyFunc = saveMySurvey;
    surveyCreator.showInvisibleElementsInTestSurveyTab = false;
    surveyCreator.toolbox.removeItem("microphone");

    surveyCreator.render("surveyCreador");

    if (currentView != null) {
        surveyCreator.text = currentView.Content;
    }
}

function configureSurveyCreatorAdvanced(canEdit) {
    var options = null;

    if (canEdit == true) {
        options = {
            showDesignerTab: true,
            haveCommercialLicense: true,
            showEmbeddedSurveyTab: false,
            showTestSurveyTab: true,
            showInvisibleElementsInTestSurveyTab: false,
            showToolbox: "right",
            showPropertyGrid: "right",
            showSurveyTitle: "never",
            showJSONEditorTab: false,
            questionTypes: ["text", "checkbox", "radiogroup", "dropdown", "comment", "rating", "ranking", "imagepicker", "boolean", "matrix", "matrixdropdown", "matrixdynamic", "multipletext", "emotionsratings"],
            designerHeight: document.documentElement.clientHeight - 175
        };
    }
    else {
        options = {
            showDesignerTab: false,
            haveCommercialLicense: true,
            showEmbeddedSurveyTab: false,
            showTestSurveyTab: true,
            showJSONEditorTab: false,
            showInvisibleElementsInTestSurveyTab: false,
            showToolbox: "right",
            showPropertyGrid: "right",
            showSurveyTitle: "never",
            questionTypes: ["text", "checkbox", "radiogroup", "dropdown", "comment", "rating", "ranking", "imagepicker", "boolean", "matrix", "matrixdropdown", "matrixdynamic", "multipletext", "emotionsratings"],
            designerHeight: document.documentElement.clientHeight - 175
        };
    }

    surveyCreator = new SurveyCreator.SurveyCreator(null, options);

    surveyCreator.rightContainerActiveItem("toolbox");
    surveyCreator.isAutoSave = true;
    surveyCreator.showInvisibleElementsInTestSurveyTab = false;
    surveyCreator.saveSurveyFunc = saveMySurvey;

    surveyCreator.toolbox.removeItem("microphone");

    if (!window.surveyCreator && surveyCreator !== undefined) {
        window.surveyCreator = surveyCreator;
    }

    surveyCreator.render("surveyCreador");

    if (currentView != null) {
        surveyCreator.text = currentView.Content;
    }
}

function configureSurveyCreator(canEdit, mode) {
    var langKey = JSON.parse(localStorage.getItem("roLanguage")).key;

    SurveyCreator.localization.locales[langKey] = JSON.parse(localStorage.getItem("roLanguage")).survey;
    SurveyCreator.editorLocalization.currentLocale = langKey;
    SurveyCreator.editorLocalization.defaultLocale = langKey;
    SurveyCreator.StylesManager.applyTheme("orange");

    if (IsAdvancedEdition.toLowerCase() != 'true') {
        configureSurveyCreatorSimple(canEdit);
    }
    else {
        if (mode == 0) {
            configureSurveyCreatorSimple(canEdit);
        }
        else {
            configureSurveyCreatorAdvanced(canEdit);
        }
    }
}

function refreshSurveys() {
    var surveySurvey = $("#gridStatusSurveys").dxDataGrid("instance");
    surveySurvey.refresh();
}
function onSurveyPopupHiding() {
    $('#idSurveySelected').val("");
    $('#surveySelectedStatus').val("");
    $("#SurveyExpiredByPercentage").dxCheckBox("instance").option("value", false);
    $("#SurveyExpiredByPercentage").dxCheckBox("instance").option("disabled", false);
    $("#SurveyPercentage").dxSlider("instance").option("value", 0);
    var surveySurvey = $("#gridStatusSurveys").dxDataGrid("instance");
    surveySurvey.refresh();
}

function saveMySurvey() {
    currentView.Content = surveyCreator.text;
}

function surveySelected(selectedItems) {
    if (selectedItems.row.rowType == "data") {
        var popup = $("#newSurveyPopup").dxPopup("instance");
        $('#idSurveySelected').val(selectedItems.data.Id);
        $('#surveySelectedStatus').val(selectedItems.data.Status);
        $('#surveySelectedStatus').val(selectedItems.data.SurveyMode);
        popup.show();
    }
}

function SurveyRemoved() {
    $('#onboardingRemoved').val("true");
    $.ajax({
        type: "GET",
        url: BASE_URL + 'Survey/LoadInitialData',
        contentType: "application/json; charset=utf-8",
        success: function (data) {
        },
        error: function () { }
    });
}

function context_menu(e) {
}

function RefreshSurveySurvey() {
    var surveySurvey = $("#gridStatusSurveys").dxDataGrid("instance");
    surveySurvey.refresh();
}

window.parentCloseAndApplySelector = function (currentSelection) {
    currentView.Employees = currentSelection.Employees;
    currentView.Groups = currentSelection.Groups;

    $("#SurveyDestination").dxTextBox("instance").option("value", buildSelectedEmployeesString(currentSelection));
};

var headerFilter = {
    load: function (loadOptions) {
        return [{
            text: window.parent.Globalize.formatMessage("SURVEY_DRAFT"),
            value: [['Status', '=', '0']]
        }, {
            text: window.parent.Globalize.formatMessage("SURVEY_IN_PROGRESS"),
            value: [['Status', '=', '1']]
        }, {
            text: window.parent.Globalize.formatMessage("SURVEY_COMPLETED"),
            value: [['Status', '=', '2']]
        }];
    }
}

function onCellPrepared(e) {
    if (e.rowType == "header") {
        e.cellElement.css("text-align", "left");
    }
}

function progressBar(container, options) {
    if (options.displayValue != null) {
        var sDate = moment();
        var lpDate = moment(options.row.data.ExpirationDate);
        var totalDays = lpDate.diff(sDate, 'days') + 1;

        switch (options.row.data.Status) {
            case 0:

                $("<span style='float: left;background: #FF5C3575;border-radius: 4px;text-align: center;color: white;width: 300px;'>" + window.parent.Globalize.formatMessage("SURVEY_DRAFT") + " </span>").appendTo(container);
                break;
            case 1:
                $("<div id='proBar'/>").dxProgressBar({
                    min: 0,
                    max: 100,
                    value: options.row.data.CurrentPercentage,
                    width: 300,
                    statusFormat: function (ratio, value) { return viewUtilsManager.DXTranslate('roSurveyAnswers') + ': ' + options.row.data.Progress + ' (' + options.row.data.CurrentPercentage + '%' + ')' + ' - ' + viewUtilsManager.DXTranslate('roSurveyRemaining') + ' ' + totalDays + 'd' }
                }).appendTo(container);

                break;
            case 2:
                $("<span style='float: left;background: #0f8ed075;border-radius: 4px;text-align: center;color: white;width: 300px;'> " + window.parent.Globalize.formatMessage("SURVEY_COMPLETED") + " </span>").appendTo(container);
                break;
            case 3:
                $("<span style='float: left;background: #d00f0f75;border-radius: 4px;text-align: center;color: white;width: 300px;'> " + window.parent.Globalize.formatMessage("SURVEY_CANCELED")  + "</span>").appendTo(container);
                break;
            default:
        }

    }
}

function employeeResultsSelected(e) {
    resultsSelected = $("#EmployeeTextResults").dxTagBox("instance").option("selectedItems");

    if (survey != null) {
        if (e.addedItems.length == 0) {
            if (currentView != null) {
                paintResults(0);
            }
        }
        else {
            paintResults(resultsSelected);
        }
    }
}

function switchModeExternal(data) {
    if (data == "Tabla") {
        showTable();
    }
    else {
        showGraphics();
    }
}
function switchMode(data) {
    if (data.value == "Tabla") {
        showTable();
    }
    else {
        showGraphics();
    }
}
function switchSurveyMode(data) {
    if (currentView != null) {
        currentSurveyContent = currentView.Content;
    }
    else {
        currentSurveyContent = "";
    }
    configureSurveyCreator(true, data.value);
}

function showGraphics() {
    var langKey = JSON.parse(localStorage.getItem("roLanguage")).key;

    SurveyAnalytics.localization.locales[langKey] = JSON.parse(localStorage.getItem("roLanguage")).surveyAnalytics;
    SurveyAnalytics.localization.currentLocale = langKey;
    SurveyAnalytics.localization.defaultLocale = langKey;

    var visPanel = new SurveyAnalytics.VisualizationPanel(survey.getAllQuestions(), responses, { haveCommercialLicense: true, labelTruncateLength: 27 });
    visPanel.showHeader = true;
    surveyResultNode.innerHTML = "";
    visPanel.render(surveyResultNode);
    $("#loadingIndicator").hide();
}

function showTable() {
    var langKey = JSON.parse(localStorage.getItem("roLanguage")).key;

    SurveyAnalyticsTabulator.localization.locales[langKey] = JSON.parse(localStorage.getItem("roLanguage")).surveyAnalytics;
    SurveyAnalyticsTabulator.localization.currentLocale = langKey;
    SurveyAnalyticsTabulator.localization.defaultLocale = langKey;

    var surveyAnalyticsTabulator = new SurveyAnalyticsTabulator.Tabulator(survey, responses, { haveCommercialLicense: true });
    surveyResultNode.innerHTML = "";
    if (IsAdvancedEdition == 'False') {
        surveyAnalyticsTabulator.options.downloadButtons = [];
    }
    surveyAnalyticsTabulator.render(surveyResultNode);
    $("#loadingIndicator1").hide();
}

function groupSelected(e) {
    if (e.addedItems.length > 0) {
        $('#idGroupsSelected').val(e.addedItems);
    }
}

function employeeSelector(container, options) {
    $("<div class='custom-item'><img src='" +
        container.Image + "' /><div class='product-name'>" +
        container.EmployeeName + "</div></div>").appendTo(container);
}

function AllowModify() {
    if (Permission > 3) {
        return true;
    }
    else {
        return false;
    }
}

function hasData() {
    var refreshPressed = localStorage.getItem("refreshPressed");
    localStorage.removeItem("refreshPressed");
    return refreshPressed;
}

function cancelSurvey() {
    var popup = $("#newSurveyPopup").dxPopup("instance");
    popup.hide();
}

function SetGridHeight(e) {
    e.rowElement.css({ height: 70 });
}

function beforeSend(operation, ajaxSettings) {
}

function formatLabel(value) {
    return value + "%";
}

function byPercentage(value) {
    if (value.value == true) {
        $("#SurveyPercentage").dxSlider("instance").option("disabled", false);
    }
    else {
        $("#SurveyPercentage").dxSlider("instance").option("disabled", true);
    }
}