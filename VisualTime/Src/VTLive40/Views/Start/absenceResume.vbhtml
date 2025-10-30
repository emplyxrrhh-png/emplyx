@Code
    Dim oSerializer As System.Web.Script.Serialization.JavaScriptSerializer
    oSerializer = New System.Web.Script.Serialization.JavaScriptSerializer()
    Dim StartController As VTLive40.StartController = New VTLive40.StartController()
End Code
<div class="panHeaderDashboardSmall">
    @StartController.GetServerLanguage().Translate("roAbsenceTeam", "Start")
</div>

<div class="list-containerRequests">

    @(Html.DevExtreme().List() _
                                                                                                                                                                                                                                                                                                .ID("listResumeAbsence") _
                                                                                    .Height("100%") _
                                                                                    .DataSource(Function(ds)
                                                                                                    Return ds.Mvc() _
                                                                                        .Controller("Start") _
            .Key("ImageSrc") _
                                                                                        .LoadAction("GetAbsenceResume")
                                                                                                End Function) _
                                                                                                .PageLoadingText(StartController.GetServerLanguage().Translate("roStartLoading", "Start")) _
                                                                                                .NoDataText(StartController.GetServerLanguage().Translate("roNoData", "Start")) _
                                                                                                .FocusStateEnabled(False) _
                                                                        .SelectionMode(ListSelectionMode.Single) _
                                                                        .ShowSelectionControls(True) _
                                                                        .ActiveStateEnabled(False) _
                                                                        .OnSelectionChanged("filterGridAbsences") _
                                                .OnItemClick("restoreFiltersAbsences") _
                                                                                                .ItemTemplate("<div class='requestDB' style='cursor: default;'><img src=' <%- ImageSrc %>' height='32'><div style='cursor: default;'><%- Description %></div><div class='image' style='cursor: default;'><%- Count %></div></div>")
    )
</div>