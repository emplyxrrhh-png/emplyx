// "Clase" roTree (Arbre Independent)
// -------------------------------------------------------------------------------------------------
// TreeDiv     Div on es carregará l'arbre
// TreeType    (1-Arbre normal, 2-Arbre Agrupat per camp, 3-Arbre Filtrat per camp/valor
// multiSel    Arbre amb checkboxes (true|false)
// OnlyGroups  Mostra sols els grups (true|false)
// functName   Nom de la funció a cridar (sense parentesis)
function roTree(objPrefix, TreeDiv, TreeType, multiSel, OnlyGroups, functName, SelectorPage, ImagePath, enableDD, functDD, FeatureAlias, FeatureType, ReloadGroups, FilterFixed) {


    var expandedON = true;
    var oTree;                              // Arbre dintre de la clase
    var root;
    var oTreeExpandedNodes = null;          // Nodes expandits
    var strExpandedNodes;


    var bolMultiSelect = multiSel;               //Multi-seleccio
    var bolShowOnlyGroups = OnlyGroups;          //Mostrar sols grups

    var InitExpanding = false;                //Indica que està actualizando la expansión de los nodos del árbol
    var bolTimeout = false;

    //Url de Carrega
    var LoaderUrl;

    var rPath = eval(objPrefix + '_Path');
    var strUrl = rPath + SelectorPage;   //'EmployeeSelectorData.aspx';

    //Imatge transparent (necesari per ext)
    Ext.BLANK_IMAGE_URL = rPath + "../ext-3.4.0/resources/images/default/s.gif";

    var oLoader;                            //Carregador
    var oSelModel;                          //Objecte (checkboxnode) multi-seleccio



    //Parametre URL per retornar sols grups
    if (bolShowOnlyGroups) { LoaderUrl = strUrl + '?OnlyGroups=1'; } else { LoaderUrl = strUrl + '?OnlyGroups=0'; }
    //Path de les imatges
    LoaderUrl = LoaderUrl + '&ImagesPath=' + rPath + ImagePath;  //'../images/EmployeeSelector';

    //Si es l'arbre per agrupació, pasa parametres de filtrat
    if (TreeType == '2') { LoaderUrl = LoaderUrl + '&UserField=Usr_' + encodeURIComponent(getUserField(objPrefix)); }

    //Filtrat normal
    var Filter = getFilter(objPrefix);
    LoaderUrl = LoaderUrl + '&Filters=' + Filter[0];
    LoaderUrl = LoaderUrl + '&FilterUserFields=' + encodeURIComponent(Filter[1]);

    //filtro fijo desde el constructor del control
    LoaderUrl = LoaderUrl + '&FilterFixed=' + FilterFixed;

    //Si es el ultim arbre, pasa parametres corresponents
    if (TreeType == '3') {
        LoaderUrl = LoaderUrl + '&action=FieldFindData';
        var FieldFind = getFieldFind(objPrefix);
        LoaderUrl = LoaderUrl + "&FieldFindColumn=" + FieldFind[0];
        LoaderUrl = LoaderUrl + "&FieldFindValue=" + encodeURIComponent(FieldFind[1]);
    }

    if (FeatureAlias != null) {
        LoaderUrl = LoaderUrl + '&FeatureAlias=' + FeatureAlias;
    }
    if (FeatureType != null) {
        LoaderUrl = LoaderUrl + '&FeatureType=' + FeatureType;
    }

    var _ReloadGroups = false;
    if (typeof (ReloadGroups) != "undefined" && ReloadGroups != null) { _ReloadGroups = (ReloadGroups == true ? true : false); }
    LoaderUrl = LoaderUrl + '&ReloadGroups=' + _ReloadGroups;

    if (bolMultiSelect) {                   //Multi-Selecció activat
        LoaderUrl = LoaderUrl + '&MultiSelect=1';
        oLoader = new Ext.tree.CustomUITreeLoader({ dataUrl: LoaderUrl, baseAttr: { uiProvider: Ext.tree.CheckboxNodeUI} });
        oSelModel = new Ext.tree.CheckNodeMultiSelectionModel();
    } else {                                //Multi-Seleccio desactivat
        LoaderUrl = LoaderUrl + '&MultiSelect=0';
        //oLoader = new Ext.tree.CustomUITreeLoader({ dataUrl: LoaderUrl });
        oLoader = new Ext.tree.TreeLoader({ dataUrl: LoaderUrl, requestMethod: 'POST' })
        oSelModel = new Ext.tree.DefaultSelectionModel();
    } //endif bolMultiSelect


    Ext.onReady(function() {
        // shorthand
        var Tree = Ext.tree;
        // Genera el "root node"
        root = new Tree.AsyncTreeNode({
            nodeType: 'async',
            text: 'rootnode',
            draggable: false,
            id: 'source'
        });


        oTree = new Tree.TreePanel({
            el: TreeDiv,
            useArrows: true,
            autoScroll: true,
            animate: true,
            enableDD: enableDD,
            floating: false,
            autoShow: true,
            containerScroll: true,
            // auto create TreeLoader
            loader: oLoader,
            rootVisible: false,
            rootUIProvider: Ext.tree.CheckboxNodeUI,
            root: root
        });

        if (enableDD == true) {
            if (functDD != '') {
                oTree.on('movenode', function(tree, node, oldParent, newParent, index) {
                    eval(functDD + '(tree, node, oldParent, newParent);');
                });
            }
        }

        if (bolMultiSelect) {
            /* Check (Marca el Checkbox) *****************************/
            oTree.on('check', function() {

                var strSelectedAll = this.getChecked().join(',');
                var strSelected;

                // Gaurdamos la selección actual en la cookie
                setSelected(strSelectedAll, TreeType, objPrefix);

                // Recorremos selección para eliminar nodos finales no necesarios.
                var NodesList = this.getChecked();
                var NodesList2 = this.getCheckedNodes();

                strSelected = strSelectedAll;

                try {
                    eval(functName + '(NodesList,strSelected,strSelectedAll,NodesList2);');
                } catch (e) { } //alert('error functName:' + e); }

            }, oTree);
            /* *******************************************************/

        }
        else {
            oTree.on('click', function(node, ev) {

                if (node != null) {
                    var Selection = node.id;
                    //var NamesSelection = node.id + '#' + node.text;

                    // Guardar selección en las cookies
                    var bolSaveSelected = true;

                    //Carga el Nodo (cargaNodo se debe sobreescribir en otros scripts)            

                    //cargaNodo;
                    try {
                        eval(functName + '(node);');
                    } catch (e) { } //alert('error functName:' + e); }

                    if (node.id.indexOf('C') != -1) {
                        bolSaveSelected = false; //Si no es ni grup ni usuari... no fa res...
                    }

                    //Si es tenen de grabar les cookies i seleccionar
                    if (bolSaveSelected) {

                        setSelected(node.id, TreeType, objPrefix);
                        setSelectedPath(node.getPath(), TreeType, objPrefix);

                        switch (TreeType) {
                            case '1':
                                {  // Árbol grupos
                                    if (getSelected('2', objPrefix) != node.id) {

                                        if (node.id.substr(0, 1) == 'B') {
                                            setSelected(node.id, '2', objPrefix);
                                            // Obtener la ruta del empleado seleccionado por el árbol 2
                                            eval(objPrefix + "_roTrees.GetSelectedPath('2', getSelected(TreeType,objPrefix),true,objPrefix);");
                                        }
                                        else {
                                            setSelectedPath(null, '2', objPrefix);
                                        }
                                    }
                                    break;
                                }

                            case '2':
                                {   // Árbol campos ficha empleado
                                    if (getSelected('1') != node.id) {

                                        if (node.id.substr(0, 1) == 'B') {
                                            setSelected(node.id, '1', objPrefix);
                                            // Obtener la ruta del empleado seleccionado por el árbol 1
                                            eval(objPrefix + "_roTrees.GetSelectedPath('1', getSelected(TreeType,objPrefix),true,objPrefix);");
                                        }
                                        else {
                                            setSelectedPath(null, '1', objPrefix);
                                        }
                                    }
                                    break;
                                }
                        }

                        //PPR  seleccionar nodo en arbol principal si se selecciona en el de "Buscar Por"
                        if (TreeType == "3") {
                            try {
                                eval(objPrefix + "_roTrees.BuscaNodoFiltrado(node.id)");
                            }
                            catch (e) { }
                        }

                    }
                }
            }, oTree);  //end onclick

            oTree.getSelectionModel().on('selectionchange', function() {
                if (this.selNode != null) {
                    var Selection = this.selNode.id;

                    // Guardar selección en las cookies
                    var bolSaveSelected = true;

                    if (this.selNode.id.indexOf('C') != -1) {
                        bolSaveSelected = false; //Si no es ni grup ni usuari... no fa res...
                    }

                    //Si es tenen de grabar les cookies i seleccionar
                    if (bolSaveSelected) {
                        setSelected(this.selNode.id, TreeType, objPrefix);
                        setSelectedPath(this.selNode.getPath(), TreeType, objPrefix);
                    }
                }
            }, oTree.getSelectionModel()); //end selectionchange

        } //endif


        // collapse NODO DE l'arbre ------------------------------------------------------------------------
        oTree.on('collapsenode', function(nodeExpandPPR) {
            expandedON = false;
        }, oTree); //end collapse --------------------------------------------------------------------------------


        // Expandeix l'arbre ------------------------------------------------------------------------
        oTree.on('expand', function() {
            if (InitExpanding == true) {
                var NodesList;
                var oNode;
                var i;

                // Expandir nodos hijos
                var Expanded = strExpandedNodes;
                if (Expanded != null) {
                    NodesList = Expanded.split(",");
                    Expanded = '';
                    for (i = 0; i < NodesList.length; i++) {
                        oNode = this.getNodeById(NodesList[i]);
                        if (typeof oNode != 'undefined') {
                            //if (i == (NodesList.length-1)) InitExpanding = false;
                            if (oNode.isExpanded() == false)
                                oNode.expand(false);
                        }
                        else {
                            Expanded = Expanded + "," + NodesList[i];
                        }
                    }
                    if (Expanded != '') Expanded = Expanded.substring(1);
                    strExpandedNodes = Expanded;
                }

                if (strExpandedNodes == '') InitExpanding = false;

            } else {
                // Actualizar nodos expandidos
                var strExpanded = ExpandedNodes(this.getRootNode());
                switch (TreeType) {
                    case '1': treeExpandedNodes = strExpanded; break;
                    case '2': treeUserFieldExpandedNodes = strExpanded; break;
                }

                if (bolMultiSelect) {
                    setExpanded(strExpanded, TreeType, objPrefix);
                }

            } //endif

            function ExpandedNodes(node) {
                var strRet = '';
                var n = 0;
                if (node.isExpanded() == true) {
                    strRet = node.id + ',';
                    var strChilds;
                    for (n = 0; n < node.childNodes.length; n++) {
                        strChilds = ExpandedNodes(node.childNodes[n]);
                        if (strChilds != '')
                            strRet = strRet + strChilds + ',';
                    }
                }
                if (strRet.length > 1) {
                    if (strRet.substr(strRet.length - 1, strRet.length - 1) == ',')
                        strRet = strRet.substr(0, strRet.length - 1)
                }
                return strRet;
            }

        }, oTree); //end expand --------------------------------------------------------------------------------

        oTree.on('collapse', function() {
            // Actualizar nodos expandidos
            var strExpanded = ExpandedNodes(this.getRootNode());
            switch (TreeType) {
                case '1': tree1ExpandedNodes = strExpanded; break;
                case '2': tree2ExpandedNodes = strExpanded; break;
            }

            function ExpandedNodes(node) {
                var strRet = '';
                var n = 0;
                if (node.isExpanded() == true) {
                    strRet = node.id + ',';
                    var strChilds;
                    for (n = 0; n < node.childNodes.length; n++) {
                        strChilds = ExpandedNodes(node.childNodes[n]);
                        if (strChilds != '')
                            strRet = strRet + strChilds + ',';
                    }
                }
                if (strRet.length > 1) {
                    if (strRet.substr(strRet.length - 1, strRet.length - 1) == ',')
                        strRet = strRet.substr(0, strRet.length - 1)
                }
                return strRet;
            }

        }, oTree); //end collapse

        //load: Carrega el Preloader
        oLoader.on('load', async function(itself, nodeLoaded, response) {

            var myMask = new Ext.LoadMask(TreeDiv, { msg: "&nbsp;" });
            myMask.hide();

            // Marcar nodos selección temporal
            if (bolMultiSelect) {
                var strChecked = await getSelected(TreeType, objPrefix);
                if (strChecked != null) {

                    Nodes = strChecked.split(",");
                    for (i = 0; i < Nodes.length; i++) {
                        oNode = oTree.getNodeById(Nodes[i]);
                        if (typeof oNode != 'undefined') {
                            oNode.getUI().check(true, false, true);
                        } //endif
                    } //end for

                    var strSelectedAll = oTree.getChecked().join(',');
                    var strSelected;

                    // Recorremos selección para eliminar nodos finales no necesarios.
                    var NodesList = oTree.getChecked();
                    var SelectedNodes = [];

                    var oNode, oChildNode;
                    var bolAddNode;
                    var i, j, childsChecked;
                    for (i = 0; i < NodesList.length; i++) {
                        bolAddNode = true;
                        oNode = oTree.getNodeById(NodesList[i]);
                        //if (oNode.isLeaf()) {
                        if (oNode.parentNode != null) {
                            if (oNode.parentNode.attributes.checked) {
                                childsChecked = 0;
                                for (j = 0; j < oNode.parentNode.childNodes.length; j++) {
                                    oChildNode = oNode.parentNode.childNodes[j];
                                    if (oChildNode.attributes.checked) {
                                        childsChecked = childsChecked + 1;
                                    } //endif
                                } //end for
                                if (childsChecked == oNode.parentNode.childNodes.length) {
                                    bolAddNode = false;
                                } //endif
                            } //end if
                        } //endif 
                        //}
                        if (bolAddNode) {
                            SelectedNodes.push(oNode.id);
                        } //endif
                    } //end for
                    strSelected = SelectedNodes.join(',');

                    try {
                        eval(functName + '(NodesList,strSelected,strSelectedAll);');
                    } catch (e) { }

                }
            }
            else {
                eval(objPrefix + "_roTrees.GetSelectedPath('" + TreeType + "', getSelected(TreeType,objPrefix),true,objPrefix,false);");
                // No hace falta posicionar manualmente, la función GetSelectedPath ya posiciona si se le pasa el último parámetro a true.
                var selPath = await getSelectedPath(TreeType, objPrefix);
                if (selPath != "" && selPath != null && selPath != '/source/') {
                    //alert('fire');
                    if (expandedON == true) {
                        oTree.expandPath(selPath, '', function(bSuccess, oLastNode) {
                            var NodeSelected = false;
                            if (bSuccess) {
                                var pIsFirst = false;
                                eval("var pIsFirst = " + objPrefix + "_roTrees.getIsFirst();");
                                if (pIsFirst) {
                                    oTree.fireEvent('click', oLastNode);
                                    eval(objPrefix + "_roTrees.setIsFirst(false);");
                                }
                            }
                        });
                    }
                }
                else {
                    SelectFirstNode(true, true);
                }
            }
        }, oLoader); //end load

        //beforeload (no fot res)
        oLoader.on('beforeload', function() { return true; }, oLoader);

        // Pinta el Arbre
        //Si es el ultim arbre i no hi han parametres, no carrega l'arbre
        if (TreeType == '3' && FieldFind[1] == "") {
        } else {
            oTree.render();
        }

    });      //end Ext.Ready

    strExpandedNodes = getExpanded(TreeType, objPrefix); //Recupera si hi han nodes expandits       
    var Expanded = strExpandedNodes;
    if (Expanded != null && oTree != null && oTree != 'undefined') {
        InitExpanding = true;
        var NodesList = Expanded.split(",");
        Expanded = '';
        var i = 0;
        var oNode;
        for (i = 0; i < NodesList.length; i++) {
            oNode = oTree.getNodeById(NodesList[i]);
            if (typeof oNode != 'undefined') {
                if (oNode.isExpanded() == false)
                    oNode.expand(false);
            }
            else {
                Expanded = Expanded + "," + NodesList[i];
            }
        }
        if (Expanded != '') Expanded = Expanded.substring(1);
        strExpandedNodes = Expanded;
    }

    function SelectFirstNode(bolExpandFirstNode, clickEvent) {
        if (bolExpandFirstNode == true) {
            root.expand(false, false, function() {
                root.firstChild.expand(false);
                root.firstChild.select();
                if (clickEvent == true) {
                    oTree.fireEvent('click', root.firstChild);
                }
            });
        }
    }


    /* Propietats */
    this.getMultiSelect = function() { return bolMultiSelect; }
    this.getShowOnlyGroups = function() { return bolShowOnlyGroups; }
    this.getoTreeExpandedNodes = function() { return oTreeExpandedNodes; }
    this.getstrExpandedNodes = function() { return strExpandedNodes; }
    this.getstrSelected = function() { return strSelected; }
    this.getTreeType = function() { return TreeType; }
    this.getobjPrefix = function() { return objPrefix; }

    this.roTree = function() {
        return oTree;
    }

    this.getNodeById = function(idNode) { return oTree.getNodeById(idNode); }


    /* Funcions publiques */
    this.render = function() { oTree.render(); }
    this.SelectFirstNode = function(bolExpand) { SelectFirstNode(bolExpand); }
    this.SelectFirstNodeAndClick = function(bolExpand) { SelectFirstNode(bolExpand, true); }

    this.expandPath = function(NodePath, strP) {
        oTree.expandPath(NodePath, '', function(bSuccess, oLastNode) {
            var NodeSelected = false;
            if (bSuccess) {
                oLastNode.select();
            }
        });
    }

    this.expandPathPPR = function(NodePath, callback) {
        oTree.expandPath(NodePath, '', callback);
    }


    this.expandPathAndClick = function(NodePath, strP) {
        oTree.expandPath(NodePath, '', function(bSuccess, oLastNode) {
            var NodeSelected = false;
            if (bSuccess) {
                oLastNode.select();
                oTree.fireEvent('click', oLastNode);
            }
        });
    }

    this.click = function() {
        var selPath = getSelectedPath(TreeType, objPrefix);
        oTree.expandPath(selPath, '', function(bSuccess, oLastNode) {
            var NodeSelected = false;
            if (bSuccess) {
                oTree.fireEvent('click', oLastNode);
            }
        });
    }


} // End Class roTree

