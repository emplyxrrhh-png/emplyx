<%@ Page Language="VB" AutoEventWireup="false" Inherits="VTLive40.Wizards_SupervisorCopy" Culture="auto" UICulture="auto" CodeBehind="SupervisorCopy.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html lang="en" class="notranslate" translate="no" xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Asistente para modificar parámetros de seguridad</title>
</head>
<body class="bodyPopup" style="background-attachment: fixed;">

    <form id="frmSecurityActions" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="True" EnableScriptLocalization="True" />

        <div>

            <script language="javascript" type="text/javascript">

                function PageBase_Load() {

                    var oActiveFrameIndex = document.getElementById('<%= hdnActiveFrame.ClientID %>').value;
                    ConvertControls('divStep' + oActiveFrameIndex);

                    CheckFile();
                }

                function CheckFile() {
                    var showFile = $get('<%= Me.hiddenShowFile.ClientID %>').value;
                    if (showFile != '') {
                        __doPostBack('<%= Me.btRefresh.ClientID %>', '');
                    }
                }

                //Llença aquest script al recarregar els updatepanels per poder controlar per js els opclient
                function endRequestHandler() {
                    hidePopupLoader();
                }

                function showPopupLoader() {
                    if (typeof (window.parent.frames["ifPrincipal"]) != "undefined") {
                        if (typeof (window.parent.frames["ifPrincipal"].showLoadingGrid) == "function") {
                            window.parent.frames["ifPrincipal"].showLoadingGrid(true);
                        } else {
                            window.parent.frames["ifPrincipal"][0].showLoadingGrid(true);
                        }
                    } else {
                        window.parent.parent.frames["ifPrincipal"].showLoadingGrid(true);
                    }
                }

                function hidePopupLoader() {
                    if (typeof (window.parent.frames["ifPrincipal"]) != "undefined") {
                        if (typeof (window.parent.frames["ifPrincipal"].showLoadingGrid) == "function") {
                            window.parent.frames["ifPrincipal"].showLoadingGrid(false);
                        } else {
                            window.parent.frames["ifPrincipal"][0].showLoadingGrid(false);
                        }

                    } else {
                        window.parent.parent.frames["ifPrincipal"].showLoadingGrid(false);
                    }
                }

                function CheckFrame() {
                    var bolRet = true;
                    var oActiveFrameIndex = document.getElementById('<%= hdnActiveFrame.ClientID %>').value;

                    if (CheckConvertControls('divStep' + oActiveFrameIndex) == false) {
                        bolRet = false;
                    } else {
                        bolRet = true;
                    }
                    if (!bolRet) hidePopupLoader();
                    return bolRet;
                }
            </script>

            <div class="popupWizardContent">

                <!-- Welcome -->
                <asp:UpdatePanel ID="updStep0" runat="server" RenderMode="Inline">
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="btPrev" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="btNext" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="btEnd" EventName="Click" />
                    </Triggers>
                    <ContentTemplate>
                        <div id="divStep0" runat="server" style="display: block; width: 500px;">
                            <table id="tbStep0" style="width: 100%;" cellspacing="0" cellpadding="0">
                                <tr>
                                    <td style="height: 360px">
                                        <asp:Image ID="imgSecurityActionWizard" runat="server" Style="border-radius: 5px;" ImageUrl="~/Base/Images/Wizards/wzmens.gif" />
                                    </td>
                                    <td style="padding-left: 20px; padding-right: 20px; padding-top: 50px" valign="top">
                                        <asp:Label ID="lblWelcomeEmployees" Style="display: block;" runat="server" Text="Bienvenido al asistente para la copia de propiedades de supervisores" Font-Bold="True" Font-Size="Large"></asp:Label>
                                        <br />
                                        <br />
                                        <br />
                                        <asp:Label ID="lblWelcome2" runat="server" Text="Este asistente le ayudará a asignar restricciones, centros de coste o grupos de negocio a partir de un supervisor." Font-Bold="true"></asp:Label>
                                        <br />
                                        <br />
                                        <asp:Label ID="lblWelcome3" runat="server" Text="Para continuar, haga clic en siguiente."></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td style="height: 10px" colspan="2" class="PassportWizards_ButtonsPanel"></td>
                                </tr>
                            </table>
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>

                <!-- Selector de Empleados o Usuarios -->
                <asp:UpdatePanel ID="updStep1" runat="server" RenderMode="Inline">
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="btPrev" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="btNext" EventName="Click" />
                    </Triggers>
                    <ContentTemplate>
                        <div id="divStep1" runat="server" style="display: none;">
                            <table style="width: 100%;" cellspacing="0" cellpadding="0">
                                <tr>
                                    <td class="PassportWizards_StepTitle">
                                        <table style="width: 100%">
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblStep1Title" runat="server" Text="" Font-Bold="True" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="padding-left: 20px">
                                                    <asp:Label ID="lblStep2InfoEmployees" runat="server" Text="Seleccione los ${Employees} que desea tratar." />
                                                    <!-- <asp:Label ID="lblStep2InfoUsers" runat="server" Text="Seleccione los usuarios que desea tratar." /> -->
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="PassportWizards_StepError popupWizardError">
                                        <asp:Label ID="lblStep1Error" runat="server" CssClass="errorText" />
                                    </td>
                                </tr>
                                <tr>
                                    <td valign="top">
                                        <table style="margin-top: 10px; margin-left: auto; margin-right: auto;">
                                            <tr>
                                                <td>
                                                    <dx:ASPxGridView ID="grdSupervisors" runat="server" Cursor="pointer" AutoGenerateColumns="False" ClientInstanceName="grdSupervisorsClient" KeyboardSupport="True" Width="100%" Settings-ShowTitlePanel="True">
                                                        <SettingsBehavior AllowFocusedRow="True" AllowSelectSingleRowOnly="false" AllowSort="False" />
                                                        <Styles>
                                                            <Cell Wrap="False"></Cell>
                                                            <TitlePanel CssClass="TitlePanelClass"></TitlePanel>
                                                        </Styles>
                                                        <SettingsPager Mode="ShowAllRecords" ShowEmptyDataRows="false"></SettingsPager>
                                                        <Border BorderColor="#CDCDCD" />
                                                        <SettingsLoadingPanel Text=""></SettingsLoadingPanel>
                                                        <Settings VerticalScrollBarMode="Auto" UseFixedTableLayout="True" VerticalScrollableHeight="275" />
                                                    </dx:ASPxGridView>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>

                <asp:UpdatePanel ID="updStep2" runat="server" RenderMode="Inline">
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="btPrev" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="btNext" EventName="Click" />
                    </Triggers>
                    <ContentTemplate>
                        <div id="divStep2" runat="server" style="display: none;">
                            <table style="width: 100%;" cellspacing="0" cellpadding="0">
                                <tr>
                                    <td class="PassportWizards_StepTitle">
                                        <table style="width: 100%">
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblStep2Title" runat="server" Text="Seleccione la acción a realizar sobre la selección anterior." Font-Bold="True" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="padding-left: 20px"></td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="PassportWizards_StepError popupWizardError">
                                        <asp:Label ID="lblStep2Error" runat="server" CssClass="errorText" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="SchedulerWizards_StepContent">
                                        <asp:Label ID="lblStep4Info2" runat="server" Text="Finalmente seleccione que opciones del supervisor seleccionado desea copiar. Puede copiar centros de coste, centros de negocio y restricciones." />
                                        <br />
                                        <div style="padding-top: 10px">
                                            <div style="float: left; width: 100%">
                                                <div class="panBottomMargin">
                                                    <div class="panHeader2 panBottomMargin">
                                                        <span class="panelTitleSpan">
                                                            <asp:Label runat="server" ID="lblCopyTitle" Text="¿Qué desea copiar?"></asp:Label>
                                                        </span>
                                                    </div>
                                                </div>
                                                <div class="panBottomMargin">
                                                    <div class="divRow">
                                                        <div class="">
                                                            <dx:ASPxCheckBox ID="ckCopyRestrictions" runat="server" Checked="true" Text="Copiar restricciones de seguridad" />
                                                        </div>
                                                    </div>
                                                    <div id="v3Functions" runat="server">
                                                        <div class="divRow">
                                                            <div class="">
                                                                <dx:ASPxCheckBox ID="ckCopyGroups" runat="server" Checked="true" Text="Copiar grupos que gestiona" />
                                                            </div>
                                                        </div>
                                                        <div class="divRow">
                                                            <div class="">
                                                                <dx:ASPxCheckBox ID="ckCopyCategories" runat="server" Checked="true" Text="Copiar configuración de categorías de solicitud" />
                                                            </div>
                                                        </div>
                                                        <div class="divRow">
                                                            <div class="">
                                                                <dx:ASPxCheckBox ID="ckCopyRoles" runat="server" Checked="true" Text="Copiar roles" />
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </td>
                                </tr>
                            </table>
                            <asp:HiddenField ID="hiddenShowFile" Value="" runat="server" />
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
            <div class="popupWizardButtons">
                <asp:UpdatePanel ID="updButtons" runat="server" RenderMode="Inline">
                    <ContentTemplate>
                        <table align="right" cellpadding="0" cellspacing="0">
                            <tr class="PassportWizards_ButtonsPanel" style="height: 44px">
                                <td>&nbsp
                                </td>
                                <td>
                                    <asp:Button ID="btPrev" Text="${Button.Previous}" runat="server" OnClientClick="showPopupLoader();" Visible="false" TabIndex="1" CssClass="btnFlat btnFlatBlack btnFlatAsp" />
                                </td>
                                <td>
                                    <asp:Button ID="btNext" Text="${Button.Next}" runat="server" OnClientClick="showPopupLoader();return CheckFrame();" TabIndex="2" CssClass="btnFlat btnFlatBlack btnFlatAsp" />
                                    <asp:Button ID="btEnd" Text="${Button.End}" runat="server" Visible="false" TabIndex="4" OnClientClick="showPopupLoader();" CssClass="btnFlat btnFlatBlack btnFlatAsp" />
                                </td>
                                <td>
                                    <asp:Button ID="btClose" Text="${Button.Cancel}" runat="server" OnClientClick="Close(); return false;" TabIndex="5" CssClass="btnFlat btnFlatBlack btnFlatAsp" />

                                    <asp:HiddenField ID="hdnCanClose_PageBase" runat="server" />
                                    <asp:HiddenField ID="hdnMustRefresh_PageBase" Value="0" runat="server" />
                                </td>
                            </tr>
                        </table>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
        </div>

        <input type="hidden" id="hdnActiveFrame" value="0" runat="server" />

        <asp:Button ID="btRefresh" runat="server" Style="display: none;" />
    </form>
</body>
</html>