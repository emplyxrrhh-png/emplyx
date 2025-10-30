function Settings() {
    this.login = "";
    this.password = "";
    this.MT = "";
    this.wsURL = "";
    this.language = "";
    this.saveLogin = false;
    this.isSSL = false;
    this.serverTimezone = 'Europe/Berlin';
    this.HeaderMD5 = '';
    this.BackgroundMD5 = '';
    this.timeGateBackground = '';
    this.showForbiddenSections = true;
    this.userPhoto = '';
    this.wsBackground = '';
    this.companyID = '';
    this.lastStatus = '';
    this.requestFilters = '';
    this.teleWorkingFilters = '';
    this.punchesFilters = '';
    this.authorizationFilters = '';
    this.supervisorFilters = '';
    this.supervisorFiltersDR = '';
    this.documentsFilters = '';
    this.empPermissions = '';
    this.UUID = '';
    this.ApiVersion = 0;
    this.licenseAccepted = true;
    this.onlySupervisor = false;
    this.supervisorPortalEnabled = false;

    this.isUsingAdfs = false;
    this.adfsCredential = '';

    this.rememberLogin = false;
    this.rememberLoginApp = '';
    this.isTimeGate = false;
    this.screenTimeout = 10;
    this.BackgroundMD5 = '';
    this.timeGateBackground = '';

    this.lastStatusRefresh = this.getNewStatusObject(-1);
    this.refreshConfiguration = {
        causes: { refresh: 1, interval: 1440, off: '' },
        zones: { refresh: 1, interval: 1440, off: '' },
        status: { refresh: 1, interval: 1, off: '' },
        punches: { refresh: 1, interval: 1, off: '' },
        accruals: { refresh: 1, interval: -1, off: '' },
        notifications: { refresh: 1, interval: 30, off: '' },
        dailyrecord: { refresh: 1, interval: -1, off: '' },
        communiques: { refresh: 1, interval: 60, off: '' },
        channels: { refresh: 1, interval: 60, off: '' },
        telecommute: { refresh: 1, interval: -1, off: '' },
        tcinfo: { refresh: 1, interval: -1, off: '' },
        surveys: { refresh: 1, interval: 60, off: '' },
        alertstatus: { refresh: 1, interval: 60, off: '' },
        weblinks: { refresh: 1, interval: -1, off: '' }
    };

    this.serverSSOConfig = {};
}

Settings.prototype.deleteRefreshTimestamps = function () {
    if (VTPortal.roApp.userId != this.lastStatusRefresh.userId) {
        this.lastStatusRefresh = this.getNewStatusObject(VTPortal.roApp.userId);
        return true;
    }
    return false
    //this.saveParameter('refreshStatus');
}

Settings.prototype.deleteVTPortalUserData = function () {
    if (VTPortal.roApp.userId != this.lastStatusRefresh.userId) {
        this.lastStatus = '';
        this.requestFilters = '';
        this.teleWorkingFilters = '';
        this.punchesFilters = '';
        this.authorizationFilters = '';
        this.supervisorFilters = '';
        this.supervisorFiltersDR = '';
        this.documentsFilters = '';
        this.userPhoto = '';
        this.wsBackground = '';
        this.HeaderMD5 = '';        
        return true;
    }
    return false;
}


Settings.prototype.getCacheDS = function (key) {
    if (VTPortal.roApp.impersonatedIDEmployee != -1) return null;

    if (typeof this.lastStatusRefresh[key] != 'undefined') return this.lastStatusRefresh[key].ds;

    return null;
}
Settings.prototype.updateCacheDS = function (key, sourceDS) {

    if (VTPortal.roApp.impersonatedIDEmployee != -1) return;

    if (typeof this.lastStatusRefresh[key] != 'undefined') {
        this.lastStatusRefresh[key] = {
            timestamp : moment().toDate(),
            ds: sourceDS
        };

        this.saveParameter('refreshStatus');
    }
}

Settings.prototype.markForRefresh = function (keys) {

    if (VTPortal.roApp.impersonatedIDEmployee != -1) return;

    for (var i = 0; i < keys.length; i++) {
        if (typeof this.lastStatusRefresh[keys[i]] != 'undefined') {
            this.lastStatusRefresh[keys[i]] = { timestamp: new Date(1970, 1, 1, 0, 0, 0, 0), ds: null };
        }
    }

    if (keys.find(function (element) { return element == 'status'; }) != null) {
        if (typeof this.lastStatusRefresh['alertstatus'] != 'undefined') {
            this.lastStatusRefresh['alertstatus'] = { timestamp: new Date(1970, 1, 1, 0, 0, 0, 0), ds: null };
        } 
    }

    this.saveParameter('refreshStatus');
}

Settings.prototype.resetFastPunchStatusObject = function () {

    if (VTPortal.roApp.impersonatedIDEmployee != -1) return;

    this.lastStatusRefresh.status = { timestamp: new Date(1970, 1, 1, 0, 0, 0, 0), ds: null };
    this.lastStatusRefresh.alertstatus = { timestamp: new Date(1970, 1, 1, 0, 0, 0, 0), ds: null };
    this.lastStatusRefresh.punches = { timestamp: new Date(1970, 1, 1, 0, 0, 0, 0), ds: null };
    this.lastStatusRefresh.telecommute = { timestamp: new Date(1970, 1, 1, 0, 0, 0, 0), ds: null };
    this.lastStatusRefresh.tcinfo = { timestamp: new Date(1970, 1, 1, 0, 0, 0, 0), ds: null };
}

Settings.prototype.resetAlertsStatusObject = function () {

    if (VTPortal.roApp.impersonatedIDEmployee != -1) return;

    this.lastStatusRefresh.notifications = { timestamp: new Date(1970, 1, 1, 0, 0, 0, 0), ds: null };
    this.lastStatusRefresh.dailyrecord = { timestamp: new Date(1970, 1, 1, 0, 0, 0, 0), ds: null };
    this.lastStatusRefresh.communiques = { timestamp: new Date(1970, 1, 1, 0, 0, 0, 0), ds: null };
    this.lastStatusRefresh.channels = { timestamp: new Date(1970, 1, 1, 0, 0, 0, 0), ds: null };
    this.lastStatusRefresh.surveys = { timestamp: new Date(1970, 1, 1, 0, 0, 0, 0), ds: null };
    this.lastStatusRefresh.accruals = { timestamp: new Date(1970, 1, 1, 0, 0, 0, 0), ds: null };
}

Settings.prototype.getNewStatusObject = function (userId) {
    return {
        userId: userId,
        causes: { timestamp: new Date(1970, 1, 1, 0, 0, 0, 0), ds: null },
        zones: { timestamp: new Date(1970, 1, 1, 0, 0, 0, 0), ds: null },
        status: { timestamp: new Date(1970, 1, 1, 0, 0, 0, 0), ds: null },
        punches: { timestamp: new Date(1970, 1, 1, 0, 0, 0, 0), ds: null },
        accruals: { timestamp: new Date(1970, 1, 1, 0, 0, 0, 0), ds: null },
        notifications: { timestamp: new Date(1970, 1, 1, 0, 0, 0, 0), ds: null },
        dailyrecord: { timestamp: new Date(1970, 1, 1, 0, 0, 0, 0), ds: null },
        communiques: { timestamp: new Date(1970, 1, 1, 0, 0, 0, 0), ds: null },
        channels: { timestamp: new Date(1970, 1, 1, 0, 0, 0, 0), ds: null },
        telecommute: { timestamp: new Date(1970, 1, 1, 0, 0, 0, 0), ds: null },
        tcinfo: { timestamp: new Date(1970, 1, 1, 0, 0, 0, 0), ds: null },
        surveys: { timestamp: new Date(1970, 1, 1, 0, 0, 0, 0), ds: null },
        alertstatus: { timestamp: new Date(1970, 1, 1, 0, 0, 0, 0), ds: null },
        weblinks: { timestamp: new Date(1970, 1, 1, 0, 0, 0, 0), ds: null }
    }

}

Settings.prototype.commitSettings = function () {
    var serverToken = Cookies.get('VTPortalToken');
    var adfsSecToken = (typeof serverToken != 'undefined') ? decodeURIComponent(serverToken) : '';

    VTPortal.roApp.adfsLoginInprogress = (localStorage.getItem('adfsLoginInprogress') == 'true' ? true : false);

    VTPortal.roApp.configWS(this.wsURL);
    VTPortal.roApp.mtServer(this.companyID);
    VTPortal.roApp.companyID = this.companyID;
    VTPortal.roApp.configSSL(this.isSSL);
    VTPortal.roApp.screenTimeout(this.screenTimeout);

    if (VTPortal.roApp.db.settings.isTimeGate == true) {

        VTPortal.roApp.db.settings.wsURL = this.wsURL;
        VTPortal.roApp.db.settings.isSSL = this.isSSL;
        VTPortal.roApp.db.settings.companyID = this.companyID;
        VTPortal.roApp.companyID = VTPortal.roApp.db.settings.companyID;

        VTPortal.roApp.currentLanguageId(VTPortal.roApp.db.settings.language);
        VTPortal.roApp.timegateLanguage(VTPortal.roApp.db.settings.language);
        VTPortal.roApp.setLanguage(VTPortal.roApp.getLanguageByID(VTPortal.roApp.db.settings.language).tag);

        VTPortal.roApp.isTimeGate(true);
        VTPortal.app.navigate("TimeGate", { root: true });
    } else {
        if (!VTPortal.roApp.isModeApp() && typeof adfsSecToken != 'undefined' && adfsSecToken != "") {
            Cookies.remove('VTPortalToken');
            var sTokens = adfsSecToken.decodeBase64().split('#');

            if (sTokens[1] == 'unknown') {
                if (sTokens.length > 2) {
                    VTPortal.roApp.companyID = sTokens[2];
                    VTPortal.roApp.db.settings.companyID = sTokens[2];
                }
                VTPortal.roApp.mtServer(VTPortal.roApp.companyID);
                VTPortal.roApp.ssoUnkownError(true);
                VTPortal.roApp.setLanguage(VTPortal.roApp.getLanguageByID(VTPortal.roApp.db.settings.language).tag);
                VTPortal.roApp.db.settings.save(function () { VTPortal.app.navigate("login", { root: true }); }); //REVISAR SI PONEMOS REDIRECT A TIMEGATE
            } else {
                VTPortal.roApp.guidToken = sTokens[0];
                VTPortal.roApp.securityToken = sTokens[1];

                if (sTokens.length > 2) {
                    VTPortal.roApp.companyID = sTokens[2];
                    VTPortal.roApp.db.settings.companyID = sTokens[2];
                }

                VTPortal.roApp.mtServer(this.companyID);
                VTPortal.roApp.companyID = this.companyID;

                VTPortal.roApp.adfsEnabled(true);
                VTPortal.roApp.db.settings.isUsingAdfs = true;
                VTPortal.roApp.db.settings.adfsCredential = adfsSecToken;
                VTPortal.roApp.db.settings.wsURL = VTPortal.roApp.configWS();
                VTPortal.roApp.db.settings.isSSL = VTPortal.roApp.configSSL();
                VTPortal.roApp.mtServer(VTPortal.roApp.companyID);
                VTPortal.roApp.setLanguage(VTPortal.roApp.getLanguageByID(VTPortal.roApp.db.settings.language).tag);
                VTPortal.roApp.db.settings.save(function () { VTPortal.roApp.redirectAtHome(); });
            }
        } else {
            VTPortal.roApp.userName(this.login);

            if (VTPortal.roApp.db.settings.adfsCredential == '') {
                if (!(this.rememberLogin == false && this.password == "")) {
                    VTPortal.roApp.db.settings.wsURL = this.wsURL;
                    VTPortal.roApp.db.settings.isSSL = this.isSSL;
                    VTPortal.roApp.db.settings.isUsingAdfs = this.isSSL;
                    VTPortal.roApp.db.settings.language = this.language;
                    VTPortal.roApp.db.settings.login = this.login;
                    VTPortal.roApp.db.settings.MT = this.MT;
                    VTPortal.roApp.db.settings.companyID = this.companyID;
                    VTPortal.roApp.db.settings.password = this.password;
                    VTPortal.roApp.db.settings.rememberLogin = this.rememberLogin;
                    VTPortal.roApp.db.settings.rememberLoginApp = this.rememberLoginApp;

                    VTPortal.roApp.db.settings.userPhoto = this.userPhoto;
                    VTPortal.roApp.db.settings.wsBackground = this.wsBackground;

                    VTPortal.roApp.db.settings.lastStatus = this.lastStatus;
                    VTPortal.roApp.db.settings.requestFilters = this.requestFilters;
                    VTPortal.roApp.db.settings.empPermissions = this.empPermissions;
                    VTPortal.roApp.db.settings.ApiVersion = this.ApiVersion;
                    VTPortal.roApp.db.settings.adfsCredential = this.adfsCredential;
                    VTPortal.roApp.db.settings.teleWorkingFilters = this.teleWorkingFilters;
                    VTPortal.roApp.db.settings.punchesFilters = this.punchesFilters;
                    VTPortal.roApp.db.settings.authorizationFilters = this.authorizationFilters;
                    VTPortal.roApp.db.settings.supervisorFilters = this.supervisorFilters;
                    VTPortal.roApp.db.settings.supervisorFiltersDR = this.supervisorFiltersDR;
                    VTPortal.roApp.db.settings.documentsFilters = this.documentsFilters;
                    VTPortal.roApp.db.settings.licenseAccepted = this.licenseAccepted;
                    VTPortal.roApp.db.settings.onlySupervisor = this.onlySupervisor;
                    VTPortal.roApp.db.settings.supervisorPortalEnabled = this.supervisorPortalEnabled;

                    VTPortal.roApp.db.settings.lastStatusRefresh = this.lastStatusRefresh;

                    VTPortal.roApp.db.settings.serverTimezone = this.serverTimezone;
                    VTPortal.roApp.db.settings.HeaderMD5 = this.HeaderMD5;                    
                    VTPortal.roApp.db.settings.showForbiddenSections = this.showForbiddenSections;
                    VTPortal.roApp.db.settings.showLogoutHome = this.showLogoutHome;
                    VTPortal.roApp.db.settings.isAD = this.isAD;
                    VTPortal.roApp.userPhoto(this.userPhoto);
                    VTPortal.roApp.licenseAccepted(this.licenseAccepted);
                    VTPortal.roApp.setLanguage(VTPortal.roApp.getLanguageByID(VTPortal.roApp.db.settings.language).tag);
                    VTPortal.roApp.serverTimeZone = VTPortal.roApp.db.settings.serverTimezone;
                    VTPortal.roApp.HeaderMD5 = VTPortal.roApp.db.settings.HeaderMD5;                    
                    VTPortal.roApp.ShowForbiddenSections = VTPortal.roApp.db.settings.showForbiddenSections;
                    VTPortal.roApp.showLogoutHome(VTPortal.roApp.db.settings.showLogoutHome);
                    VTPortal.roApp.isAD(VTPortal.roApp.db.settings.isAD);

                    VTPortal.roApp.db.settings.isTimeGate = this.isTimeGate;
                    VTPortal.roApp.isTimeGate(VTPortal.roApp.db.settings.isTimeGate);

                    //if (this.lastStatus != '') VTPortal.roApp.userStatus(JSON.parse(this.lastStatus.decodeBase64()));
                    if (this.requestFilters != '') VTPortal.roApp.requestFilters(JSON.parse(this.requestFilters.decodeBase64()));
                    if (this.empPermissions != '') VTPortal.roApp.empPermissions(JSON.parse(this.empPermissions.decodeBase64()));
                    if (this.teleWorkingFilters != '') VTPortal.roApp.teleWorkingFilters(JSON.parse(this.teleWorkingFilters.decodeBase64()));
                    if (this.authorizationFilters != '') VTPortal.roApp.authorizationFilters(JSON.parse(this.authorizationFilters.decodeBase64()));
                    if (this.supervisorFilters != '') VTPortal.roApp.supervisorFilters(JSON.parse(this.supervisorFilters.decodeBase64()));
                    if (this.supervisorFiltersDR != '') VTPortal.roApp.supervisorFiltersDR(JSON.parse(this.supervisorFiltersDR.decodeBase64()));
                    if (this.punchesFilters != '') VTPortal.roApp.punchesFilters(JSON.parse(this.punchesFilters.decodeBase64()));
                    if (this.documentsFilters != '') VTPortal.roApp.documentsFilters(JSON.parse(this.documentsFilters.decodeBase64()));
                }
            } else {
                var sTokens = VTPortal.roApp.db.settings.adfsCredential.decodeBase64().split('#');
                VTPortal.roApp.guidToken = sTokens[0];
                VTPortal.roApp.securityToken = sTokens[1];

                if (sTokens.length > 2) {
                    VTPortal.roApp.companyID = sTokens[2];
                }

                VTPortal.roApp.db.settings.language = this.language;
                VTPortal.roApp.setLanguage(VTPortal.roApp.getLanguageByID(VTPortal.roApp.db.settings.language).tag);
            }

            var endSaveCallback = function () {
                if (VTPortal.roApp.db.settings.adfsCredential == '') {
                    if (VTPortal.roApp.db.settings.rememberLogin == false && VTPortal.roApp.db.settings.password == "") {
                        VTPortal.app.navigate("login", { root: true });
                    } else {
                        VTPortal.roApp.redirectAtHome();
                    }
                } else {
                    VTPortal.roApp.redirectAtHome();
                }
            }

            VTPortal.roApp.db.settings.save(endSaveCallback);
        }
    }
}

Settings.prototype.loadSettings = function (db) {
    var trans = db.result.transaction("Settings", "readwrite");
    var store = trans.objectStore("Settings");
    // Get everything in the store;
    var keyRange = VTPortal.roApp.db.IDBKeyRange.lowerBound(0);
    var cursorRequest = store.openCursor(keyRange);

    cursorRequest.onsuccess = function (e) {
        var result = e.target.result;
        if (!!result == false) {
            setTimeout(function () { VTPortal.roApp.db.settings.commitSettings(); }, 200);
            return;
        }

        switch (result.value.key) {
            case 'wsURL':
                VTPortal.roApp.db.settings.wsURL = result.value.data;
                break;
            case 'ssl':
                if (result.value.data == 'true') {
                    VTPortal.roApp.db.settings.isSSL = true;
                } else {
                    VTPortal.roApp.db.settings.isSSL = false;
                }
                break;
            case 'isUsingAdfs':
                if (result.value.data == 'true') {
                    VTPortal.roApp.db.settings.isUsingAdfs = true;
                } else {
                    VTPortal.roApp.db.settings.isUsingAdfs = false;
                }
                break;
            case 'saveLogin':
                if (result.value.data == 'true') {
                    VTPortal.roApp.db.settings.saveLogin = true;
                } else {
                    VTPortal.roApp.db.settings.saveLogin = false;
                }
                break;
            case 'login':
                try {
                    VTPortal.roApp.db.settings.login = result.value.data.decodeBase64();
                } catch (e) {
                    VTPortal.roApp.db.settings.login = '';
                }
                break;
            case 'MT':
                VTPortal.roApp.db.settings.MT = (result.value.data == '' ? true : (result.value.data == 'false' ? false : true));
                localStorage.setItem('IsMT', VTPortal.roApp.db.settings.MT);
                break;
            case 'companyID':
                try {
                    VTPortal.roApp.db.settings.companyID = result.value.data.decodeBase64();
                } catch (e) {
                    VTPortal.roApp.db.settings.companyID = '';
                }
                break;
            case 'password':
                try {
                    VTPortal.roApp.db.settings.password = result.value.data.decodeBase64();
                } catch (e) {
                    VTPortal.roApp.db.settings.password = '';
                }
                break;
            case 'rememberLogin':
                if (result.value.data == 'true') {
                    VTPortal.roApp.db.settings.rememberLogin = true;
                } else {
                    VTPortal.roApp.db.settings.rememberLogin = false;
                }
                break;
            case 'rememberLoginApp':
                try {
                    VTPortal.roApp.db.settings.rememberLoginApp = result.value.data.decodeBase64();
                } catch (e) {
                    VTPortal.roApp.db.settings.rememberLoginApp = '';
                }
                break;
            case 'language':
                VTPortal.roApp.db.settings.language = result.value.data;
                break;
            case 'userPhoto':
                VTPortal.roApp.db.settings.userPhoto = result.value.data;
                break;
            case 'wsBackground':

                if (result.value.data == null || result.value.data == '') {
                    VTPortal.roApp.HasCustomHeader(false);
                    VTPortal.roApp.customBackground(null);
                }
                else {
                    try {
                        VTPortal.roApp.HasCustomHeader(true);
                        VTPortal.roApp.customBackground(JSON.parse(result.value.data));
                    } catch (e) {
                        VTPortal.roApp.HasCustomHeader(false);
                        VTPortal.roApp.customBackground(null);
                    }
                }

                VTPortal.roApp.db.settings.wsBackground = result.value.data;
                break;
            case 'lastStatus':
                VTPortal.roApp.db.settings.lastStatus = result.value.data;
                break;
            case 'requestFilters':
                VTPortal.roApp.db.settings.requestFilters = result.value.data;
                break;
            case 'empPermissions':
                VTPortal.roApp.db.settings.empPermissions = result.value.data;
                break;
            case 'ApiVersion':
                VTPortal.roApp.db.settings.ApiVersion = parseInt(result.value.data, 10);
                break;
            case 'adfsCredential':
                VTPortal.roApp.db.settings.adfsCredential = result.value.data
                break;
            case 'teleWorkingFilters':
                VTPortal.roApp.db.settings.teleWorkingFilters = result.value.data;
                break;
            case 'punchesFilters':
                VTPortal.roApp.db.settings.punchesFilters = result.value.data;
                break;
            case 'authorizationFilters':
                VTPortal.roApp.db.settings.authorizationFilters = result.value.data;
                break;
            case 'supervisorFilters':
                VTPortal.roApp.db.settings.supervisorFilters = result.value.data;
                break;
            case 'supervisorFiltersDR':
                VTPortal.roApp.db.settings.supervisorFiltersDR = result.value.data;
                break;
            case 'documentsFilters':
                VTPortal.roApp.db.settings.documentsFilters = result.value.data;
                break;
            case 'licenseAccepted':
                VTPortal.roApp.db.settings.licenseAccepted = (result.value.data == 'false' ? false : true);
                break;
            case 'serverTimezone':
                VTPortal.roApp.db.settings.serverTimezone = result.value.data;
                break;
            case 'HeaderMD5':
                VTPortal.roApp.db.settings.HeaderMD5 = result.value.data;
                break;
            case 'showForbiddenSections':
                VTPortal.roApp.db.settings.showForbiddenSections = (result.value.data == 'false' ? false : true);
                break;
            case 'showLogoutHome':
                VTPortal.roApp.db.settings.showLogoutHome = (result.value.data == 'false' ? false : true);
                break;
            case 'isAD':
                VTPortal.roApp.db.settings.isAD = (result.value.data == '' || result.value.data == 'false' ? false : true);
                break;
            case 'onlySupervisor':
                VTPortal.roApp.db.settings.onlySupervisor = (result.value.data == 'false' ? false : true);
                break;
            case 'supervisorPortalEnabled':
                VTPortal.roApp.db.settings.supervisorPortalEnabled = (result.value.data == 'false' ? false : true);
                break;
            case 'UUID':
                VTPortal.roApp.db.settings.UUID = result.value.data;
                VTPortal.roApp.guidToken = result.value.data;
                break;
            case 'refreshStatus':
                if (result.value.data != null && result.value.data != '') VTPortal.roApp.db.settings.lastStatusRefresh = JSON.parse(result.value.data.decodeBase64());
                break;
            case 'refreshConfig':
                if (result.value.data != null && result.value.data != '') VTPortal.roApp.db.settings.refreshConfiguration = JSON.parse(result.value.data.decodeBase64());
                break;
            case 'isTimeGate':
                VTPortal.roApp.db.settings.isTimeGate = (result.value.data == 'true' ? true : false);
                break;
            case 'screenTimeout':
                VTPortal.roApp.db.settings.screenTimeout = parseInt(result.value.data,10);
                break;
            case 'BackgroundMD5':
                VTPortal.roApp.db.settings.BackgroundMD5 = result.value.data;
                break;
            case 'timeGateBackground':
                VTPortal.roApp.db.settings.timeGateBackground = result.value.data;
                break;                
            
        }

        result.continue();
    };
};

Settings.prototype.save = function (onEndCallback) {
    if (typeof (VTPortal.roApp.db.indexedDB) != 'undefined' && VTPortal.roApp.db.dbObject != null) {
        var db = VTPortal.roApp.db.dbObject;

        var tx = db.result.transaction("Settings", "readwrite"); //.objectStore("Settings");

        tx.objectStore("Settings").put({ id: 1, key: "wsURL", data: this.wsURL });
        tx.objectStore("Settings").put({ id: 2, key: "ssl", data: (this.isSSL == true ? 'true' : 'false') });

        tx.objectStore("Settings").put({ id: 4, key: "login", data: this.login.encodeBase64() });
        tx.objectStore("Settings").put({ id: 5, key: "password", data: this.password.encodeBase64() });
        tx.objectStore("Settings").put({ id: 11, key: "userPhoto", data: this.userPhoto });
        tx.objectStore("Settings").put({ id: 12, key: "wsBackground", data: this.wsBackground });
        tx.objectStore("Settings").put({ id: 13, key: "lastStatus", data: '' });// this.lastStatus });
        tx.objectStore("Settings").put({ id: 14, key: "requestFilters", data: this.requestFilters });
        tx.objectStore("Settings").put({ id: 15, key: "empPermissions", data: this.empPermissions });
        tx.objectStore("Settings").put({ id: 16, key: "teleWorkingFilters", data: this.teleWorkingFilters });
        tx.objectStore("Settings").put({ id: 17, key: "language", data: this.language });
        tx.objectStore("Settings").put({ id: 18, key: "ApiVersion", data: this.ApiVersion });
        tx.objectStore("Settings").put({ id: 19, key: "documentsFilters", data: this.documentsFilters });
        tx.objectStore("Settings").put({ id: 20, key: "adfsCredential", data: this.adfsCredential });
        tx.objectStore("Settings").put({ id: 21, key: "licenseAccepted", data: this.licenseAccepted == true ? 'true' : 'false' });
        tx.objectStore("Settings").put({ id: 22, key: "punchesFilters", data: this.punchesFilters });
        tx.objectStore("Settings").put({ id: 23, key: "authorizationFilters", data: this.authorizationFilters });
        tx.objectStore("Settings").put({ id: 24, key: "serverTimezone", data: this.serverTimezone });
        tx.objectStore("Settings").put({ id: 25, key: "showForbiddenSections", data: (this.showForbiddenSections == true ? 'true' : 'false') });
        tx.objectStore("Settings").put({ id: 26, key: "showLogoutHome", data: (this.showLogoutHome == true ? 'true' : 'false') });
        tx.objectStore("Settings").put({ id: 27, key: "onlySupervisor", data: (this.onlySupervisor == true ? 'true' : 'false') });
        tx.objectStore("Settings").put({ id: 28, key: "supervisorPortalEnabled", data: (this.supervisorPortalEnabled == true ? 'true' : 'false') });
        tx.objectStore("Settings").put({ id: 29, key: "supervisorFilters", data: this.supervisorFilters });
        tx.objectStore("Settings").put({ id: 30, key: "isUsingAdfs", data: (this.isUsingAdfs == true ? 'true' : 'false') });
        tx.objectStore("Settings").put({ id: 31, key: "isAD", data: (this.isAD != '' ? 'true' : 'false') });
        tx.objectStore("Settings").put({ id: 34, key: "MT", data: this.MT });
        tx.objectStore("Settings").put({ id: 35, key: "companyID", data: this.companyID.encodeBase64() });
        tx.objectStore("Settings").put({ id: 36, key: "HeaderMD5", data: this.HeaderMD5 });        
        tx.objectStore("Settings").put({ id: 37, key: "supervisorFiltersDR", data: this.supervisorFiltersDR });
        tx.objectStore("Settings").put({ id: 38, key: "rememberLogin", data: (this.rememberLogin == true ? 'true' : 'false') });
        tx.objectStore("Settings").put({ id: 39, key: "rememberLoginApp", data: this.rememberLoginApp.encodeBase64() });

        tx.objectStore("Settings").put({ id: 40, key: "refreshStatus", data: JSON.stringify(this.lastStatusRefresh).encodeBase64() });
        tx.objectStore("Settings").put({ id: 41, key: "refreshConfig", data: JSON.stringify(this.refreshConfiguration).encodeBase64() });
        tx.objectStore("Settings").put({ id: 42, key: "isTimeGate", data: (this.isTimeGate ? 'true' : 'false') });
        tx.objectStore("Settings").put({ id: 43, key: "screenTimeout", data: this.screenTimeout }); 
        tx.objectStore("Settings").put({ id: 44, key: "BackgroundMD5", data: this.BackgroundMD5 });
        tx.objectStore("Settings").put({ id: 45, key: "timeGateBackground", data: this.timeGateBackground });
        
        

        tx.oncomplete = function (event) {
            if (VTPortal.roApp.isModeApp() && VTPortal.roApp.db.fileSystem != null) VTPortal.roApp.db.exportToJsonString(function () { });

            if (typeof onEndCallback != 'undefined') onEndCallback();
        }
    }
};

Settings.prototype.saveParameter = function (pName, callback, bSaveToDisk) {
    if (typeof (VTPortal.roApp.db.indexedDB) != 'undefined' && VTPortal.roApp.db.dbObject != null) {
        var db = VTPortal.roApp.db.dbObject;

        switch (pName) {
            case 'wsURL':
                this.updateParameters(db, 'wsURL', this.wsURL, callback);
                break;
            case 'ssl':
                this.updateParameters(db, 'ssl', (this.isSSL == true ? 'true' : 'false'), callback);
                break;
            case 'isUsingAdfs':
                this.updateParameters(db, 'isUsingAdfs', (this.isUsingAdfs == true ? 'true' : 'false'), callback);
                break;
            case 'login':
                this.updateParameters(db, 'login', this.login.encodeBase64(), callback);
                break;
            case 'MT':
                this.updateParameters(db, 'MT', this.MT, callback);
                break;
            case 'companyID':
                this.updateParameters(db, 'companyID', this.companyID.encodeBase64(), callback);
                break;
            case 'password':
                this.updateParameters(db, 'password', this.password.encodeBase64(), callback);
                break;
            case 'rememberLogin':
                this.updateParameters(db, 'rememberLogin', (this.isSSL == true ? 'true' : 'false'), callback);
                break;
            case 'password':
                this.updateParameters(db, 'rememberLoginApp', this.rememberLoginApp.encodeBase64(), callback);
                break;
            case 'language':
                this.updateParameters(db, 'language', this.language, callback);
                break;
            case 'userPhoto':
                this.updateParameters(db, 'userPhoto', this.userPhoto, callback);
                break;
            case 'wsBackground':
                this.updateParameters(db, 'wsBackground', this.wsBackground, callback);
                break;
            case 'lastStatus':
                this.updateParameters(db, 'lastStatus', this.lastStatus, callback);
                break;
            case 'requestFilters':
                this.updateParameters(db, 'requestFilters', this.requestFilters, callback);
                break;
            case 'empPermissions':
                this.updateParameters(db, 'empPermissions', this.empPermissions, callback);
                break;
            case 'ApiVersion':
                this.updateParameters(db, 'ApiVersion', this.ApiVersion, callback);
                break;
            case 'adfsCredential':
                this.updateParameters(db, 'adfsCredential', this.adfsCredential, callback);
                break;
            case 'teleWorkingFilters':
                this.updateParameters(db, 'teleWorkingFilters', this.teleWorkingFilters, callback);
                break;
            case 'punchesFilters':
                this.updateParameters(db, 'punchesFilters', this.punchesFilters, callback);
                break;
            case 'authorizationFilters':
                this.updateParameters(db, 'authorizationFilters', this.authorizationFilters, callback);
                break;
            case 'supervisorFilters':
                this.updateParameters(db, 'supervisorFilters', this.supervisorFilters, callback);
                break;
            case 'supervisorFiltersDR':
                this.updateParameters(db, 'supervisorFiltersDR', this.supervisorFiltersDR, callback);
                break;
            case 'documentsFilters':
                this.updateParameters(db, 'documentsFilters', this.documentsFilters, callback);
                break;
            case 'licenseAccepted':
                this.updateParameters(db, 'licenseAccepted', this.licenseAccepted, callback);
                VTPortal.roApp.licenseAccepted(this.licenseAccepted);
                break;
            case 'serverTimezone':
                this.updateParameters(db, 'serverTimezone', this.serverTimezone, callback);
                break;
            case 'HeaderMD5':
                this.updateParameters(db, 'HeaderMD5', this.HeaderMD5, callback);
                break;
            case 'showForbiddenSections':
                this.updateParameters(db, 'showForbiddenSections', this.showForbiddenSections, callback);
                break;
            case 'showLogoutHome':
                this.updateParameters(db, 'showLogoutHome', this.showLogoutHome, callback);
                break;
            case 'isAD':
                this.updateParameters(db, 'isAD', this.isAD, callback);
                break;
            case 'onlySupervisor':
                this.updateParameters(db, 'onlySupervisor', this.onlySupervisor, callback);
                break;
            case 'refreshStatus':
                this.updateParameters(db, 'refreshStatus', JSON.stringify(this.lastStatusRefresh).encodeBase64(), callback);
                break;
            case 'refreshConfig':
                this.updateParameters(db, 'refreshConfig', JSON.stringify(this.refreshConfiguration).encodeBase64(), callback);
                break;
            case 'isTimeGate':
                this.updateParameters(db, 'isTimeGate', (this.isTimeGate ? 'true' : 'false'), callback);
                break;
            case 'screenTimeout':
                this.updateParameters(db, 'screenTimeout', this.screenTimeout, callback);
                break;
            case 'BackgroundMD5':
                this.updateParameters(db, 'BackgroundMD5', this.BackgroundMD5, callback);
                break;
            case 'timeGateBackground':
                this.updateParameters(db, 'timeGateBackground', this.timeGateBackground, callback);
                break;                
        }
    }
};

Settings.prototype.updateParameters = function (db, key, value, callback) {
    var objectStore = db.result.transaction("Settings", "readwrite").objectStore("Settings");
    var request = objectStore.get(key);
    var onEndUpdate = callback;

    request.onerror = function (event) {
        // Handle errors!
    };

    request.onsuccess = function (event) {
        // Get the old value that we want to update
        var data = event.target.result;

        if (!!data == false) {
            return;
        }

        // update the value(s) in the object that you want to change
        data.data = value;

        // Put this updated object back into the database.
        var requestUpdate = objectStore.put(data);
        requestUpdate.onerror = function (event) {
            // Do something with the error
        };
        requestUpdate.onsuccess = function (event) {
            // Success - the data is updated!
            if (typeof onEndUpdate != 'undefined') onEndUpdate();
        };
    };
};