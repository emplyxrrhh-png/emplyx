// Provides functions to create, read or delete cookies.

function createCookie(name, value, days, bolCookieExpire) {
    //if (name.startsWith("TreeState_")) {
    //    if (value != null) {
    //        let serverRes = false;
    //        setServerCookie(name, value).then(result => (serverRes = result));
    //    }
    //} else {
        if (value != null) {
            if (value.toString().indexOf(";") > -1) { alert("semicolon in value: " + value); }

            //PPR  filtrar caracteres '=' porque son incompatibles!!!!!
            var sAux = ''
            sAux = value.toString();
            sAux = sAux.replace(/=/gi, '..EQUAL..');

            var expires = -1;
            if (typeof (days) != 'undefined') {
                expires = (days * 24 * 60 * 60 * 1000);
            } else {
                if (name.indexOf('roTrees1') >= 0 || name.indexOf('Budget') >= 0 || name == 'CalendarType' || name == 'PlanView' || name == 'SchedulerIntervalDates' || name == 'CalendarLoadRecursive') expires = (30 * 24 * 60 * 60 * 1000);
                else expires = (10 * 60 * 1000);
            }

            if (expires > 0) Set_Cookie(name, sAux, expires, '/', '', '');
            else Set_Cookie(name, sAux, '', '/', '', '');

            return;
        }

        var expires = "";
        if (typeof (days) != 'undefined') {
            var date = new Date();
            date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
            expires = "; expires=" + date.toGMTString();
        } else {
            var date = new Date();
            if (typeof (bolCookieExpire) != 'undefined') {
                date.setTime(date.getTime() + (60 * 60 * 1000));
                expires = "; expires=" + date.toGMTString();
            }
        }

        var cookieValue = name + "=" + value + expires + "; path=/;samesite=strict;";

        cookieValue = cookieValue + (document.location.protocol == "http:" ? "" : ";secure");

        document.cookie = cookieValue;
    //}
}

function createCookie2(name,value,days) {    
	var caduca = new Date(); 
	caduca.setTime(caduca.getTime() + (days*24*60*60*1000));	
	setCookie (name, value, caduca,"/",true);
}

function readCookie(name, defaultvalue) {
    //if (name.startsWith("TreeState_")) {
    //    var sCookie = null;
    //    if (name != null) {
    //        getServerCookie(name).then(result => (sCookie = result));
    //    }
    //    if (sCookie != null) return sCookie.Value;
    //    else return defaultvalue;
    //} else {
        var retValue = Get_Cookie(name);
        if (retValue == null) retValue = defaultvalue;
        return retValue;

        var nameEQ = name + "=";
        var ca = document.cookie.split(';');
        for (var i = 0; i < ca.length; i++) {
            var c = ca[i];
            while (c.charAt(0) == ' ') c = c.substring(1, c.length);
            if (c.indexOf(nameEQ) == 0) {
                return c.substring(nameEQ.length, c.length);
            }
        }
        return defaultvalue;
    //}
}

function eraseCookie(name) {
    //if (name.startsWith("TreeState_")) {
    //    if (value != null) {
    //        let serverRes = false;
    //        deleteServerCookie(name).then(result => (serverRes = result));
    //    }
    //} else {
        Delete_Cookie(name, '/', '');
        //createCookie(name,"",-1);
    //}
}

function Set_Cookie(name, value, expires, path, secure, domain) {
    // set time, it's in milliseconds
    var today = new Date();
    today.setTime(today.getTime());

    if (expires == '') expires = 0;
    var expires_date = new Date(today.getTime() + (expires));

    var cookieValue = name + "=" + escape(value) +
    ((expires) ? ";expires=" + expires_date.toGMTString() : "") +
    ((path) ? ";path=" + path : "") +
    ((domain) ? ";domain=" + domain : "") +
    ";samesite=strict";

    cookieValue = cookieValue + (document.location.protocol == "http:" ? "" : ";secure");

    document.cookie = cookieValue
}

// this fixes an issue with the old method, ambiguous values
// with this test document.cookie.indexOf( name + "=" );
function Get_Cookie(check_name) {
    // first we'll split this cookie up into name/value pairs
    // note: document.cookie only returns name=value, not the other components
    var a_all_cookies = document.cookie.split(';');
    var a_temp_cookie = '';
    var cookie_name = '';
    var cookie_value = '';
    var b_cookie_found = false; // set boolean t/f default f

    for (i = 0; i < a_all_cookies.length; i++) {
        // now we'll split apart each name=value pair
        a_temp_cookie = a_all_cookies[i].split('=');


        // and trim left/right whitespace while we're at it
        cookie_name = a_temp_cookie[0].replace(/^\s+|\s+$/g, '');

        // if the extracted name matches passed check_name
        if (cookie_name == check_name) {
            b_cookie_found = true;
            // we need to handle case where cookie has no value but exists (no = sign, that is):
            if (a_temp_cookie.length > 1) {
                cookie_value = unescape(a_temp_cookie[1].replace(/^\s+|\s+$/g, ''));
                
                //PPR  Reponer caracteres '=' porque son incompatibles!!!!!
                cookie_value = cookie_value.replace(/..EQUAL../gi, '=');

            }
            // note that in cases where cookie is initialized but no value, null is returned
            return cookie_value;
            break;
        }
        a_temp_cookie = null;
        cookie_name = '';
    }
    if (!b_cookie_found) {
        return null;
    }
}

// this deletes the cookie when called
function Delete_Cookie(name, path, domain) {
    if (Get_Cookie(name)) document.cookie = name + "=" +
((path) ? ";path=" + path : "") +
((domain) ? ";domain=" + domain : "") +
";expires=Thu, 01-Jan-1970 00:00:01 GMT";
}

async function getServerCookie(name) {
    let BASE_URL = readCookie('serverURL', '/./');
    let cValue = '';
    try {
        await $.ajax({
            url: `${BASE_URL}Cookie/GetCookie`,
            data: { 'sCookieName': name },
            type: "POST",
            dataType: "json",
            success: (data) => (cValue = data),
            error: (error) => (cValue = ''),
        });
    } catch (e) {
        //console.log(e);
    }
    

    return cValue;
}

async function setServerCookie(name, value) {
    let BASE_URL = readCookie('serverURL', '/./');
    let cValue = false;
    try {
        await $.ajax({
            url: `${BASE_URL}Cookie/SetCookie`,
            data: { 'sCookieName': name, 'sCookieValue': value },
            type: "POST",
            dataType: "json",
            success: (data) => (cValue = data),
            error: (error) => (cValue = false),
        });
    } catch (e) {
        //console.log(e);
    }
    
    return cValue;
}

async function deleteServerCookie(name) {
    let BASE_URL = readCookie('serverURL', '/./');
    let cValue = false;
    try {
        await $.ajax({
            url: `${BASE_URL}Cookie/EraseCookie`,
            data: { 'sCookieName': name },
            type: "POST",
            dataType: "json",
            success: (data) => (cValue = data),
            error: (error) => (cValue = false),
        });
    } catch (e) {
        //console.log(e);
    }
    

    return cValue;
}