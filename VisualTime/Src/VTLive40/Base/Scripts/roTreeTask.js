
// Clase roTreeTask
function roTreeTask(objPrefix, TreeDiv) {

    var oTreeTask;  // Arbre dintre de la clase
    var root;

    //Url de Carrega
    var LoaderUrl;

    var rPath = eval(objPrefix + '_PathTask');

    //Imatge transparent (necesari per ext)
    Ext.BLANK_IMAGE_URL = rPath + "../ext-3.4.0/resources/images/default/s.gif";

    var oLoader;                            //Carregador

    LoaderUrl = rPath + 'TaskSelectorData.aspx?action=FieldFindData';
    var FieldFind = getFieldFind(objPrefix);
    LoaderUrl = LoaderUrl + "&FieldFindColumn=" + FieldFind[0];
    LoaderUrl = LoaderUrl + "&FieldFindValue=" + encodeURIComponent(FieldFind[1]);

    oLoader = new Ext.tree.TreeLoader({ dataUrl: LoaderUrl, requestMethod: 'POST' })

    Ext.onReady(function() {

        var Tree = Ext.tree;

        // Genera el "root node"
        root = new Tree.AsyncTreeNode({
            nodeType: 'async',
            text: 'rootnode',
            draggable: false,
            id: 'source'
        });

        oTreeTask = new Tree.TreePanel({
                    el: TreeDiv,
                    useArrows: true,
                    autoScroll: true,
                    animate: true,
                    enableDD: false,
                    floating: false,
                    autoShow: true,
                    containerScroll: true,
                    // auto create TreeLoader
                    loader: oLoader,
                    rootVisible: false,
                    rootUIProvider: Ext.tree.CheckboxNodeUI,
                    root: root
        });

        oTreeTask.on('click', function(node, ev) {
            if (node != null) {
                setSelectedNode(node.id, node.text);
            }
            else {
                setSelectedNode("", "");
            }
        }, oTreeTask);  //end onclick

        oTreeTask.getSelectionModel().on('selectionchange', function() {
            if (this.selNode != null) {
                setSelectedNode(this.selNode.id, this.selNode.text);
            }
            else {
                setSelectedNode("", "");
            }
        }, oTreeTask.getSelectionModel()); //end selectionchange


        //load: Carrega el Preloader
        oLoader.on('load', function(itself, nodeLoaded, response) {
            var myMask = new Ext.LoadMask(TreeDiv, { msg: "&nbsp;" });
            myMask.hide();

        }, oLoader); //end load del loader

        oLoader.on('beforeload', function() { return true; }, oLoader);

        if (FieldFind[1] == "") {
        }
        else {
            oTreeTask.render();
        }

    }); //end Ext.Ready

    /* Funcions publiques */
    this.render = function() { oTreeTask.render(); }
    this.SelectFirstNode = function(bolExpand) { SelectFirstNode(bolExpand); }
    this.SelectFirstNodeAndClick = function(bolExpand) { SelectFirstNode(bolExpand, true); }

} // End Class roTreeTask

