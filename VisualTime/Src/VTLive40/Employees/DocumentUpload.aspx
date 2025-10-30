<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="DocumentUpload.aspx.vb" Inherits="VTLive40.DocumentUpload" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html lang="en" class="notranslate" translate="no" xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Ausencia Prolongada</title>
</head>
<body style="">

    <form id="form1" runat="server">

        <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="True" EnableScriptLocalization="True" />

        <script language="javascript" type="text/javascript">

            function PageBase_Load() {
                ConvertControls();

                changeDocName(cmbDocumentTemplateClient.GetSelectedItem());
            }

            function ChangeText(e) {
                var item = document.getElementById('fUploader').value;
                var lastItem = item.split("\\").pop();
                var name = lastItem.substr(0, lastItem.lastIndexOf('.'))
                txtDocTitleClient.SetText(name);
            }
            function SaveDeliveredDocument() {
                if (ASPxClientEdit.ValidateGroup(null, true)) {
                    showLoadingGrid(true);
                    CallbackSessionClient.PerformCallback("SAVEDOCUMENT")
                }
            }

            function AddDocumentCallback(fileName, title) {
                txtDocumentPathClient.SetText(fileName);
                txtDocTitleClient.SetText("");
            }

            function Close_Response() {
                ClosePopup();
            }

            function CallbackSession_CallbackComplete2(s, e) {
                ClosePopup();
            }

            function ClosePopup() {
                var controlCaller = document.getElementById('<%= hdnControlCaller.ClientID%>').value;
                var pendingControl = controlCaller.replace("DocumentManagment", "DocumentPendingManagment");

                var documentGridController = eval("window.parent." + controlCaller + "_GridDocsClient");
                var documentStatusGridController = eval("window.parent." + pendingControl + "_ASPxCallbackPanelContenidoClient");
                showLoadingGrid(false);

                if (typeof documentGridController != 'undefined' && documentGridController != null) documentGridController.PerformCallback("RELOAD");
                if (typeof documentStatusGridController != 'undefined' && documentStatusGridController != null) documentStatusGridController.PerformCallback("REFRESH");

                var popupController = eval("window.parent." + controlCaller + "_NewDocumentPopup");
                popupController.Hide();

                try {
                    var selectedItem = cmbDocumentTemplateClient.GetSelectedItem();
                    if (typeof selectedItem != 'undefined' && selectedItem != null) {
                        var docType = parseInt(selectedItem.value.split("_")[1], 10);
                        if (docType == 1) window.parent.reloadparent();
                    }
                } catch (e) { }
            }

            function changeDocName(selectedItem) {

                if (typeof selectedItem != 'undefined' && selectedItem != null) {
                    var docType = parseInt(selectedItem.value.split("_")[1], 10);
                    if (docType == 1) $("#receptionDate").show();
                    else $("#receptionDate").hide();

                    txtDocTitleClient.SetText("");
                }
            }
        </script>

        <asp:HiddenField ID="hdnIdEmployee" runat="server" />
        <asp:HiddenField ID="hdnBeginDate" runat="server" />
        <asp:HiddenField ID="hdnState" runat="server" />
        <asp:HiddenField ID="hdnControlCaller" runat="server" />
        <dx:ASPxCallback ID="CallbackSession" runat="server" ClientInstanceName="CallbackSessionClient" ClientSideEvents-CallbackComplete="CallbackSession_CallbackComplete2"></dx:ASPxCallback>

        <div class="bodyPopupExtended">
            <div style="min-height: 250px">
                <!-- Definicion -->
                <div>
                    <div class="panBottomMargin">
                        <div class="panHeader2 panBottomMargin">
                            <span class="panelTitleSpan">
                                <asp:Label runat="server" ID="lblDefinition" Text="Definición de documento"></asp:Label>
                            </span>
                        </div>
                    </div>

                    <div class="divRow">
                        <div class="divRowDescription">
                            <asp:Label ID="lblDocumentTemplateTitleDesc" runat="server" Text="Indique la plantilla de documento que va a utilizar:"></asp:Label>
                        </div>
                        <asp:Label ID="lblDocumentTemplateTitle" runat="server" Text="Plantilla:" CssClass="labelForm"></asp:Label>
                        <div class="componentForm">
                            <dx:ASPxComboBox ID="cmbDocumentTemplate" runat="server" Width="150" AutoPostBack="false" ClientInstanceName="cmbDocumentTemplateClient">
                                <ClientSideEvents SelectedIndexChanged="function(s,e ){changeDocName(s.GetSelectedItem());}" />
                                <ValidationSettings SetFocusOnError="True">
                                    <RequiredField IsRequired="True" ErrorText="(*)" />
                                </ValidationSettings>
                            </dx:ASPxComboBox>
                        </div>
                    </div>
                    <div class="divRow" id="receptionDate">
                        <div class="divRowDescription">
                            <asp:Label ID="lblReceptionDateTitleDesc" runat="server" Text="Indique la fecha en la que se ha recibido el documento del empleado:"></asp:Label>
                        </div>
                        <asp:Label ID="lblReceptionDateTitle" runat="server" Text="Fecha rececpción:" CssClass="labelForm"></asp:Label>
                        <div class="componentForm">
                            <dx:ASPxDateEdit runat="server" ID="detReceivedDate" Width="150" ClientInstanceName="detReceivedDateClient">
                            </dx:ASPxDateEdit>
                        </div>
                    </div>
                    <br />
                    <div class="divRow">
                        <div class="divRowDescription">
                            <asp:Label ID="lblFileUploadDesc" runat="server" Text="Seleccione el fichero que desea adjuntar:"></asp:Label>
                        </div>
                        <asp:Label ID="lblFileUpload" runat="server" Text="Documento:" CssClass="labelForm"></asp:Label>
                        <div class="componentForm">
                            <asp:FileUpload ID="fUploader" onchange="ChangeText();" runat="server" />
                            <dx:ASPxCheckBox ID="ckDocumentNotMandatory" runat="server" Text="Documento no requerido" />
                        </div>
                    </div>
                    <br />
                    <div class="divRow">
                        <div class="divRowDescription">
                            <asp:Label ID="lblDocumentNameDesc" runat="server" Text="Indique el nombre del documento:"></asp:Label>
                        </div>
                        <asp:Label ID="lblDocumentName" runat="server" Text="Nombre:" CssClass="labelForm"></asp:Label>
                        <div class="componentForm">
                            <dx:ASPxTextBox runat="server" ID="txtDocTitle" Width="150" ClientInstanceName="txtDocTitleClient">
                                <ValidationSettings SetFocusOnError="True">
                                    <RequiredField IsRequired="True" ErrorText="(*)" />
                                </ValidationSettings>
                            </dx:ASPxTextBox>
                        </div>
                    </div>
                </div>
                <br />

                <!-- Botones -->
                <div>
                    <table cellpadding="1" cellspacing="1" border="0" style="height: 30px; width: 100%;">
                        <tr style="height: 40px">
                            <td></td>
                            <td align="right" style="padding-right: 5px;">
                                <table>
                                    <tr>
                                        <td>

                                            <dx:ASPxButton ID="btOK" runat="server" AutoPostBack="true" CausesValidation="False" Text="${Button.Accept}" ToolTip="${Button.Accept}" HoverStyle-CssClass="btnFlat-hover btnFlatBlack-hover" CssClass="btnFlat btnFlatBlack">
                                                <%--<ClientSideEvents Click="function(s,e){SaveDeliveredDocument();}" />--%>
                                            </dx:ASPxButton>
                                        </td>
                                        <td>
                                            <dx:ASPxButton ID="btCancel" Text="${Button.Cancel}" AutoPostBack="true" runat="server" HoverStyle-CssClass="btnFlat-hover btnFlatBlack-hover" CssClass="btnFlat btnFlatBlack">
                                                <ClientSideEvents Click="function(s,e){ ClosePopup(); return false; }" />
                                            </dx:ASPxButton>
                                        </td>
                                    </tr>
                                </table>
                                <asp:HiddenField ID="hdnCanClose_PageBase" runat="server" />
                                <asp:HiddenField ID="hdnMustRefresh_PageBase" Value="0" runat="server" />
                                <asp:HiddenField ID="hdnParams_PageBase" runat="server" />
                            </td>
                        </tr>
                    </table>
                </div>
            </div>
        </div>

        <Local:MessageFrame ID="MessageFrame1" runat="server" />
    </form>
</body>
</html>