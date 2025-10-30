<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="EditDocument.aspx.vb" Inherits="VTLive40.EditDocument" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html lang="en" class="notranslate" translate="no" xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Ausencia Prolongada</title>
</head>
<body style="height: 460px !important; min-height: initial;">

    <form id="form1" runat="server" style="height: 420px">

        <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="True" EnableScriptLocalization="True" />

        <script language="javascript" type="text/javascript">

            var isUpdate = false;
            function PageBase_Load() {
                ConvertControls();
            }

            function SaveDeliveredDocument() {
                if (ASPxClientEdit.ValidateGroup(null, true)) {
                    showLoadingGrid(true);
                    CallbackSessionClient.PerformCallback("UPDATEDOCUMENT")
                }
            }

            function AddDocumentCallback(fileName, title) {
                txtDocumentPathClient.SetText(fileName);
                txtDocTitleClient.SetText(title);
            }

            function CallbackSession_CallbackComplete2(s, e) {
                var controlCaller = document.getElementById('<%= hdnControlCaller.ClientID%>').value;
                var pendingControl = controlCaller.replace("DocumentManagment", "DocumentPendingManagment");

                var documentGridController = eval("window.parent." + controlCaller + "_GridDocsClient");
                var documentStatusGridController = eval("window.parent." + pendingControl + "_ASPxCallbackPanelContenidoClient");
                showLoadingGrid(false);

                if (typeof (s.cpResultRO) != 'undefined' && (s.cpResultRO == "VALIDATED" || s.cpResultRO == "REJECTED" || s.cpResultRO == "INVALIDATED")) {
                    if (typeof documentGridController != 'undefined' && documentGridController != null) documentGridController.PerformCallback("RELOAD");
                    if (typeof documentStatusGridController != 'undefined' && documentStatusGridController != null) documentStatusGridController.PerformCallback("REFRESH");
                } else {
                    if (typeof documentGridController != 'undefined' && documentGridController != null) documentGridController.PerformCallback("RELOAD");
                    if (typeof documentStatusGridController != 'undefined' && documentStatusGridController != null) documentStatusGridController.PerformCallback("REFRESH");
                    ClosePopup();
                }
            }

            function ClosePopup() {
                var controlCaller = document.getElementById('<%= hdnControlCaller.ClientID%>').value;

                var popupController = eval("window.parent." + controlCaller + "_EditDocumentPopup");
                popupController.Hide();
            }
            var actualState = '';
            var actualComment = '';

            var commentPopup = {
                popup: null,
                show: function () { }
            };
            function ValidateState(state) {
                actualState = state;
                actualComment = '';

                if (ASPxClientEdit.ValidateGroup(null, true)) {
                    if (state == "REJECTED" || state == "INVALIDATED") {
                        commentPopup.popup = $('#CallbackSession_validateStatePopup').dxPopup({
                            fullScreen: false,
                            width: 400,
                            height: 200,
                            showTitle: true,
                            title: Globalize.formatMessage("roValidateState"),
                            visible: false,
                            dragEnabled: true,
                            hideOnOutsideClick: false
                        }).dxPopup("instance");

                        commentPopup.show = function () {
                            commentPopup.popup.show();

                            $('#CallbackSession_txtSupervisorRemarks').dxTextArea({
                                value: '',
                                height: 90,
                                valueChangeEvent: "change",
                                onValueChanged: function (data) {
                                    actualComment = data.value;
                                }
                            });

                            $('#CallbackSession_btnCancelAddNode').dxButton({
                                text: Globalize.formatMessage("Cancel"),
                                onClick: function () {
                                    CloseSaveStatePopup();
                                }
                            });

                            $('#CallbackSession_btnOkAddNode').dxButton({
                                text: Globalize.formatMessage("Done"),
                                onClick: function () {
                                    ValidateStateFinal(actualState + '@@' + actualComment);
                                }
                            });
                        }

                        commentPopup.show();

                    } else ValidateStateFinal(state);
                }
            }

            function ValidateStateFinal(state) {
                if (commentPopup.popup != null) commentPopup.popup.hide();
                showLoadingGrid(true);
                CallbackSessionClient.PerformCallback(state)
            }

            function CloseSaveStatePopup() {
                commentPopup.popup.hide();
                commentPopup.popup = null;
            }

            function DateValidation(s, e) {
                var beginDate = detValidityBeginDateClient.GetDate();
                var endDate = detValidityEndDateClient.GetDate();
                //si los dos tienen datos
                if (beginDate != null && endDate != null) {
                    //valido si la fecha final sea mayor a la actual
                    //if (endDate < new Date() || endDate < beginDate) {
                    if (endDate < beginDate) {
                        e.isValid = false;
                        e.errorText = "(*)";
                        return;
                    }
                } //else if (endDate != null) {
                //  if (endDate < new Date()) {
                //      e.isValid = false;
                //      e.errorText = "(*)";
                //      return;
                //  }
                //}
                e.isValid = true;
                return;
            }
        </script>

        <asp:HiddenField ID="hdnIdEmployee" runat="server" />
        <asp:HiddenField ID="hdnBeginDate" runat="server" />
        <asp:HiddenField ID="hdnState" runat="server" />
        <asp:HiddenField ID="hdnControlCaller" runat="server" />

        <dx:ASPxCallbackPanel ID="CallbackSession" runat="server" Width="100%" ClientInstanceName="CallbackSessionClient">
            <SettingsLoadingPanel Enabled="false" />
            <ClientSideEvents EndCallback="CallbackSession_CallbackComplete2" />
            <PanelCollection>
                <dx:PanelContent ID="PanelContent1" runat="server">
                    <div style="width: 96%; padding: 0px !important;" class="bodyPopupExtended">
                        <div style="min-height: 385px">
                            <!-- Definicion -->
                            <div style="padding-left: 10px; padding-right: 10px; padding-top: 10px">
                                <div class="panBottomMargin">
                                    <div class="panHeader2 panBottomMargin">
                                        <span class="panelTitleSpan">
                                            <asp:Label runat="server" ID="lblDefinition" Text="Definición"></asp:Label>
                                        </span>
                                    </div>
                                </div>
                                <div class="panBottomMargin">
                                    <div class="divRowDescription" style="padding-left: 0px">
                                        <asp:Label ID="lblDefincionDescription" runat="server" Text="Documento que ha entregado el empleado para su validación."></asp:Label>
                                    </div>
                                    <div>
                                        <roUserControls:roGroupBox ID="RoGroupBox2" runat="server" style="width: 100%">
                                            <Content>
                                                <div class="divRow">
                                                    <div class="splitDivLeft">
                                                        <div class="editDocumentLeft">
                                                            <asp:Label ID="lblDocumentTemplate" runat="server" Text="Plantilla Documento:"></asp:Label>
                                                        </div>
                                                        <div class="editDocumentRight">
                                                            <dx:ASPxComboBox ID="cmbDocumentTemplate" runat="server" Width="150" AutoPostBack="false" ClientInstanceName="cmbDocumentTemplateClient">
                                                                <ValidationSettings SetFocusOnError="True">
                                                                    <RequiredField IsRequired="True" ErrorText="(*)" />
                                                                </ValidationSettings>
                                                            </dx:ASPxComboBox>
                                                        </div>
                                                    </div>
                                                    <div class="splitDivRight">
                                                        <div class="editDocumentLeft">
                                                            <asp:Label ID="lblReceivedDate" runat="server" Text="Fecha Recepción:"></asp:Label>
                                                        </div>
                                                        <div class="editDocumentRight">
                                                            <dx:ASPxDateEdit runat="server" ID="detReceivedDate" Width="150" ClientInstanceName="detReceivedDateClient" PopupVerticalAlign="WindowCenter" PopupHorizontalAlign="OutsideRight">
                                                                <ValidationSettings SetFocusOnError="True">
                                                                    <RequiredField IsRequired="True" ErrorText="(*)" />
                                                                </ValidationSettings>
                                                            </dx:ASPxDateEdit>
                                                        </div>
                                                    </div>
                                                </div>
                                                <br />
                                                <div class="divRow">
                                                    <div class="splitDivLeft">
                                                        <div class="editDocumentLeft">
                                                            <asp:Label ID="lblDocumentTitle" runat="server" Text="Título:"></asp:Label>
                                                        </div>
                                                        <div class="editDocumentRight">
                                                            <dx:ASPxTextBox runat="server" ID="txtDocTitle" Width="150" ClientInstanceName="txtDocTitleClient">
                                                                <ValidationSettings SetFocusOnError="True">
                                                                    <RequiredField IsRequired="True" ErrorText="(*)" />
                                                                </ValidationSettings>
                                                            </dx:ASPxTextBox>
                                                        </div>
                                                    </div>
                                                    <div class="splitDivRight">
                                                        <div class="editDocumentLeft">
                                                            <asp:Label ID="lblDocumento" runat="server" Text="Documento:"></asp:Label>
                                                        </div>
                                                        <div class="editDocumentRight">
                                                            <dx:ASPxTextBox runat="server" ID="txtDocumentPath" ReadOnly="true" Width="150" ClientInstanceName="txtDocumentPathClient" BackColor="#DBDBDB" />
                                                        </div>
                                                    </div>
                                                </div>
                                                <br />
                                                <div class="divRow">
                                                    <div class="splitDivLeft">
                                                        <div class="editDocumentLeft">
                                                            <asp:Label ID="lblValidityBeginDate" runat="server" Text="Inicio:"></asp:Label>
                                                        </div>
                                                        <div class="editDocumentRight">
                                                            <dx:ASPxDateEdit runat="server" ID="detValidityBeginDate" Width="150" ClientInstanceName="detValidityBeginDateClient" PopupVerticalAlign="WindowCenter" PopupHorizontalAlign="OutsideRight">
                                                            </dx:ASPxDateEdit>
                                                        </div>
                                                    </div>
                                                    <div class="splitDivRight">
                                                        <div class="editDocumentLeft">
                                                            <asp:Label ID="lblValidityEndDate" runat="server" Text="Fin:"></asp:Label>
                                                        </div>
                                                        <div class="editDocumentRight">
                                                            <dx:ASPxDateEdit runat="server" ID="detValidityEndDate" Width="150" ClientInstanceName="detValidityEndDateClient" PopupVerticalAlign="WindowCenter" PopupHorizontalAlign="OutsideRight">
                                                                <ClientSideEvents Validation="DateValidation" />
                                                                <ValidationSettings SetFocusOnError="True">
                                                                    <RequiredField IsRequired="True" ErrorText="(*)" />
                                                                </ValidationSettings>
                                                            </dx:ASPxDateEdit>
                                                        </div>
                                                    </div>
                                                </div>
                                                <br />
                                                <div class="divRow">
                                                    <div style="float: right; padding-left: 5px;" id="divRejectDoc">
                                                        <dx:ASPxButton ID="btnReject" runat="server" AutoPostBack="False" CausesValidation="False"
                                                            ClientInstanceName="btnRejectClient" GroupName="Status"
                                                            Text="Rechazar" Width="100">
                                                            <Image Url="~/Base/Images/Rechazado.png"></Image>
                                                            <FocusRectBorder BorderStyle="Groove" BorderWidth="1px" BorderColor="ActiveBorder" />
                                                            <ClientSideEvents Click="function(s,e) { ValidateState('REJECTED') }" />
                                                        </dx:ASPxButton>
                                                    </div>
                                                    <div style="float: right; padding-left: 5px;" id="divInvalidate">
                                                        <dx:ASPxButton ID="btnInvalidateDoc" runat="server" AutoPostBack="False" CausesValidation="False"
                                                            ClientInstanceName="btnInvalidateDocClient" GroupName="Status"
                                                            Text="Invalidar" Width="100">
                                                            <Image Url="~/Base/Images/Cancelada.png"></Image>
                                                            <FocusRectBorder BorderStyle="Groove" BorderWidth="1px" BorderColor="ActiveBorder" />
                                                            <ClientSideEvents Click="function(s,e) { ValidateState('INVALIDATED') }" />
                                                        </dx:ASPxButton>
                                                    </div>
                                                    <div style="float: right; padding-left: 5px;" id="divValidateDoc">
                                                        <dx:ASPxButton ID="btnValidate" runat="server" AutoPostBack="False" CausesValidation="False"
                                                            ClientInstanceName="btnValidateClient" GroupName="Status"
                                                            Text="Validar" Width="100">
                                                            <Image Url="~/Base/Images/Finalizada.png"></Image>
                                                            <FocusRectBorder BorderStyle="Groove" BorderWidth="1px" BorderColor="ActiveBorder" />
                                                            <ClientSideEvents Click="function(s,e) { ValidateState('VALIDATED') }" />
                                                        </dx:ASPxButton>
                                                    </div>
                                                </div>
                                            </Content>
                                        </roUserControls:roGroupBox>
                                    </div>
                                </div>
                            </div>

                            <!-- Estado -->
                            <div style="padding-left: 10px; padding-right: 10px;">
                                <div class="panBottomMargin">
                                    <div class="panHeader2 panBottomMargin">
                                        <span class="panelTitleSpan">
                                            <asp:Label runat="server" ID="lblDocStateGeneral" Text="Estado"></asp:Label>
                                        </span>
                                    </div>
                                </div>
                                <div class="panBottomMargin">
                                    <div class="divRowDescription" style="padding-left: 0px">
                                        <asp:Label ID="lblStatusDescription" runat="server" Text="Estado y fecha del último estado en el que se encuentra el documento."></asp:Label>
                                    </div>
                                    <div>
                                        <roUserControls:roGroupBox ID="RoGroupBox1" runat="server" style="width: 100%">
                                            <Content>
                                                <div class="divRow">
                                                    <div id="lblUploadedByDescription" runat="server"></div>
                                                    <br />
                                                    <div id="lblActualStateDescription" runat="server"></div>
                                                    <br />
                                                    <div id="lblApprovedByDescription" runat="server"></div>
                                                </div>
                                                <div class="divRow" style="display: none">
                                                    <div class="splitDivLeft">
                                                        <div class="editDocumentLeft">
                                                            <asp:Label ID="lblStatus" runat="server" Text="Estado Actual:"></asp:Label>
                                                        </div>
                                                        <div class="editDocumentRight">
                                                            <div class="editDocumentLeft">
                                                                <dx:ASPxTextBox runat="server" ID="txtDocStatus" Width="150" ReadOnly="true" ClientInstanceName="txtDocStatusClient" BackColor="#DBDBDB" />
                                                            </div>
                                                            <div class="editDocumentRight" style="width: 95px;">
                                                                <img src="../Base/Images/Finalizada.png" alt="" id="imgState" />
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div class="splitDivRight">
                                                        <div class="editDocumentLeft">
                                                            <asp:Label ID="lblStatusDate" runat="server" Text="Fecha cambio estado:"></asp:Label>
                                                        </div>
                                                        <div class="editDocumentRight">
                                                            <dx:ASPxDateEdit runat="server" ID="detStatusDate" Width="150" ClientInstanceName="detStatusDateClient" ReadOnly="true" BackColor="#DBDBDB" />
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="divRow" style="display: none">
                                                    <div class="splitDivLeft">
                                                        <div class="editDocumentLeft">
                                                            <asp:Label ID="lblDocLevel" runat="server" Text="Nivel:"></asp:Label>
                                                        </div>
                                                        <div class="editDocumentRight">
                                                            <dx:ASPxTextBox runat="server" ID="txtLevel" Width="150" ClientInstanceName="txtLevelClient" ReadOnly="true" BackColor="#DBDBDB">
                                                            </dx:ASPxTextBox>
                                                        </div>
                                                    </div>
                                                    <div class="splitDivRight">
                                                        <div class="editDocumentLeft">
                                                            <asp:Label ID="lblSupervisor" runat="server" Text="Supervisor:"></asp:Label>
                                                        </div>
                                                        <div class="editDocumentRight">
                                                            <div style="float: left">
                                                                <dx:ASPxTextBox runat="server" ID="txtSupervisor" Width="150" ClientInstanceName="txtSupervisorClient" ReadOnly="true" BackColor="#DBDBDB">
                                                                    <ValidationSettings SetFocusOnError="True">
                                                                        <RequiredField IsRequired="True" ErrorText="(*)" />
                                                                    </ValidationSettings>
                                                                </dx:ASPxTextBox>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </Content>
                                        </roUserControls:roGroupBox>
                                    </div>
                                </div>
                            </div>

                            <!-- Botones -->
                            <div>
                                <table cellpadding="1" cellspacing="1" border="0" style="width: 100%; padding-bottom: 10px;">
                                    <tr>
                                        <td></td>
                                        <td align="right" style="padding-right: 5px;">
                                            <table>
                                                <tr>
                                                    <td>

                                                        <dx:ASPxButton ID="btOK" runat="server" AutoPostBack="False" CausesValidation="False" Text="${Button.Accept}" ToolTip="${Button.Accept}" HoverStyle-CssClass="btnFlat-hover btnFlatBlack-hover" CssClass="btnFlat btnFlatBlack">
                                                            <ClientSideEvents Click="function(s,e){SaveDeliveredDocument();}" />
                                                        </dx:ASPxButton>
                                                    </td>
                                                    <td>
                                                        <dx:ASPxButton ID="btCancel" runat="server" AutoPostBack="False" CausesValidation="False" Text="${Button.Cancel}" ToolTip="${Button.Cancel}" HoverStyle-CssClass="btnFlat-hover btnFlatBlack-hover" CssClass="btnFlat btnFlatBlack">
                                                            <ClientSideEvents Click="function(s,e){ClosePopup();}" />
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

                    <div id="validateStatePopup" runat="server" style="display: none">
                        <div style="width: 100%">
                            <div id="txtSupervisorRemarks" runat="server"></div>
                        </div>
                        <div style="padding-top: 10px; width: 100%">
                            <div id="btnCancelAddNode" runat="server" style="float: right">
                            </div>
                            <div id="btnOkAddNode" class="acceptButton" runat="server" style="float: right; margin-right: 15px;">
                            </div>
                        </div>
                    </div>
                </dx:PanelContent>
            </PanelCollection>
        </dx:ASPxCallbackPanel>

        <Local:MessageFrame ID="MessageFrame1" runat="server" />
    </form>
</body>
</html>