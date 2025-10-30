<%@ Control Language="VB" AutoEventWireup="false" Inherits="VTLive40.Base_WebUserControls_roTreeV3" CodeBehind="roTreeV3.ascx.vb" %>

<%@ Register Src="~/Base/WebUserControls/roTreeV3Prev.ascx" TagName="roTreeV3Prev" TagPrefix="rws" %>

<input type="hidden" id="hdnNodes" value="" runat="server" />
<input type="hidden" id="hdnListNodes" value="" runat="server" />

<input type="hidden" id="hdnStrtype" value='<%= Me.Language.Translate("ColumnType","roTreeV3") %>' />
<input type="hidden" id="hdnStrName" value='<%= Me.Language.Translate("ColumnName","roTreeV3") %>' />

<div style="height: 100%; width: 100%; padding: 1px">

    <div style="float: left; height: 100%; width: 45%; text-align: left; background-color: white;">
        <rws:roTreeV3Prev ID="objTreePrev" runat="server" />
    </div>

    <div style="float: left; height: 100%; width: 9%; vertical-align: middle; align-content: center; background-color: Transparent; overflow: hidden">
        <div style="text-align: center; width: 100%; height: 100%">
            <div style="width: 100%; align-content: center; clear: both; position: relative; top: 40px;">
                <a href="javascript: void(0)" style="cursor: pointer" onclick="<%= Me.ClientID %>_addNode();" title="<%= Me.Language.Translate("Button.AddNode","roTreeV3") %>">
                    <div class="treeSelect"></div>
                </a>
            </div>

            <div style="width: 100%; align-content: center; clear: both; position: relative; top: 80px;">
                <a href="javascript: void(0)" style="cursor: pointer" onclick="<%= Me.ClientID %>_addChildren();" title="<%= Me.Language.Translate("Button.AddChildren","roTreeV3") %>">
                    <div class="treeSelectChildren"></div>
                </a>
            </div>

            <div style="width: 100%; align-content: center; clear: both; position: relative; top: 120px;" id="<%= Me.CookieNameTreeV25 %>_selectAllTree">
                <a href="javascript: void(0)" style="cursor: pointer" onclick="<%= Me.ClientID %>_addAll();" title="<%= Me.Language.Translate("Button.AddAll", "roTreeV3") %>">
                    <div class="treeSelectAll"></div>
                </a>
            </div>

            <div style="width: 100%; align-content: center; clear: both; position: relative; top: 160px">
                <a href="javascript: void(0)" style="cursor: pointer;" onclick="<%= Me.ClientID %>_deleteAll();" title="<%= Me.Language.Translate("Button.DeleteAll","roTreeV3") %>">
                    <div class="treeTrash"></div>
                </a>
            </div>
        </div>
    </div>

    <div id="<%= Me.ClientID %>_divGrid" style="float: right; height: 100%; width: 45%; text-align: left; background-color: white;">
        <div id="<%= Me.ClientID %>_grdLista" style="overflow: auto; height: 100%; width: 100%">
        </div>
    </div>

    <div style="clear: both" />
</div>

<script language="javascript" type="text/javascript">

    async function <%= Me.ClientID %>_GetFilters() {
        await LanzaFuncionExterna();
    }

    var $<%= Me.ClientID %>_Progreso = $('<div class="ext-el-mask-msg x-mask-loading" id="ext-gen4" style="position:relative; top:-170px; left:60px; width:50px;"><div>...</div></div>')
        .hide();

    //Nodo seleccionado en el control roTree V.1
    var <%= Me.ClientID %>_SelectedNode = '';

    //control Grid
    var <%= Me.ClientID %>_Grid = null;

    async function EnProgreso(Modo) {
        var divGrid = document.getElementById('<%= Me.ClientID %>_divGrid');
        if (Modo == true) {
            $<%= Me.ClientID %>_Progreso.appendTo(divGrid).show();
        } else {
            $<%= Me.ClientID %>_Progreso.hide();
        }
    }

    //****************************************************************
    //** GUARDA EL ID DEL NODO SELECCIONADO DEL ARBOL AL HACER UN CLICK
    //****************************************************************
    async function cargaNodoV3(Nodo) {
        if (Nodo.id.substring(0, 1) == 'A' || Nodo.id.substring(0, 1) == 'B') {
            eval("<%= Me.ClientID %>_SelectedNode = Nodo.id");
        }
        else {
            if (Nodo.id.substring(0, 1) == 'C') {
                eval("<%= Me.ClientID %>_SelectedNode = Nodo.id");
            }
            else {
                eval("<%= Me.ClientID %>_SelectedNode = ''");
            }
        }
    }

    async function <%= Me.ClientID %>_cargaNodoV3(Nodo) {
        if (Nodo.id.substring(0, 1) == 'A' || Nodo.id.substring(0, 1) == 'B') {
            eval("<%= Me.ClientID %>_SelectedNode = Nodo.id");
        }
        else {
            if (Nodo.id.substring(0, 1) == 'C') {
                eval("<%= Me.ClientID %>_SelectedNode = Nodo.id");
            }
            else {
                eval("<%= Me.ClientID %>_SelectedNode = ''");
            }
        }
    }

    //****************************************************************
    //** AGREGA EL NODO SELECCIONADO EN EL GRID
    //****************************************************************
    async function <%= Me.ClientID %>_addNode() {
        try {
            var nodeTemp = <%= Me.ClientID %>_SelectedNode;
            if (nodeTemp != "")
                <%= Me.ClientID %>_newItemInGrid(nodeTemp);
        }
        catch (e) {
            showError("<%= Me.ClientID %>_addNode", e);
        }
    }

    //****************************************************************
    //** AGREGA EL NODO SELECCIONADO EN EL GRID
    //****************************************************************
    async function <%= Me.ClientID %>_addChildren() {
        try {
            var nodeTemp = <%= Me.ClientID %>_SelectedNode;
            if (nodeTemp != "") <%= Me.ClientID %>_newChildrenInGrid(nodeTemp);
        }
        catch (e) {
            showError("<%= Me.ClientID %>_addChildren", e);
        }
    }
    //****************************************************************
    //** AGREGA EL NODO SELECCIONADO EN EL GRID
    //****************************************************************
    async function <%= Me.ClientID %>_addAll() {
        try {
            var nodeTemp = <%= Me.ClientID %>_SelectedNode;
            if (nodeTemp != "") {
                var oTreeStateV25 = await getroTreeState("<%= Me.CookieNameTreeV25 %>");
                <%= Me.ClientID %>_newItemInGrid(<%= Me.CookieNameTreeV25 %>_roTrees.getAllRootNodes(oTreeStateV25.getActiveTreeType()));
            }
        }
        catch (e) {
            showError("<%= Me.ClientID %>_addAll", e);
        }
    }

    //****************************************************************
    //** GESTIONAR COOKIE DE LA SELECCION hdnNodes
    //****************************************************************
    async function setSelectedV3(strNodes, objPrefix) {
        var oTreeState = await getroTreeState(objPrefix);
        if (strNodes == null) strNodes = "";
        oTreeState.setSelected(strNodes, "1");
    }

     async function getSelectedV3(TreeType, objPrefix) {
        var oTreeState = await getroTreeState(objPrefix);
        return oTreeState.getSelected(TreeType);
    }

    async function <%= Me.ClientID %>_deleteSelectedNode(sId, objNodes) {

        var hdnNodes = document.getElementById("<%= Me.ClientID %>_hdnNodes");
        var hdnListNodes = document.getElementById("<%= Me.ClientID %>_hdnListNodes");

        var arrNodes = hdnNodes.value.split(",");
        var pos = -1;
        for (var n = 0; n < arrNodes.length; n++) {
            if (arrNodes[n] == sId) pos = n;
        }
        if (pos != -1) arrNodes.splice(pos, 1);

        hdnNodes.value = arrNodes.join(",");

        if (objNodes != null)
            hdnListNodes.value = JSON.stringify(objNodes);
        else
            hdnListNodes.value = "";

        var objPrefix = "<%= Me.PrefixCookie %>";
        await setSelectedV3(hdnNodes.value, objPrefix);

        //Lanzar funcion definida por el usuario
        await LanzaFuncionExterna();
    }

    async function LanzaFuncionExterna() {
        var functName = "<%= Me.AfterSelectFuncion %>";
        if (functName != "") {
            try {
                var hdnNodes = document.getElementById("<%= Me.ClientID %>_hdnNodes");
                //obtener cookie de arbol v2.5
                var oTreeStateV25 = await getroTreeState("<%= Me.CookieNameTreeV25 %>");
                var sFilterUser;
                var sFilter = oTreeStateV25.getFilter()[0];
                if (sFilter.substring(4, 5) == "1")
                    sFilterUser = oTreeStateV25.getFilter()[1];
                else
                    sFilterUser = "";

                eval(functName + '(hdnNodes.value, sFilter, sFilterUser);');
            } catch (e) { }
        }
    }

    async function <%= Me.ClientID %>_addSelectedNodes(strNodes, objNodes) {

        var hdnNodes = document.getElementById("<%= Me.ClientID %>_hdnNodes");
        var hdnListNodes = document.getElementById("<%= Me.ClientID %>_hdnListNodes");

        if (strNodes == null) strNodes = "";
        hdnNodes.value = strNodes;

        if (objNodes != null)
            hdnListNodes.value = JSON.stringify(objNodes);
        else
            hdnListNodes.value = "";

        var objPrefix = "<%= Me.PrefixCookie %>";
        await setSelectedV3(hdnNodes.value, objPrefix);

        //Lanzar funcion definida por el usuario
        await LanzaFuncionExterna();
    }

    //****************************************************************
    //****************************************************************

    //****************************************************************
    //** ELIMINA EL NODO SELECCIONADO EN EL GRID
    //****************************************************************
    async function deleteGrid<%= Me.ClientID %>TreeV3(sId) {
        try {
            //obtener la fila del grid
            var tmpRow = <%= Me.ClientID %>_Grid.retRowJSON(sId);

            //borrar la fila del grid
            <%= Me.ClientID %>_Grid.deleteRow(sId);

            //obtener lista del nodos del grid
            var arrRows = <%= Me.ClientID %>_Grid.toJSONStructureAdvanced();

            //eliminarlo de los seleccionados
            <%= Me.ClientID %>_deleteSelectedNode(tmpRow[0].value, arrRows)

        }
        catch (e) {
            showError("deleteGrid<%= Me.ClientID %>TreeV3", e);
        }
    }

    //****************************************************************
    //** ELIMINA TODO LOS NODOS DEL GRID
    //****************************************************************
    async function <%= Me.ClientID %>_deleteAll() {
        try {

            eval('<%= Me.ClientID %>_loadGrid(null)');

            //Lanzar funcion definida por el usuario
            await LanzaFuncionExterna();

        }
        catch (e) {
            showError("<%= Me.ClientID %>_deleteAll", e);
        }
    }

    async function <%= Me.ClientID %>_newItemInGrid(sId) {
        try {

            EnProgreso(true);

            var arrRows = "";
            if (typeof sId == "undefined" || sId == null) sId = "";

            //obtener grid
            if (<%= Me.ClientID %>_Grid != null) {
                arrRows = <%= Me.ClientID %>_Grid.toJSONStructureAdvanced();
                if (arrRows == "undefined" || arrRows == null)
                    arrRows = "";
                else {
                    //quitar los iconos antes de enviarlos
                    for (var n = 0; n < arrRows.length; n++) {
                        arrRows[n].icono = "";
                    }

                    arrRows = JSON.stringify(arrRows);
                    arrRows = encodeURIComponent(arrRows);
                }
            }

            var oTreeStateV25 = await getroTreeState("<%= Me.CookieNameTreeV25 %>");
            var sFilterUser;
            var sFilter = oTreeStateV25.getFilter()[0];
            if (sFilter.substring(4, 5) == "1") {
                sFilterUser = oTreeStateV25.getFilter()[1];
            } else {
                sFilterUser = "";
            }

            var path = '<%= Me.Page.ResolveURL("~/Base/WebUserControls/roTreeV3srv.aspx")%>' + "?action=newItemInGrid&ID=" + encodeURIComponent(sId) + "&rows=" + arrRows + "&feature=<%= Me.FeatureAlias %>" +
                "&filter=" + sFilter + "&filterUser=" + encodeURIComponent(sFilterUser) + ""
            AsyncCall("POST", path, "json", "arrGrid", "<%= Me.ClientID %>_loadGrid(arrGrid);");

        }
        catch (e) {
            EnProgreso(false);
            showError("<%= Me.ClientID %>_newItemInGrid", e);
        }
    }

    async function <%= Me.ClientID %>_newChildrenInGrid(sId) {
        try {

            EnProgreso(true);

            var arrRows = "";
            if (typeof sId == "undefined" || sId == null) sId = "";

            //obtener grid
            if (<%= Me.ClientID %>_Grid != null) {
                arrRows = <%= Me.ClientID %>_Grid.toJSONStructureAdvanced();
                if (arrRows == "undefined" || arrRows == null) {
                    arrRows = "";
                } else {
                    //quitar los iconos antes de enviarlos
                    for (var n = 0; n < arrRows.length; n++) {
                        arrRows[n].icono = "";
                    }

                    arrRows = JSON.stringify(arrRows);
                    arrRows = encodeURIComponent(arrRows);
                }
            }

            var oTreeStateV25 = await getroTreeState("<%= Me.CookieNameTreeV25 %>");
            var sFilterUser;
            var sFilter = oTreeStateV25.getFilter()[0];
            if (sFilter.substring(4, 5) == "1")
                sFilterUser = oTreeStateV25.getFilter()[1];
            else
                sFilterUser = "";

            var path = '<%= Me.Page.ResolveURL("~/Base/WebUserControls/roTreeV3srv.aspx")%>' + "?action=newChildrenInGrid&ID=" + encodeURIComponent(sId) + "&rows=" + arrRows + "&feature=<%= Me.FeatureAlias %>" +
                "&filter=" + sFilter + "&filterUser=" + encodeURIComponent(sFilterUser)
            AsyncCall("POST", path, "json", "arrGrid", "<%= Me.ClientID %>_loadGrid(arrGrid);");

        }
        catch (e) {
            EnProgreso(false);
            showError("<%= Me.ClientID %>_newItemInGrid", e);
        }
    }

    //*****************************************************************************************/
    // loadGrid
    // carga grid
    //*****************************************************************************************/
    async function <%= Me.ClientID %>_loadGrid(arrGridParam) {
        try {

            EnProgreso(true);

            if (arrGridParam == null) {
                eval("<%= Me.ClientID %>_createGrid(null)");
            }
            else {
                if (arrGridParam[0].error == "true") return;
                eval("<%= Me.ClientID %>_createGrid(arrGridParam[0], null)");
            }

            var strNodes = "";
            var objNodes = null;

            if (arrGridParam != null) {
                objNodes = arrGridParam[0];
                for (var n = 0; n < objNodes.length; n++) {
                    strNodes += objNodes[n].fields[0].value + ",";
                }
                if (strNodes.charAt(strNodes.length - 1) == ",") strNodes = strNodes.substring(0, strNodes.length - 1);
            }

            eval("<%= Me.ClientID %>_addSelectedNodes(strNodes, objNodes)");

        }
        catch (e) {
            showError("<%= Me.ClientID %>_loadGrid", e);
        }
        finally {
            EnProgreso(false);
        }
    }

    async function <%= Me.ClientID %>_createGrid(arrGridParam) {
        try {

            var headerGrid = [{ 'fieldname': 'id', 'description': '', 'size': '-1' },
            { 'fieldname': 'ruta', 'description': '', 'size': '-1' },
            { 'fieldname': 'tipo', 'description': '', 'size': '-1' },
            { 'fieldname': 'nombrefull', 'description': '', 'size': '-1' },
            { 'fieldname': 'icono', 'description': '', 'size': '10%', html: true },
            { 'fieldname': 'nombre', 'description': '', 'size': '90%' }]

            headerGrid[0].description = "Id";
            headerGrid[1].description = "Path";
            headerGrid[2].description = document.getElementById('hdnStrtype').value; //Tipo
            headerGrid[3].description = "";
            headerGrid[4].description = "";
            headerGrid[5].description = document.getElementById('hdnStrName').value; //Nombre

            var edtRow = false;
            var delRow = true;

            if (arrGridParam != null) {

                var imgGrup = '<%= Me.Page.ResolveURL("~/Base/Images/EmployeeSelector/Grupos-16x16.Gif") %>';
                var imgEmp = '<%= Me.Page.ResolveURL("~/Base/Images/EmployeeSelector/Empleado-16x16.gif") %>';

                for (var n = 0; n < arrGridParam.length; n++) {
                    if (arrGridParam[n].fields[1].value == "G") {
                        var newField = new Object;
                        newField.field = "icono";

                        newField.value = "<img src='" + imgGrup + "' />";
                        arrGridParam[n].fields.splice(2, 0, newField);
                    }
                    else if (arrGridParam[n].fields[1].value == "E") {
                        var newField = new Object;
                        newField.field = "icono";
                        newField.value = "<img src='" + imgEmp + "' />";
                        arrGridParam[n].fields.splice(2, 0, newField);
                    }
                }
            }

            <%= Me.ClientID %>_Grid = new jsGrid("<%= Me.ClientID %>_grdLista", headerGrid, arrGridParam, edtRow, delRow, false, "<%= Me.ClientID %>TreeV3");

            var tmpGridRows = eval('<%= Me.ClientID %>_Grid.getRows()');
            if (tmpGridRows.length > 0) {
                for (var n = 0; n < tmpGridRows.length; n++) {
                    var item = tmpGridRows[n];
                    if (typeof (tmpGridRows[n].cells[4].childNodes[0]) != 'undefined') {
                        item.setAttribute("title", tmpGridRows[n].cells[4].childNodes[0].nodeValue);
                    } else {
                        item.setAttribute("title", tmpGridRows[n].cells[3].childNodes[0].nodeValue);
                    }
                }
            }

        }
        catch (e) {
            showError("<%= Me.ClientID %>_createGrid", e);
        }
    }

    async function <%= Me.ClientID %>_loadGridIni() {
        try {

            var hdnNodes = document.getElementById("<%= Me.ClientID %>_hdnNodes");

            var objPrefix = "<%= Me.PrefixCookie %>";

            hdnNodes.value = await getSelectedV3("1", objPrefix);
            if (hdnNodes.value != "") {
                eval('<%= Me.ClientID %>_newItemInGrid(hdnNodes.value)');
            }
            else {
                eval('<%= Me.ClientID %>_loadGrid(null)');
            }
        }
        catch (e) {
            showError("<%= Me.ClientID %>_loadGridIni", e);
        }
    }

    //INSTRUCCION INICIAL
    window.onload = function () {
        eval('<%= Me.ClientID %>_loadGridIni()');
    }
</script>