VTPortal.documentInput = function (params) {
    var selectedFiles = ko.observableArray(['']);
    var formImageUri = ko.observable('');
    var formReadOnly = ko.observable(false);
    var originalPhoto = ko.observable('');
    var canSelectTemplate = ko.observable(true);
    var idDocumentTemplate = ko.observable(-1);
    var documentTemplateDS = ko.observable([]);
    var documentIdTemplate = ko.observable('');

    var selectFileText = ko.observable(i18nextko.i18n.t('roFindFile'));

    function initializeData(templates, parentReadOnly, previousIdDocumentTemplate) {
        selectedFiles(['']);
        selectFileText(i18nextko.i18n.t('roFindFile'));
        canSelectTemplate(true);
        formImageUri('')
        idDocumentTemplate(-1);
        formReadOnly(parentReadOnly);
        documentTemplateDS(templates);
        if (templates.length > 0) {
            if (previousIdDocumentTemplate == -1) {
                idDocumentTemplate(templates[0].FieldValue);
                if (templates.length == 1) canSelectTemplate(false);
            } else {
                canSelectTemplate(false);
                idDocumentTemplate(previousIdDocumentTemplate + "")
            }
        }
    }

    function getDocument() {
        var formData = new FormData();

        formData.append("idTemplateDocument", idDocumentTemplate());

        if (VTPortal.roApp.isModeApp() == false) formData.append("userfile", selectedFiles()[0]);
        else formData.append("userfile", originalPhoto());

        return formData;
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

    var viewModel = {
        lblDocumentSelect: i18nextko.t('roRequestDocumentType'),
        lblImageCapture: i18nextko.t('roSelectFile'),
        lblSelectFile: i18nextko.t('roSelectFile'),
        selectPhotoEvent: selectPhotoEvent,
        imageSrc: formImageUri,
        appMode: VTPortal.roApp.isModeApp(),
        cmbDocumentSelect: {
            dataSource: documentTemplateDS,
            displayExpr: "FieldName",
            valueExpr: "FieldValue",
            value: idDocumentTemplate,
            readOnly: formReadOnly
        },
        uploadOptions: {
            multiple: false,
            accept: "*",
            value: selectedFiles,
            uploadMode: "useForm",
            showFileList: false,
            selectButtonText: selectFileText,
            onValueChanged: function (e) {
                selectFileText(e.value[0].name);
                documentIdTemplate(idDocumentTemplate())
            }
        },
        initializeData: initializeData,
        getDocument: getDocument,
        documentIdTemplate: documentIdTemplate,
        canSelectTemplate: canSelectTemplate
    };

    return viewModel;
};