(function () {
    const usr = JSON.parse(localStorage.getItem("usr"));
    if (usr.showLegalText == "1" && localStorage.getItem("ShowLegalText.VTVisits") != "0") {
        setTimeout(function () { ShowENSMessagePopup(); }, 500);
    }

    function ShowENSMessagePopup() {
        const text = $.t("ENSShowLegalText") || "El uso de este sistema sólo está permitido a los usuarios autorizados. El acceso no autorizado está totalmente prohibido, y podrá ser objeto de acciones disciplinarias, sin perjuicio de las restantes acciones de naturaleza legal a las que hubiere lugar. Toda la actividad del sistema se registra y es revisada periódicamente por el personal designado. Cualquier usuario que acceda al sistema lo hace declarando conocer y aceptar íntegramente estas reglas y la normativa general de utilización de los recursos y sistemas de información.";
        const title = $.t("ENSShowLegalTextTitle") || "AVISO A LOS USUARIOS DEL SISTEMA";

        $('#ENSpopup').dxPopup({
            width: 550,
            height: 280,
            visible: true,
            showTitle: false,
            closeOnOutsideClick: true,
            hideOnOutsideClick: true,
            contentTemplate() {
                const scrollView = $('<div/>');
                const content = $('<div style="height: 100%;display: flex;flex-direction: column;justify-content: space-around;text-align: center;" />')
                content.append($('<h3 id="legalTitle" style="font-size:18px;"></h3>').html(title));
                content.append($('<p id="legalText" style="font-size:14px;"></p>').html(text));
                scrollView.append(content);
                scrollView.dxScrollView({
                    width: '100%',
                    height: '100%',
                });
                return scrollView;
            },
        });

        localStorage.setItem("ShowLegalText.VTVisits", "0");
    }
})();