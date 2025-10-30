


function showWUF(frmID,bol,ignoreBg) {
    try {
        var divC = document.getElementById(frmID + '_frm');
        if (divC != null) {
            if (bol == true) {
                if (ignoreBg != true) {
                    disableScreenWUF(frmID, true);
                }
                
                divC.style.display = '';
                divC.style.marginLeft = ((divC.offsetWidth / 2) * -1) + "px" ;
                divC.style.marginTop = (((divC.offsetHeight / 2) * -1)) + "px";   //- 160;
            } else {
                if (ignoreBg != true) {
                    disableScreenWUF(frmID, false);
                }
                
                divC.style.display = 'none';
            }
        }
    } catch (e) { showError("showWUF", e); }
}

/* funcio per bloquejar sols l'area menu */
function disableScreenWUF(frmID, bol) {
    var divBg = document.getElementById('divModalBgDisabled');
    if (divBg != null) {
        if (bol == true) {
            document.body.style.overflow = "hidden";

            divBg.style.height = 2000; 
            divBg.style.width = 3000;

            divBg.style.display = '';
        } else {
            document.body.style.overflow = "";
            divBg.style.display = 'none';
        }
    }
}