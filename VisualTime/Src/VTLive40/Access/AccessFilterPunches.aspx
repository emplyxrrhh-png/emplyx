<%@ Page Language="VB" AutoEventWireup="false" EnableEventValidation="false" Inherits="VTLive40.AccessFilterPunches" Culture="auto" UICulture="auto" CodeBehind="AccessFilterPunches.aspx.vb" %>

<html lang="en" class="notranslate" translate="no" xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Filtro de marcajes de acceso</title>
</head>

<body>
    <script language="javascript" type="text/javascript">

        var oFilter; //Grid class
        var bolLoaded = false;

        async function PageBase_Load() {
            if (!bolLoaded) {
                await getroTreeState('objContainerTreeV3_treeEmpFilterPunches').then(roState => roState.reset());
                await getroTreeState('objContainerTreeV3_treeEmpFilterPunchesGrid').then(roState => roState.reset());
                bolLoaded = true;
            }

            ConvertControls();
            RefreshGridAccess();

            if (!bolLoaded) bolLoaded = true;
        }

        //==========================================================================
        //Muestra ventana modal con el selector y botones de aceptar/cancelar
        //==========================================================================
        function ShowGroupSelector() {
            $find('RoPopupFrame1Behavior').show();
            $get('<%= RoPopupFrame1.ClientID %>').style.display = '';
            document.getElementById('RoPopupFrame1MyPopupFrame_DIV').style.top = '20px';
            document.getElementById('RoPopupFrame1MyPopupFrame_DIV').style.left = '120px';
        }

        //==========================================================================
        //Oculta ventana modal con el selector y botones de aceptar/cancelar
        //==========================================================================
        function HideGroupSelector(IsOk) {

            if (IsOk == false) {
                document.getElementById('aFEmployees').textContent = document.getElementById('<%= lblAllEmp.ClientID %>').textContent;
                document.getElementById('hdnEmployees').value = "ALL";
                document.getElementById('hdnFilter').value = "";
                document.getElementById('hdnFilterUser').value = "";
            }

            $find('RoPopupFrame1Behavior').hide();
            $get('<%= RoPopupFrame1.ClientID %>').style.display = 'none';

        }

        //==========================================================================
        //Muestra el selector de zonas o el selector de empleados con TreeV3
        //==========================================================================
        function ShowSelector(oAction, oSel) {
            try {
                var Title = '';
                var iFrm = document.getElementById('<%= GroupSelectorFrame.ClientID %>');
                iFrm.style.width = "500px";
                iFrm.style.height = "400px";

                iFrm.style.top = "5px";
                iFrm.style.left = "5px";

                if (oAction == "E") {
                    if (oSel == 1) {
                        document.getElementById('aFEmployees').textContent = document.getElementById('<%= lblAllEmp.ClientID %>').textContent;
                        document.getElementById('hdnEmployees').value = "ALL";
                        document.getElementById('hdnFilter').value = "";
                        document.getElementById('hdnFilterUser').value = "";
                    }
                    else {

                        document.getElementById('aFEmployees').textContent = document.getElementById('<%= lblEmpSelect.ClientID %>').textContent;

                        var strBase = '<%= Me.Page.ResolveURL("~/Base/") %>' + "WebUserControls/roWizardSelectorContainerMultiSelectV3.aspx?" +
                            "PrefixTree=treeEmpFilterPunches&FeatureAlias=Access&PrefixCookie=objContainerTreeV3_treeEmpFilterPunchesGrid&" +
                            "AfterSelectFuncion=parent.GetSelectedTreeV3";
                        iFrm.src = strBase;

                    }
                }
                else {
                    if (oSel == 1) {
                        document.getElementById('aFZones').textContent = document.getElementById('<%= lblAllZone.ClientID %>').textContent;
                        document.getElementById('hdnZones').value = "ALL";
                    } else {
                        var hBase = '<%= Me.Page.ResolveURL("~/Base/") %>';
                        iFrm.src = hBase + "WebUserControls/roWizardSelectorContainerMultiSelectV3.aspx?TreesEnabled=100&" +
                            "TreesMultiSelect=100&TreesOnlyGroups=000&" +
                            "TreeFunction=parent.GroupZonesSelected&FilterFloat=false&" +
                            "PrefixTree=AccFilterTreeGroupAccessZone&" +
                            "FiltersVisible=0000&AdvancedFilterVisible=false&" +
                            "TreeImagePath=Images/AccessZonesSelector" +
                            "&TreeSelectorPage=../../Access/AccessFilterPunchesSelectorData.aspx&" +
                            "FeatureAlias=Access&EnableDD=00&Tree1FunctDD=";
                    }
                }

                //Mostrar popup
                if (oSel != 1) {
                    ShowGroupSelector();
                }
            }
            catch (e) {
                showError("ShowSelector", e);
            }
        }

        //==========================================================================
        //Guarda los empleados seleccionados en el TreeV3
        //==========================================================================
        function GetSelectedTreeV3(oParm1, oParm2, oParm3) {
            if (oParm1 == "") {
                document.getElementById('hdnEmployees').value = "ALL";
                document.getElementById('hdnFilter').value = "";
                document.getElementById('hdnFilterUser').value = "";
            }
            else {
                document.getElementById('hdnEmployees').value = oParm1;
                document.getElementById('hdnFilter').value = oParm2;
                document.getElementById('hdnFilterUser').value = oParm3;
            }

            if (document.getElementById('hdnEmployees').value == "ALL")
                document.getElementById('aFEmployees').textContent = document.getElementById('<%= lblAllEmp.ClientID %>').textContent;
            else
                document.getElementById('aFEmployees').textContent = document.getElementById('<%= lblEmpSelect.ClientID %>').textContent;
        }

        //==========================================================================
        //Guarda las zonas seleccionadas en el TreeV1
        //==========================================================================
        function GroupZonesSelected(oParm, oParm2, oParm3, oParm4) {
            var oZones = new Array();
            oZones = oParm2.split(",");
            var oCount = 0;
            var strNameZone = "";
            for (var x = 0; x < oZones.length; x++) {
                if (oZones[x].startsWith("B")) {
                    oCount++;
                    if (strNameZone == "") { strNameZone = oParm4[x].text; }
                }

            }
            if (oCount == 1) {
                document.getElementById('aFZones').textContent = strNameZone;
            } else {
                document.getElementById('aFZones').textContent = '(' + oCount + ') ' + '<%= Me.Language.TranslateJavaScript("SelectedZones",DefaultScope) %>';
            }
            document.getElementById('hdnZones').value = oParm;
        }

        //==========================================================================
        // funcion generica. Comprobació de temps
        //==========================================================================
        function AccessFilterPunches_CheckTime(obj, permitBlank) {
            try {
                if (permitBlank == false) {
                    if (obj.value == "") {
                        obj.focus();
                        showErrorPopup("Error.AccessFilterPunches." + obj.id + "Title", "ERROR", "Error.AccessFilterPunches." + obj.id + "Desc", "Error.AccessFilterPunches.OK", "Error.AccessFilterPunches.OKDesc", "");
                        return true;
                    }
                }

                var oField = Ext.getCmp(obj.getAttribute("ConvertedId"));
                if (oField != null) {
                    oField.validate();
                    if (oField.isValid(false) == false) {
                        obj.focus();
                        showErrorPopup("Error.AccessFilterPunches." + obj.id + "Title", "ERROR", "Error.AccessFilterPunches." + obj.id + "Desc", "Error.AccessFilterPunches.OK", "Error.AccessFilterPunches.OKDesc", "");
                        return true;
                    }
                }

                return false;
            } catch (e) { showError("AddException_CheckTime", e); }
        }

        //==========================================================================
        // funcion generica. aviso modal al usuario
        //==========================================================================
        function showErrorPopup(Title, typeIcon, Description, Opt1Text, Opt1Desc, strScript1, Opt2Text, Opt2Desc, strScript2, Opt3Text, Opt3Desc, strScript3) {
            try {
                var url = "/Access/srvMsgBoxAccess.aspx?action=Message";
                url = url + "&TitleKey=" + Title;
                url = url + "&DescriptionKey=" + Description;
                url = url + "&Option1TextKey=" + Opt1Text;
                url = url + "&Option1DescriptionKey=" + Opt1Desc;
                url = url + "&Option1OnClickScript=HideMsgBoxForm();" + strScript1 + "; return false;";
                if (Opt2Text != null) {
                    url = url + "&Option2TextKey=" + Opt2Text;
                    url = url + "&Option2DescriptionKey=" + Opt2Desc;
                    url = url + "&Option2OnClickScript=HideMsgBoxForm();" + strScript2 + "; return false;";
                }
                if (Opt3Text != null) {
                    url = url + "&Option3TextKey=" + Opt3Text;
                    url = url + "&Option3DescriptionKey=" + Opt3Desc;
                    url = url + "&Option3OnClickScript=HideMsgBoxForm();" + strScript3 + "; return false;";
                }
                if (typeIcon.toUpperCase() == "TRASH") {
                    url = url + "&IconUrl=~/Base/Images/MessageFrame/trash64.png";
                } else if (typeIcon.toUpperCase() == "ERROR") {
                    url = url + "&IconUrl=~/Base/Images/MessageFrame/alert32.png";
                } else if (typeIcon.toUpperCase() == "INFO") {
                    url = url + "&IconUrl=~/Base/Images/MessageFrame/dialog-information.png";
                }

                parent.ShowMsgBoxForm(url, 400, 300, '');
            } catch (e) { showError("showErrorPopup", e); }
        }

        //==========================================================================
        //==========================================================================
        //==========================================================================
        // REVISADO HASTA AQUI

        function SelectorOk() {
            //ButtonClick();
        }

        function SelectorAction(Action) {
            //createCookie('EmployeeMobility_SelectorAction', Action, 10);
        }

        function RefreshGridAccess(oOrder, objPrefix, fieldName) {
            try {

                //if (AccessFilterPunches_CheckTime(document.getElementById('dpDateBegin'))) { return false; }
                //if (AccessFilterPunches_CheckTime(document.getElementById('dpDateEnd'))) { return false; }
                if (AccessFilterPunches_CheckTime(document.getElementById('txtHourBegin'))) { return false; }
                if (AccessFilterPunches_CheckTime(document.getElementById('txtHourEnd'))) { return false; }

                if (document.getElementById('hdnEmployees').value == "ALL" || document.getElementById('hdnEmployees').value == "") {
                    document.getElementById('aFEmployees').textContent = document.getElementById('<%= lblAllEmp.ClientID %>').textContent;
                    document.getElementById('hdnEmployees').value = "ALL";
                    document.getElementById('hdnFilter').value = "";
                    document.getElementById('hdnFilterUser').value = "";
                }
                else {
                    document.getElementById('aFEmployees').textContent = document.getElementById('<%= lblEmpSelect.ClientID %>').textContent;
                }

                if (document.getElementById('hdnZones').value == "ALL" || document.getElementById('hdnZones').value == "") {
                    document.getElementById('aFZones').textContent = document.getElementById('<%= lblAllZone.ClientID %>').textContent;
                    document.getElementById('hdnZones').value = "ALL";
                }

                var oEmployees = document.getElementById('hdnEmployees').value;
                var oFilterTree = document.getElementById('hdnFilter').value;
                var oFilterTreeUser = document.getElementById('hdnFilterUser').value;

                var oZones = document.getElementById('hdnZones').value;
                //var oDateBegin = document.getElementById('dpDateBegin').value;
                var oDateBeginaux = dpDateBeginClient.GetValue();
                var oDateBegin = oDateBeginaux.getFullYear() + "/" + (oDateBeginaux.getMonth() + 1) + "/" + oDateBeginaux.getDate();

                //var oDateEnd = document.getElementById('dpDateEnd').value;
                var oDateEndaux = dpDateEndClient.GetValue();
                var oDateEnd = oDateEndaux.getFullYear() + "/" + (oDateEndaux.getMonth() + 1) + "/" + oDateEndaux.getDate();

                var oHourBegin = document.getElementById('txtHourBegin').value;
                var oHourEnd = document.getElementById('txtHourEnd').value;

                var oParms = {
                    "Employees": oEmployees,
                    "FilterTree": oFilterTree,
                    "FilterTreeUser": oFilterTreeUser,
                    "Zones": oZones,
                    "DateBegin": oDateBegin,
                    "DateEnd": oDateEnd,
                    "HourBegin": oHourBegin,
                    "HourEnd": oHourEnd
                };

                if (oFilter == null) {
                    oFilter = new DataAccessFilter(oParms, fieldName, oOrder);
                }
                else {
                    oFilter.refreshGrid(oParms, fieldName, oOrder);
                }

            } catch (e) { showError("RefreshGridAccess", e); }
        }

        function showCapture(IDCapture) {
            var iFrm = document.getElementById('<%= GroupSelectorFrame.ClientID %>');
            iFrm.style.width = "360px";
            iFrm.style.height = "360px";

            iFrm.src = "CapturePhoto.aspx?ID=" + IDCapture;
            $find('RoPopupFrame1Behavior').show();
            $get('<%= RoPopupFrame1.ClientID %>').style.display = '';
            document.getElementById('RoPopupFrame1MyPopupFrame_DIV').style.top = '40px';
            document.getElementById('RoPopupFrame1MyPopupFrame_DIV').style.left = '170px';
        }

        function editGridAccess() { }
        function sortGridAccess(obj, objPrefix, fieldName) {
            var oOrder = obj.getAttribute("ordering");
            if (oOrder == "DESC") { oOrder = "ASC"; } else { oOrder = "DESC"; }
            RefreshGridAccess(oOrder, objPrefix, fieldName);
        }
    </script>

    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="True" EnableScriptLocalization="True" />

        <input type="hidden" id="hdnParams" value="<%= Request("strParams") %>" />
        <input type="hidden" id="hdnImageID" value="<%= Request("ImageID") %>" />
        <input type="hidden" id="hdnFixPoints" value="<%= Request("FixPoints") %>" />

        <input type='hidden' id='hdnEmployees' runat='server' value='<%# Request("IDEmployee") %>' />
        <input type="hidden" id="hdnFilter" runat="server" value='<%# Request("FilterTree") %>' />
        <input type="hidden" id="hdnFilterUser" runat="server" value='<%# Request("FilterTreeUser") %>' />

        <input type='hidden' id='hdnZones' runat='server' value='<%# Request("IDZone") %>' />

        <input type="hidden" id="hdnAllEmp" value="<%= Me.Language.Translate("Criteria.AllEmployees",Defaultscope) %>" />
        <input type="hidden" id="hdnSomeEmps" value="<%= Me.Language.Translate("Criteria.SomeEmployees",Defaultscope) %>" />
        <input type="hidden" id="hdnNoEmp" value="<%= Me.Language.Translate("Criteria.NoEmployees",Defaultscope) %>" />

        <input type="hidden" id="hdnHdrZoneName" value="<%= Me.Language.Translate("GridFilter.hdnHdrZoneName",Defaultscope) %>" />
        <input type="hidden" id="hdnHdrEmployeeName" value="<%= Me.Language.Translate("GridFilter.hdnHdrEmployeeName",Defaultscope) %>" />
        <input type="hidden" id="hdnHdrDateTime" value="<%= Me.Language.Translate("GridFilter.hdnHdrDateTime",Defaultscope) %>" />
        <input type="hidden" id="hdnHdrIDCapture" value="<%= Me.Language.Translate("GridFilter.hdnHdrIDCapture",Defaultscope) %>" />

        <table width="100%" height="100%">
            <tr>
                <td>
                    <table>
                        <tr>
                            <td>
                                <img src="Images/AccessPunches.png" /></td>
                            <td>
                                <asp:Label ID="lblFilterDesc" runat="server" Text="Seleccione un criterio para visualizar los marcajes."></asp:Label>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td width="100%" height="100%" valign="top">
                    <table>
                        <tr>
                            <td style="padding-left: 10px;">
                                <table border="0">
                                    <tr>
                                        <td style="padding-left: 5px; padding-right: 5px;">
                                            <asp:Label ID="lblFEmployees" runat="server" CssClass="spanEmp-Class" Text="Empleado(s)"></asp:Label>
                                        </td>
                                        <td style="width: 300px; border: solid 1px #CCCCCC; display: block;" class="defaultBackgroundColor">
                                            <a href="javascript: void(0)" id="aFEmployees" runat="server" class="btnDDownMode" onmouseover="document.getElementById('divFloatMenuE').style.display='';" onmouseout="document.getElementById('divFloatMenuE').style.display='none';"></a>
                                            <div id="divFloatMenuE" class="floatMenu defaultBackgroundColor" style="display: none;" onmouseover="document.getElementById('divFloatMenuE').style.display='';" onmouseout="document.getElementById('divFloatMenuE').style.display='none';">
                                                <table border="0" style="width: 270px; margin-left: 10px; margin-right: 10px;">
                                                    <tr>
                                                        <td nowrap="nowrap"><a href="javascript: void(0)" id="aEmpAll" class="btnMode" onclick="ShowSelector('E',1);document.getElementById('divFloatMenuE').style.display='none';" style="width: 100%;">
                                                            <asp:Label ID="lblAllEmp" runat="server" Text="Todos los empleados"></asp:Label></a></td>
                                                    </tr>
                                                    <tr>
                                                        <td nowrap="nowrap"><a href="javascript: void(0)" id="aEmpSelect" class="btnMode" onclick="ShowSelector('E',2);document.getElementById('divFloatMenuE').style.display='none';" style="width: 100%;">
                                                            <asp:Label ID="lblEmpSelect" runat="server" Text="Seleccionar.."></asp:Label></a></td>
                                                    </tr>
                                                </table>
                                            </div>
                                        </td>
                                        <td style="padding-left: 10px;">
                                            <asp:Label ID="lblFDateBegin" runat="server" CssClass="spanEmp-Class" Text="Fecha inicio:"></asp:Label></td>
                                        <%--<td style="padding-left: 5px;"><input type="text" id="dpDateBegin" runat="server" style="width:75px;" class="textClass" ConvertControl="DatePicker" CCallowblank="false" /></td>--%>
                                        <td style="padding-left: 5px;">
                                            <dx:ASPxDateEdit runat="server" ID="dpDateBegin" Width="100px" ClientInstanceName="dpDateBeginClient">
                                                <CalendarProperties ShowClearButton="false" />
                                            </dx:ASPxDateEdit>
                                        </td>
                                        <td style="padding-left: 10px;">
                                            <asp:Label ID="lblFHourBegin" runat="server" CssClass="spanEmp-Class" Text="Hora inicio:"></asp:Label></td>
                                        <td style="padding-left: 5px;">
                                            <input type="text" cctime="true" runat="server" convertcontrol="TextField" style="width: 50px; text-align: right;" class="textClass x-form-text x-form-field" id="txtHourBegin" maxlength="5" ccallowblank="false" /></td>
                                    </tr>
                                    <tr>
                                        <td style="padding-left: 5px; padding-right: 5px;">
                                            <asp:Label ID="lblFZone" runat="server" Text="Zona(s)" CssClass="spanEmp-Class"></asp:Label>
                                        </td>
                                        <td style="width: 300px; border: solid 1px #CCCCCC; display: block;" class="defaultBackgroundColor">
                                            <a href="javascript: void(0)" id="aFZones" runat="server" class="btnDDownMode" style="background-image: none"></a>
                                            <div id="divFloatMenuZ" class="floatMenu defaultBackgroundColor" style="display: none;" onmouseover="document.getElementById('divFloatMenuZ').style.display='';" onmouseout="document.getElementById('divFloatMenuZ').style.display='none';">
                                                <table border="0" style="width: 270px; margin-left: 10px; margin-right: 10px;">
                                                    <tr>
                                                        <td nowrap="nowrap"><a href="javascript: void(0)" id="aZoneAll" class="btnMode" onclick="ShowSelector('Z',1);document.getElementById('divFloatMenuZ').style.display='none';" style="width: 100%;">
                                                            <asp:Label ID="lblAllZone" runat="server" Text="Todas las zonas"></asp:Label></a></td>
                                                    </tr>
                                                    <tr>
                                                        <td nowrap="nowrap"><a href="javascript: void(0)" id="aZoneSelect" class="btnMode" onclick="ShowSelector('Z',2);document.getElementById('divFloatMenuZ').style.display='none';" style="width: 100%;">
                                                            <asp:Label ID="lblZoneSelect" runat="server" Text="Seleccionar.."></asp:Label></a></td>
                                                    </tr>
                                                </table>
                                            </div>
                                        </td>
                                        <td style="padding-left: 10px;">
                                            <asp:Label ID="lblFDateEnd" runat="server" CssClass="spanEmp-Class" Text="Fecha final:"></asp:Label></td>
                                        <%--<td style="padding-left: 5px;"><input type="text" id="dpDateEnd" runat="server" style="width:75px;" class="textClass" ConvertControl="DatePicker" CCallowblank="false" /></td>--%>
                                        <td style="padding-left: 5px;">
                                            <dx:ASPxDateEdit runat="server" ID="dpDateEnd" Width="100px" ClientInstanceName="dpDateEndClient">
                                                <CalendarProperties ShowClearButton="false" />
                                            </dx:ASPxDateEdit>
                                        </td>
                                        <td style="padding-left: 10px;">
                                            <asp:Label ID="lblFHourEnd" runat="server" CssClass="spanEmp-Class" Text="Hora final:"></asp:Label></td>
                                        <td style="padding-left: 5px;">
                                            <input type="text" cctime="true" runat="server" convertcontrol="TextField" style="width: 50px; text-align: right;" class="textClass x-form-text x-form-field" id="txtHourEnd" maxlength="5" ccallowblank="false" /></td>
                                    </tr>
                                    <tr>
                                        <td colspan="6" align="right" style="padding-top: 10px; padding-right: 10px;">
                                            <table>
                                                <tr>
                                                    <td><a href="javascript: void(0);" class="icoRefresh" onclick="RefreshGridAccess();"></a></td>
                                                    <td>&nbsp;<a href="javascript: void(0);" onclick="RefreshGridAccess()"><asp:Label ID="lblBtnRefresh" runat="server" Text="[ Actualizar ]" Font-Bold="true"></asp:Label></a></td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td style="padding: 10px; padding-left: 25px;">
                                <!-- GRID ACCESSPUNCHES -->
                                <div style="border-bottom: solid 1px #CCCCCC;">
                                    <div id="grdAccessHeader"></div>
                                    <div id="grdAccess" style="height: 220px; overflow-y: scroll; border-left: solid 1px #cccccc; border-top: 0;"></div>
                                </div>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td width="100%" height="100%" align="right">
                    <table>
                        <tr>
                            <td>
                                <asp:Button ID="btClose" Text="${Button.Cancel}" runat="server" OnClientClick="Close(); return false;" CssClass="btnFlat btnFlatBlack btnFlatAsp" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>

        <asp:HiddenField ID="hdnCanClose_PageBase" runat="server" />
        <asp:HiddenField ID="hdnMustRefresh_PageBase" Value="0" runat="server" />
        <asp:HiddenField ID="hdnParams_PageBase" runat="server" />

        <roWebControls:roPopupFrameV2 ID="RoPopupFrame1" runat="server" ShowTitleBar="true" BehaviorID="RoPopupFrame1Behavior" CssClassPopupExtenderBackground="modalBackgroundTransparent">
            <FrameContentTemplate>
                <table cellpadding="0" cellspacing="0">
                    <tr>
                        <td>
                            <asp:Label ID="lblGroupSelection" Text="Selección ${Group}" runat="server" />
                        </td>
                        <td align="right">
                            <asp:ImageButton ID="btSelectorOk" runat="server" ImageUrl="~/Base/Images/ButtonOK_16.png" Style="cursor: pointer;" OnClientClick='SelectorAction(true); SelectorOk(); HideGroupSelector(true); return false;' />
                            <asp:ImageButton ID="btSelectorCancel" runat="server" ImageUrl="~/Base/Images/ButtonCancel_16.png" Style="cursor: pointer;" OnClientClick='HideGroupSelector(false); return false;' />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2" valign="top">
                            <asp:HiddenField ID="hdnIDGroupSelected" runat="server" Value="" />
                            <asp:HiddenField ID="hdnIDGroupSelectedName" runat="server" Value="" />
                            <iframe id="GroupSelectorFrame" runat="server" style="background-color: Transparent;" height="300" width="250"
                                scrolling="auto" frameborder="0" marginheight="0" marginwidth="0" src="" />
                        </td>
                    </tr>
                </table>
            </FrameContentTemplate>
        </roWebControls:roPopupFrameV2>
        <Local:MessageFrame ID="MessageFrame1" runat="server" />
    </form>
</body>
</html>