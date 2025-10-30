@Imports Robotics.Base.DTOs
@Imports Robotics.Web.Base

@ModelType roGuiAction
<div Class='mainActionBtn'>
    <a id='zoneRemoveBtn' href="#" title="@Model.Text">
        @(Html.DevExtreme().Button() _
            .Icon("trash") _
            .OnClick("function() { " & Model.AfterFunction & "(); }"))
    </a>
</div>