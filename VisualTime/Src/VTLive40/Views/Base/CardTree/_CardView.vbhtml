@Imports Robotics.DTOs
@Imports VTLive40.Helpers
@Code

    Dim BaseIcon As String = ViewData("CardBaseIcon")
    Dim SelectedCardId As String = If(ViewData("SelectedCardId") IsNot Nothing, ViewData("SelectedCardId"), "").ToString
    Dim cardviewData = ViewData(Constants.DefaultCardTreeData)
    Dim cardDataType As String = ViewData(Constants.DefaultCardTreeType)

End Code

@Html.DevExpress().CardView(Sub(settings)
                                     settings.Name = "CardView"

                                     settings.Width = Unit.Percentage(100)
                                     settings.Styles.Card.CssClass = "CardsTree-card"

                                     settings.SettingsPager.Mode = GridViewPagerMode.ShowAllRecords
                                     settings.SettingsPager.SettingsTableLayout.ColumnCount = 1
                                     settings.Settings.VerticalScrollBarMode = 2
                                     settings.EnableCardsCache = False

                                     settings.SettingsSearchPanel.Visible = True
                                     settings.SettingsSearchPanel.Delay = 300
                                     settings.SettingsPager.EnableAdaptivity = True

                                     settings.CallbackRouteValues = New With {Key .Controller = "Base", Key .Action = "CardView", Key .cardTreeType = cardDataType}

                                     settings.ClientSideEvents.CardClick = "function(s,e){ loadRequest(); }"
                                     settings.ClientSideEvents.CardDblClick = "function(s,e){ openReportConfigurationPopUp(); }"
                                     settings.ClientSideEvents.EndCallback = "function(s,e){ reloadCardsPanel(); }"
                                     settings.ClientSideEvents.Init = "function(s,e){if (typeof selectCardById !== 'undefined') selectCardById('" & SelectedCardId & "'); }"
                                     settings.Columns.Add("Id")
                                     settings.Columns.Add("Name")
                                     settings.Columns.Add("CreatedOn")
                                     settings.Columns.Add("Description")
                                     'settings.Columns.Add("Icon")
                                     settings.Columns.Add("Filterfield")
                                     settings.Columns.Add("Filterfield2")
                                     settings.SetCardTemplateContent(Sub(c)
                                                                         ViewContext.Writer.Write(
                                                                                            "<div class='cardsTree-CardIcon' style='background:url(" &
                                                                                             If(DataBinder.Eval(c.DataItem, "Icon"), BaseIcon) &
                                                                                            ")'></div>" &
                                                                                         "<div class='cardsTree-CardInfo' data-card-id='" & DataBinder.Eval(c.DataItem, "Id") & "' data-card-filterField='" & DataBinder.Eval(c.DataItem, "Filterfield") & "' data-card-filterField2='" & DataBinder.Eval(c.DataItem, "Filterfield2") & "' data-card-issystem='" & DataBinder.Eval(c.DataItem, "IsSystem") & "'>" &
                                                                                             "<h3 class='cardName'>" &
                                                                                                 HttpContext.Current.Server.HtmlEncode(DataBinder.Eval(c.DataItem, "Name")) &
                                                                                             "</h3>" &
                                                                                             If(DataBinder.Eval(c.DataItem, "Description") IsNot Nothing AndAlso Not DataBinder.Eval(c.DataItem, "Description").ToString.Equals(""), "<div class='cardsTree-Description'><span class='cardsTree-DescriptionText description'>" & If(DataBinder.Eval(c.DataItem, "AllowHtml"), DataBinder.Eval(c.DataItem, "Description"), HttpContext.Current.Server.HtmlEncode(DataBinder.Eval(c.DataItem, "Description"))) & "</span></div>", "") &
                                                                                         "</div>" &
                                                                                         "<ul class='cardsTree-Historic'>" &
                                                                                            "<li>" &
                                                                                                "<span>" &
                                                                                                    If(CType(DataBinder.Eval(c.DataItem, "CreatedOn"), DateTime).ToString("dd-MM-yyyy").Equals("01-01-0001"), String.Empty, CType(DataBinder.Eval(c.DataItem, "CreatedOn"), DateTime).ToString("dd-MM-yyyy")) &
                                                                                                "</span>" &
                                                                                             "</li>" &
                                                                                         "</ul>")
                                                                     End Sub)
                                 End Sub).Bind(cardviewData).GetHtml()