function CallbackSession_CallbackComplete(s, e) {
    loadConcurrency(s.cpGraphInfo)
}

function customizeText() {
    var tt = new moment(this.value);
    return tt.format('hh:mm A DD/MM/YY');
}

function loadConcurrency(concurrentInfo) {
    var maxConcurrent = $('#maxConcurrentUsers').val();
    var oConcurrentInfo = JSON.parse(concurrentInfo)

    $("#dxChart").dxChart({
        barWidth: 1,
        size: {
            width: screen.width - screen.width / 30,
            height: screen.height / 1.5
        },
        commonSeriesSettings: {
            barPadding: 5,
            argumentField: "Datetime",
            ignoreEmptyPoints: "true"
        },
        series: [
            { valueField: "RealCount", name: "Licencias en uso"/*, type: "bar" */ },
            {
                valueField: "Max",
                name: "Límite alcanzado",
                type: "bar"
            }
        ]
        ,
        customizePoint: function (point) {
            if (point.value == 0)
                return {
                    color: 'transparent',
                    hoverStyle: {
                        color: 'transparent'
                    },
                    selectionStyle: {
                        color: 'transparent'
                    }
                }
        },
        dataSource: oConcurrentInfo,
        title: "",
        argumentAxis: {
            argumentType: "datetime",
            tickInterval: "day",
            minorTickInterval: "hours",
            tick: { visible: "true" },
            label: {
                overlappingBehavior: "rotate",
                customizeText: customizeText,
            }
        },
        crosshair: {
            enabled: true,
            color: "#949494",
            width: 3,
            dashStyle: "dot",
            label: {
                visible: true,
                backgroundColor: "#949494",
                font: {
                    color: "#fff",
                    size: 12,
                }
            }
        },
        valueAxis: [{
            allowDecimals: false,
            max: maxConcurrent,
            valueMarginsEnabled: false,
            constantLines: [{
                value: maxConcurrent,
                color: 'red',
                width: 3,
                label: {
                    visible: true,
                    text: "Licencias contratadas"
                }
            }]
        }],

        tooltip: {
            enabled: true,
            customizeTooltip: function (arg) {
                return {
                    text: arg.valueText
                };
            }
        },
        "export": {
            enabled: true
        },
        legend: {
            verticalAlignment: "bottom",
            horizontalAlignment: "center",
            itemTextPosition: "bottom",
            equalColumnWidth: true
        },
        title: {
            text: "Licencias Usadas"
        },
        scrollingMode: "all",
        zoomingMode: "all",
        adjustOnZoom: false,
    });
};