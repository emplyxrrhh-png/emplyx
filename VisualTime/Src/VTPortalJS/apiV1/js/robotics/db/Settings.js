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
    this.documentsFilters = '';
    this.empPermissions = '';
    this.UUID = '';
    this.ApiVersion = 0;
    this.licenseAccepted = true;
    this.onlySupervisor = false;
    this.supervisorPortalEnabled = false;

    this.isUsingAdfs = false;
    this.adfsCredential = '';

    //this.AnywhereLicense = false;
    //this.RequiereFineLocation = true;
}

Settings.prototype.commitSettings = function () {
    var serverToken = Cookies.get('VTPortalToken');

    var adfsSecToken = (typeof serverToken != 'undefined') ? decodeURIComponent(serverToken) : '';
    var vtLiveAPIUrl = Cookies.get('VTPortalApi');

    VTPortal.roApp.adfsLoginInprogress = (localStorage.getItem('adfsLoginInprogress') == 'true' ? true : false);

    VTPortal.roApp.isMT((typeof Cookies.get('IsMT') == 'undefined') ? "false" : Cookies.get('IsMT'));

    if (vtLiveAPIUrl != null && vtLiveAPIUrl != '') {
        vtLiveAPIUrl = decodeURIComponent(vtLiveAPIUrl);

        var serverUrl = vtLiveAPIUrl.replace('https://', '').replace('http://', '');
        serverUrl = serverUrl.substr(0, serverUrl.indexOf('/'));
        VTPortal.roApp.configWS(serverUrl);
        VTPortal.roApp.mtServer(this.companyID);
        this.wsURL = VTPortal.roApp.configWS();

        if (vtLiveAPIUrl.toLowerCase().indexOf('https://') == -1) VTPortal.roApp.configSSL(false);
        else VTPortal.roApp.configSSL(true);
        this.isSSL = VTPortal.roApp.configSSL();
    } else {
        if (!VTPortal.roApp.isModeApp() && typeof adfsSecToken != 'undefined' && adfsSecToken != "") {
            var serverUrl = window.location.href.toLowerCase().replace('https://', '').replace('http://', '');
            serverUrl = serverUrl.substr(0, serverUrl.indexOf('/'));
            VTPortal.roApp.configWS(serverUrl);
            VTPortal.roApp.mtServer(this.companyID);
            if (window.location.href.toLowerCase().indexOf('https://') == -1) VTPortal.roApp.configSSL(false);
            else VTPortal.roApp.configSSL(true);
        } else {
            VTPortal.roApp.configWS(this.wsURL);
            VTPortal.roApp.mtServer(this.companyID);
            VTPortal.roApp.configSSL(this.isSSL);
        }
    }

    if (!VTPortal.roApp.isModeApp() && typeof adfsSecToken != 'undefined' && adfsSecToken != "") {
        var sTokens = adfsSecToken.decodeBase64().split('#');

        VTPortal.roApp.guidToken = sTokens[0];
        VTPortal.roApp.securityToken = sTokens[1];

        if (sTokens.length > 2) {
            VTPortal.roApp.companyID = sTokens[2];
        }

        VTPortal.roApp.adfsEnabled(true);

        VTPortal.roApp.db.settings.isUsingAdfs = true;
        VTPortal.roApp.db.settings.adfsCredential = adfsSecToken;
        VTPortal.roApp.db.settings.wsURL = VTPortal.roApp.configWS();
        VTPortal.roApp.db.settings.isSSL = VTPortal.roApp.configSSL();

        VTPortal.roApp.setLanguage(VTPortal.roApp.getLanguageByID(VTPortal.roApp.db.settings.language).tag);

        VTPortal.roApp.db.settings.save();
        Cookies.remove('VTPortalToken');

        VTPortal.roApp.redirectAtHome();
    } else {
        VTPortal.roApp.userName(this.login);
        VTPortal.roApp.mtServer(this.companyID);
        VTPortal.roApp.companyID = this.companyID;

        if (VTPortal.roApp.db.settings.adfsCredential == '') {
            if (this.password == "") {
                VTPortal.app.navigate("login", { root: true });
            } else {
                VTPortal.roApp.db.settings.wsURL = this.wsURL;
                VTPortal.roApp.db.settings.isSSL = this.isSSL;
                VTPortal.roApp.db.settings.isUsingAdfs = this.isSSL;
                VTPortal.roApp.db.settings.language = this.language;
                VTPortal.roApp.db.settings.login = this.login;
                VTPortal.roApp.db.settings.MT = this.MT;
                VTPortal.roApp.db.settings.companyID = this.companyID;
                VTPortal.roApp.db.settings.password = this.password;

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
                VTPortal.roApp.db.settings.documentsFilters = this.documentsFilters;
                VTPortal.roApp.db.settings.licenseAccepted = this.licenseAccepted;
                VTPortal.roApp.db.settings.onlySupervisor = this.onlySupervisor;
                VTPortal.roApp.db.settings.supervisorPortalEnabled = this.supervisorPortalEnabled;
                //VTPortal.roApp.db.settings.AnywhereLicense = this.AnywhereLicense;
                //VTPortal.roApp.db.settings.RequiereFineLocation = this.RequiereFineLocation;

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

                //VTPortal.roApp.AnywhereLicense =VTPortal.roApp.db.settings.AnywhereLicense;
                //VTPortal.roApp.RequiereFineLocation =VTPortal.roApp.db.settings.RequiereFineLocation;

                if (this.lastStatus != '') VTPortal.roApp.userStatus(JSON.parse(this.lastStatus.decodeBase64()));
                if (this.requestFilters != '') VTPortal.roApp.requestFilters(JSON.parse(this.requestFilters.decodeBase64()));
                if (this.empPermissions != '') VTPortal.roApp.empPermissions(JSON.parse(this.empPermissions.decodeBase64()));
                if (this.teleWorkingFilters != '') VTPortal.roApp.teleWorkingFilters(JSON.parse(this.teleWorkingFilters.decodeBase64()));
                if (this.authorizationFilters != '') VTPortal.roApp.authorizationFilters(JSON.parse(this.authorizationFilters.decodeBase64()));
                if (this.supervisorFilters != '') VTPortal.roApp.supervisorFilters(JSON.parse(this.supervisorFilters.decodeBase64()));
                if (this.punchesFilters != '') VTPortal.roApp.punchesFilters(JSON.parse(this.punchesFilters.decodeBase64()));
                if (this.documentsFilters != '') VTPortal.roApp.documentsFilters(JSON.parse(this.documentsFilters.decodeBase64()));

                VTPortal.roApp.redirectAtHome();
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

            VTPortal.roApp.redirectAtHome();
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
                //try {
                //    VTPortal.roApp.db.settings.MT = result.value.data.decodeBase64();
                //} catch (e) {
                //    VTPortal.roApp.db.settings.MT = '';
                //}
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
                break;

            //case 'AnywhereLicense':
            //    VTPortal.roApp.db.settings.AnywhereLicense = (result.value.data == 'false' ? false : true);
            //    break;
            //case 'RequiereFineLocation':
            //    VTPortal.roApp.db.settings.RequiereFineLocation = (result.value.data == 'false' ? false : true);
            //    break;
        }

        result.continue();
    };
};

Settings.prototype.save = function (onEndCallback) {
    if (typeof (VTPortal.roApp.db.indexedDB) != 'undefined' && VTPortal.roApp.db.dbObject != null) {
        var db = VTPortal.roApp.db.dbObject;

        this.updateParameters(db, 'wsURL', this.wsURL);
        this.updateParameters(db, 'ssl', (this.isSSL == true ? 'true' : 'false'));
        this.updateParameters(db, 'isUsingAdfs', (this.isUsingAdfs == true ? 'true' : 'false'));
        this.updateParameters(db, 'login', this.login.encodeBase64());
        this.updateParameters(db, 'MT', this.MT);
        this.updateParameters(db, 'companyID', this.companyID.encodeBase64());
        this.updateParameters(db, 'password', this.password.encodeBase64());
        this.updateParameters(db, 'language', this.language);
        this.updateParameters(db, 'userPhoto', this.userPhoto);
        this.updateParameters(db, 'wsBackground', this.wsBackground);
        this.updateParameters(db, 'lastStatus', this.lastStatus);
        this.updateParameters(db, 'requestFilters', this.requestFilters);
        this.updateParameters(db, 'empPermissions', this.empPermissions);
        this.updateParameters(db, 'ApiVersion', this.ApiVersion);
        this.updateParameters(db, 'adfsCredential', this.adfsCredential);
        this.updateParameters(db, 'teleWorkingFilters', this.teleWorkingFilters);
        this.updateParameters(db, 'punchesFilters', this.punchesFilters);
        this.updateParameters(db, 'authorizationFilters', this.authorizationFilters);
        this.updateParameters(db, 'supervisorFilters', this.supervisorFilters);
        this.updateParameters(db, 'documentsFilters', this.documentsFilters);
        this.updateParameters(db, 'licenseAccepted', this.licenseAccepted == true ? 'true' : 'false');
        this.updateParameters(db, 'serverTimezone', this.serverTimezone);
        this.updateParameters(db, 'HeaderMD5', this.HeaderMD5);
        this.updateParameters(db, 'showForbiddenSections', this.showForbiddenSections == true ? 'true' : 'false');
        this.updateParameters(db, 'showLogoutHome', this.showLogoutHome == true ? 'true' : 'false');
        this.updateParameters(db, 'isAD', this.isAD != '' ? 'true' : 'false');
        this.updateParameters(db, 'onlySupervisor', this.onlySupervisor == true ? 'true' : 'false');
        this.updateParameters(db, 'supervisorPortalEnabled', this.supervisorPortalEnabled == true ? 'true' : 'false');
        //this.updateParameters(db, 'AnywhereLicense', this.AnywhereLicense == true ? 'true' : 'false');
        //this.updateParameters(db, 'RequiereFineLocation', this.RequiereFineLocation == true ? 'true' : 'false');

        if (VTPortal.roApp.isModeApp() && VTPortal.roApp.db.fileSystem != null) VTPortal.roApp.db.exportToJsonString(function () { });
    }

    if (typeof onEndCallback != 'undefined') onEndCallback();
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