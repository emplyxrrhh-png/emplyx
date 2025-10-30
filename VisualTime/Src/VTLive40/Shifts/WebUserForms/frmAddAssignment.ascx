<%@ Control Language="VB" AutoEventWireup="false" Inherits="VTLive40.frmAddAssignment" CodeBehind="frmAddAssignment.ascx.vb" %>

<!-- Div flotant AddAssignment -->
<input type="hidden" id="hdnAddAssignmentIDShift" />
<input type="hidden" id="hdnAddAssignmentIDRow" />
<input type="hidden" id="hdnAddAssignmentCoverage" />
<div id="<%= Me.ClientID %>_frm" style="position: fixed; *position: absolute; z-index: 995; display: none; top: 50%; left: 50%; *width: 450px;">
    <div id="<%= Me.ClientID %>_BgS" style="position: absolute; top: 0; left: 0; display: none; z-index: 991;"></div>
    <div class="bodyPopupExtended" style="width: 450px;">
        <!-- Controls Popup Aqui -->
        <div style="">
            <div style="width: 100%; height: 100%; background-color: White;" class="bodyPopup">
                <table style="width: 100%; padding-top: 5px;" border="0">
                    <tr>
                        <td colspan="2">
                            <div class="panHeader2">
                                <span style="">
                                    <asp:Label runat="server" ID="lblAddAssignment" Text="Puesto"></asp:Label></span>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2" style="padding: 2px; text-align: left">
                            <table border="0" style="width: 100%;">
                                <tr>
                                    <td>
                                        <asp:Label ID="lblTitleFormAddAssignment" runat="server" CssClass="spanEmp-class" Text="Seleccione el puesto y la cobertura para el horario actual."></asp:Label></td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2" style="padding: 2px; padding-bottom: 10px;" align="center">
                            <table border="0">
                                <tr>
                                    <td style="white-space: nowrap;">
                                        <asp:Label ID="lblAddAssignmentDesc" runat="server" Text="Puesto:"></asp:Label></td>
                                    <td style="width: 150px;">
                                        <dx:ASPxComboBox runat="server" ID="cmbAssignment" Width="150px" ClientInstanceName="cmbAddAssignmentClient">
                                            <ClientSideEvents SelectedIndexChanged="function(s,e){}" />
                                        </dx:ASPxComboBox>
                                    </td>
                                    <td style="width: 90px; display: none" align="right">
                                        <asp:Label ID="lblCoverage" runat="server" Text="Cobertura:"></asp:Label></td>
                                    <td style="width: 50px; display: none">
                                        <dx:ASPxTextBox ID="txtCoverage" runat="server" Width="50px" ClientInstanceName="txtAddCoverageClient">
                                            <ClientSideEvents TextChanged="function(s,e){}" />
                                            <MaskSettings IncludeLiterals="None" Mask="<0..100>" />
                                        </dx:ASPxTextBox>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
                <table border="0" style="width: 100%;">
                    <tr>
                        <td>&nbsp;</td>
                        <td style="width: 110px;" align="right">
                            <dx:ASPxButton ID="btnOk" runat="server" AutoPostBack="False" CausesValidation="False" Text="Aceptar" ToolTip="Aceptar" HoverStyle-CssClass="btnFlat-hover btnFlatBlack-hover" CssClass="btnFlat btnFlatBlack">
                                <ClientSideEvents Click="function(s,e){ frmAddAssignment_Save(); }" />
                            </dx:ASPxButton>
                        </td>
                        <td style="width: 110px;" align="left">
                            <dx:ASPxButton ID="btnCancel" runat="server" AutoPostBack="False" CausesValidation="False" Text="Cancelar" ToolTip="Cancelar" HoverStyle-CssClass="btnFlat-hover btnFlatBlack-hover" CssClass="btnFlat btnFlatBlack">
                                <ClientSideEvents Click="function(s,e){ frmAddAssignment_Close(); }" />
                            </dx:ASPxButton>
                        </td>
                    </tr>
                </table>
            </div>
        </div>
    </div>
</div>
<!-- End Div flotant AddAssignment -->