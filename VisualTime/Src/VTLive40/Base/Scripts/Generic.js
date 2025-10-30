// JScript File

function getObj(name)
{
    /* DHTML Micro API
    * Source: http://www.quirksmode.org/js/dhtmloptions.html
    */
    if (document.getElementById) // test if browser supports document.getElementById
    {
        this.obj = document.getElementById(name);
        this.style = document.getElementById(name).style;
    }
        else if (document.all) // test if browser supports documen.all
    {
        this.obj = document.all[name];
        this.style = document.all[name].style;
    }
        else if (document.layers) // test if browser supports document.layers
    {
        this.obj = document.layers[name];
        this.style = document.layers[name].style;
    }
}

function showPopup(PopupBehavior) {
    //createCookie(PopupBehavior,true,30);                
    var modalPopupBehavior = $find(PopupBehavior);        
    modalPopupBehavior.show();        
}

function hidePopup(PopupBehavior) {
    //eraseCookie(PopupBehavior);
    var modalPopupBehavior = $find(PopupBehavior);
    modalPopupBehavior.hide();                
}        


function IsNumeric(sText) {
   var ValidChars = "0123456789.";
   var IsNumber=true;
   var Char;
 
   for (i = 0; i < sText.length && IsNumber == true; i++) 
      { 
      Char = sText.charAt(i); 
      if (ValidChars.indexOf(Char) == -1) 
         {
         IsNumber = false;
         }
      }
   return IsNumber;
   
}

function DisableElement(Element) {

    if (Element != null) {    
        if (Element.disabled == false) {
        
            var _backgroundDiv = document.createElement('div');
            _backgroundDiv.style.display = 'none';
            _backgroundDiv.style.zIndex = 10000;
            _backgroundDiv.className = 'Background';
            _backgroundDiv.style.display = '';
                    
            Element.parentNode.appendChild(_backgroundDiv);
                    
            var _Bounds = Sys.UI.DomElement.getBounds(Element);        
            _backgroundDiv.style.width = _Bounds.width + 'px';
            _backgroundDiv.style.height = _Bounds.height + 'px';                            
            Sys.UI.DomElement.setLocation(_backgroundDiv, _Bounds.x, _Bounds.y);        
        
            Element.disabled = true;
        
            return _backgroundDiv;
        }
    }
    
}

function EnableElement(Element, _backgroundDiv) {

    if (Element != null) {    
        if (Element.disabled == true) {
        
            if (_backgroundDiv != null) Element.parentNode.removeChild(_backgroundDiv);
        /*
            var _backgroundDiv = document.createElement('div');
            _backgroundDiv.style.display = 'none';
            _backgroundDiv.style.zIndex = 10000;
            _backgroundDiv.className = 'Background';
            _backgroundDiv.style.display = '';
                    
            Element.parentNode.appendChild(_backgroundDiv);
                    
            var _Bounds = Sys.UI.DomElement.getBounds(Element);        
            _backgroundDiv.style.width = _Bounds.width + 'px';
            _backgroundDiv.style.height = _Bounds.height + 'px';                            
            Sys.UI.DomElement.setLocation(_backgroundDiv, _Bounds.x, _Bounds.y);        
        */
            Element.disabled = false;
        
        }
    }
    
}

function ButtonClick(button) {
    // Ejecuta un click del botón des del lado cliente
    if(BrowserDetect.browser == "Explorer") {
            button.click();
    } else {
            var e = document.createEvent("MouseEvents");
            e.initEvent("click", true, true);
            button.dispatchEvent(e);
    }

}

/* addCssClass: Afegeix una nova Clase a un objecte */
/*************************************************************************************************************/
function addCssClass(obj, clsTxt) {
    obj.className = obj.className + ' ' + clsTxt;
}

/* removeCssClass: Elimina una Clase a un objecte */
/*************************************************************************************************************/
function removeCssClass(obj, clsTxt) {
    var parmCss = new Array();
    parmCss = obj.className.split(" ");

    obj.className = ''; //Reset dels CSS
    //Carreguem tots els anteriors atributs    
    for (nCss = 0; nCss < parmCss.length; nCss++) {
        if (parmCss[nCss] != clsTxt) {
            obj.className = obj.className + ' ' + parmCss[nCss];
        }
    }
}

 function isValidEmail(valor) {
    if (/^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$/.test(valor)){
        return true;
    } else {
        return false;
    }
}

function PrintObject(_HTML, _CSSPath1, _CSSPath2) {
    var ventana = window.open("", "", "");
    var contenido = "<html>";
    contenido = contenido + "<head runat='server'>";
    if (_CSSPath1 != null && _CSSPath1 != '') {
        contenido = contenido + "<link href='" + _CSSPath1 + "' type='text/css' rel='stylesheet'/>";
    }
    if (_CSSPath2 != null && _CSSPath2 != '') {
        contenido = contenido + "<link href='" + _CSSPath2 + "' type='text/css' rel='stylesheet'/>";
    }
    contenido = contenido + "</head>";
    contenido = contenido + "<body onload='window.print();window.close();'>";
    //contenido = contenido + "<body >";
    contenido = contenido + _HTML + "</body></html>";
    ventana.document.open();
    ventana.document.write(contenido);
    ventana.document.close();
}

function getUrlParameter(name) {
    name = name.replace(/[\[]/, "\\\[").replace(/[\]]/, "\\\]");
    var regexS = "[\\?&]" + name + "=([^&#]*)";
    var regex = new RegExp(regexS);
    var results = regex.exec(window.location.href);
    if (results == null)
        return "";
    else
        return results[1];
}

function deleteElementFromArray(arr, element) {
    if (typeof arr != 'undefined' && arr.length > 0) {
        for (var i = 0; i < arr.length; i++) {
            if (arr[i] === element) {
                arr.splice(i, 1);
                i--;
            }
        }
    }
}

function array2String(array, separator) {
    var str = "";
    for (var i = 0; i < array.length; i++) {
        if (str == "")
            str = array[i];
        else
            str += separator + array[i];
    }
    return str;
}
