@Imports Robotics.Base.DTOs

@ModelType roGeniusView
@Code
    Dim labels = ViewData(VTLive40.Helpers.Constants.DefaultLanguagesEntries)
    Dim GeniusController As VTLive40.GeniusController = New VTLive40.GeniusController()
    Dim items = {New roItem With {.Id = 0, .Text = labels("Genius#scheduleDaily")}, New roItem With {.Id = 1, .Text = labels("Genius#scheduleWeekly")}, New roItem With {.Id = 2, .Text = labels("Genius#scheduleMonthly")}, New roItem With {.Id = 3, .Text = labels("Genius#scheduleOnce")}, New roItem With {.Id = 4, .Text = labels("Genius#scheduleInterval")}}
    Dim periodTypes = {New roItem With {.Id = 0, .Text = labels("Genius#optPeriodOther")}, New roItem With {.Id = 1, .Text = labels("Genius#optPeriodTomorrow")}, New roItem With {.Id = 2, .Text = labels("Genius#optPeriodToday")}, New roItem With {.Id = 3, .Text = labels("Genius#optPeriodYesterday")}, New roItem With {.Id = 4, .Text = labels("Genius#optPeriodCurrentWeek")}, New roItem With {.Id = 5, .Text = labels("Genius#optPeriodLastWeek")}, New roItem With {.Id = 6, .Text = labels("Genius#optPeriodCurrentMonth")}, New roItem With {.Id = 7, .Text = labels("Genius#optPeriodLastMonth")}, New roItem With {.Id = 8, .Text = labels("Genius#optPeriodCurrentYear")}, New roItem With {.Id = 9, .Text = labels("Genius#optPeriodNextWeek")}, New roItem With {.Id = 10, .Text = labels("Genius#optPeriodNextMonth")}}
    Dim daysOfWeek = {New roItem With {.Id = 0, .Text = labels("Genius#monday")}, New roItem With {.Id = 1, .Text = labels("Genius#tuesday")}, New roItem With {.Id = 2, .Text = labels("Genius#wednesday")}, New roItem With {.Id = 3, .Text = labels("Genius#thursday")}, New roItem With {.Id = 4, .Text = labels("Genius#friday")}, New roItem With {.Id = 5, .Text = labels("Genius#saturday")}, New roItem With {.Id = 6, .Text = labels("Genius#sunday")}}
    Dim daysOfWeek4Month = {New roItem With {.Id = 1, .Text = labels("Genius#monday")}, New roItem With {.Id = 2, .Text = labels("Genius#tuesday")}, New roItem With {.Id = 3, .Text = labels("Genius#wednesday")}, New roItem With {.Id = 4, .Text = labels("Genius#thursday")}, New roItem With {.Id = 5, .Text = labels("Genius#friday")}, New roItem With {.Id = 6, .Text = labels("Genius#saturday")}, New roItem With {.Id = 7, .Text = labels("Genius#sunday")}}
    Dim weeksOfMonth = {New roItem With {.Id = 1, .Text = labels("Genius#first")}, New roItem With {.Id = 2, .Text = labels("Genius#second")}, New roItem With {.Id = 3, .Text = labels("Genius#third")}, New roItem With {.Id = 4, .Text = labels("Genius#fourth")}, New roItem With {.Id = 5, .Text = labels("Genius#last")}}

End Code

<section id="geniusPlannerScheduler" style="display:none;">
    <div class="panHeader2 panBottomMargin">
        <span class="panelTitleSpan">
            <span id="">@Html.Raw(labels("Genius#planScheduler"))</span>
        </span>
    </div>
    <div style="min-height:15px"></div>
    <div style="width: 95%;margin: 0 auto;">
        <div style="display: flex;justify-content: center;">
            <div class="dx-field-label">
                @(Html.DevExtreme().RadioGroup() _
                                                    .ID("radioGeniusSchedulerPlan") _
                                                    .DataSource(items) _
                            .ValueExpr("Id") _
                                    .DisplayExpr("Text") _
                                    .Value(3) _
                        .OnValueChanged("onSchedulerChanged") _
                                    .Layout(Orientation.Vertical) _
                )
            </div>
            <div class="dx-field-value">
                <div id="dailySchedule" style="display:none">
                    <span style="float:left; margin-top:7px; margin-right:10px;">@Html.Raw(labels("Genius#each"))</span>
                    <span style="float: left;margin-right: 10px;">@(Html.DevExtreme().NumberBox().ID("txtDays").OnValueChanged("markRowAsUpdated").Min(1).Value(1).Mode(NumberBoxMode.Number).Width(100).ShowSpinButtons(True))</span>
                    <span style="float: left; margin-top: 7px;">@Html.Raw(labels("Genius#days"))</span>
                    <span style="float: left; margin-top: 7px; margin-right: 10px;">@Html.Raw(labels("Genius#at"))</span>
                    <span style="float: left;">
                        @(Html.DevExtreme().DateBox().ID("timeOfDay") _
.Type(DateBoxType.Time).Width(150) _
.OnValueChanged("markRowAsUpdated") _
            )
                    </span>
                </div>
                <div id="weeklySchedule" style="display:none">
                    <span style="float:left; margin-top:7px; margin-right:10px;">@Html.Raw(labels("Genius#each"))</span>
                    <span style="float: left;margin-right: 10px;">@(Html.DevExtreme().NumberBox().ID("txtWeeks").Min(1).Value(1).Mode(NumberBoxMode.Number).Width(100).ShowSpinButtons(True).OnValueChanged("markRowAsUpdated"))</span>
                    <span style="float: left; margin-top: 7px;margin-right:10px;">@Html.Raw(labels("Genius#weeks"))</span>
                    <span style="float: left;margin-right: 10px;">
                        @(Html.DevExtreme().TagBox() _
.ID("lstDaysOfWeek") _
.Placeholder("Seleccionar...") _
.ShowSelectionControls(True) _
.ValueExpr("Id") _
.DisplayExpr("Text") _
.Multiline(False) _
.ShowClearButton(True) _
.SearchEnabled(True) _
.ApplyValueMode(EditorApplyValueMode.UseButtons) _
.SearchExpr("Text") _
.DataSource(daysOfWeek) _
.OnValueChanged("markRowAsUpdated") _
.Width(250) _
                    )
                    </span>
                    <span style="float: left; margin-top: 7px; margin-right: 10px;">@Html.Raw(labels("Genius#at"))</span>
                    <span style="float: left;">
                        @(Html.DevExtreme().DateBox().ID("timeOfDayWeekly") _
.OnValueChanged("markRowAsUpdated") _
.Type(DateBoxType.Time).Width(150) _
            )
                    </span>
                </div>
                <div id="monthlySchedule" style="display:none">
                    <div id="monthlyOption1" style="width: 100%; float: left;">
                        <span style="float:left; margin-top:7px; margin-right:10px;">@Html.Raw(labels("Genius#theDay"))</span>
                        <span style="float: left;margin-right: 10px;">@(Html.DevExtreme().NumberBox().ID("txtDayOfMonth").Min(1).Max(31).Value(1).Mode(NumberBoxMode.Number).Width(100).ShowSpinButtons(True).OnValueChanged("markRowAsUpdated"))</span>
                        <span style="float: left; margin-top: 7px;margin-right:10px;">@Html.Raw(labels("Genius#ofEachMonth"))</span>
                    </div>
                    <div id="monthlyOption2" style="width:100%;float:left;">
                        <span style="float:left; margin-top:7px; margin-right:10px;">@Html.Raw(labels("Genius#the"))</span>
                        <span style="float: left;margin-right: 10px;">
                            @(Html.DevExtreme().SelectBox() _
.ID("lstWeeksOfMonth") _
.OnValueChanged("markRowAsUpdated") _
.Placeholder("Seleccionar...") _
.ShowSelectionControls(True) _
.ValueExpr("Id") _
.DisplayExpr("Text") _
.ShowClearButton(True) _
.SearchEnabled(True) _
.SearchExpr("Text") _
.DataSource(weeksOfMonth) _
.Value(0) _
.Width(200) _
                    )
                        </span>
                        <span style="float: left;margin-right: 10px;">
                            @(Html.DevExtreme().SelectBox() _
.ID("lstDaysOfWeekMonthly") _
.Placeholder("Seleccionar...") _
.ShowSelectionControls(True) _
.ValueExpr("Id") _
.OnValueChanged("markRowAsUpdated") _
.DisplayExpr("Text") _
.ShowClearButton(True) _
.SearchEnabled(True) _
.SearchExpr("Text") _
.DataSource(daysOfWeek4Month) _
.Width(200) _
.Value(0) _
                    )
                        </span>
                        <span style="float: left; margin-top: 7px;margin-right:10px;">@Html.Raw(labels("Genius#ofEachMonth"))</span>
                    </div>
                    <span style="float: left; margin-top: 7px; margin-right: 10px;">@Html.Raw(labels("Genius#at"))</span>
                    <span style="float: left;">
                        @(Html.DevExtreme().DateBox().ID("timeOfDayMonthly") _
.Type(DateBoxType.Time).Width(150) _
.OnValueChanged("markRowAsUpdated") _
            )
                    </span>
                </div>
                <div id="onceSchedule">
                    <span style="float:left; margin-top:7px; margin-right:10px;">@Html.Raw(labels("Genius#theDay"))</span>
                    <span style="float: left;">
                        @(Html.DevExtreme().DateBox().ID("selectedDateTime") _
.Width(250) _
.Type(DateBoxType.DateTime) _
.Value(Date.Today.AddDays(1)) _
.OnValueChanged("markRowAsUpdated") _
            )
                    </span>
                </div>
                <div id="intervalSchedule" style="display:none">
                    <span style="float:left; margin-top:7px; margin-right:10px;">@Html.Raw(labels("Genius#each"))</span>
                    <span style="float: left;">
                        @(Html.DevExtreme().DateBox().ID("timeOfInterval") _
.Type(DateBoxType.Time).Width(150) _
.PickerType(DateBoxPickerType.Rollers) _
.Value("00:00") _
.OnValueChanged("markRowAsUpdated") _
            )
                    </span>
                    <span style="float:left; margin-top:7px; margin-left:10px;">@Html.Raw(labels("Genius#hours"))</span>
                </div>
            </div>
        </div>
    </div>
</section>
<section id="geniusConfigScheduler">
    <div class="panHeader2 panBottomMargin">
        <span class="panelTitleSpan">
            <span id="">@Html.Raw(labels("Genius#configScheduler"))</span>
        </span>
    </div>
    <div style="min-height:15px"></div>
    <div style="width: 95%;margin: 0 auto;">
        <div style="display: flex;justify-content: center;">
            <div class="dx-field-label">
                @(Html.DevExtreme().RadioGroup() _
                                                            .ID("radioGeniusSchedulerConfig") _
                                                            .DataSource(periodTypes) _
                                    .ValueExpr("Id") _
                                            .DisplayExpr("Text") _
                                            .Value(2) _
                                .OnValueChanged("onConfigChanged") _
                                            .Layout(Orientation.Vertical) _
                )
            </div>
            <div class="dx-field-value">
                <span style="float:left; margin-top:7px; margin-right:10px;">@Html.Raw(labels("Genius#from"))</span>
                <span style="float: left;">
                    @(Html.DevExtreme().DateBox().ID("selectedFromDate") _
.Width(250) _
.Type(DateBoxType.DateTime) _
.OnValueChanged("markRowAsUpdated") _
            )
                </span>
                <span style="float: left; margin-top: 7px; margin-right: 10px; margin-left: 10px;">@Html.Raw(labels("Genius#to"))</span>
                <span style="float: left;">
                    @(Html.DevExtreme().DateBox().ID("selectedToDate") _
    .Width(250) _
.Type(DateBoxType.DateTime) _
.OnValueChanged("markRowAsUpdated") _
            )
                </span>
            </div>
        </div>
    </div>
</section>