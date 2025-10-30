// TreeView
var bolMultiSelect=false; //this.Request.Params['MultiSelect'];
var bolShowOnlyGroups=true;
//var strFilter = '11111'; //Filtres (per omisio, tots actius)
//var strUserFieldFilter = ''; // Valor filtro campos de la ficha del empleado
//var strUserFieldGlobal = '';   //Variable Global UserField

bolMultiSelect = false; //Si es multi-seleccio
bolShowOnlyGroups = false; //Sols mostra els grups

var tree1;  // Árbol empleados por grupos
var tree1ExpandedNodes = null; //Nodos expandidos primer àrbol

var tree2;  // Árbol empleados por campo de la ficha
var tree2ExpandedNodes = null; //Nodos expandidos segundo àrbol

var tree3;  // Árbol para lista búsqueda

var disableReposition = false; //Deshabilita el marcat en "selectionchange" per no fer doble recarrega

Ext.BLANK_IMAGE_URL = hBaseRef + "ext-3.4.0/resources/images/default/s.gif";


function EventReady(TreeType, oDiv, bolExpandFirstNode) {

    var oTree;
    switch (TreeType) {
    case '1': { oTree = tree1; break; }
    case '2': { oTree = tree2; break; }
    case '3': { oTree = tree3; break; }
    }
    
    var LoaderUrl;
    var strUrl = 'Base/WebUserControls/EmployeeSelectorData.aspx';
    if (bolShowOnlyGroups) {
        //LoaderUrl = 'EmployeeSelectorData.aspx?OnlyGroups=1';   
        LoaderUrl = strUrl + '?OnlyGroups=1';   
    }
    else {
        //LoaderUrl = 'EmployeeSelectorData.aspx?OnlyGroups=0';
        LoaderUrl = strUrl + '?OnlyGroups=0';
    }
    
    LoaderUrl = LoaderUrl + '&ImagesPath=base/images/EmployeeSelector';
    
    if (TreeType == '2') LoaderUrl = LoaderUrl + '&UserField=Usr_' + encodeURIComponent(getUserField());
    
    var Filter = getFilter();
    LoaderUrl = LoaderUrl + '&Filters=' + Filter[0];
    LoaderUrl = LoaderUrl + '&FilterUserFields=' + encodeURIComponent(Filter[1]);
    
    if (TreeType == '3') {
        LoaderUrl = LoaderUrl + '&action=FieldFindData';
        var FieldFind = getFieldFind();        
        LoaderUrl = LoaderUrl + "&FieldFindColumn=" + FieldFind[0];
        LoaderUrl = LoaderUrl + "&FieldFindValue=" + FieldFind[1];         
    }
        
    //Mantenir el camp de seleccio per poder recarregar el treeview desde altres funcions
    /*if(strUserField != ''){
        strUserFieldGlobal = strUserField;
    }*/
    
    var oLoader;
    var oSelModel;
    if (bolMultiSelect) {
        LoaderUrl = LoaderUrl + '&MultiSelect=1';
        oLoader = new Ext.tree.CustomUITreeLoader({dataUrl:LoaderUrl,
			                                       baseAttr: {
				                                              uiProvider: Ext.tree.CheckboxNodeUI
			                                                 }
		                                          });
		oSelModel = new Ext.tree.CheckNodeMultiSelectionModel();
    }
    else {
        LoaderUrl = LoaderUrl + '&MultiSelect=0';
        oLoader = new Ext.tree.CustomUITreeLoader({dataUrl:LoaderUrl});
        oSelModel = new Ext.tree.DefaultSelectionModel();
    }
    
	oTree = new Ext.tree.TreePanel(oDiv, {
		animate:true,
		loader:oLoader,
		/*loader: new Ext.tree.CustomUITreeLoader({
			dataUrl:'get-nodes.aspx?ShowOnlyGroups=0',
			baseAttr: {
				uiProvider: Ext.tree.CheckboxNodeUI
			}
		}),*/
		enableDD:false,
		containerScroll: true, 
		rootUIProvider: Ext.tree.CheckboxNodeUI,
		selModel:oSelModel,
		/*selModel:new Ext.tree.CheckNodeMultiSelectionModel(),*/
		rootVisible:false
	});
	
	if (bolMultiSelect) {
	    oTree.on('check', function() {
	        var Selection = this.getChecked().join(',');
		    var Nodes = this.getChecked();
		    var NamesSelection = "";
		    var i;
		    for (i=0; i<Nodes.length; i++) {
		        NamesSelection = NamesSelection + '%' + Nodes[i] + '#' + this.getNodeById(Nodes[i]).text;		                                                            
		    }		    		    
		    strSelected = Selection;
	    }, oTree);
    }
    else {
	    oTree.getSelectionModel().on('selectionchange', function() {
	    
	        if (this.selNode != null) {
	        
	            var Selection = this.selNode.id;
	            var NamesSelection = this.selNode.id + '#' + this.selNode.text;		    
		        // Guardar selección en las cookies

                var bolSaveSelected = true;
    
                //Carga el Nodo (sobreescribe otros scripts)            
                if(disableReposition==false){ //Solo si es el primer reposicionamiento
                    cargaNodo(this.selNode);
                } else {
                    disableReposition=false;
                }
                
		        if(this.selNode.id.indexOf('C') != -1){
		            bolSaveSelected = false;
    		        //Si no es ni grup ni usuari... no fa res...
		        }

                if (bolSaveSelected) {
                    setSelected(this.selNode.id, TreeType);
                    setSelectedPath(this.selNode.getPath(), TreeType);
		            switch (TreeType) {
		                case '1':{  // Árbol grupos
		                            if (getSelected('2') != this.selNode.id) {
		                                if (this.selNode.id.substr(0, 1) == 'B') {
		                                    setSelected(this.selNode.id, '2');
		                                    // Obtener la ruta del empleado seleccionado por el árbol 2
		                                    GetSelectedPath('2', getSelected(TreeType),true);
		                                }
		                                else {
		                                    setSelectedPath(null, '2');
		                                }
		                            }
		                            break; }
    		                      
		                case '2':{   // Árbol campos ficha empleado
		                            if (getSelected('1') != this.selNode.id) {
		                                if (this.selNode.id.substr(0, 1) == 'B') {
		                                    setSelected(this.selNode.id, '1');
		            		                // Obtener la ruta del empleado seleccionado por el árbol 1
		                                    GetSelectedPath('1', getSelected(TreeType),true);
		                                }
		                                else {
		                                    setSelectedPath(null, '1');
		                                }
		                            }
		                            break; }

		            }
		        
		        }
		    }
	    }, oTree.getSelectionModel());
    }
    
    var strExpandedNodes = null;
    switch (TreeType) {
    case '1': strExpandedNodes = tree1ExpandedNodes; break;
    case '2': strExpandedNodes = tree2ExpandedNodes; break;    
    }

    var InitExpanding=false; //Indica que està actualizando la expansión de los nodos del árbol

    var bolTimeout=false;    
    
    oTree.on('expand', function() {

        if (InitExpanding == true) {
        
            var Nodes;
            var oNode;
            var i;
            
            // Expandir nodos hijos
	        var Expanded = strExpandedNodes; 
	        if (Expanded != null) {
                Nodes = Expanded.split(",");    			                
                for (i=0; i<Nodes.length; i++) {
		            oNode = this.getNodeById(Nodes[i]);
		            if (typeof oNode != 'undefined') {
		                if (i == (Nodes.length-1)) InitExpanding = false;
		                if (oNode.isExpanded() == false)
		                    oNode.expand(false);
		            }                
                }                        
            }
        }
        else {
        
            // Actualizar nodos expandidos
            var strExpanded = ExpandedNodes(this.getRootNode());
            switch (TreeType) {
            case '1': treeExpandedNodes = strExpanded; break;
            case '2': treeUserFieldExpandedNodes = strExpanded; break;
            }            
            
            if (bolMultiSelect) {
                var Selection = this.getChecked().join(',');
		        var Nodes = this.getChecked();
		        var NamesSelection = "";
		        var i;
		        for (i=0; i<Nodes.length; i++) {
		            NamesSelection = NamesSelection + '%' + Nodes[i] + '#' + this.getNodeById(Nodes[i]).text;		                                                            
		        }		    		    
		        // Guardar selección en las cookies		        
		        createCookie(cookiePrefix + '_Selector_Selected', Selection, 30); 
            }
            
        }
        
        function ExpandedNodes(node) {    
            var strRet='';
            var n=0;        
            if (node.isExpanded() == true) {
                strRet = node.id + ',';
                var strChilds;
                for (n=0; n<node.childNodes.length; n++) {
                    strChilds = ExpandedNodes(node.childNodes[n]);
                    if (strChilds != '') 
                        strRet = strRet + strChilds + ',';
                }            
            }        
            if (strRet.length > 1) {
                if (strRet.substr(strRet.length-1, strRet.length-1) == ',') 
                    strRet = strRet.substr(0, strRet.length-1)
            }
            return strRet;        
        }
               
    }, oTree);

    oTree.on('collapse', function() {
     
        // Actualizar nodos expandidos
        var strExpanded = ExpandedNodes(this.getRootNode());
        switch (TreeType) {
        case '1': tree1ExpandedNodes = strExpanded; break;
        case '2': tree2ExpandedNodes = strExpanded; break;
        }                
        
        function ExpandedNodes(node) {    
            var strRet='';
            var n=0;        
            if (node.isExpanded() == true) {
                strRet = node.id + ',';
                var strChilds;
                for (n=0; n<node.childNodes.length; n++) {
                    strChilds = ExpandedNodes(node.childNodes[n]);
                    if (strChilds != '') 
                        strRet = strRet + strChilds + ',';
                }            
            }        
            if (strRet.length > 1) {
                if (strRet.substr(strRet.length-1, strRet.length-1) == ',') 
                    strRet = strRet.substr(0, strRet.length-1)
            }
            return strRet;        
        }
        
    }, oTree);
    
    oLoader.on('load', function() {
    
        var Loading = document.getElementById('divLoading');        
        if (Loading != null) {
            Loading.style.display = 'none';
        }
        
    }, oLoader);

    oLoader.on('beforeload', function() {
    
        return true;
        
    }, oLoader);
        
    
	// set the root node
	var root = new Ext.tree.AsyncTreeNode({
		text: 'root',
		draggable:false,
		id:'source',
		uiProvider: Ext.tree.CheckboxNodeUI
	});
	oTree.setRootNode(root);
   
	// render the tree
	oTree.render();		
	
	var Expanded = strExpandedNodes; 	
	if (Expanded != null) {	    
	    InitExpanding = true;
        var Nodes = Expanded.split(",");    			
	    //var arrNodes = new Array(Nodes.length);	
		var i = 0;
		var oNode;
		for (i=0; i<Nodes.length; i++) {
		    oNode = oTree.getNodeById(Nodes[i]);
		    if (typeof oNode != 'undefined') {
		        if (oNode.isExpanded() == false)
		            oNode.expand(false);
		    }
		}
				
	}
	else {
	
	}
			
	function SelectFirstNode() {	    
	    if (bolExpandFirstNode == true) {
            root.expand(false, false, function() {
	            root.firstChild.expand(false);
	            root.firstChild.select();
            });	    
        }
	}
	
    // Establecer la imagen a 'ibtShowUserFields' en función de si está visible los filtros per campos 'panFilterUserFields'
    var panFilterUserFields = document.getElementById('panFilterUserFields');    
    var oImage = document.getElementById('ibtShowUserFields');
    if (oImage != null) {
        if (panFilterUserFields.style.display == 'none') 
            oImage.src = 'Base/Images/EmployeeSelector/collapse.jpg'    
        else 
            oImage.src = 'Base/Images/EmployeeSelector/expand.jpg'    
    }
    
    switch (TreeType) {
    case '1': { tree1 = oTree; break; }
    case '2': { tree2 = oTree; break; }
    case '3': { tree3 = oTree; break; }
    }
    
    
    return true;
    
}

function TreeSelectNode(TreeType, NodeId, NodePath) {
    
    var bolExpandFirstNode = false;
    
    var oTree=null;
    switch (TreeType) {
    case '1': { oTree = tree1; bolExpandFirstNode = true; break; }
    case '2': { oTree = tree2; break; }
    case '3': { oTree = tree3; break; }
    }
    
    if (oTree != null) {
	
	    function SelectFirstNode() {	    
	        if (bolExpandFirstNode == true) {
                oTree.root.expand(false, false, function() {
	                oTree.root.firstChild.expand(false);
	                oTree.root.firstChild.select();
                });	    
            }
	    }
    	
	    // Seleccionar nodo activo.		
        if (NodePath != null) {		
		    oTree.expandPath(NodePath, '', function (bSuccess, oLastNode) {		    
		        var NodeSelected = false;		    
		        if (bSuccess) {		        
		            oLastNode.select();	
		        }
		    });
        
        }
        else
            SelectFirstNode();

    }
    
}

function Refresh() {  
    createCookie(cookiePrefix + '_EmployeeSelector_FilterUserFieldsApply', true, 30);  
    __doPostBack('imgFilterUserFields','');
    eraseCookie(cookiePrefix + '_EmployeeSelector_FilterUserFieldsApply');
}

function ShowUserFieldsImage() {
}

function TreeExpandCollapse(oTree) {   
    var oImage = document.getElementById('ibtTreeExpandCollapse');
    if (oImage.src.indexOf('plus.gif') > -1) {
        oTree.root.expandChildNodes(true);        
        oImage.src = 'Base/Images/EmployeeSelector/minus.gif'
    }
    else {
        oTree.root.collapseChildNodes(true);
        oImage.src = 'Base/Images/EmployeeSelector/plus.gif'
    }   
}


// Carrega per filtre 
function LoadTreeUserField(){
    var divTree = document.getElementById('tree-div-UserField');
    divTree.innerHTML='';
    if (getUserField() != '') EventReady('2', 'tree-div-UserField', false);
}


//Carrega els arbres
function LoadTreeViews(LoadTree1, LoadTree2, LoadTree3){

    if (LoadTree1) {
        LoadTree('1');
        TreeSelectNode('1', getSelected('1'), getSelectedPath('1'));
    }
    if (LoadTree2) {
        LoadTree('2');
        TreeSelectNode('2', getSelected('2'), getSelectedPath('2'));
    }
    if (LoadTree3) {
        LoadTree('3');
    }
    
}

function LoadTree(TreeType) {
    var Loading = document.getElementById('divLoading');    
    switch (TreeType) {
    case '1': { document.getElementById('tree-div').innerHTML='';
                if (Loading != null) Loading.style.display = '';
                EventReady('1','tree-div',true);
                break; }
    case '2': { document.getElementById('tree-div-UserField').innerHTML='';
                if (Loading != null) Loading.style.display = '';
                if (getUserField() != '') EventReady('2', 'tree-div-UserField', false);
                break; }
    case '3': { document.getElementById('tree-div-FieldFind').innerHTML='';
                var FieldFind = getFieldFind();
                if (FieldFind[1] != '') {                    
                    if (Loading != null) Loading.style.display = '';
                    EventReady('3','tree-div-FieldFind',false);
                }
                break; }
    }
}


function setActiveTreeType(strTreeType) {
    createCookie(cookiePrefix + '_Selector_ActiveTree', strTreeType, 30);
}
function getActiveTreeType() {
    var strTreeType = readCookie(cookiePrefix + '_Selector_ActiveTree');
    if (strTreeType == '' || strTreeType == 'undefined' || strTreeType == null) strTreeType = '1';
    return strTreeType;
}


// Funciones para guardar y obtener la configuración de filtros
function setFilter(strFilter, strUserFieldFilter) {
    createCookie(cookiePrefix + '_Selector_Filter', strFilter, 30);
    if (strUserFieldFilter != null) 
        createCookie(cookiePrefix + '_Selector_UserFieldFilter', strUserFieldFilter, 30);
}
function getFilter() {
    var Filters = new Array(2);
    
    var strFilter = readCookie(cookiePrefix + '_Selector_Filter');
    if (strFilter == '' || strFilter == 'undefined' || strFilter == null) strFilter = '11111';        
    Filters[0] = strFilter;
    
    var strUserFieldFilter = readCookie(cookiePrefix + '_Selector_UserFieldFilter');
    if (strUserFieldFilter == 'undefined' || strUserFieldFilter == null) strUserFieldFilter = '';
    Filters[1] = strUserFieldFilter;
    
    return Filters;
}

// Funciones para guardar y obtener el campo de la ficha utilizado para generar el segundo árbol
function setUserField(strUserField) {
    createCookie(cookiePrefix + '_Selector_UserField', strUserField, 30);
}
function getUserField() {
    var strUserField = readCookie(cookiePrefix + '_Selector_UserField');
    if (strUserField == 'undefined' || strUserField == null) strUserField = '';
    return strUserField;
}

// Funciones para guardar y obtener los nodos seleccionados de los árboles
function setSelected(id, TreeType) {
    createCookie(cookiePrefix + '_Selector_Selected' + TreeType, id, 30); 		        
}
function getSelected(TreeType) {
    return readCookie(cookiePrefix + '_Selector_Selected' + TreeType);
}
function setSelectedPath(path, TreeType) {
    createCookie(cookiePrefix + '_Selector_SelectedPath' + TreeType, path, 30); 		        
}
function getSelectedPath(TreeType) {
    return readCookie(cookiePrefix + '_Selector_SelectedPath' + TreeType);
}

//Carrega les cookies per el tercer treeview
function setFieldFind(FieldFindColumn, FieldFindValue)
{        
    createCookie(cookiePrefix + '_Selector_FieldFindColumn',FieldFindColumn, 30);
    createCookie(cookiePrefix + '_Selector_FieldFindValue',FieldFindValue, 30);
}
function getFieldFind() {
    var FieldFind = new Array(2);
    
    var strColumn = readCookie(cookiePrefix + '_Selector_FieldFindColumn');
    if (strColumn == '' || strColumn == 'undefined' || strColumn == null) strColumn = 'EmployeeName';        
    FieldFind[0] = strColumn;
    
    var strValue = readCookie(cookiePrefix + '_Selector_FieldFindValue');
    if (strValue == 'undefined' || strValue == null) strValue = '';
    FieldFind[1] = strValue;
    
    return FieldFind;
}


function GetSelectedPath(TreeType, SelectedID, disReposition)
{
    // Establece las cookies de selección del elemento seleccionado para el árbol indicado
    var UserField = '';
    if (TreeType == '2') UserField = getUserField();
    
        
    var stamp = '&StampParam=' + new Date().getMilliseconds();

    var _ajax=nuevoAjax();
    _ajax.open("GET", hBaseRef + "WebUserControls/EmployeeSelectorData.aspx?action=getSelectionPath&node=" + SelectedID + "&TreeType="+TreeType + "&UserField="+UserField +stamp ,true);
    
    _ajax.onreadystatechange=function() {        
        if (_ajax.readyState==4) {
            //alert(_ajax.responseText);
            var Path = _ajax.responseText;                       
            setSelectedPath(Path, TreeType);
                        
            var oTree=null;
            switch (TreeType) {
            case '1': { oTree = tree1; break; }
            case '2': { oTree = tree2; break; }
            case '3': { oTree = tree3; break; }
            }            
            if (oTree != null) {
                //alert('Posiciona ' + TreeType + ' ' + getSelectedPath(TreeType));
                disableReposition = disReposition;
                TreeSelectNode(TreeType, getSelected(TreeType), getSelectedPath(TreeType));
            }
            
        }
    }
    
    _ajax.send(null)
    
}

// Elimina el nodo seleccionado de los árboles
function DeleteSelectedNode() {
    
    var oTree = null;
    var TreeType;
    
    var oNode = null;
    var oParentNode = null;
    var oSelNode = null;
    var SelTreeType = '';
    
    for (n=1; n<=3; n++) {
    
        switch (n) {
        case 1: { oTree = tree1; TreeType = '1'; break; }
        case 2: { oTree = tree2; TreeType = '2'; break; }
        case 3: { oTree = tree3; TreeType = '3'; break; }
        }

        if (oTree != null) {    
            // Seleccionamos el nodo a borrar
            oNode = oTree.getNodeById(getSelected(TreeType));
            if (oNode != null) {
                // Buscamos el nodo previo o si no existe, el nodo padre.
                if (oSelNode == null) {
                    oSelNode = oNode.previousSibling;
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
    
        setSelected(oSelNode.id, SelTreeType);
        setSelectedPath(oSelNode.getPath(), SelTreeType);
        
        switch (SelTreeType) {
            case '1':{  // Árbol grupos
                        if (getSelected('2') != getSelected(SelTreeType)) {
                            if (oSelNode.id.substr(0, 1) == 'B') {
                                setSelected(oSelNode.id, '2');
                                // Obtener la ruta del empleado seleccionado por el árbol 2
                                GetSelectedPath('2', getSelected(SelTreeType),true);
                            }
                            else {
                                setSelectedPath(null, '2');
                            }
                        }
                        break; }
                      
            case '2':{   // Árbol campos ficha empleado
                        if (getSelected('1') != getSelected(SelTreeType)) {
                            if (oSelNode.id.substr(0, 1) == 'B') {
                                setSelected(oSelNode.id, '1');
            		            // Obtener la ruta del empleado seleccionado por el árbol 1
                                GetSelectedPath('1', getSelected(SelTreeType),true);
                            }
                            else {
                                setSelectedPath(null, '1');
                            }
                        }
                        break; }

        }
        
        TreeSelectNode('1', getSelected('1'), getSelectedPath('1'));
        TreeSelectNode('2', getSelected('2'), getSelectedPath('2'));
        TreeSelectNode('3', getSelected('3'), getSelectedPath('3'));
                
    }
    
}

// Actualiza el nombre del nodo seleccionado de los árboles
function RenameSelectedNode(textNode) {
    
    var oTree = null;
    var TreeType;
    
    for (n=1; n<=3; n++) {
    
        switch (n) {
        case 1: { oTree = tree1; TreeType = '1'; break; }
        case 2: { oTree = tree2; TreeType = '2'; break; }
        case 3: { oTree = tree3; TreeType = '3'; break; }
        }
    
        if (oTree != null) {    
            oNode = oTree.getNodeById(getSelected(TreeType));
            if (oNode != null) {            
                oNode.setText(textNode);
            }
        }
    
    }
    
}

//Navega dintre de l'arbre al node seleccionat
function NavigateDown(TreeType,GroupID){
    var sel = getSelected(TreeType);
    var selPath = getSelectedPath(TreeType);
    //alert('sel=' + sel + ' selPath=' + selPath);
    
   if (sel.charAt(0)=='A'){
        //Si es un grup
        selPath = selPath + "/A" + GroupID;
        sel = "A" + GroupID;
    } else if(sel.charAt(0)=='B') {
        //Si es un usuari
        var arrNodes = new Array();
        arrNodes = selPath.split("/");
        selPath = ""
        for(n=1;n<arrNodes.length-1;n++){
            selPath = selPath + "/" + arrNodes[n];
        }
        selPath = selPath + "/A" + GroupID;
        sel = "A" + GroupID;
    } else {
        //Si es una altre cosa... (grup personalitzat)
        return false;
    }
    
    TreeSelectNode(TreeType, sel, selPath);
}

//Navega dintre de l'arbre en cap amunt
function NavigateUp(TreeType){
    var sel = getSelected(TreeType);
    var selPath = getSelectedPath(TreeType);
        
    if (sel.charAt(0)=='A'){
        //Si es un grup
        var arrNodes = new Array();
        arrNodes = selPath.split("/");
        selPath = ""
        for(n=1;n<arrNodes.length-1;n++){
            selPath = selPath + "/" + arrNodes[n];
            if(n==arrNodes.length-1){
                sel = arrNodes[n];
            }
        }
    } else if(sel.charAt(0)=='B') {
        //Si es un usuari
        var arrNodes = new Array();
        arrNodes = selPath.split("/");
        selPath = ""
        for(n=1;n<arrNodes.length-2;n++){
            selPath = selPath + "/" + arrNodes[n];
            if(n==arrNodes.length-2){
                sel = arrNodes[n];
            }
        }

    } else {
        //Si es una altre cosa... (grup personalitzat)
        return false;
    }
    TreeSelectNode(TreeType, sel, selPath);
}
