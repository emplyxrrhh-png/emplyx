<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="visitors.aspx.vb" Inherits="LiveVisits.visitors" %>

<!DOCTYPE html>

<html lang="en" class="notranslate" translate="no" xmlns="http://www.w3.org/1999/xhtml">
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1, maximum-scale=1" />
    <meta name="robots" content="noindex" />
    <meta name="googlebot" content="noindex" />
    <meta name="robots" content="nofollow" />
    <meta name="robots" content="nosnippet" />
    <meta name="robots" content="noarchive" />

    <title>Visualtime Live Visits</title>
    <link rel="shortcut icon" href="img/logovtl.png" />
    <meta charset="utf-8" />
    <link type="text/css" rel="stylesheet" href="css/src/normalize.css<%= "" & Me.MasterVersion %>" />
    <link type="text/css" rel="stylesheet" href="css/src/foundation-5.5.3.min.css<%= "" & Me.MasterVersion %>" />
    <link type="text/css" rel="stylesheet" href="css/src/font-awesome.min.css<%= "" & Me.MasterVersion %>" />
    <link type="text/css" href="css/visits.min.css<%= "" & Me.MasterVersion %>" rel="stylesheet" />

    <link type="text/css" href="css/dx.common.css<%= "" & Me.MasterVersion %>" rel="stylesheet" />
    <link type="text/css" href="css/dx.spa.css<%= "" & Me.MasterVersion %>" rel="stylesheet" />
    <link type="text/css" href="css/dx.light.css<%= "" & Me.MasterVersion %>" rel="stylesheet" />

    <script type="text/javascript" src="js/cldr.min.js<%= "" & Me.MasterVersion %>"></script>
    <script type="text/javascript" src="js/cldr/event.min.js<%= "" & Me.MasterVersion %>"></script>
    <script type="text/javascript" src="js/cldr/supplemental.min.js<%= "" & Me.MasterVersion %>"></script>
    <script type="text/javascript" src="js/cldr/unresolved.min.js<%= "" & Me.MasterVersion %>"></script>
    <script type="text/javascript" src="js/globalize.min.js<%= "" & Me.MasterVersion %>"></script>
    <script type="text/javascript" src="js/Globalize/currency.min.js<%= "" & Me.MasterVersion %>"></script>
    <script type="text/javascript" src="js/Globalize/date.min.js<%= "" & Me.MasterVersion %>"></script>
    <script type="text/javascript" src="js/Globalize/message.min.js<%= "" & Me.MasterVersion %>"></script>
    <script type="text/javascript" src="js/Globalize/number.min.js<%= "" & Me.MasterVersion %>"></script>

    <script type="text/javascript" src="js/visitors.min.js<%= "" & Me.MasterVersion %>"></script>
    <script type="text/javascript" src="js/jszip.min.js<%= "" & Me.MasterVersion %>"></script>
    <script type="text/javascript" src="js/quill.min.js<%= "" & Me.MasterVersion %>"></script>
    <script type="text/javascript" src="js/dx.all.js<%= "" & Me.MasterVersion %>"></script>
</head>
<body>
    <div id="visitors-content">

        <!-- ================== INICIO VISITANTE ================== -->
        <div id="mod-visit">
            <div class="row title"><i class="fa fa-user fa-fw"></i>&nbsp;<span data-i18n="visit">Visita</span></div>
            <div class="row" id="mod-visit-response"></div>
            <div class="row mod-visit-row">
                <div class="large-3 column large-text-right medium-text-left small-text-left">
                    <label class="inline" data-i18n="mod-visit-code" for="mod-visit-code">Visita o Visitante:</label>
                </div>
                <div class="large-9 column large-text-left medium-text-left small-text-left">
                    <input type="text" value="" name="mod-visitid" id="mod-visitid" maxlength="5" />
                </div>
            </div>
            <div class="row mod-visit-row" id="mod-visit-buttons">
                <div class="column text-center">
                    <a class="button tiny" data-i18n="mod.visit-newvisitor" id="mod-visit-buttons" onclick="getVisitor()">Avanzar</a>&nbsp;
                </div>
            </div>
            <div class="row" id="mod-visit-response"></div>
        </div>
        <!-- ================== INICIO VISITANTE ================== -->
        <!-- ================== INICIO VISITANTE ================== -->
        <div id="mod-visitor">
            <div class="row title"><i class="fa fa-user fa-fw"></i>&nbsp;<span data-i18n="visitor">Visitante</span></div>
            <div class="row" id="mod-visitor-response"></div>
            <div class="row mod-visitor-row">
                <div class="large-3 column large-text-right medium-text-left small-text-left">
                    <label class="inline" data-i18n="name" for="mod-visitor-name">Nombre:</label>
                </div>
                <div class="large-9 column large-text-left medium-text-left small-text-left">
                    <input type="hidden" value="" id="mod-visitor-idvisitor" />
                    <input type="text" value="" name="mod-visitor-name" id="mod-visitor-name" style="margin: 10px 0px 10px 0px !important" maxlength="100" />
                </div>
            </div>
            <div id="mod-visitor-fields">
            </div>
            <br />
            <div class="row mod-visitor-row" id="mod-visitor-checks">
                <div class="column text-right">
                    <div id="legalAgreement1">
                        <div style="float: left;">
                            <input type="checkbox" value="" name="mod-visitor-name" id="acceptLegal1" />
                        </div>
                        <div id="legalAgreement1Content" style="float: left">
                            <!--<label type="text" value="adeu" name="acceptLegal1-name" id="acceptLegal1-id" maxlength="100" />-->
                            <!--<label class="" for="acceptLegal1-name" style="padding-left:25px;">Texto 1</label>-->
                            <a href="#" onclick="showInfo1();" id="acceptLegal1-name" style="padding-left: 25px;" class="">Texto 1</a>
                        </div>
                    </div>
                    <div id="legalAgreement2" style="clear: both">
                        <div style="float: left;">
                            <input type="checkbox" value="" name="mod-visitor-name" id="acceptLegal2" />
                        </div>
                        <div id="legalAgreement2Content" style="float: left">
                            <!--<label type="text" value="hola" name="acceptLegal2-name" id="acceptLegal2-id" maxlength="100" />-->
                            <!--<label class="" for="acceptLegal2-name" style="padding-left:25px;">Texto 2</label>-->
                            <a href="#" onclick="showInfo2();" id="acceptLegal2-name" style="padding-left: 25px;" class="">Texto 1</a>
                        </div>
                    </div>
                </div>
            </div>
            <br />
            <div class="row mod-visitor-row" id="mod-visitor-buttons">
                <div class="column text-center">
                    <a class="button tiny" data-i18n="save" id="mod-visitor-buttons-save" onclick="saveVisitor()">Guardar</a>&nbsp;
                    <a class="button tiny" data-event="data-event" data-i18n="cancel" onclick="closeNewVisitor()" id="mod-visitor-buttons-close">Cancelar</a>
                </div>
            </div>
        </div>
        <!-- ================== FIN VISITANTE ================== -->
    </div>

    <div>
        <div id="popup">
            <div id="popupContainer" class="popup" style="overflow-y: scroll; height: 500px;"></div>
        </div>
    </div>
</body>
</html>