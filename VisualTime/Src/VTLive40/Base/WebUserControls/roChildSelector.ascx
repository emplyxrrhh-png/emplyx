<%@ Control Language="VB" AutoEventWireup="false" Inherits="VTLive40.Base_WebUserControls_roChildSelector" CodeBehind="roChildSelector.ascx.vb" %>

<input type="hidden" id="hdnAfterSelectFilterFuncion" value="" runat="server" />

<div class="fondo_tree">
    <div class="fondo_tree_toolbar">
        <!-- Icones Filtrat -->
        <table width="100%" height="16px" border="0">
            <tr>
                <td width="16px"><a id="icoFilt1" href="javascript: void(0)" class="icoFilter1 icoPressed" title="Empleados con contrato en vigor" runat="server" onclick=""></a></td>
                <td width="16px"><a id="icoFilt2" href="javascript: void(0)" class="icoFilter2 icoPressed" title="Empleados en transito" runat="server" onclick=""></a></td>
                <td width="16px"><a id="icoFilt3" href="javascript: void(0)" class="icoFilter3 icoPressed" title="Empleados de baja" runat="server" onclick=""></a></td>
                <td width="16px"><a id="icoFilt4" href="javascript: void(0)" class="icoFilter4 icoPressed" title="Altas futuras" runat="server" onclick=""></a></td>
                <td width="16px"><a id="icoFilt5" href="javascript: void(0)" class="icoFilter5 icoPressed" title="Filtrar por los campos de la ficha de empleados" runat="server" onclick=""></a></td>
                <td>&nbsp;</td>
                <td align="right" width="32px">
                    <table border="0" cellpadding="0" cellspacing="0" style="width: 32px;">
                        <tr>
                            <td style="width: 16px;">
                                <a id="icoFiltAdv" runat="server" href="javascript: void(0)" class="icoFilter icoClass" title="Filtro avanzado" onclick="" ></a>
                                <div id="divFiltreAvan_Float" style="margin-left: -10px; margin-top: -5px; position: relative; z-index: 9000; height: 200px; display: none;" runat="server"></div>
                            </td>
                            <td style="width: 16px;"><a href="javascript: void(0)" class="icoRefresh icoClass" title="<%= Me.Language.Translate("Button.Refresh","roSelector") %>" onclick="eval('<%= Me.m_TreeClientID %>_roTrees.reLoadGroupsFromDB();');eval('<%= Me.m_TreeClientID %>_roTrees.LoadTreeViews(true, true, true);');"></a></td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    </div>
    <div>
        <div id="divFiltreAvan" style="border: solid 1px silver; margin-left: 34px; margin-top: 2px; position: relative; z-index: 9000; height: auto; width: auto; background-color: White; display: none;" runat="server"></div>
        <asp:PlaceHolder ID="ContentTrees" runat="server" />
    </div>
</div>