<%@ Page Language="VB" AutoEventWireup="false" Inherits="VTLive40.Scheduler_Controls_LocalizationMapPage" CodeBehind="LocalizationMapPage.aspx.vb" %>

<html lang="en" class="notranslate" translate="no">
<head>
    <meta name="viewport" content="initial-scale=1.0, user-scalable=no" />
    <meta http-equiv="content-type" content="text/html; charset=UTF-8" />
    <title></title>
    <script type="text/javascript">
        var myMap;
        var myInfoWindow;

        function initialize() {
            var latlngBase = new google.maps.LatLng(40.4202995300293, -3.70577001571655);
            var latlngArray = new Array()
            var query = getLocations();
            if (query.length > 0) {
                var nullValues = 0;
                var arrLocations = query.split('|');
                for (i = 0; i < arrLocations.length; i++) {
                    if (arrLocations[i].toString().length > 0) {
                        var tmp = arrLocations[i].toString().replace(',', ';');
                        var coords = tmp.split(';');
                        var lat = parseFloat(coords[0]);
                        var lng = parseFloat(coords[1]);
                        latlngArray.push(new google.maps.LatLng(lat, lng));
                    }
                    else {
                        latlngArray.push(null);
                        nullValues++;
                    }
                }
                if (nullValues == arrLocations.length) latlngArray = new Array()
            }

            var centerPos = latlngBase;
            var myOptions = {
                zoom: 15,
                center: centerPos,
                mapTypeId: google.maps.MapTypeId.ROADMAP
            };
            myMap = new google.maps.Map(document.getElementById("map_canvas"), myOptions);

            for (var i = 0; i < latlngArray.length; i++) {
                if (latlngArray[i] != null) {
                    var myMarker = createMarker(latlngArray[i], myMap, (i + 1).toString(), i);
                }
            }

            var latlngbounds;
            if (latlngArray.length > 0) {
                latlngbounds = new google.maps.LatLngBounds();
                for (var i = 0; i < latlngArray.length; i++) {
                    if (latlngArray[i] != null) {
                        latlngbounds.extend(latlngArray[i]);
                    }
                }

                //alert('fin');
                var zoom = getZoom(latlngbounds);
                myMap.setCenter(latlngbounds.getCenter(), zoom);
                myMap.setZoom(zoom);
            }
            else {
                latlngbounds = latlngBase;
            }
        }

        function getLocations() {
            var points = '';
            try {
                var urlParameters = window.document.URL.split('?')[1];
                var parametersList = urlParameters.split('&');
                for (var i = 0; i < parametersList.length; i++) {
                    var parameter = parametersList[i].split('=');
                    var key = parameter[0];
                    var value = parameter[1];
                    if (key = 'points') {
                        points = value;
                        break;
                    }
                }
            }
            catch (e) { }
            return points;
        }
        function createMarker(pos, myMap, title, order) {
            var marker = new google.maps.Marker({
                position: pos,
                map: myMap,
                title: title,
                flat: false
            });
            //return marker;
        }

        function createInfoWindow(order) {
            /*
            var contentString = '<div style="font-size:large; font-weight:bold;">' +
                'Punto ' + order +
                '</div>' +
                '<div style="font-size:medium; font-weight:normal;">' +
                'Este es el punto nº ' + order + '.<br>' +
                'Datos del punto ....' +
                '</div>';
            */
            var contentString = 'Texto';

            var infowindow = new google.maps.InfoWindow({
                content: contentString
            });
            return infowindow;
        }

        function getZoom(bounds) {
            var neBound = bounds.getNorthEast();
            var swBound = bounds.getSouthWest();
            var latitude1 = neBound.lat();
            var longitude1 = neBound.lng();
            var latitude2 = swBound.lat();
            var longitude2 = swBound.lng();
            var miles = (3958.75 * Math.acos(Math.sin(latitude1 / 57.2958) * Math.sin(latitude2 / 57.2958) + Math.cos(latitude1 / 57.2958) * Math.cos(latitude2 / 57.2958) * Math.cos(longitude2 / 57.2958 - longitude1 / 57.2958)));
            var GZoom = 0;
            if (miles < 0.2) {
                GZoom = 16;
            }
            else if (miles < 0.5) {
                GZoom = 15;
            }
            else if (miles < 1) {
                GZoom = 14;
            }
            else if (miles < 2) {
                GZoom = 13;
            }
            else if (miles < 3) {
                GZoom = 12;
            }
            else if (miles < 7) {
                GZoom = 11;
            }
            else if (miles < 15) {
                GZoom = 10;
            }
            else {
                GZoom = 9;
            }
            return GZoom;
        }
    </script>
</head>
<body onload="initialize()">
    <form id="form1" runat="server">
        <div id="map_canvas" style="width: 100%; height: 100%">
        </div>
        <asp:HiddenField runat="server" ID="hdnData" Value="" />
        <asp:HiddenField runat="server" ID="hdnDefaultCoords" Value="41.554734, 2.099215" />
    </form>
</body>
</html>