VTPortal.sendDocument = function (params) {
    var documentTemplateId = ko.observable(-1), relatedObjectId = ko.observable(-1), remarks = ko.observable('');
    var vDocDate = ko.observable(null);
    var originalPhoto = ko.observable('');
    var forecastType = ko.observable('');
    var isCmbVisible = ko.computed(function () {
        return true;
    });

    var infoText = ko.computed(function () {
        if (documentTemplateId() == -1) return i18nextko.i18n.t('roRequestDocumentType');
        else return i18nextko.i18n.t('roRequestSelectFile');
    });

    var canSave = ko.observable(false);

    var selectFileText = ko.observable(i18nextko.i18n.t('roFindFile'));

    var selectedFiles = ko.observableArray();
    var formImageUri = ko.observable('');

    if (typeof params.id != 'undefined' && parseInt(params.id, 10) != -1) documentTemplateId(parseInt(params.id, 10));
    if (typeof params.param != 'undefined' && parseInt(params.param, 10) != -1) relatedObjectId(parseInt(params.param, 10));
    if (typeof params.iDate != 'undefined' && parseInt(params.iDate, 10) != -1) relatedCause(parseInt(params.iDate, 10));
    if (typeof params.objId != 'undefined' && params.objId != '') forecastType(params.objId);

    var reqValue1 = ko.observable(documentTemplateId());
    var documentTypeDS = ko.observable([]);
    var computedScrollHeight = ko.computed(function () {
        return '76%'
    });

    var loadLists = function () {
        new WebServiceRobotics(function (result) {
            if (result.Status == window.VTPortalUtils.constants.OK.value) {
                documentTypeDS(result.SelectFields);

                if (documentTemplateId() != -1) cmbDocumentTypeChanged({ value: documentTemplateId() + "" })
            } else {
                var onContinue = function () {
                    VTPortal.roApp.redirectAtHome();
                }
                VTPortalUtils.utils.processErrorMessage(result, onContinue);
            }
        }, function (error) {
            var onContinue = function () {
                VTPortal.roApp.redirectAtHome();
            }
            VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roErrorLoadingCauseInfo"), 'error', 0, onContinue);
        }).getAvailableDocumentTemplateType(VTPortal.roApp.employeeId, false);
    }

    var globalStatus = ko.observable(VTPortal.bigUserInfo());

    function viewShown() {
        globalStatus().viewShown();

        if (!VTPortal.roApp.isTimeGate()) canSave(true);
        else canSave(false);

        vDocDate(null);
        loadLists();
    }

    function saveDocument() {
        if (VTPortal.roApp.isModeApp() == false) {
            var files = selectedFiles();

            var formData = new FormData();
            formData.append("idTemplateDocument", documentTemplateId() != -1 ? documentTemplateId() : reqValue1());
            formData.append("idRelatedObject", relatedObjectId());
            formData.append("remarks", remarks());
            formData.append("docRelatedInfo", "");
            formData.append("forecastType", forecastType());
            // HTML file input, chosen by user
            formData.append("userfile", files[0]);

            new WebServiceRobotics(function (result) {
                if (result.Status == window.VTPortalUtils.constants.OK.value) {
                    VTPortal.roApp.redirectAtHome();
                    VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roDocumentSaved"), 'success', 2000);
                    VTPortal.roApp.loadInitialData(false, false, false, false, false);
                } else {
                    var onContinue = function () {
                    }
                    VTPortalUtils.utils.processErrorMessage(result, onContinue);
                }
            }, function (error) {
                var onContinue = function () {
                    VTPortal.roApp.redirectAtHome();
                }
                VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roErrorUploadingFile"), 'error', 0, onContinue);
            }).uploadDocumentDesktop(formData);
        }
        else {
            var formData = new FormData();
            formData.append("idTemplateDocument", documentTemplateId() != -1 ? documentTemplateId() : reqValue1());
            formData.append("userfile", originalPhoto());
            formData.append("idRelatedObject", relatedObjectId());
            //document.append("forecastType", "request");

            var docRelatedInfo = "";
            new WebServiceRobotics(function (result) {
                if (result.Status == window.VTPortalUtils.constants.OK.value) {
                    VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roDocumentSaved"), 'success', 2000);
                    VTPortal.roApp.redirectAtHome();
                    VTPortal.roApp.loadInitialData(false, false, false, false, false);
                } else {
                    var onContinue = function () {
                    }
                    VTPortalUtils.utils.processErrorMessage(result, onContinue);
                }
            }, function (error) {
                var onContinue = function () {
                    VTPortal.roApp.redirectAtHome();
                }
                VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roErrorUploadingFile"), 'error', 0, onContinue);
            }).uploadDocumentMobile(formData, forecastType(), remarks(), docRelatedInfo);
        }

        //reseteamos las alertas por si hemos guardado un fichaje olvidado que teniamos como alerta.
        VTPortal.roApp.db.settings.markForRefresh(['notifications']);
    }

    function selectPhotoEvent() {
        VTPortalUtils.utils.isShowingPopup(false);
        if (Object.has(navigator, 'camera')) {
            var selectFromLibrary = function () {
                navigator.camera.getPicture(
                    function (imageURI) {
                        originalPhoto(imageURI);
                        if (typeof (device) != 'undefined') {
                            if (cordova.platformId === 'ios') {
                                formImageUri(window.WkWebView.convertFilePath(imageURI));
                            }
                            else {
                                formImageUri(imageURI);
                            }
                        }
                        else {
                            formImageUri(imageURI);
                        }
                    },
                    function (message) {
                        if (!message.includes('cancelled')) { VTPortalUtils.utils.notifyMesage(message, 'error', 2000); }
                    },
                    {
                        quality: 75,
                        sourceType: Camera.PictureSourceType.PHOTOLIBRARY,
                        destinationType: Camera.DestinationType.FILE_URI,
                        allowEdit: false,
                        encodingType: Camera.EncodingType.JPEG,
                        targetWidth: 960,
                        targetHeight: 1280,
                        saveToPhotoAlbum: false,
                        correctOrientation: true
                    }
                );
            };
            var takeFromCamera = function () {
                navigator.camera.getPicture(
                    function (imageURI) {
                        originalPhoto(imageURI);
                        if (typeof (device) != 'undefined') {
                            if (cordova.platformId === 'ios') {
                                formImageUri(window.WkWebView.convertFilePath(imageURI));
                            }
                            else {
                                formImageUri(imageURI);
                            }
                        }
                        else {
                            formImageUri(imageURI);
                        }
                    },
                    function (message) {
                        if (!message.includes('cancelled')) { VTPortalUtils.utils.notifyMesage(message, 'error', 2000); }
                    },
                    {
                        quality: 75,
                        sourceType: Camera.PictureSourceType.CAMERA,
                        destinationType: Camera.DestinationType.FILE_URI,
                        allowEdit: false,
                        encodingType: Camera.EncodingType.JPEG,
                        targetWidth: 960,
                        targetHeight: 1280,
                        saveToPhotoAlbum: false,
                        correctOrientation: true
                    }
                );
            };

            VTPortalUtils.utils.questionMessage(i18nextko.i18n.t('roSelectOrigin'), 'info', 0, selectFromLibrary, takeFromCamera, i18nextko.i18n.t('roTakeFromLibrary'), i18nextko.i18n.t('roTakeFromCamera'));
        } else {
            VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t('roNoCameraAvailable'), 'error', 0);
        }
    }

    var cmbDocumentTypeChanged = function (e) {
        var idDocTemp = e.value;
        var selItem = null;

        $.grep(documentTypeDS(), function (e) {
            if (e.FieldValue == idDocTemp) { selItem = e; }
        })
    }

    var viewModel = {
        viewShown: viewShown,
        title: i18nextko.t('roRequestType_SendDocument'),
        lblRemarks: i18nextko.t('roRequestRemarksLbl'),
        lblRequestValue1: infoText,
        lblImageCapture: i18nextko.t('roTakePhoto'),
        lblSelectFile: i18nextko.t('roSelectFile'),
        lblDocDate: i18nextko.t('roRequestDocRelatedDate'),
        subscribeBlock: globalStatus(),

        scrollContent: {
            height: computedScrollHeight
        },
        btnSave: {
            onClick: saveDocument,
            text: '',
            hint: i18nextko.i18n.t('roSaveRequest'),
            icon: "Images/Common/save.png",
            visible: canSave
        },
        remarks: {
            value: remarks
        },
        cmbDocumentType: {
            dataSource: documentTypeDS,
            displayExpr: "FieldName",
            valueExpr: "FieldValue",
            value: reqValue1,
            visible: isCmbVisible,
            onValueChanged: cmbDocumentTypeChanged,
        },

        loadingPanel: VTPortalUtils.utils.loadingPanelConf(),
        selectPhotoEvent: selectPhotoEvent,
        imageSrc: formImageUri,
        appMode: VTPortal.roApp.isModeApp(),
        uploadOptions: {
            multiple: false,
            accept: "*",
            value: selectedFiles,
            uploadMode: "useForm",
            showFileList: false,
            selectButtonText: selectFileText,
            onValueChanged: function (e) {
                selectFileText(e.value[0].name)
            }
        },
        isTimeGate: VTPortal.roApp.isTimeGate(),
        lblTimegateRestricted: i18nextko.t('roRestrictedTGfunctionallity')
    };

    return viewModel;
};