var showRange = false;

function EditAllowedIP(isNew, objDocument) {
    try {
        if (isNew) {
            showRange = false;
            setIPMaxControl(false);
            findElementById("input", "_frmIPs1_hdnIpID").value = "-1";
            findElementById("input", "_frmIPs1_txtIpMin1").value = "";
            findElementById("input", "_frmIPs1_txtIpMin2").value = "";
            findElementById("input", "_frmIPs1_txtIpMin3").value = "";
            findElementById("input", "_frmIPs1_txtIpMin4").value = "";
            findElementById("input", "_frmIPs1_txtIpMax4").value = "";
            findElementById("input", "_frmIPs1_ckInputRange").checked = false;
        } else {
            findElementById("input", "_frmIPs1_hdnIpID").value = objDocument.idRow;
            findElementById("input", "_frmIPs1_txtIpMin1").value = objDocument.txtMin1;
            findElementById("input", "_frmIPs1_txtIpMin2").value = objDocument.txtMin2;
            findElementById("input", "_frmIPs1_txtIpMin3").value = objDocument.txtMin3;
            findElementById("input", "_frmIPs1_txtIpMin4").value = objDocument.txtMin4;
            findElementById("input", "_frmIPs1_txtIpMax4").value = objDocument.txtMax4;
            findElementById("input", "_frmIPs1_ckInputRange").checked = objDocument.checked;
            setIPMaxControl(objDocument.checked);
            showRange = objDocument.checked;
        }

        disableScreen(true);
        showWndIPs(true);
    }
    catch (e) {
        showError("EditAllowedIP", e);
    }
}

var MD5 = function (d) { var r = M(V(Y(X(d), 8 * d.length))); return r.toLowerCase() }; function M(d) { for (var _, m = "0123456789ABCDEF", f = "", r = 0; r < d.length; r++)_ = d.charCodeAt(r), f += m.charAt(_ >>> 4 & 15) + m.charAt(15 & _); return f } function X(d) { for (var _ = Array(d.length >> 2), m = 0; m < _.length; m++)_[m] = 0; for (m = 0; m < 8 * d.length; m += 8)_[m >> 5] |= (255 & d.charCodeAt(m / 8)) << m % 32; return _ } function V(d) { for (var _ = "", m = 0; m < 32 * d.length; m += 8)_ += String.fromCharCode(d[m >> 5] >>> m % 32 & 255); return _ } function Y(d, _) { d[_ >> 5] |= 128 << _ % 32, d[14 + (_ + 64 >>> 9 << 4)] = _; for (var m = 1732584193, f = -271733879, r = -1732584194, i = 271733878, n = 0; n < d.length; n += 16) { var h = m, t = f, g = r, e = i; f = md5_ii(f = md5_ii(f = md5_ii(f = md5_ii(f = md5_hh(f = md5_hh(f = md5_hh(f = md5_hh(f = md5_gg(f = md5_gg(f = md5_gg(f = md5_gg(f = md5_ff(f = md5_ff(f = md5_ff(f = md5_ff(f, r = md5_ff(r, i = md5_ff(i, m = md5_ff(m, f, r, i, d[n + 0], 7, -680876936), f, r, d[n + 1], 12, -389564586), m, f, d[n + 2], 17, 606105819), i, m, d[n + 3], 22, -1044525330), r = md5_ff(r, i = md5_ff(i, m = md5_ff(m, f, r, i, d[n + 4], 7, -176418897), f, r, d[n + 5], 12, 1200080426), m, f, d[n + 6], 17, -1473231341), i, m, d[n + 7], 22, -45705983), r = md5_ff(r, i = md5_ff(i, m = md5_ff(m, f, r, i, d[n + 8], 7, 1770035416), f, r, d[n + 9], 12, -1958414417), m, f, d[n + 10], 17, -42063), i, m, d[n + 11], 22, -1990404162), r = md5_ff(r, i = md5_ff(i, m = md5_ff(m, f, r, i, d[n + 12], 7, 1804603682), f, r, d[n + 13], 12, -40341101), m, f, d[n + 14], 17, -1502002290), i, m, d[n + 15], 22, 1236535329), r = md5_gg(r, i = md5_gg(i, m = md5_gg(m, f, r, i, d[n + 1], 5, -165796510), f, r, d[n + 6], 9, -1069501632), m, f, d[n + 11], 14, 643717713), i, m, d[n + 0], 20, -373897302), r = md5_gg(r, i = md5_gg(i, m = md5_gg(m, f, r, i, d[n + 5], 5, -701558691), f, r, d[n + 10], 9, 38016083), m, f, d[n + 15], 14, -660478335), i, m, d[n + 4], 20, -405537848), r = md5_gg(r, i = md5_gg(i, m = md5_gg(m, f, r, i, d[n + 9], 5, 568446438), f, r, d[n + 14], 9, -1019803690), m, f, d[n + 3], 14, -187363961), i, m, d[n + 8], 20, 1163531501), r = md5_gg(r, i = md5_gg(i, m = md5_gg(m, f, r, i, d[n + 13], 5, -1444681467), f, r, d[n + 2], 9, -51403784), m, f, d[n + 7], 14, 1735328473), i, m, d[n + 12], 20, -1926607734), r = md5_hh(r, i = md5_hh(i, m = md5_hh(m, f, r, i, d[n + 5], 4, -378558), f, r, d[n + 8], 11, -2022574463), m, f, d[n + 11], 16, 1839030562), i, m, d[n + 14], 23, -35309556), r = md5_hh(r, i = md5_hh(i, m = md5_hh(m, f, r, i, d[n + 1], 4, -1530992060), f, r, d[n + 4], 11, 1272893353), m, f, d[n + 7], 16, -155497632), i, m, d[n + 10], 23, -1094730640), r = md5_hh(r, i = md5_hh(i, m = md5_hh(m, f, r, i, d[n + 13], 4, 681279174), f, r, d[n + 0], 11, -358537222), m, f, d[n + 3], 16, -722521979), i, m, d[n + 6], 23, 76029189), r = md5_hh(r, i = md5_hh(i, m = md5_hh(m, f, r, i, d[n + 9], 4, -640364487), f, r, d[n + 12], 11, -421815835), m, f, d[n + 15], 16, 530742520), i, m, d[n + 2], 23, -995338651), r = md5_ii(r, i = md5_ii(i, m = md5_ii(m, f, r, i, d[n + 0], 6, -198630844), f, r, d[n + 7], 10, 1126891415), m, f, d[n + 14], 15, -1416354905), i, m, d[n + 5], 21, -57434055), r = md5_ii(r, i = md5_ii(i, m = md5_ii(m, f, r, i, d[n + 12], 6, 1700485571), f, r, d[n + 3], 10, -1894986606), m, f, d[n + 10], 15, -1051523), i, m, d[n + 1], 21, -2054922799), r = md5_ii(r, i = md5_ii(i, m = md5_ii(m, f, r, i, d[n + 8], 6, 1873313359), f, r, d[n + 15], 10, -30611744), m, f, d[n + 6], 15, -1560198380), i, m, d[n + 13], 21, 1309151649), r = md5_ii(r, i = md5_ii(i, m = md5_ii(m, f, r, i, d[n + 4], 6, -145523070), f, r, d[n + 11], 10, -1120210379), m, f, d[n + 2], 15, 718787259), i, m, d[n + 9], 21, -343485551), m = safe_add(m, h), f = safe_add(f, t), r = safe_add(r, g), i = safe_add(i, e) } return Array(m, f, r, i) } function md5_cmn(d, _, m, f, r, i) { return safe_add(bit_rol(safe_add(safe_add(_, d), safe_add(f, i)), r), m) } function md5_ff(d, _, m, f, r, i, n) { return md5_cmn(_ & m | ~_ & f, d, _, r, i, n) } function md5_gg(d, _, m, f, r, i, n) { return md5_cmn(_ & f | m & ~f, d, _, r, i, n) } function md5_hh(d, _, m, f, r, i, n) { return md5_cmn(_ ^ m ^ f, d, _, r, i, n) } function md5_ii(d, _, m, f, r, i, n) { return md5_cmn(m ^ (_ | ~f), d, _, r, i, n) } function safe_add(d, _) { var m = (65535 & d) + (65535 & _); return (d >> 16) + (_ >> 16) + (m >> 16) << 16 | 65535 & m } function bit_rol(d, _) { return d << _ | d >>> 32 - _ }

function changeWSMode(mode) {
    if (mode == 1) {
        txtEmpName_Client.SetEnabled(true);
        txtWSOldPassword_Client.SetEnabled(true);
        txtWSPassword_Client.SetEnabled(true);
        txtWSPasswordRepeat_Client.SetEnabled(true);
    }
    else {
        txtEmpName_Client.SetEnabled(false);
        txtWSOldPassword_Client.SetEnabled(false);
        txtWSPassword_Client.SetEnabled(false);
        txtWSPasswordRepeat_Client.SetEnabled(false);
    }
}
function generateString(length) {
    var result = '';
    var characters = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789';
    var charactersLength = characters.length;
    for (var i = 0; i < length; i++) {
        result += characters.charAt(Math.floor(Math.random() *
            charactersLength));
    }
    return result;
}

function generateToken(tokenNumber) {
    var value = generateString(15);
    var value2 = generateString(15)

    var companyCode = $('.hdnCompanyName').val();

    var result = btoa(companyCode + MD5(value) + MD5(value2));

    if (tokenNumber == 1) {
        //document.getElementById('ctl00_contentMainBody_frmWsAdmin_txtWSToken1').value = result;
        txtWSToken1_Client.SetValue(result);
        hasChanges(true);
    }
    else {
        //document.getElementById('ctl00_contentMainBody_frmWsAdmin_txtWSToken2').value = result;
        txtWSToken2_Client.SetValue(result);
        hasChanges(true);
    }
}

function findElementById(type, elementId) {
    var inputEl = null;
    var elems = document.getElementsByTagName(type);
    for (var i = 0, m = elems.length; i < m; i++) {
        if (elems[i].id && elems[i].id.indexOf(elementId) != -1) {
            return elems[i];
        }
    }
    return null;
}

function setIPMaxControl(enable) {
    var inputEl = null;
    var elems = document.getElementsByTagName("input");
    for (var i = 0, m = elems.length; i < m; i++) {
        if (elems[i].id && elems[i].id.indexOf("_frmIPs1_txtIpMax4") != -1) {
            inputEl = elems[i];
            break;
        }
    }
    if (inputEl != null) {
        if (enable) {
            Ext.getCmp(inputEl.getAttribute("ConvertedId")).enable();
        } else {
            Ext.getCmp(inputEl.getAttribute("ConvertedId")).disable();
        }
    }
}

function showHideRange() {
    var inputEl = null;
    var elems = document.getElementsByTagName("input");
    for (var i = 0, m = elems.length; i < m; i++) {
        if (elems[i].id && elems[i].id.indexOf("_frmIPs1_txtIpMax4") != -1) {
            inputEl = elems[i];
            break;
        }
    }
    if (inputEl != null) {
        if (showRange) {
            showRange = false;
            Ext.getCmp(inputEl.getAttribute("ConvertedId")).disable();
        } else {
            showRange = true;
            Ext.getCmp(inputEl.getAttribute("ConvertedId")).enable();
        }
    }
}

function showWndIPs(bol) {
    try {
        var divC = null;
        var elems = document.getElementsByTagName("div");
        for (var i = 0, m = elems.length; i < m; i++) {
            if (elems[i].id && elems[i].id.indexOf("_frmIPs1_frm") != -1) {
                divC = elems[i];
                break;
            }
        }

        if (divC != null) {
            if (bol == true) {
                divC.style.display = '';
                divC.style.marginLeft = ((divC.offsetWidth / 2) * -1) + "px";
                divC.style.marginTop = ((divC.offsetHeight / 2) * -1) + "px";
            }
            else {
                divC.style.display = 'none';
            }
        }
    }
    catch (e) {
        showError("showWndIPs", e);
    }
}

/* funcio per bloquejar sols l'area menu */
function disableScreen(bol) {
    var divBg = document.getElementById('divModalBgDisabled');
    if (divBg != null) {
        if (bol == true) {
            document.body.style.overflow = "hidden";
            divBg.style.height = 2000;  //document.body.offsetHeight;
            divBg.style.width = 3000;  //document.body.offsetWidth;

            divBg.style.display = '';
        }
        else {
            document.body.style.overflow = "";
            divBg.style.display = 'none';
        }
    }
}

function CancelAllowIP() {
    disableScreen(false);
    showWndIPs(false);
}

function SaveAllowedIP() {
    try {
        if (validateAllowIp()) {
            var grid = jsGridIPs;//oDocumentAbsence.getGridDocumentAdvice();
            var value = "";

            var idRow = "-1";

            var inputEl = null;
            var elems = document.getElementsByTagName("input");
            for (var i = 0, m = elems.length; i < m; i++) {
                if (elems[i].id && elems[i].id.indexOf("_frmIPs1_hdnIpID") != -1) {
                    inputEl = elems[i];
                    break;
                }
            }
            idRow = inputEl.value;

            for (var i = 0, m = elems.length; i < m; i++) {
                if (elems[i].id && elems[i].id.indexOf("_frmIPs1_txtIpMin1") != -1) {
                    inputEl = elems[i];
                    break;
                }
            }
            value = value + inputEl.value + ".";

            for (var i = 0, m = elems.length; i < m; i++) {
                if (elems[i].id && elems[i].id.indexOf("_frmIPs1_txtIpMin2") != -1) {
                    inputEl = elems[i];
                    break;
                }
            }
            value = value + inputEl.value + ".";

            for (var i = 0, m = elems.length; i < m; i++) {
                if (elems[i].id && elems[i].id.indexOf("_frmIPs1_txtIpMin3") != -1) {
                    inputEl = elems[i];
                    break;
                }
            }
            value = value + inputEl.value + ".";

            for (var i = 0, m = elems.length; i < m; i++) {
                if (elems[i].id && elems[i].id.indexOf("_frmIPs1_txtIpMin4") != -1) {
                    inputEl = elems[i];
                    break;
                }
            }
            value = value + inputEl.value;

            for (var i = 0, m = elems.length; i < m; i++) {
                if (elems[i].id && elems[i].id.indexOf("_frmIPs1_ckInputRange") != -1) {
                    inputEl = elems[i];
                    break;
                }
            }

            if (inputEl.checked == true) {
                for (var i = 0, m = elems.length; i < m; i++) {
                    if (elems[i].id && elems[i].id.indexOf("_frmIPs1_txtIpMax4") != -1) {
                        inputEl = elems[i];
                        break;
                    }
                }

                value = value + ":" + inputEl.value;
            }

            var arrValues = [{ field: 'id', value: idRow },
            { field: 'value', value: value }];

            if (idRow == "-1") {
                grid.createRow(arrValues, null);
            }
            else {
                grid.deleteRow(idRow);
                grid.createRow(arrValues, null);
            }

            hasChanges(true);

            //Tanquem finestra
            disableScreen(false);
            showWndIPs(false);
        }
    }
    catch (e) {
        showError("SaveAllowedIP", e);
    }
}

function validateAllowIp() {
    try {
        var ipmin1 = findElementById("input", "_frmIPs1_txtIpMin1").value;
        var ipmin2 = findElementById("input", "_frmIPs1_txtIpMin2").value;
        var ipmin3 = findElementById("input", "_frmIPs1_txtIpMin3").value;
        var ipmin4 = findElementById("input", "_frmIPs1_txtIpMin4").value;
        var ipmax4 = findElementById("input", "_frmIPs1_txtIpMax4").value;
        var isRange = findElementById("input", "_frmIPs1_ckInputRange").checked;

        var fullIp = "";
        var notValidIP = false;

        var fullIp = ipmin1 + "." + ipmin2 + "." + ipmin3 + "." + ipmin4;

        if (isRange) {
            fullIp = fullIp + ":" + ipmax4;
        }

        if (ipmin1 == "" || (ipmin1 != "" && isNaN(ipmin1))) {
            notValidIP = true;
        }

        if (notValidIP == false && (ipmin2 == "" || ipmin2 != "" && isNaN(ipmin2))) {
            notValidIP = true;
        }

        if (notValidIP == false && (ipmin3 == "" || ipmin3 != "" && isNaN(ipmin3))) {
            notValidIP = true;
        }

        if (notValidIP == false && (ipmin4 == "" || ipmin4 != "" && isNaN(ipmin4))) {
            notValidIP = true;
        }

        if (notValidIP == false && isRange && (ipmax4 == "" || ipmax4 != "" && isNaN(ipmax4))) {
            notValidIP = true;
        }

        if (notValidIP) {
            showErrorPopup("Error.Title", "error", "Error.NotValidIp", "Error.OK", "Error.OKDesc", "");
            return false;
        }

        if (checkIfExistsIpInGrid(fullIp) == true) {
            showErrorPopup("Error.Title", "error", "Error.Description.DuplicatedIpRange", "Error.OK", "Error.OKDesc", "");
            return false;
        }

        return true;
    }
    catch (e) {
        showError("validateAllowIp", e); return false;
    }
}

function checkIfExistsIpInGrid(fullIp) {
    try {
        var grid = jsGridIPs;
        if (grid != null) {
            var ipRows = jsGridIPs.getRows();

            for (var x = 0; x < ipRows.length; x++) {
                if (jsGridIPs.retRowJSON(ipRows[x].id)[1].value == fullIp) return true;
            }
            return false;
        }
        else {
            return false;
        }
    }
    catch (e) {
        showError("checkIfExistsIpInGrid", e);
    }
}

function showErrorPopup(Title, typeIcon, Description, Opt1Text, Opt1Desc, strScript1, Opt2Text, Opt2Desc, strScript2, Opt3Text, Opt3Desc, strScript3) {
    try {
        var url = "Security/srvMsgBoxSecurity.aspx?action=Message";
        url = url + "&TitleKey=" + Title;
        url = url + "&DescriptionKey=" + Description;
        url = url + "&Option1TextKey=" + Opt1Text;
        url = url + "&Option1DescriptionKey=" + Opt1Desc;
        url = url + "&Option1OnClickScript=HideMsgBoxForm();" + strScript1 + "; return false;";
        if (Opt2Text != null) {
            url = url + "&Option2TextKey=" + Opt2Text;
            url = url + "&Option2DescriptionKey=" + Opt2Desc;
            url = url + "&Option2OnClickScript=HideMsgBoxForm();" + strScript2 + "; return false;";
        }
        if (Opt3Text != null) {
            url = url + "&Option3TextKey=" + Opt3Text;
            url = url + "&Option3DescriptionKey=" + Opt3Desc;
            url = url + "&Option3OnClickScript=HideMsgBoxForm();" + strScript3 + "; return false;";
        }
        if (typeIcon.toUpperCase() == "TRASH") {
            url = url + "&IconUrl=~/Base/Images/MessageFrame/trash64.png";
        } else if (typeIcon.toUpperCase() == "ERROR") {
            url = url + "&IconUrl=~/Base/Images/MessageFrame/alert32.png";
        } else if (typeIcon.toUpperCase() == "INFO") {
            url = url + "&IconUrl=~/Base/Images/MessageFrame/dialog-information.png";
        }

        parent.ShowMsgBoxForm(url, 400, 300, '');
    } catch (e) { showError("showErrorPopup", e); }
}

function editGridAllowedIPs(sId) {
    try {
        var grid = jsGridIPs;
        if (grid != null) {
            //obtener la fila del grid
            var tmpRow = grid.retRowJSON(sId);

            var objDocument = new Object();
            objDocument.idRow = sId;

            var startIp = "";
            var endIp = "";

            if (tmpRow[1].value.indexOf(':') != -1) {
                startIp = tmpRow[1].value.split(':')[0].split('.');
                endIp = tmpRow[1].value.split(':')[1];
            } else {
                startIp = tmpRow[1].value.split('.');
                endIp = "";
            }

            objDocument.txtMin1 = startIp[0];
            objDocument.txtMin2 = startIp[1];
            objDocument.txtMin3 = startIp[2];
            objDocument.txtMin4 = startIp[3];

            if (endIp != "") {
                objDocument.txtMax4 = endIp;
                objDocument.checked = true;
            } else {
                objDocument.txtMax4 = "";
                objDocument.checked = false;
            }

            EditAllowedIP(false, objDocument);
        }
    }
    catch (e) {
        showError("editGridDocumentAdviceFieldsList", e);
    }
}

function deleteGridAllowedIPs(sId) {
    var grid = jsGridIPs;
    if (grid != null) {
        //borrar la fila del grid
        grid.deleteRow(sId);

        hasChanges(true);
    }
}

function ChangeRestrictedIP() {
    var gridIPS = findElementById("div", "_gridAllowedIPs");

    var check = findElementById("input", "_ChkRestrictedIP_chkButton").checked;

    if (check) {
        gridIPS.style.display = '';
    } else {
        gridIPS.style.display = 'none';
    }
}