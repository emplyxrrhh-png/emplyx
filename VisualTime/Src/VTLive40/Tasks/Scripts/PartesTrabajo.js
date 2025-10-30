function GridPartes_BeginCallback(e, c) {

}

function GridPartes_EndCallback(s, e) {
   
}

function GridPartes_OnRowDblClick(s, e) {
    if (s.IsEditing()) {
        s.UpdateEdit();
    }
    s.StartEditRow(e.visibleIndex);
}

function GridPartes_FocusedRowChanged(s, e) {
    if (s.IsEditing()) {
        s.UpdateEdit();
    }
}

function GridPartes_CustomButtonClick(s, e) {

}

function btnRefreshClient_Click(s, e) {
    CallbackSessionClient.PerformCallback("REFRESHGRID");
}

function CallbackSession_CallbackComplete(s, e) {
    grdPartesClient.Refresh();
}