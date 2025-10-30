<%@ Page Language="VB" AutoEventWireup="false" Inherits="VTLive40.Base_PerformingActionAndCancel" CodeBehind="PerformingActionAndCancel.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html lang="en" class="notranslate" translate="no" xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript">
</script>
</head>
<body>
    <input type="hidden" runat="server" id="hdnCaptchaAction" />
    <form id="form1" runat="server">

        <dx:ASPxPanel ID="panelUploadContent" runat="server" Width="0px" Height="0px">
            <PanelCollection>
                <dx:PanelContent ID="PopupUploadPanelContent" runat="server">
                    <div class="bodyPopupExtended" style="table-layout: fixed; height: 125px; left: 4px; width: 430px; position: relative; top: 10px;">
                        <table cellpadding="0" cellspacing="0" width="100%" height="100%" class="bodyPopup">
                            <tr style="height: 25px">
                                <td align="center" valign="middle" style="padding-left: 10px">
                                    <asp:Label ID="lblupExecuteTitle" Text="Realizando acción ..." CssClass="UpdateProgressMessage" runat="server" />
                                </td>
                                <td align="right" valign="top"></td>
                            </tr>
                            <tr>
                                <td colspan="2" style="padding-top: 5px; padding-left: 10px" align="center">
                                    <asp:Label ID="lblupExecuteInfo" Text="Esta operación puede tardar unos minutos dependiendo del volumen de información. Si lo desea puede ir al menú inicio mediante el siguiente enlace y posteriormente encontrar la analítica en 'Informes solicitados'" ForeColor="blue" runat="server" />
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2" align="center" valign="middle">
                                    <asp:Image ID="imgLoading" ImageUrl="~/Base/Images/Progress/yui/activity.gif" runat="server" />
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2" align="center">
                                    <dx:ASPxButton ID="btnGoToStart" runat="server" AutoPostBack="False" CausesValidation="False" Text="Ir a menú inicio" ToolTip="Ir a menú inicio" HoverStyle-CssClass="btnFlat-hover btnFlatBlack-hover" CssClass="btnFlat btnFlatBlack">
                                        <ClientSideEvents Click="function(s,e){ top.reenviaFrame('#Start','','',''); }" />
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