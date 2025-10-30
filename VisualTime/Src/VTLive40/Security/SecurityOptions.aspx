<%@ Page Language="VB" MasterPageFile="~/Base/mastEmp.master" AutoEventWireup="false" Inherits="VTLive40.SecurityOptions" Title="Opciones de seguridad" EnableEventValidation="True" EnableViewState="True" EnableSessionState="True" CodeBehind="SecurityOptions.aspx.vb" %>

<%@ Register Src="~/Security/WebUserControls/frmIPs.ascx" TagName="frmIPs" TagPrefix="roForms" %>

<asp:Content ID="cMainBody" ContentPlaceHolderID="contentMainBody" runat="Server">

    <script language="javascript" type="text/javascript">

        /* funcio per bloquejar sols l'area menu */
        function disableScreen(bol) {
            var divBg = document.getElementById('divModalBgDisabled');
            if (divBg != null) {
                if (bol == true) {
                    document.body.style.overflow = "hidden";
                    divBg.style.height = 2000;  //document.body.offsetHeight;
                    divBg.style.width = 3000;  //document.body.offsetWidth;

                    divBg.style.display = '';
                }
                else {
                    document.body.style.overflow = "";
                    divBg.style.display = 'none';
                }
            }
        }

        function PageBase_Load() {
            ConvertControls();

            // Reestablezco el tab activo
            if ($get('<%= SecurityOptions_TabVisibleName.ClientID %>').value != '') {
                SelectTab($get('<%= SecurityOptions_TabVisibleName.ClientID %>').value);
            }
            else {
                SelectTab('panPasswordOptions');
            }

            top.focus();
        }

        function SelectTab(SelectedTab) {

            // Hacer invisibles los panels
            $get('panPasswordOptions').style.display = 'none';
            $get('panRestrictionsOptions').style.display = 'none';

            // Desmarcar los botones de la barra
            $get('<%= TABBUTTON_PasswordOptions.ClientID %>').className = 'bTab';
            $get('<%= TABBUTTON_RestrictionsOptions.ClientID %>').className = 'bTab';

            var TabID;
            if (SelectedTab == 'panPasswordOptions') {
                TabID = 'panPasswordOptions';
                $get('<%= TABBUTTON_PasswordOptions.ClientID %>').className = 'bTab-active';
            }
            else if (SelectedTab == 'panRestrictionsOptions') {
                TabID = 'panRestrictionsOptions';
                $get('<%= TABBUTTON_RestrictionsOptions.ClientID %>').className = 'bTab-active';
            }

            $get(TabID).style.display = 'block';
            $get('<%= SecurityOptions_TabVisibleName.ClientID %>').value = TabID;

        }
    </script>
    <div id="divModalBgDisabled" class="modalBackground" style="position: absolute; left: 0px; top: 0px; z-index: 990; width: 1680px; height: 900px; display: none;"></div>

    <input type="hidden" runat="server" id="hdnValueGridName" />
    <input type="hidden" id="msgHasChanges" value="" runat="server" clientidmode="Static" />
    <input type="hidden" id="msgHasErrors" value="" runat="server" clientidmode="Static" />

    <div id="divMainBody">
        <!-- TAB SUPERIOR -->
        <div id="divTabInfo" class="divDataCells">
            <div style="min-height: 10px"></div>
            <div id="divAccessGroup" class="blackRibbonTitle">
                <div class="blackRibbonIcon">
                    <asp:Image ID="imgSecurityOptions" ImageUrl="Images/SecurityOptions.png" runat="server" />
                </div>
                <div class="blackRibbonDescription">
                    <asp:Label ID="lblHeader" runat="server" Text="Opciones de seguridad" CssClass="NameText"></asp:Label>
                    <br />
                    <asp:Label ID="lblInfo" runat="server" Text="Esta es la pantalla de opciones de seguridad. Puede configurar diferentes parámetros para minimizar el riesgo de accesos mediante suplantación de identidad."></asp:Label>
                </div>
                <div class="blackRibbonButtons">
                    <table border="0" cellpadding="0" cellspacing="0" width="100%" style="padding: 2px 5px 5px 5px;">
                        <tr>
                            <td id="rowTabButtons3" runat="server" align="right" valign="top" style="width: 140px; padding-top: 0px; padding-right: 5px;">
                                <a id="TABBUTTON_PasswordOptions" href="javascript: void(0);" class="bTab" onclick="SelectTab('panPasswordOptions');" runat="server">
                                    <asp:Label ID="lblPasswordOptionsTabButton" Text="Contraseñas" runat="server" /></a>
                                <a id="TABBUTTON_RestrictionsOptions" href="javascript: void(0);" class="bTab" onclick="SelectTab('panRestrictionsOptions');" runat="server">
                                    <asp:Label ID="lblRestrictionsOptionsTabButton" Text="Restricciones" runat="server" /></a>
                            </td>
                        </tr>
                    </table>
                </div>
            </div>
            <div style="min-height: 10px"></div>
        </div>
        <!-- FIN TAB SUPERIOR -->
        <!-- DETALLE -->
        <div id="divTabData" class="divDataCells">
            <div id="divContenido" class="divAllContent">
                <div id="divContent" style="height: initial;" class="maxHeight">
                    <asp:UpdatePanel ID="upBody" runat="server">
                        <ContentTemplate>
                            <div style="margin: 5px">
                                <table cellpadding="0" cellspacing="0" style="width: 100%; height: 100%; padding-left: 10px; padding-right: 10px;">
                                    <tr>
                                        <td valign="top" style="padding-top: 2px;">
                                            <!-- Mensajes -->
                                            <div id="divMsgTop" class="divMsg" style="display: none;">
                                                <table style="width: 100%;">
                                                    <tr>
                                                        <td align="center" style="width: 20px; height: 16px;">
                                                            <img id="Img1" src="~/Base/Images/MessageFrame/Alert16.png" runat="server" /></td>
                                                        <td align="left" style="padding-left: 10px; color: white;"><span id="msgTop"></span></td>
                                                        <td align="right" style="color: White; padding-right: 10px;">
                                                            <a href="javascript: void(0);" class="aMsg" onclick="saveChanges();"><span id="lblSaveChanges" runat="server" /></a>
                                                            &nbsp;&nbsp;&nbsp;|&nbsp;&nbsp;&nbsp;
                                                            <a href="javascript: void(0);" onclick="undoChanges();" class="aMsg"><span id="lblUndoChanges" runat="server" /></a>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </div>
                                        </td>
                                    </tr>

                                    <tr>
                                        <td valign="top">

                                            <div id="panPasswordOptions" style="width: 100%;">
                                                <table id="tbPasswordOptions" runat="server" cellpadding="0" cellspacing="0" width="100%">
                                                    <tr>
                                                        <td>
                                                            <div class="panHeader2">
                                                                <span style="">
                                                                    <asp:Label ID="lblPasswordConfiguration" runat="server" Text="Configuración de contraseñas" Font-Bold="true" /></span>
                                                            </div>
                                                            <br />
                                                        </td>
                                                    </tr>
                                                    <tr style="padding-top: 5px">
                                                        <td style="padding-left: 20px">
                                                            <asp:Label ID="lblPasswordConfigurationDescription" Text="En esta sección puede definir el nivel de seguridad de la contraseña, indicar las contraseñas que se almacenan y si caduca o no." runat="server"></asp:Label>
                                                            <br />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td style="padding-left: 20px;">
                                                            <roUserControls:roGroupBox ID="groupPassword" runat="server">
                                                                <Content>
                                                                    <table>
                                                                        <tr>
                                                                            <td>
                                                                                <table>
                                                                                    <tr>
                                                                                        <td>
                                                                                            <asp:Label ID="lblHistoricPassword" runat="server" Text="Número de contraseñas previas a recordar:" Font-Bold="true"></asp:Label>
                                                                                        </td>
                                                                                        <td colspan="2">
                                                                                            <input type="text" runat="server" id="txtHistoricPassword" style="text-align: center; text-align: center; width: 40px; border-radius: 5px;"
                                                                                                class="textEdit x-form-text x-form-field" convertcontrol="TextField"
                                                                                                ccregex="/^([0-9]?[0-9])$/" ccminlength="1" ccmaxlength="2" cctime="false" cconchange="hasChanges(true);" ccallowblank="false" />
                                                                                        </td>
                                                                                        <td>
                                                                                            <asp:Label ID="lblHistoricPasswor2" runat="server" Text="anteriores" Font-Bold="true"></asp:Label>
                                                                                        </td>
                                                                                    </tr>
                                                                                </table>
                                                                            </td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td>
                                                                                <asp:Label ID="lblHistoricPasswordDesc" CssClass="OptionPanelDescStyle" Style="padding-left: 0px" runat="server" Text="Se validará que la contraseña no sea igual que ninguna de las anteriores en caso de utilizar el nivel medio o alto de seguridad"></asp:Label><br />
                                                                                &nbsp;</td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td>
                                                                                <table>
                                                                                    <tr>
                                                                                        <td>
                                                                                            <asp:Label ID="lblOutdate" runat="server" Text="La contraseña caduca cada " Font-Bold="true"></asp:Label>
                                                                                        </td>
                                                                                        <td colspan="2">
                                                                                            <input type="text" runat="server" id="txtPasswordOutdated" style="text-align: center; text-align: center; width: 40px; border-radius: 5px;"
                                                                                                class="textEdit x-form-text x-form-field" convertcontrol="TextField"
                                                                                                ccregex="/^([0-9]?[0-9]?[0-9])$/" ccmaxlength="3" cctime="false" cconchange="hasChanges(true);" ccallowblank="false" />
                                                                                        </td>
                                                                                        <td>
                                                                                            <asp:Label ID="lblOutdateDef" runat="server" Text="dias (0 para desactivar)" Font-Bold="true"></asp:Label>
                                                                                        </td>
                                                                                    </tr>
                                                                                </table>
                                                                            </td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td>
                                                                                <asp:Label ID="lblPasswordOutDateDesc" CssClass="OptionPanelDescStyle" Style="padding-left: 0px" runat="server" Text="Pasado este número de días, cuando el usuario vuelva a iniciar sesión se le obligará a cambiar su contraseña"></asp:Label><br />
                                                                                &nbsp;</td>
                                                                        </tr>
                                                                    </table>
                                                                </Content>
                                                            </roUserControls:roGroupBox>
                                                        </td>
                                                    </tr>

                                                    <tr>
                                                        <td>
                                                            <br />
                                                            <br />
                                                            <div class="panHeader2">
                                                                <span style="">
                                                                    <asp:Label ID="lblBlockAccount" runat="server" Text="Bloqueo de cuenta" Font-Bold="true" /></span>
                                                            </div>
                                                            <br />
                                                        </td>
                                                    </tr>
                                                    <tr style="padding-top: 5px">
                                                        <td style="padding-left: 20px">
                                                            <asp:Label ID="lblBlockAccountDescription" Text="En esta sección puede definir el número de accesos erroneos que se permiten antes de bloquear la cuenta." runat="server"></asp:Label>
                                                            <br />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td style="padding-left: 20px;">
                                                            <roUserControls:roGroupBox ID="groupBlock" runat="server">
                                                                <Content>
                                                                    <table>
                                                                        <tr>
                                                                            <td>
                                                                                <asp:Label ID="lblBlockTitle" runat="server" Text="Bloqueo de cuenta" Font-Bold="true"></asp:Label></td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td>
                                                                                <table>
                                                                                    <tr>
                                                                                        <td>
                                                                                            <asp:Label ID="lblBlock1" runat="server" Text="Si se producen"></asp:Label>
                                                                                        </td>
                                                                                        <td>
                                                                                            <input type="text" runat="server" id="txtBlock1Attempts" style="text-align: center; text-align: center; width: 40px; border-radius: 5px;"
                                                                                                class="textEdit x-form-text x-form-field" convertcontrol="TextField"
                                                                                                ccregex="/^([0-9]?[0-9])$/" ccmaxlength="2" cctime="false" cconchange="hasChanges(true);" ccallowblank="false" />
                                                                                        </td>
                                                                                        <td>
                                                                                            <asp:Label ID="lblBlock1Desc" runat="server" Text="intentos erroneos de acceso, la cuenta se bloqueará temporalmente durante 10 minutos. (se enviará un correo al usuario indicando el bloqueo)"></asp:Label>
                                                                                        </td>
                                                                                    </tr>
                                                                                    <tr>
                                                                                        <td>
                                                                                            <asp:Label ID="lblBlock2" runat="server" Text="Si se producen"></asp:Label>
                                                                                        </td>
                                                                                        <td>
                                                                                            <input type="text" runat="server" id="txtBlock2Attempts" style="text-align: center; text-align: center; width: 40px; border-radius: 5px;"
                                                                                                class="textEdit x-form-text x-form-field" convertcontrol="TextField"
                                                                                                ccregex="/^([0-9]?[0-9])$/" ccmaxlength="2" cctime="false" cconchange="hasChanges(true);" ccallowblank="false" />
                                                                                        </td>
                                                                                        <td>
                                                                                            <asp:Label ID="lblBlock2Desc" runat="server" Text="intentos erroneos de acceso, la cuenta se bloqueará indefinidamente hasta que un administrador la desbloquee. (Se enviará un correo al usuario indicando el bloqueo)"></asp:Label>
                                                                                        </td>
                                                                                    </tr>
                                                                                </table>
                                                                            </td>
                                                                        </tr>
                                                                    </table>
                                                                </Content>
                                                            </roUserControls:roGroupBox>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </div>

                                            <div id="panRestrictionsOptions" style="width: 100%; display: none;">
                                                <table id="tbRestrictionsOptions" runat="server" cellpadding="0" cellspacing="0" width="100%">
                                                    <tr>
                                                        <td>
                                                            <div class="panHeader2">
                                                                <span style="">
                                                                    <asp:Label ID="lblGlobalAccessRestriction" runat="server" Text="Restricciones de acceso globales" Font-Bold="true" /></span>
                                                            </div>
                                                            <br />
                                                        </td>
                                                    </tr>
                                                    <tr style="padding-top: 5px">
                                                        <td style="padding-left: 20px">
                                                            <asp:Label ID="lblGlobalAccesRestrictionDescription" Text="En este formulario se definen las restricciones de acceso globales a la aplicación y bloqueos totales en caso de fallos de seguridad." runat="server"></asp:Label>
                                                            <br />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td style="padding-left: 20px;">
                                                            <roUserControls:roGroupBox ID="groupAccessRestrictions" runat="server">
                                                                <Content>
                                                                    <table>
                                                                        <tr>

                                                                            <td style="vertical-align: top; width: 60%; padding-right: 5px">
                                                                                <div>
                                                                                    <roUserControls:roOptionPanelClient ID="ckRequiereKey" runat="server" TypeOPanel="CheckboxOption" Width="100%" Height="Auto" Checked="False" Enabled="True">
                                                                                        <Title>
                                                                                            <asp:Label ID="lblRequiereKey" runat="server" Text="En IPs nuevas solicitar confirmación mediante clave temportal"></asp:Label>
                                                                                        </Title>
                                                                                        <Description>
                                                                                            <asp:Label ID="lblRequiereKeyDesc" runat="server" Text="La clave temporal estará activa durante los 10 minutos siguientes al envío de la misma"></asp:Label>
                                                                                        </Description>
                                                                                        <Content>
                                                                                            <table id="tbRequiereKey" runat="server" cellpadding="0" cellspacing="5" style="height: 75px;">
                                                                                                <tr>
                                                                                                    <td colspan="4">
                                                                                                        <asp:Label ID="lblSaveIp" Text="Guardar la IP autorizada para un usuario concreto:" runat="server"></asp:Label>
                                                                                                    </td>
                                                                                                </tr>
                                                                                                <tr>
                                                                                                    <td align="left" valign="middle" style="width: 10px;">&nbsp;</td>
                                                                                                    <td>
                                                                                                        <asp:Label ID="lblElapsed" runat="server" Text="Duración"></asp:Label>
                                                                                                    </td>
                                                                                                    <td>
                                                                                                        <input type="text" runat="server" id="txtSaveKeyTime" style="text-align: center; text-align: center; width: 40px; border-radius: 5px;"
                                                                                                            class="textEdit x-form-text x-form-field" convertcontrol="TextField"
                                                                                                            ccregex="/^([0-9]?[0-9])$/" ccmaxlength="2" cctime="false" ccallowblank="false" cconchange="hasChanges(true);" />
                                                                                                    </td>
                                                                                                    <td>
                                                                                                        <asp:Label ID="lblDays" runat="server" Text="dias"></asp:Label>
                                                                                                    </td>
                                                                                                </tr>
                                                                                                <tr>
                                                                                                    <td align="left" valign="middle" style="width: 10px;">&nbsp;</td>
                                                                                                    <td>
                                                                                                        <asp:Label ID="lblTimes" runat="server" Text="Hasta que haya accedido desde"></asp:Label>
                                                                                                    </td>
                                                                                                    <td>
                                                                                                        <input type="text" runat="server" id="txtAccessDiferentIps" style="text-align: center; text-align: center; width: 40px; border-radius: 5px;"
                                                                                                            class="textEdit x-form-text x-form-field" convertcontrol="TextField"
                                                                                                            ccregex="/^([0-9]?[0-9])$/" ccmaxlength="2" cctime="false" ccallowblank="false" cconchange="hasChanges(true);" />
                                                                                                    </td>
                                                                                                    <td>
                                                                                                        <asp:Label ID="lblDifferentIps" runat="server" Text="IPs distintas a esta"></asp:Label>
                                                                                                    </td>
                                                                                                </tr>
                                                                                            </table>
                                                                                        </Content>
                                                                                    </roUserControls:roOptionPanelClient>
                                                                                </div>
                                                                                <div style="padding-top: 5px">
                                                                                    <roUserControls:roOptionPanelClient ID="chkVersion" runat="server" TypeOPanel="CheckboxOption" Width="100%" Height="Auto" Checked="False" Enabled="True">
                                                                                        <Title>
                                                                                            <asp:Label ID="lblAppCheckVersion" runat="server" Text="Únicamente si están usando la misma versión de la App que en servidor de VisualTime"></asp:Label>
                                                                                        </Title>
                                                                                        <Description>
                                                                                            <asp:Label ID="lblAppCheckVersionDesc" runat="server" Text=""></asp:Label>
                                                                                        </Description>
                                                                                        <Content>
                                                                                        </Content>
                                                                                    </roUserControls:roOptionPanelClient>
                                                                                </div>
                                                                            </td>
                                                                            <td style="vertical-align: top; width: 40%">
                                                                                <roUserControls:roOptionPanelClient ID="ChkRestrictedIP" runat="server" TypeOPanel="CheckboxOption" Width="100%" Height="Auto" Checked="False" Enabled="True" CConClick="ChangeRestrictedIP()">
                                                                                    <Title>
                                                                                        <asp:Label ID="lblOnlyIps" runat="server" Text="Listado de IPs"></asp:Label>
                                                                                    </Title>
                                                                                    <Description>
                                                                                        <asp:Label ID="lblOnlyIpsDesc" runat="server" Text="Solo las IPs incluidas en la lista podran acceder a VisualTime"></asp:Label>
                                                                                    </Description>
                                                                                    <Content>
                                                                                        <table width="100%">
                                                                                            <tr>
                                                                                                <td style="width: auto; text-align: right; padding-left: 20px; padding-right: 20px;">
                                                                                                    <div class="btnFlat">
                                                                                                        <a href="javascript: void(0)" id="btnAddIPs" runat="server" onclick="EditAllowedIP(true, null);">
                                                                                                            <span class="btnIconAdd"></span>
                                                                                                            <asp:Label ID="lblAddIP" runat="server" Text="Añadir IP"></asp:Label>
                                                                                                        </a>
                                                                                                    </div>
                                                                                                </td>
                                                                                            </tr>
                                                                                            <tr>
                                                                                                <td>
                                                                                                    <input type="hidden" runat="server" id="txtAllowedIPs" />
                                                                                                    <div id="gridAllowedIPs" runat="server" style="height: 100px; overflow: auto;">
                                                                                                        <!-- grid de IPs -->
                                                                                                    </div>

                                                                                                    <!-- form Compositions -->
                                                                                                    <roForms:frmIPs ID="frmIPs1" runat="server" />
                                                                                                </td>
                                                                                            </tr>
                                                                                        </table>
                                                                                    </Content>
                                                                                </roUserControls:roOptionPanelClient>
                                                                            </td>
                                                                        </tr>
                                                                    </table>
                                                                </Content>
                                                            </roUserControls:roGroupBox>
                                                        </td>
                                                    </tr>

                                                    <tr>
                                                        <td>
                                                            <br />
                                                            <br />
                                                            <div class="panHeader2">
                                                                <span style="">
                                                                    <asp:Label ID="lblAccessBlock" runat="server" Text="Bloqueos de acceso" Font-Bold="true" /></span>
                                                            </div>
                                                            <br />
                                                        </td>
                                                    </tr>
                                                    <tr style="padding-top: 5px">
                                                        <td style="padding-left: 20px">
                                                            <asp:Label ID="lblAccessBlockDescription" Text="En este formulario puede configurar los bloqueos de acceso en caso de fallos de seguridad o intrusiones." runat="server"></asp:Label>
                                                            <br />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td style="padding-left: 20px;">
                                                            <roUserControls:roGroupBox ID="groupAccessBlock" runat="server">
                                                                <Content>
                                                                    <table>
                                                                        <tr>
                                                                            <td>
                                                                                <input type="checkbox" id="ckBlockPortal" runat="server" />&nbsp;<asp:Label ID="lblBlockPortal" runat="server" Text="Bloquear el acceso a todos los portales VisaulTtime Portal(incluido Apps)"></asp:Label>
                                                                            </td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td>
                                                                                <input type="checkbox" id="ckBlockDesktop" runat="server" />&nbsp;<asp:Label ID="lblBlockDesktop" runat="server" Text="Bloqueo total de VisualTime Desktop excepto usuario Administrador"></asp:Label>
                                                                            </td>
                                                                        </tr>
                                                                    </table>
                                                                </Content>
                                                            </roUserControls:roGroupBox>
                                                        </td>
                                                    </tr>

                                                    <tr>
                                                        <td>
                                                            <br />
                                                            <br />
                                                            <div class="panHeader2">
                                                                <span style="">
                                                                    <asp:Label ID="lblRemoteAccess" runat="server" Text="Soporte remoto" Font-Bold="true" /></span>
                                                            </div>
                                                            <br />
                                                        </td>
                                                    </tr>
                                                    <tr style="padding-top: 5px">
                                                        <td style="padding-left: 20px">
                                                            <asp:Label ID="lblRemoteAccessDescription" Text="Puede indicar si se permite el acceso a los usuarios de Robotics para realizar acciones de consultoria o mantenimiento." runat="server"></asp:Label>
                                                            <br />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td style="padding-left: 20px;">
                                                            <roUserControls:roGroupBox ID="groupRemote" runat="server">
                                                                <Content>
                                                                    <table>
                                                                        <tr>
                                                                            <td>
                                                                                <input type="checkbox" id="ckRemoteAccess" runat="server" enabled="true" />&nbsp;<asp:Label ID="lblRemoteAccessSection" runat="server" Text="Activar acceso para personal de soporte de Robotics"></asp:Label>
                                                                            </td>
                                                                        </tr>
                                                                    </table>
                                                                </Content>
                                                            </roUserControls:roGroupBox>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="right" valign="bottom" class="DetailFrame_Background" style="padding-right: 20px; height: 100%; vertical-align: bottom;">
                                            <table>
                                                <tr align="right">
                                                    <td>
                                                        <asp:Button ID="btSave" Text="${Button.ApplyChanges}" runat="server" Visible="false" OnClientClick="return saveChanges();return false;" CssClass="btnFlat btnFlatBlack btnFlatAsp" />
                                                    </td>
                                                    <td>
                                                        <asp:Button ID="btCancel" Text="${Button.UndoChanges}" runat="server" Visible="false" OnClientClick="return undoChanges();return false;" CssClass="btnFlat btnFlatBlack btnFlatAsp" />
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <!-- Mensajes -->
                                            <div id="divMsgBottom" class="divMsg" style="display: none;">
                                                <table style="width: 100%;">
                                                    <tr>
                                                        <td align="center" style="width: 20px; height: 16px;">
                                                            <img id="Img2" src="~/Base/Images/MessageFrame/Alert16.png" runat="server" /></td>
                                                        <td align="left" style="padding-left: 10px; color: white;"><span id="msgBottom"></span></td>
                                                        <td align="right" style="color: White; padding-right: 10px;">
                                                            <a href="javascript: void(0);" class="aMsg" onclick="saveChanges();"><span id="lblSaveChangesBottom" runat="server" /></a>
                                                            &nbsp;&nbsp;&nbsp;|&nbsp;&nbsp;&nbsp;
                                                            <a href="javascript: void(0);" onclick="undoChanges();" class="aMsg"><span id="lblUndoChangesBottom" runat="server" /></a>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </div>
                                        </td>
                                    </tr>
                                </table>

                                <asp:HiddenField ID="hdnChanged" runat="server" />

                                <asp:HiddenField ID="hdnIsPostBack_PageBase" runat="server" />
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </div>
        </div>

        <asp:HiddenField ID="SecurityOptions_TabVisibleName" Value="" runat="server" />
    </div>

    <Local:MessageFrame ID="MessageFrame1" runat="server" />
</asp:Content>