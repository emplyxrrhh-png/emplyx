var deleteRowId = -1;

function GridContracts_BeginCallback(e, c) {
}

function GridContracts_EndCallback(s, e) {
    if (s.IsEditing()) { }
    else {
        if (s.cpActionRO == "DELETE") {
            GridContractsClient.PerformCallback("RELOAD");
        }
    }
}

function GridContracts_OnRowDblClick(s, e) {
    if (s.IsEditing()) {
        s.UpdateEdit();
    }
    s.StartEditRow(e.visibleIndex);
}

function GridContracts_FocusedRowChanged(s, e) {
    if (s.IsEditing()) {
        s.UpdateEdit();
    }
}

//Agregar nueva fila en el grid de incidencias
function AddNewContract(s, e) {
    var grid = ASPxClientGridView.Cast("GridContractsClient");
    grid.AddNewRow();
}

//Eliminar una incidencia en el datatable del servidor
function DeleteAssignment(IdRow) {
    grid = ASPxClientGridView.Cast("GridContractsClient");
    grid.DeleteRow(IdRow);
}

function GridContracts_CustomButtonClick(s, e) {
    if (e.buttonID == "DeleteContractRow") {
        if (e.visibleIndex > -1) {
            deleteRowId = e.visibleIndex;
            GridContractsClient.GetRowValues(e.visibleIndex, 'IDContract', RemoveContractRow);
        }
    } else if (e.buttonID == "UpdateContractRow") {
        UpdateContractRow();
    } else if (e.buttonID == "EditScheduleRowsRow") {
        GridContractsClient.GetRowValues(e.visibleIndex, 'IDContract;IDLabAgree', frmShowContractScheduleRules_Show);
    }
}

function RemoveContractRow(values) {
    window.parent.showLoader(true);
    if (deleteRowId > -1) {
        var contentUrl = "../Employees/DeleteEmployeeContractCaptcha.aspx";
        PopupCaptcha_Client.SetContentUrl(contentUrl);
        PopupCaptcha_Client.Show();
    }
}

function UpdateContractRow() {
    if (GridContractsClient.IsNewRowEditing() == false) {
        window.parent.showLoader(true);
        if (GridContractsClient.IsEditing()) {
            var contentUrl = "../Employees/UpdateEmployeeContractCaptcha.aspx";
            PopupCaptcha_Client.SetContentUrl(contentUrl);
            PopupCaptcha_Client.Show();
        }
    } else {
        GridContractsClient.UpdateEdit();
    }
}

function RemoveContractRowExec() {
    GridContractsClient.DeleteRow(deleteRowId);
}

function UpdateContractRowExec() {
    GridContractsClient.UpdateEdit();
}