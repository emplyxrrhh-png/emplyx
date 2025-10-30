//Funcio Mostra / Amaga el Filtre avançat (flotant)
function filterFloatVisible(objPrefix){
 var objDiv = document.getElementById(objPrefix + '_divFiltreAvan_Float');
    if(objDiv.style.display==''){
       objDiv.style.display='none';
       objDiv.style.position='relative';
    } else {
        objDiv.style.display='';            
        objDiv.style.position='absolute'; 
    }
}

//Funcio Mostra / Amaga el Filtre avançat (embed)
function filterEmbeddedVisible(objPrefix,objPrefixTree){
    var oWidth = '';
    var oHeight = '';
    var objDiv = document.getElementById(objPrefix + '_divFiltreAvan'); //Div de fora (no scroll)
    var objContainer = document.getElementById(objPrefix + '_dvContainer'); //Div amb scroll
        
    var treed1 = document.getElementById(objPrefix + '_' + objPrefixTree + '_tree-div');
    var treed2 = document.getElementById(objPrefix + '_' + objPrefixTree + '_tree-div-UF');
    var treed3 = document.getElementById(objPrefix + '_' + objPrefixTree + '_tree-div-FF');
    
    if(treed1 != null) {if(treed1.style.display!='none') { oWidth = treed1.offsetWidth; oHeight = treed1.offsetHeight; } }
    if(treed2 != null) {if(treed2.style.display!='none') { oWidth = treed2.offsetWidth; oHeight = treed2.offsetHeight; } }
    if(treed3 != null) {if(treed3.style.display!='none') { oWidth = treed3.offsetWidth; oHeight = treed3.offsetHeight; } }
    
    if(objDiv.style.display==''){
        objDiv.style.display='none';
        objDiv.style.position = 'relative';
    } else {
        objDiv.style.position = 'absolute';
        if(oWidth != '') { objDiv.style.width=oWidth + 'px'; }
        if(oHeight != '') { objDiv.style.height=oHeight + 'px'; }
        objDiv.style.display='';
        if(objContainer != null){
            objContainer.style.width = (oWidth - 8) + 'px';
            objContainer.style.height = (oHeight - 35) + 'px';
        }
    }
}

//Borra els camps del Filtre avançat
function ClearUserFieldFilter(objPrefix){
    for(var n=1;n<6;n++){
        var usrField = document.getElementById(objPrefix + '_UserFieldFilter' + n + '_ComboBoxLabel');
        var crtField = document.getElementById(objPrefix + '_CriteriaFilter' + n + '_ComboBoxLabel');
        var valField = document.getElementById(objPrefix + '_ValueFilter' + n);
        var optAnd = document.getElementById(objPrefix + '_OptionAND' + n);
        
        if(usrField != null){ 
            roCB_setText('',objPrefix + '_UserFieldFilter' + n + '_ComboBoxLabel','','');
        }
        
        if(crtField != null){ 
            roCB_setText('',objPrefix + '_CriteriaFilter' + n + '_ComboBoxLabel','','');
        }
        
        if(valField != null){ valField.value = ''; }
        if(optAnd != null){ optAnd.checked = true; }
    }
}

//Activa el filtre avançat
function SaveUserFieldFilter(objPrefix, objPrefixTree, mJSFilterShow) {
    var strFilter = '';
    var strAux = '';
    
    //Recorrem els camps de consulta
    for(var n=1;n<6;n++){
        var usrField = document.getElementById(objPrefix + '_UserFieldFilter' + n + '_ComboBoxLabel');
        var crtField = document.getElementById(objPrefix + '_CriteriaFilter' + n + '_ComboBoxLabel');
        var valField = document.getElementById(objPrefix + '_ValueFilter' + n);
        var optAnd = document.getElementById(objPrefix + '_OptionAND' + n);

        if(usrField != null) { //Si el camp de usuari es null, salta
            if(usrField.innerHTML != ''){ //Si el camp de usuari esta en blanc, salta
                if(crtField != null){  //Si el camp de criteri es null, salta
                    if (crtField.innerHTML != '') { //Si el camp de criteri es blanc, salta

                        //Proteger valor (problemas al codificar/decodificar) le agregamos parentesis al tipo de campo xxx|y --> xxx|(y)
                        strAux = usrField.getAttribute("value").split("|")[0] + "|(" + usrField.getAttribute("value").split("|")[1] + ")"
                        strFilter += strAux + '~' + crtField.getAttribute("value");
                        
                        if(valField == null){ 
                           strFilter += '~' +  ''; 
                        } else {
                            //Proteger valor (problemas al codificar/decodificar) le agregamos parentesis
                            strAux = '(' + valField.value + ')'
                            strFilter += '~' + strAux;
                        }
                        if(optAnd != null){
                            if(optAnd.checked == true){
                                strFilter += '~AND';  
                            } else {
                                strFilter += '~OR';
                            } 
                        } 
                    }
                }
            }
        }
        
        strFilter += String.fromCharCode(127);
    }
    
    //Recupera el filtre actual 
    var arrFilters = getFilter(objPrefix + '_' + objPrefixTree);
    setFilter(arrFilters[0], strFilter,objPrefix + '_' + objPrefixTree);
    var icoFilter = document.getElementById(objPrefix + '_icoFilt5');
    if (icoFilter.className.split(' ')[1] == 'icoUnPressed') {        
        UpdTreeFilter(icoFilter,objPrefix,objPrefixTree);
    }
    else {     
        eval(objPrefix + '_' + objPrefixTree + '_roTrees.LoadTreeViews();');
    }
    
    //Executa el tancament corresponent (segons tipus de finestra de filtrat)
    eval(mJSFilterShow);

    var hdnAfterSelectFilterFuncion = document.getElementById(objPrefix + '_' + "hdnAfterSelectFilterFuncion");
    if (hdnAfterSelectFilterFuncion.value != "") {
        try {
            eval(hdnAfterSelectFilterFuncion.value);
        } catch (e) { }
    }


}


//TODO ISM check aync on this file

