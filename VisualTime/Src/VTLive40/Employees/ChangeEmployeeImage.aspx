<%@ Page Language="VB" AutoEventWireup="false" Inherits="VTLive40.ChangeEmployeeImage" CodeBehind="ChangeEmployeeImage.aspx.vb" %>

<html lang="en" class="notranslate" translate="no" xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Cambiar foto ${Employee}</title>
</head>
<body class="bodyPopup">
    <form id="frmChangeEmployeeImage" runat="server" enctype="multipart/form-data">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="True" EnableScriptLocalization="True" />
        <div>

            <script language="javascript" type="text/javascript">

                function PageBase_Load() {

                }
            </script>

            <asp:HiddenField ID="hdnCanClose_PageBase" runat="server" />
            <asp:HiddenField ID="hdnMustRefresh_PageBase" Value="0" runat="server" />

            <table cellpadding="0" cellspacing="0" width="100%" style="padding-top: 5px; padding-left: 5px" border="0">
                <tr>
                    <td style="width: 40px" align="center" valign="middle">
                        <asp:Image ID="imgChangeEmployeeImage" ImageUrl="~/Base/Images/camera.png" runat="server" />
                    </td>
                    <td align="left" style="padding-left: 10px;">
                        <asp:Label ID="lblInfo" runat="server" CssClass="editTextFormat" Text="Introduzca el nombre del fichero de la foto" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2" style="padding: 4px; padding-bottom: 0;" align="center" valign="top" height="30px">
                        <asp:FileUpload ID="fuImageFile" runat="server" EnableViewState="true" class="textClass" Style="width: 100%;" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2" align="right" valign="bottom">
                        <table border="0" width="100%" cellpadding="0" cellspacing="0">
                            <tr>
                                <td colspan="3" align="left" style="padding-bottom: 4px;">
                                    <asp:CheckBox ID="checkDel" Checked="false" Text="Eliminar la foto" runat="server" />
                                </td>
                            </tr>
                            <tr>
                                <td width="70px" height="30px" valign="bottom">&nbsp;</td>
                                <td>
                                    <asp:Button ID="btOK" Text="${Button.Accept}" runat="server" OnClientClick="" CssClass="btnFlat btnFlatBlack btnFlatAsp" />
                                </td>
                                <td>
                                    <asp:Button ID="btCancel" Text="${Button.Cancel}" runat="server" OnClientClick="Close(); return false;" CssClass="btnFlat btnFlatBlack btnFlatAsp" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>