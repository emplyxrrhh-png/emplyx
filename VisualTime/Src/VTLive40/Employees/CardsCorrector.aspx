<%@ Page Language="VB" AutoEventWireup="false" Inherits="VTLive40.CardsCorrector" EnableEventValidation="false" EnableViewState="True" CodeBehind="CardsCorrector.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html lang="en" class="notranslate" translate="no" xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Corrector de ${Punches}</title>
</head>
<body class="bodyPopup" style="background-attachment: fixed;">

    <form id="frmCardsCorrector" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="True" EnableScriptLocalization="True" />
        <div>

            <script type="text/javascript">

                function pageonclick() {
                }

                function PageBase_Load() {
                }

                function ShowCardAssignWizard(CardID) {
                    var url = 'Wizards/CardAssignWizard.aspx?CardID=' + CardID;
                    var Title = $get('<%= lblCardAssignWizardFormTitle.ClientID %>').innerHTML;
                    Title = '';
                    ShowExternalForm2(url, 500, 450, Title, '', false, false, false);
                }

                function GridEntries_BeginCallback(e, c) {
                }

                function GridEntries_EndCallback(s, e) {
                    if (s.IsEditing()) { }
                }

                function GridEntries_FocusedRowChanged(s, e) {
                    if (s.IsEditing()) s.UpdateEdit();
                }

                function GridEntries_CustomButtonClick(s, e) {
                    if (e.buttonID == "AssignCardRow") {
                        if (e.visibleIndex > -1)
                            GridEntriesClient.GetRowValues(e.visibleIndex, 'IDCredentialStr', ShowCardAssignWizard);
                    }
                }

                function RefreshScreen(DataType) {

                    if (DataType != '0') {
                        GridEntriesClient.PerformCallback("RELOAD");
                    }

                }

                function endRequestHandler() {
                    hidePopupLoader();
                }

                function showPopupLoader() {

                    var bolFound = false;
                    var curFrame = window;
                    while (bolFound == false) {
                        if (typeof (curFrame.frames["ifPrincipal"]) != "undefined") {
                            curFrame.frames["ifPrincipal"].parent.showLoadingGrid(true);
                            bolFound = true;
                        }

                        if (curFrame == window.top) {
                            bolFound = true;
                        } else {
                            curFrame = curFrame.parent;
                        }
                    }
                }

                function hidePopupLoader() {
                    var bolFound = false;
                    var curFrame = window;
                    while (bolFound == false) {
                        if (typeof (curFrame.frames["ifPrincipal"]) != "undefined") {
                            curFrame.frames["ifPrincipal"].parent.showLoadingGrid(false);
                            bolFound = true;
                        }

                        if (curFrame == window.top) {
                            bolFound = true;
                        } else {
                            curFrame = curFrame.parent;
                        }
                    }
                }
            </script>

            <table cellpadding="2" cellspacing="2" width="100%" style="padding: 5px 5px 5px 5px;">
                <tr valign="middle">
                    <td colspan="2" style="padding-top: 5px; padding-bottom: 0px;" valign="top" height="20px">
                        <div class="panHeader2">
                            <span style="">
                                <asp:Label ID="lblTitle" runat="server" Text="Corrector de ${Punches}"></asp:Label></span>
                        </div>
                    </td>
                </tr>
                <tr style="height: 48px;">
                    <td style="padding: 4px; padding-bottom: 10px;">
                        <img src="Images/STAMP_CLOCK_32.GIF" />
                    </td>
                    <td align="left" valign="middle" style="padding: 4px; padding-bottom: 10px;">
                        <asp:Label ID="lblDescription" runat="server" CssClass="editTextFormat" Text="Este formulario le permite corregir los ${Punches} que se han producido con una ${Card} incorrecta."></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <dx:ASPxGridView ID="GridEntries" runat="server" AutoGenerateColumns="False" ClientInstanceName="GridEntriesClient" KeyboardSupport="True" Width="100%">
                            <Templates>
                                <TitlePanel>
                                </TitlePanel>
                            </Templates>
                            <SettingsBehavior AllowFocusedRow="True" AllowSelectSingleRowOnly="True" AllowSort="False" />
                            <Styles>
                                <Cell Wrap="False"></Cell>
                                <TitlePanel CssClass="TitlePanelClass"></TitlePanel>
                            </Styles>
                            <SettingsPager Mode="ShowPager" ShowEmptyDataRows="false">
                            </SettingsPager>
                            <Border BorderColor="#CDCDCD" />
                            <SettingsLoadingPanel Text=""></SettingsLoadingPanel>
                            <ClientSideEvents BeginCallback="GridEntries_BeginCallback" EndCallback="GridEntries_EndCallback" FocusedRowChanged="GridEntries_FocusedRowChanged" CustomButtonClick="GridEntries_CustomButtonClick" />
                            <Settings VerticalScrollBarMode="Auto" UseFixedTableLayout="True" VerticalScrollableHeight="280" />
                        </dx:ASPxGridView>
                    </td>
                </tr>
                <tr>
                    <td colspan="2"></td>
                </tr>
                <tr>
                    <td colspan="2" align="right">
                        <table cellpadding="0" cellspacing="0" style="padding-right: 10px;">
                            <tr>
                                <td align="right">
                                    <asp:Button ID="btOK" Text="${Button.Accept}" runat="server" OnClientClick="showPopupLoader();" CssClass="btnFlat btnFlatBlack btnFlatAsp" />
                                </td>
                                <td align="right">
                                    <asp:Button ID="btCancel" Text="${Button.Cancel}" runat="server" OnClientClick="Close(); return false;" CssClass="btnFlat btnFlatBlack btnFlatAsp" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>

            <asp:Label ID="lblCardAssignWizardFormTitle" Text="Asistente para asignar códigos de ${Card}" Style="display: none" runat="server" />

            <asp:UpdatePanel ID="updPageBase" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="btOK" EventName="Click" />
                </Triggers>
                <ContentTemplate>
                    <asp:HiddenField ID="hdnCanClose_PageBase" runat="server" />
                    <asp:HiddenField ID="hdnMustRefresh_PageBase" Value="0" runat="server" />
                </ContentTemplate>
            </asp:UpdatePanel>

            <Local:MessageFrame ID="MessageFrame1" runat="server" />
            <Local:ExternalForm ID="externalform1" DragEnabled="false" runat="server" />
        </div>
    </form>
</body>
</html>