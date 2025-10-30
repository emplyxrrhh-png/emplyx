/*
jsGrid : Grid dinamic a traves de Javascript

-- Funcions definides --------------------

    destroyGrid: Esborra el jsGrid
    createTable: Crea la taula que contindra els rows
    createHeaders: Crea el header del grid
    createRow: Genera un Row
    createGridButtons: Genera els botons de editar / esborrar / moure
    getRows: Retorna un array amb els rows
    reeStyleGridList: Arregla el estil del grid (alternate-style)
    toJSONStructure: Retorna un JSON amb els rows
    toJSONStructureAdvanced: Retorna un JSON amb els rows amb altre format
    deleteRow: Eliminem un Row i arreglem els estils
    removeRow: Eliminem un Row
    addRows: Afegeix multiples rows al grid 
    existRow: Comprovem si existeix un row amb unes keys definides i retorna l'id del row
-------------------------------------------
*/
function jsGrid(divContainer, headers, arrValues, editRows, delRows, moveRows, Prefix, fixHeader, isSortable, sortedField, sortOrder, containsHtml) {

    var divID; //Div on es gravará la taula
    var objPrefix; //Prefix per la taula
    var oHeaders; //Estructura de la capcelera (Array de JSON Headers {fieldname, description, size})
    var oValues; //Valors del grid (Array JSON {fieldname, value}

    var bolEditRows = false; //Boto editar al row
    var bolDelRows = false; //Boto eliminar al row
    var bolMoveRows = false; //Botons moure al row
    var bolFixHeader = false; //Capcelera fixa
    var bolIsSortable = false; //Es poden ordenar les capceleres
    
    var strSortedField = ""; // Camp per ordenar
    var strSortOrder = ""; //Tipus d'ordenació
    var bolContainsHtml = true;
    
    /*******************************************************************
    destroyGrid: Esborra el jsGrid
    *******************************************************************/
    this.destroyGrid = function() {
        try {
            var divContainerHdr = document.getElementById(divID + "Header");
            var divContainer = document.getElementById(divID);
            if (divContainerHdr != null) {
                divContainerHdr.innerHTML = '';
            }
            if (divContainer != null) {
                divContainer.innerHTML = '';
            } //end if

        } catch (e) { showError("jsGrid::destroyGrid", e); }
    }    //end function 

    /*******************************************************************
    createTable: Crea la taula que contindra els rows
    *******************************************************************/
    this.createTable = function() {
        try {
            var oTable = document.createElement("TABLE");
            oTable.id = "tblGrid" + objPrefix;
            oTable.className = "GridStyle GridEmpleados";
            oTable.setAttribute("border", "0");
            oTable.setAttribute("cellPadding", "0");
            oTable.setAttribute("cellSpacing", "0");

            var divContainer = document.getElementById(divID);
            if (divContainer != null) { divContainer.appendChild(oTable); }

            oTable.setAttribute("style", "width: 100%; cellpadding:0; cellspacing: 0;");
            oTable.style.width = "100%";

            if (bolFixHeader) {
                var oTableHeader = this.createTableHeader();
                this.createHeaders(oTableHeader);
            } else {
                this.createHeaders(oTable);
            }

        } catch (e) { showError("jsGrid::createTable", e); }
    }       //end function

    /*******************************************************************
    createTableHeader: Crea la taula que contindra els rows de la capcelera (bolFixHeader = true)
    *******************************************************************/
    this.createTableHeader = function() {
        try {
            var oTableHeader = document.createElement("TABLE");
            oTableHeader.id = "tblGridHdr" + objPrefix;
            oTableHeader.className = "GridStyle GridEmpleados";
            oTableHeader.setAttribute("border", "0");
            oTableHeader.setAttribute("cellPadding", "0");
            oTableHeader.setAttribute("cellSpacing", "0");

            var divContainerHeader = document.getElementById(divID + "Header");
            if (divContainerHeader != null) { divContainerHeader.appendChild(oTableHeader); }

            oTableHeader.setAttribute("style", "width: 100%; cellpadding:0; cellspacing: 0;");
            oTableHeader.style.width = "100%";

            return oTableHeader;
        } catch (e) { showError("jsGrid::createTableHeader", e); }
    }        //end function

    /*******************************************************************
    createHeaders: Crea el header del grid
    *******************************************************************/
    this.createHeaders = function(oTable) {
        try {
            if (oHeaders.length == 0) { return; }

            var oTR = oTable.insertRow(-1);

            if (bolEditRows == true) {
                var oTD = oTR.insertCell(-1); //Name
                oTD.className = "GridStyle-cellheader";
                oTD.style.width = "19px";
                oTD.innerHTML = "&nbsp;";
            }

            for (var n = 0; n < oHeaders.length; n++) {
                var oTD = oTR.insertCell(-1); //Name

                if (oHeaders[n].size != "-1") {
                
                    oTD.className = "GridStyle-cellheader";
                    oTD.style.width = oHeaders[n].size;

                    if (bolIsSortable) {
                        var classSort = "";
                        if (oHeaders[n].fieldname.toUpperCase() == strSortedField.toUpperCase()) { classSort = "orderheader" + strSortOrder.toLowerCase(); }
                        oTD.innerHTML = '<a href="javascript:void(0);" ordering="' + strSortOrder + '" class="' + classSort + '" id="aHeader_' + objPrefix + '_' + oHeaders[n].fieldname + '" onclick="sortGrid' + objPrefix + '(this,\'' + objPrefix + '\',\'' + oHeaders[n].fieldname + '\');">' + oHeaders[n].description + '</a>';
                    } else {
                        oTD.textContent = oHeaders[n].description;
                    }

                    if (bolFixHeader) {
                        oTD.style.borderBottom = "0";
                    }
                }
                else {
                    oTD.style.display = "none";
                    oTD.innerHTML = "&nbsp;";
                }

            } //end for

            if (bolDelRows == true) {
                var oTD = oTR.insertCell(-1); //Name
                oTD.className = "GridStyle-cellheader";
                oTD.style.width = "19px";
                oTD.innerHTML = "&nbsp;";
            }

            if (bolFixHeader) {
                var oTD = oTR.insertCell(-1); //Name
                oTD.style.width = "19px";
            }

        } catch (e) { showError("jsGrid::createHeaders", e); }
    }                  //end function 

    /*******************************************************************
    createRow: Genera un Row
    *******************************************************************/
    this.createRow = function(arrFields, bolAtts) {
        try {
            //Carreguem el array global per mantenir els valors
            var n;

            var oTable = document.getElementById('tblGrid' + objPrefix);

            //Si no existeix la taula, la creem 
            if (oTable == null) {
                this.createTable();
                oTable = document.getElementById('tblGrid' + objPrefix);
            }

            var oNewId = this.getMaxID();
            var altRow = 1;

            /*Afegim el tr */
            var oTR = oTable.insertRow(-1);
            oTR.id = "htRow" + objPrefix + "C_" + oNewId;
            oTR.setAttribute("name", "htRows" + objPrefix);
            oTR.setAttribute("idtr", "htRow" + objPrefix + "C_" + oNewId);

            this.createGridButtons(oTR, oNewId, true);

            if ((oNewId % 2) != 0) {
                altRow = "1";
            } else {
                altRow = "2";
            }

            var fieldName;
            var value;            
            for (n = 0; n < arrFields.length; n++) {
                if (bolAtts != null) {
                    fieldName = arrFields[n].attname.toUpperCase();
                    value = arrFields[n].value;
                } else {
                    fieldName = arrFields[n].field.toUpperCase();
                    value = arrFields[n].value;
                }

                var showvalue = "";
                if(containsHtml == false){
                    showvalue = decodeURIComponent(value);
                    showvalue = this.removeHtmlTags(showvalue);
                }else{
                    showvalue = value;
                }
                
                //Establim l'atribut al TR
                oTR.setAttribute("jsGridAtt_" + fieldName, value);

                //Si existeix el Header, insertem la columna (oHeader.fieldName = arrFields.field)
                var existHeader = false;
                var sizeHeader = "95%";
                var containsHTML = false;
                for (var oH = 0; oH < oHeaders.length; oH++) {
                    if (oHeaders[oH].fieldname.toUpperCase() == fieldName) {
                        existHeader = true;
                        sizeHeader = oHeaders[oH].size;
                        if (typeof oHeaders[oH].html != 'undefined' && oHeaders[oH].html) containsHTML = oHeaders[oH].html;
                        break; // exit for 
                    } //end if
                } //end for

                //Si existeix la capcelera, afegim linia
                if (existHeader) {
                    var oTD = oTR.insertCell(-1);
                    oTD.id = "jsGridField" + objPrefix + "_" + fieldName + "_" + oNewId;
                    oTD.className = "GridStyle-cell" + altRow;
                    if (value == "") { value = "&nbsp;"; }

                    if (containsHTML) {
                        if (showvalue.length > 300) {
                            oTD.innerHTML = showvalue.substring(0, 300) + "...";
                        } else {
                            oTD.innerHTML = showvalue;
                        }
                    } else {
                        if (showvalue.length > 300) {
                            oTD.textContent = showvalue.substring(0, 300) + "...";
                        } else {
                            oTD.textContent = showvalue;
                        }
                    }
                    

                    if (sizeHeader == "-1")
                        oTD.style.display = "none";
                    else
                        oTD.style.width = sizeHeader;

                    oTD.setAttribute("idTr", "htRow" + objPrefix + "C_" + oNewId);
                    oTR.setAttribute("idTr", "htRow" + objPrefix + "C_" + oNewId);

                    oTD.setAttribute("jsIE", "editGrid" + objPrefix + "('htRow" + objPrefix + "C_" + oNewId + "');");

                    if (window.addEventListener) { // Mozilla, Netscape, Firefox
                        oTD.setAttribute("onmouseover", "javascript: jsGrid_rowOver('htRow" + objPrefix + "C_" + oNewId + "');");
                        oTD.setAttribute("onmouseout", "javascript: jsGrid_rowOut('htRow" + objPrefix + "C_" + oNewId + "');");
                        //if(bolEditRows){
                            oTD.setAttribute("onclick", "javascript: editGrid" + objPrefix + "('htRow" + objPrefix + "C_" + oNewId + "');");
                        //}
                    } else { // IE
                        oTR.onmouseover = function() { jsGrid_rowOver(this.getAttribute("idTr")); }
                        oTR.onmouseout = function() { jsGrid_rowOut(this.getAttribute("idTr")); }
                        if(bolEditRows){
                            oTD.onclick = function() { eval(this.getAttribute("jsIE")); }
                        }
                    }
                }
            } //end for

            this.createGridButtons(oTR, oNewId, false);

            //Retorna el ID del row
            return "htRow" + objPrefix + "C_" + oNewId;

        } catch (e) { showError("createRow", e); return ""; }
    }

    this.removeHtmlTags =  function removeHTMLTags(htmlString){
        if(htmlString){
            var mydiv = document.createElement("div");
            mydiv.innerHTML = htmlString;
 
            if (document.all) // IE Stuff
            {
                return mydiv.innerText;
               
            }   
            else // Mozilla does not work with innerText
            {
                return mydiv.textContent;
            }                           
      } else {
        return htmlString;
      }
   } 
    /*******************************************************************
    createGridButtons: Genera els botons de editar / esborrar / moure
    *******************************************************************/
    this.createGridButtons = function(oTR, oNewId, isEditButtonEdit) {
        try {
            if (isEditButtonEdit == true && bolEditRows == false) { return; }

            //Si no hi ha definit cap boto, sortim de la funcio
            if (isEditButtonEdit == false && bolDelRows == false && bolMoveRows == false) { return; }

            //Creem la barra d'eines al TR
            var oTD = oTR.insertCell(-1); //Name
            oTD.className = "GridStyle-cellheader";
            //oTD.style.borderRight = "solid 1px silver";
            oTD.width = "19px";
            oTD.style.whiteSpace = "nowrap";
            //oTD.style.borderRight = "solid 1px silver";

            var aEdit = document.createElement("A");
            var aDelete = document.createElement("A");

            aEdit.href = "javascript: void(0);";
            aEdit.name = "tblGrid" + objPrefix + "_btnEdit";
            aDelete.href = "javascript: void(0);";
            aDelete.name = "tblGrid" + objPrefix + "_btnRemove";
            
            if (document.getElementById("tagEditTitle") != null) {
                aEdit.title = document.getElementById("tagEditTitle").value;
            } else {
                aEdit.title = "Edit";
            }

            if (document.getElementById("tagRemoveTitle") != null) {
                aDelete.title = document.getElementById("tagRemoveTitle").value;
            } else {
                aDelete.title = "Delete";
            }

            aEdit.setAttribute("row", "htRowC_" + oNewId);
            aDelete.setAttribute("row", "htRowC_" + oNewId);

            aEdit.setAttribute("jsIE", "editGrid" + objPrefix + "('" + oTR.id + "');");
            aDelete.setAttribute("jsIE", "deleteGrid" + objPrefix + "('" + oTR.id + "');");

            if (window.addEventListener) { // Mozilla, Netscape, Firefox
                aEdit.setAttribute("onclick", "javascript: editGrid" + objPrefix + "('" + oTR.id + "');");
                aDelete.setAttribute("onclick", "javascript: deleteGrid" + objPrefix + "('" + oTR.id + "');");
            } else { // IE
                aEdit.onclick = function() { eval(this.getAttribute("jsIE")); }
                aDelete.onclick = function() { eval(this.getAttribute("jsIE")); }
            }

            var imgEdit = document.createElement("IMG");
            var imgDelete = document.createElement("IMG");
            imgEdit.src = hBaseRef + "Images/Grid/edit.png";
            imgDelete.src = hBaseRef + "Images/Grid/remove.png";

            aEdit.appendChild(imgEdit);
            aDelete.appendChild(imgDelete);

            if (isEditButtonEdit &&  bolEditRows) { oTD.appendChild(aEdit); }
            if (isEditButtonEdit == false && bolDelRows) { oTD.appendChild(aDelete); }

            oTD.style.backgroundColor = "#E8EEF7";

            if (bolMoveRows) {
                // ---------------------------------------------------- Up / Down
                //Creem la barra d'eines al TR
                var oTD2 = oTR.insertCell(-1); //Name
                oTD2.className = "GridStyle-cellheader";
                //oTD.style.borderRight = "solid 1px silver";
                oTD2.width = "50px";
                oTD2.style.whiteSpace = "nowrap";
                oTD2.style.borderRight = "solid 1px silver";

                var aMUP = document.createElement("A");
                var aMDown = document.createElement("A");

                aMUP.href = "javascript: void(0);";
                aMUP.name = "tblGrid" + objPrefix + "_moveUp";
                aMDown.href = "javascript: void(0);";
                aMDown.name = "tblGrid" + objPrefix + "_moveDown";

                aMUP.setAttribute("jsIE", "jsGrid_moveRow('tblGrid" + objPrefix + "',this.parentNode, -1);");
                aMDown.setAttribute("jsIE", "jsGrid_moveRow('tblGrid" + objPrefix + "',this.parentNode, 1);");

                if (window.addEventListener) { // Mozilla, Netscape, Firefox
                    aMUP.setAttribute("onclick", "javascript: jsGrid_moveRow('tblGrid" + objPrefix + "',this.parentNode, -1);");
                    aMDown.setAttribute("onclick", "javascript: jsGrid_moveRow('tblGrid" + objPrefix + "',this.parentNode, 1);");
                } else { // IE
                    aMUP.onclick = function() { eval(this.getAttribute("jsIE")); }
                    aMDown.onclick = function() { eval(this.getAttribute("jsIE")); }
                }

                var imgUP = document.createElement("IMG");
                var imgDown = document.createElement("IMG");
                imgUP.src = hBaseRef + "Images/Grid/uparrow.png";
                imgDown.src = hBaseRef + "Images/Grid/downarrow.png";

                aMUP.appendChild(imgUP);
                aMDown.appendChild(imgDown);
                oTD2.appendChild(aMUP);
                oTD2.appendChild(aMDown);
                oTD2.style.backgroundColor = "#E8EEF7";
            } //end if movebuttons

        } catch (e) { showError("createGridButtons", e); }
    }
    
    /*******************************************************************
    getRows: Retorna un array amb els rows
    *******************************************************************/
    this.getRows = function() {
        try {
            var hTable = document.getElementById("tblGrid" + objPrefix);
            var newArr = new Array();

            //Si no hi ha taula
            if (hTable == null) { return null; }

            var hRows;

            switch (BrowserDetect.browser) {
                case 'Firefox':
                case 'Safari':
                case 'Chrome':
                    hRows = document.getElementsByName("htRows" + objPrefix);
                    break;
                case 'Explorer':
                    hRows = getElementsByName_iefix("TR", "htRows" + objPrefix);
                    break;
                default:
                    hRows = document.getElementsByName("htRows" + objPrefix);
                    if (hRows.length == 0) {
                        hRows = getElementsByName_iefix("TR", "htRows" + objPrefix);
                    }
                    break;
            } //end switch
            
            return hRows;
        } catch (e) { showError("jsGrid::getRows", e); return null; }
    } //end function

    /*******************************************************************
    reeStyleGridList: Arregla el estil del grid
    *******************************************************************/
    this.reeStyleGridList = function() {
        try {
            var hTable = document.getElementById('tblGrid' + objPrefix);

            //Si no hi ha taula
            if (hTable == null) { return; }
            if (hTable.rows.length < 2) { return; }

            //Bucle per les files del grid, per cambiar els estils
            for (var n = 1; n < hTable.rows.length; n++) {
                if ((n % 2) != 0) {
                    altRow = "2";
                } else {
                    altRow = "1";
                } //end if

                var hRow = hTable.rows[n];

                var oRest = 0;
                if (bolEditRows) { oRest += 1; }
                if (bolDelRows) { oRest += 1; }
                if (bolMoveRows) { oRest += 1; }

                for (var nR = 0; nR <= hRow.cells.length - oRest; nR++) {
                    hRow.cells[nR].className = 'GridStyle-cell' + altRow;
                }

            } //end for 

        } catch (e) { showError("jsGrid::reeStyleGridList", e); }
    }          //end function

    /*******************************************************************
    toJSONStructureAdvanced: Retorna un JSON amb els rows advanced
    *******************************************************************/
    this.toJSONStructureAdvanced = function() {
        try {
            var hTable = document.getElementById('tblGrid' + objPrefix);

            //Si no hi ha taula
            if (hTable == null) { return; }
            if (hTable.rows.length < 2) { return []; }

            var arrRows = new Array();

            //Bucle per les files del grid, per recuperar els valors
            for (var n = 1; n < hTable.rows.length; n++) {

                var hRow = hTable.rows[n];

                var oRow = {};

                for (var x = 0; x < hRow.attributes.length; x++) {
                    if (hRow.attributes[x].nodeName.toLowerCase().indexOf("jsgridatt_") == 0) {
                        var f = hRow.attributes[x].nodeName.toUpperCase().replace("JSGRIDATT_", "").toLowerCase();
                        var v = hRow.attributes[x].nodeValue;
                        oRow[f] = v; 
                    } //end if

                } //end for

                arrRows.push(oRow); //Afegim el row
            }

            return arrRows;

        } catch (e) { showError("jsGrid::toJSONStructure", e); return null; }

    }     //end function

    /*******************************************************************
    toJSONStructure: Retorna un JSON amb els rows
    *******************************************************************/
    this.toJSONStructure = function() {
        try {
            var hTable = document.getElementById('tblGrid' + objPrefix);

            //Si no hi ha taula
            if (hTable == null) { return; }
            if (hTable.rows.length < 2) { return []; }

            var arrRows = new Array();
            var arrFields = new Array();

            //Bucle per les files del grid, per recuperar els valors
            for (var n = 1; n < hTable.rows.length; n++) {

                var hRow = hTable.rows[n];
                arrFields = new Array(); //Reiniciem l'array de camps

                for (var x = 0; x < hRow.attributes.length; x++) {
                    if (hRow.attributes[x].nodeName.toLowerCase().indexOf("jsgridatt_") == 0) {
                        var oJSField = { 'field': '', 'value': '', 'control': '', 'type': '' }; //fet aixi per no parsejar
                        oJSField.field = hRow.attributes[x].nodeName.toUpperCase().replace("JSGRIDATT_", "");
                        oJSField.value = hRow.attributes[x].nodeValue;
                        arrFields.push(oJSField); //Afegim el camp
                    } //end if

                } //end for

                arrRows.push(arrFields); //Afegim el row
            }

            return arrRows;

        } catch (e) { showError("jsGrid::toJSONStructure", e); return null; }

    }     //end function

    /*******************************************************************
    deleteRow: Eliminem un Row
    *******************************************************************/
    this.deleteRow = function(objId) {
        try {
            this.removeRow(objId);
            this.reeStyleGridList();
            return true;
        } catch (e) { showError("jsGrid::deleteRow", e); return false; }
    }

    /*******************************************************************
    existRow: Comprovem si existeix un row amb unes keys definides
    strKeys = nom de la columna a comparar (ID)
    oRow = JSON Row amb estructura de camps
    bolAtts = si es passa, es {attname, value}
    *******************************************************************/
    this.existRow = function(strKey, oRow, bolAtts) {
        try {
            var hTable = document.getElementById('tblGrid' + objPrefix);

            var oResult = "";

            //Si no hi ha taula
            if (hTable == null) { return oResult; }

            var fieldName; //nom de camp
            var value; // valor

            var oRowID;
            var oRowValue;

            //Busquem al oRow el valor de la strKey
            for (n = 0; n < oRow.length; n++) {
                if (bolAtts != null) {
                    fieldName = oRow[n].attname.toUpperCase();
                    value = oRow[n].value;
                } else {
                    fieldName = oRow[n].field.toUpperCase();
                    value = oRow[n].value;
                }

                if (fieldName.toLowerCase() == strKey) {
                    oRowID = fieldName;
                    oRowValue = value;
                    break;
                }
            }

            //Bucle per les files del grid
            for (var n = 1; n < hTable.rows.length; n++) {
                var hRow = hTable.rows[n];

                if (oRowValue == hRow.getAttribute("jsgridatt_" + strKey.toLowerCase())) {
                    return hRow.id;
                }
            } //end for

            return oResult;
        } catch (e) { showError("jsGrid::existRow", e); return ""; }
    }

    /*******************************************************************
    editRow: Establir atributs / valors al row corresponent
    Exemple JSON per restablir atributs
        var oAtts = [{ 'attname': 'idzone', 'value': TLId },
                     { 'attname': 'begin', 'value': Begin }];    
    *******************************************************************/
    this.editRow = function(rowID, arrFields) {
        try {
            var hTable = document.getElementById('tblGrid' + objPrefix);

            //Si no hi ha taula
            if (hTable == null) { return; }

            var oTR = document.getElementById(rowID);
            if (oTR == null) { return; }

            for (n = 0; n < arrFields.length; n++) {
                var fieldName = arrFields[n].attname.toUpperCase();
                var value = arrFields[n].value;

                oTR.setAttribute("jsGridAtt_" + fieldName, value);

                //jsGridFieldPeriods_WEEKDAYNAME_1
                for (var oC = 0; oC < oTR.cells.length; oC++) {
                    var oCell = oTR.cells[oC];
                    var parmCell = new Array();
                    parmCell = oCell.id.split("_");
                    if (parmCell.length > 1) {
                        if (parmCell[1].toUpperCase() == fieldName.toUpperCase()) {
                            oCell.innerHTML = value;
                        }
                    }
                }

            } //end for

        } catch (e) { showError("jsGrid::editRow", e); }
    }                //end function 

    /* ***************************************************************************************************/
    // removeRow: Eliminem un Row de la taula per ID
    /* ***************************************************************************************************/
    this.removeRow = function(objId) {
        try {
            var hTable = document.getElementById('tblGrid' + objPrefix);

            //Si no hi ha taula
            if (hTable == null) { return; }

            var hRows = this.getRows();
            
            //Bucle per les files del grid, per eliminar les files
            for (var n = 0; n < hTable.rows.length; n++) {
                var hRow = hTable.rows[n];
                if (hRow.id == objId) {
                    //si troba el row, l'eliminem (n+1, per la capcelera)
                    hTable.deleteRow(n);
                    return true;
                } //end if
            } //end for

            return false;
        } catch (e) { showError("removeRow", e); return false; }
    }     //end function

    /* ***************************************************************************************************/
    // getMaxID: Recupera el Max ID dels rows 
    /* ***************************************************************************************************/
    this.getMaxID = function() {
        try {
            var arr = new Array();
            var hTable = document.getElementById('tblGrid' + objPrefix);

            //Si no hi ha taula
            if (hTable == null) { return 1; }

            var oCount = 0;
            var oStart = 1;
            if (bolFixHeader) { oStart = 0; }
            for (var n = oStart; n < hTable.rows.length; n++) {
                var hRow = hTable.rows[n];
                var arrID = hRow.id.split("_");
                oCountID = parseInt(arrID[arrID.length - 1], 10);
                if (oCount <= oCountID) { oCount = oCountID; }
            }

            oCount++
            return oCount;
        } catch (e) {
            showError("getMaxID", e);
            return 0;
        }
    }

    /* ***************************************************************************************************/
    // getCountRows: Recupera el numero de rows al grid
    /* ***************************************************************************************************/
    this.getCountRows = function() {
        try {
            var arr = new Array();
            var hTable = document.getElementById('tblGrid' + objPrefix);

            //Si no hi ha taula
            if (hTable == null) { return 0; }

            var oCount = 0;
            if(hTable.rows.length > 0) { oCount = (hTable.rows.length -1); }
            return oCount;
        } catch (e) {
            showError("getCountRows", e);
            return 0;
        }
    }

    /*******************************************************************
    retRowJSON: Retorna els valors del Row
    *******************************************************************/
    this.retRowJSON = function(idRow) {
        try {
            var oRow = document.getElementById(idRow);
            if (oRow == null) { return; }

            var oAtts = new Array();

            for (var x = 0; x < oRow.attributes.length; x++) {
                if (oRow.attributes[x].nodeName.toLowerCase().indexOf("jsgridatt_") == 0) {
                    var oAttField = { 'attname': '', 'value': '' };
                    oAttField.attname = oRow.attributes[x].nodeName.toLowerCase();
                    oAttField.value = oRow.attributes[x].nodeValue;
                    oAtts.push(oAttField);
                } // end if
            } // end for

            return oAtts;

        } catch (e) { showError("jsGrid::retRowJSON", e); return null; }
    }

    /*******************************************************************
    addRows: Afegeix multiples rows al grid 
                {rows: [ {fields: [] } ,... ] }
    *******************************************************************/
    this.addRows = function(arrValues, strKey, bolAtts) {
        try {
            if (arrValues != null) {
                for (var n = 0; n < arrValues.length; n++) {
                    if (strKey != "") {
                        var rowID = this.existRow(strKey, arrValues[n].fields, bolAtts);
                        if (rowID == "") {
                            this.createRow(arrValues[n].fields, bolAtts); //Creem els rows
                        } else {
                            this.editRow(rowID, arrValues[n].fields); //Creem els rows
                        }
                    } else { //Si no es pasa clau, sols afegim 
                        this.createRow(arrValues[n].fields, bolAtts); //Creem els rows
                    }
                }
            }
        } catch (e) { showError("jsGrid::addRows", e); return false; }
    }

    /*******************************************************************
    createGrid: Genera el jsGrid
    *******************************************************************/
    this.createGrid = function() {
        try {
            this.destroyGrid();
            if (oHeaders != null) { this.createTable(); }
            if (oValues != null) {
                for (var n = 0; n < oValues.length; n++) {
                    this.createRow(oValues[n].fields); //Creem els rows
                }
            }

        } catch (e) { showError("createGrid", e); }
    }

    //Inicialitzem les variables
    divID = divContainer;
    objPrefix = Prefix;
    oHeaders = headers;
    oValues = arrValues;

    bolEditRows = editRows;
    bolDelRows = delRows;
    bolMoveRows = moveRows;
    if (fixHeader != null) { bolFixHeader = fixHeader; }
    if (isSortable != null) { bolIsSortable = isSortable; }
    if (sortedField != null) { strSortedField = sortedField; }
    if (sortOrder != null) { strSortOrder = sortOrder.toUpperCase(); }
    if(typeof(containsHtml) != 'undefined') { bolContainsHtml = containsHtml; }

    if (divID != null) {
        this.createGrid();
    }

} // end class ------------------------------------------------------------------------------------------

/* onmouseover Row (tr) */
function jsGrid_rowOver(rowID) {
    try {
        var table = document.getElementById(rowID);
        var cells = table.getElementsByTagName("td");
        for (var i = 0; i < cells.length; i++) {
            addCssClass(cells[i], "gridRowOver");
        } //end for
    } catch (e) { showError("jsGrid_rowOver", e); }
}

/* onmouseout Row (tr) */
function jsGrid_rowOut(rowID) {
    try {
        var table = document.getElementById(rowID);
        var cells = table.getElementsByTagName("td");
        for (var i = 0; i < cells.length; i++) {
            removeCssClass(cells[i], "gridRowOver");
        } //end for 
    } catch (e) { showError("jsGrid_rowOut", e); }
}

/* selecció Row (tr) */
function jsGrid_rowClick(rowID, ID, dTable) {
    try {
        document.getElementById('selectedIdx').value = ID;
        var tParent = document.getElementById(dTable);
        var tCells = tParent.getElementsByTagName("td");
        for (var i = 0; i < tCells.length; i++) {
            removeCssClass(tCells[i], "gridRowOver");
            removeCssClass(tCells[i], "gridRowSelected");
        } //end for

        var table = document.getElementById(rowID);
        var cells = table.getElementsByTagName("td");
        for (var i = 0; i < cells.length; i++) {
            removeCssClass(cells[i], "gridRowOver");
            addCssClass(cells[i], "gridRowSelected");
        } //end for 

    } catch (e) { showError("jsGrid_rowClick", e); }
}

/*******************************************************************
moveRow: moure els Row (direction = -1 / 1)
*******************************************************************/
function jsGrid_moveRow(tableID, cell, direction) {
    try {
        mTable = document.getElementById(tableID);
        rowIndex = parseInt(cell.parentNode.rowIndex);

        if ((rowIndex == 1 && direction > 0) || (rowIndex == mTable.rows.length - 1 && direction < 0) || (rowIndex > 0 && rowIndex < mTable.rows.length - 1)) {
            if (mTable.rows.length > 2) {
                if ((rowIndex + direction) > 0) {
                    var row = mTable.rows[rowIndex];
                    var cell0 = mTable.rows[rowIndex].cells[0];
                    var cell1 = mTable.rows[rowIndex].cells[1];
                    var cell2 = mTable.rows[rowIndex].cells[2];
                    mTable.deleteRow(rowIndex);
                    var oRow = mTable.insertRow(rowIndex + direction);

                    for (var x = 0; x < row.attributes.length; x++) {
                        oRow.setAttribute(row.attributes[x].nodeName, row.attributes[x].nodeValue);
                    } // end for 
                    oRow.id = row.id;

                    oRow.appendChild(cell0); // = swapHTML;
                    oRow.appendChild(cell1); // = swapHTML;
                    oRow.appendChild(cell2); // = swapHTML;

                    //oRow.insertCell(cell0);
                    jsGrid_reeStyleGridList(tableID);
                    hasChanges(true);
                }
            }
        } //end if
    } catch (e) { showError("jsGrid::moveRow", e); }
}


/*******************************************************************
jsGrid_reeStyleGridList: Arregla el estil del grid
*******************************************************************/
function jsGrid_reeStyleGridList(tableID) {
    try {
        var hTable = document.getElementById(tableID);

        //Si no hi ha taula
        if (hTable == null) { return; }
        
        //Bucle per les files del grid, per cambiar els estils
        for (var n = 0; n < hTable.rows.length; n++) {
            if ((n % 2) != 0) {
                altRow = "2";
            } else {
                altRow = "1";
            } //end if

            var hRow = hTable.rows[n];
            for(var oC = 0;oC < hRow.cells.length; oC++){
                hRow.cells[oC].className = 'GridStyle-cell' + altRow;           
            }
        } //end for 
    } catch (e) { showError("jsGrid::reeStyleGridList", e); }
} //end function 