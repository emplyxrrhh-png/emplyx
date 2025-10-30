//*******************************************************************************************
// ** Author: I. Santaularia.
// ** Date: 04/06/2021.
// ** Description: javascript file for Genius module.
//*******************************************************************************************

function openConfigurationHelp(version) {
    return function () {

        switch (version) {
            case "adfs":
                $("#adfsHelp").dxPopup("instance").show();
                break;
            case "aad":
                $("#aadHelp").dxPopup("instance").show();
                break;
            case "okta":
                $("#oktaHelp").dxPopup("instance").show();
                break;
        }
    };
}

function checkEnableSave(e) {
    $('#btSubmitNewConfiguration').dxButton("instance").option('disabled', !e.value);
}

function checkSSOStatus(e) {
    $('#rgSSOptions').dxList("instance").option('disabled', !e.value);
    $('#ckVPortalMixAuthEnabled').dxCheckBox("instance").option('disabled', !e.value);

    $('#ckVTLiveMixAuthEnabled').dxCheckBox("instance").option('disabled', !e.value);
}

function ssoValueChanged(s,e) {
    $('#federationMetadataURL').dxTextBox("instance").option('disabled', true);
    $('#wsRealm').dxTextBox("instance").option('disabled', true);
    $('#aadClientID').dxTextBox("instance").option('disabled', true);
    $('#aadTenantID').dxTextBox("instance").option('disabled', true);
    $('#oktaClientID').dxTextBox("instance").option('disabled', true);
    $('#oktaSecret').dxTextBox("instance").option('disabled', true);
    $('#oktaAuthority').dxTextBox("instance").option('disabled', true);
    $('#samlMetadataURL').dxTextBox("instance").option('disabled', true);
    $('#samlIpID').dxTextBox("instance").option('disabled', true);
    $('#samlSigningBehaviour').dxSelectBox("instance").option('disabled', true);

    switch (s.addedItems[0].toLowerCase()) {
        case 'adfs':
            $('#federationMetadataURL').dxTextBox("instance").option('disabled', false);
            $('#wsRealm').dxTextBox("instance").option('disabled', false);
            $('#federationMetadataURL').dxTextBox("instance").focus();
            break;
        case 'aad':
            $('#aadClientID').dxTextBox("instance").option('disabled', false);
            $('#aadTenantID').dxTextBox("instance").option('disabled', false);
            $('#aadClientID').dxTextBox("instance").focus();
            break;
        case 'okta':
            $('#oktaClientID').dxTextBox("instance").option('disabled', false);
            $('#oktaSecret').dxTextBox("instance").option('disabled', false);
            $('#oktaAuthority').dxTextBox("instance").option('disabled', false);
            $('#oktaClientID').dxTextBox("instance").focus();
            break;
        case 'saml':
            $('#samlMetadataURL').dxTextBox("instance").option('disabled', false);
            $('#samlIpID').dxTextBox("instance").option('disabled', false);
            $('#samlSigningBehaviour').dxSelectBox("instance").option('disabled', false);
            $('#samlMetadataURL').dxTextBox("instance").focus();
            break;
    }
}

function ssoItemTemplate(itemData, _, itemElement) {
    let itemTemplate = null;

    switch (itemData.toLowerCase()) {
        case 'cegidid':
            itemTemplate = $("<div style='padding-top:15px'>").append(
                $('<div>').append(
                    $('<div style="float:left">').append($("<span class='ssoTitle'>").text("cegidID")),
                    $('<div style="float:left">').append($("<span style='margin-left:15px'>").text(jsLabels["SSO#cegidInformation"]))
                ));
            break;
        case 'adfs':
            itemTemplate = $("<div>").append(
                $('<div>').append(
                    $('<div style="float:left">').append($("<span class='ssoTitle'>").text(itemData)),
                    $('<div style="float:left">').append($("<span style='margin-left:15px'>").text(jsLabels["SSO#adfsInformation"])), //"Pulsa aquí para obtener información de como configurar tu servicio ADFS"
                    $('<div style="float:left">').append($("<span>").text("[...]").on('click', openConfigurationHelp('adfs')))
                ),
                $('<div style="clear:both" class="dx-fieldset ssoOption">').append(
                    $('<div class="dx-field">').append($("<span>").text(jsLabels["SSO#adfsmetadataURL"]), $('<div id="federationMetadataURL">').dxTextBox({ width: '500px' })),
                    $('<div class="dx-field">').append($("<span>").text(jsLabels["SSO#adfsRealm"]), $('<div id="wsRealm">').dxTextBox({ width: '500px' }))
                ));
            break;
        case 'aad':
            itemTemplate = $("<div>").append(
                $('<div>').append(
                    $('<div style="float:left">').append($("<span class='ssoTitle'>").text(itemData)),
                    $('<div style="float:left">').append($("<span style='margin-left:15px'>").text(jsLabels["SSO#aadInformation"])),
                    $('<div style="float:left">').append($("<span>").text("[...]").on('click', openConfigurationHelp('aad')))
                ),
                $('<div style="clear:both" class="dx-fieldset ssoOption">').append(
                    $('<div class="dx-field">').append($("<span>").text(jsLabels["SSO#aadClient"]), $('<div id="aadClientID">').dxTextBox({ width: '500px' })),
                    $('<div class="dx-field">').append($("<span>").text(jsLabels["SSO#aadTenant"]), $('<div id="aadTenantID">').dxTextBox({ width: '500px' }))
                ));
            break;
        case 'okta':
            itemTemplate = $("<div>").append(
                $('<div>').append(
                    $('<div style="float:left">').append($("<span class='ssoTitle'>").text(itemData)),
                    $('<div style="float:left">').append($("<span style='margin-left:15px'>").text(jsLabels["SSO#oktaInformation"])),
                    $('<div style="float:left">').append($("<span>").text("[...]").on('click', openConfigurationHelp('okta')))
                ),
                $('<div style="clear:both" class="dx-fieldset ssoOption">').append(
                    $('<div class="dx-field">').append($("<span>").text(jsLabels["SSO#oktaClient"]), $('<div id="oktaClientID">').dxTextBox({ width: '500px' })),
                    $('<div class="dx-field">').append($("<span>").text(jsLabels["SSO#oktaSecret"]), $('<div id="oktaSecret">').dxTextBox({ width: '500px' })),
                    $('<div class="dx-field">').append($("<span>").text(jsLabels["SSO#oktaAuthority"]), $('<div id="oktaAuthority">').dxTextBox({ width: '500px' }))
                ));
            break;
        case 'saml':
            itemTemplate = $("<div>").append(
                $('<div>').append(
                    $('<div style="float:left">').append($("<span class='ssoTitle'>").text(itemData)),
                    $('<div style="float:left">').append($("<span style='margin-left:15px'>").text(jsLabels["SSO#samlInformation"])),
                ),
                $('<div style="clear:both" class="dx-fieldset ssoOption">').append(
                    $('<div class="dx-field">').append($("<span>").text(jsLabels["SSO#metadataURL"]), $('<div id="samlMetadataURL">').dxTextBox({ width: '500px' })),
                    $('<div class="dx-field">').append($("<span>").text(jsLabels["SSO#ipID"]), $('<div id="samlIpID">').dxTextBox({ width: '500px' })),
                    $('<div class="dx-field">').append($("<span>").text(jsLabels["SSO#signingBehaviour"]), $('<div id="samlSigningBehaviour">').dxSelectBox(
                        {
                            width: '500px',
                            displayExpr: 'Name',
                            valueExpr: 'ID',
                            dataSource: [
                                {
                                    ID: 'Never',
                                    Name: jsLabels["SSO#never"]
                                }, {
                                    ID: 'IfIdpWantAuthnRequestsSigned',
                                    Name: jsLabels["SSO#idp"]
                                }, {
                                    ID: 'Always',
                                    Name: jsLabels["SSO#always"]
                                }
                            ]
                        }))
                ));
            break;
    }
    itemElement.append(itemTemplate);
    //itemElement.parent().addClass(itemData.toLowerCase()).append(itemTemplate);
}

function saveSSOData(e) {
    saveData();
}

var currentConfig = null;

(function () {
    //Properties
    var viewHandler = null;

    $(document).ready(async function () {

        viewUtilsManager.initAccordions();
        viewUtilsManager.setupCardListFilterButton("SSO");

        viewUtilsManager.print("SSO Module loaded");
        viewHandler = viewUtilsManager.createViewStateHandler();

        // -----------------------------
        // GET DATA VIEW (ON READY) ----
        //------------------------------
        window.loadRequest = loadSSOConfiguration;
        window.saveData = saveData;
        
        document.addEventListener("startStateEvent", (data) => viewHandlerEvent(data), false);

        viewHandler.transition(viewHandler.value, "read");

        viewUtilsManager.loadViewOptions("SSO", "read", function () {
            $('#btSubmitNewConfiguration').dxButton("instance").option('disabled', false);
            loadSSOConfiguration();
        }, () => { },'LiveSecurity');


    });

    function viewHandlerEvent(eventData) {

        var eventDetails = eventData.detail;

        viewUtilsManager.print(eventDetails.currentState + " state");
        switch (eventDetails.currentState) {

            case "read":
                //Código para limpiar los controles
                //console.log('READDDDDDD');
                break;

            case "create":
                //console.log('CREATEEEEEE');

                break;

            case "update":
                //console.log('UPDATEEEEEEEEEE');
                break;

            case "delete":
                //console.log('DELETEEEEEEEEEEEE');
                break;
        }
    };

    //*******************************************************************************************
    //FUNCTIONS              ********************************************************************
    //*******************************************************************************************

    function bindSSOConfiguration(response) {
        currentConfig = response;

        $('#federationMetadataURL').dxTextBox("instance").option('value', response.Adfs.FedartionURL);
        $('#wsRealm').dxTextBox("instance").option('value', response.Adfs.FederationRealm);
        $('#aadClientID').dxTextBox("instance").option('value', response.AAD.ClientID);
        $('#aadTenantID').dxTextBox("instance").option('value', response.AAD.TenantID);
        $('#oktaClientID').dxTextBox("instance").option('value', response.Okta.ClientId);
        $('#oktaSecret').dxTextBox("instance").option('value', response.Okta.ClientSecret);
        $('#oktaAuthority').dxTextBox("instance").option('value', response.Okta.Authority);

        $('#samlMetadataURL').dxTextBox("instance").option('value', response.SAML.MetadataURL);
        $('#samlIpID').dxTextBox("instance").option('value', response.SAML.IdentityProviderID);
        $('#samlSigningBehaviour').dxSelectBox("instance").option('value', response.SAML.SigningBehaviour == '' ? 'IfIdpWantAuthnRequestsSigned' : response.SAML.SigningBehaviour);

        $('#ckSSOStatus').dxCheckBox("instance").option('value', response.Active);
        $('#ckVTLiveMixAuthEnabled').dxCheckBox("instance").option('value', response.VTLiveMixAuthEnabled);
        $('#ckVPortalMixAuthEnabled').dxCheckBox("instance").option('value', response.VTPortalMixAuthEnabled);

        switch (response.SSOType) {
            case 1:
                $('#rgSSOptions').dxList("instance").selectItem(0);
                break;
            case 2:
                $('#rgSSOptions').dxList("instance").selectItem(2);
                break;
            case 3:
                $('#rgSSOptions').dxList("instance").selectItem(1);
                break;
            case 4:
                $('#rgSSOptions').dxList("instance").selectItem(3);
                break;
            case 5:
                $('#rgSSOptions').dxList("instance").selectItem(4);
                break;
        }
    }

    const loadSSOConfiguration = () => {
        viewUtilsManager.makeServiceCall(
            "SSO",
            "GetSSOConfiguration",
            "POST",
            {  },
            null,
            function (response) {
                if (response != null) bindSSOConfiguration(response);
                else {
                    DevExpress.ui.notify(viewUtilsManager.DXTranslate('roPermissionsError'), 'error', 2000);
                }
            },
            null,
            null
        );
    }

    function loadModel() {
        var config = { ...currentConfig };
        config.Adfs.FedartionURL = $('#federationMetadataURL').dxTextBox("instance").option('value');
        config.Adfs.FederationRealm = $('#wsRealm').dxTextBox("instance").option('value');
        config.AAD.ClientID = $('#aadClientID').dxTextBox("instance").option('value');
        config.AAD.TenantID = $('#aadTenantID').dxTextBox("instance").option('value');
        config.Okta.ClientId = $('#oktaClientID').dxTextBox("instance").option('value');
        config.Okta.ClientSecret = $('#oktaSecret').dxTextBox("instance").option('value');
        config.Okta.Authority = $('#oktaAuthority').dxTextBox("instance").option('value');
        config.SAML.MetadataURL = $('#samlMetadataURL').dxTextBox("instance").option('value');
        config.SAML.IdentityProviderID = $('#samlIpID').dxTextBox("instance").option('value');
        config.SAML.SigningBehaviour = $('#samlSigningBehaviour').dxSelectBox("instance").option('value');

        config.Active = $('#ckSSOStatus').dxCheckBox("instance").option('value');
        config.VTLiveMixAuthEnabled = $('#ckVTLiveMixAuthEnabled').dxCheckBox("instance").option('value');
        config.VTPortalMixAuthEnabled = $('#ckVPortalMixAuthEnabled').dxCheckBox("instance").option('value');


        var selectedTypes = $('#rgSSOptions').dxList("instance").option("selectedItems");

        if (selectedTypes.length > 0) {
            var selectedType = selectedTypes[0];
            switch (selectedType.toLowerCase()) {
                case "cegidid":
                    config.SSOType = "cegidId";
                    break;
                case "aad":
                    config.SSOType = "AAD";
                    break;
                case "adfs":
                    config.SSOType = "Adfs";
                    break;
                case "okta":
                    config.SSOType = "Okta";
                    break;
                case "saml":
                    config.SSOType = "SAML";
                    break;
            }
        }

        return config;
    }

    const saveData = () => {
        viewUtilsManager.makeServiceCall(
            "SSO",
            "SaveSSOConfiguration",
            "POST",
            { oConf: loadModel() },
            null,
            function (response) {
                if (response) {
                    DevExpress.ui.dialog.alert(viewUtilsManager.DXTranslate('roSSOConfigSavedMT'), 'Info');
                } else {
                    DevExpress.ui.dialog.alert(viewUtilsManager.DXTranslate('roSSOSaveError'), 'Error');
                }
            },
            null,
            null
        );
    }


    
})();