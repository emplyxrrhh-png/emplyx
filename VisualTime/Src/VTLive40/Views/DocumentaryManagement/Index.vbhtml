@imports Robotics.Web.Base.API

@Code
    Layout = "~/Views/Shared/_layout.vbhtml"

    Dim scriptVersion As String = ViewData(VTLive40.Helpers.Constants.ScriptVersion)
    Dim baseURL = Url.Content("~")
    Dim DocumentaryManagementController As VTLive40.DocumentaryManagementController = New VTLive40.DocumentaryManagementController()
End Code

<script>var BASE_URL ="@baseURL"; </script>
<link href="@Url.Content("~/Content/bootstrap.min.css")" rel="stylesheet" type="text/css" />
<link href="@Url.Content("~/Base/Styles/Live/liveMVC.min.css")@Html.Raw(scriptVersion)" rel="stylesheet" type="text/css" />
<link href="@Url.Content("~/Base/Styles/Live/DocumentaryManagement/roDocumentaryManagement.min.css")@Html.Raw(scriptVersion)" rel="stylesheet" type="text/css" />

<script src="@Url.Content("~/Base/Scripts/Live/DocumentaryManagement/roDocumentaryManagementScript.min.js")@Html.Raw(scriptVersion)" type="text/javascript"></script>
<script src="@Url.Content("~/Base/globalize/dx.errors.js")@Html.Raw(scriptVersion)" type="text/javascript"></script>

@Html.Partial("~/Views/Base/BarButtons/_ButtonList.vbhtml")

<div id="divMainBody">
    <div id="divTabData" Class="divDataCells">

        <div id="divContenido" Class="divAllContent twoWindowsFlexLayout">

            @Html.Partial("~/Views/Base/CardTree/_CardTree.vbhtml")

            <div id="divButtons" class="divMVCMiddleButtons">
                @Html.Partial("~/Views/Base/TabBar/_DataTabBarButtons.vbhtml")
            </div>
            <div id="divContenido" class="twoWindowsMainPanel maxHeight divRightContent">
                <div id="saveBar" class="">
                    @Html.Partial("~/Views/Base/BarButtons/_SaveBarButtons.vbhtml")
                </div>
                <div id="mainPanelDisplay">

                    <section id="documentaryManagementUploader">
                        <form id="mainForm">

                            <div class="panHeader2 panBottomMargin">
                                <span class="panelTitleSpan">
                                    <span id="">@DocumentaryManagementController.GetServerLanguage().Translate("DocumentaryManagement", "DocumentaryManagement")</span>
                                </span>
                            </div>

                            <div style="display: flex; justify-content: flex-end; margin-bottom: 10px; margin-top: 10px;">
                                <div id="tbGlobalSearchToolbar" style="width: 60%"></div>
                            </div>

                            <div class="">
                                <div class="compFrameInside">

                                    <div id="fmDocuments"></div>
                                </div>
                                <div id="fmDocuments_limit" style="justify-content: flex-end; margin-bottom: 10px; margin-top: 10px;">
                                    <div class="dx-button-has-text">
                                        <span style="color: #FF5C35;" class="dx-button-content js-text_warning">@DocumentaryManagementController.GetServerLanguage().Translate("lblLimit", "DocumentaryManagement")</span>
                                    </div>
                                    <div class="dx-item-content dx-toolbar-item-content">
                                        <div class="dx-button dx-button-normal dx-button-mode-text dx-widget dx-button-has-icon dx-button-has-text" tabindex="0" role="button">
                                            <div id="fmDocuments_limit__button" class="dx-button-content" style="border: 1px solid #ddd; border-radius: 5px;">
                                                <i class="dx-icon dx-filemanager-i dx-icon-plus"></i>
                                                <span class="dx-button-text">@DocumentaryManagementController.GetServerLanguage().Translate("showAll", "DocumentaryManagement")</span>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </form>
                    </section>
                </div>
            </div>
        </div>
    </div>
</div>