var drawedRectangle, drawedPolygon, openPopUp, drawingmanager;
var ipRestrictionEnabled;


function maxCapacity(value) {
    if (value.value) {
        $("#txtZoneCapacity").dxTextBox("instance").option("disabled", false);
        $('.divShowZoneCapacity').show();
    }
    else {
        $("#txtZoneCapacity").dxTextBox("instance").option("disabled", true);
        $('#txtZoneCapacity').dxTextBox("option", "value", null);
        $('#chkShowZoneCapacity').dxSwitch("option", "value", null);
        $('#selectZoneSupervisor').dxSelectBox("option", "value", null);
        $('.divShowZoneCapacity').hide();
    }
}

function openAddNewZonePopUp() {
    clearEditFields();
    $("#editZonePopup").dxPopup("instance").show();
    $('#btnEditZone').hide();
    $('#btnAddNewZone').show();
}



function popUpHidden() {
    openPopUp = false;
}

function popUpShown() {
    openPopUp = true;

    let workingMode = $("#ckZoneLocationWorkingMode").dxSwitch("instance");
    ipRestrictionEnabled = workingMode.option("value");

    if (ipRestrictionEnabled) {
        $(".editZoneMap").hide();
        $(".editZoneIps").show(); 
    } else {
        $(".editZoneMap").show();
        $(".editZoneIps").hide(); 
    }


    $("#map").dxMap("instance").repaint();

    $("#configDiv").show();
    $("#advancedDiv").hide();
    $("#zoneConfiguration").addClass("bTabZones-active")
    $("#zoneAdvanced").removeClass("bTabZones-active")
    $("#zoneAdvanced").addClass("bTabZones")

    $('#zoneConfiguration').off("click");
    $('#zoneConfiguration').on("click", function (e) {
        $("#configDiv").show();
        $("#advancedDiv").hide();
        $("#zoneConfiguration").addClass("bTabZones-active")
        $("#zoneAdvanced").removeClass("bTabZones-active")
    });

    $('#zoneAdvanced').off("click");
    $('#zoneAdvanced').on("click", function (e) {
        $("#configDiv").hide();
        $("#advancedDiv").show();
        $("#zoneConfiguration").removeClass("bTabZones-active")
        $("#zoneAdvanced").addClass("bTabZones-active")
    });

    if ($('#gridInactiviyZones').dxDataGrid('instance').getDataSource() == null) {
        $("#gridInactiviyZones").dxDataGrid({ dataSource: new Array() });
    }

    if ($('#gridExceptionZones').dxDataGrid('instance').getDataSource() == null) {
        $("#gridExceptionZones").dxDataGrid({ dataSource: new Array() });
    }

    if ($('#gridIpRestrictions').dxDataGrid('instance').getDataSource() == null) {
        $("#gridIpRestrictions").dxDataGrid({ dataSource: new Array() });
    }
}

function decimalToRgb(decimal) {
    return {
        red: (decimal >> 16) & 0xff,
        green: (decimal >> 8) & 0xff,
        blue: decimal & 0xff,
    };
}

function componentToHex(c) {
    let hex = c.toString(16);
    return hex.length == 1 ? "0" + hex : hex;
}

function rgbToHex(r, g, b) {
    return "#" + componentToHex(r) + componentToHex(g) + componentToHex(b);
}

function openEditZonePopUp() {
    clearEditFields();
    $.ajax({
        type: "GET",
        url: BASE_URL + 'Zones/LoadInfoZone',
        contentType: "application/json; charset=utf-8",
        data: { id: $('#idZoneEdit').val() },
        success: function (result) {
            for (const element of result.ZonesInactivity) {
                element.Begin = new Date(parseInt(element.Begin.substr(6)));
                element.End = new Date(parseInt(element.End.substr(6)));
            }
            $("#gridInactiviyZones").dxDataGrid({
                dataSource: result.ZonesInactivity
            });
            for (const element of result.ZonesException) {
                element.ExceptionDate = new Date(parseInt(element.ExceptionDate.substr(6)));
            }

            $("#gridExceptionZones").dxDataGrid({
                dataSource: result.ZonesException
            });
            $('#txtName').dxTextBox("option", "value", result.Name);
            $('#txtZoneCapacity').dxTextBox("option", "value", result.Capacity);
            if (result.Capacity != null) {
                $("#chkZoneCapacity").dxCheckBox("instance").option("value", true);
            }
            else {
                $("#chkZoneCapacity").dxCheckBox("instance").option("value", false);
            }
            $('#chkShowZoneCapacity').dxSwitch("option", "value", result.CapacityVisible);
            $('#chkZoneNameAsLocation').dxSwitch("option", "value", result.ZoneNameAsLocation);
            $('#chkIsEmergencyZone').dxSwitch("option", "value", result.IsEmergencyZone);
            let rgbColor = decimalToRgb(parseInt(result.Color));
            $('#selectColor').dxColorBox("option", "value", rgbToHex(rgbColor.red, rgbColor.green, rgbColor.blue));
            $('#selectTimeZone').dxSelectBox("option", "value", result.DefaultTimezone);
            $('#selectTelecommutingType').dxSelectBox("option", "value", result.TelecommutingZoneType);
            let currentWorkCenters = $("#selectWorkCenter").dxSelectBox("getDataSource")._store._array;
            if (result.WorkCenter != null && currentWorkCenters.filter(e => e.Name === result.WorkCenter).length <= 0) {
                currentWorkCenters.push({ Name: result.WorkCenter });
                $("#selectWorkCenter").dxSelectBox({
                    dataSource: currentWorkCenters
                });
            }

            $('#selectWorkCenter').dxSelectBox("option", "value", result.WorkCenter);
            $('#selectZoneSupervisor').dxSelectBox("option", "value", result.Supervisor);
            $('#txtDescription').dxTextArea("option", "value", result.Description);
            $('#selectType').dxSwitch("option", "value", result.IsWorkingZone);
            if (result.GoogleMapInfo != null) {
                let shape = result.GoogleMapInfo.Shape;
                if (shape == _RECTANGLE_SHAPE) {
                    $('#googleRectCords').val(dbCoordsArray2JSON(result.GoogleMapInfo.Coordinates));
                    $('#googlePolCords').val('');
                    $('#zoneShape').val(_RECTANGLE_SHAPE);
                }
                else if (shape == _POLYGON_SHAPE) {
                    $('#googlePolCords').val(dbCoordsArray2JSON(result.GoogleMapInfo.Coordinates));
                    $('#googleRectCords').val('');
                    $('#zoneShape').val(_POLYGON_SHAPE);
                }
                $('#googleMapCenter').val(JSON.stringify({ Latitud: result.GoogleMapInfo.Center.Latitud, Longitud: result.GoogleMapInfo.Center.Longitud }));
                $('#googleMapZoom').val(result.GoogleMapInfo.Zoom);
                $('#zoneArea').val(result.Area);
            }

            let ipsArray = [];
            if (result.IpsRestriction != null && result.IpsRestriction.length > 0) ipsArray = result.IpsRestriction.map(function (x) { return { Ip: x } });

            $("#gridIpRestrictions").dxDataGrid({
                dataSource: ipsArray
            });
        },
        error: function (xhr, ajaxOptions, thrownError) {
            clearEditFields();
        }
    });

    $("#editZonePopup").dxPopup("instance").show();
    $('#btnEditZone').show();
    $('#btnAddNewZone').hide();
}

function calculateWeekDay(cellInfo) {
    let weekDays = [viewUtilsManager.DXTranslate('sunday'), viewUtilsManager.DXTranslate('monday'), viewUtilsManager.DXTranslate('tuesday'), viewUtilsManager.DXTranslate('wednesday'), viewUtilsManager.DXTranslate('thursday'), viewUtilsManager.DXTranslate('friday'), viewUtilsManager.DXTranslate('saturday')]
    return weekDays[cellInfo.value];
}

function editTimeCell(cellElement, cellInfo) {
    $("<div>")
        .appendTo(cellElement)
        .dxDateBox({
            type: "time",
            onValueChanged: function (e) {
                cellInfo.setValue(e.value);
            }
        });
}

function editDateCell(cellElement, cellInfo) {
    $("<div>")
        .appendTo(cellElement)
        .dxDateBox({
            type: "date",
            onValueChanged: function (e) {
                cellInfo.setValue(e.value);
            }
        });
}

function editSelectDayCell(cellElement, cellInfo) {
    const data = [
        {
            ID: 1,
            Name: viewUtilsManager.DXTranslate('monday')
        }, {
            ID: 2,
            Name: viewUtilsManager.DXTranslate('tuesday')
        }, {
            ID: 3,
            Name: viewUtilsManager.DXTranslate('wednesday')
        }, {
            ID: 4,
            Name: viewUtilsManager.DXTranslate('thursday')
        }, {
            ID: 5,
            Name: viewUtilsManager.DXTranslate('friday')
        },
        {
            ID: 6,
            Name: viewUtilsManager.DXTranslate('saturday')
        },
        {
            ID: 0,
            Name: viewUtilsManager.DXTranslate('sunday')
        }]


    $("<div>")
        .appendTo(cellElement)
        .dxSelectBox({
            dataSource: data,
            valueExpr: "ID",
            displayExpr: "Name",
            onValueChanged: function (e) {
                cellInfo.setValue(e.value);
            },

        });
}


function initializeCenter(mapObj) {
    if ($('#googleMapCenter') != undefined && $('#googleMapCenter').val() != "" && $('#googleMapCenter').val() != null && $('#googleMapCenter').val() != undefined) {
        let center = JSON.parse($('#googleMapCenter').val());
        mapObj.setCenter(new google.maps.LatLng(center.Latitud, center.Longitud));
    }
    else {
        centerCurrentLocation();
    }
}


function initializeZoom(map) {
    map.setZoom(parseInt($('#googleMapZoom').val()));
}

function CleanMapControl(controlDiv, map) {

    let controlUI = document.createElement('div');
    controlUI.style.backgroundColor = '#FFFFFF';
    controlUI.style.borderStyle = 'solid';
    controlUI.style.borderWidth = '3px';
    controlUI.style.borderColor = '#ccc';
    controlUI.style.height = '27px';
    controlUI.style.marginTop = '5px';
    controlUI.style.marginLeft = '-52px';
    controlUI.style.paddingTop = '3px';
    controlUI.style.cursor = 'pointer';
    controlUI.style.textAlign = 'center';
    controlDiv.appendChild(controlUI);


    let controlText = document.createElement('div');
    controlText.style.fontSize = '10px';
    controlText.style.paddingLeft = '4px';
    controlText.style.paddingRight = '4px';
    controlText.innerHTML = '<img src="Base/Images/tbt/delete.png"/>';
    controlUI.appendChild(controlText);

    google.maps.event.addDomListener(controlUI, 'click', function () {
        cleanMap();
    });
}

function addCleanMapControl(map) {
    let customControlDiv = document.createElement('div');
    CleanMapControl(customControlDiv, map);

    customControlDiv.index = 1;
    map.controls[google.maps.ControlPosition.TOP_RIGHT].push(customControlDiv);
}

function initializeRectangleZone(map) {
    if ($('#googleRectCords').val() != null && $('#googleRectCords').val() != '') {
        drawingmanager.setMap(null);
        addCleanMapControl(map);
        let rectCoordsArray = JSON.parse($('#googleRectCords').val());
        let rectBounds = new google.maps.LatLngBounds(
            new google.maps.LatLng(parseFloat(rectCoordsArray[1].Latitud), parseFloat(rectCoordsArray[1].Longitud)),
            new google.maps.LatLng(parseFloat(rectCoordsArray[0].Latitud), parseFloat(rectCoordsArray[0].Longitud))
        );

        drawedRectangle = new google.maps.Rectangle({
            bounds: rectBounds,
            map: map
        });
        let selectedColor = $('#selectColor').dxColorBox("option", "value");
        let selectedColorOptions = {
            strokeColor: selectedColor,
            fillColor: selectedColor,
            fillOpacity: 0.2
        };
        updateDrawedRectangle(selectedColorOptions);
    }
}

function initializePolygonZone(map) {
    if ($('#googlePolCords').val() != null && $('#googlePolCords').val() != '') {
        drawingmanager.setMap(null);
        let polCoordsArray = JSON.parse($('#googlePolCords').val());
        const polygonCoords = [];
        for (const element of polCoordsArray) {
            polygonCoords.push(new google.maps.LatLng(parseFloat(element.Latitud), parseFloat(element.Longitud)))
        }

        drawedPolygon = new google.maps.Polygon({
            paths: polygonCoords,
            map: map
        });
        let selectedColor = $('#selectColor').dxColorBox("option", "value");
        let selectedColorOptions = {
            strokeColor: selectedColor,
            fillColor: selectedColor,
            fillOpacity: 0.2
        };
        updateDrawedPolygon(selectedColorOptions);
    }
}

function updateDrawingManagerControlColor(selectedColorOptions) {
    if (drawingmanager != null) {
        drawingmanager.setOptions({
            rectangleOptions: selectedColorOptions,
            polygonOptions: selectedColorOptions
        });
    }
}

function updateDrawedRectangle(selectedColorOptions) {
    drawedRectangle.setOptions(
        selectedColorOptions);
}

function updateDrawedPolygon(selectedColorOptions) {
    drawedPolygon.setOptions(
        selectedColorOptions);
}

function onColorChanged(e) {
    let selectedColor = e.value;
    let selectedColorOptions = {
        strokeColor: selectedColor,
        fillColor: selectedColor,
        fillOpacity: 0.2
    };
    updateDrawingManagerControlColor(selectedColorOptions);

    if (drawedRectangle != null) {
        updateDrawedRectangle(selectedColorOptions);
    }
    if (drawedPolygon != null) {
        updateDrawedPolygon(selectedColorOptions);
    }
}

function cleanMap() {
    $('#googlePolCords').val(null);
    $('#googleRectCords').val(null);
    $('#googleMapCenter').val(null);
    $('#googleMapZoom').val(10);
    $('#zoneShape').val(null);
    $('#zoneArea').val(null);
    let map = $("#map").dxMap("instance");
    if (map != null)
        map.repaint();
}

function clearEditFields() {
    $('#txtName').dxTextBox("option", "value", null);
    $('#selectColor').dxColorBox("option", "value", null);
    $('#selectTimeZone').dxSelectBox("option", "value", null);
    $('#selectTelecommutingType').dxSelectBox("option", "value", 2);
    $('#selectWorkCenter').dxSelectBox("option", "value", null);
    $('#selectZoneSupervisor').dxSelectBox("option", "value", null);
    $('#txtDescription').dxTextArea("option", "value", null);
    $('#selectType').dxSwitch("option", "value", true);
    $('#chkShowZoneCapacity').dxSwitch("option", "value", null);
    $('#chkZoneNameAsLocation').dxSwitch("option", "value", null);
    $('#chkZoneCapacity').dxCheckBox("option", "value", null);
    $('#txtZoneCapacity').dxTextBox("option", "value", null);
    cleanMap();

    $("#gridInactiviyZones").dxDataGrid({ dataSource: new Array() });
    $("#gridExceptionZones").dxDataGrid({ dataSource: new Array() });
    $("#gridIpRestrictions").dxDataGrid({ dataSource: new Array() });

}

function cancelZone() {
    $("#editZonePopup").dxPopup("instance").hide();
}

function getPolygonPath(polygonCoordinates) {
    return polygonCoordinates.join('|').replace(/\(/g, '').replace(/\)/g, '').replace(/\ /g, '') + "|" + polygonCoordinates[0].toString().replace(/\(/g, '').replace(/\)/g, '');
}

function getRectanglePath(southWestBound, northEastBound) {
    return southWestBound.lat() + "," + southWestBound.lng() + "|" + southWestBound.lat() + "," + northEastBound.lng() + "|" + northEastBound.lat() + "," + northEastBound.lng() + "|" + northEastBound.lat() + "," + southWestBound.lng() + "|" + southWestBound.lat() + "," + southWestBound.lng();
}

function setSearchBar(map) {
    const input = document.createElement("input");
    input.placeholder = viewUtilsManager.DXTranslate('roZoneGoogleSearch');
    const searchBox = new google.maps.places.SearchBox(input);


    map.controls[google.maps.ControlPosition.TOP_LEFT].push(input);

    map.addListener("bounds_changed", () => {
        searchBox.setBounds(map.getBounds());
    });

    let markers = [];


    searchBox.addListener("places_changed", () => {
        const places = searchBox.getPlaces();

        if (places.length == 0) {
            return;
        }

        // Clear out the old markers.
        markers.forEach((marker) => {
            marker.setMap(null);
        });
        markers = [];

        // For each place, get the icon, name and location.
        const bounds = new google.maps.LatLngBounds();

        places.forEach((place) => {
            if (!place.geometry || !place.geometry.location) {
                return;
            }

            const icon = {
                url: place.icon,
                size: new google.maps.Size(71, 71),
                origin: new google.maps.Point(0, 0),
                anchor: new google.maps.Point(17, 34),
                scaledSize: new google.maps.Size(25, 25),
            };

            // Create a marker for each place.
            markers.push(
                new google.maps.Marker({
                    map,
                    icon,
                    title: place.name,
                    position: place.geometry.location,
                })
            );
            if (place.geometry.viewport) {
                // Only geocodes have viewport.
                bounds.union(place.geometry.viewport);
            } else {
                bounds.extend(place.geometry.location);
            }
        });
        map.fitBounds(bounds);
    });

}

function createMarker(place) {
    if (!place.geometry || !place.geometry.location) return;

    const marker = new google.maps.Marker({
        map,
        position: place.geometry.location,
    });

    google.maps.event.addListener(marker, "click", () => {
        infowindow.setContent(place.name || "");
        infowindow.open(map);
    });
}

function onMapReady(e) {
    if (openPopUp) {
        let map = e.originalMap;
        setSearchBar(map);
        drawingmanager = new google.maps.drawing.DrawingManager();
        drawingmanager.setOptions({
            drawingControl: true,
            drawingControlOptions: {
                position: google.maps.ControlPosition.TOP_RIGHT,
                drawingModes: [google.maps.drawing.OverlayType.RECTANGLE]
            },
        });
        drawingmanager.setMap(map);
        let selectedColor = $('#selectColor').dxColorBox("option", "value");
        let selectedColorOptions = {
            strokeColor: selectedColor,
            fillColor: selectedColor,
            fillOpacity: 0.2
        };
        if (selectedColor != null && selectedColor != undefined)
            updateDrawingManagerControlColor(selectedColorOptions);


        initializeZoom(map);
        initializeCenter(map);
        initializeRectangleZone(map);
        initializePolygonZone(map);

        google.maps.event.addListener(drawingmanager, 'polygoncomplete', function (polygon) {
            drawedPolygon = polygon;
            let polygonCoordinates = (polygon.getPath().getArray());
            drawingmanager.setDrawingMode(null);
            drawingmanager.setMap(null);
            $('input#googlePolCords').val(mapCoordsArray2JSON(polygonCoordinates));
            $('input#zoneShape').val(google.maps.drawing.OverlayType.POLYGON);
            $('#zoneArea').val(google.maps.geometry.spherical.computeArea(polygonCoordinates));
        });

        google.maps.event.addListener(drawingmanager, 'rectanglecomplete', function (rectangle) {
            addCleanMapControl(map);
            drawedRectangle = rectangle;
            let bounds = rectangle.getBounds();
            let northEastBound = bounds.getNorthEast();
            let southWestBound = bounds.getSouthWest();
            drawingmanager.setDrawingMode(null);
            drawingmanager.setMap(null);
            $('input#googleRectCords').val(mapCoordsArray2JSON([northEastBound, southWestBound]));
            $('input#zoneShape').val(google.maps.drawing.OverlayType.RECTANGLE);

            $('#zoneArea').val(google.maps.geometry.spherical.computeArea([new google.maps.LatLng(northEastBound.lat(), northEastBound.lng()),
                new google.maps.LatLng(northEastBound.lat(), southWestBound.lng()),
                new google.maps.LatLng(southWestBound.lat(), southWestBound.lng()),
                new google.maps.LatLng(southWestBound.lat(), northEastBound.lng())]));
        });


        google.maps.event.addListener(map, 'center_changed', function () {

            if (map.getCenter().lat() != 0 || map.getCenter().lng() != 0) {
                $('#googleMapCenter').val(JSON.stringify({ Latitud: map.getCenter().lat(), Longitud: map.getCenter().lng() }));
            }
        });

        google.maps.event.addListener(map, 'zoom_changed', function () {
            if (map.getZoom() != null && map.getZoom() != undefined && !isNaN(map.getZoom()))
                $('#googleMapZoom').val(map.getZoom());
        });
    }
}

function centerCurrentLocation() {
    if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition(function (position) {
            let initialLocation = { lat: position.coords.latitude, lng: position.coords.longitude };
            if ($('#googleMapCenter') == undefined || $('#googleMapCenter').val() == "" || $('#googleMapCenter').val() == null || $('#googleMapCenter').val() == undefined) {
                $("#map").dxMap({ center: initialLocation });
                $('#googleMapCenter').val(JSON.stringify({ Latitud: position.coords.latitude, Longitud: position.coords.longitude }));
            }
        }
        );
    }
}

function onInitialized(e) { }

function dbCoordsArray2JSON(coordinates) {
    let coordinatesStr = '[';
    for (let i = 0; i < coordinates.length; i++) {
        if (i < (coordinates.length - 1))
            coordinatesStr += JSON.stringify({ Latitud: coordinates[i].Latitud, Longitud: coordinates[i].Longitud }) + ",";
        else
            coordinatesStr += JSON.stringify({ Latitud: coordinates[i].Latitud, Longitud: coordinates[i].Longitud })
    }
    coordinatesStr += ']';
    return coordinatesStr;
}

function mapCoordsArray2JSON(coordinates) {
    let coordinatesStr = '[';
    for (let i = 0; i < coordinates.length; i++) {
        if (i < (coordinates.length - 1))
            coordinatesStr += JSON.stringify({ Latitud: coordinates[i].lat(), Longitud: coordinates[i].lng() }) + ",";
        else
            coordinatesStr += JSON.stringify({ Latitud: coordinates[i].lat(), Longitud: coordinates[i].lng() })
    }
    coordinatesStr += ']';
    return coordinatesStr;
}


function getZoneObject(isNew) {

    let selectedZone = $('input#idZoneEdit').val();

    let selectedName = $('#txtName').dxTextBox('instance').option('value');
    let selectedColor = $('#selectColor').dxColorBox('instance').option('value');
    let selectedTimeZone = $('#selectTimeZone').dxSelectBox('instance').option('value');
    let selectedTelecommtingType = $('#selectTelecommutingType').dxSelectBox('instance').option('value');
    let selectedWorkCenter = $('#selectWorkCenter').dxSelectBox('instance').option('value');
    let selectedZoneSupervisor = $('#selectZoneSupervisor').dxSelectBox('instance').option('value');
    let selectedDescription = $('#txtDescription').dxTextArea('instance').option('value');
    let selectedType = $('#selectType').dxSwitch('instance').option('value');
    let selectedCapacity = $('#txtZoneCapacity').dxTextBox('instance').option('value');
    let selectedCapacityVisible = $("#chkShowZoneCapacity").dxSwitch("instance").option("value");
    let selectedZoneNameAsLocation = $("#chkZoneNameAsLocation").dxSwitch("instance").option("value");
    let selectedEmergencyZone = $("#chkIsEmergencyZone").dxSwitch("instance").option("value");
    let selectedGooglePolCords = $('input#googlePolCords').val();
    let selectedGoogleRectCords = $('input#googleRectCords').val();
    let selectedInactivityZones = $('#gridInactiviyZones').dxDataGrid('instance').getDataSource().items();
    let selectedExceptionZones = $('#gridExceptionZones').dxDataGrid('instance').getDataSource().items();
    let selectedCenter = "";
    let selectedZoom = "";
    let selectedShape = $('#zoneShape').val();
    let coordinates = null;
    let selectedArea = $('#zoneArea').val();
    let mapInfo = null;


    if ($('#googleMapCenter') != undefined && $('#googleMapCenter').val() != "") selectedCenter = JSON.parse($('#googleMapCenter').val());
    if ($('#googleMapZoom').val() != null && $('#googleMapZoom').val() != undefined && !isNaN($('#googleMapZoom').val())) selectedZoom = JSON.parse($('#googleMapZoom').val());
    if (selectedGooglePolCords != null && selectedGooglePolCords != '') coordinates = JSON.parse(selectedGooglePolCords);
    if (selectedGoogleRectCords != null && selectedGoogleRectCords != '') coordinates = JSON.parse(selectedGoogleRectCords);
    if (coordinates != null) mapInfo = { Center: selectedCenter, Zoom: selectedZoom, Shape: selectedShape, Coordinates: coordinates };

    let selectedIps = [];

    let ipsArray = $('#gridIpRestrictions').dxDataGrid('instance').getDataSource().items();
    if (ipsArray != null && ipsArray.length > 0) {
        selectedIps = ipsArray.map(function (x) { return x.Ip });
    }

    return {
        ID: isNew ? -1 : selectedZone,
        Name: selectedName,
        Capacity: selectedCapacity,
        CapacityVisible: selectedCapacityVisible,
        Color: selectedColor,
        DefaultTimezone: selectedTimeZone,
        WorkCenter: selectedWorkCenter,
        Description: selectedDescription,
        IsWorkingZone: selectedType,
        IsEmergencyZone: selectedEmergencyZone,
        GoogleMapInfo: mapInfo,
        Area: selectedArea,
        ZoneNameAsLocation: selectedZoneNameAsLocation,
        ZonesInactivity: selectedInactivityZones,
        ZonesException: selectedExceptionZones,
        TelecommutingZoneType: selectedTelecommtingType,
        Supervisor: selectedZoneSupervisor,
        IpsRestriction: selectedIps
    }
}



function addNewZone() {

    let zone = getZoneObject(true);

    let workingMode = $("#ckZoneLocationWorkingMode").dxSwitch("instance");
    let ipRestrictionEnabled = workingMode.option("value");

    let zoneError = false;
    if (ipRestrictionEnabled) {
        zoneError = (zone.IpsRestriction == null || (zone.IpsRestriction != null && zone.IpsRestriction.length == 0));
    } else {
        zoneError = (zone.GoogleMapInfo == null);
    }

    if (zone.Name != "" && zone.DefaultTimezone != null && !zoneError) {
        $.ajax({
            type: "POST",
            url: BASE_URL + 'Zones/InsertOrUpdateZone',
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify(zone),
            success: function () {
                DevExpress.ui.notify("¡Zona creada!", "success", 1000);
                clearEditFields();
                RefreshZonesList();
                $("#editZonePopup").dxPopup("instance").hide();
            },
            error: function (xhr, ajaxOptions, thrownError) {
                DevExpress.ui.notify(thrownError, "error", 3000);
            }
        });
    }
    else {
        if (zone.Name == "") DevExpress.ui.notify(viewUtilsManager.DXTranslate('roZoneSaveErrorName'), "error", 3000);
        else if (zone.DefaultTimezone == null) DevExpress.ui.notify(viewUtilsManager.DXTranslate('roZoneSaveErrorTimeZone'), "error", 3000);
        else if (zoneError) DevExpress.ui.notify(viewUtilsManager.DXTranslate('roZoneSaveErrorGoogleCoords'), "error", 3000);
    }
}



function saveEditedZone() {

    let zone = getZoneObject(false);

    let workingMode = $("#ckZoneLocationWorkingMode").dxSwitch("instance");
    let ipRestrictionEnabled = workingMode.option("value");

    let zoneError = false;
    if (ipRestrictionEnabled) {
        zoneError = (zone.IpsRestriction == null || (zone.IpsRestriction != null && zone.IpsRestriction.length == 0));
    } else {
        zoneError = (zone.GoogleMapInfo == null);
    }

    if (zone.Name != "" && zone.DefaultTimezone != null && !zoneError) {
        $.ajax({
            type: "POST",
            url: BASE_URL + 'Zones/InsertOrUpdateZone',
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify(zone),
            success: function () {
                DevExpress.ui.notify("¡Zona actualizada!", "success", 1000);
                clearEditFields();
                RefreshZonesList();
                $("#editZonePopup").dxPopup("instance").hide();
            },
            error: function (xhr, ajaxOptions, thrownError) {
                DevExpress.ui.notify(thrownError, "error", 3000);
            }

        });
    }
    else {
        if (zone.Name == "") DevExpress.ui.notify(viewUtilsManager.DXTranslate('roZoneSaveErrorName'), "error", 3000);
        else if (zone.DefaultTimezone == null) DevExpress.ui.notify(viewUtilsManager.DXTranslate('roZoneSaveErrorTimeZone'), "error", 3000);
        else if (zoneError) DevExpress.ui.notify(viewUtilsManager.DXTranslate('roZoneSaveErrorGoogleCoords'), "error", 3000);
    }
}

function inactivityZoneUpdating(e) {
    if (e.newData == null) {
        e.cancel = true;
        DevExpress.ui.notify(viewUtilsManager.DXTranslte('errorInactivityZones'), "error", 3000);
    }
    else if (e.newData.Begin != null) {
        if ((e.newData.End != null && e.newData.Begin >= e.newData.End) || (e.newData.End == null && e.newData.Begin >= e.oldData.End))
            e.cancel = true;
        DevExpress.ui.notify(viewUtilsManager.DXTranslate('incorrectDates'), "error", 3000);
        e.component.cancelEditData();
    }
    else if (e.newData.End != null) {
        if ((e.newData.Begin != null && e.newData.Begin >= e.newData.End) || (e.newData.Begin == null && e.oldData.Begin >= e.newData.End))
            e.cancel = true;
        DevExpress.ui.notify(viewUtilsManager.DXTranslate('incorrectDates'), "error", 3000);
        e.component.cancelEditData();
    }
}

function checkDaySelected(params) {
    return params.data.WeekDay != null;
}

function inactivityZoneInserting(e) {
    e.data.ChildData = [];
    if (e.data == null || e.data.WeekDay == null || e.data.Begin == null || e.data.End == null) {
        e.cancel = true;
        DevExpress.ui.notify(viewUtilsManager.DXTranslate('errorInactivityZones'), "error", 3000);
        e.component.cancelEditData();
    }
    else if (e.data.Begin >= e.data.End) {
        e.cancel = true;
        DevExpress.ui.notify(viewUtilsManager.DXTranslate('incorrectDates'), "error", 3000);
        e.component.cancelEditData();
    }
}

function inactivityZonesNewRow(e) {
    e.data.IDZone = $('#idZoneEdit').val();
}

function exceptionZoneInserting(e) {
    e.data.ChildData = [];
    if (e.data == null || e.data.ExceptionDate == null) {
        e.cancel = true;
        DevExpress.ui.notify(viewUtilsManager.DXTranslate('errorExceptionZones'), "error", 3000);
        e.component.cancelEditData();
    }
}

function exceptionZonesNewRow(e) {
    e.data.IDZone = $('#idZoneEdit').val();
}

function onTimeZoneOpened(e) {
    let list = e.component._list;
    if (!list.option('useNativeScrolling')) {
        list.option('useNativeScrolling', true);
        list._scrollView.option('useNative', true);
        list.reload();
    }
}