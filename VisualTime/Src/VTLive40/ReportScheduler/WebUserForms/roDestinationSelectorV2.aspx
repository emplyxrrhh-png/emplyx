<%@ Page Language="VB" AutoEventWireup="false" Inherits="VTLive40.Base_WebUserControls_roDestinationSelectorV2" CodeBehind="roDestinationSelectorV2.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html lang="en" class="notranslate" translate="no" xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>roWizardSelectorContainerMultiSelectV3</title>
</head>
<body>
    <form id="form1" runat="server" style="height: 100%;">
        <div style="border: thin solid silver; height: 97%; width: 98%; padding: 3px; overflow: auto;" class="defaultBackgroundColor">

            <input runat="server" clientidmode="Static" type="hidden" id="hdnLngSupervisorTextSelected" value="supervisor seleccionado" />
            <input runat="server" clientidmode="Static" type="hidden" id="hdnLngSupervisorsTextSelected" value="supervisores seleccionados" />
            <input runat="server" clientidmode="Static" type="hidden" id="hdnLngNoDocument" value="Sin documento seleccionado" />
            <input runat="server" clientidmode="Static" type="hidden" id="hdnLngToDocument" value="Al documento " />
            <input runat="server" clientidmode="Static" type="hidden" id="hdnEmployeeField" value="Correo electrónico del empleado" />
            <input runat="server" clientidmode="Static" type="hidden" id="hdnNoHardcodedField" value="Correo sin especificar" />

            <div style="">
                <div style="width: 100%; height: 100%; background-color: White;" class="bodyPopup">
                    <table style="width: 100%; padding-top: 5px;" border="0">
                        <tr>
                            <td colspan="2">
                                <div class="panHeader2">
                                    <span style="">
                                        <asp:Label runat="server" ID="lblAddDestination" Text="Añadir Destino"></asp:Label></span>
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2" style="padding: 2px;">
                                <table border="0" style="width: 100%;">
                                    <tr>
                                        <td></td>
                                        <td>
                                            <asp:Label ID="lblTitleFormAddDestination" runat="server" CssClass="spanEmp-class" Text="Este formulario le permite añadir destinos a los que se enviará el listado generado tras la ejecución del informe planificado."></asp:Label></td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2" style="padding: 2px; padding-bottom: 10px;" align="center">
                                <roUserControls:roOptionPanelClient ID="opTypeMultipleExport" runat="server" TypeOPanel="RadioOption" Width="100%" Height="Auto" Checked="False" Enabled="True" CConClick="frmDestinationV2OnChange();">
                                    <Title>
                                        <asp:Label ID="lblopTypeMultipleExport" runat="server" Text="Supervisores:"></asp:Label>
                                    </Title>
                                    <Description>
                                        <asp:Label ID="lblopTypeMultipleExportDesc" runat="server" Text="Se ejecutará el informe para cada supervisor de la lista especificada y se enviará a su dirección de correo electrónico."></asp:Label>
                                    </Description>
                                    <Content>
                                        <table border="0" style="margin-left: 25px; width: 635px;">
                                            <tr>
                                                <td>
                                                    <dx:ASPxTokenBox ID="tbAvailableSupervisors" runat="server" Width="100%" ClientInstanceName="tbAvailableSupervisorsClient">
                                                        <ClientSideEvents TextChanged="frmDestinationV2OnChange" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" Init="" />
                                                    </dx:ASPxTokenBox>
                                                </td>
                                            </tr>
                                        </table>
                                    </Content>
                                </roUserControls:roOptionPanelClient>

                                <roUserControls:roOptionPanelClient ID="opTypeEmployeeExport" runat="server" TypeOPanel="RadioOption" Width="100%" Height="Auto" Checked="False" Enabled="True" CConClick="frmDestinationV2OnChange();">
                                    <Title>
                                        <asp:Label ID="lblopTypeEmployeeExportTitle" runat="server" Text="Empleados:"></asp:Label>
                                    </Title>
                                    <Description>
                                        <asp:Label ID="lblopTypeEmployeeExportDesc" runat="server" Text="Se ejecutará el informe y se enviarán los datos a cada empleado en el destino indicado"></asp:Label>
                                    </Description>
                                    <Content>

                                        <table border="0" style="margin-left: 25px; width: 635px;">
                                            <tr>
                                                <td>
                                                    <roUserControls:roOptionPanelClient ID="opEmployeeDocumentTemplate" runat="server" TypeOPanel="RadioOption" Width="100%" Height="Auto" Checked="False" Enabled="True" CConClick="frmDestinationV2OnChange();">
                                                        <Title>
                                                            <asp:Label ID="lblopEmployeeDocumentTemplateTitle" runat="server" Text="Documento"></asp:Label>
                                                        </Title>
                                                        <Description>
                                                            <asp:Label ID="lblopEmployeeDocumentTemplateDesc" runat="server" Text="Se guardará el informe como documento en el gestor documental de VisualTime"></asp:Label>
                                                        </Description>
                                                        <Content>
                                                            <table style="width: 100%;">
                                                                <tr>
                                                                    <td style="padding-left: 25px; width: 30%;">
                                                                        <asp:Label ID="lblopEmployeeDocumentTemplateField" runat="server" Text="Plantilla:"></asp:Label>
                                                                    </td>
                                                                    <td style="width: 70%;">
                                                                        <dx:ASPxComboBox ID="cmbEmployeeDocumentTemplate" runat="server" Width="250px" Font-Size="11px" ForeColor="#2D4155"
                                                                            Font-Names="Arial;Verdana;Sans-Serif" IncrementalFilteringMode="Contains" ClientInstanceName="cmbEmployeeDocumentTemplateClient">
                                                                            <ClientSideEvents SelectedIndexChanged="frmDestinationV2OnChange" />
                                                                        </dx:ASPxComboBox>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </Content>
                                                    </roUserControls:roOptionPanelClient>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <roUserControls:roOptionPanelClient ID="opEmployeeMailDestination" runat="server" TypeOPanel="RadioOption" Width="100%" Height="Auto" Checked="False" Enabled="True" CConClick="frmDestinationV2OnChange();">
                                                        <Title>
                                                            <asp:Label ID="lblopEmployeeMailDestinationTitle" runat="server" Text="Cuenta de correo electrónico"></asp:Label>
                                                        </Title>
                                                        <Description>
                                                            <asp:Label ID="lblopEmployeeMailDestinationDesc" runat="server" Text="Se enviará a la cuenta de correo indicada en el campo de la ficha del empleado."></asp:Label>
                                                        </Description>
                                                        <Content>
                                                        </Content>
                                                    </roUserControls:roOptionPanelClient>
                                                </td>
                                            </tr>
                                            <tr style="display: none">
                                                <td>
                                                    <roUserControls:roOptionPanelClient ID="opEmployeeRouteDestination" runat="server" TypeOPanel="RadioOption" Width="100%" Height="Auto" Checked="False" Enabled="True" CConClick="frmDestinationV2OnChange();">
                                                        <Title>
                                                            <asp:Label ID="lblopEmployeeRouteDestinationTitle" runat="server" Text="Ubicación física"></asp:Label>
                                                        </Title>
                                                        <Description>
                                                            <asp:Label ID="lblopEmployeeRouteDestinationDesc" runat="server" Text="Seleccione una ubicación en el servidor para guardar los informes."></asp:Label>
                                                        </Description>
                                                        <Content>
                                                            <table style="width: 100%;">
                                                                <tr>
                                                                    <td style="padding-left: 25px; width: 30%;">
                                                                        <asp:Label ID="lblopEmployeeRouteDestinationField" runat="server" Text="Indique la carpeta destino: "></asp:Label>
                                                                    </td>
                                                                    <td style="width: 70%;">
                                                                        <dx:ASPxComboBox ID="cmbEmployeeRouteDestination" runat="server" Width="250px" Font-Size="11px" ForeColor="#2D4155"
                                                                            Font-Names="Arial;Verdana;Sans-Serif" IncrementalFilteringMode="Contains" ClientInstanceName="cmbEmployeeRouteDestinationClient">
                                                                            <ClientSideEvents SelectedIndexChanged="frmDestinationV2OnChange" />
                                                                        </dx:ASPxComboBox>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </Content>
                                                    </roUserControls:roOptionPanelClient>
                                                </td>
                                            </tr>
                                        </table>
                                    </Content>
                                </roUserControls:roOptionPanelClient>

                                <roUserControls:roOptionPanelClient ID="opTypeSingleExport" runat="server" TypeOPanel="RadioOption" Width="100%" Height="Auto" Checked="False" Enabled="True" CConClick="frmDestinationV2OnChange();">
                                    <Title>
                                        <asp:Label ID="lblTypeEmail" runat="server" Text="Cuenta de correo electrónico"></asp:Label>
                                    </Title>
                                    <Description>
                                        <asp:Label ID="lblTypeEmailNormalDesc" runat="server" Text="Indique una dirección de correo válida a enviar el informe."></asp:Label>
                                    </Description>
                                    <Content>
                                        <table style="width: 100%;">
                                            <tr>
                                                <td style="padding-left: 25px; width: 30%;">
                                                    <asp:Label ID="lblSelectMail" runat="server" Text="Dirección de correo: "></asp:Label>
                                                </td>
                                                <td style="width: 70%;">
                                                    <dx:ASPxTextBox ID="txtEmail" runat="server" Rows="2" Width="250" Height="18" ClientInstanceName="txtEmailClient">
                                                        <ClientSideEvents TextChanged="frmDestinationV2OnChange" />
                                                    </dx:ASPxTextBox>
                                                </td>
                                            </tr>
                                        </table>
                                    </Content>
                                </roUserControls:roOptionPanelClient>
                            </td>
                        </tr>
                    </table>
                </div>
            </div>
        </div>
    </form>
</body>
<script type="text/javascript" language="javascript">
    frmAddDestinationV2_ShowNew();
</script>
</html>
<div class="editViewBtns">
    <div class="editBtn" style="display: none; margin-left: auto;"><span id="previousEdition">Anterior</span></div>
    <div class="editBtn" style="display: none"><span id="nextEdition">Siguiente</span></div>
    <div class="editBtn"><span id="acceptEdition">Aceptar</span></div>
    <div class="editBtn"><span id="cancelEdition">Cancelar</span></div>
</div>