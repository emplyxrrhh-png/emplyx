<%@ Control Language="VB" AutoEventWireup="false" Inherits="VTLive40.WebUserForms_frmImportExcel" CodeBehind="frmImportExcel.ascx.vb" %>

<div id="<%= Me.ClientID %>_frm" class="ui-dialog-content">
    <form id="<%= Me.ClientID %>_attr">
        <table width="100%" cellspacing="0" class="bodyPopup">
            <tr style="height: 20px;">
                <td colspan="3">
                    <div class="panHeader2">
                        <span style="">
                            <asp:Label ID="lblImportTitle" runat="server" Text="Importar fichero" />
                        </span>
                    </div>
                </td>
            </tr>
            <tr>
                <td colspan="3" style="padding: 15px; padding-bottom: 0px;">
                    <asp:Label ID="lblImportDescription" runat="server" Text="Desde este formulario puede cargar un fichero excel con la planificación para seguir modificandola e importarla a VisualTime." CssClass="editTextFormat" />
                </td>
            </tr>
            <tr>
                <td colspan="3" style="padding: 15px;" align="left">

                    <div style="width: 100%">
                        <div class="panBottomMargin">
                            <div class="panHeader2 panBottomMargin">
                                <span class="panelTitleSpan">
                                    <asp:Label runat="server" ID="lblPlanType" Text="¿Qué tipo de planificación desea realizar?"></asp:Label>
                                </span>
                            </div>
                        </div>

                        <div class="panBottomMargin">
                            <div class="divRow">
                                <div class="">
                                    <dx:ASPxRadioButton ID="rbExcelType1" GroupName="rdExcelImportType" runat="server" Checked="true" Text="Aplicar a cada empleado seleccionado la planificación reseñada en la hoja Excel" />
                                </div>
                            </div>
                        </div>
                        <div class="panBottomMargin">
                            <div class="divRow">
                                <div class="">
                                    <dx:ASPxRadioButton ID="rbExcelType2" GroupName="rdExcelImportType" runat="server" Checked="false" Text="Aplicar una plantilla de planificación a todos los empleados seleccionados" />
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="panBottomMargin">
                        <div class="panHeader2 panBottomMargin">
                            <span class="panelTitleSpan">
                                <asp:Label runat="server" ID="lblImportFile" Text="Seleccione un fichero a importar"></asp:Label>
                            </span>
                        </div>
                    </div>
                    <div class="panBottomMargin">
                        <div class="divRow">
                            <div class="">
                                <input type="file" id="<%= Me.ClientID %>_txtFileToImport" style="width: 275px" />
                            </div>
                        </div>
                    </div>

                    <div style="width: 100%">
                        <div class="panBottomMargin">
                            <div class="panHeader2 panBottomMargin">
                                <span class="panelTitleSpan">
                                    <asp:Label runat="server" ID="lblWhatCopy" Text="¿Qué desea copiar?"></asp:Label>
                                </span>
                            </div>
                        </div>
                        <div class="panBottomMargin">
                            <div class="divRow">
                                <div class="">
                                    <dx:ASPxCheckBox ID="ckImportCopyMainShifts" runat="server" Checked="true" Text="Copiar horarios principales" />
                                </div>
                            </div>
                        </div>
                        <div class="panBottomMargin">
                            <div class="divRow">
                                <div class="">
                                    <dx:ASPxCheckBox ID="ckImportCopyHolidays" runat="server" Checked="false" Text="Copiar vacaciones" />
                                </div>
                            </div>
                        </div>
                    </div>
                    <div style="width: 100%">
                        <div class="panBottomMargin">
                            <div class="panHeader2 panBottomMargin">
                                <span class="panelTitleSpan">
                                    <asp:Label runat="server" ID="lblWhatKeep" Text="¿Qué desea mantener?"></asp:Label>
                                </span>
                            </div>
                        </div>
                        <div class="panBottomMargin">
                            <div class="divRow">
                                <div class="">
                                    <dx:ASPxCheckBox ID="ckImportKeepHolidays" runat="server" Checked="true" Text="Vacaciones actuales" />
                                </div>
                            </div>
                        </div>
                        <div class="panBottomMargin">
                            <div class="divRow">
                                <div class="">
                                    <dx:ASPxCheckBox ID="ckImportKeepBloquedDays" runat="server" Checked="true" Text="Días bloqueados" />
                                </div>
                            </div>
                        </div>
                    </div>
                </td>
            </tr>
        </table>
    </form>
</div>