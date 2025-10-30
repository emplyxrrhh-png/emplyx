Type.registerNamespace('Robotics.WebControls2.GridViewControl');

Robotics.WebControls2.GridViewControl.GridViewControlBehavior = function(element) {
    Robotics.WebControls2.GridViewControl.GridViewControlBehavior.initializeBase(this, [element]);

    //  Properties
    this._rowHoverCssClass = null;
    this._rowSelectCssClass = null;
    this._columnHoverCssClass = null;
    this._columnSelectCssClass = null;
    this._cellHoverCssClass = null;
    this._cellSelectCssClass = null;
    this._headerCellHoverCssClass = null;
    this._headerCellSelectCssClass = null;
    this._KeysNavigation = null;
    this._SelectedRowIndexCookie = null;
    this._SelectedColIndexCookie = null;

    //  Class names for the datarows
    this._dataRowCssClass;
    this._alternateDataRowCssClass;
    this._headerRowCssClass = null;

    //  Variables
    this._rows = null;
    this._currow=0;
    this._curcell=0;

    this._columns = null;
    this._isResizing = false;
    this._element;
}

Robotics.WebControls2.GridViewControl.GridViewControlBehavior.prototype = {
    initialize : function() {
        Robotics.WebControls2.GridViewControl.GridViewControlBehavior.callBaseMethod(this, 'initialize');

        // get the elements
        this._rows = this.get_element().getElementsByTagName("tr");
        var KeysNavigation = this.get_KeysNavigation();
        /*
        //  add the css class to the row
        var rowHoverCssClass = this.get_RowHoverCssClass();
        var rowSelectCssClass = this.get_RowSelectCssClass();
        */

        for(var i = 0; i < this._rows.length; i++) {
            //  get the row
            var row = this._rows[i];

            for(var j = 0; j < row.cells.length; j++) {
                var args = {rowIndex: i, cellIndex: j, behavior: this};
                var cell = row.cells[j]
                //  attach to the data cell events
                if(this._isDataRow(row)) {
                    $addHandler(cell, 'mouseover', Function.createCallback(this._onDataCellOver, args));
                    $addHandler(cell, 'mouseout', Function.createCallback(this._onDataCellOut, args));
                    $addHandler(cell, 'click', Function.createCallback(this._onDataCellClick, args));
                    if (KeysNavigation == true) {
                        $addHandler(cell, 'keydown', Function.createCallback(this._onDataCellKeyDown, args));
                    }
                    $addHandler(cell, 'contextmenu', Function.createCallback(this._onDataCellContextMenu, args));
                }
                else if(this._isHeaderRow(row)) {
                    $addHandler(cell, 'mouseover', Function.createCallback(this._onHeaderCellOver, args));
                    $addHandler(cell, 'mouseout', Function.createCallback(this._onHeaderCellOut, args));
                }
            }
        }

        /*
        // attach our event handlers to all non pager cells
        for(var i = 0; i < this._rows.length; i++) {
            //  get the row
            var row = this._rows[i];
            var rowindex = i;
            if(this._isDataRow(row)) {
                if(rowHoverCssClass) {
                    //  create the callbacks
                    var rowOver = Function.createCallback(this._onRowOver, {row: row, behavior: this});
                    var rowOut = Function.createCallback(this._onRowOut, {row: row, behavior: this});
                    //  attach to the mouseover and mouseout events
                    $addHandler(row, 'mouseover', rowOver);
                    $addHandler(row, 'mouseout', rowOut);
                }

                if(rowSelectCssClass) {
                    //  create the callback
                    var rowClick = Function.createCallback(this._onRowClick, {row: row, behavior: this});
                    var rowKeyDown = Function.createCallback(this._onRowKeyDown, {rowindex: rowindex, behavior: this});
                    //  attach to the click events
                    $addHandler(row, 'click', rowClick);
                    $addHandler(row, 'keydown', rowKeyDown);
                }
            }
        }
        */

        // get the elements
        this._columns = this.get_element().getElementsByTagName("TH");

        //  if the grid has at least one th element
        if (this._columns.length > 1) {
            for(var i = 0; i < this._columns.length; i++) {
                var column = this._columns[i];

                //  determine the widths
                column.style.width = Sys.UI.DomElement.getBounds(column).width + 'px';

                //  attach the mousemove and mousedown events
                if(i < this._columns.length - 1){
                    //  create the callback
                    var columnMouseMove = Function.createCallback(this._onColumnMouseMove, {column: column, behavior: this});
                    var columnMouseDown = Function.createCallback(this._onColumnMouseDown, {column: column, behavior: this});
                    //  attach to the mouse events
                    $addHandler(column, 'mousemove', columnMouseMove);
                    $addHandler(column, 'mousedown', columnMouseDown);
                }
            }

            //  create the callback
            var MouseUp = Function.createCallback(this._onMouseUp, {behavior: this});
            var SelectStart = Function.createCallback(this._onSelectStart, {behavior: this});
            //  add a global mouseup handler
            $addHandler(document, 'mouseup', MouseUp);
            //  add a global selectstart handler
            $addHandler(document, 'selectstart', SelectStart);
        }
    },

    dispose : function() {
        // remove our event handlers from all data rows
        for(var i = 0; i < this._rows.length; i++) {
            //  get the row
            var row = this._rows[i];
            if(this._isDataRow(row)) {
                //  remove our handler
                $clearHandlers(row);
            }
        }

        Robotics.WebControls2.GridViewControl.GridViewControlBehavior.callBaseMethod(this, 'dispose');
    },

    _isHeaderRow : function(tr) {
        var headerRowClass = this.get_HeaderRowCssClass();
        return (headerRowClass && Sys.UI.DomElement.containsCssClass(tr, headerRowClass));
    },

    _isDataRow : function(tr) {
        var dataRowClass = this.get_DataRowCssClass();
        var altDataRowClass = this.get_AlternateDataRowCssClass();

        return (dataRowClass && Sys.UI.DomElement.containsCssClass(tr, dataRowClass)) || (altDataRowClass && Sys.UI.DomElement.containsCssClass(tr, altDataRowClass));
    },

    _onDataCellOver : function(e, args) {
        //  add the css class to the row
        var headerCellHoverCssClass = args.behavior.get_HeaderCellHoverCssClass();
        var rowHoverCssClass = args.behavior.get_RowHoverCssClass();
        var columnHoverCssClass = args.behavior.get_ColumnHoverCssClass();
        var cellHoverCssClass = args.behavior.get_CellHoverCssClass();
        var rows = args.behavior._rows;

        //  apply the class to all cells in this row
        if(rowHoverCssClass) {
            for(var i = 0; i < rows[args.rowIndex].cells.length; i++) {
                Sys.UI.DomElement.addCssClass(rows[args.rowIndex].cells[i], rowHoverCssClass);
            }
        }

        //  apply the class to all cells in this column (including the header rows cell)
        if(columnHoverCssClass || headerCellHoverCssClass) {
            for(var i = 0; i < rows.length; i++) {
                if(columnHoverCssClass && args.behavior._isDataRow(rows[i])) {
                    Sys.UI.DomElement.addCssClass(rows[i].cells[args.cellIndex], columnHoverCssClass);
                }
                else if(headerCellHoverCssClass && args.behavior._isHeaderRow(rows[i])) {
                    Sys.UI.DomElement.addCssClass(rows[i].cells[args.cellIndex], headerCellHoverCssClass);
                }
            }
        }

        //  apply the class to the cell that raised this event
        if(cellHoverCssClass) {
            Sys.UI.DomElement.addCssClass(rows[args.rowIndex].cells[args.cellIndex], cellHoverCssClass);
        }
    },

    _onDataCellOut : function(e, args) {
        //  remove the css class to the row
        var headerCellHoverCssClass = args.behavior.get_HeaderCellHoverCssClass();
        var rowHoverCssClass = args.behavior.get_RowHoverCssClass();
        var columnHoverCssClass = args.behavior.get_ColumnHoverCssClass();
        var cellHoverCssClass = args.behavior.get_CellHoverCssClass();
        var rows = args.behavior._rows;

        //  remove the class to all cells in this row
        if(rowHoverCssClass) {
            for(var i = 0; i < rows[args.rowIndex].cells.length; i++) {
                Sys.UI.DomElement.removeCssClass(rows[args.rowIndex].cells[i], rowHoverCssClass);
            }
        }

        //  remove the class to all cells in this column (including the header rows cell)
        if(columnHoverCssClass || headerCellHoverCssClass) {
            for(var i = 0; i < rows.length; i++) {
                if(columnHoverCssClass && args.behavior._isDataRow(rows[i])) {
                    Sys.UI.DomElement.removeCssClass(rows[i].cells[args.cellIndex], columnHoverCssClass);
                }
                else if(headerCellHoverCssClass && args.behavior._isHeaderRow(rows[i])) {
                    Sys.UI.DomElement.removeCssClass(rows[i].cells[args.cellIndex], headerCellHoverCssClass);
                }
            }
        }

        //  remove the class to the cell that raised this event
        if(cellHoverCssClass) {
            Sys.UI.DomElement.removeCssClass(rows[args.rowIndex].cells[args.cellIndex], cellHoverCssClass);
        }
    },

    _onDataCellClick : function(e, args) {
        //  remove the classes
        var rowSelectCssClass = args.behavior.get_RowSelectCssClass();
        var columnSelectCssClass = args.behavior.get_ColumnSelectCssClass();
        var cellSelectCssClass = args.behavior.get_CellSelectCssClass();
        var headerCellSelectCssClass = args.behavior.get_HeaderCellSelectCssClass();
        var rows = args.behavior._rows;

        for(var i = 0; i < args.behavior._rows.length; i++) {
            var row = args.behavior._rows[i];
            if(args.behavior._isDataRow(row) || args.behavior._isHeaderRow(row)) {
                for(var j = 0; j < row.cells.length; j++) {
                    if(headerCellSelectCssClass) {
                        Sys.UI.DomElement.removeCssClass(row.cells[j], headerCellSelectCssClass);
                    }
                    if(rowSelectCssClass) {
                        Sys.UI.DomElement.removeCssClass(row.cells[j], rowSelectCssClass);
                    }
                    if(cellSelectCssClass) {
                        Sys.UI.DomElement.removeCssClass(row.cells[j], cellSelectCssClass);
                    }
                    if(columnSelectCssClass) {
                        Sys.UI.DomElement.removeCssClass(row.cells[j], columnSelectCssClass);
                    }
                }
            }
        }

        args.behavior.set_SelectedRowIndex(args.rowIndex);
        args.behavior.set_SelectedColIndex(args.cellIndex);

        // Graba la fila y la columna seleccionada en las cookies (si están informadas las propiedades)
        var SelectedRowIndexCookie = args.behavior.get_SelectedRowIndexCookie();
        if (SelectedRowIndexCookie) {
            args.behavior.createCookie(SelectedRowIndexCookie, args.rowIndex, 30);
        }
        var SelectedColIndexCookie = args.behavior.get_SelectedColIndexCookie();
        if (SelectedColIndexCookie) {
            args.behavior.createCookie(SelectedColIndexCookie, args.cellIndex, 30);
        }

        if (rows.length > args.rowIndex) {
            if(rowSelectCssClass) {
                for(var i = 0; i < rows[args.rowIndex].cells.length; i++) {
                    Sys.UI.DomElement.addCssClass(rows[args.rowIndex].cells[i], rowSelectCssClass);
                }
            }

            if(columnSelectCssClass || headerCellSelectCssClass) {
                for(var i = 0; i < rows.length; i++) {
                    if(columnSelectCssClass && args.behavior._isDataRow(rows[i])) {
                        Sys.UI.DomElement.addCssClass(rows[i].cells[args.cellIndex], columnSelectCssClass);
                    }
                    if(headerCellSelectCssClass && args.behavior._isHeaderRow(rows[i])) {
                        Sys.UI.DomElement.addCssClass(rows[i].cells[args.cellIndex], headerCellSelectCssClass);
                    }
                }
            }

            if(cellSelectCssClass) {
                if (rows[args.rowIndex].cells.length > args.cellIndex) {
                    Sys.UI.DomElement.addCssClass(rows[args.rowIndex].cells[args.cellIndex], cellSelectCssClass);
                }
            }
        }
    },

    _onDataCellContextMenu : function(e, args) {
        //  remove the classes
        var rowSelectCssClass = args.behavior.get_RowSelectCssClass();
        var columnSelectCssClass = args.behavior.get_ColumnSelectCssClass();
        var cellSelectCssClass = args.behavior.get_CellSelectCssClass();
        var headerCellSelectCssClass = args.behavior.get_HeaderCellSelectCssClass();
        var rows = args.behavior._rows;

        for(var i = 0; i < args.behavior._rows.length; i++) {
            var row = args.behavior._rows[i];
            if(args.behavior._isDataRow(row) || args.behavior._isHeaderRow(row)) {
                for(var j = 0; j < row.cells.length; j++) {
                    if(headerCellSelectCssClass) {
                        Sys.UI.DomElement.removeCssClass(row.cells[j], headerCellSelectCssClass);
                    }
                    if(rowSelectCssClass) {
                        Sys.UI.DomElement.removeCssClass(row.cells[j], rowSelectCssClass);
                    }
                    if(cellSelectCssClass) {
                        Sys.UI.DomElement.removeCssClass(row.cells[j], cellSelectCssClass);
                    }
                    if(columnSelectCssClass) {
                        Sys.UI.DomElement.removeCssClass(row.cells[j], columnSelectCssClass);
                    }
                }
            }
        }

        args.behavior.set_SelectedRowIndex(args.rowIndex);
        args.behavior.set_SelectedColIndex(args.cellIndex);

        // Graba la fila y la columna seleccionada en las cookies (si están informadas las propiedades)
        var SelectedRowIndexCookie = args.behavior.get_SelectedRowIndexCookie();
        if (SelectedRowIndexCookie) {
            args.behavior.createCookie(SelectedRowIndexCookie, args.rowIndex, 30);
        }
        var SelectedColIndexCookie = args.behavior.get_SelectedColIndexCookie();
        if (SelectedColIndexCookie) {
            args.behavior.createCookie(SelectedColIndexCookie, args.cellIndex, 30);
        }

        if (rows.length > args.rowIndex) {
            if(rowSelectCssClass) {
                for(var i = 0; i < rows[args.rowIndex].cells.length; i++) {
                    Sys.UI.DomElement.addCssClass(rows[args.rowIndex].cells[i], rowSelectCssClass);
                }
            }

            if(columnSelectCssClass || headerCellSelectCssClass) {
                for(var i = 0; i < rows.length; i++) {
                    if(columnSelectCssClass && args.behavior._isDataRow(rows[i])) {
                        Sys.UI.DomElement.addCssClass(rows[i].cells[args.cellIndex], columnSelectCssClass);
                    }
                    if(headerCellSelectCssClass && args.behavior._isHeaderRow(rows[i])) {
                        Sys.UI.DomElement.addCssClass(rows[i].cells[args.cellIndex], headerCellSelectCssClass);
                    }
                }
            }

            if(cellSelectCssClass) {
                if (rows[args.rowIndex].cells.length > args.cellIndex) {
                    Sys.UI.DomElement.addCssClass(rows[args.rowIndex].cells[args.cellIndex], cellSelectCssClass);
                }
            }
        }
    },

    _onDataCellKeyDown : function(e, args) {
        //  remove the classes
        var rowSelectCssClass = args.behavior.get_RowSelectCssClass();
        var columnSelectCssClass = args.behavior.get_ColumnSelectCssClass();
        var cellSelectCssClass = args.behavior.get_CellSelectCssClass();
        var headerCellSelectCssClass = args.behavior.get_HeaderCellSelectCssClass();
        var rows = args.behavior._rows;

        for(var i = 0; i < args.behavior._rows.length; i++) {
            var row = args.behavior._rows[i];
            if(args.behavior._isDataRow(row) || args.behavior._isHeaderRow(row)) {
                for(var j = 0; j < row.cells.length; j++) {
                    if(headerCellSelectCssClass) {
                        Sys.UI.DomElement.removeCssClass(row.cells[j], headerCellSelectCssClass);
                    }
                    if(rowSelectCssClass) {
                        Sys.UI.DomElement.removeCssClass(row.cells[j], rowSelectCssClass);
                    }
                    if(cellSelectCssClass) {
                        Sys.UI.DomElement.removeCssClass(row.cells[j], cellSelectCssClass);
                    }
                    if(columnSelectCssClass) {
                        Sys.UI.DomElement.removeCssClass(row.cells[j], columnSelectCssClass);
                    }
                }
            }
        }

        var nextrow = args.behavior.get_SelectedRowIndex(); //args.behavior._currow;
        var nextcell = args.behavior.get_SelectedColIndex(); //args.behavior._curcell;

        var k = e.keyCode ? e.keyCode : e.rawEvent.keyCode;
        if (k === Sys.UI.Key.down){
            nextrow = nextrow + 1;
        }
        if(k === Sys.UI.Key.up){
            nextrow = nextrow - 1;
        }
        if (k === Sys.UI.Key.left) {
            nextcell = nextcell -1;
        }
        if (k === Sys.UI.Key.right) {
            nextcell = nextcell + 1;
        }

        if (nextrow >(args.behavior._rows.length-1) )
            nextrow = args.behavior._rows.length-1;
        if (nextrow<=0) nextrow=1;
        args.behavior.set_SelectedRowIndex(nextrow);
        //args.behavior._currow = nextrow;

        if (nextcell >(args.behavior._rows[nextrow].cells.length-1) )
            nextcell = args.behavior._rows[nextrow].cells.length-1;
        if (nextcell <= 0) nextcell = 0;
        args.behavior.set_SelectedRowIndex(nextcell);
        //args.behavior._curcell = nextcell;

        if(rowSelectCssClass) {
            for(var i = 0; i < rows[nextrow].cells.length; i++) {
                Sys.UI.DomElement.addCssClass(rows[nextrow].cells[i], rowSelectCssClass);
            }
        }

        if(cellSelectCssClass) {
            Sys.UI.DomElement.addCssClass(rows[nextrow].cells[nextcell], cellSelectCssClass);
        }

        // Graba la fila y la columna seleccionada en las cookies (si están informadas las propiedades)
        var SelectedRowIndexCookie = args.behavior.get_SelectedRowIndexCookie();
        if (SelectedRowIndexCookie) {
            args.behavior.createCookie(SelectedRowIndexCookie, nextrow, 30);
        }
        var SelectedColIndexCookie = args.behavior.get_SelectedColIndexCookie();
        if (SelectedColIndexCookie) {
            args.behavior.createCookie(SelectedColIndexCookie, nextcell, 30);
        }
    },

    _onHeaderCellOver : function(e, args) {
        //  add the css class to the row
        var headerCellHoverCssClass = args.behavior.get_HeaderCellHoverCssClass();
        var columnHoverCssClass = args.behavior.get_ColumnHoverCssClass();
        var rows = args.behavior._rows;

        //  apply the class to all cells in this column (including the header rows cell)
        if(columnHoverCssClass || headerCellHoverCssClass) {
            for(var i = 0; i < rows.length; i++) {
                if(columnHoverCssClass && args.behavior._isDataRow(rows[i])) {
                    Sys.UI.DomElement.addCssClass(rows[i].cells[args.cellIndex], columnHoverCssClass);
                }
                else if(headerCellHoverCssClass && args.behavior._isHeaderRow(rows[i])) {
                    Sys.UI.DomElement.addCssClass(rows[i].cells[args.cellIndex], headerCellHoverCssClass);
                }
            }
        }
    },

    _onHeaderCellOut : function(e, args) {
        //  remove the css class to the row
        var headerCellHoverCssClass = args.behavior.get_HeaderCellHoverCssClass();
        var columnHoverCssClass = args.behavior.get_ColumnHoverCssClass();
        var rows = args.behavior._rows;

        //  apply the class to all cells in this column (including the header rows cell)
        if(columnHoverCssClass || headerCellHoverCssClass) {
            for(var i = 0; i < rows.length; i++) {
                if(columnHoverCssClass && args.behavior._isDataRow(rows[i])) {
                    Sys.UI.DomElement.removeCssClass(rows[i].cells[args.cellIndex], columnHoverCssClass);
                }
                else if(headerCellHoverCssClass && args.behavior._isHeaderRow(rows[i])) {
                    Sys.UI.DomElement.removeCssClass(rows[i].cells[args.cellIndex], headerCellHoverCssClass);
                }
            }
        }
    },

    /*
    _onRowOver : function(e, args) {
        Sys.UI.DomElement.addCssClass(args.row, args.behavior.get_RowHoverCssClass());
    },

    _onRowOut : function(e, args) {
        Sys.UI.DomElement.removeCssClass(args.row, args.behavior.get_RowHoverCssClass());
    },

    _onRowClick : function(e, args) {
        //  unselect the cells
        for(var i = 0; i < args.behavior._rows.length; i++) {
            if(args.behavior._isDataRow(args.behavior._rows[i])) {
                Sys.UI.DomElement.removeCssClass(args.behavior._rows[i], args.behavior.get_RowSelectCssClass());
            }
        }

        Sys.UI.DomElement.addCssClass(args.row, args.behavior.get_RowSelectCssClass());
    },
    */
    /*
    _onRowKeyDown : function(e, args) {
        //  unselect the cells
        for(var i = 0; i < args.behavior._rows.length; i++) {
            if(args.behavior._isDataRow(args.behavior._rows[i])) {
                Sys.UI.DomElement.removeCssClass(args.behavior._rows[i], args.behavior.get_RowSelectCssClass());
            }
        }

        var k = e.keyCode ? e.keyCode : e.rawEvent.keyCode;
        if (k === Sys.UI.Key.down){
            var nextrow=args.behavior._currow +1;
            if (nextrow >(args.behavior._rows.length-1) ){
                nextrow=args.behavior._rows.length-1;
            }
            var trnext=args.behavior._rows[nextrow];
            trnext.focus();
            Sys.UI.DomElement.addCssClass(trnext, args.behavior.get_RowSelectCssClass());
            //_selrow(trnext);
            args.behavior._currow = nextrow;
        }
        if(k === Sys.UI.Key.up){
            var nextrow=args.behavior._currow -1;
            if (nextrow<=0) nextrow=1;
            var trnext=args.behavior._rows[nextrow];
            trnext.focus();
            Sys.UI.DomElement.addCssClass(trnext, args.behavior.get_RowSelectCssClass());
            //_selrow(trnext);
            args.behavior._currow = nextrow;
        }
    },
    */

    _onColumnMouseMove : function(e, args) {
        if(args.behavior._isResizing){
            //  determine the new width of the header
            var bounds = Sys.UI.DomElement.getBounds(args.behavior._element);
            var width = e.clientX - bounds.x;

            //  we set the minimum width to 1 px, so make
            //  sure it is at least this before bothering to
            //  calculate the new width
            if(width > 1){
                var element = args.behavior._element;

                //  get the next th element so we can adjust its size as well
                var nextColumn = element.nextSibling;
                var nextColumnWidth;
                if(width < args.behavior.toNumber(element.style.width)){
                    //  make the next column bigger
                    nextColumnWidth = args.behavior.toNumber(nextColumn.style.width) + args.behavior.toNumber(element.style.width) - width;
                }
                else if(width > args.behavior.toNumber(element.style.width)){
                    //  make the next column smaller
                    nextColumnWidth = args.behavior.toNumber(nextColumn.style.width) - (width - args.behavior.toNumber(element.style.width));
                }

                //  we also don't want to shrink this width to less than one pixel,
                //  so make sure of this before resizing ...
                if(nextColumnWidth > 1){
                    element.style.width = width + 'px';
                    nextColumn.style.width = nextColumnWidth + 'px';
                }
            }
        }
        else{
            //  get the bounds of the element.  If the mouse cursor is within
            //  2px of the border, display the e-cursor -> cursor:e-resize
            var bounds = Sys.UI.DomElement.getBounds(e.target);
            if(Math.abs((bounds.x + bounds.width) - (e.clientX)) <= 2) {
                e.target.style.cursor = 'e-resize';
            }
            else{
                e.target.style.cursor = '';
            }
        }
    },

    _onColumnMouseDown : function(e, args) {
        //  if the user clicks the mouse button while
        //  the cursor is in the resize position, it means
        //  they want to start resizing.  Set _isResizing to true
        //  and grab the th element that is being resized
        if(e.target.style.cursor == 'e-resize') {
            args.behavior._isResizing = true;
            args.behavior._element = e.target;
        }
    },

    _onMouseUp : function(e, args) {
        //  the user let go of the mouse - so
        //  they are done resizing the header.  Reset
        //  everything back
        if(args.behavior._isResizing){
            //  set back to default values
            args.behavior._isResizing = false;
            args.behavior._element = null;

            //  make sure the cursor is set back to default
            for(i = 0; i < args.behavior._columns.length; i++){
                args.behavior._columns[i].style.cursor = '';
            }
        }
    },

    _onSelectStart : function(e, args) {
        // Don't allow selection during drag
        if(args.behavior._isResizing){
            e.preventDefault();
            return false;
        }
    },

    get_HeaderCellHoverCssClass : function() {
        return this._headerCellHoverCssClass;
    },

    set_HeaderCellHoverCssClass : function(value) {
        this._headerCellHoverCssClass = value;
    },

    get_CellHoverCssClass : function() {
        return this._cellHoverCssClass;
    },

    set_CellHoverCssClass : function(value) {
        this._cellHoverCssClass = value;
    },

    get_ColumnHoverCssClass : function() {
        return this._columnHoverCssClass;
    },

    set_ColumnHoverCssClass : function(value) {
        this._columnHoverCssClass = value;
    },

    get_RowHoverCssClass : function() {
        return this._rowHoverCssClass;
    },

    set_RowHoverCssClass : function(value) {
        this._rowHoverCssClass = value;
    },

    get_HeaderCellSelectCssClass : function() {
        return this._headerCellSelectCssClass;
    },

    set_HeaderCellSelectCssClass : function(value) {
        this._headerCellSelectCssClass = value;
    },

    get_RowSelectCssClass : function() {
        return this._rowSelectCssClass;
    },

    set_RowSelectCssClass : function(value) {
        this._rowSelectCssClass = value;
    },

    get_ColumnSelectCssClass : function() {
        return this._columnSelectCssClass;
    },

    set_ColumnSelectCssClass : function(value) {
        this._columnSelectCssClass = value;
    },

    get_CellSelectCssClass : function() {
        return this._cellSelectCssClass;
    },

    set_CellSelectCssClass : function(value) {
        this._cellSelectCssClass = value;
    },

    get_HeaderRowCssClass : function() {
        return this._headerRowCssClass;
    },

    set_HeaderRowCssClass : function(value) {
        this._headerRowCssClass = value;
    },

    get_DataRowCssClass : function() {
        return this._dataRowCssClass;
    },

    set_DataRowCssClass : function(value) {
        this._dataRowCssClass = value;
    },

    get_AlternateDataRowCssClass : function() {
        return this._alternateDataRowCssClass;
    },

    set_AlternateDataRowCssClass : function(value) {
        this._alternateDataRowCssClass = value;
    },

    get_KeysNavigation : function() {
        return this._KeysNavigation;
    },

    set_KeysNavigation : function(value) {
        this._KeysNavigation = value;
    },

    get_SelectedRowIndexCookie : function() {
        return this._SelectedRowIndexCookie;
    },
    set_SelectedRowIndexCookie : function(value) {
        if (this._SelectedRowIndexCookie != value) {
            this._SelectedRowIndexCookie = value;
            this.raisePropertyChanged('SelectedRowIndexCookie');
        }
    },

    get_SelectedColIndexCookie : function() {
        return this._SelectedColIndexCookie;
    },
    set_SelectedColIndexCookie : function(value) {
        if (this._SelectedColIndexCookie != value) {
            this._SelectedColIndexCookie = value;
            this.raisePropertyChanged('SelectedColIndexCookie');
        }
    },

    get_SelectedRowIndex: function() {
        return this._currow;
    },
    set_SelectedRowIndex: function(value) {
        this._currow = value;
        this.raisePropertyChanged('SelectedRowIndex');
    },

    get_SelectedColIndex: function() {
        return this._curcell;
    },
    set_SelectedColIndex: function(value) {
        this._curcell = value;
        this.raisePropertyChanged('SelectedColIndex');
    },

    toNumber : function(m) {
        //  helper function to peel the px off of the widths
        return new Number(m.replace('px', ''));
    },

    createCookie: function(name,value,days) {
	    if (days)
	    {
		    var date = new Date();
		    date.setTime(date.getTime()+(days*24*60*60*1000));
		    var expires = "; expires="+date.toGMTString();
		    //setCookie (name, value, date);
	    }
	    else var expires = "";
	    document.cookie = name+"="+value+expires+"; path=/";
    },

    showMenuCell: function(menuControl, ButtonsIds, ButtonsDisplay, ButtonsEnabled) {
        if ( menuControl == null ) { return false; }

        menu = document.getElementById(menuControl);

        if ( menu == null ) { return false; }

        menu.style.display = "block";
        menu.style.left = event.x + 5;
        menu.style.top = event.y - 1;

	    var arrButtonsIds = ButtonsIds.split(";");
	    var arrButtonsDisplay = ButtonsDisplay.split(";");
	    var arrButtonsEnabled = ButtonsEnabled.split(";");

        var Button;
        for (i=0; i<arrButtonsIds.length; i++) {
            Button = document.getElementById(arrButtonsIds[i]);
            if (Button != null) {
                Button.style.display = arrButtonsDisplay[i];
                if (arrButtonsEnabled[i] == 'true') {
                    Button.enabled = true;
                }
                else {
                    Button.enabled = false;
                }
            }
        }

        event.cancelBubble = true;

        return false;
    },

    closeMenu: function(menuControl)
    {
        if ( menuControl == null ) { return; }

        menu = document.getElementById(menuControl);

        if ( menu == null ) { return; }

        menu.style.display = "none";
    },

    SelectCell: function(row, cell) {
        if (row < this._rows.length && row >= 0) {
            if (cell < this._rows[row].cells.length && cell >= 0) {
                var args = {rowIndex: row, cellIndex: cell, behavior: this};
                this._onDataCellClick(this, args);
                this.set_SelectedRowIndex(row);
                this.set_SelectedColIndex(cell);
                //this._currow = row;
                //this._curcell = cell;
            }
        }
    }
}

Robotics.WebControls2.GridViewControl.GridViewControlBehavior.registerClass('Robotics.WebControls2.GridViewControl.GridViewControlBehavior', AjaxControlToolkit.BehaviorBase);