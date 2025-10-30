var currentView = null;
var currentTgView = null;
var fileContent = null;
var fileContentTG = null;
var colorLeft = "#FF5C35";
var colorRight = "#f8aa32";
var opacity = 10;
var position = 2;

let hasTimegateChanges = false;
let hasPortalChanges = false;
let loading = false;


$(document).ready(function () {
    onPortalConfigurationShown();

    $('#portalGeneralConfiguration').off("click");
    $('#portalGeneralConfiguration').on("click", function (e) {
        $(".generalConfigurationDiv").show();
        $(".timegateConfDiv").hide();
        $(".geolocalizationConfigurationDiv").hide();
        $(".drConfigurationDiv").hide();
        $("#portalGeneralConfiguration").addClass("bTabPortalConfigurationMenu-active");
        $("#timegateConfiguration").removeClass("bTabPortalConfigurationMenu-active");
        $("#portalDRConfiguration").removeClass("bTabPortalConfigurationMenu-active");
        $("#portalGeolocalizationConfiguration").removeClass("bTabPortalConfigurationMenu-active")
    });

    $('#timegateConfiguration').off("click");
    $('#timegateConfiguration').on("click", function (e) {
        $(".generalConfigurationDiv").hide();
        $(".timegateConfDiv").show();
        $(".geolocalizationConfigurationDiv").hide();
        $(".drConfigurationDiv").hide();
        $("#portalGeneralConfiguration").removeClass("bTabPortalConfigurationMenu-active");
        $("#timegateConfiguration").addClass("bTabPortalConfigurationMenu-active");
        $("#portalDRConfiguration").removeClass("bTabPortalConfigurationMenu-active");
        $("#portalGeolocalizationConfiguration").removeClass("bTabPortalConfigurationMenu-active")
    });

    $('#portalGeolocalizationConfiguration').off("click");
    $('#portalGeolocalizationConfiguration').on("click", function (e) {
        $(".generalConfigurationDiv").hide();
        $(".timegateConfDiv").hide();
        $(".drConfigurationDiv").hide();
        $(".geolocalizationConfigurationDiv").show();
        $("#portalGeneralConfiguration").removeClass("bTabPortalConfigurationMenu-active");
        $("#timegateConfiguration").removeClass("bTabPortalConfigurationMenu-active");
        $("#portalDRConfiguration").removeClass("bTabPortalConfigurationMenu-active");
        $("#portalGeolocalizationConfiguration").addClass("bTabPortalConfigurationMenu-active");
    });

    $('#portalDRConfiguration').off("click");
    $('#portalDRConfiguration').on("click", function (e) {
        $(".drConfigurationDiv").show();
        $(".timegateConfDiv").hide();
        $(".geolocalizationConfigurationDiv").hide();
        $(".generalConfigurationDiv").hide();
        $("#portalDRConfiguration").addClass("bTabPortalConfigurationMenu-active");
        $("#timegateConfiguration").removeClass("bTabPortalConfigurationMenu-active");
        $("#portalGeolocalizationConfiguration").removeClass("bTabPortalConfigurationMenu-active")
        $("#portalGeneralConfiguration").removeClass("bTabPortalConfigurationMenu-active");
    });
    setTimeout(function () {
        $("#timegateUserFieldId").dxSelectBox("instance").option("disabled", !$("#chkTimegateUsesUserField").dxSwitch("instance").option("value"));
    }, 200);
});

function onPortalChanges() {
    if (!loading) hasPortalChanges = true;
}

function onTimegateChanges() {
    if (!loading) hasTimegateChanges = true;
}
function onTimegateIDChange(s, e) {
    if (!loading) hasTimegateChanges = true;
    $("#timegateUserFieldId").dxSelectBox("instance").option("disabled",!s.value);
}

function onLimitDRDays(s, e) {
    if (!loading) hasPortalChanges = true;
    let enabled = s.value;
    let drLimitDays = $("#drLimitDays").dxNumberBox("instance");

    // Actualizar el valor del control y su estado de habilitación
    drLimitDays.option("value", enabled ? drLimitDays.option("value") : null);
    drLimitDays.option("disabled", !enabled);

    // Pasar el foco si está habilitado
    if (enabled) {
        drLimitDays.focus();
    } 
}

function drLimitDaysValueChanged(s, e) {
    if (!loading) hasPortalChanges = true;
}

function changePosition(e) {
    position = e.value;
    if (!loading) hasPortalChanges = true;
    if (fileContent != null) {
        setImagePreview(fileContent, e.value);
    }
}

function changeTGPosition(e) {
    position = e.value;
    if (!loading) hasTimegateChanges = true;
    if (fileContentTG != null) {
        setTGImagePreview(fileContentTG, e.value);
    }
}
function changeOpacity(e) {
    opacity = e.value;
    if (fileContent != null) {
        setImagePreview(fileContent, position);
    }
}

function changeTimeGateOpacity(e) {
    opacity = e.value;
    if (fileContentTG != null) {
        setTGImagePreview(fileContentTG, position);
    }
}
function setImagePreview(image, position) {
    if(!loading) hasPortalChanges = true;
    position = 3; //alineamos siempre a la derecha con la nueva cabecera
    var elF = document.getElementById('backgroundPhoto');
    var tOpacity = opacity / 10;
    var tFileContent = "";

    if (image == null || image == "") {
        //tFileContent = BASE_URL + 'Base/Images/PortalHeader.png';
    }
    else {
        tFileContent = fileContent;
    }
    if (position == null) {
        elF.setAttribute('style', 'background: url(' + tFileContent + ') no-repeat center; background-size: contain !important;height:100%;opacity:' + tOpacity + '');
    }
    else {
        switch (position) {
            case 4:
                elF.setAttribute('style', 'background: url(' + tFileContent + ') no-repeat center; background-size: cover !important;height:100%;opacity:' + tOpacity + '');
                break;
            case 3:
                elF.setAttribute('style', 'background: url(' + tFileContent + ') no-repeat right; background-size: contain !important;height:100%;opacity:' + tOpacity + '');
                break;
            case 2:
                elF.setAttribute('style', 'background: url(' + tFileContent + ') no-repeat center; background-size: contain !important;height:100%;opacity:' + tOpacity + '');
                break;
            case 1:
                elF.setAttribute('style', 'background: url(' + tFileContent + ') no-repeat left; background-size: contain !important;height:100%;opacity:' + tOpacity + '');
                break;
            default:
                elF.setAttribute('style', 'background: url(' + tFileContent + ') no-repeat center; background-size: contain !important;height:100%;opacity:' + tOpacity + '');
                break;
        }
    }
}

function setTGImagePreview(image, position) {
    if (!loading) hasTimegateChanges = true;
    position = 3; //alineamos siempre a la izquierda
    var elF = document.getElementById('backgroundTGPhoto');
    var tOpacity = opacity / 10;
    var tFileContent = "";

    if (image == null || image == "") {
        //tFileContent = BASE_URL + 'Base/Images/PortalHeader.png';
    }
    else {
        tFileContent = fileContentTG;
    }
    
        elF.setAttribute('style', 'background: url(' + tFileContent + ') no-repeat top left; background-size: cover !important;height:100%;opacity:' + tOpacity + '');    
}


function setColorPreview(leftColor, rightColor) {    

    if (leftColor == null || leftColor == "") {
        colorLeft = "#FF5C35";
    }
    if (rightColor == null || rightColor == "") {
        colorRight = "#f8aa32";
    }
    let el = document.getElementById('previewDiv');
    el.setAttribute('style', 'background: url("Base/Images/bgHeaderPortal.png"), linear-gradient(to right, ' + colorLeft + ', ' + colorRight + ');border-radius:5px;height:100px;');
}

function setTGColorPreview(leftColor, rightColor) {

    if (leftColor == null || leftColor == "") {
        colorLeft = "#FF5C35";
    }
    if (rightColor == null || rightColor == "") {
        colorRight = "#f8aa32";
    }
    let el = document.getElementById('previewTGDiv');
    el.setAttribute('style', 'background: url("Base/Images/bgTimeGate.png"), linear-gradient(to right, ' + colorLeft + ', ' + colorRight + ');border-radius:5px;height:100px;');
}

function onColorChangedLeft(e) {
    if (e.value != "") {
        if (!loading) hasPortalChanges = true;
        colorLeft = e.value;
        let el = document.getElementById('previewDiv');
        el.setAttribute('style', 'background: url("Base/Images/bgHeaderPortal.png"), linear-gradient(to right, ' + colorLeft + ', ' + colorRight + ');border-radius:5px;height:100px;');
    }
}

function onTGColorChangedLeft(e) {
    if (e.value != "") {
        if (!loading) hasTimegateChanges = true;
        colorLeft = e.value;
        let el = document.getElementById('previewTGDiv');
        el.setAttribute('style', 'background: url("Base/Images/bgTimeGate.png"), linear-gradient(to right, ' + colorLeft + ', ' + colorRight + ');border-radius:5px;height:100px;');
    }
}

function onColorChangedRight(e) {
    if (e.value != "") {
        if (!loading) hasPortalChanges = true;
        colorRight = e.value;
        let el = document.getElementById('previewDiv');
        el.setAttribute('style', 'background: url("Base/Images/bgHeaderPortal.png"), linear-gradient(to right, ' + colorLeft + ', ' + colorRight + ');border-radius:5px;height:100px;');    }
}

function onTGColorChangedRight(e) {
    if (e.value != "") {
        if (!loading) hasTimegateChanges = true;
        colorRight = e.value;
        let el = document.getElementById('previewTGDiv');
        el.setAttribute('style', 'background: url("Base/Images/bgTimeGate.png"), linear-gradient(to right, ' + colorLeft + ', ' + colorRight + ');border-radius:5px;height:100px;');
    }
}

async function saveApplicationChanges() {
    let bSaved = true;

    let errorText = validateData();
    if (errorText == '') {
        if (hasTimegateChanges) bSaved = await saveTimegateChanges();
        if (bSaved && hasPortalChanges) await savePortalConfiguration();
    } else {
        DevExpress.ui.notify(errorText, "error", 2000);
    }
    
}

function validateData() {

    if (hasTimegateChanges) {
        let customField = $("#chkTimegateUsesUserField").dxSwitch("instance").option("value");
        let timegateUserFieldId = $('#timegateUserFieldId').dxSelectBox('instance').option('value');

        if (timegateUserFieldId == null && customField) {
            return viewUtilsManager.DXTranslate('roTimegateMissingConfiguration');
        }
    }

    if (hasPortalChanges) {
        if (document.getElementById("isDailyRecordEnabled").value == '1') {
            let chkLimitDRDays = $("#chkLimitDRDays").dxSwitch("instance").option("value");
            let drLimitDays = $('#drLimitDays').dxNumberBox('instance').option('value');

            if (drLimitDays == null && chkLimitDRDays) {
                $('#drLimitDays').dxNumberBox('instance').focus();
                return viewUtilsManager.DXTranslate('roDRMaxDaysNotInformed');
            }
        }
    }

    return "";
}

async function saveTimegateChanges() {
    let bSaved = false;

    let customField = $("#chkTimegateUsesUserField").dxSwitch("instance").option("value");
    let timegateUserFieldId = $('#timegateUserFieldId').dxSelectBox('instance').option('value');
    let timegateUserFieldText = $('#timegateUserFieldId').dxSelectBox('instance').option('text');
    let image = fileContentTG;
    let leftColor = $('#selectTGLeftColor').dxColorBox('instance').option('value');
    let rightColor = $('#selectTGRightColor').dxColorBox('instance').option('value');
    let position = $('#selectPositionTimeGateBackground').dxSelectBox('instance').option('value');
    let opacity = $('#selectOpacityTimegateBackground').dxSlider('instance').option('value');

    let tbConfig = { customUserFieldEnabled: customField, userFieldId: timegateUserFieldId, userFieldName: timegateUserFieldText, image: image, position: position, opacity: opacity, leftColor: leftColor, rightColor: rightColor }         

    loading = true;

    let response = await fetch(BASE_URL + 'PortalConfiguration/SaveTimegateConfiguration', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(tbConfig)
    });

    if (!response.ok) {
        DevExpress.ui.notify(viewUtilsManager.DXTranslate('roPortalConfigurationSaveError'), "error", 2000);
    } else {

        bSaved = await response.json();
        if (typeof bSaved == 'string') {
            DevExpress.ui.notify(bSaved, "error", 5000);
        } else if (!bSaved) {
            DevExpress.ui.notify(viewUtilsManager.DXTranslate('roPortalConfigurationSaveError'), "error", 2000);
        } else {
            DevExpress.ui.notify(viewUtilsManager.DXTranslate('roPortalConfigurationSaved'), "success", 2000);
            hasTimegateChanges = false;
        }

        
    }
    loading = false;

    return bSaved;
}

async function savePortalConfiguration() {

    let bSaved = false;

    let image = fileContent;
    let leftColor = $('#selectLeftColor').dxColorBox('instance').option('value');
    let rightColor = $('#selectRightColor').dxColorBox('instance').option('value');
    let position = $('#selectPositionPortalConfiguration').dxSelectBox('instance').option('value');
    let opacity = $('#selectOpacityPortalConfiguration').dxSlider('instance').option('value');
    let impersonate = $('#chkImpersonate').dxSwitch('instance').option('value');
    let geolocalization = $('#selectGeolocalization').dxSelectBox('instance').option('value');
    let zoneRequired = $('#chkPunchRequireZone').dxSwitch('instance').option('value');
    let drMaxDaysOnPast = -1;
    let drUsePattern = false;

    if (document.getElementById("isDailyRecordEnabled").value == '1') {
        if ($('#chkLimitDRDays').dxSwitch('instance').option('value') &&
            $('#drLimitDays').dxNumberBox('instance').option('value') != null) {
            drMaxDaysOnPast = $('#drLimitDays').dxNumberBox('instance').option('value');
        }

        if ($('#chkDR')) {
            drUsePattern = $('#chkDR').dxSwitch('instance').option('value');
        }
    }

    loading = true;

    let response = await fetch(BASE_URL + 'PortalConfiguration/SavePortalConfiguration', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({ Image: image, Position: position, Opacity: opacity, LeftColor: leftColor, RightColor: rightColor, Geolocalization: geolocalization, PunchZoneRequired: zoneRequired, DailyRecordPattern: drUsePattern, Impersonate: impersonate, DrMaxDaysOnPast: drMaxDaysOnPast })
    });

    if (!response.ok) {
        DevExpress.ui.notify(viewUtilsManager.DXTranslate('roPortalConfigurationSaveError'), "error", 2000);
    } else {

        bSaved = await response.json();
        if (!bSaved) {
            DevExpress.ui.notify(viewUtilsManager.DXTranslate('roPortalConfigurationSaveError'), "error", 2000);
        } else {
            DevExpress.ui.notify(viewUtilsManager.DXTranslate('roPortalConfigurationSaved'), "success", 2000);
            hasPortalChanges = false;
        }
    }

    loading = false;

    return bSaved;
}

function cancelPortalConfiguration() {
    onPortalConfigurationShown();
}
function restorePortalConfiguration() {
    loading = true;
    $.ajax({
        type: "POST",
        url: BASE_URL + 'PortalConfiguration/RestorePortalConfiguration',
        dataType: "json",
        success: function (e) {
            if (e == false) {
                DevExpress.ui.notify(viewUtilsManager.DXTranslate('roPortalConfigurationSaveError'), "error", 2000);
            } else {
                DevExpress.ui.notify(viewUtilsManager.DXTranslate('roPortalConfigurationSaved'), "success", 2000);
                onPortalConfigurationShown();
            }
            loading = false;
        },
        error: function (e) {
            DevExpress.ui.notify(viewUtilsManager.DXTranslate('roPortalConfigurationSaveError'), "error", 2000);
            loading = false;
        }
    });
}

async function onPortalConfigurationShown() {
    loading = true;

    let loadError = false;
    let response = await fetch(BASE_URL + 'PortalConfiguration/GetPortalConfiguration', {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json'
        }
    });

    if (!response.ok) {
        loadError = true
    } else {
        let data = await response.json();

        currentView = data;
        if (data != null && data.HeaderConfiguration != null) {
            fileContent = data.HeaderConfiguration.Image;
            if (fileContent != null && fileContent != "") {
                $("#dropzone-text")[0].style.display = "none";
                $("#dropzone-image")[0].src = fileContent;
                $("#btnDeleteImage")[0].style.display = "";
            } else {
                $("#dropzone-text")[0].style.display = "";
                $("#dropzone-image")[0].src = "";
                $("#btnDeleteImage")[0].style.display = "none";
            }
            $('#selectLeftColor').dxColorBox('instance').option('value', data.HeaderConfiguration.LeftColor);
            $('#selectRightColor').dxColorBox('instance').option('value', data.HeaderConfiguration.RightColor);
            $('#selectPositionPortalConfiguration').dxSelectBox('instance').option('value', data.HeaderConfiguration.Position);
            $('#chkImpersonate').dxSwitch('instance').option('value', data.GeneralConfiguration.Impersonate);
            $('#selectGeolocalization').dxSelectBox('instance').option('value', data.GeolocalizationConfiguration.Geolocalization);
            $('#chkPunchRequireZone').dxSwitch('instance').option('value', data.PunchOptions.ZoneRequired);
            if ($('#chkDR') && document.getElementById("isDailyRecordEnabled").value == '1') $('#chkDR').dxSwitch('instance').option('value', data.DailyRecordPattern);

            if (document.getElementById("isDailyRecordEnabled").value == '1') {
                if (data.DailyRecordMaxDaysOnPast > -1) {
                    $('#chkLimitDRDays').dxSwitch('instance').option('value', true);
                    $('#drLimitDays').dxNumberBox('instance').option('value', data.DailyRecordMaxDaysOnPast);
                } else {
                    $('#chkLimitDRDays').dxSwitch('instance').option('value', false);
                    $('#drLimitDays').dxNumberBox('instance').option('value', null);
                    $('#drLimitDays').dxNumberBox('instance').option('disabled', true);
                }
            }

            $('#selectOpacityPortalConfiguration').dxSlider('instance').option('value', data.HeaderConfiguration.Opacity);
            opacity = parseInt(data.HeaderConfiguration.Opacity);
            position = parseInt(data.HeaderConfiguration.Position);
            setColorPreview(data.HeaderConfiguration.LeftColor, data.HeaderConfiguration.RightColor);
        } else {
            if (data == null || data.HeaderConfiguration == null) {
                colorLeft = "#0046fe";
                colorRight = "#57aeeb";
                fileContent = "";
                $("#dropzone-text")[0].style.display = "";
                $("#dropzone-image")[0].src = "";
                $("#btnDeleteImage")[0].style.display = "none";
                $('#selectLeftColor').dxColorBox('instance').option('value', colorLeft);
                $('#selectRightColor').dxColorBox('instance').option('value', colorRight);
                $('#selectPositionPortalConfiguration').dxSelectBox('instance').option('value', 1);
                if ($('#chkDR') && document.getElementById("isDailyRecordEnabled").value == '1') $('#chkDR').dxSwitch('instance').option('value', false);
                opacity = 10;
                position = 1;
                $('#selectOpacityPortalConfiguration').dxSlider('instance').option('value', "10");
                setImagePreview(fileContent, position);
                setColorPreview(colorLeft, colorRight);
                if (data.GeolocalizationConfiguration == null) {
                    $('#selectGeolocalization').dxSelectBox('instance').option('value', 1);
                    $('#chkPunchRequireZone').dxSwitch('instance').option('value', false);
                }
                else {
                    $('#selectGeolocalization').dxSelectBox('instance').option('value', data.GeolocalizationConfiguration.Geolocalization);
                    $('#chkPunchRequireZone').dxSwitch('instance').option('value', data.PunchOptions.ZoneRequired);
                }
            }
        }

        if (data != null && data.GeneralConfiguration != null) {
            $('#chkImpersonate').dxSwitch('instance').option('value', data.GeneralConfiguration.Impersonate);
        } else {
            $('#chkImpersonate').dxSwitch('instance').option('value', true);
        }
    }

    if (!loadError) {
        let response = await fetch(BASE_URL + 'PortalConfiguration/GetTimegateConfiguration', {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json'
            }
        });



        if (!response.ok) {
            loadError = true
        } else {
            let data = await response.json();

            $("#chkTimegateUsesUserField").dxSwitch("instance").option("value", data.CustomUserFieldEnabled);
            $("#timegateUserFieldId").dxSelectBox("instance").option("value", data.CustomUserFieldEnabled ? '' + data.UserFieldId : null);
            if (data != null && data.BackgroundConfiguration != null) {
                fileContentTG = data.BackgroundConfiguration.Image;
                if (fileContentTG != null && fileContentTG != "") {
                    $("#dropzonetg-text")[0].style.display = "none";
                    $("#dropzonetg-image")[0].src = fileContentTG;
                    $("#btnDeleteTGImage")[0].style.display = "";
                } else {
                    $("#dropzonetg-text")[0].style.display = "";
                    $("#dropzonetg-image")[0].src = "";
                    $("#btnDeleteTGImage")[0].style.display = "none";
                }
                $('#selectTGLeftColor').dxColorBox('instance').option('value', data.BackgroundConfiguration.LeftColor);
                $('#selectTGRightColor').dxColorBox('instance').option('value', data.BackgroundConfiguration.RightColor);
                $('#selectPositionTimeGateBackground').dxSelectBox('instance').option('value', data.BackgroundConfiguration.Position);
                $('#selectOpacityTimegateBackground').dxSlider('instance').option('value', data.BackgroundConfiguration.Opacity);
                opacity = parseInt(data.BackgroundConfiguration.Opacity);
                position = parseInt(data.BackgroundConfiguration.Position);
                setTGImagePreview(fileContentTG, position);
                setTGColorPreview(data.BackgroundConfiguration.LeftColor, data.BackgroundConfiguration.RightColor);
            } else {
                if (data == null || data.BackgroundConfiguration == null) {
                    colorLeft = "#0046fe";
                    colorRight = "#57aeeb";
                    fileContent = "";
                    $("#dropzonetg-text")[0].style.display = "";
                    $("#dropzonetg-image")[0].src = "";
                    $("#btnDeleteTGImage")[0].style.display = "none";
                    $('#selectTGLeftColor').dxColorBox('instance').option('value', colorLeft);
                    $('#selectTGRightColor').dxColorBox('instance').option('value', colorRight);
                    $('#selectPositionTimeGateBackground').dxSelectBox('instance').option('value', 1);                    
                    opacity = 10;
                    position = 1;
                    $('#selectOpacityTimegateBackground').dxSlider('instance').option('value', "10");
                    setTGImagePreview(fileContentTG, position);
                    setTGColorPreview(colorLeft, colorRight);                    
                }
            }
        }
    }

    if (loadError) {
        //show error
    }
    
    loading = false;
}

function getProgressBarInstance() {
    return $("#upload-progress").dxProgressBar("instance");
}

function getProgressBarTGInstance() {
    return $("#uploadtg-progress").dxProgressBar("instance");
}

function fileUploader_dropZoneEnter(e) {
    if (e.dropZoneElement.id === "dropzone-external")
        toggleDropZoneActive(e.dropZoneElement, true);
}

function fileUploaderTG_dropZoneEnter(e) {
    if (e.dropZoneElement.id === "dropzonetg-external")
        toggleDropZoneTGActive(e.dropZoneElement, true);
}

function fileUploader_dropZoneLeave(e) {
    if (e.dropZoneElement.id === "dropzone-external")
        toggleDropZoneActive(e.dropZoneElement, false);
}
function fileUploaderTG_dropZoneLeave(e) {
    if (e.dropZoneElement.id === "dropzonetg-external")
        toggleDropZoneTGActive(e.dropZoneElement, false);
}

function fileUploader_uploaded(e) {
    if (!loading) hasPortalChanges = true;

    const file = e.file;
    const fileReader = new FileReader();
    fileReader.onload = function () {
        toggleDropZoneActive($("#dropzone-external")[0], false);
        $("#dropzone-image")[0].src = fileReader.result;
        fileContent = fileReader.result;

        setImagePreview(fileContent, position);
    }
    fileReader.readAsDataURL(file);
    $("#dropzone-text")[0].style.display = "none";
    $("#btnDeleteImage")[0].style.display = "";
    getProgressBarInstance().option({
        visible: false,
        value: 0
    });
}

function fileUploaderTG_uploaded(e) {
    if (!loading) hasTimegateChanges = true;

    const file = e.file;
    const fileReader = new FileReader();
    fileReader.onload = function () {
        toggleDropZoneTGActive($("#dropzonetg-external")[0], false);
        $("#dropzonetg-image")[0].src = fileReader.result;
        fileContentTG = fileReader.result;

        setTGImagePreview(fileContentTG, position);
    }
    fileReader.readAsDataURL(file);
    $("#dropzonetg-text")[0].style.display = "none";
    $("#btnDeleteTGImage")[0].style.display = "";
    getProgressBarTGInstance().option({
        visible: false,
        value: 0
    });
}

function fileUploader_progress(e) {
    getProgressBarInstance().option("value", e.bytesLoaded / e.bytesTotal * 100);
}

function fileUploaderTG_progress(e) {
    getProgressBarTGInstance().option("value", e.bytesLoaded / e.bytesTotal * 100);
}

function fileUploader_uploadStarted() {
    toggleImageVisible(false);
    getProgressBarInstance().option("visible", true);
}

function fileUploaderTG_uploadStarted() {
    toggleTGImageVisible(false);
    getProgressBarTGInstance().option("visible", true);
}

function toggleDropZoneActive(dropZone, isActive) {
    if (!loading) hasPortalChanges = true;

    if (isActive) {
        dropZone.classList.add("dx-theme-accent-as-border-color");
        dropZone.classList.remove("dx-theme-border-color");
        dropZone.classList.add("dropzone-active");
    } else {
        dropZone.classList.remove("dx-theme-accent-as-border-color");
        dropZone.classList.add("dx-theme-border-color");
        dropZone.classList.remove("dropzone-active");
    }
}

function toggleDropZoneTGActive(dropZone, isActive) {
    if (!loading) hasTimegateChanges = true;

    if (isActive) {
        dropZone.classList.add("dx-theme-accent-as-border-color");
        dropZone.classList.remove("dx-theme-border-color");
        dropZone.classList.add("dropzone-active");
    } else {
        dropZone.classList.remove("dx-theme-accent-as-border-color");
        dropZone.classList.add("dx-theme-border-color");
        dropZone.classList.remove("dropzone-active");
    }
}

function toggleImageVisible(visible) {
    $("#dropzone-image")[0].hidden = !visible;
}

function toggleTGImageVisible(visible) {
    $("#dropzonetg-image")[0].hidden = !visible;
}

function fileUploaderReset() {
    $("#dropzone-text")[0].style.display = "";
    $("#dropzone-image")[0].src = "";
    $("#btnDeleteImage")[0].style.display = "none";
    fileContent = null;
    toggleImageVisible(true);
    setImagePreview(null, 3);
}

function fileUploaderTGReset() {
    $("#dropzonetg-text")[0].style.display = "";
    $("#dropzonetg-image")[0].src = "";
    $("#btnDeleteTGImage")[0].style.display = "none";
    fileContentTG = null;
    toggleTGImageVisible(true);
    setTGImagePreview(null, 3);
}