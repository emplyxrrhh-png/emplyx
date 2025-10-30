<%@ Page Language="VB" AutoEventWireup="false" Inherits="VTLive40.AnnualView" CodeBehind="AnnualView.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html lang="en" class="notranslate" translate="no" xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Annual View</title>

    <script language="javascript" type="text/javascript">

        function PageBase_Load() {
            CloseIfNeeded();
        }

        function CloseIfNeeded() {
            var _CanClose = document.getElementById("hdnCanClose");
            if (_CanClose.value == '1') Close();
        }

        function PageBase_Unload() {
            var hdnDaySelectedToView = document.getElementById("hdnDaySelectedToView");
            if (hdnDaySelectedToView.value != "") {
                try {
                    parent.frames['ifPrincipal'].showDaySelected(hdnDaySelectedToView.value);
                }
                catch (e) {
                    alert(e);
                }
            }
        }

        function getSelectedDay(daySelected) {
            var hdnDaySelectedToView = document.getElementById("hdnDaySelectedToView");
            hdnDaySelectedToView.value = daySelected;
            Close();
        }

        function Imprimir(que) {
            PrintObject(document.getElementById(que).innerHTML, 'Styles/AnnualView.css');
        }
    </script>
</head>

<body>

    <form id="form1" runat="server" style="padding-left: 2px;">

        <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="True" EnableScriptLocalization="True" />

        <div id="General">

            <asp:HiddenField ID="hdnCanClose" runat="server" />

            <asp:HiddenField ID="hdnDaySelectedToView" runat="server" Value="" />

            <div style="padding-left: 10px;">
                <table width="100%" cellpadding="0" cellspacing="0">
                    <tr>
                        <td>
                            <asp:Label ID="lblEmployee" runat="server" Text="" Font-Bold="true"></asp:Label>
                        </td>
                        <td align="right" style="padding-right: 25px;" class="noprint">
                            <a id="aPrint" href="javascript: void(0);" class="icoPrintFast" title="<%= Me.Language.Translate("aPrint",Me.DefaultScope) %>" onclick="Imprimir('General');"></a>
                        </td>
                    </tr>
                </table>
            </div>

            <!-- <div id="xplaceAnnualx" runat="server" style="text-align: center; width: 100%;"></div> -->

            <div id="placeAnnual" runat="server" class="annualStyle">
                <table style="width: 100%;">
                    <tr>
                        <td id="tdPlaceMonths" runat="server"></td>
                        <td id="tdPlaceSelectors" runat="server">
                            <table border="0" cellpadding="0" cellspacing="0" style="height: 512px;">
                                <tr>
                                    <td style="width: 155px; vertical-align: top;">
                                        <div style="height: 20px; padding: 5px; text-align: left;">
                                            <table>
                                                <tr>
                                                    <td><a id="PreviousYear" runat="server" onclick="parent.showLoader(true);" href="javascript: void(0);" class="stock2-previous" alt="&lt;"></a></td>
                                                    <td id="tdYearSelected" runat="server" style="width: 105px; text-align: center; font-weight: bold; font-size: 13px;"></td>
                                                    <td><a id="NextYear" runat="server" onclick="parent.showLoader(true);" href="javascript: void(0);" class="stock2-next" alt="&gt;"></a></td>
                                                </tr>
                                            </table>
                                        </div>
                                        <div style="height: 45px;">
                                            <table style="width: 100%;" border="0" cellpadding="0" cellspacing="0">
                                                <tr>
                                                    <td style="white-space: nowrap;">
                                                        <asp:Label ID="lblContract" runat="server" Text="Contrato" class="spanEmp-class"></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <dx:ASPxComboBox ID="cmbContractsDev" runat="server" CssClass="editTextFormat" Font-Size="11px" Width="158" Border-BorderColor="Silver" AutoPostBack="True">
                                                            <ClientSideEvents SelectedIndexChanged="function (s,e) { parent.showLoader(true);}" />
                                                        </dx:ASPxComboBox>
                                                    </td>
                                                </tr>
                                            </table>
                                        </div>
                                        <div>
                                            <asp:Label ID="lblShifts" runat="server" Text="Horarios" class="spanEmp-class"></asp:Label>
                                        </div>
                                        <div id="divShifts" runat="server" style="height: 340px; overflow: auto; padding: 2px; border: solid 1px silver; margin-bottom: 5px; text-align: left;" class="defaultContrastColor"></div>
                                        <div>
                                            <asp:Label ID="lblTotalPlanned" runat="server" Text="Total horas planificadas" class="spanEmp-class"></asp:Label>
                                        </div>
                                        <div id="divTotalPlanned" runat="server" style="height: 50px; overflow: auto; padding: 2px; border: solid 1px silver; text-align: left;" class="defaultContrastColor"></div>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </div>

            <div style="padding-top: 2px;">
                <asp:Label ID="lblInfo" runat="server" Text="(Si pulsa doble click sobre un día determinado se situará automáticamente en la planificación de ese día)"></asp:Label>
            </div>
        </div>
    </form>
</body>
</html>