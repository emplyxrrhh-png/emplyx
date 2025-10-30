var drawedMainMapRectangle, drawedMainMapPolygon, globalMap, allzones = [], isFirstShape, dynamicZoomBounds = [];
var firstLoad = true;

const _RECTANGLE_SHAPE = "rectangle";
const _POLYGON_SHAPE = "polygon";

function getWidth() {
    return document.documentElement.clientWidth - 40;
}

function getHeight() {
    return document.documentElement.clientHeight - 40;
}

function selection_changed(selectedItems) {
    if (selectedItems.columnIndex == 0) {
        var idEmployee = selectedItems.row.data.IdEmployee;
        window.open('../#/' + RootUrl + '/Employees/Employees?IDEmployee=' + idEmployee, "_blank"); return false;
    }
}

function updateComboValue(id) {
    $('#ZonesList').dxSelectBox('instance').getDataSource().load().done(function (res) {
        for (i = 0; i < res.length; i++) {
            if (res[i].ID == id) {
                $('#ZonesList').dxSelectBox('instance').option("value", res[i]);
                return;
            }
        }
    });
}

function refreshGrid(e) {
    if (e.value != null) {
        localStorage.setItem("zoneSelected", $("#ZonesList").dxSelectBox('instance').option('value').ID);

        try {
            $("#gridStatusEmployees").dxDataGrid("instance").refresh();
            $("#gridStatusEmployeesLastHour").dxDataGrid("instance").refresh();
        }
        catch (err) {
        }

        if (e.value.GoogleMapInfo != null) {
            var zonaElegida = ((e.value.GoogleMapInfo.Coordinates[0].Latitud + e.value.GoogleMapInfo.Coordinates[1].Latitud) / 2) + "," + ((e.value.GoogleMapInfo.Coordinates[0].Longitud + e.value.GoogleMapInfo.Coordinates[1].Longitud) / 2);

            localStorage.setItem("zoneSelectedCoords", zonaElegida);

            var mapWidget = $("#globalMap").dxMap("instance");
            var markersData = mapWidget.option("markers");
            var newMarkers = $.map(markersData, function (item) {
                if (item.location == zonaElegida) {
                    return $.extend(true, {}, item, { tooltip: { isShown: true } });
                }
                else {
                    return $.extend(true, {}, item, { tooltip: { isShown: false } });
                }
            });
            mapWidget.option("markers", newMarkers);
        }
    }
    else {
    }
}

function selectFirstMarker(zone) {
    var zoneSelected = localStorage.getItem("zoneSelectedCoords");

    if (zoneSelected == zone.options.location) {
        zone.options.tooltip.isShown = true;
    }
}

var firstRender = true;

function onContentReady(e) {
    var zoneSelected = localStorage.getItem("zoneSelected");
    if (firstRender) {
        e.component.getDataSource().load().done(function (res) {
            if (zoneSelected != null && zoneSelected != 'undefined' && zoneSelected != 'null' && zoneSelected != "") {
                for (i = 0; i < res.length; i++) {
                    if (res[i].ID == zoneSelected) {
                        e.component.option("value", res[i]);

                        return;
                    }
                }
            }
            else if (zoneSelected == null || zoneSelected == "") {
                e.component.option("value", res[0]);
                return;
            }
            else {
            }
        });
    }
    firstRender = false;
}

function initializeGlobalMap(e) {
    globalMap = e.originalMap;

    $("#globalMap").dxMap({ center: ZonaGlobal.MaxLatitude + "," + ZonaGlobal.MaxLongitud });

    const map = $("#globalMap").dxMap("instance");

    Zones.forEach(zone => {
        if (zone.GoogleMapInfo != null) {
            if (zone.CurrentPeopleIn > 0 || zone.CurrentPeopleOut > 0) {
                map.addMarker(
                    {
                        iconSrc: ("Base/Images/markerColorMedium.png"),
                        location: ((zone.GoogleMapInfo.Coordinates[0].Latitud + zone.GoogleMapInfo.Coordinates[1].Latitud) / 2) + "," + ((zone.GoogleMapInfo.Coordinates[0].Longitud + zone.GoogleMapInfo.Coordinates[1].Longitud) / 2),
                        tooltip: { isShown: true, text: "<strong><div>" + zone.Name + "</div></strong><br><div>" + zone.CurrentPeopleInDesc + "</div><br><div>" + zone.CurrentPeopleOutDesc + "</div><br><i><div>" + zone.CapacityDesc + "</div></i>" },
                        onClick: function () {
                            updateComboValue(zone.ID);
                        }
                    }
                )
            }
            else {
                map.addMarker(
                    {
                        iconSrc: ("Base/Images/markerGreyMedium.png"),
                        location: ((zone.GoogleMapInfo.Coordinates[0].Latitud + zone.GoogleMapInfo.Coordinates[1].Latitud) / 2) + "," + ((zone.GoogleMapInfo.Coordinates[0].Longitud + zone.GoogleMapInfo.Coordinates[1].Longitud) / 2),
                        tooltip: { isShown: false, text: "<strong><div>" + zone.Name + "</div></strong><br><div>" + zone.CurrentPeopleInDesc + "</div><br><div>" + zone.CurrentPeopleOutDesc + "</div><br><i><div>" + zone.CapacityDesc + "</div></i>" },
                        onClick: function () {
                            updateComboValue(zone.ID);
                        }
                    }
                )
            }
        }
    });
}

function updateGlobalMap(data) {
    dynamicZoomBounds = [];
    //Eliminamos todas las zonas marcadas para actualizar el mapa
    deleteAllShape();
    for (var i = 0; i < data.length; i++) {
        var googleMapInfo = data[i].GoogleMapInfo;
        if (googleMapInfo != null && globalMap != null) {
            globalMap.setCenter(new google.maps.LatLng(googleMapInfo.Center.Latitud.toString(), googleMapInfo.Center.Longitud.toString()));
            globalMap.setZoom(googleMapInfo.Zoom)
            if (googleMapInfo.Coordinates.length > 0) {
                if (googleMapInfo.Shape == _RECTANGLE_SHAPE) {
                    drawGlobalMapRectZone(googleMapInfo, data[i].Color);
                } else if (googleMapInfo.Shape == _POLYGON_SHAPE) {
                    drawGlobalMapPolyZone(googleMapInfo, data[i].Color);
                }
            }
        }
    }

    //Actualiza el zoom del mapa dinámicamente según las zonas seleccionadas
    var latLngBounds = new google.maps.LatLngBounds();
    for (var i = 0; i < dynamicZoomBounds.length; i++) {
        latLngBounds.extend(dynamicZoomBounds[i]);
    }
    if (dynamicZoomBounds.length > 0) {
        globalMap.fitBounds(latLngBounds);
    }
}

function beforeSend(operation, ajaxSettings) {
    if ($("#ZonesList").dxSelectBox('instance').option('value') != null) {
        if ($("#ZonesList").dxSelectBox('instance').option('value').ID > 0) {
            ajaxSettings.method = "POST";
            ajaxSettings.data.idZone = localStorage.getItem("zoneSelected");
        }
    }
}

function beforeSendLastHour(operation, ajaxSettings) {
    if ($("#ZonesList").dxSelectBox('instance').option('value') != null) {
        if ($("#ZonesList").dxSelectBox('instance').option('value').ID > 0) {
            ajaxSettings.method = "POST";
            ajaxSettings.data.idZone = localStorage.getItem("zoneSelected");
        }
    }
}

function hasData() {
    return false;
}

var headerFilter = {
    load: function (loadOptions) {
        return [{
            text: viewUtilsManager.DXTranslate('roPresent'),
            value: [['PresenceStatus', '=', 'In']]
        }, {
            text: viewUtilsManager.DXTranslate('roAbsent'),
            value: [['PresenceStatus', '<>', 'In']]
        }];
    }
}

var headerFilterTelecommute = {
    load: function (loadOptions) {
        return [{
            text: viewUtilsManager.DXTranslate('roHome'),
            value: [['InTelecommute', '=', true]]
        }, {
            text: viewUtilsManager.DXTranslate('roOffice'),
            value: [['InTelecommute', '=', false]]
        }];
    }
}

function cell_prepared(selectedItems) {
    if (selectedItems.columnIndex == 0) {
        if (typeof selectedItems.row !== 'undefined') {
            if (selectedItems.row.data.PresenceStatus == "In") {
                selectedItems.cellElement.addClass("employeeListPhoto");
            }
            else {
                if (selectedItems.row.data.LastPunchFormattedDateTime != "") {
                    selectedItems.cellElement.addClass("employeeListPhotoOut");
                }
                else {
                    selectedItems.cellElement.addClass("employeeListNA");

                    var arrayLength = selectedItems.row.cells.length;
                    for (var i = 0; i < arrayLength; i++) {
                        if (typeof selectedItems.row.cells[i].cellElement !== 'undefined') {
                            selectedItems.row.cells[i].cellElement.addClass("naUser");
                        }
                    }
                }
            }
        }
    }
}