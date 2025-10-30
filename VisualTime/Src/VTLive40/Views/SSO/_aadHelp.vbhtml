@Code
    Dim labels = ViewData(VTLive40.Helpers.Constants.DefaultLanguagesEntries)
End Code

<div class="panHeader4">
    Configuración de la aplicación VisualTime en <span class='ssoTitle'>AAD</span>
</div>

<div class="list-shareContent">
    <br />
    <div style="width: 95%;margin: 0 auto;">
        <div class="helpBodyPopup">
            <span>
                Configuración del servidor AAD para su vinculación con VisualTime
            </span>
            <br />
            <span>
                Debemos acceder la configuración de Azure Active Directory mediante el portal de microsoft:
            </span>
            <div style="width:100%"><image src="Base/images/SSO/AAD0.png" class="helpIMG" /></div>
            <span>
                Accedemos ahora a Registros de aplicaciones:
            </span>
            <div style="width:100%"><image src="Base/images/SSO/AAD1.png" class="helpIMG" /></div>
            <span>
                Después, seleccionamos Nuevo registro:
            </span>
            <div style="width:100%"><image src="Base/images/SSO/AAD2.png" class="helpIMG" /></div>
            <span>
                En la pantalla de registro de aplicación, hay 3 campos a configurar:
            </span>
            <br />
            <span>
                1- Nombre de la aplicación: Solo es el nombre que se mostrará en el portal de Azure. En este caso pondré “DemoBeta”, pero podría haber puesto perfectamente cualquier otro.
            </span>
            <br />
            <span>
                2- Quién puede acceder a la aplicación: Hay tres opciones posibles. De forma estándar dejaremos la segunda: "Cuentas en cualquier directorio organizativo"
            </span>
            <br />
            <span>
                3- URI de redirección: Seleccionar la opción web. La URI debe ser siempre la URL que lleve a VTLogin. Por ejemplo https://xxxxxx.visualtime.net/VTLive/VTLogin/
            </span>
            <div style="width:100%"><image src="Base/images/SSO/AAD3.png" class="helpIMG" /></div>
            <span>
                Acto seguido, una vez ya hayamos hecho click en Registrar, automáticamente seremos llevados a la pantalla de información general de la aplicación registrada.
            </span>
            <br />
            <span>
                Primero nos anotaremos el Identificador de cliente (ClientID) y el identificador de inquilino(Tenant) que deberemos configurar en VisualTime. Posteriormente pulsaremos sobre Autenticación:
            </span>
            <div style="width:100%"><image src="Base/images/SSO/AAD4.png" class="helpIMG" /></div>
            <span>Dentro de la pantalla de Autenticación añadiremos la url de prueba para poder validar la aplicación VisualTime correctamente</span>
            <span>Una vez añadida en el apartado de "Concesión implícita" marcaremos la opción Tokens de id:</span>
            <div style="width:100%"><image src="Base/images/SSO/AAD5.png" class="helpIMG" /></div>
        </div>
    </div>
</div>