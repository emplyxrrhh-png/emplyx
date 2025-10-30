Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Cause
Imports Robotics.VTBase
Imports Robotics.Web.Base
Imports Robotics.Web.Base.API
Imports Robotics.Web.Base.HelperWeb

Partial Class Forms_EditProgrammedOvertimes
    Inherits PageBase

#Region "Declarations"

    Private intEmployeeID As Integer
    Private intProgrammedOvertimeID As Integer
    Private bIsNew As Boolean

    Private Const EmployeeFeatureAlias As String = "Employees"
    Private oPermission As Permission
    Private oPermissionEmp As Permission

#End Region

#Region "Properties"

    Public Property ProgrammedOvertimeData() As roProgrammedOvertime
        Get
            Dim oRet As roProgrammedOvertime = ViewState("EditProgrammedOvertime_ProgrammedOvertimeData")
            If oRet Is Nothing Then
                oRet = ProgrammedOvertimesServiceMethods.GetProgrammedOvertimeById(Me.Page, Me.intProgrammedOvertimeID, True)

                ViewState("EditProgrammedOvertime_ProgrammedOvertimeData") = oRet
            End If
            Return oRet
        End Get
        Set(ByVal value As roProgrammedOvertime)
            ViewState("EditProgrammedOvertime_ProgrammedOvertimeData") = value
        End Set
    End Property

#End Region

#Region "Events"

    Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        Me.InsertExtraJavascript("Cookies", "~/Base/Scripts/Cookies.js", , True)
        Me.InsertExtraJavascript("Generic", "~/Base/Scripts/Generic.js", , True)
        Me.InsertExtraJavascript("GridViewHelper", "~/Base/Scripts/GridViewHelper.js", , True)
        Me.InsertExtraJavascript("moment", "~/Base/Scripts/moment.min.js",, True)
        Me.InsertExtraJavascript("momenttz", "~/Base/Scripts/moment-tz.min.js",, True)
        Me.InsertExtraJavascript("roDate", "~/Base/Scripts/Live/roDateManager.min.js",, True)
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.InsertCssIncludes(Me.Page)

        If Not Me.HasFeaturePermission("Employees.Type", Permission.Write) Then
            WLHelperWeb.RedirectAccessDenied(True)
            Exit Sub
        End If

        Me.intEmployeeID = roTypes.Any2Integer(Request.Params("EmployeeID"))

        If Request.Params("ProgrammedHolidaysID") IsNot Nothing Then
            Me.intProgrammedOvertimeID = roTypes.Any2Integer(Request.Params("ProgrammedHolidaysID"))
        Else
            Me.intProgrammedOvertimeID = 0
        End If

        Me.bIsNew = (Me.intProgrammedOvertimeID = -1)

        If Me.bIsNew = False Then
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

            Me.ProgrammedOvertimeData = Nothing

            LoadCausesCombo()

            LoadData()

        End If

    End Sub

    Protected Sub btnReload_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnReload.Click
        Me.ProgrammedOvertimeData = Nothing
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

            Dim oOvertime As roProgrammedOvertime = Nothing

            If Me.bIsNew Then
                oOvertime = New roProgrammedOvertime
            Else
                oOvertime = Me.ProgrammedOvertimeData()
            End If

            Dim duration As DateTime

            Dim dBeginTime As DateTime = New DateTime(1899, 12, 30, txtNormalHourBegin.DateTime.Hour, txtNormalHourBegin.DateTime.Minute, 0)
            Dim dEndTime As DateTime = New DateTime(1899, 12, 30, txtNormalHourEnd.DateTime.Hour, txtNormalHourEnd.DateTime.Minute, 0)

            With oOvertime
                .IDEmployee = Request.Params("EmployeeID")
                .ProgrammedBeginDate = txtBeginDate.Date.Date
                .ProgrammedEndDate = txtEndDate.Date.Date

                .IDCause = roTypes.Any2Integer(cmbCausesList.SelectedItem.Value)

                If dBeginTime > dEndTime Then
                    dEndTime = dEndTime.AddDays(1)
                End If

                .BeginTime = dBeginTime
                .EndTime = dEndTime

                duration = New DateTime(.EndTime.Ticks - .BeginTime.Ticks)

                .Duration = roTypes.Any2Time(duration.ToShortTimeString()).NumericValue
                .MinDuration = roTypes.Any2Time(Format(Me.txtMinDuration.DateTime, HelperWeb.GetShortTimeFormat())).NumericValue
                .Description = txtDescription.Text
            End With

            oOvertime = ProgrammedOvertimesServiceMethods.SaveProgrammedOvertime(oOvertime, Me.Page, True)

            If oOvertime.ID > 0 AndAlso ProgrammedOvertimesServiceMethods.LastResult = OvertimeResultEnum.NoError Then
                ProgrammedOvertimeData = oOvertime
                Me.bIsNew = False
                Me.intProgrammedOvertimeID = oOvertime.ID
                LoadData()
                Dim oLst = DocumentsServiceMethods.GetCauseAvailableDocumentTemplateByEmployee(Me.intEmployeeID, oOvertime.IDCause, False, Me.Page, False)
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

            txtDuration.Text = duration.ToShortTimeString()
        Else
            ShowMessage(Me, "", Me.Language.Translate("EditProgrammedIncidence.CheckCause", Me.DefaultScope))
        End If

        If HelperSession.GetFeatureIsInstalledFromApplication("Feature\Absences") = False Then
            Me.Content_ABS00.Style("display") = ""
            Me.Content_ABS01.Style("display") = "none"
            Me.TABBUTTON_ABS01.Style("display") = "none"
            Me.TABBUTTON_ABS00.Attributes("class") = "bTab-active"
        End If
    End Sub

    Protected Sub cmbCausesList_ValueChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmbCausesList.SelectedIndexChanged
        Dim IDCause As Integer = roTypes.Any2Integer(cmbCausesList.Value)

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

#End Region

#Region "Methods"

    Private Sub LoadCausesCombo()

        Dim tb As DataTable = CausesServiceMethods.GetCausesShortListByRequestType(Me.Page, eCauseRequest.ProgrammedOvertime, True)

        Dim dvCauses As New DataView(tb)
        dvCauses.RowFilter = "WorkingType = 1 AND IsHoliday = 0 AND DayType = 0 AND CustomType=0"

        dvCauses.Sort = "Name"
        tb = dvCauses.ToTable

        cmbCausesList.Value = Nothing
        cmbCausesList.DataSource = tb
        cmbCausesList.TextField = "Name"
        cmbCausesList.ValueField = "Id"
        cmbCausesList.ValueType = GetType(Integer)
        cmbCausesList.DataBind()
    End Sub

    Private Sub LoadData()

        If Me.bIsNew Then
            ' Es un registro nuevo
            Me.txtBeginDate.Date = DateTime.Now.Date
            Me.txtEndDate.Date = DateTime.Now.Date
            Me.txtMinDuration.DateTime = New DateTime(1899, 12, 30, 0, 0, 0)

            Me.txtNormalHourBegin.DateTime = New DateTime(1899, 12, 30, 0, 0, 0)
            Me.txtNormalHourEnd.DateTime = New DateTime(1899, 12, 30, 0, 0, 0).AddDays(1).AddMinutes(-1)

            Dim duration As New DateTime(txtNormalHourEnd.DateTime.Ticks - txtNormalHourBegin.DateTime.Ticks)
            Me.txtDuration.Text = duration.ToShortTimeString()
            Me.TABBUTTON_ABS01.Style("display") = "none"
        Else
            ' Es un registro existente
            LoadProgrammedOvertime(Me.intEmployeeID, Me.intProgrammedOvertimeID)
        End If

        hdnIdEmployee.Value = Me.intEmployeeID
        hdnIdProgrammedOvertime.Value = Me.intProgrammedOvertimeID
        hdnIdCause.Value = roTypes.Any2String(Me.cmbCausesList.Value)

        Me.AbsDocumentManagment.SetScope(Me.intEmployeeID, DocumentType.Employee, If(Not Me.bIsNew, ProgrammedOvertimeData().ID, -1), ForecastType.OverWork)

        Me.AbsDocumentPendingManagment.LoadAlerts(Me.intEmployeeID, DocumentType.Employee, If(Not Me.bIsNew, ProgrammedOvertimeData().ID, -1), ForecastType.OverWork)

        If Not Me.bIsNew AndAlso Me.AbsDocumentManagment.HasDocuments Then
            Me.cmbCausesList.Enabled = False
        Else
            Me.cmbCausesList.Enabled = True
        End If

    End Sub

    Private Sub LoadProgrammedOvertime(ByVal EmployeeID As Integer, ByVal ProgrammedOvertimeId As Integer)

        Dim oOvertime As roProgrammedOvertime = ProgrammedOvertimesServiceMethods.GetProgrammedOvertimeById(Me.Page, ProgrammedOvertimeId, True)
        If oOvertime IsNot Nothing Then

            Me.cmbCausesList.Value = oOvertime.IDCause

            If Me.cmbCausesList.SelectedItem Is Nothing Then
                Dim oList As New DevExpress.Web.ListEditItem
                Dim oCause As roCause = CausesServiceMethods.GetCauseByID(Me.Page, oOvertime.IDCause, False)

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
            End If

            Me.txtBeginDate.Date = oOvertime.ProgrammedBeginDate
            Me.txtEndDate.Date = oOvertime.ProgrammedEndDate

            Me.txtNormalHourBegin.DateTime = oOvertime.BeginTime
            Me.txtNormalHourEnd.DateTime = oOvertime.EndTime
            Me.txtDuration.Text = CDate(roTypes.Any2Time(oOvertime.Duration).Value).ToShortTimeString()
            Me.txtMinDuration.DateTime = CDate(roTypes.Any2Time(oOvertime.MinDuration).Value)

            Me.txtDescription.Text = oOvertime.Description

            If HelperSession.GetFeatureIsInstalledFromApplication("Feature\Absences") = True Then
                Dim oLst = DocumentsServiceMethods.GetCauseAvailableDocumentTemplateByEmployee(Me.intEmployeeID, oOvertime.IDCause, False, Me.Page, False)
                If oLst IsNot Nothing AndAlso oLst.Count > 0 Then
                    Me.TABBUTTON_ABS01.Style("display") = ""
                Else
                    Me.TABBUTTON_ABS01.Style("display") = "none"
                End If
            End If
        End If

    End Sub

#End Region

End Class