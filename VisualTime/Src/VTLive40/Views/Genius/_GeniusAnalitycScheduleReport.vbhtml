@Imports Robotics.Base.DTOs

@ModelType roGeniusView
@Code
    Dim labels = ViewData(VTLive40.Helpers.Constants.DefaultLanguagesEntries)
End Code

<section id="geniusAnalyticPlanification">
    <div class="panHeader2 panBottomMargin">
        <span class="panelTitleSpan">
            <span id="">@Html.Raw(labels("Genius#scheduleReport"))</span>
        </span>
    </div>
    @Code
        Html.DevExtreme().DataGrid() _
            .ID("PlanningList") _
            .DataSource(Function(ds)
                            Return ds.Mvc() _
                            .Controller("Genius") _
                            .UpdateAction("UpdatePlanning") _
                            .UpdateMethod("Post") _
                            .DeleteAction("DeletePlanning") _
                            .InsertAction("InsertNewPlanning") _
                            .OnLoaded("onPlanningListLoaded") _
                            .Key("ID") _
                            .OnBeforeSend("onBeforeSendPlanning") _
                                .OnInserted("RefreshPlanningList") _
                                    .OnUpdated("RefreshPlanningList") _
                                    .OnLoading("OnPlanningListLoading") _
                                    .LoadAction("GetPlanningList") _
                                    .LoadParams(New With {.idGeniusView = New JS("GetCurrentGeniusView")})
                        End Function) _
        .ShowColumnLines(False) _
.ShowRowLines(True) _
.ShowColumnHeaders(True) _
            .Columns(Sub(columns)
                         columns.Add().AllowEditing(True).DataField("Name").Caption(labels("Genius#planningDescription"))
                         columns.Add().AllowEditing(False).DataField("SchedulerText").Caption(labels("Genius#planningPeriod"))
                         columns.Add().AllowEditing(False).DataField("NextDateTimeExecuted").CalculateCellValue("formatNextDate").Caption(labels("Genius#nextPlanning"))
                         columns.Add().AllowEditing(False).Visible(False).DataField("ID")
                         columns.Add().AllowEditing(False).Visible(False).DataField("LastDateTimeUpdated")
                         columns.Add().AllowEditing(False).Visible(False).DataField("IDGeniusView")
                     End Sub) _
                    .Editing(Sub(edit)
                                 edit.AllowDeleting(True)
                                 edit.AllowUpdating(True)
                                 edit.Mode(GridEditMode.Row)
                                 edit.RefreshMode(GridEditRefreshMode.Reshape)
                                 edit.UseIcons(True)
                                 edit.Texts(Sub(texts)
                                                texts.ConfirmDeleteMessage(labels("Genius#deletePlanning"))
                                                texts.ConfirmDeleteTitle(labels("Genius#deletePlanningHeader"))
                                            End Sub)
                             End Sub) _
                                         .RowAlternationEnabled(False) _
                    .ShowBorders(False) _
                    .ColumnHidingEnabled(False) _
                    .ColumnAutoWidth(True) _
                    .AllowColumnResizing(True) _
                    .OnRowValidating("validateSchedulerData") _
                    .OnEditingStart("onPlanningListEditing") _
                    .OnRowUpdating("onPlanningUpdating") _
                    .OnRowInserting("onPlanningInserting") _
                    .OnInitNewRow("onPlanningInitNewRow") _
                    .OnEditingStart("onPlanningEditingStart") _
        .NoDataText(labels("Genius#empty")).Render()

    End Code
</section>