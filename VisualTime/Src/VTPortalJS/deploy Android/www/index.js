function navigateToPortal() {
    var isMT = localStorage.getItem('IsMT');

    if (typeof (isMT) == 'undefined' || isMT == null) {
        isMT = true;
    } else {
        isMT = localStorage.getItem('IsMT') == 'true' ? true : false;
    }

    var path = window.location.href.substr(0, window.location.href.indexOf('index.html'));

    if (typeof (isMT) == 'undefined' || isMT == null || isMT == true) {
        document.location.href = path + '2/index.html';
    } else {
        document.location.href = path + '1/index.html';
    }
}