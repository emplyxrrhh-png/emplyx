@Imports Robotics.Base.DTOs

@Code
    Dim labels = ViewData(VTLive40.Helpers.Constants.DefaultLanguagesEntries)
    Dim baseURL = Url.Content("~")
End Code

<section id="BotsNew">
    <div class="panHeader2 panBottomMargin">
        <span class="panelTitleSpan">
            <span id="">@Html.Raw(labels("Bots#rulesHeader"))</span>
        </span>
    </div>

    <div style="min-height:15px"></div>

    <div id="botAtions" style="display: flex; justify-content: flex-end">
        <div id="RulesRefresh" style="display: inline-block;margin-left:10px;">
            <div id="employeeStatus" style=" margin-left: auto; margin-right: 0;">
                @(Html.DevExtreme().Button() _
                        .ID("addRule") _
                        .Icon("plus") _
                        .OnClick("addNewRule") _
                        .Text(labels("Bots#addRule")) _
                        .Type(ButtonType.Default))
            </div>
        </div>
    </div>

    <div style="min-height:15px"></div>

    <div id="divRules" Class="jsGridContentChannel dextremeGrid">
        @(Html.DevExtreme().DataGrid() _
                .ID("gridRulesBots") _
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
                                            texts.ConfirmDeleteMessage(labels("Bots#roRulesDelete"))
                                            texts.ConfirmDeleteTitle(labels("Bots#roRulesDeleteTitle"))
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
                .OnRowRemoved("RulesRemoved") _
                .Export(Sub(columns)
                            columns.Enabled(False)
                            columns.AllowExportSelectedData(True)
                        End Sub) _
                .NoDataText(labels("Bots#roNoData").ToString()) _
                .Paging(Sub(columns)
                            columns.PageSize(25)
                        End Sub) _
                .Pager(Sub(columns)
                           columns.ShowPageSizeSelector(True)
                           columns.AllowedPageSizes({25, 50, 100})
                           columns.ShowInfo(False)
                       End Sub) _
                .OnCellClick("RuleSelected") _
                .OnCellPrepared("onCellPrepared") _
                .OnContextMenuPreparing("context_menu") _
                .FilterRow(Sub(columns)
                               columns.Visible(True)
                           End Sub) _
                .HeaderFilter(Sub(columns)
                                  columns.Visible(True)
                              End Sub) _
                .OnInitNewRow("onInitNewRow") _
                .Columns(Sub(columns)
                             columns.Add().DataField("Id").Visible(False)
                             columns.Add().DataField("Name").Caption(labels("Bots#name").ToString())
                             columns.Add().DataField("Type").Caption(labels("Bots#type").ToString()).Lookup(Function(lookup) lookup.DataSource(ViewBag.AllRules).ValueExpr("Id").DisplayExpr("Name"))
                             columns.Add().DataField("IsActive").Caption(labels("Bots#enabled").ToString()).Width(100)
                             columns.Add().DataField("IDTemplate").Visible(False)
                             columns.Add().Type(GridCommandColumnType.Buttons).Buttons(Sub(b)
                                                                                           b.Add().Hint("Configurar").Icon("edit").OnClick("RuleSelected")
                                                                                           b.Add().Name(GridColumnButtonName.Delete)
                                                                                       End Sub)

                         End Sub) _
                .OnToolbarPreparing("toolbar_preparing"))

        @Code
            Html.DevExtreme().Popup() _
                .ID("newRulePopup") _
                .Width(New JS("getWidth")) _
                .Height(New JS("getHeight")) _
                .ShowTitle(False) _
                .Title("Crear regla") _
                .OnShown("onRulePopupShown") _
                .OnHiding("onRulePopupHiding") _
                .DragEnabled(False) _
                .HideOnOutsideClick(True) _
                .ContentTemplate(Sub()
                @<text>
                    @Html.Partial("_BotsCreateRule")
                </text>
            End Sub).Render()
        End Code
    </div>
</section>