@Imports Robotics.Base.DTOs
@Imports Robotics.Web.Base

@ModelType roGuiAction
<div Class='mainActionBtn'>
    <a href="#" title="@Model.Text" class="ButtonBarVertical @Model.CssClass" onclick="@Model.AfterFunction">

        @*@(Html.DevExtreme().Button() _
                                     .Text(Model.LanguageTag) _
                                     .ElementAttr("class", "dynamicContainedButton") _
                         .Type(ButtonType.Normal) _
                         .StylingMode(ButtonStylingMode.Contained) _
                         .OnClick("function() { " & Model.AfterFunction & "(); }")
            )*@
    </a>
</div>