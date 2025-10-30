// used by dateAdd, dateDiff, datePart, weekdayName, and monthName
// note: less strict than VBScript's isDate, since JS allows invalid dates to overflow (e.g. Jan 32 transparently becomes Feb 1)
function isDate(p_Expression){
	return !isNaN(new Date(p_Expression));		// <<--- this needs checking
}

// ******************************************************************
// This function accepts a string variable and verifies if it is a
// proper date or not. It validates format matching either
// mm-dd-yyyy or mm/dd/yyyy. Then it checks to make sure the month
// has the proper number of days, based on which month it is.

// The function returns true if a valid date, false if not.
// ******************************************************************

function isDateEx(dateStr) {

    var datePat = /^(\d{1,2})(\/|-)(\d{1,2})(\/|-)(\d{4})$/;
    var matchArray = dateStr.match(datePat); // is the format ok?

    if (matchArray == null) {
        //alert("Please enter date as either mm/dd/yyyy or mm-dd-yyyy.");
        return false;
    }

    month = matchArray[1]; // p@rse date into variables
    day = matchArray[3];
    year = matchArray[5];

    if (month < 1 || month > 12) { // check month range
        //alert("Month must be between 1 and 12.");
        return false;
    }

    if (day < 1 || day > 31) {
        //alert("Day must be between 1 and 31.");
        return false;
    }

    if ((month == 4 || month == 6 || month == 9 || month == 11) && day == 31) {
        //alert("Month " + month + " doesn`t have 31 days!")
        return false;
    }

    if (month == 2) { // check for february 29th
        var isleap = (year % 4 == 0 && (year % 100 != 0 || year % 400 == 0));
        if (day > 29 || (day == 29 && !isleap)) {
            //alert("February " + year + " doesn`t have " + day + " days!");
            return false;
        }
    }
    return true; // date is valid
}


// REQUIRES: isDate()
function dateAdd(p_Interval, p_Number, p_Date){
	if(!isDate(p_Date)){return "invalid date: '" + p_Date + "'";}
	if(isNaN(p_Number)){return "invalid number: '" + p_Number + "'";}	

	p_Number = new Number(p_Number);
	var dt = new Date(p_Date);
	switch(p_Interval.toLowerCase()){
		case "yyyy": {// year
			dt.setFullYear(dt.getFullYear() + p_Number);
			break;
		}
		case "q": {		// quarter
			dt.setMonth(dt.getMonth() + (p_Number*3));
			break;
		}
		case "m": {		// month
			dt.setMonth(dt.getMonth() + p_Number);
			break;
		}
		case "y":		// day of year
		case "d":		// day
		case "w": {		// weekday
			dt.setDate(dt.getDate() + p_Number);
			break;
		}
		case "ww": {	// week of year
			dt.setDate(dt.getDate() + (p_Number*7));
			break;
		}
		case "h": {		// hour
			dt.setHours(dt.getHours() + p_Number);
			break;
		}
		case "n": {		// minute
			dt.setMinutes(dt.getMinutes() + p_Number);
			break;
		}
		case "s": {		// second
			dt.setSeconds(dt.getSeconds() + p_Number);
			break;
		}
		case "ms": {		// second
			dt.setMilliseconds(dt.getMilliseconds() + p_Number);
			break;
		}
		default: {
			return "invalid interval: '" + p_Interval + "'";
		}
	}
	return dt;
}

// REQUIRES: isDate()
// NOT SUPPORTED: firstdayofweek and firstweekofyear (defaults for both)
function dateDiff(p_Interval, p_Date1, p_Date2, p_firstdayofweek, p_firstweekofyear){
	if(!isDate(p_Date1)){return "invalid date: '" + p_Date1 + "'";}
	if(!isDate(p_Date2)){return "invalid date: '" + p_Date2 + "'";}
	var dt1 = new Date(p_Date1);
	var dt2 = new Date(p_Date2);

	// get ms between dates (UTC) and make into "difference" date
	var iDiffMS = dt2.valueOf() - dt1.valueOf();
	var dtDiff = new Date(iDiffMS);

	// calc various diffs
	var nYears  = dt2.getUTCFullYear() - dt1.getUTCFullYear();
	var nMonths = dt2.getUTCMonth() - dt1.getUTCMonth() + (nYears!=0 ? nYears*12 : 0);
	var nQuarters = parseInt(nMonths/3);	//<<-- different than VBScript, which watches rollover not completion
	
	var nMilliseconds = iDiffMS;
	var nSeconds = parseInt(iDiffMS/1000);
	var nMinutes = parseInt(nSeconds/60);
	var nHours = parseInt(nMinutes/60);
	var nDays  = parseInt(nHours/24);
	var nWeeks = parseInt(nDays/7);


	// return requested difference
	var iDiff = 0;		
	switch(p_Interval.toLowerCase()){
		case "yyyy": return nYears;
		case "q": return nQuarters;
		case "m": return nMonths;
		case "y": 		// day of year
		case "d": return nDays;
		case "w": return nDays;
		case "ww":return nWeeks;		// week of year	// <-- inaccurate, WW should count calendar weeks (# of sundays) between
		case "h": return nHours;
		case "n": return nMinutes;
		case "s": return nSeconds;
		case "ms":return nMilliseconds;	// millisecond	// <-- extension for JS, NOT available in VBScript
		default: return "invalid interval: '" + p_Interval + "'";
	}
}

// REQUIRES: isDate(), dateDiff()
// NOT SUPPORTED: firstdayofweek and firstweekofyear (does system default for both)
function datePart(p_Interval, p_Date, p_firstdayofweek, p_firstweekofyear){
	if(!isDate(p_Date)){return "invalid date: '" + p_Date + "'";}

	var dtPart = new Date(p_Date);
	switch(p_Interval.toLowerCase()){
		case "yyyy": return dtPart.getFullYear();
		case "q": return parseInt(dtPart.getMonth()/3)+1;
		case "m": return dtPart.getMonth()+1;
		case "y": return dateDiff("y", "1/1/" + dtPart.getFullYear(), dtPart);			// day of year
		case "d": return dtPart.getDate();
		case "w": return dtPart.getDay();	// weekday
		case "ww":return dateDiff("ww", "1/1/" + dtPart.getFullYear(), dtPart);		// week of year
		case "h": return dtPart.getHours();
		case "n": return dtPart.getMinutes();
		case "s": return dtPart.getSeconds();
		case "ms":return dtPart.getMilliseconds();	// millisecond	// <-- extension for JS, NOT available in VBScript
		default: return "invalid interval: '" + p_Interval + "'";
	}
}

// REQUIRES: isDate()
// NOT SUPPORTED: firstdayofweek (does system default)
function weekdayName(p_Date, p_abbreviate){
	if(!isDate(p_Date)){return "invalid date: '" + p_Date + "'";}
	var dt = new Date(p_Date);
	var retVal = dt.toString().split(' ')[0];
	var retVal = Array('Sunday','Monday','Tuesday','Wednesday','Thursday','Friday','Saturday')[dt.getDay()];
	if(p_abbreviate==true){retVal = retVal.substring(0, 3)}	// abbr to 1st 3 chars
	return retVal;
}
// REQUIRES: isDate()
function monthName(p_Date, p_abbreviate){
	if(!isDate(p_Date)){return "invalid date: '" + p_Date + "'";}
	var dt = new Date(p_Date);	
	var retVal = Array('January','February','March','April','May','June','July','August','September','October','November','December')[dt.getMonth()];
	if(p_abbreviate==true){retVal = retVal.substring(0, 3)}	// abbr to 1st 3 chars
	return retVal;
}

// ====================================

// bootstrap different capitalizations
function IsDate(p_Expression){
	return isDate(p_Expression);
}
function DateAdd(p_Interval, p_Number, p_Date){
	return dateAdd(p_Interval, p_Number, p_Date);
}
function DateDiff(p_interval, p_date1, p_date2, p_firstdayofweek, p_firstweekofyear){
	return dateDiff(p_interval, p_date1, p_date2, p_firstdayofweek, p_firstweekofyear);
}
function DatePart(p_Interval, p_Date, p_firstdayofweek, p_firstweekofyear){
	return datePart(p_Interval, p_Date, p_firstdayofweek, p_firstweekofyear);
}
function WeekdayName(p_Date){
	return weekdayName(p_Date);
}
function MonthName(p_Date){
	return monthName(p_Date);
}

function formatDate(dateValue, format) {
    var fmt = format.toUpperCase();
    var re = /^(M|MM|D|DD|YYYY)([\-\/]{1})(M|MM|D|DD|YYYY)(\2)(M|MM|D|DD|YYYY)$/;
    if (!re.test(fmt)) { fmt = "MM/DD/YYYY"; }
    if (fmt.indexOf("M") == -1) { fmt = "MM/DD/YYYY"; }
    if (fmt.indexOf("D") == -1) { fmt = "MM/DD/YYYY"; }
    if (fmt.indexOf("YYYY") == -1) { fmt = "MM/DD/YYYY"; }

    var M = "" + (dateValue.getMonth() + 1);
    var MM = "0" + M;
    MM = MM.substring(MM.length - 2, MM.length);
    var D = "" + (dateValue.getDate());
    var DD = "0" + D;
    DD = DD.substring(DD.length - 2, DD.length);
    var YYYY = "" + (dateValue.getFullYear());

    var sep = "/";
    if (fmt.indexOf("-") != -1) { sep = "-"; }
    var pieces = fmt.split(sep);
    var result = "";

    switch (pieces[0]) {
        case "M": result += M + sep; break;
        case "MM": result += MM + sep; break;
        case "D": result += D + sep; break;
        case "DD": result += DD + sep; break;
        case "YYYY": result += YYYY + sep; break;
    }

    switch (pieces[1]) {
        case "M": result += M + sep; break;
        case "MM": result += MM + sep; break;
        case "D": result += D + sep; break;
        case "DD": result += DD + sep; break;
        case "YYYY": result += YYYY + sep; break;
    }
    
    switch (pieces[2]) {
        case "M": result += M; break;
        case "MM": result += MM; break;
        case "D": result += D; break;
        case "DD": result += DD; break;
        case "YYYY": result += YYYY; break;
    } 
 
     return result;

 }


 //Recupera el format de la data pasat desde PageBase (format dd/mm/yyyy, etc.)
 function jsDate_retDateFormat() {
     try {
         var strDate = "";
         var fmtDate = document.getElementById('hdnPageBaseDateFormat');
         if (frmDate != null) {
             strDate = fmtDate.value;
         }
         return strDate;
     } catch (e) { showError("jsDate:retDateFormat", e); }
 }
 
 
//Recupera el format de la data pasat desde PageBase (format numeros)
 function jsDate_retDateFormatType() {
     try {
         var strDate = "";
         var fmtDate = document.getElementById('hdnPageBaseDateFormatType');
         if (fmtDate != null) {
             strDate = fmtDate.value;
         }
         return strDate;
     } catch (e) { showError("jsDate:retDateFormatType", e); }
 }

//Funcio generica que retorna una data d'una cadena
 function jsDate_retDate(sDate) {
     try {
         var retDate = null;
         
         var aDate = new Array();
         aDate = sDate.split('/');

         var rFormat = new String();
         rFormat = jsDate_retDateFormatType();
         
         var pos1 = rFormat.substring(0, 1);
         var pos2 = rFormat.substring(1, 2);
         var pos3 = rFormat.substring(2, 3);

         if (pos1 == "1" && pos2 == "0" && pos3 == "2") {
             var retDate = new Date(parseInt(aDate[2]), parseInt(aDate[0]) - 1, parseInt(aDate[1]), 0, 0, 0);
         } else if (pos1 == "0" && pos2 == "1" && pos3 == "2") {
             var retDate = new Date(parseInt(aDate[2]), parseInt(aDate[1]) - 1, parseInt(aDate[0]), 0, 0, 0);
         }

         return retDate;
     } catch (e) { showError("jsDate:retDate", e); }
 }

// Comprovació si la hora esta ben expresada (hh:mm)
 function jsDate_checkTime(sTime) {
     try {
         var arrHour = new Array();

         var iHour = 0;
         var iMinute = 0;

         arrHour = sTime.toString().split(":");

         if(arrHour.length != 2){
            return false;
         }
         
         iHour = parseInt(arrHour[0]);
         iMinute = parseInt(arrHour[1]);

         if (iHour > 23) {
             return false;
         }

         if (iMinute > 59) {
             return false;
         }
         
         return true;
         
     } catch (e) { showError("jsDate:checkTime", e); return false; }
 }

 // Comprovació si la hora esta ben expresada (hhhh:mm)
 function jsDate_checkTimeLong(sTime) {
     try {
         var arrHour = new Array();

         var iHour = 0;
         var iMinute = 0;

         arrHour = sTime.toString().split(":");

         if (arrHour.length > 4) {
             return false;
         }

         iHour = parseInt(arrHour[0]);
         iMinute = parseInt(arrHour[1]);

         if (iHour > 9999) {
             return false;
         }

         if (iMinute > 59) {
             return false;
         }

         return true;

     } catch (e) { showError("jsDate:checkTimeLong", e); return false; }
 }

//Recupera els minuts de la hora
 function jsDate_retMinutesToTime(sTime, noCheckTime) {
     try {
         if (noCheckTime == null || noCheckTime == false) {
             if (!jsDate_checkTime(sTime)) { return null; }
         }
                
         var intRet = 0;
         var arrHour = new Array();
         
         var iHour = 0;
         var iMinute = 0;

         arrHour = sTime.toString().split(":");
         
         iHour = parseInt(arrHour[0]);
         iMinute = parseInt(arrHour[1]);         
         
         intRet = (iHour * 60) + iMinute;
         
         return intRet;
     } catch (e) { showError("jsDate:retMinutesToHour", e); return null; }
 }

 //Comprova que entre dues dates formin un periode valid
 function jsDate_checkPeriods(sTimeBegin,sTimeEnd) {
     try {
         if (!jsDate_checkTime(sTimeBegin)) { return false; }
         if (!jsDate_checkTime(sTimeEnd)) { return false; }

         var intBegin = 0;
         var intEnd = 0;

         intBegin = jsDate_retMinutesToTime(sTimeBegin,true);
         intEnd = jsDate_retMinutesToTime(sTimeEnd,true);

         if (intBegin < intEnd) {
             return true;
         } else {
             return false;
         }
     } catch (e) { showError("jsDate:checkPeriods", e); return false; }
 }