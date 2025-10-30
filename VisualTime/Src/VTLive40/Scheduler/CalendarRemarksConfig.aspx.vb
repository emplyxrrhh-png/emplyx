Imports Robotics.Base.DTOs
Imports Robotics.Web.Base
Imports Robotics.Web.Base.API

Partial Class CalendarRemarksConfig
    Inherits PageBase

    Private Const FeatureAlias As String = "Calendar.Highlight"

#Region "Declarations"

    Private oPermission As Permission

#End Region

#Region "Properties"

    Private Property RemarksData(Optional ByVal bolReload As Boolean = False) As roCalendarPassportConfig
        Get
            Dim tb As roCalendarPassportConfig = ViewState("CalendarRemarks_RemarksData")

            If bolReload OrElse tb Is Nothing Then
                tb = CalendarServiceMethods.GetCalendarConfiguration(Me.Page)
                ViewState("CalendarRemarks_RemarksData") = tb
            End If

            Return tb
        End Get
        Set(ByVal value As roCalendarPassportConfig)
            ViewState("CalendarRemarks_RemarksData") = value
        End Set
    End Property

    Private Property CausesData() As DataView
        Get
            Dim tbCauses As DataTable = ViewState("Remarks_CausesData")
            Dim dv As DataView = Nothing
            If tbCauses IsNot Nothing Then
                dv = New DataView(tbCauses)
                dv.Sort = "Name ASC"
            End If
            If dv Is Nothing Then
                Dim tb As DataTable = CausesServiceMethods.GetCausesShortList(Me)
                If tb IsNot Nothing Then
                    dv = New DataView(tb)
                    dv.Sort = "Name ASC"
                    ViewState("Remarks_CausesData") = dv.Table
                End If
            End If
            Return dv
        End Get
        Set(ByVal value As DataView)
            If value IsNot Nothing Then
                ViewState("Remarks_CausesData") = value.Table
            Else
                ViewState("Remarks_CausesData") = Nothing
            End If
        End Set
    End Property

    Private Property ConceptsData() As DataView
        Get
            Dim tbCauses As DataTable = ViewState("Remarks_ConceptsData")
            Dim dv As DataView = Nothing
            If tbCauses IsNot Nothing Then
                dv = New DataView(tbCauses)
                dv.Sort = "Name ASC"
            End If
            If dv Is Nothing Then
                Dim tb As DataTable = API.ConceptsServiceMethods.GetConcepts(Me.Page)
                If tb IsNot Nothing Then
                    dv = New DataView(tb)
                    dv.Sort = "Name ASC"
                    ViewState("Remarks_ConceptsData") = dv.Table
                End If
            End If
            Return dv
        End Get
        Set(ByVal value As DataView)
            If value IsNot Nothing Then
                ViewState("Remarks_ConceptsData") = value.Table
            Else
                ViewState("Remarks_ConceptsData") = Nothing
            End If
        End Set
    End Property

#End Region

#Region "Events"

    Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        Me.InsertExtraJavascript("BrowserDetect", "~/Base/Scripts/BrowserDetect.js", , True)
        Me.InsertExtraJavascript("Cookies", "~/Base/Scripts/Cookies.js", , True)
        Me.InsertExtraJavascript("Generic", "~/Base/Scripts/Generic.js", , True)
        Me.InsertExtraJavascript("rgbcolor", "~/Base/Scripts/rgbcolor.js", , True)

        Me.InsertExtraJavascript("roTabContainerClient", "~/Base/Scripts/roTabContainerClient.js")
    End Sub

    Protected Sub Page_Load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.InsertCssIncludes()

        Me.oPermission = Me.GetFeaturePermission(FeatureAlias)
        If Me.oPermission >= Permission.Read Then
            Me.btAccept.Visible = (Me.oPermission >= Permission.Write)
            If Me.oPermission = Permission.Read Then
                Me.btCancel.Text = Me.Language.Keyword("Button.Close")
            End If

            If Not Me.IsPostBack Then
                Me.RemarksData = Nothing
                Me.CausesData = Nothing
            End If

            LoadCombos()

            If Not Me.IsPostBack Then
                LoadData()
            End If
        Else
            WLHelperWeb.RedirectAccessDenied(True)
        End If

    End Sub

    Protected Sub LoadCombos()
        Me.cmbCalendarConcept.ValueField = "ShortName"
        Me.cmbCalendarHolidays.ValueField = "ShortName"
        Me.cmbCalendarNotJustified.ValueField = "ShortName"
        Me.cmbCalendarOvertime.ValueField = "ShortName"
        Me.cmbCalendarWorking.ValueField = "ShortName"

        Me.cmbCalendarConcept.TextField = "Name"
        Me.cmbCalendarHolidays.TextField = "Name"
        Me.cmbCalendarNotJustified.TextField = "Name"
        Me.cmbCalendarOvertime.TextField = "Name"
        Me.cmbCalendarWorking.TextField = "Name"

        Me.cmbCalendarConcept.DataSource = ConceptsData
        Me.cmbCalendarHolidays.DataSource = ConceptsData
        Me.cmbCalendarNotJustified.DataSource = ConceptsData
        Me.cmbCalendarOvertime.DataSource = ConceptsData
        Me.cmbCalendarWorking.DataSource = ConceptsData

        Me.cmbCalendarConcept.DataBind()
        Me.cmbCalendarHolidays.DataBind()
        Me.cmbCalendarNotJustified.DataBind()
        Me.cmbCalendarOvertime.DataBind()
        Me.cmbCalendarWorking.DataBind()

        Me.cmbCauseRemark1.ValueType = GetType(Integer)
        Me.cmbCauseRemark2.ValueType = GetType(Integer)
        Me.cmbCauseRemark3.ValueType = GetType(Integer)

        Me.cmbCauseRemark1.ValueField = "Id"
        Me.cmbCauseRemark2.ValueField = "Id"
        Me.cmbCauseRemark3.ValueField = "Id"

        Me.cmbCauseRemark1.TextField = "Name"
        Me.cmbCauseRemark2.TextField = "Name"
        Me.cmbCauseRemark3.TextField = "Name"

        Me.cmbCauseRemark1.DataSource = CausesData
        Me.cmbCauseRemark2.DataSource = CausesData
        Me.cmbCauseRemark3.DataSource = CausesData

        Me.cmbCauseRemark1.DataBind()
        Me.cmbCauseRemark2.DataBind()
        Me.cmbCauseRemark3.DataBind()

        Me.cmbComparisonRemark1.ValueType = GetType(Integer)
        Me.cmbComparisonRemark2.ValueType = GetType(Integer)
        Me.cmbComparisonRemark3.ValueType = GetType(Integer)

        Me.cmbComparisonRemark1.Items.Clear()
        Me.cmbComparisonRemark2.Items.Clear()
        Me.cmbComparisonRemark3.Items.Clear()
        For Each CompareItem As RemarkCompare In System.Enum.GetValues(GetType(RemarkCompare))
            Me.cmbComparisonRemark1.Items.Add(Me.Language.Translate("CompareList." & System.Enum.GetName(GetType(RemarkCompare), CompareItem) & ".Caption", Me.DefaultScope), CInt(CompareItem))
            Me.cmbComparisonRemark2.Items.Add(Me.Language.Translate("CompareList." & System.Enum.GetName(GetType(RemarkCompare), CompareItem) & ".Caption", Me.DefaultScope), CInt(CompareItem))
            Me.cmbComparisonRemark3.Items.Add(Me.Language.Translate("CompareList." & System.Enum.GetName(GetType(RemarkCompare), CompareItem) & ".Caption", Me.DefaultScope), CInt(CompareItem))
        Next

    End Sub

    Protected Sub LoadData()
        Me.cmbCalendarConcept.SelectedItem = Me.cmbCalendarConcept.Items.FindByValue(Me.RemarksData.CalendarAccrual)
        Me.cmbCalendarHolidays.SelectedItem = Me.cmbCalendarHolidays.Items.FindByValue(Me.RemarksData.CalendarHolidays)
        Me.cmbCalendarNotJustified.SelectedItem = Me.cmbCalendarNotJustified.Items.FindByValue(Me.RemarksData.CalendarNotJustified)
        Me.cmbCalendarOvertime.SelectedItem = Me.cmbCalendarOvertime.Items.FindByValue(Me.RemarksData.CalendarOvertime)
        Me.cmbCalendarWorking.SelectedItem = Me.cmbCalendarWorking.Items.FindByValue(Me.RemarksData.CalendarWorking)

        Me.txtColorRemark1.Color = Drawing.ColorTranslator.FromWin32(Me.RemarksData.CalendarRemarks(0).Color)
        Me.txtColorRemark2.Color = Drawing.ColorTranslator.FromWin32(Me.RemarksData.CalendarRemarks(1).Color)
        Me.txtColorRemark3.Color = Drawing.ColorTranslator.FromWin32(Me.RemarksData.CalendarRemarks(2).Color)

        Me.cmbCauseRemark1.SelectedItem = Me.cmbCauseRemark1.Items.FindByValue(CInt(Me.RemarksData.CalendarRemarks(0).IdCause))
        Me.cmbCauseRemark2.SelectedItem = Me.cmbCauseRemark2.Items.FindByValue(CInt(Me.RemarksData.CalendarRemarks(1).IdCause))
        Me.cmbCauseRemark3.SelectedItem = Me.cmbCauseRemark3.Items.FindByValue(CInt(Me.RemarksData.CalendarRemarks(2).IdCause))

        Me.cmbComparisonRemark1.SelectedItem = Me.cmbComparisonRemark1.Items.FindByValue(CInt(Me.RemarksData.CalendarRemarks(0).Comparison))
        Me.cmbComparisonRemark2.SelectedItem = Me.cmbComparisonRemark2.Items.FindByValue(CInt(Me.RemarksData.CalendarRemarks(1).Comparison))
        Me.cmbComparisonRemark3.SelectedItem = Me.cmbComparisonRemark3.Items.FindByValue(CInt(Me.RemarksData.CalendarRemarks(2).Comparison))

        Me.txtValueRemark1.DateTime = New DateTime(1900, 1, 1, Me.RemarksData.CalendarRemarks(0).Value.Hour, Me.RemarksData.CalendarRemarks(0).Value.Minute, 0)
        Me.txtValueRemark2.DateTime = New DateTime(1900, 1, 1, Me.RemarksData.CalendarRemarks(1).Value.Hour, Me.RemarksData.CalendarRemarks(1).Value.Minute, 0)
        Me.txtValueRemark3.DateTime = New DateTime(1900, 1, 1, Me.RemarksData.CalendarRemarks(2).Value.Hour, Me.RemarksData.CalendarRemarks(2).Value.Minute, 0)
    End Sub

    Protected Sub btAccept_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btAccept.Click
        Me.SaveData()
    End Sub

#End Region

#Region "Methods"

    Private Sub SaveData()

        If Me.cmbCalendarConcept.SelectedItem IsNot Nothing Then Me.RemarksData.CalendarAccrual = Me.cmbCalendarConcept.SelectedItem.Value
        If Me.cmbCalendarHolidays.SelectedItem IsNot Nothing Then Me.RemarksData.CalendarHolidays = Me.cmbCalendarHolidays.SelectedItem.Value
        If Me.cmbCalendarNotJustified.SelectedItem IsNot Nothing Then Me.RemarksData.CalendarNotJustified = Me.cmbCalendarNotJustified.SelectedItem.Value
        If Me.cmbCalendarOvertime.SelectedItem IsNot Nothing Then Me.RemarksData.CalendarOvertime = Me.cmbCalendarOvertime.SelectedItem.Value
        If Me.cmbCalendarWorking.SelectedItem IsNot Nothing Then Me.RemarksData.CalendarWorking = Me.cmbCalendarWorking.SelectedItem.Value

        Me.RemarksData.CalendarRemarks(0).Color = Drawing.ColorTranslator.ToWin32(Me.txtColorRemark1.Color)
        Me.RemarksData.CalendarRemarks(1).Color = Drawing.ColorTranslator.ToWin32(Me.txtColorRemark2.Color)
        Me.RemarksData.CalendarRemarks(2).Color = Drawing.ColorTranslator.ToWin32(Me.txtColorRemark3.Color)

        If (Me.cmbCauseRemark1.SelectedItem IsNot Nothing) Then
            Me.RemarksData.CalendarRemarks(0).IdCause = Me.cmbCauseRemark1.SelectedItem.Value
        Else
            Me.RemarksData.CalendarRemarks(0).IdCause = -1
        End If
        If (Me.cmbCauseRemark2.SelectedItem IsNot Nothing) Then
            Me.RemarksData.CalendarRemarks(1).IdCause = Me.cmbCauseRemark2.SelectedItem.Value
        Else
            Me.RemarksData.CalendarRemarks(1).IdCause = -1
        End If
        If (Me.cmbCauseRemark3.SelectedItem IsNot Nothing) Then
            Me.RemarksData.CalendarRemarks(2).IdCause = Me.cmbCauseRemark3.SelectedItem.Value
        Else
            Me.RemarksData.CalendarRemarks(2).IdCause = -1
        End If

        Me.RemarksData.CalendarRemarks(0).Comparison = Me.cmbComparisonRemark1.SelectedItem.Value
        Me.RemarksData.CalendarRemarks(1).Comparison = Me.cmbComparisonRemark2.SelectedItem.Value
        Me.RemarksData.CalendarRemarks(2).Comparison = Me.cmbComparisonRemark3.SelectedItem.Value

        Me.RemarksData.CalendarRemarks(0).Value = Me.txtValueRemark1.DateTime
        Me.RemarksData.CalendarRemarks(1).Value = Me.txtValueRemark2.DateTime
        Me.RemarksData.CalendarRemarks(2).Value = Me.txtValueRemark3.DateTime

        If CalendarServiceMethods.SaveCalendarConfiguration(Me.Page, Me.RemarksData) Then
            WLHelperWeb.IniContext()
            Me.hdnCanClose.Value = "1"
        Else
            Me.hdnCanClose.Value = "0"
        End If
    End Sub

#End Region

End Class