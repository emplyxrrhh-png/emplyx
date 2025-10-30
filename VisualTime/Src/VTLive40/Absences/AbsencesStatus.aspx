<%@ Page MasterPageFile="~/Base/mastEmp.master" Language="VB" AutoEventWireup="false" Inherits="VTLive40.Absences_AbsencesStatus" Title="Gestión de bajas" CodeBehind="AbsencesStatus.aspx.vb" %>

<%@ Register Src="~/Requests/WebUserForms/frmComments.ascx" TagName="frmComments" TagPrefix="roForms" %>
<%@ Register Src="~/Requests/WebUserControls/frmRequestResume.ascx" TagName="frmRequestResume" TagPrefix="roForms" %>

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

        //============ SELECTOR EMPLEADOS =================================
        function PopupSelectorEmployeesClient_PopUp(s, e) {
            try {
                s.SetHeaderText("");
                var iFrm = document.getElementById("ctl00_contentMainBody_PopupSelectorEmployees_ASPxPanel3_GroupSelectorFrame");
                var strBase = "../Base/WebUserControls/roWizardSelectorContainerMultiSelectV3.aspx?" +
                    "PrefixTree=treeEmpAbsenceStatus&FeatureAlias=Employees&PrefixCookie=objContainerTreeV3_treeEmpAbsenceStatusGrid&" +
                    "AfterSelectFuncion=parent.GetSelectedTreeV3";
                iFrm.src = strBase;
            }
            catch (e) {
                showError("PopupSelectorEmployeesClient_PopUp", e);
            }
        }
        //==================================================================
        //Guarda los empleados seleccionados en el TreeV3
        function GetSelectedTreeV3(oParm1, oParm2, oParm3) {
            if (oParm1 == "") {
                document.getElementById('ctl00_contentMainBody_CallbackPanelPivot_hdnEmployees').value = "";
                document.getElementById('ctl00_contentMainBody_CallbackPanelPivot_hdnFilter').value = "";
                document.getElementById('ctl00_contentMainBody_CallbackPanelPivot_hdnFilterUser').value = "";
            }
            else {
                document.getElementById('ctl00_contentMainBody_CallbackPanelPivot_hdnEmployees').value = oParm1;
                document.getElementById('ctl00_contentMainBody_CallbackPanelPivot_hdnFilter').value = oParm2;
                document.getElementById('ctl00_contentMainBody_CallbackPanelPivot_hdnFilterUser').value = oParm3;
            }
        }
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
                    <img src="Images/AbsencesStatus.png" height="50px" alt="" />
                </div>
                <div class="blackRibbonDescription">
                    <span class="NameText" id="sectionName" runat="server" clientidmode="Static"></span>
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
                                <div class="RoundCornerFrame roundCorner">
                                    <div style="float: left;">

                                        <table cellpadding="0" cellspacing="0" border="0">
                                            <tr>
                                                <td style="width: 110px;">
                                                    <asp:Label ID="lblEmployees" runat="server" Text="Empleados" Style="display: inline;"></asp:Label>
                                                </td>
                                                <td style="width: 310px;">
                                                    <!-- ReadOnly="true" Font-Size="11px" Height="20px" CssClass="editTextFormat" Width="300px" BorderStyle="Solid" BorderColor="#A0A0A0" BorderWidth="1" BackColor="#DBDBDB" -->
                                                    <dx:ASPxTextBox ID="txtEmployees" ClientInstanceName="txtEmployees_Client" runat="server" ReadOnly="true" Height="20px" Font-Size="11px" CssClass="editTextFormat" Width="300px" Border-BorderStyle="Solid" Border-BorderColor="#A0A0A0" Border-BorderWidth="1" BackColor="#DBDBDB"></dx:ASPxTextBox>
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
                                                    <dx:ASPxTextBox ID="txtCauses" ClientInstanceName="txtCauses_Client" runat="server" ReadOnly="true" Height="20px" Font-Size="11px" CssClass="editTextFormat" Width="300px" Border-BorderStyle="Solid" Border-BorderColor="#A0A0A0" Border-BorderWidth="1" BackColor="#DBDBDB"></dx:ASPxTextBox>
                                                </td>
                                                <td style="width: 180px;" valign="middle">
                                                    <dx:ASPxButton ID="btnOpenPopupSelectorCauses" runat="server" AutoPostBack="False" CausesValidation="False" Text="Seleccionar" ToolTip="Justificaciones..." HoverStyle-CssClass="btnFlat-hover" CssClass="btnFlat">
                                                        <Image Url="~/Absences/Images/EmployeeSelector16.png"></Image>
                                                        <ClientSideEvents Click="btnOpenPopupSelectorCausesClient_Click" />
                                                    </dx:ASPxButton>
                                                </td>
                                            </tr>
                                            <tr id="trConcepts" runat="server">
                                                <td>
                                                    <asp:Label ID="lblStatus" runat="server" Text="Estado"></asp:Label>
                                                </td>
                                                <td style="width: 310px;">
                                                    <dx:ASPxComboBox ID="cmbStatus" runat="server" Visible="true" CssClass="editTextFormat"
                                                        Font-Size="11px" Width="302px" ClientInstanceName="cmbStatusClient" ClientSideEvents-ValueChanged="cmbStatusClient_ValueChanged">
                                                    </dx:ASPxComboBox>
                                                </td>
                                                <td style="width: 180px;" colspan="5" valign="middle">
                                                    <table id="dateFilters" style="display: none">
                                                        <tr>
                                                            <td style="width: 100px;">
                                                                <dx:ASPxDateEdit ID="txtDateInf" runat="server" AllowNull="False" Width="100px" CssClass="editTextFormat" Font-Size="11px" ClientInstanceName="txtDateInfClient" PopupVerticalAlign="TopSides" PopupHorizontalAlign="OutsideRight">
                                                                </dx:ASPxDateEdit>
                                                            </td>
                                                            <td style="width: 15px;">
                                                                <asp:Label ID="lblFechaSup" runat="server" Text="Y"></asp:Label>
                                                            </td>
                                                            <td style="width: 136px;">
                                                                <dx:ASPxDateEdit ID="txtDateSup" runat="server" AllowNull="False" Width="100px" CssClass="editTextFormat" Font-Size="11px" ClientInstanceName="txtDateSupClient" PopupVerticalAlign="TopSides" PopupHorizontalAlign="OutsideRight">
                                                                </dx:ASPxDateEdit>
                                                            </td>
                                                        </tr>
                                                    </table>
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
                                                <dx:ASPxGridView ID="GridAbsencesStatus" runat="server" AutoGenerateColumns="False" Width="100%" Cursor="pointer"
                                                    DataSourceID="LinqAbsencesDataSource" ClientInstanceName="GridAbsencesStatusClient">
                                                    <ClientSideEvents CustomButtonClick="GridAbsencesClientCustomButton_Click" EndCallback="GridAbsencesClient_EndCallback" />
                                                    <SettingsLoadingPanel Text="Cargando&amp;hellip;"></SettingsLoadingPanel>
                                                    <Settings ShowTitlePanel="True" />
                                                </dx:ASPxGridView>
                                                <dx:LinqServerModeDataSource ID="LinqAbsencesDataSource" runat="server" />
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
        PopupVerticalAlign="WindowCenter" PopupHorizontalAlign="WindowCenter" Height="475px" Width="540px"
        ShowHeader="False" ScrollBars="Auto" ShowPageScrollbarWhenModal="True" PopupAnimationType="None" BackColor="Transparent" ContentStyle-Paddings-Padding="0px" Border-BorderColor="Transparent" ShowShadow="false">
        <ContentCollection>
            <dx:PopupControlContentControl ID="PopupControlContentControl2" runat="server">
                <dx:ASPxPanel ID="ASPxPanel1" runat="server" Width="0px" Height="0px">
                    <PanelCollection>
                        <dx:PanelContent ID="PanelContent1" runat="server">
                            <div class="bodyPopupExtended" style="width: 500px; height: 430px">
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

    <!-- POPUP DEL SELECTOR DE INCIDENCIAS -->
    <dx:ASPxPopupControl ID="PopupEditRequest" runat="server" ClientInstanceName="PopupEditRequestClient" Width="0px" Height="0px" Modal="True"
        CloseAction="None" ShowShadow="true" PopupHorizontalAlign="WindowCenter" PopupVerticalAlign="WindowCenter"
        PopupAnimationType="Fade" ShowCloseButton="False" ShowHeader="false" PopupHorizontalOffset="-260" PopupVerticalOffset="-350">
        <ContentStyle>
            <Paddings Padding="0px" />
        </ContentStyle>
        <Border BorderStyle="None" />
        <ContentCollection>
            <dx:PopupControlContentControl ID="PopupControlContentControl3" runat="server">
                <dx:ASPxPanel ID="ASPxPanel2" runat="server" Width="0px" Height="0px">
                    <PanelCollection>
                        <dx:PanelContent ID="PanelContent4" runat="server">
                            <div class="bodyPopupExtended" style="table-layout: fixed; min-height: 350px; min-width: 515px;">
                                <table id="Table2" cellpadding="0" cellspacing="0" style="width: 100%">
                                    <tr>
                                        <td valign="top">
                                            <table cellpadding="1" cellspacing="1" border="0">
                                                <tr>
                                                    <td>
                                                        <div id="divRequestContent" style="height: 540px; overflow-x: hidden; overflow-y: auto; display: none">
                                                            <% Response.WriteFile("../Requests/srvRequests.aspx")%>
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
                                                        <dx:ASPxButton ID="btCloseRequest" runat="server" AutoPostBack="False" CausesValidation="False" Text="${Button.Accept}" ToolTip="${Button.Accept}" HoverStyle-CssClass="btnFlat-hover btnFlatBlack-hover" CssClass="btnFlat btnFlatBlack">
                                                            <ClientSideEvents Click="btCloseRequestClient_Click" />
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

    <dx:ASPxGridViewExporter ID="ASPxGridViewExporter1" runat="server"></dx:ASPxGridViewExporter>
</asp:Content>