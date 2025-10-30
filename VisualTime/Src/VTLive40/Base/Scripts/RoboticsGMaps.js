function RoboticsGMaps(container, mapOptions, initialPos, enableGeocoder, cssClass) {
    this.geocoder = null;
    this.map = null;
    this.isClosed = false;
    this.markers = [];
    this.poly = null;
    this.divContainer = container;
    this.enableGeocoder = enableGeocoder;

    this.endDrawCallback = undefined;
    this.endDrawClientCallback = undefined;
    this.cssClass = undefined

    if (typeof (mapOptions) == "undefined") {
        this.myOptions = {
            zoom: 13,
            //center: latlng,
            mapTypeId: google.maps.MapTypeId.ROADMAP,
            streetViewControl: false
        }
    } else {
        this.myOptions = mapOptions;
    }

    var latlng = null;
    if (typeof (initialPos) != 'undefined') {
        this.myOptions.center = initialPos;
    }

    if (typeof (cssClass) != 'undefined') {
        this.cssClass = cssClass;
    }
    else {
        this.cssClass = '"style="border: thin solid #BAA7AA; height: 150px; width:300px; float:left;"';
    }

    if (typeof (this.divContainer) == 'undefined') {
        alert("Map not initialized");
    }
}

RoboticsGMaps.prototype.initializeMap = function () {
    if (typeof (this.myOptions.center) != 'undefined') {
        this.drawMap();
    } else {
        if (navigator.geolocation) {
            navigator.geolocation.getCurrentPosition(this.setCurrentPosition(this), this.setErrorPosition(this));
        } else {
            this.myOptions.center = new google.maps.LatLng(40.4202995300293, -3.70577001571655);            
        }        
        this.drawMap();
    }
}


RoboticsGMaps.prototype.setErrorPosition = function (rObject) {
    return function (position) {
        var latlon = new google.maps.LatLng(40.4202995300293, -3.70577001571655);
        rObject.myOptions.center = latlon;
        rObject.drawMap();
    };
}

RoboticsGMaps.prototype.setCurrentPosition = function (rObject) {
    return function (position) {
        var latlon = new google.maps.LatLng(position.coords.latitude, position.coords.longitude);
        rObject.myOptions.center = latlon;
        rObject.drawMap();
    };
}

RoboticsGMaps.prototype.drawMap = function () {


    

    var container = $('#' + this.divContainer).parent();
    $('#' + this.divContainer).remove();
    container.append('<div id="' + this.divContainer + '" style="border: thin solid #BAA7AA; height: 100px; width:300px; float:left;"></div>');

    this.map = new google.maps.Map(document.getElementById(this.divContainer), this.myOptions);

    if (this.enableGeocoder) {
        this.geocoder = new google.maps.Geocoder();
        this.poly = new google.maps.Polyline({ map: this.map, path: [], strokeColor: "#FF0000", strokeOpacity: 1.0, strokeWeight: 2 });

        if (typeof (this.endDrawCallback) == 'undefined') {
            this.isClosed = false;
        } else {
            this.endDrawCallback(this);
        }

        google.maps.event.addListener(this.map, 'click', this.drawPolylineOnClick(this));
    }
}



RoboticsGMaps.prototype.drawMarker = function (latlng) {
    var exMap = this;
    var marker = new google.maps.Marker({
        position: latlng,
        map: exMap.map
    });
}


RoboticsGMaps.prototype.drawPolylineOnClick = function (rbObject) {
    return function (clickEvent) {
        if (rbObject.isClosed)
            return;
        var markerIndex = rbObject.poly.getPath().length;
        var isFirstMarker = markerIndex === 0;
        var marker = new google.maps.Marker({ map: rbObject.map, position: clickEvent.latLng, draggable: true });
        rbObject.markers.push(marker);
        if (isFirstMarker) {
            google.maps.event.addListener(marker, 'click', function () {
                if (rbObject.isClosed)
                    return;
                var path = rbObject.poly.getPath();
                rbObject.poly.setMap(null);
                rbObject.poly = new google.maps.Polygon({ map: rbObject.map, path: path, strokeColor: "#FF0000", strokeOpacity: 0.8, strokeWeight: 2, fillColor: "#FF0000", fillOpacity: 0.35 });
                rbObject.isClosed = true;

                try {
                    if (rbObject.endDrawClientCallback != undefined) {
                        rbObject.endDrawClientCallback(rbObject);
                    }

                    hasChanges(true);
                } catch (e) { }

            });
        }
        google.maps.event.addListener(marker, 'drag', function (dragEvent) {
            rbObject.poly.getPath().setAt(markerIndex, dragEvent.latLng);

            if (rbObject.isClosed) {
                try {
                    if (rbObject.endDrawClientCallback != undefined) {
                        rbObject.endDrawClientCallback(rbObject);
                    }

                    hasChanges(true);
                } catch (e) { }
            }

        });
        rbObject.poly.getPath().push(clickEvent.latLng);
    };
}

RoboticsGMaps.prototype.reinitializeMap = function () {
    if (this.map != null && this.poly != null) {
        google.maps.event.trigger(this.map, 'resize')
        for (var i = 0; i < this.markers.length; i++) {
            this.markers[i].setMap(null);
        }
        this.markers = [];
        this.poly.setMap(null);
        this.poly = new google.maps.Polyline({ map: this.map, path: [], strokeColor: "#FF0000", strokeOpacity: 1.0, strokeWeight: 2 });
        this.isClosed = false;
    }
}

RoboticsGMaps.prototype.positionMapOnCity = function (city) {
    if (city == "") return;

    if (!this.geocoder) this.geocoder = new google.maps.Geocoder();
    if (this.geocoder) {
        try {
            this.geocoder.geocode({ 'address': city }, this.centerOnCity(this));
        }
        catch (e) {
            alert(e);
        }
    }
    else {
        alert(city = 'Geocoder not initialized');
    }
}

RoboticsGMaps.prototype.centerOnCity = function(rObject){
    return function (results, status) {
        if (status == google.maps.GeocoderStatus.OK) {
            var lat = results[0].geometry.location.lat();
            var lng = results[0].geometry.location.lng();
            rObject.map.setCenter(new google.maps.LatLng(lat, lng));
        }
        else {
            alert('Geocoder failed due to: ' + status);
        }
    };
}

RoboticsGMaps.prototype.getPolyPath = function () {
    var path = this.poly.getPath();

    var jsonPath = [];
    for (var i = 0; i < path.length; i++) {
        jsonPath.push({ lat: path.getAt(i).lat(), lng: path.getAt(i).lng() });
    }

    return JSON.stringify({ coords: jsonPath });
}

RoboticsGMaps.prototype.drawPolyPath = function (arrPath) {
    var objPath = [];
    var markerBounds = new google.maps.LatLngBounds();

    for (var i = 0; i < arrPath.length; i++) {
        var curLatLng = new google.maps.LatLng(arrPath[i].lat, arrPath[i].lng);
        objPath.push(curLatLng);

        //var marker = new google.maps.Marker({ map: this.map, position: curLatLng, draggable: false });
        //this.markers.push(marker);

        markerBounds.extend(curLatLng);
    }

    this.poly.setMap(null);
    this.poly = new google.maps.Polygon({ map: this.map, path: objPath, strokeColor: "#FF0000", strokeOpacity: 0.8, strokeWeight: 2, fillColor: "#FF0000", fillOpacity: 0.35 });
    this.isClosed = true;
    this.map.fitBounds(markerBounds);

}
