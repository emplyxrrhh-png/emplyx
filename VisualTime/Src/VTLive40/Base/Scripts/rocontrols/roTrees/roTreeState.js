let roTreeStateHash = {};

function isPromise(obj) {
    return !!obj && typeof obj.then === 'function';
}

const getroTreeState = async function (id, localInit) {

    let validateId = id;
    if (validateId.indexOf('"TreeState_"') < 0) validateId = "TreeState_" + id;

    if (typeof roTreeStateHash[validateId] == 'undefined' || roTreeStateHash[validateId] == null) {
        let state = new ROTreeState();

        if (typeof localInit != 'undefined' && localInit) await state.initLocalCookie(id);
        else await state.load(id);

        roTreeStateHash[validateId] = state;
    } else if (typeof localInit != 'undefined' && localInit) {
        let state = new ROTreeState();
        await state.initLocalCookie(id);
        roTreeStateHash[validateId] = state;
    }

    return roTreeStateHash[validateId];
}



function getStorageKey(key) {
    if (typeof String.prototype.encodeBase64 == 'undefined' && typeof Sugar == 'function') Sugar.extend();

    let sValue = localStorage.getItem(key);

    if (typeof sValue == 'undefined' || sValue == null || sValue == 'null' || sValue == "") return null;
    else return sValue.decodeBase64();
}

function setStorageKey(key, value) {

    if (typeof String.prototype.encodeBase64 == 'undefined' && typeof Sugar == 'function') Sugar.extend();

    if (value != null && value != 'null') localStorage.setItem(key, value.encodeBase64());
    else localStorage.setItem(key, null);
}

function ROTreeState() {

    let ID = '';
    let ActiveTree = '1';

    let Selected1 = '';
    let Selected2 = '';
    let Selected3 = '';
    let SelectedPath1 = '';
    let SelectedPath2 = '';
    let SelectedPath3 = '';
    let Expanded1 = '';
    let Expanded2 = '';
    let Filter = '11110';
    let UserFieldFilter = '';
    let UserField = '';
    let FieldFindColumn = '';
    let FieldFindValue = '';
    let serverURL = '/Cookie';
    let loaded = false;

    this.load = async function (treeID) {
        try {
            ID = treeID;
            await this.getFromCookie();
        } catch (e) { showError("roTreeState::load", e); }
    }


    /* (SET) Cookie Arbre Actiu */
    this.setActiveTreeType = function (strTreeType) {
        ActiveTree = strTreeType;
        this.saveInCookie();
    }

    /* (GET) Cookie Arbre Actiu */
    this.getActiveTreeType = function () {
        return ActiveTree;
    }

    /* (SET) Funciones para guardar y obtener la configuración de filtros */
    this.setFilter = function (strFilter, strUserFieldFilter) {
        Filter = strFilter;
        if (strUserFieldFilter != null) UserFieldFilter = strUserFieldFilter;
        this.saveInCookie();
    }

    /* (SET) Funciones para guardar y obtener la configuración de filtros */
    this.getFilter = function () {
        let Filters = new Array(2);

        Filters[0] = Filter;
        Filters[1] = UserFieldFilter;

        return Filters;
    }

    /* (SET) Funciones para guardar y obtener el campo de la ficha utilizado para generar el segundo árbol */
    this.setUserField = function (strUserField) {
        UserField = strUserField;
        this.saveInCookie();
    }

    /* (GET) Funciones para guardar y obtener el campo de la ficha utilizado para generar el segundo árbol */
    this.getUserField = function () {
        return UserField;
    }

    /* (SET) Funciones para guardar y obtener los nodos seleccionados de los árboles */
    this.setSelected = function (id, TreeType) {
        switch (TreeType) {
            case '1':
                Selected1 = id;
                break;
            case '2':
                Selected2 = id;
                break;
            case '3':
                Selected3 = id;
                break;
        }
        this.saveInCookie();
    }
    /* (GET) Funciones para guardar y obtener los nodos seleccionados de los árboles */
    this.getSelected = function (TreeType) {
        let retStr = '';
        switch (TreeType) {
            case '1':
                retStr = Selected1;
                break;
            case '2':
                retStr = Selected2;
                break;
            case '3':
                retStr = Selected3;
                break;
        }
        return retStr;
    }

    this.setSelectedPath = function (val, TreeType) {
        if (val == null || val == 'null') { val = ''; }
        switch (TreeType) {
            case '1':
                SelectedPath1 = val;
                break;
            case '2':
                SelectedPath2 = val;
                break;
            case '3':
                SelectedPath3 = val;
                break;
        }
        this.saveInCookie();
    }



    this.getSelectedPath = function (TreeType) {
        let retStr = '';
        switch (TreeType) {
            case '1':
                retStr = SelectedPath1;
                break;
            case '2':
                retStr = SelectedPath2;
                break;
            case '3':
                retStr = SelectedPath3;
                break;
        }
        if (retStr == null || retStr == 'null') { retStr = ''; }
        return retStr;
    }

    this.setExpanded = function (NodeIds, TreeType) {
        switch (TreeType) {
            case '1':
                Expanded1 = NodeIds;
                break;
            case '2':
                Expanded2 = NodeIds;
                break;
        }
        this.saveInCookie();
    }
    this.getExpanded = function (TreeType) {
        let retStr = '';
        switch (TreeType) {
            case '1':
                retStr = Expanded1;
                break;
            case '2':
                retStr = Expanded2;
                break;
        }
        if (retStr == null || retStr == 'null') { retStr = ''; }
        return retStr;
    }

    //Carrega les cookies per el tercer treeview
    this.setFieldFind = function (FieldColumn, FieldValue) {
        FieldFindColumn = FieldColumn;
        if (FieldValue.charAt(0) == ' ') {
            FieldValue = '%20' + FieldValue.substring(1, FieldValue.length);
        }
        FieldFindValue = FieldValue;
        this.saveInCookie();
    }
    this.getFieldFind = function () {
        let FieldFind = new Array(2);

        let strColumn = FieldFindColumn;
        if (strColumn == '' || strColumn == 'undefined' || strColumn == null) strColumn = 'EmployeeName';
        FieldFind[0] = strColumn;

        let strValue = FieldFindValue;
        if (strValue == 'undefined' || strValue == null) strValue = '';
        FieldFind[1] = strValue;

        return FieldFind;
    }

    this.buildTreeStateObject = function () {
        let objTreeState = {
            "ID": ID, "ActiveTree": ActiveTree, "Filter": Filter, "Selected1": Selected1, "Selected2": Selected2.replace(";", "-"), "Selected3": Selected3,
            "SelectedPath1": SelectedPath1, "SelectedPath2": SelectedPath2.replace(";", "-"), "SelectedPath3": SelectedPath3,
            "Expanded1": Expanded1, "Expanded2": Expanded2, "UserFieldFilter": encodeURIComponent(UserFieldFilter),
            "UserField": UserField, "FieldFindColumn": FieldFindColumn, "FieldFindValue": FieldFindValue
        };

        return objTreeState;
    }
    this.saveInCookie = async function () {

        if (getStorageKey("TreeState_" + ID) == null || !loaded) return;

        let objTreeState = this.buildTreeStateObject();

        try {
            let bNeedToSave = true;
            let bInProcessToServer = false;
            let cookieStorageItem = null;

            if (getStorageKey("TreeState_" + ID) !== null) {
                cookieStorageItem = JSON.parse(getStorageKey("TreeState_" + ID));
                bInProcessToServer = cookieStorageItem.sendToServer;

                let sItem = JSON.stringify(cookieStorageItem.item);
                let nItem = JSON.stringify(objTreeState);



                if (sItem == nItem) bNeedToSave = false;
            }

            setStorageKey("TreeState_" + ID, JSON.stringify({ item: objTreeState, timestamp: moment().format("YYYYMMDDHHmmss"), sendToServer: (bNeedToSave || bInProcessToServer) }));

            let tmpTreeState = this;
            roTreeStateHash["TreeState_" + ID] = tmpTreeState;

            if (!bInProcessToServer && bNeedToSave) {

                let saveFunction = function (saveID) {
                    return async function () {
                        try {
                            let tmpCookieStorageItem = JSON.parse(getStorageKey("TreeState_" + saveID));
                            // Configurar la solicitud POST
                            let response = await fetch(serverURL + "/SetCookie", {
                                method: 'POST',
                                headers: {
                                    'Content-Type': 'application/json'
                                },
                                body: JSON.stringify({ sCookieName: "TreeState_" + saveID, sCookieValue: JSON.stringify(tmpCookieStorageItem.item) })
                            });

                            // Verificar la respuesta
                            if (!response.ok) {
                                throw new Error('Error en la solicitud POST');
                            }

                            // Convertir la respuesta a JSON
                            let bSaved = await response.json();

                            if (bSaved) {
                                setStorageKey("TreeState_" + saveID, JSON.stringify({ item: tmpCookieStorageItem.item, timestamp: moment().format("YYYYMMDDHHmmss"), sendToServer: false }));
                            }

                        } catch {
                        }
                    }
                }

                setTimeout(saveFunction(ID), 1000);

            } else {
                setStorageKey("TreeState_" + ID, JSON.stringify({ item: objTreeState, timestamp: moment().format("YYYYMMDDHHmmss"), sendToServer: bInProcessToServer }));
            }

        }
        catch (e) {
            showError("roTreeState::saveInCookie", e);
        }
    }

    this.getFromCookie = async function () {
        try {

            eraseCookie("TreeState_" + ID);
            eraseCookie("TreeState_" + ID + "_Big");


            let cookieValue = null;
            let cookieStorageItem = null;
            let loadFromServer = true;
            let bIsSendingToServer = false;

            if (getStorageKey("TreeState_" + ID) !== null) {
                cookieStorageItem = JSON.parse(getStorageKey("TreeState_" + ID));
                bIsSendingToServer = cookieStorageItem.sendToServer;
                loadFromServer = false;

                let cookieCreatedAt = moment(cookieStorageItem.timestamp, "YYYYMMDDHHmmss");
                let now = moment();
                if (bIsSendingToServer && now.diff(cookieCreatedAt, 'seconds') > 10) bIsSendingToServer = false;
                if (now.diff(cookieCreatedAt, 'seconds') > 1800) loadFromServer = true;
                else cookieValue = cookieStorageItem.item;
            }



            if (loadFromServer) {
                try {
                    // Configurar la solicitud POST
                    let response = await fetch(serverURL + "/GetCookie", {
                        method: 'POST',
                        headers: {
                            'Content-Type': 'application/json'
                        },
                        body: JSON.stringify({ sCookieName: "TreeState_" + ID })
                    });

                    // Verificar la respuesta
                    if (!response.ok) {
                        cookieValue = null;
                    }

                    // Convertir la respuesta a JSON
                    let serverResponse = await response.json();

                    if (serverResponse != null && serverResponse != "") {
                        cookieValue = JSON.parse(serverResponse);
                    }
                } catch {
                    cookieValue = null;
                }
            }

            if (cookieValue != null) {

                ActiveTree = cookieValue.ActiveTree;
                Filter = cookieValue.Filter;
                Selected1 = cookieValue.Selected1;
                Selected2 = cookieValue.Selected2;
                Selected3 = cookieValue.Selected3;
                SelectedPath1 = cookieValue.SelectedPath1;
                SelectedPath2 = cookieValue.SelectedPath2;
                SelectedPath3 = cookieValue.SelectedPath3;
                Expanded1 = cookieValue.Expanded1;
                Expanded2 = cookieValue.Expanded2;
                Filter = cookieValue.Filter;
                UserFieldFilter = decodeURIComponent(cookieValue.UserFieldFilter);
                UserField = cookieValue.UserField;
                FieldFindColumn = cookieValue.FieldFindColumn;
                FieldFindValue = cookieValue.FieldFindValue;

            }

            setStorageKey("TreeState_" + ID, JSON.stringify({ item: this.buildTreeStateObject(), timestamp: moment().format("YYYYMMDDHHmmss"), sendToServer: bIsSendingToServer }));

            roTreeStateHash["TreeState_" + ID] = this;
        }
        catch (e) {
            showError("roTreeState::getFromCookie", e);
        } finally {
            loaded = true;
        }
    }


    this.initLocalCookie = async function (treeID) {
        try {
            ID = treeID;

            eraseCookie("TreeState_" + ID);
            eraseCookie("TreeState_" + ID + "_Big");

            setStorageKey("TreeState_" + ID, JSON.stringify({ item: this.buildTreeStateObject(), timestamp: moment().format("YYYYMMDDHHmmss"), sendToServer: false }));
            roTreeStateHash["TreeState_" + ID] = this;
        }
        catch (e) {
            showError("roTreeState::initLocalCookie", e);
        } finally {
            loaded = true;
        }
    }

    this.setRedirectData = async function (selectedNode, selectedPath) {
        try {
            eraseCookie("TreeState_" + ID);
            eraseCookie("TreeState_" + ID + "_Big");

            Selected1 = selectedNode;
            SelectedPath1 = selectedPath;
            Selected2 = "";
            SelectedPath2 = "";
            Selected3 = "";
            SelectedPath3 = "";
            Filter = "11110";
            UserFieldFilter = "";

            let tmpCookieStorageItem = this.buildTreeStateObject();
            // Configurar la solicitud POST
            let response = await fetch(serverURL + "/SetCookie", {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({ sCookieName: "TreeState_" + ID, sCookieValue: JSON.stringify(tmpCookieStorageItem) })
            });

            // Verificar la respuesta
            if (!response.ok) {
                throw new Error('Error en la solicitud POST');
            }

            // Convertir la respuesta a JSON
            let bSaved = await response.json();

            if (bSaved) {
                setStorageKey("TreeState_" + ID, JSON.stringify({ item: tmpCookieStorageItem, timestamp: moment().format("YYYYMMDDHHmmss"), sendToServer: false }));
                roTreeStateHash["TreeState_" + ID] = this;
            }
        }
        catch (e) {
            showError("roTreeState::initLocalCookie", e);
        } finally {
            loaded = true;
        }
    }

    this.setLocalData = async function (selectedNode1,selectedNode2, selectedNode3, strFilter, strUserFieldFilter) {
        try {
            eraseCookie("TreeState_" + ID);
            eraseCookie("TreeState_" + ID + "_Big");

            Selected1 = selectedNode1;
            Selected2 = selectedNode2;
            Selected3 = selectedNode3;
            Filter = strFilter;
            if (strUserFieldFilter != null) UserFieldFilter = strUserFieldFilter;

            let tmpCookieStorageItem = this.buildTreeStateObject();
            // Configurar la solicitud POST
            let response = await fetch(serverURL + "/SetCookie", {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({ sCookieName: "TreeState_" + ID, sCookieValue: JSON.stringify(tmpCookieStorageItem) })
            });

            // Verificar la respuesta
            if (!response.ok) {
                throw new Error('Error en la solicitud POST');
            }

            // Convertir la respuesta a JSON
            let bSaved = await response.json();

            if (bSaved) {
                setStorageKey("TreeState_" + ID, JSON.stringify({ item: tmpCookieStorageItem, timestamp: moment().format("YYYYMMDDHHmmss"), sendToServer: false }));
                roTreeStateHash["TreeState_" + ID] = this;
            }
        }
        catch (e) {
            showError("roTreeState::initLocalCookie", e);
        } finally {
            loaded = true;
        }
    }


    this.clear = async function () {
        eraseCookie("TreeState_" + ID);

        setStorageKey("TreeState_" + ID, null);
        roTreeStateHash["TreeState_" + ID] = null;

        try {
            await fetch(serverURL + "/EraseCookie", {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({ sCookieName: "TreeState_" + ID })
            });



        } catch { }
    }

    this.reset = async function () {
        eraseCookie("TreeState_" + ID);

        setStorageKey("TreeState_" + ID, null);
        roTreeStateHash["TreeState_" + ID] = null;
    }

    this.reload = async function () {
        eraseCookie("TreeState_" + ID);
        roTreeStateHash["TreeState_" + ID] = null;
    }
}


