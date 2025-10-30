VTPortal.sharedPortalConfig = function (params) {
    let modelIsReady = ko.observable(false);        
    let txtTerminalName = ko.observable('');    
    let isTimegateEnabled = ko.observable(true);  
    const updateSharedPortalStatus = function (timeGate) {
        let lastValue = VTPortal.roApp.db.settings.isTimeGate;

        VTPortal.roApp.db.settings.isTimeGate = (timeGate != null);
        isTimegateEnabled(VTPortal.roApp.db.settings.isTimeGate);

        if (isTimegateEnabled()) {
            VTPortal.roApp.db.settings.screenTimeout = timeGate.ScreenTimeout
            VTPortal.roApp.db.settings.language = timeGate.Language;
            VTPortal.roApp.isTimeGate(true);
            VTPortal.roApp.screenTimeout(timeGate.ScreenTimeout);
            VTPortal.roApp.timegateLanguage(timeGate.Language);
        } else {
            VTPortal.roApp.isTimeGate(false);
            VTPortal.roApp.screenTimeout(10);
            VTPortal.roApp.timegateLanguage(VTPortal.roApp.currentLanguageId());
        }

        VTPortal.roApp.db.settings.save(
            function () {
                if (lastValue != VTPortal.roApp.isTimeGate() && VTPortal.roApp.isTimeGate()) {
                    VTPortal.app.navigate("TimeGate", { root: true });
                }
            }
        );
    }

    let btnEnableSharedPortal = function (fParam) {                
        let serialNumber = VTPortal.roApp.db.settings.UUID;
        if (serialNumber != -1) {            
            new WebServiceRobotics(function (result) {
                if (result.Status == window.VTPortalUtils.constants.OK.value) {                                                            
                    VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t('roOkEnablingSharedPortal'), 'success', 3500);

                    //Marcamos los parametros necesarios para el cierre de sesión
                    VTPortalUtils.utils.onLogoutResetParameters();
                    VTPortal.roApp.userId = -999;
                    VTPortal.roApp.db.settings.deleteRefreshTimestamps();

                    //Marcamos los parametros necesarios para activar el Time Gate
                    VTPortal.roApp.db.settings.isTimeGate = true;
                    isTimegateEnabled(VTPortal.roApp.db.settings.isTimeGate);
                    VTPortal.roApp.isTimeGate(true);
                    VTPortal.roApp.screenTimeout(result.Value.ScreenTimeout);
                    VTPortal.roApp.timegateLanguage(result.Value.Language);

                    //Redirect to TimeGate
                    VTPortal.roApp.db.settings.save( function () { VTPortal.app.navigate("TimeGate", { root: true }); } );
                } else {
                    VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t(window.VTPortalUtils.utils.languageTag(result.Status)), 'error', 0, function () { });
                }
            }).enableTimegate(serialNumber, txtTerminalName(), VTPortal.roApp.infoVersion);
        }        
    }    

    let btnDisableSharedPortal = function (fParam) {        
        let serialNumber = VTPortal.roApp.db.settings.UUID;
        if (serialNumber != -1) {            
            new WebServiceRobotics(function (result) {
                if (result.Status == window.VTPortalUtils.constants.OK.value) {
                    VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t('roOkDisablingSharedPortal'), 'success', 2500);
                    txtTerminalName("");

                    //Marcamos los parametros necesarios para el cierre de sesión
                    VTPortalUtils.utils.onLogoutResetParameters();
                    VTPortal.roApp.userId = -999;
                    VTPortal.roApp.db.settings.deleteRefreshTimestamps();

                    //Marcamos los parametros necesarios para activar el Time Gate
                    VTPortal.roApp.db.settings.isTimeGate = false;
                    isTimegateEnabled(VTPortal.roApp.db.settings.isTimeGate);
                    VTPortal.roApp.isTimeGate(false);
                    VTPortal.roApp.screenTimeout(10);

                    //Redirect to TimeGate
                    VTPortal.roApp.db.settings.save(function () { VTPortal.app.navigate("login", { root: true }); });
                } else {
                    VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t(window.VTPortalUtils.utils.languageTag(result.Status)), 'error', 0, function () { });
                }
            }).disableTimegate(serialNumber);
        }
    }

    let getTimegateConfiguration = function () {
        let serialNumber = VTPortal.roApp.db.settings.UUID;
        new WebServiceRobotics(function (result) {
            if ((result.Status == window.VTPortalUtils.constants.OK.value) || (result.Status == window.VTPortalUtils.constants.NOT_FOUND.value)) {
                if (result.Value != null) {
                    txtTerminalName(result.Value.Name);
                    updateSharedPortalStatus(result.Value);
                }
                else {
                    updateSharedPortalStatus(null);
                }
            } else {
                updateSharedPortalStatus(null);
                VTPortalUtils.utils.processErrorMessage(result, function () { });
            }
        }, function (error) {
            VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roErrorLoadingSummaryHolidaysInfo"), 'error', 0, function () { });
        }).getTimegateConfiguration(serialNumber);
    }


    function viewShown(selDate) {
        getTimegateConfiguration();
    };


    let viewModel = {
        modelIsReady: modelIsReady,        
        viewShown: viewShown,        
        lblTerminalName: i18nextko.t('lblTerminalName'),
        lblTimegateTitle: i18nextko.t('lblTimegateTitle'),
        isTimegateEnabled: isTimegateEnabled,        
        btnEnabled: {
            text: i18nextko.t('roDisableSharedPortalBt'),
            onClick: btnDisableSharedPortal            
        },
        btnEnableSharedPortal: {
            onClick: btnEnableSharedPortal,
            text: i18nextko.t('roEnableSharedPortalBt'),
        },          
        txtTerminalName: {
            placeholder: i18nextko.t('roPH_terminalName'),
            value: txtTerminalName,
            disabled: isTimegateEnabled
        },        
        loadingPanel: VTPortalUtils.utils.loadingPanelConf(),
        lblTimegateActionWarning: i18nextko.t('lblTimegateActionWarning'),
    };

    return viewModel;
};