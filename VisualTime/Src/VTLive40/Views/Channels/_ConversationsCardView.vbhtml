@Imports Robotics.Base.DTOs
@Imports VTLive40.Helpers

@Code
    Dim labels = ViewData(VTLive40.Helpers.Constants.DefaultLanguagesEntries)
    Dim baseURL = Url.Content("~")

    Dim BaseIcon As String = ViewData("CardBaseIcon")
    Dim cardviewData = ViewData("ChannelConversation")
    Dim cardDataType As String = "Conversations"
    Dim SelectedCardId As String = If(ViewData("SelectedCardId") IsNot Nothing, ViewData("SelectedCardId"), "").ToString
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

                                     settings.CallbackRouteValues = New With {Key .Controller = "Channels", Key .Action = "ConversationsCardView", Key .idChannel = SelectedCardId}

                                     settings.ClientSideEvents.CardClick = "function(s,e){ loadConversation(); }"
                                     settings.ClientSideEvents.CardDblClick = "function(s,e){ loadConversation(); }"
                                     settings.ClientSideEvents.EndCallback = "function(s,e){ reloadCardsPanel(); }"
                                     settings.ClientSideEvents.Init = "function(s,e){if (typeof selectCardById !== 'undefined') selectCardById('-1'); }"
                                     settings.Columns.Add("Id")
                                     settings.Columns.Add("Name")
                                     settings.Columns.Add("CreatedOn")
                                     settings.Columns.Add("Description")
                                     'settings.Columns.Add("Icon")
                                     settings.Columns.Add("Filterfield")
                                     settings.Columns.Add("Filterfield2")
                                     settings.Columns.Add("StatusText")
                                     settings.SetCardTemplateContent(Sub(c)
                                                                         ViewContext.Writer.Write(
                                                                                         "<div class='cardsTree-CardIcon conversationIcon' style='background:url(" &
                                                                                             If(DataBinder.Eval(c.DataItem, "Icon"), BaseIcon) &
                                                                                         ")'></div>" &
                                                                                         "<div class='cardsTree-CardInfo conversationInfo' data-card-id='" & DataBinder.Eval(c.DataItem, "Id") & "' data-card-filterField='" & DataBinder.Eval(c.DataItem, "Filterfield") & "' data-card-filterField2='" & DataBinder.Eval(c.DataItem, "Filterfield2") & "' data-card-issystem='" & DataBinder.Eval(c.DataItem, "IsSystem") & "'>" &
                                                                                             "<h3 class='cardName'>" &
                                                                                                 HttpContext.Current.Server.HtmlEncode(DataBinder.Eval(c.DataItem, "Name")) &
                                                                                             "</h3>" &
                                                                                             If(DataBinder.Eval(c.DataItem, "Description") IsNot Nothing AndAlso Not DataBinder.Eval(c.DataItem, "Description").ToString.Equals(""), "<div class='cardsTree-Description conversationDescription'><span class='cardsTree-DescriptionText description'>" & HttpContext.Current.Server.HtmlEncode(DataBinder.Eval(c.DataItem, "Description")) & "</span></div>", "") &
                                                                                         "</div>" &
                                                                                         "<div id='conversationInfo" & DataBinder.Eval(c.DataItem, "Id") & "' class='conversationDate'>" &
                                                                                            "<div style='" & If(CInt(DataBinder.Eval(c.DataItem, "Status")) < 2 AndAlso CInt(DataBinder.Eval(c.DataItem, "Badge")) > 0, "", "display:none") & "' class='csBadgeSelector conversationBadge'>" & DataBinder.Eval(c.DataItem, "Badge") & "</div>" &
                                                                                            "<div style='" & If(CInt(DataBinder.Eval(c.DataItem, "Status")) > 1 OrElse CInt(DataBinder.Eval(c.DataItem, "Badge")) = 0, "", "display:none") & "' class='conversationState conversationState" & DataBinder.Eval(c.DataItem, "Status") & "'>" & DataBinder.Eval(c.DataItem, "StatusText") & "</div>" &
                                                                                            "<div class='lastMessage' style='' >" &
                                                                                                 "<span>" &
                                                                                                     If(CType(DataBinder.Eval(c.DataItem, "CreatedOn"), DateTime).ToString("dd-MM-yyyy").Equals("01-01-0001"), String.Empty, CType(DataBinder.Eval(c.DataItem, "CreatedOn"), DateTime).ToString("dd-MM-yyyy HH:mm")) &
                                                                                                 "</span>" &
                                                                                            "</div>" &
                                                                                         "</divl>")
                                                                     End Sub)
                                 End Sub).Bind(cardviewData).GetHtml()