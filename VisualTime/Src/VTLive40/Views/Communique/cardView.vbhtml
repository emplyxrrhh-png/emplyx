@Imports Robotics.Base.DTOs
@Code
    Dim CommuniqueController As VTLive40.CommuniqueController = New VTLive40.CommuniqueController()
End Code

@Html.DevExpress().CardView(Sub(settings)
                                     settings.Name = "CardView"

                                     settings.Width = Unit.Percentage(100)
                                     settings.Styles.Card.CssClass = "communiqueCard"

                                     settings.SettingsPager.Mode = GridViewPagerMode.ShowAllRecords
                                     settings.SettingsPager.SettingsTableLayout.ColumnCount = 1
                                     settings.Settings.VerticalScrollBarMode = 2
                                     settings.EnableCardsCache = False

                                     settings.SettingsSearchPanel.Visible = True
                                     settings.SettingsSearchPanel.Delay = 300
                                     settings.SettingsPager.EnableAdaptivity = True

                                     settings.CallbackRouteValues = New With {Key .Controller = "Communique", Key .Action = "CardView"}
                                     settings.ClientSideEvents.EndCallback = "function(s,e){reloadCardsPanel();}"

                                     'settings.Columns.Add("CreatorPassportId").Caption = "CreatorPassportId"
                                     'settings.Columns.Add("Id").Caption = "Id"
                                     settings.Columns.Add("Subject").Caption = "Subject"
                                     settings.Columns.Add("CreatedOn").Caption = "Date"
                                     'settings.Columns.Add("Status.Reads").Caption = "reads"
                                     settings.Columns.Add("Message").Caption = "Message"

                                     settings.SetCardTemplateContent(Sub(c)
                                                                         ViewContext.Writer.Write(
                                                                                            "<div class='comm-CardFavIconRobotics'><i style='font-size:25px' class='" &
                                                                                             If(DataBinder.Eval(c.DataItem, "Status").ToString.Equals("Draft"), "dx-icon dx-icon-edit", If(DataBinder.Eval(c.DataItem, "Status").ToString.Equals("Online"), "dx-icon dx-icon-message", If(DataBinder.Eval(c.DataItem, "Status").ToString.Equals("Cancelled"), "dx-icon dx-icon-close", "dx-icon dx-icon-clock"))) &
                                                                                            "'></i></div>" &
                                                                                         "<div class='communiqueCardInfo' data-communique-id='" & DataBinder.Eval(c.DataItem, "Id") & "'>" &
                                                                                             "<h3>" &
                                                                                                 HttpContext.Current.Server.HtmlEncode(DataBinder.Eval(c.DataItem, "Subject")) &
                                                                                             "</h3>" &
                                                                                             If(Not DataBinder.Eval(c.DataItem, "Message").ToString.Equals(""),
                                                                                                 "<div class='reportDescription'>" &
                                                                                                     "<span class='descriptionText'>" &
                                                                                                     "</span>" &
                                                                                                 "</div>",
                                                                                                 ""
                                                                                             ) &
                                                                                         "</div>" &
                                                                                         "<ul class='reportsHistoric'>" &
                                                                                             "<li class='lastExecution'>" &
                                                                                             "<span>" &
                                                                                                 If(DataBinder.Eval(c.DataItem, "SentOn").ToString.Contains("1970") Or DataBinder.Eval(c.DataItem, "SentOn").ToString.Contains("0001"), CType(DataBinder.Eval(c.DataItem, "CreatedOn"), DateTime).ToString("dd-MM-yyyy"), CType(DataBinder.Eval(c.DataItem, "SentOn"), DateTime).ToString("dd-MM-yyyy")) &
                                                                                              "</span><i style='font-size:20px; display:block; vertical-align: sub; color: #0046FE' class='" &
                                                                                                 If(CType(DataBinder.Eval(c.DataItem, "Documents"), System.Array).Length.Equals(0), "", "dx-icon dx-icon-docfile") &
                                                                                              "'></i>" &
                                                                                             "</li>" &
                                                                                         "</ul>")
                                                                     End Sub)
                                 End Sub).Bind(Model).GetHtml()