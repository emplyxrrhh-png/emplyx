VTPortal.TimeGate = function (params) {

    const status = ko.observable(null); //No mostramos formulario hasta que se conecte por primera vez
    const conectado = ko.computed(function () {
        return status() == 0 || status() == null;
    });
    let isTimeGateDisabled = false;
    let invalidResponseAttempts = 0;
    let timegateNFCEnabled = ko.observable(false);
    let showBackButton = ko.observable(false);        

    const loginID = ko.observable("");
    const loginPIN = ko.observable("");
    const loginNFC = ko.observable("");
    const actualTime = ko.observable(moment().format('HH:mm'));
    const actualDate = ko.observable(moment().format('dddd D MMM').replace('.', ''));

    const enableTextBox = ko.observable(false); //recogerlo del servidor
    var customCssBack = ko.observable('');
    var customPhotoBack = ko.observable('');

    const objInputID = ko.computed(function () {
        let propertiesID = {
            placeholder: i18nextko.i18n.t('idPlaceHolder'),
                value: loginID,
                    inputAttr: {
                autocomplete: "newID"
            }
        };

        if (!enableTextBox()) {
            propertiesID.pattern = "[0-9]*";
            propertiesID.inputAttr.inputmode = "numeric";
        }

        return propertiesID;
    });

    function isIOS() {
        const userAgent = window.navigator.userAgent;
        return VTPortal.roApp.isModeApp() && /iPad|iPhone|iPod/.test(userAgent) && !window.MSStream;
    }
    function isAndroid() {
        const userAgent = window.navigator.userAgent;
        return VTPortal.roApp.isModeApp() && /Android/.test(userAgent) && !window.MSStream;
    }


    let initializeTimegateurl = async function () {

        if (!VTPortal.roApp.serverURL.valid) {
            let urlValidator = new Robotics.Client.ValidateURL();
            VTPortal.roApp.serverURL = await urlValidator.getURLforClient(VTPortal.roApp.db.settings.companyID, VTPortal.roApp.db.settings.wsURL, VTPortal.roApp.isModeApp());
        }

        if (VTPortal.roApp.serverURL.valid) {
            let timeGateURL = window.VTPortal.roApp.getServerUrl(VTPortal.roApp.serverURL.url).toLowerCase();

            setTimeout(function () {
                VTPortalUtils.utils.logoutTimegate();
                if (VTPortal.roApp.timeGateTimmerId == -1) checkServerStatus();

                //Activamos NFC
                VTPortal.roApp.nfcTimegateInProgress(false);
                if (VTPortal.roApp.isModeApp()) {
                    if (typeof nfc != 'undefined') {
                        if (isIOS()) {
                            $("#nfc-container").show();
                        }
                        else {
                                startNFCListener();
                            }
                        }
                    }
            }, 200);


        } else {
            status(-4);
            setTimeout(function () { initializeTimegateurl() }, 60000);
            //} else {
            //    VTPortal.roApp.db.settings.isTimeGate = false;
            //    VTPortal.roApp.isTimeGate(false);
            //    VTPortal.roApp.db.settings.save(function () {
            //        VTPortal.app.navigate("login", { root: true });
            //    });
            //}

        }
    }

    let checkServerStatus = function () {
        VTPortal.roApp.timeGateTimmerId = -1;
        new WebServiceRobotics(function (result) {
            if (typeof result.Status != 'undefined' && (result.Status == 0 || result.Status == -3)) {
                showBackButton(false);
                invalidResponseAttempts = 0;

                status(result.Status);
                let restartTimmer = false;

                if (result.Status == 0) {
                    VTPortal.roApp.timeGateConfig = result.Value;

                    if (VTPortal.roApp.timeGateConfig.BackgroundMD5 != null && VTPortal.roApp.timeGateConfig.BackgroundMD5 != '') {                        
                        if (VTPortal.roApp.timeGateConfig.BackgroundMD5 != VTPortal.roApp.db.settings.BackgroundMD5) {
                            VTPortal.roApp.db.settings.BackgroundMD5 = VTPortal.roApp.timeGateConfig.BackgroundMD5;
                            VTPortal.roApp.db.settings.save(function () { });        
                            updateBackgroundConfiguration();                            
                        }
                        else
                        changeBackground(VTPortal.roApp.db.settings.timeGateBackground);
                    }

                    enableTextBox(VTPortal.roApp.timeGateConfig.IDmode == 0);

                    let needToSave = ((VTPortal.roApp.db.settings.screenTimeout != VTPortal.roApp.timeGateConfig.ScreenTimeout) || VTPortal.roApp.db.settings.language != VTPortal.roApp.timeGateConfig.Language);

                    VTPortal.roApp.db.settings.language = VTPortal.roApp.timeGateConfig.Language;
                    VTPortal.roApp.db.settings.screenTimeout = VTPortal.roApp.timeGateConfig.ScreenTimeout;

                    VTPortal.roApp.timegateLanguage(VTPortal.roApp.timeGateConfig.Language);
                    VTPortal.roApp.screenTimeout(VTPortal.roApp.db.settings.screenTimeout);

                    if (needToSave) {
                        VTPortal.roApp.db.settings.save(function () { });
                    }

                    restartTimmer = true;

                }else if (result.Status == -3) {
                    //result.status = -3 -> NotFound, el terminal no existe, desactivamos el TG de BD, y redirect al login //cualquier otro estado, es desconectado.
                    disableTimeGate();
                } 

                if (restartTimmer) VTPortal.roApp.timeGateTimmerId = setTimeout(checkServerStatus, 480000); // 8 min

            } else {
                status(-4);
                invalidResponseAttempts += 1;
                //if (invalidResponseAttempts == 1) showBackButton(true);
                if (VTPortal.roApp.isTimeGate()) VTPortal.roApp.timeGateTimmerId = setTimeout(checkServerStatus, 10000); // 8 min
            }

        }, function (error) {
            //Si entra aquí mostramos el desconectado
            status(-4);
            invalidResponseAttempts += 1;

            //if (invalidResponseAttempts == 1) showBackButton(true);
            if (VTPortal.roApp.isTimeGate()) VTPortal.roApp.timeGateTimmerId = setTimeout(checkServerStatus, 10000); // 8 min
            
        }).initializeTimeGate();

    }

    let updateBackgroundConfiguration = function () {        
        new WebServiceRobotics(function (result) {
            if (typeof result.Status != 'undefined' && (result.Status == 0 || result.Status == -3)) {
                status(result.Status);                

                if (result.Status == 0) {
                    VTPortal.roApp.timeGateBackground = result.Value;
                    VTPortal.roApp.db.settings.timeGateBackground = VTPortal.roApp.timeGateBackground;
                    VTPortal.roApp.db.settings.save(function () { });    
                    changeBackground(VTPortal.roApp.db.settings.timeGateBackground);                    

                } else if (result.Status == -3) {                    
                    disableTimeGate();
                }                

            } else {
                status(-4);
                invalidResponseAttempts += 1;
                //if (invalidResponseAttempts == 1) showBackButton(true);
                if (VTPortal.roApp.isTimeGate()) VTPortal.roApp.timeGateTimmerId = setTimeout(checkServerStatus, 10000); // 8 min
            }

        }, function (error) {
            //Si entra aquí mostramos el desconectado
            status(-4);
            invalidResponseAttempts += 1;

            //if (invalidResponseAttempts == 1) showBackButton(true);
            if (VTPortal.roApp.isTimeGate()) VTPortal.roApp.timeGateTimmerId = setTimeout(checkServerStatus, 10000); // 8 min

        }).getTimeGateBackground();

    }

    let onViewShown = function () {
        showBackButton(false);
        invalidResponseAttempts = 0;
        VTPortal.roApp.clientTimeZone = moment.tz.guess(true);
        if (isTimeGateDisabled) return;

        setInterval(() => { actualTime(moment().format('HH:mm')); actualDate(moment().format('dddd D MMM').replace('.', '')); }, 1000);

        initializeTimegateurl();
    }

    function tagDetected(nfcEvent) {
        try {
            if (VTPortalUtils.utils.isOnView('TimeGate')) {
                if (typeof nfcEvent.tag.ndefMessage[0] != 'undefined') {
                    if (!VTPortal.roApp.nfcTimegateInProgress()) {
                        VTPortal.roApp.nfcTimegateInProgress(true);

                        let onEndPunchCallback = function () {
                            if (isIOS()) {
                                nfc.invalidateSession();
                            }
                            setTimeout(function () {
                                VTPortal.roApp.nfcTimegateInProgress(false);
                            }, 3000);
                        }

                        let tagReaded = ndef.textHelper.decodePayload(nfcEvent.tag.ndefMessage[0].payload);
                        loginNFC(tagReaded);
                        loginPIN(''); //Si detectamos NFC, eliminamos información de los inputs para que llegue fichaje NFC al server
                        loginID('');
                        identify(onEndPunchCallback);
                    }
                }
                else {
                    loginNFC('');
                    VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t('roInvalidNFC'), 'warning', 2000);
                }
            }
        } catch (e) {
            VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t('roErrorReadingNFC') + ": " + e.message, 'error', 2000);
        }
    };

    function startNFCListener() {
        nfc.addNdefListener(
            tagDetected,
            function () {
                timegateNFCEnabled(true);
            },
            function () {
                timegateNFCEnabled(false);
                VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t('roErrorNFC'), 'warning', 2000);
            }
        );
    };

    function errorIos(error) {
        VTPortal.roApp.nfcEnabled(false);
        VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t('roErrorNFC'), 'warning', 2000);
    };

    function nfcIosEnabled() {
        if (typeof nfc != 'undefined') {
            if (isIOS()) {
                nfc.beginSession(startNFCListener, errorIos);
            }
        }
    }    

    function disableTimeGate() {
        isTimeGateDisabled = true;
        VTPortal.roApp.db.settings.isTimeGate = false;
        VTPortal.roApp.isTimeGate(false);
        VTPortal.roApp.timeGateMode(null);
        VTPortal.roApp.db.settings.save(function () {
            VTPortal.app.navigate("login", { root: true });
        });
        
    }

    function identify(onEndCallback) {
        clearTimeout(VTPortal.roApp.timeGateTimmerId);
        VTPortal.roApp.timeGateTimmerId = -1;
        clearTimeout(VTPortal.roApp.timeGateTimmerResponse);
        VTPortal.roApp.timeGateTimmerResponse = -1;

        new WebServiceRobotics(function (result) {
            //result.Status => 0,-3,-4 etc
            VTPortal.roApp.timeGateMode(result.TimeGateMode);
            if (result.Status == 0 && result.TimeGateMode == VTPortal.roApp.timeGateModeEIP && result.LoginInfo.Status == 0) {
                if (VTPortal.roApp.wsRequestCounter() == 1) { 
                    VTPortal.roApp.lastRequestFailed(false);
                    VTPortal.roApp.offlineCounter(0);
                    VTPortal.roApp.isOnline(true);
                }

                VTPortal.roApp.lastTimegateAction = moment();
                VTPortal.roApp.timegateLoggedIn = true;

                if (VTPortal.roApp.timegateLanguage() != result.LoginInfo.DefaultLanguage) {
                    VTPortal.roApp.currentLanguageId(result.LoginInfo.DefaultLanguage);
                    VTPortal.roApp.setLanguage(VTPortal.roApp.getLanguageByID(result.LoginInfo.DefaultLanguage).tag);
                    setTimeout(function () {
                        if (VTPortal.roApp.isTimeGate()) VTPortal.roApp.checkAutomaticLogout();
                        VTPortalUtils.utils.onLoginCommitFunc(result.LoginInfo, true);
                    }, 500);


                } else {
                    if (VTPortal.roApp.isTimeGate()) VTPortal.roApp.checkAutomaticLogout();
                    VTPortalUtils.utils.onLoginCommitFunc(result.LoginInfo, true);
                }

                //Ocultamos el reloj, porque va a acceder al portal y en algunas ocasiones se duplica por el cambio de idioma (revisar esto)
                $('.top-section').hide();                

            } else if (result.Status == 0 && result.TimeGateMode == VTPortal.roApp.timeGateModeTA) {
                let message = moment.tz(result.PunchInfo.Display.DateTimeValue, VTPortal.roApp.serverTimeZone).format('LTS') + "</br>" + result.PunchInfo.Display.UserInfo + "</br>" + result.PunchInfo.Display.StringValue

                if (result.PunchInfo.Punch.ActualType === 1) {
                    $('#response').addClass('inPunch').removeClass('outPunch error').html(message);
                } else if (result.PunchInfo.Punch.ActualType === 2) {
                    $('#response').addClass('outPunch').removeClass('inPunch error').html(message);
                }

                $('#response').show();
                checkServerStatus();
            } else if (result.Status == 0 && result.TimeGateMode == VTPortal.roApp.timeGateModeCO) {
                let stringValue = result.PunchInfo.Display.StringValue;
                let availableCostCenters = result.PunchInfo.EmployeeStatus.CostsStatus.AvailableCostCenters;
                let costCenterName = availableCostCenters && availableCostCenters.length > 0
                    ? availableCostCenters[0].Name
                    : "";            
                let message = stringValue + " <strong>" + costCenterName + "</strong>";
                $('#response').addClass('coPunch').removeClass('inPunch outPunch error').html(message);

                $('#response').show();
                checkServerStatus();

            } else {
                let errorMessage = i18nextko.i18n.t('notRecognized');
                if (result?.TimeGateMode == VTPortal.roApp.timeGateModeEIP && result?.LoginInfo?.Status == -73) {
                    errorMessage = i18nextko.i18n.t('permissionDenied');
                }
                if (result?.TimeGateMode == VTPortal.roApp.timeGateModeCO && result?.Status == -15) {
                    errorMessage = i18nextko.i18n.t('roshouldbeinforthisaction');
                }
                $('#response').addClass('error').removeClass('inPunch outPunch').html(errorMessage);
                $('#response').show();
                checkServerStatus();
            }

            // Ocultamos el formulario y mostramos respuesta
            $('#form-container').hide();
            $('#nfc-container').hide();

            const displayDuration = isIOS() ? 6000 : 3000; // Para iOS esperamos 6 segundos por el NFC

            // Mostramos el formulario después de X segundos
            VTPortal.roApp.timeGateTimmerResponse = setTimeout(resetForm, displayDuration);
            //-------------------------------------------------------------------

            document.getElementById('timeGate').addEventListener('click', handleClick);


            if (typeof onEndCallback == 'function') onEndCallback();

        }, function (error) {

            if (typeof onEndCallback == 'function') onEndCallback();
            VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t('serverLostConnexion'), 'error', 2000);

            //Si entra aquí mostramos el desconectado
            status(-4);
            if (VTPortal.roApp.isTimeGate()) VTPortal.roApp.timeGateTimmerId = setTimeout(checkServerStatus, 10000); // 8 min

        }).identify(loginID(), loginPIN(), loginNFC()) //si no existe el nfc, el backend hace caso del id+`pin  si existe nfc solo mira esta cadena
    }

    //Cuando se muestra la respuesta, reseteamos el form al hacer click en la pantalla
    function handleClick() {
        resetForm(); // Vaciamos los inputs
    }

    function clickForm() {
        const trimmedLoginID = String(loginID()).trim();
        const trimmedLoginPIN = String(loginPIN()).trim();

        if (trimmedLoginID == "" || trimmedLoginPIN == "" || (!enableTextBox() && trimmedLoginID == '0')) {
            VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t('invalidCredentials'), 'error', 2500);
        } else {
            loginID(trimmedLoginID);
            loginPIN(trimmedLoginPIN);
            identify();
        }
    }

    function changeBackground(backgroundConfig) {
        var objBackgroundConfig = JSON.parse(backgroundConfig);
        customCssBack(setCustomColor(objBackgroundConfig.LeftColor, objBackgroundConfig.RightColor));
        customPhotoBack(setCustomImage(objBackgroundConfig.Image, objBackgroundConfig.Position, objBackgroundConfig.Opacity));
    }    

    function setCustomColor(colorLeft, colorRight) {        
            if (colorLeft == null || colorLeft == "") {
                colorLeft = "#0046FE";
            }
            if (colorRight == null || colorRight == "") {
                colorRight = "#286EFF";
            }
        
        return 'background: linear-gradient(to right, ' + colorLeft + ', ' + colorRight + ');background-size: contain !important; height: 100%;';
    }

    function setCustomImage(image, position, opacity) {
        // var elF = document.getElementById('customPhoto');
        var tOpacity = opacity / 10;

        if (image == "") {
            //image = "../2/Images/path2994-7-7-6.png";
        }
        var tFileContent = image;

        
            
        return 'background: url(' + tFileContent + ') no-repeat top left; background-size: cover !important;height:100%;width:100%;opacity:' + tOpacity + '';                            
    }    

    function resetForm() {
        clearTimeout(VTPortal.roApp.timeGateTimmerResponse); // Cancelamos el timeout

        loginNFC('');
        loginID('');
        loginPIN('');

        //Ocultamos el bloque de respuesta
        $('#response').hide();
        //Mostramos el formulario
        $('#form-container').show();
        if (isIOS()) { $('#nfc-container').show(); }
        //Limpiamos los inputs
        if (document.getElementById('form') != null) document.getElementById('form').reset();
        $('#form #accessPwd input').focus();
        $('#form #accessId input').focus();

        document.getElementById('timeGate').removeEventListener('click', handleClick); //Eliminamos el evento de clic
    }

    let viewModel = {
        viewShown: onViewShown,
        lblServerTime: actualTime, 
        lblServerDate: actualDate, 
        btnSend: {
            onClick: clickForm,
            text: i18nextko.i18n.t('sendMsg'),

        },
        btnNFC: {
            onClick: nfcIosEnabled,
            text: i18nextko.i18n.t('activateNFC'),
            icon: "Images/Actions/nfc.png"
        },
        nfcAndroid: {
            onClick: () => { },
            text: "",
            icon: "Images/Actions/nfc.png"
        },
        backButtonVT: {
            onClick: () => {
                disableTimeGate()
            },
            text: "",
            icon: "Images/Actions/exit.png"
        },
        isAndroid: isAndroid(),
        timegateNFCEnabled: timegateNFCEnabled,
        showBackButton: showBackButton,
        status: status,
        conectado: conectado,
        txtID: objInputID,
        txtPIN: {
            placeholder: i18nextko.i18n.t('pinPlaceHolder'),
            value: loginPIN,
            inputAttr: {
                autocomplete: "newPIN",
                inputmode: "numeric"
            },
            pattern: "[0-9]*"
        },
        lblNotAvailable: i18nextko.i18n.t('notAvailable'),
        enableTextBox: enableTextBox,
        customCssBackground: customCssBack,
        customPhotoBackground: customPhotoBack
        
    };

    return viewModel;
};