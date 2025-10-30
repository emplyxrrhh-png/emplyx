Date.prototype.toJSON = function (key) {

    var sendDate = new Date(this.getTime());

    sendDate.setHours(sendDate.getHours() + (sendDate.getTimezoneOffset() / -60))

    function f(n) {
        // Format integers to have at least two digits.
        return n < 10 ? '0' + n : n;
    }

    return sendDate.getUTCFullYear() + '-' +
         f(sendDate.getUTCMonth() + 1) + '-' +
         f(sendDate.getUTCDate()) + 'T' +
         f(sendDate.getUTCHours()) + ':' +
         f(sendDate.getUTCMinutes()) + ':' +
         f(sendDate.getUTCSeconds()) + 'Z';

};

function roDateReviver(key, value) {
    var a;
    if (typeof value === 'string') {
        //a = /^(\d{4})-(\d{2})-(\d{2})T(\d{2}):(\d{2}):(\d{2}(?:\.\d*)?)Z$/.exec(value);
        //if (a) {
        //    return new Date(Date.UTC(+a[1], +a[2] - 1, +a[3], +a[4],
        //                    +a[5], +a[6]));
        //}
        var dateISO = /\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}(?:[.,]\d+)?Z/i;
        var dateISOUTC = /\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}/i;
        var dateTimezone = /\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}([+-])\d{2}:\d{2}/i;

        if (dateISO.test(value)) {
            var tmp = new Date(value);

            var offset = tmp.getTimezoneOffset() / 60;
            var hours = tmp.getHours();
            tmp.setHours(hours + offset);

            return tmp;
        }

        if (dateTimezone.test(value)) {
            var a = /^(\d{4})-(\d{2})-(\d{2})T(\d{2}):(\d{2}):(\d{2})[+-](\d{2}):(\d{2})$/.exec(value);
            var tmp = new Date(+a[1], +a[2] - 1, +a[3], +a[4], a[5], +a[6]);
            return tmp;
        }

        if (dateISOUTC.test(value)) {
            var a = /^(\d{4})-(\d{2})-(\d{2})T(\d{2}):(\d{2}):(\d{2}?)$/.exec(value);
            var tmp = new Date(+a[1], +a[2] - 1, +a[3], +a[4], a[5], +a[6]);
            return tmp;
        }
    }
    return value;
};

Date.prototype.format2Time = function () {
    var curDate = this;
    var returnStr = "";

    if (curDate.getHours() < 10) {
        returnStr = "0" + curDate.getHours();
    } else {
        returnStr = curDate.getHours();
    }

    if (curDate.getMinutes() < 10) {
        returnStr += ":0" + curDate.getMinutes();
    } else {
        returnStr += ":" + curDate.getMinutes();
    }

    return returnStr;
}

Date.prototype.ToStringDate = function (strFormat, strSeparator) {
    try {

        var arrFormat = strFormat.split(strSeparator);
        var m = this.getMonth() + 1;
        var d = this.getDate();
        var y = this.getFullYear();

        if (m < 10) m = "0" + m;
        if (d < 10) d = "0" + d;

        //D-M-Y
        if (arrFormat[0].substring(0, 1).toUpperCase() == "D" && arrFormat[1].substring(0, 1).toUpperCase() == "M" && arrFormat[2].substring(0, 1).toUpperCase() == "Y") {
            return [d, m, y].join('/');
        }
        else {
            //M-D-Y
            if (arrFormat[0].substring(0, 1).toUpperCase() == "M" && arrFormat[1].substring(0, 1).toUpperCase() == "D" && arrFormat[2].substring(0, 1).toUpperCase() == "Y") {
                return [m, d, y].join('/');
            }
            else {
                //Y-M-D
                if (arrFormat[0].substring(0, 1).toUpperCase() == "Y" && arrFormat[1].substring(0, 1).toUpperCase() == "M" && arrFormat[2].substring(0, 1).toUpperCase() == "D") {
                    return [y, m, d].join('/');
                }
                else {
                    return "";
                }
            }
        }
    }
    catch (e) { return ""; }
}

Date.prototype.FromStringDate = function (strDate, strFormat, strSeparator) {
    try {
        var arrFormat = strFormat.split(strSeparator);
        var arrDate = strDate.split(strSeparator);

        //D-M-Y
        if (arrFormat[0].substring(0, 1).toUpperCase() == "D" && arrFormat[1].substring(0, 1).toUpperCase() == "M" && arrFormat[2].substring(0, 1).toUpperCase() == "Y") {
            this.setFullYear(arrDate[2], parseInt(arrDate[1]) - 1, arrDate[0]);
        }
        else  {
            //M-D-Y
            if (arrFormat[0].substring(0, 1).toUpperCase() == "M" && arrFormat[1].substring(0, 1).toUpperCase() == "D" && arrFormat[2].substring(0, 1).toUpperCase() == "Y") {
                this.setFullYear(arrDate[2], parseInt(arrDate[0]) - 1, arrDate[1]);
            }
            else {
                //Y-M-D
                if (arrFormat[0].substring(0, 1).toUpperCase() == "Y" && arrFormat[1].substring(0, 1).toUpperCase() == "M" && arrFormat[2].substring(0, 1).toUpperCase() == "D") {
                    this.setFullYear(arrDate[0], parseInt(arrDate[1]) - 1, arrDate[2]);
                }
                else {
                }
            }
        }
    }
    catch (e) { }
}

String.prototype.filterTimeFormat = function () {
    var result = 0;

    // Number of decimal places to round to
    var decimal_places = 2;
 
    // Maximum number of hours before we should assume minutes were intended. Set to 0 to remove the maximum.
    var maximum_hours = 59;
 
    // 3
    var int_format = this.match(/^\d+$/);
 
    // 1:15
    var time_format = this.match(/([\d]*):([\d]+)/);
 
    // 10m
    var minute_string_format = this.toLowerCase().match(/([\d]+)m/);
 
    // 2h
    var hour_string_format = this.toLowerCase().match(/([\d]+)h/);
 
    if (time_format != null) {
        hours = parseInt(time_format[1],10);
        minutes = parseFloat(time_format[2]/60);
        result = hours + minutes;
    } else if (minute_string_format != null || hour_string_format != null) {
        if (hour_string_format != null) {
            hours = parseInt(hour_string_format[1],10);
        } else {
            hours = 0;
        }
        if (minute_string_format != null) {
            minutes = parseFloat(minute_string_format[1]/60);
        } else {
            minutes = 0;
        }
        result = hours + minutes;
    } else if (int_format != null) {
        // Entries over 15 hours are likely intended to be minutes.
        result = parseInt(this,10);
        if (maximum_hours > 0 && result > maximum_hours) {
            result = (result / 60).toFixed(decimal_places);
        }
    }
 
    // make sure what ever we return is a 2 digit float
    result = parseFloat(result).toFixed(decimal_places);
 
    return result;
}

Number.prototype.HoursToHHMMSS = function (includeSeconds) {
    var sec_num = this * 60 * 60; // don't forget the second param
    var hours = Math.floor(sec_num / 3600);
    var minutes = Math.floor((sec_num - (hours * 3600)) / 60);
    var seconds = sec_num - (hours * 3600) - (minutes * 60);

    if (hours < 10) { hours = "0" + hours; }
    if (minutes < 10) { minutes = "0" + minutes; }
    if (seconds < 10) { seconds = "0" + seconds; }
    var time = hours + ':' + minutes;

    if (includeSeconds) time = time + ':' + seconds;

    return time;
}

String.prototype.endsWith = function (suffix) {
    return this.indexOf(suffix, this.length - suffix.length) !== -1;
};

String.prototype.startsWith = function (prefix) {
    return this.indexOf(prefix, 0) == 0;
};

String.prototype.replaceAll = function (search, replacement) {
    var target = this;
    return target.replace(new RegExp(search, 'g'), replacement);
};

String.prototype.decodeEntities = function(){
    var str;
    var temp = document.createElement('p');
    temp.innerHTML = this;
    str= temp.textContent || temp.innerText;
    temp=null;
    return str;
}

String.prototype.count = function (s1) {
    return (this.length - this.replace(new RegExp(s1, "g"), '').length) / s1.length;
}

String.prototype.lpad = function (padString, length) {
    var str = this;
    while (str.length < length)
        str = padString + str;
    return str;
}

function MaximizeMinimizeContent(sender, treeContainer, contentContainer, minimizedText, maximizedText, evalFunc) {
    var tree = $('#' + treeContainer);
    var contenido = $('#' + contentContainer);
    var senderObj = $('#' + sender)
    var buttonObj = senderObj.next();
    if (tree.css('display') == 'none') {
        tree.css('display', '');
        contenido.css('width', 'calc(100% - 330px)');
        buttonObj.addClass('btnMaximize2');
        buttonObj.removeClass('btnMinimize2');
        senderObj.find('.VerticalToolTip').each(function () { $(this).text(minimizedText) });
    } else {
        tree.css('display', 'none');
        contenido.css('width', 'calc(100% - 40px)');
        buttonObj.addClass('btnMinimize2');
        buttonObj.removeClass('btnMaximize2');
        senderObj.find('.VerticalToolTip').each(function () { $(this).text(maximizedText) });
    }

    if (typeof (evalFunc) != 'undefined' && evalFunc != "") eval(evalFunc);
}

function resizeHeaders() {
    var tree = $('#divTree');
    var headers = $('#fixedHeaderColumns');
    if (tree.css('display') == 'none') {
        headers.css('width', (parseInt(headers.css('width')) + 290) + "px");
    } else {
        headers.css('width', (parseInt(headers.css('width')) - 290) + "px");
    }
}
