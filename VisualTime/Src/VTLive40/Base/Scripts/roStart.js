
$(function () {
    $("#divValuesGrid").dxDataGrid({
        showColumnLines: true,
        showRowLines: true,
        rowAlternationEnabled: true,
        showBorders: true,
        headerFilter: { visible: true },
        allowColumnResizing: true,
        filterRow: { visible: true },

        selection: {
            mode: "multiple" // or "multiple" | "none"
        },
        editing: {
            mode: "row",
            allowUpdating: false,
            allowAdding: false,
            allowDeleting: false,
            texts: { deleteRow: 'Delete', editRow: 'Edit', confirmDeleteMessage: "" }
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
        remoteOperations: {
            sorting: true,
            paging: true
        },
        paging: {
            enabled: true,
            pageSize: 50
        },
        pager: {
            showPageSizeSelector: true,
            allowedPageSizes: [10, 50, 100],
            showInfo: true
        },
        columns: [
            { caption: "Id", dataField: "EmployeeId", allowEditing: false, allowDeleting: false, visible: false, },
            { caption: "Empleado", dataField: "Name", allowEditing: false, allowDeleting: false, },
            { caption: "Estado", dataField: "PresenceStatus", allowEditing: false, allowDeleting: false, },
            { caption: "Grupo", dataField: "[Group]", allowEditing: false, allowDeleting: false, },
            { caption: "Tarea", dataField: "TaskTitle", allowEditing: false, allowDeleting: false, },
            { caption: "Centro de Coste", dataField: "CostCenterName", allowEditing: false, allowDeleting: false, }
            //{
            //    caption: "Tipo", dataField: "DocumentTemplateScope",
            //    allowEditing: false, allowDeleting: false,
            //    calculateCellValue: function (rowData) {
            //        switch (rowData.DocumentTemplateScope) {

            //            case "EmployeeField":
            //                return "Campo de la ficha";
            //                break;
            //            case "EmployeeContract":
            //                return "Contrato";
            //                break;
            //            case "Company":
            //                return "Empresa";
            //                break;
            //            case "LeaveOrPermission":
            //                return "Baja o permiso";
            //                break;
            //            case "CauseNote":
            //                return "Justificante";
            //                break;
            //            case "EmployeeAccessAuthorization":
            //                return "Autorización de acceso";
            //                break;
            //            case "CompanyAccessAuthorization":
            //                return "Autorizacón de empresa";
            //                break;
            //            case "Communique":
            //                return "Comunicado";
            //                break;
            //            default:
            //                return "Desconocido";
            //                break;

            //        }
            //    },
            //},
       

        ]
    }).dxDataGrid("instance");
});