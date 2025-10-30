
var popupInstance;
function showVersionHistory() {
    try {
        var popupInstance;
        var historyItems = document.getElementById('MainMenu_HiddenVersionHistory').value.split('*');
        var popupContainer = document.getElementById('popupVersionHistory');
        if (!popupContainer) return;

        if (!popupInstance) {
            popupInstance = $("<div>").appendTo(popupContainer).dxPopup({
                width: 330,
                height: 420,
                contentTemplate: function (contentElement) {
                    //font-family: icomoon; font-size: 15px;
                    var listHtml = $("<ul>").css({ padding: "10px", "list-style": "none", display: "flex", "flex-direction": "column", "align-items": "center", "font-family": "icomoon", "font-size": "15px" });
                    historyItems.map(function (item) {
                        $("<li>")
                            .text(item)
                            .css({ padding: "5px 0" })
                            .appendTo(listHtml);
                    });
                    listHtml.appendTo(contentElement);
                },
                showCloseButton: true,
                title: document.getElementById('MainMenu_VersionHistoryText').value,
                visible: false,
                dragEnabled: true,
                closeOnOutsideClick: true
            }).dxPopup("instance");
        }

        // Mostramos el popup
        popupInstance.show();

    } catch (e) {
        console.error('Error al mostrar el historial: ', e);
    }
}