//*******************************************************************************************
// ** Author: A. Sánchez.
// ** Date: 23/03/2021.
// ** Description: javascript file for Documentary Management module.
//*******************************************************************************************
(function () {

    var viewHandler = null;
    var searchTerm = "";
    var delayTimer;
    var isGlobalSearch = false;

    $(document).ready(async function () {
        window.hasChanges(false);

        viewUtilsManager.initAccordions();
        viewUtilsManager.setupCardListFilterButton("DocumentaryManagement");

        viewUtilsManager.print("Documentary Management Module loaded");
        viewHandler = viewUtilsManager.createViewStateHandler();

        // -----------------------------
        // GET DATA VIEW (ON READY) ----
        //------------------------------
        window.loadRequest = loadSelected;
        window.editCurrentDocumentTemplate = editCurrentDocumentTemplate;
        window.deleteCurrentDocumentTemplate = deleteCurrentDocumentTemplate;
        top.confirmDeleteDocumentTemplate = confirmDeleteDocumentTemplate;

        window.addNewDocumentTemplate = addNewDocumentTemplate;
        window.RefreshScreen = RefreshScreen;

        $("#fmDocuments_limit__button").on("click",() => {
            $("#fmDocuments_limit__button").toggleClass("active");
            loadSelected();
            //Creamos un popup de loading para la vista de thumbnails, que no existía
            $("#fmDocuments .dx-filemanager-files-view.dx-filemanager-thumbnails").append('<div id="fmDocuments__customLoadPanel" class="dx-overlay dx-widget dx-visibility-change-handler dx-loadpanel" > <div class="dx-overlay-content dx-resizable dx-loadpanel-content" style="width: 200px; height: 90px; z-index: 20002; margin: 0px; left: 0px; top: 0px; transform: translate(661px, 180px); transition: all 0s ease 0s;" aria-hidden="true"><div class="dx-loadpanel-content-wrapper"><div class="dx-loadpanel-indicator dx-loadindicator dx-widget"><div class="dx-loadindicator-wrapper"><div class="dx-loadindicator-content"><div class="dx-loadindicator-icon"><div class="dx-loadindicator-segment dx-loadindicator-segment7"></div><div class="dx-loadindicator-segment dx-loadindicator-segment6"></div><div class="dx-loadindicator-segment dx-loadindicator-segment5"></div><div class="dx-loadindicator-segment dx-loadindicator-segment4"></div><div class="dx-loadindicator-segment dx-loadindicator-segment3"></div><div class="dx-loadindicator-segment dx-loadindicator-segment2"></div><div class="dx-loadindicator-segment dx-loadindicator-segment1"></div><div class="dx-loadindicator-segment dx-loadindicator-segment0"></div></div></div></div></div><div class="dx-loadpanel-message">Cargando...</div></div></div></div >');
        });
       
       

        document.addEventListener("startStateEvent", (data) => viewHandlerEvent(data), false);

        viewHandler.transition(viewHandler.value, "read");

        viewUtilsManager.loadViewOptions("DocumentaryManagement", "read", function () {

           

            $("#tbGlobalSearchToolbar").dxTextBox({
                placeholder: viewUtilsManager.DXTranslate("SearchInAllFolders"), 
                mode: "search",
                valueChangeEvent: "keyup",
                onValueChanged: function (e) {

                    var _searchTerms = $("#tbGlobalSearchToolbar").dxTextBox("instance").option("text").toString();

                    $("#fmDocuments_limit").removeClass("show");
                    $("#fmDocuments_limit__button").removeClass("active");

                    clearTimeout(delayTimer);
                    delayTimer = setTimeout(function () {

                        isGlobalSearch = true;
                        globalSearchToolbar(_searchTerms);
                    }, 1000);


                }
            });

            viewUtilsManager.getHtmlControl("fmDocuments").option('toolbar', {
                items: [
                    {
                        name: "upload",
                        text: viewUtilsManager.DXTranslate("UploaFile"),
                        visible: true,
                        location: "before"
                    },
                    {
                        widget: "dxTextBox",
                        location: "after",
                        id: "fmTextBoxS",
                        options: {
                            id: "fileManagerSearchTerm",
                            buttons: [{
                                name: "search",
                                location: "after",
                                options: {
                                    icon: "search",
                                    type: "normal",
                                    onClick: function () {
                                        loadSelected();
                                    }
                                }
                            }],
                            valueChangeEvent: "keyup",
                            onValueChanged: function (data) {
                                loadSelected(data.value);
                            }
                        }
                    },
                    "refresh",
                    {
                        name: "separator",
                        location: "after"
                    },
                    "switchView"
                ]
            });

            viewUtilsManager.getHtmlControl("fmDocuments").option('fileSystemProvider', new DevExpress.fileManagement.CustomFileSystemProvider({
                keyExpr: 'keyExpr',
                getItems: function (pathInfo) {

                    var _templateIdSelected = viewUtilsManager.getSelectedCardId();
                   
                    if (isGlobalSearch)
                        _templateIdSelected = -1;

                    const showAll = $("#fmDocuments_limit__button").hasClass("active");

                    return viewUtilsManager.makeServiceCall(
                        "DocumentaryManagement",
                        "GetDocuments",
                        "POST",
                        { templateId: _templateIdSelected, searchTerm: searchTerm, showAll: showAll },
                        null,
                        function (response) {
                            if (response) {
                                $.ajax({
                                    url: `${BASE_URL}DocumentaryManagement/GetLimitReached`,
                                    type: "POST",
                                    contentType: false,
                                    processData: false,
                                    success: function (res) {
                                        if ((res == true || res == "True") && response.length == 200) {
                                            if (!$("#fmDocuments_limit").hasClass("show")) $("#fmDocuments_limit").addClass("show");
                                            $("#fmDocuments_limit .js-text_warning").css("opacity", "1");
                                            if (showAll) {
                                                $("#fmDocuments_limit .js-text_warning").css("opacity", "0"); //ocultamos el aviso pero mantenemos el botón
                                                $("#fmDocuments_limit__button").removeClass("active");
                                            }
                                        } else {
                                            $("#fmDocuments_limit").removeClass("show");
                                            $("#fmDocuments_limit__button").removeClass("active");
                                            $("#fmDocuments__customLoadPanel").remove();  //eliminamos el loadingPanel custom de la vista thumbnails
                                        }
                                    },
                                    error: function (_err) {
                                        console.error(_err);
                                    }
                                });
                            } else {
                                $("#fmDocuments_limit").removeClass("show");
                            }

                            //var fmDocInstance = viewUtilsManager.getHtmlControl("fmDocuments");
                            if (isGlobalSearch) {                               
                               CardView.UnselectCards();                                
                                //fmDocInstance.option("disabled", true);
                            }
                            else {
                                //fmDocInstance.option("disabled", viewUtilsManager.getSelectedCardId() == 0);
                            }

                            $(".cardsTree-CardIcon").css("background", "url('Base/Images/StartMenuIcos/Documents.png')");
                            $(".CardsTree-cardClicked .cardsTree-CardIcon").css("background", "url('Base/Images/StartMenuIcos/DocumentsAbsences.png')");
                            isGlobalSearch = false;
                            return response;
                        },
                        null,
                        null
                    );
                },
                deleteItem: function (item) {
                    return viewUtilsManager.makeServiceCall(
                        "DocumentaryManagement",
                        "RemoveDocument",
                        "POST",
                        { documentId: item.dataItem.id },
                        null,
                        function (response) { return response; },
                        null,
                        null
                    );
                },
                uploadFileChunk: function (fileData, chunksInfo, destinationDir) {

                    var deferred = $.Deferred();

                    var frmUpload = new FormData();
                    frmUpload.append(fileData.name, fileData);
                    frmUpload.append('templateId', viewUtilsManager.getSelectedCardId());

                  
                    $.ajax({
                        url: `${BASE_URL}DocumentaryManagement/AddDocument`,
                        type: "POST",
                        contentType: false,  
                        processData: false, 
                        data: frmUpload,
                        success: function (result) {

                            if (result.StatusCode == 200 && result.StatusDescription == '') {
                                deferred.resolve({
                                    errorId: 0,
                                    fileItem: fileData,
                                    errorText: result.StatusDescription
                                });
                            }
                            else if (result.StatusCode == 200 && result.StatusDescription != '') {
                                deferred.resolve({
                                    errorId: 0,
                                    fileItem: fileData,
                                    errorText: ''
                                });
                                Robotics.Client.JSErrors.showJSerrorPopup(Robotics.Client.JSErrors.JSErrorTypes.roJsInfo, '',
                                    { text: '', key: 'roJsInfo' }, { text: result.StatusDescription, key: '' },
                                    { text: '', textkey: 'roErrorClose', desc: '', desckey: '', script: '' },
                                    Robotics.Client.JSErrors.createEmptyButton(), Robotics.Client.JSErrors.createEmptyButton(), Robotics.Client.JSErrors.createEmptyButton())
                            }
                            else {
                                deferred.reject({
                                    errorId: 0,
                                    fileItem: fileData,
                                    errorText: result.StatusDescription
                                });
                            }
                        },
                        error: function (_err) {

                            deferred.reject({
                                errorId: 0,
                                fileItem: fileData,
                                errorText: _err.StatusDescription
                            });
                        }

                    });

                    return deferred.promise();
                },
                abortFileUpload: function (fileData, chunksInfo, destinationDir) {

                },
                downloadItems: function (items) {

                    $(items).each(function (index) {
                        downloadFileFormSubmit(items[index], index);
                    });
                    
                }
            }));            

            viewUtilsManager.getHtmlControl("fmDocuments").option("customizeThumbnail", function (fileSystemItem) {

                const fileExtension = fileSystemItem.getFileExtension();
                switch (fileExtension) {
                    case ".pdf":
                        return "Base/Images/Documents/pdf-file-icon.svg";
                        break;
                    case ".xlsx":
                        return "Base/Images/Documents/xlsx-file-icon.png";
                        break;
                    case ".txt":
                        return "Base/Images/Documents/txt-file-icon.png";
                        break;
                    case ".rar":
                    case ".zip":
                        return "Base/Images/Documents/compressed-file-icon.png";
                        break;
                    case ".jpeg":
                        return "Base/Images/Documents/jpg-file-icon.png";
                        break;
                    case ".png":
                    case ".jpg":
                    case ".jpeg":
                        return "Base/Images/Documents/jpg-file-icon.png";
                        break;
                    default:
                        return "";
                }
            });

            $(".dx-widget.dx-filemanager-breadcrumbs").hide();
        });
    });

    function globalSearchToolbar(search) {

        searchTerm = search
        viewUtilsManager.makeServiceCall(
            "DocumentaryManagement",
            "FindAllDocuments",
            "POST",
            { searchTerm: searchTerm },
            null,
            function (response) {
                loadSelected();
            },
            null,
            null
        );
    };

    function viewHandlerEvent(eventData) {

        var eventDetails = eventData.detail;

        viewUtilsManager.print(eventDetails.currentState + " state");
        switch (eventDetails.currentState) {

            case "read":
                break;

            case "create":
                break;

            case "update":
                break;

            case "delete":
                break;
        }
    };

    function downloadFileFormSubmit(fileItem, currentIndex) {
        setTimeout(function () {
            var form = $('<form name="frmDownload_' + fileItem.dataItem.id + '"></form>').attr('action', `${BASE_URL}DocumentaryManagement/GetDocument`).attr('method', 'post');
            form.append($("<input></input>").attr('type', 'hidden').attr('name', 'documentId').attr('value', fileItem.dataItem.id));
            form.appendTo('body').submit().remove();
        }, (1000 * currentIndex));
    };

    const bindView = (response) => {
        
    }

    const loadSelected = (term) => {
        //console.log("load selected");
        if (searchTerm != undefined && term != undefined) {
            searchTerm = term;
        }
        var fmCtrl = viewUtilsManager.getHtmlControl("fmDocuments");
        if (fmCtrl != null)
            fmCtrl.refresh(); 
    };

    const RefreshScreen = (DataType, params) => {
        if (DataType == "save") refreshCardTree(params);
    }

    const editCurrentDocumentTemplate = () => {

        var _templateIsSystemSelected = viewUtilsManager.getSelectedCardIsSystem();
        if (_templateIsSystemSelected == "True") {

            this.parent.Robotics.Client.JSErrors.showJSerrorPopup(this.parent.Robotics.Client.JSErrors.JSErrorTypes.roJsInfo, '',
                { text: '', key: 'roJsInfo' }, { text: '', key: 'roDocumentaryManagementIsSystem' },
                { text: '', textkey: 'Done', desc: '', desckey: '', script: '' },
                this.parent.Robotics.Client.JSErrors.createEmptyButton(), this.parent.Robotics.Client.JSErrors.createEmptyButton(), this.parent.Robotics.Client.JSErrors.createEmptyButton());

        }
        else {
            var url = 'Documents/DocumentTemplateWizard.aspx?IDdocumentTemplate=' + viewUtilsManager.getSelectedCardId();
            top.ShowExternalForm2(url, 1150, 750, '', '', false, false, false);
        }

    };

    const addNewDocumentTemplate = () => {
        var url = 'Documents/DocumentTemplateWizard.aspx?IDdocumentTemplate=-1';
        top.ShowExternalForm2(url, 1150, 750, '', '', false, false, false);
    };

    const deleteCurrentDocumentTemplate = () => {
        var _templateIsSystemSelected = viewUtilsManager.getSelectedCardIsSystem();

        if (_templateIsSystemSelected == "True") {
            this.parent.Robotics.Client.JSErrors.showJSerrorPopup(this.parent.Robotics.Client.JSErrors.JSErrorTypes.roJsInfo, '',
                { text: '', key: 'roJsInfo' }, { text: '', key: 'roDocumentaryManagementIsSystem' },
                { text: '', textkey: 'OK', desc: '', desckey: '', script: '' },
                this.parent.Robotics.Client.JSErrors.createEmptyButton(), this.parent.Robotics.Client.JSErrors.createEmptyButton(), this.parent.Robotics.Client.JSErrors.createEmptyButton());

        }
        else {
            this.parent.Robotics.Client.JSErrors.showJSerrorPopup(this.parent.Robotics.Client.JSErrors.JSErrorTypes.roJsWarning, '',
                { text: '', key: 'roJsWarning' }, { text: '', key: 'roDocumentaryManagementConfirmDelete' },
                { text: '', textkey: 'OK', desc: '', desckey: '', script: 'confirmDeleteDocumentTemplate()' },
                { text: '', textkey: 'Cancel', desc: '', desckey: '', script: '' }, this.parent.Robotics.Client.JSErrors.createEmptyButton(), this.parent.Robotics.Client.JSErrors.createEmptyButton());
        }
    };

    const confirmDeleteDocumentTemplate = () => {
        viewUtilsManager.makeServiceCall(
            "DocumentaryManagement",
            "RemoveDocumentTemplate",
            "POST",
            { templateId: viewUtilsManager.getSelectedCardId() },
            null,
            null,
            function (response) {
                //console.log(response);
                if (response.status == 200) {
                    this.parent.Robotics.Client.JSErrors.showJSerrorPopup(this.parent.Robotics.Client.JSErrors.JSErrorTypes.roJsSuccess, '',
                        { text: '', key: 'roJsSuccess' }, { text: '', key: 'roDocumentaryManagementOkDelete' },
                        { text: '', textkey: 'OK', desc: '', desckey: '', script: '' },
                        this.parent.Robotics.Client.JSErrors.createEmptyButton(), this.parent.Robotics.Client.JSErrors.createEmptyButton(), this.parent.Robotics.Client.JSErrors.createEmptyButton());
                } else {
                    this.parent.Robotics.Client.JSErrors.showJSerrorPopup(this.parent.Robotics.Client.JSErrors.JSErrorTypes.roJsError, '',
                        { text: '', key: 'roJsError' }, { text: response.statusText, key: 'roDocumentaryManagementKoDelete' },
                        { text: '', textkey: 'OK', desc: '', desckey: '', script: '' },
                        this.parent.Robotics.Client.JSErrors.createEmptyButton(), this.parent.Robotics.Client.JSErrors.createEmptyButton(), this.parent.Robotics.Client.JSErrors.createEmptyButton());
                }
                refreshCardTree();
            },
            null
        );
    }

    const filterFiles = (term) => {

    }

})();