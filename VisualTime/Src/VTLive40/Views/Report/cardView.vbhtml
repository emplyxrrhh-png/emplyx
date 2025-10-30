@imports Newtonsoft.Json.Linq

@Code
    Dim ReportController As VTLive40.ReportController = New VTLive40.ReportController()
    Dim baseURL = Url.Content("~")
    Dim PlanificationIcon = baseURL & "Base/Images/StartMenuIcos/Events.png"
End Code

@Html.DevExpress().CardView(Sub(settings)
                                     settings.Name = "CardView"

                                     settings.Width = Unit.Percentage(100)
                                     settings.Styles.Card.CssClass = "reportCard"
                                     'settings.Styles.Card.CssClass = "reportCard" & If(DataBinder.Eval(, "IsEmergencyReport") = True, " isEmergencyReport", "")

                                     settings.SettingsPager.Mode = GridViewPagerMode.ShowAllRecords
                                     settings.SettingsPager.SettingsTableLayout.ColumnCount = 1
                                     settings.Settings.VerticalScrollBarMode = 2
                                     settings.EnableCardsCache = False

                                     settings.SettingsSearchPanel.Visible = True
                                     settings.SettingsSearchPanel.Delay = 500
                                     settings.SettingsPager.EnableAdaptivity = True

                                     settings.CallbackRouteValues = New With {Key .Controller = "report", Key .Action = "CardView"}
                                     settings.ClientSideEvents.EndCallback = "function(s,e){reloadCardsPanel();}"

                                     settings.Columns.Add("Id").Caption = "id"
                                     settings.Columns.Add("Name").Caption = "name"
                                     settings.Columns.Add("DisplayName").Caption = "DisplayName"
                                     settings.Columns.Add("LastExecution.ExecutionDate").Caption = "date"
                                     settings.Columns.Add("LastExecution.Status").Caption = "status"
                                     settings.Columns.Add("Description").Caption = "desc"
                                     'settings.Columns.Add("CategoriesList").Caption = "cat"
                                     'settings.Columns.Add("CategoriesList.Description").Caption = "cat"
                                     settings.Columns.Add("CategoriesDescription").Caption = "cat"

                                     settings.SetCardTemplateContent(Sub(c)
                                                                         ViewContext.Writer.Write(
                                                                                            "<div class='" &
                                                                                            If(DataBinder.Eval(c.DataItem, "CreatorPassportId") IsNot Nothing, "reportCardFavIconClient ", "reportCardFavIconRobotics ") &
                                                                                            "' style='background-image:url(" &
                                                                                            If(DataBinder.Eval(c.DataItem, "CreatorPassportId") IsNot Nothing, Url.Content("~/Base/Images/StartMenuIcos/ReportScheduler.png"), If(DataBinder.Eval(c.DataItem, "IsEmergencyReport") = True, Url.Content("~/Base/Images/EmergencyPrintNew30.png"), Url.Content("~/Base/Images/Toolbar/LogoVisualTimeLive.png"))) &
                                                                                            "); background-size: contain;background-repeat: no-repeat;border-radius:5px;'></div>" &
                                                                                         "<div class='reportCardInfo' data-report-categories='" &
                                                                                         If(DataBinder.Eval(c.DataItem, "CategoriesArray") IsNot Nothing, Join(CType(DataBinder.Eval(c.DataItem, "CategoriesArray"), Robotics.Base.DTOs.roSecurityCategory()).Select(Of String)(Function(Cat) Cat.Description).ToArray, ", "), "") &
                                                                                         "' data-report-id='" & DataBinder.Eval(c.DataItem, "Id") & "' data-report-catSearch='" & DataBinder.Eval(c.DataItem, "CategoriesDescription") & "'>" &
                                                                                             "<h3>" &
                                                                                                 HttpContext.Current.Server.HtmlEncode(DataBinder.Eval(c.DataItem, "DisplayName")) &
                                                                                             "</h3>" &
                                                                                             If(Not DataBinder.Eval(c.DataItem, "Description").ToString.Equals(""),
                                                                                                 "<div class='reportDescription'>" &
                                                                                                     "<span class='descriptionText'>" &
                                                                                                         HttpContext.Current.Server.HtmlEncode(DataBinder.Eval(c.DataItem, "Description")) &
                                                                                                     "</span>" &
                                                                                                 "</div>", "") &
                                                                                         "</div>" &
                                                                                         "<ul class='hasPlanner'>" &
                                                                                             "<li id='imagePlanReport" & DataBinder.Eval(c.DataItem, "Id") & "' >" &
                                                                                              If(DataBinder.Eval(c.DataItem, "HasExecutionsPlanned"), "<img src='" & PlanificationIcon & "'  width='16px' readonly>", "") &
                                                                                        " </li></ul><ul class='reportsHistoric'>" &
                                                                                             "<li class='lastExecution'" &
                                                                                                 " data-status='" & DataBinder.Eval(c.DataItem, "LastExecution.Status") & "'" &
                                                                                                 " data-guid='" & DataBinder.Eval(c.DataItem, "LastExecution.BlobLink") & "'" &
                                                                                             "><span>" &
                                                                                                 If(DataBinder.Eval(c.DataItem, "LastExecution.ExecutionDate") IsNot Nothing, CType(DataBinder.Eval(c.DataItem, "LastExecution.ExecutionDate"), DateTime).ToString("dd-MM-yyyy"), ReportController.GetServerLanguage().Translate("roNewReportNoExecutions", "ReportsDX")) &
                                                                                              "</span></li>" &
                                                                                         "</ul>")
                                                                         'c.CardView.CssClass = "OMG" 'If(DataBinder.Eval(c.DataItem, "IsEmergencyReport") = True, "isEmergencyReport", "")
                                                                     End Sub)
                                 End Sub).Bind(Model).GetHtml()