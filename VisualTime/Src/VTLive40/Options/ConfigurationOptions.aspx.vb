Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common.AdvancedParameter
Imports Robotics.Security
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions
Imports Robotics.Web.Base
Imports Robotics.Web.Base.API

Partial Class ConfigurationOptions
    Inherits PageBase

    Private Const FeatureAlias As String = "Administration.Options.General"

    Private Const FeatureAttendanceAlias As String = "Administration.Options.Attendance"



#Region "Declarations"

    Private oPermission As Permission
    Private oAttendancePermission As Permission

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
        Me.oAttendancePermission = Me.GetFeaturePermission(FeatureAttendanceAlias)

        If Me.oPermission = Permission.None And Me.oAttendancePermission = Permission.None Then
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
        Me.hdnValueGridName.Value = Me.Language.Translate("GridIps.NameValue", DefaultScope)

        Me.chkDisableBiometricData.Text = Me.Language.Translate("msgDisableBiometricData", DefaultScope)
        Me.lblDeleteBiometricData.Text = Me.Language.Translate("msgDeleteBiometricData", DefaultScope)
        Me.chkA3Payroll.Text = Me.Language.Translate("msgA3PayrollImport", DefaultScope)

        Me.txtCertificateHeader.Value = Me.Language.Translate("GridCertificates.Certificate", DefaultScope)
        Me.txtCreatedByHeader.Value = Me.Language.Translate("GridCertificates.CreatedBy", DefaultScope)
        Me.txtCreatedAtHeader.Value = Me.Language.Translate("GridCertificates.CreatedAt", DefaultScope)
        Me.txtLinkHeader.Value = Me.Language.Translate("GridCertificates.Link", DefaultScope)


        If Not WLHelper.HasFeaturePermission(WLHelperWeb.CurrentPassportID, "Documents", Permission.Write) Then
            Me.TABBUTTON_Document.Style("display") = "none"
        Else
            Me.TABBUTTON_Document.Style("display") = ""
        End If

        If HelperSession.AdvancedParametersCache("VTLive.Edition").ToString.ToLower = roServerLicense.roVisualTimeEdition.Starter.ToString.ToLower Then
            Me.TABBUTTON_WsConfig.Style("display") = "none"
        Else
            Me.TABBUTTON_WsConfig.Style("display") = ""
        End If

        If Not Me.IsPostBack Then
            Me.LoadOptions()
            Me.LoadAttendanceOptions()

            Me.SetPermissions()
            Me.SetAttendancePermissions()

            Me.frmWsAdmin.InitComponents()

            If Not (HelperSession.GetFeatureIsInstalledFromApplication("Feature\Absences") OrElse HelperSession.GetFeatureIsInstalledFromApplication("Feature\Documents")) Then trDocumentsAudit.Visible = False
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

            If Me.CheckData(strKeyMsg) AndAlso Me.CheckAttendanceData(strKeyMsg) Then

                Dim bolSaved As Boolean = False
                Dim strErrorText As String = ""

                Dim oParams As New roCollection(Me.oParameters.ParametersXML)

                'Biometric Data
                oParams.Remove(Me.oParameters.ParametersNames(Parameters.BiometricDataKeepPeriod))
                If Not Me.txtBiometricData.Value Is Nothing Then
                    oParams.Add(Me.oParameters.ParametersNames(Parameters.BiometricDataKeepPeriod), roTypes.Any2Integer(Me.txtBiometricData.Text))
                End If

                'Disable Biometric Data
                If Me.oParameters.ParametersNames.Contains(Parameters.DisableBiometricData.ToString()) AndAlso roTypes.Any2Boolean(oParams.Item(Me.oParameters.ParametersNames(Parameters.DisableBiometricData))) <> Me.chkDisableBiometricData.Checked Then
                    API.EmployeeServiceMethods.DisableBiometricDataForAllEmployees(Me, Me.chkDisableBiometricData.Checked)
                End If
                oParams.Remove(Me.oParameters.ParametersNames(Parameters.DisableBiometricData))
                oParams.Add(Me.oParameters.ParametersNames(Parameters.DisableBiometricData), roTypes.Any2Boolean(Me.chkDisableBiometricData.Checked))

                'A3 Payroll
                If chkA3Payroll.Checked Then
                    ' Si la nómina A3 no está registrada, la registro ahora
                    Dim documentTemplate As roDocumentTemplate = API.DocumentsServiceMethods.GetTemplateDocumentsList(False, Nothing, Nothing, True).Where(Function(template) template.ShortName = "A3_PR").FirstOrDefault()
                    If documentTemplate Is Nothing OrElse documentTemplate.ShortName <> "A3_PR" Then
                        API.DocumentsServiceMethods.RegisterA3Payroll(Me, True)
                    End If
                Else
                    API.DocumentsServiceMethods.UnRegisterA3Payroll(Me, False)
                End If

                'Photos
                oParams.Remove(Me.oParameters.ParametersNames(Parameters.PhotosKeepPeriod))
                    If Not Me.txtPhotoDelete.Value Is Nothing Then
                        oParams.Add(Me.oParameters.ParametersNames(Parameters.PhotosKeepPeriod), roTypes.Any2Integer(Me.txtPhotoDelete.Text))
                    End If

                    'Doucments
                    oParams.Remove(oParameters.ParametersNames(Parameters.DocumentsKeepPeriod))
                    If Not txtDocDelete.Value Is Nothing Then
                        oParams.Add(oParameters.ParametersNames(Parameters.DocumentsKeepPeriod), roTypes.Any2Integer(txtDocDelete.Text))
                    End If

                    oParams.Remove(oParameters.ParametersNames(Parameters.NumMonthsAccess))
                    If Not txtAccessMovesDelete.Value Is Nothing Then
                        oParams.Add(oParameters.ParametersNames(Parameters.NumMonthsAccess), roTypes.Any2Integer(txtAccessMovesDelete.Text))
                    End If

                    'TimeFormat
                    oParams.Remove(Me.oParameters.ParametersNames(Parameters.TimeFormat))
                    If Me.optTimeFormatStandard.Checked Then
                        oParams.Add(Me.oParameters.ParametersNames(Parameters.TimeFormat), "1")
                    Else
                        oParams.Add(Me.oParameters.ParametersNames(Parameters.TimeFormat), "0")
                    End If

                    'Día de la semana
                    oParams.Remove(Me.oParameters.ParametersNames(Parameters.WeekPeriod))
                    If Me.cmbWeekDay.Value <> "" Then
                        oParams.Add(Me.oParameters.ParametersNames(Parameters.WeekPeriod), Me.cmbWeekDay.Value)
                    End If

                    ' Máximo número de horas entre entrada y salida
                    Dim oAdvancedParameter As roAdvancedParameter = Nothing
                    oAdvancedParameter = API.CommonServiceMethods.GetAdvancedParameter(Nothing, "OptionsParameters.MaxMovementHours")
                    oAdvancedParameter.Value = roConversions.ConvertTimeToHours(Me.txtESDetectionClassicTime.Value)
                    oAdvancedParameter.Value = oAdvancedParameter.Value.Replace(roConversions.GetDecimalDigitFormat, ".")
                    API.CommonServiceMethods.SaveAdvancedParameter(Me.Page, oAdvancedParameter, True)

                    'Guardar roCollection
                    Me.oParameters.ParametersXML = oParams.XML

                    bolSaved = API.ConnectorServiceMethods.SaveParameters(Me, Me.oParameters, True)

                    oParams = New roCollection(Me.oParameters.ParametersXML)
                    oParams.Item(Me.oParameters.ParametersNames(Parameters.MovMaxHours)) = Me.txtESDetectionClassicTime.Value

                    If Not oParams.Exists(Me.oParameters.ParametersNames(Parameters.PunchPeriodRTIn)) Then _
                oParams.Add(Me.oParameters.ParametersNames(Parameters.PunchPeriodRTIn), 0)
                    If Not oParams.Exists(Me.oParameters.ParametersNames(Parameters.PunchPeriodRTOut)) Then _
                oParams.Add(Me.oParameters.ParametersNames(Parameters.PunchPeriodRTOut), 0)
                    oParams.Item(Me.oParameters.ParametersNames(Parameters.PunchPeriodRTIn)) = roTypes.Any2Time(Me.txtPunchPeriodRTIn.Value).Minutes
                    oParams.Item(Me.oParameters.ParametersNames(Parameters.PunchPeriodRTOut)) = roTypes.Any2Time(Me.txtPunchPeriodRTOut.Value).Minutes

                    If Not oParams.Exists(Me.oParameters.ParametersNames(Parameters.MonthPeriod)) Then
                        oParams.Add(Me.oParameters.ParametersNames(Parameters.MonthPeriod), txtMonthPeriod.Value)
                    Else
                        oParams.Item(Me.oParameters.ParametersNames(Parameters.MonthPeriod)) = txtMonthPeriod.Value
                    End If
                    If Not oParams.Exists(Me.oParameters.ParametersNames(Parameters.YearPeriod)) Then
                        oParams.Add(Me.oParameters.ParametersNames(Parameters.YearPeriod), txtYearPeriod.Value)
                    Else
                        oParams.Item(Me.oParameters.ParametersNames(Parameters.YearPeriod)) = txtYearPeriod.Value
                    End If

                    Me.oParameters.ParametersXML = oParams.XML

                    bolSaved = (bolSaved AndAlso ConnectorServiceMethods.SaveParameters(Me, Me.oParameters, True))
                    Dim wsResult = Me.frmWsAdmin.SaveWsConfig()
                    bolSaved = bolSaved AndAlso wsResult.Item1
                    If Not wsResult.Item1 Then
                        strErrorText = wsResult.Item2
                    End If

                    Dim tokenResult = Me.frmWsAdmin.SaveTokenConfig()
                    bolSaved = bolSaved AndAlso tokenResult.Item1
                    If Not tokenResult.Item1 Then
                        strErrorText = tokenResult.Item2
                    End If

                    Dim pgpResult = Me.frmWsAdmin.SavePGPConfig()
                    bolSaved = bolSaved AndAlso pgpResult.Item1
                    If Not pgpResult.Item1 Then
                        strErrorText = pgpResult.Item2
                    End If
                    'External Access IP
                    oAdvancedParameter = API.CommonServiceMethods.GetAdvancedParameter(Nothing, "ExternAccessIPs")
                    oAdvancedParameter.Value = Me.txtAllowedIPs.Value
                    API.CommonServiceMethods.SaveAdvancedParameter(Me.Page, oAdvancedParameter, True)

                    If bolSaved Then
                        Me.hdnChanged.Value = "0"
                        Me.LoadOptions()
                        Me.LoadAttendanceOptions()
                        Me.frmWsAdmin.InitComponents()
                    Else
                        If Not String.IsNullOrEmpty(strErrorText) Then
                            HelperWeb.ShowMessage(Me, Me.Language.Translate(strKeyMsg & ".Title", Me.DefaultScope), Me.Language.Translate(strErrorText, Me.DefaultScope), , , , HelperWeb.MsgBoxIcons.AlertIcon)
                        End If
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
        Me.LoadAttendanceOptions()

    End Sub

    Protected Sub btRefresh_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btRefresh.Click

        Me.hdnChanged.Value = "1"

    End Sub

#End Region

#Region "Methods"

    Private Sub LoadAttendanceOptions()

        ' Settings de la base de datos
        Me.oParameters = API.ConnectorServiceMethods.GetParameters(Me)
        Session.Remove("AttendanceOptions_Parameters")
        Session.Add("AttendanceOptions_Parameters", Me.oParameters)

        If Me.oParameters IsNot Nothing Then

            Dim oParams As New roCollection(Me.oParameters.ParametersXML)
            ' Maximo de horas entre entradas y salidas para terminales serie
            Dim strTime As String = oParams.Item(Me.oParameters.ParametersNames(Parameters.MovMaxHours))
            If strTime <> "" Then
                Me.txtESDetectionClassicTime.Value = Format(CDate(strTime), "HH:mm")
            Else
                Me.txtESDetectionClassicTime.Value = "00:00"
            End If

            Dim intMinutes As Integer = oParams.Item(Me.oParameters.ParametersNames(Parameters.PunchPeriodRTIn))
            Me.txtPunchPeriodRTIn.Value = Format(CDate(roTypes.Any2Time(intMinutes / 60).Value), "HH:mm")
            intMinutes = oParams.Item(Me.oParameters.ParametersNames(Parameters.PunchPeriodRTOut))
            Me.txtPunchPeriodRTOut.Value = Format(CDate(roTypes.Any2Time(intMinutes / 60).Value), "HH:mm")

            Dim strPeriod As String = oParams.Item(Me.oParameters.ParametersNames(Parameters.YearPeriod))
            If strPeriod <> "" Then
                Me.txtYearPeriod.Value = Val(strPeriod)
            Else
                Me.txtYearPeriod.Value = 1
            End If
            strPeriod = oParams.Item(Me.oParameters.ParametersNames(Parameters.MonthPeriod))
            If strPeriod <> "" Then
                Me.txtMonthPeriod.Value = Val(strPeriod)
            Else
                Me.txtMonthPeriod.Value = 1
            End If

        End If
    End Sub

    Private Sub LoadOptions()

        'Cargamos los días de la semana
        cmbWeekDay.Items.Clear()
        cmbWeekDay.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("WeekDayNames.Monday", DefaultScope), "1"))
        cmbWeekDay.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("WeekDayNames.Tuesday", DefaultScope), "2"))
        cmbWeekDay.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("WeekDayNames.Wednesday", DefaultScope), "3"))
        cmbWeekDay.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("WeekDayNames.Thursday", DefaultScope), "4"))
        cmbWeekDay.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("WeekDayNames.Friday", DefaultScope), "5"))
        cmbWeekDay.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("WeekDayNames.Saturday", DefaultScope), "6"))
        cmbWeekDay.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("WeekDayNames.Sunday", DefaultScope), "7"))
        cmbWeekDay.SelectedIndex = 1

        'Settings de la base de datos
        Me.oParameters = ConnectorServiceMethods.GetParameters(Me)
        Session.Remove("ConfigurationOptions_Parameters")
        Session.Add("ConfigurationOptions_Parameters", Me.oParameters)

        If Me.oParameters IsNot Nothing Then
            Dim oParams As New roCollection(oParameters.ParametersXML)
            ' Fecha de congelación
            Dim oKeepDays As Object = Nothing
            Try
                oKeepDays = oParams.Item(Me.oParameters.ParametersNames(Parameters.BiometricDataKeepPeriod))
            Catch ex As Exception
                oKeepDays = 15
            End Try

            If oKeepDays IsNot Nothing AndAlso IsNumeric(oKeepDays) Then
                Dim iKeepDays As Integer = oKeepDays
                'Me.txtFreezeDate.Value = xFreezeDate
                Me.txtBiometricData.Value = iKeepDays
            Else
                Me.txtBiometricData.Value = 15
            End If

            Try
                oKeepDays = oParams.Item(Me.oParameters.ParametersNames(Parameters.PhotosKeepPeriod))
            Catch ex As Exception
                oKeepDays = 15
            End Try

            If oKeepDays IsNot Nothing AndAlso IsNumeric(oKeepDays) Then
                Dim iKeepDays As Integer = oKeepDays
                'Me.txtFreezeDate.Value = xFreezeDate
                Me.txtPhotoDelete.Value = iKeepDays
            Else
                Me.txtPhotoDelete.Value = 15
            End If

            'Audoitoria de documentos
            Try
                oKeepDays = oParams.Item(Me.oParameters.ParametersNames(Parameters.DocumentsKeepPeriod))
            Catch ex As Exception
                oKeepDays = 30
            End Try

            If oKeepDays IsNot Nothing AndAlso IsNumeric(oKeepDays) Then
                Dim iKeepDays As Integer = oKeepDays
                'Me.txtFreezeDate.Value = xFreezeDate
                txtDocDelete.Value = iKeepDays
            Else
                txtDocDelete.Value = 30
            End If

            'Audoitoria de documentos
            Try
                oKeepDays = oParams.Item(Me.oParameters.ParametersNames(Parameters.NumMonthsAccess))
            Catch ex As Exception
                oKeepDays = 30
            End Try

            If oKeepDays IsNot Nothing AndAlso IsNumeric(oKeepDays) Then
                Dim iKeepDays As Integer = oKeepDays
                'Me.txtFreezeDate.Value = xFreezeDate
                txtAccessMovesDelete.Value = iKeepDays
            Else
                txtAccessMovesDelete.Value = 0
            End If

            Me.optTimeFormatNumeric.Checked = False
            Me.optTimeFormatStandard.Checked = False

            If Not oParams.Item(Me.oParameters.ParametersNames(Parameters.TimeFormat)) Is Nothing Then
                If roTypes.Any2String(oParams.Item(Me.oParameters.ParametersNames(Parameters.TimeFormat))) = "1" Or roTypes.Any2String(oParams.Item(Me.oParameters.ParametersNames(Parameters.TimeFormat))) = "" Then
                    Me.optTimeFormatStandard.Checked = True
                Else
                    Me.optTimeFormatNumeric.Checked = True
                End If
            Else
                Me.optTimeFormatStandard.Checked = True
            End If

            ' Carga de día de inicio de la semana, y valores por defecto
            If Not oParams.Item(Me.oParameters.ParametersNames(Parameters.WeekPeriod)) Is Nothing Then
                cmbWeekDay.SelectedIndex = roTypes.Any2Integer(oParams.Item(Me.oParameters.ParametersNames(Parameters.WeekPeriod))) - 1
            End If

            ' Deshabilitar datos biométricos
            Dim bDIsableBiometricData As Object = Nothing
            Try
                bDIsableBiometricData = oParams.Item(Me.oParameters.ParametersNames(Parameters.DisableBiometricData))
            Catch ex As Exception
                bDIsableBiometricData = False
            End Try

            chkDisableBiometricData.Checked = bDIsableBiometricData

            'A3 Payroll
            Dim documentTemplates As New List(Of roDocumentTemplate)
            documentTemplates = API.DocumentsServiceMethods.GetDocumentTemplateListbyType(DocumentType.Employee, Me, False)
            Me.chkA3Payroll.Checked = documentTemplates.FindAll(Function(x) x.ShortName = "A3_PR").Any

        End If

    End Sub

    Private Function CheckAttendanceData(ByRef strErrorMessage As String) As Boolean

        Dim strMsg As String = ""

        If Not IsDate(Me.txtESDetectionClassicTime.Value) Then
            strMsg = Me.Language.Keyword("InvalidTime.Message")
            Me.txtESDetectionClassicTime.Focus()
        ElseIf Not IsDate(Me.txtPunchPeriodRTIn.Value) Then
            strMsg = Me.Language.Keyword("InvalidTime.Message")
            Me.txtPunchPeriodRTIn.Focus()
        ElseIf Not IsDate(Me.txtPunchPeriodRTOut.Value) Then
            strMsg = Me.Language.Keyword("InvalidTime.Message")
            Me.txtPunchPeriodRTOut.Focus()
        ElseIf Not IsNumeric(Me.txtYearPeriod.Value) OrElse Val(Me.txtYearPeriod.Value) <= 0 OrElse Val(Me.txtYearPeriod.Value) > 12 Then
            strMsg = Me.Language.Keyword("InvalidYearPeriod.Message")
            Me.txtYearPeriod.Focus()
        ElseIf Not IsNumeric(Me.txtMonthPeriod.Value) Then
            strMsg = Me.Language.Keyword("InvalidMonthPeriod.Message")
            Me.txtMonthPeriod.Focus()
        Else
            Try
                Dim xdate As New Date(Now.Year, Me.txtYearPeriod.Value, Me.txtMonthPeriod.Value)
            Catch
                strMsg = Me.Language.Keyword("InvalidMonthPeriod.Message")
                Me.txtMonthPeriod.Focus()
            End Try
        End If

        strErrorMessage = strMsg

        Return (strMsg = "")

    End Function

    Private Function CheckData(ByRef strKeyMsg As String) As Boolean

        If Not Me.txtBiometricData.Value Is Nothing AndAlso IsNumeric(Me.txtBiometricData.Value) AndAlso roTypes.Any2Integer(Me.txtBiometricData.Text) < 15 Then
            strKeyMsg = "InvalidBiometricDataPeriod.Message"
            Me.txtBiometricData.Focus()
        End If

        If Not Me.txtBiometricData.Value Is Nothing AndAlso IsNumeric(Me.txtBiometricData.Value) AndAlso roTypes.Any2Integer(Me.txtBiometricData.Text) > 32767 Then
            strKeyMsg = "InvalidBiometricDataMaxPeriod.Message"
            Me.txtBiometricData.Focus()
        End If

        If Not Me.txtPhotoDelete.Value Is Nothing AndAlso IsNumeric(Me.txtPhotoDelete.Value) AndAlso roTypes.Any2Integer(Me.txtPhotoDelete.Text) < 15 Then
            strKeyMsg = "InvalidPhotoPeriod.Message"
            Me.txtPhotoDelete.Focus()
        End If

        If Not Me.txtPhotoDelete.Value Is Nothing AndAlso IsNumeric(Me.txtPhotoDelete.Value) AndAlso roTypes.Any2Integer(Me.txtPhotoDelete.Text) > 32767 Then
            strKeyMsg = "InvalidPhotoMaxPeriod.Message"
            Me.txtPhotoDelete.Focus()
        End If

        If Not Me.txtDocDelete.Value Is Nothing AndAlso IsNumeric(Me.txtDocDelete.Value) AndAlso roTypes.Any2Integer(Me.txtDocDelete.Text) < 15 Then
            strKeyMsg = "InvalidDocPeriod.Message"
            Me.txtDocDelete.Focus()
        End If

        If Not Me.txtDocDelete.Value Is Nothing AndAlso IsNumeric(Me.txtDocDelete.Value) AndAlso roTypes.Any2Integer(Me.txtDocDelete.Text) > 32767 Then
            strKeyMsg = "InvalidDocMaxPeriod.Message"
            Me.txtDocDelete.Focus()
        End If

        If Not Me.txtAccessMovesDelete.Value Is Nothing AndAlso IsNumeric(Me.txtAccessMovesDelete.Value) AndAlso roTypes.Any2Integer(Me.txtAccessMovesDelete.Text) > 600 Then
            strKeyMsg = "InvalidAccessMovesMaxPeriod.Message"
            Me.txtAccessMovesDelete.Focus()
        End If

        Return (strKeyMsg = "")

    End Function

    Private Function ValidateEmail(ByVal sEmail As String) As Boolean
        Dim objRegExp As New Regex("^[a-zA-Z][\w\.-]*[a-zA-Z0-9]@[a-zA-Z][\w\.-]*[a-zA-Z0-9]\.[a-zA-Z][a-zA-Z\.]*[a-zA-Z]$")
        Dim bolRet As Boolean = False
        bolRet = objRegExp.IsMatch(sEmail)

        objRegExp = Nothing
        Return bolRet
    End Function

    Private Sub SetAttendancePermissions()

        If Me.oAttendancePermission = Permission.None Then
            Me.TABBUTTON_MovesOptions.Style("display") = "none"
            Me.TABBUTTON_StartYearOptions.Style("display") = "none"
            Me.tbMoveOptions.Visible = False
            Me.ConfigurationOptions_TabVisibleName.Value = "panEmployeeUserFieldsOptions"
        ElseIf Me.oAttendancePermission < Permission.Write Then
            Me.DisableControls(Me.tbMoveOptions.Controls)
        End If

        ' Desactivar los botons de grabación sólo si no tiene acceso a modificar la configuración de presencia
        If Me.oPermission < Permission.Write Then
            Me.btSave.Visible = False '.Style("display") = "none"
            Me.btCancel.Visible = False ' .Style("display") = "none"
        End If

    End Sub

    Private Sub SetPermissions()

        If Me.oPermission = Permission.None Then
            Me.TABBUTTON_TimeFormatOptions.Style("display") = "none"
            Me.TABBUTTON_DatabaseOptions.Style("display") = "none"
            Me.tbTimeFormatOptions.Visible = False
            Me.tbDatabaseOptions.Visible = False
            Me.ConfigurationOptions_TabVisibleName.Value = "panEmployeeUserFieldsOptions"
        ElseIf Me.oPermission < Permission.Write Then
            Me.DisableControls(Me.tbTimeFormatOptions.Controls)
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