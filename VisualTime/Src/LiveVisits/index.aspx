<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="index.aspx.vb" Inherits="LiveVisits.index" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

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

    <link type="text/css" href="css/dx.common.css<%= "" & Me.MasterVersion %>" rel="stylesheet" />
    <link type="text/css" href="css/dx.spa.css<%= "" & Me.MasterVersion %>" rel="stylesheet" />
    <link type="text/css" href="css/dx.light.css<%= "" & Me.MasterVersion %>" rel="stylesheet" />

    <link type="text/css" rel="stylesheet" href="css/src/normalize.css<%= "" & Me.MasterVersion %>" />
    <link type="text/css" rel="stylesheet" href="css/src/foundation-5.5.3.min.css<%= "" & Me.MasterVersion %>" />
    <link type="text/css" rel="stylesheet" href="css/src/font-awesome.min.css<%= "" & Me.MasterVersion %>" />
    <link type="text/css" rel="stylesheet" href="css/src/dataTables.foundation.css<%= "" & Me.MasterVersion %>" />
    <link type="text/css" rel="stylesheet" href="css/src/jquery.datetimepicker.css<%= "" & Me.MasterVersion %>" />

    <link type="text/css" href="css/visits.min.css<%= "" & Me.MasterVersion %>" rel="stylesheet" />

    <script type="text/javascript" src="js/cldr.min.js<%= "" & Me.MasterVersion %>"></script>
    <script type="text/javascript" src="js/cldr/event.min.js<%= "" & Me.MasterVersion %>"></script>
    <script type="text/javascript" src="js/cldr/supplemental.min.js<%= "" & Me.MasterVersion %>"></script>
    <script type="text/javascript" src="js/cldr/unresolved.min.js<%= "" & Me.MasterVersion %>"></script>
    <script type="text/javascript" src="js/globalize.min.js<%= "" & Me.MasterVersion %>"></script>
    <script type="text/javascript" src="js/Globalize/currency.min.js<%= "" & Me.MasterVersion %>"></script>
    <script type="text/javascript" src="js/Globalize/date.min.js<%= "" & Me.MasterVersion %>"></script>
    <script type="text/javascript" src="js/Globalize/message.min.js<%= "" & Me.MasterVersion %>"></script>
    <script type="text/javascript" src="js/Globalize/number.min.js<%= "" & Me.MasterVersion %>"></script>

    <script type="text/javascript" src="js/visits.min.js<%= "" & Me.MasterVersion %>"></script>

    <script type="text/javascript" src="js/jszip.min.js<%= "" & Me.MasterVersion %>"></script>
    <script type="text/javascript" src="js/quill.min.js<%= "" & Me.MasterVersion %>"></script>
    <script type="text/javascript" src="js/dx.all.js<%= "" & Me.MasterVersion %>"></script>
</head>
<body class="dx-viewport">
    <div id="topBar" class="hide-on-print">
        <div class="row">
            <div id="logo" class="large-3 medium-3 small columns">
                <img class="appLogo" src="img/logovtl_bar.png" />Visualtime Visits
            </div>
            <div class=" small-6 columns text-right show-for-small-only ">
                <!--               <a href="#/opt" class="button  tiny" data-tab="opt"><i class="fa fa-cogs"></i></a>&nbsp;
                <a onclick="rologout()" class="button  tiny"><i class="fa fa-power-off"></i></a> -->
                &nbsp;
                <div class="text-center">
                    <a class="button plani-new" onclick="showVisit('new')"><span data-i18n="new_visit">Nueva visita...</span></a>
                </div>
            </div>
            <div class="large-6 medium-6 small columns text-center">
                <a href="#/plani" class="button " data-i18n="planning" data-tab="plani">Planificación</a>
                <a href="#/gest" class="button " data-i18n="management" data-tab="gest">Gestión</a>
            </div>
            <div class="large-3 medium-3 columns text-right hide-for-small-only">
                <!--                 <a href="#/opt" class="button  tiny" data-tab="opt"><i class="fa fa-cogs"></i></a>&nbsp;
                                <a onclick="rologout()" class="button  tiny" data-tab="logout"><i class="fa fa-power-off"></i></a>
                -->
                <div class="text-center">
                    <a class="button plani-new" onclick="showVisit('new')"><span data-i18n="new_visit">Nueva visita...</span></a>
                </div>
            </div>
        </div>
        <!-- ================== INICIO MENU ================== -->
        <div id="mainmenu">
            <div class="row">
                <div class="column large-6 medium-6 small-12">
                    <div class="row text-left" id="mainmenu-tabs">
                        <a href="#/opt" id="optionsmenu"><i class="fa fa-cogs fa-2x"></i><span data-i18n="options">Opciones</span></a>
                    </div>
                </div>
                <div class="column large-6 medium-6 small-12 smallmenu ">
                    <div class="row text-right">
                        <span id="applabel">Visualtime Visits</span>
                        <span id="versionlabel"></span>
                    </div>
                    <div class="row text-right">
                        <a id="changepassword" onclick="openModal('#mod-changepassword')"><span data-i18n="changepassword">Cambiar contraseña</span> <i class="fa fa-link fa-fw"></i></a>
                    </div>
                    <div class="row text-right">
                        <a id="changelang" onclick="openModal('#mod-changelang')"><span data-i18n="changelang">Cambiar idioma</span> <i class="fa fa-language  fa-fw"></i></a>
                    </div>
                    <div class="row text-right">
                        <a id="closesession" onclick="rologout()"><span data-i18n="closesession">Cerrar sessión</span> <i class="fa fa-power-off  fa-fw"></i></a>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div id="content">
        <!-- ================== INICIO PLANIFICACION ================== -->
        <div id="tab-plani" class="tab">
            <!--<div id="tab-plani-date" class="topbar row">
                <div class="large-2 large-centered column text-center">
                    <!-- <input id="tab-plani-date-input" type="text" />
                    <span id="tab-plani-date-value"></span>
                </div>
            </div>-->
            <div id="tab-plani-filter" class="topbar row">
                <div class="large-8 medium-10 medium-centered large-centered column text-center ">
                    <!--<a href="#/plani/active" data-subtab="active" class="button tiny" data-i18n="active">Activos</a>-->
                    <a href="#/plani/scheduled" data-subtab="scheduled" class="button tiny" data-i18n="scheduled">Programadas</a>
                    <a href="#/plani/inprogress" data-subtab="inprogress" class="button tiny" data-i18n="inprogress">En curso</a>
                    <a href="#/plani/finished" data-subtab="finished" class="button tiny" data-i18n="finished">Finalizadas</a>
                    <a href="#/plani/notpresented" data-subtab="notpresented" class="button tiny" data-i18n="notpresented">No presentado</a>
                    <a href="#/plani/results" data-subtab="search" class="button tiny" data-i18n="results">Resultados</a>
                </div>
            </div>
            <div id="tab-plani-isloading">
            </div>
            <div id="newVisits" data-alert class="text-center alert-box info" style="display: none;">
                Se han encontrado modificaciones, para actualizar hacer click aquí.
            </div>

            <!--            <div id="subtab-visits" class="subtab row" data-subtab="active">Activos</div>-->
            <div id="subtab-scheduled" class="subtab plani-subtab row" data-subtab="scheduled">Planficados</div>
            <div id="subtab-inprogress" class="subtab plani-subtab row" data-subtab="inprogress">En curso</div>
            <div id="subtab-finished" class="subtab plani-subtab row" data-subtab="finished">Finalizados</div>
            <div id="subtab-notpresented" class="subtab plani-subtab row" data-subtab="notpresented">No presentados</div>
            <div id="subtab-results" class="subtab plani-subtab row" data-subtab="search">
                <div id="subtab-results-loading"></div>
                <div>
                    <div class="gest-top-title togglenext" data-i18n="today">Hoy</div>
                    <div id="subtab-results-today"></div>
                </div>
                <div>
                    <div class="gest-top-title togglenext" data-i18n="scheduled">Programadas</div>
                    <div id="subtab-results-scheduled"></div>
                </div>
                <div>
                    <div class="gest-top-title togglenext" data-i18n="closeds">Cerradas</div>
                    <div id="subtab-results-finished"></div>
                </div>
            </div>

            <div id="plani-right">
                <div id="plani-clock" class="text-center">
                    <span id="tab-plani-date-value"></span>
                    <span id="tab-plani-time-value"></span>
                </div>
                <div id="plani-location">
                    <div class="gest-top-title-home togglenext" data-i18n="LocationBox">Localización</div>
                    <div class="search-dropdown">
                        <div>
                            <select name="plani-Location-selector" id="plani-Location-selector">
                            </select>
                        </div>
                        <div style="min-height: 20px">&nbsp; </div>
                    </div>
                    <div style="min-height: 20px">&nbsp; </div>
                </div>
                <div id="plani-order">
                    <div class="gest-top-title-home togglenext" data-i18n="orderBox">Ordenación</div>
                    <div class="search-dropdown">
                        <div>
                            <span data-i18n="orderBy">Ordenar por: </span>
                            <select name="plani-orderBy-selector" id="plani-orderBy-selector">
                                <option value="hour" data-i18n="hour">Hora</option>
                                <option value="description" data-i18n="visitDescription">Descripción</option>
                                <option value="visitor" data-i18n="visitor">Visitante</option>
                                <option value="employee" data-i18n="employee">Empleado</option>
                                <option value="userField" data-i18n="userfield">Campo personalizado</option>
                            </select>
                        </div>
                        <div style="min-height: 20px">&nbsp; </div>
                    </div>
                    <div style="min-height: 20px">&nbsp; </div>
                </div>
                <div id="plani-search">
                    <div class="gest-top-title-home togglenext" data-i18n="filters">Buscar</div>
                    <div class="search-dropdown">
                        <div>
                            <span data-i18n="keyword">Palabra Clave: </span>
                            <input id="plani-search-text" type="text" autofocus />
                            <span data-i18n="where">Donde: </span>
                            <select name="plani-search-selector" id="plani-search-selector">
                                <option value="all" data-i18n="all">Todos</option>
                                <option value="visits" data-i18n="visits">Visitas</option>
                                <option value="visitors" data-i18n="visitors">Visitantes</option>
                                <option value="employees" data-i18n="employees">Empleados</option>
                            </select>
                        </div>
                        <span>&nbsp;</span>
                        <div class="search-dropdown-button">
                            <a id="VisitListSearch" class="button small"><span data-i18n="search">Buscar</span></a>
                            <span>&nbsp;</span>
                            <a id="VisitListSearchClean" class="button small"><span data-i18n="cleanfilter">Limpiar busqueda</span></a>
                        </div>
                    </div>
                </div>

                <div id="plani-status">
                    <div id="plani-status-scheduled"></div>
                    <div id="plani-status-inprogress"></div>
                    <div id="plani-status-finished"></div>
                    <div id="plani-status-notpresented"></div>
                </div>
                <span>&nbsp;</span>
                <div id="plani-fastopen" class="search-dropdown">
                    <div class="gest-top-title-home togglenext" data-i18n="filtersQuick">Cierre rapido</div>
                    <div>
                        <div>
                            <span id="plani-fastopen-field"></span>
                            <input id="plani-fastopen-value" type="text" />
                        </div>
                        <div id="plani-fastopen-response"></div>
                        <span>&nbsp;</span>
                        <div class="search-dropdown-button">
                            <a id="plani-fastopen-search" onclick="searchfastfield()" class="button small"><span data-i18n="search">Buscar</span></a>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <!-- ================== FIN PLANIFICACION ================== -->
        <!-- ================== INICIO GESTIÓN ================== -->
        <div id="tab-gest" class="tab">
            <div class="topbar row hide-on-print">
                <div class="large-8 medium-10 medium-centered large-centered column text-center ">
                    <a href="#/gest/visits" class="button  tiny" style="font-size: 14px" data-subtab="visits" data-i18n="visits">Visitas</a>
                    <a href="#/gest/visitors" class="button  tiny" style="font-size: 14px" data-subtab="visitors" data-i18n="visitors">Visitantes</a>
                </div>
            </div>
            <div id="subtab-gestvisits" class="subtab row" data-subtab="visits">
                <div class="subtabtitle hide-on-print"><i class="fa fa-list-alt fa-fw"></i>&nbsp;<span data-i18n="visits">Visitas</span></div>
                <div class="hide-on-print" id="gest-visits-response"></div>
                <div class="hide-on-print gest-top" id="gest-visits-filters">
                    <div class="gest-top-title togglenext" data-i18n="filters">Filtros</div>
                    <div id="gest-visits-filters-values">

                        <div>
                            <span data-i18n="startdatefilter">Inicio</span>&nbsp;<i class="fa fa-trash-o resetvisitsinput"></i>
                            <input id="gest-visits-filters-begin" class="datepicker" type="text" />
                        </div>
                        <div>
                            <span data-i18n="enddatefilter">Fin</span>&nbsp;<i class="fa fa-trash-o resetvisitsinput"></i>
                            <input id="gest-visits-filters-end" class="datepicker" type="text" />
                        </div>

                        <div id="visitcustomfilters"></div>
                        <div>
                            <a onclick="filterVisitGest()" class="button small"><span data-i18n="filter">Aplicar filtro</span></a> &nbsp;&nbsp;
                        </div>
                    </div>
                </div>

                <br />
                <div id="gest-visits-devextreme">
                </div>

                <div id="gest-visits">
                </div>
                <br />
            </div>
            <div id="subtab-gestvisitors" class="subtab row" data-subtab="visitors">
                <div class="subtabtitle hide-on-print"><i class="fa fa-list-alt fa-fw"></i>&nbsp;<span data-i18n="visitor">Visitante</span></div>
                <div class="hide-on-print" id="gest-visitors-response"></div>
                <div class="hide-on-print gest-top" id="gest-visitors-filters">
                    <div class="gest-top-title togglenext" data-i18n="filters">Filtros</div>
                    <div id="gest-visitors-filters-values">
                        <div>
                            <span data-i18n="name">Nombre</span>&nbsp;<i class="fa fa-trash-o resetinput"></i>
                            <input id="gest-visitors-filters-name" type="text" />
                        </div>
                        <div id="visitorcustomfilters"></div>
                        <div>
                            <a onclick="filterVisitorGest()" class="button small"><span data-i18n="filter">Aplicar filtro</span></a>&nbsp;&nbsp;
                            <a onclick="cleanfilterVisitorGest()" class="button small"><span data-i18n="cleanfilter">Limpiar filtro</span></a>
                        </div>
                    </div>
                </div>
                <div class=" hide-on-print gest-top" id="gest-visitors-columns">
                    <div class="gest-top-title togglenext" data-i18n="columnsvisible">Columnas visibles</div>
                    <div id="gest-visitors-columns-values"></div>
                </div>
                <div id="gest-visitors-title" class="table-title hide-on-print">
                    <div class="row">
                        <div class="column large-9"><span data-i18n="visitors">Visitantes</span></div>
                        <div class="column large-3 text-right">
                            <a class="button tiny" href="#/gest/visitors/new"><span data-i18n="new_visitor">Nuevo visitante...</span></a>
                            <a class="button tiny" onclick="_visitorsGestCache = null; _visitorsCache = null; getVisitorGest(_currVisitorFilter)"><i class="fa fa-refresh fa-lg"></i></a>
                            <a class="button tiny" onclick="window.print()"><i class="fa fa-print fa-lg"></i></a>
                        </div>
                    </div>
                </div>
                <div id="gest-visitors">
                </div>
                <div class="table_footer">&nbsp;</div>
            </div>
        </div>

        <!-- ================== FIN GESTION ================== -->
        <!-- ================== INICIO OPCIONES ================== -->
        <div id="tab-opt" class="tab">
            <div class="topbar row hide-on-print">
                <div class="large-8 medium-10 medium-centered large-centered column text-center ">
                    <a href="#/opt/general" class="button  tiny" data-subtab="general" data-i18n="general">General</a>
                    <a href="#/opt/visitfields" class="button  tiny" data-subtab="visitfields" data-i18n="visitfields">Campos de visita</a>
                    <a href="#/opt/visitorfields" class="button  tiny" data-subtab="visitorfields" data-i18n="visitorfields">Campos de visitantes</a>
                    <a href="#/opt/visittypes" class="button  tiny" data-subtab="visittypes" data-i18n="visittypes">Tipos de visita</a>
                    <a href="#/opt/printconfig" class="button  tiny" data-subtab="printconfig" data-i18n="printconfig">Configuración de impresión</a>
                    <a href="#/opt/regtemplates" class="button  tiny" data-subtab="regtemplates" data-i18n="regtemplates">Configuración de normativas</a>
                </div>
            </div>
            <div id="subtab-general" class="subtab row" data-subtab="general">
                <div class="subtabtitle hide-on-print"><i class="fa fa-list"></i>&nbsp;<span data-i18n="general">General</span></div>
                <div class="hide-on-print" id="opt-general-response"></div>
                <div id="opt-general">
                    <div class="row">
                        <div class="column small-12 medium-6 large-4">
                            <label>
                                <span style="margin-left: 5px" data-i18n="notificacions">Notificaciones</span>
                                <select id="opt-general-notification">
                                    <option value="0" data-i18n="notnotifier">No notificar</option>
                                    <option value="1" data-i18n="notifieremployee">Notificar al responsable</option>
                                    <option value="2" data-i18n="notifieruser">Notificar al usuario que realiza la acción</option>
                                </select>
                            </label>
                        </div>
                        <div class="column small-12 medium-6 large-4 end">
                            <label>
                                <span style="margin-left: 5px" data-i18n="fastsearchfield">Campo de busqueda rápida</span>
                                <select id="opt-general-searchfield"></select>
                            </label>
                        </div>
                    </div>
                    <br />
                    <div class="row">
                        <div class="column" style="background-color: #85a6ff;">
                            <label>
                                <span style="margin-left: 5px; color: white; font-weight: bold;" data-i18n="automaticVisitClose">Cierre automático visita</span>
                            </label>
                        </div>
                    </div>
                    <div class="row">
                        <div class="column small-12 medium-6 large-4">
                            <label>
                                <span style="margin-left: 5px" data-i18n="cardfield">Campo tarjeta</span>
                                <select id="opt-general-cardfield"></select>
                            </label>
                        </div>
                        <div class="column small-12 medium-6 large-4 end">
                            <label>
                                <span data-i18n="cardZoneToDeposit" style="margin-left: 5px">Zona a depositar la tarjeta</span>
                                <select id="opt-general-zonefield"></select>
                            </label>
                        </div>
                        <input id="opt-general-allowvisitfieldmodify" style="display: none;"></input>
                    </div>
                </div>
                <br />
                <div class="row">
                    <div class="column" style="background-color: #85a6ff;">
                        <label>
                            <span style="margin-left: 5px; color: white; font-weight: bold;" data-i18n="cleanVisitorData">Limpieza datos clientes</span>
                        </label>
                    </div>
                </div>
                <div class="row">
                    <div class="column small-12 medium-6 large-4">
                        <label>
                            <span style="margin-left: 5px" data-i18n="days">Días</span>
                            <select id="opt-general-cleanVisitorData"></select>
                        </label>
                    </div>
                </div>
                <br />
                <div class="row">
                    <div class="column" style="background-color: #85a6ff;">
                        <label>
                            <span style="margin-left: 5px; color: white; font-weight: bold;" data-i18n="visitorIdentificationNumberField">Reconocimiento de documentos de identidad</span>
                        </label>
                    </div>
                </div>
                <div class="row">
                    <div class="column small-12 medium-6 large-4">
                        <label>
                            <span style="margin-left: 5px" data-i18n="identificationnumber">Campo DNI</span>
                            <select id="opt-general-visitorIdentificationNumberField"></select>
                        </label>
                    </div>
                    <div class="column small-12 medium-6 large-4 end">
                        <label>
                            <span style="margin-left: 5px" data-i18n="scannerserviceport">Puerto del servicio de reconocimiento</span>
                            <input id="opt-general-scannerserviceport" style="margin-top: 4px" type="text"></input>
                        </label>
                    </div>
                </div>
                <div class="row">
                    <div class="column" style="background-color: #85a6ff;">
                        <label>
                            <span style="margin-left: 5px; color: white; font-weight: bold;" data-i18n="VisitOptionData">Opciones de las visitas</span>
                        </label>
                    </div>
                </div>
                <div class="row">
                    <div class="column small-12 medium-6 large-4">
                        <label>
                            <span style="margin-left: 5px" data-i18n="uniqueidnumber">Campo identificador de visitas</span>
                            <select id="opt-general-visitUniqueIDField"></select>
                        </label>
                    </div>
                    <div class="column small-12 medium-6 large-4 end">
                        <label>
                            <span style="margin-left: 5px" data-i18n="uniqueidnumberVisitor">Campo identificador de visitantes</span>
                            <select id="opt-general-visitorUniqueIDField"></select>
                        </label>
                    </div>
                </div>
                <div class="row">
                    <div class="column small-12 medium-6 large-4">
                        <label>
                            <span style="margin-left: 5px" data-i18n="multilocationField">Campo de la ubicación</span>
                            <select id="opt-general-multilocationField"></select>
                        </label>
                    </div>
                </div>

                <br />
                <div class="row">
                    <div class="column text-left">
                        &nbsp;<a class="button  tiny" data-i18n="save" onclick="saveOptions()">Guardar</a>&nbsp;
                        <a class="button tiny" data-event data-i18n="cancel" onclick="getOptions()">Cancelar</a>
                    </div>
                </div>
            </div>
        </div>
        <div id="subtab-visitfields" class="subtab row" data-subtab="visitfields">
            <div class="subtabtitle hide-on-print"><i class="fa fa-list"></i>&nbsp;<span data-i18n="visitfield">Campo de visita</span></div>
            <div class="hide-on-print" id="opt-visitfields-response"></div>
            <div class="hide-on-print">
            </div>
            <div id="opt-title-visitsfields" class="table-title hide-on-print">
                <div class=" hide-on-print row">
                    <div class="column large-9"><span data-i18n="visitfields">Campos de visitas</span></div>
                    <div class="column large-3 text-right">
                        <div class="" id="new_visitfield">
                            <a class="button tiny" href="#/opt/visitfields/new"><span data-i18n="new_visitfield">Nuevo campo de visita...</span></a>
                        </div>
                    </div>
                </div>
            </div>
            <div id="opt-visitfields">
            </div>
            <div class="table_footer">&nbsp;</div>
        </div>
        <div id="subtab-visitorfields" class="subtab row hide-on-print" data-subtab="visitorfields">
            <div class="subtabtitle hide-on-print "><i class="fa fa-list"></i>&nbsp;<span data-i18n="visitorfields">Campo de visitantes</span></div>
            <div class="hide-on-print" id="opt-visitorfields-response"></div>
            <div class=" hide-on-print">
            </div>
            <div id="opt-title-visitorsfields" class="table-title hide-on-print">
                <div class=" hide-on-print row">
                    <div class="column large-9"><span data-i18n="visitorfields">Campos de visitantes</span></div>
                    <div class="column large-3 text-right">
                        <div class="" id="new_visitorfield">
                            <a class="button tiny" href="#/opt/visitorfields/new"><span data-i18n="new_visitorfield">Nuevo campo de visitante...</span></a>
                        </div>
                    </div>
                </div>
            </div>
            <div id="opt-visitorfields"></div>
            <div class="table_footer">&nbsp;</div>
        </div>
        <div id="subtab-visittypes" class="subtab row hide-on-print" data-subtab="visittypes">
            <div class="subtabtitle hide-on-print "><i class="fa fa-list"></i>&nbsp;<span data-i18n="visittypes">Tipo de visitas</span></div>
            <div class="hide-on-print" id="opt-visittypes-response"></div>
            <div class=" hide-on-print">
            </div>
            <div id="opt-title-visittypes" class="table-title hide-on-print">
                <div class=" hide-on-print row">
                    <div class="column large-9"><span data-i18n="visittypes">Tipos de visita</span></div>
                    <div class="column large-3 text-right">
                        <div class="" id="new_visittype">
                            <a class="button tiny" href="#/opt/visittypes/new"><span data-i18n="new_visittype">Nuevo tipo de visita...</span></a>
                        </div>
                    </div>
                </div>
            </div>
            <div id="opt-visittypes"></div>
            <div class="table_footer">&nbsp;</div>
        </div>

        <!-- ========= Campos impresión ================== -->
        <div id="subtab-printconfig" class="subtab row hide-on-print" data-subtab="printconfig">
            <div class="subtabtitle hide-on-print "><i class="fa fa-list"></i>&nbsp;<span data-i18n="printconfig">Configuración de impresión</span></div>
            <div class="hide-on-print" id="opt-printconfig-response"></div>
            <div class=" hide-on-print">
            </div>
            <div id="opt-title-printconfig" class="table-title hide-on-print">
                <div class=" hide-on-print row">
                    <div class="column large-9"><span data-i18n="printconfig">Diseño de etiqueta</span></div>
                    <input id="opt-printconfig-id" type="hidden" value="" />
                </div>
            </div>
            <br />
            <div id="printlabels">
                <div class="label-configurator" id="labels" style="width: 70%; margin: 0 auto;">
                    <div class="html-editor" style="height: 500px;">
                        <br />
                        <br />
                        <br />
                    </div>
                </div>
                <span>&nbsp;</span>

                <div class="label-configurator-button" style="width: 100px; margin: 0 auto;">
                    <a id="PrintConfigSave" onclick="saveVisitPrintConfig()" class="button small"><span data-i18n="save">Guardar</span></a>
                    <%--  <span>&nbsp;</span>
                <a id="PrintConfigClean" class="button small"><span data-i18n="cleanconfig">Limpiar diseño</span></a>--%>
                </div>
            </div>
        </div>
        <!-- ========= Campos de reglamentos ================== -->
        <div id="subtab-regtemplates" class="subtab row hide-on-print" data-subtab="regtemplates">
            <div class="subtabtitle hide-on-print "><i class="fa fa-list"></i>&nbsp;<span data-i18n="regtemplates">Configuración de normativas</span></div>
            <div class="hide-on-print" id="opt-regtemplates-response"></div>
            <div class=" hide-on-print">
            </div>
            <div id="opt-title-regtemplates" class="table-title hide-on-print">
                <div class=" hide-on-print row">
                    <div class="column large-9"><span data-i18n="regtemplates">Textos legales</span></div>
                    <input id="opt-regtemplates-id" type="hidden" value="" />
                </div>
            </div>
            <br />
            <div id="Laws">
                <div class="label-configurator" id="laws" style="width: 80%; margin: 0 auto;">
                    <label>
                        <span style="margin-left: 15px" data-i18n="legalText1">Texto Legal 1</span>
                        <input id="opt-legalText1" style="margin: 15px; width: 99%;" type="text"></input>
                    </label>
                    <div class="html-editor" style="height: 250px;">
                        <br />
                        <br />
                        <br />
                    </div>
                </div>
                <br />
                <div class="label-configurator" id="laws2" style="width: 80%; margin: 0 auto;">
                    <label>
                        <span style="margin-left: 15px" data-i18n="legalText2">Texto legal 2</span>
                        <input id="opt-legalText2" style="margin: 15px; width: 99%;" type="text"></input>
                    </label>
                    <div class="html-editor" style="height: 250px;">
                        <br />
                        <br />
                        <br />
                    </div>
                </div>
                <br />
            </div>
            <span>&nbsp;</span>

            <div class="label-configurator-button" style="width: 100px; margin: 0 auto;">
                <a id="saveVisitLaws" onclick="saveVisitLaws()" class="button small"><span data-i18n="save">Guardar</span></a>
                <%--  <span>&nbsp;</span>
                <a id="PrintConfigClean" class="button small"><span data-i18n="cleanconfig">Limpiar diseño</span></a>--%>
            </div>
        </div>
    </div>
    <!-- ========= Fin Campos impresión ================== -->
    <!-- ========= Campos ocultos impresión ================== -->
    <div class="label-configurator" id="labelsinput" style="width: 100%; margin: 0 auto; display: none; font-size: 10px!important;">
        <div style="margin: 0; width: 100%;">
            <div id="opt-printconfig"></div>
        </div>

        <div style="clear: both; height: 10px;">&nbsp;</div>

        <!-- ========= Fin Campos ocultos impresión ================== -->
    </div>
    <!-- ========= Fin Campos de reglamentos ================== -->

    <!-- ================== FIN OPCIONES ================== -->
    <!-- ================== INICIO CONSENTIMIENTO ================== -->
    <div id="mod-consent" class="reveal-modal small" data-reveal data-options="closeOnClick:false;">

        <div class="row title"><i class="fa fa-list"></i>&nbsp;<span data-i18n="consent">Consentimiento</span></div>
        <div class="row" id="mod-consent-response"></div>
        <div class="row mod-consent-row">
            <div>
                <label class="inline" for="mod-consent-name" id="mod-consent-name"></label>
            </div>
        </div>
        <div class="column text-center">

            <label style="display: inline-block;">
                &nbsp;<input type="checkbox" value="t" style="margin-top: 13px;" id="mod-consent-edit" name="mod-consent-edit" onchange="onChange()" />
                <label class="inline" data-i18n="consentAgreed" for="mod-consent-type">De acuerdo</label>
            </label>
        </div>
        <div class="row mod-consent-row" id="mod-consent-buttons">
            <div class="column text-center">
                <a class="button  tiny" data-i18n="accept" id="mod-consent-buttons-save" onclick="saveConsent()">Guardar</a>&nbsp;
            </div>
        </div>
    </div>
    <!-- ================== FIN CONSENTIMIENTO ================== -->
    <!-- ================== INICIO VISITA ================== -->
    <div id="mod-visit" class="reveal-modal small" data-reveal>
        <a class="close-reveal-modal">&#215;</a>
        <div class="row title"><i class="fa fa-list-alt fa-fw"></i>&nbsp;<span data-i18n="visits">Visitas</span></div>
        <div class="row" id="mod-visit-response"></div>
        <div class="row mod-visit-row">
            <div class="large-3 column large-text-right medium-text-left small-text-left">
                <label class="inline" data-i18n="type" for="mod-visit-type">Tipo de visita:</label>
            </div>
            <div class="large-9 column large-text-left medium-text-left small-text-left">
                <select name="mod-visit-type" id="mod-visit-type">
                    <option value=""></option>
                </select>
            </div>
        </div>
        <br />
        <div class="row mod-visit-row">
            <div class="large-3 column large-text-right medium-text-left small-text-left">
                <label class="inline" data-i18n="subject" for="mod-visit-name">Asunto:</label>
            </div>
            <div class="large-9 column large-text-left medium-text-left small-text-left">
                <input type="text" value="" name="mod-visit-name" id="mod-visit-name" maxlength="100" />
                <div class="alert-box alert-input">Invalid entry</div>
            </div>
        </div>
        <div class="row mod-visit-row">
            <div class="large-3 medium-4 column large-text-right medium-text-right small-text-left">
                <label data-i18n="visitors">Visitante/s</label>
            </div>
            <div class="large-9 medium-8 column large-text-left medium-text-left small-text-left" id="mod-visit-visitors">
            </div>
        </div>
        <div class="row mod-visit-row">
            <div class="large-3 medium-4 column large-text-right medium-text-right small-text-left">
                <label data-i18n="when">Cuando:</label>
            </div>
            <div class="large-9 medium-8 column large-text-left medium-text-left small-text-left">
                <div>
                    <input id="mod-visit-idvisit" type="hidden" value="" />
                    <input id="mod-visit-idparentvisit" type="hidden" value="" />
                    <input id="mod-visit-status" type="hidden" value="0" />
                    &nbsp;<input id="chkWhen0" type="radio" value="0" name="chkWhen" />
                    <label for="chkWhen0" data-i18n="oneday">Una día/hora concreto</label>
                </div>
                <div>
                    &nbsp;<input id="chkWhen1" type="radio" value="1" name="chkWhen" />
                    <label for="chkWhen1" data-i18n="aperiod">Un periodo</label>
                </div>
            </div>
        </div>
        <div class="row mod-visit-row">
            <div class="large-3 medium-4 column large-text-right medium-text-right small-text-left  ">
                <label class="inline" data-i18n="startdate">Inicio:</label>
            </div>

            <div class="large-9 medium-8 column large-text-left medium-text-left small-text-left">
                <input id="mod-visit-startdate" class="datetimepicker" type="text" />
            </div>
        </div>
        <div class="row mod-visit-row visit-period ">
            <div class="large-3 medium-4 column large-text-right medium-text-right small-text-left">
                <label class="inline" data-i18n="enddate">Repetición:</label>
            </div>
            <div class="large-9 medium-8 column large-text-left medium-text-left small-text-left">
                <input id="mod-visit-enddate" class="datetimepicker" type="text" />
            </div>
        </div>
        <div class="row mod-visit-row visit-period">
            <div class="large-3 medium-4 small-2 column large-text-right medium-text-right">
                &nbsp;
            </div>
            <div class="large-9 medium-8 small-10 column large-text-left medium-text-left end">
                <label id="mod-visit-repeat-label">
                    &nbsp;<input id="mod-visit-repeat" type="checkbox">
                    &nbsp;<span data-i18n="repeatvisit">La visita puede venir mas de una vez en el periodo</span>
                </label>
            </div>
        </div>
        <div class="row mod-visit-row visit-repeattime">
            <div class="large-3 medium-4 column large-text-right medium-text-right small-text-left">
                <label class="inline" data-i18n="repeater">Repetición:</label>
            </div>
            <div class="large-9 medium-8 column large-text-left medium-text-left small-text-left">
                <input id="mod-visit-repeattime" type="hidden" value="" />
                <div>
                    <label>
                        <select id="visit-repeattype">
                            <option value="--"><span data-i18n="visit-repeattime-none">Unica</span></option>
                            <option value="daily"><span data-i18n="visit-repeattime-daily">Diaria</span></option>
                            <option value="weekly"><span data-i18n="visit-repeattime-weekly">Semanalmente</span></option>
                            <option value="monthly"><span data-i18n="visit-repeattime-monthly">Mensualmente</span></option>
                            <option value="yearly"><span data-i18n="visit-repeattime-yearly">Anualmente</span></option>
                        </select>
                    </label>
                </div>
                <div id="visit-repeattime-daily">
                    <div id="mod-visit-repeattime-daily-alldays">
                        <label>
                            &nbsp;<input type="radio" value="a" name="mod-visit-repeattime-dailyinput">
                            &nbsp;<span data-i18n="visit-repeattime-daily-alldays">Repetir todos los dias </span>
                        </label>
                    </div>
                    <div id="mod-visit-repeattime-daily-days">
                        <label>
                            &nbsp;<input type="radio" value="n" name="mod-visit-repeattime-dailyinput">
                            &nbsp;<span data-i18n="visit-repeattime-daily-days">Repetir cada </span>
                            <input type="text" id="mod-visit-repeattime-daily-ndays" input style="width: 50px; display: inline-block;" />&nbsp;<span data-i18n="days">días</span>
                        </label>
                    </div>
                </div>
                <div id="visit-repeattime-weekly">
                    <label>
                        &nbsp;<span data-i18n="visit-repeattime-weekly">Repetir cada </span>
                        <input type="text" id="visit-repeattime-weekly-nweeks" style="width: 50px; display: inline-block;" />
                        &nbsp;<span data-i18n="weeks">semanas</span>
                    </label>

                    <div>
                        &nbsp;<input id="mod-visit-repeattime-daily-days1" type="checkbox" value="1">
                        <label for="mod-visit-repeattime-daily-days1" data-i18n="Monday">Lunes</label>
                        <input id="mod-visit-repeattime-daily-days2" type="checkbox" value="2">
                        <label for="mod-visit-repeattime-daily-days2" data-i18n="Tuesday">Martes</label>
                        <input id="mod-visit-repeattime-daily-days3" type="checkbox" value="3">
                        <label for="mod-visit-repeattime-daily-days3" data-i18n="Wednesday">Miercoles</label>
                        <input id="mod-visit-repeattime-daily-days4" type="checkbox" value="4">
                        <label for="mod-visit-repeattime-daily-days4" data-i18n="Thursday">Jueves</label>
                    </div>
                    <div>
                        &nbsp;<input id="mod-visit-repeattime-daily-days5" type="checkbox" value="5">
                        <label for="mod-visit-repeattime-daily-days5" data-i18n="Friday">Viernes</label>
                        <input id="mod-visit-repeattime-daily-days6" type="checkbox" value="6">
                        <label for="mod-visit-repeattime-daily-days6" data-i18n="Saturday">Sabado</label>
                        <input id="mod-visit-repeattime-daily-days0" type="checkbox" value="0">
                        <label for="mod-visit-repeattime-daily-days0" data-i18n="Sunday">Domingo</label>
                    </div>
                </div>
                <div id="visit-repeattime-monthly">
                    <label style="display: inline-block;">
                        &nbsp;<span data-i18n="repeateveryday">Repetir cada </span>
                        <select style="display: inline-block; width: 60px;" id="visit-repeattime-monthly-day">
                            <option value="1">1</option>
                            <option value="2">2</option>
                            <option value="3">3</option>
                            <option value="4">4</option>
                            <option value="5">5</option>
                            <option value="6">6</option>
                            <option value="7">7</option>
                            <option value="8">8</option>
                            <option value="9">9</option>
                            <option value="10">10</option>
                            <option value="11">11</option>
                            <option value="12">12</option>
                            <option value="13">13</option>
                            <option value="14">14</option>
                            <option value="15">15</option>
                            <option value="16">16</option>
                            <option value="17">17</option>
                            <option value="18">18</option>
                            <option value="19">19</option>
                            <option value="20">20</option>
                            <option value="21">21</option>
                            <option value="22">22</option>
                            <option value="23">23</option>
                            <option value="24">24</option>
                            <option value="25">25</option>
                            <option value="26">26</option>
                            <option value="27">27</option>
                            <option value="28">28</option>
                            <option value="29">29</option>
                            <option value="30">30</option>
                            <option value="31">31</option>
                        </select>
                    </label>
                    <label style="display: inline-block;">
                        <span data-i18n="">de cada</span>
                        <input type="text" style="width: 50px; display: inline-block;" id="visit-repeattime-monthly-nmonths" />&nbsp;<span data-i18n="months">meses</span>
                    </label>
                </div>
                <div id="visit-repeattime-yearly">
                    <label style="display: inline-block;">
                        &nbsp;<span data-i18n="">Dia</span>
                        <select style="display: inline-block; width: 60px;" id="visit-repeattime-yearly-day">
                            <option value="1">1</option>
                            <option value="2">2</option>
                            <option value="3">3</option>
                            <option value="4">4</option>
                            <option value="5">5</option>
                            <option value="6">6</option>
                            <option value="7">7</option>
                            <option value="8">8</option>
                            <option value="9">9</option>
                            <option value="10">10</option>
                            <option value="11">11</option>
                            <option value="12">12</option>
                            <option value="13">13</option>
                            <option value="14">14</option>
                            <option value="15">15</option>
                            <option value="16">16</option>
                            <option value="17">17</option>
                            <option value="18">18</option>
                            <option value="19">19</option>
                            <option value="20">20</option>
                            <option value="21">21</option>
                            <option value="22">22</option>
                            <option value="23">23</option>
                            <option value="24">24</option>
                            <option value="25">25</option>
                            <option value="26">26</option>
                            <option value="27">27</option>
                            <option value="28">28</option>
                            <option value="29">29</option>
                            <option value="30">30</option>
                            <option value="31">31</option>
                        </select>
                    </label>
                    <label style="display: inline-block;">
                        <span data-i18n="">Mes</span>
                        <select style="display: inline-block; width: 100px;" id="visit-repeattime-yearly-month">
                            <option value="1"><span data-i18n="">Enero</span></option>
                            <option value="2"><span data-i18n="">Febrero</span></option>
                            <option value="3"><span data-i18n="">Marzo</span></option>
                            <option value="4"><span data-i18n="">Abril</span></option>
                            <option value="5"><span data-i18n="">Mayo</span></option>
                            <option value="6"><span data-i18n="">Junio</span></option>
                            <option value="7"><span data-i18n="">Julio</span></option>
                            <option value="8"><span data-i18n="">Agosto</span></option>
                            <option value="9"><span data-i18n="">Septiembre</span></option>
                            <option value="10"><span data-i18n="">Octubre</span></option>
                            <option value="11"><span data-i18n="">Noviembre</span></option>
                            <option value="12"><span data-i18n="">Diciembre</span></option>
                        </select>
                    </label>
                </div>
            </div>
        </div>
        <div class="row mod-visit-row" id="visit-row-employee">
            <div class="large-3 column large-text-right medium-text-left small-text-left">
                <label for="whoisvisiting" data-i18n="whoisvisiting">A quien se visita:</label>
            </div>
            <div class="large-9 column large-text-left medium-text-left small-text-left">
                <div id="mod-visit-employee"></div>
            </div>
        </div>
        <div id="mod-visit-fields"></div>
        <div class="row mod-visit-row" id="mod-visit-createdby">
            <div class="large-3 column large-text-right medium-text-left small-text-left">
                <label data-i18n="createdbyname">Creado por:</label>
            </div>
            <div class="large-9 column text-left">
                <span id="mod-visit-createdbyname"></span>
            </div>
        </div>
        <div class="row mod-visit-row" id="visit-row-punches">
            <div class="large-3 column large-text-right medium-text-left small-text-left">
                <label data-i18n="punches">Fichajes:</label>
            </div>
            <div class="large-9 column text-left">
                <div id="mod-visit-punches"></div>
            </div>
        </div>
        <div class="row mod-visit-row">
            <div class="column text-right" id="mod-visit-buttons">
            </div>
        </div>
    </div>
    <!-- ================== FIN VISITA ================== -->
    <!-- ================== INICIO VISITANTE ================== -->
    <div id="mod-visitor" class="reveal-modal small" data-reveal>
        <a class="close-reveal-modal">&#215;</a>
        <div class="row title"><i class="fa fa-user fa-fw"></i>&nbsp;<span data-i18n="visitor">Visitante</span></div>
        <div class="row" id="mod-visitor-response"></div>
        <div class="row mod-visitor-row">
            <div class="large-3 column large-text-right medium-text-left small-text-left">
                <label class="inline" data-i18n="name" for="mod-visitor-name">Nombre :</label>
            </div>
            <div class="large-9 column large-text-left medium-text-left small-text-left">
                <input type="hidden" value="" id="mod-visitor-idvisitor" />
                <input type="text" value="" name="mod-visitor-name" id="mod-visitor-name" maxlength="100" />
                <div class="alert-box alert-input">Invalid entry</div>
            </div>
        </div>

        <div class="row mod-visitor-row">
            <div class="large-3 column large-text-right medium-text-left small-text-left">
                <label class="inline" data-i18n="private" for="mod-visitor-private">Privado:</label>
            </div>
            <div class="large-9 column large-text-left medium-text-left small-text-left">
                &nbsp;<input type="checkbox" style="margin-top: 18px;" value="1" name="mod-visitor-private" id="mod-visitor-private" />
            </div>
        </div>
        <div id="mod-visitor-fields">
        </div>
        <div class="row mod-visitor-row" id="visitor-row-punches">
            <div class="large-3 column large-text-right medium-text-left small-text-left">
                <label data-i18n="punches">Fichajes:</label>
            </div>
            <div class="large-9 column text-left">
                <div id="mod-visitor-punches"></div>
            </div>
        </div>
        <div class="row mod-visitor-row" id="mod-visitor-buttons">
            <div class="column text-right">
                <a class="button tiny align-left" id="mod-visitorlist-buttons-addfromscanner" onclick="getVisitorDataFromScanner()"><span data-i18n="addfromscanner">Escanear documento</span></a>
                <a class="button tiny" data-i18n="save" id="mod-visitor-buttons-save" onclick="saveVisitor()">Guardar</a>&nbsp;
                <a class="button tiny" data-event data-i18n="cancel" onclick="closeModal('#mod-visitor')" id="mod-visitor-buttons-close">Cancelar</a>
            </div>
        </div>
    </div>
    <!-- ================== FIN VISITANTE ================== -->
    <!-- ================== INICIO CAMPO DE VISITA ================== -->
    <div id="mod-visitfield" class="reveal-modal small" data-reveal>
        <a class="close-reveal-modal">&#215;</a>
        <div class="row title"><i class="fa fa-list"></i>&nbsp;<span data-i18n="visitfield">Campo de visita</span></div>
        <div class="row" id="mod-visitfield-response"></div>
        <div class="row mod-visitfield-row">
            <div class="large-2 medium-3 column large-text-right medium-text-right small-text-left">
                <label class="inline" data-i18n="name" for="mod-visitfield-name">Nombre:</label>
            </div>
            <div class="large-10 medium-9 column large-text-left medium-text-left small-text-left">
                <input type="hidden" value="" id="mod-visitfield-idfield" />
                <input type="text" value="" name="mod-visitfield-name" id="mod-visitfield-name" maxlength="100" />
            </div>
        </div>
        <div class="row mod-visitfield-row">
            <div class="large-2 medium-3 column large-text-right medium-text-right small-text-left">
                <label class="inline" data-i18n="visible" for="mod-visitfield-visible">Visible:</label>
            </div>
            <div class="large-10 medium-9 column large-text-left medium-text-left small-text-left">
                &nbsp;<input type="checkbox" style="margin-top: 18px;" value="1" name="mod-visitfield-visible" id="mod-visitfield-visible" />
            </div>
        </div>
        <div class="row mod-visitfield-row">
            <div class="large-2 medium-3 column large-text-right medium-text-right small-text-left">
                <label class="inline" data-i18n="required" for="mod-visitfield-required">Requerido:</label>
            </div>
            <div class="large-10 medium-9 column large-text-left medium-text-left small-text-left">
                <select name="mod-visitfield-required" id="mod-visitfield-required">
                    <option value="0" data-i18n="notrequired">No requerido</option>
                    <option value="1" data-i18n="oncreatevisit">Al crear la visita</option>
                    <option value="2" data-i18n="onstartvisit">Al iniciar la visita</option>
                    <option value="3" data-i18n="onendvisit">Al finalizar la visita</option>
                </select>
            </div>
        </div>
        <div class="row mod-visitfield-row">
            <div class="large-2 medium-3 column large-text-right medium-text-right small-text-left">
                <label class="inline" data-i18n="type" for="mod-visitfield-type">Tipo de visita:</label>
            </div>
            <div class="large-10 medium-9 column large-text-left medium-text-left small-text-left">
                <select name="mod-visitfield-type" id="mod-visitfield-type">
                </select>
            </div>
        </div>
        <div class="row mod-visitfield-row" id="row-mod-visitfield-type">
            <div class="large-2 medium-3 column large-text-right medium-text-right small-text-left">
                <label class="inline" data-i18n="typefield" for="mod-visitfield-type">Tipo:</label>
            </div>
            <div class="large-10 medium-9 column large-text-left medium-text-left small-text-left">
                <label style="display: inline-block;">
                    &nbsp;<input type="radio" value="t" style="margin-top: 13px;" id="mod-visitfield-type-t" name="mod-visitfield-type" checked /><span data-i18n="texttype">Tipo texto</span>
                </label>
                &nbsp;
                <label style="display: inline-block;">
                    <input type="radio" value="d" style="margin-top: 13px;" id="mod-visitfield-type-d" name="mod-visitfield-type" /><span data-i18n="listtype">Tipo desplegable</span>
                </label>
                &nbsp;
            </div>
        </div>
        <div class="row mod-visitfield-row" id="mod-visitfield-valuesdiv">
            <div class="large-2 medium-3 column large-text-right medium-text-right small-text-left">
                <label class="inline" data-i18n="values" for="mod-visitfield-values">Valores:</label>
            </div>
            <div class="large-10 medium-9 column large-text-left medium-text-left small-text-left">
                <input type="text" value="" name="mod-visitfield-values" id="mod-visitfield-values" maxlength="300"
                    class="has-tip" data-tooltip aria-haspopup="true" title="Valores del desplegable serparados por ';'" />
            </div>
        </div>

        <div class="row mod-visitfield-row" id="row-mod-visitfield-edit">
            <div class="large-2 medium-3 column large-text-right medium-text-right small-text-left">
                <label class="inline" data-i18n="typeModified" for="mod-visitfield-type">Permitir modificar:</label>
            </div>
            <div class="large-10 medium-9 column large-text-left medium-text-left small-text-left">
                <label style="display: inline-block;">
                    &nbsp;<input type="checkbox" value="t" style="margin-top: 13px;" id="mod-visitfield-edit" name="mod-visitfield-edit" checked />
                </label>
            </div>
        </div>

        <div class="row mod-visitfield-row" id="mod-visitfield-buttons">
            <div class="column text-right">
                <a class="button  tiny" data-i18n="save" id="mod-visitfield-buttons-save" onclick="saveVisitField()">Guardar</a>&nbsp;
                <a class="button tiny" data-event data-i18n="cancel" onclick="closeModal('#mod-visitfield')" id="mod-visitfield-buttons-close">Cancelar</a>
            </div>
        </div>
    </div>
    <!-- ================== FIN CAMPO DE VISITA ================== -->
    <!-- ================== INICIO CAMPO DE TIPO DE VISITA ================== -->
    <div id="mod-visittype" class="reveal-modal small" data-reveal>
        <a class="close-reveal-modal">&#215;</a>
        <div class="row title"><i class="fa fa-list"></i>&nbsp;<span data-i18n="visittype">Tipo de visita</span></div>
        <div class="row" id="mod-visittype-response"></div>
        <div class="row mod-visittype-row">
            <div class="large-2 medium-3 column large-text-right medium-text-right small-text-left">
                <label class="inline" data-i18n="name" for="mod-visittype-name">Nombre:</label>
            </div>
            <div class="large-10 medium-9 column large-text-left medium-text-left small-text-left">
                <input type="hidden" value="" id="mod-visittype-idtype" />
                <input type="text" value="" name="mod-visittype-name" id="mod-visittype-name" maxlength="100" />
            </div>
        </div>
        <div class="row mod-visittype-row" id="mod-visittype-buttons">
            <div class="column text-right">
                <a class="button  tiny" data-i18n="save" id="mod-visittype-buttons-save" onclick="saveVisitType()">Guardar</a>&nbsp;
                <a class="button tiny" data-event data-i18n="cancel" onclick="closeModal('#mod-visittype')" id="mod-visittype-buttons-close">Cancelar</a>
            </div>
        </div>
    </div>
    <!-- ================== FIN CAMPO DE TIPO DE VISITA ================== -->
    <!-- ================== INICIO CAMPO DE VISITANTE ================== -->
    <div id="mod-visitorfield" class="reveal-modal small" data-reveal>
        <a class="close-reveal-modal">&#215;</a>
        <div class="row title"><i class="fa fa-list"></i>&nbsp;<span data-i18n="visitorfield">Campo de visitante</span></div>
        <div class="row" id="mod-visitorfield-response"></div>
        <div class="row mod-visitorfield-row">
            <div class="large-2 medium-3 column large-text-right medium-text-right small-text-left">
                <label class="inline" data-i18n="name" for="mod-visitorfield-name">Nombre:</label>
            </div>
            <div class="large-10 medium-9 column large-text-left medium-text-left small-text-left">
                <input type="hidden" value="" id="mod-visitorfield-idfield" />
                <input type="text" value="" name="mod-visitorfield-name" id="mod-visitorfield-name" maxlength="100" />
            </div>
        </div>
        <div class="row mod-visitorfield-row">
            <div class="large-2 medium-3 column large-text-right medium-text-right small-text-left">
                <label class="inline" data-i18n="visible" for="mod-visitorfield-name">Visible:</label>
            </div>
            <div class="large-10 medium-9 column large-text-left medium-text-left small-text-left">
                &nbsp;<input type="checkbox" style="margin-top: 18px;" value="1" name="mod-visitorfield-visible" id="mod-visitorfield-visible" />
            </div>
        </div>
        <div class="row mod-visitorfield-row">
            <div class="large-2 medium-3 column large-text-right medium-text-right small-text-left">
                <label class="inline" data-i18n="required" for="mod-visitorfield-required">Requerido:</label>
            </div>
            <div class="large-10 medium-9 column large-text-left medium-text-left small-text-left">
                <select name="mod-visitorfield-required" id="mod-visitorfield-required">
                    <option value="0" data-i18n="notrequired">No requerido</option>
                    <option value="1" data-i18n="oncreatevisitor">Al crear el visitante</option>
                    <option value="2" data-i18n="oncreatevisit">Al crear la visita</option>
                    <option value="3" data-i18n="onstartvisit">Al iniciar la visita</option>
                </select>
            </div>
        </div>
        <div class="row mod-visitorfield-row mod-visitorfield-row-required hide">
            <div class="large-2 medium-3 column large-text-right medium-text-right small-text-left">
                <label class="inline" data-i18n="askevery" for="mod-visitorfield-askevery">Preguntar cada:</label>
            </div>
            <div class="large-10 medium-9 column large-text-left medium-text-left small-text-left">
                <select name="mod-visitorfield-askevery" id="mod-visitorfield-askevery" style="margin-top: 17px !important;">
                    <option value="0">&nbsp;</option>
                    <option value="1" data-i18n="onetime">Una sola vez</option>
                    <option value="2" data-i18n="everystartvisit">Cada vez al iniciar la visita</option>
                    <option value="3" data-i18n="every30days">Cada 30 días</option>
                    <!--<option value="4" data-i18n="alwaysEdit">Siempre editable</option>-->
                </select>
            </div>
        </div>
        <div class="row mod-visitorfield-row">
            <div class="large-2 medium-3 column large-text-right medium-text-right small-text-left">
                <label class="inline" data-i18n="typefield" for="mod-visitorfield-type">Tipo:</label>
            </div>
            <div class="large-10 medium-9 column large-text-left medium-text-left small-text-left">
                <label style="display: inline-block;">
                    &nbsp;<input type="radio" value="t" style="margin-top: 13px;" id="mod-visitorfield-type-t" name="mod-visitorfield-type" checked />
                    <span data-i18n="texttype">Tipo texto</span>
                </label>
                &nbsp;
                <label style="display: inline-block;">
                    <input type="radio" value="d" style="margin-top: 13px;" id="mod-visitorfield-type-d" name="mod-visitorfield-type" /><span data-i18n="listtype">Tipo desplegable</span>
                </label>
            </div>
        </div>

        <div class="row mod-visitfield-row" id="mod-visitorfield-valuesdiv">
            <div class="large-2 medium-3 column large-text-right medium-text-right small-text-left">
                <label class="inline" data-i18n="values" for="mod-visitorfield-values">Valores:</label>
            </div>
            <div class="large-10 medium-9 column large-text-left medium-text-left small-text-left">
                <input type="text" value="" name="mod-visitorfield-values" id="mod-visitorfield-values" maxlength="300"
                    class="has-tip" data-tooltip aria-haspopup="true" title="Valores del desplegable serparados por ';'" />
            </div>
        </div>

        <div class="row mod-visitorfield-row">
            <div class="large-2 medium-3 column large-text-right medium-text-right small-text-left">
                <label class="inline" data-i18n="typeModified" for="mod-visitorfield-type">Permitir modificar:</label>
            </div>
            <div class="large-10 medium-9 column large-text-left medium-text-left small-text-left">
                <label style="display: inline-block;">
                    &nbsp;<input type="checkbox" value="t" style="margin-top: 13px;" id="mod-visitorfield-edit" name="mod-visitorfield-edit" checked />
                </label>
            </div>
        </div>

        <div class="row mod-visitorfield-row" id="mod-visitorfield-buttons">
            <div class="column text-right">
                <a class="button tiny" data-i18n="save" id="mod-visitorfield-buttons-save" onclick="saveVisitorField()">Guardar</a>&nbsp;
                <a class="button tiny" data-event data-i18n="cancel" onclick="closeModal('#mod-visitorfield')" id="mod-visitorfield-buttons-close">Cancelar</a>
            </div>
        </div>
    </div>
    <!-- ================== FIN CAMPO DE VISITA ================== -->
    <!-- ================== INICIO LISTA EMPLEADOS ================== -->
    <div id="mod-employeelist" class="reveal-modal medium" data-reveal>
        <a class="close-reveal-modal">&#215;</a>
        <div class="row title"><i class="fa fa-user fa-fw"></i>&nbsp;<span data-i18n="listofemployees">Empleados</span></div>
        <div class="hide-on-print" id="mod-employeelist-response"></div>
        <div class="hide-on-print">
        </div>
        <div id="mod-employeelist-content"></div>
        <div class="right" id="mod-employeelist-buttons">
            <a class="button tiny" id="mod-employeelist-buttons-add" onclick="addEmployee2Visit()"><span data-i18n="add">Añadir</span></a>&nbsp;
            <a class="button tiny" onclick="closeModal('#mod-employeelist')" data-event id="mod-employeelist-buttons-cancel"><span data-i18n="cancel">Cancelar</span></a>
        </div>
    </div>
    <!-- ==================== FIN LISTA EMPLEADOS =================== -->
    <!-- ================== INICIO LISTA VISITANTES ================== -->
    <div id="mod-visitorlist" class="reveal-modal medium" data-reveal>
        <a class="close-reveal-modal">&#215;</a>
        <div class="row title"><i class="fa fa-user fa-fw"></i>&nbsp;<span data-i18n="listofvisitors">Visitante</span></div>
        <div class="hide-on-print" id="mod-visitorlist-response"></div>
        <div class=" hide-on-print">
        </div>
        <div id="mod-visitorlist-content"></div>
        <div id="mod-visitorlist-buttons">
            <a class="button tiny" onclick="showVisitor('new',true);"><span data-i18n="new_visitor">Nuevo visitante...</span></a>
            <a class="button tiny" id="mod-visitorlist-buttons-cancel" data-event onclick="closeModal('#mod-visitorlist')"><span data-i18n="cancel">Cancelar</span></a>
            <a class="button tiny" id="mod-visitorlist-buttons-add" onclick="addVisitor2Visit()"><span data-i18n="add">Añadir</span></a>&nbsp;
        </div>
    </div>
    <!-- ==================== FIN LISTA VISITANTES =================== -->
    <!-- ================== INICIO PREGUNTAS ================== -->
    <div id="mod-question" class="reveal-modal small" data-reveal>
        <a class="close-reveal-modal">&#215;</a>
        <div class="row title" id="mod-question-title">&nbsp;</div>
        <div id="mod-question-question"></div>
        <div class="right" id="mod-question-buttons">
            <a class="button tiny" id="mod-question-buttons-yes"><span data-i18n="yes">Si</span></a>&nbsp;
            <a class="button tiny" data-event onclick="closeModal('#mod-question')" id="mod-question-buttons-no"><span data-i18n="no">No</span></a>
        </div>
    </div>
    <!-- ==================== FIN PREGUNTAS =================== -->
    <!-- ================== INICIO CAMBIO DE CONTRASEÑA ================== -->
    <div id="mod-changepassword" class="reveal-modal tiny" data-reveal>
        <div id="mod-changepassword-form">
            <div class="formtext text-center"><span data-i18n="changepassword">Cambio de contraseña</span></div>
            <div>
                <div class="loginfrmlabel2"><span data-i18n="currentpass">Contraseña actual:</span></div>
                <div class="loginfrminput2">
                    <input type="password" id="currentpass" />
                </div>
            </div>
            <div>
                <div class="loginfrmlabel2"><span data-i18n="newpass">Contraseña nueva:</span></div>
                <div class="loginfrminput2">
                    <input type="password" class="" id="newpassword" />
                </div>
            </div>
            <div>
                <div class="loginfrmlabel2"><span data-i18n="newpass2">Repetir contraseña:</span></div>
                <div class="loginfrminput2">
                    <input type="password" class="" id="newpassword2" />
                </div>
            </div>
            <div id="mod-changepassword-response"></div>
            <div id="mod-changepassword-buttons">
                <div class="text-right">
                    <a class="button tiny" onclick="changePassword()"><span data-i18n="change">Modficar</span></a>
                    <a class="button tiny" onclick="closeModal('#mod-changepassword')"><span data-i18n="close">Cerrar</span></a>
                </div>
            </div>
        </div>
    </div>
    <!-- ================== INICIO CAMBIO DE IDIOMA ================== -->
    <div id="mod-changelang" class="reveal-modal tiny" data-reveal>
        <div id="mod-changelang-form">
            <div class="formtext text-center"><span data-i18n="changelang">Cambio de idioma</span></div>
            <div>
                <div>
                    <select id="mod-changelang-lang">
                        <option value="es"><span data-i18n="spanish">Español</span></option>
                        <option value="ca"><span data-i18n="catalan">Català</span></option>
                        <option value="en"><span data-i18n="english">English</span></option>
                    </select>
                </div>
            </div>
        </div>
        <div id="mod-changelang-response"></div>
        <div id="mod-changelang-buttons">
            <div class="text-right">
                <a class="button tiny" onclick="changeLangForm()"><span data-i18n="change">Modficar</span></a>
                <a class="button tiny" onclick="closeModal('#mod-changelang')"><span data-i18n="close">Cerrar</span></a>
            </div>
        </div>
    </div>
    <div id="ENSpopup">
    </div>
</body>
</html>