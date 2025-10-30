Imports Robotics.Base.DTOs
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions
Imports Robotics.Web.Base
Imports Robotics.Web.Base.API

Partial Class LockDB
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

        Me.InsertExtraJavascript("lockDB", "~/Security/Scripts/LockDb.js")
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
        Dim xLockDate As Date = New Date(1900, 1, 1)
        If Me.oPermission >= Permission.Write Then

            If Me.CheckData(strKeyMsg) Then

                Dim bolSaved As Boolean = False

                Dim oParams As New roCollection(Me.oParameters.ParametersXML)
                oParams.Remove(Me.oParameters.ParametersNames(Parameters.FirstDate))

                If Not Me.txtFreezeDate.Value Is Nothing Then
                    oParams.Add(Me.oParameters.ParametersNames(Parameters.FirstDate), Me.txtFreezeDate.Date)
                    xLockDate = Me.txtFreezeDate.Date
                End If

                If (initialFreezeDate.Contains("closeDate")) Then
                    initialFreezeDate("closeDate") = Me.txtFreezeDate.Value
                Else
                    initialFreezeDate.Add("closeDate", Me.txtFreezeDate.Value)
                End If

                If Me.chkAlertFreezeDate.Checked Then
                    oParams.Add(Me.oParameters.ParametersNames(Parameters.CloseDateAlertPeriod), Me.txtAlertFreezeDatePeriod.Text)
                    oParams.Add(Me.oParameters.ParametersNames(Parameters.CloseDateAlert), 1)
                Else
                    oParams.Add(Me.oParameters.ParametersNames(Parameters.CloseDateAlert), 0)
                End If

                oParams.Add(Me.oParameters.ParametersNames(Parameters.InitialNotifierDate), xLockDate)

                'Guardar roCollection
                Me.oParameters.ParametersXML = oParams.XML
                bolSaved = API.ConnectorServiceMethods.SaveParameters(Me, Me.oParameters, True)

                If bolSaved Then

                    bolSaved = API.EmployeeServiceMethods.SaveLockDate(Me, xLockDate, True)

                    API.LiveTasksServiceMethods.CheckCloseDate(Me.Page)

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
            ' Fecha de congelación
            Dim oDate As Object = Nothing
            Try
                oDate = oParams.Item(Me.oParameters.ParametersNames(Parameters.FirstDate))
            Catch ex As Exception
                oDate = Nothing
            End Try

            If oDate IsNot Nothing AndAlso IsDate(oDate) Then
                Dim xFreezeDate As Date = oDate
                Me.txtFreezeDate.Date = xFreezeDate
            Else
                Me.txtFreezeDate.Value = Nothing
            End If

            If (initialFreezeDate.Contains("closeDate")) Then
                initialFreezeDate("closeDate") = Me.txtFreezeDate.Value
            Else
                initialFreezeDate.Add("closeDate", Me.txtFreezeDate.Value)
            End If

            Dim oChecked As Integer = 0
            Dim oPeriod As Integer = Nothing

            Try
                oChecked = roTypes.Any2Integer(oParams.Item(Me.oParameters.ParametersNames(Parameters.CloseDateAlert)))
                oPeriod = roTypes.Any2Integer(oParams.Item(Me.oParameters.ParametersNames(Parameters.CloseDateAlertPeriod)))
            Catch ex As Exception
                oChecked = 0
                oPeriod = 30
            End Try

            Me.chkAlertFreezeDate.Checked = IIf(oChecked = 1, True, False)
            Me.txtAlertFreezeDatePeriod.Value = oPeriod.ToString()

        End If
    End Sub

    Private Function CheckData(ByRef strKeyMsg As String) As Boolean
        If Not Me.txtFreezeDate.Value Is Nothing AndAlso Not IsDate(Me.txtFreezeDate.Date) Then
            strKeyMsg = Me.Language.Keyword("InvalidDate.Message")
            Me.txtFreezeDate.Focus()
        ElseIf Me.chkAlertFreezeDate.Checked AndAlso
            (Me.txtAlertFreezeDatePeriod.Value IsNot Nothing AndAlso Not IsNumeric(Me.txtAlertFreezeDatePeriod.Value)) Or
            (Me.txtAlertFreezeDatePeriod.Value IsNot Nothing AndAlso roTypes.Any2Integer(Me.txtAlertFreezeDatePeriod.Value) < 0) Then
            strKeyMsg = Me.Language.Keyword("InvalidCloseAlert.Message")
            Me.txtAlertFreezeDatePeriod.Focus()
        End If

        Return (strKeyMsg = "")

    End Function

    Private Sub SetPermissions()

        If Me.oPermission = Permission.None Then
            Me.TABBUTTON_DatabaseOptions.Style("display") = "none"
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