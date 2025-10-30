(function () {
    var namespace = function (name) {
        var namespaces = name.split('.'),
            namespace = window,
            index;
        for (index = 0; index < namespaces.length; index += 1) {
            namespace = namespace[namespaces[index]] = namespace[namespaces[index]] || {};
        }
        return namespace;
    };

    namespace("Robotics.Client.Controls.Forms");
}());

Robotics.Client.Controls.Forms.AssignmentsForm = function (prefix, showErrorPopup) {
    this.prefix = prefix;
    this.oShiftDefinition = null;
    this.oEmployeeAssignments = null;
    this.showErrorPopup = showErrorPopup;

    this.shiftDefinitionCallback = eval(this.prefix + "_shiftDefinitionCallbackClient");
    this.cmbAvailableAssignments = eval(this.prefix + "_cmbAvailableAssignments");

    this.showAllAssignments = false;

    this.assignmentsDefinition = {};
};

Robotics.Client.Controls.Forms.AssignmentsForm.prototype.prepareAssignmentsDialog = function (oShiftDefinition, oEmployeeAssignments, idShift, assignmentsDefinition) {
    if (typeof (oEmployeeAssignments) != 'undefined' && oEmployeeAssignments != null) this.oEmployeeAssignments = oEmployeeAssignments;
    else this.oEmployeeAssignments = new Array();

    if (typeof (assignmentsDefinition) != 'undefined' && assignmentsDefinition != null) this.assignmentsDefinition = assignmentsDefinition;

    if (idShift == -2) this.showAllAssignments = true;

    if (oShiftDefinition == null) {
        var oParameters = {};
        oParameters.idShift = idShift;
        oParameters.StampParam = new Date().getMilliseconds();
        oParameters.action = "SHIFTLAYERDEFINITION";

        var strParameters = JSON.stringify(oParameters);
        strParameters = encodeURIComponent(strParameters);

        this.shiftDefinitionCallback.PerformCallback(strParameters);

        return false;
    } else {
        this.oShiftDefinition = oShiftDefinition;

        this.finallyPrepareDialogElements();
        return true;
    }
};
    
Robotics.Client.Controls.Forms.AssignmentsForm.prototype.finallyPrepareDialogElements = function (objResult) {
    var emptyItem = this.cmbAvailableAssignments.FindItemByValue(0);

    this.cmbAvailableAssignments.ClearItems();
    if (!this.showAllAssignments) { this.cmbAvailableAssignments.AddItem(emptyItem.text, emptyItem.value); }

    if (typeof this.oShiftDefinition.Assignments != 'undefined' && this.oShiftDefinition.Assignments != null) {
        for (var shiftIndex = 0; shiftIndex < this.oShiftDefinition.Assignments.length; shiftIndex++) {
            var cAssignment = this.oShiftDefinition.Assignments[shiftIndex];

            if (this.showAllAssignments) {
                for (var employeeIndex = 0; employeeIndex < this.assignmentsDefinition.length; employeeIndex++) {
                    if (cAssignment.IDAssig == this.assignmentsDefinition[employeeIndex].Id) {
                        this.cmbAvailableAssignments.AddItem(this.assignmentsDefinition[employeeIndex].Name, this.assignmentsDefinition[employeeIndex].Id);
                        break;
                    }
                }
            } else {
                for (var employeeIndex = 0; employeeIndex < this.oEmployeeAssignments.length; employeeIndex++) {
                    if (cAssignment.IDAssig == this.oEmployeeAssignments[employeeIndex].ID) {
                        this.cmbAvailableAssignments.AddItem(this.oEmployeeAssignments[employeeIndex].Name, this.oEmployeeAssignments[employeeIndex].ID);
                        break;
                    }
                }
            }
        }
    }
    this.cmbAvailableAssignments.SetSelectedIndex(0);
};

Robotics.Client.Controls.Forms.AssignmentsForm.prototype.focusDialog = function () {
    this.cmbAvailableAssignments.Focus();
};

Robotics.Client.Controls.Forms.AssignmentsForm.prototype.selectcurrentAssignment = function (curAssignment) {
    var assignemntValue = "0";

    if (typeof curAssignment != 'undefined' && curAssignment != null) {
        assignemntValue = curAssignment.ID;
    }

    this.cmbAvailableAssignments.SetSelectedItem(this.cmbAvailableAssignments.FindItemByValue(assignemntValue));
};

Robotics.Client.Controls.Forms.AssignmentsForm.prototype.isValid = function () {
    if (this.cmbAvailableAssignments.GetSelectedItem() != null) {
        return true;
    } else {
        return false;
    }
};

Robotics.Client.Controls.Forms.AssignmentsForm.prototype.getDayData = function (objResult) {
    var retObject = null;

    var sAssign = parseInt(this.cmbAvailableAssignments.GetSelectedItem().value, 10);

    if (this.showAllAssignments) {
        for (var employeeIndex = 0; employeeIndex < this.assignmentsDefinition.length; employeeIndex++) {
            if (sAssign == this.assignmentsDefinition[employeeIndex].Id) {
                var shiftCoverage = 1

                retObject = {
                    ID: this.assignmentsDefinition[employeeIndex].Id,
                    Name: this.assignmentsDefinition[employeeIndex].Name,
                    ShortName: this.assignmentsDefinition[employeeIndex].ShortName,
                    Color: this.assignmentsDefinition[employeeIndex].Color,
                    Cover: shiftCoverage
                };
                break;
            }
        }
    } else {
        for (var employeeIndex = 0; employeeIndex < this.oEmployeeAssignments.length; employeeIndex++) {
            if (sAssign == this.oEmployeeAssignments[employeeIndex].ID) {
                var shiftCoverage = 1

                for (var shiftIndex = 0; shiftIndex < this.oShiftDefinition.Assignments.length; shiftIndex++) {
                    var cAssignment = this.oShiftDefinition.Assignments[shiftIndex];
                    if (cAssignment.IDAssig == sAssign) {
                        shiftCoverage = cAssignment.Cover;
                        break;
                    }
                }

                retObject = {
                    ID: this.oEmployeeAssignments[employeeIndex].ID,
                    Name: this.oEmployeeAssignments[employeeIndex].Name,
                    ShortName: this.oEmployeeAssignments[employeeIndex].ShortName,
                    Color: this.oEmployeeAssignments[employeeIndex].Color,
                    Cover: shiftCoverage
                };
                break;
            }
        }
    }

    return retObject;
};