<%@ Page Language="VB" AutoEventWireup="false" Inherits="VTLive40.MoveEmployee" EnableEventValidation="false" CodeBehind="MoveEmployee.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html lang="en" class="notranslate" translate="no" xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Mover ${Employee}</title>
</head>
<body class="bodyPopup">

    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="True" EnableScriptLocalization="True" />
        <div>

            <script type="text/javascript" language="javascript">
                function showLoadingGrid(loading) { parent.showLoader(loading); }

                function SelectorOk() {
                    ASPxCallbackMoveContenidoClient.PerformCallback("LOAD");
                }

                function SelectorAction(Action) {
                    createCookie('EmployeeMobility_SelectorAction', Action, 10);
                }

                function ShowGroupSelector() {
                    $find('RoPopupFrame1Behavior').show();
                    $get('<%= RoPopupFrame1.ClientID %>').style.display = '';
                    document.getElementById('RoPopupFrame1MyPopupFrame_DIV').style.top = '62px';
                    document.getElementById('RoPopupFrame1MyPopupFrame_DIV').style.left = '64px';
                }

                function HideGroupSelector() {
                    $find('RoPopupFrame1Behavior').hide();
                    $get('<%= RoPopupFrame1.ClientID %>').style.display = 'none';
                }

                function PageBase_Load() {
                    DateDisableState();
                    enableCombo();
                }

                function checkOPCPanelClients() {
                    venableOPC('<%= optNow.ClientID %>');
                venableOPC('<%= optFuture.ClientID %>');
                }

                function CheckPage() {
                    var optNow = document.getElementById('ASPxCallbackMoveContenido_optNow_imgButton');
                    var optFuture = document.getElementById('ASPxCallbackMoveContenido_optFuture_imgButton');

                    if (optFuture.disabled != true || optNow.disabled != true) {
                        ASPxCallbackMoveContenidoClient.PerformCallback("SAVE");
                    }
                    else {
                        ASPxCallbackMoveContenidoClient.PerformCallback("NOSAVE");
                    }
                }

                function GroupSelected(Nodo) {
                    var hdnSelected = document.getElementById('<%= Me.hdnIDGroupSelected.ClientID %>');
                    hdnSelected.value = Nodo.id;
                }

                function ASPxCallbackMoveContenidoClient_EndCallBack(s, e) {
                    if (s.cpActionRO == "SAVE") {
                        showLoadingGrid(false);
                        Close();
                    } else {
                        showLoadingGrid(false);
                        ConvertControls('');
                        linkOPCItems('<%= optNow.ClientID %>,<%= optFuture.ClientID %>');
                        checkOPCPanelClients();
                        DateDisableState();
                        enableCombo();
                    }

                }

                function DateDisableState() {
                    txtMoveDateClient.SetEnabled(false);
                }

                function DateEnableState(status) {
                    if (document.getElementById('ASPxCallbackMoveContenido_optFuture_rButton').checked == true) {
                        txtMoveDateClient.SetEnabled(true);
                    }
                }

                function enableCombo() {
                    if (ddlSourceEmployeeClient.GetEnabled() == true) {
                        ddlSourceEmployeeClient.SetEnabled(false);
                    } else {
                        ddlSourceEmployeeClient.SetEnabled(true);
                    }
                }
            </script>

            <dx:ASPxCallbackPanel ID="ASPxCallbackMoveContenido" runat="server" Width="100%" ClientInstanceName="ASPxCallbackMoveContenidoClient">
                <SettingsLoadingPanel Enabled="false" />
                <ClientSideEvents EndCallback="ASPxCallbackMoveContenidoClient_EndCallBack" />
                <PanelCollection>
                    <dx:PanelContent ID="PanelContent1" runat="server">
                        <asp:HiddenField ID="hdnCanClose_PageBase" runat="server" />
                        <asp:HiddenField ID="hdnMustRefresh_PageBase" Value="0" runat="server" />

                        <table cellpadding="0" cellspacing="0" width="100%" height="480px" style="padding: 5px 5px 5px 5px;">
                            <tr>
                                <td colspan="2">
                                    <div class="panHeader2">
                                        <span style="">
                                            <asp:Label ID="lblTitle" runat="server" Text="Mover ${Employees}" /></span>
                                    </div>
                                </td>
                            </tr>
                            <tr valign="middle">
                                <td align="center">
                                    <asp:Image ID="imgMove" ImageUrl="Images/Empleado-Move-32x32.GIF" runat="server" />
                                </td>
                                <td style="padding-left: 10px;">
                                    <asp:Label ID="lblDescription" runat="server" CssClass="editTextFormat" Text="Va a mover el ${Employee} actual a otro grupo. Seleccione cuando debe hacerse efectivo el movimiento y pulse el botón 'Aceptar'."></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2">
                                    <table width="100%">
                                        <tr>
                                            <td width="20">
                                                <asp:Label ID="lblGroup" runat="server" Text="Grupo " Font-Bold="true" Style="display: block; padding: 2px; padding-right: 5px;"></asp:Label>
                                            </td>
                                            <td style="width: auto;">
                                                <asp:LinkButton ID="lnkGroupSelected" runat="server" Text="Seleccionar ${Group}" OnClientClick="ShowGroupSelector(); return false;" TabIndex="1" Style="display: block; padding: 2px; padding-left: 5px; border: solid 1px silver; width: auto;"></asp:LinkButton>
                                            </td>
                                        </tr>
                                    </table>
                                    <asp:HiddenField ID="hdnGroupSelected" runat="server" Value="" />
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2">
                                    <roUserControls:roOptionPanelClient ID="optNow" runat="server" TypeOPanel="RadioOption" Width="100%" Height="Auto" Enabled="True" CConClick="DateDisableState(false)">
                                        <Title>
                                            <asp:Label ID="lblNumberContractLastTitle" runat="server" Text="Mover el ${Employee} ahora"></asp:Label>
                                        </Title>
                                        <Description>
                                            <asp:Label ID="lblNowDescription" Text="Moverá el ${Employee} inmediatamente." ForeColor="DarkBlue" runat="server"></asp:Label>
                                        </Description>
                                        <Content>
                                        </Content>
                                    </roUserControls:roOptionPanelClient>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2">
                                    <roUserControls:roOptionPanelClient ID="optFuture" runat="server" TypeOPanel="RadioOption" Width="100%" Height="Auto" Enabled="True" CConClick="DateEnableState(true)">
                                        <Title>
                                            <asp:Label ID="lblFutureTitle" runat="server" Text="Mover el ${Employee} a partir de una fecha específica"></asp:Label>
                                        </Title>
                                        <Description>
                                            <asp:Label ID="lblFutureDescription" Text="El movimiento quedará guardado y se hará efectivo en la fecha " ForeColor="DarkBlue" runat="server"></asp:Label>
                                        </Description>
                                        <Content>
                                            <table cellpadding="0" cellspacing="5" style="padding-left: 10px; padding-top: 5px;" width="100%">
                                                <tr>
                                                    <td align="left" style="padding-left: 10px;">
                                                        <dx:ASPxDateEdit ID="txtMoveDate" PopupVerticalAlign="WindowCenter" runat="server" AllowNull="false" Width="150" ClientEnabled="true" ClientInstanceName="txtMoveDateClient">
                                                            <ClientSideEvents DateChanged="function(s,e){}" />
                                                        </dx:ASPxDateEdit>
                                                        <%--<input type="text" id="txtMoveDate" runat="server" class="textClass" ConvertControl="DatePicker" CCallowBlank="false" style="width:75px;"  />--%>
                                                    </td>
                                                </tr>
                                            </table>
                                        </Content>
                                    </roUserControls:roOptionPanelClient>
                                    <roUserControls:roOptPanelClientGroup ID="optGroup" runat="server" />
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2">
                                    <roUserControls:roOptionPanelClient ID="chkCopyPlan" runat="server" TypeOPanel="CheckboxOption" Width="100%" Height="Auto" CConClick="enableCombo()">
                                        <Title>
                                            <asp:Label ID="lblCopyPlanInfo" runat="server" Text="Copiar la planificación de otro ${Employee}"></asp:Label>
                                        </Title>
                                        <Description>
                                        </Description>
                                        <Content>
                                            <table cellpadding="0" cellspacing="0" style="padding-left: 20px; padding-top: 5px;">
                                                <tr>
                                                    <td>
                                                        <asp:Label ID="lblCopyPlanDescription" Text="Al mover el ${Employee} copiar la planificación de " ForeColor="DarkBlue" runat="server"></asp:Label>
                                                        <br />
                                                        <dx:ASPxComboBox ID="ddlSourceEmployee" runat="server" Width="250" ClientInstanceName="ddlSourceEmployeeClient">
                                                            <ClientSideEvents SelectedIndexChanged="function(s,e){}" />
                                                        </dx:ASPxComboBox>
                                                        <%--<roWebControls:roComboBox ID="ddlSourceEmployee" runat="server" EnableViewstate="True" Width="250px" ParentWidth="250px" HiddenText="ddlSourceEmployee_Text"  HiddenValue="ddlSourceEmployee_Value" ChildsVisible="5" />
                                                <asp:HiddenField ID="ddlSourceEmployee_Text" runat="server" />
                                                <asp:HiddenField ID="ddlSourceEmployee_Value" runat="server" />--%>
                                                    </td>
                                                </tr>
                                            </table>
                                        </Content>
                                    </roUserControls:roOptionPanelClient>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2" align="right">
                                    <table>
                                        <tr>
                                            <dx:ASPxButton ID="btOK" runat="server" AutoPostBack="False" CausesValidation="False" Text="${Button.Accept}" ToolTip="${Button.Accept}" HoverStyle-CssClass="btnFlat-hover btnFlatBlack-hover" CssClass="btnFlat btnFlatBlack">
                                                <ClientSideEvents Click="function(s,e){showLoadingGrid(true);  return CheckPage();}" />
                                            </dx:ASPxButton>
                                            <dx:ASPxButton ID="btCancel" runat="server" AutoPostBack="False" CausesValidation="False" Text="${Button.Cancel}" ToolTip="${Button.Cancel}" HoverStyle-CssClass="btnFlat-hover btnFlatBlack-hover" CssClass="btnFlat btnFlatBlack">
                                                <ClientSideEvents Click="function(s,e){Close(); return false;}" />
                                            </dx:ASPxButton>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </dx:PanelContent>
                </PanelCollection>
            </dx:ASPxCallbackPanel>

            <roWebControls:roPopupFrameV2 ID="RoPopupFrame1" runat="server" ShowTitleBar="true" BehaviorID="RoPopupFrame1Behavior" CssClassPopupExtenderBackground="modalBackgroundTransparent">
                <FrameContentTemplate>
                    <table cellpadding="0" cellspacing="0">
                        <tr>
                            <td>
                                <asp:Label ID="lblGroupSelection" Text="Selección ${Group}" runat="server" />
                            </td>
                            <td align="right">
                                <asp:ImageButton ID="btSelectorOk" runat="server" ImageUrl="~/Base/Images/ButtonOK_16.png" Style="cursor: pointer;" OnClientClick='SelectorAction(true); SelectorOk(); HideGroupSelector(); return false;' />
                                <asp:ImageButton ID="btSelectorCancel" runat="server" ImageUrl="~/Base/Images/ButtonCancel_16.png" Style="cursor: pointer;" OnClientClick='HideGroupSelector(); return false;' />
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2" valign="top">
                                <asp:HiddenField ID="hdnIDGroupSelected" runat="server" Value="" />
                                <%--<asp:HiddenField ID="hdnIDGroupSelectedName" runat="server" Value="" />--%>
                                <iframe id="GroupSelectorFrame" runat="server" style="background-color: Transparent" height="350" width="250"
                                    scrolling="auto" frameborder="0" marginheight="0" marginwidth="0" src="" />
                            </td>
                        </tr>
                    </table>
                </FrameContentTemplate>
            </roWebControls:roPopupFrameV2>

            <Local:MessageFrame ID="MessageFrame1" runat="server" />
        </div>
    </form>
</body>
</html>