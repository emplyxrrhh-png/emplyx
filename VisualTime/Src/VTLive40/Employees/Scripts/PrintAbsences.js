function LoadEmployeeAbsences(s) {
    actualEmployeeAbsences = JSON.parse(s.cpAbsences, roDateReviver);

    gridEmployeeAbsences = $("#divAbsencesGrid").dxDataGrid({
        showColumnLines: true,
        showRowLines: true,
        rowAlternationEnabled: true,
        showBorders: true,
        dataSource: {
            store: actualEmployeeAbsences
        },
        editing: {
            mode: "row",
            allowUpdating: false,
            allowDeleting: false,
            texts: { deleteRow: 'Delete', editRow: 'Edit' }
        },
        onCellPrepared: function (e) {
            if (e.rowType === "data" && e.column.command === "edit") {
                var isEditing = e.row.isEditing, $links = e.cellElement.find(".dx-link");
                $links.text("");

                if (isEditing) {
                    $links.filter(".dx-link-save").addClass("dx-icon-save");
                    $links.filter(".dx-link-cancel").addClass("dx-icon-revert");
                } else {
                    $links.filter(".dx-link-edit").addClass("dx-icon-edit");
                    $links.filter(".dx-link-delete").addClass("dx-icon-trash");
                }
            }
        },

        onRowRemoved: function (e) {
            hasChanges(true);
        },
        onEditingStart: function (e) {
            editProductiveUnit(e);
        },
        remoteOperations: {
            sorting: true,
            paging: true
        },
        paging: {
            pageSize: 12
        },
        pager: {
            showPageSizeSelector: false
        },
        columns: [

            //{ caption: "ID", dataField: "IDCause", allowEditing: false },
            { caption: "Tipo", dataField: "Tipo", allowEditing: false },
            { caption: "Código Unix", dataField: "CodigoUnix", allowEditing: false },
            { caption: "Descripción", dataField: "Name", allowEditing: false },
            { caption: "Inicio", dataField: "BeginDate", allowEditing: false },
            { caption: "Fin", dataField: "FinishDate", allowEditing: false },
            { caption: "Días Laborables", dataField: "DiasLaborables", allowEditing: false },
            //{ caption: "Días Pendientes", dataField: "DiasPendientes", allowEditing: false },
            { caption: "Día de Incorporación", dataField: "DiaDeIncorporacion", allowEditing: false }
            //{
            //    width: 100,
            //    alignment: 'center',
            //    cellTemplate: function (container, options) {
            //        $('<a/>').addClass('dx-link')
            //            .text('Exportar')
            //            .on('dxclick', function () {
            //                var oParameters = {};
            //                oParameters.data = options.data.RowID;
            //                oParameters.action = 'EXPORT';
            //                var strParameters = JSON.stringify(oParameters);
            //                strParameters = encodeURIComponent(strParameters);

            //                PerformActionCallbackClient.PerformCallback(strParameters);
            //            })
            //            .appendTo(container);
            //    }
            //},
            //{
            //    type: "buttons",
            //    width: 110,
            //    buttons: [{
            //        hint: "Exportar",
            //        icon: "download",
            //        visible: function (e) {
            //            return true;
            //        },
            //        onClick: function (e) {
            //        }
            //    }]
            //},
        ]
    }).dxDataGrid("instance");
}