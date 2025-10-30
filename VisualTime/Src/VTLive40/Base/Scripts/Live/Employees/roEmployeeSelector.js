//*******************************************************************************************
// ** Author: I. Santaularia.
// ** Date: 25/08/2021.
// ** Description: javascript file for Employee selector module.
//*******************************************************************************************

let partialMode = undefined;
let operationValue = undefined;
let selectAllEnabled = undefined;
let selectNoneEnabled = undefined;
let componentLabels = undefined;
let pageName = undefined;


let swapMode = false;
let syncSelectorWithServer = false;
let universalSelectorName = "";
let selectorView = null;
let serverURL = '/Cookie'
let selectMode = "onlyCustom";
let allowNone = false;

async function initUniversalSelector(customView, syncSelector, selectorName) {
    swapMode = true;

    syncSelectorWithServer = syncSelector;
    universalSelectorName = selectorName;

    if (typeof selectAllEnabled != 'undefined') {
        selectMode = selectAllEnabled ? 'all' : 'onlyCustom';
    }

    if (typeof selectNoneEnabled != 'undefined') {
        allowNone = selectNoneEnabled;
    }

    selectorView = await initSelectorView(customView)

    $("#desinationPopupAdvanced").dxPopup("instance").hide();
    $("#desinationPopup").dxPopup("instance").show();

    swapMode = false;
    //}
}

async function initSelectorView(customView) {
    let tmpView = null;

    if (typeof customView != 'undefined') {
        if (typeof customView == 'object') {
            tmpView = customView;
            tmpView = fillComponentsData(tmpView);
        } else {
            tmpView = generateDefaultFilter(customView);
        }
    } else if (typeof currentView != 'undefined') {
        tmpView = await syncDefaultFilter(currentView);
        tmpView = fillComponentsData(tmpView);
    } else {
        tmpView = generateDefaultFilter(selectMode == 'all' ? "All" : "Custom");
    }

    if (typeof tmpView["ComposeMode"] == 'undefined') tmpView["ComposeMode"] = selectMode == 'all' ? "All" : "Custom";
    if (typeof tmpView["Operation"] == 'undefined') tmpView["Operation"] = "or";

    return tmpView;
}

async function syncDefaultFilter(currentView) {
    let tmpView = currentView;

    if (typeof tmpView['ByPassCache'] == 'undefined') tmpView['ByPassCache'] = false;

    if (syncSelectorWithServer) {
        if (!currentView.ByPassCache) {
            let serverView = await loadSelectorViewFromServer(serverURL, universalSelectorName, pageName);
            if (serverView != null) tmpView = serverView;
        } else {
            if (typeof tmpView["ByPassCache"] != 'undefined') delete tmpView.ByPassCache;
            await sendFilterToServer(tmpView);
        }
    }

    if (typeof tmpView["ByPassCache"] != 'undefined') delete tmpView.ByPassCache;

    return tmpView;
}

async function sendFilterToServer(tmpView) {
    if (typeof tmpView["Employees"] != 'undefined') delete tmpView.Employees;
    if (typeof tmpView["Groups"] != 'undefined') delete tmpView.Groups;
    if (typeof tmpView["Collectives"] != 'undefined') delete tmpView.Collectives;
    if (typeof tmpView["LabAgrees"] != 'undefined') delete tmpView.LabAgrees;

    let response = await fetch(serverURL + "/SetUniversalSelector", {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({ sSelectorName: universalSelectorName, sSelectorValue: JSON.stringify(tmpView) })
    });

    if (!response.ok) {
        throw new Error('Error en la solicitud POST');
    }

    // Convertir la respuesta a JSON
    return await response.json();
}

function plainSelectorEmployeeShown(s, e) {
    let control = document.getElementById("simpleUniversalSelector");

    if (control) {
        // Accedir al primer pare (immediat)
        let parent1 = control.parentElement;

        // Accedir al segon pare
        let parent2 = parent1?.parentElement;

        // Eliminar l'alçada (height) si els pares existeixen
        if (parent1) parent1.style.height = "";
        if (parent2) parent2.style.height = "";
    }

    if (selectorView.ComposeFilter != 'All' && selectorView.ComposeFilter != 'None')


    selectorSimpleGroupsShown();
    selectorSimpleEmployeesShown();
    selectorCollectivesShown();
    selectorLabagreesShown();

    if (selectMode == "onlyCustom" || selectorView.ComposeMode == 'Custom') {
        $(".usCustomFilter").css('display', 'flex');
    } else {
        $(".usCustomFilter").css('display', 'none');
    }

    if (allowNone) $('.usNoneFilter').show();
    else $('.usNoneFilter').hide();

    $("#selectOperation").dxSwitch("instance").option("value", operationValue);
    $("#userFieldsFilter").dxCheckBox("instance").option("value", (selectorView.Filter != "11110" && selectorView.UserFields != ""));



    $(`input[name="rbSelectionMode"][value="${selectorView.ComposeMode}"]`).prop('checked', true);

}


function advSelectorEmployeeShown(s, e) {
    //do nothing by the moment
}

function updateUniversalSelectorMode(s) {
    if (s.defaultValue == 'Custom' && s.checked) {
        $(".usCustomFilter").css('display', 'flex');
    } else {
        $(".usCustomFilter").css('display', 'none');
    }
    if (s.checked) selectorView.ComposeMode = s.defaultValue;
}

function selectorSimpleEmployeesShown() {
    let selectorDataSource = $("#EmployeeText").dxTagBox("instance").getDataSource();
    selectorDataSource.reload();

    if (selectorDataSource.store()._array.length > 200) {
        $("#EmployeeText").dxTagBox("instance").option("minSearchLength", 3);
        $("#EmployeeText").dxTagBox("instance").option("openOnFieldClick", false);
        if (jsLabels) $("#EmployeeText").dxTagBox("instance").option("placeholder", jsLabels["EmployeeSelector#roemployeeplaceholderlarge"]);
    } else {
        $("#EmployeeText").dxTagBox("instance").option("minSearchLength", 0);
        $("#EmployeeText").dxTagBox("instance").option("openOnFieldClick", true);
        if (jsLabels) $("#EmployeeText").dxTagBox("instance").option("placeholder", jsLabels["EmployeeSelector#roemployeeplaceholder"]);
    }

    if (typeof selectorView['Employees'] == 'undefined') selectorView['Employees'] = [];
    $("#EmployeeText").dxTagBox("instance").option("value", selectorView.Employees.map(element => element.IdEmployee));
    $("#EmployeeText").dxTagBox("instance").repaint();
}

function selectorSimpleGroupsShown() {
    let selectorDataSource = $("#GroupText").dxTagBox("instance").getDataSource();
    selectorDataSource.reload();

    if (selectorDataSource.store()._array.length > 200) {
        $("#GroupText").dxTagBox("instance").option("minSearchLength", 3);
        $("#GroupText").dxTagBox("instance").option("openOnFieldClick", false);
        if (jsLabels) $("#GroupText").dxTagBox("instance").option("placeholder", jsLabels["EmployeeSelector#rogroupplaceholderlarge"]);
    } else {
        $("#GroupText").dxTagBox("instance").option("minSearchLength", 0);
        $("#GroupText").dxTagBox("instance").option("openOnFieldClick", true);
        if (jsLabels) $("#GroupText").dxTagBox("instance").option("placeholder", jsLabels["EmployeeSelector#rogroupplaceholder"]);
    }

    if (typeof selectorView['Groups'] == 'undefined') selectorView['Groups'] = [];
    $("#GroupText").dxTagBox("instance").option("value", selectorView.Groups.map(element => element.IdGroup));
    $("#GroupText").dxTagBox("instance").repaint();
}

function selectorCollectivesShown() {
    let selectorDataSource = $("#CollectiveText").dxTagBox("instance").getDataSource();
    selectorDataSource.reload();

    if (selectorDataSource.store()._array.length > 200) {
        $("#CollectiveText").dxTagBox("instance").option("minSearchLength", 3);
        $("#CollectiveText").dxTagBox("instance").option("openOnFieldClick", false);
        if (jsLabels) $("#CollectiveText").dxTagBox("instance").option("placeholder", jsLabels["EmployeeSelector#rocollectiveplaceholderlarge"]);
    } else {
        $("#CollectiveText").dxTagBox("instance").option("minSearchLength", 0);
        $("#CollectiveText").dxTagBox("instance").option("openOnFieldClick", true);
        if (jsLabels) $("#CollectiveText").dxTagBox("instance").option("placeholder", jsLabels["EmployeeSelector#rocollectiveplaceholder"]);
    }

    if (typeof selectorView['Collectives'] == 'undefined') selectorView['Collectives'] = [];
    $("#CollectiveText").dxTagBox("instance").option("value", selectorView.Collectives.map(element => element.FieldValue));
    $("#CollectiveText").dxTagBox("instance").repaint();
}

function selectorLabagreesShown() {
    let selectorDataSource = $("#LabagreeText").dxTagBox("instance").getDataSource();
    selectorDataSource.reload();

    if (selectorDataSource.store()._array.length > 200) {
        $("#LabagreeText").dxTagBox("instance").option("minSearchLength", 3);
        $("#LabagreeText").dxTagBox("instance").option("openOnFieldClick", false);
        if (jsLabels) $("#LabagreeText").dxTagBox("instance").option("placeholder", jsLabels["EmployeeSelector#rolabagreeplaceholderlarge"]);
    } else {
        $("#LabagreeText").dxTagBox("instance").option("minSearchLength", 0);
        $("#LabagreeText").dxTagBox("instance").option("openOnFieldClick", true);
        if (jsLabels) $("#LabagreeText").dxTagBox("instance").option("placeholder", jsLabels["EmployeeSelector#rolabagreeplaceholder"]);
    }

    if (typeof selectorView['LabAgrees'] == 'undefined') selectorView['LabAgrees'] = [];
    $("#LabagreeText").dxTagBox("instance").option("value", selectorView.LabAgrees.map(element => element.FieldValue));
    $("#LabagreeText").dxTagBox("instance").repaint();
}

function setSelectorDisabled(bDisabled) {
    $("#GroupText").dxTagBox("instance").option("disabled", bDisabled);
    $("#EmployeeText").dxTagBox("instance").option("disabled", bDisabled);
    $("#CollectiveText").dxTagBox("instance").option("disabled", bDisabled);
    $("#LabagreeText").dxTagBox("instance").option("disabled", bDisabled);
}

async function showAdvancedSelector() {
    swapMode = true;
    let tmpFilter = "";

    loadSimpleSelectedItems();

    if (selectorView.Groups.length > 0) {
        tmpFilter = selectorView.Groups.map(function (a) { return "A" + a.IdGroup }).join(",");
    }
    if (selectorView.Employees.length > 0) {
        if (tmpFilter != '') tmpFilter = tmpFilter + ",";
        tmpFilter = tmpFilter + selectorView.Employees.map(function (a) { return "B" + a.IdEmployee }).join(",");
    }

    //Erase cookies selector
    await getroTreeState('objContainerTreeV3_mvcTreeSelector').then(roState => roState.reset());
    await getroTreeState('objContainerTreeV3_mvcTreeSelectorGrid').then(roState => roState.reset());

    let state = await getroTreeState('objContainerTreeV3_mvcTreeSelector', true);
    state.setLocalData('', '', '', selectorView.Filter, selectorView.UserFields);

    state = await getroTreeState('objContainerTreeV3_mvcTreeSelectorGrid', true);
    state.setLocalData(tmpFilter, '', '', selectorView.Filter, selectorView.UserFields);

    setTimeout(function () {
        $("#desinationPopup").dxPopup("instance").hide();
        $("#desinationPopupAdvanced").dxPopup("instance").show();
        swapMode = false;
    }, 400);

}

function backToSimpleSelector() {
    swapMode = true;

    saveAdvancedDestination();
    $("#desinationPopupAdvanced").dxPopup("instance").hide();
    $("#desinationPopup").dxPopup("instance").show();

    swapMode = false;
}

function employeeMVCSelector_groupSelected(e) {
    //do nothing by the moment
}

function employeeMVCSelector_empSelected(e) {
    //do nothing by the moment
}

function employeeMVCSelector_collectiveSelected(e) {
    //do nothing by the moment
}

function employeeMVCSelector_labagreeSelected(e) {
    //do nothing by the moment
}

function employeeMVCSelector_collectParamValues(oParm1, oParm2, oParm3) {
    if (oParm1 == "") {
        document.getElementById('hdnMVCEmployees').value = "";
        document.getElementById('hdnMVCFilter').value = "";
        document.getElementById('hdnMVCFilterUser').value = "";
    } else {
        document.getElementById('hdnMVCEmployees').value = oParm1;
        document.getElementById('hdnMVCFilter').value = oParm2;
        document.getElementById('hdnMVCFilterUser').value = oParm3;
    }
}

function saveAdvancedDestination() {
    let groupsArray = [];
    let employeesArray = [];

    let selectedNodes = document.getElementById('hdnMVCEmployees').value.split(",");
    for (let node of selectedNodes) {
        if (node !== "") {
            let id = parseInt(node.substring(1), 10);
            if (node.charAt(0) === "A") {
                groupsArray.push({ IdGroup: id, Name: '' });
            } else {
                employeesArray.push({ IdEmployee: id, EmployeeName: '' });
            }
        }
    }

    selectorView.Employees = employeesArray;
    selectorView.Groups = groupsArray;

    selectorView.Filter = document.getElementById('hdnMVCFilter').value;
    selectorView.UserFields = document.getElementById('hdnMVCFilterUser').value;

}

function loadSimpleSelectedItems() {
    
    if (selectMode == "onlyCustom" || $(`input[name="rbSelectionMode"][value="Custom"]`).prop('checked')) {
        selectorView.Employees = $("#EmployeeText").dxTagBox("instance").option("selectedItems");
        selectorView.Groups = $("#GroupText").dxTagBox("instance").option("selectedItems");
        selectorView.Collectives = $("#CollectiveText").dxTagBox("instance").option("selectedItems");
        selectorView.LabAgrees = $("#LabagreeText").dxTagBox("instance").option("selectedItems");
        selectorView.ComposeFilter = '';

        if (selectorView.Groups.length > 0) {
            selectorView.ComposeFilter = selectorView.Groups.map(function (a) { return "A" + a.IdGroup }).join(",");
        }
        if (selectorView.Employees.length > 0) {
            if (selectorView.ComposeFilter != '') selectorView.ComposeFilter = selectorView.ComposeFilter + ",";
            selectorView.ComposeFilter = selectorView.ComposeFilter + selectorView.Employees.map(function (a) { return "B" + a.IdEmployee }).join(",");
        }
        if (selectorView.Collectives.length > 0) {
            if (selectorView.ComposeFilter != '') selectorView.ComposeFilter = selectorView.ComposeFilter + ",";
            selectorView.ComposeFilter = selectorView.ComposeFilter + selectorView.Collectives.map(function (a) { return "C" + a.FieldValue }).join(",");
        }

        if (selectorView.LabAgrees.length > 0) {
            if (selectorView.ComposeFilter != '') selectorView.ComposeFilter = selectorView.ComposeFilter + ",";
            selectorView.ComposeFilter = selectorView.ComposeFilter + selectorView.LabAgrees.map(function (a) { return "L" + a.FieldValue }).join(",");
        }
        
        selectorView.ComposeMode = 'Custom';
    } else {
        selectorView.Groups = [];
        selectorView.Employees = [];
        selectorView.Collectives = [];
        selectorView.LabAgrees = [];
        selectorView.ComposeFilter = '';
        selectorView.Filter = '11110';
        selectorView.UserFields = '';

        if ($(`input[name="rbSelectionMode"][value="All"]`).prop('checked')) selectorView.ComposeMode = "All";
        else selectorView.ComposeMode = "None";
    }

    
}


async function saveDestination() {
    selectorView.Operation = ($("#selectOperation").dxSwitch("instance").option("value") ? "and" : "or");
    selectorView.Advanced = false;
    loadSimpleSelectedItems();


    $("#desinationPopup").dxPopup("instance").hide();
    await closeAndApplySelector();


}

async function cleanFilter() {
    selectorView.Groups = [];
    selectorView.Employees = [];
    selectorView.Collectives = [];
    selectorView.LabAgrees = [];
    selectorView.ComposeFilter = '';
    selectorView.Filter = '11110';
    selectorView.UserFields = '';
    selectorView.Advanced = false;
    
    $("#GroupText").dxTagBox("instance").option("value", []);
    $("#GroupText").dxTagBox("instance").repaint();

    $("#EmployeeText").dxTagBox("instance").option("value", []);
    $("#EmployeeText").dxTagBox("instance").repaint();

    $("#CollectiveText").dxTagBox("instance").option("value", []);
    $("#CollectiveText").dxTagBox("instance").repaint();

    $("#LabagreeText").dxTagBox("instance").option("value", []);
    $("#LabagreeText").dxTagBox("instance").repaint();

    $("#selectOperation").dxSwitch("instance").option("value", selectorView.Operation);
    $("#userFieldsFilter").dxCheckBox("instance").option("value", false);
}

async function closeAndApplySelector() {
    try {
        
               

        let selectorResponse = { Employees: [], Groups: [], Collectives: [], LabAgrees: [], Operation: "or", Filter: "", UserFields: "", ComposeMode: 'Custom', ComposeFilter: "", Advanced: false, Description: "" };

        selectorResponse.ComposeMode = selectorView.ComposeMode;
        selectorResponse.ComposeFilter = selectorView.ComposeFilter;
        selectorResponse.Employees = selectorView.Employees.map(function (x) { return x.IdEmployee }); 
        selectorResponse.Groups = selectorView.Groups.map(function (x) { return x.IdGroup });  
        selectorResponse.Collectives = selectorView.Collectives.map(function (x) { return x.FieldValue });
        selectorResponse.LabAgrees = selectorView.LabAgrees.map(function (x) { return x.FieldValue });  
        selectorResponse.Operation = selectorView.Operation;
        selectorResponse.Filter = selectorView.Filter;
        selectorResponse.UserFields = selectorView.UserFields;
        selectorResponse.Description = buildSelectedEmployeesString(selectorView);

        let actualTicks = Date.now();
        localStorage.setItem(pageName, JSON.stringify({ view: selectorView, timestamp: actualTicks }));
        if (syncSelectorWithServer) await sendFilterToServer(selectorView); 

        if (!partialMode) {
            await window.parent.closeAndApplySelector(selectorResponse);
        } else {
            try {
                parentCloseAndApplySelector(selectorResponse);
            } catch (e) {
            }
        }
    } catch (e) {
    }
}



function selectorOnHiding(s, e) {
    if (!swapMode) {
        if (!partialMode) window.parent.PopupSelectorEmployeesClient.Hide();
    }
}

registerFunction('initUniversalSelector');