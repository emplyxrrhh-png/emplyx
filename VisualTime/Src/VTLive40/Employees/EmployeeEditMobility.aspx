<%@ Page Language="VB" AutoEventWireup="false" Inherits="VTLive40.EmployeeEditMobility" EnableEventValidation="false" EnableViewState="True" ValidateRequest="false" CodeBehind="EmployeeEditMobility.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html lang="en" class="notranslate" translate="no" xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Edición movilidad</title>
</head>
<body class="bodyPopup" style="background-attachment: fixed;">

    <form id="frmEmployeeEditMobility" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="True" EnableScriptLocalization="True" />

        <div>

            <script language="javascript" type="text/javascript">

                function PageBase_Load() {
                    ConvertControls('');
                }

                function pageonclick() {

                }
                document.onclick = pageonclick;

                function SelectorAction(Action) {
                    createCookie('EmployeeMobility_SelectorAction', Action, 10);
                }

                function ShowGroupSelector() {
                    $find('pfGroupSelectorBehavior').show();
                    $get('<%= pfGroupSelector.ClientID %>').style.display = '';
                }

                function GroupSelected(Nodo) {
                    var hdnSelected = document.getElementById('<%= Me.hdnIDGroupSelected.ClientID %>');
                hdnSelected.value = Nodo.id.replace('A', '');
                var hdnSelectedName = document.getElementById('<%= Me.hdnIDGroupSelectedName.ClientID %>');
                    hdnSelectedName.value = Nodo.text;
                }

                function HideGroupSelector(acceptAction) {
                    $find('pfGroupSelectorBehavior').hide();
                    if (acceptAction) {
                        var hdnSelected = document.getElementById('<%= Me.hdnIDGroupSelected.ClientID %>');
                    var hdnSelectedName = document.getElementById('<%= Me.hdnIDGroupSelectedName.ClientID %>');
                        var grid = ASPxClientGridView.Cast("GridMobilityClient");
                        if (grid.IsEditing()) {
                            var strName = hdnSelectedName.value;
                            grid.SetEditValue('FullGroupName', strName.decodeEntities());
                            grid.SetEditValue('IDGroup', hdnSelected.value);
                            hdnSelectedName.value = "";
                            hdnSelected = "";
                        }
                    }
                }

                function GridMobility_BeginCallback(e, c) {

                }

                function GridMobility_EndCallback(s, e) {
                    if (s.IsEditing()) {

                    }
                }

                function GridMobility_OnRowDblClick(s, e) {

                }

                function GridMobility_FocusedRowChanged(s, e) {
                    if (s.IsEditing()) {
                        s.UpdateEdit();
                    }
                }

                //Agregar nueva fila en el grid de incidencias
                function AddNewMobility(s, e) {
                    var grid = ASPxClientGridView.Cast("GridMobilityClient");
                    grid.AddNewRow();
                }

                //Eliminar una incidencia en el datatable del servidor
                function DeleteMobility(IdRow) {
                    grid = ASPxClientGridView.Cast("GridMobilityClient");
                    grid.DeleteRow(IdRow);
                }
            </script>
            <div class="popupWizardContent">
                <table style="width: 100%;" border="0">
                    <tr>
                        <td colspan="2" style="padding-top: 5px; padding-bottom: 0px;" height="20px" valign="top">
                            <div class="panHeader2">
                                <span style="">
                                    <asp:Label ID="lblTitle" runat="server" Text="Movilidad"></asp:Label></span>
                            </div>
                        </td>
                    </tr>
                    <tr style="height: 48px">
                        <td style="padding: 4px; padding-bottom: 10px;">
                            <img src="~/Base/Images/Employees/Mobilidad.png" runat="server" />
                        </td>
                        <td align="left" valign="middle" style="padding: 4px; padding-bottom: 10px;">
                            <asp:Label ID="lblDescription" runat="server" CssClass="editTextFormat" Text="Puede ver en qué grupos ha estado y estará el empleado actual. Para mover un empleado de un grupo a otro escriba la fecha en que se dará la movilidad y seleccione el grupo al que dirigirá el empleado."></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2" align="center">
                            <div class="jsGrid">
                                <asp:Label ID="lblMobilitiesCaption" runat="server" CssClass="jsGridTitle" Text="Mobilidades"></asp:Label>
                                <div class="jsgridButton">
                                    <dx:ASPxButton ID="btnAddNewHMobility" runat="server" AutoPostBack="False" CausesValidation="False" Text="Añadir" ToolTip="Añadir nueva movilidad" HoverStyle-CssClass="btnFlat-hover" CssClass="btnFlat">
                                        <Image Url="~/Base/Images/Grid/add.png"></Image>
                                        <ClientSideEvents Click="AddNewMobility" />
                                    </dx:ASPxButton>
                                </div>
                            </div>
                            <div class="jsGridContent">
                                <dx:ASPxGridView ID="GridMobility" runat="server" AutoGenerateColumns="False" ClientInstanceName="GridMobilityClient" KeyboardSupport="True" Width="100%" ClientSideEvents-BeginCallback="GridMobility_BeginCallback">
                                    <Settings ShowTitlePanel="False" VerticalScrollBarMode="Auto" UseFixedTableLayout="True" VerticalScrollableHeight="150" />
                                    <ClientSideEvents EndCallback="GridMobility_EndCallback" RowDblClick="GridMobility_OnRowDblClick" FocusedRowChanged="GridMobility_FocusedRowChanged" />
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
                        </td>
                    </tr>
                </table>
            </div>
            <div class="popupWizardButtons" align="right">
                <table>
                    <tr>
                        <td align="right" colspan="2" style="height: 20px; padding-right: 5px;">
                            <asp:UpdatePanel ID="updActions" runat="server">
                                <ContentTemplate>
                                    <table>
                                        <tr>
                                            <td>
                                                <asp:Button ID="btOK" Text="${Button.Accept}" runat="server" OnClientClick="" CssClass="btnFlat btnFlatBlack btnFlatAsp" /></td>
                                            <td>
                                                <asp:Button ID="btCancel" Text="${Button.Cancel}" runat="server" OnClientClick="Close(); return false;" CssClass="btnFlat btnFlatBlack btnFlatAsp" /></td>
                                        </tr>
                                    </table>
                                    <asp:HiddenField ID="hdnCanClose_PageBase" runat="server" />
                                    <asp:HiddenField ID="hdnMustRefresh_PageBase" Value="0" runat="server" />
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </td>
                    </tr>
                </table>
            </div>
            <asp:UpdatePanel ID="updGroupSelector" runat="server" UpdateMode="conditional">
                <ContentTemplate>
                    <roWebControls:roPopupFrameV2 ID="pfGroupSelector" runat="server" ShowTitleBar="true" BehaviorID="pfGroupSelectorBehavior" CssClassPopupExtenderBackground="modalBackgroundTransparent">
                        <FrameContentTemplate>
                            <table cellpadding="0" cellspacing="0">
                                <tr>
                                    <td>
                                        <asp:Label ID="lblGroupSelection" Text="Selección ${Group}" runat="server" />
                                    </td>
                                    <td align="right">
                                        <asp:ImageButton ID="btSelectorOk" runat="server" ImageUrl="~/Base/Images/ButtonOK_16.png" Style="cursor: pointer;"
                                            OnClientClick="HideGroupSelector(true); return false;" />
                                        <asp:ImageButton ID="btSelectorCancel" runat="server" ImageUrl="~/Base/Images/ButtonCancel_16.png" Style="cursor: pointer;"
                                            OnClientClick='HideGroupSelector(false); return false;' />
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="2" valign="top">
                                        <asp:HiddenField ID="hdnIDGroupSelected" runat="server" Value="" />
                                        <asp:HiddenField ID="hdnIDGroupSelectedName" runat="server" Value="" />
                                        <iframe id="GroupSelectorFrame" runat="server" style="background-color: Transparent" height="400" width="250"
                                            scrolling="auto" frameborder="0" marginheight="0" marginwidth="0" src="" />
                                    </td>
                                </tr>
                            </table>
                        </FrameContentTemplate>
                    </roWebControls:roPopupFrameV2>
                    <asp:HiddenField ID="IDGroupSelected" runat="server" />
                </ContentTemplate>
            </asp:UpdatePanel>

            <Local:MessageFrame ID="MessageFrame1" runat="server" />
        </div>
    </form>
</body>
</html>