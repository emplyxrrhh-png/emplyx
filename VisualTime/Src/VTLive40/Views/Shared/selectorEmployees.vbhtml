@imports Robotics.Base

@Code
    Dim Employees As String = "Employees"
    If ViewBag.RequieredFeature IsNot Nothing Then
        Employees = ViewBag.RequieredFeature
    Else
        If Model IsNot Nothing Then
            Employees = CType(Model, Report).RequieredFeature
        End If
    End If

End Code

<div style="height:470px;width:650px;" id="editView">
    <form id="selectorEmployees"
          onsubmit="(event) => event.preventDefault()"
          style="height:100%;width:100%;">
        <iframe id="ifEmployeesSelector" name="advSelectorContainer"
                runat="server"
                style="background-color:Transparent " height="100%" width="100%" scrolling="no" frameborder="0" marginheight="0" marginwidth="0"
                src="~/Base/WebUserControls/roWizardSelectorContainerMultiSelectV3.aspx?PrefixTree=treeEmployeesReportExecution&FeatureAlias=@Html.Raw(Employees)&PrefixCookie=objContainerTreeV3_treeEmployeesReportExecutionGrid&AfterSelectFuncion=parent.collectParamValues">
        </iframe>
    </form>
</div>