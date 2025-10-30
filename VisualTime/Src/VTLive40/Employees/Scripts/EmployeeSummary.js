var objMap;
var pieCentersLoad = false;
var pieTaskssLoad = false;

var chartFont = {
    family: 'Robotics',
    size: 18
}

var legendFont = {
    family: 'Robotics',
    size: 10
}

function LoadTaskCharts(summaryEmployee) {
    var summaryData = JSON.parse(summaryEmployee, roDateReviver);

    TaskPieExtreme(summaryData)

    var options = {
        scale: {
            startValue: 0, endValue: 100,
            tickInterval: 50,
            label: {
                customizeText: function (arg) {
                    return arg.valueText + ' %';
                }
            }
        }
    };
}

function LoadBusniessCenterCharts(summaryEmployee) {
    var summaryData = JSON.parse(summaryEmployee, roDateReviver);

    CentersPieExtreme(summaryData);
    PresencePieExtreme(summaryData);
    AbsencePieExtreme(summaryData);

    var options = {
        scale: {
            startValue: 0, endValue: 100,
            tickInterval: 50,
            label: {
                customizeText: function (arg) {
                    return arg.valueText + ' %';
                }
            }
        }
    };
}

function TaskPieExtreme(summaryData) {
    var dataSource = [];
    if (typeof (summaryData.taskValues) != "undefined") {
        for (var i = 0; i < summaryData.taskValues.length; i++) {
            dataSource.push({
                taskName: summaryData.taskNames[i],
                taskValue: summaryData.taskValues[i]
            });
        }

        if (dataSource.length > 0) {
            $("#ctl00_contentMainBody_ASPxCallbackPanelContenido_divCanvas").dxPieChart({
                size: {
                    width: 1000
                },
                dataSource: dataSource,
                series: [
                    {
                        argumentField: "taskName",
                        valueField: "taskValue",
                        label: {
                            font: legendFont,
                            visible: true,
                            connector: {
                                visible: true,
                                width: 1
                            },
                            customizeText: function (arg) {
                                return parseFloat(arg.valueText).HoursToHHMMSS();
                            }
                        }
                    }
                ],
                tooltip: {
                    font: legendFont,
                    enabled: true,
                    customizeTooltip: function (point) {
                        return {
                            text: point.argumentText + ". " + point.percentText + " (" + parseFloat(point.valueText).HoursToHHMMSS() + ")"
                        }
                    }
                },
                legend: {
                    font: legendFont
                },
                title: {
                    text: summaryData.taskSummaryName,
                    font: chartFont
                },
                "export": {
                    enabled: true
                },
                onPointClick: function (e) {
                    //var point = e.target;
                    //toggleVisibility(point);
                },
                onLegendClick: function (e) {
                    //var arg = e.target;
                    //toggleVisibility(this.getAllSeries()[0].getPointsByArg(arg)[0]);
                }
            });
        } else {
            $("#ctl00_contentMainBody_ASPxCallbackPanelContenido_divCanvas").empty();
            $("#ctl00_contentMainBody_ASPxCallbackPanelContenido_divCanvas").removeData();
        }
    } else {
        $("#ctl00_contentMainBody_ASPxCallbackPanelContenido_divCanvas").empty();
        $("#ctl00_contentMainBody_ASPxCallbackPanelContenido_divCanvas").removeData();
    }
}

function CentersPieExtreme(summaryData) {
    var dataSourceCenter = [];
    if (typeof (summaryData.centersValues) != "undefined") {
        for (var i = 0; i < summaryData.centersValues.length; i++) {
            dataSourceCenter.push({
                centerName: summaryData.centersNames[i],
                centerValue: summaryData.centersValues[i]
            });
        }

        var maxWidth = $("#centersSummary").width();

        if (dataSourceCenter.length > 0) {
            $("#centersSummary").dxPieChart({
                size: {
                    width: 700,
                    height: 500
                },
                dataSource: dataSourceCenter,
                series: [
                    {
                        argumentField: "centerName",
                        valueField: "centerValue",
                        label: {
                            font: legendFont,
                            visible: true,
                            connector: {
                                visible: true,
                                width: 2
                            },
                            customizeText: function (arg) {
                                return (Math.round(arg.valueText * 100) / 100) + " €" + " (" + arg.percentText + ")";
                            }
                        }
                    }
                ],
                tooltip: {
                    font: legendFont,
                    enabled: true,
                    customizeTooltip: function (point) {
                        return {
                            text: point.argumentText + ". " + point.percentText + " (" + (Math.round(point.valueText * 100) / 100) + " €)"
                        }
                    }
                },
                legend: {
                    font: legendFont
                },
                title: {
                    text: summaryData.centersSummaryName,
                    font: chartFont
                },
                "export": {
                    enabled: true
                },
                onPointClick: function (e) {
                    //var point = e.target;
                    //toggleVisibility(point);
                },
                onLegendClick: function (e) {
                    //var arg = e.target;
                    //toggleVisibility(this.getAllSeries()[0].getPointsByArg(arg)[0]);
                }
            });
        } else {
            $("#centersSummary").empty();
            $("#centersSummary").removeData();
        }
    } else {
        $("#centersSummary").empty();
        $("#centersSummary").removeData();
    }
}

function PresencePieExtreme(summaryData) {
    var dataSourceWorking = [];
    if (typeof (summaryData.workingValues) != "undefined") {
        for (var i = 0; i < summaryData.workingValues.length; i++) {
            dataSourceWorking.push({
                workingName: summaryData.workingNames[i],
                workingValue: summaryData.workingValues[i]
            });
        }

        if (dataSourceWorking.length > 0) {
            $("#presenceSummary").dxPieChart({
                size: {
                    width: 550,
                    height: 350
                },
                dataSource: dataSourceWorking,
                series: [
                    {
                        argumentField: "workingName",
                        valueField: "workingValue",
                        label: {
                            font: legendFont,
                            visible: true,
                            connector: {
                                visible: true,
                                width: 2
                            },
                            customizeText: function (arg) {
                                return arg.percentText + "(" + (Math.round(arg.valueText * 100) / 100) + " €)";
                            }
                        }
                    }
                ],
                tooltip: {
                    font: legendFont,
                    enabled: true,
                    customizeTooltip: function (point) {
                        return {
                            text: point.argumentText + ". " + point.percentText + " (" + (Math.round(point.valueText * 100) / 100) + " €)"
                        }
                    }
                },
                legend: {
                    font: legendFont
                },
                title: {
                    text: summaryData.workingSummaryName,
                    font: chartFont
                },
                "export": {
                    enabled: true
                },
                onPointClick: function (e) {
                    //var point = e.target;
                    //toggleVisibility(point);
                },
                onLegendClick: function (e) {
                    //var arg = e.target;
                    //toggleVisibility(this.getAllSeries()[0].getPointsByArg(arg)[0]);
                }
            });
        } else {
            $("#presenceSummary").empty();
            $("#presenceSummary").removeData();
        }
    } else {
        $("#presenceSummary").empty();
        $("#presenceSummary").removeData();
    }
}

function AbsencePieExtreme(summaryData) {
    var dataSourceAbsence = [];
    if (typeof (summaryData.absenceValues) != "undefined") {
        for (var i = 0; i < summaryData.absenceValues.length; i++) {
            dataSourceAbsence.push({
                absenceName: summaryData.absenceNames[i],
                absenceValue: summaryData.absenceValues[i]
            });
        }

        if (dataSourceAbsence.length > 0) {
            $("#absencesSummary").dxPieChart({
                size: {
                    width: 550,
                    height: 350
                },
                dataSource: dataSourceAbsence,
                series: [
                    {
                        argumentField: "absenceName",
                        valueField: "absenceValue",
                        label: {
                            font: legendFont,
                            visible: true,
                            connector: {
                                visible: true,
                                width: 2
                            },
                            customizeText: function (arg) {
                                return arg.percentText + " (" + (Math.round(arg.valueText * 100) / 100) + " €)";
                            }
                        }
                    }
                ],
                tooltip: {
                    font: legendFont,
                    enabled: true,
                    customizeTooltip: function (point) {
                        return {
                            text: point.argumentText + ". " + point.percentText + " (" + (Math.round(point.valueText * 100) / 100) + " €)"
                        }
                    }
                },
                legend: {
                    font: legendFont
                },
                title: {
                    text: summaryData.absenceSummaryName,
                    font: chartFont
                },
                "export": {
                    enabled: true
                },
                onPointClick: function (e) {
                    //var point = e.target;
                    //toggleVisibility(point);
                },
                onLegendClick: function (e) {
                    //var arg = e.target;
                    //toggleVisibility(this.getAllSeries()[0].getPointsByArg(arg)[0]);
                }
            });
        } else {
            $("#absencesSummary").empty();
            $("#absencesSummary").removeData();
        }
    } else {
        $("#absencesSummary").empty();
        $("#absencesSummary").removeData();
    }
}

function toggleVisibility(item) {
    item.isVisible() ? item.hide() : item.show();
}

function LoadPunchImage(coord0, coord1) {
    if (coord0 != "" && coord1 != "") {
        try {
            var map = new google.maps.LatLng(Number(coord0), Number(coord1));
            objMap = new RoboticsGMaps("divPunchMap", undefined, map, false);
            objMap.initializeMap();
            objMap.drawMarker(map);
        } catch (e) { }
    }
}

function showEmpAnnualDetail(IDEmp) {
    //Añadir el año en curso
    var url = 'Scheduler/AnnualView.aspx?EmployeeID=' + IDEmp + "&Year=" + new Date().getFullYear() + "&fromPage=employees";
    var Title = '';
    parent.ShowExternalForm2(url, 950, 560, Title, '', true, false, false);
}

function LoadEmployeeSummary(selectedPeriod) {
    showLoadingGrid(true);

    $('#ctl00_contentMainBody_ASPxCallbackPanelContenido_divAccrualsSummary').hide();

    $('#ctl00_contentMainBody_ASPxCallbackPanelContenido_divCausesSummary').hide();

    $('#ctl00_contentMainBody_ASPxCallbackPanelContenido_divTasksSummary').hide();

    $('#ctl00_contentMainBody_ASPxCallbackPanelContenido_divBussinessCentersSummary').hide();

    LoadAcrrualsSummary(selectedPeriod);

    LoadPunchImage($('#ctl00_contentMainBody_ASPxCallbackPanelContenido_divSummaryPunch').attr("coord0"), $('#ctl00_contentMainBody_ASPxCallbackPanelContenido_divSummaryPunch').attr("coord1"))
}

function LoadAcrrualsSummary(selectedPeriod) {
    var Url = "";
    var divAccruals = document.getElementById("ctl00_contentMainBody_ASPxCallbackPanelContenido_divAccualDraw");
    divAccruals.innerHTML = "";
    Url = "Handlers/srvEmployees.ashx?action=DrawEmployeeSummary&ID=" + actualEmployee + "&Type=accrual&Range=" + selectedPeriod;
    AsyncCall("POST", Url, "CONTAINER", "ctl00_contentMainBody_ASPxCallbackPanelContenido_divAccualDraw", "LoadCausesSummary('" + selectedPeriod + "');");
}

function LoadCausesSummary(selectedPeriod) {
    var Url = "";
    var divAccruals = document.getElementById("ctl00_contentMainBody_ASPxCallbackPanelContenido_divCausesDraw");
    divAccruals.innerHTML = "";
    Url = "Handlers/srvEmployees.ashx?action=DrawEmployeeSummary&ID=" + actualEmployee + "&Type=cause&Range=" + selectedPeriod;
    AsyncCall("POST", Url, "CONTAINER", "ctl00_contentMainBody_ASPxCallbackPanelContenido_divCausesDraw", "LoadTaskPieDate('" + selectedPeriod + "');");
}

function LoadTaskPieDate(selectedPeriod) {
    var Url = "";

    var divCanvas = $("#divCanvas");
    divCanvas.empty();
    Url = "Handlers/srvEmployees.ashx?action=DrawEmployeeSummary&ID=" + actualEmployee + "&Type=tasks&Range=" + selectedPeriod
    AsyncCall("POST", Url, "json2", "summaryInfo", "LoadTaskCharts(summaryInfo);LoadCentersPieDate('" + selectedPeriod + "');");
}

function LoadCentersPieDate(selectedPeriod) {
    var Url = "";
    var divCentersSummary = $("#centersSummary");//document.getElementById("centersSummary");
    var divPresenceSummary = $("#presenceSummary");//document.getElementById("presenceSummary");
    var divAbsencesSummary = $("#absencesSummary");//document.getElementById("absencesSummary");
    divCentersSummary.empty();
    divPresenceSummary.empty();
    divAbsencesSummary.empty();
    Url = "Handlers/srvEmployees.ashx?action=DrawEmployeeSummary&ID=" + actualEmployee + "&Type=centers&Range=" + selectedPeriod;
    AsyncCall("POST", Url, "json2", "summaryInfo", "LoadBusniessCenterCharts(summaryInfo);ShowHideEmptySummaryInfo();showLoadingGrid(false);");
}

function ShowHideEmptySummaryInfo() {
    var bNoData = true;

    if ($('#ctl00_contentMainBody_ASPxCallbackPanelContenido_divAccrualsSummary').attr("permission") == "1") {
        if ($('#ctl00_contentMainBody_ASPxCallbackPanelContenido_divAccualDraw').html().trim() == '') {
            $('#ctl00_contentMainBody_ASPxCallbackPanelContenido_divAccrualsSummary').hide();
        } else {
            $('#ctl00_contentMainBody_ASPxCallbackPanelContenido_divAccrualsSummary').show();
            bNoData = false;
        }
    }

    if ($('#ctl00_contentMainBody_ASPxCallbackPanelContenido_divCausesSummary').attr("permission") == "1") {
        if ($('#ctl00_contentMainBody_ASPxCallbackPanelContenido_divCausesDraw').html().trim() == '') {
            $('#ctl00_contentMainBody_ASPxCallbackPanelContenido_divCausesSummary').hide();
        } else {
            $('#ctl00_contentMainBody_ASPxCallbackPanelContenido_divCausesSummary').show();
            bNoData = false;
        }
    }

    if ($('#ctl00_contentMainBody_ASPxCallbackPanelContenido_divTasksSummary').attr("permission") == "1") {
        if ($('#ctl00_contentMainBody_ASPxCallbackPanelContenido_divCanvas').html().trim() == '') {
            $('#ctl00_contentMainBody_ASPxCallbackPanelContenido_divTasksSummary').hide();
        } else {
            $('#ctl00_contentMainBody_ASPxCallbackPanelContenido_divTasksSummary').show();
            bNoData = false;
        }
    }

    if ($('#ctl00_contentMainBody_ASPxCallbackPanelContenido_divBussinessCentersSummary').attr("permission") == "1") {
        if ($('#centersSummary').html().trim() == '' && $('#presenceSummary').html().trim() == '' && $('#absencesSummary').html().trim() == '') {
            $('#ctl00_contentMainBody_ASPxCallbackPanelContenido_divBussinessCentersSummary').hide();
        } else {
            $('#ctl00_contentMainBody_ASPxCallbackPanelContenido_divBussinessCentersSummary').show();
            bNoData = false;
        }
    }

    if (bNoData) {
        $('#ctl00_contentMainBody_ASPxCallbackPanelContenido_noDataRow').show();
    } else {
        $('#ctl00_contentMainBody_ASPxCallbackPanelContenido_noDataRow').hide();
    }
}

function auditAndShowPunchImage() {
    $("#dvSummaryLoadPunch").css('display', 'none');
    $("#dvEmployeePunch").css('display', '');
    $("#imgEmployeePunch").attr('src', $('#imgLoadEmployeePunch').attr('attr-urlPunch'));
}