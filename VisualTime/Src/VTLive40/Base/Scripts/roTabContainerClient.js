/* Canvi de visualització dels Tabs */
function activeTabContainer(objPrefix, idTab) {
    idTab = idTab + 1;
    var n = 0;
    var hdnTab = document.getElementById(objPrefix + "_hdnActiveTab");
    hdnTab.value = idTab;
    for (n = 1; n < 9; n++) {
        var objTd = document.getElementById(objPrefix + '_tab0' + n);
        var objDiv = document.getElementById(objPrefix + '_tbC0' + n);
        
        if (objTd != undefined) {
            if (n == idTab) {
                //createCookie(objPrefix + 'SelectedTab', n, 30);
                objTd.className = 'tabHeader-Active';
                if (objDiv != undefined) {
                    objDiv.style.display = '';
                }
            } else {
                objTd.className = 'tabHeader';
                if (objDiv != undefined) {
                    objDiv.style.display = 'none';
                }
            }
        }
    }
}