function DataAccessFilter(oParms, oFieldSort, oSortOrder) {
    var arrStatus;

    var oParms; //Parametres (JSON)

    var arrGrids; //Array Grids Accessos
    var jsGridAccess; //Grid Accessos

    var strSortedField;
    var strSortOrder;

    //*****************************************************************************************/
    // refreshGrid
    // Carrega els grids
    //********************************************************************************************/
    this.refreshGrid = function (oParms, oFieldSort, oSortOrder) {
        try {
            if (oFieldSort != null) { strSortedField = oFieldSort; } else { strSortedField = ""; }
            if (oSortOrder != null) { strSortOrder = oSortOrder; } else { strSortOrder = ""; }

            //Esborrem els grids
            if (jsGridAccess != null) { jsGridAccess.destroyGrid(); }

            var strParms = "";
            strParms += "&Employees=" + encodeURIComponent(oParms.Employees);

            strParms += "&FilterTree=" + encodeURIComponent(oParms.FilterTree);
            strParms += "&FilterTreeUser=" + encodeURIComponent(oParms.FilterTreeUser);

            strParms += "&Zones=" + encodeURIComponent(oParms.Zones);
            strParms += "&DateBegin=" + encodeURIComponent(oParms.DateBegin);
            strParms += "&DateEnd=" + encodeURIComponent(oParms.DateEnd);
            strParms += "&HourBegin=" + encodeURIComponent(oParms.HourBegin);
            strParms += "&HourEnd=" + encodeURIComponent(oParms.HourEnd);

            if (strSortedField != "") { strParms += "&SortField=" + encodeURIComponent(strSortedField); }
            if (strSortOrder != "") { strParms += "&SortOrder=" + encodeURIComponent(strSortOrder); }

            AsyncCall("POST", "srvAccessFilter.aspx?action=getXGridsJSON" + strParms, "json", "arrGrids", "createAccessFilterGrids(arrGrids,'" + strSortedField + "','" + strSortOrder + "');")
        } catch (e) { showError("refreshGrid", e); }
    }                     //end function

    //*****************************************************************************************/
    // createAccessFilterGrids
    // Carrega del Grid
    //********************************************************************************************/
    createAccessFilterGrids = function (arrCC, oFieldSort, oSortOrder) {
        try {
            arrGrids = arrCC;

            if (arrGrids == null) { return; }
            if (arrGrids.length == 0) { return; }

            if (arrGrids[0].error == "true") {
                checkStatus(arrGrids);
                return;
            }

            //Creació dels grids
            createGridAccess(arrGrids[0].access, oFieldSort, oSortOrder);
        } catch (e) { showError('createAccessFilterGrids', e); }
    }                          //end function

    //*****************************************************************************************/
    // createGridAccess
    // Carrega del Grids (Accessos)
    //********************************************************************************************/
    createGridAccess = function (arrGridAccess, oFieldSort, oSortOrder) {
        try {
            var hdGridAccess = [{ 'fieldname': 'ZoneName', 'description': '', 'size': '130px' },
            { 'fieldname': 'EmployeeName', 'description': '', 'size': '170px' },
            { 'fieldname': 'DateTime', 'description': '', 'size': '80px' },
            { 'fieldname': 'IDCapture', 'description': '', 'size': '10px', html:true }
            ];

            hdGridAccess[0].description = document.getElementById('hdnHdrZoneName').value;
            hdGridAccess[1].description = document.getElementById('hdnHdrEmployeeName').value;
            hdGridAccess[2].description = document.getElementById('hdnHdrDateTime').value;
            hdGridAccess[3].description = "&nbsp;"

            jsGridAccess = new jsGrid('grdAccess', hdGridAccess, arrGridAccess, false, false, false, 'Access', true, true, oFieldSort, oSortOrder);
        } catch (e) { showError("createGridAccess", e); }
    }

    //*****************************************************************************************/
    // checkStatus
    // Retorn d'objecte Status de la crida Ajax correcte
    //********************************************************************************************/
    checkStatus = function (oStatus) {
        try {
            //Carreguem el array global per mantenir els valors
            arrStatus = oStatus;
            objError = arrStatus[0];

            //Si es un error, mostrem el missatge
            if (objError.error == "true") {
                if (objError.typemsg == "1") { //Missatge estil pop-up
                    var url = "Access/srvMsgBoxAccess.aspx?action=Message&TitleKey=SaveName.Error.Text&" +
                        "DescriptionText=" + objError.msg + "&" +
                        "Option1TextKey=SaveName.Error.Option1Text&" +
                        "Option1DescriptionKey=SaveName.Error.Option1Description&" +
                        "Option1OnClickScript=HideMsgBoxForm(); return false;&" +
                        "IconUrl=~/Base/Images/MessageFrame/stock_dialog-error.png";
                    parent.ShowMsgBoxForm(url, 400, 300, '');
                } else { //Missatge estil inline
                }
                //hasChanges(true);
            }
        } catch (e) { showError("checkStatus", e); }
    }             //end checkStatus function

    //*****************************************************************************************/
    // setAccessFilterParams
    // Recupera Params del AccessFilter
    //********************************************************************************************/
    this.setParams = function (oValue) {
        try {
            oParams = oValue;
        } catch (e) {
            showError('setParams', e);
        }
    }

    if (oFieldSort != null) { strSortedField = oFieldSort; } else { strSortedField = ""; }
    if (oSortOrder != null) { strSortOrder = oSortOrder; } else { strSortOrder = ""; }

    //Si es passen parms, carreguem grid
    if (oParms != null) {
        this.refreshGrid(oParms, strSortedField, strSortOrder);
    }
}