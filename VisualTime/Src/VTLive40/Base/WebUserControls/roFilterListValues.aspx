<%@ Page Language="VB" AutoEventWireup="false" Inherits="VTLive40.Base_WebUserControls_roFilterListValues" CodeBehind="roFilterListValues.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html lang="en" class="notranslate" translate="no" xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Selector de ${Tasks}</title>
</head>

<body class="bodyPopup">
    <form id="frmTaskSelector" runat="server">

        <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="True" EnableScriptLocalization="True" />

        <asp:HiddenField ID="hdnCanClose" runat="server" />
        <input type="hidden" id="hdnSelectedValue" value="-1" runat="server" />
        <input type="hidden" id="hdnSelectedName" value="" runat="server" />
        <input type="hidden" id="hdnIDField" value="" runat="server" />

        <div style="width: 100%; height: 100%;">
            <table cellpadding="0" cellspacing="0" style="width: 100%; height: 100%;">
                <tr>
                    <td>
                        <div class="panHeader2">
                            <asp:Label ID="lblShiftSelectorTitle" Text="Selector de valores" CssClass="panHeaderLabel" runat="server" />
                        </div>
                    </td>
                </tr>
                <tr id="trList">
                    <td>
                        <div id="divFilterList" class="frameFilterList">
                            <div id="grdLista" class="frameFilterSelector">
                            </div>
                        </div>
                    </td>
                </tr>
                <tr style="height: 35px;">
                    <td align="right">
                        <table>
                            <tr>
                                <td>
                                    <asp:Button ID="btAccept" Text="${Button.Accept}" runat="server" CssClass="btnFlat btnFlatBlack btnFlatAsp" /></td>
                                <td>
                                    <asp:Button ID="btCancel" Text="${Button.Cancel}" runat="server" OnClientClick="Close(); return false;" CssClass="btnFlat btnFlatBlack btnFlatAsp" /></td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </div>

        <script language="javascript" type="text/javascript">

            function PageBase_Load() {

                // Cerrar el formulario si está pendiente de cerrar.
                CloseIfNeeded();

            }

            function PageBase_Unload() {
                var hdnSelectedValue = document.getElementById("hdnSelectedValue").value;
                var hdnSelectedName = document.getElementById("hdnSelectedName").value;
                if (hdnSelectedValue != "-1") {
                    try {
                        parent.ListChange(getUrlParam("IDField"), hdnSelectedName);
                    } catch (e) { }
                }
            }

            function CloseIfNeeded() {
                // Cerrar el formulario si está pendiente de cerrar.
                var _CanClose = $get('<%= Me.hdnCanClose.ClientID %>');
                if (_CanClose.value == '1') Close();
            }

            function Close() {
                try {
                    parent.HideExternalForm();
                } catch (e) { }
            }

            var $Progreso = $('<div class="ext-el-mask-msg x-mask-loading" id="ext-gen4" style="position:relative; margin-left:auto; margin-right:auto; top:-200px; width:50px;"><div>...</div></div>').hide();

            //###################################
            function FieldFindChanged(e) {
                tecla = (document.all) ? e.keyCode : e.which;
                if (tecla == 13) {
                    var ls = document.getElementById("FieldFindValue");
                    newItemInGrid(ls.value);

                    // Para cancelar la pulsación de tecla (así en la pantalla de calendario no sale el popup de pegado especial al apretar la tecla enter)
                    if (e.preventDefault) {
                        e.preventDefault();
                    }
                    else {
                        e.returnValue = false;
                    }
                }
            }

            //###################################
            function EnProgreso(Modo) {
                var divFilterList = document.getElementById("divFilterList");
                if (Modo == true)
                    $Progreso.appendTo(divFilterList).show();
                else
                    $Progreso.hide();
            }

            //###################################
            function newItemInGrid(KeyToFiter) {
                try {

                    EnProgreso(true);

                    var IDField = getUrlParam("IDField");

                    var path = '<%= Me.Page.ResolveURL("~/Base/WebUserControls/roFilterListValuessrv.aspx")%>' + "?action=newItemInGrid&IDField=" + IDField;
                if (KeyToFiter != null) {
                    path = path + "&KeyFilter=" + KeyToFiter;
                }
                AsyncCall("POST", path, "json", "arrGrid", "loadGrid(arrGrid);");

            }
            catch (e) {
                EnProgreso(false);
                alert(e);
                //showError("<%= Me.ClientID %>_newItemInGrid", e);
                }
            }

            //###################################
            function editGridFilterList(sIdRow) {
                try {
                    //obtener la linea seleccionada
                    var row = document.getElementById(sIdRow);
                    var IdReg = row.attributes["jsgridatt_id"].value;
                    var NameReg = row.attributes["jsgridatt_name"].value;
                    var hdnSelectedValue = document.getElementById("<%= hdnSelectedValue.ClientID %>");
                var hdnSelectedName = document.getElementById("<%= hdnSelectedName.ClientID %>");
                hdnSelectedValue.value = IdReg;
                hdnSelectedName.value = NameReg;

                jsGrid_rowClick(sIdRow, "tblGridFilterList");
            }
            catch (e) {
                hdnSelectedValue.value = "0";
                hdnSelectedName.value = "";
                alert(e);
                //showError("<%= Me.ClientID %>_createGrid", e);
                }
            }

            /* selecció Row (tr) */
            function jsGrid_rowClick(rowID, dTable) {
                try {
                    var tParent = document.getElementById(dTable);
                    var tCells = tParent.getElementsByTagName("td");
                    for (var i = 0; i < tCells.length; i++) {
                        removeCssClass(tCells[i], "gridRowOver");
                        removeCssClass(tCells[i], "gridRowSelected");
                    } //end for

                    var table = document.getElementById(rowID);
                    var cells = table.getElementsByTagName("td");
                    for (var i = 0; i < cells.length; i++) {
                        removeCssClass(cells[i], "gridRowOver");
                        addCssClass(cells[i], "gridRowSelected");
                    } //end for

                } catch (e) { showError("jsGrid_rowClick", e); }
            }

            //###################################
            function createGrid(arrGridParam) {
                try {

                    var headerGrid = [{ 'fieldname': 'id', 'description': '', 'size': '-1' },
                    { 'fieldname': 'name', 'description': '', 'size': '100%' }]

                    headerGrid[0].description = "Id";
                    headerGrid[1].description = "Valor" //document.getElementById('hdnStrName').value; //Nombre

                    var edtRow = false;
                    var delRow = false;

                    Grid = new jsGrid("grdLista", headerGrid, arrGridParam, edtRow, delRow, false, "FilterList");

                }
                catch (e) {
                    alert(e);
                //showError("<%= Me.ClientID %>_createGrid", e);
                }
            }

            //###################################
            function loadGrid(arrGridParam) {
                try {

                    EnProgreso(true);

                    if (arrGridParam == null) {
                        createGrid(null);
                    } else {
                        if (arrGridParam.length == 0) {
                            createGrid(null);
                        } else {
                            if (arrGridParam[0].error == "true") return;
                            createGrid(arrGridParam[0], null);
                        }
                    }
                }
                catch (e) {
                    alert(e);
                //showError("<%= Me.ClientID %>_loadGrid", e);
                }
                finally {
                    EnProgreso(false);
                }
            }

            //INSTRUCCION INICIAL
            window.onload = function () {
                loadGrid(null);
                newItemInGrid(null);
                resizeList();
            }

            window.onresize = function () {
                resizeList();
            }

            function resizeList() {
                var tr = document.getElementById('trList');
                var dv = document.getElementById('divFilterList');
                var gr = document.getElementById('grdLista');

                var oHeight = tr.clientHeight;
                dv.style.height = oHeight + 'px';
                gr.style.height = oHeight - 55 + 'px';
            }
            function getUrlParam(name) {
                if (name = (new RegExp('[?&]' + encodeURIComponent(name) + '=([^&]*)')).exec(location.search))
                    return decodeURIComponent(name[1]);
            }
        </script>
    </form>
</body>
</html>