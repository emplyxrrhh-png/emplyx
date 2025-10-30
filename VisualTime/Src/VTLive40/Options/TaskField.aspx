<%@ Page Language="VB" AutoEventWireup="false" Inherits="VTLive40.OptionsTaskField" CodeBehind="TaskField.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html lang="en" class="notranslate" translate="no" xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Campo personalizado</title>
</head>
<body class="bodyPopup" style="background-attachment: fixed;">

    <form id="frmTaskField" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="True" EnableScriptLocalization="True" />

        <div>

            <script language="javascript" type="text/javascript">

                function PageBase_Load() {

                    ConvertControls();

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

                function endRequestHandler() {
                    ConvertControls();
                }

                function VisibleAccessValidationControl(visible) {

                }

                function ShowAddCategory(button) {

                }
                function AddCategoryOK() {
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
                        var message = "TitleKey=SaveTaskField.Check.Invalid.Text&" +
                            "DescriptionKey=SaveTaskField.Check.Invalid.Description&" +
                            "Option1TextKey=SaveTaskField.Check.Invalid.Option1Text&" +
                            "Option1DescriptionKey=SaveTaskField.Check.Invalid.Option1Description&" +
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
                }

                function ShowRequestCriteria() {

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
                <tr style="vertical-align: top; height: 300px;">
                    <td colspan="2">

                        <asp:UpdatePanel ID="updTaskField" runat="server">
                            <ContentTemplate>
                                <roUserControls:roTabContainerClient ID="tabCtl01" runat="server">
                                    <TabTitle1>
                                        <asp:Label ID="lblGeneralTitle" runat="server" Text="General"></asp:Label>
                                    </TabTitle1>
                                    <TabContainer1>

                                        <table cellpadding="1" cellspacing="1" width="100%" style="padding-top: 0px; height: 100%;" border="0">
                                            <tr style="padding-top: 10px;">
                                                <td align="right" style="height: 10px;">
                                                    <asp:Label ID="lblName" Text="Nombre" runat="server" />
                                                </td>
                                                <td style="padding-left: 5px" style="height: 10px;" colspan="1">
                                                    <input type="text" id="txtName" value="" style="width: 204px;" runat="server" convertcontrol="TextField" ccallowblank="false" ccmaxlength="50" class="textClass" />
                                                </td>
                                            </tr>
                                            <tr style="padding-top: 5px">
                                                <td align="right" style="height: 10px;">
                                                    <asp:Label ID="lblType" Text="Tipo" runat="server" />
                                                </td>
                                                <td style="padding-left: 5px" style="height: 10px;" colspan="2">
                                                    <roWebControls:roComboBox ID="cmbTypes" runat="server" ItemsRunAtServer="false" ParentWidth="170px" ChildsWidth="50px" EnableViewState="true" HiddenText="cmbTypes_Text" ChildsVisible="8" AutoResizeChildsWidth="true" HiddenValue="cmbTypes_Value" />
                                                    <asp:HiddenField ID="cmbTypes_Text" runat="server" />
                                                    <asp:HiddenField ID="cmbTypes_Value" runat="server" />
                                                    <%--
                                    <asp:DropDownList ID="ddlTypes" AutoPostBack ="false" Width="200" runat="server" CssClass="textClass" />
                                                    --%>
                                                </td>
                                            </tr>
                                            <tr style="padding-top: 5px">
                                                <td align="right" style="height: 10px;">
                                                    <asp:Label ID="lblAction" Text="Cuando" runat="server" />
                                                </td>
                                                <td style="padding-left: 5px" style="height: 10px;" colspan="2">
                                                    <roWebControls:roComboBox ID="cmbActions" runat="server" ItemsRunAtServer="false" ParentWidth="170px" ChildsWidth="50px" EnableViewState="true" HiddenText="cmbTypes_Text" ChildsVisible="8" AutoResizeChildsWidth="true" HiddenValue="cmbActions_Value" />
                                                    <asp:HiddenField ID="cmbActions_Text" runat="server" />
                                                    <asp:HiddenField ID="cmbActions_Value" runat="server" />
                                                    <%--
                                    <asp:DropDownList ID="ddlActions" AutoPostBack ="false" Width="200" runat="server" CssClass="textClass" />
                                                    --%>
                                                </td>
                                            </tr>
                                            <tr style="padding-top: 5px">
                                                <td align="right" style="height: 10px;">
                                                    <asp:Label ID="lblValueTypes" Text="Tipo valores" runat="server" />
                                                </td>
                                                <td style="padding-left: 5px" style="height: 10px;" colspan="2">
                                                    <roWebControls:roComboBox ID="cmbValueTypes" runat="server" ItemsRunAtServer="false" ParentWidth="170px" ChildsWidth="50px" EnableViewState="true" HiddenText="cmbTypes_Text" ChildsVisible="8" AutoResizeChildsWidth="true" HiddenValue="cmbValueTypes_Value" />
                                                    <asp:HiddenField ID="cmbValueTypes_Text" runat="server" />
                                                    <asp:HiddenField ID="cmbValueTypes_Value" runat="server" />
                                                    <%--
                                    <asp:DropDownList ID="ddlValueTypes" AutoPostBack ="false" Width="200" runat="server" CssClass="textClass" />
                                                    --%>
                                                </td>
                                            </tr>
                                            <tr style="padding-top: 5px">
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
                                                            <img id="imgAddListValue" src="~/Base/Images/Grid/add.png" visible="true" runat="server" style="cursor: pointer;" onclick="ShowAddListValue(this); " />
                                                            <img id="imgRemoveListValue" src="~/Base/Images/Grid/remove.png" visible="true" runat="server" style="cursor: pointer;" onclick="RemoveListValue();" />
                                                        </div>
                                                    </div>
                                                </td>
                                            </tr>
                                        </table>
                                    </TabContainer1>
                                </roUserControls:roTabContainerClient>
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
                <tr>
                    <td colspan="2" align="center">
                        <asp:UpdatePanel ID="updError" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                            <ContentTemplate>
                                <asp:Label ID="lblError" Visible="false" CssClass="errorText" runat="server" />
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