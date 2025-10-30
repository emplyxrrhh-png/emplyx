<%@ Page Language="VB" AutoEventWireup="false" Inherits="VTLive40.UserField" CodeBehind="UserField.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html lang="en" class="notranslate" translate="no" xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Campo personalizado</title>
</head>
<body class="bodyPopup" style="background-attachment: fixed;">

    <form id="frmUserField" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="True" EnableScriptLocalization="True" />

        <div>

            <script language="javascript" type="text/javascript">

                function PageBase_Load() {

                    ConvertControls();

                    venableOPC('<%= Me.optRequestAll.ClientID %>');
                venableOPC('<%= Me.optRequestNobody.ClientID %>');
                //venableOPC('<%= Me.optRequestCriteria.ClientID %>');
                //linkOPCItems('<%= Me.optRequestAll.ClientID %>,<%= Me.optRequestNobody.ClientID %>,<%= Me.optRequestCriteria.ClientID %>');
                linkOPCItems('<%= Me.optRequestAll.ClientID %>,<%= Me.optRequestNobody.ClientID %>');

                }

                function endRequestHandler() {
                    ConvertControls();
                }

                function ConvertClick(s) {
                    var a = 1;
                }
                function VisibleListValuesControl(visible) {

                    var lbl = $get('<%= lblListValues.ClientID %>');
                var cmb = $get('<%= cmbListValues.ClientID %>');
                var div = $get('<%= divListActions.ClientID %>');

                    var strDisplay = '';
                    if (!visible) {
                        strDisplay = 'none';
                    }

                    lbl.style.display = strDisplay;
                    cmb.style.display = strDisplay;
                    div.style.display = strDisplay;

                }

                function VisibleAccessValidationControl(visible) {

                    var lbl = $get('<%= lblAccessValidation.ClientID %>');
                var cmb = $get('<%= cmbAccessValidation.ClientID %>');

                    var strDisplay = '';
                    if (!visible) {
                        strDisplay = 'none';
                    }

                    lbl.style.display = strDisplay;
                    cmb.style.display = strDisplay;

                }

                function ShowAddCategory(button) {

                    var ButtonBounds = Sys.UI.DomElement.getBounds(button);

                    var popup = $find('mpxNewCategoryBehavior');
                    popup._xCoordinate = ButtonBounds.x - 300;
                    popup._yCoordinate = ButtonBounds.y;

                    showPopup('mpxNewCategoryBehavior');

                }
                function AddCategoryOK() {
                    ButtonClick($get('<%= btNewCategoryOK.ClientID %>'));
                }
                function RemoveCategory() {

                }

                function ShowAddListValue(button) {

                    var ButtonBounds = Sys.UI.DomElement.getBounds(button);

                    var popup = $find('mpxNewListValueBehavior');
                    popup._xCoordinate = ButtonBounds.x - 300;
                    popup._yCoordinate = ButtonBounds.y;

                    showPopup('mpxNewListValueBehavior');

                }
                function AddListValueOK() {
                    ButtonClick($get('<%= btNewListValueOK.ClientID %>'));
                }
                function RemoveListValue() {
                    ButtonClick($get('<%= btDeleteListValue.ClientID %>'));
                }

                function CheckSave() {
                    if (CheckConvertControls('') == false) {
                        var message = "TitleKey=SaveUserField.Check.Invalid.Text&" +
                            "DescriptionKey=SaveUserField.Check.Invalid.Description&" +
                            "Option1TextKey=SaveUserField.Check.Invalid.Option1Text&" +
                            "Option1DescriptionKey=SaveUserField.Check.Invalid.Option1Description&" +
                            "Option1OnClickScript=HideMsgBoxForm(); return false;&" +
                                  'IconUrl=<%= Robotics.Web.Base.HelperWeb.MsgBoxIconsUrl(Robotics.Web.Base.HelperWeb.MsgBoxIcons.ErrorIcon) %>'
                        var url = "Options/srvMsgBoxOptions.aspx?action=MessageEx&Parameters=" + encodeURIComponent(message);
                        parent.ShowMsgBoxForm(url, 500, 300, '');
                        return false;
                    }
                    else {
                        return true;
                    }
                }

                function RequestCriteriaOK() {
                    ButtonClick($get('<%= btRequestCriteriaOK.ClientID %>'));
                }

                function ShowRequestCriteria() {

                    /*var ButtonBounds = Sys.UI.DomElement.getBounds(button);
                    var popup = $find('mpxRequestCriteriaBehavior');
                    popup._xCoordinate = ButtonBounds.x - 300;
                    popup._yCoordinate = ButtonBounds.y;*/

                    showPopup('mpxRequestCriteriaBehavior');

                }
            </script>

            <table style="width: 100%; height: 100%;" border="0">
                <tr>
                    <td colspan="2" style="padding-top: 5px; padding-bottom: 0px;" valign="top" height="20px">
                        <div class="panHeader2">
                            <span style="">
                                <asp:Label ID="LblTitle" runat="server" Text="Campo personalizado"></asp:Label></span>
                        </div>
                    </td>
                </tr>
                <tr style="height: 48px">
                    <td style="padding: 4px; padding-bottom: 10px;">
                        <img src="Images/UserFields 32.gif" />
                    </td>
                    <td align="left" valign="middle" style="padding: 4px; padding-bottom: 10px;">
                        <asp:Label ID="lblDescription" runat="server" CssClass="editTextFormat" Text="Puede definir un campo personalizado para la ficha del ${Employee}."></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td colspan="2" align="center">
                        <asp:UpdatePanel ID="updError" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                            <ContentTemplate>
                                <asp:Label ID="lblError" Visible="false" CssClass="errorText" runat="server" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                </tr>
                <tr style="vertical-align: top; height: 290px;">
                    <td colspan="2">

                        <asp:UpdatePanel ID="updUserField" runat="server">
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="btNewCategoryOK" EventName="Click" />
                                <asp:AsyncPostBackTrigger ControlID="btNewListValueOK" EventName="Click" />
                                <asp:AsyncPostBackTrigger ControlID="btDeleteListValue" EventName="Click" />
                            </Triggers>
                            <ContentTemplate>
                                <roUserControls:roTabContainerClient ID="tabCtl01" runat="server">
                                    <TabTitle1>
                                        <asp:Label ID="lblGeneralTitle" runat="server" Text="General"></asp:Label>
                                    </TabTitle1>
                                    <TabContainer1>
                                        <br />
                                        <table cellpadding="1" cellspacing="1" width="100%" style="padding-top: 0px; height: 100%;" border="0">
                                            <tr style="padding-top: 10px;">
                                                <td align="right" style="height: 10px;">
                                                    <asp:Label ID="lblName" Text="Nombre" runat="server" />
                                                </td>
                                                <td style="padding-left: 5px; height: 10px;" colspan="1">
                                                    <input type="text" id="txtName" value="" style="width: 400px;" runat="server" convertcontrol="TextField" ccallowblank="false" ccmaxlength="50" class="textClass" />
                                                </td>
                                                <td align="center">
                                                    <img src="~/Base/Images/MessageFrame/Alert16.png" id="imgUsed" runat="server" style="padding: 3px;" title="Este campo de la ficha se utiliza en un proceso de recálculo." />
                                                </td>
                                            </tr>
                                            <tr id="descriptionTR" runat="server" style="padding-top: 5px;">
                                                <td align="right" style="height: 30px; vertical-align: text-top;">
                                                    <asp:Label ID="lblDescriptionField" Text="Descripción" runat="server" />
                                                </td>
                                                <td style="padding-left: 5px; height: 30px;" colspan="2">
                                                    <textarea id="txtDescription" style="width: 400px;" runat="server" convertcontrol="TextArea" ccallowblank="true" class="textClass"></textarea>
                                                </td>
                                            </tr>
                                            <tr style="padding-top: 5px">
                                                <td align="right" style="height: 10px;">
                                                    <asp:Label ID="lblType" Text="Tipo" runat="server" />
                                                </td>
                                                <td style="padding-left: 5px; height: 10px;" colspan="2">
                                                    <roWebControls:roComboBox ID="cmbTypes" runat="server" ItemsRunAtServer="false" ParentWidth="373px" ChildsWidth="50px" EnableViewState="true" HiddenText="cmbTypes_Text" ChildsVisible="8" AutoResizeChildsWidth="true" HiddenValue="cmbTypes_Value" />
                                                    <asp:HiddenField ID="cmbTypes_Text" runat="server" />
                                                    <asp:HiddenField ID="cmbTypes_Value" runat="server" />
                                                    <%--
                                    <asp:DropDownList ID="ddlTypes" AutoPostBack ="false" Width="200" runat="server" CssClass="textClass" />
                                                    --%>
                                                </td>
                                            </tr>
                                            <tr style="padding-top: 5px">
                                                <td align="right" style="height: 10px;">
                                                    <asp:Label ID="lblAccessLevel" Text="Nivel de Acceso" runat="server" />
                                                </td>
                                                <td style="padding-left: 5px; height: 10px;" colspan="2">
                                                    <roWebControls:roComboBox ID="cmbAccessLevels" runat="server" ItemsRunAtServer="false" ParentWidth="373px" EnableViewState="true" HiddenText="cmbAccessLevels_Text" ChildsVisible="4" AutoResizeChildsWidth="True" HiddenValue="cmbAccessLevels_Value" />
                                                    <asp:HiddenField ID="cmbAccessLevels_Text" runat="server" />
                                                    <asp:HiddenField ID="cmbAccessLevels_Value" runat="server" />
                                                    <%--
                                    <asp:DropDownList ID="ddlAccessLevels" AutoPostBack ="false" Width="200" runat="server" CssClass="textClass" />
                                                    --%>
                                                </td>
                                            </tr>
                                            <tr id="historyTR" runat="server" style="padding-top: 5px">
                                                <td align="right" style="height: 10px;"></td>
                                                <td style="padding-left: 5px; height: 10px;" colspan="2" valign="middle">
                                                    <table>
                                                        <tr>
                                                            <td style="height: 15px;">
                                                                <input id="chkHistory" type="checkbox" runat="server" /></td>
                                                            <td><a href="javascript: void(0);" onclick="CheckLinkClick('<%= chkHistory.ClientID %>');">
                                                                <asp:Label ID="lblHistory" runat="server" Text="Control histórico"></asp:Label>
                                                            </a>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                            <tr id="categoriesTR" runat="server" style="padding-top: 5px">
                                                <td align="right" style="height: 10px;">
                                                    <asp:Label ID="lblCategory" Text="Categoría" runat="server" />
                                                </td>
                                                <td style="padding-left: 5px; height: 10px;">
                                                    <roWebControls:roComboBox ID="cmbCategories" runat="server" ItemsRunAtServer="false" ParentWidth="373px" EnableViewState="true" HiddenText="cmbCategories_Text" ChildsVisible="4" AutoResizeChildsWidth="True" HiddenValue="cmbCategories_Value" />
                                                    <asp:HiddenField ID="cmbCategories_Text" runat="server" />
                                                    <asp:HiddenField ID="cmbCategories_Value" runat="server" />
                                                </td>
                                                <td valign="middle" align="left" style="padding-left: 3px;">
                                                    <div id="divAddCategory" class="taskField" runat="server">
                                                        <div style="padding-top: 5px;">
                                                            <img id="imgAddCategory" src="~/Base/Images/Grid/add.png" visible="true" runat="server" title='<%# Me.Language.Translate("addCategory",Me.DefaultScope) %>' style="cursor: pointer;"
                                                                onclick="ShowAddCategory(this); " />
                                                        </div>
                                                    </div>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="right" valign="top">
                                                    <asp:Label ID="lblListValues" Text="Valores lista" runat="server"></asp:Label>
                                                </td>
                                                <td style="padding-left: 5px;" valign="top">
                                                    <asp:ListBox ID="cmbListValues" runat="server" Width="204px" Height="100%"
                                                        class="textClass x-form-text x-form-field" onblur="this.className='textClass x-form-text x-form-field';" onfocus="this.className='textClass x-form-text x-form-field x-form-focus';" />
                                                </td>
                                                <td valign="top" align="left" style="padding-left: 3px;">
                                                    <div id="divListActions" class="taskField" runat="server">
                                                        <div style="padding-top: 5px;">
                                                            <img id="imgAddListValue" src="~/Base/Images/Grid/add.png" visible="true" runat="server" title='<%# Me.Language.Translate("addListValue",Me.DefaultScope) %>' style="cursor: pointer;" onclick="ShowAddListValue(this); " />
                                                            <img id="imgRemoveListValue" src="~/Base/Images/Grid/remove.png" visible="true" runat="server" title='<%# Me.Language.Translate("delListValue",Me.DefaultScope) %>' style="cursor: pointer;" onclick="RemoveListValue();" />
                                                        </div>
                                                    </div>
                                                </td>
                                            </tr>
                                            <tr style="padding-top: 5px">
                                                <td align="right" style="height: 10px;">
                                                    <asp:Label ID="lblAccessValidation" Text="Validación acceso" runat="server" />
                                                </td>
                                                <td style="padding-left: 5px; height: 10px;" colspan="2">
                                                    <roWebControls:roComboBox ID="cmbAccessValidation" runat="server" ItemsRunAtServer="false" ParentWidth="170px" EnableViewState="true" HiddenText="cmbAccessValidation_Text" ChildsVisible="4" AutoResizeChildsWidth="True" HiddenValue="cmbAccessValidation_Value" />
                                                    <asp:HiddenField ID="cmbAccessValidation_Text" runat="server" />
                                                    <asp:HiddenField ID="cmbAccessValidation_Value" runat="server" />
                                                </td>
                                            </tr>
                                            <tr id="uniqueTR" runat="server" style="padding-top: 5px">
                                                <td align="right" style="height: 10px;"></td>
                                                <td style="padding-left: 5px; height: 10px;" colspan="2" valign="middle">
                                                    <table>
                                                        <tr>
                                                            <td style="height: 15px;">
                                                                <input id="chkUnique" type="checkbox" runat="server" /></td>
                                                            <td>
                                                                <a href="javascript: void(0);" onclick="CheckLinkClick('<%= chkUnique.ClientID %>');">
                                                                    <asp:Label ID="lblUnique" runat="server" Text="Único"></asp:Label>
                                                                </a>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                        </table>
                                    </TabContainer1>
                                    <TabTitle2>
                                        <asp:Label ID="lblRequestsTitle" runat="server" Text="Visibilidad"></asp:Label>
                                    </TabTitle2>
                                    <TabContainer2>
                                        <br />
                                        <table style="width: 100%;">
                                            <tr>
                                                <td colspan="2" style="padding-top: 5px; padding-left: 10px;">
                                                    <asp:Label ID="lblRequestsInfo" runat="server" Text="Quién puede solicitar un cambio de valor de este campo" Font-Bold="false"></asp:Label></td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <roUserControls:roOptionPanelClient ID="optRequestAll" runat="server" TypeOPanel="RadioOption" Width="100%" Height="Auto" Enabled="True" Border="True">
                                                        <Title>
                                                            <asp:Label ID="lblRequestAll" runat="server" Text="Todos"></asp:Label>
                                                        </Title>
                                                        <Description>
                                                            <asp:Label ID="lblRequestAllDesc" runat="server" Text="Todos los ${Employees} podrán solicitar un cambio de valor de este campo."></asp:Label>
                                                        </Description>
                                                        <Content>
                                                        </Content>
                                                    </roUserControls:roOptionPanelClient>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>&nbsp;
                                    <roUserControls:roOptionPanelClient ID="optRequestNobody" runat="server" TypeOPanel="RadioOption" Width="100%" Height="Auto" Enabled="True" Border="True">
                                        <Title>
                                            <asp:Label ID="lblRequestNobody" runat="server" Text="Nadie"></asp:Label>
                                        </Title>
                                        <Description>
                                            <asp:Label ID="lblRequestNobodyDesc" runat="server" Text="Ningún empleado podrá solicitar un cambio de valor de este campo."></asp:Label>
                                        </Description>
                                        <Content>
                                        </Content>
                                    </roUserControls:roOptionPanelClient>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="height: 9px;">
                                                    <!--
                                    <roUserControls:roOptionPanelClient ID="optRequestCriteria" runat="server" TypeOPanel="RadioOption" Width="100%" Height="Auto" Enabled="false" Border="True">
                                        <Title>
                                            <asp:Label ID="lblRequestCriteria" runat="server" Text="Según criterio"></asp:Label>
                                        </Title>
                                        <Description>
                                            <asp:Label ID="lblRequestCriteriaDesc" runat="server" Text="Los ${Employees} que cumplan los siguientes criterios podrán solicitar cambios de valor de este campo."></asp:Label>
                                        </Description>
                                        <Content>
                                            <table style="width: 100%;">
                                                <tr><td align="right" style="padding-right: 10px;">
                                                    <a href="javascript: void(0);" style="text-decoration: underline;" onclick="ShowRequestCriteria();"><asp:label ID="lblRequestCriteriaConfiguration" runat="server" CssClass="spanEmp-Class" Text="Configuración"></asp:label></a>
                                                </td></tr>
                                            </table>
                                        </Content>
                                    </roUserControls:roOptionPanelClient>
                                    -->
                                                </td>
                                            </tr>
                                        </table>
                                    </TabContainer2>

                                    <TabTitle3>
                                        <asp:Label ID="lblAdvancedTitle" runat="server" Text="Avanzado"></asp:Label>
                                    </TabTitle3>
                                    <TabContainer3>
                                        <br />
                                        <div style="margin-left: 30px; margin-right: 30px;">
                                            <asp:Label ID="ConvertDesc" runat="server" Text="Desde aquí puedes reaprovechar este campo personalizado para utilizarlo como campo de sistema y así sacarle todo el partido desde Genius People Analytics."></asp:Label>
                                        </div>
                                        <br />
                                        <div style="margin-left: 50px;">
                                            <table>
                                                <tr>
                                                    <%--<td style="height: 15px;">
                                        <input id="chkConvert" type="checkbox" runat="server" /></td>--%>
                                                    <td>
                                                        <%-- <a href="javascript: void(0);" onclick="ConvertClick('<%= chkConvert.ClientID %>');">--%>
                                                        <asp:Label ID="lblConvert" runat="server" Text="Convertir a campo de sistema de tipo"></asp:Label>
                                                        </a>
                                                    </td>
                                                    <td style="padding-left: 5px; height: 10px;">
                                                        <roWebControls:roComboBox ID="cbConvertType" runat="server" ItemsRunAtServer="false" ParentWidth="150px" EnableViewState="true" AutoResizeChildsWidth="True" HiddenText="cbConvert_Text" HiddenValue="cbConvert_Value" />
                                                        <asp:HiddenField ID="cbConvert_Text" runat="server" />
                                                        <asp:HiddenField ID="cbConvert_Value" runat="server" />
                                                    </td>
                                                </tr>
                                            </table>
                                        </div>
                                    </TabContainer3>
                                </roUserControls:roTabContainerClient>

                                <roUserControls:roOptPanelClientGroup ID="optRequestGroup" runat="server" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                </tr>
                <tr>
                    <td align="right" colspan="2" style="height: auto;">
                        <asp:UpdatePanel ID="updActions" runat="server">
                            <ContentTemplate>
                                <table>
                                    <tr align="right">
                                        <td>
                                            <asp:Button ID="btAccept" Text="${Button.Accept}" runat="server" OnClientClick="return CheckSave();" CssClass="btnFlat btnFlatBlack btnFlatAsp" />
                                        </td>
                                        <td>
                                            <asp:Button ID="btCancel" Text="${Button.Cancel}" runat="server" OnClientClick="Close(); return false;" CssClass="btnFlat btnFlatBlack btnFlatAsp" />
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

            <asp:Button ID="btNewCategoryOK" Style="display: none;" runat="server" />

            <asp:UpdatePanel ID="updNewCategory" runat="server">
                <ContentTemplate>
                    <ajaxToolkit:ModalPopupExtender ID="mpxNewCategory" runat="server" BehaviorID="mpxNewCategoryBehavior"
                        TargetControlID="hiddenTargetControlForNewCategoryPopup"
                        PopupControlID="divNewCategory"
                        DropShadow="False"
                        PopupDragHandleControlID="panNewCategoryDragHandle"
                        X="400" Y="300" EnableViewState="true">
                    </ajaxToolkit:ModalPopupExtender>
                    <asp:Button runat="server" ID="hiddenTargetControlForNewCategoryPopup" Style="display: none" />
                    <div id="divNewCategory" runat="server" style="display: none">
                        <roWebControls:roPopupFrameV2 ID="ropfNewCategory" runat="server" BorderStyle="None" Height="75px" Width="300px">
                            <FrameContentTemplate>
                                <div style="width: 100%;" class="bodyPopup">
                                    <table width="100%" cellspacing="0">

                                        <tr id="panNewCategoryDragHandle" style="cursor: move; height: 20px;" class="defaultBackgroundColor">
                                            <td align="center">
                                                <%-- <asp:Image ID="imgNewCategory" runat="server" ImageUrl="~/Images/PRIMARY_KEY_16.GIF" Width="16px" Height="16px" /> --%>
                                            </td>
                                            <td>&nbsp;
                                            <asp:Label ID="lblNewCategoryTitle" Text="Nueva categoría" ForeColor="white" runat="server" />
                                            </td>
                                            <td align="right">
                                                <asp:ImageButton ID="ibtNewCategoryOK" runat="server" ImageUrl="~/Base/Images/ButtonOK_16.gif" OnClientClick="hidePopup('mpxNewCategoryBehavior'); AddCategoryOK(); return false;" Style="cursor: pointer;" />
                                                <img id="ibtNewCategoryCancel" onclick="hidePopup('mpxNewCategoryBehavior');" style="cursor: pointer;" src="~/Base/Images/ButtonCancel_16.gif" runat="server" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="3" style="padding-left: 10px; padding-top: 5px; vertical-align: middle">
                                                <asp:Label ID="lblCategoryTitle2" runat="server" Text="Introuzca el nombre de la nueva categoría" />
                                                <br />
                                                <asp:TextBox ID="txtNewCategory" Width="100%" runat="server"></asp:TextBox>
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                            </FrameContentTemplate>
                        </roWebControls:roPopupFrameV2>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>

            <asp:Button ID="btNewListValueOK" Style="display: none;" runat="server" />
            <asp:Button ID="btDeleteListValue" Style="display: none;" runat="server" />

            <asp:UpdatePanel ID="updNewListValue" runat="server">
                <ContentTemplate>
                    <ajaxToolkit:ModalPopupExtender ID="mpxNewListValue" runat="server" BehaviorID="mpxNewListValueBehavior"
                        TargetControlID="hiddenTargetControlForNewListValuePopup"
                        PopupControlID="divNewListValue"
                        DropShadow="False"
                        PopupDragHandleControlID="panNewListValueDragHandle"
                        X="400" Y="300" EnableViewState="true">
                    </ajaxToolkit:ModalPopupExtender>
                    <asp:Button runat="server" ID="hiddenTargetControlForNewListValuePopup" Style="display: none" />
                    <div id="divNewListValue" runat="server" style="display: none">
                        <roWebControls:roPopupFrameV2 ID="ropfNewListValue" runat="server" BorderStyle="None" Height="75px" Width="300px">
                            <FrameContentTemplate>
                                <div style="width: 100%;" class="bodyPopup">
                                    <table width="100%" cellspacing="0">

                                        <tr id="panNewListValueDragHandle" style="cursor: move; height: 20px;" class="defaultBackgroundColor">
                                            <td align="center">
                                                <asp:Image ID="imgNewListValue" runat="server" ImageUrl="Images/PRIMARY_KEY_16.GIF" Width="16px" Height="16px" />
                                            </td>
                                            <td>
                                                <asp:Label ID="lblNewListValueTitle" Text="Nuevo valor de la lista" ForeColor="white" runat="server" />
                                            </td>
                                            <td align="right">
                                                <asp:ImageButton ID="ibtNewListValueOK" runat="server" ImageUrl="~/Base/Images/ButtonOK_16.gif" OnClientClick="hidePopup('mpxNewListValueBehavior'); AddListValueOK(); return false;" Style="cursor: pointer;" />
                                                <img id="ibtNewListValueCancel" onclick="hidePopup('mpxNewListValueBehavior');" style="cursor: pointer;" src="~/Base/Images/ButtonCancel_16.gif" runat="server" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="3" style="padding-left: 10px; padding-top: 5px; vertical-align: middle">
                                                <asp:Label ID="lblListValueTitle2" runat="server" Text="Introuzca el nuevo valor de la lista" />
                                                <br />
                                                <asp:TextBox ID="txtNewListValue" Width="100%" runat="server"></asp:TextBox>
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                            </FrameContentTemplate>
                        </roWebControls:roPopupFrameV2>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
            <asp:Button ID="btRequestCriteriaOK" Style="display: none;" runat="server" />
            <asp:UpdatePanel ID="updRequestCriteria" runat="server">
                <ContentTemplate>
                    <ajaxToolkit:ModalPopupExtender ID="mpxRequestCriteria" runat="server" BehaviorID="mpxRequestCriteriaBehavior"
                        TargetControlID="hiddenTargetControlForRequestCriteria"
                        PopupControlID="divRequestCriteria"
                        DropShadow="False"
                        PopupDragHandleControlID="panRequestCriteriaDragHandle"
                        X="10" Y="10" EnableViewState="true">
                    </ajaxToolkit:ModalPopupExtender>
                    <asp:Button runat="server" ID="hiddenTargetControlForRequestCriteria" Style="display: none" />
                    <div id="divRequestCriteria" runat="server" style="display: none">
                        <roWebControls:roPopupFrameV2 ID="ropfREquestCriteria" runat="server" BorderStyle="None" Height="75px" Width="530px">
                            <FrameContentTemplate>
                                <div style="width: 100%;" class="bodyPopup">
                                    <table width="100%" cellspacing="0">

                                        <tr id="panRequestCriteriaDragHandle" style="cursor: move; height: 20px;" class="defaultBackgroundColor">
                                            <td align="center">
                                                <asp:Image ID="imgRequestCriteria" runat="server" ImageUrl="Images/PRIMARY_KEY_16.GIF" Width="16px" Height="16px" />
                                            </td>
                                            <td>
                                                <asp:Label ID="lblRequestCriteriaTitle" Text="Configuración criterio" ForeColor="white" runat="server" />
                                            </td>
                                            <td align="right">
                                                <asp:ImageButton ID="ibtRequestCriteriaOK" runat="server" ImageUrl="~/Base/Images/ButtonOK_16.gif" OnClientClick="hidePopup('mpxRequestCriteriaBehavior'); RequestCriteriaOK(); return false;" Style="cursor: pointer;" />
                                                <img id="ibtRequestCriteriaCancel" onclick="hidePopup('mpxRequestCriteriaBehavior');" style="cursor: pointer;" src="~/Base/Images/ButtonCancel_16.gif" runat="server" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="3" style="padding-left: 10px; padding-top: 5px; vertical-align: middle">
                                                <roUserControls:roUserCtlFieldCriteria ID="usrRequestCriteria" runat="server" />
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                            </FrameContentTemplate>
                        </roWebControls:roPopupFrameV2>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>

            <Local:MessageFrame ID="MessageFrame1" runat="server" />
        </div>
    </form>
</body>
</html>