@Code
    Dim oSerializer As System.Web.Script.Serialization.JavaScriptSerializer
    oSerializer = New System.Web.Script.Serialization.JavaScriptSerializer()
    Dim OnBoardingController As VTLive40.OnBoardingController = New VTLive40.OnBoardingController()
End Code
<div class="panHeader4">
    @OnBoardingController.GetServerLanguage().Translate("roOnBoardingTasksCopy", "OnBoarding")
</div>

<div class="list-containerRequests">
    <br />

    <div class="configField" style="display: flex;justify-content: center;">
        <div id="employeeStatus" style="width:50%; margin-right:20px;">
            @(Html.DevExtreme().Autocomplete() _
                            .ID("EmployeeTextCopy") _
                            .Placeholder(OnBoardingController.GetServerLanguage().Translate("roSelectUser", "Start")) _
                            .ValueExpr("EmployeeName") _
                            .SearchExpr("EmployeeName") _
                            .DataSource(ViewBag.AvailableEmployees) _
                )
        </div>
        <div id="employeeStatus" style="">
            @(Html.DevExtreme().Button() _
            .ID("CopyTasksFromUser") _
            .Icon("check") _
            .Type(ButtonType.Normal) _
            )
        </div>
    </div>

    <div style="margin: 15px;position: absolute; bottom: 0;right: 0;">
        @(Html.DevExtreme().Button() _
            .Icon("save") _
            .Type(ButtonType.Default) _
            )
    </div>
</div>