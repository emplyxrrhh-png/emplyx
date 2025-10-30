var menu;
var _backgroundDiv;

function Init()
{
    menu = null;
}

function onMouseOver(row, rowColor)
{
    if ( row == null ) { return; }

    if ( menu != null ) {
        if ( menu.style.display != "none" ) {
            return;
        }
    }

    row.style.background = rowColor;
}

function onMouseOut(row, rowColor)
{
    if ( row == null ) { return; }
    if ( menu != null ) {
        if ( menu.style.display != "none" ) {
            return;
        }
    }

    row.style.background = rowColor;
}

function showMenuRow(menuControl, rowId, rowIdBox, tableRow, bgColor)
{
    if ( menuControl == null ) { return false; }

    menu = document.getElementById(menuControl);

    if ( menu == null ) { return false; }

    menu.style.display = "block";
    menu.style.left = event.x + 5;
    menu.style.top = event.y - 1;

    tableRow.style.background = bgColor;
    
    saveId(rowId, rowIdBox);
    
    event.cancelBubble = true;

    return false;
}

var arrButtonsOnClick;

function ContextMeunInit() {

    menu = null;
    arrButtonsOnClick = null;
            
}

function showMenuCell(menuControl, ButtonsIds, ButtonsDisplay, ButtonsEnabled, /*SelectedRowIndexCookie, SelectedColIndexCookie,*/ gridviewId, TopOffset)
{
    if ( menuControl == null ) { return false; }

    menu = document.getElementById(menuControl);

    if ( menu == null ) { return false; }

    // Inicializar array con la definición del evento 'onclick' para los botones del menú
    if (arrButtonsOnClick == null) {
        arrButtonsOnClick = new Array(100);
        for (i=0; i<arrButtonsOnClick.length; i++) {
            arrButtonsOnClick[i] = new Array(2);
            arrButtonsOnClick[i][0] = '';
            arrButtonsOnClick[i][1] = '';
        }
    }
    
    var arrButtonsIds = ButtonsIds.split(";");
        
    // Actualizar el array de eventos onclick si no esxite    
    for (i=0; i<arrButtonsIds.length; i++) {
        // Buscar id a arrButtonsOnClick        
        for (j=0; j<arrButtonsOnClick.length; j++) {
            if (arrButtonsOnClick[j][0] == arrButtonsIds[i]) {
                break;
            }
            else {
                if (arrButtonsOnClick[j][0] == '') {
                    arrButtonsOnClick[j][0] = arrButtonsIds[i];                    
                    Button = document.getElementById(arrButtonsIds[i]);
                    if (Button != null) {                
                        arrButtonsOnClick[i][1] = Button.onclick;                        
                    }                                        
                    break;                    
                }
            }
        }        
    }
        
    
    // Establecer propiedades opciones menú	
	var arrButtonsDisplay = ButtonsDisplay.split(";");
	var arrButtonsEnabled = ButtonsEnabled.split(";");
    var Button;
    for (i=0; i<arrButtonsIds.length; i++) {
        Button = document.getElementById(arrButtonsIds[i]);
        if (Button != null) {
            Button.style.display = arrButtonsDisplay[i];
            if (arrButtonsEnabled[i] == 'true') {                
                Button.style.color = '';
                // Asignar el evento 'onclick' al botón en el array 'arrButtonsOnClick'
                /*for (j=0; j<arrButtonsOnClick.length; j++) {
                    if (arrButtonsOnClick[j][0] == arrButtonsIds[i]) {
                        Button.onclick = arrButtonsOnClick[j][1];                        
                    } 
                    else {
                        if (arrButtonsOnClick[j][0] == '') break;
                    }
                } */               
                Button.enabled = true;
            }
            else {
                Button.style.color = 'gray';                                
                /*Button.onclick = function() {return false; };*/
                Button.enabled = false;
            }
        }            
    }

  
    //createCookie(SelectedRowIndexCookie, '[' + rowId + ']', 30);
    //createCookie(SelectedColIndexCookie, '[' + colId + ']', 30);  
    //saveId(rowId, rowIdBox);
    //saveId(colId, colIdBox);

    
    // Sobreponer div a la gridview para desabilitarla
    
    // Obtener control gridview
    var bolIsroGridViewControl = true;
    var _gridView = $get('__gv' + gridviewId + '__div');
    if (_gridView == null) {
        _gridView = $get(gridviewId);
        bolIsroGridViewControl = false;
        if (_gridView == null) {
            _gridView = document.getElementById(gridviewId);
            bolIsroGridViewControl = false;        
        }
    }
    
    // Definir div
    if (_backgroundDiv == null)
        _backgroundDiv = document.createElement('div');
    _backgroundDiv.style.display = 'none';
    _backgroundDiv.style.zIndex = 999;
    _backgroundDiv.className = 'Background';
        
    _gridView.parentNode.appendChild(_backgroundDiv);
    
    // Hacer el div visible
    _backgroundDiv.style.display = '';
    //_backgroundDiv.style.position = 'absolute';
        
    // Obtener dimensiones de la gridview
    var gridViewBounds = Sys.UI.DomElement.getBounds(_gridView);                   
    //_backgroundDiv.style.left = gridViewBounds.x + 'px';
    //_backgroundDiv.style.top = gridViewBounds.y + 'px';
    
    //  Establecer dimensiones
    _backgroundDiv.style.width = gridViewBounds.width + 'px';
    _backgroundDiv.style.height = gridViewBounds.height + 'px';              
      
    //  Poner el div delante de la gridview
    Sys.UI.DomElement.setLocation(_backgroundDiv, gridViewBounds.x , gridViewBounds.y ); //-250

        
    // Establecer la posición del menú
    var intLeft = document.documentElement.scrollLeft + 5;
    var intTop = document.documentElement.scrollTop - 1;    
    try {
        intLeft = document.documentElement.scrollLeft + event.x + 5;
        intTop = document.documentElement.scrollTop + (event.y - 1); //+250;
    } catch (ex){
        
    }
          
    if (bolIsroGridViewControl == true) {
        intLeft = intLeft + gridViewBounds.x;
        intTop = intTop + gridViewBounds.y;
    }
    //if ((intTop + menu.clientHeight) > document.documentElement.clientHeight) 
    //    intTop = document.documentElement.clientHeight - menu.clientHeight;
        
    menu.style.position = 'absolute';
    menu.style.display = "block";
    menu.style.left = intLeft;
    menu.style.top = intTop;
    menu.style.zIndex = 1000;    

    try {    
        event.cancelBubble = true;
    }
    catch (ex) { }

    return false;
}

function closeMenu(menuControl, progressdivcontrol)
{
    if ( menuControl == null ) { return; }

    menu = document.getElementById(menuControl);

    if ( menu == null ) { return; }

    menu.style.display = "none";

    if (_backgroundDiv != null) {
        _backgroundDiv.style.display = 'none';
    }

    //saveId(-1, rowIdBox);
    //saveId(-1, colIdBox);
}

function saveId(Id, IdBox)
{
    if ( IdBox == null ) { return false; }
    
    var IdHiddenBox = document.getElementById(IdBox);
    if ( IdHiddenBox == null ) { return false; }
    IdHiddenBox.value = Id;

}
