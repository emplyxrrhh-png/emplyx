@Code
    Dim labels = ViewData(VTLive40.Helpers.Constants.DefaultLanguagesEntries)
End Code

<div class="panHeader4">
    Configuración de la aplicación VisualTime en <span class='ssoTitle'>ADFS</span>
</div>

<div class="list-shareContent">
    <br />
    <div style="width: 95%;margin: 0 auto;">
        <div class="helpBodyPopup">
            <span>
                Configuración del servidor ADFS para su vinculación con VisualTime
            </span>
            <br />
            <span>
                En la configuración del servicio de ADFS crear un nuevo "Relying party"
            </span>
            <div style="width:100%"><image src="Base/images/SSO/ADFS1.png" class="helpIMG" /></div>
            <span>
                Seleccionamos la opción "Claims aware"
            </span>
            <div style="width:100%"><image src="Base/images/SSO/ADFS2.png" class="helpIMG" /></div>
            <span>
                Una vez abierto el diálogo seleccionar la tercera opción para poder añadir manualmente la configuración de la aplicación VisualTime
            </span>
            <div style="width:100%"><image src="Base/images/SSO/ADFS3.png" class="helpIMG" /></div>
            <span>
                Identificaremos con el nombre deseado el “Relying Party” y pulsaremos "Siguiente".
            </span>
            <div style="width:100%"><image src="Base/images/SSO/ADFS4.png" class="helpIMG" /></div>
            <span>
                Si se desea se configura el certificado y pulsamos "Siguiente"
            </span>
            <div style="width:100%"><image src="Base/images/SSO/ADFS5.png" class="helpIMG" /></div>
            <span>
                Habilitamos el protocolo WS-federation pasivo e indicamos la url del dominio de VisualTime. Este dominio es el que indicaremos en VisualTime como "Identificador del site ADFS"
            </span>
            <div style="width:100%"><image src="Base/images/SSO/ADFS6.png" class="helpIMG" /></div>
            <span>
                Añadimos el dominio del sitio de prueba de VisualTime y pulsamos "Siguiente"
            </span>
            <div style="width:100%"><image src="Base/images/SSO/ADFS7.png" class="helpIMG" /></div>
            <span>
                Mantenemos las políticas de acceso tal y como nos propone y pulsamos "Siguiente"
            </span>
            <div style="width:100%"><image src="Base/images/SSO/ADFS8.png" class="helpIMG" /></div>
            <span>
                Mantenemos la opción de configurar claims de seguridad y pulsamos "Cerrar"
            </span>
            <div style="width:100%"><image src="Base/images/SSO/ADFS10.png" class="helpIMG" /></div>
            <span>
                Se abre un nuevo dialogo y pulsamos sobre el botón "Añadir regla"
            </span>
            <div style="width:100%"><image src="Base/images/SSO/ADFS11.png" class="helpIMG" /></div>
            <span>
                Mantenemos las opciones propuestas y pulsamos "Siguiente"
            </span>
            <div style="width:100%"><image src="Base/images/SSO/ADFS12.png" class="helpIMG" /></div>
            <span>
                Configuramos la regla para que asigne una propiedad única al "Name ID". Se debe vincular una propiedad única como puede ser el "smb-account-name"
            </span>
            <div style="width:100%"><image src="Base/images/SSO/ADFS13.png" class="helpIMG" /></div>
            <span>
                Aplicamos y cerramos el diálgo para que apliquen los cambios.
            </span>
            <div style="width:100%"><image src="Base/images/SSO/ADFS14.png" class="helpIMG" /></div>
            <span>
                Ya tenemos lista la aplicación para enlazar con VisualTime
            </span>
        </div>
    </div>
</div>