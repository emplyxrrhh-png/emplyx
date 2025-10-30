<%@ Page Language="VB" AutoEventWireup="false" Inherits="VTLive40.Access_AccessFilterPlates" CodeBehind="AccessFilterPlates.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html lang="en" class="notranslate" translate="no" xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Filtro de Accesos por Matrícula y Empleado</title>
    <style type="text/css" media="screen">
        .NoWhiteSpace {
            white-space: nowrap;
        }

        .divErrorStyle {
            border: thin solid #A3A3A3;
            font-size: 14px;
            margin-bottom: 10px;
            margin-left: auto;
            margin-right: auto;
            padding: 5px;
            text-align: center;
            vertical-align: middle;
            width: 80%;
        }
    </style>

    <script language="javascript" type="text/javascript">

        function CloseMe() {
            Close();
            return false;
        }

        //============ SELECTOR EMPLEADOS =================================
        function PopupSelectorEmployeesClient_PopUp(s, e) {
            try {
                s.SetHeaderText("");
                var iFrm = document.getElementById("PopupSelectorEmployees_ASPxPanel3_GroupSelectorFrame");
                var strBase = "../Base/WebUserControls/roWizardSelectorContainerMultiSelectV3.aspx?" +
                    "PrefixTree=treeEmpFilterPlates&FeatureAlias=Access&PrefixCookie=objContainerTreeV3_treeEmpFilterPlatesGrid&" +
                    "AfterSelectFuncion=parent.GetSelectedTreeV3";
                iFrm.src = strBase;
            }
            catch (e) {
                showError("PopupSelectorEmployeesClient_PopUp", e);
            }
        }
        //==================================================================

        //==========================================================================
        //Guarda los empleados seleccionados en el TreeV3
        function GetSelectedTreeV3(oParm1, oParm2, oParm3) {
            if (oParm1 == "") {
                document.getElementById('CallbackPanelPivot_hdnEmployees').value = "";
                document.getElementById('CallbackPanelPivot_hdnFilter').value = "";
                document.getElementById('CallbackPanelPivot_hdnFilterUser').value = "";
            }
            else {
                document.getElementById('CallbackPanelPivot_hdnEmployees').value = oParm1;
                document.getElementById('CallbackPanelPivot_hdnFilter').value = oParm2;
                document.getElementById('CallbackPanelPivot_hdnFilterUser').value = oParm3;
            }
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <input type="hidden" id="hdnAllEmployees" runat="server" value="" />
        <dx:ASPxCallback ID="CallbackSession" runat="server" ClientInstanceName="CallbackSessionClient" ClientSideEvents-CallbackComplete="CallbackSession_CallbackComplete"></dx:ASPxCallback>
        <div id="divMainBody" style="width: 100%; height: 100%; vertical-align: top; display: block;">
            <table cellpadding="0" cellspacing="0" border="0" style="width: 100%; height: 100%;">
                <tr>
                    <td style="padding-left: 5px; width: auto; height: 70px;" valign="top">
                        <div id="divTabInfo" class="divDataCells">
                            <div style="min-height: 10px"></div>
                            <div id="divAccessGroup" class="blackRibbonTitle">
                                <div class="blackRibbonIcon">
                                    <img src="Images/AccessPunches.png" height="50px" alt="" />
                                </div>
                                <div class="blackRibbonDescription" style="max-width: calc(100% - 100px);">
                                    <span class="NameText" id="sectionName" runat="server" clientidmode="Static"></span>
                                </div>
                            </div>
                            <div style="min-height: 10px"></div>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td style="width: 100%; height: 75%;" valign="top" align="left">
                        <div class="RoundCornerFrame roundCorner">
                            <dx:ASPxCallbackPanel ID="CallbackPanelPivot" runat="server" Width="100%" ClientInstanceName="CallbackPanelPivotClient">
                                <PanelCollection>
                                    <dx:PanelContent ID="PanelContent2" runat="server">

                                        <asp:HiddenField ID="hdnEmployeesSelected" runat="server" Value="0" />
                                        <asp:HiddenField ID="hdnEmployees" runat="server" Value="" />
                                        <asp:HiddenField ID="hdnFilter" runat="server" Value="" />
                                        <asp:HiddenField ID="hdnFilterUser" runat="server" Value="" />
                                        <div>
                                            <div style="float: left;">
                                                <table cellpadding="0" cellspacing="0" border="0">
                                                    <tr>
                                                        <td>
                                                            <table>
                                                                <tr>
                                                                    <td style="width: 60px;">
                                                                        <span>
                                                                            <asp:Label ID="lblPlates" runat="server" CssClass="NoWhiteSpace" Text="Matrículas" Style="display: inline;"></asp:Label></span>
                                                                    </td>
                                                                    <td>
                                                                        <span>
                                                                            <asp:Label ID="lblPlate1" runat="server" CssClass="NoWhiteSpace" Text="" Style="display: inline;"></asp:Label></span>
                                                                    </td>
                                                                    <td style="width: 90px;">
                                                                        <dx:ASPxTextBox ID="txtPlate1" runat="server" Font-Size="11px" Height="20px" Width="90px" ClientInstanceName="txtPlate1Client">
                                                                        </dx:ASPxTextBox>
                                                                    </td>
                                                                    <td>
                                                                        <asp:Label ID="lblPlate2" runat="server" CssClass="NoWhiteSpace" Text="" Style="display: inline;"></asp:Label>
                                                                    </td>
                                                                    <td style="width: 90px;">
                                                                        <dx:ASPxTextBox ID="txtPlate2" runat="server" Font-Size="11px" Height="20px" Width="90px" ClientInstanceName="txtPlate2Client">
                                                                        </dx:ASPxTextBox>
                                                                    </td>
                                                                    <td>
                                                                        <asp:Label ID="lblPlate3" runat="server" CssClass="NoWhiteSpace" Text="" Style="display: inline;"></asp:Label>
                                                                    </td>
                                                                    <td style="width: 90px;">
                                                                        <dx:ASPxTextBox ID="txtPlate3" runat="server" Font-Size="11px" Height="20px" Width="90px" ClientInstanceName="txtPlate3Client">
                                                                        </dx:ASPxTextBox>
                                                                    </td>
                                                                    <td>
                                                                        <asp:Label ID="lblPlate4" runat="server" CssClass="NoWhiteSpace" Text="" Style="display: inline;"></asp:Label>
                                                                    </td>
                                                                    <td>
                                                                        <dx:ASPxTextBox ID="txtPlate4" runat="server" Font-Size="11px" Height="20px" Width="90px" ClientInstanceName="txtPlate4Client">
                                                                        </dx:ASPxTextBox>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            <table>
                                                                <tr>
                                                                    <td style="width: 60px;">
                                                                        <asp:Label ID="lblEmployees" runat="server" Text="Empleados" Style="display: inline;"></asp:Label>
                                                                    </td>
                                                                    <td style="width: 255px;">
                                                                        <dx:ASPxTextBox ID="txtEmployees" ClientInstanceName="txtEmployeesClient" runat="server" ReadOnly="true" Height="20px" Font-Size="11px" CssClass="editTextFormat" Width="300px" Border-BorderStyle="Solid" Border-BorderColor="#A0A0A0" Border-BorderWidth="1" BackColor="#DBDBDB"></dx:ASPxTextBox>
                                                                    </td>
                                                                    <td style="width: 140px;">
                                                                        <dx:ASPxButton ID="btnOpenPopupSelectorEmployees" runat="server" AutoPostBack="False" CausesValidation="False" Text="Seleccionar" ToolTip="Empleados..." HoverStyle-CssClass="btnFlat-hover" CssClass="btnFlat">
                                                                            <Image Url="~/Access/Images/EmployeeSelector16.png"></Image>
                                                                            <ClientSideEvents Click="btnOpenPopupSelectorEmployeesClient_Click" />
                                                                        </dx:ASPxButton>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            <table>
                                                                <tr>
                                                                    <td style="width: 60px;">
                                                                        <asp:Label ID="lblFechaInf" runat="server" Text="Entre"></asp:Label>
                                                                    </td>
                                                                    <td style="width: 100px;">
                                                                        <dx:ASPxDateEdit ID="txtDateInf" runat="server" AllowNull="False" Width="100px" CssClass="editTextFormat" Font-Size="11px" ClientInstanceName="txtDateInfClient">
                                                                        </dx:ASPxDateEdit>
                                                                    </td>
                                                                    <td style="width: 15px;">
                                                                        <asp:Label ID="lblFechaSup" runat="server" Text="Y"></asp:Label>
                                                                    </td>
                                                                    <td style="width: 136px;">
                                                                        <dx:ASPxDateEdit ID="txtDateSup" runat="server" AllowNull="False" Width="100px" CssClass="editTextFormat" Font-Size="11px" ClientInstanceName="txtDateSupClient">
                                                                        </dx:ASPxDateEdit>
                                                                    </td>
                                                                    <td>
                                                                        <dx:ASPxButton ID="btnRefresh" runat="server" AutoPostBack="False" CausesValidation="False" Text="Obtener" ToolTip="Obtener Datos"
                                                                            ClientInstanceName="btnRefreshClient" HoverStyle-CssClass="btnFlat-hover" CssClass="btnFlat">
                                                                            <Image Url="~/Base/Images/Grid/button_reload.png"></Image>
                                                                            <ClientSideEvents Click="btnRefreshClient_Click" />
                                                                        </dx:ASPxButton>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </div>
                                            <div style="float: right;">
                                                <dx:ASPxButton ID="btnExportToXls" runat="server" CausesValidation="False" Text="Exportar" ToolTip="Exportar a Excel"
                                                    HoverStyle-CssClass="btnFlat-hover" CssClass="btnFlat">
                                                    <Image Url="~/Base/Images/Grid/ExportToExcel16.png"></Image>
                                                </dx:ASPxButton>
                                                <dx:ASPxButton ID="btnOpenPopupSaveView" runat="server" AutoPostBack="false" CausesValidation="False" Text="Guardar Sel." ToolTip="Guardar Selección del conjunto de datos"
                                                    HoverStyle-CssClass="btnFlat-hover" CssClass="btnFlat">
                                                    <Image Url="~/Base/Images/Grid/button_save.png"></Image>
                                                    <ClientSideEvents Click="btnOpenPopupSaveViewClient_Click" />
                                                </dx:ASPxButton>
                                                <dx:ASPxButton ID="btnOpenPopupViews" runat="server" AutoPostBack="false" CausesValidation="False" Text="Cargar Sel." ToolTip="Cargar Selección del conjunto de datos"
                                                    HoverStyle-CssClass="btnFlat-hover" CssClass="btnFlat">
                                                    <Image Url="~/Base/Images/Grid/button_reload.png"></Image>
                                                    <ClientSideEvents Click="btnOpenPopupViewsClient_Click" />
                                                </dx:ASPxButton>
                                            </div>
                                            <div style="clear: both;"></div>
                                        </div>

                                        <table cellpadding="0" cellspacing="0" style="width: 99%; height: 100%; margin-left: auto; margin-right: auto;" border="0">
                                            <tr>
                                                <td valign="top">
                                                    <div id="divFilterPlates" style="text-align: left; vertical-align: top; padding-top: 10px; height: 99%; display: block;">
                                                        <dx:ASPxGridView ID="GridFilterPlates" runat="server" AutoGenerateColumns="False" Width="100%" Cursor="pointer" ClientInstanceName="GridFilterPlatesClient">
                                                        </dx:ASPxGridView>
                                                    </div>
                                                </td>
                                            </tr>
                                        </table>
                                    </dx:PanelContent>
                                </PanelCollection>
                            </dx:ASPxCallbackPanel>
                        </div>
                    </td>
                </tr>
                <tr align="right">
                    <td style="height: 30px;">
                        <dx:ASPxButton ID="bntClose" runat="server" AutoPostBack="false" CausesValidation="False" Text="${Button.Close}" ToolTip="${Button.Close}"
                            HoverStyle-CssClass="btnFlat-hover btnFlatBlack-hover" CssClass="btnFlat btnFlatBlack">
                            <ClientSideEvents Click="CloseMe" />
                        </dx:ASPxButton>

                        <asp:HiddenField ID="hdnCanClose_PageBase" runat="server" />
                    </td>
                </tr>
            </table>
        </div>

        <!-- POPUP DEL SELECTOR DE EMPLEADOS -->
        <dx:ASPxPopupControl ID="PopupSelectorEmployees" runat="server" AllowDragging="True" CloseAction="None" Modal="True" ClientInstanceName="PopupSelectorEmployeesClient" ClientSideEvents-PopUp="PopupSelectorEmployeesClient_PopUp"
            PopupVerticalAlign="WindowCenter" PopupHorizontalAlign="WindowCenter" Height="500px" Width="800px"
            ShowHeader="False" ScrollBars="Auto" ShowPageScrollbarWhenModal="True" PopupAnimationType="None" BackColor="Transparent" ContentStyle-Paddings-Padding="0px" Border-BorderColor="Transparent" ShowShadow="false">
            <ContentCollection>
                <dx:PopupControlContentControl ID="PopupControlContentControl1" runat="server">
                    <dx:ASPxPanel ID="ASPxPanel3" runat="server" Width="0px" Height="0px">
                        <PanelCollection>
                            <dx:PanelContent ID="PanelContent3" runat="server">
                                <div class="bodyPopupExtended" style="table-layout: fixed; height: 460px; width: 775px;">
                                    <div class="panHeader2">
                                        <span style="">
                                            <asp:Label ID="Label1" runat="server" Text="Seleccionar empleados a mostrar" /></span>
                                    </div>
                                    <br />
                                    <table id="tbPopupFrame" cellpadding="0" cellspacing="0">
                                        <tr>
                                            <td valign="top">
                                                <iframe id="GroupSelectorFrame" runat="server" style="background-color: Transparent;" height="350" width="725" scrolling="no"
                                                    frameborder="0" marginheight="0" marginwidth="0" src="" />
                                            </td>
                                        </tr>
                                        <tr style="height: 35px;">
                                            <td align="right">
                                                <table>
                                                    <tr>
                                                        <td>
                                                            <dx:ASPxButton ID="btnPopupSelectorEmployeesAccept" runat="server" AutoPostBack="false" CausesValidation="False" Text="${Button.Close}" ToolTip="${Button.Close}"
                                                                HoverStyle-CssClass="btnFlat-hover btnFlatBlack-hover" CssClass="btnFlat btnFlatBlack">
                                                                <ClientSideEvents Click="btnPopupSelectorEmployeesAcceptClient_Click" />
                                                            </dx:ASPxButton>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                            </dx:PanelContent>
                        </PanelCollection>
                    </dx:ASPxPanel>
                </dx:PopupControlContentControl>
            </ContentCollection>
        </dx:ASPxPopupControl>

        <!-- POPUP DE SELECCION DE VISTA A CARGAR -->
        <dx:ASPxPopupControl ID="PopupViews" runat="server" AllowDragging="True" CloseAction="None" Modal="True" ClientInstanceName="PopupViewsClient" ClientSideEvents-PopUp="PopupViewsClient_PopUp"
            PopupVerticalAlign="WindowCenter" PopupHorizontalAlign="WindowCenter" Height="430px" Width="640px"
            ShowHeader="False" ScrollBars="Auto" ShowPageScrollbarWhenModal="True" PopupAnimationType="None" BackColor="Transparent" ContentStyle-Paddings-Padding="0px" Border-BorderColor="Transparent" ShowShadow="false">
            <ContentCollection>
                <dx:PopupControlContentControl ID="PopupViewsControlContent" runat="server">
                    <dx:ASPxPanel ID="ASPxPanel1" runat="server" Width="0px" Height="0px">
                        <PanelCollection>
                            <dx:PanelContent ID="PopupViewsPanelContent" runat="server">
                                <div class="bodyPopupExtended" style="table-layout: fixed; height: 390px; width: 600px;">
                                    <div class="panHeader2">
                                        <span style="">
                                            <asp:Label ID="lblSelec" runat="server" Text="Selecciones Guardadas" /></span>
                                    </div>
                                    <br />

                                    <table style="height: 300px; width: 100%">
                                        <tr valign="top">
                                            <td>
                                                <dx:ASPxGridView ID="GridViews" runat="server" Cursor="pointer" AutoGenerateColumns="False" ClientInstanceName="GridViewsClient"
                                                    KeyboardSupport="True" Width="100%">
                                                </dx:ASPxGridView>
                                            </td>
                                        </tr>
                                    </table>
                                    <!-- BOTONES -->
                                    <table style="margin-left: auto;">
                                        <tr>
                                            <td>
                                                <dx:ASPxButton ID="btnPopupViewsAccept" runat="server" AutoPostBack="false" CausesValidation="False" Text="${Button.Accept}" ToolTip="${Button.Accept}"
                                                    HoverStyle-CssClass="btnFlat-hover btnFlatBlack-hover" CssClass="btnFlat btnFlatBlack">
                                                    <ClientSideEvents Click="btnPopupViewsAcceptClient_Click" />
                                                </dx:ASPxButton>
                                            </td>
                                            <td>
                                                <dx:ASPxButton ID="btnPopupViewsCancel" runat="server" AutoPostBack="false" CausesValidation="False" Text="${Button.Cancel}" ToolTip="${Button.Cancel}"
                                                    HoverStyle-CssClass="btnFlat-hover btnFlatBlack-hover" CssClass="btnFlat btnFlatBlack">
                                                    <ClientSideEvents Click="function(s, e) { PopupViewsClient.Hide(); }" />
                                                </dx:ASPxButton>
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                            </dx:PanelContent>
                        </PanelCollection>
                    </dx:ASPxPanel>
                </dx:PopupControlContentControl>
            </ContentCollection>
        </dx:ASPxPopupControl>

        <!-- POPUP DE GUARDAR VISTA -->
        <dx:ASPxPopupControl ID="PopupSaveView" runat="server" AllowDragging="True" CloseAction="None" Modal="True" ClientInstanceName="PopupSaveViewClient"
            PopupVerticalAlign="WindowCenter" PopupHorizontalAlign="WindowCenter" Height="280px" Width="540px"
            ShowHeader="False" ScrollBars="Auto" ShowPageScrollbarWhenModal="True" PopupAnimationType="None" BackColor="Transparent" ContentStyle-Paddings-Padding="0px" Border-BorderColor="Transparent" ShowShadow="false">
            <ContentCollection>
                <dx:PopupControlContentControl ID="PopupSaveViewControlContent" runat="server">
                    <dx:ASPxPanel ID="ASPxPanel2" runat="server" Width="0px" Height="0px">
                        <PanelCollection>
                            <dx:PanelContent ID="PopupSaveViewPanelContent" runat="server">
                                <div class="bodyPopupExtended" style="table-layout: fixed; height: 240px; width: 500px;">
                                    <div class="panHeader2">
                                        <span style="">
                                            <asp:Label ID="lblActionsTitle" runat="server" Text="Guardar estado del conjunto de datos" /></span>
                                    </div>
                                    <br />
                                    <table border="0" cellpadding="0" cellspacing="0" style="width: 100%; padding-left: 20px;">
                                        <tr style="height: 50px;">
                                            <td valign="top">
                                                <asp:Label ID="lblPopupSaveViewName" runat="server" Text="Nombre:" CssClass="editTextFormat"></asp:Label>
                                            </td>
                                            <td valign="top">
                                                <dx:ASPxTextBox ID="txtPopupSaveViewName" runat="server" Width="200px" CssClass="editTextFormat" ClientInstanceName="txtPopupSaveViewName_Client"></dx:ASPxTextBox>
                                            </td>
                                        </tr>
                                        <tr style="height: 70px;">
                                            <td valign="top">
                                                <asp:Label ID="lblPopupSaveViewDescription" runat="server" Text="Descripción" CssClass="editTextFormat"></asp:Label>
                                            </td>
                                            <td valign="top">
                                                <dx:ASPxMemo ID="txtPopupSaveViewDescription" runat="server" Height="50px" Width="300px" CssClass="editTextFormat" ClientInstanceName="txtPopupSaveViewDescription_Client"></dx:ASPxMemo>
                                            </td>
                                        </tr>
                                    </table>
                                    <div id="divError" style="display: none;" class="divErrorStyle RoundCorner">
                                        <asp:Label ID="lblError" runat="server" Text="Debe indicar el nombre de la vista" CssClass="errorText"></asp:Label>
                                    </div>

                                    <!-- BOTONES -->
                                    <table style="margin-left: auto;">
                                        <tr>
                                            <td>
                                                <dx:ASPxButton ID="btnPopupSaveViewSave" runat="server" AutoPostBack="false" CausesValidation="False" Text="${Button.Accept}" ToolTip="${Button.Accept}"
                                                    HoverStyle-CssClass="btnFlat-hover btnFlatBlack-hover" CssClass="btnFlat btnFlatBlack">
                                                    <ClientSideEvents Click="btnPopupSaveViewSaveClient_Click" />
                                                </dx:ASPxButton>
                                            </td>
                                            <td>
                                                <dx:ASPxButton ID="PopupSaveViewCancel" runat="server" AutoPostBack="false" CausesValidation="False" Text="${Button.Cancel}" ToolTip="${Button.Cancel}"
                                                    HoverStyle-CssClass="btnFlat-hover btnFlatBlack-hover" CssClass="btnFlat btnFlatBlack">
                                                    <ClientSideEvents Click="function(s, e) { PopupSaveViewClient.Hide(); }" />
                                                </dx:ASPxButton>
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                            </dx:PanelContent>
                        </PanelCollection>
                    </dx:ASPxPanel>
                </dx:PopupControlContentControl>
            </ContentCollection>
        </dx:ASPxPopupControl>

        <dx:ASPxGridViewExporter ID="ASPxGridViewExporter1" runat="server"></dx:ASPxGridViewExporter>
    </form>
</body>
</html>