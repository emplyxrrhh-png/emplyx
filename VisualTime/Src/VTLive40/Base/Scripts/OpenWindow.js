// Provides functions to work with popup windows in Internet Explorer or FireFox.

function OpenDialog(url, mWidth, mHeight) {
	// for IE
	if (window.showModalDialog) {
		var dFeatures = 'dialogHeight: ' + mHeight +'px; dialogWidth: ' + mWidth + 'px; center: Yes; help: No; resizable: No; status: No; scroll: No;';
		window.showModalDialog(url, '', dFeatures);
		window.location = window.location;
	}
	// for FF / Mozilla
	else {
		width = mWidth;
		height = mHeight;
		var iLeft = (screen.width  - width) / 2;
		var iTop  = (screen.height - height) / 2;
		var sOptions = "toolbar=no,resizable=false,status=no,dependent=yes,scrollbars=no,z-lock=yes";
		sOptions += ",width=" + width;
		sOptions += ",height=" + height;
		sOptions += ",left=" + iLeft;
		sOptions += ",top=" + iTop;
		myTWin = window.open(url, '', sOptions);
	}
	return false;
}

function OpenPopup(url, mWidth, mHeight, resizable) {
	width = mWidth;
	height = mHeight;
	var iLeft = (screen.width  - width) / 2;
	var iTop  = (screen.height - height) / 2;
	var sOptions = "toolbar=no,status=no,scrollbars=no,resizable=";
	sOptions += (resizable != null && resizable != false) ? "yes" : "no";
	sOptions += ",width=" + width;
	sOptions += ",height=" + height;
	sOptions += ",left=" + iLeft;
	sOptions += ",top=" + iTop;
	myTWin = window.open(url, '', sOptions);
	return false;
}