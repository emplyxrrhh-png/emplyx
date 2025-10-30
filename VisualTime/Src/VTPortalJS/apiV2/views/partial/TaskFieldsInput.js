VTPortal.taskFieldsInput = function (params) {
    var lblTaskTitle = ko.observable('');

    var divTitile = ko.observable(false);
    var divTaskField1 = ko.observable(false), divTaskField2 = ko.observable(false), divTaskField3 = ko.observable(false), divTaskField4 = ko.observable(false), divTaskField5 = ko.observable(false), divTaskField6 = ko.observable(false);
    var lblTaskField1 = ko.observable(''), lblTaskField2 = ko.observable(''), lblTaskField3 = ko.observable(''), lblTaskField4 = ko.observable(''), lblTaskField5 = ko.observable(''), lblTaskField6 = ko.observable('');
    var valueTaskField1 = ko.observable(''), valueTaskField2 = ko.observable(''), valueTaskField3 = ko.observable(''), valueTaskField4 = ko.observable(''), valueTaskField5 = ko.observable(''), valueTaskField6 = ko.observable('');
    var cmbTaskField1 = ko.observable([]), cmbTaskField2 = ko.observable([]), cmbTaskField3 = ko.observable([]), cmbTaskField4 = ko.observable([]), cmbTaskField5 = ko.observable([]), cmbTaskField6 = ko.observable([]);
    var textFieldType1 = ko.observable(true), textFieldType2 = ko.observable(true), textFieldType3 = ko.observable(true), textFieldType4 = ko.observable(true), textFieldType5 = ko.observable(true), textFieldType6 = ko.observable(true);
    var cmbFieldType1 = ko.observable(true), cmbFieldType2 = ko.observable(true), cmbFieldType3 = ko.observable(true), cmbFieldType4 = ko.observable(true), cmbFieldType5 = ko.observable(true), cmbFieldType6 = ko.observable(true);

    var formReadOnly = ko.observable(false);

    var initTaskProperties = function (readOnly) {
        formReadOnly(readOnly);

        valueTaskField1(''); valueTaskField2(''); valueTaskField3(''); valueTaskField4(''); valueTaskField5(''); valueTaskField6('');

        var punchStep = VTPortal.roApp.taskPunchRequest()
        var fConf = null;
        var fValues = null;

        if (punchStep.currentAction == -2) {
            divTitile(true);
            fConf = VTPortal.roApp.taskPunchRequest().currentTaskUserFieldsDefinition;
            fValues = VTPortal.roApp.taskPunchRequest().currentTaskUserFieldsValue;
            lblTaskTitle(i18nextko.i18n.t('roTaskFieldsInfo'));
        } else {
            if (punchStep.currentAction != -1 || punchStep.newAction != -1) {
                divTitile(true);
                if (punchStep.currentAction != -1 && punchStep.newAction == -1) {
                    fConf = VTPortal.roApp.taskPunchRequest().currentTaskUserFieldsDefinition;
                    fValues = VTPortal.roApp.taskPunchRequest().currentTaskUserFieldsValue;
                    lblTaskTitle(i18nextko.i18n.t('roCurrentTaskUserFields') + VTPortal.roApp.userStatus().TaskTitle);
                } else {
                    fConf = VTPortal.roApp.taskPunchRequest().newTaskUserFieldsDefinition;
                    fValues = VTPortal.roApp.taskPunchRequest().newTaskUserFieldsValue;
                    lblTaskTitle(i18nextko.i18n.t('roNewTaskUserFields'));
                }
            }
        }

        if (fConf != null && fConf.length == 6) {
            for (var i = 0; i < fConf.length; i++) {
                var divField = eval('divTaskField' + (i + 1));
                var lblName = eval('lblTaskField' + (i + 1));

                var textTaskField = eval('lblTaskField' + (i + 1));
                var cmbTaskField = eval('cmbTaskField' + (i + 1));
                var textFieldType = eval('textFieldType' + (i + 1));
                var cmbFieldType = eval('cmbFieldType' + (i + 1));
                var valField = eval('valueTaskField' + (i + 1));

                if (fConf[i].Used) {
                    divField(true);

                    if (i < 3 && fValues[i] != '') valField(fValues[i]);
                    else if (i >= 3 && parseFloat(fValues[i]) != -1) valField(fValues[i]);

                    lblName(fConf[i].Name);
                    textFieldType(fConf[i].ValueType == 0);
                    cmbFieldType(fConf[i].ValueType == 1);

                    if (fConf[i].ValueType == 1) {
                        if (i < 3) cmbTaskField(fConf[i].ValuesList)
                        else if (i >= 3) {
                            var tmpList = [];

                            for (x = 0; x < fConf[i].ValuesList.length; x++) {
                                tmpList.push(parseFloat(fConf[i].ValuesList[x]));
                            }
                            cmbTaskField(tmpList)
                        }
                    }
                } else {
                    divField(false);
                }
            }
        }
    }

    var savePunchInfo = function () {
        var punchStep = VTPortal.roApp.taskPunchRequest()

        if (punchStep.currentAction == -2) {
            if (VTPortal.roApp.taskPunchRequest().currentTaskUserFieldsDefinition[0].Used) VTPortal.roApp.taskPunchRequest().currentTaskUserFieldsValue[0] = valueTaskField1();
            if (VTPortal.roApp.taskPunchRequest().currentTaskUserFieldsDefinition[1].Used) VTPortal.roApp.taskPunchRequest().currentTaskUserFieldsValue[1] = valueTaskField2();
            if (VTPortal.roApp.taskPunchRequest().currentTaskUserFieldsDefinition[2].Used) VTPortal.roApp.taskPunchRequest().currentTaskUserFieldsValue[2] = valueTaskField3();
            if (VTPortal.roApp.taskPunchRequest().currentTaskUserFieldsDefinition[3].Used) VTPortal.roApp.taskPunchRequest().currentTaskUserFieldsValue[3] = valueTaskField4();
            if (VTPortal.roApp.taskPunchRequest().currentTaskUserFieldsDefinition[4].Used) VTPortal.roApp.taskPunchRequest().currentTaskUserFieldsValue[4] = valueTaskField5();
            if (VTPortal.roApp.taskPunchRequest().currentTaskUserFieldsDefinition[5].Used) VTPortal.roApp.taskPunchRequest().currentTaskUserFieldsValue[5] = valueTaskField6();
        } else {
            if (punchStep.currentAction != -1 || punchStep.newAction != -1) {
                if (punchStep.currentAction != -1 && punchStep.newAction == -1) {
                    if (VTPortal.roApp.taskPunchRequest().currentTaskUserFieldsDefinition[0].Used) VTPortal.roApp.taskPunchRequest().currentTaskUserFieldsValue[0] = valueTaskField1();
                    if (VTPortal.roApp.taskPunchRequest().currentTaskUserFieldsDefinition[1].Used) VTPortal.roApp.taskPunchRequest().currentTaskUserFieldsValue[1] = valueTaskField2();
                    if (VTPortal.roApp.taskPunchRequest().currentTaskUserFieldsDefinition[2].Used) VTPortal.roApp.taskPunchRequest().currentTaskUserFieldsValue[2] = valueTaskField3();
                    if (VTPortal.roApp.taskPunchRequest().currentTaskUserFieldsDefinition[3].Used) VTPortal.roApp.taskPunchRequest().currentTaskUserFieldsValue[3] = valueTaskField4();
                    if (VTPortal.roApp.taskPunchRequest().currentTaskUserFieldsDefinition[4].Used) VTPortal.roApp.taskPunchRequest().currentTaskUserFieldsValue[4] = valueTaskField5();
                    if (VTPortal.roApp.taskPunchRequest().currentTaskUserFieldsDefinition[5].Used) VTPortal.roApp.taskPunchRequest().currentTaskUserFieldsValue[5] = valueTaskField6();
                } else {
                    if (VTPortal.roApp.taskPunchRequest().newTaskUserFieldsDefinition[0].Used) VTPortal.roApp.taskPunchRequest().newTaskUserFieldsValue[0] = valueTaskField1();
                    if (VTPortal.roApp.taskPunchRequest().newTaskUserFieldsDefinition[1].Used) VTPortal.roApp.taskPunchRequest().newTaskUserFieldsValue[1] = valueTaskField2();
                    if (VTPortal.roApp.taskPunchRequest().newTaskUserFieldsDefinition[2].Used) VTPortal.roApp.taskPunchRequest().newTaskUserFieldsValue[2] = valueTaskField3();
                    if (VTPortal.roApp.taskPunchRequest().newTaskUserFieldsDefinition[3].Used) VTPortal.roApp.taskPunchRequest().newTaskUserFieldsValue[3] = valueTaskField4();
                    if (VTPortal.roApp.taskPunchRequest().newTaskUserFieldsDefinition[4].Used) VTPortal.roApp.taskPunchRequest().newTaskUserFieldsValue[4] = valueTaskField5();
                    if (VTPortal.roApp.taskPunchRequest().newTaskUserFieldsDefinition[5].Used) VTPortal.roApp.taskPunchRequest().newTaskUserFieldsValue[5] = valueTaskField6();
                }
            }
        }
    }

    var viewModel = {
        initTaskProperties: initTaskProperties,
        savePunchInfo: savePunchInfo,
        lblTaskTitle: lblTaskTitle,
        lblTaskField1: lblTaskField1,
        lblTaskField2: lblTaskField2,
        lblTaskField3: lblTaskField3,
        lblTaskField4: lblTaskField4,
        lblTaskField5: lblTaskField5,
        lblTaskField6: lblTaskField6,
        divTitile: divTitile,
        divTaskField1: divTaskField1,
        divTaskField2: divTaskField2,
        divTaskField3: divTaskField3,
        divTaskField4: divTaskField4,
        divTaskField5: divTaskField5,
        divTaskField6: divTaskField6,
        txtTaskField1: {
            value: valueTaskField1,
            visible: textFieldType1,
            readOnly: formReadOnly
        },
        txtTaskField2: {
            value: valueTaskField2,
            visible: textFieldType2,
            readOnly: formReadOnly
        },
        txtTaskField3: {
            value: valueTaskField3,
            visible: textFieldType3,
            readOnly: formReadOnly
        },
        txtTaskField4: {
            value: valueTaskField4,
            visible: textFieldType4,
            showSpinButtons: true,
            readOnly: formReadOnly
        },
        txtTaskField5: {
            value: valueTaskField5,
            visible: textFieldType5,
            showSpinButtons: true,
            readOnly: formReadOnly
        },
        txtTaskField6: {
            value: valueTaskField6,
            visible: textFieldType6,
            showSpinButtons: true,
            readOnly: formReadOnly
        },
        cmbTaskField1: {
            dataSource: cmbTaskField1,
            value: valueTaskField1,
            visible: cmbFieldType1,
            readOnly: formReadOnly
        },
        cmbTaskField2: {
            dataSource: cmbTaskField2,
            value: valueTaskField2,
            visible: cmbFieldType2,
            readOnly: formReadOnly
        },
        cmbTaskField3: {
            dataSource: cmbTaskField3,
            value: valueTaskField3,
            visible: cmbFieldType3,
            readOnly: formReadOnly
        },
        cmbTaskField4: {
            dataSource: cmbTaskField4,
            value: valueTaskField4,
            visible: cmbFieldType4,
            readOnly: formReadOnly
        },
        cmbTaskField5: {
            dataSource: cmbTaskField5,
            value: valueTaskField5,
            visible: cmbFieldType5,
            readOnly: formReadOnly
        },
        cmbTaskField6: {
            dataSource: cmbTaskField6,
            value: valueTaskField6,
            visible: cmbFieldType6,
            readOnly: formReadOnly
        }
    };

    return viewModel;
};