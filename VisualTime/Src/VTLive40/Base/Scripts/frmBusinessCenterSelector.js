var selectedNodes = [];
function frmBusinessCenterSelector_Show(values) {
    showLoadingGrid(true);
    var oParameters = {};
    oParameters.action = "INIT";
    oParameters.gridCenters = '';
    var strParameters = JSON.stringify(oParameters);
    strParameters = encodeURIComponent(strParameters);
    ASPxBusinessCentersSelectorCallbackPanelContenidoClient.PerformCallback(strParameters);
}

function LoadCentersGrid(values) {
    lgGridClient.Show();
    var oParameters = {};
    oParameters.action = "LOADGRID";
    oParameters.gridCenters = values.toString();
    var strParameters = JSON.stringify(oParameters);
    strParameters = encodeURIComponent(strParameters);
    ASPxBusinessCentersSelectorCallbackPanelContenidoClient.PerformCallback(strParameters);
}

function SendSelectedCenters() {
    tltBusinessCentersClient.GetSelectedNodeValues("ID", LoadCentersGrid);
}

function DeleteCentersGrid(values) {
    lgGridClient.Show();
    var oParameters = {};
    oParameters.action = "DELETEGRID";
    oParameters.gridCenters = selectedNodes.toString();
    var strParameters = JSON.stringify(oParameters);
    strParameters = encodeURIComponent(strParameters);
    ASPxBusinessCentersSelectorCallbackPanelContenidoClient.PerformCallback(strParameters);
}

function FilterCentersTree() {
    lpSelectorClient.Show();
    var oFilterParameters = {};
    oFilterParameters.centerName = txtCenterNameClient.GetText();
    oFilterParameters.field = cmbBCFieldsValues1Client.GetValue();
    oFilterParameters.criteria = cmbBCCriteria1Client.GetValue();
    oFilterParameters.value = txtValue1Client.GetText();

    var oParameters = {};
    oParameters.action = "FILTER";
    oParameters.gridCenters = "";
    oParameters.filter = oFilterParameters;

    var strParameters = JSON.stringify(oParameters);
    strParameters = encodeURIComponent(strParameters);
    ASPxBusinessCentersSelectorCallbackPanelContenidoClient.PerformCallback(strParameters);
}

function AddNodeToArray(values) {
    selectedNodes = []
    selectedNodes.push(values);
}

function frmBusinessCenterSelector_Cancel() {
    showWUF('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmBusinessCenterSelector', false);
}

function CancelBusinessCenters() {
    showLoadingGrid(true);
    var oParameters = {};
    oParameters.action = "CANCEL";
    oParameters.gridCenters = "";
    var strParameters = JSON.stringify(oParameters);
    strParameters = encodeURIComponent(strParameters);
    ASPxBusinessCentersSelectorCallbackPanelContenidoClient.PerformCallback(strParameters);
}

function selectAllCenters() {
    lpTreeClient.Show();
    lgGridClient.Show();
    if (chkCentersClient.GetChecked()) {
        var keys = tltBusinessCentersClient.GetVisibleNodeKeys();
        for (var i = 0; i < keys.length; i++) {
            tltBusinessCentersClient.SelectNode(keys[i]);
        }
    }
    else {
        var keys = tltBusinessCentersClient.GetVisibleNodeKeys();
        for (var i = 0; i < keys.length; i++) {
            tltBusinessCentersClient.SelectNode(keys[i], false);
        }
    }
    setTimeout(function () { lpTreeClient.Hide(); }, 200);
    setTimeout(function () { lgGridClient.Hide(); }, 200);
}

function GridCenters_EndCallback(s, e) {
}

function SaveBusinessCenters() {
    showLoadingGrid(true);
    var oParameters = {};
    oParameters.action = "SAVEBC";
    oParameters.gridCenters = "";
    var strParameters = JSON.stringify(oParameters);
    strParameters = encodeURIComponent(strParameters);
    ASPxBusinessCentersSelectorCallbackPanelContenidoClient.PerformCallback(strParameters);
}