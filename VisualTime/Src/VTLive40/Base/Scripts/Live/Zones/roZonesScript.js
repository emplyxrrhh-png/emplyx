var drawedMainMapRectangle, drawedMainMapPolygon, globalMap, allzones = [], isFirstShape, dynamicZoomBounds = [];
var firstLoad = true;
var mapInitialized = false;
var ipInitialized = false;
const _RECTANGLE_SHAPE = "rectangle";
const _POLYGON_SHAPE = "polygon";

function getWidth() {
    return document.documentElement.clientWidth - 40;
}

function getHeight() {
    return document.documentElement.clientHeight - 40;
}

//Shows the edit pop up of the selected row
function OnZoneClick(selectedItems) {
    if (selectedItems != null && selectedItems.rowType != "header") {
        if (selectedItems.columnIndex != 0 && selectedItems.rowType == "data") {
            $("#idZoneEdit").val(selectedItems.data.ID)
            openEditZonePopUp();
        }
    }
}

function selectZoneByDefault(e) {
    if (firstLoad) {
        e.component.selectRowsByIndexes([0]);
        firstLoad = false;
    }
}

$(document).ready(function () {
    ipInitialized = false;
    let gridZones = $("#gridZones").dxDataGrid("instance");
    gridZones.option("pager.displayMode", "compact");
    gridZones.option("scrolling.RowRenderingMode", "1");

    onZoneLocationWorkingModeChanged();
    ipInitialized = true;

});


function onZoneLocationWorkingModeChanged(s, e) {
    let workingMode = $("#ckZoneLocationWorkingMode").dxSwitch("instance");
    let ipRestrictionEnabled = workingMode.option("value");

    if (!ipRestrictionEnabled) $(".sectionMap").show(); 
    else $(".sectionMap").hide(); 


    let gridZones = $("#gridZones").dxDataGrid("instance");
    let colDefinition = gridZones.option("columns");

    colDefinition.find(function (x) { return x.dataField == "IpsRestriction" }).visible = ipRestrictionEnabled;
    gridZones.option("columns", colDefinition);


    if (ipInitialized) {
        $.ajax({
            type: "POST",
            url: BASE_URL + 'Zones/SetIpRestrictionStatus',
            dataType: "json",
            data: { status: ipRestrictionEnabled },
            success: function (data) {
                if (typeof data == 'string') {
                    ipInitialized = false;
                    workingMode.option("value", !ipRestrictionEnabled);
                    DevExpress.ui.notify(data, "error", 5000);
                    setTimeout(function () { ipInitialized = true; } , 500);
                } 
            },
            error: function () {
                
            }
        });
    }
}

function selection_changed(selectedItems) {
    let gridZones = $("#gridZones").dxDataGrid("instance");
    let data = gridZones.getSelectedRowsData();
    updateGlobalMap(data);
}

function RefreshZonesList() {
    let zonesList = $("#gridZones").dxDataGrid("instance");
    zonesList.refresh();
}

function initializeGlobalMap(e) {  
    if (!mapInitialized) {
        globalMap = e.originalMap;    

        if (navigator.geolocation) {
            navigator.geolocation.getCurrentPosition(function (position) {
                let initialLocation = { lat: position.coords.latitude, lng: position.coords.longitude };
                $("#globalMap").dxMap({ center: initialLocation });            
            });
        }
        mapInitialized = true;
    }
}

//elimina todas las zonas del mapa para poder volver a pintarlas posteriormente y así actualizar el mapa
function deleteAllShape() {
    for (const element of allzones) {
        element.setMap(null);
    }
    allzones = [];
}

function updateGlobalMap(data) {
    let workingMode = $("#ckZoneLocationWorkingMode").dxSwitch("instance");
    let ipRestrictionEnabled = workingMode.option("value");

    if (ipRestrictionEnabled) return;



    dynamicZoomBounds = [];
    //Eliminamos todas las zonas marcadas para actualizar el mapa
    deleteAllShape();
    for (const element of data) {
        let googleMapInfo = element.GoogleMapInfo;
        if (googleMapInfo != null && globalMap != null) {
            globalMap.setCenter(new google.maps.LatLng(googleMapInfo.Center.Latitud.toString(), googleMapInfo.Center.Longitud.toString()));
            globalMap.setZoom(googleMapInfo.Zoom)
            if (googleMapInfo.Coordinates.length > 0) {
                if (googleMapInfo.Shape == _RECTANGLE_SHAPE) {
                    drawGlobalMapRectZone(googleMapInfo, element.Color);
                } else if (googleMapInfo.Shape == _POLYGON_SHAPE) {
                    drawGlobalMapPolyZone(googleMapInfo, element.Color);
                }
            }
        }
    }

    //Actualiza el zoom del mapa dinámicamente según las zonas seleccionadas
    let latLngBounds = new google.maps.LatLngBounds();
    for (const element of dynamicZoomBounds) {
        latLngBounds.extend(element);
    }
    if (dynamicZoomBounds.length > 0) {
        globalMap.fitBounds(latLngBounds);
    }
}

// ------------- START: FUNCIONES DE IMPRESIÓN DE ZONAS ------------- 

function updateMainMapDrawedRectangle(selectedColorOptions) {
    drawedMainMapRectangle.setOptions(selectedColorOptions);
}

function updateMainMapDrawedPolygon(selectedColorOptions) {
    drawedMainMapPolygon.setOptions(selectedColorOptions);
}


//Dibuja una zona rectángulo en el mapa
function drawGlobalMapRectZone(googleMapInfo, color) {

    let southWestX = parseFloat(googleMapInfo.Coordinates[0].Latitud);
    let southWestY = parseFloat(googleMapInfo.Coordinates[0].Longitud);
    let northEastX = parseFloat(googleMapInfo.Coordinates[1].Latitud);
    let northEastY = parseFloat(googleMapInfo.Coordinates[1].Longitud);
    let rectBounds = new google.maps.LatLngBounds(
        new google.maps.LatLng(northEastX, northEastY),
        new google.maps.LatLng(southWestX, southWestY)
    );

    drawedMainMapRectangle = new google.maps.Rectangle({
        bounds: rectBounds,
        map: globalMap
    });

    let rgbColor = decimalToRgb(parseInt(color));
    let sColor   = rgbToHex(rgbColor.red, rgbColor.green, rgbColor.blue);

    let selectedColorOptions = {
        strokeColor: sColor,
        fillColor: sColor,
        fillOpacity: 0.2
    };

    updateMainMapDrawedRectangle(selectedColorOptions);

    //Se añade la zona al listado global
    allzones.push(drawedMainMapRectangle);

    //Se añaden sus puntos para poder actualizar dinámicamente el zoom del mapa
    for (const element of googleMapInfo.Coordinates) {
        dynamicZoomBounds.push(new google.maps.LatLng(element.Latitud, element.Longitud))
    }
}


function drawGlobalMapPolyZone(googleMapInfo, color) {

    const polygonCoords = [];
    for (let i = 0; i < googleMapInfo.Coordinates.length; i++) {
        var coordX = parseFloat(googleMapInfo.Coordinates[i].Latitud);
        var coordY = parseFloat(googleMapInfo.Coordinates[i].Longitud);
        polygonCoords.push(new google.maps.LatLng(coordX, coordY))
    }

    drawedMainMapPolygon = new google.maps.Polygon({
        paths: polygonCoords,
        map: globalMap
    });

    var rgbColor = decimalToRgb(parseInt(color));
    var sColor = rgbToHex(rgbColor.red, rgbColor.green, rgbColor.blue);

    var selectedColorOptions = {
        strokeColor: sColor,
        fillColor: sColor,
        fillOpacity: 0.2
    };

    updateMainMapDrawedPolygon(selectedColorOptions);

    //Se añade la zona al listado global
    allzones.push(drawedMainMapPolygon);

    //Se añaden sus puntos para poder actualizar dinámicamente el zoom del mapa
    for (let i = 0; i < googleMapInfo.Coordinates.length; i++) {
        dynamicZoomBounds.push(new google.maps.LatLng(googleMapInfo.Coordinates[i].Latitud, googleMapInfo.Coordinates[i].Longitud))
    }
}

// ------------- END: FUNCIONES DE IMPRESIÓN DE ZONAS ------------- 

// ------------- START: FUNCIONES DE CONVERSIÓN DEL COLOR  ------------- 

function decimalToRgb(decimal) {
    return {
        red: (decimal >> 16) & 0xff,
        green: (decimal >> 8) & 0xff,
        blue: decimal & 0xff,
    };
}

function componentToHex(c) {
    var hex = c.toString(16);
    return hex.length == 1 ? "0" + hex : hex;
}

function rgbToHex(r, g, b) {
    return "#" + componentToHex(r) + componentToHex(g) + componentToHex(b);
}

// ------------- END: FUNCIONES DE CONVERSIÓN DEL COLOR  ------------- 


// ------------- START: FUNCIONES DE CONVERSIÓN DE COORDS  ------------- 

function dbCoordsArray2JSON(coordinates) {    
    var i = 0;
    var coordinatesStr = '[';
    for (i = 0; i < coordinates.length; i++) {        
        if (i < (coordinates.length - 1))
            coordinatesStr += JSON.stringify({ Latitud: coordinates[i].Latitud, Longitud: coordinates[i].Longitud }) + ",";
        else
            coordinatesStr += JSON.stringify({ Latitud: coordinates[i].Latitud, Longitud: coordinates[i].Longitud })
    }
    coordinatesStr += ']';    
    return coordinatesStr;
}

function mapCoordsArray2JSON(coordinates) {
    var i = 0;
    var coordinatesStr = '[';
    for (i = 0; i < coordinates.length; i++) {        
        if (i < (coordinates.length - 1))
            coordinatesStr += JSON.stringify({ Latitud: coordinates[i].lat(), Longitud: coordinates[i].lng() }) + ",";
        else
            coordinatesStr += JSON.stringify({ Latitud: coordinates[i].lat(), Longitud: coordinates[i].lng() })
    }
    coordinatesStr += ']';    
    return coordinatesStr;
}

// ------------- END: FUNCIONES DE CONVERSIÓN DE COORDS  ------------- 

function selectPresenceZoneChanged(e) {
    var selectedZone = $('#selectPresenceZone').dxSelectBox("option", "value");    
    $.ajax({
        type: "GET",
        url: BASE_URL + 'Zones/LoadEmployeesInZone',
        contentType: "application/json; charset=utf-8",
        data: { id: selectedZone },
        success: function (result) {            
            $("#lstPresentsInZone").dxList({
                dataSource: new DevExpress.data.DataSource({
                    store: {
                        type: "array",
                        data: result,
                        key: "ID"
                    },
                    paginate: true,
                    pageSize: 57,
                    pageLoadMode: "scrollBottom"
                })                
            });
            e.component.close();
            }         
    });
}

function onSelectPresenceZoneLoaded(e) {
    if (e.component.getDataSource()._store._array != null && e.component.getDataSource()._store._array.length > 0)
    $('#selectPresenceZone').dxSelectBox("option", "value", e.component.getDataSource()._store._array[0].ID);
}

function selectAbsenceZoneChanged(e) {
    var selectedZone = $('#selectAbsenceZone').dxSelectBox("option", "value");
    $.ajax({
        type: "GET",
        url: BASE_URL + 'Zones/LoadEmployeesInZoneLastHour',
        contentType: "application/json; charset=utf-8",
        data: { id: selectedZone },
        success: function (result) {
            $("#lstAbsentsInZone").dxList({
                dataSource: new DevExpress.data.DataSource({
                    store: {
                        type: "array",
                        data: result,
                        key: "ID"
                    },
                    paginate: true,
                    pageSize: 57,
                    pageLoadMode: "scrollBottom"
                })
            });
            e.component.close();
        }
    });
}

function onSelectAbsenceZoneLoaded(e) {
    if (e.component.getDataSource()._store._array != null && e.component.getDataSource()._store._array.length > 0)
        $('#selectAbsenceZone').dxSelectBox("option", "value", e.component.getDataSource()._store._array[0].ID);
}

function onEmployeesInZoneInitialized(e) {
    if (e.component.option("items").length > 20)
        e.element.addClass("minimize")
}

function onEmployeesAbsentsInZoneInitialized(e) {
    if (e.component.option("items").length > 20)
        e.element.addClass("minimize")
}


