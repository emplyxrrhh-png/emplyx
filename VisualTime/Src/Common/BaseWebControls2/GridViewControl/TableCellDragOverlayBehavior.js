Type.registerNamespace('Robotics');

Robotics.TableCellDragOverlayBehavior = function (element) {
    Robotics.TableCellDragOverlayBehavior.initializeBase(this, [element]);
    this._dataType = "_tableCell";
}

Robotics.TableCellDragOverlayBehavior.prototype = {
    initialize: function () {
        Robotics.TableCellDragOverlayBehavior.callBaseMethod(this, 'initialize');

        var element = this.get_element();
        if (this.isMovable()) {
            element.style.cursor = "move";
        }

        this.mouseDownHandler = Function.createDelegate(this, this.onMouseDown);
        $addHandler(element, 'mousedown', this.mouseDownHandler);

        this._disposed = false;
        this._parentTableCell = this.getParentTableCell();
        this._floatTable = null;
        this._dropDiv = null;
        Sys.Preview.UI.DragDropManager.registerDropTarget(this);
    },
    dispose: function () {
        var element = this.get_element();

        if (this._parentTableCell) {
            if (this._parentTableCell.___bounds) {
                this._parentTableCell.___bounds = null;
            }
        }

        Sys.Preview.UI.DragDropManager.unregisterDropTarget(this);

        if (this.mouseDownHandler) {
            $removeHandler(element, 'mousedown', this.mouseDownHandler);
        }

        this.mouseDownHandler = null;
        this._parentTableCell = null;
        this._floatTable = null;
        this._dropDiv = null;
        this._rowIndex = null;
        this._colIndex = null;
        this._UniqueID = null;
        this._gridViewUniqueID = null;

        Robotics.TableCellDragOverlayBehavior.callBaseMethod(this, 'dispose');
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
            //element.parentNode.insertBefore(this._floatTable, element);
            document.body.appendChild(this._floatTable);
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

        var LocationX;
        var LocationY;

        /*var pos = this.getAbsoluteElementPosition(this._parentTableCell);
        LocationX = pos.left;
        LocationY = pos.top;
        floatTable.style.left = LocationX;
        floatTable.style.top = LocationY;*/

        var currentLocation = Sys.UI.DomElement.getLocation(this._parentTableCell);
        var LocationX = currentLocation.x;
        var LocationY = currentLocation.y;
        /*var div = document.getElementById('__gv' + this._gridViewUniqueID + '__div');
        if (div != null) {
            LocationX -= div.offsetLeft; LocationX += div.scrollLeft;
            LocationY -= div.offsetTop; LocationY += div.scrollTop;
        } */
        Sys.UI.DomElement.setLocation(floatTable, LocationX, LocationY);
        floatTable.style.width = this._parentTableCell.offsetWidth + "px";
        floatTable.style.height = this._parentTableCell.offsetHeight + "px";

        var floatTableRow = document.createElement("tr");
        var tdClone = this._parentTableCell.cloneNode(true);
        floatTableRow.appendChild(tdClone);
        floatTableBody.appendChild(floatTableRow);
        floatTable.appendChild(floatTableBody);
        return floatTable;
    },
    createDropDiv: function () {
        var parentCell = this._parentTableCell;
        var dropDiv = document.createElement("div");
        dropDiv.style.filter = "progid:DXImageTransform.Microsoft.BasicImage(opacity=0.50);";
        dropDiv.style.opacity = "0.50";
        dropDiv.style.width = (parentCell.offsetWidth - 8) + "px";
        dropDiv.style.height = (parentCell.offsetHeight - 8) + "px"; //"2px";
        dropDiv.style.fontSize = "0px";
        dropDiv.style.border = "solid 4px blue";
        //dropDiv.style.backgroundColor = "red";
        dropDiv.style.position = "absolute";
        dropDiv.style.zIndex = 9000;

        var parentCellLocation = Sys.UI.DomElement.getLocation(parentCell);
        var LocationX = parentCellLocation.x;
        var LocationY = parentCellLocation.y;
        /*var div = document.getElementById('__gv' + this._gridViewUniqueID + '__div');
        if (div != null) {
            LocationX -= div.offsetLeft; LocationX += div.scrollLeft;
            LocationY -= div.offsetTop; LocationY += div.scrollTop;
        }*/
        Sys.UI.DomElement.setLocation(dropDiv, LocationX, LocationY);
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
        data.colIndex = this.get_colIndex() + '';
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
        return this._parentTableCell;
    },
    onDragEnterTarget: function (dragMode, dataType, data) {
        if (!this._dropDiv) {
            this._dropDiv = this.createDropDiv();
            //var element = this.get_element();
            //element.parentNode.insertBefore(this._dropDiv, element);
            document.body.appendChild(this._dropDiv);
        }
    },
    onDragLeaveTarget: function (dragMode, dataType, data) {
        //var element = this.get_element();
        //element.parentNode.removeChild(this._dropDiv);
        document.body.removeChild(this._dropDiv);
        this._dropDiv = null;
    },
    onDragInTarget: function () {
    },
    drop: function (dragMode, dataType, data) {
        if (this.cellMoved(data)) {
            this.hideVisuals();
            var eventTarget = this._UniqueID;
            var eventArgument = data.gridViewUniqueID + ":" + data.rowIndex + ":" + data.colIndex;
            __doPostBack(eventTarget, eventArgument);
        }
    },
    isMovable: function () {
        return (this._rowIndex != -1 && this._colIndex != -1);
    },
    cellMoved: function (data) {
        if (data.gridViewUniqueID != this._gridViewUniqueID) {
            return true;
        }
        if ((this._rowIndex != -1) && (data.rowIndex != this._rowIndex)) {
            return true;
        }
        return ((this._colIndex != -1) &&
            (data.colIndex != this._colIndex));
    },
    hideVisuals: function () {
        var element = this.get_element();
        if (this._floatTable) {
            if (this._floatTable.parentNode === element.parentNode) {
                element.parentNode.removeChild(this._floatTable);
            }
            else
                document.body.removeChild(this._floatTable);
            this._floatTable = null;
        }
        if (this._dropDiv) {
            if (this._dropDiv.parentNode === element.parentNode) {
                element.parentNode.removeChild(this._dropDiv);
            }
            else
                document.body.removeChild(this._dropDiv);
            this._dropDiv = null;
        }
    },
    getParentTableCell: function () {
        var element = this.get_element();
        var curElem = element;
        while (curElem) {
            if (curElem.tagName == 'TR') {
                return curElem.cells[this._colIndex];
            }
            curElem = curElem.parentNode;
        }
        return null;
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
    get_colIndex: function () {
        return this._colIndex;
    },
    set_colIndex: function (value) {
        this._colIndex = value;
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
    get_parentTableCell: function () {
        return this._parentTableCell;
    },
    getAbsoluteElementPosition: function (element) {
        if (typeof element == "string")
            element = document.getElementById(element)

        if (!element) return { top: 0, left: 0 };

        var y = 0;
        var x = 0;
        while (element.offsetParent) {
            x += element.offsetLeft;
            y += element.offsetTop;
            element = element.offsetParent;
        }
        return { top: y, left: x };
    }
}

Robotics.TableCellDragOverlayBehavior.descriptor = {
    properties: [{ name: 'gridViewUniqueID', type: String },
    { name: 'rowIndex', type: Number },
    { name: 'colIndex', type: Number },
    { name: 'UniqueID', type: String }]
}
Robotics.TableCellDragOverlayBehavior.registerClass('Robotics.TableCellDragOverlayBehavior', Sys.UI.Control, Sys.Preview.UI.IDragSource, Sys.Preview.UI.IDropTarget);

//var dropTargets = Sys.Preview.UI.DragDropManager._getInstance()._dropTargets.length;
//for (var i = 0; i < dropTargets.length; i++) {
//}