// "Clase" roTree (Arbre Independent)
// -------------------------------------------------------------------------------------------------
// TreeDiv     Div on es carregará l'arbre
// TreeType    (1-Arbre normal, 2-Arbre Agrupat per camp, 3-Arbre Filtrat per camp/valor
// OnlyGroups  Mostra sols els grups (true|false)
// functName   Nom de la funció a cridar (sense parentesis)

function roTree(oTreeConf) {
    this.objPrefix = oTreeConf.objPrefix;
    this.TreeDiv = oTreeConf.TreeDiv;
    this.TreeType = oTreeConf.TreeType;
    this.multiSel = oTreeConf.multiSel;
    this.OnlyGroups = oTreeConf.OnlyGroups;
    this.functName = oTreeConf.functName;
    this.SelectorPage = oTreeConf.SelectorPage;
    this.ImagePath = oTreeConf.ImagePath;
    this.enableDD = oTreeConf.enableDD;
    this.functDD = oTreeConf.functDD;
    this.FeatureAlias = oTreeConf.FeatureAlias;
    this.FeatureType = oTreeConf.FeatureType;
    this.ReloadGroups = oTreeConf.ReloadGroups;
    this.FilterFixed = oTreeConf.FilterFixed;
    this.onLoadCallback = oTreeConf.onLoadCallback;


    this.oTree = null; // Arbre dintre de la clase
    this.root = null;
    this.bolShowOnlyGroups = this.OnlyGroups; //Mostrar sols grups
    this.expandedON = true;
    this.LoaderUrl = null; //Url de Carrega

    this.rPath = eval(this.objPrefix + '_Path');
    this.strUrl = this.rPath + this.SelectorPage; //'EmployeeSelectorData.aspx';

    //Imatge transparent (necesari per ext)
    Ext.BLANK_IMAGE_URL = this.rPath + "../ext-3.4.0/resources/images/default/s.gif";

    this.oLoader = null; //Carregador


    let Filter = getFilter(this.objPrefix);
    let FieldFind = getFieldFind(this.objPrefix);
    let UserField = getUserField(this.objPrefix)

    let self = this;
    if (isPromise(Filter)) {
        Filter.then(resFilter => FieldFind.then(resFieldFind => UserField.then( resUserField => buildLoaderUrl(self,resFilter,resFieldFind, resUserField))));
    } else {
        buildLoaderUrl(self, Filter, FieldFind, UserField);
    }
    
}

function buildLoaderUrl(self, Filter, FieldFind, UserField) {
    //Parametre URL per retornar sols grups
    if (self.bolShowOnlyGroups) {
        self.LoaderUrl = self.strUrl + '?OnlyGroups=1';
    } else {
        self.LoaderUrl = self.strUrl + '?OnlyGroups=0';
    }
    //Path de les imatges
    self.LoaderUrl = self.LoaderUrl + '&ImagesPath=' + self.rPath + self.ImagePath; //'../images/EmployeeSelector';

    //Si es l'arbre per agrupació, pasa parametres de filtrat
    if (self.TreeType == '2') {
        self.LoaderUrl = self.LoaderUrl + '&UserField=Usr_' + encodeURIComponent(UserField);
    }

    //Filtrat normal
    self.LoaderUrl = self.LoaderUrl + '&Filters=' + Filter[0];
    self.LoaderUrl = self.LoaderUrl + '&FilterUserFields=' + encodeURIComponent(Filter[1]);

    //filtro fijo desde el constructor del control
    self.LoaderUrl = self.LoaderUrl + '&FilterFixed=' + self.FilterFixed;

    //Si es el ultim arbre, pasa parametres corresponents
    if (self.TreeType == '3') {
        self.LoaderUrl = self.LoaderUrl + '&action=FieldFindData';
        self.LoaderUrl = self.LoaderUrl + "&FieldFindColumn=" + FieldFind[0];
        self.LoaderUrl = self.LoaderUrl + "&FieldFindValue=" + encodeURIComponent(FieldFind[1]);
    }

    if (self.FeatureAlias != null) {
        self.LoaderUrl = self.LoaderUrl + '&FeatureAlias=' + self.FeatureAlias;
    }
    if (self.FeatureType != null) {
        self.LoaderUrl = self.LoaderUrl + '&FeatureType=' + self.FeatureType;
    }

    self._ReloadGroups = false;
    if (typeof (self.ReloadGroups) != "undefined" && self.ReloadGroups != null) {
        self._ReloadGroups = self.ReloadGroups;
    }
    self.LoaderUrl = self.LoaderUrl + '&ReloadGroups=' + self._ReloadGroups;

    self.LoaderUrl = self.LoaderUrl + '&MultiSelect=0';

    self.init(FieldFind);
}

roTree.prototype = {
    constructor: roTree,

    init: function (FieldFind) {
        let self = this;
        let Tree = Ext.tree;

        this.oLoader = new Tree.TreeLoader({ dataUrl: self.LoaderUrl, requestMethod: 'POST' });

        // Genera el "root node"
        this.root = new Tree.AsyncTreeNode({
            nodeType: 'async',
            text: 'rootnode',
            draggable: false,
            id: 'source'
        });

        this.oTree = new Tree.TreePanel({
            el: this.TreeDiv,
            useArrows: true,
            autoScroll: true,
            animate: true,
            enableDD: this.enableDD,
            floating: false,
            autoShow: true,
            containerScroll: true,
            // auto create TreeLoader
            loader: this.oLoader,
            rootVisible: false,
            rootUIProvider: Ext.tree.CheckboxNodeUI,
            root: this.root
        });

        if (this.enableDD) {
            if (this.functDD != '') {
                this.oTree.on('movenode', function (tree, node, oldParent, newParent, index) {
                    eval(self.functDD + '(tree, node, oldParent, newParent);');
                });
            }
        }

        this.oTree.on('click', async function (node, ev) {
            if (node != null) {
                try {
                    eval(self.functName + '(node);');
                } catch {
                    //Si no existe reimplemntada la función no pasa nada seguimos igual
                }
                self.DefaultTreeClick(node);
            }
        }, this.oTree);  //end onclick

        this.oTree.getSelectionModel().on('selectionchange', async function () {
            if (self.selNode != null) {
                // Guardar selección en las cookies
                let bolSaveSelected = true;

                if (self.selNode.id.indexOf('C') != -1) {
                    bolSaveSelected = false; //Si no es ni grup ni usuari... no fa res...
                }

                //Si es tenen de grabar les cookies i seleccionar
                if (bolSaveSelected) {
                    await setSelected(self.selNode.id, thselfis.TreeType, self.objPrefix);
                    await setSelectedPath(self.selNode.getPath(), self.TreeType, self.objPrefix);
                }
            }
        }, this.oTree.getSelectionModel()); //end selectionchange

        // collapse NODO DE l'arbre ------------------------------------------------------------------------
        this.oTree.on('collapsenode', function (nodeCollapsed) {
            this.expandedON = false;
        }, this.oTree); //end collapse --------------------------------------------------------------------------------

        //load: Carrega el Preloader
        this.oLoader.on('load', async function (itself, nodeLoaded, response) {
            let myMask = new Ext.LoadMask(self.TreeDiv, { msg: "&nbsp;" });
            myMask.hide();

            let treeObject = null;
            eval("treeObject = " + self.objPrefix + "_roTrees");
            treeObject.GetSelectedPath(self.TreeType, await getSelected(self.TreeType,self.objPrefix),true,self.objPrefix,false);



            // No hace falta posicionar manualmente, la función GetSelectedPath ya posiciona si se le pasa el último parámetro a true.
            let selPath = await getSelectedPath(self.TreeType, self.objPrefix);
            if (selPath != "" && selPath != null && selPath != '/source/') {
                if (self.expandedON && selPath.indexOf(nodeLoaded.id) != -1) {
                    self.oTree.expandPath(selPath, '', function (bSuccess, oLastNode) {
                        if (bSuccess) {
                            let pIsFirst = false;
                            eval("pIsFirst = " + self.objPrefix + "_roTrees.getIsFirst();");
                            if (pIsFirst) {
                                self.oTree.fireEvent('click', oLastNode);
                                eval(self.objPrefix + "_roTrees.setIsFirst(false);");
                            }
                        }
                    });
                }
            }
            else {
                self.SelectFirstNode(true, true);
            }
        }, this.oLoader); //end load del loader

        //beforeload (no fot res)
        this.oLoader.on('beforeload', function () { return true; }, this.oLoader);
        
        if (this.TreeType == '3' && FieldFind[1] == "") {
            // Pinta el Arbre -> Si es l´ultim arbre i no hi han parametres, no carrega l'arbre
        } else {
            this.oTree.render();
        }
    },

    DefaultTreeClick: async function (node) {
        let self = this;

        await setSelected(node.id, this.TreeType, this.objPrefix);
        await setSelectedPath(node.getPath(), this.TreeType, this.objPrefix);

        let selectedNodeId = await getSelected('1', this.objPrefix);

        switch (this.TreeType) {
            case '2':
                if (selectedNodeId != node.id) {
                    if (node.id.substr(0, 1) == 'B') { //Si el nodo es un empleado del arbol de la ficha
                        try {
                            setTimeout(function () { eval(self.objPrefix + "_roTrees.BuscaNodoFiltrado(node.id)"); }, 200);
                        }
                        catch { }
                    }
                }
                break;
            case '3':
                if (selectedNodeId != node.id) {
                    try {
                        eval(self.objPrefix + "_roTrees.BuscaNodoFiltrado(node.id)");
                    }
                    catch { }
                }
                break;
        }
    },
    SelectFirstNode: function (bolExpand, clickEvent) {
        let self = this;
        if (bolExpand) {
            if (this.root.childNodes.length > 0) {
                this.root.expand(false, false, function () {
                    self.root.firstChild.expand(false);
                    self.root.firstChild.select();
                    if (clickEvent) {
                        self.oTree.fireEvent('click', self.root.firstChild);
                    }
                });
            }
        }
    },

    SelectFirstNodeAndClick: function (bolExpand) {
        let self = this;
        if (bolExpand) {
            if (this.root.childNodes.length > 0) {
                this.root.expand(false, false, function () {
                    self.root.firstChild.expand(false);
                    self.root.firstChild.select();
                    self.oTree.fireEvent('click', self.root.firstChild);
                });
            }
        }
    },

    expandPath: function (NodePath, strP) {
        if (typeof this.oTree != 'undefined' && this.oTree != null) {
            this.oTree.expandPath(NodePath, '', function (bSuccess, oLastNode) {
                if (bSuccess)  oLastNode.select();
            });
        }
    },

    expandPathPPR: function (NodePath, callback) {
        if (typeof this.oTree != 'undefined' && this.oTree != null) {
            this.oTree.expandPath(NodePath, '', callback);
        }
    },

    expandPathAndClick: function (NodePath, strP) {
        let self = this;
        if (typeof this.oTree != 'undefined' && this.oTree != null) {
            this.oTree.expandPath(NodePath, '', function (bSuccess, oLastNode) {
                if (bSuccess) {
                    oLastNode.select();
                    self.oTree.fireEvent('click', oLastNode);
                }
            });
        }
    },

    click: function () {
        let self = this;
        let selPath = getSelectedPath(this.TreeType, this.objPrefix);

        if (isPromise(selPath)) {
            selPath.then(resSelPath => {
                if (typeof self.oTree != 'undefined' && self.oTree != null) {
                    self.oTree.expandPath(resSelPath, '', function (bSuccess, oLastNode) {
                        if (bSuccess) self.oTree.fireEvent('click', oLastNode);
                    });
                }
            });
        } else if (typeof self.oTree != 'undefined' && self.oTree != null) {
            self.oTree.expandPath(resSelPath, '', function (bSuccess, oLastNode) {
                if (bSuccess) self.oTree.fireEvent('click', oLastNode);
            });
        }

        
    },

    getShowOnlyGroups: function () {
        return this.bolShowOnlyGroups;
    },

    getstrSelected: function () {
        return this.strSelected;
    },

    getTreeType: function () {
        return this.TreeType;
    },

    getobjPrefix: function () {
        return this.objPrefix;
    },

    roTree: function () {
        return this.oTree;
    },

    getNodeById: function (idNode) {
        return this.oTree.getNodeById(idNode);
    },

    render: function () {
        this.oTree.render();
    }
};
