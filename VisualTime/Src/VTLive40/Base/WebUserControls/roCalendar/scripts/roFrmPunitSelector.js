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

Robotics.Client.Controls.Forms.PUnitSelectorForm = function (prefix, showErrorPopup) {
    this.prefix = prefix;
    this.showErrorPopup = showErrorPopup;

    this.cmbAvailablePUnits = eval(this.prefix + "_cmbAvailablePUnits");
};

Robotics.Client.Controls.Forms.PUnitSelectorForm.prototype.getSelectedItem = function () {
    if (this.cmbAvailablePUnits.GetSelectedItem() != null) {
        return this.cmbAvailablePUnits.GetSelectedItem().value;
    } else {
        return -1;
    }
};