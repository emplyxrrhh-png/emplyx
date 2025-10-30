<%@ Page Language="VB" AutoEventWireup="false" Inherits="VTLive40.UploadDataFile" CodeBehind="UploadDataFile.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html lang="en" class="notranslate" translate="no" xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Subir fichero de datos del terminal</title>
</head>
<body>
    <form id="frmAddDocumentsTracking" runat="server" style="width: 100%; height: 100%">
        <dx:ASPxPanel ID="panelUploadContent" runat="server" Width="0px" Height="0px">
            <PanelCollection>
                <dx:PanelContent ID="PopupUploadPanelContent" runat="server">
                    <div class="bodyPopupExtended" style="table-layout: fixed; height: 125px; width: 350px">
                        <div class="panHeader2">
                            <span style="">
                                <asp:Label ID="lblPopupUploadTitle" runat="server" Text="Subir fichero de datos del terminal" /></span>
                        </div>
                        <br />
                        <table>
                            <tr>
                                <td>
                                    <asp:Label ID="lblSelectFile" runat="server" Text="Selecciona fichero:" />
                                </td>
                                <td>
                                    <asp:FileUpload ID="fUploader" runat="server" />
                                </td>
                            </tr>
                        </table>

                        <!-- BOTONES -->
                        <table style="margin-left: auto; margin-right: auto;">
                            <tr>
                                <td>
                                    <dx:ASPxButton ID="btnPopupUpload" runat="server" AutoPostBack="True" CausesValidation="False" Text="${Button.Accept}" ToolTip="${Button.Accept}"
                                        HoverStyle-CssClass="btnFlat-hover btnFlatBlack-hover" CssClass="btnFlat btnFlatBlack" OnClick="uploadClick">
                                    </dx:ASPxButton>
                                </td>
                                <td>
                                    <dx:ASPxButton ID="btnPopupUploadCancel" runat="server" AutoPostBack="False" CausesValidation="False" Text="${Button.Cancel}" ToolTip="${Button.Cancel}"
                                        HoverStyle-CssClass="btnFlat-hover btnFlatBlack-hover" CssClass="btnFlat btnFlatBlack">
                                        <ClientSideEvents Click="function(s, e) { window.parent.PopupTerminalFile_Client.Hide(); }" />
                                    </dx:ASPxButton>
                                </td>
                            </tr>
                        </table>
                    </div>
                </dx:PanelContent>
            </PanelCollection>
        </dx:ASPxPanel>
    </form>
</body>
</html>