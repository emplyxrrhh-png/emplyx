//import { callbackify } from "util";
var modals = [];
var modalopening = "";
var modalclosing = "";
var _usr = { tku: "0000", id: 0, lang: "es", hasaccess: 0, hascreate: 0, hascreateEmployee: 0, haschange: 0, hasfields: 0, idemployee: 0 };
var _nextStatus = -1;
var _showPunches = false;
var _currVisit = {};
var _currVisitList = {};
var _currVisitSearchList = {};
var _currVisitSearchValues = { where: "", value: "", orderBy: "hour", _loadpunches_: true };
var _currVisitFilterList = { date: "", _loadpunches_: true }; //s'usa per anar actualitzant cada x segons
var _currVisitFilter = {};
var _currVisitFields = [];
var _currVisitorFields = [];
var _currVisitorFilter = { _loadpunches_: false };
var _currVisitorListFilter = { ids: "", _loadpunches_: true };
var _currOptions = { "cardfield": "", "notification": "", "allowvisitfieldmodify": "" };
var _lastVisitChange = moment();
var _lastResponse = 0;
var _newVisitor = "";
var _refreshVisitVisitor = true;
var datatablesOpt = { language: { "url": 'locales/datatables.' + _usr.lang + '.json', "pageLength": 10 } };
var datatablesOptOrd = { language: { "url": 'locales/datatables.' + _usr.lang + '.json' }, "pageLength": 10, "order": [[1, "asc"]] };
var _columns_visitors_visible = [];

var _visitorsCache = null;
var _visitorsCacheFilter = "";
var _visitorsGestCache = null;
var _visitorsGestCacheFilter = "";

var _visitorsSearchCache = null;
var _visitorsSearchFilter = "";

var labelseditor;
var labelcontent;
var labelsquill;
var labelsLastPos = 0;
var dsFields = ["Fecha", "Hora actual", "Visitante", "Asunto", "Visitado", "ID Visita", "ID Visitante"];
var lawseditor;

$(document).ready(function () {
    $("[id$='mod-visit-type']").change(function () {
        getVisit("new", this.value);
    })
});
function initGUI() {
    $("#topBar .button").each(function (index) {
        $(this).removeClass("active");
    });

    $(".tab").each(function (index) {
        $(this).removeClass("show");
        $(this).addClass("hide");
    });

    $("[data-subtab]").each(function (index) {
        if ($(this).hasClass("button")) {
            $(this).removeClass("active");
        } else {
            $(this).removeClass("show");
            $(this).addClass("hide");
        }
    });
}

var clk = setInterval(function () {
    if (_lastResponse == 0) { getLastVisitChange(); }
}, 30000);

function getVersion() {
    return "6.0.2.0";
}

function changeToolTips() {
    $("[data-tooltip]").each(function (index) {
        Foundation.libs.tooltip.getTip($("#" + this.id)).html($.t("tooltip-" + this.id, "") + '<span class="nub"></span>')
    });
}

function changeLocale(locale, bLoadList) {
    _usr.lang = locale;
    localStorage.setItem("usr", JSON.stringify(_usr));
    moment.locale(locale);
    datatablesOpt = { language: { "url": 'locales/datatables.' + _usr.lang + '.json', "pageLength": 12 } };
    datatablesOptOrd = { language: { "url": 'locales/datatables.' + _usr.lang + '.json' }, "pageLength": 12, "order": [[1, "asc"]] };
    i18n.setLng(locale, function (t) {
        $(document).i18n();
    });
    $("#tab-plani-date-value").html(moment().format('LL'));
    // $.datetimepicker.setLocale(locale);

    var bReload = typeof bLoadList == 'undefined' ? false : bLoadList;
    if (bReload) getVisitList(_currVisitFilterList, true);

    changeToolTips();
    $("mod-changelang-lang").val(_usr.lang);

    var path = window.location.href.substr(0, window.location.href.indexOf('index.aspx'));
    $.getJSON(path + "js/cldr/locales/likelySubtags.json", function (data) { Globalize.load(data) });
    $.getJSON(path + "js/cldr/locales/environment-ca.json", function (data) { Globalize.load(data) });
    $.getJSON(path + "js/cldr/locales/environment-en.json", function (data) { Globalize.load(data) });
    $.getJSON(path + "js/cldr/locales/environment-es.json", function (data) { Globalize.load(data) });
    $.getJSON(path + "js/cldr/locales/environment-gl.json", function (data) { Globalize.load(data) });
    $.getJSON(path + "js/cldr/locales/environment-pt.json", function (data) { Globalize.load(data) });
    setTimeout(function () {
        $.getJSON(path + "js/localization/dx.all." + _usr.lang + ".json", function (data3) {
            Globalize.loadMessages(data3);
            Globalize.locale(_usr.lang);
        });
    }, 1000)
}

function activeTab(name, subname) {
    //activamos el tab necesario
    $("#topBar .button").each(function (index) {
        if ($(this).attr("data-tab") == name) {
            $(this).addClass("active");
        } else {
            $(this).removeClass("active");
        }
    });

    //activamos el contenido necesario
    $(".tab").each(function (index) {
        if (this.id == "tab-" + name) {
            $(this).addClass("show");
            $(this).removeClass("hide");
        } else {
            $(this).removeClass("show");
            $(this).addClass("hide");
        }
    });
    if (name != "plani") {
        $("#plani-right").addClass("hide");
    } else {
        $("#plani-right").removeClass("hide");
    }
    //activamos subtab necesario
    $("[data-subtab]").each(function (index) {
        if ($(this).attr("data-subtab") == subname) {
            if ($(this).hasClass("button")) {
                $(this).addClass("active");
            } else {
                $(this).addClass("show");
                $(this).removeClass("hide");
            }
        } else {
            if ($(this).hasClass("button")) {
                $(this).removeClass("active");
            } else {
                $(this).removeClass("show");
                $(this).addClass("hide");
            }
        }
    });

    if (_currVisitSearchValues.value.length > 0) {
        $("a[data-subtab='search']").removeClass("hide");
        //if (_.isEmpty(_currVisitSearchList)){
        getVisitListSearch(_currVisitSearchValues);
        //} else {
        //    parseVisitListSearch(_currVisitSearchList);
        //}
    } else {
        $("a[data-subtab='search']").addClass("hide");
    }

    switch (name) {
        case 'gest':
            switch (subname) {
                case 'visits':
                    var count = 0;
                    try {
                        count = $("#gest-visits-devextreme").dxDataGrid('instance').totalCount();
                    } catch (e) { count = 0; }
                    if (count == 0) {
                        getVisitGest(_currVisitFilter);
                    }
                    break;
                case 'visitors':
                    if ($("#gest-visitors-table").length == 0) {
                        getVisitorGest(_currVisitorFilter, false);
                    }
                    break;
            }
            break;
        case 'opt':
            switch (subname) {
                case 'general':
                    updateFieldsList();
                    updateVisitorFieldList();
                    updateZoneList();
                    getOptions();
                    break;
                case 'visitfields':
                    if ($("#opt-visitfields table").length == 0) {
                        getVisitFieldsOpt();
                    }
                    break;
                case 'visitorfields':
                    if ($("#opt-visitorfields table").length == 0) {
                        getVisitorFieldsOpt();
                    }
                    break;
                case 'visittypes':
                    if ($("#opt-visittypes table").length == 0) {
                        getVisitTypesOpt();
                    }
                    break;
                case 'printconfig':

                    getCurrentPrintConfig();
                case 'regtemplates':

                    getCurrentLegalTexts();
            }
    }
}
function executeTask(object, action, id) {
    switch (object) {
        case "visit":
            switch (action) {
                case "del":
                    showQuestion($.t("visits", "Visitas"), $.t("deletevisits", "Desea borrar la visita seleccionada?"), delVisitGest, id);
                    break;
                default:
                    _showPunches = false;
                    showVisit(id);
            }
            break;
        case "visitor":
            switch (action) {
                case "del":
                    showQuestion($.t("visitor", "Visitante"), $.t("deletevisitor", "Desea borrar el visitante seleccionado?"), delVisitorGest, id);
                    break;
                case "new":
                    _nextStatus = -1;
                    _showPunches = false;
                    showVisitor("new");
                    break;
                default:
                    _nextStatus = -1;
                    _showPunches = true;
                    showVisitor(id);
            }
            break;
        case "visitfield":
            switch (action) {
                case "del":
                    showQuestion($.t("visitfield", "Campo de visitas"), $.t("deletevisitfield", "Desea borrar el campo de visita seleccionado?"), delVisitFieldsOpt, id);
                    break;
                default:
                    showVisitField(id);
            }
            break;
        case "visittype":
            switch (action) {
                case "del":
                    showQuestion($.t("visittype", "Tipo de visita"), $.t("deletevisittype", "Desea borrar el tipo de visita seleccionado?"), delVisitTypesOpt, id);
                    break;
                default:
                    showVisitType(id);
            }
            break;
        case "visitorfield":
            switch (action) {
                case "del":
                    showQuestion($.t("visitorfield", "Campo de visitantes"), $.t("deletevisitorfield", "Desea borrar el campo de visitantes seleccionado?"), delVisitorFieldsOpt, id);
                    break;
                default:
                    showVisitorField(id);
            }
            break;
    }
}
function refreshReset() {
    //visitors screen
    $(".resetinput").off('click');
    $(".resetinput").off("click").click(function () {
        $(this).next().val("");
    });

    //visits screen
    $(".resetvisitsinput").off('click');
    $(".resetvisitsinput").off("click").click(function () {
        $(this).next().val("");
        saveVisitsFilter();
    });
}

function rologout(force) {
    var params = 'userId=' + _usr.id;
    localStorage.removeItem("usr");
    $.removeCookie("tku")
    if (force) { localStorage.setItem("hash", location.hash); }
    _usr = {};

    if (params != '') params += '&timestamp=' + new Date().getTime();
    else params = 'timestamp=' + new Date().getTime();

    var jqxhr = callRoboticsWS_GET(jsonEngineURI + "Logout", params, function (data) {
    });
    location.href = location.origin + location.pathname.substring(0, location.pathname.lastIndexOf("/") + 1) + "login.aspx";
}

function hasError(data) {
    try {
        if (_.has(data.d, "result")) {
            if (data.d.result == "NO_SESSION") {
                //TODO: ROBOTICS
                localStorage.setItem("roerror", "nosession");
                rologout(true)
                return false;
            } else if (data.d.result == "NoError" || data.d.result == "Ok") {
                return false;
            } else {
            }
        } else {
            return false;
        }
    }
    catch (err) {
        return true;
    }
    return true;
}

function closeModal(name) {
    $(name).foundation('reveal', 'close');
}

function openModal(name) {
    $(name).foundation('reveal', 'open');
}

function updateClock() {
    $("#tab-plani-time-value").html(moment().format("HH:mm:ss"));
}
function roChecklogin() {
    var params = 'timestamp=' + new Date().getTime();
    var jqxhr = callRoboticsWS_POST(jsonEngineURI + "AuthenticateSession", params, function (data) {
        if (data.d.status == 0) {
            _usr.tku = data.d.token;
            _usr.lang = data.d.language;
            _usr.id = data.d.userid;
            _usr.idemployee = data.d.idemployee;
            _usr.hasaccess = data.d.hasaccess;
            _usr.hascreate = data.d.hascreate;
            _usr.hascreateEmployee = data.d.hascreateEmployee;
            _usr.haschange = data.d.haschange;
            _usr.hasfields = data.d.hasfields;
            checkPermisions();
            switch (_usr.lang) {
                case 'CAT': changeLocale('ca'); break;
                case 'ESP': changeLocale('es'); break;
                case 'ENG': changeLocale('en'); break;
            }

            localStorage.setItem("usr", JSON.stringify(_usr));
            $.cookie("tku", _usr.tku, { expires: 2 })
            localStorage.removeItem("hash")
        } else {
            rologout(true)
        }
    });
}

function checkPermisions() {
    if (_usr.hasaccess < 3) {
        $("[data-subtab=general]").remove();
    }
    if (_usr.haschange < 2) {
        $(".button[data-tab='plani']").remove();
        $("#tab-plani").remove();
    }
    if (_usr.hascreate == 0 && _usr.hascreateEmployee == 0) {
        $("a>span[data-i18n='new_visit']").parent().remove();
    }
    if (_usr.hascreate == 0 && _usr.hascreateEmployee > 0) {
        $("#visit-row-employee").remove();
        $("#mod-visitorlist-buttons-addfromscanner").remove();
        $("[data-subtab=visitors]").remove();
        $("#mainmenu-tabs").remove();
    }

    if (_usr.hasfields < 3) {
        $("#new_visitfield").remove();
        $("#new_visittype").remove();
    }
    if (_usr.hasfields < 2) {
        $("[data-subtab=visitfields]").remove();
        $("[data-subtab=visitorfields]").remove();
    }
}

/****************************************************************/
/************             Visitas                ****************/
function getLastVisitChange() {
    if (_usr.tku != "0000") {
        var params = 'filter=' + encodeURIComponent(JSON.stringify(_currVisitFilterList));
        params += '&timestamp=' + Date.now()
        _lastResponse = 1;

        if (params != '') params += '&timestamp=' + new Date().getTime();
        else params = 'timestamp=' + new Date().getTime();

        var jqxhr = callRoboticsWS_GET(jsonEngineURI + "lastvisitchange", params, function (data) {
            if (!hasError(data)) {
                if (moment(data.d.lastdate).isValid()) {
                    var curr = moment(data.d.lastdate);
                    //console.log(curr.format());
                    if (curr.isAfter(_lastVisitChange)) {
                        getVisitList(_currVisitFilterList);
                        _lastVisitChange = curr;
                    }
                }
            }
            _lastResponse = 0;
        });
    }
    if ($("#tab-plani-date-value").html() != moment().format('LL')) {
        _currVisitFilterList.date = moment().toISOString();
        $("#tab-plani-date-value").html(moment().format('LL'));
        getVisitList(_currVisitFilterList, true);
    }
}

function getVisitUniqueID() {
    var params = 'idField=' + _currOptions.uninqueIDField;
    var jqxhr = callRoboticsWS_GET(jsonEngineURI + "getuniqueid", params, function (data) {
        if (!hasError(data)) {
            $(".mod-visit-field input[data-idfield='" + _currOptions.uninqueIDField + "']").val(data.d);
        }
    });
}

function saveVisitsFilter() {
    if ($("#gest-visits-filters-begin").val().length > 0) {
        _currVisitFilter["begindate"] = $("#gest-visits-filters-begin").val();
    } else {
        _currVisitFilter["begindate"] = "";
    }
    if ($("#gest-visits-filters-end").val().length > 0) {
        _currVisitFilter["enddate"] = $("#gest-visits-filters-end").val();
    } else {
        _currVisitFilter["enddate"] = "";
    }
    localStorage.setItem("currVisitFilter", JSON.stringify(_currVisitFilter));
}

function loadVisitsFilter() {
    $("#gest-visits-filters-begin").val(_currVisitFilter["begindate"]);
    $("#gest-visits-filters-end").val(_currVisitFilter["enddate"]);
}
function getVisitList(filter, parse) {
    _currVisitList = {};
    var params = 'timestamp=' + new Date().getTime();

    if (_currVisitSearchValues.orderBy.length > 0) {
        filter.orderBy = _currVisitSearchValues.orderBy;
    } else {
        filter.orderBy = "hour";
    }
    if ($("#plani-Location-selector").val() != "" && $("#plani-Location-selector").val() != null) {
        filter.location = $("#plani-Location-selector").val();
    }

    if (filter != "")
        params += '&filter=' + encodeURIComponent(JSON.stringify(filter));

    //parseVisitList({ visits: [] }, true);

    var jqxhr = callRoboticsWS_GET(jsonEngineURI + "visitlist", params, function (data) {
        if (!hasError(data)) {
            _currVisitList = data.d;
            //ordenamos por fecha
            //_currVisitList.visits.sort(function (a, b) { return moment(a.begindate).isAfter(b.begindate); });

            if (parse) {
                parseVisitList(_currVisitList, false);
            } else {
                $("#newVisits").slideDown();
            }
        } else {
            $("#visits-response").html(showErrorClose({ text: data.result }));
        }
    }, function () {
        $("#visits-response").html(showErrorClose({ text: $.t("unknownerror", "Error, aviste a soporte.") }));
    });
}
function parseVisitList(data, bIsLoading) {
    $("#subtab-visits").html("");
    $("#subtab-scheduled").html("");
    $("#subtab-inprogress").html("");
    $("#subtab-finished").html("");
    $("#subtab-notpresented").html("");
    var std = { scheduled: 0, inprogress: 0, finished: 0, notpresented: 0 };

    if (_.isString(_currVisitFilterList.visitor)) {
        $("#plani-search-visitor").val(_currVisitFilterList.visitor);
    } else {
        $("#plani-search-visitor").val("");
    }
    if (_.isString(_currVisitFilterList.employee)) {
        $("#plani-search-employee").val(_currVisitFilterList.employee);
    } else {
        $("#plani-search-employee").val("");
    }
    if (_.isString(_currVisitFilterList.location)) {
        $("#plani-Location-selector").val(_currVisitFilterList.location);
    } else {
        $("#plani-Location-selector").val("");
    }

    $(data.visits).each(function (index) {
        var vst = this;
        getEmployeeStatus(vst.idemployee);
        var lnk = "#/plani/";
        var ico = "";
        var tmp = '<div class="row visit-item '
        switch (vst.status) {
            case 0:
                tmp += 'visit-item-scheduled';
                lnk += "scheduled";
                ico = "fa-play";
                std.scheduled += 1;
                break;
            case 1:
                tmp += 'visit-item-inprogress';
                lnk += "inprogress";
                ico = "fa-stop";
                std.inprogress += 1;
                break;
            case 2:
                tmp += 'visit-item-finished';
                lnk += "finished";
                ico = "fa-info"
                std.finished += 1;
                break;
            case 3, 4:
                tmp += 'visit-item-notpresented';
                lnk += "notpresented";
                ico = "fa-info";
                std.notpresented += 1;
                break;
        }
        tmp += '">';

        var userFieldInfo = "";

        for (var iField = 0; iField < vst.fields.length; iField++) {
            if (vst.fields[iField].name == _currOptions.cardfield) {
                userFieldInfo = vst.fields[iField].value;
                break;
            }
        }

        tmp += '   <div class="large-1 medium-1 column text-center"><span class="visit-item-time"><span class="visit-label">' + $.t("begin", "Inicio") + '<br/></span> ' + moment(vst.begindate).format('HH:mm') + '</span></div>';
        if (userFieldInfo == "") {
            tmp += '   <div class="large-3 medium-3 column text-center"><span><strong>' + $.t("subject", "Asunto") + ": </strong>" + vst.name + '</span></div>';
        } else {
            tmp += '   <div class="large-3 medium-3 column text-center"><span><strong>' + $.t("subject", "Asunto") + ": </strong>" + vst.name + '</span><br /><span><strong>' + _currOptions.cardfield + "</strong>:" + userFieldInfo + '</span></div>';
        }

        tmp += '   <div class="large-5 medium-5 column">';
        tmp += '<div class="row">';
        var vstrname = ""
        $(vst.visitors).each(function (index) {
            if (vstrname.length > 0) { vstrname += ", "; }
            vstrname += vst.visitors[index].name;
        });
        tmp += '<span class="visit-label">' + $.t("visitors", "Visitantes") + ': </span>' + vstrname;
        tmp += '</div>';
        tmp += '<div class="row">';
        tmp += '<span class="visit-label">' + $.t("visitto", "Visita a") + ': </span>' + vst.employee + "<span class='employeestatus-" + vst.idemployee + "'></span>";
        tmp += '</div>';
        tmp += '</div>';
        tmp += '<div class="large-1 medium-1 column">'
        if (ico != "") {
            tmp += '<a href="' + lnk + '/visit/' + vst.idvisit + '"><i class="fa ' + ico + ' fa-2x"></i></a>';
        }
        tmp += '</div>';
        tmp += '</div>';
        switch (vst.status) {
            case 0: $("#subtab-scheduled").append(tmp); break;
            case 1: $("#subtab-inprogress").append(tmp); break;
            case 2: $("#subtab-finished").append(tmp); break;
            case 3: $("#subtab-notpresented").append(tmp); break;
        }

        /*
        $("#plani-status-scheduled").html($.t("visitsscheduledetail",{ total: std.scheduled}, "Hay __total__ visitas planificadas."));
        $("#plani-status-inprogress").html($.t("visitsinprogressdetail",{ total: std.inprogress}, "Hay __total__  visitas activas."));
        $("#plani-status-finished").html($.t("visitsfinisheddetail",{ total: std.finished}, "Hay __total__  visitas finalizadas."));
        $("#plani-status-notpresented").html($.t("visitsnotpresenteddetail",{ total: std.notpresented}, "Hay __total__  visitas no presentadas."));
        */
        $("#newVisits").slideUp();
    });
    if ($("#subtab-scheduled").children().size() == 0) { $("#subtab-scheduled").append($.t("norecordset", "No hay datos en esta vista")); }
    if ($("#subtab-inprogress").children().size() == 0) { $("#subtab-inprogress").append($.t("norecordset", "No hay datos en esta vista")); }
    if ($("#subtab-finished").children().size() == 0) { $("#subtab-finished").append($.t("norecordset", "No hay datos en esta vista")); }
    if ($("#subtab-notpresented").children().size() == 0) { $("#subtab-notpresented").append($.t("norecordset", "No hay datos en esta vista")); }

    if (bIsLoading) {
        $("#tab-plani-isloading").html(showLoading());
    } else {
        $("#tab-plani-isloading").html("");
        _lastVisitChange = moment();
    }
}
function getVisitListSearch(searchparams, reloadList) {
    _currVisitSearchList = {};
    _currVisitSearchValues = searchparams;
    var params = 'timestamp=' + new Date().getTime();
    if (searchparams != "") { params += '&searchparams=' + encodeURIComponent(JSON.stringify(searchparams)); }

    parseVisitListSearch({ visits: [] }, true);

    if (JSON.stringify(searchparams) != _visitorsSearchFilter || _visitorsSearchCache == null || reloadList) {
        _visitorsSearchFilter = JSON.stringify(searchparams);

        var jqxhr = callRoboticsWS_GET(jsonEngineURI + "visitlist", params, function (data) {
            if (!hasError(data)) {
                _visitorsSearchCache = data.d;
                //ordenamos por fecha
                //_currVisitSearchList.visits.sort(function (a, b) { return moment(a.begindate).isAfter(b.begindate); });
                parseVisitListSearch(_visitorsSearchCache, false);
            } else {
                $("#visitssearch-response").html(showErrorClose({ text: data.result }));
            }
        }, function () {
            $("#visitssearch-response").html(showErrorClose({ text: $.t("unknownerror", "Error, aviste a soporte.") }));
        });
    } else {
        parseVisitListSearch(_visitorsSearchCache, false);
    }
}
function parseVisitListSearch(data, bIsLoading) {
    $("#subtab-results-today").html("");
    $("#subtab-results-scheduled").html("");
    $("#subtab-results-finished").html("");

    if (_currVisitSearchValues.value.length > 0) {
        $("#plani-search-text").val(_currVisitSearchValues.value);
        $("#tab-plani-filter > div a[data-subtab='search']").attr("href", "#/search/" + _currVisitSearchValues.where + "/" + _currVisitSearchValues.value);
    } else {
        $("#plani-search-text").val("");
    }
    if (_currVisitSearchValues.where.length > 0) {
        $("#plani-search-selector").val(_currVisitSearchValues.where);
    } else {
        $("#plani-search-selector").val("");
    }

    if (_currVisitSearchValues.orderBy.length > 0) {
        $("#plani-orderBy-selector").val(_currVisitSearchValues.orderBy);
    } else {
        $("#plani-orderBy-selector").val("hour");
    }

    var nowt = moment();
    $(data.visits).each(function (index) {
        var vst = this;
        var lnk = "#/search/";
        var ico = "";
        var tmp = '<div class="row visit-item-search '
        var dt = moment(vst.begindate);
        var fi = moment(vst.enddate);
        if (dt.isSame(nowt, 'day') || moment().isBetween(dt, fi)) {
            switch (vst.status) {
                case 0:
                    ico = "fa-play";
                    break;
                case 1:
                    ico = "fa-stop";
                    break;
                default:
                    ico = "fa-info"
            }
        } else {
            ico = "fa-info"
        }

        tmp += '">';

        var userFieldInfo = "";

        for (var iField = 0; iField < vst.fields.length; iField++) {
            if (vst.fields[iField].name == _currOptions.cardfield) {
                userFieldInfo = vst.fields[iField].value;
                break;
            }
        }

        tmp += '   <div class="large-1 medium-1 column text-center"><span class="visit-item-time"><span class="visit-label">' + $.t("begin", "Inicio") + '<br/></span> ' + moment(vst.begindate).format('HH:mm') + '</span></div>';

        if (userFieldInfo == "") {
            tmp += '   <div class="large-3 medium-3 column text-center"><span><strong>' + $.t("subject", "Asunto") + ": </strong>" + vst.name + '</span></div>';
        } else {
            tmp += '   <div class="large-3 medium-3 column text-center"><span><strong>' + $.t("subject", "Asunto") + ": </strong>" + vst.name + '</span><br /><span><strong>' + _currOptions.cardfield + "</strong>:" + userFieldInfo + '</span></div>';
        }

        tmp += '   <div class="large-5 medium-5 column">';
        tmp += '<div class="row">';
        var vstrname = ""
        $(vst.visitors).each(function (index) {
            if (vstrname.length > 0) { vstrname += ", "; }
            vstrname += vst.visitors[index].name;
        });
        tmp += '<span class="visit-label">' + $.t("visitors", "Visitantes") + ': </span>' + vstrname;
        tmp += '</div>';
        tmp += '<div class="row">';
        tmp += '<span class="visit-label">' + $.t("visitto", "Visita a") + ': </span>' + vst.employee;
        tmp += '</div>';
        tmp += '</div>';
        tmp += '<div class="large-1 medium-1 column">'
        if (ico != "") {
            tmp += '<a href="#search/visit/' + vst.idvisit + '"><i class="fa ' + ico + ' fa-2x"></i></a>';
        }
        tmp += '</div>';
        tmp += '</div>';

        if (dt.isSame(nowt, 'day') || moment().isBetween(dt, fi)) {
            $("#subtab-results-today").append(tmp);
        } else if (dt.isAfter(nowt, 'day')) {
            $("#subtab-results-scheduled").append(tmp);
        } else {
            $("#subtab-results-finished").append(tmp);
        }
    });

    if (bIsLoading) {
        $("#subtab-results-loading").html(showLoading());
        $("#subtab-results-today").hide();
        $("#subtab-results-scheduled").hide();
        $("#subtab-results-finished").hide();
    } else {
        $("#subtab-results-loading").html("");
        $("#subtab-results-today").show();
        $("#subtab-results-scheduled").show();
        $("#subtab-results-finished").show();

        if ($("#subtab-results-today").children().size() == 0) { $("#subtab-results-today").append($.t("norecordset", "No hay datos en esta vista")); }
        if ($("#subtab-results-scheduled").children().size() == 0) { $("#subtab-results-scheduled").append($.t("norecordset", "No hay datos en esta vista")); }
        if ($("#subtab-results-finished").children().size() == 0) { $("#subtab-results-finished").append($.t("norecordset", "No hay datos en esta vista")); }
    }
}

//function cleanfilterVisitGest() {
//    _currVisitFilter = {};

//    //$("#gest-visits-filters-begin").val("");
//    //$("#gest-visits-filters-end").val("");

//    filterVisitGest();
//}

function filterVisitGest() {
    var m = moment($("#gest-visits-filters-begin").val(), "DD/MM/YYYY");
    if (m.isValid()) {
        _currVisitFilter["begindate"] = $("#gest-visits-filters-begin").val();
    } else {
        _currVisitFilter["begindate"] = "";
    }
    var m = moment($("#gest-visits-filters-end").val(), "DD/MM/YYYY");
    if (m.isValid()) {
        _currVisitFilter["enddate"] = $("#gest-visits-filters-end").val();
    } else {
        _currVisitFilter["enddate"] = "";
    }

    localStorage.setItem("currVisitFilter", JSON.stringify(_currVisitFilter));
    getVisitGest(_currVisitFilter);
}

function getVisitGest(filter) {
    $("#gest-visits").html("");
    // $(".gest-top").addClass("hide");
    $("#gest-visits-response").html(showLoading());
    var params = 'timestamp=' + new Date().getTime();
    if (_.isObject(filter) && filter != "") filter._loadpunches_ = true;
    params += '&filter=' + encodeURIComponent(JSON.stringify(filter));

    var jqxhr = callRoboticsWS_GET(jsonEngineURI + "visitlist", params, function (data) {
        if (!hasError(data)) {
            data.d.visits.sort(function (a, b) { return moment(a.begindate).isBefore(b.begindate); });
            parseVisitsGest();
            configureDevExtremeGrid(data.d);
        } else {
            $("#gest-visits-response").html(showErrorClose({ text: $.t("errorLoadingVisitsList", "Error desconocido cargando la lista de visitas") }));
        }
    }, function () {
        $("#gest-visits-response").html(showErrorClose({ text: "Error al cargar la lista de visitas. Por favor, acote más el filtro." }));
    });
}

function parseVisitsGest() {
    //Mostramos los filtros si existen
    if (_.isString(_currVisitFilter.lastIn)) {
        var m = moment(_currVisitFilter.lastIn, "DD/MM/YYYY");
        if (m.isValid()) {
            $("#gest-visits-filters-lastIn").val(m.format("DD/MM/YYYY"));
        } else {
            $("#gest-visits-filters-lastIn").val("");
        }
    } else {
        $("#gest-visits-filters-lastIn").val("");
    }
    if (_.isString(_currVisitFilter.lastOut)) {
        var m = moment(_currVisitFilter.lastOut, "DD/MM/YYYY");
        if (m.isValid()) {
            $("#gest-visits-filters-lastOut").val(m.format("DD/MM/YYYY"));
        } else {
            $("#gest-visits-filters-lastOut").val("");
        }
    } else {
        $("#gest-visits-filters-lastOut").val("");
    }

    $("#gest-visits-response").html("");
    $(".gest-top").removeClass("hide");
    $("#tab-gest .datepicker").datetimepicker("hide");
}

function configureDevExtremeGrid(data) {
    //pq el grid no hagi de fer coses estranyes, el q fem es moure el array .Fields[] del datasource al mateix nivell q les altres propietats.
    //en JS usarem aquesta funcionalitat:
    //data["PropertyD"]= 4;
    //alert(data.PropertyD); <-works!

    data.visits.forEach(function (visit) {
        visit.fields.forEach(function (field) {
            var nomDelCamp = field.idfield;
            visit[nomDelCamp.toString()] = field.value;
        });

        visit.visitors.forEach(function (visitor) {
            visitor.fields.forEach(function (field, i) {
                var concat = "";
                visit.visitors.forEach(function (visitorTraversal) {
                    concat += visitorTraversal.fields[i].value + ",";
                });
                var nomDelCamp = field.idfield;
                if (concat.length > 0) concat = concat.slice(0, -1);
                visit[nomDelCamp.toString()] = concat;
            })
        })
    });

    $("#gest-visits-devextreme").dxDataGrid({
        dataSource: data.visits,
        stateStoring: {
            enabled: true,
            type: 'localStorage',
            customLoad: function () {
                var d = new $.Deferred();
                setTimeout(function () {
                    var state = localStorage.getItem("dxFilters");
                    d.resolve($.parseJSON(state));
                }, 1000);
                return d.promise();
            },
            customSave: function (gridState) {
                localStorage.setItem('dxFilters', JSON.stringify(gridState));
            }
            //WARNING: si baixem el savingTimeout deixa de funcionar. deixar-ho amb el default (2000). savingTimeout: 500,
        },
        sorting: {
            mode: "multiple"
        },
        paging: {
            pageSize: 10
        },
        pager: {
            allowedPageSizes: [10, 20, 50, 100],
            showPageSizeSelector: true
        },
        columnChooser: {
            enabled: true
        },
        "export": {
            enabled: true,
            fileName: "Visits",
            allowExportSelectedData: false
        },
        groupPanel: {
            visible: true
        },

        filterRow: {
            visible: true,
            applyFilter: "auto"
        },
        searchPanel: {
            visible: true,
            width: 300,
            placeholder: $.t("Search", "Buscar") + "..."
        },
        headerFilter: {
            visible: true
        },
        showBorders: true,
        allowColumnReordering: true,
        allowColumnResizing: true,
        columnAutoWidth: false, //el problema de true és q al filtrar fa un resize
        columns: [{
            caption: $.t("state", "Estado"),
            dataField: "status",
            calculateCellValue: function (data) {
                return getStatusEnum(data.status)
            }
        }, {
            caption: $.t("subject", "Asunto"),
            dataField: "name",
        }, {
            caption: $.t("lastIn", "Última Entrada"),
            dataField: "lastInDate",
            dataType: 'string',
            //dataType: 'date',
            //format: "dd/MM/yy HH:mm", //DevExpress uses Globalize convention, not moment.js convention
            calculateCellValue: function (vst) {
                return vstParseMoment(vst.lastInDate);
            },
            cellTemplate: function (container, options) {
                if (options.value != "") {
                    $("<div>")
                        .append(moment(options.value, "YY/MM/DD HH:mm").format("DD/MM/YYYY HH:mm"))
                        .appendTo(container);
                }
                else {
                    $("<div>")
                        .append("")
                        .appendTo(container);
                }
            }
        }, {
            caption: $.t("lastOut", "Última Salida"),
            dataField: "lastOutDate",
            dataType: 'string',
            //format: "shortDateShortTime",
            calculateCellValue: function (vst) {
                return vstParseMoment(vst.lastOutDate);
            },
            cellTemplate: function (container, options) {
                if (options.value != "") {
                    $("<div>")
                        .append(moment(options.value, "YY/MM/DD HH:mm").format("DD/MM/YYYY HH:mm"))
                        .appendTo(container);
                }
                else {
                    $("<div>")
                        .append("")
                        .appendTo(container);
                }
            }
        }, {
            caption: $.t("visitors", "Visitantes"),
            dataField: "visitors",
            calculateCellValue: function (vst) {
                var vstrname = ""
                $(vst.visitors).each(function (index) {
                    if (vstrname.length > 0) { vstrname += ", "; }
                    vstrname += vst.visitors[index].name;
                });
                return vstrname;
            }
        }, {
            caption: $.t("employee", "Empleado"),
            dataField: "employee",
        }, {
            caption: $.t("createdby", "Creado por"),
            dataField: "createdbyname",
        }, {
            caption: $.t("enters", "Entradas"),
            dataField: "enters",
            calculateCellValue: function (vst) {
                return (vst.repeat == 1 ? $.t('severaltimes', "Varias veces") : $.t('onetime', "Una vez"));
            }
        }, {
            caption: $.t("begindate", "Inicio Planificación"),
            dataField: "begindate",
            dataType: 'string',
            //format: "shortDateShortTime",
            calculateCellValue: function (vst) {
                return vstParseMoment(vst.begindate);
            },
            cellTemplate: function (container, options) {
                if (options.value != "") {
                    $("<div>")
                        .append(moment(options.value, "YY/MM/DD HH:mm").format("DD/MM/YYYY HH:mm"))
                        .appendTo(container);
                }
                else {
                    $("<div>")
                        .append("")
                        .appendTo(container);
                }
            }
        }, {
            caption: $.t("enddate", "Final Planificación"),
            dataField: "enddate",
            dataType: 'string',
            //format: "shortDateShortTime",
            calculateCellValue: function (vst) {
                return vstParseMoment(vst.enddate);
            },
            cellTemplate: function (container, options) {
                if (options.value != "") {
                    $("<div>")
                        .append(moment(options.value, "YY/MM/DD HH:mm").format("DD/MM/YYYY HH:mm"))
                        .appendTo(container);
                }
                else {
                    $("<div>")
                        .append("")
                        .appendTo(container);
                }
            }
        },
        {
            caption: $.t("visittypedesc", "Tipo de Visita"),
            dataField: "visittypedesc",
        }
        ]
        ,
        customizeColumns: function (columns) {
            customizarColumnas(columns, data)
        }
    });

    var local = $("#gest-visits-devextreme").dxDataGrid("instance");
    local.refresh();
    local.repaint();
}

function isColumnStateVisible(gridState, lookingFor) {
    if (gridState == null)
        return true;

    var result = true;
    gridState.columns.forEach(function (column) {
        if (column.dataField == lookingFor) {
            result = column.visible;
        }
    });
    return result;
}

function customizarColumnas(columns, data) {
    //afegeix camps definits lliurement per l'usuari (targeta, portatil..)
    if (data.visits.length > 0) {
        data.visits[0].fields.sort(function (a, b) { return a.position > b.position; }); //associats a la visita

        var gridState = $.parseJSON(localStorage.getItem("dxFilters"));

        if (data.visits[0].fields.length > 0) {
            $(data.visits[0].fields).each(function (index) {
                columns.push({ caption: this.name, dataField: this.idfield, visible: isColumnStateVisible(gridState, this.idfield) });
            });
        }
        //associats al(s) visitant(s)
        if (data.visits[0].visitors.length > 0) {
            $(data.visits[0].visitors[0].fields).each(function (index) {
                columns.push({ caption: this.name, dataField: this.idfield, visible: isColumnStateVisible(gridState, this.idfield) });
            });
        }
    }

    //afegim Commands aquí, després d'haver afegit les dinamiques, pq apareguin al final..
    columns.push({
        caption: "",
        name: "comandes",
        cellTemplate: function (container, options) {
            container.addClass("editCommandsContainer");
            $("<a href='#/gest/visits/edit/" + options.data.idvisit + "'><i class='fa fa-pencil-square-o fa-lg'></i></a>").appendTo(container);
            $("<a href='#/gest/visits/del/" + options.data.idvisit + "'><i class='fa fa-trash-o fa-lg'></i></a>").appendTo(container);
        }
    });
}

function vstParseMoment(date) {
    var m = moment(date);
    if (!m.isValid())
        return "Not valid";

    if (m.year() == 1970)
        return "";
    else
        return m.format("YY/MM/DD HH:mm");
}

function getStatusEnum(status) {
    switch (status) {
        case 0: return $.t('scheduled'); break;
        case 1: return $.t('inprogress'); break;
        case 2: return $.t('finished'); break;
        case 3: return $.t('notpresented'); break;
        case 4: return $.t('autoclose', 'Cierre automatico'); break;
        case 5: return $.t('canceled', 'Cancelada'); break;
            return '';
    }
}
function delVisitGest(idvisit) {
    $("#gest-visits").html("");
    $("#gest-visits-response").html(showLoading());
    var params = 'idvisit=' + encodeURIComponent(idvisit);

    params += '&filter=' + encodeURIComponent(localStorage.getItem("currVisitFilter"));

    if (params != '') params += '&timestamp=' + new Date().getTime();
    else params = 'timestamp=' + new Date().getTime();

    var jqxhr = callRoboticsWS_GET(jsonEngineURI + "delvisit", params, function (data) {
        if (!hasError(data)) {
            data.d.visits.sort(function (a, b) { return moment(a.begindate).isBefore(b.begindate); });
            parseVisitsGest();
            configureDevExtremeGrid(data.d);
            getVisitList(_currVisitFilterList, true);
        } else {
            $("#gest-visits-response").html(showErrorClose({ text: data.result }));
        }
    }, function () {
        $("#gest-visits-response").html(showErrorClose({ text: $.t("unknownerror", "Error, aviste a soporte.") }));
    });
}

function showVisit(idvisit) {
    getVisit(idvisit, null);
    openModal('#mod-visit');
}
function getVisit(idvisit, idtype) {
    $(".mod-visit-row, #mod-visit-buttons").addClass("hide");
    $("#mod-visit-response").html(showLoading());
    var params = 'idvisit=' + idvisit
    params += '&idtype=' + idtype;

    if (params != '') params += '&timestamp=' + new Date().getTime();
    else params = 'timestamp=' + new Date().getTime();

    var formData = new FormData();
    formData.append("idvisit", idvisit);
    formData.append("idtype", idtype);

    var retCallback = function (pData) {
        return function () {
            var jqxhr = callRoboticsWS_POST(jsonEngineURI + "visit", pData, function (data) {
                if (!hasError(data)) {
                    parseVisit(data.d);
                } else {
                    $("#mod-visit-response").html(showError({ text: data.d.result }));
                }
            }, function () {
                $("#mod-visit-response").html(showError({ text: $.t("unknownerror", "Error, aviste a soporte.") }));
            });
        }
    }
    getVisitTypes(params, retCallback(formData));
}

/*
 * Get date with correct hour. Add 1 if it necessary
 * @param {date. Date to inspect}
 * @returns {return correct date}
 */
function getDateFormatted(date) {
    var result = date;
    if (date.format("mm") > 0) {
        result = date.add(1, "hour");
    }
    return date.format("DD/MM/YYYY HH:00");
}

function parseVisit(data) {
    var tmp = "";
    _nextStatus = 0;
    $("#mod-visit .alert-input").remove();

    $("#mod-visit-idvisit").val(data.idvisit);
    $("#mod-visit-type").val(data.idtype);
    //if (data.status != 0) {
    //    $('#mod-visit-type').attr('readonly', true);
    //}
    $("#mod-visit-idparentvisit").val(data.idparentvisit);
    $("#mod-visit-status").val(data.status);
    $("#mod-visit-name").val(data.name);

    if (data.status == 0) {
        tmp += '<div id="mod-visit-novisitors-div"><input id="mod-visit-novisitors" type="checkbox">&nbsp;<label for="mod-visit-novisitors">' + $.t("addtostartvisit", "Indicar al iniciar la visita") + '</label></div>';
    }
    $(data.visitors).each(function (index) {
        tmp += '<div class="mod-visit-visitor-item" data-idvisitor="' + this.idvisitor + '"';
        tmp += ' data-requiredfieldsoncreatevisit="' + this.requiredfieldsoncreatevisit + '" data-requiredfieldsonstartvisit="' + this.requiredfieldsonstartvisit + '">';
        tmp += '<a onClick="showVisitor(\'' + this.idvisitor + '\')">' + this.name + '</a>';
        if (data.status == 0) { tmp += '  <a onclick="delVisitor2Visit(this)"><i class="fa fa-times"></i></a>'; }
        tmp += '</div>'
        var itm = "";
        itm += this.idvisitor + ";"
        if (itm == "") { itm = "-"; }
        _currVisitorListFilter["ids"] = itm;
    });

    if (data.status == 0) { tmp += '<br/><a onclick="showVisitorList()" id="mod-visit-visitors-add"><div class="add"><i class="fa fa-user-plus fa-lg"></i> ' + $.t("addvisitor", "Añadir visitantes") + '</div></a>'; }
    $("#mod-visit-visitors").html(tmp);
    if (data.visitors.length > 0) {
        $("#mod-visit-novisitors-div").addClass("hide");
    } else if ($(".mod-visit-visitor-item").length == 0 && data.status == 0 && data.idvisit != "new" && data.idvisit != "0" && data.idvisit != "") {
        $("#mod-visit-novisitors").prop("checked", true);
        $("#mod-visit-novisitors-div").removeClass("hide");
    } else {
        $("#mod-visit-novisitors-div").removeClass("hide");
    }
    var stdate = moment(data.begindate);
    var enddate = moment(data.enddate);
    if (stdate.year() == 1970) {
        //$("#mod-visit-startdate").val(getDateFormatted(moment()));
        $("#mod-visit-startdate").val(moment().format("DD/MM/YYYY HH:mm"));
    } else {
        //$("#mod-visit-startdate").val(getDateFormatted(stdate));
        $("#mod-visit-startdate").val(stdate.format("DD/MM/YYYY HH:mm"));
    }
    if (enddate.year() == 1970) {
        $("#mod-visit-enddate").val("");
        $("#chkWhen0").prop("checked", true);
    } else {
        $("#mod-visit-enddate").val(getDateFormatted(enddate));
        $("#chkWhen1").prop("checked", true);
    }
    $("#mod-visit-repeat").prop("checked", data.repeat);

    //Borramos datos antiguos
    $("#mod-visit-repeattime-daily-ndays").val('');
    $("#visit-repeattime-weekly-nweeks").val('');
    for (i = 0; i < 7; i++) { $("#mod-visit-repeattime-daily-days" + i).prop("checked", false); }
    $("#visit-repeattime-monthly-day").val(1);
    $("#visit-repeattime-monthly-nmonths").val('');
    $("#visit-repeattime-yearly-day").val(1);
    $("#visit-repeattime-yearly-month").val(1);

    if (data.cloneevery.length > 0) {
        $("#visit-repeattype").val(data.cloneevery.split(";")[0]);

        switch (data.cloneevery.split(";")[0]) {
            case "daily":
                if ((data.cloneevery.split(";")[1]).split(",")[0] == 'a') {
                    $("[name='mod-visit-repeattime-dailyinput'][value='a']").prop("checked", true);
                } else {
                    $("[name='mod-visit-repeattime-dailyinput'][value='n']").prop("checked", true);
                    $("#mod-visit-repeattime-daily-ndays").val(data.cloneevery.split(";")[2]);
                }
                break;
            case "weekly":
                $("#visit-repeattime-weekly-nweeks").val((data.cloneevery.split(";")[1]).split(",")[0]);
                _.each((data.cloneevery.split(";")[2]).split(","), function (element, index, list) {
                    $("#mod-visit-repeattime-daily-days" + element).prop("checked", true);
                });
                break;
            case "monthly":
                $("#visit-repeattime-monthly-day").val(data.cloneevery.split(";")[1]);
                $("#visit-repeattime-monthly-nmonths").val(data.cloneevery.split(";")[2]);
                break;
            case "yearly":
                $("#visit-repeattime-yearly-day").val(data.cloneevery.split(";")[1]);
                $("#visit-repeattime-yearly-month").val(data.cloneevery.split(";")[2]);
                break;
        }
    } else {
        $("#visit-repeattype").val("--");
    }
    $("#visit-repeattype").trigger("change");

    var fl = "";
    $("#mod-visit-fields").html("");
    //data.fields.sort(function (a, b) {
    //    return a.position > b.position;
    //});
    $(data.fields).each(function (index) {
        this.name = this.name.trim();
        fl += '<div class="row mod-visit-row mod-visit-field">';
        fl += '    <div class="large-3 column large-text-right medium-text-left small-text-left">';
        fl += '        <label class="inline" for="mod-visit-field-' + this.name + '">' + this.name + '</label>';
        fl += '    </div>';
        if (this.values == '') {
            fl += '    <div class="large-9 column large-text-left medium-text-left small-text-left">';
            fl += '        <textarea data-idfield="' + this.idfield + '" data-name="' + this.name + '" data-required="' + this.required + '" data-askevery="' + this.askevery + '"';
            fl += '           name="mod-visit-field-' + this.name + '" id="mod-visit-field-' + this.name + '" ';
            if ((!this.edit) && (this.required != 3)) {
                fl += '       readonly="true"';
            }
            if (this.required == 99) {
                fl += '       readonly="true"  disabled="true"';
            }
            //fl += '           maxlength = "100"';
            fl += '           >' + this.value + '</textarea>';
            fl += '    </div>';
        } else {
            fl += '    <div class="large-9 column large-text-left medium-text-left small-text-left">';
            fl += '                <select data-idfield="' + this.idfield + '" data-name="' + this.name + '" data-required="' + this.required + '" data-askevery="' + this.askevery + '"';
            fl += '                   name="mod-visit-field-' + this.name + '" id="mod-visit-field-' + this.name + '" value="' + this.value + '"';
            if ((!this.edit) && (this.required != 3)) {
                fl += '       disabled="true"';
            }
            fl += '>';
            var value = this.value;
            if (value == "") { fl += '        <option value="">' + $.t("selectvalue", "-- Seleccione un valor --") + '</option>'; }
            $(this.values.split(";")).each(function (index, val) {
                fl += '        <option value="' + val + '"';
                if (value == val) { fl += ' selected '; }
                fl += '>' + val + '</option>';
            });
            fl += '                </select>';
            fl += '    </div>';
        }
        fl += '</div>';
    });
    $("#mod-visit-fields").html(fl);
    if (data.idemployee > 0) {
        getEmployeeStatus(data.idemployee);
        var tmp = '<div class="mod-visit-employee-item" data-idemployee="' + data.idemployee + '"><span class="employeestatus-' + data.idemployee + '"></span>' + data.employee + '</a>'
        if (data.status == 0) { tmp += '  <a onclick="delEmployee2Visit(this)"><i class="fa fa-times"></i></a>'; }
        tmp += '</div>'
        $("#mod-visit-employee").html(tmp);
    } else {
        if (data.status == 0) { $("#mod-visit-employee").html('<a onclick="showEmployeeList()"><div class="add"><i class="fa fa-user-plus fa-lg"></i> ' + $.t("addemployee", "Añadir empleado") + '</div></a>'); }
    }
    $("#mod-visit-createdbyname").html(data.createdbyname);
    if (data.punches.length > 0) {
        var punc = "";
        $("#visit-row-punches").removeClass("hide_");
        data.punches.sort(function (a, b) {
            return moment(a.punchdate).isAfter(b.punchdate);
        });
        punc = '<table width="100%">';
        punc += "<thead>";
        punc += "<th>" + $.t("visitor", "Visitante") + "</th>";
        punc += "<th>" + $.t("datetime", "Día/Hora") + "</th>";
        punc += "<th>" + $.t("action", "Acción") + "</th>";
        punc += "</thead>";
        punc += "<tbody>";
        $(data.punches).each(function (index) {
            punc += "<tr>";
            punc += "<td>" + this.visitorname + "</td>";
            punc += "<td>" + moment(this.punchdate).format("DD/MM/YYYY HH:mm") + "</td>";
            punc += "<td>" + (this.action == "IN" ? $.t("in", "Entrada") : $.t("out", "Salida")) + "</td>";
            punc += "</tr>";
        });
        punc += "</tbody>";
        punc += "</table>";
        $("#mod-visit-punches").html(punc);
    } else {
        $("#visit-row-punches").addClass("hide_");
        $("#mod-visit-punches").html("");
    }

    //Control de botones
    $("#mod-visit-buttons").html("");
    if (data.idvisit != "new") {
        switch (data.status) {
            case 0:
                if (_usr.haschange > 2 || (_usr.haschange == 2 && _usr.idemployee == data.idemployee)) {
                    if (data.visitors.length > 0) {
                        $("#mod-visit-buttons").append('<a class="button tiny" data-i18n="print" id="mod-visit-buttons-print" onclick="printVisit()">' + $.t("print", "Imprimir") + '</a>&nbsp;');
                    }

                    if (stdate.format("DD/MM/YYYY") == moment().format("DD/MM/YYYY") || moment().isBetween(stdate, enddate)) {
                        $("#mod-visit-buttons").append('<a class="button tiny" data-i18n="start" id="mod-visit-buttons-start" onclick="saveVisit(1)">' + $.t("start", "Iniciar") + '</a>&nbsp;');
                        if (data.punches.length == 0) {
                            $("#mod-visit-buttons").append('<a class="button tiny" data-i18n="notpresented" id="mod-visit-buttons-notpresented" onclick="saveVisit(3)">' + $.t("notpresented", "No presentado") + '</a>&nbsp;');
                        }
                    }
                    if (moment().isAfter(stdate, "day")) {
                        if (data.punches.length == 0) {
                            $("#mod-visit-buttons").append('<a class="button tiny" data-i18n="notpresented" id="mod-visit-buttons-notpresented" onclick="saveVisit(3)">' + $.t("notpresented", "No presentado") + '</a>&nbsp;');
                        } else {
                            $("#mod-visit-buttons").append('<a class="button  tiny" data-i18n="end" id="mod-visit-buttons-end" onclick="saveVisit(2)">' + $.t("ending", "Finalizar") + '</a>&nbsp;');
                        }
                    }
                    if (moment().isBefore(stdate, "day")) {
                        $("#mod-visit-buttons").append('<a class="button tiny" data-i18n="canceled" id="mod-visit-buttons-canceled" onclick="saveVisit(5)">' + $.t("cancel", "Cancelar") + '</a>&nbsp;');
                    }
                }
                $("#mod-visit-buttons").append('<a class="button  tiny" data-i18n="save" id="mod-visit-buttons-save" onclick="saveVisit()">' + $.t("save", "Guardar") + '</a>&nbsp;');
                break;
            case 1:
                //if (any2Boolean(_currOptions.allowvisitfieldmodify))
                $("#mod-visit-buttons").append('<a class="button tiny" data-i18n="print" id="mod-visit-buttons-print" onclick="printVisit()">' + $.t("print", "Imprimir") + '</a>&nbsp;');
                $("#mod-visit-buttons").append('<a class="button  tiny" data-i18n="save" id="mod-visit-buttons-save" onclick="saveVisit(1)">' + $.t("save", "Guardar") + '</a>&nbsp;');
                $("#mod-visit-buttons").append('<a class="button  tiny" data-i18n="end" id="mod-visit-buttons-end" onclick="saveVisit(2)">' + $.t("ending", "Finalizar") + '</a>&nbsp;');
                break;
        }
    } else {
        $("#mod-visit-buttons").append('<a class="button  tiny " data-i18n="save" id="mod-visit-buttons-save" onclick="saveVisit()">' + $.t("save", "Guardar") + '</a>&nbsp;');
        $("#mod-visit-buttons").append('<a class="button  tiny " data-i18n="save" id="mod-visit-buttons-saveAndStart" onclick="saveVisit(1)">' + $.t("saveAndStart", "Guardar e Iniciar") + '</a>&nbsp;');
    }
    //    $("#mod-visit-buttons").append('<a class="button tiny" data-event  id="mod-visit-buttons-close" data-i18n="close" onclick="$(\'a.close-reveal-modal\').trigger(\'click\');">' + $.t("close", "Cerrar") + '</a>');
    $("#mod-visit-buttons").append('<a class="button tiny" data-event  id="mod-visit-buttons-close" onclick="closeModal(\'#mod-visit\')" data-i18n="close">' + $.t("close", "Cerrar") + '</a>');

    $(".mod-visit-row, #mod-visit-buttons").removeClass("hide");
    if (enddate.year() == 1970) {
        $(".visit-period").addClass("hide");
    }

    if (data.status > 0) {
        //no dejar modificar
        $(".mod-visit-row input").each(function (index) {
            if (this.id.indexOf("mod-visit-field") == -1) $(this).prop("readonly", true);
        });
        $("#visit-repeattype").prop("disabled", true);
        $("#mod-visit-type").prop("disabled", true);
        $(".mod-visit-row input:radio").prop("disabled", true);
        $(".mod-visit-row .datetimepicker").datetimepicker("destroy");
        //$("#mod-visit-buttons-save").addClass("hide");

        //si ... permitir modificar los campos de la ficha
        $(".mod-visit-row input").each(function (index) {
            if (this.id.indexOf("mod-visit-field") > -1 && $(this).prop("readonly") == false) $("#mod-visit-buttons-save").removeClass("hide");
        });
    } else {
        $(".mod-visit-row input").prop("readonly", false);
        $("#visit-repeattype").prop("disabled", false);
        $("#mod-visit-type").prop("disabled", false);
        $(".mod-visit-row input:radio").prop("disabled", false);
        $(".mod-visit-row .datetimepicker").datetimepicker({ lang: _usr.lang, mask: true, format: 'd/m/Y H:i', dayOfWeekStart: 1, closeOnTimeSelect: true });
        $("#mod-visit-buttons-save").removeClass("hide");
    }

    $("#mod-visit-buttons-close").html($.i18n.t("close", "Cerrar"));

    if (data.idvisit == "new") {
        $("#mod-visit-type").prop("disabled", false);
        $("#mod-visit-createdby").addClass("hide");
        //$(".mod-visit-row input[data-required=2] ").parent().parent().addClass("hide");
    } else {
        $("#mod-visit-createdby").removeClass("hide");
        $(".mod-visit-row input[data-required=2] ").parent().parent().removeClass("hide");
    }

    $("#mod-visit-response").html("");
}
function getVisitVisitors(filter) {
    $("#mod-visit-visitors").html(showLoading());
    var params = 'timestamp=' + new Date().getTime();
    if (filter != "") { params += '&filter=' + encodeURIComponent(JSON.stringify(filter)); }

    var jqxhr = callRoboticsWS_GET(jsonEngineURI + "visitorlist", params, function (data) {
        if (!hasError(data)) {
            parseVisitVisitors(data.d);
        } else {
            $("#mod-visit-visitors").html(showError({ text: data.d.result }));
        }
    }, function () {
        $("#mod-visit-visitors").html(showError({ text: $.t("unknownerror", "Error, aviste a soporte.") }));
    });
}
function parseVisitVisitors(data) {
    var tmp = "";
    if ($("#mod-visit-status").val() == 0) {
        tmp += '<div id="mod-visit-novisitors-div"><input id="mod-visit-novisitors" type="checkbox">&nbsp;<label for="mod-visit-novisitors">' + $.t("addtostartvisit", "Indicar al iniciar la visita") + '</label></div>';
    }
    $(data.visitors).each(function (index) {
        tmp += '&nbsp;<div class="mod-visit-visitor-item " data-idvisitor="' + this.idvisitor + '"';
        tmp += ' data-requiredfieldsoncreatevisit="' + this.requiredfieldsoncreatevisit + '" data-requiredfieldsonstartvisit="' + this.requiredfieldsonstartvisit + '">';
        tmp += '&nbsp;<a onClick="showVisitor(\'' + this.idvisitor + '\')">' + this.name + '</a>';
        if ($("#mod-visit-status").val() == 0) { tmp += '  <a onclick="delVisitor2Visit(this)"><i class="fa fa-times"></i></a>'; }
        tmp += '</div>'
    });

    if ($("#mod-visit-status").val() == 0) { tmp += '<br/><a onclick="showVisitorList()" id="mod-visit-visitors-add"><div class="add"><i class="fa fa-user-plus fa-lg"></i> ' + $.t("addvisitor", "Añadir visitantes") + '</div></a>'; }
    $("#mod-visit-visitors").html(tmp);
    if (data.visitors.length > 0) {
        $("#mod-visit-novisitors-div").addClass("hide");
    } else if ($(".mod-visit-visitor-item").length == 0 && $("#mod-visit-status").val() == 0 && $("#mod-visit-idvisit").val() != "new") {
        $("#mod-visit-novisitors").prop("checked", true);
        $("#mod-visit-novisitors-div").removeClass("hide");
    } else {
        $("#mod-visit-novisitors-div").removeClass("hide");
    }
}
function addVisitor2Visit() {
    var itm = "";
    $("input:checked", _tblVisitorsList.rows().nodes()).each(function (index) {
        itm += $(this).data("idvisitor") + ";"
    });
    // $(".mod-visit-visitor-item").each(function (index) {
    //     itm += $(this).data("idvisitor") + ";"
    // });
    closeModal('#mod-visitorlist');
    if (itm == "") { itm = "-"; }
    _currVisitorListFilter["ids"] = itm;
    getVisitVisitors(_currVisitorListFilter);
}
function delVisitor2Visit(elem) {
    $(elem).parent().remove();
    if ($(".mod-visit-visitor-item").length > 0 && $("#mod-visit-status").val() == 0) {
        $("#mod-visit-novisitors-div").addClass("hide");
    } else {
        $("#mod-visit-novisitors-div").removeClass("hide");
    }
}
function checkVisit(status) {
    if (_.isUndefined(status)) { status = 0; }
    _nextStatus = status;

    if (status > 2) {
        return true;
    }

    $("#mod-visit .alert-input").remove();

    if ($("#mod-visit-name").val() == "") {
        $("#mod-visit-name").after('<div class="alert-input">' + $.t("alertrequired", "Este campo es obligatorio.") + '</div>');
    }

    if ($(".mod-visit-visitor-item").length == 0 && $("#mod-visit-status").val() == 0 && !$("#mod-visit-novisitors").prop("checked")) {
        $("#mod-visit-visitors").append('<div class="alert-input">' + $.t("alertvisitvisitors", "Marque la casilla o añada minimo un visitante") + '</div>');
    }
    if (status == 1 && $(".mod-visit-visitor-item").length == 0) {
        $("#mod-visit-visitors").append('<div class="alert-input">' + $.t("alertvisitvisitorsstart", "Añada minimo un visitante") + '</div>');
    }
    $(".mod-visit-visitor-item").each(function (index) {
        if (status == 0 && $(this).data("requiredfieldsoncreatevisit")) {
            $(this).addClass("visitor-item-red");
        }
        if (status == 1 && $(this).data("requiredfieldsonstartvisit")) {
            $(this).addClass("visitor-item-red");
        }
    });
    if ($(".visitor-item-red").length > 0) {
        $("#mod-visit-visitors").append('<div class="alert-input">' + $.t("alertrequiredvisitor", "Algun visitante tiene campos obligatorios.") + '</div>');
    }

    var m = moment($("#mod-visit-startdate").val(), "DD/MM/YYYY HH:mm");
    if (!m.isValid()) {
        $("#mod-visit-startdate").after('<div class="alert-input">' + $.t("alertinvaliddate", "Formato de fecha invalido.") + '</div>');
    } else {
        if (status < 1 && m.add(10, 'minutes').isBefore()) {
            $("#mod-visit-startdate").after('<div class="alert-input">' + $.t("alertfuturedate", "La fecha ha de ser futura.") + '</div>');
        }
    }
    if ($("#chkWhen1").prop("checked")) {
        var m2 = moment($("#mod-visit-enddate").val(), "DD/MM/YYYY HH:mm");
        if (!m2.isValid()) {
            $("#mod-visit-enddate").after('<div class="alert-input">' + $.t("alertinvaliddate", "Formato de fecha invalido.") + '</div>');
        } else {
            if (status < 2 && (m2.isBefore() || m2.isBefore(m))) {
                $("#mod-visit-enddate").after('<div class="alert-input">' + $.t("alertfuturedate2", "La fecha ha de ser futura y posterior a la entrada.") + '</div>');
            }
        }
    }
    if (_.isUndefined($(".mod-visit-employee-item").data("idemployee"))) {
        $("#mod-visit-employee").after('<div class="alert-input">' + $.t("alertemployee", "Se ha de seleccionar un empleado.") + '</div>');
    }
    $("#mod-visit-fields input, #mod-visit-fields select").each(function (index) {
        if ($(this).data("required") == 1) {
            if ($(this).val() == "") {
                $(this).after('<div class="alert-input">' + $.t("alertrequired", "Este campo es obligatorio.") + '</div>');
            }
        } else if ($(this).data("required") == 2) { //al iniciar la visita
            if (status == 1) {
                if ($(this).val() == "") {
                    $(this).after('<div class="alert-input">' + $.t("alertrequiredonstart", "Este campo es obligatorio al iniciar la visita.") + '</div>');
                }
            }
        }
        else if ($(this).data("required") == 3) { //al finalizar la visita
            if (status == 2) {
                if ($(this).val() == "") {
                    $(this).after('<div class="alert-input">' + $.t("alertrequiredonend", "Este campo es obligatorio al finalizar la visita.") + '</div>');
                }
            }
        }
    });

    switch ($("#visit-repeattype").val()) {
        case "daily":
            if (!_.isString($("[name='mod-visit-repeattime-dailyinput']:checked").val())) {
                $("#mod-visit-repeattime-daily-ndays").after('<div class="alert-input">' + $.t("alertselect", "Ha seleccionar una opción.") + '</div>');
            }
            if ($("[name='mod-visit-repeattime-dailyinput']:checked").val() == "n") {
                if ($("#mod-visit-repeattime-daily-ndays").val().length == 0 || isNaN($("#mod-visit-repeattime-daily-ndays").val())) {
                    $("#mod-visit-repeattime-daily-ndays").after('<div class="alert-input">' + $.t("alertnumeric", "Ha de contener un numero valido.") + '</div>');
                }
            }
            break;
        case "weekly":
            if ($("#visit-repeattime-weekly-nweeks").val().length == 0 || isNaN($("#mod-visit-repeattime-daily-ndays").val())) {
                $("#visit-repeattime-weekly-nweeks").after('<div class="alert-input">' + $.t("alertnumeric", "Ha de contener un numero valido.") + '</div>');
            }
            if ($("[id^='mod-visit-repeattime-daily-days']:checked").length == 0) {
                $("#mod-visit-repeattime-daily-days0").after('<div class="alert-input">' + $.t("alertcheckday", "Ha de seleccionar minimo un día.") + '</div>');
            }
            break;
        case "monthly":
            if ($("#visit-repeattime-monthly-nmonths").val().length == 0 || isNaN($("#visit-repeattime-monthly-nmonths").val())) {
                $("#visit-repeattime-monthly-nmonths").after('<div class="alert-input">' + $.t("alertnumeric", "Ha de contener un numero valido.") + '</div>');
            }
            break;
        case "yearly":
            vst.cloneevery += ";" + $("#visit-repeattime-yearly-day").val();
            vst.cloneevery += ";" + $("#visit-repeattime-yearly-month").val();
            break;
    }

    return $("#mod-visit .alert-input").length == 0 && $(".visitor-item-red").length == 0;
}

function printVisit() {
    var visit = {};
    visit.idVisit = $("#mod-visit-idvisit").val();

    var params = 'values=' + encodeURIComponent(JSON.stringify(visit));
    if (params != '') params += '&timestamp=' + new Date().getTime();
    else params = 'timestamp=' + new Date().getTime();

    var jqxhr = callRoboticsWS_GET(jsonEngineURI + "printfields", params, function (data) {
        parsePrintFields(data.d);
    }, function () {
    });
}

function parsePrintFields(data) {
    document.getElementById("opt-printconfig").innerHTML = '<div>' + data.value + '</div>';
    Imprimir("labelsinput");
}

function saveVisit(status) {
    var vst = { idvisit: 0, name: '', visitors: [], begindate: Date.now, enddate: Date.now, repeat: false, cloneevery: '', idemployee: 0, fields: [], createdby: 0, status: 0, visittype: 0 };
    var vstr = { idvisitor: 0 };
    var fld = { idvisit: 0, idfield: 0, value: "", required: false };
    vst.name = $("#mod-visit-name").val();
    vst.idvisit = $("#mod-visit-idvisit").val();
    vst.idtype = $("#mod-visit-type").val();
    vst.idparentvisit = $("#mod-visit-idparentvisit").val();
    vst.status = $("#mod-visit-status").val();
    $("#mod-visit-visitors .mod-visit-visitor-item").each(function (index) {
        var vstrv = _.clone(vstr);
        vstrv.idvisitor = $(this).data("idvisitor");
        vst.visitors.push(vstrv);
    });
    vst.begindate = moment($("#mod-visit-startdate").val(), "DD/MM/YYYY HH:mm").toISOString();
    var m = moment($("#mod-visit-enddate").val(), "DD/MM/YYYY HH:mm");
    vst.enddate = m.isValid() ? m.toISOString() : moment("1970-01-01").toISOString();

    vst.repeat = $("#mod-visit-repeat").prop("checked");
    vst.cloneevery = $("#visit-repeattype").val();
    switch (vst.cloneevery) {
        case "daily":
            vst.cloneevery += ";" + $("[name='mod-visit-repeattime-dailyinput']:checked").val() + ";";
            if ($("[name='mod-visit-repeattime-dailyinput']:checked").val() == "n") {
                vst.cloneevery += $("#mod-visit-repeattime-daily-ndays").val();
            }
            break;
        case "weekly":
            vst.cloneevery += ";" + $("#visit-repeattime-weekly-nweeks").val() + ";";
            $("[id^='mod-visit-repeattime-daily-days']:checked").each(function (index, value) {
                vst.cloneevery += $(value).val() + ",";
            });
            vst.cloneevery = vst.cloneevery.substring(0, vst.cloneevery.length - 1)
            break;
        case "monthly":
            vst.cloneevery += ";" + $("#visit-repeattime-monthly-day").val();
            vst.cloneevery += ";" + $("#visit-repeattime-monthly-nmonths").val();
            break;
        case "yearly":
            vst.cloneevery += ";" + $("#visit-repeattime-yearly-day").val();
            vst.cloneevery += ";" + $("#visit-repeattime-yearly-month").val();
            break;
    }

    if (_usr.hascreate > 0)
        vst.idemployee = $(".mod-visit-employee-item").data("idemployee");
    else
        if (_usr.hascreateEmployee > 0)
            vst.idemployee = _usr.idemployee;
    vst.createdby = _usr.id;
    $("#mod-visit-fields textarea").each(function (index) {
        fldv = _.clone(fld);
        fldv.idvisit = vstr.idvisit;
        fldv.idfield = $(this).data("idfield");
        fldv.value = $(this).val();
        fldv.required = $(this).data("required");
        vst.fields.push(fldv);
    });
    $("#mod-visit-fields select").each(function (index) {
        fldv = _.clone(fld);
        fldv.idvisit = vstr.idvisit;
        fldv.idfield = $(this).data("idfield");
        fldv.value = $(this).val();
        fldv.required = $(this).data("required");
        vst.fields.push(fldv);
    });
    var itm = "";
    $(".mod-visit-visitor-item").each(function (index) {
        itm += $(this).data("idvisitor") + ";"
    });
    _currVisitorListFilter["ids"] = itm;

    if (checkVisit(status)) {
        vst.status = status;
        putVisit(vst);
    }
}
function putVisit(values) {
    _nextStatus = 0;
    $(".mod-visit-row, #mod-visit-buttons").addClass("hide");
    $("#mod-visit-response").html(showLoading());
    var params = 'values=' + encodeURIComponent(JSON.stringify(values));

    if (params != '') params += '&timestamp=' + new Date().getTime();
    else params = 'timestamp=' + new Date().getTime();

    var formData = new FormData();
    formData.append("values", JSON.stringify(values));

    var jqxhr = callRoboticsWS_POST(jsonEngineURI + "visit", formData, function (data) {
        if (!hasError(data)) {
            //parseVisit(data.d);
        } else {
            $("#mod-visit-response").html(showError({ text: data.d.result }));
        }
        closeVisitOnSave();
    }, function () {
        $("#mod-visit-response").html(showError({ text: $.t("unknownerror", "Error, aviste a soporte.") }));
        closeVisitOnSave();
    });
}

function closeVisitOnSave() {
    $("#mod-visit-response").html(showSuccess({ text: $.t("saved", "Datos almacenados correctamente.") }));
    // getVisitGest(_currVisitFilter);
    getVisitList(_currVisitFilterList, true);
    _.delay(function () {
        closeModal('#mod-visit');
    }, 1500);
}

function getVisitFieldsOpt() {
    if ($("#opt-visitfields > table").length > 0) {
        var table = $("#opt-visitfields-table").DataTable();
        table.destroy();
    }
    $("#opt-visitfields-response").html(showLoading());
    $("#opt-visitfields").html("");

    var params = 'timestamp=' + new Date().getTime();

    var jqxhr = callRoboticsWS_GET(jsonEngineURI + "visitfieldslist", params, function (data) {
        if (!hasError(data)) {
            //data.d.fields.sort(function (a, b) { return a.position > b.position; });
            parseOptVisitFields(data.d);
        } else {
            $("#opt-visitfields-response").html(showErrorClose({ text: data.d.result }));
        }
    }, function () {
        $("#opt-visitfields-response").html(showErrorClose({ text: $.t("unknownerror", "Error, aviste a soporte.") }));
    });
}

function getPrintConfig() {
    $("#opt-printconfig-response").html(showLoading());
    var params = 'timestamp=' + new Date().getTime();

    var jqxhr = callRoboticsWS_GET(jsonEngineURI + "visitfieldslist", params, function (data) {
        if (!hasError(data)) {
            getCurrentPrintConfig();
        } else {
            $("#opt-visitfields-response").html(showErrorClose({ text: data.d.result }));
        }
    }, function () {
        $("#opt-visitfields-response").html(showErrorClose({ text: $.t("unknownerror", "Error, aviste a soporte.") }));
    });
}

function getCurrentLegalTexts() {
    $("#opt-printconfig-response").html(showLoading());

    var params = 'timestamp=' + new Date().getTime();

    var jqxhr = callRoboticsWS_GET(jsonEngineURI + "getCurrentLegalTexts", params, function (data) {
        parseCurrentLegalTexts(data.d);
        $("#opt-printconfig-response").html("");
    }, function () {
        $("#opt-visitfields-response").html(showErrorClose({ text: $.t("unknownerror", "Error, aviste a soporte.") }));
    });
}

function getCurrentPrintConfig() {
    $("#opt-printconfig-response").html(showLoading());

    var params = 'timestamp=' + new Date().getTime();

    var jqxhr = callRoboticsWS_GET(jsonEngineURI + "printconfig", params, function (data) {
        parseCurrentPrintConfig(data.d);
        $("#opt-printconfig-response").html("");
    }, function () {
        $("#opt-visitfields-response").html(showErrorClose({ text: $.t("unknownerror", "Error, aviste a soporte.") }));
    });
}
function parseCurrentPrintConfig(data) {
    labelseditor.option("value", data.value);
}

function parseCurrentLegalTexts(data) {
    lawseditor.option("value", data.value1);
    lawseditor2.option("value", data.value2);
    $("#opt-legalText1").val(data.title1);
    $("#opt-legalText2").val(data.title2);
}

function parseOptVisitFields(data) {
    _currVisitFields = data.fields;
    localStorage.setItem("currVisitFields", JSON.stringify(_currVisitFields));

    var tmp = ""
    var cols = '';
    cols += "<th>" + $.t("position", "Posición") + "</th>";
    cols += "<th>" + $.t("name", "Nombre") + "</th>";
    cols += "<th>" + $.t("visivle", "Visible") + "</th>";
    cols += "<th>" + $.t("required", "Requerido") + "</th>";
    cols += "<th>" + $.t("visittypename", "Tipo") + "</th>";
    cols += '<th width="170px">&nbsp;</th>';

    tmp += '<table id="opt-visitfields-table" class="display" cellspacing="0" width="100%">';
    tmp += '<thead>';
    tmp += '<tr>' + cols + '</tr>';
    tmp += '</thead>';
    //tmp += '<tfoot>';
    //tmp += '<tr>' + cols + '</tr>';
    //tmp += '</tfoot>';
    tmp += '<tbody>';

    $(data.fields).each(function (index) {
        var vst = this;
        tmp += '<tr>'
        tmp += "   <td>" + vst.position + "</td>";
        tmp += "   <td>" + vst.name + "</td>";
        tmp += '   <td data-order="' + (vst.visible ? $.t("yes", "Si") : $.t("no", "No")) + '">' + (vst.visible ? "Si" : "No") + '</td>';
        var des = "";
        switch (vst.required) {
            case 0:
                des = $.t("norequired", "No requerido");
                break;
            case 1:
                des = $.t("requiredoncreatevisit", "Al crear la visita");
                break;
            case 2:
                des = $.t("requiredonstartvisit", "Al iniciar la visita");
                break;
            case 3:
                des = $.t("requiredonendvisit", "Al finalizar la visita");
                break;
        }

        tmp += '   <td data-order="' + des + '">' + des + '</td>';
        tmp += "   <td>" + vst.visittypename + "</td>";
        tmp += '<td data-order="' + vst.position + '"><a href="#/opt/visitfields/edit/' + vst.idfield + '"><i class="fa fa-pencil-square-o fa-fw fa-lg"></i></a>'
        tmp += '<a href="#/opt/visitfields/del/' + vst.idfield + '"><i class="fa fa-trash-o fa-fw fa-lg"></i></a>';
        if (index > 0) { tmp += "<a onclick='upVisitFieldsOpt(\"" + vst.idfield + "\")'> <i class='fa fa-caret-square-o-up fa-fw fa-lg'></i></a>"; }
        if (index < data.fields.length - 1) { tmp += "<a onclick='downVisitFieldsOpt(\"" + vst.idfield + "\")'><i class='fa fa-caret-square-o-down fa-fw fa-lg'></i>"; }
        tmp += "</td>";
        tmp += "</tr>"
    });
    if ($("#opt-visitfields > table").length > 0) {
        var table = $("#opt-visitfields-table").DataTable();
        table.destroy();
    }
    $("#opt-visitfields-response").html("");
    $("#opt-visitfields").html(tmp);
    _tblVisitsFields = $("#opt-visitfields-table").DataTable(datatablesOpt);
    $("#opt-visitfields-table_wrapper > .row").addClass("hide-on-print")
}
function upVisitFieldsOpt(idfield) {
    if ($("#opt-visitfields > table").length > 0) {
        var table = $("#opt-visitfields-table").DataTable();
        table.destroy();
    }
    $("#opt-visitfields-response").html(showLoading());
    $("#opt-visitfields").html("");

    var params = 'idfield=' + idfield;

    if (params != '') params += '&timestamp=' + new Date().getTime();
    else params = 'timestamp=' + new Date().getTime();

    var jqxhr = callRoboticsWS_GET(jsonEngineURI + "upvisitfield", params, function (data) {
        if (!hasError(data)) {
            //data.d.fields.sort(function (a, b) { return a.position > b.position; });
            parseOptVisitFields(data.d);
        } else {
            $("#opt-visitfields-response").html(showErrorClose({ text: data.d.result }));
        }
    }, function () {
        $("#opt-visitfields-response").html(showErrorClose({ text: $.t("unknownerror", "Error, aviste a soporte.") }));
    });
}
function downVisitFieldsOpt(idfield) {
    if ($("#opt-visitfields > table").length > 0) {
        var table = $("#opt-visitfields-table").DataTable();
        table.destroy();
    }
    $("#opt-visitfields-response").html(showLoading());
    $("#opt-visitfields").html("");

    var params = 'idfield=' + idfield;

    if (params != '') params += '&timestamp=' + new Date().getTime();
    else params = 'timestamp=' + new Date().getTime();

    var jqxhr = callRoboticsWS_GET(jsonEngineURI + "downvisitfield", params, function (data) {
        if (!hasError(data)) {
            //data.d.fields.sort(function (a, b) { return a.position > b.position; });
            parseOptVisitFields(data.d);
        } else {
            $("#opt-visitfields-response").html(showErrorClose({ text: data.d.result }));
        }
    }, function () {
        $("#opt-visitfields-response").html(showErrorClose({ text: $.t("unknownerror", "Error, aviste a soporte.") }));
    });
}
function delVisitFieldsOpt(idfield) {
    if ($("#opt-visitfields > table").length > 0) {
        var table = $("#opt-visitfields-table").DataTable();
        table.destroy();
    }
    $("#opt-visitfields-response").html(showLoading());
    $("#opt-visitfields").html("");

    var params = 'idfield=' + idfield;

    if (params != '') params += '&timestamp=' + new Date().getTime();
    else params = 'timestamp=' + new Date().getTime();

    var jqxhr = callRoboticsWS_GET(jsonEngineURI + "delvisitfield", params, function (data) {
        if (!hasError(data)) {
            data.d.fields.sort(function (a, b) { return a.position > b.position; });
            parseOptVisitFields(data.d);
        } else {
            $("#opt-visitfields-response").html(showErrorClose({ text: data.d.result }));
        }
    }, function () {
        $("#opt-visitfields-response").html(showErrorClose({ text: $.t("unknownerror", "Error, aviste a soporte.") }));
    });
}

function showVisitField(idvisitfield) {
    getVisitField(idvisitfield);
}
function getVisitField(idvisitfield) {
    $(".mod-visitfield-row, #mod-visitfield-buttons").addClass("hide");
    $("#mod-visitfield-response").html(showLoading());
    var params = 'idfield=' + idvisitfield;

    if (params != '') params += '&timestamp=' + new Date().getTime();
    else params = 'timestamp=' + new Date().getTime();

    var retCallback = function (pData) {
        return function () {
            var jqxhr = callRoboticsWS_GET(jsonEngineURI + "visitfield", pData, function (data) {
                if (!hasError(data)) {
                    parseVisitField(data.d);
                    openModal('#mod-visitfield');
                } else {
                    $("#mod-visitfield-response").html(showError({ text: data.d.result }));
                }
            }, function () {
                $("#mod-visitfield-response").html(showError({ text: $.t("unknownerror", "Error, aviste a soporte.") }));
            });
        };
    }

    getVisitTypes(params, retCallback(params));
}

function getVisitTypes(data, retCallback) {
    var jqxhr = callRoboticsWS_GET(jsonEngineURI + "visittypeslist", data, function (data) {
        if (!hasError(data)) {
            parseOptVisitTypesList(data.d);
            retCallback();
        } else {
            $("#opt-visittypes-response").html(showErrorClose({ text: data.result }));
        }
    }, function () {
        $("#opt-visittypes-response").html(showErrorClose({ text: $.t("unknownerror", "Error, aviste a soporte.") }));
    });
}
function parseOptVisitTypesList(data) {
    $("#mod-visitfield-type").empty();
    $("#mod-visit-type").empty();
    $("#mod-visitfield-type").append($('<option>', {
        value: 0,
        text: ""
    }));
    $("#mod-visit-type").append($('<option>', {
        value: 0,
        text: ""
    }));

    data.types.forEach(function (type) {
        var option = document.createElement("option");
        option.value = type.idtype;
        option.innerHTML = type.name;
        $("#mod-visitfield-type").append($('<option>', {
            value: option.value,
            text: option.innerHTML
        }));
        $("#mod-visit-type").append($('<option>', {
            value: option.value,
            text: option.innerHTML
        }));
    });
}
function parseVisitField(data) {
    //Eliminamos los errores anteriores
    $("#mod-visitfield .alert-input").remove();

    $("#mod-visitfield-idfield").val(data.idfield);
    $("#mod-visitfield-name").val(data.name);
    $("#mod-visitfield-visible").prop("checked", data.visible);
    $("#mod-visitfield-required").val(data.required);
    $("#mod-visitfield-type").val(data.visittype);
    $("#mod-visitfield-values").val(data.values);
    $("#mod-visitfield-edit").prop("checked", data.edit);
    $("#mod-visitfield-response").html("");
    $(".mod-visitfield-row, #mod-visitfield-buttons").removeClass("hide");
    if (data.values.length > 0) {
        $("#mod-visitfield-type-d").off('click');
        $("#mod-visitfield-type-d").click();
        $("#mod-visitfield-valuesdiv").removeClass("hide");
    } else {
        $("#mod-visitfield-type-t").off('click');
        $("#mod-visitfield-type-t").click();
        $("#mod-visitfield-valuesdiv").addClass("hide");
    }
}
function checkVisitField() {
    $("#mod-visitfield .alert-input").remove();
    if ($("#mod-visitfield-name").val() == "") {
        $("#mod-visitfield-name").after('<div class="alert-input">' + $.t("alertvisitorname", "Ha de indicar un nombre.") + '</div>');
    }
    return $("#mod-visitfield .alert-input").length == 0;
}
function saveVisitField() {
    var fld = { idfield: 0, name: "", required: 0 };
    fld.idfield = $("#mod-visitfield-idfield").val();
    fld.name = $("#mod-visitfield-name").val();
    fld.visible = $("#mod-visitfield-visible").prop("checked");
    fld.required = $("#mod-visitfield-required").val();
    fld.visittype = $("#mod-visitfield-type").val();
    fld.values = $("#mod-visitfield-values").val();
    fld.edit = $("#mod-visitfield-edit").prop("checked");
    if (checkVisitField()) { putVisitField(fld); }

    if (_.isString(_currOptions.multilocationField) && _currOptions.multilocationField != null && _currOptions.multilocationField.length > 0) {
        var fld = _currVisitFields.filter(function (itm) { return itm.idfield == _currOptions.multilocationField });
        if (fld.length > 0) {
            opt = "<option value=''>---</option>";
            if (fld[0].values.length > 0) {
                fld[0].values.split(";").forEach(function (itm) {
                    opt += "<option value='" + itm + "'>" + itm + "</option>"
                })
            }
            $("#plani-Location-selector").html(opt);
        }
        $("#plani-location").show();
    }
}

function saveVisitPrintConfig() {
    var pcnfg = {};

    pcnfg.name = "";
    pcnfg.value = labelseditor.option("value");

    //$("#printlabels")
    //labelcontent = labelseditor.option("value");

    putVisitPrintConfig(pcnfg);
}

function saveVisitLaws() {
    var laws = {};

    laws.title1 = $("#opt-legalText1").val();;
    laws.value1 = lawseditor.option("value");
    laws.title2 = $("#opt-legalText2").val();;
    laws.value2 = lawseditor2.option("value");

    putVisitLaws(laws);
}

function checkVisitType() {
    $("#mod-visittype .alert-input").remove();
    if ($("#mod-visittype-name").val() == "") {
        $("#mod-visittype-name").after('<div class="alert-input">' + $.t("alertvisitorname", "Ha de indicar un nombre.") + '</div>');
    }
    return $("#mod-visittype .alert-input").length == 0;
}
function putVisitType(values) {
    $(".mod-visittype-row, #mod-visittype-buttons").addClass("hide");
    $("#mod-visittype-response").html(showLoading());
    var params = 'values=' + encodeURIComponent(JSON.stringify(values));

    if (params != '') params += '&timestamp=' + new Date().getTime();
    else params = 'timestamp=' + new Date().getTime();

    var jqxhr = callRoboticsWS_GET(jsonEngineURI + "visittype", params, function (data) {
        if (!hasError(data)) {
            closeVisitTypeOnSave();
        } else {
            $("#mod-visittype-response").html(showError({ text: data.d.result }));
        }
    }, function () {
        $("#mod-visittype-response").html(showError({ text: $.t("unknownerror", "Error, aviste a soporte.") }));
    });
}
function saveVisitType() {
    var fld = { idtype: 0, name: "" };
    fld.idtype = $("#mod-visittype-idtype").val();
    fld.name = $("#mod-visittype-name").val();
    if (checkVisitType()) { putVisitType(fld); }
}

function putVisitLaws(values) {
    $("#opt-printconfig-response").html(showLoading());

    var formData = new FormData();
    formData.append("title1", encodeURIComponent(values.title1));
    formData.append("value1", encodeURIComponent(values.value1));
    formData.append("title2", encodeURIComponent(values.title2));
    formData.append("value2", encodeURIComponent(values.value2));

    var jqxhr = callRoboticsWS_POST(jsonEngineURI + "saveVisitLaws", formData, function (data) {
        $("#opt-printconfig-response").html("");
    }, function () {
        $("#opt-printconfig-response").html(showError({ text: $.t("unknownerror", "Error, aviste a soporte.") }));
    });
}

function putVisitPrintConfig(values) {
    $("#opt-printconfig-response").html(showLoading());
    var params = 'values=' + encodeURIComponent(JSON.stringify(values));

    if (params != '') params += '&timestamp=' + new Date().getTime();
    else params = 'timestamp=' + new Date().getTime();

    var jqxhr = callRoboticsWS_GET(jsonEngineURI + "savePrintConfig", params, function (data) {
        $("#opt-printconfig-response").html("");
    }, function () {
        $("#opt-printconfig-response").html(showError({ text: $.t("unknownerror", "Error, aviste a soporte.") }));
    });
}

function putVisitField(values) {
    if (values.visittype == null) {
        values.visittype = 0;
    }
    $(".mod-visitfield-row, #mod-visitfield-buttons").addClass("hide");
    $("#mod-visitfield-response").html(showLoading());
    var params = 'values=' + encodeURIComponent(JSON.stringify(values));

    if (params != '') params += '&timestamp=' + new Date().getTime();
    else params = 'timestamp=' + new Date().getTime();

    var jqxhr = callRoboticsWS_GET(jsonEngineURI + "visitfield", params, function (data) {
        if (!hasError(data)) {
            closeVisitFieldOnSave();
        } else {
            $("#mod-visitfield-response").html(showError({ text: data.d.result }));
        }
    }, function () {
        $("#mod-visitfield-response").html(showError({ text: $.t("unknownerror", "Error, aviste a soporte.") }));
    });
}

function closeVisitFieldOnSave() {
    $("#mod-visitfield-response").html(showSuccess({ text: $.t("saved", "Datos almacenados correctamente.") }));
    getVisitFieldsOpt();
    _.delay(function () {
        closeModal('#mod-visitfield');
    }, 1500);
}
function closeVisitTypeOnSave() {
    $("#mod-visittype-response").html(showSuccess({ text: $.t("saved", "Datos almacenados correctamente.") }));
    getVisitTypesOpt();
    _.delay(function () {
        closeModal('#mod-visittype');
    }, 1500);
}

/************             Visitas                ****************/
/****************************************************************/

/****************************************************************/
/************          AGM Tipos Visitas          ****************/

function getVisitTypesOpt() {
    if ($("#opt-visittypes > table").length > 0) {
        var table = $("#opt-visittypes-table").DataTable();
        table.destroy();
    }
    $("#opt-visittypes").html("");
    $("#opt-visittypes-response").html(showLoading());
    var params = 'timestamp=' + new Date().getTime();

    var jqxhr = callRoboticsWS_GET(jsonEngineURI + "visittypeslist", params, function (data) {
        if (!hasError(data)) {
            parseOptVisitTypes(data.d);
        } else {
            $("#opt-visittypes-response").html(showErrorClose({ text: data.result }));
        }
    }, function () {
        $("#opt-visittypes-response").html(showErrorClose({ text: $.t("unknownerror", "Error, aviste a soporte.") }));
    });
}
function parseOptVisitTypes(data) {
    var tmp = ""
    var cols = '';
    cols += "<th>" + $.t("idtype", "Id") + "</th>";
    cols += "<th>" + $.t("name", "Nombre") + "</th>";
    cols += '<th width="170px">&nbsp;</th>';

    tmp += '<table id="opt-visittypes-table" class="display" cellspacing="0" width="100%" align="center">';
    tmp += '<thead>';
    tmp += '<tr>' + cols + '</tr>';
    tmp += '</thead>';
    //tmp += '<tfoot>';
    //tmp += '<tr>' + cols + '</tr>';
    //tmp += '</tfoot>';
    tmp += '<tbody>';

    $(data.types).each(function (index) {
        var vst = this;
        tmp += '<tr>'
        tmp += "   <td>" + vst.idtype + "</td>";
        tmp += "   <td>" + vst.name + "</td>";
        tmp += '<td data-order="' + vst.idtype + '"><a href="#/opt/visittypes/edit/' + vst.idtype + '"><i class="fa fa-pencil-square-o fa-fw fa-lg"></i></a>'
        tmp += '<a href="#/opt/visittypes/del/' + vst.idtype + '"><i class="fa fa-trash-o fa-fw fa-lg"></i></a>';
        tmp += "</td>";
        tmp += "</tr>"
    });
    if ($("#opt-visittypes > table").length > 0) {
        var table = $("#opt-visittypes-table").DataTable();
        table.destroy();
    }
    $("#opt-visittypes-response").html("");
    $("#opt-visittypes").html(tmp);
    _tblVisitsFields = $("#opt-visittypes-table").DataTable(datatablesOpt);
    $("#opt-visittypes-table_wrapper > .row").addClass("hide-on-print")
}

function delVisitTypesOpt(idfield) {
    $("#opt-visittypes").html("");
    $("#opt-visittypes-response").html(showLoading());
    var params = 'idfield=' + idfield;

    if (params != '') params += '&timestamp=' + new Date().getTime();
    else params = 'timestamp=' + new Date().getTime();

    var jqxhr = callRoboticsWS_GET(jsonEngineURI + "delvisitorfield", params, function (data) {
        if (!hasError(data)) {
            data.d.fields.sort(function (a, b) { return a.position > b.position; });
            parseOptVisitTypes(data.d);
        } else {
            $("#opt-visittypes-response").html(showErrorClose({ text: data.result }));
        }
    }, function () {
        $("#opt-visittypes-response").html(showErrorClose({ text: $.t("unknownerror", "Error, aviste a soporte.") }));
    });
}

/************             Visitas                ****************/
/****************************************************************/

/****************************************************************/
/************             Visitantes             ****************/
function showVisitor(idvisitor, refresh) {
    if (!_.isUndefined(refresh)) { _refreshVisitVisitor = refresh; }
    getVisitor(idvisitor);
    openModal('#mod-visitor');
}

function getVisitor(idvisitor) {
    $(".mod-visitor-row, #mod-visitor-buttons").addClass("hide");
    $("#mod-visitor-response").html(showLoading());
    var params = 'idvisitor=' + idvisitor;

    if (params != '') params += '&timestamp=' + new Date().getTime();
    else params = 'timestamp=' + new Date().getTime();

    var jqxhr = callRoboticsWS_GET(jsonEngineURI + "visitor", params, function (data) {
        if (!hasError(data)) {
            parseVisitor(data.d);
            $('#mod-visitor .alert-box').parent().remove()
        } else {
            $("#mod-visitor-response").html(showError({ text: data.d.result }));
        }
    }, function () {
        $("#mod-visitor-response").html(showError({ text: $.t("unknownerror", "Error, aviste a soporte.") }));
    });
}
function parseVisitor(data) {
    var tmp = "";
    //Eliminamos los errores anteriores
    $("#mod-visitor .alert-input").remove();
    $("#mod-visit .alert-input").remove();

    //rellanamos datos
    $("#mod-visitor-idvisitor").val(data.idvisitor);
    $("#mod-visitor-name").val(data.name);
    $("#mod-visitor-private").prop("checked", data.isprivate);

    if (data.idvisitor == "new") {
        $("#mod-visitor-name").prop("readonly", false);
    } else {
        $("#mod-visitor-name").prop("readonly", true);
    }

    //Si hay escaner instalado muestro el botón
    if (_currOptions.scanServicePort == "" || _currOptions.scanServicePort == null) {
        $("#mod-visitorlist-buttons-addfromscanner").hide();
    }

    var fl = "";
    $("#mod-visitor-fields").html("");
    data.fields.sort(function (a, b) {
        return a.position > b.position;
    });
    //fl += '<div id="mod-visit-novisitors-div"><input id="mod-visit-novisitors" type="checkbox">&nbsp;<label for="mod-visit-novisitors">' + $.t("addtostartvisit", "Indicar al iniciar la visita") + '</label></div>';

    $(data.fields).each(function (index) {
        fl += '<div class="row mod-visitor-row mod-visitor-field hide">';
        fl += '    <div class="large-3 column large-text-right medium-text-left small-text-left">';
        fl += '        <label for="mod-visitor-field-' + this.name + '">' + this.name + ':</label>';
        fl += '    </div>';

        var m = moment(this.modified);
        if (this.values == '') {
            fl += '    <div class="large-9 column large-text-left text-left">';
            fl += '        <textarea data-idfield="' + this.idfield + '" data-name="' + this.name + '"';
            fl += ' data-required="' + this.required + '" data-askevery="' + this.askevery + '"';
            //console.log(this.value + "_" + m.format("LLL") + "-" + moment(this.modified).format("LLL"));
            if (_nextStatus == 1 && this.required == 3 && this.askevery == 3 && moment().isAfter(m.add(30, 'days')) && m.year > 1970) {
                fl += '        placeholder="' + $.t("caducado", "Valor caducado") + ': ' + this.value + '"  value="" data-value=""';
            } else if (_nextStatus == 1 && this.required == 3 && this.askevery == 2 && moment().isAfter(m.add(5, 'minutes'))) {
                fl += '        placeholder="' + $.t("lastvalue", "Valor anterior") + ': ' + this.value + '"  value="" data-value=""';
            } else if (this.value.length > 0) {
                //fl += '        value="' + this.value + '" data-value="' + this.value + '" readonly="' + (this.edit) ? "false" : "true";
                fl += '" data-value="' + this.value + '"';
                if (!this.edit || this.required == 99) {
                    fl += '       readonly="true"';
                }
            } else {
                fl += '" data-value="' + this.value + '"';
            }
            fl += ' id="mod-visitor-field-' + this.name + '" name="mod-visitor-field-' + this.name + '"  type="text" maxlength="100">';
            fl += this.value + '</textarea>'
        } else {
            fl += '    <div class="large-9 column large-text-left medium-text-left small-text-left">';
            fl += '                <select data-idfield="' + this.idfield + '" data-name="' + this.name + '" data-required="' + this.required + '" data-askevery="' + this.askevery + '"';
            fl += '                   name="mod-visit-field-' + this.name + '" id="mod-visit-field-' + this.name + '" value="' + this.value + '"';
            if (_nextStatus == 1 && this.required == 3 && this.askevery == 3 && moment().isAfter(m.add(30, 'days')) && m.year > 1970) {
                fl += ' >';
                fl += '        <option value="">' + $.t("selectvalue", "-- Seleccione un valor --") + '</option>';
            } else if (_nextStatus == 1 && this.required == 3 && this.askevery == 2 && moment().isAfter(m.add(5, 'minutes'))) {
                fl += ' >';
                fl += '        <option value="">' + $.t("selectvalue", "-- Seleccione un valor --") + '</option>';
            } else if (this.value.length > 0) {
                fl += ' disabled >';
            } else {
                fl += ' >';
                fl += '        <option value="">' + $.t("selectvalue", "-- Seleccione un valor --") + '</option>';
            }
            var value = this.value;
            $(this.values.split(";")).each(function (index, val) {
                fl += '        <option value="' + val + '"';
                if (value == val) { fl += ' selected '; }
                fl += '>' + val + '</option>';
            });
            fl += '                </select>';
            fl += '    </div>';
        }

        fl += '    </div>';
        fl += '</div>';
    });

    if (_showPunches && data.punches.length > 0) {
        var punc = "";
        $("#visitor-row-punches").removeClass("hide_");
        data.punches.sort(function (a, b) {
            return moment(a.punchdate).isAfter(b.punchdate);
        });
        punc = '<table width="100%">';
        punc += "<thead>";
        punc += "<th>" + $.t("employee", "Empleado") + "</th>";
        punc += "<th>" + $.t("datetime", "Día/Hora") + "</th>";
        punc += "<th>" + $.t("action", "Acción") + "</th>";
        punc += "</thead>";
        punc += "<tbody>";
        $(data.punches).each(function (index) {
            punc += "<tr>";
            punc += "<td>" + this.visitorname + "</td>";
            punc += "<td>" + moment(this.punchdate).format("DD/MM/YYYY HH:mm") + "</td>";
            punc += "<td>" + (this.action == "IN" ? "Entrada" : "Salida") + "</td>";
            punc += "</tr>";
        });
        punc += "</tbody>";
        punc += "</table>";
        $("#mod-visitor-punches").html(punc);
    } else {
        $("#visitor-row-punches").addClass("hide_");
        $("#mod-visitor-punches").html("");
    }

    $("#mod-visitor-fields").html(fl);
    $(".mod-visitor-row, #mod-visitor-buttons").removeClass("hide");
    $("#mod-visitor-response").html("");

    //if (data.idvisitor = "new") {
    //    $(".mod-visitor-row input[data-required=2] ").parent().parent().addClass("hide")
    //} else {
    //    $(".mod-visitor-row input[data-required=2] ").parent().parent().addClass("hide")
    //}
}
function checkVisitor(status) {
    $("#mod-visitor .alert-input").remove();
    if ($("#mod-visitor-name").val() == "") {
        $("#mod-visitor-name").after('<div class="alert-input">Este campo es obligatorio.</div>');
    }

    $("#mod-visitor-fields input, #mod-visitor-fields select").each(function (index) {
        if ($(this).val() == "") {
            if ($(this).data("required") == 1) {
                $(this).after('<div class="alert-input">Este campo es obligatorio.</div>');
            }
            switch (_nextStatus) {
                case 0:   //crear un visita
                    if ($(this).data("required") == 2) {
                        $(this).after('<div class="alert-input">Este campo es obligatorio al crear la visita.</div>');
                    }
                    break;
                case 1:   //iniciar una visita
                    if ($(this).data("required") == 3) {
                        $(this).after('<div class="alert-input">Este campo es obligatorio al iniciar la visita.</div>');
                    }
                    break;
            }
        }
    });
    return $("#mod-visitor .alert-input").length == 0;
}
function saveVisitor() {
    var vstr = { idvisitor: 0, name: "", fields: [] };
    var fld = { idvisitor: 0, idfield: 0, value: "" };

    vstr.idvisitor = $("#mod-visitor-idvisitor").val();
    vstr.name = $("#mod-visitor-name").val();
    vstr.isprivate = $("#mod-visitor-private").prop("checked");
    $("#mod-visitor-fields textarea, #mod-visitor-fields select").each(function (index) {
        //if ($(this).val() != $(this).data("value")) {
        fldv = _.clone(fld);
        fldv.idvisitor = vstr.idvisitor;
        fldv.idfield = $(this).data("idfield");
        fldv.value = $(this).val();
        vstr.fields.push(fldv);
        // }
    });
    if (checkVisitor()) { putVisitor(vstr); }
}

function getVisitorDataFromScannerGET() {
    this.varData = { "usuario": _currOptions.scanServiceUser, "pwd": _currOptions.scanServicePwd, "tipoDocumento": "0", "nuevoEscaneo": "1", "modoPresentacion": "2" };
    this.varContentType = "application/json; charset=utf-8";
    this.varDataType = "json";
    this.varProcessData = true;
    this.url = "http://localhost:" + _currOptions.scanServicePort + "/DltId/DltIdServer.dll?DltGetDocument";

    $('#mod-visitor .alert-box').parent().remove()
    var extendContext = function (data, textStatus) {
        if (data.error == undefined) {
            if ($("#mod-visitor-name").is("[readonly]") != true) {
                $("#mod-visitor-name").val(data.documento.nombre + ' ' + data.documento.apellido1 + ' ' + data.documento.apellido2);
            }
            if (_currOptions.documentNumberfield != '' && $("#mod-visitor-field-" + _currOptions.documentNumberfield).is("[readonly]") != true) {
                $("#mod-visitor-field-" + _currOptions.documentNumberfield).val(data.documento.numero_documento);
            }
        }
        else {
            if (data.error == '-200') {
                $("#mod-visitor-buttons").after(showError({ text: $.t("scannerunaccessible", "No se pudo acceder al escáner") + " (" + data.error + ")" }));
            }
            else {
                if (data.error == '-2') {
                    $("#mod-visitor-buttons").after(showError({ text: $.t("scannerunrecognizeddocument", "Formato de documento desconocido") + " (" + data.error + ")" }));
                }
                else {
                    $("#mod-visitor-buttons").after(showError({ text: $.t("scannerunknownerror", "Error procesando documento") + " (" + data.error + ")" }));
                }
            }
        }
    };

    var extendContextError = function (xhr, ajaxOptions, thrownError) {
        $("#mod-visitor-buttons").after(showError({ text: $.t("errorrecoveringscannerdata", "Error recuperando información de documento en") + ": http://localhost:" + _currOptions.scanServicePort }));
    }

    var extendContextComplete = function (xhr, ajaxOptions, thrownError) {
        $('#mod-visitor .loading').remove();
    }

    $("#mod-visitor-buttons").after(showLoading());

    $.ajax({
        type: "GET",
        url: this.url,
        timeout: 30000,
        data: decodeURIComponent($.param(this.varData)),
        contentType: this.varContentType,
        dataType: this.varDataType,
        processdata: this.varProcessData,
        success: extendContext,
        error: extendContextError,
        complete: extendContextComplete
    });
}

function getVisitorDataFromScanner() {
    if (window.XMLHttpRequest) {// code for IE7+, Firefox, Chrome, Opera, Safari
        xmlhttp = new XMLHttpRequest();
    }
    $('#mod-visitor .alert-box').parent().remove()
    $('#mod-visitor .loading').remove();
    var queryString = "usuario=" + _currOptions.scanServiceUser + "&pwd=" + _currOptions.scanServicePwd + "&tipoDocumento=0&nuevoEscaneo=1&modoPresentacion=2";
    $("#mod-visitor-buttons").after(showLoading());

    xmlhttp.open("GET", "https://localhost:" + _currOptions.scanServicePort + "/DltId/DltIdServer.dll?DltGetDocument?" + queryString, true);
    xmlhttp.onreadystatechange = function () {
        if (xmlhttp.readyState == 4) {
            if (xmlhttp.status == 200) {
                // Respuesta
                try {
                    var data = JSON.parse(xmlhttp.response);
                } catch (e) {
                    var xmlDoc = xmlhttp.responseXML;
                    if (xmlDoc.firstChild.localName == 'error') {
                        $('#mod-visitor .loading').remove();
                        $("#mod-visitor-buttons").after(showError({ text: $.t("scannerunknownerror", "Error procesando documento") + " (" + xmlDoc.firstChild.childNodes[0].nodeValue + " - " + xmlDoc.firstChild.childNodes[1].innerHTML + ")" }));
                    }
                }
                //if (data.necesita_siguiente_pagina) {
                //    alert("Por favor, introduzca la siguiente cara");
                //    queryString = "usuario=" + _currOptions.scanServiceUser + "&pwd=" + _currOptions.scanServicePwd + "&tipoDocumento=0&nuevoEscaneo=0&modoPresentacion=2";
                //    $("#mod-visitor-buttons").after(showLoading());
                //    xmlhttp.open("GET", "https://localhost:" + _currOptions.scanServicePort + "/DltId/DltIdServer.dll?DltGetDocument?" + queryString, true);
                //    try {
                //        xmlhttp.send();
                //    } catch (e) {
                //        $("#mod-visitor-buttons").after(showError({ text: $.t("errorrecoveringscannerdata", "Error recuperando información de documento en") + ": https://localhost:" + _currOptions.scanServicePort }));
                //        $('#mod-visitor .alert-box').parent().remove();
                //        $('#mod-visitor .loading').remove();
                //    }

                //    $('#mod-visitor .alert-box').parent().remove()
                //    data = JSON.parse(xmlhttp.response);
                //}
                if (data.documento) {
                    if (data.error == undefined) {
                        if ($("#mod-visitor-name").is("[readonly]") != true) {
                            $("#mod-visitor-name").val(data.documento.nombre + ' ' + data.documento.apellido1 + ' ' + data.documento.apellido2);
                        }
                        if (_currOptions.documentNumberfield != '' && $("#mod-visitor-field-" + _currOptions.documentNumberfield).is("[readonly]") != true) {
                            $("#mod-visitor-field-" + _currOptions.documentNumberfield).val(data.documento.numero_documento);
                        }
                    }
                    else {
                        if (data.error == '-200') {
                            $("#mod-visitor-buttons").after(showError({ text: $.t("scannerunaccessible", "No se pudo acceder al escáner") + " (" + data.error + ")" }));
                        }
                        else {
                            if (data.error == '-2') {
                                $("#mod-visitor-buttons").after(showError({ text: $.t("scannerunrecognizeddocument", "Formato de documento desconocido") + " (" + data.error + ")" }));
                            }
                            else {
                                $("#mod-visitor-buttons").after(showError({ text: $.t("scannerunknownerror", "Error procesando documento") + " (" + data.error + ")" }));
                            }
                        }
                    }
                    $('#mod-visitor .loading').remove();
                }

                if (data.error) {
                    $('#mod-visitor .loading').remove();
                    $("#mod-visitor-buttons").after(showError({ text: $.t("errorrecoveringscannerdata", "Error recuperando información de documento en") + ": https://localhost:" + _currOptions.scanServicePort + ". " + data.error + " - " + data.description }));
                }
            }
            else {
                if (xmlhttp.status == 0) {
                    // No hubo respuesta
                    $("#mod-visitor-buttons").after(showError({ text: $.t("errorrecoveringscannerdata", "Error recuperando información de documento en") + ": https://localhost:" + _currOptions.scanServicePort }));
                    $('#mod-visitor .loading').remove();
                }
            }
        }
    }
    xmlhttp.send();
}

function getVisitorDataFromImage() {
    this.varData = { "modoPresentacion": "2", "usuario": "admin", "pwd": "admin", "tipoDocumento": "0", "nuevoEscaneo": "1", "imagen1": "img1base64", "imagen2": "img2base64" };
    this.varContentType = "application/json; charset=utf-8";
    this.varDataType = "json";
    this.varProcessData = true;
    this.url = "https://localhost:" + _currOptions.scanServicePort + "/DltId/DltIdServer.dll?DltGetDocumentEx";

    var extendContext = function (data, textStatus) {
        $("#mod-visitor-name").val(data.documento.nombre);
        if (_currOptions.documentNumberfield != '') {
            $("#mod-visitor-field-" + _currOptions.documentNumberfield).val(data.documento.numero_documento);
        }
    };

    var extendContextError = function (xhr, ajaxOptions, thrownError) {
        alert("Error recuperando información de documento");
    }

    $.ajax({
        type: "POST",
        url: this.url,
        timeout: 30000,
        data: decodeURIComponent($.param(this.varData)),
        contentType: this.varContentType,
        dataType: this.varDataType,
        processdata: this.varProcessData,
        success: extendContext,
        error: extendContextError
    });
}

function onChange() {
    var checked = $("#mod-consent-edit").is(':checked');

    if (checked == true) {
        $("#mod-consent-buttons-save").show();
    } else {
        $("#mod-consent-buttons-save").hide();
    }
}

function saveConsent(values) {
    var message = $("#mod-consent-name").text();
    $(".mod-consent-row, #mod-consent-buttons").addClass("hide");
    $("#mod-consent-response").html(showLoading());

    var formData = new FormData();
    formData.append("values", message);

    var jqxhr = callRoboticsWS_POST(jsonEngineURI + "SaveConsent", formData, function (data) {
        if (data.d.Status == 0) {
            closeModal('#mod-consent');
            localStorage.setItem("consent", JSON.stringify(true))
        } else {
            $("#mod-consent-response").html(showError({ text: data.d.result }));
        }
    }, function () {
        $("#mod-consent-response").html(showError({ text: $.t("unknownerror", "Error, aviste a soporte.") }));
    });
}

function putVisitor(values) {
    $(".mod-visitor-row, #mod-visitor-buttons").addClass("hide");
    $("#mod-visitor-response").html(showLoading());
    var params = 'values=' + encodeURIComponent(JSON.stringify(values));

    if (params != '') params += '&timestamp=' + new Date().getTime();
    else params = 'timestamp=' + new Date().getTime();

    var jqxhr = callRoboticsWS_GET(jsonEngineURI + "visitor", params, function (data) {
        if (!hasError(data)) {
            _visitorsGestCache = null;
            _visitorsCache = null;
            _newVisitor = data.d.idvisitor;
            closeVisitorOnSave();
        } else {
            $("#mod-visitor-response").html(showError({ text: data.d.result }));
        }
    }, function () {
        $("#mod-visitor-response").html(showError({ text: $.t("unknownerror", "Error, aviste a soporte.") }));
    });
}
function closeVisitorOnSave() {
    $("#mod-visitor-response").html(showSuccess({ text: $.t("saved", "Datos almacenados correctamente.") }));
    $("#visit-visitors [data-idvisitor=" + $("#visitor-idvisitor").val() + "] a:first-child").html($("#visitor-name").val());

    getVisitorList({ _loadpunches_: false }, true);
    getVisitorGest(_currVisitorFilter, true);
    if (_nextStatus >= 0 && _refreshVisitVisitor) {
        getVisitVisitors(_currVisitorListFilter);
    }
    _refreshVisitVisitor = true;
    _.delay(function () {
        closeModal('#mod-visitor');
    }, 1500);
}

function showVisitorList() {
    getVisitorList({ _loadpunches_: false });
    _newVisitor = "";
    _currVisitorListFilter = { ids: "" };
    openModal('#mod-visitorlist');
}
function getVisitorList(filter, forceLoad) {
    if ($("#visitorslist > table").length > 0) {
        var table = $("#visitorslist-table").DataTable();
        table.destroy();
    }
    $("#visitorslist").html("");

    $("#visitorslist-response").html(showLoading());
    var params = 'filter=' + encodeURIComponent(JSON.stringify(filter))
    params += '&timestamp=' + new Date().getTime();

    var reloadList = (typeof forceLoad == 'undefined') ? false : forceLoad;

    parseVisitorsList({ visitors: [] }, true);
    if (JSON.stringify(filter) != _visitorsCacheFilter || _visitorsCache == null || reloadList) {
        _visitorsCacheFilter = JSON.stringify(filter);
        var jqxhr = callRoboticsWS_GET(jsonEngineURI + "visitorlist", params, function (data) {
            if (!hasError(data)) {
                _visitorsCache = data.d;
                parseVisitorsList(_visitorsCache, false);
            } else {
                $("#visitorslist-response").html(showErrorClose({ text: data.result }));
            }
        }, function () {
            $("#visitorslist-response").html(showErrorClose({ text: data.result }));
        });
    } else {
        parseVisitorsList(_visitorsCache, false);
    }
}
function parseVisitorsList(data, bIsLoading) {
    var tmp = ""
    var cols = "<th>&nbsp;</th>";
    cols += "<th>Nombre</th>";
    if (data.visitors.length > 0) {
        data.visitors[0].fields.sort(function (a, b) {
            return a.position > b.position;
        });

        $(data.visitors[0].fields).each(function (index) {
            if (index < 3) {
                cols += "<th>" + this.name + "</th>";
            }
        });
    }
    cols += "<th>&nbsp;</th>";

    tmp += '<table id="mod-visitorlist-table" class="display compact" cellspacing="0" width="100%">';
    tmp += '<thead>';
    tmp += '<tr>' + cols + '</tr>';
    tmp += '</thead>';
    //tmp += '<tfoot>';
    //tmp += '<tr>' + cols + '</tr>';
    //tmp += '</tfoot>';
    tmp += '<tbody>';

    $(data.visitors).each(function (index) {
        var vst = this;
        tmp += '<tr>'
        tmp += '<td data-order="' + vst.idvisitor + '"><input id="visitorslist-idvisitor-' + vst.idvisitor + '" data-idvisitor="' + vst.idvisitor + '" data-visitor-name="' + vst.name + '" type="checkbox"></td>';
        tmp += "   <td>" + vst.name + "</td>"
        vst.fields.sort(function (a, b) {
            return a.position > b.position;
        });
        $(vst.fields).each(function (index2) {
            if (index2 < 3) {
                tmp += "<td>" + this.value + "</td>";
            }
        });

        tmp += '<td><a onclick="showVisitor(\'' + vst.idvisitor + '\')"><i class="fa fa-pencil-square-o fa-lg"></i></a></td>';
        tmp += "</tr>"
    });
    tmp += '</tbody></table>';
    if ($("#mod-visitorlist > table").length > 0) {
        var table = $("#mod-visitorlist-table").DataTable();
        table.destroy();
    }
    $("#mod-visitorlist-response").html("");

    if (bIsLoading) {
        $("#mod-visitorlist-content").html(showLoading());
    } else {
        $("#mod-visitorlist-content").html(tmp);
    }
    //Marcamos los visitantes ya seleccionados
    $(".mod-visit-visitor-item").each(function (index) {
        $('#visitorslist-idvisitor-' + $(this).data("idvisitor")).attr("checked", "true");
    });
    if (_newVisitor.length > 0) { $('#visitorslist-idvisitor-' + _newVisitor).attr("checked", "true"); }

    _tblVisitorsList = $("#mod-visitorlist-table").DataTable(datatablesOptOrd);
    $("#mod-visitorlist-table_wrapper > .row").addClass("hide-on-print")
}

function getVisitorGest(filter, forceLoad) {
    if ($("#gest-visitors > table").length > 0) {
        var table = $("#gest-visitors-table").DataTable();
        table.destroy();
    }
    $("#gest-visitors").html("");
    $("#gest-visitors-response").html(showLoading());
    var params = 'timestamp=' + new Date().getTime();
    if (_.isObject(filter) && filter != "") { params += '&filter=' + encodeURIComponent(JSON.stringify(filter)); }

    var reloadList = (typeof forceLoad == 'undefined') ? false : forceLoad;
    parseVisitorsGest({ visitors: [] }, true);
    if (JSON.stringify(filter) != _visitorsGestCacheFilter || _visitorsGestCache == null || reloadList) {
        _visitorsGestCacheFilter = JSON.stringify(filter);

        var jqxhr = callRoboticsWS_GET(jsonEngineURI + "visitorlist", params, function (data) {
            if (!hasError(data)) {
                _visitorsGestCache = data.d;
                parseVisitorsGest(_visitorsGestCache, false);
            } else {
                $("#gest-visitors-response").html(showErrorClose({ text: data.result }));
            }
        }, function () {
            $("#gest-visitors-response").html(showErrorClose({ text: $.t("unknownerror", "Error, aviste a soporte.") }));
        });
    } else {
        parseVisitorsGest(_visitorsGestCache, false);
    }
}
function parseVisitorsGest(data, bIsLoading) {
    var tmp = ""
    var cols = '';
    var colsTo = ""
    cols += "<th>" + $.t("name") + "</th>";
    colsTo += '<input type="checkbox" class="toggle-visitors" data-column="0" ' + (any2Boolean(_columns_visitors_visible[1], true) ? 'checked' : '') + '>&nbsp;' + $.t("name");

    if (!_.isNull(localStorage.getItem("columns_visitors_visible"))) {
        _.each(localStorage.getItem("columns_visitors_visible").split(","), function (element, index, list) {
            _columns_visitors_visible[index] = any2Boolean(element, true);
        });
    }

    if (data.visitors.length > 0) {
        data.visitors[0].fields.sort(function (a, b) {
            return a.position > b.position;
        });

        if (data.visitors[0].fields.length > 0) { _currVisitorFields = []; }
        $(data.visitors[0].fields).each(function (index) {
            cols += "<th>" + this.name + "</th>";
            colsTo += ' - <input type="checkbox" class="toggle-visitors" data-column="' + (1 + index) + '" ' + (any2Boolean(_columns_visitors_visible[1 + index], true) ? 'checked' : '') + '>' + this.name;
            _currVisitorFields.push({ 'id': this.idfield, 'name': this.name });
        });
    }

    //Campos personalizados
    var fils = '';
    _.each(_currVisitorFields, function (element, index, list) {
        fils += '<div>';
        fils += '    <span>' + element.name + ': </span><i class="fa fa-trash-o resetinput"></i>';
        fils += '    <input id="gest-visitors-filters-cst-' + element.id + '" type="text" />';
        fils += ' </div>';
    });

    $("#visitorcustomfilters").html(fils);
    refreshReset();
    _.each(_currVisitorFilter, function (element, index, list) {
        if (index != 'name') {
            if (_.isString(element)) {
                $("#gest-visitors-filters-cst-" + index).val(element);
            } else {
                $("#gest-visitors-filters-cst-" + index).val("");
            }
        }
    });

    cols += '<th class="hide-on-print" width="120px">&nbsp;</th>';
    tmp += '<table id="gest-visitors-table" class="display" cellspacing="0" width="100%">';
    tmp += '<thead>';
    tmp += '<tr>' + cols + '</tr>';
    tmp += '</thead>';
    //tmp += '<tfoot>';
    //tmp += '<tr>' + cols + '</tr>';
    //tmp += '</tfoot>';
    tmp += '<tbody>';

    $(data.visitors).each(function (index) {
        var vst = this;
        tmp += '<tr>'
        tmp += "   <td>" + vst.name + "</td>"
        vst.fields.sort(function (a, b) {
            return a.position > b.position;
        });
        $(vst.fields).each(function (index2) {
            tmp += "<td>" + this.value + "</td>";
        });
        tmp += '<td class="hide-on-print" data-order="' + vst.idvisitor + '"><a href="#/gest/visitors/edit/' + vst.idvisitor + '"><i class="fa fa-pencil-square-o fa-lg"></i></a>'
        tmp += '&nbsp;&nbsp;<a href="#/gest/visitors/del/' + vst.idvisitor + '"><i class="fa fa-trash-o fa-lg"></i></a></td>';
        tmp += "</tr>"
    });
    tmp += '</tbody></table>';
    if ($("#gest-visitors > table").length > 0) {
        var table = $("#gest-visitors-table").DataTable();
        table.destroy();
    }
    $("#gest-visitors-response").html("");
    if (bIsLoading) {
        $("#gest-visitors").html(showLoading());
    } else {
        $("#gest-visitors").html(tmp);
        $("#gest-visitors-columns-values").html(colsTo);
        _tblVisitors = $("#gest-visitors-table").DataTable(datatablesOptOrd);
        _tblVisitors.off('draw');
        _tblVisitors.on('draw', function () {
            $(".dataTables_wrapper>.row").addClass("hide-on-print");
        });

        _.each(_columns_visitors_visible, function (element, index, list) {
            var column = _tblVisitors.column(index);
            column.visible(element);
        });
        $('input.toggle-visitors').off('change');
        $('input.toggle-visitors').on('change', function (e) {
            e.preventDefault();
            var column = _tblVisitors.column($(this).attr('data-column'));
            column.visible($(this).prop("checked"));
            _columns_visitors_visible[$(this).attr('data-column')] = $(this).prop("checked");
            localStorage.setItem("columns_visitors_visible", _columns_visitors_visible);
        });
        $('#mod-visitfield-required').on('change', function (e) {
            e.preventDefault();
            if ($("#mod-visitfield-required").val() != "99") {
                $("#row-mod-visitfield-type").show();
                $("#row-mod-visitfield-edit").show();
            } else {
                $("#row-mod-visitfield-type").hide();
                $("#row-mod-visitfield-edit").hide();
                $("#mod-visitfield-edit").val(false);
            }
        });
    }
}
function delVisitorGest(idvisitor) {
    if ($("#gest-visitors > table").length > 0) {
        var table = $("#gest-visitors-table").DataTable();
        table.destroy();
    }
    $("#gest-visitors").html("");
    $("#gest-visitors-response").html(showLoading());
    var params = 'idvisitor=' + idvisitor;

    if (params != '') params += '&timestamp=' + new Date().getTime();
    else params = 'timestamp=' + new Date().getTime();

    parseVisitorsGest({ visitors: [] }, true);

    var jqxhr = callRoboticsWS_GET(jsonEngineURI + "delvisitor", params, function (data) {
        if (!hasError(data)) {
            _visitorsGestCache = null;
            _visitorsCache = null;
            parseVisitorsGest(data.d, false);
        } else {
            $("#gest-visitors-response").html(showErrorClose({ text: data.result }));
        }
    }, function () {
        $("#gest-visitors-response").html(showErrorClose({ text: $.t("unknownerror", "Error, aviste a soporte.") }));
    });
}

function cleanfilterVisitorGest() {
    _currVisitorFilter = { _loadpunches_: false };
    $("#gest-visitors-filters-name").val("");
    $("[id^='gest-visitors-filters-cst']").each(function (index, value) {
        $(value).val("");
    });
    filterVisitorGest();
}
function filterVisitorGest() {
    _currVisitorFilter = { _loadpunches_: false };
    if ($("#gest-visitors-filters-name").val().length > 0) {
        _currVisitorFilter["name"] = $("#gest-visitors-filters-name").val();
    } else {
        _currVisitorFilter["name"] = "";
    }

    $("[id^='gest-visitors-filters-cst']").each(function (index, value) {
        var tmpItem = value.id.replace('gest-visitors-filters-cst-', '');

        if ($(value).val().length > 0) {
            _currVisitorFilter[tmpItem] = $(value).val();
        } else {
            _currVisitorFilter[tmpItem] = "";
        }
    });

    localStorage.setItem("currVisitorFilter", JSON.stringify(_currVisitorFilter));
    getVisitorGest(_currVisitorFilter, false);
}

function getVisitorFieldsOpt() {
    if ($("#opt-visitorfields > table").length > 0) {
        var table = $("#opt-visitorfields-table").DataTable();
        table.destroy();
    }
    $("#opt-visitorfields").html("");
    $("#opt-visitorfields-response").html(showLoading());
    var params = 'timestamp=' + new Date().getTime();

    var jqxhr = callRoboticsWS_GET(jsonEngineURI + "visitorfieldslist", params, function (data) {
        if (!hasError(data)) {
            //data.d.fields.sort(function (a, b) { return a.position > b.position; });
            parseOptVisitorFields(data.d);
        } else {
            $("#opt-visitorfields-response").html(showErrorClose({ text: data.result }));
        }
    }, function () {
        $("#opt-visitorfields-response").html(showErrorClose({ text: $.t("unknownerror", "Error, aviste a soporte.") }));
    });
}
function parseOptVisitorFields(data) {
    var tmp = ""
    var cols = '';
    cols += "<th>" + $.t("position", "Posición") + "</th>";
    cols += "<th>" + $.t("name", "Nombre") + "</th>";
    cols += "<th>" + $.t("visivle", "Visible") + "</th>";
    cols += "<th>" + $.t("required", "Requerido") + "</th>";
    cols += "<th>" + $.t("ask", "Preguntar") + "</th>";
    cols += '<th width="170px">&nbsp;</th>';

    tmp += '<table id="opt-visitorfields-table" class="display" cellspacing="0" width="100%">';
    tmp += '<thead>';
    tmp += '<tr>' + cols + '</tr>';
    tmp += '</thead>';
    //tmp += '<tfoot>';
    //tmp += '<tr>' + cols + '</tr>';
    //tmp += '</tfoot>';
    tmp += '<tbody>';

    $(data.fields).each(function (index) {
        var vst = this;
        tmp += '<tr>';
        tmp += "   <td>" + vst.position + "</td>";
        tmp += "   <td>" + vst.name + "</td>";
        tmp += "   <td>" + (vst.visible ? $.t("yes", "Si") : $.t("no", "No")) + "</td>";
        var des = "";
        switch (vst.required) {
            case 0:
                des = $.t("norequired", "No requerido");
                break;
            case 1:
                des = $.t("requiredoncreatevisitor", "Al crear el visitante")
                break;
            case 2:
                des = $.t("requiredoncreatevisit", "Al crear la visita");
                break;
            case 3:
                des = $.t("requiredonstartvisit", "Al iniciar la visita");
                break;
        }
        tmp += '   <td data-order="' + des + '">' + des + '</td>';
        switch (vst.askevery) {
            case 0:
                des = "";
                break;
            case 1:
                des = $.t("ontime", "Una sola vez");
                break;
            case 2:
                des = $.t("everystartbisit", "Cada vez al iniciar la visita");
                break;
            case 3:
                des = $.t("every30days", "Cada 30 días");
                break;
            //case 4:
            //    des = $.t("alwaysEdit", "Editar al iniciar la visita");
            //    break;
        }
        tmp += '   <td data-order="' + des + '">' + des + '</td>';

        tmp += '<td data-order="' + vst.position + '"><a href="#/opt/visitorfields/edit/' + vst.idfield + '"><i class="fa fa-pencil-square-o fa-lg"></i></a>';
        tmp += '<a href="#/opt/visitorfields/del/' + vst.idfield + '"><i class="fa fa-trash-o fa-lg"></i></a>';
        if (index > 0) { tmp += "<a onclick='upVisitorFieldsOpt(\"" + vst.idfield + "\")'> <i class='fa fa-caret-square-o-up fa-fw fa-lg'></i></a>"; }
        if (index < data.fields.length - 1) { tmp += "<a onclick='downVisitorFieldsOpt(\"" + vst.idfield + "\")'><i class='fa fa-caret-square-o-down fa-fw fa-lg'></i></a>"; }
        tmp += "   </td>";
        tmp += "</tr>";
    });
    if ($("#opt-visitorfields > table").length > 0) {
        var table = $("#opt-visitorfields-table").DataTable();
        table.destroy();
    }
    $("#opt-visitorfields-response").html("");
    $("#opt-visitorfields").html(tmp);
    _tblVisitorsFields = $("#opt-visitorfields-table").DataTable(datatablesOpt);
    $("#opt-visitorfields-table_wrapper > .row").addClass("hide-on-print")
}
function upVisitorFieldsOpt(idfield) {
    if ($("#opt-visitorfields > table").length > 0) {
        var table = $("#opt-visitorfields-table").DataTable();
        table.destroy();
    }
    $("#opt-visitorfields-response").html(showLoading());
    $("#opt-visitorfields").html("");

    var params = 'idfield=' + idfield;

    if (params != '') params += '&timestamp=' + new Date().getTime();
    else params = 'timestamp=' + new Date().getTime();

    var jqxhr = callRoboticsWS_GET(jsonEngineURI + "upvisitorfield", params, function (data) {
        if (!hasError(data)) {
            data.d.fields.sort(function (a, b) { return a.position > b.position; });
            parseOptVisitorFields(data.d);
        } else {
            $("#opt-visitorfields-response").html(showErrorClose({ text: data.d.result }));
        }
    }, function () {
        $("#opt-visitorfields-response").html(showErrorClose({ text: $.t("unknownerror", "Error, aviste a soporte.") }));
    });
}
function downVisitorFieldsOpt(idfield) {
    if ($("#opt-visitorfields > table").length > 0) {
        var table = $("#opt-visitorfields-table").DataTable();
        table.destroy();
    }
    $("#opt-visitorfields-response").html(showLoading());
    $("#opt-visitorfields").html("");

    var params = 'idfield=' + idfield;

    if (params != '') params += '&timestamp=' + new Date().getTime();
    else params = 'timestamp=' + new Date().getTime();

    var jqxhr = callRoboticsWS_GET(jsonEngineURI + "downvisitorfield", params, function (data) {
        if (!hasError(data)) {
            data.d.fields.sort(function (a, b) { return a.position > b.position; });
            parseOptVisitorFields(data.d);
        } else {
            $("#opt-visitorfields-response").html(showErrorClose({ text: data.d.result }));
        }
    }, function () {
        $("#opt-visitorfields-response").html(showErrorClose({ text: $.t("unknownerror", "Error, aviste a soporte.") }));
    });
}
function delVisitorFieldsOpt(idfield) {
    $("#opt-visitorfields").html("");
    $("#opt-visitorfields-response").html(showLoading());
    var params = 'idfield=' + idfield;

    if (params != '') params += '&timestamp=' + new Date().getTime();
    else params = 'timestamp=' + new Date().getTime();

    var jqxhr = callRoboticsWS_GET(jsonEngineURI + "delvisitorfield", params, function (data) {
        if (!hasError(data)) {
            data.d.fields.sort(function (a, b) { return a.position > b.position; });
            parseOptVisitorFields(data.d);
        } else {
            $("#opt-visitorfields-response").html(showErrorClose({ text: data.result }));
        }
    }, function () {
        $("#opt-visitorfields-response").html(showErrorClose({ text: $.t("unknownerror", "Error, aviste a soporte.") }));
    });
}

function showVisitorField(idvisitorfield) {
    getVisitorField(idvisitorfield);
    openModal('#mod-visitorfield');
}

function showVisitType(idvisittype) {
    getVisitType(idvisittype);
    openModal('#mod-visittype');
}

function getVisitType(idvisittype) {
    $(".mod-visittype-row, #mod-visittype-buttons").addClass("hide");
    $("#mod-visittype-response").html(showLoading());
    var params = 'idtype=' + idvisittype;

    if (params != '') params += '&timestamp=' + new Date().getTime();
    else params = 'timestamp=' + new Date().getTime();

    var jqxhr = callRoboticsWS_GET(jsonEngineURI + "visittype", params, function (data) {
        if (!hasError(data)) {
            parseVisitType(data.d);
        } else {
            $("#mod-visittype-response").html(showError({ text: data.d.result }));
        }
    }, function () {
        $("#mod-visittype-response").html(showError({ text: $.t("unknownerror", "Error, aviste a soporte.") }));
    });
}
function parseVisitType(data) {
    //Eliminamos los errores anteriores
    $("#mod-visittype .alert-input").remove();
    $("#mod-visittype-idtype").val(data.idtype);
    $("#mod-visittype-name").val(data.name);
    $("#mod-visittype-edit").prop("checked", data.edit);
    $(".mod-visittype-row, #mod-visittype-buttons").removeClass("hide");
    $("#mod-visittype-response").html("");
}
function delVisitTypesOpt(idtype) {
    if ($("#opt-visittypes > table").length > 0) {
        var table = $("#opt-visittypes-table").DataTable();
        table.destroy();
    }
    $("#opt-visittypes-response").html(showLoading());
    $("#opt-visittypes").html("");

    var params = 'idtype=' + idtype;

    if (params != '') params += '&timestamp=' + new Date().getTime();
    else params = 'timestamp=' + new Date().getTime();

    var jqxhr = callRoboticsWS_GET(jsonEngineURI + "delvisittype", params, function (data) {
        if (!hasError(data)) {
            parseOptVisitTypes(data.d);
            localStorage.setItem("currVisitFilter", "");
        } else {
            $("#opt-visittypes-response").html(showErrorClose({ text: data.d.result }));
        }
    }, function () {
        $("#opt-visittypes-response").html(showErrorClose({ text: $.t("unknownerror", "Error, aviste a soporte.") }));
    });
}
function getVisitorField(idvisitfield) {
    $(".mod-visitorfield-row, #mod-visitorfield-buttons").addClass("hide");
    $("#mod-visitorfield-response").html(showLoading());
    var params = 'idfield=' + idvisitfield;

    if (params != '') params += '&timestamp=' + new Date().getTime();
    else params = 'timestamp=' + new Date().getTime();

    var jqxhr = callRoboticsWS_GET(jsonEngineURI + "visitorfield", params, function (data) {
        if (!hasError(data)) {
            parseVisitorField(data.d);
        } else {
            $("#mod-visitorfield-response").html(showError({ text: data.d.result }));
        }
    }, function () {
        $("#mod-visitorfield-response").html(showError({ text: $.t("unknownerror", "Error, aviste a soporte.") }));
    });
}
function parseVisitorField(data) {
    //Eliminamos los errores anteriores
    $("#mod-visitorfield .alert-input").remove();
    $("#mod-visitorfield-idfield").val(data.idfield);
    $("#mod-visitorfield-name").val(data.name);
    $("#mod-visitorfield-visible").prop("checked", data.visible);
    $("#mod-visitorfield-required").val(data.required);
    $("#mod-visitorfield-askevery").val(data.askevery);
    $("#mod-visitorfield-values").val(data.values);
    $("#mod-visitorfield-edit").prop("checked", data.edit);

    $(".mod-visitorfield-row, #mod-visitorfield-buttons").removeClass("hide");
    if (data.values.length > 0) {
        $("#mod-visitorfield-type-d").off('click');
        $("#mod-visitorfield-type-d").click();
        $("#mod-visitorfield-valuesdiv").removeClass("hide");
    } else {
        $("#mod-visitorfield-type-t").off('click');
        $("#mod-visitorfield-type-t").click();
        $("#mod-visitorfield-valuesdiv").addClass("hide");
    }
    if (data.required < 3) {
        $(".mod-visitorfield-row-required").addClass("hide");
    } else {
        $(".mod-visitorfield-row-required").removeClass("hide");
    }

    $("#mod-visitorfield-response").html("");
}
function checkVisitorField() {
    $("#mod-visitorfield .alert-input").remove();
    if ($("#mod-visitorfield-name").val() == "") {
        $("#mod-visitorfield-name").after('<div class="alert-input">Ha de indicar un nombre</div>');
    }
    if ($("#mod-visitorfield-required").val() == 3) {
        if ($("#mod-visitorfield-askevery").val() == 0) {
            $("#mod-visitorfield-askevery").after('<div class="alert-input">Ha de indicar cuando se ha de informar si el campo es requerido</div>');
        }
    }

    return $("#mod-visitorfield .alert-input").length == 0;
}
function saveVisitorField() {
    var fld = { idfield: 0, name: "", required: 0, askevery: 0, values: "", edit: 0 };
    fld.idfield = $("#mod-visitorfield-idfield").val();
    fld.name = $("#mod-visitorfield-name").val();
    fld.visible = $("#mod-visitorfield-visible").prop("checked");
    fld.required = $("#mod-visitorfield-required").val();
    fld.askevery = $("#mod-visitorfield-askevery").val();
    fld.values = $("#mod-visitorfield-values").val();
    fld.edit = $("#mod-visitorfield-edit").prop("checked");
    if (checkVisitorField()) { putVisitorField(fld); }
}
function putVisitorField(values) {
    $(".mod-visitorfield-row, #mod-visitorfield-buttons").addClass("hide");
    $("#mod-visitorfield-response").html(showLoading());
    var params = 'values=' + encodeURIComponent(JSON.stringify(values));

    if (params != '') params += '&timestamp=' + new Date().getTime();
    else params = 'timestamp=' + new Date().getTime();

    var jqxhr = callRoboticsWS_GET(jsonEngineURI + "visitorfield", params, function (data) {
        if (!hasError(data)) {
            closeVisitorFieldOnSave();
        } else {
            $("#mod-visitorfield-response").html(showError({ text: data.d.result }));
        }
    }, function () {
        $("#mod-visitorfield-response").html(showError({ text: $.t("unknownerror", "Error, aviste a soporte.") }));
    });
}
function closeVisitorFieldOnSave() {
    $("#mod-visitorfield-response").html(showSuccess({ text: $.t("saved", "Datos almacenados correctamente.") }));

    getVisitorFieldsOpt();
    _.delay(function () {
        closeModal('#mod-visitorfield');
    }, 1500);
}

/************             Visitantes             ****************/
/****************************************************************/

/****************************************************************/
/************             Empleados             ****************/

function showEmployeeList() {
    getEmployeeList("");
    $('#mod-employeelist').foundation('reveal', 'open');
}
function getEmployeeList(filter) {
    if ($("#mod-employeelist > table").length > 0) {
        var table = $("#visitorslist-table").DataTable();
        table.destroy();
    }
    $("#mod-employeelist-content").html("");

    $("#mod-employeelist-response").html(showLoading());
    var params = 'filter=' + encodeURIComponent(filter)

    if (params != '') params += '&timestamp=' + new Date().getTime();
    else params = 'timestamp=' + new Date().getTime();

    var jqxhr = callRoboticsWS_GET(jsonEngineURI + "employeelist", params, function (data) {
        if (!hasError(data)) {
            parseEmployeeList(data.d);
        } else {
            $("#mod-employeelist-response").html(showErrorClose({ text: data.result }));
        }
    }, function () {
        $("#mod-employeelist-response").html(showErrorClose({ text: data.result }));
    });
}
function parseEmployeeList(data) {
    var cols = "<th width='60px'>&nbsp;</th>";
    cols += "<th>Nombre</th>";
    cols += "<th>Departamento</th>";

    var tmp = '<table id="mod-employeelist-table" class="display compact" cellspacing="0" width="100%">';
    tmp += '<thead>';
    tmp += '<tr>' + cols + '</tr>';
    tmp += '</thead>';
    //tmp += '<tfoot>';
    //tmp += '<tr>' + cols + '</tr>';
    //tmp += '</tfoot>';
    tmp += '<tbody>';

    $(data.employees).each(function (index) {
        var emp = this;
        tmp += '<tr>'
        tmp += '<td data-order="' + emp.idvisitor + '"><input name="mod-employeelist-idemployee" data-idemployee="' + emp.idemployee + '" data-employeename="' + emp.name + '" type="radio">';
        tmp += '</td>'
        tmp += "   <td>" + emp.name + "</td>"
        tmp += "   <td>" + emp.groupname + "</td>"
        tmp += "</tr>"
    });
    tmp += '</tbody></table>';
    if ($("#mod-employeelist > table").length > 0) {
        var table = $("#mod-employeelist-table").DataTable();
        table.destroy();
    }
    $("#mod-employeelist-response").html("");
    $("#mod-employeelist-content").html(tmp);
    _tblVisitorsList = $("#mod-employeelist-table").DataTable(datatablesOptOrd);
    $("#mod-employeelist-table_wrapper > .row").addClass("hide-on-print")
}
function addEmployee2Visit() {
    var itm = "";
    $("#mod-employeelist-add").remove();
    $("#mod-employeelist [data-idemployee]:checked").each(function (index) {
        itm = '<div class="mod-visit-employee-item" data-idemployee="' + $(this).data("idemployee") + '"><span class="employeestatus-' + $(this).data("idemployee") + '"></span>' + $(this).data("employeename");
        itm += '  <a onclick="delEmployee2Visit(this)"><i class="fa fa-times"></i></a>';
        itm += '</div>'
        $("#mod-visit-employee").html(itm);
        getEmployeeStatus($(this).data("idemployee"));
    });
    closeModal("#mod-employeelist");
}
function delEmployee2Visit(elem) {
    $(elem).parent().remove();
    if ($(".mod-visit-employee-item").length == 0 && $("#mod-visit-status").val() == 0) {
        $("#mod-visit-employee").html('<a onclick="showEmployeeList()"><div class="add"><i class="fa fa-user-plus fa-lg"></i> ' + $.t("addemployee", "Añadir empleado") + '</div></a>');
    }
}

function getEmployeeStatus(idemployee) {
    var params = 'idemployee=' + idemployee;

    if (params != '') params += '&timestamp=' + new Date().getTime();
    else params = 'timestamp=' + new Date().getTime();

    var jqxhr = callRoboticsWS_GET(jsonEngineURI + "employeestatus", params, function (data) {
        if (!hasError(data)) {
            if (data.d.value == 1) {
                $(".employeestatus-" + idemployee).html("&nbsp;<i class='fa fa-sign-in' style='color:green'></i>");
            }
            else if (data.d.value == 0) {
                $(".employeestatus-" + idemployee).html("&nbsp;<i class='fa fa-sign-out' style='color:red'></i>");
            }
            else if (data.d.value == 3) {
                $(".employeestatus-" + idemployee).html("&nbsp;<i class='fa-stop-circle-o' style='color:red'></i>");
            }
        }
    });
}

/************             Empleados             ****************/
/****************************************************************/

/****************************************************************/
/************             Opciones               ****************/

function updateCleanVisitorsData() {
    $("#fastsearchfield").html(showLoading());

    var params = 'timestamp=' + new Date().getTime();

    var jqxhr = callRoboticsWS_GET(jsonEngineURI + "zoneListByWorkingOut", params, function (data) {
        if (!hasError(data)) {
            /* rellenamos campo zona*/
            var opts2 = "<option value='-1' data-i18n='notapply'>---</option>"
            _.each(data.d.zoneList, function (element, index, list) {
                opts2 += '<option value="' + element.zoneId + '"';
                if (element.zoneId == _currOptions.zoneValueField) { opts2 += " selected "; }
                opts2 += '>' + element.zoneName + '</option>';
            });
            $("#opt-general-zonefield").html(opts2);
        } else {
            $("#opt-visitfields-response").html(showErrorClose({ text: data.d.result }));
        }
    }, function () {
        $("#opt-visitfields-response").html(showErrorClose({ text: $.t("unknownerror", "Error, aviste a soporte.") }));
    });
}

function updateZoneList() {
    $("#fastsearchfield").html(showLoading());

    var params = 'timestamp=' + new Date().getTime();

    var jqxhr = callRoboticsWS_GET(jsonEngineURI + "zoneListByWorkingOut", params, function (data) {
        if (!hasError(data)) {
            /* rellenamos campo zona*/
            var opts2 = "<option value='-1' data-i18n='notapply'>---</option>"
            _.each(data.d.zoneList, function (element, index, list) {
                opts2 += '<option value="' + element.zoneId + '"';
                if (element.zoneId == _currOptions.zoneValueField) { opts2 += " selected "; }
                opts2 += '>' + element.zoneName + '</option>';
            });
            $("#opt-general-zonefield").html(opts2);
        } else {
            $("#opt-visitfields-response").html(showErrorClose({ text: data.d.result }));
        }
    }, function () {
        $("#opt-visitfields-response").html(showErrorClose({ text: $.t("unknownerror", "Error, aviste a soporte.") }));
    });
}

function Imprimir(que) {
    //PrintObject(document.getElementById(que).innerHTML, 'css/src/foundation-5.5.3.min.css');
    PrintObject(document.getElementById(que).innerHTML, 'css/src/visits.min.css');
}
function PrintObject(_HTML, _CSSPath1, _CSSPath2) {
    var ventana = window.open("", "", "");
    var contenido = "<html>";
    contenido = contenido + "<head runat='server'>";

    if (_CSSPath1 != null && _CSSPath1 != '') {
        contenido = contenido + "<link href='" + _CSSPath1 + "' type='text/css' rel='stylesheet'/>";
    }
    if (_CSSPath2 != null && _CSSPath2 != '') {
        contenido = contenido + "<link href='" + _CSSPath2 + "' type='text/css' rel='stylesheet'/>";
    }
    contenido = contenido + "<style type='text/css'>.print-background {-webkit-print-color-adjust: exact!important;}</style>";
    contenido = contenido + "</head>";
    //contenido = contenido + "<body onload='window.print();window.close();'>";
    contenido = contenido + "<body >";
    contenido = contenido + _HTML + "</body></html>";
    ventana.document.open();
    ventana.document.write(contenido);
    ventana.document.close();
}

function updateFieldsList() {
    $("#fastsearchfield").html(showLoading());

    var params = 'timestamp=' + new Date().getTime();

    var jqxhr = callRoboticsWS_GET(jsonEngineURI + "visitfieldslist", params, function (data) {
        if (!hasError(data)) {
            _currVisitFields = data.d.fields;
            localStorage.setItem("currVisitFields", JSON.stringify(_currVisitFields));

            /* rellenamos busqueda rapida*/
            var opts = "";
            var optsSel = "<option value=''>---</option>";
            var optsTxt = "<option value=''>---</option>";
            //data.d.fields.sort(function (a, b) { return a.name > b.name; });
            _.each(data.d.fields, function (element, index, list) {
                opts += '<option value="' + element.idfield + '"';
                if (element.name == _currOptions.cardfield) { opts += " selected "; }
                opts += '>' + element.name + '</option>';
                if (element.values == "") {
                    optsTxt += '<option value="' + element.idfield + '">' + element.name + '</option>';
                } else {
                    optsSel += '<option value="' + element.idfield + '">' + element.name + '</option>';
                }
            });
            $("#opt-general-searchfield").html(opts);
            $("#opt-general-visitUniqueIDField").html(optsTxt);
            $("#opt-general-multilocationField").html(optsSel);
            /* rellenamos campo tarjeta*/
            var opts2 = "<option value='-1' data-i18n='notapply'>---</option>"
            _.each(data.d.fields, function (element, index, list) {
                opts2 += '<option value="' + element.name + '"';
                if (element.name == _currOptions.cardNumberField) { opts2 += " selected "; }
                opts2 += '>' + element.name + '</option>';
            });
            $("#opt-general-cardfield").html(opts2);
            /* rellenamos dias para eliminar datos de los visitantes*/
            var opts3 = ""
            var days = ["30", "60", "90", "120"];
            _.each(days, function (element, index, list) {
                opts3 += '<option value="' + element + '"';
                if (element == _currOptions.deleteDataVisitorsField) { opts3 += " selected "; }
                opts3 += '>' + element + '</option>';
            });
            $("#opt-general-cleanVisitorData").html(opts3);
        } else {
            $("#opt-visitfields-response").html(showErrorClose({ text: data.d.result }));
        }
    }, function () {
        $("#opt-visitfields-response").html(showErrorClose({ text: $.t("unknownerror", "Error, aviste a soporte.") }));
    });
}

function updateVisitorFieldList() {
    var params = 'timestamp=' + new Date().getTime();
    var jqxhr = callRoboticsWS_GET(jsonEngineURI + "visitorfieldslist", params, function (data) {
        if (!hasError(data)) {
            /* rellenamos campos para DNI*/
            var opts4 = "<option value='-1' data-i18n='notapply'>---</option>"
            var opts5 = "<option value='' data-i18n='notapply'>---</option>"
            _.each(data.d.fields, function (element, index, list) {
                opts4 += '<option value="' + element.name + '"';
                if (element.name == _currOptions.documentNumberfield) { opts4 += " selected "; }
                opts4 += '>' + element.name + '</option>';

                opts5 += '<option value="' + element.idfield + '"';
                if (element.name == _currOptions.documentNumberfield) { opts5 += " selected "; }
                opts5 += '>' + element.name + '</option>';
            });
            $("#opt-general-visitorIdentificationNumberField").html(opts4);
            $("#opt-general-visitorUniqueIDField").html(opts5);
        } else {
            $("#opt-visitorfields-response").html(showErrorClose({ text: data.d.result }));
        }
    }, function () {
        $("#opt-visitorfields-response").html(showErrorClose({ text: $.t("unknownerror", "Error, aviste a soporte.") }));
    });
}

function getOptions(bIsInitializingApp) {
    $("#opt-general-response").html(showLoading());
    var params = 'timestamp=' + new Date().getTime();

    var bNeetToInitialize = typeof bIsInitializingApp == "undefined" ? false : bIsInitializingApp;

    var jqxhr = callRoboticsWS_GET(jsonEngineURI + "options", params, function (data) {
        if (!hasError(data)) {
            _currOptions = data.d;
            parseOptions(data.d);

            if (bNeetToInitialize) initilizeApp();
        } else {
            $("#opt-general-response").html(showError({ text: data.d.result }));
        }
    }, function () {
        $("#opt-general-response").html(showError({ text: $.t("unknownerror", "Error, aviste a soporte.") }));
    });
}
function parseOptions(data) {
    var tmp = "";

    //rellanamos datos
    $("#opt-general-searchfield").val(data.cardfield);
    $("#opt-general-cardfield").val(data.cardNumberField);
    $("#opt-general-notification").val(data.notification);
    $("#opt-general-zonefield").val(data.zoneValueField);
    $("#opt-general-allowvisitfieldmodify").val(data.allowvisitfieldmodify);
    $("#opt-general-cleanVisitorData").val(data.deleteDataVisitorsField);
    $("#opt-general-visitorIdentificationNumberField").val(data.documentNumberfield);
    $("#opt-general-scannerserviceport").val(data.scanServicePort);
    $("#opt-general-response").html("");
    $("#opt-general-multilocationField").val(data.multilocationField);
    $("#opt-general-visitorUniqueIDField").val(data.visitorUniqueIDField);
    $("#opt-general-visitUniqueIDField").val(data.visitUniqueIDField);

    if (_.isString(_currOptions.cardNumberField) && _currOptions.cardfield != null && _currOptions.cardfield.length > 0) {
        $("#plani-fastopen").show();
        var fld = _currVisitFields.filter(function (itm) { return itm.idfield == _currOptions.cardfield });
        if (fld.length > 0) {
            $("#plani-fastopen-field").html(fld[0].name);
        }
    } else {
        $("#plani-fastopen").hide();
    }
    if (_.isString(_currOptions.multilocationField) && _currOptions.multilocationField != null && _currOptions.multilocationField.length > 0) {
        var fld = _currVisitFields.filter(function (itm) { return itm.idfield == _currOptions.multilocationField });
        if (fld.length > 0) {
            opt = "<option value=''>---</option>";
            if (fld[0].values.length > 0) {
                fld[0].values.split(";").forEach(function (itm) {
                    opt += "<option value='" + itm + "'>" + itm + "</option>"
                })
            }
            $("#plani-Location-selector").html(opt);
        }
        $("#plani-location").show();
    } else {
        $("#plani-location").hide();
    }
}
function saveOptions() {
    var opt = { cardfield: 0, notification: "", cardnumberfield: "-1", allowvisitfieldmodify: "0" };

    opt.cardfield = $("#opt-general-searchfield").val();
    opt.notification = $("#opt-general-notification").val();
    opt.cardnumberfield = $("#opt-general-cardfield").val();
    opt.zoneValueField = $("#opt-general-zonefield").val();
    opt.allowvisitfieldmodify = $("#opt-general-allowvisitfieldmodify").val();
    opt.deleteDataVisitorsField = $("#opt-general-cleanVisitorData").val();
    opt.documentNumberfield = $("#opt-general-visitorIdentificationNumberField").val();
    opt.scanServicePort = $("#opt-general-scannerserviceport").val();
    opt.multilocationField = $("#opt-general-multilocationField").val();
    opt.visitUniqueIDField = $("#opt-general-visitUniqueIDField").val();
    opt.visitorUniqueIDField = $("#opt-general-visitorUniqueIDField").val();
    putOptions(opt);
}
function putOptions(values) {
    $("#opt-general-response").html(showLoading());
    var params = 'values=' + encodeURIComponent(JSON.stringify(values));

    if (params != '') params += '&timestamp=' + new Date().getTime();
    else params = 'timestamp=' + new Date().getTime();

    var jqxhr = callRoboticsWS_GET(jsonEngineURI + "options", params, function (data) {
        if (!hasError(data)) {
            getOptions();
            $("#opt-general-response").html(showSuccess({ text: $.t("saved", "Datos almacenados correctamente.") })).delay(3000).slideUp();
        } else {
            $("#opt-general-response").html(showError({ text: data.d.result }));
        }
    }, function () {
        $("#opt-general-response").html(showError({ text: $.t("unknownerror", "Error, aviste a soporte.") }));
    });
}

/************             Opciones               ****************/
/****************************************************************/

function searchfastfield() {
    if ($("#plani-fastopen-value").val().length > 0) {
        var filter = { "status": "1" };
        filter[_currOptions.cardfieldid] = $("#plani-fastopen-value").val();
        var params = 'timestamp=' + new Date().getTime();
        if (_.isObject(filter) && filter != "") { params += '&filter=' + encodeURIComponent(JSON.stringify(filter)); }
        $("#plani-fastopen-value").val('');

        var jqxhr = callRoboticsWS_GET(jsonEngineURI + "visitlist", params, function (data) {
            if (!hasError(data)) {
                if (data.d.visits.length > 0) {
                    data.d.visits.sort(function (a, b) { return moment(a.begindate).isBefore(b.begindate); });
                    showVisit(data.d.visits[0].idvisit);
                } else {
                    showErrorMsg("#plani-fastopen-response", $.t("notfound", "No se han encontrado visitas activas con estos datos."));
                    //$().html(showErrorClose({ text:  }));
                }
            } else {
                $("#plani-fastopen-response").html(showErrorClose({ text: data.result }));
            }
        }, function () {
            $("#plani-fastopen-response").html(showErrorClose({ text: $.t("unknownerror", "Error, aviste a soporte.") }));
        });
    }
}

function showQuestion(title, question, func, params) {
    $("#mod-question-title").html(title);
    $("#mod-question-question").html(question);
    $("#mod-question-buttons-yes").off('click');
    $("#mod-question-buttons-yes").click(function () { func(params); closeModal('#mod-question'); });
    openModal('#mod-question');
}
function showErrorMsg(obj, msg) {
    $(obj).html(showErrorClose({ text: msg }));
    $(obj + ">div").delay(1500).slideUp("fast", function () { this.remove(); });
}
function showSuccessMsg(obj, msg) {
    $(obj).html(showSuccess({ text: msg }));
    $(obj + ">div").delay(1500).slideUp("fast", function () { this.remove(); });
}

var optOnlyDate = {};
$(document).ready(function () {
    //jsonEngineURI = "http://roboticstest22.cloudapp.net/visits/api/1/json.svc/";
    //jsonEngineURI = "api/1/json.svc/";

    i18n.init({ shortcutFunction: 'defaultValue', resGetPath: 'locales/__ns__.__lng__.json' }, function (t) {
        $(document).i18n();
    });

    //var i18v = ""; $("[data-i18n]").each(function (index) { i18v+= ('"' + $(this).attr("data-i18n")+'" : "' + $(this).html() +'",\n') })
    //Recupera la variable de sessi�n si existe
    //TODO: ROBOTICS
    if (_.isString(localStorage.usr)) {
        if (_usr.tku == "0000") {
            try {
                _usr = JSON.parse(localStorage.usr)
                roChecklogin();
            } catch (ex) {
            }
        } else {
            roChecklogin();
        }
    } else {
        var tku = $.cookie("tku");
        if (_.isString(tku)) {
            _usr.tku = tku;
            roChecklogin();
        } else {
            rologout(true)
        }
    }
    if (_.isString(localStorage.consent)) {
        if (localStorage.consent == "true") {
        }
        else {
            openModal('#mod-consent');
            $("#mod-consent-name").html(JSON.parse(localStorage.consentMessage));
            //$("#mod-consent-edit").checked(false);
            $("#mod-consent-buttons-save").hide();
        }
    }
    //changeLocale(_usr.lang);
    //checkPermisions();

    if (!(_.isString(_usr.lang))) {
        _usr.lang = "es";
    }

    $("#tab-plani-time-value").html(moment().format("HH:mm:ss"));
    setInterval(updateClock, 1000);

    Foundation.global.namespace = '';
    $(document).foundation({
        reveal: {
            multiple_opened: true
        }
    });

    parseVisitList({ visits: [] }, true);

    //Cargamos variables de sesión
    if (_.isString(localStorage.currVisitSearchValues) && localStorage.currVisitSearchValues != '') {
        _currVisitSearchValues = JSON.parse(localStorage.currVisitSearchValues);
    }

    if (_.isString(localStorage.currVisitFilter) && localStorage.currVisitFilter != '') {
        _currVisitFilter = JSON.parse(localStorage.currVisitFilter);
    }
    if (_.isString(localStorage.currVisitorFilter) && localStorage.currVisitorFilter != '') {
        _currVisitorFilter = JSON.parse(localStorage.currVisitorFilter);
    }
    if (_.isString(localStorage.currVisitFilterList) && localStorage.currVisitFilterList != '') {
        _currVisitFilterList = JSON.parse(localStorage.currVisitFilterList);
    }
    if (_.isString(localStorage.currVisitFields) && localStorage.currVisitFields != '') {
        _currVisitFields = JSON.parse(localStorage.currVisitFields);
    }
    if (!_.isDate($("#tab-plani-date-input").val())) {
        $("#tab-plani-date-input").val(moment().format('DD/MM/YYYY'))
        _currVisitFilterList.date = moment().toISOString();
    }

    if (typeof _currVisitFilter.begindate == 'undefined') {
        _currVisitFilter.begindate = moment().startOf('week').format('DD/MM/YYYY');
        localStorage.currVisitFilter = JSON.stringify(_currVisitFilter);
    }

    if (typeof _currVisitFilter.enddate == 'undefined') {
        _currVisitFilter.enddate = '';
        localStorage.currVisitFilter = JSON.stringify(_currVisitFilter);
    }

    loadVisitsFilter();

    if (typeof _currVisitFilterList.begindate == 'undefined') {
        _currVisitFilterList.begindate = moment().startOf('week').format('DD/MM/YYYY');
        localStorage.currVisitFilterList = JSON.stringify(_currVisitFilterList);
    }

    if (typeof _currVisitFilterList.enddate == 'undefined') {
        _currVisitFilterList.enddate = '';
        localStorage.currVisitFilterList = JSON.stringify(_currVisitFilterList);
    }

    //Cargamos el dia de hoy por defecto
    _currVisitFilterList.date = moment().toISOString();
    $("#tab-plani-date-value").html(moment().format('LL'));

    $('#plani-Location-selector').change(function (e) {
        e.preventDefault();
        _currVisitFilterList.location = $('#plani-Location-selector').val();
        getVisitList(_currVisitFilterList, true);
    });

    $("#mod-visitorfield-required").change(function () {
        if ($(this).val() < 3) {
            $(".mod-visitorfield-row-required").addClass("hide");
            $("#mod-visitorfield-askevery").val(0);
        } else {
            $(".mod-visitorfield-row-required").removeClass("hide");
        }
    });

    $("#chkWhen0, #chkWhen1").change(function () {
        if (this.id == "chkWhen0") {
            $(".visit-period").addClass("hide");
        } else {
            $(".visit-period").removeClass("hide");
            $("#mod-visit-enddate").val("");
        }
    });
    $("#newVisits").off('click');
    $("#newVisits").click(function () {
        parseVisitList(_currVisitList, false);
    });
    $(".togglenext").off('click');
    $(".togglenext").click(function () {
        $(this).next().slideToggle("fast");
    });
    $("#VisitListSearchClean").off('click');
    $("#VisitListSearchClean").click(function () {
        $("#plani-search-text").val("");
        _currVisitSearchValues = { where: "", value: "", orderBy: "hour" };
        _currVisitSearchList = {};
        window.location.hash = "";
        $("#plani-search-selector").val("visits");
        $("#plani-orderBy-selector").val("hour");

        localStorage.setItem("currVisitSearchValues", JSON.stringify(_currVisitSearchValues));

        getVisitList(_currVisitFilterList, true);
    });

    $("#plani-orderBy-selector").off('change');
    $("#plani-orderBy-selector").on('change', function () {
        _currVisitSearchValues.where = $("#plani-search-selector").val();
        _currVisitSearchValues.value = $("#plani-search-text").val();
        _currVisitSearchValues.orderBy = $("#plani-orderBy-selector").val();

        localStorage.setItem("currVisitSearchValues", JSON.stringify(_currVisitSearchValues));

        getVisitList(_currVisitFilterList, true);

        if (window.location.hash.indexOf('search') != -1) {
            getVisitListSearch(_currVisitSearchValues);
        }
    });

    $("#VisitListSearch").off('click');
    $("#VisitListSearch").click(function () {
        if ($("#plani-search-text").val().length > 0) {
            _visitorsSearchCache = null;
            _visitorsSearchFilter = "";

            _currVisitSearchValues.where = $("#plani-search-selector").val();
            _currVisitSearchValues.value = $("#plani-search-text").val();
            _currVisitSearchValues.orderBy = $("#plani-orderBy-selector").val();

            var newHash = "#/search/" + _currVisitSearchValues.where + "/" + _currVisitSearchValues.value + "/" + _currVisitSearchValues.orderBy;

            if (window.location.hash.indexOf('search') == -1) {
                window.location.hash = "/search/" + _currVisitSearchValues.where + "/" + _currVisitSearchValues.value;
            } else {
                getVisitListSearch(_currVisitSearchValues);
            }

            localStorage.setItem("currVisitSearchValues", JSON.stringify(_currVisitSearchValues));
        }
    });
    var timeLogo
    $("#logo").off('click');
    $("#logo").click(function () {
        clearTimeout(timeLogo);
        //$("#mainmenu").stop();
        $("#mainmenu").slideToggle("fast");
    });
    $("#logo, #mainmenu").mouseenter(function () {
        clearTimeout(timeLogo);
        //$("#mainmenu").stop();
        $("#mainmenu").slideDown("fast");
    });
    $("#logo, #mainmenu").mouseleave(function () {
        timeLogo = setTimeout(function () { $("#mainmenu").slideUp(); }, 1000);
        //$("#mainmenu").delay(2000).slideUp();
    });

    $("#tab-gest .datepicker").datetimepicker({ lang: _usr.lang, mask: false, format: 'd/m/Y', dayOfWeekStart: 1, closeOnDateSelect: true, timepicker: false });

    $("div>[id^='visit-repeattime']").hide();
    $("#visit-repeattype").off("change");
    $("#visit-repeattype").on("change", function () {
        $("div>[id^='visit-repeattime']").hide();
        $("#visit-repeattime-" + $("#visit-repeattype").val()).show();
    })
    $("#versionlabel").html(getVersion());
    $('#plani-fastopen-value').keypress(function (e) {
        if (e.keyCode == 13)
            searchfastfield();
    });
    $("[name='mod-visitfield-type']").off('click');
    $("[name='mod-visitfield-type']").click(function () {
        if ($("[name='mod-visitfield-type']:checked").val() == "d") {
            $("#mod-visitfield-valuesdiv").removeClass("hide");
        } else {
            $("#mod-visitfield-valuesdiv").addClass("hide");
        }
    });
    $("#mod-visitfield-valuesdiv").addClass("hide");
    $("[name='mod-visitorfield-type']").off('click');
    $("[name='mod-visitorfield-type']").click(function () {
        if ($("[name='mod-visitorfield-type']:checked").val() == "d") {
            $("#mod-visitorfield-valuesdiv").removeClass("hide");
        } else {
            $("#mod-visitorfield-valuesdiv").addClass("hide");
        }
    });
    $("#mod-visitorfield-valuesdiv").addClass("hide");

    $('<button id="refresh">Refresh</button>').appendTo('#gest-visits.dataTables_filter');

    dxComponentsInit();

    initGUI();
    refreshReset();

    getOptions(true);
});

function initilizeApp() {
    updateFieldsList();
    updateZoneList();
    getVisitorList({ _loadpunches_: false }, true);

    getVisitList(_currVisitFilterList, true);

    routie({
        '/plani': function () {
            routie('/plani/scheduled');
        },
        '/plani/:subtab': function (subtab) {
            activeTab("plani", subtab);
        },
        '/plani/:subtab/visit/:idvisit': function (subtab, idvisit) {
            executeTask("visit", "edit", idvisit);
            routie('/plani/' + subtab);
        },
        '/plani/:subtab/*': function (subtab) {
            //TODO: pendiente de parsear
            activeTab("plani", subtab);
        },
        '/gest': function () {
            activeTab("gest", "visits")
        },
        '/gest/:subtab': function (subtab) {
            activeTab("gest", subtab)
        },
        '/gest/visits/edit/:idvisit': function (idvisit) {
            executeTask("visit", "edit", idvisit);
            routie('/gest/visits');
        },
        '/gest/visits/del/:idvisit': function (idvisit) {
            executeTask("visit", "del", idvisit);
            routie('/gest/visits');
        },
        '/gest/visits/:idvisit': function (idvisit) {
            executeTask("visit", "edit", idvisit);
            routie('/gest/visits');
        },
        '/gest/visitors/edit/:idvisitor': function (idvisitor) {
            executeTask("visitor", "edit", idvisitor);
            routie('/gest/visitors');
        },
        '/gest/visitors/del/:idvisitor': function (idvisitor) {
            executeTask("visitor", "del", idvisitor);
            routie('/gest/visitors');
        },
        '/gest/visitors/new': function () {
            executeTask("visitor", "new", "");
            routie('/gest/visitors');
        },
        '/gest/visitors/:idvisitor': function (idvisitor) {
            executeTask("visitor", "edit", idvisitor);
            routie('/gest/visitors');
        },
        '/opt': function (subtab) {
            routie('/opt/visitfields');
        },
        '/opt/:subtab': function (subtab) {
            activeTab("opt", subtab)
        },
        '/opt/visittypes/edit/:idvisittype': function (idvisittype) {
            executeTask("visittype", "edit", idvisittype);
            routie('/opt/visittypes');
        },
        '/opt/visittypes/del/:idvisittype': function (idvisittype) {
            executeTask("visittype", "del", idvisittype);
            routie('/opt/visittypes');
        },
        '/opt/visittypes/:idvisittype': function (idvisittype) {
            executeTask("visittype", "edit", idvisittype);
            routie('/opt/visittypes');
        },
        '/opt/visitfields/edit/:idvisitfield': function (idvisitfield) {
            executeTask("visitfield", "edit", idvisitfield);
            routie('/opt/visitfields');
        },
        '/opt/visitfields/del/:idvisitfield': function (idvisitfield) {
            executeTask("visitfield", "del", idvisitfield);
            routie('/opt/visitfields');
        },
        '/opt/visitfields/:idvisitfield': function (idvisitfield) {
            executeTask("visitfield", "edit", idvisitfield);
            routie('/opt/visitfields');
        },
        '/opt/visitorfields/edit/:idvisitorfield': function (idvisitorfield) {
            executeTask("visitorfield", "edit", idvisitorfield);
            routie('/opt/visitorfields');
        },
        '/opt/visitorfields/del/:idvisitorfield': function (idvisitorfield) {
            executeTask("visitorfield", "del", idvisitorfield);
            routie('/opt/visitorfields');
        },
        '/opt/visitorfields/:idvisitorfield': function (idvisitorfield) {
            executeTask("visitorfield", "edit", idvisitorfield);
            routie('/opt/visitorfields');
        },
        '/visit/:idvisit': function (idvisit) {
            executeTask("visit", "edit", idvisit);
            routie('/gest/visits');
        },
        '/visitor/:idvisitor': function (idvisitor) {
            executeTask("visitor", "edit", idvisitor);
            routie('/gest/visitors');
        },
        '/search/visit/:idvisit': function (idvisit) {
            executeTask("visit", "edit", idvisit);
            routie("/search/" + _currVisitSearchValues.where + "/" + _currVisitSearchValues.value);
        },
        '/search/:where/:value': function (where, value) {
            _currVisitSearchValues.where = where;
            _currVisitSearchValues.value = value;
            activeTab("plani", "search");
        },
        '*': function () {
            if (location.hash.length == 0) {
                if (_usr.haschange < 2) {
                    routie('/gest/visits');
                } else {
                    routie('/plani/scheduled');
                }
            }
            else if (location.hash[1] == '/') {
                //TODO: pendiente de parsear
            } else {
                routie('/' + location.hash.substring(1));
            }
            console.log(location.hash);
        }
    });
}

function dxComponentsInit() {
    labelseditor = $("#labels .html-editor").dxHtmlEditor({
        toolbar: {
            items: [
                "undo", "redo", "separator",
                {
                    formatName: "size",
                    formatValues: ["8pt", "10pt", "12pt", "14pt", "18pt", "24pt", "36pt"]
                },
                {
                    formatName: "font",
                    formatValues: ["Arial", "Courier New", "Georgia", "Impact", "Lucida Console", "Tahoma", "Times New Roman", "Verdana"]
                },
                "separator", "bold", "italic", "strike", "underline", "separator",
                "alignLeft", "alignCenter", "alignRight", "alignJustify", "separator",
                {
                    formatName: "header",
                    formatValues: [false, 1, 2, 3, 4, 5]
                }, "separator",
                "orderedList", "bulletList", "separator",
                "color", "background", "separator",
                "link", "image", "separator",
                "clear", "codeBlock", "blockquote",
                {
                    widget: "dxSelectBox",
                    options: {
                        items: dsFields,
                        placeholder: "Campos automáticos",
                        onValueChanged: function (args) {
                            labelseditor.insertText(labelsLastPos, "{" + args.value + "}");
                            //args.value = "";
                        }
                    },
                    locateInMenu: "never"
                }
            ]
        },
        onValueChanged: function (e) {
            var range = labelsquill.getSelection();
            setTimeout(function () {
                if (range) {
                    if (range.length == 0) {
                        labelsLastPos = range.index;
                    }
                }
            }, 1000);
        }
    }).dxHtmlEditor("instance");

    lawseditor = $("#laws .html-editor").dxHtmlEditor({
        toolbar: {
            items: [
                "undo", "redo", "separator",
                {
                    formatName: "size",
                    formatValues: ["8pt", "10pt", "12pt", "14pt", "18pt", "24pt", "36pt"]
                },
                {
                    formatName: "font",
                    formatValues: ["Arial", "Courier New", "Georgia", "Impact", "Lucida Console", "Tahoma", "Times New Roman", "Verdana"]
                },
                "separator", "bold", "italic", "strike", "underline", "separator",
                "alignLeft", "alignCenter", "alignRight", "alignJustify", "separator",
                {
                    formatName: "header",
                    formatValues: [false, 1, 2, 3, 4, 5]
                }, "separator",
                "image", "separator",
                "clear"

            ]
        }
    }).dxHtmlEditor("instance");;

    lawseditor2 = $("#laws2 .html-editor").dxHtmlEditor({
        toolbar: {
            items: [
                "undo", "redo", "separator",
                {
                    formatName: "size",
                    formatValues: ["8pt", "10pt", "12pt", "14pt", "18pt", "24pt", "36pt"]
                },
                {
                    formatName: "font",
                    formatValues: ["Arial", "Courier New", "Georgia", "Impact", "Lucida Console", "Tahoma", "Times New Roman", "Verdana"]
                },
                "separator", "bold", "italic", "strike", "underline", "separator",
                "alignLeft", "alignCenter", "alignRight", "alignJustify", "separator",
                {
                    formatName: "header",
                    formatValues: [false, 1, 2, 3, 4, 5]
                }, "separator",
                "image", "separator",
                "clear"

            ]
        }
    }).dxHtmlEditor("instance");;

    if (!labelsquill) {
        labelsquill = labelseditor.getQuillInstance();
        $("#labels .html-editor").on("click", function () {
            var range = labelsquill.getSelection();
            setTimeout(function () {
                if (range) {
                    if (range.length == 0) {
                        labelsLastPos = range.index;
                    }
                }
            }, 1000);
        });
    }
}

function changePassword() {
    if ($("#newpassword2").val() == $("#newpassword").val()) {
        rochangepassword($("#currentpass").val(), $("#newpassword").val());
    } else {
        showErrorMsg("#mod-changepassword-response", $.t("diffpasswored", "La contraseña y su validación son diferentes"));
        $("#mod-changepassword-buttons").removeClass("hide");
    }
}
function rochangepassword(oldpwd, newpwd) {
    $("#mod-changepassword-response").html(showLoading());
    //$("#mod-changepassword-buttons").addClass("hide");
    $("#newpassword2").val('');
    $("#newpassword").val('');
    $("#currentpass").val('');

    var formData = new FormData();
    formData.append("userId", _usr.id);
    formData.append("oldPassword", ecryptString(oldpwd));
    formData.append("newPassword", ecryptString(newpwd));
    formData.append("timestamp", new Date().getTime());
    var jqxhr = callRoboticsWS_POST(jsonEngineURI + "ChangePassword", formData, function (data) {
        var resp = "";
        if (data.d.status == 0) {
            resp = $.t("changepasswordOK", "El cambio de contraseña se ha realizado correctamente");
            showSuccessMsg("#mod-changepassword-response", resp);
            _.delay(closeModal, 2000, "#mod-changepassword");
        } else {
            switch (data.d.status) {
                case -1:
                    resp = $.t("NO_SESSION")
                    break;
                case -2:
                    resp = $.t("BAD_CREDENTIALS")
                    break;
                case -3:
                    resp = $.t("NOT_FOUND")
                    break;
                case -4:
                    resp = $.t("GENERAL_ERROR")
                    break;
                case -5:
                    resp = $.t("WRONG_MEDIA_TYPE")
                    break;
                case -6:
                    resp = $.t("NOT_LICENSED")
                    break;
                case -7:
                    resp = $.t("SERVER_NOT_RUNNING")
                    break;
                case -8:
                    resp = $.t("NO_LIVE_PORTAL")
                    break;
                case -9:
                    resp = $.t("NO_PERMISSIONS")
                    break;
                case -59:
                    resp = $.t("LOGIN_RESULT_LOW_STRENGHT_ERROR")
                    break;
                case -60:
                    resp = $.t("LOGIN_RESULT_MEDIUM_STRENGHT_ERROR")
                    break;
                case -61:
                    resp = $.t("LOGIN_RESULT_HIGH_STRENGHT_ERROR")
                    break;
                case -62:
                    resp = $.t("LOGIN_PASSWORD_EXPIRED")
                    break;
                case -63:
                    resp = $.t("LOGIN_NEED_TEMPORANY_KEY")
                    break;
                case -64:
                    resp = $.t("LOGIN_TEMPORANY_KEY_EXPIRED")
                    break;
                case -65:
                    resp = $.t("LOGIN_INVALID_VALIDATION_CODE")
                    break;
                case -66:
                    resp = $.t("LOGIN_BLOCKED_ACCESS_APP")
                    break;
                case -67:
                    resp = $.t("LOGIN_TEMPORANY_BLOQUED")
                    break;
                case -68:
                    resp = $.t("LOGIN_GENERAL_BLOCK_ACCESS")
                    break;
                case -69:
                    resp = $.t("LOGIN_INVALID_CLIENT_LOCATION")
                    break;
                case -70:
                    resp = $.t("LOGIN_INVALID_VERSION_APP")
                    break;
                case -71:
                    resp = $.t("LOGIN_INVALID_APP")
                    break;
                default:
                    resp = $.t("unknownerror") + "(" + data.d.status + ")";
            }
            if (resp != "") {
                showErrorMsg("#mod-changepassword-response", resp);
            }
        }
    }, function () {
        showErrorMsg("#changepassword-response", $.t("unknownerror"));
        $("#changepassword-buttons").removeClass("hide");
    });
}

function changeLangForm() {
    if ($("#mod-changelang-lang").val() != _usr.lang) {
        changeLocale($("#mod-changelang-lang").val());
        changeLanguage($("#mod-changelang-lang").val());
    }

    closeModal("#mod-changelang");
}

function changeLanguage(lang) {
    var params = 'Language=' + encodeURIComponent(lang);

    if (params != '') params += '&timestamp=' + new Date().getTime();
    else params = 'timestamp=' + new Date().getTime();

    var jqxhr = callRoboticsWS_GET(jsonEngineURI + "ChangeLanguage", params, function (data) { });
}

var i18file = {};
function geti18n() {
    var key = "";
    var val = "";
    $("[data-i18n]").each(function (index) {
        var item = {};
        key = $(this).attr("data-i18n");
        val = $(this).html();
        item[key] = val;
        if (!_.has(i18file, key)) {
            _.extend(i18file, item);
        }
    })
    console.log(JSON.stringify(i18file));
}

function any2Boolean(obj, defvalue) {
    if (defvalue == undefined) {
        defvalue = false;
    }
    if (typeof obj == "boolean") {
        return obj
    } else if (typeof obj == "string") {
        if (obj.toLowerCase() == "true" || obj == "1") {
            return true;
        } else if (obj.toLowerCase() == "false" || obj == "0") {
            return false;
        } else {
            return defvalue;
        }
    } else if (typeof obj == "string") {
        if (obj != 0) { return true; }
    } else if (typeof obj == "undefined") {
        return defvalue
    } else {
        return defvalue;
    }
}

/*******************************************************************/
/*****             Templates undescore              ****************/

var tpl = ""
tpl = '<div class="text-center loading">';
tpl += '    <img src="img/loader.gif" class="imgrow" />&nbsp;';
tpl += '    <span data-i18n="loading">Cargando...</span>';
tpl += '</div>';
var showLoading = _.template(tpl);

tpl = '<div class="text-center">';
tpl += '    <div data-alert class="alert-box alert">';
tpl += '        <%= text %>';
tpl += '    </div>';
tpl += '</div>';
var showError = _.template(tpl);

tpl = '<div class="text-center">';
tpl += '    <div data-alert class="alert-box alert">';
tpl += '        <%= text %>';
tpl += '        <a class="close">&times;</a>';
tpl += '    </div>';
tpl += '</div>';
var showErrorClose = _.template(tpl);

tpl = '<div class="text-center">';
tpl += '    <div data-alert class="alert-box success">';
tpl += '        <%= text %>';
tpl += '    </div>';
tpl += '</div>';
var showSuccess = _.template(tpl);

tpl = '<div class="text-center">';
tpl += '    <div data-alert class="alert-box success">';
tpl += '        <%= text %>';
tpl += '        <a  class="close">&times;</a>';
tpl += '    </div>';
tpl += '</div>';
var showSuccessClose = _.template(tpl);

/*****             Templates undescore               ****************/
/********************************************************************/