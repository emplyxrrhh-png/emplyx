Imports DevExpress.Web
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTEmployees.Contract
Imports Robotics.Base.VTEmployees.LabAgree
Imports Robotics.VTBase
Imports Robotics.Web.Base
Imports Robotics.Web.Base.API

Partial Class WebUserControls_QueryEmployeeScheduleRules
    Inherits UserControlBase

    <Runtime.Serialization.DataContract()>
    Private Class CallbackRequest

        <Runtime.Serialization.DataMember(Name:="aTab")>
        Public aTab As Integer

        <Runtime.Serialization.DataMember(Name:="ID")>
        Public ID As String

        <Runtime.Serialization.DataMember(Name:="IDLabAgree")>
        Public IDLabAgree As String

        <Runtime.Serialization.DataMember(Name:="action")>
        Public Action As String

        <Runtime.Serialization.DataMember(Name:="StampParam")>
        Public StampParam As Double

        <Runtime.Serialization.DataMember(Name:="resultClientAction")>
        Public resultClientAction As String

    End Class

    <Serializable()>
    <Runtime.Serialization.DataContract()>
    Private Class WeekDayInfo

        <Runtime.Serialization.DataMember(Name:="ID")>
        Public ID As Integer

        <Runtime.Serialization.DataMember(Name:="Name")>
        Public Name As String

    End Class

    Private Property LaboralDays(Optional ByVal bolReload As Boolean = False) As Generic.List(Of WeekDayInfo)
        Get
            Dim dic = ViewState("ContractLabAgree_WeekDaysDictionary")

            If dic Is Nothing Then
                dic = New Generic.List(Of WeekDayInfo)

                For n As Integer = 1 To 7
                    Dim item As New WeekDayInfo With {
                        .ID = If(n = 7, 0, n),
                        .Name = Me.Language.Keyword("weekday." & n.ToString)
                    }
                    dic.Add(item)
                Next

                ViewState("ContractLabAgree_WeekDaysDictionary") = dic
            End If

            Return dic
        End Get
        Set(value As Generic.List(Of WeekDayInfo))
            ViewState("ContractLabAgree_WeekDaysDictionary") = value
        End Set
    End Property

    Private Property QueryEmployeeScheduleRules(Optional ByVal bolReload As Boolean = False) As Generic.List(Of roScheduleRule)
        Get

            Dim tbValues As Generic.List(Of roScheduleRule) = Session("Query_EmployeeScheduleRules")

            If bolReload Or tbValues Is Nothing Then

                Dim oList As New Generic.List(Of roScheduleRule)

                If roTypes.Any2String(Session("Contract_SelectedID")) <> String.Empty Then
                    oList = ScheduleRulesServiceMethods.GetEmployeeCurrentScheduleRules(Me.Page, roTypes.Any2String(Session("Contract_SelectedID"))).ToList
                End If

                tbValues = oList
                Session("Query_EmployeeScheduleRules") = tbValues
            End If

            tbValues = tbValues.FindAll(Function(x) x.RuleType = ScheduleRuleBaseType.User)

            Return tbValues

        End Get
        Set(ByVal value As Generic.List(Of roScheduleRule))
            If value IsNot Nothing Then
                Session("Query_EmployeeScheduleRules") = value
            Else
                Session("Query_EmployeeScheduleRules") = Nothing
            End If
        End Set
    End Property

    Private Property QueryEmployeeScheduleRulesSystem(Optional ByVal bolReload As Boolean = False) As Generic.List(Of roScheduleRule)
        Get

            Dim tbValues As Generic.List(Of roScheduleRule) = Session("Query_EmployeeScheduleRules")

            If bolReload Or tbValues Is Nothing Then

                Dim oList As New Generic.List(Of roScheduleRule)

                If roTypes.Any2String(Session("Contract_SelectedID")) <> String.Empty Then
                    oList = ScheduleRulesServiceMethods.GetContractScheduleRules(Me.Page, roTypes.Any2String(Session("Contract_SelectedID"))).ToList
                End If

                tbValues = oList
                Session("Query_EmployeeScheduleRules") = tbValues

            End If

            tbValues = tbValues.FindAll(Function(x) x.RuleType = ScheduleRuleBaseType.System)

            Return tbValues

        End Get
        Set(ByVal value As Generic.List(Of roScheduleRule))
            If value IsNot Nothing Then
                Session("Query_EmployeeScheduleRules") = value
            Else
                Session("Query_EmployeeScheduleRules") = Nothing
            End If
        End Set
    End Property

    Private oPermission As Permission
    Private intEmployeeID As Integer

    Public Sub New()

    End Sub

    Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        Dim cacheManager As New Robotics.Web.Base.NoCachePageBase

        cacheManager.InsertJavascriptIncludes(Me.Parent.Page)

        cacheManager.InsertExtraJavascript("Ajax", "~/Base/Scripts/Ajax.js", Me.Parent.Page)
        cacheManager.InsertExtraJavascript("jsDate", "~/Base/Scripts/jsDate.js", Me.Parent.Page)

        cacheManager.InsertExtraJavascript("webUserForms", "~/Base/Scripts/webUserForms.js", Me.Parent.Page)
        cacheManager.InsertExtraJavascript("frmEditScheduleRules", "~/LabAgree/Scripts/frmEditScheduleRules.js", Me.Parent.Page)

        cacheManager.InsertExtraJavascript("GenericData", "~/Base/Scripts/GenericData.js", Me.Parent.Page)
        cacheManager.InsertExtraJavascript("roOptionPanelClient", "~/Base/Scripts/roOptionPanelClient.js", Me.Parent.Page)
        cacheManager.InsertExtraJavascript("roOptPanelClientGroup", "~/Base/Scripts/roOptPanelClientGroup.js", Me.Parent.Page)

    End Sub

    Protected Sub Page_Load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.oPermission = Permission.Read

        LoadStaticInfo()
        If Not Me.IsPostBack Then
            CreateScheduleRulesColumns()
        End If

    End Sub

    Public Sub InitControl(ByVal idemployee As Integer)
        Me.intEmployeeID = idemployee

        LoadData()
    End Sub

    Private Sub LoadStaticInfo()

        tbWorkingDays.Items.Clear()
        For Each oDay In LaboralDays
            tbWorkingDays.Items.Add(oDay.Name, oDay.ID)
        Next

    End Sub

    Private Sub LoadData()

        If Me.oPermission < Permission.Write Then
            Me.DisableControls()
        End If

        Dim result As String = "OK"
        Try

            Dim oContract As roContract = API.ContractsServiceMethods.GetActiveContract(Me.Page, Me.intEmployeeID, False)
            If API.ContractsServiceMethods.LastError.Result = ContractsResultEnum.ContractNotFound Then
                Return
            End If

            Session("Contract_SelectedID") = oContract.IDContract

            BindQueryEmployeeRules(True)

            Dim labAgreeRules As New Generic.List(Of roScheduleRule)
            If oContract.LabAgree IsNot Nothing Then labAgreeRules = ScheduleRulesServiceMethods.GetLabAgreeScheduleRules(Me.Page, oContract.LabAgree.ID).ToList

            Dim tmpRule As roScheduleRule = Nothing
            Dim oMaxAnualHours As roScheduleRule = Me.QueryEmployeeScheduleRulesSystem.Find(Function(x) x.IDRule = ScheduleRuleType.MinMaxExpectedHours)
            If oMaxAnualHours IsNot Nothing Then
                Me.txtYearHours.Value = CType(oMaxAnualHours, roScheduleRule_MinMaxExpectedHours).MaximumWorkingHours
                Me.txtFork.Value = CType(oMaxAnualHours, roScheduleRule_MinMaxExpectedHours).MaximumWorkingHoursFork
            Else
                tmpRule = labAgreeRules.Find(Function(x) x.IDRule = ScheduleRuleType.MinMaxExpectedHours)
                If tmpRule IsNot Nothing Then
                    Me.txtYearHours.Value = CType(tmpRule, roScheduleRule_MinMaxExpectedHours).MaximumWorkingHours
                    Me.txtFork.Value = CType(tmpRule, roScheduleRule_MinMaxExpectedHours).MaximumWorkingHoursFork
                End If
            End If

            Dim oMaxYearHolidays As roScheduleRule = Me.QueryEmployeeScheduleRulesSystem.Find(Function(x) x.IDRule = ScheduleRuleType.MaxHolidays)
            If oMaxYearHolidays IsNot Nothing Then
                Me.txtYearHolidays.Value = CType(oMaxYearHolidays, roScheduleRule_MaxHolidays).MaximumHoliDays
            Else
                tmpRule = labAgreeRules.Find(Function(x) x.IDRule = ScheduleRuleType.MaxHolidays)
                If tmpRule IsNot Nothing Then
                    Me.txtYearHolidays.Value = CType(tmpRule, roScheduleRule_MaxHolidays).MaximumHoliDays
                End If
            End If

            Dim oWorkOnHolidays As roScheduleRule = Me.QueryEmployeeScheduleRulesSystem.Find(Function(x) x.IDRule = ScheduleRuleType.WorkOnFestive)
            If oWorkOnHolidays IsNot Nothing Then
                Me.chkCanWorkOnFeastDays.Checked = Not CType(oWorkOnHolidays, roScheduleRule_WorkOnFestive).Enabled
            Else
                tmpRule = labAgreeRules.Find(Function(x) x.IDRule = ScheduleRuleType.WorkOnFestive)
                If tmpRule IsNot Nothing Then
                    Me.chkCanWorkOnFeastDays.Checked = Not CType(tmpRule, roScheduleRule_WorkOnFestive).Enabled
                End If
            End If

            Dim oWorkOnWeekend As roScheduleRule = Me.QueryEmployeeScheduleRulesSystem.Find(Function(x) x.IDRule = ScheduleRuleType.WorkOnWeekend)
            If oWorkOnWeekend IsNot Nothing Then
                Me.chkCanWorkOnNonWorkingDays.Checked = Not CType(oWorkOnWeekend, roScheduleRule_WorkOnWeekend).Enabled
                Me.tbWorkingDays.Value = CType(oWorkOnWeekend, roScheduleRule_WorkOnWeekend).LabourDaysIndex
            Else
                tmpRule = labAgreeRules.Find(Function(x) x.IDRule = ScheduleRuleType.WorkOnWeekend)
                If tmpRule IsNot Nothing Then
                    Me.chkCanWorkOnNonWorkingDays.Checked = Not CType(tmpRule, roScheduleRule_WorkOnWeekend).Enabled
                    Me.tbWorkingDays.Value = CType(tmpRule, roScheduleRule_WorkOnWeekend).LabourDaysIndex
                End If
            End If
        Catch ex As Exception
            result = "KO"
        Finally

        End Try

    End Sub

#Region "Grid ScheduleRules"

    Private Sub CreateScheduleRulesColumns()

        Dim GridColumn As GridViewDataTextColumn
        Dim GridColumnCommand As GridViewCommandColumn
        Dim GridCheckCommand As GridViewDataCheckColumn

        Dim VisibleIndex As Integer = 0

        Me.QueryEmployeeRules.Columns.Clear()
        Me.QueryEmployeeRules.KeyFieldName = "Id"
        Me.QueryEmployeeRules.SettingsText.EmptyDataRow = " "
        Me.QueryEmployeeRules.Settings.VerticalScrollBarMode = ScrollBarMode.Auto

        If Me.oPermission = Permission.Admin Or Me.oPermission >= Permission.Write Then
            Me.QueryEmployeeRules.SettingsEditing.Mode = GridViewEditingMode.Inline
        End If

        'Clave
        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = "Name"
        GridColumn.FieldName = "Id"
        GridColumn.ReadOnly = False
        GridColumn.Visible = False
        Me.QueryEmployeeRules.Columns.Add(GridColumn)

        Dim obj As New roStartupValue

        If Me.oPermission >= Permission.Read Then

            GridColumnCommand = New GridViewCommandColumn()
            GridColumnCommand.ButtonRenderMode = GridCommandButtonRenderMode.Image

            Dim editButton As GridViewCommandColumnCustomButton = New GridViewCommandColumnCustomButton()
            editButton.ID = "ShowDetailButton"
            editButton.Image.Url = "~/Base/Images/Grid/edit.png"
            editButton.Text = Me.Language.Translate("GridContractScheduleRules.Column.Edit", DefaultScope) 'Mostrar detalles"

            GridColumnCommand.CustomButtons.Add(editButton)
            GridColumnCommand.ShowEditButton = False
            GridColumnCommand.ShowDeleteButton = False
            GridColumnCommand.Caption = " "
            GridColumnCommand.VisibleIndex = VisibleIndex
            GridColumnCommand.CellStyle.HorizontalAlign = HorizontalAlign.Center
            GridColumnCommand.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
            GridColumnCommand.Width = 32
            VisibleIndex = VisibleIndex + 1

            Me.QueryEmployeeRules.Columns.Add(GridColumnCommand)

        End If

        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = Me.Language.Translate("GridContractScheduleRules.Column.LabAgree", DefaultScope) '"Fecha"
        GridColumn.FieldName = "Id"
        GridColumn.Visible = False
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        Me.QueryEmployeeRules.Columns.Add(GridColumn)

        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = Me.Language.Translate("GridContractScheduleRules.Column.LabAgree", DefaultScope) '"Fecha"
        GridColumn.FieldName = "IdLabAgree"
        GridColumn.Visible = False
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        Me.QueryEmployeeRules.Columns.Add(GridColumn)

        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = Me.Language.Translate("GridContractScheduleRules.Column.Name", DefaultScope) '"Fecha"
        GridColumn.FieldName = "RuleName"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        GridColumn.UnboundType = DevExpress.Data.UnboundColumnType.String
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        Me.QueryEmployeeRules.Columns.Add(GridColumn)

        GridCheckCommand = New GridViewDataCheckColumn()
        GridCheckCommand.Caption = Me.Language.Translate("GridContractScheduleRules.Column.Active", DefaultScope) '"Fecha"
        GridCheckCommand.FieldName = "Enabled"
        GridCheckCommand.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridCheckCommand.ReadOnly = True
        GridCheckCommand.UnboundType = DevExpress.Data.UnboundColumnType.String
        GridCheckCommand.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridCheckCommand.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        GridCheckCommand.Width = 80
        Me.QueryEmployeeRules.Columns.Add(GridCheckCommand)

    End Sub

    Protected Sub QueryEmployeeRules_CustomUnboundColumnData(sender As Object, e As ASPxGridViewColumnDataEventArgs) Handles QueryEmployeeRules.CustomUnboundColumnData
        Select Case e.Column.FieldName
            Case "RuleValidation"
                If e.IsGetData Then
                    If e.GetListSourceFieldValue("Id") IsNot System.DBNull.Value Then
                        Dim oItem As roScheduleRule = getSchedulerRule(roTypes.Any2Integer(e.GetListSourceFieldValue("IDRule")))
                        If oItem IsNot Nothing Then e.Value = oItem.RuleName
                    End If
                End If

        End Select
    End Sub

    Private Sub BindQueryEmployeeRules(ByVal bolReload As Boolean)
        Me.QueryEmployeeRules.DataSource = Me.QueryEmployeeScheduleRules(bolReload)
        Me.QueryEmployeeRules.DataBind()
    End Sub

    Protected Sub QueryEmployeeRules_CustomCallback(ByVal sender As Object, ByVal e As DevExpress.Web.ASPxGridViewCustomCallbackEventArgs) Handles QueryEmployeeRules.CustomCallback
        If e.Parameters = "REFRESH" Then
            BindQueryEmployeeRules(False)
            QueryEmployeeRules.JSProperties("cpAction") = "REFRESH"
        ElseIf e.Parameters = "RELOAD" Then
            BindQueryEmployeeRules(True)
            QueryEmployeeRules.JSProperties("cpAction") = "RELOAD"
        End If
    End Sub

    Private Function getSchedulerRule(ByVal oId As Integer) As roScheduleRule
        Dim oObject As roScheduleRule = Nothing

        Dim oList As Generic.List(Of roScheduleRule) = QueryEmployeeScheduleRules

        If oList IsNot Nothing AndAlso oList.Count > 0 Then
            For Each oElem As roScheduleRule In oList
                If oElem.Id = oId Then
                    oObject = oElem
                    Exit For
                End If
            Next
        End If

        Return oObject
    End Function

#End Region

End Class