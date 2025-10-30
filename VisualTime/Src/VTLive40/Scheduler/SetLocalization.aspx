<%@ Page Language="VB" AutoEventWireup="false" Inherits="VTLive40.Scheduler_SetLocalization" CodeBehind="SetLocalization.aspx.vb" %>

<html lang="en" class="notranslate" translate="no" xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Click para seleccionar la ubicación:</title>
    <meta name="viewport" content="initial-scale=1.0, user-scalable=no" />
    <meta http-equiv="content-type" content="text/html; charset=UTF-8" />

    <script type="text/javascript">
        // Funciones del mapa
        var geocoder;
        var map;

        function initialize() {
            geocoder = new google.maps.Geocoder();
            var position = getLocation();

            var latlng;
            if (position.length > 0) {
                var coords = position.replace(" ", "").split(',');
                var lat = parseFloat(coords[0]);
                var lng = parseFloat(coords[1]);
                latlng = new google.maps.LatLng(lat, lng);
            }
            else {
                //latlng = new google.maps.LatLng(40.4202995300293,-3.70577001571655);
                navigator.geolocation.getCurrentPosition(handle_geolocation_query);
            }
            var myOptions = {
                zoom: 13,
                center: latlng,
                mapTypeId: google.maps.MapTypeId.ROADMAP,
                streetViewControl: false
            }
            if (position.length > 0) {
                map = new google.maps.Map(document.getElementById("map_canvas"), myOptions);
            }

            if (position.length > 0) {
                var marker = new google.maps.Marker({
                    position: latlng,
                    map: map
                });
            }
            google.maps.event.addListener(map, 'click', function (event) {
                placeMarker(event.latLng);
            });

            txtFindClient.Focus();
        }

        function handle_geolocation_query(position) {
            var lat = position.coords.latitude;
            var lng = position.coords.longitude;

            var latlng = new google.maps.LatLng(lat, lng);
            var myOptions = {
                zoom: 13,
                center: latlng,
                mapTypeId: google.maps.MapTypeId.ROADMAP,
                streetViewControl: false
            }
            map = new google.maps.Map(document.getElementById("map_canvas"), myOptions);

            google.maps.event.addListener(map, 'click', function (event) {
                placeMarker(event.latLng);
            });
        }

        function getLocation() {
            var point = '';
            try {
                var urlParameters = window.document.URL.split('?')[1];
                var parametersList = urlParameters.split('&');
                for (var i = 0; i < parametersList.length; i++) {
                    var parameter = parametersList[i].split('=');
                    var key = parameter[0];
                    var value = parameter[1];
                    if (key == 'pos') {
                        point = value;
                        document.getElementById("hdnPosition").value = value;
                        break;
                    }
                }
            }
            catch (e) { }
            return point;
        }

        function placeMarker(location) {
            var clickedLocation = new google.maps.LatLng(location);
            getCity(location);
            timer = setTimeout("captureClick()", 750);
        }

        function captureClick() {
            if (document.getElementById("hdnCity").value.length > 0) {
                var _CanClose = document.getElementById("hdnCanClose");
                _CanClose.value = '1';
                Close();
            }
            else
                alert('Ciudad no capturada');
        }

        function getCity(location) {
            var city = '';
            var posBox = document.getElementById("hdnPosition");
            posBox.value = location.toUrlValue();
            if (!geocoder) geocoder = new google.maps.Geocoder();
            if (geocoder) {
                try {
                    geocoder.geocode({ 'latLng': location }, function (results, status) {
                        if (status == google.maps.GeocoderStatus.OK) {
                            for (i = 0; i < results[0].address_components.length; i++) {
                                if (results[0].address_components[i].types[0] == 'locality') {
                                    city = results[0].address_components[i].long_name;
                                }
                            }
                            var cityBox = document.getElementById("hdnCity");
                            cityBox.value = city;
                        }
                        else {
                            alert('Geocoder failed due to: ' + status);
                        }
                    });
                }
                catch (e) {
                    alert(e);
                }
            }
            else {
                alert(city = 'Geocoder not initialized');
            }
        }

        function getCityex(location) {
            var latlng;
            //var city = document.getElementById("txtFind").value;
            var city = txtFindClient.GetText();

            if (!geocoder) geocoder = new google.maps.Geocoder();
            if (geocoder) {
                try {
                    geocoder.geocode({ 'address': city }, function (results, status) {
                        if (status == google.maps.GeocoderStatus.OK) {
                            var lat = results[0].geometry.location.lat();
                            var lng = results[0].geometry.location.lng();

                            latlng = new google.maps.LatLng(lat, lng);
                            var myOptions = {
                                zoom: 13,
                                center: latlng,
                                mapTypeId: google.maps.MapTypeId.ROADMAP,
                                streetViewControl: false
                            }
                            map = new google.maps.Map(document.getElementById("map_canvas"), myOptions);

                            google.maps.event.addListener(map, 'click', function (event) {
                                placeMarker(event.latLng);
                            });
                        }
                        else {
                            alert('Geocoder failed due to: ' + status);
                        }
                    });
                }
                catch (e) {
                    alert(e);
                }
            }
            else {
                alert(city = 'Geocoder not initialized');
            }
        }
    </script>

    <script type="text/javascript">
        var _generalUpdateProgressDiv;
        var _generalBackgroundDiv;

        function PageBase_Load() {
            CloseIfNeeded();
        }

        function CloseIfNeeded() {
            var _CanClose = document.getElementById("hdnCanClose");
            if (_CanClose.value == '1') Close();
        }

        function PageBase_Unload() {
            var city = document.getElementById("hdnCity").value;
            var position = document.getElementById("hdnPosition").value;
            parent.CityChange(city, position);
        }

        function Close() {
            try {
                parent.HideExternalForm();
            } catch (e) { }
        }

        function KeyPressFunction(e, FuncExec, noFunct) {
            tecla = (document.all) ? e.keyCode : e.which;
            if (tecla == 13) {
                if (noFunct) {
                    return false;
                }
                else {
                    if (FuncExec != null && FuncExec != '') {
                        eval(FuncExec);
                    }
                    else {
                        //RunAccept();
                    }
                    return false;
                }
            }
        }
    </script>
</head>

<body class="bodyPopup" onload="initialize()">

    <form id="form1" runat="server">

        <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="True" EnableScriptLocalization="True" />

        <div style="border: thin solid #BAA7AA; height: 370px; width: 460px;" id="map_canvas">
        </div>
        <asp:HiddenField runat="server" ID="hdnPosition" />
        <asp:HiddenField runat="server" ID="hdnCity" />
        <asp:HiddenField ID="hdnCanClose" runat="server" />
        <table style="width: 100%;">
            <tr style="height: 50px;">
                <td align="left">
                    <asp:Label runat="server" ID="lblCity" Text="Población:"></asp:Label></td>
                <td align="left" style="width: 220px;">
                    <dx:ASPxTextBox ID="txtFindx" runat="server" Font-Size="11px" CssClass="editTextFormat" Font-Names="Arial;Verdana;Sans-Serif" Width="200px"
                        ClientInstanceName="txtFindClient" class="textClass x-form-text x-form-field">
                    </dx:ASPxTextBox>
                </td>
                <td>
                    <dx:ASPxButton ID="btnAccept" runat="server" AutoPostBack="False" CausesValidation="False" Text="Buscar" ToolTip="Buscar" HoverStyle-CssClass="btnFlat-hover btnFlatBlack-hover" CssClass="btnFlatWithoutWith btnFlatBlack">
                        <Image Url="~/Base/Images/Grid/button_search.png" />
                        <ClientSideEvents Click="getCityex" />
                    </dx:ASPxButton>
                </td>
                <td>
                    <dx:ASPxButton ID="btnCancel" runat="server" AutoPostBack="False" CausesValidation="False" Text="${Button.Cancel}" ToolTip="${Button.Cancel}" HoverStyle-CssClass="btnFlat-hover btnFlatBlack-hover" CssClass="btnFlatWithoutWith btnFlatBlack">
                        <ClientSideEvents Click="Close" />
                    </dx:ASPxButton>
                </td>
            </tr>
        </table>
    </form>
</body>
</html>