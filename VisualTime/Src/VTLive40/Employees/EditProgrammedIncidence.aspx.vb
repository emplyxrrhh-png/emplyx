Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Cause
Imports Robotics.Base.VTBusiness.Incidence
Imports Robotics.VTBase
Imports Robotics.Web.Base
Imports Robotics.Web.Base.API
Imports Robotics.Web.Base.HelperWeb

Partial Class Forms_EditProgrammedIncidence
    Inherits PageBase

#Region "Declarations"

    Private intEmployeeID As Integer
    Private intIdDayAbsence As Integer
    Private xBeginDate As Date
    Private xEndDate As Date
    Private bolNew As Boolean
    Private bIsWin32 As Boolean = False

    Private Const EmployeeFeatureAlias As String = "Employees"
    Private oPermission As Permission
    Private oPermissionEmp As Permission

#End Region

#Region "Properties"

    Public Property ProgrammedCauseData() As roProgrammedCause
        Get
            Dim oRet As roProgrammedCause = ViewState("EditProgrammedCause_ProgrammedCauseData")
            If oRet Is Nothing Then
                oRet = ProgrammedCausesServiceMethods.GetProgrammedCause(Me.Page, Me.intEmployeeID, Me.xBeginDate, Me.intIdDayAbsence)
                If oRet IsNot Nothing AndAlso oRet.ProgrammedEndDate.HasValue Then
                    Me.xEndDate = oRet.ProgrammedEndDate.Value
                Else
                    Me.xEndDate = Me.xBeginDate
                End If

                ViewState("EditProgrammedCause_ProgrammedCauseData") = oRet
            End If
            Return oRet
        End Get
        Set(ByVal value As roProgrammedCause)
            ViewState("EditProgrammedCause_ProgrammedCauseData") = value
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
                Me.xBeginDate = roTypes.Any2DateTime(Request.Params("BeginDate"))
            End If

            If Request.Params("NewRecord") IsNot Nothing Then
                Me.bolNew = (roTypes.Any2Integer(Request.Params("NewRecord")) = 1)
            End If

            If Request.Params("AbsenceID") IsNot Nothing Then
                Me.intIdDayAbsence = roTypes.Any2Integer(Request.Params("AbsenceID"))
            Else
                Me.intIdDayAbsence = 0
            End If
        Else
            Me.bolNew = False
            Me.xBeginDate = Date.MinValue.Date
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

            Me.ProgrammedCauseData = Nothing

            LoadCausesCombo()

            LoadData()

            If Me.bIsWin32 Then
                ' Se trata de una ausencia prevista creada con una versión Win32 de VT. Sólo permitimos verla, y borrarla
                Me.DisableControls(UpdatePanel1.Controls)
                Me.btOK.Visible = False
                Me.btCancel.Text = Me.Language.Keyword("Button.Close")
                'Me.lblInfo.Text = Me.Language.Translate("EditProgrammedIncidence.lblWin32", Me.DefaultScope)
                'Me.lblInfo.ForeColor = Drawing.Color.Red
            Else
                'Me.lblInfo.Text = Me.Language.Translate("EditProgrammedIncidence.lblInfo", Me.DefaultScope)
            End If
        End If

        If HelperSession.GetFeatureIsInstalledFromApplication("Feature\Absences") = False Then
            Me.Content_ABS00.Style("display") = ""
            Me.Content_ABS01.Style("display") = "none"
            Me.TABBUTTON_ABS01.Style("display") = "none"
            Me.TABBUTTON_ABS00.Attributes("class") = "bTab-active"
        End If

    End Sub

    Protected Sub btnReload_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnReload.Click
        Me.ProgrammedCauseData = Nothing
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

    Protected Sub SaveAbsence(ByVal bMusctCloseOnSave As Boolean)
        If txtBeginDate.Date.Date = #12:00:00 AM# Then
            ShowMessage(Me, "", Me.Language.Translate("InitialDateRequired", Me.DefaultScope))
            Exit Sub
        End If

        If txtEndDate.Date.Date = #12:00:00 AM# Then
            ShowMessage(Me, "", Me.Language.Translate("EndDateRequired", Me.DefaultScope))
            Exit Sub
        End If

        If Me.cmbCausesList.Value IsNot Nothing AndAlso roTypes.Any2String(Me.cmbCausesList.Value) <> "" Then

            Dim oProgIncidence As roProgrammedCause = Nothing

            If Me.bolNew Then
                oProgIncidence = New roProgrammedCause
            Else
                oProgIncidence = Me.ProgrammedCauseData()
            End If

            With oProgIncidence
                .IDEmployee = Request.Params("EmployeeID")
                .ProgrammedDate = txtBeginDate.Date.Date
                .ProgrammedEndDate = txtEndDate.Date.Date

                .IDCause = roTypes.Any2Integer(cmbCausesList.SelectedItem.Value)

                Dim dBeginTime As DateTime = txtNormalHourBegin.DateTime
                Dim dEndTime As DateTime = txtNormalHourEnd.DateTime

                If dBeginTime > dEndTime Then
                    dEndTime = dEndTime.AddDays(1)
                End If

                .BeginTime = dBeginTime
                .EndTime = dEndTime
                .Duration = roTypes.Any2Time(Format(Me.txtDuration.DateTime, HelperWeb.GetShortTimeFormat())).NumericValue
                .MinDuration = roTypes.Any2Time(Format(Me.txtMinDuration.DateTime, HelperWeb.GetShortTimeFormat())).NumericValue
                .Description = txtDescription.Text
            End With

            If ProgrammedCausesServiceMethods.SaveProgrammedCause(Me, oProgIncidence, True) Then
                ProgrammedCauseData = Nothing
                Me.bolNew = False
                Me.intIdDayAbsence = oProgIncidence.ID

                LoadData()
                Dim oLst = DocumentsServiceMethods.GetCauseAvailableDocumentTemplateByEmployee(Me.intEmployeeID, ProgrammedCauseData.IDCause, False, Me.Page, False)
                If oLst IsNot Nothing AndAlso oLst.Count > 0 AndAlso Not bMusctCloseOnSave Then
                    Me.CanClose = False
                    Me.MustRefresh = "0"
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
            End If
        Else
            ShowMessage(Me, "", Me.Language.Translate("EditProgrammedIncidence.CheckCause", Me.DefaultScope))
        End If

    End Sub

#End Region

#Region "Methods"

    Private Sub LoadCausesCombo()
        'Dim tb As DataTable = CausesServiceMethods.GetCausesShortList(Me.Page, True)
        Dim tb As DataTable = CausesServiceMethods.GetCausesShortListByRequestType(Me.Page, eCauseRequest.ProgrammedCause, True)

        Dim dvCauses As New DataView(tb)
        'dvCauses.RowFilter = "WorkingType = 0 AND IsHoliday = 0 AND DayType = 0 AND CustomType=0"

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
        Dim IDCause As Integer = roTypes.Any2Integer(cmbCausesList.SelectedItem.Value)
        Dim oCause As roCause = CausesServiceMethods.GetCauseByID(Me.Page, IDCause, False)
        If oCause IsNot Nothing Then
            If oCause.Absence_BetweenMin.HasValue Then
                txtNormalHourBegin.DateTime = oCause.Absence_BetweenMin.Value
            Else
                txtNormalHourBegin.DateTime = New Date(1899, 12, 30, 0, 0, 0)
            End If
            If oCause.Absence_BetweenMax.HasValue Then
                txtNormalHourEnd.DateTime = oCause.Absence_BetweenMax.Value
            Else
                txtNormalHourEnd.DateTime = New Date(1899, 12, 30, 0, 0, 0)
            End If
            If oCause.Absence_DurationMin.HasValue Then
                txtMinDuration.DateTime = oCause.Absence_DurationMin.Value
            Else
                txtMinDuration.DateTime = New Date(1899, 12, 30, 0, 0, 0)
            End If
            If oCause.Absence_DurationMax.HasValue Then
                txtDuration.DateTime = oCause.Absence_DurationMax.Value
            Else
                txtDuration.DateTime = New Date(1899, 12, 30, 0, 0, 0)
            End If
        Else
            txtNormalHourBegin.DateTime = New Date(1899, 12, 30, 0, 0, 0)
            txtNormalHourEnd.DateTime = New Date(1899, 12, 30, 0, 0, 0)
            txtMinDuration.DateTime = New Date(1899, 12, 30, 0, 0, 0)
            txtDuration.DateTime = New Date(1899, 12, 30, 0, 0, 0)
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
            If Me.xBeginDate = Nothing Then
                Me.txtBeginDate.Date = Now.Date
                Me.txtEndDate.Date = Now.Date
            Else
                Me.txtBeginDate.Date = Me.xBeginDate.Date
                Me.txtEndDate.Date = Me.xEndDate.Date
            End If
            Me.TABBUTTON_ABS01.Style("display") = "none"
        Else
            ' Es un registro existente
            LoadProgrammedCause(Me.intEmployeeID, If(Me.xBeginDate = Date.MinValue, Me.txtBeginDate.Date.Date, Me.xBeginDate.Date), Me.intIdDayAbsence, bIsWin32)
        End If

        Me.hdnIdEmployee.Value = Me.intEmployeeID
        Me.hdnBeginDate.Value = Me.xBeginDate.Date
        Me.hdnIdCause.Value = roTypes.Any2String(Me.cmbCausesList.Value)
        Me.hdnIdDayAbsence.Value = If(Not Me.bolNew, ProgrammedCauseData().ID, 0)

        Me.hdnIdAbsence.Value = If(Not Me.bolNew, ProgrammedCauseData().AbsenceID, -1)

        Me.AbsDocumentManagment.SetScope(Me.intEmployeeID, DocumentType.Employee, If(Not Me.bolNew, ProgrammedCauseData().AbsenceID, -1), ForecastType.AbsenceHours)
        Me.AbsDocumentPendingManagment.LoadAlerts(Me.intEmployeeID, DocumentType.Employee, If(Not Me.bolNew, ProgrammedCauseData().AbsenceID, -1), ForecastType.AbsenceHours)

        If Not Me.bolNew AndAlso Me.AbsDocumentManagment.HasDocuments Then
            Me.cmbCausesList.Enabled = False
        Else
            Me.cmbCausesList.Enabled = True
        End If
    End Sub

    Private Sub LoadProgrammedCause(ByVal EmployeeID As Integer, ByVal BeginDate As Date, ByVal AbsenceID As Integer, ByRef bIsWin32 As Boolean)

        Me.intEmployeeID = EmployeeID
        Me.xBeginDate = BeginDate.Date
        Me.intIdDayAbsence = AbsenceID

        Dim oProgAbsence As roProgrammedCause = ProgrammedCausesServiceMethods.GetProgrammedCause(Me.Page, EmployeeID, BeginDate, AbsenceID, True)
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
                    Dim oLst = DocumentsServiceMethods.GetCauseAvailableDocumentTemplateByEmployee(Me.intEmployeeID, oProgAbsence.IDCause.Value, False, Me.Page, False)
                    If oLst IsNot Nothing AndAlso oLst.Count > 0 Then
                        Me.TABBUTTON_ABS01.Style("display") = ""
                    Else
                        Me.TABBUTTON_ABS01.Style("display") = "none"
                    End If
                Else
                    Me.TABBUTTON_ABS01.Style("display") = "none"
                End If

            End If

            Me.txtBeginDate.Date = oProgAbsence.ProgrammedDate.Value
            If oProgAbsence.ProgrammedEndDate.HasValue Then
                Me.txtEndDate.Date = oProgAbsence.ProgrammedEndDate.Value
            Else
                Me.txtEndDate.Date = oProgAbsence.ProgrammedDate.Value
            End If

            Me.txtNormalHourBegin.DateTime = oProgAbsence.BeginTime
            Me.txtNormalHourEnd.DateTime = oProgAbsence.EndTime
            Me.txtDuration.DateTime = CDate(roTypes.Any2Time(oProgAbsence.Duration).Value)
            Me.txtMinDuration.DateTime = CDate(roTypes.Any2Time(oProgAbsence.MinDuration).Value)

            Me.txtDescription.Text = oProgAbsence.Description
            bIsWin32 = oProgAbsence.IsWin32

        End If

    End Sub

#End Region

End Class