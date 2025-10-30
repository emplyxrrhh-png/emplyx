var arrCombos = new Array();

//Show/Hide ComboBox DropDown
function roCB_DropDownClick(objDivID,eventObj){
    roCB_hideAllCombos();
    var oDiv = document.getElementById(objDivID);
    if (oDiv == undefined){ return; }
    if (oDiv.style.display==''){
        oDiv.style.display='none';
        oDiv.style.position='relative';
    } else {
        var objCmb = document.getElementById(retComboPrefix(objDivID) + "ComboBoxLabel");
        var objTbl = document.getElementById(retComboPrefix(objDivID) + "tblContainer");
        if (objCmb != null) {
            if (objCmb.getAttribute("disabled") != null) {
                if (objCmb.getAttribute("disabled") == "true") { return; }
            }
        }

        if (window.addEventListener) { // Mozilla, Netscape, Firefox
            oDiv.style.position = 'absolute';
            oDiv.style.display = '';
        }
        else { //Explorer
            oDiv.style.position = 'absolute';
            oDiv.style.display = '';
            var objCtl = document.getElementById(retComboPrefixEx(objDivID));
            if (objCtl != null) {
                if (objCtl.getAttribute("position") != null) {
                    if (objCtl.getAttribute("position") == "relative") { 
                        oDiv.style.position = 'relative';
                        oDiv.style.display = '';
                    }
                }
            }
        }

        if (oDiv.offsetWidth < (objCmb.offsetWidth + 22)) {
            oDiv.style.width = (objCmb.offsetWidth + 22) + 'px';
        }
        objTbl.style.width = "100%"        
    }
    eventObj.cancelBubble = true;
    return true;
}

//Hide ComboBox DropDown
function roCB_HideDropDownClick(objDivID,eventObj){
    var oDiv = document.getElementById(objDivID);
    if (oDiv == undefined){ return; }
    if (oDiv.style.display==''){
        oDiv.style.display='none';
        oDiv.style.position='relative';        
    }
    eventObj.cancelBubble = true;
    return true;
}

//roCB_Clicked
function roCB_Clicked(objLabelID, objComboID, ss_Text, ss_Value) {
    var oCombo = document.getElementById(objComboID);
    var oLabel = document.getElementById(objLabelID);
    if (oLabel == undefined) { return; }
    
    if (oLabel.getAttribute("disabled") == "true") { return; }
    if (oCombo.getAttribute("disabled") == "true") { return; }

    //check onchange event
    if (oCombo.getAttribute("value") != oLabel.getAttribute("value")) {
        if (oCombo.getAttribute("cbonchange") != null) {
            eval(oCombo.getAttribute("cbonchange"));        
        }
    }

    oCombo.setAttribute("value", oLabel.getAttribute("value"));
    
    oCombo.innerHTML = oLabel.innerHTML;
    oCombo.title = oLabel.innerHTML;
    roCB_saveState(objComboID,ss_Text,ss_Value);
}

//roCB_saveState
function roCB_saveState(objComboID,ss_Text,ss_Value){
    var oCombo = document.getElementById(objComboID);
    var sText = document.getElementById(ss_Text);
    var sValue = document.getElementById(ss_Value);
    if (oCombo == null){ return; }
    if (sText != null){
        sText.value = oCombo.innerHTML;
    }
    if(sValue != null){
        sValue.value = oCombo.getAttribute("value");
    }
}

//Bucle per pagines per trobar roComboBoxes
function autoAddComboHandlers() {
    try {
        var oCombos = retElementsByName("roComboBox");
        for (var n = 0; n < oCombos.length; n++) {
            roCB_addComboHandler(oCombos[n].getAttribute("vid") + "_DropDown");
        }
        
    } catch (e) { showError("roComboBox.autoAddComboHandlers", e); }
}

//Agrega control dels events del combo
function roCB_addComboHandler(objComboID) {
    try {
        var ifExists = false;
        for (var n = 0; n < arrCombos.length; n++) {
            if (arrCombos[n] == objComboID) {
                ifExists = true;
            }
        }

        if (!ifExists) {
            var numCmb = arrCombos.length;
            arrCombos[numCmb] = objComboID;
        }

    } catch (e) { showError("roCB_AddComboHandler", e); }    
}

//Amaga tots els combobox que es troben desplegats (document.onclick)
function roCB_hideAllCombos(){
    for(n=0;n<arrCombos.length;n++){
        oDiv = document.getElementById(arrCombos[n]);
        if(oDiv != undefined){
            oDiv.style.display='none';
            oDiv.style.position='relative'; 
        }
    }
}

//roCB_setText (assigna el Texte directament i modifica els camps de valor i el atribut "value") <- actiu al onchange
function roCB_setText(newText,objComboID,ss_Text,ss_Value){
    var oCombo = document.getElementById(objComboID);
    var sText = document.getElementById(ss_Text);
    var sValue = document.getElementById(ss_Value);
    if (oCombo == null){ return; }
    
    oCombo.innerHTML = newText;
    var valor = findValueByText(retComboPrefix(objComboID) + "DivContainer", newText);
    if(valor == null){ valor = ''; }
    oCombo.setAttribute("value",valor);        
    
    //Si esta declarat el asp:label per guardar el texte (tema perdida viewstate)
    if (sText != null){
        sText.value = newText;
    }
    
    //Si esta declarat el asp:label per guardar el valor (tema perdida viewstate)
    if(sValue != null){
        sValue.value = valor;
    }
}

//Retalla el combobox_label per poder buscar altres contenidors
function retComboPrefix(objComboID){
    var arrSplit = new Array();
    var arrNom = '';
    arrSplit = objComboID.split("_");
    for(var n=0;n<(arrSplit.length-1);n++)
    {
        arrNom += arrSplit[n] + '_';
    }
    return arrNom;
}

//Retalla el combobox_label per poder buscar altres contenidors
function retComboPrefixEx(objComboID) {
    var arrSplit = new Array();
    var arrNom = '';
    arrSplit = objComboID.split("_");
    for (var n = 0; n < (arrSplit.length - 1); n++) {
        arrNom += arrSplit[n];
    }
    return arrNom;
}

//roCB_setValue (assigna el nou valor i busca el texte corresponent per assignar-lo)
function roCB_setValue(newValue, objComboID, ss_Text, ss_Value) {
   var oCombo = document.getElementById(objComboID);
    var sText = document.getElementById(ss_Text);
    var sValue = document.getElementById(ss_Value);
    if (oCombo == null){ return; }
    
    var valor = findTextByValue(retComboPrefix(objComboID) + "DropDown", newValue);
    if(valor == null){ valor = newValue; }
    oCombo.setAttribute("value", newValue);
    oCombo.setAttribute("title", valor);
    oCombo.innerHTML = valor;

    if (sText != null) {
        sText.value = valor;
        oCombo.innerHTML = valor;
    }
        
    if(sValue != null){
        sValue.value = newValue;
    }
}

//roCB_clearItems (Borra tots els items de dins el roComboBox)
function roCB_clearItems(objComboId, ss_Text, ss_Value) {
    try {
        var oCombo = document.getElementById(objComboId);
        var sText = document.getElementById(ss_Text);
        var sValue = document.getElementById(ss_Value);
        if (oCombo == null) { return; }

        oCombo.setAttribute("value", "");
        var oDivCombo = document.getElementById(objComboId + "_DropDown");
        var oCmbLabel = document.getElementById(objComboId + "_ComboBoxLabel");
        if (oDivCombo != null) {
            oDivCombo.setAttribute("vheight", oDivCombo.style.height);
            oDivCombo.setAttribute("vwidth", oDivCombo.style.width);
            oDivCombo.innerHTML = "";
            oDivCombo.style.height = "0px";
        }
        if (oCmbLabel != null)
            oCmbLabel.innerHTML = "";

        if (sText != null) {
            sText.value = "";
        }

        if (sValue != null) {
            sValue.value = "";
        }
    }
    catch (e) {
        alert('roCB_clearItems: ' + e); 
    }
}

function roCB_disable(objComboId, bolDisable) {
    var oCombo = document.getElementById(objComboId + "_ComboBoxLabel");
    var oBtnCombo = document.getElementById(objComboId + "_ButtonDown");
    if (oCombo == null) { return; }

    if (bolDisable) {
        roCB_disableObj(oCombo);
        roCB_disableObj(oBtnCombo);
    } else {
        roCB_enableObj(oCombo);
        roCB_enableObj(oBtnCombo);
    }
}

//deshabilita el combo (sols botons desplegables)
function roCB_disableObj(obj) {
    if (obj.getAttribute("onclick") == null) {
        obj.setAttribute("ononclick", "");
    } 
    else {
        if (obj.getAttribute("onclick") != "javascript: void(0);") {
            obj.setAttribute("ononclick", obj.getAttribute("onclick"));
        }
    }

    if (obj.getAttribute("href") == null) {
        obj.setAttribute("onhref", "");
    } 
    else {        
        obj.setAttribute("onhref", obj.getAttribute("href"));        
    }

    if (window.addEventListener) { // Mozilla, etc.
        obj.setAttribute("onclick", "javascript: void(0);");
    } 
    else { //IE
        //ppr esto no funciona!!--> obj.onclick = null;
        obj.setAttribute("onclick", "javascript: void(0);");
        $(obj).unbind('onclick');
    }
    obj.disable = true;    
}

//habilita el combo
function roCB_enableObj(obj) {
    if (window.addEventListener) { // Mozilla, etc.
        if (obj.getAttribute("ononclick") == null) {
            obj.setAttribute("ononclick", obj.getAttribute("onclick"));
        }
        if (obj.getAttribute("onhref") == null) {
            obj.setAttribute("onhref", obj.getAttribute("href"));
        }
        obj.setAttribute("onclick", obj.getAttribute("ononclick"));
    } 
    else { //IE
        if (obj.getAttribute("ononclick") == null) {
            obj.setAttribute("ononclick", obj.getAttribute("onclick"));
        }
        if (obj.getAttribute("onhref") == null) {
            obj.setAttribute("onhref", obj.getAttribute("href"));
        }
        try {
            obj.onclick = eval(obj.getAttribute("ononclick"));
        }
        catch (e) { }
    }
    obj.disable = false;
}

function roCB_disableItem(objComboId, oIndex, bolDisable) {
    var oCmbContainer = document.getElementById(objComboId + "_tblContainer");
    var arrAnchors = new Array();
    arrAnchors = oCmbContainer.getElementsByTagName("A")
    var oAnchor = arrAnchors[oIndex];
    if (bolDisable == true) {
        if (oAnchor.color == null) {
            oAnchor.setAttribute("ocolor", "#2D4155");    
        } else {
            oAnchor.setAttribute("ocolor", oAnchor.color);
        }
        
        oAnchor.style.color = "silver";
        roCB_disableObj(oAnchor);
    } else {
        if (oAnchor.getAttribute("ocolor") == null) {
            oAnchor.style.color = "#2D4155";
        } else {
            oAnchor.style.color = oAnchor.getAttribute("ocolor");
        }
        roCB_enableObj(oAnchor);
    }
}

//roCB_addItem (Afegeix un item dintre del combo (sols part de client))
function roCB_addItem(objComboId, oText, oValue, func) {
    var oCombo = document.getElementById(objComboId + "_ComboBoxLabel");
    var oDivCombo = document.getElementById(objComboId + "_DropDown");
    if (oDivCombo == null) {return; }
    
    //Comprovem el alt del div (per si s'ha buidat i el carreguem
    if (oDivCombo.getAttribute("vheight") != null) {
        if (oDivCombo.getAttribute("vheight") == "0pt" || oDivCombo.getAttribute("vheight") == "0px" || oDivCombo.getAttribute("vheight") == "0") {
            oDivCombo.setAttribute("vheight","auto");
        }
        
        oDivCombo.style.height = oDivCombo.getAttribute("vheight");
    }

    if (oDivCombo.getAttribute("vwidth") != null) {
        if (oDivCombo.getAttribute("vwidth") == "0pt" || oDivCombo.getAttribute("vwidth") == "0px" || oDivCombo.getAttribute("vwidth") == "0") {
            oDivCombo.setAttribute("vwidth", "100%");
        }

        oDivCombo.style.width = oDivCombo.getAttribute("vwidth");
    } else {
        oDivCombo.setAttribute("vwidth", oCombo.clientWidth + 28);
        oDivCombo.style.width = oDivCombo.getAttribute("vwidth");
    }
    

    //Busquem la taula per inserir el item
    var oTable = document.getElementById(objComboId + "_tblContainer");
    // Si la taula no existeix, la creem 
    if (oTable == null) {
        oTable = document.createElement("TABLE");
        oTable.id = objComboId + "_tblContainer";
        oDivCombo.appendChild(oTable);
        oTable.setAttribute("style", "width: 100%;");
        oTable.style.width = "100%";
    }
    var oNewId = oTable.rows.length;
    
    /*Afegim el tr */
    var oTR = oTable.insertRow(-1);
    var oTD = oTR.insertCell(-1);
    oTD.setAttribute("style", "white-space: nowrap; width: 100%;");
    oTD.style.width = oDivCombo.getAttribute("vwidth");
    var oAnchor = document.createElement("a");
    oAnchor.setAttribute("href", "javascript: void(0);");
    oAnchor.id = objComboId + "_LabelChild_" + oNewId;
    oAnchor.className = "roComboBox_LabelChild";
    var oW = (oCombo.offsetWidth + 10);
    if (oW <= 10) { oW = "95%"; } else { oW = (oCombo.offsetWidth + 10) + "px;"; }
    oAnchor.setAttribute("style", "height: 14px; width: " + oW);
    oAnchor.setAttribute("value", oValue);
    oAnchor.innerHTML = oText;

    if (window.addEventListener) { // Mozilla, Netscape, Firefox
        oAnchor.setAttribute("onclick", "roCB_Clicked('" + objComboId + "_LabelChild_" + oNewId + "','" + objComboId + "_ComboBoxLabel','" + objComboId + "_Text','" + objComboId + "_Value');" + func + ";roCB_HideDropDownClick('" + objComboId + "_DropDown',event);");
    } 
    else { // IE
        oAnchor.setAttribute("ononclick", "roCB_Clicked('" + objComboId + "_LabelChild_" + oNewId + "','" + objComboId + "_ComboBoxLabel','" + objComboId + "_Text','" + objComboId + "_Value');" + func + ";roCB_HideDropDownClick('" + objComboId + "_DropDown',event);");
        oAnchor.setAttribute("clickId", objComboId);
        oAnchor.setAttribute("func", func);
        oAnchor.onclick = function() {
            roCB_Clicked(this.id, this.clickId + '_ComboBoxLabel', this.clickId + '_Text', this.clickId + '_Value');
            eval(this.func);
            roCB_HideDropDownClick(this.clickId + '_DropDown', event);
        };
    }
    
    oTD.appendChild(oAnchor);
   
}

// Recupera el valor del combo segons el Texte
function findValueByText(oDivContID,txtFind){
    var retVal = '';
    var arrObjs = new Array();
    
    oDiv = document.getElementById(oDivContID);
    if(oDiv == null) { return retVal; }
    
    arrObjs = oDiv.getElementsByTagName('a');
    for(var n=0;n<arrObjs.length;n++){
        var obj = arrObjs[n];
        if(obj.innerHTML == txtFind){
            retVal = obj.getAttribute("value");
            break;
        }
    }
    return retVal;
}

// Recupera el texte del combo segons el valor
function findTextByValue(oDivContID, txtValue){
    var retVal = '';
    var arrObjs = new Array();
    
    oDiv = document.getElementById(oDivContID);
    if(oDiv == null){return retVal; }

    arrObjs = oDiv.getElementsByTagName('a');
    
    for(var n=0;n<arrObjs.length;n++){
        var obj = arrObjs[n];
        if(obj.getAttribute("value") == txtValue){
            retVal = obj.innerHTML;
            break;
        }
    }
    
    return retVal;
}

document.onclick = roCB_hideAllCombos;