//*******************************************************************************************
// ** Author: A. Avellaneda.
// ** Date: 10/02/2022.
// ** Description: javascript file for Supervisors module.
//*******************************************************************************************


var supervisorFrame = null;
var contentLoaded = false;
let selectedID = null;

(function () {
    //Properties
    var viewHandler = null;

    $(document).ready(async function () {
        //console.log("Carga script supervisors");

        // -----------------------------
        // GET DATA VIEW (ON READY) ----
        //------------------------------
        viewUtilsManager.initAccordions();
        viewUtilsManager.setupCardListFilterButton("Supervisors");
        viewUtilsManager.setBarButtonCallback(supervisorsChangeTab);
        viewUtilsManager.print("Supervisors Module loaded");
        //Initialize ViewHandler
        viewHandler = viewUtilsManager.createViewStateHandler();
        supervisorFrame = window.frames["ifSupervisorsContent"].contentWindow;

        //Set public functions
        window.loadRequest = loadSupervisor;
        window.supervisorsChangeTab = supervisorsChangeTab;
        window.continueLoading = continueLoading;
        window.refreshCardTree = viewUtilsManager.refreshCardTree;
        window.showLoadingGrid = viewUtilsManager.showLoadingGrid;

        window.RefreshScreen = RefreshScreen;
        
        //Register events
        document.addEventListener("startStateEvent", (data) => viewHandlerEvent(data), false);
        viewHandler.transition(viewHandler.value, "read");

        //Wait for carview load and 
        viewUtilsManager.loadViewOptions("Supervisors", "read", function () {
            //$("#ifSupervisorsContent").ready("continueLoading");

            $('.ButtonBarVertical.btnMaximize2').on('click', function (id) {
                MaxMinimize();
            });

            $('.ButtonBarVertical.btnTbAdd2').on('click', function (id) {
                supervisorFrame.ShowNewPassportWizard(id);
            });

            $('.ButtonBarVertical.btnTbDel2').on('click', function (id) {
                if (contentLoaded && supervisorFrame != null) {
                    selectedID && supervisorFrame.ShowRemovePassport(selectedID);
                }
            });

            $('.ButtonBarVertical.btnTbCopySupervisor').on('click', function () {
                supervisorFrame.copySupervisors();
            });

            $('.ButtonBarVertical.btnTbCurrent3').on('click', function () {
                supervisorFrame.ShowCurrentLoggedUsers();
            });

        }, () => { }, 'LiveSecurity');
    });

    const continueLoading = () => {
        contentLoaded = true;
        supervisorsChangeTab(0);
    }

    const loadSupervisor = () => {
        if (contentLoaded) {
            const pageTitle = $('.ScreenIconTitleGeneric h1');
            const pageImg = $('#divScreenTabsGeneric .ScreenIconTitleGeneric img');
            const selectedName = $('.CardsTree-cardClicked .cardsTree-CardInfo .cardName').text();
            const selectedImg = $('.CardsTree-cardClicked .cardsTree-CardIcon').css('background-image').replace(/^url\(['"](.+)['"]\)/, '$1');

            //actualizamos info de la cabecera de la página al seleccionar supervisor de la lista
            $(pageTitle).text(selectedName);
            $(pageTitle).css('text-transform', 'capitalize');
            $(pageImg).attr('src', selectedImg);

            if (contentLoaded && supervisorFrame != null) {
                selectedID = parseInt(viewUtilsManager.getSelectedCardId(), 10);
                supervisorFrame.cargaPassport(selectedID);
            }
        }
    }

    const RefreshScreen = (id) => {
        //supervisorFrame.refreshParentCardTree(id); //TODO: pasar el id del recién creado cuando lo obtengamos, el que viene ahora es el del passport que lo ha lanzado
        location.reload();
    }

    function supervisorsChangeTab(tabId) {
        if (contentLoaded && supervisorFrame != null) {
            supervisorFrame.changeTabs(tabId);
        }
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

})();