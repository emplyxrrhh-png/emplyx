<%@ Page Language="VB" AutoEventWireup="false" Inherits="VTLive40.Wizards_ImportsUpload" CodeBehind="ImportsUpload.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html lang="en" class="notranslate" translate="no" xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>ImportsUpload.aspx</title>    
</head>
<body>
    <form id="form1" runat="server">

        <script language="javascript" type="text/javascript">
            function SendFiles() {
                __doPostBack('btnSend', '')
            }

            function hidePopupLoader() {
                parent.hidePopupLoader();
            }            

            $(document).ready(function () {                                
                if ($(".selectedType").val() == "TEXT") {
                    $(".trLblSchema").show();
                    $(".lblSchema").show();
                    $(".trFileSchema").show();
                    $(".fileSchema").show();
                    $(".trSeparator").show();
                    $(".lblSelectTemplate").hide();
                } else {
                    $(".trLblSchema").hide();
                    $(".lblSchema").hide();
                    $(".trFileSchema").hide();
                    $(".fileSchema").hide();
                    $(".trSeparator").hide();
                    $(".lblSelectTemplate").show();
                }                    

                $(".rdList input").change(function () {                    
                    $(".selectedType").val($(this).val())
                    if ($(this).val() == "TEXT") {
                        $(".trLblSchema").show();
                        $(".lblSchema").show();
                        $(".trFileSchema").show();
                        $(".fileSchema").show();
                        $(".trSeparator").show(); 
                        $(".lblSelectTemplate").hide();                         
                    } else {
                        $(".trLblSchema").hide();
                        $(".lblSchema").hide();
                        $(".trFileSchema").hide();
                        $(".fileSchema").hide();
                        $(".trSeparator").hide();
                        $(".lblSelectTemplate").show();                         
                    }                    
                });                
            });
        </script>
        <input type="hidden" runat="server" id="hdnIsBusiness" value="0" />

        <table width="100%" style="margin-top: 30px;">
            <tr>
                <td valign="top" style="width: 100px;">
                    <asp:Label ID="lblTypeInf" runat="server" Text="Tipo de importación"></asp:Label></td>
                <td style="">
                    <table>
                        <tr>
                            <td style="padding-bottom: 10px;">
                                <asp:RadioButtonList ID="rdList" CssClass="rdList" runat="server">
                                    <asp:ListItem id="rdExcel" runat="server" Text="Excel" Value="EXCEL"></asp:ListItem>
                                    <asp:ListItem id="rdASCII" runat="server" Text="TEXT" Value="TEXT"></asp:ListItem>
                                    <asp:ListItem id="rdXML" runat="server" Text="XML" Value="XML"></asp:ListItem>
                                </asp:RadioButtonList>
                                <input type="hidden" id="selectedType" class="selectedType" runat="server" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr id="trSeparator" style="display:none;" runat="server" class="trSeparator">
                <td style="width: 100px">
                    <asp:Label ID="lblSeparator" runat="server" Text="Separador"></asp:Label></td>
                <td>
                    <asp:TextBox ID="txtSeparator" runat="server" Text="" Width="50px" MaxLength="1"></asp:TextBox></td>
            </tr>
        </table>
        <table width="100%" style="margin-top: 30px;">
            <tr>
                <td colspan="2">
                    <asp:Label ID="lblFileOrig" runat="server" Text="Fichero Origen"></asp:Label></td>
            </tr>
            <tr>
                <td colspan="2">
                    <asp:FileUpload ID="fileOrig" runat="server" Width="400" size="50" /></td>
            </tr>
            <tr class="trLblSchema" style="display:none;">
                <td colspan="2">
                    <asp:Label ID="lblSchema" CssClass="lblSchema" runat="server" Text="Plantilla" Visible="false"></asp:Label></td>
            </tr>
            <tr class="trFileSchema" style="display:none;">                
                <td colspan="2">                        
                    <asp:FileUpload ID="fileSchema" CssClass="fileSchema" runat="server" Width="400" size="50" Visible="false" /></td>
            </tr>
            <tr>                
    <td style="padding-top:10px;" colspan="2">           
        <asp:Label ID="lblSelectTemplate" CssClass="lblSelectTemplate labelImportForm" runat="server" Text="Conector" Visible="false" ></asp:Label>    
        <div class="componentFormMaxWidth">
                <dx:ASPxComboBox runat="server" CssClass="lblSelectTemplate" Paddings-Padding="0" Paddings-PaddingLeft="5" ID="lstAvailableCustomTemplates" Width="250px" Visible="false" TextField="Conector">
    <ClientSideEvents  GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
</dx:ASPxComboBox></div>
</tr>
            <tr>
                <td colspan="2" align="center">
                    <asp:Label ID="lblMsg" runat="server" CssClass="errorText" Visible="false"></asp:Label></td>
            </tr>
        </table>

        <asp:Button ID="btnSend" runat="server" Style="display: none;"
            Text="Enviar archivos" />
        <!-- Para poder utilizar el __doPostBack() de la function SendFiles() es necesario que haya un PostBack en la página.
                Es por eso que para no realizarlo en la lista de radiobuttons original se hace con esta de dummy sin valores-->
        <div style="display: none">
            <asp:RadioButtonList ID="rdListDummyPostBack" CssClass="rdListDummyPostBack" runat="server" AutoPostBack="true">
            </asp:RadioButtonList>
        </div>
    </form>
</body>
</html>