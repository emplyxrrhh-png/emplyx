function showComplaintLogBook() {
    $("#divConversationContent").load('/LogBook/ShowComplaint?idComplaint=' + $("#ComplaintId").dxTextBox("instance").option("value"), function () { });
}

function OnComplaintIdChange() {
    if ($("#ComplaintId").dxTextBox("instance").option("value") != null && $("#ComplaintId").dxTextBox("instance").option("value") != "") {
        $("#showComplaintLogBook").dxButton("instance").option("disabled", false);
    }
    else
        $("#showComplaintLogBook").dxButton("instance").option("disabled", true);
}

function printLogBook() {
    window.print();
}