//Utiles para crear la base de datos de configuración y cache de imagenes en disco
function DatabaseUtils(name, desc, isModeApp) {
    this.dbName = name;
    this.dbDescription = desc;

    this.isModeApp = isModeApp;
    this.tableCounter = 0;
    this.fileSystem = null;

    this.dbObject = null;
    this.indexedDB = window.indexedDB || window.mozIndexedDB || window.webkitIndexedDB || window.msIndexedDB;
    this.IDBTransaction = window.IDBTransaction || window.webkitIDBTransaction || window.msIDBTransaction || { READ_WRITE: "readwrite" }; // This line should only be needed if it is needed to support the object's constants for older browsers
    this.IDBKeyRange = window.IDBKeyRange || window.webkitIDBKeyRange || window.msIDBKeyRange;

    this.settings = new Settings();
    this.loadFileSystem();
};

DatabaseUtils.prototype.loadFileSystem = function () {
    if (this.isModeApp) {
        var roDB = this;

        var loadFileSystemCallback = function (dbUtils) {
            return function (dbObject) {
                roDB.onSuccessFileSystem(dbObject, dbUtils);
            }
        }

        //alert(device.platform);
        window.requestFileSystem(LocalFileSystem.PERSISTENT, 0, loadFileSystemCallback(roDB), function () { roDB.onSuccessFileSystem(null, null); });
        //if (device.platform != 'Android') window.requestFileSystem(LocalFileSystem.PERSISTENT, 0, roDB.onSuccessFileSystem, function () { roDB.onSuccessFileSystem(null); });
        //else this.onSuccessFileSystem(null);
    } else {
        this.onSuccessFileSystem(null, null);
    }
};

DatabaseUtils.prototype.onSuccessFileSystem = function (reqFileSystem, dbUtils) {
    if (dbUtils != null) {
        if ((typeof (dbUtils.fileSystem) == 'undefined' || dbUtils.fileSystem == null) && reqFileSystem != null) {
            dbUtils.fileSystem = reqFileSystem;
        }
    }
}

//DatabaseUtils.prototype.onSuccessFileSystem = function (reqFileSystem, dbUtils) {
//    if (typeof (dbUtils.fileSystem) != 'undefined' && dbUtils.fileSystem == null && reqFileSystem != null) {
//        dbUtils.fileSystem = reqFileSystem;
//    }
//}

DatabaseUtils.prototype.loadSettings = function () {
    if (typeof (VTPortal.roApp.db.indexedDB) != 'undefined' && VTPortal.roApp.db.dbObject == null) {
        var actualVersion = 23;

        VTPortal.roApp.db.dbObject = VTPortal.roApp.db.indexedDB.open(VTPortal.roApp.db.dbName, actualVersion);

        VTPortal.roApp.db.dbObject.onupgradeneeded = function (e) {
            var storeCreateIndex = function (objectStore, name, options) {
                if (!objectStore.indexNames.contains(name)) {
                    objectStore.createIndex(name, name, options);
                }
            }

            for (var i = e.oldVersion; i < actualVersion; i++) {
                var func = eval("DatabaseUtils.script" + (i + 1));
                func(e, storeCreateIndex);
            }
        };

        VTPortal.roApp.db.dbObject.onsuccess = function (e) {
            if (this.isModeApp && VTPortal.roApp.db.fileSystem != null) {
                VTPortal.roApp.db.loadJsStringFromDisk(function () {
                    VTPortal.roApp.db.settings.loadSettings(VTPortal.roApp.db.dbObject);
                });
            } else {
                VTPortal.roApp.db.settings.loadSettings(VTPortal.roApp.db.dbObject);
            }
        };

        VTPortal.roApp.db.dbObject.onerror = function (e) {
        };
    } else {
        VTPortal.app.navigate("login", { root: true });
    }
};

DatabaseUtils.script1 = function (e, storeCreateIndex) {
    var dataBase = e.target.result;

    var settingsCatalong = dataBase.createObjectStore('Settings', { keyPath: 'key' });
    storeCreateIndex(settingsCatalong, "key", { unique: true });

    var settingsData = [
        { id: 1, key: "wsURL", data: '' },
        { id: 2, key: "ssl", data: 'false' },
        { id: 3, key: "saveLogin", data: 'false' },
        { id: 4, key: "login", data: '' },
        { id: 5, key: "password", data: '' },
        { id: 6, key: "over4g", data: '120' },
        { id: 7, key: "overWifi", data: '60' },
        { id: 8, key: "emptyPhotoWifi", data: 'false' },
        { id: 9, key: "UUID", data: VTPortal.roApp.guidToken },
        { id: 10, key: "VinculationCode", data: '' }
    ];

    for (var i in settingsData) {
        settingsCatalong.add(settingsData[i]);
    }
}

DatabaseUtils.script2 = function (e, storeCreateIndex) {
    var txn = e.target.transaction;

    var settingsCatalong = txn.objectStore('Settings');

    var settingsData = [
        { id: 11, key: "userPhoto", data: '' },
        { id: 12, key: "wsBackground", data: '' },
        { id: 13, key: "lastStatus", data: '' },
        { id: 14, key: "requestFilters", data: '' },
        { id: 15, key: "empPermissions", data: '' },
        { id: 16, key: "teleWorkingFilters", data: '' }
    ];

    for (var i in settingsData) {
        settingsCatalong.add(settingsData[i]);
    }
}

DatabaseUtils.script3 = function (e, storeCreateIndex) {
    var txn = e.target.transaction;

    var settingsCatalong = txn.objectStore('Settings');

    var settingsData = [
        { id: 17, key: "language", data: 'ESP' }
    ];

    for (var i in settingsData) {
        settingsCatalong.add(settingsData[i]);
    }
}

DatabaseUtils.script4 = function (e, storeCreateIndex) {
    var dataBase = e.target.result;

    var viewsCatalong = dataBase.createObjectStore('Views', { keyPath: 'key' });
    storeCreateIndex(viewsCatalong, "key", { unique: true });

    var jsViewsCatalong = dataBase.createObjectStore('JsViews', { keyPath: 'key' });
    storeCreateIndex(jsViewsCatalong, "key", { unique: true });
}

DatabaseUtils.script5 = function (e, storeCreateIndex) {
    var txn = e.target.transaction;

    var settingsCatalong = txn.objectStore('Settings');

    var settingsData = [
        { id: 18, key: "ApiVersion", data: '0' }
    ];

    for (var i in settingsData) {
        settingsCatalong.add(settingsData[i]);
    }
}

DatabaseUtils.script6 = function (e, storeCreateIndex) {
    var txn = e.target.transaction;

    var settingsCatalong = txn.objectStore('Settings');

    var settingsData = [
        { id: 19, key: "documentsFilters", data: '' }
    ];

    for (var i in settingsData) {
        settingsCatalong.add(settingsData[i]);
    }
}

DatabaseUtils.script7 = function (e, storeCreateIndex) {
    var txn = e.target.transaction;

    var settingsCatalong = txn.objectStore('Settings');

    var settingsData = [
        { id: 20, key: "adfsCredential", data: '' }
    ];

    for (var i in settingsData) {
        settingsCatalong.add(settingsData[i]);
    }
}

DatabaseUtils.script8 = function (e, storeCreateIndex) {
    var txn = e.target.transaction;

    var settingsCatalong = txn.objectStore('Settings');

    var settingsData = [
        { id: 21, key: "licenseAccepted", data: 'true' }
    ];

    for (var i in settingsData) {
        settingsCatalong.add(settingsData[i]);
    }
}

DatabaseUtils.script9 = function (e, storeCreateIndex) {
    var txn = e.target.transaction;

    var settingsCatalong = txn.objectStore('Settings');

    var settingsData = [
        { id: 22, key: "punchesFilters", data: '' },
        { id: 23, key: "authorizationFilters", data: '' }
    ];

    for (var i in settingsData) {
        settingsCatalong.add(settingsData[i]);
    }
}

DatabaseUtils.script10 = function (e, storeCreateIndex) {
    var txn = e.target.transaction;

    var settingsCatalong = txn.objectStore('Settings');

    var settingsData = [
        { id: 24, key: "serverTimezone", data: 'Europe/Berlin' }
    ];

    for (var i in settingsData) {
        settingsCatalong.add(settingsData[i]);
    }
}

DatabaseUtils.script11 = function (e, storeCreateIndex) {
    var txn = e.target.transaction;

    var settingsCatalong = txn.objectStore('Settings');

    var settingsData = [
        { id: 25, key: "showForbiddenSections", data: 'true' }
    ];

    for (var i in settingsData) {
        settingsCatalong.add(settingsData[i]);
    }
}

DatabaseUtils.script12 = function (e, storeCreateIndex) {
    var txn = e.target.transaction;

    var settingsCatalong = txn.objectStore('Settings');

    var settingsData = [
        { id: 26, key: "showLogoutHome", data: 'false' }
    ];

    for (var i in settingsData) {
        settingsCatalong.add(settingsData[i]);
    }
}

DatabaseUtils.script13 = function (e, storeCreateIndex) {
    var txn = e.target.transaction;

    var settingsCatalong = txn.objectStore('Settings');

    var settingsData = [
        { id: 27, key: "onlySupervisor", data: 'false' },
        { id: 28, key: "supervisorPortalEnabled", data: 'false' },
        { id: 29, key: "supervisorFilters", data: '' }
    ];

    for (var i in settingsData) {
        settingsCatalong.add(settingsData[i]);
    }
}

DatabaseUtils.script14 = function (e, storeCreateIndex) {
    var txn = e.target.transaction;

    var settingsCatalong = txn.objectStore('Settings');

    var settingsData = [
        { id: 30, key: "isUsingAdfs", data: 'false' }
    ];

    for (var i in settingsData) {
        settingsCatalong.add(settingsData[i]);
    }
}

DatabaseUtils.script15 = function (e, storeCreateIndex) {
    var txn = e.target.transaction;

    var settingsCatalong = txn.objectStore('Settings');

    var settingsData = [
        { id: 31, key: "isAD", data: '' },
        { id: 34, key: "MT", data: '' }
    ];

    for (var i in settingsData) {
        settingsCatalong.add(settingsData[i]);
    }
}

DatabaseUtils.script16 = function (e, storeCreateIndex) {
    var txn = e.target.transaction;

    var settingsCatalong = txn.objectStore('Settings');

    var settingsData = [
        //{ id: 32, key: "AnywhereLicense", data: 'false' },
        //{ id: 33, key: "RequiereFineLocation", data: 'true' }
        { id: 35, key: "companyID", data: '' }
    ];

    for (var i in settingsData) {
        settingsCatalong.add(settingsData[i]);
    }
}

DatabaseUtils.script17 = function (e, storeCreateIndex) {
    var txn = e.target.transaction;

    var settingsCatalong = txn.objectStore('Settings');

    var settingsData = [
        //{ id: 32, key: "AnywhereLicense", data: 'false' },
        //{ id: 33, key: "RequiereFineLocation", data: 'true' }
        { id: 36, key: "HeaderMD5", data: '' }
    ];

    for (var i in settingsData) {
        settingsCatalong.add(settingsData[i]);
    }
}

DatabaseUtils.script18 = function (e, storeCreateIndex) {
    var txn = e.target.transaction;

    var settingsCatalong = txn.objectStore('Settings');

    var settingsData = [
        { id: 37, key: "supervisorFiltersDR", data: '' }
    ];

    for (var i in settingsData) {
        settingsCatalong.add(settingsData[i]);
    }
}

DatabaseUtils.script19 = function (e, storeCreateIndex) {
    var txn = e.target.transaction;

    var settingsCatalong = txn.objectStore('Settings');

    var settingsData = [
        { id: 38, key: "rememberLogin", data: '' },
        { id: 39, key: "rememberLoginApp", data: '' }
    ];

    for (var i in settingsData) {
        settingsCatalong.add(settingsData[i]);
    }
}

DatabaseUtils.script20 = function (e, storeCreateIndex) {
    var txn = e.target.transaction;

    var settingsCatalong = txn.objectStore('Settings');

    var settingsData = [
        { id: 40, key: "refreshStatus", data: '' },
        { id: 41, key: "refreshConfig", data: '' }
    ];

    for (var i in settingsData) {
        settingsCatalong.add(settingsData[i]);
    }
}

DatabaseUtils.script21 = function (e, storeCreateIndex) {
    var txn = e.target.transaction;
    var settingsCatalong = txn.objectStore('Settings');

    settingsCatalong.add({ id: 42, key: "isTimeGate", data: '' });
}

DatabaseUtils.script22 = function (e, storeCreateIndex) {
    var txn = e.target.transaction;
    var settingsCatalong = txn.objectStore('Settings');

    settingsCatalong.add({ id: 43, key: "screenTimeout", data: '10' });
}

DatabaseUtils.script23 = function (e, storeCreateIndex) {
    var txn = e.target.transaction;

    var settingsCatalong = txn.objectStore('Settings');

    var settingsData = [
        { id: 44, key: "BackgroundMD5", data: '' },
        { id: 45, key: "timeGateBackground", data: '' }
    ];

    for (var i in settingsData) {
        settingsCatalong.add(settingsData[i]);
    }
}

DatabaseUtils.prototype.updateParameters = function (key, value) {
    Settings.updateParameters(this.dbName, this.dbDescription, this.databaseSize, key, value);
};

DatabaseUtils.prototype.loadView = function (key) {
    if (!VTPortal.app.getViewTemplateInfo(key)) {
        this.loasJsView(key);
        this.loadDview(key);
    } else {
        VTPortal.app.navigate(key);
    }
}

DatabaseUtils.prototype.emptyViewsCatalogs = function () {
    if (typeof (VTPortal.roApp.db.indexedDB) != 'undefined' && VTPortal.roApp.db.dbObject != null) {
        var jsViewOS = VTPortal.roApp.db.dbObject.result.transaction("JsViews", "readwrite").objectStore("JsViews");
        var viewOS = VTPortal.roApp.db.dbObject.result.transaction("Views", "readwrite").objectStore("Views");

        jsViewOS.clear();
        viewOS.clear();
    }
}

DatabaseUtils.prototype.loasJsView = function (key) {
    if (typeof (VTPortal.roApp.db.indexedDB) != 'undefined' && VTPortal.roApp.db.dbObject != null) {
        var objectStore = VTPortal.roApp.db.dbObject.result.transaction("JsViews", "readwrite").objectStore("JsViews");
        var request = objectStore.get(key);

        request.onerror = function (event) {
            // Handle errors!
        };

        request.onsuccess = function (event) {
            // Get the old value that we want to update
            var data = event.target.result;

            if (typeof data != 'undefined') {
                eval(data.markup);
            }
        };
    }
};

DatabaseUtils.prototype.loadDview = function (key) {
    if (typeof (VTPortal.roApp.db.indexedDB) != 'undefined' && VTPortal.roApp.db.dbObject != null) {
        var objectStore = VTPortal.roApp.db.dbObject.result.transaction("Views", "readwrite").objectStore("Views");
        var request = objectStore.get(key);

        request.onerror = function (event) {
            new WebServiceRobotics(function (result) {
                if (result.Status == window.VTPortalUtils.constants.OK.value) {
                    var viewsCatalong = VTPortal.roApp.db.dbObject.result.createObjectStore('Views', { keyPath: 'key' });
                    viewsCatalong.add({ key: key, markup: result.viewContent })

                    var jsViewsCatalong = VTPortal.roApp.db.dbObject.result.createObjectStore('JsViews', { keyPath: 'key' });
                    jsViewsCatalong.add({ key: key, markup: result.jsContent })

                    eval(result.jsContent);
                    VTPortal.app.loadTemplates($(result.viewContent));
                    VTPortal.app.navigate(key);
                } else {
                    VTPortalUtils.utils.processSecurityMessage(result);
                }
            }).getView(key);
        };

        request.onsuccess = function (event) {
            // Get the old value that we want to update
            var data = event.target.result;

            if (typeof data == 'undefined') {
                new WebServiceRobotics(function (result) {
                    if (result.Status == window.VTPortalUtils.constants.OK.value) {
                        var viewsCatalong = VTPortal.roApp.db.dbObject.result.transaction("Views", "readwrite").objectStore("Views");
                        viewsCatalong.add({ key: key, markup: result.viewContent })

                        var jsViewsCatalong = VTPortal.roApp.db.dbObject.result.transaction("JsViews", "readwrite").objectStore("JsViews");
                        jsViewsCatalong.add({ key: key, markup: result.jsContent })

                        eval(result.jsContent);
                        VTPortal.app.loadTemplates($(result.viewContent));
                        VTPortal.app.navigate(key);
                    } else {
                        VTPortalUtils.utils.processSecurityMessage(result);
                    }
                }).getView(key);
            } else {
                VTPortal.app.loadTemplates($(data.markup));
                VTPortal.app.navigate(key);
            }
        };
    }
};

DatabaseUtils.prototype.exportToJsonString = function (callback) {
    if (typeof (VTPortal.roApp.db.indexedDB) != 'undefined' && VTPortal.roApp.db.dbObject != null) {
        var exportObject = {};
        if (VTPortal.roApp.db.dbObject.result.objectStoreNames.length > 0) {
            //VTPortalUtils.utils.notifyMesage('Vamos a guardar info a disco', 'info', 2000);

            $.each(VTPortal.roApp.db.dbObject.result.objectStoreNames, function (index, storeName) {
                var allObjects = [];
                var transaction = VTPortal.roApp.db.dbObject.result.transaction(VTPortal.roApp.db.dbObject.result.objectStoreNames, "readonly");

                var store = transaction.objectStore(storeName);
                var keyRange = VTPortal.roApp.db.IDBKeyRange.lowerBound(0);
                var cursorRequest = store.openCursor(keyRange);

                cursorRequest.onerror = function (event) {
                    if (VTPortal.roApp.db.fileSystem != null) VTPortal.roApp.db.writeConfigFile('');
                };

                cursorRequest.onsuccess = function (event) {
                    var cursor = event.target.result;
                    if (cursor) {
                        allObjects.push(cursor.value);
                        cursor.continue();
                    } else {
                        exportObject[storeName] = allObjects;
                        if (VTPortal.roApp.db.dbObject.result.objectStoreNames.length == Object.keys(exportObject).length) {
                            var dbJson = JSON.stringify(exportObject);
                            if (typeof callback != 'undefined' && callback != null) callback(dbJson);
                            if (VTPortal.roApp.db.fileSystem != null) VTPortal.roApp.db.writeConfigFile(dbJson);
                        }
                    }
                };
            });
        }
    }
}

DatabaseUtils.prototype.importFromJsonString = function (jsonString, callback) {
    if (typeof (VTPortal.roApp.db.indexedDB) != 'undefined' && VTPortal.roApp.db.dbObject != null) {
        try {
            if (jsonString == '') {
                if (typeof callback != 'undefined' && callback != null) callback();
                return;
            }

            var importObject = JSON.parse(jsonString);

            $.each(VTPortal.roApp.db.dbObject.result.objectStoreNames, function (index, storeName) {
                VTPortal.roApp.db.tableCounter = 0;

                if (importObject[storeName].length > 0) {
                    $.each(importObject[storeName], function (index, toAdd) {
                        var transaction = VTPortal.roApp.db.dbObject.result.transaction(VTPortal.roApp.db.dbObject.result.objectStoreNames, "readwrite");
                        var store = transaction.objectStore(storeName);
                        var cursorRequest = store.add(toAdd);

                        cursorRequest.onerror = function (event) {
                        }

                        cursorRequest.onsuccess = function (event) {
                            VTPortal.roApp.db.tableCounter++;
                            if (VTPortal.roApp.db.tableCounter == importObject[storeName].length) { // added all objects for this store
                                delete importObject[storeName];
                                if (Object.keys(importObject).length == 0) // added all object stores
                                    if (typeof callback != 'undefined' && callback != null) callback();
                            }
                        }
                    });
                } else {
                    delete importObject[storeName];
                    if (typeof callback != 'undefined' && callback != null) callback();
                }
            });
        } catch (e) {
            if (typeof callback != 'undefined' && callback != null) callback();
        }
    }
}

DatabaseUtils.prototype.clearDatabase = function (callback) {
    if (typeof (VTPortal.roApp.db.indexedDB) != 'undefined' && VTPortal.roApp.db.dbObject != null) {
        VTPortal.roApp.db.tableCounter = 0;
        $.each(VTPortal.roApp.db.dbObject.result.objectStoreNames, function (index, storeName) {
            var transaction = VTPortal.roApp.db.dbObject.result.transaction(VTPortal.roApp.db.dbObject.result.objectStoreNames, "readwrite");
            var store = transaction.objectStore(storeName);
            var cursorRequest = store.clear();

            cursorRequest.onerror = function (event) {
            }

            cursorRequest.onsuccess = function () {
                VTPortal.roApp.db.tableCounter++;
                if (VTPortal.roApp.db.tableCounter == VTPortal.roApp.db.dbObject.result.objectStoreNames.length) // cleared all object stores
                    callback();
            };
        });
    }
}

DatabaseUtils.prototype.loadJsStringFromDisk = function (callback) {
    if (VTPortal.roApp.db.fileSystem != null) {
        VTPortal.roApp.db.fileSystem.root.getFile("vtportal.conf", { create: false, exclusive: false }, function (fileEntry) {
            fileEntry.file(function (file) {
                var reader = new FileReader();

                reader.onloadend = function (evt) {
                    //VTPortalUtils.utils.notifyMesage('Fichero configuración abierto', 'info', 2000);
                    var filecontent = evt.target.result;
                    VTPortal.roApp.db.clearDatabase(function () {
                        VTPortal.roApp.db.importFromJsonString(filecontent, callback)
                    });
                };

                reader.readAsText(file);
            }, function () {
                //VTPortalUtils.utils.notifyMesage('Error abriendo fichero', 'error', 2000);
                if (typeof callback != 'undefined' && callback != null) callback();
            });
        }, function () {
            //VTPortalUtils.utils.notifyMesage('Fichero de configuración no existe, procediendo normal', 'info', 2000);
            if (typeof callback != 'undefined' && callback != null) callback();
        });
    } else {
        if (typeof callback != 'undefined' && callback != null) callback();
    }
}

DatabaseUtils.prototype.writeConfigFile = function (dbJson) {
    VTPortal.roApp.db.fileSystem.root.getFile("vtportal.conf", { create: true, exclusive: false }, function (fileEntry) {
        fileEntry.createWriter(function (fileWriter) {
            fileWriter.onwriteend = function () {
                //VTPortalUtils.utils.notifyMesage('Archivo configuración guardado', 'success', 2000);
            };

            fileWriter.onerror = function (e) {
                //VTPortalUtils.utils.notifyMesage('Error guardando fichero', 'error', 2000);
            };

            fileWriter.write(new Blob([dbJson], { type: 'text/plain' }));
        });
    }, function () {
        //VTPortalUtils.utils.notifyMesage('no se ha podido guardar el fichero de configuración', 'error', 2000);
    });
}