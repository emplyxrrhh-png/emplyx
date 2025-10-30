Imports Robotics.Base.DTOs
Imports Robotics.Web.Base

Partial Class SecurityOptions
    Inherits PageBase

    Private Const FeatureAlias As String = "Administration.SecurityOptions"

#Region "Declarations"

    Private oPermission As Permission

#End Region

#Region "Events"

    Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        Me.InsertExtraJavascript("Ajax", "~/Base/Scripts/Ajax.js")
        Me.InsertExtraJavascript("jsDate", "~/Base/Scripts/jsDate.js")
        Me.InsertExtraJavascript("GenericData", "~/Base/Scripts/GenericData.js")
        Me.InsertExtraJavascript("webUserForms", "~/Base/Scripts/webUserForms.js")
        Me.InsertExtraJavascript("jsGrid", "~/Base/Scripts/jsGrid.js")
        Me.InsertExtraJavascript("roOptionPanelClient", "~/Base/Scripts/roOptionPanelClient.js")
        Me.InsertExtraJavascript("roOptPanelClientGroup", "~/Base/Scripts/roOptPanelClientGroup.js")
        Me.InsertExtraJavascript("jsDatePicker", "~/Base/Scripts/jsDatePicker.js")
        Me.InsertExtraJavascript("roTabContainerClient", "~/Base/Scripts/roTabContainerClient.js")
        Me.InsertExtraJavascript("securityOptions", "~/Security/Scripts/SecurityOptions.js")
        Me.InsertExtraJavascript("securityOptionsData", "~/Security/Scripts/SecurityOptionsData.js")
        Me.InsertExtraJavascript("securityIPeditor", "~/Security/Scripts/SecurityIPEditor.js")
    End Sub

    Protected Sub Page_Load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.lblSaveChanges.InnerText = Me.Language.Translate("msgSaveChangesbar", DefaultScope)
        Me.lblSaveChangesBottom.InnerText = Me.lblSaveChanges.InnerText
        Me.lblUndoChanges.InnerText = Me.Language.Translate("msgUndoChangesbar", DefaultScope)
        Me.lblUndoChangesBottom.InnerText = Me.lblUndoChanges.InnerText

        Me.msgHasChanges.Value = Me.Language.Translate("msgHasChanges", DefaultScope)
        Me.msgHasErrors.Value = Me.Language.Translate("msgHasErrors", DefaultScope)

        ' Si el passport actual no tiene permisso de lectura, rediriguimos a pàgina de acceso denegado
        Me.oPermission = Me.GetFeaturePermission(FeatureAlias)

        Me.hdnValueGridName.Value = Me.Language.Translate("GridIps.NameValue", DefaultScope)
        If Me.oPermission = Permission.None Then
            WLHelperWeb.RedirectAccessDenied(False)
            Exit Sub
        End If

        If Not Me.IsPostBack Then
            Me.SetPermissions()
        End If

    End Sub

#End Region

#Region "Methods"

    Private Sub SetPermissions()

        If Me.oPermission = Permission.None Then
            Me.TABBUTTON_PasswordOptions.Style("display") = "none"
            Me.TABBUTTON_RestrictionsOptions.Style("display") = "none"
            Me.tbPasswordOptions.Visible = False
            Me.tbRestrictionsOptions.Visible = False
            Me.SecurityOptions_TabVisibleName.Value = ""
        ElseIf Me.oPermission < Permission.Write Then
            Me.DisableControls(Me.tbPasswordOptions.Controls)
            Me.DisableControls(Me.tbRestrictionsOptions.Controls)
        End If

    End Sub

#End Region

End Class