function GridTasks_beginCallback(e, c) {
}

function GridTasks_EndCallback(s, e) {
    if (s.IsEditing()) {
        var editor = s.GetEditor('InitialDuration');
        editor.Focus();
    }
}

function GridTasks_OnRowDblClick(s, e) {
    if (s.IsEditing()) {
        s.UpdateEdit();
    }
    s.StartEditRow(e.visibleIndex);
}

function GridTasks_FocusedRowChanged(s, e) {
    if (s.IsEditing()) {
        s.UpdateEdit();
    }
}

function GridProjectFields_beginCallback(e, c) {
    var a = 0;
}

function GridProjectFields_EndCallback(s, e) {
    if (s.IsEditing()) {
        var editor = s.GetEditor('FieldValue');
        editor.Focus();
    }
}

function GridProjectFields_OnRowDblClick(s, e) {
    if (s.IsEditing()) {
        s.UpdateEdit();
    }
    s.StartEditRow(e.visibleIndex);
}

function GridProjectFields_FocusedRowChanged(s, e) {
    if (s.IsEditing()) {
        s.UpdateEdit();
    }
}

function GridTaskFieldsTask_beginCallback(e, c) {
}

function GridTaskFieldsTask_EndCallback(s, e) {
    if (s.IsEditing()) {
        var editor = s.GetEditor('ViewName');
        editor.Focus();
    }
}

function GridTaskFieldsTask_OnRowDblClick(s, e) {
    if (s.IsEditing()) {
        s.UpdateEdit();
    }
    s.StartEditRow(e.visibleIndex);
}

function GridTaskFieldsTask_FocusedRowChanged(s, e) {
    if (s.IsEditing()) {
        s.UpdateEdit();
    }
}

function applySelectedMask() {
    var pattern = new RegExp($('#RoGroupBox3_txtMaskFilter').val().toLowerCase().replace(/\*/gi, "([0-9a-z:/\040\\\\]*)"));

    var childContainer = document.getElementById("RoGroupBox3_treeTaskTemplates");
    var childChkBoxes = childContainer.getElementsByTagName("input");
    var childChkBoxCount = childChkBoxes.length;
    var allChecked = true;
    for (var i = 0; i < childChkBoxCount; i++) {
        var curElem = childChkBoxes[i];
        if (pattern.test(curElem.nextSibling.innerHTML.toLowerCase()) == true) {
            curElem.checked = 'checked';
        } else {
            if (curElem.checked == '') {
                allChecked = false;
            }
        }
    }

    if (allChecked == true) {
        if (allSelected == false) {
            document.getElementById("RoGroupBox3_chkTaskTemplates").checked = 'checked';
            allSelected = true;
        }
    } else {
        if (allSelected == true) {
            document.getElementById("RoGroupBox3_chkTaskTemplates").checked = '';
            allSelected = false;
        }
    }
}

function removeSelectedMask() {
    var pattern = new RegExp($('#RoGroupBox3_txtMaskFilter').val().toLowerCase().replace(/\*/gi, "([0-9a-z:/\040\\\\]*)"));
    var childContainer = document.getElementById("RoGroupBox3_treeTaskTemplates")
    var childChkBoxes = childContainer.getElementsByTagName("input");
    var childChkBoxCount = childChkBoxes.length;
    var allChecked = true;
    for (var i = 0; i < childChkBoxCount; i++) {
        var curElem = childChkBoxes[i];
        if (pattern.test(curElem.nextSibling.innerHTML.toLowerCase()) == true) {
            curElem.checked = '';
            allChecked = false;
        } else {
            if (curElem.checked == '') {
                allChecked = false;
            }
        }
    }

    if (allChecked == true) {
        if (allSelected == false) {
            allSelected = true;
            document.getElementById("RoGroupBox3_chkTaskTemplates").checked = 'checked';
        }
    } else {
        allSelected = false;
        document.getElementById("RoGroupBox3_chkTaskTemplates").checked = '';
    }
}

function filterSelectedMask() {
    var pattern = new RegExp($('#RoGroupBox3_txtMaskFilter').val().toLowerCase().replace(/\*/gi, "([0-9a-z:/\040\\\\]*)"));
    var childContainer = document.getElementById("RoGroupBox3_treeTaskTemplates")
    var childChkBoxes = childContainer.getElementsByTagName("input");
    var childChkBoxCount = childChkBoxes.length;

    for (var i = 0; i < childChkBoxCount; i++) {
        var curElem = childChkBoxes[i];
        var aux = curElem.parentElement.parentElement.parentElement.parentElement;

        if (pattern.test(curElem.nextSibling.innerHTML.toLowerCase()) == false) {
            aux.style.display = "none";
        } else {
            aux.style.display = "";
        }
    }
}