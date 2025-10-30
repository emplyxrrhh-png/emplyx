<%@ Control Language="VB" AutoEventWireup="false" Inherits="VTLive40.Base_WebUserControls_roTrees_roTreesSelector" CodeBehind="roTreesSelector.ascx.vb" %>

<script type="text/javascript">
    $(document).ready(function () {
        $("#caja").hover(
            function () {
                jQuery(".slide_likebox").stop(true, false).animate({ height: "106px" }, "medium");
            },
            function () {
                jQuery(".slide_likebox").stop(true, false).animate({ height: "0" }, "medium");
            }
        );

        $(".slide_likebox").hover(
            function () {
                jQuery(".slide_likebox").stop(true, false).animate({ height: "106px" }, "medium");
            },
            function () {
                jQuery(".slide_likebox").stop(true, false).animate({ height: "0" }, "medium");
            }
        );

        $(".slide_likebox").click(function () {
            jQuery(".slide_likebox").stop(true, false).animate({ height: "0" }, "fast");
        });

        $(".zass").hover(
            function () {
                jQuery("#<%= Me.ClientID %>_btnTreeDouble").stop(true, false).animate({ height: "40px" }, "medium");
            },
            function () {
                jQuery("#<%= Me.ClientID %>_btnTreeDouble").stop(true, false).animate({ height: "0" }, "fast");
            }
        );

        $("#<%= Me.ClientID %>_btnTreeDouble").hover(
            function () {
                jQuery("#<%= Me.ClientID %>_btnTreeDouble").stop(true, false).animate({ height: "40px" }, "medium");
            },
            function () {
                jQuery("#<%= Me.ClientID %>_btnTreeDouble").stop(true, false).animate({ height: "0" }, "fast");
            }
        );

    });

    function TreeSelectorKeyDown(event) {
        var keyCode = ('which' in event) ? event.which : event.keyCode;
        console.info("The Unicode key code is: " + keyCode);
        console.info(event);
        console.info(event.ctrlKey);
        if (event.ctrlKey && event.altKey) {
            if (keyCode == 49) {
                jQuery("#<%= Me.ClientID %>_tabTree01").click();
            }
            if (keyCode == 50) {
                jQuery("#<%= Me.ClientID %>_tabTree02").click();
            }
            if (keyCode == 51) {
                jQuery("#<%= Me.ClientID %>_tabTree03").click();
            }
        }
    }
</script>

<div onkeydown="TreeSelectorKeyDown(event)">
    <input type="hidden" id="hdnAfterSelectFilterFuncion" value="" runat="server" />
    <input type="hidden" id="hdnTreeDoubleClickFuncion" value="" runat="server" />

    <div style="margin-top: 3px;"></div>

    <div id="dvCaption" runat="server" class="RoundCorner_5_Sup mygradientleft treeCaption">
        <div id="dvTreeDoubleSelector" runat="server" style="float: left;">
            <a class="zass icoTreeDouble_24 treeIconDouble" href="javascript: void(0)"></a>
        </div>
        <div style="float: left;">
            <span id="spanCaption" runat="server" class="treeDescriptionText"></span>
        </div>
        <div id="dvRefreshSimple" runat="server" style="float: right; width: 24px; padding-right: 5px;">
            <a href="javascript: void(0)" class="icoRefresh_24 icoClass_24" title="<%= Me.Language.Translate("Button.Refresh","roSelector") %>" onclick="eval('<%= Me.ClientID %>_roTrees.reLoadGroupsFromDB();');eval('<%= Me.ClientID %>_roTrees.LoadTreeViews(true, true, true);');"></a>
        </div>
        <div style="clear: both"></div>
    </div>
    <div id="btnTreeDouble" runat="server" style="position: absolute; width: 135px; height: 0px; overflow: hidden; z-index: 9000; top: 125px; left: 12px;">
        <a id="spanCaptionDouble" runat="server" href="javascript: void(0)" class="RoundCorner spanCaptionDoubleClass"></a>
    </div>

    <div id="dvCab1" runat="server" style="padding: 2px; width: 280px;">
        <table border="0" width="100%">
            <tr>
                <td id="DesplegableCaja" runat="server" width="36px">
                    <div id="caja" class="tabSel1-active" style="height: 32px; width: 32px; background-color: Gray"></div>
                    <div class="slide_likebox">
                        <table border="0" cellpadding="0" cellspacing="1" style="">
                            <tr>
                                <td><a href="javascript: void(0);" runat="server" id="tabTree01" class="tabSel1-active"></a></td>
                            </tr>
                            <tr>
                                <td><a href="javascript: void(0);" runat="server" id="tabTree02" class="tabSel2"></a></td>
                            </tr>
                            <tr>
                                <td><a href="javascript: void(0);" runat="server" id="tabTree03" class="tabSel3"></a></td>
                            </tr>
                        </table>
                    </div>
                </td>
                <td width="24px"><a id="icoFilt1" href="javascript: void(0)" class="icoFilter1_24 icoPressed_24" title="Empleados con contrato en vigor" runat="server" onclick=""></a></td>
                <td width="24px"><a id="icoFilt2" href="javascript: void(0)" class="icoFilter2_24 icoPressed_24" title="Empleados en transito" runat="server" onclick=""></a></td>
                <td width="24px"><a id="icoFilt3" href="javascript: void(0)" class="icoFilter3_24 icoPressed_24" title="Empleados de baja" runat="server" onclick=""></a></td>
                <td width="24px"><a id="icoFilt4" href="javascript: void(0)" class="icoFilter4_24 icoPressed_24" title="Altas futuras" runat="server" onclick=""></a></td>
                <td width="24px"><a id="icoFilt5" href="javascript: void(0)" class="icoFilter5_24 icoPressed_24" title="Filtrar por los campos de la ficha de empleados" runat="server" onclick=""></a></td>
                <td width="24px">&nbsp;</td>
                <td align="right" width="32px">
                    <table border="0" cellpadding="0" cellspacing="0" style="width: 32px;">
                        <tr>
                            <td style="width: 24px;">
                                <a id="icoFiltAdv" runat="server" href="javascript: void(0)" class="icoFilter_24 icoPressed_24" title="Filtro avanzado" onclick=""></a>
                                <div id="divFiltreAvan_Float" style="margin-left: -10px; margin-top: -5px; position: relative; z-index: 9000; height: 200px; display: none;" runat="server"></div>
                            </td>
                            <td style="width: 24px;">
                                <a href="javascript: void(0)" class="icoRefresh_24 icoClass_24" title="<%= Me.Language.Translate("Button.Refresh","roSelector") %>" onclick="eval('<%= Me.ClientID %>_roTrees.reLoadGroupsFromDB();');eval('<%= Me.ClientID %>_roTrees.LoadTreeViews(true, true, true);');"></a>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    </div>
</div>

<div id="<%= Me.ClientID %>_dvCab2" class="RoundCorner_5_Inf treeBottomBox" style="border-radius: 5px !important">

    <!-- Filtre Avançat -->
    <div id="divFiltreAvan" runat="server" style="border: solid 1px silver; margin-left: 34px; margin-top: 2px; position: relative; z-index: 9000; height: auto; width: auto; background-color: White; display: none;"></div>
    <!-- Fin Filtre Avançat -->

    <div id="<%= Me.ClientID %>_dvContenedor" style="height: 100%;">

        <!-- Arbre #1 Normal -->
        <div id="<%= Me.ClientID %>_tree-div" class="treeBottomBoxTree" style="overflow: auto;"></div>

        <!-- Arbre #2 Agrupar Por -->
        <div id="<%= Me.ClientID %>_tree-div-UF" class="treeBottomBoxTree" style="display: none;">
            <div id="<%= Me.ClientID %>_tr_Ucombo" style="width: 100%; height: 50px;">
                <span style="display: block; padding-left: 10px; padding-top: 4px; padding-bottom: 2px; color: #2D4155;">
                    <asp:Label ID="lblAgruparPor" runat="server" Text="Agrupar por:"></asp:Label>
                </span>
                <div style="padding-left: 8px;">
                    <roWebControls:roComboBox runat="server" ID="cmbAgrupacio" Width="217px" AutoResizeChildsWidth="true" ParentWidth="217px" ChildsHeight="18px" ChildsWidth="217px" ItemsRunAtServer="false" ChildsVisible="10" />
                </div>
            </div>
            <div style="width: 100%; vertical-align: top">
                <div id="<%= Me.ClientID %>_tree-div2" style="overflow: auto; width: 100%; height: 100%; border-top: solid 1px silver;"></div>
            </div>
        </div>

        <!-- Arbre #3 Buscar Por -->
        <div id="<%= Me.ClientID %>_tree-div-FF" class="treeBottomBoxTree" style="display: none;">
            <div style="height: 40px; width: 100%" id="<%= Me.ClientID %>_tr1_Tree3">
                <span style="display: block; padding-left: 10px; padding-top: 4px; padding-bottom: 2px; color: #2D4155;">
                    <asp:Label ID="lblFindText" runat="server" Text="Buscar por:"></asp:Label>
                </span>
                <div style="padding-left: 8px;">
                    <roWebControls:roComboBox runat="server" ID="cmbFieldFind" Width="217px" AutoResizeChildsWidth="true" ParentWidth="217px" ChildsHeight="18px" ChildsWidth="217px" ItemsRunAtServer="false" ChildsVisible="10" />
                </div>
            </div>
            <div style="height: 36px; width: 100%" id="<%= Me.ClientID %>_tr2_Tree3">
                <div style="padding-left: 8px; padding-top: 2px; margin-top: 10px;">
                    <input type="text" id="<%= Me.ClientID %>_FieldFindValue" class="textClass" style="width: 255px; height: 20px; border-color: Silver" onkeypress="FieldFindChanged(event,'<%= Me.ClientID %>');" />
                </div>
            </div>
            <div style="width: 100%; vertical-align: top">
                <div id="<%= Me.ClientID %>_tree-div3" style="overflow: auto; width: 100%; height: 100%; border-top: solid 1px silver;"></div>
            </div>
        </div>
    </div>
</div>

<script type="text/javascript" language="javascript">

    eval("var <%= Me.ClientID %>_roTrees;"); //Variable Global i dinamica de l'arbre
    eval("var <%= Me.ClientID %>_Path;"); //Path d'acces
    eval("<%= Me.ClientID %>_Path = '<%= Me.ResolvePath %>';"); //declara la clase dels arbres dinamicament i globalment

    //Constructor de roTrees(Prefixe,Arbre1,MultiSeleccio1, Solsgrups1, funcio1, Arbre2,...)
    var createScript = "<%= Me.ClientID %>_roTrees = new roTrees('<%= Me.ClientID %>',";
    createScript += "<%= Me.Tree1Visible.ToString.ToLower  %>,<%= Me.Tree1MultiSel.ToString.ToLower  %>,<%= Me.Tree1ShowOnlyGroups.ToString.ToLower  %>,'<%= Me.Tree1Function %>','<%= Me.Tree1SelectorPage %>','<%= Me.Tree1ImagePath %>',<%= Me.Tree1EnableDD.ToString.ToLower  %>,'<%= Me.Tree1FunctDD %>',";
    createScript += "<%= Me.Tree2Visible.ToString.ToLower  %>,<%= Me.Tree2MultiSel.ToString.ToLower  %>,<%= Me.Tree2ShowOnlyGroups.ToString.ToLower  %>,'<%= Me.Tree2Function %>','<%= Me.Tree2SelectorPage %>','<%= Me.Tree2ImagePath %>',<%= Me.Tree2EnableDD.ToString.ToLower  %>,'<%= Me.Tree2FunctDD %>',";
    createScript += "<%= Me.Tree3Visible.ToString.ToLower  %>,<%= Me.Tree3MultiSel.ToString.ToLower  %>,<%= Me.Tree3ShowOnlyGroups.ToString.ToLower  %>,'<%= Me.Tree3Function %>','<%= Me.Tree3SelectorPage %>','<%= Me.Tree3ImagePath %>',";
    createScript += "'<%= Me.FeatureAlias %>','<%= Me.FeatureType %>',<%= Me.FirstClick.ToString.ToLower %>);";

    eval(createScript);

    let initTreeSelector_<%= Me.ClientID %> = function () {
        chTree(getActiveTreeType('<%= Me.ClientID %>'), '<%= Me.ClientID %>');
        //Redimensio dels arbres
        eval('<%= Me.ClientID %>_resizeTrees()');
    }


    if (window.addEventListener) {
        window.addEventListener('load', initTreeSelector_<%= Me.ClientID %>)
    } else {
        window.attachEvent('onload', initTreeSelector_<%= Me.ClientID %>)
    }


    //Posiciona al Tab de l'arbre corresponent via cookie
    

    window.onresize = function () {
        eval('<%= Me.ClientID %>_resizeTrees()');
    }

    function <%= Me.ClientID %>_resizeTrees() {

        var ctlTreeDiv = $("#ctlTreeDiv");
        var dvCaption = $("#" + '<%= Me.ClientID %>_dvCaption');
        var dvCab1 = $("#" + '<%= Me.ClientID %>_dvCab1');
        var dvCab2 = $("#" + '<%= Me.ClientID %>_dvCab2');

        var dvCaptionHeight = (dvCaption.length == 0) ? 0 : dvCaption.outerHeight(true);
        var dvCab1Height = (dvCab1.length == 0) ? 0 : dvCab1.outerHeight(true);

        dvCab2.height((ctlTreeDiv.height() - dvCab1Height - dvCaptionHeight - 10 - 1));

        var dvContenedor = $("#" + '<%= Me.ClientID %>_dvContenedor');

        dvContenedor.height(dvCab2.height() - 4);

        //Arbol 1
        var divTree1 = $("#" + '<%= Me.ClientID %>_tree-div');
        divTree1.height(dvContenedor.height() - 0);
        //console.log("dvContenedor: " + dvContenedor.height() + "| " + "divTree1: " + divTree1.height());
        //==========================================================================

        //Arbol 2
        var divTree2 = $("#" + '<%= Me.ClientID %>_tree-div-UF');
        divTree2.height(dvContenedor.height() - 0);

        var tr_Ucombo = $("#" + '<%= Me.ClientID %>_tr_Ucombo');

        var tdDivTree2 = $("#" + '<%= Me.ClientID %>_tree-div2');
        tdDivTree2.height((divTree2.height() - tr_Ucombo.outerHeight(true) - 4));
        //console.log("dvContenedor: " + dvContenedor.height() + "| " + "divTree2: " + divTree2.height() + "| " + "tr_Ucombo: " + tr_Ucombo.outerHeight() + "| " + "tdDivTree2: " + tdDivTree2.height());
        //==========================================================================

        //Arbol 3 BuscarPor
        var divTree3 = $("#" + '<%= Me.ClientID %>_tree-div-FF');
        divTree3.height(dvContenedor.height() - 0);

        var tr1_Tree3 = $("#" + '<%= Me.ClientID %>_tr1_Tree3');
        var tr2_Tree3 = $("#" + '<%= Me.ClientID %>_tr2_Tree3');

        var tdDivTree3 = $("#" + '<%= Me.ClientID %>_tree-div3');
        tdDivTree3.height((divTree3.height() - tr1_Tree3.outerHeight(true) - tr2_Tree3.outerHeight(true) - 4));
        //console.log("dvContenedor: " + dvContenedor.height() + "| " + "divTree3: " + divTree3.height() + "| " + "tr1_Tree3: " + tr1_Tree3.outerHeight() + "| " + "tr2_Tree3: " + tr2_Tree3.outerHeight() + "| " + "tdDivTree3: " + tdDivTree3.height());
        //==========================================================================
    }
</script>

<script type="text/javascript" language="javascript">

    //==========================================================
    //Funcio Mostra / Amaga el Filtre avançat (flotant)
    function filterFloatVisible(objPrefix) {
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
    function filterEmbeddedVisible(objPrefix, objPrefixTree) {
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
                objContainer.style.width = (oWidth - 8) + 'px';
                objContainer.style.height = (oHeight - 35) + 'px';
            }
        }
    }

    //Borra els camps del Filtre avançat
    function ClearUserFieldFilter(objPrefix) {
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
    async function SaveUserFieldFilter(objPrefix, objPrefixTree, mJSFilterShow) {
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
        if (icoFilter.className.split(' ')[1] == 'icoUnPressed_24') {
            await UpdTreeFilter24(icoFilter, objPrefix);
        }
        else {
            //eval(objPrefix + '_' + objPrefixTree + '_roTrees.LoadTreeViews();');
            eval(objPrefix + '_roTrees.LoadTreeViews();');
        }

        icoFilter = document.getElementById(objPrefix + '_icoFiltAdv');
        if ((icoFilter.className.split(' ')[1] == 'icoUnPressed_24' && strFilter != '') ||
            (icoFilter.className.split(' ')[1] == 'icoPressed_24' && strFilter == '')) {
            await UpdTreeFilter24(icoFilter, objPrefix);
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