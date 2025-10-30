Imports Robotics.Base.DTOs
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions
Imports Robotics.Web.Base
Imports Robotics.Web.Base.API

Partial Class AdvancedSecurity
    Inherits PageBase

    Private Const FeatureAlias As String = "Administration.Security"

#Region "Declarations"

    Private oPermission As Permission
    Private oParameters As roParameters

#End Region

#Region "Events"

    Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        Me.InsertExtraJavascript("Options", "~/Options/Scripts/Options.js")
        Me.InsertExtraJavascript("frmAddRoute", "~/Options/Scripts/frmAddRoute.js")
        Me.InsertExtraJavascript("webUserForms", "~/Base/Scripts/webUserForms.js")
        Me.InsertExtraJavascript("jsGrid", "~/Base/Scripts/jsGrid.js")

        Me.InsertExtraJavascript("lockDB", "~/Security/Scripts/AdvancedSecurity.js")
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

        If Me.oPermission = Permission.None Then
            WLHelperWeb.RedirectAccessDenied(False)
            Exit Sub
        End If

        If Me.oPermission <= Permission.Read Then
            Me.hdnModeEdit.Value = "0"
        Else
            Me.hdnModeEdit.Value = "1"
        End If

        If Not Me.IsPostBack Then
            Me.LoadOptions()
            Me.SetPermissions()
        Else

            Me.oParameters = Session("ConfigurationOptions_Parameters")
        End If

        Try

            If Request.Form("__EVENTTARGET") IsNot Nothing Then
                If Request.Form("__EVENTTARGET").EndsWith("btSave") Then
                    Me.btSave_Click(Me.btSave, Nothing)
                ElseIf Request.Form("__EVENTTARGET").EndsWith("btCancel") Then
                    Me.btCancel_Click(Me.btCancel, Nothing)
                End If
            End If
        Catch ex As Exception
        End Try

    End Sub

    Protected Sub btSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btSave.Click

        Dim strKeyMsg As String = ""
        If Me.oPermission >= Permission.Write Then
            If Me.CheckData(strKeyMsg) Then
                Dim bolSaved As Boolean = False

                'BlockUser
                Dim iBlockUser As Integer = 0
                If Me.chkBlockUser.Checked Then
                    iBlockUser = 1
                End If
                Dim iBlockUserPeriod As Integer = roTypes.Any2Integer(Me.txtBlockUserPeriod.Value)

                'ShowLegalText
                Dim iShowText As Integer = 0
                If Me.chkShowText.Checked Then
                    iShowText = 1
                End If

                bolSaved = API.SecurityServiceMethods.SaveBlockUserByInactivity(Me, iBlockUser, iBlockUserPeriod) And API.SecurityServiceMethods.SaveShowLegalText(Me, iShowText)
                If bolSaved Then
                    Me.hdnChanged.Value = "0"
                    Me.LoadOptions()
                Else
                    Me.hdnChanged.Value = "1"
                End If
            Else
                HelperWeb.ShowMessage(Me, Me.Language.Translate(strKeyMsg & ".Title", Me.DefaultScope), Me.Language.Translate(strKeyMsg & ".Description", Me.DefaultScope), , , , HelperWeb.MsgBoxIcons.AlertIcon)
                Me.hdnChanged.Value = "1"
            End If
        Else
            WLHelperWeb.RedirectAccessDenied(False)
        End If

    End Sub

    Protected Sub btCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btCancel.Click

        Me.hdnChanged.Value = "0"
        Me.LoadOptions()
    End Sub

    Protected Sub btRefresh_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btRefresh.Click
        Me.hdnChanged.Value = "1"
    End Sub

#End Region

#Region "Methods"

    Private Sub LoadOptions()

        'Settings de la base de datos
        Me.oParameters = ConnectorServiceMethods.GetParameters(Me)
        Session.Remove("ConfigurationOptions_Parameters")
        Session.Add("ConfigurationOptions_Parameters", Me.oParameters)

        If Me.oParameters IsNot Nothing Then
            Dim oParams As New roCollection(Me.oParameters.ParametersXML)

            'BlockUser
            Dim oCheckedBlock As Integer = 0
            Dim oBlockPeriod As Integer = Nothing

            'ShowLegalText
            Dim oCheckedShowLegalText = 0

            Try
                'BlockUser
                oCheckedBlock = roTypes.Any2Integer(HelperSession.AdvancedParametersCache("VisualTime.Security.BlockUser"))
                oBlockPeriod = roTypes.Any2Integer(HelperSession.AdvancedParametersCache("VisualTime.Security.BlockUserPeriod"))

                'ShowLegalText
                oCheckedShowLegalText = roTypes.Any2Integer(HelperSession.AdvancedParametersCache("VisualTime.Security.ShowLegalText"))
            Catch ex As Exception
                'BlockUser
                oCheckedBlock = 0
                oBlockPeriod = 24 'Default value
            End Try

            'BlockUser
            Me.chkBlockUser.Checked = IIf(oCheckedBlock = 1, True, False)
            Me.txtBlockUserPeriod.Value = IIf(oBlockPeriod > 24, 24, IIf(oBlockPeriod < 3, 3, oBlockPeriod))
            If roTypes.Any2Boolean(Me.chkBlockUser.Checked) = False Then
                Me.txtBlockUserPeriod.ClientEnabled = False
            Else
                Me.txtBlockUserPeriod.ClientEnabled = True
            End If

            'ShowLegalText
            Me.chkShowText.Checked = IIf(oCheckedShowLegalText = 1, True, False)

        End If
    End Sub

    Private Function CheckData(ByRef strKeyMsg As String) As Boolean
        If Me.chkBlockUser.Checked AndAlso
            Me.txtBlockUserPeriod.Value Is Nothing Or
            (Me.txtBlockUserPeriod.Value IsNot Nothing AndAlso Not IsNumeric(Me.txtBlockUserPeriod.Value)) Or
            (Me.txtBlockUserPeriod.Value IsNot Nothing AndAlso roTypes.Any2Integer(Me.txtBlockUserPeriod.Value) < 3) Or
            (Me.txtBlockUserPeriod.Value IsNot Nothing AndAlso roTypes.Any2Integer(Me.txtBlockUserPeriod.Value) > 24) Then
            strKeyMsg = "InvalidPeriod.Message"
            Me.txtBlockUserPeriod.Focus()
        End If

        Return (strKeyMsg = "")

    End Function

    Private Sub SetPermissions()

        If Me.oPermission = Permission.None Then
            Me.tbDatabaseOptions.Visible = False
        ElseIf Me.oPermission < Permission.Write Then
            Me.DisableControls(Me.tbDatabaseOptions.Controls)
        End If

        ' Desactivar los botons de grabación sólo si no tiene acceso a modificar la configuración de presencia y no tiene permisos de administrar la definición de la ficha
        If Me.oPermission < Permission.Write Then
            Me.btSave.Visible = False '.Style("display") = "none"
            Me.btCancel.Visible = False '.Style("display") = "none"
        End If

    End Sub

#End Region

End Class