Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Cause
Imports Robotics.VTBase
Imports Robotics.Web.Base
Imports Robotics.Web.Base.API
Imports Robotics.Web.Base.HelperWeb
Imports WebGrease

Partial Class Forms_EditProgrammedHolidays
    Inherits PageBase

#Region "Declarations"

    Private intProgrammedHolidaysID As Integer
    Private intEmployeeID As Integer
    Private bolNew As Boolean

    Private Const EmployeeFeatureAlias As String = "Employees"
    Private oPermission As Permission
    Private oPermissionEmp As Permission

#End Region

#Region "Properties"

    Public Property ProgrammedHolidaysData() As roProgrammedHoliday
        Get
            Dim oRet As roProgrammedHoliday = ViewState("EditProgrammedHolidays_ProgrammedholdaysData")
            If oRet Is Nothing Then
                oRet = ProgrammedHolidaysServiceMethods.GetProgrammedHolidayById(Me.Page, Me.intProgrammedHolidaysID, True)

                ViewState("EditProgrammedHolidays_ProgrammedholdaysData") = oRet
            End If
            Return oRet
        End Get
        Set(ByVal value As roProgrammedHoliday)
            ViewState("EditProgrammedHolidays_ProgrammedholdaysData") = value
        End Set
    End Property

#End Region

#Region "Events"

    Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        Me.InsertExtraJavascript("Cookies", "~/Base/Scripts/Cookies.js", , True)
        Me.InsertExtraJavascript("Generic", "~/Base/Scripts/Generic.js", , True)
        Me.InsertExtraJavascript("GridViewHelper", "~/Base/Scripts/GridViewHelper.js", , True)
        Me.InsertExtraJavascript("roOptPanelClientGroup", "~/Base/Scripts/roOptPanelClientGroup.js",, True)
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

        Me.intProgrammedHolidaysID = roTypes.Any2Integer(Request.Params("ProgrammedHolidaysID"))
        Me.intEmployeeID = roTypes.Any2Integer(Request.Params("EmployeeID"))

        Me.bolNew = (intProgrammedHolidaysID < 0)

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
            Me.ProgrammedHolidaysData = Nothing

            LoadCausesCombo()
            LoadData()

            hdnIdEmployee.Value = Me.intEmployeeID
            hdnIdCause.Value = roTypes.Any2String(Me.cmbCausesList.Value)
        End If

    End Sub

    Protected Sub btOK_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btOK.Click
        If txtBeginDate.Date.Date = #12:00:00 AM# OrElse txtEndDate.Date.Date = #12:00:00 AM# Then
            ShowMessage(Me, "", Me.Language.Translate("InitialDateRequired", Me.DefaultScope))
            Exit Sub
        End If

        If Me.cmbCausesList.Value IsNot Nothing AndAlso roTypes.Any2String(Me.cmbCausesList.Value) <> "" Then

            Dim dBeginTime As DateTime = txtNormalHourBegin.DateTime
            Dim dEndTime As DateTime = txtNormalHourEnd.DateTime

            Dim beginDay = txtBeginDate.Date.Date
            Dim endDay = txtEndDate.Date.Date

            While (beginDay <= endDay)

                Dim oProgIncidence As roProgrammedHoliday = Nothing

                If Me.bolNew Then
                    oProgIncidence = New roProgrammedHoliday
                Else
                    oProgIncidence = Me.ProgrammedHolidaysData()
                End If

                With oProgIncidence
                    .IDEmployee = Request.Params("EmployeeID")
                    .ProgrammedDate = beginDay

                    .IDCause = roTypes.Any2Integer(cmbCausesList.SelectedItem.Value)
                    .Duration = roTypes.Any2Time(txtDuration.Text).NumericValue

                    If Me.optCompleteDay.Checked Then
                        .AllDay = True
                    Else
                        .AllDay = False
                    End If

                    If dBeginTime > dEndTime Then
                        dEndTime = dEndTime.AddDays(1)
                    End If

                    .BeginTime = dBeginTime
                    .EndTime = dEndTime
                    .Description = txtDescription.Text
                End With

                Dim duration As New DateTime(dEndTime.Ticks - dBeginTime.Ticks)

                oProgIncidence = ProgrammedHolidaysServiceMethods.SaveProgrammedHoliday(oProgIncidence, Me.Page, True)
                If oProgIncidence.ID > 0 AndAlso ProgrammedHolidaysServiceMethods.LastResult = HolidayResultEnum.NoError Then
                    Me.CanClose = True
                    Me.MustRefresh = "5"
                    Me.hdnParams_PageBase.Value = "SAVE_PROGRAMMED_ABSENCE"
                Else
                    Me.CanClose = False
                    Me.MustRefresh = "0"
                End If

                txtDuration.Text = duration.ToShortTimeString()

                beginDay = beginDay.AddDays(1)
            End While
        Else
            ShowMessage(Me, "", Me.Language.Translate("EditProgrammedIncidence.CheckCause", Me.DefaultScope))
        End If

    End Sub

#End Region

#Region "Methods"

    Private Sub LoadCausesCombo()
        'Dim tb As DataTable = CausesServiceMethods.GetCausesShortList(Me.Page, True)
        Dim tb As DataTable = CausesServiceMethods.GetCausesShortListByRequestType(Me.Page, eCauseRequest.PlannedHolidays, True)

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
        Else
            txtNormalHourBegin.DateTime = New Date(1899, 12, 30, 0, 0, 0)
            txtNormalHourEnd.DateTime = New Date(1899, 12, 30, 0, 0, 0)
            txtDuration.Text = (New Date(1899, 12, 30, 0, 0, 0)).ToShortTimeString()
        End If
    End Sub

    Private Sub LoadData()

        If Me.bolNew Then
            ' Es un registro nuevo
            Me.txtBeginDate.Date = Now
            Me.txtEndDate.Date = Now
            Me.optCompleteDay.Checked = True
            Me.optPanelOnlyHours.Checked = False
        Else
            Dim otmpProgHolidays = Me.ProgrammedHolidaysData
            ' Es un registro existente
            Me.cmbCausesList.Value = otmpProgHolidays.IDCause

            If Me.cmbCausesList.SelectedItem Is Nothing Then
                Dim oList As New DevExpress.Web.ListEditItem
                Dim oCause As roCause = CausesServiceMethods.GetCauseByID(Me.Page, otmpProgHolidays.IDCause, False)

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

            Me.txtBeginDate.Date = otmpProgHolidays.ProgrammedDate
            Me.txtEndDate.Date = otmpProgHolidays.ProgrammedDate

            Me.txtNormalHourBegin.DateTime = otmpProgHolidays.BeginTime
            Me.txtNormalHourEnd.DateTime = otmpProgHolidays.EndTime
            Me.txtDuration.Text = CDate(roTypes.Any2Time(otmpProgHolidays.Duration).Value).ToShortTimeString()

            Me.txtDescription.Text = otmpProgHolidays.Description

            If otmpProgHolidays.AllDay Then
                Me.optCompleteDay.Checked = True
                Me.optPanelOnlyHours.Checked = False
            Else
                Me.optCompleteDay.Checked = False
                Me.optPanelOnlyHours.Checked = True
            End If
        End If

    End Sub

#End Region

End Class