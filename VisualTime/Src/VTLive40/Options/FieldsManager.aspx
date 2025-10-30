<%@ Page Language="VB" MasterPageFile="~/Base/mastEmp.master" AutoEventWireup="false" Inherits="VTLive40.FieldsManager" Title="Opciones de configuración" EnableEventValidation="True" EnableViewState="True" EnableSessionState="True" CodeBehind="FieldsManager.aspx.vb" %>

<asp:Content ID="cMainBody" ContentPlaceHolderID="contentMainBody" runat="Server">

    <script language="javascript" type="text/javascript">

        var _gridEmployeeUserFieldsNotUsed;
        var _gridEmployeeUserFieldsUsed;
        var _gridGroupUserFieldsNotUsed;
        var _gridGroupUserFieldsUsed;
        var _gridTaskFieldsNotUsed;
        var _gridBusinessCenterFieldsNotUsed;

        var _grdSystemEmployeeUserFields;

        function PageBase_Load() {

            ConvertControls();
            _gridEmployeeUserFieldsNotUsed = $get('__gv<%= Me.grdEmployeeUserFieldsNotUsed.ClientID %>__div');
            if (_gridEmployeeUserFieldsNotUsed == null) _gridEmployeeUserFieldsNotUsed = $get('<%= Me.grdEmployeeUserFieldsNotUsed.ClientID %>');
            _gridEmployeeUserFieldsUsed = $get('__gv<%= Me.grdEmployeeUserFieldsUsed.ClientID %>__div');
            if (_gridEmployeeUserFieldsUsed == null) _gridEmployeeUserFieldsUsed = $get('<%= Me.grdEmployeeUserFieldsUsed.ClientID %>');
            _grdSystemEmployeeUserFields = $get('__gv<%= Me.grdSystemEmployeeUserFields.ClientID%>__div');
            if (_grdSystemEmployeeUserFields == null) _grdSystemEmployeeUserFields = $get('<%= Me.grdSystemEmployeeUserFields.ClientID%>');

            _gridTaskFieldsNotUsed = $get('__gv<%= Me.grdTaskFieldsNotUsed.ClientID %>__div');
            if (_gridTaskFieldsNotUsed == null) _gridTaskFieldsNotUsed = $get('<%= Me.grdTaskFieldsNotUsed.ClientID %>');

            _gridBusinessCenterFieldsNotUsed = $get('__gv<%= Me.grdBusinessCenterFieldsNotUsed.ClientID%>__div');
            if (_gridBusinessCenterFieldsNotUsed == null) _gridBusinessCenterFieldsNotUsed = $get('<%= Me.grdBusinessCenterFieldsNotUsed.ClientID%>');

            _gridGroupUserFieldsNotUsed = $get('__gv<%= Me.grdGroupUserFieldsNotUsed.ClientID %>__div');
            if (_gridGroupUserFieldsNotUsed == null) _gridGroupUserFieldsNotUsed = $get('<%= Me.grdGroupUserFieldsNotUsed.ClientID %>');
            _gridGroupUserFieldsUsed = $get('__gv<%= Me.grdGroupUserFieldsUsed.ClientID %>__div');
            if (_gridGroupUserFieldsUsed == null) _gridGroupUserFieldsUsed = $get('<%= Me.grdGroupUserFieldsUsed.ClientID %>');

            $get('panEmployeeUserFieldsOptions').style.display = 'block';
            $get('panGroupUserFieldsOptions').style.display = 'none';
            $get('panTaskFieldsOptions').style.display = 'none';

            // Reestablezco el tab activo
            if ($get('<%= ConfigurationOptions_TabVisibleName.ClientID %>').value != '') {
                SelectTab($get('<%= ConfigurationOptions_TabVisibleName.ClientID %>').value);
            }
            else {
                SelectTab('panEmployeeUserFieldsOptions');
            }

            // Establecer selección actual en la grids
            var gridView = $get('<%= grdEmployeeUserFieldsNotUsed.ClientID %>');
            if (gridView != null) {
                var currow = $get('<%= hdnEmployeeUserFieldsNotUsedSelectedRowIndex.ClientID %>').value;
                var curcol = $get('<%= hdnEmployeeUserFieldsNotUsedSelectedColIndex.ClientID %>').value;
                if (currow != -1 && curcol != -1) {
                    roGridViewControl_Load(gridView, currow, curcol, 'row-select');
                }
            }

            gridView = $get('<%= grdEmployeeUserFieldsUsed.ClientID %>');
            if (gridView != null) {
                var currow = $get('<%= hdnEmployeeUserFieldsUsedSelectedRowIndex.ClientID %>').value;
                var curcol = $get('<%= hdnEmployeeUserFieldsUsedSelectedColIndex.ClientID %>').value;
                if (currow != -1 && curcol != -1) {
                    roGridViewControl_Load(gridView, currow, curcol, 'row-select');
                }
            }

            gridView = $get('<%= grdSystemEmployeeUserFields.ClientID%>');
            if (gridView != null) {
                var currow = $get('<%= hdnSystemEmployeeUserFieldsSelectedRowIndex.ClientID%>').value;
                var curcol = $get('<%= hdnSystemEmployeeUserFieldsSelectedColIndex.ClientID%>').value;
                if (currow != -1 && curcol != -1) {
                    roGridViewControl_Load(gridView, currow, curcol, 'row-select');
                }
            }

            gridView = $get('<%= grdGroupUserFieldsNotUsed.ClientID %>');
            if (gridView != null) {
                var currow = $get('<%= hdnGroupUserFieldsNotUsedSelectedRowIndex.ClientID %>').value;
                var curcol = $get('<%= hdnGroupUserFieldsNotUsedSelectedColIndex.ClientID %>').value;
                if (currow != -1 && curcol != -1) {
                    roGridViewControl_Load(gridView, currow, curcol, 'row-select');
                }
            }
            gridView = $get('<%= grdGroupUserFieldsUsed.ClientID %>');
            if (gridView != null) {
                var currow = $get('<%= hdnGroupUserFieldsUsedSelectedRowIndex.ClientID %>').value;
                var curcol = $get('<%= hdnGroupUserFieldsUsedSelectedColIndex.ClientID %>').value;
                if (currow != -1 && curcol != -1) {
                    roGridViewControl_Load(gridView, currow, curcol, 'row-select');
                }
            }

            gridView = $get('<%= grdTaskFieldsNotUsed.ClientID %>');
            if (gridView != null) {
                var currow = $get('<%= hdnTaskFieldsNotUsedSelectedRowIndex.ClientID %>').value;
                var curcol = $get('<%= hdnTaskFieldsNotUsedSelectedColIndex.ClientID %>').value;
                if (currow != -1 && curcol != -1) {
                    roGridViewControl_Load(gridView, currow, curcol, 'row-select');
                }
            }

            gridView = $get('<%= grdBusinessCenterFieldsNotUsed.ClientID%>');
            if (gridView != null) {
                var currow = $get('<%= hdnBusinessCenterFieldsNotUsedSelectedRowIndex.ClientID%>').value;
                var curcol = $get('<%= hdnBusinessCenterFieldsNotUsedSelectedColIndex.ClientID%>').value;
                if (currow != -1 && curcol != -1) {
                    roGridViewControl_Load(gridView, currow, curcol, 'row-select');
                }
            }

            checkOPCPanelClients();

            if ($get('<%= hdnChanged.ClientID %>').value == '0' || $get('<%= hdnChanged.ClientID %>').value == '') {
                hasChanges(false);
            } else {
                hasChanges(true);
            }

            top.focus();
        }

        function SelectTab(SelectedTab) {

            // Hacer invisibles los panels
            $get('panEmployeeUserFieldsOptions').style.display = 'none';
            $get('panGroupUserFieldsOptions').style.display = 'none';
            $get('panTaskFieldsOptions').style.display = 'none';
            $get('panBusinessCenterFieldsOptions').style.display = 'none';

            // Desmarcar los botones de la barra
            $get('<%= TABBUTTON_EmployeeUserFieldsOptions.ClientID %>').className = 'bTab';
            $get('<%= TABBUTTON_GroupUserFieldsOptions.ClientID %>').className = 'bTab';
            $get('<%= TABBUTTON_TaskFieldsOptions.ClientID %>').className = 'bTab';
            $get('<%= TABBUTTON_BusinessCenterFieldsOptions.ClientID%>').className = 'bTab';

            var TabID;
            if (SelectedTab == 'panEmployeeUserFieldsOptions') {
                TabID = 'panEmployeeUserFieldsOptions';
                $get('<%= TABBUTTON_EmployeeUserFieldsOptions.ClientID %>').className = 'bTab-active';
            } else if (SelectedTab == 'panGroupUserFieldsOptions') {
                TabID = 'panGroupUserFieldsOptions';
                $get('<%= TABBUTTON_GroupUserFieldsOptions.ClientID %>').className = 'bTab-active';
            } else if (SelectedTab == 'panTaskFieldsOptions') {
                TabID = 'panTaskFieldsOptions';
                $get('<%= TABBUTTON_TaskFieldsOptions.ClientID %>').className = 'bTab-active';
            } else if (SelectedTab == 'panBusinessCenterFieldsOptions') {
                TabID = 'panBusinessCenterFieldsOptions';
                $get('<%= TABBUTTON_BusinessCenterFieldsOptions.ClientID%>').className = 'bTab-active';

            }
            $get(TabID).style.display = 'block';
            $get('<%= ConfigurationOptions_TabVisibleName.ClientID %>').value = TabID;

        }

        function ResizeGrid(grid) {
            if (grid != null) {
                var _Height = (document.documentElement.clientHeight - _Top - 140);
                if (_Height > 0) {
                    grid.style.height = _Height + 'px';
                }
            }
        }

        function CreateUserField(type) {
            parent.ShowExternalForm2('Options/UserField.aspx?UserFieldName=&Type=' + type, 650, 450, '', '', false, false, false);
        }

        function EditTaskFieldNotUsed(type, rowIndex) {
            var grid;
            var hdnRow;

            grid = $get('<%= grdTaskFieldsNotUsed.ClientID %>');
            hdnRow = $get('<%= hdnTaskFieldsNotUsedSelectedRowIndex.ClientID %>');

            var currow = rowIndex;
            if (currow == null) currow = hdnRow.value;
            var gridRow = grid.rows[currow];
            if (null != gridRow) {

                var UF = "";
                try {
                    if (window.addEventListener) //Firefox
                        UF = gridRow.cells[1].childNodes[3].childNodes[0].nodeValue
                    else //IE
                        UF = gridRow.cells[1].childNodes[2].childNodes[0].nodeValue
                }
                catch (e) { UF = ""; }

                parent.ShowExternalForm2('Options/TaskField.aspx?TaskFieldID=' + currow, 650, 440, '', '', false, false, false);
            }
        }

        function EditBusinessCenterFieldNotUsed(type, rowIndex) {
            var grid;
            var hdnRow;

            grid = $get('<%= grdBusinessCenterFieldsNotUsed.ClientID%>');
            hdnRow = $get('<%= hdnBusinessCenterFieldsNotUsedSelectedRowIndex.ClientID%>');

            var currow = rowIndex;
            if (currow == null) currow = hdnRow.value;
            var gridRow = grid.rows[currow];
            if (null != gridRow) {

                var UF = "";
                try {
                    if (window.addEventListener) //Firefox
                        UF = gridRow.cells[1].childNodes[3].childNodes[0].nodeValue
                    else //IE
                        UF = gridRow.cells[1].childNodes[2].childNodes[0].nodeValue
                }
                catch (e) { UF = ""; }

                parent.ShowExternalForm2('Options/BusinessCenterField.aspx?BusinessCenterFieldID=' + currow, 650, 440, '', '', false, false, false);
            }
        }

        function EditUserFieldNotUsed(type, rowIndex) {
            var grid;
            var hdnRow;
            if (type == '<%= Robotics.Base.DTOs.UserFieldsTypes.Types.EmployeeField %>') {
                grid = $get('<%= grdEmployeeUserFieldsNotUsed.ClientID %>');
                hdnRow = $get('<%= hdnEmployeeUserFieldsNotUsedSelectedRowIndex.ClientID %>');
            }
            else {
                grid = $get('<%= grdGroupUserFieldsNotUsed.ClientID %>');
                hdnRow = $get('<%= hdnGroupUserFieldsNotUsedSelectedRowIndex.ClientID %>');
            }
            var currow = rowIndex;
            if (currow == null) currow = hdnRow.value;
            var gridRow = grid.rows[currow];
            if (null != gridRow) {

                var UF = "";
                try {
                    if (window.addEventListener) //Firefox
                        UF = gridRow.cells[1].childNodes[3].childNodes[0].nodeValue
                    else //IE
                        UF = gridRow.cells[1].childNodes[2].childNodes[0].nodeValue
                }
                catch (e) { UF = ""; }

                parent.ShowExternalForm2('Options/UserField.aspx?UserFieldName=' + encodeURIComponent(UF) + '&Type=' + type, 650, 450, '', '', false, false, false);
            }
        }
        function EditUserFieldUsed(type, rowIndex) {
            var grid;
            var hdnRow;
            if (type == '<%= Robotics.Base.DTOs.UserFieldsTypes.Types.EmployeeField %>') {
                grid = $get('<%= grdEmployeeUserFieldsUsed.ClientID %>');
                hdnRow = $get('<%= hdnEmployeeUserFieldsUsedSelectedRowIndex.ClientID %>');
            }
            else {
                grid = $get('<%= grdGroupUserFieldsUsed.ClientID %>');
                hdnRow = $get('<%= hdnGroupUserFieldsUsedSelectedRowIndex.ClientID %>');
            }
            var currow = rowIndex;
            if (currow == null) currow = hdnRow.value;
            var gridRow = grid.rows[currow];
            if (null != gridRow) {

                var UF = "";
                try {
                    if (window.addEventListener) //Firefox
                        UF = gridRow.cells[1].childNodes[3].childNodes[0].nodeValue
                    else //IE
                        UF = gridRow.cells[1].childNodes[2].childNodes[0].nodeValue
                }
                catch (e) { UF = ""; }

                parent.ShowExternalForm2('Options/UserField.aspx?UserFieldName=' + encodeURIComponent(UF) + '&Type=' + type, 650, 450, '', '', false, false, false);
            }
        }

        function EditSystemUserField(type, rowIndex) {
            var grid;
            var hdnRow;
            if (type == '<%= Robotics.Base.DTOs.UserFieldsTypes.Types.EmployeeField %>') {
                grid = $get('<%= grdSystemEmployeeUserFields.ClientID%>');
                hdnRow = $get('<%= hdnSystemEmployeeUserFieldsSelectedRowIndex.ClientID%>');

                var currow = rowIndex;
                if (currow == null) currow = hdnRow.value;
                var gridRow = grid.rows[currow];
                if (null != gridRow) {

                    var UF = "";
                    try {
                        if (window.addEventListener) //Firefox
                            UF = gridRow.cells[1].childNodes[3].childNodes[0].nodeValue
                        else //IE
                            UF = gridRow.cells[1].childNodes[2].childNodes[0].nodeValue
                    }
                    catch (e) { UF = ""; }

                    parent.ShowExternalForm2('Options/UserField.aspx?UserFieldName=' + encodeURIComponent(UF) + '&Type=' + type, 650, 450, '', '', false, false, false);
                }
            }

        }

        function RefreshScreen(DataType) {
            ButtonClick($get('<%= btRefresh.ClientID %>'));
        }

        function checkOPCPanelClients() {

        }

        //Llença aquest script al recarregar els updatepanels per poder controlar per js els opclient
        function endRequestHandler() {
            checkOPCPanelClients();
        }

        function saveChanges() {
            try {
                if (CheckSave() == true) {
                    showLoadingGrid(true);
                    __doPostBack('<%= btSave.ClientID %>');
                }

            } catch (e) { showError("saveChanges", e); }
        }

        function undoChanges() {
            try {
                __doPostBack('<%= btCancel.ClientID %>');
            } catch (e) { showError("undoChanges", e); }
        }

        function RemoveEmployeeUserField(rowIndex) {

            try {
                __doPostBack('<%= btRemoveEmployeeUserField.ClientID %>##' + rowIndex);

            } catch (e) { showError("RemoveEmployeeUserField", e); }

        }

        function RemoveGroupUserField(rowIndex) {

            try {
                __doPostBack('<%= btRemoveGroupUserField.ClientID %>##' + rowIndex);

            } catch (e) { showError("RemoveGroupUserField", e); }

        }
    </script>

    <input type="hidden" runat="server" id="hdnModeEdit" value="" />
    <input type="hidden" id="msgHasChanges" value="" runat="server" clientidmode="Static" />
    <input type="hidden" id="msgHasErrors" value="" runat="server" clientidmode="Static" />

    <div id="divMainBody">
        <!-- TAB SUPERIOR -->
        <div id="divTabInfo" class="divDataCells">
            <div style="min-height: 10px"></div>
            <div id="divStartRibbon" class="blackRibbonTitle" style="">
                <div class="blackRibbonIcon">
                    <asp:Image ID="imgConfigurationOptions" ImageUrl="Images/FieldManagement.png" runat="server" />
                </div>
                <div class="blackRibbonDescription">
                    <asp:Label ID="lblHeader" runat="server" Text="Campos de la ficha" CssClass="NameText"></asp:Label>
                    <br />
                    <asp:Label ID="lblInfo" runat="server" Text="Esta es la pantalla puede configurar los campo de la ficha disponibles tanto para empleados, empreasas o tareas."></asp:Label>
                </div>
                <div class="blackRibbonButtons" style="">
                    <table border="0" cellpadding="0" cellspacing="0" width="100%" style="padding: 2px 5px 5px 5px;">
                        <tr>
                            <td style="width: 100px;" valign="middle"></td>
                            <td valign="top" style="padding-top: 10px; padding-bottom: 20px;"></td>
                            <td id="rowTabButtons1" runat="server" align="right" valign="top" style="width: 140px; padding-top: 0px; padding-right: 1px;">
                                <a id="TABBUTTON_EmployeeUserFieldsOptions" href="javascript: void(0);" class="bTab" onclick="SelectTab('panEmployeeUserFieldsOptions');" runat="server">
                                    <asp:Label ID="lblEmployeeUserFieldsOptionsTabButton" Text="Ficha ${Employee}" runat="server" /></a>
                                <a id="TABBUTTON_GroupUserFieldsOptions" href="javascript: void(0);" class="bTab" onclick="SelectTab('panGroupUserFieldsOptions');" runat="server">
                                    <asp:Label ID="lblGroupUserFieldsOptionsTabButton" Text="Ficha empresa" runat="server" /></a>
                            </td>
                            <td id="rowTabButtons2" runat="server" align="right" valign="top" style="width: 140px; padding-top: 0px; padding-right: 1px;">
                                <a id="TABBUTTON_TaskFieldsOptions" href="javascript: void(0);" class="bTab" onclick="SelectTab('panTaskFieldsOptions');" runat="server">
                                    <asp:Label ID="lblTaskFieldsOptionsTabButton" Text="Ficha ${Task}" runat="server" /></a>
                                <a id="TABBUTTON_BusinessCenterFieldsOptions" href="javascript: void(0);" class="bTab" onclick="SelectTab('panBusinessCenterFieldsOptions');" runat="server">
                                    <asp:Label ID="lblBusinessCenterFieldsOptionsTabButton" Text="Ficha ${BusinessCenter}" runat="server" /></a>
                            </td>
                        </tr>
                    </table>
                </div>
            </div>
            <div style="min-height: 10px"></div>
        </div>
        <!-- FIN TAB SUPERIOR -->
        <!-- DETALLE -->
        <div id="divTabData" class="divDataCells">
            <div id="divContenido" class="divAllContent">
                <div id="divContent" style="height: initial;" class="maxHeight">
                    <asp:UpdatePanel ID="upBody" runat="server">
                        <ContentTemplate>
                            <div style="margin: 5px;">
                                <table cellpadding="0" cellspacing="0" style="width: 100%; height: 100%; padding-left: 10px; padding-right: 10px;">
                                    <tr>
                                        <td valign="top" style="padding-top: 2px;">
                                            <!-- Mensajes -->
                                            <div id="divMsgTop" class="divMsg" style="display: none;">
                                                <table style="width: 100%;">
                                                    <tr>
                                                        <td align="center" style="width: 20px; height: 16px;">
                                                            <img id="Img1" src="~/Base/Images/MessageFrame/Alert16.png" runat="server" /></td>
                                                        <td align="left" style="padding-left: 10px; color: white;"><span id="msgTop"></span></td>
                                                        <td align="right" style="color: White; padding-right: 10px;">
                                                            <a href="javascript: void(0);" class="aMsg" onclick="saveChanges();"><span id="lblSaveChanges" runat="server" /></a>
                                                            &nbsp;&nbsp;&nbsp;|&nbsp;&nbsp;&nbsp;
                                                            <a href="javascript: void(0);" onclick="undoChanges();" class="aMsg"><span id="lblUndoChanges" runat="server" /></a>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </div>
                                        </td>
                                    </tr>

                                    <tr>
                                        <td valign="top">

                                            <div id="panEmployeeUserFieldsOptions" style="width: 100%; height: 100%; display: none; vertical-align: top;">
                                                <table id="tbEmployeeUserFieldsOptions" runat="server" cellpadding="0" cellspacing="0" style="width: 100%;">
                                                    <tr>
                                                        <td>
                                                            <div class="panHeader2">
                                                                <span style="">
                                                                    <asp:Label ID="lblEmployeeUserFieldsOptionsHeader" runat="server" Text="Personalización de la ficha del ${Employee}" /></span>
                                                            </div>
                                                            <br />
                                                        </td>
                                                    </tr>
                                                    <tr style="padding-top: 5px">
                                                        <td style="padding-left: 20px">
                                                            <asp:Label ID="lblEmployeeUserFieldsOptionsInfo" Text="Este formulario le permite crear nuevos campos que necesite definir para los ${Employees}. Puede crear, editar, borrar campos y seleccionar cuales desea ver." CssClass="editTextFormat" runat="server"></asp:Label>
                                                            <br />
                                                            <br />
                                                        </td>
                                                    </tr>
                                                    <tr id="Tr1" style="padding-top: 20px; padding-bottom: 20px; vertical-align: top">
                                                        <td style="padding-left: 20px">

                                                            <asp:ObjectDataSource ID="BlankData" runat="server" SelectMethod="_Select" TypeName="BlankDataObject" />
                                                            <table cellpadding="0" cellspacing="0" width="100%">
                                                                <tr>
                                                                    <td>

                                                                        <table cellpadding="0" cellspacing="0" width="100%">
                                                                            <tr>
                                                                                <td>
                                                                                    <asp:Label ID="lblEmployeeUserFieldsNotUsed" Text="Campos disponibles" Font-Bold="true" runat="server" />
                                                                                </td>
                                                                                <td align="right" style="">
                                                                                    <div id="tbAddEmployeeUserFieldNotUsed" runat="server" class="btnFlat">
                                                                                        <a href="javascript: void(0)" id="btAddEmployeeUserFieldNotUsed" onclick="CreateUserField('<%= Robotics.Base.DTOs.UserFieldsTypes.Types.EmployeeField %>');">
                                                                                            <span class="btnIconAdd"></span>
                                                                                            <asp:Label ID="lblAddEmployeeUserFieldNotUsed" Text="Añadir" runat="server" /></a>
                                                                                        </a>
                                                                                    </div>
                                                                                </td>
                                                                            </tr>
                                                                        </table>
                                                                    </td>
                                                                    <td></td>
                                                                    <td>
                                                                        <asp:Label ID="lblEmployeeUserFieldsUsed" Text="Campos seleccionados" Font-Bold="true" runat="server" />
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td style="width: 40%">
                                                                        <roWebControls:roGridViewControl ID="grdEmployeeUserFieldsNotUsed" runat="server" AutoGenerateColumns="false" DataSourceID="BlankData"
                                                                            GridLines="None" CellPadding="4"
                                                                            ShowFooter="False" AllowPaging="false" PageSize="7"
                                                                            CssClass="yui-datatable-small-theme"
                                                                            Width="100%" ScrollWidth="100%" Height="210px" FreezeHeader="true" Scrolling="Auto"
                                                                            CellHoverCssClass="row-over" CellSelectCssClass="row-select"
                                                                            RowSelectedControlID='<%# hdnEmployeeUserFieldsNotUsedSelectedRowIndex.ClientID%>'
                                                                            ColSelectedControlID='<%# hdnEmployeeUserFieldsNotUsedSelectedColIndex.ClientID%>'>
                                                                            <RowStyle CssClass="data-row" />
                                                                            <AlternatingRowStyle CssClass="alt-data-row" />
                                                                            <Columns>
                                                                                <asp:ButtonField Text="" CommandName="EditClick" Visible="False" />
                                                                                <asp:ButtonField Text="" CommandName="RemoveClick" Visible="false" />
                                                                                <asp:TemplateField>
                                                                                    <ItemTemplate>
                                                                                        <div class="fix-data-row-div">
                                                                                            <div class="fix-data-row-div-left">
                                                                                                <%--<asp:Image ID="Select_Image" ImageUrl="../Images/Transparencia.gif" Height="8" Width="10" runat="server"  />--%>
                                                                                                <img id="imgEdit" src="~/Base/Images/Grid/edit.png" visible="true" runat="server" title="Editar" />
                                                                                                <img id="imgEditAccept" src="~/Base/Images/Grid/save.png" visible="false" runat="server" />
                                                                                            </div>
                                                                                            <div class="fix-data-row-div-right">
                                                                                                <img id="imgRemove" src="~/Base/Images/Grid/remove.png" visible="true" runat="server" title="Eliminar" />
                                                                                                <img id="imgEditCancel" src="~/Base/Images/Grid/cancel.png" visible="false" runat="server" />
                                                                                            </div>
                                                                                        </div>
                                                                                    </ItemTemplate>
                                                                                    <ItemStyle Width="50px" HorizontalAlign="Center" VerticalAlign="Middle" CssClass="fix-data-row" />
                                                                                </asp:TemplateField>
                                                                                <asp:TemplateField HeaderText="Nombre">
                                                                                    <ItemTemplate>
                                                                                        <asp:Image ID="EmployeeUserFieldNotUsed_Image" ImageUrl="Images/UserFields 32.gif" Height="12" Width="12" runat="server" />
                                                                                        <asp:Label ID="EmployeeUserFieldNotUsed_Label" Text='<%# Eval("FieldName") %>' Visible="true" runat="server" Width="150" CssClass="yui-datatable-moves-theme" />
                                                                                    </ItemTemplate>
                                                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                                </asp:TemplateField>
                                                                                <asp:TemplateField HeaderText="Tipo">
                                                                                    <ItemTemplate>
                                                                                        <asp:TextBox ID="EmployeeUserFieldNotUsed_TextBox" Visible="true" TextMode="SingleLine" Rows="1" Width="60" runat="server" ReadOnly="true" CssClass="yui-datatable-moves-theme"
                                                                                            Text='<%# FieldTypeName(Eval("FieldType")) %>'
                                                                                            Style="border: none; background-color: Transparent;" />
                                                                                    </ItemTemplate>
                                                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                                </asp:TemplateField>
                                                                                <asp:TemplateField HeaderText="Nivel acceso">
                                                                                    <ItemTemplate>
                                                                                        <asp:TextBox ID="EmployeeAccessLeveldNotUsed_TextBox" Visible="true" TextMode="SingleLine" Rows="1" Width="60" runat="server" ReadOnly="true" CssClass="yui-datatable-moves-theme"
                                                                                            Text='<%# AccessLevelName(Eval("AccessLevel")) %>'
                                                                                            Style="border: none; background-color: Transparent;" />
                                                                                    </ItemTemplate>
                                                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                                </asp:TemplateField>
                                                                                <asp:TemplateField HeaderText="Único">
                                                                                    <ItemTemplate>
                                                                                        <asp:CheckBox ID="EmployeeUniquedNotUsed_TextBox" Visible="true" Width="30" runat="server" Enabled="false" ReadOnly="true" CssClass="yui-datatable-moves-theme"
                                                                                            Checked='<%# FieldUnique(Eval("Unique")) %>' Style="border: none; background-color: Transparent;" />
                                                                                    </ItemTemplate>
                                                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                                </asp:TemplateField>
                                                                            </Columns>
                                                                        </roWebControls:roGridViewControl>
                                                                        <asp:HiddenField ID="hdnEmployeeUserFieldsNotUsedSelectedRowIndex" runat="server" Value="-1" />
                                                                        <asp:HiddenField ID="hdnEmployeeUserFieldsNotUsedSelectedColIndex" runat="server" Value="-1" />
                                                                    </td>
                                                                    <td align="center" valign="top" style="padding-top: 10px; padding-right: 10px; width: 10%;">
                                                                        <asp:ImageButton ID="ibtEmployeeUserFieldAsign" ImageUrl="Images/UserFields Asign 32.gif" ToolTip="Agregar campo personalizado seleccionado." Width="32" Height="32" runat="server" />
                                                                        <br />
                                                                        <asp:ImageButton ID="ibtEmployeeUserFieldRemove" ImageUrl="Images/UserFields Remove 32.gif" ToolTip="Quitar campo personalizado seleccionado." Width="32" Height="32" runat="server" />
                                                                    </td>
                                                                    <td style="width: 40%;">
                                                                        <roWebControls:roGridViewControl ID="grdEmployeeUserFieldsUsed" runat="server" AutoGenerateColumns="false" DataSourceID="BlankData"
                                                                            GridLines="None" CellPadding="4"
                                                                            ShowFooter="False" AllowPaging="false" PageSize="7"
                                                                            CssClass="yui-datatable-small-theme"
                                                                            Width="100%" ScrollWidth="100%" Height="210px" FreezeHeader="true" Scrolling="Auto"
                                                                            CellHoverCssClass="row-over" CellSelectCssClass="row-select"
                                                                            RowSelectedControlID='<%# hdnEmployeeUserFieldsUsedSelectedRowIndex.ClientID%>'
                                                                            ColSelectedControlID='<%# hdnEmployeeUserFieldsUsedSelectedColIndex.ClientID%>'>
                                                                            <RowStyle CssClass="data-row" />
                                                                            <AlternatingRowStyle CssClass="alt-data-row" />
                                                                            <Columns>
                                                                                <asp:ButtonField Text="" CommandName="EditClick" Visible="False" />
                                                                                <asp:ButtonField Text="" CommandName="RemoveClick" Visible="false" />
                                                                                <asp:TemplateField>
                                                                                    <ItemTemplate>
                                                                                        <div class="fix-data-row-div" style="width: 20px;">
                                                                                            <div class="fix-data-row-div-left">
                                                                                                <%--<asp:Image ID="Select_Image" ImageUrl="../Images/Transparencia.gif" Height="8" Width="10" runat="server"  />--%>
                                                                                                <img id="imgEdit2" src="~/Base/Images/Grid/edit.png" visible="true" runat="server" title="Editar" />
                                                                                                <img id="imgEditAccept2" src="~/Base/Images/Grid/save.png" visible="false" runat="server" />
                                                                                            </div>
                                                                                            <div class="fix-data-row-div-right">
                                                                                                <img id="imgRemove2" src="~/Base/Images/Grid/remove.png" visible="true" runat="server" title="Eliminar" />
                                                                                                <img id="imgEditCancel2" src="~/Base/Images/Grid/cancel.png" visible="false" runat="server" />
                                                                                            </div>
                                                                                        </div>
                                                                                    </ItemTemplate>
                                                                                    <ItemStyle Width="20px" HorizontalAlign="Center" VerticalAlign="Middle" CssClass="fix-data-row" />
                                                                                </asp:TemplateField>
                                                                                <asp:TemplateField HeaderText="Nombre">
                                                                                    <ItemTemplate>
                                                                                        <asp:Image ID="EmployeeUserFieldUsed_Image" ImageUrl="Images/UserFields 32.gif" Height="12" Width="12" runat="server" />
                                                                                        <asp:Label ID="EmployeeUserFieldUsed_Label" Text='<%# Eval("FieldName") %>' Visible="true" runat="server" Width="150" CssClass="yui-datatable-moves-theme" />
                                                                                    </ItemTemplate>
                                                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                                </asp:TemplateField>
                                                                                <asp:TemplateField HeaderText="Tipo">
                                                                                    <ItemTemplate>
                                                                                        <asp:TextBox ID="EmployeeUserFieldUsed_TextBox" Visible="true" TextMode="SingleLine" Rows="1" Width="60" runat="server" ReadOnly="true" CssClass="yui-datatable-moves-theme"
                                                                                            Text='<%# FieldTypeName(Eval("FieldType")) %>'
                                                                                            Style="border: none; background-color: Transparent;" />
                                                                                    </ItemTemplate>
                                                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                                </asp:TemplateField>
                                                                                <asp:TemplateField HeaderText="Nivel acceso">
                                                                                    <ItemTemplate>
                                                                                        <asp:TextBox ID="EmployeeAccessLeveldUsed_TextBox" Visible="true" TextMode="SingleLine" Rows="1" Width="60" runat="server" ReadOnly="true" CssClass="yui-datatable-moves-theme"
                                                                                            Text='<%# AccessLevelName(Eval("AccessLevel")) %>'
                                                                                            Style="border: none; background-color: Transparent;" />
                                                                                    </ItemTemplate>
                                                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                                </asp:TemplateField>
                                                                                <asp:TemplateField HeaderText="Único">
                                                                                    <ItemTemplate>
                                                                                        <asp:CheckBox ID="EmployeeUniquedUsed_Checkbox" Visible="true" Width="30" runat="server" Enabled="false" ReadOnly="true" CssClass="yui-datatable-moves-theme"
                                                                                            Checked='<%# Eval("Unique") %>' Style="border: none; background-color: Transparent;" />
                                                                                    </ItemTemplate>
                                                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                                </asp:TemplateField>
                                                                            </Columns>
                                                                        </roWebControls:roGridViewControl>
                                                                        <asp:HiddenField ID="hdnEmployeeUserFieldsUsedSelectedRowIndex" runat="server" Value="-1" />
                                                                        <asp:HiddenField ID="hdnEmployeeUserFieldsUsedSelectedColIndex" runat="server" Value="-1" />
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td style="height: 10px;">&nbsp;</td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            <div class="panHeader2">
                                                                <span style="">
                                                                    <asp:Label ID="lblSystemFieldsHeader" runat="server" Text="Campos de la ficha de sistema" /></span>
                                                            </div>
                                                            <br />
                                                        </td>
                                                    </tr>
                                                    <tr id="Tr3" style="padding-top: 20px; padding-bottom: 20px; vertical-align: top">
                                                        <td style="padding-left: 20px">
                                                            <table cellpadding="0" cellspacing="0" width="100%">
                                                                <tr>
                                                                    <td>
                                                                        <asp:Label ID="lblSystemFields" Text="Campos disponibles" Font-Bold="true" runat="server" />
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td style="width: 40%;">
                                                                        <roWebControls:roGridViewControl ID="grdSystemEmployeeUserFields" runat="server" AutoGenerateColumns="false" DataSourceID="BlankData"
                                                                            GridLines="None" CellPadding="4"
                                                                            ShowFooter="False" AllowPaging="false" PageSize="7"
                                                                            CssClass="yui-datatable-small-theme"
                                                                            Width="100%" ScrollWidth="100%" Height="210px" FreezeHeader="true" Scrolling="Auto"
                                                                            CellHoverCssClass="row-over" CellSelectCssClass="row-select"
                                                                            RowSelectedControlID='<%# hdnSystemEmployeeUserFieldsSelectedRowIndex.ClientID%>'
                                                                            ColSelectedControlID='<%# hdnSystemEmployeeUserFieldsSelectedColIndex.ClientID%>'>
                                                                            <RowStyle CssClass="data-row" />
                                                                            <AlternatingRowStyle CssClass="alt-data-row" />
                                                                            <Columns>
                                                                                <asp:ButtonField Text="" CommandName="EditClick" Visible="False" />
                                                                                <asp:ButtonField Text="" CommandName="RemoveClick" Visible="false" />
                                                                                <asp:TemplateField>
                                                                                    <ItemTemplate>
                                                                                        <div class="fix-data-row-div" style="width: 20px;">
                                                                                            <div class="fix-data-row-div-left">
                                                                                                <%--<asp:Image ID="Select_Image" ImageUrl="../Images/Transparencia.gif" Height="8" Width="10" runat="server"  />--%>
                                                                                                <img id="imgEdit2" src="~/Base/Images/Grid/edit.png" visible="true" runat="server" title="Editar" />
                                                                                                <img id="imgEditAccept2" src="~/Base/Images/Grid/save.png" visible="false" runat="server" />
                                                                                            </div>
                                                                                            <div class="fix-data-row-div-right">
                                                                                                <img id="imgRemove2" src="~/Base/Images/Grid/remove.png" visible="true" runat="server" title="Eliminar" />
                                                                                                <img id="imgEditCancel2" src="~/Base/Images/Grid/cancel.png" visible="false" runat="server" />
                                                                                            </div>
                                                                                        </div>
                                                                                    </ItemTemplate>
                                                                                    <ItemStyle Width="20px" HorizontalAlign="Center" VerticalAlign="Middle" CssClass="fix-data-row" />
                                                                                </asp:TemplateField>
                                                                                <asp:TemplateField HeaderText="Nombre">
                                                                                    <ItemTemplate>
                                                                                        <asp:Image ID="EmployeeUserFieldUsed_Image" ImageUrl="Images/UserFields 32.gif" Height="12" Width="12" runat="server" />
                                                                                        <asp:Label ID="EmployeeUserFieldUsed_Label" Text='<%# Eval("FieldName") %>' Visible="true" runat="server" Width="150" CssClass="yui-datatable-moves-theme" />
                                                                                    </ItemTemplate>
                                                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                                </asp:TemplateField>
                                                                                <asp:TemplateField HeaderText="Tipo">
                                                                                    <ItemTemplate>
                                                                                        <asp:TextBox ID="EmployeeUserFieldUsed_TextBox" Visible="true" TextMode="SingleLine" Rows="1" Width="60" runat="server" ReadOnly="true" CssClass="yui-datatable-moves-theme"
                                                                                            Text='<%# FieldTypeName(Eval("FieldType")) %>'
                                                                                            Style="border: none; background-color: Transparent;" />
                                                                                    </ItemTemplate>
                                                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                                </asp:TemplateField>
                                                                                <asp:TemplateField HeaderText="Nivel acceso">
                                                                                    <ItemTemplate>
                                                                                        <asp:TextBox ID="EmployeeAccessLeveldUsed_TextBox" Visible="true" TextMode="SingleLine" Rows="1" Width="60" runat="server" ReadOnly="true" CssClass="yui-datatable-moves-theme"
                                                                                            Text='<%# AccessLevelName(Eval("AccessLevel")) %>'
                                                                                            Style="border: none; background-color: Transparent;" />
                                                                                    </ItemTemplate>
                                                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                                </asp:TemplateField>
                                                                                <asp:TemplateField HeaderText="Único">
                                                                                    <ItemTemplate>
                                                                                        <asp:CheckBox ID="EmployeeUniqueSystem_Checkbox" Visible="true" Width="30" runat="server" Enabled="false" ReadOnly="true" CssClass="yui-datatable-moves-theme"
                                                                                            Checked='<%# Eval("Unique") %>' Style="border: none; background-color: Transparent;" />
                                                                                    </ItemTemplate>
                                                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                                </asp:TemplateField>
                                                                            </Columns>
                                                                        </roWebControls:roGridViewControl>
                                                                        <asp:HiddenField ID="hdnSystemEmployeeUserFieldsSelectedRowIndex" runat="server" Value="-1" />
                                                                        <asp:HiddenField ID="hdnSystemEmployeeUserFieldsSelectedColIndex" runat="server" Value="-1" />
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </div>

                                            <div id="panGroupUserFieldsOptions" style="width: 100%; height: 100%; display: none; vertical-align: top;">
                                                <table id="tbGroupUserFieldsOptions" runat="server" cellpadding="0" cellspacing="0" style="width: 100%;">
                                                    <tr>
                                                        <td>
                                                            <div class="panHeader2">
                                                                <span style="">
                                                                    <asp:Label ID="lblGroupUserFieldsOptionsHeader" runat="server" Text="Personalización de la ficha de la empresa" /></span>
                                                            </div>
                                                            <br />
                                                        </td>
                                                    </tr>
                                                    <tr style="padding-top: 5px">
                                                        <td style="padding-left: 20px">
                                                            <asp:Label ID="lblGroupUserFieldsOptionsInfo" Text="Este formulario le permite crear nuevos campos que necesite definir para las empresas.
                                                                                                            Puede crear, editar, borrar campos y seleccionar cuales desea ver."
                                                                CssClass="editTextFormat" runat="server"></asp:Label>
                                                            <br />
                                                            <br />
                                                        </td>
                                                    </tr>
                                                    <tr id="Tr2" style="padding-top: 20px; padding-bottom: 20px; vertical-align: top">
                                                        <td style="padding-left: 20px">

                                                            <table cellpadding="0" cellspacing="0" width="100%">
                                                                <tr>
                                                                    <td>

                                                                        <table cellpadding="0" cellspacing="0" width="100%">
                                                                            <tr>
                                                                                <td>
                                                                                    <asp:Label ID="lblGroupUserFieldsNotUsed" Text="Campos disponibles" Font-Bold="true" runat="server" />
                                                                                </td>
                                                                                <td align="right" style="">
                                                                                    <div id="tbAddGroupUserFieldNotUsed" runat="server" class="btnFlat">
                                                                                        <a href="javascript: void(0)" id="btAddGroupUserFieldNotUsed" onclick="CreateUserField('<%= Robotics.Base.DTOs.UserFieldsTypes.Types.GroupField%>');">
                                                                                            <span class="btnIconAdd"></span>
                                                                                            <asp:Label ID="lblAddGroupUserFieldNotUsed" Text="Añadir" runat="server" /></a>
                                                                                        </a>
                                                                                    </div>
                                                                                </td>
                                                                            </tr>
                                                                        </table>
                                                                    </td>
                                                                    <td></td>
                                                                    <td>
                                                                        <asp:Label ID="lblGroupUserFieldsUsed" Text="Campos seleccionados" Font-Bold="true" runat="server" />
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td style="width: 40%">
                                                                        <roWebControls:roGridViewControl ID="grdGroupUserFieldsNotUsed" runat="server" AutoGenerateColumns="false" DataSourceID="BlankData"
                                                                            GridLines="None" CellPadding="4"
                                                                            ShowFooter="False" AllowPaging="false" PageSize="7"
                                                                            CssClass="yui-datatable-small-theme"
                                                                            Width="100%" ScrollWidth="100%" Height="210px" FreezeHeader="true" Scrolling="Auto"
                                                                            CellHoverCssClass="row-over" CellSelectCssClass="row-select"
                                                                            RowSelectedControlID='<%# hdnGroupUserFieldsNotUsedSelectedRowIndex.ClientID%>'
                                                                            ColSelectedControlID='<%# hdnGroupUserFieldsNotUsedSelectedColIndex.ClientID%>'>
                                                                            <RowStyle CssClass="data-row" />
                                                                            <AlternatingRowStyle CssClass="alt-data-row" />
                                                                            <Columns>
                                                                                <asp:ButtonField Text="" CommandName="EditClick" Visible="False" />
                                                                                <asp:ButtonField Text="" CommandName="RemoveClick" Visible="false" />
                                                                                <asp:TemplateField>
                                                                                    <ItemTemplate>
                                                                                        <div class="fix-data-row-div">
                                                                                            <div class="fix-data-row-div-left">
                                                                                                <%--<asp:Image ID="Select_Image" ImageUrl="../Images/Transparencia.gif" Height="8" Width="10" runat="server"  />--%>
                                                                                                <img id="imgEdit3" src="~/Base/Images/Grid/edit.png" visible="true" runat="server" title="Editar" />
                                                                                                <img id="imgEditAccept3" src="~/Base/Images/Grid/save.png" visible="false" runat="server" />
                                                                                            </div>
                                                                                            <div class="fix-data-row-div-right">
                                                                                                <img id="imgRemove3" src="~/Base/Images/Grid/remove.png" visible="true" runat="server" title="Eliminar" />
                                                                                                <img id="imgEditCancel3" src="~/Base/Images/Grid/cancel.png" visible="false" runat="server" />
                                                                                            </div>
                                                                                        </div>
                                                                                    </ItemTemplate>
                                                                                    <ItemStyle Width="50px" HorizontalAlign="Center" VerticalAlign="Middle" CssClass="fix-data-row" />
                                                                                </asp:TemplateField>
                                                                                <asp:TemplateField HeaderText="Nombre">
                                                                                    <ItemTemplate>
                                                                                        <asp:Image ID="GroupUserFieldNotUsed_Image" ImageUrl="Images/UserFields 32.gif" Height="12" Width="12" runat="server" />
                                                                                        <asp:Label ID="GroupUserFieldNotUsed_Label" Text='<%# Eval("FieldName") %>' Visible="true" runat="server" Width="150" CssClass="yui-datatable-moves-theme" />
                                                                                    </ItemTemplate>
                                                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                                </asp:TemplateField>
                                                                                <asp:TemplateField HeaderText="Tipo">
                                                                                    <ItemTemplate>
                                                                                        <asp:TextBox ID="GroupUserFieldNotUsed_TextBox" Visible="true" TextMode="SingleLine" Rows="1" Width="60" runat="server" ReadOnly="true" CssClass="yui-datatable-moves-theme"
                                                                                            Text='<%# FieldTypeName(Eval("FieldType")) %>'
                                                                                            Style="border: none; background-color: Transparent;" />
                                                                                    </ItemTemplate>
                                                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                                </asp:TemplateField>
                                                                                <asp:TemplateField HeaderText="Nivel acceso">
                                                                                    <ItemTemplate>
                                                                                        <asp:TextBox ID="GroupAccessLeveldNotUsed_TextBox" Visible="true" TextMode="SingleLine" Rows="1" Width="60" runat="server" ReadOnly="true" CssClass="yui-datatable-moves-theme"
                                                                                            Text='<%# AccessLevelName(Eval("AccessLevel")) %>'
                                                                                            Style="border: none; background-color: Transparent;" />
                                                                                    </ItemTemplate>
                                                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                                </asp:TemplateField>
                                                                            </Columns>
                                                                        </roWebControls:roGridViewControl>
                                                                        <asp:HiddenField ID="hdnGroupUserFieldsNotUsedSelectedRowIndex" runat="server" Value="-1" />
                                                                        <asp:HiddenField ID="hdnGroupUserFieldsNotUsedSelectedColIndex" runat="server" Value="-1" />
                                                                    </td>
                                                                    <td align="center" valign="top" style="padding-top: 10px; padding-right: 10px; width: 10%;">
                                                                        <asp:ImageButton ID="ibtGroupUserFieldAsign" ImageUrl="Images/UserFields Asign 32.gif" ToolTip="Agregar campo personalizado seleccionado." Width="32" Height="32" runat="server" />
                                                                        <br />
                                                                        <asp:ImageButton ID="ibtGroupUserFieldRemove" ImageUrl="Images/UserFields Remove 32.gif" ToolTip="Quitar campo personalizado seleccionado." Width="32" Height="32" runat="server" />
                                                                    </td>
                                                                    <td style="width: 40%;">
                                                                        <roWebControls:roGridViewControl ID="grdGroupUserFieldsUsed" runat="server" AutoGenerateColumns="false" DataSourceID="BlankData"
                                                                            GridLines="None" CellPadding="4"
                                                                            ShowFooter="False" AllowPaging="false" PageSize="7"
                                                                            CssClass="yui-datatable-small-theme"
                                                                            Width="100%" ScrollWidth="100%" Height="210px" FreezeHeader="true" Scrolling="Auto"
                                                                            CellHoverCssClass="row-over" CellSelectCssClass="row-select"
                                                                            RowSelectedControlID='<%# hdnGroupUserFieldsUsedSelectedRowIndex.ClientID%>'
                                                                            ColSelectedControlID='<%# hdnGroupUserFieldsUsedSelectedColIndex.ClientID%>'>
                                                                            <RowStyle CssClass="data-row" />
                                                                            <AlternatingRowStyle CssClass="alt-data-row" />
                                                                            <Columns>
                                                                                <asp:ButtonField Text="" CommandName="EditClick" Visible="False" />
                                                                                <asp:ButtonField Text="" CommandName="RemoveClick" Visible="false" />
                                                                                <asp:TemplateField>
                                                                                    <ItemTemplate>
                                                                                        <div class="fix-data-row-div">
                                                                                            <div class="fix-data-row-div-left">
                                                                                                <%--<asp:Image ID="Select_Image" ImageUrl="../Images/Transparencia.gif" Height="8" Width="10" runat="server"  />--%>
                                                                                                <img id="imgEdit4" src="~/Base/Images/Grid/edit.png" visible="true" runat="server" title="Editar" />
                                                                                                <img id="imgEditAccept4" src="~/Base/Images/Grid/save.png" visible="false" runat="server" />
                                                                                            </div>
                                                                                            <div class="fix-data-row-div-right">
                                                                                                <img id="imgRemove4" src="~/Base/Images/Grid/remove.png" visible="true" runat="server" title="Eliminar" />
                                                                                                <img id="imgEditCancel4" src="~/Base/Images/Grid/cancel.png" visible="false" runat="server" />
                                                                                            </div>
                                                                                        </div>
                                                                                    </ItemTemplate>
                                                                                    <ItemStyle Width="50px" HorizontalAlign="Center" VerticalAlign="Middle" CssClass="fix-data-row" />
                                                                                </asp:TemplateField>
                                                                                <asp:TemplateField HeaderText="Nombre">
                                                                                    <ItemTemplate>
                                                                                        <asp:Image ID="GroupUserFieldUsed_Image" ImageUrl="Images/UserFields 32.gif" Height="12" Width="12" runat="server" />
                                                                                        <asp:Label ID="GroupUserFieldUsed_Label" Text='<%# Eval("FieldName") %>' Visible="true" runat="server" Width="150" CssClass="yui-datatable-moves-theme" />
                                                                                    </ItemTemplate>
                                                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                                </asp:TemplateField>
                                                                                <asp:TemplateField HeaderText="Tipo">
                                                                                    <ItemTemplate>
                                                                                        <asp:TextBox ID="GroupUserFieldUsed_TextBox" Visible="true" TextMode="SingleLine" Rows="1" Width="60" runat="server" ReadOnly="true" CssClass="yui-datatable-moves-theme"
                                                                                            Text='<%# FieldTypeName(Eval("FieldType")) %>'
                                                                                            Style="border: none; background-color: Transparent;" />
                                                                                    </ItemTemplate>
                                                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                                </asp:TemplateField>
                                                                                <asp:TemplateField HeaderText="Nivel acceso">
                                                                                    <ItemTemplate>
                                                                                        <asp:TextBox ID="GroupAccessLeveldUsed_TextBox" Visible="true" TextMode="SingleLine" Rows="1" Width="60" runat="server" ReadOnly="true" CssClass="yui-datatable-moves-theme"
                                                                                            Text='<%# AccessLevelName(Eval("AccessLevel")) %>'
                                                                                            Style="border: none; background-color: Transparent;" />
                                                                                    </ItemTemplate>
                                                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                                </asp:TemplateField>
                                                                            </Columns>
                                                                        </roWebControls:roGridViewControl>
                                                                        <asp:HiddenField ID="hdnGroupUserFieldsUsedSelectedRowIndex" runat="server" Value="-1" />
                                                                        <asp:HiddenField ID="hdnGroupUserFieldsUsedSelectedColIndex" runat="server" Value="-1" />
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </div>

                                            <div id="panTaskFieldsOptions" style="width: 100%; height: 100%; display: none; vertical-align: top;">
                                                <table id="tbTaskFieldsOptions" runat="server" cellpadding="0" cellspacing="0" style="width: 100%;">
                                                    <tr>
                                                        <td>
                                                            <div class="panHeader2">
                                                                <span style="">
                                                                    <asp:Label ID="lblTaskFieldsOptionsHeader" runat="server" Text="Personalización de la ficha de la ${Task}" /></span>
                                                            </div>
                                                            <br />
                                                        </td>
                                                    </tr>
                                                    <tr style="padding-top: 5px">
                                                        <td style="padding-left: 20px">
                                                            <asp:Label ID="lblTaskFieldsOptionsInfo" Text="Este formulario le permite definir los campos que necesite utilizar en las ${Tasks}." CssClass="editTextFormat" runat="server"></asp:Label>
                                                            <br />
                                                            <br />
                                                        </td>
                                                    </tr>
                                                    <tr id="Tr10" style="padding-top: 20px; padding-bottom: 20px; vertical-align: top">
                                                        <td style="padding-left: 20px">

                                                            <asp:ObjectDataSource ID="BlankData1" runat="server" SelectMethod="_Select" TypeName="BlankDataObject" />
                                                            <table cellpadding="0" cellspacing="0" width="100%">
                                                                <tr>
                                                                    <td>

                                                                        <table cellpadding="0" cellspacing="0" width="100%">
                                                                            <tr>
                                                                                <td>
                                                                                    <asp:Label ID="lblTaskFieldsNotUsed" Text="Campos disponibles" Font-Bold="true" runat="server" />
                                                                                </td>
                                                                            </tr>
                                                                        </table>
                                                                    </td>
                                                                    <td></td>
                                                                </tr>
                                                                <tr>
                                                                    <td style="width: 40%">
                                                                        <roWebControls:roGridViewControl ID="grdTaskFieldsNotUsed" runat="server" AutoGenerateColumns="false" DataSourceID="BlankData1"
                                                                            GridLines="None" CellPadding="4"
                                                                            ShowFooter="False" AllowPaging="false" PageSize="7"
                                                                            CssClass="yui-datatable-small-theme"
                                                                            Width="100%" ScrollWidth="100%" Height="210px" FreezeHeader="true" Scrolling="Auto"
                                                                            CellHoverCssClass="row-over" CellSelectCssClass="row-select"
                                                                            RowSelectedControlID='<%# hdnTaskFieldsNotUsedSelectedRowIndex.ClientID%>'
                                                                            ColSelectedControlID='<%# hdnTaskFieldsNotUsedSelectedColIndex.ClientID%>'>
                                                                            <RowStyle CssClass="data-row" />
                                                                            <AlternatingRowStyle CssClass="alt-data-row" />
                                                                            <Columns>
                                                                                <asp:ButtonField Text="" CommandName="EditClick" Visible="False" />
                                                                                <asp:TemplateField>
                                                                                    <ItemTemplate>
                                                                                        <div class="fix-data-row-div">
                                                                                            <div class="fix-data-row-div-left">
                                                                                                <%--<asp:Image ID="Select_Image" ImageUrl="../Images/Transparencia.gif" Height="8" Width="10" runat="server"  />--%>
                                                                                                <img id="imgEdit5" src="~/Base/Images/Grid/edit.png" visible="true" runat="server" title="Editar" />
                                                                                                <img id="imgEditAccept5" src="~/Base/Images/Grid/save.png" visible="false" runat="server" />
                                                                                            </div>
                                                                                            <div class="fix-data-row-div-right">
                                                                                                <img id="imgEditCancel5" src="~/Base/Images/Grid/cancel.png" visible="false" runat="server" />
                                                                                            </div>
                                                                                        </div>
                                                                                    </ItemTemplate>
                                                                                    <ItemStyle Width="50px" HorizontalAlign="Center" VerticalAlign="Middle" CssClass="fix-data-row" />
                                                                                </asp:TemplateField>
                                                                                <asp:TemplateField HeaderText="Nombre">
                                                                                    <ItemTemplate>
                                                                                        <asp:Label ID="TaskFieldNotUsed_Label" Text='<%# Eval("Name") %>' Visible="true" runat="server" Width="150" CssClass="yui-datatable-moves-theme" />
                                                                                    </ItemTemplate>
                                                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                                </asp:TemplateField>
                                                                                <asp:TemplateField HeaderText="Tipo">
                                                                                    <ItemTemplate>
                                                                                        <asp:TextBox ID="TaskFieldNotUsed_TextBox" Visible="true" TextMode="SingleLine" Rows="1" Width="60" runat="server" ReadOnly="true" CssClass="yui-datatable-moves-theme"
                                                                                            Text='<%# FieldTypeName(Eval("Type")) %>'
                                                                                            Style="border: none; background-color: Transparent;" />
                                                                                    </ItemTemplate>
                                                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                                </asp:TemplateField>
                                                                                <asp:TemplateField HeaderText="Cuando">
                                                                                    <ItemTemplate>
                                                                                        <asp:TextBox ID="TaskFieldNotUsed_TextBoxWhen" Visible="true" TextMode="SingleLine" Rows="1" Width="130" runat="server" ReadOnly="true" CssClass="yui-datatable-moves-theme"
                                                                                            Text='<%# FieldActionName(Eval("Action")) %>'
                                                                                            Style="border: none; background-color: Transparent;" />
                                                                                    </ItemTemplate>
                                                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                                </asp:TemplateField>
                                                                            </Columns>
                                                                        </roWebControls:roGridViewControl>

                                                                        <asp:HiddenField ID="hdnTaskFieldsNotUsedSelectedRowIndex" runat="server" Value="-1" />
                                                                        <asp:HiddenField ID="hdnTaskFieldsNotUsedSelectedColIndex" runat="server" Value="-1" />
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </div>

                                            <div id="panBusinessCenterFieldsOptions" style="width: 100%; height: 100%; display: none; vertical-align: top;">
                                                <table id="tbBusinessCenterFieldsOptions" runat="server" cellpadding="0" cellspacing="0" style="width: 100%;">
                                                    <tr>
                                                        <td>
                                                            <div class="panHeader2">
                                                                <span style="">
                                                                    <asp:Label ID="lblBusinessCenterFieldsOptionsHeader" runat="server" Text="Personalización de la ficha del ${BusinessCenter}" /></span>
                                                            </div>
                                                            <br />
                                                        </td>
                                                    </tr>
                                                    <tr style="padding-top: 5px">
                                                        <td style="padding-left: 20px">
                                                            <asp:Label ID="lblBusinessCenterFieldsOptionsInfo" Text="Este formulario le permite definir los campos que necesite utilizar en los ${BusinessCenters}." CssClass="editTextFormat" runat="server"></asp:Label>
                                                            <br />
                                                            <br />
                                                        </td>
                                                    </tr>
                                                    <tr id="Tr4" style="padding-top: 20px; padding-bottom: 20px; vertical-align: top">
                                                        <td style="padding-left: 20px">

                                                            <asp:ObjectDataSource ID="BlankData2" runat="server" SelectMethod="_Select" TypeName="BlankDataObject" />
                                                            <table cellpadding="0" cellspacing="0" width="100%">
                                                                <tr>
                                                                    <td>

                                                                        <table cellpadding="0" cellspacing="0" width="100%">
                                                                            <tr>
                                                                                <td>
                                                                                    <asp:Label ID="lblBusinessCenterFieldsNotUsed" Text="Campos disponibles" Font-Bold="true" runat="server" />
                                                                                </td>
                                                                            </tr>
                                                                        </table>
                                                                    </td>
                                                                    <td></td>
                                                                </tr>
                                                                <tr>
                                                                    <td style="width: 40%">
                                                                        <roWebControls:roGridViewControl ID="grdBusinessCenterFieldsNotUsed" runat="server" AutoGenerateColumns="false" DataSourceID="BlankData2"
                                                                            GridLines="None" CellPadding="4"
                                                                            ShowFooter="False" AllowPaging="false" PageSize="7"
                                                                            CssClass="yui-datatable-small-theme"
                                                                            Width="100%" ScrollWidth="100%" Height="210px" FreezeHeader="true" Scrolling="Auto"
                                                                            CellHoverCssClass="row-over" CellSelectCssClass="row-select"
                                                                            RowSelectedControlID='<%# hdnBusinessCenterFieldsNotUsedSelectedRowIndex.ClientID%>'
                                                                            ColSelectedControlID='<%# hdnBusinessCenterFieldsNotUsedSelectedColIndex.ClientID%>'>
                                                                            <RowStyle CssClass="data-row" />
                                                                            <AlternatingRowStyle CssClass="alt-data-row" />
                                                                            <Columns>
                                                                                <asp:ButtonField Text="" CommandName="EditClick" Visible="False" />
                                                                                <asp:TemplateField>
                                                                                    <ItemTemplate>
                                                                                        <div class="fix-data-row-div">
                                                                                            <div class="fix-data-row-div-left">
                                                                                                <%--<asp:Image ID="Select_Image" ImageUrl="../Images/Transparencia.gif" Height="8" Width="10" runat="server"  />--%>
                                                                                                <img id="imgEdit5" src="~/Base/Images/Grid/edit.png" visible="true" runat="server" title="Editar" />
                                                                                                <img id="imgEditAccept5" src="~/Base/Images/Grid/save.png" visible="false" runat="server" />
                                                                                            </div>
                                                                                            <div class="fix-data-row-div-right">
                                                                                                <img id="imgEditCancel5" src="~/Base/Images/Grid/cancel.png" visible="false" runat="server" />
                                                                                            </div>
                                                                                        </div>
                                                                                    </ItemTemplate>
                                                                                    <ItemStyle Width="50px" HorizontalAlign="Center" VerticalAlign="Middle" CssClass="fix-data-row" />
                                                                                </asp:TemplateField>
                                                                                <asp:TemplateField HeaderText="Nombre">
                                                                                    <ItemTemplate>
                                                                                        <asp:Label ID="BusinessCenterFieldNotUsed_Label" Text='<%# Eval("Name") %>' Visible="true" runat="server" Width="150" CssClass="yui-datatable-moves-theme" />
                                                                                    </ItemTemplate>
                                                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                                </asp:TemplateField>
                                                                            </Columns>
                                                                        </roWebControls:roGridViewControl>

                                                                        <asp:HiddenField ID="hdnBusinessCenterFieldsNotUsedSelectedRowIndex" runat="server" Value="-1" />
                                                                        <asp:HiddenField ID="hdnBusinessCenterFieldsNotUsedSelectedColIndex" runat="server" Value="-1" />
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="right" valign="bottom" class="DetailFrame_Background" style="padding-right: 20px; height: 100%; vertical-align: bottom;">
                                            <table>
                                                <tr align="right">
                                                    <td>
                                                        <asp:Button ID="btSave" Text="${Button.ApplyChanges}" runat="server" OnClientClick="return CheckSave();" Visible="false" CssClass="btnFlat btnFlatBlack btnFlatAsp" />

                                                        <asp:Button ID="btRemoveEmployeeUserField" Text="${Button.ApplyChanges}" runat="server" Visible="false" CssClass="btnFlat btnFlatBlack btnFlatAsp" />
                                                        <asp:Button ID="btRemoveGroupUserField" Text="${Button.ApplyChanges}" runat="server" Visible="false" CssClass="btnFlat btnFlatBlack btnFlatAsp" />
                                                    </td>
                                                    <td>
                                                        <asp:Button ID="btCancel" Text="${Button.UndoChanges}" runat="server" Visible="false" CssClass="btnFlat btnFlatBlack btnFlatAsp" />
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <!-- Mensajes -->
                                            <div id="divMsgBottom" class="divMsg" style="display: none;">
                                                <table style="width: 100%;">
                                                    <tr>
                                                        <td align="center" style="width: 20px; height: 16px;">
                                                            <img id="Img2" src="~/Base/Images/MessageFrame/Alert16.png" runat="server" /></td>
                                                        <td align="left" style="padding-left: 10px; color: white;"><span id="msgBottom"></span></td>
                                                        <td align="right" style="color: White; padding-right: 10px;">
                                                            <a href="javascript: void(0);" class="aMsg" onclick="saveChanges();"><span id="lblSaveChangesBottom" runat="server" /></a>
                                                            &nbsp;&nbsp;&nbsp;|&nbsp;&nbsp;&nbsp;
                                                            <a href="javascript: void(0);" onclick="undoChanges();" class="aMsg"><span id="lblUndoChangesBottom" runat="server" /></a>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </div>
                                        </td>
                                    </tr>
                                </table>

                                <asp:Button ID="btRefresh" runat="server" Style="display: none;" />

                                <asp:HiddenField ID="hdnChanged" runat="server" />

                                <asp:HiddenField ID="hdnIsPostBack_PageBase" runat="server" />
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </div>
        </div>
    </div>
    <asp:HiddenField ID="ConfigurationOptions_TabVisibleName" Value="" runat="server" />

    <Local:MessageFrame ID="MessageFrame1" runat="server" />
</asp:Content>