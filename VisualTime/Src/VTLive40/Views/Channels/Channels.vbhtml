@Imports Robotics.Base.DTOs
@Imports Robotics.Web.Base.roLanguageWeb

@ModelType roChannel

@Code
    Layout = "~/Views/Shared/_layoutSP.vbhtml"
    Dim baseURL = Url.Content("~")
    Dim labels = ViewData(VTLive40.Helpers.Constants.DefaultLanguagesEntries)
    Dim scriptVersion As String = ViewData(VTLive40.Helpers.Constants.ScriptVersion)
End Code

<script>
    var RootUrl = "@Html.Raw(ViewBag.RootUrl)";
    var Permission = @Html.Raw(ViewBag.PermissionOverEmployees);
    var idPassport = @Html.Raw(ViewBag.IdPassport);
    var BASE_URL = "@baseURL";
    var jsLabels = null;
</script>
<style>
    div.dx-column-indicators {
        float: right !important;
    }
</style>

<link href="@Url.Content("~/Base/Styles/roStart.min.css")@Html.Raw(scriptVersion)" rel="stylesheet" type="text/css" />
<link href="@Url.Content("~/Base/Styles/roLiveStyles.min.css")@Html.Raw(scriptVersion)" rel="stylesheet" type="text/css" />
<link href="@Url.Content("~/Base/Styles/dx.robotics.main.css")@Html.Raw(scriptVersion)" rel="stylesheet" type="text/css" />
<link href="@Url.Content("~/Base/Styles/dx.robotics.css")@Html.Raw(scriptVersion)" rel="stylesheet" type="text/css" />
<script src="@Url.Content("~/Base/Scripts/Ajax.js")@Html.Raw(scriptVersion)" type="text/javascript"></script>
<script src="@Url.Content("~/Base/Scripts/Live/Channels/roChannel.min.js")@Html.Raw(scriptVersion)" type="text/javascript"></script>

<div id="news" style="float:left"></div>
<div id="newsOk" style="float:right; margin-top:60px;"></div>

<div id="loadingDiv" style=" z-index: 20000; position: absolute;top: 25%; left: 50%;">
    @(Html.DevExtreme().LoadIndicator() _
                                            .ID("loading") _
                                            .Height(60) _
                                            .Width(60) _
                                            .Visible(False) _
    )
</div>

<div style="display:flex;">
    <div style="border-left: 1vw solid white;border-top: 1vw solid white; width:100%;border-right: 1vw solid white;">
        <div class="panHeader5" style="display: flex;align-items: center;">
            <div style="margin-right:0.5vw"><img src="~/Base/Images/StartMenuIcos/Channels.png" width="48" height="48" /></div>
            <div>
                <div style="font-size:20px;width:100%">@Html.Raw(labels("Channels#roChannelsTitle"))</div>
                <div><span id="readOnlyDescritionCompany" style="font-size: 11px;font-weight: 100;">@Html.Raw(labels("Channels#roChannelInfo"))</span></div>
            </div>
        </div>
    </div>
</div>
<br />

<div id="divMainBody" style="min-height:unset !important; height:unset !important">
    <!-- TAB SUPERIOR -->
    <div id="divTabData">

        <input id="idChannelSelected" type="hidden" value="" />
        <input type='hidden' id='hdnEmployees' runat='server' value='' />
        <input type="hidden" id="hdnFilter" runat="server" value='' />
        <input type="hidden" id="hdnFilterUser" runat="server" value='' />

        <div id="divContenido" Class="divAllContentChannels twoWindowsFlexLayout">

            <div id="divContenido" class="twoWindowsMainPanel maxHeight divRightContent">

                <div class="form ro-tab-section container" style="display: flex;">

                    <div class="sectionOnBoarding" style="flex-grow: 1;" id="section2">
                        <div id="ChannelStatusResume" class="panHeader4">@Html.Raw(labels("Channels#roChannelStatusResume"))</div>
                        <br />

                        <div id="channelStatus" style="display: flex; justify-content: flex-end">
                            <div id="ChannelRefresh" style="display: inline-block;margin-left:10px;">
                                @(Html.DevExtreme().Button() _
                                        .ID("refreshChannel") _
                                        .Icon("refresh") _
                                        .OnClick("refreshChannels") _
                                        .Type(ButtonType.Default))
                            </div>
                            <div id="ChannelRefresh" style="display: inline-block;margin-left:10px;">
                                @If (ViewBag.PermissionOverEmployees > 3 AndAlso ViewBag.HasChannelsPermission) Then
                                    @<div id="employeeStatus" style=" margin-left: auto; margin-right: 0;">
                                        @(Html.DevExtreme().Button() _
                                            .ID("addChannel") _
                                            .Icon("plus") _
                                            .OnClick("addNewChannel") _
                                            .Text(labels("Channels#roChannelNew")) _
                                            .Type(ButtonType.Default))
                                    </div>
                                End If
                            </div>
                        </div>

                        <br />

                        <div id="divChannels" runat="server" Class="jsGridContentChannel dextremeGrid">
                            @(Html.DevExtreme().DataGrid() _
.ID("gridStatusChannels") _
.DataSource(Function(ds)
                Return ds.Mvc() _
                .Controller("Channels") _
                .LoadAction("GetChannels") _
                .Key("Id") _
                .OnBeforeSend("beforeSend") _
                .DeleteAction("DeleteChannel") _
                .OnModified("refreshChannels")
            End Function) _
.LoadPanel(Sub(loadPanel)
               loadPanel.Enabled(False)
           End Sub) _
.ShowColumnLines(False) _
.ShowRowLines(True) _
.Editing(Sub(edit)
             edit.Mode(GridEditMode.Row)
             edit.RefreshMode(GridEditRefreshMode.Reshape)
             edit.AllowDeleting(New JS("AllowModify"))
             edit.AllowUpdating(New JS("AllowModify"))
             edit.Texts(Sub(texts)
                            texts.ConfirmDeleteMessage(labels("Channels#roChannelDelete"))
                            texts.ConfirmDeleteTitle(labels("Channels#roChannelDeleteTitle"))
                        End Sub)
             edit.UseIcons(True)
         End Sub) _
.Selection(Sub(columns)
               columns.Mode(SelectionMode.Single)
           End Sub) _
.RowAlternationEnabled(False) _
.ShowBorders(False) _
.ColumnHidingEnabled(False) _
.ColumnAutoWidth(False) _
.AllowColumnResizing(True) _
.OnRowRemoved("ChannelRemoved") _
.Export(Sub(columns)
            columns.Enabled(False)
            columns.AllowExportSelectedData(True)
        End Sub) _
.NoDataText(labels("Channels#roNoData").ToString()) _
.Paging(Sub(columns)
            columns.PageSize(25)
        End Sub) _
.Pager(Sub(columns)
           columns.ShowPageSizeSelector(True)
           columns.AllowedPageSizes({25, 50, 100})
           columns.ShowInfo(False)
       End Sub) _
.OnCellClick("ChannelSelected") _
.OnCellPrepared("onCellPrepared") _
.OnContextMenuPreparing("context_menu") _
.FilterRow(Sub(columns)
               columns.Visible(True)
           End Sub) _
.HeaderFilter(Sub(columns)
                  columns.Visible(True)
              End Sub) _
.Columns(Sub(columns)
             columns.Add().DataField("Id").Visible(False)
             columns.Add().DataField("IsComplaintChannel").Visible(False)
             columns.Add().DataField("Title").Caption(labels("Channels#roChannelName")).SortIndex(0).SortOrder(SortOrder.Asc)
             columns.Add().DataField("CreatedOn").Width(150).DataType(GridColumnDataType.Date).Format("dd/MM/yyyy").Alignment(HorizontalAlignment.Left).Caption(labels("Channels#roChannelCreatedOn"))
             columns.Add().DataField("Status").Caption(labels("Channels#roChannelStatus").ToString()) _
                         .HeaderFilter(Sub(filter)
                                           filter.DataSource(New JS("headerFilter"))
                                       End Sub) _
                        .CellTemplate(New JS("progressBar"))
             columns.Add().DataField("OpenConversations").Alignment(HorizontalAlignment.Left).Width(200).Caption(labels("Channels#roChannelPendingConversations"))
             columns.Add().DataField("NewMessages").Alignment(HorizontalAlignment.Left).Width(150).Caption(labels("Channels#roChannelNewMessages"))

             columns.Add().Type(GridCommandColumnType.Buttons).Buttons(Sub(b)
                                                                           b.Add().Hint("Configurar").Icon("edit").OnClick("ChannelSelected")
                                                                           b.Add().Name(GridColumnButtonName.Delete)
                                                                       End Sub)

         End Sub) _
.OnToolbarPreparing("toolbar_preparing") _
                                                                                )

                            @Code
                                Html.DevExtreme().Popup() _
                                                        .ID("newChannelPopup") _
                                                        .Width(New JS("getWidth")) _
                                                        .Height(New JS("getHeight")) _
                                                        .ShowTitle(False) _
                                                        .Title("Crear encuesta") _
                                                        .OnShown("onChannelPopupShown") _
                                                        .OnHiding("onChannelPopupHiding") _
                                                        .DragEnabled(False) _
                                                        .HideOnOutsideClick(True) _
                                                        .ContentTemplate(Sub()
                                                        @<text>
                                                            @Html.Partial("CreateChannel")
                                                        </text>
                                                    End Sub) _
.Render()
                            End Code
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>
<div id="divChannelsEmployeeSelector">
</div>

<Script>
</Script>