<div style="height:95%;width:850px;" id="editView">
    <!--<input type="hidden" id="hdnEmployeesSelected" runat="server" value="" />
    <input type="hidden" id="hdnFilter" runat="server" value="" />
    <input type="hidden" id="hdnFilterUser" runat="server" value="" />-->
    <form id="selectorDestination"
          onsubmit="(event) => event.preventDefault()"
          style="height:100%;width:100%;">
        <iframe id="ifEmployeesSelector"
                runat="server"
                style="background-color:Transparent " height="100%" width="100%" scrolling="no" frameborder="0" marginheight="0" marginwidth="0"
                src="~/ReportScheduler/WebUserForms/roDestinationSelectorV2.aspx">
        </iframe>
    </form>
</div>