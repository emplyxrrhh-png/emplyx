<%@ Control Language="VB" AutoEventWireup="false" Inherits="VTLive40.Base_WebUserControls_IdentifyMethods" CodeBehind="IdentifyMethods.ascx.vb" %>

<input type="hidden" id="hdnType" value="Employee" runat="server" />
<input type="hidden" id="hdnID" value="-1" runat="server" />

<div style="width: 99%;">

    <div id="divMethodsArea" runat="server">

        <div class="panHeader3" style="margin: 5px">
            <span style="padding-left: 24px; padding-top: 3px; display: block">
                <asp:Label ID="lblTerminalAccess" runat="server" Text="Métodos de identificación por Terminal" Font-Bold="true" />
            </span>
        </div>

        <table>
            <tr>
                <td >
                    <div style="float: left; padding: 2px; vertical-align: top;margin-left:15px;">
                <asp:Label ID="lblInformation" ForeColor="#2D4155" runat="server" />
                        </div>
                    </td>
            </tr>
            <tr>
                <td>
                    <div style="float: left; padding: 2px; width: 340px; vertical-align: top;">
                        <roUserControls:roOptionPanelClient ID="chkBiometric" runat="server" TypeOPanel="CheckboxOption" Width="100%" Height="Auto" Checked="False" Enabled="True" CConClick="hasIdentifyChanges();">
                            <Title>
                                <asp:Label ID="lblBiometricTitle" runat="server" Text="Mediante biometría"></asp:Label>
                            </Title>
                            <Description></Description>
                            <Content>
                                <table id="tableBiometric" runat="server" cellpadding="0" cellspacing="5" style="height: 75px;">
                                    <tr>
                                        <td rowspan="2" valign="middle">
                                            <asp:Image ID="imgBiometric" ImageUrl="~/Base/Images/IdentifyMethods/IdentifyMethods_Biometric_36.png" Width="36" Height="36" runat="server" />
                                        </td>
                                        <td colspan="2" style="padding-left: 10px;">
                                            <asp:Label ID="lblBiometricInfo" Text="El ${Employee} se identifica mediante biometría." ForeColor="DarkBlue" runat="server"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" valign="middle" style="padding-left: 10px;">
                                            <asp:Label ID="lblIDBiometric" Text="Código biometría" runat="server" Visible="false"></asp:Label>
                                        </td>
                                        <td align="left" valign="middle" style="">
                                            <input type="text" id="txtIDBiometricSX" runat="server" visible="false" readonly="readonly"
                                                class="textClass x-form-text x-form-field" onblur="this.className='textClass x-form-text x-form-field';" onfocus="this.className='textClass x-form-text x-form-field x-form-focus';" />
                                        </td>
                                    </tr>
                                </table>
                            </Content>
                        </roUserControls:roOptionPanelClient>
                    </div>
                    <div style="float: left; padding: 2px; width: 340px; vertical-align: top;">
                        <roUserControls:roOptionPanelClient ID="chkCard" runat="server" TypeOPanel="CheckboxOption" Width="100%" Height="Auto" Checked="False" Enabled="True" CConClick="hasIdentifyChanges();">
                            <Title>
                                <asp:Label ID="lblCardTitle" runat="server" Text="Mediante tarjeta"></asp:Label>
                            </Title>
                            <Description></Description>
                            <Content>
                                <table id="tableCard" runat="server" cellpadding="0" cellspacing="5" style="height: 75px;">
                                    <tr>
                                        <td rowspan="2" valign="middle">
                                            <asp:Image ID="imgCard" ImageUrl="~/Base/Images/IdentifyMethods/IdentifyMethods_Card_36.png" Width="36" Height="36" runat="server" />
                                        </td>
                                        <td id="tdCardDescription" runat="server" align="left" style="padding-left: 5px;" colspan="2">
                                            <asp:Label ID="lblCardDescription" Text="Debe indicar el código de tarjeta asignado al ${Employee}" ForeColor="DarkBlue" runat="server"></asp:Label>
                                        </td>
                                        <td id="tdCardDescription2" runat="server" align="left" style="padding-left: 5px;" colspan="2">
                                            <asp:Label ID="lblCardDescription2" Text="El ${Employee} se identifica mediante tarjeta." ForeColor="DarkBlue" runat="server"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr id="trCardData" runat="server">
                                        <td align="left" valign="middle" style="padding-left: 10px;">
                                            <asp:Label ID="lblCard" Text="Código de tarjeta " Width="100" runat="server"></asp:Label>
                                        </td>
                                        <td align="left" valign="middle" style="">
                                            <input type="text" id="txtCardMX" runat="server" convertcontrol="TextField" ccallowblank="false" class="textClass" cconchange="hasIdentifyChanges();" />
                                        </td>
                                    </tr>
                                </table>
                            </Content>
                        </roUserControls:roOptionPanelClient>
                    </div>
                    <div style="float: left; padding: 2px; width: 340px; vertical-align: top;">
                        <roUserControls:roOptionPanelClient ID="chkNFC" runat="server" TypeOPanel="CheckboxOption" Width="100%" Height="Auto" Checked="False" Enabled="True" CConClick="hasIdentifyChanges();">
                            <Title>
                                <asp:Label ID="lblNFCTitle" runat="server" Text="Mediante NFC"></asp:Label>
                            </Title>
                            <Description></Description>
                            <Content>
                                <table id="table1" runat="server" cellpadding="0" cellspacing="5" style="height: 75px;">
                                    <tr>
                                        <td rowspan="2" valign="middle">
                                            <asp:Image ID="Image3" ImageUrl="~/Base/Images/IdentifyMethods/IdentifyMethods_NFC_36.png" Width="36" Height="36" runat="server" />
                                        </td>
                                        <td id="tdNFCDescription" runat="server" align="left" style="padding-left: 5px;" colspan="2">
                                            <asp:Label ID="lblNFCDescription" Text="Debe indicar el código nfc asignado al usuario" ForeColor="DarkBlue" runat="server"></asp:Label>
                                        </td>
                                        <td id="tdNFCDescription2" runat="server" align="left" style="padding-left: 5px;" colspan="2">
                                            <asp:Label ID="lblDescription2" Text="El usuario se identifica mediante NFC." ForeColor="DarkBlue" runat="server"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr id="trNFCData" runat="server">
                                        <td align="left" valign="middle" style="padding-left: 10px;">
                                            <asp:Label ID="lblNFC" Text="Código NFC" Width="100" runat="server"></asp:Label>
                                        </td>
                                        <td align="left" valign="middle" style="">
                                            <input type="text" id="txtNFC" runat="server" convertcontrol="TextField" ccallowblank="false" class="textClass" cconchange="hasIdentifyChanges();" />
                                        </td>
                                    </tr>
                                </table>
                            </Content>
                        </roUserControls:roOptionPanelClient>
                    </div>

                </td>
            </tr>
        </table>        
        <table>
            <tr>
                <td>
                    <div style="float: left; padding: 2px; width: 340px; vertical-align: top;">
                        <roUserControls:roOptionPanelClient ID="chkPlate" runat="server" TypeOPanel="CheckboxOption" Width="100%" Height="Auto" Checked="False" Enabled="True" CConClick="hasIdentifyChanges();">
                            <Title>
                                <asp:Label ID="lblPlateTitle" runat="server" Text="Mediante Matrículas"></asp:Label>
                            </Title>
                            <Description></Description>
                            <Content>
                                <table cellpadding="0" cellspacing="5" style="height: 75px; width: 100%">
                                    <tr>
                                        <td valign="middle">
                                            <asp:Image ID="Image10" ImageUrl="~/Base/Images/IdentifyMethods/IdentifyMethods_Plates_36.png" Width="36" Height="36" runat="server" />
                                        </td>
                                        <td>
                                            <table>
                                                <tr>
                                                    <td align="left" valign="middle" style="padding-left: 5px;">
                                                        <asp:Label ID="lblPlates" Text="Matrículas:" Width="100" runat="server"></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" valign="middle" style="padding-left: 5px;">
                                                        <input type="text" id="txtPlate1" runat="server" style="width: 110px;" convertcontrol="TextField" class="textClass" cconchange="hasIdentifyChanges();" />
                                                        <input type="text" id="txtPlate2" runat="server" style="margin-left: 5px; width: 110px;" convertcontrol="TextField" class="textClass" cconchange="hasIdentifyChanges();" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" valign="middle" style="padding-left: 5px;">
                                                        <input type="text" id="txtPlate3" runat="server" style="width: 110px;" convertcontrol="TextField" class="textClass" cconchange="hasIdentifyChanges();" />
                                                        <input type="text" id="txtPlate4" runat="server" style="margin-left: 5px; width: 110px;" convertcontrol="TextField" class="textClass" cconchange="hasIdentifyChanges();" />
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                </table>
                            </Content>
                        </roUserControls:roOptionPanelClient>
                    </div>                                                                                     
                    <div style="float: left; padding: 2px; width: 340px; vertical-align: top;">
                        <roUserControls:roOptionPanelClient ID="chkPin" runat="server" TypeOPanel="CheckboxOption" Width="100%" Height="Auto" Checked="False" Enabled="True" CConClick="hasIdentifyChanges();">
                            <Title>
                                <asp:Label ID="lblPinTitle" runat="server" Text="Mediante pin"></asp:Label>
                            </Title>
                            <Description></Description>
                            <Content>
                                <table cellpadding="0" cellspacing="5" style="height: 75px; width: 100%">
                                    <tr>
                                        <td rowspan="2" valign="middle">
                                            <asp:Image ID="Image2" ImageUrl="~/Base/Images/IdentifyMethods/IdentifyMethods_Pin_36.png" Width="36" Height="36" runat="server" />
                                        </td>
                                                                                                                        <td id="tdPinDescription2" runat="server" align="left" style="padding-left: 5px;" colspan="2">
    <asp:Label ID="lblPinDescription2" ForeColor="DarkBlue" runat="server"></asp:Label>
                                                                                                                            <asp:Label ID="lblPinDescription" ForeColor="DarkBlue" runat="server" Width="100%"></asp:Label>
</td>
    
                                        </tr>
                                    <tr>
                                        <td align="left" valign="middle" style="padding-left: 10px;">
                                            <asp:Label ID="lblPin" Text="Pin" Width="100" runat="server"></asp:Label>
                                        </td>                                        
                                        <td align="left" valign="middle" style="">
                                            <input type="password" id="txtPin" runat="server" convertcontrol="TextField" ccregex="/^([0-9]?[0-9]?[0-9]?[0-9]?[0-9]?[0-9])$/" ccminlength="4" ccmaxlength="6" ccallowblank="false" ccinputtype="password" cconchange="hasIdentifyChanges();" class="textClass" />
                                        </td>
                                    </tr>
                                </table>
                            </Content>
                        </roUserControls:roOptionPanelClient>
                    </div>
                    <div style="float: left; padding: 2px; min-width: 340px; vertical-align: top;">
                    </div>
                </td>
            </tr>
        </table>
    </div>
    <div id="divApplicationArea" runat="server">
        <div class="panHeader3" style="margin: 5px;">
            <span style="padding-left: 24px; padding-top: 3px; display: block">
                <asp:Label ID="lblApplicationAccess" runat="server" Text="Métodos de identificación por Aplicaciones" Font-Bold="true" />
            </span>
        </div>
        <div style="text-align: center; width: 90%; margin: 0 auto;">
            <table style="width: 100%">
                <tr>
                    <td align="left">
                        <div style="float: left; padding: 2px; width: 510px; vertical-align: top;">
                            <roUserControls:roOptionPanelClient ID="chkUsername" runat="server" TypeOPanel="CheckboxOption" Width="100%" Height="Auto" Checked="False" Enabled="True" CConClick="hasIdentifyChanges();checkMustActivateBlock();">
                                <Title>
                                    <asp:Label ID="lblUserNameTitle" runat="server" Text="Mediante usuario y contraseña"></asp:Label>
                                </Title>
                                <Description></Description>
                                <Content>

                                    <table cellpadding="0" cellspacing="5" style="height: 75px; width: 100%;">
                                        <tr>
                                            <td rowspan="6" valign="top">
                                                <asp:Image ID="Image1" ImageUrl="~/Base/Images/IdentifyMethods/IdentifyMethods_Username_36.png" Width="36" Height="36" runat="server" />
                                            </td>
                                            <td align="left" valign="middle" style="padding-left: 10px;">
                                                <asp:Label ID="lblUsername" Text="Nombre usuario" Width="100" runat="server"></asp:Label>
                                            </td>
                                            <td align="right" valign="middle" style="">
                                                <input type="text" id="txtUserName" size="50" width="200" runat="server" convertcontrol="TextField" ccallowblank="false" class="textClass" />
                                            </td>
                                        </tr>
                                        <tr id="trPassword" runat="server">
                                            <td align="left" valign="middle" style="padding-left: 10px;">
                                                <asp:Label ID="lblPassword" Text="Contraseña" Width="100" runat="server"></asp:Label>
                                            </td>
                                            <td align="right" valign="middle" style="">
                                                <input type="password" id="txtPassword" runat="server" convertcontrol="TextField" ccallowblank="false" ccinputtype="password" class="textClass" cconchange="hasIdentifyChanges();" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="2" style="padding-left: 10px; padding-bottom: 5px; vertical-align: middle">
                                                <asp:Label ID="lblInactiveUser" Text="Usuario inactivo" ForeColor="#FF0000" runat="server"></asp:Label>
                                                <asp:Label ID="lblBlockedByInactivity" Text="Usuario bloqueado por inactividad" ForeColor="#FF0000" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="2" style="padding-left: 10px; vertical-align: middle">
                                                <input type="hidden" runat="server" id="hdnMustActivateApplicationAccess" />
                                                <input type="checkbox" runat="server" id="chkRestictApplicationAccess" onchange="hasIdentifyChanges();" /><asp:Label ID="lblRestrictApplicationAccess" Text="Bloquear acceso a aplicaciones" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr id="trValidateByAD" runat="server" style="display: none;">
                                            <td align="left" colspan="2" valign="middle" style="padding-left: 10px;">
                                                <asp:Label ID="lblAD" Text="El usuario se identifica en Active Directory" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="2">
                                                <div id="divUsernameOptions" runat="server" style="float: left; padding: 2px; width: 100%; vertical-align: top;">
                                                    <table>
                                                        <tr>
                                                            <td align="right" valign="middle" style="">
                                                                <div class="btnFlat">
                                                                    <a href="javascript: void(0)" id="btnRestorePwd" onclick="IdentifyRestorePwd();">
                                                                        <asp:Label ID="lblRestorePwd" runat="server" Text="Regenerar contraseña actual"></asp:Label>
                                                                    </a>
                                                                </div>
                                                            </td>
                                                            <td align="right" valign="middle" style="">
                                                                <div class="btnFlat">
                                                                    <a href="javascript: void(0)" id="btnSendUsername" onclick="IdentifySendUsername();">
                                                                        <asp:Label ID="lblSendUsername" runat="server" Text="Enviar usuario"></asp:Label>
                                                                    </a>
                                                                </div>
                                                            </td>
                                                        </tr>
                                                        <tr id="trRecoveryKey" runat="server">
                                                            <td>
                                                                <asp:Label Style="padding-left: 12px;" ID="lblRestoreKey" runat="server" Text="El código de validación del usuario és el ${0}"></asp:Label>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </div>
                                            </td>
                                        </tr>
                                    </table>
                                </Content>
                            </roUserControls:roOptionPanelClient>
                        </div>
                        <div style="float: left; padding: 2px; width: 400px; vertical-align: top;">  
                            <roUserControls:roOptionPanelClient ID="ckCegidID" runat="server" TypeOPanel="CheckboxOption" Width="100%" Height="Auto" Checked="False" Enabled="True" CConClick="hasIdentifyChanges()">
                                <Title>
                                    <asp:Label ID="lblCegidIdDesc" runat="server" Text="Mediante cegidID"></asp:Label>
                                </Title>
                                <Description></Description>
                                <Content>
                                    <div class="divRow" style="margin-top:10px;margin-left:25px">
                                        <div style="float:left">
                                            <img id="imgCegidIdStatus" runat="server" src="~/Base/Images/TasksIdle.png" alt="" />
                                        </div>
                                        <div style="float:left">
                                            <asp:Label Style="padding-left: 12px;" ID="lblCegidIdStatus" runat="server" Text="inactivo"></asp:Label>
                                        </div>
                                        <div style="float:left">    
                                            <asp:Label Style="padding-left: 12px;" ID="lblCegidIdRegisterDate" runat="server" Text=""></asp:Label>
                                        </div>
                                        <div id="btnResetCegidId" runat="server" style="float:left;margin-top:-10px;margin-left: 15px;display:none">
                                            <div class="btnFlat">
                                                <a href="javascript: void(0)" id="bResetCegidId" onclick="resetCegidIDUser();">
                                                    <asp:Label ID="lbl" runat="server" Text="Desvincular cuenta cegidID"></asp:Label>
                                                </a>
                                            </div>
                                        </div>
                                    </div>
                                </Content>
                            </roUserControls:roOptionPanelClient>

                        </div>
                    </td>
                </tr>
            </table>
        </div>
    </div>
</div>
<div style="clear: both;"></div>
