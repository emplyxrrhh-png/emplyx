<%@ Control Language="VB" AutoEventWireup="false" Inherits="VTLive40.Base_WebUserControls_roTreeV3Prev" CodeBehind="roTreeV3Prev.ascx.vb" %>

<input type="hidden" id="hdnAfterSelectFilterFuncion" value="" runat="server" />

<div class="fondo_tree">
    <div class="fondo_tree_toolbar">
        <!-- Icones Filtrat -->
        <table width="100%" height="16px" border="0">
            <tr>
                <td width="16px" id="btn1Filter" runat="server"><a id="icoFilt1" href="javascript: void(0)" class="icoFilter1 icoPressed" title="Empleados con contrato en vigor" runat="server" onclick=""></a></td>
                <td width="16px" id="btn2Filter" runat="server"><a id="icoFilt2" href="javascript: void(0)" class="icoFilter2 icoPressed" title="Empleados en transito" runat="server" onclick=""></a></td>
                <td width="16px" id="btn3Filter" runat="server"><a id="icoFilt3" href="javascript: void(0)" class="icoFilter3 icoPressed" title="Empleados de baja" runat="server" onclick=""></a></td>
                <td width="16px" id="btn4Filter" runat="server"><a id="icoFilt4" href="javascript: void(0)" class="icoFilter4 icoPressed" title="Altas futuras" runat="server" onclick=""></a></td>
                <td width="16px" id="btn5Filter" runat="server"><a id="icoFilt5" href="javascript: void(0)" class="icoFilter5 icoPressed" title="Filtrar por los campos de la ficha de empleados" runat="server" onclick=""></a></td>
                <td>&nbsp;</td>
                <td align="right" width="32px">
                    <table border="0" cellpadding="0" cellspacing="0" style="width: 32px;">
                        <tr>
                            <td id="showEmployeeFilter" runat="server" style="width: 16px;">
                                <a id="icoFiltAdv" runat="server" href="javascript: void(0)" class="icoFilter icoClass icoPressed" title="Filtro avanzado" onclick=""></a>
                                <div id="divFiltreAvan_Float" style="margin-left: -10px; margin-top: -5px; position: relative; z-index: 9000; height: 200px; display: none;" runat="server"></div>
                            </td>
                            <td style="width: 16px;">
                                <a href="javascript: void(0)" class="icoRefresh icoClass" title="<%= Me.Language.Translate("Button.Refresh","roSelector") %>"
                                    onclick="eval('<%= Me.ClientID %>_roTrees.reLoadGroupsFromDB();');eval('<%= Me.ClientID %>_roTrees.LoadTreeViews(true, true, true);');"></a>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    </div>
    <div style="height: calc(100% - 26px)">
        <div id="divFiltreAvan" style="border: solid 1px silver; margin-left: 34px; margin-top: 2px; position: relative; z-index: 9000; height: 250px; width: 400px; background-color: White; display: none;" runat="server">
        </div>

        <table border="0" cellpadding="0" cellspacing="0" style="height: 100%; width: 100%">
            <tr>
                <td valign="top" style="width: 20px; padding-top: 15px;">
                    <!-- Tabs -->
                    <table border="0" cellpadding="0" cellspacing="0">
                        <tr>
                            <td style="padding-bottom: 1px;"><a href="javascript: void(0);" runat="server" id="tabTree01" class="tabSel1-active"></a></td>
                        </tr>
                        <tr>
                            <td style="padding-bottom: 1px;"><a href="javascript: void(0);" runat="server" id="tabTree02" class="tabSel2"></a></td>
                        </tr>
                        <tr>
                            <td style="padding-bottom: 1px;"><a href="javascript: void(0);" runat="server" id="tabTree03" class="tabSel3"></a></td>
                        </tr>
                    </table>
                </td>

                <!-- Arboles -->
                <td valign="top" style="padding-top: 1px; padding-right: 1px;" id="<%= Me.ClientID %>_td">

                    <!-- Arbre Normal -->
                    <div id="<%= Me.ClientID %>_tree-div" style="display: block; overflow: auto; width: calc(100% - 30px); height: 250px; text-align: left; border: solid 1px silver;"></div>

                    <!-- Arbre Agrupat -->
                    <div id="<%= Me.ClientID %>_tree-div-UF" style="display: block; width: 100%; height: 250px; text-align: left; border: solid 1px silver; display: none;">
                        <table style="height: 100%; width: 100%" cellpadding="0" cellspacing="0" border="0">
                            <tr>
                                <td valign="top" height="50px">
                                    <span style="display: block; width: 99%; padding-left: 8px; padding-top: 4px; padding-bottom: 2px; color: #2D4155;">
                                        <asp:Label ID="lblAgruparPor" runat="server" Text="Agrupar por:"></asp:Label>
                                    </span>
                                    <!-- Combo Agrupació -->
                                    <div style="width: 98%; padding: 2px; text-align: left;">
                                        <roWebControls:roComboBox runat="server" ID="cmbAgrupacio" Width="100%" Height="14px" AutoResizeChildsWidth="true" ParentHeight="14px"
                                            ParentWidth="100%" ChildsHeight="14px" ChildsWidth="100%" ItemsRunAtServer="false" ChildsVisible="10" />
                                    </div>
                                </td>
                            </tr>
                            <tr>
                                <td valign="top">
                                    <div id="<%= Me.ClientID %>_tree-div2" style="position: relative; overflow: auto; width: 100%; height: 100%; text-align: left; border-top: solid 1px #BFBFBF;"></div>
                                </td>
                            </tr>
                        </table>
                    </div>

                    <!-- Arbre 3 -->
                    <div id="<%= Me.ClientID %>_tree-div-FF" style="display: block; width: 100%; height: 250px; text-align: left; border: solid 1px silver; display: none;">
                        <table style="height: 100%; width: 100%" cellpadding="0" cellspacing="0" border="0">
                            <tr>
                                <td valign="top" height="26px">
                                    <span style="display: block; width: 99%; padding-left: 8px; padding-top: 4px; padding-bottom: 2px; color: #2D4155;">
                                        <asp:Label ID="lblFindText" runat="server" Text="Buscar por:"></asp:Label>
                                    </span>
                                    <!-- Combo Agrupació -->
                                    <div style="width: 98%; padding: 2px; text-align: left;">
                                        <roWebControls:roComboBox runat="server" ID="cmbFieldFind" Width="100%" Height="14px" AutoResizeChildsWidth="true" ParentHeight="14px"
                                            ParentWidth="100%" ChildsHeight="14px" ChildsWidth="100%" ItemsRunAtServer="false" ChildsVisible="10" />
                                    </div>
                                </td>
                            </tr>
                            <tr>
                                <td valign="top" height="20px">
                                    <div style="width: 100%; padding-left: 2px; padding-top: 7px; padding-bottom: 2px; text-align: left; margin-top: 5px;">
                                        <input type="text" id="<%= Me.ClientID %>_FieldFindValue" class="textClass" style="padding: 2px; width: 95%;" onkeydown="FieldFindChanged(event,'<%= Me.ClientID %>');" onblur="FieldFindBlur(event);" />
                                    </div>
                                </td>
                            </tr>
                            <tr>
                                <td valign="top" style="height: 100%; width: 100%">
                                    <div id="<%= Me.ClientID %>_tree-div3" style="overflow: auto; width: 100%; height: 100%; text-align: left; border-top: solid 1px #BFBFBF;"></div>
                                </td>
                            </tr>
                        </table>
                    </div>
                </td>
            </tr>
        </table>
    </div>
</div>

<script type="text/javascript" language="javascript">
    eval("var <%= Me.ClientID %>_roTrees;"); //Variable Global i dinamica de l'arbre
    eval("var <%= Me.ClientID %>_Path;"); //Path d'acces

    eval("<%= Me.ClientID %>_Path = '<%= Me.ResolvePath %>';");

    var createScript = "<%= Me.ClientID %>_roTrees = new roTrees('<%= Me.ClientID %>',";
    createScript += "true,false,<%= Me.OnlyGroups.ToString.ToLower %>,'<%= Me.Tree1Function %>','<%= Me.Tree1SelectorPage %>','<%= Me.Tree1ImagePath %>',false,'',";
    createScript += "true,false,<%= Me.OnlyGroups.ToString.ToLower %>,'<%= Me.Tree2Function %>','<%= Me.Tree2SelectorPage %>','<%= Me.Tree2ImagePath %>',false,'',";
    createScript += "true,false,<%= Me.OnlyGroups.ToString.ToLower %>,'<%= Me.Tree3Function %>','<%= Me.Tree3SelectorPage %>','<%= Me.Tree3ImagePath %>',";
    createScript += "'<%= Me.FeatureAlias %>','<%= Me.FeatureType %>',<%= Me.FirstClick.ToString.ToLower %>,'" + encodeURIComponent('<%= Me.FilterFixed %>') + "');";

    eval(createScript);

    let initTreeSelector_<%= Me.ClientID %> = function () {
        chTree(getActiveTreeType('<%= Me.ClientID %>'), '<%= Me.ClientID %>');
        //Redimensio dels arbres
        eval('<%= Me.ClientID %>_resizeTreesV3()');
    }

    if (window.addEventListener) {
        window.addEventListener('load', initTreeSelector_<%= Me.ClientID %>)
    } else {
        window.attachEvent('onload', initTreeSelector_<%= Me.ClientID %>)
    }
    

    window.onresize = function () {
        eval('<%= Me.ClientID %>_resizeTreesV3()');
    }

    function <%= Me.ClientID %>_resizeTreesV3() {

        var TD = document.getElementById('<%= Me.ClientID %>_td');

         var ar1 = document.getElementById('<%= Me.ClientID %>_tree-div');
         var ar2 = document.getElementById('<%= Me.ClientID %>_tree-div-UF');
         var tree2 = document.getElementById('<%= Me.ClientID %>_tree-div2');
         var ar3 = document.getElementById('<%= Me.ClientID %>_tree-div-FF');
         var tree3 = document.getElementById('<%= Me.ClientID %>_tree-div3');

        var newHeight = 0;
        var newWidth = 0;
        if (typeof (TD.clientHeight) != 'undefined' && TD.clientHeight > 0) {
            newHeight = TD.clientHeight;
            newWidth = TD.clientWidth;
        } else {
            newHeight = 270;
            newWidth = 170;
        }

        ar1.style.height = (newHeight - 4) + "px";
        ar1.style.width = (newWidth - 3) + "px";
        ar2.style.height = (newHeight - 4) + "px";
        ar2.style.width = (newWidth - 3) + "px";

        tree2.style.height = (newHeight - 55) + "px";
        tree2.style.width = (newWidth - 3) + "px";

        ar3.style.height = (newHeight - 4) + "px";
        ar3.style.width = (newWidth - 3) + "px";
        tree3.style.height = (newHeight - 73) + "px";
        tree3.style.width = (newWidth - 3) + "px";

    }
</script>

<script type="text/javascript" language="javascript">

    //Funcio Mostra / Amaga el Filtre avançat (flotant)
    function filterFloatVisibleV3Prev(objPrefix) {
        var objDiv = document.getElementById(objPrefix + '_divFiltreAvan_Float');
        if (objDiv.style.display == '') {
            objDiv.style.display = 'none';
            objDiv.style.position = 'relative';
        } else {
            objDiv.style.display = '';
            objDiv.style.position = 'absolute';
        }
    }

    //Funcio Mostra / Amaga el Filtre avançat (embed)
    function filterEmbeddedVisibleV3Prev(objPrefix, objPrefixTree) {
        var oWidth = '';
        var oHeight = '';
        var objDiv = document.getElementById(objPrefix + '_divFiltreAvan'); //Div de fora (no scroll)
        var objContainer = document.getElementById(objPrefix + '_dvContainer'); //Div amb scroll

        var treed1 = document.getElementById(objPrefix + '_' + objPrefixTree + '_tree-div');
        var treed2 = document.getElementById(objPrefix + '_' + objPrefixTree + '_tree-div-UF');
        var treed3 = document.getElementById(objPrefix + '_' + objPrefixTree + '_tree-div-FF');

        if (treed1 != null) { if (treed1.style.display != 'none') { oWidth = treed1.offsetWidth; oHeight = treed1.offsetHeight; } }
        if (treed2 != null) { if (treed2.style.display != 'none') { oWidth = treed2.offsetWidth; oHeight = treed2.offsetHeight; } }
        if (treed3 != null) { if (treed3.style.display != 'none') { oWidth = treed3.offsetWidth; oHeight = treed3.offsetHeight; } }

        if (objDiv.style.display == '') {
            objDiv.style.display = 'none';
            objDiv.style.position = 'relative';
        } else {
            objDiv.style.position = 'absolute';
            if (oWidth != '') { objDiv.style.width = oWidth + 'px'; }
            if (oHeight != '') { objDiv.style.height = oHeight + 'px'; }
            objDiv.style.display = '';
            if (objContainer != null) {
                objContainer.style.width = 390 + 'px';
                objContainer.style.height = 215 + 'px';
            }
        }
    }

    //Borra els camps del Filtre avançat
    function ClearUserFieldFilterV3Prev(objPrefix) {
        for (var n = 1; n < 6; n++) {
            var usrField = document.getElementById(objPrefix + '_UserFieldFilter' + n + '_ComboBoxLabel');
            var crtField = document.getElementById(objPrefix + '_CriteriaFilter' + n + '_ComboBoxLabel');
            var valField = document.getElementById(objPrefix + '_ValueFilter' + n);
            var optAnd = document.getElementById(objPrefix + '_OptionAND' + n);

            if (usrField != null) {
                roCB_setText('', objPrefix + '_UserFieldFilter' + n + '_ComboBoxLabel', '', '');
            }

            if (crtField != null) {
                roCB_setText('', objPrefix + '_CriteriaFilter' + n + '_ComboBoxLabel', '', '');
            }

            if (valField != null) { valField.value = ''; }
            if (optAnd != null) { optAnd.checked = true; }
        }
    }

    //Activa el filtre avançat
    async function SaveUserFieldFilterV3Prev(objPrefix, objPrefixTree, mJSFilterShow) {
        let strFilter = '';
        let strAux = '';
        let filterExists = false;

        //Recorrem els camps de consulta
        for (var n = 1; n < 6; n++) {
            let usrField = document.getElementById(objPrefix + '_UserFieldFilter' + n + '_ComboBoxLabel');
            let crtField = document.getElementById(objPrefix + '_CriteriaFilter' + n + '_ComboBoxLabel');
            let valField = document.getElementById(objPrefix + '_ValueFilter' + n);
            let optAnd = document.getElementById(objPrefix + '_OptionAND' + n);

            if (usrField != null) { //Si el camp de usuari es null, salta
                if (usrField.innerHTML != '') { //Si el camp de usuari esta en blanc, salta
                    if (crtField != null) {  //Si el camp de criteri es null, salta
                        if (crtField.innerHTML != '') { //Si el camp de criteri es blanc, salta
                            filterExists = true;
                            //Proteger valor (problemas al codificar/decodificar) le agregamos parentesis al tipo de campo xxx|y --> xxx|(y)
                            strAux = usrField.getAttribute("value").split("|")[0] + "|(" + usrField.getAttribute("value").split("|")[1] + ")"
                            strFilter += strAux + '~' + crtField.getAttribute("value");

                            if (valField == null) {
                                strFilter += '~' + '';
                            } else {
                                //Proteger valor (problemas al codificar/decodificar) le agregamos parentesis
                                strAux = '(' + valField.value + ')'
                                strFilter += '~' + strAux;
                            }
                            if (optAnd != null) {
                                if (optAnd.checked == true) {
                                    strFilter += '~AND';
                                } else {
                                    strFilter += '~OR';
                                }
                            }
                        }
                    }
                }
            }

            strFilter += String.fromCharCode(127);
        }

        if (!filterExists) strFilter = '';

        //Recupera el filtre actual
        //var arrFilters = getFilter(objPrefix + '_' + objPrefixTree);
        var arrFilters = await getFilter(objPrefix);

        //setFilter(arrFilters[0], strFilter, objPrefix + '_' + objPrefixTree);
        await setFilter(arrFilters[0], strFilter, objPrefix);

        var icoFilter = document.getElementById(objPrefix + '_icoFilt5');
        if (icoFilter.className.split(' ')[1] == 'icoUnPressed') {
            await UpdTreeFilter16(icoFilter, objPrefix);
        }
        else {
            //eval(objPrefix + '_' + objPrefixTree + '_roTrees.LoadTreeViews();');
            eval(objPrefix + '_roTrees.LoadTreeViews();');
        }

        icoFilter = document.getElementById(objPrefix + '_icoFiltAdv');
        if ((icoFilter.className.split(' ')[1] == 'icoUnPressed' && strFilter != '') ||
            (icoFilter.className.split(' ')[1] == 'icoPressed' && strFilter == '')) {
            await UpdTreeFilter16(icoFilter, objPrefix);
        }

        //Executa el tancament corresponent (segons tipus de finestra de filtrat)
        eval(mJSFilterShow);

        var hdnAfterSelectFilterFuncion = document.getElementById(objPrefix + '_' + "hdnAfterSelectFilterFuncion");
        if (hdnAfterSelectFilterFuncion.value != "") {
            try {
                eval(hdnAfterSelectFilterFuncion.value);
            } catch (e) { }
        }
    }
</script>