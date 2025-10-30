<%@ Page Language="VB" AutoEventWireup="false" Inherits="VTLive40.PlanesWizard" EnableEventValidation="false" Culture="auto" UICulture="auto" CodeBehind="PlanesWizard.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html lang="en" class="notranslate" translate="no" xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Planos de zona</title>
</head>
<body class="bodyPopup" style="background-attachment: fixed;">

    <form id="frmPlanesWizard" runat="server">

        <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="True" EnableScriptLocalization="True" />

        <script language="javascript" type="text/javascript">

            function PageBase_Load() {
                // Establecer selección actual en la grid 'grdEntries'
                var oGrd = $get('grdPlanes');
                var currow = $get('<%= hdnGrdPlanesSelectedRowIndex.ClientID %>').value;
                var curcol = $get('<%= hdnGrdPlanesSelectedColIndex.ClientID %>').value;
                if (currow != -1 && curcol != -1) {
                    roGridViewControl_Load(oGrd, currow, curcol, 'row-select');
                }

                ConvertControls('<%= divPlanes.ClientID %>');
            }

            function KeyPressFunction(e) {
                tecla = (document.all) ? e.keyCode : e.which;
                if (tecla == 13) {
                    RunAccept();
                    //var button = $get('LoginObject_Login_btAccept_btButton');
                    //ButtonClick(button);
                    return false;
                }
            }

            function RunAccept() {
                //TODO:..
            }

            function showImage(ID) {
                var oImg = document.getElementById('planeImg');
                if (oImg != null) {
                    oImg.setAttribute("planeid", ID);
                    oImg.src = "LoadPlaneImg.aspx?ID=" + ID + "&stamp=" + new Date().getTime();
                }
            }

            function retIDPlane() {
                var oImg = document.getElementById('planeImg');
                var oPlaneID = null;
                if (oImg != null) {
                    oPlaneID = oImg.getAttribute('planeid');
                }
                return oPlaneID;
            }
        </script>

        <div>

            <table style="width: 100%; height: 100%" border="0">
                <tr>
                    <td colspan="2" style="padding-top: 5px; padding-bottom: 0px;" valign="top" height="20px">
                        <div class="panHeader2">
                            <span style="">
                                <asp:Label ID="LblZonePlaneTitle" runat="server" Text="Planos de zona"></asp:Label></span>
                        </div>
                    </td>
                </tr>
                <tr style="height: 48px">
                    <td style="width: 48px; padding: 4px; padding-bottom: 10px;">
                        <img src="../Images/ZonePlanes.png" />
                    </td>
                    <td align="left" valign="middle" style="padding: 4px; padding-bottom: 10px;">
                        <asp:Label ID="lblZonePlaneDescription" runat="server" CssClass="editTextFormat" Text="Cree los planos de zona para enlazar con su zona de acceso."></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td colspan="2" align="right">
                        <div class="btnFlat">
                            <a href="javascript: void(0)" id="btAddPlane" runat="server">
                                <asp:Label ID="lblAddPlane" Text="Añadir" runat="server" />
                            </a>
                        </div>
                    </td>
                </tr>
                <tr style="height: 200px; vertical-align: top">
                    <td colspan="2">

                        <asp:UpdatePanel ID="updPlanes" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="btAddPlane" EventName="ServerClick" />
                            </Triggers>
                            <ContentTemplate>
                                <table border="0" style="width: 100%; height: 380px;">
                                    <tr>
                                        <td style="width: 340px;" valign="top" align="left">
                                            <div id="divPlanes" runat="server" class="planesModal defaultContrastColor">

                                                <asp:ObjectDataSource ID="BlankData" runat="server" SelectMethod="_Select" TypeName="BlankDataObject" />
                                                <roWebControls:roGridViewControl ID="grdPlanes" runat="server" CellPadding="4" ForeColor="#333333" GridLines="None"
                                                    Width="100%" ScrollWidth="97%" Height="380px" Scrolling="Auto" DataSourceID="BlankData" AutoGenerateColumns="false"
                                                    CssClass="yui-datatable-small-theme" DataKeyNames="ID, Name"
                                                    CellHoverCssClass="row-over" CellSelectCssClass="row-select"
                                                    RowSelectedControlID='<%# hdnGrdPlanesSelectedRowIndex.ClientID%>'
                                                    ColSelectedControlID='<%# hdnGrdPlanesSelectedColIndex.ClientID%>'>
                                                    <RowStyle CssClass="data-row" />
                                                    <AlternatingRowStyle CssClass="alt-data-row" />
                                                    <Columns>
                                                        <asp:ButtonField Text="" CommandName="EditClick" Visible="False" />
                                                        <asp:ButtonField Text="" CommandName="RemoveClick" Visible="False" />
                                                        <asp:TemplateField HeaderText="ID" Visible="false" ShowHeader="false">
                                                            <ItemTemplate>
                                                                <asp:Label ID="ID_Label" runat="server"></asp:Label>
                                                                <input type="text" id="ID_TextBox" runat="server" convertcontrol="TextField" ccallowblank="false" class="textClass" style="color: #333333;" visible="false" onkeypress="return KeyPressFunction(event);" />
                                                            </ItemTemplate>
                                                            <ItemStyle HorizontalAlign="Left" Width="0" />
                                                            <HeaderStyle HorizontalAlign="Left" VerticalAlign="Middle" Width="0" />
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Nombre">
                                                            <ItemTemplate>
                                                                <asp:Label ID="Name_Label" runat="server" Style="width: 100%; cursor: pointer; display: block; height: 100%;"></asp:Label>
                                                                <input type="text" id="Name_TextBox" runat="server" convertcontrol="TextField" ccallowblank="false" class="textClass" style="width: 95%; color: #333333;" visible="false" onkeypress="return KeyPressFunction(event);" />
                                                            </ItemTemplate>
                                                            <ItemStyle HorizontalAlign="Left" />
                                                            <HeaderStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                        </asp:TemplateField>
                                                        <asp:TemplateField>
                                                            <ItemTemplate>
                                                                <div class="fix-data-row-div">
                                                                    <div class="fix-data-row-div-left">
                                                                        <img id="imgEdit" src="~/Base/Images/Grid/edit.png" visible="true" runat="server" title="Editar" />
                                                                        <img id="imgEditAccept" src="~/Base/Images/Grid/save.png" visible="false" runat="server" title="Aplicar" />
                                                                    </div>
                                                                    <div class="fix-data-row-div-right">
                                                                        <img id="imgRemove" src="~/Base/Images/Grid/remove.png" visible="true" runat="server" title="Eliminar" />
                                                                        <img id="imgEditCancel" src="~/Base/Images/Grid/cancel.png" visible="false" runat="server" title="Cancelar" />
                                                                    </div>
                                                                </div>
                                                            </ItemTemplate>
                                                            <ItemStyle Width="40px" HorizontalAlign="Center" VerticalAlign="Middle" CssClass="fix-data-row" />
                                                            <HeaderStyle CssClass="fix-header" />
                                                        </asp:TemplateField>
                                                    </Columns>
                                                </roWebControls:roGridViewControl>
                                                <asp:HiddenField ID="hdnGrdPlanesSelectedRowIndex" runat="server" Value="-1" />
                                                <asp:HiddenField ID="hdnGrdPlanesSelectedColIndex" runat="server" Value="-1" />
                                            </div>
                                        </td>
                                        <td style="width: 400px; height: 380px;">
                                            <div style="display: block; width: 100%; height: 100%;">
                                                <table border="0" style="width: 100%; height: 100%;">
                                                    <tr>
                                                        <td valign="top" style="height: 65px;">
                                                            <iframe id="ifUploads" name="ifUploads" runat="server" style="background-color: Transparent" height="65" width="100%" scrolling="no" frameborder="0" marginheight="0" marginwidth="0" src="ImportsZonePlaneUpload.aspx" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td valign="middle" align="center">
                                                            <img id="planeImg" src="" alt="<%= Language.Translate("NoImage",DefaultScope) %>" planeid="" />
                                                        </td>
                                                    </tr>
                                                </table>
                                            </div>
                                        </td>
                                    </tr>
                                </table>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                </tr>
                <tr>

                    <td align="right" colspan="2" style="height: 20px; padding-right: 5px;">
                        <asp:UpdatePanel ID="updActions" runat="server">
                            <ContentTemplate>
                                <table>
                                    <tr>
                                        <td>
                                            <asp:Button ID="btClose" Text="${Button.Close}" runat="server" CssClass="btnFlat btnFlatBlack btnFlatAsp" />
                                        </td>
                                    </tr>
                                </table>
                                <asp:HiddenField ID="hdnCanClose_PageBase" runat="server" />
                                <asp:HiddenField ID="hdnMustRefresh_PageBase" Value="0" runat="server" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                </tr>
            </table>

            <Local:MessageFrame ID="MessageFrame1" runat="server" />
        </div>
    </form>
</body>
</html>