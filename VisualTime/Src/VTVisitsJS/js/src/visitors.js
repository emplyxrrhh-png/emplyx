var popup = null;
var _usr = { tku: "0000", id: 0, lang: "es", hasaccess: 0, hascreate: 0, hascreateEmployee: 0, haschange: 0, hasfields: 0, idemployee: 0 };

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
            } else if (data.d.result == "NotFound") {
                $("#mod-visit-response").html(showError({ text: "No existe la visita indicada." }))
                setTimeout(function () { $("#mod-visit-response").html(""); }, 3000);
            } else {
            }
        }
    }
    catch (err) {
        return true;
    }
    return true;
}

var popupOptions = {
    width: 800,
    height: 600,
    showTitle: true,
    title: "Información legal",
    visible: false,
    dragEnabled: false,
    resizeEnabled: true,
    closeOnOutsideClick: true
};
function getVisitor() {
    $(".mod-visitor-row, #mod-visitor-buttons").addClass("hide");
    $("#mod-visitor-response").html(showLoading());
    $("#mod-visit-response").html(showLoading());
    $("#mod-visitor").hide();
    var params = 'idvisit=' + $("#mod-visitid").val();
    params += '&timestamp=' + new Date().getTime();

    var jqxhr = callRoboticsWS_GET(jsonEngineURI + "visitors", params, function (data) {
        if (!hasError(data)) {
            $("#mod-visit-response").html("");
            parseVisitor(data.d);
            $('#mod-visitor .alert-box').parent().remove()
        } else {
            $("#mod-visitor-response").html(showError({ text: data.d.result }));
        }
    }, function () {
        $("#mod-visitor-response").html(showError({ text: "Error, avise a soporte." }));
    });
}

function parseVisitor(data) {
    var tmp = "";
    //Eliminamos los errores anteriores
    $("#mod-visitor .alert-input").remove();
    $("#mod-visit .alert-input").remove();

    var fl = "";
    $("#mod-visitor-idvisitor").val(data.idvisitor);
    $("#mod-visitor-name").val(data.name);
    $("#mod-visitor-fields").html("");
    data.fields.sort(function (a, b) {
        return a.position > b.position;
    });
    //fl += '<div id="mod-visit-novisitors-div"><input id="mod-visit-novisitors" type="checkbox">&nbsp;<label for="mod-visit-novisitors">' + $.t("addtostartvisit", "Indicar al iniciar la visita") + '</label></div>';

    $(data.fields).each(function (index) {
        fl += '<div class="row mod-visitor-row mod-visitor-field hide"'
        if (this.required == 99) {
            fl += ' style="display:none" ';
        }
        fl += '>';
        fl += '    <div class="large-3 column large-text-right medium-text-left small-text-left">';
        fl += '        <label for="mod-visitor-field-' + this.name + '">' + this.name + ':</label>';
        fl += '    </div>';

        fl += '    <div class="large-9 column large-text-left text-left">';
        fl += '        <textarea data-idfield="' + this.idfield + '" data-name="' + this.name + '"';
        fl += ' data-required="' + this.required + '" data-askevery="' + this.askevery + '"';
        fl += '        value="' + this.value + '" data-value="' + this.value + '"';
        fl += ' id="mod-visitor-field-' + this.name + '" name="mod-visitor-field-' + this.name + '"  type="text" maxlength="100" >';
        fl += this.value + "</textarea>"
        fl += '    </div>';
        fl += '</div>';
    });

    $("#mod-visitor-fields").html(fl);
    $(".mod-visitor-row, #mod-visitor-buttons").removeClass("hide");
    $("#mod-visitor-response").html("");
    $("#mod-visitor").show();
    $("#mod-visit").hide();

    showTitles();
}
function checkVisitor(status) {
    $("#mod-visitor .alert-input").remove();
    if ($("#mod-visitor-name").val() == "") {
        $("#mod-visitor-name").after('<div class="alert-input">Este campo es obligatorio.</div>');
    }

    if (document.getElementById('acceptLegal1').checked && document.getElementById('acceptLegal2').checked) {
    }
    else {
        $("#mod-visitor-checks").after('<div class="alert-input">Debe leer y aceptar los términos.</div>');
    }

    $("#mod-visitor-fields input, #mod-visitor-fields select").each(function (index) {
        if ($(this).val() == "") {
            if ($(this).data("required") > 0) {
                $(this).after('<div class="alert-input">Este campo es obligatorio.</div>');
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
    $("#mod-visitor-fields textarea, #mod-visitor-fields select").each(function (index) {
        fldv = _.clone(fld);
        fldv.idvisitor = vstr.idvisitor;
        fldv.idfield = $(this).data("idfield");
        fldv.value = $(this).val();
        vstr.fields.push(fldv);
    });
    if (checkVisitor()) { putVisitor(vstr); }
}

function showTitles() {
    var params = 'timestamp=' + new Date().getTime();

    var jqxhr = callRoboticsWS_GET(jsonEngineURI + "getCurrentLegalTexts", params, function (data) {
        parseTitles(data.d);
        $("#opt-printconfig-response").html("");
    }, function () {
        $("#opt-visitfields-response").html(showErrorClose({ text: $.t("unknownerror", "Error, aviste a soporte.") }));
    });
};

function parseTitles(data) {
    $("#acceptLegal1-name").text(data.title1);
    $("#acceptLegal2-name").text(data.title2);
}
function showInfo1() {
    var params = 'timestamp=' + new Date().getTime();

    var jqxhr = callRoboticsWS_GET(jsonEngineURI + "getCurrentLegalTexts", params, function (data) {
        parseCurrentLegalTexts1(data.d);
        $("#opt-printconfig-response").html("");
    }, function () {
        $("#opt-visitfields-response").html(showErrorClose({ text: $.t("unknownerror", "Error, aviste a soporte.") }));
    });
};

function showInfo2() {
    var params = 'timestamp=' + new Date().getTime();

    var jqxhr = callRoboticsWS_GET(jsonEngineURI + "getCurrentLegalTexts", params, function (data) {
        parseCurrentLegalTexts2(data.d);
        $("#opt-printconfig-response").html("");
    }, function () {
        $("#opt-visitfields-response").html(showErrorClose({ text: $.t("unknownerror", "Error, aviste a soporte.") }));
    });
}

function parseCurrentLegalTexts1(data) {
    var div = $('#popupContainer');

    //div.attr('class') = 'row';
    div.html(data.value1);

    popup = $('#popup').dxPopup(popupOptions).dxPopup("instance");
    popup.show();
}

function parseCurrentLegalTexts2(data) {
    var div = $('#popupContainer');

    //div.attr('class') = 'row';
    div.html(data.value2);

    popup = $('#popup').dxPopup(popupOptions).dxPopup("instance");
    popup.show();
}

function putVisitor(values) {
    $(".mod-visitor-row, #mod-visitor-buttons").addClass("hide");
    $("#mod-visitor-response").html(showLoading());
    var params = 'idvisit=' + $("#mod-visitid").val();
    params += '&values=' + encodeURIComponent(JSON.stringify(values));

    if (params != '') params += '&timestamp=' + new Date().getTime();
    else params = 'timestamp=' + new Date().getTime();

    var jqxhr = callRoboticsWS_GET(jsonEngineURI + "visitors", params, function (data) {
        if (!hasError(data)) {
            _newVisitor = data.d.idvisitor;
            closeNewVisitor();
            $("#mod-visit-response").html(showSuccess({ text: "Visitante almacenado." }));
            setTimeout(function () { $("#mod-visit-response").html(""); }, 5000);
        } else {
            $("#mod-visitor-response").html(showError({ text: data.d.result }));
        }
    }, function () {
        $("#mod-visitor-response").html(showError({ text: "Error, avisee a soporte." }));
    });
}

function closeNewVisitor() {
    $("#mod-visitid").val("");
    $("#mod-visitor").hide();
    $("#mod-visit").show();
}

$(document).ready(function () {
    $("#mod-visitor").hide();
});

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