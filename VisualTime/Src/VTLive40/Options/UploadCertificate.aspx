<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="UploadCertificate.aspx.vb" Inherits="VTLive40.UploadCertificate" %>

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
            }

            function Close_Response() {
                ClosePopup();
            }

            function CallbackSession_CallbackComplete2(s, e) {
                ClosePopup();
            }

            function ClosePopup() {
                var controlCaller = document.getElementById('<%= hdnControlCaller.ClientID%>').value;
                showLoadingGrid(false);

                var popupController = eval("window.parent." + controlCaller + "_NewCertificatePopup");
                popupController.Hide();
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
                                <asp:Label runat="server" ID="lblPGPDefinition" Text="Suba su clave pública"></asp:Label>
                            </span>
                        </div>
                    </div>
                    <div class="divRow">
                        <div class="divRowDescription">
                            <asp:Label ID="lblFileUploadDesc" runat="server" Text="Seleccione el fichero que desea adjuntar:"></asp:Label>
                        </div>
                        <asp:Label ID="lblFileUploaded" runat="server" Text="Ya tiene una clave subida, si sube otro se sobrescribirá."></asp:Label>
                        <asp:Label ID="lblFileUpload" runat="server" Text="Clave:" CssClass="labelForm"></asp:Label>
                        <div class="componentForm">
                            <asp:FileUpload ID="fUploader" runat="server" />
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