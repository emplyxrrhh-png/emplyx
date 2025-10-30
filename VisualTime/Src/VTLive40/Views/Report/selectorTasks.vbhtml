<div id="editView">
    <form id="selectorTasks"
          onsubmit="(event) => event.preventDefault()"
          style="height:100%;width:100%;">
        <input type="hidden" id="hdnTasksSelected" runat="server" value="" />
        <iframe id="ifTaskSelector" name="advSelectorContainer"
                runat="server"
                style="background-color:Transparent " height="100%" width="100%" scrolling="no" frameborder="0" marginheight="0" marginwidth="0"
                src="~/Base/WebUserControls/roTreeTaskContainer.aspx?PrefixCookie=objContainerTreeV3_treeTaskReportProfile&AfterSelectFuncion=parent.GetSelectedTreeTask">
        </iframe>
    </form>
</div>