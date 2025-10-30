Imports Robotics.Base.DTOs
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions
Imports Robotics.Web.Base
Imports Robotics.Web.Base.API

Partial Class EmergencyReport
    Inherits PageBase

    Private Const FeatureAlias As String = "Administration.Options.General"

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
    End Sub

    Protected Sub Page_Load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
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

        Me.lblSaveChanges.InnerText = Me.Language.Translate("msgSaveChangesbar", DefaultScope)
        Me.lblSaveChangesBottom.InnerText = Me.lblSaveChanges.InnerText
        Me.lblUndoChanges.InnerText = Me.Language.Translate("msgUndoChangesbar", DefaultScope)
        Me.lblUndoChangesBottom.InnerText = Me.lblUndoChanges.InnerText

        Me.msgHasChanges.Value = Me.Language.Translate("msgHasChanges", DefaultScope)
        Me.msgHasErrors.Value = Me.Language.Translate("msgHasErrors", DefaultScope)

        If Not Me.IsPostBack Then
            Me.LoadOptions()
            Me.SetPermissions()
            Me.SetAttendancePermissions()

            Me.aURLDescript.HRef = HttpContext.Current.Request.Url.OriginalString.Replace(HttpContext.Current.Request.Url.AbsolutePath, "/Emergency/Emergency.aspx")

            Me.aURLDescript.InnerText = Me.aURLDescript.HRef
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

                Dim oParams As New roCollection(Me.oParameters.ParametersXML)

                'EmergencyReportActive
                oParams.Remove(Me.oParameters.ParametersNames(Parameters.EmergencyReportActive))
                If Me.opCheckEmergencyActive.Checked Then
                    oParams.Add(Me.oParameters.ParametersNames(Parameters.EmergencyReportActive), "1")
                Else
                    oParams.Add(Me.oParameters.ParametersNames(Parameters.EmergencyReportActive), "")
                End If

                'EmergencyReportKey
                oParams.Remove(Me.oParameters.ParametersNames(Parameters.EmergencyReportKey))
                If Me.opCheckEmergencyActive.Checked Then
                    oParams.Add(Me.oParameters.ParametersNames(Parameters.EmergencyReportKey), Me.txtEmergencyReportKey.Text.Trim)
                Else
                    oParams.Add(Me.oParameters.ParametersNames(Parameters.EmergencyReportKey), "")
                End If

                'Guardar roCollection
                Me.oParameters.ParametersXML = oParams.XML
                bolSaved = ConnectorServiceMethods.SaveParameters(Me, Me.oParameters, True)

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

            '=====================================================
            Me.opCheckEmergencyActive.Checked = False
            If Not oParams.Item(Me.oParameters.ParametersNames(Parameters.EmergencyReportActive)) Is Nothing Then
                If roTypes.Any2String(oParams.Item(Me.oParameters.ParametersNames(Parameters.EmergencyReportActive))) <> "" Then
                    Me.opCheckEmergencyActive.Checked = True
                End If
            End If

            Me.opCheckEmergencyActive.Checked = False

            Me.txtEmergencyReportKey.Text = ""

            If Not oParams.Item(Me.oParameters.ParametersNames(Parameters.EmergencyReportKey)) Is Nothing Then
                If roTypes.Any2String(oParams.Item(Me.oParameters.ParametersNames(Parameters.EmergencyReportKey))) <> "" Then
                    Me.opCheckEmergencyActive.Checked = True
                    Me.txtEmergencyReportKey.Attributes("value") = roTypes.Any2String(oParams.Item(Me.oParameters.ParametersNames(Parameters.EmergencyReportKey)))
                Else
                    Me.opCheckEmergencyActive.Checked = False
                    Me.txtEmergencyReportKey.Text = ""
                End If
            Else
                Me.opCheckEmergencyActive.Checked = False
            End If

            '=====================================================

        End If

    End Sub

    Private Function CheckData(ByRef strKeyMsg As String) As Boolean
        If strKeyMsg = "" Then
            If Me.opCheckEmergencyActive.Checked = True AndAlso Me.txtEmergencyReportKey.Text.Trim = String.Empty Then
                strKeyMsg = Me.Language.Translate("InvalidEmergencyReportKey.Message", Me.DefaultScope)
                Me.txtEmergencyReportKey.Focus()
            End If
        End If

        Return (strKeyMsg = "")

    End Function

    Private Sub SetAttendancePermissions()
        ' Desactivar los botons de grabación sólo si no tiene acceso a modificar la configuración de presencia
        If Me.oPermission < Permission.Write Then
            Me.btSave.Visible = False '.Style("display") = "none"
            Me.btCancel.Visible = False ' .Style("display") = "none"
        End If

    End Sub

    Private Sub SetPermissions()
        If Me.oPermission >= Permission.Read Then
            Me.TABBUTTON_EmergencyOptions.Style("display") = ""
            Me.tbEmergencyOptions.Visible = True
        Else
            Me.TABBUTTON_EmergencyOptions.Style("display") = "none"
            Me.tbEmergencyOptions.Visible = False
        End If

        ' Desactivar los botons de grabación sólo si no tiene acceso a modificar la configuración de presencia y no tiene permisos de administrar la definición de la ficha
        If Me.oPermission < Permission.Write Then
            Me.btSave.Visible = False '.Style("display") = "none"
            Me.btCancel.Visible = False '.Style("display") = "none"
        End If

    End Sub

#End Region

End Class