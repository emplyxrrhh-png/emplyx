<%@ Page MasterPageFile="~/Base/mastEmp.master" Language="VB" AutoEventWireup="false" Inherits="VTLive40.Tasks_ReviewBusinessCenters" Title="Gestión de bajas" CodeBehind="ReviewBusinessCenters.aspx.vb" %>

<%@ Register Src="~/Base/WebUserControls/frmBusinessCenterSelector.ascx" TagPrefix="roForms" TagName="frmBusinessCenterSelector" %>
<%--<%@ Register Src="~/Requests/WebUserForms/frmComments.ascx" TagName="frmComments" TagPrefix="roForms" %>
<%@ Register Src="~/Requests/WebUserControls/frmRequestResume.ascx" TagName="frmRequestResume" TagPrefix="roForms" %>--%>

<asp:Content ID="Content1" ContentPlaceHolderID="contentMainBody" runat="Server">
    <style type="text/css" media="screen">
        .NoWhiteSpace {
            white-space: nowrap;
        }

        .divErrorStyle {
            border: thin solid #A3A3A3;
            font-size: 14px;
            margin-bottom: 10px;
            margin-left: auto;
            margin-right: auto;
            padding: 5px;
            text-align: center;
            vertical-align: middle;
            width: 80%;
        }
    </style>

    <script language="javascript" type="text/javascript">
</script>

    <asp:HiddenField ID="dtFormat" runat="server" />
    <asp:HiddenField ID="dtFormatText" runat="server" />
    <input type="hidden" id="hdnAllEmployees" runat="server" value="" />
    <input type="hidden" id="hdnAllCauses" runat="server" value="" />
    <input type="hidden" id="hdnCauseSelectedText" runat="server" value="" />
    <dx:ASPxCallback ID="CallbackSession" runat="server" ClientInstanceName="CallbackSessionClient" ClientSideEvents-CallbackComplete="CallbackSession_CallbackComplete"></dx:ASPxCallback>

    <div id="divMainBody">
        <!-- TAB SUPERIOR -->
        <div id="divTabInfo" class="divDataCells">
            <div style="min-height: 10px"></div>
            <div id="divAccessGroup" class="blackRibbonTitle">
                <div class="blackRibbonIcon">
                    <img src="Images/BusinessCenters48.png" height="50px" alt="" />
                </div>
                <div class="blackRibbonDescription">
                    <span class="NameText"><%=Me.Language.Translate("ReviewTitle", Me.DefaultScope)%></span>
                </div>
            </div>
            <div style="min-height: 10px"></div>
        </div>
        <!-- FIN TAB SUPERIOR -->
        <!-- DETALLE -->
        <div id="divTabData" class="divDataCells">
            <div id="divContenido" class="divAllContent">
                <div id="divContent" style="height: initial;" class="maxHeight">
                    <dx:ASPxCallbackPanel ID="CallbackPanelPivot" runat="server" Width="100%" ClientInstanceName="CallbackPanelPivotClient">
                        <PanelCollection>
                            <dx:PanelContent ID="PanelContent2" runat="server">

                                <asp:HiddenField ID="hdnEmployeesSelected" runat="server" Value="0" />
                                <asp:HiddenField ID="hdnEmployees" runat="server" Value="" />
                                <asp:HiddenField ID="hdnCausesSelected" runat="server" Value="" />
                                <asp:HiddenField ID="hdnCauses" runat="server" Value="" />
                                <asp:HiddenField ID="hdnFilter" runat="server" Value="" />
                                <asp:HiddenField ID="hdnFilterUser" runat="server" Value="" />
                                <asp:HiddenField ID="hdnSelectedRowVisibleIndex" runat="server" Value="" />
                                <asp:HiddenField ID="hdnBusinessCentersSelected" runat="server" Value="" />
                                <asp:HiddenField ID="hdnBusinessCentersSelectedText" runat="server" Value="" />
                                <div class="RoundCornerFrame roundCorner">
                                    <div style="float: left;">
                                        <table cellpadding="0" cellspacing="0" border="0">
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblBusinessCenters" runat="server" Text="Centros de coste" Style="display: inline;"></asp:Label>
                                                </td>
                                                <td style="width: 310px;">
                                                    <asp:TextBox ID="txtBusinessCenters" runat="server" ReadOnly="true" Font-Size="11px" Height="20px" CssClass="editTextFormat" Width="300px" BorderStyle="Solid" BorderColor="#A0A0A0" BorderWidth="1" BackColor="#DBDBDB"></asp:TextBox>
                                                </td>
                                                <td style="width: 180px;" valign="middle">
                                                    <dx:ASPxButton ID="btnOpenPopupSelectorCenter" runat="server" AutoPostBack="False" CausesValidation="False" Text="Seleccionar" ToolTip="Empleados..." HoverStyle-CssClass="btnFlat-hover" CssClass="btnFlat">
                                                        <Image Url="~/Absences/Images/EmployeeSelector16.png"></Image>
                                                        <ClientSideEvents Click="btnOpenPopupSelectorCenterClient_Click" />
                                                    </dx:ASPxButton>
                                                    <roForms:frmBusinessCenterSelector runat="server" ID="frmBusinessCenterSelector" />
                                                </td>
                                                <td style="width: 35px;">
                                                    <asp:Label ID="lblFechaInf" runat="server" Text="Entre"></asp:Label>
                                                </td>
                                                <td style="width: 100px;">
                                                    <dx:ASPxDateEdit ID="txtDateInf" runat="server" AllowNull="False" Width="100px" CssClass="editTextFormat" Font-Size="11px" ClientInstanceName="txtDateInfClient">
                                                    </dx:ASPxDateEdit>
                                                </td>
                                                <td style="width: 15px;">
                                                    <asp:Label ID="lblFechaSup" runat="server" Text="Y"></asp:Label>
                                                </td>
                                                <td style="width: 100px;">
                                                    <dx:ASPxDateEdit ID="txtDateSup" runat="server" AllowNull="False" Width="100px" CssClass="editTextFormat" Font-Size="11px" ClientInstanceName="txtDateSupClient">
                                                    </dx:ASPxDateEdit>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="width: 110px;">
                                                    <asp:Label ID="lblEmployees" runat="server" Text="Empleados" Style="display: inline;"></asp:Label>
                                                </td>
                                                <td style="width: 310px;">
                                                    <asp:TextBox ID="txtEmployees" runat="server" ReadOnly="true" Font-Size="11px" Height="20px" CssClass="editTextFormat" Width="300px" BorderStyle="Solid" BorderColor="#A0A0A0" BorderWidth="1" BackColor="#DBDBDB"></asp:TextBox>
                                                </td>
                                                <td style="width: 180px;" valign="middle">
                                                    <dx:ASPxButton ID="btnOpenPopupSelectorEmployees" runat="server" AutoPostBack="False" CausesValidation="False" Text="Seleccionar" ToolTip="Empleados..." HoverStyle-CssClass="btnFlat-hover" CssClass="btnFlat">
                                                        <Image Url="~/Absences/Images/EmployeeSelector16.png"></Image>
                                                        <ClientSideEvents Click="btnOpenPopupSelectorEmployeesClient_Click" />
                                                    </dx:ASPxButton>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblCauses" runat="server" Text="Justificación" Style="display: inline;"></asp:Label>
                                                </td>
                                                <td style="width: 310px;">
                                                    <asp:TextBox ID="txtCauses" runat="server" ReadOnly="true" Font-Size="11px" Height="20px" CssClass="editTextFormat" Width="300px" BorderStyle="Solid" BorderColor="#A0A0A0" BorderWidth="1" BackColor="#DBDBDB"></asp:TextBox>
                                                </td>
                                                <td style="width: 180px;" valign="middle">
                                                    <dx:ASPxButton ID="btnOpenPopupSelectorCauses" runat="server" AutoPostBack="False" CausesValidation="False" Text="Seleccionar" ToolTip="Justificaciones..." HoverStyle-CssClass="btnFlat-hover" CssClass="btnFlat">
                                                        <Image Url="~/Absences/Images/EmployeeSelector16.png"></Image>
                                                        <ClientSideEvents Click="btnOpenPopupSelectorCausesClient_Click" />
                                                    </dx:ASPxButton>
                                                </td>
                                            </tr>
                                        </table>
                                    </div>
                                    <div style="float: right; padding-top: 2px;">
                                        <div style="float: left; padding-right: 10px;">
                                            <dx:ASPxButton ID="btnRefresh" runat="server" AutoPostBack="False" CausesValidation="False" Text="Obtener" ToolTip="Obtener Datos" HoverStyle-CssClass="btnFlat-hover" CssClass="btnFlat" ClientInstanceName="btnRefreshClient">
                                                <Image Url="~/Base/Images/Grid/button_reload.png"></Image>
                                                <ClientSideEvents Click="btnRefreshClient_Click" />
                                            </dx:ASPxButton>
                                        </div>
                                        <div style="float: left; padding-right: 10px;">
                                            <dx:ASPxButton ID="btnExportToXls" runat="server" CausesValidation="False" Text="Exportar" ToolTip="Exportar a Excel" HoverStyle-CssClass="btnFlat-hover" CssClass="btnFlat">
                                                <Image Url="~/Base/Images/Grid/ExportToExcel16.png"></Image>
                                            </dx:ASPxButton>
                                        </div>
                                    </div>
                                    <div style="clear: both;"></div>
                                </div>

                                <table cellpadding="0" cellspacing="0" style="width: 99%; height: 100%; margin-left: auto; margin-right: auto;" border="0">
                                    <tr>
                                        <td valign="top">
                                            <div id="divFilterPlates" style="text-align: left; vertical-align: top; padding-top: 10px; height: 99%; display: block;">
                                                <dx:ASPxGridView ID="GridIncidences" runat="server" Cursor="pointer" AutoGenerateColumns="False"
                                                    ClientInstanceName="GridIncidencesClient" KeyboardSupport="True" Width="100%"
                                                    ClientSideEvents-BeginCallback="GridIncidences_beginCallback" Settings-ShowTitlePanel="True">
                                                    <SettingsBehavior AllowFocusedRow="True" AllowSelectSingleRowOnly="false" AllowSort="False" />
                                                    <Styles>
                                                        <Cell Wrap="False"></Cell>
                                                        <TitlePanel CssClass="TitlePanelClass"></TitlePanel>
                                                    </Styles>
                                                    <SettingsPager Mode="ShowAllRecords" ShowEmptyDataRows="false"></SettingsPager>
                                                    <Border BorderColor="#CDCDCD" />
                                                    <SettingsLoadingPanel Text=""></SettingsLoadingPanel>
                                                    <ClientSideEvents EndCallback="GridIncidences_EndCallback" RowDblClick="GridIncidences_OnRowDblClick" FocusedRowChanged="GridIncidences_FocusedRowChanged" SelectionChanged="GridIncidences_SelectionChanged" />
                                                    <Settings VerticalScrollBarMode="Auto" UseFixedTableLayout="True" VerticalScrollableHeight="350" />
                                                </dx:ASPxGridView>
                                            </div>
                                            <br />
                                            <div style="clear: both">
                                                <div style="float: left; margin-top: 6px;">
                                                    <asp:Label ID="lblSelectedHours" runat="server" Text="Total de horas seleccionadas:" Font-Bold="True" />
                                                </div>
                                                <div style="float: left">
                                                    <dx:ASPxTextBox runat="server" ID="txtTimeSelected" MaxLength="9" Width="75px" ClientInstanceName="txtTimeSelectedClient" ReadOnly="true">
                                                        <MaskSettings Mask="<000000..999999>:<00..59>" />
                                                        <ClientSideEvents TextChanged="function(s,e){ }" />
                                                        <ValidationSettings ErrorDisplayMode="None" />
                                                    </dx:ASPxTextBox>
                                                </div>
                                            </div>
                                            <div style="clear: both">
                                                <div style="float: left; margin-top: 3px;">
                                                    <input type="checkbox" id="ckMoveAllTo" runat="server" />
                                                </div>
                                                <div style="float: left; margin-top: 3px;">
                                                    <asp:Label ID="lblMoveTo" runat="server" Text="Mover todas las horas seleccionadas al centro de coste:" Font-Bold="True" />
                                                </div>
                                                <div style="float: left">
                                                    <dx:ASPxComboBox ID="cmbCostCenters" runat="server" ClientInstanceName="cmbCostCentersClient" Width="450px"></dx:ASPxComboBox>
                                                </div>
                                            </div>
                                            <div style="float: right; padding-top: 2px;">
                                                <div style="float: left; padding-right: 10px;">
                                                    <dx:ASPxButton ID="btnRun" runat="server" AutoPostBack="False" CausesValidation="False" Text="Guardar cambios" ToolTip="Guardar cambios" HoverStyle-CssClass="btnFlat-hover" CssClass="btnFlat" ClientInstanceName="btnRefreshClient">
                                                        <Image Url="~/Base/Images/Grid/button_save.png"></Image>
                                                        <ClientSideEvents Click="btnRunClient_Click" />
                                                    </dx:ASPxButton>
                                                </div>
                                            </div>
                                        </td>
                                    </tr>
                                </table>
                            </dx:PanelContent>
                        </PanelCollection>
                    </dx:ASPxCallbackPanel>
                </div>
            </div>
        </div>
    </div>

    <!-- POPUP DEL SELECTOR DE EMPLEADOS -->
    <dx:ASPxPopupControl ID="PopupSelectorEmployees" runat="server" AllowDragging="True" CloseAction="None" Modal="True" ClientInstanceName="PopupSelectorEmployeesClient" ClientSideEvents-PopUp="PopupSelectorEmployeesClient_PopUp"
        PopupVerticalAlign="WindowCenter" PopupHorizontalAlign="WindowCenter" Height="500px" Width="800px"
        ShowHeader="False" ScrollBars="Auto" ShowPageScrollbarWhenModal="True" PopupAnimationType="None" BackColor="Transparent" ContentStyle-Paddings-Padding="0px" Border-BorderColor="Transparent" ShowShadow="false">
        <ContentCollection>
            <dx:PopupControlContentControl ID="PopupControlContentControl1" runat="server">
                <dx:ASPxPanel ID="ASPxPanel3" runat="server" Width="0px" Height="0px">
                    <PanelCollection>
                        <dx:PanelContent ID="PanelContent3" runat="server">
                            <div class="bodyPopupExtended" style="width: 775px; height: 460px">
                                <table id="tbPopupFrame" cellpadding="0" cellspacing="0">
                                    <tr>
                                        <td valign="top">
                                            <iframe id="GroupSelectorFrame" runat="server" style="background-color: Transparent;" height="420" width="775" scrolling="no"
                                                frameborder="0" marginheight="0" marginwidth="0" src="" />
                                        </td>
                                    </tr>
                                    <tr style="height: 35px;">
                                        <td align="right">
                                            <table>
                                                <tr>
                                                    <td>
                                                        <dx:ASPxButton ID="btnPopupSelectorEmployeesAccept" runat="server" AutoPostBack="False" CausesValidation="False" Text="${Button.Accept}" ToolTip="${Button.Accept}" HoverStyle-CssClass="btnFlat-hover btnFlatBlack-hover" CssClass="btnFlat btnFlatBlack">
                                                            <ClientSideEvents Click="btnPopupSelectorEmployeesAcceptClient_Click" />
                                                        </dx:ASPxButton>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </dx:PanelContent>
                    </PanelCollection>
                </dx:ASPxPanel>
            </dx:PopupControlContentControl>
        </ContentCollection>
        <ContentStyle>
            <Paddings PaddingBottom="5px" />
        </ContentStyle>
    </dx:ASPxPopupControl>

    <!-- POPUP DEL SELECTOR DE INCIDENCIAS -->
    <dx:ASPxPopupControl ID="PopupSelectorCauses" runat="server" AllowDragging="True" CloseAction="None" Modal="True" ClientInstanceName="PopupSelectorCausesClient"
        PopupVerticalAlign="WindowCenter" PopupHorizontalAlign="WindowCenter" Height="500px" Width="540px"
        ShowHeader="False" ScrollBars="Auto" ShowPageScrollbarWhenModal="True" PopupAnimationType="None" BackColor="Transparent" ContentStyle-Paddings-Padding="0px" Border-BorderColor="Transparent" ShowShadow="false">
        <ContentCollection>
            <dx:PopupControlContentControl ID="PopupControlContentControl2" runat="server">
                <dx:ASPxPanel ID="ASPxPanel1" runat="server" Width="0px" Height="0px">
                    <PanelCollection>
                        <dx:PanelContent ID="PanelContent1" runat="server">
                            <div class="bodyPopupExtended" style="width: 500px; height: 470px">
                                <table id="Table1" cellpadding="0" cellspacing="0" style="width: 100%">
                                    <tr>
                                        <td valign="top">
                                            <table style="width: 100%;">
                                                <tr>
                                                    <td style="padding-left: 5px;">
                                                        <asp:Label ID="lblPopupCauses" runat="server" Text="Justificaciones" />
                                                    </td>
                                                </tr>
                                                <tr style="vertical-align: top;">
                                                    <td>
                                                        <div style="border: thin solid #9F9F9F; overflow-y: auto; height: 380px;">
                                                            <dx:ASPxCheckBoxList ID="ListCauses" TextWrap="false" runat="server" RepeatColumns="1" RepeatLayout="Table" Width="100%" ClientEnabled="True"
                                                                ClientInstanceName="ListCauses_Client" Border-BorderColor="Transparent" Paddings-PaddingRight="5px" ClientVisible="True">
                                                            </dx:ASPxCheckBoxList>
                                                        </div>
                                                    </td>
                                                </tr>
                                                <tr style="vertical-align: top;">
                                                    <td>
                                                        <div style="float: left; margin-top: 3px;">
                                                            <dx:ASPxCheckBox ID="chkCauses" runat="server" ClientInstanceName="chkCausesClient" AutoPostBack="false">
                                                                <ClientSideEvents CheckedChanged="function(s,e){SelectAllCauses();}" />
                                                            </dx:ASPxCheckBox>
                                                        </div>
                                                        <div style="float: left; margin-top: 6px;">
                                                            <asp:Label ID="lblAllCauses" runat="server" Text="Seleccionar todas las justificaciones" Font-Bold="True" />
                                                        </div>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr style="height: 35px;">
                                        <td align="right">
                                            <table>
                                                <tr>
                                                    <td>
                                                        <dx:ASPxButton ID="btnPopupSelectorCausesAccept" runat="server" AutoPostBack="False" CausesValidation="False" Text="${Button.Accept}" ToolTip="${Button.Accept}" HoverStyle-CssClass="btnFlat-hover btnFlatBlack-hover" CssClass="btnFlat btnFlatBlack">
                                                            <ClientSideEvents Click="btnPopupSelectorCausesAcceptClient_Click" />
                                                        </dx:ASPxButton>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </dx:PanelContent>
                    </PanelCollection>
                </dx:ASPxPanel>
            </dx:PopupControlContentControl>
        </ContentCollection>
        <ContentStyle>
            <Paddings PaddingBottom="5px" />
        </ContentStyle>
    </dx:ASPxPopupControl>

    <!-- POPUP NEW OBJECT -->
    <dx:ASPxPopupControl ID="CaptchaObjectPopup" runat="server" AllowDragging="False" CloseAction="None" Modal="True" ContentUrl="~/Base/Popups/GenericCaptchaValidator.aspx"
        PopupVerticalAlign="WindowCenter" PopupHorizontalAlign="WindowCenter" ModalBackgroundStyle-Opacity="0" Width="500" Height="320"
        ShowHeader="False" ScrollBars="Auto" ShowPageScrollbarWhenModal="True" ClientInstanceName="CaptchaObjectPopup_Client" PopupAnimationType="None" BackColor="Transparent" ContentStyle-Paddings-Padding="0px" Border-BorderColor="Transparent" ShowShadow="false">
        <SettingsLoadingPanel Enabled="false" />
    </dx:ASPxPopupControl>

    <dx:ASPxGridViewExporter ID="ASPxGridViewExporter1" runat="server"></dx:ASPxGridViewExporter>
</asp:Content>