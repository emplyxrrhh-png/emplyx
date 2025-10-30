@Imports Robotics.Base.DTOs
@Imports Robotics.Web.Base

@ModelType roGuiAction
<div Class='mainActionBtn'>
    <a id='roCreateBtn' href="#" title="@Model.Text">
        @(Html.DevExtreme().Button() _
                    .Icon("plus") _
                    .OnClick("function() { " & Model.AfterFunction & "(); }"))
    </a>
</div>