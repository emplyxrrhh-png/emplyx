var pivotData = { default: { dataSource: [], series: [] } };

function drawDevXtremeGraph(summaryData) {
    if (typeof summaryData != 'undefined' && summaryData != null) {
        pivotData = JSON.parse(summaryData, roDateReviver);

        reloadChart(null, null);
    } else {
        pivotData = { default: { dataSource: [], series: [] } };
        reloadChart(null, null);
    }
}

function reloadChart(s, e) {
    if (typeof ChartType_Client != 'undefined') {
        var clientWidth = $('#ctl00_contentMainBody_CallbackPanelPivot_graphsServer').width();
        var chartType = ChartType_Client.GetSelectedItem().value;
        var legendVisible = PointLabels_Client.GetChecked();

        if ($("#dxChartMenu").is(":visible") == true) {
            $("#graphSwitch").text($("#hideGraphLng").attr('value'))
        } else {
            $("#graphSwitch").text($("#showGraphLng").attr('value'));
        }

        if ($("#dxChartMenu").is(":visible") == true) {
            if (chartType != 'pie' && chartType != 'doughnut') {
                $("#dxChart").show();
                $("#dxPieChart").hide();
                $("#dxChart").dxChart({
                    size: {
                        width: clientWidth
                    },
                    commonSeriesSettings: {
                        type: chartType,
                        label: {
                            visible: false
                        }
                    },
                    dataSource: pivotData.default.dataSource,
                    series: pivotData.default.series,
                    title: "",
                    tooltip: {
                        enabled: true,
                        customizeTooltip: function () {
                            return { text: this.argumentText + "<br>" + this.seriesName + ": " + parseFloat(this.valueText).HoursToHHMMSS(false) }
                        }
                    },
                    "export": {
                        enabled: true
                    },
                    legend: {
                        visible: legendVisible
                    },
                    scrollingMode: "all",
                    zoomingMode: "all",
                });
            } else {
                $("#dxChart").hide();
                $("#dxPieChart").show();
                $("#dxPieChart").dxPieChart({
                    size: {
                        width: clientWidth
                    },
                    commonSeriesSettings: {
                        type: chartType,
                        label: {
                            visible: false
                        }
                    },
                    tooltip: {
                        enabled: true,
                        customizeTooltip: function () {
                            return { text: this.argumentText + "<br>" + this.seriesName + ": " + parseFloat(this.valueText).HoursToHHMMSS(false) }
                        }
                    },
                    dataSource: pivotData.default.dataSource,
                    series: pivotData.default.series,
                    title: "",
                    "export": {
                        enabled: true
                    },
                    legend: {
                        visible: legendVisible
                    },
                    onLegendClick: function (e) {
                        var arg = e.target;

                        var series = this.getAllSeries();

                        for (var sIn = 0; sIn < series.length; sIn++) {
                            toggleVisibility(series[sIn].getPointsByArg(arg)[0]);
                        }
                    },
                    scrollingMode: "all",
                    zoomingMode: "all",
                });
            }
        }
    }
}

function toggleVisibility(item) {
    item.isVisible() ? item.hide() : item.show();
}

function graphPerformanceWarning(s, e) {
    if (s.GetChecked()) {
        DevExpress.ui.dialog.alert(Globalize.formatMessage('roPerformanceWarning'), Globalize.formatMessage('roAlert'));
    }
}