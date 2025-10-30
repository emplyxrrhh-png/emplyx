function CloseClick() {
    window.parent.PopupPopupTaskEmployees_Client.SetContentUrl('');
    window.parent.PopupPopupTaskEmployees_Client.Hide();
    window.parent.isShowingTaskEmployees = false;
    window.parent.refreshGrid();
}

function selectedTaks(values) {
    var url = '../Scheduler/MovesNew.aspx?GroupID=-1&TaskFilterID=' + document.getElementById("panelUploadContent_hdnIDTaskSelected").value;
    url = url + "&EmployeeID=" + values[0] + "&Date=" + values[1].getDate() + "/" + (values[1].getMonth() + 1) + "/" + values[1].getFullYear();
    var Title = '';
    parent.ShowExternalForm2(url, 1400, 620, Title, '', false, false, false);
    window.parent.PopupPopupTaskEmployees_Client.Hide()
}

function GridCurrentEmployeesClientCustomButton_Click(s, e) {
    if (e.buttonID == "ShowPunchEmployee") {
        GridCurrentEmployeesClient.GetRowValues(e.visibleIndex, 'ID;PunchDateTime', selectedTaks);
    }
}

function GridPastEmployeesClientCustomButton_Click(s, e) {
    if (e.buttonID == "ShowPunchEmployee") {
        GridPastEmployeesClient.GetRowValues(e.visibleIndex, 'ID;PunchDateTime', selectedTaks);
    }
}