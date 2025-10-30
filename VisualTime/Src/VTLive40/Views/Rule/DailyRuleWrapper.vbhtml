<div style="width: 1260px; height: 800px;" id="roDailyRuleEditorWrapper">
    
    <form id="roDailyRuleEditorWrapperSubmit"
          onsubmit="(event) => event.preventDefault()"
          style="height:100%;width:100%;">
        <iframe id="ifDailyRuleEditor"
                runat="server"
                style="background-color: Transparent; width: 1260px; height: 800px; border: none; overflow: hidden; " title="Rule editor"
                src="~/Shifts/Standalone/roDailyRule.aspx">
        </iframe>
    </form>
</div>