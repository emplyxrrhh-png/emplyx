@Code
    Dim oSerializer As System.Web.Script.Serialization.JavaScriptSerializer
    oSerializer = New System.Web.Script.Serialization.JavaScriptSerializer()
    Dim OnBoardingController As VTLive40.OnBoardingController = New VTLive40.OnBoardingController()
End Code
<div class="panHeader4">
    @OnBoardingController.GetServerLanguage().Translate("roOnBoardingTasks", "OnBoarding")
</div>

<div id="listResumeTasksDiv" class="list-containerRequests">

    @Code
        Html.DevExtreme().ScrollView() _
            .ID("scrollView") _
            .Height(350) _
            .ShowScrollbar(ShowScrollbarMode.Always) _
        .Direction(ScrollDirection.Both) _
        .Content(Sub()
        @<text>

            @(Html.DevExtreme().DataGrid() _
                                                                                                                     .ID("listResumeTasks") _
                                                                                                        .DataSource(Function(ds)
                                                                                                                        Return ds.Mvc() _
                                                                                           .Controller("OnBoarding") _
                                                                                           .UpdateAction("UpdateTask") _
                                                                                           .DeleteAction("DeleteTask") _
                                                                                           .InsertAction("InsertNewTask") _
                                                    .OnUpdated("RefreshTaskList") _
                                                                                           .Key("Id") _
                                                                                           .LoadAction("GetTasksResume") _
                                                                                           .LoadParams(New With {.idList = New JS("GetIdList")})
                                                                                                                    End Function) _
                                                                                           .ShowColumnLines(False) _
                                                                                           .ShowRowLines(True) _
                                                                              .Editing(Sub(edit)
                                                                                           edit.AllowDeleting(New JS("AllowModify"))
                                                                                           edit.AllowUpdating(New JS("AllowModify"))
                                                                                           edit.Mode(GridEditMode.Cell)
                                                                                           edit.RefreshMode(GridEditRefreshMode.Reshape)
                                                                                           edit.UseIcons(True)
                                                                                           edit.Texts(Sub(texts)
                                                                                                          texts.ConfirmDeleteMessage(OnBoardingController.GetServerLanguage().Translate("roOnBoardingDeleteTask", "OnBoarding"))
                                                                                                          texts.ConfirmDeleteTitle(OnBoardingController.GetServerLanguage().Translate("roOnBoardingDeleteTitle", "OnBoarding"))
                                                                                                      End Sub)
                                                                                       End Sub) _
                                                                                                                                               .RowAlternationEnabled(False) _
                                                                                                                                               .ShowBorders(False) _
                                                                                                                                               .ShowColumnHeaders(False) _
                                                                                                                                               .OnRowPrepared("SetGridHeight") _
                                                                                                                                               .OnCellPrepared("cell_prepared") _
                                                                                                                                               .ColumnHidingEnabled(False) _
                                                                                                                                               .ColumnAutoWidth(True) _
                                                                                                                                               .AllowColumnResizing(True) _
                                                                                                                                               .NoDataText(OnBoardingController.GetServerLanguage().Translate("roNoData", "OnBoarding")) _
                                                                                                                                               .LoadPanel(Sub(columns)
                                                                                                                                                              columns.Text(OnBoardingController.GetServerLanguage().Translate("roOnBoardingLoading", "OnBoarding"))
                                                                                                                                                          End Sub) _
                                                                                                                                  .Paging(Sub(columns)
                                                                                                                                              columns.PageSize(25)
                                                                                                                                          End Sub) _
                                                                                                                     .Pager(Sub(columns)
                                                                                                                                columns.ShowPageSizeSelector(True)
                                                                                                                                columns.AllowedPageSizes({25, 50, 100})
                                                                                                                                columns.ShowInfo(False)
                                                                                                                            End Sub) _
                                                                                                        .Columns(Sub(columns)
                                                                                                                     columns.Add().DataField("Done").SortIndex(0).SortOrder(SortOrder.Asc).Width(30).Caption(OnBoardingController.GetServerLanguage().Translate("roOnBoardingTasks", "OnBoarding"))
                                                                                                                     columns.Add().DataField("TaskName").SortIndex(2).SortOrder(SortOrder.Asc).Visible(True)
                                                                                                                     columns.Add().DataField("IdList").Visible(False)
                                                                                                                     columns.Add().DataField("Id").SortIndex(1).SortOrder(SortOrder.Asc).Visible(False)
                                                                                                                     columns.Add().DataField("SupervisorName").AllowEditing(False).Alignment(HorizontalAlignment.Right).Caption(OnBoardingController.GetServerLanguage().Translate("roOnBoardingUser", "OnBoarding"))
                                                                                                                     columns.Add().DataField("LastChangeDate").AllowEditing(False).Width(90).Alignment(HorizontalAlignment.Right).Caption(OnBoardingController.GetServerLanguage().Translate("roOnBoardingStartDate", "OnBoarding"))
                                                                                                                 End Sub) _
                                                                                )
        </text>
    End Sub).Render()
    End Code

    @If (ViewBag.PermissionOverEmployees > 3) Then
    @<div style="margin-top:20px;">
        <div style="width:90%; float:left">
            @(Html.DevExtreme().TextBox() _
                                                                                        .ID("TaskText") _
                                                                        .OnEnterKey("addNewTask") _
                                                                                                                        .Placeholder(OnBoardingController.GetServerLanguage().Translate("roOnBoardingNewTask", "OnBoarding")) _
                )
        </div>
        <div style="float:right">
            @(Html.DevExtreme().Button() _
                                .ID("addTask") _
                                .Icon("plus") _
                        .OnClick("addNewTask") _
                                    .Type(ButtonType.Normal) _
            )
        </div>
    </div>
End if
</div>