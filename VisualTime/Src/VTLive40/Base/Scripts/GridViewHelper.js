function GridViewHelperClass()
{
	function GridViewInit(idGrid, ExtraWidth, ExtraHeight)
    {
		var oTBODY = FindTBODY(idGrid);
		InitDrillDown(idGrid);
    }
    this.Init = GridViewInit;
    
    function ResizeMe(idGrid, ExtraWidth, ExtraHeight)
    {
		if(window.frameElement != null)
		{
    		if(window.frameElement.tagName == "IFRAME")
			{
				// we are in an iframe
				
				// set the width to be really wide so there is no column wrapping
				window.frameElement.width = 10000;
			
				// now calculate the width required and set the frame width to it (plus a fudge factor)
				// window.alert("before width: "+GridViewHelper.CalcWidth(document.all.GridView1)+" "+ExtraWidth);
				////document.all.GridView1
				window.frameElement.width = GridViewHelper.CalcWidth(idGrid)+ExtraWidth;
				
				// set the frame height to height of the generated document.
				// window.alert("height: "+document.body.scrollHeight);
				window.frameElement.height = document.body.scrollHeight+ExtraHeight;
				return;
			}
		}
		// get the container around the grid
		var Parent = idGrid.offsetParent; //GridView1
		// make the parent really wide so that no columns will wrap
		Parent.style.width = "10000px";
		// calcuate the real width
		var RealWidth = GridViewHelper.CalcWidth(idGrid)+100; //document.all.GridView1
		// set the parent width back to nothing	
		Parent.style.width = "";
		//set the grid to the size it needs to be
		idGrid.width=""+RealWidth;	//GridView1
	}
    
   // change the onclick function for the select buttons
	function InitDrillDown(idGrid)
	{
		var oTBODY= FindTBODY(idGrid);
		for(var i=0; ; i++)
		{
			try
			{
				var ctl=oTBODY.childNodes[i].childNodes[0];
				var selectctl = ctl.childNodes[ctl.childNodes.length-1]
				if(selectctl.tagName == "INPUT")
				{
					var onc = selectctl.onclick.toString();
					//window.alert(onc);	// uncomment this to see what the onclick actually contains
					if(onc.indexOf("Select$") == -1)
						continue;	// probably an Add row line
					onc = "return(GridViewHelper.DrillDownOrUp(this));"
					selectctl.onclick = onc;
					selectctl.outerHTML = selectctl.outerHTML;		// if you don't do this then the onclick will not work. it is probably related to how the onclick is actually defined (see window.alert above)
				}
			}
			catch(e)
			{
				break;
			}
		}
	}

	function GetParentObject(o, tagName)
	{
		srcElem = o;
		//crawl up to find the table
		while (srcElem.tagName != tagName)
			srcElem = srcElem.parentElement;
		return(srcElem);
	}
	function RowObjectToIndex(oTR)
	{
		if(oTR == null)
			return(-1);

		var oTABLE = GetParentObject(oTR, "TABLE");
		// find the row index of our row
		var i;
		for(i=0; i<oTABLE.rows.length; i++)
		{
			if(oTABLE.rows[i] == oTR)
			{
				return(i);
			}
		}
	}
	function DrillDownOrUpX(This)
	{
		var oRow = GetParentObject(This, "TR");
			// window.alert("oRow: "+oRow.outerHTML);
		var RowIndex = RowObjectToIndex(oRow)
		var oTable = GetParentObject(This, "TABLE");
			// window.alert("in drill: open='"+oRow.open+"' hascontent='"+oRow.hascontent+"'");
		var oPlusMinus = oRow.firstChild.childNodes[0]; //4		
		if(oRow.open == "1")
		{
			var DetailsRow = oTable.rows[RowIndex+1];
			DetailsRow.style.display="none";
			oRow.open = "0";
			var Gif = oPlusMinus.src;
			var iii = Gif.lastIndexOf("/");
			Gif = Gif.slice(0, iii)+"/Plus.gif";
			oPlusMinus.src = Gif;
			return(false);
		}
		if(oRow.hascontent == "1")
		{
			var DetailsRow = oTable.rows[RowIndex+1];
			DetailsRow.style.display="block";
			oRow.open = "1";
			var Gif = oPlusMinus.src;
			var iii = Gif.lastIndexOf("/");
			Gif = Gif.slice(0, iii)+"/Minus.gif";
			oPlusMinus.src = Gif;
			return(false);
		}
				
		var ColumnCount = oRow.cells.length;
		// need to add the row
		var NewRow = oTable.insertRow(RowIndex+1);
		var NewCell = NewRow.insertCell(0);
		NewCell.setAttribute("colSpan", ColumnCount.toString());
			
		var CellContent =
			"<table cellpadding='0' cellspacing='0'>"+
			"<tr><td><iframe src='"+oRow.href+"' frameborder='0' width='100%' height='200'></iframe></td></tr>"+
			"</table>";
		// window.alert(CellContent);
		// window.prompt("", oRow.href);
		NewCell.innerHTML = CellContent;
		// window.alert("NewRow: "+NewRow.outerHTML);
		oRow.open = "1";
		oRow.hascontent = "1";
		var Gif = oPlusMinus.src;
		var iii = Gif.lastIndexOf("/");
		Gif = Gif.slice(0, iii)+"/Minus.gif";
		oPlusMinus.src = Gif;
		// window.alert("oRow: "+oRow.outerHTML);
		return(false);
	}
	this.DrillDownOrUp = DrillDownOrUpX;
	function FindTBODY(idGrid)
	{
		if(idGrid.firstChild.tagName == "TBODY")
			return(idGrid.firstChild);
		// there is a caption, so go down one more level
		return(idGrid.firstChild.nextSibling);		
	}
	
	function CalcWidth(idGrid)
	{
		var oTBODY=FindTBODY(idGrid);
		var oTR = oTBODY.firstChild;	// get the first row object
		var oLastCell = oTR.cells[oTR.cells.length-1];

	 	var kb=0;
	 	var r = oLastCell;
		while(r)
		{
	    	kb+=r["offsetLeft"];
	    	r=r.offsetParent
	  	}
	  	kb += oLastCell.offsetWidth;
	  	return kb;
	}
	this.CalcWidth = CalcWidth;


	function AddToolTips(idGrid, ToolTips)
	{
		var oTBODY=FindTBODY(idGrid);
		var oTR = oTBODY.firstChild;// get the first row object which contains the column titles
		if(ToolTips.length > oTR.children.length)
			ToolTips.length = oTR.children.length;
		for(var i=0; i<ToolTips.length; i++)
		{
			var oChild = oTR.children[i];
			// window.alert("OOO: "+oChild.outerHTML);
			oChild.title = ToolTips[i];
		}
	}
	this.AddToolTips = AddToolTips;
}


var GridViewHelper = new GridViewHelperClass();


// ***************************************************************************************************
/*function saveSelected(RowId, RowIdBox, ColId, ColIdBox)
{
    if ( RowIdBox != null ) {     
        var RowIdHiddenBox = document.getElementById(RowIdBox);
        if ( RowIdHiddenBox != null ) { 
            RowIdHiddenBox.value = RowId;
        }
    }
    if ( ColIdBox != null ) {     
        var ColIdHiddenBox = document.getElementById(ColIdBox);
        if (  ColIdHiddenBox != null ) { 
            ColIdHiddenBox.value = ColId;
        }
    }        
}*/
