Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Absence
Imports Robotics.Base.VTBusiness.Cause
Imports Robotics.VTBase
Imports Robotics.Web.Base
Imports Robotics.Web.Base.API
Imports Robotics.Web.Base.HelperWeb

Partial Class Forms_EditProgrammedAbsence
    Inherits PageBase

#Region "Declarations"

    Private intEmployeeID As Integer
    Private xBeginDate As Date
    Private bolNew As Boolean

    Private Const EmployeeFeatureAlias As String = "Employees"
    Private oPermission As Permission
    Private oPermissionEmp As Permission

#End Region

#Region "Properties"

    Public Property ProgrammedAbsenceData() As roProgrammedAbsence
        Get
            Dim oRet As roProgrammedAbsence = ViewState("EditProgrammedAbsence_ProgrammedAbsenceData")
            If oRet Is Nothing Then
                oRet = ProgrammedAbsencesServiceMethods.GetProgrammedAbsence(Me.Page, Me.intEmployeeID, Me.xBeginDate.Date)
                ViewState("EditProgrammedAbsence_ProgrammedAbsenceData") = oRet
            End If
            Return oRet
        End Get
        Set(ByVal value As roProgrammedAbsence)
            ViewState("EditProgrammedAbsence_ProgrammedAbsenceData") = value
        End Set
    End Property

#End Region

#Region "Events"

    Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        Me.InsertExtraJavascript("Cookies", "~/Base/Scripts/Cookies.js", , True)
        Me.InsertExtraJavascript("Generic", "~/Base/Scripts/Generic.js", , True)
        Me.InsertExtraJavascript("GridViewHelper", "~/Base/Scripts/GridViewHelper.js", , True)
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.InsertCssIncludes(Me.Page)

        If Not Me.HasFeaturePermission("Employees.Type", Permission.Write) Then
            WLHelperWeb.RedirectAccessDenied(True)
            Exit Sub
        End If

        Me.intEmployeeID = roTypes.Any2Integer(Request.Params("EmployeeID"))

        If Me.hdnIdAbsence.Value = String.Empty OrElse Me.hdnIdAbsence.Value = "-1" Then
            If Request.Params("BeginDate") IsNot Nothing Then
                Me.xBeginDate = roTypes.Any2DateTime(Request.Params("BeginDate")).Date
            End If

            If Request.Params("NewRecord") IsNot Nothing Then
                Me.bolNew = (roTypes.Any2Integer(Request.Params("NewRecord")) = 1)
            End If
        Else
            Me.bolNew = False
            Me.xBeginDate = Date.MinValue
        End If

        If Me.bolNew = False Then
            oPermission = Me.GetFeaturePermission(EmployeeFeatureAlias)
            oPermissionEmp = Me.GetFeaturePermissionByEmployee(EmployeeFeatureAlias, intEmployeeID)
            Dim oPerm As Permission
            If oPermission > oPermissionEmp Then
                oPerm = oPermissionEmp
            Else
                oPerm = oPermission
            End If

            If oPerm < Permission.Write Then
                Me.DisableControls(UpdatePanel1.Controls)
                Me.btOK.Visible = False
                Me.btCancel.Text = Me.Language.Keyword("Button.Close")
            End If
        End If

        If Not Me.IsPostBack Then
            Me.ProgrammedAbsenceData = Nothing
            LoadCausesCombo()
            LoadData()
        End If

        ' Muestra la fecha de recaída
        Dim bolVisible As Boolean = roTypes.Any2Boolean(HelperSession.AdvancedParametersCache("RelapsedDateEnabled"))
        Me.lblRelapsedDate.Visible = bolVisible
        Me.txtRelapsedDate.Visible = bolVisible

        If HelperSession.GetFeatureIsInstalledFromApplication("Feature\Absences") = False Then
            Me.Content_ABS00.Style("display") = ""
            Me.Content_ABS01.Style("display") = "none"
            Me.TABBUTTON_ABS01.Style("display") = "none"
            Me.TABBUTTON_ABS00.Attributes("class") = "bTab-active"
        End If
    End Sub

    Protected Sub btnReload_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnReload.Click
        Me.ProgrammedAbsenceData = Nothing
        LoadCausesCombo()
        LoadData()
    End Sub

    Protected Sub btnSaveAndEdit_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSaveAndEdit.Click
        If HelperSession.GetFeatureIsInstalledFromApplication("Feature\Absences") = False Then
            SaveAbsence(True)
        Else
            SaveAbsence(False)
        End If

    End Sub

    Protected Sub btOK_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btOK.Click
        SaveAbsence(True)
    End Sub

    Private Sub SaveAbsence(ByVal bMusctCloseOnSave As Boolean)
        If txtBeginDate.Date.Date = #12:00:00 AM# Then
            ShowMessage(Me, "", Me.Language.Translate("InitialDateRequired", Me.DefaultScope))
            Exit Sub
        End If

        If Me.cmbCausesList.Value IsNot Nothing AndAlso roTypes.Any2String(Me.cmbCausesList.Value) <> "" Then

            If Val(txtMaxDays.Value) <= Integer.MaxValue AndAlso Val(txtMaxDays.Value) >= 0 Then

                Dim oProgAbsence As roProgrammedAbsence = Nothing

                If Me.bolNew Then
                    oProgAbsence = New roProgrammedAbsence
                Else
                    oProgAbsence = Me.ProgrammedAbsenceData()
                End If

                If Not Me.bolNew AndAlso oProgAbsence Is Nothing Then
                    oProgAbsence = New roProgrammedAbsence
                    Me.bolNew = True
                End If

                With oProgAbsence
                    .IDEmployee = Request.Params("EmployeeID")
                    .BeginDate = txtBeginDate.Date.Date
                    If txtEndDate.Value IsNot Nothing Then
                        .FinishDate = txtEndDate.Date.Date
                    Else
                        .FinishDate = Nothing
                    End If
                    .IDCause = roTypes.Any2Integer(cmbCausesList.SelectedItem.Value)
                    .MaxLastingDays = Val(txtMaxDays.Value)
                    .Description = txtDescription.Text
                    If txtRelapsedDate.Value IsNot Nothing Then
                        .RelapsedDate = txtRelapsedDate.Date.Date
                    Else
                        .RelapsedDate = Nothing
                    End If

                End With

                If ProgrammedAbsencesServiceMethods.SaveProgrammedAbsence(Me, oProgAbsence, True) Then
                    ProgrammedAbsenceData = Nothing
                    Me.bolNew = False
                    LoadData()
                    Dim oLst = DocumentsServiceMethods.GetCauseAvailableDocumentTemplateByEmployee(Me.intEmployeeID, ProgrammedAbsenceData.IDCause, False, Me.Page, False)
                    If oLst IsNot Nothing AndAlso oLst.Count > 0 AndAlso Not bMusctCloseOnSave Then
                        Me.CanClose = False
                        Me.MustRefresh = "0"
                        Me.hdnParams_PageBase.Value = ""
                        Me.Content_ABS00.Style("display") = "none"
                        Me.Content_ABS01.Style("display") = ""
                        Me.TABBUTTON_ABS00.Attributes("class") = "bTab"
                        Me.TABBUTTON_ABS01.Attributes("class") = "bTab-active"
                    Else
                        Me.CanClose = True
                        Me.MustRefresh = "5"
                        Me.hdnParams_PageBase.Value = "SAVE_PROGRAMMED_ABSENCE"
                    End If
                Else
                    Me.CanClose = False
                    Me.MustRefresh = "0"
                    Me.hdnParams_PageBase.Value = ""
                End If
            Else
                HelperWeb.ShowMessage(Me, "", Me.Language.Translate("MaxDays.Incorrect.Message", Me.DefaultScope), , , , HelperWeb.MsgBoxIcons.InformationIcon)
            End If
        Else
            ShowMessage(Me, "", Me.Language.Translate("EditProgrammedAbsence.CheckCause", Me.DefaultScope))
        End If
    End Sub

#End Region

#Region "Methods"

    Private Sub LoadCausesCombo()
        '        Dim tb As DataTable = CausesServiceMethods.GetCausesShortList(Me.Page, True)
        Dim tb As DataTable = CausesServiceMethods.GetCausesShortListByRequestType(Me.Page, eCauseRequest.ProgrammedAbsence, True)

        Dim dvCauses As New DataView(tb)
        dvCauses.Sort = "Name"
        tb = dvCauses.ToTable

        cmbCausesList.Value = Nothing
        cmbCausesList.DataSource = tb
        cmbCausesList.TextField = "Name"
        cmbCausesList.ValueField = "Id"
        cmbCausesList.ValueType = GetType(Integer)
        cmbCausesList.DataBind()
    End Sub

    Protected Sub cmbCausesList_ValueChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmbCausesList.SelectedIndexChanged
        Dim IDCause As Integer = roTypes.Any2Integer(cmbCausesList.Value)
        Dim oCause As roCause = CausesServiceMethods.GetCauseByID(Me.Page, IDCause, False)

        If oCause IsNot Nothing AndAlso oCause.MaxProgrammedAbsence > 0 Then
            chkMaxDays.Checked = True
            chkEndDate.Checked = False
            txtMaxDays.Value = oCause.MaxProgrammedAbsence
            txtBeginDate.Value = Nothing
        ElseIf oCause IsNot Nothing AndAlso oCause.Absence_MaxDays.HasValue Then
            chkMaxDays.Checked = True
            chkEndDate.Checked = False
            txtMaxDays.Value = oCause.Absence_MaxDays
            txtBeginDate.Value = Nothing
        Else
            chkMaxDays.Checked = False
            chkEndDate.Checked = True
            txtMaxDays.Value = 0
        End If

        If HelperSession.GetFeatureIsInstalledFromApplication("Feature\Absences") = True Then
            Dim oLst = DocumentsServiceMethods.GetCauseAvailableDocumentTemplateByEmployee(Me.intEmployeeID, IDCause, False, Me.Page, False)
            If oLst IsNot Nothing AndAlso oLst.Count > 0 Then
                Me.TABBUTTON_ABS01.Style("display") = ""
            Else
                Me.TABBUTTON_ABS01.Style("display") = "none"
            End If
        Else
            Me.TABBUTTON_ABS01.Style("display") = "none"
        End If

    End Sub

    Private Sub LoadData()

        If Me.bolNew Then
            ' Es un registro nuevo
            chkEndDate.Checked = True
            chkMaxDays.Checked = False
            If Me.xBeginDate = Nothing Then
                Me.txtBeginDate.Date = Now.Date
            Else
                Me.txtBeginDate.Date = Me.xBeginDate.Date.Date
            End If
            txtRelapsedDate.Value = Nothing
            Me.TABBUTTON_ABS01.Style("display") = "none"
        Else
            ' Es un registro existente
            LoadProgrammedAbsence(Me.intEmployeeID, If(Me.xBeginDate = Date.MinValue, Me.txtBeginDate.Date.Date, Me.xBeginDate.Date.Date))
        End If

        If Not Me.bolNew AndAlso ProgrammedAbsenceData Is Nothing Then
            Me.bolNew = True
            LoadData()
            Exit Sub
        End If

        Me.hdnIdEmployee.Value = Me.intEmployeeID
        Me.hdnBeginDate.Value = Me.xBeginDate.Date.Date
        Me.hdnIdCause.Value = roTypes.Any2String(Me.cmbCausesList.Value)
        Me.hdnIdAbsence.Value = If(Not Me.bolNew, ProgrammedAbsenceData().IdAbsence, -1)

        Me.AbsDocumentManagment.SetScope(Me.intEmployeeID, DocumentType.Employee, If(Not Me.bolNew, ProgrammedAbsenceData().IdAbsence, -1), ForecastType.AbsenceDays)

        Me.AbsDocumentPendingManagment.LoadAlerts(Me.intEmployeeID, DocumentType.Employee, If(Not Me.bolNew, ProgrammedAbsenceData().IdAbsence, -1), ForecastType.AbsenceDays)

        If Not Me.bolNew AndAlso Me.AbsDocumentManagment.HasDocuments Then
            Me.cmbCausesList.Enabled = False
        Else
            Me.cmbCausesList.Enabled = True
        End If

    End Sub

    Private Sub LoadProgrammedAbsence(ByVal EmployeeID As Integer, ByVal BeginDate As Date)

        Me.intEmployeeID = EmployeeID
        Me.xBeginDate = BeginDate.Date

        Dim oProgAbsence As roProgrammedAbsence = ProgrammedAbsenceData()
        If oProgAbsence IsNot Nothing Then

            Me.cmbCausesList.Value = oProgAbsence.IDCause.Value

            If Me.cmbCausesList.SelectedItem Is Nothing Then
                Dim oList As New DevExpress.Web.ListEditItem
                Dim oCause As roCause = CausesServiceMethods.GetCauseByID(Me.Page, oProgAbsence.IDCause.Value, False)

                If oCause IsNot Nothing Then
                    oList.Text = oCause.Name & ("*")
                    oList.Value = oCause.ID
                    Me.cmbCausesList.Items.Add(oList)

                    If roTypes.Any2String(HelperSession.AdvancedParametersCache("BusinessGroup.ApplyNotAllowedModifyCause")) = "1" Then
                        'If roTypes.Any2String(CommonService.CommonServiceMethods.GetAdvancedParameter(Me.Page, "BusinessGroup.ApplyNotAllowedModifyCause").Value) = "1" Then
                        Me.DisableControls(UpdatePanel1.Controls)
                        Me.btOK.Visible = False
                        Me.btCancel.Text = Me.Language.Keyword("Button.Close")
                    End If

                End If

                If HelperSession.GetFeatureIsInstalledFromApplication("Feature\Absences") = True Then
                    Dim oLst = DocumentsServiceMethods.GetCauseAvailableDocumentTemplateByEmployee(Me.intEmployeeID, oCause.ID, False, Me.Page, False)
                    If oLst IsNot Nothing AndAlso oLst.Count > 0 Then
                        Me.TABBUTTON_ABS01.Style("display") = ""
                    Else
                        Me.TABBUTTON_ABS01.Style("display") = "none"
                    End If
                End If
            End If

            Me.txtBeginDate.Date = oProgAbsence.BeginDate.Value

            If oProgAbsence.FinishDate.HasValue Then
                chkEndDate.Checked = True
                chkMaxDays.Checked = False
                Me.txtEndDate.Date = oProgAbsence.FinishDate.Value
                oProgAbsence.MaxLastingDays = 0
            Else
                chkEndDate.Checked = False
                chkMaxDays.Checked = True
                Me.txtEndDate.Date = Nothing
                Me.txtMaxDays.Value = oProgAbsence.MaxLastingDays
            End If

            If oProgAbsence.MaxLastingDays = 0 Then txtMaxDays.Value = ""
            Me.txtDescription.Text = oProgAbsence.Description

            If oProgAbsence.RelapsedDate.HasValue Then
                Me.txtRelapsedDate.Date = oProgAbsence.RelapsedDate
            Else
                Me.txtRelapsedDate.Date = Nothing
            End If
        End If

    End Sub

#End Region

End Class