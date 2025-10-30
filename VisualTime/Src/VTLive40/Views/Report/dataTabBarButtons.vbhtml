@imports Robotics.Web.Base.API

@Code
    Dim ReportController As VTLive40.ReportController = New VTLive40.ReportController()
    Dim reportManagerPermissionsByUser = SecurityServiceMethods.GetPermissionOverFeature(Nothing, "Reports", "U")
    Dim baseURL = Url.Content("~")
    Dim CommuniqueIcon = baseURL & "Base/Images/StartMenuIcos/Communique.png"
    Dim ShrinkIcon = baseURL & "Base/Images/tbt/maximize32.png"
    Dim ExpandIcon = baseURL & "Base/Images/tbt/minimize32.png"
    Dim NewIcon = baseURL & "Base/Images/tbt/ReportSchedulerSupervisor.png"
    Dim DeleteIcon = baseURL & "Base/Images/tbt/delete32.png"

    Dim CloneIcon = baseURL & "Base/Images/tbt/copyTask32.png"
    Dim EditIcon = baseURL & "Base/Images/tbt/Edit32.png"
    Dim PlanificationIcon = baseURL & "Base/Images/StartMenuIcos/Events.png"

End Code

<div id="divBarButtons" class="maxHeight">
    <div id="divBarButtons" class="maxHeight">
        <div class="middleBarButtonsMain">
            <div style="height:50%;vertical-align:top;">
                <a class='reportActionBtn' id='treeShrinkBtn' href="#" title="@ReportController.GetServerLanguage().Translate("roReportBtnHide", "ReportsDX")">
                    <img src="@ShrinkIcon" alt="@ReportController.GetServerLanguage().Translate("roReportBtnHide", "ReportsDX")." />
                </a>
                <a class='reportActionBtn' id='treeExpandBtn' href="#" title="@ReportController.GetServerLanguage().Translate("roReportBtnShow", "ReportsDX")" style="display:none">
                    <img src="@ExpandIcon" alt="@ReportController.GetServerLanguage().Translate("roReportBtnShow", "ReportsDX")." />
                </a>
                @If reportManagerPermissionsByUser >= 9 Then
                @<a Class='reportActionBtn' id='reportCreateBtn' href="javascript:openDesignerLink()" title="@ReportController.GetServerLanguage().Translate("roReportBtnNew", "ReportsDX")">
                    <img src="@NewIcon" alt="@ReportController.GetServerLanguage().Translate("roReportBtnNew", "ReportsDX")." />
                </a>
                @<a Class='reportActionBtn' id='reportRemoveBtn' href="#" title="@ReportController.GetServerLanguage().Translate("roReportBtnRemove", "ReportsDX")" style="display:none">
                    <img src="@DeleteIcon" alt="@ReportController.GetServerLanguage().Translate("roReportBtnRemove", "ReportsDX")." />
                </a>
                @<a Class='reportActionBtn' id='reportEditBtn' href="javascript:openDesignerLink()" title="@ReportController.GetServerLanguage().Translate("roReportBtnEdit", "ReportsDX")" style="display:none">
                    <img src="@EditIcon" alt="@ReportController.GetServerLanguage().Translate("roReportBtnEdit", "ReportsDX")." />
                </a>
                @<a Class='reportActionBtn' id='reportCloneBtn' href="#" title="@ReportController.GetServerLanguage().Translate("roReportBtnClone", "ReportsDX")" style="display:none">
                    <img src="@CloneIcon" alt="@ReportController.GetServerLanguage().Translate("roReportBtnClone", "ReportsDX")." />
                </a>
                End If
                @If reportManagerPermissionsByUser >= 6 Then
                @<a Class='reportActionBtn' id='reportAddPlanificationBtn' href="#" title="@ReportController.GetServerLanguage().Translate("roReportBtnNewPlanification", "ReportsDX")" style="display:none">
                    <img src="@PlanificationIcon" alt="@ReportController.GetServerLanguage().Translate("roReportBtnNewPlanification", "ReportsDX")." />
                </a>
                End If
            </div>
            <div style="height:50%;display:flex;flex-direction:column;justify-content:flex-end;"></div>
        </div>
    </div>
</div>