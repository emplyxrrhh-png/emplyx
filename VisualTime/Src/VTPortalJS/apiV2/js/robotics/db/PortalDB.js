//######################################################
//#          Portal DataBase                              #
function PortalDB() {
    this.myDB = typeof window.sqlitePlugin != 'undefined' ? window.sqlitePlugin.openDatabase({ name: "Portal.db", location: 'default' }) : null;

    this.currentDBVersion = 2;

    if (this.myDB != null) this.initDB();
}

PortalDB.prototype.initDB = async function () {
    await this.createPortalDB();
    await this.resetOngoingPunches();
}

PortalDB.prototype.dbActionError = function (customError) {
    return function (error) {
        if (typeof customError == 'function') customError();
    };
};

PortalDB.prototype.createPortalDB = async function () {
    var instance = this;

    return new Promise(async function (resolve, reject) {
        if (instance.myDB == null) {
            instance.dbActionError();
            reject('PortalDB::clearDB::No database found');
        }

        instance.myDB.sqlBatch([
            'CREATE TABLE IF NOT EXISTS employees (idEmployee INTEGER PRIMARY KEY, name TEXT, language TEXT, pin TEXT,AllowedCauses TEXT,isOnline BOOLEAN DEFAULT 0,bioConsent BOOLEAN DEFAULT 0)',
            'CREATE UNIQUE INDEX IF NOT EXISTS idxemployees ON employees(idEmployee)',
            'CREATE TABLE IF NOT EXISTS punches(idEmployee INTEGER, credential Text, Method INTEGER, Action TEXT, PunchDateTime DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP, PunchData Text, Photo TEXT, online BOOLEAN DEFAULT 0, status INTEGER DEFAULT 0, Other TEXT, sentDateTime DATETIME DEFAULT NULL, command INTEGER DEFAULT 0);',
            'CREATE INDEX IF NOT EXISTS idxpunches ON punches(PunchDateTime DESC);',
            'CREATE TABLE IF NOT EXISTS causes (idCause INTEGER PRIMARY KEY, name TEXT)',
            'CREATE UNIQUE INDEX IF NOT EXISTS idxcauses ON causes(idCause)',
            'CREATE TABLE IF NOT EXISTS sysconfig (name TEXT PRIMARY KEY, value TEXT);',
            'CREATE INDEX IF NOT EXISTS idxsysconfig ON sysconfig(name);',

        ], async function (rs) {
            resolve(true);
        }, instance.dbActionError(function () { resolve(false) }));
    });
};

PortalDB.prototype.addCausesBatch = function (causes) {
    var instance = this;

    return new Promise(function (resolve, reject) {
        if (instance.myDB == null) {
            instance.dbActionError();
            reject('PortalDB::addCausesBatch::No database found');
        }

        var sInsert = [];
        sInsert.push(["delete FROM causes"]);

        for (var i = 0; i < causes.length; i++) {
            sInsert.push(["insert into causes (idCause, name) VALUES(?,?)", [causes[i].IDCause, causes[i].Name]]);
        }

        instance.myDB.sqlBatch(sInsert, function (rs) {
            resolve(true);
        }, instance.dbActionError(function () { resolve(false) }));
    });
};

PortalDB.prototype.delallCauses = function () {
    var instance = this;

    return new Promise(function (resolve, reject) {
        if (instance.myDB == null) {
            instance.dbActionError();
            reject('PortalDB::delallCauses::No database found');
        }

        instance.myDB.executeSql("delete FROM causes", [], function (rs) {
            resolve(true);
        }, instance.dbActionError(function () { resolve(false) }));
    });
}

//------------------------------- LOGICA -----------------------------------------------------------------------------

PortalDB.prototype.savePunch = function (punch) {
    var instance = this;

    return new Promise(function (resolve, reject) {
        if (instance.myDB == null) {
            instance.dbActionError();
            resolve(false);
            //reject('PortalDB::savePunch::No database found');
        }

        var punchParams = [punch.punch.idEmployee, punch.punch.credential, punch.punch.method, punch.punch.action, punch.punch.punchDateTime, JSON.stringify(punch.punch.PunchData), punch.punch.punchImage, punch.punch.online, punch.punch.status, punch.punch.other, null, punch.punch.command];

        instance.myDB.executeSql('insert or replace into punches(idEmployee, credential, Method, Action, PunchDateTime, PunchData, Photo, online, status, Other, sentDatetime, command) VALUES(?,?,?,?,?,?,?,?,?,?,?,?)', punchParams, function (rs) {
            resolve(true);
        }, instance.dbActionError(function () { resolve(false); }));// reject('PortalDB::savePunch::cound not save punch idEmployee:' + punch.idEmployee + ' datetime:' + punch.punchDateTime); }));
    });
};

PortalDB.prototype.updatePunchStatus = function (punch, sentDateTime, punchId) {
    var instance = this;

    return new Promise(function (resolve, reject) {
        if (instance.myDB == null) {
            instance.dbActionError();
            reject('PortalDB::updatePunchStatus::No database found');
        }

        var punchParams = [punch.status, sentDateTime, punchId];

        instance.myDB.executeSql('update punches set status=?, sentDateTime=? where rowid=?', punchParams, function (rs) {
            resolve();
        }, instance.dbActionError(function () { reject('PortalDB::updatePunchStatus::cound not update punch staus rowId:' + punchId + ' status:' + punch.status + ' sentDateTime:' + moment(sentDateTime).format('YYYY-MM-DD hh:mm')); }));
    });
};

PortalDB.prototype.deletePunch = function (punchId) {
    var instance = this;

    return new Promise(function (resolve, reject) {
        if (instance.myDB == null) {
            instance.dbActionError();
            reject('PortalDB::deletePunch::No database found');
        }

        var punchParams = [punchId];

        instance.myDB.executeSql('delete from punches where rowid=?', punchParams, function (rs) {
            resolve();
        }, instance.dbActionError(function () { reject('PortalDB::deletePunch::cound not delete punch with rowId:' + punchId + ' status:' + punch.status + ' sentDateTime:' + moment(sentDateTime).format('YYYY-MM-DD hh:mm')); }));
    });
};

PortalDB.prototype.resetOngoingPunches = function () {
    var instance = this;

    return new Promise(function (resolve, reject) {
        if (instance.myDB == null) {
            instance.dbActionError();
            reject('PortalDB::resetOngoingPunches::No database found');
        }

        instance.myDB.executeSql('update punches set status=0 where status=1', [], function (rs) {
            resolve();
        }, instance.dbActionError(function () { reject('PortalDB::resetOngoingPunches::cound not update punch '); }));
    });
};

PortalDB.prototype.getNextPunchesToSend = function () {
    var instance = this;

    return new Promise(function (resolve, reject) {
        if (instance.myDB == null) {
            instance.dbActionError();
            reject('PortalDB::getNextPunchesToSend::No database found');
        }

        instance.myDB.executeSql("SELECT rowid, * from punches Where status = 0 OR (status = 1 AND DATETIME(PunchDateTime, '+1 minutes') <= datetime('now','localtime')) OR (status = 3 AND (sentDatetime = NULL OR sentDatetime <= datetime('now','localtime'))) order by PunchDateTime ASC ", [], function (selectRs) {
            instance.myDB.executeSql('UPDATE punches SET status = 1 Where status = 0', [], function (updateRS) {
                resolve(selectRs.rows);
            }, instance.dbActionError(function () { reject('PortalDB::getNextPunchesToSend::cound not update punches to sending'); }));
        }, instance.dbActionError(function () { reject('PortalDB::getNextPunchesToSend::cound not select pending punches'); }));
    });
};

PortalDB.prototype.close = function (onSuccessCallback) {
    if (this.myDB == null) {
        this.dbActionError(onErrorCallback);
        return;
    }

    this.myDB.close(function () {
        if (typeof onSuccessCallback == 'function') onSuccessCallback();
    });
};