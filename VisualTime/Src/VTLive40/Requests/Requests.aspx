<%@ Page Language="VB" MasterPageFile="~/Base/mastEmp.master" AutoEventWireup="false" Inherits="VTLive40.Requests" Title="Solicitudes y trámites" CodeBehind="Requests.aspx.vb" %>

<asp:Content ID="cMainBody" ContentPlaceHolderID="contentMainBody" runat="Server">
    <%@ Register Src="~/Requests/WebUserForms/frmAdvFilter.ascx" TagName="frmAdvFilter" TagPrefix="roForms" %>

    <script language="javascript" type="text/javascript">

        //=================================================================================
        function PageBase_Load() {

            setTimeout(function () { initialLoad(); }, 100);

        }

        function resizeListHeight() {

            var divGrid = document.getElementById('gridMyRequestsListContainer');
            if (divGrid != null) {
                divGrid.style.height = document.body.clientHeight - 180 - 32 + "px";
            }

            divGrid = document.getElementById('gridOtherRequestsListContainer');
            if (divGrid != null) {
                divGrid.style.height = document.body.clientHeight - 181 - 32 - 20 + "px";
            }

            var divGrid = document.getElementById('gridHistoryRequestsListContainer');
            if (divGrid != null) {
                divGrid.style.height = document.body.clientHeight - 180 - 32 + "px";
            }

            var divButtons = document.getElementById('divButtons');
            if (divButtons != null) {
                divButtons.style.height = document.body.clientHeight - 128 + "px";
            }

            var divRequestContent = document.getElementById('divRequestContent');
            if (divRequestContent != null) {
                divRequestContent.style.height = document.body.clientHeight - 140 + "px";
            }
        }

        var SelectedRowID = '';
        var SelectedRowName = '';

        function SelectRow(RowSelected, IdTableRowSelected) {
            try {

                var tdRow = document.getElementById(SelectedRowName + "_" + SelectedRowID);
                if (tdRow != null) {
                    if (RowSelected != null) {
                        if (tdRow.className == 'RequestsListRow-selectedlocked')
                            tdRow.className = 'RequestsListRow-locked';
                        else
                            tdRow.className = 'RequestsListRow';
                    }
                    else {
                        tdRow.className = 'RequestsListRow-selected';
                    }
                }

                if (RowSelected != null) {

                    if (RowSelected.className == 'RequestsListRow-lockedover')
                        RowSelected.className = 'RequestsListRow-selectedlocked';
                    else
                        RowSelected.className = 'RequestsListRow-selected';

                    var oArray = new Array();
                    oArray = RowSelected.id.split("_");
                    SelectedRowID = oArray[1];
                    SelectedRowName = oArray[0];
                }
                else {
                    if (tdRow == null) {
                        SelectedRowID = '';
                        SelectedRowName = '';
                    }
                }

                if (SelectedRowID != '') {
                    loadRequest(SelectedRowID, IdTableRowSelected);
                }
                else {
                    loadRequest(0, '');
                }

            }
            catch (e) {
                showError("SelectRow", e);
            }
        }

        function OverRow(Row) {
            switch (Row.className) {
                case 'RequestsListRow':
                    Row.className = 'RequestsListRow-over';
                    break;
                case 'RequestsListRow-locked':
                    Row.className = 'RequestsListRow-lockedover';
                    break;
            }
        }

        function OutRow(Row) {
            switch (Row.className) {
                case 'RequestsListRow-over':
                    Row.className = 'RequestsListRow';
                    break;
                case 'RequestsListRow-lockedover':
                    Row.className = 'RequestsListRow-locked';
                    break;
            }
        }

        function LockRequest(IDRequest, IdTableRow) {
            //    try {
            //showLoadingGrid(false);

            $("#" + IdTableRow).removeClass();
            $("#" + IdTableRow).addClass('RequestsListRow-selectedlocked');

            $("#" + IdTableRow).find("div.buttonApproveRequest").first().parent().empty();
            $("#" + IdTableRow).find("div.buttonRefuseRequest").first().parent().empty();

            $("#lnkApprove").parent().empty();
            $("#lnkRefuse").parent().empty();
            //    }
            //    catch (e) {
            //        showError("LockRequest", e);
            //    }
        }
    </script>
    <asp:HiddenField ID="actualListValue" ClientIDMode="Static" runat="server" />
    <asp:HiddenField ID="dtFormat" runat="server" />
    <asp:HiddenField ID="dtFormatText" runat="server" />
    <asp:HiddenField ID="bCalendarMode" runat="server" />

    <div id="divModalBgDisabled" class="modalBackground" style="position: absolute; left: 0px; top: 0px; z-index: 990; width: 1680px; height: 900px; display: none;"></div>

    <div id="divMainBody">
        <!-- TAB SUPERIOR -->
        <div id="divTabInfo" class="divDataCells">
            <div style="min-height: 10px"></div>
            <div id="divTab" class="blackRibbonTitle">
            </div>
            <div style="min-height: 10px"></div>
        </div>
        <!-- FIN TAB SUPERIOR -->
        <!-- DETALLE -->
        <div id="divTabData" class="divDataCells">
            <div id="divContenido" class="divAllContent">
                <div id="divFContent" style="height: initial;" class="maxHeight">
                    <div style="float: left; width: calc(45% - 25px)">
                        <div class="RoundCornerFrame roundCorner">
                            <!-- Listas solicitudes -->
                            <div id="divRequestLists" style="">
                                <!-- TABS Superiores -->
                                <div id="buttonsTab0">
                                    <table border="0" cellspacing="0" cellpadding="0">
                                        <tr>
                                            <td>
                                                <a href="javascript: void(0);" id="tabMyRequestsListTitle" class="tabt_l_active" onclick="chTopTabList('MyRequests');">
                                                    <span style="display: block; margin-left: 15px; padding-top: 5px;">
                                                        <asp:Label ID="lblMyRequestsListTitle" runat="server" Text="Mis solicitudes"></asp:Label>
                                                    </span></a>
                                            </td>
                                            <td>
                                                <div id="tab2actiu" runat="server" style="display: ;">
                                                    <a href="javascript: void(0);" id="tabOtherRequestsListTitle" class="tabt_r" onclick="chTopTabList('OtherRequests');">
                                                        <span style="display: block; margin-left: 15px; padding-top: 5px;">
                                                            <asp:Label ID="lblOtherRequestsListTitle" runat="server" Text="Otras solicitudes"></asp:Label>
                                                        </span>
                                                    </a>
                                                </div>
                                                <div id="tab2inactiu" style="display: none;" runat="server"><span class="tabt_r_inactive"></span></div>
                                            </td>
                                        </tr>
                                    </table>
                                </div>

                                <div id="tabMyRequestsListFilter">
                                    <roForms:frmAdvFilter ID="Pend" runat="server" Mode="0" Prefix="Pend" />
                                </div>
                                <div id="tabMyRequestsListContent" style="">
                                    <div id="gridMyRequestsListContainer" class="RequestsContentList" style="">
                                    </div>
                                </div>

                                <div id="tabOtherRequestsListFilter1" style="display: none;">
                                    <roForms:frmAdvFilter ID="Other" runat="server" Mode="1" Prefix="Other" />
                                </div>
                                <div id="tabOtherRequestsListContent" style="display: none;">
                                    <div id="gridOtherRequestsListContainer" class="RequestsContentList">
                                    </div>
                                </div>
                            </div>

                            <!-- Listas historico -->
                            <div id="divRequestLists2" style="display: none;">
                                <!-- TABS Superiores -->
                                <div id="buttonsTab1">
                                    <table border="0" cellspacing="0" cellpadding="0">
                                        <tr>
                                            <td>
                                                <a href="javascript: void(0);" id="A1" class="tabt_r_active" onclick="">
                                                    <span style="display: block; margin-left: 15px; padding-top: 5px;">
                                                        <asp:Label ID="lblHistoryRequestsListTitle" runat="server" Text="Histórico"></asp:Label>
                                                    </span></a>
                                            </td>
                                            <td>
                                                <div id="Div3" style="display: none;" runat="server"><span class="tabt_r_inactive"></span></div>
                                            </td>
                                        </tr>
                                    </table>
                                </div>

                                <div id="tabHistoryRequestsListFilter" valign="top">
                                    <roForms:frmAdvFilter ID="Hist" runat="server" Mode="2" Prefix="Hist" />
                                </div>
                                <div id="tabHistoryRequestsListContent">
                                    <div id="gridHistoryRequestsListContainer" class="RequestsContentList">
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div>
                        <div id="divButtons" class="divMiddleButtons">
                            <div id="divBarButtons" runat="server" class="maxHeight">&nbsp</div>
                        </div>
                    </div>
                    <div style="float: right; width: calc(55% - 22px)">
                        <!-- Contenido -->
                        <div class="RoundCornerFrame roundCorner" style="overflow-y: scroll">
                            <div id="divRequestContent"></div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <script language="javascript" type="text/javascript">
        window.onresize = function () {
            resizeListHeight();
        }
    </script>
</asp:Content>