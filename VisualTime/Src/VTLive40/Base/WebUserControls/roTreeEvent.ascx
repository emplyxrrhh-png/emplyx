<%@ Control Language="VB" AutoEventWireup="false" Inherits="VTLive40.Base_WebUserControls_roTreeEvent" CodeBehind="roTreeEvent.ascx.vb" %>
<input type="hidden" id="hdnStrName" value='<%= Me.Language.Translate("ColumnName", "roTreeEventV3")%>' />

<div style="height: 100%; width: 100%;">

    <div style="float: left; height: 100%; width: 45%; text-align: left; border: solid 1px silver; background-color: white;">
        <table width="100%" style="height: 100%" cellpadding="0" cellspacing="0" border="0">
            <tr>
                <td valign="top" style="height: 50px;">
                    <span style="display: block; padding-left: 3px; padding-top: 4px; padding-bottom: 2px; color: #666666;">
                        <asp:Label ID="lblFindText" runat="server" Text="Buscar por:" Font-Bold="True"></asp:Label>
                    </span>
                    <!-- Combo Agrupació -->
                    <div style="width: 98%; padding: 2px; text-align: left; height: 16px;">
                        <roWebControls:roComboBox runat="server" ID="cmbFieldFind" Width="100%" AutoResizeChildsWidth="true" ItemsRunAtServer="false" ChildsVisible="4" ParentWidth="100%" ForeColor="#666666" />
                    </div>
                </td>
            </tr>
            <tr>
                <td valign="top" style="height: 25px;">
                    <div style="width: 98%; padding-left: 2px; text-align: left;">
                        <input type="text" id="<%= Me.ClientID %>_FieldFindValue" class="textClass" style="color: #666666; border: 1px solid silver; padding: 0px; width: 99%; height: 18px;" onkeypress="FieldFindChanged(event,'<%= Me.ClientID %>');" />
                    </div>
                </td>
            </tr>
            <tr>
                <td id="<%= Me.ClientID %>_td_FF" valign="top" width="100%" style="height: 100%; border-top: solid 1px #BFBFBF;">
                    <div id="<%= Me.ClientID %>_tree-div3" style="overflow: auto; text-align: left; width: 100%; height: 100%;"></div>
                </td>
            </tr>
        </table>
    </div>

    <div style="float: left; height: 100%; width: 9%; text-align: center; background-color: Transparent;">
        <a href="javascript: void(0)" style="cursor: pointer; position: relative; top: 100px;" onclick="<%= Me.ClientID %>_newItemInGrid();" title='<%= Me.Language.Translate("ColumnName", "roTreeEvent.AddNew")%>'>
            <img alt="" src="<%= Me.Page.ResolveURL("~/Base/Images/tree_select.png") %>" style="border: 1px solid #C0C0C0; padding: 5px;" />
        </a>

        <a href="javascript: void(0)" style="cursor: pointer; position: relative; top: 150px;" onclick="<%= Me.ClientID %>_deleteAll();" title='<%= Me.Language.Translate("ColumnName", "roTreeEvent.DeleteAll")%>'>
            <img alt="" src="<%= Me.Page.ResolveURL("~/Base/Images/Grid/trash.png") %>" style="border: 1px solid #C0C0C0; padding: 5px;" />
        </a>
    </div>

    <div id="<%= Me.ClientID %>_divGrid" style="float: right; height: 100%; width: 45%; text-align: left; border: solid 1px silver; background-color: white;">
        <div id="<%= Me.ClientID %>_grdLista" style="overflow: auto; height: 100%; width: 100%">
        </div>
    </div>

    <div style="clear: both" />
</div>

<script language="javascript" type="text/javascript">

    eval("var <%= Me.ClientID %>_Grid;"); //control Grid
    eval("<%= Me.ClientID %>_Grid = null;");
    eval("<%= Me.ClientID %>_createGrid()");
    eval('<%= Me.ClientID %>_resizeControls()');

    function <%= Me.ClientID %>_newItemInGrid() {

        if (<%= Me.ClientID %>_Grid == null) {
            eval('<%= Me.ClientID %>_createGrid()');
        }

        var tmpId = getSelectedNodeId();
        if (tmpId != "") {
            var tmpName = getSelectedNodeName();

            //Bucle per les files del grid
            var rows = <%= Me.ClientID %>_Grid.toJSONStructureAdvanced();
            if (typeof (rows) != "undefined" && rows.length > 0) {
                var strKey = tmpId + "-" + tmpName;
                for (var n = 0; n < rows.length; n++) {
                    var strKeyToSearch = rows[n].id + "-" + rows[n].name;
                    if (strKeyToSearch == strKey) {
                        return;
                    }
                }
            }

            var tmpType = getTypeSearchSelected();

            var tmpImg = '';
            if (tmpType == 'event')
                tmpImg = "<img src='" + '<%= Me.Page.ResolveUrl("~/Base/Images/EventSelector/Event16.png")%>' + "' />";
            else
                tmpImg = "<img src='" + '<%= Me.Page.ResolveUrl("~/Base/Images/EventSelector/Event16.png")%>' + "' />";

            arrValues = [{ field: 'id', value: tmpId }, { field: 'type', value: tmpType }, { field: 'icon', value: tmpImg }, { field: 'name', value: tmpName }];
            <%= Me.ClientID %>_Grid.createRow(arrValues, null);

            //Guardar seleccion en la cookie y lanzar funcion externa si esta definida
            setSelectedInCookie();
        }
    }

    //** CREA EL GRID VACIO ***********************************************************************************/
    function <%= Me.ClientID %>_createGrid() {
        try {

            var headerGrid = [{ 'fieldname': 'id', 'description': '', 'size': '-1' },
                { 'fieldname': 'type', 'description': '', 'size': '-1' },
                { 'fieldname': 'icon', 'description': '', 'size': '10%', html: true },
                { 'fieldname': 'name', 'description': '', 'size': '90%' }]
            headerGrid[0].description = ""  //Id;
            headerGrid[1].description = ""  //Tipo
            headerGrid[2].description = ""; //Icono
            headerGrid[3].description = document.getElementById('hdnStrName').value; //Nombre

            <%= Me.ClientID %>_Grid = new jsGrid("<%= Me.ClientID %>_grdLista", headerGrid, null, false, true, false, "<%= Me.ClientID %>TreeEvent");

        }
        catch (e) {
            showError("<%= Me.ClientID %>_createGrid", e);
        }
    }

    //*** BORRA UN ELEMENTO **********************************************************************/
    function deleteGrid<%= Me.ClientID %>TreeEvent(sId) {
        try {
            //borrar la fila del grid
            <%= Me.ClientID %>_Grid.deleteRow(sId);

            //Guardar seleccion en la cookie y lanzar funcion externa si esta definida
            setSelectedInCookie();
        }
        catch (e) {
            showError("deleteGrid<%= Me.ClientID %>TreeEvent", e);
        }
    }

    //** BORRA TODOS LOS ELEMENTOS *****************************************************************/
    function <%= Me.ClientID %>_deleteAll() {
        try {
            eval('<%= Me.ClientID %>_createGrid()');

            //Guardar seleccion en la cookie y lanzar funcion externa si esta definida
            setSelectedInCookie();

        }
        catch (e) {
            showError("<%= Me.ClientID %>_deleteAll", e);
        }
    }

    function setSelectedInCookie() {
        var objSelected = ["", ""];
        var tmpItemsSelected = <%= Me.ClientID %>_Grid.toJSONStructureAdvanced();
        if (typeof (tmpItemsSelected) != "undefined" && tmpItemsSelected.length > 0) {
            for (var n = 0; n < tmpItemsSelected.length; n++) {
                if (tmpItemsSelected[n].type == 'event') {
                    objSelected[0] = objSelected[0] + encodeURIComponent(tmpItemsSelected[n].id) + ",";
                }
                else {
                    objSelected[1] = objSelected[1] + encodeURIComponent(tmpItemsSelected[n].id) + ",";
                }
            }
            if (objSelected[0].charAt(objSelected[0].length - 1) == ",") objSelected[0] = objSelected[0].substring(0, objSelected[0].length - 1);
            if (objSelected[1].charAt(objSelected[1].length - 1) == ",") objSelected[1] = objSelected[1].substring(0, objSelected[1].length - 1);
        }

        var oTreeState = getroTreeState("<%= Me.PrefixCookie %>");
        oTreeState.setSelected(objSelected[0], "1");
        oTreeState.setSelected(objSelected[1], "2");

        LanzaFuncionExterna(objSelected);
    }

    function <%= Me.ClientID %>_LoadGridFromCookie(arrGridParam) {

        if (<%= Me.ClientID %>_Grid == null) {
            eval('<%= Me.ClientID %>_createGrid()');
        }

        var tmpParam = arrGridParam[0];
        <%= Me.ClientID %>_Grid.addRows(tmpParam, "", null);

        setSelectedInCookie();
    }

    function LanzaFuncionExterna(arrObjSelected) {
        var functName = "<%= Me.AfterSelectFuncion %>";
        if (functName != "") {
            try {
                eval(functName + '(arrObjSelected[0],arrObjSelected[1]);');
            }
            catch (e) { }
        }
    }

    function getSelectedFromCookie() {
        var oTreeState = getroTreeState("<%= Me.PrefixCookie %>");

        var EventValues = oTreeState.getSelected('1');

        if (typeof (oTreeState.getSelected('2')) != 'undefined' && oTreeState.getSelected('2') != "") {
            if (EventValues != "") {
                EventValues = EventValues + "," + oTreeState.getSelected('2');
            } else {
                EventValues = oTreeState.getSelected('2');
            }
        }

        var path = '<%= Me.Page.ResolveUrl("~/Base/WebUserControls/EventSelectorData.aspx")%>' + "?action=LoadInitialData&EventValues=" + EventValues;
        AsyncCall("POST", path, "json", "arrGrid", "<%= Me.ClientID %>_LoadGridFromCookie(arrGrid);");
    }

    function <%= Me.ClientID %>_loadGridIni() {
        try {

            getSelectedFromCookie();

        }
        catch (e) {
            showError("<%= Me.ClientID %>_loadGridIni", e);
        }
    }

    function <%= Me.ClientID %>_resizeControls() {
        var parent = document.getElementById('<%= Me.ClientID %>_td_FF');
        var child = document.getElementById('<%= Me.ClientID %>_tree-div3');
        child.style.height = parent.clientHeight + "px";
        child.style.width = parent.clientWidth + "px";

        parent = document.getElementById('<%= Me.ClientID %>_divGrid');
        child = document.getElementById('<%= Me.ClientID %>_grdLista');
        child.style.height = parent.clientHeight + "px";
        child.style.width = parent.clientWidth + "px";
    }

    //INSTRUCCION INICIAL
    window.onload = function () {
        eval('<%= Me.ClientID %>_loadGridIni()');
    }

    window.onresize = function () {
        eval('<%= Me.ClientID %>_resizeControls()');
    }
</script>

<script language="javascript" type="text/javascript">

    eval("var <%= Me.ClientID %>_roTreeEvent;"); //Variable Global i dinamica de l'arbre

    eval("var <%= Me.ClientID %>_PathEvent;"); //Path d'acces
    eval("<%= Me.ClientID %>_PathEvent = '<%= Me.ResolvePathEvent%>';");

    eval("var <%= Me.ClientID %>_FieldFindColumnEvent;");
    eval("<%= Me.ClientID %>_FieldFindColumnEvent = '';");

    eval("var <%= Me.ClientID %>_FieldFindValueEvent;");
    eval("<%= Me.ClientID %>_FieldFindValueEvent = '';");

    eval("var <%= Me.ClientID %>_IdSelectedNode;");
    eval("<%= Me.ClientID %>_IdSelectedNode= '';");

    eval("var <%= Me.ClientID %>_NameSelectedNode;");
    eval("<%= Me.ClientID %>_NameSelectedNode= '';");

    <%= Me.ClientID %>_roTreeEvent = new roTreeEvent('<%= Me.ClientID %>', document.getElementById('<%= Me.ClientID %>' + '_tree-div3'));

    //=========================================================
    function FieldFindChanged(e, objPrefix) {
        var FieldFind = getFieldFind(objPrefix);
        setFieldFind(FieldFind[0], document.getElementById(objPrefix + '_FieldFindValue').value, objPrefix);
        tecla = (document.all) ? e.keyCode : e.which;
        if (tecla == 13) {
            clearSelectedNode();
            eval(objPrefix + "_LoadTree(objPrefix);");
            if (e.preventDefault) {
                e.preventDefault();
            } else {
                e.returnValue = false;
            }
        }
    }

    //=========================================================
    function <%= Me.ClientID %>_LoadTree(objPrefix) {
        var oTr = document.getElementById(objPrefix + '_tree-div3');
        oTr.innerHTML = '';
        var FieldFind = getFieldFind(objPrefix);
        if (FieldFind[1] != '') {
            var myMask = new Ext.LoadMask(objPrefix + '_tree-div3', { msg: "..." });
            myMask.show();

            <%= Me.ClientID %>_roTreeEvent = new roTreeEvent('<%= Me.ClientID %>', document.getElementById('<%= Me.ClientID %>' + '_tree-div3'));
        }
    }

    //=========================================================
    function setFieldFindALL(FieldFindColumn, FieldFindValue, objPrefix) {
        setFieldFind(FieldFindColumn, FieldFindValue, objPrefix);
        document.getElementById(objPrefix + '_FieldFindValue').value = '';
        clearSelectedNode();
        eval(objPrefix + '_LoadTree(objPrefix)');
        return false;
    }

    //=========================================================
    function getFieldFind(objPrefix) {
        var FieldFind = new Array(2);
        var strColumn = <%= Me.ClientID %>_FieldFindColumnEvent;
        if (strColumn == '' || strColumn == 'undefined' || strColumn == null) strColumn = 'year';
        FieldFind[0] = strColumn;

        var strValue = <%= Me.ClientID %>_FieldFindValueEvent;
        if (strValue == 'undefined' || strValue == null) strValue = '';
        FieldFind[1] = strValue;

        return FieldFind;
    }

    //=========================================================
    function setFieldFind(FieldFindColumn_, FieldFindValue_, objPrefix) {
        <%= Me.ClientID %>_FieldFindColumnEvent = FieldFindColumn_;
        if (FieldFindValue_.charAt(0) == ' ') {
            FieldFindValue_ = '%20' + FieldFindValue_.substring(1, FieldFindValue_.length);
        }
        <%= Me.ClientID %>_FieldFindValueEvent = FieldFindValue_;
    }

    //=========================================================
    function setSelectedNode(id, name) {
        <%= Me.ClientID %>_IdSelectedNode = id;
        <%= Me.ClientID %>_NameSelectedNode = name;
    }

    //=========================================================
    function getSelectedNodeId() {
        return <%= Me.ClientID %>_IdSelectedNode;
    }

    function getSelectedNodeName() {
        return <%= Me.ClientID %>_NameSelectedNode;
    }

    //=========================================================
    function getTypeSearchSelected() {
        return <%= Me.ClientID %>_FieldFindColumnEvent;
    }

    function clearSelectedNode() {
        <%= Me.ClientID %>_IdSelectedNode = "";
        <%= Me.ClientID %>_NameSelectedNode = "";
    }
</script>