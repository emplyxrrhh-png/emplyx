Type.registerNamespace('Robotics');

Robotics.RowDragOverlayBehavior = function (element) {
    Robotics.RowDragOverlayBehavior.initializeBase(this, [element]);
    this._dataType = "_tableRow";
}

Robotics.RowDragOverlayBehavior.prototype = {
    initialize: function () {
        Robotics.RowDragOverlayBehavior.callBaseMethod(this, 'initialize');

        var element = this.get_element();
        if (this.isMovable()) {
            element.style.cursor = "move";
        }

        this.mouseDownHandler = Function.createDelegate(this, this.onMouseDown);
        $addHandler(element, 'mousedown', this.mouseDownHandler);

        this._disposed = false;
        this._parentTableRow = this.getParentTableRow();
        this._floatTable = null;
        this._dropDiv = null;
        Sys.Preview.UI.DragDropManager.registerDropTarget(this);
    },
    dispose: function () {
        var element = this.get_element();

        if (this._parentTableRow) {
            if (this._parentTableRow.___bounds) {
                this._parentTableRow.___bounds = null;
            }
        }

        Sys.Preview.UI.DragDropManager.unregisterDropTarget(this);

        if (this.mouseDownHandler) {
            $removeHandler(element, 'mousedown', this.mouseDownHandler);
        }

        this.mouseDownHandler = null;
        this._parentTableRow = null;
        this._floatTable = null;
        this._dropDiv = null;
        this._rowIndex = null;
        this._UniqueID = null;
        this._gridViewUniqueID = null;

        Robotics.RowDragOverlayBehavior.callBaseMethod(this, 'dispose');
        this._disposed = true;
    },
    onDragStart: function () {
    },
    onMouseDown: function (domEvent) {
        if (this.isMovable()) {
            window._event = domEvent;
            domEvent.preventDefault();
            this.startDragDrop();
        }
    },
    startDragDrop: function () {
        var element = this.get_element();

        if (!this._floatTable) {
            this._floatTable = this.createFloatTable();
            element.parentNode.insertBefore(this._floatTable, element);
            //document.body.appendChild(this._floatTable);
        }

        Sys.Preview.UI.DragDropManager.startDragDrop(this, this._floatTable, null);
    },
    createFloatTable: function () {
        var floatTable = document.createElement("TABLE");
        var floatTableBody = document.createElement("TBODY");

        floatTable.style.filter = "progid:DXImageTransform.Microsoft.BasicImage(opacity=0.75);";
        floatTable.style.opacity = "0.75";
        floatTable.style.position = "absolute";
        floatTable.style.zIndex = 9000;
        floatTable.style.border = "1px solid black";
        floatTable.style.borderCollapse = "collapse";

        var currentLocation = Sys.UI.DomElement.getLocation(this._parentTableRow);
        Sys.UI.DomElement.setLocation(floatTable, currentLocation.x, currentLocation.y);
        floatTable.style.width = this._parentTableRow.offsetWidth + "px";
        floatTable.style.height = this._parentTableRow.offsetHeight + "px";

        var trClone = this._parentTableRow.cloneNode(true);
        floatTableBody.appendChild(trClone);
        floatTable.appendChild(floatTableBody);
        return floatTable;
    },
    createDropDiv: function () {
        var parentRow = this._parentTableRow;
        var parentRowLocation = Sys.UI.DomElement.getLocation(parentRow);
        var dropDiv = document.createElement("div");
        dropDiv.style.width = parentRow.offsetWidth + "px";
        dropDiv.style.height = "2px";
        dropDiv.style.fontSize = "0px";
        dropDiv.style.backgroundColor = "red";
        Sys.UI.DomElement.setLocation(dropDiv, parentRowLocation.x, parentRowLocation.y);
        return dropDiv;
    },
    get_dragMode: function () {
        return Sys.Preview.UI.DragMode.Move;
    },
    get_dragDataType: function () {
        return this._dataType;
    },
    getDragData: function (context) {
        var data = {};
        data.gridViewUniqueID = this.get_gridViewUniqueID() + '';
        data.rowIndex = this.get_rowIndex() + '';
        return data;
    },
    onDrag: function () {
    },
    onDragEnd: function (cancelled) {
        this.hideVisuals();
    },
    canDrop: function (dragMode, dataType, data) {
        if (this._disposed) {
            return false;
        }
        return (dataType === this._dataType);
    },
    get_dropTargetElement: function () {
        return this._parentTableRow;
    },
    onDragEnterTarget: function (dragMode, dataType, data) {
        this._dropDiv = this.createDropDiv();
        var element = this.get_element();
        element.parentNode.insertBefore(this._dropDiv, element);
    },
    onDragLeaveTarget: function (dragMode, dataType, data) {
        var element = this.get_element();
        element.parentNode.removeChild(this._dropDiv);
        this._dropDiv = null;
    },
    onDragInTarget: function () {
    },
    drop: function (dragMode, dataType, data) {
        if (this.rowMoved(data)) {
            this.hideVisuals();
            var eventTarget = this._UniqueID;
            var eventArgument = data.gridViewUniqueID + ":" + data.rowIndex;
            __doPostBack(eventTarget, eventArgument);
        }
    },
    isMovable: function () {
        return (this._rowIndex != -1);
    },
    rowMoved: function (data) {
        if (data.gridViewUniqueID != this._gridViewUniqueID) {
            return true;
        }
        return ((this._rowIndex != -1) &&
            (data.rowIndex != this._rowIndex));
    },
    hideVisuals: function () {
        var element = this.get_element();
        if (this._floatTable) {
            if (this._floatTable.parentNode === element.parentNode) {
                element.parentNode.removeChild(this._floatTable);
            }
            this._floatTable = null;
        }
        if (this._dropDiv) {
            if (this._dropDiv.parentNode === element.parentNode) {
                element.parentNode.removeChild(this._dropDiv);
            }
            this._dropDiv = null;
        }
    },
    getParentTableRow: function () {
        var element = this.get_element();
        var curElem = element;
        while (curElem) {
            if (curElem.tagName == 'TR') {
                return curElem;
            }
            curElem = curElem.parentNode;
        }
        return null;
    },
    get_rowIndex: function () {
        return this._rowIndex;
    },
    set_rowIndex: function (value) {
        this._rowIndex = value;
    },
    get_UniqueID: function () {
        return this._UniqueID;
    },
    set_UniqueID: function (value) {
        this._UniqueID = value;
    },
    get_gridViewUniqueID: function () {
        return this._gridViewUniqueID;
    },
    set_gridViewUniqueID: function (value) {
        this._gridViewUniqueID = value;
    },
    get_parentTableRow: function () {
        return this._parentTableRow;
    }
}

Robotics.RowDragOverlayBehavior.descriptor = {
    properties: [{ name: 'gridViewUniqueID', type: String },
    { name: 'rowIndex', type: Number },
    { name: 'UniqueID', type: String }]
}
Robotics.RowDragOverlayBehavior.registerClass('Robotics.RowDragOverlayBehavior', Sys.UI.Control, Sys.Preview.UI.IDragSource, Sys.Preview.UI.IDropTarget);

//var dropTargets = Sys.Preview.UI.DragDropManager._getInstance()._dropTargets.length;
//for (var i = 0; i < dropTargets.length; i++) {
//}