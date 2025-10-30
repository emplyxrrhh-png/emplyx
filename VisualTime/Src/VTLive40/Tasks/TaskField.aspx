<%@ Page Language="VB" AutoEventWireup="false" Inherits="VTLive40.TaskField" CodeBehind="TaskField.aspx.vb" %>

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

                function endRequestHandler() {
                    ConvertControls();
                }

                function Refresh() {
                    var DataChanged = document.getElementById('<%= hdnMustRefresh_PageBase.ClientID %>').value;
                if (DataChanged == '1') parent.RefreshTaskFieldsScreen('1', document.getElementById('<%= hdnParams_PageBase.ClientID %>').value);
                }

                function onChangeField(newValue, newtypevalue, newid) {
                    var stamp = '&StampParam=' + new Date().getMilliseconds();

                    roCB_setValue(newValue, '<%= cmbActions.ClientID %>_ComboBoxLabel', '<%= cmbActions_Text.ClientID %>', '<%= cmbActions_Value.ClientID %>')

                roCB_disable('<%= cmbListValues.ClientID %>_ComboBoxLabel', true)

                document.getElementById('<%= cmbListValues_Text.ClientID %>').value = '';
                document.getElementById('<%= cmbListValues.ClientID %>').style.display = 'none';
                document.getElementById('<%= lblList.ClientID %>').style.display = 'none';
                document.getElementById('<%= txtValue.ClientID %>').value = '';

                if (newValue == '0') {
                    if (newtypevalue == '0') {
                        document.getElementById('<%= txtValue.ClientID %>').disabled = '';
                        roCB_disable('<%= cmbListValues.ClientID %>_ComboBoxLabel', true)

                        document.getElementById('<%= cmbListValues.ClientID %>').style.display = 'none';
                        document.getElementById('<%= lblList.ClientID %>').style.display = 'none';

                        document.getElementById('<%= cmbListValues_Text.ClientID %>').value = '';

                    } else {
                        document.getElementById('<%= txtValue.ClientID %>').value = '';

                        document.getElementById('<%= cmbListValues.ClientID %>').style.display = '';
                        document.getElementById('<%= lblList.ClientID %>').style.display = '';

                        roCB_disable('<%= cmbListValues.ClientID %>_ComboBoxLabel', false)

                        var cmb3 = document.getElementById('<%= cmbListValues.ClientID %>');
                        var cmb3_Text = document.getElementById('<%= cmbListValues_Text.ClientID %>');
                        var cmb3_Value = document.getElementById('<%= cmbListValues_Value.ClientID %>');

                        var tmvalue = '';

                        ajax = nuevoAjax();
                        ajax.open("GET", "srvTasks.aspx?action=getTaskFieldList&fieldListID=" + newid + stamp, true);

                        ajax.onreadystatechange = function () {
                            if (ajax.readyState == 4) {
                                eval("var arrObj = new Array(" + ajax.responseText + ");");
                                roCB_clearItems(cmb3.id, cmb3_Text.id, cmb3_Value.id);
                                for (var n = 0; n < arrObj.length; n++) {
                                    roCB_addItem(cmb3.id, arrObj[n], arrObj[n], 'onChangeListValue("' + arrObj[n] + '");');
                                    if (n == 0) {
                                        tmvalue = arrObj[n];
                                        document.getElementById('<%= txtValue.ClientID %>').value = arrObj[n];
                                    roCB_setValue(tmvalue, '<%= cmbListValues.ClientID %>_ComboBoxLabel', '<%= cmbListValues_Text.ClientID %>', '<%= cmbListValues_Value.ClientID %>')
                                    }

                                }
                            }
                        }
                        ajax.send(null)

                    }

                } else {
                    document.getElementById('<%= txtValue.ClientID %>').disabled = 'disabled';
                    roCB_disable('<%= cmbListValues.ClientID %>_ComboBoxLabel', true)
                    document.getElementById('<%= cmbListValues_Text.ClientID %>').value = '';
                    document.getElementById('<%= cmbListValues.ClientID %>').style.display = 'none';
                    document.getElementById('<%= lblList.ClientID %>').style.display = 'none';

                    }

                }

                function onChangeListValue(newValue) {

                    document.getElementById('<%= txtValue.ClientID %>').value = newValue;

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
                        <asp:Label ID="lblDescription" runat="server" CssClass="editTextFormat" Text="Puede definir un campo personalizado para la ficha de la ${Task}."></asp:Label>
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
                                                    <roWebControls:roComboBox ID="cmbAvailableFields" runat="server" ItemsRunAtServer="false" ParentWidth="170px" ChildsWidth="50px" EnableViewState="true" HiddenText="cmbAvailableFields_Text" ChildsVisible="8" AutoResizeChildsWidth="true" HiddenValue="cmbAvailableFields_Value" />
                                                    <asp:HiddenField ID="cmbAvailableFields_Text" runat="server" />
                                                    <asp:HiddenField ID="cmbAvailableFields_Value" runat="server" />
                                                </td>
                                            </tr>
                                            <tr style="padding-top: 5px; display: none;">
                                                <td align="right" style="height: 10px;">
                                                    <asp:Label ID="lblType" Text="Tipo" runat="server" />
                                                </td>
                                                <td style="padding-left: 5px" style="height: 10px;" colspan="2">
                                                    <roWebControls:roComboBox ID="cmbActions" runat="server" ItemsRunAtServer="false" ParentWidth="170px" ChildsWidth="50px" EnableViewState="true" HiddenText="cmbActions_Text" ChildsVisible="8" AutoResizeChildsWidth="true" HiddenValue="cmbActions_Value" />
                                                    <asp:HiddenField ID="cmbActions_Text" runat="server" />
                                                    <asp:HiddenField ID="cmbActions_Value" runat="server" />
                                                </td>
                                            </tr>
                                            <tr style="padding-top: 5px">
                                                <td align="right" style="height: 10px;">
                                                    <asp:Label ID="lblAction" Text="Valor" runat="server" />
                                                </td>
                                                <td style="padding-left: 5px" style="height: 10px;" colspan="2">
                                                    <input type="text" runat="server" id="txtValue" class="textClass x-form-text x-form-field" maxlength="50" style="width: 45px;" convertcontrol="TextField" ccallowblank="true" />
                                                </td>
                                            </tr>
                                            <tr style="padding-top: 10px;">
                                                <td align="right" style="height: 10px;">
                                                    <asp:Label ID="lblList" Text="Lista" runat="server" />
                                                </td>
                                                <td style="padding-left: 5px" style="height: 10px;" colspan="1">
                                                    <roWebControls:roComboBox ID="cmbListValues" runat="server" ItemsRunAtServer="false" ParentWidth="170px" ChildsWidth="50px" EnableViewState="true" HiddenText="cmbListValues_Text" ChildsVisible="8" AutoResizeChildsWidth="true" HiddenValue="cmbListValues_Value" />
                                                    <asp:HiddenField ID="cmbListValues_Text" runat="server" />
                                                    <asp:HiddenField ID="cmbListValues_Value" runat="server" />
                                                </td>
                                            </tr>
                                        </table>
                                    </TabContainer1>
                                </roUserControls:roTabContainerClient>
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
                                            <asp:Button ID="btAccept" Text="${Button.Accept}" runat="server" CssClass="btnFlat btnFlatBlack btnFlatAsp" />
                                        </td>
                                        <td>
                                            <asp:Button ID="btCancel" Text="${Button.Cancel}" runat="server" CssClass="btnFlat btnFlatBlack btnFlatAsp" />
                                        </td>
                                    </tr>
                                </table>
                                <asp:HiddenField ID="hdnCanClose_PageBase" runat="server" />
                                <asp:HiddenField ID="hdnMustRefresh_PageBase" Value="0" runat="server" />
                                <asp:HiddenField ID="hdnParams_PageBase" runat="server" />
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