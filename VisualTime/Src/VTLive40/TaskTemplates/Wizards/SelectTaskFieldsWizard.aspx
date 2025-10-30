<%@ Page Language="VB" AutoEventWireup="false" Inherits="VTLive40.Wizards_SelectTaskFieldsWizard" Culture="auto" UICulture="auto" CodeBehind="SelectTaskFieldsWizard.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html lang="en" class="notranslate" translate="no" xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Asistente para la asignación de campos de tarea</title>
</head>
<body class="bodyPopup" style="background-attachment: fixed;">

    <form id="frmSelectTaskFieldsTemplates" runat="server">

        <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="True" EnableScriptLocalization="True" />

        <div>

            <script language="javascript" type="text/javascript">

                var bolLoaded = false;

                function PageBase_Load() {

                    var oActiveFrameIndex = document.getElementById('<%= hdnActiveFrame.ClientID %>').value;
                    ConvertControls('divStep' + oActiveFrameIndex);

                    if (bolLoaded == false) bolLoaded = true;
                }

                function CheckFrame(intFrameIndex) {
                    var bolRet = true;

                    if (intFrameIndex == null) {
                        intFrameIndex = parseInt(document.getElementById('<%= hdnActiveFrame.ClientID %>').value);
                    }

                    if (CheckConvertControls('divStep' + intFrameIndex) == false) {
                        bolRet = false;
                    }

                    if (!bolRet) hidePopupLoader();

                    return bolRet;
                }

                function endRequestHandler() {
                    hidePopupLoader();
                }

                function showPopupLoader() {
                    if (typeof (window.parent.frames["ifPrincipal"]) != "undefined") {
                        window.parent.frames["ifPrincipal"].showLoadingGrid(true);
                    } else {
                        window.parent.parent.frames["ifPrincipal"].showLoadingGrid(true);
                    }
                }

                function hidePopupLoader() {
                    if (typeof (window.parent.frames["ifPrincipal"]) != "undefined") {
                        window.parent.frames["ifPrincipal"].showLoadingGrid(false);
                    } else {
                        window.parent.parent.frames["ifPrincipal"].showLoadingGrid(false);
                    }
                }

                function Refresh() {
                    var DataChanged = document.getElementById('<%= hdnMustRefresh_PageBase.ClientID %>').value;
                    if (DataChanged == '1') parent.RefreshScreen('1', document.getElementById('<%= hdnParams_PageBase.ClientID %>').value);
                }
            </script>

            <table style="width: 500px; display: block;" cellpadding="0" cellspacing="0">
                <tr>
                    <td style="border: none 1px black;">

                        <asp:UpdatePanel ID="updStep0" runat="server" RenderMode="Inline">
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="btEnd" EventName="Click" />
                            </Triggers>
                            <ContentTemplate>

                                <%-- Selección de campos disponibles --%>
                                <div id="divStep0" runat="server" style="display: block;">
                                    <table style="width: 500px; height: 410px;" cellspacing="0" cellpadding="0">
                                        <tr>
                                            <td class="LaunchTaskTemplateWizard_StepTitle">
                                                <table style="width: 100%">
                                                    <tr>
                                                        <td>
                                                            <asp:Label ID="lblStep1Title" runat="server" Text="" Font-Bold="True" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td style="padding-left: 20px; padding-right: 50px;">
                                                            <asp:Label ID="lblStep1Info" runat="server" Text="" />
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="LaunchTaskTemplateWizard_StepContent">

                                                <table border="0" style="width: 100%; padding-left: 10px;">
                                                    <tr>
                                                        <td style="height: 40px;">
                                                            <asp:Label ID="lblTreeTaskFields" runat="server" Text="Campos disponibles:"></asp:Label>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            <asp:TreeView ID="treeTaskFields" ShowCheckBoxes="All" ShowExpandCollapse="false" runat="server"></asp:TreeView>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="LaunchTaskTemplateWizard_StepError">
                                                <asp:Label ID="lblStep1Error" runat="server" CssClass="errorText" />
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:UpdatePanel ID="updButtons" runat="server" RenderMode="Inline">
                            <ContentTemplate>

                                <table align="right" cellpadding="0" cellspacing="0">
                                    <tr class="LaunchTaskTemplateWizard_ButtonsPanel" style="height: 44px">
                                        <td>&nbsp
                                        </td>
                                        <td>&nbsp
                                        </td>
                                        <td>
                                            <asp:Button ID="btEnd" Text="${Button.End}" runat="server" TabIndex="4" CssClass="btnFlat btnFlatBlack btnFlatAsp" />
                                        </td>
                                        <td>
                                            <asp:Button ID="btClose" Text="${Button.Cancel}" runat="server" OnClientClick="" TabIndex="5" CssClass="btnFlat btnFlatBlack btnFlatAsp" />

                                            <asp:HiddenField ID="hdnCanClose_PageBase" runat="server" />
                                            <asp:HiddenField ID="hdnMustRefresh_PageBase" Value="0" runat="server" />
                                            <asp:HiddenField ID="hdnParams_PageBase" runat="server" />
                                        </td>
                                    </tr>
                                </table>

                                <input type="hidden" id="hdnActiveFrame" value="0" runat="server" />
                                <input type="hidden" id="hdnIDEmployeeSource" value="" runat="server" />
                                <input type="hidden" id="hdnFrames" value="" runat="server" />
                                <input type="hidden" id="hdnFramesOnlyClient" value="" runat="server" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>