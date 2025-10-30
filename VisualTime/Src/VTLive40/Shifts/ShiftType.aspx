<%@ Page Language="VB" AutoEventWireup="false" Inherits="VTLive40.ShiftType" CodeBehind="ShiftType.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html lang="en" class="notranslate" translate="no" xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Tipo ${Shift}</title>
</head>
<body class="bodyPopup" style="background-attachment: fixed;">
    <form id="frmShiftType" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="True" EnableScriptLocalization="True" />

        <div>

            <asp:UpdatePanel ID="updShiftType" runat="server">
                <ContentTemplate>

                    <table cellpadding="0" cellspacing="0" width="100%" height="100%" style="margin-top: -5px;">
                        <tr>
                            <td style="padding-top: 5px; padding-bottom: 0px;" valign="top" height="20px">
                                <div class="panHeader2">
                                    <span style="">
                                        <asp:Label ID="lblNewShiftTypeTitle" Text="Seleccione el tipo de ${Shift}" runat="server" /></span>
                                </div>
                                <br />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <roUserControls:roOptionPanelContainer ID="optNewShift_Rigid" runat="server" TypeOPanel="RadioOption" Width="100%" Height="Auto"
                                    Text="Rígido (de X a Y)"
                                    Description="${Shift} en el que el ${Employee} ha de estar presente obligatoriamente, durante una franja horaria concreta."
                                    Checked="true">
                                    <Content></Content>
                                </roUserControls:roOptionPanelContainer>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <roUserControls:roOptionPanelContainer ID="optNewShift_PartRigid" runat="server" TypeOPanel="RadioOption" Width="100%" Height="Auto"
                                    Text="Rígido partido (de X a Y y de Z a W)"
                                    Description="Igual que el anterior, pero con dos franjas horarias."
                                    Checked="false">
                                    <Content></Content>
                                </roUserControls:roOptionPanelContainer>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <roUserControls:roOptionPanelContainer ID="optNewShift_SemiFlex" runat="server" TypeOPanel="RadioOption" Width="100%" Height="Auto"
                                    Text="Semi-Flexible(Ent. de X a Y, Sal. a las Z horas, o W horas después de la Ent.)"
                                    Description="${Shift} en el que la entrada y la salida se pueden realizar durate un rango de horas, pero en el que hay que trabajar un tiempo obligado o salir a una hora fija."
                                    Checked="false">
                                    <Content></Content>
                                </roUserControls:roOptionPanelContainer>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <roUserControls:roOptionPanelContainer ID="optNewShift_PartSemiFlex" runat="server" TypeOPanel="RadioOption" Width="100%" Height="Auto"
                                    Text="Partido Semi-Flexible (igual que el anterior, pero con dos franjas)"
                                    Description="Igual que el anterior, pero con dos franjas horarias."
                                    Checked="false">
                                    <Content></Content>
                                </roUserControls:roOptionPanelContainer>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <roUserControls:roOptionPanelContainer ID="optNewShift_Flex" runat="server" TypeOPanel="RadioOption" Width="100%" Height="Auto"
                                    Text="100% Flexible (Z horas entre X e Y horas)"
                                    Description="${Shift} en el que se ha de trabajar un total de horas, pero entrando y saliendo cuando uno quiera."
                                    Checked="false">
                                    <Content></Content>
                                </roUserControls:roOptionPanelContainer>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <roUserControls:roOptionPanelContainer ID="optNewShift_Delay" runat="server" TypeOPanel="RadioOption" Width="100%" Height="Auto"
                                    Text="Descanso/Vacaciones/Fiesta"
                                    Description="Días que no se trabajan."
                                    Checked="false">
                                    <Content></Content>
                                </roUserControls:roOptionPanelContainer>
                            </td>
                        </tr>
                        <tr>
                            <td align="right">
                                <table cellpadding="0" cellspacing="0">
                                    <tr align="right">
                                        <td>
                                            <asp:Button ID="btOKNewShift" Text="${Button.Accept}" runat="server" CssClass="btnFlat btnFlatBlack btnFlatAsp" />
                                        </td>
                                        <td>
                                            <asp:Button ID="btCancelNewShift" Text="${Button.Cancel}" runat="server" CssClass="btnFlat btnFlatBlack btnFlatAsp" />
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>

                    <roUserControls:roOptionPanelGroup ID="optGroup" runat="server" />

                    <asp:HiddenField ID="hdnCanClose_PageBase" runat="server" />
                    <asp:HiddenField ID="hdnMustRefresh_PageBase" Value="0" runat="server" />
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </form>
</body>
</html>