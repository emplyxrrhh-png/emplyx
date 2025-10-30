let roRequestParamsHash = {};

const getroRequestParams = async function (forceLoad) {

    if (typeof forceLoad != 'undefined' && forceLoad) {
        let state = new roRequestListParams();
        await state.forceload("TreeState_RequestListParams");
        roRequestParamsHash["TreeState_RequestListParams"] = state;
    } else if (typeof roRequestParamsHash["TreeState_RequestListParams"] == 'undefined' || roRequestParamsHash["TreeState_RequestListParams"] == null) {
        let state = new roRequestListParams();
        state.load("TreeState_RequestListParams");

        roRequestParamsHash["TreeState_RequestListParams"] = state;
    }

    return roRequestParamsHash["TreeState_RequestListParams"];
}
function roRequestListParams() {
    //LISTA PENDIENTES
    let PendList_OrderField = 'RequestDate';
    let PendList_OrderDirection = 'ASC';
    let PendList_FilterRequestType = '1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16';
    let PendList_FilterRequestDate = ''; //BeginDate*EndDate yyyy/MM/dd*yyyy/MM/dd
    let PendList_FilterRequestedDate = ''; //BeginDate*EndDate yyyy/MM/dd*yyyy/MM/dd
    let PendList_FilterEmployee = '';
    let PendList_FilterTree = '';
    let PendList_FilterTreeUser = '';
    let PendList_IdCause = '0';
    let PendList_IdSupervisor = '0';

    //LISTA OTRAS
    let OtherList_OrderField = 'RequestDate';
    let OtherList_OrderDirection = 'ASC';
    let OtherList_FilterRequestType = '1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16';
    let OtherList_FilterRequestDate = ''; //BeginDate*EndDate yyyy/MM/dd*yyyy/MM/dd
    let OtherList_FilterRequestedDate = ''; //BeginDate*EndDate yyyy/MM/dd*yyyy/MM/dd
    let OtherList_FilterEmployee = '';
    let OtherList_FilterTree = '';
    let OtherList_FilterTreeUser = '';
    let OtherList_IdCause = '0';
    let OtherList_IdSupervisor = '0';
    let OtherList_FilterLevels = ''; //Mostrar solicitudes que estén X niveles por debajo del nivel actual
    let OtherList_FilterDaysFrom = ''; // Mostrar las solicitudes con X días de antiguedad

    //LISTA HISTORICO
    let HistoryList_OrderField = 'RequestDate';
    let HistoryList_OrderDirection = 'ASC';
    let HistoryList_FilterRequestType = '1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16';
    let HistoryList_FilterRequestDate = ''; //BeginDate*EndDate yyyy/MM/dd*yyyy/MM/dd
    let HistoryList_FilterRequestedDate = ''; //BeginDate*EndDate yyyy/MM/dd*yyyy/MM/dd
    let HistoryList_FilterEmployee = '';
    let HistoryList_FilterTree = '';
    let HistoryList_FilterTreeUser = '';
    let HistoryList_IdCause = '0';
    let HistoryList_IdSupervisor = '0';
    let HistoryList_FilterRequestState = '11000';

    let FilterVersion = '17';
    let CurrentFilterVersion = '17';

    let serverURL = '/Cookie';
    let loaded = false;

    /* Reseteja la clase */
    this.clear = function () {
        eraseCookie("TreeState_RequestListParams");
    }

    this.load = function () {
        try {
            this.getFromCookie();
        } catch (e) { showError("roRequestListParams::load", e); }
    }

    this.forceload = async function () {
        try {
            await this.loadFromServer();
        } catch (e) { showError("roRequestListParams::forceload", e); }
    }

    //ORDERFIELD
    this.setOrder = function (ListType, strOrderField) {
        if (ListType == 0) {
            PendList_OrderField = strOrderField;
        }
        if (ListType == 1) {
            OtherList_OrderField = strOrderField;
        }
        if (ListType == 2) {
            HistoryList_OrderField = strOrderField;
        }
        this.saveInCookie();
    }
    this.getOrder = function (ListType) {
        if (ListType == 0) {
            return PendList_OrderField + " " + PendList_OrderDirection;
        }
        if (ListType == 1) {
            return OtherList_OrderField + " " + OtherList_OrderDirection;
        }
        if (ListType == 2) {
            return HistoryList_OrderField + " " + HistoryList_OrderDirection;
        }
    }
    //END ORDERFIELD

    //filter version
    this.setFilterVersion = function (strFilterVersion) {
        FilterVersion = strFilterVersion;
        this.saveInCookie();
    }

    this.getCurrentFilterVersion = function () {
        return CurrentFilterVersion;
    }

    this.getFilterVersion = function () {
        return FilterVersion;
    }

    //ORDERDIRECTION
    this.setOrderDirection = function (ListType, strOrderDirection) {
        if (ListType == 0) {
            PendList_OrderDirection = strOrderDirection;
        }
        if (ListType == 1) {
            OtherList_OrderDirection = strOrderDirection;
        }
        if (ListType == 2) {
            HistoryList_OrderDirection = strOrderDirection;
        }
        this.saveInCookie();
    }
    this.getOrderDirection = function (ListType) {
        if (ListType == 0) {
            return PendList_OrderDirection;
        }
        if (ListType == 1) {
            return OtherList_OrderDirection;
        }
        if (ListType == 2) {
            return HistoryList_OrderDirection;
        }
    }
    //END ORDERDIRECTION

    //REQUESTTYPE
    this.setFilterRequestType = function (ListType, strRequestType) {
        if (ListType == 0) {
            PendList_FilterRequestType = strRequestType;
        }
        if (ListType == 1) {
            OtherList_FilterRequestType = strRequestType;
        }
        if (ListType == 2) {
            HistoryList_FilterRequestType = strRequestType;
        }
        this.saveInCookie();
    }
    this.getFilterRequestType = function (ListType) {
        if (ListType == 0) {
            return PendList_FilterRequestType;
        }
        if (ListType == 1) {
            return OtherList_FilterRequestType;
        }
        if (ListType == 2) {
            return HistoryList_FilterRequestType;
        }
    }
    //END REQUESTTYPE

    //REQUESTDATE
    this.setFilterRequestDate = function (ListType, BeginDate, EndDate) {
        let strDates = "";
        if (BeginDate != "" && EndDate != "") strDates = BeginDate + '*' + EndDate;

        if (ListType == 0) {
            PendList_FilterRequestDate = strDates;
        }
        if (ListType == 1) {
            OtherList_FilterRequestDate = strDates;
        }
        if (ListType == 2) {
            HistoryList_FilterRequestDate = strDates;
        }
        this.saveInCookie();
    }
    this.getFilterRequestDate = function (ListType) {
        if (ListType == 0) {
            return PendList_FilterRequestDate;
        }
        if (ListType == 1) {
            return OtherList_FilterRequestDate;
        }
        if (ListType == 2) {
            return HistoryList_FilterRequestDate;
        }
    }
    //END REQUESTDATE

    //RequestedDATE
    this.setFilterRequestedDate = function (ListType, BeginDate, EndDate) {
        let strDates = "";
        if (BeginDate != "" && EndDate != "") strDates = BeginDate + '*' + EndDate;

        if (ListType == 0) {
            PendList_FilterRequestedDate = strDates;
        }
        if (ListType == 1) {
            OtherList_FilterRequestedDate = strDates;
        }
        if (ListType == 2) {
            HistoryList_FilterRequestedDate = strDates;
        }
        this.saveInCookie();
    }
    this.getFilterRequestedDate = function (ListType) {
        if (ListType == 0) {
            return PendList_FilterRequestedDate;
        }
        if (ListType == 1) {
            return OtherList_FilterRequestedDate;
        }
        if (ListType == 2) {
            return HistoryList_FilterRequestedDate;
        }
    }
    //END RequestedDATE

    //FILTEREMPLOYEE
    this.setFilterEmployees = function (ListType, strEmployees) {
        if (ListType == 0) {
            PendList_FilterEmployee = strEmployees;
        }
        if (ListType == 1) {
            OtherList_FilterEmployee = strEmployees;
        }
        if (ListType == 2) {
            HistoryList_FilterEmployee = strEmployees;
        }
        this.saveInCookie();
    }
    this.getFilterEmployees = function (ListType) {
        if (ListType == 0) {
            return PendList_FilterEmployee;
        }
        if (ListType == 1) {
            return OtherList_FilterEmployee;
        }
        if (ListType == 2) {
            return HistoryList_FilterEmployee;
        }
    }
    //END FILTEREMPLOYEE

    //FILTERTREE
    this.setFilterTree = function (ListType, strFilterTree) {
        if (ListType == 0) {
            PendList_FilterTree = strFilterTree;
        }
        if (ListType == 1) {
            OtherList_FilterTree = strFilterTree;
        }
        if (ListType == 2) {
            HistoryList_FilterTree = strFilterTree;
        }
        this.saveInCookie();
    }
    this.getFilterTree = function (ListType) {
        if (ListType == 0) {
            return PendList_FilterTree;
        }
        if (ListType == 1) {
            return OtherList_FilterTree;
        }
        if (ListType == 2) {
            return HistoryList_FilterTree;
        }
    }
    //END FILTERTREE

    //FILTERTREEUSER
    this.setFilterTreeUser = function (ListType, strFilterTreeUser) {
        if (ListType == 0) {
            PendList_FilterTreeUser = strFilterTreeUser;
        }
        if (ListType == 1) {
            OtherList_FilterTreeUser = strFilterTreeUser;
        }
        if (ListType == 2) {
            HistoryList_FilterTreeUser = strFilterTreeUser;
        }
        this.saveInCookie();
    }
    this.getFilterTreeUser = function (ListType) {
        if (ListType == 0) {
            return PendList_FilterTreeUser;
        }
        if (ListType == 1) {
            return OtherList_FilterTreeUser;
        }
        if (ListType == 2) {
            return HistoryList_FilterTreeUser;
        }
    }
    //END FILTERTREEUSER

    //IDCAUSE
    this.setFilterIdCause = function (ListType, strIdCause) {
        if (ListType == 0) {
            PendList_IdCause = strIdCause;
        }
        if (ListType == 1) {
            OtherList_IdCause = strIdCause;
        }
        if (ListType == 2) {
            HistoryList_IdCause = strIdCause;
        }
        this.saveInCookie();
    }
    this.getFilterIdCause = function (ListType) {
        if (ListType == 0) {
            return PendList_IdCause;
        }
        if (ListType == 1) {
            return OtherList_IdCause;
        }
        if (ListType == 2) {
            return HistoryList_IdCause;
        }
    }
    //END IDCAUSE

    //IDSupervisor
    this.setFilterIdSupervisor = function (ListType, strIdSupervisor) {
        if (ListType == 0) {
            PendList_IdSupervisor = strIdSupervisor;
        }
        if (ListType == 1) {
            OtherList_IdSupervisor = strIdSupervisor;
        }
        if (ListType == 2) {
            HistoryList_IdSupervisor = strIdSupervisor;
        }
        this.saveInCookie();
    }

    this.getFilterIdSupervisor = function (ListType) {
        if (ListType == 0) {
            return PendList_IdSupervisor;
        }
        if (ListType == 1) {
            return OtherList_IdSupervisor;
        }
        if (ListType == 2) {
            return HistoryList_IdSupervisor;
        }
    }
    //END IDSupervisor

    this.setFilterLevels = function (Levels) {
        OtherList_FilterLevels = Levels;
        this.saveInCookie();
    }
    this.getFilterLevels = function () {
        return OtherList_FilterLevels;
    }

    this.setFilterDaysFrom = function (Days) {
        OtherList_FilterDaysFrom = Days;
        this.saveInCookie();
    }

    this.getFilterDaysFrom = function () {
        return OtherList_FilterDaysFrom;
    }

    this.setFilterRequestState = function (strRequestState) {
        HistoryList_FilterRequestState = strRequestState;
        this.saveInCookie();
    }

    this.getFilterRequestState = function () {
        return HistoryList_FilterRequestState;
    }

    this.getLevelsBelow = function (ListType) {
        if (ListType == 0) {
            return '1';
        }
        if (ListType == 1) {
            if (OtherList_FilterLevels != '') {
                return '2#' + (1 + parseInt(OtherList_FilterLevels))
            }
            else {
                return 'gt1';
            }
        }
        if (ListType == 2) {
            return "";
        }
    }

    //========================================================================================
    this.getServerFilterRequestDate = function (ListType) {
        let strAux = this.getFilterRequestDate(ListType);
        if (strAux != '') {
            let oArray = strAux.split('*');
            let filter = '';
            if (oArray[0] != '') {
                filter = filter + "AND RequestDate >= CONVERT(smalldatetime, '" + oArray[0] + " 00:00:00', 102) ";
            }
            if (oArray[1] != '') {
                filter = filter + "AND RequestDate <= CONVERT(smalldatetime, '" + oArray[1] + " 23:59:59', 102) ";
            }
            return filter;
        }
        return "";
    }

    this.getServerFilterRequestedDate = function (ListType) {
        let strAux = this.getFilterRequestedDate(ListType);
        if (strAux != '' && strAux != undefined) {
            let oArray = strAux.split('*');
            let filter = '';
            if (oArray[0] != '') {
                filter = filter + "AND Date1 >= CONVERT(smalldatetime, '" + oArray[0] + " 00:00:00', 102) ";
            }
            if (oArray[1] != '') {
                filter = filter + "AND Date1 <= CONVERT(smalldatetime, '" + oArray[1] + " 00:00:00', 102) ";
            }
            return filter;
        }
        return "";
    }

    this.getServerFilterRequestType = function (ListType) {
        let strAux = this.getFilterRequestType(ListType)
        if (strAux != '') {
            return "AND RequestType IN (" + strAux + ") ";
        }
        else {
            return "AND RequestType IN (0) ";
        }
    }

    this.getServerFilterHistoric = function (ListType) {
        let filter = '';
        if (ListType == 2 && HistoryList_FilterRequestState != '') {
            let strStates = '';
            if (HistoryList_FilterRequestState.substring(0, 1) == '1') strStates += ',0'; 
            if (HistoryList_FilterRequestState.substring(1, 2) == '1') strStates += ',1'; 
            if (HistoryList_FilterRequestState.substring(2, 3) == '1') strStates += ',2'; 
            if (HistoryList_FilterRequestState.substring(3, 4) == '1') strStates += ',3'; 
            if (HistoryList_FilterRequestState.substring(4, 5) == '1') strStates += ',4'; 

            if (strStates != '') {
                strStates = strStates.substring(1, strStates.length);
                filter = 'AND Requests.Status IN (' + strStates + ') ';
            }
        }

        return filter;
    }

    this.getFilter = function (ListType) {
        let strFilter = this.getServerFilterRequestDate(ListType);
        strFilter += this.getServerFilterRequestedDate(ListType);
        strFilter += this.getServerFilterRequestType(ListType);

        // Sólo mostramos las que estén pendientes o en curso
        if (ListType == 0)  strFilter = strFilter + 'AND Requests.Status IN (0,1) '; 

        if (ListType == 1) {
            if (OtherList_FilterDaysFrom != '') {
                strFilter = strFilter + "AND DATEDIFF(day, Requests.RequestDate, GETDATE()) >= " + OtherList_FilterDaysFrom + " ";
            }
            strFilter = strFilter + 'AND Requests.Status IN (0,1) '; // Sólo mostramos las que estén pendientes o en curso
        }

        strFilter += this.getServerFilterHistoric(ListType);

        if (strFilter != '' && strFilter.length > 4) {
            strFilter = strFilter.substring(4, strFilter.length);
        }

        return strFilter;
    }

    this.buildRequestObject = function () {
        return {
            "OrderFieldPend": PendList_OrderField,
            "OrderDirectionPend": PendList_OrderDirection,
            "FilterRequestTypePend": PendList_FilterRequestType,
            "FilterRequestDatePend": PendList_FilterRequestDate,
            "FilterRequestedDatePend": PendList_FilterRequestedDate,
            "FilterEmployeePend": PendList_FilterEmployee,
            "FilterTreePend": PendList_FilterTree,
            "FilterTreeUserPend": PendList_FilterTreeUser,
            "FilterIdCausePend": PendList_IdCause,
            "FilterIdSupervisorPend": PendList_IdSupervisor,
            "OrderFieldOther": OtherList_OrderField,
            "OrderDirectionOther": OtherList_OrderDirection,
            "FilterRequestTypeOther": OtherList_FilterRequestType,
            "FilterRequestDateOther": OtherList_FilterRequestDate,
            "FilterRequestedDateOther": OtherList_FilterRequestedDate,
            "FilterEmployeeOther": OtherList_FilterEmployee,
            "FilterTreeOther": OtherList_FilterTree,
            "FilterTreeUserOther": OtherList_FilterTreeUser,
            "FilterIdCauseOther": OtherList_IdCause,
            "FilterIdSupervisorOther": OtherList_IdSupervisor,
            "FilterLevelsOther": OtherList_FilterLevels,
            "FilterDaysFromOther": OtherList_FilterDaysFrom,
            "OrderFieldHist": HistoryList_OrderField,
            "OrderDirectionHist": HistoryList_OrderDirection,
            "FilterRequestTypeHist": HistoryList_FilterRequestType,
            "FilterRequestDateHist": HistoryList_FilterRequestDate,
            "FilterRequestedDateHist": HistoryList_FilterRequestedDate,
            "FilterEmployeeHist": HistoryList_FilterEmployee,
            "FilterTreeHist": HistoryList_FilterTree,
            "FilterTreeUserHist": HistoryList_FilterTreeUser,
            "FilterIdCauseHist": HistoryList_IdCause,
            "FilterIdSupervisorHist": HistoryList_IdSupervisor,
            "FilterRequestStateHist": HistoryList_FilterRequestState,
            "FilterVersion": FilterVersion
        };
    }

    this.fillRequestObject = function (obj) {
        PendList_OrderField = obj.OrderFieldPend;
        PendList_OrderDirection = obj.OrderDirectionPend;
        PendList_FilterRequestType = obj.FilterRequestTypePend;
        PendList_FilterRequestDate = obj.FilterRequestDatePend;
        PendList_FilterRequestedDate = obj.FilterRequestedDatePend;
        PendList_FilterEmployee = obj.FilterEmployeePend;
        PendList_FilterTree = obj.FilterTreePend;
        PendList_FilterTreeUser = obj.FilterTreeUserPend;
        PendList_IdCause = obj.FilterIdCausePend;
        PendList_IdSupervisor = obj.FilterIdSupervisorPend;

        OtherList_OrderField = obj.OrderFieldOther;
        OtherList_OrderDirection = obj.OrderDirectionOther;
        OtherList_FilterRequestType = obj.FilterRequestTypeOther;
        OtherList_FilterRequestDate = obj.FilterRequestDateOther;
        OtherList_FilterRequestedDate = obj.FilterRequestedDateOther;
        OtherList_FilterEmployee = obj.FilterEmployeeOther;
        OtherList_FilterTree = obj.FilterTreeOther;
        OtherList_FilterTreeUser = obj.FilterTreeUserOther;
        OtherList_IdCause = obj.FilterIdCauseOther;
        OtherList_IdSupervisor = obj.FilterIdSupervisorOther;
        OtherList_FilterLevels = obj.FilterLevelsOther;
        OtherList_FilterDaysFrom = obj.FilterDaysFromOther;

        HistoryList_OrderField = obj.OrderFieldHist;
        HistoryList_OrderDirection = obj.OrderDirectionHist;
        HistoryList_FilterRequestType = obj.FilterRequestTypeHist;
        HistoryList_FilterRequestDate = obj.FilterRequestDateHist;
        HistoryList_FilterRequestedDate = obj.FilterRequestedDateHist;
        HistoryList_FilterEmployee = obj.FilterEmployeeHist;
        HistoryList_FilterTree = obj.FilterTreeHist;
        HistoryList_FilterTreeUser = obj.FilterTreeUserHist;
        HistoryList_IdCause = obj.FilterIdCauseHist;
        HistoryList_IdSupervisor = obj.FilterIdSupervisorHist;
        HistoryList_FilterRequestState = obj.FilterRequestStateHist;

        if (typeof obj.FilterVersion != 'undefined' && obj.FilterVersion != CurrentFilterVersion) {
            obj = this.buildRequestObject();
            obj.saveInCookie();
        }
    }

    this.saveInCookie = async function () {
        if (getStorageKey("TreeState_RequestListParams") == null || !loaded) return;

        let objTreeState = this.buildRequestObject();

        try {
            let bNeedToSave = true;
            let bInProcessToServer = false;
            let cookieStorageItem = null;

            if (getStorageKey("TreeState_RequestListParams") !== null) {
                cookieStorageItem = JSON.parse(getStorageKey("TreeState_RequestListParams"));
                bInProcessToServer = cookieStorageItem.sendToServer;

                let sItem = JSON.stringify(cookieStorageItem.item);
                let nItem = JSON.stringify(objTreeState);
                
                if (sItem == nItem) bNeedToSave = false;
            }

            setStorageKey("TreeState_RequestListParams", JSON.stringify({ item: objTreeState, timestamp: moment().format("YYYYMMDDHHmmss"), sendToServer: (bNeedToSave || bInProcessToServer) }));

        }
        catch (e) {
            showError("roRequestListParams::saveInCookie", e);
        }
    }

    this.loadFromServer = async function () {
        try {
            eraseCookie("TreeState_RequestListParams");

            let cookieValue = null;

            try {
                // Configurar la solicitud POST
                let response = await fetch(serverURL + "/GetCookie", {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify({ sCookieName: "TreeState_RequestListParams" })
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


            if (cookieValue != null) {
                let obj = cookieValue;

                this.fillRequestObject(obj);

                setStorageKey("TreeState_RequestListParams", JSON.stringify({ item: this.buildRequestObject(), timestamp: moment().format("YYYYMMDDHHmmss"), sendToServer: false }));

                return obj;
            }
        } catch (e) {
            showError("roRequestListParams::getFromCookie", e);
        } finally {
            loaded = true;
        }
    }
    this.getFromCookie = async function () {
        try {
            eraseCookie("TreeState_RequestListParams");

            let cookieValue = null;
            let cookieStorageItem = null;
            let loadFromServer = true;
            let bIsSendingToServer = false;

            if (getStorageKey("TreeState_RequestListParams") !== null) {
                cookieStorageItem = JSON.parse(getStorageKey("TreeState_RequestListParams"));
                bIsSendingToServer = cookieStorageItem.sendToServer;
                loadFromServer = false;

                let cookieCreatedAt = moment(cookieStorageItem.timestamp, "YYYYMMDDHHmmss");
                let now = moment();

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
                        body: JSON.stringify({ sCookieName: "TreeState_RequestListParams" })
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
                let obj = cookieValue;

                this.fillRequestObject(obj);

                setStorageKey("TreeState_RequestListParams", JSON.stringify({ item: this.buildRequestObject(), timestamp: moment().format("YYYYMMDDHHmmss"), sendToServer: bIsSendingToServer }));

                return obj;
            }
        } catch (e) {
            showError("roRequestListParams::getFromCookie", e);
        } finally {
            loaded = true;
        }
    
    }

    this.reset = async function () {
        eraseCookie("TreeState_RequestListParams");

        setStorageKey("TreeState_RequestListParams", null);
        roRequestParamsHash["TreeState_RequestListParams"] = null;
    }
}