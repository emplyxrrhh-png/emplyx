/* -----------------------------------------------------------------------*/
/* roTrees Class (Classe que manté els 3 arbres) -------------------------*/
/* -----------------------------------------------------------------------*/
/* objPrefix    : Tipus de arbre (1|2|3)
/* div'x'Load   : Carrega dels arbres (true|false) 
/* multiSel'x'  : Arbre 'x' amb multiseleccio (true|false)
/* onlyGroups'x': Arbre 'x' sols mostra grups (true|false)
/* -----------------------------------------------------------------------------------*/
function roTrees(objPrefix, div1Load, multiSel1, onlyGroups1, funct1, selPage1, ImgPath1, enableDD1, functDD1,
    div2Load, multiSel2, onlyGroups2, funct2, selPage2, ImgPath2, enableDD2, functDD2,
    div3Load, multiSel3, onlyGroups3, funct3, selPage3, ImgPath3, FeatureAlias, FeatureType, FirstClick, FilterFixed) {

    let roTree1, roTree2, roTree3;
    let oDiv1, oDiv2, oDiv3;
    let LoadTree1 = false;
    let LoadTree2 = false;
    let LoadTree3 = false;
    let disableReposition1 = false;
    let disableReposition2 = false;
    let reLoadGroups = false;
    let onLoadCallback = null;

    let isFirst = false;
    let strFilterFixed = "";

    if (FirstClick == null) {
        isFirst = false;
    }
    else {
        isFirst = FirstClick;
    }

    if (FilterFixed == null) {
        strFilterFixed = "";
    }
    else {
        strFilterFixed = FilterFixed;
    }

    this.getdisableReposition1 = function () { return disableReposition1; }
    this.setdisableReposition1 = function (val) { disableReposition1 = val; }

    this.getdisableReposition2 = function () { return disableReposition2; }
    this.setdisableReposition2 = function (val) { disableReposition2 = val; }

    this.getIsFirst = function () { return isFirst; }
    this.setIsFirst = function (val) { isFirst = val; }

    if (div1Load) { //Primer arbre
        oDiv1 = document.getElementById(objPrefix + '_tree-div');
    }
    if (div2Load) { //Segon arbre
        oDiv2 = document.getElementById(objPrefix + '_tree-div2');
    }
    if (div3Load) { //Tercer arbre
        oDiv3 = document.getElementById(objPrefix + '_tree-div3');
    }

    /* reloadTree (torna a crear el objecte roTree  -------------------------*/
    /* TreeType : Arbre a marcar el node (1,2,3)
    /* 
    /* -----------------------------------------------------------------------------------*/
    this.reloadTree = function (TreeType) {
        if (TreeType == '1') {
            roTree1 = new roTree({
                objPrefix: objPrefix,
                TreeDiv: oDiv1,
                TreeType: '1',
                multiSel: multiSel1,
                OnlyGroups: onlyGroups1,
                functName: funct1,
                SelectorPage: selPage1,
                ImagePath: ImgPath1,
                enableDD: enableDD1,
                functDD: functDD1,
                FeatureAlias: FeatureAlias,
                FeatureType: FeatureType,
                ReloadGroups: reLoadGroups,
                FilterFixed: strFilterFixed,
                onLoadCallback: this.onLoadCallback
            });
        }
        if (TreeType == '2') {
            roTree2 = new roTree({
                objPrefix: objPrefix,
                TreeDiv: oDiv2,
                TreeType: '2',
                multiSel: multiSel2,
                OnlyGroups: onlyGroups2,
                functName: funct2,
                SelectorPage: selPage2,
                ImagePath: ImgPath2,
                enableDD: enableDD2,
                functDD: functDD2,
                FeatureAlias: FeatureAlias,
                FeatureType: FeatureType,
                ReloadGroups: reLoadGroups,
                FilterFixed: strFilterFixed,
                onLoadCallback: this.onLoadCallback
            });
        }
        if (TreeType == '3') {
            roTree3 = new roTree({
                objPrefix: objPrefix,
                TreeDiv: oDiv3,
                TreeType: '3',
                multiSel: multiSel3,
                OnlyGroups: onlyGroups3,
                functName: funct3,
                SelectorPage: selPage3,
                ImagePath: ImgPath3,
                enableDD: null,
                functDD: null,
                FeatureAlias: FeatureAlias,
                FeatureType: FeatureType,
                ReloadGroups: reLoadGroups,
                FilterFixed: strFilterFixed,
                onLoadCallback: this.onLoadCallback
            });
        }
        reLoadGroups = false;
    }

    /* TreeSelectNode  (Selecció del Node corresponent al arbre) -------------------------*/
    /* TreeType : Arbre a marcar el node (1,2,3)
    /* NodeId   : Id. del Node
    /* NodePath : Path del Node
    /* -----------------------------------------------------------------------------------*/
    this.TreeSelectNode = function (TreeType, NodeId, NodePath, fireclick) {
        let self = this;
        if (isPromise(NodeId)) {
            NodeId.then(resNode => NodePath.then(resPath => self.endTreeSelectNode(TreeType, resNode, resPath, fireclick)));
        } else {
            self.endTreeSelectNode(TreeType, NodeId, NodePath, fireclick);
        }
    }
    this.endTreeSelectNode = function (TreeType, NodeId, NodePath, fireclick) {

        let bolExpandFirstNode = false;

        let oTree = null;
        switch (TreeType) {
            case '1': { oTree = roTree1; bolExpandFirstNode = true; break; }
            case '2': { oTree = roTree2; break; }
            case '3': { oTree = roTree3; break; }
        }

        if (oTree != null) {
            // Seleccionar nodo activo.		
            if (NodePath != null) {
                if (fireclick) {
                    oTree.expandPathAndClick(NodePath, '');
                } else {
                    oTree.expandPath(NodePath, '');
                }
            } else {
                oTree.SelectFirstNodeAndClick(bolExpandFirstNode);
            }
        }

    }


    /* LoadTreeViews --------------------------------------------------------------------*/
    /* TreeType : Tipus de arbre (1,2,3)
    /* NodeId   :
    /* NodePath :
    /* -----------------------------------------------------------------------------------*/
    this.LoadTreeViews = async function (bTree1, bTree2, bTree3, reloadFullData) {

        let tree1Loaded = false;
        if (typeof (bTree1) != 'undefined') LoadTree1 = bTree1;
        if (typeof (bTree2) != 'undefined') LoadTree2 = bTree2;
        if (typeof (bTree3) != 'undefined') LoadTree3 = bTree3;

        if (typeof (reloadFullData) != 'undefined') reLoadGroups = reloadFullData;

        if (LoadTree1) {
            await this.LoadTreeSync('1', tree1Loaded);
            await this.TreeSelectNode('1', await getSelected('1', objPrefix), await getSelectedPath('1', objPrefix));
            tree1Loaded = true;
        }

        if (LoadTree2) {
            await this.LoadTreeSync('2', tree1Loaded);
            await this.TreeSelectNode('2', await getSelected('2', objPrefix), await getSelectedPath('2', objPrefix));
        }

        if (LoadTree3) {
            await this.LoadTreeSync('3', tree1Loaded);
        }

        await this.clickTree(await getActiveTreeType(objPrefix));
    }

    /* isTreeLoad (es troba l'arbre seleccionat carregat )--------------------------------*/
    /* tab      : Tipus de arbre (1,2,3)
    /* 
    /* 
    /* -----------------------------------------------------------------------------------*/
    this.isTreeLoad = function (tab) {
        let valBool = false;
        switch (tab) {
            case '1': { valBool = LoadTree1; break }
            case '2': { valBool = LoadTree2; break; }
            case '3': { valBool = LoadTree3; break; }
        }
        return valBool;
    }

    /* LoadTree --------------------------------------------------------------------*/
    /* TreeType : Tipus de arbre (1,2,3)
    /* objPrefix: Prefix per identificar els elements
    /* -----------------------------------------------------------------------------------*/
    this.LoadTree = function (TreeType, tree1Loaded) {
        switch (TreeType) {
            case '1':
                if (!tree1Loaded) { this.LoadTreeExt(TreeType); }
                break;
            case '2':
            case '3':
                if (!tree1Loaded) { this.LoadTreeExt('1'); }
                this.LoadTreeExt(TreeType);
                break;
        }
    }

    this.LoadTreeSync = async function (TreeType, tree1Loaded, callback) {

        if (typeof callback == 'function') this.onLoadCallback = callback;

        switch (TreeType) {
            case '1':
                if (!tree1Loaded) { await this.LoadTreeExtSync(TreeType); }
                break;
            case '2':
            case '3':
                if (!tree1Loaded) { await this.LoadTreeExtSync('1'); }
                await this.LoadTreeExtSync(TreeType);
                break;
        }
    }

    this.SelectFirstNode = function (TreeType) {
        let oTree = null;
        switch (TreeType) {
            case '1':
                oTree = roTree1;
                break;
            case '2':
                oTree = roTree2;
                break;
            case '3':
                oTree = roTree3;
                break;
        }
        if (oTree != null) oTree.SelectFirstNodeAndClick(true);
    }

    this.LoadTreeExt = function (TreeType) {

        let UserField = getUserField(objPrefix);
        let FieldFind = getFieldFind(objPrefix);


        if (isPromise(UserField)) {
            UserField.then(resUserField => FieldFind.then(resFieldFind => this.endLoadTreeExt(TreeType, resUserField, resFieldFind)));
        }
        else {
            this.endLoadTreeExt(TreeType, UserField, FieldFind);
        }
    }

    this.LoadTreeExtSync = async function (TreeType) {

        let UserField = await getUserField(objPrefix);
        let FieldFind = await getFieldFind(objPrefix);

        this.endLoadTreeExt(TreeType, UserField, FieldFind);
    }

    this.endLoadTreeExt = function (TreeType, UserField, FieldFind) {
        switch (TreeType) {
            case '1':
                {
                    let oTr = document.getElementById(objPrefix + '_tree-div');
                    oTr.innerHTML = '';
                    let myMask = new Ext.LoadMask(objPrefix + '_tree-div', { msg: "..." });
                    myMask.show();

                    this.reloadTree('1');
                    LoadTree1 = true;
                    break;
                }
            case '2':
                {
                    let oTr = document.getElementById(objPrefix + '_tree-div2');
                    oTr.innerHTML = '';

                    let myMask = new Ext.LoadMask(objPrefix + '_tree-div2', { msg: "..." });

                    if (UserField != '') {
                        myMask.show();
                        this.reloadTree('2');
                    }
                    LoadTree2 = true;
                    break;
                }
            case '3':
                {
                    let oTr = document.getElementById(objPrefix + '_tree-div3');
                    oTr.innerHTML = '';
                    if (FieldFind[1] != '') {
                        let myMask = new Ext.LoadMask(objPrefix + '_tree-div3', { msg: "..." });
                        myMask.show();

                        this.reloadTree('3');
                        LoadTree3 = true;
                    }
                    break;
                }
        }
    }

    /* LoadTreeUserField --------------------------------------------------------------------*/
    /* TreeType : Tipus de arbre (1,2,3)
    /* objPrefix: Prefix per identificar els elements
    /* -----------------------------------------------------------------------------------*/
    this.LoadTreeUserField = function () {
        let divTree = document.getElementById(objPrefix + '_tree-div2');
        divTree.innerHTML = '';
        if (getUserField(objPrefix) != '') this.reloadTree('2');
    }



    /* RenameSelectedNode: Actualiza el nombre del nodo seleccionado de los árboles ------*/
    /* textNode: Nou texte a possar en el node seleccionat
    /*
    /* -----------------------------------------------------------------------------------*/
    this.RenameSelectedNode = function (textNode) {

        let oTree = null;
        let TreeType;

        for (let n = 1; n <= 3; n++) {

            switch (n) {
                case 1: { oTree = roTree1; TreeType = '1'; break; }
                case 2: { oTree = roTree2; TreeType = '2'; break; }
                case 3: { oTree = roTree3; TreeType = '3'; break; }
            }

            if (oTree != null) {
                let oNode = oTree.getNodeById(getSelected(TreeType, objPrefix));
                if (oNode != null) {
                    oNode.setText(textNode);
                }
            }
        }
    }

    /* DeleteSelectedNode (Borra el Node seleccionat de tots els arbres)------------------*/
    /*
    /*
    /* -----------------------------------------------------------------------------------*/
    this.DeleteSelectedNode = async function () {
        let oTree = null;
        let TreeType;

        let oNode = null;
        let oParentNode = null;
        let oSelNode = null;
        let SelTreeType = '';

        for (let n = 1; n <= 3; n++) {

            switch (n) {
                case 1: { oTree = roTree1; TreeType = '1'; break; }
                case 2: { oTree = roTree2; TreeType = '2'; break; }
                case 3: { oTree = roTree3; TreeType = '3'; break; }
            }

            if (oTree != null) {
                // Seleccionamos el nodo a borrar
                oNode = oTree.getNodeById(await getSelected(TreeType, objPrefix));
                if (oNode != null) {
                    // Buscamos el nodo previo o si no existe, el nodo padre.
                    if (oSelNode == null) {
                        oSelNode = oNode.previousSibling;
                        if (oSelNode == null) oSelNode = oNode.nextSibling;
                        if (oSelNode == null) oSelNode = oNode.parentNode;
                        if (oSelNode != null) SelTreeType = TreeType;
                    }
                    // Borramos el nodo seleccionado
                    if (oNode.parentNode != null) {
                        oParentNode = oNode.parentNode;
                        oParentNode.removeChild(oNode);
                    }
                }
            }
        }

        // Seleccionamos el nuevo nodo y posicionamos los árboles
        if (oSelNode != null) {

            await setSelected(oSelNode.id, SelTreeType, objPrefix);
            await setSelectedPath(oSelNode.getPath(), SelTreeType, objPrefix);

            switch (SelTreeType) {
                case '1': {  // Árbol grupos
                    if (roTree2 != null) {
                        if (await getSelected('2', objPrefix) != await getSelected(SelTreeType, objPrefix)) {
                            if (oSelNode.id.substr(0, 1) == 'B') {
                                await setSelected(oSelNode.id, '2', objPrefix);
                                // Obtener la ruta del empleado seleccionado por el árbol 2
                                this.GetSelectedPath('2', await getSelected(SelTreeType, objPrefix), true, objPrefix);
                            }
                            else {
                                await setSelectedPath(null, '2', objPrefix);
                            }
                        }
                    }
                    break;
                }

                case '2': {   // Árbol campos ficha empleado
                    if (roTree1 != null) {
                        if (await getSelected('1', objPrefix) != await getSelected(SelTreeType, objPrefix)) {
                            if (oSelNode.id.substr(0, 1) == 'B') {
                                await setSelected(oSelNode.id, '1', objPrefix);
                                // Obtener la ruta del empleado seleccionado por el árbol 1
                                this.GetSelectedPath('1', await getSelected(SelTreeType, objPrefix), true, objPrefix);
                            }
                            else {
                                await setSelectedPath(null, '1', objPrefix);
                            }
                        }
                    }
                    break;
                }
            }

            this.TreeSelectNode('1', await getSelected('1', objPrefix), await getSelectedPath('1', objPrefix));
            this.TreeSelectNode('2', await getSelected('2', objPrefix), await getSelectedPath('2', objPrefix));
            this.TreeSelectNode('3', await getSelected('3', objPrefix), await getSelectedPath('3', objPrefix));
            this.clickTree('1');
        }
    }

    /* GetSelectedPath (Obte el Path del Arbre desde el Servidor) ------------------------*/
    /* TreeType : Tipus de arbre (1,2,3)
    /* objPrefix: Prefix per identificar els elements
    /* -----------------------------------------------------------------------------------*/
    this.GetSelectedPath = function (TreeType, SelectedID, disReposition, objPrefix, launch) {
        let self = this;
        let UserField = getUserField(objPrefix);
        let selectedItem = getSelected(TreeType, objPrefix)


        if (isPromise(SelectedID)) {
            SelectedID.then(result => UserField.then(resUserField => selectedItem.then(resSelectedItem => self.endGetSelectedPath(TreeType, result, disReposition, objPrefix, launch, resUserField, resSelectedItem))));
        } else {
            if (isPromise(UserField)) {
                UserField.then(resUserField => selectedItem.then(resSelectedItem => self.endGetSelectedPath(TreeType, SelectedID, disReposition, objPrefix, launch, resUserField, resSelectedItem)));
            } else {
                this.endGetSelectedPath(TreeType, SelectedID, disReposition, objPrefix, launch, UserField, resSelectedItem);
            }
        }

    }
    this.endGetSelectedPath = function (TreeType, SelectedID, disReposition, objPrefix, launch, pUserField, selectedItem) {
        // Establece las cookies de selección del elemento seleccionado para el árbol indicado
        let UserField = '';
        if (TreeType == '2') UserField = pUserField;

        let stamp = '&StampParam=' + new Date().getMilliseconds();

        if (selectedItem != SelectedID) {

            let _ajax = nuevoAjax();
            let rPath = eval(objPrefix + '_Path');
            let selPage = eval("selPage" + TreeType);
            _ajax.open("GET", rPath + selPage + "?action=getSelectionPath&node=" + SelectedID + "&TreeType=" + TreeType + "&UserField=" + UserField + stamp, true);

            _ajax.onreadystatechange = async function () {
                if (_ajax.readyState == 4) {
                    let Path = _ajax.responseText;

                    await setSelectedPath(Path, TreeType, objPrefix);

                    let oTree = null;
                    switch (TreeType) {
                        case '1': { oTree = roTree1; break; }
                        case '2': { oTree = roTree2; break; }
                        case '3': { oTree = roTree3; break; }
                    }

                    if (oTree != null) {

                        let oTreeObj = eval(objPrefix + "_roTrees");

                        oTreeObj.TreeSelectNode(TreeType, await getSelected(TreeType, objPrefix), await getSelectedPath(TreeType, objPrefix));
                        if (launch) {
                            if (await getSelectedPath(TreeType, objPrefix) == '/source/') {
                                oTree.SelectFirstNode(true, true);
                            }
                            else {
                                oTreeObj.clickTree(TreeType);
                            }
                        }
                        if (typeof oTreeObj.onLoadCallback == 'function') {
                            oTreeObj.onLoadCallback();
                            oTreeObj.onLoadCallback = null;
                        }
                    }
                }
            }

            _ajax.send(null)
        } else if (typeof this.onLoadCallback == 'function') {
            this.onLoadCallback();
            this.onLoadCallback = null;
        }

    }

    /* clickTree (Event click sobre el arbre) ------------------------*/
    /* TreeType : Tipus de arbre (1,2,3)
    /* 
    /* -----------------------------------------------------------------------------------*/
    this.clickTree = function (TreeType) {
        var oTree = null;
        switch (TreeType) {
            case '1': { oTree = roTree1; break; }
            case '2': { oTree = roTree2; break; }
            case '3': { oTree = roTree3; break; }
        }

        if (oTree != null) {
            oTree.click();
        }
    }

    /* getAllRootNodes (Funcion para obtener todos los ids de los nodos raiz del arbol seleccionado) ------------------------*/
    /* TreeType : Tipus de arbre (1,2,3)
    /* 
    /* -----------------------------------------------------------------------------------*/
    this.getAllRootNodes = function (TreeType) {
        var oTree = null;
        switch (TreeType) {
            case '1': { oTree = roTree1; break; }
            case '2': { oTree = roTree2; break; }
            case '3': { oTree = roTree3; break; }
        }

        if (oTree != null) {
            var nodeIDs = '';
            for (var i = 0; i < oTree.roTree().root.childNodes.length; i++) {
                if (nodeIDs != '') nodeIDs += ',';

                var selNodeID = oTree.roTree().root.childNodes[i].id;

                nodeIDs += selNodeID.replace(',', '#');
            }
            return nodeIDs;
        } else {
            return "";
        }

    }

    /* NavigateDown Navega dintre de l'arbre al node seleccionat ------------------------*/
    /* TreeType : Tipus de arbre (1,2,3)
    /* GroupID
    /* -----------------------------------------------------------------------------------*/
    this.NavigateDown = async function (TreeType, GroupID) {
        var sel = await getSelected(TreeType, objPrefix);
        var selPath = await getSelectedPath(TreeType, objPrefix);

        if (sel.charAt(0) == 'A') {
            //Si es un grup
            selPath = selPath + "/A" + GroupID;
            sel = "A" + GroupID;
        } else if (sel.charAt(0) == 'B') {
            //Si es un usuari
            var arrNodes = new Array();
            arrNodes = selPath.split("/");
            selPath = ""
            for (n = 1; n < arrNodes.length - 1; n++) {
                selPath = selPath + "/" + arrNodes[n];
            }
            selPath = selPath + "/A" + GroupID;
            sel = "A" + GroupID;
        } else {
            //Si es una altre cosa... (grup personalitzat)
            return false;
        }
        this.TreeSelectNode(TreeType, sel, selPath, true);
    }

    /* NavigateUp Navega dintre de l'arbre cap amunt ------------------------*/
    /* TreeType : Tipus de arbre (1,2,3)
    /* GroupID
    /* -----------------------------------------------------------------------------------*/
    this.NavigateUp = async function (TreeType) {
        //alert('TreeType = ' + TreeType + ' objPrefix = ' + objPrefix);        
        var sel = await getSelected(TreeType, objPrefix);
        var selPath = await getSelectedPath(TreeType, objPrefix);

        if (sel.charAt(0) == 'A') {
            //Si es un grup
            var arrNodes = new Array();
            arrNodes = selPath.split("/");
            selPath = ""
            for (n = 1; n < arrNodes.length - 1; n++) {
                selPath = selPath + "/" + arrNodes[n];
                if (n == arrNodes.length - 1) {
                    sel = arrNodes[n];
                }
            }
        } else if (sel.charAt(0) == 'B') {
            //Si es un usuari
            var arrNodes = new Array();
            arrNodes = selPath.split("/");
            selPath = ""
            for (n = 1; n < arrNodes.length - 2; n++) {
                selPath = selPath + "/" + arrNodes[n];
                if (n == arrNodes.length - 2) {
                    sel = arrNodes[n];
                }
            }

        } else {
            //Si es una altre cosa... (grup personalitzat)
            return false;
        }
        this.TreeSelectNode(TreeType, sel, selPath, true);
    }


    this.reLoadGroupsFromDB = function () {
        reLoadGroups = true;
    }


    this.BuscaNodoFiltrado = function (nodoId) {

        if (nodoId.indexOf("B") == 0) {
            let tmpNodeId = nodoId.substr(1);
            $.ajax({
                url: `/Employee/GetEmployeeTreeSelectionPath/${tmpNodeId}`,
                data: {},
                type: "GET",
                dataType: "json",
                success: async (data) => {
                    if (typeof data != 'string') {
                        await setSelected(data.EmployeePath, "1", objPrefix);
                        await setSelectedPath(data.GroupSelectionPath, "1", objPrefix);

                        var oTree = roTree1;
                        if (oTree != null) {
                            eval(objPrefix + "_roTrees.TreeSelectNode('1', '" + data.EmployeePath + "', '" + data.GroupSelectionPath + "')");
                            if (await getSelectedPath("1", objPrefix) == '/source/') {
                                oTree.SelectFirstNode(true, true);
                            }
                        }

                    } else {
                        DevExpress.ui.notify(data, 'error', 2000);
                    }
                },
                error: (error) => console.error(error),
            });
        }




        //var _ajax = nuevoAjax();
        //var rPath = eval(objPrefix + '_Path');
        //var selPage = eval("selPage1");
        //_ajax.open("GET", rPath + selPage + "?action=getSelectionPath&node=" + nodoId + "&TreeType=1&UserField=&StampParam=" + new Date().getMilliseconds(), true);

        //_ajax.onreadystatechange = async function () {
        //    if (_ajax.readyState == 4) {
        //        var Path = _ajax.responseText;

        //        await setSelected(nodoId, "1", objPrefix);
        //        await setSelectedPath(Path, "1", objPrefix);

        //        var oTree = roTree1;
        //        if (oTree != null) {
        //            eval(objPrefix + "_roTrees.TreeSelectNode('1', nodoId, Path)");
        //            if (await getSelectedPath("1", objPrefix) == '/source/') {
        //                oTree.SelectFirstNode(true, true);
        //            }
        //        }
        //    }
        //}
        //_ajax.send(null)

    }

    //Posiciona els arbres a la seleccio guardada...
    //Primer Load reposicionant...
    //this.TreeSelectNode('1', getSelected('1', objPrefix), getSelectedPath('1', objPrefix));
    //this.TreeSelectNode('2', getSelected('2',objPrefix), getSelectedPath('2',objPrefix));
}  // End Class roTrees

function Refresh(objPrefix) {
    createCookie(objPrefix + '_EmployeeSelector_FilterUserFieldsApply', true, 30);
    __doPostBack('imgFilterUserFields', '');
    eraseCookie(objPrefix + '_EmployeeSelector_FilterUserFieldsApply');
}

/************************************************************
 Funcions COOKIES per els Arbres
**************************************************************/

/* (SET) Cookie Arbre Actiu */
async function setActiveTreeType(strTreeType, objPrefix) {
    let oTreeState = await getroTreeState(objPrefix);
    oTreeState.setActiveTreeType(strTreeType);
}

/* (GET) Cookie Arbre Actiu */
async function getActiveTreeType(objPrefix) {
    let oTreeState = await getroTreeState(objPrefix);
    let strTreeType = oTreeState.getActiveTreeType();
    if (strTreeType == '' || strTreeType == 'undefined' || strTreeType == null) strTreeType = '1';
    return strTreeType;
}

/* (SET) Funciones para guardar y obtener la configuración de filtros */
async function setFilter(strFilter, strUserFieldFilter, objPrefix) {
    let oTreeState = await getroTreeState(objPrefix);
    oTreeState.setFilter(strFilter, strUserFieldFilter);
}

/* (SET) Funciones para guardar y obtener la configuración de filtros */
async function getFilter(objPrefix) {
    let oTreeState = await getroTreeState(objPrefix);

    let Filters = new Array(2);
    Filters = oTreeState.getFilter();
    return Filters;

}

/* (SET) Funciones para guardar y obtener el campo de la ficha utilizado para generar el segundo árbol */
async function setUserField(strUserField, objPrefix) {
    let oTreeState = await getroTreeState(objPrefix);
    oTreeState.setUserField(strUserField);
}

/* (GET) Funciones para guardar y obtener el campo de la ficha utilizado para generar el segundo árbol */
async function getUserField(objPrefix) {
    let oTreeState = await getroTreeState(objPrefix);
    return oTreeState.getUserField();
}

/* (SET) Funciones para guardar y obtener los nodos seleccionados de los árboles */
async function setSelected(id, TreeType, objPrefix) {
    let oTreeState = await getroTreeState(objPrefix);
    oTreeState.setSelected(id, TreeType);
}

/* (GET) Funciones para guardar y obtener los nodos seleccionados de los árboles */
async function getSelected(TreeType, objPrefix) {
    let oTreeState = await getroTreeState(objPrefix);
    return oTreeState.getSelected(TreeType);
}

async function setSelectedPath(val, TreeType, objPrefix) {
    let oTreeState = await getroTreeState(objPrefix);
    oTreeState.setSelectedPath(val, TreeType);
}
async function getSelectedPath(TreeType, objPrefix) {
    let oTreeState = await getroTreeState(objPrefix);
    return oTreeState.getSelectedPath(TreeType);
}

async function setExpanded(NodeIds, TreeType, objPrefix) {
    let oTreeState = await getroTreeState(objPrefix);
    oTreeState.setExpanded(NodeIds, TreeType);
}
async function getExpanded(TreeType, objPrefix) {
    let oTreeState = await getroTreeState(objPrefix);
    return oTreeState.getExpanded(TreeType);
}

//Carrega les cookies per el tercer treeview
async function setFieldFind(FieldFindColumn, FieldFindValue, objPrefix) {
    let oTreeState = await getroTreeState(objPrefix);
    oTreeState.setFieldFind(FieldFindColumn, FieldFindValue);
}

async function getFieldFind(objPrefix) {
    let oTreeState = await getroTreeState(objPrefix);
    return oTreeState.getFieldFind();
}



/* Cambia entre TABS dels arbres --------------------------------------------------------------------*/
function chTree(tabTreeSel, objPrefix) {

    if (isPromise(tabTreeSel)) {
        tabTreeSel.then(function (result) {
            endChTree(result, objPrefix);
        });
    } else {
        endChTree(tabTreeSel, objPrefix);
    }

}
function endChTree(tabTreeSel, objPrefix) {
    var TreeLoaded = false;

    var caja = document.getElementById('caja');

    var tab01 = document.getElementById(objPrefix + '_tabTree01');
    var tab02 = document.getElementById(objPrefix + '_tabTree02');
    var tab03 = document.getElementById(objPrefix + '_tabTree03');

    var selectAll = document.getElementById(objPrefix + '_selectAllTree');

    var tree01 = document.getElementById(objPrefix + '_tree-div');
    var tree02 = document.getElementById(objPrefix + '_tree-div-UF');
    var tree03 = document.getElementById(objPrefix + '_tree-div-FF');

    switch (tabTreeSel) {
        case '1':
            if (tab01 != null) { tab01.className = 'tabSel1-active'; }
            if (tab02 != null) { tab02.className = 'tabSel2'; }
            if (tab03 != null) { tab03.className = 'tabSel3'; }

            if (tree01 != null) { tree01.style.display = ''; }
            if (tree02 != null) { tree02.style.display = 'none'; }
            if (tree03 != null) { tree03.style.display = 'none'; }

            if (caja != null) { caja.className = 'tabSel1-active' }

            if (selectAll != null) { selectAll.style.display = ''; }

            break;

        case '2':
            if (tab01 != null) { tab01.className = 'tabSel1'; }
            if (tab02 != null) { tab02.className = 'tabSel2-active'; }
            if (tab03 != null) { tab03.className = 'tabSel3'; }

            if (tree01 != null) { tree01.style.display = 'none'; }
            if (tree02 != null) { tree02.style.display = ''; }
            if (tree03 != null) { tree03.style.display = 'none'; }

            if (caja != null) { caja.className = 'tabSel2-active' }

            if (selectAll != null) { selectAll.style.display = 'none'; }

            break;

        case '3':
            if (tab01 != null) { tab01.className = 'tabSel1'; }
            if (tab02 != null) { tab02.className = 'tabSel2'; }
            if (tab03 != null) { tab03.className = 'tabSel3-active'; }

            if (tree01 != null) { tree01.style.display = 'none'; }
            if (tree02 != null) { tree02.style.display = 'none'; }
            if (tree03 != null) { tree03.style.display = ''; }

            if (caja != null) { caja.className = 'tabSel3-active' }

            if (selectAll != null) { selectAll.style.display = ''; }

            break;
    }

    // Informar la cookie de tree seleccionado
    setActiveTreeType(tabTreeSel, objPrefix);

    let treeObject = null;
    eval("treeObject = " + objPrefix + "_roTrees");

    treeObject.isTreeLoad(tabTreeSel);

    if (!TreeLoaded) {
        treeObject.LoadTreeSync(tabTreeSel, false, async function (res) {
            treeObject.TreeSelectNode(tabTreeSel, await getSelected(tabTreeSel, objPrefix), await getSelectedPath(tabTreeSel, objPrefix));
            treeObject.clickTree(tabTreeSel);
        });
    }

}

//Activa / Desactiva els filtres per els arbres
async function UpdTreeFilter(obj, objPrefix, objTrees) {
    var optionFilter = '';
    for (n = 1; n < 6; n++) {
        var treeObj = null;
        switch (n) {
            case 1: treeObj = document.getElementById(objPrefix + '_icoFilt1'); break;
            case 2: treeObj = document.getElementById(objPrefix + '_icoFilt2'); break;
            case 3: treeObj = document.getElementById(objPrefix + '_icoFilt3'); break;
            case 4: treeObj = document.getElementById(objPrefix + '_icoFilt4'); break;
            case 5: treeObj = document.getElementById(objPrefix + '_icoFilt5'); break;
        }

        if (treeObj != null) {
            if (treeObj.id == obj.id) {
                //Es el seleccionat, modifiquem el valor
                var claseObj = new Array();
                claseObj = treeObj.className.split(' ');
                if (claseObj[1] == 'icoUnPressed') {
                    treeObj.className = claseObj[0] + ' icoPressed';
                }
                else {
                    treeObj.className = claseObj[0] + ' icoUnPressed';
                }
            }
            //Recupera les clases (normalment 2 icoFilterx icoEstat)        
            var clase = new Array();
            clase = treeObj.className.split(' ');
            if (clase[1] == 'icoUnPressed') {
                optionFilter = optionFilter + '0';
            }
            else {
                optionFilter = optionFilter + '1';
            }
        }
    }

    // Guardar configuración filtros
    if (objTrees == "")
        await setFilter(optionFilter, null, objPrefix);
    else
        await setFilter(optionFilter, null, objPrefix + '_' + objTrees);

    // Cargar árboles aplicando nuevos filtros
    try {
        var tmp = objTrees + "_roTrees.LoadTreeViews();"
        eval(tmp);
    }
    catch (e) {
        try {
            if (objTrees == "") {
                var tmp = objPrefix + "_roTrees.LoadTreeViews();"
            }
            else {
                var tmp = objPrefix + '_' + objTrees + "_roTrees.LoadTreeViews();"
            }
            eval(tmp);
        }
        catch (ex) {
        }
    }
}

//Activa / Desactiva els filtres per els arbres
async function UpdTreeFilter24(obj, objTrees) {
    var optionFilter = '';
    for (n = 1; n < 7; n++) {
        var treeObj = null;
        switch (n) {
            case 1: treeObj = document.getElementById(objTrees + '_icoFilt1'); break;
            case 2: treeObj = document.getElementById(objTrees + '_icoFilt2'); break;
            case 3: treeObj = document.getElementById(objTrees + '_icoFilt3'); break;
            case 4: treeObj = document.getElementById(objTrees + '_icoFilt4'); break;
            case 5: treeObj = document.getElementById(objTrees + '_icoFilt5'); break;
            case 6: treeObj = document.getElementById(objTrees + '_icoFiltAdv'); break;
        }

        if (treeObj != null) {
            if (treeObj.id == obj.id) {
                //Es el seleccionat, modifiquem el valor
                let claseObj = treeObj.className.split(' ');
                if (claseObj[1] == 'icoUnPressed_24') {
                    treeObj.className = claseObj[0] + ' icoPressed_24';
                }
                else {
                    treeObj.className = claseObj[0] + ' icoUnPressed_24';
                }
            }
            if (n < 6) {
                //Recupera les clases (normalment 2 icoFilterx icoEstat)        
                let clase = treeObj.className.split(' ');
                if (clase[1] == 'icoUnPressed_24') {
                    optionFilter = optionFilter + '0';
                }
                else {
                    optionFilter = optionFilter + '1';
                }
            }
        }
    }

    if (obj.id != (objTrees + '_icoFiltAdv')) {
        // Guardar configuración filtros
        await setFilter(optionFilter, null, objTrees);

        // Cargar árboles aplicando nuevos filtros
        try {
            eval(objTrees + "_roTrees.LoadTreeViews();");
        }
        catch (e) {
            try {
                eval(objTrees + "_roTrees.LoadTreeViews();");
            }
            catch (ex) {
            }
        }
    }
}

function FieldFindBlur(e) {
    //console.log("input blurrred");
}

async function FieldFindChanged(e, objPrefix) {
    //console.log(e.keyCode)
    var FieldFind = await getFieldFind(objPrefix);
    setFieldFind(FieldFind[0], document.getElementById(objPrefix + '_FieldFindValue').value, objPrefix);

    tecla = (document.all) ? e.keyCode : e.which;
    if (tecla == 13 || tecla == 9) {
        eval(objPrefix + "_roTrees.LoadTree('3');");

        // Para cancelar la pulsación de tecla (así en la pantalla de calendario no sale el popup de pegado especial al apretar la tecla enter)
        if (e.preventDefault) {
            e.preventDefault();
        } else {
            e.returnValue = false;
        }
    }
}

function onResizeSelector() {
    var panMax = document.getElementById('panMaximize');
    if (panMax != undefined) {
        switch (BrowserDetect.browser) {
            case 'Firefox':
            case 'Safari':
            case 'Explorer':
            case 'Chrome':
                var arbre1 = document.getElementById('tree-div');
                var arbre2 = document.getElementById('tree-div-UserField');
                var arbre3 = document.getElementById('tree-div-FieldFind');
                panMax.style.height = document.body.offsetHeight - 145 + 'px';
                arbre1.style.height = document.body.offsetHeight - 200 + 'px';
                arbre1.style.maxHeight = document.body.offsetHeight - 200 + 'px';
                arbre2.style.height = document.body.offsetHeight - 250 + 'px';
                arbre2.style.maxHeight = document.body.offsetHeight - 250 + 'px';
                arbre3.style.height = document.body.offsetHeight - 273 + 'px';
                arbre3.style.maxHeight = document.body.offsetHeight - 273 + 'px';
                break;
            default:
                panMax.style.height = document.body.offsetHeight - 130 + 'px';
                break;
        }

    }
}


//=============================================================
//Activa / Desactiva els filtres per els arbres
async function UpdTreeFilter16(obj, objTrees) {
    var optionFilter = '';
    for (n = 1; n < 7; n++) {
        var treeObj = null;
        switch (n) {
            case 1: treeObj = document.getElementById(objTrees + '_icoFilt1'); break;
            case 2: treeObj = document.getElementById(objTrees + '_icoFilt2'); break;
            case 3: treeObj = document.getElementById(objTrees + '_icoFilt3'); break;
            case 4: treeObj = document.getElementById(objTrees + '_icoFilt4'); break;
            case 5: treeObj = document.getElementById(objTrees + '_icoFilt5'); break;
            case 6: treeObj = document.getElementById(objTrees + '_icoFiltAdv'); break;
        }

        if (treeObj != null) {
            if (treeObj.id == obj.id) {
                //Es el seleccionat, modifiquem el valor
                let claseObj = treeObj.className.split(' ');
                if (claseObj[1] == 'icoUnPressed') {
                    treeObj.className = claseObj[0] + ' icoPressed';
                }
                else {
                    treeObj.className = claseObj[0] + ' icoUnPressed';
                }
            }

            if (n < 6) {
                //Recupera les clases (normalment 2 icoFilterx icoEstat)        
                let clase = treeObj.className.split(' ');
                if (clase[1] == 'icoUnPressed') {
                    optionFilter = optionFilter + '0';
                }
                else {
                    optionFilter = optionFilter + '1';
                }
            }
        }
    }

    if (obj.id != (objTrees + '_icoFiltAdv')) {
        // Guardar configuración filtros
        await setFilter(optionFilter, null, objTrees);

        // Cargar árboles aplicando nuevos filtros

        try {
            eval(objTrees + "_roTrees.LoadTreeViews();");
        }
        catch (e) {
            try {
                eval(objTrees + "_roTrees.LoadTreeViews();");
            }
            catch (ex) {
            }
        }
    }
}
