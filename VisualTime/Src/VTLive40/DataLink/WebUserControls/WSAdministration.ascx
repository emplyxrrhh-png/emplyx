<%@ Control Language="VB" AutoEventWireup="false" Inherits="VTLive40.WebUserControls_WSAdministration" CodeBehind="WSAdministration.ascx.vb" %>

<%@ Register Src="~/Security/WebUserControls/frmIPs.ascx" TagName="frmIPs" TagPrefix="roForms" %>

<script language="javascript" type="text/javascript">

    function btnRefreshClient_Click() {
        PerformActionCallbackClient.PerformCallback("PERFORM_ACTION");
    }

<%--    linkOPCItems('<%= rbWSModeV1.ClientID %>,<%= rbWSModeV2.ClientID %>');
    checkOnChangeOPanel('<%= rbWSModeV1.ClientID %>', '0', 'false');
    checkOnChangeOPanel('<%= rbWSModeV2.ClientID %>', '0', 'false');--%>

    function PerformActionCallback_CallbackComplete(s, e) {
        if (s.cpAction == "PERFORM_ACTION") {
            AspxLoadingPopup_Client.Show();
            monitor = setInterval(function () {
                PerformActionCallbackClient.PerformCallback("CHECKPROGRESS");
            }, 5000);
        } else if (s.cpAction == "ERROR") {
            clearInterval(monitor);
            AspxLoadingPopup_Client.Hide();
        } else if (s.cpAction == "CHECKPROGRESS") {
            //document.getElementById('ctl00_contentMainBody_CallbackPanelPivot_lblAnalyticsOldData').style.display = 'none';
            if (s.cpActionResult == "OK") {
                clearInterval(monitor);
                grdAuditClient.PerformCallback("REFRESH");
                AspxLoadingPopup_Client.Hide();
            }
        }
    }

    function createWSElements(s, prefix) {
        var hdGridIPs = [{ 'fieldname': 'value', 'description': '', 'size': '100%' }];
        hdGridIPs[0].description = document.getElementById('ctl00_contentMainBody_hdnValueGridName').value;

        var allowedIPs = document.getElementById("txtAllowedIPs").value;
        var arrGridIPs = [];
        if (allowedIPs != null) {
            var ips = allowedIPs.split('#');
            for (var i = 0; i < ips.length; i++) {
                if (ips[i] != "") {
                    arrGridIPs.push({ fields: [{ field: 'id', value: i }, { field: 'value', value: ips[i] }] });
                }
            }

            jsGridIPs = new jsGrid('gridAllowedIPs', hdGridIPs, arrGridIPs, true, true, false, 'AllowedIPs');
        }
    }

    function callbackGenerateToken() {
        setTimeout(function () {
            window.scrollTo(0, 700);
        }, 100);
    }

    function <%= Me.ClientID %>_AddNewCertificate(idPGPKey) {
        var controlerId = "<%= Me.ClientID %>";
        var url = "../Options/UploadCertificate.aspx?ClientId=" + controlerId;
        var Title = '';

        <%= Me.ClientID %>_NewCertificatePopup.SetContentUrl(url);
        <%= Me.ClientID %>_NewCertificatePopup.Show()
    }

    function <%= Me.ClientID %>_DownloadCertificate() {
        window.open("/Employees/DocumentVisualize.aspx?VTPublicPGPKey=true", "_blank");
    }
</script>

<dx:ASPxCallback ID="PerformActionCallback" runat="server" ClientInstanceName="PerformActionCallbackClient" ClientSideEvents-CallbackComplete="PerformActionCallback_CallbackComplete"></dx:ASPxCallback>

<div class="panHeader2">
    <span style="">
        <asp:Label runat="server" ID="lblWsImportGeneral" Text="Configuración del servicio web de importación."></asp:Label></span>
</div>
<div class="panelHeaderContent" style="padding-top: 25px">
    <div class="panelDescriptionImage">
        <div class="wsConfigSection" runat="server" style="height: 48px;"></div>
    </div>
    <div class="panelDescriptionText">
        <asp:Label ID="lblWsImportGeneralDesc" runat="server" Text="En esta sección puede configurar los datos de conexión con el servicio web ofrece robotics para realizar importaciones de empleados / ausencias."></asp:Label>
    </div>
</div>

<input type="hidden" id="hdnCompanyName" class="hdnCompanyName" runat="server" value="" />

<div class="panBottomMargin">
    <div class="divRow">
        <div class="divRowDescription">
            <asp:Label ID="lblWSUrlDesc" runat="server" Text="Url de los servicios web de importación de VisualTime"></asp:Label>
        </div>
        <asp:Label ID="lblWSUrl" runat="server" Text="URL:" CssClass="labelForm"></asp:Label>
        <div class="componentForm">
            <a id="txtURLWS" runat="server" href="#" />
        </div>
    </div>

    <div>

        <%--     <roUserControls:roOptionPanelClient ID="rbWSModeV1" runat="server" TypeOPanel="RadioOption" Width="100%" Height="Auto" Checked="False" Enabled="True" CConClick="changeWSMode(1); hasChanges(true);">
            <Title>
                <asp:Label ID="lblWSModeV1" runat="server" Text="V1: Usar usuario y contraseña como medio de identificación"></asp:Label>
            </Title>
            <Description>
            </Description>
            <Content>--%>
        <br />
        <div style="font-weight: 600 !important; margin-left: 15px;">
            <asp:Label ID="lblWSModeV1" runat="server" Text="V1: Usar usuario y contraseña como medio de identificación"></asp:Label>
        </div>
        <br />
        <div class="divRow">
            <div class="divRowDescription">
                <asp:Label ID="lblWSUsernameConfigDesc" runat="server" Text="Nombre de usuario del utilizado para realizar login en el servicio web de importación"></asp:Label>
            </div>
            <asp:Label ID="lblWSUsernameConfig" runat="server" Text="Nombre de usuario:" CssClass="labelForm"></asp:Label>
            <div class="componentForm">
                <dx:ASPxTextBox ID="txtWSUserName" runat="server" Width="350px" ClientInstanceName="txtEmpName_Client" NullText="_____">
                    <ClientSideEvents TextChanged="function(s,e){hasChanges(true)}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                    <ValidationSettings SetFocusOnError="True" ValidationGroup="wsConfigGroup">
                        <RequiredField IsRequired="True" ErrorText="(*)" />
                    </ValidationSettings>
                </dx:ASPxTextBox>
            </div>
        </div>

        <div class="divRow">
            <div class="divRowDescription">
                <asp:Label ID="lblWSOldPasswordDesc" runat="server" Text="Indique la contraseña antigua para poder guardar las modificaciones realizadas."></asp:Label>
            </div>
            <asp:Label ID="lblWSOldPassword" runat="server" Text="Contraseña antigua:" CssClass="labelForm"></asp:Label>
            <div class="componentForm">
                <dx:ASPxTextBox ID="txtWSOldPassword" runat="server" Width="350px" ClientInstanceName="txtWSOldPassword_Client" Password="true" NullText="" AutoCompleteType="Disabled">
                    <ClientSideEvents TextChanged="function(s,e){hasChanges(true)}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                </dx:ASPxTextBox>
            </div>
        </div>

        <div class="divRow">
            <div class="divRowDescription">
                <asp:Label ID="lblWSPasswordConfigDesc" runat="server" Text="Contraseña utilizada para realizar login en el servicio web de importación"></asp:Label>
            </div>
            <asp:Label ID="lblWSPasswordConfig" runat="server" Text="Contraseña:" CssClass="labelForm"></asp:Label>
            <div class="componentForm">
                <dx:ASPxTextBox ID="txtWSPassword" runat="server" Width="350px" ClientInstanceName="txtWSPassword_Client" Password="true" NullText="" AutoCompleteType="Disabled">
                    <ClientSideEvents TextChanged="function(s,e){hasChanges(true)}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                    <ValidationSettings SetFocusOnError="True" ValidationGroup="wsConfigGroup">
                        <RequiredField IsRequired="True" ErrorText="(*)" />
                    </ValidationSettings>
                </dx:ASPxTextBox>
            </div>
        </div>

        <div class="divRow">
            <div class="divRowDescription">
                <asp:Label ID="lblWSRepeatPasswordConfigDesc" runat="server" Text="Repita la contraseña indicada anteriormente"></asp:Label>
            </div>
            <asp:Label ID="lblWSRepeatPasswordConfig" runat="server" Text="Repita:" CssClass="labelForm"></asp:Label>
            <div class="componentForm">
                <dx:ASPxTextBox ID="txtWSPasswordRepeat" runat="server" Width="350px" ClientInstanceName="txtWSPasswordRepeat_Client" Password="true" NullText="" AutoCompleteType="Disabled">
                    <ClientSideEvents TextChanged="function(s,e){hasChanges(true)}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                    <ValidationSettings SetFocusOnError="True" ValidationGroup="wsConfigGroup">
                        <RequiredField IsRequired="True" ErrorText="(*)" />
                    </ValidationSettings>
                </dx:ASPxTextBox>
            </div>
        </div>

        <%--   </Content>
        </roUserControls:roOptionPanelClient>--%>

        <%--    <roUserControls:roOptionPanelClient ID="rbWSModeV2" runat="server" TypeOPanel="RadioOption" Width="100%" Height="Auto" Checked="False" Enabled="True" CConClick="changeWSMode(2); hasChanges(true);">
            <Title>
                <asp:Label ID="lblWSModeV2" runat="server" Text="V2: Usar un token identificación"></asp:Label>
            </Title>
            <Description>
            </Description>
            <Content>--%>
        <br />
        <div style="font-weight: 600 !important; margin-left: 15px;">
            <asp:Label ID="lblWSModeV2" runat="server" Text="V2: Usar un token como medio de identificación"></asp:Label>
        </div>
        <br />

        <div class="divRow">
            <div class="divRowDescription">
                <asp:Label ID="lblWSToken1Desc" runat="server" Text="Token utilizado para realizar login en el servicio web de importación"></asp:Label>
            </div>
            <asp:Label ID="lblWSToken1Config" runat="server" Text="Token primario:" CssClass="labelForm"></asp:Label>
            <div class="componentForm">

                <div id="wrapper">
                    <div id="div1" style="display: inline-block;">
                        <dx:ASPxTextBox ID="txtWSToken1" runat="server" Width="650px" ClientInstanceName="txtWSToken1_Client">
                            <ClientSideEvents TextChanged="function(s,e){hasChanges(true)}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                        </dx:ASPxTextBox>
                    </div>
                    <div id="div2" style="display: inline-block; vertical-align: top;">
                        <div id="generateToken1" runat="server" class="btnFlat" style="padding: 1px 5px 1px 5px !important">
                            <a href="javascript: void(0)" id="btnGenerateToken1" runat="server" onclick="generateToken(1);">
                                <span class="btnIconAdd"></span>
                                <asp:Label ID="lblGenerateToken1" runat="server" Text="Generar Token Primario"></asp:Label>
                            </a>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div class="divRow">
            <div class="divRowDescription">
                <asp:Label ID="lblWSToken2Desc" runat="server" Text="Segundo token utilizado para realizar login en el servicio web de importación"></asp:Label>
            </div>
            <asp:Label ID="lblWSToken2Config" runat="server" Text="Token secundario:" CssClass="labelForm"></asp:Label>
            <div class="componentForm">
                <div id="wrapperT2">
                    <div id="div1T2" style="display: inline-block;">

                        <dx:ASPxTextBox ID="txtWSToken2" runat="server" Width="650px" ClientInstanceName="txtWSToken2_Client">
                            <ClientSideEvents TextChanged="function(s,e){hasChanges(true)}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                        </dx:ASPxTextBox>
                    </div>
                    <div id="div2T2" style="display: inline-block; vertical-align: top;">
                        <div id="generateToken2" runat="server" class="btnFlat" style="padding: 1px 5px 1px 5px !important">
                            <a href="javascript: void(0)" id="btnGenerateToken2" runat="server" onclick="generateToken(2)">
                                <span class="btnIconAdd"></span>
                                <asp:Label ID="lblGenerateToken2" runat="server" Text="Generar Token Secundario"></asp:Label>
                            </a>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <%--         </Content>
        </roUserControls:roOptionPanelClient>--%>
    </div>

    <br />
    <div class="divRow">
        <div class="divRowDescription" style="padding-left: 0 !important">
            <asp:Label ID="lblWSIPsConfigDesc" runat="server" Text="Indique el listado de ip's habilitadas para realizar peticiones al servicio web. (Dejar en blanco para no aplicar restricciones)"></asp:Label>
        </div>
        <div style="text-align: right; float: left; margin-top: 5px;">
            <asp:Label ID="lblWSIPsConfig" runat="server" Text="IPs:"></asp:Label>
        </div>
        <div class="componentForm" style="width: calc(100% - 25px) !important">
            <table width="100%" style="position: relative; top: -35px;">
                <tr>
                    <td style="width: auto; text-align: right; padding-left: 20px; padding-right: 20px;">
                        <div id="tableAddIP" runat="server" class="btnFlat">
                            <a href="javascript: void(0)" id="btnAddIPs" runat="server" onclick="EditAllowedIP(true, null);">
                                <span class="btnIconAdd"></span>
                                <asp:Label ID="lblAddIP" runat="server" Text="Añadir IP"></asp:Label>
                            </a>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td>
                        <input type="hidden" runat="server" id="txtAllowedIPs" clientidmode="Static" />
                        <div id="gridAllowedIPs" runat="server" clientidmode="Static" style="height: 100px; overflow: auto;">
                            <!-- grid de IPs -->
                        </div>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <!-- form Compositions -->
    <roForms:frmIPs ID="frmIPs1" runat="server" />
</div>
<div id="divBIInformation" runat="server" >
<div class="panHeader2">
    <span style="">
        <asp:Label runat="server" ID="lblWsBItGeneral" Text="Configuración del contenedor de exportación a BI."></asp:Label></span>
</div>
<div class="panelHeaderContent" style="padding-top: 25px">
    <div class="panelDescriptionImage">
        <div class="biConfigSection" runat="server" style="height: 48px;"></div>
    </div>
    <div class="panelDescriptionText">
        <asp:Label ID="lblBIGeneralDesc" runat="server" Text="En esta sección puede generar el enlace SAS al contenedor de ficheros de exportación de BI."></asp:Label>
    </div>
</div>

<input type="hidden" id="Hidden1" class="hdnCompanyName" runat="server" value="" />

<div class="panBottomMargin">

    <div>
        <br />

        <div class="divRow">
            <div class="divRowDescription">
                <asp:Label ID="lblBISaSLinkDesc" runat="server" Text="Enlace a través del cual se puede acceder a todas las ejecuciones exportables de Genius realizadas por todos los usuarios"></asp:Label>
            </div>
            <asp:Label ID="lblBISasLinkConfig" runat="server" Text="Enlace SaS:" CssClass="labelForm"></asp:Label>
            <div class="componentForm">

                <div id="wrapper">
                    <div id="div1" style="display: inline-block;">
                        <dx:ASPxTextBox ID="txtBISaSLink" runat="server" Width="650px" ClientInstanceName="txtBISaSLink_Client">
                            <ClientSideEvents TextChanged="function(s,e){hasChanges(true)}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                        </dx:ASPxTextBox>
                    </div>
                    <div id="div2" style="display: inline-block; vertical-align: top;">
                        <div id="generateSaSLinkBI" runat="server" class="btnFlat" style="padding: 1px 5px 1px 5px !important">
                            <a href="javascript: void(0)" id="A1" runat="server" onserverclick="generateSaSLinkBI_ServerClick">
                                <span class="btnIconAdd"></span>
                                <asp:Label ID="lblGenerateSaSLinkBI" runat="server" Text="Generar enlace SaS"></asp:Label>
                            </a>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <br />
        <div class="divRow">
            <asp:Label ID="lblContainerURL" runat="server" Text="Enlace al contenedor:" CssClass="labelForm"></asp:Label>
            <div class="componentForm">

                <div id="wrapper">
                    <div id="div1" style="display: inline-block;">
                        <dx:ASPxTextBox ID="txtContainerURL" runat="server" Width="650px" ClientInstanceName="txtBISaSLink_Client" Enabled="false">
                        </dx:ASPxTextBox>
                    </div>
                </div>
            </div>
        </div>
        <br />
        <div class="divRow">
            <asp:Label ID="lblSaSToken" runat="server" Text="Token:" CssClass="labelForm"></asp:Label>
            <div class="componentForm">

                <div id="wrapper">
                    <div id="div1" style="display: inline-block;">
                        <dx:ASPxTextBox ID="txtSaSToken" runat="server" Width="650px" ClientInstanceName="txtBISaSLink_Client" Enabled="false">
                        </dx:ASPxTextBox>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>
</div>
<div class="panBottomMargin">
    <div class="panHeader2">
        <span style="">
            <asp:Label runat="server" ID="lblTitleSC" Text="Visualtime Secure Connect"></asp:Label></span>
    </div>
    <div class="panelHeaderContent" style="padding-top: 25px">
        <div class="panelDescriptionImage">
            <div class="wsConfigSection" runat="server" style="height: 48px;"></div>
        </div>
        <div class="panelDescriptionText">
            <asp:Label ID="lblSubTitleSC" runat="server" Text="Tokens para el servicio de Visualtime Secure Connect"></asp:Label>
        </div>
    </div>
    <div class="panBottomMargin" style="margin-top: -35px">
        <div class="divRow">
            <div class="divRowDescription">
                <asp:Label ID="lblClientNameSC" runat="server" Text="Nombre del cliente para la conexión"></asp:Label>
            </div>
            <asp:Label ID="lblClientName2SC" runat="server" Text="ClientId:" CssClass="labelForm"></asp:Label>
            <div class="componentForm">

                <div id="wrapper2">
                    <div id="div12" style="display: inline-block;">
                        <dx:ASPxTextBox ID="txtWSSCClientName" runat="server" Width="650px" ReadOnly="true" ClientInstanceName="txtWSSCClientName_Client">
                            <ClientSideEvents TextChanged="function(s,e){}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                        </dx:ASPxTextBox>
                    </div>
                </div>
            </div>
        </div>
        <div class="divRow">
            <div class="divRowDescription">
                <asp:Label ID="lblToken1DescSC" runat="server" Text="Token primario utilizado para Secure Connect"></asp:Label>
            </div>
            <asp:Label ID="lblToken1SC" runat="server" Text="Token primario:" CssClass="labelForm"></asp:Label>
            <div class="componentForm">

                <div id="wrapper2">
                    <div id="div12" style="display: inline-block;">
                        <dx:ASPxTextBox ID="txtWSSCToken1" runat="server" Width="650px" ReadOnly="true" ClientInstanceName="txtWSSCToken1_Client">
                            <ClientSideEvents TextChanged="function(s,e){}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                        </dx:ASPxTextBox>
                    </div>
                    <div id="div22" style="display: inline-block; vertical-align: top;">
                        <div id="Div3" runat="server" class="btnFlat" style="padding: 1px 5px 1px 5px !important">
                            <a href="javascript: void(0)" id="btnGenerateTokenSC1" runat="server" onserverclick="generateTokenSC">
                                <span class="btnIconAdd"></span>
                                <asp:Label ID="lblGenerateToken1SC" runat="server" Text="Generar Token Primario"></asp:Label>
                            </a>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div class="divRow">
            <div class="divRowDescription">
                <asp:Label ID="lblToken2DescSC" runat="server" Text="Token secundario utilizado para Secure Connect"></asp:Label>
            </div>
            <asp:Label ID="lblToken2SC" runat="server" Text="Token secundario:" CssClass="labelForm"></asp:Label>
            <div class="componentForm">
                <div id="wrapperT22">
                    <div id="div1T22" style="display: inline-block;">

                        <dx:ASPxTextBox ID="txtWSSCToken2" runat="server" Width="650px" ReadOnly="true" ClientInstanceName="txtWSSCToken2_Client">
                            <ClientSideEvents TextChanged="function(s,e){}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                        </dx:ASPxTextBox>
                    </div>
                    <div id="div2T22" style="display: inline-block; vertical-align: top;">
                        <div id="Div4" runat="server" class="btnFlat" style="padding: 1px 5px 1px 5px !important">
                            <a href="javascript: void(0)" id="btnGenerateTokenSC2" runat="server" onserverclick="generateTokenSC">
                                <span class="btnIconAdd"></span>
                                <asp:Label ID="lblGenerateToken2SC" runat="server" Text="Generar Token Secundario"></asp:Label>
                            </a>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="divRow">
            <div class="divRowDescription">
                <asp:Label ID="lblErrorSC" runat="server" Style="color: red !important;" Visible="false" Text="No se han podido obtener los datos para la conexión del SC."></asp:Label>
            </div>
        </div>
    </div>
</div>

<div class="panHeader2">
    <span style="">
        <asp:Label runat="server" ID="lblTitleDI" Text="Configuración del contenedor de tickets comedor."></asp:Label></span>
</div>
<div class="panelHeaderContent" style="padding-top: 25px">
    <div class="panelDescriptionImage">
        <div class="diConfigSection" runat="server" style="height: 48px;"></div>
    </div>
    <div class="panelDescriptionText">
        <asp:Label ID="lblSubTitleDI" runat="server" Text="En esta sección puede generar el enlace SAS al contenedor de tickets comedor."></asp:Label>
    </div>
</div>
<div class="panBottomMargin" style="margin-top: -35px">
    <div class="divRow">
        <div class="divRowDescription">
            <asp:Label ID="lblTokenDescription" runat="server" Text="SaS Token que se debe poner en la configuración del Servicio Comedor."></asp:Label>
        </div>
        <asp:Label ID="lblDISaSToken" runat="server" Text="Token:" CssClass="labelForm"></asp:Label>
        <div class="componentForm">

            <div id="wrapper2">
                <div id="div12" style="display: inline-block;">
                    <dx:ASPxTextBox ID="txtDISaSToken" runat="server" Width="650px" ReadOnly="true" ClientInstanceName="txtDISaSToken_Client">
                        <ClientSideEvents TextChanged="function(s,e){}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                    </dx:ASPxTextBox>
                </div>
                <div id="div22" style="display: inline-block; vertical-align: top;">
                    <div id="generateSaSLink" runat="server" class="btnFlat" style="padding: 1px 5px 1px 5px !important">
                        <a href="javascript: void(0)" id="btnGenerateSaSLink" runat="server" onserverclick="generateSaSLink_ServerClick">
                            <span class="btnIconAdd"></span>
                            <asp:Label ID="lblGenerateSaSLink" runat="server" Text="Generar enlace SaS"></asp:Label>
                        </a>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>

<div class="panHeader2">
    <span style="">
        <asp:Label runat="server" ID="lblTitlePGP" Text="Encriptación PGP"></asp:Label>
    </span>
</div>
<div class="panelHeaderContent" style="padding-top: 25px">
    <div class="panelDescriptionImage">
        <div class="pgpConfigSection" runat="server" style="height: 48px;"></div>
    </div>
    <div class="panelDescriptionText">
        <asp:Label ID="lblSubtitlePGP" runat="server" Text="En esta seccion podrás configurar encriptación automática de archivos mediante PGP para proteger los datos importados y exportados en Visualtime."></asp:Label>
    </div>
</div>
<div class="panBottomMargin" style="margin-top: -35px"  align-items: center;>
    <div class="divRow">
        <div class="divRowDescription">
            <asp:Label ID="lblPGPEnabledDescription" runat="server" Text="Activa la encriptación automática de archivos mediante PGP."></asp:Label>
        </div>
          <div class="componentForm" style="padding-left: 130px">
            <dx:ASPxCheckBox ID="chkPGPEnabled" runat="server" Checked="false" Text="Encriptación PGP" ClientInstanceName="chkPGPEnabledClient" CssClass="chkForm">
               <ClientSideEvents CheckedChanged="function(s,e){ 
                   hasChanges(true);
                   btnAddCertificate_Client.SetClientVisible(s.GetChecked());
               }" />
       </dx:ASPxCheckBox>
         </div>
    </div>
    <div class="divRow">
        <div class="divRowDescription">
            <asp:Label ID="lblVisualTimePGPKeyDescription" runat="server" Text="Clave pública Visualtime."></asp:Label>
        </div>
        <div class="componentForm" style="padding-left: 130px">
            <div id="wrapper2">
                <dx:ASPxButton ID="btnDownloadCertificate" runat="server" AutoPostBack="False" CausesValidation="False" Text="Descargar" ToolTip="" HoverStyle-CssClass="btnFlat-hover" CssClass="btnFlat" ClientInstanceName="btnDownloadCertificate_Client" ClientVisible="False">
                    <Image Url="~/Base/Images/Grid/add.png"></Image>
                </dx:ASPxButton>
            </div>
        </div>
    </div>
    
    <div class="divRow">
        <div class="divRowDescription">
            <asp:Label ID="lblVisualTimeClientPGPKeyDescription" runat="server" Text="Clave pública cliente."></asp:Label>
        </div>
        <div class="componentForm" style="padding-left: 130px">
            <dx:ASPxButton ID="btnAddCertificate" runat="server" AutoPostBack="False" CausesValidation="False" Text="Añadir" ToolTip="Añadir tu clave pública" HoverStyle-CssClass="btnFlat-hover" CssClass="btnFlat" ClientInstanceName="btnAddCertificate_Client" ClientVisible="False">
                <Image Url="~/Base/Images/Grid/add.png"></Image>
            </dx:ASPxButton>
                     <asp:Label ID="hdnFileUploaded" runat="server" Text="Ya dispone de una Clave Pública" ClientVisible="False"></asp:Label>
        </div>
    </div>
</div>

<div class="panHeader2">
    <span style="">
        <asp:Label runat="server" ID="lblExternComplaintChannel" Text="Acceso al canal de denuncias externo"></asp:Label></span>
</div>
<div class="panelHeaderContent" style="padding-top: 25px">
    <div class="panelDescriptionImage">
        <div class="wsConfigSection" runat="server" style="height: 48px;"></div>
    </div>
    <div class="panelDescriptionText">
        <asp:Label ID="lblExternComplaintChannelDesc" runat="server" Text="En esta sección puede consultar la dirección de acceso al canal de denuncias externo"></asp:Label>
    </div>
</div>

<div class="panBottomMargin">
    <div class="divRow">
        <div class="divRowDescription">
            <asp:Label ID="lblExternComplaintChannelUrlDesc" runat="server" Text="Url del canal de denuncias externo"></asp:Label>
        </div>
        <asp:Label ID="lblExternComplaintChannelUr" runat="server" Text="URL:" CssClass="labelForm"></asp:Label>
        <div class="componentForm">
            <a id="lnkExternComplaintChannel" runat="server" href="#" />
        </div>
    </div>
</div>

<div class="panBottomMargin">
    <div class="panHeader2">
        <span style="">
            <asp:Label runat="server" ID="lblWsAudit" Text="Auditoria del servicio web de importación."></asp:Label></span>
    </div>
    <div class="panelHeaderContent" style="padding-top: 25px">
        <div class="panelDescriptionImage">
            <div class="wsAuditSection" runat="server" style="height: 48px;"></div>
        </div>
        <div class="panelDescriptionText">
            <asp:Label ID="lblWsAuditDesc" runat="server" Text="En esta sección puede revisar la auditoría de los últimos 7 días del servicio web de importación de datos. Si requiere ampliar el periodo puede consultar los datos desde la auditoría completa de VisualTime."></asp:Label>
        </div>
    </div>

    <div class="panBottomMargin" style="padding-top: 15px;">
        <div class="jsGrid">
            <div class="jsgridButton">
                <dx:ASPxButton ID="btnRefresh" runat="server" AutoPostBack="False" CausesValidation="False" Text="Refrescar" ToolTip="Refrescar" HoverStyle-CssClass="btnFlat-hover" CssClass="btnFlat">
                    <Image Url="~/Base/Images/Grid/button_reload.png"></Image>
                    <ClientSideEvents Click="btnRefreshClient_Click" />
                </dx:ASPxButton>
            </div>
        </div>
        <div class="jsGridContent">
            <dx:ASPxGridView ID="grdAudit" runat="server" AutoGenerateColumns="False" Width="99%" ClientInstanceName="grdAuditClient">
                <Settings ShowFilterRow="True" ShowGroupPanel="True" VerticalScrollableHeight="175" />
                <SettingsDataSecurity AllowDelete="False" AllowEdit="False" AllowInsert="False" />
                <ClientSideEvents EndCallback="function(s,e) { showLoadingGrid(false); }" />
            </dx:ASPxGridView>
        </div>
    </div>
</div>

<!-- POPUP NEW OBJECT -->
<dx:ASPxPopupControl ID="NewCertificatePopup" runat="server" AllowDragging="False" CloseAction="None" Modal="True" ContentUrl="~/Options/UploadCertificate.aspx"
    PopupVerticalAlign="WindowCenter" PopupHorizontalAlign="WindowCenter" Width="540px" Height="400px"
    ShowHeader="False" ScrollBars="Auto" ShowPageScrollbarWhenModal="True" PopupAnimationType="None" BackColor="Transparent" ContentStyle-Paddings-Padding="0px" Border-BorderColor="Transparent" ShowShadow="false">
    <SettingsLoadingPanel Enabled="false" />
</dx:ASPxPopupControl>