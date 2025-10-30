<%@ Control Language="VB" AutoEventWireup="false" Inherits="VTLive40.WebUserForms_frmCopyOptions" CodeBehind="frmCopyOptions.ascx.vb" %>

<div id="<%= Me.ClientID %>_frm" class="ui-dialog-content">
    <form id="<%= Me.ClientID %>_attr">
        <table width="100%" cellspacing="0" class="bodyPopup">
            <tr style="height: 20px;">
                <td colspan="3">
                    <div class="panHeader2">
                        <span style="">
                            <asp:Label ID="lblTitle" runat="server" Text="Pegado Especial" />
                        </span>
                    </div>
                </td>
            </tr>
            <tr>
                <td colspan="3" style="padding: 15px; padding-bottom: 0px;">
                    <asp:Label ID="lblEspecialPasteInfo" runat="server" Text="Este formulario le permite asignar los horarios que tenga copiados en el portapapeles a un empleado entre las fechas que especifique." CssClass="editTextFormat" />
                </td>
            </tr>
            <tr>
                <td colspan="3" style="padding: 15px;" align="left">
                    <%--<div id="advCopyWhatObjects" style="width: 100%">
                        <div class="panBottomMargin">
                            <div class="panHeader2 panBottomMargin">
                                <span class="panelTitleSpan">
                                    <asp:Label runat="server" ID="lblCopyTitle" Text="¿Qué desea copiar?"></asp:Label>
                                </span>
                            </div>
                        </div>
                        <div id="advCopyWhatMain" class="panBottomMargin">
                            <div class="divRow">
                                <div class="">
                                    <dx:ASPxCheckBox ID="ckSPCopyMainShifts" runat="server" Checked="true" ClientInstanceName="ckSPCopyMainShifts_Client" Text="Copiar horarios principales" />
                                </div>
                            </div>
                        </div>
                        <div id="advCopyWhatHolidays" class="panBottomMargin">
                            <div class="divRow">
                                <div class="">
                                    <dx:ASPxCheckBox ID="ckSPCopyHolidays" runat="server" Checked="false" ClientInstanceName="ckSPCopyHolidays_Client" Text="Copiar vacaciones" />
                                </div>
                            </div>
                        </div>
                    </div>--%>
                    <div id="<%= Me.ClientID %>_advCopyKeepObjects" style="width: 100%">
                        <div class="panBottomMargin">
                            <div class="panHeader2 panBottomMargin">
                                <span class="panelTitleSpan">
                                    <asp:Label runat="server" ID="lblMaintainTitle" Text="¿Qué desea mantener?"></asp:Label>
                                </span>
                            </div>
                        </div>
                        <div id="<%= Me.ClientID %>_advCopyKeepHolidays" class="panBottomMargin">
                            <div class="divRow">
                                <div class="">
                                    <dx:ASPxCheckBox ID="ckSPKeepHolidays" runat="server" Checked="true" ClientInstanceName="ckSPKeepHolidays_Client" Text="Vacaciones actuales" />
                                </div>
                            </div>
                        </div>
                        <div id="<%= Me.ClientID %>_advCopyKeepBloqued" class="panBottomMargin">
                            <div class="divRow">
                                <div class="">
                                    <dx:ASPxCheckBox ID="ckSPKeepBloquedDays" runat="server" Checked="true" ClientInstanceName="ckSPKeepBloquedDays_Client" Text="Días bloqueados" />
                                </div>
                            </div>
                        </div>
                    </div>
                </td>
            </tr>
        </table>
    </form>
</div>