@Code
    Dim oSerializer As System.Web.Script.Serialization.JavaScriptSerializer
    oSerializer = New System.Web.Script.Serialization.JavaScriptSerializer()
    Dim OnBoardingController As VTLive40.OnBoardingController = New VTLive40.OnBoardingController()
End Code
<div class="panHeader4">
    @OnBoardingController.GetServerLanguage().Translate("roOnBoardingNew", "OnBoarding")
</div>

<div class="list-containerRequests">
    <br />
    <div style="width: 95%;margin: 0 auto;">
        <div class="configField" style="display: flex;justify-content: center;">
            <div class="dx-field-label">
                @OnBoardingController.GetServerLanguage().Translate("roOnBoardingFor", "OnBoarding")
            </div>
            <div class="dx-field-value">
                @(Html.DevExtreme().Autocomplete() _
    .ID("EmployeeText") _
    .Placeholder(OnBoardingController.GetServerLanguage().Translate("roSelectUser", "Start")) _
.MaxItemCount(1000) _
    .ValueExpr("EmployeeName") _
.ShowClearButton(True) _
    .SearchExpr("EmployeeName") _
.OnSelectionChanged("employeeSelected") _
.DataSource(ViewBag.AvailableEmployees) _
                )
            </div>
        </div>
        <br />
        <div class="configField" style="display: flex;justify-content: center;">
            <div class="dx-field-label">
                @OnBoardingController.GetServerLanguage().Translate("roOnBoardingDay", "OnBoarding")
            </div>
            <div class="dx-field-value">
                @(Html.DevExtreme().DateBox() _
.ID("dateSelector") _
.DisplayFormat("dd/MM/yyyy") _
                    .Type(DateBoxType.Date) _
.OnValueChanged("dateSelected") _
                    .Value(DateTime.Now) _
            )
            </div>
        </div>
    </div>
    <br />
    <br />
    <div class="panHeaderListOptional">
        @OnBoardingController.GetServerLanguage().Translate("roOnBoardingOptional", "OnBoarding")
    </div>
    <br />
    <div style="width: 95%;  margin: 0 auto;">
        <div class="configField" style="display: flex;justify-content: center;">
            <div class="dx-field-label">
                @OnBoardingController.GetServerLanguage().Translate("roOnBoardingCopy", "OnBoarding")
            </div>
            <div class="dx-field-value">
                @(Html.DevExtreme().Autocomplete() _
                                                                                            .ID("EmployeeTextCopy") _
                                                                                            .Placeholder(OnBoardingController.GetServerLanguage().Translate("roSelectUser", "Start")) _
                                                                                            .ValueExpr("EmployeeName") _
                                                    .OnSelectionChanged("copySelected") _
                .ShowClearButton(True) _
                .MaxItemCount(1000) _
                                                                                .SearchExpr("EmployeeName") _
                                                                                .DataSource(ViewBag.AlreadyUsedEmployees) _
                )
            </div>
        </div>
    </div>

    <div style="margin: 15px;position: absolute; bottom: 0;right: 0;">
        @(Html.DevExtreme().Button() _
                .ID("addUser") _
                                    .Icon("save") _
                        .OnClick("addNewOnBoarding") _
                                    .Type(ButtonType.Default) _
            )
    </div>
</div>