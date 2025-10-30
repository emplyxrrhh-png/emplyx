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

Robotics.Client.Controls.Forms.ShiftSelectorForm = function (prefix, showErrorPopup) {
    this.prefix = prefix;
    this.showErrorPopup = showErrorPopup;

    this.shiftDefinition = {};
    this.assignmentsDefinition = {};
    this.cmbAvailableShifts = eval(this.prefix + "_cmbAvailableShifts");
    this.cmbSelectedShiftProperties = null;
};

Robotics.Client.Controls.Forms.ShiftSelectorForm.prototype.getShiftsDefinition = function () {
    this.shiftDefinition = JSON.parse(this.cmbAvailableShifts.cpShiftDefinition, roDateReviver);

    return this.shiftDefinition;
};

Robotics.Client.Controls.Forms.ShiftSelectorForm.prototype.getAssignmentsDefinition = function () {
    this.assignmentsDefinition = JSON.parse(this.cmbAvailableShifts.cpAssignmentsDefinition, roDateReviver);

    return this.assignmentsDefinition;
};

Robotics.Client.Controls.Forms.ShiftSelectorForm.prototype.getSelectedItem = function () {
    this.shiftDefinition = JSON.parse(this.cmbAvailableShifts.cpShiftDefinition, roDateReviver);
    this.assignmentsDefinition = JSON.parse(this.cmbAvailableShifts.cpAssignmentsDefinition, roDateReviver);

    if (this.cmbAvailableShifts.GetSelectedItem() != null) {
        for (var i = 0; i < this.shiftDefinition.length; i++) {
            if (this.shiftDefinition[i].Id == parseInt(this.cmbAvailableShifts.GetSelectedItem().value, 10)) {
                this.cmbSelectedShiftProperties = Object.clone(this.shiftDefinition[i], true);
                break;
            }
        }
    }

    return this.cmbSelectedShiftProperties;
};