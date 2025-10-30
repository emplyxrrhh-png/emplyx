@imports System.Web.Mvc
@Code
    Dim scriptVersion As String = ViewData(VTLive40.Helpers.Constants.ScriptVersion)

    Dim deleteButtonId As String = Model.Section & "_btnDeleteEntity"
    Dim undoButtonId As String = Model.Section & "_btnUndoEntity"
    Dim saveButtonId As String = Model.Section & "_btnSaveEntity"
    Dim closeButtonId As String = Model.Section & "_btnCloseEntity"
    Dim visualizeButtonId As String = Model.Section & "_btnVisualizeEntity"
    Dim barTitle As String = Model.Section & "_spanTitle"
    Dim barTitleDefault As String = Nothing
    If Model.GetType().GetProperty("Title") IsNot Nothing AndAlso Not String.IsNullOrEmpty(Model.Title) Then
        barTitleDefault = Model.Title
    End If
    Dim barStyle As String = "defaultSaveBar"
    Dim classIconSave As String = "todo"
    If Model.GetType().GetProperty("IsHeader") IsNot Nothing AndAlso Model.IsHeader Then
        barStyle = "headerSaveBar"
        classIconSave = "return"
    End If

End Code
   

<div id="divContenidoRight" class="@Html.Raw(barStyle) twoWindowsMainPanel maxHeight divRightContent d-flex flex-row gap-3" style="padding: 10px;">
    <div class="">
        <div style="display: inline-block" class="objectName">
            <span id="@Html.Raw(barTitle)">@Html.Raw(barTitleDefault)</span>
        </div>
    </div>
    <div class="align-self-start" style="display:flex">
        <div style="display: inline-block; margin-left: 10px;">
            @(Html.DevExtreme().Button() _
                    .ID(visualizeButtonId) _
                    .Icon("eyeopen") _
                    .Visible(Model.ShowVisualize) _
                    .OnClick("visualizeEntity") _
                    .Type(ButtonType.Normal) _
                    .ElementAttr(New With {.section = Model.Section}) _
            )
        </div>
        <div style="display: inline-block; margin-left: 10px;">
            @(Html.DevExtreme().Button() _
                    .ID(deleteButtonId) _
                    .Icon("trash") _
                    .Visible(Model.ShowDelete) _
                    .OnClick("deleteEntity") _
                    .Type(ButtonType.Danger) _
                    .ElementAttr(New With {.section = Model.Section}) _
                    )
        </div>
        <div style="display: inline-block; margin-left: 10px;">
            @(Html.DevExtreme().Button() _
                    .ID(undoButtonId) _
                    .Icon("undo") _
                    .Visible(Model.ShowUndo) _
                    .OnClick("undoEntity") _
                    .Type(ButtonType.Default) _
                    .ElementAttr(New With {.section = Model.Section}) _
            )
        </div>
        <div style="display: inline-block;margin-left:10px;">
            @(Html.DevExtreme().Button() _
                            .ID(saveButtonId) _
                            .Icon(classIconSave) _
                            .Visible(Model.ShowSave) _
                            .OnClick("saveEntity") _
                            .Type(ButtonType.Success) _
                            .ElementAttr(New With {.section = Model.Section}) _
    )
        </div>
        <div style="display: inline-block;margin-left:10px;">
            @(Html.DevExtreme().Button() _
                        .ID(closeButtonId) _
                        .Icon("close") _
                        .Visible(Model.ShowClose) _
                        .OnClick("closeEntity") _
                        .Type(ButtonType.Normal) _
                        .ElementAttr(New With {.section = Model.Section}) _
    )
        </div>
    </div>
</div>