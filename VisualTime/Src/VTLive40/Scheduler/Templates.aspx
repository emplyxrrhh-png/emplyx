<%@ Page Language="VB" AutoEventWireup="false" Inherits="VTLive40.Templates" EnableEventValidation="false" Culture="auto" UICulture="auto" CodeBehind="Templates.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html lang="en" class="notranslate" translate="no" xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Configuración plantilla calendario</title>
</head>
<body class="bodyPopup" style="background-attachment: fixed;">

    <input type="hidden" id="msgHasChanges" value="" runat="server" clientidmode="Static" />
    <input type="hidden" id="msgHasErrors" value="" runat="server" clientidmode="Static" />

    <form id="frmTemplate" runat="server">

        <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="True" EnableScriptLocalization="True" />

        <script language="javascript" type="text/javascript">

            function PageBase_Load() {

            }

            function GridNonLaboralDays_EndCallback(s, e) {
                if (s.IsEditing()) { }
                else {

                }
            }

            function GridNonLaboralDays_OnRowDblClick(s, e) {
                if (s.IsEditing()) {
                    s.UpdateEdit();
                }
                s.StartEditRow(e.visibleIndex);
            }

            function GridNonLaboralDays_FocusedRowChanged(s, e) {
                if (s.IsEditing()) {
                    s.UpdateEdit();
                }
            }

            function AddNonLaboralDay(s, e) {
                var grid = ASPxClientGridView.Cast("GridNonLaboralDaysClient");
                grid.AddNewRow();
            }
            function ASPxHolidayShiftTemplate_EndCallback(s, e) {

            }
        </script>

        <div>

            <table style="width: 100%; height: 100%" border="0">
                <tr>
                    <td colspan="2" style="padding-top: 5px; padding-bottom: 0px;" valign="top" height="20px">
                        <div class="panHeader2"><span style="">
                            <asp:Label ID="LblTitle" runat="server" Text="Plantillas de horario"></asp:Label></span></div>
                    </td>
                </tr>
                <tr style="height: 48px">
                    <td style="padding: 4px; padding-bottom: 10px;">
                        <img src="<%=Me.ResolveUrl("~/Scheduler/Images/template2.png")%>" />
                    </td>
                    <td align="left" valign="middle" style="padding: 4px; padding-bottom: 10px;">
                        <asp:Label ID="lblDescription" runat="server" CssClass="editTextFormat" Text="Defina las plantillas para asignar planificaciones al calendario de forma automática."></asp:Label>
                    </td>
                </tr>
                <tr style="display: none">
                    <td colspan="2" align="left" valign="middle">
                        <table cellpadding="0" cellspacing="0">
                            <tr>
                                <td>
                                    <asp:Label ID="lblTemplates" Text="Plantillas :" runat="server"></asp:Label>
                                </td>
                                <td>
                                    <asp:UpdatePanel ID="updSelector" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                                        <Triggers>
                                            <asp:AsyncPostBackTrigger ControlID="btRefresh" EventName="Click" />
                                        </Triggers>
                                        <ContentTemplate>
                                            <roWebControls:roComboBox ID="cmbTemplates" runat="server" EnableViewState="True" ParentWidth="150px" HiddenText="cmbTemplates_Text" HiddenValue="cmbTemplates_Value" ChildsVisible="5" Enabled="false" />
                                            <asp:HiddenField ID="cmbTemplates_Text" runat="server" />
                                            <asp:HiddenField ID="cmbTemplates_Value" runat="server" />

                                            <asp:Button ID="btRefresh" runat="server" Style="display: none; visibility: hidden;" />
                                            <asp:HiddenField ID="hdnIDTemplateSelected" runat="server" Value="" />
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr style="height: 220px; vertical-align: top">
                    <td colspan="2">

                        <dx:ASPxCallbackPanel ID="ASPxHolidayShiftTemplate" runat="server" Width="100%" Height="100%" ClientInstanceName="ASPxHolidayShiftTemplateClient">
                            <SettingsLoadingPanel Enabled="false" />
                            <ClientSideEvents EndCallback="ASPxHolidayShiftTemplate_EndCallback" />
                            <PanelCollection>
                                <dx:PanelContent ID="PanelContent2" runat="server">

                                    <div class="jsGrid">
                                        <asp:Label ID="lblNonLaboralDaysCaption" runat="server" CssClass="jsGridTitle" Text="Días festivos"></asp:Label>
                                        <div class="jsgridButton">
                                            <dx:ASPxButton ID="btnAddNonLaboralDay" runat="server" AutoPostBack="False" CausesValidation="False" Text="Añadir" ToolTip="Añadir nuevo día festivo" HoverStyle-CssClass="btnFlat-hover" CssClass="btnFlat">
                                                <Image Url="~/Base/Images/Grid/add.png"></Image>
                                                <ClientSideEvents Click="AddNonLaboralDay" />
                                            </dx:ASPxButton>
                                        </div>
                                    </div>
                                    <div class="jsGridContent">
                                        <dx:ASPxGridView ID="GridNonLaboralDays" runat="server" AutoGenerateColumns="False" ClientInstanceName="GridNonLaboralDaysClient" KeyboardSupport="True" Width="100%">
                                            <Settings ShowTitlePanel="False" VerticalScrollBarMode="Auto" UseFixedTableLayout="True" VerticalScrollableHeight="200" />
                                            <ClientSideEvents EndCallback="GridNonLaboralDays_EndCallback" RowDblClick="GridNonLaboralDays_OnRowDblClick" FocusedRowChanged="GridNonLaboralDays_FocusedRowChanged" />
                                            <SettingsCommandButton>
                                                <DeleteButton Image-Url="~/Base/Images/Grid/remove.png" Image-ToolTip="" />
                                                <UpdateButton Image-Url="~/Base/Images/Grid/save.png" Image-ToolTip="" />
                                                <CancelButton Image-Url="~/Base/Images/Grid/cancel.png" Image-ToolTip="" />
                                                <EditButton Image-Url="~/Base/Images/Grid/edit.png" Image-ToolTip="" />
                                            </SettingsCommandButton>
                                            <Styles>
                                                <CommandColumn Spacing="5px" />
                                                <Header CssClass="jsGridHeaderCell" />
                                                <Cell Wrap="False" />
                                            </Styles>
                                            <SettingsPager Mode="ShowAllRecords" ShowEmptyDataRows="false">
                                            </SettingsPager>
                                        </dx:ASPxGridView>
                                    </div>
                                </dx:PanelContent>
                            </PanelCollection>
                        </dx:ASPxCallbackPanel>
                    </td>
                </tr>
                <tr>

                    <td align="right" colspan="2" style="height: 20px; padding-right: 5px;">
                        <asp:UpdatePanel ID="updActions" runat="server">
                            <ContentTemplate>
                                <table>
                                    <tr>
                                        <td>
                                            <asp:Button ID="btSaves" Text="${Button.Save}" runat="server" OnClientClick="" CssClass="btnFlat btnFlatBlack btnFlatAsp" />
                                        </td>
                                        <td>
                                            <asp:Button ID="btClose" Text="${Button.Cancel}" runat="server" OnClientClick="" CssClass="btnFlat btnFlatBlack btnFlatAsp" />
                                        </td>
                                    </tr>
                                </table>
                                <asp:HiddenField ID="hdnCanClose_PageBase" runat="server" />
                                <asp:HiddenField ID="hdnMustRefresh_PageBase" Value="0" runat="server" />

                                <%--  <asp:Button ID="btSave" Text="" runat="server" Visible="false" CssClass="btnFlat btnFlatBlack btnFlatAsp" />
                                <asp:Button ID="btUndo" Text="" runat="server" visible="false" CssClass="btnFlat btnFlatBlack btnFlatAsp" />--%>

                                <asp:HiddenField ID="hdnChanged" runat="server" Value="0" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                </tr>
            </table>

            <Local:MessageFrame ID="MessageFrame1" runat="server" />
        </div>
    </form>
</body>
</html>