function navigateToPortal() {
    var path = window.location.href.substr(0, window.location.href.indexOf('index.aspx'));
    localStorage.setItem('VersionEnabled', '2');
    document.location.href = path + '2/indexv2.aspx';
}