VTPortal.requestsList = function (params) {
    var requestTypeDS = ko.observable([]);
    var selDate = null;
    if (typeof params.param != 'undefined' && params.param != 'null') selDate = moment(params.param);

    var globalStatus = ko.observable(VTPortal.bigUserInfo());

    function viewShown() {
        globalStatus().viewShown();
        var perm = VTPortal.roApp.empPermissions().Requests;

        var filter = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15];

        if (typeof params.id != 'undefined') filter = params.id.split(',').map(Number);

        var tmpReq = [];
        for (var i = 0; i < perm.length; i++) {
            if (perm[i].Permission && filter.includes(perm[i].RequestType)) {
                switch (perm[i].RequestType) {
                    case 1:
                        tmpReq.push({ "ID": 1, "Name": i18nextko.i18n.t('roRequestType_UserFieldsChange'), "cssClass": "dx-icon-userFieldChange", 'hasAction': true });
                        break;
                    case 2:
                        if ($.grep(tmpReq, function (e) { return e.ID == 2; }).length == 0) tmpReq.push({ "ID": 2, "Name": i18nextko.i18n.t('roRequestType_ForbiddenPunch'), "cssClass": "dx-icon-forbiddenPunch", 'hasAction': true });
                        break;
                    case 3:
                        //Solo se puede seleccionar desde un fihcaje
                        //tmpReq.push({ "ID": 3, "Name": i18nextko.t('roRequestType_JustifyPunch'), "cssClass": "dx-icon-justifyPunch", 'hasAction': true });
                        break;
                    case 4:
                        tmpReq.push({ "ID": 4, "Name": i18nextko.i18n.t('roRequestType_ExternalWorkResumePart'), "cssClass": "dx-icon-externalWorkResume", 'hasAction': true });
                        break;
                    case 5:
                        tmpReq.push({ "ID": 5, "Name": i18nextko.i18n.t('roRequestType_ChangeShift'), "cssClass": "dx-icon-changeShift", 'hasAction': true });
                        break;
                    case 6:
                        if ($.grep(tmpReq, function (e) { return e.ID == 13; }).length == 0) tmpReq.push({ "ID": 13, "Name": i18nextko.i18n.t('roRequestType_PlannedHoliday'), "cssClass": "dx-icon-plannedHoliday", 'hasAction': true });
                        break;
                    case 7:
                        tmpReq.push({ "ID": 7, "Name": i18nextko.i18n.t('roRequestType_PlannedAbsences'), "cssClass": "dx-icon-plannedAbsences", 'hasAction': true });
                        break;
                    case 8:
                        tmpReq.push({ "ID": 8, "Name": i18nextko.i18n.t('roRequestType_ShiftExchange'), "cssClass": "dx-icon-shiftExchange", 'hasAction': true });
                        break;
                    case 9:
                        tmpReq.push({ "ID": 9, "Name": i18nextko.i18n.t('roRequestType_PlannedCauses'), "cssClass": "dx-icon-plannedCauses", 'hasAction': true });
                        break;
                    case 10:
                        if ($.grep(tmpReq, function (e) { return e.ID == 2; }).length == 0) tmpReq.push({ "ID": 2, "Name": i18nextko.i18n.t('roRequestType_ForbiddenPunch'), "cssClass": "dx-icon-forbiddenPunch", 'hasAction': true });
                        break;
                    case 11:
                        tmpReq.push({ "ID": 11, "Name": i18nextko.i18n.t('roRequestType_CancelHolidays'), "cssClass": "dx-icon-holidaysCancel", 'hasAction': true });
                        break;
                    case 12:
                        if ($.grep(tmpReq, function (e) { return e.ID == 2; }).length == 0) tmpReq.push({ "ID": 2, "Name": i18nextko.i18n.t('roRequestType_ForbiddenPunch'), "cssClass": "dx-icon-forbiddenPunch", 'hasAction': true });
                        break;
                    case 13:
                        if ($.grep(tmpReq, function (e) { return e.ID == 13; }).length == 0) tmpReq.push({ "ID": 13, "Name": i18nextko.i18n.t('roRequestType_PlannedHoliday'), "cssClass": "dx-icon-plannedHoliday", 'hasAction': true });
                        break;
                    case 14:
                        tmpReq.push({ "ID": 14, "Name": i18nextko.i18n.t('roRequestType_PlannedOvertime'), "cssClass": "dx-icon-plannedOvertime", 'hasAction': true });
                        break;
                    case 15:
                        tmpReq.push({ "ID": 15, "Name": i18nextko.i18n.t('roRequestType_ExternalWorkWeekResume'), "cssClass": "dx-icon-externalWorkWeekResume", 'hasAction': true });
                        break;
                }
            }
        }
        tmpReq = tmpReq.sortBy(function (n) { return n.Name; });
        requestTypeDS(tmpReq);
    }

    var viewModel = {
        viewShown: viewShown,
        title: i18nextko.t('roSelectRequestType'),
        subscribeBlock: globalStatus(),
        listOptions: {
            dataSource: requestTypeDS,
            height: "76%",
            itemTemplate: 'RequestTypeItem',
            onItemClick: function (info) {
                if (info.itemData.hasAction) {
                    switch (info.itemData.ID) {
                        case 1:
                            VTPortal.app.navigate('userFields/-1', { target: 'current' })
                            break;
                        case 2:
                            VTPortal.app.navigate('forgottenPunch/-1' + (selDate != null ? '/ ' + selDate.format('YYYY-MM-DD') : ''), { target: 'current' });
                            break;
                        case 3:
                            //Solo se puede seleccionar desde un fihcaje
                            //tmpReq.push({ "ID": 3, "Name": i18nextko.t('roRequestType_JustifyPunch'), "cssClass": "dx-icon-justifyPunch", 'hasAction': true });
                            break;
                        case 4:
                            VTPortal.app.navigate('externalWork/-1' + (selDate != null ? '/ ' + selDate.format('YYYY-MM-DD') : '')), { target: 'current' };
                            break;
                        case 5:
                            VTPortal.app.navigate('changeShift/-1' + (selDate != null ? '/ ' + selDate.format('YYYY-MM-DD') : ''), { target: 'current' });
                            break;
                        case 6:
                            VTPortal.app.navigate('plannedHoliday/-1' + (selDate != null ? '/ ' + selDate.format('YYYY-MM-DD') : ''), { target: 'current' });
                            break;
                        case 7:
                            VTPortal.app.navigate('plannedAbsence/-1' + (selDate != null ? '/ ' + selDate.format('YYYY-MM-DD') : ''), { target: 'current' });
                            break;
                        case 8:
                            VTPortal.app.navigate('shiftExchange/-1' + (selDate != null ? '/ ' + selDate.format('YYYY-MM-DD') : ''), { target: 'current' });
                            break;
                        case 9:
                            VTPortal.app.navigate('plannedCause/-1' + (selDate != null ? '/ ' + selDate.format('YYYY-MM-DD') : ''), { target: 'current' });
                            break;
                        case 10:
                            VTPortal.app.navigate('forgottenPunch/-1' + (selDate != null ? '/ ' + selDate.format('YYYY-MM-DD') : ''), { target: 'current' });
                            break;
                        case 11:
                            VTPortal.app.navigate('cancelHolidays/-1' + (selDate != null ? '/ ' + selDate.format('YYYY-MM-DD') : ''), { target: 'current' });
                            break;
                        case 12:
                            VTPortal.app.navigate('forgottenPunch/-1' + (selDate != null ? '/ ' + selDate.format('YYYY-MM-DD') : ''), { target: 'current' });
                            break;
                        case 13:
                            VTPortal.app.navigate('plannedHoliday/-1' + (selDate != null ? '/ ' + selDate.format('YYYY-MM-DD') : ''), { target: 'current' });
                            break;
                        case 14:
                            VTPortal.app.navigate('plannedOvertime/-1' + (selDate != null ? '/ ' + selDate.format('YYYY-MM-DD') : ''), { target: 'current' });
                            break;
                        case 15:
                            VTPortal.app.navigate('externalWorkWeekResume/-1' + (selDate != null ? '/ ' + selDate.format('YYYY-MM-DD') : ''), { target: 'current' });
                            break;
                    }
                }
            }
        },
    };

    return viewModel;
};