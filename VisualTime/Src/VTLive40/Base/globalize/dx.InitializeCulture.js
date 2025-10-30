
$(function () {
    DevExpress.ui.dxOverlay.baseZIndex(20000);
});

var cLangPrefix = "";
var cLangVersion = "";

function loadJSLanguages(prefix, version, onContinue) {

    cLangVersion = version;
    cLangPrefix = prefix;

    var locale = readCookie("VTLive_Language", "es");

    if (typeof (Storage) !== "undefined") {

        if (localStorage.roLanguage) {

            try {
                var oLanguage = JSON.parse(localStorage.roLanguage);
                if (oLanguage.key == locale) {
                    moment.locale(locale);
                    Globalize.load(oLanguage.likely);
                    Globalize.load(oLanguage.environment);
                    Globalize.loadMessages(oLanguage.devxData);
                    Globalize.loadMessages(oLanguage.localeData);
                    Globalize.loadMessages(oLanguage.errors);
                    Globalize.locale(locale);
                } else {
                    loadJSLanguagesFinally(onContinue, true);
                }
                if (typeof onContinue != 'undefined' && onContinue != '') onContinue();

            } catch (e) {
                localStorage.removeItem("roLanguage");
                loadJSLanguagesFinally(onContinue, true);
            }
            
            
        } else {
            loadJSLanguagesFinally(onContinue, true);
        }
    } else {
        loadJSLanguagesFinally(onContinue, false);
    }
}

function loadJSLanguagesFinally(onContinue, bSave) {
    var locale = readCookie("VTLive_Language", "es");
    var roLangObj = { key: locale };

    $.getJSON(cLangPrefix + "/globalize/cldr/locales/likelySubtags.json?v=" + cLangVersion, function (lsData) {
        moment.locale(locale);
        Globalize.load(lsData);
        roLangObj.likely = lsData;
        $.getJSON(cLangPrefix + "/globalize/cldr/locales/environment-" + locale + ".json?v=" + cLangVersion, function (enData) {
            Globalize.load(enData);
            roLangObj.environment = enData;

            $.getJSON(cLangPrefix + "/globalize/localization/dx.all." + locale + ".json?v=" + cLangVersion, function (devxData) {
                Globalize.loadMessages(devxData);
                roLangObj.devxData = devxData;

                $.getJSON(cLangPrefix + "/globalize/localization/dx.vtlive." + locale + ".json?v=" + cLangVersion, function (localeData) {
                    Globalize.loadMessages(localeData);
                    roLangObj.localeData = localeData;


                    $.getJSON(cLangPrefix + "/globalize/localization/dx.errors." + locale + ".json?v=" + cLangVersion, function (errorsData) {
                        Globalize.loadMessages(errorsData);
                        roLangObj.errors = errorsData;

                        $.getJSON(cLangPrefix + "/surveyjs/locale/surveyjs." + locale + ".json?v=" + cLangVersion, function (surveyData) {
                            roLangObj.survey = surveyData;
                           
                            $.getJSON(cLangPrefix + "/surveyjs/locale/surveyanalyticsjs." + locale + ".json?v=" + cLangVersion, function (surveyAnalyticsData) {
                                roLangObj.surveyAnalytics = surveyAnalyticsData;
                                Globalize.locale(locale);

                                if (bSave) localStorage.setItem("roLanguage", JSON.stringify(roLangObj));
                                if (typeof onContinue != 'undefined' && onContinue != '') onContinue();
                            });
                        
                        });
                    });
                });
            });

        });
    });
}

function loadJSErrorLanguages(onContinue) {
    var locale = readCookie("VTLive_Language", "es");

    if (typeof (Storage) !== "undefined") {

        if (localStorage.roLanguage) {

            try {
                var oLanguage = JSON.parse(localStorage.roLanguage);
                if (oLanguage.key == locale && typeof oLanguage.errors != 'undefined') {
                    Globalize.loadMessages(oLanguage.errors);
                } else {
                    loadJSErrorLanguagesFinally(onContinue, true);
                }
            } catch (e) {
                localStorage.removeItem("roLanguage");

                var callback = function () {
                    loadJSErrorLanguagesFinally(onContinue, true);
                }
                loadJSLanguagesFinally(cLangPrefix, cLangVersion, callback, true);
            }

            if (typeof onContinue != 'undefined' && onContinue != '') onContinue();

        } else {
            var callback = function () {
                loadJSErrorLanguagesFinally(onContinue, true);
            }

            loadJSLanguagesFinally(cLangPrefix, cLangVersion, callback, true);
        }
    } else {
        loadJSErrorLanguagesFinally(onContinue, false);
    }
}

function loadJSErrorLanguagesFinally(onContinue, bSave) {
    var locale = readCookie("VTLive_Language", "es");
    var roLangObj = { key: locale };

    $.getJSON(cLangPrefix + "/localization/dx.errors." + locale + ".json?v=" + cLangVersion, function (localeData) {
        Globalize.loadMessages(localeData);

        if (typeof (Storage) !== "undefined" && localStorage.roLanguage) {
            roLangObj = JSON.parse(localStorage.roLanguage);
            roLangObj.errors = localeData;
        }
        

        if (bSave) {
            localStorage.setItem("roLanguage", JSON.stringify(roLangObj));
        }


        if (typeof onContinue != 'undefined' && onContinue != '') onContinue();

    });
}


