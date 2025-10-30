<%@ Page Language="VB" AutoEventWireup="false" Inherits="VTLive40.Base_EditObjectPopup" CodeBehind="EditObjectPopup.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html lang="en" class="notranslate" translate="no" xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript">
        function CloseClick() {
            window.parent.EditObjectPopup_Client.SetContentUrl('');
            window.parent.EditObjectPopup_Client.Hide();
        }
        function AcceptClick() {
            if (newObjectName_Client.GetValue() != null && newObjectName_Client.GetValue() != "") {
                window.parent.EditObjectCallback(newObjectName_Client.GetValue());
                window.parent.EditObjectPopup_Client.SetContentUrl('');
                window.parent.EditObjectPopup_Client.Hide();
            } else {
                lblError_Client.SetVisible(true);
            }
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <dx:ASPxPanel ID="panelUploadContent" runat="server" Width="0px" Height="0px">
            <PanelCollection>
                <dx:PanelContent ID="PopupUploadPanelContent" runat="server">
                    <div class="bodyPopupExtended" style="table-layout: fixed; height: 175px; width: 375px;">
                        <table width="100%">
                            <tr>
                                <td style="padding-top: 5px; padding-bottom: 10px;" height="20px" valign="top">
                                    <div class="panHeader2">
                                        <span style="">
                                            <asp:Label ID="lblTitle" runat="server" Text="Nombre del objecto"></asp:Label>
                                        </span>
                                    </div>
                                </td>
                            </tr>
                            <tr valign="top">
                                <td>
                                    <table>
                                        <tr>
                                            <td valign="top" style="padding-left: 15px; padding-right: 15px;">
                                                <img alt="" id="Img1" src="~/Base/Images/logovtl.ico" runat="server" />
                                            </td>
                                            <td align="left" valign="middle">
                                                <asp:Label ID="lblDescription1" runat="server" CssClass="editTextFormat" Text="Introduzca el nuevo para el objecto:"></asp:Label>
                                                <br />
                                                <strong>
                                                    <asp:Label ID="lblDescription2" runat="server" CssClass="editTextFormat" Text=""></asp:Label></strong>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr valign="top">
                                <td>
                                    <table style="padding-left: 95px;">
                                        <tr>
                                            <td>
                                                <table style="width: 100%" cellspacing="0" cellpadding="0" border="0">
                                                    <tr>
                                                        <td>
                                                            <asp:Label ID="lblName" runat="server" CssClass="editTextFormat" Text="Nuevo Nombre"></asp:Label>
                                                        </td>
                                                        <td style="padding-left: 5px">
                                                            <dx:ASPxTextBox ID="newObjectName" runat="server" ClientInstanceName="newObjectName_Client" />
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>

                        <table style="margin-left: auto; margin-right: auto">
                            <tr>
                                <td>
                                    <dx:ASPxLabel ID="lblError" ClientInstanceName="lblError_Client" runat="server" Text="" Font-Bold="True" Font-Size="13px" Height="30" Style="white-space: nowrap; color: Red" />
                                </td>
                            </tr>
                        </table>
                        <table style="margin-left: auto;">
                            <tr>
                                <td>
                                    <dx:ASPxButton ID="btnAccept" runat="server" AutoPostBack="False" CausesValidation="False" Text="${Button.Accept}" ToolTip="${Button.Accept}"
                                        HoverStyle-CssClass="btnFlat-hover btnFlatBlack-hover" CssClass="btnFlat btnFlatBlack">
                                        <ClientSideEvents Click="AcceptClick" />
                                    </dx:ASPxButton>
                                </td>
                                <td>
                                    <dx:ASPxButton ID="btnCancel" runat="server" AutoPostBack="False" CausesValidation="False" Text="${Button.Cancel}" ToolTip="${Button.Cancel}"
                                        HoverStyle-CssClass="btnFlat-hover btnFlatBlack-hover" CssClass="btnFlat btnFlatBlack">
                                        <ClientSideEvents Click="CloseClick" />
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