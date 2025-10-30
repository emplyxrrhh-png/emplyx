<%@ Page Language="VB" AutoEventWireup="false" Inherits="VTLive40.Base_AboutMe" CodeBehind="AboutMe.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html lang="en" class="notranslate" translate="no" xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript">

        function hideAboutMe() {
            window.parent.AboutMe_Client.SetContentUrl('');
            window.parent.AboutMe_Client.Hide();
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <dx:ASPxPanel ID="AboutMeContent" runat="server" Width="100%" Height="100%">
            <PanelCollection>
                <dx:PanelContent ID="PopupUploadPanelContent" runat="server">
                    <div class="bodyPopupExtended" style="table-layout: fixed; height: 275px; width: 675px;">
                        <table width="100%">
                            <tr>
                                <td style="padding-top: 5px; padding-bottom: 10px;" height="20px" valign="top">
                                    <div class="panHeader2">
                                        <span style="">
                                            <asp:Label ID="lblTitle" runat="server" Text="Acerca de"></asp:Label>
                                        </span>
                                    </div>
                                </td>
                            </tr>
                            <tr valign="top">
                                <td style="height: 160px;">
                                    <table>
                                        <tr>
                                            <td valign="top" style="padding-left: 15px; padding-right: 15px;">
                                                <img alt="" id="Img1" src="~/Base/Images/logovtl.ico" runat="server" />
                                            </td>
                                            <td align="left" valign="middle">
                                                <asp:Label ID="lblDescription1" runat="server" CssClass="editTextFormat" Text="Acerca de"></asp:Label>
                                                <br />
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>

                            <tr>
                                <td style="height: 70px" align="center" valign="middle">
                                    <div id="divAcceptAboutMe">
                                        <asp:Button ID="btChangePwdOK" Text="${Button.Accept}" runat="server" TabIndex="3" OnClientClick="hideAboutMe();" CssClass="btnFlat btnFlatBlack btnFlatAsp" />
                                    </div>
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
                        <br />
                    </div>
                </dx:PanelContent>
            </PanelCollection>
        </dx:ASPxPanel>
    </form>
</body>
</html>