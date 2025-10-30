@Code
    Dim labels = ViewData(VTLive40.Helpers.Constants.DefaultLanguagesEntries)
End Code

<div class="panHeader4">
    Configuración de la aplicación VisualTime en <span class='ssoTitle'>OKTA</span>
</div>

<div class="list-shareContent">
    <br />
    <div style="width: 95%;margin: 0 auto;">
        <div class="helpBodyPopup">
            <span>
                Configuración del servidor OKTA para su vinculación con VisualTime
            </span>
            <br />
            <span>
                Accederemos al servicio de OKTA y añadiremos una nueva aplicación con el nombre de VisualTime
            </span>
            <br />
            <span>
                En la configuración de la aplicación dejaremos la configuración tal  como se muestra en la siguiente captura:
            </span>
            <div style="width:100%"><image src="Base/images/SSO/OKTA01.png" class="helpIMG" /></div>
            <span>
                También se añadirá la URL "https://xxxxxxxx.visualtime.net/VTLive/VTCheckSSO/" como url de redirección de inicio de sesión valida.
            </span>
            <br />
            <span>Copiaremos los datos de identificador de cliente y secreto necesarios para configurar el inicio de sesión en VisualTime</span>
            <div style="width:100%"><image src="Base/images/SSO/OKTA02.png" class="helpIMG" /></div>
        </div>
    </div>
</div>